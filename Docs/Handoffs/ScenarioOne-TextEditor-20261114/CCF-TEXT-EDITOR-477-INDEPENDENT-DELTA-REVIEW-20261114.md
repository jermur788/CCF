# CCF Text Editor — independent delta review of the #477 repair (#480)

**Task:** [#480](https://github.com/jermur788/CCF/issues/480), independent non-author delta review of the [#477](https://github.com/jermur788/CCF/issues/477) repair of the standalone CCF Text Editor and its four-file DRAFT.
**Binding packet:** [MANAGER-480-CCF-TEXT-EDITOR-477-DELTA-REVIEW-DISPATCH-20261114.md](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-477-delta-review-dispatch-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-480-CCF-TEXT-EDITOR-477-DELTA-REVIEW-DISPATCH-20261114.md)
**Previous review:** [#474 report](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md) (2 MAJOR, 11 MINOR, 5 DRAFT clarifications). **Repair result:** [#477 handoff](https://github.com/jermur788/CCF/blob/feature/ccf-text-editor-474-repair-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-474-FINDINGS-REPAIR-RESULT-20261114.md).
**Date:** 14 November 2026. **Routing:** Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424) owns the disposition.

> **Status labels (all apply):** **NOT UNITY-INTEGRATED · NOT GAME-VERIFIED · DRAFT NOT PRODUCT-APPROVED · NO USER ACCEPTANCE.** This review covers a standalone browser tool that runs outside Unity and four DRAFT documents. A JSON export is never accepted game copy, a Unity import, a source commit or a save change.

---

## 0. Verdicts (two separate, independent verdicts)

| Scope | Verdict | Why, in one line |
|---|---|---|
| **(a) Standalone browser tool** (`Tools/ContentReview/**`) for **owner browser use** | **APPROVED** (owner use of the standalone tool only; one MINOR and several observations carried as non-blocking) | The real `index.html` was driven in a real Chromium (Edge 157) at 1280×720 and 1920×1080 (plus 560 px) with real pointer events. Both MAJORs (F-01, F-02) are fixed, and F-03…F-13 each pass in the actual app and/or committed Node tests. No console error, no network request beyond the five local files, no injection path. One new MINOR (N-01, Tree Marks help seed entry) is a content-fidelity gap, not a behaviour defect. |
| **(b) Four-file DRAFT quality** (`specs/ccf-cross-scenario-text-editor-v1/**`) | **APPROVED** as DRAFT *quality* only (bounded, no rework) | S-01…S-05 are present, internally consistent, and every factual claim I could check against the code and the C# is accurate (the S-03 Unicode list matches the validator exactly). All four files still open with DRAFT / NOT APPROVED. |

**A positive DRAFT-quality verdict does not approve the feature specification for Unity, does not promote any DRAFT content, and does not replace #424 / #425 / user approval.** Neither verdict approves production integration, game copy, a Unity adapter, saves or a build.

---

## 1. Reviewer identity, lane and pinned source

| Item | Value |
|---|---|
| Actual reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code harness in T3 Code, Windows 11 host with WSL2 Ubuntu. Reasoning effort is not observable beyond the harness default. A separate new session: **not** the #477 author (OpenAI GPT-6) and **not** the #474 reviewer session (Claude Opus 5.5). The packet prefers Opus High; this is a recorded substitution, not a false registration. |
| Registration | [#480 [RUNNING] comment](https://github.com/jermur788/CCF/issues/480#issuecomment-6869233217), posted **before** any branch, file or other GitHub write |
| Reviewer branch | `docs/ccf-text-editor-477-delta-review-20261114-v1`, created from `main` @ `fc18190b976f1363f1b5d1b0881810db2f8c6e39` after re-verifying main immediately before creation |
| Only file written to GitHub | `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-477-INDEPENDENT-DELTA-REVIEW-20261114.md` (this report) |
| Pinned repair candidate (read-only) | `feature/ccf-text-editor-474-repair-20261114-v1` @ **`94b82a0149c549a80324bb6212e038731125651f`**. Verified at start and again before publishing. |
| Comparison base (read-only) | original #473 author candidate `c5461948d2088e27f8e51a2d867a2aff70e243cc`. The author branch ref still equals it (`compare` identical). |
| Local scratch (disposable) | `C:\Users\hp\AppData\Local\Temp\claude\7ac52aaebe79051c\b63bef9b-0f63-44ea-8d8a-28d9972059b1\scratchpad` (browser copy under `pinned\`, harness under `harness\`, evidence under `evidence-node\` and `evidence-browser\`). The full checkout for Node and the audit is a separate shallow clone on the WSL native disk, `~/ccf-480-scratch/src-94b82a0`, at the pinned HEAD with a clean tree. |
| Not touched | Unity/Editor, the CCF-Ops physical lease, `Assets/`, saves, PlayerPrefs, the user's working checkout, the author branch, the #473/#474 reports and branches, `active-tasks.md`, `main`, the #477 source and its four DRAFT files. **Disclosure:** my first command ran a read-only `git fetch origin` inside the user's checkout (it updates remote-tracking refs only). Its working tree was clean before and after (`git status --short` empty, branch `main`). |

### 1.1 Scope comparison (actual git output)

`git diff --name-status c5461948… 94b82a0…` in the full checkout → **12 paths**: 1 `A` (the #477 handoff) and 11 `M` (7 under `Tools/ContentReview/`: `README.md`, `app.js`, `catalog.js`, `catalog.json`, `core.js`, `styles.css`, `tests.mjs`; 4 under `specs/ccf-cross-scenario-text-editor-v1/`: `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`). GitHub `compare` agrees: ahead 12, behind 0, 12 files. A grep for any path outside the packet allowlist returned **nothing**. A grep for `Assets/`, `Packages/`, `ProjectSettings/` or `Docs/Project/` returned **nothing**. `index.html`, `source-audit.mjs` and `source-map.json` are unchanged. **Scope PASS.**

Integrity of the baseline: `catalog.json` is parsed-equal to `catalog.js` (committed test), and **no existing entry's `baselineText` or `text` changed** between base and repair. The repair adds exactly one entry (`s1.help.walkinghud.title`), removes none, and every `text` equals its `baselineText` in the seed.

---

## 2. Environment and execution evidence

| Tool | Version / method |
|---|---|
| Node | **v26.13.1** in both places: native Windows (on a byte-verified copy of the pinned files) and WSL Ubuntu (in the real full checkout). |
| Browser | **Microsoft Edge 157.0.4341.46** (Chromium; UA `…HeadlessChrome/157.0.0.0 … Edg/157.0.0.0`), `--headless=new`, plus one **headed** launch. Page loaded by `file:///C:/…/pinned/Tools/ContentReview/index.html` (**`file://`, no HTTP server**). |
| Interaction | A dependency-free CDP driver I wrote (`harness\cdp.mjs`). Clicks are `Input.dispatchMouseEvent` press/release, scrolling is real `mouseWheel` events at the scrollable region's centre, and **every click first checks that `document.elementFromPoint` returns the intended element** (a covered or off-screen target fails the test). Typing is `Input.insertText`. Shortcuts are `Input.dispatchKeyEvent`. Import goes through the real **Import JSON** button and the browser file chooser (`Page.setInterceptFileChooserDialog` + `DOM.setFileInputFiles`). Exports are real browser downloads. `beforeunload` and `confirm` dialogs are captured as real browser dialogs. |
| Viewports | **1280×720**, **1920×1080** (viewport emulation on a real page), **560×900** smoke, plus one headed window (outer 1280×720 requested; Edge opened it at 1152×1039 outer, 1128×947 inner, so it is **not** a required viewport and is informational only). |
| Firefox | **NOT RUN.** Firefox 160.0 is installed, but my driver is Chromium/CDP-only and I built no Firefox harness. One engine satisfies the packet minimum; both were preferred. |
| macOS / `Command+S` | **NOT RUN** (no Mac). `Ctrl+S` was tested; the `metaKey` branch was read in `app.js` only. |
| Human click-through | **NOT RUN.** Every browser action was a scripted real input event; I reviewed captured screenshots by eye. |
| Fixture-only layout | **Not used as evidence.** All F-01 results below come from the actual 269-field `index.html`. |

### 2.1 Commands and exit codes

| Command (cwd = full pinned checkout unless noted) | Exit | Result |
|---|---|---|
| `node --version` | 0 | `v26.13.1` (WSL and Windows) |
| `node Tools/ContentReview/tests.mjs` (WSL, **full checkout**) | **0** | **36 `PASS` lines, 36 committed check groups.** Final line: `36 deterministic static checks passed. NOT Unity or rendered-browser qualification.` stderr empty. |
| `node Tools/ContentReview/tests.mjs` (native Windows, byte-verified pinned copy) | **0** | Identical: 36 PASS, stderr 0 bytes. |
| `node Tools/ContentReview/source-audit.mjs . --json` (WSL, full checkout) | **0** | `baseline` `fc18190b…`, **28 verified**, **0 changed**, **0 missing**, **88 unmapped candidate C# files**, **1 serialized text candidate** (`Assets/Scenes/ForestryAssetReview.unity`, 11 lexical candidates). |
| `node Tools/ContentReview/source-audit.mjs .` (text mode) | 0 | Same counts. |
| `harness\layout.mjs`, `layout2.mjs`, `flow.mjs` ×2, `hostile.mjs` ×2, `oldimport.mjs` (Windows Node, real Edge) | 0 | Matrix in §3 and §4. |
| `harness\mutate.mjs` (mutation sanity on a temp copy) | 0 | **5/5 mutations CAUGHT** by the committed tests. |
| `harness\source-check.mjs` (independent C# literal decoder, WSL) | 0 | §3 F-08. |

**Interpretation of the audit (as the packet requires):** the 28 main anchors and 88 unmapped C# files are a **drift and candidate scanner, not a census**. Full player-visible coverage remains **UNKNOWN**, not zero. The coverage statements in the UI, README and spec say so. I found no "every text" claim.

**Mutation sanity** (do the committed tests really guard the repair?): on a temp copy I removed, one at a time, (1) the unknown-key rejection, (2) the entry-order check, (3) the C1/bidi/zero-width ranges, (4) numeric counts on early errors, (5) the revision upper bound. `tests.mjs` exited non-zero on **all five** (`AssertionError`). I did not weaken any committed test.

**Harness defects I hit and fixed (disclosed):** (1) my first 560 px "coverage visible" criterion required the whole panel box (including padding) inside the viewport and reported a false FAIL; I replaced it with "last text line fully visible", which PASSES (§3 F-01). (2) My first 1280×720 hostile run reported two false FAILs: `textContent` glued the `<li>` errors together ("approvedUnknown…"), which broke a `\b` regex, and a download-count assertion was invalid because Edge overwrites an identically named download. I fixed both and **re-ran the complete hostile matrix at both viewports (31/31 each)**. The failing first-run log is kept locally (`evidence-browser-hostile-1280.log`).

Screenshots, JSON results and logs are stored **only in the local scratch path** above (`evidence-browser\`, `evidence-node\`, `evidence-browser-*.log`). The packet allows exactly one new GitHub file, so they are not published.

---

## 3. Finding matrix F-01 … F-13 (actual app unless stated)

| ID | Result | Observed vs expected (evidence) |
|---|---|---|
| **F-01 MAJOR** 720p sidebar reachability | **PASS** | **1280×720:** sidebar box 644 px, `overflow:hidden`; scenario list 66 px box (scrollH 567); **page list 284 px box** (scrollH 4,826); coverage 95 px box. Real wheel + pointer clicks with hit test: **first** page (0 wheel steps) → `1 / 79`; **middle** (page 40, 8 steps) → `40 / 79` "Plan an eligible pruning lift"; **final** (79, 8 steps) → `79 / 79` "Objective 1 UX candidate copy (not main)". Return to first (15 steps) → `1 / 79`. Scrolling the scenario list: last scenario (9th) clicked after 2 steps, placeholder shown, first scenario clicked back → `Pages · 79`. **1920×1080:** page list 635 px; same clicks all PASS (7–8 steps). **560×900:** single column, page list 330 px; first/middle/final, last scenario and back all PASS; last coverage line fully visible after document scroll (top 845, bottom 863 of 900). **Coverage:** at both desktop sizes the coverage box is 95 px with 116 px of content, so its last line starts 9 px clipped, but a wheel over the box reveals it (scrollTop 22, clipped −13 px) — reachable, cosmetic (O-1). Headed real window (inner 1128×947): first/middle/final PASS. Zero console errors in every run. *Before the repair (#474) no page button was clickable at 1280×720.* |
| **F-02 MAJOR** fake approval keys | **PASS** | Through the real Import button, with 3 real edits in memory (2 exported, 1 unsaved): **B01** top-level `approved:true, integrated:true, approvedBy` → "Import rejected; 3 validation errors", messages `Unknown package field: approved` / `integrated` / `approvedBy`. **B02** `entries[0].approved/reviewedBy` → `Unknown entries[0] field: approved` / `reviewedBy`. Nested: `Unknown provenance field: approved`, `Unknown scenarios[0] field: approved`, `Unknown pages[0] field: approvedBy`, `Unknown entries[3].source field: reviewed`, `Unknown coverage field: approved`, `Unknown coverage.classifications field: approved`, mixed valid edit + several unknown keys also rejected, `__proto__` own key → `Unknown package field: __proto__`. For every case: **zero confirm dialogs** (rejected before the replace prompt), the unsaved edit and status text intact. After all 31 hostile files, **Ctrl+S export contained no `approved`/`integrated`/`approvedBy`/`reviewedBy`/`__proto__` key anywhere (56 distinct keys, all in the closed schema)**, no `Manager #424` string, `authoringStatus:"draft-local-export"`, `provenance.reviewed:false`, `provenance.unityIntegrated:false`, and all 3 in-memory edits present. Positive control: the genuine good draft imported after a `Replace your current unsaved draft…?` confirm. `window.__xss` never set, 0 injected `img`/`svg` nodes. The same matrix passed at 1280×720 and at 1920×1080. *Baseline-is-not-a-signature honesty:* the validator proves equality to the bundled seed, not authenticity; I did not treat it as cryptographic human approval. |
| F-03 entry order | **PASS** | **B03** (entries 0 and 1 swapped) → `Entry field order / identities changed.` in the browser; committed Node test `field array order B03 remains immutable`. In-memory edits intact. |
| F-04 invisible / bidi / C1 | **PASS** | Browser: B04 (`U+200B U+202E U+202C U+FEFF`), B05 (`U+0085 U+009B`), B05b (`U+00AD U+2060`), NUL → each `Disallowed control/invisible/bidi character: <entry id>`. Irish accents, arrows, smart quotes, newline and tab are accepted (typed edit round-trips byte-exact, §4). Hardening observation O-4. |
| F-05 search | **PASS** | Real typing into the search box: `sitka` → **`Search results · 5`**; `plantation` → **7**; `regeneration` → 10. `scenario` → 12 (it was all 79 in #474, because the scenario label was searched). A real click on a result opened a page that contains the word. Clearing restores `Pages · 79`. Same at both viewports. |
| F-06 live edited marker, caret | **PASS** | Real click into a field, `Ctrl+End`, typed `Z` then `Y`: active element stays `edit-0`, `selectionStart` 34 → 35 → 36 (no caret reset), the active page's sidebar marker became `main-source · 1 edited` on the first keystroke, global status `1 locally edited field · not exported · not approved`, flag `EDITED`. |
| F-07 unsaved-change prompt | **PASS** | Native `beforeunload` captured as a real dialog (empty message by design): (a) unsaved edit → reload prompts; **cancel keeps the page and the edit**; (b) `Ctrl+S` with the textarea focused → one download, status `exported draft`; (c) reload after a successful export → **no prompt**; (d) after a successful import → **no prompt**; (e) edit then undo back to the saved text (`Q` + Backspace) → **no prompt** (prompts only when text differs from the last successful export/import); (f) a *blocked* export (token deleted → `Export blocked: fix validation errors first.`, no download) → reload **still prompts**; (g) accepting the prompt leaves the page and the edits are lost as documented. Exactly three `beforeunload` dialogs in the whole run (2 cancelled, 1 accepted). Same at both viewports. `Command+S` NOT RUN. |
| F-08 source line citations | **PASS** | Independent literal-aware decoder against the **full checkout's C#** (`harness\source-check.mjs`). *Opening-fragment-on-cited-line:* author base **206 ok / 59 not**; repair **262 ok / 4 not**. *Whole text on the cited line:* base 206 exact / 39 within ±4 / 20 miss; repair **254 exact / 10 within ±4 / 2**. All residuals are legitimate multi-line anchors, checked by reading the C#: the 4 are `LearningObjectivesView.Curriculum.cs:113` (`case "rack.read": return "…" + …`), `MenuHelpView.Text.cs:31` (a multi-line `const`), and the two Annual Review headings cited at `AnnualReviewView.cs:122` (start of a ternary; the literals are on 123/124, O-3); the 2 whole-text misses are the long `Tree Marks` and `Walking HUD` full-F1 strings, which begin on the cited line and run past the window. Spot-checked samples (cited line, was): F1 full help — Tree Inspection **106** (was 40), Stand Map **112** (41), Work Plan **117** (42), Annual Review **122** (43); Work Plan title **128** (120) = `title.text = $"Work Plan · Year {…}"`; Annual Review empty state **127** (135), `Work done` column **146** (156); teaching `practice` **39 / 44 / 49 / 54** (35 / 40 / 45 / 50), each a verbatim literal line. **Source text faithfulness:** zero `baselineText`/`text` changes to any of the 268 existing entries (see §1.1). Exception noted separately as **N-01** (a composed string whose seed text is not faithful). |
| F-09 Walking HUD help title | **PASS** | Source: `MenuHelpView.Text.cs:46` is `default: return "Walking HUD · your forest at a glance";` (decoder: exact). App: searching the phrase gives 1 page (`WalkingHud`) whose field `title` holds exactly that text with `Content ID (locked): s1.help.walkinghud.title`, source `main · …MenuHelpView.Text.cs : 46 · literal`. Seed is 269 fields. |
| F-10 unknown coverage | **PASS** | `coverage.classifications` = `collected 236, conditional 28, generated 5, serialized null, unlocated null, outOfScope null` (base had `0, 0, 0`). `collected` rose by 1 for the new title. The notes state these categories are UNKNOWN, not measured zero. The validator accepts only `null` or a non-negative safe integer. |
| F-11 committed negatives | **PASS** | Node 36/36 (exit 0, Node and WSL), mutation 5/5 caught. The tests cover B01–B05, revision, status promotions, scenario/page/profile/source/baseline mutation, entry add/remove, >10,000 / >3,000 characters, Unicode, nulls and numeric early-error counts. |
| F-12 candidate commit pin | **PASS** | `provenance.comparisonCandidateCommit` and **all 3** candidate `source.commit` fields equal the full 40-hex `acb4e1111fdc9aa9b295413038eda4e03fe56402`. That commit exists and is still the candidate branch head. At that commit the three cited lines (`MenuHelpView.Text.cs:41`, `:64`, `LearningObjectivesView.Curriculum.cs:102`) still contain the candidate wording. |
| F-13 early-error counts | **PASS** | Browser: root `[]`, `null`, `"hello"`, `42` → `package must be an object.`; `{}` → 8 errors incl. `Unsupported schema version`; wrong schema; `entries` as a string; `scenarios {}` and `pages 5`; missing provenance. Every summary reads like `0 entries / 0 pages / 0 edited fields. 1 errors, 0 warnings.`: **numeric, never `undefined` or `NaN`**. Also: 3,249,737-byte file → `File is too large (3 MB maximum).` (rejected by `file.size` before reading); corrupt JSON; 200,000-level nesting → `package must be an object.`. All with the edit intact and no confirm dialog. |

**Additional behaviour checked (not in the matrix):** an old 268-entry draft (author-base seed with one edit) is **rejected** ("Import rejected; 65 validation errors", leading messages `Protected source provenance / approval facts changed.`, `Entry count changed; adding/removing fields needs schema review.`) and the current unsaved edit stayed intact. That is safe and matches the documented seed-revision-2 incompatibility; the message does not name "seed revision 2" (O-5).

---

## 4. Editor flow results (actual app)

All PASS at **1280×720 and 1920×1080** with 0 console errors/warnings/exceptions and **network = the five local files only** (`index.html`, `styles.css`, `catalog.js`, `core.js`, `app.js`; 25 requests in a run, 0 non-`file:`).

- **Round trip:** typed a multi-line Unicode edit (`Athbhreithniú · Bliain — “réidh” & O’Brien’s → ✓ C:\temp\n literal⏎⇥Tab líne 3: Fadó, Tá an choill beo.`), `Ctrl+S` → `ccf-text-draft-2026-11-14.json` with `authoringStatus:"draft-local-export"`, `revision` 3 (seed 2 + 1), `unityIntegrated:false`, `reviewed:false`, only schema keys, the 268 other entries unchanged, entry order identical to the seed, the edited text **byte-exact**. Reload → `No edits · source baseline`. Import through the real chooser → `Imported ccf-text-draft-2026-11-14.json; 1 edited fields restored.` and the text was byte-exact again.
- **Legacy / Rack V2 variants:** *Create your first rack* offers `default`, `legacy-or-fallback`, `enlarged80-rack-v2`; the two views show different bodies (legacy "Press B in the forest…", V2 "Open the Stand Map [M] and choose Plan Racks…").
- **Placeholders and filters:** Scenario Two shows no fields and `NO SHIPPED TEXT YET…`; status filter `unmerged-candidate` → `Pages · 1`, `main-source` → `Pages · 78`.
- **Keys:** Alt+→ inside a textarea no longer navigates (page stays `2 / 79`). `Ctrl+S` while a textarea has focus exports.
- **Injection:** `<img src=x onerror=…>` → `Rich/HTML markup is not supported in V1`; tag-free `<svg/onload=window.__xss=1` is accepted as inert text (no node, `__xss` unset). A junk `exportedAt` suffix is accepted by the validator but **never re-exported** (the export regenerates `exportedAt`).

---

## 5. Four-file DRAFT review: S-01 … S-05

I read the exact repair delta of all four files (`+79` lines, additions only) and checked each claim against `core.js`, `catalog.json` and the C#. **All four still begin with DRAFT / NOT APPROVED.** No Unity import is claimed.

| ID | Result | Assessment |
|---|---|---|
| **S-01** closed recursive schema; baseline ≠ signature | **PASS** | Spec lists unknown-field rejection at package, provenance, scenario, page, entry, source, coverage and classifications; the code's `closed()` runs at exactly those levels and I observed each in the browser (F-02). Optional draft fields are only `authoringStatus="draft-local-export"` and `exportedAt`, as coded. "Equality to the bundled seed is integrity relative to that baseline, **not** cryptographic authentication" is stated plainly, and I confirmed it holds (a hand-written file matching the seed passes). |
| **S-02** derived / composed strings | **PASS (as a DRAFT rule)** — see N-01 | Facts verified in C#: `ControlsReference.KeysLineFor(bool)` is `KeysLine.Replace(…).Replace(…)` (`ControlsReference.cs:20–22`), `MenuHelpView.cs:129–130` applies further `.Replace()` on help text in a V2 world, and `MenuHelpView.Text.cs:93–99` builds Tree Marks help from `TreeMarkEntries`, two constants and literals. The rule (expose pieces + dependency, or mark read-only/derived; never one flattened C# replacement; `{{keysLine}}` as a typed placeholder) is correct. **Gap between the rule and the seed:** the current seed does not follow it for the very example named (N-01). |
| **S-03** exact Unicode allowed/blocked | **PASS** | The spec list matches the validator regex character-for-character: C0 except tab/LF/CR, `U+007F–U+009F`, `U+00AD`, `U+034F`, `U+061C`, `U+180E`, `U+200B–U+200F`, `U+202A–U+202E`, `U+2060–U+206F`, `U+FEFF` blocked; Irish, arrows, quotes, `\n`, `\t` allowed. I probed 23 code points against the real validator (all spec-listed ones REJECTED; Irish, arrows, NBSP, tab, newline ACCEPTED). Cosmetic: the spec calls `U+007F` a C1 control, but it is DEL (C0 range in Unicode). Hardening suggestion O-4. |
| **S-04** atomic coupled legacy/V2 fallback | **PASS** | Whole-family fallback to the existing C# literal or generator; no mixed new/old copy; no deriving legacy from V2 by editing numbers; no profile inference from the editor. The seed has coupled families (`rack.create`, `rack.map`) with both variants complete, so the rule is testable later. Draft only; no Unity resolver exists or is claimed. |
| **S-05** shared source-status vocabulary | **PASS** | The vocabulary (`main-source`, `unmerged-candidate`, `no-shipped-text`, `draft-no-text`, `reviewed-not-integrated`) covers every status in the catalogue: entries and pages use `main-source`/`unmerged-candidate`; scenarios use `main-source`, `no-shipped-text` ×7, `draft-no-text` ×1. The spec states these are source statuses, not approval flags, and that `APPROVED/NEEDS CHANGES/REJECTED` are human verdicts. (`reviewed-not-integrated` is defined but unused, appropriately.) |

`tasks.md` rows and the checklist honestly read "SOURCE REPAIR IN PROGRESS / independently UNVERIFIED", with only R474-07 ticked. They describe the state *before this review*; #424 should refresh them on disposition (admin, not quality).

---

## 6. Numbered findings (new, residual or carried)

Severity: **BLOCKING** none · **MAJOR** none · **MINOR** 1 · **OBSERVATION** 5.

### MINOR

**N-01 — The Tree Marks full-F1 help entry is an editable flattened copy with blank slots, contradicting the new S-02 rule.** `Tools/ContentReview/catalog.json` / `catalog.js`, entry `s1.help.treemarks.explanation.full-F1` (source `MenuHelpView.Text.cs:93`, `kind: conditional`, status `main-source`, `format: plain-text`).
*Observed:* its `baselineText` is `…a paint mark records a management decision on the tree itself:\n\n•  — \n\n One spray paints vertical stripes…\n\n\n\nAfter Confirm Rack Plan…`. The bullet list and two constants that C# interpolates at `MenuHelpView.Text.cs:94–96` (`TreeMarkEntries`, `TreeMarksOrangeText`, `TreeMarksSimplificationText`) are **empty**, so the field and the "Original source" panel show text the player never sees. The field is fully editable, and the only caution is a generic note. The text is byte-identical to the #473 base, so this is **not a #477 regression** and was not a #474 finding; S-02 now names this exact composition.
*Expected:* either independently supported constituent pieces with an explicit dependency, or a read-only/derived marker (S-02), and an "Original source" that is faithful or visibly marked as not.
*Minimal bounded fix (one writer, no Unity):* in `catalog.json`/`catalog.js` (seed revision 3), replace the flattened field with the three real `TreeMarkEntries` pairs plus the two existing constants as separate entries, with a dependency note, or flag it derived/read-only and surface that flag in the field header; add one committed Node test that no editable `baselineText` contains an empty bullet slot such as `•  — `; refresh the README line. The same audit is worth repeating for any other `string.Join` or `.Replace` composition in the seven mapped files.
*Impact:* the tool still behaves safely and nothing is integrated. Owners should treat that one field as non-authoritative until fixed.

### OBSERVATIONS (no action needed for the verdicts)

- **O-1** At ≥ 901 px the coverage panel is 95 px tall with 116 px of content, so its last line ("No edits change the game; download and separately review the JSON.") is about 9 px clipped until the user scrolls inside the box. At 1280×720 the scenario list is likewise only 66 px (about one row) before scrolling. Both are reachable by wheel; raising the coverage `max-height` to about 120 px would show it fully.
- **O-2** The F-01 sizing relies on `100vh`. I tested real Chromium only; Firefox and Safari are NOT RUN.
- **O-3** Two Annual Review headings (`s1.annual.review.yearHeading` and `historicalHeading`) cite `AnnualReviewView.cs:122`, the start of the ternary statement, while their literals are on lines 124 and 123. This is a defensible "statement start" convention but differs from the per-literal convention used elsewhere.
- **O-4** Unicode hardening (outside what S-03 promises): the validator, exactly as the spec states, still **accepts** other invisible or layout characters: `U+2028` / `U+2029`, Hangul fillers `U+3164` / `U+FFA0`, `U+2800` braille blank, `U+FFF9–U+FFFC`, `U+FE0F`, and the astral **Tag characters `U+E0000–U+E007F`** (not matched by the non-`u` regex). If hidden text in game copy matters, consider adding them and widening S-03; NBSP and `U+FE0F` may be legitimate and need a deliberate decision.
- **O-5** An old 268-entry draft is rejected with 65 generic errors; the message could name "seed revision 2 / re-apply manually" to help an owner who has older exports.

---

## 7. NOT RUN, limits and honesty statement

- **Not run:** Firefox; macOS and `Command+S`; a human-eye usability session; any Unity Editor, compiler, Player, saves, PlayerPrefs, Windows build or physical Ops board; any claim about text shown inside the game. Browser evidence is **one engine** (Edge 157 / Chromium), scripted real input events.
- The viewports are emulated device metrics on a real page (the headed window was not 720p). Hit testing used the real browser (`elementFromPoint`) at the real coordinates the pointer events used.
- The full-checkout Node run and audit are against the **pinned** HEAD in a disposable clone; the browser ran on a hash-verified copy (`git hash-object` equals the checkout blob for every file in `Tools/ContentReview` and `specs/ccf-cross-scenario-text-editor-v1`).
- The #477 author's own V8-with-shims results were not relied upon; everything above was re-executed.
- Dashboard and Drive mirrors: **NOT VERIFIED**. This report distinguishes executed from inspected. Where an item was only read (the `metaKey` branch) it is labelled.

## 8. Recommendation for #424 (nothing started)

1. **Tool (a) APPROVED for owner browser use** of the standalone editor. Optionally open one bounded follow-up for **N-01** (and O-1, O-4, O-5) before the owner reviews the Tree Marks page or anyone builds a Unity adapter.
2. **DRAFT quality (b) APPROVED as DRAFT quality only.** Promotion to an approved spec, any typed Unity adapter, the source-coverage census, #443 copy reconciliation and any Windows build stay behind the separate #424/#425/user gates. Refresh the stale `tasks.md`/checklist rows when you disposition this review.
3. No further browser re-run is needed for F-01 and F-02. A Firefox/macOS smoke is a nice-to-have only if the owner will use those.

## 9. Administrative status

- `active-tasks.md`: read only (blob `0c6677ef…` unchanged from start to finish); #424 is the sole writer. `main` unchanged at `fc18190b…`. The #477 branch is still `94b82a01…`; the #473 author branch is still `c5461948…`; the #474 review branch is still one added Markdown.
- Reviewer branch comparison against `main`: expected **1 added Markdown file, 0 other changes**; the actual readback is recorded in the #480 handoff comment.
- Review completion means **executed and handed off**, not integration, feature acceptance or product approval.
