using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
public static class ReferenceArchiveIntegrity
{
    private const string ArchivePath = "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes";
    private const string SchedulePath = "Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json";
    private static readonly Dictionary<int, string> PinnedWorlds = new Dictionary<int, string>
    {
        { 0, "A564039D9B7CE31D" }, { 20, "F7DF7DAB53B6FD32" }, { 50, "D5E75D6D21D631AC" }, { 100, "7AD177B3CC2F73C7" }
    };
    private const string PinnedSchedule = "56C8B99FA1E8DDD1";

    public static void Verify()
    {
        string text;
        using (var compressed = new MemoryStream(File.ReadAllBytes(ArchivePath)))
        using (var decoder = new GZipStream(compressed, CompressionMode.Decompress))
        using (var json = new MemoryStream())
        {
            decoder.CopyTo(json);
            text = Encoding.UTF8.GetString(json.ToArray());
        }
        VerifyText(text, true);
        Debug.Log($"REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule={PinnedSchedule} "
            + $"archiveFileFnv={ScenarioReferenceArchive.Hash(Convert.ToBase64String(File.ReadAllBytes(ArchivePath)))}");

        // Negative control: one changed value inside the Year-100 embedded world
        // (in memory only) must be rejected.
        int lastWorld = text.LastIndexOf("\"world\"", StringComparison.Ordinal);
        int target = text.IndexOf("\"diameterCm\"", lastWorld, StringComparison.Ordinal);
        Require(lastWorld >= 0 && target > lastWorld, "negative control could not locate a Year-100 tree value");
        int digit = target + "\"diameterCm\"".Length;
        while (digit < text.Length && !char.IsDigit(text[digit])) digit++;
        string tampered = text.Substring(0, digit) + (text[digit] == '9' ? '8' : (char)(text[digit] + 1)) + text.Substring(digit + 1);
        bool rejected = false;
        try { VerifyText(tampered, false); }
        catch (InvalidOperationException) { rejected = true; }
        Require(rejected, "a changed Year-100 value was not detected");
        Debug.Log("REFERENCE_ARCHIVE_NEGATIVE_CONTROL_PASS tamperedYear100Value=detected");
    }

    private static void VerifyText(string text, bool log)
    {
        // Stored metadata (strings and ints only; schema-insensitive reads).
        ScenarioReferenceArchive stored = JsonUtility.FromJson<ScenarioReferenceArchive>(text);
        Require(stored != null && stored.referenceId == "reference-future-v1" && stored.saveVersion == 12
            && stored.milestones != null && stored.milestones.Count == 4, "archive metadata is not Reference Future v1");

        List<string> worlds = EmbeddedObjects(text, "world");
        Require(worlds.Count == stored.milestones.Count, $"found {worlds.Count} embedded worlds for {stored.milestones.Count} milestones");
        for (int i = 0; i < worlds.Count; i++)
        {
            ScenarioReferenceMilestone milestone = stored.milestones[i];
            string hash = ScenarioReferenceArchive.Hash(worlds[i]);
            Require(PinnedWorlds.TryGetValue(milestone.year, out string pinned), "unexpected milestone year " + milestone.year);
            Require(hash == milestone.worldHash && hash == pinned && milestone.world != null && milestone.world.version == 12,
                $"Year-{milestone.year} embedded world hash {hash} (stored {milestone.worldHash}, pinned {pinned})");
            if (log) Debug.Log($"REFERENCE_ARCHIVE_WORLD year={milestone.year} embeddedHash={hash} stored={milestone.worldHash} pinned=True version=12");
        }
        Require(stored.startingStandHash == PinnedWorlds[0], "starting-stand hash is not the Year-0 anchor");

        List<string> schedules = EmbeddedObjects(text, "schedule");
        Require(schedules.Count == 1, "archive must embed exactly one schedule");
        string scheduleHash = ScenarioReferenceArchive.Hash(schedules[0]);
        Require(scheduleHash == stored.scheduleHash && scheduleHash == PinnedSchedule,
            $"embedded schedule hash {scheduleHash} (stored {stored.scheduleHash}, pinned {PinnedSchedule})");
        string scheduleFile = Compact(File.ReadAllText(SchedulePath), 0, out _);
        Require(scheduleFile == schedules[0], "schedule resource file differs from the archive's embedded schedule");
    }

    // Compact JSON of every object value whose key is exactly "key", in order.
    private static List<string> EmbeddedObjects(string text, string key)
    {
        var result = new List<string>();
        string token = "\"" + key + "\"";
        int at = 0;
        while ((at = text.IndexOf(token, at, StringComparison.Ordinal)) >= 0)
        {
            int colon = at + token.Length;
            while (colon < text.Length && char.IsWhiteSpace(text[colon])) colon++;
            if (colon >= text.Length || text[colon] != ':') { at += token.Length; continue; }
            int open = colon + 1;
            while (open < text.Length && char.IsWhiteSpace(text[open])) open++;
            if (open < text.Length && text[open] == '{')
            {
                result.Add(Compact(text, open, out int end));
                at = end;
            }
            else at = open;
        }
        return result;
    }

    // Copies one balanced JSON value starting at 'start' (or the whole text if
    // it starts with whitespace before '{'), dropping whitespace outside strings.
    private static string Compact(string text, int start, out int end)
    {
        var sb = new StringBuilder();
        bool quoted = false, escaped = false;
        int depth = 0;
        int i = start;
        for (; i < text.Length; i++)
        {
            char c = text[i];
            if (!quoted && char.IsWhiteSpace(c)) continue;
            sb.Append(c);
            if (quoted)
            {
                if (escaped) escaped = false;
                else if (c == '\\') escaped = true;
                else if (c == '"') quoted = false;
                continue;
            }
            if (c == '"') quoted = true;
            else if (c == '{' || c == '[') depth++;
            else if ((c == '}' || c == ']') && --depth == 0) { i++; break; }
        }
        end = i;
        return sb.ToString();
    }

    private static void Require(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException("Reference archive integrity: " + message);
    }
}
