# Verification Harness Hygiene 01 — Local Dispatch

**Purpose:** run a bounded offline repair of two known verification-tool blind spots while MFM-08 #156 owns the Unity-critical path.

- **Difficulty:** 2/5 — tooling-only, but must prove real diagnostic sensitivity with negative controls.
- **Best suited:** OpenAI Codex / Sol-class local code-verification worker. Ultra Code is better reserved for MFM-08 #156.
- **Branch:** `verify/harness-hygiene-v1`
- **Worktree:** `/media/jer/Videos/CCF-HARNESS-HYGIENE`
- **Exact start:** `77fb9d7a8b5ff4652149b1d4335e948c7523c9ac`
- **Unity:** do not use or acquire the Unity lock.
- **Production:** no `Assets/ForestPrototype/` edits.
- **Packet:** `Docs/Handoffs/Verification-Harness-Hygiene-01/TASK-PACKET.md`
- **Expected handoff:** `Docs/Handoffs/Verification-Harness-Hygiene-01/HANDOFF.md`

## Copy/paste dispatch

`Run Verification Harness Hygiene 01 from Docs/Handoffs/Verification-Harness-Hygiene-01/TASK-PACKET.md starting at exact 77fb9d7a8b5ff4652149b1d4335e948c7523c9ac on branch verify/harness-hygiene-v1 in /media/jer/Videos/CCF-HARNESS-HYGIENE after refreshing active-tasks and the task claim. Offline only: repair CloudTypeCheck so known Painter2D compatibility gaps no longer prevent method-body binding, prove a deliberate body-level mutation is caught, and replace SiteInfrastructure's all-or-nothing verification-reference guard with an explicit narrow path allow-list plus positive/negative controls while keeping the production zero-reference guard strict. Do not touch Assets/ForestPrototype, gameplay values, accepted thresholds, Unity, or Windows. Publish Docs/Handoffs/Verification-Harness-Hygiene-01/HANDOFF.md and return VERIFICATION HARNESS HYGIENE QUALIFIED — READY FOR MANAGER REVIEW or the exact blocker.`
