# Workflow foundation validation

Implementation is on `ops/workflow-foundation-v1`, base/context `c6dc136b7f1f8668d9f67f95f48968d290ad5067`. The final exact commit and deployment manifest are recorded after commit in `/media/jer/ZX20/CCF-Ops/WorkflowFoundationHandoff.md` and `deployment.json`, avoiding a self-referential committed SHA. No integration/acceptance is asserted.

## Offline results

**42/42 checks PASS**. Raw output: `/media/jer/ZX20/CCF-Verification/WorkflowFoundation/self-test.txt`; structured result: `self-test-result.json`; tested script/config SHA-256 values: `tested-tool-hashes.json`. Tests execute the working implementation while its repository HEAD still equals the supplied base; the hash manifest identifies the exact tested tool bytes, subsequently committed unchanged. Test Git refs/state and all generated fixtures stay on ZX20/CCF-Temp.

- lane-status: missing lane, clean lane, dirty lane (exit zero), local ahead/behind, Git metadata byte/mtime immutability, duplicate config, malformed JSON/config shape.
- packet-lint: valid packet PASS, valid Unity packet PASS, missing base/tier/stop FAIL, conflicting Unity owner FAIL, draft WARN, malformed SHA FAIL, Markdown headings and malformed board schema.
- unity-lock: acquire/status/rejected second acquire, wrong-owner release refused, matching owner release, naturally exited fixture PID reported stale, stale acquisition refused, reason required for force, complete previous metadata and inspection preserved, malformed raw metadata preserved, six concurrent acquisitions yield exactly one winner, isolated state free afterward, test override cannot leave ZX20 scratch. No process is killed.
- Board/ledger: required headers/fields checked; one actual compact proof entry is labelled SIMULATED PASS and not integrated. No historical task migration.
- Pilot: packet → lint → read-only lane status → isolated board ACTIVE → simulated worker handoff → VERIFY → mock criterion check → ledger → close. Stage snapshots and handoff: `/media/jer/ZX20/CCF-Verification/WorkflowFoundation/pilot/`. This proves operational mechanics, not a real worker review or Unity verification.

## Before / after lanes

All requested targets were absent before setup; exact parent filesystem checks are in the preflight. New independent Git store: `/media/jer/ZX20/CCF-Repository.git`, ~4.2 GiB allocation, no alternates, origin CCF remote. Existing common Git store/worktrees remain untouched. Its 83 original registration/HEAD records compare byte-identically before/after; raw comparison receipts remain under WorkflowFoundation. No stale worktree pruning.

| Lane | New path | Branch | HEAD before implementation commit | Allocated approx | Library | Status |
|---|---|---|---|---|---|---|
| U1 | /media/jer/ZX20/CCF-U1 | lane/u1 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.7 GiB | absent | clean |
| U2 | /media/jer/ZX20/CCF-U2 | lane/u2 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.7 GiB | absent | clean |
| Integration | /media/jer/ZX20/CCF-Integration | lane/integration | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 1.7 GiB | absent | clean |
| NonUnity | /media/jer/ZX20/CCF-NonUnity | ops/workflow-foundation-v1 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | ~209 MiB | absent | operations-only changes before commit |

Live deployment `/media/jer/ZX20/CCF-Ops` contains the versioned operational tree plus provenance/handoff; board/ledger/state are maintained there. No symlinks are used on exFAT. Scripts/config/roles deployment hashes identify their source; do not overwrite live state in future updates. The first G4 row is WAITING/assignment pending, not permission to execute canonical changes. U1/U2 warm-up is deferred; existing warm worktrees cannot be moved by this foundation task.

Disk: initial available 60,003,057,664 bytes (~55.88 GiB); about 47 GiB remains after lane creation, comfortably above the 15 GiB setup reserve. Final exact available bytes and per-target usage are in the final handoff. Old SSD pressure and candidate cleanup are separately inventoried; no deletion/migration is performed.

## Boundaries and remaining gates

No Unity Editor launch, game/simulation change, scene/prefab/meta edit, Packages/ProjectSettings change, production Assets change, canonical context/decision-log change, active-tasks authority change, accepted feature-branch change, subscription or credential setup. Only `CCF-Ops/` and `Docs/Operations/` are staged/committed. Diff and final clean-status receipts are recorded at handoff. Lane refs are idle local branches, not new accepted features. Only the implementation branch is pushed.

The Unity lock is cooperative and Linux-specific; existing launchers are not silently modified. Operator packets/roles must enforce it until separately adopted. Board synchronization remains a manual authorised-maintainer bridge; no writer overwrites active-tasks. Routes are placeholders, not installed provider/subscription claims. Integration/review authority is unchanged.

Next: independent G4 review of exact handoff HEAD using G4WorkflowFoundationReviewPacket.md → separately authorised canonical AgentWorkflow update/context refresh → one real Tier-1 pilot. No mandatory G4 gate is bypassed.

## Exact created source files

- `CCF-Ops/AGENTS.md`
- `CCF-Ops/CLAUDE.md`
- `CCF-Ops/README.md`
- `CCF-Ops/board/active.md`
- `CCF-Ops/environment.sh`
- `CCF-Ops/lanes.json`
- `CCF-Ops/ledger/2026-10-15-CCF-DUMMY-T1.md`
- `CCF-Ops/ledger/README.md`
- `CCF-Ops/pilot/README.md`
- `CCF-Ops/pilot/manager-packet.md`
- `CCF-Ops/roles/execution-manager.md`
- `CCF-Ops/roles/reviewer.md`
- `CCF-Ops/roles/unity-operator.md`
- `CCF-Ops/scripts/_ccf.py`
- `CCF-Ops/scripts/lane-status`
- `CCF-Ops/scripts/packet-lint`
- `CCF-Ops/scripts/unity-lock`
- `CCF-Ops/tests/fixtures/conflicting-unity-ownership.md`
- `CCF-Ops/tests/fixtures/missing-base.md`
- `CCF-Ops/tests/fixtures/missing-stop-condition.md`
- `CCF-Ops/tests/fixtures/missing-verification-tier.md`
- `CCF-Ops/tests/fixtures/partial.md`
- `CCF-Ops/tests/fixtures/valid-unity.md`
- `CCF-Ops/tests/fixtures/valid.md`
- `CCF-Ops/tests/self-test.py`
- `Docs/Operations/ActiveTasksTransition.md`
- `Docs/Operations/AgentWorkflowProposedChanges.md`
- `Docs/Operations/G4WorkflowFoundationReviewPacket.md`
- `Docs/Operations/WorkflowFoundationPreflight.md`
- `Docs/Operations/WorkflowFoundationValidation.md`
