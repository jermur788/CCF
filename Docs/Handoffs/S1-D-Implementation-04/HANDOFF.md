# S1-D T-04 — Guide Monotonicity — Offline Handoff

**Claim:** #124  
**Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`  
**Status:** **OFFLINE VERIFIED; PRODUCTION UNCHANGED; UNITY GATE PENDING; INDEPENDENT REVIEW PENDING**  
**Provider/model:** `opencode-go/deepseek-v4.1-flash` (T3 Code / OpenCode harness)  
**Branch/worktree:** `feature/s1-d-t04-guide-monotonicity-v2` at `/media/jer/Videos/CCF-S1D-T04`  
**Exact start:** `bccd2b90b3eafc3728a9f29ac421b2719d042d72` (integrated #143 lineage)  
**Production end:** `bccd2b90b3eafc3728a9f29ac421b2719d042d72` — **no production change**  
**Offline verification candidate:** `c838aca0b405490c5792c4150868b319c02529c8`  
**Docs successor:** the commit containing this `HANDOFF.md` (production/verification identical to `c838aca0`)  
**Unity:** not run in this phase, per #91 serialized-lane instruction.  
**Integration:** not merged.

## Bottom line

No production change is genuinely needed. The exact integrated #143 candidate already contains the T-04
monotone floor and its shared durable read:

- `ScenarioOneManager.FirstGuidedRackReviewComplete` is the one-way v24 historical latch.
- `LearningObjectivesView.TeachingFacts()` bridges it into `ScenarioOneTeachingSequence.Facts.FirstGuidedRackReviewComplete`.
- `ScenarioOneTeachingSequence.Current(f)` is already
  `f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f)`.
- `CurrentUnlatched(f)` preserves the accepted pre-amendment guard order; `RecruitmentAndLater(f)` drives
  `recruitment`, `establishment` and `transformation` from later current facts.
- The only write hook is `LearningObjectivesView.DecideRackInspection` → `TryRecordFirstGuidedRackReview`
  after a real `ScenarioOneRackReview.Decide`; display refreshes and loads cannot write it.

Adding another T-04-only history store or second floor would duplicate or alter accepted behaviour. This
task therefore adds focused offline verification only. No save/schema/validator/hash/reference file was
touched; no T-04-only historical meaning was created; no `[J]/[K]` or `PlayerPrefs` authority was added;
no guide index was persisted.

## Shared read and lesson-floor mapping

| Layer | Exact API | T-04 use |
|---|---|---|
| Durable read | `ScenarioOneManager.FirstGuidedRackReviewComplete` | sole historical completion fact |
| Facts bridge | `LearningObjectivesView.TeachingFacts()` | sets `Facts.FirstGuidedRackReviewComplete` |
| Pre-latch sequence | `ScenarioOneTeachingSequence.CurrentUnlatched(Facts)` | accepted lessons 1–5 / response |
| Monotone floor | `ScenarioOneTeachingSequence.Current(Facts)` | latch true → `RecruitmentAndLater` (index 5/6/7) |
| Later facts | `RecruitmentAndLater` | beech/oak + planting year + establishment review drive lessons 6–8 |
| Write path | `TryRecordFirstGuidedRackReview` / `CrossesFirstRackReview` | owned by #143; T-04 writes nothing |

Pre-latch behavior is unchanged: `ReleaseReviewed == true` with `RackReviewComplete == false` stays at
`response` (index 4), and a missing/old latch stays false until the authoritative session review is newly
completed. Post-latch behavior never returns below `recruitment` (index 5), regardless of Crop Tree count
increase/decrease, Crop Tree loss, later in-footprint felling, executed-share drift or save/load/rebuild.

## Required matrix and results

New harness: `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingSequenceMonotonicityOfflineCheck.cs`
with runner `run_teaching_sequence_offline.py`.

| Packet requirement | Evidence | Result |
|---|---|---|
| Latch false + incomplete session review | `Facts` default false; response stays index 4 | PASS |
| `ReleaseReviewed` true but `RackReviewComplete` false | `Current` and `CurrentUnlatched` both index 4 | PASS |
| Authoritative `response → recruitment` transition | `CrossesFirstRackReview` accepts only the real boundary; manager records true once; repeats refuse | PASS |
| Post-latch Crop Tree count increase | index stays 5 while unlatched would be 1 | PASS |
| Post-latch Crop Tree count decrease | index stays 5 while unlatched would be 1 | PASS |
| Post-latch Crop Tree loss | index stays 5 while unlatched would be 1/0 | PASS |
| Post-latch later in-footprint felling | index stays 5 while unlatched would be 2 | PASS |
| Post-latch executed-share drift | index stays 5 while unlatched would be 2 | PASS |
| Post-latch save/load/rebuild at lessons 6, 7, 8 | latch persists; session review resets; rebuilt guide stays at 5/6/7 respectively | PASS |
| Missing-latch save conservative false | new `Facts` false; save restore requires `saveVersion >= 24`; no reconstruction | PASS |
| Later facts still drive lessons 6–8 | beech/oak → 5, planting year → 6, reviewed establishment → 7; drift does not pull 7 back | PASS |
| Repeated reads never write | 512 repeated reads identical and fact snapshot unchanged; transition predicate read-only | PASS |
| Eight lesson IDs unchanged | exact ordered `access, future-trees, release, habitat, response, recruitment, establishment, transformation`; no duplicates | PASS |
| No `PlayerPrefs` / `[J]/[K]` durable authority | source audit of sequence/view; latch is the only durable read | PASS |
| No persisted guide index | `ScenarioOneSaveData` has exactly one teaching field (`firstGuidedRackReviewComplete`) and no guide/lesson index | PASS |
| Pre-latch sequence exactness | 16,384 boolean/crop rows equal the exact accepted `c8cfae8c` sequence oracle | PASS |
| Check can fail | 5/5 deliberate sequence mutations caught | PASS |

## Exact commands and results

Durable evidence root:
`/media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/`

```text
python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_teaching_sequence_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/offline
TEACHING_SEQUENCE_MONOTONICITY_OFFLINE_PASS checks=62 NOT_UNITY_VERIFIED
T04_PRODUCTION_UNCHANGED base=bccd2b90b3eafc3728a9f29ac421b2719d042d72

python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_teaching_sequence_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/offline-mutations \
  --mutations
TEACHING_SEQUENCE_MONOTONICITY_OFFLINE_PASS checks=62 NOT_UNITY_VERIFIED
TEACHING_SEQUENCE_MUTATIONS_PASS caught=5 survived=0

python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_teaching_cap_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/t03-cap \
  --base bccd2b90b3eafc3728a9f29ac421b2719d042d72
TEACHING_CAP_OFFLINE_PASS checks=48 NOT_UNITY_VERIFIED
TEACHING_CAP_PROTECTED_DIFF_PASS base=bccd2b90b3eafc3728a9f29ac421b2719d042d72 paths=16

python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/t02-cycle
CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED

python3 Tools/Verification/TeachingPersistence/run_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/h143
TEACHING_CAP_OFFLINE_PASS checks=48 NOT_UNITY_VERIFIED
TEACHING_PERSISTENCE_OFFLINE_PASS checks=36
PERSISTENCE_V23_ALL_HASH_BASELINE_PASS 80 fixture rows
PERSISTENCE_PROTECTED_SOURCES_UNCHANGED c8cfae8c4025cec6e960a050edf4bfde2a981de5
# nested MFM-06 suite: MFM06_SAVE_OFFLINE_PASS checks=172

python3 Tools/Verification/S1CManagementHistory/run_offline_check.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/s1c-history
S1C_HISTORY_QUERY_PASS checks=55

python3 Tools/Verification/S1CManagementHistory/run_t03_offline_check.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/s1c-t03
S1C_T03_DRILLDOWN_PASS checks=72

python3 Tools/Verification/S1CManagementHistory/run_t04_offline_check.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/s1c-t04
S1C_T04_PROVENANCE_PASS checks=56

python3 Tools/Verification/S1CManagementHistory/run_t06_offline_check.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/s1c-t06
S1C_T06_GUIDANCE_PASS checks=53 failures=0

python3 Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/n3
N3_OFFLINE_PASS checks=88 NOT_UNITY_VERIFIED
# runner asserts the accepted normalized completion-rules SHA-256 8166775d...; no rebaseline

python3 Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py \
  --out /media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/nonui
NONUI_KEYBOARD_PASS checks=46
NONUI_RACKBAY_PASS checks=87
NONUI_RACKSAMPLE_PASS checks=75
NONUI_APPROACH_PASS checks=49
```

`T04_PRODUCTION_UNCHANGED` is also independently confirmed by:

```text
git diff --name-only bccd2b90b3eafc3728a9f29ac421b2719d042d72 -- Assets/ForestPrototype
# no output
```

## Regressions and protected surfaces

- T-03 cap decision table remains accepted, including the post-latch advisory path from the #143 suite.
- T-02 cycle-state derivation passes 93 checks.
- #143 latch/save/hash suite passes; v23/v24 identity fixtures are byte-identical, 80 rows, no rebaseline.
- S1-C history, provenance, drill-down and guidance checks pass.
- N3 normalized completion-rule source hash is unchanged.
- `Assets/ForestPrototype` is byte-identical to the exact start; no save/schema/validator/hash/reference,
  completion/objective, economy/ecology, Rack V2, MFM, S1-C storage, scene, prefab, package or
  ProjectSettings change.

## Unity-readiness checklist (not yet run)

Pin the Unity lane to `c838aca0b405490c5792c4150868b319c02529c8` (production identical to the start and to
the docs successor) and run under the normal machine-wide Unity lock:

1. compile/import;
2. prove pre-latch `response` behavior is unchanged;
3. prove `ReleaseReviewed` without complete rack review does not skip `response`;
4. complete the actual session rack review and prove progression reaches `recruitment`;
5. save/load at lessons 6–8 and prove no regression behind the historical boundary;
6. exercise Crop Tree loss/count drift, later felling and executed-share drift after completion;
7. prove repeated view refreshes and calls do not mutate persistence or the latch;
8. rerun TeachingCopy and MenuTutorial (Unity-only; not run offline) plus T-03 cap/rack-lesson, T-02,
   #143 latch/save/hash, S1-C and N3 Unity gates.

## Evidence gap / unresolved questions

- **TeachingCopy and MenuTutorial were not run**: both are Unity Editor harnesses and this phase was
  explicitly offline-only. The new harness does statically assert the changed response copy (J/K choices
  are not stored; completion is remembered after loading), but that is not the rendered copy gate.
- Real `JsonUtility`/controller disk round-trip and live view-refresh persistence remain for the Unity gate;
  the offline save/DTO checks are serializer stand-ins plus the exact integrated #143 source audit.
- Independent review is **not yet performed**. Manager must assign a cross-provider reviewer on the exact
  candidate before acceptance.
- #124 is not integrated or Manager-accepted; no Windows build was produced.

## Next action

Manager review of this handoff and the offline matrix, then schedule the separately locked Unity
qualification on `c838aca0b405490c5792c4150868b319c02529c8` (or its docs successor, production-identical).
After Unity passes, assign independent review on the exact candidate. Do not merge until those gates pass.

`S1-D T-04 GUIDE MONOTONICITY QUALIFIED — READY FOR MANAGER REVIEW`
