# Scenario One full-demo programme — working record

Branch `claude/stoic-mccarthy-gtzaub`, base and locked context `c1564b857972ee18917e8a461c457533a54cc3f1`.
Manager packet: "CCF Overnight Master Implementation Programme — Scenario One Full Playable Demo". Worker: Claude (cloud, code-only).

**Every phase here is CLOUD / NOT UNITY-VERIFIED unless a row says otherwise.** No Unity install existed in the session; see `Phase0-Baseline.md` for exactly what could and could not be run and how each claim was established.

This folder is a working record owned by this programme. It is not a canonical project document; canonical updates required are listed at the end of each phase file and collected in the final handoff.

## Reproduce the code-only checks

```text
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod     # TYPECHECK_CLEAN = no error beyond baseline-prod.txt
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools    # same, with UNITY_EDITOR and the Tools/Verification harnesses
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py  # THREE_SPECIES_OFFLINE_PASS
```

Both need the .NET SDK (8+); the type-check also needs nuget.org once. Nothing is written inside the repository.

## Phase status

See the bottom table, updated as phases complete.

| Phase | Subject | State |
|---|---|---|
| 0 | Baseline qualification | Recorded (code-only); see `Phase0-Baseline.md` |
| 1 | Species-aware mixed density (growth model 2) | Implemented, type-checked, offline-verified; Unity gate written, not run; see `Phase1-MixedDensity.md` |
| 2 | Oak adult parity | Stopped on evidence; nothing changed; qualification gate written (not run); see `Phase2to5-BroadleafParity.md` |
| 3 | Beech adult parity | Stopped on evidence; dormant crown constants confirmed unread; see `Phase2to5-BroadleafParity.md` |
| 4 | Species × site / canopy | Stopped on a design decision (no site identity in data or docs); options S0–S3 recorded |
| 5 | Three-species qualification | Runbook prepared; needs Unity |
