using System.Collections.ObjectModel;
using System.Globalization;

namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Reads Integrate+ / TreMs Darwin Core Archives into deterministic <see cref="WoodlandImportStand"/>
/// snapshots and validates readiness under Option A rules (zero silent deletion, zero species substitution,
/// zero allometric estimation, no inferred boundaries, zero silent row overwrite).
/// </summary>
public sealed class IntegrateMarteloscopeReader : IWoodlandImportReader
{
    public const string DefaultDatasetTitle = "Spatially explicit database of tree related microhabitats (TreMs)";
    public const string DefaultPublisher = "Institut national de recherche pour l'agriculture, l'alimentation et l'environnement (INRAE)";
    public const string DefaultLicence = "CC BY 4.0 (http://creativecommons.org/licenses/by/4.0/legalcode)";
    public const string DefaultDoi = "10.15468/ocof3v";
    public const string DefaultDatasetKey = "2e102194-f384-4712-89a4-5db7a3fc409a";

    public WoodlandImportResult Read(WoodlandImportRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        WoodlandImportOptions options = request.Options ?? WoodlandImportOptions.Default;

        DwcArchiveReadResult archive = DarwinCoreArchiveReader.Read(request.ArchiveBytes, options);
        if (!archive.Success)
        {
            return new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.Refused,
                Stand = null,
                Issues = archive.Issues,
            };
        }

        var priorIssues = new List<ImportValidationIssue>(archive.Issues);

        // Resolve optional surveyed boundary polygon (never inferred from tree positions).
        WoodlandBoundary? surveyedBoundary = null;
        if (request.SurveyedBoundary is not null)
        {
            if (WoodlandBoundary.TryCreateLocal(
                    request.SurveyedBoundary.VerticesLocalM,
                    out var validated,
                    out var bIssues,
                    request.SurveyedBoundary.CoordinateSource,
                    request.SurveyedBoundary.GeographicCentroid,
                    request.SurveyedBoundary.ProjectionNote))
            {
                surveyedBoundary = validated;
            }
            else
            {
                priorIssues.AddRange(bIssues);
            }
        }
        else if (request.RawLocalBoundaryVertices is not null)
        {
            if (WoodlandBoundary.TryCreateLocal(request.RawLocalBoundaryVertices, out var validated, out var bIssues))
            {
                surveyedBoundary = validated;
            }
            else
            {
                priorIssues.AddRange(bIssues);
            }
        }
        else if (request.RawGeoBoundaryVertices is not null)
        {
            if (WoodlandBoundary.TryCreateFromGeographic(request.RawGeoBoundaryVertices, out var validated, out var bIssues))
            {
                surveyedBoundary = validated;
            }
            else
            {
                priorIssues.AddRange(bIssues);
            }
        }

        // Filter records by optional TargetPlotId; refuse multi-plot archives when TargetPlotId is omitted.
        var selected = new List<DwcJoinedRecord>();
        var distinctPlots = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        string? detectedPlotName = null;
        string? detectedCountry = null;

        for (int i = 0; i < archive.Records.Count; i++)
        {
            DwcJoinedRecord rec = archive.Records[i];
            string plotId = ExtractPlotId(rec);

            if (!string.IsNullOrWhiteSpace(request.TargetPlotId))
            {
                if (!plotId.Equals(request.TargetPlotId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }
            else
            {
                distinctPlots.Add(plotId);
            }

            selected.Add(rec);
            detectedPlotName ??= plotId;
            if (detectedCountry is null)
            {
                string? cc = GetTerm(rec.CoreTerms, "countryCode") ?? GetTerm(rec.CoreTerms, "country");
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    detectedCountry = cc.Trim();
                }
            }
        }

        if (string.IsNullOrWhiteSpace(request.TargetPlotId) && distinctPlots.Count > 1)
        {
            string listed = string.Join(", ", distinctPlots.Take(10));
            if (distinctPlots.Count > 10) listed += ", ...";
            priorIssues.Add(new ImportValidationIssue(
                "MultiplePlotsRequireTargetPlotId",
                IssueSeverity.Refusal,
                $"Archive contains {distinctPlots.Count} distinct plots ({listed}); specify TargetPlotId to avoid merging separate local coordinate frames."));
            priorIssues.Sort(WoodlandImportValidator.CompareIssues);
            return new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.Refused,
                Stand = null,
                Issues = priorIssues.AsReadOnly(),
            };
        }

        if (!string.IsNullOrWhiteSpace(request.TargetPlotId) && selected.Count == 0)
        {
            priorIssues.Add(new ImportValidationIssue(
                "TargetPlotNotFound",
                IssueSeverity.Refusal,
                $"Requested target plot '{request.TargetPlotId}' was not found in the archive."));
            priorIssues.Sort(WoodlandImportValidator.CompareIssues);
            return new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.Refused,
                Stand = null,
                Issues = priorIssues.AsReadOnly(),
            };
        }

        // Build tree records in canonical SourceTreeId ordinal order, enforcing MaxLivingStems BEFORE unbounded allocation.
        var trees = new List<ImportedTreeRecord>(Math.Min(selected.Count, options.MaxTotalRecords));
        int livingCount = 0;
        int deadCount = 0;
        int unknownCount = 0;
        bool anyTreeHasLocalPosition = false;

        for (int i = 0; i < selected.Count; i++)
        {
            DwcJoinedRecord rec = selected[i];
            ImportedTreeRecord tree = MapTreeRecord(rec, priorIssues);

            if (!tree.PositionLocalM.IsMissing)
            {
                anyTreeHasLocalPosition = true;
            }

            if (tree.IsConservativelyLiving)
            {
                livingCount++;
                if (tree.VitalStatus.IsMissing || tree.VitalStatus.Value == TreeVitalStatus.Unknown)
                {
                    unknownCount++;
                }

                if (livingCount > options.MaxLivingStems)
                {
                    priorIssues.Add(new ImportValidationIssue(
                        "CapLivingStemsExceeded",
                        IssueSeverity.Refusal,
                        $"Living stem count (including unknown status counted conservatively as living) exceeded cap ({options.MaxLivingStems}) while reading archive."));
                    priorIssues.Sort(WoodlandImportValidator.CompareIssues);
                    return new WoodlandImportResult
                    {
                        Readiness = WoodlandReadiness.Refused,
                        Stand = null,
                        Issues = priorIssues.AsReadOnly(),
                    };
                }
            }
            else
            {
                deadCount++;
            }

            trees.Add(tree);
        }

        // F-05: A geographic tangent-plane boundary is centred at the polygon's WGS-84 centroid and is NOT
        // registered to plot-local tree coordinates (which carry no per-tree georeference). Never run cross-frame
        // containment or admit such a stand; withhold the unregistered boundary and keep the stand InventoryOnly.
        if (surveyedBoundary is not null &&
            surveyedBoundary.CoordinateSource == BoundaryCoordinateSource.GeographicTangentPlane &&
            anyTreeHasLocalPosition)
        {
            priorIssues.Add(new ImportValidationIssue(
                "BoundaryTreeFrameMismatch",
                IssueSeverity.InventoryOnlyBlocker,
                "Geographic tangent-plane boundary cannot be registered to non-georeferenced plot-local tree coordinates. Supply a boundary in the plot-local metre frame; stand retained as InventoryOnly.",
                FieldName: "Boundary"));
            surveyedBoundary = null;
        }

        // Ensure canonical ordinal ordering on SourceTreeId.
        trees.Sort(static (a, b) => StringComparer.Ordinal.Compare(a.SourceTreeId, b.SourceTreeId));

        // Pure translation normalization to deterministic origin (distances preserved exactly).
        var (normalizedBoundary, normalizedTrees, origin) =
            LocalCoordinateNormalizer.Normalize(surveyedBoundary, trees);

        string plotName = !string.IsNullOrWhiteSpace(request.TargetPlotId)
            ? request.TargetPlotId.Trim()
            : (!string.IsNullOrWhiteSpace(detectedPlotName) ? detectedPlotName! : "unnamed-plot");

        string standId = BuildStandId(plotName, archive.ArchiveSha256);

        ImportSourceAttribution attribution = ResolveAttribution(request, archive, plotName);

        var stand = new WoodlandImportStand
        {
            SchemaVersion = WoodlandImportStand.CurrentSchemaVersion,
            StandId = standId,
            PlotName = plotName,
            CountryCode = detectedCountry,
            Attribution = attribution,
            Boundary = normalizedBoundary,
            CoordinateOrigin = origin,
            Trees = normalizedTrees,
            TotalArchiveRowsScanned = archive.CoreRowsRead,
            TotalMeasurementRowsScanned = archive.ExtensionRowsRead,
            ArchiveUncompressedBytes = archive.TotalUncompressedBytesRead,
            LivingStemCount = livingCount,
            DeadStemCount = deadCount,
            UnknownStatusStemCount = unknownCount,
        };

        return WoodlandImportValidator.Validate(stand, options, priorIssues);
    }

    private static ImportedTreeRecord MapTreeRecord(
        DwcJoinedRecord rec,
        List<ImportValidationIssue> issues)
    {
        string sourceId = rec.CoreId;

        // Parse core-level provenance FIRST so it applies honestly to species, position, and fallback measurements.
        FieldProvenance coreProvenance = ParseDeclaredProvenance(GetTerm(rec.CoreTerms, "provenance"));

        // 1. Species (never substituted; raw value retained verbatim alongside exact mapping).
        string? rawSci = GetTerm(rec.CoreTerms, "scientificName")
            ?? GetTerm(rec.CoreTerms, "species");
        ImportField<string> rawSpeciesField = string.IsNullOrWhiteSpace(rawSci)
            ? ImportField<string>.Missing(new FieldEvidence("scientificName", rawSci))
            : new ImportField<string>
            {
                Provenance = coreProvenance,
                HasValue = true,
                Value = rawSci.Trim(),
                Evidence = new FieldEvidence("scientificName", rawSci),
            };
        SpeciesMappingResult speciesMapping = WoodlandSpeciesMapper.Map(rawSci);

        // 2. Local coordinates (verbatimCoordinates e.g. "34 , 48.82", or explicit localX/localY).
        ImportField<LocalPoint2D> positionField = ParseLocalPosition(rec.CoreTerms, sourceId, coreProvenance, issues);

        // 3. Measurements & vital status from MeasurementOrFact extension rows (plus core fallbacks).
        // Detect repeated/conflicting rows for any single-valued field per tree deterministically (order-independent).
        ImportField<TreeVitalStatus> vitalField = ImportField<TreeVitalStatus>.Missing(new FieldEvidence("status", null));
        ImportField<double> dbhField = ImportField<double>.Missing(new FieldEvidence("diameter", null));
        ImportField<double> heightField = ImportField<double>.Missing(new FieldEvidence("height", null));
        ImportField<double> crownField = ImportField<double>.Missing(new FieldEvidence("crownRadius", null));
        ImportField<int> ageField = ImportField<int>.Missing(new FieldEvidence("age", null));

        var seenValues = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        var extra = new SortedDictionary<string, string>(StringComparer.Ordinal);

        // Preserve plot-level context terms without confusing them for per-tree local coordinates.
        CopyExtraIfPresent(rec.CoreTerms, extra, "habitat", "habitat");
        CopyExtraIfPresent(rec.CoreTerms, extra, "eventDate", "eventDate");
        CopyExtraIfPresent(rec.CoreTerms, extra, "decimalLatitude", "plotDecimalLatitude");
        CopyExtraIfPresent(rec.CoreTerms, extra, "decimalLongitude", "plotDecimalLongitude");
        CopyExtraIfPresent(rec.CoreTerms, extra, "verbatimElevation", "verbatimElevation");

        for (int i = 0; i < rec.ExtensionRows.Count; i++)
        {
            DwcExtensionRow ext = rec.ExtensionRows[i];
            string mType = (GetTerm(ext.Terms, "measurementType") ?? string.Empty).Trim();
            string mRemarks = (GetTerm(ext.Terms, "measurementRemarks") ?? string.Empty).Trim();
            string mValue = (GetTerm(ext.Terms, "measurementValue") ?? string.Empty).Trim();
            string mUnit = (GetTerm(ext.Terms, "measurementUnit") ?? string.Empty).Trim();
            string mMethod = (GetTerm(ext.Terms, "measurementMethod") ?? string.Empty).Trim();

            FieldProvenance rowProv = ParseDeclaredProvenance(mMethod);
            if (rowProv == FieldProvenance.Observed && coreProvenance != FieldProvenance.Observed)
            {
                rowProv = coreProvenance;
            }

            if (IsStatusRow(mType, mRemarks))
            {
                RecordFieldOccurrence(seenValues, nameof(ImportedTreeRecord.VitalStatus), mValue);
                vitalField = ParseVitalStatus(mValue, mRemarks.Length > 0 ? mRemarks : mType, rowProv);
            }
            else if (IsDiameterRow(mType, mRemarks))
            {
                RecordFieldOccurrence(seenValues, nameof(ImportedTreeRecord.DbhCm), FormatValueWithUnit(mValue, mUnit));
                dbhField = ParseDoubleMeasurement(
                    mValue, mUnit, "cm", sourceId, nameof(ImportedTreeRecord.DbhCm),
                    mRemarks.Length > 0 ? mRemarks : mType, rowProv, issues);
            }
            else if (IsHeightRow(mType, mRemarks))
            {
                RecordFieldOccurrence(seenValues, nameof(ImportedTreeRecord.HeightM), FormatValueWithUnit(mValue, mUnit));
                heightField = ParseDoubleMeasurement(
                    mValue, mUnit, "m", sourceId, nameof(ImportedTreeRecord.HeightM),
                    mRemarks.Length > 0 ? mRemarks : mType, rowProv, issues);
            }
            else if (IsCrownRadiusRow(mType, mRemarks))
            {
                RecordFieldOccurrence(seenValues, nameof(ImportedTreeRecord.CrownRadiusM), FormatValueWithUnit(mValue, mUnit));
                crownField = ParseDoubleMeasurement(
                    mValue, mUnit, "m", sourceId, nameof(ImportedTreeRecord.CrownRadiusM),
                    mRemarks.Length > 0 ? mRemarks : mType, rowProv, issues);
            }
            else if (IsAgeRow(mType, mRemarks))
            {
                RecordFieldOccurrence(seenValues, nameof(ImportedTreeRecord.AgeYears), mValue);
                ageField = ParseIntMeasurement(
                    mValue, sourceId, nameof(ImportedTreeRecord.AgeYears),
                    mRemarks.Length > 0 ? mRemarks : mType, rowProv, issues);
            }
            else if (mType.Contains("micro-habitat", StringComparison.OrdinalIgnoreCase) ||
                     mType.Contains("microhabitat", StringComparison.OrdinalIgnoreCase))
            {
                if (!string.IsNullOrWhiteSpace(mRemarks) && !string.IsNullOrWhiteSpace(mValue))
                {
                    string tremKey = "trem:" + mRemarks;
                    RecordFieldOccurrence(seenValues, tremKey, mValue);
                    extra[tremKey] = mValue;
                }
            }
        }

        // Emit deterministic, row-order-independent refusal issues for any repeated single-valued field on this tree.
        foreach (var kv in seenValues)
        {
            if (kv.Value.Count > 1)
            {
                kv.Value.Sort(StringComparer.Ordinal);
                string joinedValues = string.Join(", ", kv.Value);
                issues.Add(new ImportValidationIssue(
                    "DuplicateMeasurementRow",
                    IssueSeverity.Refusal,
                    $"Tree '{sourceId}' has {kv.Value.Count} rows for single-valued field '{kv.Key}' (values: [{joinedValues}]); refusing silent overwrite.",
                    SourceTreeId: sourceId,
                    FieldName: kv.Key));
            }
        }

        // Core-table fallback for vital status if not provided via MeasurementOrFact extension.
        if (vitalField.IsMissing && !seenValues.ContainsKey(nameof(ImportedTreeRecord.VitalStatus)))
        {
            string? coreVitality = GetTerm(rec.CoreTerms, "vitality") ?? GetTerm(rec.CoreTerms, "treeStatus");
            if (!string.IsNullOrWhiteSpace(coreVitality))
            {
                vitalField = ParseVitalStatus(coreVitality.Trim(), "vitality", coreProvenance);
            }
        }

        // Core-table fallback for DBH / height if a simple core-only DwC-A supplies them directly.
        if (dbhField.IsMissing && !seenValues.ContainsKey(nameof(ImportedTreeRecord.DbhCm)))
        {
            string? coreDbh = GetTerm(rec.CoreTerms, "dbhCm") ?? GetTerm(rec.CoreTerms, "diameterCm");
            if (!string.IsNullOrWhiteSpace(coreDbh))
            {
                dbhField = ParseDoubleMeasurement(coreDbh.Trim(), "cm", "cm", sourceId, nameof(ImportedTreeRecord.DbhCm), "dbhCm", coreProvenance, issues);
            }
        }

        if (heightField.IsMissing && !seenValues.ContainsKey(nameof(ImportedTreeRecord.HeightM)))
        {
            string? coreHeight = GetTerm(rec.CoreTerms, "heightM");
            if (!string.IsNullOrWhiteSpace(coreHeight))
            {
                heightField = ParseDoubleMeasurement(coreHeight.Trim(), "m", "m", sourceId, nameof(ImportedTreeRecord.HeightM), "heightM", coreProvenance, issues);
            }
        }

        return new ImportedTreeRecord
        {
            SourceTreeId = sourceId,
            RawSpeciesName = rawSpeciesField,
            SpeciesMapping = speciesMapping,
            VitalStatus = vitalField,
            PositionLocalM = positionField,
            DbhCm = dbhField,
            HeightM = heightField,
            CrownRadiusM = crownField,
            AgeYears = ageField,
            ExtraAttributes = extra.Count == 0
                ? ImportedTreeRecord.EmptyAttributes
                : new ReadOnlyDictionary<string, string>(extra),
        };
    }

    private static void RecordFieldOccurrence(Dictionary<string, List<string>> seenValues, string fieldKey, string rawValue)
    {
        if (!seenValues.TryGetValue(fieldKey, out var list))
        {
            list = new List<string>(1);
            seenValues[fieldKey] = list;
        }
        list.Add(rawValue);
    }

    private static string FormatValueWithUnit(string value, string unit) =>
        string.IsNullOrEmpty(unit) ? value : $"{value} {unit}";

    private static ImportField<LocalPoint2D> ParseLocalPosition(
        IReadOnlyDictionary<string, string> coreTerms,
        string sourceId,
        FieldProvenance provenance,
        List<ImportValidationIssue> issues)
    {
        string? verbatim = GetTerm(coreTerms, "verbatimCoordinates");
        if (!string.IsNullOrWhiteSpace(verbatim))
        {
            string trimmed = verbatim.Trim();
            string[] parts = trimmed.Contains(',')
                ? trimmed.Split(',', StringSplitOptions.TrimEntries)
                : (trimmed.Contains(';')
                    ? trimmed.Split(';', StringSplitOptions.TrimEntries)
                    : trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
            {
                return new ImportField<LocalPoint2D>
                {
                    Provenance = provenance,
                    HasValue = true,
                    Value = new LocalPoint2D(x, y),
                    Evidence = new FieldEvidence("verbatimCoordinates", verbatim),
                };
            }

            issues.Add(new ImportValidationIssue(
                "MalformedTreeCoordinates",
                IssueSeverity.Refusal,
                $"Tree '{sourceId}' has unparsable verbatimCoordinates '{verbatim}'.",
                SourceTreeId: sourceId,
                FieldName: nameof(ImportedTreeRecord.PositionLocalM)));
            return ImportField<LocalPoint2D>.Missing(new FieldEvidence("verbatimCoordinates", verbatim, "Malformed coordinate string"));
        }

        string? xRaw = GetTerm(coreTerms, "localXM") ?? GetTerm(coreTerms, "xM");
        string? yRaw = GetTerm(coreTerms, "localYM") ?? GetTerm(coreTerms, "yM");
        if (!string.IsNullOrWhiteSpace(xRaw) || !string.IsNullOrWhiteSpace(yRaw))
        {
            if (double.TryParse(xRaw?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
                double.TryParse(yRaw?.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
            {
                return new ImportField<LocalPoint2D>
                {
                    Provenance = provenance,
                    HasValue = true,
                    Value = new LocalPoint2D(x, y),
                    Evidence = new FieldEvidence("localXM/localYM", $"{xRaw},{yRaw}"),
                };
            }

            issues.Add(new ImportValidationIssue(
                "MalformedTreeCoordinates",
                IssueSeverity.Refusal,
                $"Tree '{sourceId}' has unparsable local coordinates ('{xRaw}', '{yRaw}').",
                SourceTreeId: sourceId,
                FieldName: nameof(ImportedTreeRecord.PositionLocalM)));
        }

        return ImportField<LocalPoint2D>.Missing(new FieldEvidence("verbatimCoordinates", verbatim));
    }

    private static ImportField<TreeVitalStatus> ParseVitalStatus(
        string rawValue,
        string sourceCol,
        FieldProvenance provenance)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return ImportField<TreeVitalStatus>.Missing(new FieldEvidence(sourceCol, rawValue));
        }

        string normalized = rawValue.Trim().ToLowerInvariant();
        TreeVitalStatus? parsed = normalized switch
        {
            "1" or "alive" or "living" or "live" or "true" => TreeVitalStatus.Alive,
            "0" or "dead" or "standing dead" or "snag" or "false" => TreeVitalStatus.Dead,
            _ => null,
        };

        if (parsed is null)
        {
            // Preserve declared provenance honestly (never launder Inferred or Simulated into Observed).
            return new ImportField<TreeVitalStatus>
            {
                Provenance = provenance,
                HasValue = true,
                Value = TreeVitalStatus.Unknown,
                Evidence = new FieldEvidence(sourceCol, rawValue, "Unrecognized status code; counted conservatively as living."),
            };
        }

        return new ImportField<TreeVitalStatus>
        {
            Provenance = provenance,
            HasValue = true,
            Value = parsed.Value,
            Evidence = new FieldEvidence(sourceCol, rawValue),
        };
    }

    private static ImportField<double> ParseDoubleMeasurement(
        string rawValue,
        string rawUnit,
        string targetUnit,
        string sourceId,
        string fieldName,
        string sourceCol,
        FieldProvenance provenance,
        List<ImportValidationIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return ImportField<double>.Missing(new FieldEvidence(sourceCol, rawValue));
        }

        if (!double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
        {
            issues.Add(new ImportValidationIssue(
                "MalformedMeasurementValue",
                IssueSeverity.Refusal,
                $"Tree '{sourceId}' field '{fieldName}' has non-numeric value '{rawValue}'.",
                SourceTreeId: sourceId,
                FieldName: fieldName));
            return ImportField<double>.Missing(new FieldEvidence(sourceCol, rawValue, "Malformed numeric value"));
        }

        string unit = rawUnit.Trim().ToLowerInvariant();
        if (unit.Length > 0)
        {
            if (targetUnit == "cm")
            {
                if (unit == "cm") { }
                else if (unit == "mm") val /= 10.0;
                else if (unit == "m") val *= 100.0;
                else
                {
                    issues.Add(new ImportValidationIssue(
                        "UnsupportedMeasurementUnit",
                        IssueSeverity.Refusal,
                        $"Tree '{sourceId}' field '{fieldName}' specifies unsupported unit '{rawUnit}'; expected cm, mm, or m.",
                        SourceTreeId: sourceId,
                        FieldName: fieldName));
                    return ImportField<double>.Missing(new FieldEvidence(sourceCol, rawValue, $"Unsupported unit '{rawUnit}'"));
                }
            }
            else if (targetUnit == "m")
            {
                if (unit == "m") { }
                else if (unit == "cm") val /= 100.0;
                else if (unit == "mm") val /= 1000.0;
                else
                {
                    issues.Add(new ImportValidationIssue(
                        "UnsupportedMeasurementUnit",
                        IssueSeverity.Refusal,
                        $"Tree '{sourceId}' field '{fieldName}' specifies unsupported unit '{rawUnit}'; expected m, cm, or mm.",
                        SourceTreeId: sourceId,
                        FieldName: fieldName));
                    return ImportField<double>.Missing(new FieldEvidence(sourceCol, rawValue, $"Unsupported unit '{rawUnit}'"));
                }
            }
        }

        return new ImportField<double>
        {
            Provenance = provenance,
            HasValue = true,
            Value = val,
            Evidence = new FieldEvidence(sourceCol, rawValue, string.IsNullOrEmpty(rawUnit) ? null : $"unit={rawUnit}"),
        };
    }

    private static ImportField<int> ParseIntMeasurement(
        string rawValue,
        string sourceId,
        string fieldName,
        string sourceCol,
        FieldProvenance provenance,
        List<ImportValidationIssue> issues)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return ImportField<int>.Missing(new FieldEvidence(sourceCol, rawValue));
        }

        if (!int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out int val))
        {
            issues.Add(new ImportValidationIssue(
                "MalformedMeasurementValue",
                IssueSeverity.Refusal,
                $"Tree '{sourceId}' field '{fieldName}' has non-integer value '{rawValue}'.",
                SourceTreeId: sourceId,
                FieldName: fieldName));
            return ImportField<int>.Missing(new FieldEvidence(sourceCol, rawValue, "Malformed integer value"));
        }

        return new ImportField<int>
        {
            Provenance = provenance,
            HasValue = true,
            Value = val,
            Evidence = new FieldEvidence(sourceCol, rawValue),
        };
    }

    private static FieldProvenance ParseDeclaredProvenance(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return FieldProvenance.Observed;
        string s = raw.Trim();
        if (s.Equals("simulated", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("provenance=simulated", StringComparison.OrdinalIgnoreCase))
        {
            return FieldProvenance.Simulated;
        }
        if (s.Equals("inferred", StringComparison.OrdinalIgnoreCase) ||
            s.Contains("provenance=inferred", StringComparison.OrdinalIgnoreCase))
        {
            return FieldProvenance.Inferred;
        }
        return FieldProvenance.Observed;
    }

    private static bool IsStatusRow(string type, string remarks) =>
        remarks.Equals("status", StringComparison.OrdinalIgnoreCase) ||
        type.Contains("status", StringComparison.OrdinalIgnoreCase);

    private static bool IsDiameterRow(string type, string remarks) =>
        remarks.Equals("diameter", StringComparison.OrdinalIgnoreCase) ||
        remarks.Equals("dbh", StringComparison.OrdinalIgnoreCase) ||
        type.Contains("diameter", StringComparison.OrdinalIgnoreCase) ||
        type.Contains("dbh", StringComparison.OrdinalIgnoreCase);

    private static bool IsHeightRow(string type, string remarks) =>
        remarks.Equals("height", StringComparison.OrdinalIgnoreCase) ||
        type.Equals("tree height", StringComparison.OrdinalIgnoreCase) ||
        type.Equals("height", StringComparison.OrdinalIgnoreCase);

    private static bool IsCrownRadiusRow(string type, string remarks) =>
        remarks.Equals("crownRadius", StringComparison.OrdinalIgnoreCase) ||
        type.Contains("crown radius", StringComparison.OrdinalIgnoreCase);

    private static bool IsAgeRow(string type, string remarks) =>
        remarks.Equals("age", StringComparison.OrdinalIgnoreCase) ||
        type.Equals("tree age", StringComparison.OrdinalIgnoreCase) ||
        type.Equals("age", StringComparison.OrdinalIgnoreCase);

    internal static string ExtractPlotId(DwcJoinedRecord rec)
    {
        string? explicitPlot = GetTerm(rec.CoreTerms, "locationID")
            ?? GetTerm(rec.CoreTerms, "eventID")
            ?? GetTerm(rec.CoreTerms, "locality");
        if (!string.IsNullOrWhiteSpace(explicitPlot))
        {
            return explicitPlot.Trim();
        }

        // Integrate+ occurrenceID pattern: "{numericId}_{plotName}" or "{numericId}_{plotName}_{surveyNum}"
        // Examples: "24839_IPlus_Donadea_MHF_2", "11290_IPlus_DE-Bensheim"
        string id = rec.CoreId;
        int firstUnderscore = id.IndexOf('_');
        if (firstUnderscore > 0 && firstUnderscore + 1 < id.Length)
        {
            return id[(firstUnderscore + 1)..];
        }
        return id;
    }

    private static string BuildStandId(string plotName, string? archiveSha256)
    {
        string slug = new string(plotName
            .Select(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' ? char.ToLowerInvariant(c) : '-')
            .ToArray())
            .Trim('-');
        if (string.IsNullOrEmpty(slug)) slug = "stand";
        string shortHash = !string.IsNullOrEmpty(archiveSha256) && archiveSha256.Length >= 12
            ? archiveSha256[..12]
            : "000000000000";
        return $"wi-v1-{slug}-{shortHash}";
    }

    private static ImportSourceAttribution ResolveAttribution(
        WoodlandImportRequest request,
        DwcArchiveReadResult archive,
        string plotName)
    {
        if (request.AttributionOverride is not null)
        {
            return request.AttributionOverride with
            {
                ArchiveSha256 = request.AttributionOverride.ArchiveSha256 ?? archive.ArchiveSha256,
                SourcePlotId = request.AttributionOverride.SourcePlotId ?? plotName,
            };
        }

        string? datasetId = null;
        string? datasetName = null;
        if (archive.Records.Count > 0)
        {
            datasetId = GetTerm(archive.Records[0].CoreTerms, "datasetID");
            datasetName = GetTerm(archive.Records[0].CoreTerms, "datasetName");
        }

        return new ImportSourceAttribution
        {
            DatasetTitle = string.IsNullOrWhiteSpace(datasetName) ? DefaultDatasetTitle : datasetName.Trim(),
            Publisher = DefaultPublisher,
            Licence = DefaultLicence,
            Citation = "Kraus D, Schuck A, et al. (2022). Spatially explicit database of tree related microhabitats (TreMs). INRAE. https://doi.org/10.15468/ocof3v",
            Doi = string.IsNullOrWhiteSpace(datasetId) ? DefaultDoi : datasetId.Trim(),
            DatasetKey = DefaultDatasetKey,
            ArchiveSha256 = archive.ArchiveSha256,
            RetrievalDateUtc = null,
            SourcePlotId = plotName,
        };
    }

    private static string? GetTerm(IReadOnlyDictionary<string, string> terms, string key)
    {
        if (terms.TryGetValue(key, out string? val) && !string.IsNullOrWhiteSpace(val))
        {
            return val;
        }
        return null;
    }

    private static void CopyExtraIfPresent(
        IReadOnlyDictionary<string, string> source,
        SortedDictionary<string, string> target,
        string sourceKey,
        string targetKey)
    {
        if (source.TryGetValue(sourceKey, out string? val) && !string.IsNullOrWhiteSpace(val))
        {
            target[targetKey] = val.Trim();
        }
    }
}
