# Failure and recovery (Workstream O)

**Status:** design proposal. Distinguishes **recoverable mistakes** from **true scenario failure**. The hard-fail question is a **PRODUCT DECISION**.

## 1. Current failure model [REPO]

| Rule | Code | Reachability |
|---|---|---|
| Failed if cash is **exactly** €0.00, the contractor rate > 0 and no approved orders | `ScenarioOneManager.cs:2221` | Rare: spending is blocked when cost > cash, so cash seldom lands on exactly zero |
| Failed if Year 100 is reached without completion | `:2225` | Reachable |
| **Hidden dead end:** cash < €2,500 (+ other approved work) before any thinning → `managed-opening` can never be met | `ApprovePendingWork :1111` | **Reachable**: e.g. buying more than about €9,500 of stock before the first thinning. Reported only at Year 100 |

## 2. Mistakes, their recovery path, and whether the game supports it

| Mistake | What happens in the model | Recovery path in the model | Supported now? | Missing |
|---|---|---|---|---|
| **Cash low** | Cannot hire a harvest. Timber is the only income | (a) none for harvest; planting with own labour still needs stock money | **No** for harvest | Warning before spend; a standing-sale option (buyer bears harvesting, [ECON] §sale basis) or net settlement at approval (economy decision) |
| **Excessive thinning** | Low density, big openings, light; canopy may approach 0.35; (M2) more bramble/bracken; (storms) Exposed edges | Time: crowns spread, regeneration in openings, RD recovers over ~15 years | **Yes**, unless canopy < 0.35 at Year ≥ 25 (current check is final-year only) | Explain the recovery: "Openings close over time" (Annual Review trend) |
| **Poor regeneration** | Few cells; light-limited or no seed | Thin over seedlings; wait for Sitka seed; enrichment planting | **Yes** | Diagnosis "Why" already names light/seed. Good |
| **Browse damage** | Leaders browsed; height delayed; unprotected planted stock lost | Shelters on new planting; replant | **Partly**: shelters only for new planting; natural regeneration cannot be protected | Accepted deferral (D-044). Say so in help |
| **Failed planting** | Planted juveniles die (light, browse, M2 vegetation) | Buy and plant again (cash) | **Yes** while cash allows | Per-cause loss line (M2 ledger) |
| **Bad clearance** | Removes young trees in the footprint | Seed rain recolonises; replant | **Yes** | Under M2 a repeated clearance can reset regeneration to zero: show it in history |
| **Storm damage** (after storms) | Windthrow, deadwood, gaps, Exposed edges | Salvage (cash) or leave; gaps regenerate | After storms | Storm calibration must keep completion viable (storm decision #22) |

## 3. Does Scenario One need a hard fail state?

| Option | For | Against |
|---|---|---|
| **Keep hard fail** (current: cash 0, Year 100) | Stakes | The cash-0 rule almost never fires; the real dead end is invisible until Year 100. "Failure" at Year 100 contradicts "multiple credible strategies" |
| **No hard fail; outcome = "not yet completed" + Century Review** | Matches a learning scenario and the brief's no-single-prescription principle; the player always sees consequences | Less tension |
| **Soft fail with a guided restart point** | Recovery without frustration | Needs checkpoints; restart can hide consequences |

**Recommendation: no hard fail in Scenario One.**

1. Replace "Failed" at Year 100 with **"Century Review — objectives not completed"**, showing the same multidimensional review.
2. Remove or reword the cash-0 failure. Instead, **detect the true dead end** (no thinning yet *and* cash < next harvest minimum) and say so the moment it happens:
   > "You do not have enough cash for a contractor's minimum harvest visit (€2,500). In this scenario timber sales are your only income, so you cannot start thinning. You can still walk, plant with your own labour using stock you own, and observe the forest — or start a new forest."
3. **Prevent it rather than punish it:** a Work Plan warning when approving or buying would take cash below the next harvest minimum before a first thinning:
   > "After this purchase you would have €1,900 — less than one harvest visit (€2,500)."
4. A real **recovery** route needs an economy decision (owner: economy, not pedagogy):
   - (a) **standing sale** for the first thinning (buyer bears harvesting; the owner receives a lower standing price) — a real Irish arrangement [ECON], and a good lesson about sale basis;
   - (b) net settlement (approve if expected revenue + cash ≥ cost);
   - (c) a policy-layer CCF grant (WIS €1,200/ha per intervention [ECON] → €192 on 0.16 ha: too small to rescue on its own).
   Recommendation: (a), if any; decide with the economy owner.

## 4. True failure (if any is kept)

Only one candidate is defensible: **loss of the forest itself**, i.e. continuous cover broken by player felling (mean canopy < 0.35 caused by felling) — a clear-fell in a CCF scenario. Even then, recommend a **"scenario goal no longer reachable"** state with explanation and continued play, not a game-over.

## 5. Save and anchor impact

- Warnings and wording: none (UI).
- Removing the cash-0 failure rule and renaming the Year-100 outcome: changes `EvaluateProgress` → **completion anchor and outcome semantics**. Saved `outcome = Failed` values in old saves must keep loading (map them to "not completed" in the UI only).
- Standing sale: economy change, owned by the economy single-writer; changes the economy anchors.
