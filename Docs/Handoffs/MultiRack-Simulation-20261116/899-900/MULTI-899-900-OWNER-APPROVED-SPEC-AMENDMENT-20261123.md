# MULTI #899 / #900 — Owner-approved spec amendment

**Status:** `APPROVED` quartets published; **implementation not started** by the orchestrator.
**Owner decision:** D-094 (2026-11-23), five locked product choices.
**Orchestrator:** opencode-go / DeepSeek V4.1 Flash — documentation and custody only; no production source edit.
**Base:** `docs/multi-900-canopy-growing-space-light-draft-spec-20261123-v1` (`b8545650`), with the #899 quartet taken from `docs/multi-899-sitka-realised-crown-model-spec-20261122-v1`.

## 1. Why the amendment was required

Both quartets were `DRAFT`, and `#900` explicitly said "NOT APPROVED FOR IMPLEMENTATION". Three requirements also contradicted the owner's locked choices:

| Draft | Owner decision |
|---|---|
| #899 "does not select a particular crown algorithm" | C2 selected with exact equations |
| #899 "no cross-area neighbours"; #900 "Area A and Area B are independent … never couple their neighbours/light" | Connected multi-rack woodland |
| #900 leaves U/V light model and grid to a later G5 | Unity proxy fixed, `b = 2.5`, 2.5 m default |
| #900 §2 requires a heatmap/inspector overlay | No overlay this pass |
| #900 Candidate U keeps `reach = R + cell/2` | Buffer decoupled from grid |

Per `Docs/Project/AgentWorkflow.md`, only `APPROVED`/`IMPLEMENTING`/`VERIFYING`/`VERIFIED` specs authorise implementation. The gate is now cleared.

## 2. Files changed (documentation only)

- `Docs/Project/decision-log.md` — new `D-094` row.
- `specs/multi-899-sitka-realised-crowns-v1/spec.md` — status APPROVED; C2 equations, year-0 equilibrium, model 0 freeze, connected-woodland boundary, provenance/bias caveat.
- `specs/multi-899-sitka-realised-crowns-v1/plan.md` — approved staged plan (engine readiness → C2 → evidence).
- `specs/multi-899-sitka-realised-crowns-v1/tasks.md` — decision tasks closed; implementation tasks open.
- `specs/multi-899-sitka-realised-crowns-v1/checklists/requirements.md` — decision gates closed; evidence gates open.
- `specs/multi-900-canopy-growing-space-light-v1/spec.md` — status APPROVED; decoupled light model, grid/buffer, metadata, per-rack output, no overlay.
- `specs/multi-900-canopy-growing-space-light-v1/plan.md` — engine-first plan.
- `specs/multi-900-canopy-growing-space-light-v1/tasks.md` — decision tasks closed; evaluator tasks open.
- `specs/multi-900-canopy-growing-space-light-v1/checklists/requirements.md` — decision gates closed; evidence gates open.

No source, test, fixture, Unity, scene, prefab, asset, save or package file was touched.

## 3. Approved numerical contract (summary)

```text
#899 CrownModel 1 (crownModelVersion = 1, area-scaled-equilibrium-c2-v1)
r_pot(DBH_cm) = max(0.1, 0.9415 + 0.07635 * DBH_cm)
r_target(CI)  = max(0.1, r_pot / sqrt(1 + CI / Ci50))
r_0           = r_target(CI_0)
r_new         = max(0.1, r + (r_target(CI) - r) * k)     k = 0.15, Ci50 = 5
CI            = Hegyi, 8 m radius, 0.5 m floor, self-excluding, connected woodland, TreeId order

#900 light (lightModelVersion = "unity-k10a10-decoupled-v1")
reach_i = r_i * 1.0 + 2.5
lateral = clamp01(1 - d/reach); vertical = clamp01(h/8)
light   = product(1 - lateral*vertical)      # index only, not calibrated PAR
grid    = 2.5 m default, 5.0 m parity oracle, 0.25 m union micro-grid
```

Model 0 remains the current formula and must reproduce exact current output.

## 4. Explicit non-claims

- Not implemented, not numerically qualified, not reviewed, not owner-accepted.
- No MA-A/B light or cover values have been computed.
- The Frenchpark evidence is one site, one age class, 12 trees per treatment and maximum-width measurements; the known +5–15 % mean-radius bias is carried, not tuned away.
- No Unity or `/multi` overlay change is authorised by this amendment.

## 5. Next step

One D-091 implementation lane on base `1e4e02215d373e224fe6a74074ae3f62983db242` (`feat/multi-885-astra-opus-ui-20261122-v1`), one writer, `Tools/MultiRackSitkaP1Lab` only, Tier 1 build evidence then Tier 2 bounded qualification and independent review.
