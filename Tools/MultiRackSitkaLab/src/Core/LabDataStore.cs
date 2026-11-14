using System.Text.RegularExpressions;

namespace SitkaLab.Core;

/// <summary>Thin file-system seam so tests can prove that rejected paths never reach a file open.</summary>
public interface ILabFileAccess
{
    bool DirectoryExists(string path);
    /// <summary>Non-null when <paramref name="path"/> is a symbolic link / reparse point.</summary>
    string? LinkTarget(string path);
    bool IsRegularFile(string path);
    long FileLength(string path);
    IReadOnlyList<string> ListFileNames(string dir);
    byte[] ReadAllBytes(string path);
    void WriteAtomic(string finalPath, byte[] bytes);
    void CreateDirectory(string path);
}

public sealed class RealLabFileAccess : ILabFileAccess
{
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public string? LinkTarget(string path)
    {
        FileSystemInfo info = Directory.Exists(path) ? new DirectoryInfo(path) : new FileInfo(path);
        return info.LinkTarget;
    }
    public bool IsRegularFile(string path)
    {
        var fi = new FileInfo(path);
        return fi.Exists && (fi.Attributes & (FileAttributes.Directory | FileAttributes.Device | FileAttributes.ReparsePoint)) == 0;
    }
    public long FileLength(string path) => new FileInfo(path).Length;
    public IReadOnlyList<string> ListFileNames(string dir) =>
        new DirectoryInfo(dir).EnumerateFileSystemInfos().Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
    public byte[] ReadAllBytes(string path) => File.ReadAllBytes(path);
    public void WriteAtomic(string finalPath, byte[] bytes)
    {
        string tmp = finalPath + ".tmp-" + Guid.NewGuid().ToString("N");
        using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            fs.Write(bytes, 0, bytes.Length);
        File.Move(tmp, finalPath, overwrite: true);
    }
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);
}

/// <summary>
/// The only gateway to lab saves. Accepts nothing but a bare file name from the lab-generated listing; there is no path,
/// no directory argument and no way to name a CCF game save. Checks run before any file is opened.
/// </summary>
public sealed class LabDataStore
{
    private static readonly Regex SaveFileName = new("^[A-Za-z0-9_-]{1,64}\\.sitkalab\\.json$", RegexOptions.CultureInvariant);
    private static readonly Regex SaveBaseName = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.CultureInvariant);

    private readonly ILabFileAccess _io;
    public string RootPath { get; }

    public LabDataStore(string toolRoot, ILabFileAccess? io = null)
    {
        _io = io ?? new RealLabFileAccess();
        RootPath = Path.GetFullPath(Path.Combine(toolRoot, ".lab-data"));
    }

    public void EnsureRoot()
    {
        if (!_io.DirectoryExists(RootPath)) _io.CreateDirectory(RootPath);
        CheckRoot();
    }

    private void CheckRoot()
    {
        if (!_io.DirectoryExists(RootPath)) throw new LabException("RootMissing", "lab data root does not exist");
        if (_io.LinkTarget(RootPath) is not null) throw new LabException("SymlinkRejected", "the lab data root is a symbolic link and is refused");
    }

    private string Resolve(string fileName)
    {
        string full = Path.GetFullPath(Path.Combine(RootPath, fileName));
        if (!string.Equals(Path.GetDirectoryName(full), RootPath, StringComparison.Ordinal) || !full.StartsWith(RootPath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new LabException("PathRejected", "resolved path is outside the lab data root");
        return full;
    }

    /// <summary>Lab-generated list of loadable files (regular files with a safe name; symlinks are not offered).</summary>
    public IReadOnlyList<string> ListSaves()
    {
        EnsureRoot();
        var names = new List<string>();
        foreach (var n in _io.ListFileNames(RootPath))
        {
            if (!SaveFileName.IsMatch(n)) continue;
            string full = Resolve(n);
            if (_io.LinkTarget(full) is not null || !_io.IsRegularFile(full)) continue;
            names.Add(n);
        }
        return names;
    }

    public byte[] ReadSave(string fileName)
    {
        // 1. name shape — pure string check, no file system access
        if (!SaveFileName.IsMatch(fileName ?? "")) throw new LabException("PathRejected", "only a bare '<name>.sitkalab.json' from the lab file list is accepted (no paths, separators or traversal)");
        // 2. root + listing membership
        CheckRoot();
        if (!ListSaves().Contains(fileName!, StringComparer.Ordinal)) throw new LabException("NotListed", "that file is not in the lab file list");
        // 3. symlink / type check on the exact entry, still without opening it
        string full = Resolve(fileName!);
        if (_io.LinkTarget(full) is not null) throw new LabException("SymlinkRejected", "symbolic links are refused");
        if (!_io.IsRegularFile(full)) throw new LabException("NotRegularFile", "not a regular file");
        if (_io.FileLength(full) > LabConstants.MaxFileBytes) throw new LabException("FileTooLarge", "file exceeds the size limit");
        return _io.ReadAllBytes(full);
    }

    /// <summary>Saves under a new or existing lab name. The server appends the suffix; the caller never supplies a path.</summary>
    public string WriteSave(string baseName, byte[] bytes)
    {
        if (!SaveBaseName.IsMatch(baseName ?? "")) throw new LabException("PathRejected", "save name must be 1-64 characters of [A-Za-z0-9_-]");
        EnsureRoot();
        string fileName = baseName + LabConstants.FileSuffix;
        string full = Resolve(fileName);
        if (_io.LinkTarget(full) is not null) throw new LabException("SymlinkRejected", "refusing to overwrite a symbolic link");
        _io.WriteAtomic(full, bytes);
        return fileName;
    }
}
