using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace SitkaP1.Core;

/// <summary>
/// Read-only, descriptor-anchored, no-follow file reading for the original-P0 import (spec §9.2 T-02 steps 2–4).
/// P1's own implementation; it is not a copy of P0's NoFollowFiles and is not assumed to inherit P0's qualification.
///
///   • Every component of the directory path is opened relative to the previous handle with O_NOFOLLOW|O_DIRECTORY, starting
///     at "/", so no symlinked directory anywhere in the path is followed.
///   • The file is opened relative to that directory handle with O_RDONLY|O_NOFOLLOW|O_NONBLOCK|O_CLOEXEC (a FIFO opens
///     without blocking and is then refused).
///   • Type, link count and size come from statx on the OPEN descriptor; only a regular single-link file of the exact
///     expected size is read, and at most expected+1 bytes are read so growth underneath is detected.
///
/// Linux x64 only. Any other platform is refused with PlatformUnverifiable; there is no path-based fallback.
/// Nothing here writes, creates, renames or deletes anything.
/// </summary>
public static class NoFollowRead
{
    private const int O_RDONLY = 0, O_NONBLOCK = 0x800, O_CLOEXEC = 0x80000, O_DIRECTORY = 0x10000, O_NOFOLLOW = 0x20000;
    private const int AT_EMPTY_PATH = 0x1000;
    private const uint STATX_TYPE = 0x1, STATX_MODE = 0x2, STATX_NLINK = 0x4, STATX_SIZE = 0x200;
    private const int S_IFMT = 0xF000, S_IFREG = 0x8000;
    private const int EINTR = 4, ELOOP = 40, ENOTDIR = 20, ENOENT = 2;

    [DllImport("libc", EntryPoint = "openat", SetLastError = true)]
    private static extern int OpenAt(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);
    [DllImport("libc", EntryPoint = "statx", SetLastError = true)]
    private static extern int Statx(int dirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags, uint mask, byte[] buf);
    [DllImport("libc", EntryPoint = "read", SetLastError = true)]
    private static extern IntPtr Read(int fd, byte[] buf, UIntPtr count);
    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int Close(int fd);

    public static void RequireSupportedPlatform()
    {
        if (!OperatingSystem.IsLinux() || RuntimeInformation.ProcessArchitecture != Architecture.X64)
            throw new P1Refusal("PlatformUnverifiable", "P1 file access is implemented for Linux x64 only");
    }

    /// <summary>Reads exactly <paramref name="expectedLength"/> bytes of <paramref name="fileName"/> inside <paramref name="directory"/>, or refuses.</summary>
    public static byte[] ReadExact(string directory, string fileName, long expectedLength)
    {
        RequireSupportedPlatform();
        if (fileName.Length == 0 || fileName.Contains('/') || fileName is "." or "..")
            throw new P1Refusal("NameInvalid", "file name must be a single path component");
        if (expectedLength < 0 || expectedLength > int.MaxValue - 1)
            throw new P1Refusal("SizeInvalid", "expected length out of range");

        int dir = OpenDirectoryNoFollow(directory);
        try
        {
            int fd = OpenAt(dir, fileName, O_RDONLY | O_NOFOLLOW | O_NONBLOCK | O_CLOEXEC);
            if (fd < 0)
            {
                int e = Marshal.GetLastPInvokeError();
                throw e switch
                {
                    ELOOP => new P1Refusal("SymlinkRefused", "the fixture file is a symbolic link"),
                    ENOENT => new P1Refusal("FileMissing", "the fixture file does not exist"),
                    _ => new P1Refusal("OpenFailed", $"openat failed (errno {e})"),
                };
            }
            try
            {
                var (mode, nlink, size) = StatFd(fd);
                if ((mode & S_IFMT) != S_IFREG) throw new P1Refusal("NotRegularFile", "the fixture is not a regular file");
                if (nlink != 1) throw new P1Refusal("HardLinked", $"the fixture has {nlink} links; exactly 1 is required");
                if (size != (ulong)expectedLength) throw new P1Refusal("FileSizeMismatch", $"size {size} differs from the trusted length {expectedLength}");

                var buf = new byte[expectedLength + 1];
                long total = 0;
                while (total < buf.Length)
                {
                    var chunk = new byte[buf.Length - total];
                    long n = (long)Read(fd, chunk, (UIntPtr)chunk.Length);
                    if (n < 0)
                    {
                        if (Marshal.GetLastPInvokeError() == EINTR) continue;
                        throw new P1Refusal("ReadFailed", "read failed");
                    }
                    if (n == 0) break;
                    Array.Copy(chunk, 0, buf, total, n);
                    total += n;
                }
                if (total != expectedLength) throw new P1Refusal("FileChangedDuringRead", $"read {total} bytes, expected {expectedLength}");
                return buf.AsSpan(0, (int)expectedLength).ToArray();
            }
            finally { Close(fd); }
        }
        finally { Close(dir); }
    }

    private static int OpenDirectoryNoFollow(string directory)
    {
        string full = Path.GetFullPath(directory);
        int fd = OpenAt(-100, "/", O_RDONLY | O_DIRECTORY | O_CLOEXEC);
        if (fd < 0) throw new P1Refusal("OpenFailed", "cannot open /");
        foreach (var part in full.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            int next = OpenAt(fd, part, O_RDONLY | O_DIRECTORY | O_NOFOLLOW | O_CLOEXEC);
            int e = next < 0 ? Marshal.GetLastPInvokeError() : 0;
            Close(fd);
            if (next < 0)
                throw e switch
                {
                    ELOOP or ENOTDIR => new P1Refusal("DirectorySymlinkRefused", $"path component '{part}' is a symlink or not a directory"),
                    _ => new P1Refusal("DirectoryOpenFailed", $"cannot open path component '{part}' (errno {e})"),
                };
            fd = next;
        }
        return fd;
    }

    /// <summary>
    /// #643 B-01: P1 never opens an output file. Generated bytes may only be streamed to an inherited stream descriptor that
    /// is a pipe, socket or character device (terminal). If the caller's shell connected the descriptor to a regular file,
    /// directory or anything else, P1 refuses before writing a byte.
    /// </summary>
    public static void RequireStreamDescriptor(int fd)
    {
        RequireSupportedPlatform();
        var (mode, _, _) = StatFd(fd);
        int type = mode & S_IFMT;
        if (type is not (0x1000 or 0x2000 or 0xC000)) // S_IFIFO, S_IFCHR, S_IFSOCK
            throw new P1Refusal("OutputNotStream", "standard output is a file or other non-stream object; P1 only streams to a pipe or terminal");
    }

    // struct statx: stx_mask@0 (u32), stx_nlink@16 (u32), stx_mode@28 (u16), stx_size@40 (u64); 256 bytes total.
    private static (int mode, uint nlink, ulong size) StatFd(int fd)
    {
        var buf = new byte[256];
        if (Statx(fd, "", AT_EMPTY_PATH, STATX_TYPE | STATX_MODE | STATX_NLINK | STATX_SIZE, buf) != 0)
            throw new P1Refusal("StatFailed", $"statx failed (errno {Marshal.GetLastPInvokeError()})");
        uint mask = BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(0));
        const uint need = STATX_TYPE | STATX_NLINK | STATX_SIZE;
        if ((mask & need) != need) throw new P1Refusal("StatFailed", "statx did not return type, link count and size");
        return (BinaryPrimitives.ReadUInt16LittleEndian(buf.AsSpan(28)),
                BinaryPrimitives.ReadUInt32LittleEndian(buf.AsSpan(16)),
                BinaryPrimitives.ReadUInt64LittleEndian(buf.AsSpan(40)));
    }
}
