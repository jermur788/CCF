> **DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN.** Provisional docs-only record under Manager decision D1. Not canon, not independently reviewed, not for merge.

# #860 desktop Drive preservation, dedup and nine-report addendum (provisional record)

Date: 2026-11-20. Writer: Claude Sonnet 5.5 on the owner's Windows desktop. Class: docs/metadata only. Base: `main` at `4b2dde934f56d285db7c09c50fd8db4303100246`.

## What is in this folder
| File | Content |
|---|---|
| `DESKTOP-PRESERVATION-REPORT.md` | What was downloaded, hashed, trashed, reconciled and not done |
| `SLOT-ROSTER-PROPOSAL.md` | Roster A (12 of 25 used) and Roster B (11 used). Proposal only; Project membership not inspected |
| `CCF_2026-11-20_New_Reports_Evidence_Addendum.md` | Draft derivative of nine new reports. Every figure is as reported; no primary source opened |
| `DRIVE-CCFGAME-HASHES.tsv` | SHA-256 and size of all 87 files in Drive `CCF Game` as extracted from the owner-downloaded zip |
| `DRIVE-03-ECOLOGY-RESEARCH-HASHES.tsv` | Same for the 51 files in Drive `03 - Ecology Research` |
| `DRIVE-DEDUP-ACTIONS.tsv` | The 38 files moved to reversible Drive Trash: hash, size, the retained `CCF Game` copy, reason |
| `194-MANIFEST-ABSENT-FROM-DESKTOP.tsv` | 70 rows of the #194 manifest (165 unique hashes) with no byte match on the desktop. Old-laptop directory prefix replaced by `Downloads/` |

No PDFs, ZIPs or copyrighted full text are included.

## Method and counts (all desktop-reported, not connector-verified)
- Hashes: PowerShell `Get-FileHash` SHA-256 over files extracted from two zips the owner downloaded from Drive. They are hashes of the extracted bytes, not of Drive's stored objects. Drive's own checksums were not compared.
- 38 top-level files in `03` were byte-identical (SHA-256) to a retained `CCF Game` file and were moved to Drive Trash by the owner's instruction. Nothing was permanently deleted. `CCF Game` was not modified.
- **Drive file IDs of the trashed items were NOT recorded.** Identification is by file name and SHA-256. A reviewer with Drive access should confirm the IDs. If Drive access is unavailable, treat the trash step as unverified.
- Restoration: Drive, Trash, select the file, Restore. The Drive Trash empties itself after about 30 days, so a reviewer should check before then.
- #194: manifest has 168 rows and 165 unique SHA-256 values. 95 are byte-present on the desktop, 70 are not. All 42 book photographs matched.
- Per Manager D2, the 5 unique files in `03` and the one `CCF Book` duplicate were left untouched and nothing was moved.

## Project composites (not changed)
| File | Project copy bytes | SHA-256 of the Project copy |
|---|---|---|
| CCF_Ecology_and_Regeneration_Evidence_Composite.md | 37,540 | `85B23B2BDA00F7E59BD8D612486229A769B13ED467C56F202C0127EEF2B36CA0` |
| CCF_Browsing_Protection_and_Management_Evidence_Composite.md | 27,209 | `004756B693C4B159840EA7B44E238CB42DC9BE8798A8DDA6A31E2D5E180BAB88` |
| CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md | 33,058 | `12DDC2B1D42AEA3161008CA3FB8B171B2C1EE6E12CD7580CBC27B5718A13147F` |
| CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md | 34,910 | `CB1C2421D348FC664547BADD36D46DB6862BC1FE10D377FD37A582D23BC173A6` |

The repo copy of the Growth composite at the base SHA (`Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`) is 35,555 bytes, SHA-256 `2704B0AA689F3120FF0549377B973B2C190E2740EA9828ADE88481741F29F73B`. It differs from the Project copy and carries the "V1B EVIDENCE UPDATE (23 October 2026)" text. Only a line-count diff was done. Per Manager D4, no attachment is changed here. The replacement is a separate #425 decision.

## Open items
- Old-laptop files not located: `2018_Light_growth_responses_of_sitka_spruce.pdf`, `CCF_Research_to_Game_Companion.pdf`, Model Capability report, an older GROWFOR extraction (35,250 bytes), two 2026-10-02 project-pack ZIPs. Archive coverage is PARTIAL.
- Not read: the 111-page six-papers PDF, its 24-page extraction, five academic papers, `perrin2011`, `davies2008`, the 2025 yearbook.
- Addendum contradictions between the marteloscope reports are listed in its section D and are unresolved.
- Live Project membership and the context-slot count (6 assumed) are UNKNOWN.
- `active-tasks.md` was not touched.
