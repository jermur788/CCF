# D-073 #416 — Annual Review year-comparison wiring (T-04)

Child of [#412](https://github.com/jermur788/CCF/issues/412); implementation claim [#416](https://github.com/jermur788/CCF/issues/416). Task T-04 only: FR-1, FR-2, FR-3, FR-10 and FR-14.

**IMPLEMENTED ON TASK BRANCH; offline/static gates PASS. NOT UNITY-VERIFIED.** Ready for T-05 qualification by a different worker, conditional on its fresh lane/lease preflight. No integration, independent review, feature acceptance or Manager verdict is claimed.

## Worker, authority and exact state

- Actual worker: Codex **gpt-6.1-sol**, **high** reasoning, Codex harness in T3 Code, on the default **Codex** account according to the dispatch. Work performed directly; no subagents.
- Routing history: the first gpt-6.1-sol attempt on **ChatGPT - OpenAI B** stopped on a usage limit before changing anything, as recorded by the orchestrator in the [#416 worker-change comment](https://github.com/jermur788/CCF/issues/416#issuecomment-6826061365). This attempt independently confirmed the expected clean start. Account selection and the prior usage-limit event come from the packet/issue record; they were not independently introspected through account telemetry.
- Sole writable worktree: `/media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1`.
- Branch: `feature/scenario-one-d073-annual-review-v1-20261112`.
- Start HEAD: `6baee47bd677f48e763b7e8c10092eaf45ba4c79`, clean; branch name matched. Remote branch also matched this start before editing.
- Tested implementation commit: **`d58c8b44eadbeb2e8b4d5734387e17f25fec2fb0`**.
- Tested tree: **`1fa88a3d7059751858302bb3c8ed27237580bc1a`**. All gates below ran on this committed, clean tree, before adding this handoff.
- Implementation commit message: `feat(annual-review): wire D-073 year-comparison card into Annual Review (#416 T-04)`.
- Final HEAD: the handoff-only child of the tested commit. Its exact SHA is in the #416 result comment and external `publication.json`; this document cannot contain its own commit SHA.
- Canonical project files were read from the start snapshot. Their local last-touch context commit is `63044dfbbeb869996ceecec8a47619dcba048b0b`; the packet/approved spec supplies the newer bounded D-073 task authority.
- Approved spec, plan, tasks and requirements checklist were fetched read-only using `git fetch origin docs/manager-d073-approved-scope-20261112-v1` and read with `git show FETCH_HEAD:specs/scenario-one-multiyear-decision-outcome-feedback-v2/<path>`. Fetched HEAD exactly **`53dec8db19a9bdde20176b2f3abf508402c7f0af`**. These files were not copied onto this branch.
- Read the #415 mapper handoff, its public API/rendering notes and all 13 recorded design decisions. Mapper/helper code, view and existing UiKit/USS patterns were inspected before editing.
- Live GitHub `main` coordination was fetched read-only at task start; #416 was independently read as RUNNING, with the worker-change comment. The latest direct packet supplies exclusive child ownership. No local coordination or physical board was changed or queried for mutation.
- External evidence: `/media/jer/Files/CCF/Verification/D073-AnnualReviewWiring-416-20261112/` (all temporary compiler/fixture output under its `tmp/` via `TMPDIR`).

## Exact diff and boundaries

Implementation, `git diff --name-status 6baee47b d58c8b44`:

```text
M	Assets/ForestPrototype/UI/AnnualReviewView.cs
```

`git diff --stat 6baee47b d58c8b44`: **1 file changed, 50 insertions(+), 0 deletions**. View blob: `8e571bb2` (abbreviated). The later handoff-only commit adds exactly:

```text
A	Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/D073-416-ANNUAL-REVIEW-WIRING-RESULT.md
```

Static comparison removes only the new private method and its one call and obtains the complete baseline view byte for byte. This proves all pre-existing view code, including title, year buttons, footer, acknowledgement, skip summary, navigation and other builders, is unchanged.

No serialized Unity file, `.meta`, scene, prefab, stylesheet, package, ProjectSettings, manager, helper, mapper, objective, event, hash, save schema or save file changed. No new asset or dependency. No Unity process, Player build or Windows build was started. Primary, main, other worktrees, the Ops board and `active-tasks.md` were not edited. Initial unrelated changes: none.

## Wiring and design readings

1. **One source and bounded cost.** The existing change-key guard is at `AnnualReviewView.cs:115–116`. The sole invocation `BuildYearComparison(m, year)` is at line **175**, after that early return and after the no-completed-year return. The sole `ScenarioOneManagementFollowonReadout.Build(` call is at line **187**, inside that private builder, passed directly to `ScenarioOneYearComparisonText.Describe` with `manager.HistoryYears`, `year.Year` and `ScenarioOneUiFacts.SpeciesName`. No constructor, callback, unguarded refresh or other caller invokes it. Thus each content rebuild with a selected year builds one readout; unchanged per-frame refreshes and the existing empty state build none. No live-forest values, intervention history, delta or previous snapshot supply this card.
2. **Placement (FR-1).** The card is added to the existing scroll content (line 193), immediately after the existing prior-treatment guidance builder and before the storm/history/trend/objective cards. Existing detailed annual records remain in their existing position. The spec requires scroll placement but does not prescribe ordering among those scroll cards. Nothing is added to the header/footer.
3. **Hidden states.** `comparison.Visible == false` returns at line 189 before creating or adding any UI. Latest-year and gap-state contents are the mapper's exact rows. The existing no-year message remains unchanged.
4. **Plain text rows rather than a new table layout.** All mapper `Row.Text` strings are shown in order, including its dated table-header line and combined `A → B (difference)` rows. This follows #415's explicitly supported text rendering and the review's existing standing-tree convention. No arithmetic, units, state precedence or wording is reimplemented in the view. Compared rows use the mapper's row-level DERIVED provenance; selected-only observations use RECORDED. A separate three-column UI is unnecessary and would add layout work beyond this wiring task.
5. **Provenance (FR-10).** Every row with provenance goes through `AddTagged(parent, row.Provenance.Value, row.Text, textClass)` at line 227; null provenance goes through `UiKit.Add`. Existing `Tag`/chip handling and `.hide-data-sources .prov` control disclosure. No bracket labels are inserted into plain text. `Row.Muted` selects the existing `muted` class, headings select `heading`, other rows select `body`.
6. **One species foldout.** The mapper's `SpeciesHeading` becomes a `Foldout` titled with its exact text. All later `SpeciesMeasure` and `SpeciesNotRecorded` rows use that foldout as parent. Its independent key is `year.Year + "|year-comparison-species"`, held only in the existing `openDetails` set. It starts closed because the key is absent; year browsing preserves each year's open state in the same manner as existing detail keys. Existing `ShowLatest`/`TryShowYear`/new-year clearing behaviour is unchanged. No saved state is needed.
7. **Indentation.** Species continuation rows (`FirstRowOfSpecies == false`) use an unclassed wrapper with `style.marginLeft = 10`, exactly the existing `AddDetail` indentation style (baseline line 439, now line 489). This reuses an existing inline style; no new shared class or USS rule. The indentation contains the tagged row, so its chip and text align together. First/missing species rows are not indented further.
8. **Guidance pointer decision (#415 decision 4).** The guidance builder runs immediately before this card. Because `scroll` was cleared for the rebuild, querying `scroll.Q<VisualElement>("prior-treatment-guidance")` at line 197 proves that the view actually built the card for this year. The mapper's `GuidancePointer` row is omitted when that element is absent. This avoids recalculating guidance eligibility or inferring it from comparison data; the query supplies only presentation presence, never stand/work values.
9. **Read-only behaviour.** The new method changes only freshly created UI elements and, when the player toggles the foldout, the existing view-local `openDetails`. It contains no manager/history/order/event/snapshot assignment, save call or acknowledgement. Stable `annual-year-comparison-*` names are element identifiers for qualification, not CSS classes.
10. **No change to mapper decisions.** All 13 #415 decisions remain as implemented: its exact supplementary wording, both-missing handling, latest-year precedence, compared-only caveat/pointer, DERIVED zero-event count, unlabelled structural rows, invariant number formatting, zero counts, cancellation warning policy, hidden invalid input, capitalised species names, no species list when latest snapshot is missing, and “felled” wording. T-04 only conditionally displays the existing guidance pointer per decision 4. No new ecology, causal claim, recommendation or money display is introduced.

The current mapper guarantees that `SpeciesHeading` precedes species rows; the view follows that ordered public contract. It does not silently fall back to rendering a malformed future species sequence outside the foldout. Actual scene-based species naming and all rendered results remain for T-05.

## Styles reused (no new classes)

All definitions are in `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`:

| Use | Existing definition line |
|---|---:|
| Direct `heading` | 69 |
| Direct `body` | 70 |
| Direct `muted` | 71 |
| Direct `card` | 129 |
| Existing helper `annual-text-entry` | 656 |
| Existing helper `annual-text-source` | 663 |
| Existing helper `annual-text-body` | 669 |
| Existing helper `faint` (optional source qualifier) | 72 |
| Existing helper `chip` | 83 |
| Existing helper `chip-crop` (DERIVED) | 91 |
| Existing helper `chip-ok` (RECORDED) | 93 |
| Existing helper `chip-bad` (NOT RECORDED) | 95 |
| Existing `prov` disclosure under `hide-data-sources` | 595 |

No `AddToClassList` or new class literal was added. `Tag` continues to attach its pre-existing `prov-recorded`, `prov-derived` and `prov-notrecorded` metadata identifiers (not new stylesheet rules); only the shared `prov` selector controls their disclosure. The native Foldout uses Unity's existing control styling. `marginLeft = 10` is reused from the existing view, as above.

## Gates on the committed clean implementation

Commands ran from the assigned worktree with `TMPDIR=/media/jer/Files/CCF/Verification/D073-AnnualReviewWiring-416-20261112/tmp`. `<worktree>` and `<evidence>` below expand to the exact absolute roots above.

`commands/<label>.json` records argv, cwd and exit; companion `.stdout.log`/`.stderr.log` preserve output. `run-gates.py` is the external command recorder. Existing tools/tests were not edited.

| Gate | Exact command | Exit / result | Evidence label |
|---|---|---|---|
| a scope | `git diff --name-status 6baee47b HEAD` | 0; exactly the single `M` line above | `a-scope` |
| a clean | `git status --porcelain` | 0; empty before and after gates | `a-status`, `after-gates-status` |
| hygiene | `git diff --check 6baee47b HEAD` | 0; no whitespace errors | `diff-check` |
| identity | `git rev-parse HEAD HEAD^{tree}` | 0; exact tested commit/tree above | `tested-state` |
| b type binding | `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <worktree> --out <evidence>/typecheck` | wrapper 0; **`sources=192 refs=116 exit=0 errors=0`** | `b-typecheck` |
| c mapper | `python3 -I -B Tools/Verification/YearComparison/run_offline.py` | 0; `dotnet build exit=0`; **132 PASS**, 43 cards; `fixture run exit=0` | `c-year-comparison` |
| c helper | `python3 -I -B Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0; **53 PASS**; explicit prebuild 0, underlying run 0 | `c-management-followon` |
| d static comparison | `python3 -I -B <evidence>/static-proof.py` | 0; removing only the new method and call restores baseline exactly; one guarded call, one readout Build, unchanged acknowledgement, no legacy input | `d-static`, plus `static-proof.log` |
| d call locations | `rg -n 'ScenarioOneManagementFollowonReadout\.Build\(\|BuildYearComparison\(\|AcknowledgeAnnualReview\(\|if \(!force && ChangeKey' Assets/ForestPrototype/UI/AnnualReviewView.cs` | 0; lines 84, 115, 175, 184, 187 | `d-call-sites` |
| d count | `rg -c 'ScenarioOneManagementFollowonReadout\.Build\(' Assets/ForestPrototype/UI/AnnualReviewView.cs` | 0; **1** | `d-build-count` |
| d baseline acknowledgement | `git grep -n 'AcknowledgeAnnualReview(' 6baee47b -- Assets/ForestPrototype/UI/AnnualReviewView.cs` | 0; same sole callback, same text, line 84 | `d-base-ack` |
| d legacy input | `rg -n 'UnityEngine\.Input' Assets/ForestPrototype/UI/AnnualReviewView.cs` | **1 means no match**, as expected; PASS | `d-legacy-input` |
| d classes | `rg -n '^\.(body \|card \|heading \|muted \|chip \|chip-crop \|chip-ok \|chip-bad \|faint \|hide-data-sources\|annual-text)' Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` | 0; existing definitions listed above | `d-class-definitions` |

The typecheck wrapper can exit 0 even when its compiler fails. Therefore the external `audit-bin/dotnet` forwarding wrapper from #415 was reused unchanged on PATH; it forwards to `/usr/bin/dotnet` and records subprocess output/exit without changing production tooling. Inspected `<evidence>/b-typecheck-dotnet/dotnet-build.json`: **underlying `dotnet build -nologo -v q -c Release '-clp:NoSummary;ErrorsOnly'` exit 0**. Its stdout reports **0 warnings and 0 errors**. Helper `prebuild.json` and `dotnet-run.json` each report exit 0; prebuild stdout reports 0 warnings and 0 errors. For the mapper, `dotnet-build.json` and DLL-run metadata are likewise retained under `c-year-comparison-dotnet/`.

**Gate b is a .NET syntax/typecheck against managed assemblies from the already-built Player at `/home/jer/ccf-verify-player/CCFVerify_Data/Managed`. It is not a Unity compile.** The two offline fixtures use existing test-only history doubles; they do not exercise UI Toolkit, actual saves or an Editor. No fixture/assertion/tool was weakened or modified.

## NOT UNITY-VERIFIED — T-05 still required

Every item below is **NOT RUN** in #416. T-05 belongs to a different worker with fresh ownership and machine-wide Unity lease, on the exact final source candidate in Unity **6000.6.0f1**. No Player or Windows build is authorised.

1. **Compile/import — NOT RUN.** Open/import the exact candidate in the assigned Unity lane. Confirm mapper GUID import and Assembly-CSharp compilation with no errors; the .NET gate above is insufficient.
2. **Placement and states — NOT RUN.** Open Annual Review with no completed year: original empty state, no comparison card. Latest completed year: exactly the mapper's one muted hint. Use Earlier year for complete, selected-missing, latest-missing and ambiguous fixtures: exact dated rows/gap messages, no borrowed numbers, card inside scroll after guidance when present. Check actual source-based species display names.
3. **Show data sources — NOT RUN.** With the comparison on screen, toggle the existing checkbox both ways. Its chips must hide/show with the other review chips; plain gap/uncertainty wording must remain visible. Confirm recorded/derived/not-recorded chip words and units/signs.
4. **Species foldout — NOT RUN.** On an earlier comparable year it starts closed. Open it: every recorded/missing species row is inside; continuation measures are indented; four species measures stay separate. Toggle chips while open, switch years/back and reopen the review: verify existing open-state/reset semantics, with no extra action or saved state.
5. **1280×720 and 1920×1080 fit — NOT RUN.** Capture latest, complete earlier and missing-snapshot states at both sizes, including expanded species. Text must remain readable and wrap; card must scroll; no clipped important row or displaced footer/year buttons. Stop/report if 720p needs a new shared style.
6. **Footer/year buttons and real input — NOT RUN.** Earlier/later navigation and Back to forest [Esc] work as before. Latest-year acknowledgement appears under the original conditions; earlier years hide it and cannot acknowledge the newest review. Title, skip summary and footer priority remain correct. No new acknowledgement or button path.
7. **Read-only/events/hash and saved histories — NOT RUN.** Compare event count, world hash and saved state before/after opening, year stepping, species toggling and closing (AC-7). Same saved history after N one-year advances versus one long advance must produce identical card rows (AC-8/FR-13). Check actual refresh/build invocation frequency if runtime instrumentation is used.
8. **Compatibility and Work-column parity — NOT RUN.** Current save, genuine older save, Legacy40 and frozen Reference Future v1 load/preview unchanged (AC-10); missing records show honest gaps. Compare only common event-counted Work categories on consistent records; report-derived removals and malformed provenance may differ only with the specified explanation (AC-12). Preserve all original saves and frozen reference bytes.
9. **Independent review/integration and dashboard synchronization — NOT RUN.** T-06/parent responsibility. This child has no authority to edit coordination/board, integrate into main or declare the feature accepted. Published dashboard/Drive ingestion has not been verified here.

## Assessment and unresolved work

- Verified: start and tested identities/cleanliness, one-file implementation scope, all offline/static gates and unchanged pre-existing view code.
- Contract relied upon: mapper row ordering and semantics already documented/tested by #415; account routing and prior usage-limit history are packet/issue facts, not fresh telemetry. No new ecological or saved-state assumption.
- STOP conditions encountered: **none**. No new style, saved state, expanded file scope or base workaround needed.
- Open product questions from this wiring: **none**. All Unity/runtime/rendered acceptance remains unverified, as listed above.
- Recommended next step: parent #412 review this bounded source diff and dispatch T-05; do not treat this task as Unity-qualified or integrated.
