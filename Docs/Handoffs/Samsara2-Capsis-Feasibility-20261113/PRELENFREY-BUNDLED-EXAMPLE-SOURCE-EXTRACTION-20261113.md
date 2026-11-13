# Prelenfrey source-example extraction — Samsara2 supplied installer

**13 November 2026 — Research only, no executable run.** Source: user-uploaded Samsara2 4.2.7-22961 installer, read-only extraction of embedded IzPack data pack. Cross-reference: official [martelosim guide](https://martelosim.lessem.forge.inrae.fr/articles/martelosim.html) and January 2026 [EU MOSAIC report, Attachment 6](https://www.alpine-space.eu/wp-content/uploads/2026/01/Report-on-FLFLL-network-equipped-with-virtual-tool-and-solutions-for-knowledge_for-report.pdf). Findings are source inputs, not simulated predictions or CCF authority.

## Real forest

The living lab is the Prelenfrey marteloscope in the Le Gua forest, Vercors foothills ~20 km south of Grenoble, France. An approximately 40-degree alpine slope below cliffs contributes rockfall and avalanche protection. The EU report describes mature dominant silver fir, smaller beech and other broadleaves, sparse recruitment into larger trees, and the challenge of renewing the forest without damaging protection or timber value. Real current low-intensity management harvests large trees approximately every **15 years**. The model plot itself records altitude 1280 m; the nearby village is described at 978 m.

## Embedded initial inventory (not result of model execution)

**Prelenfrey_Inventory.txt**, date 2023, planar bounds X=0..125m and Y=-1..89m (125×90m rectangle; not an audited net stand area), 5-m cells, annual light update, slope field 40, plot altitude 1280m, water-holding capacity 47mm, recruitment DBH 7.5cm.

Parsed **645 living adult trees** with DBH 7.5–79.1 cm, median 14.4 cm, summed input basal area 34.70055 m²: European beech 306, silver fir 233, sycamore 65, rowan 20, whitebeam 11, Norway maple 5, wych elm 2, ash 2, Norway spruce 1. A ten-species parameter file also defines hazel, though no hazel adult was recorded. Separate inventory sections include **429 sapling records and 252 cohort rows**; cohort counts are not another 252 individual saplings and may represent multiple stems. Tree height/geometry includes NA fields; don't call every field directly measured.

## Embedded example commands and marks

**Prelenfrey_Marking_All.txt** has 645 random marking records, **203 Cut and 442 Wait**; all 203 cut IDs map to adult inventory. Their summed basal area is 12.08m², approximately **34.8% of input adult BA**. This is a random **marking input**, not an executed harvest or recommended thinning intensity. The other demonstration group X contains Wait for all records.

**Prelenfrey_CommandFile.txt** invokes the flexible script class, starts in 2023, has active Random scenario initial marking in 2023, multicriteria thinning in 2033 and End in 2043. A different X scenario with 2023 initial marking, 2033/2043/2053/2063 thinnings and End 2073 is **commented out** — not active. The accompanying silviculture text contains residual BA target 17m²/ha and maximum 20% BA cut at subsequent multicriteria thinning. These are demonstration parameters, not universal forestry prescriptions or approved CCF values. The model has **not** been run in this analysis.

The official R guide shows a Prelenfrey workshop chain: new_inventory, build_samsa2_files, random_marking, write_samsara_cmd (10-year thinning, 20-year evolution, auto) -> run_samsara -> martelo_report. Its speed claim (~1 minute) is an example only, not measured on user's computer. Wrapper guide build 22711 differs from supplied 22961, so compatibility requires an actual test.

## Teaching and CCF relevance

MOSAIC Attachment 6 illustrates species/diameter/action stand maps, basal area before/after and trajectories, harvest by species/size/quality, rockfall protection indices and snow interception. These are **published report illustrations**, not newly reproduced results. The setting offers an excellent external test of tree-level judgement, release and regeneration across repeated interventions, with more explicit environmental trade-offs. It is NOT numerical calibration for Irish Sitka/Oak/Beech, proof of a correct marking strategy, or grounds to add rockfall mechanics to Scenario One. No CCF changes, Unity, saves or runtime verification performed. #456 remains separate and subject to actual local Gate 0.

Dashboard and Drive synchronization not verified.