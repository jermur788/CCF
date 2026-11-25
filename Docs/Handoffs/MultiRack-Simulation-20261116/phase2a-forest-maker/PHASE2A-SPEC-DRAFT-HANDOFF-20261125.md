# Phase 2A Forest Maker — DRAFT Spec Quartet — Step A Return Handoff (2026-11-25)

**Canonical status:** `DRAFT QUARTET PUBLISHED — AWAITING OWNER APPROVAL`
Zero code, zero tests, zero builds, zero servers, zero self-approval. Five Markdown files added. Nothing else changed.

---

## 1. Identity and provenance

| Role | Requested | Selected | Resolved | Executed | Effort (req / exec) | Thread ID |
|---|---|---|---|---|---|---|
| Coordinator | DeepSeek V4.1 Flash | UNKNOWN (not observed here) | UNKNOWN | UNKNOWN | UNKNOWN / UNKNOWN | UNKNOWN |
| Spec Author | Claude Opus 5.5 (`claudeAgent / claude-opus-5-5`) or Neon | Claude Opus 5.5 | `claude-opus-5-5` (harness-reported model ID) | `claude-opus-5-5` (harness-reported) | not specified / UNKNOWN | UNKNOWN (not exposed to this session) |

**Routing note:** the dispatch was labelled "Coordinator (This Session): DeepSeek V4.1 Flash", but this session ran as **Claude Opus 5.5**. To keep custody clean, it acted **only as the Spec Author**: read-only Step 0 checks, authoring the five files in an isolated docs worktree, and one docs commit and push. It did not do any coordination writes (no `active-tasks.md`, issues, locks or admissions). The coordinator's actual model and thread are not observable from here and stay `UNKNOWN`.

## 2. Git

- **Repository:** https://github.com/jermur788/CCF
- **`origin/main` at Step 0:** `cc2757a730ad9451e3d6e3680dffd123171a5df5` (unchanged from the dispatch's observed value)
- **Base:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (`feat/multi-2d-phase1-time-history-core-20261124-v1`)
- **Docs branch:** `docs/multi-2d-phase2a-forest-maker-spec-20261125-v1` (new; did not exist on origin at Step 0)
- **Docs worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A-Spec-20261125` (isolated)
- **Docs branch HEAD:** the single commit that adds this file. Its SHA, the `git diff --stat` and the `git status --short` are reported in the return message, because a file cannot contain its own commit SHA.
- **Files (only these five are added):**
  1. `specs/multi-2d-phase2a-forest-maker-v1/spec.md`
  2. `specs/multi-2d-phase2a-forest-maker-v1/plan.md`
  3. `specs/multi-2d-phase2a-forest-maker-v1/tasks.md`
  4. `specs/multi-2d-phase2a-forest-maker-v1/checklists/requirements.md` (all rows `[ ]`)
  5. `Docs/Handoffs/MultiRack-Simulation-20261116/phase2a-forest-maker/PHASE2A-SPEC-DRAFT-HANDOFF-20261125.md`

## 3. Read-only lock check

- **Live `Docs/Project/coordination/active-tasks.md` on `main`:** blob SHA `39d4ffd38da382cf540fcd77ecf3b61605061fd8`, last touched by commit `cc2757a7…`. Fetched **2026-11-25T10:58:36Z** (GitHub API, read-only). There is no Phase 2A row and none was added.
- **Phase 1 Step 2** (`feat/multi-2d-phase1-time-history-ui-20261125-v1` @ `513355c4bcef881413d517aa62feed4676234319`, worktree `CCF-Multi-2D-Phase1-TimeHistoryUI-20261125`): **untouched**. The branch was only read through `git show <ref>:<path>` from the main repo's object store, with no checkout and no worktree entry.
- **Unity lock:** not touched. No Unity process was used.
- **Protected listeners `:33767`, `:43649`, `:46593`:** not bound, probed or stopped. No server was launched.

## 4. Sources inspected (at `5fe2fabe` unless noted)

`MultiAreaGenerator.cs`, `MultiAreaExperiment.cs`, `MultiAreaGeometry.cs`, `MultiAreaContinuation.cs` (genesis, `SaveExpectation`, `Restore`), `MultiAreaEcology.cs` (`MultiAreaDemoExperiments`), `Phase2Models.cs` (species constants), `Phase2Rules.cs` (maturity, mast), `Stage5Models.cs` / `Stage5SeedRain.cs`, `CanopyLightEvaluator.cs` (world-anchored lattice, strict-inside cells), `Phase1SaveFormat.cs`, `Phase1SaveEnvelope.cs`, `Phase1SaveCodec.cs`, `Phase1SaveState.cs`, `Phase1SaveSlots.cs` (API surface), `Program.cs`, `PlayerApi.cs`, `PlayerMultiApi.cs` (base and Step 2 branch, read only). On `main`, read only: `Assets/ForestPrototype/ForestStartingStand.cs` (`GenerateEnlarged80`, start parameters only) and `StandGeometryModel.cs` (`StartingStemsPerHectare = 2100`). The anchor list comes from the Phase 1 Step 1 implementation handoff §5.

### Source findings that shaped the spec

1. **The payload gap is only on the Step 2 branch.** `MaxViewPayloadBytes = 524_288` and the final fallback (≈ L508–529) exist on `feat/multi-2d-phase1-time-history-ui-20261125-v1`, not on `5fe2fabe`. Step 2A-2 therefore has to start from the accepted Step 2 HEAD.
2. **Lane boundaries vs the light lattice.** Cells are world-anchored and assigned by strict centre-inside. For 80 m / 5 lanes, no centre sits on a boundary, so 1,024 cells split 192/224/192/224/192. Some custom widths (e.g. 30 m / 8 lanes → a boundary at 3.75 m) would drop a cell column from both lanes, so the spec adds a typed refusal for them.
3. **Tree-ID width.** MA-1 uses `D3`. Above 999 stems per lane, ordinal order breaks `TreeIdOrder`, so the forest maker uses `D4`.
4. **No age→size model and no broadleaf genesis-size model in Core.** MA-1-MIXED re-ages Sitka-sized stems to 60. The spec transfers the `Enlarged80` age-20 values and flags everything else (Q1/Q2).
5. **The badge and generator note are hard-coded for MA-1.** Additive null-preserving overrides (`GeneratorNoteOverride` in 2A-1, `BadgeOverride` in 2A-2) keep the MA-1 bytes unchanged.
6. **The session is fixed per process** (`PlayerApi` constructor fields). 2A-2 adds one small `MultiSessionHost` swap point.

## 5. Plain-English summary

- **Step 2A-1 (Core only):** a recipe (size, lanes, age, density, seed, species mix) goes into a deterministic generator. It makes an ordinary multi-area experiment that the unchanged Phase 2 engine runs, with exact typed refusals and a 3,000-stem cap. Two fixed 0.64 ha five-lane presets: Sitka with 8 edge broadleaves, and pure Sitka. Both have exactly 1,344 stems and 1,024 cells, and their manifest hashes get frozen as anchors 10–11. Tests prove cross-lane competition and seeding. There are no UI or save changes.
- **Step 2A-2:** a New woodland button and modal with Core-computed preview and a discard confirmation. Atomic host session replacement with new session IDs. Save Schema v3 stores the recipe and hash so a generated stand rebuilds and hash-verifies on load, while MA-1 / MA-1-MIXED keep writing byte-identical v2. A 2–3 lane neighbour view with a mini-map and 1-lane toggle. Measure first, then enforce a scaled payload budget with a typed refusal instead of a silent overflow.

## 6. Open questions for owner approval (one reply can approve all defaults)

| # | Question | Recommended default |
|---|---|---|
| **Q1** | Starting Sitka size for custom ages ≠ 20 (no Core model exists) | A species-neutral piecewise table, mean DBH `10→70, 20→160, 30→240, 40→300, 47→345, 60→400 mm`. The 20 and 47 anchors are TRANSFER; the others are **GAMEPLAY CALIBRATION — RESEARCH REQUIRED**. Height `max(3.5 m, 2 m + 0.62·DBH cm)`. Density does not affect start size (disclosed). |
| **Q2** | Starting DBH/height of age-60 edge broadleaves | Reuse the MA-1 Area B older cohort exactly (mean 345 mm, spread 80, clamp 220–480 mm, height 27.2 m + 20 mm per mm DBH): the MA-1-MIXED precedent, tagged RESEARCH REQUIRED. Crowns and mast come from existing Phase 2 constants. |
| **Q3** | Lane widths when `W_mm` is not divisible by `N` | Boundaries at `floor(i·W_mm/N)`, so lanes are equal to within 1 mm, with exact per-lane areas reported. Refuse boundaries on the light-cell centre lines (`ForestMakerLaneGridAlignment`). |
| **Q4** | Loading a **v2** (MA-1/MA-1-MIXED) save while a forest-maker stand is running | Keep the exact v2 semantics (`SaveGenesisMismatch`). The Saved games dialog explains "start MA-1 / MA-1-MIXED from New woodland first". Revisit after 2A-2 playtest. |
| **Q5** | 5-lane payload ceiling and speed targets | Scaled budget `min(2.5 MiB, 512 KiB × ceil(genesisStems/300))` (about 2.5 MiB at 1,344 stems), finalised **downward only** after M0. Targets at Year 15: snapshot ≤ 250 ms, parse ≤ 150 ms, 3-lane render ≤ 500 ms, annual advance ≤ 2 s. |
| **Q6** | Edge-broadleaf exclusion radius and lane edge margin | `2.0 m` exclusion, `0.5 m` lane edge margin, `1.25 m` broadleaf inset (as dispatched). |
| **Q7** | Implementation base for 2A-1 | Both steps start from the **accepted Phase 1 Step 2 HEAD** (one lineage). 2A-1 is low-conflict (new Core files only), so the owner may instead admit it earlier on `5fe2fabe` and rebase later. |
| **Q8** | Preset seeds and IDs | `6,400,101` / `mad-fm-064-sitka-edge-broadleaves-v1` and `6,400,202` / `mad-fm-064-pure-sitka-v1`. |

## 7. Next authorised step

The owner reviews the quartet and approves or amends Q1–Q8 → the Manager records approval and admits the 2A-1 lane under D-091 → implementation starts. This handoff authorises none of these steps.
