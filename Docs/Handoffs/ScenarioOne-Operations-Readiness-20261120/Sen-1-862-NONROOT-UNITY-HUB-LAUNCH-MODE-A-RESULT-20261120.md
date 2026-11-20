# #862 — Non-root Unity Hub launch smoke: MODE A PASS

**Result: PASS for #862's acceptance (mode A).** On `jer-laptop` there were two separate GUI Editor launches of a disposable project. Both ran as effective UID 1000 inside the authenticated Unity Hub Flatpak sandbox. Neither showed a root warning, both connected to the Hub's licensing client, and both closed normally (exit 0). No new root-owned files appeared. The Unity lease was released normally. Mode B was not needed.

This is a launch smoke only. It does not test CCF gameplay, a CCF build or historical saves. It does not apply a lasting fix to how agents launch Unity; that still needs separate scope under #861.

## 1. Actor and authority

| Item | Value |
|---|---|
| Executor | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code inside T3 Code, T3 thread `48af0d29-298b-45e5-b2a6-2d5a551e24f7`, host `jer-laptop`, `uid=1000(jer)`, uid_map identity (not in a user namespace) |
| Predecessor | Codex `gpt-6.1-sol` high in the same T3 thread. It completed the #857 status correction, the #862 lane/row registration and the worktree creation (receipt below). It then stopped on a provider `429 Too Many Requests` error before creating the fixture or taking a lease. |
| Takeover authority | The owner chose "Claude takes over" in this thread on 2026-11-20. This answered the case in amendment §1 ("if not [available], stop only pending real actor registration"). The work stayed inside the scope of [owner-direct amendment `2d5ed15e`](https://github.com/jermur788/CCF/blob/docs/sen1-865-862-owner-direct-single-worker-unblock-20261120-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Sen-1-865-862-OWNER-DIRECT-SINGLE-WORKER-OPS-ADMISSION-AND-HUB-SMOKE-AMENDMENT-20261120.md). |
| Lane / branch / base | `Sen1-862-NonRootHubSmoke`, `/media/jer/ZX20/CCF-Sen1-862-NonRootHubSmoke-20261120`, `test/sen1-862-nonroot-hub-launch-20261120-v1` @ `3d8fbd698cd5b9290d24e4cbb3cf14849a27c2b6`. The worktree was clean before and after (0 porcelain lines). |

## 2. Physical Ops changes (all under `flock -x -n /media/jer/ZX20/CCF-Ops/board`, full `packet-lint --board` PASS each time)

| Step | Actor | board sha256 | lanes sha256 | Change |
|---|---|---|---|---|
| 0 pre | — | `8eeb3a87…1d12` | `0047ef12…9c22` | — |
| 1 | Codex | `8e6ca209…bfe1` | `579b5d9d…e5ad` | #857 row: only `State` (`O0 RUNNING / SOURCE+UNITY HELD` → `DONE-PENDING-LEDGER`) and `Locks`, after the normal-release witnesses `f736db4d`, `860e848a` and `0c7756ae` were checked in the deployed history. Added one #862 row and one #862 lane. |
| 2 | Claude | `7ae98831…425d` | `22823ce9…a09c` | #862 row only: fields Actor/Tool/Model/Provider (Codex → Claude). #862 lane only: actor text in `purpose`. |
| 3 | Claude | `950230cd…9066` | unchanged | #862 row only: State `ACTIVE` → `DONE-PENDING-LEDGER`, plus the Locks, result and next-action fields. |

No other row, lane, script, deployment file or permission was changed. #857 is **not** represented as reviewed, integrated or accepted.

## 3. Fixture

- Path: `/media/jer/ZX20/CCF-Sen1-862-NonRootHubSmoke-20261120/Build/NonRootHubSmoke`. The repo ignores this path (`.gitignore:9 /[Bb]uild/`).
- How it was created: `Unity -batchmode -nographics -quit -createProject <path> -cloneFromTemplate …/ProjectTemplates/com.unity.template.urp-blank-17.2.1.tgz`, run inside the Hub sandbox. This is the installed blank 3D (URP) template, the same Editor mechanism Hub uses. Exit 0, Editor `6000.6.0f1 (f7f8ed4d1e24)`. UPM resolved the template's normal public packages; Unity itself was not installed or updated.
- Identity: `companyName: CCFNonRootTest`, `productName: HubSmoke862`. These were set after creation. It contains no CCF scripts, scenes or saves.
- Size about 5.1 GB, mostly `Library`. It is retained; disposal is for the manager to decide.
- **Mode-A deviation from the packet wording:** the project was created by the Editor `-createProject` command, not by clicking "New project" in the Hub GUI. A different session previously attempted a manual template copy (Codex `create_fixture.py`), which failed on a missing cached `com.unity.ide.visualstudio@2.0.26` archive before creating anything.

## 4. Launch method (mode A)

`launch.sh` finds the Hub's own Flatpak instance (`3611728743`, `com.unity.UnityHub`) and its `unity serve` process. It copies that process's environment into an `env -i` launch, dropping agent and credential variables; the environment is never logged. It then runs:

```
flatpak enter <hub-instance> /usr/bin/env -i <hub env> /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -projectPath <fixture> -logFile <run>/editor.log
```

This is the `flatpak enter` route proposed in #829. There was no `sudo` and no root shell, and no dialog was clicked. Each Editor was closed with `wmctrl -ic <main window>`, the normal window-manager close request, which triggers Unity's normal quit path.

## 5. Results

| Check | Launch 1 (13:06:38–13:17:51Z) | Launch 2 (13:18:12–13:19:56Z) |
|---|---|---|
| Editor process `/proc/<pid>/status` Uid/Gid | 1000 1000 1000 1000 / same | 1000 1000 1000 1000 / same |
| Licence | `Successfully connected to LicensingClient on channel "LicenseClient-jer"`; entitlements resolved; signed-in account shown in toolbar | same |
| Root/privilege warning | none. The only Unity windows were the untitled splash windows and `Unity - NonRootHubSmoke - Untitled - … Unity 6.6 (6000.6.0f1)`. Screenshot shows the main Editor and no dialog. | none (same windows; screenshot) |
| Log text `running as root` / `superuser` | 0 | 0 |
| Close | window close → `Exiting`, launcher exit **0**, no Editor left | window close → launcher exit **0**, no Editor left |

Launch 1 took longer because it ran the first GUI import of the new project.

Two disclosed driver issues, neither of which affected the product:

1. On launch 1 the driver matched windows by `_NET_WM_PID`, which the sandbox does not set. It therefore never selected the main window. I verified the window state and UID manually and closed the window myself; the driver then recorded launcher exit 0.
2. On launch 2 the driver's warning regex `root` matched the project name "Non**Root**HubSmoke" and logged "ROOT-LIKE WINDOW" on every poll. The window list and screenshots show the only titled window was the main Editor window. **This was a false positive, not a warning.**

**Files and saves**

- Root-owned file counts are unchanged (0 → 0) in the lane, `~/.var/app/com.unity.UnityHub` and `~/.config/unity3d`. The `/tmp` root-owned list is byte-identical before and after.
- All 36 pre-existing files under both `DefaultCompany` config trees have unchanged sha256. This includes `…/DefaultCompany/CCF/forest-save.json` and the 2026-11-04 save backup.
- **New, scratch-only files:** 3 files under `~/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/NonRootHubSmoke/` (`prefs` and two Editor analytics files). The creation run wrote them before the scratch company/product names were set. They are harmless but not yet removed, pending a disposal decision.

**Lease**

`cf696a20ad01474f81a0ccbfbf90bf78` was acquired at 13:02:13Z on a live holder process. My first release call omitted `--task-id` and was refused, and I had already ended the holder, so the lease briefly read `stale`. A second call with the exact owner, task and lock ID then released it normally at 13:20:59Z (`forced: false`, history `event: release`). The final status is `free`, with metadata null and no Editor PIDs.

## 6. What this does and does not settle

- **Settled:** an agent shell on this laptop can open the Editor non-root, with a working licence and no root warning, by entering the existing Hub Flatpak sandbox.
- **Not settled:** what exactly caused the two earlier root warnings. The owner reported them; their launch processes were not captured. A likely candidate is an agent sandbox that maps the user to UID 0, as `bwrap`-style tool sandboxes can. This is an **unverified hypothesis**: this session's `uid_map` was an identity mapping, so it did not reproduce that condition.
- Mode B (GUI mouse/keyboard via Hub) was not run because mode A passed.
- A lasting launcher change, for example adding this route to CCF gate runners, needs its own approval under #861. No CCF-Ops scripts were changed.

## 7. Evidence

All evidence is on the Files drive at `/media/jer/Files/CCF/Verification/ScenarioOneNonRootHubExecution-20261120/`:

- Codex step: `register862.py`, `registration-receipt.json`, `board/lanes.{before,candidate,after}`, `*-lint.txt`.
- Claude steps, in `claude-takeover/`: `actor_txn.py` + `receipt.json`; `launch.sh`, `gui_run.sh`, `gui_run2.sh`; `create.log`; `run1/`, `run2/` (editor logs with access tokens redacted, `run.txt`, screenshots); `lease-acquire.json`, `lease-release.json`, `lock-final.json`; `root-owned.{pre,post}.txt`, `tmp-root-owned.pre.list`, `saves.pre.sha256`; `close/close_txn.py` + `receipt.json`.
- Screenshots stay local because they show the owner's desktop.

## 8. Next actions (for #865 / #861)

1. Manager review of this result. Note that the executor changed from Codex to Claude by owner choice.
2. Decide whether to dispose of the lane, worktree and fixture (about 5.1 GB on ZX20) and the 3 `DefaultCompany/NonRootHubSmoke` config files.
3. Under #861, decide whether to make `flatpak enter` into the Hub sandbox the standard way agents launch Unity on this laptop.
