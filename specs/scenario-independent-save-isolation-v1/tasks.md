# Independent Scenario Save Isolation V1 — Future Task Packets

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
These packets are **BLOCKED** until the four-part spec is explicitly APPROVED and a fresh, separate production claim / live ownership record exists. Task #436 authorised Markdown drafting only; **#524 (three-profile amendment) likewise authorises Markdown drafting only.** Do not mark any task started, implemented, verified or accepted based on this document. No task may be marked [RUNNING] without independent approval and an actual registered local owner.

**Authority split (#524):** **#425** is the accountable shared save-policy/spec owner and decides spec approval and blockers B-01–B-15 (with owner/save sign-off where required). **#424** owns later source integration, the live board, the one named source writer and Unity scheduling. Neither role is assumed by a docs claim.

## Task dependency graph

T-00 audit + approval → T-01 three-profile slot routing → T-02 fail-closed, profile-specific preflight → T-03 profile/session/scene lifecycle, preview and namespaces → T-04 backup/legacy and reference protection → T-05 exact-head feature qualification + independent review → Manager integration disposition.

An approved implementer may merge or subdivide adjacent bounded packets only after confirming non-overlap and the actual current source; no automatic team dispatch or simultaneous writers on ForestSaveController. The O2 authored starting forest and the compact authored world (H1/path/G1 generator, geometry) are **separate approved dependencies**, not included in these docs. The compact feature consumes T-01–T-03 rather than building its own router.

### T-00 — Re-pin source, resolve open questions and obtain spec approval

- **State:** BLOCKED. **Difficulty:** High; cross-world ownership, schema and old files. **Route:** FREE HERE Manager/source analysis, with independent review where material.
- **Permitted:** read-only current code and real historical save/archived Reference Future fixtures; propose a signed identity/scene/backup/prefs policy and an exact three-slot file/path matrix. No source changes.
- **Inspect:** ForestSaveController, ForestSaveData, ForestSaveValidation (+ partials), StandGeometryModel, ScenarioOneManager/Definition/SaveMigration, ScenarioReferenceArchive, StandaloneSessionMenu, LearningObjectivesView*, MenuHelpView*, ForestStartingStand, current tests, current live coordination and #424 ownership.
- **Output:** revised approved spec/plan/checklist; an old-version fixture manifest with genuine/synthetic provenance (B-06); a source-caller map, including every direct LoadData/CaptureData caller (B-15); recorded rulings on B-01–B-15; explicit Manager review. No code launch before that approval.
- **Stop:** missing old fixtures, unclear ID/provenance, O2 or compact definition absent, D-082 unresolved where compact is presented as first demo, active overlap or product scope disagreement.

### T-01 — Fixed three-profile slot and selection routing (amended #524)

- **State:** BLOCKED. **Difficulty:** Medium; shared disk risk. **Route:** SOL/CODEX local for implementation, only after approval and an exact claim.
- **Tentative code ownership (must re-check):** ForestSaveController.cs, a narrowly reviewed fixed slot-router helper if needed, the session-selection seam in UI/StandaloneSessionMenu*. No generic eight-world framework and no per-species content.
- **Required behaviour:**
  - existing O1 forest-save.json, .tmp and .bak continue unchanged;
  - proposed compact slot forest-save-scenario-one-compact-v1.json and proposed O2 slot forest-save-scenario-two.json, each with only its own sidecars;
  - strict compile-time allowlist and pairwise-distinct paths (case-insensitive too);
  - no implicit migration/fallback in any of the six directions, no deleted save, and no new game overwriting an old slot;
  - selecting a profile writes nothing;
  - the UI duplicate path literal is replaced by the router query.
- **Verification:** Tier 1 path/alias tests and a static F5/F9/F10/Continue route audit; before/after hashes on all three primaries and .bak files. Later targeted Editor confirmation where relevant.
- **Handoff:** exact commit, test evidence, remaining risks; no own integration authority.
- **Stop:** any path collision, unexpected O1 write or existing file ownership conflict.

### T-02 — Profile-specific identity and no-mutation load preflight (amended #524)

- **State:** BLOCKED. **Difficulty:** High; greatest cross-save risk. **Route:** SOL/CODEX local, optionally independent read-only browser review.
- **Tentative files:** ForestSaveData.cs (additive envelope type only, if B-02 approves), ForestSaveValidation.cs (+ relevant partial), ForestSaveController.cs, StandGeometryModel.cs only for B-11 compact dispatch and unknown fail-closed; minimal ScenarioOneManager.cs identity alignment only if directly required; narrowly bounded verification tests.
- **Required behaviour:**
  - **one shared preflight** for every route (spec FR-01/FR-02);
  - new-profile envelope with raw presence/type/duplicate checks (a JsonUtility default cannot masquerade as explicit input); fixed slot, active scene/definition, geometry/content and nested ID agree;
  - historical slot rejects an envelope marker, and a foreign nested scenarioId per the B-03 ruling, before any version rule; v1–v9 legacy O1-only; v10–v24 classified per the B-04 ruling;
  - **compact/O2 missing/unknown/newer versions reject before first mutation; genuine historical newer-version warn/best-effort is preserved**;
  - malformed/missing/conflicting/duplicate identity rejected in CanLoad/Load/LoadData alike;
  - existing duplicate-tree, cell, rack, species, model and management-event validation stays intact; preserve original rejection reasons where possible.
- **Verification:** Tier 1 deterministic table cases and a no-mutation sentinel; genuine O1 old-save fixtures; all six copied/renamed cross-placements; unsupported future-version tests per profile; a historical serialization byte-identity check. Source review of mutating order. Do not weaken checks to pass old malformed files.
- **Handoff:** proof that each load entrypoint fails closed per profile and that legacy identity classification follows the recorded ruling; schema/v24 compatibility report.
- **Stop:** legacy ambiguity unreviewed, archive/hash drift, fallthrough to the manager's late warning, any failed-load state mutation, or any global tightening of historical newer-version behaviour without approval.

### T-03 — Three authored forest sessions, UI, preview bridge and namespaces (amended #524)

- **State:** BLOCKED on T-02 **and** on the separately qualified O2 and compact authored-world/scene dependencies. **Difficulty:** High; session and scene transition can mutate before validation. **Route:** SOL/CODEX local with serialized Unity.
- **Tentative files:** StandaloneSessionMenu.cs/.Startup.cs, authorised profile selection UI, scene/new-game bootstrapping (B-10) and a minimal manager/definition interface; the Reference preview bridge only for the profile gate (FR-12); LearningObjectivesView*/MenuHelpView* for profile prefixes (FR-10). **No mass scene rewrite without a new packet.**
- **Required behaviour:**
  - the player explicitly selects historical O1, compact O1 or O2;
  - each uses its actual authored start, not a relabelled O1;
  - first save/reload selects the correct slot;
  - wrong-profile load refusal leaves the current scene, selection, forest, UI and prefs;
  - historical Reference Future preview return stays byte/state compatible, and preview is not offered in compact/O2;
  - historical prefs keys unchanged by any compact/O2 action, including reset;
  - compact teaching evidence persisted per FR-11 (B-08).
- **Verification:** narrow Tier 2 Editor New/Continue/F9/F10 and preview contract; prefs snapshot; retain O1 tests, with screenshot evidence only for changed UI. No first external O2 or compact-complete claim.
- **Handoff:** actual candidate/Unity logs, scene-world identity before/after, saved history/cash/order witness, prefs diff.
- **Stop:** unqualified O2/compact scene/model, preview bypass, transition before preflight, or any historical prefs change.

### T-04 — Backup, fault injection and v1–v24 regression

- **State:** BLOCKED on T-03. **Difficulty:** High; accidental data loss. **Route:** SOL/CODEX local focused verification/repairs, no broad refactor.
- **Tentative files:** limited ForestSaveController.cs (if needed), a small explicit recovery UI seam, targeted verification under Tools/Verification/SaveHardeningVerification.cs / SaveCellIndexHardening/ / Parity08Save/ / RackV2Save/ / TeachingPersistence/ / Startup/ / ScenarioReferenceVerification.cs. This list is an inspection map, **not** permission to edit every listed file.
- **Required behaviour:**
  - per-slot .tmp/.bak containment for **all three** profiles;
  - no automatic unsafe fallback; preserve primary and backup until an explicit valid recovery;
  - cases: corrupted primary + valid own backup; corrupted/foreign backup (including another profile's file in the .bak position); stale temp; failed write/replace; permission error; future-version file;
  - no false success or cross-slot recovery; no frozen v12 archive edits.
- **Verification:** Tier 1 offline corruption/IO tests; Tier 2 only for actual runtime/Editor recovery and UI. Byte/hash checks on the O1 v1–v24 fixture set, Legacy40, Enlarged80, model 0, Rack V2, MFM v23, v24 teaching, the frozen Reference Future preview+resume, and all three slots. Avoid test re-baselining.
- **Handoff:** test/failure matrix, old-file preservation, platform fallback caveats, recovery UX.
- **Stop:** the test harness damages unbacked-up real saves, file-promotion ambiguity, or a non-atomic fallback with unmitigated loss.

### T-05 — Feature conformance and integration decision

- **State:** BLOCKED on implementation handoffs T-01…T-04. **Difficulty:** High; compound identity/compatibility. **Route:** one serialized Unity Tier 2 qualification worker, an independent focused reviewer, then the #425/#424 integration decision.
- **Scope:** read/qualify the exact candidate branch/HEAD; no opportunistic source edits. One feature-level acceptance matrix covering:
  - historical O1 old/current;
  - compact and O2 initial authored worlds, and profile switching;
  - six-way wrong-load, partial/backup and long-form annual history;
  - frozen preview, namespaces, real negative UI, and no cross-profile money/trees.

  Check each item in requirements.md (ISO-01–ISO-63).
- **Verification:** Tier 2 FEATURE, including Editor runtime and rendered/input evidence where relevant. Existing relevant Tier 1 negative fixtures remain supplementary; no repetitive full Tier 3 and no weakening of expected results. Any source repair after a failed gate requires a new candidate and a proportional retest.
- **Handoff:** durable GitHub Markdown with exact source and test evidence, reviewer verdict (APPROVED / NEEDS CHANGES / REJECTED), present/absent Unity evidence, legacy hash checks and unresolved issues. The Manager decides REVIEWED/INTEGRATED/VERIFIED separately.
- **Stop:** an unresolved failure; any missing exact-HEAD proof; a protected archive difference; accidental world transfer or an unregistered lock.

## Delivery rules for every later task

Each dispatched packet must explicitly name:
- its exact handoff path and public GitHub URL, and a plain-English purpose;
- Low/Medium/High difficulty and the reason, and the best-suited actual model/provider;
- branch/worktree/starting HEAD, the current live lock owner and the exact writable file list;
- claim number + [RUNNING]/[BLOCKED]/[HANDED-OFF] lifecycle, verification tier, exclusions and safe stopping conditions;
- one paste-ready dispatch line.

A claim tracks execution; it does not confer source ownership or override Docs/Project/coordination/active-tasks.md.

No implementation may start from a DRAFT spec. Not permitted without separate authority: concurrent overlapping writers; stashes/cleans/discards of unrelated dirty files; editor bypass; archive regeneration; game source changes via a docs-only claim; a Windows build; a main merge.

## Gate status ledger

| Gate | Current status |
| --- | --- |
| Four draft documents and review | DRAFT under #436, amended under #524 to three profiles; no acceptance |
| Independent nonauthor persistence/doc review of #524 amendment | NOT STARTED — next, under #425 |
| Manager spec approval | NOT GIVEN |
| Owner/save rulings B-01–B-15 | OPEN |
| D-082 canonical compact first-demo exception (#471) | PAUSED / NOT ACCEPTED |
| Compact authored world (H1/path/G1, geometry) | DRAFT specs only; NOT RELEASED |
| O2 authored forest / six-species modelling | NOT RELEASED; 3 additional species HOLD |
| Production locks and claims | NOT REGISTERED by #436 or #524 |
| Tier 1 code tests | NOT RUN by #436 or #524 |
| Unity/Editor Tier 2 | NOT RUN by #436 or #524 |
| First complete O1 external demo / personal test gate | Separate #424 programme, not satisfied by this specification |
| O2 Gate 1 | FUTURE INTERNAL ONLY |
| O2 complete six-species Gate 2/3 | FUTURE, SEPARATE SPECS AND ACCEPTANCE |
| Dashboard/Drive sync | NOT VERIFIED |
