# European beech species parameters

`BeechSpecies.asset` is the individual-tree parameter set for European beech
(`Fagus sylvatica`) in the CCF simulation. Beech is treated as non-native but
naturalised in Ireland: it adds structural and timber diversity, not native
Atlantic woodland restoration credit.

**Current state (Scenario One three-species parity P0).** The adult growth rows
below are the Oak/Beech ordinal production calibration v1. They replace the
initial Beech values this file first recorded (potential DBH growth 0.40
cm/year, potential height growth 0.40 m/year, maximum height 40 m, `Ci50` 3.0).
Those initial values are **history, not current**, and the 40 m maximum was
never an empirical production constant.

Evidence classes. The authoritative project classes are `DIRECT`, `TRANSFER`,
`MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`,
`FALLBACK-CONSERVATIVE` and `UNSUPPORTED — NEEDS DECISION`. The coarse letters
used in asset comments and older rows are A = empirical, B = evidence-derived,
C = simulation abstraction, D = calibration; the runtime tags `[C]`/`[D]` map to
`GAMEPLAY CALIBRATION`. Rows marked "older tag" keep their original letter class
and were not re-reviewed when the adult rows were corrected.

| Parameter | Value | Class | Rationale |
| --- | ---: | --- | --- |
| Potential DBH growth | 0.9 cm/year | `GAMEPLAY CALIBRATION` | Search-grid centre of the pre-registered Oak/Beech calibration (candidate Beech-122). Not a fitted or measured Irish value. |
| Maximum DBH | 120 cm | `GAMEPLAY CALIBRATION` | Soft simulation maximum, grid centre; not a biological limit. |
| Potential height growth | 0.75 m/year | `GAMEPLAY CALIBRATION` | Grid centre. Height follows the shared asymptotic law and has no competition term. |
| Maximum height | 42.5 m | `GAMEPLAY CALIBRATION` | Simulation bound, grid centre; the law approaches it and never reaches it. Replaces the earlier 40 m figure that this table once tagged A/B. |
| Ci50 | 6.5 | `GAMEPLAY CALIBRATION` | Hegyi competition index at which DBH growth is halved. It also sets Beech's crown target and suppression diagnostics. Under growth model 2's mixed density route it is **not** used to choose which stems die. |
| Growing-space class | Intermediate (weight 1.5) | `ORDINAL REPRESENTATION` (rank), `GAMEPLAY CALIBRATION` (weight) | Weak, provisional ordering Oak High 2.0 > Beech Intermediate 1.5 > Sitka Reference 1.0. Not empirical precision. Read only by growth model 2. |
| Crown model | `ln(CPA) = 0.05 + 1.01 ln(DBH)` | A/B (older tag) | Moderately/heavily thinned Beech crown relation. |
| Crown relaxation | 0.15/year | D (older tag) | Provisional multi-year lateral crown response. |
| Reproductive onset | 40 years | A/B (older tag) | Open-grown onset; dense stands can be delayed. |
| Full reproductive age | 60 years | B/C (older tag) | Initial model ramp; suppression can delay effective reproduction. |
| Seed dispersal scale/cutoff | 6 m / 50 m | B/C (older tag) | Short local Beech seed shadow; short scale means most seed lands near the parent, but the long cutoff allows occasional longer dispersal. |
| Juvenile light response | 0, 0, 0.15, 0.35, 0.65, 1, 1 at 0, 2, 5, 10, 20, 35, 100% light | A/B/C (older tag) | Shade-tolerant persistence, but severe deep-shade stagnation; strong response from ~20% light upwards. |
| Regeneration support | enabled (v1) | Scope | Beech seed production, regeneration and promotion are active and tested. |

The asset's regeneration fields are used by the live ecology.
Mixed-species verification tests natural regeneration, shared-space capacity,
light-driven juvenile growth, promotion and persistence as well as
individual-tree competition, canopy, felling and save/load.

## Adult growth law

Beech uses the shared legacy asymptotic law, the same one Oak uses; it does not
use Sitka's Class III age/site height envelope.

- DBH growth per year = `Pd × site × (1 − DBH/Dmax) × 1/(1 + CI/Ci50)`.
- Height growth per year = `Ph × site × (1 − H/Hmax)`, with no competition term.
- `site` is exactly 1.0 in Scenario One (see below), and the competition index
  `CI` is the shared Hegyi sum over an 8 m neighbourhood.

## Accepted height-band miss

From a promoted 3.5 m stem (age about 10) this law gives Beech about 23.37 m at
age 50, 30.24 m at age 75 and 34.65 m at age 100 (arithmetic derivation from the
recurrence, not simulation output). The provisional calibration bands, 25–30 m at
age 50 and 35–40 m at age 100, are gameplay-calibration targets rather than
measurements, and Beech sits 1.63 m and 0.35 m below their lower edges (Oak sits
inside its bands). This is an **accepted provisional calibration limitation**,
not an empirical failure and not an implementation defect. It is not to be
retuned, and no new height model is to be introduced to close it, without a new
evidence/calibration packet. Runtime verification prints these heights as
diagnostics only.

## Site, canopy and competition (current)

Site interpretation. The research direction is that fresh/moist, well-aerated
mineral soil suits Beech and that drought and waterlogging are limitations. That
direction is recorded only as ordinal data in the dormant species-site
catalogue. **There is no numerical site effect:** the species×site response is
exactly 1.0 for every species and site in Scenario One P0, so **no drought or
waterlogging penalty is modelled**, and a neutral multiplier does not make
waterlogged sites favourable for Beech. Scenario One's site is a versioned
compatibility reference, not a soil survey or an "optimal" site.

Canopy and light are shared with the other species. Beech differs only through
its crown geometry, its crown relaxation rate and `Ci50` in the crown target;
no species transmissivity or deciduous/leaf-off model exists. The competition
neighbourhood is the shared 8 m Hegyi cutoff.

Beech persists below a Sitka canopy but does not thrive at arbitrarily deep shade
or spread without a Beech seed source.
