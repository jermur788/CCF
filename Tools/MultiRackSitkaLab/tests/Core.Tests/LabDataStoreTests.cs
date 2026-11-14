using System.Diagnostics;
using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class LabDataStoreTests
{
    /// <summary>Counts every file-system call so tests can prove a rejected request never reached a file open.</summary>
    private sealed class RecordingAccess : ILabFileAccess
    {
        public readonly RealLabFileAccess Real = new();
        public int Reads, Writes, Lists, Probes;
        public bool ForceListed; public string? ForcedLink;
        public bool DirectoryExists(string path) { Probes++; return Real.DirectoryExists(path); }
        public string? LinkTarget(string path) { Probes++; return ForcedLink ?? Real.LinkTarget(path); }
        public bool IsRegularFile(string path) { Probes++; return ForceListed || Real.IsRegularFile(path); }
        public long FileLength(string path) { Probes++; return 1; }
        public IReadOnlyList<string> ListFileNames(string dir) { Lists++; return ForceListed ? new[] { "evil.sitkalab.json" } : Real.ListFileNames(dir); }
        public byte[] ReadAllBytes(string path) { Reads++; return Real.ReadAllBytes(path); }
        public void WriteAtomic(string finalPath, byte[] bytes) { Writes++; Real.WriteAtomic(finalPath, bytes); }
        public void CreateDirectory(string path) { Probes++; Real.CreateDirectory(path); }
    }

    private static string TempToolRoot() => Directory.CreateTempSubdirectory("sitkalab-test-").FullName;

    [Test]
    public static void PathEscapes_AreRejectedByNameShape_BeforeAnyFileSystemAccess()
    {
        var io = new RecordingAccess();
        var root = TempToolRoot();
        var store = new LabDataStore(root, io);
        string[] hostile =
        {
            "../outside.sitkalab.json", "..\\outside.sitkalab.json", "/etc/passwd", "/etc/passwd.sitkalab.json", "sub/dir.sitkalab.json",
            "..", ".", "", "a.sitkalab.json/..", "%2e%2e%2fx.sitkalab.json", "x.sitkalab.json\0.txt", "x.json", "x.sitkalab.json.bak",
            "C:\\Windows\\x.sitkalab.json", "~/x.sitkalab.json", new string('a', 70) + ".sitkalab.json", "forest-save.json", "x y.sitkalab.json",
            "..%5cx.sitkalab.json", "\u202e.sitkalab.json", "x.sitkalab.json ",
        };
        foreach (var name in hostile)
        {
            var ex = T.Throws<LabException>(() => store.ReadSave(name), $"hostile name '{name.Replace('\0', '?')}'");
            T.Eq("PathRejected", ex.Code, "rejected by shape");
        }
        T.Eq(0, io.Reads, "no file was opened");
        T.Eq(0, io.Probes, "no file-system probe at all for shape-rejected names");
        T.Eq(0, io.Lists, "not even a directory listing");
        foreach (var bad in new[] { "../x", "a/b", "x.sitkalab.json", "", "x y" })
            T.Eq("PathRejected", T.Throws<LabException>(() => store.WriteSave(bad, new byte[] { 1 }), "bad save name " + bad).Code, "save names are shape-checked too");
        T.Eq(0, io.Writes, "nothing written");
    }

    [Test]
    public static void SymlinkToOutsideFile_IsRefusedBeforeOpen_ProvenWithAFifo()
    {
        // A symlink to a FIFO with no writer: opening it would block forever, so returning promptly proves it was never opened.
        var root = TempToolRoot(); var data = Directory.CreateDirectory(Path.Combine(root, ".lab-data")).FullName;
        string fifo = Path.Combine(root, "outside.fifo");
        var mk = Process.Start(new ProcessStartInfo("mkfifo", fifo) { UseShellExecute = false })!; mk.WaitForExit();
        T.Eq(0, mk.ExitCode, "mkfifo available");
        File.CreateSymbolicLink(Path.Combine(data, "evil.sitkalab.json"), fifo);
        File.WriteAllBytes(Path.Combine(root, "secret.txt"), new byte[] { 1, 2, 3 });
        File.CreateSymbolicLink(Path.Combine(data, "secret.sitkalab.json"), Path.Combine(root, "secret.txt"));
        var store = new LabDataStore(root);
        T.False(store.ListSaves().Contains("evil.sitkalab.json") || store.ListSaves().Contains("secret.sitkalab.json"), "symlinks are not even offered in the lab list");
        foreach (var n in new[] { "evil.sitkalab.json", "secret.sitkalab.json" })
        {
            var task = Task.Run(() => { try { store.ReadSave(n); return "opened"; } catch (LabException ex) { return ex.Code; } });
            T.True(task.Wait(5000), "ReadSave returned promptly (never blocked on the FIFO) for " + n);
            T.True(task.Result is "NotListed" or "SymlinkRejected", $"{n} refused: {task.Result}");
        }
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.WriteSave("evil", new byte[] { 9 }), "overwrite of a symlink").Code, "writes never follow symlinks");
        T.Seq(new byte[] { 1, 2, 3 }, File.ReadAllBytes(Path.Combine(root, "secret.txt")), "outside file untouched");
    }

    [Test]
    public static void SymlinkAppearingAfterListing_IsStillRefusedBeforeOpen()
    {
        var io = new RecordingAccess { ForceListed = true, ForcedLink = "/etc/passwd" };
        var root = TempToolRoot(); Directory.CreateDirectory(Path.Combine(root, ".lab-data"));
        var store = new LabDataStore(root, io);
        // The root check itself reports a link first: the whole store is refused.
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.ReadSave("evil.sitkalab.json"), "linked root").Code, "link detected");
        T.Eq(0, io.Reads, "never opened");
    }

    [Test]
    public static void SymlinkedLabRoot_IsRefused()
    {
        var root = TempToolRoot(); var elsewhere = Directory.CreateDirectory(Path.Combine(root, "elsewhere")).FullName;
        Directory.CreateSymbolicLink(Path.Combine(root, ".lab-data"), elsewhere);
        var store = new LabDataStore(root);
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.ListSaves(), "list").Code, "list refused");
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.WriteSave("x", new byte[] { 1 }), "write").Code, "write refused");
        T.Eq(0, Directory.GetFiles(elsewhere).Length, "nothing written through the link");
    }

    [Test]
    public static void CcfShapedJsonInsideTheLabRoot_FailsAtTheHeaderGate_WithoutWriting()
    {
        var root = TempToolRoot(); var data = Directory.CreateDirectory(Path.Combine(root, ".lab-data")).FullName;
        File.WriteAllText(Path.Combine(data, "scenario-one.sitkalab.json"), "{\"saveVersion\":3,\"trees\":[{\"id\":1,\"species\":\"Oak\"}],BROKEN");
        var store = new LabDataStore(root);
        var before = Directory.GetFileSystemEntries(data).OrderBy(x => x).ToList();
        T.Seq(new[] { "scenario-one.sitkalab.json" }, store.ListSaves(), "listed (it is a regular .sitkalab.json name)");
        var bytes = store.ReadSave("scenario-one.sitkalab.json");
        var ex = T.Throws<LabException>(() => LabFile.Parse(bytes), "CCF-shaped content");
        T.Eq("WrongFormat", ex.Code, "rejected by the header gate, not by the broken body");
        T.Seq(before, Directory.GetFileSystemEntries(data).OrderBy(x => x), "nothing written or created by the failed load");
    }

    [Test]
    public static void UnlistedNames_AreRefused_AndSavesRoundTripThroughTheStore()
    {
        var root = TempToolRoot();
        var store = new LabDataStore(root);
        T.Seq(Array.Empty<string>(), store.ListSaves(), "empty list at first");
        T.Eq("NotListed", T.Throws<LabException>(() => store.ReadSave("ghost.sitkalab.json"), "unlisted").Code, "not in the lab list");
        var s = Fx.Demo(); Fx.Ok(s.MarkTrees(Fx.Adults(s, 2)));
        byte[] bytes = LabFile.Serialize(s.ToFile());
        string written = store.WriteSave("my-run_01", bytes);
        T.Eq("my-run_01.sitkalab.json", written, "server appended the suffix");
        T.Seq(new[] { "my-run_01.sitkalab.json" }, store.ListSaves(), "now listed");
        T.Seq(bytes, store.ReadSave("my-run_01.sitkalab.json"), "byte-exact round trip");
        T.Eq(0, Directory.GetFiles(Path.Combine(root, ".lab-data"), "*.tmp-*").Length, "atomic write leaves no temp file");
        var loaded = LabSession.FromFile(LabFile.Parse(store.ReadSave("my-run_01.sitkalab.json")), Fx.Catalog);
        T.Eq(s.StateHash, loaded.StateHash, "state preserved");
        Fx.Ok(loaded.UnmarkTrees(Fx.Adults(loaded, 0).Concat(loaded.State.Trees.Where(t => t.Mark == MarkState.PlayerMarked).Select(t => t.TreeId)).ToList()));
        store.WriteSave("my-run_01", LabFile.Serialize(loaded.ToFile())); // overwrite of a regular file is allowed
        T.Eq(1, store.ListSaves().Count, "still one file");
    }

    [Test]
    public static void ProductionCcfSavePathsAreNeverReferencedByTheStore()
    {
        var store = new LabDataStore("/nonexistent-tool-root-for-test");
        T.True(store.RootPath.EndsWith(Path.DirectorySeparatorChar + ".lab-data"), "root is exactly <tool>/.lab-data");
        string src = File.ReadAllText(Path.Combine(ToolPaths.ToolRoot, "src", "Core", "LabDataStore.cs"));
        foreach (var banned in new[] { "forest-save", "ForestSaveData", "persistentDataPath", "Reference Future", "ReferenceFuture" })
            T.False(src.Contains(banned, StringComparison.OrdinalIgnoreCase), "store source never mentions " + banned);
    }
}
