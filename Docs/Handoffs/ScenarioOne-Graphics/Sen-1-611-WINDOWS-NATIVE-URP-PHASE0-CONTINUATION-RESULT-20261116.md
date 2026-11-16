# Sen-1 #611 - Windows native URP Phase-0 continuation: BLOCKED

2026-11-16. **OPEN / BLOCKED before project launch. Native result: UNQUALIFIED.** The owner-authorised one-off Windows exception is confirmed. Technical admission failed; the native material experiment is not complete.

Authority: [complete continuation handoff](https://github.com/jermur788/CCF/blob/docs/sen-1-611-windows-editor-installed-continuation-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-WINDOWS-UNITY-EDITOR-INSTALLED-LOCAL-PHASE0-CONTINUATION-20261116.md). The [earlier stop report](https://github.com/jermur788/CCF/blob/1ecd3b4e5323f7cf11447dbdd54d8701c77772ab/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-COMPACT-WORNEARTH-PHASE0-UNITY-LOOK-RESULT-20261116.md) is preserved unchanged. This new result supersedes its installation observation for current planning, not its historical accuracy.

## Actual worker and lane

| Item | Verified state |
|---|---|
| Worker | OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), High reasoning, Codex harness in T3 Code |
| Machine | `DESKTOP-DL1BQI7`, Windows 11 Pro |
| Pre-report registration | [#611 continuation BLOCKED comment](https://github.com/jermur788/CCF/issues/611#issuecomment-6921257682), `2026-11-16T15:46:49Z` |
| Inspection checkout | `C:\Users\hp\Documents\GitHub\CCF`, local `main`; starting HEAD `04defad6f28c33156cc986dd0685dcb1998073a4`, clean and preserved |
| Current integration baseline inspected | GitHub/fetched `main` at `60b7960d5ff343d3f84340cff2c689a74d1bc763` |
| Latest context-source commit at that baseline | `7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d`; no Unity execution context admitted |
| Live ownership refresh | #611 and latest #424 comments, GitHub live coordination blob `6380f13bdcd11cd3014902dcff6a1f98c98e70f3`, local registered Git worktrees; no competing #611 execution claim observed |
| Report-only branch | `docs/sen-1-611-windows-continuation-blocked-result-20261116-v1`, new GitHub-only docs lane based on the current inspected integration baseline |
| Experiment worktree/branch | NONE created; proposed Operations experiment lane remains unadmitted |
| Own Library / isolated Editor/Prefs/save configuration | NONE created; custody/isolation not established |
| Physical machine lease | NOT ACQUIRED. No acquire/release timestamp exists, no release needed; FREE is not asserted |
| File allowlist | This one added result Markdown only; no PR, merge or main write |

## Precise blockers

**1. Installed Editor is the wrong version for the packet.** Unity Hub's reliable installed-Editor listing reports exactly one Editor:

`C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Unity.exe`

Version **6000.6.9f1**, x86_64. Executable Windows metadata independently reports ProductVersion `6000.6.9f1_8c45277fb9c5`, FileVersion `6000.6.9.9192743`. The required version is **6000.6.0f1**, revision `f7f8ed4d1e24`, also recorded in the preserved project. Continuation preflight step 2 explicitly says a different Editor version is STOP, rather than silently upgrading settings/packages. Reliable Hub listing was used; the Editor itself was not started even for `-version`.

**2. Actual Windows CCF-Ops admission remains unestablished.** No usable physical host registry/board, globally serialized `unity-lock`, approved Operations worktree root/registered clean lane, or isolated save/Prefs custody protocol was discovered. Read-only root/Documents inspection and searches of accessible `C:\Users\hp` and `C:\ProgramData` for `CCF-Ops`, `lanes.json`, `unity-lock`, `unity-lock.ps1` and storage instructions found only unrelated dashboard `tests/fixtures/ccf-ops` directories. No configured CCF Ops path hints were present in the inspected environment variables. Search encountered 106 inaccessible directories, so this is an inability to establish usable Ops, not proof that no files exist anywhere. Repository `Docs/Operations` contains CloudUnityWorker proof packets, not a deployed Windows board/lock facility.

No compatible physical lock could therefore be queried/acquired, and cross-host/global ownership could not be certified. No experimental worktree, Library or isolated Prefs root was created. Continuation step 4 explicitly prohibits deploying Ops, making a bypass lock, borrowing a lane or substituting an empty project. None was attempted.

Hub's `editors running` returned count **0**. Windows process inspection showed Unity Hub and its licensing client, not an Editor. These observations are not a machine-wide lease FREE receipt. Existing Hub/licensing processes were left alone; no process was killed. `C:` had approximately 104 GiB free; `F:` reported no usable filesystem capacity. Capacity does not resolve either admission blocker.

## URP and source integrity

URP **17.6.0** was verified read-only in:

- This checkout's `Packages/manifest.json` and `Packages/packages-lock.json`.
- The current integration main versions of those files.
- The installed 6000.6.9f1 Editor's bundled `Editor\Data\Resources\PackageManager\BuiltInPackages\com.unity.render-pipelines.universal\package.json`.

This verifies source and installed package metadata only. **Native project import/resolution remains NOT RUN.** Matching URP metadata does not waive the Editor mismatch or Ops requirements. No package/project settings changed.

Read source provenance/replay instructions and independently rehashed the original v2 files without executing texture generation. Source directory:

`C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-v2-20261116\`

**41/41 PASS, zero mismatches**. `SHA256SUMS` SHA-256 remains `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673`, matching #605 and prior preflight. The original procedural authorship and Y+ normal declaration are unchanged; no licence/release approval is inferred.

All selected filenames share prefix `Sen1_Compact_WornEarth_Footpath_Mat01_` in `maps/`:

| Map suffix | SHA-256 |
|---|---|
| `BaseColor.png` | `b0de0b456d9b136e532e305fed528444c9a0c960563635a9f965eaf89f8155c0` |
| `Normal_GL.png` | `d6d21c97e42b68c36b788f888b924b2a0b6f763dcbf2d594bafb1fd4f41c80a2` |
| `Roughness.png` | `36810e66e2df91344d9a0960775fe714eadcdf3606873b185285c74530e2201c` |
| `Height16.png` | `8fcc9dae2783c08cac4c06115c2a6119fb0a680871f04b756fd4b4fb0b084507` |
| `AO_heightderived.png` | `30a06d422710dc279f601b510040ef13a731eb964b6f533a74e3c8c3796887f7` |

Preserved sibling v1 folder verified **31/31**, and original review folder **14/14**, against their original manifests. All three were rechecked for the receipt. No transfer, generator run, Blender work or map modification.

## Evidence, unperformed gates and protection

External receipt: `C:\Users\hp\Documents\CCF-Scratch\Sen-1-611-Windows-Continuation-Preflight-20261116\continuation-preflight-receipt.json`

Receipt SHA-256: `3dc0b7d253c244a84f6c5089adcd32e7a62715bd41ba4a9addd65835d00a3f8a`

It includes UTC, host/model, exact CLI inventory/running commands and results, source hashes, local status/HEAD, URP package metadata, Ops discovery limits and unacquired lease state. It contains no credential. No screenshots exist from this continuation. Earlier software previews are not native URP evidence.

**NOT RUN:** import/sRGB/normal-Y+ convention and inversion control, roughness-to-smoothness conversion, height/AO interpretation, shader/material/plane/camera/light setup, 1 m tile seam/repetition, needle/twig/embossing/cast/roughness judgment, overhead 3x3/player-height/close frames at 1280x720 and 1920x1080, normal-disabled control or native v1/v2 comparison. No performance/memory measurements. Native verdict **UNQUALIFIED**, not visual NEEDS CHANGES or production approval.

Primary Git status remained empty. No project tracked/untracked edits, no imports, no serialized scene/material/meta changes, no `ProjectSettings`/`Packages`/graphics/quality/volume edits, no source scripts or protected documentation changes. No access through gameplay/save controllers, no historical-world edits, no Prefs operation, no save/load/harness or Player run. ForestTest, production assets/shared Forest Ground/Path materials, Legacy40/E80 Rack V2/MFM, frozen Reference Future v1 and historical saves were not touched by this worker. Full machine-wide save/Prefs custody remains NOT ESTABLISHED, not claimed PASS. Hub CLI can write routine logs outside the repository.

No lease/Editor was created, so no release or Editor cleanup is due. No install/deployment/download, force release, borrowed lane, reset/stash/clean or Windows build. Prior stop report and all prior receipts are preserved. The live coordination board is Manager-owned and was not edited.

## Disposition

Keep **#611 OPEN / BLOCKED**. Publish/read back this exact new report and verify GitHub compare is one added Markdown only; the publication receipt is posted to #611. Return evidence to Manager #424 for the experiment-evidence verdict, without claiming completion or self-acceptance. Dashboard/Drive sync **NOT VERIFIED**; Manager operational follow-up remains outstanding.

Resume only when the exact required **6000.6.0f1 Editor** is available (or a separately authorised packet revision explicitly changes that pin) **and** a compatible physical Windows CCF-Ops registry/board/registered isolated lane/Library/Prefs and exclusive machine lease are established. Recheck live ownership and source, then post real [RUNNING] registration before any Unity project launch/import. No repeat art choice or narrow Phase-0 product approval is requested. Linux remains the production development/integration host; this report grants no broader Windows permission.
