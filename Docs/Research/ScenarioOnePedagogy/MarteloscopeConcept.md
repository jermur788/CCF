# Marteloscope-style training — concept and reuse audit (Workstream C, C1)

**Status:** NEW PROPOSAL [INF]. Not accepted. Supporting evidence: Pro Silva marteloscope practice [PRAC §2.5, §11 (Ireland), §12 (Spain), §14.4 (France), Luxembourg].

## 1. What a marteloscope is (practitioner basis)

A marteloscope is a small, fully mapped training plot. Every tree is numbered and measured. Trainees mark the trees they would remove, and software or an instructor compares the consequences using economic, silvicultural and ecological criteria [PRAC §2.5, Luxembourg]. Irish Pro Silva tree-marker training uses this format to compare low, crown and graduated-density thinning [PRAC §11 Ireland]. The French *Forêt-Irrégulière-École* pairs it with permanent plots and revisits [PRAC §14.4].

The key properties to keep:

1. **The same stand for everyone**, so different choices can be compared.
2. **Immediate multi-criteria feedback**, never a single grade.
3. **Reset and retry.**
4. **(Game addition)** advance time and walk the consequence, which a real marteloscope cannot do.

## 2. Key finding: Scenario One already contains a marteloscope-sized stand

| Property | Real marteloscope (typical) | Scenario One Year 0 [REPO] |
|---|---|---|
| Area | often around 1 ha (varies) | 0.16 ha (40 × 40 m) |
| Trees | hundreds, all numbered | **336, all with persistent ids** (`P{row}{col}`) |
| Measured per tree | species, DBH, height, quality, habitat | species, DBH, height, crown radius, age, competition, volume, timber assortments, wind diagnostic |
| Deterministic? | fixed physical plot | **Yes.** The starting stand is generated from FNV hashes of slot ids ([REPO] `ForestStartingStand`), identical in every new game |
| Can reset? | n/a (marking is on paper) | **Yes, technically.** In-memory capture and `LoadData` restore are proven by existing harnesses, and by this work's `ResidualStandEvaluation` (with a two-frame yield) |

So **no new stand content is needed** for a first training stand. The Scenario One Year-0 stand already has the decision cases the training needs (see `TrainingStandSpecification.md`), except stem-form cases.

## 3. Reuse audit (C1) — existing systems that support a training stand

| System | Exists | Reuse for training | Gap |
|---|---|---|---|
| Tree Inspection | `TreeInspectionView` | Per-tree facts and "if felled now" estimate | Neighbour/competitor relationship not shown (derivable) |
| Crop Tree mark | `TreeMarkType.CropTree` | Positive selection | — |
| Fell mark | `TreeMarkType.Fell` | Proposed removals | — |
| Work Plan quote | `ScenarioOneManager.GetHarvestQuote` | Gross revenue, contractor cost, small-job minimum, net — **production figures**, as used by the harness | — |
| Timber value | `TimberYieldCalculator` | Assortments for removed and retained trees | Retained "value" is a notional roadside figure, not a market valuation |
| Competition | `ForestEcologyController.HegyiTerm`, `GetCompetitionIndex` | Crop-Tree release before/after | — |
| Light | `RecomputeCanopy` formula (also used by the marking forecast) | Per-cell light after marking | — |
| Wind diagnostic | `GetWindRisk` formula | Stability dimension | No windthrow consequence (D-044) |
| Seed source | `TreeSpeciesDefinition.Maturity` | Seed trees retained/removed | All trees are immature at Year 0 |
| DBH, crown, height | `ForestTree` | Structure, H/D | — |
| Stand Map | `StandMapView` | Locate marks and openings | Training overlay (e.g. "your removals") optional |
| Annual Review | `AnnualReviewView` | Long-term consequence | Comparison between treatments needs history (Packet 5) |
| Reset / reload | `ForestSaveController.CaptureData` / `LoadData` | Restore the training stand. **Must restore from a serialized copy and let a frame pass** before enumerating or capturing (finding during this work: `CaptureData` includes inactive, not-yet-destroyed trees) | A production "practice sandbox" flag (like Reference preview) |
| Sandbox precedent | Reference Future preview (`TryBeginReferencePreview` / `EndReferencePreview`): captures the world, loads another, blocks saving, restores on exit | **The best existing pattern for a practice mode** | Preview is view-only. Practice needs marking, approval and time advance inside the sandbox |
| Determinism | RNG models, deterministic ordering | Same marking → same immediate result; long-term repeat | Verify two-process (done in this work, see `ResidualStandEvaluationPrototype.md`) |

**No duplicate systems are needed.** A practice mode is mainly: (a) a sandbox session wrapper modelled on the Reference preview, (b) an assessment panel built from existing formulas, and (c) stored marking plans (id lists) kept outside the player's save.

## 4. Three forms considered

| Form | Description | Teaching value | Cost / risk |
|---|---|---|---|
| **A. Inside Scenario One's opening** | Year 0 of the player's real forest is the training stand. Marking is reviewed before approval; there is no reset | High relevance; no new mode | No retry. A mistake is real and permanent. Comparing plans requires mental bookkeeping |
| **B. Separate tutorial scenario** | A dedicated "Training plot" scenario with its own definition | Clean separation; can author special cases | Needs a scenario-selection front end that does not exist. Duplicates scenario setup |
| **C. Reusable practice mode (sandbox)** | From the Work Plan: "Practise on a copy". Captures the world, lets the player mark plan A/B/C, compares them, optionally simulates 5/20 years, then returns. Like Reference preview, no save is written | Highest: retry, compare, long-term, then apply what was learned to the real forest | Moderate: sandbox wrapper, plan store, assessment panel. Save/autosave must be blocked during practice (precedent exists) |

**Recommendation: C, built in two steps.**

1. **C1 — Year-0 training copy:** practice on the deterministic starting stand only. The teaching cases and error library can then reference fixed tree ids.
2. **C2 — "Practise on my forest now":** the same sandbox on a copy of the player's current forest. Useful before every later intervention, and directly supports repeated management.

Form A remains the *real* first intervention (S4–S7 in the arc). The sandbox is an optional rehearsal, recommended by the S4 stage text but never required. **PRODUCT DECISION REQUIRED** — `MarteloscopeDecision.md`.

## 5. Player flow (form C)

```
Work Plan › "Practise on a copy"
  → banner: PRACTICE COPY — nothing here changes your forest
  → mark Crop Trees and Fell (Plan A) → "Review plan"
  → assessment panel (MarkingAssessmentFramework.md)
  → "New plan" (resets to the copy) → Plan B → review → compare A vs B
  → optional: "See this plan in 5 / 20 years" → simulate in the copy → walk it
  → "Return to my forest" (world restored; optionally keep Plan X's marks as proposals in the real forest)
```

The last option — *carry marks back* — is a strong teaching bridge. It is still a proposal: the player must approve in the real Work Plan.

## 6. What the practice mode must never do

- Never write the save slot, or alter the real forest's history, cash or learning progress, other than recording that practice was used.
- Never present one plan as "correct" (`MarkingAssessmentFramework.md` §4).
- Never import practitioner thresholds as pass marks.
- Never claim consequences the simulation does not model (windthrow, harvest damage, stem form).
