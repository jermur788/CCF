# 11 — #896 Stage D corrections: R-1…R-28 disposition and correction handoff

**Status:** `RESEARCH PRODUCED`. **NOT** independently re-reviewed, **NOT** canonically approved, **NOT** published.
**Lane:** `research/883-condensed-evidence-coverage-review-20261120-v1`
**Base (original producer):** `053686b590098b1abbebc6d6df7c27585956ca80`
**Independent review:** #896, commit `00c69cd12298043c32640ac60230a492b3eca9f0` —
`CCF-896-INDEPENDENT-SCIENTIFIC-COVERAGE-AND-STAGE-C-REVIEW-RESULT.md`
**Verdict being answered:** `NEEDS CHANGES`, 28 corrections, 14 blocking.
**Correction commit:** `bd8560f1b156cc7596fe214d9ad41b455e8821df` — this file's own commit contains every R-1…R-28
change; the SHA is recorded here in a follow-up docs commit because a file cannot embed its own commit hash.
**Write scope observed:** `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**` only.

## How to read this map

`CORRECTED` = the requested change is applied in the named file. `PARTIAL` = applied with a stated limit, flagged
for the delta review. `HELD` = deliberately not done, with cause. No item is silently dropped, and no reviewer
judgement is overruled.

**Evidence dependence, stated once and applying to every row:** this producer has **no PDF renderer**. Corrections
that depend on rendered-page readings use the #896 review's rendered findings and are attributed to it. This
producer claims **no personal rendered-page verification** anywhere in the packet. Wherever ambiguity exists in the
source (P5 18 vs 20; P4 "17 plots at each of") it is preserved as unresolved rather than resolved.

## Disposition map

| R | B | File(s) | Exact change applied | Evidence used | Status |
|---|---|---|---|---|---|
| R-1 | **B** | `03` V-9; `05` U-6; `10` residual #2; `08` §7; TSV `P5-01` | Relabelled: "CONFIRMED as stated; source internally inconsistent" — 18 in abstract/§2.1/P5:4, 20 in P5:3 (16 thinned + 4 unthinned). "Unverified" removed. | c.85, c.87, c.88 (#896 rendered) | CORRECTED |
| R-2 | **B** | `03` V-10; `10` residual #3; TSV `P5-04`; `08` §7 | "3,360 trees ≥1.3 m; on average 40 ingrowth trees per measurement time and sample plot, or 49 per hectare and year (P5:4)". "the extraction quotes" removed. | c.88 (#896 rendered) | CORRECTED |
| R-3 | **B** | `08` §7 model structure; `08` §7 mortality bullet; TSV `P5-08` | "more than half of the causes of death remained unknown [P5:11]; where attributed, mostly wind and snow, then competition and harvesting damage [P5:4]". "every observed death" removed. | c.95, c.88 | CORRECTED |
| R-4 | **B** | `08` §6 Table 1; `03` V-4; TSV `P4-04` | Recruitment symbol → **λ_B**. Mortality row: (Eq. 10) attached to c₀/c₁ only; d marked "no equation cited". | c.66 (#896 rendered) | CORRECTED |
| R-5 | — | `08` §6 | "freely determined" citation → `[P4:12–13]`. | c.67 | CORRECTED |
| R-6 | **B** | `08` §4 ×2 bullets; TSV `P2-07` | Rewritten: κ_F unrelated to forest characteristics; **weak positive correlation** of c̄ with v_d and k_d, authors suggest conformity rises with diameter-structure complexity; correlational, plantation sample (v_d 0.24–0.37). Editorial "does not support either direction" and the "prevents claims" action deleted. | c.29, c.20 | CORRECTED |
| R-7 | **B** | `08` §4 design; `08` Table 2 note; `09`; TSV `P2-01`, `P2-02` | "0.1 ha each **except Ae at 0.133 ha** (P1:7)". Added P1 Table 1 Ae 1,321/ha, h₁₀₀ 21.2 m, n = 176 vs P2 1,336/21.1 — different year/area, neither is "the" Ae inventory. | c.9, c.19, c.20 | CORRECTED |
| R-8 | **B** | `09` | MART clause replaced: MART pp. 2, 10 is the secondhand report of **this same agreement finding** (p. 10); rights tiers are a separate bullet (pp. 6-7, 15, 19). | MART PDF pp. 2, 10 | CORRECTED |
| R-9 | **B** | `09`; `04` §2.1; `06` §4 | Page pointers → P2 pp. 1 (abstract), 11 (p < 0.001), 14 (κ_F ranges; Loch Ard 2015 κ_F = 0.44), 16 (Fig. 7: poor 4, slight 14, fair 17, moderate 1), 17 (4 of 36 poor); Table 3 p. 9. Crown thinning described as poor-to-slight with the one fair exception. | c.15, 25, 28, 30, 31 | CORRECTED |
| R-10 | **B** | `09` | Verifiers named ("#883 producer V-6/V-11; independently #896") with the artefact; note added that the addendum author still did not read the six papers. | addendum line 78 | CORRECTED |
| R-11 | **B** | TSV + `02` §4/§6.1; `00`; `06`; `10` | `P2-01`, `P2-04` → `MATERIAL GAP` (resolved by verification); `P1-01` strength `ADJACENT` → `PARTIAL` and "evidenced basis" → "supporting literature". Totals re-derived. | §4.2, §6.2 | CORRECTED |
| R-12 | **B** | TSV `P6-09`; `02` §6.3; `08` §8 | P6-09 no longer `COVERED` → `DESIGN NOTE` with strength NONE. `08` now states the distinction is the producer's, not P6's, and that P6's own figures (Fig. 3B, Fig. 5) draw extraction racks inside strip/wedge/group systems. | git grep zero; c.105, c.107 | CORRECTED |
| R-13 | **B** | TSV `P5-09`; `02` §6.4; `08` §7 row | → `CROSS-REFERENCED-INTENTIONALLY`, ADJACENT, low priority. "strongest published argument/justification" deleted from all three; replaced with the qualitative comparison wording and "says nothing about CCF's Hegyi treatment". | c.95 | CORRECTED |
| R-14 | **B** | TSV `P2-08`, `P2-09`; `02` §6.1/§6.2 | `P2-09` → `DESIGN NOTE` ("P2 silent on automation/delegation; P2:2 only notes machine operators select trees in some countries"); removed from the cross-reference count. `P2-08` location → "none (design inference; supported in part by P2:17)". | grep c.15–34 | CORRECTED |
| R-15 | — | TSV `P5-08`, `O-06` | Placeholder "Growth §?" replaced: P5-08 → "Growth §6 (windthrow/stability) and §9 (GROWFOR pointer); Growth has no adult-mortality section"; O-06 → "Growth §9 (GROWFOR/species pointer); research-index". | TSV | CORRECTED |
| R-16 | **B** | `08` §5 table; `08` §5 illustration; `08` §4 use-and-limits | "Handling" column deleted; table now definition/page/class with an explicit statement that no product recommendation is made. "…without improving recruitment" replaced by "the formula says nothing about whether either arrangement regenerates better". The training-intent list is labelled **"Producer inference, not a P2 finding"** and anchored to P2:17. | §7.3 | CORRECTED |
| R-17 | — | `08` §3 density table; `08` §9 | Brownhill ~250 future parent/frame trees ha⁻¹ (measured Irish trial, Practice §3–§4) added beside P1's ~100/ha rule of thumb, with an explicit "not reconciled; neither is an Irish target" note, plus a §9 cross-reference row. | Practice L167 | CORRECTED |
| R-18 | — | `08` §3 | Absolute levels added (P1:9): low thinning fair, frame-tree selection slight (no poor case, 13 fair), competitors slight; "in all cases agreement was low and did not exceed one case of moderate agreement". Section heading no longer calls it "the strongest teaching evidence". | c.11 | CORRECTED |
| R-19 | — | `08` §5 T_ij | Classes → left-closed 0 ≤ T < 0.3, 0.3 ≤ T < 0.5, 0.5 ≤ T < 0.7, 0.7 ≤ T ≤ 1.0. | c.39 | CORRECTED |
| R-20 | — | `03` V-1; `08` §6; TSV `P4-02` | Source sentence quoted verbatim and the 17-total vs 17-per-site ambiguity marked **unresolved**, with "do not state 17 in total as fact". | c.66 | CORRECTED |
| R-21 | — | `02` §2; `05` U-8 | Byte figures corrected: four attachment copies 132,717 B (131,136 characters); 135,214 B with the canonical Growth composite. | measured | CORRECTED |
| R-22 | — | `02` §6.5/§6.6 + TSV `P6-11`, `P6-12`; `07` §4 | P3-08 framed as a caveat to P3-01…07 rather than an independent finding; P6-11 harmonised to cross-reference in both TSV and narrative; P6 Sitka mentions added as candidate row **P6-12** and as a note in `08` §8; "the 68 rows are not exhaustive" stated in `02` §7 and `07` §4. | c.104, c.107 | **PARTIAL** — P6-12 is added and flagged, but its pages come from #896's **text-layer** reading; rendered confirmation is outstanding and the row says so. |
| R-23 | — | TSV header + `02` §7 | `original_pdf_page` → `paper_page`; collection offsets documented (P1 +2, P2 +14, P3 +34, P4 +54, P5 +84, P6 +98) with the P4 p.12 coincidence noted. | P2 Table 2: p.6 = c.20 | CORRECTED |
| R-24 | — | `03` | A page column added and each verbatim quote now carries its page (P4:12/c.66, P5:4/c.88, P2:6/c.20, P1:7/c.9, etc.), attributed to #896's rendered reading where that is the source. | §3 | CORRECTED |
| R-25 | — | `08` §3 A-thinning row | Marked **NOT VERIFIED**: "the A = 4–8 range and its direction were taken from the extraction and must be confirmed on the rendered equation page before reuse". | not checked by either party | CORRECTED (as an explicit non-claim) |
| R-26 | — | `08` §7 Table 2 row | Plot **VES01**, mean site index; ±1.96σ plot effect; site-index spread at zero plot effect 25.1 / 26.8 / 30.5 m²/ha; note that the plot-effect variation is roughly an 11–21 m site-index range. | c.93, c.95 | CORRECTED |
| R-27 | — | `08` §9; TSV `P1-10`, `P5-05`, `P5-09` | Slenderness/h-d pointer corrected to **Growth §6** (windthrow and stability), not §5. Hegyi pointer corrected to **Growth §5** (competition multiplier) plus §8 and Ecology §2. | Growth §5, §6 | CORRECTED |
| R-28 | — | TSV `P6-01`; `08` §8 | Third principle named **"bare land"** (the paper's term), with a parenthetical that the extraction renders it "open". | c.100 / P6:2 | CORRECTED |

## Resulting state

| Item | Value |
|---|---|
| Matrix rows | **68** (67 original + candidate `P6-12`) |
| MATERIAL GAP | **46** |
| CROSS-REFERENCED-INTENTIONALLY | **13** |
| COVERED | **5** |
| DESIGN NOTE | **2** (`P2-09`, `P6-09`) |
| DISPUTED | **1** (`O-04`) |
| OUT OF SCOPE | **1** (`O-06`) |
| Arithmetic | 46 + 13 + 5 + 2 + 1 + 1 = **68** |
| TSV integrity | one row per ID, no duplicates, 11 fields per row (verified programmatically) |

This replaces the 67 / 44 / 13 / 6 / 3 / 1 tally. The movement is five status changes (`P2-01` and `P2-04`
`DISPUTED` → `MATERIAL GAP`; `P5-09` `MATERIAL GAP` → `CROSS-REFERENCED-INTENTIONALLY`; `P2-09` and `P6-09` →
`DESIGN NOTE`) and one added row (`P6-12`). `DESIGN NOTE` was introduced for rows that were never source findings.
The two P2 rows are no longer disputed because verification resolved the dispute in favour of the source; only
`O-04` remains `DISPUTED`.

## Persisting unresolved items (unchanged by this pass)

P5 18 vs 20 (U-6); P4 "17 plots at each of" (U-6b); #860 nine-report conflicts (U-4); 2025 Yearbook (U-5);
topic-level sweep (U-8); CC BY-NC-ND licensing hold on reproducing Table 1 (U-10); partial verification coverage —
8 of 77 assertions and 19 of 67 rows not re-audited (U-11); Project attachment bytes unconfirmed (U-12); stale
Growth attachment and three composites without Git counterparts (U-1, U-2).

## What this pass does not claim

- Not `APPROVED`. The #896 review was `NEEDS CHANGES`; this is the producer's answer, and delta closure is the
  reviewer's.
- Not canonical. No composite, addendum, attachment, Drive object or main-branch change was made.
- Not a licence decision. Publication of `08` is held (U-10).
- Not rendered verification. Every rendered reading in the packet is attributed to #896.
