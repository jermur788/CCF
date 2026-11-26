using System.Buffers.Binary;
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
/// Enforces per-entry and cumulative uncompressed byte caps (declared and streamed), verifies ZIP entry
/// byte counts and IEEE CRC-32, refuses orphaned or blank-key extension rows, and prohibits DTD/DOCTYPE
/// and external XML entity resolution.
/// Note on delimiter support: only single-character <c>fieldsTerminatedBy</c> and <c>fieldsEnclosedBy</c>
/// (after unescaping <c>\t</c>, <c>\r</c>, <c>\n</c>) are supported; CR, LF, and CRLF are always treated as
/// row endings regardless of <c>linesTerminatedBy</c>.
/// </summary>
public static class DarwinCoreArchiveReader
{
    private static readonly uint[] Crc32Table = BuildCrc32Table();

    private readonly record struct ZipEntryRawLocator(
        ushort CompressionMethod,
        int DataOffset,
        int CompressedLength);

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

            Dictionary<string, ZipEntryRawLocator> rawLocators = IndexRawZipEntries(rawArray);
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

            if (!TryReadEntryBounded(metaEntry, rawArray, rawLocators, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] metaBytes, issues))
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

            if (!TryReadEntryBounded(coreEntry, rawArray, rawLocators, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] coreBytes, issues))
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
            // Every extension row MUST match an existing core row; blank keys and orphan keys are refused deterministically.
            int extRowsRead = 0;
            int blankExtKeyCount = 0;
            var orphanExtCountsByKey = new SortedDictionary<string, int>(StringComparer.Ordinal);

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

                if (!TryReadEntryBounded(extEntry, rawArray, rawLocators, options.MaxUncompressedBytes, ref actualBytesRead, out byte[] extBytes, issues))
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
                            if (string.IsNullOrWhiteSpace(key))
                            {
                                blankExtKeyCount++;
                                return true;
                            }
                            if (!coreRowsById.ContainsKey(key))
                            {
                                orphanExtCountsByKey.TryGetValue(key, out int prev);
                                orphanExtCountsByKey[key] = prev + 1;
                                return true;
                            }
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

            if (blankExtKeyCount > 0)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveBlankExtensionCoreId",
                    IssueSeverity.Refusal,
                    $"{blankExtKeyCount} extension row(s) have a blank coreid and cannot be joined to any core tree record."));
            }

            if (orphanExtCountsByKey.Count > 0)
            {
                foreach (var kv in orphanExtCountsByKey)
                {
                    issues.Add(new ImportValidationIssue(
                        "ArchiveOrphanExtensionRow",
                        IssueSeverity.Refusal,
                        $"{kv.Value} extension row(s) reference coreid '{kv.Key}' which does not exist in core table '{coreDesc.Location}'.",
                        SourceTreeId: kv.Key));
                }
            }

            if (blankExtKeyCount > 0 || orphanExtCountsByKey.Count > 0)
            {
                return Failed(issues, archiveSha256, coreRowsRead, extRowsRead, actualBytesRead);
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
        byte[] rawArchiveBytes,
        Dictionary<string, ZipEntryRawLocator> rawLocators,
        long maxUncompressedBytes,
        ref long cumulativeBytesRead,
        out byte[] data,
        List<ImportValidationIssue> issues)
    {
        data = Array.Empty<byte>();
        try
        {
            using Stream s = OpenEntryForVerifiedRead(entry, rawArchiveBytes, rawLocators);
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

            if (entryBytes != entry.Length)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveEntrySizeMismatch",
                    IssueSeverity.Refusal,
                    $"Archive entry '{entry.FullName}' decompressed to {entryBytes} bytes, which disagrees with its declared ZIP header size ({entry.Length} bytes)."));
                return false;
            }

            byte[] result = ms.ToArray();
            uint actualCrc = ComputeCrc32(result);
            if (actualCrc != entry.Crc32)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveEntryCrcMismatch",
                    IssueSeverity.Refusal,
                    $"Archive entry '{entry.FullName}' failed CRC-32 verification (expected 0x{entry.Crc32:x8}, computed 0x{actualCrc:x8})."));
                return false;
            }

            data = result;
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

    /// <summary>
    /// Opens the raw compressed stream directly for Deflate (method 8) and Store (method 0) entries so that
    /// a lying ZIP uncompressed-size header cannot silently truncate decompression before the streamed cap
    /// or size-mismatch check runs.
    /// </summary>
    private static Stream OpenEntryForVerifiedRead(
        ZipArchiveEntry entry,
        byte[] rawArchiveBytes,
        Dictionary<string, ZipEntryRawLocator> rawLocators)
    {
        if (rawLocators.TryGetValue(entry.FullName, out ZipEntryRawLocator loc) &&
            loc.DataOffset >= 0 &&
            loc.CompressedLength >= 0 &&
            loc.DataOffset + loc.CompressedLength <= rawArchiveBytes.Length)
        {
            if (loc.CompressionMethod == 8)
            {
                var compressedSlice = new MemoryStream(rawArchiveBytes, loc.DataOffset, loc.CompressedLength, writable: false);
                return new DeflateStream(compressedSlice, CompressionMode.Decompress, leaveOpen: false);
            }
            if (loc.CompressionMethod == 0)
            {
                return new MemoryStream(rawArchiveBytes, loc.DataOffset, loc.CompressedLength, writable: false);
            }
        }

        return entry.Open();
    }

    private static Dictionary<string, ZipEntryRawLocator> IndexRawZipEntries(byte[] raw)
    {
        var map = new Dictionary<string, ZipEntryRawLocator>(StringComparer.Ordinal);
        if (raw.Length < 22) return map;

        // Locate End of Central Directory record (PK\x05\x06) scanning backward up to 65,557 bytes.
        int minStart = Math.Max(0, raw.Length - 22 - 65535);
        int eocdOffset = -1;
        for (int i = raw.Length - 22; i >= minStart; i--)
        {
            if (raw[i] == 0x50 && raw[i + 1] == 0x4B && raw[i + 2] == 0x05 && raw[i + 3] == 0x06)
            {
                eocdOffset = i;
                break;
            }
        }
        if (eocdOffset < 0) return map;

        ushort entryCount = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(eocdOffset + 10, 2));
        uint cdOffsetU = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(eocdOffset + 16, 4));
        if (cdOffsetU == 0xFFFFFFFFu || entryCount == 0xFFFF || cdOffsetU > (uint)raw.Length)
        {
            return map; // ZIP64 falls back to entry.Open() + size/CRC check.
        }

        int pos = (int)cdOffsetU;
        for (int e = 0; e < entryCount; e++)
        {
            if (pos + 46 > raw.Length) break;
            if (BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(pos, 4)) != 0x02014B50u) break;

            ushort method = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(pos + 10, 2));
            uint compSizeU = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(pos + 20, 4));
            ushort nameLen = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(pos + 28, 2));
            ushort extraLen = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(pos + 30, 2));
            ushort commentLen = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(pos + 32, 2));
            uint localHeaderOffsetU = BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(pos + 42, 4));

            if (pos + 46 + nameLen > raw.Length) break;
            string name = Encoding.UTF8.GetString(raw, pos + 46, nameLen);

            if (compSizeU != 0xFFFFFFFFu && localHeaderOffsetU != 0xFFFFFFFFu &&
                localHeaderOffsetU <= (uint)(raw.Length - 30))
            {
                int lh = (int)localHeaderOffsetU;
                if (BinaryPrimitives.ReadUInt32LittleEndian(raw.AsSpan(lh, 4)) == 0x04034B50u)
                {
                    ushort lhNameLen = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(lh + 26, 2));
                    ushort lhExtraLen = BinaryPrimitives.ReadUInt16LittleEndian(raw.AsSpan(lh + 28, 2));
                    int dataOffset = lh + 30 + lhNameLen + lhExtraLen;
                    if (compSizeU <= int.MaxValue && dataOffset >= 0 && dataOffset + (int)compSizeU <= raw.Length)
                    {
                        map[name] = new ZipEntryRawLocator(method, dataOffset, (int)compSizeU);
                    }
                }
            }

            pos += 46 + nameLen + extraLen + commentLen;
        }

        return map;
    }

    private static uint[] BuildCrc32Table()
    {
        var table = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint c = i;
            for (int bit = 0; bit < 8; bit++)
            {
                c = (c & 1u) != 0u ? (0xEDB88320u ^ (c >> 1)) : (c >> 1);
            }
            table[i] = c;
        }
        return table;
    }

    private static uint ComputeCrc32(ReadOnlySpan<byte> bytes)
    {
        uint crc = 0xFFFFFFFFu;
        for (int i = 0; i < bytes.Length; i++)
        {
            crc = Crc32Table[(crc ^ bytes[i]) & 0xFFu] ^ (crc >> 8);
        }
        return crc ^ 0xFFFFFFFFu;
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

        // DtdProcessing.Prohibit + XmlResolver = null is the primary XXE / DOCTYPE defence.
        // MaxCharactersFromEntities is set to a small positive bound (in .NET, 0 means unlimited).
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreWhitespace = true,
            MaxCharactersFromEntities = 1024,
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
            bool hasDoctypeMarker = ContainsDoctypeToken(metaBytes);
            bool isDtd = hasDoctypeMarker ||
                         ex.Message.Contains("DTD", StringComparison.OrdinalIgnoreCase) ||
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

    private static bool ContainsDoctypeToken(byte[] bytes)
    {
        string sample = Encoding.UTF8.GetString(bytes);
        return sample.Contains("<!DOCTYPE", StringComparison.OrdinalIgnoreCase) ||
               sample.Contains("<!ENTITY", StringComparison.OrdinalIgnoreCase);
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
        if (!string.IsNullOrWhiteSpace(ignoreRaw))
        {
            if (!int.TryParse(ignoreRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out ignoreHeaderLines) ||
                ignoreHeaderLines < 0)
            {
                issues.Add(new ImportValidationIssue(
                    "ArchiveMalformedMetaXml",
                    IssueSeverity.Refusal,
                    $"Invalid ignoreHeaderLines attribute '{ignoreRaw}' in meta.xml; must be a non-negative integer."));
                return null;
            }
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
            IgnoreHeaderLines = ignoreHeaderLines,
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

        // Single-character delimiter/quote supported; CR, LF, and CRLF are treated as row endings.
        char delimiter = desc.FieldDelimiter[0];
        char? quote = desc.QuoteChar;

        int pos = 0;
        int lineNumber = 0;
        int dataRows = 0;

        while (pos < text.Length)
        {
            List<string> cols = ReadNextRow(text, ref pos, delimiter, quote);
            lineNumber++;

            bool isBlankLine = cols.Count == 1 && string.IsNullOrWhiteSpace(cols[0]);

            // Leading blank lines inside the declared header region would shift the real header into data rows; refuse explicitly.
            if (lineNumber <= desc.IgnoreHeaderLines)
            {
                if (isBlankLine)
                {
                    issues.Add(new ImportValidationIssue(
                        "ArchiveMalformedHeader",
                        IssueSeverity.Refusal,
                        $"Table '{desc.Location}' has a blank line at header line {lineNumber} (ignoreHeaderLines={desc.IgnoreHeaderLines}); refusing to shift header into data."));
                    return false;
                }
                continue;
            }

            // Skip completely blank trailing/separator lines after headers are consumed.
            if (isBlankLine)
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
