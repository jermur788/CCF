# CCF #894: scanned spruce root-flare graft, one-tree prototype result

**Verdict: AUTHOR PROTOTYPE / READY FOR OWNER VISUAL TEST / NOT INDEPENDENTLY REVIEWED / NOT INTEGRATED / NOT ACCEPTED**

- **Date:** 2026-11-21 ~00:45 UTC.
- **Author:** Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, jer-laptop. This session is not the #891 actor. It also wrote the Phase 0 result `CCF-894-PHASE0-INSPECTION-RESULT-20261120.md` (d6ebbe01).
- **Authority:** #894 packet `8e9c988c` plus owner in-session instructions:
  - picked the two best of the five downloaded scans;
  - "unity is free and the licence is not an issue, continue".
- **Independence:** I built this, so I must not review it.

## Candidate

| | |
|---|---|
| Branch | `art/ccf-894-root-flare-prototype-20261120-v1`. **Local only, not pushed**, held in the shared bare store. |
| HEAD | `f1ea9574a8bc6cec58e4949b77f1ce7c5b183217` (parent `fce41bd3`, the #876 reference) |
| Worktree / lane | `/media/jer/ZX20/CCF-894-RootFlare-20261120` / `CCF894-RootFlare`. It has an imported Library. |
| Board | `CCF-894-ROOT-FLARE` set to **VERIFY** |
| Unity lock | Acquired `e6f87092…` 2026-11-20 23:54Z. **Released** after the runs; status is free. |
| Evidence | `/media/jer/Files/CCF/Verification/CCF894-RootFlare-20261120/`, containing `build/`, `run1–3/`, `unity-drift-reverted.patch` and `lock.txt` |

## Assets used

The owner-downloaded Fab/Quixel **Mid** sets (FBX with 2K maps):

- **`thksce3da`**: `spruce_tree_trunk_thksce3da_mid(1).zip`, sha256 `dc336238…`, 3,857 tris. **Option B, the scene default.**
- **`ugjoddlfa`**: `spruce_tree_trunk_ugjoddlfa_mid.zip`, sha256 `60db6c97…`, 3,188 tris. **Option A.**

I chose these from the five Raw scans by trunk diameter at 1.3 m, straightness and scan height. The Raw measurements were:

| Scan | Diameter at 1.3 m | Scan height | Notes |
|---|---:|---:|---|
| `ugjoddlfa` | 0.44 m | 1.96 m | Straight |
| `thksce3da` | 0.52 m | 2.94 m | ~5° lean |
| `ugghcjqfa` | 0.53 m | 1.83 m | Leans |
| `ugggcjmla` | 0.36 m | 1.43 m | Too short |
| Nordic `ugfmbftfa` | 0.26 m | — | Stem on a 1.3 m flare | The bytes stay **local and git-ignored** (`Experiments/CCF894/Scans/.gitignore`). The owner says the licence is not an issue; D-032 *storage* (committing commercial binaries) is still a separate open decision, so nothing licensed is in Git.

## Composition

```
P0510_Sitka_Mature_Benchmark_01   (prefab instance, ForestTree added at runtime by the bootstrap; unchanged)
├─ LOD0..2 Trunk     -> trunk mesh swapped for a trimmed copy (triangles below the seam removed)
├─ LOD0..2 Branches  (unchanged)
├─ LOD0..2 Needles   (unchanged)
└─ CCF894_RootFlare_thksce3da   (added child at root origin, no Collider)
   └─ Fit (uniform scale + horizontal offset) └─ ScanMesh  (in all 3 LODs)
```

- **The prefab is normalised.** Compact30 tree prefabs are normalised to 1 m and scaled by height, so the in-game Benchmark trunk is only **~0.22 m** across at P0510 (12.14 m). That is about **0.29 m** at 15.9 m.
- **The fit is uniform.** The builder slices both meshes into horizontal cross-sections and solves one uniform scale. At that scale, the scan's *inscribed* top radius encloses the trunk with a 4 % margin and 0.10 m of overlap. There is no stretching or rotation.
- **One fit serves every height.** The fit is solved in root-local units, so the same values work for every Benchmark height.
- **Seam and top heights:**

| | Seam / top, 12.14 m tree | 15.9 m tree | Footprint (12.14 m) |
|---|---|---|---|
| A `ugjoddlfa` | 1.08 / 1.14 m | 1.42 / 1.49 m | 1.06 × 0.90 m |
| **B `thksce3da`** | **1.51 / 1.57 m** | **1.98 / 2.05 m** | 0.89 × 0.75 m |

- **The colour tint is partial.** The scan colour is pulled 60 % toward the Benchmark bark mean at the seam. Option A needed a large tint (×1.27 to ×1.36) because its scan is darker and greener. Option B needed almost none (×0.98 to ×1.02).

## Changed files (all new)

- `Assets/ForestPrototype/Art/Experiments.meta`. This uses #891's GUID, so it is byte-identical and the branches won't conflict.
- `Assets/ForestPrototype/Art/Experiments/CCF894/**`:
  - the Editor builder;
  - 2 URP Lit materials;
  - 6 trimmed trunk meshes;
  - 2 flare prefabs;
  - the disposable **same-name scene copy** `Scenes/ScenarioOneCompact30.unity`;
  - the folder metas;
  - `Scans/.gitignore`.
- `Tools/Verification/CCF894RootFlare/`: the smoke harness, `README.md` (fit method, rebuild and rollback) and `prepare_scans.py`.

**Not touched:**
- the canonical `Scenes/ScenarioOneCompact30.unity`;
- the Benchmark prefab and FBX;
- all C#;
- the roster, tree positions, height and DBH;
- colliders, felling, stumps and saves;
- `Packages`, `ProjectSettings`;
- the #886 and #891 lanes.

The scene copy differs from the canonical scene in **120 YAML lines, all on P0510**: three `m_Mesh` overrides, the LOD renderer lists, and one added child.

Unity also auto-modified some files: the `Packages/` sdk bump, two SectionFive Forb materials and `.vscode/settings.json`. I saved those changes to `unity-drift-reverted.patch` and **reverted** them; they are not committed.

## Checks run

**Unity 6000.6.0f1**, batch mode in the Hub sandbox, isolated `XDG_CONFIG_HOME`.

- **Builder:** `CCF894_BUILD_PASS`.
- **Smoke:** run3 `CCF894_ROOTFLARE_SMOKE_PASS checks=13 failures=0` (verdict sha256 `d67f2801…`). It checks:
  - the bootstrap did not refuse the scene copy;
  - P0510 is still the bound `ForestTree` `P0510`;
  - its collider set is identical to reference tree P0907: one `CapsuleCollider`, r = 0.25 local;
  - the flare has **no Collider**;
  - the flare renderer is inside the tree's renderer hierarchy, so `Compact30Session` hides it on fell (`GetComponentsInChildren<Renderer>`, Compact30Session.cs:485);
  - the flare is in all 3 LODs;
  - the trimmed trunk is in use on every LOD;
  - no project errors were logged. The only error was Unity's own QuickSearch index exception, also seen in #891.
- **Captures:** paired identical-camera PNGs for before / A / B. The comparison sheets are `run3/sheet-*.png`. The views are:
  - `walk-eye-3m` and `close-low-1.6m` (the main views);
  - `seam-side-1.3m`;
  - `spawn-eye`;
  - `far-11m`. That camera missed P0510, so this view is inconclusive.
- **Not run:**
  - a play-mode fell of P0510 (no hierarchy change puts felling at risk);
  - the player walking past P0510. No collider changed, but the existing 0.25 local radius (about 3 m at this scale) is #886's issue;
  - LOD transitions in motion;
  - performance;
  - a GUI walk.

## Author visual read

These are my opinions; the owner decides.

- **Both options clearly improve the ground entry over the Benchmark trunk.** The trunk now has real buttress roots, soil and moss, instead of a straight cylinder with small spikes.
- **Option B (`thksce3da`) is better.** The bark colour matches, and the seam at ~1.5 m is hard to see from the side.
- **Option A (`ugjoddlfa`) shows a visible grey-green band at the ~1.1 m seam.** It also has a darker soil skirt edge on the ground.

## Limitations

- **The scan is a veteran tree.** The Benchmark trunk is the 26 m / 48 cm model scaled down, so the scan shrinks to about 0.53× (B) at P0510. The root architecture is from an old Swedish spruce (`thksce3da`) applied to a ~12–16 m Sitka. This is a **visual calibration, not an ecological claim**. Root form for Sitka at this age and DBH is UNVERIFIED.
- **Only Benchmark_01 trunks are fitted.** Mature_01, Pole and Young use different meshes and would need their own fit. The proposed stand-wide approach is to replace Mature_01's visuals with Benchmark (see Phase 0).
- **The flare scales with each tree's height**, so larger trees get larger flares and higher seams (up to ~2 m on B).
- **The scan mesh is one 3.9 k-tri mesh used in all LODs.** That's acceptable for 37 roster Benchmark trees (plus 55 outside), but it is unprofiled.
- **Option A in the scene would need a rebuild.** It isn't toggleable in the Editor; set `SceneOption` and rebuild, or use the harness captures.

## Owner launch instruction

Open `/media/jer/ZX20/CCF-894-RootFlare-20261120` in Unity Hub. Then open **`Assets/ForestPrototype/Art/Experiments/CCF894/Scenes/ScenarioOneCompact30.unity`**. Take care to use the copy under `Experiments/CCF894`, not `Assets/ForestPrototype/Scenes/`. Press Play. P0510 is the tree ~4 m ahead and slightly to the right of the spawn. Walk around its base.

To compare, open the canonical scene and look at the same tree. Close the Editor afterwards so the lease stays free.

## Next (owner)

1. Visual verdict: B, A, or neither.
2. If yes, the stand-wide rollout is **SPEC REQUIRED**. It needs:
   - per-variant fits;
   - the Mature_01 → Benchmark swap decision;
   - D-032 storage for the scan binaries;
   - #886 collision reconciliation;
   - an approved four-part spec.
