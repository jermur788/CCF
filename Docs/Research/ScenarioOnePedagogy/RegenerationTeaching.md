# Scenario One — regeneration teaching (Workstreams B5, B2 S9a–S9d)

**Status:** NEW PROPOSAL [INF] grounded in implemented state [REPO], harness evidence [HARNESS], empirical synthesis [EMP] and practitioner guidance [PRAC]. Not accepted.

## 1. Regeneration is not every intervention's goal (B5)

### 1.1 Four intervention purposes

Walloon and Spanish practitioner material separates the purposes of an intervention. Harvest and improvement can be primary while regeneration is secondary [PRAC §13.1, §12.2]. Irish guidance gives early transformation the purposes of stability, quality and infrastructure; regeneration comes later [PRAC §11.1].

| Purpose | Plain meaning | In the current game [REPO] | Supported? |
|---|---|---|---|
| **Improvement** | Remove poorer stems so better ones have space | No stem-form/quality state. "Poorer" can only mean smaller or more suppressed | PARTIAL (vigour only) |
| **Release** | Free chosen Crop Trees from their competitors | Hegyi competition; DBH response; harness T2/T4 | SUPPORTED |
| **Harvest** | Take valuable mature trees for income | Timber assortments and prices for Sitka; contractor minimum | SUPPORTED (Sitka only; no broadleaf market) |
| **Regeneration-oriented opening** | Make a gap where seed and light can start a new cohort | Cell light rises; seed rain from mature trees; harness T5 | SUPPORTED |

### 1.2 How the current build accidentally teaches "every intervention should make regeneration"

| Signal | Where | Why it misleads |
|---|---|---|
| Ground "Why" line names light as the only constraint ("Too dark under the canopy…") | `ScenarioOneUiFacts.Why` | Suggests the fix for every dark cell is opening it. Seed supply is not mentioned |
| Clearance square shown on every ground glance | `ForestPlayer` preview | Suggests the ground always needs action |
| Annual Review "Opened cells: n with mean light x" line | `ScenarioEcologyReviewLines` | Frames openings by their light, not by their purpose |
| Mandatory planting of both broadleaves | `ScenarioOneObjectives` | Frames regeneration as something the player must *produce* |

The **regeneration ≥3 cells** objective does *not* push the player: untreated stands meet it by Year 1 [HARNESS T0 Y1 = 14 cells]. It teaches nothing either way.

### 1.3 Proposal

- At S4, give the first intervention an explicit purpose label that the player chooses: *release my Crop Trees* / *take some timber* / *open space for young trees* / *several of these*. This is a **note**, not a mechanic. It is stored with the management event and shown in history, so the review can say "You aimed to release Crop Trees: competition on them fell 20 %." Needs a small event field — **PRODUCT DECISION REQUIRED** (save schema).
- Alternative with no save change: infer nothing, and phrase the review neutrally around all four purposes. Recommended as the *first* step; the purpose note can follow.

## 2. Where new forest comes from (S9a)

Facts the player can observe now [REPO][HARNESS]:

| Fact | Source | How to show it |
|---|---|---|
| Plantation Sitka begin producing seed at age 20 and reach full seed production at 30. The stand starts at age 20, so seed rain starts in Year 1 and is full by Year 10 | `SitkaSpruce.asset` maturity 20/30 | Inspection "Reproduction" field already shows this. Link it in S9a text |
| Natural Sitka regeneration appears even without thinning (14 cells at Year 1, 37 at Year 20 in the untreated harness run) | harness T0 | Map Regeneration layer |
| Openings raise early regeneration (T5 concentrated gap: 23 cells at Y1 vs 17 for T2 with the same removed volume) | harness T2/T5 | Diary comparison (Packet 5) |
| Seedlings need light to grow up. Promotion to a canopy tree needs 3.5 m height, and Oak needs ≥ 0.2 light | species assets | Ground "Why" ("Growing, but light is below what it needs…") already exists |
| **There is no Oak or Beech seed source on the property.** Broadleaves can only arrive by planting | starting stand is 336 Sitka only | **Not stated anywhere now.** Add to S9b |
| Planted Oak and Beech begin seeding at age 40 | `SessileOak.asset`, `BeechSpecies.asset` | Long-horizon point: enrichment planting now creates seed sources for natural broadleaf regeneration decades later |

**Teaching point:** abundance is not diversity. Natural regeneration can keep tree cover going here (Sitka renews itself), but it cannot add species with no seed source [PRAC §11.8 Irish Quick Guide 07].

## 3. Natural regeneration vs enrichment planting (S9b)

| Question the player should ask | Where the answer is | Example feedback |
|---|---|---|
| Is anything already regenerating here? | Ground report; map Regeneration layer | "Sitka seedlings 0.4 m in this cell." |
| Which species could arrive by seed? | Seed-bearing trees nearby (Sitka only) | "Seed reaches this cell from Sitka only." *(new, derivable from per-species seed rain)* |
| Is there enough light for the species I want? | Cell light; species thresholds | "Oak needs about 0.20 light to grow into the canopy; this cell has 0.12." *(derivable; thresholds exist)* |
| Will deer browse it? | Browse band; palatability (Oak 1.0, Beech 0.5, Sitka 0.15) | "Oak is highly palatable to deer; unprotected oak saplings here will be browsed until about 1.8 m." |
| What does it cost? | Nursery €4.50/€5.50; shelter €5; contractor time | existing Work Plan |

**Planting where adequate regeneration exists** is a novice error (`NoviceErrorLibrary.md` E7). In this stand the important nuance is *adequate for what*: Sitka regeneration is adequate for cover, but not for species diversity. The game should let the player see both.

**The completion objective that requires both broadleaves is PRODUCT DECISION REQUIRED** (`DecisionMatrix.md`). Options:

1. Keep it, and explain it: "No Oak or Beech seed trees exist on this property; planting is the only way to add them."
2. Replace it with "Establish at least one species that the original plantation could not supply" — the same practical effect, framed as diversification.
3. Make it optional (a Century Review comparison only).

**Recommendation: option 1 now (copy only), option 2 when objectives are next revised.**

## 4. Browsing and protection (S9c)

What the simulation supports [REPO; `Docs/BrowsingProtectionV1.md`]:

- Background browse pressure is fixed at 0.2, shown as "low". There is no deer population or fencing gameplay.
- Browsing suppresses height growth of palatable juveniles below the escape height (1.8 m for broadleaves, 0.8 m for Sitka).
- Shelters stop browsing for 8 years on one exact planted stem.
- Form damage and browse history are **not** persisted (D-044).

Best demonstration: the **sheltered vs unsheltered pair**. The completion gate already builds such pairs and logs `shelteredTaller=2 … exposedOakBrowses=1` [LOG P8]. A tutorial variant: the player plants two oaks in the same cell, shelters one, and revisits after 3–5 years. The diary shows both heights.

Do **not** teach "browsing kills"; in this model it mainly delays. That matches the Sitka evidence [EMP §4.2 Welch: delay ≈ 1 year; form damage is the larger long-term effect] and the D-023 direction.

## 5. Ground vegetation and clearance (S9d) — teach honestly

**Implemented truth [REPO]:**

- Ground vegetation (ferns, grasses, forbs, shrubs) is a provisional functional-group state. It has **no causal effect** on juvenile survival or growth, and no concealment effect on browsing (`BrowsingConditions` hook = 1; D-047 open item).
- Area clearance (U, 5 × 5 m) removes ground plants **and all tree regeneration cohorts and unpromoted planted saplings** inside the square, for contractor cost. Plants recolonise in later years.
- Planting clearance (1 m² spot, D-016) reduces cohort density proportionally in the circle.

**Therefore the tutorial must not say** "clearing vegetation helps seedlings grow" until a causal understorey model is accepted (D-024 is still current direction).

**Proposed S9d content:**

1. What clearance does here: cuts back everything low in the square, including young trees.
2. What it costs: contractor time.
3. What it does not do in this version: it does not make seedlings grow faster, because weed competition is not yet simulated.
4. When a forester would use it in real life (one sentence, labelled real-world practice): to give planted trees a start where bracken or bramble would smother them [EMP §5.1, transfer evidence].

This is an unusual situation: a tool whose real-world purpose the game cannot yet simulate. The honest framing keeps the game credible, and it sets up a clear future lesson when D-024 is implemented.

**Product decision:** should area clearance stay available in the tutorial before causal understorey exists? Recommended: keep it available, with the honest text above. Do **not** add any objective or lesson step that rewards using it. Remove the always-on preview (Packet 1). **PRODUCT DECISION REQUIRED** — `DecisionMatrix.md` row 9.

## 6. Tests

1. Ground "Why" names seed limitation where seed rain for all regenerating species is zero (derivable).
2. S9b copy appears only after S9a; the "no broadleaf seed source" statement is true for the scenario start (no non-Sitka tree with maturity > 0 within dispersal cutoff).
3. No string in the clearance lesson claims a growth or survival benefit (string test).
4. The sheltered/unsheltered pair diary shows heights from saved juvenile state after save/load.
