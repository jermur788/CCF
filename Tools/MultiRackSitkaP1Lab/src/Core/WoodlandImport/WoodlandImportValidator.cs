using System.Globalization;

namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Deterministic validator for Woodland Importer V1.
/// Enforces the documented readiness precedence:
/// 1. Structurally untrustworthy inputs -&gt; <see cref="WoodlandReadiness.Refused"/>
/// 2. Missing/invalid boundary, unsupported living species, or missing simulation-required field -&gt; <see cref="WoodlandReadiness.InventoryOnly"/>
/// 3. Otherwise -&gt; <see cref="WoodlandReadiness.CandidateForSimulationIntegration"/>
/// </summary>
public static class WoodlandImportValidator
{
    public static WoodlandImportResult Validate(
        WoodlandImportStand stand,
        WoodlandImportOptions options,
        IEnumerable<ImportValidationIssue>? priorIssues = null)
    {
        var issues = new List<ImportValidationIssue>();
        if (priorIssues is not null)
        {
            issues.AddRange(priorIssues);
        }

        // ------------------------------------------------------------------
        // 1. Structural / cap / integrity checks (Refusal tier)
        // ------------------------------------------------------------------
        if (stand.SchemaVersion != WoodlandImportStand.CurrentSchemaVersion)
        {
            issues.Add(new ImportValidationIssue(
                "UnsupportedSchemaVersion",
                IssueSeverity.Refusal,
                $"Unsupported WoodlandImportStand schema version {stand.SchemaVersion}; expected {WoodlandImportStand.CurrentSchemaVersion}.",
                FieldName: nameof(WoodlandImportStand.SchemaVersion)));
        }

        if (stand.ArchiveUncompressedBytes > options.MaxUncompressedBytes)
        {
            issues.Add(new ImportValidationIssue(
                "CapUncompressedBytesExceeded",
                IssueSeverity.Refusal,
                $"Uncompressed archive byte count ({stand.ArchiveUncompressedBytes}) exceeds cap ({options.MaxUncompressedBytes}).",
                FieldName: nameof(WoodlandImportStand.ArchiveUncompressedBytes)));
        }

        int totalRecords = Math.Max(stand.TotalArchiveRowsScanned, stand.Trees.Count);
        if (totalRecords > options.MaxTotalRecords)
        {
            issues.Add(new ImportValidationIssue(
                "CapTotalRecordsExceeded",
                IssueSeverity.Refusal,
                $"Total record count ({totalRecords}) exceeds cap ({options.MaxTotalRecords}).",
                FieldName: nameof(WoodlandImportStand.TotalArchiveRowsScanned)));
        }

        if (stand.TotalMeasurementRowsScanned > options.MaxMeasurementRows)
        {
            issues.Add(new ImportValidationIssue(
                "CapMeasurementRowsExceeded",
                IssueSeverity.Refusal,
                $"Measurement row count ({stand.TotalMeasurementRowsScanned}) exceeds cap ({options.MaxMeasurementRows}).",
                FieldName: nameof(WoodlandImportStand.TotalMeasurementRowsScanned)));
        }

        // Recompute living/dead/unknown conservatively from the stand trees.
        int livingCount = 0;
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        // Validate boundary object if supplied directly (in case caller constructed WoodlandBoundary without TryCreate).
        WoodlandBoundary? validBoundary = null;
        if (stand.Boundary is null)
        {
            bool alreadyHasBoundaryIssue = issues.Any(i => i.FieldName == "Boundary");
            if (!alreadyHasBoundaryIssue)
            {
                issues.Add(new ImportValidationIssue(
                    "BoundaryMissing",
                    IssueSeverity.InventoryOnlyBlocker,
                    "No surveyed boundary polygon was supplied. Stand is retained as InventoryOnly until a valid surveyed boundary is provided.",
                    FieldName: "Boundary"));
            }
        }
        else
        {
            if (WoodlandBoundary.TryCreateLocal(
                    stand.Boundary.VerticesLocalM,
                    out var revalidated,
                    out var boundaryIssues,
                    stand.Boundary.CoordinateSource,
                    stand.Boundary.GeographicCentroid,
                    stand.Boundary.ProjectionNote))
            {
                validBoundary = revalidated;
            }
            else
            {
                issues.AddRange(boundaryIssues);
            }
        }

        for (int i = 0; i < stand.Trees.Count; i++)
        {
            ImportedTreeRecord tree = stand.Trees[i];
            string treeId = tree.SourceTreeId;

            if (string.IsNullOrWhiteSpace(treeId))
            {
                issues.Add(new ImportValidationIssue(
                    "MissingSourceTreeId",
                    IssueSeverity.Refusal,
                    $"Tree at index {i} has a blank or missing SourceTreeId.",
                    FieldName: nameof(ImportedTreeRecord.SourceTreeId)));
            }
            else if (!seenIds.Add(treeId))
            {
                issues.Add(new ImportValidationIssue(
                    "DuplicateSourceTreeId",
                    IssueSeverity.Refusal,
                    $"Duplicate SourceTreeId '{treeId}' encountered in stand.",
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.SourceTreeId)));
            }

            // Refuse Simulated provenance at Year 0 on any field — never silently accept.
            ValidateFieldProvenance(tree.RawSpeciesName, treeId, nameof(ImportedTreeRecord.RawSpeciesName), issues);
            ValidateFieldProvenance(tree.VitalStatus, treeId, nameof(ImportedTreeRecord.VitalStatus), issues);
            ValidateFieldProvenance(tree.PositionLocalM, treeId, nameof(ImportedTreeRecord.PositionLocalM), issues);
            ValidateFieldProvenance(tree.DbhCm, treeId, nameof(ImportedTreeRecord.DbhCm), issues);
            ValidateFieldProvenance(tree.HeightM, treeId, nameof(ImportedTreeRecord.HeightM), issues);
            ValidateFieldProvenance(tree.CrownRadiusM, treeId, nameof(ImportedTreeRecord.CrownRadiusM), issues);
            ValidateFieldProvenance(tree.AgeYears, treeId, nameof(ImportedTreeRecord.AgeYears), issues);

            // Physical domain integrity on present numeric fields (non-finite or non-positive -> structural refusal).
            if (!tree.PositionLocalM.IsMissing && !tree.PositionLocalM.Value.IsFinite)
            {
                issues.Add(new ImportValidationIssue(
                    "NonFiniteTreeCoordinate",
                    IssueSeverity.Refusal,
                    $"Tree '{treeId}' has a non-finite local coordinate.",
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.PositionLocalM)));
            }

            if (!tree.DbhCm.IsMissing && (!double.IsFinite(tree.DbhCm.Value) || tree.DbhCm.Value <= 0.0))
            {
                issues.Add(new ImportValidationIssue(
                    "InvalidDbhValue",
                    IssueSeverity.Refusal,
                    string.Create(CultureInfo.InvariantCulture, $"Tree '{treeId}' has invalid DBH ({tree.DbhCm.Value} cm); observed DBH must be finite and > 0."),
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.DbhCm)));
            }

            if (!tree.HeightM.IsMissing && (!double.IsFinite(tree.HeightM.Value) || tree.HeightM.Value <= 0.0))
            {
                issues.Add(new ImportValidationIssue(
                    "InvalidHeightValue",
                    IssueSeverity.Refusal,
                    string.Create(CultureInfo.InvariantCulture, $"Tree '{treeId}' has invalid height ({tree.HeightM.Value} m); observed height must be finite and > 0."),
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.HeightM)));
            }

            if (!tree.CrownRadiusM.IsMissing && (!double.IsFinite(tree.CrownRadiusM.Value) || tree.CrownRadiusM.Value < 0.0))
            {
                issues.Add(new ImportValidationIssue(
                    "InvalidCrownRadiusValue",
                    IssueSeverity.Refusal,
                    string.Create(CultureInfo.InvariantCulture, $"Tree '{treeId}' has invalid crown radius ({tree.CrownRadiusM.Value} m)."),
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.CrownRadiusM)));
            }

            if (!tree.AgeYears.IsMissing && tree.AgeYears.Value < 0)
            {
                issues.Add(new ImportValidationIssue(
                    "InvalidAgeValue",
                    IssueSeverity.Refusal,
                    $"Tree '{treeId}' has negative age ({tree.AgeYears.Value} years).",
                    SourceTreeId: treeId,
                    FieldName: nameof(ImportedTreeRecord.AgeYears)));
            }

            // Point-in-polygon check against valid surveyed boundary: refuses out-of-bounds without clamping.
            if (validBoundary is not null && !tree.PositionLocalM.IsMissing && tree.PositionLocalM.Value.IsFinite)
            {
                if (!validBoundary.Contains(tree.PositionLocalM.Value))
                {
                    issues.Add(new ImportValidationIssue(
                        "TreeOutOfBounds",
                        IssueSeverity.Refusal,
                        string.Create(CultureInfo.InvariantCulture, $"Tree '{treeId}' at ({tree.PositionLocalM.Value.XM:F3} m, {tree.PositionLocalM.Value.YM:F3} m) lies outside the surveyed boundary polygon (no clamping permitted)."),
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.PositionLocalM)));
                }
            }

            bool isLiving = tree.IsConservativelyLiving;
            if (isLiving)
            {
                livingCount++;
            }

            // ------------------------------------------------------------------
            // 2. InventoryOnly admission blockers (living / conservatively-living stems only)
            // Option A: retain all measured trees; block simulation admission while any
            // unsupported living tree or missing required measurement remains.
            // ------------------------------------------------------------------
            if (isLiving)
            {
                SpeciesSupportCategory cat = tree.SpeciesMapping.Category;
                bool speciesSupported = cat == SpeciesSupportCategory.ModelledAtBase ||
                    (options.AllowPendingModelSupportForSimulation && cat == SpeciesSupportCategory.PendingModelSupport);

                if (!speciesSupported)
                {
                    string code = cat switch
                    {
                        SpeciesSupportCategory.PendingModelSupport => "PendingModelSupportLivingSpecies",
                        SpeciesSupportCategory.UnidentifiedSpecies => "UnidentifiedLivingSpecies",
                        _ => "UnsupportedLivingSpecies",
                    };
                    issues.Add(new ImportValidationIssue(
                        code,
                        IssueSeverity.InventoryOnlyBlocker,
                        $"Living tree '{treeId}' species '{tree.SpeciesMapping.RawName}' is classified as {cat} and blocks simulation admission (retained in inventory).",
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.SpeciesMapping)));
                }

                if (tree.VitalStatus.IsMissing || tree.VitalStatus.Value == TreeVitalStatus.Unknown)
                {
                    issues.Add(new ImportValidationIssue(
                        "MissingVitalStatus",
                        IssueSeverity.InventoryOnlyBlocker,
                        $"Tree '{treeId}' has missing/unknown vital status (counted conservatively as living; blocks simulation admission).",
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.VitalStatus)));
                }

                if (tree.PositionLocalM.IsMissing)
                {
                    issues.Add(new ImportValidationIssue(
                        "MissingRequiredPosition",
                        IssueSeverity.InventoryOnlyBlocker,
                        $"Living tree '{treeId}' is missing local coordinates required for simulation.",
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.PositionLocalM)));
                }

                if (tree.DbhCm.IsMissing)
                {
                    issues.Add(new ImportValidationIssue(
                        "MissingRequiredDbh",
                        IssueSeverity.InventoryOnlyBlocker,
                        $"Living tree '{treeId}' is missing DBH measurement required for simulation (no allometric estimation permitted).",
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.DbhCm)));
                }

                if (options.RequireHeightForSimulation && tree.HeightM.IsMissing)
                {
                    issues.Add(new ImportValidationIssue(
                        "MissingRequiredHeight",
                        IssueSeverity.InventoryOnlyBlocker,
                        $"Living tree '{treeId}' is missing height measurement required for simulation (no allometric estimation permitted).",
                        SourceTreeId: treeId,
                        FieldName: nameof(ImportedTreeRecord.HeightM)));
                }
            }
        }

        if (livingCount > options.MaxLivingStems)
        {
            issues.Add(new ImportValidationIssue(
                "CapLivingStemsExceeded",
                IssueSeverity.Refusal,
                $"Living stem count ({livingCount}, counting unknown status conservatively as living) exceeds cap ({options.MaxLivingStems}).",
                FieldName: nameof(WoodlandImportStand.LivingStemCount)));
        }

        if (livingCount == 0 && !issues.Any(i => i.Severity == IssueSeverity.Refusal))
        {
            issues.Add(new ImportValidationIssue(
                "NoLivingTreesForSimulation",
                IssueSeverity.InventoryOnlyBlocker,
                "Stand contains zero living trees and cannot be admitted for simulation.",
                FieldName: nameof(WoodlandImportStand.Trees)));
        }

        // Canonical deterministic ordering of issues.
        issues.Sort(CompareIssues);

        WoodlandReadiness readiness = DetermineReadiness(issues);

        return new WoodlandImportResult
        {
            Readiness = readiness,
            Stand = readiness == WoodlandReadiness.Refused ? null : stand,
            Issues = issues.AsReadOnly(),
        };
    }

    private static void ValidateFieldProvenance<T>(
        ImportField<T> field,
        string treeId,
        string fieldName,
        List<ImportValidationIssue> issues)
    {
        if (field.Provenance == FieldProvenance.Simulated)
        {
            issues.Add(new ImportValidationIssue(
                "SimulatedFieldAtYearZero",
                IssueSeverity.Refusal,
                $"Tree '{treeId}' field '{fieldName}' carries FieldProvenance.Simulated, which is forbidden at Year 0 import.",
                SourceTreeId: treeId,
                FieldName: fieldName));
        }

        if (field.Provenance == FieldProvenance.Missing && field.HasValue)
        {
            issues.Add(new ImportValidationIssue(
                "InconsistentFieldProvenance",
                IssueSeverity.Refusal,
                $"Tree '{treeId}' field '{fieldName}' is marked Missing but has HasValue=true.",
                SourceTreeId: treeId,
                FieldName: fieldName));
        }
        else if (field.Provenance != FieldProvenance.Missing && !field.HasValue)
        {
            issues.Add(new ImportValidationIssue(
                "InconsistentFieldProvenance",
                IssueSeverity.Refusal,
                $"Tree '{treeId}' field '{fieldName}' is marked {field.Provenance} but has HasValue=false.",
                SourceTreeId: treeId,
                FieldName: fieldName));
        }
    }

    private static WoodlandReadiness DetermineReadiness(List<ImportValidationIssue> issues)
    {
        bool hasInventoryBlocker = false;
        for (int i = 0; i < issues.Count; i++)
        {
            if (issues[i].Severity == IssueSeverity.Refusal)
            {
                return WoodlandReadiness.Refused;
            }
            if (issues[i].Severity == IssueSeverity.InventoryOnlyBlocker)
            {
                hasInventoryBlocker = true;
            }
        }

        return hasInventoryBlocker
            ? WoodlandReadiness.InventoryOnly
            : WoodlandReadiness.CandidateForSimulationIntegration;
    }

    internal static int CompareIssues(ImportValidationIssue a, ImportValidationIssue b)
    {
        int cmp = b.Severity.CompareTo(a.Severity); // Highest severity first.
        if (cmp != 0) return cmp;
        cmp = StringComparer.Ordinal.Compare(a.Code, b.Code);
        if (cmp != 0) return cmp;
        cmp = StringComparer.Ordinal.Compare(a.SourceTreeId ?? string.Empty, b.SourceTreeId ?? string.Empty);
        if (cmp != 0) return cmp;
        cmp = StringComparer.Ordinal.Compare(a.FieldName ?? string.Empty, b.FieldName ?? string.Empty);
        if (cmp != 0) return cmp;
        return StringComparer.Ordinal.Compare(a.Message, b.Message);
    }
}
