# Sapling price authority audit (P3.11)

**Current code basis:** `origin/main` @ `869ee92` (save v19, P2 and Storm integration). The price authority finding from the earlier `1fbefd8` inspection still applies; this is a static audit, not a price change. **No price is changed by P3.**

## Finding

| Value | Where | Used for | Drives player cash? |
|---|---|---|---|
| Beech **€4.50**, Sessile Oak **€5.50** per sapling | `ScenarioOneDefinition.shopEntries` (code default and `ScenarioOne.asset`), tooltip "[D] provisional gameplay calibration" | `TryPurchaseStock` (`total = quantity × offer.unitPriceCents`), Work Plan nursery card price | **Yes.** This is the only sapling price the player pays |
| Beech **€0.95**, Sessile Oak **€1.00** (and Sitka €0.45) | `WorkEconomy/Stage1EconomyDefaults` price book (`Plant(...)`), source "Report p.10: None-So-Hardy 2025–26 wholesale list, ex VAT, not complete planted cost" [E] | Validation (`ForestryEconomyValidation` requires a nursery-stock price entry for every planting material) | **No.** Planting quotes use `MaterialSupply.ExistingStock`, and the calculator charges only purchased-for-job materials. Stock was already paid for at the definition price |

## Why both exist

`ScenarioOneEconomyAdapter.PriceBook` intends to add each definition offer to the book ("Existing live nursery price; consumed stock is not repurchased"), but only `if (book.FindMaterial(offer.itemId) == null)`. The Stage 1 defaults already define the **same item ids** (`beech-sapling`, `sessile-oak-sapling`). So the definition price never enters the book, and the book keeps the wholesale values.

## Is it a real conflict?

- **Today: inert.** No cash path reads the book's nursery price for these items. Player-visible prices and charges are consistent (€4.50 / €5.50).
- **Latent risk:** any future code that values stock from the book, or plants with `MaterialSupply.PurchaseForJob` (e.g. a "buy and plant in one job" option, or an economy report valuing inventory), would silently use €0.95 / €1.00. A reviewer reading the price book would also see different numbers from the game.
- **Evidence status:** €0.95 / €1.00 are cited wholesale nursery prices [E], explicitly *not* complete planted cost. €4.50 / €5.50 are [D] placeholders with no stated basis. The report gives no delivered small-order retail price.

## Recommendation (Manager / economy-owner decision)

1. **Choose one authority** for what the player pays. The definition is the current authority; keep it unless the economy owner decides otherwise.
2. **Make the difference explicit, not silent.** Either:
   - (a) document the definition values as "delivered small-order price incl. VAT and handling [C]" and rename the book entries' role to "wholesale reference [E]" (no gameplay change); or
   - (b) let the adapter override book entries with the definition price for Scenario One (one-line change in `PriceBook`; economy gate re-run; no cash change today); or
   - (c) adopt the wholesale price plus an explicit handling/delivery component (a price change: changes the economy anchors and the completion minimum cash).
3. **Recommended: (b) + (a)**, with no player-visible change. It removes the latent hazard and records why €4.50 / €5.50 exceed the wholesale list. Out of P3 scope; needs the economy owner.
