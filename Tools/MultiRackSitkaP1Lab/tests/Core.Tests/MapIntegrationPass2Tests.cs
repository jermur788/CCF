using System.Text.Json;
using System.Text.Json.Serialization;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MULTI Map Integration Pass 2 Core tests (spec multi-map-integration-pass2-v1):
/// single commitment gate across all six order types (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear),
/// all 14 required Core refusal codes with zero state mutation, sequential Plant -> Shelter -> SpotClear
/// fence-chained partial-failure disclosure and recovery, post-Execute unsheltered PJ shelter installation,
/// and raw UTF-8 JSON payload budgets with Pass 2 orders pending and executed at Year 0 and Year +5.
/// </summary>
internal static class MapIntegrationPass2Tests
{
    private const int MaxViewBytes = 512 * 1024;
    private const int MaxEcologyBytes = 150 * 1024;

    private static readonly JsonSerializerOptions WireJson = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        Converters = { new JsonStringEnumConverter() },
    };

    internal static void Register()
    {
        T.Add("MI-P2 C-01: single commitment gate across all six order types (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear) mutates zero ecology on Queue/Remove/Approve/Cancel and commits atomically on Execute with zero year advance", () =>
        {
            var (exp, opts, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
            var adapter = new MultiAreaWorkPlanAdapter(session, exp, "ma-1-mixed", badge);

            var snap0 = adapter.ReadCompositeUiSnapshot();
            var eco0 = snap0.Ecology!;
            var v0 = snap0.View;

            // Choose concrete valid targets across both areas:
            // - Fell: living Sitka in MA-A
            // - Prune: living Oak in MA-B
            // - ClearUndergrowth: first in-polygon ground cell in MA-B
            // - Plant + Shelter + SpotClear: valid open point in MA-B (>= 1.0 m from living adults)
            string fellId = v0.Areas[0].Trees[0].TreeId;
            string oakId = v0.Areas[1].Trees.First(t =>
                eco0.SpeciesByTreeId.TryGetValue(t.TreeId, out var sp) && sp == Phase2Species.OakId).TreeId;
            var clearCell = snap0.Readout.Areas[1].Ground[0];
            var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);

            var cellBefore = snap0.Stage5!.Areas[1].Cells.Single(c => c.Column == clearCell.Column && c.Row == clearCell.Row);
            float oakCrownBaseBefore = snap0.Readout.Areas[1].Trees.Single(t => t.TreeId == oakId).CrownBaseHeightM;

            // 1. Queue all 6 order types sequentially with refreshed fences
            var v = adapter.AddPending(Fence(v0), "MA-A", fellId, "Pass 2 commitment gate fell").View;
            v = adapter.AddPrune(Fence(v), "MA-B", oakId).View;
            v = session.AddClearUndergrowth(Fence(v), "MA-B", clearCell.Column, clearCell.Row).View;
            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            v = session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            v = session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;

            var snapPending = adapter.ReadCompositeUiSnapshot();
            T.Eq(6, snapPending.View.PlanStateRevision, "6 queued orders bumped planStateRevision to 6");
            T.Eq(1, snapPending.View.Pending.Count, "1 pending Fell");
            T.Eq(1, snapPending.Readout.PendingPrunes.Count, "1 pending Prune");
            T.Eq(1, snapPending.Ecology!.PendingClear.Count, "1 pending ClearUndergrowth");
            T.Eq(3, snapPending.Ecology.PendingPlanting.Count, "3 pending planting orders (Plant, Shelter, SpotClear)");
            AssertZeroEcologyMutation(snap0, snapPending, clearCell.Column, clearCell.Row, cellBefore, oakId, oakCrownBaseBefore, "after queueing");

            // Targeted removals before Approve change planStateRevision only and mutate zero ecology
            string spotOrderId = snapPending.Ecology.PendingPlanting.Single(o => o.Kind == "SpotClear").OrderId;
            v = session.RemoveClearUndergrowth(Fence(v), "MA-B", clearCell.Column, clearCell.Row).View;
            v = session.RemovePlanting(Fence(v), spotOrderId).View;
            var snapRemoved = adapter.ReadCompositeUiSnapshot();
            T.Eq(0, snapRemoved.Ecology!.PendingClear.Count, "ClearUndergrowth removed");
            T.Eq(2, snapRemoved.Ecology.PendingPlanting.Count, "SpotClear removed");
            AssertZeroEcologyMutation(snap0, snapRemoved, clearCell.Column, clearCell.Row, cellBefore, oakId, oakCrownBaseBefore, "after targeted removal");

            // Re-queue ClearUndergrowth and SpotClear, then Approve
            v = session.AddClearUndergrowth(Fence(v), "MA-B", clearCell.Column, clearCell.Row).View;
            v = session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
            v = adapter.Approve(Fence(v)).View;

            var snapApproved = adapter.ReadCompositeUiSnapshot();
            T.True(snapApproved.View.Approved is not null, "plan approved");
            T.Eq(1, snapApproved.View.Approved!.Orders.Count, "1 approved Fell");
            T.Eq(1, snapApproved.Readout.ApprovedPrunes.Count, "1 approved Prune");
            T.Eq(1, snapApproved.Ecology!.ApprovedClear.Count, "1 approved ClearUndergrowth");
            T.Eq(3, snapApproved.Ecology.ApprovedPlanting.Count, "3 approved planting orders");
            AssertZeroEcologyMutation(snap0, snapApproved, clearCell.Column, clearCell.Row, cellBefore, oakId, oakCrownBaseBefore, "after Approve");

            // Execute commits atomically with zero year advance
            var exec = adapter.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(exec.RefusalCode is null && exec.Execution is not null, "Execute succeeded");
            var snapExec = adapter.ReadCompositeUiSnapshot();
            T.Eq(0, snapExec.View.Source.EcologicalYear, "Execute advances ecological year by 0");
            T.Eq(1, snapExec.View.Source.WorkRevision, "Execute bumps workRevision 0 -> 1");
            T.True(snapExec.View.Approved is null && snapExec.View.Pending.Count == 0, "approval and pending cleared");
            T.Eq(0, snapExec.Ecology!.ApprovedClear.Count + snapExec.Ecology.ApprovedPlanting.Count, "approved clear/planting cleared");
            T.Eq(170, snapExec.View.Areas[0].Stand.LivingCount, "MA-A fell committed (171 -> 170)");
            T.Eq(73, snapExec.View.Areas[1].Stand.LivingCount, "MA-B living count unchanged by non-Fell actions");

            var oakAfter = snapExec.Readout.Areas[1].Trees.Single(t => t.TreeId == oakId);
            T.Eq(1, oakAfter.PruningLifts, "Oak lift 1 committed");
            T.True(Math.Abs(oakAfter.CrownBaseHeightM - 3.5f) < 1e-4f, "Oak crown base raised to 3.5 m");

            var cellAfter = snapExec.Stage5!.Areas[1].Cells.Single(c => c.Column == clearCell.Column && c.Row == clearCell.Row);
            T.Eq(0f, cellAfter.Bramble, "cleared cell bramble -> 0");
            T.Eq(0f, cellAfter.Bracken, "cleared cell bracken -> 0");
            T.Eq(cellBefore.Bands.Count, cellAfter.Bands.Count, "ClearUndergrowth spares natural seedling cohorts");

            T.Eq(1, snapExec.Ecology.Patches.Count, "1 SpotClear patch created");
            T.True(Math.Abs(snapExec.Ecology.Patches[0].RadiusM - 0.5641896f) < 0.001f, "SpotClear patch radius ~0.564 m");
            T.Eq(1, snapExec.Ecology.Planted.Count, "1 planted juvenile PJ created");
            T.Eq(Phase2Species.OakId, snapExec.Ecology.Planted[0].SpeciesId, "PJ species is sessile-oak");
            T.True(snapExec.Ecology.Planted[0].IsActive && Math.Abs(snapExec.Ecology.Planted[0].HeightM - 0.6f) < 1e-4f, "PJ active nursery 0.6 m");
            T.Eq(1, snapExec.Ecology.Shelters.Count, "1 active 8-year shelter created");
            T.Eq(snapExec.Ecology.Planted[0].JuvenileId, snapExec.Ecology.Shelters[0].JuvenileId, "Shelter bound to newly planted PJ (SpotClear -> Plant -> Shelter)");
            T.True(snapExec.Ecology.Shelters[0].IsEffective && snapExec.Ecology.Shelters[0].ExpiryYear == snapExec.Ecology.Shelters[0].InstalledYear + 8,
                "Shelter active with 8-year expiry");

            // CancelApproved discards an approved mixed plan with zero ecology mutation
            var pt2 = Phase2Support.FindPoint(session, "MA-B", 1.0f, (x, z) => (x - pt.X) * (x - pt.X) + (z - pt.Z) * (z - pt.Z) > 9f);
            var cell2 = snapExec.Readout.Areas[1].Ground[1];
            v = snapExec.View;
            v = session.AddClearUndergrowth(Fence(v), "MA-B", cell2.Column, cell2.Row).View;
            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt2.X, pt2.Z).View;
            v = adapter.Approve(Fence(v)).View;
            v = adapter.CancelApproved(Fence(v)).View;
            var snapCancel = adapter.ReadCompositeUiSnapshot();
            T.True(snapCancel.View.Approved is null, "approved plan cancelled");
            T.Eq(0, snapCancel.Ecology!.PendingClear.Count + snapCancel.Ecology.ApprovedClear.Count, "clear discarded");
            T.Eq(0, snapCancel.Ecology.PendingPlanting.Count + snapCancel.Ecology.ApprovedPlanting.Count, "planting discarded");
            T.Eq(1, snapCancel.Ecology.Planted.Count, "CancelApproved added zero PJ saplings");
            T.Eq(1, snapCancel.View.Source.WorkRevision, "CancelApproved left workRevision unchanged");
        });

        T.Add("MI-P2 C-02: all 14 required Core refusal codes trigger cleanly with exact codes and zero state mutation", () =>
        {
            var (exp, opts, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
            var adapter = new MultiAreaWorkPlanAdapter(session, exp, "ma-1-mixed", badge);
            var snap0 = adapter.ReadCompositeUiSnapshot();
            var v0 = snap0.View;

            var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);
            var cellB = snap0.Readout.Areas[1].Ground[0];
            var cellA = snap0.Readout.Areas[0].Ground[0];
            var livingAdult = v0.Areas[1].Trees.First(t => t.IsLiving);

            // Find a boundary-sliver point strictly inside a rack polygon with no in-polygon 2.5 m ground cell
            var locate = Phase2Annual.Locator(Phase2Support.Stage5Of(session));
            var allAdults = v0.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
            (string AreaId, float X, float Z)? sliver = null;
            foreach (var area in v0.Areas)
            {
                double minX = area.Polygon.Min(p => p.XM), maxX = area.Polygon.Max(p => p.XM);
                double minZ = area.Polygon.Min(p => p.ZM), maxZ = area.Polygon.Max(p => p.ZM);
                for (double z = Math.Floor(minZ) + 0.125; z < maxZ && sliver is null; z += 0.25)
                    for (double x = Math.Floor(minX) + 0.125; x < maxX && sliver is null; x += 0.25)
                    {
                        float fx = (float)x, fz = (float)z;
                        if (!Stage5Promotion.StrictlyInside(area.Polygon, fx, fz)) continue;
                        if (allAdults.Any(t => (t.XM - fx) * (t.XM - fx) + (t.ZM - fz) * (t.ZM - fz) < 0.7f * 0.7f)) continue;
                        if (locate(fx, fz) is null) sliver = (area.AreaId, fx, fz);
                    }
            }
            T.True(sliver is not null, "found boundary-sliver point for Phase2PointWithoutGroundCell");

            void AssertRefusal(string expectedCode, Action call)
            {
                var before = session.ReadView();
                T.Refuses(expectedCode, call);
                var after = session.ReadView();
                T.Eq(before.PlanStateRevision, after.PlanStateRevision, expectedCode + " leaves planStateRevision unchanged");
                T.Eq(before.Source, after.Source, expectedCode + " leaves source unchanged");
            }

            // 1. Phase2CoordinatesInvalid
            AssertRefusal("Phase2CoordinatesInvalid", () => session.AddPlant(Fence(session.ReadView()), "MA-B", Phase2Species.OakId, float.PositiveInfinity, pt.Z));
            AssertRefusal("Phase2CoordinatesInvalid", () => session.AddSpotClear(Fence(session.ReadView()), "MA-B", float.NaN, pt.Z));
            // 2. Phase2PointOutsideArea
            AssertRefusal("Phase2PointOutsideArea", () => session.AddPlant(Fence(session.ReadView()), "MA-B", Phase2Species.OakId, -50f, -50f));
            // 3. Phase2PointWithoutGroundCell
            AssertRefusal("Phase2PointWithoutGroundCell", () => session.AddPlant(Fence(session.ReadView()), sliver!.Value.AreaId, Phase2Species.OakId, sliver.Value.X, sliver.Value.Z));
            // 4. Phase2SpeciesUnknown
            AssertRefusal("Phase2SpeciesUnknown", () => session.AddPlant(Fence(session.ReadView()), "MA-B", Phase2Species.SitkaId, pt.X, pt.Z));
            AssertRefusal("Phase2SpeciesUnknown", () => session.AddPlant(Fence(session.ReadView()), "MA-B", "larch", pt.X, pt.Z));
            // 5. PlantTooCloseToAdult
            AssertRefusal("PlantTooCloseToAdult", () => session.AddPlant(Fence(session.ReadView()), "MA-B", Phase2Species.OakId, livingAdult.XM, livingAdult.ZM));
            // 6. ShelterNoTarget
            AssertRefusal("ShelterNoTarget", () => session.AddShelter(Fence(session.ReadView()), "MA-B", pt.X, pt.Z));
            // 7. ClearCellNotInArea (and MultiAreaWrongArea for another area's cell)
            AssertRefusal("ClearCellNotInArea", () => session.AddClearUndergrowth(Fence(session.ReadView()), "MA-B", 999, 999));
            AssertRefusal("MultiAreaWrongArea", () => session.AddClearUndergrowth(Fence(session.ReadView()), "MA-B", cellA.Column, cellA.Row));
            // 8. UnknownWorkOrder
            AssertRefusal("UnknownWorkOrder", () => session.RemoveClearUndergrowth(Fence(session.ReadView()), "MA-B", cellB.Column, cellB.Row));
            AssertRefusal("UnknownWorkOrder", () => session.RemovePlanting(Fence(session.ReadView()), "P2W9999"));

            // Queue valid orders to test duplicate, spacing, stale, approved and already-present refusals
            var staleFence = Fence(session.ReadView());
            var v = session.AddClearUndergrowth(Fence(session.ReadView()), "MA-B", cellB.Column, cellB.Row).View;
            // 9. ClearDuplicateCell
            AssertRefusal("ClearDuplicateCell", () => session.AddClearUndergrowth(Fence(v), "MA-B", cellB.Column, cellB.Row));
            // 10. PlayerPlanStateStale (across all 6 verbs)
            AssertRefusal("PlayerPlanStateStale", () => session.AddClearUndergrowth(staleFence, "MA-B", cellB.Column, cellB.Row));
            AssertRefusal("PlayerPlanStateStale", () => session.RemoveClearUndergrowth(staleFence, "MA-B", cellB.Column, cellB.Row));
            AssertRefusal("PlayerPlanStateStale", () => session.AddPlant(staleFence, "MA-B", Phase2Species.OakId, pt.X, pt.Z));
            AssertRefusal("PlayerPlanStateStale", () => session.AddShelter(staleFence, "MA-B", pt.X, pt.Z));
            AssertRefusal("PlayerPlanStateStale", () => session.AddSpotClear(staleFence, "MA-B", pt.X, pt.Z));
            AssertRefusal("PlayerPlanStateStale", () => session.RemovePlanting(staleFence, "P2W0001"));

            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            // 11. Phase2DuplicateOrder (exact same point)
            AssertRefusal("Phase2DuplicateOrder", () => session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z));
            // 12. PlantSpacingTooClose (< 0.5 m from queued Plant)
            AssertRefusal("PlantSpacingTooClose", () => session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt.X + 0.2f, pt.Z));

            v = session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            AssertRefusal("Phase2DuplicateOrder", () => session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z));
            v = session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
            AssertRefusal("Phase2DuplicateOrder", () => session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z));

            string existingOrderId = adapter.ReadCompositeUiSnapshot().Ecology!.PendingPlanting[0].OrderId;
            v = session.Approve(Fence(v)).View;
            // 13. WorkPlanAlreadyApproved (across all 6 verbs)
            AssertRefusal("WorkPlanAlreadyApproved", () => session.AddClearUndergrowth(Fence(v), "MA-B", cellB.Column, cellB.Row));
            AssertRefusal("WorkPlanAlreadyApproved", () => session.RemoveClearUndergrowth(Fence(v), "MA-B", cellB.Column, cellB.Row));
            AssertRefusal("WorkPlanAlreadyApproved", () => session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt.X + 3f, pt.Z));
            AssertRefusal("WorkPlanAlreadyApproved", () => session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z));
            AssertRefusal("WorkPlanAlreadyApproved", () => session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z));
            AssertRefusal("WorkPlanAlreadyApproved", () => session.RemovePlanting(Fence(v), existingOrderId));

            // Execute so PJ0001 + active shelter SH0001 exist, then verify 14. ShelterAlreadyPresent
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            AssertRefusal("ShelterAlreadyPresent", () => session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z));
        });

        T.Add("MI-P2 C-03: sequential fence-chained Plant -> Shelter -> SpotClear stops on step-3 409 with honest partial state, cleans up via RemovePlanting, and supports post-Execute shelter on unsheltered PJ", () =>
        {
            var (exp, opts, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            var session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
            var adapter = new MultiAreaWorkPlanAdapter(session, exp, "ma-1-mixed", badge);
            var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);

            // Pre-queue a SpotClear at pt so step 3 of Plant -> Shelter -> SpotClear hits Phase2DuplicateOrder
            var v = session.AddSpotClear(Fence(session.ReadView()), "MA-B", pt.X, pt.Z).View;
            string preSpotId = adapter.ReadCompositeUiSnapshot().Ecology!.PendingPlanting.Single().OrderId;

            // Step 1: Plant succeeds with fresh fence
            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            // Step 2: Shelter succeeds with fresh fence
            v = session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            // Step 3: SpotClear refuses with Phase2DuplicateOrder
            int revBeforeStep3 = v.PlanStateRevision;
            T.Refuses("Phase2DuplicateOrder", () => session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z));

            // Earlier pending Plant and Shelter orders stay honestly queued (zero silent rollback, zero ecology mutation)
            var partialEco = adapter.ReadCompositeUiSnapshot().Ecology!;
            T.Eq(revBeforeStep3, partialEco.PlanStateRevision, "refused step 3 left planStateRevision unchanged");
            T.Eq(3, partialEco.PendingPlanting.Count, "pre-existing SpotClear + earlier Plant & Shelter remain pending");
            string plantId = partialEco.PendingPlanting.Single(o => o.Kind == "Plant").OrderId;
            string shelterId = partialEco.PendingPlanting.Single(o => o.Kind == "Shelter").OrderId;

            // One-click RemovePlanting recovers cleanly (removing Shelter, Plant, and pre-existing SpotClear)
            v = session.RemovePlanting(Fence(session.ReadView()), shelterId).View;
            v = session.RemovePlanting(Fence(v), plantId).View;
            v = session.RemovePlanting(Fence(v), preSpotId).View;
            T.Eq(0, adapter.ReadCompositeUiSnapshot().Ecology!.PendingPlanting.Count, "all pending orders cleanly removed");

            // Post-Execute unsheltered PJ inspection -> Queue 8-year Shelter in next Work Plan
            v = session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt.X, pt.Z).View;
            v = session.Approve(Fence(v)).View;
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            var unsheltered = adapter.ReadCompositeUiSnapshot().Ecology!;
            T.Eq(1, unsheltered.Planted.Count, "1 unsheltered Beech PJ created");
            T.Eq(0, unsheltered.Shelters.Count, "0 shelters initially");
            var pj = unsheltered.Planted[0];
            T.Eq(Phase2Species.BeechId, pj.SpeciesId, "Beech sapling");

            v = session.AddShelter(Fence(v), pj.AreaId, pj.XM, pj.ZM).View;
            T.Eq(1, adapter.ReadCompositeUiSnapshot().Ecology!.PendingPlanting.Count, "8-year shelter queued for existing PJ");
            T.Eq(0, adapter.ReadCompositeUiSnapshot().Ecology!.Shelters.Count, "zero shelter installed while Pending");
            v = session.Approve(Fence(v)).View;
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            var sheltered = adapter.ReadCompositeUiSnapshot().Ecology!;
            T.Eq(1, sheltered.Shelters.Count, "8-year shelter installed on Execute");
            T.Eq(pj.JuvenileId, sheltered.Shelters[0].JuvenileId, "Shelter targets exact existing PJ sapling");
            T.True(sheltered.Shelters[0].IsEffective, "Installed shelter is active");
        });

        T.Add("MI-P2 C-04: raw UTF-8 JSON payload budgets hold on MA-1 and MA-1-MIXED at Year 0 and Year +5 with Pass 2 orders both Pending and Executed", () =>
        {
            var labels = new[] { typeof(PlayerLabels), typeof(CropLabels), typeof(MultiAreaLabels) }
                .SelectMany(t => t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .ToDictionary(f => f.Name, f => (string)f.GetRawConstantValue()!, StringComparer.Ordinal);

            void CheckBudgets(MultiAreaWorkPlanAdapter adapter, string tag)
            {
                var snap = adapter.ReadCompositeUiSnapshot();
                var scenes = snap.View.Areas.ToDictionary(a => a.AreaId, a => MultiAreaProjection.Scene(snap.View, a), StringComparer.Ordinal);
                var envelope = new
                {
                    view = snap.View,
                    scenes,
                    labels,
                    stage4 = snap.Readout,
                    ecology = snap.Ecology,
                };
                byte[] totalBytes = JsonSerializer.SerializeToUtf8Bytes(envelope, WireJson);
                byte[] ecoBytes = JsonSerializer.SerializeToUtf8Bytes(snap.Ecology, WireJson);
                Console.WriteLine(FormattableString.Invariant(
                    $"  MI-P2 payload {tag}: total {totalBytes.Length} B ({totalBytes.Length / 1024.0:0.00} KiB), ecology {ecoBytes.Length} B ({ecoBytes.Length / 1024.0:0.00} KiB)"));
                T.True(totalBytes.Length <= MaxViewBytes, $"{tag} total {totalBytes.Length} <= {MaxViewBytes}");
                T.True(ecoBytes.Length <= MaxEcologyBytes, $"{tag} ecology {ecoBytes.Length} <= {MaxEcologyBytes}");
            }

            foreach (string key in new[] { "ma-1", "ma-1-mixed" })
            {
                MultiAreaContinuation session;
                MultiAreaWorkPlanAdapter adapter;
                if (key == "ma-1-mixed")
                {
                    var (exp, opts, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
                    session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
                    adapter = new MultiAreaWorkPlanAdapter(session, exp, key, badge);
                }
                else
                {
                    var exp = MultiAreaGenerator.Generate();
                    session = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2);
                    adapter = new MultiAreaWorkPlanAdapter(session, exp, key, MultiAreaDemoExperiments.Ma1Badge);
                }

                // Year 0: queue Pass 2 orders -> check Pending budget -> Approve + Execute -> check Executed budget
                var pt0 = Phase2Support.FindPoint(session, "MA-B", 1.0f);
                var cell0 = adapter.ReadCompositeUiSnapshot().Readout.Areas[1].Ground[0];
                var v = session.AddClearUndergrowth(Fence(session.ReadView()), "MA-B", cell0.Column, cell0.Row).View;
                v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt0.X, pt0.Z).View;
                v = session.AddShelter(Fence(v), "MA-B", pt0.X, pt0.Z).View;
                v = session.AddSpotClear(Fence(v), "MA-B", pt0.X, pt0.Z).View;
                CheckBudgets(adapter, key + " Year 0 Pending");

                v = session.Approve(Fence(v)).View;
                v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
                CheckBudgets(adapter, key + " Year 0 Executed");

                // Advance to Year +5 -> queue another Pass 2 batch -> check Pending budget -> Approve + Execute -> check Executed budget
                for (int y = 0; y < 5; y++) v = session.Advance(Fence(v), false).View;
                var pt5 = Phase2Support.FindPoint(session, "MA-B", 1.0f, (x, z) => (x - pt0.X) * (x - pt0.X) + (z - pt0.Z) * (z - pt0.Z) > 9f);
                var cell5 = adapter.ReadCompositeUiSnapshot().Readout.Areas[1].Ground[1];
                v = session.AddClearUndergrowth(Fence(v), "MA-B", cell5.Column, cell5.Row).View;
                v = session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, pt5.X, pt5.Z).View;
                v = session.AddShelter(Fence(v), "MA-B", pt5.X, pt5.Z).View;
                v = session.AddSpotClear(Fence(v), "MA-B", pt5.X, pt5.Z).View;
                CheckBudgets(adapter, key + " Year 5 Pending");

                v = session.Approve(Fence(v)).View;
                v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
                CheckBudgets(adapter, key + " Year 5 Executed");
            }
        });
    }

    private static void AssertZeroEcologyMutation(
        MultiAreaCompositeUiSnapshot baseline,
        MultiAreaCompositeUiSnapshot current,
        int cellCol,
        int cellRow,
        Stage5Cell cellBefore,
        string oakId,
        float oakCrownBaseBefore,
        string stage)
    {
        T.Eq(baseline.View.Source.EcologicalYear, current.View.Source.EcologicalYear, stage + ": ecologicalYear unchanged");
        T.Eq(baseline.View.Source.WorkRevision, current.View.Source.WorkRevision, stage + ": workRevision unchanged");
        T.Eq(baseline.View.Source.AnnualRevision, current.View.Source.AnnualRevision, stage + ": annualRevision unchanged");
        T.Eq(baseline.View.Areas[0].Stand.LivingCount, current.View.Areas[0].Stand.LivingCount, stage + ": MA-A living count unchanged");
        T.Eq(baseline.View.Areas[1].Stand.LivingCount, current.View.Areas[1].Stand.LivingCount, stage + ": MA-B living count unchanged");
        T.Eq(oakCrownBaseBefore, current.Readout.Areas[1].Trees.Single(t => t.TreeId == oakId).CrownBaseHeightM, stage + ": Oak crown base unchanged");
        var cellNow = current.Stage5!.Areas[1].Cells.Single(c => c.Column == cellCol && c.Row == cellRow);
        T.Eq(cellBefore.Bramble, cellNow.Bramble, stage + ": cell bramble unchanged");
        T.Eq(cellBefore.Bracken, cellNow.Bracken, stage + ": cell bracken unchanged");
        T.Eq(0, current.Ecology!.Planted.Count, stage + ": zero planted juveniles");
        T.Eq(0, current.Ecology.Shelters.Count, stage + ": zero shelters");
        T.Eq(0, current.Ecology.Patches.Count, stage + ": zero spot-clear patches");
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
}
