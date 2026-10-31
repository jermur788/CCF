# Rack V2 Current-Lineage Red-Gate Diagnosis 01 — Handoff

## Result

`RACK V2 CURRENT-LINEAGE RED-GATE DIAGNOSIS COMPLETE — READY FOR MANAGER REVIEW`

Claim: #197  
Lane: browser/GitHub read-only (#162)  
Reviewer/diagnostic owner: Overall Manager / OpenAI GPT-5.6 Sol  
Exact current integration: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`  
Historical MFM feature start used by #195: `cfdc93bf3943125d21570bc5749a9338f52d1cce`  
MFM-08 repaired candidate used only as comparison: `e5fb424635f16182748efcb0539e8a49539495a9`

No production, verification, fixture, scene, save or Unity state was changed by this diagnosis. No Unity lock was acquired. MFM-08 was not modified.

## Executive finding

The three red diagnostics reported by #195 do **not** form one Rack V2 defect.

1. **`guided-v2` is a stale verification-contract family.** The live `SequentialObjectivesVerification.cs` still asserts `ForestSaveData.CurrentVersion == 23` and hard-codes the pre-v24/pre-D-066 reload expectation `Stage(4, "... requires fresh rack review")`. The current project lineage is v24 and earlier manager records already identified pre-D-066/v23 teaching-harness expectations as stale after the persistent teaching-latch work. The reported lesson-index 6/7 and zeroed session-only rack-review counters are consistent with this obsolete contract. No production Rack regression is established by this gate.

2. **`combined-v2` / `combined-legacy` are current Scenario One HUD/focus regressions against a still-protected gate, but they are not MFM-08-specific and do not invalidate the accepted Rack V2/D-064 feature evidence by themselves.** The protected Combined HUD gate was repaired and requalified during RACK-V2-08: model-0 passed **450/0** and Rack V2 passed **455/0** after the accepted layout fix. #195 now reproduces a smaller stable red set on `cfdc93bf`, `e5fb4246` and current integration `ce673714`, so this is not an MFM change and not a transient one-run failure. The failing families are generic estimate/minimap, competitor presentation and focus-state composition checks.

3. **Rack V2/D-064 remains feature-scope VERIFIED.** The later independent G4 review accepted exact integrated Rack candidate `60fd9025e0205e7e8b24203295dd606ffa4a8061` after the R09 field gate, R11 real-disk/cold-start persistence, R12 **173 checks / 24 captures** end-to-end flow and R13 720p/1080p rendered review. Current integration retains that candidate in ancestry. The #195 diagnostics therefore create follow-up verification/UI debt, not a basis to revoke D-064.

## Failure inventory and classification

| Gate / failure family | #195 evidence | Classification | Impact |
|---|---|---|---|
| `guided-v2` annual-advance / lesson-index expectation | same first failure on `cfdc93bf`, `e5fb4246`, `ce673714` | **STALE VERIFICATION CONTRACT** | Verification reconciliation required; no production Rack defect established. |
| `guided-v2` fresh-rack-review reload expects lesson 4 but gets 6/7 | current integration; current source hard-codes `Stage(4, "... requires fresh rack review")` | **STALE VERIFICATION CONTRACT** | Pre-v24/pre-D-066 reload semantics in the gate must be reconciled to accepted current teaching-latch authority. |
| `guided-v2` `three future trees and six stand portions reviewed [0 0]` | current integration after reload path | **STALE VERIFICATION CONTRACT** with bounded residual check required | Current gate deliberately treats RackReviewComplete/RevisitedTrees/StandPortionsReviewed as session-only, while current v24 teaching lineage has later persisted latch semantics. Reconcile harness first; do not repair production without a post-reconciliation failure. |
| `combined-v2` `ESTIMATE_MINIMAP_STAYS_VISIBLE`, 1280 estimate overlap, 1920 competitor panel/tags, `FOCUS_04/06/07` both sizes | 447 pass / 11 fail identically on `cfdc93bf`, `e5fb4246`, `ce673714` | **CURRENT PRODUCTION/UI REGRESSION** relative to the protected Combined HUD contract; exact responsible later lineage not isolated here | Separate from MFM/Rack feature behavior. Needs bounded current-lineage UI/focus repair, preserving protected assertions. |
| `combined-legacy` `ESTIMATE_MINIMAP_STAYS_VISIBLE`, `FOCUS_04/06/07` both sizes | 440 / 8 on `e5fb4246` and `ce673714`; historical `cfdc93bf` report 416 / 32 | **CURRENT PRODUCTION/UI REGRESSION** relative to protected Combined HUD contract; some historical improvement exists | Same repair family; must preserve model-0/Legacy compatibility. |

## Why the Combined HUD reds are classified as regression rather than stale gate

During RACK-V2-08, the same protected Combined HUD assertions initially exposed genuine pre-existing layout defects. The Manager explicitly ruled that the assertions **remain valid and must not be weakened/rebaselined**. A separate layout repair was integrated (`b118d8e...`). RACK-V2-08 then reran the protected gate on exact candidate `0904f0f...` and obtained:

- Guided/Sequential model-0: 600/0;
- Guided/Sequential Rack V2: 553/0;
- Combined HUD model-0: **450/0**;
- Combined HUD Rack V2: **455/0**;
- Legacy40 and RACK-V2-07 preservation: PASS.

That candidate was Manager-approved/integrated. Therefore the current 11/8 Combined HUD failures are not an accepted historical red baseline and cannot be waived as inherited known failures. Their stability across the MFM feature start, MFM candidate and current integration makes an environment-only explanation unlikely.

The current Combined HUD README still defines these as live composition checks: estimate panel stays above the minimap, competitor list/tags avoid it, and focus/HUD/input state remains correct through inspection, Objectives, Stand Map/rack planning, Work Plan/help and return to forest. Nothing in D-064 supersedes those generic Scenario One UI contracts.

## Why this does not revoke Rack V2 G4

The final independent G4 review (`Docs/Handoffs/RACK-V2-14/FINAL-CONFORMANCE-REVIEW-20261030.md`) accepted exact integrated Rack candidate `60fd902...` and explicitly separated feature-scope verification from later whole-Scenario-One Tier-3. It reconciled:

- R09 field marking 99/10 + runtime/save evidence;
- R11 real-disk/cold-start persistence and malformed-save refusal;
- R12 173-check/24-capture complete Objective-1 flow, including Objective 2 transition;
- R13 1280×720 and 1920×1080 rendered visual PASS;
- AC-1…AC-20 and D-064 OPEN-1…OPEN-3.

The #195 Combined HUD failures concern generic walking-HUD composition/focus seams rather than rack geometry authority, physical entrance marking, rack persistence, or the D-064 commitment sequence. They remain relevant to eventual integrated/Tier-3 quality, but they do not establish that Rack V2 feature scope itself has regressed.

## Required successors

### A. Guided-v2 current-contract reconciliation

Verification-only first. Update the old `ScenarioOneRackUx` / `SequentialObjectives` companion so it expresses the accepted **v24 / D-066 teaching-latch** contract rather than v23 and pre-latch session-only reload assumptions. Preserve real-input, rack progression, three-future-tree/release/review coverage where still applicable. Do not weaken assertions or change production unless the correctly reconciled gate then exposes a genuine production failure.

### B. Combined HUD/focus current-lineage repair

Treat the 11/8 stable failures as a bounded Scenario One UI regression. Preserve the protected assertions. Root-cause the current estimate/minimap, competitor presentation and focus failures, make the smallest presentation/focus repair, then rerun both `combined-v2` and `combined-legacy` at both sizes plus proportionate affected UI/Rack gates. This work is separate from MFM-08.

## MFM-08 disposition

#197 does **not** add a new MFM-08 blocker.

#156 should continue to wait on #196 for its MFM-specific MapPlanning contract closure. If #196 passes the complete authoritative MapPlanning gate under the bounded state-aware reconciliation and finds no new MFM-specific failure, #156 may be upgraded on that evidence without waiting for the generic Combined HUD repair, subject to the existing final integration overlap review.

The generic Combined HUD repair must still be closed before final integrated Scenario One Tier-3/player-test readiness if those protected gates remain part of the integrated qualification set.

## Next action

Create two bounded successors:

1. verification-only guided-v2 v24/D-066 contract reconciliation;
2. current-lineage Combined HUD/focus UI regression repair.

No Windows build.
