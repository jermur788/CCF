# RACK-V2-05 — Overnight Coordination Amendment

**Manager assignment:** the mandatory DeepSeek watcher for RACK-V2-05 is also the **assigned live-coordination integrator for exactly two rows** while this overnight task starts and runs:

- `RACK-V2-05 — map rack editing`
- `MFM-03 — coverage calibration measurement`

It may make only the minimum `Docs/Project/coordination/active-tasks.md` edits needed to:

1. add/refresh the RACK-V2-05 row with claim #43, primary writer, branch/worktree, file ownership and status;
2. state that RACK-V2-05 has overnight priority for the machine-wide Unity lane;
3. change the MFM-03 row to WAITING for Unity behind #43 while preserving its scope/claim #39;
4. update those two rows during executor/model transitions, lock acquisition/release and handoff.

It must refresh the live file immediately before every such edit and preserve every unrelated row byte-for-byte. It may not use this assignment to alter other ownership, historical status, specs, decisions or task scopes.

The GitHub issue #39 has already been marked `[WAITING]` behind #43. This amendment exists because live `active-tasks.md` remains the authoritative ownership/lock record and must be reconciled at the start of the overnight run.

No other worker gains coordination-write authority from this amendment.
