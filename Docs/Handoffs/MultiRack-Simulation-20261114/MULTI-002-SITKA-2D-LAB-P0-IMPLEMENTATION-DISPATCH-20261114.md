# MULTI-002 — Implement the approved synthetic Sitka 2D reference laboratory, P0 only

**Issued:** 14 November 2026  
**Status:** READY / NOT RUNNING — product scope approved, **NO actual code worker registered and NO physical host/worktree lock acquired**.  
**Binding execution claim:** [#494](https://github.com/jermur788/CCF/issues/494).  
**Difficulty:** **MEDIUM–HIGH**: reproducible C# Core plus isolated local API, tree/plan state machine, robust geometry/persistence, real 2D browser UI and meaningful headless/browser negative verification. Does **not** require ecological model research or Unity verification.  
**Best-suited model/provider:** **OpenAI Sol/Codex High in local T3 Code**, one implementation writer for Core, Host and UI, independent Claude Opus or separate high-reasoning Codex review after publication. Actual identity must be recorded, not inferred.  
**User action:** paste the single dispatch at the end into a fresh local T3 Code session. If a T3 Code orchestrator already controls the host, do not run a competing worker without reconciling ownership.  
**Scope:** P0 engineering only. No production/Unity/CCF save work, GBIF account/download, real marteloscope, physics/growth, five-rack treatment, P1 code or Windows build.

## 0. Exact approved authority — inspect before any source edit

**Direct owner product approval:** user explicitly said **“approved”** to all three recommended P0 choices under [MULTI-001 #493](https://github.com/jermur788/CCF/issues/493). They are:
- **L1: YES**, standalone engineering/research tool, not playable Unity/2D game mode;
- **L2: YES**, **NO BIOLOGICAL GROWTH IN P0**, deterministic artificial trees/forest operations only, P1 separately gated;
- **L3: YES**, independent **C#/.NET Core with a loopback-only 127.0.0.1 browser UI**, dependent on safe toolchain preflight.

**Formal approval receipt:** [MULTI-001 P0 USER APPROVAL RECORD](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-001-SITKA-2D-LAB-P0-USER-APPROVAL-RECORD-20261114.md).

**Approved four-file specification, exact implementation authority** on GitHub branch \`docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1\` (starting approved HEAD shown in issue, not relayed in user-facing prose):
1. [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md)
2. [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/plan.md)
3. [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/tasks.md)
4. [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/checklists/requirements.md)

**Review:** [#485](https://github.com/jermur788/CCF/issues/485) returned NEEDS CHANGES; repaired [#486](https://github.com/jermur788/CCF/issues/486); independent [#488 Claude Opus 5.5 re-check](https://github.com/jermur788/CCF/blob/fc7384226843aee3abb9314de0a0eb30cd64576e/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md) gave **all 18 original findings PASS**, no blocking regressions. Optional seven wording clarifications were folded into the approved version. Do not reopen a whole-document quality review as a precondition to P0 coding.

**Hard-science boundary:** Every P0 tree is **an engineering fixture, not a measured Irish tree**. GROWFOR/Broad–Lynch is at most a later stand-level reference; it does not validate spatial individual-tree release and must not be implemented as P0 growth. Public Irish marteloscope raw inventory remains unverified/data-access blocked [#479](https://github.com/jermur788/CCF/issues/479).

## 1. Lane, ownership and no-collision preflight — MUST occur before files or code

**Authorised prospective implementation branch created from the approved spec:** \`feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1\`.

**Exact *proposed*, NOT created or reserved, local worktree on HP WSL:** \`/home/jer/Development/CCF-HP-Worktrees/CCF-MULTI-002-SitkaLab-P0-20261114\`.

**Allowed write paths after successful preflight only:**
- \`Tools/MultiRackSitkaLab/**\` — entirely new standalone root, including source, tests, fake fixtures, versioned schemas, lab README and new root-local ignore rules; 
- **one** new final tracked Markdown handoff at \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md\`.

**Forbidden even if easy:** \`Assets/**\`, \`Packages/**\`, \`ProjectSettings/**\`, \`specs/**\` approved files, \`Docs/Project/**\` and \`coordination/active-tasks.md\` unless newly delegated to the authorised live board writer, existing \`Tools/ContentReview/**\`, root shared build files, \`main\`, unrelated branches/worktrees, Unity scenes/prefabs/saves/Reference Future, proprietary/real source data, Windows/Player build or external contact.

**Mandatory actions:**
1. Read [#494](https://github.com/jermur788/CCF/issues/494), this full packet, approved four-file spec and current [live coordination board](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md) **fresh**. Verify existing shared worktree ownership and any active Unity/local worker, not just a stale chat status. Inspect actual host, .NET SDK, browser/Node, worktree root and storage capacity **read-only** first; do not assume this proposed HP path exists or is free. The claim itself gives **execution presence only**, not subsystem/host or board ownership.
2. If any overlap, dirty work, missing board writer authority or machine hazard: mark #494 **[BLOCKED]**, describe exact impediment and ask authorised manager to allocate a lane. **Never stash, clean, reset, discard, kill another worker's process, bypass a lock or alter any protected save.** If the suggested host/path is unavailable, do not invent a host; propose an actual isolated substitute for owner approval before local edits.
3. Once allowed: create/verify isolated worktree and branch at exact approved base, confirm clean branch, register actual **model/provider, machine/host, worktree path, starting full HEAD, exclusive allowed \`Tools/MultiRackSitkaLab/**\` ownership and phase** in #494 as **[RUNNING] before source edits**. Coordinate through the **actual authorised active-board writer**; do not self-grant a live board lock. No Unity lease is required or provided, and never launch Unity.
4. If any implementation uncertainty materially expands the approved P0 scope, STOP for Manager rather than silently building a different P1 or game feature.

## 2. Executable technical scope — P0 only

Implement all accepted G2–G5 tasks in the approved plan under one writer, without deviation:

**Core / fixtures**
- A versioned C# class library holds exactly one forest authority: forest ID, one polygon in **integer millimetres** (simple ring, no holes, reject self-intersections/bow-ties), eligible Sitka adult list and stable unique IDs, living/dead/felled status, tree marks, plan states, event history.
- **Checked-in hand-audited artificial fixtures only**; no P0 random seed/PRNG, “real measured plot,” Irish empirical stand density/growth constants or user saved-game import. Fixture inputs have safe ID character set, positive DBH and height, threshold in mm, inclusive on-boundary points, no out-of-polygon stems, unique IDs.
- Derive exact area in m²/ha, adult N and N/ha, BA using \`π(dbhCm/200)^2\`, BA/ha and Dq; dead/felled/below-threshold trees remain visible as appropriate but do not count in eligible adult metrics or qualify for work. **No top-height in P0.**

**Actions / work**
- Keep per-tree **UNMARKED / PLAYER_MARKED** separate from eight plan stages **DRAFT / PROPOSED / APPROVED / PENDING_WORK / EXECUTED / REJECTED / CANCELLED / STALE** with the approved transition table; a tree belongs to no more than one nonterminal plan.
- Approve records forest revision and opaque session epoch; other execution/experiment-step advance invalidates approval, and loading/resetting a fixture makes former approved/pending plans stale even if epoch appears numerically identical. A stale plan cannot be executed or modified in place.
- **All-or-nothing test execution**: any single invalid/duplicate/dead/below-threshold/stale tree ID causes **zero actual felling of the whole plan**, and writes a clearly audit-only rejection event. On success, execute **all and only** the listed unique, currently eligible living adults once. Marks/proposals/approval/pending **never change actual living trees or BA**.
- Every state-changing action contributes to an ordered, hash-checked replayable event stream. Immutable artificial fixture records, canonical compact UTF-8 JSON, fixed field order, integer values, ordinal sorted IDs, SHA-256 per-snapshot event chain. Match the full approved spec and its historical event-vs-runtime-epoch semantics.

**Local-only host, saves and UI**
- .NET local host binds to \`127.0.0.1\` only, restrictive Host/Origin/CORS, no accounts, telemetry or external network calls. Only lab-listed \`.sitkalab.json\` under a safe gitignored \`Tools/MultiRackSitkaLab/.lab-data/\`, no arbitrary OS file picker/root path, no CCF \`ForestSaveData\`, path/symlink escape or protected save access. Reject path escape before file open, wrong-format JSON after header read but before forest/event parsing.
- Top-down plot with metre-scale pan/zoom, unique tree selection/inspector, live/selected/pending/felled not colour-alone, legend, Core-supplied actual and preview metrics, separate explicit approve/queue/execute buttons and cancelled/rejected/stale errors. Browser does **NO separate BA/N/density arithmetic**.
- Always show **“Synthetic Sitka test stand (flat)”**, **“SYNTHETIC TEST FOREST”** and **“NO BIOLOGICAL GROWTH”**. P0 age at inventory, DBH and height do not increase when **“Advance experiment step (calendar only)”** is pressed. This creates a plan-staling event and changes no biology.

## 3. Proportionate verification and stopping rules

**Tier 1 FAST / headless** actual executed tests (record exact commands and results):
- Fixed synthetic manual fixture: a 0.25 ha square with 20/30/40cm live stems gives BA \`π(0.01+0.0225+0.04)≈0.227765\` m², BA/ha ≈0.911062 m²/ha, N/ha=12 and Dq≈31.0913cm; felling 40cm is BA removal ≈0.125664m². This is **a mathematical test case only**, not an Irish forest.
- Reversed winding, skewed/simple boundary, on-edge stem accepted, off-polygon rejected, invalid bow-tie ring rejected; non-finite, overflow, duplicate IDs and below-threshold work rejected. Zero eligible adults produces defined N and non-applicable Dq.
- Tree/plan transitions and no-mutation previews; reject whole plan on one dead/duplicate/stale/under-threshold ID, no partial felling, monotonic eventSeq, every SHA-256 before/after chain and deterministic canonical clean fixture load/save/replay.
- Calendar-only step stales approved work but **never** changes age, DBH/height or natural growth. Unknown/CCF-shaped JSON inside lab root fails after header read before body parse/write; outside/traversal/symlink attempted path fails **before file open**.
- Security: loopback bind, hostile Host/Origin/CORS/request methods rejected, response cannot read/write unrelated files; no Unity saves read.

**Tier 2 FEATURE / actual local browser**: prove actual displayed forest/tree IDs equal Core, select and inspect real fixture IDs, mark/propose/approve/queue/execute once, cancel/reject and stale negative, save/reload, core/UI metric agreement. Capture actual browser screenshots/receipts at desktop and narrower viewport. Use **2,000-tree artificial fixture** and record device/SDK/browser with selection+inspector+preview update **≤2 seconds** on the actual test host. No synthetic screenshot or fake green test.

**Do not run Tier 3 Unity full integration, Player Windows build or future-site L4 accuracy checks.** Stop on missing browser tooling/host access, data-path escape, save ambiguity, CI/compile defects, test oracle mismatch, branch conflict, or any request to claim biological/ecological validation. Do not weaken tests to get PASS.

## 4. Publication, review and owner handoff

**One final implementation Markdown** under \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md\` on the implementation branch. It must state:
- actual provider/model, branch, full starting/candidate HEAD, worktree/host and owner/lock receipts; exactly what source/fixtures/tests were created or left incomplete;
- approved requirements LAB-01 through LAB-05 and exact PASS/FAIL/NOT TESTED for key AC-01…AC-11 / checklist, with commands and raw logs/hashes/screenshots in a permissible off-repo evidence directory (referenced, not fabricated);
- actual mechanics and UI screenshots, known limitations, performance, external package/licence listing, real user/Unity saved-game isolation, and no claim of Irish ecological accuracy;
- source path-scoped comparison versus approved spec HEAD, no unrelated edits/dirty work, actual check that \`Tools/MultiRackSitkaLab/**\` is sole production subtree, README docs, synthetic fixtures only;
- independent code/UX review requested on **a new claim** after writer stops, reviewer verdict APPROVED / NEEDS CHANGES / REJECTED; implementation existence ≠ integrated into Scenario One, verified ecological model or user acceptance;
- claim [#494](https://github.com/jermur788/CCF/issues/494) updated [HANDED-OFF] and closed **only after** GitHub file readback + correct branch diff + evidence; report BLOCKED honestly if no local work can start.

**Dashboard/Drive sync:** NOT VERIFIED unless separately proven.

## 5. Paste-ready dispatch for local T3 Code

> **MULTI-002 — APPROVED P0 Sitka 2D reference lab implementation.** Take CCF [#494](https://github.com/jermur788/CCF/issues/494), and read the **complete binding GitHub packet** at https://github.com/jermur788/CCF/blob/docs/multi-002-sitka-lab-p0-implementation-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-DISPATCH-20261114.md . The user has explicitly approved P0 standalone research-only, **NO BIOLOGICAL GROWTH**, C#/.NET Core plus 127.0.0.1 browser UI. Authoritative **approved four-file spec** is on \`docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1\`; implementation branch \`feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1\` was created empty relative to that approved base. Proposed **uncreated/unclaimed** HP WSL worktree: \`/home/jer/Development/CCF-HP-Worktrees/CCF-MULTI-002-SitkaLab-P0-20261114\`. **Read live coordination, physical host/worktree status and source HEAD BEFORE doing anything, acquire appropriate authorised ownership and register actual model/provider/branch/host/worktree/start HEAD as RUNNING on #494 before source edits; if any conflict or unavailable host/board approval, mark BLOCKED and stop.** One code writer, only **new \`Tools/MultiRackSitkaLab/**\`**, plus final \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md\`. Implement tree/plan state and deterministic verified synthetic forest/2D map, atomic work, canonical lab-only saves and browser UI; execute real headless and browser verification, do one-file/source-subtree GitHub scope proof and hand off for independent review. **Do NOT build biological growth/P1, Irish real-marteloscope imports, rack extrapolation, Unity, Scenario One, protected saves, coordination edits, outside-subtree code or Windows game builds.**
