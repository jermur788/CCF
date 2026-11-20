# 01 — Source and version inventory

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
Machine-readable form: `01-source-inventory.tsv`.

## 1. Where the material actually lives

The packet anticipated Drive-only custody. In practice the owner's desktop already holds a complete, hash-recorded
copy of the Drive/Project material, which is why this audit could run at all:

- `C:\Users\hp\Documents\CCF Papers\` — the six source papers, the two extraction PDFs, and the straight-five
  academic articles.
- `C:\Users\hp\Documents\CCF Research Archive\860-drive-ccfgame-20261120\` — the 20 November desktop archive:
  four composite **Project attachment copies** (`project-attachments-copy-20261120\`), Drive mirrors
  (`originals-from-drive\`, `originals-from-drive-03-ecology-research\`), the #860 addendum, and
  `work-text\` — text extraction of **175 PDFs** with a page index (`_pages.tsv`, `_list.txt`).

Nothing was copied into Git. Research binaries stay out of the repository, as `research-index.md` requires.

## 2. The six papers and their extractions

| ID | Paper (short) | In `CCF Papers\` | Extraction coverage |
|---|---|---|---|
| P1 | Pommerening, Maleki & Haufe 2021 — individual-based forest management | `individual based forest management.pdf` | extraction pp. 6–7, source pp. 3–14 |
| P2 | Pommerening et al. 2018 — rating experiments / tree marking | `Rating experiments in forestry.pdf` | pp. 8–9, source pp. 15–34 |
| P3 | Pommerening 2002 — approaches to quantifying forest structures | `approaches to quantifying forest structures.pdf` | pp. 10–11, source pp. 35–54 |
| P4 | Pommerening et al. 2021 — CanopyShotNoise | `CanopyShotNoise …` | pp. 12–13, source pp. 55–84 |
| P5 | Myllymäki et al. 2024 — Bayesian spatial forest dynamics | `a beyesian approach toproject forest dynamcs.pdf` | pp. 14–15, source pp. 85–98 |
| P6 | Pommerening, Szmyt & Duchiron 2024 — revisiting silvicultural systems | `revisiting siliculturar systems.pdf` | pp. 16–17, source pp. 99–111 |

Both extraction documents are present and internally consistent with each other:

| File | Bytes | SHA-256 (first 16) | Pages |
|---|---|---|---|
| `CCF_Six_Papers_Relevance_Evidence_Extraction.pdf` | 131,279 | `f8a699d96d5146d0` | 24 |
| `CCF_Six_Complete_Forestry_Research_Papers.pdf` | 17,650,706 | `4f1d75578d25e8dd` | 111 |

The source register's page arithmetic checks out: P1 12 pp. (3–14), P2 20 pp. (15–34), P3 20 pp. (35–54),
P4 30 pp. (55–84), P5 14 pp. (85–98), P6 13 pp. (99–111) = **111 pages**, matching the companion volume. The
extraction is therefore not a summary of an abstract set: it indexes complete papers.

## 3. The four composites — byte state and version skew

All four are present as **Project attachment copies**. Only **one** has a committed Git counterpart.

| Composite | Attachment bytes | Attachment SHA-256 (first 16) | Git `main` counterpart | Git bytes | Verdict |
|---|---|---|---|---|---|
| Ecology and Regeneration | 37,540 | `85b23b2bda00f7e5` | **none** | — | Git counterpart missing |
| Browsing Protection and Management | 27,209 | `004756b693c4b159` | **none** | — | Git counterpart missing |
| Growth Density and Species Quantitative | 33,058 | `12ddc2b1d42aea31` | `Docs/Research/CCF_Growth_…Composite.md` | 35,555 | **STALE ATTACHMENT** |
| CCF Practice Economics and Transformation | 34,910 | `cb1c2421d348fc66` | **none** | — | Git counterpart missing |

The attachment byte counts match the #860 roster exactly (37,540 / 27,209 / 33,058 / 34,910), so the archive copy is
faithful to what is attached.

### 3.1 The Growth composite is behind — verified, not inferred

- Git `main` copy: 35,555 B, SHA-256 `2704b0aa689f3120…`.
- Drive `03 – Ecology Research` copy: SHA-256 `2704b0aa689f3120…` — **byte-identical to Git**.
- Project attachment copy: 33,058 B, SHA-256 `12ddc2b1d42aea31…` — **differs**, 751 differing lines under a
  windowed comparison.

The attachment lacks the **`V1B EVIDENCE UPDATE (23 October 2026)`** block and the revised cross-reference text
(Broad & Lynch 2006a/2006b evidence-role split, V1B 2006a/2006b fields, G3/G4 NFI statistics pointer). The
canonical Git/Drive copy carries it.

**Consequence for #425:** the Project attachment set is serving an older Growth composite than the canonical one.
This is a version-preservation defect, not a science defect, and it is *not* fixed here — replacement needs #425.

### 3.2 Three composites exist only as attachments

Ecology, Browsing and Practice have no committed Git file. Their only durable copy is the Project attachment plus
the Drive mirror plus this desktop archive. That is a preservation exposure independent of science quality: if an
attachment is replaced, no Git diff records what changed.

This is recorded as an open item for #425 in `05-UNRESOLVED-EVIDENCE.md` (U-1). It is not repaired here: creating
Git counterparts is a canonical-publication act the packet explicitly reserves to #425.

## 4. Other retained research — presence and status

| Item | Present | Status found |
|---|---|---|
| `GROWFOR_Technical_Evidence_Extraction_v1.md` | Git, `Docs/Research/SitkaGrowthMortality/` | Committed; cross-referenced, not duplicated |
| `perrin2011.pdf` | archive, 699,685 B, SHA-256 `597c1210…` | **Covered** in Ecology §4/§5/§13 as `IRISH DIRECT` |
| `davies2008.pdf` | archive, 721,792 B, SHA-256 `69a5f59a…` | **Covered** in Growth §8 (crown) and cited in Ecology |
| `2025Forestry_TimberYearbook.pdf` | archive, 3,490,089 B, SHA-256 `a67be3bd…` | **Not incorporated** — see matrix row O-03 |
| `CCF Species Evidence Pack v1 …pdf`, `CCF Academic Paper Consolidation …pdf` | archive | Pointer/roster documents (Conry/Dunbar/Moore/Pretzsch), **not** P1–P6 |
| #860 nine-report addendum | branch `docs/860-desktop-drive-consolidation-addendum-20261120-v1`; desktop copy 11,934 B, SHA-256 `b0afc312…` | Still `DRAFT / SECONDHAND / CONFLICTS OPEN`; unchanged by this packet |
| `research-index.md` | Git | Read; used as the authority-boundary reference |

## 5. What this inventory does and does not prove

**Proves:** which files exist, their exact bytes and hashes, that the six-paper extraction covers six complete
papers totalling 111 source pages, that three composites lack Git counterparts, and that the Growth attachment is
a stale revision.

**Does not prove:** that the Drive/Project attachments are byte-identical to the desktop copies beyond the four
composites hashed here; that any Project attachment was uploaded or replaced; or that the #860 addendum's own
figures are correct (its conflicts stay open in `05-UNRESOLVED-EVIDENCE.md`).
