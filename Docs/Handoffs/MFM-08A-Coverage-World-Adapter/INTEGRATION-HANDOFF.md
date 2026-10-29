# MFM-08A — Integration Handoff

**Status:** INTEGRATED / MANAGER-APPROVED  
**Claim:** #129  
**Reviewed production candidate:** `716486a21588e76a482d3eaea4fed12069f75ffd`  
**Worker docs successor:** `0b33c3288ebc78ab7361d0cdb3fcee537324dadb`  
**Pre-merge integration head:** `08a060d14c50753009e06e29e650b778a5762bec`  
**Exact integration merge:** `e19f026d0fa4978e89836caa5f0ec94cba61cd59`

## Integration decision

Manager verdict: **APPROVED**. Review is published on canonical `main` at `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/MANAGER-REVIEW.md` (publication commit `806bb692fd0c230cff4d447fc07c5dd6ca6a92b7`).

The integration branch differed from the exact implementation base `c8cfae8c4025cec6e960a050edf4bfde2a981de5` only by MFM-07 integration-documentation commits. There was no overlapping production delta. The merge therefore preserved both histories without conflict:

- first parent: `08a060d14c50753009e06e29e650b778a5762bec`;
- second parent: `0b33c3288ebc78ab7361d0cdb3fcee537324dadb`;
- merge tree includes the exact MFM-08A production files and worker HANDOFF while preserving the latest MFM-07 integration handoff wording.

No post-merge Unity re-run was required for this docs-only divergence. The exact production code at the merge is the already qualified `716486a2` content.

## Accepted evidence

- Offline: coverage-world 102/102; MFM-04 240/240; MFM-05 95/95; MFM-07 125/125, each repeated identically.
- Unity 6000.6.0f1 on exact production candidate: compile PASS; coverage-world 56; MFM-07 approval 106; MFM-06 save 159; hash dump identical 18/18 to accepted MFM-06 base; Reference Future v1 PASS.
- Live witness: Track/Clearing/Site/Headland masks all materially affect coverage; moving player does not; read is deterministic/non-mutating; active management work, shelter and clearance-patch exclusion plus natural-regeneration flagging are observed.
- Protected-area polygon mapping fails closed rather than inventing sampling precision.
- No calibration/save/schema/UI/Work Plan/Reference Future change; no rebaseline; no Windows build.

## Accepted residuals

Standing-juvenile exclusion, deadwood record, runtime-built Site and protected-area fail-closed were not all created as live Unity fixtures; those branches are covered offline/static, with dead-juvenile non-mapping exercised live. This is an evidence-strength limitation only and does not block MFM-08A integration.

## Downstream release

MFM-08 Stand Map / 3D Review UX (#156) may now be repinned to this integrated lineage and released, subject to live UI ownership. It must consume `ScenarioOneManager.ReadCoverage` rather than recreate a second coverage-world adapter, and it still requires its own Tier-2 1280×720 / 1920×1080 rendered and interaction qualification.

The machine-wide Unity lane remains separately scheduled. #147 S1-C T-06 was already offline-approved and is the immediate waiting Unity qualification task after #129 released the lane; live lock/process/free-space checks govern actual acquisition.

No Windows build is authorised.
