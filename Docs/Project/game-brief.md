# CCF Game Brief

## Purpose

This file defines the long-term vision and intended player experience for CCF. It should change only when the direction of the game changes.

Implementation facts belong in the Unity Project Overview. Current work belongs in the Current Milestone. Accepted, proposed and unresolved decisions belong in the Decision Log.

## Core concept

CCF is a 3D sustainable forestry and land-management game in which the player makes management decisions, advances ecological time, and then physically walks through the landscape to experience the consequences.

The central loop is:

**decide → advance time → walk the landscape → observe and experience consequences → understand why → decide again**

Forestry is the first and most developed land-management system. Continuous-cover forestry is central to Scenario One: the player should be able to harvest timber while maintaining woodland cover, encourage or plant regeneration, manage competition and browsing, retain habitat structures, and gradually transform an even-aged plantation into a more structurally diverse forest.

The long-term game may expand beyond forestry into restoration, biodiversity, water, food production and other forms of land use, but the project should first make one Irish forestry scenario understandable and enjoyable.

## Intended player experience

The player should feel physically present in the landscape rather than operating a spreadsheet with a 3D forest attached.

Management decisions should be visible and spatial. The player should walk the stand, inspect trees and regeneration, mark work, place planting locations or protection, and later return to see what those decisions produced.

Consequences should be:

- **visual** — canopy structure, regeneration, understorey, deadwood, tree form and built features change;
- **ecological** — recruitment, competition, browsing, habitat and resilience respond;
- **economic/logistical** — work, stock, contractor costs and material choices matter;
- **sensorial** — lighting, soundscape and atmosphere reinforce landscape change.

The player should be able to experience the same place 10, 20, 50 or 100 years later and recognise both intentional management and unintended consequences.

## Creative reference

Valheim remains a reference for exploration, gathering, building, progression and cooperative physical presence. It is an inspiration rather than a design specification. CCF should not inherit Valheim mechanics when they conflict with the land-management simulation, ecological credibility or the staged Scenario One direction.

## Scenario One — Irish reference implementation

Scenario One is the reference implementation: conversion of an even-aged Irish Sitka spruce plantation toward continuous-cover forestry.

Ireland is the authored ecological and cultural context for this scenario, not a hard-coded assumption of the engine.

The project should keep a practical separation between core simulation/game systems, scenario/regional ecological data, and presentation/content packs. That separation should serve actual maintainability and future regional scenarios without creating speculative infrastructure.

Scenario One should be completed to a strong standard before broadening to many biomes or scenarios.

## Staged gameplay direction

### Stage 1 — Management Simulation

The player inspects the forest, makes spatial management decisions, builds an annual Work Plan, approves contractor work, advances one ecological year, reviews the results, and walks the changed forest.

**inspect → mark/plan → approve → contractor executes → advance one year → ecology/economy update → review → walk**

Stage 1 is the current reference mode for Scenario One.

### Stage 2 — Hybrid Solo

The player can choose whether to perform some work manually or contract it out. Survival, construction, material handling and direct forestry work become more important, while preserving the same authoritative ecological consequences.

### Stage 3 — Physical Cooperative World

A later cooperative mode may use a persistent shared world in which players physically perform forestry, construction and land-management work. Preserve reasonable future multiplayer options, but do not build multiplayer infrastructure before a milestone needs it.

## Management architecture principle

**Management decisions create tasks. How a task is executed is separate from what the task does to the world.**

The same ecological action can be completed by a contractor in Stage 1, by the player in Stage 2, or by players in a future cooperative world without duplicating the ecological rule.

## Time

Stage 1 uses discrete annual ecological advances. Later stages may introduce seasons or finer time for survival, weather and physical work, but annual ecological processes should not be mechanically divided into quarters unless evidence or gameplay requires it.

Long time horizons are part of the experience. Advance through decades while retaining a comprehensible history of the stand.

## Survival and construction

Survival and construction support the land-management game rather than replacing it. Use recognisable tools and plausible materials rather than a primitive stone-age crafting ladder.

Future progression may include shelters/buildings, paths/signs/access infrastructure, suitable forestry residues or retained timber, fencing/tree protection, timber processing, food and other land uses.

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
