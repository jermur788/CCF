TASK PACKET
PROJECT: CCF

MANAGER: ChatGPT Game Dev (Overall Manager)
ROUTING: SOL/CODEX for implementation; later REGULAR CLAUDE VALUE for the bounded G4 integration and canon review
ROLE: Primary
ROLE FILE: CCF-Ops/roles/execution-manager.md
UNITY REQUIRED: No
BASE: 37e33bf7fda828898f701752ca8450f51f6f92b4
CONTEXT COMMIT: cddad55213d3491b9f320a7a9a7a1bcd913b6ec8
LANE: NonUnity
BRANCH: ops/workflow-foundation-v1
GOAL: Prove the deferred T2 operational mechanics (append-only board-change history plus duplicate protection) and complete one real Tier-1 operational task under explicit T1-record (active-tasks) and board agreement, without promoting board authority.

AUTHORITATIVE SOURCES:
Docs/Project/AgentWorkflow.md; Docs/Project/coordination/active-tasks.md; CCF-Ops/README.md; CCF-Ops/ActiveTasksTransition.md; CCF-Ops/G4BCorrections.md; CCF-Ops/ledger/README.md.

IN SCOPE:
Add one append-only board-change history writer for CCF-Ops using the existing guarded flock + fsync-append pattern (JSONL: timestamp, task_id, from_state, to_state, actor, reason, prior board SHA-256), strictly append-only and never rewriting prior lines; confirm the existing mechanical duplicate protection (packet-lint --board and lane-status/preflight duplicate Lane/Branch checks) with a negative case; execute exactly one real, reversible Tier-1 operational task only after the board row and the authoritative active-tasks row agree on ID/state/owner/lane/branch/HEAD; record the closed result in the ledger before removing any live row.

OUT OF SCOPE:
Gameplay, ecology, assets, scenes, packages, ProjectSettings, save/determinism, canonical context (AgentWorkflow, brief, decision log, milestone, overview, entry files), any board-authority promotion, any change to active-tasks authority, arbitrary worker-thread dispatch, Unity execution, credentials, storage-policy or lane-migration changes.

OWNERSHIP: CCF-Ops only — board/active.md, ledger/, scripts/ (new history writer plus its tests), tests/fixtures/, pilot/. No other subsystem.

SHARED / LOCKED FILES:
Docs/Project/coordination/active-tasks.md remains single-writer Manager/authorised-integrator; read-only here. Unity lock state read-only.

COMPATIBILITY REQUIREMENTS:
Append-only history must never rewrite prior entries; existing board/ledger schema and Unity-lock history unchanged; both Git stores checked before any branch/lane action; saves, determinism, frozen reference, Unity pipeline and installed packages untouched.

MUST-NOT-CHANGE HASHES / ANCHORS:
Unity lock state, active-tasks bytes, canonical context commit cddad55213d3491b9f320a7a9a7a1bcd913b6ec8, all Assets/Packages/ProjectSettings.

INTENTIONALLY CHANGED HASHES / ANCHORS:
None unless explicitly authorised.

VERIFICATION TIER:
Tier 1

TIME BUDGET:
20 minutes.

QUESTION THIS VERIFICATION MUST ANSWER:
Does the append-only board-change history record every state transition without rewriting prior entries, and does duplicate protection reject a duplicate live Lane/Branch, while one real Tier-1 task runs end to end with board and T1 record in agreement?

VERIFICATION:
Offline only, on ZX20/CCF-Temp scratch fixtures plus the isolated self-test harness; packet-lint PASS/FAIL/WARN cases for packets, board and duplicates; append-only history assertions (line count monotonic, prior bytes unchanged, fsync-append); the single real task recorded board -> ledger -> row removal; no Unity.

DO NOT ESCALATE TO A HIGHER TIER WITHOUT:
a failed gate; a concrete newly discovered risk; a direct protected-reference/save/determinism risk; or Manager approval.

STOP CONDITIONS:
Storage/device mismatch; unknown or dirty lane ownership; unresolved board/active-tasks disagreement; any duplicate live Lane/Branch; any attempt to promote board authority, edit active-tasks, edit canonical context or touch Unity; missing field; any need to relax an existing gate.

HANDOFF FORMAT:
Standard worker handoff with exact base/branch/HEAD, evidence paths on ZX20, append-only-history assertions, duplicate-protection negative-case result, the single real Tier-1 task result, remaining gates and preserved unrelated work. Explicitly state proposal versus accepted canonical change.
