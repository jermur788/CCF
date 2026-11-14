# Manager disposition: #480 independent approval; bounded N-01 Tree Marks follow-up #487

**Date:** 14 November 2026  
**Management status:** #480 **ACCEPTED as an independent, non-author executed review**, with two distinct verdicts. Tool **APPROVED FOR OWNER USE AS STANDALONE BROWSER EDITOR**; four-file specification **APPROVED FOR DRAFT QUALITY ONLY** and **still DRAFT / NOT APPROVED for Unity implementation**. #480 is already HANDED-OFF/CLOSED; this management record does not recreate its testing or independently verify the local scratch images.  
**Source review:** [#480 Claude Sonnet 5.5 full browser/Node report](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-477-delta-review-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-477-INDEPENDENT-DELTA-REVIEW-20261114.md).  
**Approved standalone source candidate:** [#477 repair branch](https://github.com/jermur788/CCF/tree/feature/ccf-text-editor-474-repair-20261114-v1/Tools/ContentReview) pinned at `94b82a0149c549a80324bb6212e038731125651f`; **not merged into main or integrated in Unity**.

## A. Evidence-based disposition

The different non-author reviewer registered on #480, ran the committed Node suite twice with real Node 26.13.1 (**36/36 PASS**, exit zero), full candidate source scanner (**28 verified main C# anchors, 0 changed, 0 missing, 88 unmapped**; not a player-visible text census), and the **actual** local `index.html` in Edge 157 at 1280×720, 1920×1080 and 560px. Real pointer hit testing reached first, middle and last page, including scenario sidebar. **31/31 hostile import files were rejected at each of the two desktop sizes**, with no injected script execution or approval-shaped key leakage. Report read back from GitHub; review branch is **exactly one new Markdown file** against main. This is enough to accept the review verdict for **owner offline browser use**, not evidence for cross-browser parity, Mac, game runtime, save, image UI or production acceptance. Firefox, macOS, manual human click-through, Unity and full game text census remain unverified.

Original F-01–F-13 and S-01–S-05 now pass in the reviewer report. **No re-run of those full tests is required** just to allow the owner to use the standalone editor. The report's one MINOR is new source-fidelity finding **N-01** (Tree Marks full-F1 composition), carried as non-blocking for all other pages. Observations O-1 (9px scrollable coverage clipping), O-2 (other engines not tested), O-3 (two ternary statement-start anchors), O-4 (additional invisible Unicode outside the accepted S-03 contract) and O-5 (old-draft error wording) remain nonblocking. Do not silently broaden #487 to these, or falsely call them all repaired.

### Owner use boundary

Owner may use the externally browsed local editor on **the independently approved #477 source**, with its current 79-page/269-field starter catalogue, to review and export standalone drafts. Open `Tools/ContentReview/index.html` from a local checkout; follow `Tools/ContentReview/README.md`. A JSON export is not GitHub publication or changed game text. **Until #487 is corrected, disregard the editable composite full-F1 body of the Tree Marks help page**: it contains invented blank fragments instead of C#-composed runtime text. Other fields still carry their own source-status/coverage caveats. This approval does not promote the DRAFT, accept final wording or authorise a Unity importer.

## B. #487 bounded work packet

**Task issue:** [#487 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/487).  
**Purpose in plain English:** stop displaying a fabricated editable Tree Marks explanation; provide source-exact editable constituent wording and an explicit readonly/derived composition boundary.  
**Difficulty:** **LOW–MEDIUM** — a small source catalogue/schema/editor change and tests, no game mechanics, but needs careful provenance and safe JSON import.  
**Best-equipped model/provider:** one independent implementation-capable Sol/Codex or equivalent GitHub+Node+real browser worker. Actual worker **not registered or running**; model is a recommendation.  
**Exact source baseline:** pinned independently reviewed `feature/ccf-text-editor-474-repair-20261114-v1` at `94b82a0149c549a80324bb6212e038731125651f`; original and #480 review branches **read-only**.  
**Proposed writable branch:** `feature/ccf-text-editor-treemarks-n01-20261114-v1`, create only after actual worker checks for collision/ownership, from **the pinned #477 candidate**, not current main.  
**Local lane/worktree:** a **new isolated disposable scratch checkout**, exact path to be declared by worker at registration. There is no pre-created checkout, machine/Unity lock, editor lease or right to touch user's existing checkout.  
**Claim:** [#487](https://github.com/jermur788/CCF/issues/487) is execution presence only, not file ownership by itself; check live `Docs/Project/coordination/active-tasks.md` but don't edit it (Manager #424 owns it). Register [RUNNING] with actual provider, branch, source HEAD and scratch path **before source edits**.  
**Allowed writes:** `Tools/ContentReview/**`, limited DRAFT notes under `specs/ccf-cross-scenario-text-editor-v1/**` only if essential for schema alignment, plus one new result report `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-N01-TREEMARKS-COMPOSED-COPY-REPAIR-RESULT-20261114.md`. No Unity, `Assets/`, `Packages/`, `ProjectSettings/`, saved-world/PlayerPrefs, production C#, main, approved spec, existing #473/#474/#477/#480 reports, or live coordination changes.

### Technical source and decision

At the original main baseline, `Assets/ForestPrototype/UI/MenuHelpView.Text.cs:19–24` defines **three** actual `TreeMarkEntries` pairs:
- `Orange vertical stripes` — `Rack / machine-path removal`
- `Red` — `Fell`
- `Blue` — `Crop Tree / future timber`

`MenuHelpView.Text.cs:93–99` composes the displayed Tree Marks full-F1 text from literal introductory/concluding fragments, `string.Join` over those pairs, and the **already source-extracted separate** constants `TreeMarksOrangeText` (line 27) and `TreeMarksSimplificationText` (line 28). The current `s1.help.treemarks.explanation.full-F1` catalogue field misleadingly contains `•  — ` and missing pieces; its source comparison is not faithful. The DRAFT S-02 rule explicitly prohibits presenting this as an authoritative editable flat string.

**Preferred bounded repair decision:** add the six independently sourced mark-label/meaning fields with stable new content IDs and exact line provenance; retain the existing two constants, and make the assembled help preview explicitly **derived/read-only** with visible dependency context. Retain separately evidenced literal fragments if needed to represent the composition truthfully. Do **not** ask the owner to edit an inaccurate flattened “Original source.” The typed future Unity adapter's composition is *not* implemented here. If `editable:false` or `kind:"derived"` requires schema support, implement it consistently in `core.js` allowlists/immutable metadata, UI readonly behaviour, JSON import/export, and committed tests. Do not create a mechanism to alter C# runtime conditions.

**Seed revision:** publish an explicit new `catalog.json` revision and exact `catalog.js` parsed-data mirror. Document incompatibility with prior 269-entry exports if entry structure changes; provide a manual reapply recommendation rather than silently rewriting old user drafts.

### Focused acceptance tests

1. Source audit verifies all six entry fragments against `TreeMarkEntries`, existing two constants and the construction statement; no blank `•  — ` is presented as authoritative editable copy.
2. Real Node `tests.mjs` **PASS**, including a new negative for fake editable flattened content and attempts to edit a readonly derived field/import metadata. No weakening of existing 36 groups.
3. Real browser on actual `index.html`: Tree Marks entry fields correctly identify editable source fragments vs derived/composed readonly text; compare and revert remain honest; a validated JSON draft exports/reimports; 1280×720 page sidebar remains clickable; unknown approval keys still reject.
4. Bound scope; no source/Unity/game/saved-progress changes. All residual observations O-1..O-5 remain optional/deferred and are not falsely marked complete.

**Exact return handoff:** `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-N01-TREEMARKS-COMPOSED-COPY-REPAIR-RESULT-20261114.md` on the new feature branch. Report actual tests/commands/version/exits, UI evidence, source lines, before-after and GitHub compare with #477 source. Fetch/read back, comment result on #487, close as [HANDED-OFF] and notify #424. A *short independent N-01 delta check* should follow before treating that specific page as cleared; no need to re-review the already approved F-01/F-02. No automatic Unity integration or Windows build.

## C. Paste-ready dispatch

> Take ownership of CCF [#487](https://github.com/jermur788/CCF/issues/487), a bounded **Tree Marks help source-fidelity** repair for the independently approved standalone Text Editor. Read and follow this complete binding handoff: https://github.com/jermur788/CCF/blob/docs/manager-487-tree-marks-text-editor-n01-dispatch-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-480-APPROVAL-DISPOSITION-AND-487-TREE-MARKS-N01-DISPATCH-20261114.md. Register your actual provider, source HEAD, new isolated branch/worktree and [RUNNING] status before editing. From the pinned #477 approved source, replace the misleading editable composed Tree Marks full-F1 field with source-exact constituent entries and an explicit readonly/derived composition boundary. Preserve the two already separate constants and game-state identities, run real Node and browser tests, publish the exact one-file GitHub result, and close #487 as HANDED-OFF. No Unity/production code, saved game, main, live coordination or unrelated source edits. Do not ask routine questions; report any true blocker.

**User action:** none to acknowledge #480 approval or begin using other parts of the offline editor. To **execute #487**, start a separate equipped code worker and paste the dispatch. Issue creation alone does not start a model.  
**Dashboard/Drive mirrors:** NOT VERIFIED. Canonical GitHub sources and all existing manager/game authority remain unchanged.
