# Implementation sequence, what not to build, and external testing

**Status:** RECOMMENDATION. Covers task Parts 20–22. Wave names W0–W4 refer to the expansion proposal (`ForestryExpansion/ImplementationWaves.md`, PROPOSAL); this file only places them against the curriculum.

## 1. Systems by category (Part 21)

| Category | Systems | Scenarios that need them |
|---|---|---|
| **CAN BUILD WITH CURRENT SYSTEMS** | Scenario One P-series completion (P3 residual summary, P4 history/diary, P5 progression, P6 second cycle/completion — all PROPOSALS); plan comparison (Q1); B1 storm content once a package exists | S1, B1 (content), Training v1 (content) |
| **SMALL EXTENSION** | Scenario package v0 (id in save, start snapshot, inherited-history boundary, scenario select); authoring harness; per-scenario completion and assistance profiles; "no work needed" record; diagnosis tiering ("evidence, not verdict"); optional owner's-plan record | S2, B1, Training v1; tiering for S4; owner's plan for S6 |
| **REQUIRES MULTI-SPECIES FOUNDATION** | W0: species tags/literal-id removal, shade casting/occupancy, species-aware density (pure-Sitka bit-identical); W1-lite: Scots pine, downy birch, common alder; later rowan, holly; Douglas fir from GROWFOR; Norway spruce | S3 (W0 + SP/birch/alder + DF planting), S4 (rowan, holly), S5 (NS) |
| **REQUIRES SITE SYSTEM** | Site map with moisture/drainage, fertility, exposure (ordinal); species tolerance envelopes; site in stand generator | S3 onward |
| **REQUIRES TIMBER QUALITY** | Per-tree quality state (form class, browse history — D-044 deferred item), save bump; assortment eligibility from quality | S5 (optional pull-forward of visible form class to S2 is an open decision) |
| **REQUIRES NEW ECONOMIC SYSTEM** | Species assortment registry (T1); DF/NS/SP markets (T3) with no fabricated prices; fencing costing; liquidity reporting | T1 for merchantable SP in S3 (or honest "no market"); fencing in S4; T3 in S5 |
| **REQUIRES NEW DISTURBANCE / ECOLOGY SYSTEM** | Storm activation per scenario (policy decision, D-050); fencing gameplay (D-044); open-habitat state; standing deadwood; hazel resprouting; WH/WRC (research-blocked) | Storm on: B1, S5, S6. Fencing: S4. Open habitat + standing deadwood: B2 |

## 2. Recommended build order

```text
NOW            Finish Scenario One P-series (P3 → P4 → P5 → P6) — in progress elsewhere
               Research in parallel (no code): GROWFOR extraction (SP, DF, NS);
                 density evidence audit; ordinal trait note (birch, alder, rowan, holly)

WAVE S2        Scenario package v0 + authoring harness + scenario select
               → Scenario 2 content + completion/assistance profile + "no work needed" record
               → Training Stand v1 (same machinery)                         [can overlap]
               → Branch B1 After the Storm (storm on for this package only)  [after S2]

WAVE S3        W0 foundation (Sitka bit-identical) + site map + stand generator
               → Scots pine, birch, alder (+ Douglas fir planting if GROWFOR ready)
               → Scenario 3

WAVE S4        Diagnosis tiering + fencing gameplay + rowan/holly (ordinal) + species browse classes
               → Scenario 4

WAVE S5        W2 layer diagnostics + quality state (W4 subset) + T1/T3 markets + Norway spruce
               → Scenario 5 (graduation)

WAVE S6        Content only (+ owner's-plan record) → Scenario 6
               → Sandbox (gates SB1–SB7)

LATER          Branch B2 (open habitat, standing deadwood, hazel); WH/WRC after Irish research
```

**Guard rule for the manager:** a packet may build a system only if the *next unbuilt scenario in this order* needs it. Example: fencing must not be built during Wave S2; the stand generator must not be built while Scenario 2 is unfinished; quality state must not be built during Wave S3 unless the S2 form-class decision pulls a subset forward.

## 3. Top implementation dependencies

1. Scenario One P-series settled, especially P4 (history/diary — Scenario 2 is built on reading history) and P6 (second-cycle completion — the model Scenario 2's completion profile generalises).
2. Scenario package v0 with scenario id in the save, start-from-snapshot, Reference Future isolation.
3. Per-scenario completion and assistance profiles (small, data-driven).
4. W0 foundation + site map + GROWFOR extraction research — the long lead for Scenario 3; research should start now because it blocks nothing current.
5. Quality state and fencing gameplay — gate Scenarios 5 and 4 respectively; both are D-044 deferrals that need their own accepted packets.

## 4. What not to build yet (Part 22)

| Tempting system | Why not yet | Earliest |
|---|---|---|
| Sandbox UI | Exposes uncalibrated species and meaningless combinations; nothing to template yet | After S6 (gates SB1–SB7) |
| General scenario editor | S2 is authored by simulation; S3 needs a generator, not an editor | Possibly never; sandbox UI covers player needs |
| Stand-table description format | Not needed for S2 | Wave S3 |
| Full market simulation / price dynamics | No evidence base; 2026 prices storm-distorted | Not planned |
| Detailed timber grading | Player must first understand stand management; market evidence missing | Wave S5 (subset) |
| Many species at once / equal depth for all | Evidence is uneven by design; variety is not a reason | Per scenario need |
| Western hemlock / red cedar | Quantitatively blocked in Ireland | After Irish research |
| Certification schemes, grant optimisation | Teaches compliance, not forestry | Not before S6, as policy layer only |
| Universal forest-quality / biodiversity / CCF score | Contradicts the Game Brief and completion design | Never |
| Multiplayer-specific scenario infrastructure | D-022/D-034 | When a milestone needs it |
| Landscape management outside forestry (ponds, construction, orchards) | User direction: forestry first | After the forestry-mature gate |
| Adaptive AI tutor / difficulty scaling | Fixed per-concept assistance profiles are enough and testable | Not planned |
| Deer population model | Pressure as configuration is sufficient for S1–S4 | Possibly B2, only if a decision needs it |
| Terrain/biome generalisation | Irish holdings only; reuse terrain where possible | When scenario content demands it |

## 5. External testing (Part 20)

No telemetry system. Use short, structured sessions with think-aloud and the existing playtest protocols (`BeginnerPlaytestProtocol.md`, `ForesterReviewProtocol.md`, PROPOSALS on the readiness branch), extended per scenario.

| Scenario | A. Forestry-naive players | B. Experienced gamers | C. Small woodland owners | D. Foresters / CCF practitioners |
|---|---|---|---|---|
| **S1** | **Essential.** Can they explain why they kept a tree and what changed when they came back? Where do they stall? | Pacing of the annual loop; is walking back worth it? | Does it feel like *their* kind of woodland problem? Are contractor economics recognisable? | **Essential.** Any misleading causal claim or terminology? |
| **S2** | **Essential.** Do they transfer selection to an inherited stand? Do they ever choose "no work here"? | Is reassessment engaging without new mechanics? | **Essential.** Is the inherited-woodland story credible (buying/inheriting a part-managed forest)? | Are the previous owner's mistakes realistic? Is advance-regeneration release plausible? |
| **B1** | Do they understand salvage vs retention? | — | **Essential** (post-Éowyn relevance): credible choices and economics? | Credible storm aftermath; edge-stability teaching |
| **S3** | Can they say *why* a species did well in one place and not another? | Does species choice feel like a decision, not a palette? | Planting choices credible for Irish farm forests? | **Essential.** Species–site claims and strategy contrasts; no misleading trajectories |
| **S4** | **Essential.** With evidence but no verdict, can they name the limiting factor? Opaque or fair? | Is diagnosis satisfying or tedious? | Protection economics realistic? | Browse/regeneration plausibility by species |
| **S5** | Optional (can graduates of S1–S4 manage it?) | **Essential**: depth and replay value | Income model credible? | **Essential**: does it look like practised CCF? Would a practitioner recognise good and bad management? |
| **S6** | Graduates only | Long-session engagement | **Essential**: realism of priorities, sequencing, cash and labour | Credibility of the integrated outcome review |

**First external public test beyond Scenario One:** Scenario 2 (cheap, current systems, tests the most important transfer). B1 is the strongest candidate for small-owner testers specifically.
