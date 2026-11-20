# 07 — Independent review packet (Stage D, not yet performed)

**This file is a brief for a reviewer who has not written any of the above.**
The reviewer must be a genuinely separate agent/thread. Per the task packet, the producer and any helper or child
of the producer do **not** count as independent. If no such reviewer is appointed, the correct end state for #883
is `RESEARCH PRODUCED / AWAITING INDEPENDENT REVIEW`, which is where this stands.

## 1. What is being reviewed

| Artefact | Path |
|---|---|
| Coverage audit packet | `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/` |
| The audited extraction | `C:\Users\hp\Documents\CCF Papers\CCF_Six_Papers_Relevance_Evidence_Extraction.pdf` (24 pp) |
| The verification oracle | `C:\Users\hp\Documents\CCF Papers\CCF_Six_Complete_Forestry_Research_Papers.pdf` (111 pp) |
| The four composites | `C:\Users\hp\Documents\CCF Research Archive\860-drive-ccfgame-20261120\project-attachments-copy-20261120\` |
| The #860 addendum | archive root, `CCF_2026-11-20_New_Reports_Evidence_Addendum.md` |

Text extraction of every PDF is in `...\work-text\` (`_pages.tsv` maps file numbers to paths and page counts).

## 2. What the reviewer must decide

Give a verdict on each dimension separately. A single overall "looks fine" is not a usable verdict.

| # | Dimension | The question |
|---|---|---|
| i | **Source traceability** | Can every figure in the matrix be followed back to an original page in the 111-page volume? Are any citations invented, off-by-one, or attributed to the wrong paper? |
| ii | **Scientific quantities** | Are the reported numbers, units, sample sizes and geographies correct? Is V-4's correction right? Is V-9 honestly labelled? |
| iii | **Coverage completeness** | Are the 43 `MATERIAL GAP` verdicts defensible, or does the token-sweep method miss existing coverage? Are the 13 `CROSS-REFERENCED` calls under- or over-stated? |
| iv | **Practical suitability** | Is the proposed single new composite a sane project attachment, or does it duplicate/compete with the existing four? Is it usable at retrieval time? |

## 3. Required sampling (minimum, not maximum)

1. **Re-derive two matrix rows from scratch** — one `MATERIAL GAP` (suggest P3-01 or P5-09) and one
   `CROSS-REFERENCED` (suggest P1-01 or P2-09). Try to prove the producer wrong.
2. **Open the original page for every `DISPUTED` row** (P2-01, P2-04, P4-04, O-04) and for V-4 and V-11.
3. **Re-run one concept sweep** with your own vocabulary against all four composites — the producer's token lists
   are published in `02-COVERAGE-MATRIX.md` §2 for exactly this test.
4. **Check every number in `03-PRIMARY-VERIFICATION-LOG.md`** that is marked CONFIRMED against the rendered page,
   not the text layer, if you have PDF access.
5. **Read `04-PROPOSED-INCORPORATION.md` §3** and confirm nothing there quietly authorises a mechanic.

You need not read all six papers end to end. Nothing in the packet depends on a full re-read.

## 4. Specific things to challenge

- **The bulk verdict.** The packet claims the composites barely overlap the six papers. If that is wrong — if the
  composites do cover marking agreement or structural indices under other wording — the entire recommendation
  changes.
- **P3-08 and P6-11 as gaps.** Negative/limitation claims were scored as gaps. Is that inflation?
- **P5-09 as a gap.** It supports the status quo; the packet calls it high-value. Is that reasoning sound?
- **The V-4 correction.** If the producer misread which rows belong to which parameter, the correction is itself
  an error and must be withdrawn.
- **Scope discipline.** Did anything in the packet stray into mechanics, decisions, attachment changes or
  canonical claims?

## 5. Verdict format

```
VERDICT: APPROVED | NEEDS CHANGES | REJECTED
(i)  Source traceability:   <verdict> — <evidence>
(ii) Scientific quantities: <verdict> — <evidence>
(iii) Coverage completeness:<verdict> — <evidence>
(iv) Practical suitability: <verdict> — <evidence>

REQUIRED CORRECTIONS: <numbered, each with the file and the exact change>
OPTIONAL IMPROVEMENTS: <numbered>
RESIDUAL RISK: <what remains unverified and why that is acceptable or not>
```

`NEEDS CHANGES` requires the producer to apply the corrections and the reviewer to close them explicitly. A
structural-only review ("the sections are well organised") is not a scientific review and must be rejected as a
verdict of record.

## 6. What the reviewer must not do

- Do not edit the composites, the addendum, `Docs/Research/**`, `Docs/Project/**` or any attachment.
- Do not approve publication, roster membership or a Drive change. That is #425.
- Do not treat this packet as evidence for a gameplay change.

## 7. State of the gate

| Label | State |
|---|---|
| RESEARCH PRODUCED | **yes** — this packet |
| INDEPENDENTLY REVIEWED | **no** |
| CANONICALLY APPROVED / PUBLISHED | **no** |
