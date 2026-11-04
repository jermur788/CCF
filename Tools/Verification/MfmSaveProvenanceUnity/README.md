# MFM-06 Unity gates (U1 only)

Disposable verification. Same safety model as `S1CObjectiveHistoryUnity/run_gate.py` (exact clean U1 HEAD, already-held machine-wide lock, isolated XDG config and private Xephyr display, harness + `.meta` removed afterwards, host saves digested). Evidence goes under `/media/jer/Files/CCF/Verification/`.

Gates (`--gate`): `compile` (must pass first, at `<run>/01-compile`), `hash-dump` (prints every protected world-identity hash through Unity `JsonUtility`; run on the base `ac876a6` and on the candidate, outputs must be identical), `save` (drives the real `ForestSaveController`: v23 round trip, repeated disk save/load with stable ids, v22-shaped file load with nothing fabricated, 23 malformed v23 documents + 7 typed corruptions refused with no world mutation), plus the unchanged historical protected gates `reference`, `hardening`, `progress`, `completion`, `completion-model1`, `n3`, `density-save`, `interaction`.

`save` writes `forest-save.json` only inside the isolated config directory; the runner hashes then removes it. Prescription state is stamped onto *captured save data*: MFM-06 contains no code that creates a prescription (that is MFM-07).
