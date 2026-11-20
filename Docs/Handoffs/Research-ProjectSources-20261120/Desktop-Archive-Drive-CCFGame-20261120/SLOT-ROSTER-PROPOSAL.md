> **DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN.** Provisional docs-only record under the Manager decision D1 in `CCF-860-MANAGER-PUBLICATION-DECISIONS-AND-INDEPENDENT-CLOUD-REVIEW-PACKET-20261120.md`. Not canon, not reviewed, not merged. Desktop writer: Claude Sonnet 5.5. No PDFs or ZIPs are in this branch.

# #860 — Project attachment roster proposals (PROPOSED, not applied)

Date: 2026-11-20. Author: Claude (Sonnet 5.5), desktop. Status: **proposal only**. Live ChatGPT/Claude Project membership was NOT inspected, so current occupancy is **UNKNOWN**. Nothing in any Project was changed.

Cap (owner): at most 15 occupied of 25, at least 10 free, counting instructions, roles and context.

## Verified inventory facts
- 175 PDF paths found across the Drive extractions, the earlier desktop archive and Documents. Only **59 are unique by SHA-256**. Page counts come from `pdftotext` form feeds. Every unique PDF has a text layer (no scan-only files).
- Topic grouping below is by filename plus first lines of the text. **I have not read the papers**, so grouping is provisional.
- Two papers (`perrin2011.pdf`, `davies2008.pdf`) have no confirmed topic; they are left out of the bundles until their first pages are read.
- Not bundled (archive only): `the-earth-care-manual…` (482 p, general reference per research-index), `How to balance foresty and biodiversity…` (644 p), the 168 p `2006_VOL63` journal (Broad & Lynch 2006a/b are already in the 31 p extract and in V1B).
- **Three of the four existing composites are not in either Drive folder I downloaded** (Ecology & Regeneration, Browsing & Protection, CCF Practice/Economics). Only the Growth/Density composite and `research-index.md` were present, in `03`. Their location is UNKNOWN (research-index says an external path). They are needed for Roster A.

## Update (14:25 UTC): composites now located
The owner downloaded all four composites from the Project. Verified copies are in `project-attachments-copy-20261120\`. Ecology 37,540 B, Browsing 27,209 B, Practice/Economics 34,910 B, Growth/Density 33,058 B.
**Finding:** the Project's Growth/Density composite is an older version than the repo/Drive-03 copy (35,555 B, 436 lines vs 403). The Project copy lacks the "V1B EVIDENCE UPDATE (23 October 2026)" text on Broad & Lynch 2006a/2006b roles and the validation verdict. Recommend replacing it with the repo version (needs owner/#425 approval; not applied). Line diff only; I did not check for other differences.

## Context slot cost (assumption, needs your confirmation)
The 2026-11-19 verified context zip holds 13 mirrored files plus two instruction files. If all were attached individually they would use about 13-15 slots on their own. I assume a **minimum 6 context slots**: project instructions, decision-log, current-milestone, game-brief, unity-project-overview, research-index. AgentWorkflow, AGENTS/CLAUDE and the role files would then have to live in the instructions or be merged. **Merging context files needs #425 approval.**

## Roster A — Hybrid (recommended)
| # | Slot | Kind | Covers |
|---|---|---|---|
| 1-6 | Context (see above) | context | project governance |
| 7 | CCF_Ecology_and_Regeneration_Evidence_Composite.md | composite (location UNKNOWN) | ecology/regeneration |
| 8 | CCF_Browsing_Protection_and_Management_Evidence_Composite.md | composite (location UNKNOWN) | browsing/protection |
| 9 | CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md | composite (found in `03`) | growth/density |
| 10 | CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md | composite (location UNKNOWN) | practice/economics |
| 11 | CCF Species Evidence Pack v1 (27 p) | retained original | multi-species |
| 12 | New 2026 addendum pack (NOT YET WRITTEN) | derivative | scaling, 2D map, #853, marteloscope |

Used 12, free 13. All 59 originals stay on the desktop and in Drive.
Lost from in-chat access: every individual paper's tables and figures (they stay in the archive).

## Roster B — Complete-page topic bundles
Bundles are merged copies of whole PDFs with a front manifest and page-offset map. Pages are totals of the unique files; merging changes pagination and may drop bookmarks and metadata.
| # | Bundle | Files | Pages |
|---|---|---|---|
| 7 | Growth/density/yield/Sitka/GROWFOR | 17 | 206 |
| 8 | Ecology/regeneration/browsing | 11 | 179 |
| 9 | CCF practice/marking/transformation/economics (without the 105 p yearbook) | 5 | 121 |
| 10 | Species/Scenario Two (Species Evidence Pack, Closure, Selection, Comprehensive Edition) | 4 | 60 |
| 11 | New 2026 reports, marteloscope, nurse-crop summary, six-papers extraction | 8 | 84 |

Left out of every bundle for now (archive only, topic not yet assigned): the 111 p `CCF_Six_Complete_Forestry_Research_Papers.pdf`, the five academic papers (individual-based management, silvicultural systems, Bayesian projection, structure indices, CanopyShotNoise), `perrin2011`, `davies2008`, the 2025 yearbook, and the superseded `CCF Academic Paper Consolidation.pdf`.

Used 11 with the same 6 context slots, free 14. Larger files may hurt retrieval quality (UNVERIFIED). Merge steps need your approval.

## Not done yet
- Derivative evidence packs (per-claim page citations): **not written**. That is a large job and needs the papers read.
- Comparison against the #194 manifest, and registering #860 as RUNNING in `active-tasks.md`.
- Publishing: done as this provisional docs record (see README.md).

## Decisions needed
1. Roster A or B?
2. Where are the three missing composites?
3. Confirm the context slot count (6 assumed).
