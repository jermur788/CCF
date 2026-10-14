# N3 two-cycle completion: persistence boundary

Source b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a; canonical context 321791b8203bc709c57615fd4d6a19614d78db24. Non-Unity inspection only.

## Mandatory stop reached

`MarkAnnualReviewSeen` only sets `annualReviewSeen = true` once an annual report exists. It records no event/year/event-ID. Capture/Restore persist that boolean. Management event types include no review acknowledgement; report and order records have no review timestamp.

Consider identical successful interventions in years 10 and 25. History A acknowledges an idle annual review in year 1, before either intervention, and never again. History B acknowledges in year 10 after the first intervention and before the second. Both have the same annual reports, work orders, management events, ecological snapshots and true review boolean. Nothing in persisted data distinguishes A from B. The accepted rule must reject A and accept B. Therefore any implementation using only that existing persisted information necessarily misclassifies one.

The packet explicitly says: “If existing persisted data cannot prove review-between-cycles across save/load: STOP that part and report before schema change.” No gate, objective or save change has been made.

## Concrete minimal proposal, not implemented

Append an `AnnualReviewAcknowledged` value to `ScenarioManagementEventType` (preserve all existing enum numeric values), record it through the normal acknowledgement handler with current year and existing monotonically allocated event ID. Derive successful work-cycle groups and temporal acknowledgement ordering from existing events. Use event IDs to distinguish acknowledgement after work in the same annual year. Existing event-list storage needs no new field or version number, but this is a persistence-contract extension requiring explicit Manager authorization under the packet stop rule.

Old completed v20 saves remain Completed; do not retroactively revoke their recorded outcome. Older active saves with only `annualReviewSeen=true` cannot prove chronological review and should need a newly recorded acknowledgement after a successful cycle, never infer chronology from the boolean. Newer saves with the appended enum may be rejected by older validators: this compatibility boundary must be documented/tested if authorized.

Do not use work-order createdYear as a review timestamp, infer review from automatic screen opening, or reset/reinterpret the existing persisted boolean to fake chronology. No schema bump or canonical doc update is proposed here.

## Accepted decision for later canonical publication

“Scenario One completion requires the existing eight objectives plus two resolved Work Plans in different simulation years, with an acknowledged Annual Review after the first and before the second. Each counted Work Plan must contain resolved work. The second intervention is non-prescriptive.”

## Current status

N3 implementation BLOCKED at the explicit persistence boundary. Eight objective production bytes unchanged; save version 20 unchanged; no Unity run. N1/N2 can proceed independently.

## Executed offline checks

`Tools/Verification/ScenarioOneCompletion/review_persistence_boundary.py` PASS: six source/persistence checks and an abstract identical-records/different-required-verdict witness. This is a source proof, not a Unity/save-loader run. Existing source history logic suite: 70/70 PASS.

Old shallow path remains Completed in unchanged production; no new gate has been implemented, so no claim that the requested negative path now passes. Two-cycle valid-path/negative-path/old-completed-save gates are NOT RUN for an implementation. Eight objective bytes and save v20 unchanged. No U4 implementation HEAD exists yet. The report branch may be published for Manager inspection, but its documentation commit is not a U4 candidate.
