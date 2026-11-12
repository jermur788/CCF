# Plan — CCF Management Cadence and Dual Advance V1

**Status:** APPROVED DESIGN (D-077), implementation NOT STARTED. See [spec](spec.md), [tasks](tasks.md), [checklist](checklists/requirements.md). Independent [#396 NEEDS CHANGES](https://github.com/jermur788/CCF/issues/396) findings B-1…B-5 and M-1…M-5 repaired in this four-file contract.

## Stage 0 — mandatory actual source and lock preflight
Manager or assigned Sol source owner reads live `Docs/Project/coordination/active-tasks.md` and verifies latest isolated C-01 integrated-candidate lineage, clean worktree, active owners, disk >= 15GiB floor and user #382 protection. Current code and confirmation/eligibility UI may have moved after reviewer inspected earlier source; inspect exact source again. No Unity lease assumed for initial source work. Source worker registers claim [RUNNING] with actual model/provider/effort, branch/worktree/starting HEAD only when ready. Do not reset or clean unrelated dirty work.

## Stage 1 — bounded, source-controlled implementation (one writer)
Build a **pure read-only deterministic calculator** for suggested target year and eligibility (current teaching `Current==7`, completed felling year, 4-year guidance, 5 ticks, Year25/100), with plain-world tests. UI controls: retain current `Advance one year` unchanged, add optional skip from open Work Plan, visible year/reason/confirmation and 1-per-frame loop over existing `manager.AdvanceYear()`, with pre/post guards, between-tick cancellation, input lock, no synthetic review events.

At destination, pass a **session-only** elapsed-year summary to existing Annual Review selected-year browser (or one equivalent presentation component). Do not introduce new saved data, duplicate history, auxiliary Update simulation or change manager/economy/learning completion logic. If UI root source needs a tiny helper to open latest Review and set session summary, keep it UI-only. Do not falsely attribute changes to interventions.

## Stage 2 — focused offline/source qualification before Unity
Build tests from authentic snapshot fixtures. Exact parity A versus direct manager calls without ack; semantic parity B against manual UI with real intermediate ack events, after careful acknowledgement normalization; completion/achievement/storm/Year25 tests; each negative eligibility case; no automatic MFM proposal/Work Plan work; data/old-save nonmutation. Mutation/integrity test retained for new parity checks if they would otherwise green under a broken guard. A source pass is not Unity PASS.

## Stage 3 — one exact feature qualification under lock
After source handoff, assign a **separate bounded Unity qualification** on the exact candidate under exclusive machine-wide lease (no overlapping Editor). Reuse warm eligible lane only after owner/space/registry check. Test genuine 720p/1080p time-skip UI, proper year browser, save/reload, frozen Reference Future/Legacy40, 5-tick performance and no unauthorized first-teaching bypass. Preserve personally used #382 scene/Prefs/worktree, no Windows build.

## Stage 4 — independent review if necessary and integration verdict
Source/Unity evidence inspected by Manager; independent reviewer if implementation crossed complex chronology/source contracts or confidence is insufficient. Verdict APPROVED / NEEDS CHANGES / REJECTED. Integrate only into exact current UI successor lineage, not divergent main; then sync canonical docs/dashboard with status. Whole Tier-3 only once for consolidated milestone.

## Separate D-074 objective goal (not automatically complete)
Create a distinct four-file DRAFT for **spaced major-intervention learning**, based on verified S1-D completion and old-save compatibility. It remains required before declaring the educational product goal done. The V1 time-advance source may ship without changing the existing completion predicate; do not present guide-only cadence as proof of a newly enforced learning objective. Coordinate after time-advance data contract is stable.

## D-076 interface
A later management review may lead to new representative rack sampling in a separate approved MFM extension. The skip never executes an old sample-derived prescription, chooses new racks/Crop Trees or fabricates a per-rack ecological history. If Work Plan/Annual Review and future MFM changes overlap, serialize writers and integration rather than co-editing.

**Routing:** one local Sol 6.1 Medium bounded source worker, source issue READY not RUNNING until actual preflight, with published GitHub exact packet; no automated asynchronous user work or promise of completion. Independent design review #396 is closed, no repeated general reviewer round requested for wording-only repair.
