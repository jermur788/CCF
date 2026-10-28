# RACK-V2-08 — Bounded Coordination Amendment

The Overall Manager assigns the RACK-V2-08 primary worker as a **bounded coordination integrator** solely for these live `Docs/Project/coordination/active-tasks.md` changes at task start:

1. Change the existing `RACK-V2-07 — confirmation + minimap` row from `HANDED-OFF` / awaiting review to `QUALIFIED / MANAGER APPROVED / INTEGRATED`, recording PR #48 / merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9` and Manager review `Docs/Handoffs/RACK-V2-07/MANAGER-REVIEW.md`.
2. Add/update a `RACK-V2-08 — remove competing first-person rack authoring` row naming the actual executor/model/provider, branch, worktree, owned first-person rack-authoring/UI verification surface, exclusions and task-claim status.

All unrelated rows must remain byte-for-byte unchanged. This authority does not extend to MFM-03, other Rack tasks, Decision Log/spec authority, or any other coordination entry.

Refresh `main`, the task claim and the live coordination file immediately before applying the edit. If either target row has been changed incompatibly by the Overall Manager or another assigned integrator, do not overwrite it; reconcile only the two authorised rows or stop and report the conflict.

This amendment grants no gameplay scope beyond `TASK-PACKET.md` and no right to bypass the machine-wide Unity lock.
