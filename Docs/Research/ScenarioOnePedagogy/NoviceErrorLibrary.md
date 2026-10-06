# Novice error library (Workstream C6)

**Status:** NEW PROPOSAL [INF]. The errors come from practitioner training material [PRAC §11.4, §12.2] and this audit. Feedback values marked [HARNESS] come from `Evidence/` (Scenario One Year-0 stand, RNG 1 / regeneration 1).

**Rule for every entry:** feedback states facts and consequences. It never says "wrong". Where a consequence is not simulated, the entry says so. A practice-mode panel (`MarkingAssessmentFramework.md`) is the main delivery surface; the real-forest Work Plan shows the same facts.

Columns: **ERROR · WHY A NOVICE DOES IT · IMMEDIATE FEEDBACK · LONG-TERM CONSEQUENCE · WHAT TO LEARN · SIM SUPPORT**.

---

### E1 — Removing every suppressed tree ("cleaning up")

- **Why:** small, crowded stems look weak and untidy, and the HUD calls them "crowded".
- **Immediate:** harness T1 removed 84 small crowded stems (4.90 m³). Crop-Tree competition fell 6.09 → 5.08 (−17 %) and predicted Crop-Tree growth rose 9.3 %. T2 removed only 30 *actual competitors* (5.19 m³) and gave −20 % competition and +11.7 % growth [HARNESS]. T1 cost €2,346.95 net.
- **Long-term:** T1 Crop-Tree DBH at Year 20 was 29.51 cm vs T2 29.73 cm, with 54 more trees removed to get there [HARNESS].
- **Learn:** suppression alone is not a reason to remove a tree. Many small removals buy little release and still cost a contractor visit [PRAC §11.4, §12.2].
- **Sim support:** SUPPORTED. The *bole-protection* and microclimate value of small trees is **not** simulated, so do not claim it.

### E2 — Cutting a neighbour that is not a real competitor

- **Why:** "it is next to my Crop Tree".
- **Immediate:** the release readout shows that tree's share of the Crop Tree's competition. A small stem 3 m away contributes far less than a large stem 4 m away (Hegyi term ∝ DBH ratio ÷ distance). See `PositiveSelectionTeaching.md` for real ids.
- **Long-term:** Crop-Tree increment barely changes.
- **Learn:** judge a competitor by size *and* closeness, not by adjacency alone.
- **Sim support:** SUPPORTED for size and distance. Crown overlap and crown class are not separate states.

### E3 — Protecting the largest tree regardless of form

- **Why:** "biggest is best".
- **Immediate:** cannot be shown. There is no stem-form state.
- **Long-term:** cannot be shown.
- **Learn:** in real forests a Crop Tree is chosen for vigour, form and crown, not size alone [PRAC §12.2].
- **Sim support:** **NOT SUPPORTED.** Teach with one honest sentence: "This game does not yet model stem straightness or branching; in a real forest you would check them." Do not build an exercise around it.

### E4 — Thinning too heavily

- **Why:** a bigger cut releases more and pays more.
- **Immediate:** harness T3 (87 trees, 13.52 m³): Crop-Tree growth +30.8 %, net −€1,871 (the least loss: the fixed minimum is spread over more timber). Retained basal area falls from 43.3 to about 30 m²/ha; the largest opened patch covers 12 cells [HARNESS]. The wind diagnostic rises.
- **Long-term:** Crop-Tree DBH at Year 20 is 31.33 cm (highest); regeneration 46 cells; standing capital is the lowest of all treatments [HARNESS].
- **Learn:** a heavy cut is not "wrong". It trades capital, cover and exposure for faster release and fewer visits. Practitioners warn that abrupt release can bring form problems, wind exposure and vegetation flushes [PRAC §12.2; EMP §6 gap-edge quality].
- **Sim support:** PARTIAL. Windthrow, gap-edge timber quality and epicormic shoots are **not** simulated. The wind number is a diagnostic only.

### E5 — Pursuing regeneration at the wrong stage

- **Why:** "CCF is about regeneration".
- **Immediate:** an opening made for regeneration at Year 0 raises light, but the Sitka are only just starting to seed (maturity 20 → 30 years).
- **Long-term:** concentrated gap T5 had 23 regenerating cells at Year 1 vs 17 for T2 (same volume). By Year 20 both have 40 [HARNESS]. In this stand, regeneration arrives either way.
- **Learn:** early interventions are mainly about quality and stability. Regeneration-oriented openings make more sense once seed trees are mature and the canopy structure is ready [PRAC §11.1, §13.1].
- **Sim support:** SUPPORTED.

### E6 — Clearing vegetation everywhere

- **Why:** the clearance square appears on every ground glance; brambles look like a problem.
- **Immediate:** contractor cost per 5 × 5 m cell (`removalBaseMinutes` + density-based minutes at the contractor rate), and the affected list shows the **tree seedlings and saplings** that will also be removed.
- **Long-term:** vegetation regrows; regeneration in the cleared cells restarts from zero.
- **Learn:** clearance is a targeted tool with costs. In this version it does **not** help seedlings grow, because weed competition is not simulated.
- **Sim support:** PARTIAL (cost and removal are real; the benefit is not modelled).

### E7 — Planting where adequate regeneration already exists

- **Why:** "planting = doing CCF"; the completion objective requires planting.
- **Immediate:** the ground report shows existing regeneration. Planting clearance in the same spot reduces existing cohort density.
- **Long-term:** planted Sitka is not offered (the nursery sells broadleaves only), so this error in Scenario One is really *planting broadleaves where light will not let them grow* (Oak needs ≥ 0.20 light to grow into the canopy).
- **Learn:** plant to add what natural regeneration cannot supply, where light and protection allow [PRAC §11.8].
- **Sim support:** SUPPORTED (light thresholds, cohort reduction).

### E8 — Ignoring browse failure

- **Why:** the browse band is "low" and nothing visibly dies.
- **Immediate:** the ground report says "Low browse risk — unprotected" for an unsheltered oak.
- **Long-term:** unprotected oaks are held below the 1.8 m escape height longer. Completion evidence shows sheltered oaks taller after 4 years [LOG P8].
- **Learn:** browsing mostly *delays* palatable species; protection is a cost/benefit choice [EMP §4.2].
- **Sim support:** PARTIAL (fixed pressure; no form damage).

### E9 — Ignoring the contractor minimum

- **Why:** marking a handful of trees "to be gentle".
- **Immediate:** the Work Plan line "Small-job minimum (visit minimum €2,500)". Harness T4 (15 trees) earns little timber income and nets −€2,365.54 [HARNESS].
- **Long-term:** cash constrains later interventions; repeated tiny visits are expensive.
- **Learn:** operations have fixed costs. Combine work into fewer, well-planned visits, while still keeping each intervention light [PRAC §9 contractor issues].
- **Sim support:** SUPPORTED.

### E10 — Harvesting a Crop Tree too early

- **Why:** it is the biggest and most valuable tree now.
- **Immediate:** the Fell mark replaces the Crop Tree mark (they are exclusive, D-013). The release readout loses that Crop Tree.
- **Long-term:** the stand's best-growing stem is gone; its increment no longer accrues.
- **Learn:** a Crop Tree is an investment; harvest it when it reaches its purpose [PRAC §11.5 step 5].
- **Sim support:** SUPPORTED (increment). "Target diameter" is not a game rule; do not invent one.

### E11 — Keeping a Crop Tree forever despite maturity

- **Why:** "never cut the good trees".
- **Immediate:** nothing.
- **Long-term:** at high DBH its growth slows (size factor `1 − DBH/maxDBH`), while its timber value and its seed role grow.
- **Learn:** CCF includes harvest; a mature Crop Tree can be harvested when its role is fulfilled [PRAC §11.5].
- **Sim support:** PARTIAL: Scenario One's 100 years reaches about 40–50 cm DBH; no quality premium exists.

### E12 — Judging success only by harvested volume

- **Why:** volume and money are the most visible numbers.
- **Immediate:** the residual-stand block puts *what remains* next to *what was taken*. T2 and T5 remove the same volume (5.19 vs 5.22 m³) but leave different forests: 23 vs 11 cells touched; largest opened patch 3 vs 9 cells [HARNESS].
- **Long-term:** different Crop-Tree release, light pattern and regeneration timing.
- **Learn:** the forest left behind matters [PRAC §2.1, §5.2A].
- **Sim support:** SUPPORTED.

### E13 — One intervention, then nothing for 25 years

- **Why:** the scenario can be completed that way.
- **Immediate:** —
- **Long-term:** the release fades and light in opened cells falls back (T2 light 0.103 at Year 10 → 0.063 at Year 20) [HARNESS].
- **Learn:** CCF is repeated management (O2).
- **Sim support:** SUPPORTED.

### E14 — Reading the stand-wide forecast as the Crop-Tree effect

- **Why:** the current forecast line says "growth +y %" for the whole stand.
- **Immediate:** T1 shows **+12.2 % stand growth** but only +9.3 % Crop-Tree growth; T2 shows +6.7 % stand but +11.7 % Crop-Tree [HARNESS].
- **Learn:** ask "growth of *what*?" — of the trees you chose to favour.
- **Sim support:** SUPPORTED. This is an existing **UI** issue (Packet 2).

### E15 — Using the map to decide what to cut

- **Why:** dark cells look like problems.
- **Immediate:** the map cannot mark or cut (already enforced), and its help says so.
- **Learn:** the map finds where to look; the decision is made at the trees.
- **Sim support:** SUPPORTED (existing behaviour).

---

## Coverage

| Error | Immediate signal exists now? | Needs packet |
|---|---|---|
| E1, E2, E12, E14 | Partly (forecast line, Work Plan) | Packet 2 (Crop-Tree release readout), Packet 3 (residual block) |
| E4, E9 | Yes (Work Plan money, forecast wind) | Packet 3 for capital |
| E5, E13 | No | Packet 5 (history) |
| E6, E7, E8 | Partly | Packet 1 (copy), Packet 5 (diary) |
| E10, E11 | Partly | Packet 2 |
| E3 | Not simulated | none (honest sentence only) |
| E15 | Yes | none |
