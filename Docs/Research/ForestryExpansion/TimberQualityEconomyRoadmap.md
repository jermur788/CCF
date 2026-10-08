# Timber, quality and economy roadmap (Parts 8 and 9)

**Status:** proposal. **No price changes. No fabricated market prices.** Values appear here only where the project already holds them [R:EC]. Everything else is a research requirement.

## 1. What the current timber/economy code can already carry (Part 8 audit) [REPO]

| Component | Multi-species capable? | Note |
|---|---|---|
| `MerchantableStemModel` (taper/volume budget from DBH, height, volume) | **Yes, structurally**: species id carried. The taper shape is the same for all species | Broadleaf forking/branching not represented (quality) |
| `TimberYieldCalculator` | **Yes**: filters `AssortmentSpecification` by `SpeciesId`; bucks logs by small-end diameter, length and quality flags | Only `CreateSitka()` specs exist |
| `AssortmentSpecification` | Yes (species, assortment enum, potential use, lengths, diameters, allowed flags) | Assortment enum: Pulp/Stake/Pallet/Sawlog (+ residue). Broadleaf categories (firewood, planking, veneer/beam) absent |
| `ForestryPriceBook` (timber prices, densities, materials) | Yes: rows keyed by species | Only Sitka rows (IFA 2024 roadside [R:EC]) |
| `TimberYieldEconomyAdapter` bindings (species × assortment → price/density) | Yes | Sitka bindings only |
| `ScenarioOneEconomyAdapter` | **No**: hard-codes the Sitka density and price ids; non-Sitka = "unmarketed" (charged felling, no revenue) | G8 |
| Harvest work cost (€/t by intervention, minimum job) | Species-neutral | Fine (contractor cost is mass-based) |
| Quality flags (`PoorForm`, `StemDefect`, `WindDamage`, `BrowseFormDamage`, `Pruned`, `Unusable`) | Exist in the yield model | **Only `WindDamage` is used** (salvage). No tree-level quality state |

**Conclusion:** structure is ready; content (specs, prices, densities, quality inputs) and one adapter are missing.

## 2. Staged timber/economy model

| Stage | Adds | Needs (research) | Save |
|---|---|---|---|
| **T1 Species assortment registry** | A data asset per market species: assortment specs, green density, price ids. Generic adapter. Inspection estimate for any marketed species | Assortment specs per species (Teagasc / Irish buyer specs [R:EC] cover Sitka; others **missing**) | None |
| **T2 Low-grade outlets for broadleaves** | Firewood / pulp-equivalent for birch, alder, thinnings; honest "no market" where none | Irish firewood/roundwood prices by species (**missing**) | None |
| **T3 Productive conifer markets** | Douglas fir, Norway spruce, Scots pine (and others in Wave B) sawlog/pallet/pulp | Irish prices or a documented relation to Sitka (**missing**; [R:EC] states no robust schedule beyond the IFA Sitka table) | None |
| **T4 Quality hardwood markets** | Oak, sycamore, cherry: planking, beam, **veneer/high-value** only where justified | Irish hardwood market evidence (**missing**); risk of fabricating premiums | None |
| **T5 Quality-dependent value** | Grade from tree quality state (§3) → assortment eligibility | Grade rules (MG) | Uses the quality state |

Contractor implications: mass-based harvest cost is species-neutral. Selective CCF productivity penalties ([R:EC]: UK analogue, harvester 90 % / forwarder 76 %) stay a scenario calibration, not a species trait. Broadleaf felling may need a different work rate [I] (research, not assumption).

## 3. Tree quality (Part 9)

### 3.1 Candidate quality drivers

| Driver | Visible state? | Evidence | Gameplay value | Recommend |
|---|---|---|---|---|
| **Pruning history** | Yes (lifts, clear stem height, exists) | MG (Teagasc pruning guidance [R:EC]) | High: pruning currently has no payoff | **v1** |
| **Browse / form damage** (leader browsed while young) | Partly (browse flag per year, not persisted) | TR [Browsing report]; D-044 deferred persistence | High for oak/rowan/cedar | **v1** (persist "leader browsed ≥ n times") |
| **Suppression history** | `equivalentSuppressedYears` exists (diagnostic, never decreases) | INF | Medium (slow, even rings vs. fast growth) | Later; do not feed into value without evidence |
| **Storm damage** | Exists for salvage | TR [R:EC] | Medium | Already (salvage) |
| **Stem form / straightness** | No state; bent visual variants exist (D-020: no state) | MG | High for Crop Tree selection | **v1 as an inherited trait** (form class at establishment, random by species) |
| Branchiness | No | INF | Medium; correlated with spacing | Later (derive from spacing/light history) |
| Damage from extraction | No | — | Later (Stage 2 access) | Defer |

### 3.2 Simplest history-dependent quality system worth building

**Per-tree `QualityState`** (persisted): `formClass` (good / average / poor; drawn at establishment from a species distribution [CAL]), `leaderBrowseCount` (incremented by the existing browse roll while juvenile), `clearStemHeightM` (existing pruning), `stormDamaged` (existing for salvage).

- **Visible defect state** (inspection, world variants where assets exist): form class, browse-forked leader, pruned clear stem. Visible to the player, so Crop Tree selection becomes a form-based choice.
- **Economic consequence** (separate): the yield calculator marks `PoorForm` / `BrowseFormDamage` sections (lower assortments) and `Pruned` clear-stem sections (eligibility for a pruned/quality assortment **only if a market parameter exists**; default no premium, matching [R:EC] "no guaranteed premium").

Keep visible state and market consequence decoupled, so the defect can be taught before any price exists.

### 3.3 Save

`QualityState` per tree is a schema change (version bump), done in the quality wave together with the browse-history persistence that D-044 deferred.

## 4. Research required before T1–T5

See `ResearchBacklog.md` R3 (assortment specs by species), R4 (prices by species/assortment, Irish basis), R7 (quality grading rules), R8 (broadleaf work rates).
