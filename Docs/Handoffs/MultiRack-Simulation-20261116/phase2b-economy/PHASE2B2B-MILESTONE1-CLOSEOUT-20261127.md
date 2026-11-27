# Phase 2B-2b Part 2 — Playable Milestone 1 closeout (integration record)

**Date:** 2026-11-27 · **Role:** single integrator · **Status:** combined on `integration/2d-sim`; NOT merged to `main`; one carried pre-existing defect (see §6)

| Item | Value |
|---|---|
| Integration base | `24871551ae906b6911b48bb17c252c023bdea502` (`origin/integration/2d-sim`) |
| Feature candidate | `cd907446282ca6a0f12fdbc1d33dc03076f6ac58` (28 files, +2637/−114) |
| Independent review | `112e5dfd2d053bd9b2892812b6653ef871c991c6` — **INTEGRATION DECISION: APPROVE**, no blocking findings |
| Merge commit (`--no-ff`) | `92df88d589a877e0fe9839b803e2983b23a6602d` |
| Integrator delta | `07a91679` (see §3) |
| Protected `origin/main` | `bdeb6a324acaf4129c10fb0846faac8b64974360` — untouched |

## 1. What merged

Three-way felling dispositions (Sell & Extract / Keep for Own Use / Leave as Fallen Deadwood) wired into `/multi` and its HTTP routes; authoritative retained timber (m³, total and per species) in the top strip and the single History drawer; fallen deadwood materialised into Stage 4 on Execute; the S1 rule (Leave refuses at choice/quote time when `Stage4` is null); and the `blocks.economy` Save v3 feature block with Save → server restart → Load persistence.

## 2. Why an integrator delta was needed

The reviewed candidate could not pass gate G4:

- it placed `EconomyFeatureBlockCodec` under `src/Core/Phase1/`, while G4 requires every `IFeatureBlockCodec` to live under `src/Core/Saves/`;
- it added an economy-aware `Phase1EngineLineage` overload to `src/Core/Phase1/Phase1SaveFormat.cs`, while G4 keeps `src/Core/Phase1/**` byte-identical to base and forbids v3 references there;
- `build/2d-gate/gate-config.json` still listed `economy` under `reservedOnlyBlockKeys`, asserting it is *not* implemented.

## 3. Integrator delta `07a91679` (owner-authorised)

| Change | Evidence |
|---|---|
| `EconomyFeatureBlockCodec.cs` moved `src/Core/Phase1/ → src/Core/Saves/` | blob `a79884f3…` identical before and after the move; namespace already `SitkaP1.Core.Saves` |
| `Phase1SaveFormat.cs` restored exactly to base | blob `18bea769…` equals base; `git diff base..HEAD -- src/Core/Phase1/` empty |
| `SaveV3Codec.LineageFor(models, economy)` added; used by `CaptureSave` and `VerifyRecipeGenesis` | same expression as the removed overload; no behaviour change |
| `gate-config.json`: `economy` moved to `requiredCodecs` (v1), `reservedOnlyBlockKeys` emptied | strictly stronger: the codec must now exist, be registered and sit in Saves |
| `baseline-counts.json`: core 312 → 314, save 50 → 52 | floors raised, never lowered |

No change to dispositions, retained timber, deadwood or the v2 path. All suites were re-run after the move (314/314, 52/52, 22/22).

## 4. Verification

| Gate | Result |
|---|---|
| `build/2d-gate.sh --browser --base 24871551 --p0ref-dir <verified 9a99f781…>` (`gate-1`, pre-fix) | G0/G1/G2/G3 **PASS** (Core 314/314, Save 52/52, ui-smoke 363/363, cli-safety 23/23, 16 modules, 9/9 anchors); G4 **FAIL** on the codec location |
| `gate-2` (post-fix) | G2 stopped on the documented NB1c WSL2 flake (`expected 1, got 3`); one permitted retry |
| `gate-3` (post-fix) | **G0–G7 PASS** (Core 314/314, Save 52/52, ui-smoke 363/363 baseline drift D-…, cli-safety 23/23, build-output-safety, p0-isolation, 9/9 anchors, G4 `economy:v1` with `phase1V3Refs=0`, G5 5/5 payload checks, G6 determinism, G7 protected paths 175 checked); **Stage B FAIL** — pre-existing, see §6 |
| Base probe (same suite on unmodified `24871551`) | **also FAIL (55 passed / 10 failed)** with byte-identical sweep sizes: `ma-1-mixed` Y00–Y06 = 393501/400141/404285/406670/408943/412640/415260 in both trees |
| `tests/phase2b2b-ui` (independent run) | **66/66 PASS**: 29/29 @1920×1080, 29/29 @1280×720; real server restart → Load restores cash €9,505.23, hours 39.8/40, retained 0.126 m³, `DW0001` fell deadwood, ledger and dispositions; zero console/CSP/404/5xx; zero h-scroll; one History button; ≤3 columns |
| Deferred G4 mutation check | baseline PASS; **3/3 mutations killed** (golden byte length, unregister economy codec, v3 reference in Phase1), each restored byte-identically |
| Deferred `build/2d-gate/selftest.sh` | see §5 |

## 5. Deferred selftest (Forest Maker Part 2 note)

`selftest.sh --p0ref-dir <verified seed>` → **26/29 passed, 3 failed, exit 1**.

| Case | Result | Attribution |
|---|---|---|
| C25-g7-denylist | FAIL (expected 4, got 1) | G2 aborted first on the NB1c WSL2 flake; the planted G7 violation was never reached |
| C26-clean-uismoke-exact-four | FAIL (expected 0, got 1) | the clean `--browser` gate reaches Stage B and hits the **pre-existing** payload-budget failure (§6); this case cannot pass until that is fixed |
| C29-g2-s0c-hook-inventory | FAIL (exit matched, marker missing) | G2 aborted first on the same NB1c flake |

All other cases passed, including the mutation cases that prove the gate still discriminates: C09 (dropped test), C11 (G4 Phase1 tamper), C21 (asset identity), C22 (move proof), C23 (G5 budget), C24 (RNG literal), C27 (module tamper), C28 (retired module). None of the three failures is caused by this lane: C25/C29 are flake-aborted, C26 is the carried defect below.

## 6. Carried defect (pre-existing, not this lane)

`tests/phase1-time-history-ui/run-step2-edge.sh` fails at both base and this tree: the `/multi` view crosses the 512 KiB payload budget at year 7 of the 30-year sweep (`503 PAYLOAD BUDGET EXCEEDED`, `bytes=524551` / `527778` vs `budget=524288`), so `Z02` reports server stderr. The base probe reproduces identical pass/fail totals (55/10) and identical per-year sizes, so this lane adds no bytes to that payload. Owner ruling 2026-11-27: proceed, record as a carried defect, and open a separate follow-up for the Year-30 payload ceiling. Suggested follow-up: compact the deep-history projection rather than raise `MultiViewBudget`.

## 7. Other carried items

- **SF-1** (reviewer, non-blocking): the economy block is validated against itself but not cross-checked against the rest of the save. Engine-produced saves are consistent.
- **SF-2** (reviewer, non-blocking): `tests/phase2b2a-economy-ui/drive-economy.mjs` still asserts the superseded "dispositions deferred" strings and will fail if run.
- **Pending dispositions** reset to Sell & Extract after a v3 load by design (§11.1 of the engine handoff).
- **Host `p0ref` note:** `build/build-p0-core-ref.sh` under SDK 8.0.134 produces `bcbdbf05…` on this host, not the pinned `9a99f781…`; the closeout used the external verified seed per the owner ruling.

## 8. Roadmap sync

`2D_Simulator_Strategic_Roadmap.md` v1.20 was **not** applied: the repo holds v1.18 and no v1.20 text exists in Git history or branches. Owner ruling 2026-11-27: skip the sync for now; the Strategy Manager supplies the text later.

## 9. State (D-030)

- Isolated branch **verified** and combined on `integration/2d-sim`.
- **NOT merged** to `main`; `origin/main` `bdeb6a32` untouched.
- Not user-accepted; no Windows build; no external demo.

## 10. Owner playtest — Playable Milestone 1

```bash
cd Tools/MultiRackSitkaP1Lab
export DOTNET_ROOT="$HOME/.dotnet"
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

Open the printed `…/multi` address in Microsoft Edge on Windows (the port is chosen per launch; `localhost` is refused). The `.p0ref` seed must already be present and hash-verified as described in `Tools/MultiRackSitkaP1Lab/README.md`. Toggle **Money: On** at Year 0 to see the disposition selector, retained-timber strip and Save v3.

## 11. Evidence

`/home/jer/.ccf-orch/evidence/2d-m1-closeout-20261127/` — `gate-1`, `gate-2`, `gate-3`, `base-probe/step2-base.log`, `phase2b2b-ui/` (66/66 plus screenshots and expected JSON), `g4-mutation/`, `selftest/`.
