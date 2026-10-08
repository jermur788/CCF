# Manager handoff — forestry scenario progression and learning curriculum

**Status:** design study; every recommendation below is a PROPOSAL until the user accepts it.

## What was done

- Read canonical context at `341ccbf` (identical `Docs/Project/` at main `869ee92`), the forestry-expansion proposals at `22d6b21`, the Scenario One readiness research (completion, second intervention, marteloscope, post-scenario roadmap, P5/P6 packets), the P2 record on main, the Species Evidence Pack v1 and the Irish economics report.
- Wrote this docs-only study. **No code, no Unity, no canonical-file edits, no merge.**
- Verified: main SHA; that `Docs/Project/` is unchanged between `341ccbf` and `869ee92`; that P4–P7 feature names do not appear in main's C# (search only). Everything else about implementation comes from canonical docs, not from running code.

## Key findings for the manager

1. **The manager's suspicion is right**: the expansion roadmap's Scenario Two (mixed transformation) needs the whole W0–W3 programme before a second scenario exists, and mixes two new difficulties (species/site and reassessment).
2. **But Scenario Two should not repeat Scenario One's second cycle.** Scenario One already contains two cycles (PROPOSAL model D/P6). Scenario Two is more valuable as **reassessing someone else's forest**: reading management history, correcting earlier mistakes, deciding where not to work. That is a different judgement and buildable now.
3. **The expansion branch's species premise is outdated**: it says the project has no non-Sitka conifer evidence; Species Evidence Pack v1 reports Irish GROWFOR models for Douglas fir, Norway spruce and Scots pine (conditional on extraction).
4. **`PostScenarioRoadmap.md` (readiness branch) recommends Stage 2 before Scenario Two**; that is superseded by the user's forestry-first direction and should be marked so.
5. **The forestry-first and sandbox-later directions are not yet in canonical files** (Game Brief at `341ccbf` has no such text). They exist in the packet and as proposed text on the expansion branch.

## Direct answers to the 17 questions

| # | Question | Recommendation |
|---|---|---|
| 1 | Scenarios before sandbox | Six core (+ B1 recommended, B2 optional); sandbox built after S6, unlocked after S5 |
| 2 | Two cycles in Scenario One? | Yes |
| 3 | Scenario Two on repeated intervention before species/site? | Yes, as reassessment of an *inherited* stand, not a repeat of S1's second cycle |
| 4 | Scots pine | Scenario 3 |
| 5 | Douglas fir / Norway spruce | DF: S3 as planting option (GROWFOR-gated), managed in S5. NS: S5 |
| 6 | Oak/beech as management species | S4 (regeneration), S5 (oak quality Crop Trees) |
| 7 | Alder/birch | Scenario 3 |
| 8 | Rowan/holly visible | Scenario 4 (central in B2) |
| 9 | Meaningful site variation | Scenario 3 |
| 10 | Pruning/timber quality | Basic pruning S2; quality state S5 |
| 11 | Browsing central | Scenario 4 (present at low level from S1) |
| 12 | Storms as teaching | Branch B1 after S2; background risk from S5 |
| 13 | Native woodland | Branch B2 after S4 (open: core) |
| 14 | Graduation | Scenario 5, The Irregular Forest |
| 15 | Sandbox unlock | Gates SB1–SB7 (build); S5 completion (player) |
| 16 | Marteloscope | Plan comparison in all scenarios; Training Stand after S1 |
| 17 | First external test beyond S1 | Scenario 2 (B1 for small-owner testers) |

---

FORESTRY SCENARIO CURRICULUM — MANAGER HANDOFF

RECOMMENDED NUMBER OF CORE SCENARIOS:
- Six core scenarios before the sandbox is built, plus two optional branch scenarios (one after Scenario 2, one after Scenario 4). The sandbox unlocks to players after Scenario 5.

RECOMMENDED SEQUENCE:
1. First Steps in CCF (existing Scenario One; two cycles in the player's own forest)
2. The Inherited Stand (reassessing a part-transformed Sitka stand; history; not intervening; pruning as investment)
   - Branch B1: After the Storm (Sitka windthrow aftermath; salvage vs retention vs regeneration vs stability)
3. Right Tree, Right Place (species strategies × site axes; Scots pine, birch, alder; Douglas fir as a planting option)
4. The Next Generation (regeneration diagnosis with evidence but no verdict; browsing central; fencing vs shelters)
   - Branch B2: The Old Oakwood (native woodland; restraint; open habitat; deadwood; browse at scale)
5. The Irregular Forest (CCF graduation; maintain irregularity; quality-led selection; strategic economics)
6. The Farm Woodland (integrated multi-compartment holding; no focus lesson; owner's own priorities)

SCENARIO ONE ROLE:
- Teach seeing, positive selection, release, light-driven regeneration, planting/protection and coming back once to the player's *own* decisions. Keep both management cycles (do not move the second cycle out). Storms stay off.

SCENARIO TWO ROLE:
- Teach reassessment of an inherited, part-transformed stand: read management history from records and from the forest, judge which past decisions to continue or correct, release advance regeneration, record "no work needed" where appropriate, and meet pruning as a long-term investment. Same species, uniform site, current ecology. Buildable with a small scenario-package extension.

SECOND-INTERVENTION RECOMMENDATION:
- Option C: Scenario One contains two cycles (consequences of my decisions); Scenario Two is an inherited stand (diagnosing someone else's decisions). Scenario Two comes before any multi-species expansion because it isolates reassessment as a skill, tests transfer early and is cheap to build.

SPECIES INTRODUCTION ORDER:
- S1: Sitka (crop); oak, beech (planting stock).
- S3: Scots pine (existing component), downy birch (pioneer, ordinal), common alder (wet niche, ordinal), Douglas fir (planting option on the sheltered free-draining slope; GROWFOR-gated).
- S4: oak and beech as managed regeneration; rowan (browse indicator, ordinal); holly (understorey, ordinal).
- S5: Norway spruce; Douglas fir merchantable; oak quality.
- Not before S5 (likely post-core): western hemlock, western red cedar (Irish quantitative research required). Hazel only in B2. Ash and Japanese larch never as planting options.

SITE COMPLEXITY INTRODUCTION:
- S1 and S2 uniform (S2's lesson is temporal). S3 introduces three separate ordinal axes (moisture/drainage, fertility, exposure) as patches. Present thereafter; full variation across compartments in S6.

REGENERATION/BROWSING PROGRESSION:
- S1: light, seed present, planting, vegetation (Model 2) and low browse with shelters — all already implemented. S2: advance-regeneration release and judging past clearance/planting. S3: seed source and species × site. S4: multi-cause diagnosis with evidence, not a verdict; browsing central (high pressure, species palatability, fencing). S5: maintaining recruitment in an irregular canopy. B2: browse at landscape scale.

TIMBER QUALITY / PRUNING:
- S2: basic pruning as investment using existing pruning work and clear stem, with an honest "no premium configured". S5: per-tree quality state (form class, browse history) and quality-led selection; assortments for new species without fabricated prices. Optional: pull a visible form class forward to S2 (open decision).

DISTURBANCE / STORMS:
- Off in Scenario One (optionally an opt-in "continue with storms on" after completion, if the activation policy allows). Dedicated branch B1 after Scenario 2 using Storm Model 1 (frozen profile) and an authored disturbed start generated by the storm model. Background, unannounced risk from Scenario 5 onward.

NATIVE WOODLAND SCENARIO:
- Branch B2 after Scenario 4 (middle-late). Designed against "native = good": a valuable open glade not to plant, holly as normal, deer making planting futile, deadwood as habitat, the possibility that nearly no work is best. Needs open-habitat state and standing deadwood (new). Open decision: promote to core.

CCF GRADUATION SCENARIO:
- Scenario 5, The Irregular Forest: maintaining an established irregular, multi-aged mixed forest across at least three cycles while taking income and keeping recruitment in several layers.

FINAL INTEGRATED CHALLENGE:
- Scenario 6, The Farm Woodland: two or three compartments with different sites, species and histories; finite cash and owner time; storms and browse as background risk; difficulty from competing priorities, not scarcity or random disasters; completion reviews outcomes against the owner's own recorded priorities.

SANDBOX UNLOCK GATE:
- Build after Scenario 6, when: the stand generator and site map have authored S3, S5 and S6; exposed species are calibrated at the depth exposed (uncalibrated species absent); site system active; more than one credible economic strategy; a combination guard excludes unmodelled states; a tree-count performance budget is enforced. Players unlock it after completing Scenario 5.

MARTELLOSCOPE POSITION:
- Plan comparison (no time advance) inside every scenario from Scenario 1 (after P3). Training Stand mode v1 unlocked after Scenario 1 and built with Scenario Two's package machinery; species exercises after S3; quality exercises after S5. No score; no time-advanced copy of the player's own forest (oracle under fixed seed).

SYSTEMS NEEDED BEFORE SCENARIO TWO:
- Scenario One P-series settled (especially P4 history and P6 second-cycle completion).
- Scenario package v0: scenario id in the save (next free version), start-from-snapshot, inherited-history boundary, Reference Future isolation, minimal scenario select.
- Deterministic authoring harness that produces the start snapshot by running a scripted previous-owner plan through the real engine.
- Per-scenario completion profile and assistance profile.
- "No work needed here" saved decision record.

SYSTEMS THAT CAN WAIT:
- Stand-table/stand generator and site map (Wave S3); W0 multi-species foundation (S3); fencing gameplay and diagnosis tiering (S4); quality state, layer diagnostics and new-species markets (S5); owner's-plan record (S6); open habitat, standing deadwood, hazel (B2); sandbox UI (after S6); WH/WRC (after research).

TOP 5 IMPLEMENTATION DEPENDENCIES:
1. Scenario One P-series (P3–P6) settled and integrated.
2. Scenario package v0 with scenario id in the save and start-from-snapshot.
3. Per-scenario completion and assistance profiles.
4. W0 foundation + site map + GROWFOR extraction research (long lead for Scenario 3; start the research now).
5. Quality state (Scenario 5) and fencing gameplay (Scenario 4), each needing its own accepted packet under D-044.

TOP 5 DESIGN RISKS:
1. Scenarios become "levels with a new species" — species added as a palette rather than because they change a decision.
2. Assistance fades into opacity (information hidden) or stays trivial (players follow a learned label); requires the "evidence, not verdict" split and playtests for both failure modes.
3. Small-holding economics teach "never thin": every thinning on 0.16 ha loses money under the €2,500 [C] minimum; holding size for S2+ is an open decision.
4. Deterministic fixed-seed scenarios become memorisable walkthroughs and authored snapshots silently stay on old model versions; needs a per-package regeneration/anchor policy and possibly replay-seed variation.
5. Uneven species evidence produces false precision or "native = good" framing; foresters reject trajectories (mitigate with ordinal-only roles, evidence labels in UI, and forester review gates at S3, S5, B2).

CANONICAL DECISIONS RECOMMENDED:
- (User direction, to record) Forestry is developed as a multi-scenario simulator before broad Stage 2; scenarios are independent forests; the player carries knowledge, not the forest; the sandbox comes after the structured scenarios.
- (Proposed) Adopt the six-scenario core sequence and two branches above as current direction, not fixed scope.
- (Proposed) Scenario One keeps two management cycles; Scenario Two is The Inherited Stand.
- (Proposed) Progression = scenario access, knowledge pages, assistance defaults and information complexity; never capability or tool unlocks (extends D-011).
- (Proposed) Assistance removes advice, attention direction and interpretation, never information; late scenarios show evidence, not verdicts.
- (Proposed) Completion per scenario = separated cycles + guardrails held + focus lesson evidenced from saved data + observation time; no universal score or target forest shape.
- (Proposed) Scenario Two uses a scenario package generated by the existing engine; the general stand-description format is deferred to Scenario Three.
- (Housekeeping) Mark `PostScenarioRoadmap.md`'s Stage-2-first order as superseded; note that the Species Evidence Pack supersedes the expansion branch's "no conifer evidence" premise.

OPEN USER DECISIONS:
- Holding size for Scenario 2 onward (0.16 ha keeps thinnings loss-making; ~0.3–0.5 ha makes contractor timing a real choice; cost is tree count/performance).
- Whether Scenario 2 reuses the Scenario One terrain (cheap; may read as "the same place") or needs a new terrain (serialized-asset work).
- Whether a visible form class (quality state subset, save bump) is pulled forward to Scenario 2 or waits for Scenario 5.
- Native woodland as core or branch.
- Storm scenario as a branch after Scenario 2 (recommended) or as a core scenario.
- Whether Douglas fir appears in Scenario 3 (needs GROWFOR extraction in time) or waits for Scenario 5.
- Storm activation policy for Scenarios 5–6 (open D-050 item).
- Replay seed variation for completed scenarios (anti-memorisation) versus strict determinism.
- Whether grants ever appear (recommended: not before Scenario 6, as an explicit policy layer).

SUGGESTED NEXT IMPLEMENTATION PACKET:
- After P4 and P6 are integrated: **"SCN-0 Scenario Package v0 spike"** — scenario id in the save (next free version, missing → Scenario One), start-from-snapshot, inherited-history boundary in history views, Reference Future isolation, and a deterministic authoring harness that produces a test "inherited" snapshot from Scenario One's stand with a scripted previous-owner plan. Acceptance: Scenario One anchors unchanged; snapshot load deterministic across two processes; old saves load as Scenario One; no ecology change. In parallel, a no-code research packet: **GROWFOR extraction for Scots pine, Douglas fir and Norway spruce**.

STATUS:
READY FOR MANAGER REVIEW
