# Sen-1 #836 — Compact30 path, planting-edge seam and reusable deer fence: implementation and Unity qualification result

**Status:** IMPLEMENTED and NATIVE-QUALIFIED on one candidate branch. **Not integrated, not merged, not owner-accepted.**
**Date:** 19 November 2026.
**Task:** [#836](https://github.com/jermur788/CCF/issues/836). **Design input:** [#835](https://github.com/jermur788/CCF/issues/835) research + Manager [#824 verdict](https://github.com/jermur788/CCF/blob/docs/sen-1-824-835-path-fence-design-verdict-20261119-v1/Docs/Handoffs/ScenarioOne-Visual-Research-20261119/Sen-1-824-MANAGER-835-PATH-AND-DEER-FENCE-DESIGN-VERDICT-TO-836-20261119.md).

## 1. Actor, authority and lane

| Item | Value |
|---|---|
| Actor | OpenCode / `opencode-go` / `deepseek-v4.1-flash`, single parent |
| Skill | installed `orchestrate-work` (`/home/jer/.agents/skills/orchestrate-work`, sha256 `07831c7c…`), invoked and followed; portable routing ledger because the host exposes no Runtime ledger |
| Branch | `sen-1/836-path-seam-deer-fence-20261119-v1` |
| Base | `0709d05a79756d3b72ccbd0452bd0252a75c3804` (accepted #813 E1 scene) |
| Candidate | `8765db07` (change) + `bccffe650` (README meta) + `6299e711` (opening alignment) |
| Worktree | `/media/jer/ZX20/CCF-Sen1-836-PathFence-20261119`, own cold `Library`, registered lane `Sen1-836-PathFence` |
| Unity lease | `8a5eb9ee2b284164a5239da618d080de`, acquired through `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`, one serial Editor operator |
| Writer exclusivity | single writer for the scene and the new art folder; #813's own worktree and its three uncommitted files untouched |

Owner grant (19 Nov 2026, in-session): named writer, this branch/worktree at `0709d05a`, reuse of the #786 H1 kit as owned input, a ZX20 Unity lease, the protected-set exclusion, and the **1.2 m core** ruling.

Ops change made and disclosed: one lane entry added to the live `/media/jer/ZX20/CCF-Ops/lanes.json` (backup `lanes.json.bak-sen836-<timestamp>`) so the lock tool would accept the lane. No other Ops edit.

## 2. What the owner complained about, and what was actually wrong

Diagnosed read-only at G0 from the base scene, not guessed from the screenshot:

| Complaint | Actual cause in `0709d05a` |
|---|---|
| Poor-quality path | The path was **176 separate built-in cube slabs** (`C0_001…C0_176`), each 1.6 m × 0.02 m, independently rotated, casting shadows, all sharing one material whose base map restarted its 3×3 tiling on every segment. Sawtooth edge, hard slab silhouettes, visible repetition, no continuous UV. |
| Square seam / gap at the planting area | `H1_WornEarth_12x10` was a **single axis-aligned 12 m × 10 m cube slab** at the same height band as the path. The "square" was literally its boundary; the "gap" was bare ground between that rectangle and the rotated path segments. |
| Nearly invisible fence | `Fence_Post` = 0.055 m × **0.55 m** cubes and `Fence_Wire` = **0.01 m** × 1.25 m cubes, ~2.5 m apart, **only on the east and north edges**. No west fence at all. |

## 3. What changed

| Area | Change |
|---|---|
| Footpath | One continuous mesh `C0_FootpathRibbon`: **1.2 m core + 0.20 m verge each side**. Catmull–Rom through the nine accepted C0 anchors, 26 samples/segment, 208 segments, 1,045 vertices, 1,664 triangles, 3 submeshes (core, left verge, right verge). UV `u` = arc length / 2 m so the texture flows along the route instead of restarting; the outer verge boundary wobbles ±75 mm so there is no straight edge; both ends taper to zero width over 1.4 m so there is no cap line. No collider; shadow casting off, receiving on. |
| H1 clearing | `H1_WornEarth_12x10` replaced by an organic 12 × 10 m fan mesh (513 vertices, 896 triangles) with four octaves of boundary wobble **and a directional lobe that reaches the C0 mouth at (2.99, −4.75)**. The path therefore overlaps the clearing, removing the bare wedge. No collider. |
| Deer fence | First-party modular kit, 1.9 m above ground: `PF_Sen1_DeerFencePost` (2.10 m, ⌀90 mm octagon), `PF_Sen1_DeerFenceStrainer` (2.30 m, ⌀130 mm), `PF_Sen1_DeerFencePanel_3m` (3.0 × 1.9 m netting built as thin boxes: 150 mm grid, two heavier tension wires, 420 triangles), plus eight corner braces. Placed on **all four edges at ±15.75 m**, posts every 3.0 m, strainers at corners and at both sides of the **southern H1 opening (x −3.75 … −0.75, one full panel bay)**. No gate, because the current world has no authorised gated access point. No colliders. |
| Art | 4 original procedural 512² textures (path soil, verge litter, clearing earth, timber), 5 URP Lit materials, 5 mesh assets, 3 prefabs, 1 README. **1.07 MiB** of new assets; no third-party content. |

**Nothing else moved.** `git diff 0709d05a..HEAD` touches exactly two paths: `ScenarioOneCompact30.unity` and `Assets/ForestPrototype/Art/Sen1PathFence/**` (41 files). The scene diff is +6,627 / −20,263 lines.

## 4. Native verification (single Editor, batchmode, GPU)

| Check | Result |
|---|---|
| Import + compile after the authoring tool was removed | **PASS**, 0 `error CS` (only pre-existing CS0618 warnings in unrelated files) |
| Structural verify (in-Editor) | `sitka=175 shell_children=5 textmesh=0 path_ribbon=True clearing=True legacy_slabs=0 g1_o1=True g1_b2=True`; after the opening correction: **38 posts, 39 panels, 6 strainers, 8 braces**, no panel inside the opening |
| Prefab-instance roster vs base | 433 base instances → 517; **the only changed sources are the three new fence prefabs** (+39 panels, +39 posts, +6 strainers). Every pre-existing prefab instance count is unchanged. |
| Colliders | 5 before, 5 after — identical set. No collider was added to the path, clearing or fence. |
| G1 clearance, 1.2 m core | O1 +1.574 m, O2 +1.567 m, B1 +3.292 m, B2 +2.796 m to the core edge; subtracting the 0.55 m footprint ring still leaves **+0.82 m worst case**, i.e. better than #813's +0.767 m because the core narrowed from 1.6 m to 1.2 m. |
| Renders | 56 PNGs: 7 viewpoints × 2 resolutions × before/after, same camera poses, plus a southern-entrance pair after the opening fix. |
| Runtime performance | **NOT TESTED** (no frame-time measurement). Proxy: ≈19 k added triangles and 92 added renderers in a 30 × 30 m scene; the netting panels are 420 triangles each and do not cast shadows. |

### Render evidence

Root: `/media/jer/Files/CCF/Verification/Sen1-836-PathFence-20261119/captures/{before,after}/`

- `01_path_mouth` — walking-player view along the C0 approach.
- `02_h1_seam` — the exact owner-observed join, eye height.
- `07_h1_junction_close` — top-down close-up of the join.
- `03_fence_corner`, `04_fence_east` — perimeter readability and construction.
- `08_south_open_entrance` — the H1 arrival opening: two strainers, one clear bay, no netting across it.
- `05_overhead`, `06_path_north` — plan view and the northern half of the route.

The before/after pairs are the actual argument, not this document: the before `07` shows the rectangular slab edge with a bare strip; the after `07` shows the clearing lobe meeting the tapered path with no gap. The before `03` shows no fence at the corner at all; the after `03` shows posts, netting, tension wire and brace.

## 5. Protected content

- 175 simulated Sitka instances: unchanged.
- 226 exterior cosmetic instances + 32 brash under `E1_Outside_VisualOnly`: unchanged.
- Four G1 markers (O1/O2 cell 15, B1/B2 cell 14), three Crop Trees, playable bounds ±15 m, four `Containment` colliders, Main Camera, Player, Sun, lighting values: unchanged.
- No save, world manifest, profile, Work Plan, economy, regeneration or deer-browse logic was touched. The fence does **not** create protection gameplay; `BrowseProtectionGeometry` and `BrowsingConditions` are untouched.
- Reference Future v1, Legacy40 and Enlarged80 content: not present in this branch and not modified.

## 6. Independent review

A fresh-context, non-author reviewer was given the candidate, the scene diff and both evidence folders, with instructions to check the roster diff, collider set, dimensions, URP shader use and whether A/B/C are genuinely fixed.

### Bounded correction found during this run

The first placement left a 3 m netting panel overlapping the southern opening and planted its strainers inside that panel. This was found by a structural check of the opening, not by the reviewer, and fixed in `6299e711`: the opening now occupies exactly one panel bay (x −3.75 … −0.75), with strainers replacing the ordinary posts at both edges. The correction changes only fence instance positions; the path, clearing and materials are untouched.

**Reviewer verdict: PENDING — recorded on #836 when it returns.**

## 7. Honest limits

- The 3.0 m post rhythm is a deliberate game-art choice for readability, **not** an Irish forestry standard, and it is denser than the 6–8 m candidate suggested in the design note.
- The netting does not cast shadows (deliberate: woven mesh is mostly transparent, and the first build showed the shadow caster produced an unusable full-scene shadow).
- Terrain in Compact30 is flat. Slope handling is documented in the kit README as a placement rule, not demonstrated in-scene.
- Materials are tuned against this scene's lighting. The clearing still reads pale next to the surrounding needle litter in full sun; that is a colour-calibration judgement the owner should make on the actual walk.
- No GUI Play-mode walk was performed in this run: qualification is batchmode render plus structural checks. Owner acceptance still requires the owner's own walk.
- Dashboard/Drive: **NOT VERIFIED**. GitHub is canonical.

## 8. Next authorised action

Owner walk of this branch in the Editor:

```bash
cd /media/jer/ZX20/CCF-Sen1-836-PathFence-20261119
git pull --ff-only origin sen-1/836-path-seam-deer-fence-20261119-v1
XDG_DATA_HOME="$HOME/.var/app/com.unity.UnityHub/data" \
XDG_CONFIG_HOME="$HOME/.var/app/com.unity.UnityHub/config" \
/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -projectPath "$PWD"
```

Open `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`, confirm `git log -1 --oneline` reads `6299e711`, press Play, and judge the three original complaints. Nothing is merged into any integration branch and no Windows build was produced.
