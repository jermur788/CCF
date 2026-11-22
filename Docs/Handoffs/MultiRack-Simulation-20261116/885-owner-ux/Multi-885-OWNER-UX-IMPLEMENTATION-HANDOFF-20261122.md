# Multi #885 owner UI implementation handoff

Implementation on task branch; build blocked by missing prebuilt P0 dependency. DONE below means source implementation, not rendered or behavioural acceptance.

- Worker/role: CCF Sol Writer, sole implementation writer.
- Branch: `feat/multi-885-owner-ux-20261122-v1`.
- Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-OwnerUX-20261122`.
- Base: `4cecf87339bea4d32907955a8d5625d1f27f96bd`.
- Inspected context commit: `62e39ec621e38a84030c08b35a171e297056c9ed`.
- Committed source SHA: `b3a62953bd614736f4d5bc7dcbcf0de0b95037e0`.
- Handoff commit: the separate commit adding this file after the source commit; neither commit pushed by this worker.

The latest direct packet expands the older spec's three-file grant to include ONLY `MultiAreaPage.Html`, `.Css` and `.Js` inside `PlayerMultiApi.cs`. The owner `/multi` page and the sibling single-area page both contain the improvements. No Core, API contract, fixture, Unity, save, package or other-worktree source was edited. Starting and post-source-commit tracked status were clean; there was no unrelated work to preserve in this lane.

Exact changed files:

1. `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` — presentation literals only.
2. `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/index.html`.
3. `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.css`.
4. `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js`.
5. `Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-OWNER-UX-IMPLEMENTATION-HANDOFF-20261122.md` — this handoff only.

| Item | Status | Source evidence |
|---|---|---|
| U01 | DONE | Both scripts contain `CANOPY_PALETTE`, Unity hash/value-noise adaptation, five shared large/small pointed whorl variants, lit branch facets/needles, leader, seeded tint/oval/rotation, SE soft shadow, bare dead branches and stump discs; each original true-radius `circle.crown` remains first and solely interactive. |
| U02 | DONE | Both HTML surfaces put N/W/Pan/E/S compass inside `.map-viewport`; compass targets are 44px and retain IDs/labels/titles; vertical zoom is separate and only buttons intercept pointer events. |
| U03 | DONE | Multi `ZOOMS` has eleven fixed levels from 1 to 4; single `SCALES` has Fit plus ten fixed levels to 5; existing function names now use clamped metre viewports, selected-tree Fit anchoring, reversible steps and truthful scale notes. |
| U04 | DONE | Existing any-living-tree selection paths still call their fenced read-only competition endpoint immediately; both reports display server top-five data and explicit zero-neighbour text without Crop or pending-order writes. |
| U05 | DONE | Rank buttons preserve `competitionTarget`/`competitionTargetId`, highlight the exact selected neighbour, expose `Add tree <ID> to Fell plan`, offer `Return to reference tree <ID>` and frame both trees when zoomed; add/approve/execute payload and confirmation fences remain intact. |
| U06 | DONE | Dropdown removed on both pages; intentional optional-note disclosure and contextual unchecked release checkbox use explicit precedence: trimmed note, else selected release intent, else exactly `Owner-selected felling`; Crop routes still require no reason. |
| U07 | DONE | `technicalPlan()` uses collapsed `Technical reference` details for full IDs/keys in plan/confirmation/receipt contexts; routine history/announcements show human values; internal confirmation/replay tokens stay unchanged. |
| U08 | DONE | Single page separates Tree and Work Plan panes; both move approval/execution controls ahead of long order lists, collapse full tree records and compact competitor rows; map stays dominant and sub-1024px layouts stack. |
| U09 | DONE | Both maps use Crop C, pending dashed P, approved double-ring A, stump rings/cross/X, bare branches/D, dashed Reference halo/tag and numbered competitors plus selected-candidate tag; legends describe these cues. |

Graphics are an efficient SVG adaptation of the inspected plain-C# reference, not a pixel-identical raster port. Shared branch facets/needles and sampled value-noise tiles approximate raster leaf/pixel shading. Five seeded rotations are baked into shared outlines so lighting stays north-west; per-tree seeds select variant, opacity and oval stretch. No new tree species or biological radius/state is inferred. Broadleaf palette values are retained as reference constants but not presented as synthetic Sitka data. U10/U11 remain outside scope; no unsupported light, seed, regeneration, browsing or economics data was added.

Commands and actual results (external helper scripts are in `C:/Users/hp/AppData/Local/Temp/opencode/`; they are not repository deliverables):

- `wsl -d Ubuntu -- git -C /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-OwnerUX-20261122 status --short`, `log -1 --format=%H%n%D`, and `remote -v`: clean requested branch/base; origin is the CCF GitHub repository.
- Read-only inspection used PowerShell `Get-Content -LiteralPath`/`Select-Object` and `rg -n` against the workflow/context, three Unity references and the UI/API/project files. One Windows `rg` call with a literal `Core/Work*` path failed on wildcard path syntax; the directory/glob search succeeded subsequently.
- `wsl -d Ubuntu -- bash /mnt/c/Users/hp/AppData/Local/Temp/opencode/ccf885-read.sh`: read the approved remote spec quartet using `gh api -H 'Accept: application/vnd.github.raw+json'` on `docs/multi-885-owner-ux-implementation-spec-20261122-v1`, read live `main` coordination the same way, and inspected the context commit. No fetch/merge/pull/branch switch or remote write.
- `python C:/Users/hp/AppData/Local/Temp/opencode/ccf885-edit.py`, `ccf885-refine.py`, and `ccf885-finish-source.py`: bounded edits to the four allowed source files, plus external syntax copies. The first script asserted non-presentation C# identity before writing.
- Direct Windows `node.exe --check` on the UNC single-page source initially failed with `ERR_INVALID_PACKAGE_CONFIG` while traversing the UNC package path; no JavaScript syntax verdict resulted from that attempt.
- `& 'C:/Program Files/nodejs/node.exe' --check C:/Users/hp/AppData/Local/Temp/opencode/ccf885-single-syntax.cjs` and the equivalent `ccf885-multi-syntax.cjs`: both exited 0 with no syntax errors on external copies of the final source.
- `wsl -d Ubuntu -- bash /mnt/c/Users/hp/AppData/Local/Temp/opencode/ccf885-source-check.sh`: passed byte masking comparison against the base for everything outside the three C# presentation strings, checked for prohibited inline style/`element.style` assignments, and ran `git diff --check`/`git diff --stat` successfully.
- `wsl -d Ubuntu -- bash /mnt/c/Users/hp/AppData/Local/Temp/opencode/ccf885-build.sh`, whose build command was exactly `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` from this lane's `Tools/MultiRackSitkaP1Lab`: **FAILED**, exit 1, 0 warnings, 1 error: `P0 Core reference missing. Run Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh first.` Restore succeeded; compilation stopped at the protected dependency guard. Only this one build was attempted.
- `wsl -d Ubuntu -- bash /mnt/c/Users/hp/AppData/Local/Temp/opencode/ccf885-commit-source.sh`: scoped `git add`, whitespace check and WSL commit with author `CCF Sol Writer <writer@local>` succeeded at the source SHA above; tracked status was clean.
- Final base-to-HEAD allowed-path/byte-boundary/whitespace checks and handoff commit use `ccf885-finalize.sh`; the final response records its resulting handoff SHA.

**Runtime check: NOT RUN.** The missing pinned prebuilt `.p0ref/SitkaLab.Core.dll` prevents creating the changed runnable assembly. The packet expressly prohibits P0 rebuilding and grants no cross-lane binary-copy permission; no recovery build script was run. No UI process was started or stopped, no HTTP request was made to the demo ports, and no HTTP 200 result is claimed.

No automated test suite, Core tests, ui-smoke scripts, Unity build, Windows build, long scenario, independent review, browser rendering or gameplay interaction check was run. Desktop 1280x720 appearance, 200% zoom, actual crown pointer behaviour, note/release submission, approve/execute/replay and annual outcomes are not directly checked in a running browser. Source/syntax checks and the failed build are the entire validation evidence.

Owner access after the orchestrator supplies the exact pinned prebuilt P0 reference through an authorized action and builds this candidate:

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-OwnerUX-20261122/Tools/MultiRackSitkaP1Lab
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

Open the locally printed **Two-area MA-1 simulator** address ending in `/multi`. Its capability stays in the local terminal and is deliberately absent here. The printed single-area address opens the sibling page. Expected source-defined flow: select a living tree, read ranked neighbours, pick a rank, optionally add a note or release intent, add the exact candidate to pending Fell, then separately approve and confirm Execute. Fit restores the currently shown area's complete map. Process exit ends this session; no saving is added.

Remaining blocker: the lane is not buildable without its pinned prebuilt P0 dependency. Remaining publication belongs to the orchestrator: push the two commits and publish the GitHub-tracked handoff; no integration, dashboard synchronization or owner acceptance is claimed by this worker.
