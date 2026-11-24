using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Phase1Save.Tests;

/// <summary>
/// Hand-built representative save states. They are SYNTHETIC contract fixtures, not engine output: every region of the
/// root is populated (dead trees, felled and naturally dead stems, crop marks, events, executed receipts, approved or
/// pending work of every kind, three annual records, Stage 4 pruning/deadwood, Stage 5 cells/bands/accounts/blocks,
/// Phase 2 species/bands/juveniles/shelters/patches/orders/log/accounts) with awkward float and double bit patterns.
/// They satisfy the validator's structural invariants (including the R1 provenance, mast-seed, lattice, account and
/// plan-state rules) but do not claim ecological plausibility. Numeric fields that sit next to each other are given
/// DIFFERENT values on purpose, so a codec that swapped two of them would fail the exact round trip.
/// </summary>
public static class Samples
{
    public static readonly float NegZero = BitConverter.UInt32BitsToSingle(0x80000000);
    public static readonly float Subnormal = BitConverter.UInt32BitsToSingle(0x00000001); // float.Epsilon
    public static readonly float Odd = BitConverter.UInt32BitsToSingle(0x3db851ec);
    public static readonly double NegZeroD = BitConverter.UInt64BitsToDouble(0x8000000000000000);
    public const long BigSeed = 9_007_199_254_740_993; // 2^53 + 1: not exactly representable as a double
    public const string Reason = "Release \"crop\" B004 — café ✓ 🌲 \\ tab\tend";

    public const string Hash = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef";

    /// <summary>The engine's Stage 5 mast seed when the options name none: unchecked((int)GeneratorSeed).</summary>
    public static readonly int DerivedStage5MastSeed = unchecked((int)BigSeed);

    /// <summary>Phase 2 approved / pending plan-state revisions: at or above the validator's proven lower bound (20 / 19).</summary>
    public const int ApprovedPlanStateRevision = 24, PendingPlanStateRevision = 23;

    public static Phase1GenesisIdentity Genesis(string hash = Hash, int seedB = -7) => new("mad-test-0001", hash, "phase1-test-gen", 3,
        BigSeed, Array.AsReadOnly(new[] { new Phase1GenesisArea("MA-A", 11, 0.16f), new Phase1GenesisArea("MA-B", seedB, Odd) }));

    public static Phase1SaveExpectation Expect(Phase1ModelVector models) => new(models, Genesis(), Phase1EngineLineage.Current(models));

    private static IReadOnlyList<T> L<T>(params T[] items) => Array.AsReadOnly(items);

    private static Phase1Stand Stand(int living, double a) =>
        new(living, a, NegZeroD, double.Epsilon, 0.1 + 0.2);

    /// <summary>A receipt's recorded source exactly as Execute would have stamped it (MultiAreaContinuation.Token).</summary>
    public static Phase1SourceToken Token(int year, int annual, int work)
    {
        var l = Phase1EngineLineage.Current(Phase1ModelVector.Phase2);
        return new("MultiAreaSitkaExperimentV1", "mad-test-0001", Hash, "MA-A,MA-B", year, annual, work, l.RngModelVersion,
            l.GrowthModelVersion, l.P1AlgorithmVersion, l.AreaModelVersion, l.ActionEligibleDbhThresholdMm);
    }

    private static Phase1SaveHeader Header(string label, string utc, long generation, int year, Phase1ModelVector models) =>
        new(label, utc, generation, year, models, Genesis(), Phase1EngineLineage.Current(models));

    /// <summary>A full Phase 2 (1/1/1/1/1) save in Year 3, with an approved plan (all pending lists empty).</summary>
    public static Phase1SaveEnvelope Phase2Approved() =>
        new(Header("Spring review", "2026-11-24T13:53:37Z", 4, 3, Phase1ModelVector.Phase2), Phase2State(approved: true));

    /// <summary>The same forest with every kind of work still pending and no approved plan.</summary>
    public static Phase1SaveEnvelope Phase2Pending() =>
        new(Header("Pending draft", "2026-11-24T14:00:00Z", 5, 3, Phase1ModelVector.Phase2), Phase2State(approved: false));

    /// <summary>A frozen model 0/0/0/0/0 save in Year 1: no Stage 4/5/Phase 2 state or options; one Crop mark (MarkCrop = 1).</summary>
    public static Phase1SaveEnvelope Model0()
    {
        var trees = L(new Phase1Tree("A001", 1.5f, 2.5f, 30, 22.5f, 18f, 2.25f, null),
            new Phase1Tree("A002", 4f, NegZero, 31, 12f, 11f, 1.5f, new Phase1Death("A002", 1, "self-thinning", "density")));
        var treesB = L(new Phase1Tree("B001", 3f, 3.25f, 28, 20f, 16f, 2f, null));
        var state = new Phase1SaveState(new Phase1Options(null, null, null), 1, 1, 0, 1,
            L(new Phase1AreaState("MA-A", 1, trees, L<string>()), new Phase1AreaState("MA-B", 1, treesB, L("B001"))),
            L<Phase1WorkEvent>(), L<Phase1ExecutedWork>(), L<Phase1WorkOrder>(), null,
            L(new Phase1YearRecord(0, 1, 1, 0, L(
                new Phase1AreaYear("MA-A", Stand(2, 125.0), Stand(1, 62.5), L(new Phase1Death("A002", 1, "self-thinning", "density")), false),
                new Phase1AreaYear("MA-B", Stand(1, 50.0), Stand(1, 50.0), L<Phase1Death>(), false)), false)),
            null, null, null);
        return new Phase1SaveEnvelope(Header("Model zero", "2026-01-02T03:04:05Z", 1, 1, Phase1ModelVector.Model0), state);
    }

    private static Phase1SaveState Phase2State(bool approved)
    {
        // ---- rosters: living, felled (A002 in event 1, B002 in event 2), naturally dead (A003), promoted Oak (A-PJ0001)
        var areaA = new Phase1AreaState("MA-A", 3, L(
            new Phase1Tree("A-PJ0001", 6.25f, 7.75f, 6, 0.5f, 3.5f, 0.75f, null),
            new Phase1Tree("A001", NegZero, Subnormal, 41, float.MaxValue, 21.5f, Odd, null),
            new Phase1Tree("A002", 2f, 3f, 40, 18f, 17f, 1.9f, new Phase1Death("A002", 1, FellCause.Harvest, FellCause.Mechanism)),
            new Phase1Tree("A003", 5f, 1f, 39, 9.5f, 10f, 0.9f, new Phase1Death("A003", 2, "DensitySuppression", "density")),
            new Phase1Tree("A004", 8f, 8.5f, 42, 30f, 22f, 3.1f, null)), L("A004"));
        var areaB = new Phase1AreaState("MA-B", 3, L(
            new Phase1Tree("B001", 1f, 1.5f, 35, 21f, 19f, 2.2f, null),
            new Phase1Tree("B002", 2.25f, 2.75f, 36, 23f, 20f, 2.4f, new Phase1Death("B002", 2, FellCause.Harvest, FellCause.Mechanism)),
            new Phase1Tree("B003", 3.25f, 3.75f, 37, 25f, 21f, 2.6f, null)), L<string>());

        var fellOrderA = new Phase1WorkOrder("MA-A", "A002", WorkAction.Fell, Reason);
        var fellOrderB = new Phase1WorkOrder("MA-B", "B002", WorkAction.Fell, "Second thinning");
        var event1 = new Phase1WorkEvent("key-1", "sess:ma-plan:2", 1, 1, L(
            new Phase1AreaChange("MA-A", Stand(4, 250.0), Stand(3, 187.5), L(new Phase1FellRecord("MA-A", "A002", FellCause.Harvest,
                FellCause.Mechanism, Reason, 1, 1, "key-1", "sess:ma-plan:2"))),
            new Phase1AreaChange("MA-B", Stand(3, 40.0), Stand(3, 40.0), L<Phase1FellRecord>())));
        var event2 = new Phase1WorkEvent("key-2", "sess:ma-plan:5", 2, 2, L(
            new Phase1AreaChange("MA-A", Stand(3, 187.5), Stand(3, 187.5), L<Phase1FellRecord>()),
            new Phase1AreaChange("MA-B", Stand(3, 40.0), Stand(2, 26.0), L(new Phase1FellRecord("MA-B", "B002", FellCause.Harvest,
                FellCause.Mechanism, "Second thinning", 2, 2, "key-2", "sess:ma-plan:5")))));
        var executed = L(new Phase1ExecutedWork("key-1", Token(1, 1, 0), L(fellOrderA)), new Phase1ExecutedWork("key-2", Token(2, 2, 1), L(fellOrderB)));

        var history = L(
            new Phase1YearRecord(0, 1, 1, 0, L(new Phase1AreaYear("MA-A", Stand(4, 250.0), Stand(4, 250.0), L<Phase1Death>(), false),
                new Phase1AreaYear("MA-B", Stand(3, 40.0), Stand(3, 40.0), L<Phase1Death>(), false)), false),
            new Phase1YearRecord(1, 2, 2, 1, L(new Phase1AreaYear("MA-A", Stand(3, 187.5), Stand(2, 125.0),
                    L(new Phase1Death("A003", 2, "DensitySuppression", "density")), false),
                new Phase1AreaYear("MA-B", Stand(3, 40.0), Stand(3, 40.0), L<Phase1Death>(), false)), true),
            new Phase1YearRecord(2, 3, 3, 2, L(new Phase1AreaYear("MA-A", Stand(3, 187.5), Stand(3, 187.5), L<Phase1Death>(), false),
                new Phase1AreaYear("MA-B", Stand(2, 26.0), Stand(2, 26.0), L<Phase1Death>(), false)), false));

        var fellB003 = new Phase1WorkOrder("MA-B", "B003", WorkAction.Fell, "Make space ✓");
        var pruneB001 = new Phase1PruneOrder("MA-B", "B001", 4.75f);
        var clear = new Phase1ClearOrder("MA-A", 1, 0);
        var plantingOrders = L<Phase1PlantingOrder>(new Phase1SpotClearOrder("P2W0004", "MA-B", 1.25f, 1.75f),
            new Phase1PlantOrder("P2W0005", "MA-B", "beech", 1.375f, 1.625f), new Phase1ShelterOrder("P2W0006", "MA-B", 1.125f, 1.875f));

        var stage4 = new Phase1Stage4State(
            L(new Phase1TreePruning("A001", 1, 2.5f, 2), new Phase1TreePruning("B001", 0, 0f, -1)),
            approved ? L<Phase1PruneOrder>() : L(pruneB001), approved ? L(pruneB001) : L<Phase1PruneOrder>(),
            L(new Phase1PruneRecord("MA-A", "A001", 2.5f, 2, 2, "key-2", "sess:ma-plan:5", 3.4f, Odd, 1)),
            L(new Phase1Deadwood("DW0001", "A003", "MA-A", "sitka-spruce", 5f, 1f, 2, "DensitySuppression", 9.5f, 10f, 0.0354f)), 2);

        var promotion = new Phase1PromotionRecord("A-PJ0001", "MA-A", 2, 3, 1, 4, 0, 6, 3.5f, 0.5f, 0.75f, NegZero, 6.25f, 7.75f, true, 0.625f, 5,
            0.875);
        // De-aliased: every int and double field differs from its neighbours.
        var s5Account = new Phase1Stage5AreaAccount("MA-A", 2, 1.5, 0.25, 0.01, 0.02, NegZeroD, 0.5, 0.4, 0.1, 3, 4, 5, 6, 7, 0.125, 0.64, 0.33,
            0.45, 0.2, 0.71, L(promotion), 8, 9, L(0.25f, NegZero));
        var stage5 = new Phase1Stage5State(DerivedStage5MastSeed,
            L(new Phase1Stage5AreaCells("MA-A", L(
                    new Phase1Stage5Cell(0, 0, 1.25f, 1.375f, 6.25, 0.8f, 0.3f, 0.1f, L(new Phase1Stage5Band(0.5f, 0.15f, 2, 1, 0.2f, 3),
                        new Phase1Stage5Band(Subnormal, 0f, 3, 2, 0f, -1))),
                    new Phase1Stage5Cell(1, 0, 3.75f, 1.25f, 5.123456789012345, 0.75f, 0f, 0.05f, L<Phase1Stage5Band>()))),
                new Phase1Stage5AreaCells("MA-B", L(new Phase1Stage5Cell(0, 0, 1.25f, 1.125f, 6.0, 1f, 0.5f, 0.55f, L<Phase1Stage5Band>())))),
            approved ? L<Phase1ClearOrder>() : L(clear), approved ? L(clear) : L<Phase1ClearOrder>(),
            L(new Phase1ClearRecord("MA-A", 0, 0, 2, 2, "key-2", "sess:ma-plan:5", 0.6f, 0.2f, 1, 0.5)),
            L(new Phase1Stage5YearAccount(0, 1, 0.123456789, "normal", 1f, L(s5Account with { Promotions = L<Phase1PromotionRecord>() })),
                new Phase1Stage5YearAccount(1, 2, 0.987654321, "mast", 2.5f, L(s5Account)),
                new Phase1Stage5YearAccount(2, 3, double.Epsilon, "poor", 0.25f, L<Phase1Stage5AreaAccount>())),
            L(new Phase1BlockCredit(3, 1, 4, 2), new Phase1BlockCredit(5, 6, 7, 8)));

        // Phase 2 seeds: Oak named by the options, Beech derived from the Stage 5 mast seed (Phase2Annual.Genesis).
        var phase2 = new Phase1Phase2State(1234, DerivedStage5MastSeed,
            L(new Phase1TreeSpecies("A-PJ0001", "sessile-oak"), new Phase1TreeSpecies("A004", "beech")),
            L(new Phase1Phase2AreaBands("MA-A", L(new Phase1Phase2CellBands(0, 0, L(new Phase1Phase2Band("sessile-oak", "Natural", 0.4f, 0.3f, 2, 1,
                    NegZero, 3))), new Phase1Phase2CellBands(1, 0, L<Phase1Phase2Band>()))),
                new Phase1Phase2AreaBands("MA-B", L(new Phase1Phase2CellBands(0, 0, L(new Phase1Phase2Band("beech", "Natural", Subnormal, 0.15f, 3, 2,
                    0f, -1)))))),
            L(new Phase1PlantedJuvenile("PJ0001", "MA-A", "sessile-oak", 6.25f, 7.75f, 1, 6f, 3.5f, true, "A-PJ0001", -1, null, false, 0.1f, 0.05f, 3),
                new Phase1PlantedJuvenile("PJ0002", "MA-B", "beech", 2.5f, 2.75f, 1, 4f, 0.7f, false, null, 2, "Browse", true, 0.6f, 0.2f, 2),
                new Phase1PlantedJuvenile("PJ0003", "MA-B", "beech", 3.5f, 1.5f, 2, 4.5f, Odd, true, null, -1, null, false, NegZero, 0f, 3)),
            L(new Phase1Shelter("S-JPJ0003", "PJ0003", 3.5f, 1.5f, 2, 5, -1)),
            L(new Phase1SpotPatch("SP0001", "MA-B", 3.5f, 1.5f, 0.5642f, 2, 3, 0.05f, Subnormal)),
            approved ? L<Phase1PlantingOrder>() : plantingOrders, approved ? plantingOrders : L<Phase1PlantingOrder>(),
            L(new Phase1Phase2WorkRecord("P2W0001", "Plant", "MA-A", 6.25f, 7.75f, "sessile-oak", "PJ0001", null, null, 1, 1, "key-1", "sess:ma-plan:2"),
                new Phase1Phase2WorkRecord("P2W0002", "SpotClear", "MA-B", 3.5f, 1.5f, null, null, null, "SP0001", 2, 2, "key-2", "sess:ma-plan:5"),
                new Phase1Phase2WorkRecord("P2W0003", "Shelter", "MA-B", 3.5f, 1.5f, "beech", "PJ0003", "S-JPJ0003", null, 2, 2, "key-2",
                    "sess:ma-plan:5")),
            7, 4, 2,
            // One account per committed year, 0→1, 1→2, 2→3 (Phase2Annual.Step appends one per Advance).
            L(new Phase1Phase2YearAccount(0, 1, L<Phase1MastRecord>(), L<Phase1SpeciesAreaAccount>(), L<Phase1PlantedAccount>(),
                    L<Phase1DensityRecord>()),
                new Phase1Phase2YearAccount(1, 2,
                    L(new Phase1MastRecord("sessile-oak", 1234, 0.25, "normal", 1f),
                        new Phase1MastRecord("beech", DerivedStage5MastSeed, NegZeroD, "mast", 3f)),
                    L(new Phase1SpeciesAreaAccount("MA-A", "sessile-oak", 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 1, 2, 3, 4, 5, 0.9, 1.1, 6, 7,
                        L(promotion))),
                    L(new Phase1PlantedAccount("beech", 10, 1, 2, 3, 4, 0, L<string>()),
                        new Phase1PlantedAccount("sessile-oak", 1, 0, 0, 0, 0, 1, L("A-PJ0001"))),
                    L(new Phase1DensityRecord("MA-A", true, 14, 13, 1, 2, 0.75, 0.5, 0.25, 3, 0.3125, 4, 0.3, 0.912, 5, 6, false, 0.1, 0.02,
                            L<string>()),
                        new Phase1DensityRecord("MA-B", false, 7, 6, 8, 9, 0.55, 0.45, 0.35, 10, 0.65, 11, 0.15, null, 12, 13, true, 0.2, 0.05,
                            L("B9")))),
                new Phase1Phase2YearAccount(2, 3, L<Phase1MastRecord>(), L<Phase1SpeciesAreaAccount>(), L<Phase1PlantedAccount>(),
                    L<Phase1DensityRecord>())));

        var options = new Phase1Options(
            new Phase1Stage4Options(Phase1SaveFormat.SiteProductivityUniform, L(new Phase1GrowthAgePhase(0f, 1f, 1.25f),
                new Phase1GrowthAgePhase(40f, 0.75f, Odd))),
            new Phase1Stage5Options(2.5, 0.2f, null),
            new Phase1Phase2Options(1234, null, L(new Phase1TreeSpecies("A004", "beech"))));

        return new Phase1SaveState(options, 3, 3, 2, approved ? ApprovedPlanStateRevision : PendingPlanStateRevision, L(areaA, areaB),
            L(event1, event2), executed,
            approved ? L<Phase1WorkOrder>() : L(fellB003),
            approved ? new Phase1ApprovedPlan(L(fellB003)) : null,
            history, stage4, stage5, phase2);
    }
}
