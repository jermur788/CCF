# Sen-1 #876 — F-1 Crop/Fell marker scale repair: result

**Date:** 2026-11-20 · **Task:** [#876](https://github.com/jermur788/CCF/issues/876) (follows #873; Manager #872 grant `7d890e6f`)
**Actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, native `jer-laptop` UID 1000.
**Status:** **IMPLEMENTED + TARGETED UNITY VERIFIED (batch + render captures). Manual GUI walk NOT RUN. Not independently reviewed, not merged, not owner-accepted.**

## Authority and registration

- **Self-registration:** the owner gave direct approval in this session (2026-11-20) for this actor to self-register the #876 lane/row. This overrides the packet's #876-only "no self-registration" default. [RUNNING comment 7046639508](https://github.com/jermur788/CCF/issues/876#issuecomment-7046639508).
- **Registration:** done under `flock -x -n /media/jer/ZX20/CCF-Ops/board`, hash-guarded.
  - Added lane `Sen1-876-MarkerScale` → `/media/jer/ZX20/CCF-Sen1-876-MarkerScale-20261120` (unity: true) and row `SEN1-876-MARKER-SCALE`.
  - Board `92ef4182…` → `a7af9136…`; lanes `c1e35b2b…` → `bd1ee92a…`.
  - +1 row, +1 lane, 0 other lines changed, 0 conflicts.
- **Branch:** `fix/sen1-876-mark-visual-scale-20261120-v1`, new worktree from exact `31fbe46ec5773ab65fee4f2fda68119d2bdc107b`. The #873 lane was not reused.
- **Unity leases:** every lease was acquired and released normally by `Claude-876-e6dc7356`, none forced. There was one lease per HEAD:
  - `e89f0ea3…` on `23e9ae35`;
  - `bf703700…` on `21ee4e95`;
  - `ab7c7527…` on `7001da9c`.
- **Final state:** unity-lock FREE, no Editor PID, lane clean.

## Source change (granted file only)

Commit `21ee4e95f055ed4c612cce4cda3f271fbdaad62d`, `Assets/ForestPrototype/ForestTreeMarkingManager.cs`, +15 lines:

- **The change:** `CreateMarker` now calls `CancelParentScale(marker.transform)` right after `SetParent(tree.transform, false)`. This sets the mark root's local scale to the inverse of the tree root's `lossyScale`, with a guard against zero.
- **What stays the same:** the mark stays a child of its tree, so it still follows it and is destroyed with it. All child sizes and offsets are unchanged and are now real world metres.
- **Unit-scaled roots:** they get exactly `(1,1,1)`, which is the pre-fix transform.
- **Untouched:** mark state, `ForestTree.SetMark`, save, UI, scene, prefabs, rack-entrance markers (`RackEntrances.cs`) and Packages.

**Root cause (measured):** Compact30 Sitka roots are 1 m normalised prefabs scaled to their height (root scale = `Height`, 9.1–15.9 here), so every mark child was multiplied by that factor.

Verifier (new, `Tools/Verification/Sen1MarkerScale/`, commits `23e9ae35`, `7001da9c`): disposable play-mode gate; see its README.
Final branch head: see the commit that adds this file.

## Results

| Run (isolated `XDG_CONFIG_HOME`, batch, real GPU) | Source | Verdict |
|---|---|---|
| baseline-compact | pre-fix (`23e9ae35`) | **FAIL 37/110**: F-1 reproduced for C **and X** |
| fixed-compact | `21ee4e95` | **PASS 110/110** |
| fixed-legacy40 (`CCF_STAND_GEOMETRY=0`, ForestTest new game) | `21ee4e95` | **PASS 24/24** |
| fixed-enlarged80 (`CCF_STAND_GEOMETRY=1`) | `21ee4e95` | **PASS 24/24** |
| capture-after / capture-before | `7001da9c` / pre-fix blob in working tree | PASS 110 / FAIL 37 (expected) |

Measured on the actual Compact Sitka (metres, relative to the tree base):

| Case | Root scale | Disc width before → after | Diamond height before → after (expected H+0.6) | Diamond − crown top after |
|---|---|---|---|---|
| Crop P0000 | 15.90 | 19.08 → **1.20** | 262.35 → **16.50** | +0.60 |
| Crop P0001 | 14.06 | 16.87 → **1.20** | 206.12 → **14.66** | +0.60 |
| Crop P0002 | 11.21 | 13.45 → **1.20** | 132.39 → **11.81** | +0.60 |
| Fell P0003 | 9.13 | 10.96 → **1.20** | 88.84 → **9.73** | +0.60 |
| Fell P0009 | 13.67 | 16.40 → **1.20** | 195.07 → **14.27** | +0.60 |

- **Blue trunk band:** was at 22–25 m, now at 1.60 m, with its width taken from DBH (0.25–0.30 m).
- **Every mark:** world scale is 1.000 and there are 0 colliders.
- **Other cases (all pass):**
  - Crop Tree count is 3.
  - Three unmark/re-mark rounds each leave exactly one visual.
  - A mark follows its tree when the tree is moved 2 m.
  - The play-mode-only unit-root and non-uniform root (6×14×9) cases each give a 1.2 m disc and world scale 1.
  - Unmarking destroys the visual, and no mark visual is left at the end.
- **Historical worlds:** Legacy40 has model 0 and Enlarged80 has model 1. Every tree root is unit-scaled. The mark local scale is exactly `(1,1,1)`, as before, and the diamond sits 0.60 m above the crown.

Captures (`capture-before/` vs `capture-after/`, 1280×720): before, the floor is flooded blue and orange. After, there are normal-size discs inside the fence and small diamonds just above the crowns.

## Protected files (D-1 handling)

- **Witnesses:** all 86 files under `~/.config/unity3d` and `~/.var/app/com.unity.UnityHub/config/unity3d` were hashed before and after.
- **Unchanged:** shared Hub CCF prefs stayed at `f64398ca…`, which is the D-1 state left as is, not restored. All `forest-save.json` files and the user CCF prefs are also unchanged.
- **Changed:** only three files owned by the Hub's running licensing client: `Unity.Licensing.Client.log`, `Unity.Entitlements.Audit.log`, and the entitlement licence refresh. No game or prefs file changed.
- **Where game data went:** every Editor process used task-local `XDG_CONFIG_HOME` from its first launch. Isolated licence copies were deleted from evidence.

## Transient drift, restored

Each Editor open bumped `com.unity.sdk.linux-*`/toolchain 1.1.0→1.1.1 and rewrote `.vscode/settings.json`. Five SectionFive `.mat` files were also re-serialised. The patches are preserved in evidence, all files were restored to HEAD by exact path, and nothing was committed. One `ArgumentOutOfRangeException` from Unity's QuickSearch indexer appears at startup, as it does in the #873 logs. It is unrelated, and there were 0 `error CS`.

## NOT RUN / open

- **Manual GUI walk: NOT RUN.** That covers real C/X key presses by a player, walking three Crop Trees, and a Year-2 juvenile view after the advance. The batch gate drives the same `Mark` API the keys call, and the captures show that the floor is visible again. Player-input feel and the post-advance view are left for the owner playtest.
- **Old save files: NOT RUN.** Legacy40/Enlarged80 old saves were not loaded; fresh new games in each geometry were tested instead. Reference Future and determinism hashes were **NOT RUN**. The change touches only mark visual transforms, which are not saved, but that was not verified against those harnesses.
- **Rack-entrance (orange) markers:** `CreateRackEntranceMarker` in `ForestTreeMarkingManager.RackEntrances.cs` uses the same parenting pattern. It is out of scope and not used in Compact, so I left it alone; it would need a separate grant if racks ever appear on scaled roots.
- **Review:** an independent non-author review is required. I co-authored #873, so neither Claude-873 nor Codex-873 may review this.
- **Still pending:** Manager #872 readiness judgement, owner acceptance and the Windows build, each a separate gate.

## Owner Editor test

1. **Open the project:** in Unity Hub, add `/media/jer/ZX20/CCF-Sen1-876-MarkerScale-20261120` and open it with 6000.6.0f1. The first open imports and bumps the toolchain packages; do not commit that.
2. **Start the scenario:** choose **CCF → Player Test → Play Compact30**, then click the Game view.
3. **Mark trees:** walk to Sitka and aim at a trunk.
   - **C:** you should see a 1.2 m blue disc at the foot, a blue band at chest height and a small blue diamond just above the crown.
   - **X:** you should see an orange disc and diamond.
4. **Expected:** the floor and juveniles stay visible. Mark three Crop Trees, advance the year as in #873, and confirm the juveniles remain visible.

Evidence: `/media/jer/Files/CCF/Verification/Sen1-876-MarkerScale-20261120/` (registration, leases, launcher, witnesses, per-run `editor.log`/`markers.csv`/`verdict.txt`, captures, drift patches).
