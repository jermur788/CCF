# AgentWorkflow proposed changes — G4 review required

Proposal only. `Docs/Project/AgentWorkflow.md`, all other canonical context sources and the gameplay Decision Log are unchanged. Review the exact foundation branch/HEAD and raw offline receipts before authorising a separate canonical patch.

- Record User → ChatGPT Overall Manager → mechanical execution checks → thin T3 Execution Manager → disposable task workers. T3 reports ready/not-ready and prepares packets; it does not autonomously orchestrate threads or choose gameplay.
- Replace assumptions of a standing reviewer with explicit independent review gates according to risk/acceptance criteria. G4 Integration / Canon owns this authority transition. Review cannot silently relax thresholds.
- Adopt persistent ZX20 lanes with distinct branches and disposable threads; remove per-task worktree churn. U1/U2 are dedicated Unity lanes; Integration single integrator; NonUnity offline. No shared/copied Libraries and no automatic cleanup/migration.
- Specify Claude A/B as an explicitly assigned pool, with availability/quota checked at dispatch, no autonomous account switching or credential handling. Examples are placeholders, not verified subscriptions/model availability.
- Require route identity: harness / model / quota source. Retain dynamic capability-based routing and no permanent model disciplinary ownership.
- Require serialized Unity queue and one machine-wide ownership lock across lanes/clones. Exact branch/HEAD, owner/task, ETA, authorised phases and Player/native input requirements belong in packets; stale/force release is audited. Cooperative adoption by existing launchers remains separate work.
- Approve a reviewed board/ledger bridge first; retain current active-tasks authority until an explicit later change. Board contains actionable state only, closed summaries go to ledger; no transcript/token-telemetry accumulation.
- Persist roles in CCF-Ops/roles. Implementation workers remain primarily packet-constrained. Role persistence confers no product/acceptance authority.
- Incorporate the hard ZX20 storage rule: verify UUID/mount, redirect temp/cache, keep Git metadata/Unity caches/builds/large receipts there. Inventory old SSD material; migration requires separate authorisation. Path text alone is not proof of storage placement.

Recommended G4 packet: independently review ops/workflow-foundation-v1 at the exact handoff HEAD against all foundation phases and storage constraints; inspect preflight/before-after inventory and self-test raw output; verify canonical/game paths unchanged; assess cooperative lock limits, deployment provenance and authority bridge. Verdict: APPROVED / NEEDS CHANGES / REJECTED per explicit criteria. Do not merge, change authority, warm Unity, move old data or launch workers during this review. Then issue a separately bounded canonical update packet and one real Tier-1 pilot after acceptance.
