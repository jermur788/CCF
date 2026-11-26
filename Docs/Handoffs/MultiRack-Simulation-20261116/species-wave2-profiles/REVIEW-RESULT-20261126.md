# Independent Review Result — Species Wave 2 Profiles

- **Reviewer route:** antigravity_2 / Gemini 4 Neon, effort medium, fresh thread (separate provider from the Opus builder).
- **Review date:** 2026-11-26.
- **Authority:** Manager brief 2026-11-26 (SPEC-multi-2d-species-wave2-profiles-v1).
- **Candidate reviewed:** branch `feat/multi-2d-species-wave2-profiles-20261126-v1`, head `3e98c88a9cb7b00252fa68978d08ceb8cba4d0a7`; code head `f2aceeda4e8845f65b674b2be222c65de11e3e93`; base `6d72c30538b1fbe30d3cbcf083b5b368733662b8`.
- **Verdict:** **APPROVED**.

## Checks performed by the reviewer

| Area | Result |
|---|---|
| File fence | PASS — 18 brand-new files, all inside the four allowed paths; 0 existing files modified; `git grep SpeciesWave2` shows no reference outside the fence. |
| Live wiring | PASS — no annual-loop, economy, save or UI reference. |
| Moisture table | PASS — all 6 classes, 30 exact cells, `NeutralLegacy == (1.0, 1.0)`, tier defaults and `[Calibration]` tags, bog caution only, unmultiplied response. |
| Profile cards | PASS — readiness (pine GROWFOR 10–60 y / 100–5,000 stems/ha / 10–100 m²/ha / ≥5 m; birch YC8 + pioneer→competition; alder pioneer/short-lived + heart-rot flag only), colonisation, crown formulas/anchors (max error 0.0045 m), pine clamp at 58.6 cm, juvenile positivity, `ci=0` and `ci=ci50` identities, silviculture, timber metadata, mixed evidence tags, no Sitka reuse. |
| Tests | PASS — W2-01…W2-09 genuinely assert the requirements; `[ModuleInitializer]` registers deterministically before `T.RunAll()`; Core 300/300 (floor 291). |
| Gate evidence | PASS — `gate-attempt2-summary.json` confirms head `f2aceeda`, G0–G7 PASS, G3 9/9 anchors, exit 5 (`PARTIAL`, headless), tier B `NOT RUN`; matches the handoff. |

Independently executed: `git status`, `git log`, `git diff --name-status`, `git grep`, SDK 8.0.134 solution build with `-warnaserror` (0 warnings / 0 errors), Core tests (300/300), and an independent formula evaluation of the crown anchors. The full `2d-gate.sh` was not re-run; the committed gate evidence was inspected instead.

## Findings — non-blocking

1. **[Minor] `specs/multi-2d-species-wave2-profiles-v1/tasks.md` T-03** — still says “8 author tests” and “299/299 at author time” after F1 added W2-09. Clerical; update to 9 tests / 300/300 when next touching docs.
2. **[Minor] `src/Core/SpeciesWave2/FutureSpeciesCatalog.cs` `ComputeTargetCrownRadiusM`** — `hegyiCi > 0.0` without `double.IsFinite`, so `+Infinity` returns `0.0` while `NaN` returns `r_pot`. Low risk because the engine supplies finite Hegyi CI and the catalog is immutable; optional hardening plus explicit non-finite/negative input tests.

## Residual unknowns

- Browser tier B was not run (`--headless-only`); no UI or economy code was touched.
- The base-commit NB1c failure rate was not re-measured. NB1c passed 50/50 on the accepted gate run; it is a documented pre-existing OS-scheduling race and `Phase1.Save.Tests` is unchanged here.
- Ecological constants come from the Manager brief and were not re-researched.

## Scope statement

The reviewer was read-only: no edits, commits, pushes or merges. The code reviewed at `f2aceeda` is unchanged by this record. This record is documentation only and does not authorise integration. The single integrator must still run the full safety gate before combining the branch.
