# T3 implementation handoff — retire the hidden Forestry Workbench from the new Scenario One starting world

**Task:** [CCF #284](https://github.com/jermur788/CCF/issues/284) — D-070.
**Branch:** `feature/scenario-one-workbench-retirement-20261105`
**Isolated lane:** `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105`
**Base:** `50042bbe97734b031210ddebb603b5ec00ff459d` (current `main`; 11 documentation-only commits ahead of production source head `9a6ed0007cc16469f3591f69bf0fd9b3c937ec61`; `Assets/Scenes/ForestTest.unity` bytes identical between the two).
**Implementation commit:** `57f10ff09a5e1b4e1e776ed7be2b0d46d6f7c411`
**Worker:** GPT-6.1 Sol Medium (Codex), delegated by the T3 orchestrator.
**Status:** **IMPLEMENTED ON THE ISOLATED BRANCH — SOURCE/OFFLINE VERIFIED; UNITY QUALIFICATION NOT RUN; NOT MERGED; NO WINDOWS BUILD.**

---

## 1. Preflight observed at dispatch

| Gate | Observation |
|---|---|
| Unity Editor | **No Editor process**, no `UnityLockfile`. Unity Hub was running; the Hub alone does not satisfy the playtest occupancy gate. |
| User personal playtest | **Not released.** No Unity launch attempted; Phase 3 remains blocked. |
| Machine-wide lease | `./scripts/unity-lock status` fails preflight: `lanes.json` records device `/dev/sda2`; ZX20 is actually `/dev/sdc2` (UUID `18D6-896E`, exfat match). No `lock.json` and no lease acquired. No force-release, reset, stash or process kill. |
| Target branch | Did not exist locally or remotely. |
| Target path | Did not exist; created as an isolated clone only after user approval. |
| Canonical store | `/media/jer/Files/CCF/Repository.git` is mounted **read-only**. A true `git worktree add` cannot write its admin files. The approved alternative is a self-contained clone at the exact proposed path. |
| Disk | ZX20 34 GB free after checkout, above the 15 GiB floor. No Unity Library was created. |
| Protected checkouts | `/media/jer/ZX20/CCF-MainIntegration-20261104`, `/media/jer/Files/CCF-Primary` and `/home/jer/dev/t3code` were not written to. |

---

## 2. Source audit (read-only, current production bytes)

- `Assets/Scenes/ForestTest.unity` contains the active root `Forestry Workbench` (`GameObject 778820502`, `Transform 778820505`) at `(-2.75, 0, 11)`, with `ForestBuildable 778820503` (`buildId: workbench-01`), a serialized `BoxCollider 778820504`, and built/unbuilt visual children. The whole subtree is **72 objects** (19 GameObjects, 19 Transforms, 16 MeshRenderers, 16 MeshFilters, 1 MonoBehaviour, 1 BoxCollider).
- **Four prerequisite references** point at the workbench component: Saw Pit (`113386332`), Timber Sledge (`719181071`), Basic Shelter (`1298786801`), Log Rack (`1773917806`). They are the only external references into the subtree, apart from the scene-root list.
- `ForestBuildable.IsPrerequisiteMet` is null-safe. `SetVisuals` hides the unbuilt site and disables every collider when `constructionAvailable` is false.
- Existing evidence (`Docs/Verification/S1BIntegration/BuildingFrames/audit-after.txt`) records `unbuiltVisible=False`, `unbuiltRenderers=0`, `unbuiltColliders=0`, no build prompt. **No invisible runtime collision is claimed**; the handoff's warning is respected.
- The real footprint is simulation-only:
  - `ForestStartingStand.CollectClearancePoints()` includes every `ForestBuildable`, inactive included.
  - `ScenarioOneManager.PrescriptionApproval.ReadCoverageWorld()` emits a `MaskKind.Site` circle for every buildable.
  - `ForestSaveController.CaptureData()` writes `workbench-01` into `buildables`, and `ScenarioReferenceArchive.WorldHash` includes that list.
- Frozen compatibility is live and must not move:
  - Reference Future v1 milestones contain `workbench-01` (unbuilt) at years 0/20/50/100; pinned world hashes `A564039D9B7CE31D`, `F7DF7DAB53B6FD32`, `D5E75D6D21D631AC`, `7AD177B3CC2F73C7`.
  - Enlarged80 evidence pins start `04A78188A6F8E6F9` and year 3 `081DB58CE3034648`.
  - No C# test names `workbench-01`; `MixedSpeciesTest.unity` and all prefabs/assets are untouched.

---

## 3. Approved Phase-1 contract (Option 3)

The user approved the narrow **new-game-scoped retirement** contract before any scene edit:

1. Add a serialized policy flag `retiresFromStartingWorld` to `ForestBuildable`, default `false`, and set it to `1` on the `workbench-01` scene instance only.
2. Exclude retired buildables from starting-stand clearance, MFM `Site` masks, and new-game save capture.
3. Reconcile retirement from `ForestSaveData.buildables` on load: a save that names `workbench-01` (old saves, Legacy40, Reference Future v1) keeps the workbench exactly; a new-game save that does not name it keeps it retired. **No save-schema change.**
4. Keep the GameObject, component, assets, prerequisite references and `workbench-01` ID so old saves and references remain valid.
5. Keep the MFM-08A gate's independent mask construction in parity with the new source policy.
6. Keep the dev scene builder consistent.

**Explicitly out of scope and unchanged:** workbench code/asset deletion, other buildables, `Dirt Path*`, access-spine/herringbone geometry, rack geometry or entrance policy, MFM topology, save schema, Reference Future archive, user playtest checkout, merge, Windows build.

---

## 4. Changed files and semantic reason

| File | Change | Why |
|---|---|---|
| `Assets/ForestPrototype/ForestBuildable.cs` | Policy flag, runtime retirement state, hide/solid/update/build guards, public accessors | One reusable buildable can be retired from the new-game world without deleting the class or prefab-style scene content. |
| `Assets/ForestPrototype/ForestStartingStand.cs` | Skip retired buildables in `CollectClearancePoints()` | Removes the workbench's starting-tree reservation for a new game. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs` | Skip retired buildables when building `source.Sites` | Removes the phantom MFM `Site` exclusion for a new game. |
| `Assets/ForestPrototype/ForestSaveController.cs` | Skip retired buildables in `CaptureData()`; reconcile retirement from saved buildable ids in `LoadData()` | New-game saves omit `workbench-01`; old saves and Reference Future restore it, and exiting the Reference preview restores retirement. |
| `Assets/Scenes/ForestTest.unity` | One line: `retiresFromStartingWorld: 1` in the `workbench-01` block | Marks the instance only; no object, reference or transform changes. |
| `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` | Independent Site construction skips retired buildables | Keeps the gate's source-parity assertion true; no assertion removed or weakened. |
| `Assets/ForestPrototype/Editor/ForestSceneBuilder.cs` | Set the flag when generating the workbench | A dev-regenerated scene cannot silently reintroduce the footprint. |

`git diff --stat`: **7 files changed, 34 insertions(+), 9 deletions(-)**. The scene diff is **exactly one added line**. No `.meta`, asset, package, project-setting or other scene change.

---

## 5. Compatibility analysis

| World / save | `workbench-01` in save | Runtime retirement | Result |
|---|---|---|---|
| New Scenario One game (fresh Enlarged80) | No (capture skips it) | `true` | No clearance point, no MFM Site mask, no saved buildable, no visuals/collider/prompt. |
| New-game save reloaded | No | `true` | Stays retired. Round-trip stable. |
| Old v24 / older save with `workbench-01` | Yes | `false` | Existing workbench state is restored exactly; no save-file migration. |
| Legacy40 verification replay | Yes, if it names it | `false` | Unchanged behaviour and identity. |
| Reference Future v1 preview/continuation | Yes in every archive milestone | `false` while loaded | Archive integrity, preview restore and continuation hashes are unchanged. Exiting the preview restores the player's retired new-game state. |

**Identity delta, explicitly accounted for:** a new-game capture loses exactly one `buildables` entry (`workbench-01`). It does **not** lose or move a tree. The Enlarged80 start/year-3 evidence labels must receive **new, labelled** values during Phase 3; the old values are not silently overwritten. Legacy40 and the frozen Reference Future anchors are untouched.

---

## 6. Verification evidence

### 6.1 Offline checks — PASS

Command: `python3 /tmp/opencode/ccf-workbench-verify.py` (read-only; script and log are outside the repository).

```text
SCENE_GRAPH_PASS
  workbench_subtree_objects=72 (GameObjects=19, Transforms=19)
  workbench_component=778820503 buildId=workbench-01 flag=retiresFromStartingWorld:1
  prerequisite_refs=4 [113386332, 719181071, 1298786801, 1773917806]
  scene_roots_reference_present=True (object retained, so expected true)
STARTING_STAND_PASS
  slots_generated=1681 workbench_clearance_slots=5 clearing_slots=5
  workbench_only_slots_without_clearing=0 []
  workbench_only_slots_also_covered_by_player=0
  workbench_only_slots_also_covered_by_saw-pit=0
  workbench_only_slots_also_covered_by_log-rack-01=0
  workbench_only_slots_also_covered_by_log-rack-02=0
  workbench_only_slots_also_covered_by_grinding-stone=0
  workbench_only_slots_also_covered_by_timber-sledge=0
  workbench_only_slots_also_covered_by_shelter=0
MFM_SITE_DELTA_PASS
  year=  0 trees=336 in_site_circle=0 in_site_but_not_clearing=0
  year= 20 trees=352 in_site_circle=0 in_site_but_not_clearing=0
  year= 50 trees=401 in_site_circle=1 in_site_but_not_clearing=0
  year=100 trees=517 in_site_circle=3 in_site_but_not_clearing=0
  site_circle_minus_clearing_crescent_area_m2=0.0969
  centre_separation_m=0.021994
CHANGED_FILES_PASS
  Assets/ForestPrototype/Editor/ForestSceneBuilder.cs
  Assets/ForestPrototype/ForestBuildable.cs
  Assets/ForestPrototype/ForestSaveController.cs
  Assets/ForestPrototype/ForestStartingStand.cs
  Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs
  Assets/Scenes/ForestTest.unity
  Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs
ALL_OFFLINE_CHECKS_PASS
```

**Changed site/stand metrics**

| Metric | Baseline | Candidate |
|---|---|---|
| Starting-tree lattice slots | 1,681 | 1,681 (unchanged) |
| Slots excluded only by the workbench | 5 | **0** (all 5 are already inside the `Forest Clearing` circle) |
| Living starting trees (Enlarged80 evidence) | 1,344 | **1,344** (no ID/position/dimension change expected) |
| MFM Site circles | 7 | **6 for a new game** (6 for old saves/Reference) |
| Site-only crescent removed | — | **0.0969 m²** |
| Stems in removed crescent | — | **0 at Year 0/20; 0 at Year 50/100** (the 1/3 stems in the circle are also inside `Clearing`) |
| New-game world hash | `04A78188A6F8E6F9` | **PENDING Unity measurement**; only the omitted `workbench-01` buildable entry changes it |

### 6.2 Unity / Editor — NOT RUN (blocked, not passed)

- The user's personal playtest occupancy was not released.
- The machine-wide lease tool is misconfigured (`/dev/sda2` vs actual `/dev/sdc2`), so a valid lease could not be acquired.
- Therefore the following are **NOT RUN**, not inferred: compile/import, fresh Enlarged80 tree count/IDs/hash capture, MFM Site/Track/Headland membership measurement, old v24/older save load/CanLoad/round-trip, Legacy40 and Reference Future contract gates, rack/Objective 1 non-regression, 720p/1080p captures.
- No tree or coverage change is claimed as "verified at runtime"; the zero-tree and 0.0969 m² Site results are deterministic offline measurements against the production scene and frozen archive.

### 6.3 Static/source checks — PASS

- `git diff --check`: clean.
- `git status --short`: clean after commit.
- The scene contains exactly one `retiresFromStartingWorld: 1`, on `ForestBuildable 778820503` (`workbench-01`), immediately after `builtVisual`.
- All four `requiredBuildable` references and the 72-object subtree are intact.
- Every changed query path is guarded by `IsRetiredFromStartingWorld`; the only unguarded enumeration (`ScenarioOneManager.MoveToReferenceView`) is presentation-only and runs only on Legacy40/reference worlds where the workbench is intentionally active.
- No C# test references `workbench-01`; no other scene, prefab, asset, `.meta`, package or project-setting file changed.

---

## 7. Independent review

**Claude Sonnet 5.5 Medium (read-only, opposite provider): PENDING — review dispatched after publication of this draft; verdict and findings will be appended as a follow-up commit, not edited into the evidence above.**

---

## 8. Stop reasons, open items and Manager request

**Not a stop condition:** the implementation itself is complete on the isolated branch; no migration or construction-system rebuild was required.

**Blocking follow-ups:**

1. **Unity Phase 3** requires the user to finish/stop the personal playtest and explicitly release occupancy, plus a corrected lease configuration (`lanes.json` device identity) and a fresh exclusive lease. Until then, compile/runtime/save/reference/MFM gates are **NOT RUN**.
2. **Enlarged80 anchor re-baseline:** Phase 3 must record the new labelled start/year-3 `CurrentWorldHash` values and the exact `buildables` delta. The old `04A78188A6F8E6F9` / `081DB58CE3034648` labels must not be reused.
3. **MFM-08A gate:** re-run after the retirement-filter edit. The expected result is the same assertions with `site.Count = 6`; any other difference is a finding, not a waiver.
4. **Old-save/reference matrix:** load v24 and relevant older saves, Legacy40 and Reference Future v1, and confirm the workbench restores only for saves that name `workbench-01`.
5. **Independent review:** complete and record the Claude Sonnet 5.5 Medium verdict.

**Manager integration request:** review this branch and the Phase-3 plan; do not merge or build. The branch is isolated, the source diff is seven files, and the only production behaviour change is new-game retirement. After Phase 3 evidence and the labelled anchor update, the Manager alone decides integration.

---

## 9. Reproduction

```bash
git -C /media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105 show --stat 57f10ff0
git -C /media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105 diff 50042bbe..HEAD
python3 /tmp/opencode/ccf-workbench-verify.py
```

The verification script is intentionally outside the repository; it reads the production scene, the frozen Reference Future v1 archive and the branch diff, and asserts the scene graph, the zero-slot clearance delta and the Site/stem delta.
