using System.Runtime.InteropServices;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Core.Saves;

/// <summary>Inspection summary of a named save slot supporting both frozen v2 and strict C-V3 saves.</summary>
internal sealed record MultiSlotInspection(
    string SlotName,
    bool PrimaryExists,
    int? PrimarySchemaVersion,
    string? PrimaryChecksum,
    string? PrimaryRefusal,
    bool BackupExists,
    int? BackupSchemaVersion,
    string? BackupChecksum,
    string? BackupRefusal);

/// <summary>
/// Server-managed named 2D save slots supporting both frozen Save v2 (delegated byte-for-byte to
/// <see cref="Phase1SaveSlots"/>) and Save v3 (C-V3) in the same dedicated <c>ccf-multi-2d-saves</c> folder.
/// Preserves every durability and confinement invariant of <see cref="Phase1SaveSlots"/>: strict slot-name allowlist,
/// no-follow descriptor reads, atomic <c>O_CREAT|O_EXCL</c> temp + <c>fsync</c> + read-back verification + prior-good
/// backup refresh + atomic <c>rename(2)</c> / <c>renameat2(RENAME_NOREPLACE)</c> + directory <c>fsync</c>.
/// Never rewrites a v2 file in place.
/// </summary>
internal sealed class SaveV3Slots
{
    private const string TempInfix = ".tmp-v3-";
    private static long tempSequence;

    private readonly Phase1SaveSlots inner;

    internal SaveV3Slots(Phase1SaveSlots inner)
    {
        ArgumentNullException.ThrowIfNull(inner);
        this.inner = inner;
    }

    internal string Directory => inner.Directory;

    internal IReadOnlyList<string> ListSlots() => inner.ListSlots();

    internal IDisposable LockSlot(string slotName) => inner.LockSlot(slotName);

    internal (bool Exists, long NextGeneration) NextGeneration(string slotName)
    {
        Phase1SaveSlots.ValidateSlotName(slotName);
        using (inner.LockSlot(slotName))
        {
            var info = Inspect(slotName);
            if (!info.PrimaryExists) return (false, 1);
            try { return (true, Load(slotName, v2Expectation: null).Header.SaveGeneration + 1); }
            catch (P1Refusal) { return (true, 1); }
        }
    }

    internal Phase1SlotSaveResult Save(string slotName, byte[] saveBytes, bool replaceExisting)
    {
        ArgumentNullException.ThrowIfNull(saveBytes);
        var decoded = SaveV3Codec.DecodeAny(saveBytes);
        if (decoded.SchemaVersion == Phase1SaveFormat.SchemaVersion)
            return inner.Save(slotName, saveBytes, replaceExisting);

        NoFollowRead.RequireSupportedPlatform();
        Phase1SaveSlots.ValidateSlotName(slotName);

        using (inner.LockSlot(slotName))
        {
            string primaryName = slotName + Phase1SaveSlots.Extension;
            string primary = Confined(primaryName);
            RequireNotLinkOrDirectory(primary);
            RequireNotLinkOrDirectory(Confined(primaryName + Phase1SaveSlots.BackupSuffix));

            bool exists = File.Exists(primary);
            if (exists && !replaceExisting)
                throw new P1Refusal("SlotExists",
                    "slot '" + slotName + "' already holds a save; replacing it needs explicit confirmation");

            string temp = NewTempPath(primaryName);
            bool renamed = false;
            bool backupUpdated = false;
            try
            {
                WriteDurable(temp, saveBytes);
                byte[] back = NoFollowRead.ReadExact(Directory, Path.GetFileName(temp), saveBytes.Length);
                if (!back.AsSpan().SequenceEqual(saveBytes))
                    throw new P1Refusal("SlotWriteVerifyFailed", "the written temp file does not match the save bytes");

                if (exists)
                    backupUpdated = TryRefreshBackup(primaryName);

                if (exists) AtomicRename(temp, primary);
                else AtomicRenameNoReplace(temp, primary, slotName);
                renamed = true;
            }
            finally
            {
                if (!renamed) TryDelete(temp);
            }

            var status = TryFsyncDirectory()
                ? Phase1SlotCommitStatus.DurablyCommitted
                : Phase1SlotCommitStatus.ReplacedDurabilityUnconfirmed;
            return new Phase1SlotSaveResult(slotName, status, backupUpdated, decoded.Checksum, saveBytes.Length);
        }
    }

    internal MultiDecodedSave Load(string slotName, Phase1SaveExpectation? v2Expectation, FeatureBlockRegistry? registry = null)
    {
        Phase1SaveSlots.ValidateSlotName(slotName);
        byte[] bytes = ReadFile(slotName + Phase1SaveSlots.Extension);
        return SaveV3Codec.DecodeAny(bytes, v2Expectation, registry);
    }

    internal MultiSlotInspection Inspect(string slotName)
    {
        Phase1SaveSlots.ValidateSlotName(slotName);
        string primaryName = slotName + Phase1SaveSlots.Extension;
        var (pExists, pVer, pSum, pErr) = Check(primaryName);
        var (bExists, bVer, bSum, bErr) = Check(primaryName + Phase1SaveSlots.BackupSuffix);
        return new MultiSlotInspection(slotName, pExists, pVer, pSum, pErr, bExists, bVer, bSum, bErr);
    }

    internal Phase1SlotSaveResult RecoverFromBackup(string slotName)
    {
        NoFollowRead.RequireSupportedPlatform();
        Phase1SaveSlots.ValidateSlotName(slotName);
        using (inner.LockSlot(slotName))
        {
            string primaryName = slotName + Phase1SaveSlots.Extension;
            string primary = Confined(primaryName);
            RequireNotLinkOrDirectory(primary);

            byte[] backup = ReadFile(primaryName + Phase1SaveSlots.BackupSuffix);
            var decoded = SaveV3Codec.DecodeAny(backup);
            var inspection = Inspect(slotName);
            if (inspection.PrimaryExists && inspection.PrimaryRefusal is null)
                throw new P1Refusal("SlotPrimaryValid",
                    "slot '" + slotName + "' still holds a complete, verified save; restoring a backup is only offered when its primary file is unreadable or missing");

            if (decoded.SchemaVersion == Phase1SaveFormat.SchemaVersion)
                return inner.RecoverFromBackup(slotName);

            string temp = NewTempPath(primaryName);
            bool renamed = false;
            try
            {
                WriteDurable(temp, backup);
                if (inspection.PrimaryExists) AtomicRename(temp, primary);
                else AtomicRenameNoReplace(temp, primary, slotName);
                renamed = true;
            }
            finally
            {
                if (!renamed) TryDelete(temp);
            }

            var status = TryFsyncDirectory()
                ? Phase1SlotCommitStatus.DurablyCommitted
                : Phase1SlotCommitStatus.ReplacedDurabilityUnconfirmed;
            return new Phase1SlotSaveResult(slotName, status, BackupUpdated: false, decoded.Checksum, backup.Length);
        }
    }

    // ------------------------------------------------------------------------------------------------ internals

    private (bool Exists, int? SchemaVersion, string? Checksum, string? Refusal) Check(string fileName)
    {
        string path = Confined(fileName);
        if (System.IO.Directory.Exists(path) && new FileInfo(path).LinkTarget is null)
            return (true, null, null, "SlotNotAFile");
        if (!File.Exists(path) && new FileInfo(path).LinkTarget is null)
            return (false, null, null, null);
        try
        {
            var decoded = SaveV3Codec.DecodeAny(ReadFile(fileName));
            return (true, decoded.SchemaVersion, decoded.Checksum, null);
        }
        catch (P1Refusal e)
        {
            return (true, null, null, e.Code);
        }
    }

    private bool TryRefreshBackup(string primaryName)
    {
        byte[] current;
        try
        {
            current = ReadFile(primaryName);
            _ = SaveV3Codec.DecodeAny(current);
        }
        catch (P1Refusal)
        {
            return false;
        }
        string temp = NewTempPath(primaryName);
        bool renamed = false;
        try
        {
            WriteDurable(temp, current);
            AtomicRename(temp, Confined(primaryName + Phase1SaveSlots.BackupSuffix));
            renamed = true;
        }
        finally
        {
            if (!renamed) TryDelete(temp);
        }
        return true;
    }

    private byte[] ReadFile(string fileName)
    {
        string path = Confined(fileName);
        var info = new FileInfo(path);
        if (info.LinkTarget is not null)
            throw new P1Refusal("SlotSymlinkRefused", "a slot file must not be a symbolic link");
        if (System.IO.Directory.Exists(path))
            throw new P1Refusal("SlotNotAFile", "the slot path is not a regular file");
        if (!info.Exists)
            throw new P1Refusal("SlotMissing", "the slot file '" + fileName + "' does not exist");
        if (info.Length > SaveV3Format.MaxSaveBytes)
            throw new P1Refusal("SaveTooLarge", "the slot file exceeds the save size limit");
        return NoFollowRead.ReadExact(Directory, fileName, info.Length);
    }

    private static void WriteDurable(string path, byte[] bytes)
    {
        if (!OperatingSystem.IsLinux())
            throw new P1Refusal("PlatformUnverifiable", "slot writes are implemented for Linux x64 only");
        using var stream = new FileStream(path, new FileStreamOptions
        {
            Mode = FileMode.CreateNew,
            Access = FileAccess.Write,
            Share = FileShare.None,
            UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite,
        });
        stream.Write(bytes);
        stream.Flush(flushToDisk: true);
    }

    private string Confined(string fileName)
    {
        string path = Path.GetFullPath(Path.Combine(Directory, fileName));
        if (!string.Equals(Path.GetDirectoryName(path), Directory, StringComparison.Ordinal) ||
            !string.Equals(Path.GetFileName(path), fileName, StringComparison.Ordinal))
            throw new P1Refusal("SlotPathEscape", "a slot path must stay directly inside the slot directory");
        return path;
    }

    private static void RequireNotLinkOrDirectory(string path)
    {
        if (new FileInfo(path).LinkTarget is not null)
            throw new P1Refusal("SlotSymlinkRefused", "a slot file must not be a symbolic link");
        if (System.IO.Directory.Exists(path))
            throw new P1Refusal("SlotNotAFile", "the slot path is not a regular file");
    }

    private string NewTempPath(string primaryName)
    {
        long seq = Interlocked.Increment(ref tempSequence);
        return Confined(FormattableString.Invariant($"{primaryName}{TempInfix}{Environment.ProcessId}-{seq}"));
    }

    private static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private static void AtomicRename(string from, string to)
    {
        if (Native.Rename(from, to) != 0)
            throw new P1Refusal("SlotRenameFailed", FormattableString.Invariant(
                $"atomic rename failed (errno {Marshal.GetLastPInvokeError()}); the previous slot is unchanged"));
    }

    private static void AtomicRenameNoReplace(string from, string to, string slotName)
    {
        int result, errno;
        try
        {
            result = Native.RenameAt2(Native.AT_FDCWD, from, Native.AT_FDCWD, to, Native.RENAME_NOREPLACE);
            errno = result == 0 ? 0 : Marshal.GetLastPInvokeError();
        }
        catch (EntryPointNotFoundException)
        {
            (result, errno) = (-1, Native.ENOSYS);
        }
        if (result == 0) return;
        if (errno == Native.EEXIST) throw SlotExists(slotName);
        if (errno is not (Native.EINVAL or Native.ENOSYS))
            throw new P1Refusal("SlotRenameFailed", FormattableString.Invariant(
                $"atomic no-replace rename failed (errno {errno}); the slot is unchanged"));

        if (Native.Link(from, to) != 0)
        {
            int linkErrno = Marshal.GetLastPInvokeError();
            if (linkErrno == Native.EEXIST) throw SlotExists(slotName);
            throw new P1Refusal("SlotRenameFailed", FormattableString.Invariant(
                $"atomic no-replace link failed (errno {linkErrno}); the slot is unchanged"));
        }
        TryDelete(from);
    }

    private static P1Refusal SlotExists(string slotName) =>
        new("SlotExists", "slot '" + slotName + "' already holds a save; replacing it needs explicit confirmation");

    private bool TryFsyncDirectory()
    {
        int fd = Native.Open(Directory, Native.O_RDONLY | Native.O_DIRECTORY | Native.O_CLOEXEC);
        if (fd < 0) return false;
        try { return Native.Fsync(fd) == 0; }
        finally { Native.Close(fd); }
    }

    private static class Native
    {
        public const int O_RDONLY = 0, O_DIRECTORY = 0x10000, O_CLOEXEC = 0x80000;
        public const int AT_FDCWD = -100;
        public const uint RENAME_NOREPLACE = 1;
        public const int EEXIST = 17, EINVAL = 22, ENOSYS = 38;

        [DllImport("libc", EntryPoint = "rename", SetLastError = true)]
        public static extern int Rename([MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);

        [DllImport("libc", EntryPoint = "renameat2", SetLastError = true)]
        public static extern int RenameAt2(int oldDirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, int newDirFd,
            [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath, uint flags);

        [DllImport("libc", EntryPoint = "link", SetLastError = true)]
        public static extern int Link([MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);

        [DllImport("libc", EntryPoint = "open", SetLastError = true)]
        public static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);

        [DllImport("libc", EntryPoint = "fsync", SetLastError = true)]
        public static extern int Fsync(int fd);

        [DllImport("libc", EntryPoint = "close", SetLastError = true)]
        public static extern int Close(int fd);
    }
}
