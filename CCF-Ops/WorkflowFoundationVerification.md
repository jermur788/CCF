# ZX20 workflow foundation — current packet verification

Historical receipt for the prior foundation refresh (49 checks). The G4 B-corrections receipt and current 70-check verification are in G4BCorrections.md; the rules there supersede the old operational descriptions below.

This is an update/reuse pass of the existing foundation at `b09031c5cf3882915011638b3da5dee5d760abb2`, on `ops/workflow-foundation-v1`. Context remains `c6dc136b7f1f8668d9f67f95f48968d290ad5067`. No canonical authority change, new persistent lane, repository clone, cleanup or migration.

## Storage and lane preflight

Verified before new task output: `/media/jer/ZX20`, device `/dev/sda2`, exFAT, UUID `18D6-896E`. Available before this pass: 49,434,591,232 bytes (46.04 GiB).

All eight requested paths exist, resolve beneath the verified mount and share its device. All four Git lanes are registered to `/media/jer/ZX20/CCF-Repository.git`, clean at preflight, distinct branches, no observed process cwd ownership. CCF-Ops immutable hashes match the prior deployment. Production Unity lock is FREE; no Editor PIDs. Reuse is authorised by this packet. No unknown dirty work found. CCF-Ops/Builds/Verification/Temp are existing operational directories, not additional Git worktrees.

| Lane | Path | Branch | Preflight HEAD | Allocation | Library | Active / reusable |
|---|---|---|---|---|---|---|
| CCF-U1 | /media/jer/ZX20/CCF-U1 | lane/u1 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.63 GiB | False | no observed owner / yes |
| CCF-U2 | /media/jer/ZX20/CCF-U2 | lane/u2 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.63 GiB | False | no observed owner / yes |
| CCF-Integration | /media/jer/ZX20/CCF-Integration | lane/integration | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.63 GiB | False | no observed owner / yes |
| CCF-NonUnity | /media/jer/ZX20/CCF-NonUnity | ops/workflow-foundation-v1 | b09031c5cf3882915011638b3da5dee5d760abb2 | 0.20 GiB | False | no observed owner / yes |

U1 and U2 remain cold: this packet explicitly prohibits Unity launch, so warming/import remains deferred. No Library is copied/shared/recreated. Integration remains clean at the supplied context; NonUnity is the sole implementation lane. No accepted gameplay/feature branch is modified.

## Changes for this packet

- Board now uses exactly: Task ID, State, Owner, Harness, Model, Quota source, Lane, Branch, HEAD, Subsystem, Locks, Unity required?, Unity queue position, Verification tier, Open gate, Dependency, Next action. Existing task IDs/states/owners/locks are preserved during live schema migration. The G4 row remains WAITING/assignment pending; its review target is pinned after commit.
- Ledger uses Verification rather than Verification performed. Existing example purpose/base/final HEAD/verdict/integration/canonical-impact/evidence/date values are preserved. Still one explicitly simulated example, no backfill.
- lane-status reports backing_device, resolved_path and zx20 boolean in JSON; human output explicitly states ZX20: YES/NO. Off-storage configurations produce NO + STOP warning without inspecting that path as a writable lane. Only read-only inventory can admit such a path; mutation storage helpers remain strict.
- packet-lint requires Unity lane as well as owner, exact HEAD, phases, input/native requirement and stops. Handoff format is an explicit supported field. Fixtures updated.
- Execution Manager reports READY/BLOCKED and cannot waive gates. Reviewer reports A/B/C against the Manager-provided rubric; no A/B/C meanings were supplied here, so the future review packet must define them. If absent, reviewer reports classification BLOCKED rather than inventing criteria. Unity Operator remains serialized and cannot kill another owner's process automatically.
- ActiveTasksTransition.md is now directly in the versioned/deployed CCF-Ops root. Existing Docs/Operations proposal remains historical supporting material; active-tasks and AgentWorkflow authority are unchanged.

## Offline evidence

49/49 checks PASS, including valid ZX20/missing/dirty lanes, exact device reporting, off-storage ZX20: NO, unchanged Git metadata bytes/mtimes, malformed configs, packet PASS/FAIL/draft WARN, missing Unity lane, board/ledger schema, atomic concurrent Unity-lock acquisition, second-owner rejection, stale metadata, normal/force release and preserved previous owner/state. No Unity/process kill. Production lock never acquired by tests; test state is isolated under ZX20/CCF-Temp.

The negative off-storage fixture only observes existing `/`; it creates no path/worktree/data there. Mock Tier-1 sequence executes in isolated scratch: packet → lint → lane-status → ACTIVE → mock handoff → VERIFY → mock verification → ledger → close. The live board is not used as test scratch. This is not production acceptance or independent review.

Raw preflight, output, test result, mock stage snapshots and final handoff: `/media/jer/ZX20/CCF-Verification/WorkflowFoundationRefresh-20261015T212616Z/`. Test code executes while repository HEAD is the prior commit; tested file hashes pin the working tool bytes, which are committed/deployed unchanged. Final exact HEAD, changed-file list, deployment hashes, disk state and git diff --check results are recorded after commit to avoid a self-referential SHA.

All new scratch/cache/output uses ZX20; TMPDIR/TMP/TEMP are redirected. Global caches are not moved. Live board, ledger and lock state are preserved; deployment updates individual reviewed files, not a blanket copy.

Next: G4 review of the operational foundation and proposed AgentWorkflow update. Manager supplies the review's A/B/C classification rubric. No canonical edits in this pass.
