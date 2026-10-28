# RACK-V2-06 — Bounded Coordination Amendment

The Overall Manager assigns the RACK-V2-06 primary worker as a **bounded coordination integrator** solely for the following live `Docs/Project/coordination/active-tasks.md` changes at task start:

1. Correct the completed `RACK-V2-04 — map planning mode` row so it no longer says RACK-V2-05 is blocked by OPEN-3. OPEN-3 was accepted by the user on 2026-10-28 and RACK-V2-05 is now integrated.
2. Change the existing `RACK-V2-05 — map rack editing` row from `HANDED-OFF / AWAITING MANAGER REVIEW` to `QUALIFIED / MANAGER APPROVED / INTEGRATED`, recording PR #44 / merge `0ec6bc4d473975e9938040026737e70a33432c48` and Manager review `Docs/Handoffs/RACK-V2-05/MANAGER-REVIEW.md`.
3. Add/update a `RACK-V2-06 — spacing teaching` row naming the actual executor/model/provider, branch, worktree, owned Stand Map/spacing files, exclusions, and claim #45 status.

All unrelated rows must remain byte-for-byte unchanged. This authority does not extend to any other coordination entry.

Refresh `main`, claim #45 and the live coordination file immediately before applying the edit. If any authorised target row has been changed incompatibly by the Overall Manager or another assigned integrator, do not overwrite it; reconcile only the three authorised rows or stop and report the conflict.

This amendment grants no gameplay scope beyond `TASK-PACKET.md` and no right to bypass the machine-wide Unity lock.
