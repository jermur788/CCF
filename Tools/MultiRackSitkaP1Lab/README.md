# Multi-Rack Sitka P1 Lab (F0 foundation)

Standalone sibling of the P0 lab (`Tools/MultiRackSitkaLab`, which this tool never writes). Specification:
`specs/multirack-sitka-p1-adult-rules-comparator-v1/`. Task: CCF #639.

> **SYNTHETIC CLOSED ADULT SITKA. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.**

## What exists in this stage

| Part | State |
|---|---|
| Three domain tags (`DerivedP1ThirtyMetreExperimentV1`, `DerivedUnitySquareParityV1`, `OriginalPolygonExploratoryV1`) with exact, case-sensitive parsing | Implemented |
| Deterministic 30 × 30 m starting-forest manifest `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` and its strict parser | Implemented. **Pending independent review and Manager acceptance** |
| Compiled original-P0 trust table (3 fixtures) and read-only, no-follow, descriptor-checked inspection | Implemented. **Records pending review**: normal inspection refuses them |
| Annual gate | Refuses every domain. **No annual engine exists in this stage** |
| Reuse of Scenario One `SimulationRandom.cs` and `SitkaGrowthModel.cs` | Linked unchanged (read-only), function-level probes only |
| Manual Work Plan: pending, approve (0 Fell), execute (one typed Fell), replay, real Year+1 with consent for unexecuted work | Implemented in the in-memory session (G2/A-1 engine); reachable through the #849 player facade and UI |
| Player session facade in Core: `PlayerSessionView`, `PlayerWorkPlanAdapter`, `PlayerMapScene` (one-lock view, fenced commands, engine-minted PlanId and key) | Implemented (#849 PM-T01–T04), with the Tier-2 repair: a first Execute must echo the minted key. The independent review of the earlier candidate returned NEEDS CHANGES; **this repair is not yet re-reviewed, not integrated** |
| Playable 2D Stand Map: the loopback UI process `src/Ui` and its page (see below) | Implemented (#849 PM-T05–T07), with the Tier-2 repair. One in-memory session per process on `127.0.0.1` behind a capability URL. **It is an HTTP server**, allowed only as this single presentation process (spec `multirack-sitka-2d-playable-standmap-v1` §0.4 Q-1). **Not yet re-reviewed, not integrated, not user accepted** |
| Saves, load or resume, `.p1-data/` root, persistence Host, H2, several users, remote clients | **Not implemented** (UNAVAILABLE). Ending the process ends the session |
| Comparison extension (G1), proposals, representative samples | **DISABLED — Manual/NotCovered** |
| Costs, revenue, budget | **UNAVAILABLE (not €0)** |
| Racks, corridors, Scenario One map code, Unity | Not used. The map is a small local SVG layer (spec §4, decision B) |

The rows above the Work Plan row describe the F0 foundation stage as first written; #849 did not re-verify them.

## Playable 2D Stand Map (Stage 5 slice, #849)

Specification: `specs/multirack-sitka-2d-playable-standmap-v1/` (UI-A). One player and one in-memory session on one
undivided 30 × 30 m management area of 100 synthetic Sitka. Select exact trees on a north-up map or in the tree table,
add them to a Work Plan with a written reason, approve the whole plan (no tree is felled), execute it (one typed Fell
event, or nothing), then advance one real ecological year from the survivors.

### Launch (Ubuntu on WSL2, from the repository root)

The build needs the frozen P0 reference `.p0ref/SitkaLab.Core.dll`, which Git ignores. **On this path it is never
rebuilt** (spec §3.5, correction C-1). Copy it from an authorised, already-qualified, read-only pinned copy, and check that
four values are equal before building. If any value differs or a file is missing, STOP. Do not run
`build/build-p0-core-ref.sh`.

```bash
cd Tools/MultiRackSitkaP1Lab
PIN=9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3   # 1. P0CoreExpectedSha256 in src/Core/SitkaP1.Core.csproj
SRC=<authorised pinned .p0ref directory>                                # read-only; record this path in your run notes
mkdir -p .p0ref && cp "$SRC/SitkaLab.Core.dll" "$SRC/SitkaLab.Core.dll.sha256" "$SRC/provenance.txt" .p0ref/
sha256sum .p0ref/SitkaLab.Core.dll                  # 2. the DLL itself: must print $PIN
cut -d' ' -f1 .p0ref/SitkaLab.Core.dll.sha256       # 3. the .sha256 record: must print $PIN
grep '^dllSha256=' .p0ref/provenance.txt            # 4. provenance.txt: must show $PIN
#   any mismatch or missing file: STOP
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

The process prints:

```text
Synthetic 30 m Sitka experiment. Session only — nothing is saved.
Open http://127.0.0.1:<port>/s/<capability>/
Ctrl+C ends the session; nothing is saved.
```

Open the printed address exactly, for example in Microsoft Edge on Windows (WSL2 localhost forwarding lets a Windows
browser reach it). The operating system picks the port at each launch, and it is never 5187. Any other host name,
including `localhost`, is refused with 403, and an address without the capability gets an empty 404. Press **Ctrl+C** in
the terminal to end the session. Nothing is saved: the next launch starts again at Year 0.

The UI project is deliberately **not** in `MultiRackSitkaP1Lab.sln`, so build it by its project path as shown. Set
`TMPDIR` outside the repository, as for the other builds. Exit codes: 0 after Ctrl+C, 2 the fixture was refused (the code
is printed), 3 the listener check failed, 64 usage.

### Using it

1. **Select a tree.** Click or tap a crown on the map, press *Select* in the tree table (the keyboard and screen-reader
   path), or type an exact, case-sensitive ID in the search box and press Enter. Where crowns overlap, the topmost is
   selected and the others are listed beside the map. Clicking bare ground selects nothing.
   - **Layout.** The page is one column at every window width and zoom level, so 200 % zoom of a wide window (for
     example 1920 px, a 960 CSS px viewport) is one column too. Only the map pane and the tree table scroll sideways.
   - **Zoom and pan.** The map scrolls inside its own pane. Below 900 CSS px wide (for example 360 px, or 200 % zoom) it
     opens at 64 CSS px per metre, where every crown and stump is at least 44 × 44 px; the smallest, a 0.35 m stump, is
     44.8 px across. A wide desktop window at ordinary zoom opens on the whole area. Use *Zoom in*, *Zoom out*, *Whole
     area* and the four *Pan* buttons, or scroll or drag the map. Choosing a tree in the table scrolls the map to it.
     Trees keep their true crown radii at every zoom, and a click selects only a tree whose drawn disc is under the
     pointer.
2. **Inspector.** It shows the exact ID, status, position, age, DBH, height and crown radius, and for a dead tree its death
   year and cause. Write a reason and choose *Add to Work Plan — no tree is felled*.
3. **Work Plan.** Remove pending orders, or choose *Approve whole Work Plan — 0 trees felled*. The engine mints the PlanId
   and the execution key. *Cancel approved plan* withdraws the plan; its orders do not return to pending.
4. **Execute approved plan.** A confirmation names the trees, the year and the PlanId, and sends exactly the plan it
   shows. If the current view stops showing that plan under the same fence while the confirmation is open, it is
   withdrawn, nothing is sent, and the page asks for a re-read. The receipt shows the felled IDs with cause
   `fell (manual-work-plan)`, the work revision w → w+1, and the living count and basal area before and after. The map
   shows a stump where the crown was. A batch that would leave no living tree is refused as a whole (D-093).
5. **Advance one ecological year.** If work is still unexecuted, choose *Keep orders — stay in Year y* or *Cancel
   unexecuted work and advance one year*. The history lists every committed year and every work event.

Every value on the page comes from the engine's latest view. The page computes no forestry statistic and keeps no state of
its own. If a request fails in transit, the page shows *Outcome unconfirmed — re-read current state* rather than guessing.
Until a re-read succeeds, every command is disabled, refused if activated anyway, and no request except a read leaves the
page. If the plan is still approved after the re-read, Execute echoes the same engine-minted execution key, so it can
never fell twice; the engine accepts only that minted key for a first execution.

### Honesty boundaries

- **It is an HTTP server.** It is allowed only as this single, in-memory, loopback presentation process (spec §0.4 Q-1):
  `127.0.0.1` only, an ephemeral port other than 5187, a 256-bit capability in the URL path, exact Host and Origin checks,
  JSON-only POST bodies of at most 16 KiB, no CORS, no outbound network and no file written. The Stage 5 no-Host rule is
  narrowed for this one process, not withdrawn.
- Session only: no save, load or resume, no persistence Host, no H2, no several users and no remote clients.
- Comparison extension (G1), proposals and representative samples: **DISABLED — Manual/NotCovered**. Manual exact-ID
  selection is the whole feature, so Stage 5 R-06 and X-05 are not met by this slice.
- Costs, revenue, budget: **UNAVAILABLE (not €0)**.
- A synthetic closed adult Sitka experiment: not an Irish field forecast, and not Unity numerical or geometry parity.
- No racks, corridors, entrances or marking, and no Scenario One or Unity code. The map is a small local SVG layer.
- Author checks only for this repair: **not yet re-reviewed, not integrated, not user accepted**. No screen reader has been
  run.

### Test the UI process

```bash
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
TMPDIR=<directory outside the repository> tests/ui-smoke.sh [<evidence directory outside the repository>]
```

`tests/ui-smoke.sh` uses bash, curl, the python3 standard library and node (no packages; `NODE` overrides which node). It
starts the built UI against a scratch copy of the fixture, with hostile URL environment variables set. It checks that the
process listens only on 127.0.0.1, exercises every guard and one full Work Plan cycle through the JSON API, and then runs
the page's own served `app.js` in a small DOM stand-in against the same process (the J checks: the Execute confirmation
is bound to the plan it shows, and after an unconfirmed outcome nothing but a read can be sent). The stand-in is not a
browser: it has no layout or CSS, so target sizes are not measured by it. Finally the smoke stops the process with SIGINT
and checks that nothing was written to the lab tree or `~/.aspnet`. Exit 0 means every check passed.

## Build and test (Linux x64, .NET SDK 8.0.133)

```bash
cd Tools/MultiRackSitkaP1Lab
build/build-p0-core-ref.sh              # P0 Core DLL from the pinned tree, built outside the repo -> .p0ref/ (ignored)
dotnet build MultiRackSitkaP1Lab.sln
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
tests/p0-isolation.sh <evidence-dir>    # cold rebuild + tests under strace; L0–L4 proof that P0 is untouched
tests/build-output-safety.sh <evidence-dir>  # builder vs hostile .p0ref states (symlink, file, hard link, ...) using scratch stand-ins
tests/cli-safety.sh <evidence-dir>      # documented CLI vs hostile outputs/inputs using scratch stand-ins
```

Set `TMPDIR` to a directory **outside** the repository. The build script refuses to run if it is inside.

## CLI

```bash
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll generate-manifest --stdout | sha256sum          # streams to a pipe/terminal only
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll generate-manifest --check fixtures/derived       # compare with the committed fixture
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll verify-derived p1d-30m-sitka-adult-v1 fixtures/derived
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll inspect-original ../MultiRackSitkaLab/fixtures/synthetic oracle-quarter-ha [--review-evidence]
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1   # always REFUSED in F0
```

- **No P1 command opens a file for writing.** `generate-manifest --stdout` refuses (`OutputNotStream`) unless standard
  output is a pipe, socket or terminal, so redirecting it to a file is refused before any byte is written (#643 B-01).
- Fixture commands take a **directory**. The file name and exact length come from the compiled catalog or trust table, and
  the file is read through the no-follow, descriptor-checked reader (#643 M-02).
- `--review-evidence` lets a reviewer run the full check sequence on a pending record. Its output is labelled
  `UNTRUSTED-PENDING-REVIEW` and never creates experiment state.

## Isolation rules

- `build/build-p0-core-ref.sh` refuses to write if `.p0ref` (or any of its three output names) is a symlink, a non-directory,
  a foreign-owned directory, a non-regular file or a hard link. It installs files by exclusive temp name + `rename(2)` (#643 M-01).
  Residual risk: a same-user process racing the script between its checks and the rename.
- The L4 write trace (`tests/l4_analyze.py`) resolves relative paths against each traced process's working directory and
  self-tests on a scratch decoy. Its PASS covers only the traced syscalls in the traced process tree for that run.
- P0 is a **binary** reference (never a `ProjectReference`). The project build fails if the DLL's SHA-256 differs from the
  expected value in `src/Core/SitkaP1.Core.csproj`.
- Only P0's public `LabFile.Parse`, `LabFile.Serialize` (tests only) and `CanonicalJson.SnapshotHash` are used.
- The trust table never reads P0's `MANIFEST.sha256`.
- P1 never binds port 5187 and never opens P0's `.lab-data/`.
- Changing a trust or catalog record to `Accepted` is a reviewed source change that cites the independent review and the
  Manager acceptance.
