using System.Text;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;
using SitkaP1.Phase1Save.Tests;

// 2D Phase 1 A0 save-foundation tests (contract level only). They exercise the public DTO, codec, validator and slot store
// with hand-built SYNTHETIC states. They do NOT capture or restore a real running forest (that is A1). Slot tests use a fresh
// temporary directory outside the repository, removed at the end.

string repoMarker = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
string scratch = Directory.CreateTempSubdirectory("ccf-phase1-save-tests.").FullName;
string outside = Directory.CreateTempSubdirectory("ccf-phase1-save-outside.").FullName;
if (Path.GetFullPath(scratch).StartsWith(repoMarker + Path.DirectorySeparatorChar, StringComparison.Ordinal))
    throw new Exception("the slot test directory must be outside the repository");
int slotCounter = 0;
string NewSlotDir()
{
    string dir = Path.Combine(scratch, "slots-" + (++slotCounter).ToString(System.Globalization.CultureInfo.InvariantCulture));
    Directory.CreateDirectory(dir);
    return dir;
}

var approvedSave = Samples.Phase2Approved();
var pendingSave = Samples.Phase2Pending();
var model0Save = Samples.Model0();
byte[] approvedBytes = Phase1SaveCodec.Encode(approvedSave);
byte[] pendingBytes = Phase1SaveCodec.Encode(pendingSave);
byte[] model0Bytes = Phase1SaveCodec.Encode(model0Save);
var phase2Expect = Samples.Expect(Phase1ModelVector.Phase2);
var model0Expect = Samples.Expect(Phase1ModelVector.Model0);

// ------------------------------------------------------------------ exact round trip and determinism
T.Add("roundtrip: Phase 2 with approved plan, every region, exact by reflection", () =>
{
    var decoded = Phase1SaveCodec.Decode(approvedBytes, phase2Expect);
    Deep.Equal(approvedSave, decoded.Envelope);
    T.Eq(approvedBytes.Length, decoded.ByteLength, "byte length");
});
T.Add("roundtrip: Phase 2 with every kind of pending work (Fell, Prune, Clear, Plant/Shelter/SpotClear)", () =>
{
    var decoded = Phase1SaveCodec.Decode(pendingBytes, phase2Expect);
    Deep.Equal(pendingSave, decoded.Envelope);
    var orders = decoded.Envelope.State.Phase2!.PendingOrders;
    T.True(orders[0] is Phase1SpotClearOrder && orders[1] is Phase1PlantOrder { SpeciesId: "beech" } && orders[2] is Phase1ShelterOrder,
        "polymorphic planting orders keep their concrete types and order");
});
T.Add("roundtrip: model 0/0/0/0/0 save keeps no Stage 4/5/Phase 2 parts", () =>
{
    var decoded = Phase1SaveCodec.Decode(model0Bytes, model0Expect);
    Deep.Equal(model0Save, decoded.Envelope);
    var s = decoded.Envelope.State;
    T.True(s.Stage4 is null && s.Stage5 is null && s.Phase2 is null && s.Options.Stage4 is null, "no later-model state appears");
});
T.Add("determinism: re-encoding and rebuilding give byte-identical output", () =>
{
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(approvedSave), "encode twice");
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(Samples.Phase2Approved()), "independently rebuilt state");
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(Phase1SaveCodec.Decode(approvedBytes).Envelope), "decode then encode");
    T.Bytes(model0Bytes, Phase1SaveCodec.Encode(Phase1SaveCodec.Decode(model0Bytes).Envelope), "model 0 decode then encode");
});
T.Add("precision: float/double bits, negative zero, subnormal, max, int64 above 2^53 and free text survive exactly", () =>
{
    var e = Phase1SaveCodec.Decode(approvedBytes).Envelope;
    var a001 = e.State.Areas[0].Trees.Single(t => t.TreeId == "A001");
    T.Eq(0x80000000u, BitConverter.SingleToUInt32Bits(a001.XM), "negative zero float");
    T.Eq(0x00000001u, BitConverter.SingleToUInt32Bits(a001.ZM), "subnormal float");
    T.Eq(0x7f7fffffu, BitConverter.SingleToUInt32Bits(a001.DbhCm), "float.MaxValue");
    T.Eq(0x3db851ecu, BitConverter.SingleToUInt32Bits(a001.CrownRadiusM), "odd pattern");
    T.Eq(0x8000000000000000ul, BitConverter.DoubleToUInt64Bits(e.State.Events[0].Areas[0].Before.BasalAreaM2PerHa), "negative zero double");
    T.Eq(BitConverter.DoubleToUInt64Bits(0.1 + 0.2), BitConverter.DoubleToUInt64Bits(e.State.Events[0].Areas[0].Before.RelativeDensity), "0.1+0.2");
    T.Eq(Samples.BigSeed, e.Header.Genesis.GeneratorSeed, "int64 2^53+1");
    T.Eq(Samples.Reason, e.State.Events[0].Areas[0].Fells[0].Reason, "reason text with quotes, backslash, tab, accents and emoji");
    string json = Mut.Text(approvedBytes);
    T.True(json.Contains("{\"k\":\"f32\",\"b\":\"80000000\"}", StringComparison.Ordinal), "f32 written as its bit pattern");
    T.True(json.Contains("\"generatorSeed\":{\"k\":\"i64\",\"b\":\"0020000000000001\"}", StringComparison.Ordinal), "i64 written as bits");
    T.True(!json.Contains("0.30000000000000004", StringComparison.Ordinal), "no decimal float text");
});

// ------------------------------------------------------------------ version / identity refusals
T.Add("refuse: other format marker or schema version", () =>
{
    T.Eq(2, Phase1SaveFormat.SchemaVersion, "R1 schema version");
    T.Refuses("SaveSchemaUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"schemaVersion\":2", "\"schemaVersion\":3")));
    T.Refuses("SaveSchemaUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"schemaVersion\":2", "\"schemaVersion\":1")));
    T.Refuses("SaveSchemaUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"schemaVersion\":2", "\"schemaVersion\":0")));
    T.Refuses("SaveFormatUnknown", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "ccf-2d-multiarea-save", "ccf-3d-save")));
    T.Refuses("SaveFormatUnknown", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes("[1,2]")));
});
T.Add("refuse: model-vector mismatch both ways; model 0 is never upgraded", () =>
{
    var e = T.Refuses("SaveModelMismatch", () => Phase1SaveCodec.Decode(model0Bytes, phase2Expect));
    T.True(e.Message.Contains("0/0/0/0/0", StringComparison.Ordinal) && e.Message.Contains("never upgraded", StringComparison.Ordinal),
        "explanatory message names the saved vector and says no upgrade");
    T.Refuses("SaveModelMismatch", () => Phase1SaveCodec.Decode(approvedBytes, model0Expect));
    T.Refuses("SaveModelMismatch", () => Phase1SaveCodec.Decode(approvedBytes, Samples.Expect(Phase1ModelVector.Stage5)));
    // Without an expectation the model 0 save decodes as itself: nothing is added or filled in.
    T.Eq(Phase1ModelVector.Model0, Phase1SaveCodec.Decode(model0Bytes).Envelope.Header.Models, "model 0 stays model 0");
});
T.Add("refuse: unsupported or tampered model vector, and later-model state in a model 0 save", () =>
{
    T.Refuses("SaveModelUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(model0Bytes,
        "\"models\":{\"adult\":0", "\"models\":{\"adult\":1")));
    // A Phase 2 payload relabelled as model 0 is refused: its Stage 4/5/Phase 2 parts cannot exist under model 0.
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes,
        "\"models\":{\"adult\":1,\"mortality\":1,\"crown\":1,\"regeneration\":1,\"species\":1}",
        "\"models\":{\"adult\":0,\"mortality\":0,\"crown\":0,\"regeneration\":0,\"species\":0}")));
});
T.Add("refuse: genesis identity mismatch (content hash, area seed, experiment)", () =>
{
    var lineage = Phase1EngineLineage.Current(Phase1ModelVector.Phase2);
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(hash: new string('f', 64)), lineage)));
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(seedB: 8), lineage)));
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis() with { ExperimentId = "mad-other" }, lineage)));
});
T.Add("F5 lineage: the header carries the engine lineage and a load requires every field to match exactly", () =>
{
    var current = Phase1EngineLineage.Current(Phase1ModelVector.Phase2);
    // The sample lineage is the real engine lineage, built as MultiAreaContinuation builds it.
    T.Eq("ma1-connected-woodland-stage4-adult-mortality-v1+regeneration-v1+species-v1", current.AreaModelVersion, "Phase 2 area model");
    T.Eq("ma1-isolated-per-area-v1", Phase1EngineLineage.Current(Phase1ModelVector.Model0).AreaModelVersion, "model 0 area model");
    T.Eq("ma1-connected-woodland-crown-c2-v1", Phase1EngineLineage.Current(Phase1ModelVector.Stage3CrownC2).AreaModelVersion, "C2 area model");
    T.Eq(current, Phase1SaveCodec.Decode(approvedBytes).Envelope.Header.Lineage, "lineage round trip");
    T.True(Mut.Text(approvedBytes).Contains("\"lineage\":{\"rngModelVersion\":", StringComparison.Ordinal), "lineage is in the header");
    var older = new[]
    {
        current with { RngModelVersion = current.RngModelVersion + 1 },
        current with { GrowthModelVersion = current.GrowthModelVersion - 1 },
        current with { P1AlgorithmVersion = current.P1AlgorithmVersion + "-old" },
        current with { AreaModelVersion = "ma1-connected-woodland-stage4-adult-mortality-v1+regeneration-v1" },
        current with { ActionEligibleDbhThresholdMm = current.ActionEligibleDbhThresholdMm + 1 },
    };
    foreach (var other in older)
    {
        var e = T.Refuses("SaveLineageMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
            new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(), other)));
        T.True(e.Message.Contains("never continued", StringComparison.Ordinal), "explanatory lineage refusal");
        // A model 0 save (no receipts to cross-check) is refused the same way.
        T.Refuses("SaveLineageMismatch", () => Phase1SaveCodec.Decode(model0Bytes,
            new Phase1SaveExpectation(Phase1ModelVector.Model0, Samples.Genesis(), other with { AreaModelVersion = "ma1-isolated-per-area-v0" })));
    }
    // Tampering the header lineage of a save with receipts (resealed) is refused: receipts record the session's lineage.
    T.RefusesRule("recorded lineage", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes,
        "\"p1AlgorithmVersion\":\"" + current.P1AlgorithmVersion + "\",\"areaModelVersion\"",
        "\"p1AlgorithmVersion\":\"p1-older\",\"areaModelVersion\"")));
    T.Refuses("SaveExpectationInvalid", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(), null!)));
});

// ------------------------------------------------------------------ strict decoding refusals
T.Add("refuse: duplicate JSON key", () =>
{
    T.Refuses("DuplicateKey", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(Mut.Text(approvedBytes),
        "\"schemaVersion\":2,", "\"schemaVersion\":2,\"schemaVersion\":2,"))));
    T.Refuses("DuplicateKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"workRevision\":2,", "\"workRevision\":2,\"workRevision\":2,")));
});
T.Add("refuse: unknown enum values (work action, planting order type, work-log kind)", () =>
{
    // The reason is text-hex encoded, so the literal action string is the only "Fell" token in the orders.
    T.Refuses("SaveEnumUnknown", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(pendingBytes, "\"action\":\"Fell\"", "\"action\":\"Chop\"")));
    T.Refuses("SaveEnumUnknown", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(pendingBytes, "\"type\":\"plant\"", "\"type\":\"graft\"")));
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(pendingBytes, "\"kind\":\"Plant\"", "\"kind\":\"Sow\"")));
});
T.Add("refuse: missing and unknown fields", () =>
{
    T.Refuses("MissingKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24,", "")));
    T.Refuses("MissingKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, ",\"nextPatchNumber\":2", "")));
    T.Refuses("UnknownKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24,",
        "\"planStateRevision\":24,\"sessionId\":\"abc\",")));
});
T.Add("refuse: trailing bytes, whitespace, BOM, escapes and non-canonical spelling", () =>
{
    T.Refuses("JsonInvalid", () => Phase1SaveCodec.Decode(approvedBytes.Concat(Encoding.UTF8.GetBytes("{}")).ToArray()));
    T.Refuses("JsonInvalid", () => Phase1SaveCodec.Decode(approvedBytes.Concat(new byte[] { (byte)'x' }).ToArray()));
    T.Refuses("SaveNotCanonical", () => Phase1SaveCodec.Decode(approvedBytes.Concat(new byte[] { (byte)'\n' }).ToArray()));
    T.Refuses("NotCanonical", () => Phase1SaveCodec.Decode(new byte[] { 0xEF, 0xBB, 0xBF }.Concat(approvedBytes).ToArray()));
    T.Refuses("SaveNotCanonical", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(Mut.Text(approvedBytes),
        "\"format\":", "\"form\\u0061t\":"))));
    T.Refuses("SaveNotCanonical", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(Mut.Text(approvedBytes), ",\"header\":", ", \"header\":"))));
    T.Refuses("NumberNotInteger", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":24.0")));
    T.Refuses("FloatBitsInvalid", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"b\":\"80000000\"", "\"b\":\"8000000\"")));
    T.Refuses("JsonInvalid", () => Phase1SaveCodec.Decode(Array.Empty<byte>()));
    T.Refuses("JsonInvalid", () => Phase1SaveCodec.Decode(approvedBytes.AsSpan(0, approvedBytes.Length / 2)));
});
T.Add("refuse: over-deep nesting", () =>
{
    string deep = "{\"format\":\"ccf-2d-multiarea-save\",\"schemaVersion\":1,\"x\":" + new string('[', 40) + new string(']', 40) + "}";
    T.Refuses("JsonInvalid", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(deep)));
});
T.Add("refuse: oversized input before parsing", () =>
{
    T.Refuses("SaveTooLarge", () => Phase1SaveCodec.Decode(approvedBytes, null, maxBytes: approvedBytes.Length - 1));
    T.Refuses("SaveTooLarge", () => Phase1SaveCodec.Decode(new byte[Phase1SaveFormat.MaxSaveBytes + 1]));
    T.True(Phase1SaveCodec.Decode(approvedBytes, null, maxBytes: approvedBytes.Length).ByteLength == approvedBytes.Length, "exactly at the cap is accepted");
});
T.Add("refuse: non-finite float32/float64 on decode and encode; overflowing integer", () =>
{
    T.Refuses("FloatNotFinite", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"b\":\"80000000\"", "\"b\":\"7f800000\"")));
    T.Refuses("FloatNotFinite", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"b\":\"8000000000000000\"", "\"b\":\"7ff8000000000000\"")));
    T.Refuses("NumberOutOfRange", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":2147483648")));
    T.Refuses("NumberOutOfRange", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":9007199254740992")));
    var s = Samples.Model0();
    var badStand = s with { State = s.State with { AnnualHistory = new[] { s.State.AnnualHistory[0] with { Areas = new[] {
        s.State.AnnualHistory[0].Areas[0] with { Before = s.State.AnnualHistory[0].Areas[0].Before with { StemsPerHa = double.NaN } },
        s.State.AnnualHistory[0].Areas[1] } } } } };
    T.Refuses("FloatNotFinite", () => Phase1SaveCodec.Encode(badStand));
    var badTree = s with { State = s.State with { Areas = new[] { s.State.Areas[0] with { Trees = new[] {
        s.State.Areas[0].Trees[0] with { HeightM = float.PositiveInfinity }, s.State.Areas[0].Trees[1] } }, s.State.Areas[1] } } };
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Encode(badTree));
});
T.Add("refuse: corrupt checksum and edited content", () =>
{
    string json = Mut.Text(approvedBytes);
    int at = json.IndexOf("\"value\":\"", StringComparison.Ordinal) + 9;
    char flipped = json[at] == '0' ? '1' : '0';
    T.Refuses("SaveChecksumMismatch", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(json[..at] + flipped + json[(at + 1)..])));
    // An edit that stays canonical but is not resealed: the checksum is what refuses it.
    T.Refuses("SaveChecksumMismatch", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(json, "\"b\":\"80000000\"", "\"b\":\"00000000\""))));
    T.Refuses("SaveChecksumMismatch", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(json, "\"label\":\"Spring review\"", "\"label\":\"Spring reviev\""))));
    T.Refuses("SaveChecksumInvalid", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(json[..at] + json.Substring(at, 64).ToUpperInvariant() + json[(at + 64)..])));
});
T.Add("refuse: inconsistent but well-formed saves (validator)", () =>
{
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"nextDeadwoodNumber\":2", "\"nextDeadwoodNumber\":3")));
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"annualRevision\":3", "\"annualRevision\":4")));
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"cropTreeIds\":[\"A004\"]", "\"cropTreeIds\":[\"A003\"]")));
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"siteProductivity\":\"uniform-1.0\"",
        "\"siteProductivity\":\"custom\"")));
    var s = Samples.Phase2Approved();
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Encode(s with { State = s.State with { Pending = Samples.Phase2Pending().State.Pending } }));
    T.Refuses("SaveLabelInvalid", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Label = "bad \"label\"" } }));
    T.Refuses("SaveTimeInvalid", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { SavedUtc = "2026-11-24 13:53:37" } }));
    var badText = s with { State = s.State with { Approved = s.State.Approved! with { Orders = new[] {
        s.State.Approved.Orders[0] with { Reason = "lone \ud800 surrogate" } } } } };
    T.Refuses("SaveTextInvalid", () => Phase1SaveCodec.Encode(badText));
});
// R1: the former "previously loaded state untouched" assertion over a pure decode could not fail (Decode takes a
// ReadOnlySpan and returns new immutable records), so it was replaced by a slot-level fail-closed check that can fail: a
// refused Load/LoadBackup/Inspect must not repair, recover, rewrite or delete anything on disk.
T.Add("fail-closed: refused slot loads and inspections change no file on disk", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    slots.Save("alpha", approvedBytes, replaceExisting: false);
    slots.Save("alpha", model0Bytes, replaceExisting: true); // backup = approved
    string primary = Path.Combine(slots.Directory, "alpha.ccf2d.json"), backup = primary + ".bak";
    byte[] corrupt = Mut.EditAndReseal(model0Bytes, "\"cropTreeIds\":[\"B001\"]", "\"cropTreeIds\":[\"B002\"]");
    File.WriteAllBytes(primary, corrupt);
    var before = Directory.EnumerateFiles(slots.Directory).Order(StringComparer.Ordinal)
        .Select(f => (Path.GetFileName(f), File.ReadAllBytes(f))).ToArray();
    T.Refuses("SaveInconsistent", () => slots.Load("alpha", model0Expect));
    T.Refuses("SaveModelMismatch", () => slots.LoadBackup("alpha", model0Expect));
    T.Eq("SaveInconsistent", slots.Inspect("alpha").PrimaryRefusal, "inspect reports the refusal");
    var after = Directory.EnumerateFiles(slots.Directory).Order(StringComparer.Ordinal)
        .Select(f => (Path.GetFileName(f), File.ReadAllBytes(f))).ToArray();
    T.Eq(string.Join(",", before.Select(f => f.Item1)), string.Join(",", after.Select(f => f.Item1)), "same files");
    for (int i = 0; i < before.Length; i++) T.Bytes(before[i].Item2, after[i].Item2, before[i].Item1 + " unchanged");
    T.Bytes(corrupt, File.ReadAllBytes(primary), "corrupt primary kept for inspection, not repaired");
    T.Bytes(approvedBytes, File.ReadAllBytes(backup), "backup kept");
});

// ------------------------------------------------------------------ slot store
T.Add("slots: illegal names refused (traversal, absolute, separators, dots, case, device, length, empty)", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    foreach (var name in new[] { "", "..", ".", "../x", "a/b", "a\\b", "/etc/passwd", "C:\\x", "Upper", "a.b", "-lead", "con", "nul", "lpt1",
                 "slot name", "slot\u00e9", new string('a', 49), "a\0b", "x.ccf2d.json", "alpha\n", "\nalpha", "al\npha", "alpha\r\n",
                 "alpha\r" })
    {
        T.Refuses("SlotNameInvalid", () => slots.Save(name, approvedBytes, replaceExisting: true));
        T.Refuses("SlotNameInvalid", () => slots.Load(name, null));
    }
    T.Refuses("SlotNameInvalid", () => slots.Load(null!, null));
    T.Eq(0, Directory.EnumerateFileSystemEntries(slots.Directory).Count(), "nothing written");
});
T.Add("slots: directory must be absolute, existing and not a symlink", () =>
{
    T.Refuses("SlotDirectoryInvalid", () => new Phase1SaveSlots("relative/dir"));
    T.Refuses("SlotDirectoryInvalid", () => new Phase1SaveSlots(Path.Combine(scratch, "missing-dir")));
    string link = Path.Combine(scratch, "linked-slots");
    Directory.CreateSymbolicLink(link, outside);
    T.Refuses("SlotDirectoryInvalid", () => new Phase1SaveSlots(link));
});
T.Add("slots: save, list, load, explicit replace and prior-good backup", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    var first = slots.Save("alpha", approvedBytes, replaceExisting: false);
    T.Eq(Phase1SlotCommitStatus.DurablyCommitted, first.Status, "durable commit");
    T.True(!first.BackupUpdated, "no backup on first save");
    T.Eq("alpha", string.Join(",", slots.ListSlots()), "listing");
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(slots.Load("alpha", phase2Expect).Envelope), "load");
    T.Refuses("SlotExists", () => slots.Save("alpha", pendingBytes, replaceExisting: false));
    T.Bytes(approvedBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json")), "unconfirmed replace changed nothing");
    var second = slots.Save("alpha", pendingBytes, replaceExisting: true);
    T.True(second.BackupUpdated && second.Status == Phase1SlotCommitStatus.DurablyCommitted, "backup refreshed, durable");
    T.Bytes(pendingBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json")), "primary replaced");
    T.Bytes(approvedBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json.bak")), "backup is the prior good save");
    T.Eq(second.Checksum, slots.Inspect("alpha").PrimaryChecksum, "inspect primary");
    T.Eq(first.Checksum, slots.Inspect("alpha").BackupChecksum, "inspect backup");
    T.Eq("alpha", string.Join(",", slots.ListSlots()), "backup is not listed as a slot");
});
T.Add("slots: invalid bytes are never written", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    T.Refuses("JsonInvalid", () => slots.Save("beta", Encoding.UTF8.GetBytes("not json"), replaceExisting: true));
    T.Refuses("SaveChecksumMismatch", () => slots.Save("beta", Encoding.UTF8.GetBytes(Mut.ReplaceFirst(Mut.Text(approvedBytes),
        "\"b\":\"80000000\"", "\"b\":\"00000000\"")), replaceExisting: true));
    T.Eq(0, Directory.EnumerateFileSystemEntries(slots.Directory).Count(), "no file, no temp left");
});
T.Add("slots: symlink escape refused for load, save and backup, outside file untouched", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    string target = Path.Combine(outside, "victim.json");
    File.WriteAllBytes(target, approvedBytes);
    File.CreateSymbolicLink(Path.Combine(slots.Directory, "evil.ccf2d.json"), target);
    T.Refuses("SlotSymlinkRefused", () => slots.Load("evil", null));
    T.Refuses("SlotSymlinkRefused", () => slots.Save("evil", pendingBytes, replaceExisting: true));
    slots.Save("gamma", approvedBytes, replaceExisting: false);
    File.CreateSymbolicLink(Path.Combine(slots.Directory, "gamma.ccf2d.json.bak"), target);
    T.Refuses("SlotSymlinkRefused", () => slots.Save("gamma", pendingBytes, replaceExisting: true));
    T.Refuses("SlotSymlinkRefused", () => slots.LoadBackup("gamma", null));
    T.Bytes(approvedBytes, File.ReadAllBytes(target), "the file outside the slot directory is unchanged");
    T.Bytes(approvedBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "gamma.ccf2d.json")), "gamma unchanged");
});
T.Add("slots: a fault at every write stage leaves the previous good slot and backup intact, no temp left", () =>
{
    foreach (var stage in Enum.GetValues<Phase1SlotStage>())
    {
        string dir = NewSlotDir();
        new Phase1SaveSlots(dir).Save("alpha", approvedBytes, replaceExisting: false);
        new Phase1SaveSlots(dir).Save("alpha", model0Bytes, replaceExisting: true); // backup = approved, primary = model 0
        var faulty = new Phase1SaveSlots(dir, s => { if (s == stage) throw new IOException("simulated crash / disk full at " + s); });
        try { faulty.Save("alpha", pendingBytes, replaceExisting: true); throw new Exception("expected the simulated fault at " + stage); }
        catch (IOException) { }
        T.Bytes(model0Bytes, File.ReadAllBytes(Path.Combine(dir, "alpha.ccf2d.json")), stage + ": primary unchanged");
        var inspect = new Phase1SaveSlots(dir).Inspect("alpha");
        T.True(inspect.PrimaryRefusal is null && inspect.BackupRefusal is null, stage + ": primary and backup still verify");
        T.Eq(0, inspect.LeftoverTempFiles.Count, stage + ": temp file removed");
        T.Bytes(model0Bytes, Phase1SaveCodec.Encode(new Phase1SaveSlots(dir).Load("alpha", model0Expect).Envelope), stage + ": still loads");
    }
});
T.Add("slots: a temp file left by a killed process is ignored, reported and removable", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    slots.Save("alpha", approvedBytes, replaceExisting: false);
    string leftover = Path.Combine(slots.Directory, "alpha.ccf2d.json.tmp-deadbeefdeadbeef");
    File.WriteAllBytes(leftover, approvedBytes.AsSpan(0, 100).ToArray());
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(slots.Load("alpha", phase2Expect).Envelope), "the good slot still loads");
    T.Eq("alpha", string.Join(",", slots.ListSlots()), "the temp is not a slot");
    T.Eq(1, slots.Inspect("alpha").LeftoverTempFiles.Count, "reported");
    T.Eq(1, slots.RemoveLeftoverTemps("alpha"), "removed");
    T.True(!File.Exists(leftover), "gone");
    T.True(File.Exists(Path.Combine(slots.Directory, "alpha.ccf2d.json")), "primary kept");
});
T.Add("slots: missing primary → refused, then recovered from the prior-good backup", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    slots.Save("alpha", approvedBytes, replaceExisting: false);
    slots.Save("alpha", pendingBytes, replaceExisting: true);
    File.Delete(Path.Combine(slots.Directory, "alpha.ccf2d.json"));
    T.Refuses("SlotMissing", () => slots.Load("alpha", phase2Expect));
    var inspect = slots.Inspect("alpha");
    T.True(!inspect.PrimaryExists && inspect.BackupExists && inspect.BackupRefusal is null, "backup verifies");
    var recovered = slots.RecoverFromBackup("alpha");
    T.Eq(Phase1SlotCommitStatus.DurablyCommitted, recovered.Status, "recovery durable");
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(slots.Load("alpha", phase2Expect).Envelope), "prior good save restored");
    T.True(File.Exists(Path.Combine(slots.Directory, "alpha.ccf2d.json.bak")), "backup kept");
});
T.Add("slots: corrupt primary is refused, never promoted over a good backup, and recoverable", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    slots.Save("alpha", approvedBytes, replaceExisting: false);
    slots.Save("alpha", model0Bytes, replaceExisting: true); // backup = approved
    File.WriteAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json"), model0Bytes.AsSpan(0, model0Bytes.Length - 10).ToArray());
    T.Refuses("JsonInvalid", () => slots.Load("alpha", model0Expect));
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(slots.LoadBackup("alpha", phase2Expect).Envelope), "backup still good");
    var replaced = slots.Save("alpha", pendingBytes, replaceExisting: true);
    T.True(!replaced.BackupUpdated, "the corrupt primary did not become the backup");
    T.Bytes(approvedBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json.bak")), "good backup kept");
    File.WriteAllBytes(Path.Combine(slots.Directory, "alpha.ccf2d.json"), Encoding.UTF8.GetBytes("garbage"));
    slots.RecoverFromBackup("alpha");
    T.Bytes(approvedBytes, Phase1SaveCodec.Encode(slots.Load("alpha", phase2Expect).Envelope), "recovered");
});
T.Add("slots: a directory or bad backup in place of a slot file is refused", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    Directory.CreateDirectory(Path.Combine(slots.Directory, "delta.ccf2d.json"));
    T.Refuses("SlotNotAFile", () => slots.Save("delta", approvedBytes, replaceExisting: true));
    T.Refuses("SlotNotAFile", () => slots.Load("delta", null));
    slots.Save("eps", approvedBytes, replaceExisting: false);
    File.WriteAllBytes(Path.Combine(slots.Directory, "eps.ccf2d.json.bak"), Encoding.UTF8.GetBytes("garbage"));
    T.Refuses("JsonInvalid", () => slots.RecoverFromBackup("eps"));
    T.Bytes(approvedBytes, File.ReadAllBytes(Path.Combine(slots.Directory, "eps.ccf2d.json")), "primary untouched by a refused recovery");
});

// ------------------------------------------------------------------ R1 repairs
T.Add("F1 slot names: trailing, leading and embedded newlines are refused everywhere a name is matched", () =>
{
    foreach (var name in new[] { "alpha\n", "\nalpha", "al\npha" })
    {
        T.Refuses("SlotNameInvalid", () => Phase1SaveSlots.ValidateSlotName(name));
        var slots = new Phase1SaveSlots(NewSlotDir());
        T.Refuses("SlotNameInvalid", () => slots.Save(name, approvedBytes, replaceExisting: false));
        T.Refuses("SlotNameInvalid", () => slots.LoadBackup(name, null));
        T.Refuses("SlotNameInvalid", () => slots.Inspect(name));
        T.Refuses("SlotNameInvalid", () => slots.RecoverFromBackup(name));
        T.Refuses("SlotNameInvalid", () => slots.RemoveLeftoverTemps(name));
        T.Eq(0, Directory.EnumerateFileSystemEntries(slots.Directory).Count(), "nothing written for " + name.Replace("\n", "\\n"));
    }
    // ListSlots ignores a file whose name part only matches with a trailing newline.
    var listing = new Phase1SaveSlots(NewSlotDir());
    listing.Save("alpha", approvedBytes, replaceExisting: false);
    File.WriteAllBytes(Path.Combine(listing.Directory, "beta\n" + Phase1SaveSlots.Extension), approvedBytes);
    T.Eq("alpha", string.Join(",", listing.ListSlots()), "newline-named file is not listed as a slot");
});
T.Add("F2 invalid UTF-8: typed refusal from Decode, Inspect and Save; a corrupt primary is still replaceable", () =>
{
    string json = Mut.Text(approvedBytes);
    byte[] badString = approvedBytes.ToArray();
    badString[json.IndexOf("Spring review", StringComparison.Ordinal)] = 0xFF;            // high-bit byte inside a string value
    byte[] badKey = approvedBytes.ToArray();
    badKey[json.IndexOf("\"label\"", StringComparison.Ordinal) + 1] = 0xC3;               // truncated 2-byte sequence inside a key
    byte[] overlong = approvedBytes.ToArray();
    int at = json.IndexOf("Spring review", StringComparison.Ordinal);
    overlong[at] = 0xC1; overlong[at + 1] = 0x81;                                          // overlong encoding of 'A'
    foreach (var bad in new[] { badString, badKey, overlong })
    {
        T.Refuses("SaveUtf8Invalid", () => Phase1SaveCodec.Decode(bad));
        T.Refuses("SaveUtf8Invalid", () => Phase1SaveCodec.Decode(bad, phase2Expect));
    }
    // Valid UTF-8 that is not printable ASCII inside an identifier string is still a typed refusal (StrictJson rule).
    var asciiOnly = T.RefusesAny(() => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "Spring review", "Spring révïew")));
    T.True(asciiOnly.Code.Length > 0, "typed: " + asciiOnly.Code);

    var slots = new Phase1SaveSlots(NewSlotDir());
    // Invalid bytes are never written.
    T.Refuses("SaveUtf8Invalid", () => slots.Save("alpha", badString, replaceExisting: true));
    T.Eq(0, Directory.EnumerateFileSystemEntries(slots.Directory).Count(), "no file, no temp");
    slots.Save("alpha", approvedBytes, replaceExisting: false);
    slots.Save("alpha", model0Bytes, replaceExisting: true); // backup = approved
    string primary = Path.Combine(slots.Directory, "alpha.ccf2d.json");
    File.WriteAllBytes(primary, badString);                // bit rot in the primary
    T.Refuses("SaveUtf8Invalid", () => slots.Load("alpha", null));
    var inspect = slots.Inspect("alpha");
    T.True(inspect.PrimaryExists && inspect.PrimaryRefusal == "SaveUtf8Invalid" && inspect.BackupRefusal is null,
        "Inspect reports a typed refusal instead of throwing: " + inspect.PrimaryRefusal);
    var replaced = slots.Save("alpha", pendingBytes, replaceExisting: true);
    T.True(!replaced.BackupUpdated, "the corrupt primary did not become the backup");
    T.Bytes(pendingBytes, File.ReadAllBytes(primary), "the corrupt primary was replaced by a normal save");
    T.Bytes(approvedBytes, File.ReadAllBytes(primary + ".bak"), "good backup kept");
    T.Eq(0, slots.Inspect("alpha").LeftoverTempFiles.Count, "no temp file left");
});
T.Add("F3 no-clobber: a slot created concurrently after the existence check is never overwritten", () =>
{
    string dir = NewSlotDir();
    var other = new Phase1SaveSlots(dir);
    bool raced = false;
    // Park the first writer after its temp is written and verified, just before the publish; a second writer creates
    // the slot in that window. The first writer must then refuse, and the second writer's save must survive intact.
    var parked = new Phase1SaveSlots(dir, stage =>
    {
        if (stage != Phase1SlotStage.BeforeReplace || raced) return;
        raced = true;
        other.Save("alpha", pendingBytes, replaceExisting: false);
    });
    T.Refuses("SlotExists", () => parked.Save("alpha", approvedBytes, replaceExisting: false));
    T.True(raced, "the concurrent writer ran inside the window");
    T.Bytes(pendingBytes, File.ReadAllBytes(Path.Combine(dir, "alpha.ccf2d.json")), "the concurrent save was not clobbered");
    T.True(!File.Exists(Path.Combine(dir, "alpha.ccf2d.json.bak")), "no backup invented");
    T.Eq(0, other.Inspect("alpha").LeftoverTempFiles.Count, "the refused writer removed its temp");
    T.Eq(1, Directory.EnumerateFileSystemEntries(dir).Count(), "only the slot file remains");
    // The published new slot is a single-link regular file (no leftover hard link from the publish).
    T.Bytes(pendingBytes, Phase1SaveCodec.Encode(other.Load("alpha", phase2Expect).Envelope), "loads through the single-link reader");
    // The confirmed-replace path keeps prior-good backup semantics.
    var replaced = other.Save("alpha", approvedBytes, replaceExisting: true);
    T.True(replaced.BackupUpdated, "confirmed replace refreshed the backup");
    T.Bytes(pendingBytes, File.ReadAllBytes(Path.Combine(dir, "alpha.ccf2d.json.bak")), "backup is the prior good save");
});
T.Add("F4a accounts: Stage 5 and Phase 2 annual accounts align to the year", () =>
{
    var s = Samples.Phase2Approved();
    var st = s.State;
    T.RefusesRule("account", () => Phase1SaveCodec.Encode(s with { State = st with { Stage5 = st.Stage5! with {
        Accounts = Array.Empty<Phase1Stage5YearAccount>() } } }));
    T.RefusesRule("account", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2! with {
        Accounts = Array.Empty<Phase1Phase2YearAccount>() } } }));
    T.RefusesRule("account", () => Phase1SaveCodec.Encode(s with { State = st with { Stage5 = st.Stage5! with {
        Accounts = st.Stage5.Accounts.Take(2).ToArray() } } }));
    T.RefusesRule("account", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2! with {
        Accounts = new[] { st.Phase2.Accounts[1], st.Phase2.Accounts[0], st.Phase2.Accounts[2] } } } }));
});
T.Add("F4b mast seeds: Stage 5 and Phase 2 seeds are the engine's derivation from options and genesis", () =>
{
    var s = Samples.Phase2Approved();
    var st = s.State;
    T.RefusesRule("mast seed", () => Phase1SaveCodec.Encode(s with { State = st with { Stage5 = st.Stage5! with { MastSeed = 999 } } }));
    T.RefusesRule("mast seed", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2! with { OakMastSeed = 999 } } }));
    T.RefusesRule("mast seed", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2! with { BeechMastSeed = 42 } } }));
    // Positive control: naming the seed in the options makes the same state value legal.
    var named = s with { State = st with { Options = st.Options with { Stage5 = st.Options.Stage5! with { MastSeed = 999 } },
        Stage5 = st.Stage5! with { MastSeed = 999 }, Phase2 = st.Phase2! with { BeechMastSeed = 999 } } };
    Deep.Equal(named, Phase1SaveCodec.Decode(Phase1SaveCodec.Encode(named)).Envelope);
});
T.Add("F4c lattice: the Phase 2 band lattice matches the Stage 5 cells position by position", () =>
{
    var s = Samples.Phase2Approved();
    var st = s.State;
    var a = st.Phase2!.Areas[0];
    T.RefusesRule("band lattice", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2 with { Areas = new[] {
        a with { Cells = new[] { a.Cells[1], a.Cells[0] } }, st.Phase2.Areas[1] } } } }));
    T.RefusesRule("band lattice", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2 with { Areas = new[] {
        a with { Cells = new[] { a.Cells[0] } }, st.Phase2.Areas[1] } } } }));
    T.RefusesRule("band lattice", () => Phase1SaveCodec.Encode(s with { State = st with { Phase2 = st.Phase2 with { Areas = new[] {
        a with { Cells = new[] { a.Cells[0], a.Cells[1] with { Column = 2 } } }, st.Phase2.Areas[1] } } } }));
});
T.Add("F4d plan-state revision: below the proven lower bound is refused, at the bound is accepted", () =>
{
    T.RefusesRule("plan-state revision", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":1")));
    T.RefusesRule("plan-state revision", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":19")));
    T.Eq(20, Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":24", "\"planStateRevision\":20"))
        .Envelope.State.PlanStateRevision, "the exact lower bound (20) is accepted");
    T.RefusesRule("plan-state revision", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(pendingBytes, "\"planStateRevision\":23", "\"planStateRevision\":18")));
    T.Eq(19, Phase1SaveCodec.Decode(Mut.EditAndReseal(pendingBytes, "\"planStateRevision\":23", "\"planStateRevision\":19"))
        .Envelope.State.PlanStateRevision, "pending sample lower bound (19) is accepted");
    // Model 0 with one Crop mark needs at least one MarkCrop.
    T.RefusesRule("plan-state revision", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(model0Bytes, "\"planStateRevision\":1", "\"planStateRevision\":0")));
});
T.Add("F4e deaths and fells are tied: a Fell death needs its fell record, a fell needs a Fell death in its year", () =>
{
    var s = Samples.Phase2Approved();
    var st = s.State;
    Phase1SaveEnvelope WithTree(string areaId, string treeId, Func<Phase1Tree, Phase1Tree> change) => s with { State = st with {
        Areas = st.Areas.Select(a => a.AreaId != areaId ? a : a with { Trees = a.Trees.Select(t => t.TreeId == treeId ? change(t) : t).ToArray() })
            .ToArray() } };
    // A Harvest death with no fell record (A003 relabelled as felled).
    T.RefusesRule("Fell death of", () => Phase1SaveCodec.Encode(WithTree("MA-A", "A003", t => t with {
        Death = t.Death! with { Cause = FellCause.Harvest, Mechanism = FellCause.Mechanism } })));
    // A fell whose tree's death year differs from the event year.
    T.RefusesRule("died by Fell", () => Phase1SaveCodec.Encode(WithTree("MA-A", "A002", t => t with { Death = t.Death! with { EcologicalYear = 2 } })));
    // A fell whose tree died naturally.
    T.RefusesRule("died by Fell", () => Phase1SaveCodec.Encode(WithTree("MA-B", "B002", t => t with {
        Death = t.Death! with { Cause = "DensitySuppression", Mechanism = "density" } })));
    // A fell record that does not carry the engine's Fell cause.
    T.RefusesRule("Fell cause", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes,
        "\"cause\":\"fell\",\"mechanism\":\"manual-work-plan\",\"reason\"", "\"cause\":\"felled\",\"mechanism\":\"manual-work-plan\",\"reason\"")));
});
T.Add("F4f provenance: receipt tokens match genesis, event and lineage; receipt orders match ledger fells", () =>
{
    var s = Samples.Phase2Approved();
    var st = s.State;
    Phase1SaveEnvelope WithReceipt(int index, Func<Phase1ExecutedWork, Phase1ExecutedWork> change) => s with { State = st with {
        Executed = st.Executed.Select((x, i) => i == index ? change(x) : x).ToArray() } };
    T.RefusesRule("names this experiment", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { RecordedSource = x.RecordedSource with {
        ManifestContentHash = new string('f', 64) } })));
    T.RefusesRule("names this experiment", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { RecordedSource = x.RecordedSource with {
        ExperimentId = "mad-other" } })));
    T.RefusesRule("names this experiment", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { RecordedSource = x.RecordedSource with {
        AreaIds = "MA-A" } })));
    T.RefusesRule("names this experiment", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { RecordedSource = x.RecordedSource with {
        Domain = "SomethingElse" } })));
    // Years / revisions out of range for the receipt's event.
    T.RefusesRule("exactly as it was", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { RecordedSource = Samples.Token(0, 0, 0) })));
    T.RefusesRule("exactly as it was", () => Phase1SaveCodec.Encode(WithReceipt(1, x => x with { RecordedSource = Samples.Token(7, 7, 1) })));
    T.RefusesRule("exactly as it was", () => Phase1SaveCodec.Encode(WithReceipt(1, x => x with { RecordedSource = Samples.Token(2, 2, 0) })));
    T.RefusesRule("recorded lineage", () => Phase1SaveCodec.Encode(WithReceipt(1, x => x with { RecordedSource = x.RecordedSource with {
        GrowthModelVersion = x.RecordedSource.GrowthModelVersion + 1 } })));
    // Receipt orders differ from the event's fells (other tree, other reason, extra order).
    T.RefusesRule("orders are exactly", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { Orders = st.Executed[1].Orders })));
    T.RefusesRule("orders are exactly", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { Orders = new[] { x.Orders[0] with { Reason = "other" } } })));
    T.RefusesRule("orders are exactly", () => Phase1SaveCodec.Encode(WithReceipt(0, x => x with { Orders = x.Orders.Concat(st.Executed[1].Orders).ToArray() })));
});
T.Add("B1 approved plan: only orders are saved (no planId, executionKey or recordedSource)", () =>
{
    T.Eq("Orders", string.Join(",", typeof(Phase1ApprovedPlan).GetProperties().Select(p => p.Name)), "DTO has orders only");
    string json = Mut.Text(approvedBytes);
    T.True(json.Contains("\"approved\":{\"orders\":[{", StringComparison.Ordinal), "approved object holds only orders");
    T.True(!json.Contains("key-approved", StringComparison.Ordinal) && !json.Contains("ma-plan:9", StringComparison.Ordinal), "no minted id");
    T.Refuses("UnknownKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"approved\":{\"orders\":",
        "\"approved\":{\"planId\":\"sess:ma-plan:9\",\"orders\":")));
    // An approval with no order of any kind cannot exist (Approve refuses an empty plan).
    var s = Samples.Phase2Approved();
    var st = s.State;
    T.RefusesRule("at least one order", () => Phase1SaveCodec.Encode(s with { State = st with {
        Approved = new Phase1ApprovedPlan(Array.Empty<Phase1WorkOrder>()), Stage4 = st.Stage4! with { ApprovedPrunes = Array.Empty<Phase1PruneOrder>() },
        Stage5 = st.Stage5! with { ApprovedClears = Array.Empty<Phase1ClearOrder>() }, Phase2 = st.Phase2! with {
            ApprovedOrders = Array.Empty<Phase1PlantingOrder>() } } }));
});
T.Add("robustness: null header parts are typed refusals, not NullReferenceException", () =>
{
    var s = Samples.Phase2Approved();
    T.Refuses("SaveModelUnsupported", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Models = null! } }));
    T.RefusesRule("lineage", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Lineage = null! } }));
    T.RefusesRule("genesis identity", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Genesis = null! } }));
    T.RefusesRule("content hash", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Genesis = s.Header.Genesis with {
        ManifestContentHash = null! } } }));
    T.Refuses("SaveTimeInvalid", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { SavedUtc = null! } }));
    T.Refuses("SaveLabelInvalid", () => Phase1SaveCodec.Encode(s with { Header = s.Header with { Label = null! } }));
});
T.Add("robustness: a symlinked ancestor of the slot directory is refused at construction", () =>
{
    string realParent = Path.Combine(outside, "real-parent");
    Directory.CreateDirectory(Path.Combine(realParent, "slots"));
    string linkedParent = Path.Combine(scratch, "linked-parent");
    Directory.CreateSymbolicLink(linkedParent, realParent);
    var e = T.Refuses("SlotDirectoryInvalid", () => new Phase1SaveSlots(Path.Combine(linkedParent, "slots")));
    T.True(e.Message.Contains("linked-parent", StringComparison.Ordinal), "names the symlinked component");
    T.True(new Phase1SaveSlots(Path.Combine(realParent, "slots")).Directory.EndsWith("slots", StringComparison.Ordinal), "real path accepted");
});
T.Add("robustness: Inspect reports a directory at the primary path as present and not a file", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    Directory.CreateDirectory(Path.Combine(slots.Directory, "delta.ccf2d.json"));
    Directory.CreateDirectory(Path.Combine(slots.Directory, "delta.ccf2d.json.bak"));
    var inspect = slots.Inspect("delta");
    T.True(inspect.PrimaryExists && inspect.PrimaryRefusal == "SlotNotAFile", "primary: " + inspect.PrimaryExists + "/" + inspect.PrimaryRefusal);
    T.True(inspect.BackupExists && inspect.BackupRefusal == "SlotNotAFile", "backup: " + inspect.BackupExists + "/" + inspect.BackupRefusal);
    var missing = slots.Inspect("nothing");
    T.True(!missing.PrimaryExists && missing.PrimaryRefusal is null, "a genuinely absent slot is still 'does not exist'");
});
T.Add("NB1a: an absent slot is published no-clobber even when replaceExisting is true", () =>
{
    // A0 defect NB1: with replaceExisting:true, Save used a plain rename for a slot it saw as absent, so a competing
    // writer's save could be clobbered. The publish is now no-clobber whenever the slot was absent, whatever the caller asked.
    string dir = NewSlotDir();
    var other = new Phase1SaveSlots(dir);
    bool raced = false;
    var racer = new Phase1SaveSlots(dir, stage =>
    {
        if (stage != Phase1SlotStage.BeforeReplace || raced) return;
        raced = true;
        other.Save("beta", pendingBytes, replaceExisting: false);
    });
    T.Refuses("SlotExists", () => racer.Save("beta", approvedBytes, replaceExisting: true));
    T.True(raced, "the concurrent writer ran inside the window");
    T.Bytes(pendingBytes, File.ReadAllBytes(Path.Combine(dir, "beta.ccf2d.json")), "the concurrent save was not clobbered");
    T.True(!File.Exists(Path.Combine(dir, "beta.ccf2d.json.bak")), "no backup invented for a slot that did not exist");
    T.Eq(0, other.Inspect("beta").LeftoverTempFiles.Count, "the refused writer removed its temp");
});
T.Add("NB1b: concurrent saves of one slot are serialized and leave one complete verified slot", () =>
{
    // Owner A0 repair NB1: mutations of one slot name are serialized inside the process, so two requests for the same slot
    // can never interleave their temp/backup/rename steps. Both saves succeed, in some order, and the slot is one complete
    // save with the other kept as its prior-good backup.
    string dir = NewSlotDir();
    var slots = new Phase1SaveSlots(dir);
    var results = new List<Phase1SlotSaveResult>();
    var faults = new List<Exception>();
    using var start = new Barrier(2);
    void Writer(byte[] bytes) => new Thread(() =>
    {
        start.SignalAndWait();
        try { var r = slots.Save("race", bytes, replaceExisting: true); lock (results) results.Add(r); }
        catch (Exception e) { lock (faults) faults.Add(e); }
    }).Start();
    Writer(approvedBytes);
    Writer(pendingBytes);
    for (int i = 0; i < 400 && (results.Count < 2 && faults.Count == 0); i++) Thread.Sleep(10);
    T.Eq(0, faults.Count, "no concurrent save failed" + (faults.Count == 0 ? "" : ": " + faults[0].Message));
    T.Eq(2, results.Count, "both serialized saves reported success");
    string primary = Path.Combine(dir, "race.ccf2d.json");
    var decoded = Phase1SaveCodec.Decode(File.ReadAllBytes(primary));
    T.True(decoded.Envelope.Header.Label is "Spring review" or "Pending draft",
        "the slot holds one complete save, not a mixture: " + decoded.Envelope.Header.Label);
    var inspect = slots.Inspect("race");
    T.Eq(0, inspect.LeftoverTempFiles.Count, "no temp file left");
    T.True(inspect.PrimaryRefusal is null && inspect.BackupExists && inspect.BackupRefusal is null,
        "one save is the primary and the other its verified backup");
});
T.Add("NB1c: a generation read and written inside the slot scope is unique per save", () =>
{
    // Two saves of one slot that read the generation, then write, can otherwise both record the same number. The scope
    // (Phase1SaveSlots.LockSlot) is re-entrant and is what makes the read and the write one critical section.
    string dir = NewSlotDir();
    var slots = new Phase1SaveSlots(dir);
    slots.Save("gen", approvedBytes, replaceExisting: false); // next generation is 2
    var generations = new List<long>();
    var faults = new List<Exception>();
    using var start = new Barrier(2);
    void Writer(byte[] bytes) => new Thread(() =>
    {
        start.SignalAndWait();
        try
        {
            using (slots.LockSlot("gen"))
            {
                var (_, generation) = slots.NextGeneration("gen");
                lock (generations) generations.Add(generation);
                Thread.Sleep(40); // widen the window that used to let two saves record one number
                slots.Save("gen", bytes, replaceExisting: true);
            }
        }
        catch (Exception e) { lock (faults) faults.Add(e); }
    }).Start();
    Writer(model0Bytes);
    Writer(pendingBytes);
    for (int i = 0; i < 400 && (generations.Count < 2 || faults.Count > 0); i++) Thread.Sleep(10);
    T.Eq(0, faults.Count, "no scoped save failed" + (faults.Count == 0 ? "" : ": " + faults[0].Message));
    T.Eq(2, generations.Count, "both scoped saves ran");
    generations.Sort();
    T.True(generations[0] != generations[1], "the two saves recorded different generations (" + string.Join(",", generations) + ")");
    T.Eq(1L, generations[1] - generations[0], "the second save carries the next generation after the first");
});
T.Add("R2 backup recovery is refused while the primary still verifies, and is exactly what a corrupt primary gets", () =>
{
    // Independent review finding 2: Restore backup used to replace a valid newer save with the older backup on one click.
    // The store now refuses recovery while the primary decodes, so a good save can never be rolled back by accident.
    var slots = new Phase1SaveSlots(NewSlotDir());
    slots.Save("alpha", model0Bytes, replaceExisting: false);
    slots.Save("alpha", pendingBytes, replaceExisting: true); // backup = model0, primary = pending
    string primary = Path.Combine(slots.Directory, "alpha.ccf2d.json"), backup = primary + ".bak";
    byte[] primaryBefore = File.ReadAllBytes(primary);
    var refused = T.Refuses("SlotPrimaryValid", () => slots.RecoverFromBackup("alpha"));
    T.True(refused.Message.Contains("still holds a complete, verified save", StringComparison.Ordinal), "the refusal says why");
    T.Bytes(primaryBefore, File.ReadAllBytes(primary), "the newer save is byte-identical after the refusal");
    T.Bytes(model0Bytes, File.ReadAllBytes(backup), "the backup is untouched");
    T.Eq(0, slots.Inspect("alpha").LeftoverTempFiles.Count, "no temp file left");

    // A primary that no longer verifies is the one case recovery exists for.
    File.WriteAllBytes(primary, Encoding.UTF8.GetBytes("{ not a save"));
    var recovered = slots.RecoverFromBackup("alpha");
    T.Eq(Phase1SlotCommitStatus.DurablyCommitted, recovered.Status, "recovery over a corrupt primary committed");
    T.Bytes(model0Bytes, File.ReadAllBytes(primary), "the verified backup was restored over the corrupt primary");
    T.Bytes(model0Bytes, File.ReadAllBytes(backup), "the backup itself is kept");
    T.Eq(0, slots.Inspect("alpha").LeftoverTempFiles.Count, "still no temp file");
});
T.Add("golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding", () =>
{
    // Pins the exact canonical bytes of schema 2. Any change to key order, spelling, tags, fixture values or the schema
    // changes this value; update it only together with a deliberate, reviewed format change.
    const string PinnedFileSha256 = "c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7";
    const string PinnedEmbeddedChecksum = "9629ac1648fabd85702fc4105075cebe9f69e884a816edcce4c441a3866c9086";
    string fileSha = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(approvedBytes)).ToLowerInvariant();
    string embedded = Phase1SaveCodec.Decode(approvedBytes).Checksum;
    T.Eq(24703, approvedBytes.Length, "byte length");
    T.Eq(PinnedFileSha256, fileSha, "file SHA-256");
    T.Eq(PinnedEmbeddedChecksum, embedded, "embedded checksum");
});

int exit;
try
{
    Console.WriteLine("Phase1 save tests — slot scratch: " + scratch);
    exit = T.RunAll();
}
finally
{
    Directory.Delete(scratch, recursive: true);
    Directory.Delete(outside, recursive: true);
    Console.WriteLine("scratch removed: " + (!Directory.Exists(scratch) && !Directory.Exists(outside)));
}
return exit;
