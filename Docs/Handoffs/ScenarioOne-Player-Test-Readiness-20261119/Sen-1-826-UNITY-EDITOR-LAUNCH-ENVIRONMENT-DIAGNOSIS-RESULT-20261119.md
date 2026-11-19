# Sen-1 #826: Unity Editor launch-environment diagnosis for #820, RESULT

**19 November 2026. Phase A only. Verdict: `(c) EVIDENCE INSUFFICIENT: HOST NOT INSPECTABLE FROM THIS SESSION`, with a ranked hypothesis list and one discriminating read-only check for an on-host worker.**

Manager: [#824](https://github.com/jermur788/CCF/issues/824). Claim: [#826](https://github.com/jermur788/CCF/issues/826) ([registration](https://github.com/jermur788/CCF/issues/826#issuecomment-7018222512)). Packet: [`docs/sen-1-826-unity-launch-diagnostic-20261119-v1` @ `24ac22a2`](https://github.com/jermur788/CCF/blob/24ac22a256d8d70d49fbc7dd6e0481539c0d2ab3/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-826-UNITY-LAUNCH-CONTEXT-READONLY-DIAGNOSTIC-HANDOFF-20261119.md). Dashboard/Drive: **NOT VERIFIED**.

---

## 0. Who ran this, where, and what it could not do

| Field | Value |
|---|---|
| Worker | Claude Code (inside T3 Code), model `claude-opus-5-5`, Anthropic. One parent, no child agents |
| Host actually used | `DESKTOP-DL1BQI7` (Windows 11 + WSL2 Ubuntu). **Not `jer-laptop`** |
| `jer-laptop` reachability | Tailscale peer online (`100.113.123.38`, direct). **No shell**: OpenSSH port 22 refused, Tailscale SSH not enabled. No other remote-access route was attempted |
| Board at start | `origin/main` @ `fe96b8b8`: #826 READY / NOT RUNNING, no worker, result path unreserved |
| Result branch | `docs/sen-1-826-worker-launch-diagnosis-result-20261119-v1`, worker-owned, created from packet tip `24ac22a2`. The Manager's packet branch was **not** modified |
| Local worktree | none. This file was written directly to GitHub |

**Consequence.** Most of the packet's Phase A steps need a shell on `jer-laptop`: the failing launch's environment, UID/groups, `HOME`/XDG, display/DBus/runtime dir, Hub and licensing-client process ownership, and the local evidence files `06-tier2-licence-blocker.txt`, `unity/harness-run.log` and `unity/gui-licence-test.log` under `/media/jer/Files/CCF/Verification/Sen1-820-E2-Native-20261119/`. **None of that was observed.** The evidence files were never published to GitHub. Everything below about `jer-laptop` comes from the published [#820 amended result @ `3ff6f406`](https://github.com/jermur788/CCF/blob/3ff6f406f2766db007a8d48b1bdd2d1c5d487549/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-820-E2-NATIVE-SCENE-COMPOSITION-GAMEPLAY-AND-REGRESSION-QUALIFICATION-RESULT-20261119.md) and the [owner correction](https://github.com/jermur788/CCF/issues/820#issuecomment-7016663762).

**Not done (packet prohibitions held):** no Unity Editor or batchmode launch, no Hub sign-in or activation, no licensing-service change, no credential, token, licence-XML, `accounts.db`, keyring or cookie access, no `unity-lock` action, no Ops/`lanes.json`/board/systemd/DBus change, no #820 checkout, no source, scene, asset or save edits, no Windows build, no merge.

---

## 1. Which launch failed, and under which user and session

| Question | Answer | Basis |
|---|---|---|
| Which launches failed | Two #820 attempts on `jer-laptop`, under lease `c5fc24d6…` (≈17:09–17:15 UTC): one `-batchmode -executeMethod`, one GUI. Both failed before project import. Batch exit code 198 | #820 §4.4 (published) |
| Error lines | `[Licensing::Module] Error: Access token is unavailable; failed to update`, then `'com.unity.editor.headless' was not found`, then `No valid Unity Editor license found` | #820 §4.4 (published) |
| Editor binary | Probably under `/media/jer/ZX20/Unity/6000.6.0f1` (changeset `f7f8ed4d1e24`). #820 names this as the "Unity SDK present" but **does not publish the exact executable path or command line** | #820 §1.1. Inference, not verified |
| Effective user | Probably Linux user `jer`, since the inspected paths are under `/home/jer/.config/…`. **UID/groups not published. NOT VERIFIED** | Inference |
| Graphical session, DBus, runtime dir | **NOT VERIFIED.** #820 does not record `DISPLAY`/`WAYLAND_DISPLAY`, `XAUTHORITY`, `DBUS_SESSION_BUS_ADDRESS` or `XDG_RUNTIME_DIR` for the OpenCode process that started the Editor. It also does not say whether Unity Hub was running, or as whom | Absent from evidence |
| Launched via Hub or directly | **NOT VERIFIED.** The wording ("launched twice", one with `-executeMethod`) suggests the binary was run directly by the agent, not from Hub | Inference |

## 2. Owner's working context, as far as it can be compared

| Question | Answer |
|---|---|
| Where did the owner's working session (~1 h before the failures, ~16:xx UTC) run? | **Not established.** The owner said "desktop session". That could mean the graphical login on `jer-laptop` or this Windows PC. |
| Was it this Windows PC (`DESKTOP-DL1BQI7`, user `hp`)? | **No evidence that it was.** Read-only timestamp metadata only: no `%LOCALAPPDATA%\Unity\Editor\Editor.log` exists; the newest Unity Hub log (`info-log.json`) was last written **2026-11-17 01:00 UTC**; the only Hub-managed Editor installed is **6000.6.9f1**, not 6000.6.0f1; no Unity process is running. So on this PC, as user `hp`, nothing shows Unity use on 19 Nov. The owner's session most likely ran on `jer-laptop`, but that is an inference. |
| Same executable or build as the worker? | **Unknown.** |
| Same Linux user, graphical session and DBus bus? | **Unknown.** No direct one-to-one comparison is possible from this session. |
| Do the worker's inspected `~/.config/unityhub` store and `~/.config/unity3d/.../licenses` cache belong to the same login context the owner used? | **Unknown, and this is the crux.** #820 reported "0 accounts" and an entitlement `UpdateDate` of 2026-10-15. Neither shows whether the owner's working Hub writes to those same paths. |

## 3. What the published evidence does and does not show

**Observed (published by #820):**

1. The failure is client-side licensing at Editor start-up: no access token, so no entitlement refresh. It is not a project, compile or asset failure. Import was never reached.
2. `license.unity3d.com` answered HTTP 200 from the worker context, so plain network reachability was fine.
3. The cached entitlement under `/home/jer/.config/unity3d/` was last updated 2026-10-15. The Hub account store under `/home/jer/.config/unityhub/` held no account rows.

**Not shown, despite the original #820 wording:**

- That the owner's Hub "is simply not signed in" or that the owner's entitlement is stale. The owner's first-hand report of a working Editor ~1 h earlier contradicts this, unless that session used a different config root, user, install or host.
- That re-activation is the remedy. The owner has since refreshed the licence. That proves nothing about the worker context; see §6.

**Process note (no further action):** #820 reported `accounts.db` row counts. That is closer to "private database rows" than #826 now allows. This run did not repeat it, and the next check in §5 does not need it.

## 4. Ranked hypotheses (none verified)

Unity's licensing behaviour below is general product knowledge, labelled as an **assumption**. It was not verified on this machine.

| # | Hypothesis | Fits the evidence because | Would be contradicted by |
|---|---|---|---|
| **H1** | **Different config root.** The owner's working Hub/Editor keeps its account and licence state somewhere other than `/home/jer/.config/unityhub` and `/home/jer/.config/unity3d`. For example: a sandboxed Hub (Flatpak `~/.var/app/com.unity.UnityHub/…` or snap `~/snap/…`), a different Linux user, or a different `XDG_CONFIG_HOME`. The worker's directly launched Editor read the stale, unsigned default root. | It explains **all three** facts together: 0 accounts, a 35-day-stale cache, and the owner's success one hour earlier. | The owner's working-session logs and the post-refresh licence file both appearing under `/home/jer/.config/unity3d` with 19 Nov timestamps. |
| **H2** | **Same root, but the Hub session's token was not reachable from the agent process.** The Editor was started directly by an OpenCode/T3 process that lacks the desktop session's DBus, keyring or runtime environment, or Hub was not running or signed in at that moment. The licensing client therefore could not obtain an access token. | It matches the exact error text ("Access token is unavailable") and the "works from the desktop, fails from the agent" pattern. | The owner's working session also being a direct non-Hub launch from a plain shell without DBus. |
| **H3** | **Different Editor install.** The owner used a Hub-managed 6000.6.0f1 (or another version or project), while the worker used the copy on exFAT `/media/jer/ZX20/Unity/6000.6.0f1`. That copy might have an incomplete licensing-client component or the wrong permissions. | The two installs are not shown to be the same. | Both contexts showing the same `Application path`/command-line binary in their Editor logs. |
| **H4** | **Different host.** The owner's working session was on another machine. | "Desktop session" is ambiguous. | §2: this Windows PC shows no 19 Nov Unity use as user `hp`. So H4 is **weakened for this PC** but not excluded for other users or machines. |

H1 and H2 are the leading candidates. They can be told apart with one short check.

## 5. One discriminating next check (read-only, on `jer-laptop`, as the owner's desktop user)

Run this **inside the owner's normal graphical login session** on `jer-laptop`, in a terminal opened from the desktop. It does not launch Unity, touch credentials, or print file contents other than filtered log lines. Long token-like strings are redacted.

```bash
# A. Which config roots have Unity state, and when they last changed (metadata only)
for d in ~/.config/unity3d ~/.config/unityhub ~/.var/app/com.unity.UnityHub ~/snap/unityhub; do
  [ -e "$d" ] && stat -c '%y  %U  %n' "$d" || echo "absent  $d"; done
find ~/.config/unity3d ~/.var/app/com.unity.UnityHub -maxdepth 4 \
  \( -name 'Editor*.log' -o -name '*Licensing*.log' -o -name '*.xml' -path '*licenses*' \) \
  -printf '%TY-%Tm-%Td %TH:%TM  %u  %p\n' 2>/dev/null | sort
flatpak list --app 2>/dev/null | grep -i unity; snap list 2>/dev/null | grep -i unity

# B. Which binary, and which licensing outcome, each recent Editor session had (filtered, redacted)
for f in ~/.config/unity3d/Editor.log ~/.config/unity3d/Editor-prev.log \
         /media/jer/Files/CCF/Verification/Sen1-820-E2-Native-20261119/unity/*.log; do
  [ -f "$f" ] || continue; echo "== $f  ($(stat -c %y "$f"))"
  grep -m12 -E 'Command line arguments|Application path|\[Licensing::(Module|Client)\].*(Error|Successfully|connected|entitlement)' "$f" \
    | sed -E 's/[A-Za-z0-9_.+\/=-]{32,}/<redacted>/g'
done

# C. Environment *names present* (no values) for Hub vs the agent process
for p in $(pgrep -f -i 'unityhub|Unity.Licensing.Client') $(pgrep -f -i 'opencode|t3' | head -3); do
  echo "== pid $p user=$(ps -o user= -p $p) cmd=$(ps -o comm= -p $p)"
  tr '\0' '\n' < /proc/$p/environ 2>/dev/null | cut -d= -f1 \
    | grep -xE 'HOME|XDG_CONFIG_HOME|XDG_RUNTIME_DIR|DISPLAY|WAYLAND_DISPLAY|XAUTHORITY|DBUS_SESSION_BUS_ADDRESS' | tr '\n' ' '; echo
done
```

**How to read the result:**

- **Branch H1.** The owner's working session or post-refresh licence appears under a **different** root (Flatpak/snap path, another user, or another `XDG_CONFIG_HOME`), while `~/.config/unity3d` stays at 2026-10-15. **Root cause: config-root mismatch.** Minimal remedy for Phase B: launch the #820 Editor so it uses the same root and licensing client as the owner's working Hub. For example, start it from that Hub, or from inside the same sandbox or user. Do not change user settings.
- **Branch H2.** The owner's working session **and** the post-refresh licence are under `~/.config/unity3d` with 19 Nov timestamps, and the agent process lacks `DBUS_SESSION_BUS_ADDRESS`/`XDG_RUNTIME_DIR`/display variables that Hub has. **Root cause: agent launch context cannot reach the Hub session.** Minimal remedy for Phase B: run the #820 Editor with Hub running and signed in, from a process inside the owner's graphical session, for example an agent started from a desktop terminal. Then confirm the licensing lines show success before import.
- **Branch H3.** The `Application path` or command-line binary differs between the owner's working log and the #820 logs. Retry with the owner's known-working install, after the Manager approves.
- **None of these apply.** Report (c) not reproducible, and keep the full #820 log excerpts for the Manager.

## 6. Verdict

```
SEN1_826_UNITY_LAUNCH_DIAGNOSIS
host_inspected:                   NO  (worker on DESKTOP-DL1BQI7; jer-laptop has no shell access)
failing_launch:                   #820 batchmode + GUI, jer-laptop, ~17:09-17:15 UTC, pre-import licensing error
failing_user_session:             NOT VERIFIED (paths imply user jer; session/DBus/display unknown)
owner_working_context:            NOT VERIFIED (no 19-Nov Unity activity on DESKTOP-DL1BQI7 as user hp)
root_cause:                       NOT ESTABLISHED
leading_hypotheses:               H1 config-root mismatch; H2 agent context cannot reach Hub token
discriminating_check:             Section 5 (read-only, on jer-laptop, inside owner desktop session)
licence_refresh_proves_retry_ok:  NO
unity_launched:                   NO
credentials_accessed:             NO
source_or_asset_edits:            NONE
ops_lock_board_changes:           NONE
dashboard_drive:                  NOT VERIFIED
verdict:                          (c) EVIDENCE INSUFFICIENT FROM THIS HOST, with (b)-style next check
```

**Can the Editor be retried now?** Not on this evidence. The owner's licence refresh does **not** show that the worker's launch context will now get a token. Under H1 the worker would still read the old root. Under H2 it would still lack the session. Phase B stays with Manager #824 as written: run the §5 check first (or an on-host repeat of Phase A), then, if approved, take a fresh lawful exclusive lease under D-091 before any Editor start. #820's composed candidate `8cb22de0` and Tier 1 PASS are untouched.

**Smallest way to unblock a proper Phase A:** either a native worker running on `jer-laptop`, or the owner running the §5 block there and pasting the redacted output into #826.
