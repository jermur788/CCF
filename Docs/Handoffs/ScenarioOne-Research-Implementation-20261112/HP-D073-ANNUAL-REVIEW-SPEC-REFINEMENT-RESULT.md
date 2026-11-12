# HP #409 — D-073 Annual Review comparison spec refinement result

**Worker assessment: DRAFT REFINED — ready for Overall Manager review.** Documentation only. This is not approval of the spec, not implementation, and not Unity verification. The four files remain `DRAFT — MANAGER REVIEW REQUESTED`.

## Implementation handoff

| Field | Value |
|---|---|
| Task | [#409](https://github.com/jermur788/CCF/issues/409), under the [Manager packet](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-HP-FIRST-TASK-D073-SPEC-REFINEMENT-20261112.md) and the [Manager #411 release review](https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-411-REVIEW-HP409-READY-RELEASE-20261112.md) |
| Worker | Claude Fable 5.2 (`claude-fable-5-2`), Claude Code harness inside T3 Code, HP ProDesk. Sole worker; no delegates or subagents |
| Role | Primary (spec author). Task class SPEC REQUIRED — spec documentation only |
| Branch | `docs/d073-annual-review-spec-hp-20261112-v1` |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-D073-AnnualReview-Spec` (Ubuntu WSL2, ext4) |
| Base (pinned) | `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4` = `origin/qualify/scenario-one-management-followon-readout-20261112-v1`, fetched and unchanged at start |
| Context commit | Decisions read from `main` canonical context `3f4e852e9579de7335e6d71ca660ecd2b0f8b561`. See "Context commit note" |
| Spec commit | `e6dbd68f09ee3af9dc6fe67a4b59fc9c872b4df3` (four spec files) |
| GitHub handoff path | `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/HP-D073-ANNUAL-REVIEW-SPEC-REFINEMENT-RESULT.md` |
| GitHub handoff commit | the commit after `e6dbd68f` that adds this file; it is the branch HEAD named in the #409 hand-off comment |
| External evidence archive | none — all evidence is in this file and the two commits |

### What changed

The #400 browser draft was rewritten into one narrow, self-consistent proposal for the Manager's **Option B**: a single read-only card inside the existing Annual Review that compares an earlier completed year with the latest completed year.

- **spec.md** — follows the `specs/_template` structure. Fourteen numbered requirements (FR-1 to FR-14) replace M1–M7. Twelve acceptance criteria. A state table giving exact wording for every helper state. A text-only layout sketch with placeholders. An offline-tested versus Unity-unverified table. Stop conditions. Six open decisions, each with a recommendation.
- **plan.md** — current-state findings from source, a two-piece approach (pure text mapper plus a thin view method), exact file boundaries, ownership dependency on the Fable cadence lane, verification split by tier, risks, and a sequence mapped to task IDs.
- **tasks.md** — seven bounded tasks (T-00 to T-06) in template form. Each has scope, exclusions, verification, stop conditions and a completion state. The implementer (T-04) and the Unity qualifier (T-05) are different workers with separate gates.
- **checklists/requirements.md** — every FR and AC as an evidence line, grouped by where it can be checked: offline, source review, or Unity.

### Files changed

Relative to base `0ce4793f`, exactly five paths:

1. `specs/scenario-one-multiyear-decision-outcome-feedback-v2/spec.md` (+162 −35)
2. `specs/scenario-one-multiyear-decision-outcome-feedback-v2/plan.md` (+73 −11)
3. `specs/scenario-one-multiyear-decision-outcome-feedback-v2/tasks.md` (+117 −12)
4. `specs/scenario-one-multiyear-decision-outcome-feedback-v2/checklists/requirements.md` (+96 −29)
5. `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/HP-D073-ANNUAL-REVIEW-SPEC-REFINEMENT-RESULT.md` (new, this file)

- **Serialized Unity files touched:** none.
- **Player-facing result:** none. No game behaviour changed.
- **Simulation / save impact:** none.
- **Assets added or modified:** none.
- **Must-not-change hashes / anchors:** none touched. No source, save, reference or hash file is in the diff.
- **Intentionally changed hashes / anchors:** none.

## Scope chosen (F0)

**Option B, minimum first slice.** In the card:

- viewed-year resolved work: succeeded counts by task type, one failed count, one cancelled count;
- the stand recorded at the end of the viewed year and at the end of the latest completed year, both dated;
- five same-unit stand differences: standing trees, basal area, mean canopy cover, regeneration map squares (modelled), fallen deadwood volume;
- per species, inside a closed foldout: standing trees, natural regeneration squares, planted regeneration squares, planted young trees;
- provenance words and explicit wording for every missing or ambiguous record.

Left out of this slice, by decision recorded in the spec's non-goals:

- money in the card — the existing money column and harvest settlement box stay the only financial display, so no per-tree or per-order currency can appear;
- per-coverage-unit executed-work grouping and candidate treatment years — the helper provides them, but they belong to the D-076 and D-074 drafts;
- mean DBH, mean light, deadwood piece count, regeneration density and per-species basal area — available, but with no established player-facing unit wording;
- any new advice text — the card ends with one fixed caveat and points to the existing prior-treatment guidance.

## Reconciliation with the #406 helper

Checked against the actual `ScenarioOneManagementFollowonReadout.cs` on the base (blob `2c7bdb3c6b6f13790a325ebabc187c497b335b52`), not the earlier browser source.

| Packet requirement | Where the draft enforces it |
|---|---|
| No per-rack historic ecology | Non-goals; FR-3 limits the card to one `Build` result of stand and species aggregates |
| No old-save backfill | Non-goals; FR-6; FR-9 gap wording; AC-10 |
| Candidate fell year does not certify a major intervention | `PossibleTreatmentYears` is not displayed; open decision 6 |
| Event and order targets validated; duplicate IDs fail closed | FR-5: ambiguous rows are left out of counts and shown on their own line |
| Grouped money needs report and settlement; never per-tree currency | FR-12: the card shows no currency |
| Natural model cells and density versus planted cells and exact juveniles | FR-8: four separate measures, never summed; "modelled" versus "counted" |
| No invented survival, hectares or causal attribution | Non-goals; FR-11; AC-11 |

One fact found in the source that the earlier draft did not state: `Result.RecordedWork` contains rows for **every** completed year, not only the selected one. FR-4 therefore requires filtering by year. A second: `ScenarioOneUiFacts.SpeciesName` looks up a scene object, so the plan passes species names into the text mapper as a function to keep it testable offline.

## Verification run

Tier 1 documentation checks, run in the worktree:

| Check | Result |
|---|---|
| `git diff --check` on the four spec files | PASS — clean |
| Carriage returns in the four files | 0 in each |
| Each file ends with a newline | PASS |
| Relative link targets exist in the worktree | PASS — 10 of 10 |
| Every `FR-n`, `AC-n` and `T-nn` referenced in any of the four files is defined | PASS — none undefined |
| Status line agrees across the four files | PASS — `DRAFT — MANAGER REVIEW REQUESTED` in all four |
| Staged set for the spec commit | exactly 4 paths |
| Committed #406 evidence contains the cited figures | Read, not re-run: `commands.json` holds `PASS 53 focused assertions`; `typecheck-final.log` holds `sources=191 refs=116 exit=0 errors=0` |

**Not run by this task:** any C# compile, the 53 helper assertions, the mutation runs, Unity import, compile, Editor, runtime, rendering, save or load, Legacy40, Reference Future, or any build. The packet excludes them. External GitHub links in the files were not fetched one by one.

**Spec conformance:** NOT VERIFIED — there is no implementation to conform.

### Four-file consistency

- Requirement and acceptance IDs are defined once, in `spec.md`; the plan, tasks and checklist only refer to them.
- Every FR appears in the checklist under exactly one check location, except FR-14, which is split into its offline part and its source-review part.
- Every AC appears in the checklist, and each Unity-only AC is named in T-05's scope.
- The plan's seven sequence steps map one-to-one to T-00 … T-06.
- File boundaries in the plan match the scope and exclusions in T-03 and T-04.
- The helper blob `2c7bdb3c…` and the cadence dependency appear in the spec's stop conditions, the plan's ownership section, T-01 and the checklist preconditions.

## Offline-tested versus Unity-unverified

Offline-tested by #406 (worker-reported, logs committed, Manager-approved for offline scope): the helper's exact-year logic, its four comparison states, fail-closed identity checks, the natural/planted separation and its binding to production type signatures.

Unity-unverified: the helper inside Unity 6000.6; every part of the card; fit at 1280×720 and 1920×1080; scroll and acknowledgement priority; identical content after one-year and long advances; genuine old saves, Legacy40 and frozen Reference Future v1. The layout in the spec is a text sketch that has never been rendered.

## Remaining verification gates

T-00 Manager decision → T-01 cadence release and base → T-02 helper re-run on that base → T-03 mapper fixture → T-04 wiring → T-05 Unity Tier 2 by a different worker → T-06 independent review and verdict.

## Known limitations

- The cadence candidate `e7591933` was read only for its 15-line `AnnualReviewView.cs` diff. Its qualification ([#408](https://github.com/jermur788/CCF/issues/408)) was still RUNNING at start.
- The proposed copy has not been seen in game and may not fit at 720p.
- The physical board proof is the #411 snapshot on GitHub. The HP cannot read the Predator board.
- `current-milestone.md` and `game-brief.md` were not re-read for this task; the spec links the milestone rather than restating it.

## Unrelated work preserved

- `/home/jer/Development/CCF-HP` stayed on `main` at `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`, clean. It was not edited or fast-forwarded. `git fetch` updated remote-tracking refs only, and the new worktree registered itself in that clone's `.git/worktrees`.
- No Windows-side Git was used and no `C:\` copy of CCF exists.
- `main`, `active-tasks.md`, the Fable cadence branches, the #382 personal lane, Unity and all game source are untouched. No pull request was opened.

## Decisions needed (Overall Manager)

1. APPROVED / NEEDS CHANGES / REJECTED for this four-file draft.
2. Latest-year view: one muted line (recommended) or no card until an earlier year is chosen.
3. Felling word in the card: reuse the Work column's "thinned" (recommended) or neutral "felled". The existing column counts rack-access felling as "thinned"; this slice classifies nothing.
4. Keep the two work-count lines (recommended) or replace them with a pointer to the Work column, which removes AC-12.
5. Confirm the deferred measures stay out of the first slice.
6. Confirm per-unit grouping and candidate treatment years stay with D-076 and D-074.
7. Confirm the context-commit handling described below.
8. Whether T-02 and T-03, which touch no cadence-owned file, may be released before the cadence lane finishes.

## Context commit note

The packet supplied `d05b27ad376ce02748ee4703429ad3ff66a65542`. That commit is dated 2026-10-29 and predates D-073. Three different values were in play:

- `d05b27ad…` — packet value; an ancestor of both `main` and the base.
- `63044dfb…` — the latest context-source commit contained in the pinned base (2026-11-05). The base's own Decision Log has no D-073 row.
- `3f4e852e…` — the latest context-source commit on `main` (2026-11-12), which holds D-073 to D-077.

The draft cites D-073 and D-077 from `main` at `3f4e852e…` and implementation facts from the base at `0ce4793f…`. `AGENTS.md` and `AgentWorkflow.md` were read from `main`.

## Routing ledger

Format follows the classes of the proposed Orchestration Evidence Ledger Standard V3 (`OBSERVED_FACT`, `ORCHESTRATOR_CLAIM`). Times are UTC on 2026-11-12. Where a time was not captured when the event happened it is written `not captured` rather than estimated. All entries were recorded when this file was written.

### Starting policy

- **Objective:** refine four D-073 DRAFT files and publish one result. **Protected:** Unity, game source, Fable #402/#408, personal #382, `main`.
- **Hard constraints:** five writable paths; one writer; no build; no PR; no local physical board.
- **Preferences:** correctness and scope discipline over speed; lowest-cost route that can do the task.
- **Concurrency:** one worker, one worktree, one branch.
- **Review:** the Manager reviews; this worker does not self-approve.

### Decisions

| ID | Occurred | Decision | Class | Basis and alternatives |
|---|---|---|---|---|
| R-01 | session start, not captured | **Provider and model: Claude Fable 5.2 via Claude Code in T3 Code.** The packet preferred Claude Sonnet 5.5 High, with Codex GPT-6 Medium as fallback | OBSERVED_FACT (model in use); ORCHESTRATOR_CLAIM (why) | The user dispatched this session on this model. The worker did not switch provider or model and could not have observed quota for the others. Cost versus the preferred route: unknown |
| R-02 | session start, not captured | **No delegation.** No subagents, no second provider, no parallel threads | ORCHESTRATOR_CLAIM | Single small document set with one writer; delegation would add cost and a second writer for no gain |
| R-03 | before first command | **Execution route: Windows host shell → `wsl.exe -d Ubuntu` → bash.** Every Git and file operation ran inside Ubuntu on ext4 | OBSERVED_FACT | `gh` and the CCF clone exist only inside Ubuntu. The packet asks for Ubuntu execution rather than a PowerShell subprocess; the session's host shell is Windows, so this is disclosed rather than hidden. Draft text was composed in the session's scratch directory and copied in with carriage returns stripped |
| R-04 | preflight | **Board ownership proof taken from GitHub** (#411 result at `302d2379…`, Manager review at `af86f97f…`), not from disk | OBSERVED_FACT | The Manager's #411 review names this as the accepted route. No local board was created and no access to the Predator was attempted |
| R-05 | preflight | **Proceed rather than report BLOCKED** | ORCHESTRATOR_CLAIM | `main` at `861b61bb…` released #409; #411 and #410 closed; target branch, worktree and result path absent everywhere; no open claim names the five paths; the cadence candidate does not contain the spec folder |
| R-06 | preflight | **Base pinned to `0ce4793f…`**; decisions read from `main` at `3f4e852e…` | OBSERVED_FACT | See "Context commit note". Alternative rejected: starting from `main`, which the packet forbids |
| R-07 | 11:54:23 | **Worktree and branch created**, then #409 marked RUNNING | OBSERVED_FACT | RUNNING comment at 11:55:02: https://github.com/jermur788/CCF/issues/409#issuecomment-6822898431 . The Manager's release text orders "pin, then mark RUNNING"; the user's dispatch orders "create worktree, then mark RUNNING with the actual worktree". The second was followed so the comment could state real values; 39 seconds separated the two |
| R-08 | drafting | **#406 checks not re-run.** Figures cited from the committed logs | ORCHESTRATOR_CLAIM | The packet lists "build" and "C#" as out of scope. Re-running would have been cheap but outside the grant; the draft labels the figures as worker-reported |
| R-09 | drafting | **Verification tier: Tier 1, documentation checks only** | ORCHESTRATOR_CLAIM | Nothing in the diff can affect runtime |
| R-10 | 12:01:09 | **Commit identity supplied per command** (`jermur788` with the GitHub no-reply address) | OBSERVED_FACT | The Ubuntu clone has no configured Git identity. Git configuration was left unchanged |
| R-11 | publication | **Branch push only; no pull request** | ORCHESTRATOR_CLAIM | The packet asks for a published result on the branch; nothing asks for a PR |
| R-12 | publication | **Ledger kept in this file** | ORCHESTRATOR_CLAIM | The user asked for the ledger to be filled in. `Docs/Project/coordination/development-throughput-ledger.md` is Manager-owned on `main`, and the Predator board row is unreachable from the HP; both are outside the five writable paths |
| R-13 | hand-off | **#409 retitled [HANDED-OFF] and closed** | ORCHESTRATOR_CLAIM | The packet and the issue body both say to mark HANDED-OFF and close the execution claim. Closure is hand-off, not acceptance |

### Capacity

Provider quota and remaining capacity for this session: UNKNOWN — not observable from inside the session. No rate limit, retry, switch or interruption occurred.

## Dashboard and board synchronisation

**Not available from the HP; not verified.** Outstanding for the Manager or the Predator board owner:

- update the `active-tasks.md` #409 entry with this return;
- move the physical row `CCF-409-HP-D073-SPEC-REMOTE` from READY to its post-hand-off state. It was never set to RUNNING on the physical board, because the HP cannot write it;
- run or allow dashboard ingestion and confirm the published state; Drive mirrors likewise unverified.

## Worker assessment

**Ready for review: Yes — as a DRAFT for the Manager's decision.** Recommended next step: T-00. The worktree is kept for review; the writer is finished.
