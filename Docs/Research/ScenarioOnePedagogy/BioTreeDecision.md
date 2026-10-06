# Decision paper 2 — Bio Tree designation

**PRODUCT DECISION REQUIRED.**

## Question

Should the player be able to mark a tree as retained for **ecological** reasons (a "Bio Tree"), distinct from a Crop Tree retained for timber?

## Evidence

- Irish marking guidance separates **Quality (Q) trees** (future sawlog) from **Bio trees**: trees kept for unusual ecological value, under-represented seed species, veterans, cavity trees, deadwood [PRAC §11.5]. The Irish eight-category paint system includes bio-tree retention [PRAC §11.6].
- The synthesis itself calls Bio Tree "the strongest candidate for a genuinely distinct future retained-tree designation", and warns that "if accepted, it should express management intent and protection, not fabricate biodiversity points" [PRAC §11.6, §19].
- Accepted decisions: marks are None/Fell/CropTree, mutually exclusive (D-013). D-020 forbids inventing habitat state.

## What the simulation can say about a tree's ecological role today [REPO]

| Possible Bio-Tree reason | State exists? |
|---|---|
| Under-represented species (e.g. a broadleaf among Sitka) | **Yes** (species), but at the start every tree is Sitka. Planted broadleaves join the canopy only after promotion (years later) |
| Seed source for that species | Yes (maturity), but Sitka is everywhere |
| Veteran / large old tree | Partly (DBH, age), but Scenario One trees are all the same age |
| Cavities, cracks, microhabitats | **No** |
| Standing deadwood | **No** (no ring-barking or standing-dead state; D-020 assets remain unspawned) |
| Fallen deadwood | Yes, but that is a felling outcome, not a standing tree |

## Options

| | A. No new mark | B. Bio Tree mark | C. Contextual "retained for habitat" state |
|---|---|---|---|
| Player expression | Leave the tree unmarked | Explicit third mark (green/white), exclusive with Fell/Crop | A reason tag on an *unmarked* retained tree, shown in history |
| Teaching value | Low: intent is invisible | High: separates timber from ecological intent | Medium |
| Risk of fake ecology | none | **High today**: there is little authoritative habitat state for the mark to point at | Medium |
| Save / anchor impact | none | `TreeMarkType` value (persisted, v13+); exclusivity rules; harnesses that enumerate marks; HUD/map legend | event or tree tag (save) |
| D-021 (no shortcut accumulation) | — | Needs a new key or a context choice | Context choice |

## Recommendation

**Option A now. Revisit B when at least one habitat-tree state exists** (e.g. standing deadwood from ring-barking, cavity/veteran attributes, or a mature broadleaf seed tree in the canopy).

Reasons:

1. In the Year-0 Sitka monoculture there is almost nothing a Bio Tree could truthfully represent. The mark would be empty intent, or would tempt the UI to invent habitat value.
2. The concept can still be **taught**: one honest line in the Crop Tree explanation, "Foresters also keep some trees for wildlife rather than timber — old trees, trees with holes, rare species. This game will add that choice when it can show what those trees do."
3. When the first planted oak or beech joins the canopy (promotion), the case for B becomes real (seed source of an under-represented species). That is the natural trigger to reopen this decision.

## If the user chooses B anyway

Minimum safe design: a third **retention** mark, exclusive with Fell/Crop; it blocks Fell; it is shown on the map's marks layer. It confers **no** score, bonus or habitat value of its own. The review shows only true facts ("species: sessile oak — the only oak seed tree in this cell"). Requires a save-schema review, harness updates (marks), copy and legend. Sequence after Sol's save work.
