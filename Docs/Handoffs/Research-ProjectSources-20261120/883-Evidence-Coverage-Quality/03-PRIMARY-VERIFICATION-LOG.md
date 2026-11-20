# 03 — Primary verification log

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
**Method:** each extraction claim was located in the text layer of `CCF_Six_Complete_Forestry_Research_Papers.pdf`
(the 111-page companion, text-extracted to `work-text\083.txt`) and compared verbatim. **Not** re-read
end-to-end — only the consequential or contested items below.

**Method limit, stated plainly:** verification used extracted text, not page images. That method is adequate for
counts embedded in running prose (sample sizes, plot counts, observation totals) and **inadequate for table or
parameter claims**. It produced exactly one false positive in the first version of this packet — the V-4 claim —
which the Manager overturned by inspecting the rendered pages. Table and parameter claims must therefore be
verified against a rendered page. This host has no PDF renderer (no poppler, mutool, Ghostscript or PyMuPDF), so
the V-4 retraction rests on the Manager's direct rendered-page inspection plus the article's own row descriptions,
not on a local render.

## Results

| ID | Claim as extracted | Original evidence (verbatim) | Outcome |
|---|---|---|---|
| V-1 | P4 data: 5,591 records, 17 plots, 5 sites, 4-9 remeasurements, ~7 yr | "The 5591 spatial tree records (crown radii, total tree heights and tree coordinates) were collected in a total of 17 plots at each of the five sites at Aarburg, Concise, Embrach, Zofingen and Zollikon … 4-9 re-measurements at variable survey intervals with an average of 7 years." | **CONFIRMED** |
| V-2 | P5 growth data: 17,153 observations, 5,206 trees, 2 mm/yr, IQR 0.7-2.9 | "In total, the data available to modeling included 17153 growth observations (change in stem diameter between two consecutive measurements) from 5206 trees, with an absolute mean annual stem diameter growth rate of 2 mm and an interquartile range of 0.7 to 2.9 mm." | **CONFIRMED** |
| V-3 | P4 Table 1: k 1.1287, q -0.0387, kernel 0.2478/1.6060/0.0747, standardising 96.3815 | "k … 1.1287"; "q … -0.0387"; "Shot-noise parameter scaling kernel strength … 0.2478"; "… kernel range … 1.6060"; "… kernel range … 0.0747"; "Parameter used in standardizing the interaction function … 96.3815" | **CONFIRMED** |
| V-4 | ~~P4 Table 1 grouping: extraction shifted by one group~~ | Rendered compendium page 66 (journal p. 6847) and rendered extraction p. 12 both group: **Height relationship** a = 0.5697, b = 9.1091 (Eq. 8); **Mortality** c0 = 1.0200, c1 = 0.0200, d = 0.0060 (Eq. 10); **Recruitment** B = 0.0390, f = 0.0900; **Crown movement** R = 0.4000 m; **Superorganism switch** S = 1.3000. The article's own row descriptions say the same. | **CLAIM WITHDRAWN — false positive.** The extraction is correctly grouped; only the plain-text layer was interleaved. P4-04 reclassified from DISPUTED to MATERIAL GAP. |
| V-5 | P2 Table 2: Ae Sitka 1,336 trees/ha, 41.9 m²/ha, dg 20.1 cm, h100 21.1 m | "Ae Sitka spruce 1336 41.9 20.1 21.1" | **CONFIRMED** |
| V-6 | P2: 36 experiments at 12 sites | "we analysed data from twelve sites including 36 experiments in Britain using different agreement indices" | **CONFIRMED** |
| V-7 | P1: 26 frame-tree selection experiments | "asked to select … frame trees in 26 experiments carried out throughout Great Britain" | **CONFIRMED** |
| V-8 | P5 mortality data: 18,423 records, 5,635 trees, 1,206 deaths | "we had 18 423 observations of tree status live or dead, originating from 5635 trees in total. Among these observations there were 1206 trees that had died, mostly as a consequence of wind and snow, followed by competition and harvesting damage" | **CONFIRMED** |
| V-9 | P5: "18 analysed stands/plots in southern Finland" | Not located as a figure this pass; only "southern Finland" as a site description | **UNVERIFIED — retain as stated, flag for reviewer** |
| V-10 | P5: 3,360 trees reaching 1.3 m; ~49 trees/ha/year | "a total of 3360 trees reaching or surpassing" (continuation not captured); ~49/ha/yr not independently recomputed | **PARTIALLY VERIFIED** |
| V-11 | #860 addendum: P2 agreement is "slight-to-fair, 36 experiments, 12 sites" and "not verified against the paper" | P2 reports poor-to-fair (crown thinning), fair-to-moderate (low thinning), overall mostly slight-to-fair, 4/36 poor, none substantial; 36 experiments, 12 sites | **PROMOTABLE:** the addendum's secondhand figure is correct and can be cited with P2 page refs (pp. 11, 16-17) |

## Retraction: V-4 was a false positive, and why that matters

The first version of this packet claimed the 24-page extraction's P4 Table 1 was shifted by one group and told
readers to correct it before reuse. **That was wrong.** Manager intake rendered compendium page 66 and extraction
page 12 and found both correctly grouped and mutually consistent, matching the article's own row descriptions:
`a`/`b` belong to the total-height relationship (Eq. 8), `c0`/`c1`/`d` to mortality thresholds (Eq. 10), `B`/`f` to
recruitment, `R` to crown movement and `S` to the superorganism rule.

The error was methodological, not arithmetic. Visiting the table in a plain-text extract on the **source** side
showed row descriptions interleaved with values; the same interleaving in the **extraction's** text layer made the
labels look displaced by one. Comparing two corrupted renderings produced a confident, wrong conclusion.

Three consequences are now built into this packet:

1. `P4-04` is reclassified from `DISPUTED` to `MATERIAL GAP` — the table is simply absent from the composites.
2. No table or parameter claim may be verified from a text layer. Prose counts are still acceptable (V-1, V-2,
   V-5–V-8 remain sound).
3. The proposed composite (`08-…`) reproduces Table 1 in the article's own grouping and carries an explicit note
   that it supersedes the earlier error, so the mistake cannot propagate into a Project attachment.

The general lesson is worth keeping beyond this task: a defect claim about a source is as fallible as the source,
and a text-extraction artefact will happily impersonate a real error in either direction.

## Items deliberately not verified

- The P3 formulas (mingling, differentiation, angle index). They are textbook definitions and are quoted with
  their worked forms; the risk of transcribing them wrongly is low but non-zero, and the reviewer should spot-check
  M_i and T_ij against P3 pages 4-5.
- P6's system taxonomy — descriptive, low numeric risk.
- The four composites' own internal figures. They are a separate evidence family and were not re-audited here;
  this packet tested only whether they cover the six-paper topics.
