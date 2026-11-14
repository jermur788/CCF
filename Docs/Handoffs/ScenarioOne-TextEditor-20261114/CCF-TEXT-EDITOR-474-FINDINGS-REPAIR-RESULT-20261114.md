# CCF Text Editor #477 — #474 findings repaired in standalone source and four-file DRAFT

**Status:** SOURCE REPAIR HANDED OFF FOR INDEPENDENT DELTA REVIEW; **NOT SELF-APPROVED**, **NOT UNITY-INTEGRATED**, **NOT GAME-VERIFIED**, **SPEC STILL DRAFT**, **NO USER CONTENT ACCEPTANCE**.  
**Date:** 14 November 2026.  
**Task claim:** [#477](https://github.com/jermur788/CCF/issues/477), based on the [binding repair packet](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-474-repair-routing-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-477-474-TEXT-EDITOR-REPAIR-PACKET-20261114.md).  
**Review findings:** [#474, independent Claude Opus 5.5 real Edge 157/Node run](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md) — **tool NEEDS CHANGES**, **DRAFT quality NEEDS CHANGES**, originally 2 MAJOR + 11 MINOR + 5 S findings.  
**Actual writer:** OpenAI ChatGPT **GPT-6**, independently registered on #477 before any write; provider/effort setting beyond model not attested. Not #474 reviewer.  
**Branch:** `feature/ccf-text-editor-474-repair-20261114-v1`, created from and compared against **pinned original #473 author candidate** `c5461948d2088e27f8e51a2d867a2aff70e243cc`, not main; original author branch not touched.  
**Scratch:** `/mnt/data/ccf-477-scratch` local isolated layout fixture only; no full CCF checkout or existing user worktree. **No Unity/Editor, physical machine lease, `Assets`, `Packages`, `ProjectSettings`, save, PlayerPrefs, live coordination or main change**.

## Executive result

All 13 #474 findings have a source-level repair in the isolated browser tool, source seed or committed tests; the DRAFT has explicit S-01…S-05 amendments. In particular, a 720p page-list scroll fix was written and independently mouse-clicked with a **matching static sidebar-layout fixture** in Chromium; JSON imports now fail closed on unknown approval-looking keys at every object level and on reordered fields. **These are source-implementation results awaiting a real full-app browser test**. The editor has not been integrated into the game.

The original 79-page starter seed now contains **269 fields** (one new source-backed Walking HUD help title), still only current Scenario One main/candidate source and explicitly empty later chapters. No text claims to represent every runtime screen. Existing old exported JSON drafts with 268 records are intentionally incompatible with revised seed revision 2 until manual three-way reconciliation.

## Exact changed file scope

Compared with pinned original author candidate, before this new report **11 modified files** and **zero changes outside allowed source/DRAFT paths**:
- `Tools/ContentReview/core.js`: closed recursive schema, immutable entry order, Unicode/control character checks, stable early error counts and bounded export revision.
- `Tools/ContentReview/app.js`: real text-only search instead of repeating scenario ancestry, live page edited indicator, native beforeunload for changed-since-export/import text, numeric early-error display, byte-length preflight for import and non-hijacked Alt+arrows in editing fields.
- `Tools/ContentReview/styles.css`: bounded scroll areas for scenario list, page list and coverage, with narrow-width fallback.
- `Tools/ContentReview/catalog.json` and `catalog.js`: byte-equivalent parsed source package revision 2, corrected source lines, added missing Walking HUD help title, candidate commit pin, null unmeasured coverage.
- `Tools/ContentReview/tests.mjs`: **16 additional committed checks** beyond original 20 (36 total), covering #474 adversarial classes and source integrity.
- `Tools/ContentReview/README.md`: safe authoring behavior, seed migration, derived-string boundary and explicit browser/approval limitations.
- Four and only four `specs/ccf-cross-scenario-text-editor-v1/` DRAFT files: bounded S-01…S-05 addenda and review/verification/ownership gates.

This new Markdown handoff is the **twelfth** changed path. `source-map.json` remains a read-only 28-file main blob anchor; no unrelated file is modified.

## Detailed #474 disposition

| Finding | Implementation / evidence | Remaining gate |
|---|---|---|
| **F-01 MAJOR** 720p sidebar impossible | `styles.css` now caps the scenario list and independently scrolls scenarios/pages, retains a coverage region and a 130px page-list floor. Chromium **144.0.7559.96** via Playwright `set_content` with a **matching static sidebar fixture**: at **1280×720**, page area 321px and scrollable, coverage top 657px, first/middle/last synthetic page buttons 1/40/79 mouse-clicked; **1920×1080** 661px scrollable, first/middle/last clicked; **560×900** page list 330px, all clicked. | **The actual full 269-field app was NOT browser-tested** in this environment; isolated Chromium rejects both `file://` and localhost `Page.goto` with `ERR_BLOCKED_BY_ADMINISTRATOR`. A separate browser-equipped reviewer must repeat with actual `index.html`. |
| **F-02 MAJOR** fake approval flags survive import/export | Closed-schema allowlists on package/provenance/scenarios/pages/entries/source/coverage/classification nested objects; unknown fields error with field path. B01 `approved/integrated/approvedBy`, B02 `approved/reviewedBy`, and nested metadata mutations all rejected by directly executed V8 contract negatives. Draft export remains `draft-local-export`, `reviewed=false`, `unityIntegrated=false`. | Re-run actual UI import/download with malicious JSON in independent browser. |
| F-03 unordered entries accepted | Exact `entries[].id` sequence compared to baseline; B03 swap rejection directly tested. | Browser reimport negative after export. |
| F-04 invisible/bidi/C1 accepted | Reject C1, zero-width, soft hyphen, bidi controls, `U+2060–U+206F`, BOM; legitimate Irish Unicode/arrow/newline/tab preserved in direct V8 tests; spec aligned. | Real UI negative fixture. |
| F-05 scenario-name search matches all 79 | Default search now indexes page title, section and entry text/baseline only. Applying the exact expression to current 79-page checked-in catalogue returns `sitka` **5**, `plantation` **7**, `regeneration` **10**, matching #474 expected counts. | Live browser search and result navigation. |
| F-06 edited-count sidebar stale | Field input directly updates selected page's `small` text without rebuilding the sidebar or textarea; count and comparison update live. | Browser caret/focus smoke. |
| F-07 no unsaved warning | Native `beforeunload` guard only when in-memory text differs from last **successful export or import** snapshot; successful download/import updates saved fingerprint. No autosave, no custom browser dialog wording. | Actual browser prompt/cancel/accept (not attested). |
| F-08 24 incorrect source-line citations | Literal-aware source scan against seven exact pinned main C# files re-anchored **57 per-field citations** (including all unambiguous off-by-many/help, per-lesson field locations and generated Work Plan/Annual lines). The prior 24 reported faulty citations are included. Three ambiguous repeated controls key literal references were **left unchanged** because their recorded lines genuinely contain the same literal; no fabricated offsets. Source strings themselves are unchanged. | Independent reader should spot-check corrected C# line mapping; no Unity run needed. |
| F-09 missing Walking HUD help title | Added literal `Walking HUD · your forest at a glance` from `MenuHelpView.Text.cs:46`, stable new content ID under the existing help page. Seed now 269 fields. | Browser help page display. |
| F-10 unmeasured coverage stored as zeros | `serialized`, `unlocated`, `outOfScope` now **null/unknown**. Counted `collected` increments by one for real title. Full player-facing coverage still UNKNOWN. | Source inventory expansion is a separate later job. |
| F-11 committed suite missing guards | Added 16 source/identity/security test groups to `tests.mjs`, total **36 check blocks**, including B01–B05, revisions, statuses, source/schema/profile mutation, entry adds/removes, >10k size and Unicode. Directly executed published test body in V8 using shims for Node I/O: **36/36 PASS**. | **Actual `node Tools/ContentReview/tests.mjs` NOT RUN** because this environment has no local CCF checkout/files transferred from GitHub connector. Node v22.16.0 is present; follow-up reviewer must run exact repo script. |
| F-12 unpinned #443 candidate source | Exact observed #443 HEAD `acb4e1111fdc9aa9b295413038eda4e03fe56402` added to package provenance and each of three candidate `source.commit` records. Read-only fetched candidate source still contains all three copy snippets. | Follow-up integration must check whether candidate moved before choosing canonical wording. |
| F-13 undefined counts on early errors | `core.validate` returns numeric entry/page/changed counts on all early errors; UI also uses numeric fallbacks. Tested unsupported version, missing/invalid arrays and invalid root object. | Actual browser validation message smoke. |

**Additional tiny safe observations:** prevent browser Alt+arrow pagination from hijacking editable text inputs; preflight `file.size` before reading big JSON; reject unsafe revision overflow; align stale DRAFT task rows in an addendum. The rest of #474 O-01…O-10 are not made blocking feature changes. No rich-text/localization/framework expansion.

## S-01…S-05 four-file DRAFT clarifications

- **S-01:** recursively reject unknown approval-like package/entry/nested fields; entry IDs/order immutable; seed equality not cryptographic Manager approval.
- **S-02:** source-derived/concatenated strings (`ControlsReference.KeysLineFor` `.Replace()`, `MenuHelpView` Tree Marks assembled from parts) need separately approved components/dependency or readonly/derived status; never automatically flatten into one writable C# replacement.
- **S-03:** exact Unicode/C0/C1/bidi/zero-width allowance and rejection ranges aligned with code; `\n`/tab/Irish Unicode remains usable.
- **S-04:** partial legacy/Enlarged80 V2 approved variant families fail **atomically** to existing original C# source, not mismatched mixed copy; profile selection remains actual game state only.
- **S-05:** shared `main-source`, `unmerged-candidate`, `no-shipped-text`, `draft-no-text`, `reviewed-not-integrated` authoring/status meanings; source status is not a game-acceptance flag.

All four documents **still start with DRAFT / NOT APPROVED**. #424/#425/user retain product/spec authority, not this source worker. No Unity adapter was implemented.

## Tests and environment limitations

**Direct PASS:**
1. Connector reads of published `core.js`, `app.js`, `catalog.json`, `catalog.js` and committed `tests.mjs`: JS syntax/seed checks; mirror parsed equality, original source schema accepted; 269 entries, 79 pages and three candidate entries.
2. **36/36 committed check blocks executed under JavaScript V8** with Node I/O APIs shimmed (not a Node process/CI). Includes unknown top/nested approval fields, B03 entry reordering, C1/bidi controls, source/provenance and export restrictions. No test was weakened to obtain green.
3. Default source-backed search exact result counts: `sitka` 5, `plantation` 7, `regeneration` 10.
4. Real isolated **Chromium 144** with Playwright **`page.set_content` layout fixture only**, pointer-click tests at 1280×720, 1920×1080 and 560×900. Screenshots/logs not claimed as production app evidence; fixture at `/mnt/data/ccf-477-scratch/layout-fixture.html` not a CCF repository artifact.

**NOT RUN / NOT CLAIMED:**
- Actual `node Tests/ContentReview/tests.mjs` (correct command is `node Tools/ContentReview/tests.mjs`); full CCF checkout unavailable. `source-audit.mjs` likewise NOT RUN here; #474 previously executed it successfully on its own pinned checkout.
- Actual full `index.html` browser file/open/download/import/close flow: navigation denied by container administrator (both file:// and localhost); test is a **blocker for full browser qualification**, not a failed application UI assertion. Do not label F-01 independently PASS for actual UI from the fixture.
- Firefox, Mac-specific shortcuts, Unity Editor/compiler, Legacy40/Reference runtime/saves, PlayerPrefs, Windows build, copied text displayed in the game.
- User or independent reviewer acceptance. Raw #474 screenshots/logs remain only in the prior reviewer's own scratch; no claim of access to those bytes.

## Exact next step — independent focused delta re-check

**Required:** one **different non-author** Node/browser-capable reviewer on a NEW claimed docs-only report branch. Compare this repair branch against original author candidate. Actually run `node Tools/ContentReview/tests.mjs`, `source-audit.mjs . --json` if a full checkout is available, and actual browser `Tools/ContentReview/index.html` via `file://` or local-only HTTP. At 1280×720 and 1920×1080, confirm sidebar first/middle/last button pointer accessibility, coverage, status/search results, dirty marker/caret, unsaved prompt, JSON export/reimport and B01/B02/B03/B04/B05 rejection; verify all 13 F and five S in a small delta review, **not a repeat of all 79 full-page journeys or heavy Unity tests**.

Verdicts must be **APPROVED / NEEDS CHANGES / REJECTED** separately for **standalone tool** and **DRAFT spec quality**. A positive review still leaves the DRAFT unapproved by the user and the game untouched. If findings remain, create one bounded fix packet; no automatic Unity source edits.

**Dashboard:** NOT VERIFIED. **Drive:** NOT VERIFIED. **User action:** none for #477 writer handoff; launching an independent reviewer session is a separate action unless a connected orchestrator actually starts it.

**Claim admin:** publish/readback this report, compare path allowlist, mark #477 `[HANDED-OFF]`/close, notify #424, and prepare a next reviewer task when warranted. No merge to main.
