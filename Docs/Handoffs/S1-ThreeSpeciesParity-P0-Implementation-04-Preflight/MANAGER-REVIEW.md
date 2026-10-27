# PARITY-04-PREFLIGHT — MANAGER REVIEW

**Verdict: APPROVED — BOUNDED DOCS / VERIFICATION PATCH REQUIRED.**

Returned handoff: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Preflight/HANDOFF.md`  
Returned branch commit: `cd89b4370b860c48a9933e412439822522d6b224`  
Canonicalized on `main` via PR #16 / merge `90af680c0310df1622a37dcb31ff1267bc0a8f32`  
Claim #11: `[HANDED-OFF]`, closed.

## Accepted findings

The read-only audit found no measured or structural defect in the accepted Oak/Beech adult constants, shared asymptotic adult-height path, neutral Scenario One site seam, shared canopy/light calculation, shared 8 m Hegyi neighbourhood, or protected pure-Sitka path. The Beech height-band miss is an arithmetic consequence of the already accepted calibration and remains an accepted provisional limitation, not a new implementation defect.

The audit did identify three bounded maintenance defects:

1. stale human documentation, including `Docs/Project/unity-project-overview.md`, `Docs/BeechSpeciesParameters.md` and `Assets/ForestPrototype/Docs/SessileOakV1.md`;
2. a drifted `integrated_source_contract.json` whose normalized hashes no longer match three later-attributed source changes even though those changes do not alter the accepted Oak/Beech adult arithmetic;
3. an adult-growth verification harness whose header/coverage is stale and which reports accepted constants rather than asserting them.

No production value, arithmetic, species YAML, scene or production C# change is justified by this preflight.

## Manager decisions M-1…M-4

### M-1 — source-contract refresh

**AUTHORISED, bounded and attributed.** Refresh `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json` only for the three normalized-source drifts identified in the preflight after re-confirming their attribution on the exact patch base. The refresh must not change a protected expected value, biological threshold, species parameter, fingerprint, or pass criterion. The handoff must record old hash, new hash, introducing commit(s), and why the change is adult-arithmetic neutral.

After the refresh, `check_integrated_v1.py` must pass on the exact patch HEAD. A new mismatch discovered outside the three preflight-identified files is a stop condition and returns to the Manager.

### M-2 — canonical documentation correction

**Assigned to PARITY-04 bounded patch.** Correct the stale current-state wording in `Docs/Project/unity-project-overview.md` and the two species documentation surfaces during the same patch so the labels and values match the accepted live model. The dated historical adoption record may remain historically accurate if clearly dated and not presented as current implementation truth.

The current accepted adult calibration is:

- Oak: Pd 0.8, Ph 0.65, Dmax 120 cm, Hmax 42.5 m, Ci50 5;
- Beech: Pd 0.9, Ph 0.75, Dmax 120 cm, Hmax 42.5 m, Ci50 6.5.

These are `GAMEPLAY CALIBRATION`, not empirical Irish broadleaf growth constants. The Beech height-band miss remains an accepted provisional limitation. Scenario One species×site response remains neutral/dormant at 1.0.

### M-3 — PARITY-04 production scope

**APPROVED: zero serialized-YAML/scene and zero production-C# change.** PARITY-04 is narrowed to documentation and verification tooling only unless a later measured gate demonstrates a genuine defect and returns to the Manager. This removes the old Rack YAML/scene ownership dependency from the bounded patch.

The patch may proceed before PARITY-03 Unity qualification. Final `ADULT SPECIES PARITY REQUALIFIED` status remains gated by PARITY-03 Unity completion and the PARITY-04 runtime/offline checks on the exact final candidate.

### M-4 — FR-3 / Ci50 scope

**CONFIRMED.** Species-neutral density-victim selection and the prohibition on species `Ci50` coupling apply to the growth-model-2 mixed/species-aware density route. Models 0/1 retain their protected legacy behaviour unchanged, and model-2 pure Sitka continues through the exact protected Sitka implementation. No worker is authorised to "fix" legacy models to satisfy an over-broad reading of FR-3.

The approved checklist and bounded task file have been clarified accordingly.

## Required patch / verification

The next PARITY-04 patch must:

- correct current Oak/Beech adult documentation and evidence labels;
- update stale save-version comments where they are clearly comments/docstrings only;
- strengthen `BroadleafAdultGrowthVerification` to assert the accepted loaded constants and the neutral/shared site-canopy-Hegyi invariants identified by the preflight;
- record the accepted Beech age-50/75/100 height trajectory as diagnostic output, not a pass/fail empirical target;
- perform the authorised, attributed three-file source-contract refresh;
- run the non-Unity contract/site checks on the exact patch HEAD;
- run the focused adult Unity gate only when the machine-wide Unity lane is available and sequencing permits;
- consume PARITY-03 Unity evidence for protected pure-Sitka/model-2 density behaviour rather than duplicating or weakening those gates.

No Windows build. No asset/scene retune. No protected-anchor rebaseline.

## Status

- PARITY-04 preflight: **REVIEWED / ACCEPTED**.
- PARITY-04 bounded patch: **IMPLEMENTATION READY**.
- PARITY-04 final qualification: **WAITING ON PARITY-03 UNITY + NORMAL UNITY QUEUE**.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
