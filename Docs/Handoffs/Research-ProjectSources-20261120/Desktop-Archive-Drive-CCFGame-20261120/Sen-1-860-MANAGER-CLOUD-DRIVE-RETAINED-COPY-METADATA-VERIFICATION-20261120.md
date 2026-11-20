# #860 — Cloud Drive retained-copy verification (partial), 2026-11-20

**Reviewer:** CCF Scenario One Manager #872, GPT-6, connected Google Drive + GitHub (independent Drive-metadata check; not an independent review of the original desktop bytes or operations).  
**Classification:** **PARTIAL VERIFIED / TRASH IDENTITY & CRYPTOGRAPHIC DEDUP STILL OPEN / DRAFT SOURCES NOT APPROVED FOR MERGE**. This is metadata-only, no modifications in Drive, Project attachments, or production repository source.  
**Desktop reference:** [#860 provisional eight-file branch](https://github.com/jermur788/CCF/blob/docs/860-desktop-drive-consolidation-addendum-20261120-v1/Docs/Handoffs/Research-ProjectSources-20261120/Desktop-Archive-Drive-CCFGame-20261120/README.md), commit `d90d1fe26503bf503ba3f8417bf3658cc29fe4fc`; desktop roster `DRIVE-DEDUP-ACTIONS.tsv`. Source label preserved: **DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN**.  
**Method:** Connected Google Drive folder listing for exact identified live folders, with Drive IDs, names and Drive-reported file sizes compared to GitHub desktop roster. Search `trashed = true` via available Drive connector returned zero results; this is **NOT evidence Trash is empty**, and the connector did not expose the 38 trashed source objects' IDs. The connector metadata fetch with checksum/trashed fields requested returned no checksum or trashed property, so no end-to-end SHA-256 identity check was possible using the chosen metadata reads.

## Verified live folders

- [CCF Game](https://drive.google.com/drive/folders/1A1sde5bwmlAnO8au9VBasRvHtkyD0bPL), live top level 34 files and 3 folders: `GrowFor` ID `1ELDZ9prV9IhYWRJxb8WFovTJL5CTgXaa`, `2ndPaperSearch` ID `1AX8RXHFj6BQtGuHzsAzKAvK-UjZURCO_`, `CCF BOOK` ID `1iwG_EC7Lc_wKe2wq4jK1eRLYptMcEldI`. These were individually listed.
- [03 — Ecology Research](https://drive.google.com/drive/folders/10ughHBiC9CaE7qc72B5O9L5FbTnynHyK), seven direct children: five research files `GROWFOR_Technical_Evidence_Extraction_v1.md`, `research-index.md`, `SitkaGrowthModel1.md`, `CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`, and `Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf`, plus folders `CCF Book` and `Racks and Tree Marking`. This verifies **present folder membership at inspection**, not that no hidden historical copy exists elsewhere or what was previously in Trash.
- [03/CCF Book](https://drive.google.com/drive/folders/1D3uD_BUkWCyY-AOWNcx-pNnLCZm_BE-v) retains `CCF_Book_Definitive_Reference.md` Drive ID `1DiNQPpLdAQnpHxuff9lhGjIt5QiHUzWc`, reported size `94305`; [CCF Game/CCF BOOK](https://drive.google.com/drive/folders/1iwG_EC7Lc_wKe2wq4jK1eRLYptMcEldI) retains a same-named item Drive ID `1HEU_0-RWJWFCuJgcn9TV0f5URvTcVPJ7`, size `94305`. **Both remain in place**, metadata size equal; SHA-256 byte identity not independently checked. These are the additional non-trashed #39 roster action, **not** part of the 38 Trash candidates.

## Retained counterparts for all 38 desktop-reported trashed items

For **38 of 38** rows marked `TRASH-REVERSIBLY`, connected Drive returned a live **CCF Game** retained item with the corresponding name (including the named subfolders), and size matched the desktop roster **38 of 38**. The table below records the retained item Drive IDs; **these are NOT the IDs of the supposedly trashed originals**. The SHA-256 field in the desktop TSV is secondhand and not re-hashed here.

| # | Live retained CCF Game name/path | **Retained** Drive file ID (not Trash ID) | Drive bytes, matched to roster |
|---|---|---|---:|
| 1 | Atlantic Temperate Rainforest Under Continuous-Cover Forestry_ Game-Simulation Mechanisms.pdf | `1ZEf51faZ2SNMIosy6Cu1M3hw5adUZnBS` | 222548 |
| 2 | GrowFor\Broad & Lynch 2006 - Growth models for Sitka spruce.pdf | `1xSwMigHi9hkHLGRBpUVzLJcERd_rD9BN` | 798862 |
| 3 | CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | `1X57zzAv6TX2_g2lZqPKfthOTEZrY-nCB` | 141856 |
| 4 | CCF Academic Paper Consolidation.pdf | `10mf-pXhmOloR-rqOwR6bpX7a5YDpmnoF` | 126246 |
| 5 | CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf | `1joQEF3jJAHPJmZL4oUM7UNUGuAUwr3zd` | 369686 |
| 6 | CCF-SITKA-GROWTH-EVIDENCE-AUDIT-V1B.pdf | `1EmiZC_aU0q6KdJ5FAriRVdEs0CayAQNJ` | 323028 |
| 7 | CCF_Primary_Literature_Synthesis_v1.pdf | `1mXTkESIMpaYNwk4OOdZfJUVwF-XonQnE` | 271106 |
| 8 | 2ndPaperSearch\conry1989.pdf | `1Dc1QeRfHdOg6eAqLwPzKyG6IK-VzuEd3` | 2149921 |
| 9 | davies2008.pdf | `1IW_1gQX4XguUScFnMMw8ewecBH_KLbfg` | 721792 |
| 10 | 2ndPaperSearch\dunbar2002.pdf | `1Fv7hnhrZNIeFDsxCUSADuU4n4CNfNRka` | 104630 |
| 11 | European_Pro_Silva_CCF_Practice_Synthesis.md | `1554Vgz9QfekRBtD7IkgHgzWBIFVosbrW` | 71808 |
| 12 | Game Mechanics for Atlantic Temperate Rainforest (Continuous-Cover Forestry).pdf | `1e8l3iemZi_2WXgekjszonStPD9JcnxVI` | 98292 |
| 13 | gill2009.pdf | `13LUboJ9Ii7FvLmJA_vcqA1_LbhvqYaEY` | 251675 |
| 14 | GrowFor\GROWFOR User Manual.pdf | `1yliHCrGw-9XadeowFWWcYcsQBB6uEgv5` | 1430770 |
| 15 | harmer2007.pdf | `1BWKNNsyJSiFQxhlMECYGt-72qAsY1omI` | 178961 |
| 16 | height and diameter growth of Sessile oak and European beech.pdf | `1kQrBFrl6DBtRZ7awJ-xPVY9dgKydByw4` | 51162 |
| 17 | Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf | `1iiLARPLj5GQMHtLOz5MfNK5vyMdkoq_E` | 502361 |
| 18 | Irish Continuous-Cover Forestry Transformation Evidence_ ContinuFOR, TranSSFor, LISS and Related Iri.pdf | `1wB8NRcMtetwHfumQTgGZjevzqe9uoL3U` | 446654 |
| 19 | 2006_VOL63_NO1&2-ss.pdf | `1h6oJDnWP5uyZPtaTzrSrTxLgfwMLBkLO` | 6786783 |
| 20 | Irish_CCF_Browsing_Protection_v1_Report.pdf | `1k_9C4_Dj6By7dbhR8ZMXRMFKKPrr9rny` | 369117 |
| 21 | Irish_CCF_ecology.pdf | `1O5H9TEAsRaYaGRiOMLShaYidyh6hTVjW` | 95157 |
| 22 | Irish_Sitka_CCF_Deep_Research_Report.pdf | `163Am8swWjLi8E7BBlVuVOjVRBoOCjqAM` | 342406 |
| 23 | lekwadi2012.pdf | `16bF4P0ALKeXyAID804LY1f-NHfohJwX_` | 1745921 |
| 24 | macdonald2009.pdf | `1HO07b4axSkMjaaN4fP7TuQFgqMCD9kDC` | 304421 |
| 25 | GrowFor\McCullagh et al 2013 - A comparison of two yield forecasting methods.pdf | `1-dEN7mgmymupK4uXBdRxjs5FXF5PWFML` | 1030424 |
| 26 | GrowFor\McCullagh et al 2017 - Evaluation of tree and stand-level growth models.pdf | `1zL8F4Gr5DgblODkFnmmzxMrbDQHzLWSr` | 564568 |
| 27 | 2ndPaperSearch\moore2000.pdf | `1TVNks-HLzYHYSdN3v1pGqYTo-Ch50q5X` | 129146 |
| 28 | MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | `1u7hchYwOQagaWR426Rt7EvwAF8LBtbas` | 65695 |
| 29 | perrin2011.pdf | `1pnxtr2CWx9lSRIpuw8kdUnrPdftt6CE0` | 699685 |
| 30 | 2ndPaperSearch\pretzsch2019.pdf | `15-YqNFHtNRHmd0LlOlyWH7WFE5wMw4vc` | 3369642 |
| 31 | Primary-Source Audit of the CCF Sitka Spruce Growth Evidence Base.pdf | `1AMIUrtrWPMl4knsbHsTuTbcU9XNEydTt` | 392626 |
| 32 | Scenario Two Species Evidence Closure.pdf | `1nIh3j5qqB5R4gjRnVoVoFQMcgFuBanOz` | 99859 |
| 33 | Scenario Two Species Selection for Irish Continuous-Cover Forestry.pdf | `10ROuuWAxwqCJAvQrJJ_1XmIJE3VJPQeL` | 263898 |
| 34 | Species-Specific Density.pdf | `1Da57Iyh45iCmQJ8zFdLZruhx0_P2AI1r` | 175148 |
| 35 | tabbush1988.pdf | `1_aJ5sx87qau6wWS8bfHUFYBTfsDfdYUj` | 201787 |
| 36 | the-earth-care-manual_-a-permaculture-handbook-for-britain-and-other-temperate-climates-pdfdrive.com-.pdf | `1TL1NKji73U_kpNph6KLcHsSEKbqG9KQm` | 13555529 |
| 37 | Understorey_Dynamics_Irish_CCF_Report.pdf | `1jL32AUVwHuYQP_fNzXPP6FymG62mDd3X` | 383818 |
| 38 | welch1992.pdf | `126PB6aloC4lceI7S3g6aO5qtd0P1FkX3` | 866642 |

## What cannot be verified by this check

1. **Trashed-file IDs: 0 of 38 confirmed.** `trashed = true` connector search returned zero matching objects. This may be a connector search visibility/filter limitation; it does not establish whether the 38 files remain in Trash, were restored, moved or permanently deleted. Do not infer a Trash deletion event from the live folder having fewer files.
2. **Cryptographic identity:** Desktop SHA-256 equality between duplicate and retained file is an author-supplied claim. This metadata check verifies matching **names/path and sizes** only. It does NOT demonstrate identical bytes, previous Drive object content, or original desktop zip completeness. For final byte proof, an authorised actor must fetch and hash the relevant live retained objects and, if recoverable, original trashed objects, or obtain trustworthy native Drive checksums. There is no evidence of this in this report.
3. **Trash 30-day retention window:** Google Drive Trash retention makes prompt checking reasonable, but no automated restore/delete was attempted. If originals need preservation, an authorised human with Drive Trash visibility should open [Drive Trash](https://drive.google.com/drive/trash), record the 38 true source IDs, parent context and checksums where possible, and decide restore vs leave; **do not empty Trash** until independent verification.
4. **The linked earlier partial-preservation branch, #194 archive completeness and old-laptop originals were not independently reconciled here.** The desktop roster is labelled PARTIAL, and the nine-report addendum stays secondhand and conflict-open. D4 Project attachment replacement still needs #425 approval.

## Verdict / recommended next action

- **PASS (limited):** All 38 live retained counterpart file names/path + sizes present in Drive, with actual retained IDs documented in the table. Five live 03 research files and separately preserved same-name CCF Book duplicate verified by folder listing.
- **OPEN:** independent Trash source IDs, original-to-retained byte equality, prior preservation context, incomplete archive and original papers. Do NOT claim 38 items' safe reversible Trash state established.
- **NOT APPROVED:** merging draft research/addendum, deleting more files, emptying Trash, or replacing Project sources. #425 retains publication/Project decisions; #860 remains provisional.

**No Drive or source mutations.** This verification is limited to what the connector exposed; no files were restored or deleted. Dashboard/Project publication not performed.
