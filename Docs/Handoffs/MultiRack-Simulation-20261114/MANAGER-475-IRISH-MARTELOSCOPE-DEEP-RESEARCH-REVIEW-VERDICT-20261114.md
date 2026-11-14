# Independent Manager review — #475 Irish marteloscope Deep Research data-readiness result

**Date:** 14 November 2026  
**Reviewer claim:** [#478](https://github.com/jermur788/CCF/issues/478); OpenAI GPT-6 GitHub/web reviewer, not the original OpenAI Deep Research author. GitHub/browser only; worktree NONE.  
**Research submission:** [Deep Research returned text preserved in its exactly assigned #475 result path](https://github.com/jermur788/CCF/blob/research/irish-marteloscope-inventory-and-rights-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/IRISH-MARTELOSCOPE-INVENTORY-COMPLETENESS-RIGHTS-AND-CONTACT-SHORTLIST-20261114.md). The returned pasted text is preserved verbatim after a small publication provenance preface. It is not independently rerun research.  
**Verdict: NEEDS CHANGES — research-evidence quality and exact-reconstruction GO are BLOCKED; promising *leads* retained.** No rejection of the owner's “authentic Irish starting forest” idea; no production or field-contact authorisation.  
**Scope of this review:** provenance, internal consistency, quick first-party cross-checks and transparent stop/go. We did **not** download/parse the GBIF Darwin Core archive, verify any tree IDs, run an importer, contact stewards, run CCF/Samsara or invoke Unity.

## 1. Decisions for the user

1. **Do not yet select Curraghchase, Donadea, Oughaval or Glenart as a verified import-ready real plot.** The source report assigns all four “VERIFIED RAW-DATA,” yet says explicitly that it **did not download** the underlying raw archive and found no named public GBIF listing for Curraghchase. Its per-site conclusion is unsubstantiated.
2. **The strongest documented open-data lead is genuine.** The [GBIF TreMs dataset](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a) is labelled **CC BY 4.0**, and the [2022 data paper](https://doi.org/10.3897/BDJ.10.e91385) says a publicly released subset includes tree plot positions, species, DBH, height and living/dead status, from 42,078 trees at 111 plots (historical edition). Important: the same paper's **Table 2 lists IRELAND = 1 plot**, not one or more each at Curraghchase/Donadea/Oughaval/Glenart. The exact name, tree IDs, species, polygon, coordinate notation and count for that lone 2022 Irish plot remain **UNKNOWN** here. A later archive version could differ; inspect its actual version and rows, not extrapolate from a multi-plot network's site list.
3. **Teagasc Oak Park/Curragh Chase are confirmed named measured training plots**, and an official Teagasc contact is publicly verified. That is good acquisition potential, not an unlocked 2D source. The reported Sitka species on each subplot and full per-tree export/ownership need source evidence.
4. **No candidate in this research currently provides proven individual-tree longitudinal ecological validation.** Re-establishment, retagging or a new marteloscope does **not** prove dated same-stem remeasurement after *executed* thinning. Stand-level repeat inventories may support separate aggregate validation.
5. **Recommended path:** one bounded *raw Irish GBIF dataset audit* first, in a new offline-only public-data research claim, using the official archive. In parallel, preserve the two planned future contacts **unsent**, with the Teagasc route preferred. If there is no valid Irish Sitka full-XY dataset, a clearly labelled **synthetic Sitka flat stand** is the honest initial ecological-test fixture, not an “exact” real marteloscope.

## 2. What is supported by first-party evidence

| Claim | Best inspected supporting source | Responsible current label |
|---|---|---|
| A field-mapped Irish network exists with Oak Park conifer and broadleaf and Curragh Chase. | [Teagasc marteloscope page](https://teagasc.ie/crops/forestry/advice/management/continuous-cover-forestry/the-marteloscope-training-network/): three Teagasc/ six Coillte plots **as stated in that article**; mapped numbered and measured trees. | **DOCUMENTED SITE**; no downloadable complete tree list demonstrated. Nine is a **publication-era** report, not necessarily up-to-date total. |
| Official public contact for the Teagasc training programme. | [Teagasc Jonathan Spazzi profile](https://teagasc.ie/people/jonathan-spazzi/), [Teagasc course page](https://teagasc.ie/crops/forestry/news/forest-management-workshops-2024-php/). | **VERIFIED OFFICIAL PUBLIC ROUTE**. His departmental involvement is established; raw data stewardship/authority to license still **UNKNOWN**. |
| Integrate GBIF public dataset offers tree-level spatial and mensuration fields. | [GBIF dataset](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a), [Zudin et al. 2022](https://doi.org/10.3897/BDJ.10.e91385), [INRAE account](https://www.inrae.fr/en/news/toward-better-understanding-tree-related-microhabitats). | **VERIFIED DATASET-LEVEL DESCRIPTION + CC BY 4.0**; raw Irish rows/plot details **NOT INSPECTED**. Dataset is *a public subset of I+*, not the complete private I+ repository. |
| Exact 2022 geographic distribution. | [Zudin et al. 2022, Table 2](https://pmc.ncbi.nlm.nih.gov/articles/PMC9836569/), indexed official paper; 111 plots, 19 countries; **Ireland 1**. | **VERIFIED PUBLISHED HISTORICAL COUNT**, not the present/latest GBIF country tally. The returned report's four named per-plot assertions are **not supported by this source**. |
| GBIF internal structure. | [Zudin et al. 2022, Table of data resources](https://www.sciencedirect.com/org/science/article/pii/S1314283622002421): two DwC-A tab-separated files \`occurrence.txt\` (tree rows and site/plot metadata) and \`measurementorfact.txt\` (TreMs and additional diameter/height measurements) and \`verbatimCoordinates\` (tree coordinates in plot). Inclusion **DBH >=7.5 cm** in this dataset. | **VERIFIED SCHEMA DESCRIPTION**, not per-plot column completeness; report's exact “polar distance + angle” assertion is **NOT VERIFIED from a decoded record**. Note \`decimalLatitude/Longitude\` describe the **plot** coordinate; do not treat them as individual stem XY. |
| Shared licences for data. | [GBIF official record](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a) — CC BY 4.0. | **PUBLIC LICENSE WITH CONDITIONS**, NOT “public domain”; attribution/source/version necessary, and other restricted plot data are not covered. |
| Suggested Coillte contact Ken Sweeney. | His name appears as **Sweeny K** among GBIF source contributors and another public forestry reference; a private/social profile was found, but no independent official Coillte source verifying the report's quoted email **in this review**. | **CONTACT LEAD**, not a verified official address. Prefer Coillte published institutional route if needed; do not send any message now. |

## 3. Blocking and material findings

### B-1 — False “VERIFIED RAW-DATA” classification for multiple named plots — BLOCKING

The source claims raw-verified Curraghchase, Donadea, Oughaval, Glenart and states their data and complete polygons are accessible in GBIF. But it also admits **no archive download** and **no Curraghchase plot listing** was found. A site's inclusion in the wider Integrate network **does not** entail inclusion in the CC-BY released GBIF subset. The primary published 2022 Table 2 lists **only one Irish marteloscope** in the publicly released 111 plots.

**Required correction:** inspect the current archive/release and enumerate \`country=Ireland\` observations, distinct original I+ plot IDs, genuine species/DBH/XY fields, plot area and inclusion date/rights. If inaccessible: downgrade all named sites to DOCUMENTATION-ONLY or PLAUSIBLE LEADS, and return **CONDITIONAL**, not import-ready. Do not guess which plot was the single published Irish one.

### B-2 — No site-level tree-ID/XY/DBH/polygon audit or provenance — BLOCKING

No underlying tree rows, original IDs, site count, coordinate values, coordinate origin or polygon vertices are shown, even though #475 demanded a data field audit. “Polar coordinates can convert to XY” is a geometry *possibility*, not a verified import. The schema's \`verbatimCoordinates\` needs actual record parsing and metadata meaning (angle conventions, reference survey origin, units and accuracy). A square 1ha educational training plot is not proof of *that one dataset* being a valid 100×100m measured polygon with all qualifying trees.

**Required correction:** fetch licensed raw subset; show dataset version, table keys and joins, exact Irish plot stem count, missingness and geometrical unit/transform audit; calculate unique tree count, BA m² and BA m²/ha from *that same eligible sampled area*; document any missing boundary and stop without fabricated XY.

### B-3 — Reuse rights overstated — MAJOR

Report states “rights are CC BY (public domain data)” and extends dataset rights to named forest sites. **CC BY is not public domain**; it grants reuse subject to attribution and terms, and only for the specific licensed data. The public dataset does not license all withheld I+ trees, images, timber score workbooks, activity marks, field coordinate materials or third-party game artwork.

**Required correction:** classify rights for each dataset version/record, derived maps, internal research, externally published tree maps and a commercial game separately; request specific permission for additional custodial material. No automatic licence transfer.

### B-4 — Unsupported longitudinal inference — MAJOR

The report ranks Oughaval ahead for potential future accuracy because a training plot was “updated” in 2020/BIOForest; a date/update is not evidence of dated **same-tree** DBH/height/survival repeated *after actual executed thinning*. All ranked future ecological validation remains speculative. Workshopped tree marks are virtual decisions, not a harvested-treatment record.

**Required correction:** require field measurement dates, persistent IDs, measured changed DBH/height, documented real cut dates/IDs and site/wind/soil metadata, and an independent validation holdout. Unknown until a custodian confirms.

### B-5 — Synthetic stand numbers incompatible without hidden diameter variance — MAJOR

Report suggests **400–500 stems/ha**, *average* DBH **25–30cm**, and **50–60m²/ha BA**, attaching an untraceable source reference **[85]**. At equal diameters 25–30cm, 400–500 trees imply approx **19.6–35.3 m²/ha**, not 50–60. With that stem count, BA 50–60 would require quadratic mean diameter roughly **35.7–43.7cm**, or a broad explicitly specified diameter distribution with substantial large trees. An ordinary mean DBH cannot determine BA alone, and yield-class 17 m³/ha/year is not inferred merely from these attributes.

**Required correction:** remove these asserted numeric specifications until pinned Irish evidence and a coherent N/BA/Dq tree distribution support them; synthetic geometry can be generated later but must be labelled as derived/calibrated, never a field reconstruction. Citation [85] is not resolvable in the supplied source; include primary reference URLs if reused.

### B-6 — Contacts and report citations incomplete — MAJOR

The returned source contains no actionable source bibliography or in-text primary-source URLs for plot-specific/year/species assertions. Teagasc Jonathan Spazzi contact is independently verified. Ken Sweeney may be a plausible Coillte lead, but his exact email/plot stewardship was not independently established from the official evidence reviewed. All acquisition/enquiry drafts must ask for *internal research and potential commercial-game/derived-map rights separately*, not promise “non-commercial” if the ultimate game may be distributed.

**Required correction:** every chosen site claim should cite official individual source and date; retain exactly one or two **UNSENT** enquiry drafts; verify public official contact channel. No communication is authorised by this task.

### B-7 — Premature plot ranking and incomplete evidence labels — MAJOR

The source's rank order starts Curraghchase/Donadea although no raw records confirm either is the **one published Irish GBIF plot**. Sitka-vs-broadleaf compatibility is unverified; “best for future validation” is based on potential future activities, not observed remeasurement. “Kilteel NO-GO” is too strong as a *future data acquisition* statement: public-data immediate use is unready, but request-only access may still be an option. Several county/site characterisations and species need individual source support.

**Required correction:** one shortlist for **actually ready static import** (presently none independently proven), one for future **field accuracy** (presently no dataset fully proven), and one or two **verified contact candidates**; disclose rather than score unknowns.

## 4. Explicit reviewer assessment against #475 acceptance contract

| Original deliverable | Assessment |
|---|---|
| Named Irish plot leads | **PARTIAL PASS / DOCUMENTATION-ONLY**. Teagasc site map and other leads helpful; site details require individual citations. |
| Real GBIF Irish data archive/plot schema read | **FAIL / NOT DONE**. Research explicitly skipped raw archive. 2022 Table 2 shows one IE plot, not four verified. |
| Exact 2D measured-tree import-ready verdict | **NEEDS CHANGES**. No legitimate GO until plot polygon, original IDs, within-plot XY, species and DBH are actually inspected, consistent and use-rights checked. |
| Two rankings: 2D now vs longitudinal field verification | **PARTIAL, EVIDENCE UNSUPPORTED**. Rankings may be provisional leads only. |
| One or two official contacts + UNSENT drafts | **PARTIAL PASS**. Teagasc/Jonathan is verified. Coillte/Ken exact route requires source confirmation. No outreach occurred. |
| Synthetic fallback | **CONCEPT ACCEPTABLE, NUMERIC EXAMPLE NOT APPROVED**. Needs arithmetic and Irish calibration correction. |
| Distinguish real harvest vs practice marks and real follow-ups | **PASS AS A CAUTION**; no empirical longitudinal record verified. |
| Publication and one-file custody | **PASS** after manager GitHub publication of the unedited returned source with provenance preface; this independent review is separately published. |
| Authorised contact, Unity implementation or model-validation result | **NOT APPROVED / NOT RUN**. |

## 5. Narrow repair proposal and release gates — **not an execution authorisation**

**Next useful task:** one small **public GBIF Irish plot actual-row verification study**, using the existing [open CC-BY archive](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a) if accessible. It must first **establish which one or more IE plot IDs exist in the current actual release**, then determine whether all measured qualifying adult trees, measured DBH and real within-plot XY/plot polygon are reconstructable. The 2022 paper's one-IE-plot count is a historical negative control, not a claim the current archive cannot have added plots. Do not launch a large multi-site simulator or more generic literature research to compensate.

**Future isolated execution proposal only:**
- **Purpose:** answer exactly whether there is a scientifically defensible and legally usable Irish mapped plot for CCF offline 2D import; if not, retain synthetic fallback. No CCF ecology/Unity run needed.
- **Difficulty:** **MEDIUM–HIGH**, because archive field encoding, plot-ID attribution, coordinate transform and appropriate area must be parsed, not merely skimmed.
- **Recommended provider:** OpenAI GPT-6.1 Sol High or Claude Opus High with reliable file/data parsing and official internet download access; a browser-only worker may instead record that exact sample cannot be accessed. Full Deep Research rerun is not needed.
- **Potential lane:** fresh research-only branch/worktree after owner and issue claim; no current reservation or physical host authority established. Public dataset read only, no CCF code or protected saves, no Unity/Java.
- **Tests:** archive manifest/licence; full Irish subset and no missing plot IDs; exactly measured ID-to-DBH/height data joins; strict real XY with origin/range/units; actual plot polygon/exclusions; N and BA computation under agreed DBH threshold; unique-tree and missingness report; if any essential datum absent, **CONDITIONAL or NO-GO exact**.
- **Deliverable:** one separately reviewed GitHub Markdown record with actual source version, measured rows or hash/field examples without disclosing restricted records, explicit “plot ready” or “not proven,” and official-contact follow-up if needed.
- **Stop:** no downloading data whose public licence does not permit it; no unauthorised restricted I+, no account registrations, no commercial shipping, no script/code copied into Unity; no contacting public stewards until owner approves outreach.

**Result status:** **NEEDS CHANGES** to evidence fidelity; do not carry the author's claims “verified raw data for Curraghchase/Donadea/Oughaval/Glenart” or “exact reconstruction possible now” into the Decision Log, feature spec or simulator design. The notion of seeking a real measured plot remains **well worth testing**. The next product decision should follow the limited real-archive verification, or proceed with an explicitly synthetic Sitka fixture while it runs independently.

**Dashboard/Drive sync:** NOT VERIFIED. No user action to accept or preserve the source result. **No outreach, product/spec approval or source implementation authorised.**
