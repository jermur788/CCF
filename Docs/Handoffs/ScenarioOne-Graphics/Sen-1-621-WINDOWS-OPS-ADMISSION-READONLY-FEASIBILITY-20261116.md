# Sen-1 #621 - Windows CCF-Ops admission: read-only feasibility

**Result: investigation complete; Windows execution NOT ADMITTED. Recommend Option B: defer #611 to a later authorised Linux material-inspection slot.** Option A is technically plausible but requires a separately scoped Windows operations port, review and isolation setup. No sanctioned ready-to-use Windows installer/deployment was found. Installing the Editor does not supply those facilities.

This is a feasibility handoff for [#621](https://github.com/jermur788/CCF/issues/621), not deployment approval, a Unity qualification or a new #611 attempt. #611 remains OPEN/BLOCKED, native material UNQUALIFIED. Manager #424 decides whether Windows setup is worth its cost. All production Scenario One development/integration remains Linux-only; the owner-approved Windows exception covers #611 scratch inspection only.

## Registration and scope

| Item | Actual value |
|---|---|
| Worker | OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), High reasoning; Codex harness in T3 Code |
| Physical machine | `DESKTOP-DL1BQI7`, Windows 11 Pro `10.0.26300` |
| Registration before report edits | [#621 RUNNING comment](https://github.com/jermur788/CCF/issues/621#issuecomment-6921611746), `2026-11-16T16:00:21Z` |
| Exclusive documentation branch | `docs/sen-1-621-windows-ops-admission-feasibility-20261116-v1` |
| Starting documentation HEAD | `e994f0ae4e96cff9693180bd8026ed4b25abb96d`, fresh inspected integration main at claim start; no repin |
| Physical inspection checkout | `C:\Users\hp\Documents\GitHub\CCF`, local main `04defad6f28c33156cc986dd0685dcb1998073a4`, clean before/after; no local task worktree |
| Live coordination at start | GitHub blob `412feed167a76e044b0338e1dcf91af773986b32`; #621/#611/latest #424 refreshed, no competing #621/#611 execution owner observed |
| Sole writable output | This one GitHub Markdown; no main/board/registry/production writes |
| Unity lease / Editor launch | NONE requested/acquired/launched; no operational lock commands or self-tests executed |

Physical investigation was read-only. Git source refs were fetched for inspection and the report branch was created through GitHub; no game checkout was switched or created. No software/asset downloads or installs, production edits, account/credential changes, transfer, Registry/Prefs/save writes, Ops deployment, process interference or build.

## What is physically available

| Prerequisite | Observation | Admission conclusion |
|---|---|---|
| Windows Editor | Existing `C:\Program Files\Unity\Hub\Editor\6000.6.9f1\Editor\Unity.exe`; ProductVersion `6000.6.9f1_8c45277fb9c5` | Available; owner's #611-only version exception already clears the version gate |
| URP | Prior #611 source/lock/bundled metadata verification: 17.6.0; relevant package files unchanged between inspection checkout and claim baseline | Metadata known; native import NOT RUN |
| Source textures | Prior [#611 evidence](https://github.com/jermur788/CCF/blob/e3f68780a7c138aa49607b4f34aee8da381bc2f1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-WINDOWS-NATIVE-URP-PHASE0-CONTINUATION-RESULT-20261116.md): 41/41 v2 hashes pass, manifest `ecc97ca6580c5477eab23cc706464c80316da9d97d67e783a0f53cbe53615673` | Referenced as #621 permits; not regenerated, transferred or rehashed in this investigation |
| Physical board/registry | No operational Windows `CCF-Ops`, `lanes.json` or `unity-lock` located; no Ops command/path hint configured | NOT ESTABLISHED |
| Git worktree registration | Windows primary common Git directory is its own `.git`; `git worktree list --porcelain` shows primary plus existing artist/reviewer docs worktrees, none a #611 Ops lane | Git registration is not Ops admission; existing paths are not available for borrowing |
| Own Unity Library | No #611 experiment worktree/Library exists | NOT ESTABLISHED; do not copy/share a warm Library |
| Prefs/save isolation | No independent experiment user/profile/custody arrangement exists | NOT ESTABLISHED; another checkout alone is insufficient |
| Processes / lease | Windows process query shows Hub PIDs 6832/2416 and licensing client PID 960; no Editor observed | Observation only; no physical lease receipt, FREE not asserted; those processes were left untouched |
| Capacity | `C:` 111,923,802,112 free bytes (~104.24 GiB); `F:` no usable capacity returned. 16,600,360 KiB visible RAM, 4,570,928 KiB free at snapshot | Disk seems plausible for one isolated import; RAM headroom is limited. Neither measurement qualifies a future run; Windows floors/forecast need approval |
| Licence | Licensing client exists; no account/serial inspection or isolated-user activation test | Licence entitlement and operation under a new isolated account UNKNOWN |

Windows discovery inspected root/Documents/configuration/command paths, then accessible hp user and ProgramData trees for `CCF-Ops`, `lanes.json`, `unity-lock`, `unity-lock.ps1`, and `StorageArchitecture.md`. Cache/Git/node_modules/Library/Temp and agent-state trees were excluded. 106 directories were inaccessible. Hits were only dashboard `project-overview-dashboard/tests/fixtures/ccf-ops`, whose board contains example ALPHA/BETA tasks. They are test data, not the CCF physical board. This is a bounded negative search, not proof no installation exists anywhere or a licence to create one.

The running local WSL Ubuntu reports the same machine hostname and kernel `6.18.58.1-microsoft-standard-WSL2`; it is not the separately referenced Linux laptop. Read-only searches under `/home`, `/opt`, `/media`, `/srv`, `/usr/local/bin` found only dashboard fixtures (zero access errors, bounded depth/exclusions). `/media` is empty. All inspected canonical paths are absent: `/media/jer/ZX20/CCF-Ops`, `/media/jer/Files/CCF/Repository.git`, `/media/jer/Files/CCF/Operations`, `/media/jer/ZX20/CCF-Repository.git`, `/media/jer/Files/CCF-Primary`, `/home/jer/CCF/.git`. `unity-lock` is absent from WSL PATH. `/mnt/c` is a WSL mount, not the historical ZX20 device.

No remote Linux device was accessed. Current remote Editor/process/licence/disk/board/lease/owner state remains UNKNOWN. Historical Linux FREE receipts are not reused as current authority.

## The operational source exists, but it is Linux-specific

Important distinction: CCF-Ops source is not on this current integration-main tree. It is versioned on operations branches. The inspection found:

- Foundation reference `ops/workflow-foundation-v1` at `a29c2fa480f1077e12434dcf6679f2f5d9465cdc`.
- Later inspected candidate `ops/unity-lease-persistent-identity-20261115-v1` at `343ae9909b223b105f962a39058df1e9a544323a`.
- Historical deployed path in source/provenance: `/media/jer/ZX20/CCF-Ops`; migrated canonical store `/media/jer/Files/CCF/Repository.git`.

Read the Ops entry instructions, README, Unity Operator role, authority-transition note, storage architecture, source registry/environment/lease/helper and deployment provenance. These are source/historical reference, not current physical deployment receipts. The later [durability report](https://github.com/jermur788/CCF/blob/343ae9909b223b105f962a39058df1e9a544323a/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-OPS-LEASE-PERSISTENT-IDENTITY-DURABILITY-RESULT-20261115.md) explicitly retains a failing inherited self-test and does not claim full deployment/integration. Do not choose that branch as an accepted Windows installer or blanket-copy its board/registry/state.

Specific source findings at that exact candidate:

- [`unity-lock`](https://github.com/jermur788/CCF/blob/343ae9909b223b105f962a39058df1e9a544323a/CCF-Ops/scripts/unity-lock#L8) imports `fcntl`; uses a permanent `flock` guard, Linux `/proc` PID/start/boot identity and Editor executable basename `Unity`. State is fixed beneath the verified Linux root. Native Windows `Unity.exe` ownership is not established by this implementation. Simply running it in WSL would not make it a Windows-wide lease.
- [`_ccf.py`](https://github.com/jermur788/CCF/blob/343ae9909b223b105f962a39058df1e9a544323a/CCF-Ops/scripts/_ccf.py#L74) uses `findmnt`, `lsblk`, Linux block-device identity and ZX20 UUID/fstype/root validation. Windows drive letters and NTFS are not that storage identity. Git path/store/lane registration and duplicate writable-branch checks are also enforced; adding a JSON row does not make an arbitrary clone eligible.
- [`README`](https://github.com/jermur788/CCF/blob/343ae9909b223b105f962a39058df1e9a544323a/CCF-Ops/README.md) describes an explicit reviewed-file copy to the existing Linux deployment, with provenance and preservation of board/ledger/state. It provides no sanctioned Windows installer. Existing script/environment inventory and targeted Windows/PowerShell/installer search revealed no supported Windows route.
- The source board is a snapshot. A live row does not override GitHub active-tasks; its designated Manager must reconcile the two. Linux live lane registrations and lock records must never be copied to create a new Windows authority.
- Even `unity-lock status` can enter `guarded(state)` and open/create its guard when state exists. No operational status command was run under this strict read-only task. Source was inspected as text, never imported/executed.

Source SHA-256 anchors: `scripts/unity-lock` = `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5`; `scripts/_ccf.py` = `1295def55a4821ccd00423f65ab58e64a3be916dc18b127ed99ae1099c3e7677`; `environment.sh` = `0dde119a47f668d03f93445cc606a054465bf6eea49155dc0f71eb2eb41d89d4`. These identify inspected source, not installed Windows software.

## Save and Prefs custody must be separate from Library custody

Project settings use `DefaultCompany` / `CCF`. Source at the inspection HEAD has `forest-save.json` under `Application.persistentDataPath`, with `.tmp`/`.bak`, in `ForestSaveController.cs:15`; session-menu code independently constructs the same location. These source files and the relevant project/package settings are byte-identical at the claim baseline. UI code writes PlayerPrefs. The installed offline Unity PlayerPrefs documentation explicitly puts Windows Editor Play prefs under `HKCU\Software\Unity\UnityEditor\<company>\<product>`; standalone prefs use `HKCU\Software\<company>\<product>`.

Read-only existence probes found no current `C:\Users\hp\AppData\LocalLow\DefaultCompany\CCF` directory and no corresponding CCF Editor/standalone Registry key. The broader `HKCU\Software\Unity Technologies\Unity Editor 5.x` key exists; its values were not read/exported/changed. Absence of these particular CCF locations does not prove all user saves/prefs absent or safe to share.

A new worktree and own Library would still run under the same Windows user Registry/profile. `TMP`/`TEMP` or Linux `XDG_CONFIG_HOME` are not a demonstrated Windows HKCU isolation method. No unverified Unity command-line override is proposed. A controlled Windows route should use a dedicated OS user/profile (or separately approved equivalent proven isolation), keeping company/product/source settings unchanged. That account may need owner-controlled Unity licence login; no credentials are requested or stored in this report.

## Option A - later controlled Windows Ops admission

**Feasibility: conditional engineering/setup work; not ready to execute.** No existing sanctioned installer can be named from the evidence. The safe route is an explicitly approved Windows port/admission packet, followed by independent review and negative tests, rather than copying Linux scripts or creating a private substitute lease. This is more work than registering a worktree.

Proposed future operator sequence, none executed:

1. Manager/owner explicitly authorise the Windows Ops port/setup scope and one-time Windows storage/profile policy exception. Name the Ops writer, source/review commit, machine, new root, canonical Windows Git store, exclusive lane, capacity floors, profile/credential operator, exact files and rollback before writes. Reconcile all current Windows/WSL callers and remote Linux branch owners. Do not relocate/change the Linux store or lease.
2. Develop/review a host-aware admission mechanism preserving fail-closed semantics: Windows volume identity/free-space checks; native Windows Editor PID plus creation identity; permanent guard/atomic complete records; stale/unknown/malformed record handling; owner/task/lock-id release; branch/path/store validation including case/junction/WSL aliases. All cooperating Windows and WSL launchers on this physical PC must use the same native host authority. A remote Linux machine keeps its own machine lock; Manager coordination prevents cross-host writable-branch conflicts. A copied Linux lock record is not mutual exclusion.
3. Qualify normal acquire/reject/release, concurrent acquisitions, foreign/unknown PID, malformed/stale lock, wrong volume/root/store/lane/head, duplicate checkout, dirty lane and low-capacity refusals using disposable fixtures only. No production lock weakening, automatic force-release or process-kill logic. Review incomplete inherited tests before relying on them. This test/deployment work requires its own authorisation.
4. After acceptance, use an explicit reviewed per-file deployment manifest and hash checks. No general installer/download is currently prescribed. Create the approved dedicated Windows account/profile with restricted access and owner-operated licence setup as needed; do not reuse hp's EditorPrefs/save namespace. Stage the sanctioned Windows Ops deployment, create its approved store and a unique clean #611 worktree, reconcile Manager registration, and leave Library empty for its own cold import. Do not copy asset sources under the Ops setup task.
5. Verify deployment/board/store registration/profile/drive health and native process inventory. Setup qualification and its normal lease self-test must end released, with exact receipts. Only a later #611 admission can acquire its actual execution lease and import the five authorised maps. No automatic Editor start follows setup.

**Proposed bounded file allowlist for a future setup packet, not permission now:** reserve a new root such as `R = C:\CCF-Phase0-621`, subject to owner approval, verified NTFS volume and restricted ACLs. Exact account/profile and final absolute root must be pinned in that packet; this path is not registered or created.

- `R\Ops\AGENTS.md`, `README.md`, `DeploymentProvenance.md`, `deployment.json`, `lanes.json`; approved Windows environment/launcher file only when its name/hash is reviewed.
- `R\Ops\scripts\_ccf.py`, `unity-lock`, `lane-status`, `packet-lint`, `verify-deployment`, plus only explicitly reviewed Windows support files/tests listed by hash in the setup manifest. Existing source is not already Windows-compatible.
- `R\Ops\board\active.md` (Manager-approved Windows task row), `R\Ops\ledger\621-setup.md`; `R\Ops\state\unity\` only through the reviewed authoritative tool for guard/record/history lifecycle, never hand-created fake ownership.
- New `R\Repository.git\` and `R\Worktrees\CCF-Sen-1-611-CompactWornEarth-URP-Phase0\`, generated only by authorised Git registration; own generated Library/Temp/Logs remain in that worktree. `R\Verification\` and `R\Temp\` only for setup evidence/tests.
- Only the dedicated account's new profile/Registry/Unity licence locations explicitly enumerated after Windows assigns the real profile path, with owner credential custody. No hp profile/Registry changes, Linux live files, existing worktrees, shared caches or project content changes. The account name/path/ACL/licence effects are unresolved until approved; do not pretend this is already a fully specified executable installer.

**Rollback proposal:** before deployment capture source hashes, exact touched-file before-images, ACL/account ownership and live process/lease state. On failure, stop only owned setup activity, release only its matching normal lease if acquired, disable the new launcher and quarantine its root with receipts. Restore only exact files this setup changed when no concurrent owner exists; never restore a stale shared board/lock snapshot or delete guard/history. Preserve new Git refs and evidence; removing an account/profile/worktree/imported assets requires the future packet's explicit itemised cleanup permission. Existing Linux Ops and hp's account remain untouched, providing the original operating baseline. No rollback was performed here.

**Additional authority required:** named Windows Ops implementation/deployment and reviewed installer/file manifest; new Windows storage/registry/lane policy; dedicated account/ACL/licence action; metadata-only lease/concurrency tests; rollback/cleanup scope. Current #611 art and Editor exceptions do not grant those operations. No setup secrets are presently needed for this feasibility result.

## Option B - defer #611 to a later authorised Linux art slot

**Recommended for this one material question.** Preserve the Windows v2 source as-is and leave #611 blocked/deferred. Core-first gameplay/spec work continues independently. Schedule the native material test when a real Linux Ops lane is already being admitted for appropriate art/preview work, paying for one bounded test rather than a Windows operations port. This does not claim that production graphics/art integration is approved now.

Before that future test: verify the actual Linux machine/Editor/URP/licence, current live board/tool provenance, canonical Git store/branch ownership, isolated clean lane/own Library/save-Prefs custody, sufficient storage and a real acquired machine lease. Obtain explicit owner permission for **Windows-to-Linux transfer** of the exact source manifest/maps to a named new destination, with source/destination SHA-256 proof; no shared-folder or permission assumption. State the Linux Editor/version and rendering comparability separately; the Windows 6000.6.9f1 exception is not a general Linux version change. Run only the bounded #611 native plane/normal/tiling/capture task after refreshed authority; later art integration remains independently gated.

Neither transfer permission nor live Linux readiness was established here. No transfer or remote host action occurred. Option B avoids creating Windows operational infrastructure for a one-off test while retaining the same provenance/native-evidence gates.

## Outcome and verification

**#621 may close as HANDED-OFF FEASIBILITY ONLY. #611 remains OPEN/BLOCKED, native UNQUALIFIED.** Manager #424 should choose whether to defer (recommended) or authorise a separate bounded Windows port/setup task. There is no automatic follow-on installation or implied native material acceptance.

Evidence consists of source reads pinned above, Windows/WSL directory/process/capacity/Registry-key-existence queries, Git status/common-dir/worktree inspection and the prior #611 hash receipt. No compiler/runtime/lock/self-test gate was run; those would violate this task or require absent infrastructure. No current remote Linux operational PASS is claimed.

Final publication must be exactly one added Markdown against the registered starting HEAD, with byte-identical GitHub readback and a compare/closure receipt on #621. Primary checkout remains clean; no Ops, lock, Unity project, production source/asset/scene/Packages/ProjectSettings, saves/Prefs/historical worlds or process changed by this investigation. No Editor, Player or Windows build. Git source fetching/publication are the only repository bookkeeping writes. Live coordination/physical board remain designated-Manager ownership. Dashboard/Drive synchronization **NOT VERIFIED**; operational follow-up remains with Manager #424.
