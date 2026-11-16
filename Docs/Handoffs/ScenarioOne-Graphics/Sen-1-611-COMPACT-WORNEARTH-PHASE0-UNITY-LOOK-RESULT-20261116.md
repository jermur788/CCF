# Sen-1 #611 — Phase-0 Unity look result: BLOCKED before import

2026-11-16. **Task OPEN / BLOCKED. Native material verdict: UNQUALIFIED.** No Unity material test or native capture occurred. This is the mandatory pre-import stop report under [the owner-approved packet, section 3](https://github.com/jermur788/CCF/blob/docs/sen-1-611-owner-approved-phase0-executable-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-OWNER-APPROVED-LOCAL-ONLY-UNITY-URP-PHASE0-EXECUTION-PACKET-20261116.md), not a completed experiment or visual rejection.

## Worker, ownership and baseline

| Item | Actual state |
|---|---|
| Worker | OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), High reasoning, Codex harness in T3 Code |
| Host | `DESKTOP-DL1BQI7`, Microsoft Windows 11 Pro, `10.0.26300` |
| Registration | [#611 BLOCKED preflight owner](https://github.com/jermur788/CCF/issues/611#issuecomment-6919665817), GitHub timestamp `2026-11-16T14:45:08Z`, before report publication |
| Inspection checkout | `C:\Users\hp\Documents\GitHub\CCF`, local `main`, clean at starting HEAD `04defad6f28c33156cc986dd0685dcb1998073a4`; unchanged |
| Current integration main inspected | `c969df77671db7d6d8f984ff4292144df7e620a0`, fetched and confirmed against live GitHub |
| Latest context-source commit at that baseline | `7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d`; no execution lane/context was admitted |
| Live coordination | GitHub `main:Docs/Project/coordination/active-tasks.md` blob `f686e2b5594189941a17916dafa6305eb7af4d39`; latest #424 and #611 comments read; owner exception confirmed; no competing #611 execution claim observed. Board remains Manager-owned and unchanged by this worker. |
| Report branch | `docs/sen-1-611-phase0-local-preflight-blocked-20261116-v1`, GitHub-only docs lane from the inspected integration main |
| Experiment branch/worktree | NOT CREATED. No approved Windows Operations root established. Proposed execution branch and `CCF-Sen-1-611-CompactWornEarth-URP-Phase0` lane remain unadmitted. |
| Library / Editor configuration / isolated Prefs | NONE created; required custody/isolation protocol not established |
| Machine-wide Unity lease | NOT ACQUIRED. No acquire/release receipt or timestamp exists; machine-wide FREE is NOT asserted. No bypass or force-release. |
| Tracked allowlist | This one added Markdown; no PR, merge or main write |

## Mandatory preflight failure

Windows Unity Hub is installed as `UnityTechnologies.UnityHub` version `4.0.0.65535`. Its CLI is at `C:\Users\hp\AppData\Local\Packages\UnityTechnologies.UnityHub_2vrhnee42bhxm\LocalCache\Local\Unity\bin\unity.exe`. This is the Hub CLI, **not the Unity Editor**.

Read-only commands returned success:

- `unity --json --non-interactive editors --installed` → `data: []`.
- `unity --json --non-interactive install-path --get` → `C:\Program Files\Unity\Hub\Editor`; that directory is absent.
- `unity --json --non-interactive editors running` → `count: 0`, `instances: []`.

Windows process inspection found no Unity Editor. Searches of accessible Program Files, Program Files (x86) and the hp user tree found no Editor or operational CCF-Ops deployment/physical board/`lanes.json`/`unity-lock`. Unrelated dashboard test fixtures named `ccf-ops` are not a real deployment. These searches do not prove a lease FREE or exclude an unregistered installation on an unavailable volume. `C:` was the available storage drive; `F:` reported no filesystem capacity. Approximately 121 GiB free on `C:` does not cure missing Editor/Ops/custody prerequisites.

Repository source requires Unity `6000.6.0f1` (`f7f8ed4d1e24`) and declares/locks URP `17.6.0`. These are configuration facts, **not installed/runtime qualification**. No Library or package import occurred. Windows registry lists Blender `5.3.0` at `C:\Program Files\Blender Foundation\Blender 5.3\`; Blender was not needed, launched or startup-qualified.

**STOP:** packet section 3 requires verified Windows Unity, physical Ops ownership/root/lease and safe isolated save/Prefs custody before launch/import. These could not be established. The existing Phase-0/D-032 owner permission is valid; technical admission is missing. No installation, package update, transfer or substitute workflow was attempted. No fictitious [RUNNING] Unity registration was posted.

## Source integrity — PASS

Read original `PROVENANCE.md`, `REPLAY.md`, manifest, generator normal/roughness/height definitions and published #605 result. Recomputed original manifest entries without running the generator or modifying source.

Source: `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-v2-20261116\`.

`SHA256SUMS` SHA-256: `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673`, matching #605. **41 checked / 41 pass / 0 mismatches**, repeated for final receipt. Source describes original procedural authorship, 1024×1024 maps, 1 m design tile and tangent OpenGL/Y+ normal. Global D-032 licence remains undecided; no reuse/release approval follows from hashes.

All selected names share prefix `Sen1_Compact_WornEarth_Footpath_Mat01_` within `maps/`:

| Map suffix | SHA-256 |
|---|---|
| `BaseColor.png` | `b0de0b456d9b136e532e305fed528444c9a0c960563635a9f965eaf89f8155c0` |
| `Normal_GL.png` | `d6d21c97e42b68c36b788f888b924b2a0b6f763dcbf2d594bafb1fd4f41c80a2` |
| `Roughness.png` | `36810e66e2df91344d9a0960775fe714eadcdf3606873b185285c74530e2201c` |
| `Height16.png` | `8fcc9dae2783c08cac4c06115c2a6119fb0a680871f04b756fd4b4fb0b084507` |
| `AO_heightderived.png` | `30a06d422710dc279f601b510040ef13a731eb964b6f533a74e3c8c3796887f7` |

Preserved sibling v1 (`Sen-1-597-WornEarth-20261116`) also verified **31/31**, and original review folder (`Sen-1-603-WornEarth-Review-20261116`) **14/14**, against their existing manifests. No source/review files changed or transferred.

## External evidence and unperformed gates

Receipt: `C:\Users\hp\Documents\CCF-Scratch\Sen-1-611-Preflight-20261116\preflight-receipt.json`.

Receipt SHA-256: `4e32ca438eebf111beaf8c1e8ee370b68d5077522ee6411caa198d8cfdd8d74b`.

It records UTC, actual host/model, full per-file source hashes, primary checkout status/HEAD, exact CLI commands/results and failed admission. No credential is included. Evidence stays outside Git. No screenshots were generated; #605 software previews were not substituted for native images.

**NOT RUN:** texture importer/sRGB/compression settings, normal Y+ inversion control, roughness-to-smoothness interpretation, height/AO treatment, URP material/shader, camera/light/plane setup, overhead 3×3/oblique/detail captures at 1280×720 and 1920×1080, normal-disabled negative control, native v1/v2 comparison, repetition/needles/grit/cast/seam judgment. Memory/performance NOT MEASURED. The native verdict is **UNQUALIFIED**, not production APPROVED or visually NEEDS CHANGES.

## Protection, disposition and resumption

Primary `git status --porcelain` was empty before and after preflight. No project tracked/untracked edits, including `Assets/`, `.meta`, scenes/materials, `ProjectSettings`, `Packages`, `.vscode/settings.json`, `AGENTS.md` or storage documentation. No Editor, gameplay harness, save controller or Player ran. No import, serialized edit, regeneration, save/Prefs operation, source generation, historical-world access, build, process kill, reset/stash/clean or dependency addition occurred. ForestTest, Forest Ground/Path materials, Legacy40/E80 Rack V2/MFM, frozen Reference Future v1 and historical saves were not touched by this worker. Full machine-wide save/Prefs custody remains UNESTABLISHED and is a launch blocker, not a custody PASS. Hub CLI may write routine diagnostic logs outside the repository.

No lease or Editor process was created; no release/Editor cleanup is due. #611 stays **OPEN / BLOCKED**, not HANDED-OFF/CLOSED as a completed experiment. Final exact one-added-Markdown GitHub compare/readback is recorded in the issue after publication. No acceptance/integration claim. Dashboard ingestion/Drive synchronization **NOT VERIFIED**; Manager operational follow-up remains outstanding.

Resume after Windows Unity 6000.6.0f1 and actual CCF-Ops board/root/lease/save-Prefs isolation are established, with fresh ownership/source checks and genuine [RUNNING] registration before import. Linux routing instead requires separately recorded owner transfer permission and source/destination hash proof under the packet. Existing narrow owner product approval needs no repetition. **No Windows build.**
