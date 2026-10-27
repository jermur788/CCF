# RACK-V2-04 Publication Request

**Task:** publish the already-qualified RACK-V2-04 map-planning result to canonical GitHub state.

## Why this packet exists

Claim #30 reports a successful local qualification at production checkpoint `bcb4f345544e50cbb32b8fecd9f82a33a284b0e2` and exact tested candidate `68bcf6d0b1f0f6907d1dc0ba5a220583e4ade058`, with `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8`. However, the claimed publication branch `feature/scenario-one-rack-v2-map-planning-mode-v2` and publication commit are not currently visible on GitHub. The only visible `Docs/Handoffs/RACK-V2-04/HANDOFF.md` is the historical blocked v1 handoff. Local-only qualification is therefore incomplete for Manager review/integration.

## Mode

**PUBLICATION ONLY.** Do not reimplement, retune, or rerun Unity unless publication integrity cannot be established from the existing clean worktree/evidence.

## Required actions

1. Refresh `origin/main` and live `Docs/Project/coordination/active-tasks.md`.
2. Use the existing clean RACK-V2-04 v2 worktree/branch if still intact. Preserve unrelated work.
3. Verify the production checkpoint is exactly `bcb4f345544e50cbb32b8fecd9f82a33a284b0e2` and the tested verification-only HEAD is exactly `68bcf6d0b1f0f6907d1dc0ba5a220583e4ade058`.
4. Verify `Assets/`, `Packages/`, and `ProjectSettings/` at the tested HEAD are byte-identical to the production checkpoint, as reported in claim #30.
5. Publish the v2 branch and `Docs/Handoffs/RACK-V2-04/HANDOFF.md` to GitHub. The handoff must identify the exact production checkpoint, exact tested HEAD, 142-check Unity result, 8 rendered captures at 1280x720 and 1920x1080, non-mutation evidence, lock release, and the existing caveat that the player rack was an in-memory fixture.
6. Do not edit `active-tasks.md`; Manager owns live coordination.
7. Do not change production code, Rack geometry policy, OPEN-3, save/schema, field marking, minimap scope, MFM, or Windows build state.

## Stop conditions

Return `PUBLICATION MISMATCH — MANAGER REVIEW REQUIRED` if any claimed SHA, production-byte identity, handoff content, or evidence provenance cannot be reproduced exactly from the existing worktree/history.

## Success exit

`RACK-V2-04 QUALIFICATION PUBLISHED — READY FOR MANAGER REVIEW`

At success, link the canonical GitHub `Docs/Handoffs/RACK-V2-04/HANDOFF.md` and record the published branch/commit in the task claim.
