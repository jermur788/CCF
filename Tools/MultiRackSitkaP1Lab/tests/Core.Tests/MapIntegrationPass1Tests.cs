using System.Text.Json;
using System.Text.Json.Serialization;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MULTI Map Integration Pass 1 Core tests (spec multi-map-integration-pass1-v1):
/// MA-1-MIXED demo factory, single-lock atomic composite snapshot, additive compact ecology DTO,
/// species-aware prune readout helper, read-only cell detail, and raw-byte payload budgets.
/// </summary>
internal static class MapIntegrationPass1Tests
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
        T.Add("MI-P1 C-01: MA-1-MIXED demo factory is deterministic, keeps Area A 100% Sitka, assigns Area B ordinal round-robin 0=Sitka/1=Oak(60)/2=Beech(60), and leaves MA-1 unchanged", () =>
        {
            var ma1 = MultiAreaGenerator.Generate();
            var (mixed1, opts1, badge1) = MultiAreaDemoExperiments.CreateMixedDemo();
            var (mixed2, opts2, badge2) = MultiAreaDemoExperiments.CreateMixedDemo();

            T.True(mixed1.ManifestFileBytes().AsSpan().SequenceEqual(mixed2.ManifestFileBytes()), "two calls give identical manifest bytes");
            T.Eq(MultiAreaDemoExperiments.Ma1MixedBadge, badge1, "mixed demo truthful badge");
            T.Eq(badge1, badge2, "badge deterministic");
            T.Eq(MultiAreaDemoExperiments.Ma1Badge, MultiAreaDemoExperiments.BadgeFor("ma-1"), "MA-1 truthful badge");
            T.Eq(MultiAreaDemoExperiments.Ma1MixedExperimentId, mixed1.ExperimentId, "mixed demo experiment ID");
            T.True(mixed1.ContentHash() != ma1.ContentHash(), "mixed demo has distinct content hash from MA-1");
            T.Eq("c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95", ma1.ContentHash(), "MA-1 generator content hash stays pinned");

            // Area A: 100% Sitka, untouched
            var areaA = mixed1.Areas.Single(a => a.AreaId == "MA-A");
            var ma1AreaA = ma1.Areas.Single(a => a.AreaId == "MA-A");
            T.True(areaA.Trees.SequenceEqual(ma1AreaA.Trees), "Area A trees identical to MA-1");
            T.True(areaA.Trees.All(t => !opts1.GenesisSpecies.ContainsKey(t.TreeId)), "Area A has zero broadleaf entries (100% Sitka)");

            // Area B: ordinal round-robin 0->Sitka, 1->Oak(age 60), 2->Beech(age 60)
            var areaB = mixed1.Areas.Single(a => a.AreaId == "MA-B");
            var ma1AreaB = ma1.Areas.Single(a => a.AreaId == "MA-B").Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
            var orderedB = areaB.Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
            T.Eq(ma1AreaB.Length, orderedB.Length, "Area B stem count preserved");
            int sitka = 0, oak = 0, beech = 0;
            for (int i = 0; i < orderedB.Length; i++)
            {
                var t = orderedB[i];
                var orig = ma1AreaB[i];
                T.Eq(orig.TreeId, t.TreeId, "ordinal TreeId order");
                if (i % 3 == 0)
                {
                    T.True(!opts1.GenesisSpecies.ContainsKey(t.TreeId), t.TreeId + " i%3==0 is Sitka");
                    T.Eq(orig.AgeYears, t.AgeYears, t.TreeId + " Sitka keeps MA-1 age");
                    sitka++;
                }
                else if (i % 3 == 1)
                {
                    T.Eq(Phase2Species.OakId, opts1.GenesisSpecies[t.TreeId], t.TreeId + " i%3==1 is sessile-oak");
                    T.Eq(60, t.AgeYears, t.TreeId + " Oak age 60");
                    oak++;
                }
                else
                {
                    T.Eq(Phase2Species.BeechId, opts1.GenesisSpecies[t.TreeId], t.TreeId + " i%3==2 is beech");
                    T.Eq(60, t.AgeYears, t.TreeId + " Beech age 60");
                    beech++;
                }
            }
            T.True(sitka > 0 && oak > 0 && beech > 0 && Math.Abs(sitka - oak) <= 1 && Math.Abs(oak - beech) <= 1,
                $"Area B balanced 1/3 split: sitka={sitka}, oak={oak}, beech={beech}");
            T.True(opts1.GenesisSpecies.Count == opts2.GenesisSpecies.Count &&
                opts1.GenesisSpecies.All(kv => opts2.GenesisSpecies.TryGetValue(kv.Key, out var v) && v == kv.Value),
                "GenesisSpecies map deterministic");

            // Adapter OpenExperiment checks
            var adapter = MultiAreaWorkPlanAdapter.OpenExperiment("ma-1-mixed");
            T.Eq("ma-1-mixed", adapter.ExperimentKey, "adapter key");
            T.Eq(MultiAreaDemoExperiments.Ma1MixedBadge, adapter.ExperimentBadge, "adapter badge");
            T.Refuses("MultiAreaUnknownExperiment", () => MultiAreaWorkPlanAdapter.OpenExperiment("other"));
            T.Refuses("Phase2ModelRequired", () => MultiAreaWorkPlanAdapter.OpenExperiment("ma-1-mixed", MultiAreaModelSelection.Stage4));
        });

        T.Add("MI-P1 C-02: ReadCompositeUiSnapshot shares one exact sessionId/source/planStateRevision across View, Stage4, Stage5, Phase2, Readout and Ecology; CompositeFor refuses stale roots", () =>
        {
            var adapter = MultiAreaWorkPlanAdapter.OpenExperiment("ma-1-mixed");
            var snap = adapter.ReadCompositeUiSnapshot();
            var v = snap.View;
            T.Eq(v.SessionId, snap.Readout.SessionId, "Readout sessionId");
            T.Eq(v.Source, snap.Readout.Source, "Readout source");
            T.Eq(v.PlanStateRevision, snap.Readout.PlanStateRevision, "Readout planStateRevision");
            T.Eq((v.SessionId, v.Source, v.PlanStateRevision), (snap.Stage4.SessionId, snap.Stage4.Source, snap.Stage4.PlanStateRevision), "Stage4 parity");
            T.True(snap.Stage5 is not null, "Stage5 present on Phase 2");
            T.Eq((v.SessionId, v.Source, v.PlanStateRevision), (snap.Stage5!.SessionId, snap.Stage5.Source, snap.Stage5.PlanStateRevision), "Stage5 parity");
            T.True(snap.Phase2 is not null, "Phase2 present on Phase 2");
            T.Eq((v.SessionId, v.Source, v.PlanStateRevision), (snap.Phase2!.SessionId, snap.Phase2.Source, snap.Phase2.PlanStateRevision), "Phase2 parity");
            T.True(snap.Ecology is not null, "Ecology present on Phase 2");
            T.Eq((v.SessionId, v.Source, v.PlanStateRevision), (snap.Ecology!.SessionId, snap.Ecology.Source, snap.Ecology.PlanStateRevision), "Ecology parity");

            var paired = adapter.CompositeFor(v);
            T.True(paired is not null && paired.Ecology is not null && paired.Stage4.Source == v.Source && paired.Ecology.Source == v.Source,
                "CompositeFor returns paired readout for current view");
            var moved = adapter.MarkCrop(Fence(v), "MA-A", v.Areas[0].Trees[0].TreeId).View;
            T.True(adapter.CompositeFor(v) is null, "CompositeFor returns null for moved root (zero torn frames)");
            T.True(adapter.CompositeFor(moved) is not null, "CompositeFor succeeds for new root");
        });

        T.Add("MI-P1 C-03: additive ecology DTO is null when SpeciesModelVersion == 0, enforces non-zero floor, sparse species map, bitmasks, shelters and patches", () =>
        {
            var ma1 = MultiAreaGenerator.Generate();
            foreach (var sel in new[] { MultiAreaModelSelection.Legacy, MultiAreaModelSelection.Stage3CrownC2, MultiAreaModelSelection.Stage4, MultiAreaModelSelection.Stage5 })
            {
                var snap = new MultiAreaContinuation(ma1, sel).ReadCompositeUiSnapshot();
                T.True(snap.Ecology is null, $"ecology is null for {sel} (Species == 0)");
            }

            // Positive non-zero floor check
            T.Eq(0.0, MultiAreaEcologyBuilder.CompactPositive(0.0), "0 stays 0");
            T.Eq(0.0, MultiAreaEcologyBuilder.CompactPositive(-0.5), "negative clamps to 0");
            T.True(MultiAreaEcologyBuilder.CompactPositive(1e-200) > 0.0, "tiny positive never rounds down to 0");
            T.Eq(0.0001, MultiAreaEcologyBuilder.CompactPositive(1e-6), "minimum positive floor 0.0001");
            T.Eq(0.4123, MultiAreaEcologyBuilder.CompactPositive(0.41234), "normal 4-decimal rounding");

            // Check MA-1 all-Sitka sparse species map (empty) vs MA-1-MIXED
            var ma1Eco = MultiAreaWorkPlanAdapter.OpenExperiment("ma-1").ReadCompositeUiSnapshot().Ecology!;
            T.Eq(0, ma1Eco.SpeciesByTreeId.Count, "all-Sitka MA-1 sparse speciesByTreeId is empty");
            T.True(ma1Eco.CellSchema.SequenceEqual(new[] { "areaIndex", "col", "row", "light", "bramble", "bracken", "speciesMask", "bandCount" }), "exact cellSchema");

            // Advance MA-1-MIXED 5 years and also plant+shelter+spotclear via Continuation to verify planted/shelter/patch projection
            var (exp, opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var c = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
            var pt = Phase2Support.FindPoint(c, "MA-B", 1.0f);
            var v = c.ReadView();
            v = c.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
            v = c.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            v = c.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            var pendingSnap = c.ReadCompositeUiSnapshot();
            T.Eq(3, pendingSnap.Ecology!.PendingPlanting.Count, "3 pending planting orders projected");
            v = c.Approve(Fence(v)).View;
            T.Eq(3, c.ReadCompositeUiSnapshot().Ecology!.ApprovedPlanting.Count, "3 approved planting orders projected");
            v = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;

            var postExec = c.ReadCompositeUiSnapshot().Ecology!;
            T.Eq(1, postExec.Planted.Count, "1 planted juvenile");
            T.Eq(1, postExec.Shelters.Count, "1 shelter");
            T.Eq(9, postExec.Shelters[0].ExpiryYear, "installedYear 1 + 8 = expiryYear 9");
            T.True(postExec.Shelters[0].IsEffective, "newly installed shelter is active");
            T.Eq(1, postExec.Patches.Count, "1 spot-clear patch");
            T.True(Math.Abs(postExec.Patches[0].RadiusM - 0.5641896f) < 0.001f, "patch radius ~0.564 m");

            // Advance 9 years so shelter expires (year 9 >= expiryYear 9) and natural bands establish
            for (int y = 0; y < 9; y++) v = c.Advance(Fence(v), false).View;
            var y9 = c.ReadCompositeUiSnapshot().Ecology!;
            T.True(!y9.Shelters[0].IsEffective, "shelter expired at year 9 (8+ yr)");
            T.True(y9.CellsCompact.Any(row => row[6] > 0 && row[7] > 0), "natural regeneration sets speciesMask and bandCount");
            T.True(y9.Areas.Sum(a => a.CellCounts.WithNatural) > 0, "area cellCounts.withNatural > 0");
            // Verify every non-zero cell value stayed strictly positive in cellsCompact
            var s5 = c.ReadStage5View();
            int rowIdx = 0;
            for (int ai = 0; ai < s5.Areas.Count; ai++)
            {
                foreach (var cell in s5.Areas[ai].Cells)
                {
                    var row = y9.CellsCompact[rowIdx++];
                    if (cell.Bramble > 0f) T.True(row[4] > 0.0, "positive bramble never rounded to 0");
                    if (cell.Bracken > 0f) T.True(row[5] > 0.0, "positive bracken never rounded to 0");
                }
            }
        });

        T.Add("MI-P1 C-04: species-aware prune readout gives Oak/Beech max 2 lifts (3.5/6.0 m, 3-yr wait) and Sitka max 3 lifts (2.5/5.0/6.5 m, 5-yr wait)", () =>
        {
            var adapter = MultiAreaWorkPlanAdapter.OpenExperiment("ma-1-mixed");
            var snap = adapter.ReadCompositeUiSnapshot();
            var eco = snap.Ecology!;
            string sitkaId = snap.View.Areas[1].Trees.First(t => !eco.SpeciesByTreeId.ContainsKey(t.TreeId)).TreeId;
            string oakId = snap.View.Areas[1].Trees.First(t => eco.SpeciesByTreeId.TryGetValue(t.TreeId, out var s) && s == Phase2Species.OakId).TreeId;
            string beechId = snap.View.Areas[1].Trees.First(t => eco.SpeciesByTreeId.TryGetValue(t.TreeId, out var s) && s == Phase2Species.BeechId).TreeId;

            var psSitka = eco.PruneStatusByTreeId[sitkaId];
            var psOak = eco.PruneStatusByTreeId[oakId];
            var psBeech = eco.PruneStatusByTreeId[beechId];
            T.Eq((true, 3, 5, (float?)2.5f), (psSitka.Eligible, psSitka.MaxLifts, psSitka.MinIntervalYears, psSitka.NextTargetM), "Sitka initial prune status");
            T.Eq((true, 2, 3, (float?)3.5f), (psOak.Eligible, psOak.MaxLifts, psOak.MinIntervalYears, psOak.NextTargetM), "Oak initial prune status");
            T.Eq((true, 2, 3, (float?)3.5f), (psBeech.Eligible, psBeech.MaxLifts, psBeech.MinIntervalYears, psBeech.NextTargetM), "Beech initial prune status");

            // Consistency between stage4.areas[].trees[] and ecology.pruneStatusByTreeId
            var s4Oak = snap.Readout.Areas[1].Trees.Single(t => t.TreeId == oakId);
            T.Eq((psOak.Eligible, psOak.MaxLifts, psOak.NextTargetM), (s4Oak.PruneEligible, s4Oak.MaxPruningLifts, s4Oak.NextPruneTargetM), "stage4 and ecology agree on Oak");

            // Execute lift 1 on Oak -> 3-yr wait -> lift 2 at 6.0 m -> max lifts reached
            var v = snap.View;
            v = adapter.AddPrune(Fence(v), "MA-B", oakId).View;
            T.Eq("PendingPrune", adapter.ReadCompositeUiSnapshot().Ecology!.PruneStatusByTreeId[oakId].PlanState, "PendingPrune reflected");
            v = adapter.Approve(Fence(v)).View;
            T.Eq("ApprovedPrune", adapter.ReadCompositeUiSnapshot().Ecology!.PruneStatusByTreeId[oakId].PlanState, "ApprovedPrune reflected");
            v = adapter.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;

            var afterLift1 = adapter.ReadCompositeUiSnapshot().Ecology!.PruneStatusByTreeId[oakId];
            T.Eq((false, 1, "PruneIntervalTooShort", (float?)6.0f, (int?)3),
                (afterLift1.Eligible, afterLift1.Lifts, afterLift1.RefusalCode!, afterLift1.NextTargetM, afterLift1.NextEligibleYear),
                "Oak after lift 1: 3-year wait, next target 6.0 m");

            for (int y = 0; y < 3; y++) v = adapter.Advance(Fence(v), false).View;
            var year3 = adapter.ReadCompositeUiSnapshot().Ecology!.PruneStatusByTreeId[oakId];
            T.True(year3.Eligible && year3.NextTargetM == 6.0f, "Oak eligible again after 3 years for 6.0 m lift");
            v = adapter.AddPrune(Fence(v), "MA-B", oakId).View;
            v = adapter.Approve(Fence(v)).View;
            v = adapter.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;

            for (int y = 0; y < 3; y++) v = adapter.Advance(Fence(v), false).View;
            var afterLift2 = adapter.ReadCompositeUiSnapshot().Ecology!.PruneStatusByTreeId[oakId];
            T.Eq((false, 2, "PruneMaxLiftsReached", (float?)null),
                (afterLift2.Eligible, afterLift2.Lifts, afterLift2.RefusalCode!, afterLift2.NextTargetM),
                "Oak after 2 lifts: max lifts reached, no third target");
        });

        T.Add("MI-P1 C-05: ReadCellDetail is strictly read-only, returns full cohorts/undergrowth/clear state on 200 and refuses stale/invalid cells with fresh snapshot", () =>
        {
            var (exp, opts, _) = MultiAreaDemoExperiments.CreateMixedDemo();
            var c = new MultiAreaContinuation(exp, MultiAreaModelSelection.Phase2, null, null, opts);
            var adapter = new MultiAreaWorkPlanAdapter(c, exp, "ma-1-mixed", MultiAreaDemoExperiments.Ma1MixedBadge);
            var v = adapter.Read();
            for (int y = 0; y < 3; y++) v = adapter.Advance(Fence(v), false).View;

            var snapBefore = adapter.ReadCompositeUiSnapshot();
            var cell0 = snapBefore.Readout.Areas[1].Ground[0];
            var res = adapter.ReadCellDetail(Fence(snapBefore.View), "MA-B", cell0.Column, cell0.Row);
            T.True(res.RefusalCode is null && res.Cell is not null, "valid cell detail succeeds");
            var detail = res.Cell!;
            T.Eq(("MA-B", cell0.Column, cell0.Row), (detail.AreaId, detail.Column, detail.Row), "cell coordinates");
            T.True(detail.Polygon.Count >= 3 && detail.ClippedAreaM2 > 0, "clipped polygon and area");
            T.True(detail.Bramble is not null && detail.Bracken is not null && detail.BrowsePressure is not null, "Stage 5 undergrowth & browse present");
            T.Eq("None", detail.ClearState, "no clear queued");
            T.Eq(snapBefore.View.Source, res.Snapshot.View.Source, "strictly read-only: source unchanged");
            T.Eq(snapBefore.View.PlanStateRevision, res.Snapshot.View.PlanStateRevision, "strictly read-only: planStateRevision unchanged");

            // Queue ClearUndergrowth via Continuation to confirm PendingClear -> ApprovedClear reporting
            var cv = c.AddClearUndergrowth(Fence(v), "MA-B", cell0.Column, cell0.Row).View;
            var resPending = adapter.ReadCellDetail(Fence(cv), "MA-B", cell0.Column, cell0.Row);
            T.True(resPending.RefusalCode is null && resPending.Cell!.ClearState == "PendingClear" && resPending.Cell.HasPendingClear, "PendingClear reported");
            cv = c.Approve(Fence(cv)).View;
            var resApproved = adapter.ReadCellDetail(Fence(cv), "MA-B", cell0.Column, cell0.Row);
            T.True(resApproved.RefusalCode is null && resApproved.Cell!.ClearState == "ApprovedClear" && resApproved.Cell.HasApprovedClear, "ApprovedClear reported");

            // Refusals: stale fence, invalid/out-of-bounds cell, wrong area, unknown area; all read-only with fresh snapshot
            int revBefore = cv.PlanStateRevision;
            var stale = adapter.ReadCellDetail(Fence(snapBefore.View), "MA-B", cell0.Column, cell0.Row);
            T.Eq("PlayerPlanStateStale", stale.RefusalCode, "stale fence refused");
            T.True(stale.Cell is null && stale.Snapshot.View.PlanStateRevision == revBefore, "stale refusal returns fresh view and zero mutation");

            var oob = adapter.ReadCellDetail(Fence(cv), "MA-B", 9999, 9999);
            T.Eq("CellNotInArea", oob.RefusalCode, "out-of-bounds cell refused");
            T.True(oob.Cell is null && oob.Snapshot.View.PlanStateRevision == revBefore, "OOB refusal zero mutation");

            var cellA = snapBefore.Readout.Areas[0].Ground[0];
            var wrongArea = adapter.ReadCellDetail(Fence(cv), "MA-B", cellA.Column, cellA.Row);
            T.Eq("MultiAreaWrongArea", wrongArea.RefusalCode, "cell of MA-A requested under MA-B refused as MultiAreaWrongArea");

            var unknownArea = adapter.ReadCellDetail(Fence(cv), "MA-Z", cell0.Column, cell0.Row);
            T.Eq("MultiAreaUnknownArea", unknownArea.RefusalCode, "unknown area refused");
            T.Eq(revBefore, adapter.Read().PlanStateRevision, "all cell reads left planStateRevision completely untouched");
        });

        T.Add("MI-P1 C-06: raw UTF-8 JSON payload budgets hold on MA-1 and MA-1-MIXED at Year 0 and Year +5 (view <= 512 KiB, ecology <= 150 KiB)", () =>
        {
            var labels = new[] { typeof(PlayerLabels), typeof(CropLabels), typeof(MultiAreaLabels) }
                .SelectMany(t => t.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
                .Where(f => f.IsLiteral && f.FieldType == typeof(string))
                .ToDictionary(f => f.Name, f => (string)f.GetRawConstantValue()!, StringComparer.Ordinal);

            foreach (string key in new[] { "ma-1", "ma-1-mixed" })
            {
                var adapter = MultiAreaWorkPlanAdapter.OpenExperiment(key);
                for (int targetYear = 0; targetYear <= 5; targetYear += 5)
                {
                    if (targetYear == 5)
                    {
                        var v = adapter.Read();
                        for (int y = 0; y < 5; y++) v = adapter.Advance(Fence(v), false).View;
                    }
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
                        $"  MI-P1 payload {key} Year {targetYear}: total {totalBytes.Length} B ({totalBytes.Length / 1024.0:0.00} KiB), ecology {ecoBytes.Length} B ({ecoBytes.Length / 1024.0:0.00} KiB)"));
                    T.True(totalBytes.Length <= MaxViewBytes, $"{key} Year {targetYear} total {totalBytes.Length} <= {MaxViewBytes}");
                    T.True(ecoBytes.Length <= MaxEcologyBytes, $"{key} Year {targetYear} ecology {ecoBytes.Length} <= {MaxEcologyBytes}");
                }
            }
        });
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
}
