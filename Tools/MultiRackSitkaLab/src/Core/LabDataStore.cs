using System.Text.RegularExpressions;

namespace SitkaLab.Core;

/// <summary>Thin file-system seam so tests can prove that rejected paths never reach a file open (and can inject a swap into the open window).</summary>
public interface ILabFileAccess
{
    bool DirectoryExists(string path);
    /// <summary>Non-null when <paramref name="path"/> is a symbolic link / reparse point. Used for the lab root only.</summary>
    string? LinkTarget(string path);
    IReadOnlyList<string> ListFileNames(string dir);
    /// <summary>What <paramref name="fileName"/> inside <paramref name="dir"/> is, judged without following links and without opening it (never blocks).</summary>
    LabEntryKind Classify(string dir, string fileName);
    /// <summary>
    /// The only way lab bytes are read. Opens the entry no-follow relative to a handle on <paramref name="dir"/>, verifies on the OPEN handle
    /// that it is a single-link regular file, and returns at most <paramref name="maxBytes"/> bytes. Throws <see cref="LabException"/> otherwise.
    /// </summary>
    byte[] ReadRegularFile(string dir, string fileName, long maxBytes);
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
    public IReadOnlyList<string> ListFileNames(string dir) =>
        new DirectoryInfo(dir).EnumerateFileSystemInfos().Select(f => f.Name).OrderBy(n => n, StringComparer.Ordinal).ToList();
    public LabEntryKind Classify(string dir, string fileName) => NoFollowFiles.Classify(dir, fileName);
    public byte[] ReadRegularFile(string dir, string fileName, long maxBytes) => NoFollowFiles.ReadRegular(dir, fileName, maxBytes);

    /// <summary>
    /// Writes a sibling temp file with CreateNew (never follows a link), then renames it over the target (rename replaces the directory entry
    /// itself, so a symlink or FIFO squatting the name is replaced, never written through). If anything fails after the temp file was created,
    /// exactly that temp file is removed and a controlled <see cref="LabException"/> is thrown; no other path is ever deleted.
    /// </summary>
    public void WriteAtomic(string finalPath, byte[] bytes)
    {
        string tmp = finalPath + ".tmp-" + Guid.NewGuid().ToString("N");
        bool created = false;
        try
        {
            using (var fs = new FileStream(tmp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                created = true;                       // CreateNew succeeded, so this exact path is ours
                fs.Write(bytes, 0, bytes.Length);
            }
            File.Move(tmp, finalPath, overwrite: true);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            if (created) { try { File.Delete(tmp); } catch (Exception) { /* best effort; the name is unique to this call */ } }
            throw new LabException("WriteFailed", "the save could not be written; the existing entry was left unchanged");
        }
    }
    public void CreateDirectory(string path) => Directory.CreateDirectory(path);
}

/// <summary>
/// The only gateway to lab saves. Accepts nothing but a bare file name from the lab-generated listing; there is no path,
/// no directory argument and no way to name a CCF game save. Name checks run before any file-system access; the file itself is then opened
/// no-follow relative to a handle on the lab folder and verified on the open handle (see <see cref="NoFollowFiles"/>).
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

    /// <summary>Lab-generated list of loadable files: safe name AND a single-link regular file (symlinks, FIFOs, directories, hard-linked files are not offered).</summary>
    public IReadOnlyList<string> ListSaves()
    {
        EnsureRoot();
        var names = new List<string>();
        foreach (var n in _io.ListFileNames(RootPath))
        {
            if (!SaveFileName.IsMatch(n)) continue;
            if (_io.Classify(RootPath, n) != LabEntryKind.Regular) continue;
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
        // 3. early, non-opening classification of the exact entry. This is only a cheap first line: it can be raced, so it is NOT what makes the read safe.
        switch (_io.Classify(RootPath, fileName!))
        {
            case LabEntryKind.Regular: break;
            case LabEntryKind.Symlink: throw new LabException("SymlinkRejected", "symbolic links are refused");
            case LabEntryKind.HardLinked: throw new LabException("HardLinkRejected", "files with more than one hard link are refused");
            case LabEntryKind.Missing: throw new LabException("NotListed", "that file is not in the lab file list");
            default: throw new LabException("NotRegularFile", "not a regular file");
        }
        // 4. the authoritative check: no-follow open relative to the lab folder handle, type/link-count verified on the open handle, size-capped read
        return _io.ReadRegularFile(RootPath, fileName!, LabConstants.MaxFileBytes);
    }

    /// <summary>Saves under a new or existing lab name. The server appends the suffix; the caller never supplies a path.</summary>
    public string WriteSave(string baseName, byte[] bytes)
    {
        if (!SaveBaseName.IsMatch(baseName ?? "")) throw new LabException("PathRejected", "save name must be 1-64 characters of [A-Za-z0-9_-]");
        EnsureRoot();
        string fileName = baseName + LabConstants.FileSuffix;
        string full = Resolve(fileName);
        switch (_io.Classify(RootPath, fileName))
        {
            case LabEntryKind.Missing:
            case LabEntryKind.Regular:
            case LabEntryKind.HardLinked: break;   // replacing a regular file is allowed: the rename swaps the directory entry and leaves other links untouched
            case LabEntryKind.Symlink: throw new LabException("SymlinkRejected", "refusing to overwrite a symbolic link");
            default: throw new LabException("NotRegularFile", "a folder or special file already uses that name; refusing to replace it");
        }
        _io.WriteAtomic(full, bytes);
        return fileName;
    }
}
