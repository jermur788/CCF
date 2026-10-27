# U4 player guidance — Tier 1

Source: eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac. Branch: task/scenario-one-two-cycle-guidance-v1.

Three derived states direct the player to acknowledge Annual Review after resolved work, explain that all eight objectives alone are insufficient, or return to the forest for another non-prescriptive intervention in a different simulation year. Existing completion/stale-order hints keep priority. No persisted state or automatic acknowledgement.

Adapted the already published guidance 40b0a7654fe40c44254914b897c12e816e7bb6be, moving all presentation additions out of the protected ScenarioOneCompletionRules file. That file is byte-identical to the source (also asserted by the runner). The presentation helper mirrors its qualification filter only to choose wording; the authoritative completion predicate remains unchanged. Keep the helper and rules aligned when future contracts change.

Changed production files: ScenarioOneCompletionGuidance.cs + meta (new), ScenarioInterventionHistory.cs (optional hint), ScenarioOneManager.History.cs (derived state), AnnualReviewView.cs (footer wording). Changed tooling: N3OfflineCheck.cs, run_n3_offline.py; ScenarioHistoryOfflineCheck.cs (four hint checks inherited from the bounded existing work).

Verification: `python3 Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py --out <external-dir>` PASS 88 checks: original 67 plus 21 guidance state/text/non-prescription checks, actual acknowledgement handler transition, chronology negatives and round trips. The updated reviewed-one-cycle text assertion explicitly requires another cycle in a different year. Rules hash, protected manager boundaries and event numbering are asserted. `git diff --check` PASS. No Unity, rendering/input verification or broad regression. UI wrapping/real display needs bounded Unity smoke on the later candidate.
