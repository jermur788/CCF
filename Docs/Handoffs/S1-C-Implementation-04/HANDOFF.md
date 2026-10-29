# S1-C T-04 — Provenance labelling: handoff

`S1-C PROVENANCE LABELLING QUALIFIED — READY FOR MANAGER REVIEW`

Worker assessment only; this is not Manager acceptance or integration. Claim #123, parent #91/#106.

## Identity

- Executor: Claude Code / Anthropic Claude Sonnet 5.5 (Medium). Single writer, no delegated workers.
- Branch `feature/s1-c-t04-provenance-labels-v1`, worktree `/media/jer/Videos/CCF-S1C-T04`.
- Base (exact production, post-MFM-06 v23): `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- Production candidate (source gated in Unity): `d524de6ccc3de2366f39bdc939ce7c00657ba93c`. Branch tip `cdd850f7954f5876c09111df9fdcd7af11f26523` adds only the T-03 harness assertion update (one child count) on top; production source is byte-identical to `d524de6`. This handoff is a later docs-only commit.
- Start gate: #91, #106, #123 and live `active-tasks.md` (origin/main `4aeb207`) read; no other owner of `AnnualReviewView`/`ManagementHistoryText`; no Editor running.

## What changed

Production (presentation only):
- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`: `HistoryProvenance` enum (Recorded, Current, Derived, Reconstructed, SessionOnly, NotRecorded), `HistoryProvenanceText` (labels, plain meanings, legend, session-only/storm wording, line classifiers that return null rather than guess) and intervention-level provenance accessors. Existing history wording is unchanged.
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`: one label chip per line (words, not colour alone), a legend at the top, storm card split into recorded vs reconstructed vs current, browsing split into current pressure vs session-only last-year result, honest NOT RECORDED labels for absent failure reasons / execution method / location / tree id / snapshot / report / objective year, current objective state labelled CURRENT even beside an old year.

Not changed: save schema/version/validator, `ScenarioOneManager`, MFM, ecology, economy, scenes, prefabs, packages, `ScenarioEcologyReviewLines`, `TreeInspectionView`. No new persisted fact.

Verification-only additions under `Tools/Verification/S1CManagementHistory/`: `S1CProvenanceOfflineCheck.cs` + `run_t04_offline_check.py`, `AnnualReviewProvenanceVerification.cs` (disposable rendered fixture), `run_t04_gate.py`. One assertion in the T-03 harness changed (`childCount` 13 → 14: the new RECORDED row).

## Provenance matrix

| Class | Where it appears | Source |
|---|---|---|
| RECORDED | Work-done aggregate lines, failed tasks with reason, cancellations, harvest settlement, detail entries, timber sales, small-job minimum, closing cash, owner time, retained timber (this year), retained deadwood (new), fallen deadwood on site (that year's snapshot), objective first reached / year reached (recorded), storm year/severity/Crop Trees lost, outcome, century review, first-intervention prompt | persisted events, orders, annual reports, snapshots, objective achievements, storm event |
| CURRENT | Opened areas, planted-tree status, browsing pressure now, shelters now, retained timber stock, deadwood remaining now (storm card), open orders needing attention, objective met/open state, completion "Objectives met (current state)" | live forest/manager; omitted from earlier-year views except the objectives card, which is headed "(current state)" |
| DERIVED | Canopy/light and regeneration deltas, "Harvesting work" and "other contractor work" and shelter material sums, per-intervention money, during-plan / since comparisons, trend strips, completion comparisons and totals, objective year for pre-feature saves ("derived from saved records") | render-time arithmetic over recorded facts |
| RECONSTRUCTED | Storm trees lost, original stem volume, affected map squares (with an explicit note) | tree-mortality and deadwood records, not a stored aggregate |
| SESSION-ONLY | Planted juveniles browsed last year, or "not recorded this session" after load/new year | in-memory `lastYearBrowsed`; not persisted (not changed) |
| NOT RECORDED | Absent failure reason, execution method, location/target tree, missing snapshot/annual report, objective year with no exact derivation | absence in the record; never rebuilt from current state |

Untagged by design (reviewed allowlist, offline- and render-audited): explanation/guidance text such as the storm model note, salvage guidance, "An opening creates an opportunity", definitions, empty-state prompts, trend-strip headings. No line was reworded to claim causation; the existing "not proof the work caused them" note is kept.

## Results (all on the exact candidate; durable evidence under `/media/jer/Files/CCF/Verification/S1C-T04-Provenance-20261029/`)

| Gate | Result |
|---|---|
| T-04 offline (`run_t04_offline_check.py`) | 49 PASS |
| T-03 offline / S1-C history query / ThreeSpecies history offline (regressions) | 72 / 55 / 74 PASS |
| T-04 rendered Unity fixture (`final-t04`) | 436 PASS, 0 failed, Editor exit 0, 1280×720 and 1920×1080 |
| T-03 drill-down regression (`final-t03b`) | 279 PASS, exit 0 |
| T-02 year-browsing regression (`final-t02`) | 246 PASS, exit 0 |

The rendered fixture covers: generated five-year run (12-order grouped harvest, planting, cancellation, two real failures with one reason blanked, forced real storm in year 5); every class present; one label per row; no text/label overlap or clipping; every other body/muted line on the allowlist; historical (year 1) Forest column has no CURRENT or SESSION-ONLY line; read-only (world hash, save JSON, counts unchanged). Old saves loaded through the production typed route from the S1-C T-05 evidence (`recorded-v22-year3-save.json`, genuine `v20-y5.json`, `render-gap-v20-y5-no-year1-snapshot.json`): RECORDED year, DERIVED year ("from saved records"), and NOT RECORDED with no year shown; session-only browsing says "not recorded this session"; browsing all years leaves the loaded save byte-identical. Fixture SHA-256 values are in `command.json`.

Rendered captures were opened and inspected at both sizes (latest-top, storm, history, objectives, year-4 failed detail, year-3 cancelled, year-1 historical, old-save derived/not-recorded/recorded). Two real layout defects were found and repaired during the work: name/value lines (`Fallen deadwood on site`) wrapped letter-by-letter in the narrow 1280 Forest column beside the label (now stacked for long values), and two harness false positives (nowrap money values; label lookup one level up) were corrected in the harness only. Earlier failed runs are retained as `t04-render`, `t04-render-2`, `t04-render-3`.

## Residuals / not done

- **DBH growth last year** is not shown in Annual Review/history, only in the tree-inspection panel (`TreeInspectionView`, current-state surface, outside the owned files), where it already says "not yet recorded". Not edited. Manager may want a one-line session-only note there.
- `ScenarioEcologyReviewLines` is classified by line prefix in the view (`HistoryProvenanceText.EcologyLine`); a new line type there renders unlabelled until classified (the rendered fixture would flag it).
- The T-05 offline check (`S1CObjectiveHistory/run_offline_check.py`) fails its schema assertion `CurrentVersion is 22` on this base because MFM-06 made it 23. Pre-existing, unrelated (no `ForestSaveData` diff); not touched.
- The T-05 Unity render gate was not re-run; the T-04 fixture loads the same three fixtures with stronger assertions. T-02 text-equality comparison against a control was not made: every review line now carries a label by design, and the T-02 functional gate (246) passes.
- Callback-driven UI; no OS-native pointer qualification. No Windows build. No Tier-3.

## Ownership / cleanup

Lock held only for the gates (owner `Claude-S1C-T04`, task `S1-C-T04`), released; U1 restored to its prior detached `c8cfae8`, clean. Disposable harness, `.meta` and copied licence removed each run; no disk saves created; Editor import side effects restored by the runner (patches retained). ZX20 free ≈ 21 GiB.

---

# Second round — Manager review `NEEDS CHANGES` corrections (2026-10-29)

Manager review: `MANAGER-REVIEW.md` at main `38d638ba93172e2c072c79acc112329e73f9171a` (reviewed production `d524de6`).
Same branch/worktree/ownership. **Corrected production candidate: `ba2922e57467326beb4571e71d5373d73eaaa049`** (production source: `ManagementHistoryText.cs`, `AnnualReviewView.cs`). Later commits are verification/docs only. No integration, no Windows build. The success token above is returned for a second Manager review; it supersedes nothing the first section states except where listed here.

## Finding 1 — presentation aggregates are DERIVED
Rule applied: a displayed value is RECORDED only if that exact value is a persisted field or record; a count/sum/grouping made at display time is DERIVED. Changes:
- `ManagementHistoryText.WorkProvenance` → Derived (intervention Work lines).
- Work column: salvaged/thinned/planted/pruned counts, "No work was resolved this year", "N task(s) failed" / "No tasks failed", cancelled-order count → DERIVED. Regeneration-removal count (annual-report scalar) and each failed event's own reason text stay RECORDED.
- Harvest settlement split: "N harvest order(s) settled together as one job (year label)" DERIVED; "Timber income · harvesting cost" (the one persisted settlement) RECORDED.
- Completion: "You carried out N interventions", per-intervention work lines and "No commissioned work…" → DERIVED; `outcomeYear` and per-species snapshot counts stay RECORDED. First-intervention prompt → DERIVED. Management-history card now has no RECORDED line (work, money and snapshot comparisons are all derived).
- Legend/meaning: "DERIVED counted or calculated from records when shown".
Underlying drill-down events/orders remain RECORDED.

## Finding 2 — storm card
- The mixed line is split: **DERIVED** "X m³ salvaged since this event, to date" (summed from completed salvage orders) and **CURRENT** "X m³ remains as deadwood now".
- The CURRENT line is emitted only on the latest review (`BuildStorm(..., historical)`); earlier storm years show RECORDED year/severity/Crop Trees lost, RECONSTRUCTED counts/volume/squares (with the note) and the DERIVED salvage line, with no live-forest line.
- The rendered fixture now forces a storm in year 3 (earlier) and year 5 (latest) and browses the earlier one: card classes asserted exactly (earlier: current 0, recorded 1, reconstructed ≥2, derived 1; latest: plus current 1), text asserted to omit "remains as deadwood now" for year 3, capture `year3-earlier-storm`.

## Finding 3 — drill-down entries
`BuildDetailItem` no longer uses one entry chip. Each entry is: a RECORDED title row (status chip FAILED/CANCELLED stays), a RECORDED facts row (species, location, execution method, year, shelter, result, volume, own money — only values the record holds), a RECORDED "Reason: …" row when a reason is held, and — only if something is absent — one NOT RECORDED row naming exactly the absent values ("execution method not recorded", "location not recorded", "tree not recorded", "Reason: not recorded"). Missing text never appears on a RECORDED row. The intro line is an untagged explanatory note. Fixture asserts per entry (12 harvest entries all fully recorded, no NOT RECORDED; failed year: two entries, one NOT RECORDED row exactly "Reason: not recorded"; cancelled: "execution method not recorded"), and the audit now covers detail-entry rows (previously excluded).

## Rerun results (exact candidate; evidence `/media/jer/Files/CCF/Verification/S1C-T04-Provenance-20261029/r2-*` and `r2b-*`)

| Gate | Result |
|---|---|
| T-04 offline provenance (`run_t04_offline_check.py`) | 55 PASS |
| S1-C history offline (`run_offline_check.py`) / T-03 offline / ThreeSpecies history | 55 / 72 / 74 PASS |
| T-04 rendered fixture (`r2b-t04`), 1280×720 and 1920×1080 incl. earlier storm year and old saves | **527 PASS**, 0 failed, Editor exit 0 |
| T-03 drill-down rendered regression (`r2b-t03`, row structure changed) | 279 PASS, exit 0 |

Notes: `r2-t04` (first rerun) failed only because the new overlap check compared the label to the neighbouring FAILED/CANCELLED chip; fixed in the harness only (chips excluded) and re-run. T-02 year-browsing was not re-run (no navigation/gate change; it passed 246 on the first-round candidate and Manager did not require it). Captures inspected at both sizes: `year3-earlier-storm`, `latest-storm`, `year4-detail`, `year3-cancelled`, `latest-top`, `year1-historical`. Lock was held only for these runs (released); U1 restored clean to `c8cfae8`; no disk saves; harness/meta/licence copies removed; no save/schema/validator/manager/MFM/simulation diff.

Accepted residuals unchanged: DBH-growth labelling in `TreeInspectionView` is outside this task; stale T-05 `CurrentVersion is 22` assertion; T-05 Unity render gate not rerun.
