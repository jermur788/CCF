# S1-D T-04 — Interim Manager Review

**Task:** S1-D T-04 — Guide monotonicity  
**Claim:** #124  
**Verdict:** **NEEDS CHANGES BEFORE UNITY QUALIFICATION**  
**Review basis:** approved S1-D specification/plan/task packet plus the worker's reported local implementation state. The reported candidate `18e33d4` is local/unpushed and was **not inspected from GitHub**, so this document is a spec-conformance/design verdict, not an implementation-code approval.

## Reported candidate state

- Branch: `feature/s1-d-t04-guide-monotonicity-v1`
- Worktree: `/media/jer/Videos/CCF-S1D-T04`
- Exact base: `b971f4da0540e606bfee9702b0c99f81a0a89355`
- Reported local candidate: `18e33d4`
- Reported offline checks: new T-04 check 27/27 PASS; T-03 teaching-cap 48/48 PASS apart from its expected protected-diff guard against the owned teaching file; T-02 cycle-state 93/93 PASS.
- Unity qualification: not run.
- Independent review: not run.
- Success token: correctly withheld.

## Manager decisions

### 1. Lesson 5 → 6 witness through later planting

**NOT ACCEPTED.**

A successful `PlantJuvenile` event does not faithfully prove that the session-only `[J]/[K]` release-review lesson was completed. The approved T-04 contract permits persisted facts such as successful planting only where they faithfully prove the relevant historical boundary. Ordinary management actions are not lesson-gated, so later planting cannot be used as a surrogate persistence record for `[J]/[K]` completion.

Where no persisted fact faithfully proves that boundary, preserve the existing current-state behaviour and report the uncovered state. Do not infer or manufacture a latch.

### 2. Release proof through any in-footprint sold/deadwood felling

**NOT ACCEPTED as historical lesson-completion proof.**

The inspected S1-D baseline distinguishes release removals serving a Crop Tree from arbitrary in-footprint felling. If that relationship cannot be reconstructed faithfully from persisted history, the task's fallback/stop rule applies. Broadening the proof to every in-footprint sale/deadwood removal can over-claim historical completion and is therefore not an authorised latch.

### 3. T-03 / T-04 shared historical-completion interpretation

**REQUIRES CORRECTION BEFORE REVIEW.**

The task packet requires the guide change and the T-03 teaching-cap decision to share one faithful historical-completion interpretation rather than ship competing definitions. The worker reports that T-03 still reads the current rack-bay state and only partly shares the new helper. Reconcile this within T-04's permitted teaching/current-state surface if possible. If doing so would require crossing a protected ownership/API boundary, stop and return a precise blocker rather than duplicating semantics.

## Required revision

1. Remove planting as a surrogate proof of `[J]/[K]` completion.
2. Remove arbitrary in-footprint felling as proof of release-lesson completion unless a persisted relationship faithfully proves the required release semantics.
3. Keep existing behaviour for any lesson boundary that lacks faithful persisted proof.
4. Reconcile T-03 and T-04 to one faithful historical-completion interpretation, or return a precise cross-lane blocker.
5. Rerun the focused offline matrix/regressions on the revised candidate.
6. Push/publish an inspectable exact candidate and `Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md` before requesting Manager code approval.

Do **not** spend the serialized Unity lane on the current `18e33d4` design. After the corrected exact candidate is inspectable and the offline gates are green, run the packet's Unity save/reload/world-drift/teaching regressions and then obtain the required independent review.

## Integration / downstream effect

- #124 remains the sole curriculum writer while revising.
- MFM-09 remains blocked on accepted S1-D T-04/T-05.
- This review does not alter `ScenarioOneCompletionRules`, save schema, Rack V2, MFM logic, or any product decision.
- No Windows build is authorised.
