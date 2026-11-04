# PARITY-05 browse-hash baseline attribution

**Claim:** #101 (parent #70), umbrella #91.
**Decision required:** Manager/user disposition on the protected `BROWSE_MATRIX_HASH` / `BROWSE_MEASUREMENT_HASH`.
**Production changed:** No. **Anchor rebaselined:** No.

## Experiment

- Repaired verification harness from `fix/parity-05-unity-harness-v1` (`18b31ad`), run on the **exact PARITY-05 integration prerequisite** `1d7445e302f31641183549a3e18a8c7282f3c36a` (PR #69 merge).
- Unity 6000.6.0f1, lane U1, Legacy40 (`CCF_STAND_GEOMETRY=0`), normal lock `d74ca21838e84ab8a3b439059b36c78a` released normally.
- Evidence: `/media/jer/Files/CCF/Verification/Parity05-HarnessRepair-20261029/g3-baseline-1d7445e/`.

## Result

| Measurement | Packet expected | At `1d7445e` (PARITY-05 base) | At current integration `18b31ad` |
|---|---|---|---|
| `BROWSE_MATRIX_HASH` | `BEC549ED9EBA18B7` | `92A0DA50D6F5D3D9` | `92A0DA50D6F5D3D9` |
| `BROWSE_MEASUREMENT_HASH` | `32224555690DD824` | `A3541E0EDF86DE66` | `A3541E0EDF86DE66` |

- The repaired harness passes all behavioural checks on both bases: `BROWSING_PROTECTION_VERIFY_PASS`, `BROWSE_EARLY_LIFE_CONFORMANCE_PASS`, `BROWSE_SCENE_RESTORED_PASS`, 144 matrix conditions.
- The exact same hashes appear on the PARITY-05 base and on the later umbrella integration (which adds Rack V2, S1-D T-06, RACK-V2-09..14, PARITY-06).
- G1 compile and G2 juvenile/canonical gates pass with exact canonical hashes `BFC55473C1506067` / `3485B6630C9EA448` (the earlier failures were stale Legacy40 fixture configuration).

## Conclusion

The browse-hash difference is **not** caused by later integrated production changes. It reproduces on the PARITY-05 implementation's own integration base. The packet's two expected browse hashes therefore do not reproduce against their own approved base under the required Legacy40 configuration. This is a provenance/expected-value mismatch in the PARITY-05 Unity packet or an earlier harness/Unity-version measurement, not evidence of a Rack/P06/T-06 regression.

No production defect is established. No anchor has been rebaselined; the new values are diagnostic only until explicitly accepted. The safe options are:

1. Authorize the new `92A0DA50D6F5D3D9` / `A3541E0EDF86DE66` as the corrected PARITY-05 browse anchors, with this exact-base attribution and the behavioural pass recorded (unblocks #70).
2. Require a further bounded provenance audit of where the packet's old hashes were measured before accepting.
3. Hold the parity lane and continue independent S1-C/S1-D/MFM work; the final playtest candidate remains blocked until PARITY-05 is dispositioned.

**No action beyond this attribution has been taken.**
