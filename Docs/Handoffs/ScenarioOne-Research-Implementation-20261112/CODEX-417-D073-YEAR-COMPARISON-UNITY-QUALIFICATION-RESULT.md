# CODEX #417 — D-073 Year Comparison Tier-2 Unity qualification

**Verdict: FOCUSED D-073 CHECKS PASS — READY FOR PARENT T-06 REVIEW.** This is a worker qualification result, not independent review, integration, Manager acceptance or full Scenario One certification. No production change or Player/Windows build.

[Claim #417](https://github.com/jermur788/CCF/issues/417), bounded child of [#412](https://github.com/jermur788/CCF/issues/412). Parent retains orchestration, G2/G3/T-06 and integration; paused Fable parents were not transferred or resumed. Work performed directly, no subagents or competing task.

## Identity, authority and scope

- Actual endpoint: provider instance **`codex`**, display **Codex**, model **gpt-6.1-sol**, **HIGH**, **default tier**, Codex harness in T3 Code. Account/subscription and remaining quota **UNKNOWN**, not inferred. No ChatGPT–OpenAI B substitution or provider retry/fallback.
- Read-only source: `feature/scenario-one-d073-annual-review-v1-20261112` at **`77a56a4461f8ab2d313082af50442ce24dc54471`**, tree `ca987a31fbc05896e0c8152c5028f7f4606899b3`. Remote rechecked before each Editor lease and at closure, unchanged.
- Candidate developer worktree `/media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1` remained clean at that source SHA.
- Qualification branch: **`qualify/scenario-one-d073-year-comparison-unity-20261112-v1`**, created from exact source. Sole writable Unity lane: **ResearchCopyQual391**, `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`; initial clean lane HEAD `dcb319f9eb93b16d60a739bd907c65de1d4204e7`. Both Git stores inspected for ownership/conflicting checkout before creation. Own warm Library retained, never copied.
- **Core tested runner:** `b0775321e1494c60449bbdc3200db9b9cce8ea10` (runtime1). **Final native/rendered tested runner:** **`0d624c61bb8852850a07b4b33d83e740a032c86c`** (runtime5). Later publication is handoff/driver-documentation only, not another tested gameplay SHA.
- Context identity from candidate lineage: `63044dfbbeb869996ceecec8a47619dcba048b0b`; canonical entry/workflow/context read alongside the current binding packet and live GitHub main coordination. Approved D-073 four-file spec fetched through authenticated gh API from `docs/manager-d073-approved-scope-20261112-v1` (approval HEAD recorded by #416: `53dec8db19a9bdde20176b2f3abf508402c7f0af`). No canonical/spec checkbox or Manager coordination edits.
- Binding [Manager recovery packet](https://github.com/jermur788/CCF/blob/docs/manager-dual-fable-dispatch-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-417-D073-UNITY-QUALIFICATION-CODEX-RECOVERY-20261112.md) and latest direct user authorisation govern this sole #417 execution.
- Evidence root: **`/media/jer/Files/CCF/Verification/D073-YearComparison-417-20261112/`**. Raw logs, screenshots, old-save copy and custody receipts remain on Files. No new durable evidence path on ZX20. Short `TMPDIR=/tmp/ccf417` used for this run.

## Accepted-item matrix

PASS means the bounded check described here actually ran. Runtime1's completed named cases are retained; its overall halted run is not called a suite PASS. Runtime5 completed the remaining native/rendered matrix normally.

| Accepted item | Result | Actual evidence / limit |
|---|---|---|
| Unity import and C# compile | **PASS** | Unity **6000.6.0f1**; zero `error CS` in all five Editor logs. Real mapper/helper/history production types compiled and executed; no offline type doubles staged. Known pre-existing Editor Search startup exception reported below. |
| AC-1 no completed year | **PASS** | Actual fresh review has no comparison card; original empty message. Runtime1 case 2 checks; mapper controls also cover uncompleted/invalid input. |
| AC-2 latest-year discovery / footer | **PASS** | One muted latest-Year-6 line; existing ACK footer priority. Actual latest ACK, Esc and Back to forest at both sizes; earlier selection hides/guards ACK. |
| AC-3 exact earlier comparison | **PASS** | Actual Year 3 vs latest Year 6: two planting outcome events, exact dated snapshots and five derived differences. Years 1–6 card contexts captured. No work is inferred from current trees. |
| AC-4 missing/ambiguous/no-work states | **PASS** | Unmodified 132-assertion fixture on actual Unity types covers every mapper state, both-missing and misdated snapshots, latest precedence and no-work. Rendered selected-missing, latest-missing, ambiguous-species and one-sided-species controls at both sizes: 13 checks per size. |
| AC-5 provenance exclusions/disclosure | **PASS** | Real-types fixture tests duplicate event identity, ambiguity, unresolved outcomes and nonduplicated warnings; missing/inconsistent retained orders disclosed event-only. Extra duplicate-order-ID control excludes both suspect rows. Created/approved intentions do not become outcomes. These are synthetic negative controls, not old-save compatibility proof. |
| AC-6 species and units | **PASS** | Oak/Beech/Sitka real display names, four separate natural/planted measures, one-sided absence explicitly not recorded rather than zero. Canopy fraction displayed as percent; differences in points. Positive and gap renders inspected. |
| AC-7 saved state/events/world hash | **PASS** | Runtime1 13 checks compare exact pre-card view actions against new-card actions, including ACK: canonical full state and world hash equal. Viewing alone leaves state/events/hash unchanged. Native view actions also unchanged at both sizes; real ACK adds exactly the existing one acknowledgement event. |
| AC-8 one-year vs long advance | **PASS** | Actual same authentic Year-4 start, ordinary UI annual advances vs actual skip to Year 6; all card fields for all completed years equal **without card normalisation**, annual reports equal. Explicit raw history/ACK distinction below. |
| AC-9 native input / rendered fit | **PASS** | Runtime5 **35 native checks at 1280×720 + 35 at 1920×1080**, actual xdotool pointer, wheel and keyboard Escape through private Xephyr. Year buttons, foldout, toggle/rebuild, wheel, footer/ACK and Back to forest work. Exact PNG dimensions independently checked; visual limitations below. No shared-style change needed. |
| AC-10 current / genuine old / Legacy40 / Reference | **PASS** | Current disk round-trip exact state/hash (3 checks); genuine unmodified v9/year39 host save in Legacy40 (9); raw frozen v12 archive integrity, tamper control and actual four preview/return worlds (22). No synthetic version rewrite used as compatibility proof. |
| AC-11 bounded wording | **PASS** | 132 real-types mapper assertions/43 cards sweep wording/provenance, currency and unsupported claims; actual comparison/caveat inspected. Existing surrounding Money/history UI is unchanged and is outside the new card. |
| AC-12 like-for-like Work counts | **PASS** | 21 authentic checks: felling/planting/pruning/salvage common valid event categories agree in Years 1–5. Report-derived removal and malformed-source differences explained below, never forced equal. |
| Read-only rebuild / placement / no new stored fields | **PASS** | Card remains in original Annual Review scroll (nearest ScrollView identity and content membership). Same card instance across 30 unchanged frames; one guarded Build call in inspected unchanged wiring. Full saved state protected; no new production writes or schema/event/ACK path. |
| Exact production tree / host custody / lease cleanup | **PASS** | All **4,011** original tracked Assets/Packages/ProjectSettings/shared-settings hashes restored exact after every session; all **37** original host custody hashes exact; five normal lease releases, clean lane, FREE/no Editor at closure. |
| Independent T-06 / integration / acceptance | **NOT RUN** | Parent #412 responsibility, expressly excluded from this execution. G3 review of harness corrections remains required. |
| Blanket cadence matrix / Player / Windows | **NOT RUN** | Deliberately excluded. #408 unchanged-feature evidence retained; no 143-case cadence rerun or build. |

No required fixture was impossible or silently replaced. No feature defect was established in this bounded qualification. **Three harness assumptions needed correction; all four stopped runs and their true FAIL/exit1 results are retained.**

## Gameplay, history and hash evidence

The inspected prior `dcb319f9` stage-seven driver supplied the production setup sequence: real Rack V2 drafting/confirmation/entrance marking, Work Plan approval/annual execution, three distributed Crop Trees and release, production inspection/decision APIs earning the historical review latch, genuine stock purchase/designation/planting and establishment. There is no stage-index assignment, cached teaching-fact injection or fabricated order/event stream for the authentic fixture. Setup is harness-driven through production APIs; physical-input claims concern Annual Review, not a claimed native forest walk.

Runtime1 `authentic-stage7.json` is Year 4. Ordinary UI steps and actual long advance each reach Year 6. `manual-cards.json` and `skip-cards.json` compare every card/row field directly, including text, state, years, units and provenance. The selected native comparison is Year 3/latest 6, with actual planting events and the same snapshot/record context. Source fixture SHA and provenance are in `resume-fixture.json`; resumed runs did not rerun the cadence suite.

Raw manual/skip hashes are **`6DF6B75E8B957F8C` / `8B0BDF75F9BC3816`**; raw event counts **538 / 537**, ACK counts **5 / 4**. These are deliberately **not** claimed raw-world parity: the manual path genuinely acknowledges the intermediate Year 5, whereas skip does not fabricate that ACK. Independent JSON audit removes only AnnualReviewAcknowledged=9 and renumbers event IDs/nextManagementEventId, then proves **every remaining saved field identical** (`parity-semantic-audit.json`). Card equality itself used no such normalisation. No record was written back or inferred.

AC-7 uses a separate class renamed from exact pre-D073 `6baee47bd677f48e763b7e8c10092eaf45ba4c79:Assets/ForestPrototype/UI/AnnualReviewView.cs`; `baseline-fixture-proof.json` verifies byte equality after the class/constructor rename only. With-card and without-card saved data are retained as `runtime1/evidence/AC7-{with,without}-card.json`. Looking does not acknowledge; latest ACK preserves its existing event semantics.

AC-12 boundaries: the existing Work column takes successful felling/planting/pruning/salvage from the same event projections. Its young-tree removal total comes from `year.Report.regenerationRemovalTasks`, while the new card counts valid recorded removal-resolution events. A report/event discrepancy is therefore not equality-tested as a shared source. The inspected clean fixture has one removal event and a default report total of zero; the new card truthfully says one recorded event outcome. Its malformed duplicate-event fixture has two raw succeeded fells that the old column would count, while the new card says zero counted with the explicit two-record ambiguous-provenance warning. Both behaviours were exercised by the real-types fixture; no counter or warning was weakened.

## Genuine compatibility and frozen anchors

Genuine old source: `/home/jer/.config/unity3d/DefaultCompany/CCF/forest-save.json.bak`, copied byte-exact to `genuine-host-v9-year39.json`, SHA-256 **`c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3`**. It is v9/year39, not synthetic old JSON. All **361** tree IDs/positions and shared original tree-state field values match after load (zero differences); Legacy40/year39 retained, no invented history/card data, ordinary advance succeeds. `genuine-v9-audit.json` records this separately from synthetic provenance controls.

Frozen raw embedded hashes verified without current-schema reserialization: Year 0 **A564039D9B7CE31D**, 20 **F7DF7DAB53B6FD32**, 50 **D5E75D6D21D631AC**, 100 **7AD177B3CC2F73C7**; schedule **56C8B99FA1E8DDD1**. In-memory Year-100 tamper was detected. Actual preview/load at all four years stays Legacy40, review browsing is unchanged, and ending each preview restores the complete current player world exactly. Original archive/schedule file SHA-256s are in `source-identity.json` and unchanged in the final production manifest. No frozen biology replay/continuation retuning or anchor rebaseline.

## Rendered/native evidence and limits

`runtime5/evidence/` contains **18** exact-dimension Game PNGs: for each size, `native-<size>-latest-footer`, `-earlier-sources`, `-expanded-species-caveat`, `-earlier-no-sources`, `-acknowledged`, plus `negative-<size>-missing-selected`, `-missing-latest`, `-ambiguous`, `-species-one-sided`. `screenshot-audit.json` records each SHA-256 and pixel dimensions. Label text/bounds dumps accompany every image. `runtime5/driver/native-actions.jsonl` and before/after desktop PNGs prove OS-event delivery; no callback invocation substitutes for these native cases.

Inspected both positive resolution sets and all eight negative images. Text wraps, chips show/hide, natural/planted rows remain separate, fixed header/footer retain priority, the caveat is reachable/readable and actual wheel offset changes. Actual species gap names **European beech**, missing for Year 1 and recorded for Year 6. No substitute latest values or differences appear when the latest snapshot is missing.

A capture is one scroll position, not the whole long card. The 1080p missing-latest capture shows selected-only values with its final gap message below that position; exact text is in its label log and is visible in the 720p counterpart. The 1080p missing-selected image fully shows the required missing-snapshot state. Exported Game PNGs have exact requested dimensions; the GameView is scaled inside the private Editor desktop. These are Linux Editor evidence, not standalone/native-Windows input or performance claims.

## Run ledger and measurement corrections (G3 input for T-06)

| Run | Tested runner | Editor exit / outcome | Reason / retained result |
|---|---|---|---|
| 1 | `b0775321e1494c60449bbdc3200db9b9cce8ea10` | **1 / overall FAIL**, 318.3 s | Eight named core cases completed PASS, then invalid logical-canvas equality stopped native entry. |
| 2 | `7b8ca89276f901a47045f49125c1fdfdbbcd66ef` | **1 / overall FAIL**, 198.2 s | Exact 720p/natively selected comparison reached; overstrict immediate-parent assertion stopped. |
| 3 | `ada4a0dd84e26b02305528ce3799b1d17badaee6` | **1 / overall FAIL**, 200.1 s | Diagnostic kept same assertion and disproved stale-reference hypothesis. |
| 4 | `e6d80838acc2536b9b31af0469b8444ff8ebabd5` | **1 / overall FAIL**, 260.2 s | Correct scroll membership/wheel PASS; harness loop mishandled existing -1 latest sentinel. |
| 5 | `0d624c61bb8852850a07b4b33d83e740a032c86c` | **0 / PASS**, 498.4 s | **99 checks / 18 captures**; native 35+35, rendered negative 13+13; normal exit, no reaper. |

1. PanelSettings uses logical reference canvas 1600×900; comparing that to requested raster pixels was wrong. Correct oracle requires exact Screen.width/height **and** independent PNG dimensions. No resolution tolerance was relaxed.
2. FR-1/AC-9 require the existing scroll, not `VisualElement.parent == ScrollView.contentContainer`. Diagnostics found `oldAttached=True`, `oldIsCurrent=True`, matching original ScrollView ancestor; full logical chain is card → original ScrollView → modal → root. The internal contentContainer is a different object. Correct oracle requires original-scroll ancestor **and** contentContainer membership. Earlier wrapper/stale guesses were disproved and are not the final explanation.
3. Existing `StepYear` (:807–818) sets `viewYear=-1` for follow-latest. The harness wrongly continued clicking disabled Later after reaching it. Correct bounded loop terminates at -1 and explicitly requires Later disabled.

No production/shared-style edit, expected card text, world/event/hash assertion or accepted criterion changed. `measurement-corrections.md`, all raw logs and `run-audit.json` retain the failures and diagnostics. **Parent T-06 must review measurement integrity; this worker does not approve its own G3 gate.** Each log also contains the known pre-existing `UnityEditor.Search.SearchDatabase.EnumerateAll` startup ArgumentOutOfRangeException (SearchDatabase.cs:351). It is reported, not mistaken for C# compile failure or concealed as a clean Console. No feature exception in final runtime5.

## Protection, leases and end-state

Fresh preflight independently read physical row `CCF-412-PREDATOR-FABLE-A-D073`, current lane/branch, both Git stores, remote candidate, warm Library, disk/device, saves/Prefs and lock/PIDs. Physical board lint PASS (24 live rows). Initial broad lane-status snapshot reported source/qualification branch-HEAD mapping mismatch during the authorised transition; the assigned row was reconciled to the actual qualifier branch/head, linted, and strict normal lock acquisition succeeded. No ownership drift or force takeover. Prior Claude jobs were terminal per direct dispatch; physical no-Editor/free-lock/clean-lane facts were independently refreshed.

Only the assigned physical row was changed for this child; unrelated board rows and live Manager Git coordination were preserved. Primary `/media/jer/Files/CCF-Primary` stayed read-only with its unrelated dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` byte-exact.

Five normal leases, each taken on clean exact runner HEAD before Editor, released after Editor exit and custody restoration:

- `a3990973f50649f18f923b5793078847`
- `b274b612e71048d88d2ce7cecd149975`
- `eb33dbf7c9c245e8848e7cfdb4e3dd98`
- `791f0101c4a44268bca3397210c0f018`
- `4508b59d15254276968bece92880df6d`

All owner/task/HEAD/ETA/release receipts are `lease*-{acquire,release}.json`. No Editor kill, force release, lease bypass or stale-licence workaround. Each Editor exited by its normal controlled exit; no exit198 occurred.

Disposable six C# copies and generated metas were removed after every Editor. Inspected Editor material/settings normalization was preserved as `lease*-Editor-drift.patch` and byte copies, then only those known paths restored. **4,011 production/shared-setting hashes and all 37 host hashes exact after every session**. End state at 2026-11-12T16:09:40Z: clean qualification lane, candidate dev worktree clean/exact, Primary two dirty files intact, lock **FREE**, **no Editor PID**, ZX20 about **67.75 GiB free**. `end-state.json`, `lock-final.json`, `lease*-production-restored.json` and custody receipts prove this. No main/personal #382 edit, integration, asset deletion, package/dependency change or Player/Windows build.

## Reproduction and handoff

Committed tooling is only under `Tools/Verification/YearComparison/`: D073Verification, renamed baseline view, extracted raw archive verifier, inspected GameView-size/native-input helpers, task-specific Python runner and native driver. The unmodified YearComparisonVerification fixture is staged against real production types; offline doubles are not staged. Production Assets/Packages/ProjectSettings diff against source is empty after cleanup. The only documentation publication is this exact handoff path.

Exact final invocation is in `runtime5-command.json`; it uses `run_unity_417.py --name D073 --method D073Verification.Begin`, six `--files YearComparison/<name>.cs`, fresh output directory, `--env CCF_D073_RESUME_HISTORY=<evidence>/runtime1/evidence/skip-final.json`, `--native --pass-token D073_PASS --fail-token D073_FAIL --timeout 1200`, and `CCF_GATE_LANE` set to the assigned lane. `mkdir -p /tmp/ccf417` precedes it. A normal #417 task lease naming actual branch/HEAD is mandatory; driver refuses absent/wrong lease, another Editor or dirty lane. Core fixture reproduction omits RESUME and supplies genuine `CCF_D073_OLD_SAVE`; never synthesize old-save version fields.

For manual review: open Annual Review, read latest hint/footer, choose earlier Year 3, expand By species, toggle data sources, scroll to caveat, return Later to latest, Escape/reopen, ACK once, Back to forest. Expected: exact dated records, derived differences, gaps honest, no viewing mutation, existing ACK only, header/footer stay available. Repeat in 1280×720 and 1920×1080. Keep synthetic negative histories separate from authentic/current and genuine old-save evidence.

**Worker assessment:** ready for T-06 evidence/spec/G3 review; no production fix requested by this result. Entire-feature acceptance/integration remains **NOT VERIFIED** here. No mandatory T-05 item is left NOT RUN. Required exclusions stay NOT RUN as listed above.

**Dashboard synchronization: NOT AVAILABLE / OUTSTANDING.** No publisher/ingestion command or dashboard workflow was available in inspected deployed Ops/scripts, repository Tools and .github. Physical child record and GitHub claim/report are updated; automatic audit advance and published dashboard reflection were not verified. No generated dashboard state was edited. Parent #412 / Manager must run/confirm the established downstream mechanism and update its own live coordination. Drive mirrors were not changed.
