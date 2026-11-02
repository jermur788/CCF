# MFM-12 integrated qualification tooling (verification-only)

This folder was added on `qual/mfm-12-integrated-v1` on top of the exact
production candidate `508b3e8124b5d9c477a4d2471cd56ebf6608f197`.
It changes no production file.

## Provenance

- `Mfm11CalibrationImpactGate.cs` is the accepted MFM-11 calibration/complete-loop
  witness copied verbatim from `origin/verify/mfm-11-calibration-impact-v1`.
  MFM-12 re-runs it on the exact candidate so the accepted MFM-11 result is
  carried forward as integration regression evidence instead of being recreated.
  Its marker remains `MFM11_UNITY_PASS` to keep that provenance honest.
- `MfmTreatmentReviewUnityGate.cs` is the accepted post-integration MFM-08
  treatment-review gate copied from `origin/qual/mfm-08-post-integration-v1`.
  It supplies the candidate confirm/reject/manual, proposal-vs-Fell, 3D
  player-judgement, conflict and rendered-state evidence.
- `run_gate.py` is the MFM-11 runner adapted only for task id `MFM-12` and the
  MFM-12 evidence root.
- `run_extension_gate.py` is the MFM-09 extension runner adapted only for task
  id `MFM-12`; it continues to stage the committed MFM-09 and MFM-08 harnesses.
- `run_matrix.py` sequences the offline and Unity gates. It never acquires or
  releases the machine-wide Unity lock.

## Ownership

Every Unity step requires the normal `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`
record held for task id `MFM-12`, the exact lane, owner and HEAD. The matrix
refuses a dirty lane, a wrong lock, or a running Editor.

## Main commands

```bash
# callers already hold the lock
python3 -B Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head <QUAL_HEAD> \
  --lock-id <LOCK_ID> --owner <OWNER> \
  --phase offline

python3 -B Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head <QUAL_HEAD> \
  --lock-id <LOCK_ID> --owner <OWNER> \
  --phase unity
```

The offline density suite is recorded as `ACCEPTED_LIMITATION` only when the
log contains exactly the accepted three pure-Beech R2 misses and no other
failure. The `0.40` criterion is never changed.

## Boundaries

Disposable harnesses may be staged under `Assets/ForestPrototype/` by the
individual runners and are always removed with their `.meta`. No production
file, save schema, calibration, protected anchor or scene is changed. Evidence
goes under `/media/jer/Files/CCF/Verification/`.
