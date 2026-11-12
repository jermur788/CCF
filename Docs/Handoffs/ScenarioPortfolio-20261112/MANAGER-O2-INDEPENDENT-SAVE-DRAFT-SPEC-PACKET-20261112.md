# CCF #436 — preimplementation four-part DRAFT specification for independent-scenario save safety

**Date:** 12 November 2026  
**Purpose:** author a reviewable, safe *specification only* for the first second-forest save boundary, under accepted D-079/D-080 and Manager resolution D-081.  
**Difficulty:** HIGH — old save preservation, restore safety, game-world identity and frozen reference compatibility.  
**Best suited:** GPT-6 High GitHub-connected source-analysis/spec author, with independent reviewer later. Actual model/effort must be disclosed.  
**Claim:** https://github.com/jermur788/CCF/issues/436 — **READY, NOT RUNNING**.  
**Packet branch:** `docs/overall-manager-o2-save-isolation-draft-spec-packet-20261112-v1` (main-based).  
**Worker branch:** NEW `docs/o2-save-isolation-four-part-draft-spec-20261112-v1` from packet branch after exact current HEAD / branch absence and live ownership preflight.  
**Lane:** browser/GitHub document/source read-only plus exact five new Markdown output files; no local machine worktree, Unity lease or game code.

## 1. Product authority and research intake

Read main `Docs/Project/decision-log.md` (D-051/052, D-059, D-078–D-081), `Docs/Project/game-brief.md`, `Docs/Project/current-milestone.md` and `Docs/Project/unity-project-overview.md`. **D-081 is binding pre-spec policy, not implementation authorisation.** Complete Scenario One first external demo retains Sitka/Oak/Beech parity, Rack V2 five parallel racks, MFM, review/history, repeated decisions, user personal playtest and explicit Windows-build permission. Scenario One Delivery Manager #424 owns all current source integration/Unity and live `Docs/Project/coordination/active-tasks.md`; Overall Manager #425 owns future-scenario strategy and approved product authority.

Read [#434 species research](https://github.com/jermur788/CCF/blob/docs/eight-scenario-species-lifecycle-evidence-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-TREE-SPECIES-LIFECYCLE-MATRIX-RESULT.md) and [Manager #434 disposition](https://github.com/jermur788/CCF/blob/docs/overall-manager-434-research-disposition-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/OVERALL-MANAGER-434-SPECIES-LIFECYCLE-RESEARCH-DISPOSITION-AND-O2-RELEASE-POLICY.md).

D-081 resolves the programme:
- **Initial O2 internal engineering proof** can reuse Sitka, Oak, Beech and a real irregular authored starting stand solely to qualify second-world identity, saves and longitudinal management, **not** to mark O2 playable complete.
- **Complete O2 acceptance retains the earlier six-species roster** (Sitka/Oak/Beech plus Scots pine/downy birch/common alder). All added species currently HOLD/unready and require separate later model/visual/teaching specs and qualification. No fourth-species automatic build in this task.
- The Stage 1 scenario-independent saves must never silently overwrite/cross-load Scenario One or each other. O8 estate-local budgets are future separate scope. No shared saved forest, trees or money between eight independent chapters.

## 2. Source inspection preflight — no guesses

Inspect current main **actual code and tests** before specifying a contract, citing files/functions:
- `Assets/ForestPrototype/ForestSaveController.cs`: existing one `forest-save.json`, .tmp/.bak, save and preflight validation pipeline, error/recovery behavior.
- `Assets/ForestPrototype/ForestSaveData.cs`: current schema/version (not every historical save has scenarioOne fields), extant world/forest IDs and serialised forests.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` and `ScenarioOneDefinition.cs`: current scenario ID, restore warns on mismatch but continues, startup and new-game definitions, Reference Preview return behavior.
- Archived/legacy Reference Future v1, Legacy40, Enlarged80 and direct Save/Load UI routes; inspect actual code to identify which path persists worlds, not only front-end labels.
- Current tests/verification harnesses for save/preflight/old-version/restoration, read-only; avoid inventing new test filenames or claiming a test already passed.

**If exact scene or runtime details are not inspectable, mark as 'unknown—to verify' rather than a Unity fact.** Review old-version compatibility requirements; do not infer from current schema alone.

## 3. Draft four-file spec deliverables — DRAFT only

Write the **NEW** four standard files at:
- `specs/scenario-independent-save-isolation-v1/spec.md`
- `specs/scenario-independent-save-isolation-v1/plan.md`
- `specs/scenario-independent-save-isolation-v1/tasks.md`
- `specs/scenario-independent-save-isolation-v1/checklists/requirements.md`

All state **DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY**. Include a single new evidence/readiness handoff at `Docs/Handoffs/ScenarioPortfolio-20261112/O2-INDEPENDENT-SAVE-DRAFT-SPEC-HANDOFF.md`.

### spec.md — behavioural contract and decisions

Required clear contract:
1. Scenario identity, selectable O1/O2 new game and saves, explicit preservation of existing Scenario One **legacy save location** and old compatibility; do not choose an unsafe migration-by-overwrite.
2. Unique per-scenario filename/slot policy and validated identifier set, absent/corrupt/foreign slot handling, root path rules, extension and backup/temporary files, with safe default.
3. **Pre-mutation wrong-scenario load rejection** in the earliest actual load entrypoint, including missing/null ID and old versions; fail closed and leave currently loaded world, scene, annual history, economy and Work Plan unchanged.
4. Backward compatibility: old save format **v1–v24**, Legacy40 and frozen Reference Future v1. No new metadata written to frozen archives, no silent Scenario One reference-change or world re-baselining.
5. Save/load transaction and recovery: unreadable/newer/partial/backup conflicts; no data loss from file naming; source/target identity and version validated at correct time; preserve unknown-field behavior conservatively as existing code allows.
6. O1 and O2 independent historical results and age/annual review; ID collision and cross-slot content negative tests; honest user-visible error states; no transfer of trees, goods or cash between scenarios.
7. **Non-goals:** no generic eight-world framework, O2 species model, estate multi-compartment ledger, new rack geometry, global achievements, redesign of Work Plan, scene mass migration or Windows build.
8. Explicit **open technical questions** derived from source inspection, with recommended conservative choice and required evidence. Do not solve uncertain policy by inventing undocumented behavior.

### plan.md — minimal staged approach and ownership

Design a minimal additive route that leaves O1 current filename and frozen archive stable, with early static source audit → narrow offline/Editor qualification (in later authorised phase) → integration gates. Document dependency on feature model and O1 end-to-end source, as well as branch/worktree ownership registration and single Unity lease. No concurrent writing on S1 save controller while #424 holds it. Do not prescribe broad framework extraction without demonstration that simple separate routing fails.

### tasks.md — bounded executable work when separately approved

Divide future steps into small work packets with clear allowed code files (tentative, requires live preflight), verification tier 1/2 chosen by risk, explicit no acceptance until runtime candidate exact-HEAD tests and Manager review. The **first draft spec is not approval**; mark every implementation task BLOCKED until spec approval and live coordination. Do not add another broad Tier 3 to each small packet, no test weakening, no parallel overlapping repo writes.

### checklists/requirements.md — acceptance checks

Use requirement IDs and observable pass/fail statements, including:
- O1 old-save identity, no overwrites and no save-drift;
- O2 initial authored world selection, first save and reload;
- negative O1 save selected into O2, negative O2 into O1, wrong schema/version, malformed and missing scenario IDs, duplicate IDs;
- mutation atomicity/no partial restores of any world, cash, Work Plan, tree state/history;
- .tmp/.bak integrity/recovery, error messages and no data destruction;
- Legacy40/Enlarged80/frozen Reference Future v1 compatibility;
- source static checks, offline negative tests and focused later Unity runtime verification;
- O2 Gate 1 *internal-only* status, no public or first-external-demo claims;
- O2 Gate 2 *six species* separate later feature-spec work, not in scope here.
Choose exact verification tools only if existing repo evidence supports them; otherwise note expected class of test, not fabricated command.

## 4. Integrity and claim lifecycle

**Allowed GitHub edits on NEW result branch ONLY** the four new draft specs and one handoff. **No others.** Do not modify existing files, main, current milestones, coordination, code or tests. No local Unity, Windows build, runtime claims or feature acceptance.

Before authoring, inspect [#436](https://github.com/jermur788/CCF/issues/436), current main/latest source, proposed result branch absence, and **live** `Docs/Project/coordination/active-tasks.md` READ ONLY; #424 is the single-writer coordinator, and conflict means stop. At start mark claim **[RUNNING]** with actual provider, GitHub docs-only branch, starting HEAD and writable paths; [BLOCKED] if any ownership/existing-file issue.

After all five docs exist, read them back, compare result branch to packet branch to show **only five newly added Markdown files**. Mark issue **[HANDED-OFF]**, link exact handoff, close if done. Return missing source evidence and findings **without changing DRAFT to APPROVED**. Manager #425 will check the draft, send an independent focused review if material, and only then determine future spec approval separately from delivery #424 and product owner as required. No game implementation is started by completion of this writer task. Dashboard and Drive status unverified unless checked.

**Paste-ready dispatch:**

> CCF DRAFT-spec task #436, HIGH difficulty; best suited GPT-6 High source-analysis/GitHub docs-only. Read and follow the full handoff: https://github.com/jermur788/CCF/blob/docs/overall-manager-o2-save-isolation-draft-spec-packet-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-O2-INDEPENDENT-SAVE-DRAFT-SPEC-PACKET-20261112.md . User-approved programme and Manager D-081 settle the first internal O2 three-species proof versus eventual complete six-species O2. Produce four proper DRAFT files for scenario-isolated save slots, wrong-scenario fail-before-mutation, legacy Scenario One frozen Reference/Legacy40 compatibility and focused negative load tests, plus one GitHub handoff. NEW disjoint branch/path and claim lifecycle exactly as packet; zero C#/Unity/Editor/build/accepted spec changes. Return direct durable result URL and unresolved evidence only.
