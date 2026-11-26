using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Canonical, deterministic UTF-8 JSON serializer and SHA-256 hasher for <see cref="WoodlandImportResult"/>
/// and <see cref="WoodlandImportStand"/>. Uses <see cref="Utf8JsonWriter"/> with fixed key order,
/// invariant number formatting, and zero wall-clock or machine-dependent values.
/// </summary>
public static class WoodlandImportSerializer
{
    public static byte[] SerializeResultUtf8(WoodlandImportResult result)
    {
        using var ms = new MemoryStream();
        using (var w = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false }))
        {
            w.WriteStartObject();
            w.WriteString("readiness", result.Readiness.ToString());
            w.WriteBoolean("isAdmissibleForSimulation", result.IsAdmissibleForSimulation);

            w.WritePropertyName("stand");
            if (result.Stand is null)
            {
                w.WriteNullValue();
            }
            else
            {
                WriteStand(w, result.Stand);
            }

            w.WritePropertyName("issues");
            w.WriteStartArray();
            for (int i = 0; i < result.Issues.Count; i++)
            {
                ImportValidationIssue issue = result.Issues[i];
                w.WriteStartObject();
                w.WriteString("code", issue.Code);
                w.WriteString("severity", issue.Severity.ToString());
                w.WriteString("message", issue.Message);
                WriteNullableString(w, "sourceTreeId", issue.SourceTreeId);
                WriteNullableString(w, "fieldName", issue.FieldName);
                w.WriteEndObject();
            }
            w.WriteEndArray();
            w.WriteEndObject();
        }
        return ms.ToArray();
    }

    public static string SerializeResultJson(WoodlandImportResult result) =>
        Encoding.UTF8.GetString(SerializeResultUtf8(result));

    public static string ComputeResultSha256Hex(WoodlandImportResult result) =>
        ComputeSha256Hex(SerializeResultUtf8(result));

    public static string ComputeSha256Hex(ReadOnlySpan<byte> bytes)
    {
        byte[] hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static void WriteStand(Utf8JsonWriter w, WoodlandImportStand stand)
    {
        w.WriteStartObject();
        w.WriteNumber("schemaVersion", stand.SchemaVersion);
        w.WriteString("standId", stand.StandId);
        w.WriteString("plotName", stand.PlotName);
        WriteNullableString(w, "countryCode", stand.CountryCode);

        w.WritePropertyName("attribution");
        WriteAttribution(w, stand.Attribution);

        w.WritePropertyName("boundary");
        if (stand.Boundary is null)
        {
            w.WriteNullValue();
        }
        else
        {
            WriteBoundary(w, stand.Boundary);
        }

        w.WritePropertyName("coordinateOrigin");
        if (stand.CoordinateOrigin is null)
        {
            w.WriteNullValue();
        }
        else
        {
            w.WriteStartObject();
            WriteCanonicalDouble(w, "originXM", stand.CoordinateOrigin.OriginXM);
            WriteCanonicalDouble(w, "originYM", stand.CoordinateOrigin.OriginYM);
            w.WriteString("derivationRule", stand.CoordinateOrigin.DerivationRule);
            w.WriteEndObject();
        }

        w.WriteNumber("totalArchiveRowsScanned", stand.TotalArchiveRowsScanned);
        w.WriteNumber("totalMeasurementRowsScanned", stand.TotalMeasurementRowsScanned);
        w.WriteNumber("archiveUncompressedBytes", stand.ArchiveUncompressedBytes);
        w.WriteNumber("livingStemCount", stand.LivingStemCount);
        w.WriteNumber("deadStemCount", stand.DeadStemCount);
        w.WriteNumber("unknownStatusStemCount", stand.UnknownStatusStemCount);

        w.WritePropertyName("trees");
        w.WriteStartArray();
        for (int i = 0; i < stand.Trees.Count; i++)
        {
            WriteTree(w, stand.Trees[i]);
        }
        w.WriteEndArray();

        w.WriteEndObject();
    }

    private static void WriteAttribution(Utf8JsonWriter w, ImportSourceAttribution attr)
    {
        w.WriteStartObject();
        w.WriteString("datasetTitle", attr.DatasetTitle);
        w.WriteString("publisher", attr.Publisher);
        w.WriteString("licence", attr.Licence);
        w.WriteString("citation", attr.Citation);
        WriteNullableString(w, "doi", attr.Doi);
        WriteNullableString(w, "datasetKey", attr.DatasetKey);
        WriteNullableString(w, "archiveSha256", attr.ArchiveSha256);
        WriteNullableString(w, "retrievalDateUtc", attr.RetrievalDateUtc);
        WriteNullableString(w, "sourcePlotId", attr.SourcePlotId);
        w.WriteEndObject();
    }

    private static void WriteBoundary(Utf8JsonWriter w, WoodlandBoundary b)
    {
        w.WriteStartObject();
        w.WriteString("coordinateSource", b.CoordinateSource.ToString());
        WriteCanonicalDouble(w, "areaM2", b.AreaM2);
        WriteCanonicalDouble(w, "areaHa", b.AreaHa);
        w.WriteString("projectionNote", b.ProjectionNote);

        w.WritePropertyName("geographicCentroid");
        if (b.GeographicCentroid is null)
        {
            w.WriteNullValue();
        }
        else
        {
            w.WriteStartObject();
            WriteCanonicalDouble(w, "latitudeDeg", b.GeographicCentroid.Value.LatitudeDeg);
            WriteCanonicalDouble(w, "longitudeDeg", b.GeographicCentroid.Value.LongitudeDeg);
            w.WriteEndObject();
        }

        w.WritePropertyName("verticesLocalM");
        w.WriteStartArray();
        for (int i = 0; i < b.VerticesLocalM.Count; i++)
        {
            w.WriteStartObject();
            WriteCanonicalDouble(w, "xM", b.VerticesLocalM[i].XM);
            WriteCanonicalDouble(w, "yM", b.VerticesLocalM[i].YM);
            w.WriteEndObject();
        }
        w.WriteEndArray();
        w.WriteEndObject();
    }

    private static void WriteTree(Utf8JsonWriter w, ImportedTreeRecord t)
    {
        w.WriteStartObject();
        w.WriteString("sourceTreeId", t.SourceTreeId);

        w.WritePropertyName("rawSpeciesName");
        WriteField(w, t.RawSpeciesName, static (wr, v) => wr.WriteStringValue(v));

        w.WritePropertyName("speciesMapping");
        w.WriteStartObject();
        WriteNullableString(w, "canonicalSpeciesId", t.SpeciesMapping.CanonicalSpeciesId);
        w.WriteString("category", t.SpeciesMapping.Category.ToString());
        w.WriteString("rawName", t.SpeciesMapping.RawName);
        w.WriteString("normalizedInput", t.SpeciesMapping.NormalizedInput);
        WriteNullableString(w, "matchedScientificName", t.SpeciesMapping.MatchedScientificName);
        w.WriteEndObject();

        w.WritePropertyName("vitalStatus");
        WriteField(w, t.VitalStatus, static (wr, v) => wr.WriteStringValue(v.ToString()));

        w.WritePropertyName("positionLocalM");
        WriteField(w, t.PositionLocalM, static (wr, v) =>
        {
            wr.WriteStartObject();
            WriteCanonicalDouble(wr, "xM", v.XM);
            WriteCanonicalDouble(wr, "yM", v.YM);
            wr.WriteEndObject();
        });

        w.WritePropertyName("dbhCm");
        WriteField(w, t.DbhCm, static (wr, v) => WriteCanonicalDoubleValue(wr, v));

        w.WritePropertyName("heightM");
        WriteField(w, t.HeightM, static (wr, v) => WriteCanonicalDoubleValue(wr, v));

        w.WritePropertyName("crownRadiusM");
        WriteField(w, t.CrownRadiusM, static (wr, v) => WriteCanonicalDoubleValue(wr, v));

        w.WritePropertyName("ageYears");
        WriteField(w, t.AgeYears, static (wr, v) => wr.WriteNumberValue(v));

        w.WritePropertyName("extraAttributes");
        w.WriteStartObject();
        if (t.ExtraAttributes.Count > 0)
        {
            var keys = new string[t.ExtraAttributes.Count];
            int idx = 0;
            foreach (string k in t.ExtraAttributes.Keys) keys[idx++] = k;
            Array.Sort(keys, StringComparer.Ordinal);
            for (int i = 0; i < keys.Length; i++)
            {
                w.WriteString(keys[i], t.ExtraAttributes[keys[i]]);
            }
        }
        w.WriteEndObject();

        w.WriteEndObject();
    }

    private static void WriteField<T>(Utf8JsonWriter w, ImportField<T> field, Action<Utf8JsonWriter, T> writeValue)
    {
        w.WriteStartObject();
        w.WriteString("provenance", field.Provenance.ToString());
        w.WriteBoolean("hasValue", field.HasValue);
        w.WritePropertyName("value");
        if (!field.HasValue || field.Value is null)
        {
            w.WriteNullValue();
        }
        else
        {
            writeValue(w, field.Value);
        }

        w.WritePropertyName("evidence");
        if (field.Evidence is null)
        {
            w.WriteNullValue();
        }
        else
        {
            w.WriteStartObject();
            w.WriteString("sourceColumn", field.Evidence.SourceColumn);
            WriteNullableString(w, "rawValue", field.Evidence.RawValue);
            WriteNullableString(w, "note", field.Evidence.Note);
            w.WriteEndObject();
        }
        w.WriteEndObject();
    }

    private static void WriteNullableString(Utf8JsonWriter w, string propertyName, string? value)
    {
        if (value is null)
        {
            w.WriteNull(propertyName);
        }
        else
        {
            w.WriteString(propertyName, value);
        }
    }

    private static void WriteCanonicalDouble(Utf8JsonWriter w, string propertyName, double value)
    {
        w.WritePropertyName(propertyName);
        WriteCanonicalDoubleValue(w, value);
    }

    private static void WriteCanonicalDoubleValue(Utf8JsonWriter w, double value)
    {
        if (!double.IsFinite(value))
        {
            w.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
            return;
        }
        // Normalize -0.0 to 0.0 so canonical JSON is strictly deterministic.
        double normalized = value == 0.0 ? 0.0 : value;
        w.WriteNumberValue(normalized);
    }
}
