# CCF Multi 2D — Phase 2A-2 Forest Maker UI, Multi-Lane View, Lane-Filtered Reads & Save Schema v3 — Implementation Handoff

- **Date:** 2026-11-27
- **Feature branch:** `feat/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1`
- **Base:** `fa69d6d3d8c1789af531b53adefb5f426e90bbed` (`origin/integration/2d-sim`)
- **Verified code head:** `9100c0b9aa8b99d2e8c1ef17d1dcc46a29b76cba`
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A2-20261127`
- **Evidence root (outside the repo):** `/home/jer/Development/CCF-Multi-2D-Phase2A2-Evidence-20261127`
- **Spec:** `SPEC-multi-2d-phase2a-forest-maker-V1` (`specs/multi-2d-phase2a-forest-maker-v1/`), Step 2A-2, plus frozen contracts `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md`
- **Authority:** owner handoff packet of 2026-11-27; owner Proportionate Verification override of 2026-11-27 (see §9)
- **Implementer:** Gemini 4 Neon (high), single writer. The first run died on a provider network error after commit `a373145f`; the lane was resumed on the same worktree and branch, not restarted.
- **Coordinator:** DeepSeek V4.1 Flash (read-only; performed the final combine, proportionate checks and this handoff under the owner override)

---

## 1. Commit history (`fa69d6d3..9100c0b9`)

| Commit | Summary |
|---|---|
| `dab915dc` | Save v3 C-V3 envelope, `genesisRecipe` codec, reserved `economy` slot, v3 slots |
| `b5a43759` | `MultiSessionHost`, woodland & C-FENCE stand routes, v3 save/load, scaled budget, 503 overflow refusal |
| `cc60a195` | New Woodland modal, multi-lane view, mini-map, 1-lane toggle, C-FENCE client composition |
| `a373145f` | Phase 2A-2 Edge E2E suite, Core floor 312, UI polish |
| `81b2c6db` | Gate G4: retire the stale S0b "no codec under Save v2" lock, replace with positive C-V3 assertions |
| `9100c0b9` | 1280×720 topbar stays single-row with the Work plan subtitle |

38 files changed, +5,037 / −365.

---

## 2. What was delivered

### 2.1 Save Schema v3 (frozen C-V3 envelope)

- `src/Core/Saves/SaveV3Format.cs`, `SaveV3Codec.cs`, `SaveV3Slots.cs`, `FeatureBlockRegistry.cs`, `GenesisRecipeBlockCodec.cs`.
- `schemaVersion = 3`, strict canonical envelope, ordinal-sorted top-level `blocks`, each block exactly `{blockVersion, data}`, checksum over all canonical bytes except the checksum object, size cap and duplicate/extra-key refusal inherited from v2 discipline.
- Fail-closed: unknown block key refused; unreadable `blockVersion` refused; missing block required by the session's model selection refused; nothing defaulted or repaired.
- `genesisRecipe` (Phase 2A) carries the canonical recipe, `recipeHash`, `generatorNote` and the genesis/model-selection state needed for self-contained verification.
- `economy` is **registered and reserved**: `FeatureBlockRegistry` exposes the slot with its typed refusal code, and there is **no implementation**. An economy-enabled forest-maker stand therefore refuses to save with `SaveEconomyRequiresV3` (HTTP 409) rather than silently dropping money state — proven in the Edge suite.
- `src/Core/Phase1/**` untouched; the v2 golden save is still exactly **24,703 bytes**, `SchemaVersion = 2`, with zero in-place rewrite and no silent v2→v3 migration. MA-1 / MA-1-MIXED still write v2 byte-for-byte.
- v3 load order: v2 gates → recipe bounds/hash → regenerate and match `manifestContentHash`/areas/seeds/hectares → model vector `1/1/1/1/1` + lineage → build a fresh `MultiAreaContinuation` and run the existing `Restore` checks → atomic swap minting a new session id.

### 2.2 New Woodland

- `New woodland` button beside `Saved games`; accessible dialog (`role=dialog`, focus trap, Esc, focus return), same dark woodland styling.
- Choices: MA-1, MA-1-MIXED, both 0.64 ha presets, Custom rectangle; Money On/Off at Year 0 carried into the session and round-tripped through v3.
- `POST api/multi/woodland/preview` returns Core-exact numbers (`1,344` stems, `1,024` cells) and Core refusals verbatim (`ForestMakerStemCapExceeded` for >3,000). No client-side bounds, counting or geometry math.
- Unsaved-progress guard: unconfirmed replacement returns 409 `WoodlandUnsavedProgress` and leaves the active stand untouched; a pre-swap fence is refused with 409 `PlayerSessionMismatch`.

### 2.3 Active lane + neighbours, mini-map, 1-lane toggle

- 1920×1080 and 1280×720: outer lane → 2 lanes, interior lane → 3 lanes, open grassland outside the perimeter, 5 mini-map lanes with 1,344 stems, 5-pill lane strip.
- Mini-map click and keyboard `Enter` switch lanes; 1-lane toggle isolates the active lane; visible-neighbour tree click routes with the exact area id (`MA-D-0072`).
- Two-area stands keep the existing layout exactly (`multi=false`, "Both areas").

### 2.4 Lane-filtered reads, C-FENCE, payload

- `GET api/multi/stand?lanes=MA-A,MA-B,MA-C` (+ a POST form) returns a fenced, lane-filtered detail read under one `sync` lock, carrying the identical `MultiFenceMembers` fence. Stale fence → 409; unknown lane → 409.
- Browser composes a frame only from byte-identical fences.
- Measured (Edge, 1280×720): overview `78,607 B`; 3-lane GET `490,910 B`; 3-lane POST `495,027 B`; all `< 524,288 B`.
- Core-side post-change table (`m0-post/results.jsonl`, Y0 / Y15):
  - primary preset: overview 78,607 / 112,829 B; 3-lane 495,027 / 508,505 B; 1-lane 167,370 / 214,648 B; raw whole-view 1,586,502 / 1,691,399 B
  - pure Sitka: overview 78,543 / 110,447 B; 3-lane 494,905 / 506,768 B; 1-lane 167,244 / 211,942 B; raw whole-view 1,586,011 / 1,683,357 B
  - **The raw whole-woodland view exceeds 512 KiB; the lane-filtered read is what brings it under.** Y15 3-lane headroom against 524,288 B is ~15.8 KB — reported honestly.
- Budgets: 524,288 B for ≤2 areas; `min(2,621,440, 524,288 × ceil(genesisStems/300))` above that. The silent-overflow fallback is gone: over-budget now returns typed HTTP 503 `MultiViewPayloadBudgetExceeded` and the page keeps the last good view (proven with a forced 400,000 B test budget: `bytes=1084530, budget=400000`, crowns 171 → 171).

### 2.5 M0 / speed (environment: .NET 8.0.133, Linux 6.18, 6 CPUs, headless Edge)

| Metric | Observed | Q5 target |
|---|---|---|
| Core snapshot median (Y0 / Y15) | 1.07–1.56 ms | ≤ 250 ms |
| Cold snapshot Y15 | 353–354 ms | – |
| Annual advance median (Y15) | 606–608 ms | ≤ 2,000 ms |
| JSON parse | 2.4–9.5 ms | ≤ 150 ms |
| SVG 3-lane build | 76–91 ms | ≤ 500 ms |
| SVG 1-lane build | 30–34 ms | – |

All Q5 targets met; the M0 pre-change run is preserved in `m0-core.log`, post-change in `m0-post/`.

### 2.6 Gate G4 lock retirement (disclosed decision)

`build/2d-gate.sh` G4 previously asserted, as a temporal S0b lock, that **no** type may implement `IFeatureBlockCodec` while `SchemaVersion == 2`. Phase 2A now implements C-V3 by design (approved spec §6; S0b §4 item 4 "register an `IFeatureBlockCodec` … once Phase 2A implements C-V3"), so the lock is stale and failed the gate. It was replaced with a **stronger, config-driven** assertion set (`build/2d-gate/gate-config.json`):

1. v2 golden still byte-exact (24,703 B, pinned file SHA, embedded checksum, `SchemaVersion = 2`);
2. `src/Core/Phase1/**` unmodified vs base;
3. codecs only under `src/Core/Saves/`;
4. every codec registered in `FeatureBlockRegistry`;
5. `economy` reserved with no implementation;
6. `genesisRecipe` implemented with a positive `blockVersion`;
7. zero v3 references from the v2 decode path.

Observed G4 line: `v2 golden verified (bytes=24703 … SchemaVersion=2, src/Core/Phase1 unchanged; SaveV3=enabled(codecs=[genesisRecipe:v1(GenesisRecipeBlockCodec)], reserved=[economy], phase1V3Refs=0))`. No floor was lowered; Core `minPassed` moved 307 → **312** (upward only).

---

## 3. Verification actually run (all on `9100c0b9`)

| Check | Result | Evidence |
|---|---|---|
| Clean builds (`.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj`) | 0 warnings / 0 errors | `gate-final/g1-build-*.log` |
| `Core.Tests` | **312/312 passed, 0 failed** | `gate-final/g2-core-tests.log` |
| `Phase1.Save.Tests` | **50/50 passed, 0 failed** | `gate-final/g2-save-tests.log` |
| `ui-smoke-multiarea.sh` | **363 passed, 0 failed** | `gate-final/g2-ui-smoke-multiarea.log` |
| `ui-smoke.sh` | 250 passed, 4 known T0 #885 IDs (L01, L02, L06, J31); G11b PASS | `gate-final/g2-ui-smoke-single.log` |
| `cli-safety.sh` | 23/23 | gate log |
| `build-output-safety.sh` | PASS (C1–C6+P0; C7/C8 HOST-NA under 8.0.134; `.p0ref` restored and re-verified) | gate log |
| `p0-isolation.sh` | PASS (decoy 13/13, protected hits 0) | gate log |
| Module graph | PASS, 16 modules, acyclic | gate log |
| G3 frozen anchors | **9/9** | `gate-final/g3-anchors.json` |
| G4 v2 golden + C-V3 | PASS (24,703 B) | gate log |
| G5 payload budgets | 5/5 | gate log |
| G6 determinism | F1=0, F2_unlisted=0, RNG_violations=0 | gate log |
| G7 protected paths | PASS (158 paths) | gate log |
| Stage B browser suites (legacy) | PASS | gate log |
| **Full `2d-gate.sh --browser`** | **VERDICT: PASS (exit 0)** | `gate-final/gate-summary.json` |
| Phase 2A-2 Edge E2E, 1920×1080 | 36 passed / 0 failed overall (20 full + 4 restart + 5 overflow, plus 1280 restart/overflow) | `phase2a-edge-final/` |
| Phase 2A-2 Edge E2E, 1280×720 | 20 passed / 0 failed | `phase2a-edge-final-vp1280/` |
| v3 save → **genuine process restart** → load → continue to Year 3 | PASS at both viewports | Edge logs + screenshots |
| Console / CSP / 404 / 5xx | 0 | Edge logs |
| `.p0ref` | `9a99f781…34b3` unchanged | gate log |
| Forbidden-path diff (`Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/`, `src/Ui/wwwroot/`, `SpeciesWave2`, `WoodlandImport`, `NaturalProcesses`, `src/Core/Phase1`, `.p0ref`) | **empty** | `git diff --name-only` |
| `specs/**` diff | **empty** (no spec edits, no ticked checklists) | `git diff --name-only` |
| Protected listeners `:33767`, `:43649`, `:46593` | untouched | gate preflight |

---

## 4. Not verified / residual risks

1. **`build/2d-gate/selftest.sh` was not completed.** It ran ~58 minutes and was stopped at case C26/29 under the owner's Proportionate Verification override of 2026-11-27 (§9). This merge *did* modify a gate script, so the override's own conditional would normally require selftest; it is **deferred to the Playable Milestone 1 closeout** and is the single outstanding gate item. The full browser gate did pass at the same code head.
2. **Y15 3-lane headroom** is ~15.8 KB under 524,288 B (508,505 B). It is inside the 2.5 MiB scaled budget for ≥3-area stands, but the narrow 512 KiB number is tight and should be re-measured if per-tree detail grows.
3. **Economy persistence is deliberately blocked**: economy-enabled forest-maker stands refuse to save (`SaveEconomyRequiresV3`) until the Phase 2B Economy v3 block addendum lands. This is fail-closed, not silent loss, but it is player-visible.
4. Two pre-existing 2A-1 review minors (Minor-1 seed-source attribution, Minor-2 corner-matrix code assertion) remain unaddressed; they are test-strength items, not defects.
5. Preset anchors 10–11 remain test-pinned, not in `frozen-anchors.json`.
6. No Unity, no Windows build, no 3D verification (out of scope).
7. Independent review of this lane is **not** part of this handoff; it is a separate read-only thread.

---

## 5. Scope proof

`git diff --name-only fa69d6d3..9100c0b9` touches only: `src/Core/Saves/**` (new v3), `src/Core/MultiArea/{ForestMaker/*,MultiAreaContinuation.cs,MultiAreaExperiment.cs}`, `src/Ui/**` (not `wwwroot`), `src/Ui/multi/**` modules, `tests/**` (Core, phase2a-ui), `build/2d-gate/{2d-gate.sh,baseline-counts.json,gate-config.json,module-graph.mjs}`, and this handoff. Zero diff under every forbidden path, zero diff under `specs/**`, zero edit to `origin/main`.

---

## 6. Proportionate combine (owner override 2026-11-27)

Per the owner directive, the combine onto `origin/integration/2d-sim` used only the fast proportionate check: clean `--no-ff` merge onto the current green tip, forbidden-path fence, clean .NET 8 build, `Core.Tests` (floor + 9 anchors), `Phase1.Save.Tests` (persistence was touched), and this feature's own targeted browser suite. The full legacy Edge sweep and `selftest.sh` were skipped as instructed; see §9 of the coordination report and item 4.1 above.
