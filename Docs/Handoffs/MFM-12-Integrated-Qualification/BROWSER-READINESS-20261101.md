# MFM-12 — Integrated Qualification — Browser Readiness — 2026-11-01

**Claim:** #225  
**Parent:** #91 / browser Manager #200/#221  
**Current production integration:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Status:** **RELEASED AS NEXT MFM GATE — LOCAL START BLOCKED ON #223/#224 AND FRESH REPIN**

## Dependency status

- Rack V2 / D-064 feature scope: accepted/verified on current lineage.
- MFM-07/08/09: accepted/integrated on current lineage.
- MFM-10: Manager accepted — `SPECIES GUARD QUALIFIED`.
- PARITY-09 #219: Manager accepted — `THREE-SPECIES PARITY P0 VERIFIED`.
- MFM-11 #220: Manager accepted — `MULTI-SCALE CALIBRATION QUALIFIED`.
- #199 Combined HUD/focus: Manager approved/integrated at `3acdf9cb...`.
- S1-C / S1-D required pre-demo slices: accepted current-lineage state as recorded in live coordination; MFM-12 must verify their coexistence rather than reopening settled product design.

## Required pre-MFM-12 cleanup

Two current-lineage issues must be resolved before the MFM-12 final integrated gate can truthfully pass:

1. **#223 verification-debt repair** — stale v23 pins, history harness compile input, `MaxRemovedBasalShare` scanner false positive, and guided-v2 stale expectation reconciliation; affected timber/mixed/pruning/history/guided gates rerun.
2. **#224 Objectives-window overflow** — fresh reproduction and bounded UI repair if still present; the rendered `current objective fits` assertion remains protected.

#225 remains released as the next MFM gate, but the local qualification must not start until #200 has reviewed those returns, integrated any accepted production repair, and repinned #225 to the resulting exact production SHA.

## MFM-11 boundary carried forward

MFM-12 may rely on the accepted MFM-11 calibration decision but must preserve its limits:

- actual extension-state source-vs-Covered CI delta = 4.169829%, within accepted 20% GAMEPLAY CALIBRATION;
- post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**;
- no claim of arbitrary delayed/repeated-extension calibration robustness;
- protected compatibility reuse is scoped exact-candidate PARITY-09 evidence plus unchanged production bytes and MFM-11 save/load witness.

## Final qualification target

MFM-12 is the end-to-end verification and independent review of `MULTI-SCALE-FORESTRY-MANAGEMENT-V1`. It must cover the full requirements checklist, especially:

- authoritative individual tree/ecology state; no drifting stand-level biology;
- Rack V2-derived sample/coverage geometry;
- existing Lessons 2–5 preserved and extension after Lesson 5;
- deterministic inspectable prescription derivation;
- spacing-only candidate suggestions with player 3D confirmation;
- Sitka-adult-only quantitative prescription; Oak/Beech excluded honestly;
- proposals distinct from Fell marks;
- prescription approval distinct from Work Plan approval;
- ordinary contractor/landowner/economy path;
- durable provenance/history and safe persistence;
- Legacy40, pre-feature saves and frozen Reference Future compatibility;
- rendered map/3D review and manual alternative;
- wider-treatment calibration/completion already accepted by MFM-11 and rechecked for integration regressions;
- no hidden automation replacing player judgement.

The checklist's personal-playtest item is sequenced to MFM-13 and is not a prerequisite for declaring the candidate **ready for** that playtest.

## Release verdict

Only after exact-candidate qualification and independent review may #200 issue:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

This does not itself authorise Scenario One Tier-3 success, a Windows build, or external testing.

No Windows build. #194 remains **NO ACTION**.
