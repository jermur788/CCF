# 03 — Primary verification log

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
**Method, stated plainly.** This producer's host has **no PDF renderer** (no poppler, mutool, Ghostscript or
PyMuPDF). All producer checks below were made against the **text layer** of the 111-page companion
(`work-text\083.txt`), which is reliable for counts embedded in prose and unreliable for tables. Where a value was
later confirmed on a rendered page, that was done by the **independent #896 reviewer** and is attributed as such.
**This producer claims no personal rendered-page verification.** Pages are given as paper page (`P5:4`) with the
collection page (`c.88`) where the review supplies it.

## Results

| ID | Claim | Page | Evidence | Outcome |
|---|---|---|---|---|
| V-1 | P4 data: 5,591 records, 17 plots, five sites, 4–9 remeasurements, ~7 yr | P4:12 / c.66 | "The 5591 spatial tree records … were collected in a total of 17 plots **at each of** the five sites at Aarburg, Concise, Embrach, Zofingen and Zollikon … 4-9 re-measurements … average of 7 years." | **CONFIRMED as quoted; interpretation UNRESOLVED.** 17-in-total vs 17-per-site is ambiguous in the source. Do not state "17 plots in total" as fact. |
| V-2 | P5 growth data: 17,153 observations, 5,206 trees, 2 mm/yr, IQR 0.7–2.9 | P5:4 / c.88 | "…17153 growth observations … from 5206 trees, with an absolute mean annual stem diameter growth rate of 2 mm and an interquartile range of 0.7 to 2.9 mm." | **CONFIRMED** (producer text layer; #896 rendered) |
| V-3 | P4 Table 1 numerical values | P4:12 / c.66 | All 15 values (k, q, α, β, δ, ν, a, b, c₀, c₁, d, λ_B, f, R, S) | **CONFIRMED** (producer text layer; #896 rendered all 15) |
| V-4 | ~~P4 Table 1 grouping shifted by one group~~ | P4:12 / c.66 and extraction p. 12 | Rendered by #896: Height relationship a = 0.5697, b = 9.1091 (Eq. 8); Mortality c₀ = 1.0200, c₁ = 0.0200 (Eq. 10) and d = 0.0060 (no equation cited); Recruitment **λ_B** = 0.0390, f = 0.0900; Crown movement R = 0.4000 m; Superorganism switch S = 1.3000 | **CLAIM WITHDRAWN — false positive.** The extraction is correctly grouped. Two fidelity fixes remain: the recruitment symbol is **λ_B**, not B, and "(Eq. 10)" covers c₀/c₁ only. |
| V-5 | P2 Table 2: Ae Sitka 1,336 trees/ha, 41.9 m²/ha, dg 20.1 cm, h100 21.1 m | P2:6 / c.20 | "Ae Sitka spruce 1336 41.9 20.1 21.1" | **CONFIRMED.** Cross-paper discrepancy: P1 Table 1 (P1:7 / c.9) gives Ae 1,321 trees/ha, h₁₀₀ 21.2 m, n = 176, and states the Ae marteloscope was 0.133 ha. Different year/area; neither is "the" Ae inventory. |
| V-6 | P2: 36 experiments at 12 sites | P2:4 / c.18; P2:6 / c.20 | "twelve sites including 36 experiments in Britain" | **CONFIRMED** |
| V-7 | P1: 26 frame-tree selection experiments | P1:2 / c.4; P1:7 / c.9 | "frame trees in 26 experiments carried out throughout Great Britain" | **CONFIRMED.** Raters 6–20; trees rated 83–323 (P1 Table 1). |
| V-8 | P5 mortality data: 18,423 records, 5,635 trees, 1,206 deaths | P5:4 / c.88 | "18 423 observations of tree status live or dead, originating from 5635 trees in total … 1206 trees that had died, mostly as a consequence of wind and snow, followed by competition and harvesting damage" | **CONFIRMED.** Note: causes are attributed for *some* deaths only — "more than half the causes of death remained unknown" (P5:11 / c.95). |
| V-9 | P5 sample size "18 plots/stands" | P5 abstract / c.85; P5:3 / c.87; P5:4 / c.88 | Abstract: "a data set involving 18 plots". §2.1: "18 Norway spruce… dominated stands in five areas (2–7 stands in each)". P5:4: "1732 trees were cut in the 18 plots altogether". **But** P5:3: "The site index of the **20 plots** varied between 25.9 m and 33 m"; and "selective thinning was repeated in 16 stands in 1996. Four stands were not thinned" (16 + 4 = 20). | **CONFIRMED as stated; source internally inconsistent.** 18 is what the paper says in three places, 20 in one. **Not reconciled.** Never cite 18 as a settled sample size. |
| V-10 | P5 ingrowth: 3,360 trees; rate | P5:4 / c.88 | "3360 trees reaching or surpassing a height of 1.3 m … on average 40 ingrowth trees per measurement time and sample plot or **49 ingrowth trees per hectare and year**" | **CONFIRMED.** The 49 ha⁻¹ yr⁻¹ figure is directly reported, not derived by this packet. Consistency check: 40 per 0.16 ha plot per five-year interval ≈ 50 ha⁻¹ yr⁻¹. |
| V-11 | #860 addendum's P2 agreement claim | P2 abstract / c.15; P2:11 / c.25; P2:14 / c.28; P2:16 Fig. 7 / c.30; P2:17 / c.31; Table 3 P2:9 / c.23 | Abstract: "general agreement in tree selection in Britain was only slight to fair". P2:14: low thinning κ_F "generally between 0.35 and 0.51 (fair to moderate)"; crown thinning "between 0.01 and 0.28 (poor to slight … with the notable exception of experiment Loch Ard 2015, here κ_F = 0.44)". Fig. 7: poor 4, slight 14, fair 17, moderate 1 = 36. | **CONFIRMED with page corrections.** The addendum's "slight-to-fair" matches the abstract; crown thinning was poor-to-slight with one fair exception. Verified by this producer (text layer) and by #896 (rendered). |

## Retraction note: V-4 was a false positive

The first version of this packet claimed the extraction's P4 Table 1 was group-shifted and advised correcting it
before reuse. That was wrong. The error was methodological: visiting the table as plain text on the **source** side
showed row descriptions interleaved with values, and the same interleaving in the **extraction's** text layer made
the labels look displaced. Comparing two corrupted renderings produced a confident, wrong answer.

Consequences now built into the packet:

1. `P4-04` is `MATERIAL GAP`, not `DISPUTED` — the table is simply absent from the composites.
2. No table or parameter claim is verified from a text layer. Prose counts (V-2, V-5–V-8) remain acceptable and
   were independently rendered by #896.
3. The proposed composite reproduces Table 1 in the article's own grouping, with **λ_B** and the equation scope
   fixed, so the earlier mistake cannot propagate into a Project attachment.

## Items deliberately not verified by this producer

- The P3 formulas (mingling, differentiation, angle index): textbook definitions quoted in their worked forms.
  #896 checked M_i = equation (5), n = 3, values 0/0.33/0.67/1.00 and the T_ij classes on P3:5.
- P6's system taxonomy — descriptive, low numeric risk. #896 read P6:6/P6:9 for the Sitka mentions.
- The four composites' own internal figures: a separate evidence family, not re-audited here.
