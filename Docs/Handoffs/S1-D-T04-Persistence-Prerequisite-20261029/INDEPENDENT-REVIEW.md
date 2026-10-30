# S1-D T-04P — Independent Implementation Review

**Claim:** #161 (read-only independent review of #143)  
**Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`  
**Reviewer:** DeepSeek V4.1 Flash (browser/read-only lane #162)  
**Implementation provider reviewed:** OpenAI gpt-6.1-sol high — cross-provider independent  
**Exact production candidate:** `b9bf11aaf8f6fd7ea43e33e39258560408b88b12`  
**Docs/evidence successor:** `d8168926` (HANDOFF only; no production delta from `b9bf11aa`)  
**Branch:** `feature/s1-d-t04-teaching-persistence-v1`  
**Base:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`  
**Date:** 2026-10-30  
**Verdict:** **APPROVED**

## Scope

Mandatory independent implementation review before Manager/delegated integration. Read-only: no production edit, no save/schema/hash write, no Unity acquisition. Repairs would return to #143 or a new owned repair claim.

## Evidence inspected

- Production diff `c8cfae8c..b9bf11aa` under `Assets/ForestPrototype` (18 files total, 12 verification files).
- Final worker handoff `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md` at `d8168926`.
- Unity evidence root `/media/jer/Files/CCF/Verification/S1D-T04P-Persistence-20261029/unity-r2/`, all eight gates pinned to `b9bf11aa` in each `command.json`.
- Published offline evidence under the same root.
- Independently reran `Tools/Verification/TeachingPersistence/run_offline.py` from the exact candidate worktree into `/media/jer/Files/CCF/Verification/S1D-T04P-Review-20261030/offline`.
- Inspected the live write path, shared read, save capture/restore, and every identity/hash projection.

## Independent re-execution

Command (from `/media/jer/Videos/CCF-S1D-T04-Persistence`):

`python3 Tools/Verification/TeachingPersistence/run_offline.py --out /media/jer/Files/CCF/Verification/S1D-T04P-Review-20261030/offline`

Observed terminal result:

- `TEACHING_PERSISTENCE_OFFLINE_PASS checks=36`
- accepted T-03 table `TEACHING_CAP_OFFLINE_PASS checks=48`
- `PERSISTENCE_V23_ALL_HASH_BASELINE_PASS 80 fixture rows` (v23 and v24 hash outputs byte-identical, including non-default prescription state and older headers)
- `PERSISTENCE_PROTECTED_SOURCES_UNCHANGED c8cfae8c...`
- exact old-sequence oracle over 16,384 pre-latch rows passed.

This is an independent execution of the same published harness, not merely a reading of the worker log. It remains an offline serializer stand-in; the Unity JsonUtility path is covered separately below.

## Unity/runtime evidence

Every one of the eight evidence directories pins `b9bf11aa` in `command.json`. Observed markers:

| Gate | Marker |
|---|---|
| compile | `MFM06_COMPILE_PASS` saveVersion=24 |
| teaching-latch | `TEACHING_LATCH_UNITY_PASS checks=60` |
| save | `MFM06_UNITY_PASS checks=159` |
| hash-dump | `MFM06_HASHDUMP_PASS lines=18` saveVersion=24 |
| hardening | `SAVE_HARDENING_VERIFY_PASS` |
| reference | `REFERENCE_FUTURE_V1_PASS` |
| approval | `MFM07_UNITY_PASS checks=106` |
| n3 | `N3_U4_VERIFY_PASS` |

Independent hash comparison: `unity-r2/04-hash-dump/hash-dump.txt` is byte-identical to the accepted MFM-07 base dump `/media/jer/Files/CCF/Verification/MFM-07-Integration-20261029/candidate/04-hash-dump/hash-dump.txt` (18 lines each). No rebaseline.

## Findings against the required contract

### 1. Exactly one persisted fact — PASS

Production save delta is `ForestSaveData.CurrentVersion` 23→24 plus one `bool firstGuidedRackReviewComplete` on `ScenarioOneSaveData`. No J/K decision, inspected tree/stand ID, transcript, cycle key/counter/list, guide index or readiness state is serialized. Capture writes only the single value; restore reads it only when `saveVersion >= 24`; a new scenario resets it false. A missing/old field is false, never backfilled.

### 2. Genuine write boundary — PASS

`LearningObjectivesView.DecideRackInspection` calls `TryRecordFirstGuidedRackReview(before, after)` only after a real `ScenarioOneRackReview.Decide` returned `changed`. The manager method refuses when already true or in Reference preview, and `CrossesFirstRackReview` requires:

- non-null rack bay before and after;
- `!before.RackReviewComplete` → `after.RackReviewComplete`;
- `after.ReleaseReviewed` true;
- exact pre-amendment sequence index 4 before and 5 after, computed by `CurrentUnlatched`.

`ScenarioOneRackReview.Complete` requires all future trees inspected/decided and exactly six stand portions; portions are added only by `Decide`. Therefore the final action that can complete the review is a successful `Decide` call, i.e. the latch write path. Display refresh, `TeachingFacts()`, Annual Review, load/restore, planting/felling and later world state cannot write it. `CurrentUnlatched` is an independent preservation of the old guard order.

### 3. One shared historical read — PASS

`LearningObjectivesView.TeachingFacts()` supplies `FirstGuidedRackReviewComplete` from `ScenarioOneManager.FirstGuidedRackReviewComplete`; `ScenarioOneTeachingSequence.Current` and both `ScenarioOneTeachingCap.Classify` call sites consume that same fact. `SampleHistoricallyComplete` is retained only as the documented pre-latch/current-world T-03 safeguard and is not treated as durable proof. Default `Classify(..., false)` preserves any uncalled legacy path.

### 4. Save round-trip and old saves — PASS

Full save JSON carries false/true; v23/older restores as false; v24 restores the stored value. The 60-check Unity latch gate and 159-check save gate cover real controller disk round-trips, true/false, reload idempotence, Crop Tree loss/count drift, release-accounting drift, later in-footprint felling and old missing-field saves.

### 5. Every identity/hash projection excludes the latch — PASS

`HashLayout` strips `"firstGuidedRackReviewComplete":true|false,` before every branch, including the non-default prescription-state branch; that branch additionally keeps its pre-amendment v23 identity header for v24 inputs. Default-prescription worlds keep the existing v20/v21 layout policy; v15–v18 paths receive the strip through `WithoutLaterFields`. The new field is declared first in `ScenarioOneSaveData`, so it is always followed by a comma and the regex cannot leave dangling JSON in the real serializer output. The 80-row v23/v24 byte comparison, the 18-line Unity hash dump equality, and the reference gate's deterministic continuation cover the accepted anchors.

### 6. No unrelated production scope — PASS

Diff outside `Tools/Verification` is limited to the save DTO/version, manager latch capture/restore/read, reference-archive stripping, teaching cap/sequence read, and the real inspection write hook, plus copy. No completion rules, objectives, ecology, economy, rack geometry, scenes, packages or ProjectSettings changes; protected source check passed. Hash/reference/protected anchors unchanged.

## Residual notes (non-blocking)

- The verification harness uses synthetic queued New Input System decisions, not a native Windows input test. That matches the accepted gate and the no-Windows constraint.
- The generic cloud type-check remains unable to compile mutually exclusive stubs; the focused installed-Unity reference compile and the eight Editor gates supersede it for this change.
- The latch is written to the in-memory save model at completion; durability follows the game's normal save path. This matches the task contract (persist the fact), not a new autosave requirement.
- #124's later curriculum replay remains explicitly out of scope and unqualified.

## Verdict

**APPROVED** for Manager/delegated integration of exact production candidate `b9bf11aaf8f6fd7ea43e33e39258560408b88b12`, with docs/evidence successor `d8168926`.

No repair is required before integration. Integration must preserve both histories, check the current integration head, and re-run any gate required by the integration policy if the merge introduces unexpected overlap. This approval is implementation review only; it is not Unity re-qualification of the merged tree, Manager acceptance, or a Windows build.
