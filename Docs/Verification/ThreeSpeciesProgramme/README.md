# Scenario One full-demo programme — working record

Branch `claude/stoic-mccarthy-gtzaub`, base and locked context `c1564b857972ee18917e8a461c457533a54cc3f1`.
Manager packet: "CCF Overnight Master Implementation Programme — Scenario One Full Playable Demo". Worker: Claude (cloud, code-only).

**Every phase here is CLOUD / NOT UNITY-VERIFIED unless a row says otherwise.** No Unity install existed in the session; see `Phase0-Baseline.md` for exactly what could and could not be run and how each claim was established.

This folder is a working record owned by this programme. It is not a canonical project document; canonical updates required are listed at the end of each phase file and collected in the final handoff.

## Reproduce the code-only checks

```text
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod     # TYPECHECK_CLEAN = no error beyond baseline-prod.txt
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools    # same, with UNITY_EDITOR and the Tools/Verification harnesses
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all   # density 91 (+ 2 isolated compiles) + timber 71 + history 70 checks
python3 Tools/Verification/ThreeSpecies/run_unity_gates.py --dry-run                      # prints the Unity gate commands; running them needs Unity
```

Both need the .NET SDK (8+); the type-check also needs nuget.org once. Nothing is written inside the repository.

## Phase status

See the bottom table, updated as phases complete.

| Phase | Subject | State |
|---|---|---|
| 0 | Baseline qualification | Recorded (code-only); see `Phase0-Baseline.md` |
| 1 | Species-aware mixed density (growth model 2) | **Reworked twice** after qualification C (Oak/Beech used the Sitka density arithmetic) and a calibration correction (no basal-area capacity): pure-Sitka legacy route plus a dimensionless species-aware occupancy route; type-checked, offline-verified; Unity gate rewritten, not run; see `Phase1-DensityRework.md` (supersedes `Phase1-MixedDensity.md`) |
| 2 | Oak adult parity | Stopped on evidence; nothing changed; qualification gate written (not run); see `Phase2to5-BroadleafParity.md` |
| 3 | Beech adult parity | Stopped on evidence; dormant crown constants confirmed unread; see `Phase2to5-BroadleafParity.md` |
| 4 | Species × site / canopy | Stopped on a design decision (no site identity in data or docs); options S0–S3 recorded |
| 5 | Three-species qualification | Runbook prepared; needs Unity |
| 6 | Oak/Beech timber and economy | Implemented behind growth model 2; pure layer verified offline (71 checks, Sitka identical to the unmodified base); Unity wiring type-checked, gate written not run; form-grade and formative-shaping sub-features stopped; see `Phase6-BroadleafTimber.md` |
| 7–9 | Management history, repeated interventions, completion presentation | Implemented as derived history (no new persisted state, save stays v20); derivation and wording verified offline against the real record classes (70 checks); manager/UI wiring type-checked; Unity gate written, not run; see `Phase7to9-HistoryAndRepeatedInterventions.md` |
| 10 | Complete-scenario qualification | Not executable here. Record of what is and is not established, ordered Unity runbook, and a staged-gate runner (`Tools/Verification/ThreeSpecies/run_unity_gates.py`, exercised against a fake editor only); see `Phase10-Qualification.md` |
