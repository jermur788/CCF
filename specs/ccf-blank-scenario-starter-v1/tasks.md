# CCF blank scenario starter v1 — bounded tasks

**Status: DRAFT / NOT APPROVED / NO G1 DISPATCH.** 20 November 2026. [Specification](spec.md) · [Plan](plan.md) · [Requirements](checklists/requirements.md) · [#856](https://github.com/jermur788/CCF/issues/856). Native actor, branch, worktree, source permissions and physical Unity lease: **UNASSIGNED**.

## Execution rule

The spec is SPEC REQUIRED and does not permit Unity/C# work while DRAFT. A future task packet must supply one approved spec/HEAD, refreshed full main/context SHAs, actor/provider/model, one sole branch/worktree, exact write files, separately exact read/compile-linked files, exclusion list, current active-tasks owner and one-machine Unity lock. Read or compilation of a source file is never automatic permission to write it. Protect #844's in-flight #820 gameplay and #836 visual candidates; #830's 71/71 historical targeted checks do not qualify any combined or blank runtime.

## T0 — independent nonauthor DRAFT review and approval

**Owner:** #425 appoints a different reviewer (browser/GitHub capable). **Input:** all four G0 files, live owner decisions, [#855 DRAFT corrected architecture](https://github.com/jermur788/CCF/blob/ff81d577326ddb95ddb1720ce5068d6e4e11341e/specs/ccf-shared-foundation-architecture-v1/spec.md), source at `main@7758e17c882fad01970d2f2fb8c09586bbcab5c1` and current #844 claim. **Output:** GitHub Markdown APPROVED/NEEDS CHANGES/REJECTED **for document quality**, not code acceptance. Examine source-backed manager-null UI, square map/rectangular copy, file-probe save fence, no rack, no artificial year/treasury and historical regression. #425/owner resolve material G4/G5 details, then **separately approve** this spec before G1. Existing template-first and separate-save owner principles need no new decision.

## T1 — exact G1 provenance and custody preflight (read-only until approval)

**Owner:** Scenario One 3D Manager [#844](https://github.com/jermur788/CCF/issues/844); prospective local Sol/Codex Unity worker (or equivalent actual qualified model) **not started**.

1. Refresh main, locked canonical context stamp, active-tasks, #844/#820/#836/#838 current source claims, #543/#544 save custodians, physical Ops board, one Unity Editor lock, editor process and unrelated dirty files.
2. Inspect actual source and serialized scene for shared UI, player, ground, menu, manager, ecology, spawner, map and saves. Discover map-view file path and every transitive compile-linked dependency with blob/version; do not infer a type from a diagram.
3. Choose exact non-overlapping custody: **A preferred** — #844 retains all shared UI/map/save C# source, template author owns only disjoint new scene/config/text; **B** — #844 formally delegates entire named shared seam set to one worker and freezes Compact writes there. One primary writer/file set; Editor usage serialized.
4. Obtain exact scene/asset/meta grants, D-032 licence scope where relevant, source/script permission and completed preflight. No native work on an unsigned or blocked lease.
5. Stop affected action if overlap/dirty/lease/source grant uncertain; preserve all unrelated work.

**Exit:** approved source/scene/compile-linked allowlist and lawful G1 claim, not code changes.

## T2 — shared UI preview capability (single shared source writer)

**Goal:** reuse existing `ScenarioOneUiRoot`, `WalkingHudView`, `WorkPlanView`, `AnnualReviewView`, `StandMapView` and `LearningObjectivesView` in a genuinely empty nonoperational mode without cloning scripts or source of truth. Source observations: [UI root](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs#L76-L143) constructs rack helpers when manager exists and [returns without manager](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs#L175-L177); [Work Plan](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/WorkPlanView.cs#L114-L169) exposes money/year controls; [objectives](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/LearningObjectivesView.cs#L118-L166) are rack-oriented.

**Proposed scope, not a write grant:** narrow existing root/view and relevant partial files. Prefer no active manager/ecology; if they are present, prove entirely inert. Fixed headings; `No eligible operations in this blank starter.`; `No year has been advanced; no annual review is available.`; `No objectives configured (development preview).`. No time-lapse, work, cash, racks, lessons or default spawn. Actual Editor positive + absence of null exceptions and negative control actions. Stop if fake manager money/trees or a new per-scenario UI engine is required.

## T3 — new Editor-only starter scene (single serialized Unity asset writer)

**Goal:** create a **new** scene with actual flat y=0 collider ground, default 30 × 30 m configurable positive finite X/Z footprint and player spawn; use existing code/prefabs by GUID, not duplicate scripts. Add only necessary scene-owned config/DEV text. No trees, paths, H1 clearing, roads, racks, fence, structures, species, forestry jobs, income or invented objective.

**Entry gate:** [EditorBuildSettings](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/ProjectSettings/EditorBuildSettings.asset) currently includes only `ForestTest`. Use direct Editor scene-open; do not change build list. [Startup/menu](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/StandaloneSessionMenu.cs) may bootstrap globally, and Restart reloads active build index: if unlisted scene would route to ForestTest, explicitly refuse restart in preview. Stop on accidental old-world instantiation/route or new art/publication grant.

## T4 — fail-closed historical save fence (one #844/#543/#544-coordinated writer)

**Goal:** no starter path reads, writes, probes or loads original `Application.persistentDataPath/forest-save.json`, its `.tmp` or `.bak`. Cover startup Continue and `CanLoad`; F10 Save/Load/`File.Exists`/Restart; F5/F9 from `ForestSaveController.Update`; direct public callers. Refuse **before** file existence, JSON read, capture or scene change. No new filename, save router, persisted ID, serializer or policy. Keep original five-rack historical save behaviour.

**Proof:** with actual historical save present, exercise all buttons, shortcuts, restarted scene and direct calls while recording read-access trace and original+sidecar byte hashes, mtimes and presence before/after. A disabled button and unchanged hash alone are insufficient: reading historic state also violates preview fence. Positive test historical ForestTest CanLoad/Save/Load via existing approved flow. Stop immediately on potential historic access, identity collision or unapproved serializer/profile change.

## T5 — true blank map and alternate-footprint asset-copy proof

**Goal:** Stand Map and HUD minimap show real empty 30 × 30 site, north-up X/Z and no trees, rack markers/edits, named `Dirt Path` / `Forest Clearing`, juveniles/shelters or fabricated relief. Source currently [has no property without ecology cells](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/Map/StandMapSource.cs#L119-L155) and [assumes a square map](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/Map/StandMapModel.cs#L116-L163). Approve only narrow read-only preview bounds path; never change [legacy saved geometry](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/StandGeometryModel.cs).

**Copy:** duplicate only starter scenario-owned scene/config/text and meta, set **45 × 60 m TEST ONLY** and prove rectangular ground **and map**. Original stays 30 × 30 m; shared scripts remain same GUID and no ecology/save geometry changes. Stop for broad square-map rewrite, source owner conflict, invented cells, or copied scripts.

## T6 — Tier 1 and Tier 2 native qualification

**Tier 1 FAST:** exact Unity 6000.6.0f1 candidate compilation, real new scene/player walk, 30 × 30 ground and preview UI screenshots/console. This does not prove saved-world safety.

**Tier 2 FEATURE:** complete [requirements/negative matrix](checklists/requirements.md): absent world/controls; no rack/tutorial; no historical read/write; exact copy/rectangle; original five-rack positive load/map/rack/work/review; v1–v24/Legacy40/Enlarged80/frozen Reference Future and protected deterministic contracts via proportionate existing tests. Do not weaken baselines for green. Screenshots, native logs, byte/access evidence, exact commit/branch, lock release and explicit NOT RUN where missing.

**Integration:** repeat affected tests on exact final candidate after composition with other approved source; historical #820 and #836 diverged checks are not enough. One durable GitHub Markdown handoff; independent review when required. No Windows build.

## T7 — independent source review, manager verdict and bounded integration

**Owner:** independent reviewer of actual code/serialized diffs, #844 3D manager integration, #425/owner for G4/G5. Record **IMPLEMENTED, COMPILED, RENDERED, REVIEWED, INTEGRATED, VERIFIED, OWNER ACCEPTED** distinctly. Review exact source/scene/meta/compile-linked allowlist and save invariants. Verdict APPROVED / NEEDS CHANGES / REJECTED; publish direct GitHub Markdown; only then #844 may issue next authorised integration packet. The accepted blank scene is still an interim authoring fixture, not played forestry.

## Conditional one-message local worker handoff — NOT ACTIVE

> **CCF Unity/3D #856 blank starter, future native G1 only after explicitly APPROVED four-file specification.** Difficulty HIGH; suitable actual available local GPT-6.1 Sol/Codex or equivalent Unity/C# worker. **Complete source-backed package:** [spec](https://github.com/jermur788/CCF/blob/docs/ccf-blank-starter-856-gpt6-g0-20261120-v1/specs/ccf-blank-scenario-starter-v1/spec.md), [plan](https://github.com/jermur788/CCF/blob/docs/ccf-blank-starter-856-gpt6-g0-20261120-v1/specs/ccf-blank-scenario-starter-v1/plan.md), [tasks](https://github.com/jermur788/CCF/blob/docs/ccf-blank-starter-856-gpt6-g0-20261120-v1/specs/ccf-blank-scenario-starter-v1/tasks.md), [checklist](https://github.com/jermur788/CCF/blob/docs/ccf-blank-starter-856-gpt6-g0-20261120-v1/specs/ccf-blank-scenario-starter-v1/checklists/requirements.md), [binding G0 handoff](https://github.com/jermur788/CCF/blob/docs/ccf-blank-starter-856-gpt6-g0-20261120-v1/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-BLANK-STARTER-G0-FOUR-FILE-SPEC-AUTHOR-RESULT-20261120.md). **NO START until #425/#844 issues a distinct exact APPROVED/spec+source packet** with real base/context full SHA, provider/model, unique branch/worktree, one source writer, explicit file+compile-linked allowlist, source/physical Ops ownership and actual Editor lease. Build editor-only real 30×30 blank ground/player with original mature five-rack shared HUD/menu/Work Plan/map/review/objective views by reference, not cloned scripts; no fake trees, racks, cash, work, ecology, year or objective; make safe empty panels and dev-only text; protect historic forest-save.json from **all reads/writes via Continue/F10/F5/F9 and callers**; duplicate scene/config/text-only into 45×60 test with truthful rectangular map. Do not change old scene, ProjectSettings, package, save schema/router, Multi, Compact feature, frozen references, main or Windows build. Run real Unity 6000.6.0f1 and targeted negative/old-world positive tests, source hashes/screens/read-traces, independent review as required; one GitHub Markdown result to #844/#425. STOP only affected work for overlap/lease/save/rectangle/scope blocker; preserve unrelated changes. **Current status DRAFT, source/Unity not authorised.**
