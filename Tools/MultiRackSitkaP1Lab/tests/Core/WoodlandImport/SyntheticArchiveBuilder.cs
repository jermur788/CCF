using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace SitkaP1.Core.WoodlandImport.Tests;

internal static class SyntheticArchiveBuilder
{
    public sealed record SyntheticTreeSpec(
        string OccurrenceId,
        string ScientificName,
        string VerbatimCoordinates,
        string? DbhCm,
        string? HeightM,
        string? StatusCode,
        string CountryCode = "IE",
        string Habitat = "Synthetic woodland",
        string? MeasurementMethodNote = null,
        string? ExtraMicrohabitat = null,
        string? CoreProvenance = null);

    public sealed record RawMofRow(
        string CoreId,
        string MeasurementType,
        string MeasurementRemarks,
        string MeasurementValue,
        string MeasurementUnit,
        string MeasurementMethod);

    public static byte[] BuildTreMsArchive(
        IReadOnlyList<SyntheticTreeSpec> trees,
        string fieldDelimiter = "\t",
        string? quoteChar = null,
        string? customMetaXml = null,
        bool includeExtension = true,
        IReadOnlyList<(string EntryName, byte[] RawBytes)>? extraEntries = null,
        IReadOnlyList<RawMofRow>? extraMofRows = null,
        bool includeCoreProvenanceColumn = false,
        string corePrefix = "",
        CompressionLevel compression = CompressionLevel.NoCompression)
    {
        string delimAttr = fieldDelimiter == "\t" ? "\\t" : fieldDelimiter;
        string quoteAttr = (quoteChar ?? string.Empty).Replace("\"", "&quot;", StringComparison.Ordinal);

        string metaXml = customMetaXml ?? BuildDefaultMetaXml(delimAttr, quoteAttr, includeExtension, includeCoreProvenanceColumn);

        var occSb = new StringBuilder();
        occSb.Append(corePrefix);
        if (includeCoreProvenanceColumn)
        {
            occSb.Append(JoinCols(fieldDelimiter, quoteChar,
                "occurrenceID", "scientificName", "verbatimCoordinates", "countryCode", "habitat", "datasetID", "datasetName", "provenance"));
        }
        else
        {
            occSb.Append(JoinCols(fieldDelimiter, quoteChar,
                "occurrenceID", "scientificName", "verbatimCoordinates", "countryCode", "habitat", "datasetID", "datasetName"));
        }
        occSb.Append('\n');

        var mofSb = new StringBuilder();
        mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
            "occurrenceID", "measurementType", "measurementRemarks", "measurementValue", "measurementUnit", "measurementMethod"));
        mofSb.Append('\n');

        foreach (SyntheticTreeSpec t in trees)
        {
            if (includeCoreProvenanceColumn)
            {
                occSb.Append(JoinCols(fieldDelimiter, quoteChar,
                    t.OccurrenceId, t.ScientificName, t.VerbatimCoordinates, t.CountryCode, t.Habitat,
                    "https://doi.org/10.15468/ocof3v", "trems dataset", t.CoreProvenance ?? string.Empty));
            }
            else
            {
                occSb.Append(JoinCols(fieldDelimiter, quoteChar,
                    t.OccurrenceId, t.ScientificName, t.VerbatimCoordinates, t.CountryCode, t.Habitat,
                    "https://doi.org/10.15468/ocof3v", "trems dataset"));
            }
            occSb.Append('\n');

            if (includeExtension)
            {
                string method = t.MeasurementMethodNote ?? "field measurement";
                if (t.DbhCm is not null)
                {
                    mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
                        t.OccurrenceId, "Tree diameter", "diameter", t.DbhCm, "cm", method));
                    mofSb.Append('\n');
                }
                if (t.HeightM is not null)
                {
                    mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
                        t.OccurrenceId, "tree height", "height", t.HeightM, "m", method));
                    mofSb.Append('\n');
                }
                if (t.StatusCode is not null)
                {
                    mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
                        t.OccurrenceId, "Tree status (dead or living)", "status", t.StatusCode, "", method));
                    mofSb.Append('\n');
                }
                if (t.ExtraMicrohabitat is not null)
                {
                    mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
                        t.OccurrenceId, "Tree related micro-habitat", t.ExtraMicrohabitat, "1", "", method));
                    mofSb.Append('\n');
                }
            }
        }

        if (includeExtension && extraMofRows is not null)
        {
            foreach (RawMofRow r in extraMofRows)
            {
                mofSb.Append(JoinCols(fieldDelimiter, quoteChar,
                    r.CoreId, r.MeasurementType, r.MeasurementRemarks, r.MeasurementValue, r.MeasurementUnit, r.MeasurementMethod));
                mofSb.Append('\n');
            }
        }

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            WriteUtf8Entry(zip, "meta.xml", metaXml, compression);
            WriteUtf8Entry(zip, "occurrence.txt", occSb.ToString(), compression);
            if (includeExtension)
            {
                WriteUtf8Entry(zip, "measurementorfact.txt", mofSb.ToString(), compression);
            }
            if (extraEntries is not null)
            {
                foreach (var (entryName, rawBytes) in extraEntries)
                {
                    WriteRawEntry(zip, entryName, rawBytes, compression);
                }
            }
        }
        return ms.ToArray();
    }

    public static byte[] BuildRawZip(params (string EntryName, byte[] Content)[] entries)
    {
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var (name, content) in entries)
            {
                WriteRawEntry(zip, name, content, CompressionLevel.NoCompression);
            }
        }
        return ms.ToArray();
    }

    /// <summary>
    /// Patches both the Local File Header and Central Directory uncompressed-size fields for a named ZIP entry
    /// so tests can construct genuine lying-header archives (F-03).
    /// </summary>
    public static byte[] PatchZipDeclaredUncompressedSize(byte[] originalZip, string entryName, uint fakeUncompressedSize)
    {
        byte[] copy = (byte[])originalZip.Clone();
        int eocdOffset = -1;
        for (int i = copy.Length - 22; i >= Math.Max(0, copy.Length - 22 - 65535); i--)
        {
            if (copy[i] == 0x50 && copy[i + 1] == 0x4B && copy[i + 2] == 0x05 && copy[i + 3] == 0x06)
            {
                eocdOffset = i;
                break;
            }
        }
        if (eocdOffset < 0) throw new InvalidOperationException("EOCD not found.");

        ushort count = BinaryPrimitives.ReadUInt16LittleEndian(copy.AsSpan(eocdOffset + 10, 2));
        int pos = (int)BinaryPrimitives.ReadUInt32LittleEndian(copy.AsSpan(eocdOffset + 16, 4));
        bool patched = false;

        for (int i = 0; i < count; i++)
        {
            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(copy.AsSpan(pos + 28, 2));
            ushort extraLen = BinaryPrimitives.ReadUInt16LittleEndian(copy.AsSpan(pos + 30, 2));
            ushort commentLen = BinaryPrimitives.ReadUInt16LittleEndian(copy.AsSpan(pos + 32, 2));
            int localOffset = (int)BinaryPrimitives.ReadUInt32LittleEndian(copy.AsSpan(pos + 42, 4));
            string name = Encoding.UTF8.GetString(copy, pos + 46, nameLen);

            if (name.Equals(entryName, StringComparison.Ordinal))
            {
                BinaryPrimitives.WriteUInt32LittleEndian(copy.AsSpan(pos + 24, 4), fakeUncompressedSize);
                BinaryPrimitives.WriteUInt32LittleEndian(copy.AsSpan(localOffset + 22, 4), fakeUncompressedSize);
                patched = true;
            }

            pos += 46 + nameLen + extraLen + commentLen;
        }

        if (!patched) throw new InvalidOperationException($"Entry '{entryName}' not found in ZIP.");
        return copy;
    }

    public static string BuildDefaultMetaXml(
        string delimAttr,
        string quoteAttr,
        bool includeExtension,
        bool includeCoreProvenanceColumn = false,
        string ignoreHeaderLines = "1")
    {
        string provField = includeCoreProvenanceColumn
            ? "    <field index=\"7\" term=\"http://ccf.local/terms/provenance\" />\n"
            : string.Empty;

        string extBlock = includeExtension
            ? $@"  <extension encoding=""UTF-8"" fieldsTerminatedBy=""{delimAttr}"" linesTerminatedBy=""\n"" fieldsEnclosedBy=""{quoteAttr}"" ignoreHeaderLines=""{ignoreHeaderLines}"" rowType=""http://rs.tdwg.org/dwc/terms/MeasurementOrFact"">
    <files><location>measurementorfact.txt</location></files>
    <coreid index=""0"" />
    <field index=""1"" term=""http://rs.tdwg.org/dwc/terms/measurementType"" />
    <field index=""2"" term=""http://rs.tdwg.org/dwc/terms/measurementRemarks"" />
    <field index=""3"" term=""http://rs.tdwg.org/dwc/terms/measurementValue"" />
    <field index=""4"" term=""http://rs.tdwg.org/dwc/terms/measurementUnit"" />
    <field index=""5"" term=""http://rs.tdwg.org/dwc/terms/measurementMethod"" />
  </extension>
"
            : string.Empty;

        return $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<archive xmlns=""http://rs.tdwg.org/dwc/text/"">
  <core encoding=""UTF-8"" fieldsTerminatedBy=""{delimAttr}"" linesTerminatedBy=""\n"" fieldsEnclosedBy=""{quoteAttr}"" ignoreHeaderLines=""{ignoreHeaderLines}"" rowType=""http://rs.tdwg.org/dwc/terms/Occurrence"">
    <files><location>occurrence.txt</location></files>
    <id index=""0"" />
    <field index=""0"" term=""http://rs.tdwg.org/dwc/terms/occurrenceID"" />
    <field index=""1"" term=""http://rs.tdwg.org/dwc/terms/scientificName"" />
    <field index=""2"" term=""http://rs.tdwg.org/dwc/terms/verbatimCoordinates"" />
    <field index=""3"" term=""http://rs.tdwg.org/dwc/terms/countryCode"" />
    <field index=""4"" term=""http://rs.tdwg.org/dwc/terms/habitat"" />
    <field index=""5"" term=""http://rs.tdwg.org/dwc/terms/datasetID"" />
    <field index=""6"" term=""http://rs.tdwg.org/dwc/terms/datasetName"" />
{provField}  </core>
{extBlock}</archive>";
    }

    private static string JoinCols(string delimiter, string? quote, params string[] cols)
    {
        if (string.IsNullOrEmpty(quote))
        {
            return string.Join(delimiter, cols);
        }
        return string.Join(delimiter, cols.Select(c => quote + c.Replace(quote, quote + quote, StringComparison.Ordinal) + quote));
    }

    private static void WriteUtf8Entry(ZipArchive zip, string name, string content, CompressionLevel compression) =>
        WriteRawEntry(zip, name, Encoding.UTF8.GetBytes(content), compression);

    private static void WriteRawEntry(ZipArchive zip, string name, byte[] bytes, CompressionLevel compression)
    {
        ZipArchiveEntry entry = zip.CreateEntry(name, compression);
        // Fixed deterministic timestamp so even raw ZIP bytes are reproducible.
        entry.LastWriteTime = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        using Stream s = entry.Open();
        s.Write(bytes, 0, bytes.Length);
    }
}
