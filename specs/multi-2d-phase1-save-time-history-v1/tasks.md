# 2D Phase 1 — Proposed Task Breakdown

**Status: APPROVED TASK PLAN — owner 2026-11-24 13:53 UTC.** Do not claim or run source tasks until a single source lane is formally admitted, and do not overlap existing map ownership.

### P1-A0 — isolated 2D save contract (candidate parallel packet)
- [x] Owner approved six recommended A defaults in `spec.md` on 2026-11-24. [ ] Before source work, check current-milestone/decisions and latest active-tasks; inspect map writer grants and compile-linked scope.
- [ ] Allocate single exclusive new worktree/branch from approved SHA, claim in live coordination with physical Ops reservation.
- [ ] Add only new `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/*.cs` files for v1 typed complete state envelope, strict codec, validation, safe slot storage.
- [ ] Add only new, non-overlapping dedicated test/evidence files; do not modify existing harness registration if owned by map; report any unrun tests honestly.
- [ ] Test malformed schema, duplicate keys, model mismatch, precision, exact arrays/IDs, all Stage4/5/Phase2 state and long annual records; restore of model 0 should not be inadvertently upgraded.
- [ ] Independent review and permanent GitHub handoff; return `IMPLEMENTED` only for actually committed and checked components, NOT gameplay save/load.

### P1-A1 — deferred Core integration packet
- [ ] After map pass closure, choose reviewed map integration HEAD and verify no source collisions or dirty worktrees.
- [ ] Single-lock capture/restore on authoritative root; stale fence rejection and new session identity; rollback on error.
- [ ] Safe save/load commands through current server; keep paths constrained and failures recoverable.
- [ ] Genuine bounded annual loop using existing authoritative one-year step; explicit stop reasons and zero implicit cancellation/approval.
- [ ] Add truthful annual summary (or view derived from existing saved accounts) without inventing past events.
- [ ] Run sequential one-year parity and interrupted skip/load continuation tests; preserve all model anchors.
- [ ] Independent review, integration verdict, qualification handoff.

### P1-B — deferred presentation packet
- [ ] Add controls only after map file grants released: named saves, explicit load, skip preview/confirmation, stop reason, year list and selected-year history.
- [ ] Read-only lazy history endpoints: no full multi-decade append to `GET /multi/view`.
- [ ] Test 1280×720 and 1920×1080, fencing/race cases, body/output caps, no 3D changes.
- [ ] Independent UI review and owner test; request G4 integration separately.

### Excluded
No simple forest generator, five-rack 0.64 ha test stand creation, economy, contractor/owner-time implementation, 3D/Unity changes, art assets, production merges, or automatic new forest/save migration. They follow Phase 1.
