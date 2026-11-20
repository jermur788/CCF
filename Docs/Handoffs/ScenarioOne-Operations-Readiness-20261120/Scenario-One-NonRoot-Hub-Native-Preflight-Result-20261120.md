# Scenario One — native non-root Hub launch preflight for #862

**Result: PARTIAL / HELD BEFORE EDITOR LAUNCH. Mode A NOT RUN; mode B NOT RUN.**

Native observations were collected on **jer-laptop**, through 2026-11-20 11:41:59 UTC. This report supersedes the earlier HP/WSL access limitation **for this executor only**. It does not supersede or overwrite that executor's historical report. Two successful Editor starts, working Editor licence, absence of root-warning modals and clean shutdown remain unverified.

## Authority and actor

- Task: [#862](https://github.com/jermur788/CCF/issues/862), PACKET ONLY, Tier 1 launch smoke.
- Binding [test packet](https://github.com/jermur788/CCF/blob/ed67644a5ada932bb145f115a187bf94bcc3bc51/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Sen-1-862-POST857-NONROOT-UNITY-HUB-LAUNCH-TEST-HANDOFF-20261120.md), especially §§1, 3–5.
- [#844 next-slot instruction](https://github.com/jermur788/CCF/issues/844#issuecomment-7037691099) reserves the next lawful slot conditionally; it is not a physical lease or assignment of another task's project.
- Actor: OpenAI / T3 Code / Codex **gpt-6.1-sol**, high reasoning, configured Codex provider. Actual host `jer-laptop`; real/effective UID **1000**, ordinary desktop user. Internal session/thread ID not established; none fabricated. This executor performed the checks directly; no child worker was launched.
- [Native preflight registration](https://github.com/jermur788/CCF/issues/862#issuecomment-7038644968).
- Refreshed main base: `3d8fbd698cd5b9290d24e4cbb3cf14849a27c2b6`; context commit on that main history: `7e24ba0eee05b9af1055c88e423e629fe409bbf1`.
- Documentation branch: `docs/scenario-one-nonroot-hub-native-preflight-20261120-v1`. Sole new tracked output: this report. No main merge, PR, implementation or acceptance claim.

## Directly observed host and lease facts

| Check | Observation | Meaning / limit |
|---|---|---|
| Native shell identity | `id`: UID 1000; Python real/effective UID both 1000 | Current executor is not root |
| Desktop session availability | DISPLAY, DBUS_SESSION_BUS_ADDRESS and XDG_RUNTIME_DIR present; WAYLAND_DISPLAY absent | Categorical presence only; no raw environment published |
| Existing Flatpak Hub instances | Two instances of `com.unity.UnityHub`: 3611728743 and 314484360 | Existing session reused only for read-only identity checks |
| Entered sandbox identity | `flatpak enter <instance> /usr/bin/id` returned UID 1000 in both instances | Joining either current sandbox for this command did not elevate UID; no Editor was started |
| Hub process | PID 151877, real/effective/saved/filesystem UID all 1000 | Same ordinary user |
| Licensing process | PID 152032, all four UIDs 1000 | Process presence does not prove valid Editor entitlement |
| Apparent lowercase Unity process | PID 152102, UID/EUID 1000; Hub background `unity serve` | Not the capitalized Unity Editor executable; not a competing Editor |
| Installed live lock | FREE, metadata null, Editor PID list empty on repeated direct checks, last 11:41:59 UTC | Historical #857 release was independently rechecked; this is a timestamped observation, not a reservation |
| ZX20 storage-only gate | PASS, writable, 289.015 GiB free against 15 GiB minimum | No storage blocker at observation time |
| Lease acquired/released by this actor | None / none required | No takeover, force release or fictitious release record |

Only the deployed live Ops scripts were used. No non-deployed lock implementation, test-state override or alternate configuration was used.

## Exact hold and next action

The packet requires an **approved disposable project/candidate, source-read-only test lane and legitimate exclusive lease**. It also says: **“Do not change Ops scripts, sudoers, global file ownership, installed Unity Hub, license state or game source during this test.”**

The inspected deployed lane configuration has **no #862-specific registered lane**. The physical board still describes other tasks, including a stale #857 O0 row. It does not assign this actor a project. Current main's coordination and the dirty coordination anchor were read; neither supplies an exact #862 fixture/lane assignment. Existing #857 lane remains on its task branch at `511032f0124e28fdf61a8d1b676c81330f7fa5c0`, with empty Git status. That is preserved #857 work, **not a disposable fixture or transferred custody**.

An explicit assignment of an existing suitable registered lane and disposable project/exact candidate is needed. If a new lane is required, a separately authorised custodian must register it; this test does not patch installed Ops to admit itself. The executor requested that missing assignment from the owner during the active native turn.

After assignment: recheck current lock/PIDs/ownership, verify the assigned clean lane's exact branch/HEAD and isolated save/cache boundaries, acquire the deployed lock normally, then attempt two separate non-root Unity **6000.6.0f1** launches through the existing authenticated Flatpak Hub context. Verify actual Editor EUID, licence, project open/import, no warning, per-launch file ownership and clean exit. Release normally using the exact actor/task/lock identity. If A fails, B requires real authorised same-session desktop control; no such capability was established here. Browser automation alone is not desktop mouse control. Never acknowledge a root/security warning automatically.

## Mode findings and acceptance

| Mode / criterion | Result |
|---|---|
| A — authenticated Flatpak Hub programmatic Editor launch ×2 | **NOT RUN — fixture/lane admission held** |
| B — ordinary-user Hub desktop GUI launch | **NOT RUN — A not attempted; desktop control not established** |
| Current executor and entered Hub commands non-root | **PASS for inspected commands only** |
| Two actual Editor EUIDs nonzero | NOT VERIFIED |
| Working Editor licence and clean project import/open | NOT VERIFIED |
| Zero root-warning modals | NOT VERIFIED |
| No root-owned new project/cache files | NOT TESTED — no test project launched |
| Original save/project unchanged by a launch | NOT TESTED — no launch; no save/project write was requested or performed by this preflight |
| Normal test lease release | N/A — no lease acquired |
| Overall #862 acceptance | **NOT MET / HELD**, not PASS or task closure |

## Preservation, source and evidence provenance

Coordination anchor: `ops/workflow-foundation-v1` at `a42eaadc42bda6d474365f39b67a78a9b14bb043`. Its existing modified `AGENTS.md`, storage-architecture and active-task documents, three untracked operations notes and coordination archive were preserved; no checkout, reset, stash or cleanup was performed. Refreshed main was fetched without moving this checkout.

No game C#, Assets, scenes, packages, ProjectSettings, saves, Hub installation, launcher, system permissions or installed Ops configuration were changed. No Editor, Windows build or background worker was started. No credentials, licensing XML or account database were copied or published. No claim is made about the cause of the owner's earlier root warnings; current shell UID alone cannot diagnose a different past launch.

Read-only provenance SHA-256:
- deployed `scripts/unity-lock`: `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5`
- deployed `lanes.json`: `0047ef120742ef15d64221cc8ca0f2e166e982f4bee6ac8af2f3572042fa9c22`
- deployed `board/active.md`: `8eeb3a87919be41b351bca6ec612630700b53cba7dfcfd91288918ed71651d12`

The report is the durable sanitized evidence artifact; raw licensing logs and full environment are not publication inputs. Publication must be read back and compared to the fresh main base, confirming exactly one added Markdown. Notify #862, [#861](https://github.com/jermur788/CCF/issues/861), [#857](https://github.com/jermur788/CCF/issues/857), [#844](https://github.com/jermur788/CCF/issues/844) and [#425](https://github.com/jermur788/CCF/issues/425) with the immutable report URL.

**Dashboard sync: NOT VERIFIED. Published dashboard reflects this task: UNKNOWN.** No dashboard ingestion/publication or Drive mirror mechanism was invoked; #844/#425 must ingest this HELD status and verify the published audit advances. No generated dashboard-state file or canonical coordination document was edited by this executor.
