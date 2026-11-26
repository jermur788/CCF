using System.Globalization;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace SitkaP1.Core.WoodlandImport;

/// <summary>Descriptor parsed from a Darwin Core Archive <c>meta.xml</c> file.</summary>
public sealed record DwcTableDescriptor
{
    public required bool IsCore { get; init; }
    public required string RowType { get; init; }
    public required string Location { get; init; }
    public required string EncodingName { get; init; }
    public required string FieldDelimiter { get; init; }
    public required string LineDelimiter { get; init; }
    public required char? QuoteChar { get; init; }
    public required int IgnoreHeaderLines { get; init; }
    public required int KeyColumnIndex { get; init; }
    public required IReadOnlyList<DwcFieldMapping> Fields { get; init; }
}

/// <summary>Single column mapping inside a DwC-A <c>meta.xml</c> core or extension block.</summary>
public sealed record DwcFieldMapping(
    int? Index,
    string Term,
    string? DefaultValue);

/// <summary>Parsed core row joined with any extension rows matching its declared core key.</summary>
public sealed record DwcJoinedRecord(
    string CoreId,
    IReadOnlyDictionary<string, string> CoreTerms,
    IReadOnlyList<DwcExtensionRow> ExtensionRows);

/// <summary>Single parsed row from a DwC-A extension table.</summary>
public sealed record DwcExtensionRow(
    string RowType,
    string Location,
    string CoreId,
    IReadOnlyDictionary<string, string> Terms);

/// <summary>Result of reading and joining a bounded Darwin Core Archive.</summary>
public sealed record DwcArchiveReadResult
{
    public required bool Success { get; init; }
    public DwcTableDescriptor? CoreDescriptor { get; init; }
    public IReadOnlyList<DwcTableDescriptor> ExtensionDescriptors { get; init; } = Array.Empty<DwcTableDescriptor>();
    public IReadOnlyList<DwcJoinedRecord> Records { get; init; } = Array.Empty<DwcJoinedRecord>();
    public int CoreRowsRead { get; init; }
    public int ExtensionRowsRead { get; init; }
    public long TotalUncompressedBytesRead { get; init; }
    public string? ArchiveSha256 { get; init; }
    public required IReadOnlyList<ImportValidationIssue> Issues { get; init; }
}

/// <summary>
/// Bounded, deterministic, XXE-hardened Darwin Core Archive (DwC-A) reader built purely on BCL types.
/// Enforces per-entry and cumulative uncompressed byte caps, core record caps, and extension row caps
/// before allocating unbounded memory. Prohibits DTD/DOCTYPE and external XML entity resolution.
/// </summary>
public static class DarwinCoreArchiveReader
{
    public static DwcArchiveReadResult Read(ReadOnlyMemory<byte> archiveBytes, WoodlandImportOptions options)
    {
        var issues = new List<ImportValidationIssue>();

        if (archiveBytes.IsEmpty)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveEmpty",
                IssueSeverity.Refusal,
                "Archive payload is empty."));
            return Failed(issues, null, 0, 0, 0);
        }

        string archiveSha256 = WoodlandImportSerializer.ComputeSha256Hex(archiveBytes.Span);

        if (options.MaxUncompressedBytes <= 0 || options.MaxTotalRecords <= 0 || options.MaxMeasurementRows < 0)
        {
            issues.Add(new ImportValidationIssue(
                "InvalidImportOptions",
                IssueSeverity.Refusal,
                "Import options specify non-positive archive or record caps."));
            return Failed(issues, archiveSha256, 0, 0, 0);
        }

        byte[] rawArray = archiveBytes.ToArray();
        ZipArchive archive;
        try
        {
            var ms = new MemoryStream(rawArray, writable: false);
            archive = new ZipArchive(ms, ZipArchiveMode.Read, leaveOpen: false);
        }
        catch (InvalidDataException ex)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMalformedZip",
                IssueSeverity.Refusal,
                "Archive is not a valid ZIP container: " + ex.Message));
            return Failed(issues, archiveSha256, 0, 0, 0);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMalformedZip",
                IssueSeverity.Refusal,
                "Archive could not be opened as a ZIP container: " + ex.Message));
            return Failed(issues, archiveSha256, 0, 0, 0);
        }

        using (archive)
        {
            // Account declared uncompressed sizes across ALL entries first (fail fast before any decompression).
            long declaredTotal = 0;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                long entryLen = entry.Length;
                if (entryLen < 0 || entryLen > options.MaxUncompressedBytes)
                {
                    issues.Add(new ImportValidationIssue(
                        "CapUncompressedBytesExceeded",
                        IssueSeverity.Refusal,
                        $"Archive entry '{entry.FullName}' declared uncompressed size ({entryLen} bytes) exceeds cap ({options.MaxUncompressedBytes} bytes)."));
                    return Failed(issues, archiveSha256, 0, 0, entryLen);
                }
                declaredTotal += entryLen;
                if (declaredTotal > options.MaxUncompressedBytes)
                {
                    issues.Add(new ImportValidationIssue(
                        "CapUncompressedBytesExceeded",
                        IssueSeverity.Refusal,
                        $"Archive total declared uncompressed size ({declaredTotal} bytes) exceeds cap ({options.MaxUncompressedBytes} bytes)."));
                    return Failed(issues, archiveSha256, 0, 0, declaredTotal);
                }
            }

            long actualBytesRead = 0;

            ZipArchiveEntry? metaEntry = FindEntry(archive, "meta.xml");
            if (metaEntry is null)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveMissingMetaXml",
                    IssueSeverity.Refusal,
                    "Darwin Core Archive does not contain a meta.xml descriptor."));
                return Failed(issues, archiveSha256, 0, 0, actualBytesRead);
            }

            if (!TryReadEntryBounded(metaEntry, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] metaBytes, issues))
            {
                return Failed(issues, archiveSha256, 0, 0, actualBytesRead);
            }

            if (!TryParseMetaXml(metaBytes, options.MaxUncompressedBytes, out DwcTableDescriptor? coreDesc, out List<DwcTableDescriptor> extDescs, issues))
            {
                return Failed(issues, archiveSha256, 0, 0, actualBytesRead);
            }

            // Read core table.
            ZipArchiveEntry? coreEntry = FindEntry(archive, coreDesc!.Location);
            if (coreEntry is null)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveMissingCoreFile",
                    IssueSeverity.Refusal,
                    $"Core file '{coreDesc.Location}' declared in meta.xml was not found in the archive."));
                return Failed(issues, archiveSha256, 0, 0, actualBytesRead);
            }

            if (!TryReadEntryBounded(coreEntry, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] coreBytes, issues))
            {
                return Failed(issues, archiveSha256, 0, 0, actualBytesRead);
            }

            var coreOrder = new List<string>();
            var coreRowsById = new Dictionary<string, Dictionary<string, string>>(StringComparer.Ordinal);
            var extRowsById = new Dictionary<string, List<DwcExtensionRow>>(StringComparer.Ordinal);

            int coreRowsRead = 0;
            if (!TryParseDelimitedTable(
                    coreBytes,
                    coreDesc,
                    maxRows: options.MaxTotalRecords,
                    capIssueCode: "CapTotalRecordsExceeded",
                    capLabel: "total records",
                    onRow: (key, terms) =>
                    {
                        coreRowsRead++;
                        if (string.IsNullOrWhiteSpace(key))
                        {
                            issues.Add(new ImportValidationIssue(
                                "ArchiveBlankCoreId",
                                IssueSeverity.Refusal,
                                $"Core row {coreRowsRead} in '{coreDesc.Location}' has a blank core id."));
                            return false;
                        }
                        if (coreRowsById.ContainsKey(key))
                        {
                            issues.Add(new ImportValidationIssue(
                                "DuplicateSourceTreeId",
                                IssueSeverity.Refusal,
                                $"Duplicate core id '{key}' encountered in '{coreDesc.Location}'.",
                                SourceTreeId: key));
                            return false;
                        }
                        coreOrder.Add(key);
                        coreRowsById[key] = terms;
                        return true;
                    },
                    issues: issues))
            {
                return Failed(issues, archiveSha256, coreRowsRead, 0, actualBytesRead);
            }

            // Read extension tables and join on declared coreid.
            int extRowsRead = 0;
            foreach (DwcTableDescriptor extDesc in extDescs)
            {
                ZipArchiveEntry? extEntry = FindEntry(archive, extDesc.Location);
                if (extEntry is null)
                {
                    issues.Add(new ImportValidationIssue(
                        "ArchiveMissingExtensionFile",
                        IssueSeverity.Refusal,
                        $"Extension file '{extDesc.Location}' declared in meta.xml was not found in the archive."));
                    return Failed(issues, archiveSha256, coreRowsRead, extRowsRead, actualBytesRead);
                }

                if (!TryReadEntryBounded(extEntry, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] extBytes, issues))
                {
                    return Failed(issues, archiveSha256, coreRowsRead, extRowsRead, actualBytesRead);
                }

                int remainingMeasurementCap = options.MaxMeasurementRows - extRowsRead;
                if (!TryParseDelimitedTable(
                        extBytes,
                        extDesc,
                        maxRows: remainingMeasurementCap,
                        capIssueCode: "CapMeasurementRowsExceeded",
                        capLabel: "measurement rows",
                        onRow: (key, terms) =>
                        {
                            extRowsRead++;
                            if (!extRowsById.TryGetValue(key, out var list))
                            {
                                list = new List<DwcExtensionRow>();
                                extRowsById[key] = list;
                            }
                            list.Add(new DwcExtensionRow(extDesc.RowType, extDesc.Location, key, terms));
                            return true;
                        },
                        issues: issues))
                {
                    return Failed(issues, archiveSha256, coreRowsRead, extRowsRead, actualBytesRead);
                }
            }

            // Assemble joined records in canonical ordinal sort order on CoreId.
            coreOrder.Sort(StringComparer.Ordinal);
            var joined = new List<DwcJoinedRecord>(coreOrder.Count);
            foreach (string id in coreOrder)
            {
                IReadOnlyList<DwcExtensionRow> exts = extRowsById.TryGetValue(id, out var list)
                    ? list.AsReadOnly()
                    : Array.Empty<DwcExtensionRow>();
                joined.Add(new DwcJoinedRecord(id, coreRowsById[id], exts));
            }

            return new DwcArchiveReadResult
            {
                Success = true,
                CoreDescriptor = coreDesc,
                ExtensionDescriptors = extDescs.AsReadOnly(),
                Records = joined.AsReadOnly(),
                CoreRowsRead = coreRowsRead,
                ExtensionRowsRead = extRowsRead,
                TotalUncompressedBytesRead = actualBytesRead,
                ArchiveSha256 = archiveSha256,
                Issues = issues.AsReadOnly(),
            };
        }
    }

    private static DwcArchiveReadResult Failed(
        List<ImportValidationIssue> issues,
        string? sha256,
        int coreRows,
        int extRows,
        long bytesRead)
    {
        issues.Sort(WoodlandImportValidator.CompareIssues);
        return new DwcArchiveReadResult
        {
            Success = false,
            CoreRowsRead = coreRows,
            ExtensionRowsRead = extRows,
            TotalUncompressedBytesRead = bytesRead,
            ArchiveSha256 = sha256,
            Issues = issues.AsReadOnly(),
        };
    }

    private static ZipArchiveEntry? FindEntry(ZipArchive archive, string relativePath)
    {
        string normalizedTarget = relativePath.Replace('\\', '/').TrimStart('/');
        foreach (ZipArchiveEntry e in archive.Entries)
        {
            string entryPath = e.FullName.Replace('\\', '/').TrimStart('/');
            if (entryPath.Equals(normalizedTarget, StringComparison.Ordinal))
            {
                return e;
            }
        }
        // Fallback: check case-insensitive match if exact path absent (some IPT exports vary casing).
        foreach (ZipArchiveEntry e in archive.Entries)
        {
            string entryPath = e.FullName.Replace('\\', '/').TrimStart('/');
            if (entryPath.Equals(normalizedTarget, StringComparison.OrdinalIgnoreCase))
            {
                return e;
            }
        }
        return null;
    }

    private static bool TryReadEntryBounded(
        ZipArchiveEntry entry,
        long maxUncompressedBytes,
        ref long cumulativeBytesRead,
        out byte[] data,
        List<ImportValidationIssue> issues)
    {
        data = Array.Empty<byte>();
        try
        {
            using Stream s = entry.Open();
            using var ms = new MemoryStream();
            byte[] buffer = new byte[16384];
            long entryBytes = 0;
            int read;
            while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
            {
                entryBytes += read;
                cumulativeBytesRead += read;
                if (entryBytes > maxUncompressedBytes || cumulativeBytesRead > maxUncompressedBytes)
                {
                    issues.Add(new ImportValidationIssue(
                        "CapUncompressedBytesExceeded",
                        IssueSeverity.Refusal,
                        $"Decompressed byte count while reading '{entry.FullName}' exceeded cap ({maxUncompressedBytes} bytes)."));
                    return false;
                }
                ms.Write(buffer, 0, read);
            }
            data = ms.ToArray();
            return true;
        }
        catch (InvalidDataException ex)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveCorruptEntry",
                IssueSeverity.Refusal,
                $"Archive entry '{entry.FullName}' failed decompression: " + ex.Message));
            return false;
        }
    }

    private static bool TryParseMetaXml(
        byte[] metaBytes,
        long maxXmlChars,
        out DwcTableDescriptor? core,
        out List<DwcTableDescriptor> extensions,
        List<ImportValidationIssue> issues)
    {
        core = null;
        extensions = new List<DwcTableDescriptor>();

        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            MaxCharactersFromEntities = 0,
            MaxCharactersInDocument = maxXmlChars,
        };

        try
        {
            using var ms = new MemoryStream(metaBytes, writable: false);
            using var reader = XmlReader.Create(ms, settings);

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.LocalName.Equals("core", StringComparison.OrdinalIgnoreCase))
                {
                    if (core is not null)
                    {
                        issues.Add(new ImportValidationIssue(
                            "ArchiveMultipleCores",
                            IssueSeverity.Refusal,
                            "meta.xml declares more than one <core> element."));
                        return false;
                    }
                    core = ParseTableElement(reader, isCore: true, issues);
                    if (core is null) return false;
                }
                else if (reader.LocalName.Equals("extension", StringComparison.OrdinalIgnoreCase))
                {
                    var ext = ParseTableElement(reader, isCore: false, issues);
                    if (ext is null) return false;
                    extensions.Add(ext);
                }
            }
        }
        catch (XmlException ex)
        {
            bool isDtd = ex.Message.Contains("DTD", StringComparison.OrdinalIgnoreCase) ||
                         ex.Message.Contains("DOCTYPE", StringComparison.OrdinalIgnoreCase);
            issues.Add(new ImportValidationIssue(
                isDtd ? "ArchiveXmlDtdProhibited" : "ArchiveMalformedMetaXml",
                IssueSeverity.Refusal,
                (isDtd ? "DTD/DOCTYPE declarations are prohibited in meta.xml: " : "Malformed meta.xml: ") + ex.Message));
            return false;
        }

        if (core is null)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMissingCoreDescriptor",
                IssueSeverity.Refusal,
                "meta.xml does not contain a <core> element."));
            return false;
        }

        return true;
    }

    private static DwcTableDescriptor? ParseTableElement(
        XmlReader reader,
        bool isCore,
        List<ImportValidationIssue> issues)
    {
        string rowType = reader.GetAttribute("rowType") ?? string.Empty;
        string encoding = reader.GetAttribute("encoding") ?? "UTF-8";
        string fieldDelim = UnescapeXmlDelimiter(reader.GetAttribute("fieldsTerminatedBy") ?? "\t");
        string lineDelim = UnescapeXmlDelimiter(reader.GetAttribute("linesTerminatedBy") ?? "\n");
        string? enclosedByRaw = reader.GetAttribute("fieldsEnclosedBy");
        char? quoteChar = string.IsNullOrEmpty(enclosedByRaw) ? null : enclosedByRaw[0];
        int ignoreHeaderLines = 0;
        string? ignoreRaw = reader.GetAttribute("ignoreHeaderLines");
        if (!string.IsNullOrWhiteSpace(ignoreRaw) &&
            !int.TryParse(ignoreRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out ignoreHeaderLines))
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMalformedMetaXml",
                IssueSeverity.Refusal,
                $"Invalid ignoreHeaderLines attribute '{ignoreRaw}' in meta.xml."));
            return null;
        }

        string? location = null;
        int? keyIndex = null;
        var fields = new List<DwcFieldMapping>();

        if (!reader.IsEmptyElement)
        {
            int elementDepth = reader.Depth;
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Depth == elementDepth)
                {
                    break;
                }

                if (reader.NodeType != XmlNodeType.Element)
                {
                    continue;
                }

                if (reader.LocalName.Equals("location", StringComparison.OrdinalIgnoreCase))
                {
                    location = reader.ReadElementContentAsString().Trim();
                }
                else if (isCore && reader.LocalName.Equals("id", StringComparison.OrdinalIgnoreCase))
                {
                    string? idxStr = reader.GetAttribute("index");
                    if (int.TryParse(idxStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int idx) && idx >= 0)
                    {
                        keyIndex = idx;
                    }
                }
                else if (!isCore && reader.LocalName.Equals("coreid", StringComparison.OrdinalIgnoreCase))
                {
                    string? idxStr = reader.GetAttribute("index");
                    if (int.TryParse(idxStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int idx) && idx >= 0)
                    {
                        keyIndex = idx;
                    }
                }
                else if (reader.LocalName.Equals("field", StringComparison.OrdinalIgnoreCase))
                {
                    string? term = reader.GetAttribute("term");
                    string? idxStr = reader.GetAttribute("index");
                    string? defaultVal = reader.GetAttribute("default");

                    if (string.IsNullOrWhiteSpace(term))
                    {
                        continue;
                    }

                    int? index = null;
                    if (!string.IsNullOrWhiteSpace(idxStr))
                    {
                        if (!int.TryParse(idxStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsedIdx) || parsedIdx < 0)
                        {
                            issues.Add(new ImportValidationIssue(
                                "ArchiveMalformedMetaXml",
                                IssueSeverity.Refusal,
                                $"Invalid field index '{idxStr}' for term '{term}' in meta.xml."));
                            return null;
                        }
                        index = parsedIdx;
                    }

                    fields.Add(new DwcFieldMapping(index, term.Trim(), defaultVal));
                }
            }
        }

        if (string.IsNullOrWhiteSpace(location))
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMalformedMetaXml",
                IssueSeverity.Refusal,
                $"meta.xml {(isCore ? "core" : "extension")} element is missing a <location> entry."));
            return null;
        }

        if (keyIndex is null)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveMalformedMetaXml",
                IssueSeverity.Refusal,
                $"meta.xml {(isCore ? "core" : "extension")} element '{location}' is missing a valid {(isCore ? "<id>" : "<coreid>")} index."));
            return null;
        }

        if (string.IsNullOrEmpty(fieldDelim))
        {
            fieldDelim = "\t";
        }

        return new DwcTableDescriptor
        {
            IsCore = isCore,
            RowType = rowType,
            Location = location,
            EncodingName = encoding,
            FieldDelimiter = fieldDelim,
            LineDelimiter = lineDelim,
            QuoteChar = quoteChar,
            IgnoreHeaderLines = Math.Max(0, ignoreHeaderLines),
            KeyColumnIndex = keyIndex.Value,
            Fields = fields.AsReadOnly(),
        };
    }

    private static string UnescapeXmlDelimiter(string value)
    {
        return value
            .Replace("\\t", "\t", StringComparison.Ordinal)
            .Replace("\\r", "\r", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal);
    }

    private static bool TryParseDelimitedTable(
        byte[] bytes,
        DwcTableDescriptor desc,
        int maxRows,
        string capIssueCode,
        string capLabel,
        Func<string, Dictionary<string, string>, bool> onRow,
        List<ImportValidationIssue> issues)
    {
        Encoding encoding;
        try
        {
            encoding = ResolveEncoding(desc.EncodingName);
        }
        catch (ArgumentException)
        {
            issues.Add(new ImportValidationIssue(
                "ArchiveUnsupportedEncoding",
                IssueSeverity.Refusal,
                $"Unsupported character encoding '{desc.EncodingName}' declared for '{desc.Location}'."));
            return false;
        }

        string text = encoding.GetString(bytes);
        if (text.Length > 0 && text[0] == '\uFEFF')
        {
            text = text[1..];
        }

        char delimiter = desc.FieldDelimiter[0];
        char? quote = desc.QuoteChar;

        int pos = 0;
        int lineNumber = 0;
        int dataRows = 0;

        while (pos < text.Length)
        {
            List<string> cols = ReadNextRow(text, ref pos, delimiter, quote);
            lineNumber++;

            // Skip completely blank trailing lines.
            if (cols.Count == 1 && string.IsNullOrEmpty(cols[0]))
            {
                continue;
            }

            if (lineNumber <= desc.IgnoreHeaderLines)
            {
                continue;
            }

            dataRows++;
            if (dataRows > maxRows)
            {
                issues.Add(new ImportValidationIssue(
                    capIssueCode,
                    IssueSeverity.Refusal,
                    $"Archive table '{desc.Location}' exceeded {capLabel} cap ({maxRows})."));
                return false;
            }

            if (desc.KeyColumnIndex >= cols.Count)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveRowMissingKeyColumn",
                    IssueSeverity.Refusal,
                    $"Row {lineNumber} in '{desc.Location}' has only {cols.Count} columns; key index {desc.KeyColumnIndex} is missing."));
                return false;
            }

            string key = cols[desc.KeyColumnIndex].Trim();
            var terms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < desc.Fields.Count; i++)
            {
                DwcFieldMapping mapping = desc.Fields[i];
                string val = string.Empty;
                if (mapping.Index.HasValue && mapping.Index.Value < cols.Count)
                {
                    val = cols[mapping.Index.Value];
                }
                else if (mapping.DefaultValue is not null)
                {
                    val = mapping.DefaultValue;
                }

                string shortName = ShortTermName(mapping.Term);
                terms[mapping.Term] = val;
                if (!terms.ContainsKey(shortName))
                {
                    terms[shortName] = val;
                }
            }

            if (!onRow(key, terms))
            {
                return false;
            }
        }

        return true;
    }

    private static List<string> ReadNextRow(string text, ref int pos, char delimiter, char? quote)
    {
        var cols = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        while (pos < text.Length)
        {
            char c = text[pos++];

            if (inQuotes)
            {
                if (quote.HasValue && c == quote.Value)
                {
                    // Escaped doubled quote inside quoted field.
                    if (pos < text.Length && text[pos] == quote.Value)
                    {
                        sb.Append(quote.Value);
                        pos++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    sb.Append(c);
                }
                continue;
            }

            if (quote.HasValue && c == quote.Value && sb.Length == 0)
            {
                inQuotes = true;
                continue;
            }

            if (c == delimiter)
            {
                cols.Add(sb.ToString());
                sb.Clear();
                continue;
            }

            if (c == '\r')
            {
                if (pos < text.Length && text[pos] == '\n')
                {
                    pos++;
                }
                break;
            }

            if (c == '\n')
            {
                break;
            }

            sb.Append(c);
        }

        cols.Add(sb.ToString());
        return cols;
    }

    private static Encoding ResolveEncoding(string name)
    {
        string normalized = name.Trim().ToUpperInvariant();
        return normalized switch
        {
            "UTF-8" or "UTF8" => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false),
            "ISO-8859-1" or "LATIN1" or "LATIN-1" => Encoding.Latin1,
            "US-ASCII" or "ASCII" => Encoding.ASCII,
            "UTF-16" or "UTF-16LE" => Encoding.Unicode,
            "UTF-16BE" => Encoding.BigEndianUnicode,
            _ => Encoding.GetEncoding(name),
        };
    }

    internal static string ShortTermName(string termUri)
    {
        int slash = termUri.LastIndexOf('/');
        int hash = termUri.LastIndexOf('#');
        int cut = Math.Max(slash, hash);
        return cut >= 0 && cut + 1 < termUri.Length ? termUri[(cut + 1)..] : termUri;
    }
}
