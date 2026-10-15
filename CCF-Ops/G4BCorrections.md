# G4 foundation B-corrections — Tier 1

Base: ops/workflow-foundation-v1 @ 340cce5b36c964ca5d4873e6ea6f4e1c272aea19.
Context: c6dc136b7f1f8668d9f67f95f48968d290ad5067.

This bounded correction changes CCF-Ops only. AgentWorkflow, project root AGENTS/CLAUDE, active-tasks, gameplay and production files remain unchanged. No lane creation/migration, Unity launch or process killing. Final commit and remote SHA are recorded in the external handoff after commit.

## Corrections

- F1: explicit Unity declaration required; generic Lane/HEAD/Worktree never infer Unity. Canonical packet aliases accepted. Substantive placeholder values rejected, including stops/acceptance/handoff. ROLE FILE required (NONE permitted). Unity lane must be configured and Unity-capable.
- F2: U1/U2 unity=true; Integration/NonUnity=false. Lock admission checks configured lane, real store, cleanliness, branch/HEAD and dual-store conflicts, failing closed without checkout changes. Force-release requires explicit Overall Manager approval reference and retains previous metadata/inspection in history. Cooperative advisory flock remains the serialization mechanism.
- F3: Integration constructs candidate HEAD; only a clean available Unity lane runs the post-integration gate at that exact HEAD. Prefer detached candidate validation to avoid duplicate writable integration branch. Operator role and example spell out the sequence.
- F4: new ZX20 common store local core.fsync changed from unset to all. environment.sh checks device/UUID/filesystem, rw and minimum 15 GiB. lane-status reports real free bytes/GiB, threshold, PASS/LOW SPACE. Below threshold no substantial lane/cache/build creation is allowed. Worktree presence is not backup: handoff needs pushed branch or explicit recorded bundle/backup. No old-store config change.
- F5: reviewer A/B/C per-packet grades are separate from canonical APPROVED/NEEDS CHANGES/REJECTED; B requires corrections unless the packet explicitly permits a documented caveat. Task packet remains scope/acceptance carrier.
- F6: two configured stores are inspected before branch assignment; conflicting writable checkout blocks READY. Origin remains the exchange boundary; old store is preserved.
- Board preparation: duplicate live Lane/Branch checks added. Append-only board-change history deferred to bounded T2 work with a reviewed writer; no authority promotion.
- Role loading: ROLE FILE plus explicit packet must be supplied until later canonical entry-file changes wire the complete chain. First-message reminder is reinforcement only.

## Offline verification

70/70 targeted checks PASS. Raw output includes NonUnity Lane/HEAD PASS without Unity inference; missing declaration, non-Unity-capable lane, placeholder stops/handoff, missing role file FAIL; canonical-style PASS; partial draft WARN. Isolated lock tests cover exact U1 branch/HEAD, false capability/wrong branch/wrong HEAD/second owner rejection, normal release, stale/malformed inspection, Manager reference requirement, complete prior-state audit and six concurrent acquisitions with one winner. Storage tests cover actual rw/free, simulated read-only/low space and low-space CLI hard stop. Dual-store conflict blocks preflight; duplicate board Lane/Branch fails. Git metadata remains unchanged by read-only inventory; mock pilot closes only after simulated evidence/ledger exists.

All fixtures/scratch/evidence stay on ZX20. Production Unity lock was not acquired. The test ran against working tool bytes while HEAD was the base; tested-tool-hashes.json pins those bytes. No repeated Unity qualification is claimed.

Evidence root: /media/jer/ZX20/CCF-Verification/G4BCorrections-20261015T215821Z/
- preflight.json, core-fsync-change.json
- self-test.txt, self-test-result.json, tested-tool-hashes.json, pilot/
- T1ActiveTasksDiscovery.json and T1ActiveTasksDiscovery.md (full per-copy details)
- final verification/deployment/remote receipts written after commit

## T1 physical active-tasks discovery — proposal only

Read-only inventory found 70 physical copies with 13 distinct contents. Each candidate report records path, Git store, branch/HEAD, last path-changing commit/date, file cleanliness, live-accuracy limits and local/cloud availability. No copy describes the current workflow-foundation/G4 correction frontier, so none is certified accurate for immediate dispatch. Other live project work cannot be declared closed from stale tables. Local/cloud reachability is assessed against current local access and published origin branch; cloud workers do not see local uncommitted files.

Recommend ONE temporary physical record:
`ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md`

Physical path: /media/jer/ZX20/CCF-NonUnity/Docs/Project/coordination/active-tasks.md.
Git store: /media/jer/ZX20/CCF-Repository.git.

Reason: persistent tools/control lane on designated storage, already published origin branch, no conflicting writable checkout. This recommendation is NOT designation: current canonical guidance expects designated integration/main coordination maintenance. Overall Manager must explicitly name the physical T1 record and authorise its bounded accuracy refresh before the canonical patch/dispatch. Do not create a second writer or promote board authority. All active-tasks bytes remain untouched.

NEXT: Manager names T1 record, then separately authorises bounded AgentWorkflow E1–E10 canonical patch.
