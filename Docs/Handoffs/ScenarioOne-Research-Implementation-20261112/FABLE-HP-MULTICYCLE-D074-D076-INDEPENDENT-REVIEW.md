# D-074 / D-076 independent review — HP #413

**INTEGRATION DECISION: APPROVED — documentation readiness only.**
**Gate: G2, bounded acceptance of the ten-file draft/source-audit evidence.**
**Both specifications remain DRAFT — MANAGER REVIEW REQUESTED.**

## Review identity, authority and exact scope

- Date: 12 November 2026.
- Independent reviewer: OpenAI Codex, **gpt-6.1-sol, medium reasoning effort**, Codex harness in T3 Code; fresh reviewer separate from the writer. No subagents/delegation.
- The requested Claude 2 Sonnet 5.5 xhigh review reportedly failed before work with an API rate limit. This review actually ran on Codex under the user's latest available-provider instruction. Claude execution and remaining provider quota are not claimed; quota was unavailable to inspect.
- [Reviewer claim posted before substantive review](https://github.com/jermur788/CCF/issues/413#issuecomment-6827249940).
- Reviewed branch: `docs/fable-hp-d074-d076-spec-programme-20261112-v1`.
- Exact reviewed HEAD: `fc90aded4c9462676de5420fe872c9ab4ffabde3`.
- Exact diff base: `53dec8db19a9bdde20176b2f3abf508402c7f0af`.
- Locked canonical context: `63044dfbbeb869996ceecec8a47619dcba048b0b`. Read AGENTS, AgentWorkflow, Decision Log, Current Milestone, Game Brief, Unity Overview and relevant Research Index.
- Updated D-064/065/071/073/074/076/077 read separately from live GitHub main; Decision Log blob `998135a36f2ff0c632a21d8ba9fdaf9bf7798491` matches the writer's immutable main pin `91511faab737718f65fee896b32628a755cae90f`.
- Read the binding orchestration packet on `docs/manager-dual-fable-dispatch-20261112-v1`, live #413/comments and live main active-tasks, including the earlier capacity pause and planned laptop fallback. Latest direct user instruction authorises this bounded review; no parent-account or lane transfer is inferred.
- Read published physical registration report at `843095ced6bc682b31290bfca3f7dfe2323b6cd9`: it reports lint PASS, #409 retired and #413 docs-only registration. **No fresh physical board, lease or process inspection.**
- Implementation truth: cadence isolated integration **E** `e759193354b4613f88e3370a62ccaccf02c556cf`; separate #406 helper **R** `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4`. R is not assumed integrated at E.

Windows PowerShell hosted commands; Git/gh/source-read checks ran inside Ubuntu WSL2 through `wsl -d Ubuntu -- bash -lc` (the disposable Python document checker was invoked with `wsl -d Ubuntu -- python3`). Root and existing docs worktree were read-only and clean at initial inspection. All source reads used pinned Git objects, not checked-out stale main. No checkout, worktree creation or repository file edit was performed.

The full ten-file change was inspected: `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` in each of:

- `specs/scenario-one-spaced-major-intervention-learning-v1/`
- `specs/scenario-one-multicycle-representative-rack-management-v1/`

Plus only `FABLE-HP-D074-COMPLETION-HISTORY-SOURCE-AUDIT.md` and `FABLE-HP-D076-REPEATED-AREA-MFM-SOURCE-AUDIT.md` under this handoff directory. Diff: **10 Markdown files, 344 insertions, 186 deletions**.

## Findings and required dispositions

**No blocking documentation defects found. No corrective edit to the reviewed ten files is required for this verdict.** The following are disclosed source/design risks, not newly executed failures or reasons to treat the drafts as implementable:

| Severity / scope | Exact reviewed location | Independent assessment | Required next action |
|---|---|---|---|
| High — future integration risk, correctly disclosed | [D076 source audit, line 35](https://github.com/jermur788/CCF/blob/fc90aded4c9462676de5420fe872c9ab4ffabde3/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/FABLE-HP-D076-REPEATED-AREA-MFM-SOURCE-AUDIT.md#L35); [D076 spec, line 46](https://github.com/jermur788/CCF/blob/fc90aded4c9462676de5420fe872c9ab4ffabde3/specs/scenario-one-multicycle-representative-rack-management-v1/spec.md#L46) | Confirmed **static integration inference**: E stamps prescription IDs on orders/OrderCreated, but RecordOrderEvent and AdvanceYear omit the ID on WorkResolved. R ValidOrder requires equality, so ordinary nonzero-prescription orders paired with zero-ID resolution events fail that join and cannot enter ExecutedCoverage. Synthetic fixture factories/calls supply matching IDs. Not a runtime-proven bug. | Separate approved provenance disposition and authentic manager-produced event/order fixture before relying on grouped real MFM history. Do not silently stamp events or change hashes under this docs task. |
| High — product/save approval gate, explicitly open | [D074 spec, lines 19, 23, 33 and 43–47](https://github.com/jermur788/CCF/blob/fc90aded4c9462676de5420fe872c9ab4ffabde3/specs/scenario-one-spaced-major-intervention-learning-v1/spec.md#L19) | Assessed non-gating learning is a proposal, not accepted D-074 fulfilment. Advisory copy alone is expressly insufficient. Completed outcomes and old Active completion routes are protected; serializer v24/default-empty history/teaching latch do not establish D-074 origin. | Manager resolves MD-01…MD-05 and explicitly approves an assessable objective/completion, origin, progress, review and hash contract. No inferred cohort or retrospective credit. |
| Medium — later-cycle design/qualification gate, explicitly open | [D076 spec, lines 34–42](https://github.com/jermur788/CCF/blob/fc90aded4c9462676de5420fe872c9ab4ffabde3/specs/scenario-one-multicycle-representative-rack-management-v1/spec.md#L34) | Current sourceYear is acceptance year; retained completed target work can exclude a unit; Approved prescriptions cannot simply be approved again. Validation has no blanket year-expiry test. Fresh sample identity/comparison state and episode/unit/tree ownership remain necessary. | Resolve MD-02…MD-05; qualify prior-stump/repeat/stale/overlap cases with manual remainder. Preserve history and current blockers until an approved replacement exists. |

Decisive source reads for the first finding:

- E [ScenarioOneManager.PrescriptionApproval.cs, lines 186–197](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs#L186): order provenance and OrderCreated stamp.
- E [ScenarioOneManager.cs, lines 1325–1345](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1325) and [2466–2487](https://github.com/jermur788/CCF/blob/e759193354b4613f88e3370a62ccaccf02c556cf/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L2466): resolution path/event construction.
- R [ScenarioOneManagementFollowonReadout.cs, lines 248–259](https://github.com/jermur788/CCF/blob/0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4/Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs#L248): equality required; Build groups only Verified succeeded rows.
- R [ManagementFollowonReadoutVerification.cs, lines 26–35](https://github.com/jermur788/CCF/blob/0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4/Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs#L26): synthetic provenance; checked matching-prescription fixture calls as well.

For the repeat findings, inspected E CreatePrescription (`sourceYear = CurrentYear`), ScenarioOneCoverage.Classify (`PlayerCompletedWork`), ValidatePrescription/Context, ApprovePrescription and TryReadPrescriptionWorld directly.

## Conformance assessment

**PASS for preparation of reviewable drafts and bounded static evidence; implementation conformance NOT VERIFIED.**

D-074 correctly distinguishes the current two-successful-plan-year completion sequence from substantive treatment. Planting/access/maintenance/single arbitrary Fell are not promoted to treatment evidence. Manual tree selection and MFM need real succeeded work plus meaningful decisions, with mixed plans classified rather than wholly discarded. The numerical minimum and reassessment proof remain Manager choices. Exact annual data, actual review and live field judgement are distinct; skipped snapshots do not become acknowledgements or claimed field visits. D-077 navigation remains separate.

D-076 preserves the original fixed third-rack lesson, three Crop Trees and protected safeguards. Later sources require a fresh demonstrated area, current comparability, explicit manual/noncovered fallback, all-or-nothing Pending creation and separate Work Plan approval. Optional multi-source support requires exclusive unit ownership and target-tree deduplication. Five racks do not become five compulsory samples. Sitka automation is not extended to Oak/Beech. Existing stand/species snapshots are not misrepresented as historical per-rack ecology or causal response.

Both four-file packages consistently retain DRAFT / MANAGER REVIEW REQUESTED status, pending product/save choices and unreleased production tasks. FR/AC/MD references and task/checklist coverage agree. Audit case tables are clearly static predictions; proposed runtime gates remain NOT RUN. No unsupported empirical forestry quantities were introduced: local constants and the possible three-year interval are labelled calibration. This review did not revalidate external forestry literature or earlier agents' private execution histories.

## Checks actually executed

| Bounded check | Result |
|---|---|
| Exact base→HEAD changed-path allowlist | PASS: eight expected spec files + two named audits; no production/serialized/asset/package/project changes |
| `git diff --check 53dec8db19a9bdde20176b2f3abf508402c7f0af fc90aded4c9462676de5420fe872c9ab4ffabde3` | PASS |
| UTF-8, no BOM, LF, final newline across ten blobs | PASS |
| Relative Markdown links resolved against exact HEAD tree | PASS: 28 links |
| Sequential unique definitions and valid references | PASS: D074 FR-01…08 / AC-01…07 / MD-01…05; D076 FR-01…09 / AC-01…08 / MD-01…06 |
| Draft headers and manual spec/plan/tasks/checklist consistency review | PASS |
| Pinned source/fixture inspection of completion, persistence, source geometry, coverage, prescription approval, cadence and helper | PASS for the bounded static conclusions above |
| Remote writer branch exact HEAD before publication | PASS: still fc90aded4c9462676de5420fe872c9ab4ffabde3 |
| Evidence branch collision check before creation | PASS: GitHub returned branch-not-found (404); no existing owner/content replaced |

The checker and diff copies were temporary files under WSL `/tmp`, outside both repositories. No auto-start verification script/meta was placed under Assets.

## Verdict limits and handoff

**APPROVED** means the parent may treat this independent review as satisfying the bounded G2 review of draft readiness/source evidence. It does **not** approve product options, either specification for implementation, G3 runtime/measurement qualification, G4 integration/canonical changes, G5 save/scope choices, Unity, or user acceptance. No integrated HEAD exists for this review.

No compilation, offline C# fixture execution, Unity import/Editor/runtime, save/load, rendered/native-input, Player/build, performance or frozen-hash replay was run. These were outside this docs-only brief. Historical cadence/helper gates were not rerun. Future authentic fixture, compatibility, rendered 720p/1080p and runtime gates remain as listed in the drafts.

Publication is limited to this new report on `docs/hp-d074-d076-independent-review-20261112-v1`, based exactly on the reviewed HEAD, through GitHub-native APIs. The publication commit and byte readback receipt are returned to the parent; the report cannot contain its own commit SHA. Parent retains sole ownership of SPEC-RESULT, issue status/closure, coordination and dashboard/Drive disposition. No main merge, task-branch mutation, physical-board edit or watcher. Dashboard/Drive synchronization remains unverified and administratively outstanding for the parent.
