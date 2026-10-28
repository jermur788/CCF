# Task Packet — PARITY-05 Unity Qualification

**Task:** `PARITY-05-UNITY-QUALIFICATION — run the strengthened early-life and browsing gates in Unity`  
**Authority:** approved `specs/scenario-one-three-species-parity-p0/`; PARITY-05 Manager review `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/MANAGER-REVIEW.md`  
**Integrated prerequisite:** PR #69 merged at `1d7445e302f31641183549a3e18a8c7282f3c36a`  
**Mode:** Tier 2 focused qualification only  
**Target lane:** T3 Code / local Unity-capable qualification lane  
**Recommended model/provider:** OpenAI Codex / strongest available Sol-class local coding+verification model; Claude Sonnet-class fallback is acceptable under the same single-owner lane  
**Windows build:** prohibited

## Queue state

This packet is published while `RACK-V2-08-LAYOUT-REPAIR-01 — fix the Stand Map and minimap Combined HUD layout regressions` claim #59 remains RUNNING and may own the machine-wide Unity lane. **Do not start Unity or claim the lock while another owner holds it.**

Before execution, refresh `Docs/Project/coordination/active-tasks.md`, claim comments and current `main`. The exact tested candidate must be a current-main descendant containing PR #69 with the PARITY-05 harness/docs blobs unchanged. Record the candidate SHA before Unity.

## Goal

Execute the new PARITY-05 early-life/browse assertions in real Unity and prove that the docs-and-verification integration did not move the protected lifecycle or browse hashes.

This is verification only. Do not retune values, change production behaviour, edit species assets, scenes/prefabs, save/schema, ecology/economy, Rack/MFM/S1-C/S1-D, Reference Future v1, or protected expected values.

## Required preflight

1. Refresh live coordination and confirm no other owner holds the machine-wide Unity lock.
2. Record actual model/provider, branch/worktree, candidate SHA and Unity lane.
3. Use a clean qualification worktree/lane; do not touch dirty `CCF-Primary` or another worker's worktree.
4. Confirm the candidate descends from PR #69 merge `1d7445e302f31641183549a3e18a8c7282f3c36a` and that the following files match the integrated PARITY-05 bytes unless a later Manager-approved change explicitly supersedes them:
   - `Tools/Verification/JuvenileMortalityFoundationVerification.cs`
   - `Tools/Verification/BrowsingProtectionVerification.cs`
   - `Tools/Verification/ThreeSpecies/OfflineLogic/check_early_life_conformance.py`
5. Acquire the machine-wide Unity lock normally before Editor execution.

## Required qualification

### G1 — Clean import / compile

- Import/open the exact candidate in the designated Unity qualification lane.
- Require normal completion with **0 C# compilation errors**.

### G2 — Juvenile early-life gate

Run `JuvenileMortalityFoundationVerification.Begin` using the harness's documented copy → run → remove process.

Require:

- `EARLY_LIFE_CONFORMANCE_VERIFY_PASS`
- `SHARED_JUVENILE_RULES_VERIFY_PASS`
- `JUVENILE_MORTALITY_FOUNDATION_VERIFY_PASS`
- normal process exit

Then run `JuvenileMortalityFoundationVerification.BeginCanonical` and require:

- `JUVENILE_CANONICAL_VERIFY_PASS`
- canonical neutral lifecycle hash exactly `BFC55473C1506067`
- Scenario One 0.2 lifecycle hash exactly `3485B6630C9EA448`

Any hash movement is a failed gate. **Do not rebaseline.**

### G3 — Browsing / protection gate

Run `BrowsingProtectionVerification` using the documented disposable-harness process.

Require:

- `BROWSE_EARLY_LIFE_CONFORMANCE_PASS`
- `BROWSING_PROTECTION_VERIFY_PASS`
- `BROWSE_MATRIX_HASH BEC549ED9EBA18B7`
- `BROWSE_MEASUREMENT_HASH 32224555690DD824`
- normal process exit

Any hash movement is a failed gate. **Do not rebaseline.**

### G4 — Preservation / cleanup

- Remove any disposable copied harness and generated `.meta` files from `Assets/ForestPrototype/`.
- Confirm no save slot or user preferences were intentionally changed; account for any unavoidable Editor-local files.
- Confirm no production/species/scene/save/schema files changed.
- Leave the qualification worktree clean except the required handoff publication.
- Release the machine-wide Unity lock normally.

## Failure handling

The new assertions have never run in Unity. Two first-run risks are already documented in the worker handoff: exact float equality in the browse probability assertion, and the shelter effective-state scan. If either fails while production state and protected hashes remain unchanged, treat it as a **harness defect requiring Manager disposition**, not as permission to change production behaviour or rebaseline expected values.

If a production defect is discovered, stop and return a precise blocker. Do not fix production within this qualification claim.

## Mandatory handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Unity/HANDOFF.md`

The handoff must include actual model/provider, branch/worktree, starting/current-main SHA, exact tested candidate, Unity version/lane, lock acquisition/release, commands, all marker/hash results, cleanup proof, repo status, any failure classification, and explicit statements for IMPLEMENTED / INTEGRATED / VERIFIED / ACCEPTED.

At task start, mark the GitHub task claim `[RUNNING]` with model/provider, branch/worktree and candidate SHA. At publication mark `[HANDED-OFF]`, link the handoff and close the claim.

## Exit tokens

Success:

`EARLY-LIFE SPECIES PARITY UNITY QUALIFIED — READY FOR MANAGER REVIEW`

Harness-only blocker:

`PARITY-05 UNITY HARNESS REPAIR REQUIRED — READY FOR MANAGER DISPOSITION`

Production/authority blocker:

`PARITY-05 UNITY QUALIFICATION BLOCKED — MANAGER REVIEW REQUIRED`
