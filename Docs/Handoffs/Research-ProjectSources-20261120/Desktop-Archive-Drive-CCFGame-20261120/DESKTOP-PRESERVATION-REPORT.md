> **DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN.** Provisional docs-only record under the Manager decision D1 in `CCF-860-MANAGER-PUBLICATION-DECISIONS-AND-INDEPENDENT-CLOUD-REVIEW-PACKET-20261120.md`. Not canon, not reviewed, not merged. Desktop writer: Claude Sonnet 5.5. No PDFs or ZIPs are in this branch.

# #860 — Drive `CCF Game` desktop preservation (partial)

Date: 2026-11-20. Actor: Claude (Sonnet 5.5) on DESKTOP, via owner-downloaded Drive zip.

## Done
- Source: owner-downloaded zip `drive-download-20261120T131501Z-1-001.zip` (174,075,018 bytes), downloaded by the owner from Drive folder `CCF Game` (ID 1A1sde5bwmlAnO8au9VBasRvHtkyD0bPL).
- Extracted to `originals-from-drive\` (87 files, 178,713,264 bytes). Zip entry names checked first: no path-traversal entries.
- SHA-256 of every extracted file: `DRIVE-CCFGAME-HASHES.tsv`.
- No duplicate hashes inside `CCF Game`.
- 1 of 87 files is byte-identical to the earlier desktop archive (`860-20261120T104951Z`): `Scenario Two Species Evidence Closure.pdf`.
- Nested zip `Attachments-RE_ Irish dynamic yield models (GROWFOR)...zip` opens and lists 4 members (members not yet compared against anything).

## NOT done / unverified
- Hashes are of the zip's extracted bytes, not of Drive's stored bytes. Drive's own checksums were not compared.
- Original Downloads zip left in place; nothing deleted anywhere.
- `03 — Ecology Research` (ID 10ughHBiC9CaE7qc72B5O9L5FbTnynHyK) downloaded and hashed later in this session; see the update below. (Superseded: earlier text said not yet downloaded.)
- Zero Drive changes: nothing trashed, moved or renamed.
- Laptop-source completeness unknown.

## Update - 03 Ecology Research (zip drive-download-20261120T131958Z, 51 files, 37,878,437 bytes)
- Extracted to originals-from-drive-03-ecology-research\; hashes in DRIVE-03-ECOLOGY-RESEARCH-HASHES.tsv. No duplicates inside 03.
- 39 of 51 files are byte-identical to a file in CCF Game (SHA-256). All 39 are HELD in DRIVE-DEDUP-ACTIONS.tsv; no Drive file was trashed.
- 12 files exist only in 03 (e.g. research-index.md, SitkaGrowthModel1.md, 4x marking jpgs, Irish Forestry Economics... pdf). Keep both locations; they are only on desktop via this extraction.
- Repo docs (Docs/Project) contain no links to the 03 folder ID or to individual file URLs; link dependencies outside the repo (Project attachments, other mirrors) could not be checked from here.

## Drive cleanup executed (owner: 'one folder, one copy, copies can be deleted')
- 38 top-level files in 03 - Ecology Research moved to Drive Trash (reversible, not permanently deleted). Each is byte-identical (SHA-256) to a file kept in CCF Game and has a verified desktop copy.
- Left in 03: subfolders 'CCF Book' and 'Racks and Tree Marking' (linked from Docs/Project/research-index.md) and 5 files with no copy in CCF Game: research-index.md, SitkaGrowthModel1.md, GROWFOR_Technical_Evidence_Extraction_v1.md, CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md, Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf.
- 1 duplicate in 03/CCF Book (CCF_Book_Definitive_Reference.md) held.
- CCF Game was not modified.

## #194 reconciliation (2026-11-20, hash comparison against Docs/Handoffs/Research-Source-Consolidation-Audit-01/SHA256-MANIFEST.tsv)
- #194 manifest: 168 rows, 165 unique SHA-256. 95 of those are byte-present in the desktop set (both Drive extractions plus the earlier 19-file archive). 70 are not; list in 194-MANIFEST-ABSENT-FROM-DESKTOP.tsv.
- Of the 70: the 4 composites and 6 rationalisation files match the Project copies the owner downloaded (sizes equal); 18 are dashboard-readback git scaffolding and a handoff note under CCFBook (not research); 13 are DevProgressVisualisation, 1 is RacksAndTreeMarking, 1 is ScenarioOneIntroContentV2 (not research). All 42 book photographs ARE present and byte-matched.
- Possible genuine research gaps, still unlocated (Downloads on the old laptop): 2018_Light_growth_responses_of_sitka_spruce.pdf, CCF_Research_to_Game_Companion.pdf, Model Capability and Cost Research - Repository Execution Report.pdf, attachments.zip, an older GROWFOR_Technical_Evidence_Extraction_v1.md (35,250 B; the Drive 03 copy is a different, larger version at 43,976 B), and two 2026-10-02 project-pack zips (14.3 MB each). Laptop-source completeness remains UNKNOWN.
- Project's four composites equal the 2026-10-16 rationalisation set; the repo Growth composite is a later version (adds the V1B update).

## Evidence addendum
CCF_2026-11-20_New_Reports_Evidence_Addendum.md written (draft, not reviewed). Not covered: six-papers extraction, six complete papers, five academic papers, perrin2011, davies2008.

## Not done
- This folder is the provisional publication. Issue comments are posted separately. No active-tasks.md registration. Project attachments untouched.
