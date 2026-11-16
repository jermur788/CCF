# Sen-1 — Scenario One Ground Graphics Parity V1

**STATUS: DRAFT — NO IMPLEMENTATION AUTHORITY.** Scenario One Manager #424, OpenAI ChatGPT GPT-6 (GitHub/browser only), 16 November 2026. Requires accepted visual decisions, independent review as appropriate and explicit approval before Unity/production changes.

## Authority and evidence
Based on the [source-graded graphics audit](https://github.com/jermur788/CCF/blob/docs/sen-1-graphics-parity-audit-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-SCENARIO-ONE-GRAPHICS-PARITY-AUDIT-AND-SPEC-HANDOFF.md): read-only scene/source inspection plus five **historical** images, including one unrelated 30m research arena. No native Unity run or current before images. D-020 prohibits invented causal ecology; D-032 binary storage policy is OPEN; D-045 prior presentation acceptance is historical baseline; D-056 Legacy40/Enlarged80, D-064/065 rack/MFM and D-071/072 five-parallel-rack geometry are protected. Frozen Reference Future v1, historical saves, current save-system reservations, signed save specs and all unrelated work remain unaffected.

## Objective
Raise ground-level presentation to one coherent visual quality at walking eye height: forest floor, existing main path, approach, headland/feeder junction, clearing, surface transitions, low plants and scene illumination. Surfaces should differ appropriately by function **while sharing one visual fidelity, exposure, texel scale and material response**.

**First benchmark vertical slice:** spawn clearing to main track to feeder junction, about 40×12m, on the actual Enlarged80 Year-0 stand. Evaluate six fixed source-audit poses C1–C6 and an oblique at both 1280×720 and 1920×1080. The complete connected path may require visual reconstruction; the player-facing score area remains the nominated corridor. No new footpath topology may be invented from a visual brief.

**Excluded:** forest/track topology, 52 source path GameObject names/transforms/active state, historical rack positions, scene physical colliders, terrain relief, canopy/gameplay, Stand Map informational truth, economy/forestry models, save schema, Compact/O2, unapproved external scenery, Windows build.

## Proposed presentation requirements
1. **GP-01 coherent light:** compare restrained E80-only Neutral/ACES tonemapping, subtle fog/haze and sky grade. Do not change global URP defaults or the shared ForestTest scene without separate approval. Final colour/look not yet owner-accepted.
2. **GP-02 light-reactive vegetation:** replace visual-only Unlit fallback material with consistent URP lighting, retaining authoritative cover meaning.
3. **GP-03 constructed-surface exclusion:** use authoritative existing path/approach/headland/feeder/pad/clearing geometry for render placement mask. Prefer safe alternate anchor **within the same ecology cell and state-derived count**; if none exists, suppress only that visual instance and record suppression. Do not change ecology, move across cells or invent cover. Multi-instance clustering is later scope.
4. **GP-04 material parity:** distinct low-clipping forest floor versus track looks, normals/roughness and world texel scale. Use existing licensed repo assets/parameter materials until D-032 storage approval; audit's four 2048² sets / ~60MB is merely a proposed budget.
5. **GP-05 visual continuity:** make track/approach/headland/feeder junction read continuously by rendering from the existing source objects. The **52 original named path GameObjects**, their positions and active state stay exactly preserved for starting-stand clearances, Stand Map, coverage, prescriptions, headland and approach code, and Editor builder. Renderer-only hiding is a candidate mechanism, **not assumed safe** before caller and Unity checks. Historical pre-approach E80 saves and Legacy40 must retain behaviour.
6. **GP-06 edges:** soft visual verges, leaf/needle drift, selective twigs or stones without a perfect linear seam/disc boundary. Non-causal dressing must not misrepresent real vegetation cover or completed work.

## Q1 surface identity: provisional research, not an owner decision
Irish COFORD *Forest Road Manual* (2004) distinguishes formed engineered tracks (usually not given surface dressing) from unsurfaced harvest tracks/racks; UK Forest Research *Operational Experience of Continuous Cover Forestry* (2006, Appendix 1) distinguishes stone forest roads, lower-spec tracks and unsurfaced/brash racks. Reference links:
- https://limerickandtipperarywoodlandowners.ie/wp-content/uploads/2024/04/ForestRoadManual-1-.pdf
- https://cdn.forestresearch.gov.uk/2022/02/ipin_1306_ccf_case_studies.pdf
- Irish conifer-track photographic example (reference only; NOT licensed production asset): https://www.geograph.ie/photo/3441512

**Appearance hypothesis:** main narrow track is compacted earth with modest stones, not a luminous orange ribbon; headland/feeder lower-spec compacted soil; future extraction racks unsurfaced/brash as appropriate. These classes are *evidence-informed aesthetic options*, not a verified diagnosis of the existing game-world road. Owner Q1 remains OPEN; do not lock materials until selected.

## Exact benchmark acceptance
- **AC-01** Before edits: fresh E80 actual same-seed images at C1–C6, both resolutions, C1/C6 frame medians and exact machine/render config. Historical 30m images cannot pass this gate.
- **AC-02** Ground/track materially distinct even in greyscale; no HDR clipped orange, plank UV scale/orientation jump, obvious tile repetition, or hard clearing disc/feeder seams. All shown fallback vegetation responds to scene illumination.
- **AC-03** Zero rendered ground vegetation anchors on constructed surfaces; no ecological cell-count/strength invention. Update focused verification assertions when visual anchors differ, retaining their intent.
- **AC-04** All 52 source path objects, Stand Map polylines, coverage, starting-tree IDs/positions, rack/history/approach semantics and all save/Reference/Legacy40 anchors are behaviourally unchanged.
- **AC-05** Performance: frame median at C1/C6 no more than +15% against **fresh same-machine** before, not the Overview's historical ~40ms estimate.
- **AC-06** HUD/tutorial/rack marking readability preserved at 720p and 1080p, and player still sees honest forestry consequences.
- **AC-07** Focused Tier-2 physical Unity, source-provenance review, one nonauthor review and explicit **user visual approval** before broad rollout.

## Gates and unresolved decisions
**Q2 Manager drafting default:** move only visual anchor within same cell/state if possible, else suppress rendering. **Q3:** permits concept comparison only, not global final-look acceptance. **Q4:** no new production texture binaries until D-032 decision/budget/licence. **Q5:** no exterior scenery in V1; separate user choice for later. **Q6:** E80 only with Legacy40 and Reference regression. **Q7:** SPEC REQUIRED, not a one-packet implementation.

Source/Unity work must wait for an **APPROVED** four-file spec, actual owner Q1/Q3/Q4 decisions as needed, fresh G-03 baseline, registered exclusive source writer and machine-wide CCF-Ops Unity lease under protected save/Prefs isolation. Stop for missing ownership, invalid historical anchors, uncovered path consumers, >15% performance or a material new gameplay/scene/persistence decision.
