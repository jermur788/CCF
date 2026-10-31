# Research Source Consolidation Audit 01 — HANDOFF

**Worker:** DeepSeek V4 Pro (max)
**Role:** Primary (analysis/plan only)
**Branch:** `research/source-consolidation-audit-v1`
**Worktree:** `/media/jer/Videos/CCF-RESEARCH-CONSOLIDATION-AUDIT`
**Packet:** `Docs/Handoffs/Research-Source-Consolidation-Audit-01/TASK-PACKET.md`
**Packet start:** `488f093e9100091858f81947b588aad5c36d84f0`
**Task start SHA:** `ba11ea4a73a8b6cdf8f7f31583abba52ccd4e59d` (descendant of packet start)
**Refreshed base at handoff:** `fc99ee0122140ae3ba279c48267e05061c3df121` (origin/main after #193 integration; claim rebased on top)
**No Unity. No Windows build. No source mutation.**

---

## Verdict

`RESEARCH SOURCE CONSOLIDATION PLAN COMPLETE — READY FOR USER / MANAGER DECISION`

The corpus is small (~210 MiB deduplicated) and lives on `/` (root SSD) and `/media/jer/Files`, **not** on the constrained `/media/jer/Videos` disk. Consolidating research sources will not free meaningful space on the 96%-full Videos volume. The project's 2026-10-16 composite/archive pattern already implements the sound approach; the highest-value actions are **preservation backups of single-copy originals** and **removal of a small number of redundant/stale Downloads copies**, not PDF merging.

---

## 1. Executive recommendation

1. **Do not build topic-bundle PDFs (B) or a master PDF (C).** They add bytes, break per-file provenance, and add maintenance for a corpus this small.
2. **Formalise Strategy D** (project-facing composites + byte-identical archive originals) — this is already in place from 2026-10-16 and is correct; no re-do needed.
3. **Back up the single-copy originals first (PRESERVATION, highest priority).** Six families have no Drive or Files archive copy and are at risk: the 38 book page photos (~113.6 MiB), the 4 full-resolution marking photos (~10.7 MiB), `welch1991.pdf`, `2018_Light_growth_responses_of_sitka_spruce.pdf`, `Delegated Marking and Operator Selection…pdf`, and `2025Forestry_TimberYearbook.pdf` (≈ 131 MiB total). Copy (not move) them to `/media/jer/Files/CCF/Research/` with the SHA-256 manifest.
4. **Only after approval**, remove redundant/stale non-canonical copies (~42 MB): the two 2026-10-02 project-pack zips, `attachments.zip`, the GROWFOR request zip (contents already extracted), the stale Downloads `GROWFOR_Technical_Evidence_Extraction_v1.md`, the Downloads `CCF-context-*d05b27a` snapshots, and the superseded `CCF_Book_Definitive_Reference_DRAFT.md`.
5. **Leave canonical Git records and every original research PDF byte-identical.** No watermarking/recompression of originals; `gs` recompression was measured to *increase* size (see §9).

Actual measured active-space saving: **~42 MB recoverable across `/` (Downloads + `~/Documents/CCF Game`)**; **0 MB on `/media/jer/Videos`** (the corpus is not stored there).

## 2. Complete inventory summary and total bytes

| Surface | Files | Bytes | MiB |
|---|---|---|---|
| `~/Documents/CCF Game/` (research archive) | 87 | 178,713,264 | 170.43 |
| ‑ of which `CCF BOOK/` (photos+md, single-copy) | 45 | 130,660,770 | 124.61 |
| `/media/jer/Files/CCF/Research/` | 49 | 5,385,752 | 5.14 |
| `/media/jer/Files/CCF/Verification/ProjectSourceRationalisation-…/` | 12 | 220,431 | 0.21 |
| `~/Downloads/` (CCF corpus files only) | ~24 | ~40,832,410 | ~38.94 |
| **Total inventoried (with cross-location duplicates)** | **~168** | **~225,151,857** | **~214.72** |

Deduplicated research-source bytes ≈ **~210 MiB**. The dominant single component is the single-copy book photography (~124.6 MiB). The "active project-facing source set" (ChatGPT Game Dev attachments + canonical repo research Markdown) is **~1.3 MiB** — the consolidation problem the packet frames (many overlapping project PDFs) is already resolved.

Full per-file paths, sizes and SHA-256 are in `SHA256-MANIFEST.tsv`; the classification table is `INVENTORY.tsv`.

## 3. Exact duplicate groups

Byte-identical (same SHA-256) duplicates are few and small:

| Group | Files | Redundant bytes |
|---|---|---|
| D-BOOKREF | `CCF_Book_Definitive_Reference.md` ×3 (repo, `~/Documents/CCF Game/CCF BOOK`, Files archive) | 188,610 |
| D-PAGENOTES | `CCF_BOOK_page-notes.md` ×2 (local, Files `PreviousNotes/`) | 35,084 |
| D-RACK | `CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md` ×2 (repo, Files) | 25,084 |
| D-TRANSFORM | `Irish Continuous-Cover Forestry Transformation Evidence…pdf` ×2 (CCF Game, Downloads) | 446,654 |
| D-2006 | `2006_VOL63_NO1&2-ss.pdf` == Drive journal (verified by temp-download hash) | 0 (intentional mirror) |
| (Drive) | `GROWFOR_Technical_Evidence_Extraction_v1.md` Drive == repo (verified by temp-download hash) | 0 (intentional mirror) |

Total redundant duplicate bytes ≈ **0.66 MB**. The Drive mirrors and the repo↔Files↔local canonical Markdown triples are **intentional distribution mirrors**, not clutter. Conclusion: exact-dedup is negligible; it is not a storage lever here.

## 4. Near-duplicate / superseded / composite relationships (verified)

Verified at text level this session (not just title similarity):

- **`CCF Academic Paper Consolidation.pdf` (2 pp) → FULLY SUPERSEDED** by `…Comprehensive Evidence Edition.pdf` (3 pp). `pdftotext -layout` diff confirms every substantive description (Conry/Dunbar/Moore/Pretzsch coverage) is re-expressed in the Comprehensive edition with added DOIs/bibliographic metadata. Prior audit's "fully superseded" claim **confirmed**.
- **`Primary-Source Audit … (V1A)` (22 pp) → historical**; **`CCF-SITKA-GROWTH-EVIDENCE-AUDIT-V1B` (21 pp) → current.** V1B text states it corrects the V1A "documentation-only" interpretation using the uploaded 2006 journal (2006a pp.53–79, 2006b pp.80–95). Distinct artifacts, both retained.
- **`Scenario Two Species Selection…` (19 pp) → superseded** Douglas-fir recommendation; **`Scenario Two Species Evidence Closure` (11 pp) → current** (Scots pine READY; birch/alder PARTIAL). Confirmed.
- **`CCF_Primary_Literature_Synthesis_v1.pdf` (12 pp) → distributed** across Composites 1–4 (each section keeps per-claim provenance); original archived in Drive. Confirmed in research-index.
- **`CCF Academic Paper Consolidation` ordinary vs Comprehensive** (above) is the only genuine supersession; **V1A/V1B** are staged, not superseded.
- **`GROWFOR_Technical_Evidence_Extraction_v1.md`**: Downloads copy (35,250 B) is the pre-V1B 15-Oct version; repo/Drive (43,976 B) is the 23-Oct V1B-sync revision. Downloads copy is stale.
- **`CCF_Growth_Density…Composite.md`**: repo copy (35,555 B) is the 23-Oct V1B-updated canonical; the rationalisation snapshot (33,058 B) is the 2026-10-16 original. Repo wins.
- **Marking photos**: `~/Documents/CCF Game/CCF BOOK/marking*.jpg` are 3072×4080 originals; Files/Drive `SourcePages/marking*.jpg` are 1506×2000 downscaled derivatives (different bytes, confirmed via `exiftool`). Originals are single-copy.
- **The four 2026-10-16 composites** (S44–S47) absorb S22–S30/S34–S37 as documented in `INVENTORY.tsv` and the prior `removal-safety.md`.

## 5. Files that must stay individually accessible (and why)

- **7 canonical context files** (game-brief, decision-log, current-milestone, unity-project-overview, AgentWorkflow, research-index, context-manifest) — independent update semantics.
- **4 composites** — project-facing derivatives; canonical Composite 3 is in the repo.
- **Retained-individual research:** `Irish Atlantic Woodland…pdf` (spatial/hydrology superset), `CCF Species Evidence Pack v1…pdf` (Scenario Two backbone), `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf` (density-packet pointer with DOIs), `GROWFOR_Technical_Evidence_Extraction_v1.md` (technical provenance), `Scenario Two Species Evidence Closure.pdf` (current closure).
- **V1A and V1B audits** — the V1A/V1B distinction is intentional; never collapse them.
- **`2006_VOL63_NO1&2-ss.pdf`** — original journal with load-bearing printed pagination (2006a/2006b page ranges).
- **The 38 book page photos + 4 full-res marking photos** — original primary evidence for the canonical book reference; single-copy.
- **Canonical extractions** (`CCF_Book_Definitive_Reference.md`, `CCF_Rack_Planning_…Extraction_v1.md`, `source-inventory.md`) — provenance manifests.

## 6. Proposed bundles / merged PDFs

**None recommended.** Rationale (measured, not speculative):

- Concatenating already-compressed PDFs yields a file ≈ the sum of its parts plus separator bytes — **zero byte savings**.
- A `gs` `pdfwrite` "merge" was measured to **increase** size (journal 6.79→8.35 MB at `/ebook`; yearbook 3.49→3.72 MB).
- Every existing citation already resolves to an individual filename (see §10); bundling would force citation rewrites for no gain.
- The corpus is ~210 MiB; file-count clutter is already resolved by the four composites.

If a topic bundle is nonetheless wanted later, `PAGE-MAP.tsv` documents the unchanged-concatenation scheme (cover page + per-member offset map, originals archived byte-identical). That is AMBER, not recommended.

## 7. Archive / backup design

- **Drive `03 — Ecology Research`** (verified live, 45 items) already mirrors every primary paper and report PDF byte-identical (spot-verified for the 2006 journal and GROWFOR extraction). Keep it as the distribution archive; read-only this task.
- **Local durable archive:** `/media/jer/Files/CCF/Research/` (ext4, durable). Currently holds the book reference/rack extraction/dashboard artefacts but **not** the single-copy photos/papers listed in §1.3.
- **Recommended archive layout (future execution):**
  - `/media/jer/Files/CCF/Research/CCFBook/PagePhotos/` ← 38 IMG photos (copy)
  - `/media/jer/Files/CCF/Research/CCFBook/MarkingOriginals/` ← 4 full-res marking JPGs (copy)
  - `/media/jer/Files/CCF/Research/PrimaryPapers/` ← welch1991, light-growth 2018, Delegated Marking, 2025 Timber Yearbook (copy)
  - `SHA256-MANIFEST.tsv` stored beside each tree.
- **Container choice:** plain folder + SHA-256 manifest (not a single 7z). A single archive container trades partial-retrieval and corruption blast-radius for only ~6% compression (measured §9). Use `tar` only for transport.

## 8. Derivative watermark / cover-page recommendation

- **Only derivatives, never originals.**
- **Prefer a cover page** (`PROJECT REFERENCE COPY — ORIGINAL ARCHIVED` + archive path + source ID + original SHA-256 + build date + pagination statement) over a watermark. A watermark risks obscuring figures/tables and degrading OCR/search; a cover page is non-destructive and reversible.
- The four composites already carry `SUPPORTING RESEARCH — NOT DECISION AUTHORITY` headers and per-claim provenance; no additional marking is required on them.
- Do not apply any marking in this task (no mutation authorised).

## 9. Current vs projected storage table

Trial compression (copies in `/tmp`, originals untouched):
- `7z a -t7z -mx=9` on a mixed 31.5 MB sample → 29.6 MB (**≈6% saving**; PDFs already compressed).
- `gs -dPDFSETTINGS=/ebook|/printer` → **larger** than source (negative savings) on the scanned journal and yearbook.

| Strategy | Active-source delta | Reclaimable on `/` | Reclaimable on `/media/jer/Videos` | Risk |
|---|---|---|---|---|
| **A — exact-dedup only** | −0.66 MB | 0.66 MB | 0 | GREEN |
| **B — topic bundles** | +cover/separator bytes | ≤0 (no savings) | 0 | AMBER |
| **C — one master PDF** | ≈0 (sum of parts) | 0 | 0 | RED (citation/pagination) |
| **D — derivatives + archived originals** | already in place | — | 0 | GREEN |
| **E — marked derivative copies** | +cover-page bytes | 0 | 0 | GREEN |
| **F — cold archive container (7z)** | −≈6% only | ≈12 MB (if all PDFs 7z'd) | 0 | AMBER |
| **Backup single-copy originals (preservation)** | +131 MB on Files | 0 (bytes added) | 0 | GREEN |
| **Remove redundant/stale Downloads + GROWFOR zip + DRAFT md (approval-gated)** | — | **≈42 MB** | 0 | AMBER |

Key point (repeated because it is the packet's core concern): **moving bytes from active to archive is not the same as reducing total bytes.** The only true reductions available are deleting redundant/stale copies (~42 MB) and that only after the single-copy items are backed up.

## 10. Citation / provenance migration plan

Citation sweep results (exact filename + page-citation grep across `Docs/` and `specs/`):

- **Page-citation load-bearing sources:** `2006_VOL63_NO1&2-ss.pdf` (pp.53–79 / 80–95), the 38 book photos (pp.37–89 map in `source-inventory.md`), marking photos (pp.84–87), `CCF_Book_Definitive_Reference.md` (pp.39–42, 58, 84–87), `GROWFOR_Technical_Evidence_Extraction_v1.md` (G2a/G2b page cites), `CCF_Rack_Planning_…Extraction_v1.md` (pp.84–87). These cite **original** pagination and cannot be replaced by a re-paginated derivative without an explicit page map.
- **Cited-by-name in canonical files:** `Irish_CCF_Browsing_Protection_v1_Report.pdf`, `Understorey_Dynamics_Irish_CCF_Report.pdf`, `CCF_Primary_Literature_Synthesis_v1.pdf`, `CCF Species Evidence Pack v1…`, `CCF Academic Paper Consolidation…` (both), `Irish Atlantic Woodland…`, `Scenario Two Species…` (both), `Irish Forestry Economics…`, `Irish_Sitka_CCF_Deep_Research_Report.pdf`, `European_Pro_Silva…`, `Primary-Source Audit…`, `CCF-SITKA-GROWTH-EVIDENCE-AUDIT-V1B.pdf`, `Broad & Lynch`, `GROWFOR User Manual`, `McCullagh`, `earth-care-manual`.
- **Not cited anywhere canonical:** `2025Forestry_TimberYearbook.pdf`, `2018_Light_growth_responses_of_sitka_spruce.pdf`, `welch1991.pdf`, `CCF_Research_to_Game_Companion.pdf`, `Delegated Marking…pdf` (D-065 supporting but unnamed), `Model Capability and Cost Research — Repository Execution Report.pdf` (execution artifact of repo Markdown). These are safe to treat as archive-only, but still back up the single-copy ones.
- **Migration rule:** because no re-pagination is recommended, existing citations keep resolving to the same filenames in the archive. The only citation-affecting cleanup is the stale Downloads `GROWFOR_…v1.md`, which should not be cited (repo copy is canonical).

## 11. Checksum / manifest / rollback plan

- `SHA256-MANIFEST.tsv` (168 rows) lists every local corpus file with location, size and SHA-256 — the manifest of originals **before** any future cleanup.
- Restoration method (future, approval-gated): restore any removed redundant copy from its surviving byte-identical location or from Drive, then verify `sha256sum -c` against the manifest. The two project-pack zips and `attachments.zip` have no surviving byte-identical local twin; back them up to Files first if their retention is wanted.
- Rollback worst-case space: any strategy that *moves* bytes needs destination space equal to the moved bytes (~131 MB for photos) plus headroom; `/media/jer/Files` has 62 GiB free, so rollback is comfortably accommodated.

## 12. Ordered future execution steps

1. **Back up single-copy originals to `/media/jer/Files/CCF/Research/`** (copy, not move) and record in a `BACKUP-MANIFEST.tsv` (GREEN; requires no code, but is the gating action).
2. **Verify** `sha256sum -c` on the new archive copies against the manifest.
3. **Optionally remove, with explicit user approval:** the two 2026-10-02 packs, `attachments.zip`, GROWFOR request zip, stale Downloads GROWFOR md, Downloads `d05b27a` context snapshots, `CCF_Book_Definitive_Reference_DRAFT.md` (~42 MB). Each deletion lists the surviving authoritative copy.
4. **No further consolidation** (no bundles, no master PDF, no 7z) unless a later requirement justifies it.
5. **Optional documentation follow-up (separate task):** correct the stale rationalisation-package path in `research-index.md` (`/media/jer/ZX20/CCF-Verification/…` → `/media/jer/Files/CCF/Verification/…`).

## 13. Actions requiring user / Manager approval

- Any deletion or move of any research/source file (including the ~42 MB redundant Downloads set and any relocation of the book photos).
- Any PDF merge, bundle, watermark, cover-page insertion, recompression or OCR rewrite.
- Any change to Drive archive content (prohibited for this task).
- The optional research-index path correction (documentation-only, separate task).
- Moving (vs copying) the single-copy book photos out of `~/Documents/CCF Game/`.

## 14. Anything not accessed / verified

- **ChatGPT Project live source list / cap:** not directly queryable; the 26-source list is from the prior rationalisation and the 2026-10-12 `project-sources-after.json` snapshot (24 entries), not a live platform read. The current "active project source" count is therefore approximate.
- **Drive content hashes** verified by temp-download for only two files (2006 journal, GROWFOR md). The remaining Drive↔local byte-identity is inferred from **exact size match** (every Drive-reported size equals the local file size), not SHA equality.
- **`attachments.zip`** contents (3 photos, 2026-09-16) were listed but not assessed for relevance; flagged as "unclear" in `INVENTORY.tsv`.
- **`CCF_Research_to_Game_Companion.pdf`** and **`CCF_Scenario One UI Mockups.html`** not classified (not cited anywhere; low priority).
- **Non-CCF Downloads files** (installers, personal media) deliberately excluded as not corpus.
- **Unity worktrees, `Library/`, verification evidence and personal folders** excluded per scope.

---

## Recommended target state (concise)

- **Active project sources:** 15 files / **~1.3 MiB** (7 canonical + 4 individual research + 4 composites) + Scenario Two closure + GROWFOR extraction (repo).
- **Archived byte-identical originals:** all research PDFs + primary papers retained in Drive (and, after step 1, the 6 single-copy families in `/media/jer/Files/CCF/Research/`).
- **Project-facing composites/bundles:** 4 composites / ~0.13 MiB (unchanged).
- **Estimated active-space reclaimed:** **~42 MB** (Downloads + `~/Documents` redundant/stale copies, approval-gated); **0 MB on `/media/jer/Videos`**.
- **Restoration method:** `sha256sum -c SHA256-MANIFEST.tsv`; restore removed copies from surviving byte-identical location or Drive; manifest located at `Docs/Handoffs/Research-Source-Consolidation-Audit-01/SHA256-MANIFEST.tsv`.

**Headline:** the research corpus is too small and already too well rationalised for PDF merging to help. The correct plan is (1) back up ~131 MB of single-copy originals to durable storage, and (2) optionally reclaim ~42 MB of redundant/stale non-canonical copies. Neither action frees `/media/jer/Videos`; that disk's pressure is unrelated to research sources.
