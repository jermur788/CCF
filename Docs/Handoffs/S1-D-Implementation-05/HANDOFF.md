# S1-D T-05 / #151 — OFFLINE CANDIDATE: previous versus proposed presentation

Provider/model: OpenCode harness in T3 Code, provider instance `opencode-go`, model `deepseek-v4.1-flash`, resuming the partial OpenAI B lane (`gpt-6.1-sol`, high reasoning) after its provider limit cut-off.

- Branch: `feature/s1-d-t05-previous-proposed-v1`
- Worktree: `/media/jer/Videos/CCF-S1D-T05`
- Exact start: `cfdc93bf3943125d21570bc5749a9338f52d1cce`
- Blocker documents: `c3a254c66ce5dce2639f33055e3040b8e8824146`, `c7dba066847e4b52ba7512ab4bb26ee6aeed6ce8`
- Partial production candidate resumed: `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1`
- Verification commit: `38904535ceb45da1b9112a7695c7c7295462efec`
- This documentation commit SHA is reported on #151 (a document cannot contain its own hash).

## Disposition

**Offline/static candidate complete; Unity gate not run and not claimed.** The Manager disposition on #151 authorised the tiny read-only exact-year API overlap. Production was resumed on the existing branch/worktree/start, inspected, and left semantically unchanged where already correct. No merge, integration, Unity Editor run or Windows build was performed. No serialized asset, save/schema/validator/hash/reference, economy, ecology, completion-rule or MFM teaching surface changed.

## Changed files

Production, in `98ac98b5`:

- `Assets/ForestPrototype/UI/WorkPlanView.cs` — current-open-order grouping, provenance display, creation-year links, status vocabulary, redraw signature.
- `Assets/ForestPrototype/UI/AnnualReviewView.cs` — `TryShowYear(int)` read-only exact-year selection.
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` — `TryShowReviewYear(int)` route that opens Review without `ShowLatest()` overwriting the year.

Verification, in `38904535` (new, outside `Assets/`):

- `Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/HeadlessUi.cs`
- `Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/PreviousProposedOfflineCheck.cs`
- `Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/run_offline_check.py`

This handoff is the only other change. `git diff cfdc93bf -- Assets Tools ProjectSettings Packages` contains only the three authorised UI files plus the new `Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/` directory; no `.meta`, save, schema, validator, hash, Reference Future v1, package or project-settings change.

## Binding classification implemented

| Open ordinary record (authoritative fields only) | Class / handling |
|---|---|
| `status == ScenarioWorkStatus.Approved` | **Approved**. Status is the Work Plan commitment; this is not MFM prescription approval. If `createdYear` is a completed `HistoryYears` year, a clearly labelled creation/origin review link is offered. |
| `Pending && createdYear == CurrentEcologicalYear` | **Pending new**. Uncommitted ordinary work; no fabricated completed-year link. |
| `Pending && createdYear < CurrentEcologicalYear` | **Pending carried over**, independent of `StaleAfterYears` / `StaleOrders`. Links only to an existing `HistoryYears` year equal to `createdYear`. |
| `createdYear > CurrentEcologicalYear` | **Inconsistent record surfaced**, not silently labelled new or carried over. A Pending future-year record is excluded from all three groups; an Approved future-year record keeps its authoritative Approved status plus the inconsistency warning. No fourth class and no invented history year. |
| `Completed` / `Failed` | Not a current proposal; retained by existing S1-C history. |

MFM-generated ordinary work follows the same table. Prescription approval creates `Pending` ordinary work with the four existing provenance values. The production writer contains no approval-status write, no cash spend and no resolution; `ApprovePendingWork` remains the separate commitment gate. T-05 did not alter prescription, approval, cancellation, completion or economy behaviour.

## History link / exact-year API evidence

- `AnnualReviewView.TryShowYear(int)` validates the requested year against the existing `ui.Manager.HistoryYears`; an absent year returns `false` before touching `viewYear`, `openDetails`, rendering or acknowledgement. A present year sets `viewYear`, synchronises `lastLatestYear` so the first/new-latest `Refresh(true)` cannot replace the deliberate selection, and clears only the presentation detail set. It never acknowledges or writes history.
- `ScenarioOneUiRoot.TryShowReviewYear(int)` validates first through the view API, then opens the existing Review surface (`screen = UiScreen.Review`, Work Plan closed, help closed, auxiliary panel open) without calling `ShowLatest()`. An invalid request leaves the whole route untouched.
- Work Plan links use `m.HistoryYears` directly and only when `createdYear < CurrentEcologicalYear` and a matching year exists. Labels say **Creation-year review · Year N** and **created in year N**, never approval year. Pending new gets no link; a missing earlier year says so and invents nothing.
- No duplicate history UI, query or store was added; no reflection, synthetic click, new persisted field or new date semantics.

Focused harness evidence (`S1D_T05_PREVIOUS_PROPOSED_PASS checks=219 failures=0`):

- all five `ScenarioWorkType` values × nine status/year cases × manual/prescription provenance (180 matrix assertions), including classification leaving the order byte-identical;
- one-year carry-over classified without waiting for the two-year stale threshold;
- production prescription creation loop plus real `CreateFellingOrder` produce ordinary `Pending` work with all four provenance values; the harness then applies only the ordinary approval status transition to show regrouping, not a prescription-side shortcut;
- exactly three explicit groups with faithful counts; completed/failed excluded; future-year records surfaced and never labelled new/carried;
- only existing prior years get buttons; carried order opens Year 4 exactly; Approved link says creation year; missing year has no link; Pending new never gets a completed-year destination;
- navigation preserves a requested year through first/new-latest refresh, rejects absent/negative/future years without fallback, hides but never acknowledges the historical footer, and leaves `WorkOrders`, `ManagementEvents`, `AnnualReports`, `HistoryYears` and `AnnualReviewSeen` unchanged;
- static audits: no duplicate history construction/store or acknowledgement writeback, no stale threshold in classification, no invented date/causal wording, ordinary latest-entry route retained.

Seven deliberate mutations were caught by the same harness: stale-threshold carry-over, prescription-as-approved, silent latest fallback, first-refresh year loss, route reset to latest, missing-year origin link, and future-Pending-as-new.

## Exact commands and results

All commands ran from the worktree. Outputs are under `/media/jer/Files/CCF/Verification/S1-D-T05-20261030/`. Focused harness (with mutation gate):

```bash
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/ScenarioOneRepeatedInterventions/PreviousProposed/run_offline_check.py --mutations --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/previous-proposed
```

Required regressions:

```bash
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t02
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_t03_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t03
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_t04_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t04
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/S1CManagementHistory/run_t06_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1c-t06
PYTHONDONTWRITEBYTECODE=1 python3 Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out /media/jer/Files/CCF/Verification/S1-D-T05-20261030/s1d-t02
```

| Check | Result |
|---|---|
| Focused T-05 matrix + mutations | `S1D_T05_PREVIOUS_PROPOSED_PASS checks=219 failures=0`; `S1D_T05_MUTATIONS_PASS caught=7 survived=0`; exit 0 |
| S1-C T-02 history query | `S1C_HISTORY_QUERY_PASS checks=55`; exit 0 |
| S1-C T-03 drill-down | `S1C_T03_DRILLDOWN_PASS checks=72`; exit 0 |
| S1-C T-04 provenance | `S1C_T04_PROVENANCE_PASS checks=56`; exit 0 |
| S1-C T-06 guidance | `S1C_T06_GUIDANCE_PASS checks=53 failures=0`; exit 0 |
| S1-D T-02 cycle state | `CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED`; exit 0 |

Additional relevant offline checks:

| Check | Result |
|---|---|
| MFM-07 approval boundary | `MFM07_APPROVAL_BOUNDARY_PASS checks=125 failed=0`; repeat byte-identical; exit 0 |
| MFM-06 save provenance | `MFM06_SAVE_OFFLINE_PASS checks=172`; `REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_ac876a6`; exit 0 |
| S1-D event-id integrity | `EVENT_ID_OFFLINE_PASS checks=75 failures=0`; save still v23; exit 0 |
| MFM-04 prescription core | `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; repeat byte-identical; exit 0 |
| Scenario One non-UI approach | `NONUI_APPROACH_PASS checks=49`; exit 0 |
| Completion N3 | `N3_OFFLINE_PASS checks=88 NOT_UNITY_VERIFIED`; exit 0 |

The MFM-07 offline gate statically proves the prescription writer never references `ApprovePendingWork`, `ScenarioWorkStatus.Approved`, cash or resolution (A11/F5), and that `ApprovePendingWork` cannot reach prescription or proposal state (F1). Its end-to-end Work Plan approval witness lives in the unrun MFM-07 Unity gate. The focused T-05 harness supplies the grouping/provenance witness for the same Pending records.

Not used as gates:

- `Tools/Verification/S1CObjectiveHistory/run_offline_check.py` is a pre-T-05 stale harness: it hard-asserts save `CurrentVersion == 22` while the integrated MFM-06 lineage is v23, so it fails before exercising this change. It is unrelated to T-05 and was not treated as a T-05 regression.
- Scenario One Completion P2/P3 offline scripts require a Unity-generated `Assembly-CSharp.csproj` whose `Library/ScriptAssemblies` references exist; all available lanes have those caches removed under the storage policy. They were attempted but could not build offline without generating Unity state. No Unity run was performed.

## Unity readiness

Unity import/compile and the required rendered qualification remain for the separately scheduled #91 Serialized Unity gate: three-cycle fixture with all classes visibly distinct, MFM-generated pending work with provenance proven Pending before Work Plan approval, Work Plan → historical Annual Review navigation, missing-year case with no fabricated destination, 1280×720 and 1920×1080 captures, and proof that S1-C remains the sole history surface and Work Plan approval the sole commitment gate.

The offline harness executes unchanged production method slices for grouping, status, exact-year selection and the root route, but it does not prove Unity layout, input, rendering, import or serialized anchors. No Unity, merge or integration claim is made.

## Unity qualification (claim #151, 2026-10-30)

Production under test: exact pushed head `4ddf4aa535d4ec5267be46d2635679411d595e0f` in U1, no production change. Verification-only harness `Tools/Verification/S1DPreviousProposedUnity/S1DPreviousProposedUnityGate.cs` plus one `s1d-t05` gate line in `run_unity_gate.py`, on branch `verify/s1d-t05-unity-v1` (tip `d9a13411`, local, not pushed). Evidence: `/media/jer/Files/CCF/Verification/S1-D-T05-Unity-20261030/` (`out/render-run4` is the qualifying receipt; shots and texts under `evidence/`).

**Qualifying run (`render-run4`):** compile pass; rendered gate exit 0, `S1D_T05_UNITY_PASS`, 579 checks / 0 failures / 12 screenshots at 1280×720 and 1920×1080; harness, `.meta` and licence copy removed; U1 clean (0 changes); host saves unchanged; no disk saves created; Unity lock released normally; U1 restored to `b9bf11aa`; no Editor.

Covered at both sizes: three-cycle fixture; Approved / Pending new / Pending carried over grouping (1 / 2 / 87); future-year order surfaced as inconsistent, in no group, not labelled new or carried; MFM prescription approval creates ordinary Pending orders, carried over, not Work Plan approval; exact creation-year Work Plan → Annual Review links for existing history years only; missing, future and negative years refused with the Work Plan left open (no latest-year fallback); historical review read-only (no acknowledge, state and hashes unchanged), titled as an earlier year; AC-5: executed work shown in Annual Review "Work done", an unplanned Fell mark has no order until the Work Plan adopts it as Pending new; collapsed/expanded/scrolled group readability, text inside panel, screenshots non-blank.

**Earlier runs, kept as failed receipts (none was a production finding):**
- `render-run1`: harness compile error (assumed a `UiScreen.WorkPlan` value; the Work Plan is open when `manager.WorkPlanOpen`). Nothing executed.
- `render-run2`: all functional checks passed; six 1920×1080 `TEXT_NOT_CLIPPED` failures. Screenshots show no clipping.
- `render-run3`: diagnostics added. Flagged single-line labels measured exactly 2× their content height (36.7 vs 18.3), i.e. a sub-pixel wrap when measuring at the exact element width. The run also produced all-white screenshots (environment) and Unity hung on exit until the runner's 1500 s timeout; cleanup was clean.
- Harness fix before run 4: measure at content width + 2 px. Pass criteria otherwise unchanged. The 2 px tolerance still flags genuinely narrow boxes; the run-2 screenshots were reviewed visually.

**Limits:** UI callbacks drive the real UI Toolkit controls; OS-native pointer targeting is not qualified. The Work Plan grouping card sits above the existing detailed sections and screenshots confirm both remain readable. The Manager may still confirm the future-year presentation choice (Approved-with-future-year stays under Approved and is flagged). No Manager acceptance, integration, dashboard closure or T-08 unblock is claimed. No Windows build.

## Residuals

- Rendered/layout review was not possible offline; the grouping card is a current-order index above the existing detailed sections, not a replacement for their quotes or controls. Rendered review should confirm the foldouts, link labels and long order lists remain readable at both resolutions.
- An Approved order with a future `createdYear` is still shown under Approved because status is authoritative, and is separately flagged as inconsistent; a Pending future-year order is excluded from all three classes. Manager may confirm this presentation choice at Unity review.
- No Manager acceptance, integration, dashboard closure or T-08 unblock is claimed. T-08 remains dependent on accepted T-02..T-06.

`S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`
