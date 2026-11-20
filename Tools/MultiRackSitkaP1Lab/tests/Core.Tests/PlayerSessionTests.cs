using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// PS-01…PS-20 (spec MULTI-STANDMAP-PLAYABLE-2D-V1, plan P2): the same-lock player session view, its fences, the
/// plan-state revision rules, engine-minted identities and the A-2 replay rule. PS-20 was added for the independent
/// Tier-2 finding D3 (a first execution must echo the minted key). These are the author's feature tests, not independent
/// qualification. The map scene tests (MS-01…MS-06) are registered from here as well.
/// </summary>
internal static class PlayerSessionTests
{
    private const int Seed = 20261116;
    private static readonly float Area = F32.FromBits(DerivedManifest.AreaHaBits);

    internal static void Register(string manifestPath)
    {
        string fixtureDirectory = Path.GetDirectoryName(manifestPath)!;
        AnnualContinuation Open() => AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1,
            ManifestGenerator.FixtureId, fixtureDirectory).ContinueInMemory();

        T.Add("player PS-01: the view is atomic under concurrent Execute, Advance and readers", () =>
        {
            const int Rounds = 16;
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var problems = new ConcurrentQueue<string>();
            long reads = 0;
            using var stop = new CancellationTokenSource();
            var readers = Enumerable.Range(0, 3).Select(_ => Task.Run(() =>
            {
                int lastPlanState = -1;
                while (!stop.IsCancellationRequested)
                {
                    var view = c.ReadPlayerView();
                    foreach (var problem in TornViewProblems(view, lastPlanState)) problems.Enqueue(problem);
                    lastPlanState = view.PlanStateRevision;
                    Interlocked.Increment(ref reads);
                }
            })).ToArray();

            int executeWins = 0;
            try
            {
                for (int round = 0; round < Rounds; round++)
                {
                    var view = player.Read();
                    view = Ok(player.AddPending(Fence(view), view.Frame.Trees.First(t => t.IsLiving).TreeId,
                        "PS-01 race " + round), "add");
                    view = Ok(player.Approve(Fence(view)), "approve");
                    var fence = Fence(view);
                    var plan = view.Approved!;
                    using var gate = new Barrier(2);
                    var execute = Task.Run(() =>
                    {
                        gate.SignalAndWait();
                        return player.Execute(fence, plan.PlanId, plan.ExecutionKey);
                    });
                    var advance = Task.Run(() =>
                    {
                        gate.SignalAndWait();
                        return player.Advance(fence, consentToCancelWork: true);
                    });
                    Task.WaitAll(execute, advance);
                    bool executed = execute.Result.RefusalCode is null;
                    bool advanced = advance.Result.RefusalCode is null;
                    T.True(executed != advanced, $"round {round}: exactly one of the racing Execute and Advance commits");
                    T.Eq("PlayerSourceStale", executed ? advance.Result.RefusalCode : execute.Result.RefusalCode,
                        $"round {round}: the losing command is refused as stale");
                    if (executed)
                    {
                        executeWins++;
                        Ok(player.Advance(Fence(player.Read()), consentToCancelWork: false), "post-Fell year");
                    }

                    // Whoever won the race, the readers also watch one real Fell and one no-work year every round.
                    view = player.Read();
                    view = Ok(player.AddPending(Fence(view), view.Frame.Trees.First(t => t.IsLiving).TreeId,
                        "PS-01 cycle " + round), "cycle add");
                    view = Ok(player.Approve(Fence(view)), "cycle approve");
                    var cycle = view.Approved!;
                    view = Ok(player.Execute(Fence(view), cycle.PlanId, cycle.ExecutionKey), "cycle execute");
                    Ok(player.Advance(Fence(view), consentToCancelWork: false), "cycle year");
                }
            }
            finally
            {
                stop.Cancel();
                Task.WaitAll(readers);
            }
            T.True(problems.IsEmpty, "torn view observed: " + string.Join("; ", problems.Take(3)));
            T.True(Interlocked.Read(ref reads) > 0, "readers observed views");
            var final = player.Read();
            T.Eq(2 * Rounds, final.Source.AnnualRevision, "exactly two genuine years per round");
            T.Eq(Rounds + executeWins, final.Source.WorkRevision, "one work revision per committed Execute");
            T.Eq(Rounds + executeWins, final.Frame.Events.Count, "one event per committed Execute");
            Console.WriteLine($"PS-01 rounds {Rounds}, Execute won {executeWins}, Advance won {Rounds - executeWins}, " +
                $"atomic reads {Interlocked.Read(ref reads)}");
        });

        T.Add("player PS-02: plan-state +1 exactly on Add, Remove, Approve, Cancel, Execute and consented Advance", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            T.Eq(0, player.Read().PlanStateRevision, "a new session starts at plan-state 0");
            T.Eq(0, player.Read().PlanStateRevision, "a read does not move the plan state");

            PlayerSessionView Step(string what, int planDelta, int workDelta, bool tokenMoves,
                Func<PlayerSessionView, PlayerCommandResult> command)
            {
                var before = player.Read();
                var after = Ok(command(before), what);
                T.Eq(before.PlanStateRevision + planDelta, after.PlanStateRevision, what + ": plan-state delta");
                T.Eq(before.Source.WorkRevision + workDelta, after.Source.WorkRevision, what + ": work revision delta");
                T.Eq(tokenMoves, after.Source != before.Source, what + ": source token moved");
                T.True(Bytes(after).SequenceEqual(Bytes(player.Read())), what + ": returned view is the current view");
                return after;
            }

            Step("Add", 1, 0, false, v => player.AddPending(Fence(v), "P30-001", "PS-02"));
            Step("Remove", 1, 0, false, v => player.RemovePending(Fence(v), "P30-001"));
            Step("Add", 1, 0, false, v => player.AddPending(Fence(v), "P30-001", "PS-02"));
            Step("Approve", 1, 0, false, v => player.Approve(Fence(v)));
            Step("Cancel", 1, 0, false, v => player.CancelApproved(Fence(v)));
            Step("Add", 1, 0, false, v => player.AddPending(Fence(v), "P30-002", "PS-02"));
            var approved = Step("Approve", 1, 0, false, v => player.Approve(Fence(v)));
            var plan = approved.Approved!;
            Step("Execute", 1, 1, true, v => player.Execute(Fence(v), plan.PlanId, plan.ExecutionKey));
            Step("Execute replay", 0, 0, false, _ => player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey));
            var beforeRefusal = player.Read();
            T.Eq("WorkReasonRequired", player.AddPending(Fence(beforeRefusal), "P30-003", " ").RefusalCode, "refused Add");
            T.Eq(beforeRefusal.PlanStateRevision, player.Read().PlanStateRevision, "a refusal leaves the plan state");
            var noWork = Step("no-work Advance", 0, 0, true, v => player.Advance(Fence(v), consentToCancelWork: false));
            T.True(!noWork.AnnualHistory.Last().CancelledUnexecutedWork, "a no-work year cancels nothing");
            Step("Add", 1, 0, false, v => player.AddPending(Fence(v), "P30-004", "PS-02"));
            var consented = Step("consented Advance", 1, 0, true,
                v => player.Advance(Fence(v), consentToCancelWork: true));
            T.True(consented.AnnualHistory.Last().CancelledUnexecutedWork, "the consented year records the cancellation");
            T.Eq(10, consented.PlanStateRevision, "ten successful plan transitions in total");

            // The increment lives at the legacy publish, so an unfenced legacy call also invalidates fenced callers.
            var fenced = player.Read();
            c.AddPendingFell("P30-005", "legacy caller");
            T.Eq(fenced.PlanStateRevision + 1, player.Read().PlanStateRevision, "a legacy Add moves the plan state");
            RefusedWithoutMutation(c, "PlayerPlanStateStale", () => player.RemovePending(Fence(fenced), "P30-005"),
                "fence read before a legacy Add");
        });

        T.Add("player PS-03: add-then-remove (ABA) leaves the old fence stale although pending content is equal", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var original = player.Read();
            var added = Ok(player.AddPending(Fence(original), "P30-010", "ABA"), "add");
            var removed = Ok(player.RemovePending(Fence(added), "P30-010"), "remove");
            T.Eq(0, removed.Pending.Count, "pending content equals the original (empty)");
            T.Eq(original.Source, removed.Source, "source token equals the original");
            T.Eq(original.PlanStateRevision + 2, removed.PlanStateRevision, "both transitions were recorded");
            foreach (var (what, call) in AllCommands(player, Fence(original), "no-plan", "no-plan:execute"))
                RefusedWithoutMutation(c, "PlayerPlanStateStale", call, "ABA fence " + what);

            // Two commands holding the same fence race: exactly one wins, the other sees the moved plan state.
            var shared = Fence(player.Read());
            using var gate = new Barrier(2);
            Task<PlayerCommandResult> Add(string treeId) => Task.Run(() =>
            {
                gate.SignalAndWait();
                return player.AddPending(shared, treeId, "race");
            });
            var a = Add("P30-011");
            var b = Add("P30-012");
            Task.WaitAll(a, b);
            var results = new[] { a.Result, b.Result };
            T.Eq(1, results.Count(r => r.RefusalCode is null), "one concurrent winner");
            T.Eq("PlayerPlanStateStale", results.Single(r => r.RefusalCode is not null).RefusalCode, "the loser is plan-state stale");
            T.Eq(1, player.Read().Pending.Count, "exactly one order was added");
        });

        T.Add("player PS-04: wrong session, stale source and stale plan state refuse all six commands, zero mutation", () =>
        {
            var probe = Open().ReadPlayerView().Source;
            var tokenFields = typeof(WorkSourceToken).GetProperties().Select(p => p.Name).Order(StringComparer.Ordinal);
            T.True(tokenFields.SequenceEqual(OneFieldChanges(probe).Select(change => change.Field).Order(StringComparer.Ordinal)),
                "every source token field is perturbed below");
            string otherSession = Open().SessionId;
            foreach (bool approved in new[] { false, true })
            {
                var c = Open();
                var player = new PlayerWorkPlanAdapter(c);
                var view = Ok(player.AddPending(Fence(player.Read()), "P30-001", "fence probe"), "setup add");
                if (approved) view = Ok(player.Approve(Fence(view)), "setup approve");
                string state = approved ? "approved" : "pending";
                string planId = view.Approved?.PlanId ?? "no-plan";
                string key = view.Approved?.ExecutionKey ?? "no-plan:execute";
                var good = Fence(view);

                var wrongSessions = new[]
                {
                    good with { SessionId = otherSession },
                    good with { SessionId = "X" + good.SessionId[1..] },
                    good with { SessionId = "" },
                    null!,
                };
                foreach (var bad in wrongSessions)
                    foreach (var (what, call) in AllCommands(player, bad, planId, key))
                        RefusedWithoutMutation(c, "PlayerSessionMismatch", call, $"{state} {what} wrong session");

                foreach (var (field, token) in OneFieldChanges(good.Source).Append(("null source", null!)))
                    foreach (var (what, call) in AllCommands(player, good with { Source = token }, planId, key))
                        RefusedWithoutMutation(c, "PlayerSourceStale", call, $"{state} {what} stale {field}");

                foreach (int delta in new[] { -1, 1 })
                    foreach (var (what, call) in AllCommands(player,
                        good with { PlanStateRevision = good.PlanStateRevision + delta }, planId, key))
                        RefusedWithoutMutation(c, "PlayerPlanStateStale", call, $"{state} {what} plan-state {delta}");

                // Fence order: the session is checked before the source, and the source before the plan state.
                var movedSource = good.Source with { WorkRevision = good.Source.WorkRevision + 1 };
                foreach (var (what, call) in AllCommands(player,
                    new PlayerFence(otherSession, movedSource, good.PlanStateRevision + 1), planId, key))
                    RefusedWithoutMutation(c, "PlayerSessionMismatch", call, $"{state} {what} session checked first");
                foreach (var (what, call) in AllCommands(player,
                    good with { Source = movedSource, PlanStateRevision = good.PlanStateRevision + 1 }, planId, key))
                    RefusedWithoutMutation(c, "PlayerSourceStale", call, $"{state} {what} source checked before plan state");

                // Control: the untouched fence still commits, so every refusal above came from the fence alone.
                var control = approved ? player.Execute(good, planId, key) : player.Approve(good);
                T.True(control.RefusalCode is null, $"{state}: the good fence still commits ({control.RefusalCode})");
            }
        });

        T.Add("player PS-05: Add, Remove, Approve and Cancel make zero Fell and keep the token; Approve mints the PlanId", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var start = player.Read();
            byte[] frame = Bytes(start.Frame);
            void NoFell(PlayerSessionView v, string what)
            {
                T.True(frame.SequenceEqual(Bytes(v.Frame)), what + ": roster, stand, events and clocks unchanged");
                T.True(frame.SequenceEqual(Bytes(c.Frame)), what + ": the legacy Frame is unchanged too");
                T.Eq(start.Source, v.Source, what + ": source token unchanged");
            }

            var v = Ok(player.AddPending(Fence(start), "P30-020", "release crop tree P30-021"), "add");
            NoFell(v, "Add");
            T.Eq(new WorkOrder("P30-020", WorkAction.Fell, "release crop tree P30-021"), v.Pending.Single(), "exact pending order");
            v = Ok(player.AddPending(Fence(v), "P30-030", "second"), "add second");
            v = Ok(player.RemovePending(Fence(v), "P30-030"), "remove");
            NoFell(v, "Remove");
            v = Ok(player.Approve(Fence(v)), "approve");
            NoFell(v, "Approve");
            var plan = v.Approved!;
            T.Eq($"{v.SessionId}:plan:{v.PlanStateRevision}", plan.PlanId, "PlanId minted from the published plan-state revision");
            T.Eq(plan.PlanId + ":execute", plan.ExecutionKey, "ExecutionKey minted from the PlanId");
            T.Eq(start.Source, plan.Source, "the approved plan is bound to the current token");
            T.True(plan.Orders.Select(o => o.TreeId).SequenceEqual(new[] { "P30-020" }), "exactly the pending orders were approved");
            T.Eq(0, v.Pending.Count, "pending moved into the approved plan");
            v = Ok(player.CancelApproved(Fence(v)), "cancel");
            NoFell(v, "Cancel");
            T.True(v.Approved is null && v.Pending.Count == 0, "cancel does not return the orders to pending");
            v = Ok(player.AddPending(Fence(v), "P30-020", "again"), "re-add");
            v = Ok(player.Approve(Fence(v)), "re-approve");
            T.True(v.Approved!.PlanId != plan.PlanId, "a second approval mints a different PlanId");

            // A caller cannot supply a PlanId: Approve takes only the fence, and only Execute names a plan.
            var approve = typeof(PlayerWorkPlanAdapter).GetMethod(nameof(PlayerWorkPlanAdapter.Approve))!;
            T.True(approve.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(PlayerFence) }),
                "Approve(fence) takes nothing else");
            var namingPlan = typeof(PlayerWorkPlanAdapter)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(m => m.GetParameters().Any(p => p.Name!.Contains("plan", StringComparison.OrdinalIgnoreCase)))
                .Select(m => m.Name).ToArray();
            T.True(namingPlan.SequenceEqual(new[] { "Execute" }), "only Execute names a plan: " + string.Join(",", namingPlan));
        });

        T.Add("player PS-06: Execute fells exactly the approved IDs once with the typed cause; clocks kept, work +1", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v = Ok(player.AddPending(Fence(player.Read()), "P30-040", "thin around P30-041"), "add first");
            v = Ok(player.AddPending(Fence(v), "P30-007", "remove a suppressed stem"), "add second");
            var before = Ok(player.Approve(Fence(v)), "approve");
            var plan = before.Approved!;
            var result = player.Execute(Fence(before), plan.PlanId, plan.ExecutionKey);
            var after = Ok(result, "execute");
            var execution = result.Execution!;
            T.True(!execution.Replayed, "a real first execution");
            T.Eq(plan.ExecutionKey, execution.ExecutionKey, "the minted key is recorded");
            T.Eq(before.Source.EcologicalYear, execution.EcologicalYear, "the Fell happens in the current year");
            var fells = execution.Event.Fells;
            T.True(fells.Select(f => f.TreeId).SequenceEqual(new[] { "P30-007", "P30-040" }), "exactly the approved IDs");
            foreach (var fell in fells)
            {
                T.Eq(FellCause.Harvest, fell.Cause, fell.TreeId + " typed cause");
                T.Eq(FellCause.Mechanism, fell.Mechanism, fell.TreeId + " mechanism");
                T.Eq(plan.PlanId, fell.PlanId, fell.TreeId + " minted PlanId");
                T.Eq(plan.ExecutionKey, fell.ExecutionKey, fell.TreeId + " minted key");
                T.Eq(plan.Orders.Single(o => o.TreeId == fell.TreeId).Reason, fell.Reason, fell.TreeId + " reason");
                T.Eq(before.Source.WorkRevision + 1, fell.WorkRevision, fell.TreeId + " work revision");
            }
            for (int i = 0; i < before.Frame.Trees.Count; i++)
            {
                var was = before.Frame.Trees[i];
                var now = after.Frame.Trees[i];
                T.Eq(was.TreeId, now.TreeId, "same exact ID at index " + i);
                var expected = plan.Orders.Any(o => o.TreeId == was.TreeId)
                    ? was with { Death = new AnnualDeath(was.TreeId, before.Source.EcologicalYear, FellCause.Harvest, FellCause.Mechanism) }
                    : was;
                T.Eq(expected, now, was.TreeId + " roster entry");
            }
            T.Eq(before.Source.EcologicalYear, after.Source.EcologicalYear, "ecological year unchanged");
            T.Eq(before.Source.AnnualRevision, after.Source.AnnualRevision, "annual revision unchanged");
            T.Eq(before.Source.WorkRevision + 1, after.Source.WorkRevision, "work revision +1");
            T.Eq(before.PlanStateRevision + 1, after.PlanStateRevision, "plan state +1 (Q-2)");
            T.Eq(WorkRules.Summarize(before.Frame.Trees, Area), execution.Event.Before, "before count/BA from WorkRules.Summarize");
            T.Eq(WorkRules.Summarize(after.Frame.Trees, Area), execution.Event.After, "after count/BA from WorkRules.Summarize");
            T.Eq(execution.Event.After, after.Frame.Stand, "the frame stand is the event's after");
            T.Eq(98, after.Frame.Stand.LivingCount, "two trees felled");
            T.True(after.Approved is null && after.Pending.Count == 0, "the approval is cleared");
            T.Eq(execution.Event, after.Frame.Events.Single(), "exactly one event, in the ledger");
        });

        T.Add("player PS-07: same key, PlanId, original source and session replays the receipt with zero mutation", () =>
        {
            var (c, player, approvedView, first) = ExecutedSession(Open(), "P30-050");
            var plan = approvedView.Approved!;
            var root = ReadRoot(c);
            var current = Bytes(player.Read());
            var replay = player.Execute(Fence(approvedView), plan.PlanId, plan.ExecutionKey);
            T.True(replay.RefusalCode is null, "a replay is not a refusal");
            T.True(replay.Execution!.Replayed, "Replayed = true");
            T.Eq(first.Execution!.Event, replay.Execution.Event, "the recorded event is returned");
            T.True(ReferenceEquals(root, ReadRoot(c)), "the replay publishes nothing");
            T.True(current.SequenceEqual(Bytes(replay.View)), "the replay returns the unchanged current view");
            T.Eq(1, replay.View.Frame.Events.Count, "no second event");
            T.Eq(99, replay.View.Frame.Stand.LivingCount, "no second Fell");

            Ok(player.Advance(Fence(player.Read()), consentToCancelWork: false), "a later year");
            var later = ReadRoot(c);
            var again = player.Execute(Fence(approvedView), plan.PlanId, plan.ExecutionKey);
            T.True(again.Execution!.Replayed && ReferenceEquals(later, ReadRoot(c)), "a replay after a later year is still a no-op");
        });

        T.Add("player PS-08: the same key under another PlanId or a later source conflicts, zero mutation", () =>
        {
            var (c, player, approvedView, _) = ExecutedSession(Open(), "P30-060");
            var first = approvedView.Approved!;
            var v = Ok(player.AddPending(Fence(player.Read()), "P30-061", "second plan"), "add");
            v = Ok(player.Approve(Fence(v)), "approve the second plan");
            var second = v.Approved!;
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(v), second.PlanId, first.ExecutionKey), "second PlanId, first key");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approvedView), second.PlanId, first.ExecutionKey), "original fence, second PlanId");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approvedView), "invented-plan", first.ExecutionKey), "invented PlanId");
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(v), first.PlanId, first.ExecutionKey), "same PlanId with a later source");
            T.True(Bytes(second).SequenceEqual(Bytes(player.Read().Approved!)), "the second plan is still approved");
            var control = player.Execute(Fence(v), second.PlanId, second.ExecutionKey);
            T.True(control.RefusalCode is null && !control.Execution!.Replayed, "the second plan executes under its own key");
        });

        T.Add("player PS-09: another session's fence is PlayerSessionMismatch, also for a replay, zero mutation", () =>
        {
            var (a, playerA, approvedA, _) = ExecutedSession(Open(), "P30-070");
            var b = Open();
            var playerB = new PlayerWorkPlanAdapter(b);
            T.True(a.SessionId != b.SessionId, "two sessions have different identities");
            foreach (var id in new[] { a.SessionId, b.SessionId })
                T.True(id.Length == 32 && id.All(ch => ch is >= '0' and <= '9' or >= 'a' and <= 'f'), "128-bit lowercase hex session id");
            var planA = approvedA.Approved!;
            var vb = Ok(playerB.AddPending(Fence(playerB.Read()), "P30-070", "session B"), "B add");
            vb = Ok(playerB.Approve(Fence(vb)), "B approve");
            // A's original fence has the same source token as B's genesis: only the session differs.
            T.Eq(approvedA.Source, vb.Source, "same fixture and clocks in both sessions");
            RefusedWithoutMutation(b, "PlayerSessionMismatch",
                () => playerB.Execute(Fence(approvedA), planA.PlanId, planA.ExecutionKey), "A's fence on B");
            RefusedWithoutMutation(a, "PlayerSessionMismatch",
                () => playerA.Execute(Fence(approvedA) with { SessionId = b.SessionId }, planA.PlanId, planA.ExecutionKey),
                "a replay carrying B's session on A");
            var foreign = Fence(vb) with { SessionId = a.SessionId };
            var planB = vb.Approved!;
            foreach (var (what, call) in AllCommands(playerB, foreign, planB.PlanId, planB.ExecutionKey))
                RefusedWithoutMutation(b, "PlayerSessionMismatch", call, "B with A's session id: " + what);
        });

        T.Add("player PS-10: a pre-Fell fence is stale for Add, Approve and Advance after the Fell", () =>
        {
            var (c, player, approvedView, _) = ExecutedSession(Open(), "P30-080");
            var preFell = Fence(approvedView);
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.AddPending(preFell, "P30-081", "stale"), "Add");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.Approve(preFell), "Approve");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.Advance(preFell, consentToCancelWork: true), "Advance");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.Advance(preFell, consentToCancelWork: false), "Advance (no consent)");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.RemovePending(preFell, "P30-080"), "Remove");
            RefusedWithoutMutation(c, "PlayerSourceStale", () => player.CancelApproved(preFell), "Cancel");
            Ok(player.AddPending(Fence(player.Read()), "P30-081", "fresh fence"), "a fresh fence commits");
        });

        T.Add("player PS-11: a pre-advance fence is stale after Year+1, although a no-work year keeps the plan state", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var year0 = player.Read();
            var year1 = Ok(player.Advance(Fence(year0), consentToCancelWork: false), "advance");
            T.Eq(1, year1.Source.EcologicalYear, "Year+1 committed");
            T.Eq(year0.PlanStateRevision, year1.PlanStateRevision, "a no-work year leaves the plan state");
            foreach (var (what, call) in AllCommands(player, Fence(year0), "no-plan", "no-plan:execute"))
                RefusedWithoutMutation(c, "PlayerSourceStale", call, "pre-advance fence " + what);
        });

        T.Add("player PS-12: an all-live batch is refused as a whole with zero mutation and the approval stays", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v = player.Read();
            foreach (var tree in v.Frame.Trees) v = Ok(player.AddPending(Fence(v), tree.TreeId, "clear-fell probe"), "add " + tree.TreeId);
            v = Ok(player.Approve(Fence(v)), "approve all 100");
            var plan = v.Approved!;
            T.Eq(100, plan.Orders.Count, "every living tree is in the batch");
            RefusedWithoutMutation(c, "WorkWouldLeaveNoLivingStems",
                () => player.Execute(Fence(v), plan.PlanId, plan.ExecutionKey), "all-live batch");
            var after = player.Read();
            T.Eq(plan.PlanId, after.Approved?.PlanId, "the approved plan is still present");
            T.Eq(100, after.Frame.Stand.LivingCount, "the stand is untouched");
            T.Eq(0, ReadRoot(c).Executed.Count, "no execution key was recorded");
        });

        T.Add("player PS-13: Advance with unexecuted work needs consent; consent cancels at zero Fell and runs one year", () =>
        {
            foreach (bool approve in new[] { false, true })
            {
                var c = Open();
                var player = new PlayerWorkPlanAdapter(c);
                var v = Ok(player.AddPending(Fence(player.Read()), "P30-090", "unexecuted at the year boundary"), "add");
                if (approve) v = Ok(player.Approve(Fence(v)), "approve");
                RefusedWithoutMutation(c, "WorkPendingConsentRequired",
                    () => player.Advance(Fence(v), consentToCancelWork: false), "advance without consent");
                var result = player.Advance(Fence(v), consentToCancelWork: true);
                var after = Ok(result, "consented advance");
                var control = Open().Advance(0, 0);
                T.True(Bytes(control).SequenceEqual(Bytes(result.Annual!)), "the same genuine year as a no-work control");
                T.True(after.Pending.Count == 0 && after.Approved is null, "unexecuted work cancelled");
                T.Eq(v.PlanStateRevision + 1, after.PlanStateRevision, "the consented cancellation is +1 plan state");
                T.Eq(0, after.Source.WorkRevision, "no work revision");
                T.Eq(0, after.Frame.Events.Count, "no work event");
                T.Eq(0, after.Frame.Trees.Count(t => t.Death?.Cause == FellCause.Harvest), "zero Fell");
                T.Eq(1, after.Source.EcologicalYear, "one genuine year");
                var record = after.AnnualHistory.Single();
                T.True(record.CancelledUnexecutedWork, "the year records the cancellation");
                T.Eq((0, 1, 1, 0), (record.YearBefore, record.YearAfter, record.AnnualRevisionAfter, record.WorkRevision), "year record clocks");
                T.Eq(result.Annual!.Before, record.Before, "record before stand is the report's");
                T.Eq(result.Annual.After, record.After, "record after stand is the report's");
                T.True(record.NaturalDeaths.SequenceEqual(result.Annual.Deaths), "record deaths are the report's");
                if (approve)
                    RefusedWithoutMutation(c, "WorkPlanNotApproved",
                        () => player.Execute(Fence(after), v.Approved!.PlanId, v.Approved.ExecutionKey), "the cancelled plan");
            }
        });

        T.Add("player PS-14: after a Fell, Year+1 survivors equal AnnualRules.Calculate(post-Fell); deaths keep causes", () =>
        {
            var (_, player, _, executed) = ExecutedSession(Open(), "P30-055");
            var view = executed.View;
            var fellDeath = view.Frame.Trees.Single(t => t.TreeId == "P30-055").Death!;
            T.Eq(new AnnualDeath("P30-055", 0, FellCause.Harvest, FellCause.Mechanism), fellDeath, "typed Fell death");
            var naturalDeaths = new List<AnnualDeath>();
            for (int year = 1; year <= 10; year++)
            {
                var expected = AnnualRules.Calculate(new AnnualState(view.Source.EcologicalYear, view.Frame.Trees), Seed, Area);
                var result = player.Advance(Fence(view), consentToCancelWork: false);
                view = Ok(result, "year " + year);
                T.True(Bytes(expected).SequenceEqual(Bytes(result.Annual!)), $"year {year}: report is AnnualRules.Calculate(post-Fell state)");
                T.True(view.Frame.Trees.SequenceEqual(expected.Trees.Select(row => row.After)), $"year {year}: roster is the calculated survivors");
                T.Eq(fellDeath, view.Frame.Trees.Single(t => t.TreeId == "P30-055").Death, $"year {year}: felled tree keeps its Fell death");
                foreach (var death in result.Annual!.Deaths)
                {
                    T.Eq("self-thinning", death.Cause, "a natural death keeps its own cause");
                    T.True(death.Mechanism is "hazard" or "boundary", "natural mechanism");
                    T.Eq(year, death.EcologicalYear, "natural death year");
                }
                naturalDeaths.AddRange(result.Annual.Deaths);
                foreach (var earlier in naturalDeaths)
                    T.Eq(earlier, view.Frame.Trees.Single(t => t.TreeId == earlier.TreeId).Death, "earlier natural deaths stay as recorded");
                var record = view.AnnualHistory.Last();
                T.True(record.NaturalDeaths.SequenceEqual(result.Annual.Deaths) && !record.CancelledUnexecutedWork,
                    "the history keeps this year's natural deaths");
            }
            T.True(naturalDeaths.Count > 0, "natural deaths were exercised after the Fell");
            T.Eq(10, view.AnnualHistory.Count, "ten year records");
            T.Eq(1, view.Frame.Events.Count, "the Fell event is preserved");
            Console.WriteLine($"PS-14 natural deaths after the Fell over ten years: {naturalDeaths.Count}");
        });

        T.Add("player PS-15: a second decision cycle in Year+1 with a fresh fence adds, approves and executes", () =>
        {
            var (_, player, _, executed) = ExecutedSession(Open(), "P30-015");
            var year1 = Ok(player.Advance(Fence(executed.View), consentToCancelWork: false), "year 1");
            string next = year1.Frame.Trees.First(t => t.IsLiving).TreeId;
            var v = Ok(player.AddPending(Fence(year1), next, "second cycle"), "add");
            v = Ok(player.Approve(Fence(v)), "approve");
            var plan = v.Approved!;
            T.Eq(1, plan.Source.EcologicalYear, "bound to year 1");
            T.Eq(1, plan.Source.WorkRevision, "bound to work revision 1");
            T.Eq($"{v.SessionId}:plan:{v.PlanStateRevision}", plan.PlanId, "a fresh minted PlanId");
            var second = player.Execute(Fence(v), plan.PlanId, plan.ExecutionKey);
            var after = Ok(second, "second execute");
            T.True(!second.Execution!.Replayed, "a real second Fell");
            T.Eq(1, second.Execution.EcologicalYear, "the Fell happens in year 1");
            T.Eq(2, after.Source.WorkRevision, "work revision 2");
            T.Eq(2, after.Frame.Events.Count, "two events");
            T.True(!after.Frame.Trees.Single(t => t.TreeId == next).IsLiving, "the second tree is felled");
            var year2 = Ok(player.Advance(Fence(after), consentToCancelWork: false), "year 2");
            T.Eq(2, year2.Source.EcologicalYear, "a second genuine year");
            T.Eq(2, year2.AnnualHistory.Count, "two year records");
        });

        T.Add("player PS-16: an injected fault through the internal seams leaves zero mutation, incl. plan state and history", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v = Ok(player.AddPending(Fence(player.Read()), "P30-025", "fault probe"), "add");
            v = Ok(player.Approve(Fence(v)), "approve");
            var plan = v.Approved!;
            FaultWithoutMutation(c, () => c.Execute(Fence(v), plan.PlanId, plan.ExecutionKey, beforePublish: Inject), "Execute");
            var committed = c.Execute(Fence(v), plan.PlanId, plan.ExecutionKey);
            T.True(!committed.Execution!.Replayed, "the retry is a real first execution");
            T.Eq(v.PlanStateRevision + 1, committed.View.PlanStateRevision, "the retry moves the plan state once");

            var w = Ok(player.AddPending(Fence(committed.View), "P30-026", "consent fault probe"), "add");
            FaultWithoutMutation(c, () => c.Advance(Fence(w), consentToCancelWork: true, beforeCommit: Inject), "consented Advance");
            var advanced = c.Advance(Fence(w), consentToCancelWork: true);
            T.Eq(w.PlanStateRevision + 1, advanced.View.PlanStateRevision, "the retry cancels and advances once");
            T.Eq(1, advanced.View.AnnualHistory.Count, "one year record after the retry");
            FaultWithoutMutation(c, () => c.Advance(Fence(advanced.View), consentToCancelWork: false, beforeCommit: Inject),
                "no-work Advance");
        });

        T.Add("player PS-17: view collections are read-only copies, and later commands never change an earlier view", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v0 = player.Read();
            var v1 = Ok(player.AddPending(Fence(v0), "P30-033", "immutability"), "add");
            var v2 = Ok(player.Approve(Fence(v1)), "approve");
            var plan = v2.Approved!;
            var v3 = Ok(player.Execute(Fence(v2), plan.PlanId, plan.ExecutionKey), "execute");
            var v4 = Ok(player.Advance(Fence(v3), consentToCancelWork: false), "advance");
            var views = new[] { v0, v1, v2, v3, v4 };
            var frozen = views.Select(view => Bytes(view)).ToArray();
            var v5 = Ok(player.AddPending(Fence(v4), "P30-034", "later"), "later add");
            Ok(player.Advance(Fence(v5), consentToCancelWork: true), "later consented advance");
            for (int i = 0; i < views.Length; i++)
                T.True(frozen[i].SequenceEqual(Bytes(views[i])), $"view {i} is unchanged by later commands");

            ReadOnly(v1.Pending, "Pending");
            ReadOnly(plan.Orders, "Approved.Orders");
            ReadOnly(v4.Frame.Trees, "Frame.Trees");
            ReadOnly(v4.Frame.Events, "Frame.Events");
            ReadOnly(v4.AnnualHistory, "AnnualHistory");
            ReadOnly(v4.AnnualHistory[0].NaturalDeaths, "AnnualHistory.NaturalDeaths");
            T.True(!ReferenceEquals(ReadRoot(c).Snapshot.State.Trees, player.Read().Frame.Trees), "the roster is copied, not shared");
            T.True(Bytes(c.Frame).SequenceEqual(Bytes(player.Read().Frame)), "the view frame equals the legacy Frame");
        });

        T.Add("player PS-18: the public facade shows 100 exact fixture IDs in order, one scope, and the fixed labels", () =>
        {
            var player = PlayerWorkPlanAdapter.Open(fixtureDirectory);
            var view = player.Read();
            var manifest = DerivedManifest.Parse(File.ReadAllBytes(manifestPath));
            T.True(view.Frame.Trees.Select(t => t.TreeId).SequenceEqual(manifest.Trees.Select(t => t.TreeId)),
                "the 100 exact original IDs in fixture order");
            T.Eq(100, view.Frame.Trees.Count, "100 trees");
            T.True(view.Frame.Trees.All(t => t.IsLiving), "all living at genesis");
            T.Eq(ManagementScope.WholeUndividedThirtyMetres, view.Source.ManagementScopeId, "token scope");
            T.Eq(ManagementScope.WholeUndividedThirtyMetres, view.Frame.ManagementScopeId, "frame scope");
            var record = DerivedCatalog.Find(ManifestGenerator.FixtureId)!;
            T.Eq(P1DomainWire.Name(P1Domain.DerivedP1ThirtyMetreExperimentV1), view.Source.Domain, "domain");
            T.Eq(ManifestGenerator.FixtureId, view.Source.FixtureId, "fixture id");
            T.Eq(record.FileSha256, view.Source.FixtureFileSha256, "fixture file SHA-256");
            T.Eq(record.ContentHash, view.Source.FixtureContentHash, "fixture content hash");
            T.Eq(new PlayerMapExtent(-15.0, -15.0, 30.0, Area), view.Extent, "map extent");
            T.True(view.PlanStateRevision == 0 && view.Pending.Count == 0 && view.Approved is null && view.AnnualHistory.Count == 0,
                "a fresh session");

            var expected = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [nameof(PlayerLabels.Title)] = "Synthetic 30 m Sitka experiment",
                [nameof(PlayerLabels.Disclosure)] = "SYNTHETIC CLOSED ADULT SITKA EXPERIMENT. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.",
                [nameof(PlayerLabels.ManagementArea)] = "One undivided management area — MS-P30-UNDIVIDED-30M-V1 (not a rack)",
                [nameof(PlayerLabels.ComparisonExtension)] = "Comparison extension: DISABLED — Manual/NotCovered",
                [nameof(PlayerLabels.Money)] = "Costs, revenue, budget: UNAVAILABLE (not €0)",
                [nameof(PlayerLabels.SessionOnly)] = "Session only — nothing is saved",
                [nameof(PlayerLabels.NotCovered)] = "UNAVAILABLE / NOT COVERED",
                [nameof(PlayerLabels.ThresholdInformational)] = "170 mm is fixture metadata only; it does not restrict Fell (D-093)",
                [nameof(PlayerLabels.AllLiveBatchRefused)] = "A batch that would leave no living tree is refused as a whole (D-093)",
                [nameof(PlayerLabels.AddToWorkPlan)] = "Add to Work Plan — no tree is felled",
                [nameof(PlayerLabels.RemoveFromWorkPlan)] = "Remove from Work Plan",
                [nameof(PlayerLabels.ApproveWholePlan)] = "Approve whole Work Plan — 0 trees felled",
                [nameof(PlayerLabels.CancelApprovedPlan)] = "Cancel approved plan",
                [nameof(PlayerLabels.ExecuteApprovedPlan)] = "Execute approved plan",
                [nameof(PlayerLabels.ExecuteConfirmation)] = "Fells exactly {n} named trees in Year {y}, or refuses all",
                [nameof(PlayerLabels.ExecuteConfirmationOneTree)] = "Fells exactly {n} named tree in Year {y}, or refuses all",
                [nameof(PlayerLabels.AlreadyExecuted)] = "Already executed — no additional Fell",
                [nameof(PlayerLabels.AdvanceYear)] = "Advance one ecological year",
                [nameof(PlayerLabels.ConsentKeepOrders)] = "Keep orders — stay in Year {y}",
                [nameof(PlayerLabels.ConsentCancelAndAdvance)] = "Cancel unexecuted work and advance one year",
                [nameof(PlayerLabels.NothingChanged)] = "Nothing changed",
                [nameof(PlayerLabels.OutcomeUnconfirmed)] = "Outcome unconfirmed — re-read current state",
                [nameof(PlayerLabels.SelectionAmbiguous)] = "Selection ambiguous — choose from the list",
                [nameof(PlayerLabels.CrownsOverlap)] = "{n} crowns overlap here",
                [nameof(PlayerLabels.StatusLiving)] = "Living",
                [nameof(PlayerLabels.StatusPendingFell)] = "Pending Fell (P)",
                [nameof(PlayerLabels.StatusApprovedFell)] = "Approved Fell (A)",
                [nameof(PlayerLabels.StatusFelled)] = "Felled Y{y}",
                [nameof(PlayerLabels.StatusNaturalDead)] = "Dead (natural) Y{y}",
            };
            var fields = typeof(PlayerLabels).GetFields(BindingFlags.Public | BindingFlags.Static);
            T.True(fields.Select(f => f.Name).Order(StringComparer.Ordinal).SequenceEqual(expected.Keys.Order(StringComparer.Ordinal)),
                "every PlayerLabels constant is checked here, and no other exists");
            string[] banned = { "corridor", "entrance", "proposal", "propose", "sample", "equivalent", "marking", "comparator", "headland" };
            foreach (var field in fields)
            {
                T.True(field.IsLiteral, field.Name + " is a constant");
                string text = (string)field.GetValue(null)!;
                T.Eq(expected[field.Name], text, "label " + field.Name);
                if (text.Contains("rack", StringComparison.OrdinalIgnoreCase))
                    T.Eq(nameof(PlayerLabels.ManagementArea), field.Name, "only the not-a-rack disclaimer mentions a rack");
                if (text.Contains('€'))
                    T.Eq(nameof(PlayerLabels.Money), field.Name, "only the UNAVAILABLE disclaimer mentions money");
                foreach (var word in banned)
                    T.True(!text.Contains(word, StringComparison.OrdinalIgnoreCase), $"{field.Name} mentions {word}");
            }
            T.Eq(P1Disclosure.Derived30, PlayerLabels.Disclosure, "the disclosure is the fixed P1 text");
            T.True(PlayerLabels.ManagementArea.Contains(view.Source.ManagementScopeId, StringComparison.Ordinal), "area label names the scope");
            T.Eq(170L, view.Source.ActionEligibleDbhThresholdMm, "the 170 mm label matches the token threshold");
        });

        T.Add("player PS-19: the public facade has exactly the §6 members and no proposal, sample, seam or plan object", () =>
        {
            var adapter = typeof(PlayerWorkPlanAdapter);
            const BindingFlags Declared = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            var methods = adapter.GetMethods(Declared).Select(m => m.Name).Order(StringComparer.Ordinal).ToArray();
            T.True(methods.SequenceEqual(new[] { "AddPending", "Advance", "Approve", "CancelApproved", "Execute", "HitTest", "Open", "Read", "RemovePending" }),
                "exact public adapter methods: " + string.Join(",", methods));
            T.Eq(0, adapter.GetConstructors().Length, "no public constructor; Open is the only entry");
            T.Eq(0, adapter.GetProperties(Declared).Length + adapter.GetFields(Declared).Length + adapter.GetEvents(Declared).Length,
                "no public property, field or event");
            T.True(adapter.IsSealed, "the adapter cannot be subclassed");
            T.True(adapter.GetMethod(nameof(PlayerWorkPlanAdapter.Advance))!.GetParameters().Select(p => p.ParameterType)
                .SequenceEqual(new[] { typeof(PlayerFence), typeof(bool) }), "only the fenced Advance(fence, consent) is public");
            T.True(adapter.GetMethod(nameof(PlayerWorkPlanAdapter.Execute))!.GetParameters().Select(p => p.ParameterType)
                .SequenceEqual(new[] { typeof(PlayerFence), typeof(string), typeof(string) }), "Execute(fence, planId, executionKey)");

            var newTypes = adapter.Assembly.GetExportedTypes()
                .Where(t => t.Name.StartsWith("Player", StringComparison.Ordinal) || t.Name == nameof(AnnualYearRecord))
                .OrderBy(t => t.Name, StringComparer.Ordinal).ToArray();
            T.True(newTypes.Select(t => t.Name).SequenceEqual(new[] { "AnnualYearRecord", "PlayerApprovedPlan", "PlayerCommandResult",
                "PlayerFence", "PlayerHitTestResult", "PlayerLabels", "PlayerMapExtent", "PlayerMapItem", "PlayerMapScene",
                "PlayerMapStatus", "PlayerSessionView", "PlayerWorkPlanAdapter" }),
                "exact new public types: " + string.Join(",", newTypes.Select(t => t.Name)));

            string[] forbidden = { "proposal", "propose", "comparator", "sample", "seam", "beforepublish", "beforecommit",
                "inject", "economy", "persist", "rack" };
            foreach (var type in newTypes)
            foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                string where = type.Name + "." + member.Name;
                T.True(!forbidden.Any(word => member.Name.Contains(word, StringComparison.OrdinalIgnoreCase)), where + " is a forbidden member");
                if (member is FieldInfo field)
                    T.True(field.IsLiteral || field.IsSpecialName, where + " is a mutable public field");
                if (member is PropertyInfo property)
                    T.True(!IsMutableCollection(property.PropertyType), where + " exposes a mutable collection");
                if (member is MethodInfo method)
                    T.True(!IsMutableCollection(method.ReturnType), where + " returns a mutable collection");
                if (member is MethodBase callable)
                    foreach (var parameter in callable.GetParameters())
                    {
                        T.True(!forbidden.Any(word => parameter.Name!.Contains(word, StringComparison.OrdinalIgnoreCase)),
                            where + " has a forbidden parameter " + parameter.Name);
                        T.True(parameter.ParameterType != typeof(WorkPlan), where + " accepts a caller-built WorkPlan");
                        T.True(!typeof(Delegate).IsAssignableFrom(parameter.ParameterType), where + " accepts a delegate seam");
                    }
            }
        });

        T.Add("player PS-20: a first Execute must echo the engine-minted key; an unminted or future key is refused, zero mutation", () =>
        {
            // Tier-2 D3: before this rule, a plan could be executed under the key a later plan would be minted, which then
            // blocked that later plan with WorkExecutionKeyConflict.
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v = Ok(player.AddPending(Fence(player.Read()), "P30-005", "PS-20 plan A"), "add A");
            var approvedA = Ok(player.Approve(Fence(v)), "approve A");
            var planA = approvedA.Approved!;
            int n = approvedA.PlanStateRevision;
            T.Eq($"{approvedA.SessionId}:plan:{n}:execute", planA.ExecutionKey, "plan A's minted key");

            // Execute, Add and Approve each move the plan state by one, so plan B below is minted with exactly this key.
            string futureKey = $"{approvedA.SessionId}:plan:{n + 3}:execute";
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approvedA), planA.PlanId, futureKey), "plan A under plan B's future key");
            foreach (string unminted in new[] { "invented", planA.PlanId, planA.ExecutionKey + "x", " " + planA.ExecutionKey,
                planA.ExecutionKey.ToUpperInvariant(), $"{approvedA.SessionId}:plan:{n - 1}:execute" })
                RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                    () => player.Execute(Fence(approvedA), planA.PlanId, unminted), "unminted key '" + unminted + "'");
            T.Eq(0, ReadRoot(c).Executed.Count, "no unminted key was recorded");
            T.Eq(planA.PlanId, player.Read().Approved?.PlanId, "plan A is still approved");

            // The legitimate plan still executes under its own minted key.
            var executedA = player.Execute(Fence(approvedA), planA.PlanId, planA.ExecutionKey);
            var afterA = Ok(executedA, "execute A with its minted key");
            T.True(!executedA.Execution!.Replayed, "a real first execution of A");
            T.True(executedA.Execution.Event.Fells.Select(f => f.TreeId).SequenceEqual(new[] { "P30-005" }), "A fells exactly P30-005");

            // Plan B is minted with the key the refused call named, and executes normally: no WorkExecutionKeyConflict.
            v = Ok(player.AddPending(Fence(afterA), "P30-006", "PS-20 plan B"), "add B");
            var approvedB = Ok(player.Approve(Fence(v)), "approve B");
            var planB = approvedB.Approved!;
            T.Eq(futureKey, planB.ExecutionKey, "plan B's minted key is the key the refused call named");
            var executedB = player.Execute(Fence(approvedB), planB.PlanId, planB.ExecutionKey);
            var afterB = Ok(executedB, "execute B with its minted key");
            T.True(!executedB.Execution!.Replayed, "a real first execution of B");
            T.True(executedB.Execution.Event.Fells.Select(f => f.TreeId).SequenceEqual(new[] { "P30-006" }), "B fells exactly P30-006");
            T.Eq(2, afterB.Frame.Events.Count, "two events");
            T.Eq(2, afterB.Source.WorkRevision, "work revision 2");

            // The A-2 replay rule is unchanged: the same key, PlanId, original source and session return each receipt.
            var root = ReadRoot(c);
            var replayA = player.Execute(Fence(approvedA), planA.PlanId, planA.ExecutionKey);
            T.True(replayA.RefusalCode is null && replayA.Execution!.Replayed, "A replays");
            T.Eq(executedA.Execution.Event, replayA.Execution!.Event, "A's recorded event is returned");
            var replayB = player.Execute(Fence(approvedB), planB.PlanId, planB.ExecutionKey);
            T.True(replayB.RefusalCode is null && replayB.Execution!.Replayed, "B replays");
            T.Eq(executedB.Execution.Event, replayB.Execution!.Event, "B's recorded event is returned");
            T.True(ReferenceEquals(root, ReadRoot(c)), "the replays publish nothing");
            // The wrong-PlanId refusal is unchanged too.
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approvedB), planA.PlanId, planB.ExecutionKey), "B's key under A's PlanId");

            // The minted key of an earlier, cancelled plan is not the current plan's key either.
            v = Ok(player.AddPending(Fence(afterB), "P30-007", "PS-20 plan X"), "add X");
            var approvedX = Ok(player.Approve(Fence(v)), "approve X");
            v = Ok(player.CancelApproved(Fence(approvedX)), "cancel X");
            v = Ok(player.AddPending(Fence(v), "P30-007", "PS-20 plan Y"), "re-add");
            var approvedY = Ok(player.Approve(Fence(v)), "approve Y");
            var planY = approvedY.Approved!;
            RefusedWithoutMutation(c, "WorkExecutionKeyConflict",
                () => player.Execute(Fence(approvedY), planY.PlanId, approvedX.Approved!.ExecutionKey), "the cancelled plan's key");
            var executedY = player.Execute(Fence(approvedY), planY.PlanId, planY.ExecutionKey);
            T.True(executedY.RefusalCode is null && !executedY.Execution!.Replayed, "Y executes under its own minted key");
        });

        PlayerMapSceneTests.Register(manifestPath);
    }

    // ------------------------------------------------------------------ shared helpers (also used by PlayerMapSceneTests)

    internal static PlayerFence Fence(PlayerSessionView view) => new(view.SessionId, view.Source, view.PlanStateRevision);

    internal static byte[] Bytes(object value) => JsonSerializer.SerializeToUtf8Bytes(value);

    /// <summary>Asserts that a command committed and returns the view it produced.</summary>
    internal static PlayerSessionView Ok(PlayerCommandResult result, string what)
    {
        T.True(result.RefusalCode is null, $"{what}: unexpected refusal {result.RefusalCode}: {result.RefusalMessage}");
        return result.View;
    }

    // Test-only inspection of the committed root, the same reflection the existing work tests use; no production seam.
    internal static WorkRoot ReadRoot(AnnualContinuation continuation) => (WorkRoot)typeof(AnnualContinuation)
        .GetField("root", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(continuation)!;

    /// <summary>
    /// A refused command must leave the exact same root (nothing published) and return, as its view, the unchanged
    /// current view; the full view is compared byte for byte.
    /// </summary>
    internal static void RefusedWithoutMutation(AnnualContinuation continuation, string code,
        Func<PlayerCommandResult> command, string what)
    {
        var root = ReadRoot(continuation);
        var before = Bytes(continuation.ReadPlayerView());
        var result = command();
        T.Eq(code, result.RefusalCode, what + ": refusal code");
        T.True(!string.IsNullOrWhiteSpace(result.RefusalMessage), what + ": refusal message");
        T.True(result.Execution is null && result.Annual is null, what + ": no execution or annual result");
        T.True(ReferenceEquals(root, ReadRoot(continuation)), what + ": nothing published");
        T.True(before.SequenceEqual(Bytes(result.View)), what + ": the returned view equals the pre-call view");
        T.True(before.SequenceEqual(Bytes(continuation.ReadPlayerView())), what + ": the current view is unchanged");
    }

    private static (AnnualContinuation Continuation, PlayerWorkPlanAdapter Player, PlayerSessionView Approved,
        PlayerCommandResult Executed) ExecutedSession(AnnualContinuation continuation, string treeId)
    {
        var player = new PlayerWorkPlanAdapter(continuation);
        var view = Ok(player.AddPending(Fence(player.Read()), treeId, "executed session"), "setup add");
        var approved = Ok(player.Approve(Fence(view)), "setup approve");
        var plan = approved.Approved!;
        var executed = player.Execute(Fence(approved), plan.PlanId, plan.ExecutionKey);
        Ok(executed, "setup execute");
        return (continuation, player, approved, executed);
    }

    private static IEnumerable<(string What, Func<PlayerCommandResult> Call)> AllCommands(PlayerWorkPlanAdapter player,
        PlayerFence fence, string planId, string executionKey)
    {
        yield return ("AddPending", () => player.AddPending(fence, "P30-050", "fenced probe"));
        yield return ("RemovePending", () => player.RemovePending(fence, "P30-001"));
        yield return ("Approve", () => player.Approve(fence));
        yield return ("CancelApproved", () => player.CancelApproved(fence));
        yield return ("Execute", () => player.Execute(fence, planId, executionKey));
        yield return ("Advance", () => player.Advance(fence, consentToCancelWork: true));
    }

    private static IEnumerable<(string Field, WorkSourceToken Token)> OneFieldChanges(WorkSourceToken s)
    {
        yield return (nameof(s.Domain), s with { Domain = s.Domain + "x" });
        yield return (nameof(s.FixtureId), s with { FixtureId = s.FixtureId + "x" });
        yield return (nameof(s.FixtureFileSha256), s with { FixtureFileSha256 = new string('0', 64) });
        yield return (nameof(s.FixtureContentHash), s with { FixtureContentHash = new string('0', 64) });
        yield return (nameof(s.ManagementScopeId), s with { ManagementScopeId = s.ManagementScopeId + "x" });
        yield return (nameof(s.EcologicalYear), s with { EcologicalYear = s.EcologicalYear + 1 });
        yield return (nameof(s.AnnualRevision), s with { AnnualRevision = s.AnnualRevision + 1 });
        yield return (nameof(s.WorkRevision), s with { WorkRevision = s.WorkRevision + 1 });
        yield return (nameof(s.RngModelVersion), s with { RngModelVersion = s.RngModelVersion + 1 });
        yield return (nameof(s.GrowthModelVersion), s with { GrowthModelVersion = s.GrowthModelVersion + 1 });
        yield return (nameof(s.P1AlgorithmVersion), s with { P1AlgorithmVersion = s.P1AlgorithmVersion + "x" });
        yield return (nameof(s.ActionEligibleDbhThresholdMm), s with { ActionEligibleDbhThresholdMm = s.ActionEligibleDbhThresholdMm + 1 });
    }

    // Every invariant that one atomic read must satisfy. Any failure means parts of the view came from different commits.
    private static IEnumerable<string> TornViewProblems(PlayerSessionView view, int lastPlanState)
    {
        var frame = view.Frame;
        var source = view.Source;
        if (frame.WorkRevision != source.WorkRevision) yield return "frame and token work revisions differ";
        if (frame.EcologicalYear != source.EcologicalYear) yield return "frame and token years differ";
        if (frame.AnnualRevision != source.AnnualRevision) yield return "frame and token annual revisions differ";
        if (frame.Events.Count != source.WorkRevision) yield return "event count differs from the work revision";
        if (view.AnnualHistory.Count != source.AnnualRevision) yield return "history length differs from the annual revision";
        if (view.AnnualHistory.Count > 0 && (view.AnnualHistory[view.AnnualHistory.Count - 1].YearAfter != source.EcologicalYear
            || view.AnnualHistory[view.AnnualHistory.Count - 1].AnnualRevisionAfter != source.AnnualRevision))
            yield return "last year record differs from the token";
        var felled = frame.Trees.Where(t => t.Death?.Cause == FellCause.Harvest).Select(t => t.TreeId).Order(StringComparer.Ordinal);
        var recorded = frame.Events.SelectMany(e => e.Fells).Select(f => f.TreeId).Order(StringComparer.Ordinal);
        if (!felled.SequenceEqual(recorded)) yield return "felled roster and event ledger differ";
        if (view.Approved is not null && (view.Pending.Count != 0 || view.Approved.Source != source))
            yield return "approved plan is not bound to this view's root";
        if (view.PlanStateRevision < lastPlanState) yield return "plan-state revision went backwards";
        if (frame.Stand != WorkRules.Summarize(frame.Trees, view.Extent.AreaHa)) yield return "frame stand differs from its roster";
    }

    private static void FaultWithoutMutation(AnnualContinuation continuation, Action command, string what)
    {
        var root = ReadRoot(continuation);
        var before = Bytes(continuation.ReadPlayerView());
        bool injected = false;
        try { command(); }
        catch (InvalidOperationException e) when (e.Message == "injected") { injected = true; }
        T.True(injected, what + ": the fault seam was reached");
        T.True(ReferenceEquals(root, ReadRoot(continuation)), what + ": nothing published");
        T.True(before.SequenceEqual(Bytes(continuation.ReadPlayerView())), what + ": full view, plan state and history unchanged");
    }

    private static void Inject() => throw new InvalidOperationException("injected");

    private static void ReadOnly<TItem>(IReadOnlyList<TItem> list, string what)
    {
        T.True(list is not TItem[], what + " is not an exposed array");
        var writable = list as IList<TItem>;
        T.True(writable is not null && writable.IsReadOnly, what + " is a read-only list");
        bool addRefused = false;
        try { writable!.Add(default!); }
        catch (NotSupportedException) { addRefused = true; }
        T.True(addRefused, what + ": Add is refused");
        if (writable!.Count > 0)
        {
            bool setRefused = false;
            try { writable[0] = writable[0]; }
            catch (NotSupportedException) { setRefused = true; }
            T.True(setRefused, what + ": indexer write is refused");
        }
    }

    private static bool IsMutableCollection(Type type)
    {
        if (type.IsArray) return true;
        if (!type.IsGenericType) return false;
        var definition = type.GetGenericTypeDefinition();
        return definition == typeof(List<>) || definition == typeof(Dictionary<,>) || definition == typeof(HashSet<>)
            || definition == typeof(IList<>) || definition == typeof(ICollection<>) || definition == typeof(IDictionary<,>)
            || definition == typeof(ISet<>);
    }
}
