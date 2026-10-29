# PLAYER-TEST-BROWSER-LANE-01 — S1-C T-05 v22 lineage review

## Identity

- Parent: #91 `PLAYER-TEST-OVERNIGHT-01`
- Browser lane: #105
- Feature claim: #104
- Browser inspection model/provider: OpenAI GPT-5.6 Sol, GitHub/browser-only manager lane.
- Required execution/review route remains the user-specified ladder; this browser inspection does **not** substitute for an OpenAI 6.1 Sol cross-provider save review.
- Main refreshed at `ad4c7027db0bb6d8b2aba61c6f1bdd768ba08f11`.
- Integration refreshed at `integration/scenario-one-player-test-ready-v1` = `aca74acacc565f72ab802177cf69ca6ebb54d637`.
- T-05 reviewed candidate: `7ae0bbcc5b70385b51a608f62037bf9782a34020` on `feature/s1-c-t05-objective-history-v1`.

## Sources inspected

- #104 body and both implementation comments.
- `Docs/Handoffs/S1-C-Implementation-05/HANDOFF.md` on the feature branch.
- `Docs/Handoffs/S1-C-Implementation-05/INDEPENDENT-REVIEW.md`, including the round-2 re-review.
- GitHub compare `aca74ac..7ae0bbc`: candidate is 5 commits ahead with the additive v22 schema/history/validation/hash/UI slice plus focused verification/docs; no unrelated merge is present in the compare.

## Findings

1. Round 1 (`cd7b518`) correctly received `NEEDS CHANGES` for the year-0 guard, missing R-1 derivation, and lost/re-gained first-achievement history.
2. Round 2 at `7ae0bbc` has an independent Claude Opus 5.5 re-review verdict `REVIEW PASS — APPROVED FOR V22 INTEGRATION LINEAGE`; the fix range is confined to objective-history replay/guard/UI labelling and the focused offline check. The already-reviewed v22 schema, validation order, hash-strip/relabel rule and frozen Reference Future v1 protection are unchanged.
3. The candidate remains **not Unity verified and not integrated**. Offline hash proof uses `System.Text.Json`, not Unity `JsonUtility`; genuine v20/v21 disk loads, the real evaluator/replay path, rendered Annual Review states and hash-bearing Unity gates remain mandatory.
4. The stale schema-pin inventory is now explicit. Pure label pins may move `21 -> 22`; `RackV2SaveVerification.cs:221` is a v20 fixture assertion and must remain 20. `ThreeSpecies/Qualification/DensityQualification.cs:952` requires runtime judgement: relabel the schema pin, then determine whether objective-history record keys alone can make model-1/model-2 key sets diverge; if so exclude only those additive history-record keys rather than weakening the comparison.
5. The existing independent review is Anthropic-on-Anthropic. The user requires OpenAI 6.1 Sol for suitable cross-provider independent review, so the high-risk save/reference/determinism review gate is **not considered administratively closed** by this browser lane.

## Browser verdict

**APPROVED FOR BOUNDED T3 EXECUTION, WITH MANDATORY OPENAI 6.1 SOL CROSS-PROVIDER SAVE REVIEW BEFORE INTEGRATION.**

This is not `INTEGRATED`, not `VERIFIED`, and not acceptance of the v22 slice.

## Exact bounded packet for T3 Code claim #106

Purpose: close S1-C T-05 v22 without broadening scope.

1. Refresh `integration/scenario-one-player-test-ready-v1`, #91/#104/#105/#106 and live `active-tasks.md`; preserve single-writer and Unity-lock rules.
2. Run an **independent OpenAI 6.1 Sol source/save review** of exact candidate `7ae0bbc` against the approved S1-C spec and the round-2 Opus review. Stop on any protected Reference/save/determinism disagreement; do not silently rebaseline.
3. If that review passes, apply one bounded verification-only **v22 pin relabel** using the complete inventory in the T-05 handoff/review. Do not edit `RackV2SaveVerification.cs:221`; do not weaken any assertion. Treat `DensityQualification.cs:952` as a measured runtime case, not a blind relabel.
4. Unity execution on the exact reviewed descendant: import/compile; real `ScenarioOneManager` capture/restore/`EvaluateProgress`; year-0 save/reload then year-1 recording; real `ForestSaveController` disk load/refusals; genuine v20 and v21 save loads; one exactly derivable and one `not recorded` old-save objective through the real evaluator; at least two objectives first reached in different years with save/load round trip.
5. Reproduce all protected v20/v21/legacy/world-hash anchors and frozen Reference Future v1 identity through Unity/`JsonUtility`. Explain any raw save-JSON movement caused solely by the new additive record; never relabel a protected world identity.
6. Render Annual Review states at the required UI resolutions for `(recorded)`, `(derived from saved records)` and `not recorded`.
7. Re-run all affected deterministic/lifecycle/hash-bearing suites. For `DensityQualification.cs:952`, if the only key-set difference is the additive objective-history record structure, exclude exactly those history-record keys and retain the original growth-model comparison strength.
8. Publish the reviewed exact SHA and executable evidence. Only after all required gates pass may #106 integrate T-05 into the integration branch. No Windows build.

## Next executable action

#106 consumes this packet: OpenAI 6.1 Sol independent save review first, then v22 pin relabel + Unity/save/hash/rendered closure, then integration review.