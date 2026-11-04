# S1-D T-03 — RackLesson half-width reconciliation (verification only)

**Verdict: RACKLESSON RECONCILED — T-03 CANDIDATE AND CLEAN INTEGRATION BASE PASS.**

Claim #103 / umbrella #91 / T3 lane #106. Verification-only change; no production, save, schema, Reference or anchor change. No Windows build.

## Identity

| | |
|---|---|
| Branch / worktree | `fix/s1-d-t03-racklesson-calibration-v1` / `/media/jer/Videos/CCF-S1D-T03Lesson` |
| Base | `8a92a6eddba7a98ca64dd10dc1db636bfe2a49ee` (T-03 qualification head; production `f629be3`) |
| Reconciled head | `217595feba6658a38c40f1eaacd54d1d6ff98973` |
| Integration base inspected | `aca74acacc565f72ab802177cf69ca6ebb54d637` |
| Authority | Browser packet `BROWSER-T03-RACKLESSON-RECONCILIATION.md`; MFM-02 Manager review `Docs/Handoffs/MFM-Implementation-02/MANAGER-REVIEW.md` (merge `07eee3d`) |

## Root cause and authority check

The authoritative production expression is `ScenarioOneRackBay.HalfWidth(sortedLines, index)` (`ScenarioOneRackBay.cs:379-385`): half the minimum gap to the neighbouring layout lines. On the current regenerated model-0 Enlarged80 world the authored rows leave a `15.989349 m` gap, so the live half-width is `7.9946747 m`.

MFM-02 was Manager-approved and integrated (merge `07eee3d`). Its review accepted this regenerated fixture: 69 living trees and `1.2949037088 m²` before access, identical membership/BA, half-width `7.9946747 m`, 9.2 mm inside the historical `8.00385 m` scene fixture. MFM-02 explicitly did **not** rebaseline the historical scene fixture or any protected target; it asserts the live gap formula and the accepted ~8.0 m calibration.

`8.00385 m` is a historical scene-fixture value used by the old RackLesson expectation, not a D-064/Rack V2/Reference Future protected anchor. The T-03 diff contains no rack-geometry production change, and the same failure reproduced on clean `aca74ac`.

## Change (1 file, +5/−1)

`Tools/Verification/ScenarioOneNonUi/UnityGates/RackLessonVerification.cs`: the stale exact expectation `8.00385 m` is replaced by the authoritative live value `7.9946747 m`, with a comment recording the MFM-02 provenance. Tolerance is unchanged (`.001 m`); the 69-tree footprint check, 2.0 m lattice check and the exact "half the gap to neighbouring layout lines" derivation check are unchanged. No tolerance broadening, no production geometry change, no assertion removed.

## Unity evidence (U1, machine-wide lock, Unity 6000.6.0f1)

Evidence root `/media/jer/Files/CCF/Verification/S1D-T03-RackLessonReconciliation-20261029/`.

| Gate | Head | Result |
|---|---|---|
| `lesson` (RackLesson) | `217595f` (T-03 descendant) | **PASS**, exit 0, pass marker, 330 s, lane clean, no Editor, no disk saves |
| `cap` (T-03 primary) | `217595f` | **PASS**, `TEACHING_CAP_UNITY_PASS`, exit 0, 288 s, lane clean |
| `lesson` on clean integration | `aca74ac` with patched harness | **PASS**, exit 0, pass marker, 202 s, lane clean |

Both lock cycles were acquired and released normally (`ee10c72c…`, `eb199755…`); U1 was clean after each run and no harness/meta/licence copy remains. Earlier T-03 gates (teaching, menu, rack, event-id, history, history-browse) passed on the same production tree at `8a92a6e`; this change is test-only and does not alter them.

## Status

T-03's pre-existing RackLesson residual is correctly classified as stale verification expectation and reconciled. The T-03 candidate and the clean integration base both pass. T-03 production remains exactly `f629be3`; integration review remains the parent's decision. No Windows build.
