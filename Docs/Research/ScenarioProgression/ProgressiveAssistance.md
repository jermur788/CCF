# Progressive assistance, unlocking and completion

**Status:** RECOMMENDATION. Covers task Parts 5, 15 and 16.

## 1. Principle: remove *advice*, never *information*

A late scenario must not become harder by hiding facts. A real small-woodland owner can measure DBH and height, count stems, see crowns, see browse signs and bramble, read their own and the previous owner's records, look at a soil map and hire a forester. The game keeps showing all of that throughout. What fades is:

1. **Explanation** (terminology, mechanism text, first-use help);
2. **Attention direction** (prompts to look at a place, highlighted trees, "next" hints);
3. **Interpretation** (a single named conclusion such as "Light-limited");
4. **Suggestion** (any hint about what to do — already absent from P2's design and kept absent).

Prescriptions are never given at any level. Tier changes only how much of (1)–(3) appears.

## 2. Four assistance tiers

| Tier | Explanation | Attention direction | Interpretation | Typical use |
|---|---|---|---|---|
| **T1 Guided** | Full first-use help; terms defined inline | Progress panel "Next"; PLACES TO INSPECT; second-look reason | Named limiting factor ("Light-limited", "Browsed — exposed") | Scenario 1 focus concepts |
| **T2 Supported** | Help on request (glossary, F1) | PLACES TO INSPECT only when the forest changed; no "Next" | Named factor shown, with the evidence lines underneath | Scenario 1 cycle 2; Scenarios 2–3 focus concepts |
| **T3 Evidence** | Glossary only | None (the player chooses where to look); the Annual Review still reports what changed | **Evidence lines only**: seedlings by species/height, browse signs, vegetation cover, canopy openness, nearest seed trees, site class. No conclusion label | Scenario 4 onward for regeneration; Scenarios 3–4 for older concepts |
| **T4 Independent** | Glossary only | None | Evidence only; consequences reported in the Annual Review | Scenarios 5–6, branches B2 |

**Information that stays at every tier:** tree card measurements, P2 competitor list (it describes the competition index, which a forester estimates by eye), P3 residual-stand figures, stand figures, map layers, management history, costs and quotes, Annual Review consequences.

**"Evidence, not verdict" (T3)** is the single most important design change for late scenarios. The existing ground "why" readout (IMPLEMENTED) returns one limiting factor. For T3/T4 the same assessment code should render its *inputs* rather than its conclusion. This is a presentation split of existing state, not new ecology.

## 3. Assistance is set per concept, not per scenario

Each scenario declares a small **assistance profile**: which concept families are at which tier.

| Concept family | S1 | S2 | S3 | S4 | S5 | S6 | B1 | B2 |
|---|---|---|---|---|---|---|---|---|
| Reading trees/stands | T1 | T2 | T3 | T3 | T4 | T4 | T3 | T4 |
| Positive selection / release / residual | T1→T2 (cycle 2) | T2 | T3 | T3 | T4 | T4 | T3 | T4 |
| Management history / reassessment | T2 (own) | **T1** | T2 | T3 | T4 | T4 | T2 | T4 |
| Regeneration, vegetation, browse | T1 | T2 | T2 | **T1→T3** (explain the evidence, then no verdict) | T4 | T4 | T2 | T3 |
| Species and site | — | — | **T1** | T2 | T3 | T4 | — | T3 |
| Pruning / quality / assortments | — | **T1** (pruning) | T2 | T2 | **T1→T3** (quality) | T4 | — | — |
| Disturbance / salvage | — | — | — | — | T3 | T4 | **T1** | — |
| Ecological objectives / restraint | — | T2 | — | — | T3 | T4 | — | **T1→T3** |
| Economics | T1 | T2 | T2 | T2 | **T2→T3** (strategic) | T4 | T2 | T3 |

Bold = the scenario's focus concept. Implementation: one small per-scenario data table read by the existing UI help/progress layers. No per-concept AI tutor, no adaptive difficulty.

**Player override:** a player may raise assistance in any scenario (e.g. re-enable named factors) from settings. Lowering assistance in Scenario 1 is allowed too. Overrides are recorded so playtest findings can be interpreted, but they do not block completion.

## 4. Specific behaviours to remove gradually

| Behaviour | Where it exists / is proposed | Fades at |
|---|---|---|
| Explicit terminology inline | Help/first-use texts (IMPLEMENTED) | T2 |
| Prompts to inspect specific locations (PLACES) | Annual Review (PROPOSAL, P6) | T3 (kept only for events the owner would plausibly learn of: a storm, a contractor's report of damage) |
| Explanations of Crop Trees | Help (IMPLEMENTED) | T2 |
| Guided Work Plan interpretation | Quote explanations (IMPLEMENTED) | T3 (quotes stay, explanations shorten) |
| Named regeneration limiting factor | Ground "why" (IMPLEMENTED) | T3 |
| Automatic "promising tree" identification | **Not recommended at any tier** — selection is the core skill. If any candidate highlighting exists in Scenario 1, it should stop at T2 |
| Stage progress panel | PROPOSAL (P5) | Scenario 1 only (and S2 for history stages) |
| Practitioner commentary | Training Stand (PROPOSAL) | Training mode only |

## 5. Not opaque, not trivial

Two failure modes to test for in playtests:

- **Opaque:** the player cannot find the information needed to reason. Test: can a tester, asked "why did regeneration fail here?", point to on-screen evidence? If not, the evidence display is insufficient (fix the display, do not add a verdict).
- **Trivial:** the player follows labels without reasoning. Test: in Scenario 4, does the player's stated reason match the evidence, or only the label they learned in Scenario 1?

## 6. Progression and unlocking (Part 15)

Progression represents **knowledge, experience, scenario access and information complexity**. It never restricts what a landowner can physically do or buy.

| What unlocks | How | Notes |
|---|---|---|
| Next core scenario | Completing the previous one | Allow "skip with warning" for experienced foresters (playtest group D) |
| Branch scenarios | B1 after Scenario 2; B2 after Scenario 4 | Knowledge prerequisites, not rewards |
| Species knowledge pages | Meeting a species in play | A reference, not a power-up; everything in a scenario is usable from Year 0 |
| Assistance defaults | Lower default tier for concepts already completed | The player can always raise it again |
| Training Stand exercises | After Scenario 1; species exercises after Scenario 3 | Practice, no score |
| Sandbox templates | After Scenario 5 (graduation); compartments from Scenario 6 later | See `ScenarioAuthoringNeeds.md` §3 |

**Never unlock:** tools, operations (pruning, fencing, planting), species *within* a scenario that already contains them, contractor access, or Work Plan features. If fencing is not in Scenario 2, that is because fencing is not modelled for that scenario's purpose, and the scenario says so plainly ("no fencing in this woodland's plan"), not because the player "hasn't learned it". D-011 (no XP ladder) holds.

## 7. Scenario completion (Part 16)

Principles carried from `ScenarioCompletionDesign.md` (PROPOSAL, model D) and extended to every scenario:

1. **Separated decision cycles**, with a minimum gap long enough to see consequences.
2. **Guardrails held throughout**, not targets: continuous cover, standing capital, solvency where relevant.
3. **The focus lesson actually encountered**, evidenced from saved data (not from session-only UI events).
4. **Time to observe**: completion only after the last cycle's consequences have had years to show.
5. **No universal score, no target forest shape**, no species percentages.
6. **Poor but recoverable forestry stays playable.** Only genuinely unrecoverable states end a scenario early, and even then the review explains rather than grades.

| Scenario | Min cycles (gap) | Guardrails | Focus-lesson evidence | Observation time |
|---|---|---|---|---|
| S1 | 2 (≥ 5 yrs) | Cover, capital | First release near a Crop Tree; renewal (PROPOSAL model D) | ≥ Year 25 |
| S2 | 2 (≥ 5 yrs) | Cover, capital | Area-specific treatment: at least two areas treated differently, including one recorded "no work needed"; history view opened at least once (saved) | ≥ 10 yrs after cycle 2 starts |
| S3 | 2 (≥ 5 yrs) | Cover, capital | Enrichment or regeneration decision on ≥ 2 site types; species outcome observed ≥ 5 yrs later | ≥ 5 yrs after the last planting |
| S4 | 2 (≥ 5 yrs) | Cover | Recruitment established by ≥ 2 different player-chosen routes; protection cost incurred *or* a recorded decision to accept browse | ≥ 8 yrs (escape height) |
| S5 | 3 (≥ 5 yrs) | Cover, solvency, layers present | Layers and recruitment maintained across cycles; income taken | ≥ 15 yrs |
| S6 | 3 across compartments | Solvency | Owner's plan recorded; review compares outcomes with it | ≥ 15 yrs |
| B1 | 2 | Stand standing (cover) | A recorded salvage/retention decision; gap regeneration observed | ≥ 8 yrs |
| B2 | 2 (or 1 + recorded restraint) | Open habitat retained; cover | Owner's ecological objective recorded and reviewed | ≥ 15 yrs |

Thresholds (canopy 0.35, capital counts, layer definitions) are [C] calibration choices to be set and tested per scenario, not proposed here.

**Solvency:** keep the existing direction (P6 PROPOSAL O1) of not failing a scenario at zero cash. From Scenario 5 onward, cash below what the next necessary operation costs should be *reported* as a real constraint (operations become unaffordable), not converted into a game-over.
