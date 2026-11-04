# MFM-06 save v23 prescription/provenance foundation — offline checks

No Unity required; needs `dotnet` (SDK 8+) and `git`. Nothing is written inside the repository (temp project under `/tmp`).

```bash
python3 Tools/Verification/MfmSaveProvenance/run_offline_check.py          # MFM06_SAVE_OFFLINE_PASS
python3 Tools/Verification/MfmSaveProvenance/run_mutation_controls.py      # MFM06_MUTATION_CONTROLS_PASS (23 deliberate defects must all be killed)
```

`run_offline_check.py` builds two projects:

1. **baseline** — the pre-v23 `ForestSaveData.cs`, `ScenarioManagementEvent.cs`, `ScenarioOneWorkOrder.cs` and the hash half of `ScenarioReferenceArchive.cs` read from git at `ac876a6` (save v22). It prints the world-identity hashes (`WorldHash`, `CurrentWorldHash`, `LegacyCompatibleWorldHash`, V15–V18) of the fixture worlds in `HashFixtures.cs`, including parsed v21/v22 inputs.
2. **candidate** — the working-tree v23 sources (schema, validators, migration, hash path, `ScenarioOneManager.Prescriptions.cs`). It must reproduce every baseline hash byte for byte, then runs `PrescriptionSaveOfflineCheck.cs` (round trip of every new field, absent-field/no-fabrication, stable ids, ~70 malformed-data refusals with no-mutation checks, JSON presence/typing, validation order, canonical-order determinism, source-text wiring audits). The runner finally proves `Assets/ForestPrototype/ScenarioOne/Resources` (Reference Future v1) is unchanged against the base.

`run_mutation_controls.py` copies the C# sources to a temp tree, applies one defect at a time and requires the offline check to fail.

What this does **not** prove: Unity import/compile, the real `ForestSaveController` load order against a scene, Unity `JsonUtility` byte layout (see `Tools/Verification/MfmSaveProvenanceUnity/`).
