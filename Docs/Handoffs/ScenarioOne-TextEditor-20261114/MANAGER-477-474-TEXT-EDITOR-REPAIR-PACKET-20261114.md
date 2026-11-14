# CCF Text Editor — #474 NEEDS CHANGES repair packet (single-writer)

**Prepared:** 14 November 2026 following the user's independent #474 review return.  
**Execution task:** [#477 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/477).  
**Finding authority:** [#474 actual independent Opus 5.5 source/Node/browser review](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md).  
**Original author handoff:** [#473 source prototype and DRAFT](https://github.com/jermur788/CCF/blob/feature/ccf-text-editor-browser-prototype-20261113-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md).  
**Management evidence disposition:** **ACCEPT #474 independent REVIEW EXECUTION/evidence as a genuine separate reviewer and return BOTH scopes NEEDS CHANGES.** This does **not** approve the tool, the DRAFT, player-facing copy, or production integration. Source-proven facts: reviewer claims real Edge 157 CDP tests plus Node 20/20 and source audit; published report/readback and one-added-file review-branch comparison verified from GitHub. Raw screenshots/logs remain local to the reviewer scratch, not archived in GitHub; browser findings are reviewer-supported, **not independently reproduced in this management chat**.

## 1. Purpose and assignment

Fix the two major defects that prevent owner use of the standalone browser editor, address all 11 remaining minor issues, make five bounded clarifications in the four-file **DRAFT** specification, then hand a narrowly scoped source candidate to a **different independent reviewer for focused delta qualification**. No Unity work is needed or authorised.

**Difficulty:** **MEDIUM–HIGH**: small independent app but subtle data-contract, imported object safety, C# source-line provenance, conditional legacy/Rack V2 text, responsive scrolling and actual-browser regression.  
**Best-suited executor:** **OpenAI GPT-6.1 Sol High on local T3 Code / Codex (Node + real Chromium browser)**, or equally capable implementation worker with an actual Git/Node/browser environment. This is a **recommended provider**, not an assertion of assignment. Original #473 author is GPT-6; #474 independent reviewer was Claude Opus 5.5. One new source writer, no simultaneous competing editor worker.  
**Claim:** [#477](https://github.com/jermur788/CCF/issues/477) — **READY / NOT RUNNING** until the real worker registers.  
**Read-only original candidate:** `feature/ccf-text-editor-browser-prototype-20261113-v1` at full pinned `c5461948d2088e27f8e51a2d867a2aff70e243cc`. **Do not modify this branch.**  
**Proposed writable branch:** `feature/ccf-text-editor-474-repair-20261114-v1` created only by the actual worker **from the pinned original candidate**, not latest divergent main. If this named branch already exists or writer ownership changes, stop and coordinate rather than overwrite.  
**Proposed local test lane:** new disposable **isolated scratch checkout** on the worker's available machine, path must be reported accurately after inspection. No worktree is pre-allocated; the Windows scratch path from #474 is reviewer evidence, **not a transferable source worktree**. Do not modify other local checkouts or delete reviewer/user scratch.  
**Permitted writable files:** `Tools/ContentReview/**`, `specs/ccf-cross-scenario-text-editor-v1/spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, plus exactly **one new** result report at `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-474-FINDINGS-REPAIR-RESULT-20261114.md`. Maintain `catalog.json` and `catalog.js` equivalence when changing the seed. Existing #473 research and handoff are read-only historic evidence.  
**Prohibited:** Unity/Editor, `Assets/`, `Packages/`, `ProjectSettings/`, scene/prefab/save/PlayerPrefs/main/Decision Log/Milestone/approved specs, physical Ops, `active-tasks.md`, Objective 1 #443/#450 or forestry mechanics, direct changes to author or independent-review branches. No production JSON importer, text replacement in C#, app server, cloud dependency or Windows Player build.

## 2. Mandatory preflight and worker registration

1. Read `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, live `Docs/Project/coordination/active-tasks.md`, #473/#474/#477, the original author handoff, the **entire #474 report including §5/§6 24-source-line evidence and all findings**, and all four DRAFT spec files.
2. Check current upstream main and source candidate branch and **verify the candidate exactly matches the pinned commit**. #474 independently saw source branch fixed. If it drifted, **STOP and request scope rebase ruling**. Compare the original 16-added-file candidate against its registered main base; record it, rather than relying on chat.
3. Confirm #477 and new repair branch have no actual registered writer; check physical/scratch availability and unrelated dirty work. **Before any production/tool/source edit** post a [RUNNING] registration comment to #477 with actual model/provider, whether effort is observable, exact repair branch, starting HEAD, real scratch/worktree path, exclusive file ownership and **no Unity/physical lease**. Then create the branch from the pinned candidate in an isolated checkout.
4. Treat existing #474 evidence as the negative baseline, not a test suite that has already been re-run. Neither issue claiming nor a read-only source audit authorises messing with the user's machine or worker scratch.

## 3. Repair checklist — all #474 findings required

| #474 finding | Severity | Required source/code change and acceptance |
|---|---|---|
| **F-01** sidebar page list inaccessible at **1280×720** | **MAJOR** | Change `styles.css` layout so scenario navigator cannot consume the entire viewport and page list, coverage and scroll controls can be reached by **real mouse and keyboard**. A scrollable/collapsible scenario section or a vertically scrollable sidebar is acceptable; keep readable density on 1920×1080 and 560 px width; no hidden/unreachable controls. Test 720p first-page and last-page buttons across scroll positions; actual click-target test, not element-exists assertion. |
| **F-02** imported unknown `approved`/`integrated`/approval-looking object keys survive export | **MAJOR** | Implement **closed-schema, recursive allowlists** for top package, entry, scenario, page, `provenance`, entry `source`, and coverage objects. At minimum reject B01 top-level `approved/integrated/approvedBy` and B02 entry `approved/reviewedBy`; reject unknown keys anywhere they could misleadingly claim content authority. Error must point to field path; do **not** simply strip data and silently accept, and do not allow an unknown `approval` field through download. Preserve legitimate export-owned `authoringStatus`/`exportedAt` with defined validation. Explicitly say baseline-equality is integrity against the bundled seed, **not cryptographic proof of approval**. |
| F-03 | MINOR | Enforce **entry array ID order** identical to baseline, rather than checking only entries keyed by ID; swapping 2 fields must reject. Keep page order/gameplay IDs stable. |
| F-04 | MINOR | Reject C1 controls, invisible/zero-width, bidi overrides/isolates and `U+2060–2064`/BOM in edited text. Precisely document allowed newlines/tabs and accepted legitimate Unicode/Irish characters. Add B04/B05 negatives. |
| F-05 | MINOR | Remove repeated scenario label/irrelevant metadata from per-page default search haystack; compare expected `sitka` result against **5** and `plantation` against **7** in pinned seed as reviewer baseline, while retaining wanted page/content field search and edited/original search. If a separate metadata search is desired, expose it explicitly; do not let every result match by ancestor scenario name. |
| F-06 | MINOR | Update active sidebar page's “N edited” label **live while typing** without losing textarea focus/caret or resetting field scroll. Compare display to global edited count after changes and revert. |
| F-07 | MINOR | Add browser `beforeunload` guard **only when unsaved edits exist**, remove when no edits; use standards-compliant prompt (browser controls exact wording) and test reload/cancel vs accept without losing unrelated drafts. Never pretend autosave to browser or Git. |
| F-08 | MINOR | Correct **all 24 inaccurate main-source line references** from review §5 using source-aware C# string/literal offsets and actual pinned files. Do not change baselineText just to fit a cited line, do not fabricate locations. If some entries are composite/generated, mark their real construction source/line transparently. Test every collected main provenance line, not a single example. |
| F-09 | MINOR | Add missing `Walking HUD · your forest at a glance` help `title` from `MenuHelpView.Text.cs`; preserve page/entry identity strategy and draft/seed schema compatibility honestly. |
| F-10 | MINOR | Replace unmeasured `serialized/unlocated/outOfScope = 0` with actual **unknown** representation (`null` or documented token); update coverage schema, source report references where permitted and validation/test logic. This is a controlled new **seed revision**, not an import-compatible mutation of old immutable coverage metadata. Avoid claiming comprehensive source coverage. |
| F-11 | MINOR | Expand committed `tests.mjs` with the negative classes listed in #474 §3.1, at least schema, revision, `authoringStatus`, status, scenarios, page order/scenario, baselineText, entries add/remove, >10k length, **F-02/03/04** and app-level corrupt/oversized imports where achievable. Match the actually committed passing count; stop saying an uncommitted V8 run is a committed Node test. |
| F-12 | MINOR | Pin the actual checked **#443 candidate full commit** to provenance for all three candidate entries; verify copy still matches exact source on that commit. If newer candidate differs, preserve #474-reviewed candidate and record staleness, don't silently replace with current unreviewed user wording. Main source still separately anchored. |
| F-13 | MINOR | Early validator error results must include consistent numeric counts or safe UI defaults; **never** render “undefined entries/pages/edits”. Test malformed JSON/unsupported schema/invalid arrays as UI-level cases. |

**Recorded observations O-01 through O-10:** not release blockers. Choose only small, safe, clearly justified improvements (e.g. browser Alt+arrows should not hijack textarea editing, inspect file size before reading, guard `revision` overflow) and document any deferral. Do not expand into localization, rich-text editing, database tooling or rewriting all forestry text. Reviewer F-10 and F-09 may require careful seed field addition; keep the stable content namespace and explicit version policy. Note #474's evidence was **11 MINOR** (F-03 through F-13), not all 10 observations needing mandatory changes.

## 4. Four-file DRAFT corrections — S-01..S-05

Keep **DRAFT / NOT APPROVED** on every spec file and update cross-references/checklists as necessary:

- **S-01 closed schema and authority:** reject unknown package, entry and nested keys; state a signed/approved package is **not** proved by matching a modifiable seed, review approval is external Git/Manager process only; no automatic production loading.
- **S-02 derived/composed strings:** document examples `ControlsReference.KeysLineFor(bool rackPlanningV2)` using `KeysLine.Replace(...)` and `MenuHelpView` combining TreeMark entries + constants. Any source string that is programmatically derived/concatenated must be either represented by separately reviewable source parts with an explicit dependency, or kept **readonly/derived** until an approved typed adapter exists. No automatic flattened-one-field mapping that changes legacy/V2 semantics.
- **S-03 invisible/control policy:** specific reject/allow Unicode ranges match validator; preserve legitimate typographic Unicode and `\n`/tab.
- **S-04 partial variant package fallback:** define whether approved packages must supply an **atomic family of legacy + V2 variants for each affected page/field**. Proposed safe choice: validate complete coupled variant sets and fall back **atomically to original C# literals/generator for that set** if any mandatory variant missing or invalid. Never mix a new text variant with old derived instructions; no world routing changes.
- **S-05 shared content status vocabulary:** reconcile `main-source`, `unmerged-candidate`, `draft-no-text`, `reviewed-not-integrated`, `no-shipped-text` with a single defined set/meaning for schema, scenario list, entry status, filter and review UI. No invented O2–O8 copy. Expand/update `tasks.md`/checklists to accurately reflect #473 closure and #474 findings.

No other product choices, scenario content, saved-world policy, source-owner lock or Forestry decisions are reopened.

## 5. Verification: proportionate actual-tool and browser qualification

**Stage V0** — verify exact candidate source/branch diff and no Unity files. **Stage V1** — implement F-01/F-02 first with dedicated negative tests, then remainder. **Stage V2** — run all committed Node tests from actual isolated checkout (record version, command, stdout and exit). If source-audit `source-map` is intentionally baseline main, verify 28 paths and known 88 unmapped remain **candidate count**, not coverage completeness. **Stage V3** — real local browser on **1280×720 and 1920×1080**; at least one Chromium or Firefox engine. Run focused matrix:

1. **720p and 1080p navigation:** all scenario buttons and all page buttons reachable/clickable, inner scroll as intended, coverage visible; 560px narrow-screen regression if feasible. Browser hit-testing, not coordinate speculation.
2. **Closed import:** B01/B02 and nested unknown keys reject; after failed import, previous in-memory draft unmodified; after export no unknown approval fields. F-03 entry reorder and F-04 bidi/zero-width/C1 reject.
3. **Search and in-progress editing:** `sitka` only 5, `plantation` 7 on pinned catalogue; edited-only/original search and clearing; sidebar dirty marker updates live without losing caret.
4. **Unsaved guard:** reload/close shows browser-controlled prompt only when edited; cancel preserves; after reverting or validated export assess the documented meaning of “unsaved” (explicitly choose whether export makes draft saved and test the chosen semantics).
5. **Source provenance and missing help title:** 24 offsets corrected using verified source, Walking HUD title, candidate commit pin, coverage unknown representation consistent in `catalog.json` and `catalog.js`.
6. **Error messages and valid workflow regression:** malformed/unsupported schema displays correct counts, unicode/multiline validation, baseline draft round-trip via real chooser/download/reimport, no new console errors or network services; test 3-page intro, Objectives, F1 and variant selection in a **short smoke**, not a re-run of all 79 click paths.

Never weaken failures by changing tests or treating a validation warning as approval. If no real browser/Node is available, report **NOT RUN / BLOCKED** and specify the exact next equipped lane. A single editor repair needs no physical Unity qualification.

**Required evidence:** new committed test cases, actual passing logs/versions, before/after issue matrix F-01…F-13 and S-01…S-05, screenshots or grounded DOM evidence where available, GitHub diff/readback, exact remaining NOT RUN, absence of remote dependencies and Unity work. Keep screenshots/logs in permissible separate scratch if binary publication disallowed, cite filenames and path honestly.

## 6. Durable handoff and independent re-check

Before ending:
1. Write one tracked Markdown handoff **on the repair branch**: `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-474-FINDINGS-REPAIR-RESULT-20261114.md`. It must include actual model/branch/HEAD/checkout, all source changes, F/S disposition table, exact test versions/outputs, profile/save/Unity non-touch statement, unresolved items and reviewer instructions. Fetch/read back it.
2. Compare repair branch to **the pinned author candidate**, not main alone: expected changes only in `Tools/ContentReview/**`, four DRAFT Markdown files and that one new handoff. Do not modify the historical #473/#474 reports or `active-tasks.md`.
3. Post [#477](https://github.com/jermur788/CCF/issues/477) result with direct handoff URL, label **[HANDED-OFF]** and close the execution claim. **Do not self-approve.** Notify #424 of repair and suggested focused independent delta review.
4. If findings are fixed, next manager creates a fresh **independent non-author re-check claim**, ideally **different session/model from both implementation writer and original Claude Opus #474**. Reviewer can work docs-only and run short browser/Node matrix; no Unity. Reviewer verdict after qualification goes to #424. The DRAFT remains DRAFT even if review quality is approved.
5. Only after independent approval and separate #424/#425/user product gate could a narrow Unity adapter be separately scoped; no direct change from this task to production.

**Integration state after this task, even if tests pass:** standalone-tool repair **IMPLEMENTED**, independent verdict **PENDING**; four-file spec **DRAFT**; Unity text adapter **NOT STARTED**; game text/save/Reference **UNCHANGED**. Dashboard/Drive mirrors **NOT VERIFIED**.

## 7. Dispatch and exact user action

**Plain-English purpose:** repair the unusable 720p page navigation, prevent approval-like fields from entering edited JSON, close the other #474 source/UX issues and finish the DRAFT contract, then prepare a small independent re-check.

**Task claim:** [#477 READY/NOT RUNNING](https://github.com/jermur788/CCF/issues/477).  
**Reviewer report source:** [#474 handoff](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md).  
**Writable repair branch:** `feature/ccf-text-editor-474-repair-20261114-v1`, worker to create from pinned original after registration.  
**Allowed worktree:** a new isolated scratch checkout, exact actual path recorded on start; no Unity process/lease.  
**Exact required output:** `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-474-FINDINGS-REPAIR-RESULT-20261114.md`.

**One paste-ready dispatch (to a separate equipped code/browser worker):**

> Take ownership of CCF Text Editor repair issue [#477](https://github.com/jermur788/CCF/issues/477). Follow every step of the complete binding handoff: https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-474-repair-routing-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-477-474-TEXT-EDITOR-REPAIR-PACKET-20261114.md. Register your actual model/provider, worker branch and scratch path on #477 as [RUNNING] before editing. Use a new isolated branch from the pinned original #473 author candidate, not the author's branch. Repair all #474 F-01–F-13 (especially 720p sidebar reachability and rejection of unknown approval flags), revise S-01–S-05 across the four DRAFT files, and keep all game data/Unity/PlayerPrefs untouched. Run real Node and browser regression at 1280×720 and 1920×1080, publish the exact one-file durable repair result, read it back, compare changed paths, close #477 as HANDED-OFF and notify #424. Continue without routine questions; do not claim an independent PASS or implement Unity integration.

**Exact user action required to execute:** paste that dispatch into a **different local T3 Code / Sol High worker chat with real Node and browser access**. This packet and issue do not themselves launch a model or modify application code.

**No user action** required for publication/triage performed in this management turn.
