# #422 — D-073 FR-10 visible provenance repair

**IMPLEMENTED ON TASK BRANCH; offline gates PASS.**

**NOT UNITY-VERIFIED; not an independent review**

Child [#422](https://github.com/jermur788/CCF/issues/422), parent [#412](https://github.com/jermur788/CCF/issues/412), Manager correction A for [#419 B-1](https://github.com/jermur788/CCF/issues/419).

## Identity and scope

- Worker: Codex, OpenAI `gpt-6.1-sol`, high reasoning, T3 Code/Codex harness; work performed directly, no subagents.
- Sole writable worktree: `/media/jer/Files/CCF/Worktrees/CCF-D073-FR10-Repair`.
- Branch: `feature/scenario-one-d073-fr10-visible-provenance-20261112-v1`.
- Starting HEAD: `77a56a4461f8ab2d313082af50442ce24dc54471`; confirmed clean and on the required branch before editing. RUNNING claim posted before implementation; `git fetch origin` completed.
- **Source-only commit: `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72`.** Tested clean source tree: `96ded36c897dbd92c44f7008a33838a1be63bcf4`.
- Source diff from starting HEAD: **1 file, 42 insertions, 0 deletions**, only `Assets/ForestPrototype/UI/AnnualReviewView.cs`. Second commit adds only this handoff. The exact branch-tip SHA is published in the #422 result comment.
- Production sources, fixtures, mapper/helper, USS, UiRoot, WorkPlan, save/event/hash code, `.meta`, scenes, prefabs and packages otherwise unchanged. No other checkout, Primary, main, ZX20, Ops board or live coordination file was edited. No new dependency or asset.

## Presentation and naming contract

StandMeasure and SpeciesMeasure rows now render the unchanged measure label and three separate, wrapping value groups. Dates and the `Difference` heading come from the mapper's existing TableHeader fields; numeric strings and units come directly from SelectedValue, LatestValue and Difference. The first species measure retains its species-name prefix. Observations pass **ValueProvenance** to the existing Tag helper; differences pass **DifferenceProvenance**. No RECORDED/DERIVED assumption is hard-coded.

Existing `row.Text` rendering remains for all other row kinds, including latest-year, missing snapshot, ambiguous, unresolved, event-only and missing-species warnings. Empty value columns produce **no value group and no tag**, never a zero or false RECORDED chip. In the selected-only state, the Year A value groups remain and the mapper's missing-latest warning remains; Year B/difference groups are absent. Mapper ordering guarantees the TableHeader precedes measure rows; the view retains that contract.

For each mapper `Row.Key`, let **P = `annual-year-comparison-` + Row.Key**:

| Element | Exact name |
|---|---|
| Existing measure label | `P` |
| Wrapping value-group parent | `P-values` |
| Year A group / caption / numeric value / chip | `P-year-a`, `P-year-a-heading`, `P-year-a-value`, `P-year-a-tag` |
| Latest Year B group / caption / numeric value / chip | `P-year-b`, `P-year-b-heading`, `P-year-b-value`, `P-year-b-tag` |
| Difference group / caption / numeric value / chip | `P-difference`, `P-difference-heading`, `P-difference-value`, `P-difference-tag` |

Example: `annual-year-comparison-stand-canopy-year-a-value` contains the mapper's SelectedValue; its chip is `annual-year-comparison-stand-canopy-year-a-tag`. The value label text is just the mapper value, permitting direct equality checks. Chips exist only for nonempty values with non-null mapper provenance. All other existing `annual-year-comparison-<Row.Key>` names remain unchanged. Tags retain the existing `prov` and `prov-recorded` / `prov-derived` / other provenance classes.

The groups reuse `row`, `row-wrap`, `annual-text-entry`, `annual-text-source`, `annual-text-body`, `muted` and existing Tag/ChipStyle primitives. A 180-point minimum group width, equal flexible shares and wrapping avoid making a long combined A/B/difference line. No USS change. Actual 720p/1080p fit is **not verified**.

## Sources ON / OFF and spec basis

Approved spec pinned at `53dec8db19a9bdde20176b2f3abf508402c7f0af`, [spec.md line 110 (FR-10)](https://github.com/jermur788/CCF/blob/53dec8db19a9bdde20176b2f3abf508402c7f0af/specs/scenario-one-multiyear-decision-outcome-feedback-v2/spec.md#L110): “Every displayed value carries correct provenance” and “The card MUST respect the existing **Show data sources** toggle in the same way as the rest of the review.”

**Decision:** retain existing disclosure. Sources ON shows separate plain-text RECORDED observation chips and DERIVED difference chips (or whatever provenance the mapper supplies). Sources OFF hides these chips through unchanged `.hide-data-sources .prov`; FR-10 does **not** require keeping them visible against the existing toggle. Dated captions, numeric values, Difference headings and all plain uncertainty/event-only warnings remain visible. Existing SourceQualifier text, such as “calculated from records”, also remains, exactly as with other review tags. No change to the toggle callback or root class rule.

## Gates on the exact source commit

Evidence root: `/media/jer/Files/CCF/Verification/D073-FR10-Repair-422-20261112/`. All generated compiler/fixture projects and temporary output are outside the repository. Existing runners/assertions were inspected before running and were not edited. `run_gate.py` records command exits; `audit-bin/dotnet` records actual subprocess argv/cwd/stdout/stderr/exit and adds `-p:UseSharedCompilation=false` to build/run, avoiding shared compiler-server dependence.

| Gate | Result and real numbers |
|---|---|
| Production scope | **PASS:** `git diff --stat 77a56a44..HEAD` on source commit: exactly **1 production file, +42 / -0**. Final publication adds only this Markdown handoff. |
| Production syntax/type binding | **PASS:** existing `Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <worktree> --out <evidence>/typecheck/project`: **192 sources, 116 references, compiler exit 0, errors 0**. Inspected actual dotnet-build JSON and stdout: **0 warnings, 0 errors**; wrapper exit also 0. |
| Helper unchanged offline fixture | **PASS:** existing `Tools/Verification/ManagementFollowonReadout/run_offline.py --out <evidence>/helper/project`: **53 focused assertions PASS**, underlying `dotnet run` exit **0**. |
| Mapper unchanged offline fixture | **PASS:** existing `Tools/Verification/YearComparison/run_offline.py --out <evidence>/mapper-retry/project`: **132 assertions PASS, 43 cards**, actual build exit **0**, **0 warnings / 0 errors**, fixture DLL exit **0**. |
| Guard and ACK preservation | **PASS:** **10 static checks, 0 failures** in `static-proof.json`; original sole ACK callback at line **84**, change-key guard/return at **115–116**, sole comparison call at **175**, sole readout Build at **187**. All code before BuildYearComparison and from BuildHeadlines onward is byte-identical to baseline. |
| Hygiene | **PASS:** source commit clean; `git diff --check 77a56a44..HEAD` exit **0**. Fixture/tool subtree diff empty. |

Type binding uses managed assemblies from the **already-built** Player at `/home/jer/ccf-verify-player/CCFVerify_Data/Managed`, as #415/#416 did. It is **not Unity compilation or a game build**. Both fixtures use their existing test-only history doubles and do not execute UI Toolkit.

**Failed recorder attempt retained:** first mapper attempt (`mapper/`) built successfully with 0 compiler errors, but the external audit wrapper treated the absolute DLL argument as part of a filename and failed to write its run log (FileNotFoundError, wrapper/fixture exit 1). No PASS is claimed for that attempt. Corrected only the external recorder to use the executable basename and reran into `mapper-retry/`; all 132 passed with both actual subprocess exits 0. Original logs and initial recorder/launcher copies remain preserved. No source/test change or assertion relaxation.

## Remaining parent-owned verification

**NOT RUN:** Unity Editor/import/compile, rendered UI/value-child oracle, native input, 1280×720 and 1920×1080 layout, runtime events/hash/save equality, actual old-save/Legacy40/frozen Reference smoke, independent review, integration, user playtest, any Player/Windows/game build. No Unity started. Dashboard ingestion/Drive synchronization and physical Ops state were not verified by this child.

Parent #412's separate test writer/reviewer should qualify **source commit 339517ed2d49bb3d5716fc9cf029d9ef0eaeee72** (or a candidate with identical production tree): open an earlier Annual Review with both snapshots, enable sources, verify each named stand and expanded-species value against its mapper field and each chip against its field-level provenance; disable sources and confirm chips hide while dated values and warnings stay visible. Check selected-only, selected-missing, ambiguous, event-only and one-sided species states; then verify original latest hint, year controls, species foldout/scroll, ACK/footer priority and read-only state at both resolutions. Stop if layout requires a forbidden shared-style change. No new AcknowledgeAnnualReview caller exists.

Ready for independent source review and focused Unity qualification; no integration, B-1 independent closure or release approval claimed.
