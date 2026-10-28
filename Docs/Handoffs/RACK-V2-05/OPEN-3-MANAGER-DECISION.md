# RACK-V2 OPEN-3 — MANAGER DECISION

**Date:** 2026-10-28  
**Authority:** direct user acceptance in the Game Dev Manager chat, recorded by the Overall Manager.  
**Status:** **ACCEPTED / IMPLEMENTATION-AUTHORISING FOR RACK-V2-05+**

## Decision

The existing OPEN-3 recommendation in the approved Rack V2 specification is accepted.

A confirmed rack plan may be reopened and edited **only while the affected rack has no executed removals**.

- Pending or approved-but-unexecuted rack work may be cancelled/reconciled through the existing authoritative work-order paths, with a clear player warning where appropriate.
- Reopening/editing must not duplicate forestry outcomes or create a second rack-removal system.
- Once a rack has **any executed removal**, that rack's geometry is locked from further editing and it is presented as harvested/executed rather than silently rewritable.
- Executed removals are never undone by rack-plan editing.
- Geometry changes to an otherwise editable confirmed rack invalidate only that rack's entrance-mark state as already specified; obsolete game-managed mark visuals/state are reset through the existing Rack V2 contract.
- Legacy40 and Reference Future v1 remain outside the V2 curriculum and unchanged.

## Effect

This resolves the approved spec's OPEN-3 blocker and **unblocks RACK-V2-05 — Editing**. It does not authorise RACK-V2-06+ scope inside RACK-V2-05, does not change save/schema by itself, and does not waive later persistence/functional/user-test gates.

Where the current `spec.md` / `tasks.md` still says OPEN-3 is open, this dated Manager decision is the later authority for execution. The Overall Manager will roll the wording into the canonical decision/spec records; workers must not reinterpret or broaden it.

No Windows build is authorised.
