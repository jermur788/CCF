# Latest #611: Windows setup completed; native rendering BLOCKED by Smart App Control

**Updated 2026-11-16T16:44:32.927599+00:00.** Original Windows Ops/lease/worktree/profile and Unity licence blockers are resolved for this one owner-authorised task. The native material test remains **UNQUALIFIED: zero rendered captures**, because Windows Smart App Control prevents Unity's script-build system from loading an installed DLL. #611 stays OPEN, pending the next supported remedy. Earlier reports below remain exact historical evidence.

## New direct owner authority and actual work

After #621 feasibility, the owner explicitly selected **"Authorise Windows setup and #611 test"**, replacing its earlier read-only/no-deployment/no-Unity limits for this bounded task. The owner also selected **"Activate Unity Personal; I accept its terms and meet eligibility"**. Official Unity browser login completed under the isolated account; Personal activation returned success/exit 0. No existing hp credentials were copied. The separate permission to use installed **6000.6.9f1** still applies only to #611. Production changes, source asset commits, historical scene/save changes and Windows Player builds remain prohibited.

Actual worker remains OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), High reasoning, Codex harness in T3 Code, `DESKTOP-DL1BQI7`. [Setup authority registration](https://github.com/jermur788/CCF/issues/611#issuecomment-6921895875), before deployment/import. Primary remains `C:\Users\hp\Documents\GitHub\CCF`, local main `04defad6f28c33156cc986dd0685dcb1998073a4`. Exact new task worktree baseline **e994f0ae4e96cff9693180bd8026ed4b25abb96d**, branch `experiment/sen-1-611-compact-wornearth-urp-phase0-20261116-v1`. No task-branch commits or source repinning.

## Windows admission is now real, task-local and tested

Root: `C:\Users\Public\Documents\CCF-Phase0-611-Windows\` (abbreviated **R** below). Restricted ACL grants hp, SYSTEM, Administrators and the task account appropriate access; `R\Private` excludes the task account. No Linux Ops/store/lease files were changed. This is a narrow owner-authorised Windows facility, **not permanent shared/canonical Ops acceptance or independent G4 approval**.

- Physical board `R\Ops\board\active.md`, registry `R\Ops\lanes.json`, native tool `R\Ops\windows_ops.py`, fixed guard/lease/history `R\Ops\state\unity\`. A Windows byte-range file lock serializes operations; complete metadata is atomically replaced. Records persist across command exit; no automatic stale release or operational force-release command exists.
- Native Windows process enumeration covers `Unity.exe`, with PID creation identity. Admission checks host, NTFS volume serial **2016364452**, configured storage capacity, actual branch/full HEAD, clean registered worktree, exact Git common store, duplicate writable checkout, isolated OS profile registration, source-tool hash and owned Library path. Refusals include foreign Editor/lease/owner/task/token, malformed record, wrong host/volume/store/head/branch/profile, dirty lane and low space.
- Qualification: **35 PASS / 0 FAIL**, including four concurrent native processes with exactly one successful lease winner, normal matched release and explicit free post-check; separate-preview/junction escape and shared Library rejection. Fixtures are under `R\Tests`, not production lease state. No thresholds weakened. No independent reviewer rerun is claimed.
- Registered worktree: `R\Worktrees\CCF-Sen-1-611-CompactWornEarth-URP-Phase0`, canonical Windows Git common store `C:\Users\hp\Documents\GitHub\CCF\.git`. Its normal Git registration is verified; primary content is untouched. Only precise new local scratch exclusions were appended to the common `info/exclude`, with before-image retained privately. No tracked .gitignore or source changes.
- New **standard**, non-administrator account `CCFPhase0611`, SID ending **1002**, profile `C:\Users\CCFPhase0611`. Separate SID/HKCU/profile/AppData were proved by an actual private Registry create/read/remove probe. Its generated password is DPAPI-protected for the hp controller, never plaintext/logged. Account/profile are retained enabled for the pending authorised continuation; no account deletion or licence return was performed. Authentication/private credential logs are excluded from Git and published evidence inventories.
- The account helper's first attempt failed before creation on Windows' 48-character description limit; a second created only the task account but direct CreateProfile failed (Win32 1399). Both failures are preserved. Standard first logon with LOGON_WITH_PROFILE then created the correct independent profile successfully. Only the exact newly created SID was repaired; no pre-existing account changed.

## Actual lease/Editor attempts and failures

[First genuine RUNNING registration](https://github.com/jermur788/CCF/issues/611#issuecomment-6922238184) preceded scratch staging/import. Source **41/41 hashes** were rechecked; the five copied maps match their original hashes. NTFS free space at admission was about 103 GiB; operational admission floor 30 GiB, safe-stop threshold 20 GiB, hard minimum 15 GiB. No low-space threshold was approached. No performance claim follows from these storage observations.

| Attempt | Genuine acquisition / owned Editor | Actual outcome |
|---|---|---|
| First account licence check | `2026-11-16T16:24:26.807114Z`, lease `1df513ebdd8b4802bf116056a88f9e8e`, Editor PID 10336 | Exit **198**, no valid licence, zero captures. Normal matched release. Subsequent official login and owner-authorised Personal activation succeeded. |
| Initial full-checkout start | [RUNNING receipt](https://github.com/jermur788/CCF/issues/611#issuecomment-6922433106), Editor PID 16180 | The newer Editor automatically changed navigation/collab/Rider/Input System/Visual Scripting dependency versions in this isolated checkout. The worker aborted its exact owned PID after normal window-close was unavailable; this was a forced **owned-process** stop, not another owner's process. No gameplay/Play Mode ran. Exact two-file package diff and changed copies preserved, then only these attributed package files restored from pre-run byte backups. All 5,862 tracked files subsequently match initial hashes. Lease released normally after Editor exit. |
| Prelaunch path check | [RUNNING receipt](https://github.com/jermur788/CCF/issues/611#issuecomment-6922596844) | A supervisor relative-path check rejected the short junction alias before Editor launch. Corrected by resolving both paths; no native process/import occurred, lease released. This failure is preserved, not relabelled PASS. |
| Separate preview start | [RUNNING receipt](https://github.com/jermur788/CCF/issues/611#issuecomment-6922604001), Editor PID 1612 | URP **17.6.0** successfully resolved. Script-build program failed on Windows Application Control; Editor exited **1**, zero captures, normal lease release. |

Full exact acquire/attach/release metadata and UTCs are in `R\Evidence\lease-*.json` and the preserved attempt receipts. Controllers were actual live supervisor PIDs, verified by process creation time; owned Editor PIDs were attached under the guard. Release history remains in the permanent operational state. Final direct tool status: **free, metadata null, no native Editors**. This final free status is observed through the implemented native tool, not inferred from an absent lock.

## Why a separate preview project was used

The dependency-upgrade attempt was stopped to preserve the CCF package graph. A wholly local project now lives at `R\Worktrees\CCF-Sen-1-611-CompactWornEarth-URP-Phase0\LocalPreview611`, inside the registered worktree. `R\Preview` is an explicitly checked short NTFS junction to that same project, used to avoid Windows path-length issues; its own Library resolves inside the owned project. It is not another lane's cache or a substitute for the lease. The native guard checks the resolved path and rejects an escaping/shared Library. Its initial generated Library is retained; no Library was copied.

The preview manifest has URP **17.6.0**, built-in image conversion and physics modules only; normal transitive resolution registered the required URP/core/Shader Graph/tool packages. Source quality/graphics/project settings and the existing first-party `Assets/Settings` assets/metas were copied byte-for-byte as the rendering baseline; originals were never edited. No ForestTest, world scene, gameplay script, save controller or historical profile is in this preview. Its ProjectVersion is 6000.6.9f1 to avoid upgrading the production graph. No new dependency was added to tracked CCF Packages.

This design can answer the bounded material-look question under the copied render settings. It cannot qualify full CCF integration/runtime/performance. The renderer script asserts actual imported URP 17.6.0 and existing URP quality asset, uses a disposable empty Editor scene, and never enters Play Mode. It was prepared but **did not execute**, due to the earlier compiler/tool failure.

Prepared controlled test: exact five v2 maps; sRGB base colour, linear normal/roughness/height/AO, normal importer Y+ with green flip false, Repeat/trilinear/mipmaps/uncompressed 1024; in-memory metallic 0/smoothness alpha = 1 minus roughness, AO strength 1, normal scale 1 and parallax scale 0.006. White sun intensity 1 at (50,-35,0), flat neutral ambient 0.25, no postprocessing/HDR/MSAA. Three modes (normal/height disabled, v2 active, deliberately green-flipped negative control), three views (3x3 overhead, 1.7m oblique, closer detail), two sizes 1280x720 and 1920x1080: intended 18 native images. **None exist yet; these are prepared settings, not observed material results.**

## Precise current Windows blocker

Unity's own build log reports:

`System.IO.FileLoadException: Could not load file or assembly '...\Editor\Data\Tools\BuildPipeline\Bee.Stevedore.Program.dll'. An Application Control policy has blocked this file. (0x800711C7)`

Installed file: `C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Data\Tools\BuildPipeline\Bee.Stevedore.Program.dll`, 125,952 bytes, SHA-256 **379f5d3265c951fb06b2a04f1a544a1d8aa7e60345d31d00394de7f1f71f5a47**. It has no Authenticode signature and no Zone.Identifier stream. The main Unity.exe has a valid Unity Technologies SF signature. No corrupt-file conclusion is established by the DLL's unsigned status.

Windows Code Integrity Operational events **3033/3077/3118** corroborate the denial under policy `{0283ac0f-fff1-49ae-ada1-8a933130cad6}` while Unity's installed `DotNetSdk\dotnet.exe` loads the DLL. Windows reports **SmartAppControlState On**. No policy, signature, trust/reputation override, Unblock-File action or security disable was attempted.

[Microsoft's Smart App Control guidance](https://support.microsoft.com/en-us/windows/what-is-smart-app-control-285ea03d-fa88-4d56-882e-6698afdb7003) states: **"There is currently no way to bypass Smart App Control protection for individual apps."** Changing this machine-wide security protection is outside the approved minimal Ops setup; the worker has not done so.

A supported alternative was researched, **not installed**: the original packet's Unity **6000.6.0f1**, official signed-installer route `https://download.unity3d.com/download_unity/f7f8ed4d1e24/Windows64EditorInstaller/UnitySetup64-6000.6.0f1.exe`. Unity's release API reports **3,858,564,328 bytes download** and **9,052,216,209 bytes installed**, before any optional modules. No Visual Studio/build modules are proposed. An owner question is pending about trying this version alongside the current Editor with Windows protection unchanged. It may also be blocked; no promise of success. No download/install occurred for this alternative. Further security-policy changes require separate explicit authority; a later authorised Linux route remains another option, with its own host/transfer gates.

## Protection and reproducible evidence

At `2026-11-16T16:41:55.488048Z`, a complete SHA-256 comparison of **5,862 tracked worktree files** found **zero changed files**. Primary and experiment `git status --porcelain` both empty. hp's existing Unity Editor preference-tree digest matches its pre-run value; CCF EditorPrefs key remains absent and hp's CCF save directory remains absent. Actual Unity runs used only the distinct task OS account. No saved world/annual simulation, historical scene/asset, production material, frozen Reference Future, binary source commit or Windows Player build occurred.

Task-local imports/tooling/source/evidence remain untracked outside production history. The continuation Markdown is the only repository content write. No production branch/main merge or asset commit. Temporary account and caches remain documented for continuation; no unsolicited cleanup/deletion of assets or worktrees.

Evidence inventory: `R\Evidence\setup-and-stop-evidence-sha256.json`, SHA-256 **3b8af0708da40f758da5adbb024eb35073da2819d2ff764de5a42f88826466f1**. It hashes non-secret source/tools/qualification/lease/stop/protection logs; credential/auth console data is excluded. Especially useful files:

- `ops-qualification.json` (35 PASS/0 FAIL), `profile-custody.json`, `deployment-manifest.json`, `source-hashes-before.json`.
- `running-registration.json`, acquire/attach/release JSONs and permanent lease history, plus preserved earlier attempt receipts.
- `unity-first-license-failure.log`, `isolated-package-upgrade.diff`, `owned-run-stop.json`, `preview-project-comparability.json`, `preview-alias.json`.
- `unity-smart-app-control-failure.log`, `smart-app-control-events.json`, `native-smart-app-control-failure.json`, `protection-checkpoint.json`.

All native visual/performance acceptance remains NOT RUN. The material is **UNQUALIFIED**, not visually rejected or approved. Owner-approved Windows Ops setup is worker-tested, not independent permanent Ops acceptance. Dashboard/Drive NOT VERIFIED; Manager #424 operational synchronization follow-up remains outstanding. Keep #611 OPEN; no completed-native-experiment closure or automatic further installation/security change.

---

# Latest #611 status: Editor exception accepted; Ops admission still BLOCKED

**Update UTC: 2026-11-16T15:54:30.353775+00:00.** The owner directly instructed the local worker to **"continue with the editor"**, following the explicit proposal to use installed **6000.6.9f1** for this disposable material preview. This clears the Editor-version blocker **for #611 only**. Existing production project pins, Linux production-host policy, global D-032 and asset/save/build exclusions remain unchanged. No further Editor-version permission is required for this experiment.

Actual worker remains OpenAI GPT-6.1 Sol High on DESKTOP-DL1BQI7. [Pre-edit exception/recheck registration](https://github.com/jermur788/CCF/issues/611#issuecomment-6921458803). Current integration main inspected at `a297a3d0579284ac78d169c00482035ec1049d90`, live coordination blob `957b140f402f7e3fec11b4ff82ab31792a518162`. Primary checkout remains clean at `04defad6f28c33156cc986dd0685dcb1998073a4`. Existing result-only branch/base remain unchanged; no execution lane repinned or created.

**Remaining blocker: no usable physical CCF-Ops board/registry, registered approved isolated worktree/Library/Prefs custody or machine-wide Unity lease has been established.** Rechecked Windows root/Documents/configuration/command paths and searched accessible hp user/ProgramData directories (106 inaccessible directories); only unrelated dashboard ccf-ops test fixtures were found. Also inspected the already-running WSL Ubuntu read-only under `/home`, `/opt`, `/media`, `/srv`, `/usr/local/bin`: only unrelated fixtures; `unity-lock` absent from PATH. WSL discovery is not proof of remote Linux availability or global lease status. No compatible board/lock facility was found from which to acquire the required lease. FREE is not asserted.

Continuation preflight step 4 explicitly says **"Do not install or deploy Ops tooling, create a bypass lock, force-release a lock or borrow another owner lane."** It also forbids an empty project as a substitute for the ownership/lease safeguard. The owner's Editor exception resolves the version question; these independent safety requirements still apply. No Ops deployment, substitute lock or borrowed lane was attempted.

Fresh v2 source check: **41/41 PASS**, manifest SHA-256 `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673`. Hub reports installed Editor 6000.6.9f1 and 0 running Editors; URP 17.6.0 metadata remains the previously verified version. No Editor project launch, native import/captures, save/Prefs operation or build. No experiment worktree/Library/lease created; no worker Editor cleanup or lease release is due. Native verdict **UNQUALIFIED**. All earlier unperformed gates remain unperformed.

External recheck receipt: `C:\Users\hp\Documents\CCF-Scratch\Sen-1-611-Windows-Continuation-Preflight-20261116\editor-exception-ops-recheck.json`. SHA-256 `19ae2bc716b8175755c961e854b82b8a354d0cad48428026153432b52ab32d3e`. No credentials. GitHub publication/readback/compare receipt is posted to #611 after this update. Live board remains Manager-owned; dashboard/Drive NOT VERIFIED.

**#611 remains OPEN / BLOCKED ON OPS ONLY.** Resume after the actual compatible Ops root/board, isolated lane/custody and exclusive lease are established; use the now-approved 6000.6.9f1 for the bounded experiment. This report does not authorise deploying Ops or waiving its safeguards. Earlier report is preserved verbatim below as historical evidence; its Editor-version stop is superseded by this dated owner exception.

---

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
