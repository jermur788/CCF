# #860 desktop preservation — partial execution result, 20 November 2026

**PARTIAL EXECUTION — NOT COMPLETE; Drive inventory, preservation and cleanup HELD.** Nineteen accessible desktop research files, totaling **255,710,034 bytes**, were copied into a new offline desktop archive and verified by full-byte SHA-256 and size. There were **19 unique hashes, 19 verified copies, zero mismatches**. **Zero Drive entries were changed or trashed.** This result does not establish laptop or Drive completeness and does not close #860.

Task authority: [binding desktop execution handoff](https://github.com/jermur788/CCF/blob/docs/860-research-25-slot-delta-audit-dispatch-20261120-v1/Docs/Handoffs/Research-ProjectSources-20261120/CCF-860-DESKTOP-DRIVE-PRESERVATION-CONSOLIDATION-AND-DEDUP-EXECUTION-HANDOFF-20261120.md), [#860](https://github.com/jermur788/CCF/issues/860), [#730](https://github.com/jermur788/CCF/issues/730), [#425](https://github.com/jermur788/CCF/issues/425). User confirmed transfers ready in this session. Task remains archive/research work, with no game-mechanics, gate, source-integration or Project-membership authority.

## Actor, source snapshot and boundaries

- Worker: OpenAI gpt-6.1-sol, high reasoning, current T3 Code session; one desktop archival writer, no child agents.
- Host: verified `DESKTOP-DL1BQI7`; Documents owner `DESKTOP-DL1BQI7\hp`; Windows user hp / WSL Ubuntu jer.
- Actual Documents: `C:/Users/hp/Documents`.
- Source worktree: `/home/jer/Development/CCF-HP`, branch main, inspected HEAD `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`. WSL-native Git was clean at the start and final readback. Initial Windows-side Git had reported many asset/doc differences; no reset, checkout, clean, stash or source mutation was performed to reconcile them. No existing work was absorbed.
- Locked local context snapshot: `3f4e852e9579de7335e6d71ca660ecd2b0f8b561`; live research-index and Decision Log were also fetched during preflight. Live coordination read at start blob `31720018083f541a83fa7bcbf6fe75431945bc18`, and before handoff blob `31720018083f541a83fa7bcbf6fe75431945bc18`. Full task ownership is not inferred from a missing board row; #860 issue and comments were inspected directly.
- GitHub publication base: fresh main `cba0a905b766ce8fc0356d351ac5bff1c2a9a008`. New docs-only branch: `docs/860-desktop-partial-preservation-20261120t104951z`. Sole report namespace: `Docs/Handoffs/Research-ProjectSources-20261120/Desktop-Preservation-20261120T104951Z/`.
- [Actual partial-start registration](https://github.com/jermur788/CCF/issues/860#issuecomment-7037629489). Explicitly a bounded local preservation start with remote actions held, **not a false full-task RUNNING claim** before Drive authentication.
- Verified free bytes before copying: `65847595008`; over twice the selected input footprint plus 1 GiB was required by the preflight.

## Desktop preservation that actually ran

Archive: `C:/Users/hp/Documents/CCF Research Archive/860-20261120T104951Z`.

Sources were restricted to direct research PDFs in Documents, files in `Documents/CCF Papers` and `Documents/Foresty Books`, and these six explicitly named research downloads:

- Implementation Options for a C#-First 2D Map.pdf
- Independent Sitka_Conifer Data Sources (E1) (1).pdf
- Scaling Continuous-Cover Forestry (CCF) Simulation.pdf
- Scenario One #853 — Deep Research Conclusion.pdf
- Scenario Two Species Evidence Closure.pdf
- 0005809-261110092616502.zip

The completed desktop set still included these new reports in Downloads, so they were copied directly from there; originals were not moved. Unrelated Documents and Downloads contents, worktrees, GitHub folders, scratch/qualification/skill directories and personal media junctions were excluded. No direct laptop filesystem access was used.

Every source was hashed before copy, written with exclusive-create semantics, reopened and SHA-256 hashed at its destination, and source-hashed again with size/mtime checks. A separate final pass reopened all 19 original and destination pairs and checked full hashes and size again: **19/19 PASS**. Existing archive entries were not overwritten. The original source files remain present and unchanged in the checked hash/size sense; ACLs/timestamps/alternate streams were not cloned as a filesystem-forensic image.

This is an offline-usable copy on the same physical desktop disk. It is **not** an independent-device disaster backup. No retained Drive original has been verified. Restoration: choose a row in DESKTOP-PRESERVATION-MANIFEST.tsv, copy its `desktop_path` to a new chosen destination (never overwrite a source blindly), and compare the restored SHA-256 and byte size to that row. No trash restoration is needed because no trash ran.

All **18 PDFs** were parsed with Windows' built-in `Windows.Data.Pdf.PdfDocument.PageCount`; total physical pages **964**. These counts are actual parsed page-tree counts, not a claim every page was read. Printed numbering and complete citation pagination are generally unreviewed. Five recent reports (47 pages) were additionally rendered/OCR-read using built-in Windows OCR; the OCR remains private and uncorrected. Four cited source pages were viewed as rendered originals. No package or external dependency was installed.

The TreMs ZIP was preserved intact. All **12 member names and member byte contents** were compared between source and copy; per-member sizes and SHA-256 are in ZIP-MEMBER-INVENTORY.tsv. Embedded rights.txt states CC BY 4.0. citations.txt identifies Kraus et al. (2022), dataset v1.26, DOI `10.15468/ocof3v`, accessed 13 November 2026. Metadata are not a validation of the dataset's ecological accuracy. No ZIP member was extracted into Unity or the source worktree.

## Reconciliation with #194 and the existing composites

The old SHA256-MANIFEST.tsv contains **168 rows / 165 unique hashes**. One of those unique hashes matches this desktop set: the Scenario Two Species Evidence Closure PDF. The other **164 old unique hashes** have no match in this selected desktop set; **18 current desktop hashes** are absent from the old manifest. Old rows include context snapshots and repository metadata as well as research, so these numbers must not be described as 164 missing primary papers or 18 new scientific findings.

The full row-by-row check is in 194-PARITY-CHECK.tsv. The old single-copy risks remain **UNKNOWN for desktop/Drive parity**: welch1991.pdf, the 2018 Sitka light-growth paper, Delegated Marking, the 2025 yearbook, original book photographs and marking photographs. They were not present in the bounded desktop set. The dispatch's earlier observation of some in Drive was not independently reverified here. A laptop-side source manifest has not been obtained, so **all-laptop-files-backed-up is not established**.

The quantitative growth/density composite was read completely from Git. research-index.md was inspected for the four-composite map and species/source status. The other three full composites were not accessible in this selected local set or tracked paths inspected, and cannot yet be compared for detailed overlap. None of the four was rewritten.

[RESEARCH-READING-NOTES.md](RESEARCH-READING-NOTES.md) is a brief dated source-quality addendum, **not the completed useful-evidence consolidation**. It preserves important limitations: the scaling report did not inspect the repository, the 2D-framework choice is a proposal, the dataset-search report's negative search is not universal dataset nonexistence, and #853's claims about code require direct candidate checks. Report advice is kept separate from accepted decisions and #849's qualified source. No unsupported forestry constants were extracted or accepted. Underlying papers/web references still need primary-source verification; one #853 matrix has a visibly clipped right column.

## Drive and platform gates still held

At the latest capability check Google Drive remained **not installed/connected in this session**. The user selected the plugin route; an installation/connection prompt was requested. No authenticated Drive API, local Drive mount or Drive write capability was available. A browser route was not selected and no browser session or stored credentials were inspected. GitHub access proves none of these remote archive facts.

Required remote roots remain:

- [CCF Game](https://drive.google.com/drive/folders/1A1sde5bwmlAnO8au9VBasRvHtkyD0bPL), ID `1A1sde5bwmlAnO8au9VBasRvHtkyD0bPL`.
- [03 — Ecology Research](https://drive.google.com/drive/folders/10ughHBiC9CaE7qc72B5O9L5FbTnynHyK), ID `10ughHBiC9CaE7qc72B5O9L5FbTnynHyK`.

Both recursive listings, full raw-byte downloads/hashes, canonical file IDs, stable-link/backlink checks and remote version status are **UNKNOWN / NOT RUN**. The counts 37 and 45 in the dispatch are historical observations only. No repeat claim of current counts is made.

DRIVE-DEDUP-ACTIONS.tsv intentionally contains its schema and **zero action rows**; missing access is not represented as fictitious per-file actions. No Drive upload, move, reparent, modification, Trash or permanent deletion occurred. Permanent mirrors and legacy cited sources remain protected. Every candidate duplicate still needs the exact desktop-copy plus retained-Drive-copy preservation gate and backlink/attachment checks before reversible Trash can be considered.

[PROPOSED-ATTACHMENT-ROSTERS.md](PROPOSED-ATTACHMENT-ROSTERS.md) provides two conditional candidates with **15 total occupied / 10 empty** slots each, explicitly counting the existing generated role/shared-instructions composite and six separate context files. Neither is APPLIED; live attachment identity/count is UNKNOWN. The full-page-PDF alternative is future separately authorized work only: no merge was made. Preferred 10–12 occupancy remains a future reduction target rather than a fabricated assurance that essential evidence fits. Completed consolidation and full source/page/rights mapping remain open.

## Published artifacts and local-only evidence

Public metadata and bounded authored notes only:

- [MASTER-RESEARCH-INVENTORY.tsv](MASTER-RESEARCH-INVENTORY.tsv)
- [DESKTOP-PRESERVATION-MANIFEST.tsv](DESKTOP-PRESERVATION-MANIFEST.tsv)
- [FINAL-READBACK.tsv](FINAL-READBACK.tsv)
- [PDF-PAGE-COUNTS.tsv](PDF-PAGE-COUNTS.tsv)
- [ZIP-MEMBER-INVENTORY.tsv](ZIP-MEMBER-INVENTORY.tsv)
- [194-PARITY-CHECK.tsv](194-PARITY-CHECK.tsv)
- [DRIVE-DEDUP-ACTIONS.tsv](DRIVE-DEDUP-ACTIONS.tsv)
- [RESEARCH-READING-NOTES.md](RESEARCH-READING-NOTES.md)
- [PROPOSED-ATTACHMENT-ROSTERS.md](PROPOSED-ATTACHMENT-ROSTERS.md)
- [PUBLISHED-ARTIFACT-HASHES.tsv](PUBLISHED-ARTIFACT-HASHES.tsv)

Original copyrighted PDFs, the dataset ZIP, embedded full metadata, private OCR and rendered source-page checks remain only under the desktop archive. They are excluded from the public branch. No new gameplay asset, .meta, serialized file, package, ProjectSettings, canonical context, existing research composite, generated dashboard-state or platform attachment was changed. No Unity/compiler/runtime gate was needed or claimed for byte-preservation work.

## Remaining work / handoff state

1. Complete the Google Drive connection and verify the concrete available operations, raw-byte download support and reversible Trash semantics. Missing operations remain individually held.
2. Re-read current #860 and live coordination; claim the real full executor only after its prerequisites are satisfied. Re-list both remote trees after confirmed upload completion; reconcile every accessible original with this archive and #194, and obtain/source-match a laptop manifest for completeness claims.
3. Copy every remaining accessible unique original to a new versioned archive location, with complete source/destination hashes and size. Preserve source versions, photographs and complete ZIP membership. Record unavailable sources explicitly.
4. Read all four existing composites; complete the source/page-cited novel-evidence addenda and rights/citation/contradiction review. Resolve report-vs-source conflicts without rewriting accepted decisions.
5. Only then consider small reversible Trash batches for byte-identical, unreferenced Drive duplicates whose exact desktop copy and retained Drive copy have passed. Verify retained objects and restoration logs after each batch; no permanent deletion.
6. Audit actual live Project membership before any future migration proposal. The current task authorizes proposals only.
7. Manager-owned live coordination, dashboard ingestion/audit-snapshot advance, published dashboard and Drive mirror synchronization are **NOT VERIFIED** and remain operational follow-up. The archival worker does not overwrite the single-writer coordination board or generated dashboard files.

Worker assessment: **ready for review of this local preservation result only**. No G2/G3/G4/G5 mechanics/integration decision is issued. Full task remains open. Publication blob readback and exact docs-only diff verification are performed after creation; the resulting commit is reported in the linked issue comments.
