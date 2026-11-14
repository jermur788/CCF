# CCF Text Editor — independent browser/Node test and four-file DRAFT review (#474)

**Task:** [#474](https://github.com/jermur788/CCF/issues/474), independent non-author review of the [#473](https://github.com/jermur788/CCF/issues/473) standalone CCF Text Editor and DRAFT spec.  
**Binding packet:** [MANAGER-CCF-TEXT-EDITOR-474-INDEPENDENT-BROWSER-AND-SPEC-REVIEW-DISPATCH-20261114.md](https://github.com/jermur788/CCF/blob/docs/ccf-text-editor-independent-review-dispatch-20261114-v1/Docs/Handoffs/ScenarioOne-TextEditor-20261114/MANAGER-CCF-TEXT-EDITOR-474-INDEPENDENT-BROWSER-AND-SPEC-REVIEW-DISPATCH-20261114.md)  
**Date:** 14 November 2026  
**Routing:** Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424) owns the disposition of this review. Overall Manager [#425](https://github.com/jermur788/CCF/issues/425) and the user hold cross-scenario product and copy authority.

> **Status labels (all apply):** **NOT UNITY-INTEGRATED · NOT GAME-VERIFIED · DRAFT NOT PRODUCT-APPROVED · NO USER ACCEPTANCE.** This review covers a standalone browser tool that runs outside Unity, plus its DRAFT documents. A browser JSON export is not a Unity content import, a source commit, accepted copy, a production build or a save change.

---

## 0. Verdicts (summary)

| Scope | Verdict | Why, in one line |
|---|---|---|
| **Standalone browser prototype** (`Tools/ContentReview/**`) | **NEEDS CHANGES** | Core editing, round-trip and import safety work in a real Chromium browser (Edge 157). There are two MAJOR defects: the sidebar page list can't be reached at a 1280×720 viewport, and the validator accepts and re-exports unknown approval-looking keys. No BLOCKING or security defect was found. |
| **Four-file DRAFT quality** (`specs/ccf-cross-scenario-text-editor-v1/**`) | **NEEDS CHANGES (minor, bounded)** | The architecture, gates and honesty boundaries are sound. The spec needs a few small contract additions: a closed-schema rule, matching the invisible-character rule to the code, rules for derived and composed strings, status vocabulary, and partial-package fallback. No rework is needed. |

Neither verdict approves production integration, game copy or the DRAFT as product authority. Promoting the DRAFT stays with #424, #425 and the user.

---

## 1. Reviewer identity, lane and pinned candidate

| Item | Value |
|---|---|
| Actual reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in T3 Code. A separate session from the OpenAI GPT-6 author of #473. Reasoning setting not observable beyond the harness default. |
| Registration | [#474 [RUNNING] comment](https://github.com/jermur788/CCF/issues/474#issuecomment-6868245114), posted **before** any branch or file write |
| Reviewer branch | `docs/ccf-text-editor-independent-review-20261114-v1`, created from `main` @ `fc18190b976f1363f1b5d1b0881810db2f8c6e39` (re-verified immediately before creation) |
| Only file written | `Docs/Handoffs/ScenarioOne-TextEditor-20261114/CCF-TEXT-EDITOR-INDEPENDENT-REVIEW-AND-BROWSER-TEST-20261114.md` (this report) |
| Pinned author candidate (read-only) | `feature/ccf-text-editor-browser-prototype-20261113-v1` @ **`c5461948d2088e27f8e51a2d867a2aff70e243cc`**. Verified unchanged at start and again before publishing. |
| Local scratch (temporary, isolated) | `C:\Users\hp\AppData\Local\Temp\claude\7ac52aaebe79051c\f893e763-5eae-469b-a9c4-a8ab6f36bd9b\scratchpad\ccf-474`. A fresh `--no-hardlinks` clone, detached at `c5461948…`. The harness scripts and evidence sit next to it in `…\scratchpad\harness`, `…\evidence*`, `…\fixtures`. Nothing was installed into any repository. |
| Not touched | Unity/Editor, the CCF-Ops physical lease, `Assets/`, saves, PlayerPrefs, the user's working checkout (read-only `git fetch`/`git show` only), the author branch, `active-tasks.md`, `main` |

### 1.1 Candidate comparison (actual `git` output)

`git rev-list --left-right --count fc18190…...c5461948…` → **`0 31`** (0 behind, 31 ahead). Merge base = `fc18190b976f1363f1b5d1b0881810db2f8c6e39`.  
`git diff --name-status fc18190… c5461948…` → **16 files, all `A` (added)**, 13,917 insertions, **0 modified or deleted**:

```
A Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md
A Docs/Research/ContentReview/CCF-PLAYER-FACING-TEXT-INVENTORY-AND-ARCHITECTURE-20261113.md
A Tools/ContentReview/{README.md, app.js, catalog.js, catalog.json, core.js, index.html, source-audit.mjs, source-map.json, styles.css, tests.mjs}
A specs/ccf-cross-scenario-text-editor-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}
```

There are no production C#, Unity scene, prefab, `Packages/`, `ProjectSettings/` or coordination changes. **This matches the author's claim.**

---

## 2. Environment and execution evidence

| Tool | Version / method |
|---|---|
| Node | **v26.13.1** (Windows, native). A second v26.13.1 exists in WSL Ubuntu but was not used for the recorded runs. |
| Browser | **Microsoft Edge 157.0.4341.46** (Chromium; UA `… HeadlessChrome/157.0.0.0 … Edg/157.0.0.0`, DevTools protocol 1.3). Primary run was `--headless=new`. A second **headed** (visible window) launch repeated the load, network and console checks and the sidebar-reachability probe. |
| Launch | `msedge.exe --remote-debugging-port=… --user-data-dir=<scratch profile> --no-first-run --disable-extensions` → `Page.navigate` to `file:///C:/…/scratchpad/ccf-474/Tools/ContentReview/index.html` (**file://, no HTTP server**) |
| Interaction | My own dependency-free CDP harness (`…\scratchpad\harness\cdp.mjs`, `journey.mjs`, `probes.mjs`). Clicks use `Input.dispatchMouseEvent`, keys use `Input.dispatchKeyEvent` (Ctrl+S, Alt+←/→, Tab, Enter, Ctrl+A, Backspace), typing uses `Input.insertText`, imports go through the real file chooser via `DOM.setFileInputFiles`, and exports are real browser downloads (`Browser.setDownloadBehavior`). Each click first checks that `elementFromPoint` hits the intended element, so a misclick fails instead of passing silently. Confirm dialogs were captured and accepted or dismissed through `Page.javascriptDialogOpening`. |
| Firefox | **NOT RUN.** Not installed. One engine (Chromium/Edge) satisfies the packet minimum; both were preferred. |
| Manual human click-through | **NOT RUN.** All browser actions were scripted real input events. No human-eye UX session took place apart from my review of the captured screenshots. |

### 2.1 Commands and exit codes

| Command (cwd = pinned scratch checkout) | Exit | Result |
|---|---|---|
| `node --version` | 0 | `v26.13.1` |
| `node Tools/ContentReview/tests.mjs` | **0** | **20/20 PASS**, stderr empty. Final line: `20 deterministic static checks passed. NOT Unity or rendered-browser qualification.` |
| `node Tools/ContentReview/source-audit.mjs . --json` | **0** | `verified` 28, `changed` 0, `missing` 0, `unmappedSource` 88, `serializedTextCandidates` 1 (`Assets/Scenes/ForestryAssetReview.unity`, 11 lexical candidates) |
| `node Tools/ContentReview/source-audit.mjs .` (text mode) | 0 | Same counts. The text mode prints only the first 30 of the 88 unmapped paths (see O-07). |
| Reviewer `harness/adversarial.mjs` (core.js validator, 55 cases) | 0 | 49/55 matched my expectation; **6 gaps** (§3.2) |
| Reviewer `harness/journey.mjs` (real browser, full matrix) | 0 | Matrix in §4 |
| Reviewer `harness/probes.mjs` (headless 1280×720 and headed) | 0 | Sidebar reachability, search haystack, validation display (§4, §6) |

Screenshots, `results.json`, `log.txt` and exported JSON files are stored **only in the local scratch path above**: `evidence\`, `evidence-probes\`, `evidence-probes-headed\`. The packet allows exactly one new GitHub file, so they are **not** published to GitHub. Capture names are listed in §4.

---

## 3. Node and data-contract review

### 3.1 Committed suite

All 20 committed checks pass under real Node. I also read `core.js` (67 lines) directly. Its validator checks schema version, array shapes, unique IDs, orphan pages and entries, and (against the bundled seed) `packageId`, `provenance`, `coverage`, `revision`, `authoringStatus`, `scenarios[]`, `pages[]` (deep equality), entry count, and per entry: `pageId, fieldKey, variantId, baselineText, required, source, status, protectedGameplayKey, tokens, format, note`. Text checks cover blank required text, C0 controls and DEL, more than 10,000 characters (error) or more than 3,000 (warning), `<tag>` markup, an ordered token signature, and malformed `{{`/`}}`.

**Gap vs the author's "30 direct V8 checks":** the 30 checks are only described by category in the author handoff (§5), not committed one by one. The committed suite has **20** checks and leaves out several categories the author lists. It has no committed negatives for: unsupported `schemaVersion`, invalid `revision`, `authoringStatus` tampering, entry `status` promotion, `scenarios[]` change, page `order`/`scenarioId` change (only `protectedStepId` is tested), `baselineText` edit, entry add/remove (count), text over 10,000 characters, and the app-level corrupt-JSON and >3 MB paths. My adversarial cases A01–A40 cover **all** of these and **all pass**, so the behaviour is correct. The committed suite just doesn't reproduce the author's claim (finding F-11).

### 3.2 Adversarial import cases (validator level, 55 cases)

All packet-listed rejection classes were **rejected with a legible error**:

| Class | Cases | Result |
|---|---|---|
| Duplicate / missing / extra content ID | A01, A02, A03 | REJECT ✔ |
| Changed `pageId`, scenario, page order, array order, lesson ID, step ID | A04–A09 | REJECT ✔ |
| Edited `baselineText`; source branch/line; `provenance.sourceCommit` | A10–A13 | REJECT ✔ |
| `reviewed=true`, `unityIntegrated=true`, `authoringStatus="approved"`, candidate→main status, variant swap | A14–A18 | REJECT ✔ |
| Note metadata, injected `protectedGameplayKey`, token-signature edit | A19–A21 | REJECT ✔ |
| Missing / extra / reordered / malformed (`{{ year }}`) / unmatched `}}` tokens | A22–A26 | REJECT ✔ |
| Blank required, unsupported schema, revision 0 or non-integer, NUL, ESC | A27–A32 | REJECT ✔ |
| `<img onerror>`, `</textarea><script>`, >10,000 characters, packageId, coverage, scenario status, non-array, non-string text | A33–A40 | REJECT ✔ |
| Non-objects (`null`, `[]`, string, number) | N01–N04 | REJECT ✔ |
| Unicode/`&`/arrows/smart quotes/backslash/tab/multiline/token; CRLF; own export stamp | P01–P03 | ACCEPT ✔ |
| Forged `exportedAt`, `revision = MAX_SAFE_INTEGER`; tag-free `<svg/onload=…` (inert text) | B06, B07 | ACCEPT (acceptable) |

**Bypasses / gaps, each reproducible:**

| Case | Input (exact) | Observed | Expected | Finding |
|---|---|---|---|---|
| **B01** | Seed + top-level `"approved": true, "integrated": true, "approvedBy": "Manager #424"` | **ACCEPTED**; the flags **survive** the tool's own validate→export (confirmed in the browser, §4 UI-10) | Reject unknown top-level keys | **F-02 MAJOR** |
| **B02** | Seed + `entries[0].approved = true, entries[0].reviewedBy = "owner"` | **ACCEPTED** | Reject unknown entry keys | F-02 |
| **B03** | Swap `entries[0]` and `entries[1]` | **ACCEPTED**; field display order on page `s1.intro.p1` changes | Field order preserved (spec §2) | F-03 MINOR |
| **B04** | `entries[0].text = "What\u200B is \u202Eyrtserof\u202C?\uFEFF"` | **ACCEPTED** | Spec §4: "reject invisible/control characters" | F-04 MINOR |
| **B05** | `entries[0].text = "What\u0085is\u009b?"` (C1 controls) | **ACCEPTED** | Reject | F-04 |
| **B08** | `entries[0].text = "Trees with DBH <b and c> stay"` | **REJECTED** as markup | Legitimate prose (false positive) | O-01 |

Fixtures are in `…\scratchpad\fixtures\{A01..A40,B01..B08,P01..P03,X01..X03}.json`. The generator is `harness/adversarial.mjs`.

---

## 4. Real-browser matrix UI-01 … UI-15 (Edge 157, file://)

Functional journey at a **1920×1080** viewport. Layout checked at **1280×720**, **1920×1080** and **560 px** wide. `PASS` means directly observed through real input events in the browser.

| ID | Result | Observed evidence (from `evidence\results.json` / `log.txt`) |
|---|---|---|
| **UI-01** Launch | **PASS** | 9 scenario buttons, *Scenario One · Inherited Sitka Plantation* active, first page *What is Continuous-Cover Forestry?* `1 / 79`. `window.CCF_CONTENT_SEED` loaded via `<script>` (268 entries, no fetch). **Network:** only 5 `file://` requests (index.html, styles.css, catalog.js, core.js, app.js), **0 remote**. Over the whole run: 25 requests, 0 non-file/blob. **Console errors/warnings: 0** for the whole run (headless and headed). Captures: `UI-01-launch-1280x720.png`, `UI-01-launch-1920x1080.png`, `evidence-probes-headed\headed-launch.png`. |
| **UI-02** Intro pages | **PASS** | p1 14 fields, p2 13, p3 6. Values and `fieldKey` order exactly match `catalog.json`. Previous/Next consistent; Previous disabled on page 1. |
| **UI-03** Objectives/lessons | **PASS** | Sidebar groups: Objectives **9**, Objectives · steps **48**, Teaching sequence **8**. Walked **79/79** pages by real *Next* clicks with **0** title/number/field mismatches. Read-only meta showed all **48** protected step keys and **8** teaching IDs. **0** editable controls hold a protected key; IDs appear only as text. |
| **UI-04** F1 / Controls | **PASS** | Walking HUD F1 textarea contains the literal `{{keysLine}}`, labelled "Required parameter tokens: {{keysLine}}"; no invented key list. TreeInspection variants: `default / first-use / full-F1`. Controls page: 38 fields. (Missing Walking HUD title: F-09.) |
| **UI-05** Search | **PARTIAL** | Search finds **edited-only** text ("líne 2") and **original replaced** wording. Clearing restores `Pages · 79`; Next moves within results. **Defect:** the scenario label is part of every page's search text, so `sitka` returns **79** pages when only 5 contain the word, and `plantation` returns 79 instead of 7 (F-05). `regeneration` correctly returns 10. |
| **UI-06** Variants | **PASS** | *Create your first rack* (`rack.create`): options `legacy-or-fallback` and `enlarged80-rack-v2`. Legacy view = `[default, legacy-or-fallback]`, V2 view = `[default, enlarged80-rack-v2]`, and the bodies differ. Main StandMap help title stays *Stand Map · find where to look*; candidate *Stand Map* never appears as main. Capture `UI-06-rack-create-v2-variant.png`. |
| **UI-07** Edit and diff | **PARTIAL** | Edited title plus multi-line body (Ctrl+End, Enter, typed line): both flags **EDITED**, global `2 locally edited fields · not approved`, comparison keeps the original and shows the draft including the newline. **Defect:** the sidebar's per-page "· N edited" marker doesn't update while typing, only after the next navigation (F-06). Capture `UI-07-edit-and-diff.png`. |
| **UI-08** Revert | **PASS** | Revert field restores the original (3→2 edits). Revert page shows confirm *"Revert 1 changed field(s) on this page to the audited source text?"*: **dismiss** keeps the edits, **accept** reverts only this page (2→1) with notice *"Current page restored; other edited pages preserved."* The edit on p2 survives. |
| **UI-09** Export and reimport | **PASS** | Ctrl+S downloaded `ccf-text-draft-2026-11-14.json`: `authoringStatus=draft-local-export`, `revision=2`, `unityIntegrated=false`, `reviewed=false`, provenance and pages identical to the seed. Browser **reload** → `No edits · source baseline`. Import via the file chooser → *"Imported export-1.json; 2 edited fields restored."* Text is byte-exact after reimport, and re-export gives `revision=3`. |
| **UI-10** Unsafe imports | **PASS** (with F-02) | With unsaved edits present, **24/24** unsafe files were rejected with the draft intact: corrupt JSON (*"Unexpected token 'o'… is not valid JSON"*), >3 MB (*"File is too large (3 MB maximum)."*), HTML in a duplicate entry ID, img/onerror, `</textarea><script>`, provenance/reviewed/approved/baseline/step/order/branch/note tampering, duplicate or missing ID, all token errors, blank, schema, revision, control character, >10,000. **`window.__xss` never set; 0 injected `img`/`svg` nodes.** A tag-free `<svg/onload=…` imported and rendered as inert text. **Bypass confirmed in the browser:** importing B01 asks *"Replace your current unsaved draft…?"* (dismiss keeps the draft), and after accepting, the exported file still carries `approved=true, integrated=true, approvedBy="Manager #424"` (F-02). Capture `UI-10-xss-id-rejected-after-last.png`. |
| **UI-11** Unicode and format | **PASS** | `Athbhreithniú · Bliain {{year}} — “réidh” & O’Brien’s → ✓ C:\temp\n literal⏎líne 2: Fadó, Tá an choill beo.` and a body with ‘single’ “double” quotes, `&`, arrows, a backslash, a tab and a blank line. Validate PASS. **Byte-exact** in the exported JSON and after reimport; all 266 other entries unchanged. |
| **UI-12** Accessibility/layout | **FAIL at 1280×720** | Every textarea, input and select has an associated `<label>` (0 unlabelled). Tab order is logical, with a 3 px solid `:focus-visible` outline. Alt+→/Alt+← and Ctrl+S work. At 560 px: no horizontal overflow, single column. `prefers-reduced-motion` honoured (scroll-behavior auto, transitions 0 s). **Defect (F-01):** at a **1280×720 viewport** the sidebar box is 644 px but its content is 1,003 px with `overflow-y: visible`. The page list starts about 890 px down, and **no page button was clickable at any of 131 page-scroll positions combined with list scrolling**. The coverage panel is also clipped. At **1920×1080** it works, but the page list gets only **~121 px** (about 2½ items). The headed window (1128×947 inner) could reach the buttons. Captures `UI-01-launch-1280x720.png`, `UI-12-sidebar-720p-scrolled-bottom.png`, `UI-12-narrow-560.png`. Alt+Arrow is also captured inside textareas (O-02). |
| **UI-13** Status separation | **PASS** | Scenario Two shows title only, *"NO SHIPPED TEXT YET. This chapter has no authored catalogue entries…"*, 0 fields. All 8 non-O1 entries are labelled `NO SHIPPED TEXT YET` (compact O1 = `draft-no-text`). Status filter `unmerged-candidate` → 1 page whose source is `feature/scenario-one-objective1-ux-repair-20261112-v1 …`. Capture `UI-13-scenario-two-empty.png`. |
| **UI-14** Failure handling | **PASS** (O-08) | Removing `{{year}}` → *"Export blocked: fix validation errors first."* and **no download** (count unchanged), for both the button and Ctrl+S. A field of 10,001+ characters → *"Over 10,000 characters: …"*. An invalid import never changed in-memory data (UI-10). **Observation:** a reload with 2 unsaved edits produced **0** `beforeunload` prompts, so edits are lost silently (F-07). Capture `UI-14-validation-blocked.png`. |
| **UI-15** Practical use | **PASS** | Dense Controls page (38 fields) renders. Longest field (`s1.help.walkinghud.explanation.full-F1`, 3,716 characters): rows=12, scrolls internally (978 px of content in a 284 px box), vertical resize. On the last page `79 / 79` Next is disabled and clicking it does nothing. Status filters: `main-source` → `Pages · 78`, page `1 / 78`; candidate → `1 / 1`. Captures `UI-15-controls-dense-1920x1080.png`, `UI-15-long-field-1920x1080.png`. |

**Summary:** 11 PASS · 3 PARTIAL/PASS-with-finding (UI-05, UI-07, UI-10) · 1 FAIL at 720p (UI-12). **0 NOT RUN.** One engine only; Firefox NOT RUN.

---

## 5. Source coverage, provenance and honesty

- **Blob anchors:** all **28** `source-map.json` Git blob hashes match the pinned checkout (audit `verified: 28, changed: 0, missing: 0`). These files are unchanged between `main` `fc18190` and the candidate. My recount of the lexical counters reproduces **2,934** quoted-literal candidates and **225** `$"` starts exactly, per file. Only **7 of the 28** mapped files contribute catalogue entries; the rest are read-only anchors.
- **Text accuracy (independent C# literal decoder, `scratchpad\trace.mjs`):** for **265/265** main-source entries every `baselineText` fragment occurs in the cited file's decoded C# literals (238 exact literal matches at the cited line ±3; the rest are concatenated or templated). **No copied-text errors were found.** The 3 candidate entries are absent from main, as expected, and **match the current #443 branch HEAD `acb4e11…`** word for word (`LearningObjectivesView.Curriculum.cs:102`, `MenuHelpView.Text.cs:41,64`).
- **Line provenance (`scratchpad\strict.mjs`):** **24 of 265** main entries cite the wrong line (the text is right). Examples: all F1 `summary first-use` and `explanation full-F1` entries cite the `Title()` switch lines 40–45 instead of 60–66 and 93–122. Annual Review entries cite 132–158 instead of 124–148. Work Plan entries cite 120–124 instead of 128–132. The five teaching `practice` entries are 4 lines early (F-08).
- **Variants and protected IDs:** `rack.create` and `rack.map` each have `legacy-or-fallback` and `enlarged80-rack-v2` bodies with full text; the former blank-V2 defect is fixed. 48 unique `protectedStepId`, 8 `protectedLessonId`, no duplicate `pageId|fieldKey|variantId`. **Missing default:** the Walking HUD help title *"Walking HUD · your forest at a glance"* (`MenuHelpView.Text.cs:46`) is absent, while the other 6 help pages include their title (F-09). Leaving out a Walking HUD and Annual Review first-use summary is **correct**, because C# returns `null` for both.
- **Tokens:** `{{keysLine}}` is honestly a token. Note that its runtime value depends on the profile: `MenuHelpView.cs:130` derives the Rack V2 keys line by applying `.Replace()` to the legacy `ControlsReference.KeysLine` (`ControlsReference.cs:20–22`). Generated Annual Review and Work Plan headings use named tokens only; no invented values.
- **Candidate provenance:** the #443 entries record the branch name only, with no commit SHA, so drift on that moving branch can't be detected (F-12).
- **Coverage claims:** README, research doc, checklist and UI all say "representative / not all Unity text", and no "every text" claim was found. The machine-readable `coverage.classifications` does store `serialized: 0, unlocated: 0, outOfScope: 0`. The research doc calls these placeholders, but the stored value reads as a measured zero (F-10).
- **Serialized scan:** the only build scene (`ProjectSettings/EditorBuildSettings.asset` → `Assets/Scenes/ForestTest.unity`, 530 KB) has **0** serialized text-field candidates. Only the non-build `ForestryAssetReview.unity` reports 11. The UI is code-built, so the serialized gap appears small, but prefab, UXML, ScriptableObject and baked-art text remain **unaudited**. The scanner looks for C# only under `UI/` and `ScenarioOne/` (88 unmapped there) and would silently skip files over 4 MB (none today) (O-07). **The audit is a drift and candidate scanner, not a census, and the docs say so.** Full coverage stays **UNKNOWN**, not zero.

---

## 6. Numbered findings

Severity: **BLOCKING** none · **MAJOR** 2 · **MINOR** 11 · **OBSERVATION** 10.

### MAJOR

**F-01 — The sidebar page list and coverage panel can't be reached at a 1280×720 viewport.** `Tools/ContentReview/styles.css:3` (`.sidebar{…max-height:calc(100vh - 76px);position:sticky;top:76px}` with no `overflow-y`). *Steps:* open index.html at a 1280×720 viewport and try to click any page in *Pages*. *Observed:* the sidebar content (1,003 px) overflows its 644 px box, the first page button sits at about 889 px, and no page button can be hit at any scroll position (131 tested). *Expected:* every page can be selected at the packet's representative 1280×720 size. At 1920×1080 the list works but shows only ~121 px (about 2½ rows) under nine scenario buttons. Navigation is still possible with Previous/Next, Alt+arrows and search, which is why this is MAJOR and not BLOCKING. *Fix direction:* make the sidebar or the scenario list scroll (or collapse the scenario list) so the page list gets the remaining height. *Owner:* tool repair writer.

**F-02 — The validator accepts unknown package-level and entry-level keys, and approval-looking flags survive validate→export.** `Tools/ContentReview/core.js:15–55` (no key allowlist). *Steps:* import fixture B01 (seed + `"approved":true,"integrated":true,"approvedBy":"Manager #424"`), then export. *Observed:* import accepted; the exported draft still carries all three flags next to the genuine `authoringStatus:"draft-local-export"`. Entry-level `approved`/`reviewedBy` (B02) are accepted too. *Expected:* `plan.md` Stage B.2 promises "strict import constraints" and the packet requires that tampered approval/integrated flags are rejected. A draft should never carry approval-looking fields the tool ignores. The tool itself never acts on them, so they don't grant approval in practice; the risk is a misleading artefact reaching human reviewers or a future importer. *Fix:* closed allowlists for top-level keys (`schemaVersion, packageId, revision, provenance, scenarios, pages, entries, coverage, authoringStatus, exportedAt`) and entry keys (`id, pageId, fieldKey, variantId, text, baselineText, required, source, status, protectedGameplayKey, format, tokens, note`), plus a Node test. *Owner:* tool repair writer, with a matching spec rule (S-01).

### MINOR

**F-03 — Reordering the entries array is accepted, so field order on a page changes.** `core.js:34–41` matches entries by ID only. B03 swaps `entries[0]` and `[1]` → accepted, and page `s1.intro.p1` shows `lead` before `title`. Spec §2 says the tool "preserves page/field order". *Fix:* compare entry ID order with the seed.

**F-04 — Invisible, bidi and C1 characters are accepted, contrary to spec §4.** `core.js:46` rejects only C0 controls and DEL. B04 (`U+200B U+202E U+202C U+FEFF`) and B05 (`U+0085 U+009B`) are accepted. *Fix:* also reject C1 controls, zero-width characters, bidi embedding/override/isolate characters, `U+2060–2064` and `U+FEFF`, or change the spec wording on purpose.

**F-05 — Searching for words in the scenario name matches every page.** `app.js:21` adds the scenario label (and page and entry IDs) to each page's search text. `sitka` → 79 pages (5 real), `plantation` → 79 (7 real), and `scenario`, `one`, `s1` behave the same way. *Fix:* leave the scenario label out of the per-page text, and consider whether IDs should be searchable by default.

**F-06 — The sidebar's per-page edited marker is stale while typing.** `app.js:116–122` updates the field flag, global count and comparison, but not the page list. After two edits the active page still reads `main-source` until the next navigation.

**F-07 — No `beforeunload` guard, so unsaved edits are lost silently.** `app.js` (no handler). A reload with 2 edits produced 0 prompts. The download is the only persistence path, so one accidental Ctrl+R or tab close discards a whole editing session. *Fix:* register `beforeunload` when `totalChanged() > 0`. (MINOR because the README states that edits live in memory; recommended before real owner use.)

**F-08 — 24 of 265 main-source entries cite the wrong source line** (the text is correct). Listed in §5; examples: `s1.help.treeinspection.explanation.full-F1` cites 40, actual 106; `s1.annual.review.workColumn` cites 156, actual 146; `s1.teaching.access.practice` cites 35, actual 39. The editor shows these lines to the owner as provenance. *Fix:* re-extract line numbers with a literal-aware pass.

**F-09 — The Walking HUD help title is missing** (`MenuHelpView.Text.cs:46`, *"Walking HUD · your forest at a glance"*), although the other six help pages include their `title`.

**F-10 — `coverage.classifications` stores `serialized/unlocated/outOfScope = 0`** (`catalog.json`). The research doc (line 30) says these counters are unknown placeholders, but a machine reader sees a measured zero. *Fix:* `null` or `"unknown"`. Note this changes protected `coverage` and needs a new seed revision.

**F-11 — The committed Node suite (20 checks) doesn't reproduce the categories in the author's "30 V8 checks"** (see §3.1). *Fix:* commit negatives for schema, revision, authoringStatus, status, scenarios, page order/scenarioId, baselineText, entry add/remove and >10,000 characters, plus the F-02, F-03 and F-04 cases.

**F-12 — Candidate (#443) provenance has a branch but no commit.** `catalog.json` `provenance.comparisonCandidateBranch` and the three entries' `source.branch`. The #443 HEAD is now `acb4e11…`; its wording still matches, but any future drift won't be detectable. *Fix:* record the candidate commit SHA.

**F-13 — The validation summary shows "undefined entries / undefined pages / undefined edited fields"** for early-exit failures such as an unsupported schema. `app.js:150` reads counts that `core.validate` doesn't return on that path (`core.js:20`).

### OBSERVATIONS (no action required for the prototype verdict)

- **O-01** The markup regex rejects legitimate prose such as `DBH <b and c>`. It's conservative and safe for V1, but document it.
- **O-02** Alt+←/→ is captured even inside textareas (`app.js:204–207`). On macOS, Option+Arrow is word-by-word cursor movement, so Mac editors lose that behaviour (inferred from the code; no Mac was tested).
- **O-03** The 3 MB limit is counted in UTF-16 characters *after* `file.text()` reads the whole file (`app.js:191`). Checking `file.size` first would be cheaper and byte-accurate.
- **O-04** The untouched seed always gives one warning (Walking HUD explanation, 3,716 characters > 3,000), so the owner sees "1 warnings" on clean data.
- **O-05** `revision = MAX_SAFE_INTEGER` imports, exports as MAX+1, and that export then fails to reimport. Trivial edge case.
- **O-06** Help pages appear in the sidebar under enum names (`WalkingHud`, `TreeInspection`) rather than the titles players see.
- **O-07** `source-audit.mjs`: the text mode truncates lists at 30; C# is scanned only under `UI/` and `ScenarioOne/`; files over 4 MB are skipped without being reported.
- **O-08** The side-by-side comparison sits below all the fields. On dense pages (38 fields) you have to scroll far from the field you're editing to see the diff.
- **O-09** Scenario `s1.profile` doesn't name its case ID (`O1`), while `s2`–`s8` carry `O2…O8`.
- **O-10** `tasks.md` D-03 still says "PENDING FINAL HANDOFF" and checklist E-04 is unticked, although #473 is closed. This is expected staleness from writing before closure.

---

## 7. Four-file DRAFT specification assessment

I read all four files in full: `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`. **All four are marked DRAFT / NOT APPROVED** at the top. This review doesn't change that.

| Packet question | Assessment |
|---|---|
| Separates editable display strings from immutable game identity (`CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, teaching keys, rack/tree/order/save IDs, Reference, persistent flags)? | **Yes.** Spec §3 "Protected keys" lists them as *never* content IDs; pages hold `protectedStepId`/`protectedLessonId` as read-only metadata (verified in the browser, UI-03). |
| Are source branch, provenance, candidate vs shipped and approval semantics unforgeable by a downloaded draft? | **Partly, and honestly so.** Spec §3 correctly says no `reviewed` flag, commit name, timestamp or export can grant approval, and that approval comes from a Manager-reviewed Git commit plus later controlled import metadata. **Validator limit:** "unforgeable" really means "cannot change relative to the bundled seed". There's no signature, so anyone can hand-write a file that passes, and (F-02) unknown keys pass through. **S-01:** state a closed-schema rule and that integrity is relative to the bundled baseline, not proof of authenticity. |
| Are the placeholder contracts safe? | **Mostly yes.** §4: ordered, repeated named tokens; no eval/HTML; money, units and plurals stay in C# or a typed formatter; `textContent`; reordering needs a new contract. **S-02:** the spec doesn't cover strings C# *derives* from other strings (`KeysLineFor` applies `.Replace()` to the legacy constant, `MenuHelpView.cs:130`) or *composes* (`TreeMarks` explanation = `string.Join(TreeMarkEntries)` + constants). Editing such a source string can silently break the derived variant, and a flattened catalogue entry can't map back. Add a rule that derived or composed strings are split into their parts or marked non-editable/derived. **S-03:** align §4's invisible-character rule with the validator (F-04). |
| Do the Unity adapter call sites and fallback preserve Legacy40, Enlarged80/Rack V2, Reference, lesson progress, first-use F1 and Work Plan approval? Is the runtime vs Editor import lifecycle clear? | **Yes, at DRAFT level.** §5 lists the exact initial files, falls back to the original literal or generator on any mismatch, never gives an old world a V2 lesson, never migrates saves, and uses a build/Editor reimport with no runtime hot-reload. **S-04:** specify *partial-package* behaviour for variant pairs, e.g. what happens if an approved package overrides `legacy-or-fallback` but not `enlarged80-rack-v2`. Atomic per-page/variant-set fallback is recommended to avoid mixed copy. No C# is authorised. |
| Eight-case teaching order and case IDs independent of chapter order? Compact O1 DRAFT only? | **Yes.** §5: O1→O2→O4→O3→O7→O6→O5→O8, with `scenarioId`, page order, world identity and save slot kept independent. The catalogue's `s2…s8` profiles match. Compact O1 has no text and no save identity (verified, UI-13). |
| Gaps left honest (source completeness, serialized/art text, generated annual messages, unmerged #443)? | **Yes.** §8, the checklist C-06…C-11 and the research doc keep them as UNKNOWN or open. Exception: F-10 (stored zeros). **S-05:** spec §3 lists the status vocabulary `main-source / unmerged-candidate / draft-no-text / reviewed-not-integrated`, but the catalogue uses `no-shipped-text` for `s2–s8`, which isn't in that list. Align them. |
| Ownership, no-Unity and Windows-build gates, non-author review, future 720p/1080p tests and user acceptance kept distinct? | **Yes.** spec §6–§7 (A vs B1–B4), plan Stage E/F entry gates, tasks U-01…U-04 and checklist D-01…D-09 keep them separate. |

**DRAFT-quality verdict: NEEDS CHANGES (minor, bounded): S-01 to S-05 plus the stale status rows (O-10).** The architecture (versioned JSON, a later typed read-only adapter, exact literal fallback, Git review as the approval authority) and the evidence boundaries are sound and well argued. No structural rework is requested. I'm not settling any OPEN product question and not promoting anything to APPROVED.

---

## 8. Recommended next claim (for #424 to decide; nothing started)

**One bounded repair claim, one writer, a new branch off the pinned candidate** (or off `main` plus the candidate, as #424 directs), limited to `Tools/ContentReview/**` and the four `specs/ccf-cross-scenario-text-editor-v1/**` files:

1. Tool: F-01 (sidebar scroll at 720p), F-02 (closed key allowlists), F-03, F-04, F-05, F-06, F-07, F-13; re-extract line numbers (F-08); add the Walking HUD title (F-09); coverage `unknown` (F-10) and candidate SHA (F-12) through a seed revision with matching `catalog.js`; commit the missing Node negatives (F-11).
2. DRAFT: S-01 to S-05; refresh the stale task/checklist rows. Keep the status **DRAFT**.
3. **Re-check, not full re-review:** a non-author reviewer re-runs `tests.mjs`, the adversarial pack (`B01–B05` must then reject), UI-05/07/12/14 at 1280×720 and 1920×1080, and a regression smoke of UI-09/10.

**No Unity, no adapter, no production source.** The gated stages after that (#424/#425/user) are unchanged: a source-coverage census (R-03), #443 copy reconciliation (G-01), spec approval (G-02), and only then a separately claimed U-01 adapter.

---

## 9. Administrative status

- Dashboard and Drive mirrors: **NOT VERIFIED**. Publishing this report on GitHub doesn't sync any mirror.
- `active-tasks.md`: read only; not edited (Manager #424 is the sole writer).
- Author branch `c5461948…`: unchanged, verified before and after.
- Reviewer branch comparison against `main` `fc18190…`: expected **1 added Markdown file, 0 other changes**. The actual readback is recorded in the #474 handoff comment.
- Review completion means **executed and handed off**. It is **not** integration, feature acceptance or product approval.
