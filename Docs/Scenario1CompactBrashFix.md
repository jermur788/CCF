# Scenario 1 — compact brash fix

Presentation-only fix on `integration/scenario-one-complete` (base `2b4d481`), for the interactive-smoke report that post-felling Sitka brash was too large, too widespread and stacked implausibly. It changes no timber yield, retained timber, deadwood, cash, tree position, ecology, felling count or save field.

## Previous fix

`/home/jer/CCF` (`feature/forestry-ecology`, uncommitted) holds a compact-residue change in `ScenarioOneManager.SpawnFellingResidueVisual`, plus matching interaction-harness checks. It is presentation only:
- it sizes the pile from its own bounds to a 0.95–1.55 m diagonal;
- it grounds the pile and places it beside the stump;
- it moves the spawn call after `expectedVolumeM3` is set;
- it guards against same-frame duplicates.

It mixed in no gameplay. Its approach was ported, but its size was not: at that size the piles fell back to two or three needle sprays and no longer read as recent work.

## Root cause (before)

`SS_Brash_Green_01/02` and `SS_Brash_Dry_01/02` (`Art/ForestryGround`, project-authored, import scale 1, 1 unit = 1 m) are each one baked pile representing a whole crown's boughs:
- footprint 6.7 × 5.1 m, height 1.36 m (dry piles 6.3 × 4.8 m, 1.08 m);
- about 56 woody bough segments longer than 0.5 m (longest 2.7–3.2 m) and about 650 needle-spray cards of 0.6 m, counted from the delivery's `Preview_brash.blend`.

The game placed one pile per felling at 0.85–1.25× scale, 0.9–2.0 m from the stump. There was no import or prefab scale bug: the code used a crown-sized pile as if it were stump-sized.

Measured after one representative felling (P1109, 13.3 m, DBH 17 cm):
- world bounds 7.4–7.8 × 7.9–8.2 m;
- height 1.35–1.45 m green, 1.0–1.05 m dry;
- centre 0.8–1.6 m from the stump.

Branches reached about 4 m in every direction, so adjacent fellings merged into one brash field.

## Change

In `ScenarioOneManager.SpawnFellingResidueVisual`:
- **Footprint.** The pile is normalised from its own renderer bounds to a 1.9–2.4 m longest horizontal footprint [D]: 1.8 + 0.6 × ∛(resolved stem volume), clamped, with ±8 % deterministic variation. That is a scale of about 0.30, so the longest bough is about 0.9 m and typical bough segments about 0.2 m.
- **Height.** Vertical scale is a further 0.8× [D]. Boughs lie closer to the ground plane and overlapping pieces compress into a low mat (about 0.35 m green, 0.28 m dry).
- **Placement.** The lowest point sits 1 cm into the ground, and the patch centre is about 1 m from the stump, so its near edge reaches the stump. Yaw and offset bearing are deterministic per work order.
- **Duplicates.** A same-frame refresh no longer shows old and new patches together: they are hidden and renamed before the deferred `Destroy`.
- **Spawn order.** The spawn call now runs after `expectedVolumeM3` is set, so live felling and save/load size the patch identically.

The visible quantity is an abstraction: one compact patch per felling, regardless of residue volume. Fresh (green) and dry variants, and the five-year appearance switch, are unchanged, and both use the new placement.

`ScenarioOneInteractionVerification` gains the ported `VerifyFellingResidue` and `FellingResidueSignature` checks:
- one patch per completed Sitka felling;
- world bounds under 3.1 m and height under 0.5 m;
- centre within 1.6 m of the stump;
- grounded;
- unchanged by a double same-frame refresh and by save/load.

## Result

Measured in the same five-felling scenario:
- own-axis footprint about 1.9–2.4 m (world-aligned bounds 2.3–2.6 m, because they are yawed);
- height 0.34–0.36 m green, 0.27–0.28 m dry;
- lowest point −0.01 m;
- centre 0.91–0.97 m from the stump.

Rendered views (graphics batch mode, player height) show:
- the stump stays identifiable, with a low patch of sprays and short boughs beside it;
- adjacent fellings stay separate patches;
- planted oaks remain readable;
- no hovering pieces.

The dry state is subtle: thin grey sticks.
