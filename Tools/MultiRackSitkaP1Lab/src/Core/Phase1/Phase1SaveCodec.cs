using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SitkaP1.Core.Phase1;

/// <summary>
/// Strict, deterministic encode/decode of a 2D save (spec §3). Encoding writes one canonical byte form: fixed key order,
/// no whitespace, lists in the order the state holds them (the state itself keeps engine order or ordinal order, never a
/// dictionary walk), floats and doubles as IEEE-754 bit patterns, longs as int64 bit patterns and free text as UTF-8 hex.
/// Decoding accepts only that canonical form: it parses with <see cref="StrictJson"/> (UTF-8, no BOM, duplicate keys
/// refused, depth ≤ 16, integers only, nothing after the root), reads every field with an exact key set, re-encodes the
/// result and requires byte equality, verifies the checksum, runs <see cref="Phase1SaveValidator"/> and, when asked,
/// requires the exact model vector and genesis identity. Nothing is ever upgraded or repaired.
/// Decode never changes its input and returns a new value only on full success (fail-closed).
/// </summary>
public static class Phase1SaveCodec
{
    // ------------------------------------------------------------------------------------------------ public API

    /// <summary>Encodes a consistent save. An inconsistent state is refused rather than written.</summary>
    public static byte[] Encode(Phase1SaveEnvelope save)
    {
        ArgumentNullException.ThrowIfNull(save);
        Phase1SaveValidator.Validate(save);
        string header = HeaderJson(save.Header);
        string payload = PayloadJson(save.State);
        byte[] bytes = Encoding.UTF8.GetBytes(EnvelopeJson(header, payload, Checksum(header, payload)));
        if (bytes.Length > Phase1SaveFormat.MaxSaveBytes)
            throw new P1Refusal("SaveTooLarge", FormattableString.Invariant(
                $"the encoded save is {bytes.Length} bytes; the limit is {Phase1SaveFormat.MaxSaveBytes}"));
        return bytes;
    }

    /// <summary>Decodes and fully verifies a save, without an identity requirement (used for listing and slot checks).</summary>
    public static Phase1DecodedSave Decode(ReadOnlySpan<byte> bytes) => Decode(bytes, expectation: null);

    /// <summary>
    /// Decodes and fully verifies a save. With <paramref name="expectation"/>, the save must have exactly that model vector
    /// and genesis identity; a model 0 (or any other) save is never upgraded into a different session.
    /// </summary>
    public static Phase1DecodedSave Decode(ReadOnlySpan<byte> bytes, Phase1SaveExpectation? expectation,
        int maxBytes = Phase1SaveFormat.MaxSaveBytes)
    {
        if (maxBytes <= 0 || maxBytes > Phase1SaveFormat.MaxSaveBytes)
            throw new ArgumentOutOfRangeException(nameof(maxBytes));
        if (bytes.Length > maxBytes)
            throw new P1Refusal("SaveTooLarge", FormattableString.Invariant($"the save is {bytes.Length} bytes; the limit is {maxBytes}"));

        var root = StrictJson.Parse(bytes);
        RequireFormatAndSchema(root);
        var top = J.Keys(root, "save", "format", "schemaVersion", "header", "payload", "checksum");
        var header = ReadHeader(top["header"]);
        var state = ReadState(top["payload"]);
        var check = J.Keys(top["checksum"], "checksum", "alg", "value");
        if (J.Str(check["alg"], "checksum.alg") != Phase1SaveFormat.ChecksumAlgorithm)
            throw new P1Refusal("SaveChecksumInvalid", "unknown checksum algorithm");
        string storedChecksum = J.Str(check["value"], "checksum.value");
        if (!IsLowerHex(storedChecksum, 64))
            throw new P1Refusal("SaveChecksumInvalid", "the checksum must be 64 lowercase hex digits");

        // Exact canonical form: the typed value re-encoded must reproduce the input byte for byte, so key order,
        // whitespace, escapes and number spelling have one accepted form only.
        string headerJson = HeaderJson(header);
        string payloadJson = PayloadJson(state);
        byte[] canonical = Encoding.UTF8.GetBytes(EnvelopeJson(headerJson, payloadJson, storedChecksum));
        if (!bytes.SequenceEqual(canonical))
            throw new P1Refusal("SaveNotCanonical", "the save is not in the one canonical encoding (order, spacing or spelling differs)");
        if (!string.Equals(Checksum(headerJson, payloadJson), storedChecksum, StringComparison.Ordinal))
            throw new P1Refusal("SaveChecksumMismatch", "the save content does not match its checksum; it is corrupt or was edited");

        var save = new Phase1SaveEnvelope(header, state);
        Phase1SaveValidator.Validate(save);
        if (expectation is not null) RequireExpected(save.Header, expectation);
        return new Phase1DecodedSave(save, storedChecksum, bytes.Length);
    }

    /// <summary>
    /// Refuses unless <paramref name="header"/> has exactly the expected model vector and genesis identity. The messages
    /// say what differs; nothing is migrated (owner choice A).
    /// </summary>
    public static void RequireExpected(Phase1SaveHeader header, Phase1SaveExpectation expectation)
    {
        ArgumentNullException.ThrowIfNull(header);
        ArgumentNullException.ThrowIfNull(expectation);
        if (header.Models != expectation.Models)
            throw new P1Refusal("SaveModelMismatch", "this save was made with model vector " + header.Models +
                " and cannot be loaded into a " + expectation.Models + " session; saves are never upgraded or migrated between models");
        var a = header.Genesis;
        var b = expectation.Genesis;
        bool same = a.ExperimentId == b.ExperimentId && a.ManifestContentHash == b.ManifestContentHash &&
            a.GeneratorId == b.GeneratorId && a.GeneratorVersion == b.GeneratorVersion && a.GeneratorSeed == b.GeneratorSeed &&
            a.Areas.Count == b.Areas.Count &&
            a.Areas.Zip(b.Areas).All(p => p.First.AreaId == p.Second.AreaId && p.First.SimulationSeed == p.Second.SimulationSeed &&
                F32.Bits(p.First.AreaHa) == F32.Bits(p.Second.AreaHa));
        if (!same)
            throw new P1Refusal("SaveGenesisMismatch", "this save belongs to experiment " + a.ExperimentId + " (content hash " +
                a.ManifestContentHash + "), not to the running experiment " + b.ExperimentId + " (content hash " +
                b.ManifestContentHash + "); no forest identity is substituted");
    }

    /// <summary>The checksum hex of an encoded header and payload: SHA-256 over their exact canonical UTF-8 bytes.</summary>
    private static string Checksum(string headerJson, string payloadJson)
    {
        byte[] data = Encoding.UTF8.GetBytes(headerJson + payloadJson);
        return Convert.ToHexString(SHA256.HashData(data)).ToLowerInvariant();
    }

    private static void RequireFormatAndSchema(JNode root)
    {
        // Read the format and schema first, so a different file or a newer schema gets a clear refusal, not a shape error.
        if (root is not JObj obj) throw new P1Refusal("SaveFormatUnknown", "a 2D save must be a JSON object");
        JNode? format = null, schema = null;
        foreach (var kv in obj.Props)
        {
            if (kv.Key == "format") format = kv.Value;
            else if (kv.Key == "schemaVersion") schema = kv.Value;
        }
        if (format is not JStr f || f.Value != Phase1SaveFormat.Format)
            throw new P1Refusal("SaveFormatUnknown", "this is not a " + Phase1SaveFormat.Format + " file");
        if (schema is not JInt s || s.Value != Phase1SaveFormat.SchemaVersion)
            throw new P1Refusal("SaveSchemaUnsupported", "only schema version " +
                Phase1SaveFormat.SchemaVersion.ToString(CultureInfo.InvariantCulture) + " can be read; no migration exists");
    }

    private static string EnvelopeJson(string header, string payload, string checksum)
    {
        var w = new Phase1JsonWriter();
        w.BeginObject().Str("format", Phase1SaveFormat.Format).Int("schemaVersion", Phase1SaveFormat.SchemaVersion)
            .Raw("header", header).Raw("payload", payload)
            .BeginObject("checksum").Str("alg", Phase1SaveFormat.ChecksumAlgorithm).Str("value", checksum).EndObject()
            .EndObject();
        return w.ToString();
    }

    // ------------------------------------------------------------------------------------------------ header

    private static string HeaderJson(Phase1SaveHeader h)
    {
        var w = new Phase1JsonWriter();
        w.BeginObject().Str("label", h.Label).Str("savedUtc", h.SavedUtc).I64("saveGeneration", h.SaveGeneration)
            .Int("ecologicalYear", h.EcologicalYear);
        w.BeginObject("models").Int("adult", h.Models.Adult).Int("mortality", h.Models.Mortality).Int("crown", h.Models.Crown)
            .Int("regeneration", h.Models.Regeneration).Int("species", h.Models.Species).EndObject();
        var g = h.Genesis;
        w.BeginObject("genesis").Str("experimentId", g.ExperimentId).Str("manifestContentHash", g.ManifestContentHash)
            .Str("generatorId", g.GeneratorId).Int("generatorVersion", g.GeneratorVersion).I64("generatorSeed", g.GeneratorSeed)
            .BeginArray("areas");
        foreach (var a in g.Areas)
            w.BeginObject().Str("areaId", a.AreaId).Int("simulationSeed", a.SimulationSeed).F32("areaHa", a.AreaHa).EndObject();
        w.EndArray().EndObject().EndObject();
        return w.ToString();
    }

    private static Phase1SaveHeader ReadHeader(JNode n)
    {
        var o = J.Keys(n, "header", "label", "savedUtc", "saveGeneration", "ecologicalYear", "models", "genesis");
        var m = J.Keys(o["models"], "header.models", "adult", "mortality", "crown", "regeneration", "species");
        var models = new Phase1ModelVector(R.I32(m["adult"], "models.adult"), R.I32(m["mortality"], "models.mortality"),
            R.I32(m["crown"], "models.crown"), R.I32(m["regeneration"], "models.regeneration"), R.I32(m["species"], "models.species"));
        var g = J.Keys(o["genesis"], "header.genesis", "experimentId", "manifestContentHash", "generatorId", "generatorVersion",
            "generatorSeed", "areas");
        var areas = R.List(g["areas"], "genesis.areas", x =>
        {
            var a = J.Keys(x, "genesis.area", "areaId", "simulationSeed", "areaHa");
            return new Phase1GenesisArea(J.Str(a["areaId"], "areaId"), R.I32(a["simulationSeed"], "simulationSeed"),
                R.F32(a["areaHa"], "areaHa"));
        });
        var genesis = new Phase1GenesisIdentity(J.Str(g["experimentId"], "experimentId"),
            J.Str(g["manifestContentHash"], "manifestContentHash"), J.Str(g["generatorId"], "generatorId"),
            R.I32(g["generatorVersion"], "generatorVersion"), R.I64(g["generatorSeed"], "generatorSeed"), areas);
        return new Phase1SaveHeader(J.Str(o["label"], "label"), J.Str(o["savedUtc"], "savedUtc"),
            R.I64(o["saveGeneration"], "saveGeneration"), R.I32(o["ecologicalYear"], "ecologicalYear"), models, genesis);
    }

    // ------------------------------------------------------------------------------------------------ payload writer

    private static string PayloadJson(Phase1SaveState s)
    {
        var w = new Phase1JsonWriter();
        w.BeginObject();
        WriteOptions(w, s.Options);
        w.Int("ecologicalYear", s.EcologicalYear).Int("annualRevision", s.AnnualRevision).Int("workRevision", s.WorkRevision)
            .Int("planStateRevision", s.PlanStateRevision);
        w.BeginArray("areas");
        foreach (var a in s.Areas)
        {
            w.BeginObject().Str("areaId", a.AreaId).Int("ecologicalYear", a.EcologicalYear).BeginArray("trees");
            foreach (var t in a.Trees)
            {
                w.BeginObject().Str("treeId", t.TreeId).F32("xM", t.XM).F32("zM", t.ZM).Int("ageYears", t.AgeYears)
                    .F32("dbhCm", t.DbhCm).F32("heightM", t.HeightM).F32("crownRadiusM", t.CrownRadiusM);
                if (t.Death is null) w.Null("death"); else { w.Key("death"); WriteDeath(w, t.Death); }
                w.EndObject();
            }
            w.EndArray().StrArray("cropTreeIds", a.CropTreeIds).EndObject();
        }
        w.EndArray();
        w.BeginArray("events");
        foreach (var e in s.Events)
        {
            w.BeginObject().Str("executionKey", e.ExecutionKey).Str("planId", e.PlanId).Int("ecologicalYear", e.EcologicalYear)
                .Int("workRevision", e.WorkRevision).BeginArray("areas");
            foreach (var c in e.Areas)
            {
                w.BeginObject().Str("areaId", c.AreaId);
                w.Key("before"); WriteStand(w, c.Before);
                w.Key("after"); WriteStand(w, c.After);
                w.BeginArray("fells");
                foreach (var f in c.Fells)
                    w.BeginObject().Str("areaId", f.AreaId).Str("treeId", f.TreeId).Str("cause", f.Cause).Str("mechanism", f.Mechanism)
                        .Text("reason", f.Reason).Int("ecologicalYear", f.EcologicalYear).Int("workRevision", f.WorkRevision)
                        .Str("executionKey", f.ExecutionKey).Str("planId", f.PlanId).EndObject();
                w.EndArray().EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray();
        w.BeginArray("executed");
        foreach (var x in s.Executed)
        {
            w.BeginObject().Str("executionKey", x.ExecutionKey);
            w.Key("recordedSource"); WriteToken(w, x.RecordedSource);
            WriteOrders(w, "orders", x.Orders);
            w.EndObject();
        }
        w.EndArray();
        WriteOrders(w, "pending", s.Pending);
        if (s.Approved is null) w.Null("approved");
        else
        {
            w.BeginObject("approved").Str("planId", s.Approved.PlanId).Str("executionKey", s.Approved.ExecutionKey);
            w.Key("recordedSource"); WriteToken(w, s.Approved.RecordedSource);
            WriteOrders(w, "orders", s.Approved.Orders);
            w.EndObject();
        }
        w.BeginArray("annualHistory");
        foreach (var y in s.AnnualHistory)
        {
            w.BeginObject().Int("yearBefore", y.YearBefore).Int("yearAfter", y.YearAfter).Int("annualRevisionAfter", y.AnnualRevisionAfter)
                .Int("workRevision", y.WorkRevision).BeginArray("areas");
            foreach (var a in y.Areas)
            {
                w.BeginObject().Str("areaId", a.AreaId);
                w.Key("before"); WriteStand(w, a.Before);
                w.Key("after"); WriteStand(w, a.After);
                w.BeginArray("naturalDeaths");
                foreach (var d in a.NaturalDeaths) WriteDeath(w, d);
                w.EndArray().Bool("noLivingStems", a.NoLivingStems).EndObject();
            }
            w.EndArray().Bool("cancelledUnexecutedWork", y.CancelledUnexecutedWork).EndObject();
        }
        w.EndArray();
        if (s.Stage4 is null) w.Null("stage4"); else { w.Key("stage4"); WriteStage4(w, s.Stage4); }
        if (s.Stage5 is null) w.Null("stage5"); else { w.Key("stage5"); WriteStage5(w, s.Stage5); }
        if (s.Phase2 is null) w.Null("phase2"); else { w.Key("phase2"); WritePhase2(w, s.Phase2); }
        w.EndObject();
        return w.ToString();
    }

    private static void WriteOptions(Phase1JsonWriter w, Phase1Options o)
    {
        w.BeginObject("options");
        if (o.Stage4 is null) w.Null("stage4");
        else
        {
            w.BeginObject("stage4").Str("siteProductivity", o.Stage4.SiteProductivity);
            if (o.Stage4.GrowthAgePhases is null) w.Null("growthAgePhases");
            else
            {
                w.BeginArray("growthAgePhases");
                foreach (var p in o.Stage4.GrowthAgePhases)
                    w.BeginObject().F32("startAgeYears", p.StartAgeYears).F32("dbhIncrementFactor", p.DbhIncrementFactor)
                        .F32("heightIncrementFactor", p.HeightIncrementFactor).EndObject();
                w.EndArray();
            }
            w.EndObject();
        }
        if (o.Stage5 is null) w.Null("stage5");
        else
            w.BeginObject("stage5").F64("gridCellM", o.Stage5.GridCellM).F32("backgroundBrowsePressure", o.Stage5.BackgroundBrowsePressure)
                .IntOrNull("mastSeed", o.Stage5.MastSeed).EndObject();
        if (o.Phase2 is null) w.Null("phase2");
        else
        {
            w.BeginObject("phase2").IntOrNull("oakMastSeed", o.Phase2.OakMastSeed).IntOrNull("beechMastSeed", o.Phase2.BeechMastSeed);
            WriteSpecies(w, "genesisSpecies", o.Phase2.GenesisSpecies);
            w.EndObject();
        }
        w.EndObject();
    }

    private static void WriteSpecies(Phase1JsonWriter w, string key, IReadOnlyList<Phase1TreeSpecies> list)
    {
        w.BeginArray(key);
        foreach (var e in list) w.BeginObject().Str("treeId", e.TreeId).Str("speciesId", e.SpeciesId).EndObject();
        w.EndArray();
    }

    private static void WriteDeath(Phase1JsonWriter w, Phase1Death d) =>
        w.BeginObject().Str("treeId", d.TreeId).Int("ecologicalYear", d.EcologicalYear).Str("cause", d.Cause)
            .Str("mechanism", d.Mechanism).EndObject();

    private static void WriteStand(Phase1JsonWriter w, Phase1Stand s) =>
        w.BeginObject().Int("livingCount", s.LivingCount).F64("stemsPerHa", s.StemsPerHa).F64("basalAreaM2PerHa", s.BasalAreaM2PerHa)
            .F64("quadraticMeanDbhCm", s.QuadraticMeanDbhCm).F64("relativeDensity", s.RelativeDensity).EndObject();

    private static void WriteToken(Phase1JsonWriter w, Phase1SourceToken t) =>
        w.BeginObject().Str("domain", t.Domain).Str("experimentId", t.ExperimentId).Str("manifestContentHash", t.ManifestContentHash)
            .Str("areaIds", t.AreaIds).Int("ecologicalYear", t.EcologicalYear).Int("annualRevision", t.AnnualRevision)
            .Int("workRevision", t.WorkRevision).Int("rngModelVersion", t.RngModelVersion).Int("growthModelVersion", t.GrowthModelVersion)
            .Str("p1AlgorithmVersion", t.P1AlgorithmVersion).Str("areaModelVersion", t.AreaModelVersion)
            .I64("actionEligibleDbhThresholdMm", t.ActionEligibleDbhThresholdMm).EndObject();

    private static void WriteOrders(Phase1JsonWriter w, string key, IReadOnlyList<Phase1WorkOrder> orders)
    {
        w.BeginArray(key);
        foreach (var o in orders)
            w.BeginObject().Str("areaId", o.AreaId).Str("treeId", o.TreeId).Str("action", ActionName(o.Action)).Text("reason", o.Reason)
                .EndObject();
        w.EndArray();
    }

    private static string ActionName(WorkAction action) => action switch
    {
        WorkAction.Fell => "Fell",
        _ => throw new P1Refusal("SaveEnumUnknown", "unknown work action"),
    };

    private static void WriteStage4(Phase1JsonWriter w, Phase1Stage4State s)
    {
        w.BeginObject().BeginArray("pruning");
        foreach (var p in s.Pruning)
            w.BeginObject().Str("treeId", p.TreeId).Int("lifts", p.Lifts).F32("crownBaseHeightM", p.CrownBaseHeightM)
                .Int("lastPruningYear", p.LastPruningYear).EndObject();
        w.EndArray();
        WritePrunes(w, "pendingPrunes", s.PendingPrunes);
        WritePrunes(w, "approvedPrunes", s.ApprovedPrunes);
        w.BeginArray("pruneLog");
        foreach (var r in s.PruneLog)
            w.BeginObject().Str("areaId", r.AreaId).Str("treeId", r.TreeId).F32("targetCrownBaseHeightM", r.TargetCrownBaseHeightM)
                .Int("ecologicalYear", r.EcologicalYear).Int("workRevision", r.WorkRevision).Str("executionKey", r.ExecutionKey)
                .Str("planId", r.PlanId).F32("crownRadiusBeforeM", r.CrownRadiusBeforeM).F32("crownRadiusAfterM", r.CrownRadiusAfterM)
                .Int("liftsAfter", r.LiftsAfter).EndObject();
        w.EndArray().BeginArray("deadwood");
        foreach (var d in s.Deadwood)
            w.BeginObject().Str("deadwoodId", d.DeadwoodId).Str("treeId", d.TreeId).Str("areaId", d.AreaId).Str("speciesId", d.SpeciesId)
                .F32("xM", d.XM).F32("zM", d.ZM).Int("deathYear", d.DeathYear).Str("cause", d.Cause).F32("dbhCm", d.DbhCm)
                .F32("heightM", d.HeightM).F32("estimatedVolumeM3", d.EstimatedVolumeM3).EndObject();
        w.EndArray().Int("nextDeadwoodNumber", s.NextDeadwoodNumber).EndObject();
    }

    private static void WritePrunes(Phase1JsonWriter w, string key, IReadOnlyList<Phase1PruneOrder> list)
    {
        w.BeginArray(key);
        foreach (var p in list)
            w.BeginObject().Str("areaId", p.AreaId).Str("treeId", p.TreeId).F32("targetCrownBaseHeightM", p.TargetCrownBaseHeightM).EndObject();
        w.EndArray();
    }

    private static void WriteStage5(Phase1JsonWriter w, Phase1Stage5State s)
    {
        w.BeginObject().Int("mastSeed", s.MastSeed).BeginArray("areas");
        foreach (var a in s.Areas)
        {
            w.BeginObject().Str("areaId", a.AreaId).BeginArray("cells");
            foreach (var c in a.Cells)
            {
                w.BeginObject().Int("column", c.Column).Int("row", c.Row).F32("centreXM", c.CentreXM).F32("centreZM", c.CentreZM)
                    .F64("clippedAreaM2", c.ClippedAreaM2).F32("light", c.Light).F32("bramble", c.Bramble).F32("bracken", c.Bracken)
                    .BeginArray("bands");
                foreach (var b in c.Bands)
                    w.BeginObject().F32("density", b.Density).F32("height", b.Height).Int("establishYear", b.EstablishYear)
                        .Int("originYear", b.OriginYear).F32("lastBrowsedFraction", b.LastBrowsedFraction)
                        .Int("lastBrowseAssessmentYear", b.LastBrowseAssessmentYear).EndObject();
                w.EndArray().EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray();
        WriteClears(w, "pendingClears", s.PendingClears);
        WriteClears(w, "approvedClears", s.ApprovedClears);
        w.BeginArray("clearLog");
        foreach (var r in s.ClearLog)
            w.BeginObject().Str("areaId", r.AreaId).Int("column", r.Column).Int("row", r.Row).Int("ecologicalYear", r.EcologicalYear)
                .Int("workRevision", r.WorkRevision).Str("executionKey", r.ExecutionKey).Str("planId", r.PlanId)
                .F32("brambleBefore", r.BrambleBefore).F32("brackenBefore", r.BrackenBefore).Int("bandsKept", r.BandsKept)
                .F64("abundanceKept", r.AbundanceKept).EndObject();
        w.EndArray().BeginArray("accounts");
        foreach (var y in s.Accounts)
        {
            w.BeginObject().Int("yearBefore", y.YearBefore).Int("yearAfter", y.YearAfter).F64("mastRoll", y.MastRoll)
                .Str("mastLabel", y.MastLabel).F32("mastMultiplier", y.MastMultiplier).BeginArray("areas");
            foreach (var a in y.Areas)
            {
                w.BeginObject().Str("areaId", a.AreaId).Int("cells", a.Cells).F64("seedArrival", a.SeedArrival)
                    .F64("startingAbundance", a.StartingAbundance).F64("vegetationLoss", a.VegetationLoss).F64("lightLoss", a.LightLoss)
                    .F64("browseLoss", a.BrowseLoss).F64("establishmentRequested", a.EstablishmentRequested)
                    .F64("establishmentAccepted", a.EstablishmentAccepted).F64("capacityRejected", a.CapacityRejected)
                    .Int("bandsCreated", a.BandsCreated).Int("bandMerges", a.BandMerges).Int("thresholdCrossings", a.ThresholdCrossings)
                    .Int("qualifyingCells", a.QualifyingCells).Int("blockCapDeferredCells", a.BlockCapDeferredCells)
                    .F64("promotionExported", a.PromotionExported).F64("endingAbundance", a.EndingAbundance)
                    .F64("meanRelativeOccupancy", a.MeanRelativeOccupancy).F64("meanBramble", a.MeanBramble)
                    .F64("meanBracken", a.MeanBracken).F64("meanLight", a.MeanLight);
                WritePromotions(w, a.Promotions);
                w.Int("cohortBandsExported", a.CohortBandsExported).Int("budgetWithheldBlocks", a.BudgetWithheldBlocks)
                    .F32Array("cellEstablishmentAccepted", a.CellEstablishmentAccepted).EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray().BeginArray("blocks");
        foreach (var b in s.Blocks)
            w.BeginObject().Int("blockX", b.BlockX).Int("blockZ", b.BlockZ).Int("inPolygonCells", b.InPolygonCells).Int("credit", b.Credit)
                .EndObject();
        w.EndArray().EndObject();
    }

    private static void WriteClears(Phase1JsonWriter w, string key, IReadOnlyList<Phase1ClearOrder> list)
    {
        w.BeginArray(key);
        foreach (var c in list) w.BeginObject().Str("areaId", c.AreaId).Int("column", c.Column).Int("row", c.Row).EndObject();
        w.EndArray();
    }

    private static void WritePromotions(Phase1JsonWriter w, IReadOnlyList<Phase1PromotionRecord> list)
    {
        w.BeginArray("promotions");
        foreach (var p in list)
            w.BeginObject().Str("treeId", p.TreeId).Str("areaId", p.AreaId).Int("column", p.Column).Int("row", p.Row)
                .Int("blockX", p.BlockX).Int("blockZ", p.BlockZ).Int("establishYear", p.EstablishYear).Int("ageYears", p.AgeYears)
                .F32("heightM", p.HeightM).F32("dbhCm", p.DbhCm).F32("crownRadiusM", p.CrownRadiusM).F32("woodlandCi", p.WoodlandCi)
                .F32("xM", p.XM).F32("zM", p.ZM).Bool("centreFallback", p.CentreFallback).F32("exportedDensity", p.ExportedDensity)
                .Int("cohortBandsExported", p.CohortBandsExported).F64("cohortDensityExported", p.CohortDensityExported).EndObject();
        w.EndArray();
    }

    private static void WritePhase2(Phase1JsonWriter w, Phase1Phase2State s)
    {
        w.BeginObject().Int("oakMastSeed", s.OakMastSeed).Int("beechMastSeed", s.BeechMastSeed);
        WriteSpecies(w, "treeSpecies", s.TreeSpecies);
        w.BeginArray("areas");
        foreach (var a in s.Areas)
        {
            w.BeginObject().Str("areaId", a.AreaId).BeginArray("cells");
            foreach (var c in a.Cells)
            {
                w.BeginObject().Int("column", c.Column).Int("row", c.Row).BeginArray("bands");
                foreach (var b in c.Bands)
                    w.BeginObject().Str("speciesId", b.SpeciesId).Str("origin", b.Origin).F32("density", b.Density).F32("height", b.Height)
                        .Int("establishYear", b.EstablishYear).Int("originYear", b.OriginYear)
                        .F32("lastBrowsedFraction", b.LastBrowsedFraction).Int("lastBrowseAssessmentYear", b.LastBrowseAssessmentYear)
                        .EndObject();
                w.EndArray().EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray().BeginArray("juveniles");
        foreach (var j in s.Juveniles)
            w.BeginObject().Str("juvenileId", j.JuvenileId).Str("areaId", j.AreaId).Str("speciesId", j.SpeciesId).F32("xM", j.XM)
                .F32("zM", j.ZM).Int("plantingYear", j.PlantingYear).F32("ageYears", j.AgeYears).F32("heightM", j.HeightM)
                .Bool("alive", j.Alive).StrOrNull("promotedTreeId", j.PromotedTreeId).Int("deathYear", j.DeathYear)
                .StrOrNull("deathCause", j.DeathCause).Bool("lastYearBrowsed", j.LastYearBrowsed)
                .F32("lastBrowseProbability", j.LastBrowseProbability).F32("lastVegetationLossProbability", j.LastVegetationLossProbability)
                .Int("lastAssessmentYear", j.LastAssessmentYear).EndObject();
        w.EndArray().BeginArray("shelters");
        foreach (var h in s.Shelters)
            w.BeginObject().Str("shelterId", h.ShelterId).Str("juvenileId", h.JuvenileId).F32("xM", h.XM).F32("zM", h.ZM)
                .Int("installedYear", h.InstalledYear).Int("effectiveYears", h.EffectiveYears).Int("failedYear", h.FailedYear).EndObject();
        w.EndArray().BeginArray("patches");
        foreach (var p in s.Patches)
            w.BeginObject().Str("patchId", p.PatchId).Str("areaId", p.AreaId).F32("xM", p.XM).F32("zM", p.ZM).F32("radiusM", p.RadiusM)
                .Int("createdYear", p.CreatedYear).Int("updatedYear", p.UpdatedYear).F32("bramble", p.Bramble).F32("bracken", p.Bracken)
                .EndObject();
        w.EndArray();
        WritePlanting(w, "pendingOrders", s.PendingOrders);
        WritePlanting(w, "approvedOrders", s.ApprovedOrders);
        w.BeginArray("workLog");
        foreach (var r in s.WorkLog)
            w.BeginObject().Str("orderId", r.OrderId).Str("kind", r.Kind).Str("areaId", r.AreaId).F32("xM", r.XM).F32("zM", r.ZM)
                .StrOrNull("speciesId", r.SpeciesId).StrOrNull("juvenileId", r.JuvenileId).StrOrNull("shelterId", r.ShelterId)
                .StrOrNull("patchId", r.PatchId).Int("ecologicalYear", r.EcologicalYear).Int("workRevision", r.WorkRevision)
                .Str("executionKey", r.ExecutionKey).Str("planId", r.PlanId).EndObject();
        w.EndArray().Int("nextOrderNumber", s.NextOrderNumber).Int("nextJuvenileNumber", s.NextJuvenileNumber)
            .Int("nextPatchNumber", s.NextPatchNumber).BeginArray("accounts");
        foreach (var y in s.Accounts)
        {
            w.BeginObject().Int("yearBefore", y.YearBefore).Int("yearAfter", y.YearAfter).BeginArray("mast");
            foreach (var m in y.Mast)
                w.BeginObject().Str("speciesId", m.SpeciesId).Int("mastSeed", m.MastSeed).F64("roll", m.Roll).Str("label", m.Label)
                    .F32("multiplier", m.Multiplier).EndObject();
            w.EndArray().BeginArray("natural");
            foreach (var a in y.Natural)
            {
                w.BeginObject().Str("areaId", a.AreaId).Str("speciesId", a.SpeciesId).F64("seedArrival", a.SeedArrival)
                    .F64("startingAbundance", a.StartingAbundance).F64("vegetationLoss", a.VegetationLoss).F64("lightLoss", a.LightLoss)
                    .F64("browseLoss", a.BrowseLoss).F64("establishmentRequested", a.EstablishmentRequested)
                    .F64("establishmentAccepted", a.EstablishmentAccepted).F64("capacityRejected", a.CapacityRejected)
                    .Int("bandsCreated", a.BandsCreated).Int("bandMerges", a.BandMerges).Int("thresholdCrossings", a.ThresholdCrossings)
                    .Int("qualifyingCells", a.QualifyingCells).Int("blockCapDeferredCells", a.BlockCapDeferredCells)
                    .F64("promotionExported", a.PromotionExported).F64("endingAbundance", a.EndingAbundance)
                    .Int("cohortBandsExported", a.CohortBandsExported).Int("budgetWithheldBlocks", a.BudgetWithheldBlocks);
                WritePromotions(w, a.Promotions);
                w.EndObject();
            }
            w.EndArray().BeginArray("planted");
            foreach (var p in y.Planted)
                w.BeginObject().Str("speciesId", p.SpeciesId).Int("starting", p.Starting).Int("vegetationDeaths", p.VegetationDeaths)
                    .Int("lightDeaths", p.LightDeaths).Int("browseDeaths", p.BrowseDeaths).Int("remaining", p.Remaining)
                    .Int("promoted", p.Promoted).StrArray("promotedTreeIds", p.PromotedTreeIds).EndObject();
            w.EndArray().BeginArray("density");
            foreach (var d in y.Density)
                w.BeginObject().Str("areaId", d.AreaId).Bool("mixed", d.Mixed).Int("stems", d.Stems).Int("sitkaSubset", d.SitkaSubset)
                    .Int("sitkaHazardDeaths", d.SitkaHazardDeaths).Int("sitkaBoundaryDeaths", d.SitkaBoundaryDeaths)
                    .F64("occupancy", d.Occupancy).F64("genericResponse", d.GenericResponse).F64("nonReferenceShare", d.NonReferenceShare)
                    .Int("eligibleForGeneric", d.EligibleForGeneric).F64("expectedGeneric", d.ExpectedGeneric)
                    .Int("genericQuota", d.GenericQuota).F64("quotaRemainder", d.QuotaRemainder).F64OrNull("quotaRoll", d.QuotaRoll)
                    .Int("genericDeaths", d.GenericDeaths).Int("budgetPassedOver", d.BudgetPassedOver)
                    .Bool("singleStemFallback", d.SingleStemFallback).F64("standBasalAreaM2", d.StandBasalAreaM2)
                    .F64("genericBasalAreaM2", d.GenericBasalAreaM2).StrArray("genericVictims", d.GenericVictims).EndObject();
            w.EndArray().EndObject();
        }
        w.EndArray().EndObject();
    }

    private static void WritePlanting(Phase1JsonWriter w, string key, IReadOnlyList<Phase1PlantingOrder> list)
    {
        w.BeginArray(key);
        foreach (var o in list)
        {
            switch (o)
            {
                case Phase1PlantOrder p:
                    w.BeginObject().Str("type", Phase1SaveFormat.PlantingTags.Plant).Str("orderId", p.OrderId).Str("areaId", p.AreaId)
                        .Str("speciesId", p.SpeciesId).F32("xM", p.XM).F32("zM", p.ZM).EndObject();
                    break;
                case Phase1ShelterOrder h:
                    w.BeginObject().Str("type", Phase1SaveFormat.PlantingTags.Shelter).Str("orderId", h.OrderId).Str("areaId", h.AreaId)
                        .F32("xM", h.XM).F32("zM", h.ZM).EndObject();
                    break;
                case Phase1SpotClearOrder c:
                    w.BeginObject().Str("type", Phase1SaveFormat.PlantingTags.SpotClear).Str("orderId", c.OrderId).Str("areaId", c.AreaId)
                        .F32("xM", c.XM).F32("zM", c.ZM).EndObject();
                    break;
                default:
                    throw new P1Refusal("SaveEnumUnknown", "unknown planting order type");
            }
        }
        w.EndArray();
    }

    // ------------------------------------------------------------------------------------------------ payload reader

    private static Phase1SaveState ReadState(JNode n)
    {
        var o = J.Keys(n, "payload", "options", "ecologicalYear", "annualRevision", "workRevision", "planStateRevision", "areas",
            "events", "executed", "pending", "approved", "annualHistory", "stage4", "stage5", "phase2");
        var areas = R.List(o["areas"], "areas", x =>
        {
            var a = J.Keys(x, "area", "areaId", "ecologicalYear", "trees", "cropTreeIds");
            var trees = R.List(a["trees"], "trees", t =>
            {
                var r = J.Keys(t, "tree", "treeId", "xM", "zM", "ageYears", "dbhCm", "heightM", "crownRadiusM", "death");
                return new Phase1Tree(J.Str(r["treeId"], "treeId"), R.F32(r["xM"], "xM"), R.F32(r["zM"], "zM"),
                    R.I32(r["ageYears"], "ageYears"), R.F32(r["dbhCm"], "dbhCm"), R.F32(r["heightM"], "heightM"),
                    R.F32(r["crownRadiusM"], "crownRadiusM"), r["death"] is JNull ? null : ReadDeath(r["death"]));
            });
            return new Phase1AreaState(J.Str(a["areaId"], "areaId"), R.I32(a["ecologicalYear"], "ecologicalYear"), trees,
                R.Strs(a["cropTreeIds"], "cropTreeIds"));
        });
        var events = R.List(o["events"], "events", x =>
        {
            var e = J.Keys(x, "event", "executionKey", "planId", "ecologicalYear", "workRevision", "areas");
            var changes = R.List(e["areas"], "event.areas", c =>
            {
                var a = J.Keys(c, "change", "areaId", "before", "after", "fells");
                var fells = R.List(a["fells"], "fells", f =>
                {
                    var r = J.Keys(f, "fell", "areaId", "treeId", "cause", "mechanism", "reason", "ecologicalYear", "workRevision",
                        "executionKey", "planId");
                    return new Phase1FellRecord(J.Str(r["areaId"], "areaId"), J.Str(r["treeId"], "treeId"), J.Str(r["cause"], "cause"),
                        J.Str(r["mechanism"], "mechanism"), R.Text(r["reason"], "reason"), R.I32(r["ecologicalYear"], "ecologicalYear"),
                        R.I32(r["workRevision"], "workRevision"), J.Str(r["executionKey"], "executionKey"), J.Str(r["planId"], "planId"));
                });
                return new Phase1AreaChange(J.Str(a["areaId"], "areaId"), ReadStand(a["before"]), ReadStand(a["after"]), fells);
            });
            return new Phase1WorkEvent(J.Str(e["executionKey"], "executionKey"), J.Str(e["planId"], "planId"),
                R.I32(e["ecologicalYear"], "ecologicalYear"), R.I32(e["workRevision"], "workRevision"), changes);
        });
        var executed = R.List(o["executed"], "executed", x =>
        {
            var e = J.Keys(x, "executed", "executionKey", "recordedSource", "orders");
            return new Phase1ExecutedWork(J.Str(e["executionKey"], "executionKey"), ReadToken(e["recordedSource"]),
                ReadOrders(e["orders"], "executed.orders"));
        });
        Phase1ApprovedPlan? approved = null;
        if (o["approved"] is not JNull)
        {
            var a = J.Keys(o["approved"], "approved", "planId", "executionKey", "recordedSource", "orders");
            approved = new Phase1ApprovedPlan(J.Str(a["planId"], "planId"), J.Str(a["executionKey"], "executionKey"),
                ReadToken(a["recordedSource"]), ReadOrders(a["orders"], "approved.orders"));
        }
        var history = R.List(o["annualHistory"], "annualHistory", x =>
        {
            var y = J.Keys(x, "year", "yearBefore", "yearAfter", "annualRevisionAfter", "workRevision", "areas", "cancelledUnexecutedWork");
            var yearAreas = R.List(y["areas"], "year.areas", c =>
            {
                var a = J.Keys(c, "areaYear", "areaId", "before", "after", "naturalDeaths", "noLivingStems");
                return new Phase1AreaYear(J.Str(a["areaId"], "areaId"), ReadStand(a["before"]), ReadStand(a["after"]),
                    R.List(a["naturalDeaths"], "naturalDeaths", ReadDeath), R.Bool(a["noLivingStems"], "noLivingStems"));
            });
            return new Phase1YearRecord(R.I32(y["yearBefore"], "yearBefore"), R.I32(y["yearAfter"], "yearAfter"),
                R.I32(y["annualRevisionAfter"], "annualRevisionAfter"), R.I32(y["workRevision"], "workRevision"), yearAreas,
                R.Bool(y["cancelledUnexecutedWork"], "cancelledUnexecutedWork"));
        });
        return new Phase1SaveState(ReadOptions(o["options"]), R.I32(o["ecologicalYear"], "ecologicalYear"),
            R.I32(o["annualRevision"], "annualRevision"), R.I32(o["workRevision"], "workRevision"),
            R.I32(o["planStateRevision"], "planStateRevision"), areas, events, executed, ReadOrders(o["pending"], "pending"), approved,
            history, o["stage4"] is JNull ? null : ReadStage4(o["stage4"]), o["stage5"] is JNull ? null : ReadStage5(o["stage5"]),
            o["phase2"] is JNull ? null : ReadPhase2(o["phase2"]));
    }

    private static Phase1Options ReadOptions(JNode n)
    {
        var o = J.Keys(n, "options", "stage4", "stage5", "phase2");
        Phase1Stage4Options? s4 = null;
        if (o["stage4"] is not JNull)
        {
            var a = J.Keys(o["stage4"], "options.stage4", "siteProductivity", "growthAgePhases");
            var phases = a["growthAgePhases"] is JNull ? null : R.List(a["growthAgePhases"], "growthAgePhases", x =>
            {
                var p = J.Keys(x, "growthAgePhase", "startAgeYears", "dbhIncrementFactor", "heightIncrementFactor");
                return new Phase1GrowthAgePhase(R.F32(p["startAgeYears"], "startAgeYears"), R.F32(p["dbhIncrementFactor"], "dbhIncrementFactor"),
                    R.F32(p["heightIncrementFactor"], "heightIncrementFactor"));
            });
            s4 = new Phase1Stage4Options(J.Str(a["siteProductivity"], "siteProductivity"), phases);
        }
        Phase1Stage5Options? s5 = null;
        if (o["stage5"] is not JNull)
        {
            var a = J.Keys(o["stage5"], "options.stage5", "gridCellM", "backgroundBrowsePressure", "mastSeed");
            s5 = new Phase1Stage5Options(R.F64(a["gridCellM"], "gridCellM"), R.F32(a["backgroundBrowsePressure"], "backgroundBrowsePressure"),
                R.I32OrNull(a["mastSeed"], "mastSeed"));
        }
        Phase1Phase2Options? p2 = null;
        if (o["phase2"] is not JNull)
        {
            var a = J.Keys(o["phase2"], "options.phase2", "oakMastSeed", "beechMastSeed", "genesisSpecies");
            p2 = new Phase1Phase2Options(R.I32OrNull(a["oakMastSeed"], "oakMastSeed"), R.I32OrNull(a["beechMastSeed"], "beechMastSeed"),
                ReadSpecies(a["genesisSpecies"], "genesisSpecies"));
        }
        return new Phase1Options(s4, s5, p2);
    }

    private static IReadOnlyList<Phase1TreeSpecies> ReadSpecies(JNode n, string what) => R.List(n, what, x =>
    {
        var e = J.Keys(x, what, "treeId", "speciesId");
        return new Phase1TreeSpecies(J.Str(e["treeId"], "treeId"), J.Str(e["speciesId"], "speciesId"));
    });

    private static Phase1Death ReadDeath(JNode n)
    {
        var d = J.Keys(n, "death", "treeId", "ecologicalYear", "cause", "mechanism");
        return new Phase1Death(J.Str(d["treeId"], "death.treeId"), R.I32(d["ecologicalYear"], "death.ecologicalYear"),
            J.Str(d["cause"], "death.cause"), J.Str(d["mechanism"], "death.mechanism"));
    }

    private static Phase1Stand ReadStand(JNode n)
    {
        var s = J.Keys(n, "stand", "livingCount", "stemsPerHa", "basalAreaM2PerHa", "quadraticMeanDbhCm", "relativeDensity");
        return new Phase1Stand(R.I32(s["livingCount"], "livingCount"), R.F64(s["stemsPerHa"], "stemsPerHa"),
            R.F64(s["basalAreaM2PerHa"], "basalAreaM2PerHa"), R.F64(s["quadraticMeanDbhCm"], "quadraticMeanDbhCm"),
            R.F64(s["relativeDensity"], "relativeDensity"));
    }

    private static Phase1SourceToken ReadToken(JNode n)
    {
        var t = J.Keys(n, "recordedSource", "domain", "experimentId", "manifestContentHash", "areaIds", "ecologicalYear", "annualRevision",
            "workRevision", "rngModelVersion", "growthModelVersion", "p1AlgorithmVersion", "areaModelVersion", "actionEligibleDbhThresholdMm");
        return new Phase1SourceToken(J.Str(t["domain"], "domain"), J.Str(t["experimentId"], "experimentId"),
            J.Str(t["manifestContentHash"], "manifestContentHash"), J.Str(t["areaIds"], "areaIds"), R.I32(t["ecologicalYear"], "ecologicalYear"),
            R.I32(t["annualRevision"], "annualRevision"), R.I32(t["workRevision"], "workRevision"),
            R.I32(t["rngModelVersion"], "rngModelVersion"), R.I32(t["growthModelVersion"], "growthModelVersion"),
            J.Str(t["p1AlgorithmVersion"], "p1AlgorithmVersion"), J.Str(t["areaModelVersion"], "areaModelVersion"),
            R.I64(t["actionEligibleDbhThresholdMm"], "actionEligibleDbhThresholdMm"));
    }

    private static IReadOnlyList<Phase1WorkOrder> ReadOrders(JNode n, string what) => R.List(n, what, x =>
    {
        var o = J.Keys(x, "workOrder", "areaId", "treeId", "action", "reason");
        string action = J.Str(o["action"], "action");
        var parsed = action switch
        {
            "Fell" => WorkAction.Fell,
            _ => throw new P1Refusal("SaveEnumUnknown", "unknown work action '" + action + "'"),
        };
        return new Phase1WorkOrder(J.Str(o["areaId"], "areaId"), J.Str(o["treeId"], "treeId"), parsed, R.Text(o["reason"], "reason"));
    });

    private static Phase1Stage4State ReadStage4(JNode n)
    {
        var s = J.Keys(n, "stage4", "pruning", "pendingPrunes", "approvedPrunes", "pruneLog", "deadwood", "nextDeadwoodNumber");
        var pruning = R.List(s["pruning"], "pruning", x =>
        {
            var p = J.Keys(x, "pruning", "treeId", "lifts", "crownBaseHeightM", "lastPruningYear");
            return new Phase1TreePruning(J.Str(p["treeId"], "treeId"), R.I32(p["lifts"], "lifts"), R.F32(p["crownBaseHeightM"], "crownBaseHeightM"),
                R.I32(p["lastPruningYear"], "lastPruningYear"));
        });
        var log = R.List(s["pruneLog"], "pruneLog", x =>
        {
            var r = J.Keys(x, "pruneRecord", "areaId", "treeId", "targetCrownBaseHeightM", "ecologicalYear", "workRevision", "executionKey",
                "planId", "crownRadiusBeforeM", "crownRadiusAfterM", "liftsAfter");
            return new Phase1PruneRecord(J.Str(r["areaId"], "areaId"), J.Str(r["treeId"], "treeId"),
                R.F32(r["targetCrownBaseHeightM"], "targetCrownBaseHeightM"), R.I32(r["ecologicalYear"], "ecologicalYear"),
                R.I32(r["workRevision"], "workRevision"), J.Str(r["executionKey"], "executionKey"), J.Str(r["planId"], "planId"),
                R.F32(r["crownRadiusBeforeM"], "crownRadiusBeforeM"), R.F32(r["crownRadiusAfterM"], "crownRadiusAfterM"),
                R.I32(r["liftsAfter"], "liftsAfter"));
        });
        var deadwood = R.List(s["deadwood"], "deadwood", x =>
        {
            var d = J.Keys(x, "deadwood", "deadwoodId", "treeId", "areaId", "speciesId", "xM", "zM", "deathYear", "cause", "dbhCm", "heightM",
                "estimatedVolumeM3");
            return new Phase1Deadwood(J.Str(d["deadwoodId"], "deadwoodId"), J.Str(d["treeId"], "treeId"), J.Str(d["areaId"], "areaId"),
                J.Str(d["speciesId"], "speciesId"), R.F32(d["xM"], "xM"), R.F32(d["zM"], "zM"), R.I32(d["deathYear"], "deathYear"),
                J.Str(d["cause"], "cause"), R.F32(d["dbhCm"], "dbhCm"), R.F32(d["heightM"], "heightM"),
                R.F32(d["estimatedVolumeM3"], "estimatedVolumeM3"));
        });
        return new Phase1Stage4State(pruning, ReadPrunes(s["pendingPrunes"], "pendingPrunes"), ReadPrunes(s["approvedPrunes"], "approvedPrunes"),
            log, deadwood, R.I32(s["nextDeadwoodNumber"], "nextDeadwoodNumber"));
    }

    private static IReadOnlyList<Phase1PruneOrder> ReadPrunes(JNode n, string what) => R.List(n, what, x =>
    {
        var p = J.Keys(x, "pruneOrder", "areaId", "treeId", "targetCrownBaseHeightM");
        return new Phase1PruneOrder(J.Str(p["areaId"], "areaId"), J.Str(p["treeId"], "treeId"),
            R.F32(p["targetCrownBaseHeightM"], "targetCrownBaseHeightM"));
    });

    private static Phase1Stage5State ReadStage5(JNode n)
    {
        var s = J.Keys(n, "stage5", "mastSeed", "areas", "pendingClears", "approvedClears", "clearLog", "accounts", "blocks");
        var areas = R.List(s["areas"], "stage5.areas", x =>
        {
            var a = J.Keys(x, "stage5.area", "areaId", "cells");
            var cells = R.List(a["cells"], "cells", c =>
            {
                var r = J.Keys(c, "cell", "column", "row", "centreXM", "centreZM", "clippedAreaM2", "light", "bramble", "bracken", "bands");
                var bands = R.List(r["bands"], "bands", b =>
                {
                    var d = J.Keys(b, "band", "density", "height", "establishYear", "originYear", "lastBrowsedFraction", "lastBrowseAssessmentYear");
                    return new Phase1Stage5Band(R.F32(d["density"], "density"), R.F32(d["height"], "height"),
                        R.I32(d["establishYear"], "establishYear"), R.I32(d["originYear"], "originYear"),
                        R.F32(d["lastBrowsedFraction"], "lastBrowsedFraction"), R.I32(d["lastBrowseAssessmentYear"], "lastBrowseAssessmentYear"));
                });
                return new Phase1Stage5Cell(R.I32(r["column"], "column"), R.I32(r["row"], "row"), R.F32(r["centreXM"], "centreXM"),
                    R.F32(r["centreZM"], "centreZM"), R.F64(r["clippedAreaM2"], "clippedAreaM2"), R.F32(r["light"], "light"),
                    R.F32(r["bramble"], "bramble"), R.F32(r["bracken"], "bracken"), bands);
            });
            return new Phase1Stage5AreaCells(J.Str(a["areaId"], "areaId"), cells);
        });
        var log = R.List(s["clearLog"], "clearLog", x =>
        {
            var r = J.Keys(x, "clearRecord", "areaId", "column", "row", "ecologicalYear", "workRevision", "executionKey", "planId",
                "brambleBefore", "brackenBefore", "bandsKept", "abundanceKept");
            return new Phase1ClearRecord(J.Str(r["areaId"], "areaId"), R.I32(r["column"], "column"), R.I32(r["row"], "row"),
                R.I32(r["ecologicalYear"], "ecologicalYear"), R.I32(r["workRevision"], "workRevision"), J.Str(r["executionKey"], "executionKey"),
                J.Str(r["planId"], "planId"), R.F32(r["brambleBefore"], "brambleBefore"), R.F32(r["brackenBefore"], "brackenBefore"),
                R.I32(r["bandsKept"], "bandsKept"), R.F64(r["abundanceKept"], "abundanceKept"));
        });
        var accounts = R.List(s["accounts"], "stage5.accounts", x =>
        {
            var y = J.Keys(x, "stage5.year", "yearBefore", "yearAfter", "mastRoll", "mastLabel", "mastMultiplier", "areas");
            var yearAreas = R.List(y["areas"], "stage5.year.areas", c =>
            {
                var a = J.Keys(c, "stage5.account", "areaId", "cells", "seedArrival", "startingAbundance", "vegetationLoss", "lightLoss",
                    "browseLoss", "establishmentRequested", "establishmentAccepted", "capacityRejected", "bandsCreated", "bandMerges",
                    "thresholdCrossings", "qualifyingCells", "blockCapDeferredCells", "promotionExported", "endingAbundance",
                    "meanRelativeOccupancy", "meanBramble", "meanBracken", "meanLight", "promotions", "cohortBandsExported",
                    "budgetWithheldBlocks", "cellEstablishmentAccepted");
                return new Phase1Stage5AreaAccount(J.Str(a["areaId"], "areaId"), R.I32(a["cells"], "cells"),
                    R.F64(a["seedArrival"], "seedArrival"), R.F64(a["startingAbundance"], "startingAbundance"),
                    R.F64(a["vegetationLoss"], "vegetationLoss"), R.F64(a["lightLoss"], "lightLoss"), R.F64(a["browseLoss"], "browseLoss"),
                    R.F64(a["establishmentRequested"], "establishmentRequested"), R.F64(a["establishmentAccepted"], "establishmentAccepted"),
                    R.F64(a["capacityRejected"], "capacityRejected"), R.I32(a["bandsCreated"], "bandsCreated"),
                    R.I32(a["bandMerges"], "bandMerges"), R.I32(a["thresholdCrossings"], "thresholdCrossings"),
                    R.I32(a["qualifyingCells"], "qualifyingCells"), R.I32(a["blockCapDeferredCells"], "blockCapDeferredCells"),
                    R.F64(a["promotionExported"], "promotionExported"), R.F64(a["endingAbundance"], "endingAbundance"),
                    R.F64(a["meanRelativeOccupancy"], "meanRelativeOccupancy"), R.F64(a["meanBramble"], "meanBramble"),
                    R.F64(a["meanBracken"], "meanBracken"), R.F64(a["meanLight"], "meanLight"), ReadPromotions(a["promotions"]),
                    R.I32(a["cohortBandsExported"], "cohortBandsExported"), R.I32(a["budgetWithheldBlocks"], "budgetWithheldBlocks"),
                    R.List(a["cellEstablishmentAccepted"], "cellEstablishmentAccepted", v => R.F32(v, "cellEstablishmentAccepted")));
            });
            return new Phase1Stage5YearAccount(R.I32(y["yearBefore"], "yearBefore"), R.I32(y["yearAfter"], "yearAfter"),
                R.F64(y["mastRoll"], "mastRoll"), J.Str(y["mastLabel"], "mastLabel"), R.F32(y["mastMultiplier"], "mastMultiplier"), yearAreas);
        });
        var blocks = R.List(s["blocks"], "blocks", x =>
        {
            var b = J.Keys(x, "block", "blockX", "blockZ", "inPolygonCells", "credit");
            return new Phase1BlockCredit(R.I32(b["blockX"], "blockX"), R.I32(b["blockZ"], "blockZ"),
                R.I32(b["inPolygonCells"], "inPolygonCells"), R.I32(b["credit"], "credit"));
        });
        return new Phase1Stage5State(R.I32(s["mastSeed"], "mastSeed"), areas, ReadClears(s["pendingClears"], "pendingClears"),
            ReadClears(s["approvedClears"], "approvedClears"), log, accounts, blocks);
    }

    private static IReadOnlyList<Phase1ClearOrder> ReadClears(JNode n, string what) => R.List(n, what, x =>
    {
        var c = J.Keys(x, "clearOrder", "areaId", "column", "row");
        return new Phase1ClearOrder(J.Str(c["areaId"], "areaId"), R.I32(c["column"], "column"), R.I32(c["row"], "row"));
    });

    private static IReadOnlyList<Phase1PromotionRecord> ReadPromotions(JNode n) => R.List(n, "promotions", x =>
    {
        var p = J.Keys(x, "promotion", "treeId", "areaId", "column", "row", "blockX", "blockZ", "establishYear", "ageYears", "heightM",
            "dbhCm", "crownRadiusM", "woodlandCi", "xM", "zM", "centreFallback", "exportedDensity", "cohortBandsExported",
            "cohortDensityExported");
        return new Phase1PromotionRecord(J.Str(p["treeId"], "treeId"), J.Str(p["areaId"], "areaId"), R.I32(p["column"], "column"),
            R.I32(p["row"], "row"), R.I32(p["blockX"], "blockX"), R.I32(p["blockZ"], "blockZ"), R.I32(p["establishYear"], "establishYear"),
            R.I32(p["ageYears"], "ageYears"), R.F32(p["heightM"], "heightM"), R.F32(p["dbhCm"], "dbhCm"),
            R.F32(p["crownRadiusM"], "crownRadiusM"), R.F32(p["woodlandCi"], "woodlandCi"), R.F32(p["xM"], "xM"), R.F32(p["zM"], "zM"),
            R.Bool(p["centreFallback"], "centreFallback"), R.F32(p["exportedDensity"], "exportedDensity"),
            R.I32(p["cohortBandsExported"], "cohortBandsExported"), R.F64(p["cohortDensityExported"], "cohortDensityExported"));
    });

    private static Phase1Phase2State ReadPhase2(JNode n)
    {
        var s = J.Keys(n, "phase2", "oakMastSeed", "beechMastSeed", "treeSpecies", "areas", "juveniles", "shelters", "patches",
            "pendingOrders", "approvedOrders", "workLog", "nextOrderNumber", "nextJuvenileNumber", "nextPatchNumber", "accounts");
        var areas = R.List(s["areas"], "phase2.areas", x =>
        {
            var a = J.Keys(x, "phase2.area", "areaId", "cells");
            var cells = R.List(a["cells"], "phase2.cells", c =>
            {
                var r = J.Keys(c, "phase2.cell", "column", "row", "bands");
                var bands = R.List(r["bands"], "phase2.bands", b =>
                {
                    var d = J.Keys(b, "phase2.band", "speciesId", "origin", "density", "height", "establishYear", "originYear",
                        "lastBrowsedFraction", "lastBrowseAssessmentYear");
                    return new Phase1Phase2Band(J.Str(d["speciesId"], "speciesId"), J.Str(d["origin"], "origin"), R.F32(d["density"], "density"),
                        R.F32(d["height"], "height"), R.I32(d["establishYear"], "establishYear"), R.I32(d["originYear"], "originYear"),
                        R.F32(d["lastBrowsedFraction"], "lastBrowsedFraction"), R.I32(d["lastBrowseAssessmentYear"], "lastBrowseAssessmentYear"));
                });
                return new Phase1Phase2CellBands(R.I32(r["column"], "column"), R.I32(r["row"], "row"), bands);
            });
            return new Phase1Phase2AreaBands(J.Str(a["areaId"], "areaId"), cells);
        });
        var juveniles = R.List(s["juveniles"], "juveniles", x =>
        {
            var j = J.Keys(x, "juvenile", "juvenileId", "areaId", "speciesId", "xM", "zM", "plantingYear", "ageYears", "heightM", "alive",
                "promotedTreeId", "deathYear", "deathCause", "lastYearBrowsed", "lastBrowseProbability", "lastVegetationLossProbability",
                "lastAssessmentYear");
            return new Phase1PlantedJuvenile(J.Str(j["juvenileId"], "juvenileId"), J.Str(j["areaId"], "areaId"),
                J.Str(j["speciesId"], "speciesId"), R.F32(j["xM"], "xM"), R.F32(j["zM"], "zM"), R.I32(j["plantingYear"], "plantingYear"),
                R.F32(j["ageYears"], "ageYears"), R.F32(j["heightM"], "heightM"), R.Bool(j["alive"], "alive"),
                J.StrOrNull(j["promotedTreeId"], "promotedTreeId"), R.I32(j["deathYear"], "deathYear"),
                J.StrOrNull(j["deathCause"], "deathCause"), R.Bool(j["lastYearBrowsed"], "lastYearBrowsed"),
                R.F32(j["lastBrowseProbability"], "lastBrowseProbability"),
                R.F32(j["lastVegetationLossProbability"], "lastVegetationLossProbability"), R.I32(j["lastAssessmentYear"], "lastAssessmentYear"));
        });
        var shelters = R.List(s["shelters"], "shelters", x =>
        {
            var h = J.Keys(x, "shelter", "shelterId", "juvenileId", "xM", "zM", "installedYear", "effectiveYears", "failedYear");
            return new Phase1Shelter(J.Str(h["shelterId"], "shelterId"), J.Str(h["juvenileId"], "juvenileId"), R.F32(h["xM"], "xM"),
                R.F32(h["zM"], "zM"), R.I32(h["installedYear"], "installedYear"), R.I32(h["effectiveYears"], "effectiveYears"),
                R.I32(h["failedYear"], "failedYear"));
        });
        var patches = R.List(s["patches"], "patches", x =>
        {
            var p = J.Keys(x, "patch", "patchId", "areaId", "xM", "zM", "radiusM", "createdYear", "updatedYear", "bramble", "bracken");
            return new Phase1SpotPatch(J.Str(p["patchId"], "patchId"), J.Str(p["areaId"], "areaId"), R.F32(p["xM"], "xM"), R.F32(p["zM"], "zM"),
                R.F32(p["radiusM"], "radiusM"), R.I32(p["createdYear"], "createdYear"), R.I32(p["updatedYear"], "updatedYear"),
                R.F32(p["bramble"], "bramble"), R.F32(p["bracken"], "bracken"));
        });
        var log = R.List(s["workLog"], "workLog", x =>
        {
            var r = J.Keys(x, "phase2.workRecord", "orderId", "kind", "areaId", "xM", "zM", "speciesId", "juvenileId", "shelterId", "patchId",
                "ecologicalYear", "workRevision", "executionKey", "planId");
            return new Phase1Phase2WorkRecord(J.Str(r["orderId"], "orderId"), J.Str(r["kind"], "kind"), J.Str(r["areaId"], "areaId"),
                R.F32(r["xM"], "xM"), R.F32(r["zM"], "zM"), J.StrOrNull(r["speciesId"], "speciesId"), J.StrOrNull(r["juvenileId"], "juvenileId"),
                J.StrOrNull(r["shelterId"], "shelterId"), J.StrOrNull(r["patchId"], "patchId"), R.I32(r["ecologicalYear"], "ecologicalYear"),
                R.I32(r["workRevision"], "workRevision"), J.Str(r["executionKey"], "executionKey"), J.Str(r["planId"], "planId"));
        });
        var accounts = R.List(s["accounts"], "phase2.accounts", x =>
        {
            var y = J.Keys(x, "phase2.year", "yearBefore", "yearAfter", "mast", "natural", "planted", "density");
            var mast = R.List(y["mast"], "mast", m =>
            {
                var r = J.Keys(m, "mast", "speciesId", "mastSeed", "roll", "label", "multiplier");
                return new Phase1MastRecord(J.Str(r["speciesId"], "speciesId"), R.I32(r["mastSeed"], "mastSeed"), R.F64(r["roll"], "roll"),
                    J.Str(r["label"], "label"), R.F32(r["multiplier"], "multiplier"));
            });
            var natural = R.List(y["natural"], "natural", c =>
            {
                var a = J.Keys(c, "phase2.natural", "areaId", "speciesId", "seedArrival", "startingAbundance", "vegetationLoss", "lightLoss",
                    "browseLoss", "establishmentRequested", "establishmentAccepted", "capacityRejected", "bandsCreated", "bandMerges",
                    "thresholdCrossings", "qualifyingCells", "blockCapDeferredCells", "promotionExported", "endingAbundance",
                    "cohortBandsExported", "budgetWithheldBlocks", "promotions");
                return new Phase1SpeciesAreaAccount(J.Str(a["areaId"], "areaId"), J.Str(a["speciesId"], "speciesId"),
                    R.F64(a["seedArrival"], "seedArrival"), R.F64(a["startingAbundance"], "startingAbundance"),
                    R.F64(a["vegetationLoss"], "vegetationLoss"), R.F64(a["lightLoss"], "lightLoss"), R.F64(a["browseLoss"], "browseLoss"),
                    R.F64(a["establishmentRequested"], "establishmentRequested"), R.F64(a["establishmentAccepted"], "establishmentAccepted"),
                    R.F64(a["capacityRejected"], "capacityRejected"), R.I32(a["bandsCreated"], "bandsCreated"),
                    R.I32(a["bandMerges"], "bandMerges"), R.I32(a["thresholdCrossings"], "thresholdCrossings"),
                    R.I32(a["qualifyingCells"], "qualifyingCells"), R.I32(a["blockCapDeferredCells"], "blockCapDeferredCells"),
                    R.F64(a["promotionExported"], "promotionExported"), R.F64(a["endingAbundance"], "endingAbundance"),
                    R.I32(a["cohortBandsExported"], "cohortBandsExported"), R.I32(a["budgetWithheldBlocks"], "budgetWithheldBlocks"),
                    ReadPromotions(a["promotions"]));
            });
            var planted = R.List(y["planted"], "planted", c =>
            {
                var p = J.Keys(c, "phase2.planted", "speciesId", "starting", "vegetationDeaths", "lightDeaths", "browseDeaths", "remaining",
                    "promoted", "promotedTreeIds");
                return new Phase1PlantedAccount(J.Str(p["speciesId"], "speciesId"), R.I32(p["starting"], "starting"),
                    R.I32(p["vegetationDeaths"], "vegetationDeaths"), R.I32(p["lightDeaths"], "lightDeaths"),
                    R.I32(p["browseDeaths"], "browseDeaths"), R.I32(p["remaining"], "remaining"), R.I32(p["promoted"], "promoted"),
                    R.Strs(p["promotedTreeIds"], "promotedTreeIds"));
            });
            var density = R.List(y["density"], "density", c =>
            {
                var d = J.Keys(c, "phase2.density", "areaId", "mixed", "stems", "sitkaSubset", "sitkaHazardDeaths", "sitkaBoundaryDeaths",
                    "occupancy", "genericResponse", "nonReferenceShare", "eligibleForGeneric", "expectedGeneric", "genericQuota",
                    "quotaRemainder", "quotaRoll", "genericDeaths", "budgetPassedOver", "singleStemFallback", "standBasalAreaM2",
                    "genericBasalAreaM2", "genericVictims");
                return new Phase1DensityRecord(J.Str(d["areaId"], "areaId"), R.Bool(d["mixed"], "mixed"), R.I32(d["stems"], "stems"),
                    R.I32(d["sitkaSubset"], "sitkaSubset"), R.I32(d["sitkaHazardDeaths"], "sitkaHazardDeaths"),
                    R.I32(d["sitkaBoundaryDeaths"], "sitkaBoundaryDeaths"), R.F64(d["occupancy"], "occupancy"),
                    R.F64(d["genericResponse"], "genericResponse"), R.F64(d["nonReferenceShare"], "nonReferenceShare"),
                    R.I32(d["eligibleForGeneric"], "eligibleForGeneric"), R.F64(d["expectedGeneric"], "expectedGeneric"),
                    R.I32(d["genericQuota"], "genericQuota"), R.F64(d["quotaRemainder"], "quotaRemainder"),
                    d["quotaRoll"] is JNull ? null : R.F64(d["quotaRoll"], "quotaRoll"), R.I32(d["genericDeaths"], "genericDeaths"),
                    R.I32(d["budgetPassedOver"], "budgetPassedOver"), R.Bool(d["singleStemFallback"], "singleStemFallback"),
                    R.F64(d["standBasalAreaM2"], "standBasalAreaM2"), R.F64(d["genericBasalAreaM2"], "genericBasalAreaM2"),
                    R.Strs(d["genericVictims"], "genericVictims"));
            });
            return new Phase1Phase2YearAccount(R.I32(y["yearBefore"], "yearBefore"), R.I32(y["yearAfter"], "yearAfter"), mast, natural,
                planted, density);
        });
        return new Phase1Phase2State(R.I32(s["oakMastSeed"], "oakMastSeed"), R.I32(s["beechMastSeed"], "beechMastSeed"),
            ReadSpecies(s["treeSpecies"], "treeSpecies"), areas, juveniles, shelters, patches,
            ReadPlanting(s["pendingOrders"], "pendingOrders"), ReadPlanting(s["approvedOrders"], "approvedOrders"), log,
            R.I32(s["nextOrderNumber"], "nextOrderNumber"), R.I32(s["nextJuvenileNumber"], "nextJuvenileNumber"),
            R.I32(s["nextPatchNumber"], "nextPatchNumber"), accounts);
    }

    private static IReadOnlyList<Phase1PlantingOrder> ReadPlanting(JNode n, string what) => R.List<Phase1PlantingOrder>(n, what, x =>
    {
        // The type tag is read first so an unknown kind is an explicit enum refusal, not a key-set error.
        if (x is not JObj obj) throw new P1Refusal("SchemaInvalid", what + " entry must be an object");
        var tag = obj.Props.FirstOrDefault(kv => kv.Key == "type").Value;
        string type = tag is JStr t ? t.Value : throw new P1Refusal("MissingKey", what + ".type is missing");
        switch (type)
        {
            case Phase1SaveFormat.PlantingTags.Plant:
            {
                var o = J.Keys(x, "plantOrder", "type", "orderId", "areaId", "speciesId", "xM", "zM");
                return new Phase1PlantOrder(J.Str(o["orderId"], "orderId"), J.Str(o["areaId"], "areaId"), J.Str(o["speciesId"], "speciesId"),
                    R.F32(o["xM"], "xM"), R.F32(o["zM"], "zM"));
            }
            case Phase1SaveFormat.PlantingTags.Shelter:
            {
                var o = J.Keys(x, "shelterOrder", "type", "orderId", "areaId", "xM", "zM");
                return new Phase1ShelterOrder(J.Str(o["orderId"], "orderId"), J.Str(o["areaId"], "areaId"), R.F32(o["xM"], "xM"),
                    R.F32(o["zM"], "zM"));
            }
            case Phase1SaveFormat.PlantingTags.SpotClear:
            {
                var o = J.Keys(x, "spotClearOrder", "type", "orderId", "areaId", "xM", "zM");
                return new Phase1SpotClearOrder(J.Str(o["orderId"], "orderId"), J.Str(o["areaId"], "areaId"), R.F32(o["xM"], "xM"),
                    R.F32(o["zM"], "zM"));
            }
            default:
                throw new P1Refusal("SaveEnumUnknown", "unknown planting order type '" + type + "'");
        }
    });

    private static bool IsLowerHex(string s, int length) =>
        s.Length == length && s.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f');

    // ------------------------------------------------------------------------------------------------ typed readers

    /// <summary>Exact-type value readers over <see cref="StrictJson"/> nodes. Every one refuses rather than coerces.</summary>
    private static class R
    {
        public static int I32(JNode n, string what)
        {
            long v = J.Int(n, what);
            if (v < int.MinValue || v > int.MaxValue) throw new P1Refusal("NumberOutOfRange", what + " is outside the 32-bit integer range");
            return (int)v;
        }

        public static int? I32OrNull(JNode n, string what) => n is JNull ? null : I32(n, what);

        public static bool Bool(JNode n, string what) =>
            n is JBool b ? b.Value : throw new P1Refusal("SchemaInvalid", what + " must be true or false");

        public static float F32(JNode n, string what) => Core.F32.FromBits(J.F32Bits(n, what));

        public static double F64(JNode n, string what)
        {
            string hex = Tagged(n, what, Phase1SaveFormat.Tags.F64, 16);
            double value = BitConverter.UInt64BitsToDouble(ulong.Parse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture));
            if (!double.IsFinite(value)) throw new P1Refusal("FloatNotFinite", what + ": NaN and infinity are not allowed");
            return value;
        }

        public static long I64(JNode n, string what) =>
            unchecked((long)ulong.Parse(Tagged(n, what, Phase1SaveFormat.Tags.I64, 16), NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture));

        public static string Text(JNode n, string what)
        {
            var o = J.Keys(n, what, "k", "b");
            if (J.Str(o["k"], what + ".k") != Phase1SaveFormat.Tags.Utf8)
                throw new P1Refusal("SaveTagInvalid", what + ".k must be \"" + Phase1SaveFormat.Tags.Utf8 + "\"");
            string hex = J.Str(o["b"], what + ".b");
            if (hex.Length % 2 != 0 || !hex.All(c => c is >= '0' and <= '9' or >= 'a' and <= 'f'))
                throw new P1Refusal("SaveTextInvalid", what + " must be lowercase hex of UTF-8 bytes");
            try { return Phase1JsonWriter.StrictUtf8.GetString(Convert.FromHexString(hex)); }
            catch (DecoderFallbackException) { throw new P1Refusal("SaveTextInvalid", what + " is not valid UTF-8"); }
        }

        public static IReadOnlyList<string> Strs(JNode n, string what) => List(n, what, x => J.Str(x, what));

        public static IReadOnlyList<T> List<T>(JNode n, string what, Func<JNode, T> read) =>
            Array.AsReadOnly(J.Arr(n, what).Items.Select(read).ToArray());

        private static string Tagged(JNode n, string what, string kind, int digits)
        {
            var o = J.Keys(n, what, "k", "b");
            if (J.Str(o["k"], what + ".k") != kind) throw new P1Refusal("SaveTagInvalid", what + ".k must be \"" + kind + "\"");
            string hex = J.Str(o["b"], what + ".b");
            if (!IsLowerHex(hex, digits))
                throw new P1Refusal("FloatBitsInvalid", FormattableString.Invariant($"{what} bits must be exactly {digits} lowercase hex digits"));
            return hex;
        }
    }
}

/// <summary>
/// Canonical compact JSON writer for the save: caller-fixed key order, no whitespace, no escapes. Identifier strings must
/// be printable ASCII without quote or backslash (the <see cref="StrictJson"/> rule) and are refused otherwise; free text
/// goes through <see cref="Text"/> as UTF-8 hex instead.
/// </summary>
internal sealed class Phase1JsonWriter
{
    /// <summary>UTF-8 that refuses lone surrogates and invalid bytes instead of substituting U+FFFD.</summary>
    internal static readonly UTF8Encoding StrictUtf8 = new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private readonly StringBuilder sb = new();
    private readonly Stack<bool> first = new();

    public Phase1JsonWriter BeginObject() { Comma(); sb.Append('{'); first.Push(true); return this; }
    public Phase1JsonWriter BeginObject(string key) { Key(key); pendingValue = false; sb.Append('{'); first.Push(true); return this; }
    public Phase1JsonWriter EndObject() { first.Pop(); sb.Append('}'); return this; }
    public Phase1JsonWriter BeginArray(string key) { Key(key); pendingValue = false; sb.Append('['); first.Push(true); return this; }
    public Phase1JsonWriter EndArray() { first.Pop(); sb.Append(']'); return this; }

    /// <summary>Writes a key whose value the caller writes next with a keyless BeginObject (no comma of its own).</summary>
    public Phase1JsonWriter Key(string key)
    {
        Comma();
        Quote(key);
        sb.Append(':');
        // The next keyless BeginObject must not write a comma: mark this level as "value pending".
        pendingValue = true;
        return this;
    }

    private bool pendingValue;

    public Phase1JsonWriter Str(string key, string value) { Key(key); Quote(value); pendingValue = false; return this; }

    public Phase1JsonWriter StrOrNull(string key, string? value)
    {
        Key(key);
        if (value is null) sb.Append("null"); else Quote(value);
        pendingValue = false;
        return this;
    }

    public Phase1JsonWriter Null(string key) { Key(key); sb.Append("null"); pendingValue = false; return this; }
    public Phase1JsonWriter Bool(string key, bool value) { Key(key); sb.Append(value ? "true" : "false"); pendingValue = false; return this; }

    public Phase1JsonWriter Int(string key, int value)
    {
        Key(key);
        sb.Append(value.ToString(CultureInfo.InvariantCulture));
        pendingValue = false;
        return this;
    }

    public Phase1JsonWriter IntOrNull(string key, int? value)
    {
        Key(key);
        sb.Append(value is null ? "null" : value.Value.ToString(CultureInfo.InvariantCulture));
        pendingValue = false;
        return this;
    }

    public Phase1JsonWriter F32(string key, float value) { Key(key); pendingValue = false; F32Value(value); return this; }

    public Phase1JsonWriter F32Array(string key, IReadOnlyList<float> values)
    {
        BeginArray(key);
        foreach (float v in values) { Comma(); F32Value(v); }
        return EndArray();
    }

    public Phase1JsonWriter F64(string key, double value)
    {
        if (!double.IsFinite(value)) throw new P1Refusal("FloatNotFinite", key + ": NaN and infinity cannot be saved");
        Key(key);
        pendingValue = false;
        Tagged(Phase1SaveFormat.Tags.F64, BitConverter.DoubleToUInt64Bits(value).ToString("x16", CultureInfo.InvariantCulture));
        return this;
    }

    public Phase1JsonWriter F64OrNull(string key, double? value)
    {
        if (value is null) return Null(key);
        return F64(key, value.Value);
    }

    public Phase1JsonWriter I64(string key, long value)
    {
        Key(key);
        pendingValue = false;
        Tagged(Phase1SaveFormat.Tags.I64, unchecked((ulong)value).ToString("x16", CultureInfo.InvariantCulture));
        return this;
    }

    /// <summary>Free text: strict UTF-8 bytes as lowercase hex, so any character a player typed is kept exactly.</summary>
    public Phase1JsonWriter Text(string key, string value)
    {
        byte[] bytes;
        try { bytes = StrictUtf8.GetBytes(value); }
        catch (EncoderFallbackException) { throw new P1Refusal("SaveTextInvalid", key + " holds an unpaired surrogate and cannot be saved exactly"); }
        Key(key);
        pendingValue = false;
        Tagged(Phase1SaveFormat.Tags.Utf8, Convert.ToHexString(bytes).ToLowerInvariant());
        return this;
    }

    public Phase1JsonWriter StrArray(string key, IEnumerable<string> values)
    {
        BeginArray(key);
        foreach (var v in values) { Comma(); Quote(v); }
        return EndArray();
    }

    /// <summary>Splices an already canonical JSON value under a key (the header and payload into the envelope).</summary>
    public Phase1JsonWriter Raw(string key, string json) { Key(key); sb.Append(json); pendingValue = false; return this; }

    private void F32Value(float value)
    {
        if (!float.IsFinite(value)) throw new P1Refusal("FloatNotFinite", "NaN and infinity cannot be saved");
        Tagged(Phase1SaveFormat.Tags.F32, Core.F32.Hex(Core.F32.Bits(value)));
    }

    private void Tagged(string kind, string bits) => sb.Append("{\"k\":\"").Append(kind).Append("\",\"b\":\"").Append(bits).Append("\"}");

    // A comma goes before every element of an object or array except the first. A value written right after its key
    // (pendingValue) never writes one.
    private void Comma()
    {
        if (pendingValue) { pendingValue = false; return; }
        if (first.Count == 0) return;
        if (first.Peek()) { first.Pop(); first.Push(false); }
        else sb.Append(',');
    }

    private void Quote(string s)
    {
        StrictJson.CheckString(s);
        sb.Append('"').Append(s).Append('"');
    }

    public override string ToString()
    {
        if (first.Count != 0) throw new InvalidOperationException("unbalanced JSON writer");
        return sb.ToString();
    }
}
