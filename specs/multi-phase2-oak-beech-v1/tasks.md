# Tasks — MULTI Phase 2 Oak and Beech (DRAFT)

**Status: DRAFT — awaiting owner approval (2026-11-24).** Tasks are implementation units, not new product decisions. None from T1 onward may be dispatched until the quartet is `APPROVED`. Boxes are ticked only from observed evidence.

- **Spec ID:** SPEC-multi-phase2-oak-beech-V1
- **Task list owner:** Overall Manager
- **Last updated:** 2026-11-24

## Step A (this quartet)

- [x] T0a Owner locked decisions 1–6 (6/6 Option A) and the `5b757d9b` base (2026-11-24).
- [x] T0b Read-only pre-flight audit published (`Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/PHASE2-PREFLIGHT-AUDIT-20261124.md`).
- [x] T0c Draft quartet published for owner review.
- [ ] T0d **Owner approves the quartet and answers spec §14 Q1–Q6; status changed to APPROVED.**

## Step B (NOT AUTHORISED until T0d)

| Task | Goal | Acceptance | Depends on |
|---|---|---|---|
| T1 | Gate 0: coordination refresh, D-091 admission, branch, baseline anchors and test count | plan Gate 0 | T0d |
| T2 | Stage 1 read-only confirmations (Comeau subset, band limit, patch year guard, `InstalledYear`, RNG shape) | recorded in handoff | T1 |
| T3 | `SpeciesModelVersion`, fifth field, allow-list, refusals, null `MultiAreaPhase2State`, token rule; **pin `c4b670ed…` (declared test change, R8)** | **P2-01** | T2 |
| T4 | Oak/Beech adult growth, linear/power-law `r_pot`, C2 relaxation | **P2-02** | T3 |
| T5 | Two-part mixed density, generic keys, BA budget, species-aware pruning, deadwood `SpeciesId` | **P2-03** | T4 |
| T6 | `Plant`/`Shelter`/`SpotClear` orders, fence, validation and refusals | **P2-04** | T3 |
| T7 | Shelter port (binary access, strict-`<` expiry, unset `FailedYear`), broadleaf browse | **P2-05** | T6 |
| T8 | Circular spot patches, recovery, selective behaviour | **P2-06** | T6 |
| T9 | Planted-juvenile annual step and `PL-*` promotion (block-cap exempt, Oak 0.2 gate, C2 radius) | **P2-07** | T4, T7, T8 |
| T10 | Per-species mast streams, cross-rack seed rain, adult-empty continuation | **P2-08** | T4 |
| T11 | Shared natural capacity, 1-total 5 m block cap and tie-break | **P2-09** | T10 |
| T12 | Test-only `MA-1-MIXED-v1` fixture (Q5); 30-year two-rack determinism, rack-order invariance, idempotent replay | **P2-10** | T5, T9, T11 |
| T13 | Full regression: all Stage 1–5 tests unchanged and green; build `--disable-build-servers`, external `TMPDIR`, 0 warnings / 0 errors | P2-01 re-run | T12 |
| T14 | GitHub handoff with honest status words, anchors before/after, per-test outcome, what was not verified | — | T13 |
| T15 | Independent review (different model/session), then owner acceptance | — | T14 |

## Acceptance scenarios (all ten must be enumerated and evidenced)

- **P2-01** Frozen byte-identity for `0/0/0/0`, `0/0/1/0`, `1/1/1/0`, `1/1/1/1` (Species Off) plus the P0 pin.
- **P2-02** Oak/Beech adult DBH+height and Beech power-law C2 crowns.
- **P2-03** Two-part mixed density; pure-Sitka Comeau parity; `Ci50 = 6.5` growth vs `Ci50 = 5` victim neutrality; `0.125` BA cap.
- **P2-04** Fenced `Plant`/`Shelter`/`SpotClear` commitment gate plus spacing/polygon/duplicate refusal.
- **P2-05** Protected vs unprotected Oak/Beech browsing; verified shelter expiry; `0` off-switch.
- **P2-06** ~1 m² circular spot clearance vs unweeded vs whole-cell `ClearUndergrowth`, sparing natural seedlings.
- **P2-07** `PL-*` promotion at 3.5 m bypassing the natural 5 m block cap, plus the Oak `0.2` light gate and C2 initial crown.
- **P2-08** Cross-rack Oak/Beech seed rain, independent mast streams, adult-empty rack continuation.
- **P2-09** Shared mixed natural capacity plus the 1-total 5 m block cap and its tie-break.
- **P2-10** 30-year two-rack mixed determinism, rack-order invariance, idempotent replay.

Detailed pass criteria: `checklists/requirements.md`.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T0a–T0c | DONE | design sub-agent | — |
| T0d | TODO | owner | T0c |
| T1–T15 | BLOCKED (Step B not authorised) | TBD | T0d |
