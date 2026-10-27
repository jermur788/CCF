# S1-B Linux Player gate (Tier-3 tooling)

Separate, bounded Linux Standalone (Mono) build + smoke path for the Tier-3 Scenario One
Player gate. It does **not** modify or replace the accepted Windows candidate path
(`WindowsCandidateBuilder.cs`, `build_windows.py`, `README.md`) and makes **no**
distribution or tester-package claim.

## Files

| File | Role |
|---|---|
| `LinuxPlayerBuilder.cs` | Disposable `-executeMethod LinuxPlayerBuilder.Build`; `StandaloneLinux64`, Mono2x, single enabled scene `ForestTest.unity`, same privacy/analytics checks and release-content filter as the Windows builder; emits `S1B_LINUX_BUILD_PASS` |
| `build_linux.py` | Orchestrator: clean-source guard, exact HEAD, `--unity`, date/sequence build id, ZX20 output, batchmode, timeout/error handling, Linux layout validation, `build-result.txt`/`build.log`, `package-manifest.json`, optional `--smoke` staging |
| `run_linux_player_smoke.py` | Launches the built Player twice (SAVE, then LOAD) and records wall-clock, timeout status, process exit and a bounded fatal/exception scan |
| `art_pack.py` | Verifies (and installs only the required files of) the locally acquired Asset Store art pack; regenerates its manifest |
| `art-pack-manifest.json` | Committed identity of the required pack files (sha256, Unity GUID, Asset Store product/version/upload, whole-pack content checksum); contains no pack content and no machine path |
| `test_build_linux.py` | Unity-free bounded tests (mock project/Unity/Player) under ZX20/CCF-Temp |

Reuses the existing `Tools/Verification/S1B/StandalonePlayerSmoke.cs` unchanged (staged by `--smoke`).

## Local art pack (required, never committed)

The shipped scene references prefabs from the Asset Store pack **Ultimate Nature – Starter** (Innerverse
Interactive). It is a locally acquired dependency (see `Assets/ForestPrototype/Docs/UltimateNatureSetup.md`):
gitignored, never committed, never redistributed. Without it the scene has missing prefab instances and the
build reports errors. `build_linux.py` therefore verifies it **before** anything is staged:

- give its folder with `--art-pack DIR` (the `InnerverseInteractive` folder, or its parent such as another
  checkout's `Assets/`) or `CCF_ART_PACK_DIR`; only the required files and their `.meta` are installed into the
  gitignored `Assets/InnerverseInteractive` of the build project, or an already installed pack is verified in place;
- a missing or different pack (any required file absent, different bytes, or different Unity GUID) stops the
  build with the reason; the manifest also records the whole-pack content checksum (a difference there is only a
  warning: the build consumes the required files);
- `build-identity.json`, `package-manifest.json` (and the Player's identity stamp) record the verified pack
  (product id, version, upload id, required-set and whole-content checksums) and the tooling source hashes.

`python3 Tools/Build/S1B/art_pack.py verify --pack DIR` checks a pack alone; `... generate --project . --pack DIR --check`
tells a maintainer when the project's own references no longer match the committed manifest.

## Build

```sh
source /media/jer/ZX20/CCF-Ops/environment.sh
# The Unity licensing client needs a socket-capable TMPDIR (NOT the exFAT ZX20 drive; the build stops early if it
# cannot create a Unix socket there) and a currently valid licence (--license-dir; the default ~/.config copy can be
# stale and Unity then exits 198 "No valid Unity Editor license found").
export TMPDIR=/run/user/$(id -u)/ccf-build; mkdir -p "$TMPDIR"
python3 Tools/Build/S1B/build_linux.py --smoke \
  --unity /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity \
  --license-dir <Hub-managed Unity licences folder> \
  --art-pack <folder of the local Ultimate Nature - Starter pack> \
  --date 20261016 --sequence L01      # sequence must be L<nn> or S<nn>
# -> /media/jer/ZX20/CCF-Builds/S1B/CCF-S1-20261016-L01-<sha7>[-SMOKE]-Linux-x64/ (expects CCF.x86_64)
```

Installed Linux Standalone Support (Mono) is required and already present on this host
(`.../PlaybackEngines/LinuxStandaloneSupport/Variations/linux64_player_*_mono`). No new
package/module is installed by this tooling.

## Smoke (needs a DISPLAY)

```sh
python3 Tools/Build/S1B/run_linux_player_smoke.py \
  --player /media/jer/ZX20/CCF-Builds/S1B/<build-id>-Linux-x64/CCF.x86_64 \
  --display "${DISPLAY:-:0}"
# evidence -> /media/jer/ZX20/CCF-Verification/ScenarioOneLinuxPlayerSmoke-<timestamp>/
```

The two launches share one isolated `XDG_CONFIG_HOME` (so the save is isolated and shared),
record `player-save.log`/`player-load.log`, and require the smoke markers
`S1B_PLAYER_SAVE_PHASE_PASS` and `S1B_PLAYER_LOAD_RESET_PHASE_PASS` with no fatal lines.

## Scope limits

- Bounded: no FPS benchmark framework, no thresholds beyond a ~600 s smoke timeout.
- Native F10 stays a later native Windows manual/Player check.
- Same-build-only save (no cross-platform save claim).
- Builds, caches and evidence stay on ZX20; never the main SSD.

## Test

```sh
python3 Tools/Build/S1B/test_build_linux.py   # 47 Unity-free checks
```
