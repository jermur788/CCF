# MULTI-SCALE FORESTRY MANAGEMENT V1 — Requirements Checklist

Use this checklist for implementation handoffs, integration review and final qualification.

## A. Architecture

- [ ] A1 Three conceptual decision scopes are represented without creating three required player modes.
- [ ] A2 Player-facing interaction remains Management/Map ↔ 3D Forest.
- [ ] A3 Individual tree/ecology-cell state remains the only authoritative biological simulation.
- [ ] A4 No independent stand-level biological state can drift from tree state.
- [ ] A5 Property-level machinery remains deferred in V1.

## B. Scenario One prerequisites

- [ ] B1 Rack V2 persistent geometry is integrated and verified.
- [ ] B2 Rack V2 world identity/old-save compatibility is verified.
- [ ] B3 Sample/coverage geometry derives from confirmed Rack V2 racks.
- [ ] B4 No parallel temporary authored-rack geometry drives the V1 slice.
- [ ] B5 Latest integrated Scenario One lineage was inspected before coding.

## C. Existing teaching preservation

- [ ] C1 Existing Lessons 2–5 remain behaviourally intact before the extension.
- [ ] C2 Existing three-Crop-Tree sample teaching remains sample calibration, not stand stocking guidance.
- [ ] C3 Teaching Crop Trees remain at least 8 m apart as a gameplay calibration.
- [ ] C4 8 m Hegyi cutoff is unchanged as a separate model constant.
- [ ] C5 Authoritative CompetitionShare implementation is reused.
- [ ] C6 30% first-release teaching requirement remains intact.
- [ ] C7 40% cumulative basal-area safeguard remains intact.
- [ ] C8 Work outside the original sample cannot falsely satisfy original lesson credit.
- [ ] C9 Unsupported DBH/dominance quality ranking is not reintroduced.

## D. Sample and coverage

- [ ] D1 Existing teaching footprint around the representative teaching rack is the V1 source sample.
- [ ] D2 Teaching rack is selected deterministically from confirmed Rack V2 geometry.
- [ ] D3 Sample uses the qualified fixed 9.0 m half-width and 12.0 m half-length, with live Unity confirmation.
- [ ] D4 Rack corridor is excluded from treatment area using actual confirmed rack geometry.
- [ ] D5 Coverage units are distinct from the fixed teaching footprint and meet without silent overlap.
- [ ] D6 Edge remainders are explicit.
- [ ] D7 Coverage is based on observable/inspectable present state only.
- [ ] D8 Covered / Not covered / Excluded / Manual states are explicit.
- [ ] D9 Every not-covered/excluded state has a player-readable reason.
- [ ] D10 Known materially different density/competition areas are not silently treated as representative.
- [ ] D11 Coverage tolerances are recorded as GAMEPLAY CALIBRATION unless evidence supports stronger status.
- [ ] D12 No slope/wet-ground/hidden condition class is invented without authoritative state.

## E. Prescription derivation

- [ ] E1 Prescription is derived from the player's executed sample decisions.
- [ ] E2 Prescription uses a closed, inspectable vocabulary.
- [ ] E3 Prescription derivation is deterministic.
- [ ] E4 Fit check replays the rule against the source sample.
- [ ] E5 Fit disagreements are inspectable, not hidden behind one score.
- [ ] E6 Prescription never claims to be more correct than the player's demonstrated judgement.
- [ ] E7 Unsupported quality/crown/vigour/etc. fields are absent.
- [ ] E8 Material future rule changes require a version strategy.

## F. Crop Tree candidates

- [ ] F1 Outside-sample candidates are spacing-only suggestions.
- [ ] F2 Candidate spacing is an explicitly qualified parameter separate from both 8 m teaching spacing and 8 m Hegyi radius.
- [ ] F3 Candidate-generation logic is deterministic.
- [ ] F4 Candidate wording does not claim hidden quality judgement.
- [ ] F5 No candidate becomes a Crop Tree automatically.
- [ ] F6 Every candidate is confirmed or rejected by the player.
- [ ] F7 Confirmation is grounded in 3D inspection.
- [ ] F8 Candidate queue/navigation supports efficient individual review without bulk confirmation.
- [ ] F9 Player may manually choose a different eligible tree.
- [ ] F10 Manual designation overrides candidate suggestion.
- [ ] F11 Candidate workload is measured and user-tested; excessive tedium triggers a product decision rather than silent automation.

## G. Species honesty

- [ ] G1 First quantitative V1 prescription is Sitka-adult-only unless an approved amendment says otherwise.
- [ ] G2 Oak/Beech are not processed with Sitka-only quantitative assumptions.
- [ ] G3 Broadleaf exclusions are visible and accurately explained.
- [ ] G4 No species-aware preference is claimed merely because identifiers exist.
- [ ] G5 Any future broadleaf extrapolation has its own accepted rule/version evidence.

## H. Proposal state

- [ ] H1 Prescription proposals are not TreeMarkType.Fell.
- [ ] H2 Proposals are not saved as though manually marked by the player.
- [ ] H3 Opening the Work Plan does not import unapproved proposals.
- [ ] H4 Approving the Work Plan does not commit unapproved proposals.
- [ ] H5 Player manual state takes precedence over proposals.
- [ ] H6 Proposal conflicts are surfaced/reconciled rather than silently overwritten.

## I. Approval and work execution

- [ ] I1 Prescription approval is distinct from Work Plan approval.
- [ ] I2 Prescription approval creates pending ordinary work only.
- [ ] I3 Work is not auto-approved/executed by prescription approval.
- [ ] I4 Existing contractor/landowner execution path remains authoritative.
- [ ] I5 Existing economy/cash/minimum-cost rules remain active.
- [ ] I6 Generated work carries durable prescription provenance.

## J. Explainability

- [ ] J1 Each generated action can identify prescription ID/version.
- [ ] J2 Each generated action can identify source sample/rack.
- [ ] J3 Each generated action can identify coverage unit.
- [ ] J4 Each generated action has a closed-vocabulary reason.
- [ ] J5 "Why proposed/marked?" uses only observable/inspectable state.
- [ ] J6 Explanations do not imply hidden biological quality.
- [ ] J7 Player can inspect disagreements and uncovered areas.

## K. Persistence and compatibility

- [ ] K1 Feature uses the next available save schema after Rack V2.
- [ ] K2 Old/missing fields mean no prescription state.
- [ ] K3 Existing marks/orders are not migrated into fabricated prescriptions.
- [ ] K4 Stable prescription IDs persist.
- [ ] K5 Approval/history/provenance required by the spec persists.
- [ ] K6 Transient proposal visuals are recomputed where safe rather than redundantly persisted.
- [ ] K7 Legacy40 remains unaffected.
- [ ] K8 Pre-feature Enlarged80 saves remain valid without fabricated prescriptions.
- [ ] K9 Frozen Reference Future is unaffected.
- [ ] K10 Malformed new save data fails safely before world mutation.

## L. UX

- [ ] L1 Sample, coverage, candidate, proposal, approved work and executed result are visually/semantically distinct.
- [ ] L2 Candidate/proposal states are distinct from Red Fell, Blue Crop Tree and Orange Rack marks.
- [ ] L3 Map shows coverage and reasons clearly.
- [ ] L4 3D supports candidate confirmation and proposal inspection.
- [ ] L5 Teaching requires at least one extrapolated-unit 3D spot-check before prescription approval.
- [ ] L6 Manual management remains available and understandable.
- [ ] L7 Existing saved teaching step IDs are not renamed/reused.
- [ ] L8 New extension uses new stable step IDs.

## M. Calibration and safety

- [ ] M1 Each generated unit respects the accepted basal-area ceiling.
- [ ] M2 Whole-stand wider treatment has been measured, not assumed safe.
- [ ] M3 Retained/continuous canopy objectives remain achievable.
- [ ] M4 Scenario One economy remains viable under the default extension path.
- [ ] M5 Timber/deadwood outcomes remain coherent.
- [ ] M6 Long-run Scenario One completion remains achievable.
- [ ] M7 Protected hashes/fingerprints are preserved or deliberately versioned; never silently rebaselined.
- [ ] M8 Tests are updated for accepted behaviour without weakening safety intent.

## N. Teaching / scope

- [ ] N1 Extension begins after existing Lesson 5.
- [ ] N2 "Extend your treatment" is required before external/beginner Scenario One testing.
- [ ] N3 The extension is not falsely declared to satisfy all S1-D requirements without explicit S1-D verification.
- [ ] N4 Full manual management remains possible.
- [ ] N5 External/beginner testing waits for integrated qualification and user playtest.
- [ ] N6 No Windows build occurs without user playtest and explicit authorisation under the project rule.

## O. Evidence and wording

- [ ] O1 Player-facing framing uses the idea of applying the demonstrated sample treatment to comparable areas; `delegated marking` is internal shorthand, not required player-facing terminology.
- [ ] O2 The simulator's exact staged workflow is not presented as an official or universal Irish/UK forestry procedure.
- [ ] O3 `Operator selection` is not presented as an established Irish technical term, and contractor marking crews are not claimed as routine practice.
- [ ] O4 Evidence-backed analogues (sample/control areas, sample racks, feller-select, comparable conditions, inspection/conformance checking) are distinguished from the game's design abstraction.
- [ ] O5 Coverage/calibration numbers remain labelled GAMEPLAY CALIBRATION unless separately evidenced.
- [ ] O6 Candidate copy says spacing identifies where to inspect; 3D player confirmation supplies the qualitative tree judgement.
- [ ] O7 Model derivations are distinguished from direct evidence.
- [ ] O8 Broadleaf/Sitka transfer claims are not manufactured.
- [ ] O9 UI/copy does not imply unrestricted machine-operator crown-release judgement where visibility/operational evidence does not support it.

## P. Final gate

- [ ] P1 All applicable requirements above are evidenced.
- [ ] P2 Integrated full Scenario One qualification is green.
- [ ] P3 Rendered visual review passes.
- [ ] P4 Independent architecture/spec-conformance review passes.
- [ ] P5 Dashboard/coordination publication reflects the integrated state.
- [ ] P6 User personally playtests the current integrated candidate.

**Final implementation verdict:**

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

This verdict does not itself authorise a Windows build or external testing.
