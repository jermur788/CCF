# Sen-1 #826 — Diagnose the Unity Editor launch-context discrepancy before resuming #820

**Issued:** 19 November 2026 by [current Scenario One Manager #824](https://github.com/jermur788/CCF/issues/824). **Status:** READY / NOT RUNNING, **Phase A READ-ONLY ONLY**. Owner has **not** requested more licence reactivation. This packet grants no Unity run, production source edit, live Ops installation or Windows build.

## Plain-English purpose

The worker's two attempts to start Unity Editor 6000.6.0f1 failed before project import with a licensing/access-token error, yet the owner reports successfully using Unity Editor from their desktop roughly one hour earlier and has now refreshed the licence. Do **not** label this a demonstrated owner-account licensing failure. Determine, with a short discriminating read-only check, what the worker's launch context differs from. Then report whether the Editor can be retried *under fresh lawful custody*, without changing the game.

**Difficulty:** LOW–MEDIUM (Linux process/session/toolchain forensic comparison, safe handling of user secrets). **Best suited:** a **native T3 Code/OpenCode DeepSeek v4.1 Flash** worker with shell/log access on the owner's Linux machine. This is not a GitHub-only browser task. No TG fork or multiple agent review is justified.

## Exact task registration and lane

- **Claim:** [Sen-1 #826 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/826). **Do not claim RUNNING** until the real native worker has checked actual host/lane/ownership and registered its model/provider/session, branch/worktree and start commit in issue comments.
- **Manager:** [#824](https://github.com/jermur788/CCF/issues/824) in this conversation; previous #731 was superseded. Scope **Scenario One only**.
- **Result branch:** `docs/sen-1-826-unity-launch-diagnostic-20261119-v1`. It exists as a GitHub management/packet branch; before the worker writes its result, refresh origin and verify its exclusive docs lane or create a disjoint worker-owned docs branch. Never modify another owner's branch.
- **Proposed native docs-only worktree (NOT physically verified):** `/media/jer/Files/CCF/Worktrees/CCF-Sen1-826-UnityLaunchDiag`; the physical worker must verify free/disjoint location before creation. No local worktree claimed by this GitHub-only manager.
- **Exact final worker result file:** `Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-826-UNITY-EDITOR-LAUNCH-ENVIRONMENT-DIAGNOSIS-RESULT-20261119.md`.
- **Source candidate to preserve READ-ONLY:** #820 `test/sen-1-820-compact30-e2-native-qualification-20261119-v1` (composed corrected E1 + final E2, original current candidate at the existing #820 pinned commit). Never checkout/reset/stash/modify it to diagnose a licensing error.
- **Evidence:** [#820 amended result](https://github.com/jermur788/CCF/blob/3ff6f406f2766db007a8d48b1bdd2d1c5d487549/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-820-E2-NATIVE-SCENE-COMPOSITION-GAMEPLAY-AND-REGRESSION-QUALIFICATION-RESULT-20261119.md) and [owner-corrected Manager interpretation](https://github.com/jermur788/CCF/issues/820#issuecomment-7016663762).

## Phase A — one short read-only diagnosis; authorised now

1. **G0 safe check.** Refresh actual #826 claim, new manager #824, live ownership and the current host. Check the result-path single-writer permission. Confirm no conflicting physical editing work or active Unity process is affected. No physical Ops board/lanes/lock modification.
2. **Establish exact failing invocation** from #820 evidence `06-tier2-licence-blocker.txt`, `unity/harness-run.log`, `unity/gui-licence-test.log` and available shell command history *where safe*. Capture timestamps, executable absolute path, user ID / groups, relevant effective `HOME` and XDG config/runtime/session paths, environment presence indicators, `DISPLAY`/`WAYLAND_DISPLAY`, `XAUTHORITY`, `DBUS_SESSION_BUS_ADDRESS`, `XDG_RUNTIME_DIR`, and Hub and licensing service process ownership. Do not expose raw environment dumps.
3. **Compare against owner's working desktop context** by reading nonsecret process/session metadata only if accessible and permitted; otherwise report that a direct one-to-one comparison is unavailable. Check whether Unity Hub account context and the worker's inspected database/cache are attributable to the same effective Linux user/session as the owner, rather than relying on row counts alone. Exact same executable/build? Same graphical session/bus? Errors before import? Distinguish observed facts from hypotheses.
4. **Make no credential discovery:** do NOT read, dump, cat or copy access tokens, Unity licence XML content, Hub accounts.db contents, browser cookies, keyring entries, private API auth, or the owner's password. Do not ask the owner to share secrets. Only permissible observations are sanitized metadata, effective account *presence/absence* where already known, process owner, permitted relevant error lines and nonsecret path identity.
5. **Produce one actionable verdict**: (a) confirmed root cause + exact minimal safe remedy; (b) leading hypothesis and one discriminating next check; or (c) not reproducible / evidence insufficient. Say which launch failed and which user/session it actually ran under; if unable to establish, state not verified. Do **not** say a refreshed licence proved the worker can now launch.
6. **Publish the result** as exactly one NEW GitHub Markdown document at the path above, link sanitized evidence references, state no native Editor run or source edits; comment `[HANDED-OFF]` on #826 and leave Manager #824 to issue disposition.

### Explicit prohibitions

No Unity Editor launch (including batchmode), no Hub activation/sign-in, no licensing-service state mutation, no credential/token access, no `unity-lock` acquire/release/force, no changes to CCF-Ops `lanes.json`/board/deployment files or systemd/DBus config, no #820 checkout writes, no gameplay/source/save/scenes/assets changes, no Windows build and no main merge. This is a diagnostic, not an attempt to fix the machine by changing user settings.

## Phase B — subsequent Manager/physical gate; NOT AUTHORISED by this packet

After reading the #826 result, Manager #824 must approve a narrowly scoped remedy or a specific remaining check. **Before any Unity launch**, a real native worker must independently refresh the machine-wide Editor status, actual worktree/branch/source scope, and lawful **exclusive Unity lease under owner's D-091 no-arbitrary-expiry policy**. The previous worker's one-off +6h lease is already released and cannot be silently reused as standing authorisation. #652's no-expiry patch is not yet shown deployed, so do not assume it is installed; escalate only the precise unresolved lease question if it persists.

Then resume **existing #820**, NOT a new source implementation: Unity 6000.6.0f1 import, absence of Editor compilation/missing-script errors, arrival/assessment gates, player mark three Crop Trees, genuinely purchase two Oak + two Beech and place four G1 pending orders, Work Plan approval/cancel, year advance, linked juveniles/finance, covering Annual Review/CT-3 chronology, S1-C history/later intervention or no-work, cosmetic outside shells excluded from simulation, and historical five-rack/Legacy40/Reference Future regression. Publish Tier2 pass/fail/not-run with actual screenshots/events; do not claim E1 owner visual acceptance or Compact save/M-A completion from these tests.

## One paste-ready native dispatch line

> **Sen-1 #826 — diagnose why the Scenario One #820 Unity Editor launch failed despite the owner's working desktop Unity session.** Execute **only Phase A read-only** from this full handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-826-unity-launch-diagnostic-20261119-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-826-UNITY-LAUNCH-CONTEXT-READONLY-DIAGNOSTIC-HANDOFF-20261119.md . Register your actual model/provider, host, docs worktree/branch and starting HEAD on issue #826 before result writing. Inspect failing invocation vs owner's interactive session with *sanitized* process/environment/log data and zero credential access. Do NOT launch Unity, acquire a lock, edit source or touch Ops. Publish exactly one diagnostic Markdown result at the specified path, update #826 to HANDED-OFF and return its GitHub link.

**Required user action:** paste the dispatch line into a **native laptop T3 Code/OpenCode** chat that can inspect the actual host. No further Unity licence activation, manual login, or Windows testing requested. Dashboard/Drive sync: NOT VERIFIED.
