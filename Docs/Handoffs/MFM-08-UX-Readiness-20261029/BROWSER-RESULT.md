# MFM-08 Stand Map / 3D Review UX — Browser Readiness Result

**Claim:** #153  
**Mode:** browser/GitHub only  
**Result:** **READY AS A CONTRACT; PRODUCTION BLOCKED**  
**Readiness token:** `MFM-08 UX CONTRACT READY — IMPLEMENTATION BLOCKED ON APPROVED #129 LINEAGE`

## 1. Verdict

The MFM-08 player-facing surface is sufficiently specified to dispatch as a bounded implementation task **after** the MFM-08A coverage-world adapter (#129) is Unity-qualified, Manager-approved and integrated into the current Scenario One player-test lineage.

The current #129 local candidate is not an implementation base for MFM-08. #129 is RUNNING but its Unity/qualification stage is blocked by the machine storage floor. Its latest worker record names local candidate `aff488a5`, with focused offline gates green; that candidate remains unpushed/unreviewed implementation evidence until the #129 handoff and Manager gate complete.

A blocked child implementation claim has been created as **#156**. It owns no production files until the #129 gate and a fresh live-ownership refresh release it.

No C#, Unity, save/schema, calibration, Windows build or production UI was changed by this browser task.

## 2. Live state refreshed

The following state was re-read from GitHub during #153:

| Claim | Refreshed state | MFM-08 consequence |
|---|---|---|
| #91 | RUNNING — Multi-Scale Forestry Management / player-test orchestration parent | Parent remains open. |
| #106 | RUNNING — local Scenario One player-test execution umbrella | MFM-08 eventually runs as a bounded child, not directly on the umbrella. |
| #121 | HANDED-OFF / closed — MFM-07 proposal and prescription-approval boundary | Accepted implementation input; no active writer. |
| #125 | INTEGRATED / VERIFIED — MFM-07 integrated at `c8cfae8c4025cec6e960a050edf4bfde2a981de5` | This is the last exact integrated MFM production base MFM-08 may inspect today. |
| #129 | RUNNING — Unity blocked by storage floor | **Hard production dependency.** Current local candidate is not Manager-approved/integrated. |
| #147 | RUNNING — S1-C prior-treatment guidance | Sole current bounded history/review presentation writer. MFM-08 must not take Annual Review/history ownership or race a shared UI-root seam. |
| #151 | BLOCKED ON #147 — S1-D previous-vs-proposed presentation | No ownership yet. Later owns generic Work Plan readiness/history presentation only, not MFM teaching/review UX. |
| #153 | browser readiness task | This result closes the browser design/API/ownership work. |
| #156 | BLOCKED ON #129 — MFM-08 implementation | Prepared child claim; no production ownership until repinned/released. |

### Coordination discrepancy found

Live `Docs/Project/coordination/active-tasks.md` still described #129 as blocked on the earlier source-authority review. That is stale relative to #129 itself: Manager source-authority dispositions are already recorded, #129 resumed, and the worker reached local `aff488a5` before the storage floor prevented Unity qualification. Coordination must therefore describe #129 as the current adapter owner with Unity blocked by storage, while still treating the candidate as unapproved/unintegrated.

The still-open #146 browser issue is no longer a semantic production blocker because its outstanding ambiguity was directly dispositioned on #129. Administrative closure of #146 is separate from this MFM-08 UX task and does not promote #129 implementation evidence.

## 3. Authority preserved

This contract follows the approved `specs/multiscale-forestry-management-v1/` requirements:

- a demonstrated sample treatment may be applied to **comparable** rack-served areas;
- coverage must expose `Covered`, `Not Covered`, `Excluded` and `Manual` states with readable reasons;
- spacing-only Crop Tree suggestions require player judgement and individual 3D confirmation;
- proposals are not Fell marks;
- at least one extrapolated unit must be spot-checked in 3D before prescription approval;
- prescription approval and Work Plan approval are separate gates;
- MFM-09, not MFM-08, owns the `Extend your treatment` lesson/curriculum and prescription-specific Work Plan teaching.

Player-facing wording must use **“apply your demonstrated/sample treatment to comparable areas”** or equivalent. Do not present `operator selection` as a formal Irish technical term.

## 4. Exact MFM-08 player flow

MFM-08 is a **review and judgement surface**, not a new teaching sequence. It is entered through the existing Stand Map UI as a bounded treatment-review mode/layer/tool, not through a new permanent global shortcut.

### 4.1 Open treatment review

When the production coverage read is valid and the demonstrated sample can be derived, the Stand Map offers a treatment-review surface. If the read is invalid, the surface remains non-destructive and shows the production-provided problem; it must not silently fall back to an empty-mask or UI-reconstructed coverage model.

The review surface shows, at the same time where legible:

1. the **source sample** that the player actually demonstrated;
2. the current coverage classification for extrapolated units;
3. spacing-only Crop Tree candidate state;
4. current prescription proposals, once a stored Accepted prescription exists;
5. the player's review progress, including the mandatory 3D spot-check gate.

The source sample is not another `Covered` target. It is labelled separately as **“Sample — demonstrated treatment”** (or equivalent concise wording).

### 4.2 Coverage classification versus player choice

`ScenarioOneCoverage.State.Covered` is an algorithmic comparability result. It must **never** be presented as already selected, approved or committed.

Player-facing meanings:

| Core state | Player-facing meaning | Allowed action |
|---|---|---|
| `Covered` | **Comparable — available to include** | Player may explicitly choose “Apply sample treatment here” or leave it manual. |
| `NotCovered` | **Not comparable** | Cannot be included by V1 extrapolation; show the reason(s). |
| `Excluded` | **Excluded from extrapolation** | Cannot be included; show the authoritative exclusion reason(s). |
| `Manual` | **Manual management** | Remains individual/manual; show whether this is player choice or an uncovered remainder. |

A Covered unit therefore has two distinct concepts on screen: **classification** (“Comparable”) and **draft player choice** (“Include” / “Leave manual”). Do not encode both with one colour or one status label.

Before a prescription snapshot is stored, Include/Manual choices are presentation draft state only. No tree mark, work order or saved world result is created by toggling them.

### 4.3 Coverage reasons and flags

The UI consumes the closed `ScenarioOneCoverage` vocabulary and maps it to plain language. At minimum:

- `SourceSample` → “Demonstrated sample treatment”;
- `PlayerChoseManual` → “You chose to manage this area manually”;
- `RemainderLeftForManualManagement` → “Remainder — manual management”;
- `PlantedJuvenile` → “Existing planted juvenile(s)”;
- `Shelter` → “Existing tree protection/shelter”;
- `ClearancePatch` → “Existing vegetation-treatment/clearance patch”;
- `DeadwoodRecord` → “Retained/fallen deadwood”;
- `ActiveManagementWork` / open-work reasons → “Existing management work”;
- `PlayerFellMark` → “Existing player Fell mark”;
- `NonSitkaAdult` → “Contains non-Sitka adult tree(s) — V1 extrapolation is Sitka-only”;
- `TooFewStems` → “Too few comparable stems”;
- `DensityDiffers` → “Stand density differs from the sample”;
- `MeanDbhDiffers` → “Mean tree size differs from the sample”;
- `MeanCompetitionDiffers` → “Competition differs from the sample”.

`NaturalRegeneration` and `PlayerCropTrees` are **flags/evidence**, not automatic exclusion labels; show them as informational badges/notes where useful.

`ProtectedArea` must not be fabricated by MFM-08. Under the #129 Manager disposition, current Scenario One supplies no live point producer for this reason; if the final production coverage read fails closed because a new authoritative protected polygon appears, MFM-08 displays that coverage-read problem and stops rather than sampling or inventing geometry.

### 4.4 Draft area decisions

For each `Covered` extrapolated unit, the side card provides explicit draft choice controls:

- **Apply sample treatment here**;
- **Leave manual**.

The map also exposes `Not Covered`, `Excluded` and remainder/manual units for inspection, but does not offer an invalid Include action.

The draft must remain reversible until the player asks to prepare a prescription snapshot. No direct Fell/Crop/Rack mark changes occur from area selection.

### 4.5 Spacing-only Crop Tree candidates

Candidate presentation is explicitly an **inspection prompt**, not a quality recommendation. Required copy is equivalent to:

> “Spacing identifies where to inspect. Judge the tree in 3D before confirming it as a Crop Tree.”

Map behaviour:

- show candidate position/order and current state (`Open`, `Confirmed`, `Rejected`);
- provide `Next` / `Previous` candidate navigation using the existing MFM-05 non-wrapping queue contract;
- provide **Inspect in forest** navigation;
- do not confirm a Crop Tree on the map;
- no bulk confirmation.

3D behaviour:

- navigation leads the player to the candidate location/tree without teleporting them;
- the normal tree-inspection surface identifies that this tree is a **spacing candidate**, not yet an authoritative Crop Tree unless already confirmed;
- confirmation uses the existing authoritative Crop Tree designation path so the normal Crop Tree mark becomes the truth;
- rejection is review-state input to the candidate batch, not a second persistent tree mark;
- once confirmed/rejected, the batch remains stable for review/navigation as required by MFM-05.

MFM-08 must not add a new persisted candidate-decision schema. If rejection state needs to survive map close/reopen during the same live review, keep it in presentation/session state. A save/load may regenerate the current candidate review from authoritative world state; no schema change is authorised here.

### 4.6 Prepare an Accepted prescription snapshot

Once the player has chosen target units and made the relevant individual Crop Tree judgements, the UI may call the existing MFM-07 `CreatePrescription(...)` boundary. Player-facing copy should say **“Prepare prescription for review”** / **“Save these coverage choices for review”**, not “approve work”.

This stores an `Accepted` prescription snapshot only. It creates no proposal mark, no work order and no Work Plan approval.

The v23 prescription record is append-only and the current API has no amend/delete operation. MFM-08 therefore must not invent one. If the player returns and changes area choices after previewing a stored snapshot, prepare a **new** Accepted snapshot from the revised choices; the prior unapproved snapshot remains inert and must never be auto-approved. If product requirements later demand explicit durable supersession/deletion, stop and return that as a separate save/schema decision.

### 4.7 Proposal review

For the selected Accepted prescription, use the read-only MFM-07 preview. Each proposal must show:

- target tree;
- coverage unit;
- confirmed Crop Tree it serves;
- the closed reason: **high-share competitor around a confirmed Crop Tree**;
- useful quantitative explanation already present in the proposal (for example competition share / basal area) where it improves judgement;
- any surfaced conflict/hold.

Every proposal is labelled **“PROPOSAL — not a Fell mark and not yet work”** or equivalent.

A proposal target that already has a player mark/order or lies in a unit no longer covered is surfaced as a conflict/hold from the production preview. MFM-08 does not overwrite player state or hide the conflict.

### 4.8 Required extrapolated-unit 3D spot-check

Before **Approve prescription** can be enabled, the player must complete at least one 3D spot-check in an extrapolated target unit represented by the current prescription.

The bounded V1 interaction contract is:

1. choose a target unit on the Stand Map;
2. select **Inspect in forest**;
3. use the existing map/waypoint and tree-inspection conventions to walk there;
4. inspect at least one relevant candidate/proposal/tree in that unit in 3D;
5. record that unit as spot-checked in presentation/session state;
6. return to the review surface, where the gate reads **“3D spot-check complete”**.

The spot-check latch is **not saved** by MFM-08 and is invalidated when the active review prescription/coverage snapshot is replaced or when the session is reloaded. This keeps the requirement honest without creating new save/schema state.

A map click, map zoom/focus, or merely setting a waypoint does **not** satisfy the spot-check. The player must actually reach/inspect in 3D.

### 4.9 Prescription approval and Work Plan separation

Only after the active prescription is valid, its proposals have been reviewed, and the required 3D spot-check is complete does MFM-08 expose **Approve prescription**.

That action calls the existing MFM-07 approval boundary. The result is ordinary **Pending** work with prescription provenance. Required confirmation text is equivalent to:

> “Prescription approved. Pending tasks were added to the Work Plan. They are not approved for execution yet.”

MFM-08 does not approve/fund/execute the Work Plan. MFM-09 later teaches how prescription-generated pending work appears in the Work Plan; #151 separately owns only generic repeated-intervention readiness grouping/history semantics after #147 releases it.

## 5. Exact read-only API dependencies

### 5.1 #129 — production coverage read

MFM-08 may consume **only the final integrated `ScenarioOneManager.ReadCoverage` façade** from #129 for current production coverage classification.

Contract already Manager-approved on #129:

- side-effect-free and read-only;
- calls the same authoritative world-read + `ScenarioOneCoverage.Classify` path used by production prescription approval;
- no cache;
- no persistence;
- no hidden fallback;
- no mutable state exposure;
- no alternate coverage semantics.

MFM-08 must **not** rebuild masks/features in UI, call a separate `ScenarioOneCoverage.Classify` world adapter, duplicate starting-stand radii or read the moving player as a Site mask.

The local #129 method signature cannot be treated as final GitHub authority until the candidate is pushed and integrated. The implementation packet therefore pins the **named façade and semantics**, then requires a fresh exact-signature inspection at release. If the final integrated lineage does not expose that façade, STOP; the smallest missing API is precisely one thin `ReadCoverage` accessor with the semantics above, not another adapter.

### 5.2 MFM-04 core read model

Read the returned `ScenarioOneCoverage.Result` and its immutable/plain-data fields:

- `Valid`, `Problem`;
- source/sample identity and metrics;
- `Areas` / `Remainders`;
- each area `Id`, `RackId`, `Segment`, `IsSource`, `State`, `Reasons`, `Flags`, `StemIds`, `Evidence`;
- comparison/fit state where useful to explain `Not Covered`;
- `Find(areaId)` and deterministic signature only for verification/debugging, not as UI state.

### 5.3 MFM-05 candidate read/navigation

Use the existing `ScenarioOneCropTreeCandidates` contract rather than inventing a second candidate selector:

- regenerate the deterministic candidate batch from current authoritative state;
- `TryGetInspectionTarget(...)` for an Open candidate;
- `TryMoveInspectionTarget(..., Previous/Next, ...)` for non-wrapping queue navigation;
- `InspectionTarget.TreeId`, `AreaId`, world `X/Z`, order and spacing-only reason;
- `Decision` state for Open/Confirmed/Rejected review presentation.

Confirmed truth remains the normal authoritative Crop Tree mark/state. No bulk confirm and no hidden quality ranking.

### 5.4 MFM-07 prescription/proposal boundary

Use existing manager boundaries:

- `FindPrescription(int)` — read current stored snapshot;
- `CreatePrescription(...)` — store explicit Accepted coverage choices only;
- `PreviewPrescription(int)` — read-only current proposal/conflict set;
- `CheckPrescriptionLink(...)` — read-only current validity of one proposal link where needed;
- `ApprovePrescription(int)` — final prescription approval, creating ordinary Pending work only.

Proposal state is recomputed plain data and is never a tree mark, order or saved transient visual.

### 5.5 Existing map / 3D presentation hooks

Prefer existing navigation and inspection conventions:

- Stand Map square selection and `SetWaypointCell(...)` / waypoint readback for navigation;
- no teleportation;
- normal `TreeInspectionView` for authoritative tree facts and contextual review text;
- existing `InspectedTreeIndicator` neutral selection grammar (ground ring + trunk outline) for the currently inspected tree, rather than management paint/symbols.

If exact-tree navigation needs a small presentation-only bridge from the MFM-05 target's `X/Z` to the existing waypoint cell, implement that in the MFM-08 UI layer. It must not become persisted world state.

## 6. Visual and semantic separation contract

Accepted authority requires candidate/proposal review visuals to be clearly distinct from Fell, Crop Tree and Rack marks. It does **not** specify a final new MFM colour palette, so this result does not invent one.

### 6.1 What MFM review visuals may use

Use **review/selection grammar**:

- map area outline/fill/hatching with a text legend for coverage states;
- hollow/outlined map glyphs for candidates and proposals;
- distinct glyph shape/pattern for candidate versus proposal;
- selected-item emphasis using the existing neutral selection convention;
- 3D navigation via the existing waypoint/inspection selection treatment;
- explicit textual chips such as `CANDIDATE`, `PROPOSAL`, `CONFIRMED CROP TREE`, `CONFLICT`.

### 6.2 What they must not use

Until authoritative state actually changes, do not use:

- the Fell-mark world symbol/paint grammar for a proposal;
- the Crop Tree trunk-band/mark grammar for an unconfirmed candidate;
- Rack V2 physical entrance marking or rack-line grammar for MFM areas/candidates/proposals;
- a colour-only distinction with identical shape/paint treatment.

Current implementation already distinguishes ordinary selection from management marks by form: the selected-tree indicator is a neutral ground ring + trunk outline, while management marks use their own symbol/band grammar. MFM-08 should extend that principle rather than create another trunk-paint language.

### 6.3 State transition rule

- Open candidate → review glyph only.
- Confirmed candidate → authoritative existing Crop Tree mark becomes truth; candidate review status updates accordingly.
- Rejected candidate → no management mark is created.
- Proposal → review glyph only, never Fell mark.
- Prescription approved → proposal preview is no longer treated as a pending visual instruction; ordinary Pending work/provenance is authoritative for later Work Plan presentation.

Final hue, exact icon art and hatch pattern are bounded implementation design choices, provided rendered qualification proves the semantic distinctions at both required resolutions and does not collide with existing mark grammar.

## 7. Ownership boundaries

### S1-C / #147

MFM-08 does **not** own:

- `AnnualReviewView.cs`;
- Management History text/history navigation;
- prior-treatment guidance;
- historical year selection or historical location inference.

#147 remains the bounded history/review presentation writer until it hands off. MFM-08 should avoid `ScenarioOneUiRoot.cs`; if it proves unavoidable, STOP until the current owner releases it or Manager assigns an explicit non-overlapping seam.

### S1-D T-05 / #151

MFM-08 does **not** own generic Work Plan grouping such as Approved / Pending-new / Pending-carried-over, nor previous-vs-proposed repeated-intervention history presentation. #151 owns that later, after #147 releases the seam.

MFM-08 may state only the local truth needed at prescription approval: generated work is Pending and still needs Work Plan approval.

### MFM-09

MFM-09 retains:

- `Extend your treatment` lesson/curriculum placement after Lesson 5;
- stable learning IDs and teaching progression;
- prescription-specific Work Plan teaching/grouping/explanation;
- beginner flow that sequences sample → coverage → candidates → proposals → 3D spot-check → approval.

MFM-08 builds the usable review controls/surfaces MFM-09 will later teach; it does not add that curriculum itself.

### Save / schema / simulation

MFM-08 owns no save/schema, validation, world-hash, Reference Future, coverage calibration, density/growth/simulation, MFM-08A world-adapter or Windows-build surface.

## 8. Prepared implementation task

- **Child claim:** #156 — `[BLOCKED ON #129] MFM-08 — Stand Map / 3D review UX`
- **Packet:** `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TASK-PACKET.md`
- **Planned branch:** `feature/mfm-08-map-3d-review-ux-v1`
- **Planned worktree:** `/media/jer/Videos/CCF-MFM08`
- **Start SHA:** deliberately unset until exact #129 Manager-approved/integrated lineage exists.
- **Difficulty:** 4/5 — substantial player-facing map/3D UX with cross-surface interaction and rendered verification, but no new simulation/save architecture.
- **Best-suited model/provider:** local SOL/Codex high-reasoning Unity/UI lane or equivalent capable local Unity worker.
- **Handoff:** `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`
- **Implementation success token:** `MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW`

Copy/paste dispatch **after** the blocker is cleared and the packet is repinned:

`Run claim #156 from Docs/Handoffs/MFM-08-Map-3D-Review-UX/TASK-PACKET.md on the exact Manager-approved/integrated #129 lineage; implement and Tier-2 qualify the bounded MFM-08 Stand Map/3D review UX only.`

## 9. Production release conditions

#156 stays BLOCKED until all are true:

1. #129 exact candidate is pushed;
2. its required Unity/adapter/MFM-07/MFM-06 regressions pass;
3. Manager verdict is APPROVED;
4. #129 is integrated into the current Scenario One player-test umbrella;
5. #156 packet is repinned to that exact integrated SHA;
6. live coordination shows no overlapping UI ownership;
7. any required shared seam with #147 is released or explicitly API-bounded.

Until then, **no production MFM-08 implementation should begin**.

## 10. User action

None for this browser result. The next executable action is the existing #129 qualification/review/integration path once machine storage permits it. #156 is already prepared and intentionally blocked.