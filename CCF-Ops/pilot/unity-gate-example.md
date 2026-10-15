# Example only — post-integration Unity gate (not dispatch)

Routing: Codex / capable implementation model / Manager-assigned allowance
Role: Unity Operator
Role file: roles/unity-operator.md
Context: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Base: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Branch: HEAD
Scope: Example authorised gate at the exact candidate HEAD; replace example context/base/HEAD and gate details in a real packet.
Exclusions: No gameplay implementation, parameter search or integration branch writes.
Ownership: Example operator, U1 only after Manager reservation.
Verification tier: 1
Time budget: 5 minutes
Acceptance criterion: Receipts identify the exact candidate HEAD and only the authorised gate is executed.
Stop conditions: Dirty/occupied lane; low/read-only/wrong storage; branch conflict; candidate mismatch; held lock; missing gate authorisation. This example itself does not authorise execution.
Handoff format: Candidate HEAD, lane, lock ID, gate result and evidence path; pushed branch or recorded backup.
Unity required: Yes
Exact HEAD: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Unity ownership: Example operator
Unity lane: CCF-U1
Allowed phases: Example only; no Unity launch authorised by this document.
Player/native input required: No

## Placement sequence

Integration constructs the candidate; it does not run the Editor. After a real packet authorises placement, reserve a clean U1/U2 lane and use the exact integrated candidate (prefer detached HEAD, without duplicating the writable integration branch), then acquire the lock, run the specified gate and return exact-HEAD evidence. Do not change a dirty or owned lane.
