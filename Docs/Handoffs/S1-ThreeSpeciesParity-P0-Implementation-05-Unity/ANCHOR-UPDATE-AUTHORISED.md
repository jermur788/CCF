# PARITY-05 browse-anchor update — authorised

**Date:** 2026-10-29
**Authority:** direct user decision in the #91 orchestration thread, selecting “Accept the new hashes with attribution”.
**Scope:** verification anchors only; no production, species, ecology, economy, save/schema, Rack/MFM/S1-C/S1-D or Reference Future v1 change.

## Authorised values

| Anchor | Previous packet value | Authorised current value |
|---|---|---|
| `BROWSE_MATRIX_HASH` | `BEC549ED9EBA18B7` | **`92A0DA50D6F5D3D9`** |
| `BROWSE_MEASUREMENT_HASH` | `32224555690DD824` | **`A3541E0EDF86DE66`** |

## Evidence supporting the update

- The repaired harness reproduces both new values on the **exact PARITY-05 integration prerequisite** `1d7445e302f31641183549a3e18a8c7282f3c36a` (PR #69 merge), not only on the later umbrella integration.
- Both bases produce identical values, so Rack V2, S1-D T-06, RACK-V2-09..14 and PARITY-06 did not move these browse anchors.
- All behavioural checks pass at both bases: `BROWSE_EARLY_LIFE_CONFORMANCE_PASS`, `BROWSING_PROTECTION_VERIFY_PASS`, `BROWSE_SCENE_RESTORED_PASS`, 144 matrix conditions, `BROWSE_NEUTRAL_LIFECYCLE_PASS`, `BROWSE_SCENARIO_LIFECYCLE` exact `3485B6630C9EA448`.
- The canonical juvenile hashes remain exact: `BFC55473C1506067` and `3485B6630C9EA448`.
- The previous packet values therefore do not reproduce on their own approved base under the required Legacy40 configuration; they are stale/inconsistent rather than a later-integration regression.
- Exact evidence: `/media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g3-baseline-1d7445e/` and `g3-browsing/`.

## Effect

- PARITY-05 G3 is **PASS** under the authorised anchors.
- #101 can close; #70 can close as `EARLY-LIFE SPECIES PARITY UNITY QUALIFIED — READY FOR MANAGER REVIEW`.
- Future PARITY-05 runs must use the authorised values above.
- No further anchor change is authorised by this document.
