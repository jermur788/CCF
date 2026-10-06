# Irish height validation (Phases 4–5)

Primary source: Lekwadi et al. 2012 (S4), via CCF Primary Literature Synthesis v1 (supporting evidence, not decision authority).

## Anchors and the rounded-coefficient problem

The published age-30 **top heights** are the validation anchors: I 27.6, II 23.3, III 20.4, IV 17.4, V 16.0 m.

The paper's model form is `H(t) = b0 · (1 − exp(−b2 t))^b3`. Its rounded Table 3 coefficients do **not** reproduce the anchors (checked here):

| Class | Rounded H30 | Anchor | Difference | Rounded H100 (asymptote b0) |
|---|---|---|---|---|
| I | 25.41 | 27.6 | −2.19 | 51.8 (58.4) |
| II | 22.31 | 23.3 | −0.99 | 44.5 (49.5) |
| III | 20.32 | 20.4 | −0.08 | 33.4 (34.2) |
| IV | 16.80 | 17.4 | −0.60 | 27.9 (28.3) |
| V | 14.05 | 16.0 | −1.95 | 22.4 (22.6) |

Classes I and II also extrapolate to implausible 45–52 m asymptotes. The rounded coefficients must not be frozen into production (synthesis QA warning confirmed).

"Anchor-matched" curves in this work keep the published b2/b3 and rescale b0 so that H30 equals the anchor exactly. Class III needs almost no correction (b0 34.23 → 34.36), so it is the most trustworthy curve.

| Anchor-matched envelope | H20 | H30 | H40 | H60 | H80 | H100 | H120 |
|---|---|---|---|---|---|---|---|
| Class II | 16.2 | 23.3 | — | — | — | — | — |
| **Class III** | **14.21** | **20.40** | **24.89** | **30.14** | **32.51** | **33.56** | **34.01** |
| Class IV | 11.3 | 17.4 | — | — | — | — | — |

## Production top height against Irish classes

Top height = mean height of the 100 largest-DBH stems per hectare. Production (old model), no mortality:

| Stand age | Unthinned | Light | Moderate | Heavy/selective | Irish III envelope |
|---|---|---|---|---|---|
| 20 | 14.63 | 14.63 | 14.63 | 14.63 | 14.21 |
| 30 | **17.05** | 17.05 | 17.05 | 17.09 | **20.40** |
| 40 | 18.62 | 18.62 | 18.72 | 19.06 | 24.89 |
| 60 | 22.11 | 22.04 | 22.13 | 22.50 | 30.14 |
| 80 | 25.01 | 24.96 | 24.96 | 25.21 | 32.51 |
| 100 | 27.21 | 27.20 | 27.19 | 27.44 | 33.56 |
| 120 | 28.99 | 28.96 | 28.91 | 29.11 | 34.01 |

Verdict:
- **Wrong for the authored stand.** Production grows a Class III starting stand along a Class IV–V trajectory: 17.05 m at age 30 lies between IV (17.4) and V (16.0), and it stays 3–8 m below Class III from age 30 to 100.
- **Mean height:** the age-independent Mitscherlich form gives its largest increments to the smallest trees, so mean height converges on top height (11.7/14.6 at 20; 28.1/29.0 at 120).

## Thinning effect on top height

Weak, as the research direction requires: at most +0.44 m (+2 %) at age 40 under heavy/selective thinning, and +0.12 m at age 120. The small effect comes from which trees remain in the 100-largest-DBH set, not from a height response. **No violation.**

## Measurement notes

- **Top height:** the synthesis does not restate the paper's exact top-height definition. "100 largest-DBH stems per hectare" is used here and is flagged for checking against S4.
- **Stand area:** a 0.16 ha stand gives a top-height sample of 16 trees.
- **Recruits:** regeneration recruits are included in the stand but never enter the top-height set.
