# CCF #896 — Independent delta review of #883 corrections at `20632cb` (R-1…R-28)

**Date:** 2026-11-21. **Issue:** [#896](https://github.com/jermur788/CCF/issues/896). **Parent:** [#883](https://github.com/jermur788/CCF/issues/883). **Decision owner:** [#425](https://github.com/jermur788/CCF/issues/425).
**Status:** `INDEPENDENT DELTA REVIEW RETURNED / NEEDS CHANGES (editorial, minimal) / NOT CANONICAL / NOT PUBLISHED / NO MERGE`.
**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code, the same reviewer as the original #896 review. Not the producer (DeepSeek V4.1 Flash), a producer helper, or a B1–B3 Manager. Claim registered on #896 **before any delta work**: <https://github.com/jermur788/CCF/issues/896#issuecomment-7065780376>.

| Pin | SHA |
|---|---|
| Original reviewed producer head | `053686b590098b1abbebc6d6df7c27585956ca80` |
| Corrected producer head (reviewed here) | `20632cb03eb67c7e731de5c3981597cc854ed048` (correction commit `bd8560f1b156cc7596fe214d9ad41b455e8821df`) |
| Original independent review | `00c69cd12298043c32640ac60230a492b3eca9f0` (file SHA-256 re-read and unchanged: `cdab3dab…0b89`) |
| Manager packet | `docs/ccf-896-delta-review-20632c-manager-packet-20261121-v1` |

---

## 1. Verdict

**OVERALL DELTA VERDICT: NEEDS CHANGES — editorial only, three tiny producer fixes, no science change, no re-render needed.** 25 of 28 requirements are **CLOSED**; 3 (R-7, R-11, R-23) stay open on short text residuals. After the fixes in §8, a text-only recheck of those three lines is enough to move this to APPROVED.

| Dimension | Verdict | Basis |
|---|---|---|
| (i) Source traceability | **NEEDS CHANGES** | Page citations, offsets, Growth pointers and the 18/20 and 17-plot ambiguities are correct. Three residuals: 11's closing sentence is ambiguous and its status count is wrong (F-1); the TSV's own comment header still names `original_pdf_page` and carries a duplicated block (F-2); 09 puts the P1-derived Ae 0.133 ha under P2 page pointers (F-3). |
| (ii) Scientific quantities | **APPROVED** | Every changed number or quote I re-read on a rendered page or the text layer matches the source (§6). Two non-blocking wording refinements (N-2, N-3). |
| (iii) Coverage completeness | **APPROVED** | TSV has 68 rows, 11 fields each, unique IDs, and the status tally 46/13/5/2/1/1 and per-paper table match 02/00/11. P2-01 and P2-04 are `MATERIAL GAP`. The only wording problem on status is in file 11 and is counted under (i). |
| (iv) Practical suitability | **APPROVED** | 08 is usable as a proposed supporting-evidence composite for #425 *consideration*: the design advice is removed or labelled, rights holds are intact, 09 stays un-applied. |

An approval here is research-quality only. It does **not** release publication rights, publish 08, apply 09, change any roster, or change CCF simulation.

---

## 2. Method and what was and was not independently observed

- Fetched both producer heads from GitHub, plus the original review, and diffed `053686b5..20632cb` file by file (12 files, all under `883-Evidence-Coverage-Quality/**`; no other path in the compare).
- **Rendered pages** (Windows.Data.Pdf renderer, read-only on the owner's local original `CCF_Six_Complete_Forestry_Research_Papers.pdf`, 17,650,706 B, SHA-256 re-verified `4f1d7557…5e13`): collection **c.104, c.105, c.107** (R-22/R-12), **c.66** (P4 Table 1 symbols), **c.93** (P5 Table 2), **c.23** (P2 Table 3). Text layer used for locating pages and checking quotes (c.7, c.20, c.29, c.31, c.39, c.42, c.51, c.91, c.92, c.95, c.100–c.102, c.11).
- TSV parsed by script (not by eye): field counts, IDs, status and per-paper tallies, old-vs-new row diff.
- **Not done:** no repeat of the six-paper audit; no look at the Project attachment slots; no re-check of unchanged 08 assertions; the #860 addendum branch was not re-fetched (the producer compare shows no such path changed).

---

## 3. R-22 / P6-12 rendered oracle (the required new check)

**c.104 = P6 p. 6, rendered and read.** Right column, §3: "…there are examples in many countries, e.g. in *Pinus sylvestris* L. in Poland, in *Picea sitchensis* (Bong.) Carr. in Ireland and in *Pinus pinaster* Aiton in Spain, where clearcutting is followed by natural regeneration." Printed page number "6".

**c.107 = P6 p. 9, rendered and read.** Fig. 6 caption: "Example of the early stages of a group-strip shelterwood system with Sitka spruce (*P. sitchensis* (Bong.) Carr.) in progress. LiDAR-derived canopy height model (1-m spatial resolution) for block 3, Cefn Du, Clocaenog Forest, North Wales, UK in 2006. Modified from Pommerening and Grabarnik (2019). Courtesy of Rachel Gaulton." The image is a greyscale canopy-height model with arrows "First strip cutting with seed trees + advance regeneration", "Groups with advance regeneration", "Main wind direction". Printed page number "9".

**Findings**
1. Pages are right: `paper_page 6, 9` = c.104, c.107 under the +98 offset. **P6-12 is real and is correctly a qualitative row.** No establishment rate, probability or stocking number appears for Sitka on either page, and none is imported.
2. **Nuance the producer's wording does not yet carry.** The Irish statement is one species named in a list of country examples of clearcut-then-natural-regeneration, with no site, rate or outcome. The Welsh figure is an *early-stage*, LiDAR-only illustration from 2006 with no regeneration result reported. 08 §8 and the TSV row say "Irish clearcut followed by natural regeneration of *Picea sitchensis*"; that is a fair gist but reads stronger than the source (N-1).
3. **R-12 re-confirmed visually.** Fig. 5 caption on c.107: "dashed black lines are extraction racks"; Fig. 3B caption on c.105 (P6 p. 7): "continuous straight lines indicate extraction racks". So 08's "[P6:3–4, 7, 9]" and its statement that the regeneration-strip/rack distinction is the producer's, not P6's, are correct.

**R-22 decision: CLOSED.** The producer's note "rendered confirmation outstanding" is now superseded by this review and should be updated (N-1, non-blocking).

---

## 4. P2-01 / P2-04 and the file-11 sentence

**Confirmed in the actual matrix.** Parsed `02-coverage-matrix.tsv` (blob at `20632cb`): `P2-01 → MATERIAL GAP`, `P2-04 → MATERIAL GAP` (both previously `DISPUTED`); `O-04` is the **only** `DISPUTED` row. `02-COVERAGE-MATRIX.md` §4/§6 and `00-README.md` agree.

**File 11 closing paragraph (lines 71–73), verbatim:** "…The movement is explained by four status changes and one added row, not by redefining the taxonomy mid-stream: `DESIGN NOTE` was introduced for rows that were never source findings, and the two P2 rows left `DISPUTED` because the dispute was resolved *in favour of* the source."

My reading, stated plainly:
- "left `DISPUTED`" is **ambiguous**. Read as "departed the DISPUTED status" it is correct, but it never says where they went. Read as "were left as DISPUTED" it contradicts 11's own R-11 row, the TSV and the tally. The Manager's concern is fair: a reader of this one sentence cannot tell.
- "**four** status changes" is **wrong on the data**: the TSV diff shows **five** (P2-01, P2-04, P5-09, P2-09, P6-09).

**No other contradictory status claim remains.** I searched every file under `new/` for `DISPUTED`, `67`, `44`, `Growth §?`, `130,756`: hits are the status definition (02-MD §3), correct statements of P4-04 no longer disputed (03, 05), 10's file-level **HISTORICAL RECORD — superseded** banner covering its old 67/44/13/6/3/1 tally, correct "of the original 67 rows" uses (02, 05, 11) and the `O-04` line in 11. No unresolved `Growth §?`, no stale `130,756`.

Required fix: **F-1** (§8). Not grounds to change the correct TSV.

---

## 5. R-1 … R-28 individual closure decisions

`[B]` = blocking in the original review. Decision vocabulary: **CLOSED** / **NEEDS CHANGES** / HELD (none used).

| R | B | Decision | Evidence at `20632cb` |
|---|---|---|---|
| R-1 | B | **CLOSED** | 03 V-9 "CONFIRMED as stated; source internally inconsistent", abstract/§2.1/P5:4 = 18, P5:3 = "the 20 plots" and 16+4; "unverified" gone; same in 05 U-6, 06 §4, 10 residual #2, 08 §7, TSV P5-01 ("UNRESOLVED… never cite 18 as settled"). |
| R-2 | B | **CLOSED** | 03 V-10 quotes "40 ingrowth trees per measurement time and sample plot or 49 per hectare and year"; 08 §7 says "stated directly"; TSV P5-04. The added check 40 per 0.16 ha per 5 yr ≈ 50 ha⁻¹ yr⁻¹ is arithmetic-correct. |
| R-3 | B | **CLOSED** | 08 §7 "more than half of the causes of death remained unknown", attributed mostly wind/snow then competition/harvesting; "every observed death" removed from all files (grep). Matches P5:4/P5:11. |
| R-4 | B | **CLOSED** | Rendered c.66: **λ_B** = 0.0390; c₀, c₁ "(Equation 10)"; **d** "Mortality threshold parameter" with no equation. 08 §6, 03 V-3/V-4, TSV P4-04 all agree, including all 15 values. |
| R-5 | — | **CLOSED** | 08 §6 "freely determined… [P4:12–13]". |
| R-6 | B | **CLOSED** | 08 §4 now: no forest characteristic significantly related to κ_F; **weak positive correlation** of c̄ with v_d and k_d; authors suggest conformity rises with diameter-structure complexity; correlational, plantation sample. Matches P2:15 ("We found a weak correlation… These correlations suggest that conformity increases with increasing complexity of tree stem diameter structure"). Editorial sentences removed; TSV P2-07 rewritten. |
| R-7 | B | **NEEDS CHANGES (editorial)** | 08 §4 and TSV P2-01/P2-02 carry "0.1 ha each except Ae at 0.133 ha (P1:7)" and the 1,321/21.2/n=176 vs 1,336/21.1 note. **But 09's proposed sentence** says "marteloscopes 0.1 ha each except Ae at 0.133 ha" inside a sentence cited only to **P2** pages; 0.133 ha is a **P1 Table 1** figure (P2 Table 2 says 0.1 ha). The "(P1:7)" my R-7 asked for was dropped. See F-3. |
| R-8 | B | **CLOSED** | 09: "MART pp.2, 10 remains the secondhand report of this same agreement finding (MART PDF p.10); the rights tiers are a separate bullet (MART pp.6-7, 15, 19)". 04 §2.1 matches. |
| R-9 | B | **CLOSED** | 09, 04 §2.1, 06 §4, 03 V-11 and TSV P2-04 all use P2 pp. 1, 11, 14, 16, 17 and Table 3 p. 9; crown thinning "poor-to-slight, one fair (Loch Ard 2015, 0.44)". |
| R-10 | B | **CLOSED** | 09 names "#883 producer (V-6/V-11) and independently… #896" and adds that the addendum's author still did not read the six papers; 09 "Deliberate non-changes" keeps the addendum's own "not read" line. |
| R-11 | B | **NEEDS CHANGES (editorial)** | **Substance done**: P2-01/P2-04 `MATERIAL GAP`; P1-01 strength `PARTIAL`, "supporting literature"; totals re-derived; 00 and 06 updated; 10 carries a superseded banner. **Residual: file 11's closing sentence** (§4 above, F-1). |
| R-12 | B | **CLOSED** | TSV P6-09 `DESIGN NOTE`, strength NONE, `git grep` zero stated; 08 §8 "Safeguards — producer inference… not made in P6… Fig. 3B, Fig. 5". Both figures re-read on c.105/c.107 (§3). |
| R-13 | B | **CLOSED** | P5-09 `CROSS-REFERENCED-INTENTIONALLY`, ADJACENT, low priority; "strongest" removed everywhere except 02-MD line 120, which quotes it to say it was deleted. 08 row quotes the source ("showed a performance similar to the selected RBA with very little difference") verified c.95. |
| R-14 | B | **CLOSED** | P2-09 → `DESIGN NOTE` (P2 silent on automation; P2:2 only notes machine operators select trees); P2-08 location → "none (design inference; supported in part by P2:17)"; removed from the cross-reference count. See N-5 on P2-08's status. |
| R-15 | — | **CLOSED** | No `§?` left. P5-08 → Growth §6 and §9; O-06 → Growth §9. Section headings re-checked in the canonical Growth file (§6 windthrow, §9 GROWFOR; no adult-mortality section). |
| R-16 | B | **CLOSED** | 08 §5 table has no Handling column and states "makes no recommendation about which, if any, belongs in the game"; "without improving recruitment" replaced by "says nothing about whether either arrangement regenerates better"; §4 first bullet "Producer inference, not a P2 finding", anchored to P2:17. |
| R-17 | — | **CLOSED** | 08 §3 and §9: Brownhill ~250 future parent/frame trees ha⁻¹ beside P1's ~100/ha, "not reconciled… neither is an Irish target". Re-found in Practice composite lines 73 and 167. |
| R-18 | — | **CLOSED** | 08 §3 "Absolute levels (P1:9)": low thinning fair, frame-tree selection slight, competitors slight; "in all cases agreement was low and did not exceed one case of moderate agreement"; 13 in the fair class. Quote and "13 cases in the fair-agreement class" found on c.11. Heading no longer "strongest". |
| R-19 | — | **CLOSED** | 08 §5 T_ij classes left-closed; matches c.39 "0.3 ≤ T < 0.5 … 0.7 ≤ T ≤ 1.0". |
| R-20 | — | **CLOSED** | Source sentence quoted verbatim in 03 V-1, 08 §6, TSV P4-02, 05 U-6b; "do not state 17 in total as fact". Matches the rendered c.66 sentence ("a total of 17 plots at each of the five sites"). |
| R-21 | — | **CLOSED** | 02-MD §2 and 05 U-8 now 132,717 B (131,136 chars) / 135,214 B; `130,756` survives only in the *old* tree. 00 never carried the figure. Arithmetic: 132,717 − 33,058 + 35,555 = 135,214 ✓. |
| R-22 | — | **CLOSED** | §3: both pages rendered; wording and pages confirmed; no rates imported; 02-MD §7 and 07 §4 state the rows are not exhaustive; P6-11 harmonised to cross-reference in TSV and narrative; P3-08 reframed as a caveat. Wording refinement N-1 is non-blocking. |
| R-23 | — | **NEEDS CHANGES (editorial)** | Column header row is `paper_page`, offsets are documented in the TSV and 02-MD §7 and 08 §1, P4 coincidence noted. **But** TSV line 2 still reads `# Columns: row_id, source, original_pdf_page, …`, and the comment block has been inserted twice (lines 3–6 and 8–11), with a stray ` \| DESIGN NOTE` on line 7. See F-2. |
| R-24 | — | **CLOSED** | 03 has a Page column; each verbatim quote carries paper page and collection page; rendered readings attributed to #896. |
| R-25 | — | **CLOSED (as an explicit non-claim)** | 08 §3 marks A = 4–8 **NOT VERIFIED**; 05 U-11 repeats it. Text-layer corroboration only, not a render: c.7 (P1:5) says "Johann (1982) defined discrete values from 4 to 8 with decreasing thinning intensity, i.e. 4 – very heavy and 8 – very weak". That is consistent with 08 but remains un-rendered. |
| R-26 | — | **CLOSED** | 08 row names plot VES01, ±1.96σ, 25.1/26.8/30.5 and the 11–21 m site-index equivalence (c.93 and c.95 re-read). Two precision caveats (my own earlier "mean site index" reading, and "range" vs "variability") are recorded in N-2; neither changes a number. |
| R-27 | — | **CLOSED** | Slenderness/h-d → Growth §6; Hegyi → Growth §5 (+ §8, Ecology §2). Canonical Growth section headings re-confirmed. |
| R-28 | — | **CLOSED** | "bare land" appears in P6 text (c.100 "bare land."; c.101/c.102 "bare-land principle"); 08 §8 and TSV P6-01 use it and note the extraction's "open". Text-layer confirmation. |

**Tally:** CLOSED 25 (blocking closed 12 of 14; non-blocking closed 13 of 14). NEEDS CHANGES 3: **R-7 [B], R-11 [B], R-23**. HELD 0.

---

## 6. Numeric and wording checks beyond the R-list

| Check | Result |
|---|---|
| P4 Table 1, rendered c.66 | All 15 values and symbols match 08 §6 and TSV P4-04: k 1.1287, q −0.0387, α 0.2478, β 1.6060, δ 0.0747, ν 96.3815, a 0.5697, b 9.1091, c₀ 1.0200, c₁ 0.0200, d 0.0060, λ_B 0.0390, f 0.0900, R 0.4000, S 1.3000. The producer's added Greek symbols are correct. "α strength; β, δ ranges" matches the descriptions. |
| P2 Table 3, rendered c.23 | 08 changed the top class from `> 0.90` to `≥ 0.90`. The rendered table reads **≥ 0.90 almost perfect**. The unrequested edit is correct. |
| P5 Table 2, rendered c.93 | 23.5 (19.8, 25.9) / 26.8 (22.7, 29.5) / 32.3 (27.7, 36); zero-effect column 25.1 / 26.8 / 30.5; matches 08. See N-2 on the "mean SI" label. |
| P5 15-year method text, c.92 | SI 25.9 min / 28.4 mean / 33 max; plot effect 0 or ±1.96σ with σ = 0.228 cm; "BA varied from 25 to 31 … plot effect 23 to 32". Consistent with 08. |
| P3 new verbatim phrases (08 §5) | "may not correspond to a particular biological meaning" (c.42), "difficult, if not impossible, to define" (c.42), "randomness as a reference is no substitute for naturalness", "something special has been detected" (c.51): all present in the source. |
| P2 phrases (08 §4) | "putting all eggs in one basket" (c.31, P2:17) present. "rather low, which is typical of plantations" is present but **truncated** (N-3). |
| P1:9 quotes (08 §3) | "in all cases agreement was low and did not exceed one case of moderate agreement" and "13 cases in the fair-agreement class" present on c.11. |
| 03 V-10 arithmetic | 40 per 0.16 ha per five-year interval = 50 ha⁻¹ yr⁻¹, consistent with the source's 49. |

---

## 7. Coverage, TSV and text consistency

- **TSV (script-parsed):** 11-field header; 68 data rows; **0 rows with the wrong field count; 0 duplicate IDs**; leading `#` comment lines excluded. Statuses: **MATERIAL GAP 46, CROSS-REFERENCED-INTENTIONALLY 13, COVERED 5, DESIGN NOTE 2, DISPUTED 1, OUT OF SCOPE 1 = 68.**
- **Per paper (TSV = 02-MD §4):** P1 11 (7 gap, 4 xref); P2 9 (7/1 xref/1 design); P3 8 (8); P4 7 (6/1); P5 14 (10/4); P6 12 (7/3/1 covered/1 design); Other 7 (1 gap, 4 covered, 1 disputed, 1 OOS). Totals match 00, 02, 11.
- **Old → new reconciliation (script diff):** gap 44 +P2-01 +P2-04 −P5-09 +P6-12 = 46; xref 13 +P5-09 −P2-09 = 13; covered 6 −P6-09 = 5; disputed 3 −P2-01 −P2-04 = 1; two DESIGN NOTE rows from P2-09 and P6-09. The tally is explained **without** residue; there are five status changes plus one added row.
- **Changed rows I examined (19 existing + the added P6-12):** P1-01, P1-10, P2-01, P2-02, P2-04, P2-07, P2-08, P2-09, P4-02, P4-04, P5-01, P5-04, P5-05, P5-08, P5-09, P6-01, P6-09, P6-11, O-06. Each matches its R-disposition.
- **No stale 67-row or 44-gap claim in current-state text** (only historical in 10, labelled superseded). No `Growth §?`.
- **P6-12 is not covered by any composite:** the only Sitka-regeneration lines in the four composites are Practice L282 (Davies & Kerr, a different source). Strength NONE is defensible; ADJACENT would also be (N-1).
- **Attribution honesty (07 check 6):** no producer file claims it rendered a page; every rendered reading is attributed to #896.
- **Scope:** compare `053686b5…20632cb` lists 12 files, all under `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**`.

---

## 8. Required producer fixes (minimal; no science, no re-render)

**F-1 (closes R-11).** In `11-R1-R28-DISPOSITION-AND-CORRECTION-HANDOFF.md`, replace the sentence at lines 71–73 with:
> This replaces the 67 / 44 / 13 / 6 / 3 / 1 tally. The movement is five status changes (`P2-01` and `P2-04` `DISPUTED` → `MATERIAL GAP`; `P5-09` `MATERIAL GAP` → `CROSS-REFERENCED-INTENTIONALLY`; `P2-09` and `P6-09` → `DESIGN NOTE`) and one added row (`P6-12`). `DESIGN NOTE` was introduced for rows that were never source findings. The two P2 rows are no longer disputed because verification resolved the dispute in favour of the source; only `O-04` remains `DISPUTED`.

**F-2 (closes R-23).** In `02-coverage-matrix.tsv`: change line 2 to `paper_page`; delete the duplicated comment block (lines 8–11) and the stray ` | DESIGN NOTE` suffix on line 7, leaving each comment once.

**F-3 (closes R-7).** In `09-PROPOSED-860-ADDENDUM-P2-CHANGE.md` proposed text, add the P1 source for the plot size, e.g. "marteloscopes 0.1 ha each (Ae 0.133 ha per P1 Table 1, p. 7; P2 Table 2 gives 0.1 ha)", and mirror in the "Study description" row of the changes table.

### Non-blocking recommendations (same pass if convenient; do not gate closure)

- **N-1 (P6-12 / 08 §8).** Update the row note to say the pages were rendered and confirmed by this delta review (c.104, c.107). Soften to: "P6 lists *Picea sitchensis* in Ireland among examples where clearcutting is followed by natural regeneration (no site, rate or outcome), and shows an early-stage 2006 LiDAR illustration of a group-strip shelterwood at Cefn Du, Clocaenog (no outcome reported)."
- **N-2 (08 §7 Table 2 row; my own earlier wording).** The rendered table **merges** the ±1.96σ cells across the Minimum/Mean/Maximum SI rows, and the text does not say which SI they use. "Mean site index" is a reasonable reading (26.8 is the zero-effect mean case) but is an inference, and my original Q-4 stated it more firmly than the render supports. Add "(Table 2 merges these cells; the SI used is not stated and is taken here as the mean case)". Separately, 08 says the plot-effect variation is "equivalent to roughly an 11–21 m site-index **range**". The source (c.95, P5:11) says the plot effects lead to a variability of stem-diameter **growth** "similar to that which is caused by a 11–21 m variability of the site index" (c.91 gives the 95% interval as "14-m (11–20 m)"). It is a width of spread in site index, applied to diameter growth, not a range of site-index values from 11 to 21 m. Prefer "an 11–21 m **variability (spread)** in site index, in terms of diameter growth".
- **N-3 (08 §4).** Complete the P2:6 quote: "…typical of plantations that have received little management".
- **N-4 (04 §2 table).** Row 2 still lists "training-design implications" and D-065 as P2 content/cross-reference, and row 6 still says "shelter/edge/open". Align with 08 ("producer inference"; "bare land").
- **N-5 (TSV P2-08).** Status stays `CROSS-REFERENCED-INTENTIONALLY` with `composite_location` = "none" and class DESIGN. By the same logic that moved P2-09, a design inference with no home is arguably a `DESIGN NOTE`. Producer's call; it would change the tally to 12/3.
- **N-6 (08 header).** Line 3 still says "NOT REVIEWED". Replace with "independently reviewed (#896, NEEDS CHANGES → delta review); not approved" at the next edit.
- **N-7 (TSV O-04).** `coverage_strength` = `DRAFT`, outside the declared NONE/ADJACENT/PARTIAL/DIRECT vocabulary (pre-existing, not introduced in this delta).

---

## 9. Design/evidence distinctions and rights (preserved)

- 08 is still labelled **PROPOSED — NOT CANONICAL, NOT ATTACHED, NOT A DECISION** and "no gameplay coefficient, threshold, score or UI string" (§10). The Handling column, the training-intent list (now "producer inference") and the recruitment assertion no longer present design advice as evidence. P6's rack/strip distinction and P2's delegation boundary are labelled producer inference or `GAP`, and carry **DESIGN NOTE** status in the matrix.
- 09 is still an un-applied, exact one-bullet proposal; it says so at the top. It does not touch the other eight #860 reports or their conflicts.
- **Rights hold preserved verbatim in effect:** 08 §9 box, 05 U-10, 06 §5, 11 "Not a licence decision". P1/P2/P5 CC BY, **P4 and P6 CC BY-NC-ND**, **P3 © Institute of Chartered Foresters**; 08 transcribes all 15 P4 Table 1 values and paraphrases substantially; whether that may sit in a Project attachment is **#425/owner**'s decision. No redistribution right is inferred here either.
- **Retained limits (unchanged by this delta, plus mine):** 8 of 77 original 08 assertions and 19 of 67 original matrix rows were never audited (this delta touched `P6-01`, which the original review listed among the rows it had not individually examined, and checked it on the text layer only); Project attachment bytes were not opened; stale Growth attachment (33,058 B vs canonical 35,555 B) and the three composites with no Git counterpart are untouched #425 matters. No complete-audit claim is made.

---

## 10. Residual unknowns

| Unknown | Impact |
|---|---|
| P5 18 vs 20 plots (inside the paper); P4 "17 plots at each of the five sites" | Deliberately preserved as unresolved; blocks any "settled sample size" claim only. |
| A = 4–8 in P1:5 not rendered | Text-layer agrees; 08 still labels it NOT VERIFIED. Render once if the figure is ever reused. |
| Which SI the ±1.96σ cells in P5 Table 2 assume | Text silent (N-2); low impact. |
| R-28 "bare land" and 04's P2/P6 wording checked on the text layer only | Low; rendered checks were spent on the contested pages. |
| 19 original rows and 8 original 08 assertions unreviewed; Project attachments unopened; #860 branch not re-fetched | Limits "fully verified" claims, not the delta verdict. |
| My own reading of rendered images (single reader) | Every rendered digit was cross-compared with the text layer where one exists; none disagreed. |

---

## 11. Next handoff

**NEXT: producer applies F-1 to F-3 (three short text edits; N-items optional) and returns a new SHA.** A **text-only recheck** of those three lines (≈5 minutes, no PDFs) can then convert this to **APPROVED**. After that, **Manager #425 decision packet**: rights/licensing hold on 08, Project attachment slots, whether and when to apply 09, and any canonical placement. Nothing in this review authorises canonical publication, an attachment change, a composite rewrite, a Drive mirror change, a #860 addendum edit, any mechanic, coefficient or UI text, or redistribution of the six papers.

## 12. Publication record

- **Branch:** `docs/883-independent-stage-d-delta-review-20261121-v1`, created from main as read at publish time; one added file, nothing else. No PR, no merge.
- **File:** `Docs/Handoffs/Research-ProjectSources-20261120/883-Independent-StageD-Review/CCF-896-DELTA-REVIEW-OF-20632CB-RESULT.md`.
- The document cannot contain its own commit SHA. The exact commit, byte-identical readback hash, base comparison and the three notification links are recorded in the #896, #883 and #425 comments that carry this review's direct link.
- **Reviewer disclosures:** claim posted before delta work began (unlike the original review); provider is Claude Sonnet 5.5, not the GPT-6 High class the original packet suggested.
