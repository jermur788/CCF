# CCF #857 — implementation stopped before source edits

**BLOCKED (`blk-7c3c05c913d8de26a`) / NOT IMPLEMENTED / NOT INDEPENDENTLY REVIEWED / NOT INTEGRATED.** The Unity licence smoke passed. No starter scene, source seam or qualification harness was written.

## Protected-source stop

The authorised shape requires the existing `ForestPlayer` to handle walking and looking in a manager-less blank scene, without exposing a world inventory or planting affordance. In the pinned base:

- [`ForestPlayer.OnGUI`, lines 992–1034](https://github.com/jermur788/CCF/blob/cbcb3850c3952a8d74e54606279165b2f4be9add/Assets/ForestPrototype/ForestPlayer.cs#L992-L1034) only yields the legacy IMGUI HUD to UI Toolkit when a `ScenarioOneManager` is present. Without that manager, the enabled player paints `Carried Wood` and its stock/capacity every GUI pass. It does so even with the cursor unlocked and even when no world actors exist. The shared UI views cannot switch off this callback on the same component that provides movement; changing its picking mode or disabling the HUD view does not touch the legacy draw call.
- [`ForestPlayer.Update`, lines 346–376](https://github.com/jermur788/CCF/blob/cbcb3850c3952a8d74e54606279165b2f4be9add/Assets/ForestPrototype/ForestPlayer.cs#L346-L376) owns movement and then calls the interaction update. [`UpdateTreeInspection`, lines 489–524](https://github.com/jermur788/CCF/blob/cbcb3850c3952a8d74e54606279165b2f4be9add/Assets/ForestPrototype/ForestPlayer.cs#L489-L524) explicitly falls back to legacy planting on aimed ground when no Scenario One manager exists. Clearing the scene's planting species array could prevent entering the mode but still leaves the legacy HUD and the unavailable-species action. No successful planting was attempted or inferred.

`Assets/ForestPrototype/ForestPlayer.cs` is not writable under the #857 allowlist. Disabling it loses walking/look; hiding the stock counter under another overlay or adding a manager to silence it would mask rather than remove the unsupported feature. The narrow continuation is for Scenario One source owner **#844** to grant exactly this file to the same writer for the preview seam: keep movement/look, return before world interaction in explicit preview mode, and suppress only the preview's legacy `OnGUI`. Preserve non-preview behaviour. This is a recommendation, not an amendment to the grant. No production implementation decision or independent verdict is claimed.

## Provenance and execution

- Worker: `openai/gpt-6.2-sol` (GPT-6.2 Sol), OpenCode session `ses_e07995150ffeJJDYGO8Usc2FTJ`, `jer-laptop`.
- Lane: `/media/jer/ZX20/CCF-Sen1-857-BlankStarter-20261120`; branch `feat/ccf-857-blank-starter-20261120-v1`; clean implementation base `cbcb3850c3952a8d74e54606279165b2f4be9add`. Live `main` was `cba0a905b766ce8fc0356d351ac5bff1c2a9a008`, one coordination-only delta from that base; the feature branch was not moved onto it.
- Read and blob-verified all four files at `23da9850a7245a8623244ff2e2e76fbf46c33f0c` via the specified `gh api … --jq .content | base64 -d` and `git hash-object`: `d93d00d71bc9d1835723804eb15152f90cfa18a7`, `445cd03720eb94aef82821b5407cf789f8ca0ed9`, `4bb84c8dd55fdd0ef3b84c26715067266b19cad8`, `846380b1c5b8acb5e696b7815927ba50a8a5ee6f`. The frozen files' DRAFT banners were not edited; [#857's separate receipt](https://github.com/jermur788/CCF/issues/857#issuecomment-7036660288) records the delegated implementation scope. The physical board text still describes O0; it was not edited.
- The deployed `unity-lock status` reported **free, no Editor PIDs**. Acquired clean with the dispatched owner, task ID, lane, branch, base and compatibility ETA `2027-12-31T23:59:59+00:00`; lock `f736db4d50914aafbf82f920d707ef8c`. Released the same ID normally with matching owner/task after the smoke and pre-edit stop; readback: **free, no Editor PIDs**.
- Licence **PASS** in running Flatpak Hub instance `3611728743`, Unity `6000.6.0f1`. First invocation intentionally used a nonexistent project and exited 1 after the licensing IPC connection; the valid minimal empty project inside sandbox `/tmp` connected, resolved entitlement, updated licence and exited 0. Token refresh was reported unavailable, as in #830, but did not prevent entitlement. Neither invocation opened the feature checkout as a Unity project.
- Exact allowlist diff from the implementation base: **one added file**, this Markdown handoff. No changes to any game C#, serialized scenes, ProjectSettings, packages or save assets. No historical save fixture staged at the live slot; no save content read or written for qualification. No merge, integration, Player/Windows build or owner playtest.

The successful smoke command (after creating a temporary project containing empty `Assets/`, `Packages/manifest.json` with `{"dependencies":{}}`, and `ProjectSettings/ProjectVersion.txt` pinning `6000.6.0f1`):

```sh
flatpak enter 3611728743 /usr/bin/env PATH=/usr/bin:/bin HOME=/home/jer \
  XDG_CONFIG_HOME=/home/jer/.var/app/com.unity.UnityHub/config \
  XDG_DATA_HOME=/home/jer/.var/app/com.unity.UnityHub/data \
  XDG_CACHE_HOME=/home/jer/.var/app/com.unity.UnityHub/cache \
  /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity \
  -batchmode -nographics -quit -projectPath /tmp/ccf857-licence.Ks6gJk \
  -logFile /media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120/licence-smoke-empty.log
```

## Acceptance results

| Check | Result |
|---|---|
| Exact specification fetch and four Git blob checks | PASS |
| Exclusive lease acquisition, licence smoke, identity-matched release | PASS |
| Pre-edit fit under protected-source allowlist | FAIL — `ForestPlayer` legacy HUD and interaction fallback |
| AC-01 — candidate import/compile, bounds, spawn, movement; N-13 invalid footprints | NOT TESTED |
| AC-02 — shared HUD/menu and headings, unavailable inspection; N-01 | NOT TESTED |
| AC-03 — blank map/rack/layers/transition; N-02, N-14 | NOT TESTED |
| AC-04 — empty Work Plan and nonoperational controls; N-03 | NOT TESTED |
| AC-05 — empty Annual Review and Objectives; N-04 | NOT TESTED |
| AC-06 — historical save access negatives; N-05 through N-09 | NOT TESTED |
| AC-07 — rectangular scene and map copy; N-10 | NOT TESTED |
| AC-08 — all three authoring text slots; N-11 | NOT TESTED |
| AC-09 — historical ForestTest positive and anchors; N-12 | NOT TESTED |
| AC-10 — implementation review and candidate scope qualification | NOT TESTED — no implementation candidate |

**Evidence:** `/media/jer/Files/CCF/Verification/CCF-857-BlankStarter-20261120/`: `licence-smoke.log`, `licence-smoke-empty.log`, `lease-before.txt`, `lease-acquire.json`, `lease-pre-release.json`, `lease-release.json`, `lease-after.txt`, `pre-edit-player-stop.txt`. No candidate console, screenshots, historical save hashes or no-access trace were produced. A separate `/etc/hosts` probe confirmed `strace -f -P` can run inside this Flatpak instance; it is not historical save evidence.
