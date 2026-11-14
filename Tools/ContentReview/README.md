# CCF Text Editor — standalone browser prototype V1

**Status:** IMPLEMENTED STANDALONE PROTOTYPE / NOT UNITY-INTEGRATED / NOT USER-ACCEPTED. The companion four-file `specs/ccf-cross-scenario-text-editor-v1/` package remains **DRAFT** and cannot authorise Unity implementation.

This is an offline, dependency-free content review tool. It reads a source-extracted Scenario One JSON catalogue without touching the game, its saves, Unity Editor, PlayerPrefs, or GitHub. Edits stay **in browser memory** until you explicitly download a revised JSON file. It does not silently use localStorage. A download is not a change to CCF source.

## Open without Unity

1. In a local **Git checkout of this task branch**, open `Tools/ContentReview/index.html` in a browser. The included `catalog.js` exposes the audited seed without a fetch request, so ordinary `file://` loading is intentionally supported. No Node server, Unity, build tool, cloud service, CDN or account is required for the editor itself.
2. If your browser blocks local scripts, from the `Tools/ContentReview/` directory run `python3 -m http.server 8765` and browse to `http://localhost:8765`; this is optional, and serves files only from that directory. Do not expose the server beyond the local machine.
3. Choose **Scenario One**. The left list offers 79 source pages and 269 text fields (of which 3 are explicitly unmerged candidate copy), not a claim of all displayed game text. The seven future scenario entries and one compact Scenario One draft are **empty placeholders**.
4. Choose a page, edit the field's textarea, inspect **Original versus draft**, and use **Previous/Next** for sequential review. Use **Conditional display variant** to inspect old/general versus Enlarged80 Rack V2 source variants without editing protected lesson IDs.
5. Search all imported text in the left search box (matches are found across scenarios, not only the selected one). You can filter page source statuses. Changed fields and the global unsaved count are visible; **Revert field** or **Revert this page** restores the audited source text.
6. Click **Validate**, correct errors, then **Export draft JSON**. Keep the downloaded `ccf-text-draft-YYYY-MM-DD.json` safe; it is the only deliberate persistence path. Import that file later with **Import JSON**. Imports validate against the bundled baseline and will reject changed protected IDs, source provenance, pages, token signatures, or unknown schema version.
7. To hand an edit to Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424), commit or attach the exported JSON on an **approved review branch/issue**, and state which pages changed. The file is **not automatically installed** into the game. No change becomes approved copy just because it was exported.

**Shortcuts:** Alt+Left/Alt+Right for page navigation; Ctrl+S or Command+S for a validated JSON download. Buttons/labels are keyboard-focusable with visible focus outlines. The layout reflows below 900px; dense-editing usability is primarily desktop-oriented. Accessibility and browser rendering have **not been independently qualified**.

## Files and data contract

- `index.html`, `styles.css`, `app.js`: browser UX. Edited strings appear only through DOM `textContent`, `textarea.value`, or text nodes. User data never becomes an HTML template.
- `core.js`: portable schema and integrity validator; checks version, duplicate content IDs, immutable scenario/page/reference/field metadata, required copy, placeholder order/count, control characters, string length, and unsupported markup.
- `catalog.json`: human-readable versioned audited seed. `catalog.js`: matching local-file compatible JavaScript seed. Keep their parsed structures exactly identical when updating the audit. `baselineText` is a source comparison anchor, not an approval stamp.
- `source-map.json`: audited source paths, main Git blob identity, quoted-literal candidate counts and interpolation candidate counts. These counts are **not rendered text coverage**.
- `source-audit.mjs`: read-only source drift and serialized-text-candidate scan against a local full CCF checkout. It does not modify or compile the repository.
- `tests.mjs`: dependency-free Node tests against the content contract and negative cases. **Requires Node 18+ for tests only**, not for use of the editor.

**Known coverage:** three introduction pages (including real titles, lead, footer, cards/labels), nine Objectives topics, 48 saved-step records, eight teaching sequence lessons, first-use/F1 screen help and controls, selected Work Plan and Annual Review content, and three explicitly unmerged #443 candidate records. Generated Annual Review/Work Plan fields use explanatory `{{tokens}}`; their runtime values remain authoritative C# responsibilities.

**Not exhaustively extracted:** detailed Work Plan task/quote messages, many Annual Review history/forest/storm lines, all context/tooltips, every Stand Map/treatment/inspection message, startup/session menu, dynamic status/counters, scene/prefab/asset text, art-baked lettering and every conditional or generated formatter. These are *audit backlog*, not empty language entries. Only checked-in `main` and the separately labelled unmerged Objective 1 candidate were inspected. No later scenario has shipped text in this starter catalogue.

## Validation and source drift

From a checkout with Node installed:

```sh
node Tools/ContentReview/tests.mjs
node Tools/ContentReview/source-audit.mjs .
node Tools/ContentReview/source-audit.mjs . --json
```

The test script and source audit were **not executed under Node in this browser-connected worker environment**. They are provided for a local/code worker to run and record real results. **Original-author thirty direct V8 checks of the checked-in `core.js`, current catalogue, `catalog.js` mirror and `app.js` passed** in the GitHub tool environment after repair of two empty Rack V2 variants; see the durable handoff. No browser click-through, visual screenshot or Unity run has been asserted.

The source-audit command needs a **real full repository checkout**; it compares blob hashes for 28 targeted C# files, reports changed or absent sources, lists unindexed C# files under UI/ScenarioOne, and notes potential literal text in relevant serialized files. Changed source is a **stop and re-extract/review signal**, never a request to mechanically replace C# strings. This audit deliberately produces candidate counts rather than a false “all strings found” percentage.

## What a later approved Unity adapter would do

The DRAFT spec proposes a separately approved, typed read-only content loader that consumes a validated catalogue bundled with the game, resolves `scenarioId + pageId + fieldKey + conditionVariant`, interpolates only **explicit named placeholders** using game-provided typed values, and falls back to the exact old C# literal when a key is absent or invalid. Initial adapters would touch `ScenarioOneOnboardingCopy`, `LearningObjectivesView.*`, `ScenarioOneTeachingSequence`, `MenuHelpView.*`, `ControlsReference`, then read-only UI call sites such as Work Plan, Annual Review and Stand Map in separately owned slices. The editor **does not contain that loader** and is not a Unity importer.

Production work requires approved four-file spec, independently reviewed editor/schema/source audit, Manager #424 source ownership, candidate reconciliation with #443, preserved `CCF.Learning.v1.*` / `CCF.MenuHelp.v1.*` / lesson/save identities, and native Unity 1280×720 / 1920×1080 profile/Legacy40/Reference Future checking. No saved-game or PlayerPrefs schema change is proposed. Do not claim a text round-trip proves the displayed game copy is correct.

**Browser compatibility tested:** NOT RUN / not attested. **Node test script executed:** NOT RUN. **Original-author direct V8 content-contract checks:** PASS (30/30), historic baseline. **#477 repaired source suite:** 36/36 checks executed under V8 with Node API shims; the committed Node script must be run by a Node-equipped reviewer for independent evidence. **Unity import, compile, runtime, rendered check:** NOT RUN. **User copy approval:** NOT GIVEN.


## Repair revision 2 (14 November 2026) — #477

The first independent browser test (#474, Edge 157) found two MAJOR issues: an unreachable page list at 720p and a validator that ignored unknown approval-looking JSON keys. This repair adds independently scrollable sidebar scenarios/pages, closed recursive metadata allowlists and immutable entry order. It also checks C1/zero-width/bidirectional controls, refreshes the live edited page count and default search, warns on **unexported** edits before tab close/reload, and shows numeric counts even on early validation failure. Real browser confirmation of these changes remains a separate focused non-author review gate.

The source seed has **79 pages and 269 fields**: one additional source-backed Walking HUD title, corrected original-source line anchors, the exact pinned unmerged Objective 1 candidate commit, and `null` for source coverage categories that were not enumerated. `catalog.js` mirrors `catalog.json`. Exported drafts from the previous 268-entry seed are **not import-compatible** with revision 2 and need a manual, reviewed reapplication of intended text changes. The tool cannot certify game-copy approval, and its validation proves only structural identity against its bundled source seed, not cryptographic or Manager approval.

**Search:** default search indexes page headings/sections and wording only, excluding repeated scenario names and technical IDs. **Draft safety:** an export or successful import becomes the last saved snapshot for the browser's native unload-warning logic. Editing after that export re-enables the warning. Text still appears as original/edited until it is reviewed; the editor never auto-commits or autosaves.

**Closed schema:** known package, provenance, scenario, page, entry, source and coverage keys only. Unknown approval-like keys (including nested fields) are rejected rather than passed through into exports. Invisible/bidi/C1 control characters are rejected while ordinary Irish Unicode, punctuation, line breaks and tabs are preserved. A later Unity adapter must treat coupled legacy and Rack V2 variant families atomically and keep C#-composed `KeysLineFor` / Tree Marks wording separate from writable flat replacement; **not part of this standalone implementation**.
