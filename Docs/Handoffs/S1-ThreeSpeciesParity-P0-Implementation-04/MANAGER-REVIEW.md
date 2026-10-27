# PARITY-04 Bounded Adult Patch — Manager Review

**Verdict: APPROVED / INTEGRATED — UNITY QUALIFICATION STILL REQUIRED.**

Returned handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04/HANDOFF.md`  
Patch branch: `feature/scenario-one-three-species-parity-adult-docverify-v1`  
Tested patch HEAD: `17aa738c58b474476ce9c6d1f6163d42c63ae5d5`  
Published handoff commit: `704f42d34e5f12b21ff187eec43a02080acb1106`  
Task claim: #17 `[HANDED-OFF]`, closed  
Canonical integration: PR #26, merge `71b5ae4c83f3ce6d489cdec81f3306abb54d9df1`

## Decision

The bounded patch is accepted and canonically integrated. This is **not** final adult-species qualification and does **not** authorize `ADULT SPECIES PARITY REQUALIFIED` until the strengthened Unity gate runs successfully after PARITY-03 Unity qualification.

The accepted patch surface is exactly:

- `Assets/ForestPrototype/Docs/SessileOakV1.md`;
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs` — comment only;
- `Docs/BeechSpeciesParameters.md`;
- `Docs/Project/unity-project-overview.md`;
- `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json`;
- `Tools/Verification/SiteInfrastructure/run_site_check.py` — docstring/current-version wording plus existing check execution;
- `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`;
- the handoff Markdown.

No species asset, scene, prefab, package, project setting, save field, production arithmetic, biological value, protected fingerprint or expected product invariant changed. The only production `.cs` file changed is `ScenarioSiteProfiles.cs`, and its change is comment-only.

## G3 / verification-contract disposition

The attributed three-file source-contract refresh is accepted. The worker demonstrated that the mismatch set was exactly the three preflight-identified sources and attributed each normalized-hash drift to later commits. The unchanged integrated-calibration gate then passed with `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7`. No fourth source was silently added and no protected expected value was rebaselined.

The Beech height-band miss remains an accepted provisional calibration limitation. Height values are diagnostic output, not a pass/fail target; do not retune Beech or introduce a new height model merely to close those provisional bands.

## Site-seam note

The strengthened adult Unity gate does not call the dormant site-seam helper because the existing `run_site_check.py` source-policy check rejects references to those seam types outside its allowed location. This is accepted for PARITY-04 because:

1. the separate site check already asserts the dormant seam returns exactly `1.0` for Sitka, Oak and Beech;
2. the adult gate directly asserts the live loaded ecology/site scalar is exactly `1.0` across the exercised scene/cells;
3. changing the site-check allow-list was outside the bounded patch surface and is not required to establish the accepted neutral-site product rule.

Do not widen PARITY-04 merely to make the adult gate call the dormant seam. Any future cleanup of that source-policy mechanism is tooling hygiene, not a product-model change.

## C# type-check hygiene note

The repository's standalone type-check route is not accepted as proof of method-body correctness for this patch because existing `Painter2D` errors prevent the compiler from reaching normal body binding. The worker's scratch negative-control/restored-body-check experiment is useful supporting evidence only. The actual Unity import/compile in the next task is the authoritative compile gate.

No production change is authorized under a tooling-hygiene pretext. If Unity compilation fails, return the exact failure to the Manager.

## Remaining qualification

PARITY-04 Unity qualification must wait until PARITY-03 Unity claim #10 completes successfully. On the exact integrated candidate, run:

- Unity import/compile;
- `BroadleafAdultGrowthVerification` with the strengthened assertions, expected successful marker/check count from the patch handoff;
- the static integrated-calibration/source-contract check;
- the site check;
- fixture controls;
- any narrow protected comparator checks required by the packet.

No Windows build and no retuning/rebaseline.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
