# Overall Manager — MULTI-014 residual documentation disposition

**Date:** 15 November 2026

**Verdict: NEEDS CHANGES — 0 blocking, 1 MAJOR (D-1 / R-2), 1 MINOR (D-2 / R-4); eight other R-1…R-10 repairs PASS in independent delta review.**

**Reviewed source:** [MULTI-014 independent Codex Sol High delta review](https://github.com/jermur788/CCF/blob/3714d4e01d9ab55e0e6a76a1d51ce0bc0fc3688f/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md), [#542](https://github.com/jermur788/CCF/issues/542) HANDED-OFF/CLOSED. Original repaired four-file DRAFT: `docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1` at `8756c452cf76a8dd3169f2ab40c93349cb1edc60`. Manager directly fetched complete reviewer report, confirmed report diff is **one ADDED Markdown** versus the repair candidate, inspected all four candidate files and confirmed remaining text. The review author was Codex Sol High (earlier LAB-06 author); independent of GPT-6 original draft and Claude Opus critic/repair worker. The reviewer explicitly did not replicate its own LAB-06 research.

## Accepted residual defects and exact repair authority

### D-1 / R-2 — MAJOR: save/reload biological equivalence does not imply raw event-hash equality

At candidate `spec.md` §4 Mode A and AC-P1-05; `plan.md` replay §5; `tasks.md` §4 Mode A; `checklists/requirements.md` REQ-E06, a named load event may be recorded but the test still expects identical canonical hashes/chains. The initial suggestion to "remove the load event" is **not** a defined projection: canonical ordering, sequence/previous hashes, session-bound data and rehashing would need explicit rules.

The source review also found that even **no open approval at the save split** does not guarantee identical future raw histories: a later approval may contain a new random session epoch after the reload. P0 snapshot hashes and event-chain histories are not interchangeable.

**Binding documentation correction, without adopting C-1A:** In all four DRAFTs, Mode A requires per-ID biological state, ecological-year, model/RNG/seed, roll inputs, mortality/felling ledger and legally equivalent executed-work outcomes to match. Require **raw canonical-history hash equality only where complete canonical histories match in event order, payload, sequence and all hashed session data**. If a load is recorded or later approval changes a session identifier, independently validate each raw audit/history chain and do not claim their raw hashes equal. A projected/normalized comparison is **a separate future named/versioned C-1/C-1A policy**, with explicit retained/omitted fields, sequence renumbering, deterministic reserialization/rehashing and independent verification before P1-03; don't invent that policy here. A restricted no-load-event/no-session-divergence identity case may remain a positive test, as such.

**Preserve Mode B:** if C-1A owner later selects P0 session-epoch semantics, exactly one documented `PlanStale(SessionLoaded)` per open APPROVED/PENDING_WORK plan, distinct event histories expected, same biological/work accounting for comparable legal actions. Reject missing/duplicate stales, unapproved survival of approvals and biological divergence. P0 remains unchanged. **C-1A remains OPEN**.

### D-2 / R-4 — MINOR: approval gate typo

`checklists/requirements.md` REQ-F09 still says `P1-G0` though the actual future spec/product gate is **`P1-G2`**. Replace this one token only, verify no stale `P1-G0` reference remains throughout the four DRAFT files. Plan `G0` means the **completed evidence-plan gate**, not product approval.

## Execution/disposition

**Docs-only Manager-owned repair is proportionate and authorised within the original owner's approval to prepare/revise a DRAFT**. Do not dispatch another model merely for five editorial replacements. Use a fresh issue claim before changing documents, new branch pinned to the repaired DRAFT, a single GitHub-only doc writer and no local Unity/source machine. **Allowed tracked scope: four modified P1 DRAFT Markdown files + one new Manager repair handoff**; no P0 lab, game/Unity, old handoffs, research, live board, decision log, context or main edits. Retain DRAFT / NOT APPROVED and C-1…C-7 OPEN.

After GitHub readback and exact diff, obtain **one new independent focused recheck of D-1 and D-2**, preferably Claude Opus or a different reviewer not the GPT-6 draft author/repair writer; independent reviewer shall only approve **DRAFT QUALITY**, not the spec or code. Do not rerun earlier eight passed topics, broader forestry research, Unity or the Windows game build. A passing review still requires separate owner C-1A/C-6/C-7 decisions and explicit product/spec approval before any production/source implementation or Unity capture.

**What passed:** review publication scope, source derived facts, independent R-1/R-3/R-5/R-6/R-7/R-8/R-9/R-10 checks. **Failed:** R-2 wording/history invariant and R-4 old gate reference. **Not tested:** P1 runtime/Unity parity, annual saved replay, Irish field accuracy, source/browser changes. **Dashboard/Drive mirror sync: NOT VERIFIED.** No user action required for this docs-only repair/recheck dispatch.
