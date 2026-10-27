# MULTI-SCALE FORESTRY MANAGEMENT V1 — Specification

- **Spec ID:** `MULTI-SCALE-FORESTRY-MANAGEMENT-V1`
- **Feature ID:** `MULTI-SCALE-FORESTRY-MANAGEMENT-V1`
- **Status:** APPROVED — user label: `APPROVED FOR IMPLEMENTATION PLANNING`; production coding remains gated by the prerequisites below
- **Owner:** Overall Manager (specification supplied by the user)
- **Created:** 2026-10-27
- **Accepted:** 2026-10-27 (D-065)
- **Last updated:** 2026-10-27
- **Decision log references:** D-065; D-010 (amended in part); D-064 (Rack V2 prerequisite)
- **Milestone:** Scenario One — see `Docs/Project/current-milestone.md`
- **Related evidence:** `Docs/Project/research-index.md` — delegated treatment / sample-to-comparable-area evidence (D-065); `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — Rack V2 prerequisite
- **Supersedes / superseded by:** none
- **Primary Scenario One slice:** representative rack-served sample → transparent treatment prescription → coverage/proposals → player confirmation/review → Work Plan
- **Product scope:** Scenario One Enlarged80 only for V1
- **External-test status:** required before beginner/external Scenario One testing

## 1. Purpose

CCF needs to teach real tree-level forestry judgement without requiring the player to repeat the same manual decision across every comparable tree in an increasingly large forest.

V1 introduces a multi-scale management architecture in which the player demonstrates forestry judgement in a real 3D sample, the simulator derives a transparent treatment prescription from that demonstrated work, and the player reviews where that prescription can and cannot reasonably apply before any wider work is committed.

The architecture preserves one authoritative biological simulation. Property-level and stand-level views summarize and manage the same individual-tree and ecology-cell world; they do not create a second authoritative stand model.

## 2. Accepted architecture

### 2.1 Decision scopes

Use three conceptual scopes:

1. **Property / strategic** — which stands, sequencing, budgets, long-term priorities. Deferred in V1 because Scenario One is effectively one stand.
2. **Stand / operational** — treatment prescription, coverage, exclusions, proposed wider work, review and approval.
3. **Sample / tree-level** — the player demonstrates actual forestry judgement on real trees in 3D.

### 2.2 Player-facing environments

V1 retains two player-facing environments:

- **Management / map** for stand-scale coverage, proposal review and Work Plan context.
- **3D forest** for sample work, Crop Tree confirmation, spot-checking and physical inspection.

Do not add a third navigation mode merely to represent the conceptual property/stand/sample hierarchy.

### 2.3 Authoritative state

The existing tree/cell simulation remains authoritative.

Do not introduce a second stand-level biological state that independently stores or advances stocking, basal area, regeneration or growth. Stand-level values are derived summaries and management state only.

## 3. Product decisions incorporated

### U1 — Spatial judgement principle

**ACCEPTED.** Important forestry judgement is learned and demonstrated in the 3D forest. That demonstrated judgement may be converted into a transparent stand-level prescription, reviewed spatially and applied to comparable areas.

This amends the universal interpretation of the earlier rule that all spatial decisions are made while walking. It does not eliminate physical 3D judgement.

### U2 — In-world framing

**ACCEPTED.** Wider application is framed in-game as **applying the player's demonstrated sample treatment to comparable areas**, integrated with the existing work architecture. `Delegated marking` may remain internal design shorthand, but it is not preferred player-facing terminology.

The gameplay chain is:

`walk/sample → demonstrate judgement → derive prescription → review coverage/proposals → approve prescription → Work Plan → execution → authoritative world result`

**Evidence status:** the delegated-treatment research is complete. Primary-source spot checks support marked sample/control areas guiding work in comparable forest, sample racks in Irish conifer first thinning, UK feller-select/example-area practice, and post-work inspection/conformance checking. The simulator's exact staged sequence remains an **evidence-informed game abstraction**, not a documented standard Irish workflow. Do not present `operator selection` as an established Irish technical term, do not claim that contractors routinely become the formal marking crew, and do not imply unrestricted operator discretion or universal manager sign-off.

### U3 — Timing

**ACCEPTED.** The Scenario One representative-management slice must be implemented and qualified before beginner/external Scenario One testing.

Current authorised Rack V2 and other prerequisite work continues while this spec is implemented.

### U4 — Manual alternative

**ACCEPTED.** Full manual management remains available. The player may manually manage additional units or the whole applicable stand rather than use wider prescription application.

### U5 — Observable-information rule

**ACCEPTED.** Sample/coverage suggestions may use only present-state information the player can reasonably observe or inspect. They must not reveal hidden future outcomes, concealed quality variables or privileged model truth.

### P1 — Scenario One insertion point

**ACCEPTED.** The extension occurs **after the current Lesson 5 review**, preserving the qualified Lessons 2–5 sample workflow.

### P2 — Crop Trees outside the sample

**ACCEPTED: option B.** The system may propose **spacing-only Crop Tree candidates** outside the sample, but every candidate requires player confirmation before prescription-generated release work can be proposed around it.

The system must not claim that a spacing-only candidate is a good-quality Crop Tree. Real forestry guidance identifies quality trees by inspecting characteristics such as stem form, branching/vigour and suitability within the stand; V1 does not encode those qualitative judgements as an automatic selector. Spacing therefore proposes **where to inspect**, and 3D confirmation is where the player makes the Crop Tree judgement. Suggested wording and UI semantics must communicate that it is a candidate for inspection, not an expert quality judgement.

### P3 — Rack V2 sequencing

**ACCEPTED.** The Scenario One slice derives its sample and coverage units from persisted, confirmed Rack V2 geometry. Do not build a parallel temporary rack geometry model.

### P4 — Required status

**ACCEPTED.** The extension is a required Scenario One teaching step before external/beginner testing. It may contribute to the S1-D second-intervention architecture, but it satisfies S1-D only when S1-D's own acceptance requirements are explicitly verified.

## 4. Scenario One sample authority

### 4.1 Sample unit

The V1 sample is the existing qualified **teaching footprint around the representative teaching rack**, not a newly invented inter-rack bay and not the whole rack-served coverage strip.

MFM-01 qualified the sample as stable under valid Rack V2 geometry when the footprint uses a fixed teaching envelope tied to the player's confirmed rack plan. For Enlarged80 V2 worlds with a confirmed five-rack plan:

- the **teaching rack** is the third rack when the five confirmed racks are ordered by perpendicular offset along the plan's common normal;
- the rack axis is the confirmed centreline direction;
- the along-rack anchor is the point on that centreline closest to stand centre `(0,0)`, clamped so the ±12 m teaching length remains on the confirmed segment; do not use the segment midpoint;
- the footprint extends **12.0 m** each way along the rack from that anchor;
- the rack corridor itself is excluded;
- the teaching footprint extends to a fixed **9.0 m half-width** on each side of the rack centreline;
- neighbouring rack positions do not resize the teaching footprint;
- access exclusion is computed from the actual confirmed rack corridor geometry, not historical scalar authored lines.

`TeachingHalfWidthM = 9.0` is **GAMEPLAY CALIBRATION** qualified offline by MFM-01 and still requires live Unity confirmation during implementation qualification. `HalfLengthM = 12.0` remains the existing gameplay calibration. Historical ~69-tree / 1.29490 m² counts are reference-layout fixtures, not protected product requirements.

The sample's existing three along-rack zones and both-side teaching structure remain intact.

### 4.2 Rack-served coverage units

Coverage units are distinct from the fixed teaching footprint. Each confirmed Rack V2 rack serves the strip extending halfway toward its neighbouring racks, with the rack corridor itself excluded from treatment. The stand is partitioned into comparable rack-served strip units/segments for coverage analysis.

Exact segmentation and edge treatment remain implementation/calibration details to be qualified from the final Rack V2 geometry before production logic is frozen. The fixed 9.0 m teaching half-width must not be silently reused as the coverage boundary.

The outer remainder beyond the outermost rack-served strips is not silently assigned to the nearest sample. It must be explicitly covered, excluded or left for manual management.

## 5. Protected Scenario One teaching mechanics

The following remain protected unless a later accepted decision explicitly changes them:

1. **Three Crop Trees in the qualified sample** remain a teaching calibration, not a stand stocking prescription.
2. **Teaching Crop Tree minimum spacing is 8 m** between stem centres. This is a gameplay/teaching calibration and is not a stand stocking prescription.
3. **8 m Hegyi competition neighbourhood** remains unchanged as a separate model constant. The fact that both values are currently 8 m does not make them one parameter.
4. The existing `CompetitionShare` calculation remains the authoritative Share computation. Do not duplicate or approximate it in the prescription system.
5. Existing near/middle/far sample zones and the qualified sample conflict-resolution behaviour remain sample teaching mechanics; they are not automatically extrapolated as stand rules.
6. The first-release teaching requirement remains at least the accepted 30% planned Share per Crop Tree for lesson credit.
7. The accepted 40% cumulative basal-area safeguard remains in the sample and becomes a hard per-unit ceiling for prescription-generated treatment unless a later accepted model changes it.
8. Work outside the qualified sample remains neutral to the original sample lesson credit.
9. No DBH-rank, dominance or hidden tree-quality criterion may be reintroduced as an automatic Crop Tree selector without evidence and a new product decision.
10. Existing Scenario One completion chronology and protected compatibility/reference behaviour remain unchanged unless this spec explicitly requires otherwise.

## 6. V1 player flow

The intended Scenario One sequence is:

1. **Rack V2 / access lesson** — plan racks, confirm, field-mark entrances, approve rack work, execute and review rack result.
2. **Existing sample Lesson 2** — choose three Crop Trees in the qualified teaching footprint.
3. **Existing sample Lesson 3** — mark competitors and satisfy release/safeguard teaching rules.
4. **Existing sample Lesson 4** — use the ordinary Work Plan, approve and execute the sample treatment.
5. **Existing sample Lesson 5** — inspect/review outcomes and make the current retain/release-further judgements.
6. **New lesson: Extend your treatment.**
   - derive a prescription from the player's executed sample decisions;
   - report how well the derived prescription reproduces the player's own sample decisions;
   - calculate which rack-served units are covered, not covered or excluded;
   - propose spacing-only Crop Tree candidates in covered Sitka-adult units;
   - require player confirmation/rejection of those candidates;
   - generate proposed competitor-release treatment around confirmed candidates;
   - require at least one 3D spot-check of extrapolated treatment;
   - allow revision, exclusion, additional manual work or another sample where needed;
   - approve the prescription;
   - convert approved proposals into pending ordinary work orders with prescription provenance.
7. **Ordinary Work Plan** — the existing global work-approval mechanism remains the commitment gate.
8. **Execution / advance year.**
9. **Review** — Annual Review plus at least one reinspection of an extrapolated unit.
10. Existing later Scenario One teaching continues, with stable saved step IDs even if visible numbering changes.

## 7. Prescription contract

### 7.1 The sample is evidence, not a template copy

The player's manual sample decisions remain exactly as made and remain ordinary world/work state.

V1 must not copy tree coordinates, tree IDs or geometric patterns from the sample into other units.

### 7.2 Closed, explainable vocabulary

The derived prescription uses a fixed, inspectable vocabulary. For the first Scenario One V1 slice it may include only mechanisms supported by the current model and accepted design:

- confirmed Crop Tree candidate spacing/placement rule;
- release target derived from the player's sample, subject to the accepted minimum teaching requirement;
- competitor ordering using the authoritative `CompetitionShare` within the existing 8 m neighbourhood;
- per-unit removal ceiling capped by the accepted 40% safeguard;
- current disposal/disposition rules only where supported by existing work/economy mechanics;
- explicit exclusions and coverage reasons;
- deterministic rule/version identity.

The prescription must not contain unsupported fields for stem quality, crown class, vigour, veteran-tree status, slope, wet-ground class, hidden windfirmness or other state that Scenario One does not currently model/observe.

### 7.3 Fit check

Before wider approval, apply the derived rule back to the original sample and show how closely it reproduces the player's own treatment.

At minimum the player must be able to see:

- agreement/disagreement count or equivalent clear summary;
- which sample decisions the rule would have made differently;
- relevant Share/removal consequences.

The fit check is an honesty device, not a score claiming forestry correctness.

### 7.4 Determinism

Given the same authoritative world state, rack plan, prescription version and confirmed Crop Trees, prescription derivation and proposal generation must be deterministic.

Any future material change to the selection algorithm requires an explicit model/rule version strategy rather than silently changing old saves.

## 8. Crop Tree candidate contract

### 8.1 Candidate generation

Outside the original sample, the system may suggest structurally plausible **spacing-only candidates**.

For Scenario One V1:

- candidate generation is restricted to eligible Sitka adult stems;
- **candidate Crop Tree spacing is its own parameter** and must be qualified separately; it must not silently reuse either the 8 m teaching Crop Tree minimum spacing or the 8 m Hegyi neighbourhood;
- MFM-01 workload measurements may inform calibration, but they do not establish a production candidate-spacing value;
- the candidate algorithm may use observable/inspectable current state only;
- it may not use unsupported quality/dominance inference.

### 8.2 Player confirmation

Every proposed Crop Tree candidate must be individually confirmed or rejected by the player before release treatment is generated around it.

Confirmation is a forestry judgement action. It must occur in, or be grounded by, 3D inspection; the map may navigate to or summarize candidates but must not turn spacing suggestion into automatic designation.

The implementation must provide efficient individual navigation (at minimum candidate queue plus Next/Previous and map focus or equivalent). Bulk confirmation is not permitted in V1. MFM-01 estimated a default-scale workload on the order of several dozen candidates; this remains a **user-experience qualification item**, not a reason to automate confirmation. If user testing shows the workload is materially tedious, stop for a product decision rather than silently introducing bulk approval.

### 8.3 Manual override

The player may designate a different eligible Crop Tree manually. Player manual designations override candidate suggestions.

### 8.4 MFM-01 qualification status

MFM-01 returned `SAMPLE STABLE UNDER VALID RACK-V2 GEOMETRY` using the fixed 9.0 m teaching half-width described above. It was an offline reconstruction, not live Unity verification. Implementation must confirm the geometry and teaching behaviour in Unity before the new fixture becomes authoritative.

The result also removes the need to constrain the suggested Rack V2 layout to 16 m middle gaps solely to preserve the historical teaching fixture. Rack V2 remains governed by its accepted 16–20 m spacing envelope and preferred ~18 m target; any independent access/geometry constraints remain in force.

## 9. Species scope

### 9.1 V1 quantitative treatment scope

The first prescription/extrapolation slice is quantitatively valid only for the currently supported Sitka adult competition/release behaviour.

### 9.2 Oak and Beech

Oak and Beech must be explicitly excluded from Sitka-calibrated automatic release proposals until species parity supplies accepted species-appropriate behaviour.

The system must not:

- treat Oak/Beech adults as Sitka equivalents;
- use Sitka-calibrated competition/release coefficients as though empirically valid for them;
- automatically remove broadleaf stems as competitors under the Sitka prescription;
- claim species-aware preference until that feature is genuinely implemented and verified.

When species parity later provides the required behaviour, extending the prescription vocabulary to multiple species requires a bounded spec amendment/version, not silent generalisation.

## 10. Coverage and representativeness

### 10.1 Principle

A sample represents **conditions**, not merely an area percentage.

This principle is evidence-informed: Irish control-plot guidance ties examples to homogeneous units, Irish CCF marking guidance subdivides stands by changing conditions, and UK guidance requires renewed familiarisation as stand conditions change. These sources support **recalibration when conditions differ**, not the simulator's exact thresholds or state machine.

No stand-wide proposal may assume the sample represents every rack-served unit.

### 10.2 Initial observable coverage measures

The first implementation may evaluate unit similarity using current, inspectable quantities such as:

- stem count/density;
- mean DBH;
- mean local competition index;
- species composition where currently represented;
- regeneration/juvenile presence;
- current marks/Crop Trees/open work;
- access/rack geometry.

Initial numerical tolerance candidates inherited from the abandoned Stage C design may be evaluated as **GAMEPLAY CALIBRATION**, but they are not automatically accepted evidence. The implementation task must qualify chosen tolerances against the current Scenario One stand before they become production constants.

### 10.3 Required exclusions/guards

Coverage logic must be able to exclude or flag, as applicable:

- rack corridors;
- materially different density/competition areas, including the known former-edge dense band where applicable;
- edge remainders outside rack-served strips;
- approach-track/work-clearing footprints;
- active regeneration or planted juvenile areas where treatment would conflict;
- shelters/clearance patches or other relevant management state;
- retained deadwood records;
- player Crop Trees/marks/open work that conflict with proposals;
- non-Sitka adult stems for the first quantitative slice.

Do not invent slope, wet-ground or other condition classes unless current authoritative state actually supports them.

### 10.4 Coverage states

Every unit is explicitly one of:

- **Covered** — sufficiently comparable for the current prescription;
- **Not covered** — materially different or outside calibrated tolerance;
- **Excluded** — protected/conflicting state means the prescription must not operate there;
- **Manual** — player has chosen to manage it manually.

The UI must explain why a unit is not covered or excluded. The existence of mapped/excluded treatment areas has a real administrative analogue in Irish guidance, but the game's four-state coverage model remains a design abstraction rather than a reproduced grant procedure.

## 11. Proposal state and precedence

### 11.1 Proposed treatment is not a Fell mark

Prescription-generated proposed removals must **not** be stored as ordinary `TreeMarkType.Fell` or any other state that the current Work Plan automatically imports.

Proposal state must remain distinct until prescription approval.

This prevents:

- accidental commitment through the global Work Plan;
- false provenance implying the player manually marked the tree;
- proposal leakage across save/load or unrelated workflow actions.

### 11.2 Player precedence

Player-authored state always takes precedence over extrapolated proposals.

If the player manually marks, designates, cancels or otherwise manages a tree/unit, the prescription must reconcile or flag the conflict; it may not silently overwrite player intent.

## 12. Approval architecture

V1 has two distinct approvals:

### Prescription approval

Accepts the stand-level proposed treatment and creates pending work orders with prescription provenance.

It does **not** execute work and does not replace the Work Plan.

### Work Plan approval

The existing Work Plan remains the commitment gate for pending work. Existing contractor/landowner execution architecture remains authoritative.

Opening or approving the Work Plan before prescription approval must not commit unapproved proposals.

## 13. Explainability and provenance

Every prescription-generated work item must be traceable to:

- prescription ID/version;
- source sample;
- coverage unit;
- confirmed Crop Tree where relevant;
- closed-vocabulary reason code;
- whether the final action originated manually or from an approved prescription.

Player-facing inspection must support a concise **Why proposed/marked?** explanation using only observable/inspectable state.

Examples of acceptable reason semantics:

- candidate suggested from spacing only — inspect stem form/branching/vigour and confirm;
- competitor contributes high Share around confirmed Crop Tree X;
- retained because outside 8 m neighbourhood;
- excluded because unit differs materially from sample;
- excluded because player management already exists;
- excluded because species parity does not support this tree.

Do not imply that the rule discovered hidden biological quality.

## 14. Persistence requirements

This feature changes persistent management state and therefore requires a save-schema change after the Rack V2 v21 foundation. Do not assume a version number until the implementation baseline is inspected; use the next available schema version.

Persist, at minimum:

- stable prescription ID allocator;
- prescription definition/version;
- source sample/rack identity and source year;
- derivation inputs necessary to explain historical treatment where not reconstructable safely;
- coverage decisions/exclusions that represent player choices;
- prescription approval state/year;
- confirmed candidate Crop Tree decisions where not already represented by durable Crop Tree state;
- provenance link on generated work orders and/or management events sufficient for history;
- any persistent review choice that is explicitly accepted as a prescription input.

Prefer to recompute purely preview/proposal visuals deterministically rather than persisting transient proposal markers. Persist approved work through ordinary work orders.

Old saves must not fabricate prescriptions. Missing fields mean no multi-scale prescription state.

Frozen Reference Future must remain unaffected.

## 15. Review / later intervention

The architecture must support future review without requiring that all S1-C/S1-D functionality be implemented in the first coding slice.

At minimum, history must be able eventually to answer:

- which prescription generated this work;
- which sample it came from;
- which units were covered/excluded;
- what fit was reported;
- what year it was approved/executed.

The first Scenario One slice must not claim full S1-D completion until the separate S1-D acceptance criteria are run.

If current Lesson 5 `[J]/[K]` review choices are later used to derive a second prescription, they must first become deliberate persisted inputs. Session-only review state must not silently affect durable prescriptions.

## 16. UX requirements

1. The map must distinguish sample, coverage state and proposals without confusing them with committed Red Fell marks.
2. 3D must allow inspection of proposed treatment and candidate Crop Trees before approval; candidate copy should make clear that spacing identifies where to inspect, while the player judges tree suitability.
3. At least one extrapolated unit must be spot-checked in 3D before prescription approval in the teaching flow.
4. The UI must make **candidate**, **proposal**, **approved work** and **executed result** visually/semantically distinct.
5. The player can navigate to disagreements, uncovered units and candidate trees.
6. The manual alternative remains available and understandable.
7. Existing stable saved teaching step IDs are not renamed/reused. Add new IDs for the extension.

## 17. Economy / execution requirements

Prescription approval creates pending work; normal Work Plan/economy logic determines cost and execution.

The feature must not bypass:

- contractor minimums;
- existing cash checks;
- timber/deadwood work semantics;
- ordinary approval/execution rules.

Before the new default flow is accepted, run a calibration-impact qualification of the wider intervention against:

- basal area and retained-canopy safeguards;
- Scenario One cash/economy;
- continuous-canopy objectives;
- long-run completion behaviour;
- existing protected anchors where applicable.

Do not assume that applying a sample-derived release across much of the stand is automatically safe merely because the sample itself is safe.

## 18. Compatibility and non-goals

### Required compatibility

- Legacy40 remains legacy.
- Pre-feature Enlarged80 saves load without fabricated prescriptions.
- Frozen Reference Future remains frozen.
- Rack V2 remains the rack geometry authority.
- Existing Red Fell / Blue Crop Tree semantics remain intact.
- Existing sample teaching mechanics remain intact until the extension begins after Lesson 5.

### Non-goals for V1

- property-level multi-stand gameplay;
- simulation level-of-detail for unvisited stands;
- automatic hidden-quality Crop Tree selection;
- broadleaf quantitative extrapolation before species parity;
- a marteloscope mechanic;
- manual physical contractor execution;
- arbitrary AI optimisation of the forest;
- changing the core growth/ecology model;
- changing Rack V2 geometry/editing rules;
- replacing the Work Plan.

## 19. Evidence classification

### DIRECTLY EVIDENCE-BACKED / existing model authority

Only mechanics already supported by accepted project evidence/model authority retain that label, including the existing competition calculation and any forestry evidence independently recorded in project research.

### MODEL DERIVATION

- applying the authoritative current-tree state to deterministic coverage/proposal calculations;
- deriving stand summaries from tree/cell state.

### GAMEPLAY CALIBRATION

- sample-to-unit coverage tolerance values;
- spacing-only candidate suggestion calibration;
- UI fit thresholds/presentation;
- required number/location of spot checks beyond the accepted minimum one;
- any prescription grouping thresholds not directly supported by evidence.

### ACCEPTED GAME DESIGN

- demonstrate judgement in a sample and scale through a transparent prescription;
- evidence-informed framing that the player's demonstrated sample treatment may be applied to comparable areas;
- mandatory player confirmation of spacing-only Crop Tree candidates;
- separate prescription approval and Work Plan approval;
- manual alternative;
- Scenario One extension after Lesson 5.

### UNSUPPORTED / PROHIBITED WITHOUT NEW DECISION

- automatic quality-based Crop Tree selection from DBH/dominance alone;
- treating Sitka coefficients as broadleaf evidence;
- presenting the simulator's exact sample → prescription → coverage → approval sequence as an official Irish forestry procedure;
- presenting `operator selection` as an established Irish technical term;
- claiming contractors routinely form a formal marking crew that reproduces the forester's sample;
- claiming unrestricted operator discretion, especially for crown-release assessment from a machine cab;
- hidden optimal-treatment AI.

## 19.1 Evidence-informed wording boundary

Safe forestry-informed framing for V1 includes:

- a marked sample/control area can communicate an intended treatment;
- one or more sample racks may guide operators in first thinning of conifers during CCF transformation;
- comparable/homogeneous conditions matter, and changing stand conditions can require reassessment;
- completed work can be inspected or checked against an approved plan.

Use these as **supporting analogues**, not as proof that the game's exact workflow is a statutory or universal professional procedure.

Additional implementation boundary: UK evidence shows that machine-cab visibility can make crown assessment unsuitable for feller-select in some transformation thinning. This supports the existing V1 choice that the system proposes treatment and the player reviews it; do not turn the mechanic into a claim that machine operators can always make equivalent crown-release judgements from the cab.

## 20. Prerequisites

Production implementation of the Scenario One slice requires:

1. Rack V2 persistent rack/world-identity foundation integrated and verified.
2. Confirmed Rack V2 geometry available as runtime authority for sample/coverage strips.
3. Safe save-schema sequencing after Rack V2.
4. Latest Scenario One production lineage inspected before coding.
5. No conflicting writer on shared Scenario One management/map/save files.

Species parity is not required merely to implement Sitka-only guarded core logic, but the external-demo candidate still remains subject to the project's accepted species-parity requirements.

## 21. Acceptance criteria

The V1 Scenario One slice is accepted only when all of the following are demonstrated:

1. Existing qualified Lessons 2–5 still behave as intended before the extension.
2. Sample geometry derives correctly from confirmed Rack V2 geometry.
3. Prescription derivation is deterministic and reuses authoritative `CompetitionShare` rather than duplicating it.
4. Fit check exposes disagreements with the player's sample.
5. Coverage explicitly distinguishes covered/not-covered/excluded/manual units.
6. Spacing-only Crop Tree candidates never become Crop Trees without player confirmation.
7. Candidate suggestions do not claim hidden quality judgement.
8. Oak/Beech are safely excluded from unsupported Sitka-only quantitative proposals.
9. Proposed removals are not ordinary Fell marks before prescription approval.
10. Opening/approving the Work Plan before prescription approval cannot commit proposals.
11. Prescription approval creates pending ordinary work with durable provenance.
12. Player manual management overrides prescription proposals safely.
13. At least one extrapolated unit is inspected in 3D in the teaching flow.
14. Per-unit and total treatment remain inside accepted safety/calibration bounds.
15. Save/load preserves accepted prescription state and does not fabricate state in old saves.
16. Reference/Legacy compatibility remains protected.
17. Map and 3D communicate sample, coverage, candidate, proposed, approved and executed states distinctly.
18. Integrated Scenario One completion remains achievable and consistent with D-060 external-demo scope.
19. Calibration impact on canopy/economy/long-run outcomes is explicitly qualified.
20. Independent review finds no hidden automation that replaces the player's core forestry judgement.

## 22. Stop conditions

Stop implementation and return to the Manager if:

- Rack V2 geometry/persistence cannot provide stable rack-served strips;
- the implementation would need an unsupported automatic tree-quality rule;
- applying the prescription requires weakening the 8 m/Hegyi or 40% protected safeguards;
- save compatibility would require silently migrating old worlds;
- wider treatment materially breaks Scenario One canopy/economy/completion and no bounded calibration can resolve it;
- species parity is required to proceed but is not available;
- the feature would require replacing the Work Plan or creating a second authoritative biological model;
- genuine product ambiguity emerges beyond the accepted decisions above.

