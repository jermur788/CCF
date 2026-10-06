# Regeneration funnel — BEFORE state (current production), model 1

No production correction exists, so only the BEFORE trajectory is reported. Source and validity: RuntimeReproduction.md. Data: `Evidence/funnels.csv` (cumulative from year 1; relative abundance units, not stems).

Starting stand: actual Scenario One generator, seed 20260914, browse pressure 0.2, RNG model 1. Treatments fell the given share of the smallest stems at year 0 (336 / 269 / 134 living trees).

| Treatment | Year | Seed arrival (rel.) | Requested | Accepted | Rejected | Light loss | Browse loss | Infill | Threshold loss | Exported | Exact trees | Merges into older (mean inherited age) | Standing abundance | Occupied cells |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| unthinned | 10 | 11,452 | 21.95 | 12.99 | 8.96 | 0.69 | 0.02 | 3.00 | 0.27 | 0 | 0 | 116 (3.9) | 15.01 | 28 |
| unthinned | 25 | 40,692 | 67.20 | 28.15 | 39.05 | 4.46 | 0.05 | 5.14 | 1.50 | 12.0 | 8 | 300 (8.2) | 15.28 | 37 |
| unthinned | 50 | 92,014 | 109.18 | 47.03 | 62.15 | 19.19 | 0.08 | 7.25 | 2.92 | 24.0 | 16 | 665 (19.3) | 8.09 | 27 |
| unthinned | 100 | 234,633 | 132.52 | 68.78 | 63.74 | 46.65 | 0.10 | 7.70 | 3.37 | 25.1 | 21 | 1,567 (36.9) | 1.24 | 15 |
| 20 % removed | 10 | 9,998 | 32.64 | 19.95 | 12.69 | 0.72 | 0.03 | 5.57 | 0.38 | 0 | 0 | 167 (3.9) | 24.40 | 35 |
| 20 % removed | 25 | 37,286 | 101.91 | 41.96 | 59.95 | 6.04 | 0.07 | 8.55 | 1.25 | 18.0 | 12 | 426 (7.7) | 25.15 | 41 |
| 20 % removed | 50 | 86,778 | 169.44 | 71.13 | 98.31 | 27.26 | 0.12 | 11.53 | 1.96 | 41.2 | 29 | 984 (18.0) | 12.09 | 34 |
| 20 % removed | 100 | 225,879 | 212.07 | 98.98 | 113.09 | 57.92 | 0.15 | 12.38 | 2.20 | 49.3 | 38 | 2,238 (35.0) | 1.84 | 23 |
| 60 % removed | 10 | 6,084 | 51.67 | 37.10 | 14.57 | 1.21 | 0.04 | 12.32 | 0.34 | 0 | 0 | 355 (4.0) | 47.83 | 54 |
| 60 % removed | 25 | 25,219 | 167.55 | 80.22 | 87.34 | 12.99 | 0.11 | 21.25 | 0.50 | 42.0 | 28 | 834 (7.9) | 45.87 | 54 |
| 60 % removed | 50 | 63,397 | 272.05 | 127.94 | 144.11 | 57.22 | 0.18 | 26.76 | 0.53 | 77.3 | 52 | 1,806 (18.1) | 19.47 | 53 |
| 60 % removed | 100 | 176,189 | 338.45 | 180.69 | 157.76 | 119.94 | 0.23 | 28.92 | 0.60 | 85.3 | 60 | 4,170 (38.2) | 3.59 | 46 |

Reading notes:
- **Units do not combine.** Seed arrival comes from an unnormalised dispersal kernel and must not be compared with abundance, and "exported" abundance must not be compared with exact trees; they are separate columns by design. Each exact tree consumed a median 1.5 units, i.e. a full cohort.
- **Rejection:** about half of all requested establishment is rejected for capacity at every horizon after year 10.
- **Inherited age:** the mean age inherited by merged recruits rises roughly linearly with run length (≈ 0.37 × horizon), because occupied cells keep one cohort for decades.
- **End state:** standing abundance collapses by year 100 in every treatment as canopy closes (light loss dominates). Every recruited tree created was still alive at each horizon.

Not yet produced: an AFTER trajectory (needs an approved representation), the tutorial-path run, mixed-species stands, and model-0 / Reference comparisons.

## Plan carried forward (from 70f5cc7)

Prior independent 40-year traces at 6f252e2 measured a prior implementation and are not reused as current outputs.

Durable fixture matrix pending: zero seed with no cohort; zero seed with existing cohort; high seed; saturation and overcapacity; low/high light; high browse; protection; habitat cover versus cleared cover; repeated years/mixed ages; promotion success/failure/repeat/identity/origin; save/load continuity; legacy model 0 and frozen Reference. The no-source test must separately check surviving stock and unsourced infill. Capacity tests must include request fully rejected while density/age stay coherent. *Status: the evidence harness covers zero seed (both cases), saturation/overcapacity, low/high light, habitat cover versus cleared cover, mixed-age merge, promotion compression/origin and save two-band restore, as reproductions of current behaviour. Protection, high browse, repeat/identity promotion, model 0 and Reference remain to be written as durable pass/fail fixtures once a representation is approved.*

Required gates after bounded implementation: compile/import; model 1 repeat; canonical lifecycle; browsing/planting/clearance/interaction; save hardening; tutorial/completion and economy; frozen Reference; accounting fixtures. Any unexpected Reference change stops work. No tutorial threshold/economy recalibration to hide failure.
