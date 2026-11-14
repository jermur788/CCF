# MULTI-004 — Manager disposition: standalone Sitka 2D Lab P0 engineering qualification

**Date:** 14 November 2026  
**Manager record claim:** [#500](https://github.com/jermur788/CCF/issues/500), OpenAI GPT-6 GitHub-only Manager, no local host/worktree or local test execution.  
**Candidate source:** [MULTI-002 author's implementation and handoff](https://github.com/jermur788/CCF/blob/feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md) on \`feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1\` at independently pinned \`f33641f85ec8146ae26b91e4d5ea0b89bfba40f0\`; 50 new lab files and one handoff only compared to the exact approved-spec starting point.  
**Independent review:** [MULTI-003 / #497, Claude Opus 5.5](https://github.com/jermur788/CCF/blob/review/multi-003-sitka-2d-lab-p0-independent-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md), published and closed as HANDED-OFF, one additional review report only.  
**Formal P0 product scope:** [MULTI-001 approved Sitka lab spec](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md). User approved standalone research-only .NET+loopback browser, **NO BIOLOGICAL GROWTH**; raw Irish marteloscope import and empirical calibration remain deferred.

## 1. Manager verdict

**APPROVED — ENGINEERING P0 QUALIFICATION OF THE EXACT STANDALONE CANDIDATE, WITH ISOLATION AND EXPLICIT HARDENING DEBT.** The independent reviewer concluded APPROVED with **zero critical, zero major and six minor** findings. I accept that bounded verdict: source and actual executed tests support a functional lab with deterministic synthetic inventory, accurate geometry/basal-area arithmetic, atomic mark/approval/work semantics, replayable event history and a working local-only 2D UI. **Not merged, not a final player product, not user acceptance, not a Unity/Scenario One release, not P1 biological validation.**

The Manager independently inspected the #497 GitHub report, #494 handoff, GitHub branch compare (51 added files scoped to lab + handoff) and \`LabDataStore.cs\` read code. The Manager did **not** independently access the laptop, screenshots, native evidence directory or rerun dotnet/Chromium. The reviewer **did** report fresh laptop execution and evidence integrity; retain this distinction.

**Real caveat:** F-1 and F-2 are genuine defects against the intended **complete save-isolation safety rule** under a hostile or concurrently modified local lab directory. Their trigger requires a same-user local process capable of creating/replacing entries inside \`.lab-data/\`; the HTTP/UI cannot create those entries. Thus they are **non-blocking for this research-only P0 acceptance**, but **must be repaired and independently delta-checked before relaxing the local-file trust model, enabling real external data import, integrating into shared gameplay or asserting complete save-root confinement**. Do not label AC-07 unconditional PASS for adversarial local writers.

## 2. Verified by the independent reviewer, not merely the original writer

| Area | Independent report | Manager classification |
|---|---|---|
| Clean .NET build | 0 warnings, 0 errors | **REPRODUCED BY INDEPENDENT REVIEWER** |
| Core unit suite | 55 / 55 PASS | **REPRODUCED** |
| Host/API/security suite | 8 / 8 PASS | **REPRODUCED** |
| Fixture files / schema | 3 pinned fixtures byte-identically regenerated; six invalid schema cases rejected | **REPRODUCED** |
| Original mutation controls | 17 / 17 CAUGHT on full independent scratch rerun, including previously surviving M17 | **REPRODUCED** |
| Reviewer extra 22 mutants | 14 caught, 8 survived; guard redundancy and test coverage rather than presently demonstrated exploitable public API | **REVIEWER FINDING; MAINTENANCE RISK** |
| Browser real-input test | headless Chromium 15 / 15 PASS, two actual screenshots examined | **REPRODUCED; ONE BROWSER ENGINE / HEADLESS ONLY** |
| 2,000-tree UI | 157 ms max selection/inspector/preview, 2,000 ms budget | **REPRODUCED ON REVIEW LAPTOP** |
| Forged consistent-hash events | illegal histories rejected, including exact requeue case | **REVIEWER NEW PROBES PASS** |
| Save-root security | static path/symlink and CCF JSON header checks PASS; two local race/special-file cases reproduced | **PARTIAL UNDER CONCURRENT SAME-USER FILE MODIFICATION** |
| Windows, WSL, Firefox, headed graphical browser | not run | **NOT TESTED**, not required for isolated P0 approval |
| Real forestry prediction, measured Irish trees | not implemented | **NOT VALIDATED / OUTSIDE P0** |

The test evidence is retained **off-repo** at \`/media/jer/Files/CCF/Verification/MULTI-003-SitkaLab-Review-20261114/\` with 38 hashed evidence items reported by the reviewer. Source SHA/branch evidence is on GitHub; Manager did not mount/access that evidence root.

## 3. Finding dispositions and required follow-up

| Finding | Manager disposition | Why / next action |
|---|---|---|
| **F-1 FIFO hang** | **Accepted as minor locally, repair priority P1 for hardening** | Direct in-root FIFO is listed as loadable and can block the host lock. Require regular-file-only bounded handle read, no FIFO/socket/device hang; no extra browser/Unity qualification. |
| **F-2 symlink TOCTOU escape** | **Accepted as minor locally, repair priority P1 for hardening** | Real same-user symlink race returned outside bytes. Require OS-appropriate atomic no-follow / root-anchored file handle and post-open regular-file checks; **a second path check followed by \`ReadAllBytes\` is not a repair**. No unsupported blanket security claim across platforms. |
| **F-3 misnamed test** | **Correct in focused repair** | Test does not exercise per-entry check despite its name. Rename honestly or exercise the intended condition. |
| **F-4 redundant guard tests** | **Correct in focused repair** | A later refactor can silently drop one guard. Add tests/forged event replay checks and targeted mutation controls, do not remove layered defence just for passing tests. |
| **F-5 temp leak/error code** | **Correct in focused repair** | \`File.Move\` failure leaves temp file and Deny returns misleading \`Forbidden\` code. Ensure cleanup and proper typed 4xx/5xx failure handling. |
| **F-6 fixture “hand-audited” wording** | **Non-blocking documentation discrepancy** | Only the 3-tree oracle is hand audited. The 162 and 2,000 stem fixtures are deterministic formula-derived with independently checked hashes. Any future README/handoff must say so; formal approved-spec wording amendment requires authorised spec steward, not unapproved unilateral source/spec changes. |

**Important:** The review's risk classification is not a claim that symlink races and special files are harmless. It is conditional on the lab's current controlled, single-user, local-only engineering environment and no untrusted process writing to the data root. Do not start importing third-party datasets or exposing lab-file root outside that environment.

## 4. Exact next executable work, no new implementation project

**MULTI-005 — bounded file safety, test confidence and error handling repair** should be issued as a separate one-writer claim on a branch starting at the **exact original MULTI-002 candidate**, retaining the accepted P0 state until delta review. Tasks F-1 through F-5 only, plus factual fixture labelling in lab README if convenient. Run targeted Core/Host/file safety and existing browser smoke; one independent focused security/regression delta review. Do not redo all 50 files, rerun the full stand ecology stack or repeat Unity tier-3. This is proportionate in effort and prevents future ambiguity.

**Source ownership collision:** a separate older [#491 P0 implementation claim](https://github.com/jermur788/CCF/issues/491) remains **READY / NOT RUNNING** on a different approved-spec fork. It does **not** authorize parallel writing to \`Tools/MultiRackSitkaLab/**\`. Manager must notify the portfolio owner and local worker to treat the completed MULTI-002 candidate as the sole current source and avoid launching #491 while MULTI-005 is active. Do not overwrite/close #491 without its actual owner determining disposition. The live coordination board had no MULTI row at prior preflights; that **absence is not a substitute for acquiring current subsystem/host ownership before any new edits**.

**Separate future gates:** (1) real marteloscope data: BLOCKED pending actual licensed stem table; (2) P1 growth/density model: source/evidence-specific spec approval; (3) 2D↔3D/Scenario One: new cross-system spec, integration and Unity qualification. **No Windows game build** without the user's personal Scenario One candidate playtest and explicit authorisation.

## 5. Status and user action

- **Changed:** independent engineering P0 accepted as an isolated lab, review history preserved.
- **Passed:** reviewer reproduction of Tier1/Tier2 and 17 original mutation safeguards; geometry and action semantics.
- **Failed/limited:** local file trust under concurrent same-user directory mutation (F-1/F-2), plus F-3…F-6 minor maintenance/docs. No headed/Firefox/Windows/WSL coverage claimed.
- **Next:** MULTI-005 focused hardening with new worker ownership and subsequent independent delta review; no unapproved new gameplay or biology.
- **User action:** no product decision needed. To begin local repair, open the forthcoming MULTI-005 handoff in a local code chat, after ownership preflight.
- **Dashboard/Drive mirror synchronization:** NOT VERIFIED. No source or local Unity work was done in this Manager session.
