# MFM-09 — Teaching Sequence / Work Plan Integration — HANDOFF

**Claim:** #207 (parent #203 / #201 / #91) · **Worker:** Claude B (`claude-sonnet-5-5`), resuming a Claude A salvage checkpoint after a provider rate limit · **Date:** 2026-10-31/11-01
**Status requested:** `HANDED OFF — AWAITING MANAGER REVIEW`. This is a worker handoff, **not** a Manager acceptance and not an integration claim.

**Result token:** `SCENARIO ONE EXTENSION FLOW QUALIFIED` — every required gate below genuinely passed (see §6, §8 for the exceptions that are harness debt rather than MFM-09 evidence).

## 1. Identity and ancestry

| Item | Value |
|---|---|
| Exact released start | `34e360f85835afd15f5323f287372e0f2ee83dd9` (`integration/scenario-one-player-test-ready-v1`) |
| Branch / lane | `feature/mfm-09-teaching-workplan-integration-v1` · `/media/jer/ZX20/CCF-U1` (U1) |
| Salvage checkpoint (Claude A) | `917390a545e7b18519748ffb8210d99b795e5adc` |
| **Tested production candidate** | `6784ab17cac922a16815f4898a30737482101018` — every Unity result below ran on this exact HEAD under one lock |
| Later commits | one offline-audit allowance (`run_offline.py`) and this document; no production/Assets change after the tested candidate |
| Evidence root | `/media/jer/Files/CCF/Verification/MFM09-20261031/` |

Commits on top of the start: `917390a5` (production + offline check, Claude A) · `0250ebc5` (T-05 harness slice) · `ad12f45d` (Unity gate + runner) · `618cb6b1`, `69ec3076`, `6784ab17` (gate fixes, found by running it) · audit allowance + handoff (final commit).

## 2. Owned files

Production (exactly the expected four; verified `git diff --stat 34e360f8 HEAD -- Assets` = 4 files, +92/−7):

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Verification only: `Tools/Verification/Mfm09ExtensionFlow/{Mfm09OfflineCheck.cs, run_offline.py, Mfm09ExtensionGate.cs, run_gate.py}`; one accepted harness extended: `Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/run_offline_check.py` (see §5).

**Not touched:** `ScenarioOneUiRoot.cs`, any MFM-08 / Stand Map / Tree Inspection file, save/schema/version/validator/hash/Reference/migration files, Rack V2, Annual Review/history, contractor/economy code, scenes, prefabs, ProjectSettings, packages. No `.meta` added. No `active-tasks.md` edit, no claim post, no push.

## 3. Teaching IDs

- **Old IDs, unchanged and in order:** `access`, `future-trees`, `release`, `habitat`, `response`, `recruitment`, `establishment`, `transformation`.
- **New stable ID:** `extend-treatment` (`ScenarioOneTeachingSequence.ExtensionId`), presented after `response` and before `recruitment`.
- Visible numbering moves (the extension is lesson 6; Secure the next generation becomes 7 and so on; nine lessons in a world that has the extension). IDs do not move.

## 4. Implementation summary

- **Presentation-only layer.** The extension is deliberately **not** in `Lessons`. `Lessons`, `Current`, `CurrentUnlatched`, `CrossesFirstRackReview`, `RecruitmentAndLater` and the D-066 / T-04P latch are byte-identical to the start (checked by the offline audit). The accepted stage index still reads 5 while the extension is shown, so `ScenarioOneWorkPlanFocus` and all earlier predicates are unaffected.
- **Offered only after a genuine response review:** `ExtensionOffered(f) = f.ExtensionApplies && !f.ExtensionApproved && Current(f) == 5`. `Current` reaches 5 only after the real review (the latched crossing). A save already past recruitment is not pulled back.
- **Derived, never saved:** `ExtensionApplies = RackPlanningV2Active && RackPlanConfirmed` (where the Treatment review exists); `ExtensionApproved = any stored prescription is Approved`. Both are recomputed on every read. Legacy40 / injected facts leave both false and keep the old sequence. No new field, version, hash input or write-on-read/load.
- **Copy:** extension text states the sample is calibration, not a stand-wide stocking prescription; the proposal is not a Fell mark and not work; prescription approval is not Work Plan approval; manual work stays available. The instruction changes from the generic action to "Prescription N is accepted but not yet approved…" when a stored Accepted prescription exists.
- **Work Plan:** explanation only. A group note appears when any open order has prescription origin, and each such order gets one line ("Added by a prescription. Still ordinary Pending work: approve it in this Work Plan to commit it." / "…and since approved in this Work Plan."). `OrderClass`, the three groups and the future/inconsistent-record handling are untouched; provenance never feeds status.
- **No bypass:** no added line calls Work Plan approval, prescription approval/creation, marking, advance-year, latch writes, or adds a serialized field (static audit + mutation controls).

## 5. Offline / static evidence (not Unity)

All under `…/offline-b/` (plus Claude A's `offline-dev/`).

| Check | Result |
|---|---|
| MFM-09 offline suite (`run_offline.py`) | `MFM09_OFFLINE_PASS checks=32`; `MFM09_STATIC_AUDIT_PASS base=34e360f8 changed=9` — `final-suite2/` at the final HEAD |
| MFM-09 mutation controls | `MFM09_MUTATIONS_PASS caught=17 survived=0` (renamed/reused old ID, extension before/without `response`, latch ignored/reopened, outside-sample back-credit, prescription work Approved / fourth status / changes new-vs-carried, fourth label, teaching action calling Work Plan or prescription approval, Work Plan note committing work, latch written) |
| T-04 sequence monotonicity (`run_teaching_sequence_offline.py --base <HEAD> --mutations`) | PASS, `TEACHING_SEQUENCE_MUTATIONS_PASS caught=5 survived=0` — `regression-cand2/` |
| T-04 cap (`run_teaching_cap_offline.py --base <HEAD>`) | `TEACHING_CAP_OFFLINE_PASS checks=48` |
| T-04P persistence/latch/hash | `PERSISTENCE_V23_ALL_HASH_BASELINE_PASS 80 fixture rows`, `PERSISTENCE_PROTECTED_SOURCES_UNCHANGED` |
| T-05 cycle state | `CYCLE_STATE_OFFLINE_PASS checks=93` |
| T-05 Previous/Proposed (+mutations) | `S1D_T05_PREVIOUS_PROPOSED_PASS checks=219 failures=0`; `S1D_T05_MUTATIONS_PASS caught=7 survived=0` (after the slice fix below) |
| S1-C history/query | `S1C_HISTORY_QUERY_PASS checks=55` |
| Non-UI | `NONUI_APPROACH_PASS checks=49` |
| MFM-04/05/06/07/08A, PARITY-08 save | all PASS (`MFM04/05/07/08A_REPEAT_PASS`, `MFM06_SAVE_OFFLINE_PASS checks=172`, `PARITY08_SPECIES_OFFLINE_PASS checks=23`) — `regression-cand/` |

**A real MFM-09-caused verification defect, fixed:** the T-05 Previous/Proposed runner compiles slices extracted from `WorkPlanView`; the two new explanation-only members were outside the slice, so it failed to build (`CS0103 PrescriptionGroupNote/PrescriptionOriginNote`). I extended the slice to include them verbatim (`0250ebc5`); no assertion or mutation changed; the run now passes with all 7 mutations caught.

## 6. Harness debt — reported, not counted as green evidence

Run identically on a clean checkout of `34e360f8` (`regression-base/`) and the candidate (`regression-cand/`):

- `ScenarioOneUx/run_ux_check.py` — `SCENARIO_ONE_UX_FAIL failures=1` (*the walking HUD has a short first-use text*), **identical FAIL line on the unchanged start**; pre-existing.
- `RackV2HelpTeaching/run_offline.py` — `failed=1`, **identical on the start**. Its S10 file-set boundary also lists MFM-09's owned files on the candidate (expected: it pins another task's ownership); its 15 other checks pass.
- `MfmTreatmentReview/run_treatment_review_offline.py` — `MFM08_BOUNDARY_FAIL`, **also fails on the unchanged start** (pins a stale `cfdc93bf` base). The new entries on the candidate are exactly my four production files and the two Tools files.
- `run_teaching_sequence_offline.py` / `run_teaching_cap_offline.py` at their default stale bases (`bccd2b90`, `23fe4b8`) or at `--base 34e360f8` report "unowned production changed" naming exactly `LearningObjectivesView.cs` and `WorkPlanView.cs` (MFM-09's owned surface) — a boundary check for another task. Their behavioural/mutation halves are green only when run with `--base <candidate>` (§5); that boundary check itself is **not** claimed green. `run_teaching_cap_offline.py --mutations` also stops on a stale mutation pattern (`advisory before completion … found 0`) on the unchanged start.
- `TeachingPersistence/run_offline.py` needs `--out` (my first runner call omitted it; rerun green).

## 7. Unity Tier-2 (Unity 6000.6.0f1, serialized, isolated)

`/media/jer/Files/CCF/Verification/MFM09-20261031/unity/run4/` — one lock, one head `6784ab17`, compile receipt `01-compile`, all-gate table `receipts.md`.

Environment: owner `Claude-MFM09-B`, task `MFM-09`, lane U1, private Xephyr `:84`, isolated `XDG_CONFIG_HOME` (host save untouched in every receipt), `TMPDIR=/tmp/`. All 29 receipts: `host_saves_unchanged`, empty `remaining_lane_changes`, harness/`.meta`/licence copy removed, no cleanup failure.

### New MFM-09 gate — `MFM09_UNITY_PASS checks=261 shots=16` (`02-extension/`, harness sha256 `92c3ef90b664`)

Proven live (production APIs plus real OS `[E]`/`[J]`/Esc keys on the private display):

- **Order:** before the sample, nothing is offered; at Lesson 5 the title is `5. Return and review`, the HUD reads 5/9, no extension wording, no latch. Three real `[J]` decisions plus six stand portions earn the D-066 latch; only then `Current==5` and the lesson is `6. Extend your treatment` (HUD 6/9; Completed lessons lists Lesson 5, not the extension).
- **Latch/save:** the latch is in the isolated slot; after a real disk `Save()`/`Load()` the completed first review stays completed, stage index stays 5, the extension is still shown; the slot holds no `extend-treatment`/`extensionApproved`/`extensionApplies` and save version is 24.
- **Outside-sample / comparable-area:** a Fell order outside the sample before the review changes no stage and awards no sample credit; later comparable-area Crop Trees and executed comparable-area work leave the sample's future set, executed removals and the latch unchanged.
- **MFM-08 review consumed, not changed:** opening it is not progress; Prepare stores an Accepted prescription (no work, no marks) and the lesson text then names it; a real `[E]` spot-check enables Approve; UI Approve creates exactly the previewed ordinary **Pending** FellTree orders with prescription/coverage-unit/Crop Tree provenance, none Approved, no cash spent, no tree felled, no mark; the extension completes (derived) and the lesson returns to `7. Secure the next generation` with stage index and latch untouched.
- **Work Plan:** exactly three groups; `Approved · 0 | Pending new · 86 | Pending carried over · 0`; all orders listed with origin line and "Still ordinary Pending work" explanation; the group note equals `WorkPlanView.PrescriptionGroupNote`; the MFM-08 layer/card is not shown on the Work Plan screen.
- **Year boundary:** after an idle year the same orders are `Pending carried over · 86`, still Pending, origin lines retained, extension still complete.
- **Commitment:** `ApprovePendingWork` is the only commitment step (cash and trees unchanged after approval); the ordinary year execution completes the orders, changes cash and fells the trees through the existing economy path.
- **Manual work:** a manual Fell mark still becomes ordinary Pending work with no prescription provenance; the note is absent when no open order has prescription origin.
- **Render:** laid-out/inside-window assertions and captures at **1280×720 and 1920×1080** for the Objectives extension, review card, prepared prescription, approved-Pending review, Work Plan Pending new, Pending carried over, Approved and manual-work states (16 PNGs in `02-extension/evidence/`; I viewed the Objectives 1280×720, Work Plan Pending new 1280×720 and Work Plan carried-over 1920×1080 captures and they are readable).

### Protected regressions — identical markers to the accepted S1-D T-08 receipts on the same Assets

`receipts.md` compares each marker with `/media/jer/Files/CCF/Verification/S1D-T08-20261031/receipts.md` (`0dfa7ea2`, identical `Assets/`). Every check count matches:

compile · objective `S1C_T05_UNITY_PASS checks=544` · MFM-08 treatment-review `MFM08_UNITY_PASS checks=373 shots=25` · MFM-07 approval `MFM07_UNITY_PASS checks=106` · T-04 guide `S1D_T04_GUIDE_UNITY_PASS checks=93` · T-04P teaching-latch `TEACHING_LATCH_UNITY_PASS checks=60` · T-05 previous/proposed `S1D_T05_UNITY_PASS checks=579` · S1-C t02/t03/t04/t06 (`246/279/529/170`) · reference `REFERENCE_FUTURE_V1_PASS` · hardening · progress · interaction · n3 · cap `223` · lesson `LESSON_PASS checks=36` · teaching `S1A_TEACHING_VERIFY_PASS checks=61` · Rack V2 runtime `RACKV2RUNTIME_VERIFY_PASS checks=14` · event-id · completion (growth model 0 and 1) · menu · baseline-hash `T05_BASELINE_HASH_PASS cases=30` · density-save · render `T05_RENDER_PASS captures=6`.

One gate failed once and was re-run: `25-render` — the runner expects a sibling `02-objective` directory (the same trap T-08 recorded); with a symlink to the objective receipt, `29-render` passed (`T05_RENDER_PASS captures=6`). `02-objective` in `receipts.md` is that symlink, not a second run.

## 8. Not verified / limits

- Not Windows/player-build verified (out of scope). Not a Manager review. No independent-provider review was run.
- The Unity gate was not itself mutation-tested (the harness references members that do not exist at the start, so a pre-fix run would fail to compile rather than fail meaningfully). Production correctness is mutation-covered offline (17 mutants) and live-covered by the gate.
- The comparable-area Crop Trees were confirmed through `ForestTreeMarkingManager.Mark` (declared fixture, as in the accepted MFM-08 gate); real `[C]`/`[N]` key behaviour is covered by the accepted MFM-08 gate re-run here (`MFM08_UNITY_PASS checks=373`).
- The extension shows no picture of its own; it reuses the Lesson 5 diagram by design (`TeachingDiagram(4, …)`).
- A world with no Covered area, or whose player never prescribes, keeps showing the extension in place of the recruitment text until a prescription is approved or the accepted stage moves past recruitment (completion is derived and nothing may be saved). I did not add an escape because it would need new persistence or a rule change; **Manager decision recommended** if players could be stuck without a Covered area.
- Host load average was 12–20 during the chain (other workers); durations in `receipts.md` are inflated (`19-lesson` 712 s vs 241 s in T-08) with identical results.
- Orchestration ledger/coordination board were not edited by me.

## 9. Lane / lock / cleanliness receipts

- Lock acquisitions and releases, normal (never forced), each at the exact new HEAD: `locks/00-acquire-ad12f45d.json` → … → `04-acquire-6784ab17.json` → `final-release.json` (final lock id `380f3eeece3444a19bfa84de8c13ba87`, owner `Claude-MFM09-B`, task `MFM-09`, ETA `2026-11-01T02:30:00Z`). Every Unity phase ran under a lock naming the exact HEAD. `unity-lock status` after release: `free`, no Editor PID.
- U1 after the last gate: `git status --porcelain` empty (including untracked), no Xephyr/Unity process, no staged harness/`.meta`/licence copy, Editor/material/`.vscode` side effects restored by the runner (patches recorded per gate).
- Temporary baseline worktree `/tmp/mfm09-base` removed and pruned; `__pycache__` created by early offline runs removed.

## 10. Exact commands

Offline (from U1): `python3 -B -I Tools/Verification/Mfm09ExtensionFlow/run_offline.py --out /media/jer/Files/CCF/Verification/MFM09-20261031/offline-b/final-suite2 --mutations`

Unity: held lock, then `/media/jer/Files/CCF/Verification/MFM09-20261031/rungate.sh run4 <NN-name> <gate>` (compile first), which runs
`python3 -I -B Tools/Verification/Mfm09ExtensionFlow/run_gate.py --out …/unity/run4/<dir> --lock-id <id> --owner Claude-MFM09-B --expected-head 6784ab17… --gate <gate> --tools-root …/tools-t08 --fixtures …/S1D-T08-20261031/fixtures`
(`tools-t08` is `git archive 0dfa7ea2 Tools` plus the MFM-08 gate from `c9336186`, read-only sources for the historical harnesses). New gate: `--gate extension`.

Markers: `MFM09_UNITY_PASS checks=261 shots=16`, `MFM09_OFFLINE_PASS checks=32`, `MFM09_MUTATIONS_PASS caught=17 survived=0`.

## 11. Orchestration Decision Appendix — V2 (orchestrator)

This appendix records the orchestrator's routing and capacity evidence. It does not alter any gate result above.

### 11.1 Policy

- **Orchestrator:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`), #203/#201.
- **Execution:** Claude Sonnet 5.5 Medium, with a capacity-driven pool switch: Claude A first, then Claude B after Claude A's API rate limit.
- **Objective:** implement and Tier-2 qualify the approved `Extend your treatment` teaching / Work Plan integration on exact `34e360f8...`; publish the handoff.
- **Hard constraints:** preserve all eight teaching IDs and the v24 D-066 latch; extension after `response`; prescription approval creates ordinary Pending work only; Work Plan approval remains the commitment gate; consume MFM-08 UX; no save/schema/hash/Reference/Rack/economy change; no Windows build; #194 NO ACTION.
- **Routing ladder:** DeepSeek orchestrates/defaults for bounded work; Sonnet 5.5 Medium for genuinely hard cross-system work; Opus only after a classified Sonnet capability/reasoning failure or separately justified high-consequence need.
- **Concurrency/review:** one writable lane, one primary writer, one Unity lock; no independent-provider review was run, so Manager review remains outstanding.

### 11.2 Capacity and context

| Pool | State | Model / effort | Capacity evidence | Work | Capacity-to-completion |
|---|---|---|---|---|---|
| DeepSeek | AVAILABLE | deepseek-v4.1-flash | runtime identity | orchestration, evidence inspection | sufficient |
| Claude A | COOLDOWN after rate limit | Sonnet 5.5 / medium | provider error text `Claude API rate limit reached. Try again later.` | partial implementation `917390a5` | insufficient after limit |
| Claude B — Pro | AVAILABLE; completed | Sonnet 5.5 / medium | provider catalogue; runtime completion | salvage/finish/Unity | sufficient (observed) |
| OpenAI/Codex | UNKNOWN / not used | — | not queried | none | UNKNOWN |

- **Orchestrator context:** numeric usage/limit not observable; no compaction reported. **UNKNOWN**.
- **Worker context:** Claude A ended on the rate limit; Claude B completed without a reported context/provider failure. Numeric usage **UNKNOWN**.
- **Reset/window:** not supplied; not inferred.
- **Switching cost:** low — Claude A left a clean committed checkpoint `917390a5`; Claude B resumed on the same lane/HEAD with no duplicate writer.

### 11.3 Routing and decomposition

- **Work unit:** #207 MFM-09, parent #203/#201.
- **Task characteristics/risk:** Medium/High cross-system teaching + Work Plan production change with Tier-2 Unity, save/latch and commitment-boundary checks.
- **Decomposition:** one coupled implementation/qualification unit; a second parallel writer would have required lane/lock handover with no independence benefit.
- **Initial selection:** Claude A / Sonnet 5.5 / medium (packet difficulty; user direction to use Sonnet, Claude A first).
- **Capacity event:** Claude A API rate limit after committing `917390a5`; classified as provider capacity, **not** capability or reasoning failure.
- **Fallback:** Claude B — Pro / Sonnet 5.5 / medium on the same checkpoint; same model family, second subscription pool; no Opus.
- **Alternatives rejected:** Opus (no classified capability failure); DeepSeek direct implementation/Unity (harder cross-system work remains Sonnet-class under the routing policy); Codex (not needed while Claude B was available).
- **Exploit/explore:** exploit the partial offline suite and four-file production checkpoint; explore only the missing Tier-2 gate/regression completion.

### 11.4 Outcomes and resources

- **Claude A:** partial production + offline suite; `MFM09_OFFLINE_PASS checks=32`, 17/17 mutants caught; then rate limit.
- **Claude B:** completed the T-05 harness slice fix, Unity Tier-2 gate and runner, 29 Unity receipts (one render harness path failure re-run as `29-render`), offline/regression evidence, handoff.
- **Tested production candidate:** `6784ab17...`; later commits are verification/offline-audit and handoff only.
- **Production diff:** exactly four expected UI files, +92/−7; no save/schema/hash/Reference/MFM-08/Rack/UiRoot change.
- **Evidence:** `/media/jer/Files/CCF/Verification/MFM09-20261031/`; lock final id `380f3eeece3444a19bfa84de8c13ba87`, normal release, lane clean, lock free.
- **Failure classes:** provider capacity (Claude A); harness path/base debt (render sibling-directory, stale regression bases, pre-existing UX/RackV2 help failures). No MFM-09 production defect established.
- **Resource observations:** Claude A work before limit; Claude B completed the remaining chain; wall-clock not precisely attributable across the handoff. Token/context counts **UNKNOWN**; no proxy is presented as measured.

### 11.5 Learning assessment

- The second Sonnet pool recovered the task without Opus; the committed checkpoint made the switch cheap.
- The no-Covered-area/no-prescription extension residual is a genuine product/teaching decision, not a gate failure; it is flagged for Manager.
- Verification debt is recurring: stale regression bases and harness assumptions (render sibling directory) cost retries. Pre-reconciling them before a long Unity chain would reduce waste.
- Independent review was not run; the token is a worker qualification, not Manager acceptance.
- **Evidence rule:** receipts/Git/lock/logs are observed facts; routing rationale is an orchestrator claim; sufficiency/learning statements are retrospective inference.

## 12. Next action

Manager review of this handoff and the evidence root; decide the no-Covered-area residual (§8); then integrate or request changes. Nothing is pushed.
