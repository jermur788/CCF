# Claim #408 — fixed-candidate Tier1/Tier2 Unity qualification: ALL PASS

OpenAI GPT-6.1 Sol, medium effort, Codex harness in T3 Code. Sole Unity test worker, child assignment of Fable #402; no subagents, production edits, merge, Tier3 suite or Player/Windows build. Resumed from `4d847bf380ad02415a2726e7bb060c94310df23d` at 10:47 UTC after the prior provider interruption. This report supersedes the provisional checkpoint, not the preserved failed evidence.

Candidate production HEAD **e759193354b4613f88e3370a62ccaccf02c556cf**, tree **00c736b933c09e3c25ac347d13657e5372821007**, origin source branch confirmed unchanged. Production fix a8471a74. Qualification branch `qualify/scenario-one-cadence-v1-after-harness-repair-20261112`; lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` (ResearchCopyQual391). Final tested runner HEAD **b8e90fbe4e11806e98e23725fc952c11592e5b86**. Final report commit is the branch tip and is recorded with GitHub byte readback in `resume/publication.json` (self-containing its own commit SHA is not possible).

Evidence root **/media/jer/Files/CCF/Verification/CadenceV1-FixedQual-408-20261112T095220Z/**. Every evidence pointer below is relative to this root unless absolute. Core feature run `resume/runtime18`; final corrected native-key run `resume/runtime21`; rendered UI `resume/runtime22`; frozen-reference contract `resume/runtime19`. All final cases ran on the fixed production tree. The final native-key and rendered UI runs use the final runner. Bulk case/dependency equivalence from core run18 is recorded in `resume/case-equivalence.json`; the only later executable changes are the native-key positive controls/focus helper and a diagnostic dispatcher branch. All remaining core case bodies/helpers and the authentic initial-fixture construction are unchanged. Reference19 uses its own byte-unchanged harness. Initial UI16 also passed, but the fresh final UI run is used below. No before-interruption PASS is needed to qualify a matrix row: previous runs are retained as provenance/diagnosis only. The unedited boundary24/99 inputs used by the final run were authentically exported by runtime11 after real annual progression; paths and SHA256s are below.

## Counts and full sub-case matrix

Counts are the explicit lines below, not the runner's internal assertion count. Row 4 includes nine control-type lines plus separate entry/stop/fault sub-cases; row 7 has eleven rendered states per resolution plus two timings. Repeated evidence for parallel assertions is intentional.

| Matrix row | PASS | FAIL | NOT RUN |
|---|---:|---:|---:|
| 1 | 2 | 0 | 0 |
| 2 | 10 | 0 | 0 |
| 3 | 10 | 0 | 0 |
| 4 | 71 | 0 | 0 |
| 5 | 6 | 0 | 0 |
| 6 | 14 | 0 | 0 |
| 7 | 24 | 0 | 0 |
| 8 | 6 | 0 | 0 |

| Row | Sub-case | Result | Observable assertion / limit | Evidence |
|---|---|---|---|---|
| 1 | Unity compile/import exact candidate | PASS | Unity 6000.6.0f1; production Assets unchanged; disposable runner imports in same Editor. | resume/runtime18/cadence.log |
| 1 | Editor error classification | PASS | Known Search indexing stack retained (same SearchDatabase.cs:351/325 and SearchInit.cs:109 stack as /media/jer/Files/CCF/Verification/RackV2-09-20261029/field-04/field.log:1600); deliberately injected TickSkip exception classified separately; see console audit. | resume/runtime18/cadence.log; resume/console-audit.json |
| 2 | A strict: quiet, N=1 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 2 | A strict: quiet, N=2 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 2 | A strict: quiet, N=3 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 2 | A strict: quiet, N=4 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 2 | A strict: quiet, N=5 (stress override) | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 2 | A strict: post-felling, N=1 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 2 | A strict: post-felling, N=2 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 2 | A strict: post-felling, N=3 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 2 | A strict: post-felling, N=4 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 2 | A strict: post-felling, N=5 (stress override) | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 3 | B semantic: quiet, N=1 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 3 | B semantic: quiet, N=2 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 3 | B semantic: quiet, N=3 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 3 | B semantic: quiet, N=4 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 3 | B semantic: quiet, N=5 (stress override) | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 3 | B semantic: post-felling, N=1 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 3 | B semantic: post-felling, N=2 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 3 | B semantic: post-felling, N=3 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 3 | B semantic: post-felling, N=4 | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 3 | B semantic: post-felling, N=5 (stress override) | PASS | Exact N required. A: identical asserted pre-tick JSON/hash and each tick complete canonical JSON/hash. B: genuine UI one-year/ack route; only real acknowledgment events and event-ID shifts normalized. Full records include ecology, juveniles, deadwood, cash, objectives and outcome. | resume/runtime18/cadence.log — CADENCE408_CASE parity/post-felling |
| 4 | 4a input: ForestSaveController | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: StandaloneSessionMenu | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: ForestPlayer | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: ForestTreeMarkingManager | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: RackTool | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: ForestBuildable | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: ForestSawPit | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: ForestWoodStorage | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a input: RecentAssetReviewControls | PASS | Disabled during skip; prior enabled/disabled states restored through normal, stop, cancellation and fault exits; nine-type dictionaries and per-type restore logs. | resume/runtime18/cadence.log — CADENCE408_LOCK / CADENCE408_RESTORE |
| 4 | 4a deliberately pre-disabled controls | PASS | Player and gallery fixture already disabled; restored as disabled. | resume/runtime18/cadence.log — CADENCE408_CASE locks/pre-disabled |
| 4 | 4a native F5 Save | PASS | F5/F9/F10 each demonstrably reaches the actual game handler in an unlocked positive control first. OS events on private display, all nine owners disabled; actual isolated save bytes and mtime, loaded-world version and complete saved state unchanged; session menu closed. Test holds between ticks during OS delivery. | resume/runtime21/cadence.log — CADENCE408_CASE locks/native-F5-F9-F10 |
| 4 | 4a native F9 Load | PASS | F5/F9/F10 each demonstrably reaches the actual game handler in an unlocked positive control first. OS events on private display, all nine owners disabled; actual isolated save bytes and mtime, loaded-world version and complete saved state unchanged; session menu closed. Test holds between ticks during OS delivery. | resume/runtime21/cadence.log — CADENCE408_CASE locks/native-F5-F9-F10 |
| 4 | 4a native F10 session menu | PASS | F5/F9/F10 each demonstrably reaches the actual game handler in an unlocked positive control first. OS events on private display, all nine owners disabled; actual isolated save bytes and mtime, loaded-world version and complete saved state unchanged; session menu closed. Test holds between ticks during OS delivery. | resume/runtime21/cadence.log — CADENCE408_CASE locks/native-F5-F9-F10 |
| 4 | 4b lesson stage 0 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-0 |
| 4 | 4b lesson stage 1 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-1 |
| 4 | 4b lesson stage 2 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-2 |
| 4 | 4b lesson stage 3 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-3 |
| 4 | 4b lesson stage 4 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-4 |
| 4 | 4b lesson stage 5 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-5 |
| 4 | 4b lesson stage 6 | PASS | Authentic teaching snapshot from real handlers; exact lesson refusal and no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/stage-6 |
| 4 | 4b order/Pending/invalid=False/prescription=0 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Pending/invalid=False/prescription=0 |
| 4 | 4b order/Pending/invalid=False/prescription=1 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Pending/invalid=False/prescription=1 |
| 4 | 4b order/Pending/invalid=True/prescription=0 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Pending/invalid=True/prescription=0 |
| 4 | 4b order/Pending/invalid=True/prescription=1 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Pending/invalid=True/prescription=1 |
| 4 | 4b order/Approved/invalid=False/prescription=0 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Approved/invalid=False/prescription=0 |
| 4 | 4b order/Approved/invalid=False/prescription=1 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Approved/invalid=False/prescription=1 |
| 4 | 4b order/Approved/invalid=True/prescription=0 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Approved/invalid=True/prescription=0 |
| 4 | 4b order/Approved/invalid=True/prescription=1 | PASS | Actual manual/MFM creation and real Work Plan approval. Invalid targets die through ApplyMortality then RevalidateOpenOrders; no synthetic orders or events. Exact Pending/Approved refusal; full saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/order/Approved/invalid=True/prescription=1 |
| 4 | 4b unread latest review | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/unread |
| 4 | 4b reference preview (flag injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/preview-injected |
| 4 | 4b reference authoring | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/authoring |
| 4 | 4b game modal | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/modal |
| 4 | 4b real session menu | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/session |
| 4 | 4b real help | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/help |
| 4 | 4b advance warning | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/advance-warning |
| 4 | 4b rack tool active (flag injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/rack-active |
| 4 | 4b rack introduction (flag injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/rack-intro |
| 4 | 4b consent pending (UI condition injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/consent |
| 4 | 4b reset pending (UI condition injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/reset-pending |
| 4 | 4b reopen pending (UI condition injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/reopen-pending |
| 4 | 4b drag (UI condition injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/drag |
| 4 | 4b ghost (UI condition injection) | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/ghost |
| 4 | 4b controller unavailable | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/controller |
| 4 | 4b ecology unavailable | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/ecology-unavailable |
| 4 | 4b rack-plan problem | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. Genuine underlying problem produced by real rack actions; earlier lesson guard wins. See invariant/coverage limit below; no forced stage7 facts. | resume/runtime18/cadence.log — CADENCE408_CASE entry/rack-plan-problem |
| 4 | 4b rack-approval problem | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. Genuine underlying problem produced by real rack actions; earlier lesson guard wins. See invariant/coverage limit below; no forced stage7 facts. | resume/runtime18/cadence.log — CADENCE408_CASE entry/rack-approval-problem |
| 4 | 4b Failed | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. Ordinary annual evaluation of explicitly injected zero-cash condition creates the failure event. | resume/runtime18/cadence.log — CADENCE408_CASE entry/Failed |
| 4 | 4b closed Work Plan | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/closed-plan |
| 4 | 4b paused time | PASS | Exact first meaningful reason and refusal; saved-world nonmutation. | resume/runtime18/cadence.log — CADENCE408_CASE entry/paused |
| 4 | 4b Year100 entry | PASS | After authentic destination100, BeginSkip refuses with Year advance is unavailable prefix; no mutation. | resume/runtime18/cadence.log — CADENCE408_CASE boundary/99-to100 |
| 4 | 4c objective achieved | PASS | k=1; exact reason: An objective was achieved. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/objective-real-at25 |
| 4 | 4c genuine Completed | PASS | k=1; exact reason: Scenario outcome changed. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE boundary/24-to25 |
| 4 | 4c genuine Failed after cash condition injection | PASS | k=1; exact reason: Scenario outcome changed. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/Failed-cash-zero-injected |
| 4 | 4c storm event and victims | PASS | k=1; exact reason: Storm recorded. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/storm |
| 4 | 4c Year24→25 boundary with simultaneous objective stop isolated | PASS | k=1; exact reason: Review boundary reached. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/Year25-boundary |
| 4 | 4c Year99→100 boundary | PASS | k=1; exact reason: Review boundary reached. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE boundary/99-to100 |
| 4 | 4c derived lesson changed through real replanting resolver injection | PASS | k=0; exact reason: The lesson changed. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/lesson-real-replanting |
| 4 | 4c new Pending work | PASS | k=1; exact reason: Resolve or remove all Pending and Approved work first. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/new-pending |
| 4 | 4c new Approved work | PASS | k=1; exact reason: Resolve or remove all Pending and Approved work first. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/new-approved |
| 4 | 4c preview flag appearing | PASS | k=1; exact reason: Return to your forest first. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/preview-injected |
| 4 | 4c authoring appearing | PASS | k=1; exact reason: Return to your forest first. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/authoring |
| 4 | 4c real help modal appearing | PASS | k=1; exact reason: Finish the open question or editing action first. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/modal-between-ticks |
| 4 | 4c cancel before first tick | PASS | k=0; exact reason: Cancelled between years. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/cancel0 |
| 4 | 4c cancel after one tick | PASS | k=1; exact reason: Cancelled between years. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/cancel1 |
| 4 | 4c controller disabled | PASS | k=1; exact reason: Forest controller unavailable. Review, exact range, genuine destination acknowledgment and RejectWrongStopReason. Cancel0/lesson0 already-read destination correctly adds zero duplicate acknowledgment events. | resume/runtime18/cadence.log — CADENCE408_CASE stop/controller-disabled |
| 4 | 4c double-click on BeginSkip | PASS | Second BeginSkip rejected; no extra year or records; all N cases checked. | resume/runtime18/cadence.log — CADENCE408_CASE parity/quiet |
| 4 | 4d exception in TickSkip | PASS | k0; Stopped after an error: Object reference not set to an instance of an object; exact platform text retained. Null progress-label injection; nine-type restoration and wrong-reason oracle. | resume/runtime18/cadence.log — CADENCE408_CASE fault/exception |
| 4 | 4d AdvanceYear false on second tick | PASS | First year and annual reports preserved; exact Year advance failed feedback; nine-type restoration. | resume/runtime18/cadence.log — CADENCE408_CASE fault/false-tick2 |
| 4 | 4d UI OnDisable | PASS | Restore-only assertion; no destination-summary claim. | resume/runtime18/cadence.log — CADENCE408_CASE fault/OnDisable |
| 4 | 4d manager reference loss | PASS | Test-side reference removal; nine-type restore-only assertion, then original reference/modal restored. With no manager FinishSkip intentionally cannot display Review. | resume/runtime18/cadence.log — CADENCE408_CASE fault/manager-reference-loss |
| 4 | 4d ecology reference loss | PASS | Test-side reference removal; all nine prior states restored and reference repaired. | resume/runtime18/cadence.log — CADENCE408_CASE fault/ecology-reference-loss |
| 5 | historical approved prescriptions unchanged | PASS | Real prescription creation/approval/execution precedes skip; compare records and selections before/after ordinary annual results. | resume/runtime18/cadence.log — CADENCE408_CASE MFM/historical-approved |
| 5 | historical orders unchanged and no Pending/Approved work created | PASS | Real prescription creation/approval/execution precedes skip; compare records and selections before/after ordinary annual results. | resume/runtime18/cadence.log — CADENCE408_CASE MFM/historical-approved |
| 5 | marks unchanged and no automatic Fell mark | PASS | Real prescription creation/approval/execution precedes skip; compare records and selections before/after ordinary annual results. | resume/runtime18/cadence.log — CADENCE408_CASE MFM/historical-approved |
| 5 | rack selection unchanged | PASS | Real prescription creation/approval/execution precedes skip; compare records and selections before/after ordinary annual results. | resume/runtime18/cadence.log — CADENCE408_CASE MFM/historical-approved |
| 5 | 3D rack confirmation count unchanged | PASS | Real prescription creation/approval/execution precedes skip; compare records and selections before/after ordinary annual results. | resume/runtime18/cadence.log — CADENCE408_CASE MFM/historical-approved |
| 5 | marked Crop Tree mortality with marking disabled | PASS | Identical forced ApplyMortality action on both legs, then ordinary annual tick. Complete canonical JSON and current hash equal; marking disabled at death; after re-enable and0.7s stale sweep dead id absent and counts printed. | resume/runtime18/cadence.log — CADENCE408_CASE F9/crop-mortality |
| 6 | genuine Legacy40 eligibility | PASS | Unmodified archived v12 Year20 input, geometry0, computed stage0; exact lesson refusal. No stage7 eligibility manufactured. | resume/runtime18/cadence.log — CADENCE408_CASE compat/Legacy40 |
| 6 | frozen Reference Future Year0 | PASS | Original frozen anchor verified; current/hash-layout projection matches C-01 baseline values below. | resume/runtime19/reference.log; resume/runtime18/cadence.log |
| 6 | frozen Reference Future Year20 | PASS | Original frozen anchor verified; current/hash-layout projection matches C-01 baseline values below. | resume/runtime19/reference.log; resume/runtime18/cadence.log |
| 6 | frozen Reference Future Year50 | PASS | Original frozen anchor verified; current/hash-layout projection matches C-01 baseline values below. | resume/runtime19/reference.log; resume/runtime18/cadence.log |
| 6 | frozen Reference Future Year100 | PASS | Original frozen anchor verified; current/hash-layout projection matches C-01 baseline values below. | resume/runtime19/reference.log; resume/runtime18/cadence.log |
| 6 | frozen archive identity and deterministic continuation | PASS | Archive FNV1A42C7BD0439E72F, schedule56C8B99FA1E8DDD1, identical SHA256 against a314d308; two genuine Year50 continuations. | resume/runtime19/reference.log; resume/source-identity.json |
| 6 | destination disk save/reload state | PASS | Driver-isolated persistent path; full canonical JSON and current hash identical after Save/Load. | resume/runtime18/cadence.log — CADENCE408_CASE compatibility/disk |
| 6 | disk reload removes session-only summary | PASS | Empty text and display None after identical destination-world reload (same year). | resume/runtime18/cadence.log — CADENCE408_CASE compatibility/disk |
| 6 | old save ordinary behavior | PASS | Unmodified archived v12 world loads; real ordinary AdvanceYear reaches21. | resume/runtime18/cadence.log — CADENCE408_CASE compat/old-no-cadence |
| 6 | world with no cadence/review history | PASS | Authentic fresh world has no acknowledgment history, loads normally and conservatively refuses skip at its real early stage. | resume/runtime18/cadence.log — CADENCE408_CASE compat/old-no-cadence |
| 6 | CurrentVersion24 unchanged vs a314d308 | PASS | Entire source file byte-identical to base; not inferred from compile. | resume/source-identity.json |
| 6 | HashLayout unchanged vs a314d308 | PASS | Entire source file byte-identical to base; not inferred from compile. | resume/source-identity.json |
| 6 | true reviewed two-cycle predicate retained | PASS | Actual initial rack/release/planting cycles and real acknowledgments; predicate true before and after skip. | resume/runtime18/cadence.log — CADENCE408_CASE compat/reviewed-two-cycle |
| 6 | completion inside skip range preserves outcome/year | PASS | Direct and skip complete at25 with full JSON/hash equality; no artificial completion history. | resume/runtime18/cadence.log — CADENCE408_CASE boundary/24-to25 |
| 7 | 1280x720 disabled-work-plan | PASS | early-stage disabled skip and lesson reason; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-disabled-work-plan.png |
| 7 | 1280x720 enabled-work-plan | PASS | stage7 enabled skip and last-felling reassessment reason; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-enabled-work-plan.png |
| 7 | 1280x720 confirmation | PASS | explicit target and no-auto-work confirmation; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-confirmation.png |
| 7 | 1280x720 F7-refused-current-reason | PASS | real Pending order invalidates retained Confirm; immediate current reason and confirmation removed; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-F7-refused-current-reason.png |
| 7 | 1280x720 progress | PASS | Advancing year1 of2 and visible Cancel; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-progress.png |
| 7 | 1280x720 destination-summary | PASS | latest Year6 review, exact Years5–6 session summary; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-destination-summary.png |
| 7 | 1280x720 earlier-year | PASS | earlier Year5 with session summary retained; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-earlier-year.png |
| 7 | 1280x720 latest-year | PASS | return to latest Year6; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-latest-year.png |
| 7 | 1280x720 loaded-world-hidden-summary | PASS | different Year0 world; summary absent, empty label display None; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-loaded-world-hidden-summary.png |
| 7 | 1280x720 one-year-cleared-summary | PASS | normal one-year advance to7; summary absent; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-one-year-cleared-summary.png |
| 7 | 1280x720 native-cancel-zero-years | PASS | OS Cancel; Year4 unchanged, No years advanced. Cancelled between years.; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1280x720-native-cancel-zero-years.png |
| 7 | 1920x1080 disabled-work-plan | PASS | early-stage disabled skip and lesson reason; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-disabled-work-plan.png |
| 7 | 1920x1080 enabled-work-plan | PASS | stage7 enabled skip and last-felling reassessment reason; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-enabled-work-plan.png |
| 7 | 1920x1080 confirmation | PASS | explicit target and no-auto-work confirmation; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-confirmation.png |
| 7 | 1920x1080 F7-refused-current-reason | PASS | real Pending order invalidates retained Confirm; immediate current reason and confirmation removed; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-F7-refused-current-reason.png |
| 7 | 1920x1080 progress | PASS | Advancing year1 of2 and visible Cancel; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-progress.png |
| 7 | 1920x1080 destination-summary | PASS | latest Year6 review, exact Years5–6 session summary; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-destination-summary.png |
| 7 | 1920x1080 earlier-year | PASS | earlier Year5 with session summary retained; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-earlier-year.png |
| 7 | 1920x1080 latest-year | PASS | return to latest Year6; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-latest-year.png |
| 7 | 1920x1080 loaded-world-hidden-summary | PASS | different Year0 world; summary absent, empty label display None; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-loaded-world-hidden-summary.png |
| 7 | 1920x1080 one-year-cleared-summary | PASS | normal one-year advance to7; summary absent; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-one-year-cleared-summary.png |
| 7 | 1920x1080 native-cancel-zero-years | PASS | OS Cancel; Year4 unchanged, No years advanced. Cancelled between years.; personally opened and inspected, nonblack and claimed state present. | resume/runtime22/evidence/1920x1080-native-cancel-zero-years.png |
| 7 | normal 4-year wall time | PASS | No between-tick hold; actual completed tick count asserted; exact measured seconds in timing section. | resume/runtime18/cadence.log — CADENCE408_CASE timing/normal4 |
| 7 | stress 5-year wall time | PASS | No between-tick hold; actual completed tick count asserted; exact measured seconds in timing section. | resume/runtime18/cadence.log — CADENCE408_CASE timing/stress5 |
| 8 | 37 host save/Prefs hashes unchanged | PASS | Each resumed acquisition/release compares the original37, not a new baseline. | custody-before.json; resume/final-custody.json |
| 8 | qualification lane clean | PASS | No disposable sources/metas; only reports and runner commits. | resume/final-state.json |
| 8 | Assets identical to e7591933 | PASS | No production edit; Editor material/settings drift preserved then restored. | resume/lease*/assets-*.diff; final-state.json |
| 8 | candidate HEAD/tree recorded | PASS | Origin source branch still exact e7591933. | resume/source-identity.json; lease13/origin-candidate.txt |
| 8 | all leases released after Editor exit | PASS | Normal releases; no process kill or forced release. Orchestrator release of9b90eca3 included below. | resume/lease-history.json; no-editor-*.json |
| 8 | disk floor preserved | PASS | Start, periodic and release samples above16,106,127,360 bytes; no continuous-monitoring claim. | disk-before.json; resume/disk-*.json; lease*/disk-*.json |

## Fixture authenticity, injections and coverage limits

The stage7 fixture earns its latch through real rack geometry/confirmation/entrance marks, three distributed Crop Trees, real release and deadwood outcome, real player InspectTree/ObserveRackInspection/DecideRackInspection actions on three future trees and six stand portions, nursery purchases/planting execution and ordinary establishment-year review. No private lesson-field writes, fake acknowledgments or edited save JSON. The post-felling fixture executes an additional real felling after teaching; its target derives from that felling year. Full parity compares all saved fields each tick; this includes the required ecology/juveniles/deadwood/cash/objective/outcome facts, not just a subset checksum.

Explicit disposable condition injections: UI editing flags for preview/rack/consent/reset/reopen/drag/ghost/warning; zero cash for an outcome evaluated by the ordinary annual code; negative cash for AdvanceYear false; missing UI references/progress label for fault exits; real ApplyMortality for invalid targets and Crop Tree death; ForceStormNextYear for a real storm; real replanting through ResolveOrder at the current year to change the derived lesson before a tick. These do not claim normal player affordances, ecological likelihood or calibration validity. No fabricated management event or acknowledgment is used in final qualification. Old direct-SetOutcome and constructed-order diagnostics were removed before the final run; their PASS markers do not qualify those cases.

The genuine Year24 world normally completes at25, so its correct higher-priority reason is Scenario outcome changed. For separate objective/boundary checks, real mortality API calls remove enough original canopy to prevent simultaneous completion while preserving earned teaching history. The objective case observes the real new Year25 achievement. Only the controller's test-side skipObjectives observation counter is set to int.MaxValue for the boundary-only case; authoritative objective/event/outcome/lesson state is untouched, and complete direct-path JSON parity is required. The failed attempt which anticipated a future outcome is retained and explained below.

Rack-plan/rack-approval negatives use genuine states and real actions, with the **actual first reason** recorded. The hard teaching safeguard is exceeded at stage2 (41% vs40%); its earlier lesson guard refuses skip. A real confirmed plan awaiting entrance marking is also an earlier lesson state. The hard safeguard is Advisory after the completed-review latch; rack approval requires open rack work, which the Pending/Approved guard already rejects. No independent stage7 execution claim for these masked lower-priority branches is made, and no lesson fact is forced to reach an impossible combination. The requested observable negatives (problem present, exact refusal and no mutation) pass; this is not a mutation-sensitivity claim for unreachable branch combinations. RackTool active/intro and map editing flags are labeled test-side conditions in the valid stage7 world.

A zero-tick cancellation returns to a review already genuinely read. Its acknowledgment is idempotent and adds zero duplicate events. Destinations with a newly advanced year add exactly one real event even when the acknowledgment handler is invoked twice. Manager-reference loss and OnDisable are restore-only fault checks; a missing manager cannot render its destination Review. No summary success is inferred for that path.

The real suggested range is at most4 years under the accepted q+4 rule. N5 is explicitly harness-only stress. Other parity lengths use only the test controller target override; production calculator, annual simulation and saved state are unchanged.


## Per-year hashes — all60 printed records

A compares complete canonical JSON and current-world hash after every tick. B uses semantic normalization only; no raw UI/skip whole-world hash equality is asserted.

| Fixture | Leg | N | Tick | Year | CurrentWorldHash |
|---|---|---:|---:|---:|---|
| quiet | direct | 1 | 1 | 5 | D5C2997D4B56176F |
| quiet | skip | 1 | 1 | 5 | D5C2997D4B56176F |
| quiet | direct | 2 | 1 | 5 | D5C2997D4B56176F |
| quiet | direct | 2 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | skip | 2 | 1 | 5 | D5C2997D4B56176F |
| quiet | skip | 2 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | direct | 3 | 1 | 5 | D5C2997D4B56176F |
| quiet | direct | 3 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | direct | 3 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | skip | 3 | 1 | 5 | D5C2997D4B56176F |
| quiet | skip | 3 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | skip | 3 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | direct | 4 | 1 | 5 | D5C2997D4B56176F |
| quiet | direct | 4 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | direct | 4 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | direct | 4 | 4 | 8 | 26C98E82B0BE2D8C |
| quiet | skip | 4 | 1 | 5 | D5C2997D4B56176F |
| quiet | skip | 4 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | skip | 4 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | skip | 4 | 4 | 8 | 26C98E82B0BE2D8C |
| quiet | direct | 5 | 1 | 5 | D5C2997D4B56176F |
| quiet | direct | 5 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | direct | 5 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | direct | 5 | 4 | 8 | 26C98E82B0BE2D8C |
| quiet | direct | 5 | 5 | 9 | F4516798470C4FB7 |
| quiet | skip | 5 | 1 | 5 | D5C2997D4B56176F |
| quiet | skip | 5 | 2 | 6 | 8B0BDF75F9BC3816 |
| quiet | skip | 5 | 3 | 7 | 1984FFF2B7579D27 |
| quiet | skip | 5 | 4 | 8 | 26C98E82B0BE2D8C |
| quiet | skip | 5 | 5 | 9 | F4516798470C4FB7 |
| post-felling | direct | 1 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | skip | 1 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | direct | 2 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | direct | 2 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | skip | 2 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | skip | 2 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | direct | 3 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | direct | 3 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | direct | 3 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | skip | 3 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | skip | 3 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | skip | 3 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | direct | 4 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | direct | 4 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | direct | 4 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | direct | 4 | 4 | 9 | 5B4DB7305AF51AC2 |
| post-felling | skip | 4 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | skip | 4 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | skip | 4 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | skip | 4 | 4 | 9 | 5B4DB7305AF51AC2 |
| post-felling | direct | 5 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | direct | 5 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | direct | 5 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | direct | 5 | 4 | 9 | 5B4DB7305AF51AC2 |
| post-felling | direct | 5 | 5 | 10 | 3180A80F8EC4D3F9 |
| post-felling | skip | 5 | 1 | 6 | BC5F18E0283EDF2B |
| post-felling | skip | 5 | 2 | 7 | 7B2A9B73FA119189 |
| post-felling | skip | 5 | 3 | 8 | FFB99D03EF9B29D8 |
| post-felling | skip | 5 | 4 | 9 | 5B4DB7305AF51AC2 |
| post-felling | skip | 5 | 5 | 10 | 3180A80F8EC4D3F9 |

## Exact reason, outcome, timing and reference output

```text
CADENCE403_STAGE actual=7 problem=
CADENCE408_REASON early-original exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON closed-plan exact=Open the Work Plan first.
CADENCE408_REASON paused exact=Finish the open question or editing action first.
CADENCE408_REASON modal exact=Finish the open question or editing action first.
CADENCE408_REASON authoring exact=Return to your forest first.
CADENCE408_REASON controller exact=Forest controller unavailable.
CADENCE408_AUTHENTIC_ORDER status=Pending invalid=False prescription=0 validation= reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Pending invalid=False prescription=1 validation= reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Pending invalid=True prescription=0 validation=Target tree is missing or already felled. reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Pending invalid=True prescription=1 validation=Target tree is missing or already felled. reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Approved invalid=False prescription=0 validation= reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Approved invalid=False prescription=1 validation= reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Approved invalid=True prescription=0 validation=Target tree is missing or already felled. reason=Resolve or remove all Pending and Approved work first.
CADENCE408_AUTHENTIC_ORDER status=Approved invalid=True prescription=1 validation=Target tree is missing or already felled. reason=Resolve or remove all Pending and Approved work first.
CADENCE408_FAILED condition=injected cash zero; production annual evaluation event/outcome year=5 direct=18D52E797C73172D skip=18D52E797C73172D
CADENCE408_REFERENCE year=0 current=321AFE4EC28E90B0 world=ECD870EB5170D1A5
CADENCE408_REFERENCE year=20 current=360C2B9EFFB8CBB3 world=89A8DC4ECC99EDFA
CADENCE408_REFERENCE year=50 current=F0387C1BE0A9DA80 world=F640255094DB3945
CADENCE408_REFERENCE year=100 current=086A3EF3B75DE6F0 world=45919D63C5EEAAD1
CADENCE408_REASON stage-0 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-1 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-2 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-3 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-4 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-5 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON stage-6 exact=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_REASON unread exact=Read and acknowledge the latest Annual Review first.
CADENCE408_REASON preview exact=Return to your forest first.
CADENCE408_REASON reset exact=Finish the open question or editing action first.
CADENCE408_REASON reopen exact=Finish the open question or editing action first.
CADENCE408_REASON consent exact=Finish the open question or editing action first.
CADENCE408_REASON drag exact=Finish the open question or editing action first.
CADENCE408_REASON ghost exact=Finish the open question or editing action first.
CADENCE408_REASON help exact=Finish the open question or editing action first.
CADENCE408_REASON session exact=Finish the open question or editing action first.
CADENCE408_REASON rack-active exact=Finish the open question or editing action first.
CADENCE408_REASON rack-intro exact=Finish the open question or editing action first.
CADENCE408_REASON advance-warning exact=Finish the open question or editing action first.
CADENCE408_REASON ecology exact=Forest controller unavailable.
CADENCE408_REASON Failed exact=Year advance is unavailable. The scenario has failed. Review the outcome in the Work Plan.
CADENCE408_FALSE exact=Session only: advanced Years 5–5. Year advance failed: Approved work costs €0.00; available cash is €-0.01. Browse earlier years below; only the latest review can be acknowledged.
CADENCE408_MORTALITY direct=9F04788D7A569F07 skip=9F04788D7A569F07
CADENCE408_MORTALITY counts crop=2 fell=0
CADENCE408_LEGACY stage=0 reason=Finish the current lesson first. Advance 1 Year is still available where permitted. version=12
CADENCE408_STORM victims=446 events=1 reason=Session only: advanced Years 5–5. Storm recorded. Browse earlier years below; only the latest review can be acknowledged.
CADENCE408_LESSON_INJECTION real purchased/designated/approved planting through production ResolveOrder at current year; no lesson-field/event/save-JSON writes; stage=6
CADENCE408_TIMING n=5 actual=5 seconds=8.263284 stressOverride=True
CADENCE408_TIMING n=4 actual=4 seconds=6.560405 stressOverride=False
CADENCE408_BOUNDARY start=24 stage=7 reason=
CADENCE408_BOUNDARY destination=25 summary=Session only: advanced Years 25–25. Scenario outcome changed. Browse earlier years below; only the latest review can be acknowledged.
CADENCE408_BOUNDARY_PARITY year=25 direct=DC5E883C1BC6DC6E skip=DC5E883C1BC6DC6E outcome=Completed
CADENCE408_BOUNDARY_CONDITION mortalityApiDeaths=945 year=24 no objective/event/lesson fields changed
CADENCE408_BOUNDARY_PRIORITY year=25 reason=An objective was achieved. outcome=Active direct=1FEE9FDABC95EBA5 skip=1FEE9FDABC95EBA5 objectiveCounterOverride=False
CADENCE408_BOUNDARY_PRIORITY year=25 reason=Review boundary reached. outcome=Active direct=1FEE9FDABC95EBA5 skip=1FEE9FDABC95EBA5 objectiveCounterOverride=True
CADENCE408_BOUNDARY start=99 stage=7 reason=
CADENCE408_BOUNDARY destination=100 summary=Session only: advanced Years 100–100. Review boundary reached. Browse earlier years below; only the latest review can be acknowledged.
CADENCE408_BOUNDARY_PARITY year=100 direct=BF48E2EF7B207061 skip=BF48E2EF7B207061 outcome=Completed
CADENCE408_RACK_PLAN underlying=Teaching rack safeguard exceeded: the teaching footprint beside the middle rack has 41% of its trunk area removed or planned for removal (limit 40% cumulative). Cancel/unmark in-footprint removals before approval or advance. Already executed excess cannot complete this lesson. skip=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_RACK_APPROVAL underlying=Mark the rack entrances in the forest before approving the rack work: 0 / 5 marked. [P] spray marker, aim within 3.5 m, left-click. skip=Finish the current lesson first. Advance 1 Year is still available where permitted.
CADENCE408_OLD version=12 ordinaryDestination=21
CADENCE408_LESSON_INJECTION real purchased/designated/approved planting through production ResolveOrder at current year; no lesson-field/event/save-JSON writes; stage=6
CADENCE403_PASS checks=418 shots=0
```

Reference contract final output:

```text
REFERENCE_ARCHIVE_WORLD year=0 embeddedHash=A564039D9B7CE31D stored=A564039D9B7CE31D pinned=True version=12
REFERENCE_ARCHIVE_WORLD year=20 embeddedHash=F7DF7DAB53B6FD32 stored=F7DF7DAB53B6FD32 pinned=True version=12
REFERENCE_ARCHIVE_WORLD year=50 embeddedHash=D5E75D6D21D631AC stored=D5E75D6D21D631AC pinned=True version=12
REFERENCE_ARCHIVE_WORLD year=100 embeddedHash=7AD177B3CC2F73C7 stored=7AD177B3CC2F73C7 pinned=True version=12
REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule=56C8B99FA1E8DDD1 archiveFileFnv=1A42C7BD0439E72F
REFERENCE_ARCHIVE_NEGATIVE_CONTROL_PASS tamperedYear100Value=detected
REFERENCE_PREVIEW_PASS year=0 trees=336 worldHash=A564039D9B7CE31D
REFERENCE_PREVIEW_PASS year=20 trees=352 worldHash=F7DF7DAB53B6FD32
REFERENCE_PREVIEW_PASS year=50 trees=401 worldHash=D5E75D6D21D631AC
REFERENCE_PREVIEW_PASS year=100 trees=517 worldHash=7AD177B3CC2F73C7
REFERENCE_CONTINUATION_PASS year50=v12 year100=v24 deterministic=True continuedHash=EBD49ADEF18C3F36 legacyV15LayoutHash=BFFF66015A6619DA
REFERENCE_FUTURE_V1_CONTRACT_PASS archive=True preview=True continuation=True replay=NotRequired
REFERENCE_FUTURE_V1_PASS
```

C-01 comparison source: `/media/jer/Files/CCF/Operations/CCF-393-Report/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-FOCUSED-INTEGRATION-GATES-RESULT.md`. The four **CurrentWorldHash / WorldHash projections** match exactly: Y0 `321AFE4EC28E90B0 / ECD870EB5170D1A5`, Y20 `360C2B9EFFB8CBB3 / 89A8DC4ECC99EDFA`, Y50 `F0387C1BE0A9DA80 / F640255094DB3945`, Y100 `086A3EF3B75DE6F0 / 45919D63C5EEAAD1`. These are a different hash domain from original embedded-v12 frozen anchors `A564039D9B7CE31D`, `F7DF7DAB53B6FD32`, `D5E75D6D21D631AC`, `7AD177B3CC2F73C7`. Archive integrity uses original embedded JSON, not a current-class reserialization. Archive SHA256 `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc`; schedule SHA256 `91cf50595e9b3337438323ef09609a70d7de11374569dd9fb3c6359fbdb798f0`. Both equal a314d308 and e7591933.

`runtime7` failed after archive integrity because the contract harness's initial archive.Matches requires Legacy40 geometry but its scene started with the normal Enlarged80 default. The exception was “the archive loader did not verify every frozen milestone for this scenario”. `runtime7b` passed with the documented Editor-only `CCF_STAND_GEOMETRY=0` override. The final reference run repeats that explicit configuration with unchanged harness bytes, not a production fix or archive edit. Normal Enlarged80 cadence is qualified separately by the full feature run.


Driver classifier correction: runtime18 Editor exit0,418 checks, all case markers PASS and no NOT_RUN. The original Python driver exited1 because its substring search treated the positive CADENCE408_FAILED outcome diagnostic as a failure. Original result/log are unchanged. resume/reclassify.py reran the corrected marker/postcondition parser on the same retained log, exit0; runtime18/classifier-rerun.json records input hashes, zero actual failure lines and seven sensitivity checks including real case/check/top/reference/compiler failures. No assertion or Editor result was weakened, and no Unity rerun was needed for a pure log parser.


## Screenshots and actual input

All 22 qualifying Game captures were opened and visually inspected, including full review text, year labels and controls. No black/blank image was accepted. Contact sheets `resume/final-UI22-shots-1.png` … `6.png` retain that review grouping. Each image path and what it shows appears on its own matrix line. Native pointer/keyboard operations and before/after private desktops are in `resume/runtime22/driver/`; desktop zoom was0.44x, while Game captures are exactly1280×720 or1920×1080. This is Editor Game-view qualification, not a Player build. The F7 refusal intentionally invokes the retained registered Confirm callback immediately after real Pending work appears, to test the atomic refusal before periodic redraw; normal confirmation, browsing and Cancel use OS pointer events.

All earlier captured Game images and auxiliary desktops were also opened via the saved contact sheets. `runtime4`, `runtime10`, `resume/runtime13` and `resume/runtime14` files named confirmation actually show an unconfirmed Work Plan; those failed input attempts are not confirmation evidence. Final 22 captures do show their claims. An image audit with dimensions/SHA256 is saved at resume/screenshot-audit.json; it includes all diagnostic images and explicitly flags the five misnamed failed-confirmation captures.


## Commands, exits and all attempts

Exact resumed launcher argument arrays are `resume/runtime*-command.json`, exit receipts `runtime*-exit.json`, driver stdout `runtime*-driver.log`; each Editor `result.json` records its own exit, elapsed seconds, HEAD, source hashes and leftovers. `resume/run.py` and `lease.py` are the exact external wrappers. `resume/commands.jsonl` records preflight/acquire/release commands with exits/stdout/stderr. Initial source-reading commands are read-only; prior-worker launcher argv was not retained verbatim, so no exact reconstructed shell command is invented. Their actual Editor command lines and exits remain in their logs/results.

| Run | Editor exit | Driver verdict | Runner HEAD | Seconds | Log |
|---|---:|---|---|---:|---|
| runtime1 | 0 | PASS | 8fb97dfd4c04021c452d58bff6a5f40e69e1e8ac | 336.5 | runtime1/cadence.log |
| runtime2 | 0 | PASS | 8fb97dfd4c04021c452d58bff6a5f40e69e1e8ac | 116.1 | runtime2/cadence.log |
| runtime3 | 0 | FAIL | 087a9e160d427abc081001db079f5b180b41dc33 | 190.2 | runtime3/cadence.log |
| runtime4 | 1 | FAIL | 2c8d4dff4a59b1cb770ca09eda64d8d11e44bcaa | 110.1 | runtime4/cadence.log |
| runtime5 | 0 | PASS | d5b35e14f3a4d32878b870a9df69b100dd8529c2 | 140.1 | runtime5/cadence.log |
| runtime6 | 0 | FAIL | d7f8e8b63d6b1f56a04e0d7b04a0f11235dbfec6 | 579.7 | runtime6/cadence.log |
| runtime7 | 1 | FAIL | d7f8e8b63d6b1f56a04e0d7b04a0f11235dbfec6 | 174.9 | runtime7/reference.log |
| runtime7b | 0 | PASS | d7f8e8b63d6b1f56a04e0d7b04a0f11235dbfec6 | 98.1 | runtime7b/reference.log |
| runtime8 | 0 | PASS | 4d847bf380ad02415a2726e7bb060c94310df23d | 176.1 | runtime8/cadence.log |
| runtime9 | 0 | FAIL | 4d847bf380ad02415a2726e7bb060c94310df23d | 482.7 | runtime9/cadence.log |
| runtime10 | 1 | FAIL | 4d847bf380ad02415a2726e7bb060c94310df23d | 82.1 | runtime10/cadence.log |
| resume/runtime11 | 1 | FAIL | 9519e0c692f86aa7f7a3da69d3549e1e486c7541 | 413.1 | resume/runtime11/cadence.log |
| resume/runtime12 | 1 | FAIL | 9be30be8be4b8c8bbe97f52c6ecb7451319a45f1 | 108.1 | resume/runtime12/cadence.log |
| resume/runtime13 | 1 | FAIL | 9be30be8be4b8c8bbe97f52c6ecb7451319a45f1 | 78.1 | resume/runtime13/cadence.log |
| resume/runtime14 | 1 | FAIL | 4d7940422c5ae10d2c3713cff4bf555d854508a2 | 112.5 | resume/runtime14/cadence.log |
| resume/runtime15 | 1 | FAIL | e2b447116c9e33d076b1c6888e17114b2ceed20a | 98.1 | resume/runtime15/cadence.log |
| resume/runtime16 | 0 | PASS | 526e13ce719fdeeb5b62286227d7fb1004432f0f | 242.3 | resume/runtime16/cadence.log |
| resume/runtime17 | 1 | FAIL | eb69cb0300f4176cf9fd409d1f63b1ee107945cb | 646.8 | resume/runtime17/cadence.log |
| resume/runtime18 | 0 | FAIL | a64b01a995afaf081f4b20084ee4552fa6bea53a | 660.9 | resume/runtime18/cadence.log |
| resume/runtime19 | 0 | PASS | a64b01a995afaf081f4b20084ee4552fa6bea53a | 100.1 | resume/runtime19/reference.log |
| resume/runtime20 | 1 | FAIL | b8e90fbe4e11806e98e23725fc952c11592e5b86 | 124.1 | resume/runtime20/cadence.log |
| resume/runtime21 | 0 | PASS | b8e90fbe4e11806e98e23725fc952c11592e5b86 | 140.1 | resume/runtime21/cadence.log |
| resume/runtime22 | 0 | PASS | b8e90fbe4e11806e98e23725fc952c11592e5b86 | 232.2 | resume/runtime22/cadence.log |

Final repeat commands (all command arrays and exits preserved):

```bash
source /media/jer/ZX20/CCF-Ops/environment.sh
/media/jer/ZX20/CCF-Ops/scripts/lane-status --storage-only
/media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json
pgrep -af '^/media/jer/ZX20/Unity/.*/Editor/Unity( |$)' #1 means no Editor
python3 -B <evidence>/resume/lease.py acquire lease13 #0, clean actual branch/HEAD, empty Assets diff,37 hashes
python3 -B <evidence>/resume/run.py runtime18 SUITE #see exact argv/exit JSON
# Save own drift patch, restore only enumerated Editor drift from e7591933; no disposable Assets/meta left.
# Reference command is preserved verbatim in resume/runtime19-command.json.
python3 -B <evidence>/resume/lease.py release lease13 #0 after Editor exit
# Native positive-control correction was committed before fresh lease14; runtime21 KEYS and runtime22 UI, then normal release14.
TMPDIR=<Files evidence scratch> python3 Tools/Verification/CadenceDualAdvance/run_offline.py #offline receipt below
```

Native-input environment correction: runtime20's positive F5/F9 probes passed, but positive F10 failed because GTK3's default F10 accelerator opened the Linux Editor File menu. This was a real runner/environment limitation, not a game PASS. Runtime21 disables only `gtk-menu-bar-accel` in its private XDG_CONFIG_HOME/gtk-3.0/settings.ini, explicitly focuses/clicks the Game view, proves all three unlocked game handlers work, then proves all three are unavailable while the nine controls are disabled. Runtime22 uses the same private profile. No host GTK preference, production source, package, Player or global keymap was altered.

Every resumed Editor launched through the inspected isolated-XDG driver; licence copies were removed after each run. No licence198/terms prompt, force lock, reaper or owned-Editor kill was needed. Initial relative `scripts/...` calls from Primary returned127 because sourcing environment.sh exports variables but does not cd; rerun with absolute Ops script paths passed. `pgrep` exit1 was the expected no-process proof. These command errors are not gameplay failures.

## Final native positive controls and isolated driver changes

Runtime21 exited0 with67 checks; runtime22 exited0 with124 checks/22 Game captures. Relevant actual output:

```text
CADENCE403_STAGE actual=7 problem=
TEACHING_CAP_CHECK PASS positive OS F5 reaches game save handler
TEACHING_CAP_CHECK PASS positive OS F9 reaches game load handler
TEACHING_CAP_CHECK PASS positive OS F10 reaches game session menu
TEACHING_CAP_CHECK PASS OS keys leave skip running and menu unavailable
TEACHING_CAP_CHECK PASS OS save/load keys leave slot bytes unchanged
TEACHING_CAP_CHECK PASS OS save/load keys neither write nor reload the actual isolated world
CADENCE408_CASE locks/native-F5-F9-F10 PASS
CADENCE403_PASS checks=67 shots=0
```

The driver creates this only under the run's private `XDG_CONFIG_HOME` before launching the Editor:

```ini
[Settings]
gtk-menu-bar-accel=
```

File: `config/gtk-3.0/settings.ini`. Without this private GTK setting, the Linux Editor reserves F10 for its File menu; native game-menu delivery is not claimed for that default Editor profile. Positive controls prove delivery with the declared profile, then the locked loop proves effective suppression. No global GTK setting was changed.

The corrected Python failure classifier uses `re.search(r"(?<![A-Za-z])FAIL(?:$|[\s:])", line)` or `error CS`, rather than accepting any occurrence of the substring FAIL as a failed test. Original drivers/results are preserved in `resume/run403-before-classifier.py` and `resume/run403-before-gtk.py`; current executable driver remains at the evidence root. Exact gate argv/exit and source manifests are retained per run.

## Earlier failures, exact outputs and repair classification

All originals remain intact. These were runner/fixture failures, not waived candidate failures:

| Attempts / exact output | Diagnosis and final replacement |
|---|---|
| runtime3 stop/cancel0: “exactly one genuine destination acknowledgment” | The zero-tick destination had already been read; requiring a duplicate event was wrong. Final checks correct idempotent delta0, versus delta1 for a newly advanced year. |
| runtime3 stop/modal-between-ticks: “stop before next tick: Finish the open question or editing action first.” | Original injection set manager.ModalOpen to its already-true skip-owned value; it did not create an additional decision. Real help modal demonstrably opens and produces the expected pre-tick stop. |
| runtime6 boundaries: “authentic boundary eligibility Read and acknowledge the latest Annual Review first.” | AcknowledgeAnnualReview refuses outside Review. Final setup opens Review and uses the real handler at every year. |
| runtime6 entry/session NullReferenceException; rack-active “negative reason Finish the open question” | Direct-entry fixture had no initialized real session menu; created and settled the real component. RackTool.Enter correctly refuses V2; its generic active-flag guard is explicitly tested as an injection, not claimed as a V2 interaction. |
| runtime9 lesson-real-unmark: “stop before next tick: The lesson changed.” | Unmarking does not change the completed historical lesson; diagnostic logs stage7 and target stop. Real replanting derives stage6 and the final test checks the lesson stop. |
| runtime9 manager loss NullReferenceException; ecology loss “loss begin”; later one-year/native/timing “... begins”; boundaries “Finish the open question or editing action first.” | Check's optional stage snapshot dereferenced a deliberately missing manager; lost-reference cleanup then left the original manager modal. Fixed instrumentation and explicit fixture restoration; all later cases rerun. |
| runtime10 UiMatrix NullReferenceException line297; runtime13/14 missing Confirm | Native coordinates omitted the Game-view toolbar/dock offset. Captured private desktop proves the click miss; targetInParent mapping fixes it. |
| runtime11 lesson-clearance “Sequence contains no matching element” | This authentic cell-planted fixture has no exact living planted juvenile. Replaced the nonexistent-clearance target with real purchased/designated/approved replanting through ResolveOrder. |
| runtime11 native “OS-input driver directory required” | Diagnostic launch omitted --native; final exact command includes it. |
| runtime12 rack-approval “authentic rack-approval problem present” | Stage0 snapshot was before rack creation. Final fixture actually creates/confirms five racks and leaves entrances unmarked. |
| runtime15 native center NaN / xdotool -2147483648 error | Newly rebuilt footer had not completed layout. NativeButton settles and reacquires the current button before computing coordinates. |
| runtime17 boundary isolation: expected Years25–25 Review boundary; actual “No years advanced. Scenario outcome changed.” | Setting a future skipOutcome failed the **pre-tick** check. Removed the override; use an actual biological condition which prevents completion and preserve complete direct parity. |

Runtime4's Return-key activation was also ineffective; final tests use pointer OS events. Synthetic-order and direct-SetOutcome PASS markers in old runs are discarded as nonqualifying; final real-handler replacements are listed in the matrix. A top-level CADENCE403_PASS in early runs did not imply all cases passed; final runner counts case failures and exits nonzero. Existing CadenceParity.cs assertions were never modified. The copied Cadence408Parity preserves them and adds per-year hash logging.

## Custody, lease history and storage

Initial host37 hashes match the inherited custody-before.json exactly; every resumed acquisition/release and final comparison matches that same baseline. Full37-path/hash inventory is below and in custody-before.json / resume/final-custody.json. Save/load used only the driver's isolated persistent path under this evidence folder. Original in-memory fixture world restored in finally. Protected #382 personal checkout/saves, U1/U2/U3, main, C-01 base, browser branch and frozen assets were not written. Primary's unrelated AGENTS.md and StorageArchitecture.md work was preserved.

Each lease uses the actual clean qualifier branch/HEAD and production Assets diff. Release follows complete Editor exit and anchored pgrep exit1. Prior worker's first five no-Editor proofs are in root/lease2…5; lease6 was normally released by Fable after independent no-Editor inspection at10:43:37Z. It has no worker-written final-state file because of the interruption; the exact lock-history release and issue comment are the evidence, not an invented worker receipt. Resume rehashed custody successfully before further work.

| Lease id | Acquired UTC | Released UTC | Exact runner HEAD | Exit proof / release record |
|---|---|---|---|---|
| 2cdbef655e9c4adcb405dde24d849fec | 2026-11-12T09:52:56.079938+00:00 | 2026-11-12T10:01:21.345953+00:00 | 8fb97dfd4c04021c452d58bff6a5f40e69e1e8ac | anchored pgrep1 in corresponding receipt; normal release |
| 37cd6b6d7f9e42df8b56db3afdeafd5d | 2026-11-12T10:01:42.430028+00:00 | 2026-11-12T10:05:03.363812+00:00 | 087a9e160d427abc081001db079f5b180b41dc33 | anchored pgrep1 in corresponding receipt; normal release |
| deeeb2eed8ec47bcb50cb146aa391379 | 2026-11-12T10:05:17.863555+00:00 | 2026-11-12T10:07:47.987858+00:00 | 2c8d4dff4a59b1cb770ca09eda64d8d11e44bcaa | anchored pgrep1 in corresponding receipt; normal release |
| 138587d0f2da4412ad0d9df4ca7ec9d8 | 2026-11-12T10:08:08.030708+00:00 | 2026-11-12T10:10:46.844578+00:00 | d5b35e14f3a4d32878b870a9df69b100dd8529c2 | anchored pgrep1 in corresponding receipt; normal release |
| 0091fec12a6e40b8810309a5495b2d67 | 2026-11-12T10:11:06.690679+00:00 | 2026-11-12T10:27:48.115708+00:00 | d7f8e8b63d6b1f56a04e0d7b04a0f11235dbfec6 | anchored pgrep1 in corresponding receipt; normal release |
| 9b90eca38be24ffdb963fcb617721415 | 2026-11-12T10:28:11.174983+00:00 | 2026-11-12T10:43:44.613647+00:00 | 4d847bf380ad02415a2726e7bb060c94310df23d | Fable coordinator inspection10:43:37Z; normal release10:43:44Z |
| f0a06327294c41abaa6bf026f1c883dc | 2026-11-12T10:51:10.636207+00:00 | 2026-11-12T10:58:28.969955+00:00 | 9519e0c692f86aa7f7a3da69d3549e1e486c7541 | anchored pgrep1 in corresponding receipt; normal release |
| 903d859cf7a44374bf3018de38bf7053 | 2026-11-12T10:58:53.695504+00:00 | 2026-11-12T11:02:49.844369+00:00 | 9be30be8be4b8c8bbe97f52c6ecb7451319a45f1 | anchored pgrep1 in corresponding receipt; normal release |
| 62c7156990a44a20ae165f02514f70f8 | 2026-11-12T11:03:34.413089+00:00 | 2026-11-12T11:06:13.446582+00:00 | 4d7940422c5ae10d2c3713cff4bf555d854508a2 | anchored pgrep1 in corresponding receipt; normal release |
| 43830ce707064cb18c4f173e567738da | 2026-11-12T11:06:18.481334+00:00 | 2026-11-12T11:08:32.656763+00:00 | e2b447116c9e33d076b1c6888e17114b2ceed20a | anchored pgrep1 in corresponding receipt; normal release |
| c68e7301018f4a6691303eb3ac1b223d | 2026-11-12T11:08:37.456778+00:00 | 2026-11-12T11:13:52.425627+00:00 | 526e13ce719fdeeb5b62286227d7fb1004432f0f | anchored pgrep1 in corresponding receipt; normal release |
| de24a543ce084dc9b75baa55ec67d65c | 2026-11-12T11:13:57.253136+00:00 | 2026-11-12T11:27:13.185523+00:00 | eb69cb0300f4176cf9fd409d1f63b1ee107945cb | anchored pgrep1 in corresponding receipt; normal release |
| b153769589c04b539165e0c0754c743f | 2026-11-12T11:27:19.034840+00:00 | 2026-11-12T11:41:07.507306+00:00 | a64b01a995afaf081f4b20084ee4552fa6bea53a | anchored pgrep1 in corresponding receipt; normal release |
| 98d069b2217046429e81305ce76d3aa2 | 2026-11-12T11:46:31.749370+00:00 | 2026-11-12T11:59:49.228823+00:00 | b8e90fbe4e11806e98e23725fc952c11592e5b86 | anchored pgrep1 in corresponding receipt; normal release |

Final lock status free, Editor PIDs [], final pgrep1; source Assets diff empty and qualifier clean. Start ZX20 free72,752,300,032 bytes; final **72752168960** bytes; floor16,106,127,360. All retained start/release/periodic disk samples exceed floor; not a continuous monitor. Root scratch avoided; durable evidence stays on Files. Final-state/lock/publication receipts are external to keep lane clean after the report commit.

```json
{
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Player-prev.log": "57461595f38c5385f41d69a17a8fd6d23c4b50001b2b5613c15f88355f1ccac7",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Player.log": "97073207b4ad8611386f3cf61fa2ae6176446f979b380b85b1ecbcde416ca671",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Editor/Analytics/values": "a45e365bb8856e9b7350db4d65c32006da85b66df33615a60a5c75d69f29e6c8",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Insights/AppInfo/unity_insights_app_info.bin": "cdf9647327be619ad5b3feab860956886d2484c2805f3420b93cc2a7c0a4c9e5",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Insights/ArchivedEvents/Session/session_end_event": "91b11d75073f0d6e029004e62de25444c0759044e0116995a689acee5103871e",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/forest-save.json.bak": "c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/prefs": "c16ff426930f16c4c74b52a2b0ecd259142599943484b44de1164841f560d71e",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/prefs.bak-20261017-before-resolution-fix": "2af15cebc666fa5c5bccd103c8dd4f6e367653c060e5c710f9c20b53a23b5307",
  "/home/jer/.config/unity3d/DefaultCompany/CCF/save-backup-20261104/forest-save.json": "7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Editor/Analytics/values": "1141062c5e6c3ae69fd24e04cfbeb0d7073d87126d2c560dd82f231cd5426757",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Insights/AppInfo/unity_insights_app_info.bin": "cdf9647327be619ad5b3feab860956886d2484c2805f3420b93cc2a7c0a4c9e5",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/4b586042-c364-4594-8b89-c5e17986018a/Insights/ArchivedEvents/Session/session_end_event": "481353ee3ddcb3fcabe4e2114918fa3cfcc335adb4ef1c0a257a4021f0b2c364",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/local.609d680435c85839bb25f1421bfb9235/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/Unity/local.609d680435c85839bb25f1421bfb9235/Editor/Analytics/values": "0c975c08cfe21e4cfad4bbed0aa9c27c56563da9b3e451680f5682dca07b17bb",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/forest-save.json": "ccb01a0125d4d55174b073cc7a0b45221b4e9e2f17bce6f8f0c4820914c5d303",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/prefs": "bc21c9ae8cac040981b99c6e989546343ae1443ab45d9cb34de79061b034e708",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/d3ed60e6-ace1-471d-bab9-6e75e4f37a84/Insights/AppInfo/unity_insights_app_info.bin": "cca8eac9bbe4767fb1e6717f1203f9fbc75b607015e71cad5c2baa8b0e83b2ba",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/d3ed60e6-ace1-471d-bab9-6e75e4f37a84/Insights/ArchivedEvents/Session/session_end_event": "395972add4cb1fa90b791f7e77e1a4789f1d44545c555f3467e545b47145ce06",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/f543d73e-ad21-4f53-add2-b374260f6d79/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/f543d73e-ad21-4f53-add2-b374260f6d79/Editor/Analytics/values": "0c975c08cfe21e4cfad4bbed0aa9c27c56563da9b3e451680f5682dca07b17bb",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/f543d73e-ad21-4f53-add2-b374260f6d79/Insights/AppInfo/unity_insights_app_info.bin": "cdf9647327be619ad5b3feab860956886d2484c2805f3420b93cc2a7c0a4c9e5",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/f543d73e-ad21-4f53-add2-b374260f6d79/Insights/ArchivedEvents/Session/session_end_event": "b9fc45930e57f857d4dc7e7ec4f06016133e8fbdc746fac88242d55017e05b21",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/local.3af78b9dc9b69813688157182be8a6a2/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/local.3af78b9dc9b69813688157182be8a6a2/Editor/Analytics/values": "0c975c08cfe21e4cfad4bbed0aa9c27c56563da9b3e451680f5682dca07b17bb",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/local.4b87c367d149c7b988a2f406f39255d8/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/Unity/local.4b87c367d149c7b988a2f406f39255d8/Editor/Analytics/values": "c2f322ff07cdcd114b2fc5cfbe04855126ee9785fdeb91ed5494bd44882c5e03",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/prefs": "ec704d4085921e975a254821be98a99c4b4284ac8185e212258d15be6b7aa47b",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/038d03a5-dbd9-4eec-86e5-67ce859e24f5/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/038d03a5-dbd9-4eec-86e5-67ce859e24f5/Editor/Analytics/values": "0c975c08cfe21e4cfad4bbed0aa9c27c56563da9b3e451680f5682dca07b17bb",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/038d03a5-dbd9-4eec-86e5-67ce859e24f5/Insights/AppInfo/unity_insights_app_info.bin": "cdf9647327be619ad5b3feab860956886d2484c2805f3420b93cc2a7c0a4c9e5",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/038d03a5-dbd9-4eec-86e5-67ce859e24f5/Insights/ArchivedEvents/Session/session_end_event": "7ee7139ac858b05a77b86b5ade4491cf745a39347bd3b2b4d4995f827c655b9f",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/local.fa8fb632dc0a99423b14c44c9d8486a3/Editor/Analytics/config": "83b3f90c4edf1f122c8faf9784ca0aee4dd017c65493ac181c1814211703db96",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/Unity/local.fa8fb632dc0a99423b14c44c9d8486a3/Editor/Analytics/values": "0c975c08cfe21e4cfad4bbed0aa9c27c56563da9b3e451680f5682dca07b17bb",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/prefs": "176d7a3fc2179a959633b91ac3cd7268715a418d6082dac49d070491f0541984",
  "/home/jer/.var/app/com.unity.UnityHub/config/unity3d/prefs": "4c6dcd791400d33826c1f2761a34b59778f786bc334d78b816c66d7d16e50154"
}
```

## Disk samples and fixture source identities

| Receipt | Free bytes |
|---|---:|
| disk-before.json | 20446576640 |
| lease2/disk-before.json | 20446969856 |
| lease3/disk-before.json | 20444086272 |
| lease4/disk-before.json | 20444217344 |
| lease5/disk-before.json | 20444610560 |
| lease6/disk-before.json | 72752562176 |
| resume/disk-start.json | 72752300032 |
| resume/lease10/disk-acquire.json | 72751644672 |
| resume/lease10/disk-release.json | 72751513600 |
| resume/lease11/disk-acquire.json | 72751644672 |
| resume/lease11/disk-release.json | 72751382528 |
| resume/lease12/disk-acquire.json | 72751513600 |
| resume/lease12/disk-release.json | 72751513600 |
| resume/lease13/disk-acquire.json | 72751644672 |
| resume/lease13/disk-release.json | 72751251456 |
| resume/lease14/disk-acquire.json | 72751382528 |
| resume/lease14/disk-release.json | 72752037888 |
| resume/lease7/disk-acquire.json | 72752300032 |
| resume/lease7/disk-release.json | 72751644672 |
| resume/lease8/disk-acquire.json | 72751775744 |
| resume/lease8/disk-release.json | 72752037888 |
| resume/lease9/disk-acquire.json | 72752037888 |
| resume/lease9/disk-release.json | 72751513600 |

Boundary inputs were read unedited from these authentic exported files:

| File | SHA256 |
|---|---|
| resume/runtime11/evidence/authentic-boundary-24.json | 57667c245454aff3f35c800cdf5395c485c0eb78d9b2b155ecd494cdb2e28f5b |
| resume/runtime11/evidence/authentic-boundary-99.json | 882e9fe3f4de89ea952e694873753af49d1902125c7c17b8b7c6ceea72824abd |

Offline calculator command exited 0; output: CADENCE CALCULATOR PASS 19 assertions (minimal record stubs; no runtime parity claim).


## Remaining FAIL / NOT RUN and disposition

None in the requested Tier1/Tier2 feature matrix. No candidate defect reproduced on the final valid fixtures. Earlier discarded/failed attempts remain listed above; no output is rounded up.

Source-only invariants and masked guard limits are explicitly separated from runtime assertions above. No Player/Windows build, Tier3 integrated qualification, merge, user acceptance, dashboard ingestion or Drive synchronization is claimed. Fable #402 owns G2 disposition and any integration decision.
