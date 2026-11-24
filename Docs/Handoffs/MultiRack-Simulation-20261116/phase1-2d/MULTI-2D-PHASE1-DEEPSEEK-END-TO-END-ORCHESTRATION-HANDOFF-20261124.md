# Multi 2D Phase 1 — DeepSeek End-to-End Orchestration Handoff

**Issued by:** CCF Overall Manager, 2026-11-24, on direct owner instruction.  
**Recipient:** DeepSeek V4.1 Flash **orchestrator/coordinator** (use another DeepSeek orchestration variant only if the intended one is unavailable; report actual provider).  
**Difficulty:** HIGH — multi-phase .NET persistence + authoritative annual simulation + browser UX, with active map coordination and model/save risks.  
**Classification:** SPEC REQUIRED. **Spec status: APPROVED** (owner 2026-11-24 13:53:37 UTC).  
**Current task status:** **DISPATCH PACKET PUBLISHED; IMPLEMENTATION NOT CLAIMED, NOT RUNNING**. No physical worktree or Ops lease can be verified by this browser-only Manager.  
**Goal:** Deliver the full **player-visible 2D Phase 1** in three gated stages: (A0) complete save-code foundation in independent files **now**, (A1) integrate save/restore + annual skip + provenance/history with authoritative Core after shared-file ownership clears, (B) integrate minimal save/load/year-skip/history controls on the current `/multi` map, qualify, independently review and hand off. **End to end** means a functioning player workflow, not just a serializer or static proof of concept. No deployment/merge/acceptance claim without actual evidence and G4/owner gates.

## 1. Binding links and source provenance

- **APPROVED four-file quartet** (this branch):
  - [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/spec.md)
  - [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/plan.md)
  - [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/tasks.md)
  - [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/checklists/requirements.md)
- [Initial parallel-admission assessment](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/MULTI-2D-PHASE1-PARALLEL-ADMISSION-DRAFT-20261124.md): historical **DRAFT**, superseded as to owner approval by this handoff; factual file-overlap observations remain useful.
- [Phase2 code baseline](https://github.com/jermur788/CCF/tree/cf6fe7a52aa096e824827c568f43fe05724dcce9/Tools/MultiRackSitkaP1Lab), SHA `cf6fe7a52aa096e824827c568f43fe05724dcce9`, independent Phase2 review/delta qualification accepted in its scope. NOT main-merged.
- [Map Pass1 code candidate](https://github.com/jermur788/CCF/tree/1f13182659769f19e7c91159dd7ad475667e0d62/Tools/MultiRackSitkaP1Lab), SHA `1f13182659769f19e7c91159dd7ad475667e0d62`, exactly five commits ahead of the above Phase2 baseline at Manager inspection. Map Pass1 **independent Opus review APPROVED, G2/G3 PASS**, but **not integrated, not merged**, as recorded in the [review](https://github.com/jermur788/CCF/blob/review/multi-map-integration-pass1-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass1/MULTI-MAP-INTEGRATION-PASS1-OPUS-INDEPENDENT-REVIEW-20261124.md). Later map Pass2/further code may exist; do not treat Pass1 as the automatically accepted join target.
- **Live ownership:** [Docs/Project/coordination/active-tasks.md on main](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md). At this packet's inspection, the file's blob was `3350187033bd14dd587682fe9f44f76479a138c5`. Refresh LIVE before substantial work and before claiming/releasing a lock; check local Ops/physical leases too. GitHub cannot establish current local cleanliness, active processes or filesystem reservation.
- Governing [AgentWorkflow](https://github.com/jermur788/CCF/blob/main/Docs/Project/AgentWorkflow.md), [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) (especially D-030, D-073, D-074, D-075, D-077, D-091), [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), [AGENTS.md](https://github.com/jermur788/CCF/blob/main/AGENTS.md), and latest full committed project context stamp verified at execution start. The user's latest approval is higher authority than older planning status.

**Owner-approved six A choices:** (1) restricted named local save slots; (2) strict schema/model/genesis matching, no auto-migration; (3) D-077-style advisory Fell-anchored 4-year review gap, maximum 5 real annual ticks per press, with no anchor → no skip eligibility and yearly stepping always possible; (4) complete real event/account records + compact annual summaries + paged/lazy histories, not a full root per historical year; (5) A0→A1→B gated delivery, no parallel writes into shared map files; (6) on load, formerly approved orders become visible unapproved drafts needing explicit reapproval, or fail load without changing the running world if conversion is unsafe. All six were accepted by direct owner reply after the Manager presented the defaults.

## 2. Orchestrator mandate (not a source-writer claim)

Own Phase 1 delivery coordination **end to end**; make the smallest functioning player feature the measure of success. Use capable authorised local C#/.NET workers for implementation and tests; use a separate independent reviewer who is NOT the implementing thread's helper. DeepSeek may coordinate and manage GitHub, but must not imply it ran .NET, a browser or Unity merely by writing a summary. Assign roles by observed capabilities; do not require a particular model if unavailable. Run cheap targeted checks before broad ones.

**Before ANY source work:**
1. Inspect exact locked project context, current milestone, decision log, approved quartet, latest existing work, active-task record, map Pass1 review and subsequent map scope; confirm which agent owns `MultiArea*.cs`, `PlayerMultiApi.cs`, `Program.cs`, Core test registration, `PlayerApi.cs` and local worktrees. Do not assume the active-tasks top paragraph is a comprehensive lock.
2. Check actual physical Ops lease/reservation, branch and HEAD, exact one writer, compile-linked read/changes, protected paths, build process ownership, server/port reservation and dirty files. Never overwrite map work or kill another worker's process. If an Ops grant is missing, stop ONLY source launch; proceed with read-only design/review and ask the proper lock owner for admission.
3. Set up a **new exclusive A0 worktree** from the Phase2 baseline at `cf6fe7a52aa096e824827c568f43fe05724dcce9` (unless a newer explicitly approved, non-conflicting base is supplied by the Manager). **Proposed, not created:** branch `feat/multi-2d-phase1-a0-20261124-v1`; worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-A0-20261124`. Verify absence before creating. The approved spec is read from its separate documentation branch above; do not commit implementation onto that docs branch. Register the exact A0 writer, paths, lane, base and physical lease as RUNNING in the LIVE `active-tasks.md` only once actually admitted. One writable owner/file set.
4. Give each delegated worker a complete packet: plain-English goal, exact base/context SHA, owner/provider, branch/worktree, allowed writes and compile-linked reads, exclusions, acceptance tests, stops, durable handoff location. A DeepSeek coordinator is not a wildcard source permission grant.

## 3. Phase A0 — NOW, only if physical/ownership prerequisites pass

**Goal:** build a technically usable, complete, strict, versioned **2D save foundation** without modifying the running map. Not yet player-visible save/load.

**Write allowlist (A0 only):**
- NEW files under `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**` (e.g. envelope, typed state DTO, codec, strict validator, restricted slot store).
- NEW disjoint dedicated tests/evidence filenames under `Tools/MultiRackSitkaP1Lab/tests/**` *only if a current physical file grant explicitly includes them*. Prefer a separate opt-in test runner rather than editing existing test registration. New GitHub Markdown handoffs under `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/**`.
- Existing source `Tools/MultiRackSitkaP1Lab/src/Core/**` is **read-only** for inspecting contracts. Other read permissions must be explicitly listed in the worker packet; a readable file is not writable.
- **No edits** to existing `MultiAreaContinuation.cs`, `MultiAreaContracts.cs`, `MultiAreaEcology.cs`, `MultiAreaExperiment.cs`, `MultiAreaUiReadout.cs`, `MultiAreaWorkPlanAdapter.cs`, `PlayerMultiApi.cs`, `Program.cs`, `PlayerApi.cs`, `src/Core/StrictJson.cs`, existing `*.csproj`/`.sln`, test registration/fixtures/smokes, map assets, `Assets/**`, 3D saves, frozen reference or `main`.
- Do not write on someone else's branch/worktree. A0's new code may compile-link to the project in its OWN isolated checkout; do not attach it to the map task's active source tree.

**Requirements:** full `MultiAreaRoot` coverage (including `Executed`, events, pending/approved work, Stage4 pruning and deadwood, Stage5 cells/mast/blocks, Phase2 species/bands/planted exact juveniles/shelters/spot patches/accounts, sequence numbers and model options); avoid saving view JSON/cache/session ID. Exact deterministic types and ordering, lossless numeric representation, content/integrity checks, safe fail-closed version/identity handling, limited input sizes, path-constrained named save slots, atomic write with prior-good recovery and no unsafe copy-over-original. Distinguish serialization DTO from engine authority; no new ecological formula, work approval, economics or random seed route.

**A0 checks:** round-trip exact representative typed state including malformed/duplicate/oversize/corruption negatives and recoverable interrupted writes. State that contract-only tests do NOT establish that a real running forest can yet be restored or continued; that needs A1. If test registration or shared file edits are needed, STOP that subtask and report; do not broaden A0 scope by implication.

**A0 exit:** source SHA, complete changed-file allowlist/diff, exact tests run/not run with command and output, limitations, independent review verdict `APPROVED / NEEDS CHANGES / REJECTED`, and **permanent GitHub Markdown handoff URL**. No G4 merge; retain working feature branch for a later reviewed A1 join.

## 4. Phase A1 — gate before shared engine / API work

**Admission:** NOT while active map ownership overlaps. After Pass1 and any Pass2 review/repair, ask the Overall Manager for a specific new **G4 shared-file/integration grant** identifying the exact reviewed candidate SHA and resolved lock scope. Confirm physical ownership and a fresh single-writer branch/worktree. Owner's Phase1 product approval is NOT permission to bypass these shared file gates.

**Goal:** make save/load and skip/history genuinely work in the authoritative `1/1/1/1/1` runtime. Reuse A0 and currently reviewed map source; inspect before changing. Only authorised writer may alter existing Core/adapter/API files.

**Engineering outcomes:**
- Capture and validate the **one authoritative immutable root under the existing single lock**; transactional restore with exact model/genesis/experiment matching, no mutation on error, reset all session-only caches, fresh session ID and stale fence refusal. Revalidate loaded orders. Former approved plan becomes visible unapproved pending draft requiring a new explicit approval; fail the whole load if safe conversion cannot be shown.
- Wire named local slots and explicit confirmation to overwrite; no browser-controlled filesystem path. Clear failures and recovery. A bad save never corrupts the current game.
- Preserve `MultiAreaContinuation.Advance` as the one-year ecological oracle. The skip controller serially executes up to five real annual steps with full actual Stage4/5/Phase2 ecology, annual accounts/history. Preview suggests a year using the approved 4-year successful-Fell anchor; without a real anchor, skip is unavailable, not an invented trigger. No queued/pending/approved work, required player review, or lesson gate may be bypassed; never set `consentToCancelWork=true` automatically. Stop on real events/gates implemented in this 2D program, with actual years completed, no fake 3D event, no phantom yearly acknowledgment. Compare with separate repeated manual one-year reference.
- Add a compact yearly history summary at a real annual commit or derive it from already recorded facts; keep L1 recorded events, L2 current observations, L3 derived changes, L4 teaching distinct. Per-year history must be lazy/paged; never concatenate all years into the large `GET api/multi/view` payload.

**A1 tests:** execute → save → load → reapprove → execute; mixed oak/beech planting→survival or death→promotion; pending/approved plan restoration; stale old tokens; executed-key idempotency; negative save corruption; forced write interruption; 1-5 year skip parity to explicit yearly steps; partial stop and no implicit order cancellation; model 0 and frozen/Phase2 test anchor preservation. Use current reviewed 2D semantics rather than fabricating unimplemented goals or storms. Independent reviewer and target runtime evidence; no broad 3D regression for 2D-only code.

**A1 exit:** exact SHA, list of shared files written with Manager grant, approved independent review, scoped Tier2 qualification, GitHub handoff, next UI packet. **No main merge yet unless a distinct G4 grant explicitly covers it.**

## 5. Phase B — gate before current map UI edits

**Admission:** after latest map integration/ownership and A1 Core review; obtain explicit single-writer UI/API paths and physical port/browser reservation. Do not interleave edits to `PlayerMultiApi.cs`, `Program.cs` or shared smokes with the map writer.

**Goal:** 2D player can see, use and trust all Phase 1 features on the latest map, not an isolated command-line demo:
- `Save` (named slot and confirmation), `Load` (status/compatibility + safe confirmation), `Advance 1 Year`, `Skip to Suggested Review` (target and warning shown before action), `History` (year list and selected-year actual work, stand observations and labelled changes). Make refusal, lost approval after load and actual skip stop reason visible.
- Preserve existing 2D map styling, tree interaction, work controls, species distinctions, source fences and size limits. Pure presentation: never compute or change ecology on the map side. Do not include entire historic roots in page payload.
- Genuine desktop/browser exercise at supported 1280×720 and 1920×1080, direct API negative/security checks, source-token races around reload and in-flight clicks, step/save/reload/skip/history interactions, current map smoke regression and all `P1-01..P1-10` requirements.

**B exit:** independently reviewed playable candidate, exact source/test evidence and GitHub Markdown handoff, ready for **owner personal 2D playtest**. Final product `ACCEPTED` only after owner acceptance. Shared/main integration needs separate **G4**, and a Windows game build remains barred until the Scenario One owner-playtest requirement and separate explicit authority are met.

## 6. Essential coordination, risk and reporting contract

- **One orchestrator, one writable worker per file set, independent reviewer.** Never self-grant a new spec, physical lock, scope or G4. Refresh `active-tasks.md` on every phase handoff. Existing local dirty files stay with their owner.
- Treat any owner-needed **new mechanics/save policy**, authoritative map merge conflict, undocumented large-model limit, unsafe root omission or conflicting lock as a **stop for that affected action**. Report exact issue, scope/effort, consequence of deferral and proposed minimal fix. Continue nonconflicting authorised work. Don't expand into a full forest creator, economics/labour or architecture rewrite.
- Keep artifacts in GitHub Markdown, not local-only scratch. The handoff for every phase must state **IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED separately**. A passing compile alone is not player verification.
- A0/A1/B should be separate task packets, branches/worktrees and review scopes; do not silently transfer a writer from A0 into the map branch. Review must be independent of code writer; reviewer findings need verdict and follow-up packet.
- Do not run expensive Tier3 or Unity/Windows builds; use fast and feature-specific .NET/browser checks. No changes to `Assets/**`, 3D `Compact30`, Legacy40, frozen archives, saved games, reference hashes, biological formulas, current `main`, pipeline security, or unrelated Ops tooling. No PR/merge/force-push unless separately admitted.
- If no map release exists after A0, **stop at an independently reviewed A0 component** and report precisely that A1/B remain BLOCKED BY MAP OWNERSHIP; don't claim Phase1 end-to-end complete or start a rival map branch.

## 7. Report back to Overall Manager / owner

At admission: owner/provider, physical Ops lease status, exact HEAD, branch/worktree, file allowlist, active-task ID, and first worker packet. After each phase: permanent GitHub Markdown URL, code SHA, tests and negative cases with exact run provenance, independent review verdict, lock release status, integration gate and next packet. At end: a clickable playtest launch/repro procedure, list of features genuinely observed, all ten P1 requirements each PASS/FAIL/NOT TESTED, current limitations, owner action required for acceptance. Never infer completed gameplay from the orchestration transcript alone.

**Current verdict:** OWNER APPROVED SCOPE / SPEC; DISPATCH READY; NO SOURCE WRITER CLAIMED OR FEATURE IMPLEMENTED BY THIS HANDOFF.
