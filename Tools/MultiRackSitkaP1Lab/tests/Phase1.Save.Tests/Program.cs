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
    T.Refuses("SaveSchemaUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"schemaVersion\":1", "\"schemaVersion\":2")));
    T.Refuses("SaveSchemaUnsupported", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"schemaVersion\":1", "\"schemaVersion\":0")));
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
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(hash: new string('f', 64)))));
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis(seedB: 8))));
    T.Refuses("SaveGenesisMismatch", () => Phase1SaveCodec.Decode(approvedBytes,
        new Phase1SaveExpectation(Phase1ModelVector.Phase2, Samples.Genesis() with { ExperimentId = "mad-other" })));
});

// ------------------------------------------------------------------ strict decoding refusals
T.Add("refuse: duplicate JSON key", () =>
{
    T.Refuses("DuplicateKey", () => Phase1SaveCodec.Decode(Encoding.UTF8.GetBytes(Mut.ReplaceFirst(Mut.Text(approvedBytes),
        "\"schemaVersion\":1,", "\"schemaVersion\":1,\"schemaVersion\":1,"))));
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
    T.Refuses("MissingKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":9,", "")));
    T.Refuses("MissingKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, ",\"nextPatchNumber\":2", "")));
    T.Refuses("UnknownKey", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":9,",
        "\"planStateRevision\":9,\"sessionId\":\"abc\",")));
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
    T.Refuses("NumberNotInteger", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":9", "\"planStateRevision\":9.0")));
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
    T.Refuses("NumberOutOfRange", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":9", "\"planStateRevision\":2147483648")));
    T.Refuses("NumberOutOfRange", () => Phase1SaveCodec.Decode(Mut.EditAndReseal(approvedBytes, "\"planStateRevision\":9", "\"planStateRevision\":9007199254740992")));
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
T.Add("fail-closed: a refused decode changes neither its input nor previously loaded state", () =>
{
    byte[] corrupt = Mut.EditAndReseal(approvedBytes, "\"nextDeadwoodNumber\":2", "\"nextDeadwoodNumber\":3");
    byte[] before = corrupt.ToArray();
    var loaded = Phase1SaveCodec.Decode(approvedBytes, phase2Expect).Envelope;
    byte[] loadedBytes = Phase1SaveCodec.Encode(loaded);
    T.Refuses("SaveInconsistent", () => Phase1SaveCodec.Decode(corrupt, phase2Expect));
    T.Refuses("SaveModelMismatch", () => Phase1SaveCodec.Decode(model0Bytes, phase2Expect));
    T.Bytes(before, corrupt, "input bytes untouched");
    T.Bytes(loadedBytes, Phase1SaveCodec.Encode(loaded), "previously decoded state untouched");
    Deep.Equal(approvedSave, loaded);
});

// ------------------------------------------------------------------ slot store
T.Add("slots: illegal names refused (traversal, absolute, separators, dots, case, device, length, empty)", () =>
{
    var slots = new Phase1SaveSlots(NewSlotDir());
    foreach (var name in new[] { "", "..", ".", "../x", "a/b", "a\\b", "/etc/passwd", "C:\\x", "Upper", "a.b", "-lead", "con", "nul", "lpt1",
                 "slot name", "slot\u00e9", new string('a', 49), "a\0b", "x.ccf2d.json" })
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
