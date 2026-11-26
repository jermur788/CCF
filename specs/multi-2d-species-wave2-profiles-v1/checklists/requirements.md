# Species Wave 2 Moisture Table and Native Profile Cards Requirements / Acceptance Checklist

- **Spec ID:** SPEC-multi-2d-species-wave2-profiles-V1
- **Spec status:** IMPLEMENTING
- **Checklist owner:** implementation sub-agent (author self-check; independent review still required)
- **Implementation under test:** `feat/multi-2d-species-wave2-profiles-20261126-v1` (head SHA in the handoff)
- **Date:** 2026-11-26

## Functional requirements

- [x] FR-1: six moisture classes — evidence: `SpeciesWave2Types.cs`; W2-02 asserts count 6.
- [x] FR-2: separate establishment/growth multipliers — evidence: `SiteResponse` record; W2-01 compares both fields.
- [x] FR-3: NeutralLegacy exactly (1.0, 1.0) — evidence: W2-01.
- [x] FR-4: 30 cells match the table — evidence: W2-01 (independent expected table in the test).
- [x] FR-5: HabitatProtectionCaution only on bog — evidence: W2-02.
- [x] FR-6: profile card fields and values — evidence: W2-03..W2-09 (W2-09: readiness records, AC-10).
- [x] FR-7: target crown formula — evidence: W2-06.
- [x] FR-8: unknown species rejected — evidence: W2-01.

## Player / user workflow

- [x] Not applicable — no player surface. No player-facing behaviour added.

## Authority and evidence

- [x] Implementation follows the spec and plan.
- [x] Evidence classifications are present for every provisional constant.
- [x] No unlabelled coefficients; open evidence questions listed in spec.

## Save / backward compatibility

Not applicable — no serialisation.

## Security / write-back

Not applicable — no I/O.

## Acceptance criteria

- [x] AC-1..AC-8, AC-10 — evidence: Core test output, `evidence/core-tests.txt` (AC-10: W2-09).
- [ ] AC-9 — see handoff gate table (filled after commit).

## Spec conformance

- [x] Deviations recorded (module-initializer registration; headless-only gate).
- [x] Unresolved items listed in spec "Open product decisions".

## Verdict

- **Spec conformance:** author self-check PASS; independent review NOT VERIFIED.
- **Implementation correctness:** author tests PASS; independent review NOT VERIFIED.
- **Notes:** browser tier NOT TESTED.
