# S1-D T-04 Guide Monotonicity — Independent Review (#124, parent #91)

**Reviewer:** Claude (Sonnet 5.5) — cross-provider to the implementer (DeepSeek V4.1 Flash)
**Date:** 2026-10-30
**Mode:** read-only; no Unity run; no production/save/hash edits; no merge.

## Verdict: **APPROVED** (offline/static scope only)

Unity/runtime qualification is a **separate pending gate** and is NOT accepted or implied by this review.

## Exact candidate

| Item | SHA |
|---|---|
| Branch | `feature/s1-d-t04-guide-monotonicity-v2` (origin tip = docs tip) |
| Start / production END | `bccd2b90b3eafc3728a9f29ac421b2719d042d72` (identical, no production change) |
| Verification candidate | `c838aca0b405490c5792c4150868b319c02529c8` |
| Docs tip | `99b7d4b47c7f411b2c528311f446aed1b13ead9d` |

`git diff --name-status bccd2b90 99b7d4b4` = exactly three **added** files:
`Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md`,
`Tools/Verification/ScenarioOneRepeatedInterventions/TeachingSequenceMonotonicityOfflineCheck.cs`,
`.../run_teaching_sequence_offline.py`.
No Assets, save/schema/hash/reference, scene, prefab, package or ProjectSettings file changed.

## Evidence inspected

- Packet (`616df930:Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md`; not on the candidate branch), spec `specs/scenario-one-repeated-interventions-v1/` FR-B1..B3, RD-7, T-04, AC-3; #143 `INTEGRATION-HANDOFF.md`.
- Production source at `bccd2b90`: `UI/ScenarioOneTeachingSequence.cs`, `UI/LearningObjectivesView.Sequence.cs`, `ScenarioOne/ScenarioOneManager.cs` (latch, record, init, capture, restore), `ForestSaveData.cs`, `ScenarioReferenceArchive.cs`.
- Candidate harness and runner (full read), HANDOFF.
- Durable logs in `/media/jer/Files/CCF/Verification/S1D-T04-GuideMonotonicity-20261030/`.
- Independent rerun (fresh local clone detached at `99b7d4b4`, output in this folder: `offline/`, `offline-mutations/`).

## Findings against the review questions

1. **No production change genuinely required — confirmed.**
   `Current(f) => f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f)`; `RecruitmentAndLater` returns only 5/6/7 from beech/oak, planting year, establishment review. `TeachingFacts()` bridges `m.FirstGuidedRackReviewComplete`. Only write site is `DecideRackInspection` → `TryRecordFirstGuidedRackReview` after real `rackReview.Decide`; guarded by `Current(f) != 4`, `CrossesFirstRackReview` (needs `CurrentUnlatched` 4→5), `referencePreviewActive`, and already-true refusal. `TeachingFacts` itself never writes. This is exactly the "simplest acceptable shape" in the packet; adding anything would duplicate #143.
2. **Latched guide cannot fall behind `recruitment`** for Crop Tree count up/down/loss, later felling, executed-share drift, rebuild: by construction (the latch branch never calls the guards). Harness asserts each case against `CurrentUnlatched` being strictly lower, so the floor is demonstrably what saves it.
3. **Pre-latch exact:** 16,384 rows compared to an independently pasted accepted `c8cfae8c` oracle (`OldAccepted`); `ReleaseReviewed`-true/`RackReviewComplete`-false stays 4 in both `Current` and `CurrentUnlatched`. Oracle is a verbatim copy, not a call into production, so not tautological.
4. **Later facts drive 6–8; IDs unchanged; no guide index; no PlayerPrefs/J-K authority; no T-04 history store:** asserted by behaviour and by source audit. The only `PlayerPrefs` in the UI is the pre-existing practice-step store in `LearningObjectivesView.cs`, which does not feed `Current`. Save DTO has exactly one teaching field; restore is `saveVersion >= 24 && data.…`, so old saves stay false (no reconstruction).
5. **Offline matrix not tautological:** I reran it — `TEACHING_SEQUENCE_MONOTONICITY_OFFLINE_PASS checks=62`, `T04_PRODUCTION_UNCHANGED`; mutations `caught=5 survived=0` (floor removed, drift-conditional floor, ID rename, session-review-as-latch, pre-latch floor via Max) each failing the intended check.
6. **Logs:** every claimed token (62-check pass, production unchanged, 5/5 mutations, T-03 cap 48 + protected diff 16 paths, T-02 93, #143 36 + 80 hash rows + protected sources + MFM06 172, S1-C 55/72/56/53, N3 88, non-UI 46/87/75/49) is present in the durable logs; `offline.log` contains no FAIL.
7. **Diff scope:** harness/runner/docs only; the runner also refuses unowned production changes.

## Non-blocking observations (no change requested)

- **Legacy (Null-Bay / 40 m) worlds never latch** (`CrossesFirstRackReview` requires `Bay != null` on both sides), so they keep prior behaviour. This matches FR-B1 ("keep existing behaviour and report") but the HANDOFF does not state it; recommend the manager note it.
- Lessons 6–8 remain fact-driven and can move within 6–8 (e.g., a later replant raising `PlantingYear` returns 7→6). This is per the packet ("later facts continue to drive normally") and the floor still holds at `recruitment`; flagging for spec-intent awareness only.
- Source-wiring checks are text/regex audits and the latch manager is a generated slice of `ScenarioOneManager.cs`; mutations cover only `ScenarioOneTeachingSequence.cs`, not the view bridge. Real wiring is confirmed by reading source, but a live check belongs to the Unity gate.
- TASK-PACKET is not on the candidate branch (lives at `616df930`); harmless.

## Not accepted / pending

- Unity compile/import, TeachingCopy, MenuTutorial, live save-at-lessons-6–8 reload, real `JsonUtility` round-trips, view-refresh no-write behaviour: **unverified here; separate Unity gate pinned to `c838aca0` (production-identical to docs tip).**
- Manager acceptance and integration: not claimed. No Windows build.
