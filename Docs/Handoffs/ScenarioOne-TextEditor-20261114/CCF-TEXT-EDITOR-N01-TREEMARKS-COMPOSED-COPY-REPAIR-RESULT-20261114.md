# #487 result: N-01 Tree Marks composed help repaired in the standalone Text Editor

**Date:** 14 November 2026
**Task:** [#487](https://github.com/jermur788/CCF/issues/487), bounded repair of the #480 MINOR finding N-01.
**Result status:** **HANDED-OFF for Manager #424 disposition and a short independent N-01 delta check.** The repair is implemented and tested by its own author. It is not yet independently reviewed.
**Standing status:** DRAFT / NOT APPROVED · NOT UNITY-INTEGRATED · NOT GAME-VERIFIED · NO USER ACCEPTANCE. Nothing here approves the four-file spec, a Unity importer or any wording.

## 1. Worker, branch and scope

| Item | Value |
|---|---|
| Worker | Anthropic Claude Sonnet 5.5 (Claude Code), new session, single writer. The packet recommended Sol/Codex "or equivalent"; this substitution was recorded at registration ([registration comment](https://github.com/jermur788/CCF/issues/487#issuecomment-6874568610)). |
| Independence | Not the #477 author. **Correction to my registration comment:** this is the *same Claude session that authored the #480 review* (the conversation continued), not a different one. It therefore knows the N-01 finding and the earlier harness, and it is **not an independent checker of its own repair**. The independent N-01 delta check in §10 must be a different reviewer. |
| Source baseline (read-only) | `feature/ccf-text-editor-474-repair-20261114-v1` @ `94b82a0149c549a80324bb6212e038731125651f`; a fresh checkout was verified at exactly this HEAD. |
| Writable branch | `feature/ccf-text-editor-treemarks-n01-20261114-v1`, created from the pin (not from main). It was pushed with an explicit refspec to this branch only. |
| Scratch checkout | `/home/jer/ccf-487-scratch` (WSL ext4, disposable). The user's checkout was not touched. |
| Tested tool commit | `4c527b0cdd27054fc28fb82e7cc72c1ddd127086` (this report is added in a second commit on the same branch). |
| Not touched | Unity, `Assets/`, `Packages/`, `ProjectSettings/`, saves/PlayerPrefs, production C#, `main`, `active-tasks.md`, the #473/#474/#477/#480 reports and branches. |

## 2. Problem and decision

`s1.help.treemarks.explanation.full-F1` was an **editable, flattened** copy whose "original source" contained blank bullets (`•  — `). In C#, `MenuHelpView.Explanation(Menu.TreeMarks)` (`MenuHelpView.Text.cs:93–99`) builds this text from the three `TreeMarkEntries` pairs (lines 21–23), the existing constants `TreeMarksOrangeText` (27) and `TreeMarksSimplificationText` (28), and literal fragments. That conflicted with DRAFT rule S-02.

Implemented the packet's preferred repair:
1. **Nine new editable, source-exact fields** (below), each with its exact C# line.
2. The two existing constants keep their IDs, text and variant (`enlarged80-rack-v2`).
3. The original ID is kept and becomes a **derived, read-only** entry. It has no textarea and no revert button. It lists the 11 editable fields it is built from, in C# order. It **recomposes live from the draft** so an owner can see the effect of an edit. It is never exported as an edit.
4. Fixed C# glue (the `• ` bullet, ` — `, and line breaks) is stored in the derived recipe with its source line and is not editable. This is why editable fields contain no leading or trailing formatting whitespace. The audit proves each field plus its glue equals the C# literal.

| Entry (`s1.help.treemarks` + …) | C# line | Kind | Baseline (start) |
|---|---|---|---|
| `explanationIntro.full-F1` | 93 | editable | "Scenario 1 introduces 3 CCF markings. In this forest…" |
| `TreeMarkEntries0Label.full-F1` / `…0Meaning.full-F1` | 21 | editable | "Orange vertical stripes" / "Rack / machine-path removal" |
| `TreeMarkEntries1Label.full-F1` / `…1Meaning.full-F1` | 22 | editable | "Red" / "Fell" |
| `TreeMarkEntries2Label.full-F1` / `…2Meaning.full-F1` | 23 | editable | "Blue" / "Crop Tree / future timber" |
| `TreeMarksOrangeText.enlarged80-rack-v2` (existing) | 27 | editable | "Orange vertical stripes identify rack entrance trees…" |
| `explanationSprayLine.full-F1` | 95 | editable | "One spray paints vertical stripes on four sides of the entrance tree." |
| `TreeMarksSimplificationText.enlarged80-rack-v2` (existing) | 28 | editable | "The game asks you to mark the rack entrance tree…" |
| `explanationClosing.full-F1` | 97 (spans 97–99) | editable | "After Confirm Rack Plan, walk to a rack entrance…" |
| `explanation.full-F1` (original ID) | 93 | **derived, read-only** | the full 931-character composition |

Choices to review:
- The closing paragraph is **one** field because C# concatenates three adjacent literals (97–99) into one string.
- The two existing constants stay in variant `enlarged80-rack-v2` (their identity is immutable). With the variant filter set to `full-F1` they are hidden, but they remain listed in the derived card's dependency list, and **All variants** shows everything.
- The intro contains C#'s fixed "3 CCF markings". Editing it changes only wording, not the number of marks.

## 3. Before and after (against the #477 pin)

From `git show 94b82a0:…/catalog.json` against the new seed (script output retained locally):
- Revision **2 → 3**. Entries **269 → 278**. Pages 79 → 79, all identical.
- **No ID removed. 9 IDs added.** Exactly **one existing entry changed** (`explanation.full-F1`: `text`, `baselineText`, `note`, new `derived`). The other **268 entries are byte-identical**, and their relative order is preserved. Scenarios, provenance and `packageId` are identical.
- Coverage: `collected` 236 → 245 (+9), one note added; the unmeasured categories are still `null`.
- `catalog.js` equals `JSON.stringify(catalog.json, null, 2)` exactly (also asserted by an existing test).
- **Before** (`baselineText`, abridged): `…itself:\n\n•  — \n\n One spray paints…\n\n\n\nAfter Confirm Rack Plan…`
- **After** (derived baseline, abridged):
  ```
  Scenario 1 introduces 3 CCF markings. In this forest a paint mark records a management decision on the tree itself:
  • Orange vertical stripes — Rack / machine-path removal
  • Red — Fell
  • Blue — Crop Tree / future timber

  Orange vertical stripes identify rack entrance trees … One spray paints vertical stripes on four sides of the entrance tree.
  ```

## 4. Schema and validator change

`entries[].derived` is an **optional, read-only** object (`readOnly`, `composedBy`, ordered `parts`). Each part is either an `entryId` reference or fixed `glue` with its C# `line`. It is inside the closed schema. `core.js` now:
- rejects any unknown key in `derived` or in a part; rejects `readOnly≠true`, an empty `parts`, a part with both or neither of `entryId`/`glue`, a bad `line`, and invisible characters in glue;
- rejects a derived `text` that differs from its `baselineText` (`Derived read-only field was edited`);
- rejects a derived baseline that is not the composition of its constituents' baselines, and a dependency that is missing or itself derived;
- protects `derived` against the bundled seed like every other protected property, so stripping, adding, reordering or altering it is rejected;
- exports `compose()` (pure, no DOM), used by the app preview, the validator and the audit.

No other validator rule was weakened. A new test run **without a seed** exercises these guards on their own, because the seed comparison would otherwise mask them.

## 5. Commands and results (all executed, none inferred)

| Check | Result |
|---|---|
| `node Tools/ContentReview/tests.mjs`, Node v26.13.1 under WSL, full checkout | **exit 0, 48/48 groups** (the original **36 unchanged and passing** + **12 new**) |
| Same file, native Windows Node v26.13.1 (copy of `Tools/ContentReview` plus `MenuHelpView.Text.cs` only) | **exit 0, 48/48** |
| `node Tools/ContentReview/source-audit.mjs .` (full checkout) | exit 0: **28 verified, 0 changed, 0 missing, 88 unmapped, 1 serialized candidate** (same as #480) and **Tree Marks composed copy: VERIFIED (12 entries)** |
| Mutation sanity on `core.js` (8 guards removed one at a time) | **8/8 caught** by the committed tests (a first pass caught only 3/8 because the seed comparison masked the others; I added seed-less tests and re-ran) |
| Audit negative tests (changed mark text, bullet glue, spray wording, dropped glue, a fourth mark pair, closing wording) | each makes the Tree Marks check fail; it fails closed if the statement shape changes |
| Real browser, actual `index.html` via `file://`, Edge 157.0.4341.46 | **1280×720: 41/41 PASS. 1920×1080: 41/41 PASS. One headed 1280×720 run: 41/41 PASS** |

New test groups: derived seed shape and retained ID; nine source-exact constituents and the two constants; derived edit rejected (including edit plus baseline, and stripping metadata); fake editable flattened blank-slot copy rejected; closed/forged derived metadata (17 variants); seed-less structural enforcement; constituent edit stays valid and recomposes, derived untouched, exports reimport; identifiers retained; revision-2 exports rejected; app renders no textarea or revert for derived entries; real-C# audit passes; audit fails closed on drift. The last two audit groups run only when a full checkout is present and are printed `SKIP … NOT RUN` otherwise.

## 6. Browser evidence (real pointer, wheel and key events; hit-tested; real file chooser and downloads)

- **Page sidebar at 1280×720** still works: the first, middle and last of 79 pages were each clicked by real wheel scrolling plus `elementFromPoint` hit testing, and the scenario list is a bounded scroll region.
- **Tree Marks page** has 13 cards: 12 editable textareas whose values equal the catalogue, and 1 derived card (`DERIVED · READ-ONLY`) with 0 inputs, buttons or contenteditable. No textarea contains a blank bullet slot. The preview is character-identical to the C#-composed baseline (931 characters) and its dependency list has 11 entries in C# order.
- **Typing at the derived preview** (click, `insertText`, X/Backspace/Delete/Enter, select-all+Backspace) changes nothing: status stays "No edits", and nothing leaks into any textarea.
- **Edit "Red"→"Red paint" and append to the closing paragraph:** status reads "2 locally edited fields · not exported"; the preview recomposes (`• Red paint — Fell`); two dependencies are flagged EDITED; the nav count is "2 edited" (the derived entry is not counted); the compare panel shows the *source composition* against the *live composition*; the constituent compare shows `Red` → `Red paint`. **Revert field** restores one; **Revert this page** first cancelled (nothing changed), then accepted (everything restored).
- **Export** (real click): the downloaded file validates against the seed, has revision 4, `authoringStatus: draft-local-export`, 278 entries, the same ID order, exactly the 2 changed IDs, the **derived entry byte-identical to the seed's**, and no approval-style keys. **Reimport** through the real chooser restores 2 edits and recomposes the preview.
- **17 hostile or incompatible imports** through the real Import button, at each viewport, all rejected with a visible path-specific error, edits intact, no replace-confirm dialog, no download, no script execution (`window.__pwn` undefined): derived text edited; derived text+baseline flattened; derived stripped to an editable flat blank-slot copy; `readOnly:false`; parts reordered; forged key inside a part; package-level approval keys (B01); entry-level approval keys (B02); altered constituent baseline; **a real 269-entry revision-2 export** and the raw revision-2 seed; `editable:true` on the derived entry; `derived` added to an ordinary entry; HTML in a constituent; a control character; `derived:null`; a dependency removed. An export after all attacks was clean.
- **Variant filter:** `full-F1` shows 11 cards including the derived preview; `enlarged80-rack-v2` shows 3; All shows 13.
- **Search** is unchanged: `sitka` returns **5** pages and `plantation` returns **7**. New fragment text is searchable (`machine-path` finds TreeMarks).
- **Unsaved-edit guard** still fires `beforeunload` for an unexported edit and does not fire after an export.
- **Hygiene:** 0 console errors or exceptions; network is only the five local files.
- Screenshots and logs are retained **reviewer-local only** (the packet allows one new GitHub file): `evidence-n01/*.png`, `evidence-n01-*.log`.

**Harness defects found and fixed before the final runs (disclosed):** my first 1280×720 pass hit (a) a hit-test failure because my scroll helper let a target slide under the sticky 76 px top bar, (b) a wrong regex in one assertion, (c) the sticky-bar buttons being unreachable by my scroll helper, and (d) a confirm-dialog policy left on "cancel" from the hostile phase. None of these was an app defect. I fixed the harness and re-ran every check at both viewports (the numbers above are from those complete runs).

## 7. Acceptance criteria from the packet

| # | Criterion | Result |
|---|---|---|
| 1 | Source audit verifies all six pair fields, the two constants and the construction statement; no blank `•  — ` is presented as editable copy | **PASS** (audit also rejects any editable Tree Marks entry with a blank bullet or dash slot) |
| 2 | Real Node tests pass incl. negatives for fake editable flattened content and readonly/metadata edits; no weakening of the existing 36 | **PASS** (36 unchanged + 12 new, 48/48; mutation-checked) |
| 3 | Real browser: editable fragments vs derived text identified, compare and revert honest, export/reimport, 720p sidebar, approval keys rejected | **PASS** at 1280×720 and 1920×1080 (+ headed) |
| 4 | Bounded scope; no source/Unity/game/save changes; O-1…O-5 not falsely marked complete | **PASS** (see §8 and the compare below) |

## 8. Compatibility, limits and what was not done

- **Revision-2 exports are not import-compatible** (the existing generic wording is shown, e.g. "Entry count changed…", and the open draft is untouched). The README documents a manual reapply recipe by content ID. Nothing is rewritten automatically. Only the old flattened Tree Marks text cannot be reapplied as one string.
- **O-1 … O-5 remain untouched and open**: the coverage box clips about 9 px; other engines untested; two ternary statement-start anchors; extra invisible Unicode still accepted; the old-draft error wording is generic (I deliberately did not improve it, to keep #487 bounded).
- **NOT RUN:** Firefox, macOS (including `Command+S`), a human-eye review, an independent delta check, any Unity, game, save or Windows build step. The browser evidence is one engine (Edge 157) driven by scripted real input.
- The derived preview shows the composition by this tool's recipe. It is **not** the game's output. The typed Unity adapter that would reproduce this in C# is not implemented here, and no runtime or save behaviour changed.
- The test harness is local scratch and is not committed.
- I added one sentence to `specs/…/spec.md` (a DRAFT note describing the optional `derived` object); nothing else under `specs/`.

## 9. Changed paths and verification of the result

GitHub compare of `feature/ccf-text-editor-474-repair-20261114-v1`@`94b82a0` against this branch: the tool commit changes 9 files (`Tools/ContentReview/{README.md,app.js,catalog.js,catalog.json,core.js,source-audit.mjs,styles.css,tests.mjs}` and `specs/ccf-cross-scenario-text-editor-v1/spec.md`, +940/−46), plus this one new report. There are no other paths. The tested files `app.js`, `core.js`, `catalog.json`, `catalog.js`, `index.html` and `styles.css` have the same SHA-256 prefixes in the browser copy and in the commit (`baf6d3b9…`, `c60dd62e…`, `ab050ed5…`, `bc0c51bc…`, `c295b1de…`, `a6a7056e…`).

## 10. For Manager #424

1. Route a **short independent N-01 delta check to a reviewer other than this session** (Tree Marks page only; reuse §5 and §6; no need to repeat F-01/F-02). It should include a human-eye look at the derived card and the first-use impression of the Tree Marks page. The #480 caution to disregard the Tree Marks composite should stay in force until that check is done.
2. Decide separately on any bounded O-1…O-5 follow-up.
3. A typed Unity adapter, a source census, #443 reconciliation and any Windows build remain behind their own gates.

*Dashboard/Drive mirrors: NOT VERIFIED. `active-tasks.md` was read, not edited.*
