# Overall Dev cross-project coordination

**Status:** active from 2026-10-31 by direct user instruction.

CCF participates in the private coordination repository `jermur788/Overall-Dev` alongside T3 Fork and Dashboard.

## CCF responsibility

Maintain `updates/ccf/latest.md` in `jermur788/Overall-Dev` using that repository's `UPDATE_TEMPLATE.md`.

Refresh the file when a meaningful CCF state change occurs, especially when:

- a milestone completes;
- current priorities change;
- a blocker appears;
- CCF needs something from T3 Fork;
- a CCF change affects Dashboard coordination;
- an important implementation or architecture decision changes the cross-project picture.

Keep the update concise. Reference the relevant CCF branch, commit, issue, handoff or canonical document instead of copying large implementation reports.

## Authority boundary

CCF remains authoritative for its own codebase, plans, task ownership, verification and implementation detail. Overall Dev is coordination-only and does not replace CCF canonical Git state, `Docs/Project/coordination/active-tasks.md`, approved specs, Decision Log, handoffs or verification evidence.

Overall Dev may return cross-project decisions or instructions to CCF. Treat those as external coordination inputs and reconcile them against the normal CCF authority order before implementation.

## Flow

`CCF → Overall Dev → cross-project decision → affected project(s)`

Dashboard may consume Overall Dev as a high-level coordination source; it must not infer implementation or qualification state beyond what the CCF update explicitly reports.
