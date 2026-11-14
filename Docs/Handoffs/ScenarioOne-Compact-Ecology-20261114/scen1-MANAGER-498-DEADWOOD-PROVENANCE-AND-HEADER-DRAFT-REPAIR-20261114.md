# scen1 — Manager #498: fix-only correction of #496 deadwood provenance and stale DRAFT text

**Issued:** 14 November 2026, UTC. **Actual author:** OpenAI GPT-6, Scenario One Manager #424, GitHub/browser-only, no local worktree or Unity/physical lease.  
**Repair claim:** [#498](https://github.com/jermur788/CCF/issues/498), closed [HANDED-OFF] only after GitHub readback and diff.  
**Independent follow-up:** [#499](https://github.com/jermur788/CCF/issues/499), READY / NOT RUNNING; different non-author session needed.  
**Difficulty of author fix:** LOW–MEDIUM, narrow source-provenance correction and stale text over 4 Markdown files; no source-code implementation. **Reviewer:** independent Claude Opus/GPT High or another registered independent capable reviewer, not the GPT-6 #498 author.  
**Branch:** `docs/scen1-497-deadwood-provenance-draft-repair-20261114-v1` (name reserved before actual #498 assignment); exact base `docs/scen1-planting-led-compact-draft-20261114-v1`, matching starting revision `a16a987c46bd93689ac60f2a5f621c4c9862d3a4`, branch created identical to base before the first change.  
**Verdict on upstream research:** [#496 independent NEEDS CHANGES, documentation quality only](https://github.com/jermur788/CCF/blob/docs/scen1-496-option-a-planting-delta-review-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-495-PLANTING-LED-OPTION-A-DRAFT-DELTA-REVIEW-20261114.md). This author repair is ready for verification, **not** self-approved.  
**DRAFT status:** NOT APPROVED / NO IMPLEMENTATION AUTHORITY. All **115** unique requirements remain unchecked.  
**User action:** send #499 reviewer dispatch at end; no new product question or Unity action.  
**Dashboard, Drive and context sync:** NOT VERIFIED.

## 1. Why this repair is necessary (N-1 source-premise correction)

The prior #495 revision and my earlier [#490 Manager research verdict](https://github.com/jermur788/CCF/blob/docs/scen1-manager-490-long-horizon-verdict-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-490-LONG-HORIZON-RESEARCH-VERDICT-AND-BROADLEAF-ESTABLISHMENT-DECISION-20261114.md) overstated #490's **zero deadwood records** as a problem in the *normal game*.

**Direct current-main source now inspected:**
- `Assets/ForestPrototype/ForestTree.cs:551–563`: `ApplyMortality(cause, year)` stores biological death/cause/year, deactivates the source tree GameObject **and then raises static `ForestTree.MortalityApplied`**.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs:510–518`: `OnEnable` subscribes and `OnDisable` unsubscribes `OnTreeBiologicalDeath`.
- `ScenarioOneManager.cs:520–555`: with growth model ≥1 (or configured windthrow exception) the event handler computes real biological volume, creates `ScenarioDeadwoodRecord` with individual tree ID/species/position/fallen year, and invokes `SpawnFallenLogVisual` (or queues storm-specific visual processing). It has a zero-volume early return. **Intended normal source behavior is a per-death deadwood record/visual, not no deadwood.**
- `ForestEcologyController.cs:429–437`: its own mortality handler only invalidates living competition/canopy caches; it does not replace the Scenario One manager's deadwood handler.

**Direct published #490 result:** [long-horizon study](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md) reports ~106/100 original Sitka deaths, zero deadwood records and no per-victim exported cause. **#496 reviewer inspected the executed off-repo harness and reports `manager.enabled=false` at harness line 77**; that calls `OnDisable` and unsubscribes the source event handler. The deadwood zeros are therefore **consistent with a harness-specific disabled subscription**, not evidence that normal-player mode loses logs. This is a **source + independent reviewer derivation**; the Manager did not directly access the off-repo executed harness or run Unity and **has not verified normal enabled-manager 3D behavior**. The #490 report's historical measured table/fixture output remains authentic for its chosen harness. The **interpretation** of deadwood in #490 §1.5 and the prior #490 Manager verdict needs this explicit erratum; neither historical record was silently rewritten.

**Correct qualification stance:** no full #490 rerun. If mortality/deadwood explanation is relevant to the future Option A compact tutorial, fold **one focused legitimate growth-model ≥1 enabled-manager Unity witness** into the already planned source-safe species/visual testing: real density death → exactly one record and log visual with correct species/tree/position/year; retained mortality cause/year; no duplicate after save/load, no phantom logs. Source code gives positive expectation; **native outcome remains NOT RUN/NOT VERIFIED**.

## 2. Exact fixes from #496

| Review finding | Repair |
| --- | --- |
| **N-1 MEDIUM, required** | Removed blanket normal-game "hidden death / no deadwood" claim in `spec.md`, `plan.md`, `tasks.md`, `R-111`; explicitly distinguished worker-measured #490 aggregate deaths / harness-disabled manager, source-intended event and log, and pending tiny native enabled-manager check. Original #490 research report/Manager verdict left untouched; erratum published here and commented on issues #490/#424. |
| **N-2 LOW, required** | Updated spec date/owners/evidence basis and complete historical #467/#484/#490/#495/#496/#498 gate; `S-21` includes narrow review/repair; C0-02 now calls OPEN-C1–C10 (Option A only establishment accepted); tasks "104-item" corrected to **115-item**; plan #496 correction chronology. All four DRAFT statuses retained. |
| **N-3 LOW, advisory** | Clarified #490 Oak **sheltered** and Beech **unsheltered**, Oak-specific crown 0.69m at Year10 → 0.33m Year20 → 0.45m Year40 versus Beech ~1.04m Year40; Manager did **not inspect PNG pixels**, not merely "not every image". |
| **N-4 LOW, advisory** | Changed "Owner-approved site framing" to **Owner-permitted site framing/provisional implementation**; hedgerow donor species/maturity viability unverified and requires evidence; no external seed source mechanic approved. |
| **N-5 LOW, advisory** | S-12A's proposed "two" management alternatives and R-107's proposed planting plurality now say **MANAGER DRAFT INFERENCE**, not owner-accepted count or design. |

**No changes** to user-approved Option A: first demo is planting-led Oak and Beech, with no required natural broadleaf cohorts by elapsed Year40; still three-species D-059 and complete-loop D-060. **No hidden ratification** of exact plot dimensions, initial broadleaf cohorts, compulsory felling or fixed end year, funding, hedgerow seed donors, same-start paired guided/independent replay (OPEN-C10), new saved profile/router, D-082 first-demo canonical patch or old-world completion changes.

## 3. Exact file allowlist and verification

Modified **only** the four existing DRAFT Markdown files, maintaining original #495 changes:
1. `specs/scenario-one-compact-complete-loop-v1/spec.md`
2. `specs/scenario-one-compact-complete-loop-v1/plan.md`
3. `specs/scenario-one-compact-complete-loop-v1/tasks.md`
4. `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md`

Added **only** this new handoff:
5. `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-498-DEADWOOD-PROVENANCE-AND-HEADER-DRAFT-REPAIR-20261114.md`

Expected GitHub compare versus #495 authored baseline: **four MODIFIED draft files + one ADDED `scen1-` handoff**, no `Assets/`, `Packages/`, `ProjectSettings/`, actual Unity scripts, other specs, canonical `Docs/Project/`, live `active-tasks.md`, source branches or historical saves. Source code and original #490 research evidence remain **read-only**.

Author pre-handoff checks: readback all five; count checklist **115 unique R-001–R-115, zero checked**; preserve all OPEN-C1–C10, D-059/D-060, old 5-rack/Legacy40/Enlarged80/model-0/frozen Reference Future v1 protected rules; exact text of original #495 Option A unchanged. The earlier #496 review was non-author and legitimately surfaced a real source-code premise defect; don't penalize or reopen it. This is documentation correction, no Unity run. **No claim of new native game tests or current property readiness.**

## 4. #499 short independent fix-only recheck — complete instructions

**Claim:** [#499](https://github.com/jermur788/CCF/issues/499) READY / NOT RUNNING.  
**Purpose:** issue **APPROVED / NEEDS CHANGES / REJECTED for document quality only** by checking #496 N-1/N-2 and advisories N-3–N-5 against these exact corrected lines and **current source**, not repeat broad evidence synthesis.  
**Difficulty:** LOW–MEDIUM. **Best suited:** genuinely distinct capable Claude Opus/GPT High non-author reviewer with GitHub read/write and source inspection; independent registered session is required, not provider dogma.  
**Reviewer branch:** `docs/scen1-499-deadwood-fix-only-review-20261114-v1` (verify unused, create from latest `main`; register real start HEAD).  
**No local worktree; no Unity/Editor/machine lease.** If reviewer does have local tools, they are read-only for source and paper; no raw evidence modifications. Check live `Docs/Project/coordination/active-tasks.md` before claiming, do not edit it.

**Only writable new Markdown:** `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-498-DEADWOOD-HEADER-FIX-ONLY-REVIEW-20261114.md`.

**Read-only review inputs:** #495 source baseline `docs/scen1-planting-led-compact-draft-20261114-v1`, #498 fixed branch `docs/scen1-497-deadwood-provenance-draft-repair-20261114-v1`, original [#496 review](https://github.com/jermur788/CCF/blob/docs/scen1-496-option-a-planting-delta-review-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-495-PLANTING-LED-OPTION-A-DRAFT-DELTA-REVIEW-20261114.md), #490 original report and current main source handlers/owner decisions. Reviewer should verify original #495 and correction-diff numbers exactly rather than trust this handoff.

**Mandatory focused checks:**
1. N-1: all four DRAFT references accurately distinguish **worker-measured source mortality** vs disabled-manager harness and expected normal manager event→deadwood source route; record/replay not native-proven; no claim game has no deadwood. Cited handler exists and is gated by correct growth model.
2. N-2: header date/author/evidence and S-21 order current; tasks C0-02, C0-06 use OPEN-C1–C10 and 115 checks; no stale blanket "#462/465 only research" gate.
3. N-3: real #490 Oak owner-sheltered/unsheltered contractor Beech, Oak-specific crown/life-stage and Manager PNG inspection limitation are accurate.
4. N-4: hedgerow seed-donor claims are unverified, not existing outside model input or Irish universal rule.
5. N-5: planting plurality and comparative two alternatives flagged **DRAFT suggestions** and OPEN-C9 remains for owner.
6. **Scope integrity:** exact four modified draft files plus this added handoff; all 115 R-001–R-115 unchecked; untouched R-001–R-104 except prior accepted R-049; OPEN-C1–C10 and future OPEN-1–OPEN-6 not prematurely closed; #471 D-082/canon, source saves/reference untouched. A manager **source** assumption is not a completed Unity verification.

**Reviewer exit:** publish its one exact named new Markdown file on disjoint reviewer branch, direct readback, diff from registered start exactly one new file; comment full handoff direct URL on #499 as [HANDED-OFF] and close. Notify #424. No redundant full 115 test suite, no #490 re-run, no Windows build, no premature production approval. Dashboard/Drive unverified.

## 5. After #499

If APPROVED as **DRAFT quality**, #424 may prepare the next owner-facing planting-cohort/site/management decision design, whereas #425 retains authority to resume paused D-082 canonical reconciliation under #471. One save-router owner and exact new world identity/slot, accepted geometry, OPEN-C1–C10 product decisions and fully approved four-file feature spec remain prerequisites for source coding/Unity. Keep #490/this note's source-vs-harness erratum visible in any later work.

If NEEDS CHANGES, route specific author-only DRAFT sentences for repair without repeating the research, full test suite or previous review chain.

## 6. Paste-ready exact independent review dispatch

> Take ownership of CCF [#499](https://github.com/jermur788/CCF/issues/499) as an independent **fix-only documentation reviewer**. Follow the complete binding `scen1` Manager packet: https://github.com/jermur788/CCF/blob/docs/scen1-497-deadwood-provenance-draft-repair-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-498-DEADWOOD-PROVENANCE-AND-HEADER-DRAFT-REPAIR-20261114.md. Register your actual non-author model/provider, clean disjoint docs branch and verified fresh starting HEAD before editing. Check only #496's N-1 source mortality/deadwood defect and N-2 stale-header/gate plus advisory N-3–N-5 against the corrected four-file DRAFT, original #495 baseline, #490 report and actual `ScenarioOneManager.OnTreeBiologicalDeath`/`ForestTree.MortalityApplied` event path. Do not infer that #490 harness zero deadwood represents normal game, and do not claim enabled-manager native behavior was proven. Verify 115 unique unchecked requirements and unchanged Option A/OPEN-C1–C10/old-world protection. Publish exactly `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-498-DEADWOOD-HEADER-FIX-ONLY-REVIEW-20261114.md` on the independent reviewer branch, with APPROVED/NEEDS CHANGES/REJECTED **DRAFT-quality** verdict, readback, one-file diff, and [HANDED-OFF] closed #499, notifying Manager #424. No production source, Unity, saved data, canonical D-082/Decision Log, live coordination or Windows build.
