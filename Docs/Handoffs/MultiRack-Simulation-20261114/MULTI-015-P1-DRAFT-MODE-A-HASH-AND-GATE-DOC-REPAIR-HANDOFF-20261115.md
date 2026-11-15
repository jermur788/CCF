# MULTI-015 — Mode A history-hash and approval-gate DRAFT documentation repair

**Date:** 15 November 2026

**Execution claim:** [#546](https://github.com/jermur788/CCF/issues/546), actual worker OpenAI GPT-6 Overall Manager, **GitHub connector only** (no local worktree, physical machine, Unity, source code execution or tests). [RUNNING was registered before edits](https://github.com/jermur788/CCF/issues/546).

**Starting source:** `docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1`, pinned `8756c452cf76a8dd3169f2ab40c93349cb1edc60`. **Writable branch:** `docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1`, created exactly from that source without absorbing the independent review branch. Live coordination read-only with no overlapping MULTI-015 P1 spec writer; issue presence is execution, not ownership of the board.

**Manager decision:** [MULTI-014 residual NEEDS CHANGES](https://github.com/jermur788/CCF/blob/docs/manager-multi-014-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-014-P1-DRAFT-RESIDUAL-NEEDS-CHANGES-DECISION-20261115.md) based on [MULTI-014 independent Codex Sol High review](https://github.com/jermur788/CCF/blob/3714d4e01d9ab55e0e6a76a1d51ce0bc0fc3688f/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md): 0 blockers, eight earlier R fixes PASS, R-2 Mode A hash **MAJOR** and R-4 gate **MINOR** remained. Original direct owner approval only covered DRAFT authoring, not final spec or implementation.

**Status:** **DOCUMENTATION REPAIRED / DRAFT / NOT APPROVED; INDEPENDENT D-1/D-2 RECHECK REQUIRED.**

## 1. Exact permitted file changes

Four revised Markdown files, all remaining under `specs/multirack-sitka-p1-adult-rules-comparator-v1/`:
1. [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md) — §4 distinguishes raw canonical history hashes from biological projections and whole-snapshot hashes, defines Mode A equality only on identical complete histories, marks normalized projected comparisons as separate OPEN C-1/C-1A policy; AC-P1-05 aligned.
2. [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md) — §5 replay invariant and Mode A corrected; cross-check future approvals with new epoch and load event without false hash equality.
3. [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md) — §4 Mode A and negative tests corrected; future review-readiness status reflects MULTI-014 NEEDS CHANGES and this correction.
4. [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md) — REQ-E06 and Mode A hash/history conditions corrected, REQ-F09 `P1-G0` replaced with `P1-G2`.

No pre-existing handoff was edited. The only added tracked file is this report. No `Assets/**`, `Tools/**`, P0 lab source/fixtures/saves/spec, Unity, `Docs/Project/**`, live board, production/reference saves, main, GROWFOR, dashboard or Drive changed. The branch has no local authoring worktree.

## 2. Exact D-1 / R-2 — why the repaired contract is executable in principle

The future P1 annual/save test **compares biological and legally equivalent executed-work outcomes first**: per-ID biological state, ecological year, model/RNG versions and seed, mortality and executed-felling IDs/causes/years, roll inputs and effect, source/fixture provenance. No adopted policy claims a P1 persisted approval is valid after loading.

**Two different hash questions are explicitly kept apart:**
- **Raw canonical history:** identical hashes are required only for **identical complete canonical histories**, including event order/bytes, sequence, prior/post hash references and session-dependent approval payloads. If a load event occurs or an approval after the split receives a different epoch, the two actual raw histories are **independently validated**, not compared for equality.
- **Optional projected history/snapshot:** no normalization, event deletion, sequence renumbering or projected rehash is defined or approved by this DRAFT. Such a function would require a separately named/versioned C-1/C-1A policy with field retention/exclusion and session-epoch rules and independent review before P1-03.

**Mode A:** No APPROVED/PENDING_WORK plan was open at the split. Require biological/work-outcome equivalence, but **not unconditional event hash equality**. Counterexamples explicitly named: a recorded load audit event and a later legal approval whose fresh session epoch is hashed. Equality is a restricted identity case only if all hashed event/session data is identical.

**Mode B:** When an approval is open, under a future owner choice of P0-like C-1A semantics, loading appends exactly one `PlanStale(SessionLoaded)` for each open plan, and raw histories can legitimately differ from the next-step stale in an uninterrupted run. Compare biological and legal-work outcomes and named lifecycle events; reject missing/duplicated stale, false approval persistence and biological divergence. A different P1 approval policy still requires explicit independent review/owner acceptance. **The existing P0 approval/session implementation was not changed.**

**Evidence status:** These statements are reviewed source-derived *documentation controls*, not proof of an implemented event stream or run. No code, emulator, Unity, C# build, P0 lab, save/load or field validation was executed.

## 3. Exact D-2 / R-4 and preserved earlier gates

REQ-F09 now names `P1-G2` for the future spec/product approval gate. Plan `G0` continues to mean **completed evidence-plan** gate; `P1-S0=G1`, `P1-G2=G2`, `P1-01..05=G3..G7` retained. No remaining obsolete `P1-G0` token should appear in the four DRAFTs.

All eight other MULTI-014-reviewed repairs remain outside this task's change scope. Specifically, source numerical parity still requires a **future owner-authorised pinned Unity Editor (Mono) capture** before a cross-checked .NET implementation; **C-2a before capture, C-2b after capture**. No physical Unity lane was scheduled or reserved.

All C-1…C-7, including C-1A and C-2a/C-2b, remain **OPEN**. Every DRAFT file says DRAFT / NOT APPROVED; no acceptance checkbox was checked. The author/Manager cannot self-approve the P1 spec. Product-owner decisions on approval/session policy, Unity scheduling and scientific labels remain separate. The first external game demo is still the complete Sitka, Oak and Beech Scenario One.

## 4. Verification, next gate and handback

**Expected custody checks:** independent GitHub fetch/readback of all four revised DRAFT files and this report; compare working branch to `8756c452cf76a8dd3169f2ab40c93349cb1edc60`: exactly **FOUR MODIFIED Markdown + ONE ADDED Markdown**, no other changes. Check no obsolete `P1-G0` appears in the four files, Mode A/Mode B wording agrees, original approval-history hash defect resolved, and no unauthorized product decision is adopted.

**Next:** independent narrow **D-1/D-2 documentation delta recheck**, by a non-GPT-6 reviewer (Claude Opus or Codex Sol independently; the previous Codex authored MULTI-014 and would be reviewing its own findings if used again). Reviewer should not re-run complete earlier forestry research, Unity, lab tests or source code. Quality verdict **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** only. Even a passing quality review does not APPROVE the P1 specification or authorise code/Unity.

**Dashboard/Drive mirror:** NOT VERIFIED; no context regeneration, source integration or mirror publishing. **User action:** none to perform the docs recheck; separate later decision for actual P1 scope/Unity time.
