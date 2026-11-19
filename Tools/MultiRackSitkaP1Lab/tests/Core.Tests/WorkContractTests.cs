using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// G2 acceptance probes for the typed intervention slice: manual Work Plan, whole-batch approval, one atomic Fell
/// transaction with once-only same-process replay, and genuine following-year survivor ecology.
/// These are feature tests, not a substitute for the independent nonauthor qualification.
/// </summary>
internal static class WorkContractTests
{
    internal static void Register(string manifestPath)
    {
        AnnualContinuation Open() => AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1,
            ManifestGenerator.FixtureId, Path.GetDirectoryName(manifestPath)!).ContinueInMemory();

        T.Add("work: committed frame is the one undivided area with 100 exact live source IDs", () =>
        {
            var frame = Open().Frame;
            T.Eq(ManagementScope.WholeUndividedThirtyMetres, frame.ManagementScopeId, "single undivided management area");
            T.Eq(ManifestGenerator.FixtureId, frame.FixtureId, "fixture identity");
            T.Eq(0, frame.EcologicalYear, "genesis year");
            T.Eq(0, frame.AnnualRevision, "genesis annual revision");
            T.Eq(0, frame.WorkRevision, "genesis work revision");
            T.Eq(100, frame.Trees.Count, "100 exact ids");
            T.Eq("P30-001", frame.Trees[0].TreeId, "first id");
            T.Eq("P30-100", frame.Trees[99].TreeId, "last id");
            T.True(frame.Trees.All(t => t.IsLiving), "all living at genesis");
            T.Eq(100, frame.Stand.LivingCount, "living stand count");
            T.True(frame.Stand.BasalAreaM2PerHa > 0, "real basal area");
            T.Eq(0, frame.Events.Count, "no events yet");
        });

        T.Add("work: selecting and approving never fells; only Execute commits", () =>
        {
            var engine = Open();
            var control = Open();
            engine.AddPendingFell("P30-001", "release a retained neighbour");
            T.Eq(100, engine.Frame.Trees.Count(t => t.IsLiving), "selection fells nothing");
            T.Eq(0, engine.Frame.WorkRevision, "selection is not work");
            var plan = engine.ApprovePlan("plan-a");
            T.Eq(100, engine.Frame.Trees.Count(t => t.IsLiving), "approval fells nothing");
            T.Eq(0, engine.Frame.WorkRevision, "approval is not work");
            T.Eq(1, plan.Orders.Count, "one order");
            T.Eq("P30-001", plan.Orders[0].TreeId, "exact id bound");
            T.True(plan.Source.Equals(engine.CurrentToken), "plan bound to the current source token");

            var result = engine.Execute(plan, "exec-a");
            T.True(!result.Replayed, "first execution is real");
            T.Eq(99, engine.Frame.Trees.Count(t => t.IsLiving), "one tree felled");
            T.Eq(1, engine.Frame.WorkRevision, "work revision advanced once");
            T.Eq(0, engine.Frame.EcologicalYear, "fell does not advance the ecological clock");
            T.Eq(0, engine.Frame.AnnualRevision, "fell does not advance the annual revision");
            T.Eq(1, engine.Frame.Events.Count, "exactly one event");
            var fell = engine.Frame.Events[0].Fells[0];
            T.Eq(FellCause.Harvest, fell.Cause, "typed fell cause");
            T.True(fell.Cause != "self-thinning", "distinct from natural death cause");
            T.Eq("exec-a", fell.ExecutionKey, "event carries the execution key");
            T.Eq(99, engine.Frame.Stand.LivingCount, "after stand from committed source");
            var felled = engine.Frame.Trees.Single(t => t.TreeId == "P30-001");
            T.True(!felled.IsLiving, "roster marks the tree dead");
            T.Eq(FellCause.Harvest, felled.Death!.Cause, "roster death is the typed work cause");
            T.Eq(100, control.Frame.Trees.Count(t => t.IsLiving), "control untouched");
        });

        T.Add("work: duplicate, unknown, case-shifted, missing and dead IDs are refused with reasons", () =>
        {
            var engine = Open();
            T.Refuses("WorkReasonRequired", () => engine.AddPendingFell("P30-002", "  "));
            T.Refuses("UnknownTreeId", () => engine.AddPendingFell("P30-999", "not in fixture"));
            T.Refuses("UnknownTreeId", () => engine.AddPendingFell("p30-002", "case-shifted"));
            engine.AddPendingFell("P30-002", "first selection");
            T.Refuses("WorkDuplicateTreeId", () => engine.AddPendingFell("P30-002", "again"));
            T.Refuses("WorkEmptyPlan", () => Open().ApprovePlan("empty"));

            var plan = engine.ApprovePlan("plan-b");
            var result = engine.Execute(plan, "exec-b");
            T.Eq(1, result.Event.Fells.Count, "one fell");
            T.Refuses("WorkPlanNotApproved", () => engine.Execute(plan, "exec-b2"));
            T.Refuses("UnknownTreeId", () => engine.AddPendingFell("P30-002", "already felled is not live"));
        });

        T.Add("work: D-093 P1 — the 170 mm threshold is informational, not an eligibility rule", () =>
        {
            var engine = Open();
            var frame = engine.Frame;
            T.Eq(170L, engine.CurrentToken.ActionEligibleDbhThresholdMm, "threshold retained as metadata");
            var below = frame.Trees.Where(t => t.DbhCm * 10 < 170).OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
            var above = frame.Trees.Where(t => t.DbhCm * 10 >= 170).OrderBy(t => t.TreeId, StringComparer.Ordinal).First();
            engine.AddPendingFell(below.TreeId, "below 170 mm is still a player choice");
            engine.AddPendingFell(above.TreeId, "above 170 mm is the same choice");
            var plan = engine.ApprovePlan("plan-d093");
            var result = engine.Execute(plan, "exec-d093");
            T.Eq(2, result.Event.Fells.Count, "both sides of 170 mm are legally selectable");
            T.Eq(98, engine.Frame.Stand.LivingCount, "both felled");
        });

        T.Add("work: D-093 P2 — a batch leaving zero living stems is refused as a whole, atomically", () =>
        {
            var engine = Open();
            var before = engine.Snapshot;
            var bytes = JsonSerializer.SerializeToUtf8Bytes(before);
            foreach (var tree in engine.Frame.Trees) engine.AddPendingFell(tree.TreeId, "clear the stand");
            var plan = engine.ApprovePlan("plan-clear");
            T.Refuses("WorkWouldLeaveNoLivingStems", () => engine.Execute(plan, "exec-clear"));
            T.True(ReferenceEquals(before, engine.Snapshot), "no roster change");
            T.True(bytes.SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(engine.Snapshot)), "no byte change");
            T.Eq(0, engine.Frame.WorkRevision, "no work revision change");
            T.Eq(0, engine.Frame.Events.Count, "no event");
            T.Refuses("WorkWouldLeaveNoLivingStems", () => engine.Execute(plan, "exec-clear-2"));
            T.Eq(0, engine.Frame.WorkRevision, "a second key for the same illegal batch also changes nothing");
            T.Eq(100, engine.Frame.Stand.LivingCount, "the stand is untouched");
        });

        T.Add("work: pre-publish fault leaves roster, event, revisions and idempotency untouched, then retry commits once", () =>
        {
            var engine = Open();
            engine.AddPendingFell("P30-002", "fault seam probe");
            var plan = engine.ApprovePlan("plan-fault");
            var before = engine.Snapshot;
            bool injected = false;
            try { engine.Execute(plan, "exec-fault", () => throw new InvalidOperationException("injected")); }
            catch (InvalidOperationException e) when (e.Message == "injected") { injected = true; }
            T.True(injected, "fault seam reached");
            T.True(ReferenceEquals(before, engine.Snapshot), "nothing published");
            T.Eq(0, engine.Frame.WorkRevision, "work revision unchanged");
            T.Eq(0, engine.Frame.Events.Count, "event ledger unchanged");
            T.Eq(100, engine.Frame.Stand.LivingCount, "roster unchanged");

            var committed = engine.Execute(plan, "exec-fault");
            T.True(!committed.Replayed, "retry commits for real");
            T.Eq(1, engine.Frame.WorkRevision, "committed once");
            var replay = engine.Execute(plan, "exec-fault");
            T.True(replay.Replayed, "identical retry is a no-op");
            T.Eq(1, engine.Frame.WorkRevision, "replay did not fell again");
            T.Eq(1, engine.Frame.Events.Count, "replay did not append an event");
            T.Eq(committed.Event, replay.Event, "replay returns the recorded result");
        });

        T.Add("work: a reused execution key with a different payload conflicts and mutates nothing", () =>
        {
            var engine = Open();
            engine.AddPendingFell("P30-003", "first payload");
            var first = engine.ApprovePlan("plan-k1");
            engine.Execute(first, "shared-key");
            var after = engine.Snapshot;

            engine.AddPendingFell("P30-004", "second payload");
            var second = engine.ApprovePlan("plan-k2");
            T.Refuses("WorkExecutionKeyConflict", () => engine.Execute(second, "shared-key"));
            T.True(ReferenceEquals(after, engine.Snapshot), "conflict changed nothing");
            T.Eq(1, engine.Frame.WorkRevision, "no extra work revision");
            T.Eq(1, engine.Frame.Events.Count, "no extra event");
        });

        T.Add("work: pending or approved-unexecuted advance warns, needs consent and cancels at zero Fell", () =>
        {
            var control = Open();
            var withPending = Open();
            withPending.AddPendingFell("P30-006", "pending at year boundary");
            var before = withPending.Snapshot;
            T.Refuses("WorkPendingConsentRequired", () => withPending.Advance(0, 0));
            T.Refuses("WorkPendingConsentRequired", () => withPending.Advance(0, 0, 0, consentToCancelWork: false));
            T.True(ReferenceEquals(before, withPending.Snapshot), "denial changes nothing");

            var report = withPending.Advance(0, 0, 0, consentToCancelWork: true);
            T.Eq(1, report.YearAfter, "year advanced after consent");
            T.Eq(100, withPending.Frame.Stand.LivingCount, "zero Fell on consent");
            T.Eq(0, withPending.Frame.WorkRevision, "work revision untouched");
            T.Eq(0, withPending.Frame.Events.Count, "no work event");
            T.True(!withPending.HasUnexecutedWork, "orders stale-cancelled");
            T.Eq(control.Advance(0, 0).After.LivingCount, report.After.LivingCount, "same genuine biology as no-work");

            var approved = Open();
            approved.AddPendingFell("P30-007", "approved, not executed");
            approved.ApprovePlan("plan-pending");
            T.Refuses("WorkPendingConsentRequired", () => approved.Advance(0, 0, 0, consentToCancelWork: false));
            T.Eq(1, approved.Advance(0, 0, 0, consentToCancelWork: true).YearAfter, "consent advances");
            T.Eq(100, approved.Frame.Stand.LivingCount, "approved batch cancelled with zero Fell");
        });

        T.Add("work: year y Fell is followed by genuine year y+1 ecology on the 99 real survivors", () =>
        {
            var engine = Open();
            engine.AddPendingFell("P30-008", "thinning entry");
            var plan = engine.ApprovePlan("plan-year");
            var executed = engine.Execute(plan, "exec-year");
            T.Eq(0, executed.EcologicalYear, "work happened in year 0");
            var survivors = engine.Frame.Trees.Where(t => t.IsLiving).ToArray();
            T.Eq(99, survivors.Length, "99 survivors enter the annual step");

            var report = engine.Advance(0, 0, 1, consentToCancelWork: false);
            T.Eq(0, report.YearBefore, "from the work year");
            T.Eq(1, report.YearAfter, "into the next genuine year");
            T.Eq(99, report.Before.LivingCount, "annual step starts from post-Fell survivors only");
            T.Eq(1, engine.Frame.EcologicalYear, "ecology clock advanced once");
            T.Eq(1, engine.Frame.AnnualRevision, "annual revision advanced once");
            T.Eq(1, engine.Frame.WorkRevision, "work revision did not move on Advance");
            T.Eq(1, engine.Frame.Events.Count, "the year-0 event is preserved");

            var felled = engine.Frame.Trees.Single(t => t.TreeId == "P30-008");
            T.True(!felled.IsLiving, "felled tree stays dead through the annual step");
            T.Eq(0, felled.AgeYears - 30, "felled tree does not age");
            foreach (var id in survivors.Select(t => t.TreeId))
            {
                var grown = engine.Frame.Trees.Single(t => t.TreeId == id);
                T.True(grown.IsLiving == false || grown.AgeYears == 31, "each survivor received one genuine growth step");
            }
            T.Eq(report.After.LivingCount, engine.Frame.Stand.LivingCount, "reported stand equals committed roster");
            T.True(report.After.LivingCount <= 99, "mortality can only remove surviving stems");
        });

        T.Add("work: repeated fell/advance cycles bind a fresh source each time", () =>
        {
            var engine = Open();
            engine.AddPendingFell("P30-010", "cycle one");
            var first = engine.ApprovePlan("cycle-1");
            engine.Execute(first, "cycle-1-key");
            engine.Advance(0, 0, 1, consentToCancelWork: false);

            engine.AddPendingFell("P30-011", "cycle two");
            var second = engine.ApprovePlan("cycle-2");
            T.Eq(1, second.Source.EcologicalYear, "freshly bound to year 1");
            T.Eq(1, second.Source.AnnualRevision, "freshly bound to annual revision 1");
            T.Eq(1, second.Source.WorkRevision, "freshly bound to work revision 1");
            var result = engine.Execute(second, "cycle-2-key");
            T.Eq(2, result.WorkRevision, "second work revision");
            T.Eq(2, engine.Frame.Events.Count, "two distinct events");
            engine.Advance(1, 1, 2, consentToCancelWork: false);
            T.Eq(2, engine.Frame.EcologicalYear, "second genuine year");
            T.Eq(2, engine.Frame.WorkRevision, "work revision still 2");
        });

        T.Add("work: no-work years never create work and keep year==revision", () =>
        {
            var engine = Open();
            for (int year = 1; year <= 10; year++)
            {
                engine.Advance(year - 1, year - 1);
                T.Eq(year, engine.Frame.EcologicalYear, "year");
                T.Eq(year, engine.Frame.AnnualRevision, "annual revision equals year");
                T.Eq(0, engine.Frame.WorkRevision, "no work revision in a no-work run");
                T.Eq(0, engine.Frame.Events.Count, "no work events");
            }
        });

        T.Add("work: the last living stem cannot be felled, and annual advance still refuses at zero live", () =>
        {
            var two = new AnnualContinuation(new AnnualState(0, new[]
            {
                new AnnualTree("P30-001", 0f, 0f, 30, 200f, 20.4f, 2f),
            }), 20261116, 0.09f);
            two.AddPendingFell("P30-001", "the only stem");
            var plan = two.ApprovePlan("last-stem");
            T.Refuses("WorkWouldLeaveNoLivingStems", () => two.Execute(plan, "last-stem-key"));
            T.Eq(1, two.Frame.Stand.LivingCount, "the survivor is intact");

            var none = new AnnualContinuation(new AnnualState(0, new[]
            {
                new AnnualTree("dead", 0f, 0f, 30, 20f, 12f, 1f, new AnnualDeath("dead", 0, "probe", "hazard")),
            }), 20261116, 0.09f);
            T.Refuses("ContinuationNoLivingStems", () => none.Advance(0, 0));
        });
    }
}
