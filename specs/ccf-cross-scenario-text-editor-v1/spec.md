# CCF cross-scenario text editor V1 — feature specification

**Status: DRAFT — NOT APPROVED.** No gameplay, Unity content-adapter, save, scene, build or PlayerPrefs change is authorised by this file. The **standalone browser prototype** is implemented separately and can be reviewed without approving this specification. Task [#473](https://github.com/jermur788/CCF/issues/473); owner for Scenario One later source integration/release is [#424](https://github.com/jermur788/CCF/issues/424); overall product/story authority remains [#425](https://github.com/jermur788/CCF/issues/425) / user.

## 1. Authority, objective and non-goals

Accepted product intent: let the owner review and edit actual CCF text **outside Unity**, one page at a time, across separate independent forestry scenarios; preserve all game decisions, progression keys and save identity. The long-term eight-case teaching sequence is established under D-078/D-079 but only current Scenario One main copy is present in the initial catalogue. Preserve D-064/D-065 Rack V2/MFM and frozen Reference Future v1, Legacy40, D-071 parallel five-rack and every independent scenario identity; a DRAFT compact O1 is **not** the shipped scenario.

**Deliverable A — standalone editor (this task):** local/offline browser application, source-extracted starter catalogue, read-only source audit, draft import/export, field edit/revert/search/diff, deterministic schema checks; never writes to Unity or saves. Verifiable through static code, browser interactions and JSON round-trip.

**Deliverable B — Unity content adapter (later separately gated claim):** typed, read-only lookup/resolution of an *approved* content package at selected call sites, exact fallback when missing/invalid, profile-aware content, no save/state mutation, tested in Unity and reviewed independently. **NOT IMPLEMENTED or authorised.** A successful A test is not evidence for B.

**Not V1 goals:** full automatic extraction of arbitrary Unity UI strings, automatic code/string replacement, translating whole CCF into other languages, a cloud CMS/collaboration server, editing saved game identifiers, applying edits live to a running game, script injection/rich HTML authoring, editing text baked into images, future story generation or Scenario Two–Eight implementation, visual pixel parity to Unity.

## 2. Owner-facing review workflow (proposed acceptance)

- A locally opened `index.html` presents scenario sidebar and ordered nested page list (topics/steps). One page is editable at a time. Future chapters display **NO SHIPPED TEXT YET** rather than synthetic lessons. Current Scenario One baseline and unmerged candidate have distinct source statuses.
- The owner navigates **Previous/Next**, selects a topic/step, searches across all imported text (including source text and edited text), filters source status, and changes text in ordinary multi-line fields. Navigation is independent of runtime progression, avoids marking steps complete, and preserves page/field order.
- Each field displays a **non-editable stable content ID**, branch/path/line provenance, relevant immutable learning/teaching ID, its condition variant and required tokens. Only `text` is editable; readonly metadata is not presented as editable controls. A visible edited indicator and global unsaved count show changes immediately.
- A source-versus-draft comparison preserves original source text and line breaks and never silently overwrites the original. **Revert field** and **Revert page** are explicit. Importing over existing dirty work requires confirmation, and a failed import does not destroy the in-memory draft.
- A condition selector shows **all**, **legacy/general fallback**, **Enlarged80 Rack V2**, and any source-specific variant known to the page; the same content ID plus variant must not be silently reused for a different world. The selected variant is a *copy preview*, not a live evaluation of Unity predicates.
- A plain text preview/diff can show `{{year}}`/other named placeholder tokens; there are no invented model values. Screens with generated copy are annotated as templates or deferred. UI preview is expressly **not** pixel-perfect Unity.
- **Validate** reports actionable errors/warnings with field IDs; **Export draft JSON** is blocked on errors and always produces a new downloaded file with a draft stamp. The downloaded file does not automatically alter GitHub or the game. **Import JSON** from an explicit user-chosen file is schema- and source-baseline-validated.
- Keyboard: Tab/focus-visible, labels for inputs, Alt+Left/Alt+Right page sequence, Ctrl/Command+S export; reading order and status feedback to assistive technology. Desktop dense-text editing is primary; responsive columns stack and page list scrolls on narrow viewports. Native browser/assistive-technology qualification remains necessary.

## 3. Content identity and portable schema

The proposed portable schema `ccf.content.v1` is JSON UTF-8, typed **arrays** (suitable for later typed Unity serializable conversion), not an anonymous map keyed by game save IDs.

At package level: `schemaVersion`, stable `packageId`, non-authoritative `revision`, `provenance` (repository, source branch/commit, audit date, candidate comparison, not-integrated flag), `scenarios[]`, `pages[]`, `entries[]`, coverage summary and optional local-draft stamp. Source ID and content ID do not encode money, rack, tree, world, save slot, Work Plan job or achievement state.

`scenarios[]`: `id` (content namespace, e.g. `s1`), display name, `status` (`main-source`, `unmerged-candidate`, `draft-no-text` or future explicitly verified `reviewed-not-integrated`), profile/case reference. Do not conflate chapter position and original case ID O1–O8.

`pages[]`: immutable `id`, `scenarioId`, hierarchy `section`, display/navigation title, deterministic `order`, source status, optional `parentPageId`, immutable `protectedStepId` or `protectedLessonId`. Page ordering describes external review, **not** changed completion predicates.

`entries[]`: immutable `id` (proposed content key), `pageId`, `fieldKey`, `variantId` (including `default`, `legacy-or-fallback`, `enlarged80-rack-v2`, candidate-only source), mutable `text`, immutable `baselineText`, `required`, immutable `source: {path, branch, line, kind}`, explicit `status`, optional readonly `protectedGameplayKey`, `format: "plain-text"`, ordered token-signature array `tokens`, and explanatory `note`. *(DRAFT note, #487)* An entry may also carry an optional readonly `derived` object (`readOnly`, `composedBy`, ordered `parts` of `entryId` references or fixed `glue` with source line) for a C#-composed display string, as required by S-02: its `text` always equals `baselineText`, which must equal the composition of its constituents, and it is never an editable replacement.

**Versioning/authority:** drafts increment local revision; neither user-entered `reviewed`, commit name, timestamp nor JSON export can grant content approval. A Manager-reviewed Git commit and explicit approval metadata in a later controlled production import are required. Baseline source changes require re-extraction and traceable three-way review rather than a silent rewrite of `baselineText`/IDs. Duplicate IDs, changed page order, deleted entries, different branch provenance and changed token signatures must fail import. Allow new content IDs **only** through a reviewed catalogue/schema generation task, not the editor's text input.

**Protected keys:** `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, `ScenarioOneTeachingSequence` lesson keys, `rack.read/rack.create/rack.layout` etc., saved objective IDs, `RackSaveData.rackId`, tree/work/history identities, save versions/model flags, achievements and scenario-world router keys are **never external text content IDs**. Rename/reordering of display headings may be allowed after copy review; underlying game keys remain identical. Approval/printing of text must not call game-state mutation.

## 4. Conditions, generated text, placeholders and text safety

**Source condition is authoritative.** Browser variants are selected for *editing*, while later Unity resolution must take validated actual world facts: e.g. legacy versus Rack Planning V2, confirmed plan, mark count, annual-history state, first-use/help context, pending/approved/completed/failed work, observed tree/site state, current vs historical year and subsequent compact-specific version if approved. Never select by substring of displayed text, user-visible page number, saved ID string transformation, or assumed world profile.

**Dynamic tokens:** `{{name}}` tokens are immutable *ordered, repeated* named parameter references. Missing/extra/reordered tokens fail; raw `{{` or `}}` mismatches fail. Only a later typed adapter can provide values from existing C# authoritative computation. Values are not evaled, scripted, decoded as HTML, or read from untrusted arbitrary property paths. `{{year}}`, `{{latestYear}}`, `{{cashAfter}}`, `{{openCount}}` are illustrative. Placeholder order policy is intentionally conservative; a reordered but semantically valid sentence would need a separately reviewed adapter contract change rather than silent acceptance.

**Plural/number/unit accuracy:** maintain currency formatting, `m`, `m²`, `m²/ha`, percentages, DBH, decimal rounding, work-time representation, count/plural selection and sign conventions in C# or a reviewed typed formatter, never as editable arithmetic. `open task(s)` is an existing C# presentation shorthand, not a general pluralisation grammar. For future condition/plural variants use explicit variant entries and typed selector plus fallback; do not make the user author executable logic.

**Unicode and markup:** UTF-8; preserve Irish accent characters, Unicode arrows, en/em dashes, tabs (where allowed), apostrophes, quotes, backslashes and `\n` line breaks. No arbitrary rich/HTML markup in V1; browser uses `textContent` and textarea values. Reject invisible/control characters outside ordinary newlines/tabs, malformed tokens, tags, empty required copy, duplicate entries, text >10,000 chars; warn for >3,000 chars but final screen-specific length/wrapping requires native checks. Do not claim that a long string passing schema fits 720p. If actual UI Toolkit rich text is required in a later slice, define a narrow allowlist/escaping contract and test separately.

**Security:** local download only, user file picker for import; no online account/analytics/CDN/server required, no automatic filesystem overwrite, no localStorage canonical drafts, no arbitrary HTML insertion, no parsing/evaluating C# code. Untrusted JSON objects can carry strings but cannot edit source metadata or trigger game actions; imported content failing schema leaves the previous draft unchanged.

## 5. Source-to-Unity architecture — proposed, separate production gate

Use a single, **read-only** typed `CCFContentCatalogue` loaded at a documented build/Editor import point from an explicitly approved, versioned `TextAsset`/asset location. Avoid user-writable runtime file injection. In a local content-authoring workflow the owner exports JSON → independent review/copy approval → a content manager commits the approved package → a separately owned Unity adapter imports it → a normal Unity compile/render check confirms the display. No save schema or PlayerPrefs key additions, and no script reparsing of source C#.

**Initial lookup surface after approval:**
1. `ScenarioOneOnboardingCopy.cs` and `ScenarioOneOnboardingView.cs`: title/lead/renewal/footer/cards and missing live literals, with exact fallback and explicit render-time line-break handling. Preserve page sequence and completion callback.
2. `LearningObjectivesView.Curriculum.cs`, `LearningObjectivesView.Sequence.cs`, `LearningObjectivesView.cs`, `ScenarioOneTeachingSequence.cs`, `ScenarioOneRackBayText.cs`, `ActiveObjectiveHudWidget.cs`: lookup of *display strings only* while unchanged saved IDs, validation, lesson order and state determine what is displayed. Gate any dynamic string adapter behind fact-typed token validation.
3. `MenuHelpView.Text.cs`, `MenuHelpView.cs`, `ControlsReference.cs`: variant-specific help and first-use wording, preserve each `CCF.MenuHelp.v1.` seen flag, Rack V2/legacy controls, and one authoritative key-list formatting rule.
4. Later slices: `WorkPlanView.cs`, `WorkPlanOverview.cs`, `AnnualReviewView.cs`, `StandMapView.cs`, `StandMapTreatmentReview.cs`, `TreeInspectionView.cs`, `ScenarioOneTreatmentReview.cs`, `ScenarioOneCompletionGuidance.cs`, `ManagementHistoryText.cs`, session/menu/HUD labels and any serialized scene/prefab/asset strings. Adapt only proven authored text, retaining typed C# values, safeguards and provenance.

**Fallback:** key/profile mismatch, missing `TextAsset`, invalid package/token contract, unsupported legacy or old saved-world path → render the existing original literal or original C# generator for that call site, emit a non-blocking dev diagnostic, and do not mutate state. Never give an old world a current Rack V2 lesson. No auto-migration of old saves, historical progression or frozen Reference content. Changing a packaged approved content file should require a build/Editor content reimport and controlled test; V1 does not hot-reload user drafts at runtime.

**Profile/portfolio:** the eight accepted independent original-case IDs in teaching order are O1 → O2 → O4 → O3 → O7 → O6 → O5 → O8. Keep `scenarioId`, content-page order, actual world identity, and per-scenario save slot independent; exact game routing belongs to a later approved world/save policy. The O1 five-rack content must remain separate from optional compact O1 DRAFT until reconciliation. Later scenario placeholders are not proof of playable worlds, copy, ecology or financial contracts.

## 6. Compatibility and ownership gates

This spec is subordinate to user, Decision Log, Milestone, Game Brief and inspected code. No live source is changed under task #473. Scenario One #424 owns integration decision/scope and sole live coordination writer. #443/#450 Objective 1 source writers retain their branches and shared UI files; their work must be resolved/handed off before introducing the adapter there. #425/user hold portfolio and copy policy; other Multi-Rack manager owns its own forestry mechanics, never this browser tool. One owner per file/subsystem, fresh Git/main/candidate pin, registered claim, physical lease only if actual Unity run is authorised. No Windows build before personal playtest and direct authorisation.

## 7. Acceptance criteria by deliverable

**A standalone prototype (current implementation; verification separately recorded):**
- A1: opens locally without Unity/CDN, navigates real three-page intro, topics, steps, first-use/F1 help, controls and representative dynamic result field; 2–8 future scenario entries and compact O1 show no invented text.
- A2: import a compatible exported file, change/dirty/revert/diff/search/variant/navigate, validate before export, and reopen an identical draft with Unicode/line breaks intact; protected IDs/source metadata are read-only.
- A3: negative tests reject duplicate ID, changed provenance/order/game key, missing/extra/reordered tokens, empty required field, control/HTML injection and malformed JSON without overwriting current work.
- A4: source manifest/coverage report and README state exactly what is collected versus missing; candidate/main/draft never conflated; no Unity/save or source write.

**B production content adapter (UNAUTHORISED/PENDING):**
- B1: independently reviewed, **APPROVED** four-file spec and scope-specific source audit; #424 explicit source lane, clear #443 ownership and separate claim.
- B2: exact fallbacks/typed tokens, valid current/Legacy40/frozen Reference display and identity protections, no saved progress/World/PlayerPrefs/money calculation mutation; no unauthorized remote imports.
- B3: focused native Unity 720p and 1080p full-panel presentation, first-use and multiline/long copy, keyboard/navigation, old save and pending/approved/year/history conditions; source+profile drift audit and independent non-author review.
- B4: authorised integration and user copy acceptance separately recorded. No proof of B is established by A's static tests.

## 8. Explicit unresolved review items

- Source coverage is currently representative, not exhaustive: true total and any serialized/art-baked text unknown; require audit before a claim of “every in-game piece of text.”
- #443 candidate wording and its integration status may change; need Manager #424 canonical copy selection before production.
- If the owner changes educational requirements rather than phrasing, route gameplay predicate/scope decision to #425/#424; text editor cannot redefine objectives.
- Whether to support typographic rich text, token reordering or plural expressions later is an explicit **new reviewed content-contract** choice, not an implicit V1 permission.
- Exact future Unity asset import location/Editor menu/assignment and any future localization are design proposals pending prototype and source review.

**No Unity production implementation is authorised while this spec remains DRAFT.**



## DRAFT amendment after #474 independent review — source/tool contract, NOT Unity authority

These bounded rules address #474 S-01…S-05. The whole four-file specification remains **DRAFT / NOT APPROVED**.

**S-01 Closed-schema and approval integrity.** `ccf.content.v1` imports must reject unknown fields at **every object level**, not merely ignore them: package, provenance, scenario, page, entry, source, coverage and classifications. Known optional draft fields are only `authoringStatus="draft-local-export"` and a valid `exportedAt` timestamp. `approved`, `integrated`, `approvedBy`, `reviewedBy`, unsupported status promotions and arbitrary nested properties always fail with a path-specific error. Array order of entry IDs is immutable. Equality to the bundled seed is integrity *relative to that editable checked-in baseline*, **not** cryptographic authentication or evidence of managerial approval; only a separately reviewed Git commit/controlled approved-package process can establish provenance for game import.

**S-02 Derived and composed strings.** A browser field may edit only an actually evidenced source string. If C# builds a displayed sentence from other strings, the catalogue must expose independently supported constituent editable pieces plus an explicit dependency, or mark the generated/composed display as **read-only/derived** pending a typed adapter. In particular, `ControlsReference.KeysLineFor(bool rackPlanningV2)` transforms the base keys line using `.Replace()`, while `MenuHelpView` assembles Tree Mark help by joining entries/constants. Flattened copies of these results are **not** one-to-one source replacements and must not be used to silently rewrite C#, break control profiles or double-apply substitutions. `{{keysLine}}` remains a read-only typed integration-time placeholder; preserve the original context's fallback.

**S-03 Unicode restrictions.** Accept ordinary UTF-8 accented Irish, punctuation, arrows, quotes, newline `\n` and tab `\t`. Reject C0 controls other than tab/LF/CR, **all C1 `U+007F–U+009F`**, soft hyphen `U+00AD`, combining grapheme joiner `U+034F`, Arabic letter mark `U+061C`, Mongolian vowel separator `U+180E`, zero-width `U+200B–U+200F`, bidi embeddings/overrides `U+202A–U+202E`, invisible formatting `U+2060–U+206F`, and BOM `U+FEFF`. HTML-like rich text is still conservatively rejected. An adjusted Unicode rule is a reviewed contract change, not a path for using invisible marks to alter game control cues.

**S-04 Atomic coupled profile fallback.** For any copy family with linked legacy/general and Enlarged80 Rack V2 variants, **the approved Unity package must provide an entire reviewed coupled family**. If any mandatory family variant or its typed token/selector is absent, invalid or incompatible, **fallback atomically to the existing C# literal/generator for the entire family in that actual game profile**. Do not mix one newly authored variant with an old derived instruction or derive legacy from V2 by altering numbers. No runtime/world/save selection is inferred from the editor UI. The standalone editor may preview independent variant text; it has no Unity resolver.

**S-05 One shared status vocabulary.** For this catalogue use `main-source` = extracted checked-in main, `unmerged-candidate` = isolated #443 candidate **not shipped**, `no-shipped-text` = future scenario has no collected game copy, `draft-no-text` = unapproved compact O1 proposal with no copy, `reviewed-not-integrated` = separately reviewed content not yet loaded into Unity. These are *source/authoring* statuses, not gameplay or approval flags. `draft-local-export` is a separate package authoring state, and `APPROVED/NEEDS CHANGES/REJECTED` are **human review verdicts**, not catalogue `status` values. The UI filter must not promote one label to another. Future terminology changes require coordinated schema + docs + seed review.

**Post-#474 seed provenance revision.** Correction adds one source-backed Walking HUD title and accurate source-line anchors, pins the three candidate records to their observed exact candidate commit, and represents unmeasured coverage categories as `null`, not numeric zero. The seeded revision is now 2 with **79 pages, 269 fields**; old imported drafts with the prior baseline are intentionally rejected pending manual three-way review. No content title or gameplay key is reused as a save identifier.
