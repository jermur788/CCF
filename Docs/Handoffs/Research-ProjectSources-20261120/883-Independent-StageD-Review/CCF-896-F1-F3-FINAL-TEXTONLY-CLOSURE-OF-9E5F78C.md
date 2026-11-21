# CCF #896 — Final text-only closure of #883 F-1 / F-2 / F-3 at `9e5f78c`

**Reviewer:** original independent Claude Sonnet 5.5 #896 reviewer (not the DeepSeek V4.1 Flash #883 producer).
**Date:** 2026-11-21. **Difficulty:** LOW, text-only. No PDFs were opened and no wider scientific re-review was done.
**Status:** research review only. Not canonical, not published, no merge. Approval here does not authorise canonical publication, publishing 08, applying 09, a Project attachment, roster change or gameplay change.

## 1. Verdict

| | Verdict |
|---|---|
| **OVERALL** | **APPROVED** |
| F-1 (closes R-11 [B]) | **CLOSED** |
| F-2 (closes R-23) | **CLOSED** |
| F-3 (closes R-7 [B]) | **CLOSED** |
| (i) Source traceability | **APPROVED** |
| (ii) Scientific quantities | **APPROVED** (carried forward by citation from the delta review; not re-adjudicated) |
| (iii) Coverage completeness | **APPROVED** |
| (iv) Practical suitability | **APPROVED** (carried forward by citation; not re-adjudicated) |

With these three closures, the delta review's **28 of 28 R-IDs are CLOSED, including all 14 blocking ones** (delta review `f55586de`: 25 closed, R-7, R-11 and R-23 open). The only items outside this closure are the optional, non-blocking notes N-1…N-7 from that review (still unchanged by the producer, as the Manager's packet says) and one new non-blocking note, N-8, in §7.

## 2. Pins, claim and independence

| Item | Value |
|---|---|
| Previously reviewed producer | `20632cb03eb67c7e731de5c3981597cc854ed048` |
| Corrected producer (reviewed now) | `9e5f78cbc6079f486b717bf39b63dc807e71978a`, branch `research/883-condensed-evidence-coverage-review-20261120-v1` |
| Basis | #896 delta review `f55586de01292c620ed4c9c0938333008e28d725`, §8 F-1/F-2/F-3 |
| Handoff | branch `docs/ccf-896-f1-f3-textonly-closure-9e5f78c-20261121-v1`, file `CCF-896-MANAGER-F1-F3-TEXTONLY-INDEPENDENT-CLOSURE-HANDOFF-20261121.md` |
| Claim | registered on #896 **before any file was written**: https://github.com/jermur788/CCF/issues/896#issuecomment-7070410907 |
| Live state at start | main `3a67d0987be0cf40d007892739d2ad295c84f0e0`; `active-tasks.md` and the #883/#896 comments refreshed; no competing #896 reviewer claim; target path free on main |
| Independence | I did not write or edit any producer file. The Manager's handoff is treated as a claim to verify, not as evidence. Every check below was re-done from the GitHub file contents at the two pinned SHAs. |

## 3. Scope of the producer change (independently confirmed)

GitHub compare `20632cb…9e5f78c`: **ahead 1 / behind 0, one commit** (`9e5f78c`, author "DeepSeek V4.1 Flash", "docs(research): #883 F-1/F-2/F-3 editorial closure"), **exactly three files, +9 / −10**, all inside `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/`:

| File | Change | Blob at `9e5f78c` |
|---|---|---|
| `11-R1-R28-DISPOSITION-AND-CORRECTION-HANDOFF.md` | +5 / −3 | `430bb2da83109871fcf9645140634458192fa3e2` |
| `02-coverage-matrix.tsv` | +1 / −5 | `e0a16940c282f6dd624db0ab9c69bc7865b881b5` |
| `09-PROPOSED-860-ADDENDUM-P2-CHANGE.md` | +3 / −2 | `7e1de5de8ef80276a865d754543c7a0082fccc2e` |

No other path changed: 08, the other producer files 00–07 and 10, the four composites, the #860 actual addendum, the #896 reviews, `Docs/Research/**`, `Docs/Project/**`, Drive, Unity and the game source are all untouched by this commit, because the compare lists only the three files above.

## 4. F-1 (closes R-11): file 11 closing paragraph — CLOSED

My delta review required replacing the ambiguous sentence at lines 71–73 with a text that names **five** status changes. The new lines 71–75 read:

> This replaces the 67 / 44 / 13 / 6 / 3 / 1 tally. The movement is five status changes (`P2-01` and `P2-04` `DISPUTED` → `MATERIAL GAP`; `P5-09` `MATERIAL GAP` → `CROSS-REFERENCED-INTENTIONALLY`; `P2-09` and `P6-09` → `DESIGN NOTE`) and one added row (`P6-12`). `DESIGN NOTE` was introduced for rows that were never source findings. The two P2 rows are no longer disputed because verification resolved the dispute in favour of the source; only `O-04` remains `DISPUTED`.

This is word-for-word the replacement text I specified in §8 F-1; the diff touches only that paragraph. **I then checked it against the data, not against the prose.** Parsing the TSV at `053686b` (the originally reviewed matrix) and at `9e5f78c`:

| Row | `053686b` | `9e5f78c` | Named in file 11? |
|---|---|---|---|
| P2-01 | DISPUTED | MATERIAL GAP | yes |
| P2-04 | DISPUTED | MATERIAL GAP | yes |
| P5-09 | MATERIAL GAP | CROSS-REFERENCED-INTENTIONALLY | yes |
| P2-09 | CROSS-REFERENCED-INTENTIONALLY | DESIGN NOTE | yes |
| P6-09 | COVERED | DESIGN NOTE | yes |
| P6-12 | (absent) | MATERIAL GAP | yes, as the added row |

- Exactly **five** rows changed status, and they are exactly these five; none were removed; one added. The paragraph's count and mapping are correct, and the TSV is the source of truth (it needed no change).
- The tally arithmetic closes: 67 → 68; MATERIAL GAP 44 +3 (P2-01, P2-04, P6-12) −1 (P5-09) = **46**; CROSS-REFERENCED 13 −1 (P2-09) +1 (P5-09) = **13**; COVERED 6 −1 (P6-09) = **5**; DISPUTED 3 −2 = **1**; DESIGN NOTE 0 +2 = **2**; OUT OF SCOPE **1**.
- `P2-01` and `P2-04` are `MATERIAL GAP` in the actual matrix, and `O-04` is the only `DISPUTED` row. The old "left `DISPUTED`" ambiguity and "four status changes" miscount are gone. A search of all 14 producer files at `9e5f78c` finds no remaining "four status changes".
- The remaining mentions of the old 67 / 44 / 13 / 6 / 3 / 1 tally are in file 10 (carries its own "superseded by 11" banner) and in file 11's "replaces" sentence; both are correctly historical.

## 5. F-2 (closes R-23): TSV comment header — CLOSED

Required: change line 2 to `paper_page`; delete the duplicated block (old lines 8–11) and the stray ` | DESIGN NOTE` suffix on old line 7.

- **Line 2** now reads `# Columns: row_id, source, paper_page, topic, claim_or_quantity, units, evidence_class, composite_location, coverage_strength, status, action`. `original_pdf_page` appears nowhere in the TSV. (It appears once in `02-COVERAGE-MATRIX.md` line 134, where it explains the rename; that is correct.)
- **Comment block** went from 12 lines to 8, each once: title, columns, status legend, DESIGN NOTE definition, `paper_page`/offset line (P1 +2, P2 +14, P3 +34, P4 +54, P5 +84, P6 +98, with the P4 p.12 coincidence), the "not exhaustive; P6-12 is an added candidate row" line, and `coverage_strength`. No duplicate comment line remains.
- **Line 7** now ends at `…P6-12 is an added candidate row.` with no suffix. (My own first-pass regex for " | DESIGN NOTE" matched the status legend on line 3, `… | OUT OF SCOPE | DESIGN NOTE`, which is correct; I confirmed line 7 by reading it directly.)
- **Data rows unchanged — checked independently:** the non-comment lines at `20632cb` and `9e5f78c`, joined on LF, are **byte-identical**. The new file has one header row of **11** fields plus **68 data rows, every one with 11 fields**, **68 unique IDs**, no empty ID/source/status cell, no CR characters, same UTF-8 encoding as before.
- **Status tally:** 46 MATERIAL GAP / 13 CROSS-REFERENCED-INTENTIONALLY / 5 COVERED / 2 DESIGN NOTE / 1 DISPUTED / 1 OUT OF SCOPE = **68**, as stated in file 11.
- **Per-source counts are identical between `20632cb` and `9e5f78c`** (P1 10, P2 8, P3 7, P4 6, P5 13, P6 11 under the short label; one row per paper under the long paper name; seven non-paper rows). The total remains 68. P6 is 11 because of the added P6-12.

## 6. F-3 (closes R-7): file 09 P1 attribution — CLOSED

Required: add the P1 source for the Ae plot size, mirrored in the "Study description" row.

- **Proposed text (line 28):** "marteloscopes 0.1 ha each (Ae 0.133 ha per **P1 Table 1, p. 7**; P2 Table 2 gives 0.1 ha), 19 rater groups of 9-20 …"
- **Study description row (line 44):** "adds 0.1 ha plots (Ae 0.133 ha per **P1 Table 1, p. 7**; P2 Table 2 gives 0.1 ha), 19 rater groups of 9-20".
- Both required locations are present, character-for-character the same parenthetical, and the explicit page is **p. 7** (paper-relative; collection c.9, matching 03 V-5 "P1 Table 1 (P1:7 / c.9)" and 08's "(P1:7)"). The unqualified "except Ae at 0.133 ha" that sat under P2 page pointers is gone.
- Preserved, unchanged: the P2 result pointers (P2 pp. 1, 11, 14, 16, 17; Table 3, p. 9; p. 11), the MART secondhand-report-plus-rights-tiers bullet, the "addendum's author still did not read the six papers" line, and the "Deliberate non-changes" section. 09 remains labelled as a proposal and is **unapplied**; the actual #860 addendum was not touched.
- Whole-subtree consistency: every other 0.133 ha mention (03 V-5, 07, 08 §4 and Table 2 note, 11 R-7 row) already carries the P1 attribution; the one that did not (09) is now fixed.

## 7. Caveats and non-blocking notes (none holds the verdict)

- **N-8 (new, non-blocking): TSV `P2-01` claim cell.** It still reads "marteloscopes 0.1 ha (Ae 0.133 ha)" with page pointer "1, 4-6" and no P1 attribution. The next row, `P2-02`, does carry the "P1 Table 1 gives Ae 1,321/ha … n=176" cross-reference. My delta review said both P2-01 and P2-02 carry "(P1:7)"; for P2-01 that was too generous. F-3 asked only for file 09 and the Manager froze the data rows, so this was correctly not changed. If the producer next edits the matrix for any other reason, append "(P1 Table 1, p.7)" to the `P2-01` cell.
- **09 line wrap (cosmetic).** In the proposed text the new parenthetical leaves a short line ("groups of 9-20; low-thinning kappa_F"). This is inside a fenced block and does not alter meaning.
- **"P2 Table 2 gives 0.1 ha"** is carried forward from F-3 in my delta review. It was not re-read against the PDF in this pass because the scope is text-only. It is consistent with 03 V-5 and 08's own "0.1 ha each except Ae" wording.
- **N-1…N-7** from the delta review are still open by choice and stay optional. Producer 08/09 remain proposals.
- **Rights and publication gates are not changed by this review.** P4 and P6 are CC BY-NC-ND; P3 is © Institute of Chartered Foresters; P1, P2 and P5 are CC BY; 08 transcribes all 15 P4 Table 1 values. Whether 08 may sit in a Project attachment, and any canonical publication, 09 application, roster or gameplay decision, stay with #425 and the owner.
- **Design versus evidence:** unchanged from the delta review §9; F-1 to F-3 only move wording and a citation and do not convert any producer inference into a source finding.

## 8. Not checked

- No PDF was opened or rendered. No new literature was retrieved.
- The 28-item audit and the six-paper audit were not repeated. The 19/67 rows and 8/77 assertions the original review did not audit are still unaudited.
- Project attachment slots, Drive and the actual #860 addendum were not opened.
- Dimensions (ii) and (iv) are carried forward from `f55586de` by explicit citation; this pass changed no number and no design statement, so nothing there needed re-adjudication.

## 9. Next handoff

Return to the Manager for disposition. The natural next gate is a #425 decision packet covering rights, version and attachment/canonical questions. This review does not choose any of them. No PR was opened and nothing was merged.

## 10. Publication record

- **Branch:** `docs/883-independent-stage-d-f1-f3-closure-20261121-v1`, created from live main `3a67d0987be0cf40d007892739d2ad295c84f0e0`; one added file, nothing else. No PR, no merge.
- **File:** `Docs/Handoffs/Research-ProjectSources-20261120/883-Independent-StageD-Review/CCF-896-F1-F3-FINAL-TEXTONLY-CLOSURE-OF-9E5F78C.md`.
- The document cannot contain its own commit SHA. The exact commit, the byte-identical readback hash, the base comparison and the three notification links are recorded in the #896, #883 and #425 comments that carry this review's direct link.
