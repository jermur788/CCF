# PARITY-04 — BOUNDED ADULT SPECIES DOCS / VERIFICATION PATCH

**Task ID:** `PARITY-04-PATCH`  
**Feature:** Scenario One Three-Species Parity P0  
**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Manager review:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/MANAGER-REVIEW.md`  
**Preflight handoff:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md`

## Status

**IMPLEMENTATION READY — DOCS / VERIFICATION ONLY.**

Final `ADULT SPECIES PARITY REQUALIFIED` remains blocked on PARITY-03 Unity qualification and the normal machine-wide Unity queue. This packet implements the bounded patch only and does not claim final runtime qualification.

## Target lane

- **Preferred executor:** Claude subscription 2, strongest available Claude reasoning/coding model (Claude Opus preferred; Sonnet acceptable if it records actual model).
- **Mode:** repository/code work in a dedicated clean worktree/branch. No Unity required for this patch.
- **Recommended branch:** `feature/scenario-one-three-species-parity-adult-docverify-v1`.
- **Base:** exact current `main` at task start, which MUST descend from Rack V2 integration `f23549b18b17b3542ec8f633d7309b11020990d4` and include the Manager review/spec clarifications.
- Refresh `Docs/Project/coordination/active-tasks.md` before claiming ownership.

## Goal

Make the current Oak/Beech adult-model documentation and verification contracts accurately describe and assert the already accepted implementation, without changing any biological value, arithmetic, serialized species asset, scene, prefab or production C#.

## Accepted implementation truth — do not change

- Oak adult calibration: `Pd 0.8`, `Ph 0.65`, `Dmax 120 cm`, `Hmax 42.5 m`, `Ci50 5`.
- Beech adult calibration: `Pd 0.9`, `Ph 0.75`, `Dmax 120 cm`, `Hmax 42.5 m`, `Ci50 6.5`.
- These are **GAMEPLAY CALIBRATION**, not empirical Irish broadleaf growth constants.
- Oak High / Beech Intermediate growing-space classes are weak/provisional ordinal representation; Oak > Beech is not empirical precision.
- Oak/Beech use the accepted shared legacy asymptotic adult-growth/height law.
- Scenario One species×site response is dormant/neutral at exactly `1.0`.
- Canopy/light is shared except existing crown-geometry/relaxation differences.
- Hegyi neighbourhood is shared at 8 m.
- The accepted Beech height-band miss is a provisional limitation, not a defect to retune.
- Under growth model 2's mixed/species-aware density route, victim selection is species-neutral and does not use species `Ci50`. Models 0/1 retain protected legacy behaviour; model-2 pure Sitka retains the exact Sitka route.

## Allowed files / surfaces

### Current-state documentation

Correct stale current implementation wording in:

- `Docs/Project/unity-project-overview.md`
- `Docs/BeechSpeciesParameters.md`
- `Assets/ForestPrototype/Docs/SessileOakV1.md`

If a dated historical adoption record is accurate for its date, leave it historical rather than rewriting history. Add a short current-state cross-reference if needed.

### Verification tooling

Allowed:

- `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`
- `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json`
- `Tools/Calibration/OakBeechGrowth/check_integrated_v1.py` only if a minimal explanatory/reporting change is necessary; do not weaken an assertion.
- `Tools/Verification/SiteInfrastructure/run_site_check.py` comment/docstring-only save-version correction.
- `Assets/ForestPrototype/ScenarioSiteProfiles.cs` comment-only save-version correction if the preflight finding still applies. Do not change executable behaviour.
- narrow verification README/handoff documentation required to explain the refreshed source contract.

No other file without returning to the Manager.

## Explicitly forbidden

- `SessileOak.asset`, `BeechSpecies.asset`, `SitkaSpruce.asset` edits;
- any scene/prefab/material change;
- any adult parameter/value retune;
- any growth, canopy, Hegyi, density, mortality or site-response production arithmetic change;
- any save field/schema change;
- any protected fingerprint/expected-value rebaseline;
- any Rack, S1-C, S1-D, MFM or unrelated UI change;
- any Windows build.

If a verification assertion cannot pass without a production/asset/value change, STOP and return `BLOCKED — MEASURED ADULT DEFECT / MANAGER REVIEW REQUIRED`.

## Source-contract refresh — Manager-authorised G3 change

The preflight found three normalized-source hashes in `integrated_source_contract.json` drifted after the previously green `8f9ed80` lineage. Manager authorises refresh **only for those three preflight-identified files**.

Before editing the contract:

1. Recompute the normalized hashes on the exact patch base.
2. Confirm the mismatch set is exactly the same three files recorded in the preflight.
3. Re-inspect/attribute each drift to the commits/findings recorded in the preflight and confirm it does not alter the accepted Oak/Beech adult arithmetic.
4. Record old hash, new hash, attribution and adult-arithmetic-neutral rationale in the handoff.

If any fourth mismatch exists or any drift changes adult arithmetic: STOP.

After refresh, the unchanged-strength gate must pass:

`Tools/Calibration/OakBeechGrowth/check_integrated_v1.py`

Expected exit:

`INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7`

Do not alter the gate merely to obtain green.

## Adult verification strengthening

Strengthen `BroadleafAdultGrowthVerification` without changing production code. It must assert, from the live loaded configuration/runtime where practical:

1. Oak and Beech accepted adult constants exactly match the values above; include Sitka protected constants where the existing gate already has authority.
2. Scenario One site response resolves to exactly 1.0 for Sitka/Oak/Beech at the reference site and sampled ecology cells remain neutral where expected.
3. Canopy/light result is species-neutral for identical geometry/state inputs; species differences may arise only from existing crown geometry/relaxation.
4. Hegyi cutoff remains exactly 8 m and a boundary check behaves identically across species (e.g. inside vs outside the 8 m threshold).
5. Oak/Beech height diagnostics at ages 50/75/100 are printed for evidence. The Beech values are **diagnostic only**, labelled as the accepted provisional height-band miss and not used as empirical pass/fail targets.

Keep existing legitimate assertions. Do not reduce coverage.

## Documentation corrections

At minimum:

- replace the stale current overview statement that Beech/Oak retain `Ci50 3` / `0.4 cm/yr` with the accepted current values and `GAMEPLAY CALIBRATION` classification;
- correct Beech current values and remove/qualify any wording that presents the superseded 40 m maximum as current empirical production truth;
- correct Sessile Oak current adult values;
- state that Scenario One site response remains neutral/dormant rather than implying active drought/waterlogging species penalties;
- record the accepted Beech height-band miss honestly;
- preserve historical statements where clearly dated as history.

## Non-Unity verification on exact patch HEAD

Run and record:

1. `check_integrated_v1.py` — expected PASS above.
2. `Tools/Verification/SiteInfrastructure/run_site_check.py` where the required .NET SDK is available — expected `SITE_INFRASTRUCTURE_PASS`.
3. Relevant static/typecheck/compile approximation already used by the repository for the modified verification C#.
4. Diff proving **zero serialized species asset, scene, prefab and production-C# change**.
5. Diff proving no protected expected values/fingerprints were changed.

If the environment cannot run one non-Unity check, report it explicitly; do not fabricate a pass.

## Unity / final qualification

**Do not take the Unity lock for this patch.** The machine-wide Unity queue is currently prioritised to S1-C-IMPL-02, and PARITY-03 Unity qualification must complete before PARITY-04 final qualification.

The later Manager-routed PARITY-04 Unity continuation will run the strengthened adult gate on the exact integrated candidate and consume PARITY-03 protected density/Sitka evidence.

## Required handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04/HANDOFF.md`

Include:

- exact starting and final SHAs;
- actual Claude model/provider and lane/worktree;
- complete changed-file list;
- before/after documentation corrections;
- the three source-contract old/new hashes + attribution;
- exact non-Unity commands/results;
- proof of zero production/asset/scene value/arithmetic diff;
- what remains for Unity qualification;
- blockers/limitations;
- task-claim lifecycle.

## Exit tokens

Success:

`PARITY-04 BOUNDED PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`

Stop:

`BLOCKED — MEASURED ADULT DEFECT / MANAGER REVIEW REQUIRED`

or

`BLOCKED — SOURCE CONTRACT DRIFT OUTSIDE AUTHORISED SET`

This packet does **not** authorise `ADULT SPECIES PARITY REQUALIFIED` by itself.
