# Sen-1 #878 — Lean independent Opus source review of #876 (and #873 compatibility)

**Date:** 2026-11-20. **Issue:** [#878](https://github.com/jermur788/CCF/issues/878). **Packet:** amendment `0e5b7545` (`docs/sen1-878-opus-lean-review-amendment-20261120-v1`).
**Verdict (#876 source fix): APPROVED.** This is a source review only. It is **not** owner playtest acceptance, GUI verification, save/load verification, integration into main or a Windows build check.
**Unity: NOT RUN** by this reviewer. Nothing was edited, no lock was taken and nothing was merged.

## 1. Reviewer identity

- Model/provider: Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, through Claude Code in T3 Code.
- Session: `4cd086a5-1d2d-4bb5-911e-df076d484232` (a T3 child session). T3 thread id: UNKNOWN to this session.
- Non-authorship: this is a new session. It is not T3 thread `0e61f428-7d88-40b9-b0b2-332da1bcb312` (the #873 continuation) or the #876 repair actor, and it is not their child or helper. It made no commits to the target branch.
- **Independence disclosure:** the same Claude account has a shared auto-memory index, and that index holds one-line notes written by the author sessions (for example "#876 marker scale repair … I authored"). This session read the index line only. It did not read the #876 memory detail file or any implementation transcript. All findings below come from Git objects inspected in this session. The Manager or owner may decide whether that shared memory affects independence.
- Registered RUNNING: [#878 comment 7047626690](https://github.com/jermur788/CCF/issues/878#issuecomment-7047626690).

## 2. Target checked (objects resolved locally from `origin`)

| Ref | SHA |
|---|---|
| `fix/sen1-876-mark-visual-scale-20261120-v1` HEAD | `fce41bd3b77a054bf82d2521648d15ec18f81141` (verified by fetch) |
| Fix commit | `21ee4e95f055ed4c612cce4cda3f271fbdaad62d` |
| #873 base | `31fbe46ec5773ab65fee4f2fda68119d2bdc107b` |

`git log 31fbe46e..fce41bd3`: `23e9ae35` verify gate, `21ee4e95` fix, `7001da9c` capture option, `fce41bd3` result doc.
`git diff --stat 31fbe46e fce41bd3`: exactly 4 files.
- `Assets/ForestPrototype/ForestTreeMarkingManager.cs` +15/−0. This is the **only production file**.
- `Tools/Verification/Sen1MarkerScale/Sen1MarkerScaleVerification.cs` +323 and `README.md` +29. Both are under `Tools/`, outside `Assets/`, so the gate is not in the build. No temporary script or `.meta` remains under `Assets/`.
- The #876 result Markdown, +95.

**Scope: inside the Manager grant.** There are no unexpected production changes.

## 3. Primary check — marker transform (`ForestTreeMarkingManager.cs` @ `fce41bd3`)

The change (lines 589–590 and 626–638):
```csharp
marker.transform.SetParent(tree.transform, false);
CancelParentScale(marker.transform);              // new
...
private static void CancelParentScale(Transform marker)
{
    Vector3 parentScale = marker.parent.lossyScale;
    marker.localScale = new Vector3(InverseOrOne(parentScale.x), InverseOrOne(parentScale.y), InverseOrOne(parentScale.z));
}
private static float InverseOrOne(float value) => Mathf.Abs(value) > 0.0001f ? 1f / value : 1f;
```

| Aspect | Finding |
|---|---|
| Correctness on scaled roots | The marker root's world scale becomes about 1. The Diamond, Disc and Blue Trunk Band children (lines 592–620) keep their local positions and sizes in metres: disc 1.2 m, diamond at `tree.Height + 0.6`, band at `min(1.6, H/2)`. Before the fix, all of these were multiplied by the root's scale of about 10–14×, which is the F-1 defect. **Correct.** |
| Unit roots (historical worlds) | `1f / 1f == 1f` exactly, so `localScale` is `(1,1,1)`. That is the same transform as before the fix. **No regression.** |
| Non-uniform root scale | The root's own scale is cancelled exactly per axis, because its world matrix is `R·S` and the marker adds `S⁻¹`. The marker's rotated Diamond therefore has no shear. The cancellation would only be approximate if an *ancestor above* the tree root had both rotation and non-uniform scale. The verifier also covers a non-uniform `(6,14,9)` case. |
| Negative / zero scale | A negative axis gives a negative inverse, so the mirror is cancelled. A near-zero axis falls back to 1, so it never divides by zero, and the tree is degenerate in that case anyway. |
| Following the tree / removal | The marker is still a child of `tree.transform`. It follows the tree and is destroyed with it. `RemoveMarker` (lines 640–652), `RefreshMarkers` stale-pruning (lines 524–533) and `MarkerObjects` bookkeeping are unchanged. |
| Scale changes after creation | The inverse is computed once, when the marker is created. If a root's scale changed later, an existing marker would drift. I searched `Assets/**/*.cs` for `localScale` writers. Every ForestTree writer touches child visuals only: trunk/canopy (`ForestTree.cs:697,711`), polished and stump visuals (`ForestTree.cs:466,482`, and `PlantationTreeVisual` on the polished child), and Compact juvenile trunk/canopy (`Compact30Session.cs:429,435`). No code rescales a tree **root** at runtime. **Not a defect for this candidate.** |

**No concrete defect found in the 15-line change.**

**Non-blocking observation O-1 (outside #876 scope).** `CreateRackEntranceMarker` (`ForestTreeMarkingManager.RackEntrances.cs:193–228`) also parents to `tree.transform`, builds its orange stripe mesh in metres and has no scale cancellation. On a scaled Compact root, the stripes would be about 10–14× too large. This cannot happen in the current Compact30 candidate: `Compact30Session.cs` has no rack or entrance code, and its header says it has "no five-rack UI". It will matter if rack entrances are ever enabled on scaled roots. In that case, call `CancelParentScale(marker.transform)` after line 199. No change is requested now.

## 4. Secondary check — composed Compact30 compatibility (spot check)

- Tree roots: `Compact30Bootstrap.BindTrees` (lines 207–226) adds `ForestTree` directly to each authored scene child of `Trees`. Those children are mapped by roster id in `TryMapAuthoredTrees` (lines 153–178). These are the scaled prefab-instance roots. The scene file has `m_LocalScale` overrides of about 9.6–15.9 on prefab instances. The marker's parent is exactly this root, so the fix targets the intended transform.
- Marking manager present: `Compact30Bootstrap.cs:112` adds a `ForestTreeMarkingManager`.
- C/X input: `ForestTreeMarkingManager.cs:173–176` maps X → `ToggleMark` and C → `ToggleCropTree` on the aimed tree. Both go through `Mark` and then `CreateMarker` (line 252), which is the path the gate drives.
- Year 2: Compact juveniles (`CreateJuvenileVisual`, `Compact30Session.cs:441`) are plain GameObjects, not `ForestTree`s, so the fix does not touch them. `CanAdvanceYear` (line 398) is unchanged by #876.
- **I found nothing in source that would block the owner's short C/X + three Crop Trees + Year 2 walk.** I did not reopen the 74-file art integration or the #873 71/71 tests.

## 5. Evidence quality

**Read in source (independently checked):** `Sen1MarkerScaleVerification.cs` @ `fce41bd3`. Its 110 checks are meaningful, not a superficial count.
- `Measure` (lines 210–241) asserts against **world-space renderer bounds**:
  - mark `lossyScale` is 1 ± 0.01;
  - disc 1.2 ± 0.1 m wide and sitting on the ground;
  - diamond centre at `ground + H + 0.6` ± 0.05 with size < 0.8 m;
  - Crop band at chest height with trunk width, and no band on Fell marks;
  - 0 colliders;
  - parent is the tree.
- The run requires at least 9 real Sitka roots with `lossyScale.y > 5` (line 122). It marks 3 Crop and 2 Fell, checks exactly 5 visuals, and repeats unmark/re-mark ×3. It also checks that the disc follows a 2 m tree move, that unmark destroys the visual, and that unit and non-uniform `(6,14,9)` forced scales work.
- Historical mode requires *all* roots to be unit-scaled and checks `localScale == Vector3.one` on both marks.
- Pre-fix, the same assertions failed 37/110. That shows the gate can detect F-1.

**Author-reported only (not reproduced here):** Compact PASS 110/110 at `21ee4e95`, baseline FAIL 37/110, Legacy40 and Enlarged80 fresh new-game PASS 24/24 each, and the render captures. The gate drives the `Mark` API, not real key presses.

## 6. Still open (not certified by this review)

- **Manual GUI walk** (real C/X presses, three Crop Trees, Year 2 juvenile view): NOT RUN. This is for the owner playtest.
- **Old saved-game loads** (Legacy40 / Enlarged80): NOT RUN. Marker visuals are not saved, but this was not verified.
- **Frozen Reference Future / determinism hashes:** NOT RUN.
- Integration into main, Windows build and owner acceptance: not done and not implied.
- The #873 D-1 shared-preferences deviation is already documented. It must **not** be reset or restored.

## 7. Verdict

**#876 source fix: APPROVED.** There are 0 blocking, 0 major and 0 minor findings, plus 1 out-of-scope observation (O-1).
**Owner Editor playtest:** I found no concrete source blocker for the limited Compact30 C/X + three Crop Trees + Year 2 walk on `fce41bd3`. Batch success here and the source approval do not stand in for owner playtest acceptance.
