using System.Text;
using SitkaLab.Core;

// Writes the checked-in synthetic fixtures. There is NO random generator: every value is a closed-form
// arithmetic pattern of the tree index, so the output is byte-identical on every run and runtime.
// These are ENGINEERING TEST FIXTURES, not measured Irish stands, and carry no biological meaning.
//
// Usage: dotnet run --project tools/FixtureTool -- <output-dir>

if (args.Length != 1) { Console.Error.WriteLine("usage: FixtureTool <output-dir>"); return 2; }
string outDir = Path.GetFullPath(args[0]);
Directory.CreateDirectory(outDir);

var written = new List<(string Name, string Sha)>();
void Write(string fixtureId, ForestSnapshot snap)
{
    SnapshotValidator.ValidateSnapshot(snap);
    var content = new LabFileContent { FixtureId = fixtureId, Snapshot = snap, SnapshotHash = CanonicalJson.SnapshotHash(snap) };
    byte[] bytes = LabFile.Serialize(content);
    LabFile.Parse(bytes); // round-trip proof: the file we write is accepted by the strict reader
    string name = fixtureId + LabConstants.FileSuffix;
    File.WriteAllBytes(Path.Combine(outDir, name), bytes);
    written.Add((name, CanonicalJson.Sha256Hex(bytes)));
    Console.WriteLine($"{name}  trees={snap.Trees.Count}  area_m2={Geometry.AreaM2(snap.Polygon)}  snapshotHash={content.SnapshotHash}");
}

static TreeState Tree(string id, long x, long y, long dbh, long h, TreeStatus st = TreeStatus.Live, long age = 28) =>
    new() { TreeId = id, XMm = x, YMm = y, ZMm = 0, DbhMm = dbh, HeightMm = h, AgeAtInventoryYears = age, Status = st };

// 1. Hand-computable oracle: 50 m x 50 m = 0.25 ha, live stems 20 / 30 / 40 cm, adult threshold 10 cm.
{
    var s = new ForestSnapshot
    {
        ForestId = "oracle-quarter-ha", InventoryYear = 2026, AdultThresholdMm = 100,
        Polygon = { new(0, 0), new(50_000, 0), new(50_000, 50_000), new(0, 50_000) },
        Trees = { Tree("T001", 10_000, 10_000, 200, 12_000), Tree("T002", 25_000, 25_000, 300, 15_000), Tree("T003", 40_000, 40_000, 400, 18_000) },
    };
    Write("oracle-quarter-ha", s);
}

// Deterministic lattice helper (index-driven arithmetic patterns, no PRNG).
List<TreeState> Lattice(Func<long, long, bool> inside, long x0, long dx, int cols, long y0, long dy, int rows, bool withIssues)
{
    var list = new List<TreeState>();
    int i = 0;
    for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            long x = x0 + dx * c, y = y0 + dy * r;
            if (!inside(x, y)) continue;
            i++;
            long dbh = 150 + (i * 53 % 29) * 9;                 // 150..402 mm pattern
            var st = TreeStatus.Live;
            if (withIssues)
            {
                if (i % 11 == 3) dbh = 60 + (i % 5) * 8;        // below the 100 mm adult threshold
                else if (i % 17 == 5) st = TreeStatus.Dead;     // dead stem, kept for history
            }
            long h = 8_000 + dbh * 25;                          // placeholder arithmetic, NOT an allometric model
            list.Add(Tree($"T{i:D4}", x, y, dbh, h, st));
        }
    return list;
}

// 2. Demo stand: skewed, non-rectangular quadrilateral (not axis-aligned), with dead, below-threshold and boundary stems.
{
    var poly = new List<Vertex> { new(0, 0), new(80_000, 0), new(95_000, 60_000), new(30_000, 75_000), new(0, 50_000) };
    bool Inside(long x, long y) => Geometry.ContainsInclusive(poly, x, y);
    var trees = Lattice(Inside, 3_000, 6_000, 16, 3_000, 6_000, 13, true);
    // Boundary-inclusive stems: exactly on the west edge x=0 and on the south edge y=0.
    trees.Add(Tree("B001", 0, 25_000, 220, 13_500));
    trees.Add(Tree("B002", 40_000, 0, 260, 14_500));
    trees.Sort((a, b) => string.CompareOrdinal(a.TreeId, b.TreeId));
    var s = new ForestSnapshot { ForestId = "sitka-flat-demo", InventoryYear = 2026, AdultThresholdMm = 100, Polygon = poly, Trees = trees };
    Write("sitka-flat-demo-v1", s);
}

// 3. Performance fixture: exactly 2,000 stems on a 50 x 40 lattice inside 200 m x 100 m (2.0 ha).
{
    var poly = new List<Vertex> { new(0, 0), new(200_000, 0), new(200_000, 100_000), new(0, 100_000) };
    var trees = Lattice((_, _) => true, 2_000, 4_000, 50, 1_250, 2_500, 40, true);
    if (trees.Count != 2000) throw new InvalidOperationException("perf fixture must have exactly 2000 trees, has " + trees.Count);
    var s = new ForestSnapshot { ForestId = "sitka-flat-perf-2000", InventoryYear = 2026, AdultThresholdMm = 100, Polygon = poly, Trees = trees };
    Write("sitka-flat-perf-2000-v1", s);
}

var sb = new StringBuilder();
foreach (var (n, sha) in written.OrderBy(w => w.Name, StringComparer.Ordinal)) sb.Append(sha).Append("  ").Append(n).Append('\n');
File.WriteAllText(Path.Combine(outDir, "MANIFEST.sha256"), sb.ToString(), new UTF8Encoding(false));
Console.WriteLine("MANIFEST.sha256 written");
return 0;
