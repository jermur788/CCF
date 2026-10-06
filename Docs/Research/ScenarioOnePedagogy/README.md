# Scenario One pedagogy, marteloscope and residual-stand research

**Branch:** `task/scenario-one-pedagogy-overnight` · **Base / context:** `3e4ee400af0673aae615317a2e2e1f548041c197` · **Status:** research and proposals for Manager review. **Not decision authority.** No production file (`Assets/ForestPrototype/**`), save schema, ecology, economy or Reference Future v1 was modified.

Evidence classes used throughout: [REPO] inspected code at `3e4ee40`, [LOG] Unity verification log, [HARNESS] this work's read-only harness, [PRAC] European Pro Silva CCF Practice Synthesis (supporting practitioner evidence), [EMP] CCF Primary Literature Synthesis v1 (supporting empirical evidence), [INF] design inference, [ACC] accepted decision.

## Start here

1. `CurrentTutorialAudit.md` — what the build actually teaches, and where it fails.
2. `ProposedTutorialArc.md` — the recommended learning sequence.
3. `ResidualStandEvaluationPrototype.md` — harness evidence behind the key claims.
4. `DecisionMatrix.md` — what is accepted vs proposed vs needs a decision.
5. `ImplementationRoadmap.md` and `Packets/` — what to build next, in which order.

## Contents

| Workstream | Files |
|---|---|
| A — Current teaching audit | `CurrentTutorialAudit.md`, `ObjectiveGraph.md`, `BeginnerKnowledgeGap.md`, `MenuTeachingAudit.md`, `ForestryTerminologyAudit.md` |
| B — Learning architecture | `LearningOutcomes.md`, `ProposedTutorialArc.md`, `PositiveSelectionTeaching.md`, `RepeatedInterventionDesign.md`, `RegenerationTeaching.md` |
| C — Marteloscope training | `MarteloscopeConcept.md`, `TrainingStandSpecification.md`, `MarkingAssessmentFramework.md`, `NoviceErrorLibrary.md`, `LongTermTrainingFlow.md` |
| D — Residual-stand evaluation | `ResidualStandMetricAudit.md`, `ResidualStandEvaluationPrototype.md`, `Evidence/` (CSV/JSON + `SHA256SUMS.txt`); harness `Tools/Verification/ScenarioOnePedagogy/ResidualStandEvaluation.cs` |
| E — Forest Diary / monitoring | `ForestDiaryConcept.md`, `PermanentPlotDecision.md`, `AnnualReviewHistoryIntegration.md` |
| F — Baseline-red gates | `BaselineRedGateInvestigation.md`; harness fixes in `Tools/Verification/{ClearanceVerification,MenuTutorialVerification,ScenarioOneRemovalVerification}.cs` |
| G — Roadmap and decisions | `ImplementationRoadmap.md`, `DecisionMatrix.md`, `MarteloscopeDecision.md`, `BioTreeDecision.md`, `MonitoringDecision.md`, `ResidualStandDecision.md`, `Packets/` |
| Copy and context | `TutorialCopyBank.md`, `CanonicalUpdateProposal.md` |

## Re-baseline warning

All [HARNESS] numbers are for RNG model 1, regeneration model 1 and the growth model at `3e4ee40`. Sol's Sitka height/mortality work (unmerged at the time of writing) will change them. Re-run the harness twice after that integration. The qualitative findings are the regression criteria, not the exact values.
