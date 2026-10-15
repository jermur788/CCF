# Live operational board

Actionable state only. Authoritative active-tasks/packet locks must agree before dispatch. Single writer: assigned Execution Manager. Source snapshot; deployment is the live copy.

| Task ID | State | Owner | Harness | Model | Quota source | Lane | Branch | HEAD | Subsystem | Locks | Unity required? | Unity queue position | Verification tier | Open gate | Dependency | Next action |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CCF-G4-WORKFLOW-FOUNDATION | WAITING | Overall Manager (assignment pending) | Unassigned | Unassigned | Unassigned | None | ops/workflow-foundation-v1 | TBD | Operations / canonical review | Proposed workflow only; canonical writes blocked before G4 | No | None | Tier 1 | G4 Integration / Canon | Foundation exact-HEAD handoff | Assign independent bounded G4 review; reconcile authoritative task locks |
