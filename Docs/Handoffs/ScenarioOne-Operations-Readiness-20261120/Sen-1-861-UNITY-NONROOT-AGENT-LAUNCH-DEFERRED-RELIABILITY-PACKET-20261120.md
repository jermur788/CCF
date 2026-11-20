# Sen-1 #861 — unattended Unity launch without running as root

**20 November 2026, Owner-requested long-term reliability work · Issue [#861](https://github.com/jermur788/CCF/issues/861) · BACKLOG / NOT RUNNING · No interruption to ongoing #857**

## Owner observation and objective

The owner twice encountered a modal Unity warning that the Editor is running as **root** and manually clicked to allow a currently active agent task to continue. They want an unattended solution, ideally avoiding root entirely, or authorised agent mouse interaction if there is truly no alternative. They explicitly do **not** want the running work interrupted just to investigate this problem.

**Recommendation:** The long-term fix is **non-root Unity GUI/batch launches under the normal actual desktop user and authenticated Unity Hub Flatpak session**, not unattended acknowledgement of a privileged-process warning. The exact cause of these two most recent prompts is **NOT VERIFIED**: no current process UID/launch command or window screenshot has been inspected in this browser-only management pass.

Do **not** confuse a worktree located on the root Linux filesystem with a Unity process launched with effective UID 0. This packet concerns the latter.

## Existing evidence: reuse, don't repeat broad diagnostics

- [#829 laptop/Hub context](https://github.com/jermur788/CCF/issues/829) already diagnosed that an earlier **same-user** failed Unity launch from the host lacked access to the authenticated Flatpak Hub licensing-client context. It did NOT demonstrate that root privilege is required; #830 later exercised the correct Hub sandbox.
- [#830 Flatpak Unity import and 71/71 targeted gameplay qualification](https://github.com/jermur788/CCF/issues/830) is historical evidence that an authorised laptop Unity session could work through Hub sandbox with actual physical permission. This is not a standing lease, today's UID proof or proof of a root launch.
- [#652 Ops lease candidate](https://github.com/jermur788/CCF/issues/652), [#548 historical identity issue](https://github.com/jermur788/CCF/issues/548), and [#802 lock-copies task](https://github.com/jermur788/CCF/issues/802) are separate physical custody/lock concerns. Do not bootstrap an unrelated Ops deployment or force release to solve a GUI warning.
- [Unity Hub official Linux installation guidance](https://docs.unity.com/en-us/hub/install-hub-linux) describes non-root-capable Flatpak/AppImage installation; [Unity Editor CLI docs](https://docs.unity.com/en-us/engine/6000.6/manual/get-started/install-and-upgrade/getting-started-installing-unity/install-unity-with-installers) provide supported non-GUI launch possibilities. Exact local behaviour needs host proof.

## Future one-parent bounded execution: no current child or start

**Trigger:** after #857's current source/Unity phase concludes and owner/manager selects a quiet window, OR if a newly observed root warning actually blocks an authorised task. If a current task is blocked, stop **only that Unity invocation**, not unrelated authorised code/docs/review work.

1. **G0 diagnostic / read-only, on actual jer-laptop**: under one named approved worker, read live main and [active-tasks.md](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), current Ops physical lock and source ownership, actual process parent/effective UID for Unity/Hub and launch method, current graphical XDG_RUNTIME_DIR / DBus / display context **as sanitized categorical facts**, and file owner/permissions for *specific* affected checkout/project cache paths. Verify whether the agent runs as root, enters a root container, or starts a root GUI due to sudo. Check existence/identity of Hub sandbox. No raw tokens, credentials, license XML, cookies, account data or full environment dumps in issues. No Unity launch needed for G0.
2. **Minimal proposed fix, subject to exact Ops/source G4 permit:** make the task launcher invoke Editor through the normal *logged-in* user session and authenticated Hub context with only necessary writable project/cache paths; no blanket privilege/ownership conversion. If the executor is itself root, create an explicitly authorised user-session delegation mechanism rather than guessing display variables or invoking Unity with sudo. A narrowly scoped permission repair must have owner consent for exact paths/UID; never recursively chown entire workspaces or alter shared installed Ops files without their custodian.
3. **Qualification, separate real exclusive Unity lease:** on host, verify the process has non-root effective UID, Editor import/load and licensing succeed, the warning is absent on two ordinary launches, source/game files remain normal-user-owned, existing 3D app/history unaffected, worker can interact with ordinary GUI; finish with normal owner release. Readback/log exact result and prove no root-created temporary project files.
4. **Fallback only if necessary:** ordinary GUI automation may be approved for *nonsecurity* interactions, in its exact desktop session, but **do not grant an agent generic permission to click privileged/root warnings**. If the elevated-process warning persists, treat that as a launch problem to fix or a clearly documented owner-only exception rather than silently clearing a security guard.

**Excluded until separately approved:** Unity installation replacement, licensing resets, credentials, OS-wide permissions, changing Ops/physical lock scripts, new GUI automation framework, broad root cleanup, arbitrary lease/lock cleanup, Windows build, code/Assets/ProjectSettings or game-source changes.

## Ownership, next step and truthful status

Future actor: one native Unity/desktop launch maintainer under [Scenario One Manager #844](https://github.com/jermur788/CCF/issues/844) and Overall #425, with #652 Ops custodian involved only if the exact launcher/physical file overlaps. Actual provider, host, branch/worktree, exact read/write file list, no-conflict lock and physical grant are **UNKNOWN / not claimed**. Read-only diagnostic can be packet-class; persistent launch-policy work or shared Ops change requires specific approved scope, independent review/rollback when warranted.

**Owner action now: NONE.** Let current [#857 template orchestrator](https://github.com/jermur788/CCF/issues/857) continue. When real worker registration/physical conditions allow, execute this one bounded follow-up and publish one new GitHub Markdown result.

**Checks at this publication:** GitHub source issue/history and official public Unity Hub information only. **NOT RUN:** process UID inspection, mouse interaction, Unity launch, filesystem/permission changes, source/installed Ops edits, native qualification, lease acquisition, or warning reproduction.
