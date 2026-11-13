# CCF Text Editor — research, four-file DRAFT specification and standalone browser prototype: worker result

**Task/claim:** [#473](https://github.com/jermur788/CCF/issues/473) (independent worker), issued by Scenario One Implementation Manager [#424](https://github.com/jermur788/CCF/issues/424) under [binding packet](https://github.com/jermur788/CCF/blob/docs/manager-424-text-editor-research-spec-prototype-dispatch-20261113-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261113/MANAGER-424-CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-PROTOTYPE-DISPATCH-20261113.md).  
**Date:** 2026-11-13 UTC.  
**Actual worker:** OpenAI ChatGPT **GPT-6**, independent GitHub/browser source/research developer session; numeric reasoning effort **not independently attested**.  
**Writable branch:** `feature/ccf-text-editor-browser-prototype-20261113-v1`, created new at verified main starting point `fc18190b976f1363f1b5d1b0881810db2f8c6e39`.  
**Worktree/Unity:** **none**; no local CCF checkout, no Unity/Editor/process, no user save/PlayerPrefs access and no physical CCF-Ops machine lease. Connector-backed GitHub writes only.  
**Result status:** **IMPLEMENTED STANDALONE SOURCE PROTOTYPE + PUBLISHED PARTIAL SOURCE AUDIT + PUBLISHED FOUR-FILE DRAFT**. **NOT GAME-INTEGRATED, NOT UNITY-VERIFIED, NOT SPEC-APPROVED, NOT USER-ACCEPTED; ACTUAL BROWSER CLICK-THROUGH NOT RUN.**

## 1. Executive result and release boundary

The user requested a lightweight offline browser CCF text-editing interface, not another Unity-only editing panel. The repository now contains a **standalone, zero-dependency HTML/CSS/JavaScript editor** in `Tools/ContentReview/`. The owner can inspect source-extracted actual Scenario One text, navigate page by page, edit wording in memory, search across text, inspect original versus draft, choose conditional variants, validate protected metadata/tokens and download a revisioned JSON draft. The browser interface does **not** change the game, GitHub, saved progress, rack/tree identities, PlayerPrefs, calculations or any Unity file. A later separate content adapter and explicit approvals are necessary before edited words appear in Scenario One.

The research maps current main C# and a separately owned Objective 1 candidate; it **does not prove that every possible UI string has been collected**. The 79-page/268-field starter catalogue is useful and authentic but **partial**. Future scenarios and a compact Scenario One draft remain unpopulated, labelled placeholders, not fabricated curricula.

**Manager review disposition requested:** evaluate the DRAFT/spec architecture and tool in an **independent non-author review**. Do not release a production adapter or claim content completeness from this handoff.

## 2. New files / exact GitHub paths

**Research:**
- `Docs/Research/ContentReview/CCF-PLAYER-FACING-TEXT-INVENTORY-AND-ARCHITECTURE-20261113.md`

**Four-file DRAFT specification, all explicitly NOT APPROVED:**
- `specs/ccf-cross-scenario-text-editor-v1/spec.md`
- `specs/ccf-cross-scenario-text-editor-v1/plan.md`
- `specs/ccf-cross-scenario-text-editor-v1/tasks.md`
- `specs/ccf-cross-scenario-text-editor-v1/checklists/requirements.md`

**Isolated browser application, data, source drift and tests:**
- `Tools/ContentReview/index.html`
- `Tools/ContentReview/styles.css`
- `Tools/ContentReview/app.js`
- `Tools/ContentReview/core.js`
- `Tools/ContentReview/catalog.json`
- `Tools/ContentReview/catalog.js` (local `file://` seed mirror)
- `Tools/ContentReview/source-map.json`
- `Tools/ContentReview/source-audit.mjs`
- `Tools/ContentReview/tests.mjs`
- `Tools/ContentReview/README.md`

**This final handoff:** `Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md`.

**GitHub pre-handoff comparison/readback:** compared `feature/ccf-text-editor-browser-prototype-20261113-v1` with its registered main starting point: **15 files, all ADDED, 0 existing files modified/deleted, 0 paths outside allowed new-file scope; ahead 26, behind 0 at that comparison**. The 15 new files were individually read back with nonempty GitHub blobs. Two later DRAFT task/checklist status-only revisions remain inside the same new files; recompare including this final 16th file at administrative close. **No main merge and no production `Assets/`, `Packages/`, `ProjectSettings/`, save, live coordination or approved spec edits.**

## 3. Actual content coverage and limits

| Item | Observed source/project result |
|---|---:|
| GitHub main baseline files read/blob-anchored | **28 C# files** |
| Quoted lexical literal candidates in those files | **2,934** (includes code keys/classes; **not** actual display count) |
| Interpolated-string starts in those files | **225** (not all dynamically generated text) |
| Navigable audited catalogue pages | **79** |
| Source-extracted catalogue fields | **268** |
| Main source fields | **265** |
| Separately labelled unmerged Objective 1 candidate fields | **3** |
| Classes in catalogue | **235 collected; 28 conditional; 5 generated template** |
| Full all-game text coverage percentage | **UNKNOWN / NOT MEASURED** |
| Future scenario/compact O1 substantive pages | **0 — deliberately not invented** |

**Actual source families inspected:** the 3-page onboarding `ScenarioOneOnboardingCopy` and approved copy document, `ScenarioOneOnboardingView`, `LearningObjectivesView.Curriculum/Sequence`, `ScenarioOneTeachingSequence`, `MenuHelpView.Text`, `ControlsReference`, `ScenarioOneRackBayText`, `ActiveObjectiveHudWidget`, `ScenarioOneUiRoot`, `WorkPlanView`, `WorkPlanOverview`, `AnnualReviewView`, `StandMapView`, `StandMapTreatmentReview`, `TreeInspectionView`, `ScenarioOneTreatmentReview`, `WalkingHudView`, `HudMinimapView`, `TeachingDiagram`, `UiKit`, startup/session menus, `ScenarioOneObjectives`, `ScenarioOneCompletionGuidance` and `ManagementHistoryText`. The exact 28-file list and Git blob IDs are in `source-map.json`.

**Source distinctions:** `main-source` means text collected from checked-in main, not independently observed in a user-launched Editor; `unmerged-candidate` means selected #443 Objective 1 copy from its separate production branch, **not** automatically approved or in main; `draft-no-text` means actual future story copy absent. Rack V2 versus old/general objectives and help use separately labelled variants; `rack.*` save keys remain protected.

**Known outstanding coverage:** many Work Plan task/estimate/error text fields, Annual Review / history/storm and repeated-year variants, in-forest/Stand Map/inspection/teaching/selection messages, completion guidance variants, session menu, serialized scenes/prefabs/UI resources/ScriptableObjects, artwork with baked text and source outside the 28 selected C# files. The imported dynamic Work Plan/Annual Review templates are explanatory `{{tokens}}`, not a running C# formatter. `source-audit.mjs` is a future full-checkout drift/serialized-candidate scanner, **not a claim that it has already surveyed all files**.

**Research recommendation:** versioned portable, dependency-free JSON with separate future **typed read-only Unity content adapter** and exact existing literal fallback. ScriptableObjects require Unity for editing, Unity Localization would add new tooling complexity for current English-only use, and naive C# string replacement is unsafe. Git reviews remain approval authority; no CMS/cloud account required.

## 4. Source-correctness repair during this run

The initial regex extraction incorrectly stopped at a semicolon inside two valid C# string literals, producing empty text for `rack.create` and `rack.map` **Enlarged80 Rack V2 variants**. The first direct V8 schema check correctly FAILED with both required fields blank. The worker fetched the exact originating C# lines, restored the full word-for-word texts and line provenance, republished `catalog.json` and `catalog.js`, and re-ran schema/negative checks. **Final result is PASS, 30/30 direct V8 checks.** The failure and correction are retained as evidence for using a proper compiler-aware extraction audit before claiming exhaustive coverage.

A second hardening pass found import validation should defend more than gameplay IDs: the worker added immutable source-provenance/approval flags, package ID, coverage metadata and field notes to the validator and negative tests. A forged reviewed/integrated flag or changed provenance is now rejected on import. Exports always carry `draft-local-export`, `reviewed=false`, `unityIntegrated=false`.

## 5. Verification — distinct truth labels

| Verification | Status | Exact evidence boundary |
|---|---|---|
| GitHub publication / every pre-handoff file readback | **PASS** | 15 source/research/spec/tool files fetched after commits; nonempty blob IDs |
| Branch scope / no existing-source edit | **PASS** | 15 ADDED-only allowed paths in base comparison; no Unity or live coordination writes |
| JavaScript syntax of `core.js` / `app.js` and embedded seed parsing | **PASS — direct V8** | Evaluated from the GitHub published contents; `catalog.js` parsed equal to `catalog.json` |
| Baseline schema and identity preservation | **PASS — direct V8** | Actual 268 records and source-protected metadata validated |
| Edited text → JSON round-trip; Unicode, line breaks, arrow, apostrophes, backslash | **PASS — direct V8** | Pure data/validator contract, not an actual browser file-picker round-trip |
| Missing/extra/reordered token, duplicate ID, blank, control character, markup, malformed token, wrong revision/schema/source/provenance/approval/status/notes | **PASS — direct V8 negatives** | **30/30 direct V8 checks total** after final hardening |
| Browser DOM, actual `file://` load, click/edit/download/re-import, accessibility/render | **NOT RUN** | No attested local browser execution; source indicates intended behavior, not a verified run |
| Committed `node Tools/ContentReview/tests.mjs` | **NOT RUN** | Node test script published; cannot infer pass from equivalent V8 checks |
| Full-checkout `source-audit.mjs` / serialized-text completeness | **NOT RUN** | No local CCF checkout; scanner code exists |
| Unity compile, native 1280×720 / 1920×1080, Legacy40 and frozen Reference, save/progression parity | **NOT RUN / OUT OF SCOPE** | No adapter, Unity Editor, source edit or execution lease |
| Spec approval / user copy acceptance / integrated game display | **NOT GIVEN** | All spec files DRAFT; no release claim |

**Reproduction instructions** are in `Tools/ContentReview/README.md`. For a reviewer with a Git checkout: open `Tools/ContentReview/index.html` locally or via optional local-only Python HTTP server; inspect actual pages, edit a field, validate, export JSON, reopen/import it; run:
```sh
node Tools/ContentReview/tests.mjs
node Tools/ContentReview/source-audit.mjs . --json
```
Record browser/version and exact Node stdout/exit as new evidence; do not rewrite the above NOT RUN statuses without actual execution. A plain browser preview cannot qualify the Unity layout.

## 6. Four-file DRAFT spec and protected integration boundary

The four documents explicitly cover:
1. Hierarchical scenario/section/page/field UX, previous/next, condition/variant selector, search, dirty/diff, revert, preview and keyboard;
2. `ccf.content.v1` stable content identity separate from immutable `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, lesson IDs and save/world/rack/tree/work keys;
3. Token/Unicode/multiline/rich-text rejection, formatting/plural/provenance rules and source drift;
4. A **separately gated** typed read-only Unity adapter with exact old literal fallback and profile/save/history correctness, not an automatic source-text replacement;
5. Eight independent original-case scenarios in accepted teaching order O1 → O2 → O4 → O3 → O7 → O6 → O5 → O8, distinct from any unimplemented compact O1 profile;
6. Independent browser versus Unity native tests, 720p/1080p, old saves/Reference, unchanged gameplay predicates and management calculations;
7. Owner/worktree/Unity serialization, #443 candidate precedence and #424/#425/Multi-Rack role boundaries.

No accepted product text policy, game mechanic, D-065 calibration, annual calculation or task ownership has changed. An editor draft cannot be interpreted as player consent to change lesson goals or dynamic numbers.

## 7. Next exact steps and ownership

**Immediate next:** Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424) commissions **one independent, non-author tool/spec reviewer**. Suggested difficulty **MEDIUM–HIGH**, because C# source-profile identities, browser import integrity and output coverage claims require care. Best-equipped reviewer: independent Claude Opus 5.5 High or equivalent capable GitHub/Node/browser reviewer, on a **new disjoint docs-only branch and execution claim**. This worker's branch is the read-only candidate; the reviewer should check both the full DRAFT and actual `file://` UI/Node test behavior, verify the source-mirror/28-anchor truth and issue APPROVED/NEEDS CHANGES/REJECTED for **standalone tool and DRAFT quality separately**. No Unity lease needed for review.

**Then:** if review accepts the prototype, #424 gives the owner a real editor link/use instructions and a bounded request for copy review, while scheduling a true all-source coverage/serialized-asset inventory. Reconcile #443/#450 candidate source before any game adaptor. User approval of exact display text is separate from game-feature spec approval.

**Only after a new APPROVED feature spec and Manager source claim:** allocate one local implementation writer, approved `TextAsset` package, typed lookup/format and old-literal fallback for the first intro/Objectives/F1 slice. That later lane checks actual current/Legacy40/Reference profile, save/PlayerPrefs keys, focus/copy and 1280×720/1920×1080 screens under safe serial Unity ownership and independent review. Expand Work Plan/Annual/Map dynamic surfaces in bounded follow-ups; **never** automatically C#-replace all literals, alter saves or assume a new scenario's save router is ready.

**User action required now:** **none** for the worker's administrative completion; the owner may later review text UX after #424's independent review. No routine questions were needed. No Windows build or main merge authorised. **Dashboard sync: NOT VERIFIED. Drive mirror sync: NOT VERIFIED.**

## 8. Handoff conclusion

**Delivered:** 15 additive research/spec/browser/data/test files and this 16th durable GitHub record; 30/30 direct V8 contract checks PASS after recorded corrections; catalogued actual O1 text is navigable and safely editable/exportable at source level; DRAFT production integration spec exists.  
**Not proven:** actual browser execution, Node script run, complete player-facing text coverage, source adaptation, Unity/Legacy40/Reference rendering, copy acceptance.  
**Required verdict:** independent quality review first; no production implementation or game-content acceptance from #473.

**Claim closure:** mark [#473](https://github.com/jermur788/CCF/issues/473) `[HANDED-OFF]` and close once GitHub readback and 16-file final compare are recorded, then notify #424. Any status updates there are execution administration only.
