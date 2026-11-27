namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>
/// Plain-English title and explanation for each known importer issue code.
/// Unknown codes fall back to the code as title and the importer's own message as explanation.
/// The wording describes what the importer found; it makes no ecological claims.
/// </summary>
public static class BlockerExplanations
{
    private static readonly IReadOnlyDictionary<string, (string Title, string Explanation)> Known =
        new Dictionary<string, (string Title, string Explanation)>(StringComparer.Ordinal)
        {
            // Inventory-only blockers: the data is kept and can be inspected, but time cannot advance.
            ["BoundaryMissing"] = (
                "No surveyed boundary",
                "This plot has no surveyed boundary polygon. CCF does not guess a boundary from tree positions, so the trees can be inspected but the stand cannot be simulated."),
            ["BoundaryTreeFrameMismatch"] = (
                "Boundary is not in the trees' local frame",
                "The supplied boundary uses geographic coordinates, but the tree positions are plot-local metres with no georeference, so the two cannot be lined up. The boundary was set aside."),
            ["BoundaryTooFewVertices"] = (
                "Boundary has too few corners",
                "The supplied boundary polygon has fewer than three corners, so it does not enclose an area."),
            ["BoundaryNonFiniteVertex"] = (
                "Boundary has an invalid corner",
                "A corner of the supplied boundary polygon is not a finite number."),
            ["BoundaryDegenerateEdge"] = (
                "Boundary has a zero-length edge",
                "Two neighbouring corners of the supplied boundary polygon are at the same point."),
            ["BoundarySelfIntersecting"] = (
                "Boundary crosses itself",
                "The supplied boundary polygon crosses over itself, so its area is ambiguous."),
            ["BoundaryZeroArea"] = (
                "Boundary encloses no area",
                "The supplied boundary polygon has zero area."),
            ["UnsupportedLivingSpecies"] = (
                "Living trees of species CCF does not model",
                "At least one living tree is a species the simulator has no growth model for. Every tree is kept for inspection; none is deleted or replaced with another species."),
            ["PendingModelSupportLivingSpecies"] = (
                "Living trees of species not yet simulated",
                "At least one living tree is a species CCF intends to support but cannot simulate yet. Every tree is kept for inspection."),
            ["UnidentifiedLivingSpecies"] = (
                "Living trees without a species identification",
                "At least one living tree is recorded without a usable species name. CCF does not guess the species."),
            ["MissingVitalStatus"] = (
                "Trees with unknown living or dead status",
                "At least one tree has no recorded living or dead status. It is counted as living to be safe, and the stand cannot be simulated until the status is known."),
            ["MissingRequiredPosition"] = (
                "Living trees without a position",
                "At least one living tree has no recorded position in the plot."),
            ["MissingRequiredDbh"] = (
                "Living trees without a measured diameter",
                "At least one living tree has no measured stem diameter. CCF does not estimate missing diameters."),
            ["MissingRequiredHeight"] = (
                "Living trees without a measured height",
                "At least one living tree has no measured height. CCF does not estimate missing heights from diameter."),
            ["NoLivingTreesForSimulation"] = (
                "No living trees",
                "The stand contains no living trees, so there is nothing to simulate."),

            // Refusals: the import itself could not be trusted, so no trees are shown.
            ["SimulatedFieldAtYearZero"] = (
                "Simulated value found in Year-0 data",
                "The data claims to contain simulated values at Year 0. Year 0 must be observed data only, so the import was refused."),
            ["TreeOutOfBounds"] = (
                "Tree outside the boundary",
                "At least one tree lies outside the supplied boundary polygon. CCF does not move trees to fit, so the import was refused."),
            ["ArchiveSha256Mismatch"] = (
                "Archive does not match its pinned fingerprint",
                "The archive bytes do not match the SHA-256 fingerprint they were expected to have, so the file may be damaged or altered. The import was refused."),
            ["DuplicateSourceTreeId"] = (
                "Duplicate tree identifier",
                "Two trees share the same source identifier, so the import was refused rather than merging or dropping one."),
            ["MissingSourceTreeId"] = (
                "Tree without an identifier",
                "A tree has no source identifier, so the import was refused."),
            ["TargetPlotNotFound"] = (
                "Requested plot not found",
                "The archive does not contain the requested plot."),
            ["MultiplePlotsRequireTargetPlotId"] = (
                "Archive contains several plots",
                "The archive holds more than one plot or survey, and no single plot was chosen, so the import was refused rather than merging them."),
            ["CapLivingStemsExceeded"] = (
                "Too many living trees",
                "The archive has more living trees than the importer's safety limit."),
            ["CapTotalRecordsExceeded"] = (
                "Too many records",
                "The archive has more records than the importer's safety limit."),
            ["CapMeasurementRowsExceeded"] = (
                "Too many measurement rows",
                "The archive has more measurement rows than the importer's safety limit."),
            ["CapUncompressedBytesExceeded"] = (
                "Archive too large",
                "The archive expands to more data than the importer's safety limit."),
            ["ArchiveMalformedZip"] = (
                "Archive is not a readable ZIP file",
                "The archive could not be opened as a ZIP file, so the import was refused."),
        };

    public static string TitleFor(string code) =>
        Known.TryGetValue(code, out var known) ? known.Title : code;

    public static string ExplanationFor(string code, string fallbackMessage) =>
        Known.TryGetValue(code, out var known) ? known.Explanation : fallbackMessage;

    public static bool IsKnown(string code) => Known.ContainsKey(code);
}
