# Onboarding reference manifest

Feature: SCENARIO-ONE-THREE-PAGE-ONBOARDING-V1
Status: ONBOARDING REFERENCES LOCKED — 2026-10-25
Source checkpoint: 0805ffb9b6b1153442b3530d7404be5847ce9c41
Source thread: 0c55a504-715b-46b5-a3a2-e8ef3639bbff
Recorded: 2026-10-25

These byte-exact copies preserve the user's normative visual sources. They are not substitute art or regenerated images. All are 1672×941 PNG. Final corrected mockups are not yet produced or approved. Hashing source files alone does not satisfy ONBOARD-01.

| Supplied source | Bytes | SHA-256 | LOCKED page/composition mapping |
|---|---:|---|---|
| supplied-ccf-definition.png | 2318699 | 95310e4322c4e6eb7ab6410ee9848bec5cc9595976417640f74641e9007ff1fc | Page 1: definition, three-scene progression, four principles |
| supplied-five-card-management.png | 2326633 | 47011222d700724e6501ab31780a781058c05caeb1de29e5d86e58d27cc05d59 | Page 2: generic iterative management process; source navigation shows page 3 |
| supplied-scenario-transformation.png | 2189800 | 100b9838ee80e3b8f057298eda31ed42d66454e91e1c852cd24e6c1bf802fe39 | Page 3: Scenario One before/process/goal; source navigation shows page 2 |

Total supplied source footprint: 6,835,132 bytes (6.52 MiB). No new binary family, package or LFS policy is introduced. Source paths and dimensions are recorded in source-hashes.json. Provenance: user-supplied generated mockups; underlying training/source/licensing provenance is not supplied, and no third-party licence claim is made.

## Approved mapping — LOCKED

User decision, 2026-10-25, source thread above: “Yes — lock that mapping.”

1. **Page 1 — What is Continuous-Cover Forestry?** Use the supplied CCF definition/progression composition.
2. **Page 2 — How CCF works over time.** Use the supplied five-card/process composition. Remove or replace all Scenario One controls, key prompts, rack instructions and scenario mechanics. Teach generic CCF: observe, retain/select, create space/light, regenerate/establish species, review/adapt and repeat over time.
3. **Page 3 — Scenario One — Transforming a Sitka Plantation.** Use the supplied before/transformation/long-term-goal composition. Put the specific Sitka transformation and mandatory establishment of both available additional species here.

The supplied images govern visual composition, layout and design language. Where image content conflicts with the written spec, the spec controls educational content and page purpose. Correct navigation and copy to match; do not otherwise redesign the compositions. Source dot positions/order are superseded by this explicit decision. The immutable source hashes above remain unchanged.

This decision locks mapping only; it does not assert final-reference, palette, forestry-image or runtime qualification.

## Unresolved conformance

2. Spec §9 requires existing V2 palette/fonts/buttons. Source art uses blue-black panels, blue borders/text/buttons, with sans titles on two pages and serif on the scenario page. Qualified V2 uses charcoal/slate, neutral text, teal/green controls, regular Liberation Serif major titles. Exact source styling and unmodified V2 tokens cannot both be matched. Manager decision is required before freezing changed references.
3. Spec §6/8 require establishing the actual additional species, Oak and Beech (verified in existing project implementation). The planting card visibly includes conifer stock. It cannot pass as the required two broadleaf species.
4. The developed goal images visibly include broadleaf foliage, retained conifers and varied sizes. They do not establish identifiable Oak AND Beech. A species-specific forestry review cannot honestly be marked PASS from these images alone.
5. Reference Page 1 contains detailed principles and an oak/beech example. Production Page 1 must use the spec's four short, scenario-independent principles. Spec text, not OCR, governs all copy.
6. A blurred forest backdrop is part of the normative composition. The current UI only dims its scene backdrop; the implementation plan must preserve or explicitly resolve this visual requirement.

Final reference lock requires final Page 1/2/3 mockups with exact copy, correct order/navigation, approved palette treatment and reviewed species imagery. Record separate final-reference hashes; retain these originals unchanged. Do not replace source originals silently.

## Forestry review of supplied originals

- Page 1 first state: visibly uniform, closely spaced conifers with plantation character — visual condition PASS; species identity is illustrative rather than botanically proven.
- Page 1 middle state: selective openings, retained mature conifers, beginning lower structure — PASS.
- Page 1 last state: broader range of crown forms/sizes and regeneration — progression PASS; Oak/Beech identity NOT VERIFIED.
- The three Page 1 states are not near-duplicates — PASS.
- Five-card planting scene: conifer included where the copy says both additional species — FAIL against §8.
- Scenario goal: retained productive conifers, irregular spacing and continuous cover without clear-fell implication — visually supported.
- Scenario goal: all three actual species clearly represented — NOT VERIFIED; cannot qualify.
- Irish suitability: pastoral hills/lake context is compatible with an illustrative Irish setting; no giant old-growth imagery is apparent. This is visual assessment, not a claim of photographic provenance.



## Final implementation-reference lock — 2026-10-25

User explicitly approved the authority reconciliation in source thread: V2 styling/tokens; supplied image composition/layout; written specification content/forestry meaning/behavior. No onboarding-specific colour tokens are authorised. Earlier unresolved findings above describe ORIGINAL source images and are retained as historical review, not current blockers.

The following revised PNGs are the frozen implementation references. Do not overwrite or regenerate them without recording a deviation. These were produced using the built-in imagegen tool with the original references as edit targets; Page 3 received one additional species-only correction. The originals above remain byte-exact. Prompts and generated source paths are preserved in image-generation-record.md/final-hashes.json.

| Final file | Bytes | SHA-256 |
|---|---:|---|
| page-1-locked.png | 2168593 | 238b273c3df788775103c17e46b6ae89dd997926e08760087feb744a929e9050 |
| page-2-locked.png | 2282543 | b4b7174aa2666d944babd49bdf22cd6d722f9a3807378d3759fa929cdfb0733e |
| page-3-locked.png | 2122050 | 2562fea2c7ab733b8b2199b9b6b128c97819ccbf4ff570d6deefea371e4009db |

Final forestry review: Page 1 has clearly distinct uniform plantation, selectively opened retained stand and developed mixed structure; broadleaf additions are broadleaf crowns/plants, not conifer substitutes. Page 2 planting image has a lobed-leaf Oak and oval-leaf Beech, no planted conifer. Page 3 has conspicuous tall productive Sitka, a lobed-leaf Oak and unlobed oval-leaf Beech at the front, intermediate broadleaf trees and regeneration; irregular spacing and cover remain. All three actual species are represented illustratively, not claimed botanically measured. No clear-fell/replacement-native-woodland implication. Pastoral Atlantic/Irish-compatible landscape retained.

Styling note: generated pixels approximate V2 colours; runtime UI must use actual existing shared V2 tokens, not sampled raster colours. Raster UI lettering is visual placement evidence only; copy-draft.md (renamed production-copy.md on lock) governs exact live text. No new palette or layout exception is authorised.

ONBOARDING REFERENCES LOCKED. Production may proceed from this lock; runtime reference fidelity and forestry review of Unity captures remain separate acceptance gates.
