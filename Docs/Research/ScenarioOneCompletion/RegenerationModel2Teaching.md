# Regeneration Model 2 — player teaching (Workstream J)

**Status:** teaching proposal and draft copy. **Production P1 copy is not edited.**

**Source status — read first.** The task packet calls Model 2 "accepted". The branch itself says otherwise: `task/understorey-recruitment-causality` @ `902903f`, `Model2Handoff.md`: **"CALIBRATION READY — PARAMETER DECISION REQUIRED."** The five-field save-18 schema was approved. The response strength, height window, recovery rates and initial/target cover rule are **not**. No main merge has happened. This document is therefore written against the *candidate* behaviour. Every line marked ◇ must be re-checked if the Manager changes a parameter or the target rule.

## 1. What Model 2 actually does [M2-CAND]

| Mechanism | Implemented rule | Evidence class |
|---|---|---|
| Two competitor states | Cell and patch `brambleCover`, `brackenCover`, independent fractions 0–1 | Schema approved |
| Where competitors grow | Shared target `0.8 × site × (0.1 + 0.9 × light)`: more light → more cover; deep shade is a floor, not absence | [I] site, [C] numbers |
| Bramble vs bracken | **Identical trajectories in a fresh game** (shared rule). Separate fields allow different histories | Explicitly *not* a botanical claim |
| Effect on young trees | Extra **survival** loss only: `0.35 × local cover × height vulnerability`. Full at ≤ 0.3 m, none at ≥ 1.5 m. Same for Sitka, Oak, Beech, natural and planted | [C] |
| What it does **not** affect | Height growth, seed supply, light, browsing | Implemented boundary |
| Clearance | Removes qualifying young trees and planted saplings in the footprint, and zeros competitor cover there | Existing footprint + M2 |
| Regrowth | Cover moves toward target 30 %/yr of the gap (rising), 45 % (falling) | [D/C] |
| Planting spot | 1 m² spot starts at zero cover, then recovers | Existing + M2 |
| Shelter | Browse protection only. **No** vegetation protection | Implemented boundary |
| Measured outcomes (39 100-year worlds) | Useful (dense opening, targeted early clearance: Sitka promotions 315 → 344 for €36); wasted (zero competition: €31.50, identical outcomes); harmful (repeated clearance every 5 years: promotions 262 → 0, €7,586) | [M2-CAND] Model2Handoff |

## 2. The three lessons

> **CLEARANCE MAY HELP · CLEARANCE MAY BE UNNECESSARY · CLEARANCE MAY DESTROY GOOD REGENERATION**

| Lesson | Game situation that shows it | What the player observes |
|---|---|---|
| **May help** | Young trees ≤ 1 m in a bright opening with dense bramble/bracken | After clearance, fewer young-tree losses to vegetation over the next years. The cover regrows |
| **May be unnecessary** | Dark cell, low cover; or young trees already above 1.5 m | Money spent, nothing changes |
| **May destroy** | A cell already holding established young trees, cleared anyway; or cleared repeatedly | Young trees removed by the clearance itself. Repeat clearance keeps resetting regeneration |

## 3. Player-facing interpretation by topic

| Topic | What to say | What never to say |
|---|---|---|
| **Bramble** | "Bramble: a thorny, sprawling shrub. Where it is dense it shades and smothers very small trees." | That bramble behaves differently from bracken in this game (it does not yet) ◇. That bramble is "bad" (it is also habitat; browse concealment is *not* modelled) |
| **Bracken** | "Bracken: a tall fern that can form dense cover in brighter places. Dense bracken can smother very small trees." | Seasonal die-back or litter effects (not modelled) |
| **Small-juvenile vulnerability** | "Seedlings and young saplings are most at risk while they are short — below about knee height. Once a young tree is taller than the vegetation (about 1.5 m), the vegetation no longer threatens it." ◇ | "Clearance makes trees grow faster" |
| **Clearance benefit** | "Clearing can help where dense bramble or bracken surrounds young trees that are still short." | "Clear before planting" as a rule |
| **Clearance harm** | "Clearance removes the young trees inside the area too. Check the preview: if good young trees are already there, clearing can undo them." | — |
| **Regrowth** | "Bramble and bracken grow back after clearance, faster in bright places. One clearance buys time; it is not permanent." ◇ | A recommended repeat interval |
| **Browsing separation** | "Vegetation and deer are separate problems. A shelter protects from browsing only. Clearing vegetation does not stop deer." | That bramble hides seedlings from deer (concealment is **not** implemented) |
| **Light link** | "Opening the canopy lets in light for young trees and for bramble and bracken." | That thinning *causes* bramble as a penalty |
| **Seed** | "Clearance does not bring seed. If no seed reaches a place, nothing new will grow there." | — |

## 4. Draft copy (concise; not yet production)

**Replacement for the P1 sentence on main** (exact text from `TutorialHandoff.md`, retained verbatim; applies only after Model 2 integrates):

> "Dense bramble or bracken can reduce young-tree survival. Clearance can improve future survival where these competitors are limiting, but it also removes qualifying young trees inside the preview. Check the targets and cost before approving. Vegetation can return. Clearance does not create seed, improve light, prevent browsing, or directly increase height growth."

**Ground report "Why" lines (new, Model 2 only):**

| Condition (authoritative) | Line |
|---|---|
| Juvenile ≤ 1.5 m and local max(bramble, bracken) ≥ HIGH band ◇ | "Dense bramble/bracken here threatens young trees that are still short." |
| Juvenile ≤ 1.5 m, cover LOW | "Ground vegetation is light here; it is not the main limit." |
| Juvenile > 1.5 m | "This young tree is taller than the ground vegetation." |
| No juvenile, cover HIGH, light OK | "Bright and covered in bramble/bracken. Seed arriving here would face competition." |

Bands LOW / MODERATE / HIGH for cover use fixed cut-points (proposal 0.2 / 0.5 [C]). No percentages are shown to the player.

**Clearance preview (Model 2):**

> "Clear 5 × 5 m · Removes: 2 young-tree groups, 1 planted sapling · Bramble/bracken: HIGH → none this year, returns over time"

**Annual Review REGENERATION line (Model 2 ledger, current year only):**

> "Young trees lost this year: mostly to shade (light) in 6 cells; to dense vegetation in 2 cells; browsing slowed 3 planted oak."

The M2 cause ledger is runtime-only, so this is the *current* year only. Order of attribution: vegetation → light → browse (implemented). Wording uses "mostly" because natural abundance is relative, not stem counts (D-047).

## 5. Sequencing in the tutorial (proposal)

1. **Never** introduce clearance before the player has seen young trees (Stage 4 of the arc in `ObjectiveRedesign.md`).
2. First exposure is diagnostic: the ground report names vegetation as a limit *somewhere*. The map gains a **Ground competitors** layer (M2) beside Regeneration.
3. The clearance lesson completes on **observation**: clear one cell, then revisit it and read the Annual Review the following year. It never completes on "clear N cells".
4. Contrast is allowed, not required: the learning step text invites the player to compare a cleared and an uncleared cell with young trees.
5. **Decision #9 (pedagogy):** move the clearance preview behind U. With Model 2 this matters more: the permanent preview implies clearance is a default.

## 6. Tests for the implementing packet

- No string claims a height-growth, seed, light or browse effect of clearance.
- No string claims bramble and bracken differ (until the model differs).
- No string recommends an interval.
- Model 1 / legacy saves keep the Model 1 sentence ("Ground-cover removal does not directly improve young-tree survival or height growth in this model…"). Model-dependent copy selected by `regenerationModel`.
