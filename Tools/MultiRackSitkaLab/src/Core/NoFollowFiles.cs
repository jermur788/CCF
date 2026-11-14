using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace SitkaLab.Core;

/// <summary>What a name inside the lab data folder really is, judged without following links and without opening it.</summary>
public enum LabEntryKind { Missing, Regular, HardLinked, Symlink, Directory, Fifo, Other }

/// <summary>Points inside one save transaction where a test may inject a hostile folder swap (MULTI-006). Production never sets a hook.</summary>
internal enum WriteStage { DirectoryOpened, TempWritten }

/// <summary>
/// Handle-anchored, no-follow file access for the lab data folder (MULTI-004 F-1/F-2).
///
/// Why this exists: checking a name and then opening it by path are two separate steps, so another process of the same user can
/// swap the entry for a symlink (or a FIFO) in between. Here the open itself is the check:
///   1. the lab folder is opened once with O_NOFOLLOW|O_DIRECTORY, giving a directory handle;
///   2. the file is opened relative to THAT handle (openat) with O_NOFOLLOW, so a symlink in the last path component is refused by
///      the kernel and the name can never resolve outside the folder;
///   3. O_NONBLOCK means opening a FIFO returns immediately instead of waiting for a writer;
///   4. the type, link count and size are then read from the OPEN descriptor (statx on the fd), so the bytes that are read belong
///      to exactly the object that was checked. Only a regular file with a single link is ever read, and reading is size-capped.
///
/// Saves (MULTI-006 R-1) use the same idea: the folder is opened ONCE and that one handle is used for every step of the save —
/// create the temp file (openat O_CREAT|O_EXCL|O_NOFOLLOW), write it, rename it over the final name (renameat) and, if anything
/// fails, remove that temp file (unlinkat). The folder path is never looked up again during the save, so swapping the folder for a
/// link to another folder half-way through cannot redirect any step of it.
///
/// Platform restriction (deliberate, fail-closed): this is implemented for Linux on x64 and arm64 only, where the O_* flag values
/// below come from the kernel headers. The first use runs a tiny self-test in a private temp folder (symlink refused, FIFO opens
/// without blocking, regular file accepted). If the platform is anything else, or the self-test fails, every call throws
/// <c>PlatformUnsupported</c> instead of falling back to a weaker path-based check. arm64 values are taken from the headers and
/// guarded by the self-test; they have not been executed on arm64 hardware by this task.
/// </summary>
internal static class NoFollowFiles
{
    // ---- Linux constants (include/uapi/asm-generic and arch/arm64 fcntl.h, linux/stat.h) -------------------------
    private const int AT_FDCWD = -100;
    private const int AT_SYMLINK_NOFOLLOW = 0x100;
    private const int AT_EMPTY_PATH = 0x1000;
    private const uint STATX_TYPE = 0x1, STATX_NLINK = 0x4, STATX_SIZE = 0x200, STATX_BASIC_STATS = 0x7ff;
    private const int O_RDONLY = 0, O_WRONLY = 1, O_CREAT = 0x40, O_EXCL = 0x80, O_NONBLOCK = 0x800, O_CLOEXEC = 0x80000;   // same on x64 and arm64
    private const uint Mode0600 = 0x180;
    private const int S_IFMT = 0xF000, S_IFREG = 0x8000, S_IFDIR = 0x4000, S_IFLNK = 0xA000, S_IFIFO = 0x1000;
    private const int EINTR = 4, ENOENT = 2, ENXIO = 6, EEXIST = 17, ENOTDIR = 20, ELOOP = 40;

    // x86 / generic and arm64 differ for these two flags.
    private static int ODirectory => RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? 0x4000 : 0x10000;
    private static int ONoFollow => RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? 0x8000 : 0x20000;

    [DllImport("libc", EntryPoint = "openat", SetLastError = true)]
    private static extern int OpenAt(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);
    // openat is variadic in C; the mode argument is a plain int-class register argument on Linux x64 and arm64, so this fixed signature is ABI-compatible there.
    [DllImport("libc", EntryPoint = "openat", SetLastError = true)]
    private static extern int OpenAtCreate(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, uint mode);
    [DllImport("libc", EntryPoint = "write", SetLastError = true)]
    private static extern IntPtr Write(int fd, ref byte buffer, UIntPtr count);
    [DllImport("libc", EntryPoint = "renameat", SetLastError = true)]
    private static extern int RenameAt(int oldDirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, int newDirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);
    [DllImport("libc", EntryPoint = "unlinkat", SetLastError = true)]
    private static extern int UnlinkAt(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);
    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);
    [DllImport("libc", EntryPoint = "read", SetLastError = true)]
    private static extern IntPtr Read(int fd, [Out] byte[] buffer, UIntPtr count);
    [DllImport("libc", EntryPoint = "statx", SetLastError = true)]
    private static extern int StatX(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, uint mask, [Out] byte[] statxBuf);
    [DllImport("libc", EntryPoint = "mkfifo", SetLastError = true)]
    private static extern int MkFifo([MarshalAs(UnmanagedType.LPUTF8Str)] string path, uint mode);

    private static readonly Lazy<string?> Support = new(SelfTest, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Throws <c>PlatformUnsupported</c> unless the no-follow primitives are present and demonstrably working.</summary>
    internal static void RequireSupported()
    {
        var why = Support.Value;
        if (why is not null)
            throw new LabException("PlatformUnsupported", "lab file access needs Linux (x64/arm64) no-follow support and refuses to run without it: " + why);
    }

    // ---- public surface used by RealLabFileAccess -----------------------------------------------------------------
    internal static LabEntryKind Classify(string dir, string name)
    {
        RequireSupported();
        int dfd = OpenDirectory(dir);
        try
        {
            var buf = new byte[256];
            if (StatX(dfd, name, AT_SYMLINK_NOFOLLOW, STATX_BASIC_STATS, buf) != 0)
                return Marshal.GetLastPInvokeError() == ENOENT ? LabEntryKind.Missing : LabEntryKind.Other;
            return KindOf(buf);
        }
        finally { Close(dfd); }
    }

    internal static byte[] ReadRegular(string dir, string name, long maxBytes)
    {
        RequireSupported();
        int dfd = OpenDirectory(dir);
        try
        {
            int fd = OpenAt(dfd, name, O_RDONLY | ONoFollow | O_NONBLOCK | O_CLOEXEC);
            if (fd < 0)
            {
                int errno = Marshal.GetLastPInvokeError();
                throw errno switch
                {
                    ELOOP => new LabException("SymlinkRejected", "symbolic links are refused"),
                    ENOENT => new LabException("NotListed", "that file is not in the lab file list"),
                    ENXIO => new LabException("NotRegularFile", "not a regular file"),
                    _ => new LabException("ReadFailed", "the lab file could not be opened"),
                };
            }
            try
            {
                var buf = new byte[256];
                if (StatX(fd, "", AT_EMPTY_PATH, STATX_BASIC_STATS, buf) != 0) throw new LabException("ReadFailed", "the lab file could not be inspected");
                switch (KindOf(buf))
                {
                    case LabEntryKind.Regular: break;
                    case LabEntryKind.HardLinked: throw new LabException("HardLinkRejected", "files with more than one hard link are refused");
                    case LabEntryKind.Missing: throw new LabException("NotListed", "that file is not in the lab file list");
                    default: throw new LabException("NotRegularFile", "not a regular file");
                }
                if (BinaryPrimitives.ReadUInt64LittleEndian(buf.AsSpan(40)) > (ulong)maxBytes)
                    throw new LabException("FileTooLarge", "file exceeds the size limit");
                return ReadAll(fd, maxBytes);
            }
            finally { Close(fd); }
        }
        finally { Close(dfd); }
    }

    /// <summary>
    /// Atomically saves <paramref name="bytes"/> as <paramref name="name"/> inside <paramref name="dir"/> (MULTI-006 R-1).
    /// <paramref name="dir"/> is resolved exactly once, no-follow; every later step is relative to that one directory handle:
    ///   openat(dfd, temp, O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC, 0600) → write every byte → close → renameat(dfd, temp, dfd, name).
    /// If any step after the temp file exists fails, unlinkat(dfd, temp) removes exactly that temp file — never another name, never in
    /// another folder. A failure is always a <see cref="LabException"/> and means the final name was not changed by this call.
    /// renameat replaces the directory ENTRY, so a symlink or FIFO squatting the final name is replaced, never written through; a folder
    /// squatting it makes the rename fail (WriteFailed) and the folder is left alone.
    /// </summary>
    internal static void WriteAtomic(string dir, string name, byte[] bytes, Action<WriteStage>? stageHook = null)
    {
        RequireSupported();
        if (string.IsNullOrEmpty(name) || name is "." or ".." || name.Contains('/') || name.Contains('\0'))
            throw new LabException("PathRejected", "only a bare lab file name can be saved");
        int dfd = OpenDirectory(dir, forWrite: true);
        try
        {
            stageHook?.Invoke(WriteStage.DirectoryOpened);
            string tmp = name + ".tmp-" + Guid.NewGuid().ToString("N");
            int fd = OpenAtCreate(dfd, tmp, O_WRONLY | O_CREAT | O_EXCL | ONoFollow | O_CLOEXEC, Mode0600);
            if (fd < 0) throw SaveFailed();                   // nothing was created, so there is nothing to clean up
            bool renamed = false;
            try
            {
                LabException? failure = null;
                try { WriteAll(fd, bytes); } catch (LabException ex) { failure = ex; }
                if (Close(fd) != 0) failure ??= SaveFailed();   // close can report a deferred write error
                if (failure is not null) throw failure;
                stageHook?.Invoke(WriteStage.TempWritten);
                if (RenameAt(dfd, tmp, dfd, name) != 0) throw SaveFailed();
                renamed = true;
            }
            finally
            {
                // Same handle, same unique name: only this call's own temp file can be removed. Best effort (result ignored).
                if (!renamed) UnlinkAt(dfd, tmp, 0);
            }
        }
        finally { Close(dfd); }
    }

    private static LabException SaveFailed() => new("WriteFailed", "the save could not be written; the existing entry was left unchanged");

    // ---- internals ---------------------------------------------------------------------------------------------------
    private static int OpenDirectory(string dir, bool forWrite = false)
    {
        int fd = OpenAt(AT_FDCWD, dir, O_RDONLY | ODirectory | ONoFollow | O_CLOEXEC);
        if (fd >= 0) return fd;
        int errno = Marshal.GetLastPInvokeError();
        throw errno switch
        {
            ELOOP or ENOTDIR => new LabException("SymlinkRejected", "the lab data root is a symbolic link or not a directory and is refused"),
            _ when forWrite => SaveFailed(),
            ENOENT => new LabException("RootMissing", "lab data root does not exist"),
            _ => new LabException("ReadFailed", "the lab data root could not be opened"),
        };
    }

    /// <summary>Writes every byte, retrying on EINTR and continuing after partial writes; a write that makes no progress is a failure, never a spin.</summary>
    private static void WriteAll(int fd, byte[] bytes)
    {
        int offset = 0;
        while (offset < bytes.Length)
        {
            long n = Write(fd, ref bytes[offset], (UIntPtr)(uint)(bytes.Length - offset)).ToInt64();
            if (n < 0)
            {
                if (Marshal.GetLastPInvokeError() == EINTR) continue;
                throw SaveFailed();
            }
            if (n == 0) throw SaveFailed();
            offset += (int)n;
        }
    }

    /// <summary>Interprets a statx buffer (layout is architecture independent): mask@0, nlink@16, mode@28, size@40.</summary>
    private static LabEntryKind KindOf(byte[] b)
    {
        uint mask = BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(0));
        if ((mask & (STATX_TYPE | STATX_NLINK | STATX_SIZE)) != (STATX_TYPE | STATX_NLINK | STATX_SIZE)) return LabEntryKind.Other; // incomplete answer: fail closed
        uint nlink = BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(16));
        int type = BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(28)) & S_IFMT;
        return type switch
        {
            // > 1 means the inode is reachable under another name. 0 is possible only on an OPEN handle whose name was just replaced; nothing else can reach it, so it is not a hard link.
            S_IFREG => nlink > 1 ? LabEntryKind.HardLinked : LabEntryKind.Regular,
            S_IFLNK => LabEntryKind.Symlink,
            S_IFDIR => LabEntryKind.Directory,
            S_IFIFO => LabEntryKind.Fifo,
            _ => LabEntryKind.Other,
        };
    }

    private static byte[] ReadAll(int fd, long maxBytes)
    {
        using var ms = new MemoryStream();
        var chunk = new byte[64 * 1024];
        while (true)
        {
            long n = Read(fd, chunk, (UIntPtr)(uint)chunk.Length).ToInt64();
            if (n < 0)
            {
                if (Marshal.GetLastPInvokeError() == EINTR) continue;
                throw new LabException("ReadFailed", "the lab file could not be read");
            }
            if (n == 0) break;
            ms.Write(chunk, 0, (int)n);
            if (ms.Length > maxBytes) throw new LabException("FileTooLarge", "file exceeds the size limit");   // the file grew after it was inspected
        }
        return ms.ToArray();
    }

    /// <summary>Returns null when the primitives behave exactly as this class assumes; otherwise a short reason.</summary>
    private static string? SelfTest()
    {
        if (!OperatingSystem.IsLinux()) return "not Linux";
        if (RuntimeInformation.ProcessArchitecture is not (Architecture.X64 or Architecture.Arm64)) return "unsupported CPU architecture " + RuntimeInformation.ProcessArchitecture;
        string? tmp = null;
        try
        {
            tmp = Directory.CreateTempSubdirectory("sitkalab-nofollow-selftest-").FullName;
            File.WriteAllBytes(Path.Combine(tmp, "reg"), new byte[] { 1 });
            File.CreateSymbolicLink(Path.Combine(tmp, "lnk"), "reg");
            if (MkFifo(Path.Combine(tmp, "fifo"), 0x180 /* 0600 */) != 0) return "mkfifo failed";
            int d = OpenAt(AT_FDCWD, tmp, O_RDONLY | ODirectory | ONoFollow | O_CLOEXEC);
            if (d < 0) return "cannot open a directory with O_NOFOLLOW|O_DIRECTORY";
            try
            {
                // order matters: the symlink check comes first, so a wrong flag value fails here rather than at the FIFO
                int l = OpenAt(d, "lnk", O_RDONLY | ONoFollow | O_NONBLOCK | O_CLOEXEC);
                if (l >= 0) { Close(l); return "O_NOFOLLOW did not refuse a symbolic link"; }
                if (Marshal.GetLastPInvokeError() != ELOOP) return "symlink open failed with an unexpected errno";
                if (OpenAt(d, "reg", O_RDONLY | ODirectory | ONoFollow | O_CLOEXEC) is var notDir && notDir >= 0) { Close(notDir); return "O_DIRECTORY accepted a regular file"; }
                if (FdKind(d, "reg") != LabEntryKind.Regular) return "regular file not recognised";
                if (FdKind(d, "fifo") != LabEntryKind.Fifo) return "FIFO not recognised (or its open blocked)";
                if (FdKind(d, "lnk") != LabEntryKind.Symlink) return "symlink not recognised by statx";
                // the save primitives (MULTI-006): exclusive no-follow create, write, rename and unlink, all relative to the handle
                if (OpenAtCreate(d, "lnk", O_WRONLY | O_CREAT | O_EXCL | ONoFollow | O_CLOEXEC, Mode0600) is var viaLink && viaLink >= 0) { Close(viaLink); return "O_CREAT|O_EXCL accepted an existing symlink"; }
                if (Marshal.GetLastPInvokeError() != EEXIST) return "exclusive create on a symlink failed with an unexpected errno";
                int w = OpenAtCreate(d, "w", O_WRONLY | O_CREAT | O_EXCL | ONoFollow | O_CLOEXEC, Mode0600);
                if (w < 0) return "cannot create a file relative to a directory handle";
                try { WriteAll(w, new byte[] { 7 }); } catch (LabException) { return "write to a created file failed"; } finally { Close(w); }
                if (RenameAt(d, "w", d, "w2") != 0) return "renameat failed";
                if (FdKind(d, "w2") != LabEntryKind.Regular) return "renamed file not found under its new name";
                if (UnlinkAt(d, "w2", 0) != 0) return "unlinkat failed";
                if (StatX(d, "w2", AT_SYMLINK_NOFOLLOW, STATX_BASIC_STATS, new byte[256]) == 0 || Marshal.GetLastPInvokeError() != ENOENT) return "unlinkat did not remove the file";
            }
            finally { Close(d); }
            return null;
        }
        catch (Exception ex) { return ex.GetType().Name + ": " + ex.Message; }   // e.g. libc or statx not found
        finally { try { if (tmp is not null) Directory.Delete(tmp, true); } catch { /* best effort */ } }
    }

    /// <summary>Self-test helper: open relative to the directory handle exactly like ReadRegular does and classify the OPEN descriptor.</summary>
    private static LabEntryKind FdKind(int dirFd, string name)
    {
        var buf = new byte[256];
        if (name == "lnk")
        {
            if (StatX(dirFd, name, AT_SYMLINK_NOFOLLOW, STATX_BASIC_STATS, buf) != 0) return LabEntryKind.Other;
            return KindOf(buf);
        }
        int fd = OpenAt(dirFd, name, O_RDONLY | ONoFollow | O_NONBLOCK | O_CLOEXEC);
        if (fd < 0) return LabEntryKind.Other;
        try { return StatX(fd, "", AT_EMPTY_PATH, STATX_BASIC_STATS, buf) != 0 ? LabEntryKind.Other : KindOf(buf); }
        finally { Close(fd); }
    }
}
