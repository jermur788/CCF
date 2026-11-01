# MFM-12 — Integrated Qualification and Independent Review — Task Packet

**Claim:** #225  
**Parent:** #91 / #200 / #221  
**Authority:** `specs/multiscale-forestry-management-v1/` (`spec.md`, `tasks.md`, `checklists/requirements.md`)  
**Task:** MFM-12  
**Status:** **RELEASED — DO NOT START LOCALLY UNTIL #223/#224 ARE MANAGER-CLOSED AND THIS PACKET IS REPINNED TO THE EXACT POST-REPAIR PRODUCTION SHA**  
**Difficulty:** High — end-to-end cross-system Unity qualification, persistence, rendered UX, complete-loop witness and independent spec-conformance review.  
**Best-suited primary:** SOL/CODEX high-reasoning local qualification lane.  
**Independent reviewer:** separate cross-provider reviewer, preferably Claude Sonnet 5.5 or another capable reviewer independent of the qualification/fix threads.  
**Suggested branch:** `qual/mfm-12-integrated-v1`  
**Worktree:** dedicated clean lane assigned by #203.  
**User action required:** none.

## Repin block

Current production integration at publication is:

`3acdf9cb635cdca79606335801e97f9d7dcce8b0`

Before local start, #200 must replace this with the exact production candidate after accepted closure of:

- #223 — verification-debt repair; and
- #224 — Objectives-window overflow repair if reproduced.

Record the exact branch/worktree/starting HEAD in #225 at `[RUNNING]` preflight. Heavy Unity is serialized machine-wide.

## Goal

Qualify Multi-Scale Forestry Management V1 end-to-end on one exact integrated Scenario One candidate and obtain independent architecture/spec-conformance review.

Success token is not self-issued:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

## Required evidence map

Use `specs/multiscale-forestry-management-v1/checklists/requirements.md` as the binding checklist. For each applicable A–P item record exact evidence or inherited evidence with exact ancestry/unchanged-surface justification. Do not use a blanket “previously passed” statement where the relevant surface moved.

### Architecture and prerequisites

- individual tree/ecology-cell state remains authoritative; no independent stand-level biological state;
- Rack V2 persistent geometry/world identity/old-save policy remain qualified;
- sample and coverage derive from confirmed Rack V2 geometry, with no parallel temporary rack source.

### Teaching and sample/coverage

- Lessons 2–5 remain behaviorally intact;
- three-Crop-Tree sample remains teaching calibration, not stocking guidance;
- 8 m teaching spacing and 8 m Hegyi radius remain distinct;
- 30% first release and 40% cumulative basal-area safeguard remain intact;
- work outside the sample cannot back-credit original lessons;
- fixed sample geometry and rack-corridor exclusion are correct;
- Covered / Not covered / Excluded / Manual states and reasons are inspectable.

### Prescription/candidate/species honesty

- deterministic prescription derived from player-executed sample decisions;
- fit disagreements inspectable, no hidden quality/vigour/crown ranking;
- spacing-only outside-sample Crop Tree candidates;
- no candidate auto-confirms; 3D player judgement is required; manual alternative remains usable;
- quantitative prescription remains Sitka-adult-only; Oak/Beech cannot enter Sitka-only quantitative logic.

### Proposal / approval / ordinary work path

- proposals are not `TreeMarkType.Fell` and cannot leak into Work Plan before prescription approval;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains the commitment gate;
- contractor/landowner execution and ordinary economy rules remain authoritative;
- manual state wins conflicts and generated actions retain durable prescription provenance.

### Persistence / compatibility

- stable prescription IDs/version/source/coverage decisions/provenance persist as specified;
- missing old fields mean no prescription and do not fabricate migration;
- malformed/ambiguous new save data required by accepted hardening fails before mutation;
- pre-feature Enlarged80 saves remain valid;
- Legacy40 remains unaffected where protected;
- frozen Reference Future v1 bytes/semantics remain exact;
- save/load continuation is deterministic for the tested current path.

### Integrated UX

Rendered evidence at **1280×720 and 1920×1080** must demonstrate:

- sample, coverage, candidate, proposal, approved work and executed result are visually/semantically distinct;
- candidate/proposal visuals are distinct from Red Fell, Blue Crop Tree and Orange Rack marks;
- map coverage/reasons are readable;
- 3D candidate navigation/confirm/reject and proposal inspection work;
- required extrapolated-unit spot-check is meaningful;
- objective/history/review/work-plan surfaces do not critically overlap or clip;
- the #199 minimap/focus contracts remain green;
- guided-v2 is green after accepted #223/#224 work.

### Calibration / complete loop

Carry MFM-11 as accepted evidence but run enough exact-candidate integration coverage to detect regressions:

- per-unit 40% safeguard;
- wider path preserves achievable retained/continuous canopy;
- economy uses ordinary rules and remains viable;
- timber/deadwood and regeneration/light outputs remain coherent;
- Scenario One complete-loop completion remains reachable;
- actual extension-state MFM-03 calibration remains within accepted boundary or any movement is explained;
- post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED** unless a new explicitly authorised diagnostic actually establishes it. Do not silently upgrade the claim.

### History / provenance / repeated-cycle coexistence

- prescription/work-order provenance survives Pending → Work Plan approval → execution → Annual Review/history;
- history remains read-only with respect to current game state;
- MFM extension coexists with accepted S1-C review/history and S1-D repeated-intervention behavior across at least two management cycles.

## Required reruns from verification-debt closure

Do not accept inherited red/stale wiring. Confirm the final candidate has current results for at least:

- BroadleafTimberEconomyVerification — Legacy40 + Enlarged80;
- MixedDensityVerification;
- pruning verification;
- ThreeSpecies history suite;
- guided-v2;
- PARITY-07/Combined/Rack rendered regressions where #224 or later UI work overlaps them.

The accepted Beech R2 C-level limitation remains an explicit limitation; do not alter the `0.40` criterion merely to make all raw checks green.

## Full Scenario One witness

Produce one explicit complete-loop witness on the exact candidate showing:

`initial forest → assess/access → sample teaching → extend treatment → review/confirm player judgement → prescription approval → Work Plan approval → execute → advance → Annual Review/history → later intervention → adapt/repeat → completion`

Record intervention years, major stand/economy/ecology transitions, species introductions required by completion, and the final completion outcome/objectives.

## Independent review

The reviewer must inspect:

- exact candidate ancestry and production diff since the last accepted integration;
- MFM requirements checklist evidence map;
- raw/runtime/rendered evidence sufficient to validate key claims;
- persistence/compatibility and protected-anchor handling;
- player-judgement boundary and absence of hidden automation;
- unresolved limitations and whether any claimed PASS exceeds the evidence.

Verdict: `APPROVED / NEEDS CHANGES / REJECTED`.

## Stop conditions

Stop and return to #200 if:

- any production/spec defect is found;
- a repair would be needed inside this qualification claim;
- protected Reference/Legacy40/pure-Sitka anchors move unexpectedly;
- guided-v2 or required rendered gates remain red for a current product reason;
- old/malformed saves mutate world state before required rejection;
- player judgement can be bypassed or unapproved proposals reach committed work;
- complete-loop completion becomes unreachable on the accepted path.

Do not weaken tests, retune production or silently rebaseline.

## Handoff

Publish:

- `Docs/Handoffs/MFM-12-Integrated-Qualification/HANDOFF.md`
- `Docs/Handoffs/MFM-12-Integrated-Qualification/INDEPENDENT-REVIEW.md`

Include exact commands/results, gate counts, screenshots manifest, save/reference fingerprints, full-loop witness, checklist matrix, cleanup/lock receipt and exact unresolved limitations.

Copy/paste dispatch after Manager repin:

`Run MFM-12 claim #225 from Docs/Handoffs/MFM-12-Integrated-Qualification/TASK-PACKET.md only after #200 repins it to the exact post-#223/#224 production integration. Qualify the full Multi-Scale Forestry Management V1 checklist end-to-end: save/load and old saves, Rack-derived sample/coverage, candidate/proposal/player-judgement boundaries, Work Plan commitment, ordinary economy, provenance/history, rendered 720p/1080p UX, repeated-cycle coexistence and complete Scenario One completion. Carry MFM-11's post-treatment CI parity as NOT DEMONSTRATED unless separately proven. No production repairs in this claim. Obtain independent cross-provider review. No Windows build; #194 NO ACTION.`
