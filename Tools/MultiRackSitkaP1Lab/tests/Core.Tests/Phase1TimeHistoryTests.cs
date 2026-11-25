using System.Reflection;
using System.Text.RegularExpressions;
using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Core.Tests;

/// <summary>
/// CCF Multi 2D — Phase 1 Step 1 (spec multi-2d-phase1-time-history-core-v1) verification suite:
/// P1-TH-01..08 covering dual time pacing, skip-vs-manual parity, WorkPlanOpen/bounds/cancellation refusals,
/// SchemaVersion 2 Save/Load continuity, read-only Decision -> Outcome history query, WoodlandStructureMetrics
/// analytical oracles, Gate G3 honesty assertions, and hermetic Core boundary invariants.
/// </summary>
internal static class Phase1TimeHistoryTests
{
    private const double Tol = 1e-12;

    private static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.NonPublic | BindingFlags.Instance)!;

    private static readonly Regex PlanIdSessionPrefix =
        new(@"\A[0-9a-f]{32}:ma-plan:", RegexOptions.CultureInvariant);

    internal static void Register()
    {
        // ------------------------------------------------------------------ P1-TH-01: Preview cadence & fence checks
        T.Add("P1-TH-01: pre-Fell observation preview, post-Fell anchoring, overdue fallback, non-Fell work & stale fence", () =>
        {
            var session = MixedForest();
            var v0 = session.ReadView();

            // Pre-first-Fell Year 0 observation preview.
            var p0 = session.PreviewNextReviewYear(Fence(v0));
            T.Eq(0, p0.CurrentYear, "year 0 current year");
            T.True(p0.Eligible, "year 0 eligible");
            T.True(p0.StopReason is null, "year 0 stop reason null");
            T.True(p0.AnchorFellYear is null, "year 0 anchor null");
            T.True(p0.IsObservationPeriod, "year 0 observation period");
            T.Eq(4, p0.SuggestedTargetYear, "year 0 suggested target = current + 4");
            T.Eq(4, p0.SuggestedGapYears, "suggested gap = 4");
            T.Eq(5, p0.MaxTicksPerPress, "max ticks = 5");
            T.True(p0.CalibrationNote.Contains("GAMEPLAY CALIBRATION", StringComparison.Ordinal), "calibration note");
            T.True(p0.Label.Contains("observation", StringComparison.OrdinalIgnoreCase) &&
                p0.Label.Contains("not a completed silvicultural thinning cycle", StringComparison.OrdinalIgnoreCase),
                "quiet observation label: " + p0.Label);

            // Non-Fell work (Plant only) executes a MultiAreaWorkEvent with 0 Fells -> must NOT set a Fell anchor.
            var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);
            var v = session.AddPlant(Fence(v0), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            v = session.Approve(Fence(v)).View;
            v = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            T.Eq(1, v.Events.Count, "non-Fell work event committed");
            var pAfterNonFell = session.PreviewNextReviewYear(Fence(v));
            T.True(pAfterNonFell.AnchorFellYear is null, "non-Fell work sets no Fell anchor");
            T.True(pAfterNonFell.IsObservationPeriod, "still observation period after non-Fell work");
            T.Eq(4, pAfterNonFell.SuggestedTargetYear, "still current + 4");

            // Advance to Year 1, execute a real Fell in Year 1 -> anchors to 1, suggests 5 (1 + 4).
            v = session.Advance(Fence(v), consentToCancelWork: false).View;
            T.Eq(1, v.Source.EcologicalYear, "at year 1");
            v = ExecuteSingleFell(session, "MA-A", "first thinning anchor");
            var pAfterFellY1 = session.PreviewNextReviewYear(Fence(v));
            T.Eq(1, pAfterFellY1.CurrentYear, "year 1");
            T.True(pAfterFellY1.Eligible, "eligible after execute");
            T.Eq(1, pAfterFellY1.AnchorFellYear, "anchored to Fell year 1");
            T.True(!pAfterFellY1.IsObservationPeriod, "no longer observation period");
            T.Eq(5, pAfterFellY1.SuggestedTargetYear, "target = anchor(1) + 4 = 5");
            T.Eq(5, pAfterFellY1.MaxTicksPerPress, "max ticks stays 5");

            // Advance 1 year to Year 2 -> still anchored to 1, target still 5 (3 years ahead).
            v = session.Advance(Fence(v), consentToCancelWork: false).View;
            var pY2 = session.PreviewNextReviewYear(Fence(v));
            T.Eq(2, pY2.CurrentYear, "year 2");
            T.Eq(1, pY2.AnchorFellYear, "anchor stays 1");
            T.Eq(5, pY2.SuggestedTargetYear, "target stays 5");

            // Advance to Year 5 (anchor + 4 == 5 <= currentYear 5 -> overdue fallback = currentYear + 4 = 9).
            var adv = session.AdvanceYears(Fence(v), 5);
            T.True(adv.ReachedTarget && adv.StopReason == TimePacingStopReason.ReachedReviewYear, "reached year 5");
            var pY5 = session.PreviewNextReviewYear(adv.FreshFence);
            T.Eq(5, pY5.CurrentYear, "year 5");
            T.Eq(1, pY5.AnchorFellYear, "anchor still 1");
            T.True(!pY5.IsObservationPeriod, "not observation");
            T.Eq(9, pY5.SuggestedTargetYear, "overdue anchor + 4 <= 5 falls back to 5 + 4 = 9");

            // Advance 1 more year to Year 6 (anchor + 4 == 5 < 6 -> strictly overdue fallback = 10).
            v = session.Advance(adv.FreshFence, consentToCancelWork: false).View;
            var pY6 = session.PreviewNextReviewYear(Fence(v));
            T.Eq(6, pY6.CurrentYear, "year 6");
            T.Eq(10, pY6.SuggestedTargetYear, "overdue anchor + 4 < 6 falls back to 6 + 4 = 10");

            // Second Fell in Year 6 updates anchor to 6 -> suggests 10 (6 + 4).
            v = ExecuteSingleFell(session, "MA-A", "second thinning anchor");
            var pAfterFellY6 = session.PreviewNextReviewYear(Fence(v));
            T.Eq(6, pAfterFellY6.AnchorFellYear, "latest Fell updates anchor to 6");
            T.Eq(10, pAfterFellY6.SuggestedTargetYear, "target = 6 + 4 = 10");

            // Stale fence refusals (PlanStateRevision, Source token, SessionId) with zero mutation.
            var beforeCropFence = Fence(v);
            string cropTree = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
            var afterCropView = session.MarkCrop(beforeCropFence, "MA-A", cropTree).View;
            byte[] snapBeforeRefusal = Canonical(session);

            T.Refuses("PlayerPlanStateStale", () => session.PreviewNextReviewYear(beforeCropFence));
            T.Refuses("PlayerPlanStateStale", () => session.AdvanceYears(beforeCropFence, 10));

            var staleSource = new MultiAreaFence(afterCropView.SessionId,
                afterCropView.Source with { WorkRevision = afterCropView.Source.WorkRevision + 1 },
                afterCropView.PlanStateRevision);
            T.Refuses("PlayerSourceStale", () => session.PreviewNextReviewYear(staleSource));
            T.Refuses("PlayerSourceStale", () => session.AdvanceYears(staleSource, 10));

            var wrongSession = new MultiAreaFence("00000000000000000000000000000000",
                afterCropView.Source, afterCropView.PlanStateRevision);
            T.Refuses("PlayerSessionMismatch", () => session.PreviewNextReviewYear(wrongSession));
            T.Refuses("PlayerSessionMismatch", () => session.AdvanceYears(wrongSession, 10));

            AssertBytes(snapBeforeRefusal, Canonical(session), "stale fence refusals mutate nothing");
        });

        // ------------------------------------------------------------------ P1-TH-02: Dual skip-vs-manual parity
        T.Add("P1-TH-02: skip-vs-manual full-root parity on both MA-1 and MA-1-MIXED (pre-Fell and post-Fell)", () =>
        {
            VerifySkipParity("MA-1", Ma1Forest);
            VerifySkipParity("MA-1-MIXED", MixedForest);
        });

        // ------------------------------------------------------------------ P1-TH-03: WorkPlanOpen, bounds, StandCannotAdvance, CancellationToken
        T.Add("P1-TH-03a: WorkPlanOpen refused with zero mutation for Pending and Approved of all 6 order types", () =>
        {
            VerifyWorkPlanOpenRefused("Fell", (s, v) =>
                s.AddPending(Fence(v), "MA-A", v.Areas[0].Trees.First(t => t.IsLiving).TreeId, "block skip").View);

            VerifyWorkPlanOpenRefused("Prune", (s, v) =>
                s.AddScheduledPrune(Fence(v), "MA-B", FirstOakId(s)).View);

            VerifyWorkPlanOpenRefused("ClearUndergrowth", (s, v) =>
            {
                var cell = s.ReadCompositeUiSnapshot().Readout.Areas[1].Ground[0];
                return s.AddClearUndergrowth(Fence(v), "MA-B", cell.Column, cell.Row).View;
            });

            VerifyWorkPlanOpenRefused("Plant", (s, v) =>
            {
                var pt = Phase2Support.FindPoint(s, "MA-B", 1.0f);
                return s.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
            });

            VerifyWorkPlanOpenRefused("Shelter", (s, v) =>
            {
                // Execute a Plant first so a standalone Shelter order can be queued without a Plant in the same plan.
                var pt = Phase2Support.FindPoint(s, "MA-B", 1.0f);
                v = s.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
                v = s.Approve(Fence(v)).View;
                v = s.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
                return s.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
            });

            VerifyWorkPlanOpenRefused("SpotClear", (s, v) =>
            {
                var pt = Phase2Support.FindPoint(s, "MA-B", 1.0f);
                return s.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
            });
        });

        T.Add("P1-TH-03b: InvalidTargetYear, StandCannotAdvance and between-step CancellationToken", () =>
        {
            var session = MixedForest();
            var v = session.Advance(Fence(session.ReadView()), consentToCancelWork: false).View; // at Year 1
            byte[] beforeInvalid = Canonical(session);

            foreach (int badTarget in new[] { 1, 0, -5, 7, 100, int.MaxValue, int.MinValue })
            {
                var res = session.AdvanceYears(Fence(v), badTarget);
                T.Eq(TimePacingStopReason.InvalidTargetYear, res.StopReason, "invalid target " + badTarget);
                T.True(!res.ReachedTarget, "not reached for " + badTarget);
                T.Eq(1, res.StartYear, "start year");
                T.Eq(1, res.DestinationYear, "destination unchanged");
                T.Eq(0, res.CompletedYears.Count, "no completed years");
                AssertBytes(beforeInvalid, Canonical(session), "zero mutation for target " + badTarget);
            }

            // Pre-cancelled CancellationToken before step 0 -> CancelledBetweenYears, 0 completed, zero mutation.
            using (var preCancelled = new CancellationTokenSource())
            {
                preCancelled.Cancel();
                var res = session.AdvanceYears(Fence(v), 5, preCancelled.Token);
                T.Eq(TimePacingStopReason.CancelledBetweenYears, res.StopReason, "pre-cancelled stop reason");
                T.True(!res.ReachedTarget, "pre-cancelled not reached");
                T.Eq(1, res.DestinationYear, "pre-cancelled year unchanged");
                T.Eq(0, res.CompletedYears.Count, "pre-cancelled 0 completed");
                AssertBytes(beforeInvalid, Canonical(session), "pre-cancelled zero mutation");
            }

            // Mid-run cancellation strictly between annual steps (request 0 -> 5, cancel after Year 2 commits).
            var cancelSession = MixedForest();
            var control2Years = MixedForest();
            for (int y = 0; y < 2; y++)
                control2Years.Advance(Fence(control2Years.ReadView()), consentToCancelWork: false);

            using (var midCts = new CancellationTokenSource())
            {
                var res = cancelSession.AdvanceYears(Fence(cancelSession.ReadView()), 5, midCts.Token,
                    afterYearCommitted: yearAfter => { if (yearAfter == 2) midCts.Cancel(); });
                T.Eq(TimePacingStopReason.CancelledBetweenYears, res.StopReason, "mid-run cancelled");
                T.True(!res.ReachedTarget, "mid-run not reached");
                T.Eq(0, res.StartYear, "start 0");
                T.Eq(2, res.DestinationYear, "stopped cleanly at 2");
                T.Eq(5, res.RequestedTargetYear, "requested 5");
                T.Eq(2, res.CompletedYears.Count, "exactly 2 completed years");
                T.Eq(1, res.CompletedYears[0].YearAfter, "step 1 was year 1");
                T.Eq(2, res.CompletedYears[1].YearAfter, "step 2 was year 2");
                T.Eq(2, res.FreshFence.Source.EcologicalYear, "fresh fence year = 2");
                T.Eq(2, cancelSession.ReadView().AnnualHistory.Count, "zero phantom remainder records");
                AssertBytes(Canonical(control2Years), Canonical(cancelSession), "cancelled state matches 2-year manual control");
            }

            // StandCannotAdvance when every area has no living stems and no juvenile regeneration (Model 0 empty stand).
            var emptySession = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var originalRoot = GetRoot(emptySession);
            var deadAreas = originalRoot.Areas.Select(a => a with
            {
                State = new AnnualState(a.State.EcologicalYear, Array.AsReadOnly(a.State.Trees
                    .Select(t => t with { Death = new AnnualDeath(t.TreeId, 0, "DensitySuppression", "density") }).ToArray())),
            }).ToArray();
            SetRoot(emptySession, originalRoot with { Areas = Array.AsReadOnly(deadAreas) });
            var emptyView = emptySession.ReadView();
            var emptyPreview = emptySession.PreviewNextReviewYear(Fence(emptyView));
            T.True(!emptyPreview.Eligible, "empty stand preview not eligible");
            T.Eq(TimePacingStopReason.StandCannotAdvance, emptyPreview.StopReason, "empty stand preview reason");
            var emptyRes = emptySession.AdvanceYears(Fence(emptyView), 4);
            T.Eq(TimePacingStopReason.StandCannotAdvance, emptyRes.StopReason, "empty stand advance stop reason");
            T.True(!emptyRes.ReachedTarget, "empty stand not reached");
            T.Eq(0, emptyRes.DestinationYear, "empty stand stayed at year 0");
            T.Eq(0, emptyRes.CompletedYears.Count, "empty stand 0 completed");
            T.Eq(0, emptySession.ReadView().AnnualHistory.Count, "empty stand added no history");
        });

        // ------------------------------------------------------------------ P1-TH-04: Save -> Load continuity (SchemaVersion 2)
        T.Add("P1-TH-04: SchemaVersion 2 Save -> Load continuity (pre-Fell and post-Fell) matches uninterrupted run", () =>
        {
            T.Eq(2, Phase1SaveFormat.SchemaVersion, "save schema version stays 2");

            // Case A: Pre-Fell Save/Load continuity after 2 observation years.
            var preControl = MixedForest();
            var preSource = MixedForest();
            var preStepA = preControl.AdvanceYears(Fence(preControl.ReadView()), 2);
            var preStepB = preSource.AdvanceYears(Fence(preSource.ReadView()), 2);
            T.True(preStepA.ReachedTarget && preStepB.ReachedTarget, "pre-Fell 2-year setup");

            var preLoaded = SaveAndRestoreIntoFreshMixed(preSource, "p1-th-04-prefell");
            AssertContinuityEqual("pre-Fell at Y2", preControl, preLoaded, new[] { 0, 1, 2 });

            var preAdvCtrl = preControl.AdvanceYears(Fence(preControl.ReadView()), 6);
            var preAdvLoad = preLoaded.AdvanceYears(Fence(preLoaded.ReadView()), 6);
            T.Eq(preAdvCtrl.DestinationYear, preAdvLoad.DestinationYear, "pre-Fell continued destination");
            T.Eq(preAdvCtrl.StopReason, preAdvLoad.StopReason, "pre-Fell continued stop reason");
            T.Eq(preAdvCtrl.IsObservationPeriod, preAdvLoad.IsObservationPeriod, "pre-Fell still observation");
            AssertBytes(Canonical(preControl), Canonical(preLoaded), "pre-Fell post-skip root identical");
            AssertContinuityEqual("pre-Fell at Y6", preControl, preLoaded, new[] { 0, 2, 6 });

            // Case B: Post-Fell Save/Load continuity (all 6 action types in Y0, advance 2 years, save/load, skip to Y4).
            var postControl = MixedForest();
            var postSource = MixedForest();
            ExecuteAllSixActionTypesInYear(postControl);
            ExecuteAllSixActionTypesInYear(postSource);
            postControl.AdvanceYears(Fence(postControl.ReadView()), 2);
            postSource.AdvanceYears(Fence(postSource.ReadView()), 2);

            var postLoaded = SaveAndRestoreIntoFreshMixed(postSource, "p1-th-04-postfell");
            AssertContinuityEqual("post-Fell at Y2", postControl, postLoaded, new[] { 0, 1, 2 });

            var postPreviewCtrl = postControl.PreviewNextReviewYear(Fence(postControl.ReadView()));
            var postPreviewLoad = postLoaded.PreviewNextReviewYear(Fence(postLoaded.ReadView()));
            T.Eq(0, postPreviewLoad.AnchorFellYear, "restored Fell anchor is Year 0");
            T.Eq(4, postPreviewLoad.SuggestedTargetYear, "restored suggested target is Year 4");
            T.Eq(postPreviewCtrl, postPreviewLoad, "post-Fell preview identical");

            var postAdvCtrl = postControl.AdvanceYears(Fence(postControl.ReadView()), postPreviewCtrl.SuggestedTargetYear);
            var postAdvLoad = postLoaded.AdvanceYears(Fence(postLoaded.ReadView()), postPreviewLoad.SuggestedTargetYear);
            T.Eq(4, postAdvLoad.DestinationYear, "post-Fell reached Year 4");
            T.Eq(postAdvCtrl.StopReason, postAdvLoad.StopReason, "post-Fell stop reason matches");
            AssertBytes(Canonical(postControl), Canonical(postLoaded), "post-Fell post-skip root identical");
            AssertContinuityEqual("post-Fell at Y4", postControl, postLoaded, new[] { 0, 1, 2, 3, 4 });
        });

        // ------------------------------------------------------------------ P1-TH-05: Read-only History Query
        T.Add("P1-TH-05: newest-first cursor-stable paging (>=15y), WorkYear vs ObservedYear, 6 provenance labels, NOT RECORDED, no weather/economy", () =>
        {
            var session = MixedForest();

            // Before any work or advance: 0 recorded years.
            var emptyIdx = session.ReadHistoryIndex();
            T.Eq(0, emptyIdx.TotalRecordedYears, "year 0 untouched has 0 recorded years");
            T.Eq(0, emptyIdx.Items.Count, "no index rows");
            T.True(emptyIdx.NextCursorBeforeYear is null, "no cursor");

            // Execute all 6 action types in Year 0 BEFORE advancing -> Year 0 has work, NO annual outcome!
            ExecuteAllSixActionTypesInYear(session);
            var preAdvIdx = session.ReadHistoryIndex();
            T.Eq(1, preAdvIdx.TotalRecordedYears, "1 recorded year (Year 0 work only)");
            T.Eq(0, preAdvIdx.Items[0].Year, "Year 0 listed");
            T.True(preAdvIdx.Items[0].HasExecutedWork, "Year 0 has work");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.Fell, "1 Fell");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.Prune, "1 Prune");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.ClearUndergrowth, "1 ClearUndergrowth");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.Plant, "1 Plant");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.Shelter, "1 Shelter");
            T.Eq(1, preAdvIdx.Items[0].ExecutedActionCounts.SpotClear, "1 SpotClear");
            T.True(!preAdvIdx.Items[0].HasAnnualOutcome, "Year 0 before advance has no annual outcome");
            T.True(preAdvIdx.Items[0].LiveStemsEndOfYear is null, "no invented LiveStemsEndOfYear");

            // Advance 16 years (with a second Fell in Year 6) to build >= 15 years of recorded history (Years 0..16).
            var f = Fence(session.ReadView());
            f = session.AdvanceYears(f, 5).FreshFence;
            f = session.AdvanceYears(f, 6).FreshFence;
            var y6View = ExecuteSingleFell(session, "MA-A", "year 6 thinning");
            f = Fence(y6View);
            f = session.AdvanceYears(f, 11).FreshFence;
            f = session.AdvanceYears(f, 16).FreshFence;
            T.Eq(16, f.Source.EcologicalYear, "reached year 16");

            byte[] beforeRead = Canonical(session);

            // Walk all pages newest-first with pageSize = 5.
            var visited = new List<HistoryIndexEntry>();
            int? cursor = null;
            int pageCount = 0;
            do
            {
                var page = session.ReadHistoryIndex(pageSize: 5, beforeYear: cursor);
                T.Eq(17, page.TotalRecordedYears, "total recorded years = 17 (16..0)");
                T.True(page.Items.Count <= 5, "page bounded by pageSize");
                visited.AddRange(page.Items);
                cursor = page.NextCursorBeforeYear;
                pageCount++;
            } while (cursor is not null);

            T.Eq(4, pageCount, "17 years / pageSize 5 = 4 pages");
            T.Eq(17, visited.Count, "all 17 years visited");
            for (int i = 0; i < visited.Count; i++)
                T.Eq(16 - i, visited[i].Year, "strictly newest-first order at index " + i);

            // Verify Year 16..1 have HasAnnualOutcome = true & non-null LiveStemsEndOfYear; Year 0 has no YearAfter == 0 record.
            T.True(visited[0].HasAnnualOutcome && visited[0].LiveStemsEndOfYear is > 0, "Year 16 has annual outcome");
            var y0IndexRow = visited[^1];
            T.Eq(0, y0IndexRow.Year, "last row is Year 0");
            T.True(y0IndexRow.HasExecutedWork && y0IndexRow.HasObservedOutcomeNextYear, "Year 0 has work and Year 1 observed outcome");
            T.True(!y0IndexRow.HasAnnualOutcome && y0IndexRow.LiveStemsEndOfYear is null, "Year 0 has no YearAfter==0 outcome");

            // Cursor stability across appended years: take page 1 cursor at Year 16, advance 2 more years (17, 18), then read page 2.
            var stablePage1 = session.ReadHistoryIndex(pageSize: 5);
            T.Eq("16,15,14,13,12", string.Join(",", stablePage1.Items.Select(x => x.Year)), "page 1 before new years");
            T.Eq(12, stablePage1.NextCursorBeforeYear, "cursor = 12");
            session.AdvanceYears(Fence(session.ReadView()), 18);
            var stablePage2 = session.ReadHistoryIndex(pageSize: 5, beforeYear: stablePage1.NextCursorBeforeYear);
            T.Eq("11,10,9,8,7", string.Join(",", stablePage2.Items.Select(x => x.Year)), "page 2 cursor-stable after new years appended");

            // WorkYear = 0 vs ObservedYear = 1 timing (never relabel Year 0 cuts as Year 1!).
            var d0 = session.ReadHistoryYear(0);
            T.Eq(0, d0.WorkYear, "d0 WorkYear = 0");
            T.Eq(1, d0.ObservedYear, "d0 ObservedYear = 1");
            T.Eq(18, d0.CurrentEcologicalYear, "current year = 18");
            T.Eq(HistoryProvenance.Recorded, d0.WorkProvenance, "work provenance RECORDED");
            T.Eq(6, d0.TotalWorkCounts.Total, "all 6 Year-0 actions recorded in Year 0");
            T.True(d0.AnnualRecordEndingInYear is null, "Year 0 has no AnnualRecordEndingInYear");
            T.True(d0.AnnualOutcomeNextYear is not null, "Year 0 has ObservedYear = 1 outcome");
            T.Eq(0, d0.AnnualOutcomeNextYear!.YearBefore, "outcome YearBefore = 0");
            T.Eq(1, d0.AnnualOutcomeNextYear.YearAfter, "outcome YearAfter = 1");
            T.Eq(HistoryProvenance.Recorded, d0.AnnualOutcomeNextYear.Provenance, "outcome RECORDED");
            T.True(d0.WorkByArea.Any(a => a.AreaId == "MA-A" && a.Fells.Count == 1 && a.ActionGroups.Any(g => g.ActionType == "Fell" && g.WorkYear == 0 && g.TreeIds.Count == 1)),
                "MA-A Year-0 Fell recorded with TreeId");
            T.True(d0.WorkByArea.Any(a => a.AreaId == "MA-B" && a.Prunes.Count == 1 && a.Clears.Count == 1 && a.PlantingWork.Count == 3),
                "MA-B Year-0 Prune/Clear/Plant/Shelter/SpotClear recorded");
            T.Eq(1, d0.PlantedWorkFollowUp.Count, "reconstructed follow-up for Year-0 planted juvenile");
            T.Eq(HistoryProvenance.Reconstructed, d0.PlantedWorkFollowUp[0].Provenance, "follow-up is RECONSTRUCTED");

            // Year 1 must NOT show Year 0's cuts; it holds Year 0->1 as AnnualRecordEndingInYear and 1->2 as AnnualOutcomeNextYear.
            var d1 = session.ReadHistoryYear(1);
            T.Eq(1, d1.WorkYear, "d1 WorkYear = 1");
            T.Eq(2, d1.ObservedYear, "d1 ObservedYear = 2");
            T.Eq(0, d1.TotalWorkCounts.Total, "Year 0 work never relabelled as Year 1");
            T.True(d1.AnnualRecordEndingInYear is { YearBefore: 0, YearAfter: 1 }, "d1 ending record = 0->1");
            T.True(d1.AnnualOutcomeNextYear is { YearBefore: 1, YearAfter: 2 }, "d1 next observed = 1->2");

            // Year 6 has BOTH ending record (5->6) and next observed (6->7) plus its own Year-6 Fell.
            var d6 = session.ReadHistoryYear(6);
            T.Eq(1, d6.TotalWorkCounts.Fell, "Year 6 Fell recorded in Year 6");
            T.True(d6.AnnualRecordEndingInYear is { YearBefore: 5, YearAfter: 6 }, "d6 ending = 5->6");
            T.True(d6.AnnualOutcomeNextYear is { YearBefore: 6, YearAfter: 7 }, "d6 next observed = 6->7");

            // Whole-woodland Phase2PlantedAccount exposed honestly; per-area split marked NOT RECORDED.
            T.True(d0.AnnualOutcomeNextYear.WholeWoodlandPlantedAccounts.Count > 0, "whole-woodland planted accounts present");
            T.True(d0.AnnualOutcomeNextYear.WholeWoodlandPlantedAccounts.All(p => p.Scope == "WholeWoodland" && p.Provenance == HistoryProvenance.Recorded),
                "whole-woodland scope and RECORDED provenance");
            T.Eq(HistoryProvenance.NotRecorded, d0.AnnualOutcomeNextYear.PerAreaPlantedSplitProvenance,
                "per-area planted split is NOT RECORDED");

            // All 6 provenance values and D-073 tier mappings.
            var allProv = Enum.GetValues<HistoryProvenance>();
            T.Eq(6, allProv.Length, "exact 6 provenance values");
            T.Eq("RECORDED,CURRENT,DERIVED,RECONSTRUCTED,SESSION-ONLY,NOT RECORDED",
                string.Join(",", allProv.Select(HistoryProvenanceCatalog.LabelOf)), "exact 6 provenance labels");
            T.Eq(6, d0.ProvenanceGlossary.Count, "glossary lists all 6 provenance tiers");
            T.True(d0.ProvenanceGlossary.All(g => !string.IsNullOrWhiteSpace(g.D073Tier)), "all mapped to D-073 tiers");

            // Mandatory NOT RECORDED disclosures.
            var notRecKeys = d0.NotRecordedDisclosures.Select(n => n.FieldKey).ToHashSet(StringComparer.Ordinal);
            T.True(notRecKeys.Contains(MultiAreaHistoryQuery.KeyPastOverheadCanopyCover), "past canopy NOT RECORDED");
            T.True(notRecKeys.Contains(MultiAreaHistoryQuery.KeyPastTreeByTreeStructure), "past tree structure NOT RECORDED");
            T.True(notRecKeys.Contains(MultiAreaHistoryQuery.KeyPerAreaPlantedAccountSplits), "per-area planted splits NOT RECORDED");
            T.True(d0.NotRecordedDisclosures.All(n => n.Provenance == HistoryProvenance.NotRecorded && n.ProvenanceLabel == "NOT RECORDED"),
                "all marked NOT RECORDED");

            // Later observed comparison: CURRENT + DERIVED, exact non-causal statement, no weather claim.
            var comp = d0.LaterObservedComparison;
            T.Eq(HistoryProvenance.Current, comp.CurrentProvenance, "current provenance");
            T.Eq(HistoryProvenance.Derived, comp.DeltaProvenance, "delta provenance");
            T.Eq(MultiAreaHistoryLabels.NonCausalComparisonStatement, comp.NonCausalStatement, "non-causal statement");
            T.True(!comp.NonCausalStatement.Contains("weather", StringComparison.OrdinalIgnoreCase), "no weather claim");
            T.True(comp.TotalCurrentLiveStems > 0 && comp.TotalCurrentBasalAreaM2 > 0, "current live stems and basal area");
            T.True(comp.Areas.All(a => a.LiveStemDeltaSinceObservedYear is not null && a.BasalAreaM2PerHaDeltaSinceObservedYear is not null),
                "derived deltas present");

            // Zero economy / money / price / objective / euro / weather properties on any History DTO.
            var forbiddenPropertyWords = new[] { "Money", "Price", "Cost", "Cash", "Euro", "Revenue", "Budget", "Objective", "Weather" };
            var historyDtoTypes = new[]
            {
                typeof(HistoryIndexPage), typeof(HistoryIndexEntry), typeof(ExecutedActionCounts),
                typeof(HistoryYearDetail), typeof(HistoryAreaWorkRecord), typeof(HistoryActionGroup),
                typeof(HistoryFellDetail), typeof(HistoryPruneDetail), typeof(HistoryClearDetail), typeof(HistoryPlantingDetail),
                typeof(HistoryAnnualOutcome), typeof(HistoryAreaAnnualOutcome), typeof(HistoryAreaNaturalRegenOutcome),
                typeof(HistoryPlantedSpeciesAnnualAccount), typeof(HistoryPlantedWorkFollowUp),
                typeof(HistoryCurrentComparison), typeof(HistoryAreaCurrentObservation),
                typeof(HistoryNotRecordedDisclosure), typeof(HistoryProvenanceDescriptor),
            };
            foreach (var dtoType in historyDtoTypes)
            {
                foreach (var prop in dtoType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    foreach (var word in forbiddenPropertyWords)
                    {
                        T.True(!prop.Name.Contains(word, StringComparison.OrdinalIgnoreCase),
                            $"forbidden property {dtoType.Name}.{prop.Name} contains {word}");
                    }
                }
            }
            // Zero € placeholders or weather mentions in history labels.
            foreach (var text in new[]
            {
                MultiAreaHistoryLabels.NonCausalComparisonStatement,
                MultiAreaHistoryLabels.PastOverheadCanopyNotRecorded,
                MultiAreaHistoryLabels.PastTreeStructureNotRecorded,
                MultiAreaHistoryLabels.PerAreaPlantedSplitNotRecorded,
            })
            {
                T.True(!text.Contains('€') && !text.Contains("weather", StringComparison.OrdinalIgnoreCase),
                    "no euro/weather in label: " + text);
            }

            // Invalid inputs refused cleanly.
            T.Refuses("HistoryPageSizeInvalid", () => session.ReadHistoryIndex(pageSize: 0));
            T.Refuses("HistoryYearInvalid", () => session.ReadHistoryYear(-1));

            // Read-only purity on Year 18 state.
            byte[] beforeReadY18 = Canonical(session);
            _ = session.ReadHistoryIndex(pageSize: 3);
            _ = session.ReadHistoryYear(0);
            _ = session.ReadHistoryYear(6);
            AssertBytes(beforeReadY18, Canonical(session), "history queries mutate nothing");
            T.True(beforeRead.Length > 0, "pre-read capture valid");
        });

        // ------------------------------------------------------------------ P1-TH-06: WoodlandStructureMetrics oracles
        T.Add("P1-TH-06a: pure Sitka M_i = 0, analytical 4-tree & 6-tree mixed fixtures, cross-rack neighbours & purity", () =>
        {
            // Pure Sitka (Phase2 == null -> Model 0; and Phase 2 all-Sitka MA-1).
            var pureModel0 = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            var s0 = pureModel0.ComputeStructure();
            T.Eq(0.0, s0.MeanMingling!.Value, "pure Sitka Model 0 M_i == 0.0");
            T.True(s0.Areas.All(a => a.MeanMingling == 0.0), "all Model 0 areas M_i == 0.0");
            T.True(s0.MeanDiameterDifferentiation is >= 0.0 and <= 1.0, "T_i in [0,1]");
            T.Eq(3, s0.MaxNeighbourCountK, "max K = 3");
            T.Eq(3, s0.EffectiveNeighbourCount, "effective K = 3");
            T.Eq(HistoryProvenance.Derived, s0.Provenance, "DERIVED");

            var purePhase2 = Ma1Forest();
            var sPure2 = purePhase2.ComputeStructure();
            T.Eq(0.0, sPure2.MeanMingling!.Value, "all-Sitka Phase 2 M_i == 0.0");

            // 4-tree analytical fixture (k = 3, cross-rack).
            var root4 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[] { LiveTree("MA-A-001", 1f, 1f, 20f), LiveTree("MA-A-002", 2f, 1f, 40f) }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 4f, 1f, 20f), LiveTree("MA-B-002", 5f, 1f, 10f) }),
                },
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["MA-A-002"] = Phase2Species.OakId,
                    ["MA-B-001"] = Phase2Species.BeechId,
                });
            var m4 = WoodlandStructureMetrics.Compute(root4);
            T.Eq(4, m4.FocalLivingTreeCount, "4 living trees");
            T.Eq(3, m4.EffectiveNeighbourCount, "k = 3");
            T.Eq(3, m4.MaxNeighbourCountK, "maxK = 3");
            AssertNear(5.0 / 6.0, m4.MeanMingling, "4-tree woodland M");
            AssertNear(11.0 / 24.0, m4.MeanDiameterDifferentiation, "4-tree woodland T");
            AssertNear(5.0 / 6.0, m4.Areas[0].MeanMingling, "4-tree MA-A M");
            AssertNear(11.0 / 24.0, m4.Areas[0].MeanDiameterDifferentiation, "4-tree MA-A T");
            AssertNear(5.0 / 6.0, m4.Areas[1].MeanMingling, "4-tree MA-B M");
            AssertNear(11.0 / 24.0, m4.Areas[1].MeanDiameterDifferentiation, "4-tree MA-B T");

            // 6-tree analytical fixture (k = 3, genuine nearest-neighbour selection across shared border x = 10).
            var root6 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[]
                    {
                        LiveTree("MA-A-001", 1f, 5f, 10f),
                        LiveTree("MA-A-002", 2f, 5f, 20f),
                        LiveTree("MA-A-003", 9f, 5f, 40f),
                    }),
                    ("MA-B", new[]
                    {
                        LiveTree("MA-B-001", 11f, 5f, 40f),
                        LiveTree("MA-B-002", 12f, 5f, 20f),
                        LiveTree("MA-B-003", 18f, 5f, 10f),
                    }),
                },
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["MA-B-001"] = Phase2Species.OakId,
                    ["MA-B-002"] = Phase2Species.BeechId,
                    ["MA-B-003"] = Phase2Species.OakId,
                });
            var m6 = WoodlandStructureMetrics.Compute(root6);
            T.Eq(6, m6.FocalLivingTreeCount, "6 living trees");
            AssertNear(4.0 / 9.0, m6.Areas[0].MeanMingling, "6-tree MA-A M");
            AssertNear(0.5, m6.Areas[0].MeanDiameterDifferentiation, "6-tree MA-A T");
            AssertNear(7.0 / 9.0, m6.Areas[1].MeanMingling, "6-tree MA-B M");
            AssertNear(19.0 / 36.0, m6.Areas[1].MeanDiameterDifferentiation, "6-tree MA-B T");
            AssertNear(11.0 / 18.0, m6.MeanMingling, "6-tree woodland M");
            AssertNear(37.0 / 72.0, m6.MeanDiameterDifferentiation, "6-tree woodland T");

            // Purity on live MA-1-MIXED session: ComputeStructure mutates zero bytes.
            var mixed = MixedForest();
            byte[] before = Canonical(mixed);
            var struct1 = mixed.ComputeStructure();
            var struct2 = mixed.ComputeStructure();
            AssertBytes(before, Canonical(mixed), "ComputeStructure mutates nothing");
            T.Eq(struct1.FocalLivingTreeCount, struct2.FocalLivingTreeCount, "deterministic count");
            T.Eq(struct1.MeanMingling, struct2.MeanMingling, "deterministic M");
            T.Eq(struct1.MeanDiameterDifferentiation, struct2.MeanDiameterDifferentiation, "deterministic T");
            T.True(struct1.MeanMingling > 0.0, "MA-1-MIXED has positive mingling");
        });

        T.Add("P1-TH-06b: small counts (0/1/2/3), zero-DBH T_i null, ordinal TreeId tie-break & exclusions (stumps/dead/deadwood/PJ)", () =>
        {
            // 2-tree stand -> k_i = 1, MaxNeighbourCountK = 3.
            var root2 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[] { LiveTree("MA-A-001", 0f, 0f, 20f) }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 3f, 4f, 40f) }),
                },
                new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-B-001"] = Phase2Species.OakId });
            var m2 = WoodlandStructureMetrics.Compute(root2);
            T.Eq(2, m2.FocalLivingTreeCount, "2 trees");
            T.Eq(1, m2.EffectiveNeighbourCount, "k = 1");
            T.Eq(3, m2.MaxNeighbourCountK, "maxK = 3");
            AssertNear(1.0, m2.MeanMingling, "2-tree M = 1");
            AssertNear(0.5, m2.MeanDiameterDifferentiation, "2-tree T = 0.5");
            AssertNear(1.0, m2.Areas[0].MeanMingling, "2-tree MA-A cross-rack M = 1");
            AssertNear(0.5, m2.Areas[1].MeanDiameterDifferentiation, "2-tree MA-B cross-rack T = 0.5");

            // 3-tree stand -> k_i = 2, MaxNeighbourCountK = 3.
            var root3 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[] { LiveTree("MA-A-001", 0f, 0f, 20f), LiveTree("MA-A-002", 2f, 0f, 40f) }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 4f, 0f, 20f) }),
                },
                new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-B-001"] = Phase2Species.OakId });
            var m3 = WoodlandStructureMetrics.Compute(root3);
            T.Eq(3, m3.FocalLivingTreeCount, "3 trees");
            T.Eq(2, m3.EffectiveNeighbourCount, "k = 2");
            T.Eq(3, m3.MaxNeighbourCountK, "maxK = 3");
            AssertNear(2.0 / 3.0, m3.MeanMingling, "3-tree M = 2/3");
            AssertNear(1.0 / 3.0, m3.MeanDiameterDifferentiation, "3-tree T = 1/3");

            // 1-tree stand -> null/unavailable.
            var root1 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[] { LiveTree("MA-A-001", 0f, 0f, 20f) }),
                    ("MA-B", Array.Empty<AnnualTree>()),
                }, null);
            var m1 = WoodlandStructureMetrics.Compute(root1);
            T.Eq(1, m1.FocalLivingTreeCount, "1 tree");
            T.Eq(0, m1.EffectiveNeighbourCount, "k = 0");
            T.Eq(3, m1.MaxNeighbourCountK, "maxK = 3");
            T.True(m1.MeanMingling is null && m1.MeanDiameterDifferentiation is null, "1-tree null");

            // 0-tree stand -> null/unavailable.
            var root0 = SyntheticRoot(
                new[]
                {
                    ("MA-A", Array.Empty<AnnualTree>()),
                    ("MA-B", Array.Empty<AnnualTree>()),
                }, null);
            var m0 = WoodlandStructureMetrics.Compute(root0);
            T.Eq(0, m0.FocalLivingTreeCount, "0 trees");
            T.Eq(0, m0.EffectiveNeighbourCount, "k = 0");
            T.True(m0.MeanMingling is null && m0.MeanDiameterDifferentiation is null, "0-tree null");

            // Zero-positive-DBH-pair stand -> M_i computed, T_i null.
            var rootZeroDbh = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[] { LiveTree("MA-A-001", 0f, 0f, 0f) }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 2f, 0f, 0f) }),
                },
                new Dictionary<string, string>(StringComparer.Ordinal) { ["MA-B-001"] = Phase2Species.OakId });
            var mZero = WoodlandStructureMetrics.Compute(rootZeroDbh);
            AssertNear(1.0, mZero.MeanMingling, "zero-DBH M still 1.0");
            T.True(mZero.MeanDiameterDifferentiation is null, "zero-DBH T is null");
            T.Eq(0, mZero.ValidDiameterFocalCount, "zero valid diameter focal count");

            // Exact distance ties break deterministically by ordinal TreeId regardless of roster order.
            var tieSpecies1 = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["MA-A-001"] = Phase2Species.OakId,
                ["MA-A-002"] = Phase2Species.OakId,
                ["MA-A-003"] = Phase2Species.OakId,
            };
            // Put MA-A-004 first in roster to prove ordinal TreeId (not roster order) selects 001, 002, 003.
            var tieRoot1 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[]
                    {
                        LiveTree("MA-A-004", 5f, 3f, 20f),
                        LiveTree("MA-A-003", 5f, 7f, 40f),
                        LiveTree("MA-A-002", 3f, 5f, 40f),
                        LiveTree("MA-A-001", 7f, 5f, 40f),
                    }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 5f, 5f, 20f) }),
                }, tieSpecies1);
            var mTie1 = WoodlandStructureMetrics.Compute(tieRoot1);
            // Focal MA-B-001 is sole tree in MA-B; top 3 are 001, 002, 003 (all Oak, DBH 40) -> M = 1.0, T = 0.5.
            AssertNear(1.0, mTie1.Areas[1].MeanMingling, "tie-break selects 001/002/003 (all Oak)");
            AssertNear(0.5, mTie1.Areas[1].MeanDiameterDifferentiation, "tie-break T = 0.5");

            // Rename 004 -> 000 (Sitka, DBH 20) so it wins the ordinal tie over 003 -> top 3 are 000, 001, 002 -> M = 2/3, T = 1/3.
            var tieRoot2 = SyntheticRoot(
                new[]
                {
                    ("MA-A", new[]
                    {
                        LiveTree("MA-A-003", 5f, 7f, 40f),
                        LiveTree("MA-A-002", 3f, 5f, 40f),
                        LiveTree("MA-A-001", 7f, 5f, 40f),
                        LiveTree("MA-A-000", 5f, 3f, 20f),
                    }),
                    ("MA-B", new[] { LiveTree("MA-B-001", 5f, 5f, 20f) }),
                }, tieSpecies1);
            var mTie2 = WoodlandStructureMetrics.Compute(tieRoot2);
            AssertNear(2.0 / 3.0, mTie2.Areas[1].MeanMingling, "tie-break selects 000/001/002 (2 Oak + 1 Sitka)");
            AssertNear(1.0 / 3.0, mTie2.Areas[1].MeanDiameterDifferentiation, "tie-break T = 1/3");

            // Exclusions: felled stumps, standing dead, fallen deadwood, and unpromoted PJ saplings at distance 0.1 m are ignored.
            var stump = LiveTree("MA-A-STUMP", 0.1f, 0f, 80f) with
            {
                Death = new AnnualDeath("MA-A-STUMP", 1, FellCause.Harvest, FellCause.Mechanism),
            };
            var standingDead = LiveTree("MA-A-DEAD", 0.05f, 0f, 90f) with
            {
                Death = new AnnualDeath("MA-A-DEAD", 1, "DensitySuppression", "density"),
            };
            var stage4WithDeadwood = MultiAreaStage4State.Empty with
            {
                Deadwood = Array.AsReadOnly(new[]
                {
                    new FallenDeadwoodRecord("DW0001", "MA-A-DEAD", "MA-A", Phase2Species.BeechId, 0.05f, 0f, 1,
                        "DensitySuppression", 90f, 25f, 7.95f),
                }),
            };
            var phase2WithPj = new MultiAreaPhase2State(
                Phase2Options.Default, 1, 1,
                new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["MA-B-001"] = Phase2Species.OakId,
                    ["MA-A-STUMP"] = Phase2Species.BeechId,
                    ["MA-A-DEAD"] = Phase2Species.BeechId,
                },
                Array.Empty<Phase2AreaBands>(),
                Array.AsReadOnly(new[]
                {
                    new PlantedJuvenile("PJ0001", "MA-A", Phase2Species.BeechId, 0.02f, 0f, 1, 4f, 1.2f, true, null, -1, null, false, 0f, 0f, -1),
                }),
                Array.Empty<BrowseShelter>(),
                Array.Empty<SpotPatch>(),
                Array.Empty<MultiAreaPlantingOrder>(),
                Array.Empty<MultiAreaPlantingOrder>(),
                Array.Empty<Phase2WorkRecord>(),
                1, 2, 1,
                Array.Empty<Phase2YearAccount>());

            var rootWithExcluded = root2 with
            {
                Areas = Array.AsReadOnly(new[]
                {
                    root2.Areas[0] with
                    {
                        State = new AnnualState(1, Array.AsReadOnly(new[] { root2.Areas[0].State.Trees[0], stump, standingDead })),
                    },
                    root2.Areas[1],
                }),
                Stage4 = stage4WithDeadwood,
                Phase2 = phase2WithPj,
            };
            var mExcl = WoodlandStructureMetrics.Compute(rootWithExcluded);
            T.Eq(2, mExcl.FocalLivingTreeCount, "stumps/dead/deadwood/PJ excluded from count");
            T.Eq(1, mExcl.EffectiveNeighbourCount, "still k = 1");
            AssertNear(1.0, mExcl.MeanMingling, "exclusions leave M unchanged");
            AssertNear(0.5, mExcl.MeanDiameterDifferentiation, "exclusions leave T unchanged");
        });

        // ------------------------------------------------------------------ P1-TH-07: Honesty / Gate G3 assertions
        T.Add("P1-TH-07: Gate G3 honesty labels and exact 6-value 2D StopReason enum (no unmodelled 3D values)", () =>
        {
            T.True(MultiAreaTimePacingConstants.CalibrationNote.Contains("GAMEPLAY CALIBRATION", StringComparison.Ordinal) &&
                MultiAreaTimePacingConstants.CalibrationNote.Contains("not universal Irish forestry advice", StringComparison.OrdinalIgnoreCase),
                "GAMEPLAY CALIBRATION framing");
            T.True(MultiAreaTimePacingConstants.ObservationPeriodLabel.Contains("observation", StringComparison.OrdinalIgnoreCase) &&
                MultiAreaTimePacingConstants.ObservationPeriodLabel.Contains("not a completed silvicultural thinning cycle", StringComparison.OrdinalIgnoreCase),
                "quiet observation framing");
            T.Eq("Observed changes reflect ongoing natural growth, competition, seeding, and light alongside recorded management — not a single-cause effect.",
                MultiAreaHistoryLabels.NonCausalComparisonStatement, "exact non-causal wording");
            T.True(!MultiAreaHistoryLabels.NonCausalComparisonStatement.Contains("weather", StringComparison.OrdinalIgnoreCase),
                "no weather mention");
            T.Eq("Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast.",
                WoodlandStructureMetrics.HonestyLabel, "exact spatial-structure honesty label");

            var stopNames = Enum.GetNames<TimePacingStopReason>();
            T.Eq(6, stopNames.Length, "exact 6 StopReason values");
            T.Eq("ReachedReviewYear,WorkPlanOpen,InvalidTargetYear,StandCannotAdvance,CancelledBetweenYears,AnnualStepRefused",
                string.Join(",", stopNames), "exact StopReason names");
            foreach (var forbidden in new[]
            {
                "StormEvent", "TeachingGateIncomplete", "ReviewUnread", "YearCapReached", "ObjectiveOrOutcomeReached",
            })
            {
                T.True(!stopNames.Contains(forbidden, StringComparer.Ordinal), "unmodelled 3D stop reason absent: " + forbidden);
            }
        });

        // ------------------------------------------------------------------ P1-TH-08: Hermetic Core boundary & schema invariants
        T.Add("P1-TH-08: Core boundary stays hermetic (SchemaVersion 2, zero UI/ASP.NET/Unity references)", () =>
        {
            T.Eq(2, Phase1SaveFormat.SchemaVersion, "save schema version remains 2");
            var coreAssembly = typeof(MultiAreaContinuation).Assembly;
            var referenced = coreAssembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
            T.True(referenced.Length > 0, "Core assembly references read");
            foreach (var name in referenced)
            {
                T.True(!name.StartsWith("Microsoft.AspNetCore", StringComparison.OrdinalIgnoreCase),
                    "Core must not reference ASP.NET Core: " + name);
                T.True(!name.Contains("SitkaP1.Ui", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(name, "sitkap1-ui", StringComparison.OrdinalIgnoreCase),
                    "Core must not reference UI assembly: " + name);
                T.True(!name.StartsWith("UnityEngine", StringComparison.OrdinalIgnoreCase) &&
                    !name.StartsWith("UnityEditor", StringComparison.OrdinalIgnoreCase),
                    "Core must not reference Unity assemblies: " + name);
            }
            T.Eq(coreAssembly, typeof(MultiAreaTimePacing).Assembly, "MultiAreaTimePacing lives in SitkaP1.Core");
            T.Eq(coreAssembly, typeof(MultiAreaHistoryQuery).Assembly, "MultiAreaHistoryQuery lives in SitkaP1.Core");
            T.Eq(coreAssembly, typeof(WoodlandStructureMetrics).Assembly, "WoodlandStructureMetrics lives in SitkaP1.Core");
        });
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static MultiAreaContinuation Ma1Forest() =>
        new(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Phase2);

    private static MultiAreaContinuation MixedForest()
    {
        var (experiment, options, _) = MultiAreaDemoExperiments.CreateMixedDemo();
        return new MultiAreaContinuation(experiment, MultiAreaModelSelection.Phase2, null, null, options);
    }

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static MultiAreaRoot GetRoot(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;

    private static void SetRoot(MultiAreaContinuation c, MultiAreaRoot value) => RootField.SetValue(c, value);

    private static string FirstOakId(MultiAreaContinuation session)
    {
        var snap = session.ReadCompositeUiSnapshot();
        foreach (var area in snap.View.Areas)
            foreach (var tree in area.Trees)
                if (tree.IsLiving && snap.Ecology is not null &&
                    snap.Ecology.SpeciesByTreeId.TryGetValue(tree.TreeId, out var sp) && sp == Phase2Species.OakId)
                    return tree.TreeId;
        throw new Exception("no living Oak found");
    }

    private static MultiAreaSessionView ExecuteSingleFell(MultiAreaContinuation session, string areaId, string reason)
    {
        var v = session.ReadView();
        var area = v.Areas.Single(a => a.AreaId == areaId);
        string treeId = area.Trees.First(t => t.IsLiving).TreeId;
        v = session.AddPending(Fence(v), areaId, treeId, reason).View;
        v = session.Approve(Fence(v)).View;
        var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
        T.True(exec.RefusalCode is null, "fell execute succeeded");
        return exec.View;
    }

    private static void ExecuteAllSixActionTypesInYear(MultiAreaContinuation session)
    {
        var snap = session.ReadCompositeUiSnapshot();
        var v = snap.View;
        string fellId = v.Areas[0].Trees.First(t => t.IsLiving).TreeId;
        string oakId = FirstOakId(session);
        var cell = snap.Readout.Areas[1].Ground[0];
        var pt = Phase2Support.FindPoint(session, "MA-B", 1.0f);

        v = session.AddPending(Fence(v), "MA-A", fellId, "Step 1 test fell").View;
        v = session.AddScheduledPrune(Fence(v), "MA-B", oakId).View;
        v = session.AddClearUndergrowth(Fence(v), "MA-B", cell.Column, cell.Row).View;
        v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, pt.X, pt.Z).View;
        v = session.AddShelter(Fence(v), "MA-B", pt.X, pt.Z).View;
        v = session.AddSpotClear(Fence(v), "MA-B", pt.X, pt.Z).View;
        v = session.Approve(Fence(v)).View;
        var exec = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
        T.True(exec.RefusalCode is null, "all-6-actions execute succeeded");
    }

    private static void VerifySkipParity(string label, Func<MultiAreaContinuation> factory)
    {
        var skip = factory();
        var manual = factory();
        AssertBytes(Canonical(manual), Canonical(skip), label + ": identical genesis");

        // 1. Pre-Fell observation skip (0 -> 4).
        var p0 = skip.PreviewNextReviewYear(Fence(skip.ReadView()));
        T.Eq(4, p0.SuggestedTargetYear, label + ": pre-Fell suggested = 4");
        T.True(p0.IsObservationPeriod, label + ": pre-Fell is observation");
        var skipObs = skip.AdvanceYears(Fence(skip.ReadView()), p0.SuggestedTargetYear);
        T.True(skipObs.ReachedTarget && skipObs.StopReason == TimePacingStopReason.ReachedReviewYear, label + ": pre-Fell reached");
        T.Eq(4, skipObs.CompletedYears.Count, label + ": 4 completed");
        T.Eq(4, skipObs.DestinationYear, label + ": destination 4");
        for (int i = 0; i < 4; i++)
            manual.Advance(Fence(manual.ReadView()), consentToCancelWork: false);
        AssertBytes(Canonical(manual), Canonical(skip), label + ": pre-Fell skip matches 4 manual Advance calls byte-for-byte");

        // 2. Post-Fell review skip (Fell in Year 4 -> skip 4 -> 8).
        ExecuteSingleFell(skip, "MA-A", label + " year-4 fell");
        ExecuteSingleFell(manual, "MA-A", label + " year-4 fell");
        var p4 = skip.PreviewNextReviewYear(Fence(skip.ReadView()));
        T.Eq(4, p4.AnchorFellYear, label + ": anchored to Year 4");
        T.Eq(8, p4.SuggestedTargetYear, label + ": suggested target = 8");
        T.True(!p4.IsObservationPeriod, label + ": post-Fell not observation");
        var skipPost = skip.AdvanceYears(Fence(skip.ReadView()), p4.SuggestedTargetYear);
        T.True(skipPost.ReachedTarget && skipPost.StopReason == TimePacingStopReason.ReachedReviewYear, label + ": post-Fell reached");
        T.Eq(4, skipPost.CompletedYears.Count, label + ": 4 post-Fell steps");
        for (int i = 0; i < 4; i++)
            manual.Advance(Fence(manual.ReadView()), consentToCancelWork: false);
        AssertBytes(Canonical(manual), Canonical(skip), label + ": post-Fell skip matches 4 manual Advance calls byte-for-byte");

        // 3. Max-ticks 5-year skip (8 -> 13).
        var skip5 = skip.AdvanceYears(Fence(skip.ReadView()), 13);
        T.True(skip5.ReachedTarget && skip5.StopReason == TimePacingStopReason.ReachedReviewYear, label + ": 5-year reached");
        T.Eq(5, skip5.CompletedYears.Count, label + ": 5 steps");
        for (int i = 0; i < 5; i++)
            manual.Advance(Fence(manual.ReadView()), consentToCancelWork: false);
        AssertBytes(Canonical(manual), Canonical(skip), label + ": 5-year skip matches 5 manual Advance calls byte-for-byte");
    }

    private static void VerifyWorkPlanOpenRefused(string kind, Func<MultiAreaContinuation, MultiAreaSessionView, MultiAreaSessionView> queueOrder)
    {
        var session = MixedForest();
        var vPending = queueOrder(session, session.ReadView());
        int pendingYear = vPending.Source.EcologicalYear;
        byte[] pendingBefore = Canonical(session);

        var pPending = session.PreviewNextReviewYear(Fence(vPending));
        T.True(!pPending.Eligible && pPending.StopReason == TimePacingStopReason.WorkPlanOpen, kind + " Pending preview blocked");
        foreach (int target in new[] { pendingYear + 4, pendingYear + 6, pendingYear })
        {
            var rPending = session.AdvanceYears(Fence(vPending), target);
            T.Eq(TimePacingStopReason.WorkPlanOpen, rPending.StopReason, kind + " Pending advance refused (target " + target + ")");
            T.Eq(MultiAreaTimePacingConstants.WorkPlanOpenLabel, rPending.StopDetail, kind + " Pending StopDetail (target " + target + ")");
            T.True(!rPending.ReachedTarget && rPending.CompletedYears.Count == 0 && rPending.DestinationYear == pendingYear,
                kind + " Pending 0 steps (target " + target + ")");
            AssertBytes(pendingBefore, Canonical(session), kind + " Pending zero mutation (target " + target + ")");
        }

        var vApproved = session.Approve(Fence(vPending)).View;
        int approvedYear = vApproved.Source.EcologicalYear;
        byte[] approvedBefore = Canonical(session);

        var pApproved = session.PreviewNextReviewYear(Fence(vApproved));
        T.True(!pApproved.Eligible && pApproved.StopReason == TimePacingStopReason.WorkPlanOpen, kind + " Approved preview blocked");
        foreach (int target in new[] { approvedYear + 4, approvedYear + 6, approvedYear })
        {
            var rApproved = session.AdvanceYears(Fence(vApproved), target);
            T.Eq(TimePacingStopReason.WorkPlanOpen, rApproved.StopReason, kind + " Approved advance refused (target " + target + ")");
            T.Eq(MultiAreaTimePacingConstants.WorkPlanOpenLabel, rApproved.StopDetail, kind + " Approved StopDetail (target " + target + ")");
            T.True(!rApproved.ReachedTarget && rApproved.CompletedYears.Count == 0 && rApproved.DestinationYear == approvedYear,
                kind + " Approved 0 steps (target " + target + ")");
            AssertBytes(approvedBefore, Canonical(session), kind + " Approved zero mutation (target " + target + ")");
        }
    }

    private static MultiAreaContinuation SaveAndRestoreIntoFreshMixed(MultiAreaContinuation source, string label)
    {
        var envelope = source.CaptureSave(Fence(source.ReadView()), label, 1);
        byte[] bytes = Phase1SaveCodec.Encode(envelope);
        var fresh = MixedForest();
        var decoded = Phase1SaveCodec.Decode(bytes, fresh.SaveExpectation());
        var load = fresh.Restore(Fence(fresh.ReadView()), decoded);
        T.True(load.RefusalCode is null, "restore succeeded: " + load.RefusalCode);
        return fresh;
    }

    private static void AssertContinuityEqual(string tag, MultiAreaContinuation control, MultiAreaContinuation restored, int[] years)
    {
        var pCtrl = control.PreviewNextReviewYear(Fence(control.ReadView()));
        var pRest = restored.PreviewNextReviewYear(Fence(restored.ReadView()));
        T.Eq(pCtrl, pRest, tag + ": preview equal");

        var iCtrl = control.ReadHistoryIndex(pageSize: 20);
        var iRest = restored.ReadHistoryIndex(pageSize: 20);
        T.Eq(iCtrl.TotalRecordedYears, iRest.TotalRecordedYears, tag + ": index total");
        T.Eq(iCtrl.NextCursorBeforeYear, iRest.NextCursorBeforeYear, tag + ": index cursor");
        T.Eq(iCtrl.Items.Count, iRest.Items.Count, tag + ": index item count");
        for (int i = 0; i < iCtrl.Items.Count; i++)
            T.Eq(iCtrl.Items[i], iRest.Items[i], tag + ": index row " + i);

        foreach (int y in years)
        {
            var yCtrl = control.ReadHistoryYear(y);
            var yRest = restored.ReadHistoryYear(y);
            T.Eq(yCtrl.WorkYear, yRest.WorkYear, tag + " Y" + y + " WorkYear");
            T.Eq(yCtrl.ObservedYear, yRest.ObservedYear, tag + " Y" + y + " ObservedYear");
            T.Eq(yCtrl.TotalWorkCounts, yRest.TotalWorkCounts, tag + " Y" + y + " TotalWorkCounts");
            T.Eq(yCtrl.AnnualOutcomeNextYear?.YearAfter, yRest.AnnualOutcomeNextYear?.YearAfter, tag + " Y" + y + " next outcome year");
            T.Eq(yCtrl.AnnualOutcomeNextYear?.Areas.Sum(a => a.After.LivingCount),
                yRest.AnnualOutcomeNextYear?.Areas.Sum(a => a.After.LivingCount), tag + " Y" + y + " next outcome live stems");
            T.Eq(yCtrl.AnnualRecordEndingInYear?.YearAfter, yRest.AnnualRecordEndingInYear?.YearAfter, tag + " Y" + y + " ending year");
            T.Eq(yCtrl.LaterObservedComparison.TotalCurrentLiveStems, yRest.LaterObservedComparison.TotalCurrentLiveStems, tag + " Y" + y + " current live stems");
            T.Eq(BitConverter.DoubleToUInt64Bits(yCtrl.LaterObservedComparison.TotalCurrentBasalAreaM2),
                BitConverter.DoubleToUInt64Bits(yRest.LaterObservedComparison.TotalCurrentBasalAreaM2), tag + " Y" + y + " current BA bits");
            T.Eq(yCtrl.PlantedWorkFollowUp.Count, yRest.PlantedWorkFollowUp.Count, tag + " Y" + y + " follow-up count");
            for (int f = 0; f < yCtrl.PlantedWorkFollowUp.Count; f++)
                T.Eq(yCtrl.PlantedWorkFollowUp[f], yRest.PlantedWorkFollowUp[f], tag + " Y" + y + " follow-up " + f);
        }

        var sCtrl = control.ComputeStructure();
        var sRest = restored.ComputeStructure();
        T.Eq(sCtrl.FocalLivingTreeCount, sRest.FocalLivingTreeCount, tag + ": structure focal count");
        T.Eq(sCtrl.EffectiveNeighbourCount, sRest.EffectiveNeighbourCount, tag + ": structure effective K");
        T.Eq(DoubleBits(sCtrl.MeanMingling), DoubleBits(sRest.MeanMingling), tag + ": structure M bits");
        T.Eq(DoubleBits(sCtrl.MeanDiameterDifferentiation), DoubleBits(sRest.MeanDiameterDifferentiation), tag + ": structure T bits");
        T.Eq(sCtrl.Areas.Count, sRest.Areas.Count, tag + ": structure area count");
        for (int a = 0; a < sCtrl.Areas.Count; a++)
        {
            T.Eq(sCtrl.Areas[a].AreaId, sRest.Areas[a].AreaId, tag + ": structure area id");
            T.Eq(DoubleBits(sCtrl.Areas[a].MeanMingling), DoubleBits(sRest.Areas[a].MeanMingling), tag + ": structure area M bits");
            T.Eq(DoubleBits(sCtrl.Areas[a].MeanDiameterDifferentiation), DoubleBits(sRest.Areas[a].MeanDiameterDifferentiation), tag + ": structure area T bits");
        }
    }

    private static ulong? DoubleBits(double? value) => value is double d ? BitConverter.DoubleToUInt64Bits(d) : null;

    private static AnnualTree LiveTree(string id, float x, float z, float dbhCm) =>
        new(id, x, z, 30, dbhCm, 18f, 2f, Death: null);

    private static MultiAreaRoot SyntheticRoot((string AreaId, AnnualTree[] Trees)[] areas,
        IReadOnlyDictionary<string, string>? broadleafSpecies)
    {
        var areaStates = areas.Select(a => new MultiAreaAreaState(
            a.AreaId,
            new AnnualState(0, Array.AsReadOnly(a.Trees)),
            Array.Empty<string>())).ToArray();

        MultiAreaPhase2State? phase2 = broadleafSpecies is null ? null
            : new MultiAreaPhase2State(
                Phase2Options.Default, 1, 1,
                new Dictionary<string, string>(broadleafSpecies, StringComparer.Ordinal),
                Array.Empty<Phase2AreaBands>(),
                Array.Empty<PlantedJuvenile>(),
                Array.Empty<BrowseShelter>(),
                Array.Empty<SpotPatch>(),
                Array.Empty<MultiAreaPlantingOrder>(),
                Array.Empty<MultiAreaPlantingOrder>(),
                Array.Empty<Phase2WorkRecord>(),
                1, 1, 1,
                Array.Empty<Phase2YearAccount>());

        return new MultiAreaRoot(
            0, 0, 0, 0,
            Array.AsReadOnly(areaStates),
            Array.Empty<MultiAreaWorkEvent>(),
            new Dictionary<string, MultiAreaExecutedWork>(StringComparer.Ordinal),
            Array.Empty<MultiAreaWorkOrder>(),
            null,
            Array.Empty<MultiAreaYearRecord>(),
            Stage4: null, Stage5: null, Phase2: phase2);
    }

    private static void AssertNear(double expected, double? actual, string what)
    {
        T.True(actual is not null, what + ": expected " + expected + ", got null");
        double diff = Math.Abs(expected - actual!.Value);
        T.True(diff <= Tol, FormattableString.Invariant($"{what}: expected {expected:R}, got {actual.Value:R} (diff {diff:E})"));
    }

    private static byte[] Canonical(MultiAreaContinuation session)
    {
        var view = session.ReadView();
        var envelope = session.CaptureSave(Fence(view), "compare", 1);
        var state = NormaliseAuthorityIds(envelope.State);
        return Phase1SaveCodec.Encode(new Phase1SaveEnvelope(envelope.Header with { SavedUtc = "2026-01-01T00:00:00Z" }, state));
    }

    private static string Norm(string value) => PlanIdSessionPrefix.Replace(value, "SESSION");

    private static Phase1SaveState NormaliseAuthorityIds(Phase1SaveState s) => s with
    {
        Events = Array.AsReadOnly(s.Events.Select(e => e with
        {
            ExecutionKey = Norm(e.ExecutionKey),
            PlanId = Norm(e.PlanId),
            Areas = Array.AsReadOnly(e.Areas.Select(a => a with
            {
                Fells = Array.AsReadOnly(a.Fells.Select(f => f with
                {
                    ExecutionKey = Norm(f.ExecutionKey),
                    PlanId = Norm(f.PlanId),
                }).ToArray()),
            }).ToArray()),
        }).ToArray()),
        Executed = Array.AsReadOnly(s.Executed.Select(x => x with { ExecutionKey = Norm(x.ExecutionKey) }).ToArray()),
        Stage4 = s.Stage4 is null ? null : s.Stage4 with
        {
            PruneLog = Array.AsReadOnly(s.Stage4.PruneLog.Select(r => r with
            {
                ExecutionKey = Norm(r.ExecutionKey),
                PlanId = Norm(r.PlanId),
            }).ToArray()),
        },
        Stage5 = s.Stage5 is null ? null : s.Stage5 with
        {
            ClearLog = Array.AsReadOnly(s.Stage5.ClearLog.Select(r => r with
            {
                ExecutionKey = Norm(r.ExecutionKey),
                PlanId = Norm(r.PlanId),
            }).ToArray()),
        },
        Phase2 = s.Phase2 is null ? null : s.Phase2 with
        {
            WorkLog = Array.AsReadOnly(s.Phase2.WorkLog.Select(r => r with
            {
                ExecutionKey = Norm(r.ExecutionKey),
                PlanId = Norm(r.PlanId),
            }).ToArray()),
        },
    };

    private static void AssertBytes(byte[] expected, byte[] actual, string what)
    {
        if (!expected.AsSpan().SequenceEqual(actual))
            throw new Exception($"{what}: {expected.Length} vs {actual.Length} bytes differ");
    }
}
