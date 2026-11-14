# MULTI-001 — Irish Sitka 2D Reference Laboratory: P0 approval decision

**Date:** 14 November 2026  
**Status:** **AWAITING USER / DESIGNATED OVERALL MANAGER APPROVAL — NO IMPLEMENTATION RELEASE**  
**Decision claim:** [#493](https://github.com/jermur788/CCF/issues/493)  
**Prepared by:** OpenAI GPT-6, dedicated Multi-Rack Simulation Manager; GitHub/browser docs-only, no local code workspace or Unity lease. The author is **not** the person authorised to self-approve their own DRAFT.
**Purpose:** Convert independently qualified, corrected four-file Sitka-first research-lab DRAFT into a user/Overall Manager **P0-only** scope decision. If approved, only then prepare a separate, uniquely claimed T3 Code local implementation handoff.

## 1. Decision-ready evidence and custody

- [Original #485 independent review: NEEDS CHANGES](https://github.com/jermur788/CCF/blob/616bd5376986b668589553d4971de7ac2888df41/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-V1-DRAFT-SPEC-REVIEW-20261114.md).
- [#486 author revision branch: four-file corrected DRAFT](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-485-review-repairs-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md), with [plan](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-485-review-repairs-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/plan.md), [tasks](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-485-review-repairs-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/tasks.md) and [requirements](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-485-review-repairs-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/checklists/requirements.md). The independently pinned revision contains exactly four modified DRAFT files against the prior author version; no code/main/Unity/coordination changes.
- [#488 independent Claude Opus 5.5 full delta-check](https://github.com/jermur788/CCF/blob/fc7384226843aee3abb9314de0a0eb30cd64576e/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md): **APPROVED AS REVISED DRAFT QUALITY**, all 18 original findings D-01–D-18 PASS; no medium/blocking regression. Review is evidence of draft quality **not** formal spec approval.
- [#479 real-data availability](https://github.com/jermur788/CCF/issues/479) remains **blocked/not inspected** for any exact Irish marteloscope tree table. It is **not a prerequisite** for a wholly synthetic P0 engineering laboratory.

The candidate **is intentionally not the Unity game**, and no physical host, source implementation branch, saved-world fixture, source code merge, approval or Windows build has started.

## 2. Three mandatory decisions — recommend YES on all three

| Decision | Recommended owner-approved P0 boundary | Alternatives and consequences |
|---|---|---|
| **OPEN-L1 — purpose** | **YES: standalone research/engineering test laboratory only**, no new player-facing 2D game mode yet. | A distributable in-game 2D interface requires a separate approved player workflow spec and safety/integration programme. |
| **OPEN-L2 — biology** | **YES: P0 has NO biological growth**. It validates tree IDs, synthetic positions/DBH/height, plot geometry, basal area/N/Dq, exact selection, preview vs work, atomic execution and lab-only event history. “Advance experiment step” changes only calendar/event state. P1 real annual growth/competition/mortality is **separately approved later** after source/model and Irish-evidence qualification. | Requiring growth in P0 would need a new science/source gate; the current reviewed DRAFT cannot authorise it. |
| **OPEN-L3 — technical isolation** | **YES: independent .NET C# Core + 127.0.0.1-only local browser UI**, subject to preflight verification of available .NET/browser tooling. One implementer owns a wholly new \`Tools/MultiRackSitkaLab/\` directory; no Unity saves, game code or production field-data import. | A different stack requires a reviewed spec amendment if it changes the accepted deterministic core/trust/data boundary. |

**Explicit deferrals:** **OPEN-L4** licensed true marteloscope/Irish full mapped inventory and **OPEN-L5** empirically calibrated Irish stand default/growth coefficients are **not part of P0 approval**, and no premature assumption should clear them.

**Choice/authority:** The **user** is the product owner. A **designated Overall Manager acting for the user** may approve the spec under the current AgentWorkflow. The Multi-Rack author, a reviewer or an implementation worker must **not** convert the DRAFT to APPROVED without this explicit product approval; #424 Scenario One's delivery and live-board role does not automatically confer this standalone tool's approval. Status after this packet alone remains DRAFT.

## 3. Optional reviewer N-01–N-07 editorial clean-ups — not blockers

The [#488 report, §4](https://github.com/jermur788/CCF/blob/fc7384226843aee3abb9314de0a0eb30cd64576e/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md) lists seven **LOW, nonblocking** text adjustments with exact suggested changes:
1. Every snapshot-changing mark/plan/step action emits exactly one sequenced event.
2. Clarify that the runtime \`sessionEpoch\` is opaque and load forces saved approved/pending plans STALE; replay uses approval event's stored epoch value, not the current runtime.
3. Distinguish path/symlink **rejection before file-open** from lab-root CCF-shaped JSON **rejection after header read, before forest/event parsing**.
4. Remove residual “Irish/generated/seed/annual” language from synthetic P0 and replace with checked-in fixture/experiment-step terminology.
5. Align polygon units to persisted integer mm, approval property name \`approvalSessionEpoch\` and tree status spelling.
6. Restrict IDs to a safe explicitly specified ASCII character set.
7. Add an optional P0 no-top-height checklist row and define “revised fixture” as load/reset.

They may be **applied as strictly editorial changes by the designated approval/spec steward with source attribution and recorded bounded delta**. The independent reviewer explicitly states these do not need another reviewer run. **Do not alter the already-reviewed DRAFT just to create another review loop.** If any cleanup changes semantics/forest physics/scope, stop and seek separate review.

## 4. Approval path and follow-up execution — CONDITIONAL

**Step A — product decision (now).** User or designated Overall Manager selects OPEN-L1, OPEN-L2 and OPEN-L3 explicitly and records approval decision on [#493](https://github.com/jermur788/CCF/issues/493) or equivalent durable project authority. If **not approved**, keep DRAFT and report what needs changing. If approved, a separate responsible spec steward updates the **four canonical files** (status, resolved decisions, bounded N-01–N-07 wording if wanted) on a new approved branch, verifies readback and branch diff, records who approved, and links an APPROVED spec artifact in GitHub. Do **not** overwrite the independently-reviewed source branch in place.

**Step B — only after accepted APPROVED spec.** Create a separate **implementation execution claim and full paste-ready \`MULTI-002\` handoff**, with exact approved spec URL, provider/complexity, actual lane/branch/worktree and ownership/starting HEAD preflight, expected output/report path, tasks, source/Unity exclusion, and Tier 1 headless + Tier 2 browser evidence. Recommended provider **Sol/Codex High in local T3 Code**, one actual writer for P0 Core/Host/UI, no physical lease/ownership claimed by this packet. Actual local worker must register claim RUNNING with branch/host before editing. Never bypass any live CCF/Unity worktree lock, personal saves or unrelated dirty work. No Windows game build.

**Step C — separate future P1 biology spec/approval.** Inspect current source code and Irish GROWFOR/Broad–Lynch data domain before modelling annual growth. Stand-level agreement is not individual-tree field accuracy. Do not import any Irish marteloscope without source records, ID/XY/DBH/polygon consistency and clear licence.

**Current test status:** independent **DRAFT document quality PASS**; **P0 implementation NOT RUN**; **P1 ecological predictions NOT IMPLEMENTED**; **Irish real-field L4 accuracy NOT VALIDATED**. Scenario One three-species parity and complete-loop requirements unchanged. **Dashboard/Drive mirror synchronization NOT VERIFIED**.

## 5. Copy-paste review/approval dispatch (if delegating to the Overall Manager)

> **MULTI-001 — CCF Sitka 2D research lab P0 product decision.** Read the full approval handoff at https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approval-gate-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-001-SITKA-2D-LAB-P0-APPROVAL-DECISION-HANDOFF-20261114.md and independently verified [#488 quality review](https://github.com/jermur788/CCF/blob/fc7384226843aee3abb9314de0a0eb30cd64576e/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md). The corrected four-file spec is DRAFT-quality APPROVED, not yet implementation-approved. On [#493](https://github.com/jermur788/CCF/issues/493), decide OPEN-L1 (standalone research lab only — recommended YES), OPEN-L2 (P0 NO BIOLOGICAL GROWTH; P1 separately gated — recommended YES), OPEN-L3 (separate C#/.NET core + loopback local browser UI — recommended YES); explicitly defer L4 actual marteloscope import and L5 empirical growth calibration. The draft author is not its own approver. If approved, record actual user/designated Overall Manager authority, publish a formally APPROVED four-file spec artifact, optionally fold in #488 N-01–N-07 low editorial changes without another review, and verify only the authorised paths. **Then**, not before, create a separately claimed one-writer Sol/Codex High T3 Code P0 implementation handoff named MULTI-002 with actual safe branch/worktree/host preflight and independent qualification. No P1, Unity, production game save, approved Scenario One source, active-coordination or Windows build changes from this approval packet. Report exact approval status and dashboard/Drive sync honestly.
