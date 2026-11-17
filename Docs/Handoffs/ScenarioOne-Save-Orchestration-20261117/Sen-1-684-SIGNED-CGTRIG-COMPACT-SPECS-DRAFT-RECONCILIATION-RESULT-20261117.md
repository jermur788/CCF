# Sen-1 #684 — Author result: Compact30 DRAFT specifications reconciled to the signed #677 CG-TRIG/save policy

**DOCUMENTATION-ONLY AUTHOR RESULT. DRAFT / NOT APPROVED. NO UNITY, SOURCE, ASSET, SAVE, PREFS, OPS OR WINDOWS WORK.**
**All eight specifications remain DRAFT. All 115 parent, 33 planting and 66 graphics checkboxes remain unchecked. Every witness named in this result is NOT RUN.**

## 1. Identity, custody and source

| Field | Observed value |
| --- | --- |
| Actual worker | **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`), variant **high**, OpenCode harness in T3 Code, Linux host `jer-laptop` |
| Registration | [#684 comment, `[RUNNING]` before the first edit](https://github.com/jermur788/CCF/issues/684#issuecomment-6956470348) |
| Parent packet | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-684-SIGNED-CGTRIG-COMPACT-SPEC-RECONCILIATION-PACKET-20261117.md` on Manager branch `docs/sen-1-684-signed-cgtrig-compact-spec-reconciliation-dispatch-20261117-v1`, commit `58bf539c` |
| Live board read at start | `main` `7ae3ad86`; the #684 row and the #677-signed row were read live; no competing author claimed these eight files or this result file |
| Overlap check | No `docs/sen-1-684…author` or `review/sen-1-684…` branch existed remotely before the claim. #676 owns world-manifest content only and is untouched here |
| Frozen source (starting HEAD) | **`1e0fba5ffd5a07704eed3e44c34b3a960f61ac60`**, branch `docs/sen-1-663-660-targeted-author-correction-20261117-v1` — the independently #665-accepted #663 tip |
| No newer authoritative draft | `main` does not contain the twelve Compact spec files, and `git log --all` shows no later commit touching the eight parent/planting paths. No re-pin or cherry-pick was needed |
| Destination | `docs/sen-1-684-cgtrig-signed-compact-draft-author-20261117-v1` at `/media/jer/Files/CCF/Worktrees/CCF-684-CompactSpecRecon-20261117` (new isolated docs-only worktree) |
| Specs commits | **`5f22e8a59507814c14b9b2e1d6afc8a61d3d5181`** (initial reconciliation) and **`0725f078a2b0f0ceac81f9f962ee1e66f3a28c72`** (bounded repair after independent review) |
| Result commit | This file's commit on the same branch (recorded in the #684 closure comment) |
| Independent review of this output | Anthropic **Claude Opus 5.5**, first review commit **`128ffa2f96104df324bc29e751d225f6d54826e8`**, verdict **NEEDS CHANGES — 0 critical / 2 major / 3 minor / 4 nit**; the bounded repair addresses every finding and the changed-lines recheck returned **APPROVED — 0/0/0/3** on review commit `b155d957` |
| Signed policy source | [#677 supplement](https://github.com/jermur788/CCF/blob/bfa01f86310bd013b87ff6686958e8b73315b591/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-677-MANAGER-COMPACT-CGTRIG-AND-SAVE-POLICY-SUPPLEMENT-20261117.md) at reviewed commit **`bfa01f86310bd013b87ff6686958e8b73315b591`**, Git blob **`bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b`** — recomputed locally with `git hash-object` and **matched**; the stale in-file "candidate / not yet reviewed" banner was **not** treated as the authority |
| Signature authority | [#677 out-of-band exact-byte D-A attestation](https://github.com/jermur788/CCF/blob/docs/sen-1-677-manager-cgtrig-exact-byte-signature-20261117-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-677-MANAGER-CGTRIG-SAVE-POLICY-EXACT-BYTE-D-A-SIGNATURE-20261117.md) — technical policy only, signer not the product owner |
| Independent review of the policy | [#681 Opus 5.5 review](https://github.com/jermur788/CCF/blob/1e923e54d373ff1f30d6810b85bb2c5d120272bb/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-681-INDEPENDENT-MANAGER-CGTRIG-SAVE-SUPPLEMENT-REVIEW-20261117.md) at `1e923e54`, **DELTA APPROVED — 0 critical / 0 major / 0 minor / 1 note (D-N1)** |
| Immutable parent | Original 15-Nov three-profile save V2 signature and `specs/scenario-independent-save-isolation-v1/spec.md` §4.2 at `200add4e6e5116268b79ef63131ff9b836f14c5d` — **unchanged, not rewritten** |
| Local user work | The `/media/jer/Files/CCF-Primary` anchor checkout had pre-existing uncommitted `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` changes. It was **not** touched, stashed, reset or cleaned |

**Single-writer discipline.** One branch, one worktree, nine permitted paths. Nothing outside the allowlist changed; `git status` in the new worktree showed only the intended paths.

## 2. Exact changed-path inventory and policy mapping

Eight modified Markdown files, all still DRAFT. Before/after blob IDs:

| Path | Before | After | Δ |
| --- | --- | --- | --- |
| `specs/scenario-one-compact-complete-loop-v1/spec.md` | `3342a125` | `04077712` | +84 / −9 |
| `specs/scenario-one-compact-complete-loop-v1/plan.md` | `e4cf9fd8` | `1d03e3de` | +14 / −1 |
| `specs/scenario-one-compact-complete-loop-v1/tasks.md` | `706b99a3` | `011f21fc` | +15 / −1 |
| `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md` | `4ff8f69a` | `bcd0d678` | +21 / −0 |
| `specs/scenario-one-compact-initial-planting-v1/spec.md` | `3658f782` | `720e5e00` | +17 / −3 |
| `specs/scenario-one-compact-initial-planting-v1/plan.md` | `6025bc82` | `862f1894` | +12 / −1 |
| `specs/scenario-one-compact-initial-planting-v1/tasks.md` | `1e757654` | `d715c679` | +14 / −1 |
| `specs/scenario-one-compact-initial-planting-v1/checklists/requirements.md` | `7c55538d` | `649ae957` | +17 / −0 |

`specs/scenario-one-compact-graphics-v1/**` — **zero** changes (`git status` on that directory is empty).

### 2.1 What the edits do

Each file gained a new **`#684`** section (DRAFT banner, signed-policy reading rule, decision table, refusal/admission cases, D-N1 disposition, evidence mapping, explicit "unchanged" list). In addition a small number of **operative** sentences were corrected in place, only where leaving the old sentence would have contradicted the signed policy:

| Location | Before | After (signed basis) |
| --- | --- | --- |
| Parent `spec.md` D84-T CT-2 | generic event-9/review wording | **T-02:** the existing Annual Review acknowledge control is the only covering acknowledgement; event9 + `reviewAcknowledgedYear=Y*` in one transaction; the existing **one event9 per year per resolution boundary** dedup; idempotent; a **covering** split state refused, and an admitted pre-final-work event9 released by the genuine covering acknowledgement |
| Parent `spec.md` D84-T CT-3 | hold across advance/skip | **T-03/T-05:** hold covers every annual/skip/service route and survives Save/Load; the two unreleasable CT-1 states are refused, not held |
| Parent `spec.md` D84-T CT-4 | "Whether admission should instead refuse it … is **not decided here**" | **T-01 Policy B:** admitted as INCOMPLETE / NO TEACHING CREDIT |
| Parent `spec.md` D84-T CT-7 | 11 negative cases; refusals citing only §4.2 | cases **12–14** added for Policy B, the two refusals and the two admitted-but-unreachable states; preamble now cites signed #677 T-05 alongside §4.2 (descriptions inside existing R-IDs, NOT RUN) |
| Parent `spec.md` D84-PJ-1 | "site-match tolerance … not chosen here" | **zero planar placement tolerance** (signed #677; D-085 OD-12 A); coordinates remain unsigned and #676-owned |
| Parent `spec.md` D84-PJ-4 | settlement bullet list | **PJ-4a** atomic effect cluster (T-06), validation before mutation, rollback, no half-commit, no double credit, **fault-injected settlement evidence** required; current source is **not** exception-atomic |
| Parent `spec.md` D84-PJ-7 | "A `Completed` order in G without its row also refuses" | **T-05:** admitted as bare incomplete, T1 permanently unreachable, no backfill, no Completed-slot replacement |
| Parent `spec.md` D84-W-1 / W-3 | UI-level freeze; plain cancel | **T-07:** freeze in UI **and** every public manager/service/bulk setter; decline mutates nothing; cancellation releases only appropriate reservations and is audited |
| Parent `spec.md` D84-O item 3 | "Signature of the CG-TRIG binding" still open | policy **SIGNED**; source/native binding, ISO-58 and the save-owner §4.2 field confirmation still open |
| Child `spec.md` C84-3 / C84-5 / C84-7 | pairing, W-A lock, open list | same signed T-02/T-06/T-07 semantics at child level; Policy B admission stated; source-level enforcement remains open |
| Child `spec.md` `#684` bullets | — | corrected after review: a passed covering year is a refusal, not loadable no-credit history; rows are per order, so a genuine 3+1 group keeps its verified rows; owner escalation recorded |
| Parent `tasks.md` D84-CF-1R | "Depends on: #663 handed off" | **Accepted by #665 for document quality only** (controller #640 accepts/rejects), with no repeat recheck |
| Parent/child `plan.md`, `tasks.md` | "CG-TRIG binding unsigned" gate rows | policy signed; **source/native** implementation and review remain; new proposed tasks inside existing IDs, including fault-injected settlement and the G1 x/z round-trip |
| Both `checklists/requirements.md` | no #684 reading | new crosswalk tables mapping the signed policy onto existing R-IDs / CIP-IDs; **no checkbox line touched** |

### 2.2 Comparison against the canonical decisions

- **D-084/D-085 are not modified.** They are `main` canon and out of this task's writable set. The specs continue to describe D-085 as **accepted design intent** only: four exact G1 sites, no rack, 30 × 30 m, 175-row roster still **unsigned**, and O1's **0.141 m** current-source failed-guard intrusion into the 1.2 m core still **unresolved**. No coordinate, manifest or world digest is asserted.
- **Signed #677, not the older #673 candidate**, is the reference. #673's Opus review covered document quality of an earlier candidate; its rulings were superseded where #677 chose differently (notably the two CT-1 deadlock states, which #673 did not select).
- **No new save keys, events, DTO fields, roots, profiles or sidecars.** The `ccfCompactTeaching` field set, the seven envelope keys, three roots, v24 payload and event enum are untouched.

## 3. Checklist identity and preservation

| Corpus | Expected | Observed | Checked |
| --- | --- | --- | --- |
| Parent requirements | 115 unique `R-001…R-115` | **115** lines, **115** unique IDs | **0** |
| Planting requirements | 33 unique `CIP-001…CIP-033` | **33** lines, **33** unique IDs | **0** |
| Graphics requirements | 66 original items, read-only | **66** lines, **0** changes | **0** |

**Byte-identity check.** All `- [ ] R-…` lines and all `- [ ] CIP-…` lines were extracted from the starting commit `1e0fba5f` and from this branch and compared with `diff`: **identical in both corpora**. No ID added, removed, renumbered, reworded, weakened or checked. The only additions are narrative crosswalk tables and `#684` sections that sit outside the checkbox lines.

No `- [x]` exists anywhere in either checklist. Nothing in the eight files claims spec approval, and every status banner still reads DRAFT — NOT APPROVED.

## 4. Signed-policy fidelity, F-01…F-04 and reviewer D-N1

### 4.1 The four signed corrections, as written into the DRAFT

| Signed item | Where it now lives | Spec language summary |
| --- | --- | --- |
| **F-01 strict deadlocked-load refusal** | Parent `spec.md` §684-2, CT-3, CT-7 case 13, D84-PJ-7; checklist table | CT-1 true + review −1 with (a) current year > Y\* or (b) covering-Y\* event9 already present **at or after the final `WorkResolved`** ⇒ **Save capture / Load refuse before any mutation**, user-visible cannot-load, no CT-3 bypass, no backdating, no fourth root/DTO field, no historical v1–v24 tightening, backup offered only if independently validated |
| **F-02 existing Annual Review binding** | Parent CT-2, §684-1 T-02, §684-6; child C84-3, `#684` | One existing control, one logical commit of event9 + `reviewAcknowledgedYear=Y*` after the final group `WorkResolved`; one event9/year dedup on the **combined** operation; idempotent repeat; no second button or event |
| **F-03 unreachable-T1 player consequence** | Parent §684-3, D84-PJ-7, CT-7 case 14; child `#684` | Missing-row `Completed` order and fully `Completed` 3+1 group load as truthful history with an explicit **no-credit diagnosis** and T1 **permanently unreachable** for that group; no row backfill, no Completed-slot replacement, no credit transfer; forest may still be managed where other guards allow |
| **F-04 exhaustive positive-claim boundary** | Parent §684-4; child C84-3 | Only (a) each `plantings` row and (b) `reviewAcknowledgedYear >= 0` are positive teaching-completion claims; `groupOrderIds` are binding references; arrival/assessment are validated **interaction signals**; a `Completed` order or `WorkResolved` is real work, not a teaching row; event9 alone is generic history — consistency refusal only when CT-1 is true for that same Y\* |

### 4.2 Reviewer D-N1, dispositioned without inventing policy

D-N1 records that the refusal case (b) qualifies an event9 at Y\* as **"at or after the final `WorkResolved`"**, while two other definitions of the covering acknowledgement omit that qualifier — a difference that matters only for an **edited-file** event9 dated Y\* *before* the final `WorkResolved`. The DRAFT now states:

1. a **false positive review claim** is always refused under original signed §4.2;
2. the hypothetical pre-final-work event9-at-Y\* case is a **separate negative-test and spec-clarity matter**, not implemented source proof and not an admitted positive claim; such a state is **admitted**, CT-3 then holds, and the genuine **covering** acknowledgement after the final row releases it (CT-7 case 7; NOT RUN);
3. the **"at or after the final `WorkResolved`"** qualifier is used **consistently**, including in CT-2's split-state definition, and "one event9 per year" always means the existing **per-resolution-boundary** dedup (the alignment #681 said a future spec could make);
4. nothing asserts that #677 silently changed those original signed bits, invents a new wire rule, or reopens the supplement;
5. if a genuine witness ever shows the difference materially changes admission or the contract, the item is **BLOCKED** and returns to the Manager for a separate signed supplement.

### 4.3 Admission test mapping (all NOT RUN)

| #677 matrix case | Spec location | Witness status |
| --- | --- | --- |
| Fresh/empty group, both false | §684-1 T-01; §684-3 | NOT RUN |
| Arrival only / assessment only / assessment without arrival | T-01; CT-4 | NOT RUN |
| 1–4 Pending, partly Approved, some genuine Completed | §684-4 | NOT RUN |
| Four genuine 3+1 Completed | §684-3 (unreachable T1) | NOT RUN |
| Completed designated order without row | §684-3, D84-PJ-7 | NOT RUN |
| Four rows, review −1, year = Y\*, no covering event9 | CT-3 hold | NOT RUN |
| Four rows, review −1, year beyond Y\* | §684-2 (refuse) | NOT RUN |
| event9 present / field −1, CT-1 false vs true | §684-4 / §684-2(2) | NOT RUN |
| Legitimate atomic review claim | T-02, CT-2 | NOT RUN |
| Non-empty real group, booleans false | T-01 Policy B | NOT RUN |
| Invalid positive claim or structural mismatch | §684-4 (refuse) | NOT RUN |
| Failed / cancelled / replacement incomplete | CT-5, D84-PJ-9 | NOT RUN |
| Later real G1 death or promotion | D84-PJ-7 | NOT RUN |
| Old historical v1–v24, dual profile, unbound O2 | §684-7, S-03/S-04 text untouched | NOT RUN |
| Interrupted settlement, Save/Load, backup, replay | §684-1 T-06, D84-PJ-4a/5, parent RV1/RV2, child CP-S6 | NOT RUN — includes the required **fault-injected settlement** (complete old or complete new state) and the exact **G1 x/z save→load round-trip** |

**Tier discipline retained.** Tier 1 targeted source/static/calibration, Tier 2 bounded feature qualification, Tier 3 one milestone integrated candidate. These DRAFT edits authorise **no** broad 115+33 suite rerun and add no native evidence.

**S-03/S-04 preserved.** The old-world protection clause (`S-03`) and the single-router clause (`S-04`) are textually unchanged; the two new refusals are Compact initial-format consistency checks and explicitly do not tighten genuine historical v1–v24 admission or introduce a competing router.

## 5. Explicitly not touched

`specs/scenario-one-compact-graphics-v1/**` and its 66 unchecked items; the signed save §4.2 file and both #677 attestation files; #681 and #673 review documents; `main`, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md`, `Docs/Project/coordination/active-tasks.md`; all #676 world JSON/packet/review paths; every `Assets/`, scene, prefab, script, package and `ProjectSettings` file; `forest-save.json` and all v1–v24 fixtures; PlayerPrefs; physical Ops and Windows paths. **No Unity process, compiler, test, save write or build was run.**

OD-16 M-A-SCOPE stays **DEFERRED**; the S1 M-A evidence format stays unsigned; three **retained** crop trees are never described as three Fell removals; the 14 H1 pre-world omissions are never credit, revenue or Fell work; the 175-row roster, G1 guard footprint and any content digest remain **unsigned**.

## 6. Blockers and honest gaps

1. **Source implementation of the signed controls does not exist.** The existing Annual Review control writes event9 without the Compact field; CT-3 has no service-level hold; the batch setters still demote Approved orders; `ScenarioOneManager` is **not** exception-atomic. All are stated as requirements and gaps, not as results.
2. **World bytes remain unsigned** (#676 separate lane). The signed policy cannot be natively exercised until the authored world, scene and manifest are owner-signed.
3. **Save-owner rulings still open:** confirmation that D84-PJ needs no field beyond the signed §4.2 row, and the S1 vs S2 M-A evidence route.
4. **Nothing here changes product scope.** No new player task, quota, threshold, species, site, price or completion condition was introduced; no owner decision was made.
5. **Dashboard local health/render and Drive/context mirrors are NOT VERIFIED**; no mirror regeneration was attempted.
6. **No material product or format conflict was found that requires a Manager BLOCKED return.** The one open wording question (D-N1) is dispositioned as above without policy change.

## 7. Independent review and bounded repair

Anthropic **Claude Opus 5.5** (different model family and provider) reviewed this output on `review/sen-1-684-compact-draft-cgtrig-independent-20261117-v1` at commit `128ffa2f`, with its own registration and push, returning **NEEDS CHANGES — 0 critical / 2 major / 3 minor / 4 nit**. It reproduced the signed blob, all 16 pre-repair blob IDs, 115/33/66 preservation, the nine-path inventory and the absence of approval/implementation claims. The repair commit `0725f078` addresses every finding:

| Finding | Disposition |
| --- | --- |
| **M-1** child text recast a passed covering year as loadable no-credit history | Corrected: that state is the signed T-05 refusal with a cannot-load consequence; the owner-escalation clause is added; "loses a row" is no longer presented as normal producer behaviour, and the producer must prevent both states |
| **M-2** invented rule barring rows for a `Completed` 3+1 group | Removed: rows are written **per order at its own successful resolution**, so a genuine 3+1 group keeps its verified rows and simply has no T1 |
| **m-1** D-N1 "used consistently" was untrue; admitted outcome unstated | Corrected: the "at or after the final `WorkResolved`" qualifier is now in CT-2's split-state definition; the dedup is defined once as **per resolution boundary**; the admitted pre-final case's expected outcome and its CT-7 case 7 witness are recorded |
| **m-2** fault-injected settlement and G1 x/z round-trip missing | Added to PJ-4a, §684-1 T-06, §684-6, parent RV1/RV2 and child CP-S6 (all NOT RUN) |
| **m-3** PJ-1 still said the site tolerance was undecided | Corrected to the signed **zero planar placement tolerance**, with coordinates still unsigned and #676-owned |
| **N-1** per-file +/− counts | Recomputed from `git diff --numstat` for the final tree and corrected in §2 |
| **N-2** undeclared in-place D84-CF-1R edit | Now declared in §2.1; wording changed to "accepted by #665 for document quality only" with #640 as the accepting controller |
| **N-3** CT-7 preamble cited only §4.2 | Now cites signed #677 T-05 alongside §4.2 |
| **N-4** overlapping R-ID ranges / missing R-092 | Ranges narrowed to R-051–R-055, R-059, R-062, R-063; R-092 added to the save-refusal row |

No finding required a Manager or owner decision, no requirement ID was added and no checkbox changed. A **changed-lines-only recheck** by a new independent review session (Opus 5.5) then returned **APPROVED — 0 critical / 0 major / 0 minor / 3 nit** for `b1150a5f..7f0b5ce7`, on review commit `b155d95702ee461819c75254c22157e5503b4e0b`, confirming all nine findings resolved, the preservation evidence intact and nothing new introduced beyond the signed text. The three non-blocking delta nits were then fixed directly by the author in a declared post-approval nit commit: the last unqualified "one event9/year" line in §684-6, the misplaced fault-injection clause in D84-SC-RV1 (now carried by D84-PJ-4a and the child CP-S6 only), and this row's full repair SHA. Those three edits are declared here rather than presented as re-approved.

## 8. Author QA verdict

**APPROVED — for the author's own DRAFT document-quality QA only, after the bounded repair above and the independent changed-lines recheck (APPROVED 0/0/0/3).** Not a spec approval, not an independent acceptance of the three declared nit fixes, not implementation authority, and not a Unity/save/native result. The eight files are internally consistent with the signed #677 supplement, preserve all 115 + 33 + 66 unchecked items byte-identically, keep graphics read-only, and leave every historical-world and deferred-item protection in place.

**Next required gate:** Overall Manager #675's own disposition of this DRAFT package, including whether the declared three-nit post-approval delta needs any further check. Only the Manager may issue any final spec approval. DRAFT documentation authorises no Unity or implementation work.
