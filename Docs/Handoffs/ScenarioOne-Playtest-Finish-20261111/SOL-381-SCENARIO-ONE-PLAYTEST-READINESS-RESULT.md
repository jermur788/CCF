# #381 Scenario One personal Editor readiness

Verdict: **BLOCKED** — exact-candidate Tier-3 complete-loop qualification is NOT RUN. No newly demonstrated production defect; no readiness or user acceptance token issued.

Operator: T3 Code / **GPT-6.1 Sol, MEDIUM**. All new checks are **AUTHOR TESTS**. No delegated agent or independent reviewer used. Manager #200 owns the separate readiness/G4 decision.

## 1. Exact candidate and source inventory

Candidate branch: `integration/scenario-one-personal-playtest-candidate-20261111-v1`.
Exact game source: `2aaacad68ed186dbe14531f31ce5bb3a87811441`. Starting/report-evidence base: `c28f40870ddcfd36dc5d3c3313e6c8fad7490eef`; Assets/Packages/ProjectSettings diff between these is empty. This result adds documentation/evidence only. No cherry-picks, production edits, package changes, asset additions or integration into main.

Source worktree: `/media/jer/Files/CCF/Operations/CCF-S1-Playtest-Readiness-Sol`.
Unity lane: `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual`, unchanged branch `qualify/scenario-one-tagged-text-379-20261111`, exact game source above. Files source worktree is never an Editor launch target.

AUTHOR ancestry checks (`git merge-base --is-ancestor <SHA> 2aaacad6`) exit0 for personal #361 `fb599eecde983552dec4f06fb9efcf8e38397aed`, S1-D `34e360f85835afd15f5323f287372e0f2ee83dd9`, PARITY-09 `3acdf9cb635cdca79606335801e97f9d7dcce8b0`, MFM-12 `e42bb9f437e4fcf20c5b753dacd1136470daf33e`.

Main comparison base `50042bbe97734b031210ddebb603b5ec00ff459d`: `git diff --name-only 2aaacad6...origin/main -- Assets Packages ProjectSettings` is empty. Main-only changes are documentation/coordination; no newer accepted production changes need transfer. Direct tip differences include the already-staged Workbench removal, Linux package alignment, D-072 map, onboarding, feedback and UI changes; blindly replacing candidate with main would lose them. No blind merge performed.

Canonical candidate context commit: `63044dfbbeb869996ceecec8a47619dcba048b0b`; live active-tasks read from origin/main `0338f14b` (full fetched ref retained in evidence). Milestone prose still says PARITY-09 unreleased; later accepted qualification reports below supersede that implementation-status sentence, not product authority.

## 2. Feature / acceptance matrix

Status meanings: IMP = implemented; REV = earlier accepted review; INT = present in this candidate lineage; UNITY = evidence scope, not a blanket current-candidate pass; USER = acceptance of this exact candidate. PARTIAL means stronger evidence is missing, not a forced green.

| Required feature | IMP | REV | INT | UNITY | USER | Evidence / player effect |
|---|---|---|---|---|---|---|
| Entry, Rack V2, five-rack headland/feeder map and entrance marks | PASS | PARTIAL | PASS | PARTIAL inherited | NOT RUN | #361 ancestry and #359 map witness; D-072 sources staged by #354/#355. Old map/diagram source-pin failures do not prove current native parity. |
| Walking, aim, inspect, X/C marks, quote, separate approval/advance | PASS | PARTIAL | PASS | PARTIAL | NOT RUN | `ScenarioOneUiRoot`, `WorkPlanView`, `ScenarioOneManager`; #379 native X/F5/F9 and callback approval/advance inherited. This run's narrow results below. |
| Latest unread review, Back before ACK, historical read-only | PASS | PARTIAL | PASS | PASS inherited scoped #379; source AUTHOR PASS | NOT RUN | Historical callback guarded; Back closes; Annual Review has no advance-year action. #370 unread reset retained. |
| Sitka/Oak/Beech relevant parity, safeguards and persistence | PASS | PASS inherited | PASS | PASS earlier lineage; exact candidate NOT RUN | NOT RUN | [PARITY-09 Manager review](../S1-ThreeSpeciesParity-P0-Implementation-09/MANAGER-REVIEW-20261101.md). Comparable mechanics do not imply equal empirical precision; evidence/ordinal limitations retained. |
| MFM guided sample, candidate C/N, proposals vs commitment | PASS | PASS inherited | PASS | PARTIAL; new native C/N NOT RUN | NOT RUN | [MFM-12 acceptance](../MFM-12-Integrated-Qualification/MANAGER-REVIEW-20261103.md); authentic current late-stage save not admitted. Pending proposals still require Work Plan approval. |
| S1-C history / grouped costs / S1-D repeated cycles and completion | PASS | PASS inherited | PASS | PASS earlier lineage; exact candidate NOT RUN | NOT RUN | [S1-D T09 review](../S1-D-Implementation-09/INDEPENDENT-REVIEW-20261031.md): ≥3 cycles, Year25 completion/Year26 continuation, 15 disk checkpoints; cash margin risk retained. |
| Legacy40 / frozen Reference / schema / valuable files | PASS | PASS inherited | PASS | PARTIAL; hashes pass, runtime NOT RUN here | NOT RUN | v24 unchanged; six frozen sources and 27 licensed files audited in both lanes. Runtime protection is not inferred from hashes. |
| #380 UI residual / occlusion matrix | PASS source | PARTIAL | PASS | PARTIAL, see receipts | NOT RUN | Current new captures and exact input distinctions below; #379 Annual/Crop successes reused. |
| Exact-candidate Tier-3 complete-loop | PARTIAL | NOT RUN | PASS source selection | NOT RUN | NOT RUN | Tier-2 prerequisite matrix incomplete; no honest full-loop receipt exists from this run. |

#287 strict Workbench gate remains FAIL/NOT FULLY QUALIFIED despite historical 107/107 in-Editor. #309 CTA checker historical pin/guard failures remain debt, not modified or counted PASS. #281 is open feedback, with implemented map/CTA descendants in candidate; no evidence here authorises a new topology. Independent/G4 readiness gate: **UNSATISFIED GATE / MANAGER DECISION NEEDED**; earlier scoped approvals do not self-approve this candidate.

## 3. AUTHOR source tests and integrated qualification

Fresh runtime compilation: PASS, zero errors/warnings using real installed Unity managed references and the existing #379 project manifest repointed only to this source path. Command: `dotnet build /media/jer/Files/CCF/Verification/CCF-381-Readiness-20261111/compile/check.csproj -v q`. This is syntax/type evidence, not Editor behavior.

Fresh exact presentation sink: PASS157; five compiled negative controls killed by their named assertions. Fresh repair consumers: PASS locked note + eight hint states, 12 source contracts, 10 negative controls killed (seven source contracts, three executed consumers). Sink emitted only unused-field warnings. Commands:

```bash
python3 -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out /media/jer/Files/CCF/Verification/CCF-381-Readiness-20261111/presentation
python3 -B Tools/Verification/ScenarioOneUiPresentation/repair_consumers.py --src "$PWD" --out /media/jer/Files/CCF/Verification/CCF-381-Readiness-20261111/consumers
```

No historical verifier edited, assertion removed, threshold relaxed or anchor rebaselined. #379 tagged/Money/inspection checks and earlier completed qualification are inherited, not rerun. Tier-3 NOT RUN: missing authentic MFM residual fixture and incomplete Tier-2/legacy runtime prerequisites. Do not use a source compile or session-exit marker as a complete-loop PASS.

## 4. New Unity receipts and custody

User explicitly released #361 Editor ownership during this run. Fresh registered clean UIQual376 lane, UUID `18D6-896E`, `/dev/sdd2`, exFAT; free 29,588,193,280 bytes before launch (>15GiB). Global lock free/no Editor before acquire. Lease `3ae1523a03284a7b803b4a64907ba266`, acquired 2026-11-11 00:37:41 UTC. Source writer released before launch; no production changes.

Unity 6000.6.0f1 actual process proved `Application.persistentDataPath`, private `ForestSaveController.SavePath` (F5/F9) and a PlayerPrefs sentinel file under `.../CCF-381-Readiness-20261111/run/config/unity3d/DefaultCompany/CCF` before test interaction. Personal U3/save/prefs never used for test. Copied harness inspected before use; existing kill/reaper branch disabled. No Unity process killed. License resolved successfully; no exit198. Known SearchDatabase startup exception and UI Toolkit render-chain diagnostics retained in raw log, not silently discounted as a diagnostic-free run.

| New AUTHOR Unity check | Result / direct observation |
|---|---|
| Actual process save/Prefs routing | PASS before interaction; [paths](evidence-381/paths.txt) |
| Intro/forest render | PASS contentful forest/start screen; capture pixel guard plus author visual inspection |
| Objective1 disabled notes at720/1080 | PASS visual/disabled hierarchy; [720](evidence-381/objective1-lower-native-scroll-1280x720.png), [1080](evidence-381/objective1-1080-1920x1080.png) |
| Objective1 actual pointer restrictions | PARTIAL: native click/scroll with unchanged year0/0 orders; Planting panel.Pick identifies disabled element. Full per-card pointer/focus matrix NOT RUN. No callback click represented as native pointer. |
| Disposable native F5/F9 | PASS fresh Year0 round trip, disk bytes unchanged by load; real Year2 archived #369 fixture then loaded at Objective3 |
| Native held Shift+V | PASS after tutorial dismissal: Page2→Page1; first overlay-blocked attempt inconclusive, retained |
| One rank/focus/occlusion pose | PASS scoped rank1 selected P0709, visible forest rank and panel row, no overlap among normal HUD/panel in [720](evidence-381/rank1-focus-720-1280x720.png) and [1080](evidence-381/rank1-focus-1080-1920x1080.png); broad cursor/focus matrix NOT RUN |
| Native MFM C/N | NOT RUN: authentic late-stage guided-sample fixture not found/admitted in bounded discovery (known #379/#369 and prior MFM12 qualification roots). Year2 Objective3 is insufficient. No fabricated cache, stage, cash or forest facts. |
| Legacy40 / Reference runtime residual | NOT RUN; inherited feature receipts and current source integrity remain distinct |
| Tier2 overall | PARTIAL — unfilled native/compatibility cases remain |
| Tier3 exact complete-loop | NOT RUN — Tier2 prerequisites incomplete; no simulated PASS |

Editor normal exit0, 362.9s, no reaper; [driver receipt](evidence-381/result.json) PASS means controlled session exit ONLY. The command client times out waiting for exit acknowledgement because the Editor exits before writing it; normal exit is established by runner/Editor receipt. No product failure inferred from that command-client timeout.

All captured new PNGs have rendered content; Objective1 notes and final rank1 captures were directly visually inspected, not judged solely by numeric tests. Initial inspection-help screenshots are retained as inconclusive input attempts, not final fit evidence.

Final custody: 145 protected manifest entries before/after hashes identical; U3 and qualification-lane 27 licensed + six protected paths match #379 baseline, no rehash/rebaseline. Primary, U3, original registered Workbench and Legacy status plus staged/unstaged binary diffs compare byte-exact before/after. Primary's two unrelated edits preserved. Six generated qualification-lane settings/material changes archived as a patch and restored to exact HEAD after Editor exit; initially clean lane is clean again. No auto-start script/meta remains. Copied license removed. Source writer released; lease released normally with no Editor PIDs. Release first omitted required task-id and was rejected without state change; corrected matching owner/task/id release succeeded.

Additional read-only inventory of CCF-WorkbenchRetirement-T3 differs from the prior #379 original Workbench baseline; it is a different checkout, not overwritten or treated as the original. The protected original is CCF-WorkbenchTier2-Registered-20261106, whose pre-run patch also matches #379 byte-exact. No external state repair performed.

## 5. Verdict, safe handback and single next action

**BLOCKED**, not READY, not user accepted, not external-demo ready. No source-visible production repair was justified by this audit. Missing authentic late-stage MFM residual evidence and exact-candidate integrated Tier-3 are qualification blockers; no simulated teaching-stage PASS offered.

Single next action: **Manager #200 admit an authenticated guided-sample-complete continuation (with provenance), then release one bounded residual/complete-loop qualification on this unchanged candidate after fresh safety preflight.** If historical evidence is to substitute for a mandatory independent gate, Manager must explicitly decide; this worker has not waived it.

Do not launch the Files worktree or upgrade the personal U3 lane. After Manager release, safe Editor handback uses the registered UIQual376 lane at exact `2aaacad68ed186dbe14531f31ce5bb3a87811441`, a new exclusive lease and fresh proven disposable config. First observe recognizable intro/forest; stop on black. Walk/aim, E inspect, X Fell/C Crop when eligible, M Stand Map, O Objectives, Tab Work Plan; deliberate approval then separate Advance one year; Annual Review latest ACK is separate from Back and historical browsing; V/held Shift+V inspection paging. F5/F9 only after actual isolated destinations are proved. Expected: visible teaching restrictions, no premature planting/pruning/nursery action, exact-year review, approved work executed once and persistent disposable world changes. No Windows Player, main merge, personal-save migration or acceptance authorised.

## 6. Publication and ownership

Candidate source writer and Unity lease are RELEASED. Report/evidence writer released after commit/push. #381 is left **[BLOCKED]**, open, with exact handoff link; #200 remains its existing **[RUNNING]** Manager issue, notified with that same link. No change to #361 acceptance state. No PR/main merge or Windows build. GitHub comment readback verifies notification publication; downstream dashboard ingestion/audit/readback is **UNVERIFIED**, an outstanding operational item owned by Manager (active-tasks is Manager-written; no generated dashboard files manually edited).

Durable full local evidence: `/media/jer/Files/CCF/Verification/CCF-381-Readiness-20261111/`. Selected raw logs, custody manifests and new screenshots accompany this report under `evidence-381/`. Earlier unchanged #379 PNGs remain in their original report/evidence tree; they are inherited, not new #381 captures.
