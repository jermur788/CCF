# MFM-05 spacing-only candidate checks

Run from the repository root (Python 3 and .NET SDK 8+):

```bash
python3 -I -B Tools/Verification/MfmCropTreeCandidates/run_candidates_offline.py
python3 -I -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py
```

The first runner compiles the actual production candidate source and the MFM-04 dependency manifest with C# 9 and warnings as errors. It runs the focused checks in two separate processes and requires byte-identical output. The second is the unchanged MFM-04 fixture regression (including the MFM-03 17-candidate sequence and calibration digest).

Default evidence/build output goes under `/media/jer/Files/CCF/Verification/`; `--out` may select an external directory. The runners never copy scripts into Assets, open Unity or access player saves. Temporary bin/obj files in their own external build directory are removed; build project and logs remain as evidence. No Unity `.meta` cleanup is required.

The 95 focused assertions cover spacing boundaries, numeric rack/segment order, deterministic centre/x/z/id tie breaks, 32 shuffled-input permutations, duplicate membership, exclusions, manual and sample blockers, batch decision semantics, stale decisions, single-target navigation, no automatic designation/confirmation/bulk API, diameter independence and static dependency boundaries. The source guard is supporting evidence; behavioral isolation also follows from immutable Stem inputs and the pure three-method API. It does not establish live Work Plan integration or 3D inspection enforcement.

`TryGetInspectionTarget` and `TryMoveInspectionTarget` consume a fresh `Suggest` result. They return a single undecided navigation target and never record a decision. Next/Previous skip decided entries, stop at the ends and accept an empty current ID to start at first/last. Unknown IDs and invalid results return false. A confirmed/rejected batch entry can anchor navigation but cannot be selected for inspection again. Selection is not proof of 3D inspection.
