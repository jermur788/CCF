# CCF #896 — Independent Stage D scientific, coverage and Stage C review of #883

**Status:** `INDEPENDENTLY REVIEWED — NEEDS CHANGES`. Research-document review only. **Not** canonical publication, **not** scientific calibration approval, **not** a gameplay, attachment, Drive, composite or source-edit authorisation.
**Date:** 2026-11-21. **Issue:** [#896](https://github.com/jermur788/CCF/issues/896). **Parent:** [#883](https://github.com/jermur788/CCF/issues/883). **Decision owner:** [#425](https://github.com/jermur788/CCF/issues/425).
**Producer commit reviewed (pinned):** `053686b590098b1abbebc6d6df7c27585956ca80` on `research/883-condensed-evidence-coverage-review-20261120-v1` (base `1511df3b6f2e7cdb8cf953c21d530947286ac280`, 13 files under `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/`).

Citation convention used below: `[P2:16]` = paper P2, **paper-relative** page 16 (the convention `08` uses). `c.30` = page 30 of the 111-page collection PDF (P1 c.3–14, P2 c.15–34, P3 c.35–54, P4 c.55–84, P5 c.85–98, P6 c.99–111; I confirmed each paper's first page lands on those boundaries). `e.12` = page 12 of the 24-page extraction.

---

## 0. Required verdict block

```text
OVERALL VERDICT: NEEDS CHANGES

(i)  Source traceability:    NEEDS CHANGES — every sampled page citation resolved to the right paper and page;
                             no invented or wrong-paper citation found. Defects: the TSV column "original_pdf_page"
                             holds paper-relative pages (P2 Table 2 = p.6 = collection c.20), 03 gives no page for
                             V-1..V-8, two matrix rows carry an unresolved "Growth §?" location and three others
                             point at the wrong Growth section (h/d is §6, Hegyi is §5), P4 "freely determined" is
                             cited p.12 but is on p.13, and 09's "MART pp.2,10 = rights only" is wrong (MART PDF p.10
                             is the marking-agreement claim itself).
(ii) Scientific quantities:  NEEDS CHANGES — all numeric values in V-1..V-8 and the whole P4 Table 1 are CONFIRMED on
                             rendered pages. No numeric transcription error found. Substantive errors are in
                             interpretation/labels: "cause of every observed death is unknown" is false (source: more
                             than half unknown); V-9 and V-10 are mis-labelled as unsettled (both are stated by the
                             source, and "18 plots" conflicts with "the 20 plots" inside the same paper); P2's
                             structure finding is reported without its direction; P4 recruitment symbol is lambda_B
                             not B.
(iii) Coverage completeness: NEEDS CHANGES — the bulk verdict SURVIVES my independent concept- and number-sweeps:
                             none of the four composites holds the P2 agreement evidence, P3 indices, P4 model,
                             P5 Bayesian model or P6 taxonomy under any wording I could find. But sampled statuses
                             change: P6-09 (COVERED is unsupported), P5-09 (priority/rationale overstated),
                             P2-09 (not a P2 claim; its "home" is not a composite), P2-01/P2-04 (stale DISPUTED),
                             P1-01 (strength understated). 67 rows is not exhaustive (one omitted Sitka-relevant item
                             found in P6).
(iv) Practical suitability:  NEEDS CHANGES — 08 is a coherent, non-duplicative, page-traced condensation and the
                             single-composite shape is sound. It must lose design-handling advice that sits inside
                             an evidence document, one unsupported recruitment assertion and several overstated
                             lines before it is fit for Manager consideration. 09's narrow P2 edit is accurate in
                             substance and narrow in scope but needs four exact corrections.

V-4 RENDERED: confirmed retraction

SAMPLE ACCOUNTING: see §7. Claims: 77 assertions audited from 08 §§2-8: 45 verified on a rendered page,
                   18 on text-layer prose only, 5 found defective without a clean verification path, 8 NOT checked,
                   1 guardrail sentence (not a source claim). 20 of the 77 carry a defect.
                   Matrix rows: 48 of 67 touched (34 of 44 gaps, 9 of 13 cross-references, 2 of 6 covered, 3 of 3
                   disputed); 10 re-derived from scratch. Original pages: 21 collection pages and extraction p.12
                   rendered and visually inspected; other pages read via the text layer for prose only.

REQUIRED CORRECTIONS: §8 (28 numbered items, each with file + exact change; 14 blocking)
OPTIONAL IMPROVEMENTS: §9
RESIDUAL UNKNOWN: §10
NEXT HANDOFF: producer correction, then a delta review of the corrected SHA restricted to §8 items
              (not a re-review of unchanged material). Manager #425 decision packet only after that closure.
```

An `APPROVED` outcome was **not** available: although no numeric error was found, there are factual errors (death causes), misreported findings (P2 structure), unsupported status claims (P6-09) and scope creep (08 §5 handling column). None of these needs new research; all are correctable from the pages cited below.

---

## 1. Reviewer identity, independence, access and method

| Item | Record |
|---|---|
| Reviewer / provider | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code in T3 Code, on the owner's Windows workstation. **Not** the GPT-6 High chat the packet suggests. The packet permits "a qualified independent research model with PDF rendering"; this substitution is disclosed, not hidden. |
| Independence | Fresh session with no prior involvement in #883. Not the #883 producer (DeepSeek V4.1 Flash), not a child/helper of it, not the Manager who performed the B1–B3 intake. The local checkout's `git config user.name` reads `DeepSeek V4.1 Flash`; I did **not** commit locally, to avoid mis-attributing authorship (the review is published through the GitHub API; reviewer identity is in this document and in the #896 claim). |
| Claim | [#896 comment 7060408254](https://github.com/jermur788/CCF/issues/896#issuecomment-7060408254). **Timing disclosure:** it was registered after most read-only audit work, because GitHub write access was only confirmed then. Nothing had been written to GitHub or the repository before it. Before posting I checked that live `active-tasks.md` (main `cfc802d2…`) has no #896 row and no owner of this path, that #896 had only the dispatch comment, and that no branch or path for this review existed. |
| Base | **Review branch base = latest observed `main` `56e1cae49f44953a87d18c181a4ac8052b69aadf`** (2026-11-21T09:22:17Z), observed immediately before publishing. That is one commit ahead of the `cfc802d26d12f3578723bec9c94a8ce165ae7124` (2026-11-20T22:48:55Z) main that I used when checking live coordination state and when posting the claim; the extra commit only edits `Docs/Project/coordination/active-tasks.md` to register the unrelated Multi #897 lane (which expressly leaves #883, #896 and the four composites untouched). After it landed I re-checked that #896 has only the dispatch and my claim, and that the review branch and path still do not exist. |
| Packet | `docs/ccf-896-883-independent-stage-d-review-dispatch-20261121-v1` @ `4a761532239397552730e08561e1fede787dac20`. |
| Rendering tool and method | Built-in **Windows.Data.Pdf** renderer driven from PowerShell, read-only on the originals, 1.6×–2.2× scale PNG output to a scratch folder outside the repository; I then **read each page image**. Text layers (`pdftotext -layout`) were used only to *locate* pages and to cross-compare digits. **No table, equation, figure or parameter value was accepted from a text layer.** Where I compared image-read numbers with the text-layer strings they agreed; I did not compare every digit systematically. I did not reproduce the producer's column-interleaving artefact. |
| Originals used | Owner's local files `CCF_Six_Complete_Forestry_Research_Papers.pdf` (17,650,706 B, 111 pp, SHA-256 `4f1d75578d25e8dd9621474ee98beb26afb8890e87106e19ac68c69336cc5e13`) and `CCF_Six_Papers_Relevance_Evidence_Extraction.pdf` (131,279 B, 24 pp, `f8a699d96d5146d05f74ec1c5b2b2ba2206986665b0019472c61a3ba3d29f8bf`). Both **match the hashes in the producer's inventory**. **Not verified:** that the ChatGPT Project's attached copies are byte-identical (I could not open the Project). |
| Four composites | Archive copies `…/860-drive-ccfgame-20261120/project-attachments-copy-20261120/`: Ecology 37,540 B `85b23b2b…`; Browsing 27,209 B `004756b6…`; **Growth 33,058 B `12ddc2b1…` (STALE)**; Practice 34,910 B `cb1c2421…` — sizes and hashes match the producer inventory and #860 roster. Also read the **canonical Git Growth** (`Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`, 35,555 B, `2704b0aa689f3120ff0549377b973b2c190e2740ea9828ade88481741f29f73b`) and swept **both** Growth versions (identical counts for every probe except one: "irregular" 1 in the attachment copy vs 2 in the canonical copy). The Project attachment slots themselves were not opened. |
| #860 addendum | Read the **branch copy** `docs/860-desktop-drive-consolidation-addendum-20261120-v1` @ `d90d1fe26503bf503ba3f8417bf3658cc29fe4fc` (12,275 B, `4077d2d1…`) **and** the desktop archive copy (11,934 B, `b0afc312…`). They differ only by one added `DRAFT / SECONDHAND…` banner line. Not just the quote in 09. |
| Producer files hashed | 08 `eca381be…`; 09 `ce59840f…`; 02 TSV `a7ae6b85…`; 03 `65395ce6…`; 07 `43535dd1…` (from the pinned commit). |
| MART report (secondhand source cited by 09) | `Deep Research — Marteloscope datasets…pdf`, 200,615 B, `7265983e…`; opened at the cited pages. |
| Still inaccessible | Project attachment bytes; GitHub issue state beyond the comments listed; original author lists/DOIs for P1/P3/P4/P6 were not independently cross-checked beyond the header lines noted in §7. |

---

## 2. Rendered-page oracle: P4 Table 1 and the V-4 retraction

**Rendered and inspected:** collection **c.66** (journal p.6847, paper page 12, containing "Table 1. Main parameters of the CrownShotNoise model, their descriptions and values as used in this study") and extraction **e.12** ("Published parameters — preserve provenance, do not adopt").

The article table is a single ordered list of 15 rows (k, q, α, β, δ, ν, a, b, c₀, c₁, d, λ_B, f, R, S); the extraction regroups them. Value-by-value comparison, both read from images:

| Group in e.12 | Symbol = value in e.12 | Article row(s) c.66 | Match |
|---|---|---|---|
| Crown potential | k = 1.1287, q = −0.0387 | k "Parameter of the power function… Equation (2)" 1.1287; q −0.0387 | ✓ |
| Interaction kernel | α = 0.2478, β = 1.6060, δ = 0.0747 | α "kernel strength (Eq. 3)" 0.2478; β 1.6060; δ 0.0747 ("kernel range") | ✓ |
| Interaction scaling | ν = 96.3815 | ν "standardizing the interaction function (Eq. 5)" 96.3815 | ✓ |
| Height relationship | a = 0.5697, b = 9.1091 | a, b "Total tree height estimation parameter (Eq. 8)" | ✓ |
| Mortality | c0 = 1.0200, c1 = 0.0200, d = 0.0060 | c₀, c₁ "Mortality threshold parameter (Eq. 10)" 1.0200, 0.0200; d "Mortality threshold parameter" 0.0060 | ✓ |
| Recruitment | λB = 0.0390, f = 0.0900 | λ_B "Annual birth intensity" 0.0390; f "Birth rate per square metre crown projection area" 0.0900 | ✓ |
| Crown movement | R = 0.4000 m | R "shift… within a radius of R m" 0.4000 | ✓ |
| Superorganism switch | S = 1.3000 | S "…must be larger than S (Equation 12)" 1.3000 | ✓ |

All 15 values present, none shifted, none dropped; the extraction's descriptions agree with the article's own row text.

**V-4 RENDERED: confirmed retraction.** The extraction's P4 Table 1 grouping is correct. The producer's second position (retraction) is right; the first ("shifted by one group") was wrong. Producer 08's Table 1 grouping, as a *grouping*, matches both rendered pages (see R-4/R-5 for two small fidelity corrections inside it: the recruitment symbol and one page citation).

One further rendered confirmation that the "fitted vs freely chosen" note in 08 is right: c.67 (journal p.6848) says *k* and *q* were fitted by upper-quantile regression at τ = 0.975; α, β, δ by nonlinear spatial regression (least squares and maximum likelihood); "**All other model parameters were freely determined** in such a way that the model would simulate continuous forest development…". 08's sentence is correct but cites `[P4:12]`; it is on **p.13**.

---

## 3. V-1 to V-11 against rendered pages

"R" = value read on a rendered page image. Producer's 03 gives verbatim quotes but **no page numbers**; the pages below are mine and should be added to 03 (R-24).

| ID | Claim | Rendered page (collection / paper) | Result |
|---|---|---|---|
| V-1 | P4 5,591 records, 17 plots, 5 sites, 4–9 remeasurements, ~7 yr | c.66 / P4:12 | **CONFIRMED as quoted.** But the sentence is "a total of 17 plots **at each of** the five sites" — ambiguous between 17 in total and 17 per site. 08/extraction adopt "17 plots at five sites". **Interpretation UNRESOLVED** (R-20). |
| V-2 | P5 17,153 obs, 5,206 trees, 2 mm/yr, IQR 0.7–2.9 | c.88 / P5:4 | **CONFIRMED (R).** |
| V-3 | P4 Table 1 numbers | c.66 | **CONFIRMED (R)** — all 15 values, §2. |
| V-4 | retraction | c.66 + e.12 | **CONFIRMED retraction (R).** |
| V-5 | P2 Table 2 Ae 1,336 / 41.9 / 20.1 / 21.1 | c.20 / P2:6 | **CONFIRMED (R)**: Ae, Sitka spruce, N 1336, G 41.9, d_g 20.1, h₁₀₀ 21.1. **Cross-paper discrepancy:** P1 Table 1 (c.9 / P1:7) lists Ae at **1321** trees/ha, G 41.9, d_g 20.1, **h₁₀₀ 21.2**, n = 176, and says the Ae marteloscope was **0.133 ha** (others 0.1 ha). Different year/plot area; 08 must not present 1,336/21.1 as *the* Ae inventory (R-7). |
| V-6 | P2 36 experiments at 12 sites | c.18 / P2:4 and c.20 / P2:6 | **CONFIRMED (R).** |
| V-7 | P1 26 frame-tree experiments | c.4 / P1:2; c.9 / P1:7 | **CONFIRMED (R).** Counting the *r* entries in P1 Table 1 gives 26. Raters 6 (Cannock Chase, Crychan) to 20 (Cannock Chase); trees rated 83 (Peckett Stone) to 323 (Tummel) — also confirmed. |
| V-8 | P5 18,423 obs, 5,635 trees, 1,206 deaths | c.88 / P5:4 | **CONFIRMED (R).** |
| V-9 | P5 "18 analysed stands/plots" — "UNVERIFIED" | c.85 abstract; c.87 / P5:3; c.88 / P5:4 | **Mis-labelled — the source states it, but the source is internally inconsistent.** Abstract: "a data set involving 18 plots". §2.1: "data from 18 Norway spruce… dominated stands in five areas (2–7 stands in each)". P5:4: "1732 trees were cut in the 18 plots altogether". **But** P5:3 also says "The site index of the **20 plots** varied between 25.9 m and 33 m", and "selective thinning was repeated in 16 stands in 1996. Four stands were not thinned" (16 + 4 = 20). Status: **18 CONFIRMED as stated; 18 vs 20 UNRESOLVED inside the source.** Write "18 (abstract, §2.1); p.3 also says 20" (R-1). |
| V-10 | P5 3,360 trees; ~49 trees/ha/yr "partially verified" | c.88 / P5:4 | **CONFIRMED (R):** "3360 trees reaching or surpassing a height of 1.3 m… on average 40 ingrowth trees per measurement time and sample plot or **49 ingrowth trees per hectare and year**." Sanity check: 40 trees per 40 m × 40 m plot (0.16 ha) = 250 /ha per five-year interval ≈ 50 /ha/yr. Upgrade the label (R-2). |
| V-11 | #860 addendum P2 claim | c.15 abstract; c.25, c.28, c.30; Fig. 7 c.30 / P2:1, 11, 14, 16 | **CONFIRMED with page corrections** — §4. |

V-1, V-2, V-5…V-8 are the "prose-count" class the producer deemed safe for text-layer checking; I confirmed all of them on rendered pages as well.

---

## 4. Disputed rows (P2-01, P2-04, O-04) and 09 against the real addendum

### 4.1 P2 agreement facts (rendered)

* **Table 3 (c.23 / P2:9)**, all six rows read: <0.10 poor; 0.10–0.33 slight; 0.33–0.50 fair; 0.50–0.67 moderate; 0.67–0.90 substantial; ≥0.90 almost perfect. 08's table matches (08 writes ">0.90"; the paper writes "≥ 0.90" — trivial).
* **Fig. 7 (c.30 / P2:16)**: bars Poor 4 (red = crown thinning), Slight 14 (red), Fair 17 (16 black low thinning + 1 red), Moderate 1 (black), Substantial 0, Perfect 0 — **total 36**. So 19 crown-thinning and 17 low-thinning experiments; "4 of 36 poor" and "no substantial/almost-perfect" are right (the "4 out of 36" prose is also on P2:17, c.31, text layer).
* **P2:14 (c.28)**: low-thinning κ_F "generally between 0.35 and 0.51 (fair to moderate)"; crown thinning "between 0.01 and 0.28 (**poor to slight**…, with the notable exception of experiment Loch Ard 2015, here κ_F = 0.44)". The paper's *summary* sentence on P2:16 says "poor to fair" for crown thinning; both are in the paper, so 08/09's "poor-to-fair" is accurate to the summary, but only **one** crown experiment (Loch Ard 2015) actually reached "fair".
* **P2:11 (c.25)**: "significantly more agreement in low thinnings than in crown thinnings (p < 0.001 for both)" (κ_F and c̄) — supports the p-value only, **not** the category wording.
* **P2:1 (c.15 abstract; text layer)**: "general agreement in tree selection in Britain was only slight to fair" — directly supports the addendum's "slight-to-fair".
* **P2:15 (c.29; prose read in the text layer, page rendered but not viewed) — a finding 08 under-reports:** "none of them [forest characteristics] were significant [for κ_F]. Instead we found a **weak correlation** between the mean conformity number c̄ and the coefficient of variation of stem diameters v_d and the skewness k_d… These correlations suggest that **conformity increases with increasing complexity of tree stem diameter structure**." Also P2:6: "stem size diversity… is rather low, which is typical of plantations" (Table 2 v_d spans only 0.24–0.37). So 08's "does not support 'complex forests cause agreement'" is an editorial position; the authors themselves read a positive (correlational) association for *conformity*, on a narrow-structure plantation sample (R-6).
* **Plot area:** P2:5 (c.19) "each marteloscope had a size of 0.1 hectares"; **P1:7** corrects this for Ae (0.133 ha). 08 §4 and 09 say "0.1 ha" without that qualifier (R-7).

### 4.2 Disputed rows

| Row | Producer status | Finding |
|---|---|---|
| **P2-01** (scale: 36/12, 0.1 ha, 19 rater groups of 9–20) | DISPUTED | Numbers CONFIRMED (36/12 rendered c.18, 19 groups/9–20 and ~95% state employees rendered c.20; 0.1 ha per P2:5 with the Ae exception above). A row the producer itself resolved as "primary-verified, promote" should no longer be `DISPUTED`; the composites contain no P2 figure, so the honest status is **MATERIAL GAP** with the addendum provenance upgrade handled separately (R-11). |
| **P2-04** (agreement results) | DISPUTED | Substance CONFIRMED (§4.1). Same status correction. Page pointers need correcting (R-9). |
| **O-04** (#860 addendum, nine reports) | DISPUTED | Remains `DRAFT / SECONDHAND / CONFLICTS OPEN`. Only the P2 sentence is promotable. I did **not** re-audit the other eight reports' claims; I confirmed only that the addendum still states the six-paper PDFs were "not read" (branch line 78). Status DISPUTED/DRAFT: **agreed.** |

### 4.3 09 against the actual addendum

* **"Current text" is accurate.** The quoted bullet appears **verbatim** in both the branch copy and the desktop copy (normalised-whitespace match; branch line 55 of 12,275 B). 
* **"Not applied / other claims unchanged":** correct — only that one bullet is touched; the Teagasc/GBIF/Zenodo/Forest Research items and the U-4 conflicts are untouched.
* **Proposed sentence — accurate in substance:** 36 experiments/12 sites ✓; 19 rater groups of 9–20 ✓; poor-to-fair (crown) and fair-to-moderate (low) ✓; "overall mostly slight-to-fair" ✓; 4 of 36 poor ✓; none substantial/almost perfect ✓; p < 0.001 low-vs-crown ✓; categories from Table 3 ✓.
* **Four defects to fix (R-8…R-10):**
  1. **Wrong MART clause.** 09 says "MART pp.2, 10 remains the secondhand source for the *dataset-rights* discussion only." In the addendum, "MART pp.2, 10" is attached to the **marking-agreement claim itself**. I opened the MART PDF: **p.10** states "In the 2018 British study, 36 experiments across 12 sites produced only slight-to-fair general agreement"; p.2 (summary table) frames human marking as a retention choice, not ground truth (wording split across table columns, so treat as paraphrase). The rights tiers are a separate bullet citing **MART pp.6-7, 15, 19**.
  2. **Page pointers.** "P2 pp. 11, 16-17" omits **p.14** (the κ_F ranges and the Loch Ard exception) and **p.1** (abstract). p.11 supports only the p < 0.001 statement. Fig. 7 (p.16) holds the counts; "4 out of 36 poor" is also stated in prose on p.17 (c.31).
  3. **Attribution.** "PRIMARY-VERIFIED against the paper" does not say *by whom*. The addendum's own text says its author did **not** read the six-paper PDFs; removing "I have **not** verified this" without naming the verifier and artefact leaves the document internally contradictory. Name the verifier(s) and link the review artefact.
  4. **"0.1 ha marteloscopes"** — qualify with the Ae 0.133 ha exception (§4.1).
* **Optional "five of the six" clarification:** factually accurate — the addendum's five titles match P1, P6, P5, P3, P4 by file name and topic, and P2 is the sixth. **Not approved here**; keep the edit to one bullet and let #425 decide (§9).

---

## 5. Further quantity and claim findings (beyond the V-list)

| # | Finding | Evidence | Effect |
|---|---|---|---|
| Q-1 | **08 §7 "The cause of every observed death is unknown" is false.** | c.95 / P5:11: "**more than half** the causes of death remained unknown (Valkonen et al., 2020b)". c.88 / P5:4: 1,206 deaths "mostly as a consequence of wind and snow, followed by competition and harvesting damage". 08 contradicts itself within §7 (the "Mortality records" bullet vs the "Model structure" bullet). The extraction (e.14 "the cause of every observed death is not known") has the same imprecision; do not edit the extraction, correct 08/02. | Blocking (R-3). |
| Q-2 | P4 recruitment symbol is **λ_B** (e.12 uses λB); 08 Table 1, 03 V-4 and TSV P4-04 write "B". Table 1 gives "(Equation 10)" for c₀ and c₁ only; d has no equation cited. | c.66 | R-4. |
| Q-3 | P3 T_ij top class is 0.7 ≤ T ≤ 1.0 (left-closed classes 0 ≤ T <0.3, 0.3 ≤ T <0.5, 0.5 ≤ T <0.7). 08 writes ">0.7". | c.39 / P3:5 | R-19. M_i values (0, 0.33, 0.67, 1.00), n = 3 and equation (5) ✓. |
| Q-4 | P5 Table 2 is for **plot VES01**, **mean** site index; cells 23.5 (19.8, 25.9) / 26.8 (22.7, 29.5) / 32.3 (27.7, 36) are plot effect −1.96σ / 0 / +1.96σ ✓. At zero plot effect the site-index spread (min/mean/max) is 25.1 / 26.8 / 30.5 — the numbers that actually support "plot effects matter more than site index" (P5:11 also states plot-effect variation ≈ an 11–21 m variation in site index). 08 omits plot ID and SI context. | c.93 / P5:9; c.95 / P5:11 | R-26 (non-blocking). |
| Q-5 | **P1-11 "agreed more on retention" needs its absolute level.** P1:9: kappa highest in low thinning (fair), then frame-tree selection (slight), lowest for competitors (slight); frame-tree selection had no poor case and 13 fair; "in all cases agreement was low and did not exceed one case of moderate agreement". 08 calls this "the strongest teaching evidence in the set" without saying agreement was still low. | c.11 text (P1:9) | R-18. |
| Q-6 | **Two incompatible frame-tree densities could enter the corpus.** 08 §3 gives P1's literature rule of thumb (~100/ha). The Practice composite already holds a measured Irish trial value ("Select ~250 future parent/frame trees ha⁻¹ (Brownhill)", §4). 08 §9's cross-reference table does not mention it. | Practice L167 | R-17. |
| Q-7 | P6-11 "no Irish probabilities" is true, but P6 **does** mention Sitka in the Irish context: "clearcutting is followed by natural regeneration… *Picea sitchensis* in Ireland" (c.104 / P6:6), and a Welsh Sitka group-strip shelterwood example, Fig. 6 (c.107 / P6:9, Cefn Du, Clocaenog). Neither is in the extraction or 08. Qualitative only, but directly relevant to a Sitka-transformation project. | text layer c.104, c.107 | Add as candidate row P6-12 or a limit note (R-22). |
| Q-8 | 08's P4 "[P4:23–25]" simulation result and "500 simulations" were read from the text layer only (prose): consistent with source. | c.78–c.79 text | no change |

---

## 6. Coverage: independent sweep, row challenges and the 44 / 13

### 6.1 Sweep method

I ran **my own vocabulary** over **all four composites, plus the canonical Git Growth** (so five files), counting case-insensitive substring hits, in three layers: (a) the producer's published tokens (37); (b) 165 synonyms/alternate terms across the same eight families (e.g. *future crop, F1/F2, Z-tree, retained tree, tending, crown release, thinning from above/below, inter-rater, consensus, subjectiv, nearest-neighbour, aggregat, clump, regularity, point pattern, size diversity, plasticity, crown shyness, lidar, shot-noise, probabilistic, stochastic, random effect, prediction interval, group selection, irregular, femel, coupe, seed tree, selection system, enrichment, slenderness, H/D*); (c) **65 exact figures and author names** from the gap rows (e.g. *5,591; 17,153; 1,206; 3,360; 0.5697; 96.3815; 0.975; Fleiss; kappa; Myllymäki; Stoyan; Valkonen; 22 m²; 12 m²; 23.5; LAP07; 0.05–0.5; 5–30 m*). I read in context the non-zero hits for every family whose status matters (all P1/P2/P3/P4 vocabulary, Hegyi, mortality, strip/shelter); for the high-count generic terms (*transformation, conversion, edge, stability, recruitment, envelope*) I sampled contexts.

| Family | Producer tokens: hits (Eco/Brw/Gro/Pra) | My broader terms: hits (Eco/Brw/Gro/Pra) | What the hits actually are |
|---|---|---|---|
| P1 frame/crop tree | 0/0/2/3 | 3/3/12/5 | Practice §3 Pro Silva positive selection and Brownhill trial; Growth "crop-tree DBH" envelopes. Concept **partly** held at practitioner level. |
| P1 competitor/release | 3/0/0/7 | 10/0/4/14 | Release in a *deer/light/thinning* sense; Practice §3 competitor-only removal. |
| P2 marking agreement | 0/0/0/1 | 0/1/0/18 | Practice: marking *conventions*, marteloscopes as exercises, labour. **No kappa, no agreement, no rater, no experiment result** anywhere (probes "Fleiss", "kappa", "κ", "Stoyan", "36 experiments", "12 sites", "agreement": all zero). |
| P3 structural indices | 0/0/0/0 | 1/0/1/1 | "structural diversity" ×1 (Practice), "aggregat" ×1 (Eco, unrelated), "mixing" ×1 (Growth, "fixed mixing coefficients"). **No mingling/differentiation/Clark–Evans/contagion/Pielou/Shannon/pair-correlation.** |
| P4 crown plasticity | 0/0/0/0 | 3/0/1/1 | Growth §8: measured crown models (Tabbush; Davies & Pommerening 2008). "Poisson" in Ecology = seedling fecundity, unrelated. |
| P5 Bayesian/uncertainty | 0/1/1/0 | 13/8/18/20 | Almost all noise (*recruitment, envelope, BAL, Norway spruce*). Hegyi: Growth §5 (competition multiplier), §8 (Davies & Pommerening caution) and Ecology §2. **No Bayesian, posterior, relascope, credible interval, random-effect model.** |
| P6 regeneration taxonomy | 0/0/0/0 | 18/18/11/62 | False positives: Browsing "shelter" = **tree shelters**, "strip" = **bark stripping**; Practice "transformation" (34) and "conversion" (Ecology 9: Sitka→CCF conversion, plus seedling→sapling "conversion") are *project* usage. No shelterwood/group/strip/plenter/two-storey system vocabulary. "clearfell" ×7 is economics. |
| h/d | 0/0/3/1 (02 prints Practice = 2) | 1/0/16/9 | Growth §6 wind-risk slenderness multiplier; Practice "high height:diameter ratios". **No h/d 40–80 realisation space or ideal-60 line.** |

**Result of the sweep: the producer's bulk verdict survives.** No composite covers marking agreement, structural indices, the CanopyShotNoise model, the Bayesian spatial model, or the regeneration-system taxonomy under other wording. The only same-author overlap (Davies & Pommerening 2008) is a different paper, correctly handled as covered (O-02). The "composites barely overlap the six papers" conclusion is **supported**; the producer's own flagged exposure (U-8, topic-level only) is **reduced, not eliminated**: I read the relevant contexts but did not read all 132,717 B line-by-line.

### 6.2 Row challenges (sampled statuses and what changes)

| Row | Producer | My finding | Result |
|---|---|---|---|
| **P3-01** (M_i) — MATERIAL GAP, NONE | GAP | From scratch: M_i and equation (5) on c.39 / P3:5 (n = 3 neighbours, 0/0.33/0.67/1.00; relation to Pielou's S on the same page); sweep above finds nothing equivalent. | **GAP upheld.** |
| **P5-09** (Hegyi variants similar) — GAP, ADJACENT | GAP, "strongest justification for not replacing Hegyi" | Source (c.95 / P5:11): Hegyi variants (Bianchi 2020; Pitkänen 2022) and Gaussian kernels "showed a performance similar to the selected RBA with very little difference"; RBA chosen partly for computational reasons (brms/Stan). It is a **qualitative one-sentence report with no numbers**, in a Finnish selection-forest growth/mortality fit, and says nothing about CCF's Hegyi or CI50. It supports "replace" as readily as "keep". Growth §5 (competition multiplier) and §8, and Ecology §2, already carry the Hegyi mechanism and the Davies & Pommerening pooled-coefficient caution. | **Status changes:** → `CROSS-REFERENCED-INTENTIONALLY` (low priority). Delete "strongest published argument/justification" wording in 02 row, 02-MD §6 item 4 and 08 §7 (R-13). |
| **P1-01** — XREF, ADJACENT | XREF | Practice §3 already states "Marking starts from the tree being favoured… assess its actual crown competitors, remove competitors only where release is worthwhile" (lines 133–135 of the archive copy) and the Brownhill frame-tree trial (lines 167–168, 172). Same passage is scored PARTIAL for P1-03. | **Status upheld; strength → PARTIAL.** Replace "evidenced basis" with "supporting literature": P1 is a review. (R-11 note.) |
| **P2-09** — XREF to "research-index D-065" | XREF | D-065 is not a composite and contains **no P2 evidence** (it is a delegated-treatment wording/analogue audit). **P2 contains no statement on automation or delegation** (grep of c.15–34: only that machine/harvester operators select trees in some countries, P2:2). The row's claim is a **design inference**, not a P2 finding. | **Status changes:** relabel `DESIGN NOTE — not a source claim`; remove from the XREF evidence count (R-14). |
| **P2-08** — XREF, ADJACENT | XREF | Partly supported: P2:17 asks whether "exact tree marking is even necessary", and says similar-but-different selections reduce "putting all eggs in one basket". The "evaluate treatment intent" training list is the producer's. Location again points at a non-composite. | **Upheld with wording fix** (R-14, R-16). |
| **P1-04** (local vs global) — XREF | XREF | Irish trial evidence on crown/low/GDT thinning is held ("crown thinning" ×5 Practice / ×1 Growth; "low thinning" ×2 / ×1) but the *local/global-thinning framing* ("local thinning", "global thinning", "local release": zero hits in all five files) is not. Adjacent, different evidence type. | **Upheld.** |
| **P1-05** (longitudinal review) — XREF to Growth §5/§7 | XREF | Growth §5/§7 are growth *models*, a weak anchor. Better home: Practice §3 "permanent sample plots and management history" (line 152 of the archive copy). | **Upheld; correct the location.** |
| **P3-08** (randomness ≠ naturalness; links unvalidated) — GAP | GAP (negative claim) | Source is more nuanced than 08's "Randomness ≠ naturalness": P3:8 (c.42) says "**By using randomness as a substitute for naturalness** it is possible to quantify…" and "In this paper, randomness is used as a reference", while conceding the criticism that a random reference "may not correspond to a particular biological meaning" and that naturalness is "difficult, if not impossible, to define" (Sprugel 1991). P3:17 (c.51, text): "Although randomness as a reference is **no substitute for naturalness**, deviation from randomness is a suitable indicator that… something special has been detected", and "further follow-up studies are needed… to relate… to habitat functions". | **Upheld as a limitation row, but it is dependent on P3-01…07** (it only qualifies those indices), so it inflates the count; state the caveat with the paper's own wording; present as caveat-to-indices, not an independent evidence gap. |
| **P6-11** — XREF | **TSV: XREF; 02-MD §6.3 and 07 §4 call it a "gap"** | Inconsistent labelling in the packet itself; plus omission in Q-7. | **Harmonise; add the Sitka note** (R-22). |
| **P6-09** — COVERED, DIRECT | COVERED by "research-index (rack/marking distinction, D-064) — safeguard already recorded" | **Not supported.** D-064 concerns map-only rack planning and first-tree entrance marking; `git grep -i "regeneration strip"` over `Docs/Project`, `Docs/Research`, `specs` on main = **zero hits**; none in any composite. The row is the producer's own DESIGN safeguard (08 §8). P6's own figures **draw extraction racks inside strip/wedge/group systems** (Fig. 3B, c.105 / P6:7; Fig. 5, c.107 / P6:9). | **Status changes:** not COVERED. Relabel as producer design caution (R-12). |
| **P6-08** — COVERED, DIRECT | COVERED | Practice §3 "Enrichment planting complements rather than replaces regeneration… underplanting where regeneration is absent" (line 145) is claim-equivalent to P6:6–7. | **Upheld** (claim-equivalence, different source, as the row says). |
| P1-03, P1-10, P2-02, P2-05, P2-07, P3-02…05, P3-07, P4-02/03/04/06/07, P5-01…04, P5-08, P5-10…13, P6-04, P6-10 | GAP | Source claim located on the original page; absence confirmed by concept- and number-sweep (P1-10 specifically: Growth §6 / Practice have slenderness but no 40–80/60 band; P5-10: the "22 m" hits are m³ yield-class values, unrelated). | **Upheld** with the labelled corrections elsewhere in this review. |
| **P5-08** and **O-06** | — | `composite_location` still contains the placeholder **"Growth §?"**. The Growth composite has no adult-mortality section (closest: §6 windthrow, §9 GROWFOR pointer). | **Unresolved placeholder** (R-15). |

### 6.3 The 44, the 13 and the tally

* **Are the 44 defensible?** The *absence* is defensible: 35 of the 44 are strength `NONE`, 9 are `PARTIAL/ADJACENT`; my sweep does not overturn any. But **"44" is a row count, not a measure of importance** — it depends on how finely the producer split papers (P5 alone is 14 rows). The more honest summary is the producer's own: six topic clusters, mostly caveats and method descriptions. Gap rows touched: **34 of 44** — 27 with the source claim verified on a rendered page, 7 on text-layer prose only (P1-08, P1-09, P1-10, P1-11, P2-06, P2-07, P4-07); **not touched (10): P1-02, P2-03, P3-06, P4-05, P5-06, P6-01, P6-02, P6-03, P6-06, O-03** (covered only by the topic/number sweeps). Sampled changes: **P5-09 leaves the set**; **P2-01 and P2-04 join it**.
* **The 13 cross-references:** I examined 9 — 7 closely (P1-01, P1-04, P1-05, P2-08, P2-09, P6-07, P6-11) and 2 lightly (P5-05, P5-07: equations verified, composite adjacency not re-derived). Changes: **P2-09 leaves** (design note), **P5-09 joins**; P1-01 strength, P1-05 location and the P5-05 Growth section corrected; P6-11 labelling harmonised. Not re-adjudicated: P1-06, P4-01, P5-14, P6-05.
* **Reviewer-recommended tally if every sampled change is adopted** (unsampled rows unchanged): GAP 44 + P2-01 + P2-04 − P5-09 = **45**; CROSS-REFERENCED 13 + P5-09 − P2-09 = **13**; COVERED 6 − P6-09 = **5**; DISPUTED 3 − P2-01 − P2-04 = **1** (O-04); OUT OF SCOPE / DESIGN NOTE 1 + P6-09 + P2-09 = **3**; **total 67** (68 if candidate row P6-12 is added as a gap). Arithmetic of the *producer's own* tally I re-derived independently from the TSV: **67 rows, 11 fields each, 44/13/6/3/1**, per-paper table matches — the arithmetic is correct; the labels are what change.
* **The 67 are not exhaustive** (Q-7).
* **Distinct paper evidence vs duplicative practitioner evidence:** the Practice composite's Pro Silva statements are practitioner synthesis; P1/P2 add *experimental human-behaviour* evidence the composites lack. That distinction is real and the producer's cross-reference logic for P1/P2 holds; it is weaker for P2-09 (not evidence) and for P5-09 (not decisive).

---

## 7. Deliverable usability: 08 (proposed composite) and licence

### 7.1 Audited sample of 08 (77 assertions, §§2–8)

Legend: **R** = verified on a rendered page image; **T** = verified in the text layer, *prose only*; **X** = defect found (see ID); **O** = not checked.

| ID | Assertion (08 section) | Check | ID | Assertion (08 section) | Check |
|---|---|---|---|---|---|
| L01 | P1 citation + CC BY (§2) | T | L40 | Mark correlation (§5) | T |
| L02 | P2 citation + CC BY | T | L41 | Edge/window bias (§5) | R |
| L03 | P3 citation, © ICF (DOI not printed in PDF) | T | L42 | "Randomness ≠ naturalness" (§5) | R+X (§6.2 P3-08) |
| L04 | P4 citation + CC BY-NC-ND | T | L43 | Index–habitat link unvalidated | T |
| L05 | P5 citation + CC BY | R | L44 | Worked illustration "…without improving recruitment" | X (Q-all: not in P3; R-16) |
| L06 | P6 citation + CC BY-NC-ND | T | L45 | P4 scope [P4:2, 5–11] | O |
| L07 | Frame tree + neighbourhood (§3) | R | L46 | P4 data: 5,591/17/5/4–9/~7 | R+X (V-1 ambiguity) |
| L08 | 5–10 yr horizon | R | L47 | τ = 0.975; shot-noise field | R |
| L09 | 0–3 competitors | R | L48 | Table 1, 15 values / 8 groups | R+X (λ_B; Eq. 10) |
| L10 | Local vs global thinning | R | L49 | Fitted vs freely determined | R+X (page) |
| L11 | Longitudinal value | O | L50 | 100 m, 0.025/m², C0S0…C1S1, R = 40 cm, S = 1.3 | R |
| L12 | Density rules of thumb (§3) | T+X (Q-6) | L51 | Superorganism simulation finding | T |
| L13 | Release classes 0–2 / 2–3 / 3+ | T (first two classes seen) | L52 | "Excluded by source's framing" (guardrail text) | n/a |
| L14 | h/d 40–80, ideal 60 | T | L53 | P5 "18 analysed plots" | R+X (18 vs 20) |
| L15 | A-thinning A = 4–8 | O | L54 | 40×40 m, 1991–2016, every fifth season | R |
| L16 | 26 experiments | R | L55 | Norway spruce ~75% BA | R |
| L17 | 6–20 raters; 83–323 trees | R | L56 | 17,153 / 5,206 / 2 mm / 0.7–2.9 | R |
| L18 | Retention > competitors; low > crown; p<0.001 | T+X (Q-5) | L57 | 18,423 / 5,635 / 1,206 | R |
| L19 | 36 experiments, 12 sites (§4) | R | L58 | 3,360 / 49 ha⁻¹yr⁻¹ | R (stated outright) |
| L20 | 0.1 ha each | T+X (Ae 0.133 ha) | L59 | RBA with BAF 2 | R |
| L21 | 19 groups of 9–20 | R | L60 | "Cause of every observed death unknown" | **X (Q-1)** |
| L22 | ~95% state-employed | R | L61 | 95% global envelopes | R |
| L23 | Fleiss κ; "not an ecological quality measure" | O | L62 | Hegyi/Gaussian vs RBA; "strongest argument" | R+X (§6.2) |
| L24 | Results: poor–fair/fair–moderate/slight–fair | R | L63 | 12 / 22 m² ha⁻¹, 5-yr checks | R |
| L25 | 4 of 36 poor; none substantial | R | L64 | 23.5 / 26.8 / 32.3 | R+X (context, Q-4) |
| L26 | p < 0.001 | R | L65 | LAP07 500–1,800 vs 1,300–1,900 | R |
| L27 | "No structural explanation" | T+X (direction) | L66 | Plot effects vs site index | R |
| L28 | Table 3, six categories | R | L67 | Shelter / margin / open (§8) | R+X (paper says "bare land", R-28) |
| L29 | Ae Table 2 row | R+X (P1 1,321) | L68 | Edge zones 5–30 m | R |
| L30 | B ratio direction | T | L69 | Four basic systems | R |
| L31 | Training should test "treatment intent… regeneration protection, work access" | **X (producer list, not P2)** | L70 | Transformation vs conversion | R |
| L32 | Variation ≠ licence for automation | **X (P2 silent)** | L71 | 0.05–0.5 ha static groups, Polish classification | R |
| L33 | M_i, n = 3 | R | L72 | Selection/plenter, two-storeyed | O |
| L34 | T_ij and classes | R+X (≥ 0.7) | L73 | Felling purposes | O |
| L35 | Clark–Evans | R | L74 | Variants vs combinations | T |
| L36 | Contagion W_i, 90° | R | L75 | Partial/enrichment planting compatible | T |
| L37 | Shannon H′ | O | L76 | "Regeneration strip is not an extraction rack" | **X (§6.2 P6-09)** |
| L38 | Pielou S | R | L77 | "No Irish probabilities" | T+X (Q-7) |
| L39 | Pair correlation g(r) | O | | | |

**Tally (recounted programmatically from this table):** 77 assertions (L52 is a guardrail, not a source claim). Verified on a rendered page (R, incl. R+X): **45**. Text-layer prose only (T, incl. T+X): **18** (for L01, L02, L04 and L06 the journal/volume/page header was seen on a rendered page but the licence statement only in the text layer, so they are scored T; author lists and DOIs for P1/P3/P4/P6 were not cross-checked). Defect with no clean verification path (X): **5** (L31, L32, L44, L60, L76). Not checked (O): **8** (L11, L15, L23, L37, L39, L45, L72, L73). n/a: 1. **Total 77 = 45 + 18 + 5 + 8 + 1.** Claims carrying a **defect** (any X marker): **20**. The unchecked ones are named so Manager #425 can see exactly what remains unverified: **L11, L15, L23, L37, L39, L45, L72, L73**.

### 7.2 Matrix rows and pages

* **Rows touched: 48 of 67.** Gap 34, cross-reference 9, covered 2 (P6-08, P6-09), disputed 3. **Untouched (19):** P1-02, P1-06, P2-03, P3-06, P4-01, P4-05, P5-06, P5-14, P6-01, P6-02, P6-03, P6-05, P6-06, O-01, O-02, O-03, O-05, O-06, O-07. **Re-derived from scratch** (original page + extraction + composite context + status): P3-01, P5-09, P1-01, P2-09, P2-08, P1-04, P3-08, P6-11, P6-09, P6-08.
* **Rendered and visually inspected collection pages (21):** 4, 9, 18, 20, 23, 25, 28, 30, 37, 39, 42, 66, 67, 68, 85, 87, 88, 93, 95, 100, 109; **extraction p.12**. (Also rendered to PNG but **not viewed** — text layer used for prose: c.11, 15, 16, 17, 29, 31, 38, 101.) Text layer, **prose only**: c.5–8, 10–11, 13, 19, 24, 29, 31, 51, 78–79, 102, 104–105, 107 and the header/licence lines of c.3, 35, 55, 56, 99.
* **Original pages cited by 02 versus the page I found:** P1 Table 1 = P1:7 (c.9) ✓; P2 Table 2 = P2:6 (c.20) ✓; P3 formulas P3:3, P3:5 ✓; P4 Table 1 = P4:12 ✓; P5 data = P5:3–4 ✓, P5 Hegyi = P5:11 ✓, P5 BA table = P5:9 ✓; P6 definitions P6:2 ✓, 0.05–0.5 ha = P6:11 ✓. **No mis-attributed paper and no off-by-one except P4 "freely determined" (p.13).**

### 7.3 Is 08 a usable Project attachment?

* **Non-duplicative: yes.** §6.1 shows the content is genuinely absent from the four composites; at ~25 KB it is a single new file whose cross-reference table (08 §9) mostly points to the right homes. Exceptions: the missing Brownhill link (Q-6) and the h/d row, which cites Growth §5 where the slenderness content is in **§6** (R-27).
* **Traceable: largely yes** — page references on every claim, evidence classes, transfer limits, a licence note. Defects are listed above.
* **Smuggled mechanics: not in the exclusions list, but present in the body.** 08 §10 and §8 safeguards are exemplary. However (a) the §5 index table has a **"Handling" column** with product advice ("Read-only candidate; show beside species proportions"; "Not a near-term HUD statistic"; "Offline research chart"); (b) §4 lists what "Training should test" (future-crop retention, removal pressure, crown release, regeneration protection, work access); (c) §5's worked illustration asserts dispersed broadleaves raise mingling "without improving recruitment" (a recruitment claim P3 does not make); (d) §7's "strongest published argument for benchmarking, not replacing" (a CCF-Hegyi position P5 does not take). None is a coefficient, but together they put *design recommendations in an evidence document*, which the packet asks me to flag. **Remove or move them** (R-13, R-16).
* **Licence note (careful, not legal advice).** Verified on the PDFs: P1 CC BY (c.3), P2 CC BY (c.15), P3 © Institute of Chartered Foresters 2002 (c.35; no open-licence statement found on the pages checked), P4 **CC BY-NC-ND 4.0** (c.56), P5 CC BY 4.0 (c.85), P6 **CC BY-NC-ND** (c.99). 08's licence column is **correct**. 08 §2's sentence "Rights are as published; this condensation grants no new distribution or derivative rights" is the right disclaimer but **does not decide** whether a Project attachment is permissible. The P4 Table 1 transcription (15 numeric values) and the substantial paraphrase of P4/P6 (NC-ND) and P3 (closed copyright) are an **#425 rights question**; I infer no redistribution right. No PDF page or image is reproduced in this review.
* **Project-attachment slot arithmetic:** unverifiable from here, as the producer also said; not assessed.

---

## 8. Required corrections (each with file and exact change)

Blocking items are marked **[B]**. After correction the producer must re-issue a new SHA; closure review is limited to these items.

| # | File | Exact change | Evidence |
|---|---|---|---|
| R-1 [B] | `03-PRIMARY-VERIFICATION-LOG.md` V-9; `05` U-6; `10` residual #2; `08` §7 sample bullet; `02-coverage-matrix.tsv` P5-01 | Re-label V-9 **"CONFIRMED as stated; source internally inconsistent"**. Wording: "18 plots/stands (abstract; §2.1; P5:4 'the 18 plots altogether'); the same paper also says 'the 20 plots' (P5:3) and 16 + 4 stands. Not reconciled." Drop "unverified". | c.85, c.87, c.88 |
| R-2 [B] | `03` V-10; `05` (if cited); `10` residual #3; `02` P5-04; `08` §7 | Re-label **CONFIRMED**: "3,360 trees ≥1.3 m; on average 40 per measurement time and plot, or 49 per hectare and year (P5:4)". Remove "the extraction quotes ~49". | c.88 |
| R-3 [B] | `08` §7 "Model structure"; `02-coverage-matrix.tsv` P5-08 | Replace "The cause of every observed death is unknown" with "more than half of the causes of death remained unknown [P5:11]; where attributed, mostly wind and snow, then competition and harvesting damage [P5:4]". Keep "association is not a causal taxonomy". | c.95, c.88 |
| R-4 [B] | `08` §6 Table 1; `03` V-4 row; `02-coverage-matrix.tsv` P4-04; `04` if quoted | Change the recruitment symbol "B" to **λ_B**. In the mortality row, "(Eq. 10)" applies to c₀ and c₁ only. | c.66 |
| R-5 | `08` §6 "Only k, q and the three kernel parameters were fitted" | Page cite `[P4:12]` → `[P4:12–13]` ("freely determined" is on P4:13). | c.67 |
| R-6 [B] | `08` §4 "No structural explanation"; `02` P2-07 action | Rewrite: "No forest characteristic was significantly related to κ_F; a **weak positive correlation** of the mean conformity number c̄ with v_d and k_d led the authors to suggest conformity increases with diameter-structure complexity [P2:15] — correlational, on low-diversity plantation sites (Table 2 v_d 0.24–0.37)." Delete "does not support 'complex forests cause agreement' or 'disagreement'" as editorial, and replace the P2-07 action "prevents 'complex forest causes agreement' claims". | c.29, c.20 |
| R-7 [B] | `08` §4 design bullet and §4 Table 2 note; `09` proposed sentence; `02` P2-01/P2-02 | Qualify "0.1 ha each": "…0.1 ha each, except Ae at 0.133 ha (P1:7)". Add: "P1 Table 1 lists Ae at 1,321 trees/ha, h₁₀₀ 21.2 m (n = 176) versus P2 Table 2 1,336 / 21.1 — different years/areas; neither is 'the' Ae inventory." | c.9, c.19, c.20 |
| R-8 [B] | `09` §"Proposed text", last sentence | Replace "MART pp.2, 10 remains the secondhand source for the *dataset-rights* discussion only" with "MART pp.2, 10 remains the secondhand report of this same agreement finding (MART PDF p.10); the rights tiers are a separate bullet (MART pp.6-7, 15, 19)". | MART PDF pp.2, 10 |
| R-9 [B] | `09` proposed text; `04` §2.1; `06` §4 | Page pointers → "P2 pp. 1 (abstract), 11 (p < 0.001), 14 (κ_F ranges; Loch Ard 2015 κ_F = 0.44), 16 (Fig. 7: poor 4, slight 14, fair 17, moderate 1), 17 (4 of 36 poor); Table 3 p. 9". Say crown thinning was poor-to-slight with one fair (Loch Ard 2015). | c.15, 25, 28, 30, 31 |
| R-10 [B] | `09` proposed text | State **who** verified and where (e.g. "verified against the original by #883 producer V-6/V-11 and independently by #896"). Add a note that the addendum's separate line stating the six-paper PDFs were "not read" remains true of its author. | addendum line 78 |
| R-11 [B] | `02-coverage-matrix.tsv` and `02-COVERAGE-MATRIX.md` §4, `06`, `00`, `10` | P2-01, P2-04: `DISPUTED` → `MATERIAL GAP` (resolved: primary-verified). P1-01 strength `ADJACENT` → `PARTIAL`; replace "evidenced basis for existing individual-tree framing" with "supporting literature". Re-tally per §6.3. | §4.2, §6.2 |
| R-12 [B] | `02` P6-09 + tally; `08` §8 Safeguards | P6-09 is **not COVERED**: D-064/D-065 and all composites contain no regeneration-strip/rack distinction (git grep zero). Relabel as producer **DESIGN caution**; in 08 say so, and note P6's own figures draw extraction racks inside strip/wedge/group systems (Fig. 3B, Fig. 5). | git grep; c.105, c.107 |
| R-13 [B] | `02` P5-09 row; `02-MD` §6 item 4; `08` §7 table row | Status → `CROSS-REFERENCED-INTENTIONALLY`, strength ADJACENT, low priority. Delete "strongest justification/argument for not replacing / for benchmarking". Say: "index choice made little difference in this Finnish fit; no numbers reported; the authors chose RBA partly for computational reasons [P5:11]". | c.95 |
| R-14 [B] | `02` P2-08, P2-09; `02-MD` §6 item 1 | P2-09 → `DESIGN NOTE` (P2 silent on automation/delegation; D-065 is not a composite and holds no P2 evidence). P2-08: replace composite_location "research-index D-065" with "none (design inference; supported in part by P2:17)". | grep c.15–34; research-index |
| R-15 | `02` P5-08, O-06 | Resolve the placeholder "Growth §?" (Growth has no adult-mortality section; §6 windthrow / §9 GROWFOR pointer are the nearest). | TSV |
| R-16 [B] | `08` §5 table "Handling" column and "Worked illustration"; `08` §4 "Use and limits" first bullet | Delete the Handling column (or move to a clearly non-evidence "design observations" section outside the composite); delete "…without improving recruitment"; label the training-intent list as the producer's inference, not a P2 finding. | §7.3 |
| R-17 | `08` §3 density table and §9 | Add a cross-reference to Practice §3/§4 (Brownhill, ~250 future parent/frame trees ha⁻¹, a measured Irish trial) next to P1's ~100/ha rule of thumb. | Practice L167 |
| R-18 | `08` §3 "Human-selection finding" | Add the absolute level: frame-tree selection "slight" overall (no poor case; 13 in the fair class), competitors "slight", low thinning "fair"; "in all cases agreement was low… one moderate" [P1:9]. | c.11 |
| R-19 | `08` §5 T_ij row | Classes are left-closed: 0 ≤ T <0.3, 0.3 ≤ T <0.5, 0.5 ≤ T <0.7, 0.7 ≤ T ≤ 1.0. | c.39 |
| R-20 | `03` V-1; `08` §6; `02` P4-02 | Quote the source sentence ("a total of 17 plots at each of the five sites") and flag the 17-total vs 17-per-site ambiguity instead of stating "17 plots at five sites". | c.66 |
| R-21 | `02-MD` §2; `05` U-8; `00` | "130,756 bytes of composite text" → the four attachment copies are 132,717 B (131,136 characters); 135,214 B with the canonical Growth. | measured |
| R-22 | `02-MD` §6.3; `07` §4; `02` P6-11; `08` §8 Limit | Harmonise P3-08 (gap) vs P6-11 (TSV: cross-reference; narrative: "gap"). Add: P6 mentions Sitka natural regeneration after clearcut in Ireland (P6:6) and a Welsh Sitka group-strip shelterwood example (Fig. 6, P6:9) — qualitative, no probabilities. Add candidate row P6-12. State that the 67 rows are not exhaustive. | c.104, c.107 |
| R-23 | `02-coverage-matrix.tsv` header; `02-MD` | Rename `original_pdf_page` → `paper_page` (values are paper-relative); add the collection offsets (P1 +2, P2 +14, P3 +34, P4 +54, P5 +84, P6 +98). Note that P4 Table 1 is p.12 in both the paper and the extraction by coincidence. | P2 Table 2: p.6 = c.20 |
| R-24 | `03` V-1…V-8 | Add the page for each verbatim quote (table in §3 of this review). | §3 |
| R-25 | `08` §3 "A-thinning index … A = 4–8" | Mark NOT VERIFIED or verify on the rendered P1:5 equation page. | not checked |
| R-26 | `08` §7 P5 Table 2 row | Add "plot VES01, mean site index; ±1.96σ plot effect"; optionally the SI spread at zero plot effect (25.1 / 26.8 / 30.5). | c.93 |
| R-27 | `08` §9 row 2; `02-coverage-matrix.tsv` P1-10, P5-05, P5-09 | Growth section pointers: slenderness/h-d is **Growth §6** (windthrow and stability), not §5; the Hegyi competition multiplier is **Growth §5** (plus §8, Ecology §2), not "§1/§2". | Growth canon L211 in §5, L257–260 in §6 |
| R-28 | `02-coverage-matrix.tsv` P6-01; `08` §8 first bullet | P6 names the third principle **"bare land"** ("bare-land principle"), not "open". Either use the paper's term or flag the relabel. | c.100 / P6:2 |

---

## 9. Optional improvements (non-blocking)

1. Put the V-4 lesson in one sentence in 08's header and drop the long correction note from the body once the review closes, so the Project attachment does not carry a retraction story.
2. Add a two-line "how to read paper vs collection pages" table (offsets above) to 08 §1; it already defines the convention but not the offsets in a lookup form.
3. List *all* P2 Table 2 rows from the rendered table (the producer chose to tabulate only Ae). I read all twelve; Sitka spruce sites are Ae, Bin, Craigvinean, Glentress, Haldon, Loch Ard; the rest are hybrid/Japanese larch, Scots pine, and one beech site (Peckett Stone).
4. Give each P5 figure its Valkonen reference where the source attributes it (the death-cause statement cites Valkonen 2020a/2020b).
5. Keep the optional "five of the six papers" addendum clarification out of the same edit; it is accurate, but it expands the change beyond the single P2 bullet the producer chose to defend. Leave to #425.
6. Consider presenting the gap count as six topic clusters rather than "44", as the producer's own §1 reasoning already does.

---

## 10. Residual unknowns and gate impact

| Unknown | Impact |
|---|---|
| P5 plot count 18 vs 20 (internal to source) | Low for the research file once labelled; blocks any use of "18" as a settled sample size. |
| P4 "17 plots at each of the five sites" interpretation | Low; blocks quoting "17 plots in total" as fact. |
| 8 of 08's 77 assertions not checked (L11, L15, L23, L37, L39, L45, L72, L73), and author lists/DOIs for P1/P3/P4/P6 not cross-checked | Does not block *consideration* if listed as unverified in 08; blocks calling 08 "fully verified". |
| 4 of 13 cross-reference rows not re-adjudicated (P1-06, P4-01, P5-14, P6-05) and 2 only lightly (P5-05, P5-07); 19 of 67 rows not touched at all (listed in §7.2); they rest on the topic/number sweeps only | Does not change the bulk verdict; limits "complete coverage" claims. |
| Project attachment bytes (PDF pair and four composites) not opened; I used hash-matched local originals/archive copies | #425 should confirm the Project slots hold these bytes before any replacement decision. |
| Rights status of reproducing P4 Table 1 / paraphrasing NC-ND and © papers inside a Project attachment | #425/owner decision; no rights inferred. |
| Stale Growth Project attachment (33,058 B vs canonical 35,555 B) and three composites without Git counterparts (producer U-1/U-2) | Untouched; separate #425 matters. |
| My visual reading of rendered images | Mitigated by cross-comparing every cited digit with the text layer; none disagreed. A second reader would still be the strongest closure check for P4 Table 1 if #425 wants it. |

---

## 11. Next handoff

**NEXT HANDOFF: producer correction.** Return §8 to the #883 producer (or a successor under #425) as a corrected SHA; the delta review should check R-1…R-28 only (14 blocking). After closure, hand to **Manager #425** as a decision packet. Nothing in this review authorises canonical publication, a Project attachment slot, a composite rewrite, a Drive mirror change, a source or #860 addendum edit, any mechanic, coefficient or UI text, or any redistribution of the source papers. The nine-report unrelated #860 claims remain `DRAFT / SECONDHAND / CONFLICTS OPEN`.

### Transfer limitations (carry with any use of the six papers)

| Paper | Geography / material | What it cannot support |
|---|---|---|
| P1 | Great Britain, planted even-aged stands (mostly Sitka), 26 training experiments | Irish release/thinning rules; frame-tree densities as targets (Irish trial uses ~250/ha) |
| P2 | Britain, 12 sites, 36 experiments, plantation structures (v_d 0.24–0.37) | Agreement under complex CCF structure; automation of marking |
| P3 | European illustrative stands, 2002 method review | Validated index–biodiversity links; edge-free indices on a bounded estate |
| P4 | Swiss European beech; **simulation** with freely chosen parameters (only k, q, α, β, δ fitted) | Any parameter in a game; facilitation/root-sharing; Irish Oak/Beech crown behaviour |
| P5 | Finnish Norway-spruce selection forest; 18 (or 20) plots | Irish ingrowth rates, mortality causes, a replacement growth system; a decision on CCF's Hegyi index |
| P6 | Conceptual review; Polish classification example; passing Sitka mentions | Irish establishment probabilities; required workflows; gap-size thresholds |

None of the six is an Irish study and none supplies an Irish coefficient — this review found no such claim in 08.

---

## 12. Boundary and publication record

* **Written:** this one file, on a new branch from `main` `56e1cae49f44953a87d18c181a4ac8052b69aadf` (the claim comment named the earlier `cfc802d2…` main; the branch was re-based because main advanced, see §1). **Not touched:** anything under `883-Evidence-Coverage-Quality/**`, the original PDFs, the four composites (neither archive nor canonical), the #860 addendum, Project attachments, Drive, `Docs/Project/**`, `Docs/Research/**`, `research-index`, GROWFOR, Unity, C#, assets, saves. **Not merged.** No PR was opened.
* **Branch:** `docs/883-independent-stage-d-review-20261121-v1`. The document cannot contain its own commit SHA; the exact commit, readback hash, base-comparison result and the three notification links are recorded in the #896, #883 and #425 comments that carry this review's direct links.
* **Scratch material** (rendered PNGs, text extractions, scripts) stays outside the repository; no PDF page or image is republished here.
* An approved research review would not authorise canonical publication or gameplay changes; this review is `NEEDS CHANGES`, and #425 keeps all source, context, attachment and publication decisions under its separate G4/G5 gate.
