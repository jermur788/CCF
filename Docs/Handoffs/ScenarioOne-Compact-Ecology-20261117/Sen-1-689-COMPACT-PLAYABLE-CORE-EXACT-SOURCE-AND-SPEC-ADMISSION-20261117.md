# Sen-1 #689: Compact30 playable core, exact source and spec admission

**STAGE 0 RESULT: READ-ONLY SOURCE AND SPEC ADMISSION. NO SOURCE GRANT.**
**This document gives no approval for C#, Unity, assets, saves, PlayerPrefs, physical Ops, specs, a main merge or Windows.**

| Field | Value |
| --- | --- |
| Claim | [#689](https://github.com/jermur788/CCF/issues/689). [RUNNING registration](https://github.com/jermur788/CCF/issues/689#issuecomment-6958047273) was posted before this file was written |
| Actual worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in T3 Code, host `jer-laptop` (Linux) |
| Binding packet | `Sen-1-689-NON-ASSET-COMPACT30-PLAYABLE-CORE-SOURCE-ROUTING-PACKET-20261117.md` at `dbb94e44` on `docs/sen-1-689-non-asset-playable-core-source-route-20261117-v1` |
| Starting main HEAD | `56a4c09af71fd77da3d810c2e9f6172fe341e4a0` |
| This branch | `docs/sen-1-689-compact-playable-core-source-admission-20261117-v1`, branched from that HEAD. It adds one new file and nothing else |
| Method | `git fetch`, then `git show`, `git grep`, `git diff`, `git merge-tree --write-tree` (a dry-run merge that touches no working tree) and the GitHub REST issue API. Nothing was executed, compiled or run in Unity. No physical worktree, Unity lane, package cache, save or Prefs was opened |
| Manager | Overall Manager [#675](https://github.com/jermur788/CCF/issues/675) |

---

## 0. Answer in brief

1. **Lane A (save/startup) is the only lane with an approved spec.** The original signed shared-save V2 (`200add4e`, §4.2) still authorises its remaining **T-03B** and **T-04** tasks, *on the historical profile only*, without a finished Compact art scene. Compact stays `IsAvailable == false` until the world is signed (#676) and a scene-binding plan is approved. So A can start as soon as **two concrete gates** are cleared. These are not research questions:
   - **G-BASE (new and blocking):** the save line and the Manager-accepted Scenario One gameplay line have **diverged** (§2). Lanes B and C cannot be built on a base that lacks the Annual Review and Cadence code they have to change.
   - **G-PHYS:** physical source custody (#553 BLOCKED) and a real lease or slot for the existing save checkout. #652 HELD only matters once Unity is needed.
2. **Lanes B and C have no approved spec.** The #684 DRAFTs are of review quality, but every one of the 115 + 33 checkboxes is unchecked. Signed #677 is *policy*, not a feature spec. The shortest honest route is **one scoped source-slice approval per lane** (§5), listing the exact R-/CIP-IDs that slice implements. This avoids waiting for whole-spec approval and for OD-16 M-A. Neither lane needs OD-16: M-A is later cycle D.
3. **B and C both write `ScenarioOneManager.cs`.** It has 2,804 lines on the save line and is also touched by lane A and by the D-073 line. To keep one writer per file, the matrix (§4) puts the new behaviour into **new partial-class files**, one per lane. The hook lines in the shared files are **serialised**: A goes first, then B, then C, each landing on the previous lane's handed-off tip.
4. **Nothing has to be rebuilt.** The three-profile registry, the selection context, T-02 admission, the closed three-root envelope with `ccfCompactTeaching` DTO validation, the T-03A bootstrap gate and the Legacy40/Enlarged80 geometry hardening all already exist on the save line (§1). The existing Annual Review acknowledgement already writes event9 once per year. The order/juvenile settlement already exists, but it is not atomic and not linked. #151/#156/#187 do not need to be re-verified.

---

## 1. Source that actually exists (static read)

### 1.1 Shared-save line: `feature/scen1-shared-three-profile-save-v2` @ `209a08e9`

Its merge-base with main is `f52dec24`. It is 12 commits ahead of that point, and **main has changed no `Assets/` file since that merge-base**. Its Assets delta is 15 files, +1001/−101.

| Commit | Task | What is in the code |
| --- | --- | --- |
| `da962372` | T-01 | `ForestSaveProfiles.cs`: fixed Historical / Compact / ScenarioTwo records, per-profile filenames, SI-09 Prefs prefixes, `CanUse`, `OwnsPreferenceKey`. **Compact `IsAvailable=false`** ("no signed authored world binding is installed"). `ForestSaveSessionContext.cs` handles selection, the seal before world creation and `TryAuthorizeWorldConstruction` |
| `84a631d0`, `5e21c944` | T-02 / T-02R (#563) | `ForestSaveEnvelope.cs`: three roots, `ForestCompactTeachingEvidence` with `reviewAcknowledgedYear=-1`. `ForestSaveValidation.cs`: exact-key/raw-shape admission, foreign refusal before mutation, visible menu refusals |
| `b730459a` | T-03A (#570) | Fail-closed bootstrap gate. A partial-apply exception locks Save. Only the manager's `Awake` is gated |
| `78298069` | #629/#638 | Exhaustive Legacy40/Enlarged80 dispatch in `StandGeometryModel.cs`, `StandWorldBoundary.cs`, `ForestStartingStand.cs` (+29/−1), approved by #633 and #640 |
| `f36691ed` | #642/#647 | v24 raw-origin geometry and Reference fixture repair (verification files only) |

**Missing on this line**, per the T-03A result §7 and the signed tasks.md:

- **T-03B:**
  - `ForestStartingStand`/`ForestEcologyController` `Awake` construction gates;
  - a true staged apply (FR-04), which needs a design;
  - Reference-preview return feedback and snapshot retention;
  - exhaustive dispatch for the remaining geometry consumers (`StandSizeMeters`/`CellsPerAxis` 40 m fallback, `Generate` Legacy fallback);
  - Prefs consumers (`LearningObjectivesView*.cs`, `MenuHelpView.cs`) moved to `TryGetPreferencePrefixes`/`OwnsPreferenceKey`.
- **T-04:** the safe temp/replace writer, removal of the `File.Copy` fallback, and explicit backup-to-memory recovery.
- **T-05:** exact-head ISO conformance.

Known verification debt: T-03A mutants 3/16, no Roslyn typecheck against Unity 6000.6, no native run, and the #633 note about stale v23 fixtures (partly repaired by #642).

### 1.2 Gameplay facts on the same line (what B and C must change)

All in `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` @ `209a08e9` unless another file is named.

| Fact | Location | Consequence |
| --- | --- | --- |
| `ResolvePlanting` creates `PlantedJuvenile{juvenileId="PJ"+n}` inline, returns `void`, and keeps no `workOrderId` on the juvenile and no `juvenileId` on the order | ~L1882–1945 | #677 item 7 (atomic direct linkage) is **not implemented**. The cash, stock, juvenile, shelter and status mutations are sequential, with no rollback |
| `AdvanceYear()` resolves every Approved order and then records `WorkResolved` | L1270–1345 | Lane B adds the pairing and the Compact row here. Lane C adds the CT-3 guard at entry |
| `CanAdvanceYearNow` has no review hold. The UI-level hold is `ScenarioOneUiRoot.AdvanceFromWorkPlan → NeedsAnnualReview` | L324; `UI/ScenarioOneUiRoot.cs` ~L460 | CT-3 currently exists **only in the UI**. #677 item 4 requires *every* route, services included |
| `MarkAnnualReviewSeen()` uses `ShouldRecordAcknowledgement` to write event9 once per year | ~L387–400; `UI/AnnualReviewView.cs` L66; `ScenarioOneUiRoot.AcknowledgeAnnualReview` L438 | This is the existing control #677 item 3 binds. Lane C only adds the atomic `reviewAcknowledgedYear=Y*` write. **No new button** |
| `CancelApprovedOrder` removes the order and records `OrderCancelled`. `SetPlantingExecution`/`SetPlantingShelters` are global planning setters. `ApprovePendingWork`, `RemovePendingOrder`, `AddMarkedTreesToWorkPlan` | L892, L901, L1146–1240 | The W-A freeze audit starts here. Cancellation already exists as removal, not demotion. That is correct, but it needs the "new Pending order" reuse path and the Compact guard on every setter |

### 1.3 The divergence that blocks B and C (G-BASE)

| Line | Tip | Contains save line? | Contains D-073 / Cadence V1 / C-01? |
| --- | --- | --- | --- |
| `main` | `56a4c09a` (last Assets change `9a6ed000`) | no | **no** |
| Save line | `209a08e9` | n/a | **no** |
| `integration/scenario-one-personal-playtest-d073-20261112-v1` | `55863f1b` (Manager #424 conditional personal-Editor GO, #412) | no | yes (+74 commits over main, 38 non-meta Assets files) |
| `feature/scenario-one-objective1-ux-repair-20261112-v1` | `acb4e111` | no | yes, plus #443 Objective 1 repair (**#443 still RUNNING, G4 not verified**) |

- `git merge-tree --write-tree` of save line × `55863f1b` gives a **textually clean** tree, `f1b0b70b`. Save line × `acb4e111` also gives a clean tree, `87ed0bb7`.
- **Three production files are modified on both sides:** `ForestSaveController.cs`, `ForestStartingStand.cs` and `ScenarioOneManager.cs`. A clean text merge is **not** a semantic merge. In particular, it is unverified whether the D-073/Cadence line changed any Scenario One saved DTO field or advance route that the T-02 exact-key admission and the v24 fixtures rely on.
- **Cadence V1 adds advance paths** (`ScenarioManagementCadence.cs`, "dual advance"). CT-3 must cover these. A CT-3 written on the save line alone would miss them.
- Fix `#370` (annual review reset) is in neither line. Whether it is superseded needs checking.

**Required Manager decision (G-BASE):** name the one gameplay integration base for Compact source.

- **Recommendation:** the Manager-accepted D-073 personal-playtest line `55863f1b`. This is the latest *accepted* gameplay, while #443 is unfinished.
- Then reserve one bounded **save-line reconciliation** (packet A0 in §6) that merges `209a08e9` onto that base and re-runs only the T-02/T-03A/#629 focused harnesses plus a Roslyn typecheck.
- The alternative, building on main/save line and porting D-073 later, makes B and C write CT-3 and review code twice. It is not recommended.

---

## 2. Spec and authority status, per lane

| Authority | Status (verified at the link/commit) | Authorises |
| --- | --- | --- |
| Shared save V2 four-file spec `200add4e` + [D-A signature](https://github.com/jermur788/CCF/blob/docs/scen1-manager-save-v2-immutable-approval-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md) | **SIGNED** (out-of-band; the in-file banner is the pre-signature snapshot) | T-01…T-05 persistence policy. Compact is a reserved, unavailable identity. **Yes for lane A T-03B/T-04 on Historical.** No Compact activation |
| [#677 D-A signature](https://github.com/jermur788/CCF/blob/docs/sen-1-677-manager-cgtrig-exact-byte-signature-20261117-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-677-MANAGER-CGTRIG-SAVE-POLICY-EXACT-BYTE-D-A-SIGNATURE-20261117.md) over supplement blob `bb9327fe` @ `bfa01f86` | **SIGNED technical policy** (Manager under delegated authority, not the product owner) | Policy B, CG-TRIG, the CT-3 hold, the two refusals, atomic linkage, the W-A freeze. It explicitly states **"NOT AUTHORISED FOR SOURCE"** and leaves specs DRAFT |
| #684 DRAFT specs @ `0725f078` (complete-loop + initial-planting, 4 files each; graphics untouched) | **DRAFT, NOT APPROVED**, 0/115 R and 0/33 CIP checked; document quality accepted by #675 | Nothing for source |
| D-083 / D-084 / D-085 on main | Accepted product **intent** | Design direction only; nothing signed |
| #676 world manifest | Review APPROVED, content **signature-blocked** (0 signed, world_digest null) | Nothing: no Compact identity, scene or save destination |
| OD-16 M-A-SCOPE | **DEFERRED** by the owner | Only cycle D needs it. A, B and C do not |

**Can narrower Compact Work Plan / UI rules be approved separately from OD-16?** Yes. W-A, the 2+2 group, CT-2/CT-3 and Policy B are all fixed by D-084 plus signed #677. None of them depends on the M-A threshold. A **scoped source-slice approval** is enough for B and C. It must name the exact R-/CIP-IDs, the file allowlist and the witness list, and it must be signed by the person or role with spec-approval authority for Compact. This is **PENDING SIGNATURE**; this document does not claim it exists.

---

## 3. Shared API and integration target

- **Target:** the single G-BASE branch, extended in order: **A-tip → B lands on A-tip → C lands on B-tip.** Each lane works on its own feature branch cut from the previous lane's *handed-off* head. Manager #675 performs the integration merge after independent review. Nothing goes to main without a separate Manager verdict.
- **A → B/C API**, already present on the save line, so B and C *consume* it and never edit it:
  - `ForestSaveSessionContext.Current.SelectedProfile`
  - `ForestSaveProfiles.Compact`
  - `ForestCompactTeachingEvidence` (DTO fields unchanged)
  - the `ForestSaveValidation` admission entry
  - A new read-only helper, `bool ForestSaveSessionContext.IsCompactSelected`, may be added by **A only** if B or C ask for it.
- **B → C API**, new and owned by B: `ScenarioOneManager.CompactPlanting.cs` exposes:
  - `IReadOnlyList<CompactPlantingRow> CompactPlantings`, built from the paired order↔juvenile records;
  - `int CompactFinalResolutionYear`, which is Y\*, or −1.

  C reads these. It never writes plantings.
- **Save capture/restore of Compact rows and `reviewAcknowledgedYear`** has one owner: **lane A** owns `ForestSaveController.cs`/`ForestSaveValidation*.cs`. B and C expose pure getters and a restore-input struct, and A wires them into capture/admission in its last packet, A3 (§6). This keeps the save files single-writer. The #677 two-state refusal checks and the Policy B reconciliation live in A's validation, fed by B's and C's pure rule functions.

---

## 4. Code admission matrix (three lanes)

| | **Lane A: save/profile identity** | **Lane B: Work Plan and planting** | **Lane C: assessment, Annual Review, CT-3** |
| --- | --- | --- | --- |
| Player-visible result | Start/Continue/Save/Load stay in the selected slot. Wrong-profile, foreign or invalid files are refused before mutation. Historical saves are untouched. Safe write with explicit backup recovery | An Approved order is frozen everywhere. Cancel means audited removal plus a new Pending order. Four genuine 2+2 plantings settle atomically, one order to one juvenile | Real arrival and assessment acknowledgements. The existing Annual Review acknowledgement writes event9 plus `reviewAcknowledgedYear=Y*` in one step. CT-3 holds every advance route across reload |
| Code already done (reuse, do not rebuild) | T-01, T-02/T-02R, T-03A, #629 geometry, #642 fixtures (§1.1) | Order lifecycle, `CancelApprovedOrder` as removal, `ResolvePlanting` biology/stock/cash, shelters, `WorkResolved` costs | `MarkAnnualReviewSeen` once-per-year event9, `AnnualReviewView` button, UI `NeedsAnnualReview` hold, `ScenarioOneCompletionRules` chronology |
| Code missing | T-03B items 1, 3, 4, 5. Item 2 (staged apply) needs a design note first. T-04 writer and recovery. Compact capture/admission wiring for rows and the review field. The #677 two refusals and Policy B reconciliation in validation | Order↔juvenile pairing (no new save keys: derived from existing order/juvenile/event data, or carried inside the already signed `ccfCompactTeaching.plantings`). Exception-atomic settlement with rollback. Compact W-A guard on every public setter (`SetPlantingExecution`, `SetPlantingShelters`, `SetPendingFellingOutcome`, bulk add/approve paths). 2+2 designated group binding (`groupOrderIds`) | Arrival/assessment interaction signals (control only; H1 *content* stays with #676/#688). Atomic event9 + field in the existing ack path. CT-3 guard in `AdvanceYear`/`CanAdvanceYearNow` and **every Cadence V1 advance/skip route** on the G-BASE line. Retained Y\* report presentation |
| Spec authority | **Signed V2 (sufficient for Historical scope).** Compact rows/refusals: signed #677 items 1, 5, 6, 9 plus a scoped approval of the parent CT-7 cases 12–14 | **Missing:** scoped source-slice approval over D84-PJ-1/PJ-4a/PJ-7, D84-W-1/W-3, child C84-3/5/7 (#684 @ `0725f078`). Signed #677 items 2, 7, 8 | **Missing:** scoped source-slice approval over D84-T CT-2/CT-3/CT-4, §684-1/-2/-6 (#684 @ `0725f078`). Signed #677 items 3, 4. Reviewer NOTE **D-N1** must be resolved in that approval, not by the implementer |
| Exclusive production writer files (proposed allowlist) | `ForestSaveController.cs`, `ForestSaveValidation.cs`, `ForestSaveValidation.Geometry.cs`, `ForestSaveEnvelope.cs` (no field change), `ForestSaveProfiles.cs`, `ForestSaveSessionContext.cs`, `ForestStartingStand.cs`, `ForestEcologyController.cs` (Awake gate only), `StandGeometryModel.cs`, `StandWorldBoundary.cs`, `UI/StandaloneSessionMenu*.cs`, `UI/LearningObjectivesView.cs`, `UI/LearningObjectivesView.Curriculum.cs`, `UI/MenuHelpView.cs`, and only the **capture/restore/preview regions** of `ScenarioOneManager.cs` | **New** `ScenarioOne/ScenarioOneManager.CompactPlanting.cs` (+ `.meta`), **new** `ScenarioOne/CompactPlantingRules.cs` (pure, testable). Only the `ResolvePlanting` + listed setter regions of `ScenarioOneManager.cs`. `UI/WorkPlanView.cs` (frozen-state presentation) | **New** `ScenarioOne/ScenarioOneManager.CompactTeaching.cs` (+ `.meta`), **new** `ScenarioOne/CompactTeachingRules.cs` (pure). Only the `MarkAnnualReviewSeen`/`AdvanceYear`-entry/`CanAdvanceYearNow` regions of `ScenarioOneManager.cs`. `ScenarioManagementCadence.cs` (advance guard only). `UI/AnnualReviewView.cs`, `UI/ScenarioOneUiRoot.cs` |
| Forbidden to this lane | Gameplay rules, Work Plan, review UI, Compact availability flip | Save files, review/advance code, UI root, geometry | Save files, planting/settlement, Work Plan setters, geometry |
| Shared-file rule | `ScenarioOneManager.cs` is edited by **one lane at a time**, in the order A → B → C. Each lane rebases onto the previous handed-off tip. No concurrent branches touch it | (same) | (same) |
| Physical gate | #553 custody of the existing checkout `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` (**state NOT inspected here**), clean HEAD = A0 result, a real lease. #652 HELD blocks only the Unity Tier 2 phase | New isolated worktree from the A handed-off tip. Same #553 / Unity-lease rule | New isolated worktree from the B handed-off tip. Same rule |
| Future issue / branch | A0, then A1–A3 under #544 custody: `feature/scen1-shared-save-v2-on-<base>-reconcile` (A0), then `feature/scen1-shared-three-profile-save-v2` continues | `feature/sen-1-compact-workplan-planting-v1` | `feature/sen-1-compact-review-ct3-v1` |
| Focused negative tests (Tier 1 first) | Existing T-02/T-03A/#629 harnesses re-run once on A0. New: Prefs consumer namespace isolation; geometry case 2 refuses at every consumer; T-04 IO injection (temp/move/replace/permission/stale tmp; primary and .bak untouched before commit); the #677 two refusals and Policy B cases 12–14 as fixtures; **Roslyn typecheck against Unity 6000.6 DLLs** | Fault injected at each settlement step (pairing, stock, cash, juvenile, shelter, status, event) leaves no half state; idempotent repeat; 3+1 and missing-row become admitted incomplete with no backfill; every setter refuses on Approved (UI and service); cancel → new Pending, stock and quote released; 12 or more mutants on the guard and pairing code | CT-3 blocks UI, service, Cadence skip and dual-advance routes, and survives Save→Load; single ack writes event9 and field together or neither; second ack is a no-op; pre-final-work event9 (D-N1 case) as ruled by the approval; no second event9 or button; Historical review behaviour byte-identical; mutants on the guard |
| Tier 2 (Unity, later) | ISO subset named in T-03/T-04 on Historical only, with disposable saves/Prefs | Only after a signed G1 world exists. Before that, the code ships behind Compact `IsAvailable=false` with offline witnesses only | Same as B |
| Do not repeat | #151/#156/#187 suites, full ISO-01…63 (that is T-05), 100-test sweeps | (same) | (same) |
| Finish handoff | Durable GitHub Markdown under `Docs/Handoffs/ScenarioOne-Save-Orchestration-*`: exact SHA, file list, commands and results, NOT RUN list, then a distinct-family reviewer | Same, under `ScenarioOne-Compact-Ecology-*` | Same |

**Why partial-class files:** they give B and C their own files, so two lanes never write the same lines. The small hook edits inside `ScenarioOneManager.cs` stay sequential and reviewable. This follows the existing pattern (`ScenarioOneManager.Racks.cs`, `.Salvage.cs` and others) and adds no framework.

---

## 5. Missing approvals and the order to obtain them

| # | Gate | Owner | Blocks | Concrete ask |
| --- | --- | --- | --- | --- |
| G-BASE | Name the gameplay integration base for Compact source (recommendation `55863f1b`) | Manager #675 | A0, and through it all three lanes | One recorded decision. If the choice is #443's line, wait for #443 G4 first |
| G-PHYS | #553 custody and a lease for the save checkout, verified clean HEAD, no competing writer. #544 custodian acknowledgement | #553 owner / CCF-Ops maintainer; #544 custodian | A0–A3 source; B and C worktrees | Physical check plus RUNNING claims. A GitHub issue alone is not a lock |
| G-A-DESIGN | One-page design for T-03B item 2 (staged apply, FR-04) | Lane A writer, Manager acceptance | A2 only. A1 can proceed without it | Short note picking the mechanism (inactive root vs additive scene) |
| G-B-SPEC | Scoped source-slice approval for lane B (IDs listed in §4) | Spec-approval authority for Compact (Manager under delegation or owner, recorded) | B source | A signed scoped approval naming R-/CIP-IDs, allowlist and witnesses. **PENDING SIGNATURE** |
| G-C-SPEC | Same for lane C, plus a D-N1 ruling | Same | C source | Same. **PENDING SIGNATURE** |
| G-UNITY | #652 live Ops install, or another verified Unity lease | #652 owner | Every Tier 2 run (not Tier 1) | Lift the hold or name the slot |
| G-WORLD | #676 owner signature on full world bytes plus an approved scene plan | Product owner | Compact `IsAvailable=true`, every Compact runtime witness, ISO-58 | Not needed to *start* A, B or C |

---

## 6. Next executable implementation packets (in order)

Each packet: one claim, one writer, a different-family reviewer, a Tier 1 gate before any Unity run, and a GitHub handoff.

1. **A0: save-line reconciliation onto G-BASE.** Medium. One writer within #544 custody.
   - Merge `209a08e9` into the chosen base.
   - Resolve the three dual-modified files semantically, checking D-073 DTO and advance-route changes against T-02 exact keys and the v24 fixtures.
   - Re-run the existing T-02/T-03A/#629/#642 harnesses once, plus a Roslyn typecheck.
   - No new behaviour. Stop on any save-schema drift.
   - **Starts when G-BASE and G-PHYS pass.**
2. **A1: T-03B mechanical items 1, 3, 4, 5.** Medium. Same writer, after A0. Signed V2 is sufficient.
3. **A2: T-03B item 2 (staged apply) and T-04 safe writer/recovery.** High. Needs G-A-DESIGN. Ends with a Tier 2 Historical-only Unity run once G-UNITY is cleared.
4. **B1: Compact W-A freeze and atomic 2+2 settlement**, behind `IsAvailable=false`. High. Starts after **G-B-SPEC** and the A1 handoff, landing on the A1 tip. A2 may run in parallel because B1 does not touch save files.
5. **C1: atomic review field and CT-3 on all routes.** Medium-high. Starts after **G-C-SPEC** and the B1 handoff, landing on the B1 tip.
6. **A3: Compact capture/admission wiring** for B and C state plus the #677 refusals. Medium. After B1 and C1. Lane A writer only.
7. **Later, gated on G-WORLD:** Compact availability flip, scene routing, Tier 2 Compact witnesses, ISO-58. **Cycle D** (M-A episode, S1 schema) needs its own spec and the OD-16 owner decision.

**Shortest route to the first code start:** the Manager records G-BASE, and #553/#544 confirm custody of the save checkout. Then A0 is fully executable on already signed authority. G-B-SPEC and G-C-SPEC can be prepared in parallel while A0 and A1 run.

---

## 7. Not verified

- The physical state of the save checkout `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115`, the CCF-Ops board, lanes and leases: **not inspected** (outside this read-only scope).
- Whether the D-073/Cadence line changes saved DTO fields or advance routes that conflict *semantically* with T-02 admission. Only the text merge was dry-run.
- Whether fix #370 is superseded on the D-073 line.
- The current content of #443 G4 work.
- No compile, typecheck, harness, mutant or Unity run was done. Every test named above is **NOT RUN**.
- Issue-thread claims (#544, #638, #647 results) were read as published text. Commit hashes were matched against `git` objects; runtime claims inside them were not re-verified.
- Dashboard / Drive mirrors / `active-tasks.md`: not edited, **NOT VERIFIED**.

## 8. Handoff to Manager #675

**State: HANDED-OFF (Stage 0 admission only).** Requested: (1) decide **G-BASE**; (2) route **G-PHYS** to the #553/#544 owners; (3) if accepted, reserve **A0** as the first real source claim; (4) open the two scoped spec-slice approvals (G-B-SPEC, G-C-SPEC), with D-N1 settled in G-C-SPEC. No owner action is required for this static admission.
