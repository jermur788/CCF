# CCF Text Editor — independent browser/Node test and DRAFT-spec review dispatch

**Issued:** 14 November 2026 in response to the product owner's direct instruction: **commission an independent reviewer to test the CCF Text Editor**.  
**Execution task:** [#474 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/474). No worker is registered yet.  
**Author's completed task:** [#473 — HANDED-OFF / CLOSED](https://github.com/jermur788/CCF/issues/473).  
**Difficulty:** **MEDIUM–HIGH** — independent real-browser interaction, import/approval safety, stable content identifiers, conditional O1 profile and source coverage accuracy.  
**Preferred reviewer:** **Claude Opus 5.5 High or equivalent genuinely independent browser/Node-capable developer/reviewer** in a *different chat/session from the OpenAI GPT-6 author*. A T3 Code/Claude Ultra Code local environment is suitable if it has an actual browser and Node. The model/provider is a recommendation, **not** an assertion that it is running.  
**Commissioner / product authority:** direct user instruction; Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424) owns any later review disposition, production integration and live coordination. Overall Manager #425 owns cross-scenario product policy.

## 1. Mission and evidence boundary

**Actually test** the #473 standalone offline browser app and review the four-file DRAFT spec. Distinguish (1) browser tool functional/correct/safe, (2) spec sufficiently safe to propose approval, (3) source/coverage limits. Report **APPROVED / NEEDS CHANGES / REJECTED** separately for **standalone browser prototype** and **four-file DRAFT quality**. DRAFT review approval is **not approval of production integration or game copy**.

Author's report asserts **30 direct V8 contract checks passed**, 79 navigable pages, 268 source-extracted fields (265 main, 3 candidate), 28 C# paths mapped, 2,934 quoted-string lexical *candidates* and 225 interpolation starts. It explicitly did **not** run the Node script or an actual browser. This review must close those actual-test gaps rather than merely repeating the author’s V8 evidence. If browsers/Node are unavailable, report a precise **BLOCKED / NOT RUN** result and next executable test lane; do not award a browser PASS.

The editor is outside Unity. A browser JSON export is not a Unity content import, source commit, accepted copy, production build or save mutation. No Unity/source integration, implementation authority or user acceptance is granted here.

## 2. Immutable read-only candidate and preflight

**Pinned author candidate:** `feature/ccf-text-editor-browser-prototype-20261113-v1` at complete Git HEAD:

`c5461948d2088e27f8e51a2d867a2aff70e243cc`

**Author's full durable result (READ FIRST):**

https://github.com/jermur788/CCF/blob/feature/ccf-text-editor-browser-prototype-20261113-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md

**Read-only source files:**
- `Tools/ContentReview/index.html`, `styles.css`, `app.js`, `core.js`;
- `Tools/ContentReview/catalog.json`, `catalog.js`, `source-map.json`, `source-audit.mjs`, `tests.mjs`, `README.md`;
- `Docs/Research/ContentReview/CCF-PLAYER-FACING-TEXT-INVENTORY-AND-ARCHITECTURE-20261113.md`;
- the four `specs/ccf-cross-scenario-text-editor-v1/` DRAFT files;
- `#473` final report and GitHub comparison to the registered main baseline `fc18190b976f1363f1b5d1b0881810db2f8c6e39`.

**Before any writable report:**
1. Read root `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, the live `Docs/Project/coordination/active-tasks.md`, relevant current product authority, [#474](https://github.com/jermur788/CCF/issues/474), [#473](https://github.com/jermur788/CCF/issues/473), [#424](https://github.com/jermur788/CCF/issues/424), and this packet. Refresh current main HEAD and author candidate HEAD; if the latter moved, **STOP and ask #424 whether to retarget**. Do not silently review a moving target.
2. Ensure no other actual worker has claimed #474 or the designated reviewer branch. **Register actual model/provider (and reasoning setting only if observable), claimed reviewer branch, starting HEAD, any local scratch path, clear no-Unity/no-physical-lease status, and exclusive report-file ownership on #474 as [RUNNING] BEFORE file edits.** `active-tasks.md` remains Manager #424's sole-writer authority; reviewer only reads it.
3. **Reviewer GitHub branch:** create `docs/ccf-text-editor-independent-review-20261114-v1` off freshly verified **main**, **not** the author's feature branch. This is a new docs-only report branch with exactly one permitted added report. If it exists or is occupied, stop/coordinate, do not overwrite it. **Writable path only:** `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md`. The reviewer does not edit tool code or the DRAFT spec; failures become findings.
4. Local tests may use a **fresh temporary isolated scratch directory only** after actually checking environment availability and identifying it in the task claim. **No project-wide CCF Ops worktree, Unity/Editor/board lease, `Assets/` edits, saves, PlayerPrefs, user working checkout, library/cache purge, cache clean, destructive Git commands or Windows build.** Leave the author branch fully read-only. Avoid installing dependencies into the repo; any browser automation test tooling belongs in temporary scratch only. If no local tools, source-only review and explicit NOT RUN may proceed, but **not** a full PASS.
5. Verify the reviewed candidate has exactly 16 added allowed-path files vs its original main base, no production C# or Unity scene changes. Record actual compare result, not inferred summary.

## 3. Required real execution — Node and browser

These are focused Tier-1 standalone-tool checks, **not** heavy Unity qualification.

### A. Node / data-contract checks (must actually run)

From a fully available isolated checkout **pinned to the exact author candidate**, run and record Node version, stdout, stderr and exit code:

```sh
node --version
node Tools/ContentReview/tests.mjs
```

Verify baseline schema, local `catalog.js` mirror of `catalog.json`, Unicode and multiline preservation, source/candidate separation, changed-copy roundtrip, token negatives and immutable metadata. Independently inspect `core.js`, not just reported test output. Check whether the author’s 30 direct V8 checks are adequately represented in the committed Node suite; note any gaps.

**Adversarial import cases:** duplicate or missing content ID, changed `pageId`/scenario/order/lesson ID, edited `baselineText`, modified source branch/commit/path, tampered approval/integrated flags, field note metadata, extra/missing/reordered or malformed `{{tokens}}`, blank required text, unsupported schema/revision, control character, HTML/script-like payload, corrupt JSON and oversized (>3 MB) input. Expect rejection with legible error and no loss of the working draft. Report any bypass precisely with reproducible input, not a general claim.

### B. Real browser smoke — mandatory for tool-quality PASS

Open the *actual* `Tools/ContentReview/index.html` using **file://** in a local Chromium/Chrome or Firefox browser (one engine required; both preferred), or use optional local-only HTTP if browser rules block file scripts. Record browser version, launch method, console errors/warnings, working screenshots or observed text, and whether `catalog.js` loads without a network dependency.

**Required run matrix:**

| ID | Browser action | Observable pass criterion |
|---|---|---|
| UI-01 | Launch locally | Shows scenario sidebar, Scenario One and the first real introduction page; no fatal console error, blank screen or remote network dependency |
| UI-02 | Navigate three intro pages | Titles, paragraphs, process/cards and footer text are present and in order; Previous/Next keeps consistent page navigation |
| UI-03 | Browse Objectives and lessons | Nine topics, 48 protected step records, eight teaching lessons; no lesson/save key can be typed into the field editor |
| UI-04 | Inspect F1/first-use and Controls | Core help/controls accessible with correct variants; `{{keysLine}}` is labelled as token, not an invented runtime value |
| UI-05 | Search all text | Search matches edited and original wording, including when navigated across page/section; clearing search restores normal page list |
| UI-06 | Conditional variant controls | Main `legacy-or-fallback` vs `enlarged80-rack-v2` remain distinguishable; no candidate text silently appears as main |
| UI-07 | Edit and diff | Change one title and multi-line body; original remains unchanged; edited indicator, unsaved count and side-by-side comparison update |
| UI-08 | Revert | Per-field and per-page revert work, without reverting unrelated pages; confirmation appears for page reset |
| UI-09 | Valid export and reimport | Export `draft-local-export` JSON, reload the browser, import file, verify edited text and source metadata exactly preserved |
| UI-10 | Unsafe/invalid import | Attempt malicious/corrupt/modified-provenance JSON; no script executes, import rejects, existing unsaved work remains |
| UI-11 | Unicode and format | Irish diacritics, apostrophe, `&`, arrows, smart quotes, literal backslash, line breaks, typed placeholders survive |
| UI-12 | Accessibility/layout | Tab/focus order, labelled textarea, keyboard Alt+arrows, Ctrl/Command+S, narrow width, reduced-motion; flag actual defects |
| UI-13 | Status separation | Main source, unmerged #443 candidate, compact draft and future scenarios clearly labelled; O2–O8 have **NO SHIPPED TEXT YET**, no invented curriculum |
| UI-14 | Failure handling | Oversized strings/token errors show clear validation and block export; invalid import never changes the prior in-memory data |
| UI-15 | Practical page/editor use | Test dense Objectives/help page, long text field scrolling, previous/next around last page, status filter and page count consistency |

Measure useful UX at representative **1280×720 and 1920×1080 browser viewport sizes** where feasible. These are **browser/editor usability checks**, not Unity rendered qualification. Include screenshot links or exact capture names if images are actually saved.

If automated browser tooling exists, script the journey and retain meaningful selectors/observed results. If manual browser is available, record manual outcomes. Do not invent screenshots, Chrome version, “click passed” or network isolation proof from reading JavaScript.

### C. Source audit and coverage honesty

Run, if a real full pinned checkout is present, and record command/exit:

```sh
node Tools/ContentReview/source-audit.mjs . --json
```

Inspect whether `source-map.json` 28 Git blob anchors correspond to the checked candidate’s main-derived C# source, whether scan identifies unmapped files or serialized UI candidates, and whether test script falsely treats a partial C# scan as total UI coverage. Missing full checkout = **NOT RUN**, not “no missing strings.”

Independently spot-check actual C#:
- intro and production copy;
- `LearningObjectivesView.Curriculum.cs` including both formerly invalid `rack.create` / `rack.map` V2 variants and protected Step IDs;
- `ScenarioOneTeachingSequence.cs`, `MenuHelpView.Text.cs`, `ControlsReference.cs`;
- `AnnualReviewView.cs` / `WorkPlanView.cs` generated/conditional titles and annual amounts;
- separately tagged Objective 1 candidate wording where possible.

Identify errors in copied text, wrong source-line provenance, missing default variants, duplicate content identities, projected token mistakes, or false claim of “every text”. **Do not demand a full source census as a condition for a truthful partial prototype PASS; do require gaps remain explicit.**

## 4. Four-file DRAFT-spec review

Read all four `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` independently. Check:

- Does the proposed schema distinguish editable display strings from immutable game progress, `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, `ScenarioOneTeachingSequence` keys, rack/tree/order/save identities, historical Reference and persistent flags?
- Are source branch, revision/provenance, candidate vs shipped, and content approval semantics genuinely unforgeable by a downloaded draft? Identify exact validator limits rather than relying on a stamp.
- Are placeholder contracts safe: Unicode, multiline, reordered token, named numbers/units, money formatting, escaping, conditional/plural scope and `textContent`?
- Do initial Unity adapter call sites and fallback semantics preserve actual Legacy40, Enlarged80/Rack V2, historical Reference, previous lesson progress, first-use F1 and Work Plan approval? Is runtime vs Editor import lifecycle clear? No C# is authorised.
- Does the accepted eight-case teaching order keep original case IDs independently of UI chapter order? Is compact O1 only DRAFT with no fake copy or save identity?
- Are source completeness, serialized UI/art-baked text, generated annual messages, and unmerged #443 scope honestly left as gaps? Are all four files DRAFT, even if reviewer approves their quality?
- Are ownership, no-Unity and Windows build gates, non-author review, 720p/1080p future Unity tests and user acceptance kept distinct?

**Do not settle OPEN product questions or promote DRAFT to APPROVED.** #424/#425/user hold that authority.

## 5. Findings, severity, verdict and closure contract

Write **one new GitHub Markdown reviewer report**, exact path:

`Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md`

on independent `docs/ccf-text-editor-independent-review-20261114-v1`. Include:
1. Actual reviewer identity/provider/claimed branch/starting HEAD and exactly pinned read-only author candidate; exact local scratch path if used.
2. Node/browser/CLI versions, run commands and exit codes; table of **UI-01..UI-15** PASS/FAIL/NOT RUN and supporting evidence. Direct screenshots/logs only if real.
3. The separate source coverage/safety/spec assessment; full file list and 16-added-file comparison.
4. Numbered findings labelled **BLOCKING / MAJOR / MINOR / OBSERVATION**, each with file/line/steps-to-reproduce, observed vs expected and fix owner. Severity must be proportionate; do not call every incomplete future text category a prototype defect.
5. Explicit independent **standalone-tool verdict** and **four-file-DRAFT-quality verdict**, each **APPROVED / NEEDS CHANGES / REJECTED**, and meaning of any required corrective actions. A missing actual browser run cannot be “APPROVED as browser-tested.”
6. Honest statuses **NOT UNITY-INTEGRATED; NOT GAME-VERIFIED; DRAFT NOT PRODUCT-APPROVED; no user acceptance**.
7. Exact next claim if repairs needed, with bounds and one writer; if approved, propose one clearly gated future coverage/adapter stage for #424, without starting Unity.
8. Dashboard/Drive verification status; do not claim mirror sync from GitHub report publication.

**Before finishing:** read report back from GitHub and compare independent reviewer branch vs registered main starting point; expected **one new Markdown only**. Post direct report URL and executable result to #474, mark [HANDED-OFF], close the execution claim and notify #424. Review completion = executed/handed off, **not** integration/feature acceptance.

**STOP rather than override:** existing claimed reviewer branch/issue, protected local user checkout, source mismatch, real Node/browser unavailability for requested proof, game-data/save/PlayerPrefs changes or any pressure to mutate author code while reviewing. If blocked, publish factual partial review with blockers and next test environment; do not silently downgrade tests to make green.

## 6. Exact routing and user action

**Plain-English purpose:** confirm whether the new outside-Unity text editor really works in a browser, safely handles import/export and variants, and whether its DRAFT specification is sound enough for later approval.

**Owner:** independent non-author reviewer under #474, no local Unity ownership, no standing source lock.  
**Actual review branch:** `docs/ccf-text-editor-independent-review-20261114-v1` (reviewer must create only after preflight).  
**Expected initial worktree:** none. Temporary local scratch only after reviewer reports exact path and verifies no collision; source remains read-only.  
**Final handoff path:** `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md`.  
**No Unity, production-source edits, new save, or main merge.**

**User action required:** Open a **different independent Claude Opus High/T3 Code chat with actual Node/browser tooling** and paste the line below. This creates a reviewer *execution session*. Publishing an issue alone cannot run another model automatically.

### Paste-ready dispatch to separate reviewer

> Take ownership of CCF independent Text Editor test task [#474](https://github.com/jermur788/CCF/issues/474). You are the **independent non-author reviewer**, not the GPT-6 #473 writer. Read and follow **every step of this binding GitHub handoff**: https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-dispatch-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-CCF-TEXT-EDITOR-474-INDEPENDENT-BROWSER-AND-SPEC-REVIEW-DISPATCH-20261114.md. Register your actual model/provider, new isolated docs branch, starting HEAD, scratch path (if any), no Unity lease and [RUNNING] status before edits. Independently run the Node checks, **actually open and test the HTML editor in a browser**, test JSON export/reimport and malicious imports, inspect source coverage and all four DRAFT spec files. Write the exact one-file GitHub report, issue separate APPROVED / NEEDS CHANGES / REJECTED verdicts for browser tool and DRAFT quality, read back the report, close #474 as HANDED-OFF and notify #424. No Unity production code, saved games, live coordination or author-branch edits. Continue without routine questions; if an execution tool is absent, report it explicitly rather than claiming browser PASS.

**Dashboard and Drive mirrors:** **NOT VERIFIED**; this packet is a new docs-only dispatch, not accepted canonical product context.
