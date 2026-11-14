# CCF cross-scenario text editor V1 — requirements and verification checklist

**DRAFT / NOT APPROVED.** Checkmarks below reflect only what has direct GitHub/source or V8 contract evidence. An unchecked item is **UNVERIFIED / not necessarily failed**. Source inspection is not proof of browser execution or Unity runtime. Do not accept a production adapter because an offline round-trip passed.

**Labels:** `[x] SOURCE` = inspected checked-in file/proven V8 contract; `[ ] BROWSER` = needs actual browser test; `[ ] UNITY` = later production gate; `[ ] AUTHORITY` = decision/review/ownership required; `[ ] COVERAGE` = truthful source completeness not established.

## A. Standalone editor source, catalogue, UX

- [x] SOURCE **A-01** — new `Tools/ContentReview/index.html`, `styles.css`, `core.js`, `app.js` exist on independent task branch; no external JS library/CDN.
- [x] SOURCE **A-02** — exact `catalog.json` + `catalog.js` parsed-data mirror, 79 navigable pages and 268 fields; current checked-in main vs three explicitly unmerged candidate fields.
- [x] SOURCE **A-03** — actual three-page introduction page content with source locations, titles, leads, footers and important detail cards.
- [x] SOURCE **A-04** — nine Objectives topics and 48 saved-step text records linked to readonly step IDs; eight teaching-sequence lesson pages; first-use/F1/help/controls and representative generated Work Plan/Annual Review templates.
- [x] SOURCE **A-05** — eight accepted chapter labels plus explicit compact O1 DRAFT profile; only current O1 pages contain copy; future chapters never invent lessons.
- [x] SOURCE **A-06** — sidebar and one-page-at-a-time Previous/Next, selected-page active indicator and sections are implemented in JS source.
- [x] SOURCE **A-07** — search scans edited/original fields across imported pages; source-status filter and condition/variant selector are implemented in JS source.
- [x] SOURCE **A-08** — editable textareas, readonly content IDs/source metadata, dirty field/global indicator, original/current side-by-side text comparison, revert field/page.
- [x] SOURCE **A-09** — explicit JSON file picker, schema validation on import, draft export with revision/unapproved state, no automatic Git/Unity/localStorage write.
- [x] SOURCE **A-10** — browser text values assigned via `textContent` / textarea value; no `innerHTML`, `insertAdjacentHTML`, network fetch or remote analytics in reviewed `app.js`.
- [x] SOURCE **A-11** — responsive CSS and keyboard Alt+Left/Right, Ctrl/Command+S, labelled fields/focus-visible and announcements are present in source.
- [ ] BROWSER **A-12** — open `index.html` in an actual browser via `file://`, verify no console errors, seed loaded with no network requests and all 79 pages accessible.
- [ ] BROWSER **A-13** — navigate 3 introduction pages → Objectives topics/steps → F1 help → Work Plan/Annual Review; verify exact text/ordering visually.
- [ ] BROWSER **A-14** — search all fields, toggle main/candidate status, inspect Rack V2 vs legacy variants, ensure original and modified fields render correctly.
- [ ] BROWSER **A-15** — edit Unicode/multiline/backslashes/quotes/arrows; verify original-versus-draft, dirty state and both revert scopes.
- [ ] BROWSER **A-16** — click export, save file, reimport into a freshly loaded browser and compare byte-equivalent user text; rejected import must preserve dirty work; warning/error display legible.
- [ ] BROWSER **A-17** — exercise page navigation/keyboard/focus/screen-reader labels, narrow viewport/mobile stack and reduced-motion preference; qualify real browser versions.
- [ ] BROWSER **A-18** — show candidate-only versus main source never silently changes during navigation, compare future scenarios without fake copy; no display text claims current shipped build unless proven.

## B. Portable content and integrity contract

- [x] SOURCE/V8 **B-01** — `schemaVersion = ccf.content.v1` parser checks arrays, source provenance, unique IDs, page/scenario references and source identity.
- [x] SOURCE/V8 **B-02** — original `baselineText`, scenario/page ordering, status, protected step/lesson keys, `source.path/branch/line/kind`, token signatures, field identifiers and formats are unchanged by text edit or accepted import.
- [x] SOURCE/V8 **B-03** — UTF-8/Unicode, arrows, apostrophes, `\n`, ampersand and backslash survive JSON serialization/re-import in equivalent V8 contract test.
- [x] SOURCE/V8 **B-04** — missing, extra and reordered named placeholder tokens rejected; malformed token patterns rejected in validation source.
- [x] SOURCE/V8 **B-05** — empty required text, duplicate IDs, control characters, and HTML-like markup rejected; very long text checked/warned.
- [x] SOURCE/V8 **B-06** — edited content exports to `draft-local-export` and always retains `unityIntegrated=false`, `reviewed=false`; no automatic approval.
- [x] SOURCE/V8 **B-07** — 30/30 directly executed V8 checks PASS on final repaired seed; an earlier invalid initial two-variant seed was corrected, not hidden.
- [ ] BROWSER/NODE **B-08** — run committed `tests.mjs` through actual Node from a checkout and preserve stdout/version/exit status.
- [ ] AUTHORITY **B-09** — approve a migration and content-ID/variant extension policy if source changes in future; prototype cannot generate approved new record identities.
- [ ] AUTHORITY **B-10** — establish exact typed plural/units and naming policy before expanding synthetic templates; currently C# maintains numbers, units and plural decisions.

## C. Research/coverage trace

- [x] SOURCE **C-01** — read 28 named main C# files and retain source Git blob anchors in `Tools/ContentReview/source-map.json`.
- [x] SOURCE **C-02** — recorded 2,934 quoted literal lexical candidates, 225 interpolated-string starts, and explicit caveat these are **not total player-visible strings**.
- [x] SOURCE **C-03** — catalogued source branch/line where available, baseline, condition variant and key protection; static main distinguished from unmerged #443.
- [x] SOURCE **C-04** — written architecture comparison of JSON/ScriptableObject/Unity Localization/C# replacement and source-safe recommendation.
- [x] SOURCE **C-05** — read-only drift-scanner code `source-audit.mjs` created, reports modified/missing source, unmapped UI/ScenarioOne C# and likely serialized text fields.
- [ ] COVERAGE **C-06** — run drift scanner on full repository checkout against exact pinned main; update source inventory with all new/changed paths and actual tested output.
- [ ] COVERAGE **C-07** — inspect all C# beyond sampled 28 files, Unity scenes/Resources/prefabs/UXML/ScriptableObjects, serialized UI texts and baked artwork; identify genuinely unlocated items.
- [ ] COVERAGE **C-08** — classify *each* player-visible string as collected/conditional/generated/serialized/unlocated/out-of-scope with source and measured denominator; currently **unknown**, not 0.
- [ ] COVERAGE **C-09** — reconcile all current source differences from #443/#450 and approved copy documents; no future catalogue can claim “shipped” until integrated candidate/runtime is observed.
- [ ] COVERAGE **C-10** — check candidate profile and later compact O1 copy independently, not by replacing rack count in five-rack instructions.
- [ ] COVERAGE **C-11** — future O2–O8 source/curricula and actual case ID/world/save interface verified when they exist; current placeholders only.

## D. Unity content adapter — NOT IMPLEMENTED OR AUTHORISED

- [ ] AUTHORITY **D-01** — independently reviewed DRAFT spec given explicit APPROVED status by correct owner/Manager; follow-up source claim and user content approval scope.
- [ ] AUTHORITY **D-02** — #424 reconciles #443/#450 source ownership and schedules one dedicated adapter source writer with actual branch/worktree/HEAD; no active writer or user Unity/save conflict.
- [ ] UNITY **D-03** — typed read-only approved asset lookup with fallbacks; no runtime untrusted JSON loading from arbitrary exported file.
- [ ] UNITY **D-04** — all saved IDs/preferences/teaching predicates and original Reference/Legacy40/Enlarged80 state unchanged; no new save or PlayerPrefs field; absence/malformed key falls back to exact C# literal.
- [ ] UNITY **D-05** — `ScenarioOneOnboardingCopy/View`, `LearningObjectivesView.*`, `ScenarioOneTeachingSequence`, `MenuHelpView.*`, `ControlsReference` migrated only for approved fixed display fields with exact old-world guards.
- [ ] UNITY **D-06** — generated Work Plan, Annual Review/history, map/inspection, treatment and completion text adapted only after typed-source audit; retain gameplay computation and provenance.
- [ ] UNITY **D-07** — live Unity 1280×720 and 1920×1080 screenshots, first-use, long Unicode text, keycaps, conditional source profile, fallback, old saves, no-work/current/earlier annual year; no unsupported font overflow.
- [ ] UNITY **D-08** — independent source/native verification: no save/progression/rack/tree/order/Reference hash drift, no scenario cross-routing, no hidden auto-work or approval behavior from text changes.
- [ ] AUTHORITY **D-09** — independent review verdict, Manager APPROVED integration, user content acceptance and explicit Windows build authorisation if any Windows build is later proposed. A DRAFT file cannot provide these.

## E. Handoff/admin correctness

- [x] SOURCE **E-01** — #473 actual worker and branch registered **before GitHub file edits**, no local worktree/Unity lease; live coordination read and left untouched.
- [x] REPO **E-02** — all 15 pre-handoff additions read back; branch compared with registered start; all files added and paths inside allowlist.
- [x] REPO **E-03** — final GitHub Markdown handoff published at exact packet path and read back; direct task/owner links included.
- [ ] REPO **E-04** — #473 marked `[HANDED-OFF]`/closed, not marked feature accepted/integrated.
- [ ] AUTHORITY **E-05** — independent reviewer and Manager #424 decide next gates; dashboard/Drive mirror synchronized only if a tool verifies it (currently NOT VERIFIED).

### Evidence statement

**Directly executed in this task:** 30 JavaScript V8 parser/content-contract checks, all passed after correcting two source-extraction errors. **Not executed:** committed Node test script, whole-repo drift scanner, local browser interaction/visual screenshots, Unity Editor/compiler/Player, save/load or scenario reference qualification. All future browser/Unity requirements remain unchecked notwithstanding static source implementation.

A requirements check can be promoted to PASS only with direct evidence from the right execution environment. No routine user question is needed to finish the standalone browser-tool handoff.



## #474 independent review repairs: additional evidence-gated DRAFT requirements

**DRAFT / NOT APPROVED.** Original checklist and earlier execution values are historical #473 source status only; #477 repair and later independent delta review are separate.

- [ ] **R474-01 (F-01 MAJOR):** Real Chromium/Firefox at **1280×720** can click first/middle/last page in sidebar, scroll both scenarios and pages, view coverage; repeat basic layout at **1920×1080** and narrow width.
- [ ] **R474-02 (F-02 MAJOR, S-01):** Unknown keys are rejected with path-specific errors at package/provenance/scenario/page/entry/source/coverage/classes levels; forged `approved`, `integrated`, `approvedBy`, `reviewedBy` never pass import or survive re-export. Bundled baseline equality is not cryptographic approval.
- [ ] **R474-03 (F-03/F-04, S-03):** Entry array order cannot change, C1/bidi/zero-width/BOM edits reject, ordinary Irish Unicode plus tabs/newlines pass; Node negatives actually committed.
- [ ] **R474-04 (F-05/F-06/F-07/F-13):** Search `sitka` and `plantation` returns actual matching pages instead of 79; live sidebar changed-count updates while typing without caret loss; only unexported edits trigger a native beforeunload prompt; malformed import errors show numerical counts, never undefined.
- [ ] **R474-05 (F-08/F-09/F-10/F-12):** Source-line audit corrects all reported mismatches, Walking HUD help title added, unknown coverage uses `null`, and pinned #443 candidate commit retained on three candidate records. Seed revision 2 has **79 pages / 269 fields**, same parsed data in `catalog.js`.
- [ ] **R474-06 (F-11):** Node suite includes all #474 missing categories; version, command, stdout and zero/nonzero exit actually recorded. Add separate real browser regression, do not call it Node equivalence.
- [x] **R474-07 (S-01…S-05 DRAFT drafting):** The revised spec states closed metadata, immutable source/composition, Unicode policy, atomic coupled-profile fallback and shared source-status vocabulary; still NOT APPROVED.
- [ ] **R474-08 (independent review):** Different non-author checks changed source and focused tests and provides separate **APPROVED / NEEDS CHANGES / REJECTED** verdicts for standalone tool and DRAFT quality.
- [ ] **R474-09 (Unity separation):** Any future production adapter requires new APPROVED spec, exclusive source/Unity ownership, current/Legacy40/Reference fallback qualifications and explicit acceptance. **No Unity code or save work in #477.**

**Historical status clarification:** original #473 HANDED-OFF/CLOSED, #474 HANDED-OFF/CLOSED with NEEDS CHANGES. #477 is the only current browser-repair execution claim. Dashboard and Drive sync NOT VERIFIED. No feature release has occurred.
