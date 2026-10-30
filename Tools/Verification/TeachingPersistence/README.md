# D-066 / S1-D T-04P verification

Offline first (output outside the repository, on Files):

```bash
python3 Tools/Verification/TeachingPersistence/run_offline.py --out /media/jer/Files/CCF/Verification/<run>/offline
python3 Tools/Verification/MfmApprovalBoundary/run_approval_boundary_offline.py --out /media/jer/Files/CCF/Verification/<run>/mfm07
python3 Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py --out /media/jer/Files/CCF/Verification/<run>/n3
```

The first runner compiles actual production sequence/cap/session-review code and the exact latch storage/write method extracted from the manager. It compares 16,384 pre-latch rows against the original sequence read from exact base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`. It runs the existing T-03 decision table, MFM-06 save/provenance checks and 80 v23/v24 all-hash fixtures (both latch values, both geometries, default/rack/prescription states, older input headers). It checks protected sources/resources against that base. The offline serializer is a stand-in; real Unity qualification is separate.

Under the normal machine-wide Unity lock, with a clean U1 detached at the exact candidate, use `Tools/Verification/MfmApprovalBoundary/run_unity_gate.py` with `--task-id S1-D-T04P --owner Codex-S1D-T04P --expected-head <SHA> --lock-id <id> --project /media/jer/ZX20/CCF-U1 --out <Files-run>/<gate>`.

Run `compile` first at `01-compile`, then `teaching-latch`, `save`, `hash-dump`, `hardening`, `reference`, `approval` and `n3`. Every gate must pass. The runner stages/removes the disposable source and `.meta`, uses isolated saves and restores only recorded Editor import side effects. Never run it on someone else's lane/lock.

`PersistenceUnityVerification` extends the already-qualified model-0 Enlarged80 rack lesson fixture. It executes real access/release Work Plans, inspection raycasts and queued New Input System J decisions; no cached facts or completion index is injected. It proves false before review and after Annual Review alone, true at the actual final session decision, real controller disk round-trips for true and false, read/reload idempotence, Crop Tree loss/count drift, release-accounting drift, later in-footprint felling, old missing-field saves and all-hash equality with non-default prescriptions. Queued input is synthetic, not a native Windows test. This does not qualify #124's later curriculum replay.

Compare the Unity `hash-dump.txt` byte-for-byte against the exact-base MFM-07 integration dump. Do not rebaseline. Independent implementation review remains a separate required gate; an implementing worker's child/helper review does not satisfy AgentWorkflow independence.
