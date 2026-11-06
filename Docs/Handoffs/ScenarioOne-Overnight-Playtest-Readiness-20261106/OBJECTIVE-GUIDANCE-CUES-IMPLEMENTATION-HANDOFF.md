# Stage B / T2 — Objective 1 contextual cues handoff (#295)

**REPAIR PASS 2 READY FOR INDEPENDENT SOURCE RE-REVIEW; NOT ACCEPTED OR INTEGRATED.**

The original implementation report below is historical evidence at `ffe7c21c` / `df6415c9`. The final **Repair pass 2** section supersedes its label-mutation, whole-root demotion, later-context styling and periodic-derivation descriptions and source line numbers. Its original 26 source PASS rows are not Unity evidence.

Actual worker: OpenAI, `gpt-6.1-sol`, Medium reasoning, Codex harness in T3 Code. Implementation sub-agent; no children launched. Source-only authority: user dispatch, Manager G4/G5 approval and Stage B packet on main `553a5901bf12aa293247f9fed1797812c148b755`, approved four-file spec at `c61bbdd92b94ca07ad806bddfa0bb34123f9e89f`. Approval addendum controls historical draft wording. Spec status untouched.

- Branch: `feature/scenario-one-next-action-guidance-v1`.
- Worktree: `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` (ZX20).
- Starting HEAD / origin tracking: `e1be15c17a403bc78130a7f01a3248d537960fd8`.
- Ending implementation HEAD: `ffe7c21c19cead30d2d2bce1b6fb58fbabf04ef7`.
- Delivery HEAD is the subsequent handoff-only commit containing this document; resolve its exact SHA with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/OBJECTIVE-GUIDANCE-CUES-IMPLEMENTATION-HANDOFF.md`. The worker's final report records that SHA separately, avoiding an impossible self-referential commit hash. Production and test bytes are pinned by the implementation HEAD above.

Preflight actually ran `git fetch origin`, `pwd -P`, porcelain status, HEAD/branch/upstream and worktree inventory. Assigned lane was clean and exact, with no unrelated changes. Live main coordination reserves this sole T2 lane; T3 is waiting. Current fetched main was exactly `553a5901bf12aa293247f9fed1797812c148b755`; `git diff e1be15c 553a590 -- Assets Tools` was empty. ZX20 had 31 GiB available. No rebase/merge/reset/stash/clean, Unity, Player, build or save access. The user's Primary and playtest checkouts were not entered or modified. Only #200 may update live coordination; this document records the actual worker handoff, not a fabricated live-lock update.

## Changed paths and implementation

| Path | Change |
|---|---|
| `Assets/ForestPrototype/UI/LearningObjectivesView.cs` | Name existing map/work/forest destinations; remove permanent forest primary; add visible instruction/reason label. |
| `Assets/ForestPrototype/UI/StandMapView.cs` | Name existing navigation; demote static primaries; safe-first rack questions; clear bounded prompt focus on rebuild; disable Confirm during either question; rederive cue after rebuild. |
| `Assets/ForestPrototype/UI/WorkPlanView.cs` | Name existing approval/advance/results/Return actions; demote permanent footer primaries; preserve safe warning Return; scoped focus clear and refresh. |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | One transient selected button, using existing facts, validators and active scope; demote competing primaries including Annual Review through the root, without editing that view. |
| `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` | New-only 20-unit cue/reason text, heavy border, wrapping; no global typography change. |
| `Tools/Verification/NextActionGuidance/check_sources.py` | 32 explicit source-contract checks; no gameplay/UITK execution. |
| This handoff | Actual evidence, limitations, future gates. |

No changes to UiKit, Curriculum/Sequence, TeachingDiagram/TeachingDiagramPlan, marking/world/manager code, geometry/MFM, saves/preferences, assets/metadata, packages or project settings. No existing test was modified. No new action button, progression engine, automatic operation, key binding or focus opt-in.

`RefreshNextActionGuidance` projects onto an existing button only; it never invokes its callback. It removes prior cue text/style and competing primaries, resolves the active screen/prompt, checks enabled state and ancestor visibility, then adds **Next:** plus a heavy border/weight to at most one candidate. A visible instruction or validator reason accompanies the three owned full-screen views. Unknown/blocked mappings have no primary. Later objectives/Legacy40 receive no invented guidance mapping; the old permanent primary emphasis is demoted rather than treated as a meaningful next action.

Card rebuilds rederive immediately. LateUpdate rechecks at the existing 0.25-second modal cadence, with Help/session/reference suspension checked every frame. This bounds ordinary external marking/load fact refresh without repeatedly scanning forest/validators every frame. Existing Help/onboarding owns its original safe navigation, and the existing same-frame Help-close guard suppresses underlying guidance. No modal/input lifecycle was changed. Actual occlusion, timing and input safety remain native verification gates.

## Source evidence at implementation HEAD

Paths below abbreviate `Assets/ForestPrototype/UI/` as `UI/`.

- `UI/ScenarioOneUiRoot.cs:325`: selector; `:335` competing-primary removal; `:346` suspension; `:349` active scope; `:358` dual/single questions; `:363` safe Advance Return; `:364` O1/first-results scope; `:370` annual-review priority; `:373` map planning; `:388` field-mark return; `:403` approval gates; `:412` quote/cash; `:426` approved/failed lifecycle; `:441` candidate check; `:464` ancestor visibility; `:476` scoped focus clearing.
- `UI/LearningObjectivesView.cs:73`: map destination calls `ShowMap`; `:84` visible reason; `:128` refresh hook. Existing `CurrentInstruction` and `RackLayoutTracker.V2ObjectiveInstruction` remain authoritative wording.
- `UI/StandMapView.cs:249`: rebuilt prompt focus/refresh; `:635` reset safe first; `:646` dual moves consent accept after reset safe choice; `:662` Confirm gates; `:698` consent safe first; `:732` reopen safe first.
- `UI/WorkPlanView.cs:68`: original warning's safe Return remains primary and first; `:169` existing approval/advance availability; `:184` prompt focus/refresh. Full pending-cost disclosure remains in the existing plan and is reinforced in guidance.
- Existing unchanged sources: `UI/RackLayoutTracker.cs:28` rack-owned lifecycle measurement, `:84` Pending/Approved; `UI/LearningObjectivesView.Sequence.cs:158` RackPlanProblem and `:178` RackMarkingProblem; `UI/ScenarioOneTeachingSequence.cs:115` topic advances when AccessOpen; `UI/AnnualReviewView.cs:66` existing acknowledgement; `ScenarioOne/ScenarioOneManager.RackEditing.cs:107` authoritative five-rack validator and `:269` confirmation recheck; `ScenarioOne/ScenarioOneManager.RackPlan.cs:360` actual entrance target; `ScenarioOne/ScenarioOneManager.cs:1221` approval validation/cost and `:2324` same total-cost quote basis.
- `UI/UiKit.cs:62` retains its deliberate `focusable=false` policy. No Tab/arrow/O/M/Esc/F1 routing was edited.

## Deterministic cue/prompt matrix

**PASS below means source/static inspection only**, supported by the branches cited above and the bounded source checker. These states were not instantiated in Unity or a mock UITK runtime. Row ranges are explicit equivalence classes, not claims of native input runs. All cue actions require deliberate pointer activation.

| # | State | Expected cue / reason | Source basis | Result |
|---|---|---|---|---|
| 1 | Objectives, unconfirmed 0–4 racks | Stand map; existing counter/instruction | Root unconfirmed branch; Objectives ShowMap | PASS (static/source only) |
| 2 | Objectives, invalid five or valid five | Stand map; never labels invalid work ready | Same navigation branch; existing instruction | PASS (static/source only) |
| 3 | Map, unconfirmed, rackMode off, planning available | Plan Racks | RackPlanningAvailable then !RackMode | PASS (static/source only) |
| 4 | Map, unavailable tool/ecology/edit permission | None; service/edit reason | Availability branch | PASS (static/source only) |
| 5 | Map, rackMode on, 0–4 or invalid five | None; actual CanConfirmRackPlan reason | Validator branch; count/geometry source | PASS (static/source only) |
| 6 | Map, five valid, no question/gesture | Confirm Rack Plan; Pending only, nothing approved/cut | Validator + existing confirm callback/caption | PASS (static/source only) |
| 7 | Manual drag/ghost, including valid-five background | None; live status/instruction | Dragging/Ghost before Confirm | PASS (static/source only) |
| 8 | Single consent | Keep approved work; accept secondary | Root consent target; safe-first panel | PASS (static/source only) |
| 9 | Single reset | Keep my racks; reset secondary; Confirm disabled | Reset target and Confirm predicate | PASS (static/source only) |
| 10 | Single reopen warning | Keep the plan confirmed; reopen secondary | Reopen target/panel | PASS (static/source only) |
| 11 | Consent + Reset, unconfirmed 0–4 / invalid-five / valid-five / confirmed background | None; two-question explanation; decline then reset-cancel before prompt destructives | Dual branch before all progression; accept moved after reset | PASS (static/source only) |
| 12 | Confirmed, 0–4 marks, an actual unmarked unlocked target exists | Objectives/Map Back to forest; P/reach/click instruction | Entrance-target query + shared instruction | PASS (static/source only) |
| 13 | Confirmed, no eligible unmarked entrance (locked/missing/conflicting mark) | None; unavailable/validator explanation | Target validation; no harvested redo | PASS (static/source only) |
| 14 | Confirmed/marked, rack-owned Pending | Objectives/Map Work Plan; Work Plan Approve only if enabled, quote/cash valid | Tracker Pending; both teaching gates; candidate enabled | PASS (static/source only) |
| 15 | RackPlanProblem / RackApprovalProblem / invalid rack Pending order | No approval cue; actual problem | Existing gates and validationMessage | PASS (static/source only) |
| 16 | Approval quote invalid / unaffordable / CanEditWorkPlan false | No approval cue; quote/cash/unavailable reason | Quote, summary cost, enabled predicate | PASS (static/source only) |
| 17 | No Pending; complete coverage; actual Approved unresolved work; valid annual gates | Work Plan destination / enabled Advance | Complete + Approved + no Failed + CanAdvanceYearNow + ecology | PASS (static/source only) |
| 18 | No Pending but absent Approved, incomplete coverage, failed work, or advance unavailable | None; review actual work/results and feedback | Explicit lifecycle fallback | PASS (static/source only) |
| 19 | NeedsAnnualReview in O1/first rack results | Work Plan destination / Read first annual results; never Advance | Annual priority | PASS (static/source only) |
| 20 | First rack work resolved, teaching topic already advanced, latest results visible | Existing annual-review-acknowledge; hidden older-year acknowledgement ineligible | First-results derived branch + visibility check | PASS (static/source only) |
| 21 | Advance warning, including over an otherwise eligible Advance | Existing safe Return only; Approve/Advance anyway secondary | Warning branch precedes ordinary mapping | PASS (static/source only) |
| 22 | Help/onboarding above any rack/Advance question; same-frame Help close | No underlying cue; original Help/onboarding navigation retained | Suspension before scope resolution; unchanged Help | PASS (static/source only) |
| 23 | Session/startup/reference preview | No forest progression cue | Existing IsOpen/ReferencePreviewActive | PASS (static/source only) |
| 24 | Selected destination disabled/hidden or ancestor hidden | No cue; reason in owned views; no automatic enabling | Candidate and ancestor checks | PASS (static/source only) |
| 25 | Rebuild/pointer dismissal/view change/load, rackMode unsaved | Drop previous selection; re-read current facts/mode/questions | Refresh hooks/cadence; unchanged editor pointer dismissal | PASS (static/source only) |
| 26 | Single/dual prompt or Advance-warning rebuild | Scoped focus cleared; no default/restore; safe choices first | ClearPromptFocus; existing nonfocusable buttons; panel child order | PASS (static/source only) |
| 27 | Actual pointer loop, first-use Help/force-close, prompt dismissal and save/load | All preceding behaviour must be observed on exact candidate | AC-G/P/H/S | NOT RUN |
| 28 | Native stale Enter/Space/Tab/arrow/O/M/Esc/F1, single/dual/reopen/Advance prompts | No harmful default/restored/background activation; existing keys preserved | AC-X/K/H | NOT RUN |
| 29 | New-control keyboard reach/cycling | DEFERRED / NOT PROVIDED by Manager Q1; never PASS | No focus opt-in or keyboard additions | NOT RUN |
| 30 | 720p/1080p colour/grayscale rendering, clipping, measured text/panel scale | New text ≥16 physical px; distinct text/border cue; usable controls/scroll | AC-V; 20-unit USS alone insufficient | NOT RUN |
| 31 | Unity 6000.6.0f1 import/compiler/runtime and performance | Exact-head qualification required | Stage D/T5 | NOT RUN |
| 32 | Protected Legacy40/frozen Reference Future and gameplay/save regressions | Exact-candidate regression disposition required | AC-S; protected sources unchanged | NOT RUN |
| 33 | Screen-reader delivery | DEFERRED / NOT PROVIDED; visible text is no accessibility certification | Manager D-10 | NOT RUN |

**Matrix totals: 26 PASS (static/source only), 0 FAIL, 7 NOT RUN.**

## Checks actually run

| Check / command | Actual result |
|---|---|
| `git diff --check` | PASS; also checked staged/committed delta before handoff. |
| `python3 Tools/Verification/NextActionGuidance/check_sources.py` | PASS: 32 source contracts, 0 failures. Includes five-production-path inventory, no untracked Assets harness/meta, forbidden added F6/focus opt-in/preferences/save declarations, no selector world-action execution or invented control. |
| `python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod --out /media/jer/Files/CCF/Verification/NextActionGuidance-T2-20261106/typecheck` | PASS **approximate non-Unity type gate only**: 6 distinct errors, exactly the existing 6-error baseline, zero new errors (`TYPECHECK_CLEAN`). Existing 2021.3 references/stubs are not Unity 6 compilation. No baseline update. |
| Existing `RackV2HelpTeaching/run_offline.py` **offline function only**, invoked through Python import with explicit durable output directory | PASS: `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`. Plain .NET production wording/unlock/flow checks. The historical whole-run source ownership check is for another task and was not invoked or represented as passing. |
| `python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | **FAIL to complete / exit 2**: `source anchor missing: ...LearningObjectivesView.cs: buttons.Add(UiKit.Button(`. This historical checker requires the old inline constructor/static primary shape and cannot assess the dynamic selector. Its existing six SnapFar offline cases ran PASS before the anchor error; no aggregate gate PASS. Checker left unchanged. Historical CTA FAIL 08 is not cleared by this source handoff. |
| Unity compiler, Editor/Player, native input, runtime, rendering, Windows build | NOT RUN. **No Unity/runtime verification.** |

Durable source/type-check/historical-check output: `/media/jer/Files/CCF/Verification/NextActionGuidance-T2-20261106/`. No new evidence path on ZX20. The Help offline function compiled/ran into `help-offline/`; its 97-check success was observed in this worker's tool output. No Unity assembly, harness or generated `.meta` was copied into Assets.

## Limitations and remaining gates

- Source assertions inspect contracts; they do not execute the cue selector, instantiate the matrix states or prove actual UITK focus, geometry, layout, pointer routing or rendering. Native prompt interleaving and pointer dismissal remain mandatory. No runtime PASS is inferred from nonfocusable source buttons or `Blur()`.
- Ordinary facts refresh within the existing modal cadence; per-frame suspension is immediate in source. Actual frame timing and performance are unmeasured.
- Source text floor is 20 UI units. Button growth and the added reason lines need measured 720p/1080p layout/reflow qualification; no physical-pixel or clipping PASS.
- Historical feedback checker needs an independently authorised successor/repair for dynamic cues; it was not weakened to obtain PASS. The new bounded source check complements it and does not retroactively erase FAIL 08.
- Later objectives, keyboard delivery, screen readers and the teaching-map diagram are deferred. No whole-game guidance or accessibility completion claim.
- Independent **Claude Sonnet 5.5 Medium source review can start** at the delivered exact head, followed by **Manager T2 acceptance**. This worker has not launched or impersonated that review. #296/T3 must wait for both gates and a clean ownership transition; no simultaneous writer.
- Subsequent Stage C/D integration remains Workbench-first and requires fresh owned candidate/Unity lease, native input/rendered O1 loop, disposable save/load and protected regression gates. Only Manager can accept/integrate/release playtest readiness. No Windows/Player build authority.
- Manager/orchestrator must update #295/live coordination and dashboard/ledger from this actual handoff. Those external status writes and dashboard ingestion are **NOT VERIFIED**, and this worker did not post issue/PR comments or edit Manager-owned records.

## Repair pass 2 — independent source review B1–B3 (#295)

Actual worker remains OpenAI gpt-6.1-sol, Medium, Codex harness in T3 Code, implementation sub-agent; no children. Source-only; no Unity. Direct repair packet controls scope. Reviewed independent report from `8284fcb52e1bdb8b2bfe4b033fb6af9371b22ae3` and approved spec `c61bbdd92b94ca07ad806bddfa0bb34123f9e89f` plus actual Manager approval.

- Start / remote HEAD: `df6415c9b4845f4f9567fb5cd297add5e8f797f4`, clean, same assigned worktree and branch.
- Repaired production/checker HEAD: `065feb12800295155dca8bd76ae7ee300d6bd024`.
- Delivery HEAD: subsequent handoff-only commit; resolve with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/OBJECTIVE-GUIDANCE-CUES-IMPLEMENTATION-HANDOFF.md`. Final worker report supplies its SHA.
- Six source/checker paths above plus this handoff changed. No protected harness expectations changed. No merge/rebase/reset/force push, no GitHub comments. Primary's unrelated `AGENTS.md` / `Docs/Operations/StorageArchitecture.md` changes were observed and preserved. No live coordination writes; #200 remains sole writer.

### Blocking repairs

**B1:** No existing button `.text` is assigned or stripped by guidance. A separate `Label("Next:")`, with picking ignored, is inserted immediately before the selected button as a sibling. Its own USS class uses 20-unit bold text; the button keeps the heavy outline/weight. `Plan Racks`, `Advance one year`, annual acknowledgement and all other original labels remain unchanged. Exact-label protected harnesses were inspected, not edited. Badge placement/reflow and physical text size remain unverified in Unity.

**B2:** Expensive cue derivation is now limited to `RefreshNextActionGuidance` after genuine Objectives/Map/Work Plan rebuilds and Review entry. The Objectives hook moved below its unchanged-content early return. Map and Work Plan already call below their signature/key early returns. Existing user plan/mark/approve/work/question operations rebuild these views; force-open refreshes reread the world after walking/marking, save/load or view re-entry. Quote/entrance queries can run there, with entrance search stopping at the first usable target. The existing 4 Hz view polling can detect changed content and rebuild, but an unchanged poll cannot invoke these cue queries.

`LateUpdate` calls only `ApplyNextActionGuidance`: cached button/scope, modal booleans, enabled state, ancestor visibility and (only when selection changes) styling/badge updates. It has no forest scan, quote, teaching validator, four-root array allocation or primary query sweep, including while Help/session/reference is open. This is cached presentation, not saved progression. Rebuild invalidates the candidate/reason, restores prior owned classes and derives again; cached visibility is checked every frame. Normal open views prevent forest marking; marking happens on return to walking and view entry forces refresh. Thus supported player actions do not depend on a timer rerunning expensive facts to clear stale guidance. Arbitrary out-of-band mutation without a view rebuild is not claimed supported; actual action/load lifecycle and frame cost remain Stage D gates.

**B3:** Demotion/restoration uses the exact 19-name `GuidanceButtonNames` allowlist checked by the source checker: the three named Objectives destinations; map Work Plan/Back to forest/Plan Racks/Confirm/Reopen; six consent/reset/reopen choices; Work Plan Approve/Advance/read-results/Return; Annual Review acknowledgement. It never queries a whole root for primary buttons. Original class membership is recorded and restored before rederivation. Non-O1/Legacy40 return before scoped demotion; original static forest/Plan/Confirm/Reopen/Advance/results constructor styles were restored. The previously removed unrelated waypoint primary was also restored. Treatment-review Inspect/Prepare/Approve source and primary classes remain untouched; other unnamed primaries remain untouched by design. Scoped prompt destructive controls stay secondary per approved D-08; no new later-objective guidance is invented.

### #270 disposition and non-blocking notes

| Finding | Disposition at repair HEAD |
|---|---|
| R-01 keyboard reach | DEFERRED / NOT PROVIDED under approved Q1 pointer-only first slice. No F6, focus opt-in or global UiKit change. Native stale Enter/Space/Tab/arrow/O/M/Esc/F1 safety remains Unity-only, never PASS by source. |
| R-02 destructive primary | IMPLEMENTED in source: consent/reset/reopen consequences remain secondary; safe single choice or Advance Return is cued. Real routing/focus/activation remains Unity-only. |
| R-03 overlapping questions | IMPLEMENTED in source: dual has no selected primary, scoped focus clearing and both safe choices before consequences. Reset remains reachable during consent; no mutual-exclusion/lifecycle rewrite. Actual interleaving/traversal remains Unity-only. |
| Medium: dual question/action separation (N2) | FIXED in source: relocated Cancel approved work now has its own card repeating `editor.ConsentMessage`, after both safe choices and Reset layout. Label unchanged. Fit/scroll/visual clarity Unity-only. |
| Medium: misleading rack reason (N3) | FIXED: failed-target reason is local to the no-markable-target fallback; a usable entrance stops the search and keeps the shared marking instruction. |
| Medium: cue latency (N4) | Removed the 250 ms presentation timer. Cached candidate is checked every LateUpdate, allowing first eligible post-layout frame and respecting same-frame Help-close guard. Actual resolved-style timing is Unity-only; no measured zero-latency claim. |
| Medium: suspension work (N5) | FIXED by cheap cached application; unchanged/suspended frames do not derive expensive facts or sweep roots. Profiling Unity-only. |
| Medium: later objectives/Legacy40 (N6) | FIXED in source by original constructors, restore-before-return and scoped allowlist. No invented later mapping. Native/protected regression checks remain NOT RUN. |
| Medium: baseline checker abort (N7) | Explicitly DEFERRED: repairing a protected historical checker is outside the same owned paths. Unchanged rerun exits 2 on old inline-constructor anchor; checks 01–07 including 06a/b/c execute before the error, 08–09 do not complete. Its old FAIL 08 is not cleared. Manager must assign an independent successor before relying on that aggregate gate at Stage D. |
| New-text sizing/non-colour delivery; screen reader | Badge/reason 20-unit source floor plus word/border cue IMPLEMENTED in source. Measured 720p/1080p physical size, grayscale, contrast, clipping and equivalent-role checks remain Unity-only. Screen-reader delivery DEFERRED under D-10. |

### Repair matrix and checks actually run

The original 33-row matrix remains the state inventory: rows 1–26 are **PASS (source/static only)** after reinspection with these corrections: row 12 uses the first eligible entrance; row 25 rereads facts on rebuild/entry and applies cached visibility every frame; rows 21–22 suspend the cue without per-frame derivation. Rows 27–33 remain **NOT RUN**, including Q1/D-10 deferred delivery. None instantiate UITK or prove runtime behaviour.

| Additional repair contract | Result |
|---|---|
| Existing button labels unchanged; sibling Next badge | PASS (source only) |
| Unchanged 4 Hz refresh and suspension path cannot call guidance scene scans/quotes | PASS (source only) |
| Exact allowlist; treatment review/waypoint excluded; non-O1 restore-before-return | PASS (source only) |
| Dual destructive action retains question; both safe choices precede consequences | PASS (source only) |
| Markable-target reason remains the shared instruction | PASS (source only) |
| Cue post-layout timing, real layout/input/performance, save/load lifecycle | NOT RUN |

| Command/check | Actual result |
|---|---|
| `python3 Tools/Verification/NextActionGuidance/check_sources.py` | **39 PASS, 0 FAIL — SOURCE ONLY.** Expanded for immutable labels, exact allowlist/restoration, no expensive application calls, hook below unchanged-content return, paired dual question and rack-reason isolation. |
| `python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod --out /media/jer/Files/CCF/Verification/NextActionGuidance-T2-Repair2-20261106/typecheck` | `TYPECHECK_CLEAN`, six distinct existing baseline errors, zero new errors; approximate 2021.3/stub gate only, not Unity compilation. No baseline edit. |
| Imported `RackV2HelpTeaching/run_offline.py` and invoked `offline(Path('/media/jer/Files/CCF/Verification/NextActionGuidance-T2-Repair2-20261106/help-offline'))` | `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`; no whole historical ownership-check claim. |
| `python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | Exit **2**, old `buttons.Add(UiKit.Button(` anchor absent in Objectives. Partial 01–07 and six SnapFar cases pass; aggregate NOT PASS. Protected checker unchanged. |
| `git diff --check`; changed-path and temporary Assets inventory | PASS. Import-created Python `__pycache__` removed; no disposable Assets harness/meta. |
| Unity/compiler/runtime/native input/rendering/profiling/save/reference gates | **NOT RUN**; source-only repair authority. |

Durable evidence root: `/media/jer/Files/CCF/Verification/NextActionGuidance-T2-Repair2-20261106/` (source-check log, approximate typecheck outputs and Help offline project). Partial historical-check and Help stdout were observed in this worker's tool output; this record does not imply an archived raw stdout file for those two runs.

For exact-candidate Unity follow-up: open Objectives then Map in fresh Enlarged80 Rack V2; expect unchanged Plan Racks label with adjacent Next badge; exercise valid/invalid-five, drawing, confirm and 0–5 marks; expect only truthful scoped guidance or reason. Trigger single/dual consent/reset/reopen and Advance warning; expect safe-first/no destructive primary, attached consent question and no stale focus/activation. Use Help/session/reference and reopen/load at each lifecycle boundary; expect prompt cue suspension/recovery and refreshed facts. Inspect treatment-review Inspect/Prepare/Approve and waypoint primaries, then later-objective and Legacy40 forest/Advance/results primaries; expect original styling retained. Repeat at 720p/1080p with native keys, disposable save and profiling, and run the unchanged exact-label/protected regression gates. These are instructions and expected observations, not executed results.

Ready for independent source **re-review**, then Manager acceptance. #296/T3 remains gated on those outcomes. No integration, release, Unity lease or playtest readiness claimed. Manager/orchestrator coordination/dashboard ingestion and published-state readback remain **NOT VERIFIED / outstanding operational handoff**; no generated dashboard files or GitHub comments were written.
