using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>
/// Canonical UTF-8 JSON for a <see cref="WoodlandInspectionView"/>: fixed key order, invariant numbers,
/// -0.0 written as 0, and no clock or machine-dependent values, so equal views always hash equally.
/// </summary>
public static class WoodlandInspectionSerializer
{
    public static byte[] SerializeUtf8(WoodlandInspectionView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        using var ms = new MemoryStream();
        using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
        {
            w.WriteStartObject();
            w.WriteNumber("schemaVersion", view.SchemaVersion);
            w.WriteString("readiness", view.Readiness.ToString());
            w.WriteBoolean("canAdvanceTime", view.CanAdvanceTime);
            w.WriteString("advanceTimeStatement", view.AdvanceTimeStatement);

            w.WritePropertyName("stand");
            WriteStand(w, view.Stand);

            w.WritePropertyName("observedYear0");
            WriteObservedYear0(w, view.ObservedYear0);

            w.WritePropertyName("speciesBreakdown");
            WriteSpeciesBreakdown(w, view.SpeciesBreakdown);

            w.WritePropertyName("microhabitats");
            w.WriteStartArray();
            foreach (InspectedMicrohabitat m in view.Microhabitats)
            {
                w.WriteStartObject();
                w.WriteString("sourceTreeId", m.SourceTreeId);
                w.WriteString("code", m.Code);
                w.WriteString("value", m.Value);
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WritePropertyName("attribution");
            WriteAttribution(w, view.Attribution);

            w.WritePropertyName("disclosures");
            WriteDisclosures(w, view.Disclosures);

            w.WritePropertyName("provenanceBoundary");
            WriteProvenanceBoundary(w, view.ProvenanceBoundary);

            w.WritePropertyName("blockerChecklist");
            w.WriteStartArray();
            foreach (BlockerChecklistItem item in view.BlockerChecklist)
            {
                w.WriteStartObject();
                w.WriteString("code", item.Code);
                w.WriteString("severity", item.Severity.ToString());
                w.WriteString("title", item.Title);
                w.WriteString("explanation", item.Explanation);
                w.WriteNumber("affectedTreeCount", item.AffectedTreeCount);
                WriteStringArray(w, "affectedTreeIds", item.AffectedTreeIds);
                WriteStringArray(w, "affectedRawSpeciesNames", item.AffectedRawSpeciesNames);
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WritePropertyName("notes");
            w.WriteStartArray();
            foreach (InspectionNote note in view.Notes)
            {
                w.WriteStartObject();
                w.WriteString("code", note.Code);
                w.WriteString("severity", note.Severity.ToString());
                w.WriteString("message", note.Message);
                WriteNullableString(w, "sourceTreeId", note.SourceTreeId);
                WriteNullableString(w, "fieldName", note.FieldName);
                w.WriteEndObject();
            }
            w.WriteEndArray();

            w.WriteEndObject();
        }
        return ms.ToArray();
    }

    public static string SerializeJson(WoodlandInspectionView view) =>
        Encoding.UTF8.GetString(SerializeUtf8(view));

    public static string ComputeSha256Hex(WoodlandInspectionView view) =>
        WoodlandImportSerializer.ComputeSha256Hex(SerializeUtf8(view));

    private static void WriteStand(Utf8JsonWriter w, InspectedStandIdentity? stand)
    {
        if (stand is null)
        {
            w.WriteNullValue();
            return;
        }

        w.WriteStartObject();
        w.WriteNumber("standSchemaVersion", stand.StandSchemaVersion);
        w.WriteString("standId", stand.StandId);
        w.WriteString("plotName", stand.PlotName);
        WriteNullableString(w, "countryCode", stand.CountryCode);
        w.WriteNumber("treeCount", stand.TreeCount);
        w.WriteNumber("livingStemCount", stand.LivingStemCount);
        w.WriteNumber("deadStemCount", stand.DeadStemCount);
        w.WriteNumber("unknownStatusStemCount", stand.UnknownStatusStemCount);
        w.WriteEndObject();
    }

    private static void WriteObservedYear0(Utf8JsonWriter w, ObservedYear0Section section)
    {
        w.WriteStartObject();

        w.WritePropertyName("coordinateOrigin");
        if (section.CoordinateOrigin is null)
        {
            w.WriteNullValue();
        }
        else
        {
            w.WriteStartObject();
            WriteDouble(w, "originXM", section.CoordinateOrigin.OriginXM);
            WriteDouble(w, "originYM", section.CoordinateOrigin.OriginYM);
            w.WriteString("derivationRule", section.CoordinateOrigin.DerivationRule);
            w.WriteEndObject();
        }

        w.WritePropertyName("boundary");
        InspectedBoundary b = section.Boundary;
        w.WriteStartObject();
        w.WriteBoolean("observed", b.Observed);
        w.WritePropertyName("verticesLocalM");
        w.WriteStartArray();
        foreach (LocalPoint2D p in b.VerticesLocalM)
        {
            WritePoint(w, p);
        }
        w.WriteEndArray();
        WriteNullableDouble(w, "areaM2", b.AreaM2);
        WriteNullableDouble(w, "areaHa", b.AreaHa);
        WriteNullableString(w, "coordinateSource", b.CoordinateSource?.ToString());
        WriteNullableString(w, "projectionNote", b.ProjectionNote);
        w.WriteEndObject();

        w.WritePropertyName("trees");
        w.WriteStartArray();
        foreach (InspectedTree t in section.Trees)
        {
            WriteTree(w, t);
        }
        w.WriteEndArray();

        w.WriteEndObject();
    }

    private static void WriteTree(Utf8JsonWriter w, InspectedTree t)
    {
        w.WriteStartObject();
        w.WriteString("sourceTreeId", t.SourceTreeId);
        w.WriteString("speciesRawName", t.SpeciesRawName);
        WriteNullableString(w, "canonicalSpeciesId", t.CanonicalSpeciesId);
        w.WriteString("speciesCategory", t.SpeciesCategory.ToString());
        w.WriteBoolean("isConservativelyLiving", t.IsConservativelyLiving);

        WriteField(w, "speciesName", t.SpeciesName, static (wr, v) => wr.WriteStringValue(v));
        WriteField(w, "vitalStatus", t.VitalStatus, static (wr, v) => wr.WriteStringValue(v!.Value.ToString()));
        WriteField(w, "positionLocalM", t.PositionLocalM, static (wr, v) => WritePoint(wr, v!.Value));
        WriteField(w, "dbhCm", t.DbhCm, static (wr, v) => WriteDoubleValue(wr, v!.Value));
        WriteField(w, "heightM", t.HeightM, static (wr, v) => WriteDoubleValue(wr, v!.Value));
        WriteField(w, "crownRadiusM", t.CrownRadiusM, static (wr, v) => WriteDoubleValue(wr, v!.Value));
        WriteField(w, "ageYears", t.AgeYears, static (wr, v) => wr.WriteNumberValue(v!.Value));
        w.WriteEndObject();
    }

    private static void WriteField<T>(Utf8JsonWriter w, string name, InspectedField<T> field, Action<Utf8JsonWriter, T> writeValue)
    {
        w.WritePropertyName(name);
        w.WriteStartObject();
        w.WriteString("provenance", field.Provenance.ToString());
        w.WritePropertyName("value");
        if (field.Value is null)
        {
            w.WriteNullValue();
        }
        else
        {
            writeValue(w, field.Value);
        }
        WriteNullableString(w, "sourceColumn", field.SourceColumn);
        WriteNullableString(w, "rawValue", field.RawValue);
        WriteNullableString(w, "note", field.Note);
        w.WriteEndObject();
    }

    private static void WriteSpeciesBreakdown(Utf8JsonWriter w, SpeciesBreakdown breakdown)
    {
        w.WriteStartObject();
        w.WritePropertyName("categories");
        w.WriteStartArray();
        foreach (SpeciesCategoryCount c in breakdown.Categories)
        {
            w.WriteStartObject();
            w.WriteString("category", c.Category.ToString());
            w.WriteNumber("totalTrees", c.TotalTrees);
            w.WriteNumber("livingTrees", c.LivingTrees);
            w.WriteEndObject();
        }
        w.WriteEndArray();

        w.WritePropertyName("species");
        w.WriteStartArray();
        foreach (SpeciesRow row in breakdown.Species)
        {
            w.WriteStartObject();
            w.WriteString("rawName", row.RawName);
            WriteNullableString(w, "canonicalSpeciesId", row.CanonicalSpeciesId);
            w.WriteString("category", row.Category.ToString());
            w.WriteNumber("totalTrees", row.TotalTrees);
            w.WriteNumber("livingTrees", row.LivingTrees);
            w.WriteEndObject();
        }
        w.WriteEndArray();
        w.WriteEndObject();
    }

    private static void WriteAttribution(Utf8JsonWriter w, InspectedAttribution? a)
    {
        if (a is null)
        {
            w.WriteNullValue();
            return;
        }

        w.WriteStartObject();
        w.WriteString("datasetTitle", a.DatasetTitle);
        w.WriteString("publisher", a.Publisher);
        w.WriteString("licence", a.Licence);
        w.WriteString("citation", a.Citation);
        WriteNullableString(w, "doi", a.Doi);
        WriteNullableString(w, "datasetKey", a.DatasetKey);
        WriteNullableString(w, "archiveSha256", a.ArchiveSha256);
        WriteNullableString(w, "declaredArchiveSha256", a.DeclaredArchiveSha256);
        WriteNullableString(w, "retrievalDateUtc", a.RetrievalDateUtc);
        WriteNullableString(w, "sourcePlotId", a.SourcePlotId);
        WriteStringArray(w, "observationDates", a.ObservationDates);
        w.WriteEndObject();
    }

    private static void WriteDisclosures(Utf8JsonWriter w, InspectionDisclosures? d)
    {
        if (d is null)
        {
            w.WriteNullValue();
            return;
        }

        w.WriteStartObject();
        w.WriteString("modificationsDisclosure", d.ModificationsDisclosure);
        w.WriteString("coordinateInterpretationCaveat", d.CoordinateInterpretationCaveat);
        w.WriteEndObject();
    }

    private static void WriteProvenanceBoundary(Utf8JsonWriter w, ProvenanceBoundarySection p)
    {
        w.WriteStartObject();
        w.WriteNumber("observedStateYear", p.ObservedStateYear);
        WriteStringArray(w, "observationDates", p.ObservationDates);
        w.WriteNumber("simulatedFutureStartsAtYear", p.SimulatedFutureStartsAtYear);
        w.WriteBoolean("simulatedFutureAvailable", p.SimulatedFutureAvailable);
        w.WriteNumber("simulatedValueCount", p.SimulatedValueCount);
        w.WriteString("statement", p.Statement);
        w.WriteEndObject();
    }

    private static void WritePoint(Utf8JsonWriter w, LocalPoint2D p)
    {
        w.WriteStartObject();
        WriteDouble(w, "xM", p.XM);
        WriteDouble(w, "yM", p.YM);
        w.WriteEndObject();
    }

    private static void WriteStringArray(Utf8JsonWriter w, string name, IReadOnlyList<string> values)
    {
        w.WritePropertyName(name);
        w.WriteStartArray();
        foreach (string v in values)
        {
            w.WriteStringValue(v);
        }
        w.WriteEndArray();
    }

    private static void WriteNullableString(Utf8JsonWriter w, string name, string? value)
    {
        if (value is null)
        {
            w.WriteNull(name);
        }
        else
        {
            w.WriteString(name, value);
        }
    }

    private static void WriteNullableDouble(Utf8JsonWriter w, string name, double? value)
    {
        if (value is null)
        {
            w.WriteNull(name);
        }
        else
        {
            WriteDouble(w, name, value.Value);
        }
    }

    private static void WriteDouble(Utf8JsonWriter w, string name, double value)
    {
        w.WritePropertyName(name);
        WriteDoubleValue(w, value);
    }

    private static void WriteDoubleValue(Utf8JsonWriter w, double value)
    {
        // JSON has no NaN/Infinity; write them as invariant strings rather than throwing.
        if (!double.IsFinite(value))
        {
            w.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            return;
        }
        // -0.0 == 0.0 is true, so this maps negative zero to positive zero.
        w.WriteNumberValue(value == 0.0 ? 0.0 : value);
    }
}
