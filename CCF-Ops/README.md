# CCF-Ops foundation v1

Versioned source: CCF repository `CCF-Ops/` on branch `ops/workflow-foundation-v1` in `/media/jer/ZX20/CCF-NonUnity`. Live deployment: `/media/jer/ZX20/CCF-Ops`. The deployment is an explicit copy, not a second repository or a symlink (ZX20 is exFAT). `deployment.json` records source commit and file hashes. Update scripts/roles/config deliberately from a reviewed commit; never overwrite the live board, ledger or state with a blanket recopy. Initial board is a source snapshot; the deployed board is the operational live copy. Reconcile with authoritative active-tasks before dispatch.

Persistent lanes / disposable threads. User → ChatGPT Overall Manager → mechanical checks → thin T3 Execution Manager → disposable workers. T3 does not autonomously create threads. Scope/authority comes from Manager packets. Read the relevant roles file.

## Storage and lanes

ZX20 is UUID `18D6-896E`, exFAT, mounted at `/media/jer/ZX20`. Verify it; do not assume `/media/jer/Files` is ZX20. Source `environment.sh` in a dedicated task shell to validate the mount and set ZX20_ROOT, TMPDIR/TMP/TEMP and XDG_CACHE_HOME. Scripts use Python standard library, Git, findmnt and du. No subscriptions, credentials or downloads are configured.

- U1: primary Unity; prepared at supplied context, Library warm-up deferred.
- U2: secondary Unity; prepared at supplied context, Library warm-up deferred.
- Integration: clean, single integrator; no accepted branch is changed automatically.
- NonUnity: sparse offline tools/tests/docs; do not launch Unity here.

Each lane has a distinct branch and common Git store on ZX20/CCF-Repository.git. No per-task worktree creation. Reuse lanes only after checking ownership/dirty state. No Library copying. All future Unity caches stay under their ZX20 lane. Builds/staging/archives: CCF-Builds. Large verification receipts, CSVs, captures and profiling: CCF-Verification. Scratch and tool caches: CCF-Temp. Existing SSD and Files-device material is inventory only; migration is separately authorised.

## Commands

```bash
source /media/jer/ZX20/CCF-Ops/environment.sh
/media/jer/ZX20/CCF-Ops/scripts/lane-status
/media/jer/ZX20/CCF-Ops/scripts/lane-status --json
/media/jer/ZX20/CCF-Ops/scripts/packet-lint packet.md
/media/jer/ZX20/CCF-Ops/scripts/packet-lint --draft draft.md
/media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md
/media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
```

lane-status never fetches, refreshes the Git index, prunes, changes refs or modifies a lane. Missing/dirty/conflicted lanes return 0 and display warnings. Actual command/config/board errors return 2. Disk is approximate `du` allocation (exFAT overhead included); ahead/behind is local refs only, no network fetch. Config is lanes.json; malformed paths/duplicates/board/mount identity fail clearly. Missing paths are allowed; unexpected repositories are reported without changing them.

packet-lint: PASS=0, WARN=0, FAIL=1, tool error=2. Explicit `--draft` downgrades missing fields to WARN; invalid explicit values still FAIL. Field syntax: `KEY: value`, multiline values, or Markdown heading followed by value. Required: routing, role, context, base, branch, scope, exclusions, ownership, verification tier (1/2/3), time budget, question/acceptance criterion, stop conditions, expected handoff. Full SHAs for context/base. Declare `Unity required: Yes/No`; omission warns. Unity additionally requires exact HEAD, a single named Unity owner, allowed phases, Player/native input Yes/No, and stop conditions. Duplicate fields/owners fail. This validates structure, not natural-language truth, permission or acceptance. Fixtures in tests/fixtures.

## Machine-wide Unity ownership

The production state path is fixed at `/media/jer/ZX20/CCF-Ops/state/unity`, across all lanes, source checkouts and clones. No environment variable redirects it. Linux flock on a permanent guard serializes record updates; same-directory rename publishes complete metadata. Ownership persists after the CLI exits. A second acquire cannot replace any existing record, including stale/malformed metadata. No automatic process kill/release.

```bash
scripts/unity-lock acquire --owner OPERATOR --task-id TASK   --lane /media/jer/ZX20/CCF-U1 --branch lane/u1   --head FULL_SHA --eta FUTURE_ISO_TIMESTAMP_WITH_TIMEZONE --notes 'authorised gate'
# Add --pid PID only when known and alive. Save the returned lock_id.
scripts/unity-lock release --owner OPERATOR --task-id TASK --lock-id RETURNED_ID
scripts/unity-lock release --force --owner INSPECTING_ACTOR --reason 'documented stale-state inspection'
```

Verify branch/HEAD independently before acquire; metadata is a declaration, not an execution gate. status shows PID liveness/start ticks/boot identity, ETA overdue and actual running Editor PIDs. Unknown PID means manual inspection, not stale. Existing unclaimed Editor PIDs reject acquisition. Normal release needs owner+task+lock ID; force needs explicit flag/actor/reason and appends the complete previous record (or raw malformed record) to history.jsonl **before** deleting the lock. Never delete the guard/history manually. The lock coordinates cooperative users on this Linux machine; it is not a security boundary, Unity launcher or supervisor. Test override `--test-state-dir` is restricted to ZX20/CCF-Temp and must never be used operationally. Old launchers need a later explicit adoption packet; installing this foundation does not make them lock-aware.

## Board, ledger and authority transition

board/active.md has only actionable state, one row per live task and all required fields. Use None/N/A explicitly. IDs unique; HEAD exact or TBD only before assignment. States: READY, ACTIVE, WAITING, BLOCKED, VERIFY, INTEGRATE, DONE-PENDING-LEDGER. Unity queue position is an integer for queued work, None otherwise. Do not claim someone else's files. ledger/ stores compact closed summaries. Role/packet rules and mechanical checks do not change gameplay canon.

Current authoritative coordination remains `Docs/Project/coordination/active-tasks.md` on the designated integration/main coordination branch. A board update cannot confer authority until that file/packet locks are reconciled by its authorised maintainer. See Docs/Operations/ActiveTasksTransition.md and AgentWorkflowProposedChanges.md. G4 is required before canonical AgentWorkflow changes.

## Route identity placeholders

Always record `harness / model / quota source`; verify actual availability and ownership at dispatch. Examples supplied by Manager, not installed/availability claims:

- Claude Code / Sonnet 5.5 xhigh / Claude A
- Claude Code / Sonnet 5.5 xhigh / Claude B
- OpenCode / GPT-6.1 Sol UltraFast / OpenAI allowance
- OpenCode / opencode-go:<model> / Go allowance
- Codex / GPT-6.1 Sol / OpenAI allowance

Claude A/B selection is an explicit Manager pool assignment, never silent account rotation. No API keys, OAuth tokens or secrets belong in board/config/ledger/receipts.

## Offline checks / pilot

`python3 -B tests/self-test.py` creates fixtures only under ZX20/CCF-Temp. It tests clean/dirty/missing lanes, config errors, packet PASS/FAIL/WARN, board/ledger schemas, lock acquire/reject/release/stale/force/malformed handling and concurrent acquisition. No Unity launch. Full receipt belongs in CCF-Verification/WorkflowFoundation. `pilot/manager-packet.md` and `pilot/README.md` describe an isolated, simulated Tier-1 chain; tests write its disposable board, handoff and ledger to scratch. No real worker thread or production gate is implied.
