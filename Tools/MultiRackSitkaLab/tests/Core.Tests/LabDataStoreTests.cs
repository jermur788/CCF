using System.Diagnostics;
using System.Runtime.InteropServices;
using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

/// <summary>
/// File-safety tests for the lab data store. Every test works in its own throw-away tool root (never a production save path) and removes it,
/// including any FIFO, symlink or hard link it created. Anything that could block is run on a worker task with a bounded wait.
/// </summary>
public static class LabDataStoreTests
{
    private const string Canary = "OUTSIDE-CANARY-3f9c1d7a";

    // File.Move(overwrite) is not guaranteed to be one rename(2) (it may unlink and link, which shows a transient second link), so the adversary uses the real atomic call.
    [DllImport("libc", EntryPoint = "rename", SetLastError = true)]
    private static extern int AtomicRename([MarshalAs(UnmanagedType.LPUTF8Str)] string from, [MarshalAs(UnmanagedType.LPUTF8Str)] string to);

    /// <summary>A disposable tool root with an empty .lab-data folder and helpers that create hostile entries.</summary>
    private sealed class Scratch : IDisposable
    {
        public readonly string Root = Directory.CreateTempSubdirectory("sitkalab-test-").FullName;
        public string Data => Path.Combine(Root, ".lab-data");
        public Scratch() { Directory.CreateDirectory(Data); }

        public string Lab(string name, string text = "LAB-CONTENT") { var p = Path.Combine(Data, name); File.WriteAllText(p, text); return p; }
        public string Outside(string name = "outside-secret.txt", string text = Canary) { var p = Path.Combine(Root, name); File.WriteAllText(p, text); return p; }
        public static void Fifo(string path) { var p = Process.Start(new ProcessStartInfo("mkfifo", path) { UseShellExecute = false })!; p.WaitForExit(); T.Eq(0, p.ExitCode, "mkfifo available"); }
        public static void HardLink(string existing, string link) { var p = Process.Start(new ProcessStartInfo("ln", new[] { existing, link }) { UseShellExecute = false })!; p.WaitForExit(); T.Eq(0, p.ExitCode, "ln available"); }

        public void Dispose() { try { Directory.Delete(Root, true); } catch (Exception) { /* best effort; the folder is under the system temp directory */ } }
    }

    /// <summary>
    /// Wraps the real file access and lets a test (a) count every call, (b) script what Classify reports, and (c) run code at the exact moment
    /// between the last pre-check and the authoritative open — the interval a hostile same-user process would use.
    /// </summary>
    private sealed class HookedAccess : ILabFileAccess
    {
        public readonly RealLabFileAccess Real = new();
        public int Reads, Writes, Lists, Probes, Classifies;
        public string? ForcedRootLink;
        public Func<int, LabEntryKind?>? ClassifyScript;          // 1-based Classify call number -> forced answer (null = the real answer)
        public Action<string, string>? BeforeOpen;                // (dir, name) runs immediately before ReadRegularFile touches the entry
        public readonly List<LabEntryKind> ClassifySeen = new();
        public bool DirectoryExists(string path) { Probes++; return Real.DirectoryExists(path); }
        public string? LinkTarget(string path) { Probes++; return ForcedRootLink ?? Real.LinkTarget(path); }
        public IReadOnlyList<string> ListFileNames(string dir) { Lists++; return Real.ListFileNames(dir); }
        public LabEntryKind Classify(string dir, string fileName)
        {
            Probes++; Classifies++;
            var kind = ClassifyScript?.Invoke(Classifies) ?? Real.Classify(dir, fileName);
            ClassifySeen.Add(kind);
            return kind;
        }
        public byte[] ReadRegularFile(string dir, string fileName, long maxBytes) { Reads++; BeforeOpen?.Invoke(dir, fileName); return Real.ReadRegularFile(dir, fileName, maxBytes); }
        public void WriteAtomic(string dir, string fileName, byte[] bytes) { Writes++; Real.WriteAtomic(dir, fileName, bytes); }
        public void CreateDirectory(string path) { Probes++; Real.CreateDirectory(path); }
    }

    /// <summary>Runs <paramref name="read"/> on a worker task and fails the test (instead of hanging it) if it does not finish within five seconds.</summary>
    private static string Prompt(Func<byte[]> read, string what)
    {
        var task = Task.Run(() => { try { var b = read(); return "returned:" + System.Text.Encoding.UTF8.GetString(b); } catch (LabException ex) { return ex.Code; } });
        T.True(task.Wait(5000), what + " returned promptly (it must never block on a special file)");
        return task.Result;
    }

    // ---- name shape ----------------------------------------------------------------------------------------------

    [Test]
    public static void PathEscapes_AreRejectedByNameShape_BeforeAnyFileSystemAccess()
    {
        var io = new HookedAccess();
        using var sc = new Scratch();
        var store = new LabDataStore(sc.Root, io);
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

    // ---- static links (the old tests, kept) --------------------------------------------------------------------

    [Test]
    public static void SymlinkToOutsideFile_IsRefusedBeforeOpen_ProvenWithAFifo()
    {
        // A symlink to a FIFO with no writer: following it would block forever, so returning promptly proves it was never opened.
        using var sc = new Scratch();
        string fifo = Path.Combine(sc.Root, "outside.fifo"); Scratch.Fifo(fifo);
        File.CreateSymbolicLink(Path.Combine(sc.Data, "evil.sitkalab.json"), fifo);
        var secret = sc.Outside("secret.txt", "\u0001\u0002\u0003");
        File.CreateSymbolicLink(Path.Combine(sc.Data, "secret.sitkalab.json"), secret);
        var store = new LabDataStore(sc.Root);
        var listed = store.ListSaves();
        T.False(listed.Contains("evil.sitkalab.json") || listed.Contains("secret.sitkalab.json"), "symlinks are not even offered in the lab list");
        foreach (var n in new[] { "evil.sitkalab.json", "secret.sitkalab.json" })
        {
            string r = Prompt(() => store.ReadSave(n), "ReadSave of symlink " + n);
            T.True(r is "NotListed" or "SymlinkRejected", $"{n} refused: {r}");
        }
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.WriteSave("evil", new byte[] { 9 }), "overwrite of a symlink").Code, "writes never follow symlinks");
        T.Eq("\u0001\u0002\u0003", File.ReadAllText(secret), "outside file untouched");
    }

    [Test]
    public static void LinkReportedOnTheRoot_RefusesTheWholeStore_BeforeAnyOpen()
    {
        // Honest scope of this test: it covers the ROOT check only (formerly named as if it covered the per-entry race; see the swap tests below).
        using var sc = new Scratch();
        sc.Lab("evil.sitkalab.json");
        var io = new HookedAccess { ForcedRootLink = "/etc/passwd" };
        var store = new LabDataStore(sc.Root, io);
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.ReadSave("evil.sitkalab.json"), "linked root").Code, "link detected on the root");
        T.Eq(0, io.Reads, "never opened");
        T.Eq(0, io.Classifies, "not even a per-entry probe once the root is refused");
    }

    [Test]
    public static void SymlinkedLabRoot_IsRefused()
    {
        using var sc = new Scratch();
        Directory.Delete(sc.Data);
        var elsewhere = Directory.CreateDirectory(Path.Combine(sc.Root, "elsewhere")).FullName;
        Directory.CreateSymbolicLink(sc.Data, elsewhere);
        var store = new LabDataStore(sc.Root);
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.ListSaves(), "list").Code, "list refused");
        T.Eq("SymlinkRejected", T.Throws<LabException>(() => store.WriteSave("x", new byte[] { 1 }), "write").Code, "write refused");
        T.Eq(0, Directory.GetFiles(elsewhere).Length, "nothing written through the link");
    }

    // ---- F-1: special files placed directly in the lab folder -----------------------------------------------------

    [Test]
    public static void Classify_ReportsTheRealKindOfEveryEntry_WithoutOpeningIt()
    {
        using var sc = new Scratch();
        var real = new RealLabFileAccess();
        var reg = sc.Lab("reg.sitkalab.json");
        File.CreateSymbolicLink(Path.Combine(sc.Data, "sym.sitkalab.json"), reg);
        Scratch.Fifo(Path.Combine(sc.Data, "fifo.sitkalab.json"));
        Directory.CreateDirectory(Path.Combine(sc.Data, "dir.sitkalab.json"));
        Scratch.HardLink(sc.Outside(), Path.Combine(sc.Data, "hard.sitkalab.json"));
        var kinds = new Dictionary<string, LabEntryKind>
        {
            ["reg.sitkalab.json"] = LabEntryKind.Regular, ["sym.sitkalab.json"] = LabEntryKind.Symlink, ["fifo.sitkalab.json"] = LabEntryKind.Fifo,
            ["dir.sitkalab.json"] = LabEntryKind.Directory, ["hard.sitkalab.json"] = LabEntryKind.HardLinked, ["ghost.sitkalab.json"] = LabEntryKind.Missing,
        };
        foreach (var (name, expected) in kinds)
        {
            var task = Task.Run(() => real.Classify(sc.Data, name));
            T.True(task.Wait(5000), "Classify returned promptly for " + name);
            T.Eq(expected, task.Result, "kind of " + name);
        }
    }

    [Test]
    public static void Listing_OffersOnlySingleLinkRegularFiles()
    {
        using var sc = new Scratch();
        sc.Lab("ok.sitkalab.json");
        File.CreateSymbolicLink(Path.Combine(sc.Data, "sym.sitkalab.json"), Path.Combine(sc.Data, "ok.sitkalab.json"));
        Scratch.Fifo(Path.Combine(sc.Data, "fifo.sitkalab.json"));
        Directory.CreateDirectory(Path.Combine(sc.Data, "dir.sitkalab.json"));
        Scratch.HardLink(sc.Outside(), Path.Combine(sc.Data, "hard.sitkalab.json"));
        sc.Lab("not-a-save.txt");
        var store = new LabDataStore(sc.Root);
        var task = Task.Run(() => store.ListSaves());
        T.True(task.Wait(5000), "listing returned promptly with a FIFO present");
        T.Seq(new[] { "ok.sitkalab.json" }, task.Result, "only the single-link regular file is offered (not a symlink, FIFO, directory or hard-linked file)");
    }

    [Test]
    public static void FifoPlacedDirectlyInTheLabFolder_IsNotOffered_AndNeverBlocksARead()
    {
        using var sc = new Scratch();
        Scratch.Fifo(Path.Combine(sc.Data, "pipe.sitkalab.json"));
        var store = new LabDataStore(sc.Root);
        T.False(store.ListSaves().Contains("pipe.sitkalab.json"), "FIFO not listed");
        T.Eq("NotListed", Prompt(() => store.ReadSave("pipe.sitkalab.json"), "ReadSave of an unlisted FIFO"), "refused by the listing layer");
        // The authoritative open layer on its own (as if the FIFO appeared after the listing): it must refuse without waiting for a writer.
        var real = new RealLabFileAccess();
        T.Eq("NotRegularFile", Prompt(() => real.ReadRegularFile(sc.Data, "pipe.sitkalab.json", 1024), "ReadRegularFile of a FIFO"), "refused at open, by the handle's type");
    }

    // ---- F-2 / F-3: the entry changes after every pre-check, right before the open --------------------------------

    /// <summary>Runs ReadSave with <paramref name="swap"/> executed in the window after ReadSave's listing and per-entry check and before the open.</summary>
    private static (string Outcome, HookedAccess Io, int Swaps) ReadWithSwap(Scratch sc, string name, Action<string> swap)
    {
        int swaps = 0;
        var io = new HookedAccess { BeforeOpen = (dir, n) => { swap(Path.Combine(dir, n)); swaps++; } };
        var store = new LabDataStore(sc.Root, io);
        string outcome = Prompt(() => store.ReadSave(name), "ReadSave with a swap in the open window");
        return (outcome, io, swaps);
    }

    private static void AssertSwapWasInTheDangerousInterval(HookedAccess io, int swaps)
    {
        T.Eq(1, swaps, "the swap hook ran exactly once");
        T.Eq(2, io.Classifies, "ListSaves and the per-entry pre-check had both run");
        T.True(io.ClassifySeen.All(k => k == LabEntryKind.Regular), "both earlier checks saw an ordinary regular file, so only the open itself can refuse the swapped-in entry");
    }

    [Test]
    public static void SymlinkSwappedInAfterEveryPreCheck_IsRefusedAtOpen_AndNoOutsideBytesAreReturned()
    {
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json"); var outside = sc.Outside();
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path => { File.Delete(path); File.CreateSymbolicLink(path, outside); });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("SymlinkRejected", outcome, "refused by the no-follow open, not returned");
        T.False(outcome.Contains(Canary), "no outside bytes");
    }

    [Test]
    public static void SymlinkToAFifoSwappedInAfterEveryPreCheck_IsRefusedAtOpen_WithoutBlocking()
    {
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json"); string fifo = Path.Combine(sc.Root, "outside.fifo"); Scratch.Fifo(fifo);
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path => { File.Delete(path); File.CreateSymbolicLink(path, fifo); });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("SymlinkRejected", outcome, "refused without waiting for a FIFO writer");
    }

    [Test]
    public static void FifoSwappedInAfterEveryPreCheck_IsRefusedAtOpen_WithoutBlocking()
    {
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json");
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path => { File.Delete(path); Scratch.Fifo(path); });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("NotRegularFile", outcome, "the open handle's type is checked, so a FIFO cannot be read");
    }

    [Test]
    public static void DirectorySwappedInAfterEveryPreCheck_IsRefusedAtOpen()
    {
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json");
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path => { File.Delete(path); Directory.CreateDirectory(path); });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("NotRegularFile", outcome, "a directory is not a lab file");
    }

    [Test]
    public static void HardLinkToAnOutsideFileSwappedInAfterEveryPreCheck_IsRefusedAtOpen()
    {
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json"); var outside = sc.Outside();
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path => { File.Delete(path); Scratch.HardLink(outside, path); });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("HardLinkRejected", outcome, "an entry that shares its inode with another name is refused");
        T.False(outcome.Contains(Canary), "no outside bytes");
    }

    [Test]
    public static void LabFolderSwappedForASymlinkAfterEveryPreCheck_IsRefusedAtOpen()
    {
        // Everything is constrained to a handle on the real lab folder: a folder swapped for a link to another folder holding a same-named file is not followed.
        using var sc = new Scratch();
        sc.Lab("race.sitkalab.json");
        string elsewhere = Directory.CreateDirectory(Path.Combine(sc.Root, "elsewhere")).FullName;
        File.WriteAllText(Path.Combine(elsewhere, "race.sitkalab.json"), Canary);
        var (outcome, io, swaps) = ReadWithSwap(sc, "race.sitkalab.json", path =>
        {
            Directory.Move(sc.Data, sc.Data + "-moved");
            Directory.CreateSymbolicLink(sc.Data, elsewhere);
        });
        AssertSwapWasInTheDangerousInterval(io, swaps);
        T.Eq("SymlinkRejected", outcome, "the folder itself is opened without following links");
        T.False(outcome.Contains(Canary), "no outside bytes");
    }

    [Test]
    public static void ConcurrentSymlinkSwapping_NeverReturnsOutsideBytes()
    {
        // Adversarial stress (the reviewer's D2 shape): a same-user thread keeps atomically swapping the entry between a regular lab file and a symlink to an outside file.
        using var sc = new Scratch();
        var outside = sc.Outside();
        string target = Path.Combine(sc.Data, "race.sitkalab.json");
        File.WriteAllText(target, "LAB-CONTENT");
        string regTmp = Path.Combine(sc.Root, "reg.tmp"), lnkTmp = Path.Combine(sc.Root, "lnk.tmp");
        var store = new LabDataStore(sc.Root);
        int stop = 0; long swaps = 0;
        var swapper = new Thread(() =>
        {
            while (Volatile.Read(ref stop) == 0)
            {
                File.WriteAllText(regTmp, "LAB-CONTENT"); if (AtomicRename(regTmp, target) != 0) throw new IOException("rename failed");
                File.CreateSymbolicLink(lnkTmp, outside); if (AtomicRename(lnkTmp, target) != 0) throw new IOException("rename failed");
                swaps++;
            }
        }) { IsBackground = true };
        int labReads = 0, outsideReads = 0, refused = 0, unexpected = 0; string? firstUnexpected = null;
        swapper.Start();
        try
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < TimeSpan.FromSeconds(2.5))
            {
                try
                {
                    string text = System.Text.Encoding.UTF8.GetString(store.ReadSave("race.sitkalab.json"));
                    if (text.Contains(Canary)) outsideReads++; else if (text == "LAB-CONTENT") labReads++; else { unexpected++; firstUnexpected ??= "content:" + text; }
                }
                catch (LabException ex) when (ex.Code is "NotListed" or "SymlinkRejected") { refused++; }
                catch (Exception ex) { unexpected++; firstUnexpected ??= ex.GetType().Name + ":" + ex.Message; }
            }
        }
        finally { Volatile.Write(ref stop, 1); swapper.Join(); }
        T.True(swaps > 50, $"the swapper really raced the reader ({swaps} swaps)");
        T.True(labReads > 0 && refused > 0, $"both outcomes were exercised (lab reads {labReads}, refusals {refused})");
        T.Eq(0, outsideReads, $"outside-file bytes returned by ReadSave (swaps={swaps}, lab reads={labReads}, refused={refused})");
        T.Eq(0, unexpected, "no other outcome; first was " + firstUnexpected);
    }

    // ---- layered store checks, each proven on its own -------------------------------------------------------------

    [Test]
    public static void PerEntryPreCheck_IsAnIndependentLayer_AndStopsBeforeTheAuthoritativeOpen()
    {
        // The listing (Classify call 1) says "regular"; the per-entry pre-check (call 2) disagrees. The open must never be reached.
        var cases = new (LabEntryKind Kind, string Code)[]
        {
            (LabEntryKind.Symlink, "SymlinkRejected"), (LabEntryKind.HardLinked, "HardLinkRejected"), (LabEntryKind.Fifo, "NotRegularFile"),
            (LabEntryKind.Directory, "NotRegularFile"), (LabEntryKind.Other, "NotRegularFile"), (LabEntryKind.Missing, "NotListed"),
        };
        foreach (var (kind, code) in cases)
        {
            using var sc = new Scratch(); sc.Lab("one.sitkalab.json");
            var io = new HookedAccess { ClassifyScript = n => n == 2 ? kind : null };
            var store = new LabDataStore(sc.Root, io);
            T.Eq(code, T.Throws<LabException>(() => store.ReadSave("one.sitkalab.json"), "pre-check " + kind).Code, "code for " + kind);
            T.Eq(2, io.Classifies, "the pre-check ran");
            T.Eq(0, io.Reads, "the open was never reached for " + kind);
        }
    }

    [Test]
    public static void ListingFilter_IsAnIndependentLayer_AndNeverOffersASpecialFile()
    {
        // Listing says non-regular for the only file: it must not be offered and ReadSave must stop at the membership check, before any pre-check or open.
        foreach (var kind in new[] { LabEntryKind.Symlink, LabEntryKind.HardLinked, LabEntryKind.Fifo, LabEntryKind.Directory, LabEntryKind.Other, LabEntryKind.Missing })
        {
            using var sc = new Scratch(); sc.Lab("one.sitkalab.json");
            var listIo = new HookedAccess { ClassifyScript = n => n == 1 ? kind : null };
            T.Eq(0, new LabDataStore(sc.Root, listIo).ListSaves().Count, "listing offered nothing for " + kind);
            var io = new HookedAccess { ClassifyScript = n => n == 1 ? kind : null };
            var store = new LabDataStore(sc.Root, io);
            T.Eq("NotListed", T.Throws<LabException>(() => store.ReadSave("one.sitkalab.json"), "unlisted " + kind).Code, "membership check for " + kind);
            T.Eq(1, io.Classifies, "no per-entry pre-check once the listing refused it");
            T.Eq(0, io.Reads, "no open");
        }
    }

    [Test]
    public static void OversizedFile_IsRefusedBeforeAnyRead()
    {
        using var sc = new Scratch();
        using (var fs = new FileStream(Path.Combine(sc.Data, "big.sitkalab.json"), FileMode.CreateNew)) fs.SetLength(LabConstants.MaxFileBytes + 1);   // sparse
        var store = new LabDataStore(sc.Root);
        T.Eq("FileTooLarge", T.Throws<LabException>(() => store.ReadSave("big.sitkalab.json"), "oversized").Code, "size limit enforced on the open handle");
    }

    // ---- unchanged behaviour ----------------------------------------------------------------------------------------

    [Test]
    public static void CcfShapedJsonInsideTheLabRoot_FailsAtTheHeaderGate_WithoutWriting()
    {
        using var sc = new Scratch();
        sc.Lab("scenario-one.sitkalab.json", "{\"saveVersion\":3,\"trees\":[{\"id\":1,\"species\":\"Oak\"}],BROKEN");
        var store = new LabDataStore(sc.Root);
        var before = Directory.GetFileSystemEntries(sc.Data).OrderBy(x => x).ToList();
        T.Seq(new[] { "scenario-one.sitkalab.json" }, store.ListSaves(), "listed (it is a regular .sitkalab.json name)");
        var bytes = store.ReadSave("scenario-one.sitkalab.json");
        var ex = T.Throws<LabException>(() => LabFile.Parse(bytes), "CCF-shaped content");
        T.Eq("WrongFormat", ex.Code, "rejected by the header gate, not by the broken body");
        T.Seq(before, Directory.GetFileSystemEntries(sc.Data).OrderBy(x => x), "nothing written or created by the failed load");
    }

    [Test]
    public static void UnlistedNames_AreRefused_AndSavesRoundTripThroughTheStore()
    {
        using var sc = new Scratch();
        var store = new LabDataStore(sc.Root);
        T.Seq(Array.Empty<string>(), store.ListSaves(), "empty list at first");
        T.Eq("NotListed", T.Throws<LabException>(() => store.ReadSave("ghost.sitkalab.json"), "unlisted").Code, "not in the lab list");
        var s = Fx.Demo(); Fx.Ok(s.MarkTrees(Fx.Adults(s, 2)));
        byte[] bytes = LabFile.Serialize(s.ToFile());
        string written = store.WriteSave("my-run_01", bytes);
        T.Eq("my-run_01.sitkalab.json", written, "server appended the suffix");
        T.Seq(new[] { "my-run_01.sitkalab.json" }, store.ListSaves(), "now listed");
        T.Seq(bytes, store.ReadSave("my-run_01.sitkalab.json"), "byte-exact round trip");
        T.Eq(0, Directory.GetFiles(sc.Data, "*.tmp-*").Length, "atomic write leaves no temp file");
        var loaded = LabSession.FromFile(LabFile.Parse(store.ReadSave("my-run_01.sitkalab.json")), Fx.Catalog);
        T.Eq(s.StateHash, loaded.StateHash, "state preserved");
        Fx.Ok(loaded.UnmarkTrees(Fx.Adults(loaded, 0).Concat(loaded.State.Trees.Where(t => t.Mark == MarkState.PlayerMarked).Select(t => t.TreeId)).ToList()));
        store.WriteSave("my-run_01", LabFile.Serialize(loaded.ToFile())); // overwrite of a regular file is allowed
        T.Eq(1, store.ListSaves().Count, "still one file");
    }

    // ---- F-5: atomic save failure -------------------------------------------------------------------------------------

    [Test]
    public static void WriteSave_RefusesNamesSquattedByFoldersAndSpecialFiles_LeavingEverythingInPlace()
    {
        using var sc = new Scratch();
        string dir = Path.Combine(sc.Data, "dirsquat.sitkalab.json"); Directory.CreateDirectory(dir); File.WriteAllText(Path.Combine(dir, "keep.txt"), "KEEP");
        Scratch.Fifo(Path.Combine(sc.Data, "fifosquat.sitkalab.json"));
        var store = new LabDataStore(sc.Root);
        T.Eq("NotRegularFile", T.Throws<LabException>(() => store.WriteSave("dirsquat", new byte[] { 1 }), "directory squat").Code, "directory squat refused");
        T.Eq("NotRegularFile", T.Throws<LabException>(() => store.WriteSave("fifosquat", new byte[] { 1 }), "fifo squat").Code, "FIFO squat refused");
        T.Eq("KEEP", File.ReadAllText(Path.Combine(dir, "keep.txt")), "the squatting folder and its content are untouched");
        T.Eq(LabEntryKind.Fifo, new RealLabFileAccess().Classify(sc.Data, "fifosquat.sitkalab.json"), "the FIFO is still a FIFO");
        T.Eq(0, Directory.GetFiles(sc.Data, "*.tmp-*").Length, "no temp file left");
    }

    [Test]
    public static void WriteAtomic_FailureAfterTheTempFileExists_RemovesOnlyItsOwnTemp_AndThrowsControlled()
    {
        using var sc = new Scratch();
        var real = new RealLabFileAccess();
        // The target is a folder that appeared after the store's own check: File.Move fails after the temp file was written.
        string target = Path.Combine(sc.Data, "late.sitkalab.json"); Directory.CreateDirectory(target); File.WriteAllText(Path.Combine(target, "inside.txt"), "INSIDE");
        string decoy = target + ".tmp-0000decoy"; File.WriteAllText(decoy, "DECOY");     // looks like a temp file but is not ours
        string neighbour = sc.Lab("neighbour.sitkalab.json", "NEIGHBOUR");
        var ex = T.Throws<LabException>(() => real.WriteAtomic(sc.Data, "late.sitkalab.json", new byte[] { 1, 2, 3 }), "write onto a folder");
        T.Eq("WriteFailed", ex.Code, "controlled error, not a raw IOException");
        T.False(ex.Message.Contains(sc.Root), "no file-system path leaks into the message");
        T.Seq(new[] { "decoy" }, Directory.GetFiles(sc.Data, "*.tmp-*").Select(f => f.EndsWith("decoy") ? "decoy" : "OURS-LEFT-BEHIND"), "only the foreign decoy remains; our own temp file was removed");
        T.Eq("DECOY", File.ReadAllText(decoy), "unrelated look-alike untouched");
        T.Eq("INSIDE", File.ReadAllText(Path.Combine(target, "inside.txt")), "the folder was not replaced");
        T.Eq("NEIGHBOUR", File.ReadAllText(neighbour), "neighbouring save untouched");
        // A failure before any temp file exists must not delete anything either.
        var ex2 = T.Throws<LabException>(() => real.WriteAtomic(Path.Combine(sc.Data, "no-such-folder"), "x.sitkalab.json", new byte[] { 1 }), "write into a missing folder");
        T.Eq("WriteFailed", ex2.Code, "controlled error when the temp file cannot even be created");
        T.Eq("DECOY", File.ReadAllText(decoy), "still untouched");
    }

    // ---- MULTI-006 R-1: the lab FOLDER is swapped while a save is in progress ---------------------------------------------
    //
    // Attack shape (independent MULTI-005 review): a same-user process atomically exchanges `.lab-data` with a symlink to another folder
    // (renameat2 RENAME_EXCHANGE). Before MULTI-006 the save re-resolved the folder path for create / rename / cleanup, so steps landed in the
    // attacker's folder. Now the folder is opened once and every step is relative to that handle. Guarantee being tested: no file is created,
    // replaced or deleted in the alternate folder, and no temp file is orphaned. (The real folder itself may have been MOVED by the attacker —
    // it is then found under the link's old name; that is the inode-anchored guarantee, not a promise about path geography.)

    [DllImport("libc", EntryPoint = "renameat2", SetLastError = true)]
    private static extern int RenameAt2(int oldDirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, int newDirFd, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath, uint flags);
    private static bool Exchange(string a, string b) => RenameAt2(-100 /* AT_FDCWD */, a, -100, b, 2 /* RENAME_EXCHANGE */) == 0;

    /// <summary>A scratch tool root plus an "alternate" folder the attacker wants saves to land in, a symlink to it, and decoys inside it.</summary>
    private sealed class SwapRig : IDisposable
    {
        public readonly Scratch Sc = new();
        public string Alternate => Path.Combine(Sc.Root, "alternate");
        public string Link => Path.Combine(Sc.Root, "swaplink");
        public readonly Dictionary<string, string> AlternateBefore = new();
        public SwapRig()
        {
            Directory.CreateDirectory(Alternate);
            foreach (var (n, text) in new[] { ("w.sitkalab.json", "{\"format\":\"sitka-lab\",\"VICTIM\":true}"), ("w.sitkalab.json.tmp-0000decoy", "DECOY"), ("unrelated.txt", "UNRELATED") })
                File.WriteAllText(Path.Combine(Alternate, n), text);
            foreach (var f in Directory.GetFileSystemEntries(Alternate)) AlternateBefore[Path.GetFileName(f)] = File.ReadAllText(f);
            Directory.CreateSymbolicLink(Link, Alternate);
        }
        /// <summary>Where the real lab folder currently is (the attacker may have exchanged it into the link's place).</summary>
        public string RealFolder => new DirectoryInfo(Sc.Data).LinkTarget is null ? Sc.Data : Link;
        public void Restore() { if (new DirectoryInfo(Sc.Data).LinkTarget is not null) T.True(Exchange(Sc.Data, Link), "swap back"); }
        public void AssertAlternateUntouched(string when)
        {
            var now = Directory.GetFileSystemEntries(Alternate).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToList();
            T.Seq(AlternateBefore.Keys.OrderBy(n => n, StringComparer.Ordinal), now, when + ": nothing created or deleted in the alternate folder");
            foreach (var (n, text) in AlternateBefore) T.Eq(text, File.ReadAllText(Path.Combine(Alternate, n)), when + ": " + n + " in the alternate folder unchanged");
        }
        public int TempFilesIn(string dir) => Directory.GetFiles(dir, "*.tmp-*").Count(f => !f.EndsWith("decoy", StringComparison.Ordinal));
        public void Dispose() { try { Restore(); } catch (Exception) { /* best effort */ } Sc.Dispose(); }
    }

    /// <summary>Runs one WriteSave with <paramref name="atStage"/> executed once at <paramref name="stage"/> inside the save transaction.</summary>
    private static (string Outcome, int HookRuns) SaveWithHook(SwapRig rig, WriteStage stage, Action atStage, byte[] payload)
    {
        int runs = 0;
        var io = new RealLabFileAccess(s => { if (s == stage) { runs++; atStage(); } });
        var store = new LabDataStore(rig.Sc.Root, io);
        string outcome;
        try { store.WriteSave("w", payload); outcome = "saved"; } catch (LabException ex) { outcome = ex.Code; }
        return (outcome, runs);
    }

    [Test]
    public static void FolderSwappedForALink_AfterTheFolderWasOpened_TempAndRenameStayInTheTrustedFolder()
    {
        using var rig = new SwapRig();
        byte[] payload = System.Text.Encoding.UTF8.GetBytes("{\"format\":\"sitka-lab\",\"LAB\":1}");
        var (outcome, runs) = SaveWithHook(rig, WriteStage.DirectoryOpened, () => T.True(Exchange(rig.Sc.Data, rig.Link), "exchange the lab folder with the link"), payload);
        T.Eq(1, runs, "the swap happened inside the transaction, after the folder handle was opened and before the temp file was created");
        T.True(new DirectoryInfo(rig.Sc.Data).LinkTarget is not null, "the lab path really pointed at the alternate folder for the rest of the save");
        T.Eq("saved", outcome, "the save completed (in the folder that was opened) and reported success truthfully");
        rig.AssertAlternateUntouched("after a mid-save folder swap");
        T.Seq(payload, File.ReadAllBytes(Path.Combine(rig.RealFolder, "w.sitkalab.json")), "the bytes are in the trusted (moved) lab folder");
        T.Eq(0, rig.TempFilesIn(rig.RealFolder) + rig.TempFilesIn(rig.Alternate), "no temp file left anywhere");
        rig.Restore();
        T.Seq(payload, new LabDataStore(rig.Sc.Root).ReadSave("w.sitkalab.json"), "after the folder is back, the save loads normally");
    }

    [Test]
    public static void FolderSwappedForALink_BetweenTempWriteAndRename_TheRenameStaysInTheTrustedFolder()
    {
        using var rig = new SwapRig();
        rig.Sc.Lab("w.sitkalab.json", "OLD-LAB-SAVE");
        byte[] payload = System.Text.Encoding.UTF8.GetBytes("{\"format\":\"sitka-lab\",\"LAB\":2}");
        var (outcome, runs) = SaveWithHook(rig, WriteStage.TempWritten, () => T.True(Exchange(rig.Sc.Data, rig.Link), "exchange the lab folder with the link"), payload);
        T.Eq(1, runs, "the swap happened after the temp file was written and before the rename");
        T.Eq("saved", outcome, "rename relative to the folder handle succeeded and was reported truthfully");
        rig.AssertAlternateUntouched("after a swap just before rename");
        T.Seq(payload, File.ReadAllBytes(Path.Combine(rig.RealFolder, "w.sitkalab.json")), "the existing lab save was replaced in the trusted folder");
        T.Eq(0, rig.TempFilesIn(rig.RealFolder) + rig.TempFilesIn(rig.Alternate), "no temp file left anywhere");
    }

    [Test]
    public static void FolderSwappedForALink_AndTheRenameFails_TheTempIsRemovedFromTheTrustedFolder_AndNothingElse()
    {
        using var rig = new SwapRig();
        string keep = rig.Sc.Lab("neighbour.sitkalab.json", "NEIGHBOUR");
        byte[] payload = System.Text.Encoding.UTF8.GetBytes("{\"format\":\"sitka-lab\",\"LAB\":3}");
        var (outcome, runs) = SaveWithHook(rig, WriteStage.TempWritten, () =>
        {
            // a folder appears under the final name in the real lab folder (so the rename must fail), then the lab folder is swapped away
            Directory.CreateDirectory(Path.Combine(rig.Sc.Data, "w.sitkalab.json"));
            T.True(Exchange(rig.Sc.Data, rig.Link), "exchange the lab folder with the link");
        }, payload);
        T.Eq(1, runs, "hook ran once");
        T.Eq("WriteFailed", outcome, "the failed rename is reported as a failure, never as saved");
        rig.AssertAlternateUntouched("after a failed save under a folder swap (the decoy temp-like file was not deleted)");
        T.Eq(0, rig.TempFilesIn(rig.RealFolder), "this save's own temp file was removed from the trusted folder via the same handle (no orphan)");
        T.True(Directory.Exists(Path.Combine(rig.RealFolder, "w.sitkalab.json")), "the squatting folder was left alone");
        T.Eq("NEIGHBOUR", File.ReadAllText(Path.Combine(rig.RealFolder, Path.GetFileName(keep))), "neighbouring save untouched");
    }

    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern int OpenPath([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);
    [DllImport("libc", EntryPoint = "close", SetLastError = true)]
    private static extern int ClosePath(int fd);

    [Test]
    public static void ConcurrentFolderExchange_DuringSaves_NeverWritesToTheAlternateFolder_OrOrphansTempFiles_AndOutcomesAreTruthful()
    {
        // The reviewer's store-level attack (MULTI-005 W probe: 1,223 alternate-folder overwrites before the fix), as a bounded regression test.
        using var rig = new SwapRig();
        var store = new LabDataStore(rig.Sc.Root);
        store.WriteSave("w", System.Text.Encoding.UTF8.GetBytes("INITIAL"));
        // A handle on the real lab folder lets the test look inside it wherever the attacker has moved it (/proc/self/fd/N).
        int realFd = OpenPath(rig.Sc.Data, 0x10000 /* O_DIRECTORY */ | 0x80000 /* O_CLOEXEC */);
        T.True(realFd >= 0, "test handle on the real lab folder");
        string realView = $"/proc/self/fd/{realFd}/w.sitkalab.json";
        int stop = 0; long swaps = 0;
        var swapper = new Thread(() => { while (Volatile.Read(ref stop) == 0) if (Exchange(rig.Sc.Data, rig.Link)) swaps++; }) { IsBackground = true };
        var outcomes = new Dictionary<string, int>(); int falseOutcome = 0, alternateChanges = 0; string? firstProblem = null;
        string lastSaved = "INITIAL";
        swapper.Start();
        try
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; sw.Elapsed < TimeSpan.FromSeconds(2.5); i++)
            {
                string text = "{\"format\":\"sitka-lab\",\"n\":" + i + "}";
                string k;
                try { store.WriteSave("w", System.Text.Encoding.UTF8.GetBytes(text)); k = "saved"; }
                catch (LabException ex) { k = ex.Code; }
                catch (Exception ex) { k = "RAW:" + ex.GetType().Name; }
                outcomes[k] = outcomes.GetValueOrDefault(k) + 1;
                string now = File.ReadAllText(realView);
                // success must mean exactly these bytes are in the real folder; failure must mean the real folder's save is unchanged
                if (k == "saved" ? now != text : now != lastSaved) { falseOutcome++; firstProblem ??= $"{k} but the real folder holds '{now}'"; }
                if (k == "saved") lastSaved = text;
                if (File.ReadAllText(Path.Combine(rig.Alternate, "w.sitkalab.json")) != rig.AlternateBefore["w.sitkalab.json"]) { alternateChanges++; firstProblem ??= "alternate folder save overwritten at iteration " + i; }
            }
        }
        finally { Volatile.Write(ref stop, 1); swapper.Join(); ClosePath(realFd); }
        rig.Restore();
        string summary = $"swaps={swaps} outcomes={string.Join(",", outcomes.Select(kv => kv.Key + "=" + kv.Value))} first problem: {firstProblem ?? "-"}";
        T.True(swaps > 50, "the swapper really raced the saves: " + summary);
        T.True(outcomes.GetValueOrDefault("saved") > 0 && outcomes.GetValueOrDefault("SymlinkRejected") > 0, "both saved and refused outcomes were exercised: " + summary);
        T.True(outcomes.Keys.All(k => k is "saved" or "SymlinkRejected" or "WriteFailed"), "only controlled outcomes: " + summary);
        T.Eq(0, alternateChanges, "alternate-folder overwrites during the race: " + summary);
        rig.AssertAlternateUntouched("after the concurrent folder-exchange race");
        T.Eq(0, rig.TempFilesIn(rig.Sc.Data), "temp files orphaned in the lab folder: " + summary);
        T.Eq(0, falseOutcome, "reported outcome matched the real folder every time: " + summary);
        T.Eq(lastSaved, System.Text.Encoding.UTF8.GetString(store.ReadSave("w.sitkalab.json")), "the last reported save is what loads");
    }

    // ---- MULTI-006 R-2: content larger than the limit although statx reported a small size ---------------------------------

    [Test]
    public static void ReadRegular_RefusesContentThatOutgrowsTheStatxSize_ProcfsStatus()
    {
        // /proc/<pid>/status is a regular, single-link procfs file whose statx size is 0 but whose read returns hundreds of bytes: a deterministic
        // stand-in for "the file grew after it was inspected". Only the second, read-loop size check can stop it. The numeric PID folder is used
        // because /proc/self is a symlink that the no-follow folder open (correctly) refuses. procfs is required on the supported Linux host.
        string dir = "/proc/" + Environment.ProcessId;
        T.True(File.Exists(Path.Combine(dir, "status")), "procfs is mounted (required on the supported Linux qualification host)");
        T.Eq(0L, new FileInfo(Path.Combine(dir, "status")).Length, "procfs reports size 0, so the first (statx) size check cannot see the content");
        T.Eq(LabEntryKind.Regular, new RealLabFileAccess().Classify(dir, "status"), "it is a single-link regular file by the existing guards");
        byte[] full = NoFollowFiles.ReadRegular(dir, "status", 1 << 20);
        T.True(full.Length > 10 && System.Text.Encoding.ASCII.GetString(full).Contains("Pid:"), $"positive control: generous limit returns the real content ({full.Length} bytes)");
        T.Eq("FileTooLarge", T.Throws<LabException>(() => NoFollowFiles.ReadRegular(dir, "status", 10), "read with a 10-byte limit").Code, "the read loop enforces the limit after statx said 0");
    }

    [Test]
    public static void ProductionCcfSavePathsAreNeverReferencedByTheStore()
    {
        var store = new LabDataStore("/nonexistent-tool-root-for-test");
        T.True(store.RootPath.EndsWith(Path.DirectorySeparatorChar + ".lab-data"), "root is exactly <tool>/.lab-data");
        foreach (var file in new[] { "LabDataStore.cs", "NoFollowFiles.cs" })
        {
            string src = File.ReadAllText(Path.Combine(ToolPaths.ToolRoot, "src", "Core", file));
            foreach (var banned in new[] { "forest-save", "ForestSaveData", "persistentDataPath", "Reference Future", "ReferenceFuture" })
                T.False(src.Contains(banned, StringComparison.OrdinalIgnoreCase), file + " never mentions " + banned);
        }
    }
}
