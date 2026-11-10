# Manager #200 — #377 actual Unity results and Annual Review readability verdict

**Date: 10 November 2026. MANAGER VERDICT: NEEDS CHANGES — NOT INTEGRATED / NOT USER ACCEPTED.** #377 has a real **partial feature pass** and a newly measured **Annual Review readability failure**; these must be kept distinct. There is no need to rerun the original entire UI/source audit or rehire Fable (second review user-cancelled).

## Evidence and current source

- [Sol #377 handoff, native observations, raw screenshots and measurements](https://github.com/jermur788/CCF/blob/92ecb095/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-377-INSPECTION-OVERFLOW-REPAIR-AND-FOCUSED-UNITY-RESULT.md).
- Exact tested game source: `c9c7d0100c7b8011b69686cdbfce5f2ecc5f0f86`. GitHub comparison against the preceding failed source confirms **one source commit changing only TreeInspectionView.cs plus an additive inspection_fit.py test**. No economy/gameplay/annual data logic change, no main integration.
- Manager inspected actual `AnnualReviewView.TaggedLine`, `UiKit.Line`, annual USS/layout source. Confirmed that `TaggedLine` nests a provenance tag/qualifier and an elastic name/value row within three equal Annual Review cards; short money values stay in the name/value **horizontal** layout because `UiKit.Line(..., value.Length > 12)` switches to stacked only when the value string is long. The operator's tight-row hypothesis is source-plausible, though exact computed widths are established by Unity measurements rather than source.
- #377 operator rendered/recorded `annual-year3-unread-1280x720.png` and `annual-year3-unread-1920x1080.png` from a **genuine archived Year 2 save followed by real approved felling and year advance**. On the Year 3 Money card, the label `Income less contractor work` wraps into roughly thirteen fragments at 720p and is unreadable at both sizes. 720 logical name rectangle reported as `(801.25,400,33.75,236.25)`. This is **actual failed UI**; not merely a failed source check. GitHub connector returned the Markdown report, not independently decoded PNG pixels, so Manager bases the rendered finding on the worker's committed screenshots/bounds receipts rather than falsely claiming independent image examination.

## Explicit passed, failed and incomplete observations

- **PASS / retained:** Initial Crop Tree overview previously clipped in #376; now **entire inspected Page 1 fits** at 720p and 1080p, with source-backed warning and route to draft detail pages. Native forward `[V]` reached the detailed draft pages (14–16 in this genuine fixture) with competition/Fell/wind explanation. Native rank selection 1–5 on a bounded 720p state produced distinct targets. Approval and year advance were separately executed, then a truthful unread Year 3 Annual Review appeared. Back before ACK preserves unread; separate ACK works in the bounded 720p scenario. A disposable F5/F9 reload preserved the sampled Year 3 state. **Do not expand these scoped successes into full acceptance.**
- **FAIL:** Annual Review Money label readable-width/typography in three-column layout at both 720p and 1080p. **No integration.**
- **PARTIAL:** Work Plan quote/approval and viewport state, initial inspected details at 720p, rank occlusion and native action specifics. Some records were observed but not every hard case.
- **NOT RUN / NOT VERIFIED:** Native `Shift+V` reversal (attempt was inconclusive, not a proven defect); complete Objective 1 locked Planting/Pruning/Nursery pointer restrictions; MFM candidate `[C]/[N]`; historical year browsing/source toggle with expanded details; all full input/focus/old-world/Reference checks. One missing 720p Condition page remains NOT RUN. Do not silently mark these PASS.

## Preservation / trust boundary

#377 reports a safely re-admitted **dedicated ZX20 Unity worktree**, exact source, exclusive machine lease, correct mounted device, disposable verified save+preferences under task-specific config, pre/post hashes identical for 36 user originals, same 27 licensed and six frozen protected manifest items, unchanged personal U3/Workbench, Unity exit 0 and normal lock release. Manager cannot independently inspect local processes/drive here; preserve detailed receipts in the Sol report. User #361 personal playtest remains separate/open/unaccepted. Do not reuse its branch, worktree or saves.

**Execution driver PASS means the Editor/capture procedure completed**, not that the UI met acceptance. Actual feature verdict remains **NEEDS CHANGES**.

## Next action, verification cost budget and scope

- Create **one bounded Sol #378** source-presentational repair of Annual Review MONEY row composition **only**, followed by conditional one-Editor feature continuation after a fresh safe admission. No redo of the inspection defect already proven fixed. Preserve all original screenshots and measurements.
- At first real Year 3 review capture 720p/1080p, require readable full `Income less contractor work` name, provenance/qualifier, and accurate **right-aligned signed amount**, no 33-pixel name column, no overlaps, no invented full net. All three headings remain exactly **Work done / Money / Forest changes**. If layout still fails, early STOP with screenshots, do not run remaining matrix.
- Do not solve by hiding “calculated from records”, dropping provenance, clipping or eliding significant text, altering economic calculation, collapsing three headline sections into unsupported UX, or making a global `UiKit.Line` behavior change without explicit source evidence and Manager decision. A locally scoped annual row stack or similar robust width allocation is preferred.
- If annual first gate passes, only finish **previously not-run required slices** in the same Editor run when possible: historical Annual Review selection and source toggle, MFM C/N candidate with valid fixture if available, locked Work Plan sections/pointer restrictions, Shift+V, remaining 720p details and rank overlays, old-world frozen Reference if safe. Report unavailable fixtures honestly. No new broad forest/release suite, no second Fable cost.
- **G3 historical Unity harness issue remains**; old layout-pinned tests must not be weakened/deleted. New source-exact measurements are additive; earlier checker failures already existed at base. A later harness verification-only update, if actually necessary for release, is a separate decision, not blanket regression waiver.
- [#378](https://github.com/jermur788/CCF/issues/378) **READY / NOT RUNNING**. Prepared dedicated repair branch `fix/scenario-one-ui-annual-review-row-layout-20261110-v1` from exact #377 candidate. Proposed new isolated Files writer worktree and existing separate ZX20 qualified worktree require fresh source ownership/machine preflight, no assumed live lock. [Binding one-paste Sol packet](MANAGER-377-TO-378-ANNUAL-ROW-REPAIR-AND-FOCUSED-CONTINUATION.md).
- Mandatory output: `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-378-ANNUAL-REVIEW-MONEY-ROW-REPAIR-AND-UNITY-RESULT.md`.

**Current final verdict: NEEDS CHANGES.** No source merge, Windows build, player acceptance or external demo approval.

**Dashboard:** GitHub coordination updated by Manager after ruling; downstream dashboard ingestion/readback **NOT VERIFIED**.
