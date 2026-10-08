# Player-facing terminology audit and glossary (Workstream U)

**Status:** audit of strings at `a8596df` [REPO]. P1 changes are marked [P1-CAND]. Extends `ForestryTerminologyAudit.md` (pedagogy branch).

## 1. Findings

### 1.1 Raw species IDs shown to players

| Where | Shows | Fix |
|---|---|---|
| Forest objectives (`ScenarioOneObjectives`: "Establish planted " + speciesId) | "Establish planted sessile-oak" | Display name. Note: the label is also stored in the century review's serialized results; use display names **at render time**, not in saved data |
| Annual Review FOREST regeneration line (`ScenarioEcologyReviewLines`: `s.speciesId`) | "sitka-spruce 14, beech 2" | Display names |
| Century Review ("speciesId presence") | "beech presence" | Display names |
| Annual Review failed tasks (`{e.taskType}`) | "FellTree: …", "RemoveRegeneration: …" | Task display names |
| Work Plan thinning table (`Assortment.ToString()`) | "Pulp", "Sawlog" (enum names) | Acceptable words; add a one-time tooltip "timber product size class" |

### 1.2 Developer language

| Term on screen | Where | Problem | Canonical replacement |
|---|---|---|---|
| "cohort(s)" | Clearance preview "Affected now: n cohorts"; map help "tallest natural seedling cohort"; Annual Review "regeneration cohort(s) removed" | Statistical jargon; D-047: abundance is relative, not counted seedlings | "young-tree group(s)" (P1 does this in some places) |
| "juvenile(s)" | Review "Planted juveniles…", "Protected juveniles" | Jargon | "young trees" / "planted saplings" |
| "CI 6.1" | Tree card | Unexplained index | "Competition 6.1 (higher = more crowded)" plus the growth-withheld sentence (P2) |
| "opening 1.0 of 2" | Marking forecast | Internal calibration state | Remove; the pattern line replaces it (P3) |
| "wind peak 29.7 (high)" | Forecast | Saturated diagnostic | Remove until the storm stability bands exist |
| "PENDING approval", "APPROVED", "COMPLETED", "FAILED" | Work Plan status | Fine, but all caps | Sentence case |
| "Approve pending work" | Work Plan | Approves everything | "Approve all pending jobs (n)" |
| "Reference Future v1 (frozen)" | Work Plan | Developer framing | "Example future (fixed, not a target)" in the label; keep the internal name in docs |
| "planted cohort, promoted in year N" | Tree origin | Legacy wording | "planted (joined the canopy in year N)" |
| "joined the canopy layer" / "promoted" | Review | "Promoted" is code language | "grew into a tree" (game meaning: became an individual tree record) — needs care, see §1.4 |
| "(n/m²)" regeneration density | `RegenerationDiagnosis.Summary` | Implies a physical density; D-047 says abundance is relative | Only on the legacy IMGUI path (suppressed in Scenario One); remove or relabel "relative abundance" if ever shown |

### 1.3 Technical terms needing first-use explanation

| Term | First appears | Explained on main? | Proposed one-line explanation |
|---|---|---|---|
| CCF / continuous-cover forestry | Nowhere on main except the failure message | **No** ([P1-CAND] yes) | "Managing a forest by repeated, selective work that keeps tree cover while new trees grow." |
| DBH | Tree card | Yes (help) | Keep: "Trunk diameter at 1.3 m height" |
| Crop Tree | Prompt / card | Partly | "A tree you choose to keep and favour for future timber." |
| Competition / competitor | Tree card | Partly | "Neighbours that limit a tree's growth; bigger, closer ones matter most." |
| Basal area | (P3) | — | "Total trunk cross-section area per hectare; a measure of how much tree is standing." |
| Thinning / release | Work Plan | No | "Thinning: removing some trees so others have room. Release: thinning aimed at chosen trees." |
| Regeneration | Map, review | Partly | "Young trees growing from seed in the forest." |
| Enrichment planting | — | No | "Planting a few trees of species the forest lacks." |
| Deadwood | Lesson | Yes | Keep |
| Pruning lift / clear stem | Work Plan | Yes | Keep |
| Light band (low/moderate/high) | Ground report | Partly | "Light reaching the ground: what seedlings get." |
| Browsing | HUD | Partly | "Deer eating young shoots." |
| Small-job minimum | Work Plan | Partly | "In this scenario a contractor visit costs at least €2,500." |
| Stability (storms) | — | — | See `StormTeaching.md` |

### 1.4 Different names for the same concept

| Concept | Names in use | Canonical |
|---|---|---|
| Removing a tree | "Fell", "harvest" (mark message "Marked P0712 as harvest"), "thinning job", "Harvested stump", "managed opening" | Action: **Fell**. Operation: **thinning**. Result: **felled** (stump). Retire "harvest" in player text (keep "harvesting work" for the cost line) |
| The kept tree | "Crop Tree", "crop tree", "Retain as Crop Tree", "CROP", "Keep as Crop Tree" [P1] | **Crop Tree** (capitalised); verb "Keep as Crop Tree" |
| Young trees | seedlings, saplings, juveniles, cohorts, regeneration, young growth | **young trees** (general); **seedlings** (natural); **saplings** (planted stock); **regeneration** (the process / the population) |
| Clearance | "Clear competing vegetation", "area clearance", "Vegetation clearance", "regeneration cohort(s) removed", "Remove regeneration" (task type) | **vegetation clearance**; say explicitly that it also removes young trees in the area |
| Original trees | "original canopy trees" (objective counts all Sitka), "Old plantation Sitka still standing" (Century Review, P-prefixed only) | **plantation trees** = the trees present at Year 0; one definition everywhere |
| Objectives | "Forest objectives", "Learning objectives", "Objectives [O]" (opens learning) | **Scenario progress** (forest) vs **Lessons** (learning) until merged (`ObjectiveRedesign.md`) |

### 1.5 Terms that overclaim causal certainty

| Text | Problem | Replacement |
|---|---|---|
| "Clear competing vegetation" (preview header) | Implies vegetation competes with young trees in the model; on main it has no effect (D-047). Under M2 it does, conditionally | Main: "Vegetation clearance (removes plants and young trees in this area)". M2: per `RegenerationModel2Teaching.md` |
| "growth +12%" (forecast) | Reads as a promise and is a stand average | "Estimated growth of trees left: +12 % (average, if nothing else changes)" [P1] plus the Crop Tree figure (P2) |
| "Wind exposure: high" | Implies a known risk; storms do not exist | Remove until storms; then Stable/Watch/Exposed with reason |
| "Retain fallen deadwood" (objective) | Implies habitat value is measured | "Fallen deadwood on site" (fact) |
| "Ready to join the canopy layer" | Fine (diagnosis) | Keep |
| "Year 100 ended before the continuous-cover objectives were reached" | Failure framing (`FailureRecoveryDesign.md`) | "Century Review — scenario goals not completed" |

## 2. Glossary (canonical, player-facing)

| Term | Definition (≤ 20 words) | Notes |
|---|---|---|
| Continuous-cover forestry (CCF) | Managing a forest by repeated, selective work that keeps tree cover while new trees grow | Never "a structure"; always "a way of managing" |
| Crop Tree | A tree you choose to keep and favour for future timber | Blue mark |
| Fell mark | A proposal to fell a tree; carried out only after approval | Red mark |
| Competitor | A neighbouring tree that limits a chosen tree's growth; larger and closer neighbours compete more | Hegyi |
| Competition (value) | How crowded a tree is by neighbours; higher means less room to grow | Number shown; no verdict |
| Release | Felling chosen competitors to give a Crop Tree more room | |
| Thinning | Felling some trees so the remaining trees have more room | |
| DBH | Trunk diameter at 1.3 m above ground | |
| Basal area | Total trunk cross-section per hectare; how much tree is standing | m²/ha |
| Stem volume | Modelled wood volume of a trunk | m³ |
| Regeneration | Young trees growing from seed in the forest | |
| Seedling / sapling | Natural young tree / nursery-grown young tree you plant | |
| Seed source | A tree old enough to produce seed | Sitka from about age 20 here |
| Enrichment planting | Planting a few trees of species the forest lacks | |
| Browsing | Deer eating young shoots, slowing or stopping young trees | |
| Tree shelter | A tube that protects one young tree from browsing for some years | Browse only |
| Vegetation clearance | Removing plants (and young trees) from an area | Conditional value |
| Bramble / bracken | Ground plants that can smother very small trees where dense (Model 2) | |
| Deadwood | Dead trees or logs left on site; habitat for fungi, insects and others | |
| Pruning lift | Removing lower branches to a set height for knot-free timber | |
| Work Plan | Where jobs from your forest decisions are reviewed, costed and approved | Not where you decide where |
| Annual Review | What happened this year, what it cost, and where to look | |
| Small-job minimum | The least a contractor visit costs in this scenario (€2,500) | Scenario rule |
| Stability (storms) | How exposed a tree is to strong wind: Stable, Watch or Exposed | After storms |
| Windthrow | A tree blown over by wind, roots and all | After storms |
| Salvage | Extracting and selling (or keeping) windthrown stems | After storms |

## 3. Tests for the implementing packet

String sweep over `UI/*`, `ScenarioEcologyReviewLines`, `ClearancePreview`, `VegetationClearance`, the marking manager and objective display: no raw species IDs (`[a-z]+-[a-z]+` matching species ids) in rendered text; no "cohort", "juvenile", "CI " in rendered text; "harvest" only in "harvesting work".
