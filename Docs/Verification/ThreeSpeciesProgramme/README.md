# Scenario One full-demo programme — working record

> **Historical working record.** This folder is non-canonical and predates the approved Scenario One Three-Species Parity P0 specification and later PARITY-03 qualification. Read `PARITY-03-Q11-EvidenceRecordCorrections.md` before using density/default-model/evidence-label claims from these files. In particular, old statements that growth model 2 is not the new-game default are superseded, old Unity measurements remain historical evidence for their exact tested lineage only, and legacy evidence tags such as `[B]`, `[C]`, `[D]`, `[ORD]` must be interpreted through FR-12 rather than as current project evidence classes.

Branch `claude/stoic-mccarthy-gtzaub`, base and locked context `c1564b857972ee18917e8a461c457533a54cc3f1`.
Manager packet: "CCF Overnight Master Implementation Programme — Scenario One Full Playable Demo". Worker: Claude (cloud, code-only).

**Every phase here is CLOUD / NOT UNITY-VERIFIED unless a row says otherwise.** No Unity install existed in the session; see `Phase0-Baseline.md` for exactly what could and could not be run and how each claim was established.

This folder is a working record owned by this programme. It is not a canonical project document; canonical updates required are listed at the end of each phase file and collected in the final handoff.

## Reproduce the code-only checks

The commands remain useful for reproducing the historical programme checks, but the check-count comments are snapshots of that earlier suite and are **not current qualification totals**. Use current handoffs and current tool output for present acceptance counts.

```text
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod     # historical TYPECHECK_CLEAN contract
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools    # historical tools/typecheck contract
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all   # historical snapshot: density 35 + timber 71 + history 70
```

Both need the .NET SDK (8+); the type-check also needs nuget.org once. Nothing is written inside the repository.

## Phase status

The table below records the state of the original programme at the time it was written; it is **not the current Scenario One milestone/status ledger**.

| Phase | Subject | State |
|---|---|---|
| 0 | Baseline qualification | Recorded (code-only); see `Phase0-Baseline.md` |
| 1 | Species-aware mixed density (growth model 2) | Historical programme state: implemented, type-checked, offline-verified; Unity gate written, not run; see `Phase1-MixedDensity.md` and the later Q11 correction note |
| 2 | Oak adult parity | Stopped on evidence; nothing changed; qualification gate written (not run); see `Phase2to5-BroadleafParity.md` |
| 3 | Beech adult parity | Stopped on evidence; dormant crown constants confirmed unread; see `Phase2to5-BroadleafParity.md` |
| 4 | Species × site / canopy | Stopped on a design decision (no site identity in data or docs); options S0–S3 recorded |
| 5 | Three-species qualification | Historical programme state: runbook prepared; later parity qualification is recorded in the canonical PARITY handoffs |
| 6 | Oak/Beech timber and economy | Historical programme state: implemented behind growth model 2; pure layer verified offline (71 checks, Sitka identical to the unmodified base); Unity wiring type-checked, gate written not run; form-grade and formative-shaping sub-features stopped; see `Phase6-BroadleafTimber.md` |
| 7–9 | Management history, repeated interventions, completion presentation | Historical programme state: implemented as derived history (no new persisted state, save stays v20); derivation and wording verified offline against the then-current record classes (70 checks); manager/UI wiring type-checked; Unity gate written, not run; see `Phase7to9-HistoryAndRepeatedInterventions.md` |

## Follow-up: density v3 canopy pulses (`fix/density-v3-canopy-pulses-v1`)

Root cause, fix, sweep and the focused Unity gate for 25-37 % one-year basal-area losses in dense broadleaf stands: `CanopyPulseRootCause.md`; raw scan tables in `CanopyPulseEvidence/`. Treat these as lineage-specific historical evidence unless a later accepted handoff explicitly adopts them for a current base.
