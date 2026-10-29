# PARITY-07 — Player-facing species truth

**Exit token:** `PLAYER-FACING SPECIES PARITY QUALIFIED` (**static-qualified only; NOT UNITY-VERIFIED; no rendered review done**)

## Identity
- Task PARITY-07, umbrella #91, claim #102 (left open).
- Actual executor: **Anthropic `claude-sonnet-5-5` (Sonnet 5.5), Claude Code harness, in T3 Code.** Sole owner, no subagents.
- Worktree `/media/jer/Videos/CCF-Parity07`, branch `feature/parity-07-player-facing-species-truth-v1`, base `23fe4b8`.
- **Code + verification candidate: `1e7b9413da770fcf9ee83b46024c82034deb2fd3`.** This handoff is a later docs-only commit; its tip is on claim #102.

## Files and wording mapping (spec FR-9 / AC-5)
| File | Change | FR-9 bullet |
|---|---|---|
| `UI/ScenarioOneUiFacts.cs` | Three shared constants: `BroadleafQualityLimit`, `PremiumOakNote`, `SharedSpeciesRulesNote` | all |
| `UI/TreeInspectionView.cs` (timber estimate) | "size and form" → quality-limit constant (size + pruning history; fork/defect not modelled). Oak pruned line: "large and straight enough" → "can become eligible … once they are large enough". Oak premium line: "needs a large, straight log" → size thresholds + premium-Oak note | quality, no form claim, premium Oak |
| `UI/TreeInspectionView.cs` (More detail) | Non-Sitka muted line: site/canopy/wind use the same simplified rules for all species; "Storm events are off in this forest." when `StormModel.None`, else "simplified shared model, not a species wind-tolerance band" | site/canopy, wind |
| `UI/WorkPlanView.cs` harvest card | When broadleaf market active: quality-limit + premium-Oak note (worded "If this visit includes Oak or Beech") | quality, premium Oak |
| `UI/WorkPlanView.cs` pruning card | Removed "applies no timber-price premium for pruning" (untrue for Oak eligibility); now: sets no price by itself, Oak pruned section can make a large log eligible, Beech has no such class, fork/defect not modelled | quality |
| `UI/LearningObjectivesView.Curriculum.cs` | Pruning topic same correction; `prune.read` states Sitka 2.5/5/6.5 m ×3 lifts ≥5 yr and Oak/Beech 3.5 then 6.0 m ×2 lifts ≥3 yr (matches `ForestTree.CanPrune`) | quality |

Review N2 (Oak "large and straight enough" / "large, straight log") is fixed. No ecology, economy, price, save, scene, prefab, package, Rack/MFM/S1-C/S1-D file touched. No new Assets file (no `.meta`).

## Offline results
Evidence: `/media/jer/Files/CCF/Verification/Parity07-Offline-20261029/`.
- `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_player_facing_truth.py --repo .` → **`PARITY07_COPY PASS checks=77 passed=77 failed=0 NOT_UNITY_VERIFIED`**. Covers: forbidden wording absent in every `UI/*.cs` (straight enough, large/straight, size and form, form-quality claims, guaranteed premium Oak, stale pruning-premium and stale all-species lift schedule); each FR-9 truth present and spec bullet quoted; wiring of constants into inspection/Work Plan/teaching; broadleaf pruning teaching vs `ForestTree.CanPrune`; Sitka schedule unchanged; 5 negative controls. (An initial over-broad "guarantee" rule failed 3 checks on pre-existing honest negations like "success is not guaranteed"; narrowed to premium/veneer/Oak promises.)
- Existing `run_offline_check.py --suite timber` → `THREE_SPECIES_TIMBER_PASS checks=73` (unchanged from PARITY-06; no existing check weakened).
- `compile_runtime_sources.py` type-check: **could not run** (tool mangles `<src>` paths in its csc invocation; errors are about its own file names, not my code). C# edits are string/const changes plus one `StormModel.None` comparison; **not compiled**.

## Rendered residual (for the later acceptance gate, at 1280x720 and 1920x1080)
Check no clipping/overflow of the longer lines:
1. Inspect an **unpruned Oak** (felled-estimate card): quality limit + not-pruned line + threshold/premium note.
2. Inspect a **pruned Oak** below and above size thresholds (premium note shown only when no veneer/beam product).
3. Inspect a **Beech** (quality limit + "no veneer, beam or fencing class").
4. **More detail [V]** on Oak/Beech with storms off (old save) and storms on, and a Sitka (no shared-rules line).
5. **Work Plan** harvest card with broadleaf market active; **Pruning** card.
6. **Learning objectives → 6. Pruning** topic and `prune.read` step.

## Preserved anchors
No ecology, density, timber value, price, save/schema or world code changed; PARITY-05/06 anchors untouched by construction. Unity lifecycle/fingerprint gates **not rerun**.

## Residual / blockers
Not Unity-compiled or rendered; no independent review; G2 acceptance pending. Qualified as static copy only. Exit-token scope is limited accordingly. No blocker for review.
