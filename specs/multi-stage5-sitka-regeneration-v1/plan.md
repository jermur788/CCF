# Plan — MULTI Stage 5 natural Sitka regeneration (engine only)

**Status: APPROVED — owner approval 2026-11-23.** `spec.md` is `APPROVED`; the owner approved the §4 `2.5 m` area-consistent scaling policy and the binding Q1–Q6 resolutions recorded verbatim in `spec.md` (Q1 mast: one annual `NextDouble()` on `STAGE5-MAST`, `<0.15 → 1.5`, `<0.50 → 0.4`, else `1.0`; Q2: `promotionMinimumLight = 0`, no extra light gate; Q3: regeneration version appended to `AreaModelVersion` only when `!= 0`; Q4: `2.5 m / 5.0 m` ratio within `[0.75, 1.33]` across ≥20 seeds for accepted abundance and multi-year promotions, edge-block effect reported honestly; Q5: aligned `5 m` block cap, max 1 promotion per block per year, tie-breaker oldest `EstablishYear`, then tallest height, then deterministic `(Column, Row)`; Q6: `V(h)` uses pre-growth height). Production edits still require a D-091 lane receipt (Gate 0 below).

**AMENDED — owner Option A, 2026-11-23.** After the first pass failed `S5-07` on promotions, the owner approved block-cohort export and an area-proportional block budget (spec §4, Q5), and split `S5-07` into the blocking `S5-07A` (identical shared light field) and the non-blocking diagnostic `S5-07B` (spec §9 Q4). The comparison horizon is 30 years.

## Gate 0 — before any production edit (all must be true)

1. Quartet in `specs/multi-stage5-sitka-regeneration-v1/` committed and marked `APPROVED` (D-094/D-095 cited at `3238846d` until reconciled on `main`).
2. Fresh `Docs/Project/coordination/active-tasks.md` and `origin/main` read at start; no overlapping writer on `Tools/MultiRackSitkaP1Lab`.
3. Exclusive D-091 worktree/path admission recorded; branch created from `e7c1a57da519ed1ee03ccfbad240196a66df0905` (owner decision; not from `main`, not from Owner UX).
4. Baseline fingerprint recorded **before** edits: full-run hash at `1/1/1/0` plus the frozen `0/0/0`, `0/0/1`, experiment and P0 pin hashes, and the base's test count and `dotnet build` warnings/errors.

## Stages (each ends in a green build and a recorded hash)

**Stage 1 — read-only confirmation in the lab (no edits).** Confirm: `SimulationRandom` call shape; how `MultiAreaSourceToken` carries model versions (spec Q3); whether Stage 4 short-circuits an area with no living adults; how `MultiAreaStage4` orders/fences prune orders (the template for clear orders); the exact Unity `JuvenileEcologyRules` signatures and the age-band merge/accumulator code to port.

**Stage 2 — contracts, no behaviour.** Add `MultiAreaRegenerationModelVersion`, the new selection plumbing and refusal rules, empty `MultiAreaStage5State` (null on all existing roots). Prove Regeneration `0` is byte-identical on every supported prior selection.

**Stage 3 — pure rules, unit-first.** New small files under `src/Core/MultiArea/`, each one responsibility and each with tests before wiring: `Stage5Undergrowth` (T, cover update, `p_veg`), `Stage5Juvenile` (light response, growth, browse, survival), `Stage5Mast`, `Stage5SeedRain`, `Stage5Establishment` (capacity, bands, accumulator, §4 scaling), `Stage5Promotion` (block cap, placement, `CrownModel 1` radius).

**Stage 4 — annual wiring.** Insert steps 5a–12 after the existing Stage 4 step, reading start-of-year state only; commit as one root. Adult-empty racks continue. Stage 5 RNG keys isolated.

**Stage 5 — `ClearUndergrowth`.** New order type through the existing Pending/Approve/Execute fence; selective clearing; refusals atomic.

**Stage 6 — qualification `S5-01`…`S5-10`** as numerical tests in `Core.Tests` plus one disposable analysis script if needed (pure Python is enough; no numpy). Honest status words: `IMPLEMENTED`, `NUMERICALLY QUALIFIED`, `REVIEWED` are separate.

**Stage 7 — handoff.** One GitHub Markdown handoff (committed SHA, hashes before/after, per-test outcome, constants provenance, anything not verified), then independent review before owner acceptance.

## Trade-offs for the owner

- **Block cap vs fractional accumulator (spec §4/Q5).** Owner chose the aligned-5 m block cap (2026-11-23): easier to read, exactly reproduces Unity at 5.0 m, costs a documented edge-block approximation. *Amended by owner Option A (2026-11-23):* the block cap stays, a promotion also exports the block's other bands of the same cohort, and the edge-block approximation is replaced by an area-proportional integer block credit. The 5.0 m behaviour is unchanged.
- **Scaled extensive quantities vs copying Unity per cell.** Copying is simpler but inflates abundance and promotions 4× at 2.5 m; rejected.
- **Selective clearing** deviates from Unity's destructive clear. Intended, documented, and needs a Unity-side decision later if the two games are ever to be compared.

## Stop conditions

Any change to frozen hashes at Regeneration `0`; need to touch `Assets/`, `/multi` UI files or a save schema; another writer on the lab; D-091 not granted; `S5-07` cannot reach tolerance without changing a locked decision. After the Option A amendment: `S5-07A` cannot reach `[0.75, 1.33]` on the amended rule (do not widen the tolerance or add a damping constant). Stop and report the concrete fact.
