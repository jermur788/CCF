# #457 — Independent non-author review of the #454 compact one-rack complete-loop DRAFT

**13 November 2026 | Reviewer: Anthropic Claude Opus 5.5 (`claude-opus-5-5`, Claude Code) | Claim: [#457](https://github.com/jermur788/CCF/issues/457)**
**Binding packet:** [Manager #425 independent review packet](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-independent-review-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/MANAGER-425-454-INDEPENDENT-ONE-RACK-DRAFT-REVIEW-PACKET-20261113.md)
**Reviewed candidate:** `docs/compact-one-rack-complete-loop-draft-20261113-v1` @ `0d857afa58bfe120de6460cbcd18097861437684` (five added Markdown files over `main` @ `6c13a9e182774945bf391bc8d4845ce74d3e5e88`)
**Reviewer branch:** `docs/compact-one-rack-independent-review-20261113-v1`, starting HEAD `432465f415d35212ae3ecf07ce42173e55b37532`. Worktree: NONE.

## Verdict

# NEEDS CHANGES — DRAFT QUALITY ONLY

This verdict rates **DRAFT document quality only**. It is **not** user approval, **not** the spec APPROVED lifecycle state, **not** an implemented feature, **not** Unity verification and **not** acceptance. The reviewer cannot promote the DRAFT; Manager #425 decides what happens next. No requirement box was checked, and no OPEN-C1–C7 or OPEN-1–OPEN-6 decision was made.

**Summary.** The DRAFT is honest and well-bounded. It traces the user's one-rack decision correctly, defers only the five-rack lesson and MFM extrapolation, keeps all three species, annual ecology, economics, review/history and repeated interventions in scope, refuses invented dimensions, and puts a pre-code fixture gate and a separate save slot first. It does not claim implementation, Unity results or approval. **Rejection is not warranted.**

It needs changes for three reasons:
- **The save/isolation contract misses facts in current `main`.** Today a future-version save loads "best-effort". A scenario-ID mismatch only logs a warning, and only after the world has already started changing. Tutorial/help progress is stored in PlayerPrefs for the whole device, not per world. Geometry code treats any value other than Enlarged80 as 40 m.
- **It does not cite or reconcile directly overlapping specs that already exist on GitHub:** the #436 O2 save-isolation DRAFT, the #413 D-074 spaced-intervention DRAFT and the approved cadence/time-advance design.
- **Several acceptance clauses are still untestable or internally inconsistent.** Examples: what counts as "substantive", which tree set drives the opening overlay, and residual-BA figures presented as "existing model outputs".

All the repairs are document edits. None needs a product decision from the reviewer.

## Prioritised findings

Each finding gives: the exact spec heading or requirement ID, the **authority** (what binding source requires it) separately from the **design suggestion** (the reviewer's proposed repair, which is not binding), and the repair.

### Blockers (fix before Manager treats the DRAFT as review-ready)

**B1 — Pre-mutation identity guard: current legacy load behaviour is missing and the guard's placement is unspecified.**
*Where:* spec.md §2 row `ForestSaveController.cs / ForestSaveData.cs / ForestSaveValidation`; §3 **S-02**, **S-03**; checklist **R-013, R-014, R-015, R-018**; plan Stage A.
*Verified source facts (main @ 6c13a9e1):*
- `ForestSaveController.cs:346-347` logs `"...newer than supported version...; loading best-effort"` and **continues loading** future-version saves.
- `ScenarioOneManager.cs:1481-1482` handles a `scenarioId` mismatch with **a warning only, then loads**. This restore (`RestoreSaveData`) is called at `ForestSaveController.cs:517`. By then `LoadData` has already applied the save geometry (`ecology.ApplyStandGeometry`, line 356, the first world mutation) and restored the trees (lines ~400–435).
- `CanLoad` (`ForestSaveController.cs:240-258`), which decides whether the title screen offers Continue, validates the single `forest-save.json` against whatever scene is live.
*Problem:* S-02 requires future schemas to be "detected as incompatible rather than best-effort". It does not say whether this applies only to the compact slot or whether it **changes the protected legacy behaviour**. S-03 forbids changing legacy behaviour. As written, an implementer could put the world check in the existing post-mutation `scenarioId` path and still pass the wording.
*Authority:* D-081 ("reject wrong-scenario loads **before any mutation**"); the #425 decision memo §2.6; S-02 itself.
*Repair (design suggestion):*
- Add both facts to §2.
- Require the compact world/slot/version check to run inside the **pre-mutation validation stage** (the `TryReadSave`/`FindLoadProblem`/`CanLoad` equivalents), never in `ScenarioOneManager` restore.
- State that legacy `forest-save.json` future-version handling **stays exactly as it is** unless the save owner records a separate decision.
- Require a profile-scoped `CanLoad`/Continue.
- Add a checklist item: a compact save with a mismatched or absent world ID must be refused even when its geometry, species and cells would otherwise validate.

**B2 — The competing save-boundary DRAFT (#436) is not cited, so two specs would define the same router.**
*Where:* spec.md §3 **S-04** ("The proposed shared save router…"); **OPEN-C4**; **S-21** gate 3; tasks **A-01**.
*Verified:* branch `docs/o2-save-isolation-four-part-draft-spec-20261112-v1` @ `50496929` holds `specs/scenario-independent-save-isolation-v1/{spec,plan,tasks,checklists/requirements}.md` (**DRAFT — NOT APPROVED**, task #436). It already proposes:
- SI-02: O1 stays at `forest-save.json`;
- SI-03: O2 slot `forest-save-scenario-two.json`;
- SI-05: how legacy saves without an ID are classified;
- FR-02: validation order, rejecting unknown/newer versions;
- FR-06: user-driven `.bak` recovery.
It also still states that O1 "remains the complete first external demo with … Rack V2 five parallel racks, MFM", which the one-rack decision now overtakes for the first demo.
*Authority:* D-081 requires the world/save boundary to go through **one separately APPROVED four-part spec**; AGENTS.md says one writer per subsystem.
*Repair (design suggestion):*
- Cite #436 by branch and SHA.
- State the relationship: either (a) the compact profile becomes a third fixed slot inside #436's router, or (b) this DRAFT owns the O1 portion and #436 is amended. Manager/save owner chooses; **do not decide it in this spec.**
- Name a single save-boundary spec owner as a gate in S-21.3 and tasks A-01.
- Add #436's backup-recovery and future-version rules to the compact negative matrix, or explicitly defer them.

**B3 — Device-wide PlayerPrefs learning/help progress is shared between worlds, and the DRAFT does not address it.**
*Where:* spec.md §3 **S-02** (PlayerPrefs is mentioned only as a mutation-ordering concern); §4 **S-06/S-07**; checklist **R-013–R-016, R-025**; **S-22**.
*Verified:*
- `LearningObjectivesView.Curriculum.cs:9` uses the prefix `CCF.Learning.v1.` with step IDs such as `rack.read`, `rack.create` and `rack.layout`. The `rack.layout` text says "five racks in all", and the source comment says "never rename or reuse" step IDs.
- `MenuHelpView.cs:11` uses `CCF.MenuHelp.v1.`.
- Both are read and written through `PlayerPrefs` (`LearningObjectivesView.cs:89-108`, `MenuHelpView.cs:64-155`). That state lives outside any save file and applies to the whole device.
*Problem:* If compact reuses these views, compact progress would mark the legacy five-rack lesson steps done, or legacy progress would pre-complete compact steps. Either way, one world's learning state changes the other's, and no slot checksum would catch it. A "rejected load leaves the entire live/disk state byte-identical" check (S-02) also needs PlayerPrefs in scope.
*Authority:* the decision memo ("old saves … never … reinterpreted as the compact scenario"); S-02's own PlayerPrefs clause; the packet's Q3 ("before scene/disk/Prefs mutation").
*Repair (design suggestion):*
- Record the fact in §2.
- Require a per-profile learning/help namespace, or an explicit save-owner policy. Legacy `CCF.Learning.v1.*` and `CCF.MenuHelp.v1.*` keys must stay unchanged.
- Add both-direction negative tests (compact play does not change legacy keys; legacy keys do not complete compact steps).
- Include PlayerPrefs in the "rejected load is byte-identical" oracle.
- Fold the namespace question into OPEN-C4 or a new save-owner OPEN item.

### Majors

**M1 — Geometry code silently treats any unknown model as Legacy40.**
*Where:* §2 row `StandGeometryModel.cs`; **S-01**, **S-05**; **R-011**; plan Stage B Tier 1.
*Verified:*
- `StandGeometryModel.StandSizeMeters` is `model == Enlarged80 ? 80f : 40f`, so any new model ID becomes **40 m**. `CellsPerAxis`, `CellCount` and `CellIndex` all use it.
- `IsKnown` accepts only Legacy40 and Enlarged80.
- `ForSave` returns Legacy40 for any pre-v20 save or missing field.
- More binary geometry ternaries exist in `ForestSaveValidation.Geometry.cs`, `ScenarioOneObjectives.cs`, `ScenarioReferenceArchive.cs` and `StandWorldBoundary.cs`.
*Problem:* S-01 forbids a Legacy40 alias but does not identify the code mechanism that would create one by default.
*Authority:* the decision memo §2.2 ("Do not alias Legacy40"); S-01.
*Repair (design suggestion):* Add the fact to §2. Require Stage B to inventory every binary geometry site and make unknown values fail closed. Add a Tier-1 test: a compact geometry ID can never resolve to the 40 m or 80 m size or cell count through a default branch.

**M2 — Reference identity hashing vs. a schema append: S-03 contradicts S-01/OPEN-C4.**
*Where:* **S-03** ("Do not … alter serial field layouts"); **S-01**; **OPEN-C4** ("version/schema"); **R-022**.
*Verified:* `ScenarioReferenceArchive.cs:265-284` computes world/reference identity by removing later fields with exact text patterns: `WithoutLaterFields`, `WithoutObjectiveAchievements`, `WithoutTeachingCompletion` (which removes the v24 latch). Any new field added to the shared `ForestSaveData` changes the serialized JSON. It would change legacy and Reference identities unless it is stripped the same way.
*Problem:* The DRAFT both forbids changing field layouts and expects a possible schema append and a new version number.
*Authority:* the decision memo ("No old world hash rebaselining"); D-074 ("no hidden event or world-hash change").
*Repair (design suggestion):* Replace the contradictory sentence with an explicit two-way choice for the save owner:
- (a) compact data lives in a separate root/type and the legacy `ForestSaveData` layout does not change; or
- (b) appended fields must be proven identity-neutral for every legacy/Reference world using the existing stripping pattern.
Add to R-022 a test that legacy and Reference identity hashes are identical before and after the compact source lands.

**M3 — "Substantive intervention" is undefined, and the existing D-074 DRAFT that proposes a definition is not cited.**
*Where:* **S-13**, **S-15**; **OPEN-C5**; **R-051, R-052, R-053, R-062**; plan Stage D; tasks D-03.
*Verified:* `specs/scenario-one-spaced-major-intervention-learning-v1/spec.md` exists on `docs/fable-hp-d074-d076-spec-programme-20261112-v1` @ `61f6fb09`. It is a DRAFT, and the [Manager #413 review](https://github.com/jermur788/CCF/blob/docs/manager-hp413-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-413-D074-D076-DRAFT-SPEC-REVIEW-20261112.md) returned NEEDS CHANGES. It contains:
- FR-02: a proposed substantive-treatment evidence rule;
- FR-04: Option A (≥3 elapsed years) versus Option B (no numeric minimum);
- MD-02 and MD-03: the open decisions;
- FR-06: v24 saves cannot tell a new D-074 world from an old resave.
Current `HasReviewedTwoCycleSequence` (`ScenarioOneCompletionRules.cs:36`) counts **any** succeeded resolved work year, so rack clearance and maintenance count too.
*Problem:* R-051/R-052/R-062 cannot be tested until a classifier exists. The DRAFT gives one criterion as OPEN ("exact minimum elapsed-years/criterion") but never states the classifier itself as an open decision. It also misses the chance to say that a distinct compact world ID resolves FR-06's origin problem for new worlds.
*Authority:* D-074 (requires a spec on "what counts" and on the 3-year option as GAMEPLAY CALIBRATION); D-077 P-1 (keep the old predicate unchanged); D-060 ("second and later interventions").
*Repair (design suggestion):*
- Cite the D-074 DRAFT and its MD-02/MD-03.
- Add an explicit OPEN item (or expand OPEN-C5) for the **substantive-intervention classifier and interval rule**, and make it a dependency of D-03.
- Note that compact world identity can carry an episode/origin marker that the old world cannot.
- State the authority for "three": it is the Manager's interpretation of D-060's "second and later" interventions (#425 packets), not a number the user stated directly.

**M4 — The cadence/time-advance state is understated, and D-077's teaching-completion eligibility has no compact equivalent.**
*Where:* §2 row `S1-C history / S1-D cycles / D-073–D-077`; **S-14**; **OPEN-C7**; **R-057–R-061**; tasks D-04.
*Verified:*
- `specs/scenario-one-management-cadence-and-advance-v1/spec.md` on `docs/manager-cadence-review-disposition-20261112-v1` reads "**APPROVED DESIGN / RELEASEABLE FOR BOUNDED IMPLEMENTATION; NOT IMPLEMENTED / VERIFIED**".
- The #413 specs cite the isolated cadence source **E** = `e759193354b4613f88e3370a62ccaccf02c556cf`, feature-qualified under #402/#408.
- D-077 makes V1 skip eligible "only after the current teaching sequence is complete". In the old world that relies on the teaching sequence and the v24 latch, which S-09/S-03 correctly say compact must **not** import.
*Problem:*
- "D-077 skip design accepted, not independently qualified on compact" omits the approved cadence spec and the existing isolated candidate.
- S-14 does not say what "teaching complete" means in compact. Without that, D-077 eligibility in compact is undefined.
- D-077 **P-2** is not reflected: any successful FellTree year, including rack-access felling, anchors the navigation suggestion. That matters if compact's first act is rack clearance.
*Authority:* D-075, D-077 (P-1, P-2, V1 eligibility).
*Repair (design suggestion):* Cite the cadence spec and candidate E as OPEN-C7 inputs. Add a requirement that compact defines its own teaching-complete signal before skip eligibility (one for P and one for A, since OPEN-C1 changes it). Reflect P-2 in S-14.

**M5 — The Work Plan residual-BA/canopy estimate and per-item access feasibility are presented as existing outputs.**
*Where:* **S-10** ("post-work retained canopy/basal area (estimated), physical access feasibility … Ground all values in current trees and existing model outputs"); **R-039**; plan Stage C ("Reuse Work Plan quotation, current BA/volume/residual…").
*Verified (limited):* A targeted `git grep` on main for `residual|retainedBasal|remainingBasal|BasalAreaAfter|canopyAfter` across `Assets/ForestPrototype/*.cs` found **no match**. The Work Plan does show a per-visit minimum (`WorkPlanView.cs:337`, `WorkPlanOverview.cs:129`). The reviewer did not trace every Work Plan field, so this is "not found by targeted search", not proof that it is absent.
*Problem:* If these outputs are new, the DRAFT treats new feature scope as reuse.
*Authority:* AGENTS.md (no silent scope expansion); the decision memo §C ("calculate removed/retained BA").
*Repair (design suggestion):* Mark residual BA/canopy and access-feasibility outputs as **new** Stage C scope unless a source audit proves they exist. Specify the method (current trees minus the selected removal set, with no growth forecast) and its L-level provenance.

**M6 — The input set for the opening overlay is inconsistent.**
*Where:* **S-17** ("actually authorised planned Fell set"); **R-067** ("current removal set"); **R-068** ("updates on undo/approval"); decision memo §2.4 ("current proposed fell set"); #453 ("pending/approved proposed removals"); **OPEN-C6**.
*Problem:* It is unclear whether the overlay draws from draft player marks, Pending orders, Approved orders, or a combination. Each answers a different player question, and the label has to say which one is shown.
*Authority:* the decision memo §1 (separate user paint, draft/approved orders and history).
*Repair (design suggestion):* Use one defined term across S-17, R-067 and R-068, and require the overlay label to name the set shown. Whether that is draft marks, Pending, Approved, or a player toggle is a design choice to record, either as an OPEN item or via OPEN-C6. The reviewer does not decide it.

**M7 — Gate order and the lane for the pre-code study are ambiguous.**
*Where:* spec header "Implementation gate"; **S-21** gates 1–3; plan Phase 0 (last bullet: "Publish approved assumptions and amended four files…"); tasks **C0-03** ("Browser/offline audit").
*Problem:*
- It is unclear whether the spec becomes APPROVED **before** the C0-03/C0-04 results (with OPEN-C2/C5 still open) or **after** they are folded in, and whether the amended four files need another independent review.
- `ForestStartingStand` is "not generic to arbitrary stand dimensions" (#453 §3.1). Generating "2+ genuine candidate data sets" therefore needs new offline code, which a browser-only lane cannot produce.
*Authority:* AgentWorkflow single-writer and lane rules; S-21.
*Repair (design suggestion):*
- State the lifecycle explicitly: DRAFT → amended DRAFT with C0-03/C0-04 evidence → bounded delta review → APPROVED.
- Give C0-03 a disposable offline-harness lane (the `Tools/Verification/` copy/run/remove pattern) with its own claim and #424 lease check, and no production source writes.

### Minors

- **m1 — §2 economy wording.** It says "€2,500 minimum **annual** commissioned harvest cost". The source is `minimumHarvestJobCents = 250000`, described in the UI as a "minimum charge **per contractor visit**" (`WorkPlanOverview.cs:129`, `WorkPlanView.cs:337`). Correct "annual" to "per contractor visit". S-11's "job minimum" is fine. €12,000 (`startingCashCents = 1200000`) and the year-25 minimum (`minimumCompletionYear = 25`) are confirmed.
- **m2 — R-002** says the review "resolves blocking decisions". A reviewer cannot resolve product decisions. Reword to "identifies blocking decisions; authority resolves them".
- **m3 — R-010 and R-082** both cover dashboard/Drive sync. Merge them or make the distinction explicit (canon reconciliation vs. final release).
- **m4 — R-036.** "Original MFM … regression" should name the **existing tests at the pinned candidate**. The milestone shows MFM-08/09/10 not yet complete, so "original" is ambiguous.
- **m5 — R-021, "genuine old v1–v24 saves".** Require an inventory and provenance record for the historical fixtures. Hash fixtures exist under `Tools/Verification/{MfmSaveProvenance,S1CObjectiveHistory,TeachingPersistence,RackV2Save}`. Fixtures newly written by the new code do not prove backward compatibility.
- **m6 — S-07 "habitat trees/deadwood/features to retain".** `TreeMarkType` on main is None/Fell/CropTree (with Rack V2 entrance semantics); the reviewer found no habitat-retain mark. Add the decision memo's qualifier "where authoritative game features support them" so that an implementer does not read a new retention mark type as required scope.
- **m7 — S-08 Approved-order edits** gives two alternatives ("withdraw approval/revalidate … or block"). Record the choice as a design decision or OPEN item. Also record the current behaviour (the curriculum text says Fell marks "join the Work Plan as jobs when you open it"), so it is clear which reconcile semantics are new.
- **m8 — Vague acceptance terms.** Stage B "believable single corridor"; S-06 "intuitive navigation", "readable map"; R-042 "meaningful economic choices"; S-15 "credible continuing-cover condition", "functioning three-species management exposure"; Stage E "practical playability"; R-053 "appropriate substantive spacing"; §9 "useful history". Bind each one either to a criterion set via OPEN-C2/C5/C6, or to an explicitly labelled **owner playtest judgement**.
- **m9 — Uncited approved spec.** S-09 cites D-073 L1–L4 but not the APPROVED `specs/scenario-one-multiyear-decision-outcome-feedback-v2` (on the #413 branch). Cite it as the controlling Annual Review contract.
- **m10 — Uncited cross-scenario novelty rule.** OPEN-C3 (authored older Oak/Beech cohorts) should cite D-081's novelty accounting. O2 is the chapter defined around a three-species **irregular-cohort** world, so authored compact O1 cohorts could weaken O2's identity. Flag this for the owner; do not decide it.

## Answers to the packet's mandatory questions

1. **Product source and priority — PASS, with M3 on the authority for "three".** MUST items trace to the user decision, D-059–D-081 or labelled proposals. D-082 is explicitly "proposed canonical wording, not yet published" (spec header; confirmed: main's Decision Log runs D-050–D-081, with no D-082). D-065/D-071/D-076 are preserved for the old world (§1, S-03, S-20). OPEN-1–6 remain undecided (§8). DRAFT status appears on all four files, and no implementation authority is claimed.
2. **Genuine new compact world — PASS.** S-05/§1 require a traversable standing area on both sides of the corridor, rule out a marteloscope or Legacy40 identity, invent no numbers, and make the pre-code fixture/route/economy study mandatory (S-21.2, plan Phase 0, C0-03). See M1 and M7.
3. **Save and compatibility — NEEDS CHANGES (B1, B2, B3, M1, M2).** The intent is correct: a disjoint slot, pre-mutation selection, both-direction negatives, every protected class listed, and D-078–D-081 coexistence without claiming the work has shipped. The verified hidden assumptions are: best-effort future load, warn-only scenario mismatch, global PlayerPrefs, the binary geometry ternary, and text-pattern Reference hashing.
4. **Real Objective 1 and manual Objective 2 — PASS, with m6 and m7.** OPEN-C1 is genuinely open. Marks are reversible and exclusive and kept separate from Pending/Approved/Completed (S-08). A spray never fells (S-06/S-08, R-028/R-038). MFM is gated off compact with no phantom racks and no weakened tests (S-20, R-035/R-036; source confirms `TeachingRackCount = 5` and the "exactly 5" refusal at `ScenarioOneRackSampleGeometry.cs:27,182`). Every eligible tree is in scope (S-07, R-030). "Eligible" should cite its source rule.
5. **Economy / forestry — PASS, with m1 and M5.** €2,500 and €12,000 are flagged as existing gameplay calibration that needs a new candidate study (S-11, OPEN-C5). Species depth, juveniles, regeneration, browsing, deadwood and habitat are preserved with no invented Irish coefficients (S-12, R-043–R-050).
6. **Real time / review / completion — NEEDS CHANGES (M3, M4).** L1–L4 (S-09), the D-074 spacing intent (S-13), the D-075 dual advance and the D-077 4-year/5-tick limit with stops (S-14), three substantive cycles, rack/maintenance excluded from credit, acknowledgement only after viewing, and no inheritance of the old two-cycle predicate are all present. The classifier and compact teaching-complete eligibility are undefined. No fixed forestry calendar is presented as universal ("3–6-year … guidance, not hard practice rules").
7. **Visual / map — PASS, with M6 and m8.** Current crowns and the conditional opening are kept separate from forecasts (S-17, R-067/R-068). The DRAFT covers MFM glyph reuse ban (R-069), 720p/1080p, colour-independent cues, LOD, collision and performance budget "selected from measurement" (S-19, OPEN-C6), and no unapproved packages (S-18).
8. **Practicality and traceability — NEEDS CHANGES (M7, m2–m4, m8).** The crosswalk is below. Tier-3 runs once, only at Stage F (plan Stage F; Stage A says "not full Tier 3"), so there is no unnecessary whole-world repetition. Stop rules and ownership are present (plan "Ownership, dependencies and stop rules"; tasks "Risk escalation").
9. **Source-state honesty — PASS.** §2 and the handoff disclose the #443 404 and claim no feature-branch inspection. The reviewer observed that the GitHub ref `feature/scenario-one-objective1-ux-repair-20261112-v1` exists at `acb4e111…` (commit dated 2026-11-13T11:06Z, 609 files different from main). It was **not audited**, is not proof of current local implementation, and the reviewer cannot tell which file path returned 404 to #454. A fresh owned-source audit through #424 is still required (OPEN-C7, R-003).

## Old-world negative compatibility table

| Protected class | Current main mechanism (verified) | DRAFT coverage | Gap / required negative |
| --- | --- | --- | --- |
| `forest-save.json` + `.tmp` + `.bak` | Single fixed slot, `ForestSaveController.cs:9-19` | S-02, R-012, R-020 | Covered. Add: legacy bytes unchanged across compact new/save/load/reject/crash. |
| v1–v9 (no scenario state) | `ScenarioOneManager.cs:1473-1478` starts the management layer fresh | S-03, R-021 | Compact must never accept missing identity (S-01 covers this). Add an explicit fixture. |
| ≤v19 / missing geometry → Legacy40 | `StandGeometryModel.ForSave` | S-03, R-021 | **M1**: a new geometry ID must not reach the 40 m default. |
| v20–v24 Enlarged80 | Stated `standGeometryModel` | S-03, R-021 | Covered. |
| Future-version legacy save | Loads **best-effort** (`ForestSaveController.cs:346`) | S-02 (ambiguous) | **B1**: legacy behaviour unchanged unless save owner decides; compact fails closed. |
| `scenarioId` mismatch | **Warning only, after first mutation** (`ScenarioOneManager.cs:1481`) | Not stated | **B1**: compact guard must run pre-mutation. |
| Title Continue | `CanLoad` against the single slot | R-013 (partial) | **B1**: Continue scoped to the profile. |
| Rack V2 / five-rack IDs (v21) | `ScenarioOneManager.cs:1487-1491` | R-024, S-20 | Covered. |
| MFM prescriptions (v23) | Validated in `TryReadSave`; identity-neutral when default | R-024, R-036 | Covered (m4 wording). |
| v24 first-guided-review latch | `saveVersion >= 24 && …` (`:1484`); stripped from identity | S-03, R-025 | Covered. **M4**: compact needs its own teaching-complete signal. |
| Frozen Reference Future v1 (`scenario-one-v12`) | Text-pattern identity stripping (`ScenarioReferenceArchive.cs:265-284`) | S-03, R-022 | **M2**: schema append vs. identity neutrality. |
| Pure-Sitka model-0 anchors | Stored growth model per save | R-023 | Covered. |
| PlayerPrefs learning/help progress | Device-wide `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*` | **Not covered** | **B3**. |
| Proposed O2 slot (#436) | DRAFT only, not shipped | R-016, S-04 | **B2**. |

**Save-owner sign-off: REQUIRED.** S-03, S-21.3, OPEN-C4 and plan Stage A already say so, which is correct. B1–B3, M1 and M2 must be inside that sign-off's scope.

## Preserve vs defer matrix

| Item | Compact first demo | Old Enlarged80 / historical | DRAFT status |
| --- | --- | --- | --- |
| Sitka/Oak/Beech parity (D-059/061/062) | Preserve | Preserve | ✓ S-12, R-043–R-046 |
| Annual ecology, regeneration, browsing, deadwood, habitat | Preserve (same engine) | Preserve | ✓ S-12, R-046–R-050 |
| Work Plan, economy | Preserve engine; new calibration config | Unchanged | ✓ S-10/S-11 (M5, m1) |
| S1-C review/history, D-073 L1–L4 | Preserve | Preserve | ✓ S-09 (m9) |
| S1-D repeated interventions, ≥3 substantive cycles | Preserve; new predicate | Old two-cycle predicate unchanged (D-077 P-1) | ✓ S-13/S-15 (M3) |
| D-074 spacing | Preserve; criterion OPEN | Not retroactive | ✓ intent (M3) |
| D-075/D-077 dual advance | Preserve if in the approved slice | Per cadence spec | ✓ S-14 (M4) |
| Five-rack Rack V2 lesson (D-064/071/072) | Deferred for compact only | Preserved | ✓ §1, S-20 |
| MFM V1 / D-065 / D-076 / #439 / #448 | Deferred for compact only | Preserved; tests intact | ✓ §1, R-035/R-036 |
| OPEN-1–OPEN-6 | Undecided | Undecided | ✓ §8 |
| Old saves v1–v24, Legacy40, Enlarged80 | Never loaded as compact | Preserved | ✓ intent (B1, B3, M1) |
| Frozen Reference Future v1 | Untouched | Preserved | ✓ intent (M2) |
| v24 latch | Not imported | Preserved | ✓ S-03, R-025 |
| D-078–D-081 O1–O8 independence | Compact is O1, not O2 | — | ✓ S-04 (B2, m10) |
| Personal playtest → explicit Windows-build gate | Preserve | — | ✓ S-21.5, R-079 |

## OPEN items — dispositions (none decided by the reviewer)

| ID | Disposition | Safe recommendation only |
| --- | --- | --- |
| OPEN-C1 | Remains **user** decision | DRAFT's conditional recommendation is acceptable. Show the user both options with C0-03 evidence. Note that the choice changes the compact teaching-complete signal (M4). |
| OPEN-C2 | Remains open until C0-03 evidence and user acceptance | Give C0-03 a real offline-harness lane (M7); ≥2 candidates; no 40 m alias (M1). |
| OPEN-C3 | Remains **owner** decision | Add D-081 novelty consideration (m10). |
| OPEN-C4 | Remains **save-owner** decision | Reconcile with #436 first (B2); include PlayerPrefs namespace (B3) and the schema-vs-identity choice (M2). |
| OPEN-C5 | Remains open; evidence + owner | Expand to the substantive-intervention classifier, depending on D-074 DRAFT MD-02/MD-03 (M3). |
| OPEN-C6 | Remains open; native evidence | Fix the overlay input-set term (M6); deferring the overlay is an acceptable fallback. |
| OPEN-C7 | Remains open; #424 handoff | Add approved cadence design + isolated candidate E as inputs (M4). |
| Suggested additions | Not decided | (a) Approved-order edit policy (m7); (b) overlay input set (M6) if not folded into C6. |
| OPEN-1 – OPEN-6 | Remain **undecided** | No compact action needed. The compact router/IDs should not prevent later multi-rack use; keep that a note, not a requirement. |

## Crosswalk: 82 requirements → spec → stage/task

All 82 IDs (R-001–R-082) were confirmed unique and **unchecked**; there are zero `[x]`.

| Checklist range | Spec anchor | Plan stage / tasks | Reviewer note |
| --- | --- | --- | --- |
| A R-001–R-010 | S-05, S-06, S-11, S-21; header; §8 | Phase 0; C0-01–C0-04 | m2 (R-002), m3 (R-010/R-082) |
| B R-011–R-025 | S-01–S-04, S-22 | Stage A; A-01, A-02 | **Add** items for B1 (pre-mutation placement, legacy future-version unchanged, profile `CanLoad`), B3 (PlayerPrefs both directions), M1 (geometry fail-closed), M2 (identity hash before/after) |
| C R-026–R-036 | S-05–S-08, S-20 | Stages B–C; B-01, B-02, C-01 | m4 (R-036), m6, m7 |
| D R-037–R-050 | S-08, S-10–S-12 | Stages C–D; C-01, C-02, D-01, D-02 | M5 (R-039) |
| E R-051–R-064 | S-09, S-13–S-15 | Stage D; D-03, D-04 | **Add** classifier-approved item (M3) and compact teaching-complete item (M4); S-15 "versioned criteria policy" has no explicit item |
| F R-065–R-075 | S-16–S-19 | Stages C, E; C-02, C-03, E-01, E-02 | M6 (R-067/R-068) |
| G R-076–R-082 | S-21, S-22 | Stage F; F-01, F-02 | Tier-3 once only ✓ |

## Evidence: what was inspected and checked

**Inputs read in full:** the four DRAFT files and the #454 handoff at `0d857afa`; the #425 user-decision memo; the #453 feasibility report; the #425 #453 verdict; the binding review packet. The GitHub diff of `0d857afa` against the merge base `6c13a9e1` shows exactly five added Markdown files (344 insertions). `main` has not moved since that base.

**Main source checked read-only at `6c13a9e1`:** `StandGeometryModel.cs`, `ForestStartingStand.cs`, `ForestSaveController.cs` (lines 236–360), `ForestSaveData.cs`, `ScenarioOneDefinition.cs`, `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneCompletionRules.cs`, `ScenarioOneManager.cs` (lines 600–614, 1395–1495), `ScenarioReferenceArchive.cs` (lines 262–290), `LearningObjectivesView(.Curriculum).cs`, `MenuHelpView.cs`, `WorkPlanView.cs`, `WorkPlanOverview.cs`, `ScenarioOneEconomyAdapter.cs`. Also targeted `git grep` for geometry ternaries, PlayerPrefs, scenarioId, minimum harvest, residual-BA terms, mark types and save fixtures.

**Confirmed accurate in DRAFT §2:** Legacy40/Enlarged80 only, 5 m cells, 64/256 cells; 336 + 1,008 trees, about 2,100/ha; `forest-save.json`/`.tmp`/`.bak`; schema v24; `scenarioId = "scenario-one"`; a pre-mutation validation stage exists; €12,000 starting cash; €2,500 minimum (but per visit — m1); year-25 minimum and year-100 review; `TeachingRackCount = 5` with the third rack as sample; the two-cycle completion predicate; the v24 latch. **Missing from §2:** B1, B3, M1, M2.

**Authority and related specs read:** main `Docs/Project/decision-log.md` (D-060, D-065, D-066, D-071, D-073–D-078, D-081; D-082 absent); `current-milestone.md` (MFM-08/09/10 status lines); the `specs/` status lines on main; the #436 O2 save-isolation DRAFT (`50496929`); the #413 D-074/D-076 DRAFT programme (`61f6fb09`) and its Manager review; cadence spec status on `docs/manager-cadence-review-disposition-20261112-v1`. Live `active-tasks.md` was read through the GitHub API (content SHA `5dda6a8c…`), and live #424 comments were read. **Neither was modified.**

**Method:** GitHub API, plus read-only `git show`/`git grep` on objects fetched into the user's existing clean `main` clone. `git fetch` updated only remote-tracking refs; there was no checkout, branch creation or file change in that clone. The single write went through the GitHub Contents API.

**Not run / not assessed:** no Unity/Editor, compilation, .NET tests, old-save fixture replay, candidate geometry generation, economic traces, rendered 720p/1080p or performance work; no save/PlayerPrefs access; no physical lease. No audit of the #443 feature branch contents (owned by #424). No complete trace of Work Plan fields (M5 is limited to a targeted search). No check of external Teagasc/Coillte sources beyond D-074's citation. Dashboard/Drive mirrors were **NOT VERIFIED**.

## Change accounting and handoff

- **Only file written:** `Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/CLAUDE-ONE-RACK-454-INDEPENDENT-DRAFT-REVIEW-20261113.md` on `docs/compact-one-rack-independent-review-20261113-v1`. The fetch-back and branch comparison are recorded in the #457 handoff comment.
- **Not touched:** the authoring branch, `main`, approved specs, the Decision Log, the milestone, `active-tasks.md`/#424, source, Unity, saves, Windows build.
- **Requested next action:** Overall Manager #425 to decide the disposition of this NEEDS CHANGES verdict, most likely a bounded #454 revision followed by a delta review. The Manager, not the reviewer, promotes the spec later. **User action now:** none required by this review. OPEN-C1 remains the next genuine user decision.
