# Sen-1 #771 — Independent nonauthor review of the #763 Compact30 complete-loop DRAFT

**2026-11-18. VERDICT: NEEDS CHANGES — FOR DRAFT DOCUMENT QUALITY ONLY.**
**Severity: 0 BLOCKER / 6 MAJOR / 6 MINOR / 5 NOTE.**

This verdict covers document quality only. It does not approve gameplay or the spec, and it does not approve source, Unity, world content or save-policy changes. The #763 draft stays **DRAFT / NOT APPROVED**. Manager #731 decides the disposition. No #763 author file was changed.

## 1. Reviewer, scope and frozen targets

| Field | Value |
| --- | --- |
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`). Harness: Claude Code (Agent SDK) inside T3 Code, local Linux session. The reasoning setting is not shown to me, so I do not state one. |
| Independence | I am not the #763 author (the author is OpenAI GPT-6, ChatGPT browser). I wrote none of the eight #763 files. |
| Claim | [#771 RUNNING registration](https://github.com/jermur788/CCF/issues/771#issuecomment-6989226459), made before any documentation write |
| Review branch | `review/sen-1-771-compact30-loop-nonauthor-20261118-v1`, created from fresh main `983d1050341a44ea5385d0d15213749040ec04c7` |
| Frozen target | `docs/sen-1-763-compact30-loop-specs-20261118-v1` @ **`9ab372bab3ca67c4dc2e7da94b2b5b88ed661360`** |
| Target scope readback | `git diff --stat origin/main...9ab372ba`: 8 commits, **8 ADDED Markdown files, 482 insertions**, all under `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/`. The 7 package files are under `Sen-1-763-DRAFT-PACKAGE/` and the eighth is the named result. Merge-base `7d24af79`. **No scope drift.** |
| Files reviewed in full | `spec.md` (83 lines), `plan.md` (63), `tasks.md` (34), `checklists/requirements.md` (110), `owner-decisions.md` (67); also the supporting `author-audit.md`, `independent-review-packet.md` and the #763 result |
| Inherited baseline | #617 branch `docs/sen-1-617-616-draft-approval-readiness-20261116-v1` @ `fcfd43c31b60f92277470aef9c490e92fa59cf3b`: all eight parent and child files read |
| Signed save | Original §4.2 @ `200add4e`. Signed #677 supplement at reviewed commit `bfa01f86`. I verified the blob with `git rev-parse` as **`bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b`**, which matches the signature file (signature branch head `94c9835a`). I also read the signature attestation itself. |
| Canonical | main `983d1050`: Decision Log D-059, D-060, D-066, D-074, D-077 and D-083…D-091, plus the Current Milestone. Main specs S1-C and S1-D were also read. |
| Plan | Full #731 parallel-lanes plan @ `docs/sen-1-731-chas-manager-intake-20261118-v1` |
| Source facts (read-only, main `983d1050`) | `ScenarioOneWorkOrder.cs` (`ScenarioWorkStatus`, `ScenarioWorkType`), `ScenarioManagementHistory.cs` (`WorkItemStatus`), `ForestTree.cs` (`TreeMarkType`), `ScenarioOneManager.cs` (crop-tree helpers) |

**Limitations:** I ran no Unity, compile, native tests or runtime checks, and I did not integrate any source. Board, lanes, Ops, dashboard and Drive were not touched or checked (**NOT VERIFIED**). Source was inspected statically, and only to check what the draft says about it.

## 2. Mechanical checks (all PASS)

- Inherited counts: **115** `- [ ] R-001…R-115` and **33** `- [ ] CIP-001…CIP-033`. None is checked (`grep '^- \[[xX]\]'` returns 0 in both files).
- New criteria: **70** `- [ ] **CL-001…CL-070**`. None is duplicated or checked.
- The draft does not edit any #617 file and does not mark any original as passed. The package carries DRAFT / NOT APPROVED banners on every file and is not presented as canonical `specs/**`.
- Every parent R ID falls inside at least one crosswalk range **except R-002 and R-003** (see M-1). Every CIP ID is covered.

## 3. Findings (grouped, most severe first)

### MAJOR

**M-1 — The 148 inherited requirements are cross-walked by family range only; stale, superseded and deferred items are not dispositioned.** *`checklists/requirements.md` §inherited + §Source crosswalk; `spec.md` §1.*
The draft says that later D-083…D-091 and #677 "supersede that earlier status" but "the substantive obligation remains". It never says **which** originals that affects. As a result, an implementer reading the originals meets requirements that ask for decisions already made, or for features that are now deferred. Sampled examples:
- **Owner options now decided by D-084:** R-001 (OPEN-C1). R-027, R-057 and R-093 (OPEN-C1/C7, now T1 + signed #677). R-034 and R-098 (OPEN-C8, now W-A). R-008, R-042, R-051, R-094, CIP-018 and CIP-021 (OPEN-C5, now F-A + M-A semantics, with the threshold still OD-16). R-052 (the "three cycles" Manager interpretation, which CL-057 now effectively negates).
- **Deferred by D-084 C6/C10:** R-067 and R-068 (opening overlay), the overlay half of R-097, and R-114 (replay). They must stay as **deferred, unchecked** items, not lapse silently.
- **Resolved by signed save policy:** R-015 (OPEN-C4 foreign-ID preflight). R-083 (one save owner). R-089 and R-091 (disjoint keys / root choice, now the signed three-root `ccfCompactTeaching`).
- **Process items already overtaken:** R-004, R-100, R-115, CIP-023 ("paused D-083"), CIP-024 and CIP-025.
- **Geometry now fixed by owner choice:** R-006 / CIP-024 ("175 only if approved", now OD-15 adopted). CIP-007 (candidate G1, now OD-09/OD-12). The O1 0.141 m guard FAIL in the overlays, now OD-11 "must remove".
- **Not mapped at all:** R-002 (independent review) and R-003 (#424 source and lease recheck) are not in any crosswalk range.

**Correction:** add one per-ID disposition table for all 148 IDs, as a new section or an `inherited-disposition.md`, without editing the originals. Each row gets one class: *retained unchanged* / *retained, design provenance updated by D-0xx or #677* / *deferred by D-084 C6/C10 (stays unchecked)* / *process item for Manager to close*. The table names the controlling row and the CL IDs that add to it. Add R-002 and R-003.

**M-2 — The signed precondition "no G1 designation before both acknowledgements" is missing.** *`spec.md` §4 C3; `checklists/requirements.md` CL-019…CL-022; `tasks.md` F1 checks; `plan.md` P3.*
Signed #677 §2 ("Designated planting group: *With both prior acknowledgements true*…") and §4 ("Arrival only → *No group designation before both acknowledgements*") make arrival and assessment a forward gate on designating the four Pending orders. The draft orders C1→C2→C3 in prose but never states the gate, and no CL criterion or F1 negative tests it. Policy B admission of older or edited files does not relax this rule for new forward play.
**Correction:** state the gate in C3. Add a positive and a negative criterion (designation refused while either flag is false; no order, reservation or group ID created), and add the negative to the F1 checks.

**M-3 — CL-036 says "false/missing" acknowledgement flags are admitted, which contradicts signed refusal of missing mandatory keys.** *`checklists/requirements.md` CL-036 (also echoed loosely in `spec.md` §8).*
Signed #677 §4 (last refusal row) and §2 admission semantics **REFUSE BEFORE MUTATION** any "duplicate/foreign/**missing mandatory key**, unknown signed-type key…". Policy B admits *present but false* `arrivalAcknowledged` / `assessmentAcknowledged`. Read literally, CL-036 would make a tester expect admission of a payload with a missing key.
**Correction:** reword to "present and `false`". Add the paired negative: a missing or ill-typed flag key REFUSES before mutation and leaves the live, Prefs, primary, tmp and bak states unchanged.

**M-4 — The "three player-retained Crop Trees" requirement has no defined status, gate, timing or persistence, so it cannot be implemented or tested.** *`spec.md` §3, §4 C2, §7; `checklists/requirements.md` CL-013/CL-014; `owner-decisions.md` §A OD-17 row.*
D-085 OD-17 (select, retain and favour three crop trees) is correctly kept out of the T1 tuple. But the draft calls it an "authored scenario-management requirement" without saying:
- whether it gates C3, T1, D-077 skip, M-A completion, or only E2 evaluation;
- what "retain" means if a selected tree later dies, is re-marked or is felled. The source mark is a mutable `TreeMarkType` (None/Fell/CropTree), and current source uses `CropTree` for `BatchPruneCropTrees` / `requiresCropTree`;
- whether the existing per-tree mark persisted in the v24 world payload is enough evidence.

CL-014 defers persistence to a "separately approved rule", but no `owner-decisions.md` §C row tracks that rule, so nobody owns the decision.
**Correction:** either (a) state that it is guidance/teaching evidence observed through existing persisted marks only, with no gate and no new key, or (b) add a §C row "Crop-tree requirement semantics" for Manager wording, escalating to the owner only if a gate changes the player experience. Do not invent a key or quota.

**M-5 — S1-D "repeated interventions" is carried into Compact without a Compact entry predicate, and its approval status is overstated.** *`spec.md` §1, §4 C7; `plan.md` P2/P4; CL-047.*
On main, the S1-D spec status reads **"APPROVED PRODUCT INTENT; D-066 implementation contract … pending Manager review #158"**. Its teaching sequence leaves `response` only when, "in a rack world, `ReleaseReviewed && RackReviewComplete`" holds, and D-066 persists a **rack** teaching-completion latch. D-074 also supersedes S1-D's spacing rule. The draft calls S1-D "approved", says "the approved S1-D concept continues", and forbids the rack and v24 latch in Compact. It never says what replaces the rack-review entry gate (for example, T1 complete), so F2 has no defined trigger for S1-D re-entry in Compact.
**Correction:** quote S1-D's real status. Add one spec clause and one CL defining the Compact S1-D entry and advisory trigger (D-074-consistent, with no rack latch). Mark it as Manager implementation wording unless it changes product intent.

**M-6 — How this package relates to the existing #617 parent, child and graphics DRAFTs on promotion is undefined.** *`plan.md` P0.2 and P0 exit; `tasks.md` dependency edges; `owner-decisions.md` §C "Canonical full-loop spec approval".*
The plan says Manager "may promote the reviewed four-file package into the canonical specs tree". It does not name a target path, and it does not say whether the package **replaces**, **amends** or **sits alongside** `specs/scenario-one-compact-complete-loop-v1` (R-001…R-115), `scenario-one-compact-initial-planting-v1` (CIP-001…033) and the graphics DRAFT. If this is left open, two live DRAFT contracts will cover the same loop, each with its own ID scheme.
**Correction:** add a §C gate "Spec consolidation path" with the concrete options. For example: A, merge the CL overlay into the parent spec directory while carrying R/CIP IDs; B, open a new spec directory and mark parent/child "superseded-by" with an ID carry table. Manager chooses; the author does not pre-select.

### MINOR

**m-1 — Two explicit #677 carry-forwards are dropped.** *`spec.md` §4 C6, §8; CL-033/CL-039.*
(a) The signature §2 assigns reviewer **NOTE D-N1** to "the next bounded four-file Compact spec fidelity review". D-N1 is the inconsistent "at/after final WorkResolved" qualifier on the pre-final-work event9 split state, and the draft never mentions it. (b) Signed T-05/§4 say that if legitimate play can reach either deadlock state, implementers **STOP and escalate to the owner**. This rule is absent.
**Correction:** add both items as an open item and a stop condition. Do not resolve D-N1 by expanding policy.

**m-2 — "Cancelled" is used as an order status.** *`spec.md` C4 "show … Failed and Cancelled distinctly"; CL-028.*
Source `ScenarioWorkStatus` is Pending/Approved/Completed/Failed. Cancellation deletes the order and records an `OrderCancelled` event, which history shows as `WorkItemStatus.Cancelled`. Signed #677 §3 says "**No new `Cancelled` status**".
**Correction:** say that cancellation is shown and persisted as an audited `OrderCancelled` event and history item, not as a work-order status.

**m-3 — The G1 coordinates are called "signed".** *`spec.md` C3 "four exact signed coordinates"; CL-019 "exact signed x/z".*
D-085 OD-12 "does **not** sign the candidate G1 coordinates", and the draft's own `owner-decisions.md` OD-09/12 row says the coordinates are not signed.
**Correction:** write "the exact stored x/z from the future owner-signed world manifest (currently unsigned)". Also state that each site has a fixed species (D-084 Geometry A; #677 "matching species").

**m-4 — Citations use mutable branch names where pins exist.** *`spec.md` §1; `owner-decisions.md` §B/§E; `checklists/requirements.md` header.*
#677 is linked by branch, while the signature binds commit `bfa01f86` / blob `bb9327fe`. The 115/33 baseline is linked by branch rather than `fcfd43c3`. The adopted roster digest `dac06a3b…` (OD-15) is not cited.
**Correction:** add the pins.

**m-5 — The designated-slot replacement rules are incomplete.** *CL-026/CL-037; `spec.md` C4/C5.*
Signed #677 §3 says a Failed or cancelled member may be replaced by a **new** Pending order in that slot, the group **never exceeds 4 active IDs**, and Completed slots are not replaceable. The draft covers cancellation and 3+1 but not Failed-slot replacement or the 4-active-ID cap.
**Correction:** add one criterion with negatives.

**m-6 — Small fidelity omissions.**
- `owner-decisions.md` §A has no row for **D-084 Geometry A** (the four original candidate positions; a distinguishable failed shelter ≤ r 0.10 m).
- `spec.md` §6 / CL-051 omit D-077's "one tick per frame, cancellable only between ticks".

**Correction:** add both lines.

### NOTE

- **n-1:** Policy-B saves where T1 can never be reached (3+1, or a Completed slot with no teaching row) also permanently lose the D-077 skip, because §6 gates skip on T1. Ordinary one-year advance is unaffected. State this player consequence beside the existing "no T1" diagnostic.
- **n-2:** Under finite €12,000 F-A, the legacy €2,500 contractor-harvest minimum (if it applies in Compact) would allow only a few contractor interventions. The owner-time alternative matters here. F2 needs a native affordability witness for the M-A loop. This is a feasibility risk to record, not a reason to change the price.
- **n-3:** Source has no "retention" work type (`ScenarioWorkType`: FellTree, PlantJuvenile, RemoveRegeneration, PruneTree, SalvageDeadwood). M-A's "retention/release … Approved→Completed" therefore needs OD-16 to say how a retention decision is evidenced. Add this to the OD-16 framing in `owner-decisions.md` §D. The draft should not answer it.
- **n-4:** The text is very compressed (for example `spec.md` §8 and CL-031/CL-034). That is acceptable for a DRAFT, but the canonical version should split run-on clauses into one obligation per sentence.
- **n-5 (strengths):** OD-16 is honestly left unresolved, with no numeric threshold invented. The world gates are stated correctly (signed 0 / candidate 80 / blocked 8, `world_digest=null`, #676 static ≠ signature). The E1/E2 split matches #731. Old-world isolation is thorough. Evidence classes are labelled. The €12k figure is treated as calibration and the €11/€9/€20/€30 figures as source examples, with no profit or survival guarantee. No owner choice, signature or physical reservation is fabricated.

## 4. Checklist against the review packet

| Packet item | Result |
| --- | --- |
| 1. #731 plan and canonical D-059/060/074/077/083–091 and milestone | Read. Owner intent is correctly ranked above the #617 OPEN-C entries in principle, but this is not dispositioned per ID (**M-1**). S1-D status overstated (**M-5**). |
| 2. 115 R + 33 CIP vs 70 CL; no lost duty, silent check or invented APPROVED; staged ≠ canonical | Counts and unchecked state PASS. CL criteria are additive and nothing is silently checked. Per-ID disposition is missing and R-002/R-003 are unmapped (**M-1**). Canonical relationship undefined (**M-6**). |
| 3. §4.2, #681 and signed #677: Policy B, CT-1 2+2 rows, Y*, CT-3 all-route hold, event9+field atomicity, 3+1, deadlock refusals, W-A all routes, unsigned M-A S1 | Mostly faithful: Y* = resolution year; single existing event9 control; CT-3 across reload; both deadlock refusals; 3+1 and missing-row admitted without credit; W-A UI/service/bulk; S1 unsigned. Defects: designation gate missing (**M-2**), "missing" flags (**M-3**), D-N1 and escalation rule (**m-1**), Cancelled status (**m-2**), replacement cap (**m-5**). |
| 4. Owner decisions not reopened; OD-16 unchosen, threshold and persistence not invented | PASS for 30 m / no rack, 2+2, €12k, three retained trees ≠ three Fell, T1, W-A, M-A semantics, C6/C10, D-091. OD-16 and S1 are correctly UNSIGNED. Crop-tree semantics underdefined (**M-4**). Geometry A row missing (**m-6**). |
| 5. Logical C0→C8 walkthrough including cancel, replace, fail, load and skip | The flow is coherent end to end. Gaps: designation gate (M-2), crop-tree timing (M-4), S1-D trigger in C7 (M-5), Failed-slot replacement (m-5), permanent skip loss (n-1), affordability (n-2). |
| 6. World truth (#676 static ≠ signed, D086–D090 limits, D091, lane ownership #764/#765/#690/#718/#652) | PASS, except that "signed coordinates" is a mislabel (**m-3**). |
| 7. Old-world isolation (Legacy40, E80 Rack V2/MFM, v1–v24, Reference Future v1, model0, no cross-slot latch or Prefs) | PASS (CL-010, CL-062…065; spec §8). |
| 8. F1/F2 ownership; no production release, lease or native-test approval; E1 vs E2; Windows only after the owner's Editor test and separate permission | PASS (plan P2/P3/P4/P5, tasks contract). |
| 9. Evidence classification | PASS (spec §9 labels; €12k is the only owner-fixed figure). |

### Sampled crosswalk (R/CIP → CL → controlling authority)

| Inherited | CL | Controlling | Review |
| --- | --- | --- | --- |
| R-029, CIP-031 (H1 omissions pre-world) | CL-003, CL-029 | D-083; D-085 OD-05 | Consistent |
| R-040, CIP-014…017 (4 owned-stock orders; €11/€9/€20/€30) | CL-019, CL-022, CL-023, CL-044 | D-084 T1/F-A; #677 T-06 | Consistent. Coordinates mislabelled as signed (m-3); acknowledgement gate missing (M-2) |
| CIP-016 (per-tree actor/shelter) | CL-021 | D-084 W-A; #677 T-07 | Consistent |
| R-034, R-098 (OPEN-C8) | CL-025, CL-026 | D-084 W-A; #677 T-07 | Substance consistent; original's open-option wording not dispositioned (M-1); "Cancelled" status (m-2) |
| R-056, R-057, R-093 (review acknowledgement / OPEN-C7) | CL-033…CL-035, CL-051 | D-084 T1; #677 T-02/T-03 | Consistent; D-N1 not carried (m-1) |
| R-013, R-014, R-018, R-086 (pre-mutation refusal) | CL-038, CL-063 | §4.2; #677 T-04/T-05 | Consistent; CL-036 "missing" conflicts (M-3) |
| R-051, R-094, R-095, CIP-021 (substantive classifier) | CL-053…CL-057 | D-084 M-A; D-085 OD-16 deferred | Consistent; no threshold invented |
| R-062, R-063 (completion persisted) | CL-058, CL-059 | #677 "M-A S1 not signed" | Consistent (blocked until signed) |
| R-067, R-068, R-114 (C6/C10) | none | D-084 deferral | **Not dispositioned as deferred** (M-1) |
| R-002, R-003 | none (CL-068 is related) | process | **Unmapped** (M-1) |

## 5. One grouped bounded correction (for #731 to dispatch)

One pass by the #763 author or an assigned documentation worker, on the draft files only:

1. M-1: add the 148-row disposition table.
2. M-2: add the designation gate, its CL criterion and the F1 negative.
3. M-3: reword CL-036 and add the missing-key negative.
4. M-4: define crop-tree semantics, or add a §C decision row.
5. M-5: state S1-D's true status and add the Compact entry trigger, with a CL.
6. M-6: add a §C spec-consolidation gate with options.
7. m-1…m-6: the line-level fixes above.
8. n-1 and n-3: one sentence each.

After that, an independent **changed-lines-only** recheck by a nonauthor. **Owner input** is needed only if Manager judges M-4 (a crop-tree gate) or M-5 (the S1-D trigger) to change the player experience materially. OD-16 remains the single principal owner question and should be framed together with n-3.

## 6. Sign-off block

| Field | Value |
| --- | --- |
| Verdict | **NEEDS CHANGES — DRAFT DOCUMENT QUALITY ONLY** |
| Counts | BLOCKER 0 / MAJOR 6 / MINOR 6 / NOTE 5 |
| Target | `9ab372bab3ca67c4dc2e7da94b2b5b88ed661360` (8 added Markdown files, unchanged by me) |
| Reviewer | Claude Opus 5.5, nonauthor, branch `review/sen-1-771-compact30-loop-nonauthor-20261118-v1` from `983d1050` |
| Not asserted | Gameplay or spec approval, implementation, Unity/native PASS, world signature, OD-16 choice, S1 schema, canonical promotion, dashboard/Drive sync |
| Next | Manager #731 verdict → one grouped correction → independent changed-lines recheck → Manager and owner gates |
