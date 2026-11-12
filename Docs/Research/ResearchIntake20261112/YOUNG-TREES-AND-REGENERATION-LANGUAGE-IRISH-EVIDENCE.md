# Young trees, regeneration and light-environment language — Irish evidence and CCF copy assessment

**Date:** 12 November 2026  
**Status:** RESEARCH COMPLETE — supporting evidence and proposed wording, **not** accepted product design or an implementation approval  
**Claim:** [#388](https://github.com/jermur788/CCF/issues/388)  
**Input handoff:** [Research-CCF-Young-Growth](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Handoffs/Research-CCF-Young-Growth-20261112/RESEARCH-HANDOFF-YOUNG-TREES-REGENERATION-TERMINOLOGY.md)  
**Research lane:** OpenAI GPT-6, browser research + connected GitHub, remote documentation branch only. No Unity/editor/Player execution.  
**Intended users:** Irish private woodland/farm-forest owner or manager playing Scenario One.  
**Authority:** Existing decisions and actual code retain priority. This document may inform a later design approval, but neither changes the 12-location objective nor authorises UI edits.

## 1. Decision summary (under 300 words)

**Recommendation:** Use **“Encourage woodland regeneration”** as an objective-purpose heading and **“Areas with young growth”** as the short progress label, accompanied by an explanation that this is a count of **distinct represented map locations** in which a natural regeneration cohort or a living individually planted juvenile is recorded. A more explicit alternative is **“Areas with recorded young growth”**. Do not call them “regenerating cells” in player copy. The count is neither young-tree stem density, hectares regenerated, nor proof that new trees will survive or replace the mature stand. The word “areas” is plain-language shorthand, not a measured area.

Irish professional material uses **natural regeneration**, **planting**, **seedling**, **sapling** and **establishment** as real but distinct concepts. Germination and seedling presence must not be collapsed into durable establishment or later recruitment. The distinction is borne out by Irish Killarney monitoring, where seedlings persist poorly to sapling stages in browsed woodland, including gaps. Shelter, gap-margin and open conditions are useful qualitative teaching concepts; they are gradients rather than automatically classified fixed bands. A machinery extraction rack is not a silvicultural regeneration strip.

Current code records natural cohort occupancy, exact planted juveniles, annual aggregated snapshots and work events, but it does **not** establish a distinct count of naturally versus artificially occupied locations or a history of each location appearing/disappearing. An annual total may be compared to an earlier recorded total where years and measurement definitions are compatible, but their difference is not a count of newly recruited sites. Separating natural/planted locations requires extra aggregation or saved evidence, not a wording-only change.

**“Establish planted Oak/Beech”** currently asserts more than its predicate: a successful planting action plus presence of the species somewhere. Prefer an event-faithful label in a later design review; leave the predicate unchanged here. All proposals remain design-only until approved.

## 2. Source register and provenance

Classification key: **IRISH-PRACTICE** = published Irish agency/forestry advice, not necessarily law; **IRISH-GUIDELINE** = site- and purpose-specific native-woodland management guidance; **IRISH-OBS** = observed Irish research; **UK-TRANSFER** = professional UK guidance not automatically transferable quantitatively; **EUROPE-TRANSFER** = general European silvicultural framework; **MODEL-DERIVATION** = directly inspected CCF implementation or accepted game calibration, *not* a real-world observation. “High” certainty applies to *what the source actually says*; it does not imply a universal biological rule.

| ID | Specific supported statement / boundary | Primary link; date; page or section | Geography / species | Class; confidence; CCF use |
|---|---|---|---|---|
| S1 | Natural regeneration comprises establishment of trees/shrubs from naturally arriving seed; light, seed supply and grazing affect germination and survival. The term is not synonymous with planted stock. | [Cross & Collins, *Management Guidelines for Ireland’s Native Woodlands*](https://www.npws.ie/sites/default/files/publications/pdf/Management%20Guidelines%20for%20Ireland%27s%20Native%20Woodlands%202017.pdf), NPWS & Forest Service, **July 2017**, §B5, printed pp. 43–46, especially pp. 44–46. | Irish native woodlands; native tree/shrub species | IRISH-GUIDELINE; high for definition; use to distinguish origins; **not** a statement that all mixed-production planting is inappropriate. |
| S2 | Native oak may produce seedlings beneath canopy without those seedlings persisting; openings can release vegetation that competes with them. Dense bramble can also sometimes protect seedlings from browsing. | [Same official guide](https://www.npws.ie/sites/default/files/publications/pdf/Management%20Guidelines%20for%20Ireland%27s%20Native%20Woodlands%202017.pdf), **2017**, §B5, printed pp. 44–46. | Irish native woodland, especially oak | IRISH-GUIDELINE; high for qualified statement; do not promise opening or vegetation removal always improves regeneration. |
| S3 | Teagasc explicitly divides natural regeneration into seed production, seed predation, germination and establishment; seed production alone does not demonstrate young trees. | [Teagasc, *Natural Regeneration in Irish Woodlands: Seed Production*](https://teagasc.ie/news--events/daily/natural-regeneration-in-irish-woodlands-seed-production/), **23 February 2024**, introduction and “Seed Production.” | Ireland; oak, beech and other broadleaves with cited transfer literature | IRISH-PRACTICE synthesis; high for terminology, moderate for older imported empirical timings; no seed-year coefficient copied. |
| S4 | Seedling establishment/survival depends on soil, moisture, competition, light and damage; beech can tolerate more shade than oak, but light still affects development. | [Teagasc, *Natural Regeneration in Irish Woodlands: Seedling Establishment*](https://teagasc.ie/news--events/daily/natural-regeneration-in-irish-woodlands-seedling-establishment/), **26 April 2024**, opening; “Vegetation competition”; “Light”; “Browsing.” | Irish forestry advice drawing extensively on British/European literature; oak/beech | IRISH-PRACTICE with **underlying transfer**; high for qualitative mechanisms, not Ireland-measured response coefficients. |
| S5 | Irish CCF transformation guidance describes canopy manipulation and natural regeneration established in small, irregular cohorts, followed by structural development. | [Teagasc, *Transformation to CCF*](https://teagasc.ie/crops/forestry/advice/management/continuous-cover-forestry/transformation-to-ccf/), **undated live guidance, checked 12 Nov 2026**, “Stage 2: Regeneration” and “Stage 3.” | Ireland; even-aged plantation transformation | IRISH-PRACTICE; high for management vocabulary; not proof that one thinning produces effective regeneration. |
| S6 | CCF scheme operations distinguish **management/re-spacing of natural regeneration** from **enrichment planting**, as well as browsing control and seedbed preparation. | [Teagasc, *WIS Element 3 – Continuous Cover Forestry*](https://teagasc.ie/crops/forestry/grants/woodland-improvement-scheme-2023-2027/wis-element-3-continuous-cover-forestry/), **current 2023–27 scheme page, checked 12 Nov 2026**, “Eligible operations”; [DAFM scheme page](https://www.gov.ie/en/department-of-agriculture-food-and-the-marine/services/woodland-improvement-scheme-including-continuous-forestry-cover/), live results checked 12 Nov 2026. | Irish CCF operations | IRISH-PRACTICE / policy description; high for categories, do not infer legal eligibility outside present scheme conditions. |
| S7 | A 30-year Killarney monitoring study uses explicit **S1/S2 seedling, S3/S4 sapling** survey categories (site-study boundaries). More seedlings did not predict sapling persistence; observed grazing limited recruitment even in gaps. | [Hamilton, Mitchell & Kelly, *IWM 141*](https://www.npws.ie/sites/default/files/publications/pdf/IWM141.pdf), **2022**, §3.1, Table 2 (printed p. 11); §5.2 (esp. figure 18 / printed p. 22); §6.1 discussion. | Killarney National Park native woodlands incl. sessile oak, holly, birch | IRISH-OBS; high for sampled site/years. Table 2: S1 <1 year; S2 >1 year and <25 cm; S3 25–200 cm; S4 >200 cm. **Not** universal Irish definitions or game cutoffs. |
| S8 | A long-term deer-exclusion comparison found strong composition changes; sessile oak failed to establish under oak canopy despite exclusions; interpretation is site-specific. | [Perrin, Kelly & Mitchell, *Forest Ecology and Management* 236](https://doi.org/10.1016/j.foreco.2006.09.025), **2006**, abstract, pp. 356–367. | Killarney yew-wood/oakwood; native woodland | IRISH-OBS; high for study context, not direct prediction for Scenario One plantation. |
| S9 | UK researchers caution that managers may over-count seedlings, under-estimate browse, and neglect regeneration distribution; **saplings are likelier to enter new stands** than undifferentiated seedlings. | [Forest Research, *Continuous cover silviculture*](https://www.forestresearch.gov.uk/research/continuous-cover-silviculture/), live page checked **12 Nov 2026**, “Regeneration” and “Regeneration of continuous cover forests.” | Great Britain; CCF conifers/broadleaves | UK-TRANSFER; high for advisory distinction, useful for honest language. |
| S10 | UK booklet describes “advance regeneration” as understorey seedlings/saplings and sets *its own* seedling/sapling measurement categories. | [Forestry Commission, *Managing Continuous Cover Forests*, OGB 7](https://cdn.forestresearch.gov.uk/2008/07/fcogb7_v3_july2008.pdf), **9 July 2008**, v3, p. 16, Table 6.1 footnote. | UK forests | UK-TRANSFER; high for the booklet only: seedling <1.3 m, sapling ≥1.3 m and <7 cm DBH. **Conflicts in boundaries with S7's study categories, demonstrating that no single universal cutoff should be asserted.** |
| S11 | Shelter, forest margin and open conditions are three elementary spatial principles; named silvicultural systems combine these in different ways. The margin is a gradient, not a fixed Irish distance band. | [Pommerening, Szmyt & Duchiron, *Revisiting silvicultural systems*](https://doi.org/10.1016/j.tfp.2024.100597), **2024**, §§2–3, paper pp. 2–5; also the user-supplied *CCF Six Papers Relevance Evidence Extraction*, P6 pp. 16–17. | International/European conceptual taxonomy | EUROPE-TRANSFER / method; high for conceptual distinction; no game or Irish gap-width parameter adopted. |
| S12 | CCF distinguishes live natural cohort population occupancy from exact planted individuals; aggregates distinct occupied grid positions and stores annual totals. | [ScenarioOneManager.cs](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs), lines around 2579–2664, and [ScenarioOneObjectives.cs](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs), around 65–94; inspected **12 Nov 2026** on named branch. | CCF simulation (Enlarged80 0.64 ha) | MODEL-DERIVATION; high for inspected source only, not Unity runtime validation. |

**Provenance note:** The 2017 native-woodland guideline is authoritative guidance *for native-woodland management*, not a regulation against planting Beech in an authored production-forest teaching scenario. Teagasc's 2024 educational series incorporates many dated foreign and historical citations; those details should not be read as freshly measured Irish establishment rates. A 2022 commissioned NPWS science report explicitly states that its views need not be those of NPWS: classify its findings as **Irish research observations**, not official policy. No new numeric transfer coefficients were selected.

**PDF visual-access limitation:** The web tool returned the original NPWS PDF text, including IWM 141 Table 2 and described figures; attempted PDF screenshot retrieval returned a cache error. Claims above are limited to readable text/tables and do not pretend that PDF figures were independently screenshot-qualified. The supplied six-paper extraction and source compendium were separately available as project documents.

## 3. Vocabulary guidance

| Term / wording | Verdict | Meaning and risk |
|---|---|---|
| **Natural regeneration** | RECOMMENDED where origin supported | Young trees developing from naturally arriving seed. Do not mix planted stock into this origin label. The process includes stages; presence is not automatic long-run success. [S1, S3] |
| **Planted young trees** / **new planting** | RECOMMENDED for separately recorded exact planting | Describes deliberately introduced nursery stock/planting events. Say “planted” rather than “naturally regenerated” for such trees. [S6, S12] |
| **Seedlings** | ALLOWED only when height/age rule is matched to shown population | A real life-stage category, but thresholds differ between S7 and S10. Avoid imposing a study's specific cutoff on the game. |
| **Saplings** | ALLOWED only if stage is verified | Meaningful later growth stage; implies more development than mere early germination, not guaranteed adulthood. |
| **Advance regeneration** | CONDITIONAL | Young trees already beneath an existing overstorey before a release/felling. Not a synonym for all observed young trees or post-planting plots. [S10] |
| **Establishment** / **established** | CONDITIONAL / CAUTION | In forestry, establishment is a process/outcome beyond the physical planting action. Do not assert an exact CCF success criterion without measured survival or an adopted game definition. [S1, S4] |
| **Recruitment** | CONDITIONAL | In research may mean passage into a later stage, adult-tree inventory or the canopy. Must state *which* transition. The objective's occupied-location count measures none of these directly. [S7, S9] |
| **Ingrowth** | AVOID for this current display | Usually used in measurement/model contexts for trees entering a tracked size/diameter class; not an everyday name for first seedling presence. No game data-contract assumption. |
| **Young growth** | RECOMMENDED as accessible umbrella | Plain language covering represented natural early growth and planted juveniles without asserting actual stem numbers. Add a note about counted locations. |
| **Areas with young growth** | RECOMMENDED short metric | More natural than technical “cells.” “Areas” means counted map locations; add explicit explanation to prevent an area/ha inference. |
| **Areas with recorded young growth** | ALLOWED / technically safer fallback | Slightly longer; makes the model's observation/representation boundary explicit. |
| **Areas where young growth is recorded** | ALLOWED | Clear, but awkward for narrow UI. |
| **Areas with young trees** | ALLOWED WITH EXPLANATION | Friendly, but could imply physically enumerated individuals in every area; natural cohorts are abstract. |
| **Parts of woodland with young trees** | ALLOWED WITH EXPLANATION | Similar to preceding; risks assuming exact live individual positions for modelled cohorts. |
| **Woodland regeneration present** | ALLOWED as a status, NOT a numerical metric label | Does not reveal that the number counts locations; use only with supplementary meaning. |
| **Regenerating cells / occupied regeneration cells** | AVOID player-facing | Internal simulation representation exposed as forestry language. [S12] |
| **Regeneration coverage / hectares regenerated / established regeneration** | AVOID for the count | Implies area measurement or successful future stand replacement not given by the metric. |
| **Regeneration patches / regeneration strips** | AVOID unless actual distinct connected patches / silvicultural strips are modelled | Neighbouring grid locations are not necessarily coherent patches; an extraction rack is a machinery route, not a deliberate silvicultural strip. [S11] |

**Terminological caution:** “Natural regeneration” can be used by forestry sources to describe the *whole process*, including establishment and growth, whereas the game's count is specifically a **representation of present young-tree occupancy**. Its usage in an explanation is justified only with this qualification, not as proof that the entire process has succeeded.

## 4. How the actual CCF count works (source-inspected)

- [ScenarioOneObjectives.cs](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs) has stable objective ID **regeneration** with current text **“Maintain regenerating cells”** and compares **occupiedRegenerationCells** to the geometry-effective target. Enlarged80 is 16 × 16 = 256 model cells, each 5 m × 5 m; 0.64 ha total. The accepted completion threshold is **12 counted locations**, a gameplay calibration, not an Irish field threshold.
- [ScenarioOneManager.cs](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs), **RecordEcologicalSnapshot**, counts a location when its model cell **HasRegeneration**. A living exact planted juvenile also adds its location only if it is not already occupied by a cohort and not already counted for another planted individual in that cell. Juveniles dead, legacy-cohort-managed or already promoted to an individual tree are excluded from this exact-juvenile pass. Distinct co-located origins are *not additive* to overall count.
- Natural regeneration is an abstract represented population/cohort, not one counted sapling per numeric density unit. Decision D-047 explicitly prohibits stems/ha interpretations; represented age bands and a promotion handoff are game abstraction. Natural cohort provenance and **planted** cohort provenance are distinguishable internally, while live exact juveniles have their own positions.
- The annual **ScenarioEcologicalSnapshot** retains one aggregate total per ecological year (unless already recorded for the same year). Per-species **regenerationCells**, **plantedRegenerationCells** and **plantedJuveniles** are not additive partitions of unique global locations; several species and origins may overlap. A separate **natural-only unique location count** is not available from this aggregate alone. The snapshots do not retain a prior-year set of occupied location indices sufficient to claim “X locations newly gained.”
- Saved management events can record completed planting and other work; they do not prove that each specific planted tree survived for a later period. Browsing assessments/history on exact juveniles and other per-stage details require persistence checks before any historical UI claim.
- Existing “introduced-species” objective [ScenarioOneObjectives.cs, lines ~81–94] is true if an Oak/Beech **planting event succeeded (or a matching order completed)** **and** **any current representation of that species** exists (living trees + species regenerationCells + plantedJuveniles). That representation need not be the original planted individual. The label **“Establish planted Oak/Beech”** is therefore stronger than the tested fact. A changed predicate would require a separate accepted product/data decision, not this research task.

### Natural versus planted breakdown

**Source-level honest now:** display one combined count and an explanation of two possible origins. For an *individually selected planted juvenile*, say “Planted Oak/Beech — position recorded” only if the exact object and species/origin are verified. For *represented cohort*, say “Natural young growth recorded here” only if its origin is Natural; a planted-origin cohort must not be silently relabelled Natural. **Do not** present the existing global counts as a pie chart or as “12 natural + 3 planted” without deduplicated origin-aware aggregation and explicit policy for overlap. Even “total minus planted” is invalid when planted and natural overlap a location.

## 5. Shelter, gap margins and open conditions — qualitative only

| Environment | Safe player explanation | What is not justified |
|---|---|---|
| **Beneath retained canopy / shelter** | A mature overstorey can moderate exposure but often shades the forest floor. Young trees may persist yet grow slowly; the species and site matter. | A fixed percentage survival, a promise of Oak survival under closed Sitka, or a universal light threshold. |
| **Near a gap edge / woodland margin** | Shade and shelter change gradually near the edge; local crown arrangement, aspect, neighbour trees, seed source and vegetation can alter outcomes. | A mandatory 5–30 m “Irish edge zone” or a binary magic distance. P6's distances are illustrative, not local calibration. |
| **More open ground / gap centre** | Light may be higher; exposure and competing vegetation can also be higher. A germinant is not yet a successful recruited tree. | Automatic success for light-demanding trees, or calling an extraction rack a regeneration strip. |

Evidence: Irish Cross & Collins 2017 [S1–S2], Teagasc 2024 [S4], and the 2024 silvicultural taxonomy [S11]. In a 0.64 ha authored grid, these conditions are **not automatic categories based only on distance**. A player display claiming *this particular point* is sheltered, edge or open must inspect the actual geometry/crown/light representation and whether that is a game-defined property. The 2022 Killarney evidence [S7] directly cautions that gaps alone did not overcome browsing-related recruitment failure at its study sites. Do not generalise this site finding to all Irish forests.

**Proposed explanatory copy:** “Under the canopy, shade can limit growth. At gap edges, light and shelter vary; open spaces may get more light but also more competing plants. Check the young trees and site conditions over time.” **Verdict:** COPY READY as *general educational help*, not a claim that the current UI has measured site-specific edge categories.

## 6. Annual Review and time-dependent evidence

- **Current-year recorded fact:** “Young growth was recorded in {currentCount} areas of the woodland in year {year}.” Only say this with a verified ecological snapshot for that year. A live/current HUD may say “Young growth is recorded in {count} areas.”
- **Recorded count comparison:** “Recorded areas with young growth: {currentCount} this year; {priorCount} in year {priorYear}.” Only when both **saved year-stamped** snapshots exist and use comparable geometry, model and counting semantics. It is a comparison of recorded *totals*, not evidence of specific individual or cohort survival.
- **Derived difference:** “The number of recorded areas is {+/-delta} compared with year {priorYear}” is permissible **only** as an arithmetic count comparison, with derivation labelled and same metric, **not** “{delta} new sites regenerated.” A location could disappear in the same period that another becomes occupied.
- **Newly recorded locations:** **NEEDS GAME DATA**: would require reliable saved per-location identity/occupancy for both dates and a defined treatment of planted overlaps, reloads, geometry and promoted trees.
- **Older surviving cohorts / ageing juveniles:** **NEEDS GAME DATA** before any historical claim: verify which cohort establishment-year/age, growth and survival records actually persist per year. A current age is not by itself proof the yearly stage was recorded historically.
- **Protection effect / browsed this year:** **NEEDS GAME DATA** for historical claims unless the saved Annual Review directly records the field/event; do not infer older-year browse injury from a present/current assessment or a current shelter. An installed protection work event alone does not prove efficacy.
- **Explanation/provenance:** keep *recorded fact*, *current observation*, *derived comparison*, *game explanation* and *uncertainty* distinct, consistent with accepted S1-C history semantics. Do not invent past measurements.

## 7. Three copy sets for design-only approval

These are proposals, **not permission to edit player UI**. “COPY READY” means content can proceed to a separate approved copy change after string/source inventory and normal verification, not that the underlying gameplay has been certified.

### A — Objective and progress

- **Headline:** **Encourage woodland regeneration** — **COPY READY / NEEDS PRODUCT DECISION** (purpose label; the user has not approved final exact copy).
- **Metric:** **Areas with young growth: {current} / {target}** — **COPY READY** for present code's location count, with explanation. Explicit fallback **“Areas with recorded young growth: {current} / {target}”** — **COPY READY** if the user prioritises precision over brevity.
- **Explanation:** “The game counts woodland locations where natural young growth is represented or a living planted young tree is recorded. It does not count individual seedlings or guarantee their survival.” — **COPY READY** for conceptual help; do not claim the UI's screen space supports it before layout review.
- **Avoid:** “12 trees established”, “12 regenerated patches”, “12 areas restored”, “12 hectares” and all generic statements of durable regeneration success.
- **Existing target:** **12 occupied locations for Enlarged80** remains fixed; no metric-policy change.

### B — Annual Review

- **Current-year statement:** “Young growth was recorded in {current} areas of the woodland this year.” — **COPY READY**, conditional on existing recorded snapshot.
- **Optional comparison:** “The previous recorded total was {previous} areas in year {previousYear}.” — **COPY READY** **only if** both year-stamped snapshots are available and semantically comparable; otherwise **NEEDS GAME DATA**.
- **Do not say:** “We gained {current - previous} new regeneration areas,” “{n} seedlings survived last year,” “browsing caused {n} losses” without corresponding saved transition and cause evidence.

### C — Inspect / Help

- **Origin:** “Natural regeneration: young trees arising from seed. Planted trees: nursery stock placed by the player.” — **COPY READY** as generic explanation. **NEEDS GAME DATA** to attach either label automatically to a particular inspected group/individual without verifying its origin field and exact UI binding.
- **Conditions:** “Beneath retained trees, light may be limited. Around openings, light and shelter vary; more open ground can also favour competing vegetation.” — **COPY READY** as general help; **NEEDS GAME DATA** to label specific points “edge” or “open” as measured game state.
- **Survival caveat:** “Young trees may still be lost or delayed by shade, competition and browsing. Check their condition after advancing time.” — **COPY READY** as general caution; not a claim that all factors were recorded for a historical individual.
- **No authored gap bands/strips or new workflows** are proposed.

### D — separate planted-Oak/Beech objective wording check

- **Current label:** “Establish planted Oak/Beech” — **AVOID** as an evidence statement about the current predicate.
- **Conservative concept:** **“Complete Oak/Beech planting”** — **NEEDS PRODUCT DECISION** because existing completion additionally requires *species presence*, and this shorter label alone hides that condition.
- **Predicate-faithful explanatory version:** “Complete an Oak/Beech planting and keep Oak/Beech present in the woodland.” — **COPY READY as an explanation of the existing predicate**, but **NEEDS PRODUCT DECISION** for the preferred player wording; does not assert the *planted* individual remains alive.
- **No rule change:** Do not redefine success, save state, species IDs or objectives here.

## 8. Cannot claim without more evidence or an accepted new policy

1. **Stems/ha, seedling density, exact natural seedling count, measured biodiversity, or individual natural-seedling positions** from abstract regeneration occupancy.
2. **Hectares successfully regenerated, continuous area coverage, connected regeneration patches, or percent of the 0.64 ha actually renewed** from the 12-location target.
3. **Sustained regeneration, forest renewal, established future overstorey, a guaranteed second cohort or survival probability** from a present representation count.
4. **Every planting being environmentally beneficial**, or Beech being “native Irish oakwood” by virtue of being planted; native-site guidance and mixed production forestry have distinct aims.
5. **Natural vs planted unique occupied-location totals** by summing per-species counters or subtracting planted totals from overall occupancy.
6. **Newly colonised locations, exact per-year survival/transition, browse damage to a historic individual or specific protection efficacy** without the needed persisted identity/history.
7. **A universal Irish seedling/sapling height cutoff**: S7's field-study categories differ substantially from S10's UK guidance.
8. **Specific Irish light thresholds, gap widths, edge-distance bands, establishment coefficients or browse probabilities** imported from P6 or foreign literature.
9. **A harvesting extraction rack being a forestry regeneration strip**, or a drawn gap edge being a validated ecological band.
10. **“Successful Oak establishment”** as the observed consequence of only a completed planting action plus any living/represented Oak somewhere.
11. **Universal policy that natural regeneration always beats planting**: S1's ecological preference is situated in native-woodland management and is explicitly conditional on suitable species, provenance, site and attainable regeneration.
12. **Guaranteed natural regeneration after thinning**: long-term Irish transformation trials and monitoring caution against that inference.

## 9. Bounded corrections / checks for project research composites

| Current or possible statement | Disposition | Reason / action |
|---|---|---|
| “Natural regeneration” used for a cell containing an exact planted juvenile | **CORRECT player copy** | Distinguish natural from planted provenance; combined umbrella is “young growth recorded”. [S1, S6, S12] |
| “Seedling present” interpreted as regeneration success | **QUALIFY** | Irish Killarney survey separates seedlings from persistent saplings and reports poor transition in browsed gaps. [S7–S8] |
| “Sapling” treated as one global fixed metric | **CORRECT** | S7 and S10 use different size categories. Do not hardcode UK or survey research thresholds as Irish standards. |
| Beech described as inherently beneficial “native regeneration” | **CORRECT context** | Beech is non-native in native-oakwood restoration guidance, but an authored species in Scenario One production-forest teaching. Context is decisive. [S1, S2] |
| 2024 Teagasc seedling-development articles treated as fully Irish measurements | **DOWNGRADE source class** | Much of their detail is synthesis citing older UK/continental literature. Retain qualitative Irish-practice interpretation, not numerical Irish calibration. [S3–S4] |
| 2017 NPWS/Forest Service guide treated as current binding regulation across all CCF | **CORRECT authority label** | Joint official management guideline for *native woodland*, not automatically the current scheme's enforceable requirements. Current DAFM/Teagasc scheme documentation is the correct place to verify grant rules. [S1, S6] |
| Teagasc 2024 deer distribution/predation examples used as a 2026 deer-population census | **DEFER / RE-VERIFY** | Teagasc reuses older study citations; not verified contemporary local abundance. Not needed for copy. |
| Understorey competition stated as “remove all bramble to save seedlings” | **CORRECT** | Irish native woodland guideline explicitly records cases of bramble protection from browse, so vegetation control needs context; CCF's present model2 is simpler gameplay calibration. [S2, S4] |
| P6 gap bands or strip sizes used as mandatory CCF access-rack or Irish regeneration dimensions | **REJECT TRANSFER** | European silvicultural classification, not a local survey; extraction racks have a different operational function. [S11] |

This is a **bounded audit** of high-relevance terminology and example claims. It is **not** a line-by-line validation of all four composite files or every current legal-policy claim. No composite was edited. Any eventual canonical research-index correction requires Manager review of the exact affected lines and authority.

## 10. Required implementation-worker source checks before copy changes

**Mandatory inspection only — no changes authorised by this report:**

1. Re-fetch exact candidate branch and current accepted spec/context; record file/branch versions and one writer/lock. Check latest Manager copy freeze and pending playtest status before any task release.
2. Inspect every consumer of stable **regeneration** objective ID and **occupiedRegenerationCells** metric, including *ScenarioOneObjectives.cs*, *ScenarioOneDefinition.cs*, objective/HUD/help screen strings, *ScenarioOneManager.cs* ecological snapshots, Annual Review and Century Review reference-comparison strings. Do not assume UI renders these names verbatim until traced.
3. Verify **RecordEcologicalSnapshot** aggregation and cohort **HasRegeneration** semantics; check whether empty/thresholded/legacy cohorts affect current presence and which world versions use which algorithm. No empirical stem counts.
4. Trace **ScenarioEcologicalSnapshot** and **ScenarioSpeciesOutcome** serialization through *ForestSaveController* and *ForestSaveData*; validate which year totals survive save/load, whether an existing year can be overwritten, old schema handling and the reference archive.
5. Inspect **per-year location identity availability**, natural/planted origin and planted overlap; do not invent a new-locations count from totals. Count rules, geometry-effective targets and save/hash compatibility remain fixed.
6. Trace work resolved events, orders, and introduced-species predicate for Oak/Beech. Any correction of completion semantics is **outside copy scope** and needs product decision.
7. Inspect *ScenarioOneManager.AdvancePlantedJuveniles*, **ForestRegenerationCohort** origin/age fields, individual *PlantedJuvenile* survival/promotion, shelters and persisted damage records before attaching historical “survived/browsed/protected” language.
8. Check all presentation sizes, truncation, accessible wording and reference-context labels at 1280×720 and 1920×1080 in a properly released Unity lane; source review does not prove rendering.
9. Preserve **Legacy40**, frozen **Reference Future v1**, save/backward compatibility and the full demo/Windows-build gate. Do not change numeric targets, stable IDs, management placement, Work Plan or vegetation/rack mechanics.
10. Cross-check product-owner final choice between **“Areas with young growth”** and more explicit **“Areas with recorded young growth”**. The original 11 November Manager wording proposal was explicitly frozen for production pending design approval.

## 11. Final assessment and handoff boundary

**Research verdict:** **SUPPORTED FOR BOUNDED DESIGN REVIEW**, not automatic UI approval. Evidence is strong enough to use honest forestry terms and avoid claims of hectares, sapling counts or proven recruitment. The source inspection grounds the combined presence metric and identifies a genuine label–predicate mismatch for planted Oak/Beech.

**No implementation/acceptance claim:** Did not edit Unity/C# or source specs, alter objectives/targets or saves, run Editor/playtest, or claim dashboard and Drive sync. No new product decision is made. A later Manager should review this report, choose final copy and determine whether a small presentation-only packet is appropriate **after the active documentation freeze and personal playtest conditions permit**.

**Output:** [GitHub research report](https://github.com/jermur788/CCF/blob/docs/research-ccf-young-growth-language-20261112-v1/Docs/Research/ResearchIntake20261112/YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md). Remote readback and final claim closure to be recorded on [#388](https://github.com/jermur788/CCF/issues/388).
