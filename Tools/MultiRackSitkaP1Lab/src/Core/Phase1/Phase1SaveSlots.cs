using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SitkaP1.Core.Phase1;

/// <summary>Points inside <see cref="Phase1SaveSlots.Save"/> where the test-only fault hook is called.</summary>
public enum Phase1SlotStage
{
    TempCreated,
    TempWritten,
    TempFlushed,
    BeforeBackup,
    BeforeReplace,
}

/// <summary>
/// How far a successful save got. <see cref="DurablyCommitted"/>: the new file replaced the slot by one atomic rename and
/// the directory entry was flushed to disk. <see cref="ReplacedDurabilityUnconfirmed"/>: the rename happened (readers
/// see the new save) but flushing the directory failed, so a power loss could still roll the slot back to the previous
/// file; the caller must not report it as durably saved.
/// </summary>
public enum Phase1SlotCommitStatus
{
    DurablyCommitted,
    ReplacedDurabilityUnconfirmed,
}

public sealed record Phase1SlotSaveResult(string SlotName, Phase1SlotCommitStatus Status, bool BackupUpdated, string Checksum,
    long Bytes);

/// <summary>A read-only look at one slot: the primary and backup files' verification results and any leftover temp files.</summary>
public sealed record Phase1SlotInspection(string SlotName, bool PrimaryExists, string? PrimaryChecksum, string? PrimaryRefusal,
    bool BackupExists, string? BackupChecksum, string? BackupRefusal, IReadOnlyList<string> LeftoverTempFiles);

/// <summary>
/// Server-managed named 2D save slots in ONE dedicated directory (owner choice A: no browser-supplied path, no upload).
/// <list type="bullet">
/// <item>Slot names match <c>\A[a-z0-9][a-z0-9-]{0,47}\z</c> (no separator, dot, newline, traversal, absolute path or
/// reserved device name); every path is built from the directory and checked to stay directly inside it. The directory
/// path itself may contain no symbolic link in any component.</item>
/// <item>Reads use <see cref="NoFollowRead.ReadExact"/>: no symlink in any path component, a regular single-link file, and
/// a size bound. A symlinked slot file is refused even if it points inside the directory.</item>
/// <item>A save is verified with <see cref="Phase1SaveCodec.Decode(ReadOnlySpan{byte})"/> before a byte is written, then written
/// to a new temp file in the same directory (O_EXCL, mode 0600), fsynced, read back and compared, the current slot copied to
/// the prior-good backup (only if it still verifies; never a corrupt file over a good backup), and finally swapped in with
/// one rename(2) call (a confirmed replace) or one atomic no-clobber rename/link (a new slot), followed by a directory fsync. rename(2) within one directory is atomic on Linux: a reader or a
/// crash sees the old file or the new one, never a mixture. There is no copy-over-original fallback: if rename fails, the
/// save fails and the old slot is untouched. A slot that did NOT exist when the mutation started is always published
/// no-clobber, even when <c>replaceExisting</c> is true, so a slot another writer creates meanwhile is never lost; and
/// every mutation of one slot name is serialized inside this process (owner A0 repair NB1).</item>
/// <item>Writes are supported on Linux x64 only (the platform whose rename/fsync semantics this code relies on and that
/// the tests run on). Anywhere else a write is refused with <c>PlatformUnverifiable</c> rather than attempted.</item>
/// </list>
/// This class moves bytes only. It never activates a save in a running forest; that is A1's job under the engine lock.
/// </summary>
public sealed class Phase1SaveSlots
{
    public const string Extension = ".ccf2d.json";
    public const string BackupSuffix = ".bak";
    private const string TempInfix = ".tmp-";

    // \A and \z, never ^ and $: in .NET "$" also matches before a final "\n", so "alpha\n" would pass a ^...$ pattern.
    private static readonly Regex NamePattern = new(@"\A[a-z0-9][a-z0-9-]{0,47}\z", RegexOptions.CultureInvariant);
    private static readonly HashSet<string> Reserved = new(StringComparer.Ordinal)
    {
        "con", "prn", "aux", "nul", "com1", "com2", "com3", "com4", "com5", "com6", "com7", "com8", "com9",
        "lpt1", "lpt2", "lpt3", "lpt4", "lpt5", "lpt6", "lpt7", "lpt8", "lpt9",
    };

    private readonly Action<Phase1SlotStage>? faultHook;

    /// <summary>The dedicated slot directory (absolute, normalized, an existing real directory).</summary>
    public string Directory { get; }

    /// <param name="directory">An existing, absolute, non-symlinked directory used for nothing but these slots.</param>
    /// <param name="faultHook">Test-only fault seam called at each <see cref="Phase1SlotStage"/>; production passes null.</param>
    public Phase1SaveSlots(string directory, Action<Phase1SlotStage>? faultHook = null)
    {
        if (string.IsNullOrEmpty(directory) || !Path.IsPathFullyQualified(directory))
            throw new P1Refusal("SlotDirectoryInvalid", "the slot directory must be an absolute path");
        string full = Path.GetFullPath(directory);
        if (full.Length > 1) full = full.TrimEnd(Path.DirectorySeparatorChar);
        // Every component from the root down, not only the last: a symlinked ancestor would let the directory silently
        // resolve somewhere else (NoFollowRead re-checks the same on every read; this refuses it up front).
        for (string? at = full; !string.IsNullOrEmpty(at); at = Path.GetDirectoryName(at))
            if (new DirectoryInfo(at).LinkTarget is not null)
                throw new P1Refusal("SlotDirectoryInvalid", "the slot directory path must not contain a symbolic link (" + at + ")");
        if (!new DirectoryInfo(full).Exists) throw new P1Refusal("SlotDirectoryInvalid", "the slot directory does not exist");
        Directory = full;
        this.faultHook = faultHook;
    }

    /// <summary>
    /// Opens the server-managed slot directory, creating it (Linux mode 0700) when absent. This is the only place this app
    /// creates a directory; every check of the constructor still applies to the result.
    /// </summary>
    public static Phase1SaveSlots OpenOrCreate(string directory)
    {
        if (string.IsNullOrEmpty(directory) || !Path.IsPathFullyQualified(directory))
            throw new P1Refusal("SlotDirectoryInvalid", "the slot directory must be an absolute path");
        if (!System.IO.Directory.Exists(directory))
        {
            System.IO.Directory.CreateDirectory(directory);
            if (OperatingSystem.IsLinux())
                File.SetUnixFileMode(directory, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);
        }
        return new Phase1SaveSlots(directory);
    }

    /// <summary>Refuses any slot name outside the strict allowlist pattern.</summary>
    public static void ValidateSlotName(string? name)
    {
        if (name is null || !NamePattern.IsMatch(name) || Reserved.Contains(name))
            throw new P1Refusal("SlotNameInvalid",
                "a slot name is 1-48 characters of a-z, 0-9 and '-', starting with a letter or digit (no paths, dots or device names)");
    }

    /// <summary>The slot names present (valid names only), in ordinal order. Backup and temp files are not slots.</summary>
    public IReadOnlyList<string> ListSlots()
    {
        var names = new List<string>();
        foreach (var path in System.IO.Directory.EnumerateFiles(Directory, "*" + Extension))
        {
            string file = Path.GetFileName(path);
            if (!file.EndsWith(Extension, StringComparison.Ordinal)) continue;
            string name = file[..^Extension.Length];
            if (NamePattern.IsMatch(name) && !Reserved.Contains(name)) names.Add(name);
        }
        names.Sort(StringComparer.Ordinal);
        return names.AsReadOnly();
    }

    /// <summary>
    /// Saves verified bytes into a slot. An existing slot is replaced only with <paramref name="replaceExisting"/> (the
    /// caller's explicit confirmation); otherwise <c>SlotExists</c>. A slot that does not exist is published with an atomic
    /// no-clobber rename (renameat2 RENAME_NOREPLACE, or link(2) + unlink where the filesystem lacks it), even when
    /// <paramref name="replaceExisting"/> is true, so a slot created by another writer after the existence check is never
    /// overwritten. Mutations of one slot name are serialized inside this process. Any failure before the rename leaves the
    /// slot and its backup exactly as they were and removes this call's temp file.
    /// </summary>
    public Phase1SlotSaveResult Save(string slotName, byte[] saveBytes, bool replaceExisting)
    {
        ArgumentNullException.ThrowIfNull(saveBytes);
        NoFollowRead.RequireSupportedPlatform();
        ValidateSlotName(slotName);
        var decoded = Phase1SaveCodec.Decode(saveBytes); // never write a file that would not load
        lock (GateFor(slotName))
        {
            string primaryName = slotName + Extension;
            string primary = Confined(primaryName);
            RequireNotLinkOrDirectory(primary);
            RequireNotLinkOrDirectory(Confined(primaryName + BackupSuffix));
            bool exists = File.Exists(primary);
            if (exists && !replaceExisting)
                throw new P1Refusal("SlotExists", "slot '" + slotName + "' already holds a save; replacing it needs explicit confirmation");

            string temp = NewTempPath(primaryName);
            bool renamed = false;
            bool backupUpdated = false;
            try
            {
                WriteDurable(temp, saveBytes, hooks: true);
                // Read the temp back through the no-follow reader and compare, so what is renamed in is exactly what was verified.
                byte[] back = NoFollowRead.ReadExact(Directory, Path.GetFileName(temp), saveBytes.Length);
                if (!back.AsSpan().SequenceEqual(saveBytes))
                    throw new P1Refusal("SlotWriteVerifyFailed", "the written temp file does not match the save bytes");

                if (exists)
                {
                    faultHook?.Invoke(Phase1SlotStage.BeforeBackup);
                    backupUpdated = TryRefreshBackup(primaryName);
                }
                faultHook?.Invoke(Phase1SlotStage.BeforeReplace);
                // NB1: only a slot this call saw as existing may be replaced. An absent slot is published no-clobber even
                // when the caller confirmed replacement, so a slot another writer created meanwhile survives untouched.
                if (exists) AtomicRename(temp, primary);
                else AtomicRenameNoReplace(temp, primary, slotName);
                renamed = true;
            }
            finally
            {
                if (!renamed) TryDelete(temp);
            }
            var status = TryFsyncDirectory() ? Phase1SlotCommitStatus.DurablyCommitted : Phase1SlotCommitStatus.ReplacedDurabilityUnconfirmed;
            return new Phase1SlotSaveResult(slotName, status, backupUpdated, decoded.Checksum, saveBytes.Length);
        }
    }

    /// <summary>Reads and fully verifies the slot's primary file (with an exact identity requirement when given).</summary>
    public Phase1DecodedSave Load(string slotName, Phase1SaveExpectation? expectation)
    {
        ValidateSlotName(slotName);
        return Phase1SaveCodec.Decode(ReadFile(slotName + Extension), expectation);
    }

    /// <summary>Reads and fully verifies the slot's prior-good backup.</summary>
    public Phase1DecodedSave LoadBackup(string slotName, Phase1SaveExpectation? expectation)
    {
        ValidateSlotName(slotName);
        return Phase1SaveCodec.Decode(ReadFile(slotName + Extension + BackupSuffix), expectation);
    }

    /// <summary>
    /// Restores the prior-good backup into the primary slot (after an interrupted or corrupt save) by the same verified temp
    /// file and atomic rename. The backup itself is kept. Refused if the backup does not verify, and — because recovery
    /// replaces the primary — also refused while the primary still holds a complete, verified save
    /// (<c>SlotPrimaryValid</c>): recovery is a last resort for a damaged slot, never a way to roll a good save back, so one
    /// click can never destroy a newer save (independent review finding 2).
    /// </summary>
    public Phase1SlotSaveResult RecoverFromBackup(string slotName)
    {
        NoFollowRead.RequireSupportedPlatform();
        ValidateSlotName(slotName);
        lock (GateFor(slotName))
        {
            string primaryName = slotName + Extension;
            string primary = Confined(primaryName);
            RequireNotLinkOrDirectory(primary);
            // Verify the backup first (a corrupt backup is reported as such, as A0 pinned), then refuse while the primary
            // still verifies: recovery is a last resort for a damaged slot, never a way to roll a good save back, so one
            // click can never destroy a newer save (independent review finding 2).
            byte[] backup = ReadFile(primaryName + BackupSuffix);
            var decoded = Phase1SaveCodec.Decode(backup);
            var inspection = Inspect(slotName);
            if (inspection.PrimaryExists && inspection.PrimaryRefusal is null)
                throw new P1Refusal("SlotPrimaryValid",
                    "slot '" + slotName + "' still holds a complete, verified save; restoring a backup is only offered when its primary file is unreadable or missing");
            string temp = NewTempPath(primaryName);
            bool renamed = false;
            try
            {
                WriteDurable(temp, backup, hooks: false);
                // Same NB1 rule as Save: a primary that did not exist is published no-clobber, so a slot another process
                // creates in this window is never overwritten.
                if (inspection.PrimaryExists) AtomicRename(temp, primary);
                else AtomicRenameNoReplace(temp, primary, slotName);
                renamed = true;
            }
            finally
            {
                if (!renamed) TryDelete(temp);
            }
            var status = TryFsyncDirectory() ? Phase1SlotCommitStatus.DurablyCommitted : Phase1SlotCommitStatus.ReplacedDurabilityUnconfirmed;
            return new Phase1SlotSaveResult(slotName, status, BackupUpdated: false, decoded.Checksum, backup.Length);
        }
    }

    /// <summary>
    /// Holds this slot's mutation gate for the caller's whole read/capture/write sequence. The gate is re-entrant, so the
    /// <see cref="Save"/>, <see cref="NextGeneration"/> and <see cref="RecoverFromBackup"/> calls made inside the scope use
    /// the same gate. Without the scope, a generation read here could be stale by the time the caller writes; with it, two
    /// saves of one slot can neither interleave nor record the same generation.
    /// </summary>
    public IDisposable LockSlot(string slotName)
    {
        ValidateSlotName(slotName);
        var gate = GateFor(slotName);
        Monitor.Enter(gate);
        return new SlotScope(gate);
    }

    private sealed class SlotScope : IDisposable
    {
        private object? gate;
        public SlotScope(object gate) => this.gate = gate;
        public void Dispose()
        {
            var held = gate;
            gate = null;
            if (held is not null) Monitor.Exit(held);
        }
    }

    /// <summary>
    /// Whether the slot holds a primary and, if so, the save generation the next write of that slot should carry. Call it
    /// inside <see cref="LockSlot"/> together with the write it belongs to; the generation is only unique per slot when the
    /// read and the write share one scope.
    /// </summary>
    public (bool Exists, long NextGeneration) NextGeneration(string slotName)
    {
        ValidateSlotName(slotName);
        lock (GateFor(slotName))
        {
            if (!Inspect(slotName).PrimaryExists) return (false, 1);
            try { return (true, Load(slotName, expectation: null).Envelope.Header.SaveGeneration + 1); }
            catch (P1Refusal) { return (true, 1); } // a corrupt primary is replaced on request; its good backup survives
        }
    }

    /// <summary>Verifies the primary and backup without changing anything, and lists leftover temp files of the slot.</summary>
    public Phase1SlotInspection Inspect(string slotName)
    {
        ValidateSlotName(slotName);
        string primaryName = slotName + Extension;
        var (pExists, pSum, pErr) = Check(primaryName);
        var (bExists, bSum, bErr) = Check(primaryName + BackupSuffix);
        return new Phase1SlotInspection(slotName, pExists, pSum, pErr, bExists, bSum, bErr, LeftoverTemps(primaryName));
    }

    /// <summary>Deletes temp files an interrupted save of this slot left behind. Never touches the primary or backup.</summary>
    public int RemoveLeftoverTemps(string slotName)
    {
        ValidateSlotName(slotName);
        lock (GateFor(slotName))
        {
            var temps = LeftoverTemps(slotName + Extension);
            foreach (var file in temps) File.Delete(Confined(file));
            return temps.Count;
        }
    }

    // ------------------------------------------------------------------------------------------------ internals

    // One gate per (directory, slot name) for the life of the process. Save, RecoverFromBackup and RemoveLeftoverTemps
    // hold it for their whole mutation, so two requests for the same slot can never interleave their temp/backup/rename
    // steps. It is keyed by the full primary path, so two Phase1SaveSlots over different directories never share a gate.
    private static readonly ConcurrentDictionary<string, object> SlotGates = new(StringComparer.Ordinal);

    private object GateFor(string slotName) => SlotGates.GetOrAdd(Confined(slotName + Extension), static _ => new object());

    private (bool Exists, string? Checksum, string? Refusal) Check(string fileName)
    {
        string path = Confined(fileName);
        if (System.IO.Directory.Exists(path) && new FileInfo(path).LinkTarget is null) return (true, null, "SlotNotAFile");
        if (!File.Exists(path) && new FileInfo(path).LinkTarget is null) return (false, null, null);
        try { return (true, Phase1SaveCodec.Decode(ReadFile(fileName)).Checksum, null); }
        catch (P1Refusal e) { return (true, null, e.Code); }
    }

    private IReadOnlyList<string> LeftoverTemps(string primaryName) =>
        System.IO.Directory.EnumerateFiles(Directory, primaryName + TempInfix + "*")
            .Select(path => Path.GetFileName(path))
            .Where(f => f.StartsWith(primaryName + TempInfix, StringComparison.Ordinal))
            .Order(StringComparer.Ordinal).ToArray();

    // The backup is refreshed from the current primary only when that primary still verifies, by its own temp + rename.
    private bool TryRefreshBackup(string primaryName)
    {
        byte[] current;
        try
        {
            current = ReadFile(primaryName);
            Phase1SaveCodec.Decode(current);
        }
        catch (P1Refusal)
        {
            return false; // keep the existing backup: a corrupt primary must never displace a good backup
        }
        string temp = NewTempPath(primaryName);
        bool renamed = false;
        try
        {
            WriteDurable(temp, current, hooks: false);
            AtomicRename(temp, Confined(primaryName + BackupSuffix));
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
        if (info.LinkTarget is not null) throw new P1Refusal("SlotSymlinkRefused", "a slot file must not be a symbolic link");
        if (System.IO.Directory.Exists(path)) throw new P1Refusal("SlotNotAFile", "the slot path is not a regular file");
        if (!info.Exists) throw new P1Refusal("SlotMissing", "the slot file '" + fileName + "' does not exist");
        if (info.Length > Phase1SaveFormat.MaxSaveBytes)
            throw new P1Refusal("SaveTooLarge", "the slot file exceeds the save size limit");
        // ReadExact re-checks on the open descriptor: no symlink anywhere in the path, regular file, one link, exact size.
        return NoFollowRead.ReadExact(Directory, fileName, info.Length);
    }

    private void WriteDurable(string path, byte[] bytes, bool hooks)
    {
        // Callers already refused non-Linux platforms; this repeat is the guard the platform analyzer can see.
        if (!OperatingSystem.IsLinux()) throw new P1Refusal("PlatformUnverifiable", "slot writes are implemented for Linux x64 only");
        using var stream = new FileStream(path, new FileStreamOptions
        {
            Mode = FileMode.CreateNew, // O_CREAT|O_EXCL: never opens an existing file or follows a planted symlink
            Access = FileAccess.Write,
            Share = FileShare.None,
            UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite,
        });
        if (hooks) faultHook?.Invoke(Phase1SlotStage.TempCreated);
        stream.Write(bytes);
        if (hooks) faultHook?.Invoke(Phase1SlotStage.TempWritten);
        stream.Flush(flushToDisk: true); // fsync: the bytes are on disk before the rename can expose them
        if (hooks) faultHook?.Invoke(Phase1SlotStage.TempFlushed);
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
        if (new FileInfo(path).LinkTarget is not null) throw new P1Refusal("SlotSymlinkRefused", "a slot file must not be a symbolic link");
        if (System.IO.Directory.Exists(path)) throw new P1Refusal("SlotNotAFile", "the slot path is not a regular file");
    }

    private string NewTempPath(string primaryName) =>
        Confined(primaryName + TempInfix + Convert.ToHexString(RandomNumberGenerator.GetBytes(8)).ToLowerInvariant());

    private static void TryDelete(string path)
    {
        try { File.Delete(path); }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    // rename(2) directly, never File.Move: File.Move may fall back to copy + delete across devices, which is the unsafe
    // copy-over-original this store must never do. Both paths are in the same directory, so EXDEV cannot apply anyway.
    private static void AtomicRename(string from, string to)
    {
        if (Native.Rename(from, to) != 0)
            throw new P1Refusal("SlotRenameFailed", FormattableString.Invariant(
                $"atomic rename failed (errno {Marshal.GetLastPInvokeError()}); the previous slot is unchanged"));
    }

    // No-clobber publish for a save without replace confirmation. renameat2(RENAME_NOREPLACE) is one atomic step that fails
    // with EEXIST if the slot appeared meanwhile. Filesystems without it return EINVAL (or glibc lacks the symbol); then
    // link(2) — also atomic and also EEXIST on an existing name — publishes the same inode and the temp name is unlinked.
    // Between link and unlink the slot file briefly has two links, so a concurrent no-follow read of it may refuse
    // (HardLinked) for that instant; it never sees partial or wrong bytes.
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
        // The slot now holds the verified bytes; dropping the temp name cannot change them. A failed unlink leaves a
        // reported, removable leftover temp file (RemoveLeftoverTemps), not a wrong slot.
        TryDelete(from);
    }

    private static P1Refusal SlotExists(string slotName) =>
        new("SlotExists", "slot '" + slotName + "' already holds a save; replacing it needs explicit confirmation");

    // fsync on the directory makes the rename itself durable (the directory entry), not just the file's bytes.
    private bool TryFsyncDirectory()
    {
        int fd = Native.Open(Directory, Native.O_RDONLY | Native.O_DIRECTORY | Native.O_CLOEXEC);
        if (fd < 0) return false;
        try { return Native.Fsync(fd) == 0; }
        finally { Native.Close(fd); }
    }

    private static class Native
    {
        // Linux x64 values (same as NoFollowRead); writes are refused on any other platform before these are reached.
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
