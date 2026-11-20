# CCF blank scenario starter v1 — requirements and acceptance checklist

**Spec:** CCF-BLANK-STARTER-V1 · **Status DRAFT / NOT APPROVED / NOT IMPLEMENTED** · 20 November 2026. [spec](../spec.md) · [plan](../plan.md) · [tasks](../tasks.md) · [#856](https://github.com/jermur788/CCF/issues/856). **Actual native candidate: NONE. ALL below are UNCHECKED / NOT RUN.** Checkboxes are for independently evidenced native results, not a G0 document-quality claim.

## Functional / acceptance requirements

- [ ] **FR-01 / AC-01:** New Editor-only starter scene opens and Unity 6000.6.0f1 imports/compiles; flat y=0 30 × 30 m ground, collider, player spawn and correct walk/look, no trees/juveniles/clearing/roads/paths/racks/fences/structures/forestry work. Evidence: exact HEAD, scene hierarchy and screenshot.
- [ ] **FR-02 / AC-02:** Existing shared `ForestPlayer`, `ScenarioOneUiRoot`, original UI Toolkit views, map, menu and relevant manager/ecology code are **referenced**, never copied/renamed per scenario; same source/script GUIDs and no second inventory/ledger. Evidence: asset/meta/source comparison.
- [ ] **FR-03 / AC-04:** No active gameplay state or auto-year, time-lapse, work approvals, quotes, cash claims, tree spawning, ecology changes or fake outcomes; disabled controls cannot call mutating APIs. Evidence: method/click tracing and before/after world counters.
- [ ] **FR-04 / AC-02:** Common HUD/menu and static headings `Work Plan`, `Stand Map`, `Annual Review`, `Objectives` visible, true tree inspection unavailable. Evidence: readable native screenshots and actions.
- [ ] **FR-05 / AC-03:** Blank Stand Map shows real ground bounds and optional real player only, no ghost tree/stump/juvenile/rack/path/clearing/landmark/terrain data or rack help/edit/lesson. Evidence: map/minimap screenshots and source layer counts.
- [ ] **FR-06 / AC-04:** Work Plan states **No eligible operations in this blank starter.** No plan approve, quote, purchase, Fell, prune, work completion, revenue/cost/treasury as if live, or Year+1. Evidence: button and hotkey negative replay.
- [ ] **FR-07 / AC-05:** Annual Review states **No year has been advanced; no annual review is available.** Objectives says **No objectives configured (development preview).** No historic report, achievement, first rack lesson or completed objective from PlayerPrefs. Evidence: open/reopen with O and review navigation.
- [ ] **FR-08 / AC-06:** **Before file access**, save/load/Continue path refuses historic `Application.persistentDataPath/forest-save.json` via startup Continue/CanLoad, F10 menu Save/Load/File.Exists, F5/F9 and direct callers; original and `.tmp`/`.bak` never read or written. Evidence: actual pre-existing historic save, read-access trace, content hashes, mtimes, sidecar presence, session state.
- [ ] **FR-09 / AC-08:** Shared labels permanent; intro/objective/guidance placeholders DEV-ONLY; production content gate blocks missing required text; existing browser draft JSON not runtime content; year/cash/counts remain engine-only. Evidence: preview and production-gate witness.
- [ ] **FR-10 / AC-07:** Duplicate scenario-owned scene/config/text assets **only**, set 45 × 60 m **TEST-ONLY**, show correct rectangular ground **and** map, preserve original 30 × 30, original shared scripts/GUIDs and old ecology geometry. Evidence: two native screenshots plus exact copied-file diff.
- [ ] **FR-11 / AC-10:** Direct Editor scene-open safe and unlisted scene Restart does not load historic ForestTest; no build-list/Player/package change. Evidence: entry/restart test and ProjectSettings diff.
- [ ] **FR-12 / AC-09:** Historical `ForestTest` five-rack/Rack V2/MFM, v1–v24, Legacy40/Enlarged80, frozen Reference Future v1, saved old-world CanLoad/Save/Load and determinism remain compatible. Evidence: native targeted old-world positive and protected existing gate results on **same exact integrated candidate**.
- [ ] **FR-13 / AC-10:** Scene clearly labelled **Development preview — unsaved**, not an independent finished forest/Compact30/Multi 2D; no false implementation/integration/player-acceptance or Windows-build claim. Evidence: screenshot and exact handoff status.

## Negative case matrix — ALL NOT RUN

| ID | Attempt | Required refusal / observation | State |
|---|---|---|---|
| N-01 | No ecology cells / no active manager; open UI | Common screens render, no null exception, real empty ground bounds, no fake inventory | [ ] NOT RUN |
| N-02 | Map M, rack B/edit/confirm, tree marking | No rack editor/tool/marker/lesson or phantom tree | [ ] NOT RUN |
| N-03 | Tab, approve/advance, planting/Fell, time-lapse T | No year, work, treasury, ecology or event mutation | [ ] NOT RUN |
| N-04 | O/review, persisted first-use preferences | No first-rack lesson, Year 1, review acknowledgement or achievement | [ ] NOT RUN |
| N-05 | Historical save present, startup Continue | Disabled/refused BEFORE old file probe / CanLoad, no load or world mutation | [ ] NOT RUN |
| N-06 | F10 Save/Load, SaveAndClose and public Save/Load | Refused before capture or disk, preserve in-memory preview | [ ] NOT RUN |
| N-07 | F5/F9 while walking, menu-open, after close | No old-file read/write; no file created and no old forest loaded | [ ] NOT RUN |
| N-08 | Restart/new/start with unlisted Editor scene | Preview remains or action disabled; never loads ForestTest inadvertently | [ ] NOT RUN |
| N-09 | Original save and sidecars across all above | Byte hashes, mtimes, presence unchanged **and read-trace proves no reads** | [ ] NOT RUN |
| N-10 | 45 × 60 copied scene/config/text only | Rectangle accurately projected; no new saved geometry/copy of code | [ ] NOT RUN |
| N-11 | Required DEV text missing in production path | Refused as unready; no dev placeholder shown as final player fact | [ ] NOT RUN |
| N-12 | Original ForestTest on same candidate | Historical positive load/save and existing five-rack UI remain correct | [ ] NOT RUN |

**Why separate N-05…09?** [`ForestSaveController.Update`](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/ForestSaveController.cs#L21-L37) owns F5/F9; [`StandaloneSessionMenu`](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/StandaloneSessionMenu.cs#L116-L149) owns menu and File.Exists; [`Startup`](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs#L87-L155) calls CanLoad. A disabled button or unchanged save hash **does not prove no file read**.

## Source custody / independent review gates

- [ ] Independent **nonauthor** four-file design quality review documented. Its verdict is not product or code approval.
- [ ] #425 G4/owner material choices resolved (preview root with no active manager; rectangular map projection; direct editor/menu safety), and spec explicitly APPROVED by authority.
- [ ] #844 grants one exact shared UI/map/save source writer and separately disjoint scene/config assets or formally delegates entire shared seam; full transitive compile-linked allowlist separately recorded, original #820/#836 protected.
- [ ] Current active-tasks read, context commit locked, source/dirty/physical Ops preflight documented and legitimate serialized Unity lease taken and safely released.
- [ ] Tier 1 real Unity compiler/scene/UI early preview PASS; not mistaken for save or gameplay verification.
- [ ] Tier 2 positive/negative tests and old-world targeted regression PASS on exact final candidate; failures not relabelled green.
- [ ] Independent source review and Manager #844 integration verdict based on actual diff, hashes, native evidence, save-safe access traces and current provenance.
- [ ] Implementation, independent review, integration, full verification and owner acceptance separately recorded. No Windows build before owner's exact Compact personal Editor test and separate approval.

## G0 documentation checks (record separately in final GitHub report)

- [ ] Four new spec files DRAFT + one handoff on #856 exclusive docs branch.
- [ ] All files GitHub readback with exact blobs/lengths and exact `main@7758e17c882fad01970d2f2fb8c09586bbcab5c1...HEAD` diff exactly five ADDED Markdown.
- [ ] Source findings tied to pinned code, #855 corrected draft and owner per-scenario save decision without suggesting existing Unity implementation.
- [ ] #856, Overall #425 and Scenario One #844 notified with direct result URL and actual PASS / FAIL / NOT RUN.

**Native verdict at G0:** spec conformance **NOT VERIFIED**; implementation correctness **NOT VERIFIED**; historical save protection **NOT VERIFIED**; Unity rendered preview **NOT RUN**; native worker/lease **NOT STARTED**.
