# MFM-05 — Spacing-only Crop Tree candidate system

**CROP TREE CANDIDATE SYSTEM QUALIFIED** — static/pure-code scope only.

**State:** IMPLEMENTED ON TASK BRANCH; ready for Manager review, not integrated or Unity-verified. Claim #100 under #91 remains open. No Unity lock acquired and no Windows build.

## Identity and authority

- Actual worker: OpenAI `gpt-6.1-sol`, medium reasoning, Codex harness in T3 Code. No delegated agents. This supersedes the claim's original rate-limited Claude dispatch identity for this implementation.
- Worktree: `/media/jer/Videos/CCF-MFM05`.
- Branch: `feature/mfm-05-crop-tree-candidates-v1`.
- Exact base: `68ad3f3277c367c860f5615dfaa113dcded3a573` (umbrella integration + MFM-04).
- **Exact production + verification candidate:** `4b54af047a8cdbfc369cc477e9b22fe8c60a9a93`.
- This handoff is added in a subsequent documentation-only commit; the candidate above identifies all tested production/check bytes. Resolve the publication commit from the branch/claim comment.
- Context commit at base: `63e95ba2175f7e239a278c616681b286719821f8`.
- Authority: latest direct MFM-05 dispatch, D-065, approved `specs/multiscale-forestry-management-v1/` (especially spec §8, plan §8, checklist F/O6), MFM-04 handoff, MFM-03 reviewed calibration `60792dc` / `73356e3`, MFM-02 geometry interface.
- Preflight: clean branch at the exact requested base, no unrelated changes. Fetched origin and read live `active-tasks.md` on main and the umbrella orchestration branch (which carries the live G2 order/MFM-05 ownership). Production ownership is confined to this candidate class. No shared save, manager, map, curriculum or history writer is entered.

## Exact change

`git diff 68ad3f3277c367c860f5615dfaa113dcded3a573 4b54af047a8cdbfc369cc477e9b22fe8c60a9a93`:

| File | Change |
| --- | --- |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneCropTreeCandidates.cs` | 68 additions, 6 deletions: single inspection-target API; explicit stable area/membership ordering; source/sample exclusions; reject infinite spacing. |
| `Tools/Verification/MfmCropTreeCandidates/OfflineLogic/CropTreeCandidatesOfflineCheck.cs` | 140 lines: 95 focused checks against production source. |
| `Tools/Verification/MfmCropTreeCandidates/run_candidates_offline.py` | 59 lines: external .NET build, warnings as errors, two-process repeat check; reuses MFM-04 source manifest. |
| `Tools/Verification/MfmCropTreeCandidates/README.md` | 16 lines: commands, semantics and limits. |

Total: 4 files, 283 insertions, 6 deletions. This publication adds only this handoff afterward. Existing candidate `.meta`/GUID untouched; no new Assets file, serialized asset, scene, prefab, save/schema, package, ProjectSettings, UI or Work Plan file changed.

## Candidate semantics

The MFM-04 `Suggest(coverage, currentStems, sampleCropTreeIds, confirmedIds, rejectedIds, calibration)` interface is preserved. There is no second candidate algorithm, competition implementation or calibration.

- Spacing identifies **where to inspect**. Every output carries the sole reason `SpacingOnlyInspectToJudge`. Tree quality remains the player's judgement, grounded in later 3D inspection.
- Use `ScenarioOnePrescriptionCalibration.V1`: `MFM03-CAL-1`, rule `MFM-V1-RULE-1`, **16 m** candidate spacing. This is one-fixture GAMEPLAY CALIBRATION, not a forestry standard. Separate 8 m teaching spacing and 8 m Hegyi reach are untouched.
- Covered, non-source areas only. Living Sitka adult records only; exclude missing/dead/felled records, Fell-marked stems, broadleaves, sample Crop Tree IDs and existing manual Crop Trees. `Stem` is the adult-domain adapter; later adapters must not supply juveniles as adults. Areas excluded by coverage (including management conflicts) stay excluded.
- Explicit order: numeric rack ID, Low/Centre/High, ordinal area ID fallback. Each area's unique eligible IDs are first sorted ordinally to make centroid summation stable. Within an area: distance to mean eligible position, x, z, ordinal tree ID. No diameter, height, competition, form, vigour, dominance or hidden quality input affects this ordering.
- Every accepted position is at least the configured spacing from earlier batch positions and other living Crop Trees/sample Crop Trees. Manual designation wins as a blocker and is never newly suggested. Exactly the boundary distance is allowed.
- Reviewed conservative **batch** default retained: confirming/rejecting labels the same batch slot; rejected positions are not replaced. Previously confirmed candidates remain visible as `Decision.Confirmed` history/blockers, **not actionable candidates**. This exception preserves MFM-04's batch contract; manual/already-designated trees outside that recorded batch are excluded.
- Decisions are supplied by the caller and echoed; this class neither creates nor authorizes them. It has no inspection evidence, decision store, confirmation, bulk confirmation, approval or mutation API. Conflicting confirm/reject sets fail; missing/nonliving/unavailable decision IDs are reported through sorted `StaleDecisions`.
- Batch stability applies with unchanged authoritative geometry/coverage/current tree state except candidate decisions. After growth, death, felling, manual designation or coverage changes, regenerate and reconcile. This is not a frozen persisted candidate list.

## Selection/navigation interface

- `TryGetInspectionTarget(Result result, string treeId, out InspectionTarget target)` returns exactly one **Open** candidate, or false.
- `TryMoveInspectionTarget(Result result, string currentTreeId, Navigation direction, out InspectionTarget target)` supports `Next`/`Previous`, skips confirmed/rejected entries, stops at queue ends without wrapping. Empty current ID starts at first/last open entry. A decided entry can anchor navigation. Unknown current IDs, invalid directions, invalid results and empty/all-decided queues fail safely.
- Immutable `InspectionTarget`: `TreeId`, `AreaId`, `X`, `Z`, `Order`, sole `Reason`. Useful for map focus/3D navigation. No target selection changes a mark or constitutes evidence of 3D inspection. No selection state is stored.
- Caller must regenerate from current authoritative state before acting and handle false/default targets. The public MFM-04 result containers remain mutable for interface compatibility; treat them as read-only snapshots.

## Actual offline verification

Commands run from this worktree:

```bash
python3 -I -B Tools/Verification/MfmCropTreeCandidates/run_candidates_offline.py --out /media/jer/Files/CCF/Verification/MFM05-Candidates-final
python3 -I -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM05-MFM04-regression
git diff --check
```

Both runners exit 0, compile 10 sources with C# 9 / .NET 8 and warnings as errors, then run two processes:

```text
MFM05_CROP_TREE_CANDIDATES_PASS checks=95 failed=0
MFM05_REPEAT_PASS two processes, byte-identical output: True
MFM04_DIGEST coverage=b50810a729c77c05 candidates=23b4eadf7e0f7228 release=a1577f2a1b137970 fit=98ce8d428bacb4f1 calibration=8591fcfc8f163e8f
MFM04_PRESCRIPTION_CORE_PASS checks=236 failed=0
MFM04_REPEAT_PASS two processes, byte-identical output: True
```

**95/95 focused + 236/236 regression = 331 distinct assertions; 662 assertion executions across four processes, zero failures.** The focused checks include 32 shuffled-input permutations, spacing boundaries, x/z/ID ties, numeric rack order, exclusion cases, manual/sample blockers, stable decision slots, stale IDs, single selection/navigation and negative APIs. Arbitrary diameter changes leave the candidate snapshot unchanged. Static source/API guards support the behavioral checks for no quality ranking, designation, bulk confirmation, Work Plan/Fell, save or I/O coupling; they do not claim live UI/Work Plan qualification.

Evidence SHA-256 for both run logs in each directory:

- Focused: `c3b6fced0a7025a9b858cb32dd26450f3c6b1ab949fe348e8b5b827e654eb2c3`.
- MFM-04: `fc4d2dd548f980302493e4c02d59d707b2838e06d8481bd16e1f05a5b94356b2` (identical to the MFM-04 handoff logs).

No disposable scripts entered Assets. No player save touched. No Unity/Editor compile, runtime, rendered review, native input, save/load or Windows build run.

## Interfaces handed to MFM-06/07/08

**MFM-06 — persistence/provenance:** recompute candidates using the existing Suggest interface; if required for durable individual decisions, supply prescription/batch identity + stable tree ID + confirm/reject choice using the approved persistence design, not serialized candidate coordinates or a second Crop Tree authority. Ordinary authoritative Crop Tree state remains the source for designations. Preserve rejected slots by supplying `rejectedIds`; supply recorded batch `confirmedIds` alongside current authoritative `Stem.Crop` flags. Store version/source context as required by that task. Reconcile `StaleDecisions`. G2: follow S1-C v22 with the next available schema; this task reserves no version/field and adds no persistence.

**MFM-07 — individual decision boundary:** needs a caller-owned operation taking one current tree ID and one player confirm/reject action, tied to the appropriate prescription/batch, with validated 3D inspection context. Before accepting it, regenerate/revalidate eligibility, live marks, manual choices and work conflicts. Confirmation uses the existing authoritative Crop Tree mechanism; rejection records the player's per-tree decision. This operation is a required later interface, **not implemented here**. Feed actual confirmed Crop Tree IDs to MFM-04 `ProposeRelease`; never derive them from all candidates or selection targets. Candidates and their targets cannot create proposals-as-Fell, orders, approval or Work Plan commitment. Pending work remains behind prescription approval then normal Work Plan approval.

**MFM-08 — map/3D UX:** use the two `Try*InspectionTarget` APIs for individual focus/Next/Previous; show the stable queue and decisions; handle invalid/stale results. Suggested copy: “Spacing identifies where to inspect. Inspect this tree in 3D and judge its suitability before confirming.” Map navigation/summary does not confirm. Candidate presentation must remain distinct from Blue Crop Tree, Red Fell and Orange Rack. Wire one confirm/reject action through MFM-07's validated boundary, without a bulk action. Required 3D grounding, rendered 720p/1080p checks and human workload testing remain here/later integration.

## Preserved anchors and conformance limits

- MFM-03 calibration/rule/signature unchanged; MFM-04's five digests byte-identical and fixture remains 17 candidates in its original order.
- Coverage, prescription, CompetitionShare/Hegyi, MFM-02 geometry, 30% release, 40% basal safeguard, 8 m teaching spacing/reach, 9 m × 12 m sample and candidate `.meta` unchanged (direct base diff plus unchanged regression).
- All ecology, density, RNG, save/hash compatibility, Legacy40 and frozen Reference Future v1 files untouched. Their full runtime hashes were **not rerun** by this static task.
- Checklist F1–F5 and F10: pure-domain behavior covered. F6/F7 require later actual player decision/3D enforcement; F8 has the data navigation interface but requires UI qualification; F9 manual designation remains a later existing-authority action respected here; F11 requires player workload testing. O6 semantics supplied as a reason/target and suggested copy, with rendering deferred. No checklist is marked globally complete.
- Preserve MFM-03 one-fixture/one-layout calibration limitations and its live drift/CI/player-authored-rack residuals. No rebaseline or waiver.

## Blockers and next gates

**No blocker for this authorized static candidate-system slice.** Manager G2 acceptance and G4 integration are outstanding. Unity import/compile and integrated interaction/Work Plan/save/rendered gates remain required in their later owned phases; this token does not claim `CROP TREE CANDIDATE FLOW QUALIFIED` or complete feature acceptance.

Operational follow-up: this lane has no configured project-dashboard ingestion/publisher or identified dashboard endpoint. Published dashboard/audit advancement is not verified. The claim comment and Git handoff report the status; the umbrella coordinator must reconcile its single-writer active-task record and verify dashboard synchronization. No generated dashboard or other worktree was edited.

**CROP TREE CANDIDATE SYSTEM QUALIFIED**
