namespace SitkaLab.Core;

public sealed class LabFixture
{
    public string FixtureId { get; init; } = "";
    public string FileName { get; init; } = "";
    public ForestSnapshot Snapshot { get; init; } = new();
    public string SnapshotHash { get; init; } = "";
    /// <summary>SHA-256 of the checked-in file's exact bytes.</summary>
    public string FileSha256 { get; init; } = "";
}

/// <summary>The checked-in, immutable synthetic fixtures. The server enumerates this directory itself; clients never supply a path.</summary>
public sealed class FixtureCatalog
{
    private readonly SortedDictionary<string, LabFixture> _byId = new(StringComparer.Ordinal);
    public IReadOnlyList<LabFixture> All => _byId.Values.ToList();
    public LabFixture? TryGet(string id) => _byId.TryGetValue(id, out var f) ? f : null;

    public static FixtureCatalog LoadFrom(string fixtureDir)
    {
        var cat = new FixtureCatalog();
        var dir = new DirectoryInfo(fixtureDir);
        if (!dir.Exists) throw new LabException("FixtureDirMissing", "fixture directory does not exist");
        foreach (var fi in dir.EnumerateFiles("*" + LabConstants.FileSuffix).OrderBy(f => f.Name, StringComparer.Ordinal))
        {
            if (fi.LinkTarget is not null) throw new LabException("SymlinkRejected", $"fixture {fi.Name} is a symbolic link");
            var bytes = File.ReadAllBytes(fi.FullName);
            var content = LabFile.Parse(bytes);
            string expectedName = content.FixtureId + LabConstants.FileSuffix;
            if (fi.Name != expectedName) throw new LabException("FixtureInvalid", $"{fi.Name}: file name must equal fixtureId + {LabConstants.FileSuffix}");
            if (content.Events.Count != 0) throw new LabException("FixtureInvalid", $"{fi.Name}: a checked-in fixture must have an empty event log");
            if (content.Snapshot.Plans.Count != 0) throw new LabException("FixtureInvalid", $"{fi.Name}: a checked-in fixture must contain no plans");
            if (content.Snapshot.ExperimentStep != 0 || content.Snapshot.ForestRevision != 0) throw new LabException("FixtureInvalid", $"{fi.Name}: a fixture starts at experimentStep 0 and forestRevision 0");
            if (content.Snapshot.Trees.Any(t => t.Mark != MarkState.Unmarked || t.Status == TreeStatus.Felled))
                throw new LabException("FixtureInvalid", $"{fi.Name}: a fixture starts with no marks and no felled trees");
            if (cat._byId.ContainsKey(content.FixtureId)) throw new LabException("DuplicateId", $"duplicate fixtureId {content.FixtureId}");
            cat._byId[content.FixtureId] = new LabFixture
            {
                FixtureId = content.FixtureId,
                FileName = fi.Name,
                Snapshot = content.Snapshot,
                SnapshotHash = content.SnapshotHash,
                FileSha256 = CanonicalJson.Sha256Hex(bytes),
            };
        }
        if (cat._byId.Count == 0) throw new LabException("FixtureDirEmpty", "no synthetic fixtures found");
        return cat;
    }
}
