# CCF-Ops foundation — G4 corrections

Versioned source: CCF repository `CCF-Ops/` (verify source branch/HEAD against the deployment receipt before updating). Live deployment: `/media/jer/ZX20/CCF-Ops`. The deployment is an explicit copy, not a second repository or a symlink (ZX20 is exFAT). `deployment.json` records source commit and file hashes. Update scripts/roles/config deliberately from a reviewed commit; never overwrite the live board, ledger or state with a blanket recopy. Initial board is a source snapshot; the deployed board is the operational live copy. Reconcile with authoritative active-tasks before dispatch.

Persistent lanes / disposable threads. User → ChatGPT Overall Manager → mechanical checks → thin T3 Execution Manager → disposable workers. T3 does not autonomously create threads. Scope/authority comes from Manager packets. Until a later canonical context update wires role persistence into project entry files, dispatch must supply `ROLE FILE` plus the explicit task packet. The first-message reminder is reinforcement only; the complete role-loading chain is not operational yet. Role files grant no independent scope or acceptance authority.

## Storage and lanes

ZX20 is UUID `18D6-896E`, exFAT, mounted at `/media/jer/ZX20`. Verify it; do not assume `/media/jer/Files` is ZX20. Machine-local lanes.json stores device, UUID, filesystem and a 15 GiB minimum free-space threshold; these identifiers are not canonical workflow policy. Source `environment.sh` in a dedicated task shell to validate device identity, writable (`rw`) mount and sufficient free space and set ZX20_ROOT, TMPDIR/TMP/TEMP and XDG_CACHE_HOME. Scripts use Python standard library, Git, findmnt and du. No subscriptions, credentials or downloads are configured.

- U1: primary Unity; prepared at supplied context, Library warm-up deferred.
- U2: secondary Unity; prepared at supplied context, Library warm-up deferred.
- Integration: clean, single integrator; `unity: false`, no Editor by default; no accepted branch is changed automatically.
- NonUnity: sparse offline tools/tests/docs; do not launch Unity here.

U1/U2 have `unity: true`; Integration/NonUnity have `unity: false`. Below the configured threshold, status reports BLOCK / LOW SPACE and no new substantial lane/cache/build work is allowed.

The canonical common repository is `/media/jer/Files/CCF/Repository.git`; its preserved `core.fsync=all` setting improves durability but is not a backup. Handoffs require a pushed remote branch or explicitly recorded bundle on Files.

Unity lanes and their Library/cache remain on ZX20. Durable builds, archives and verification output belong under `/media/jer/Files/CCF/{Builds,Archives,Verification}`; scratch stays in ZX20/CCF-Temp. environment.sh checks scratch and bounded durable output directories before exporting cache paths. Do not recreate retired ZX20 Builds/Verification directories.

## Dual Git stores and assignment

The canonical Files store and legacy `/media/jer/Files/CCF-Legacy/.git` coexist. Origin is the exchange boundary. Inspect both configured stores before assigning a writable branch; unreadable stores, competing checkouts, dirty lanes or invalid boards block assignment. Never switch another owner's checkout. Non-Unity Files inventory is read-only; Unity admission remains ZX20-only.

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

lane-status never fetches, refreshes the Git index, prunes, changes refs or modifies a lane. Normal inventory returns 0 for missing/dirty/conflicted lanes and displays warnings. Storage readiness failure returns 1; explicit assignment `--preflight` also returns 1 for a blocked assignment. `--storage-only` reports actual free bytes/GiB, threshold and PASS/LOW SPACE. Actual command/config/board errors return 2. Disk is approximate `du` allocation (exFAT overhead included); ahead/behind is local refs only, no network fetch. Every row reports backing device and explicit ZX20: YES/NO. Configuration rejects arbitrary roots (including /), sibling paths and resolved symlink escapes outside ZX20 or the bounded Files CCF tree with exit 2 before inventory. Default configuration readers admit registered, resolved bounded Files CCF lanes only when unity:false (owner-approved Option A). Inventory reports ZX20: NO and retains repository identity checks; this grants no writable or Unity capability. NonUnity acquire rejects before creating state or a guard. Unity lanes and explicit writable preflight remain ZX20-only. Config is lanes.json; malformed paths/duplicates/board/mount identity fail clearly. Missing paths are allowed; unexpected repositories are reported without changing them.

packet-lint: PASS=0, WARN=0, FAIL=1, tool error=2. Explicit `--draft` downgrades missing fields to WARN; invalid explicit values still FAIL. Field syntax: `KEY: value`, multiline values, or Markdown heading followed by value. Required: routing, role, ROLE FILE (reference or NONE), context, base, branch, scope, exclusions, ownership, verification tier (1/2/3), time budget, question/acceptance criterion, stop conditions, handoff format. Full SHAs for context/base. Declare `Unity required: Yes/No`; omission FAILS a non-draft packet. Explicit Unity ownership/lane fields also establish Unity mode; generic Lane, HEAD or Worktree fields do not. `No` plus Unity-specific ownership/lane is contradictory. Canonical aliases such as ROUTING, WORKTREE, BASE, STOP AND ASK IF and HANDOFF are accepted. Empty/none/n/a/- placeholders cannot satisfy substantive fields, including acceptance, stops and handoff (ROLE FILE: NONE is explicitly permitted). Unity additionally requires exact HEAD, a single named Unity owner, an explicit Unity lane, allowed phases, Player/native input Yes/No, and stop conditions. The Unity lane must be configured and marked Unity-capable. Duplicate fields/owners fail. This validates structure, not natural-language truth, permission or acceptance. Fixtures in tests/fixtures.

## Machine-wide Unity ownership

The production state path is fixed at `/media/jer/ZX20/CCF-Ops/state/unity`, across all lanes, source checkouts and clones. No environment variable redirects it. Linux flock on a permanent guard serializes record updates; same-directory rename publishes complete metadata. Ownership persists after the CLI exits. A second acquire cannot replace any existing record, including stale/malformed metadata. No automatic process kill/release.

```bash
scripts/unity-lock acquire --owner OPERATOR --task-id TASK   --lane /media/jer/ZX20/CCF-U1 --branch lane/u1   --head FULL_SHA --no-expiry --notes 'authorised gate'
# Add --pid PID only when known and alive. Save the returned lock_id.
scripts/unity-lock release --owner OPERATOR --task-id TASK --lock-id RETURNED_ID
scripts/unity-lock release --force --owner INSPECTING_ACTOR --reason 'documented stale-state inspection' --manager-approval 'REFERENCE_TO_EXPLICIT_OVERALL_MANAGER_APPROVAL'
```

Acquire fails closed unless the configured lane exists, is Unity-capable, belongs to the configured Git store, is clean, and its actual Git branch/HEAD exactly match the supplied values. It also checks both stores for branch checkout conflicts. It never changes the worktree to match metadata. status shows PID liveness/start ticks/boot identity, ETA overdue and actual running Editor PIDs. Unknown PID means manual inspection, not stale. Existing unclaimed Editor PIDs reject acquisition. Normal release needs owner+task+lock ID and refuses while any Unity Editor or the recorded PID still exists, including a reused PID; inspect and verify safe process exit first. With an unknown PID the operator must additionally verify that the worker/session has stopped writing. Force requires prior explicit Overall Manager authorisation, its reference via `--manager-approval`, explicit flag/actor/reason, and appends the complete previous record (or raw malformed record) to history.jsonl **before** deleting the lock. The approval reference is an auditable cooperative declaration, not cryptographic proof or self-authorisation. Never delete the guard/history manually. The lock coordinates cooperative users on this Linux machine; it is not a security boundary, Unity launcher or supervisor. Test override `--test-state-dir` is restricted to ZX20/CCF-Temp and must never be used operationally. Old launchers need a later explicit adoption packet; installing this foundation does not make them lock-aware.

Owner D-091 custody has no elapsed-time cutoff. Acquisition must explicitly choose `--no-expiry` or the mutually exclusive legacy `--eta FUTURE_ISO_TIMESTAMP_WITH_TIMEZONE`. The new mode writes `eta:null` with `no_expiry:true`; an unmarked null, missing ETA or inconsistent mode is malformed and remains blocking. Legacy timestamp records remain readable without migration and an overdue estimate is advisory only. Neither mode expires, kills a process, reclaims a stale heartbeat or transfers custody automatically. An unresponsive worker remains owned until investigated and explicitly handed off or safely released; do not manufacture a far-future timestamp.

Safe handoff is event-driven: record the actual worker/session, scope, lane, branch/HEAD and source status; confirm no worker can continue writing and the Editor/recorded process has exited; preserve evidence and release with matching identity. A custodian reconciles assignment fields under the existing board-directory flock. `HANDED-OFF`/`RELEASED` belong in receipts/ledger, not as new board states: a `DONE-PENDING-LEDGER` row still reserves its Lane/Branch until deliberately cleared or moved to the ledger. Retaining a worktree confers no writer permission. Never migrate live held lock JSON in place. Old scripts cannot read the new marked null record: rollback requires safe explicit release first or an independently reviewed compatible fallback, never deleting ownership to make rollback pass.

## Post-integration Unity gates

Integration produces the candidate HEAD → Unity Operator reserves a clean, available U1/U2 lane and checks/places that exact HEAD there → verifies branch/HEAD and ownership → acquires the lock → performs only authorised gates → returns receipts tied to the exact candidate HEAD. Integration does not host the Editor by default. Stop on dirty/owned lanes. Prefer a detached candidate HEAD in the Unity lane (`--branch HEAD`) rather than checking out the active integration branch twice; branch-based placement needs a separate, non-conflicting lane branch and explicit packet. No automatic checkout or Unity launch occurs in these scripts. See pilot/unity-gate-example.md.

## Board, ledger and authority transition

board/active.md has only actionable state, one row per live task and all required fields: Task ID, State, Owner, Harness, Model, Quota source, Lane, Branch, HEAD, Subsystem, Locks, Unity required?, Unity queue position, Verification tier, Open gate, Dependency, Next action. Use None/N/A explicitly. IDs unique; duplicate live Lane or Branch assignments are flagged by packet-lint --board and lane-status/preflight (None/N/A placeholders are unassigned). Append-only board-change history is deferred T2 work: there is no reviewed board writer/authority transition yet; adding one here would enlarge this correction task. HEAD exact or TBD only before assignment. States: READY, ACTIVE, WAITING, BLOCKED, VERIFY, INTEGRATE, DONE-PENDING-LEDGER. Unity queue position is an integer for queued work, None otherwise. Do not claim someone else's files. ledger/ stores compact closed summaries. Role/packet rules and mechanical checks do not change gameplay canon.

Current authoritative coordination remains `Docs/Project/coordination/active-tasks.md` on the designated integration/main coordination branch. A board update cannot confer authority until that file/packet locks are reconciled by its authorised maintainer. See ActiveTasksTransition.md and Docs/Operations/AgentWorkflowProposedChanges.md. G4 is required before canonical AgentWorkflow changes.

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

Reviewer A/B/C is a per-packet assessment, mapped separately to existing canonical integration decisions; see roles/reviewer.md. Existing task states and ledger verdicts remain unchanged. Runtime updates preserve board, ledger and Unity lock state; no blanket deployment copy is used.

Option A authority: the owner-approved `Sen-1-652-OWNER-APPROVED-OPTION-A-NONUNITY-DEFAULT-ADMISSION-AMENDMENT-20261116.md` supersedes only the historical phrase “default callers remain ZX20-only” in DeploymentProvenance.md. That historical file is retained unchanged; Unity admission, physical lock state and writable preflight remain verified ZX20-only.
