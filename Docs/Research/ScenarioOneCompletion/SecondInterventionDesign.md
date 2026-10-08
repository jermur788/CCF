# Second intervention — CCF as repeated management (Workstream M)

**Status:** design proposal. Updates `RepeatedInterventionDesign.md` (pedagogy branch) for Growth Model 1, Regeneration Model 2 and storms.

Scenario One must show that CCF is **repeated** management:

**first intervention → 5–10+ years → changed stand → second inspection → a possibly different treatment**

Today nothing asks for a second intervention (`ObjectiveDependencyGraph.md`).

## 1. Which signals could trigger "time to look again"?

| Candidate trigger | Authoritative? | Behaviour in this model | Verdict |
|---|---|---|---|
| Calendar (N years after the first thinning) | Yes (events) | Same for every plan | Floor only. Calendar alone teaches "thin every 5 years" |
| **Crop Tree competition back to its pre-thinning level** | Yes (Hegyi) | **Barely recovers.** Hegyi is a ratio of DBHs, so it hardly changes when all trees grow together. T2 Crop Tree CI: 4.90 (Y1) → 4.95 (Y5) → 5.01 (Y10) → 5.18 (Y20) [pedagogy harness, pre-Growth-Model-1] | **Reject** as the primary trigger: it would rarely fire |
| **Stand density reaching the self-thinning onset** (Growth Model 1 relative density RD ≥ 0.6) | **Yes** (`SitkaGrowthModel.RelativeDensity`; onset is a [C] calibration) | Differs by plan. Year-0 RD after each plan, and a rough projection to RD 0.6 at the unthinned growth rate [PROTO, INF]: none 0.435 → **~7 yrs**; T4 conservative 0.408 → ~8; T2 release 0.383 → ~10; T5 gap 0.377 → ~10; T1 clean-up 0.370 → ~10; T3 heavy 0.296 → **~15** | **Primary trigger.** Ecologically meaningful ("the stand is crowded enough that trees start to die from crowding"). Varies with what the player did |
| First self-thinning deaths after the first intervention | Yes (mortality cause/year) | Follows RD | Visible confirmation in the world (fallen logs) |
| Regeneration light-limited under canopy | Yes (`RegenerationDiagnosis` LightLimited; light < promotion minimum) | Fires where seedlings exist but the canopy has closed over them | **Secondary trigger.** Suggests a *different* treatment: releasing regeneration rather than Crop Trees |
| Storm opening | After storms (`stormEvents[]`, windthrow deaths) | Model-driven | **Event trigger** (forces re-inspection, not thinning) |
| Planting failure or browse damage | Yes (juvenile records) | Planted losses or browsed-last-year | **Event trigger** for a protection/re-planting rethink |

The RD projection uses one growth rate (unthinned, Growth Model 1, from Sol's stand-development CSV) for every plan. It is a ranking estimate, not a schedule. **Verify in Unity under the current stack before any calibration.**

## 2. Recommended trigger: "The forest has changed — inspect again"

```
WHEN  year ≥ firstThinningYear + 5                 (floor [C])
AND   ( stand RD ≥ 0.6                             (crowding returns)
     OR ≥ 3 cells with light-limited young trees    (regeneration under a closing canopy)
     OR a storm opened the canopy                  (after storms)
     OR planted stock lost ≥ 25 % since planting   (protection/planting rethink) )
THEN  Annual Review PLACES TO INSPECT leads with the reason;
      the progress panel opens stage "Second look" (ObjectiveRedesign.md S9).
FALLBACK  year = firstThinningYear + 12 → same prompt with reason "time has passed".
```

The prompt is **never "thin now"**. Copy: "Your forest has changed since your Year 2 thinning: trees have started to die from crowding in 4 cells. Walk the stand and look at your Crop Trees again."

Thresholds are [C]. Implementation reads only saved state and recomputed density.

## 3. What counts as the second intervention (completion evidence)

A **second management cycle** is complete when, in a year ≥ floor:

1. the player **re-inspects at least one Crop Tree**, or a regeneration site named in PLACES (evidence of looking: inspection events are UI events, not saved; see §5);
2. **and** resolves management work of any kind: thinning, regeneration release (felling over advanced young trees), planting/protection, clearance (Model 2);
3. **and** reads the following Annual Review.

There is no requirement on *how much* is cut, or even *whether* trees are cut, if another treatment is chosen. A deliberate **"No work needed this year"** decision is pedagogically valuable (pedagogy decision #24). It needs a saved event, so it waits for the save queue and is optional in v1.

## 4. Conceptual paths

| Path | First intervention | What changes (mechanism) | Likely trigger | Likely second treatment (examples, not prescriptions) |
|---|---|---|---|---|
| **Regeneration succeeded** | T2 release, Year 1–2 | Sitka seedlings in many cells; some bright openings | RD ≥ 0.6 around Year ~11; or seedlings light-limited | Release Crop Trees again **and/or** fell over light-limited advanced regeneration to free it |
| **Regeneration failed** (dark, no seed, or vegetation under M2) | Light thinning | Few seedlings; ground "Why" names light/seed/vegetation | Floor + RD, or failure of planting | Open more light where seed arrives; enrichment planting; under M2, targeted clearance where young trees are short and cover dense |
| **Browse high** (if pressure raised in a variant) | Any + unsheltered planting | Planted oak browsed repeatedly, still below 1.5 m | Planted loss / browsed-last-year | Shelters on new stock; accept slower broadleaves; no fencing (D-044) |
| **Storm opened the canopy** (after storms) | Any | Windthrow gap, deadwood, light; edge trees Exposed | Storm event | Salvage or leave; do *not* thin the new edge hard; regeneration in the gap |
| **Player thinned heavily** (T3) | 87 trees, CI −45 % | Low RD; bright openings; more ground competitors (M2); Exposed edges (storms) | RD late (~15 yrs); light-limited regeneration may come first; storms may come first | A **light** second touch, or none: deadwood, regeneration care. Teaches that heavy early work buys time and changes options |
| **Player thinned conservatively** (T4) | 15 trees, CI −11 % | RD returns quickly | RD around Year ~8–9 | A further release of the same Crop Trees: positive selection repeated |
| **Player did nothing** | — | RD 0.6 by Year ~7; crowding deaths; Crop Tree competition unchanged | RD (no floor without a first thinning: use Year 8 as the soft prompt) | First intervention late. Still allowed; the cost is visible as a denser stand and natural deaths |

## 5. Implementation notes

- RD per year: computable from current trees each year. For history it can be recomputed only for the current year; the trigger needs only the current year.
- "First thinning year": derivable from `WorkResolved` FellTree events.
- Re-inspection evidence: inspection is not saved. Options: (a) per-session UI observation (lost on reload: acceptable for a hint, **not** for a completion condition); (b) a saved "stage progress" record (save change, pedagogy decision #8). **Recommendation:** stage progress per forest (save field, queued after M2 and storms). Until then, completion uses resolved work and review reads only.
- Save impact: none for the trigger. One optional per-forest progress record for stage evidence.
- Anchor impact: changes the completion anchor only when bound to completion (`ScenarioCompletionDesign.md`).

## 6. Calibration checks (Unity, before acceptance)

1. Under the current stack, record RD by year for T0–T5 at Year 1 and the first year RD ≥ 0.6. Check that the ranking matches §1.
2. For each path, confirm the trigger fires between Year 6 and Year 20, so a second cycle fits before Year 25.
3. Confirm the fallback (first + 12) fires for heavy plans in which RD stays low.
4. Under storms: a storm trigger does not demand a felling.
