# Sen-1 #829: on-laptop Unity launch-context diagnosis, RESULT

**19 November 2026. Phase A only, read-only. Verdict: `VERIFIED H1: config-root mismatch (Flatpak Unity Hub sandbox vs host default root)`. Mechanism: the #820 worker started the Editor outside the Flatpak Hub sandbox, so it could not reach the Hub's running licensing client and started a second client that read the old, unsigned host root. Same Linux user and same Editor binary. H3 rejected. H2 as stated (missing DBus/display) is not needed to explain the failure.**

Manager: [#824](https://github.com/jermur788/CCF/issues/824). Claim: [#829](https://github.com/jermur788/CCF/issues/829) ([RUNNING registration](https://github.com/jermur788/CCF/issues/829#issuecomment-7018570055)). Packet: [Manager #824 verdict and #829 handoff @ `b192fcbc`](https://github.com/jermur788/CCF/blob/b192fcbc8d6a76c45805a33758aa8c44d36f264f/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-824-826-MANAGER-VERDICT-AND-829-LAPTOP-UNITY-DIAGNOSIS-HANDOFF-20261119.md). Prior: [#826 result](https://github.com/jermur788/CCF/blob/8733120811190e6841120498ea59449b8244bdae/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-826-UNITY-EDITOR-LAUNCH-ENVIRONMENT-DIAGNOSIS-RESULT-20261119.md). Dashboard/Drive: **NOT VERIFIED**.

---

## 0. Who ran this, and where

| Field | Value |
|---|---|
| Worker | Claude Code inside T3 Code, model `claude-opus-5-5`, Anthropic. One parent, no child agents |
| Host | **`jer-laptop`**, checked at task start: `hostname` = `jer-laptop`, Linux 7.0.0-42-generic, Linux Mint 22.3. Local session. No SSH, Tailscale or Windows/WSL route |
| Effective user | `jer` |
| Operator authorisation | The owner dispatched #829 into this local session |
| Branch / worktree | `docs/sen-1-829-laptop-unity-diagnosis-result-20261119-v1`, a new worker-owned branch, in `/media/jer/Files/CCF/Worktrees/CCF-Sen1-829-LaptopUnityDiag` (the path was free; tested writable). The Manager's packet branch was **not** modified |
| Starting commit | `b192fcbc8d6a76c45805a33758aa8c44d36f264f` (packet branch tip) |
| Observation window | 2026-11-19 ≈18:40–18:46 UTC. Laptop clock is GMT (+0000), so local and UTC timestamps are the same |

**Not done (prohibitions held):** no Unity Editor or Hub launch, no sign-in, activation or licence change, no Unity process killed or signalled, no `unity-lock` call, no Ops board or `lanes.json` write, no #820 source, scene, save or asset change, no merge, no Windows build. **No contents were read from** licence XML, Hub `accounts.db`/LevelDB/local storage, cookies, keyrings or token stores. Only file metadata (existence, owner, mtime) was used for those. No whole `/proc/<pid>/environ` and no raw command lines were printed. Environment checks listed variable **names** and compared values for **equality** without printing them.

## 1. Evidence used

| Source | What was read |
|---|---|
| `/media/jer/Files/CCF/Verification/Sen1-820-E2-Native-20261119/` (#820 evidence, read only) | `06-tier2-licence-blocker.txt`, plus selected header, argv[0] and `[Licensing::…]` lines from `unity/harness-run.log` (batch, 17:09:46Z) and `unity/gui-licence-test.log` (GUI, 17:11:17Z) |
| Host default Unity root `~/.config/unity3d`, `~/.config/unityhub` | Metadata only |
| Flatpak Hub sandbox root `~/.var/app/com.unity.UnityHub/{config,cache}` | Metadata only, plus selected header, argv[0] and `[Licensing::…]` status lines from the two Editor logs there (the owner's sessions) |
| Installed packages | `flatpak list`, `flatpak info --show-permissions`, `dpkg -l`, `snap` (names and versions only) |
| Live processes | Owner, start time, command name, and env-var name presence or value equality for the Hub licensing client and this agent shell |

## 2. Findings

### 2.1 Install: Unity Hub is a Flatpak

| Check | Result |
|---|---|
| Unity Hub package | **Flatpak `com.unity.UnityHub` 3.22.3**, system installation. No native `.deb` Hub and no snap. No `unityhub` on the host `PATH` |
| Flatpak permissions | `filesystems=host`, `shared=network;ipc`, sockets `x11;wayland;pulseaudio`. So the sandbox can see `/media/jer/ZX20` and `/media/jer/Files` |
| Hub-managed Editor folder `~/Unity/Hub/Editor` | Empty. The only 6000.6.0f1 install found is `/media/jer/ZX20/Unity/6000.6.0f1` (Editor binary mtime 2026-09-13) |

### 2.2 Binary (H3): same for both

| Session | Time (UTC) | Batch | argv[0] |
|---|---|---|---|
| Owner, Hub-launched (sandbox `Editor-prev.log`) | 16:40:18 | NO | `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |
| Owner, Hub-launched (sandbox `Editor.log`) | 16:43:23 | NO | `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |
| #820 worker, batch | 17:09:46 | YES | `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |
| #820 worker, GUI | 17:11:17 | NO | `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |

All four report `6000.6.0f1 (f7f8ed4d1e24)`. **H3 rejected.**

### 2.3 Licensing outcome by session (selected status lines; tokens and IDs removed)

| Session | Licensing channel | Token | Outcome |
|---|---|---|---|
| Owner 16:40 and 16:43 | `Successfully connected to LicensingClient on channel "LicenseClient-<user>" (connect: 0.00s)`: joined an **already-running** client | `Successfully updated the access token` | `Successfully resolved entitlement details`. Editor licensed |
| #820 batch 17:09 and GUI 17:11 | `Channel LicenseClient-<user> doesn't exist`, then `Successfully launched the LicensingClient`: **started its own** client | `Error: Access token is unavailable; failed to update` | `404 … Found 0 entitlement groups and 0 free entitlements`, then `'com.unity.editor.headless'` / `'com.unity.editor.ui' was not found`, then `No valid Unity Editor license found`. Batch exit 198 |

The channel name is the same per-user name in both cases. The difference is whether the existing channel could be **reached**.

### 2.4 Config root (H1): two separate roots, and the owner's activity is only in the sandbox root

| Item | Host default root (`~/.config/…`) | Flatpak Hub root (`~/.var/app/com.unity.UnityHub/config/…`) |
|---|---|---|
| `unity3d/Unity/licenses/UnityEntitlementLicense.xml` mtime | **2026-09-15 14:16** | **2026-11-19 17:22:34** (matches the owner's reported refresh, ≈10 min after the worker's failures) |
| `unity3d/Editor.log` / `Editor-prev.log` mtime | 2026-09-22 22:42 / 2026-09-22 10:00 | **2026-11-19 16:43 / 16:40** (the owner's working sessions) |
| `unity3d/Unity/Unity.Licensing.Client.log` mtime | 2026-11-19 17:12:32 (consistent with the client the worker's Editor launched) | 2026-11-19 17:56:17 |
| `unityhub` directory mtime | 2026-10-04 (#820 reported 0 account rows here) | 2026-11-19 18:41 (Hub active today; `info-log.json` written 18:41) |

A timestamp shows only that a file changed. It does not show that a licence is valid. The owner's successful Editor logs, however, do show that the sandbox context obtained a token and entitlements at 16:40–16:43.

### 2.5 Session and process context (H2)

| Check | Result |
|---|---|
| Hub licensing client (`Unity.Licensing.Client`) | Running as `jer` and **started 10:56:16 UTC**, 5 s after `unityhub-bin` (10:56:11). The same process is still alive now, so it was **running throughout the worker's 17:09–17:11 launches** |
| That client's environment | Contains `FLATPAK_ID=com.unity.UnityHub` and `container`. `XDG_CONFIG_HOME` **equals** `~/.var/app/com.unity.UnityHub/config`. `TMPDIR` is under the app's sandbox cache. `HOME`, `XDG_RUNTIME_DIR` and `DISPLAY` are **equal** to this agent shell's values |
| Mount namespace | The Hub client and this agent shell are in **different** mount namespaces (Flatpak sandbox vs host) |
| This agent session's environment (a sibling T3 Code session, *not* the #820 worker) | `HOME`, `XDG_RUNTIME_DIR`, `DBUS_SESSION_BUS_ADDRESS`, `DISPLAY` and `XAUTHORITY` present. `XDG_CONFIG_HOME` and `FLATPAK_ID` absent, so default-root resolution goes to `~/.config` |
| #820 worker's own environment at 17:09–17:11 | **Not captured and cannot be proven retrospectively.** Its process has ended. Its logs show it ran as the same user, saw the desktop (`Desktop is 1920 x 1080`), started a licensing client, and that client wrote to the host root at 17:12 |

## 3. Conclusion

**Verified H1: config-root mismatch, caused by launching outside the Flatpak sandbox.**

1. The owner's working Editor sessions were launched by the **Flatpak** Hub. They ran inside its sandbox, used `XDG_CONFIG_HOME=~/.var/app/com.unity.UnityHub/config`, and joined the Hub's long-running licensing client, which had a valid access token.
2. The #820 worker ran the **same binary** directly from the host. The Hub's licensing client was running at the time, but the Editor reported that its channel `doesn't exist`. So the worker's Editor launched a second, host-side licensing client. That client used the host default root `~/.config/unity3d` (licence file last written 2026-09-15, Hub account store empty). It had no access token, found 0 entitlements and refused to start.
3. The owner's later licence refresh (17:22) was written only to the sandbox root. **Retrying the same host-side launch would fail the same way.**

**Assumption, not verified:** the sandbox client's IPC endpoint was unreachable from the host because Flatpak gives each sandbox a private `/tmp` and mount namespace. This is general Flatpak behaviour. The different mount namespace was observed, but the socket location was not inspected.

**H2**, read as "the agent lacked DBus or display variables", is **not needed** and is not supported for this session. The sibling agent session has those variables, and the #820 worker visibly had a display. The real "cannot reach Hub" mechanism is the sandbox boundary, which belongs under H1. **H4** (different host) is rejected: the owner's working logs are on `jer-laptop`.

**Minor, unrelated:** both #820 logs show `VBCSCompilerPreWarmer … failed to write stub code temporary file (/tmpUnityVBCS…)`, a missing path separator. That suggests the worker's `TMPDIR` lacked a trailing slash or was unusual. It is harmless to licensing, but it is another sign that the launch context differed from Hub's.

## 4. Smallest correction (proposed, NOT applied)

**Start the #820 Editor inside the running Flatpak Hub sandbox, not from the host.** It then uses the same config root and the same signed-in licensing client as the owner's working sessions. No licence, Hub, settings or file changes are needed.

Preferred (keeps `-batchmode -executeMethod` for #820):

```bash
# Read-only: find the running Hub instance (first column after --columns=instance)
flatpak ps --columns=instance,application
# Run the same binary inside that instance's sandbox
flatpak enter <hub-instance-id> /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -projectPath <#820 worktree> -executeMethod <…> -logFile <Files evidence path> …
```

Fallback (GUI only): open the #820 project from the Unity Hub window.

**Untested points for the Manager to weigh.**
- `flatpak enter` into the running instance was **not tried**. A separate `flatpak run --command=…/Editor/Unity com.unity.UnityHub …` would also use the sandbox config root, but would get a fresh private `/tmp`. It would then likely start its own sandboxed client, which reads the refreshed sandbox root. That is plausible but unverified.
- Whether Personal entitlements cover `com.unity.editor.headless` (batch mode) in the sandbox context is **not verified**. The owner's successful logs are GUI sessions only.

**Do not:** sign in a second, host-native Hub, copy licence or account files between roots, set `XDG_CONFIG_HOME` on a host launch to point into the sandbox (that would split one client's state across two processes), or activate the licence again.

**Pass criterion for the first retry log** (before any import is trusted):
- **no** `Channel … doesn't exist`;
- `Successfully connected to LicensingClient … (connect: 0.00s)` or another successful connection;
- `Successfully updated the access token`;
- no `No valid Unity Editor license found`.

## 5. How to reattempt (Phase B, NOT authorised by #829)

1. Manager #824 rules on this result.
2. If approved, the existing #820 worker obtains a **fresh, lawful, exclusive machine-wide Unity custody** under D-091 (no arbitrary time-based expiry; the earlier one-off lease is not standing authority). No forced locks.
3. Confirm the Flatpak Hub is running and signed in. Run a minimal licensing smoke with the §4 sandboxed launch and check the §4 pass criterion.
4. Only then run #820 Tier 2 unchanged.

## 6. Verdict block

```
SEN1_829_LAPTOP_UNITY_LAUNCH_DIAGNOSIS
host_inspected:                    YES  jer-laptop (local session, Linux Mint 22.3)
hub_install:                       Flatpak com.unity.UnityHub 3.22.3 (system), filesystems=host
same_user:                         YES  (jer)
same_editor_binary:                YES  /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity, f7f8ed4d1e24  -> H3 REJECTED
owner_working_sessions:            2026-11-19 16:40Z, 16:43Z, Hub-launched in sandbox, token + entitlements OK
worker_failing_sessions:           2026-11-19 17:09Z batch, 17:11Z GUI, host launch, own licensing client, no token
hub_licensing_client_during_fail:  RUNNING (started 10:56Z, still alive), sandboxed, different mount namespace
config_root_owner:                 ~/.var/app/com.unity.UnityHub/config  (licence file mtime 17:22Z refresh)
config_root_worker:                ~/.config  (licence file mtime 2026-09-15, Hub account store empty per #820)
worker_env_at_fail:                NOT CAPTURED (cannot be proven retrospectively)
root_cause:                        VERIFIED H1  config-root / sandbox mismatch
H2_missing_dbus_display:           NOT SUPPORTED / NOT NEEDED
H4_different_host:                 REJECTED
minimal_fix:                       launch Editor inside the running Flatpak Hub sandbox (flatpak enter), or via Hub GUI. NOT APPLIED
licence_refresh_alone_fixes_retry: NO  (refresh landed only in the sandbox root)
unity_launched:                    NO
credentials_or_licence_contents:   NOT READ
source_asset_ops_lock_changes:     NONE
dashboard_drive:                   NOT VERIFIED
```

#820's composed candidate `8cb22de0`, E1+E2 37/37 and Tier 1 7/7 PASS are untouched. Tier 2 remains **NOT RUN**.
