# TASK PACKET — Rack Planning & Field Marking V2 Combined Integration

**Manager:** ChatGPT Game Dev / Overall Manager  
**Feature:** Scenario One Objective 1 Rack Planning & Field Marking V2 (D-064)  
**Task:** `RACK-V2-COMBINED-INTEGRATION`  
**Class:** approved-spec integration / Tier 2 feature qualification  
**Routing:** SOL/CODEX local integration lane with Unity 6000.6.0f1 and machine-wide Unity lock  
**Status:** **BLOCKED-READY — do not start integration until RACK-V2-03C is published to GitHub and the exact SHAs below are fetchable**

## Why this packet exists

The two remaining Rack V2 production lanes are now reported technically qualified:

- `RACK-V2-02B-R` — headland / fresh-world implementation; GitHub-visible qualified production candidate `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd`, handoff on `feature/scenario-one-rack-v2-headland-v1` (branch tip currently documentation-only `fef344d537e510649082b0ae84f8ef678e26f91d`).
- `RACK-V2-03C` — authoritative rack runtime / order reconciliation; worker reports production implementation `2178d392dd0fd285a30383e0bcf4d0df1745a8af` and final tested HEAD `84f801850acc26906a7cd8dd1a452d0a8da3f90a` with production tree identical and harness-only delta.

The 03C branch/handoff is **not GitHub-visible at Manager review time**. Under AgentWorkflow, local-only implementation/qualification is administratively incomplete and may not be integrated from chat claims alone. The worker must first push the exact implementation/tested lineage and `Docs/Handoffs/RACK-V2-03C/HANDOFF.md`.

## Required source identities before start

Preflight must prove all of the following from GitHub and the local integration checkout:

1. `origin/feature/scenario-one-rack-v2-headland-v1` contains `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd` and its published handoff.
2. `origin/feature/scenario-one-rack-v2-runtime-v1` exists and contains exact production implementation `2178d392dd0fd285a30383e0bcf4d0df1745a8af` and exact tested HEAD `84f801850acc26906a7cd8dd1a452d0a8da3f90a`, or the Manager records an explicit corrected SHA pair after inspecting the published handoff.
3. The 03C handoff is GitHub-tracked at `Docs/Handoffs/RACK-V2-03C/HANDOFF.md` and explains the production-vs-harness delta.
4. Both lanes descend from the designated Rack V2 base `c6342f79a3e89c040fadd232017a937132779385` (or any deviation is understood before writes).
5. Refresh live `Docs/Project/coordination/active-tasks.md` and verify no conflicting writer owns the integration surface.

If any source identity cannot be proved: **STOP — SOURCE NOT PUBLISHED / IDENTITY MISMATCH**.

## Lineage warning

Do not assume current `main` is the production base. At packet issue time `c6342f79...` and `main` are diverged: the Rack candidate line contains substantial production work while `main` contains later canonical/spec/management changes. The integrator must inspect the merge base and reconcile both sides deliberately. Do not fast-forward, force-push, or reconstruct the candidate from chat summaries.

Preferred outcome: one clean combined candidate that preserves the exact qualified production bytes from both Rack lanes except for understood conflict-resolution edits, while also carrying current canonical docs/spec authority from `main`.

Any conflict touching behaviour owned by either qualified lane must be reviewed against both handoffs and the approved spec before resolution. Do not silently choose one side.

## Integration scope

Integrate and reconcile:

- 02B-R fresh-model-1 Enlarged80 headland/feeder/pad world generation and presentation;
- 03C authoritative rack runtime, stable IDs/geometry, confirm → Pending-only work lifecycle, pre-approval reconciliation, approved-order warn/cancel/recreate behaviour, completed-linked locking, entrance-mark state, derived counters, confirmed-rack read APIs, and untouched model-0 legacy path;
- existing Rack V2 persistence/save-v21 foundation already present on the candidate lineage;
- current approved Rack V2 spec and canonical project docs from `main`.

Do **not** add Multi-Scale Forestry Management production code, S1-C/S1-D feature code, parity changes, economy/ecology retuning, new rack UX features, or Windows build work.

## Qualification requirements

Use the cheapest gates that establish the combined candidate, but this is the Rack V2 integration candidate so the result must include a focused Tier 2 feature qualification on the **exact combined HEAD**.

At minimum:

1. compile/import clean on Unity 6000.6.0f1;
2. 03C authoritative-runtime gate passes on combined HEAD (reported worker baseline: 14 phases);
3. RackV2 save verification passes (reported baseline: 35 checks);
4. 02B-R headland qualification and access/entrance verification remain green (reported baselines: 124 checks + 8 checks), or a strictly equivalent combined gate proves the same requirements;
5. all seven protected fingerprints remain exact unless a separately accepted decision authorises movement:
   - `5546B9B3B50673B2`
   - `94589C749852F359`
   - `92F9635566BF6AFE`
   - `7AD177B3CC2F73C7`
   - `BFFF66015A6619DA`
   - `9C77EB4BEE427FB9`
   - `702766DECE591E21`;
6. fresh model-1 Enlarged80 follows Rack V2; model-0 Enlarged80 / Legacy40 compatibility remains historical and does not enter V2;
7. confirmed plan creates/refreshes pending rack work only; no automatic approval/execution;
8. edit/reconcile semantics are proven before approval, after approval, and for completed-linked racks;
9. confirmed-rack IDs/geometry and entrance-mark state survive save/load;
10. subsequent Scenario One progression remains reachable; no rack runtime blocks `rack.approve`, execution or `rack.result`;
11. rendered Rack V2 states required by the approved spec are reviewed at 1280×720 and 1920×1080 where the combined candidate changes or could regress presentation;
12. no protected Reference Future archive is reserialised through current save classes.

### Reference-gate shutdown caveat

A known post-shutdown Unity hang is acceptable only when the gate has already printed all required PASS tokens/hashes, cleanup has completed, and the handoff records the non-zero process termination honestly. A hang before required evidence is complete is a failed/inconclusive run.

### Legacy `RackUxRack` gate / F-10

The worker reports the old `RackUxRack` gate encodes the superseded first-person/legacy authoring flow. Do **not** merely delete or weaken the test to obtain green. Inspect its assertions against D-064 and the approved spec:

- preserve any still-valid compatibility assertions in a current V2 gate;
- explicitly retire/supersede assertions that require behaviour the accepted design removed;
- document the mapping from old assertions to retained/replaced/retired coverage under G3 measurement-integrity rules.

## Integration / publication rules

- One integration worktree/owner.
- Preserve unrelated dirty work; do not use the user's dirty primary checkout.
- Acquire the machine-wide Unity lock for Unity execution and release it normally.
- No Windows build. The user must personally playtest the current Scenario One candidate and explicitly authorise a Windows build later.
- Do not mark the feature VERIFIED solely because source-lane gates passed; qualification must run on the exact combined candidate.
- Do not publish to `main` until the integrator has a reviewable combined handoff and the Manager can make the G4 integration decision.

## Handoff

Publish a GitHub-tracked handoff at:

`Docs/Handoffs/RACK-V2-Combined-Integration/HANDOFF.md`

Include:

- exact current-main SHA, merge base, 02B-R source SHA, 03C production SHA, 03C tested SHA and final combined HEAD;
- integration strategy and conflict resolutions;
- production diff relative to each qualified lane;
- exact Unity lane and lock record;
- commands/gates and actual results;
- protected fingerprint table;
- `RackUxRack` / F-10 disposition with retained/replaced/retired coverage;
- save-v21 compatibility result;
- rendered review evidence locations;
- failures/caveats;
- spec checklist/conformance status;
- recommendation: `APPROVE`, `NEEDS CHANGES`, or `REJECT` for integration to canonical `main`.

## Exit

`RACK V2 COMBINED CANDIDATE QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

or a precise blocker.

Do not claim user acceptance, external-demo readiness, Multi-Scale Forestry Management readiness, S1-D completion, or Windows-build readiness from this packet alone.
