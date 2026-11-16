# Sen-1 Overall Manager — pre-dispatch readiness and complete authorisation checklist

**Owner direction accepted 2026-11-16:** stop requiring agents to discover missing approvals after being dispatched. #642 and #647 encountered missing live and physical ownership despite otherwise complete engineering packets. Manager #640 owns prevention; this document supplements, not replaces, canonical live ownership or future owner instructions.

## Binding operating rule

**Prepare → reserve → verify → dispatch.** Before presenting an executable implementation handoff to the user, Manager must make a best effort to **complete all prerequisites under Manager control**, route other prereqs to authorised maintainers, and read back resulting evidence. A technically complete packet is not **dispatch-ready** unless its external permissions are also present.

### Dispatch readiness gate — all required items

1. **Product/spec authority:** cite specific accepted owner decision and approved (not draft) spec or bounded packet. Record exact allowed feature state, preserved compatibility and unresolved decisions. Do not start code from draft-only requirements.
2. **Execution claim:** create substantive task GitHub issue; state proposed worker/model/provider, best-suited capability, Medium/High/Low reason, exact result handoff and branch/worktree/starting source candidate (if not yet physically created, label proposed). Do not claim another worker RUNNING.
3. **Canonical GitHub custody:** refresh main coordination and conflicting issue claims. **As sole authorised board writer**, proactively reserve exact paths, target branch, task status and one prospective writer; commit and read back the GitHub board before dispatch. If another person is current board writer, obtain their prospective transfer or have them perform the reservation before dispatch. No backdating.
4. **Physical Ops custody when required:** separate from GitHub. When execution packet requires the deployed physical Ops board, **schedule and complete its registration first**, with real maintainer authority, exact 17-column schema, original validator PASS, diff, physical hash/readback, active workers and lane status. A validator PASS without exact task record is insufficient. If physical reservation cannot be verified, do not issue source-execution handoff as READY.
5. **Machine/Unity lease if relevant:** verify deployed/self-test-green #548 lease, real actual owner, exclusive lock, source candidate and isolated Library/Prefs/saves. A machine-gated task is not RUNNING and must not be presented as immediately executable. No Windows Player build before user playtest and express permission.
6. **Source safety:** inspect current branch/remote head, permitted worktree and dirty state whenever remote visibility allows; distinguish GitHub-visible evidence from worker-only physical evidence. Verify no owner/file overlap, identity of specific source commit, exact allowlist and protected dirty files.
7. **Dispatch payload:** include one direct permanent GitHub URL to the complete packet; ownership approvals/readbacks; source candidate; actual/proposed lane, branch and worktree; authorised file list; safe allowed action; bounded tests; stop conditions; result GitHub Markdown exact path; real model/provider expectations; precise user action. Give paste-ready text in normal selectable form.
8. **Final worker preflight:** worker still rechecks mutable board/branch/locks and registers actual [RUNNING] with real identity, worktree and HEAD BEFORE editing. This is a brief confirmation of already-granted authority, not a substitute for preparing it. If facts changed, stop rather than override.

**Status rule:** READY TO DISPATCH only when each applicable external prerequisite has a source/readback URL or is demonstrably not required. If any is absent, label HOLD/BLOCKED, commission prerequisite as its own packet and **do not send the implementation chat a start command**. A prereq maintainer may be dispatched separately, with truthful READY state. Avoid circular self-certification or pretending that a GitHub issue/board grants local physical ownership.

## Application to existing flow

- #642 was blocked because its exact two-fixture GitHub reservation was omitted before source-worker dispatch. Manager later wrote it; this should have happened first.
- #647 was blocked because the physical Ops board had no #647 row although GitHub custody existed and original physical linter passed. #648 is the **prerequisite** physical reservation task, not #647 source execution. Until #648 returns actual physical PASS and #647 rechecks source exclusivity, source #647 remains BLOCKED/NOT RUNNING.
- #645 independent browser source review did not require physical file or Unity lane, so a correctly registered docs-only reviewer could execute directly.
- Never relabel source incorporation as Unity verification, or runner-preflight as integration completion.

## Completion reporting

After each substantial handoff, state what changed, passed/failed, what that means, next action, user action/none and dashboard/Drive sync if relevant. The Manager should carry routine coordination cost, rather than delegating preventable approval discovery to code workers.

**Implementation of this rule:** prospective from 2026-11-16; no approval is backdated. Canonical current decisions, approved specifications, active-tasks.md and physical authority remain higher priority.