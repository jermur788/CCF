# Scenario dossiers

**Status:** RECOMMENDATION. Starting conditions are *design intent*, not authored parameters. No coefficients are proposed. Where a dossier depends on a system that does not exist, the dependency is named.

Each dossier uses the same fields. "Failure" means poor management that remains playable unless stated otherwise.

---

## Scenario 1 — First Steps in CCF (exists; being completed)

| Field | Content |
|---|---|
| **Starting forest** | IMPLEMENTED: even-aged Sitka, 336 trees, 0.16 ha bounded property, Irish site Class III, uniform site, low browse (0.2), bramble/bracken (Model 2), storms off (Model 0) |
| **Knowledge assumed** | None. No forestry, no Unity/game-genre assumptions beyond walking |
| **Main question** | *Which trees do I want to keep, and what must I remove to help them — and what happened when I came back?* |
| **New concepts** | C1–C6, C10–C13, C22 basic; C7 in own forest |
| **Older concepts retested** | — (cycle 2 retests cycle 1 with reduced help) |
| **Species / site** | Sitka; oak and beech as planting stock; uniform site |
| **Economic complexity** | Contractor harvest with small-job minimum; owner time for planting/shelters; thinnings on 0.16 ha lose money (research finding, `ScenarioCompletionDesign.md` §1). Teach as a fact of small woodlands, not a puzzle |
| **Disturbance / risk** | None by default. Storms remain off (D-050 dormant; activation policy open) |
| **Assistance** | High: terminology, first-use help, Crop Tree competitor reasoning (P2), residual-stand summary (P3), ground "why" readout with a named limiting factor, progress panel (P5 PROPOSAL). Cycle 2: the second-look prompt names *what changed*, never *what to do* |
| **Game explains** | What a Crop Tree is; how competition is calculated (as a description); why light matters to regeneration; what shelters do; what the contractor minimum means |
| **Player must infer** | Which trees are good Crop Trees; which neighbours matter; how much to remove; whether the result was what they intended |
| **Poor management looks like** | Thinning from below only (removing suppressed trees that were not competing); clear-felling a block; never returning; planting into dark ground; unprotected oak repeatedly browsed |
| **Success looks like** | Crop Trees with lower competition and growing crowns; light openings with regeneration; a second, possibly different treatment after the stand changed |
| **Forest state that shows learning** | Felled stumps clustered near Crop Trees (not spread evenly through small stems); regeneration in the opened cells; planted broadleaves alive ≥ 5 yrs; continuous cover throughout |
| **Unlocks** | Scenario 2; Training Stand mode; plan comparison is already available inside the scenario |
| **New technical systems** | None beyond P-series (P3–P6 PROPOSALS) |
| **Systems reused** | All current systems |

---

## Scenario 2 — The Inherited Stand

| Field | Content |
|---|---|
| **Starting forest** | A Sitka-dominated stand roughly 15–20 years after a previous owner started transformation. Deliberately mixed legacy: (a) a well-released group of Crop Trees with good crowns; (b) a patch thinned from below, where the dominant competitors were left and the Crop Trees are still crowded; (c) an over-opened corner now under heavy bramble with failed planting; (d) advance Sitka regeneration under a closing canopy; (e) a few planted oak, some sheltered and above browse height, some unsheltered and repeatedly browsed; (f) a section that is fine and needs nothing; (g) old brash, stumps and fallen deadwood showing where work happened. Uniform site (same Class III, neutral site), same species as Scenario 1. Holding possibly larger than 0.16 ha (open decision; see §Economics) |
| **Knowledge assumed** | Scenario 1 (positive selection, release, regeneration from light, planting/protection) |
| **Main question** | *What did the previous owner do, what did it achieve, and what — if anything — does each part of this woodland need now?* |
| **New concepts** | C8 management history (reading it from records and from the forest); C9 deciding not to intervene; C7 in another's forest; release of **advance regeneration** (felling over young trees to free them) rather than Crop Trees; C20 basic pruning as an investment in selected Crop Trees |
| **Older concepts retested** | C3–C5 positive selection and residual stand **at reduced support**; C10–C13 regeneration, vegetation, browse; C22 contractor economics |
| **Species / site** | None new |
| **Economic complexity** | Grouping work into one contractor visit across several parts of the stand; choosing which year to commission harvest so the minimum is worth paying; owner time for marking, pruning and planting. A previous owner's records include what they spent and earned (context, not a target) |
| **Disturbance / risk** | None active. Storm *history* may be visible as a few old windthrown stems recorded in the history (only if authored through the storm model, not as decoration — D-020) |
| **Assistance** | Medium. Focus concepts (history, reassessment, not intervening) are supported: a "previous owner's records" view, map history layer, comments that *describe* each area's history. Positive selection and thinning help are reduced (no first-use walkthrough; P2/P3 readouts still available because a forester would measure the same things) |
| **Game explains** | What management history is and how to read it (stumps, brash, tubes, browse damage, crowded vs released crowns); what advance regeneration is; what pruning does (clear stem) and that it is an investment whose value is not paid now |
| **Player must infer** | Which past decisions were good; which areas need work; whether to continue the previous owner's Crop Tree choices or choose differently; whether the over-opened corner should be replanted, cleared, left or protected |
| **Poor management looks like** | Treating every area the same; re-thinning the area that needed nothing; ignoring the crowded Crop Trees in (b); clearing bramble everywhere at high cost; pruning trees that are not Crop Trees |
| **Success looks like** | Area-specific treatments; at least one area deliberately left; crowded Crop Trees released; advance regeneration either released or deliberately left for a reason; a coherent Crop Tree set the player owns |
| **Forest state that shows learning** | Divergence between areas after 10 years that matches the player's area-specific decisions; Crop Tree crowns expanding in (b); recruitment from the advance regeneration where released |
| **Unlocks** | Scenario 3; branch B1 (After the Storm); reduced assistance default for selection/thinning in later scenarios |
| **New technical systems** | Scenario package (start snapshot + definition + scenario id); inherited-history boundary (events before Year 0 shown as "previous owner"); per-scenario completion profile; assistance tier flag; "no work needed here" decision record (saved event) |
| **Systems reused** | All Scenario 1 systems |
| **Economics decision** | 0.16 ha makes every thinning loss-making (minimum €2,500 [C] vs roughly €400–900 roadside per first thinning — a desk estimate from the Scenario 1 completion plan review, not a Unity result). A modestly larger holding (e.g. 0.3–0.5 ha) would make contractor timing a real choice rather than a fixed loss. Cost: tree count (annual step ≈ 37 ms at 336 trees, 273 ms at 1,300). **Open user decision** |

---

## Scenario 3 — Right Tree, Right Place

| Field | Content |
|---|---|
| **Starting forest** | A small farm conifer woodland with real site variation: a wet gley hollow (Sitka struggling to be stable, alder present), a drier free-draining knoll (Scots pine present, Sitka less suited than on the flat), an exposed western edge, and a sheltered mineral slope. Sitka main crop with a Scots pine component; downy birch colonising past openings |
| **Knowledge assumed** | 1–2 |
| **Main question** | *What will grow well here, and what is already trying to grow?* |
| **New concepts** | C14 species strategies (shade tolerance ≠ shade casting); C15 species–site matching on separate axes; C16 mixtures (not automatically better); C17 seed source by species |
| **Older concepts retested** | Positive selection across species (a Scots pine Crop Tree needs more light than a Sitka one); regeneration from light; planting; history; economics |
| **Species / site** | Scots pine (light-demanding productive/native conifer; GROWFOR [A/T]); downy birch (pioneer; ordinal only); common alder (wet niche; ordinal); **Douglas fir as a planting option** on the sheltered mineral slope (GROWFOR + Irish site-productivity paper [A/T]). Site axes: moisture/drainage, fertility, exposure (ordinal, per Species Evidence Pack). Sessile vs pedunculate oak site distinction may appear in planting options |
| **Economic complexity** | Species-dependent planting and protection costs. Timber markets for new species are **not required** if new species are young or planted; existing merchantable Scots pine would need an assortment registry entry (T1) or an honest "no market configured" |
| **Disturbance / risk** | Exposure appears as a site property and an inspection fact; storms remain off unless the storm site-exposure link is accepted later |
| **Assistance** | Medium. Species/site lesson fully supported: site map layer, species-site suitability shown as *ordinal facts with evidence labels* ("prefers free-draining soils"), not as a green/red verdict on a cell. Selection and regeneration at reduced support |
| **Game explains** | What each site axis means; each species' strategy in plain language; that shade tolerance and shade casting are different; that a mixture is a choice with trade-offs |
| **Player must infer** | Which species to favour or plant where; whether to accept birch in a gap or plant a productive conifer; whether alder in the hollow is an asset or should be replaced |
| **Poor management looks like** | Planting Douglas fir in the wet exposed hollow; removing all birch by reflex; planting Scots pine under a closing canopy; treating "native" as automatically correct |
| **Success looks like** | Species decisions that differ by site; enrichment that survives and grows; natural regeneration composition the player understood and chose to keep or steer |
| **Forest state that shows learning** | After ≥ 5 years: planted stock thriving on matched sites; birch flush in a large opening; Scots pine regeneration only where opening is large enough |
| **Unlocks** | Scenario 4; species knowledge pages for the species met; Training Stand species exercises |
| **New technical systems** | W0 Foundation (species tags/literal-id removal G1; shade casting/occupancy G2; species-aware density G3; site map G4); Scots pine, birch, alder parameters (ordinal where evidence is ordinal); Douglas fir growth from GROWFOR extraction (research-gated) |
| **Systems reused** | Scenario package (extended with a site map and a species roster); everything else |

---

## Scenario 4 — The Next Generation

| Field | Content |
|---|---|
| **Starting forest** | An older, already-thinned mixed stand (Sitka, Scots pine, some Douglas fir, oak and beech seed trees) that should now be regenerating, but unevenly: beech advance regeneration persisting under canopy; oak seedlings failing to escape browse; rowan held below browse height; a bright gap full of bracken with nothing in it; a seed-limited corner with no broadleaf seed source; Sitka regenerating freely. High deer pressure |
| **Knowledge assumed** | 1–3 |
| **Main question** | *Why are young trees failing here — and which response is worth its cost?* |
| **New concepts** | Regeneration diagnosis (seed vs light vs vegetation vs browse vs site, often in combination); C12 central (browse shifts composition: palatable species lost, Sitka favoured); area protection (fencing) vs individual shelters vs accepting slower recruitment; gap size and its effect by species |
| **Older concepts retested** | Species/site, light, vegetation clearance trade-offs, planting, economics |
| **Species / site** | Rowan (ordinal; browse indicator); holly (ordinal; understorey shade, slow); beech and oak become *managed regeneration* species, not only planting stock; Douglas fir palatability matters |
| **Economic complexity** | Protection economics: per-tree shelters vs perimeter fencing for a group; fence maintenance (if accepted); the cost of waiting |
| **Disturbance / risk** | Browse pressure as a persistent risk; no storms required |
| **Assistance** | Medium-low. The ground readout shows **evidence** (seedlings present by species and height, browse signs, vegetation cover, canopy openness, nearest seed trees) but **not the single limiting-factor label** used in Scenario 1 ("evidence, not verdict"). Help explains how to read each evidence type |
| **Game explains** | How to read each kind of evidence; what fencing and shelters do and cost; that deer preference differs by species |
| **Player must infer** | The limiting factor(s) in each place; whether to act; which response gives most recruitment for the money and labour |
| **Poor management looks like** | Planting more into a browse-limited gap without protection; blanket clearance where seed is the limit; fencing everything; opening the canopy where no seed source exists |
| **Success looks like** | Different responses in different places, matched to the cause; recruitment established by at least two routes |
| **Forest state that shows learning** | Recruits above browse height in protected or fenced areas; composition that includes palatable species where the player chose to protect them; untouched areas where natural regeneration was already succeeding |
| **Unlocks** | Scenario 5; branch B2 (Old Oakwood); "evidence, not verdict" diagnosis becomes the default |
| **New technical systems** | Fencing gameplay (deferred D-044; geometry and v15 field reserved per earlier records); species-specific browse classes for new species; rowan and holly (ordinal); diagnosis tiering (evidence lines separated from conclusion label) |
| **Systems reused** | W0/W1, scenario package, Model 2, browsing, shelters, economy |

---

## Scenario 5 — The Irregular Forest (CCF graduation)

| Field | Content |
|---|---|
| **Starting forest** | An established irregular, multi-aged mixed forest that has been under CCF for decades: Sitka, Douglas fir, Norway spruce, oak, beech, birch; trees in several layers; recruitment present; some fine quality Crop Trees, some poorly formed large trees; a few small gaps closing |
| **Knowledge assumed** | 1–4 |
| **Main question** | *How do I take an income from this forest while keeping it irregular and self-renewing?* |
| **New concepts** | C18 maintaining (not creating) irregular structure; C19 single-tree vs group harvest; C20/C21 quality-led selection (form, defects, pruned stems) and assortment value; C22 strategic economics (regular smaller harvests, liquidity); C27 long-term planning; C9 restraint in an already-good forest |
| **Older concepts retested** | All, at minimal support; positive selection now with quality and species value; regeneration diagnosis in several layers |
| **Species / site** | Norway spruce (tolerant conifer suited to irregular management; GROWFOR [A/T]); Douglas fir now merchantable; site variation present but not the focus |
| **Economic complexity** | **Strategically demanding for the first time**: harvest income must fund future operations; quality decisions change value; contractor minimum shapes harvest frequency |
| **Disturbance / risk** | Storms on as background risk (Model 1 profile) once activation policy allows; the player is not told when |
| **Assistance** | Low. No candidate Crop Tree highlighting, no named limiting factor, no "next step" prompt. Full measurement and record information remains |
| **Game explains** | Only terminology on request (glossary), the meaning of layer diagnostics and assortments |
| **Player must infer** | Everything about priorities |
| **Poor management looks like** | Harvesting all large trees (high-grading); regularising the stand into one cohort; not harvesting at all and letting it close; harvesting by value alone and losing recruitment |
| **Success looks like** | Income taken across several cycles while layers and recruitment persist; quality improving in retained Crop Trees |
| **Forest state that shows learning** | After ≥ 3 cycles: ≥ 3 layers still present; recruitment into each; Crop Tree quality maintained or improved; solvency |
| **Unlocks** | Scenario 6; **sandbox access**; graduation recognition (descriptive, not a score) |
| **New technical systems** | Per-tree quality state (form class, browse history; save bump; W4); assortments/markets for Douglas fir and Norway spruce without fabricated prices (T1/T3); layer diagnostics (W2); Norway spruce parameters |
| **Systems reused** | All |

**Should this be the graduation?** Yes. Converting a plantation (Scenarios 1–2) proves a player can *start* CCF; maintaining irregularity proves they understand it. It is the test practitioners would recognise (Pro Silva emphasis on continuous, single-tree-oriented management). Scenario 6 is then integration of everything, not a harder graduation.

---

## Scenario 6 — The Farm Woodland (integrated challenge)

| Field | Content |
|---|---|
| **Starting forest** | A realistic small private holding with two or three compartments: a neglected unthinned Sitka block (stability risk), a partly converted mixed block, and a young broadleaf planting under browse pressure; varied site; previous owner's partial records |
| **Knowledge assumed** | 1–5 |
| **Main question** | *Given everything, what matters most here, and in what order?* |
| **New concepts** | Prioritisation across compartments; sequencing operations across years under labour and cash limits; risk trade-offs (opening a neglected stand increases exposure) |
| **Older concepts retested** | All |
| **Species / site** | No new species; full site variation |
| **Economic complexity** | Finite cash and owner time; contractor minimums across compartments; liquidity for future operations. Difficulty comes from **competing priorities**, not low cash |
| **Disturbance / risk** | Storms on (Model 1 profile; site exposure if available); browse pressure; possibly a late plant-health constraint as an availability rule, not a simulation |
| **Assistance** | None prescriptive. The player may record their own priorities (a short owner's plan); the completion review compares outcomes with *their* stated priorities |
| **Game explains** | Nothing new |
| **Player must infer** | Everything |
| **Poor management looks like** | Working only the profitable compartment; opening the neglected block heavily and losing it to wind; spending all cash on protection in year 1 |
| **Success looks like** | A defensible sequence that improves each compartment by the owner's own priorities without collapse |
| **Forest state that shows learning** | Each compartment changed in a direction consistent with the player's recorded priorities |
| **Unlocks** | Sandbox templates derived from its compartments |
| **New technical systems** | Optional "owner's plan" record (small saved text/priority set). No new simulation |
| **Systems reused** | All |

---

## Branch B1 — After the Storm

| Field | Content |
|---|---|
| **Starting forest** | A Sitka stand one year after a severe windthrow: uprooted groups, a new exposed edge, fallen deadwood, light on the ground, some trees leaning (only if the model has that state; Model 1 has one fallen/uprooted outcome) |
| **Knowledge assumed** | 1–2 |
| **Main question** | *What should I salvage, what should I leave, and what does this opening make possible — or dangerous?* |
| **New concepts** | C24, C25, C23 (deadwood as habitat), edge stability, salvage economics under the contractor minimum |
| **Older concepts retested** | Release, regeneration in gaps, history, economics |
| **Species / site** | None new |
| **Economic complexity** | Salvage revenue vs minimum charge; depressed prices after storms are real (Species Evidence Pack economic caution) but the project should not hard-code 2026 prices — a scenario price-condition flag would be a separate decision |
| **Disturbance / risk** | Central: the disturbance has happened; further storms possible at Model 1 profile |
| **Assistance** | Medium (consequence-first; explains storm mechanics and salvage options; does not recommend) |
| **Poor management looks like** | Salvaging everything at a loss; thinning hard along the new edge; ignoring regeneration opportunity in the gap |
| **Success looks like** | A reasoned mix of salvage and retention; regeneration in the gap; a stable residual stand |
| **New technical systems** | Storm activation for this scenario (D-050 frozen profile); authored disturbed start (generated through the storm model, not decorated) |
| **Why a branch after Scenario 2** | Needs only Sitka and current systems; very relevant to Irish owners after Storms Darragh and Éowyn; reinforces reassessment from Scenario 2 |

---

## Branch B2 — The Old Oakwood

| Field | Content |
|---|---|
| **Starting forest** | Semi-natural oak–birch woodland with holly and rowan understorey, heavy deer pressure, much deadwood, an open glade with existing habitat value, little merchantable timber |
| **Knowledge assumed** | 1–4 |
| **Main question** | *What does this woodland need from me — possibly very little?* |
| **New concepts** | C28 open habitat value; non-intervention as the default option; long timescales; browse as a landscape problem (fencing a group vs accepting); standing deadwood and habitat trees |
| **Poor management looks like** | Planting the glade; thinning for timber; fencing everything; removing holly reflexively |
| **Success looks like** | Minimal, targeted intervention; recruitment trend improving where protected; glade kept open; deadwood retained |
| **New technical systems** | Open-habitat state (NEW; must be causal, not decorative); standing deadwood (NEW, D-020); hazel resprouting (optional, W-C); W1 species at full strength; fencing |
| **Why a branch and not core** | Heaviest new-system cost; its core lesson (restraint, ecological objectives) also appears in smaller form in Scenarios 2, 5 and 6. **Open user decision**: make it core if the ecological track is a priority for the user |
