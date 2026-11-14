# Implementation plan — Irish Sitka 2D Reference Laboratory V1

**Status:** **APPROVED FOR RESEARCH-ONLY P0 PLANNING AND IMPLEMENTATION**, derived from formally approved [spec.md](spec.md), 14 November 2026. **P1 / UNITY / GAME INTEGRATION NOT APPROVED.** No actual implementation worker/branch/host/Unity lease exists; separate claimed P0 task and live ownership preflight mandatory.

## Work classification

**SPEC REQUIRED**: new standalone user interaction and per-tree state/persistence, forestry calculations, experimental annual model validation and eventual 2D/3D interface boundary. Do not dispatch as a quick packet bypassing approval. **P0 spec now APPROVED after #488 independent DRAFT-quality review and explicit Overall Manager L1–L3 decisions; P1 needs a separate model-evidence gate.** 

## Parallel delivery boundary

This research/lab lane is separate from Scenario One implementation #424 and portfolio authority #425, existing `Tools/ContentReview/` text-editor development and #481 small-stand ecology review. The proposed entirely **new** file namespace is `Tools/MultiRackSitkaLab/**`. Do not edit `Assets/**`, `Packages/**`, existing project code, `specs/**` outside this spec, Game Brief/Decision Log/Current Milestone, `Docs/Project/coordination/active-tasks.md`, or protected saves. Read current GitHub and physical active ownership before claiming any implementation lane. Initial normal local test build is not a Windows game build; stop if a host/Unity machine conflict.

## Proposed package shape, not yet a created implementation

```text
Tools/MultiRackSitkaLab/
  README.md                        # execution, evidence labels, no game integration
  src/Core/                        # standalone C# class library (no Unity deps)
  src/Host/                        # loopback-only local API and lab state store
  src/Ui/                          # local 2D browser canvas/SVG and controls
  tests/Core.Tests/                # deterministic headless fixtures and negatives
  tests/Host.Tests/                # serialisation/API/event tests
  tests/Ui/                        # actual browser path, after implementation
  fixtures/synthetic/              # checked-in hand-audited synthetic sources only
  schemas/                         # field/property ordering and snapshot/event JSON contract
  .lab-data/                       # gitignored lab-only saves; reject outside paths
  evidence/README.md               # no real/personal data committed
```

Actual worker must inspect available .NET, Node, browser, disk and project source before pinning framework/dependencies. Prefer minimal package dependencies, offline/no telemetry and local-only host; record dependency licences and reproducible build. Do not develop a full GIS or cloud service. Avoid copying Unity save or code by assumption.

## Sequence and gates

**G0 – P0 SPEC APPROVAL RECORDED (not implementation evidence).** Independent #488 reviewer approved revised DRAFT quality. The distinct designated Overall Manager subsequently resolved **L1 research-only**, **L2 no biological growth P0 / separate P1**, **L3 .NET C# Core plus 127.0.0.1-only local browser UI**, and **L4/L5 DEFERRED OUTSIDE P0**. Preserve explicit proposal/approval/execution distinction and no biology claims. GROWFOR is only a later aggregate benchmark. Worker must pin the approved spec branch and register a new execution claim/physical lane before code; nothing has been built or tested under this decision.

**G1 – Worker/ownership preflight.** Create dedicated issue claim for P0 implementation. Worker registers actual provider/model, isolated branch/worktree and pinned starting HEAD. Refresh live `active-tasks.md` and physical Ops read-only, get actual owner permission if machine/file overlap or any Unity process would be impacted. No simultaneous writers in same new directory. Never move, reset or stash unrelated dirty work.

**G2 – Core and data contract.** Implement **checked-in golden synthetic fixtures (no P0 fixture generator/PRNG)**, canonical UTF-8 no-BOM/LF/ordered compact JSON, SHA-256, integer-mm `ForestState`/`TreeState` and positive measurement guards; validate a single simple polygon (including reversed winding, boundary-inclusive points and bow-tie rejection), derive area, adult threshold, BA/N/Dq, and stable immutable IDs. Headless hand-calculated test vectors and invalid-input negatives first. Do not implement actual biological growth at this gate.

**G3 – Action authority and histories.** Implement **separate tree-mark and plan-state transition tables** as spec A5; explicit Approve → Queue Test Work → Execute Test Work; **atomic all-or-nothing** execution, plan ID membership, baseForestRevision/approvalSessionEpoch staleness, audit-only rejection events and monotonic eventSeq with post-state hashes. Build **lab-root-only `.sitkalab.json`** load/save; reject outside path/traversal/symlink **before file open**, and reject CCF-shaped/wrong-format content inside the fixed root at the header check **before forest/event parsing**, with no write. Headless negative tests must cover proposal-only no change, dead/duplicate/below-threshold IDs, calendar step, fixture reload, malformed hashes, runtime rev/epoch mismatch.

**G4 – 2D viewer.** Bind map to Core only, with pan/zoom, actual Core metric API (browser computes no forestry arithmetic), selection, tree details, status legend, plan preview vs actual metrics, **distinct approval, queue and execution**. Load only listed synthetic lab-root fixtures. Bind host to **127.0.0.1**, restrictive Host/Origin/CORS, no telemetry or external networking. Actual browser test with GUI and screenshots; no screen depiction accepted as tree-state authority.

**G5 – Proportionate qualification.** Tier 1: deterministic headless unit/property tests; identity/area/BA/Dq, boundary and **self-crossing polygon**, below-threshold, event/atomic/stale status, rejected outside-root/CCF JSON, canonical snapshot and event-by-event SHA-256 replay. Tier 2: local browser pathway at ordinary desktop and constrained viewport, at least one real click/approve/**queue**/execute/replay and negative path; Core-to-UI metric parity; **2,000-tree** perf fixture ≤2 seconds for selection/preview on the recorded host and browser. Do NOT run Unity/Windows Player. Independently review outputs and scope before publication; preserve failed attempts/defects. One bounded pass unless problems require targeted repair, not repeated full integration suites.

**G6 – P0 handoff and review.** Report exact branch, diff, test receipts, actual inspected runtime, small synthetic inventory manifest and known missing processes. Reviewer verdict APPROVED / NEEDS CHANGES / REJECTED for **engineering P0** only. No automatic production integration, source-cherry-pick into Unity or release.

**G7 – P1 separately gated.** Independent source audit and explicit choice of existing CCF Sitka lineage; source/model unit and deterministic parity tests plus Irish GROWFOR domain reference. Write an approved P1 delta/qualification packet if necessary; implement annual model only after owner/manager approval. Separate empirical accuracy programme (marteloscope/longitudinal) remains conditional.

## Effort and provider

**P0 difficulty MEDIUM–HIGH:** C# deterministic event model, polygon calculations, UI, offline host and real browser testing; simpler than forestry growth calibration. Best-suited **Sol/Codex High in local T3 Code** with .NET/Node/browser capability, single dedicated worktree and new directory ownership; **one sole implementation writer for P0 Core, Host and UI (no parallel UI subowner)**; independent **Claude Opus High or separate Sol High** source/UX review. No permanent discipline ownership claimed; actual worker registers. Do not send to multiple code writers at once.

**P1 difficulty HIGH:** biological source inspection, scientific versioning, heterogeneous future effects; split research/source and software qualification with explicit model outcome/holdout gates.

## Stop conditions

Stop and report BLOCKED if the published P0-approved spec cannot be pinned, the new implementation claim/live ownership is missing, branch ownership conflicts, host capacity/Unity process conflicts, missing suitable isolated worktree, source state divergence from approved spec, model source not verified for P1, anonymous "real" GBIF plot substituted, CCF production save encountered, unreviewed numerical forestry constants proposed, or non-Sitka/slope/rack/Unity game code appears. Do not weaken tests to pass.

## Publication plan

Each substantive implementation and independent review requires its **own GitHub Markdown handoff** with actual evidence and unchanged linked source lineage, claim status and Manager verdict. This is a formally APPROVED P0 implementation **plan**, not implementation evidence. No implementation or test passed merely because these steps are written. P1 remains gated. Dashboard/Drive mirror sync must be reported NOT VERIFIED unless evidenced.
