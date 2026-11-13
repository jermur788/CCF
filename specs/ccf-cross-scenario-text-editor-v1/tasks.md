# CCF cross-scenario text editor V1 — task breakdown

**Status: DRAFT / NOT APPROVED.** Work statuses below describe the isolated #473 prototype only. The future Unity content adapter is a separately gated deliverable and **has no authorised implementation owner**. Completed browser-source tasks do not make the feature spec APPROVED or mean Unity text is integrated.

**Registered worker:** independent OpenAI ChatGPT GPT-6 GitHub/browser lane, claim [#473](https://github.com/jermur788/CCF/issues/473), branch `feature/ccf-text-editor-browser-prototype-20261113-v1`, no local worktree/Unity/physical lock. Source and save owners #443/#450/#424 are untouched. #425 retains cross-scenario product/story decisions.

| ID | Work item / owned path | State | Proof or remaining gate |
|---|---|---|---|
| A-01 | Live source/ownership/authority preflight; read actual main and separate #443 candidate | **COMPLETE (GitHub read-only)** | #473 registration, baseline and live coordination read; no existing target writer detected |
| A-02 | Source trace and gap report, main/candidate/legacy/compact distinction — `Docs/Research/ContentReview/CCF-PLAYER-FACING-TEXT-INVENTORY-AND-ARCHITECTURE-20261113.md` | **PUBLISHED, PARTIAL-COVERAGE** | 28 source C# paths blob-anchored; 2,934 literal lexical candidates and 225 interpolated starts **not actual coverage denominator** |
| A-03 | Typed `ccf.content.v1` seed and source mapping — `Tools/ContentReview/{catalog.json,catalog.js,source-map.json}` | **PUBLISHED** | 79 pages / 268 fields; 265 main source and 3 unmerged candidate; initial two blank Rack V2 variants repaired |
| B-01 | DRAFT product/editor/content/compatibility spec — `spec.md` | **PUBLISHED DRAFT** | Needs independent review and owner/Manager acceptance, not implementation authority |
| B-02 | This DRAFT plan with phases and gates — `plan.md` | **PUBLISHED DRAFT** | Clear A standalone/B Unity separation |
| B-03 | DRAFT executable tasks and dependencies — `tasks.md` | **PUBLISHED DRAFT** | Future adapter tasks are withheld |
| B-04 | DRAFT checklists/requirements acceptance matrix — `checklists/requirements.md` | **PUBLISHED DRAFT** | Unverified requirements intentionally unchecked |
| C-01 | Local static browser shell and responsive UX — `index.html`, `styles.css` | **IMPLEMENTED SOURCE** | Actual browser launch/render/focus behavior **UNRUN** |
| C-02 | One-page navigation, nested sections, current status, search, variant selector, original/draft, revert/dirty — `app.js` | **IMPLEMENTED SOURCE** | JavaScript syntax and static checks PASS; live DOM click-through **UNRUN** |
| C-03 | File chooser import, explicit export, downloadable revision stamp, in-memory safety — `app.js`, `core.js` | **IMPLEMENTED SOURCE** | Contract validation/round-trip logic PASS direct V8; downloaded file reimport through actual browser **UNRUN** |
| C-04 | Schema identity/token/source safety, escaping/Unicode negative tests — `core.js`, `tests.mjs` | **IMPLEMENTED SOURCE** | 30/30 direct V8 checks PASS on current catalogue; Node `tests.mjs` **UNRUN** |
| C-05 | Read-only future drift scanner — `source-audit.mjs` | **IMPLEMENTED SOURCE** | A full local CCF checkout scan **UNRUN** |
| C-06 | Offline usage/limitations guide — `README.md` | **PUBLISHED** | Explicit file://, optional localhost, import/export and integration gate |
| D-01 | Pull-back/readback/diff of all new files, changed-path audit, verification matrix | **COMPLETE — GitHub readback** | All 15 pre-handoff additions fetched; base comparison shows no changed existing file, zero outside allowed paths, no Unity/source modification |
| D-02 | Publish exact result at `Docs/Handoffs/ScenarioOne-TextEditor-20261113/CCF-TEXT-EDITOR-RESEARCH-SPEC-AND-STANDALONE-TOOL-HANDOFF-20261113.md` | **PENDING FINAL HANDOFF** | Read back and link direct URL |
| D-03 | Comment #473 [HANDED-OFF], close; notify #424 | **PENDING FINAL HANDOFF** | Execution closure only |

## Future externally authorised tasks — NOT STARTED

| Task | Dependency / proposed owner | Needed completion evidence |
|---|---|---|
| R-01 — independent research/spec/code review | New non-author review claim routed by Scenario One Manager #424; no overlap with #443 | Source and catalogue trace; variant correctness; review verdict APPROVED/NEEDS CHANGES/REJECTED |
| R-02 — real browser and Node test | Reviewer or explicitly assigned browser-capable local worker | `node Tools/ContentReview/tests.mjs` executed; Chrome/Firefox file:// or localhost smoke; import→edit→download→reimport with Unicode/multiline/invalid-token negative; screenshots if needed |
| R-03 — source-coverage closure | New bounded source-audit claim, coordinated with #443 and #424 | Full checkout `source-audit.mjs` run, serialized/Resources/art text inventory and runtime capture; classify all collected/conditional/generated/serialized/unlocated/out-of-scope with factual counts |
| G-01 — decide exact canonical source after Objective 1 | #424 / user; #425 if product-level wording | Which #443 copy is accepted and which baseline/catalogue revision supplies future game copy |
| G-02 — approve four-file spec | #424 + #425/user for cross-scenario product boundaries | Explicit **APPROVED** commit/status and new integration claim; open issues resolved or confined to later slice |
| U-01 — narrow read-only Unity loader and intro/Objective/Help adapter | NEW dedicated local Sol/Codex or other capable implementation worker, only after exclusive safe source/Unity ownership | Approved `TextAsset`, typed token inputs, fallback to old C# literal, world/profile correctness, no save/PlayerPrefs writes |
| U-02 — initial native qualification | Dedicated serial Unity lane; no user save/process interference | Exact candidate compile/run, 1280×720 and 1920×1080, old world/Reference/fallback, IDs, source-to-rendered checks; no Windows build |
| U-03 — dynamic/Work Plan/Annual/Map conversion slices | Separate subsequent reviewed packets, no shared writer overlap | Source coverage first; preserve money/conditions/learned progress; focused tier-2 panel checks |
| U-04 — independent integration review, Manager verdict and accepted content release | Independent reviewer → #424 Manager, with #425/user copy approval as applicable | Explicit APPROVED/NEEDS CHANGES/REJECTED, GitHub lineage, user-visible text validation and safe main integration if authorised |

## Ownership/API handoff boundary

- Browser tool owns `Tools/ContentReview/**` and DRAFT research/spec only. Its JSON ID namespace is **copy identity**, not a gameplay key. Source files for future Unity loader have **no ownership granted** here.
- #424: Scenario One delivery manager/sole coordination writer; approves source scheduling, resolves #443 candidate and coordinates independent review. #425/user: product/story/independent case approval; not a second #424 coordination writer.
- #443/#450: Objective 1 candidate and shared `LearningObjectivesView*`, `ScenarioOneUiRoot`, `MenuHelpView*`, `StandMapView` and related source; no overlap until explicit handoff. Multi-Rack manager: own forestry logic, not the editor.
- A code implementer must register a new GitHub claim as `[RUNNING]` with actual worker/provider, exact branch/worktree/starting HEAD, ownership and Unity phases *before* production edits. No local machine/lease is created by this browser task.
- If the recommended JSON system conflicts with a newly accepted product policy, revise DRAFT spec and gain approval **before** source work.

## Stop and escalation

**Stop** on a new source writer/lock conflict, missing approved spec, source drift or unverified #443 copy, wrong world/profile, unaccounted dynamic text, changed saved/game identity, uncertain existing literal fallback, or a hazardous user-owned Unity/save environment. A failed browser test or missing local test runtime is reported as FAILED/UNRUN and handed to a bounded review/repair task; do not infer pass.

**No user decision is needed to inspect the standalone prototype.** User approval is required before content becomes canonical or any production adapter is released. Dashboard/Drive sync remains unverified pending Manager-owned pipeline checks.
