# CCF cross-scenario text editor V1 — implementation and verification plan

**Status: DRAFT / NOT APPROVED.** Plan describes two deliberately separate tracks: **A. independent browser editor** (task #473, implemented on isolated branch) and **B. eventual Unity content adapter** (not authorised and not implemented). Plan and tasks cannot independently approve `spec.md`.

## 0. Authority and minimum safety boundary

Source baseline: freshly checked `main` at task start, `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; the #443 Objective 1 candidate is **separate and unmerged**. Task writer: GPT-6 browser/GitHub session, exclusive `feature/ccf-text-editor-browser-prototype-20261113-v1`, execution claim [#473](https://github.com/jermur788/CCF/issues/473). No local Unity worktree or machine/Editor lease. Manager #424 is the sole live `active-tasks.md` writer. Source ownership of Scenario One UI remains with the existing Objective 1 lane; #425 is separate portfolio authority.

Only new `Docs/Research/ContentReview/**`, exactly four files under `specs/ccf-cross-scenario-text-editor-v1/`, `Tools/ContentReview/**`, and one named final handoff may be created by this task. No existing `Assets`, `Packages`, `ProjectSettings`, approved specs, canonical authority, live coordination, saves, PlayerPrefs, Unity scene or Reference changes. GitHub writes can prove files exist, not that browser/Unity ran.

## Stage A — Source research and architecture (browser lane, implemented with known limits)

1. Read live authority and ownership; inspect current main source and compare #443 candidate in read-only mode; do not choose candidate as the foundation.
2. Map player text flows beyond a UI filename list: three intro pages/production copy; Objectives topics, saved steps and teaching; F1/first-use/controls; Work Plan/Annual/Map/inspection; session/startup and generated/history. Mark exact game-vs-copy boundaries.
3. Build a source manifest with file blob identity and approximate lexical string candidates; record each *collected* entry with immutable content ID, branch, line, page/section/order, original text, condition, tokens and protected step/lesson IDs. Unknown text and serialized/art assets must remain visible gaps, not zero.
4. Compare static JSON with ScriptableObjects, Unity localization tables, C# replacement and server/CMS options. Recommend simple versioned JSON + future typed Unity adapter and explain licensing/portability/fallback.
5. Publish `Docs/Research/ContentReview/CCF-PLAYER-FACING-TEXT-INVENTORY-AND-ARCHITECTURE-20261113.md` and `Tools/ContentReview/source-map.json`.

**Completion evidence for this stage:** targeted 28 source C# files audited, 2,934 lexical quoted-literal *candidates* and 225 interpolation-start candidates; 268 fields / 79 review pages source-traced. This is representative, **not** exhaustive runtime coverage. Current main versus #443 candidate distinction retained. No Unity verification.

## Stage B — Four-file DRAFT specification (browser lane, no production release)

1. Define external UX, hierarchical scenario/page/field/variant view, metadata/dirty/diff/import/export, disabled editing of protected IDs and keyboard accessibility.
2. Freeze a proposed `ccf.content.v1` schema with explicit metadata, source/status, named token signatures, revision/approval separation and strict import constraints.
3. Specify safe Unicode/escaping/markup/plural/currency/placeholder and source-drift rules. Define exactly what an eventual typed Unity loader may look up and where old literal fallback must win.
4. Separate old five-rack/current O1 from compact DRAFT and accepted O1–O8 teaching order, protect save identity, and identify joint ownership/review gates.
5. Define measurable test/checklist gates for A (browser/JSON) versus B (Unity integration/720p/1080p/old world). Leave **status DRAFT**, mark unresolved source/copy choices as open.

**Outputs:** `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` in the new feature-spec directory. Neither specs nor research are copied into approved product authority.

## Stage C — Standalone prototype (browser-only, no Unity)

1. Use `index.html` + `styles.css` + `core.js` + `app.js`; no dependency install, analytics, cloud, iframe, remote fonts or package.
2. Ship `catalog.json` and exact `catalog.js` data mirror for direct `file://` support. Extract real O1 intro/steps/help and representative dynamic templates; mark main/candidate statuses and future empty chapters correctly.
3. Build sidebar, sections, one page at a time, prev/next, field editing, visible unsaved count, token/provenance readouts, original/current comparison, per-field/page revert, status and variant filters, search across entries.
4. Implement explicit JSON import/export and validation with immutable schema/scenario/page/entry metadata, source provenance and required tokens; downloaded export is stamped `draft-local-export`, not approved.
5. Treat untrusted text as text nodes only. Exclude arbitrary HTML/rich text, C# source edits and automatic saved-game updates. Do not require the client to enable arbitrary local scripting privileges.
6. Provide `README.md`, `tests.mjs`, read-only `source-audit.mjs`. The source drift scan should report changed blobs/missing sources/new candidates, not auto-replace C#.
7. Correct any schema or extraction bug before the final handoff; preserve tracked baseline text and full provenance.

**Evidence now:** the source catalogue initially contained two invalid empty Rack V2 variants; both were corrected using exact main C# literals, then **30/30 direct V8 content contract/syntax checks passed**. Node test script and actual browser interaction remain **UNRUN**; no rendered screenshot or local source checkout is claimed. This is an *implemented, source-inspection-verifiable standalone prototype*, not full browser qualification.

## Stage D — Worker self-review and handoff

1. Fetch each added file from GitHub and check status/contents. Compare worker branch to registered baseline and confirm **only allowed added paths**, no existing production or canonical authority modifications.
2. Re-run equivalent available static V8 contract negatives, report exact counts/failures/repairs and distinguish this from Node/browser/Unity tests not executed.
3. Publish the final detailed Markdown handoff at `Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md`, then read back.
4. Comment/mark #473 `[HANDED-OFF]` with actor/branch/starting/ending HEAD, new-path inventory, test/coverage matrix and direct GitHub handoff; close claim. Notify Manager #424 with source/feature gate and next reviewer. Dashboard and Drive mirror synchronization unverified unless directly observed.

**Stage D criteria:** durable publication and claim closure prove *execution handoff* only, not spec approval, Unity compatibility, acceptance or integrated status.

## Future Stage E — Independent non-author review (NEW claim, owned by Manager #424)

- Review spec/architecture, catalog provenance and exactly listed source/branch differences, check no fabricated text or game ID mutation.
- Actually fetch/check out browser files and run `node Tools/ContentReview/tests.mjs` and `node Tools/ContentReview/source-audit.mjs .` against a full pinned repo checkout. Use Chrome/Firefox where available to exercise `file://` import/edit/export/reimport, 79-page navigation, search, variant handling, multiline/Unicode and malicious input. Record browsers/versions and visible screen results. Unit tests alone do not qualify user editing UX.
- Run a source completeness/deduplication audit before telling owner “every piece of text”; inspect missing serialized/assets and non-UI C# and re-extract correct candidate after #443 source handoff.
- Issue **APPROVED / NEEDS CHANGES / REJECTED for DRAFT quality and standalone tool separately**. If needed, create bounded repair tasks and repeat the affected checks, not whole game qualification.

No worker on #473 can self-approve the future adapter or resolve #425 product decisions.

## Future Stage F — Controlled content adapter (NEW spec-APPROVED claim, separately authorised)

**Entry gates:** #424 accepts independent review, owner approves the feature spec/copy scope, #443/#450 source ownership is explicitly handed off, branch/HEAD pinned and disjoint source lane registered. #425 confirms cross-scenario scope and independent save router policy. Unity physical lease must be free and safely acquired if runtime qualification is needed. Do **not** use this task branch as a production base merely because it contains the editor.

1. Choose the narrowest first source slice: intro and Objectives/Help; preserve existing C# literal fallback. Build an explicit typed reader for approved catalogue `TextAsset`, no runtime import of arbitrary drafts and no persisted game fields.
2. Use world/profile-aware lookup based on existing authoritatively derived state, not on text or global editor selection. Keys `CCF.Learning.v1.*` and `CCF.MenuHelp.v1.*`, `ScenarioOneTeachingSequence` IDs, Rack V2 and MFM state/Work Plan statuses and Legacy40/Reference stay unaltered.
3. Verify exactly picked candidate via static checks, focused Unity compile and 720p/1080p screens for intro, Objectives, First-use F1, Rack V2 and Legacy40; source-vs-rendered text traces, missing key fallback, Unicode, long copy, token negatives and no identifier/save drift.
4. Expand to Work Plan/Annual/Map/dynamic messages **only** after their source coverage/typed formatting and own bounded feature batches; avoid automatic rewrite of all string literals.
5. Independent review, Manager integration verdict, actual main merge only if authorised, user content UX approval and dashboard/context mirror controls when canonical project instructions change.

**Stop conditions:** overlap with a live source/Unity owner, unverified source candidate, modified save/key identity, unexpected legacy text routing, injection/formatting risk, changed lesson predicate, unsupported genuine source text, and unresolved product scope. Preserve unrelated dirty local work; do not acquire or forcibly terminate a Unity process or touch user saves.

## Efficiency and risk controls

- Use cheap static validation before native testing; do not require Unity for a standalone text-only tool.
- Review one approved loader + first content slice before attempting huge conversion; condition/format complexity is the primary hidden cost.
- Do not make giant automatic “all strings” C# patches; they create cross-owner conflicts and break legacy/progress semantics.
- Honest coverage categories and source-blob drift checks matter more than a decorative 100% counter.
- No Windows/Player build before separate user playtest and explicit authorization. Drive is a one-way mirror; Git is canonical.



## DRAFT Stage C repair / verification addendum from #474

**Status: DRAFT; still NOT APPROVED for Unity.** Independent #474 results were both **NEEDS CHANGES**; #477 is a bounded browser/standalone source repair rather than a new implementation programme.

1. **Repair F-01/F-02 first**: test real 1280×720 scroll reachability with pointer hit testing, and reject unrecognised package/entry/nested metadata before export. Keep exact base candidate read-only.
2. **Complete F-03…F-13**: ordered entries, invisible/bidi/C1 safety, real text search without repeating scenario names, live page edited count, browser beforeunload on unexported edits, source-literal line corrections, Walking HUD title, null unknown coverage, expanded committed Node tests, candidate commit pin and numerical early-failure validation counts.
3. **Seed migration:** treat the corrected catalogue as reviewed-source-data candidate revision 2, not an automatic migration of user edits. Old `catalog.json` drafts with a different source baseline require explicit manual compare/re-application; keep `catalog.js` parsed-data mirror identical and `source-map.json` main anchor unchanged. A draft export neither becomes approved nor changes Unity.
4. **Derived strings and partial variants:** apply spec S-02/S-04 constraints during any future Unity import; the browser editor can display computed placeholders but no flattened C# replacement is authorised. Shared statuses as S-05.
5. **Focused verification:** source correctness/Node suite + browser at 1280×720 and 1920×1080 including page clicks, search, dirty, beforeunload, JSON import/export and rejection (B01–B05), then a **separate, non-author reviewer** runs a short delta re-check. No Unity physical lease, no Windows build.
6. **Review exit:** all F-01…F-13 and S-01…S-05 addressed with explicit evidence, actual statuses for browser/Node/unknown tests, GitHub compare against the original author's branch and new durable handoff. Worker **cannot self-approve** standalone tool or DRAFT.

**Current source truth:** original #473 handoff and #474 report stay historic and unchanged. The four-file DRAFT is still subordinate to user, Decision Log and source authority. #424 directs a future typed Unity adapter only under a new APPROVED spec/claim, never from this repair alone.
