using SitkaP1.Core;
using SitkaP1.Core.Phase1;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MULTI 2D Phase 1 Save/Load INTEGRATION tests (spec multi-2d-phase1-save-time-history-v1): the A0 save contract wired to
/// the real MA-1-MIXED engine. Unlike tests/Phase1.Save.Tests (public contract, hand-built DTOs) these tests reach the
/// internal <see cref="MultiAreaContinuation"/>: they capture a genuinely played forest, restore it into a fresh session,
/// and prove the whole authoritative root — rosters, events, executed receipts, both clocks, plan state, Stage 4 deadwood
/// and pruning, Stage 5 cells/accounts and Phase 2 juveniles/shelters/accounts — is preserved and continues identically
/// against an uninterrupted control. They also prove the load policy (fresh session, old fence refused, approval → draft),
/// the A0 defect NB2 refusals (an illegal restored order fails the WHOLE load), and that a refused load changes nothing.
/// </summary>
internal static class Phase1SaveIntegrationTests
{
    internal static void Register()
    {
        // ------------------------------------------------------------------ round trip and continuation parity
        T.Add("P1-SL-01: save → decode → restore preserves the whole root and matches an uninterrupted control over two more years", () =>
        {
            var (source, _) = PlayedMixedForest();
            var (control, _) = PlayedMixedForest();
            // The two independently played forests are deterministic and identical before any save exists.
            AssertBytes(Canonical(control), Canonical(source), "the scripted forests start identical");

            var savedView = source.ReadView();
            byte[] bytes = Phase1SaveCodec.Encode(source.CaptureSave(Fence(savedView), "SL-01 integration", 1));
            var decoded = Phase1SaveCodec.Decode(bytes, source.SaveExpectation());

            var (restored, _) = MixedForest();
            var load = restored.Restore(Fence(restored.ReadView()), decoded);
            T.True(load.RefusalCode is null, "load succeeded" + (load.RefusalCode is null ? "" : ": " + load.RefusalCode));
            T.Eq(0, load.DraftOrderCount, "the saved plan was pending, not approved, so nothing became a draft");
            T.True(load.Checksum is { Length: 64 }, "the verified checksum is reported");
            AssertBytes(Canonical(source), Canonical(restored), "every authoritative field survives the round trip byte for byte");

            // Genuine continuation: the restored forest and the control run the SAME annual ticks with the same consent.
            for (int year = 1; year <= 2; year++)
            {
                var a = restored.Advance(Fence(restored.ReadView()), consentToCancelWork: true);
                T.True(a.RefusalCode is null, "restored Advance " + year + " committed");
                var b = control.Advance(Fence(control.ReadView()), consentToCancelWork: true);
                T.True(b.RefusalCode is null, "control Advance " + year + " committed");
                AssertBytes(Canonical(control), Canonical(restored), "year " + year + " after the load matches the uninterrupted control");
                T.Eq(savedView.Source.EcologicalYear + year, restored.ReadView().Source.EcologicalYear, "the year really advanced");
            }
            // Real work still runs in the restored forest after loading (no read-only corpse).
            var v = restored.ReadView();
            string fell = v.Areas[0].Trees.First(tree => tree.IsLiving).TreeId;
            v = restored.AddPending(Fence(v), "MA-A", fell, "post-load fell").View;
            T.Eq(1, v.Pending.Count, "the restored session still accepts real Work Plan orders");
        });

        // ------------------------------------------------------------------ new session authority
        T.Add("P1-SL-02: a load mints a fresh session and every old token or execution key is refused", () =>
        {
            var (source, _) = PlayedMixedForest(execute: true);
            var oldView = source.ReadView();
            string oldSession = oldView.SessionId;
            var oldPlan = oldView.Events[0];
            byte[] bytes = Phase1SaveCodec.Encode(source.CaptureSave(Fence(oldView), "SL-02", 1));

            var (restored, _) = MixedForest();
            var load = restored.Restore(Fence(restored.ReadView()), Phase1SaveCodec.Decode(bytes, restored.SaveExpectation()));
            T.True(load.RefusalCode is null, "load succeeded");
            var newView = load.View;
            T.True(newView.SessionId != oldSession, "the restored session id is new (" + newView.SessionId + ")");
            T.Eq(oldView.Source.EcologicalYear, newView.Source.EcologicalYear, "the ecological year is preserved");
            T.Eq(oldView.PlanStateRevision, newView.PlanStateRevision, "the plan-state revision is restored precisely");
            T.Eq(oldView.Events.Count, newView.Events.Count, "the event ledger is preserved");
            T.Eq(oldView.Events[0].ExecutionKey, newView.Events[0].ExecutionKey, "the executed event keeps its key as history");

            // Every command carrying the old fence fails before it can touch the restored forest.
            var oldFence = new MultiAreaFence(oldSession, oldView.Source, oldView.PlanStateRevision);
            T.Eq("PlayerSessionMismatch", RefusalCode(() => restored.Advance(oldFence, consentToCancelWork: true)),
                "an old session token cannot advance the restored forest");
            T.Eq("PlayerSessionMismatch", RefusalCode(() =>
                restored.AddPending(oldFence, "MA-A", newView.Areas[0].Trees.First(t => t.IsLiving).TreeId, "stale")),
                "an old session token cannot edit the restored Work Plan");
            // The executed receipt is still there — and is NOT a replay ticket: same plan, same key, different source.
            T.Eq("WorkExecutionKeyConflict", RefusalCode(() => restored.Execute(Fence(newView), oldPlan.PlanId, oldPlan.ExecutionKey)),
                "the kept receipt refuses a replay instead of felling again");
            T.Eq(oldView.Source.WorkRevision, restored.ReadView().Source.WorkRevision, "the refused replay felled nothing");
        });

        // ------------------------------------------------------------------ approval becomes a draft
        T.Add("P1-SL-03: a saved approved plan returns as an unapproved pending draft that must be approved again", () =>
        {
            var (source, _) = PlannedMixedForest(execute: false); // queued and approved, never executed
            var savedView = source.ReadView();
            T.True(savedView.Approved is not null, "the source plan is approved");
            int fellCount = savedView.Approved!.Orders.Count;
            int planState = savedView.PlanStateRevision;
            byte[] bytes = Phase1SaveCodec.Encode(source.CaptureSave(Fence(savedView), "SL-03", 1));

            var (restored, _) = MixedForest();
            var load = restored.Restore(Fence(restored.ReadView()), Phase1SaveCodec.Decode(bytes, restored.SaveExpectation()));
            T.True(load.RefusalCode is null, "load succeeded");
            T.True(load.View.Approved is null, "no approval came back from the file");
            T.Eq(fellCount, load.View.Pending.Count, "the approved Fell orders are visible as pending again");
            T.Eq(planState, load.View.PlanStateRevision, "the plan-state revision is restored exactly, not reset");
            T.True(load.DraftOrderCount >= fellCount, "the load reports how many orders became a draft (" + load.DraftOrderCount + ")");

            // The player must approve again; the old PlanId/execution key were never restored and cannot be guessed into use.
            var approvedAgain = restored.Approve(Fence(load.View));
            T.True(approvedAgain.RefusalCode is null && approvedAgain.View.Approved is not null, "approving the restored draft works");
            T.True(approvedAgain.View.Approved!.PlanId != savedView.Approved!.PlanId, "the new plan id is minted by the new session");
            var executed = restored.Execute(Fence(approvedAgain.View), approvedAgain.View.Approved!.PlanId,
                approvedAgain.View.Approved.ExecutionKey);
            T.True(executed.RefusalCode is null && executed.Execution is not null, "the re-approved draft executes normally");
        });

        // ------------------------------------------------------------------ NB2: illegal restored orders
        T.Add("P1-SL-05a: a restored Fell on a retained Crop Tree is refused by the save validator", () =>
        {
            var (session, _) = PendingMixedForest();
            var state = StateOf(session);
            string crop = state.Areas[0].Trees.First(t => t.Death is null).TreeId;
            var marked = state with
            {
                Areas = Replace(state.Areas, 0, state.Areas[0] with
                {
                    CropTreeIds = Array.AsReadOnly(state.Areas[0].CropTreeIds.Append(crop).Order(StringComparer.Ordinal).ToArray()),
                }),
                Pending = Array.AsReadOnly(new[] { new Phase1WorkOrder("MA-A", crop, WorkAction.Fell, "illegal crop fell") }),
                PlanStateRevision = state.PlanStateRevision + 2, // the mark and the add each bumped it
            };
            string message = RefusalMessage(() => Phase1SaveCodec.Encode(Envelope(session, marked)));
            T.True(message.StartsWith("SaveInconsistent", StringComparison.Ordinal), "refused as inconsistent: " + message);
            T.True(message.Contains("retained Crop Tree", StringComparison.Ordinal), "the crop rule is named: " + message);
        });
        T.Add("P1-SL-05b: a restored tree carrying both Fell and Prune is refused", () =>
        {
            var (session, _) = PendingMixedForest();
            var state = StateOf(session);
            var fell = state.Pending[0];
            var stage4 = state.Stage4!;
            var doubled = state with
            {
                Stage4 = stage4 with
                {
                    PendingPrunes = Array.AsReadOnly(stage4.PendingPrunes
                        .Append(new Phase1PruneOrder(fell.AreaId, fell.TreeId, 3.5f)).ToArray()),
                },
                PlanStateRevision = state.PlanStateRevision + 1,
            };
            string message = RefusalMessage(() => Phase1SaveCodec.Encode(Envelope(session, doubled)));
            T.True(message.Contains("one unexecuted order", StringComparison.Ordinal), "the dual-order rule is named: " + message);
        });
        T.Add("P1-SL-05c: duplicated unexecuted work (two ClearUndergrowth orders for one cell) is refused", () =>
        {
            var (session, _) = ClearedMixedForest();
            var state = StateOf(session);
            var stage5 = state.Stage5!;
            var clear = stage5.PendingClears[0];
            var doubled = state with
            {
                Stage5 = stage5 with { PendingClears = Array.AsReadOnly(new[] { clear, clear }) },
                PlanStateRevision = state.PlanStateRevision + 1,
            };
            string message = RefusalMessage(() => Phase1SaveCodec.Encode(Envelope(session, doubled)));
            T.True(message.Contains("one ground cell at most once", StringComparison.Ordinal), "the duplicate-work rule is named: " + message);
        });

        // ------------------------------------------------------------------ failed loads do not mutate
        T.Add("P1-SL-06: a refused load leaves the live session byte-identical and still commandable", () =>
        {
            var (session, _) = PlayedMixedForest();
            // A save of a DIFFERENT genesis identity: the plain all-Sitka MA-1 experiment, not the mixed demo.
            var other = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Phase2);
            byte[] foreign = Phase1SaveCodec.Encode(other.CaptureSave(Fence(other.ReadView()), "foreign", 1));

            byte[] before = Canonical(session);
            string beforeSession = session.ReadView().SessionId;
            T.Eq("SaveGenesisMismatch", RefusalCode(() =>
                session.Restore(Fence(session.ReadView()), Phase1SaveCodec.Decode(foreign, session.SaveExpectation()))),
                "a save of another forest identity is refused, never substituted");
            AssertBytes(before, Canonical(session), "the refused load changed no authoritative byte");
            T.Eq(beforeSession, session.ReadView().SessionId, "the refused load minted no session");
            var v = session.ReadView();
            string free = v.Areas[0].Trees.First(t => t.IsLiving && v.Pending.All(o => o.TreeId != t.TreeId)).TreeId;
            T.True(session.AddPending(Fence(v), "MA-A", free, "still works").RefusalCode is null,
                "the live session still accepts commands after the refusal");
        });
        T.Add("P1-SL-07: the engine revalidates restored orders it can only judge against the live geometry", () =>
        {
            var (session, _) = PlantedMixedForest(); // holds a valid pending Plant order
            var state = StateOf(session);
            var phase2 = state.Phase2!;
            var plant = phase2.PendingOrders.OfType<Phase1PlantOrder>().First();
            // Still a structurally valid save (the DTO validator cannot see the polygon); the engine must refuse it.
            var moved = state with
            {
                Phase2 = phase2 with
                {
                    PendingOrders = Array.AsReadOnly(phase2.PendingOrders
                        .Select(order => order is Phase1PlantOrder p && p.OrderId == plant.OrderId
                            ? new Phase1PlantOrder(p.OrderId, p.AreaId, p.SpeciesId, -50f, -50f)
                            : order).ToArray()),
                },
            };
            var decoded = Phase1SaveCodec.Decode(Phase1SaveCodec.Encode(Envelope(session, moved)), session.SaveExpectation());
            byte[] before = Canonical(session);
            string beforeSession = session.ReadView().SessionId;
            T.Eq("Phase2PointOutsideArea", RefusalCode(() => session.Restore(Fence(session.ReadView()), decoded)),
                "the engine revalidation refused the out-of-polygon restored order");
            AssertBytes(before, Canonical(session), "the illegal restored order failed the WHOLE load, unchanged");
            T.Eq(beforeSession, session.ReadView().SessionId, "no new session was minted");
        });
        T.Add("P1-SL-07b: an illegal order that was APPROVED in the file fails the whole load too", () =>
        {
            // Independent review finding 1: the saved approval's orders used to be merged into pending AFTER the engine's
            // re-validation ran, so an engine-illegal approved order could reach the restored Work Plan as a stuck draft.
            var (session, _) = ApprovedMixedForest(); // an approved Plant plus an approved Fell, never executed
            var state = StateOf(session);
            var phase2 = state.Phase2!;
            var plant = phase2.ApprovedOrders.OfType<Phase1PlantOrder>().First();
            var moved = state with
            {
                Phase2 = phase2 with
                {
                    ApprovedOrders = Array.AsReadOnly(phase2.ApprovedOrders
                        .Select(order => order is Phase1PlantOrder p && p.OrderId == plant.OrderId
                            ? new Phase1PlantOrder(p.OrderId, p.AreaId, p.SpeciesId, -50f, -50f)
                            : order).ToArray()),
                },
            };
            var decoded = Phase1SaveCodec.Decode(Phase1SaveCodec.Encode(Envelope(session, moved)), session.SaveExpectation());
            byte[] before = Canonical(session);
            string beforeSession = session.ReadView().SessionId;
            T.Eq("Phase2PointOutsideArea", RefusalCode(() => session.Restore(Fence(session.ReadView()), decoded)),
                "the approved Plan order is re-checked by the engine on load");
            AssertBytes(before, Canonical(session), "the illegal approved plan failed the WHOLE load, unchanged");
            T.Eq(beforeSession, session.ReadView().SessionId, "no new session was minted");
        });
        T.Add("P1-SL-07c: an illegally targeted approved Prune fails the whole load too", () =>
        {
            var (session, _) = ApprovedMixedForest();
            var state = StateOf(session);
            var stage4 = state.Stage4!;
            T.True(stage4.ApprovedPrunes.Count > 0, "the scripted plan holds an approved Prune");
            var bad = stage4.ApprovedPrunes[0];
            // In range (so the refusal is the schedule rule, not an out-of-range value): an Oak's first lift is exactly 3.5 m.
            var moved = state with
            {
                Stage4 = stage4 with
                {
                    ApprovedPrunes = Array.AsReadOnly(new[] { bad with { TargetCrownBaseHeightM = 4.0f } }),
                },
            };
            var decoded = Phase1SaveCodec.Decode(Phase1SaveCodec.Encode(Envelope(session, moved)), session.SaveExpectation());
            byte[] before = Canonical(session);
            T.Eq("PruneBroadleafTargetInvalid", RefusalCode(() => session.Restore(Fence(session.ReadView()), decoded)),
                "an approved Prune whose target is not the engine's scheduled lift is refused");
            AssertBytes(before, Canonical(session), "the session is unchanged");
        });
        T.Add("P1-SL-09: a save whose deterministic options differ from the session is refused", () =>
        {
            var (session, _) = PlayedMixedForest();
            var state = StateOf(session);
            var stage5 = state.Stage5!;
            var changed = state with
            {
                Options = state.Options with
                {
                    Stage5 = state.Options.Stage5! with { BackgroundBrowsePressure = 0.05f },
                },
                Stage5 = stage5,
            };
            // A differing option is caught before any root is built; the header identity is still this forest's.
            byte[] bytes = Phase1SaveCodec.Encode(Envelope(session, changed));
            var decoded = Phase1SaveCodec.Decode(bytes, session.SaveExpectation());
            byte[] before = Canonical(session);
            T.Eq("SaveOptionsMismatch", RefusalCode(() => session.Restore(Fence(session.ReadView()), decoded)),
                "different Stage 5 options are never substituted");
            AssertBytes(before, Canonical(session), "the session is unchanged");

            // The Stage 4 growth-phase branch: this session carries no growth phases, so a save that names them is a
            // different forest even though its model vector and identity match.
            var stage4Changed = state with
            {
                Options = state.Options with
                {
                    Stage4 = state.Options.Stage4! with
                    {
                        GrowthAgePhases = Array.AsReadOnly(new[] { new Phase1GrowthAgePhase(0f, 1f, 1f) }),
                    },
                },
            };
            var decoded4 = Phase1SaveCodec.Decode(Phase1SaveCodec.Encode(Envelope(session, stage4Changed)), session.SaveExpectation());
            T.Eq("SaveOptionsMismatch", RefusalCode(() => session.Restore(Fence(session.ReadView()), decoded4)),
                "different Stage 4 growth phases are never substituted");
            AssertBytes(before, Canonical(session), "the session is still unchanged");
        });
        T.Add("P1-SL-08: a stale fence cannot capture or load a save", () =>
        {
            var (session, _) = PlayedMixedForest();
            var view = session.ReadView();
            var stale = new MultiAreaFence(view.SessionId, view.Source with { WorkRevision = view.Source.WorkRevision + 1 },
                view.PlanStateRevision);
            T.Eq("PlayerSourceStale", RefusalCode(() => session.CaptureSave(stale, "never", 1)),
                "a stale fence captures nothing");
            byte[] bytes = Phase1SaveCodec.Encode(session.CaptureSave(Fence(view), "SL-08", 1));
            var decoded = Phase1SaveCodec.Decode(bytes, session.SaveExpectation());
            byte[] before = Canonical(session);
            T.Eq("PlayerSourceStale", RefusalCode(() => session.Restore(stale, decoded)),
                "a stale fence cannot load");
            AssertBytes(before, Canonical(session), "a stale load request changed nothing");
        });
    }

    // ------------------------------------------------------------------------------------------------ forest scripts

    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) MixedForest()
    {
        var (experiment, options, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
        var session = new MultiAreaContinuation(experiment, MultiAreaModelSelection.Phase2, null, null, options);
        return (session, new MultiAreaWorkPlanAdapter(session, experiment, MultiAreaDemoExperiments.Ma1MixedKey, badge));
    }

    // A genuinely played mixed forest: executed real work in Year 0, one real ecological year, then a pending plan of a
    // Fell and a scheduled Prune that is NOT approved (so a save and its restored root have the same shape).
    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) PlayedMixedForest(bool execute = true)
    {
        var (session, adapter) = MixedForest();
        var v = session.ReadView();
        string fellA = v.Areas[0].Trees.First(tree => tree.IsLiving).TreeId;
        var point = Phase2Support.FindPoint(session, "MA-B", 1.0f);
        v = session.AddPending(Fence(v), "MA-A", fellA, "Sitka release").View;
        v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, point.X, point.Z).View;
        v = session.AddShelter(Fence(v), "MA-B", point.X, point.Z).View;
        if (execute)
        {
            v = session.Approve(Fence(v)).View;
            var executed = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(executed.RefusalCode is null, "scripted Execute committed" + (executed.RefusalCode is null ? "" : ": " + executed.RefusalCode));
            v = executed.View;
        }
        v = session.Advance(Fence(v), consentToCancelWork: !execute).View;

        string secondFell = v.Areas[0].Trees.First(tree => tree.IsLiving).TreeId;
        v = session.AddPending(Fence(v), "MA-A", secondFell, "second release").View;
        string oak = OakOf(session);
        if (oak.Length > 0) v = session.AddScheduledPrune(Fence(v), "MA-B", oak).View;
        return (session, adapter);
    }

    // Queued and approved, never executed.
    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) PlannedMixedForest(bool execute)
    {
        var (session, adapter) = MixedForest();
        var v = session.ReadView();
        v = session.AddPending(Fence(v), "MA-A", v.Areas[0].Trees.First(t => t.IsLiving).TreeId, "approved release").View;
        v = session.AddPending(Fence(v), "MA-A", v.Areas[0].Trees.Where(t => t.IsLiving).Skip(1).First().TreeId, "approved release 2").View;
        v = session.Approve(Fence(v)).View;
        if (execute)
        {
            var executed = session.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            T.True(executed.RefusalCode is null, "scripted Execute committed");
        }
        return (session, adapter);
    }

    // Queued and approved (a Fell, a Plant and a scheduled Prune), never executed.
    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) ApprovedMixedForest()
    {
        var (session, adapter) = MixedForest();
        var v = session.ReadView();
        var point = Phase2Support.FindPoint(session, "MA-B", 1.0f);
        v = session.AddPlant(Fence(v), "MA-B", Phase2Species.OakId, point.X, point.Z).View;
        string oak = OakOf(session);
        v = session.AddScheduledPrune(Fence(v), "MA-B", oak).View;
        v = session.AddPending(Fence(v), "MA-A", v.Areas[0].Trees.First(t => t.IsLiving).TreeId, "approved release").View;
        v = session.Approve(Fence(v)).View;
        T.True(v.Approved is not null, "the scripted plan is approved");
        return (session, adapter);
    }

    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) PendingMixedForest()
    {
        var (session, adapter) = MixedForest();
        var v = session.ReadView();
        v = session.AddPending(Fence(v), "MA-A", v.Areas[0].Trees.First(t => t.IsLiving).TreeId, "pending release").View;
        return (session, adapter);
    }

    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) ClearedMixedForest()
    {
        var (session, adapter) = MixedForest();
        var snap = session.ReadCompositeUiSnapshot();
        var cell = snap.Readout.Areas[1].Ground[0];
        var v = session.AddClearUndergrowth(Fence(snap.View), "MA-B", cell.Column, cell.Row).View;
        T.Eq(1, v.PlanStateRevision, "the clear order is queued");
        return (session, adapter);
    }

    private static (MultiAreaContinuation Session, MultiAreaWorkPlanAdapter Adapter) PlantedMixedForest()
    {
        var (session, adapter) = MixedForest();
        var v = session.ReadView();
        var point = Phase2Support.FindPoint(session, "MA-B", 1.0f);
        v = session.AddPlant(Fence(v), "MA-B", Phase2Species.BeechId, point.X, point.Z).View;
        return (session, adapter);
    }

    private static string OakOf(MultiAreaContinuation session)
    {
        var snap = session.ReadCompositeUiSnapshot();
        foreach (var area in snap.View.Areas)
            foreach (var tree in area.Trees)
                if (tree.IsLiving && snap.Ecology is not null &&
                    snap.Ecology.SpeciesByTreeId.TryGetValue(tree.TreeId, out var species) && species == Phase2Species.OakId)
                    return tree.TreeId;
        return "";
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static MultiAreaFence Fence(MultiAreaSessionView view) => new(view.SessionId, view.Source, view.PlanStateRevision);

    private static Phase1SaveState StateOf(MultiAreaContinuation session) =>
        session.CaptureSave(Fence(session.ReadView()), "state", 1).State;

    private static Phase1SaveEnvelope Envelope(MultiAreaContinuation session, Phase1SaveState edited)
    {
        var header = session.CaptureSave(Fence(session.ReadView()), "edited", 1).Header;
        return new Phase1SaveEnvelope(header, edited);
    }

    // The canonical bytes of the session's current state under a fixed header: byte equality then proves every captured
    // field of two forests is identical (the codec is deterministic and complete). The one exception is the engine-minted
    // PlanId / execution key, which embeds the random per-session id on purpose (it is authority, not forest state), so it
    // is normalised to one token before encoding.
    private static byte[] Canonical(MultiAreaContinuation session)
    {
        var view = session.ReadView();
        var envelope = session.CaptureSave(Fence(view), "compare", 1);
        var state = NormaliseAuthorityIds(envelope.State);
        return Phase1SaveCodec.Encode(new Phase1SaveEnvelope(envelope.Header with { SavedUtc = "2026-01-01T00:00:00Z" }, state));
    }

    // Any engine-minted plan id / execution key is "<32 lowercase hex session id>:ma-plan:<revision>[:execute]". The random
    // session id is authority, not forest state, so it is replaced by one fixed token wherever it appears — including in
    // ids a restored forest carried in from the file it was loaded from.
    private static readonly System.Text.RegularExpressions.Regex PlanIdSessionPrefix =
        new(@"\A[0-9a-f]{32}:ma-plan:", System.Text.RegularExpressions.RegexOptions.CultureInvariant);

    private static string N(string value) => PlanIdSessionPrefix.Replace(value, "SESSION");

    private static Phase1SaveState NormaliseAuthorityIds(Phase1SaveState s) => s with
    {
        Events = Array.AsReadOnly(s.Events.Select(e => e with
        {
            ExecutionKey = N(e.ExecutionKey),
            PlanId = N(e.PlanId),
            Areas = Array.AsReadOnly(e.Areas.Select(a => a with
            {
                Fells = Array.AsReadOnly(a.Fells.Select(f => f with
                {
                    ExecutionKey = N(f.ExecutionKey),
                    PlanId = N(f.PlanId),
                }).ToArray()),
            }).ToArray()),
        }).ToArray()),
        Executed = Array.AsReadOnly(s.Executed.Select(x => x with { ExecutionKey = N(x.ExecutionKey) }).ToArray()),
        Stage4 = s.Stage4 is null ? null : s.Stage4 with
        {
            PruneLog = Array.AsReadOnly(s.Stage4.PruneLog.Select(r => r with
            {
                ExecutionKey = N(r.ExecutionKey),
                PlanId = N(r.PlanId),
            }).ToArray()),
        },
        Stage5 = s.Stage5 is null ? null : s.Stage5 with
        {
            ClearLog = Array.AsReadOnly(s.Stage5.ClearLog.Select(r => r with
            {
                ExecutionKey = N(r.ExecutionKey),
                PlanId = N(r.PlanId),
            }).ToArray()),
        },
        Phase2 = s.Phase2 is null ? null : s.Phase2 with
        {
            WorkLog = Array.AsReadOnly(s.Phase2.WorkLog.Select(r => r with
            {
                ExecutionKey = N(r.ExecutionKey),
                PlanId = N(r.PlanId),
            }).ToArray()),
        },
    };

    private static void AssertBytes(byte[] expected, byte[] actual, string what)
    {
        if (!expected.AsSpan().SequenceEqual(actual))
            throw new Exception($"{what}: {expected.Length} vs {actual.Length} bytes differ");
    }

    private static string RefusalCode(Action action)
    {
        try { action(); }
        catch (P1Refusal refusal) { return refusal.Code; }
        throw new Exception("expected a refusal, but the call succeeded");
    }

    private static string RefusalMessage(Action action)
    {
        try { action(); }
        catch (P1Refusal refusal) { return refusal.Code + ": " + refusal.Message; }
        throw new Exception("expected a refusal, but the call succeeded");
    }

    private static IReadOnlyList<T> Replace<T>(IReadOnlyList<T> list, int index, T value)
    {
        var copy = list.ToArray();
        copy[index] = value;
        return Array.AsReadOnly(copy);
    }
}
