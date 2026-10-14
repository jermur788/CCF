# Next canonical update draft v1

**DRAFT ONLY — NOT PUBLISHED CANONICAL CONTEXT.** Exact base/local manager context `321791b8203bc709c57615fd4d6a19614d78db24`; published context `c1564b857972ee18917e8a461c457533a54cc3f1`. No files under Docs/Project edited. No decision number assigned. Manager/integrator must review factual status and explicitly authorise publication through the normal single-context generator/mirror process. Historical entries stay intact; proposed additions below are exact text, not automatically accepted updates. No Unity run by this queue.

## A. Proposed next Decision Log entry

Target: a new row/entry in `Docs/Project/decision-log.md`; number/date supplied by Manager at publication, not guessed here.

> **Scenario One two-cycle completion — Confirmed (Manager packet).** Scenario One completion requires the existing eight objectives unchanged, plus two resolved Work Plans in different simulation years. Each counted Work Plan must contain successfully resolved work. An Annual Review must be acknowledged after the first counted Work Plan and before the second. The second intervention is non-prescriptive: this rule does not mandate a particular species, treatment or work type. Automatic review opening, approval-only plans, empty plans, failed-only work and two plans in the same simulation year do not satisfy this rule. Existing Completed v20 saves remain Completed. This accepted rule is not yet implemented by the report-only branch157ea7d; a separately qualified completion candidate is required.

## B. Proposed persistence policy note

Target: current policy section of `Docs/Project/unity-project-overview.md` and cross-reference from completion decision; do not rewrite historical save migrations.

> **Save v20 chronological review extension.** Save schema remains20 for the proposed `AnnualReviewAcknowledged` management event. Append the new event as numeric9 after `CenturyReviewed=8`; preserve every existing persisted enum numeric value. Record actual acknowledgement using existing simulation year and monotonic management-event ID so acknowledgement after first work in the same year can be distinguished from acknowledgement before it. Persisted `annualReviewSeen` retains its existing historical/tutorial meaning and cannot establish review chronology. Old Completed v20 saves retain their recorded outcome/year/reason; old Active v20 saves receive no fabricated review event or inferred timing and must establish truthful new chronology. Old Failed saves retain their terminal outcome. Existing lists suffice structurally, but older consumers may ignore the new event or reject unsupported model/assortment values; schema-number equality is not a promise of forward behavioural equivalence. Production loader/JsonUtility and old/new chronology fixtures remain qualification gates. The extension is authorised direction, not implemented by this documentation.

## C. Proposed Oak/Beech growth note

Target: current multi-species growth status in Overview and milestone; supporting research index references only.

> **Oak/Beech ordinal growth.** N2 sourcec35e001 concludes Kernel A, the existing generic diameter/height kernel, can support coarse ordinal calibration. No precise empirical growth coefficient or Irish species growth equation is authorised. Beech crop-group quadratic-mean diameter and mean height from transferred trial evidence must not be substituted for Irish individual DBH or top height. The earlier doubled-height interpretation is rejected. Positive diameter response to release is supported directionally; a universal Beech-before-Oak canopy ranking and comparative lifespan of height increment remain deferred. Numerical site response stays1.0 in the calibration experiment; dormant site descriptors are not fitted multipliers. Job3 preregisters explicitly labelled [C] calibration search ranges; Job4 lab resultB finds bounded candidate families but weak identifiability. No candidate is “correct”, no production species parameters have changed, and actual juvenile/adult spatial feedback, timber timing and save/runtime qualification remain required.

## D. Proposed density status note

Target: current implementation status in Overview/milestone; preserve D-061 pure-Sitka requirement.

> **Mixed-density v2 implementation and qualification boundary.** Sourcea7a759ded087a824c1e8149e480ddf58ac0585eb implements composite pressure from an exact Sitka-only subset signal plus a tapered generic occupancy contribution. Pure-Sitka models retain the legacy exact route; no Oak/Beech maximum-density equation, DEC/DMC or mixture bonus is claimed. V2 is offline-qualified within its documented reduced fixtures: density91, composite1917, isolation checks, timber71/history70 and five killed mutations; approximate type-check has no new errors beyond fixed baselines. Reduced long-run results are not full-ecology equilibria. Unity U3 still must prove real anchors/tree IDs, near-pure/manual/natural transitions, actual canopy/Hegyi/regeneration long runs, save/load and finances. V2 is not qualified/accepted for integration until U3 passes. Growth Model2 is not adopted as the default by this note.

## E. Proposed UX resolution and qualification note

Target: current supported-demo resolution policy in milestone/Overview; a Decision Log entry may cross-reference this exact policy with Manager-assigned number.

> **External-demo resolution and UX evidence.** Minimum supported external-demo resolution is1280×720;1920×1080 is also a required qualification fixture.1024×768 is compatibility-only and its limitations must not silently redefine the minimum supported demo requirement. UX source84e881aa6d0e96c5752f09492d5db37d2cc8925e has source-specific qualification evidencecc54d6ac9db3d0c4ca900ea3659cb989ba31992a reportingB, qualified with small fixes. That worker’s Unity evidence does not qualify a later integrated full-loop candidate. The declared P3 presentation text-anchor disposition requires explicit Manager review; no ecological/reference anchor rebaseline follows. Fresh-device startup assertion and bounded layout follow-ups, integrated history/completion/inspection/physical input and native Windows qualification remain separate gates. Newer local549eae3 small-fix evidence exists but is outside this packet’s accepted source and must be pinned/reviewed separately.

## F. Proposed milestone/current-status paragraphs

Target: factual current status in `Docs/Project/current-milestone.md` and matching implementation summary in Overview. Replace stale “P0 not started” current-state wording only when this draft is deliberately published; retain historical records.

> **Current non-Unity readiness.** Three-species implementation foundations now exist on task branches: density v2 with inherited overnight timber/history, dormant neutral multidimensional site infrastructure, and UX controls/review/teaching updates. Oak/Beech growth has completed corrected evidence review, preregistration and a deterministic offline lab (classificationB), but production calibration is not implemented. Completion remains report/design-only at157ea7d; the newly authorised v20 event contract still needs an implementation candidate. Comparable modelling depth and the complete two-cycle loop remain pre-demo requirements under D-059/D-060. No integrated complete-demo candidate or fresh qualified Windows external-demo build exists.

> **Integration and verification.** A disposable non-Unity rehearsal at c1564b8 merged densitya7a759d, site4753984 and UX84e881a without textual conflicts. Offline suites and approximate type-check passed after relevant merges; all temporary integrated production states were discarded and no main merge occurred. This proves bounded source compatibility, not Unity import/runtime/render/input acceptance. The ordered integrated verification runbook and complete-demo acceptance matrix define remaining fail-fast gates: source/diff/type-check, deterministic offline, Unity Editor/U3/U4 and actual UX, Player/performance, fresh post-parity/full-loop Windows candidate and native Windows smoke. Site remains dormant; historical builds and source-specific qualification are not external-demo acceptance.

> **Manager critical path.** Dispatch density U3; approve a bounded ordinal production-calibration experiment; dispatch the v20 chronological completion implementation; explicitly review the UX small-fix source and P3 presentation-anchor disposition; then appoint a single integrator for the approved sources and full gate sequence. Do not tune growth to compensate for density defects, alter eight objectives, change default Model2 or rebaseline protected Sitka/reference anchors.

## Publication evidence references

These branches are separate source snapshots; a docs-only publication must deliberately include/link the evidence, not whole-merge feature Assets. Exact proposed research-index additions:

- Serial queue Job1: `audit/three-species-integration-readiness-v1` @ `26d2bf8dc75ca4f612ec8e10ab9303afa044919c`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job2: `audit/save-v20-compatibility-contract-v1` @ `f4d0a1cac5f2a41e357042df417bf10d3bc6c078`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job3: `research/oak-beech-growth-preregistration-v1` @ `60cfd822916325e618dd8c51267db10d5cac2412`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job4: `tools/oak-beech-growth-calibration-lab-v1` @ `c79f710c1ed96192d43ddbc07e816899de6a2b29`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job5: `audit/scenario-one-demo-acceptance-matrix-v1` @ `52dd59f0d5f7958ed5fc36f90a3f45f856ac38dc`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job6: `audit/scenario-one-integration-rehearsal-v1` @ `6739b8b2284ff6900ca88b2d453d36c06672c286`; supporting audit/experiment evidence, not production acceptance.
- Serial queue Job7: `audit/verification-runbook-v1` @ `5f88da28a6bfbd5239c30c8fac945e5deb449d6b`; supporting audit/experiment evidence, not production acceptance.

## Manager review before applying draft

Number/date new decision rows explicitly; retain old text as historical where relevant. Confirm accepted source identities, any newer completion/UX work and U3 status at publication time. Keep pending/unrun work labelled. Apply to canonical Git only by the assigned publisher, commit the complete context set, regenerate/read back mirrors at that one new context SHA and refresh attachments. This draft does not perform those actions and does not assert that Drive or project attachments changed.
