# CCF #479 — Public GBIF Irish marteloscope raw-data verification: blocked archive receipt

**Date:** 2026-11-14 UTC  
**Worker:** OpenAI GPT-6, ChatGPT remote-sandbox research lane (not a T3 Code / HP WSL worker).  
**Status:** **NOT INSPECTED — ARCHIVE DOWNLOAD BLOCKED; NO-GO FOR EXACT-REAL IMPORT CLAIMS.** This is a *failed data-access gate with verified public metadata*, **not** a finding that no eligible Irish marteloscope exists.  
**Review required:** independent Manager disposition — APPROVED AS DATA AUDIT / NEEDS CHANGES / REJECTED.  
**Authorised write:** this Markdown only, on the isolated research branch. No CCF simulation/Unity/source/spec/authority/coordination files touched.

## 1. Gate 0 — actual worker and isolated execution custody

- #479 marked [RUNNING] with actual provider and tools before any data download or script; registration receipt: https://github.com/jermur788/CCF/issues/479#issuecomment-6868847810 .
- Live \`Docs/Project/coordination/active-tasks.md\` was read **read-only** before starting. No #479 competing registration found. No coordinating writer was impersonated and no live coordination entry was modified.
- Research branch \`research/multirack-gbif-irish-plot-raw-audit-20261114-v1\` existed and was inspected. Its starting HEAD was \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`, identical to main at that check (0 ahead, 0 changed files). No force/reset/merge performed.
- **Actual host:** isolated ChatGPT sandbox and GitHub connector; the proposed HP WSL worktree \`/home/jer/Development/CCF-HP-Worktrees/CCF-GBIFIrishPlotAudit-20261114\` was **not created, inspected, reserved or used**. GitHub-only research branch is the sole repository write lane. **No local repository worktree exists in this session.** The proposed off-repo data sandbox root \`/mnt/data/gbif-irish-plot-audit-20261114\` was **not created** after data retrieval failed.
- Container storage check: root overlay 32 GB nominal, 30 GB shown available on this execution; this is **not** an HP disk-capacity or Unity process check. No physical HP, Unity lock, Unity Editor, executable, game scene, save or other user's dirty work was accessed.

## 2. Gate 1 — exact source identification and observed download failure

### Confirmed public **metadata**, not inspected tree records

| Property | Evidence-backed finding | Limit |
|---|---|---|
| Published title | *Spatially explicit database of tree related microhabitats (TreMs)* | Official GBIF dataset page |
| GBIF dataset key | \`2e102194-f384-4712-89a4-5db7a3fc409a\` | Registration key, **not** a plot ID |
| DOI | https://doi.org/10.15468/ocof3v | Persistent citation |
| Publisher / hosted by | Institut national de recherche pour l’agriculture, l’alimentation et l’environnement (INRAE) | Official GBIF registration |
| Publisher archive | \`http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate\` | Exact source-archive link exposed by GBIF |
| Version displayed on GBIF citation | **1.26** | Web page's displayed citation; no live API version response or archive manifest |
| Publication / metadata update | 2022-08-02 displayed | Version shown by available GBIF web page; not proof of a newer release |
| Dataset rights | **CC BY 4.0** | Attribution and licence notice required; not public domain |
| Historical source paper | Zudin et al. (2022), *Biodiversity Data Journal* 10:e91385, https://doi.org/10.3897/BDJ.10.e91385 | Historical 2022 publication, not live row audit |
| Historical release size | **42,078** tree occurrences, **111** plots | 2022 paper/GBIF background only |
| Historical Ireland coverage | **1 plot** | Zudin et al. 2022 **Table 2**; plot name, tree counts and species **not established** |
| Present public ZIP size/version checksum | **NOT OBSERVED** | No archive bytes obtained |
| Archive SHA-256 | **NOT COMPUTABLE** | No downloaded file, no fabricated receipt |
| Actual source contents \`meta.xml\`, \`occurrence.txt\`, \`measurementorfact.txt\` | **NOT INSPECTED** | No header, physical row or plot ID read |

**Primary links:** [GBIF dataset metadata](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a); [GBIF source download tab](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a/download); [published study Table 2](https://pmc.ncbi.nlm.nih.gov/articles/PMC9836569/); [INRAE research context](https://www.inrae.fr/en/news/toward-better-understanding-tree-related-microhabitats).

### Actual retrieval attempts (14 November 2026, isolated ChatGPT container)

\`curl -IL -s -S --connect-timeout 5 --max-time 15\` was run against all of:

1. \`https://api.gbif.org/v1/dataset/2e102194-f384-4712-89a4-5db7a3fc409a\` → \`curl: (6) Could not resolve host: api.gbif.org\`; HTTP status \`000\`.
2. \`https://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate\` → \`curl: (6) Could not resolve host: dynids.toulouse.inra.fr\`; HTTP \`000\`.
3. \`http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate\` → same DNS failure; HTTP \`000\`.
4. \`https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a\` → \`curl: (6) Could not resolve host: www.gbif.org\`; HTTP \`000\`.

A separately routed public-URL download attempt for the **exact official HTTP archive** returned \`ERROR: download failed\`. Attempt to resolve the publisher archive through web-page navigation also did not yield archive bytes. The web browsing service successfully exposed cached GBIF *metadata*, but **cannot be substituted for the source tree archive**.

**Interpretation:** this environment has no functional shell DNS access to these hosts; no HTTP server reply or ZIP content was obtained. The evidence does **not** prove the remote GBIF/INRAE service is down. Do **not** claim a server-side HTTP 404/500 or compute an archive checksum from page text.

**Stop condition reached:** Gate 1 raw-archive inspection did not pass. All derived/tree-level checks below are **NOT TESTED**, in accordance with the governing task packet.

## 3. Ireland subset inventory — strictly bounded result

The **only source-confirmed number for Ireland is a historical plot count of ONE in the paper's 2022 release**; this is a **negative control**, not an independent current tree-level inventory. The broader Irish Teagasc/Coillte training-network inventory is a different population.

| Candidate | Public original I+ \`occurrenceID\` evidence inspected? | Confirmed in present GBIF occurrence table? | Plot-specific stems / years / species / DBH / XY |
|---|---|---|---|
| Unnamed Ireland plot in Zudin et al. (2022) Table 2 | **No** | **NOT TESTED** (historically one country-level plot reported) | **UNKNOWN** |
| Curraghchase | **No** | **NOT VERIFIED** | **UNKNOWN** |
| Donadea | **No** | **NOT VERIFIED** | **UNKNOWN** |
| Oughaval | **No** | **NOT VERIFIED** | **UNKNOWN** |
| Glenart | **No** | **NOT VERIFIED** | **UNKNOWN** |
| Any other Ireland plot in a newer archive | **No** | **NOT TESTED** | **UNKNOWN** |

No city/forest name was inferred from a country total, author affiliation, plot-centre latitude/longitude, an Irish workshop page or a general EFI/I+ directory. Teagasc describes **Irish training plots** separately, including Curraghchase and Oak Park; this is **not proof of their presence in the CC BY GBIF release**. See https://teagasc.ie/crops/forestry/advice/management/continuous-cover-forestry/the-marteloscope-training-network/ .

**Ireland raw occurrence count:** **UNKNOWN**, not 0. **Distinct Irish original trees:** **UNKNOWN**, not 0. **Exact number of current GBIF Irish sites:** **UNKNOWN**, not an extrapolated one.

## 4. Source fields, reconstruction and numerical integrity — NOT TESTED

The Zudin et al. data paper describes a DwC-A **occurrence core** plus **measurementOrFact extension**. The publication's description, not a parsed current archive, associates:

- \`occurrenceID\` with the original I+ tree identifier/country/plot suffix; \`ID\` is a dataset/GBIF joining identity that must not automatically be substituted for a tree ID;
- \`country\`, \`eventDate\`, \`scientificName\`, \`verbatimCoordinates\`, \`verbatimCoordinateSystem\` and WGS84 plot site \`decimalLatitude\`/\`decimalLongitude\` with the occurrence side;
- \`measurementType\`, \`measurementValue\`, \`measurementUnit\`, \`measurementRemarks\` with the measurement side; diameter, height and TreM records must be **type-selected**, not all treated as tree rows.

**All real header/primary-key/extension-link mappings are NOT TESTED** because \`meta.xml\` was unavailable. No original rows were disclosed or fabricated. No \`occurrenceID\` uniqueness, duplicate DBH, missingness, status, tree-species counts or threshold-qualified total has been independently computed.

**Threshold discrepancy to resolve on raw inspection:** the *data paper* says the public release includes trees above a minimum **7.5 cm DBH**, while the GBIF general methodology describes inventorying trees **over 7 cm** at 1.30 m. Do not silently conflate the source sampling rule and exact published inclusion rule; report the observed Irish distribution and its actual minimum only after parsing.

**Coordinate caution:** source metadata says tree positions were collected as polar measurements, but records' \`verbatimCoordinates\` syntax, angle convention, origin, reference axes, handedness, grid-corner registration and plot polygon were **not inspected**. \`decimalLatitude\`/\`decimalLongitude\` describes the marteloscope **site**, not individual local stem XY. A generic 100 × 100 m plot and four quadrants are conventions; dataset metadata explicitly warns that some plots have different shape/area and some measured trees lack positions. Therefore none of the following can be asserted: an unambiguous x/y pair per tree, polygon coordinates, exact 1.000 ha area, edge inclusion, missing-position count, coordinate uniqueness or plot-relative metric accuracy.

**Formula only, not a calculated result:** for a unique eligible adult with DBH in centimetres, \`BA_tree_m2 = pi * (dbh_cm / 200)^2\`. The report does **not** present measured total BA, N/ha, BA/ha, arithmetic/QMD diameter, live/dead breakdown or 2D points; all need the verified join and, for per-hectare measures, surveyed area.

### Exact import gates

| Required gate | Verdict | Evidence / missing datum |
|---|---|---|
| (i) Licensed complete qualifying public stem table | **NOT TESTED** | CC BY metadata seen; actual archive/stem population not downloaded |
| (ii) Original unique tree ID, species, DBH, status | **NOT TESTED** | Field documentation only; no original-row join |
| (iii) Verified local XY and true plot boundary/area | **NOT TESTED** | No original \`verbatimCoordinates\` values, orientation or measured polygon |
| (iv) Archived reproducible source and lawful-use conditions | **FAIL (source archive absent)** | Licence documented but no bytes, versioned checksum or structural receipt |
| Independent static reproduction of adult N, BA, size distribution | **NOT TESTED** | Gate 1 failed |
| Longitudinal field validation after interventions | **NOT TESTED / NOT ESTABLISHED** | Data paper describes a snapshot, not repeated treated-stem outcomes |

**Overall exact-real 2D reproduction:** **NO-GO until all four gates pass**; absence of data is not proof of absent plots.

## 5. Reproducible, non-secret restart steps (not executed)

A new **network-enabled** worker can use the same isolated research branch after actual new worker/claim registration and fresh branch/coordination preflight. No raw data need be committed. A valid run should:

1. Download the exact GBIF original source ZIP from the documented publisher endpoint (or a byte-equivalent officially identified public mirror). Record actual URL/redirects, response, ETag/Last-Modified when exposed, byte count, retrieval UTC, ZIP integrity and SHA-256. Verify version via \`eml.xml\`/metadata/GBIF rather than presuming this cached 1.26 page is the archive version.
2. Inspect \`meta.xml\` for \`core\` and \`extension\` row types, delimiters, encoding and **coreid/id mappings**. Record real file line counts separately from original tree-identifier cardinality.
3. Filter the original occurrence \`country\` or country-code for Ireland, then cross-check the \`IE\` suffix in original \`occurrenceID\`; group by original plot token/site ID and event year; count distinct original IDs and all key/field conflicts.
4. Restrict measurement rows to DBH and height by actual type/unit; join through the **manifest-declared key**. Count missing/duplicate physical measures, species/status unknown, ineligible trees and multi-stem exceptions. Never count TreM extension rows as extra trees.
5. Decode **actual** \`verbatimCoordinates\` examples with a documented, testable polar convention; resolve orientation/scale/origin/corner markers from public original site metadata; check finite, unique positions, absent coordinates and boundary inclusion. If no measured polygon is available, **stop before 2D-exact or per-ha claims**.
6. Independently compute unique living-eligible basal area, total N, mean DBH, QMD and valid diameter classes; calculate N/ha and BA/ha only against a verified surveyed area. Preserve structural samples, non-secret code or pseudocode, source hash and reproducibility receipts in the final Markdown, but **do not commit the raw ZIP or bulk tree-coordinate map**.

No script was created/run against actual archive data in this session. **Script hash: NOT APPLICABLE; transformation checksum: NOT APPLICABLE.** This workflow description is not evidence of a completed transformation.

## 6. Rights, static ecology and permitted contact routes

- The **specific public GBIF TreMs dataset** is labelled **CC BY 4.0** by GBIF. Reuse of actual released rows may be permitted subject to appropriate author/publisher attribution, DOI/source and version, licence link, and indication of adaptations/changes. It is **not public domain**. Attribution should be preserved in any derivative research output. A later game-shipping proposal requires its own actual-source and rights review.
- The wider **restricted EFI I+ repository**, unreleased Irish Teagasc/Coillte inventories, exact training assets, timber prices and virtual marking exercises are **not proved CC BY merely because the open TreMs subset is**. No private data or registration was used.
- The public dataset is described as a spatially explicit **snapshot** of standing trees and TreMs. Neither the public research article nor uninspected records support a demonstrated sequence of real removals, remeasurement of the same Irish trees, post-treatment ecology, repeated-regeneration recruitment or growth-model validation. Such longitudinal evidence must not be inferred from static XY or model projections.
- **Public no-outreach routes verified:** [Teagasc forestry staff/contact page](https://teagasc.ie/crops/forestry/staff-contact-us/) lists Jonathan Spazzi as forestry advisor and \`forestry@teagasc.ie\` as public routing address; the supplied specific profile URL did **not** load in this browser session. [Coillte contact page](https://www.coillte.ie/contact/) provides an official forestry-query phone \`0818 367 378\` and web enquiry form. No individual email scraped from an author list is elevated to a dataset licence grant. **No enquiries drafted/sent**, consistent with the no-contact scope.

## 7. Next-stage decision and boundaries

**Gate status:** Gate 0 **PASS for the registered isolated ChatGPT branch** with explicit absence of HP physical custody; Gate 1 **FAIL/NOT INSPECTED** because the archive could not be retrieved; Gate 2 **NOT TESTED**; Gate 3 **partial public rights/contact metadata only**.

**Ranked real-data candidate:** **NONE IS IMPORT-READY OR IDENTITY-VERIFIED.** The historical one-Irish-plot report is a bounded query lead, not a named plot. No entry among Curraghchase/Donadea/Oughaval/Glenart is promoted into an import-ready stand.

**Disposition: NO-GO for exact real-plot reconstruction now (data-access blocked).** The next step is to repeat this narrowly scoped **raw-source-only audit** on a host with working public DNS/network, valid isolated custody and a refreshed issue/ownership record. If the downloaded Irish records lack Sitka or measurable unique local XY and a real polygon, choose **a transparently synthetic, flat Sitka reference stand** with fixed synthetic IDs and internally reconciled Irish-evidence-backed stocking, basal area and DBH distribution. It must not be labelled a real marteloscope. In particular, no unsupported 400–500 stems of 25–30 cm implying 50–60 m²/ha BA claim is allowed.

No decision is made about CCF simulation mechanics, tree marking, 3D implementation, source-code integration, production licence or future ecological validity.

### Completion/status and review checklist

| Item | Status |
|---|---|
| Actual worker / branch registration on #479 | **PASS** |
| Read-only live coordination review | **PASS** |
| Official public dataset URL/version/licence discovery | **PASS (web metadata only)** |
| Raw ZIP downloaded / integrity / SHA-256 | **FAIL — inaccessible in this sandbox** |
| \`meta.xml\`/core/extension inspected | **NOT TESTED** |
| Irish original plot names and measured stem counts | **NOT TESTED** |
| Tree IDs/species/DBH/height/status/XY/polygon | **NOT TESTED** |
| Independent N/ha, BA/ha and 2D reconstruction | **NOT TESTED** |
| Private data or organisation contact | **NONE** |
| Unity / production / game/save changes | **NONE** |
| GitHub one-file branch comparison | **TO BE VERIFIED AFTER THIS COMMIT** |
| Dashboard / Drive mirror sync | **NOT VERIFIED; no update claimed** |

**Sources:** GBIF public metadata and download tab, Zudin et al. (2022) (DOI above, especially Table 2 and data field definitions), official Teagasc and Coillte contact pages, and the [#479 governing packet](https://github.com/jermur788/CCF/blob/docs/multirack-gbif-irish-raw-plot-audit-packet-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-GBIF-IRISH-RAW-PLOT-INVENTORY-VERIFICATION-PACKET-20261114.md). All data-paper facts are **historical published evidence**; no present original-tree record is claimed.
