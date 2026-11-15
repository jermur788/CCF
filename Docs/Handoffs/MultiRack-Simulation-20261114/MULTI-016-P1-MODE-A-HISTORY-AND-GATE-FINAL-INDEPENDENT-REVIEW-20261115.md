# MULTI-016 — final independent Mode A/history and approval-gate documentation review

**Date:** 15 November 2026

**Claim:** [CCF #547](https://github.com/jermur788/CCF/issues/547)

**Verdict: APPROVED AS DRAFT QUALITY. D-1 PASS; D-2 PASS. No residual blocking, major or minor defect found within this two-fix delta.**

The repaired DRAFT now separates biological/legal-work equivalence from raw history identity, handles both load audit events and approvals made after restart, preserves Mode B and leaves projection/approval policy unresolved. REQ-F09 names the correct future approval gate. This verdict qualifies the documentation for later Manager/product-owner decisions only. It does **not** approve the specification, any design choice, P1 coding, Unity scheduling/execution, game integration or an Irish forecast.

## 1. Actual reviewer, independence and preflight

| Item | Verified record |
|---|---|
| Reviewer | **OpenAI `gpt-6.1-sol`, High reasoning**, Codex harness in local T3 Code; no delegates or model switch |
| Independence and prior involvement | This is the session that authored **MULTI-014's D-1/D-2 findings**, with earlier **LAB-06/MULTI-009** research involvement. I am rechecking how another author repaired those findings, rather than supplying an additional independent discovery of them. I did **not** author the exact four-file MULTI-015 repair; that author is **GPT-6 Overall Manager**. The binding packet explicitly permits an equivalently capable reviewer independent of those repaired files, with prior involvement disclosed. This is not a fresh forestry validation or independent replication of LAB-06. |
| Host | `jer-laptop`, Linux, user `jer` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-016-SitkaP1-ModeA-DeltaReview-20261115`, newly created after preflight; Files mount `/dev/sdb4`, ext4 |
| Branch | `review/multi-016-sitka-p1-mode-a-history-final-delta-20261115-v1` |
| Starting full HEAD | `3af7b7ff30804bc9aff0d19ea394b9c45946c5a8`, equal to the pinned repair and remote review branch; clean initial worktree |
| Live ownership | Fresh-read #547 READY/NOT RUNNING and #546 HANDED-OFF/CLOSED, complete packet, repair handoff, parent review and Manager disposition. Live GitHub `main` and workflow-designated `ops/workflow-foundation-v1` coordination contained no conflicting MULTI-016/#547 entry. The proposed worktree was absent and no registered worktree used the review branch/path. Process inspection found no Unity/Claude process. Direct user/packet authority supplies this one review assignment; absence of a board row does not confer wider ownership. |
| RUNNING registration | [Actual claim registration](https://github.com/jermur788/CCF/issues/547#issuecomment-6897329265), posted **before corrected DRAFT inspection, checks or report writing** |
| Preserved work | Primary's pre-existing edits to `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were left untouched. No other worktree or live coordination edit. No Unity lease acquired. |

## 2. Inputs and scope

- **Candidate:** `3af7b7ff30804bc9aff0d19ea394b9c45946c5a8`, `docs/multi-015-p1-draft-mode-a-hash-and-gate-fix-20261115-v1`.
- **Prior DRAFT:** `8756c452cf76a8dd3169f2ab40c93349cb1edc60`.
- **Findings:** [MULTI-014 independent review](https://github.com/jermur788/CCF/blob/3714d4e01d9ab55e0e6a76a1d51ce0bc0fc3688f/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md), `3714d4e01d9ab55e0e6a76a1d51ce0bc0fc3688f`.
- **Authority:** [complete MULTI-016 packet](https://github.com/jermur788/CCF/blob/docs/multi-016-mode-a-history-final-delta-review-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-016-P1-MODE-A-HISTORY-INDEPENDENT-FINAL-DELTA-PACKET-20261115.md) and [Manager residual disposition](https://github.com/jermur788/CCF/blob/docs/manager-multi-014-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-014-P1-DRAFT-RESIDUAL-NEEDS-CHANGES-DECISION-20261115.md).
- **Repair record:** [MULTI-015 handoff](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-015-P1-DRAFT-MODE-A-HASH-AND-GATE-DOC-REPAIR-HANDOFF-20261115.md), #546.
- **Narrow source grounding:** P0 `2659db99e317d4aa93b51cf97131a61eab5fdeec`, qualified `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1`; only approval/load/hash source excerpts were re-read, with no program execution.

Inspected the complete four-file repair diff and the surrounding equivalence, approval, gate and OPEN-decision text. Direct Git diff confirms **four MODIFIED DRAFT Markdown files and one ADDED MULTI-015 handoff**, with no other candidate changes. The previously passing **R-1/R-3/R-5/R-6/R-7/R-8/R-9/R-10** topics were not reopened. Their scientific/numerical/configuration rules were outside the repair hunks; this carries forward MULTI-014's scoped PASS record, rather than claiming a new full audit. Review-readiness bookkeeping changes in tasks are consistent with the repair awaiting this recheck.

## 3. D-1 — PASS: Mode A history identity and biological equivalence are separated

Exact candidate references:

- [spec §4:77–83](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#L77) and [AC-P1-05:199–201](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#L199).
- [plan §5:118–120](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#L118).
- [tasks §4:73–77](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md#L73).
- [requirements REQ-E06:70–72](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md#L70), with REQ-E03 at 67 and C-1A at REQ-B04.

The common invariant is per-ID **biological** state, ecological year, model/RNG/seed, roll inputs, mortality/felling IDs/causes/years, legally equivalent executed work and source provenance. Spec 77 expressly conditions it on **equivalent legal actions**. Raw hash equality instead requires complete canonical-history identity, including ordered bytes/payloads, sequence, hash links and hashed session data. Each different actual chain must validate independently. Whole-snapshot hashes, biological projections and raw history hashes are explicitly distinct.

### Required thought cases

These are source-informed documentation thought tests. No P1 executable or save/load runtime test was run.

| Case | Expected interpretation of the repaired rule | Result/evidence |
|---|---|---|
| **Mode A: no open approval; no load event or later session-dependent drift; complete canonical histories identical** | Biological/work invariants still apply. Matching raw history hashes are required only in this restricted identity case. | **PASS** — spec 78/82/200, plan 118–119, tasks 75, REQ-E06 70–71 all retain the complete-history condition. |
| **Mode A: recorded load audit event** | Biological output may match while history differs. Preserve both histories and independently validate them; do not require equal raw history hashes or silently delete the event. | **PASS** — all four expressly name the load-event counterexample. Spec 79 rejects “deleting a load event” as an adequate normalization. |
| **Mode A: approval created after restart** | No open plan at the split does not prevent later epoch-dependent payload/snapshot divergence. Equal legal biological/work outcomes do not imply equal raw history or whole-snapshot hashes. | **PASS** — spec 82/200, plan 118–119, tasks 75/77 and REQ-E06 71 expressly name this second counterexample. |
| **Mode B: open APPROVED/PENDING_WORK plan, C-1A Option 1 selected later** | Exactly one `PlanStale(SessionLoaded)` per affected plan on load. Compare named lifecycle semantics; the uninterrupted next-step stale has different history. No raw-hash equality requirement. | **PASS** — spec 69/83/201, plan 120, tasks 73/76–77, REQ-E03/E06 preserve this. Missing/duplicate stales and unapproved approval persistence remain negative cases. |
| **Legal work actually differs because an approval stales** | This is a policy-dependent difference in legal action sequences, not a failure of biological determinism under equivalent actions and not permission to execute a stale plan. Report that difference. | **PASS** — read Mode B under spec 77's equivalent-legal-actions condition and plan 118–119's legally equivalent work qualifier. The wording does not authorise bypassing staleness or claim that executing in one branch but not the other is equivalent work. |
| **Future normalized history or snapshot comparison** | No projection is adopted. Any additional equality requires a separately named/versioned C-1/C-1A rule, explicit fields and epoch handling, sequence/ordering and deterministic serialization/rehashing, plus independent review. | **PASS** — spec 79, plan 118–119, tasks 75 and REQ-E06 70. C-1A remains OPEN; no event deletion, epoch suppression or reapproval policy is selected by this repair. |

The last legal-work case is a limitation on what may be compared, not an extra design decision adopted by this reviewer. Future implementation evidence must identify the actual legal actions, particularly when the two branches require different approval workflows. The repaired text no longer relies on unconditional Mode A history equality, and does not remove the owner gate needed to specify those workflows.

### Source grounding, without running P0

At P0 `2659db99e317d4aa93b51cf97131a61eab5fdeec`:

- `LabSession.cs:69–70,83` creates a fresh random session epoch on load; 221–225 records the epoch in a subsequent approval. This directly supports the later-approval counterexample even if no approval was open at the split.
- `CanonicalJson.cs:133–148,171–175` includes the epoch in canonical plan snapshots/approval payloads and hashes the snapshot bytes. An unchanged biological projection therefore does not prove equal complete-snapshot hashes.
- `LabSession.cs:75–85,144–149` records one `PlanStale(SessionLoaded)` per approved/pending plan; 247–252 stales open approvals after the uninterrupted calendar step. The reasons/ordering can differ without changing the biological comparison.
- `CanonicalJson.cs:199–208` serializes sequence, payload and previous/post-state hashes into the event record. P0's snapshot hash is not a generic digest over the entire event history. The revised DRAFT correctly distinguishes these questions rather than deriving a P1 history-hash guarantee from P0.

**Required D-1 correction: none.** The exact earlier contradiction and both counterexamples are addressed consistently. Any future policy still requires its own explicit owner decision and independent review.

## 4. D-2 — PASS: correct approval gate, no obsolete token

[REQ-F09, requirements.md:94](https://github.com/jermur788/CCF/blob/3af7b7ff30804bc9aff0d19ea394b9c45946c5a8/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md#L94) now says **“no code before P1-G2 SPEC/product approval.”** Exact literal scan of all four candidate DRAFT files returned **zero `P1-G0` occurrences**.

The preserved crosswalk remains **plan G0 = completed evidence-plan review**, **P1-S0 = G1**, **P1-G2 = G2**, **P1-01…05 = G3…G7** (spec 212–219; plan 51–69; tasks 5–26; requirements 119). Renaming this token does not mark G2 passed. Tasks 109 still says P1-G2 pending and P1-01…05 NOT RELEASED.

**Required D-2 correction: none.**

## 5. GitHub readback, GFM and protected status

Each exact candidate file was fetched from GitHub by full candidate SHA and matched the local bytes. Those same bytes were rendered through GitHub `POST /markdown`, `mode=gfm`, `context=jermur788/CCF`; returned HTML was inspected with an HTML parser.

| File | Code elements | Preformatted blocks | Rendered table rows by table | Obsolete P1-G0 |
|---|---|---|---|---|
| spec.md | 98 | 0 | 7 / 9 / 9 / 11 / 5 | 0 |
| plan.md | 25 | **1**, expected multiline architecture diagram | 5 / 10 | 0 |
| tasks.md | 27 | 0 | 5 / 8 / 9 | 0 |
| requirements.md | 34 | 0 | 9 | 0 |

All table counts match the source after excluding delimiter rows. There are zero escaped backticks or literal backticks outside code. Requirements retains **60 unchecked boxes, zero checked**. This is actual GFM HTML verification, not browser interaction, a Unity visual check or runtime parity evidence.

All seven **C-1…C-7 remain OPEN**, including C-1A and split C-2a/C-2b (spec 157–185; requirements 106–121). Spec approval remains explicit product-owner authority supported by independent review. C-1A approval/load/fork policy, C-6 scarce Unity scheduling and C-7 wording remain separate owner choices. C-2a still precedes capture; C-2b follows capture. No source, fixture, P0 approval behaviour or numerical policy has been changed here.

Durable off-repo evidence: `/media/jer/Files/CCF/Verification/MULTI-016-SitkaP1-ModeA-DeltaReview-20261115/` contains exact candidate Markdown copies, GitHub GFM HTML, and `candidate-readback-and-gfm.json` with source SHA-256 values and counts. The only tracked publication is this report.

**Directly observed:** candidate diff, four-file exact GitHub readback, D-1/D-2 text and gate scans, OPEN/unchecked status, GFM output, preflight/claim. **Source-derived:** P0 epoch/staleness/hash implications and thought-case assessments above. **Not tested:** P1 save/load or biological execution, P0 application/security/browser tests, Unity capture/adapter parity, GROWFOR/Irish field validation. No code, compilation, package installation, broad forestry audit or Windows game build was run.

## 6. Handoff and next authority

**APPROVED AS DRAFT QUALITY — D-1 and D-2 only.** Manager disposition may now consider the corrected DRAFT alongside the earlier eight passing repairs. Product-owner decisions and explicit specification approval remain required before any implementation packet or Unity scheduling. The complete Sitka/Oak/Beech Scenario One first-demo boundary is unchanged.

Only this new Markdown file is added on the review branch. Existing DRAFTs, old handoffs, source, P0, production saves, main, live coordination and frozen Reference remain untouched. The final publication SHA, GitHub byte-for-byte report readback and comparison to `3af7b7ff30804bc9aff0d19ea394b9c45946c5a8` showing **exactly ONE ADDED Markdown / ZERO other changes** are recorded in #547's publication/closure receipt. #547 is handed off and closed after those checks, with notifications to #546, parent #542 and Manager #425.

**Dashboard/Drive NOT VERIFIED.** No coordination/mirror update is authorised here. Manager follow-up must handle any later operational synchronization; closing this review claim does not certify dashboard state or adopt the specification.
