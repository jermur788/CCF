# Proposed canonical decisions (for Manager integration)

These are proposals only. `Docs/Project` is unchanged on this task branch; the integrator records accepted text and regenerates ProjectContext.

**D-048 (proposed) — Scenario One site and growth model 1.**
- **Site:** Scenario One represents Irish Sitka site Class III (average; Lekwadi et al. 2012 age-30 top height 20.4 m), matching the authored starting stand (top height 14.63 m at age 20).
- **Save:** schema v17 adds `growthModel`. A missing field, v ≤ 16 and explicit 0 mean legacy growth; new Scenario One games use 1; Reference Future v1 stays 0. No migration.
- **Height (growth model 1):** Sitka height follows the Class III top-height envelope (published b2/b3; b0 derived from the age-30 anchor, not the rounded table value). Each tree keeps its relative height, with no competition term.
- **DBH and competition:** DBH is unchanged, and local Hegyi competition is retained as the only DBH competition term.
- **Density:** a separate stand-density signal, relative density on the British Sitka maximum size–density line (Comeau et al. 2010; slope −2.063, maximum SDI 1,868; [B], transferred).
- **Adult mortality:** background/self-thinning mortality is density pressure above RD 0.6 acting on suppressed trees (`0.08·P²·S⁴/mean(S⁴)`, [C]), with the maximum line as a boundary.
- **Deadwood:** biological deaths create existing fallen-deadwood records, one per tree.
- **Anchors:** growth model 1 lifecycle neutral `7E57B9DAEF5BF4D0`, normal `35E2BF1C2F55C2DE`; completion `D7C4DDD36B53FCCE`. Legacy and regeneration-model-1 anchors are retained (growth 0).

**Open / deferred:**
- Irish validation of maximum density and mortality (Forest Yield or Irish permanent plots).
- A top-height dominance rule (6–9 % drift after age 60).
- Stem form factor and volume validation.
- Windthrow using the new H/D distributions.
- The three pre-existing harness failures (Clearance, MenuTutorial, Removal).
