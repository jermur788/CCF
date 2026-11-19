# Sen-1 #824 → #838: Research and original SVG mockups for Sitka spruce forest-floor visual improvement

**19 November 2026. Status: [#838 READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/838). Browser research and original text-SVG concepts ONLY.**

**Owner's request:** use the successful [#835 browser research + original SVG mockup workflow](https://github.com/jermur788/CCF/issues/835) again to improve how the **ground beneath the Sitka spruce trees** looks in the first-person Compact30 Scenario One. The phrase "sickest vine" in speech is interpreted from the actual three-species game context as **Sitka spruce**. No claim is made that the Manager has inspected the currently rendered ground; the owner has reported that it needs improvement.

**Purpose in plain English:** give the T3 team researched, visually useful, distinct suggestions for a credible **Irish Atlantic Sitka plantation forest floor**, not a brown flat sheet or universal lush moss carpet, while respecting the exact compact no-rack scene/path/planting constraints. Produce **three actual original SVG files in GitHub** and a clear implementation-oriented handoff, as #835 did. No native Unity or scene edits during this browser phase.

## 1. Binding inputs and ownership

- Canonical authority: latest owner request, [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), [current milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), and [live active tasks](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md). D-083 Compact30 is a **new independent 30×30m / 0.09ha** all-Sitka reference slice; native source has 175 simulated spruce and 226 outside cosmetic trees; H1 ~12×10m, one winding *pedestrian-only* C0 path, four 2-Oak/2-Beech G1 points, **no extraction racks**. D-090 selects **one continuous walkable flat plane**, logical masks separately controlling H1/path/planting/exclusions, provisional muted worn-earth/needle path and neutral daylight. Don't invent hills, drainage trenches, vehicle roads, brash tracks or already-executed interventions.
- Existing **#786 within #785** created a first-party *offline scratch* `H1Path_Ground_v1.blend` with a **single flat 30×30 plane and baked H1/1.2m-path atlas**. It has original texture/export/render sources but is **not confirmed imported to Unity**, not a finished varied Sitka floor, and rights/spec/scene remain gated. **Do not duplicate this floor mesh or claim existing material/UV truth from the browser**; recommend composable overlays/roughness/macro variation as candidate upgrades and note the need to inspect/retain it.
- Existing [#836](https://github.com/jermur788/CCF/issues/836) is a separate **one-parent TG Fork source/Unity path-and-deer-fence task** now at read-only G0; author has inspected #786 scratch and found overlapping ownership/rights/graphics gates. **No production write overlap or silent absorption of #836 path scope.** #688 H1 kit and #691 graphics spec remain subordinate; graphics spec is DRAFT, D-032 binary asset/storage rights OPEN. #830 passed native automated 71/71 but **GUI/rendered aesthetics NOT RUN**. Do not report any current ground material QA PASS.
- The user wants a *future* T3 production implementation in one substantial coordinated orchestration rather than repeated microtasks: route the accepted design to Manager #824 for integration with or separate source-file reservation from #836/#786; subsequent T3 work must **actually invoke installed TG Fork `orchestrate-work`**, with independent nonauthor review and one final handoff when admitted. Browser #838 does not create that Unity lease or grant.
- User-supplied forestry ecology/growth composites and canonical research index may be useful but should only be cited when the browser worker actually has access to those exact materials. Start with accessible primary sources below. Do not mischaracterise a research observation as a hard biome rule or model mechanical parameter.

## 2. Initial evidence leads to recheck, not assumed final art spec

**Irish National Biodiversity Data Centre, [WL5A Sitka spruce forest](https://biodiversityireland.ie/ivc-classification-explorer/wl5/wl5a/):** describes mature dense-canopy stands with a **dense needle layer**, typically poor field vegetation, occasional ferns/bramble/grass where conditions allow, and several moss/liverwort taxa that often are not abundant; canopy openings vary the ground layer. Site soils and stand stage matter. Crucially, **do not produce an unbroken emerald moss carpet by default**.

**University College Cork [FORESTBIO project](https://www.ucc.ie/en/planforbio/projects/forestbio/):** studied ground vegetation across Irish plantation age classes and found vegetation/species richness varies across the forest cycle. Different stand ages and management/opening histories mean that one uniform floor is not empirically defensible.

**COFORD [Continuous Cover Forestry Ireland study](https://www.coford.ie/media/coford/content/publications/projectreports/CCF.pdf)** and [Teagasc ContinuFOR](https://teagasc.ie/news--events/daily/transformation-of-sitka-spruce-stands-to-continuous-cover-forestry--continufor/): useful candidate context for how canopy light/openings and interventions influence understorey. Verify actual statements before applying. Avoid invented species cover percentages, soil chemistry numbers or "after thinning" patterns in a starting stand where the history is not modelled.

**Photo/reference rights:** Use photographs as **linked reference only** with attribution and licensing caveats. Do **not** copy/trace copyrighted forestry photography, texture crops or commercial assets into GitHub SVGs or production materials. Generate new original vector designs/schematic shapes from underlying observations. Do not place web photos inside the SVG files.

## 3. Questions the browser worker must resolve

1. What would a plausible Sitka spruce forest floor look like in an **Irish Atlantic plantation/CCF starting scenario** at eye height, taking shade/moisture/site variation seriously? When is the needle/twig/dark soil/bryophyte layer more prominent than taller vascular plants? What *cannot* be generalized?
2. How to make this look visually high quality in Unity **without fake homogeneous green grass**? Specify muted soil, dark brown/orange needles, twigs, fine moss islands, constrained brash/debris, occasional exposed roots/stones if location warranted. Include physically plausible variation at tree bases, under crowns and nearer light edges without imposing artificial circular brown discs.
3. Which pieces should be **texture/material/macro masks** vs **geometry/ground scatter**, and how to keep 175 simulated-tree bases plus 226 outside decorative shells visually coherent without high per-instance draw-call cost? Treat performance targets as *design proposals*, not measured implementation truth.
4. How will layers blend with the **existing H1 ground, 1.2m path, ≤0.20m visual verge and G1 planting points** without seams or hiding genuine path geometry defects? **Do not move masks/trees/juveniles, change player collision, ecology, browse/regeneration, permissions or any save/canonical world bytes.**
5. Should the look have one calibrated baseline material and a small set of optional light/shade variants, rather than dynamic forestry ecology behaviour? Make the static art direction/gameplay dynamic boundary explicit. Include an owner-friendly side-by-side comparison for acceptance.

## 4. Required original SVG mockups — all THREE as durable GitHub files

All SVGs must be valid original editable **text SVG**, legible at normal screen size (prefer 1200×700 or similar), sourced from your original shapes and palettes, with clear concept labels and legend. They are **schematic mood/design references, not Unity rendered proofs, texture bake masters, measured maps or empirical percentages**. A browser worker may additionally generate raster illustration previews if available, but must not describe unattached PNGs as GitHub assets.

**A — `Sen-1-MOCKUP-A-SITKA-SHADED-FOREST-FLOOR.svg`**
- Illustrative near-eye and top-down swatches for **dense/shaded Sitka understorey**; patchy acidic-brown needles/twig litter, dark soil, small patches of moss/bryophyte, sparse low vegetation; tree-base irregularity and restrained wood debris.
- Show a "too flat / too green" comparison beside the recommended richer restrained surface.
- No invented lush fern garden or mandatory cover percentages.

**B — `Sen-1-MOCKUP-B-SITKA-LIGHT-EDGE-AND-TREE-BASE-TRANSITION.svg`**
- A credible **dark interior → lighter edge/gap** visual transition with sensitivity to actual canopy, wetness and management history; include Sitka trunk roots/needle fall and potential localized understory changes as art variants.
- Convey blend continuity rather than a rigid circular ring, straight texture join or random repeated tiles. Tie to H1/path/planting interface without changing their logical masks.

**C — `Sen-1-MOCKUP-C-SITKA-GROUNDCOVER-MODULAR-UNITY-KIT.svg`**
- Technical-ish reusable kit sheet: base surface, macro/noise mask, needle/twig decal families, moss/fern/grass sparse ground scatter, root/branch options, edge blending and texture/LOD/alpha/performance priorities; label what can be reused by later scenarios vs what is Sitka-specific.
- Provide "minimum viable first art pass" and "optional higher fidelity later", plus human-scale eye-height vs medium-distance readability guides.

## 5. Required final research Markdown

Exact publication path (new file only):
`Docs/Handoffs/ScenarioOne-Sitka-Forest-Floor-Research-20261119/Sen-1-838-SITKA-SPRUCE-FOREST-FLOOR-RESEARCH-SVG-MOCKUPS-AND-T3-DESIGN-HANDOFF-20261119.md`.

Include: executive recommendation, source claim/evidence table (Irish first, UK transfer clearly labelled), chosen palette and physical materials with lighting considerations, A/B/C **GitHub links**, what was rejected and why, explicit owner user-test symptom, constraints/ownership, original SVG source provenance, modular art bill of materials, performance/LOD proposals and **one compact ready-to-authorise T3 implementation packet**. Include clear acceptance criteria at walking eye-level 1280×720/1920×1080, top-down and changing light, including no repeat pattern, floating ground/trees, path/planting join seam, incompatible Stand Map geometry or save/regeneration/soil mechanics drift. Cite exact linked public research without unsupported precision. Distinguish provisional **art calibration** from verified ecology research and unobserved Unity state.

**Do NOT pre-create a new production source claim or install Unity.** Manager #824 reviews this browser design, decides whether to **amend/coordinate #836** or create a disjoint later source implementation under TG Fork, with **one primary writer per file set**.

## 6. Worker GitHub process

- **Claim:** [#838](https://github.com/jermur788/CCF/issues/838) — READY now, register actual provider/model and browser session/branch/start head before edit; post [RUNNING]. No native/local machine/physical Ops ownership.
- **Difficulty:** **Medium** — research grounding and three original concepts with high-quality implementation relevance, no actual game source changes.
- **Best suited:** GPT-6 browser ChatGPT with web research and GitHub connector (or another capable browser/GitHub model). No T3/Unity needed for this research task.
- **Dedicated branch:** recommend `docs/sen-1-838-sitka-floor-browser-result-20261119-v1` from current `main`, unless collision; verify and reserve sole writer.
- **Exact write allowlist:** one new research Markdown file above and **three** specified text SVG files alongside it; no modifications to existing SVG #835, graphics spec, scenes, asset .blend/.fbx/.png, source, saves, board or live tasks.
- All three SVG files and Markdown should be created via GitHub connector, **fetched/read back**, and checked they truly contain valid source and correct links. Post [HANDED-OFF] with permanent branch/commit links; close #838 as browser research delivered and notify Manager [#824](https://github.com/jermur788/CCF/issues/824).
- No main merge, no independent source/asset/Unity proof, no changes to `Docs/Project/coordination/active-tasks.md` by the browser worker. Dashboard/Drive sync **NOT VERIFIED**.

## 7. Paste-ready browser chat dispatch

> **Sen-1 #838 — Sitka spruce forest-floor visual research and THREE editable original SVG concepts.** Complete as a **browser/GitHub research task**, matching the successful #835 path/fence research workflow. Binding complete handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-838-sitka-floor-browser-research-packet-20261119-v1/Docs/Handoffs/ScenarioOne-Sitka-Forest-Floor-Research-20261119/Sen-1-824-838-BROWSER-SITKA-SPRUCE-FOREST-FLOOR-ECOLOGY-VISUAL-RESEARCH-AND-SVG-MOCKUPS-PACKET-20261119.md . Register [RUNNING] on #838, research Irish Sitka spruce forest-floor needle/litter/moss/ground flora with primary sources, and create A shaded stand floor, B edge/tree-base transition and C modular Unity art-kit schematic as three ORIGINAL **GitHub-tracked source SVG files** plus one complete cited research/T3 handoff Markdown. Check #786 existing forest-floor H1/path atlas and #836 owner to avoid overlap; preserve Compact30 masks/path/planting/saves and do not infer deer/forestry gameplay. Fetch/read back every file, [HANDED-OFF] on #838 and return results to Scenario One Manager #824. No native Unity, source edits, physical lock or new implementation claim; Manager will route later one-parent TG Fork art implementation after acceptance.

**User action:** Open a **separate browser chat with GitHub connector** and paste Section 7 dispatch. No local T3 step yet; no need to upload SVGS manually. This packet is the complete research assignment, not proof that its three SVG files already exist.
