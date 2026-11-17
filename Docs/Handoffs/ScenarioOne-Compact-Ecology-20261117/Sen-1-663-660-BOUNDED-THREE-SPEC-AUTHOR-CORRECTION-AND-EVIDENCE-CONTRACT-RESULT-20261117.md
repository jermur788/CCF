# Sen-1 #663 — Author correction of the #657 Compact DRAFT specs after #660 NEEDS CHANGES: RESULT

**Status: AUTHOR CORRECTION COMPLETE — FOR ONE INDEPENDENT CHANGED-LINES RECHECK.** All 12 spec files remain **DRAFT / NOT APPROVED**. The 175-row roster and the manifest remain **UNSIGNED**. **B-2 is corrected as a decision-ready proposal, not resolved.** M-A-SCOPE and the M-A save-evidence route are **owner and save-owner gates (BLOCKED-OWNER)**.

## Identity and scope

| Item | Value |
| --- | --- |
| Claim | [#663](https://github.com/jermur788/CCF/issues/663); [RUNNING] registered before any write ([comment](https://github.com/jermur788/CCF/issues/663#issuecomment-6933357023)) |
| Author | Claude Code, `claude-opus-5-5` (Claude Opus 5.5), Anthropic. The original #657 author; **not** the #660 reviewer |
| Mode | GitHub only. Commits went through the GitHub Git Data API (blobs → tree → commit → non-force ref update). No local CCF worktree, no Unity, no physical Ops lease, no Windows. Source evidence came from read-only reads of the local bare store at `main` |
| Packet | [#640 binding packet](https://github.com/jermur788/CCF/blob/docs/sen-1-663-manager-bounded-review-response-packet-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-663-660-NEEDS-CHANGES-AUTHOR-CORRECTION-AND-CONTRACT-GATE-PACKET-20261117.md) |
| Review answered | [#660 NEEDS CHANGES](https://github.com/jermur788/CCF/blob/review/sen-1-660-657-compact-three-spec-roster-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-660-INDEPENDENT-657-THREE-SPEC-AND-AUTHORED-ROSTER-REVIEW-RESULT-20261116.md) |
| Branch | `docs/sen-1-663-660-targeted-author-correction-20261117-v1` |
| Starting HEAD | `55f790677e452805d9dedf08d243c5ba81e2aab2` (the frozen #657 candidate #660 reviewed) |
| Spec correction commit | `fa6776de59019c85e4cb375b5a178d5aaf541fd7` (8 Markdown files) |
| Result commit | the commit that adds this file, child of `fa6776de` |
| Live board | `main` `f6d8a223` re-checked before writing: #640's reservation row is current; no newer owner |
| Source pin | `main` `f6d8a223438142b7d3ffdee015857942a5cadcdb`; `git diff 500ef7ac..f6d8a223 -- Assets` is empty, so #660's `500ef7ac` source pin still applies |

**Changed files, base `55f79067` → `fa6776de`. The GitHub compare should show exactly these, plus this result:**

| File | Lines +/− | Why |
| --- | --- | --- |
| `specs/scenario-one-compact-complete-loop-v1/spec.md` | +139/−16 | B-1, M-1, M-2, B-2 (main contract) |
| `specs/scenario-one-compact-complete-loop-v1/plan.md` | +7/−5 | crosswalk and implementation consequences |
| `specs/scenario-one-compact-complete-loop-v1/tasks.md` | +6/−5 | recheck task, T1/M-A task acceptance |
| `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md` | +6/−4, prose rows only | D-084 reading table |
| `specs/scenario-one-compact-initial-planting-v1/{spec,plan,tasks}.md` | +3/−3, +1/−1, +2/−2 | child T1 / M-1 / M-2 alignment |
| `specs/scenario-one-compact-initial-planting-v1/checklists/requirements.md` | +2/−2, prose rows only | D-084 reading table |
| `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-663-…-RESULT-20261117.md` | new | this result |

**Not changed:**
- the graphics family (all four files): no directly dependent T1, broadleaf or M-A text was found;
- the #657 manifest, roster annex and result;
- the #660 report;
- code, Assets, saves/Prefs, coordination files, canonical D-083/D-084, `main`.

## Preservation evidence (author-run; not independent)

- **Checklist lines.** A script compared every line matching `^\s*- \[[ xX]\]` before and after each edit, and refused to write on any difference. It also asserted an exact match count for every replacement.

  | Family | Checklist lines | Changed | Checked |
  | --- | --- | --- | --- |
  | Parent | 115 | 0 | 0 |
  | Child | 33 | 0 | 0 |
  | Graphics | 66 (untouched file) | 0 | 0 |

  Graphics FR/AC identifiers are untouched because the files are untouched.
- **IDs.** No requirement ID was added, renamed or removed. New labels (T1-COMPLETE, CT-7, D84-PJ / PJ-1…PJ-9, D84-MAE, S1/S2, D84-CF-1R) are DRAFT rule/proposal labels. They are not R-/CIP-/FR-/AC- IDs and are explicitly marked proposed, not accepted.
- **Status.** All three family headers still read DRAFT / NOT APPROVED.
- **Readback.** The GitHub readback of `fa6776de` is byte-equal (`cmp`) to the author's edited copies for all 8 files.
- **Prior wording.** The #657 text replaced in place is preserved at the frozen commit, which the new parent note links.

## Corrections by finding

### B-1 — Teaching completion now requires arrival AND assessment (parent D84-T)

- **New T1-COMPLETE.** One derived Compact-only predicate: (a) `arrivalAcknowledged` AND (b) `assessmentAcknowledged` AND (c) CT-1 AND (d) CT-2.
  - CT-1: four valid, linked, distinct, current-profile 2 sessile Oak + 2 Beech `PlantJuvenile` orders actually `Completed`, each paired one-to-one with a distinct juvenile identity and completion year through a D84-PJ row.
  - CT-2: the Latest Annual Review for Y\* acknowledged through genuine event 9, where Y\* is the maximum real completion year of the final four.
  - The predicate is derived from §4.2 plus the retained v24 payload. There is no new flag, and old-profile teaching state or namespaces never satisfy it.
- **CT-3.** Rewritten as an **advance/skip hold only**, not a conjunct. Reading the review lifts the hold. That completes T1 only if arrival and assessment are also true. There is no "T1 passed" text otherwise, including after an ordinary advance while CT-1 is false.
- **CT-4.** Adds a **proposed ordering control**: assessment is offered only after arrival, and group binding is refused until both are true. This is needed because §4.2 records no acknowledgement timestamps. A loaded state with a non-empty group and a false acknowledgement is recommended as "incomplete, no credit"; refusing it at admission instead is left to CG-TRIG/save-owner ruling.
- **Three distinct facts.** `reviewAcknowledgedYear` (persisted, −1 → Y\* once), the latest-review UI year, and event 9 `eventId` chronology. No §4.2 key is added. Event 9 and `reviewAcknowledgedYear` are written in one mutation.
- **CT-5.** A Failed or cancelled reference stays in G until an explicit replacement designation swaps that exact reference.
- **CT-7.** Eleven proposed negative witnesses covering every case the packet lists: either acknowledgement false; fabricated, foreign or historical IDs; duplicates; failed, cancelled or replaced orders; mismatched juvenile or species; wrong multiset; wrong-year or early event 9; earlier-year browse, harness, load or skip; save/reload at each partial state; unread-review advance; no misleading completion text.
- **MA-1.** Now reads "T1-COMPLETE holds", with CT-3 excluded.
- **Crosswalks.** Updated in S-06, S-14, the §8 OPEN-C7 row, the D84-S crosswalk, the parent plan (lines 18, 75, 129, 148 and new consequence 8), parent tasks (D84-SC-T1, B-02, D-04), the parent checklist rows for R-001/027/057/093, R-028/040/102/105/107 and R-056/058/060/061, child C84-3, the child plan CP0 paragraph, child tasks CP-S3 and the CP-S3 table row, and the child checklist row for CIP-014/015/019.

### M-1 — Order → juvenile pairing (new parent D84-PJ)

**Source facts.** All links are to `main` `f6d8a223`, which has the same Assets as `500ef7ac`. Static reads only: nothing was compiled or run.

| Fact | Source |
| --- | --- |
| `ResolvePlanting` fails only through `Fail`, before any stock deduction or juvenile creation. `Fail` changes status, message and year only | [ScenarioOneManager.cs:1861-1925](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1861-L1925), [:2138-2144](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L2138-L2144) |
| On success, a juvenile with `"PJ"+nextJuvenileId++` is created, then cash is settled, then the order becomes `Completed`. No `Fail` comes after creation | same range |
| `WorkResolved` is recorded after resolution, with `workOrderId` but no `juvenileId` | [:1313](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1313), [:2452-2480](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L2452-L2480) |
| `PlantedJuvenileSaveData` has no `workOrderId`; `ScenarioOneWorkOrder` has no `juvenileId` | [ForestSaveData.cs:215](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ForestSaveData.cs#L215), [ScenarioOneWorkOrder.cs:29-72](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs#L29-L72) |
| `nextJuvenileId` is re-derived on load as max PJ + 1 | [:1529-1531](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1529-L1531) |
| Orders are removed only by the Pending-remove and Approved-cancel cores, and each records `OrderCancelled` | [:1185-1219](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1185-L1219) |

**Proposed contract.**
- **PJ-1 Designation.** Explicit T1-C binding of each order into the group, with per-tree actor and shelter choices (CIP-016).
- **PJ-2 Approval.** Approval freezes the order's parameters (W-A).
- **PJ-3 Settlement.** Only ordinary annual settlement validates stock, cash and site.
- **PJ-4 Success point.** It is the only place a §4.2 row `{workOrderId, speciesId, juvenileId, completionYear}` is appended. The juvenile ID is captured from the creation call, within the same `AdvanceYear` mutation, before `WorkResolved` and before any save.
- **PJ-5 Idempotency.** Duplicates are refused. Load never creates or repairs rows. Deterministic replay is reasoned statically and still needs a native witness.
- **PJ-6 No inference.** Pairing is never inferred from coordinates, Transform order, list index, timestamp, counter arithmetic or event adjacency. Equal positions may refuse but never link.
- **PJ-7 Load reconciliation.** Each row is checked against its order, its juvenile and its `WorkResolved` event. Later death or promotion does not invalidate a row, per §4.2.
- **PJ-8 Multi-year Y\*.** Rows may carry different completion years; Y\* is their maximum.
- **PJ-9 Negatives.** Failure, cancel/re-designate, reload and duplicate-site cases.

**Save boundary.** The signed §4.2 row already carries exactly the linking fields, so **no new field is proposed**. The runtime storage mapping is **not in code**. Any field beyond the signed row, such as `juvenileId` on events or `workOrderId` on `PlantedJuvenileSaveData`, would alter the historical v24 DTO. That **requires a save-owner-approved, signed revision**. W-A, CIP-016, no automatic execution and no synthetic Year event are retained.

### M-2 — Historical "both broadleaves" acceptance subordinated to D-084 M-A

- **Parent §9.** The inherited sentence is kept verbatim and marked as historical wording subordinated to D84-M. A new note follows it.
  - **Required:** both planted Oak and Beech stay honestly inspectable, with real protection, survival/mortality and history. D84-M's later substantive assessed management is required.
  - **Not required:** a separate Fell or order on each broadleaf or species. No new quota, forced per-broadleaf work or mandatory three treatments is added. Per-species later work would need a new owner decision.
  - **Kept:** three-species depth and S1-C/S1-D.
- **Other places.** A D84-S crosswalk row. MA-7 states what "inspectable" means. The parent checklist has a new prose row for R-105/R-108/R-113, whose lines are unchanged. Child C84-6 and the child checklist CIP-020/021/026/033 row are aligned.
- **Untouched.** Original checkbox identities and statuses, and historical E80 expectations.

### B-2 — Decision-ready M-A evidence boundary (new parent D84-MAE; not an approval)

- **M-A-SCOPE.** Now labelled **UNDECIDED OWNER CHOICE**. The one-stem answer is the author's recommendation only, conditional on non-token audited conditions. Neither #640 nor the owner chose it.
- **Source-fact table.** For each needed fact, it records current durable data and whether that data suffices.
  - Acted-on `targetTreeId`: **yes**.
  - Focal declaration: **no** — `markType` is current mutable state; `prescriptionCropTreeId` applies to Rack V2 prescriptions only.
  - Explicit retained neighbour: **no**.
  - Decision-time 8 m neighbourhood: **not recorded**.
  - Per-focal before/after competition or light: **no** — `competitionIndex` is runtime-only ([ForestEcologyController.cs:52](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ForestEcologyController.cs#L52)), the snapshot is aggregate ([ScenarioEcologicalSnapshot.cs:20](https://github.com/jermur788/CCF/blob/f6d8a223438142b7d3ffdee015857942a5cadcdb/Assets/ForestPrototype/ScenarioOne/ScenarioEcologicalSnapshot.cs#L20)), and a felled stem's DBH is not on its event.
  - Covering review: **yes**.
  - Explicit no-work choice: **no event type exists**.
  - Identity/idempotency: **partial**.
- **Proposed schema (all fields NEW / UNSIGNED).**
  - Focal ID with declaration year.
  - Explicitly retained neighbour inside the 8 m source neighbourhood.
  - Linked non-G1 Approved→Completed order with target, Y_t and cost.
  - Same-focal before/after model input.
  - Covering-year review.
  - A later explicit act *or* no-work record, followed by its review.
  - Append-only, replay-safe integrity.
- **Prohibited substitutes.** A fake no-work `Completed` order, a basal-area or snapshot surrogate, inferred focal or retention, and elapsed years.
- **Route S1.** A versioned Compact-only M-A record, separate from the closed §4.2 object, with no old-world change. It needs a signed amendment and versioned mutation and refusal tests. The author's view is that S1 is the only route that can carry MA-2…MA-6 as written.
- **Route S2.** An existing-state-only predicate. It is **not recommended and not viable on current fields**. Making it viable would require the owner to redefine D-084 M-A intent.
- **Flags, not changed.** #652 live Ops installation is **HELD**. The #659 runbook is docs only, with **no Unity GO**.

## Unresolved owner, save and content fields (one decision package for #640)

1. **M-A-SCOPE** (owner/content owner): does one acted-on stem that meets the D84-MAE conditions qualify? If the owner wants more than one, give any minimum as explicit game calibration. The author recommends "yes"; this is undecided.
2. **M-A evidence route** (save owner + Manager): S1 (recommended) or S2 (not viable as the predicate stands), then a signed save-contract amendment and versioned tests if S1 is chosen.
3. **CG-TRIG binding** (save owner + teaching owner):
   - sign T1-COMPLETE, CT-1…CT-7 and D84-PJ;
   - rule whether a non-empty group with a false acknowledgement is "incomplete" (author recommendation) or an admission refusal;
   - confirm that D84-PJ fits the signed §4.2 row with no new field. ISO-58 stays BLOCKED until then.
4. **Content signature** (content owner):
   - the exact authored world, including the A7 north terminus, H1, guard and the missing data #660 G-1 lists;
   - the G1 site-match tolerance PJ-1 refers to.

   The 175-row roster `dac06a3b…` stays an unsigned roster-only digest.
5. Unchanged and still open: GQ-07 scene/plank isolation, D-032 licensing, #544 registry/profile activation, the F-A native affordability witness, and Tier-1/2/3 plus the owner's personal Editor test. Windows is forbidden until the user approves after a personal test.

## Required next verification (not done by the author)

**One independent non-author changed-lines recheck** (for example the #660 GPT-6 reviewer or another independent GPT-6 browser reviewer), limited to `55f79067..fa6776de` and this result. It is **not** a full 115/33/66 re-audit. Suggested checks:
1. The compare shows only the 8 spec files plus this result, and graphics, manifest and annex are byte-identical.
2. All 115/33/66 checkbox lines are byte-identical and unchecked.
3. T1-COMPLETE has all four conjuncts; CT-3 is a hold only; MA-1 and every listed crosswalk is consistent; nothing adds a §4.2 key.
4. D84-PJ source facts match the cited lines. The transaction pairs only at successful resolution, refuses inference, and marks any extra field as save-owner gated.
5. The §9 note subordinates without deleting, and adds no quota or per-broadleaf work.
6. D84-MAE classifies current fields accurately, does not present S2 as viable, and states M-A-SCOPE is undecided.
7. Nothing claims approval, signature, Unity, native or player evidence.

## What was not done

- No production code, Unity, assets, saves/Prefs, Ops installation, `main` merge or Windows build.
- No owner, content or save decision.
- No native verification: every source statement is a static read.
- The roster, manifest and annex were not re-hashed. They were not touched, so the #660 digest result stands on unchanged bytes.
- **Self-review is not independence.** The author's preservation checks above are evidence for the recheck, not a substitute for it.

**Return to:** Overall Manager [#640](https://github.com/jermur788/CCF/issues/640). Claim #663 is to be commented [HANDED-OFF] and closed after this result is read back.
