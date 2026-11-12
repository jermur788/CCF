# Independent Scenario Save Isolation V1 — Future Task Packets

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
These packets are **BLOCKED** until the four-part spec is explicitly APPROVED and a fresh, separate production claim / live ownership record exists. Task #436 authorises Markdown drafting only. Do not mark any task started, implemented, verified or accepted based on this document.

## Task dependency graph

T-00 audit + approval → T-01 slot routing → T-02 fail-closed preflight → T-03 scene/session lifecycle → T-04 backup/legacy and reference protection → T-05 exact-head feature qualification + independent review → Manager integration disposition.

An approved implementer may merge or subdivide adjacent bounded packets only after confirming non-overlap and the actual current source; no automatic team dispatch or simultaneous writers on ForestSaveController. O2 authored starting forest is a **separate approved dependency**, not included in these five docs.

### T-00 — Re-pin source, resolve open questions and obtain spec approval

- **State:** BLOCKED. **Difficulty:** High; cross-world ownership, schema and old files. **Route:** FREE HERE Manager/source analysis, with independent review where material.
- **Permitted:** read-only current code and real historical save/archived Reference Future fixtures; propose a signed identity/scene/backup policy and exact file/path matrix. No source changes.
- **Inspect:** ForestSaveController, ForestSaveData, ForestSaveValidation (+ partials), ScenarioOneManager/Definition/SaveMigration, StandaloneSessionMenu, ForestStartingStand, ReferenceArchive, current tests, current live coordination and #424 ownership.
- **Output:** revised approved spec/plan/checklist, old-version fixture manifest and source-caller map, explicit Manager review. No code launch before that approval.
- **Stop:** missing old fixtures, unclear ID/provenance, O2 definition absent, active overlap or product scope disagreement.

### T-01 — Fixed two-scenario slot and selection routing

- **State:** BLOCKED. **Difficulty:** Medium; shared disk risk. **Route:** SOL/CODEX local for implementation, only after approval and an exact claim.
- **Tentative code ownership (must re-check):** ForestSaveController.cs, a narrowly reviewed fixed slot-router helper if needed, session-selection seam in UI/StandaloneSessionMenu*. No generic eight-world framework and no per-species content.
- **Required behaviour:** existing O1 forest-save.json, .tmp and .bak continue unchanged; new O2 fixed slot forest-save-scenario-two.json and its own sidecars; strict allowlist; no implicit migration/fallback, save file deleted or new game overwriting old slot.
- **Verification:** Tier 1 path/alias tests and static F5/F9/F10/Continue route audit; before/after hashes on O1 file and .bak. Later targeted Editor confirmation where relevant.
- **Handoff:** exact commit, test evidence, remaining risks; no own integration authority.
- **Stop:** any path collision, unexpected O1 write or existing file ownership conflict.

### T-02 — Version-aware identity and no-mutation load preflight

- **State:** BLOCKED. **Difficulty:** High; greatest cross-save risk. **Route:** SOL/CODEX local, optionally independent read-only browser review.
- **Tentative files:** ForestSaveData.cs, ForestSaveValidation.cs (+ relevant partial), ForestSaveController.cs; only minimal ScenarioOneManager.cs identity alignment if directly required; narrowly bounded verification tests.
- **Required behaviour:** explicit new-format identity and raw presence checks (JsonUtility default cannot masquerade as explicit input); fixed slot/active scene/definition/nested ID agree; v1–v9 legacy O1-only; v10–v24 valid O1 retained; unknown/newer/malformed/missing/conflicting/duplicate identity rejected before first mutation in BOTH CanLoad/Load/LoadData. Existing duplicate-tree, cell, rack, species, model and management-event validation stays intact; preserve original rejection reasons where possible.
- **Verification:** Tier 1 deterministic table cases and a no-mutation sentinel; genuine O1 old-save fixtures and copied/renamed O1→O2, O2→O1; unsupported future-version test. Source review of mutating order. Do not weaken checks to pass old malformed files.
- **Handoff:** proof that each load entrypoint fails closed and legacy identity classification is deliberate; schema/v24 compatibility report.
- **Stop:** legacy ambiguity unreviewed, archive/hash drift, fallthrough to manager late-warning, or any failed-load state mutation.

### T-03 — Two authored forest sessions, UI and preview bridge

- **State:** BLOCKED on T-02 **and** separately qualified O2 authored-world/scene dependency. **Difficulty:** High; session and scene transition can mutate before validation. **Route:** SOL/CODEX local with serialized Unity.
- **Tentative files:** StandaloneSessionMenu.cs/.Startup.cs, authorized new chapter selection UI, scene/new-game bootstrapping and minimal manager/definition interface; reference preview bridge only if necessary. **No mass scene rewrite without new packet.**
- **Required behaviour:** player explicitly selects new O1/O2; O2 uses actual authored irregular multi-cohort start, not O1 relabelled; first save/reload selects correct slot; wrong-scenario load refusal leaves current scene, selection, forest and UI; O1 Reference Future preview return stays byte/state compatible.
- **Verification:** narrow Tier 2 Editor/New/Continue/F9/F10 and preview contract; retain O1 tests and screenshot evidence only for changed UI. No first external O2 claim.
- **Handoff:** actual candidate/Unity logs, scene-world identity before/after, saved history/cash/order witness.
- **Stop:** unqualified O2 scene/model, preview bypass or transition before preflight.

### T-04 — Backup, fault injection and v1–v24 regression

- **State:** BLOCKED on T-03. **Difficulty:** High; accidental data loss. **Route:** SOL/CODEX local focused verification/repairs, no broad refactor.
- **Tentative files:** limited ForestSaveController.cs (if needed), small explicit recovery UI seam, targeted verification under Tools/Verification/SaveHardeningVerification.cs / SaveCellIndexHardening/ / Parity08Save/ / RackV2Save/ / TeachingPersistence/ / Startup/ / ScenarioReferenceVerification.cs. This list is an inspection map, **not** permission to edit every listed file.
- **Required behaviour:** per-slot .tmp/.bak containment; no automatic unsafe fallback, preserve primary and backup until explicit valid recovery; corrupted primary + valid own backup, corrupted/foreign backup, stale temp, failed write/replace, permission error, future-version file; no false success or cross-slot recovery. No frozen v12 archive edits.
- **Verification:** Tier 1 offline corruption/IO tests; Tier 2 only for actual runtime/Editor recovery and UI. Byte/hash checks O1 v1–v24 fixture set, Legacy40, Enlarged80 and frozen Reference Future preview+resume. Avoid test re-baselining.
- **Handoff:** test/failure matrix, old-file preservation, platform fallback caveats, recovery UX.
- **Stop:** test harness damages unbacked-up real saves, file-promotion ambiguity, or non-atomic fallback with unmitigated loss.

### T-05 — Feature conformance and integration decision

- **State:** BLOCKED on implementation handoffs T-01…T-04. **Difficulty:** High; compound identity/compatibility. **Route:** one serialized Unity Tier 2 qualification worker, independent focused reviewer, then #425/#424 integration decision.
- **Scope:** read/qualify exact candidate branch/HEAD; no opportunistic source edits. One feature-level acceptance matrix: O1 old/current, O2 initial authored world, scenario switching, two-way wrong-load, partial/backup, long-form annual history, frozen preview, real negative UI and no cross-scenario money/trees. Check each item in requirements.md.
- **Verification:** Tier 2 FEATURE, including Editor runtime and rendered/input evidence where relevant. Existing relevant Tier 1 negative fixtures remain supplementary; no repetitive full Tier 3 or weakening of expected results. Any source repair after a failed gate requires new candidate and proportional retest.
- **Handoff:** durable GitHub Markdown with exact source and test evidence, reviewer verdict (APPROVED / NEEDS CHANGES / REJECTED), present/absent Unity evidence, legacy hash checks and unresolved issues. Manager decides REVIEWED/INTEGRATED/VERIFIED separately.
- **Stop:** unresolved failure; any missing exact-HEAD proof; protected archive difference; accidental world transfer or unregistered lock.

## Delivery rules for every later task

Each dispatched packet must explicitly name its exact handoff path and public GitHub URL, plain-English purpose, Low/Medium/High difficulty and reason, best suited actual model/provider, branch/worktree/starting HEAD, current live lock owner, exact writable file list, claim number + [RUNNING]/[BLOCKED]/[HANDED-OFF] lifecycle, verification tier, exclusions, safe stopping conditions and one paste-ready dispatch line. A claim tracks execution; it does not confer source ownership or override Docs/Project/coordination/active-tasks.md.

No implementation may start from a DRAFT spec. No concurrent overlapping writers, stashes/cleans/discards of unrelated dirty files, editor bypass, archive regeneration, game source changes via docs-only claim, Windows build, or main merge without separate authority.

## Gate status ledger

| Gate | Current status |
| --- | --- |
| Four draft documents and review | DRAFT under #436; no acceptance |
| Manager spec approval | NOT GIVEN |
| O2 authored forest / six-species modelling | NOT RELEASED; 3 additional species HOLD |
| Production locks and claims | NOT REGISTERED by #436 |
| Tier 1 code tests | NOT RUN by #436 |
| Unity/Editor Tier 2 | NOT RUN by #436 |
| First complete O1 external demo / personal test gate | Separate #424 programme, not satisfied by this specification |
| O2 Gate 1 | FUTURE INTERNAL ONLY |
| O2 complete six-species Gate 2/3 | FUTURE, SEPARATE SPECS AND ACCEPTANCE |
| Dashboard/Drive sync | NOT VERIFIED |
