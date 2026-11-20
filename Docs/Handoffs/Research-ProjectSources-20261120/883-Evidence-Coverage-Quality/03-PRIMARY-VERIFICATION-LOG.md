# 03 — Primary verification log

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
**Method:** each extraction claim was located in the text layer of `CCF_Six_Complete_Forestry_Research_Papers.pdf`
(the 111-page companion, text-extracted to `work-text\083.txt`) and compared verbatim. **Not** re-read
end-to-end — only the consequential or contested items below.

**Method limit, stated plainly:** verification used extracted text, not page images. Text extraction preserves
numbers reliably but **destroys table row/column alignment** — and that is precisely how defect V-4 arose. A
reviewer wanting to close V-4 properly should look at the rendered page, not only the text.

## Results

| ID | Claim as extracted | Original evidence (verbatim) | Outcome |
|---|---|---|---|
| V-1 | P4 data: 5,591 records, 17 plots, 5 sites, 4-9 remeasurements, ~7 yr | "The 5591 spatial tree records (crown radii, total tree heights and tree coordinates) were collected in a total of 17 plots at each of the five sites at Aarburg, Concise, Embrach, Zofingen and Zollikon … 4-9 re-measurements at variable survey intervals with an average of 7 years." | **CONFIRMED** |
| V-2 | P5 growth data: 17,153 observations, 5,206 trees, 2 mm/yr, IQR 0.7-2.9 | "In total, the data available to modeling included 17153 growth observations (change in stem diameter between two consecutive measurements) from 5206 trees, with an absolute mean annual stem diameter growth rate of 2 mm and an interquartile range of 0.7 to 2.9 mm." | **CONFIRMED** |
| V-3 | P4 Table 1: k 1.1287, q -0.0387, kernel 0.2478/1.6060/0.0747, standardising 96.3815 | "k … 1.1287"; "q … -0.0387"; "Shot-noise parameter scaling kernel strength … 0.2478"; "… kernel range … 1.6060"; "… kernel range … 0.0747"; "Parameter used in standardizing the interaction function … 96.3815" | **CONFIRMED** |
| V-4 | P4 Table 1 grouping: "Mortality a = 0.5697, b = 9.1091"; "Recruitment c0 = 1.0200, c1 = 0.0200, d = 0.0060"; "Crown movement B = 0.0390, f = 0.0900" | Original row descriptions: "a Total tree height estimation parameter (Equation 8) … 9.1091 b"; "c0 Mortality threshold parameter (Equation 10) 1.0200"; "c1 Mortality threshold parameter (Equation 10) 0.0200"; "d Mortality threshold parameter 0.0060"; "B Annual birth intensity 0.0390"; "f Birth rate per square metre crown projection area 0.0900"; "R … shift its current location to a new one within a radius of R m. 0.4000"; "S … must be larger than S (Equation 12). 1.3000" | **EXTRACTION ERROR — groups shifted by one.** `a`/`b` are total-height parameters (Eq. 8), not mortality. `c0`/`c1`/`d` are mortality-threshold parameters (Eq. 10), not recruitment. `B`/`f` are annual birth intensity and birth rate per m² crown (recruitment), not crown movement. `R` and `S` are correctly described (max annual crown-centre shift; superorganism ratio threshold). Any reuse of the extraction's P4 parameter table must be corrected first. |
| V-5 | P2 Table 2: Ae Sitka 1,336 trees/ha, 41.9 m²/ha, dg 20.1 cm, h100 21.1 m | "Ae Sitka spruce 1336 41.9 20.1 21.1" | **CONFIRMED** |
| V-6 | P2: 36 experiments at 12 sites | "we analysed data from twelve sites including 36 experiments in Britain using different agreement indices" | **CONFIRMED** |
| V-7 | P1: 26 frame-tree selection experiments | "asked to select … frame trees in 26 experiments carried out throughout Great Britain" | **CONFIRMED** |
| V-8 | P5 mortality data: 18,423 records, 5,635 trees, 1,206 deaths | "we had 18 423 observations of tree status live or dead, originating from 5635 trees in total. Among these observations there were 1206 trees that had died, mostly as a consequence of wind and snow, followed by competition and harvesting damage" | **CONFIRMED** |
| V-9 | P5: "18 analysed stands/plots in southern Finland" | Not located as a figure this pass; only "southern Finland" as a site description | **UNVERIFIED — retain as stated, flag for reviewer** |
| V-10 | P5: 3,360 trees reaching 1.3 m; ~49 trees/ha/year | "a total of 3360 trees reaching or surpassing" (continuation not captured); ~49/ha/yr not independently recomputed | **PARTIALLY VERIFIED** |
| V-11 | #860 addendum: P2 agreement is "slight-to-fair, 36 experiments, 12 sites" and "not verified against the paper" | P2 reports poor-to-fair (crown thinning), fair-to-moderate (low thinning), overall mostly slight-to-fair, 4/36 poor, none substantial; 36 experiments, 12 sites | **PROMOTABLE:** the addendum's secondhand figure is correct and can be cited with P2 page refs (pp. 11, 16-17) |

## Why V-4 matters more than its size suggests

The extraction is otherwise careful and repeatedly warns against transferring coefficients. But its P4 table is
the one place where a reader is invited to pick up numbers, and it is wrong in a way that *looks* right: the value
count is correct, the order is correct, and the labels are shifted by one group. Anyone transcribing "mortality
a, b" or "recruitment c0, c1, d" into an offline experiment would use the wrong quantities — height-estimation
coefficients for mortality, mortality thresholds for recruitment.

Recommended correction for any future composite that carries P4: reproduce Table 1 **from the rendered page**,
label each value with its equation number, and state that only k, q and the three kernel parameters were fitted
to the Swiss data; the article says the remaining parameters were "freely determined" to produce viable
simulations.

## Items deliberately not verified

- The P3 formulas (mingling, differentiation, angle index). They are textbook definitions and are quoted with
  their worked forms; the risk of transcribing them wrongly is low but non-zero, and the reviewer should spot-check
  M_i and T_ij against P3 pages 4-5.
- P6's system taxonomy — descriptive, low numeric risk.
- The four composites' own internal figures. They are a separate evidence family and were not re-audited here;
  this packet tested only whether they cover the six-paper topics.
