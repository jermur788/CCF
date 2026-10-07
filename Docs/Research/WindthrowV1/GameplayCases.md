# Gameplay cases

**Status:** design targets. Relative V values are from the offline prototype [PROTO] (`prototype_output.txt`). "Expected under a severe storm" describes intended *relative* behaviour, to be confirmed by calibration. These are not probabilities.

| Case | Stand state | Why it is (in)vulnerable | Proposed V (rel.) | Expected under a severe storm | What the player should understand |
|---|---|---|---|---|---|
| **Low-risk light thinning** | Age 35, a few competitors removed per crop tree, opening ≤ 1 per cell | Small recent opening; dominants keep neighbours | 3.7 for trees beside a fresh opening; about 2 elsewhere | Some extra loss beside fresh openings, below the heavy case | Light thinning still changes exposure, a little and briefly |
| **Heavy abrupt thinning** | Age 35, many trees removed this year, opening 2 per cell | Tall, slender, suddenly exposed | **6.2 (highest)** | Clearly the most damage per tree, concentrated in the opened cells | Big immediate gains carry a temporary storm risk [B] |
| **Old slender unthinned stand** | Age 30–45, never thinned, H/D 83–86 | Tall and slender, but mutually sheltered | 2.0 | Moderate scattered loss of dominants | "Never thinning" is not safe either [B] |
| **Well-spaced stable crop trees** | Crop trees released repeatedly, 10 years on (H/D ~68, opening decayed) | Stouter; old opening acclimated | 1.6 | Lower loss than slender unthinned dominants | Repeated light release builds stability over time [B] |
| **Edge trees (established)** | Road/open side, never newly exposed | Open, but grew there; no recent opening | 1.4 (+ edge factor if facing the storm) | Some loss when the storm comes from that side | Old edges are exposed but adapted [I] |
| **Interior trees** | Closed canopy, dominant | Sheltered by neighbours | 2.0 (slender dominants), 0.8 (suppressed) | Losses mainly among tall slender dominants | Height and slenderness matter more than location alone |
| **Recently opened gap** | Gap made last year (felling or windthrow) | New edge, tall trees | 5.1 | High risk at the gap edges; gaps can grow | A gap is a regeneration opportunity *and* a wind risk [B] |
| **Older stabilised gap** | Same gap 10 years later | Opening decayed; edge trees stoutened | 2.9 | Moderate | Time heals exposure |
| *Young stand at start (Year 0)* | Age 20, 14.6 m | Short | 0.6 dominant / 0.03 suppressed | Negligible loss | Early thinning is not where storms bite hardest (in this model) |
| *Old self-thinned stand (age 100)* | H/D ~50 | Stout, though tall | 1.3 | Low–moderate | Mature, stable structure |

## Design checks these cases imply (Unity calibration tests)

1. Heavy-recent > light-recent > unthinned, for the same trees and storm.
2. New gap edge > old gap edge.
3. Released crop trees fail less than slender unthinned dominants of the same height.
4. Year-0 stand: negligible damage even in an extreme storm (otherwise the first-years tutorial is disrupted for reasons the player cannot influence).
5. Edge factor active only when the storm comes from the open side.
6. **One storm never proves a treatment wrong.** Across seeds, heavy thinning has higher *expected* damage, but individual runs overlap. Report distributions, not single runs.
