namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>
/// Plain-English title and explanation for every blocker and refusal code the importer can emit
/// (src/Core/WoodlandImport/*.cs). Unknown codes fall back to the code as title and the importer's own
/// message as explanation. The wording describes what the importer checked; it makes no ecological claims.
/// </summary>
public static class BlockerExplanations
{
    private const string Refused = " The import was refused.";

    private static readonly IReadOnlyDictionary<string, (string Title, string Explanation)> Known =
        new Dictionary<string, (string Title, string Explanation)>(StringComparer.Ordinal)
        {
            // ---- Inventory-only blockers: the data is kept and can be inspected, but time cannot advance. ----

            // Boundary (WoodlandImportValidator, WoodlandBoundary, IntegrateMarteloscopeReader).
            ["BoundaryMissing"] = (
                "No boundary supplied",
                "No boundary polygon was supplied with this import. CCF does not guess a boundary from tree positions, so the trees can be inspected but the stand cannot be simulated."),
            ["BoundaryTreeFrameMismatch"] = (
                "Boundary is not in the trees' local frame",
                "The supplied boundary uses geographic coordinates, but the tree positions are plot-local metres with no georeference, so the two cannot be lined up. The boundary was set aside."),
            ["BoundaryTooFewVertices"] = (
                "Boundary has too few corners",
                "The supplied boundary polygon has fewer than three distinct corners, so it does not enclose an area."),
            ["BoundaryNonFiniteVertex"] = (
                "Boundary has an invalid corner",
                "A corner of the supplied boundary polygon is not a finite number, or a geographic corner is outside the valid latitude/longitude range."),
            ["BoundaryDegenerateEdge"] = (
                "Boundary has a zero-length edge",
                "Two consecutive corners of the supplied boundary polygon are the same point, so one of its edges has zero length."),
            ["BoundarySelfIntersecting"] = (
                "Boundary crosses itself",
                "Two edges of the supplied boundary polygon cross or touch each other, so its area is ambiguous."),
            ["BoundaryZeroArea"] = (
                "Boundary encloses no usable area",
                "The supplied boundary polygon encloses no usable area: its corners lie on one straight line, or its computed area is zero, too small to count (degenerate) or not a finite number."),

            // Species and measurements of living trees (WoodlandImportValidator).
            ["UnsupportedLivingSpecies"] = (
                "Living trees of species CCF does not model",
                "At least one living tree is a species the simulator has no growth model for. Every tree is kept for inspection; none is deleted or replaced with another species."),
            ["PendingModelSupportLivingSpecies"] = (
                "Living trees of species not yet simulated",
                "At least one living tree is a species CCF intends to support but cannot simulate yet. Every tree is kept for inspection."),
            ["UnidentifiedLivingSpecies"] = (
                "Living trees not identified to species",
                "At least one living tree has no species name, or a name that does not identify a single species (for example only a genus or family). CCF does not guess the species."),
            ["MissingVitalStatus"] = (
                "Trees with unknown living or dead status",
                "At least one tree has no recorded living or dead status, or a status code the importer does not recognise. It is counted as living to be safe, and the stand cannot be simulated until the status is known."),
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

            // ---- Refusals: the import itself could not be trusted, so no trees are shown. ----

            // Stand and tree records (WoodlandImportValidator).
            ["UnsupportedSchemaVersion"] = (
                "Unsupported data format version",
                "The stand data uses a format version this importer does not support." + Refused),
            ["SimulatedFieldAtYearZero"] = (
                "Simulated value found in Year-0 data",
                "The data contains a value marked as simulated at Year 0. Year 0 must be observed data only, so no tree data is shown." + Refused),
            ["InconsistentFieldProvenance"] = (
                "Field label disagrees with its value",
                "A tree field is labelled as missing but holds a value, or labelled as present but holds none." + Refused),
            ["MissingSourceTreeId"] = (
                "Tree without an identifier",
                "A tree has a blank or missing source identifier." + Refused),
            ["DuplicateSourceTreeId"] = (
                "Duplicate tree identifier",
                "Two trees share the same source identifier, so the import was refused rather than merging or dropping one."),
            ["NonFiniteTreeCoordinate"] = (
                "Tree position is not a number",
                "A tree's position is not a finite number." + Refused),
            ["InvalidDbhValue"] = (
                "Invalid stem diameter",
                "A tree's recorded stem diameter is zero, negative or not a finite number." + Refused),
            ["InvalidHeightValue"] = (
                "Invalid height",
                "A tree's recorded height is zero, negative or not a finite number." + Refused),
            ["InvalidCrownRadiusValue"] = (
                "Invalid crown radius",
                "A tree's recorded crown radius is negative or not a finite number." + Refused),
            ["InvalidAgeValue"] = (
                "Invalid age",
                "A tree's recorded age is negative." + Refused),
            ["TreeOutOfBounds"] = (
                "Tree outside the boundary",
                "At least one tree lies outside the supplied boundary polygon. CCF does not move trees to fit, so the import was refused."),

            // Plot selection, measurements and fingerprint (IntegrateMarteloscopeReader).
            ["TargetPlotNotFound"] = (
                "Requested plot not found",
                "The archive does not contain the requested plot." + Refused),
            ["MultiplePlotsRequireTargetPlotId"] = (
                "Archive contains several plots",
                "The archive holds more than one plot or survey, and no single plot was chosen, so the import was refused rather than merging them."),
            ["ArchiveSha256Mismatch"] = (
                "Archive does not match its pinned fingerprint",
                "Either the archive bytes do not match the SHA-256 fingerprint they were declared to have, so the file may be damaged or altered, " +
                "or the import metadata declared two different fingerprints for the same archive, so there was no single fingerprint to check the file against." + Refused),
            ["DuplicateMeasurementRow"] = (
                "Conflicting repeated measurement",
                "A tree has more than one row for a measurement that can only have one value, so the import was refused rather than silently keeping one of them."),
            ["MalformedTreeCoordinates"] = (
                "Unreadable tree coordinates",
                "A tree's coordinates could not be read as numbers." + Refused),
            ["MalformedMeasurementValue"] = (
                "Unreadable measurement",
                "A tree measurement is not a valid number, or not a whole number where one is required." + Refused),
            ["MissingMeasurementUnit"] = (
                "Measurement without a unit",
                "A tree measurement has no unit, and CCF does not assume one." + Refused),
            ["UnsupportedMeasurementUnit"] = (
                "Measurement in an unsupported unit",
                "A tree measurement uses a unit the importer does not accept (it accepts m, cm or mm)." + Refused),

            // Safety limits (WoodlandImportValidator, IntegrateMarteloscopeReader, DarwinCoreArchiveReader).
            ["CapLivingStemsExceeded"] = (
                "Too many living trees",
                "The archive has more living trees than the importer's safety limit (trees with unknown status count as living)." + Refused),
            ["CapTotalRecordsExceeded"] = (
                "Too many records",
                "The archive has more records than the importer's safety limit." + Refused),
            ["CapMeasurementRowsExceeded"] = (
                "Too many measurement rows",
                "The archive has more measurement rows than the importer's safety limit." + Refused),
            ["CapUncompressedBytesExceeded"] = (
                "Archive too large",
                "The archive expands, or declares that it expands, to more data than the importer's safety limit." + Refused),
            ["InvalidImportOptions"] = (
                "Invalid import limits",
                "The import was started with safety limits that are zero or negative." + Refused),

            // Archive container and Darwin Core structure (DarwinCoreArchiveReader).
            ["ArchiveEmpty"] = (
                "Archive is empty",
                "The archive contains no bytes." + Refused),
            ["ArchiveMalformedZip"] = (
                "Archive is not a readable ZIP file",
                "The archive could not be opened as a ZIP file." + Refused),
            ["ArchiveCorruptEntry"] = (
                "Archive contains a damaged file",
                "A file inside the archive could not be decompressed, so the archive is damaged or has been altered." + Refused),
            ["ArchiveEntrySizeMismatch"] = (
                "Archive file has the wrong size",
                "A file inside the archive decompressed to a different size than the archive declares for it, so the archive may be damaged or altered." + Refused),
            ["ArchiveEntryCrcMismatch"] = (
                "Archive file failed its checksum",
                "A file inside the archive failed its CRC-32 checksum, so the archive may be damaged or altered." + Refused),
            ["ArchiveMissingMetaXml"] = (
                "Archive has no meta.xml",
                "The archive has no meta.xml file describing its tables, which a Darwin Core Archive requires." + Refused),
            ["ArchiveXmlDtdProhibited"] = (
                "meta.xml contains a DTD",
                "meta.xml contains a DTD/DOCTYPE declaration, which the importer does not accept for security reasons." + Refused),
            ["ArchiveMalformedMetaXml"] = (
                "meta.xml is invalid",
                "meta.xml is not valid XML, or one of its table settings (file location, identifier column, field index or header-line count) is missing or invalid." + Refused),
            ["ArchiveMissingCoreDescriptor"] = (
                "meta.xml lists no main table",
                "meta.xml does not describe a main (core) data table." + Refused),
            ["ArchiveMultipleCores"] = (
                "meta.xml lists several main tables",
                "meta.xml describes more than one main (core) data table." + Refused),
            ["ArchiveMissingCoreFile"] = (
                "Main data table is missing",
                "The main data table named in meta.xml is not in the archive." + Refused),
            ["ArchiveMissingExtensionFile"] = (
                "Extension table is missing",
                "An extension table named in meta.xml (such as the measurements table) is not in the archive." + Refused),
            ["ArchiveUnsupportedEncoding"] = (
                "Unsupported text encoding",
                "A data table declares a text encoding the importer does not support." + Refused),
            ["ArchiveMalformedHeader"] = (
                "Blank line in a table header",
                "A data table has a blank line among its header lines, so the import was refused rather than misreading header lines as data."),
            ["ArchiveRowMissingKeyColumn"] = (
                "Row without its identifier column",
                "A data row has too few columns to contain its identifier." + Refused),
            ["ArchiveBlankCoreId"] = (
                "Row with a blank identifier",
                "A row in the main data table has a blank identifier." + Refused),
            ["ArchiveBlankExtensionCoreId"] = (
                "Extension rows not linked to a tree",
                "Some extension rows (such as measurements) have a blank link to their tree, so they cannot be attached to any tree." + Refused),
            ["ArchiveOrphanExtensionRow"] = (
                "Extension rows for an unknown tree",
                "Some extension rows (such as measurements) refer to a tree identifier that is not in the main data table." + Refused),
        };

    public static string TitleFor(string code) =>
        Known.TryGetValue(code, out var known) ? known.Title : code;

    public static string ExplanationFor(string code, string fallbackMessage) =>
        Known.TryGetValue(code, out var known) ? known.Explanation : fallbackMessage;

    public static bool IsKnown(string code) => Known.ContainsKey(code);
}
