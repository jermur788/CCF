# Scenario 1 site productivity — decision paper

**Status: decided (Manager continuation, 2026-10-07): Irish site Class III, implemented as growth model 1 (SitkaGrowthModel1.md).** The analysis below is retained as the decision basis.

## What the current game implies

- **Starting stand (authored):** top height 14.63 m at age 20, against anchor-matched Irish envelopes at age 20 of I 19.0, II 16.2, **III 14.2**, IV 11.3 and V 10.0 m. Only Class III is within the anchors' uncertainty (about ±3 %); the start sits 0.4 m above it, slightly toward II.
  - Supporting stand data: 2,100 stems/ha, mean DBH 15.6 cm, basal area 41 m²/ha.
  - Stand state alone cannot rule out II, because DBH and stocking are authored and not class-specific. Classifying on the whole starting state (not age-30 height alone) still points to III, with II next.
- **Production growth law:** 17.05 m top height at age 30, a **Class IV–V** pace. The current game is therefore internally inconsistent: Class III trees growing at Class IV–V speed.

## Options

| | Class I (very good) | Class II (good) | **Class III (average)** | Class IV (poor) | Class V (very poor) |
|---|---|---|---|---|---|
| Age-30 top height | 27.6 m | 23.3 m | **20.4 m** | 17.4 m | 16.0 m |
| Fit with authored start (14.6 m at 20) | No: start 4.4 m short | Partial: start 1.6 m short; re-author heights (and probably DBH) | **Yes** (+0.4 m) | No: start 3.3 m tall | No: start 4.6 m tall |
| Matches today's growth pace | No | No | No: growth must speed up about 2.6× at age 20–30 (0.63 vs 0.24 m/yr) | Closest (17.05 vs 17.4) | Close |
| Visual/growth pace in play | Very fast; dramatic over 30 years | Fast | Visible: about 6 m in the tutorial's 10 years, then slowing | Slow | Very slow |
| First-thinning timing (S4: ~15–17 years on I, ~25–29 on V) | Overdue at 20 | Due or overdue at 20 | Plausibly due near 20–25, matching the tutorial's year-1 thinning | Later (~22–27) | ~25–29: the tutorial thins early |
| CCF pacing | Fast canopy recovery; frequent interventions | Fast | Moderate; regeneration windows after thinning | Slow re-closure; longer windows | Slowest |
| H/D | Higher still | Higher (~83–89 at 30 with authored DBH) | ~83 top-tree at 30 (old 67) | Lower if growth stays slow | Lowest |
| Tutorial | Re-author start; thinning timing reads late | Re-author start | No re-authoring; first thinning stays well-timed; later harvests worth more (+13–15 % year-16/25 thinning revenue); completion objectives do not read height | Re-author start (trees too tall); growth slow | Re-author start; first thinning reads early |
| Storm (future) | Most slender | High exposure | Raised early H/D needs storm calibration with it | Stout | Stoutest |

## Recommendation

**Class III (average), via the anchor-matched Chapman-Richards envelope (candidate A-III, HeightCandidateComparison.md).**
- It is the only class consistent with the authored starting stand.
- It is the median (0.50 quantile) Irish class in S4, so it suits a first, representative scenario.
- It keeps the tutorial's first-thinning timing plausible and needs no re-authoring of the stand or tutorial.

What it changes:
- **Height growth:** the age 20–30 top-height increment rises from 0.24 to 0.63 m/yr.
- **Value and slenderness:** later timber value increases, and H/D rises noticeably before any storm work.

If the product intent is a faster, showcase site, **Class II** is the alternative. It needs the starting stand re-authored and is less defensible against the current authored state.

## Decision needed

1. Scenario One site class (recommended III).
2. Whether the site class is fixed per scenario definition (recommended: one `siteProductivity`/class per scenario, no per-cell variation yet).
3. Acceptance of the save/version consequence (SaveVersioningAssessment.md): a model-version flag is needed to keep legacy and Reference replay.
