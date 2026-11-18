# Sen-1 #733 — installed Ops no-expiry custody audit
Status: **REQUIRES PATCH AND REVIEW — NEEDS CHANGES for operational compatibility**. Read-only inspection completed; no implementation or deployment authority exercised. Manager #731 acceptance remains pending.

## Registration and scope
Registered RUNNING before physical Ops inspection: https://github.com/jermur788/CCF/issues/733#issuecomment-6977969117.
Actual worker: OpenAI/Codex, gpt-6.1-sol medium, native T3 Code; session 9bcdfde6-699b-41f7-befb-acc9a7551f09; hostname jer-laptop. Suggested GPT-6.2 High was not claimed.
Starting physical checkout /media/jer/Files/CCF-Primary, ops/workflow-foundation-v1, HEAD 46247486b37904f6b36825bb40fb6ed7bea35e88, observed 2026-11-18T12:01:08Z. Pre-existing M AGENTS.md, M Docs/Operations/StorageArchitecture.md and untracked Docs/Operations/StaleWorktreeDiskPressureHandoff-20261118.md preserved; same status at readback.
GitHub-only documentation branch docs/sen-1-733-ops-no-expiry-readonly-20261118-v1 created from remotely verified main 52fd44b3ab56a3fa71ef672a82a56011a50f083c. No physical source/Unity lane claimed. Evidence is this document only, with no local evidence directory or file writes.

Authority: complete dispatch blob 4e3a33f885dbad2ff080ec8fad1f48d807830686 and D-091 read from main 52fd44b3. D-091 requires explicit release/cancellation/handoff or real conflict/safety stop, with no timer stoppage or silent stale reclaim; existing leases require custodial reconciliation rather than implicit alteration.

## Actual installation and contract
Installed copied deployment: /media/jer/ZX20/CCF-Ops, not assumed to be a Git checkout.
Configuration: lanes.json, schema 1; physical board: board/active.md; Unity record: state/unity/lock.json; permanent guard: state/unity/guard; append-only lock history: state/unity/history.jsonl.
Original controls inspected in full: scripts/_ccf.py, scripts/lane-status, scripts/packet-lint, scripts/unity-lock. Supporting README.md, tests/self-test.py relevant lifecycle cases and deployment.json inspected.
Manifest schema 2 describes RECONCILED_OBSERVATION_NOT_SOURCE_ACCEPTANCE, historical source dfe445e54d099a0e20c9c7d5a9e3e6f195968b4e. It records a separate candidate 60f97b06bd41f9a89fde4eb775527ebc6947560b as NOT_DEPLOYED. This is manifest information, not verification of that candidate's present checkout or approval. #652 currently names a later source candidate; no equivalence inferred.

1. Lane/board expiry: no structured expiry property is required or parsed by config_read or board_read. Dates appear in purpose/Locks/Next action free text. Missing expiry is not a broken schema row; adding null to an unvalidated key would prove nothing. Neither original lint nor preflight interprets those dates, stops a process or reclaims a lane. Human release-at-expiry instructions still conflict with D-091 and need explicit custodial reconciliation.
2. Unity ETA: read_state requires eta as a nonempty string and timezone-aware ISO timestamp. Missing, null or indefinite text is malformed. Acquire requires a future ISO --eta. inspect merely reports eta_overdue; no elapsed-time release or process kill exists. Existing records, even stale/malformed, block acquire. Thus ownership is already persistent but acquisition cannot express an honest no-estimate/no-expiry contract. No distant date workaround is acceptable.
3. Identity: process_identity uses PID, boot ID and /proc start ticks. Absent/reused PID is stale, not automatically released; unknown PID requires manual inspection. Acquisition checks actual Editors and lane cleanliness/branch/HEAD/store/dual-store branch conflicts. Normal release checks owner/task/lock ID and appends history before unlink, but does NOT refuse a still-running Editor; termination must be verified before release. The status CLI enters guarded(), opening guard a+ (and can mkdir); it was deliberately NOT executed in this strict no-write audit. Raw lock absence and /proc inspection substitute only for observation, not a claimed native status PASS.
4. Preflight reads both configured Git stores, board duplicates, storage identity/free capacity and actual lane Git state. command() disables Git optional locks and bytecode writes. READY requires ZX20, existence, canonical-store registration and no warnings. It does not consult Unity lock, a heartbeat or session-start permit. Board and source/Unity custody checks therefore remain separately mandatory. It inventories every configured lane even when selecting one.
5. A retained worktree has no intrinsic expiry in Git or these validators. A safely stopped lane can remain indefinitely without granting write permission, subject to explicit storage/retention disposition and capacity checks. D-091 forbids timer eviction; it does not grant unlimited SSD storage or override real safety constraints. Retained branch checkouts still block competing writers. Existing schema counts every table row for Lane/Branch duplicate checks, including DONE-PENDING-LEDGER: release of an assignment must clear those fields or move its closed record to ledger, not invent unsupported RELEASED/HANDED-OFF table states.

## Existing custody stays unchanged
Physical CCF-690-SEN1-SAVE-A0 row: BLOCKED, prospective Claude Opus 5.5 session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee, SOURCE START HELD, lane Sen1SaveA0ZX20NonUnity707, branch integration/sen-1-d073-shared-save-a0-20261117-v1, HEAD 55863f1b7719011bba16b3f3ab377e379c0b988a. Earlier advisory cutoff 2026-11-19T00:00:00Z remains physically recorded; no authority or lease transferred here.
Multi physical rows include retained #710 R custody (no active writer; recorded extension to 2026-11-23T18:00Z), #725 implementation handed off / VERIFY with recorded 2026-11-20T18:00Z retention, and #732 qualification slot WAITING / issued not running with recorded 2026-11-20T18:00Z cutoff. #730 remains their Manager. These are observed board declarations, not independent verification of other workers' worktrees or live sessions. Their separate owners must reconcile the dates and release language under their authority; #733 does not change them.
Compact30 prospective /media/jer/ZX20/CCF-Sen1-718-Compact30-Content-20261117 is not in observed lanes.json. There is no Compact30 preflight READY or source/Unity start approval from this audit.

## Minimal bounded follow-up (proposal only)
Create a separate Manager-authorised Ops implementation + nonauthor review + controlled deployment packet. Do not expand #733.
Source ceiling for no-expiry support: CCF-Ops/scripts/unity-lock, CCF-Ops/tests/self-test.py (and focused fixtures only if needed), CCF-Ops/README.md and CCF-Ops/roles/unity-operator.md. Deployment manifest/provenance updates belong solely to controlled installation. Preserve _ccf.py, lane-status, packet-lint and their schema/conflict/storage gates unless a separately demonstrated defect requires an approved expansion.
Recommended interface: explicit --no-expiry, mutually exclusive with optional --eta advisory estimate; write eta:null only through this new reviewed mode. Validate/inspect null deliberately; retain legacy valid timestamp reading and advisory-overdue display. Missing/corrupt custody fields remain fail-closed. No new timeout, stale reclaim, guard path, lock universe, force shortcut or automatic cleanup. Preserve identity checks and single machine-wide Unity lock.
Tests required in isolated approved scratch: null-mode acquire/status/reject-second/release; legacy timestamp and overdue lock still held; malformed/missing metadata refused; stale/reused PID never reclaimed; wrong owner/task/lock rejection; concurrent acquisition exclusion; original storage/device/symlink/dirty/branch-conflict failures; complete original suite. No suite was run in this audit. Test creation must follow current storage policy and the reviewed scratch constraints rather than invent a new production state root.

Event-driven custody procedure:
- Custodian registers task, actual provider/model/session/host, explicit scope, one lane/path/store/branch/exact HEAD, owner and separate Unity permission. Until explicit safe release, ownership has no expiry and no heartbeat forfeiture.
- Under exclusive flock on the existing /media/jer/ZX20/CCF-Ops/board directory, reconcile named assignment/authoritative record; preserve other rows and record prior hashes. Independent source and Unity locks remain separate.
- Before authorised work, execute original board lint and original lane-status --preflight for exact assignment; require READY exit0 plus source/authority/session checks. Unity additionally needs its normal machine lock and verified process identity.
- On completion, worker explicitly declares HANDED-OFF in receipt, records source HEAD/status and retained changes/evidence. Verify no worker process remains capable of writing and Unity process termination via PID/start/boot and Editor scan. Release normal Unity lock only after that evidence using matching owner/task/lock ID.
- Custodian records safe RELEASED in receipt/ledger under board-directory lock; use supported DONE-PENDING-LEDGER with Lane/Branch None or remove closed row after ledger publication. Retained source custody, if still needed, remains a distinct named blocking reservation. Existence of a retained directory never grants a writer.
- Owner cancellation/reassignment requires verified handoff and fresh assignee registration. Unresponsive/long-running worker stays owned while investigated. Real safety/conflict stops do not themselves transfer custody. No clock-triggered deletion or reclaim.

Migration: no opportunistic rewrite of held lock JSON. Observe current owner/process, complete safe release or separately authorised reviewed handoff, then adopt new mode at next acquisition. Reconcile #690 and Multi free-text deadlines only through their authorised custodians; preserve source/evidence and histories. Back up exact deployed source/config/provenance before installation; never overwrite live board/lanes/history with source copies. Rollback restores reviewed source/provenance under exclusive install custody only after confirming no newer null record would become unreadable; otherwise keep ownership held and obtain a reviewed compatibility rollback. Never delete lock state to make rollback pass.

#652 controlled installation (or an explicitly approved successor) is necessary before the proposed Unity no-expiry mode is usable physically. Existing #652 source approval does not authorise this scope expansion or live install. Required: Manager-approved amended source packet; actual exclusive source worker registration; exact candidate tests/provenance; distinct nonauthor independent review APPROVED; separate owner/Manager physical deployment permit; physical installer ownership and board-directory lock/no conflicting Ops writer; safe machine-lock/process state; backup, bounded deployment, post-install original gates and readback. No Unity launch until Manager #731 accepts this audit, reviewed physical compatibility is demonstrated, Tier S W §12 instrument is owner-signed and separate source/Unity start gates pass.

## Verification and hashes

Executed against original installed scripts with GIT_OPTIONAL_LOCKS=0 and PYTHONDONTWRITEBYTECODE=1, after source inspection:
- `/media/jer/ZX20/CCF-Ops/scripts/packet-lint /media/jer/ZX20/CCF-Ops/board/active.md --board`: exit 0, “PASS: board schema and duplicate checks (42 live tasks)”.
- `/media/jer/ZX20/CCF-Ops/scripts/lane-status --preflight --lane Sen1SaveA0ZX20NonUnity707 --branch integration/sen-1-d073-shared-save-a0-20261117-v1 --json`: exit 0; target ready predicates satisfied, warnings [], dual_store_conflicts [], registered_worktree true, zx20 true, dirty false, exact HEAD 55863f1b7719011bba16b3f3ab377e379c0b988a. JSON mode emits no literal READY line; exit0 and these predicates are the original READY result. Existing #690 assignment only: this is NOT source-start approval and NOT validation of a modified/no-expiry lease.
- Storage: /dev/sdd2 matched configured persistent device/UUID; PASS, writable; 53,086,126,080 bytes free versus 16,106,127,360 threshold.
- Raw lock.json absent before/after; final /proc scan found no Unity executable PIDs. Native unity-lock status/acquire/release NOT RUN; no lock lifecycle mutation.
- Full self-test, null/indefinite acquire tests, installation, Unity, build, dotnet and process termination NOT RUN.

SHA-256 below was identical immediately before and after the original lint/preflight execution. Paths relative to /media/jer/ZX20/CCF-Ops:

| Path | SHA-256 |
|---|---|
| lanes.json | b16bfcad2fbb0be09c8d44fa0acdc87841d458e784c8befee2668a3650eacd61 |
| board/active.md | 8d3c17e173899562cf508a1114d2b0fa09b79022c93c95688f2d8529f098b6af |
| README.md | 249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517 |
| scripts/_ccf.py | ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001 |
| scripts/lane-status | e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f |
| scripts/unity-lock | 54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5 |
| scripts/packet-lint | a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43 |
| tests/self-test.py | 2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3 |
| state/unity/guard | e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 |
| state/unity/history.jsonl | dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa |

Additional observed deployment.json SHA-256: 8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18.
No physical edit/install/cache-producing test or worker-lane executable was invoked. GitHub branch/file/comment are the only intentional mutations. Hash evidence establishes content stability of the listed controls during verification, not a machine-wide forensic no-write proof; initial checkout inspection has no byte-level index baseline. External schedulers/services were not audited. Inspection was not a held-board-lock transaction; future operational actions require fresh locked readback.

## Handoff and remaining gates
Worker assessment: read-only audit ready for Manager #731 review; installed no-expiry compatibility NEEDS CHANGES. Nothing integrated, deployed, Unity-verified or accepted. #690 and Multi ownership preserved. No GitHub issue closure or physical custody release performed by #733.
DASHBOARD SYNC: NOT AVAILABLE. Published dashboard reflection: NOT VERIFIED. This strict read-only packet grants no physical board/publisher writes; Manager/custodian must ingest this GitHub result through the authorised path and verify dashboard readback.
Recommended next step: Manager #731 issues APPROVED / NEEDS CHANGES / REJECTED for this audit, then routes the separate bounded Ops patch/review/deployment packet above. #652 live install and #718 source/Unity start remain held pending their own gates.
