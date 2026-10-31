# MFM-08 Integration 01 (#205) — Handoff

**Claim:** #205 · **Parent feature:** #156 · **Browser Manager:** #200/#202 · **Local orchestration:** #201/#203
**Status:** complete — exact approved MFM-08 delta replayed onto the current Scenario One integration lineage, no conflicts, post-replay Unity/rendered gates green.
**Token:** `MFM-08 INTEGRATED ON CURRENT SCENARIO ONE LINEAGE — READY FOR MANAGER REVIEW`

Worker: OpenCode `opencode-go/deepseek-v4.1-flash` (orchestrator + bounded mechanical integration and gate execution).

## Exact SHAs

| Role | Ref | SHA |
|---|---|---|
| Published Scenario One integration (input/start) | `origin/integration/scenario-one-player-test-ready-v1` | `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` |
| Common merge base | — | `cfdc93bf3943125d21570bc5749a9338f52d1cce` |
| Approved MFM-08 production candidate (input) | `feature/mfm-08-map-3d-review-ux-v1` | `e5fb424635f16182748efcb0539e8a49539495a9` |
| **Integrated production candidate (post)** | `integration/mfm-08-onto-scenario-one-v1` | **`34e360f85835afd15f5323f287372e0f2ee83dd9`** |
| Post-integration qualification harness (verification only) | `qual/mfm-08-post-integration-v1` | `c9336186781022f44b9c2e53eebb6764da080204` |

Branch `integration/mfm-08-onto-scenario-one-v1` is based on exact `ce673714...` and contains exactly three replayed commits:

- `7fa49282` MFM-08: Stand Map treatment review layer and 3D review companion (offline candidate, WIP)
- `9f5ba87e` MFM-08: bound review card height, chip outline colour; add offline/static treatment-review gate
- `34e360f8` MFM-08: per-segment area footprints and shared waypoint derivation for the Treatment review map

Cherry-pick hashes differ from the source branch because the parent lineage differs; content is identical (below).

## Integration method and equivalence

- Method: `git cherry-pick cfdc93bf3943125d21570bc5749a9338f52d1cce..e5fb424635f16182748efcb0539e8a49539495a9` onto `ce673714`.
- Conflicts: **none**. No duplicate type/API, no unexpected overlap.
- Equivalence proof: `git diff ce673714..34e360f8` is **byte-identical** to `git diff cfdc93bf..e5fb4246` (`diff -q` clean; both patch streams SHA-256 `9715913ceb7c95379c7978d62f849a17752f8a7c1865cebd5fa2a5a1e8ce7101`).
- The 29 integration-side commits between `cfdc93bf` and `ce673714` are preserved (the replayed commits sit on top of `ce673714`).
- Pre-flight overlap check confirmed independently of the browser review: the later lineage changes **no** file that the MFM-08 delta touches (`git diff --name-only cfdc93bf..ce673714` ∩ `git diff --name-only cfdc93bf..e5fb4246` = empty).

Final changed-file scope against `ce673714` (15 files, all MFM-08 additions plus the two pre-existing MFM-08 production paths):

```
A Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs (+ .meta)
A Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs (+ .meta)
A Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs (+ .meta)
A Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs (+ .meta)
A Assets/ForestPrototype/UI/StandMapTreatmentReview.cs (+ .meta)
M Assets/ForestPrototype/UI/StandMapView.cs
M Assets/ForestPrototype/UI/TreeInspectionView.cs
A Tools/Verification/MfmTreatmentReview/OfflineLogic/AreaFootprintChecks.cs
A Tools/Verification/MfmTreatmentReview/OfflineLogic/TreatmentReviewOfflineCheck.cs
A Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py
```

No save/schema/anchor/reference/ecology/economy path is in the replay diff.

## Not absorbed (explicit)

- **#196 verification-only patch `eef1bdf9` is NOT in the integrated production branch.** Its reconciled navigation-tab contract was applied only inside the throwaway qualification harness (`qual/mfm-08-post-integration-v1`, `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs`) so the authorised six-tab V2 contract could be asserted. The integrated production line keeps the lineage's own harness file.
- **#199 partial branch `42742a8a` is NOT absorbed** (neither its fixture change nor any production file).
- Unrelated handoff/docs commits from verification branches are not absorbed.
- No new product behaviour, UI redesign, calibration, schema or protected-value change.

## Verification receipts (OBSERVED FACT)

Machine-wide Unity lock held by task `MFM-08-INTEGRATION-01-205` (owner `opencode-deepseek-v4.1-flash`, lane `/media/jer/ZX20/CCF-U1`) and released normally. All runs serialized; one Editor at a time.

### 1. Import / compile

Unity 6000.6.0f1 imported and compiled the integrated candidate for both gate runs: `Assembly-CSharp.dll` / `Assembly-CSharp-Editor.dll` built, **0** `error CS` in both logs, Editor exit 0.

### 2. Focused MFM-08 treatment-review qualification (post-replay)

- Command: `Tools/Verification/MfmTreatmentReviewUnity/run_gate.py` (accepted #156 Tier-2 gate) at candidate `c9336186`.
- Result: **`MFM08_UNITY_PASS checks=373 shots=25`**, Editor exit 0, 290 s.
- Coverage: sample/coverage/candidate/proposal/3D-review behaviour, approval boundary, in-memory save/world-hash round trip, no save file written, both render sizes 1280×720 and 1920×1080.
- Log: `/media/jer/Files/CCF/Verification/MFM08-PostIntegration-Unity-R2-20261031T1245Z/unity.log` (SHA-256 `2759b55b89383e249f13b88ff38642637785c1b3e2a62c1a3dd3e257585af8f0`).
- Captures inspected: review card, candidate card, 3D candidate chip / confirmed / rejected at both sizes.

### 3. Authoritative RackV2MapPlanning gate (Manager-approved #196 contract)

- Command: `Tools/Verification/ScenarioOneProfessionalMap/run_unity_harness.py --name RackV2MapPlanningVerification ...` with `CCF_UI_SIZES=1280x720,1920x1080`.
- Result: **`RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`**, exit 0, 90 s.
- Capture inspection (1920×1080 planning-active): original five tabs `Access & position · Light · Regeneration · Browsing / protection · Fell & crop marks` plus additive `Treatment review`; `Planning Racks — Active` remains a sibling tool control, not a tab; dashed suggested + solid planned rack legend intact.
- Log: `/media/jer/Files/CCF/Verification/MFM08-PostIntegration-MapPlanning-20261031T1250Z/RackV2MapPlanningVerification.log` (SHA-256 `f14c64e2da7d85fc33be23257d5f9308a5a96409ee21de93ae6bdbc241f2231a`).

### 4. Save / Reference Future / Legacy compatibility (proportionate)

- PARITY-08 offline save-compatibility check: **`PARITY08_SPECIES_OFFLINE_PASS checks=23`**, including `teaching latch does not move WorldHash` (v24/D-066 compatibility). Evidence: `/media/jer/Files/CCF/Verification/MFM08-PostIntegration-Parity08-20261031T1255Z`.
- The replay diff contains no save/schema/reference/anchor path, so no Reference Future or Legacy40 bytes were touched or rebaselined. The MFM-08 Unity gate additionally proves an in-memory capture/reload round trip preserves `ScenarioReferenceArchive.WorldHash` and writes no save file.
- Legacy/model-0 HUD regression is deliberately **not** claimed here: it is the #199 surface and is scheduled separately by Manager packet.

### 5. Offline/static MFM-08 logic gate

- `Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py`: **`MFM08_TREATMENT_REVIEW_PASS checks=297 failed=0`**, `MFM08_REPEAT_PASS` (two processes byte-identical), warnings-as-errors build PASS.
- Its `MFM08_BOUNDARY_FAIL` result is **expected and not a defect** on this lineage: the boundary check is pinned to the MFM-08 start commit `cfdc93bf` and therefore flags the 29 preserved later integration commits. The authoritative scope check for this task is the equivalence proof above (diff vs `ce673714` byte-identical to the approved delta).

## Disclosed verification-only reconciliation

The accepted #156 Tier-2 gate pinned `ForestSaveData.CurrentVersion == 23` and labelled its in-memory round trip "v23". The current accepted Scenario One lineage is **v24** (D-066 first-guided-review latch), already Manager-accepted in #198, so the pin was stale.

- Change (qual branch only, `Tools/Verification/MfmTreatmentReviewUnity/MfmTreatmentReviewUnityGate.cs`, 2 lines): `== 23` → `== 24` and the label "reloads as v23" → "reloads as v24".
- This is a stale-contract reconciliation of exactly the #198 class. It does not weaken the assertion (the version pin is still exact) and touches no production file. The first run at `d99d2892` failed only on this stale pin (`MFM08_UNITY_FAIL checks=3 ... unchanged save version 23`) before the reconciliation; the retry at `c9336186` passed 373/373.

## Cleanup / return state

- Unity lock: acquired `f005c4cf8ecc4a18a5448d452a01185d` (v1), `fe3c16f321df4ba79b77d8b00ff84070` (v2) — both released normally; live status `free`, metadata `null`, no Editor PIDs.
- Editor side effects (`.vscode/settings.json`, SectionFive materials) restored byte-exactly; worktree clean.
- No staged harness or generated `.meta` left in `Assets/ForestPrototype/`; no Xephyr/openbox/Editor process remains.
- Disk: 18 GiB free on `/media/jer/ZX20` (97 % used) — marginal but sufficient; no cleanup performed without authorisation.

## Residuals

1. Manager review of this exact integration is required before MFM-09 is repinned. Worker completion is not acceptance.
2. The #156 qualification gate's save-version pin needed reconciliation; if that harness is promoted into verification policy, it should carry the v24 pin rather than v23.
3. #199 production layout repair remains blocked on the post-#205 repin, as the Manager disposition requires.
4. No Windows build attempted or authorised.
