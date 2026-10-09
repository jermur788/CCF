# CCF Game Brief

## Purpose

This file defines the long-term vision and intended player experience for CCF. It should change only when the direction of the game changes.

Implementation facts belong in the Unity Project Overview. Current work belongs in the Current Milestone. Accepted, proposed and unresolved decisions belong in the Decision Log.

## Core concept

CCF is a 3D sustainable forestry and land-management game in which the player makes management decisions, advances ecological time, and then physically walks through the landscape to experience the consequences.

The central loop is:

**decide → advance time → walk the landscape → observe and experience consequences → understand why → decide again**

Forestry is the first and most developed land-management system. Continuous-cover forestry is central to Scenario One: the player should be able to harvest timber while maintaining woodland cover, encourage or plant regeneration, manage competition and browsing, retain habitat structures, and gradually transform an even-aged plantation into a more structurally diverse forest.

The long-term game may expand beyond forestry into restoration, biodiversity, water, food production and other forms of land use. Scenario One is the forestry reference foundation, not the endpoint: Stage 1 should develop into a strong structured multi-scenario continuous-cover forestry simulator before broad Stage 2 expansion (D-051).

## Intended player experience

The player should feel physically present in the landscape rather than operating a spreadsheet with a 3D forest attached.

Management decisions should be visible and spatial. The player should walk the stand, inspect trees and regeneration, mark work, place planting locations or protection, and later return to see what those decisions produced.

Consequences should be:

- **visual** — canopy structure, regeneration, understorey, deadwood, tree form and built features change;
- **ecological** — recruitment, competition, browsing, habitat and resilience respond;
- **economic/logistical** — work, stock, contractor costs and material choices matter;
- **sensorial** — lighting, soundscape and atmosphere reinforce landscape change.

The player should be able to experience the same place 10, 20, 50 or 100 years later and recognise both intentional management and unintended consequences.

## Primary forestry audience

The primary reference audience is smaller private woodland and farm-forest owners/managers, rather than industrial estates (D-053). Management choices, explanations and economic/logistical framing should serve that audience without assuming a single correct prescription.

## Creative reference

Valheim remains a reference for exploration, gathering, building, progression and cooperative physical presence. It is an inspiration rather than a design specification. CCF should not inherit Valheim mechanics when they conflict with the land-management simulation, ecological credibility or the staged Scenario One direction.

## Scenario One — Irish reference implementation

Scenario One is the reference implementation: conversion of an even-aged Irish Sitka spruce plantation toward continuous-cover forestry.

Scenario One is a genuinely bounded property.

Ireland is the authored ecological and cultural context for this scenario, not a hard-coded assumption of the engine.

The project should keep a practical separation between core simulation/game systems, scenario/regional ecological data, and presentation/content packs. That separation should serve actual maintainability and future regional scenarios without creating speculative infrastructure.

Scenario One should establish a strong forestry foundation before broader scenario development. Forestry scenarios represent independent forests; the player carries knowledge and experience between them, rather than transferring the same forest or assuming shared inventories/economies. A custom sandbox comes later (D-052). Exact scenario count, curriculum, branching structure and progression mechanics require their own accepted scope.

## Staged gameplay direction

### Stage 1 — Forestry Management Simulation

The player inspects the forest, makes spatial management decisions, builds an annual Work Plan, chooses who does the work, resolves it through simulation, advances one ecological year, reviews the results, and walks the changed forest.

**inspect → mark/plan → choose who does work → resolve through simulation → advance time → ecology/economy update → review → walk**

Work may be assigned to a contractor or the landowner. Landowner execution can still be simulated through menus; manual physical execution is not required in Stage 1. Stage 1 is the current forestry-focused reference mode for Scenario One.

Stage 1 develops beyond Scenario One through structured independent forestry scenarios before broad Stage 2 expansion. This is accepted direction, not a claim that multi-scenario infrastructure is implemented.

### Stage 2 — Expanded Land-Management Simulation

After forestry maturity, fruit and nut trees are the first intended Stage 2 slice (D-054). Broaden the simulation before introducing manual execution. The wider direction includes:

- fruit and nut trees;
- ponds and water storage;
- biodiversity/habitat works;
- fencing, paths and infrastructure;
- earthworks and landscaping;
- timber construction;
- cob/earth construction where suitable;
- stone walls;
- broader regenerative/property-management systems.

These remain simulation/menu-resolved systems. Detailed Stage 2 mechanics belong in later accepted task scopes.

### Stage 3 — Optional Manual / Hybrid Solo

The player may optionally perform tasks physically that already exist in the authoritative simulation. Manual execution is an alternative execution method, not a second ecology/construction system.

### Stage 4 — Physical Cooperative World

A later cooperative mode may use a persistent shared world in which players physically perform the same authoritative forestry, construction and land-management tasks. Preserve reasonable future multiplayer options, but do not build multiplayer infrastructure before a milestone needs it.

## Management architecture principle

**Management decisions create tasks. How a task is executed is separate from what the task does to the world.**

**decision → task → labour/material/tool requirements → execution method → authoritative world result**

Possible execution methods are contractor simulation, landowner simulation, manual solo, and later cooperative manual. Choosing contractor versus landowner is separate from choosing simulated versus manual execution. All execution methods share the same authoritative world result rather than duplicating ecology or construction rules.

## Time

Stage 1 uses discrete annual ecological advances. Later stages may introduce seasons or finer time for survival, weather and physical work, but annual ecological processes should not be mechanically divided into quarters unless evidence or gameplay requires it.

Long time horizons are part of the experience. Advance through decades while retaining a comprehensible history of the stand.

## Survival and construction

Survival and construction support the land-management game rather than replacing it. Use recognisable tools and plausible materials rather than a primitive stone-age crafting ladder.

Future progression may include shelters/buildings, paths/signs/access infrastructure, suitable forestry residues or retained timber, fencing/tree protection, timber processing, food and other land uses.

Retained forest timber may feed construction, and smaller timber may have lower-grade uses. Cob/earth construction depends on suitable material, and stone may be used where available. Pond excavation and earthworks may generate potentially reusable earth or stone; do not assume all excavated soil is suitable clay.

Progression should come from capability, logistics, cost, maintenance and knowledge rather than a conventional XP ladder.

## Ecology and management philosophy

Use credible ecological knowledge, simplified where necessary for clear and enjoyable play. Distinguish measured/well-established knowledge, management guidance, evidence-derived inference, simulation abstraction and gameplay calibration.

Do not imply one universally correct forestry prescription. Create meaningful trade-offs between immediate timber/survival needs, recruitment, habitat/deadwood, understorey/browsing, long-term resilience, cost and labour.

Planting trees is not automatically ecologically beneficial. Site suitability and existing open habitats matter.

## Art, audio and authenticity

The long-term visual goal is a beautiful, immersive and readable forest. Art should make authoritative state visible rather than invent unsupported ecological state.

The Irish reference implementation may use first-party photography, field recordings and generated/authored assets. Environmental audio should communicate habitat/landscape change where credible source material is available.

## AI-assisted development

Use AI extensively for code, Unity, Blender, assets, research integration, testing and documentation. No model family permanently owns a discipline; assign tasks by capability, tools, context, availability and demonstrated quality. The user remains the product owner and final decision-maker.

## Open-source direction

Open-source release and community regional/property variants remain possibilities. If pursued, handle code/data and media-asset licensing separately so contributors know what may be reused.

## Design principles

- Make sustainability understandable through visible causes and consequences.
- Make forestry decisions spatial and physically legible.
- Experience delayed consequences rather than only reading scores.
- Preserve multiple credible management strategies.
- Separate ecological rules from presentation where that improves testing/maintenance.
- Prefer small playable improvements over speculative infrastructure.
- Build one excellent Irish reference scenario before generalising broadly.
- Verify actual player-facing behaviour, not only compilation.
