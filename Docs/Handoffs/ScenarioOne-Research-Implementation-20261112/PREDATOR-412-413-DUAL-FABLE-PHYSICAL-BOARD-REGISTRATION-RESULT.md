# Predator #412 / HP #413 — dual Fable physical-board registration result

**Status:** REGISTERED. Both parent rows are **READY / NOT RUNNING**. Native board lint **PASS**. No conflict found.
**Date:** 2026-11-12, edit applied between 12:52Z and 13:00Z.
**Actor:** Predator #412 Fable A parent, Claude Fable 5.2 (`claude-fable-5-2`), Claude Code in T3 Code on the Predator (`jer-laptop`), provider instance "Claude A". Acting as delegated temporary physical-board integrator for this assignment only (Manager packet Phase 0).

## What changed on the physical board

One atomic replace of `/media/jer/ZX20/CCF-Ops/board/active.md` (candidate written beside the board, linted, live hash re-checked, then renamed over it).

| | SHA-256 | Bytes | Live tasks |
|---|---|---|---|
| Before | `638243d1da026f04aa7ac931d85a9a6c6f95300339ee2b52b1818cf1989f87ec` | 59152 | 22 |
| After | `5e0ae753d66ef93dd4a08eabe9edcad7cca1edf19dbb239a99979775d6f2fc2b` | 63149 | 24 |

Line-level change: old line 7 (the #409 row) was replaced by three lines (new #412 row, new #413 row, updated #409 row). Every other line is byte-identical: `difflib` opcodes were exactly `[('replace', 6, 7, 6, 9)]`, and applying the inverse of the #409 edit to the after-file and dropping the two new rows reproduces the before SHA-256 exactly, so the edit is reversible. No row for #403, #406, #408, #382 or any other task was touched. `state/board-history.jsonl` and `state/unity` were not written.

Note: the before-file was not copied before the edit (a slow read-only `lane-status` held up the copy step). The before state is evidenced by the SHA-256 the edit script asserted immediately before writing, and by the exact reconstruction above.

### Row retired: `CCF-409-HP-D073-SPEC-REMOTE`

Precondition checked: state READY, owner "Remote HP ProDesk Ubuntu WSL2 worker…", branch `docs/d073-annual-review-spec-hp-20261112-v1`, HEAD TBD, and GitHub #409 CLOSED `[HANDED-OFF / MANAGER SPEC APPROVED]` with result branch at `6f9d3c902f590ad964f357d0c04bb2b5bcd085ab`. All matched.

Before:

```
| CCF-409-HP-D073-SPEC-REMOTE | READY | Remote HP ProDesk Ubuntu WSL2 worker (board row READY for separate Manager GitHub release; remote HP task NOT RUNNING; no write start until Manager release) | T3 Code (remote HP) | Claude Sonnet 5.5 High (Codex GPT-6 Medium also suitable) | Remote HP T3 Code; no Predator quota or lease | None | docs/d073-annual-review-spec-hp-20261112-v1 | TBD | Remote HP docs-only D-073 draft-spec refinement; proposed files only: specs/scenario-one-multiyear-decision-outcome-feedback-v2/spec.md, plan.md, tasks.md, checklists/requirements.md and Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/HP-D073-ANNUAL-REVIEW-SPEC-REFINEMENT-RESULT.md | Remote HP, spec-only, no Unity lease, no Predator lane or worktree, READY / NOT RUNNING, pending separate Manager GitHub release; mirrors Manager GitHub entry only; Fable #402/#408, personal #382, other lanes and frozen Reference Future protections unchanged | No | None | Tier 1 | Manager review of #410 proof and explicit separate release of #409; no HP start before then | Proposed source base qualify/scenario-one-management-followon-readout-20261112-v1 (refresh at start); Manager GitHub active-tasks #409 entry 2026-11-12; #410 proof | Remain NOT RUNNING until Manager GitHub release of #409 (GitHub #409 stays BLOCKED until then); then isolated HP worktree CCF-D073-AnnualReview-Spec; no Unity, no source, no merge |
```

After:

```
| CCF-409-HP-D073-SPEC-REMOTE | DONE-PENDING-LEDGER | NONE (remote HP docs-only worker finished; GitHub #409 HANDED-OFF/CLOSED; no active writer) | T3 Code (remote HP) | Claude Sonnet 5.5 High (Codex GPT-6 Medium also suitable) | Remote HP T3 Code; no Predator quota or lease | None | docs/d073-annual-review-spec-hp-20261112-v1 | 6f9d3c902f590ad964f357d0c04bb2b5bcd085ab | Remote HP docs-only D-073 draft-spec refinement; proposed files only: specs/scenario-one-multiyear-decision-outcome-feedback-v2/spec.md, plan.md, tasks.md, checklists/requirements.md and Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/HP-D073-ANNUAL-REVIEW-SPEC-REFINEMENT-RESULT.md | Remote HP spec-only work complete; never held a Unity lease, Predator lane or worktree; row was never set RUNNING on this board (retired from READY 2026-11-12 by Predator #412 Fable A as delegated temporary board integrator); Fable #402/#408, personal #382, other lanes and frozen Reference Future protections unchanged | No | None | Tier 1 | Manager ledger entry; D-073 four-file spec Manager-APPROVED on docs/manager-d073-approved-scope-20261112-v1 (#409 scope ruling) | GitHub #409 CLOSED HANDED-OFF; result branch docs/d073-annual-review-spec-hp-20261112-v1 @ 6f9d3c90 (pinned base 0ce4793f); Manager GitHub active-tasks | None for the HP worker; Manager ledgers the row. Implementation continues under CCF-412-PREDATOR-FABLE-A-D073 |
```

### Row added: `CCF-412-PREDATOR-FABLE-A-D073`

```
| CCF-412-PREDATOR-FABLE-A-D073 | READY | Predator #412 Fable A parent orchestrator (READY / NOT RUNNING; no source worktree, production edit or Unity lease yet; delegated children Claude B and Codex under separate GitHub claims, one writer per file set) | T3 Code (Predator) / Claude Code | Claude Fable 5.2 parent; children recorded per claim | Claude A (parent, mandatory); Claude B and Codex for delegated children only | None | feature/scenario-one-d073-annual-review-v1-20261112 | TBD | D-073 read-only Annual Review year comparison. Proposed files only: Assets/ForestPrototype/UI/AnnualReviewView.cs; NEW Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs + .meta; NEW Tools/Verification/YearComparison/**; #406 helper ScenarioOneManagementFollowonReadout.cs + .meta byte-identical via conditional base integration/scenario-one-d073-cadence-readout-base-20261112-v1; handoffs under Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/ | Proposed writable worktree /media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1 (absent at registration). Proposed Unity qualification lane ResearchCopyQual391, conditional on fresh local preflight: lane clean, prior owner released, machine-wide unity-lock FREE (FREE and no Editor PID at registration). No lease held. Forbidden: WorkPlanView, UiRoot, objectives, MFM, save/event/hash/reference policy, main, personal #382, Windows/Player build | Yes | None | Tier 2 | Local preflight, then parent marks GitHub #412 RUNNING with real provider/worktree/branch/base HEAD and child issues before any production edit | Manager GitHub main active-tasks #412 assignment 2026-11-12 and binding packet on docs/manager-dual-fable-dispatch-20261112-v1; sources integration/scenario-one-cadence-v1-qualified-20261112-v1 @ e7591933 and qualify/scenario-one-management-followon-readout-20261112-v1 @ 0ce4793f; approved spec docs/manager-d073-approved-scope-20261112-v1 | Exact-base reconciliation T-01, then mapper T-03, AnnualReviewView wiring T-04, independent Tier-2 Unity qualification T-05 and review T-06; isolated integration only, no main merge, no Windows build |
```

### Row added: `CCF-413-HP-FABLE-B-D074-D076-SPEC-REMOTE`

```
| CCF-413-HP-FABLE-B-D074-D076-SPEC-REMOTE | READY | Remote HP #413 Fable B parent orchestrator (READY / NOT RUNNING; remote HP ProDesk Ubuntu WSL2; write start only under the Manager bounded pre-authorisation once this registration proof is published and matches GitHub coordination) | T3 Code (remote HP) / Claude Code | Claude Fable 5.2 parent; children recorded per claim | Claude A (parent, mandatory; same account as #412); Claude B and Codex for delegated children only; no Predator lease | None | docs/fable-hp-d074-d076-spec-programme-20261112-v1 | TBD | Remote HP docs-only D-074/D-076 spec programme. Eight files only: specs/scenario-one-spaced-major-intervention-learning-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md} and specs/scenario-one-multicycle-representative-rack-management-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}; plus separately published Markdown audits/handoffs under Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/ | Remote HP, docs-only, zero Unity lease, no Predator lane or worktree; isolated WSL worktree /home/jer/Development/CCF-HP-Worktrees/CCF-Fable-MultiCycle-Docs from docs/manager-d073-approved-scope-20261112-v1; no C#, Unity, main, active-tasks or Predator-owned UI source; file set disjoint from CCF-412-PREDATOR-FABLE-A-D073 | No | None | Tier 1 | Published registration proof (PREDATOR-412-413-DUAL-FABLE-PHYSICAL-BOARD-REGISTRATION-RESULT.md) read back and matching live GitHub coordination; otherwise HP stays read-only/BLOCKED | Manager GitHub main active-tasks #413 assignment 2026-11-12 and HP binding packet on docs/manager-dual-fable-dispatch-20261112-v1; #409 HANDED-OFF/CLOSED | HP parent verifies the published proof, creates the isolated docs worktree and marks GitHub #413 RUNNING; both specs stay DRAFT / MANAGER REVIEW REQUESTED; no gameplay code, no Unity, no merge |
```

## Native lint output (full)

Command: `source /media/jer/ZX20/CCF-Ops/environment.sh; /media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md`

Before the edit:

```
PASS: /dev/sdc2 rw; free=72752168960 bytes; minimum=16106127360 bytes; durable=/media/jer/Files/CCF
PASS: board schema and duplicate checks (22 live tasks)
exit=0
```

Candidate file, before the rename: `PASS: board schema and duplicate checks (24 live tasks)`, exit 0.

Live board after the rename:

```
PASS: /dev/sdc2 rw; free=72752168960 bytes; minimum=16106127360 bytes; durable=/media/jer/Files/CCF
PASS: board schema and duplicate checks (24 live tasks)
exit=0
```

## Preflight facts at registration

- GitHub `main` `a57f4c15e49d278e30df6f24b05768357fab14b1`; `active-tasks.md` names the same #412 branch and worktree, the same #413 branch, WSL worktree and two spec folders. #412 and #413 OPEN (last updated 12:43:55Z / 12:43:59Z); #409, #402, #408, #403 CLOSED; #382 OPEN, untouched.
- `unity-lock status`: `free`, no metadata, no Editor PIDs. No Unity process running.
- ZX20 free 72.75 GB against a 16.1 GB floor.
- Board mtime before the edit 12:17:54Z; no competing board writer seen.
- Absent before registration: branches `feature/scenario-one-d073-annual-review-v1-20261112`, `integration/scenario-one-d073-cadence-readout-base-20261112-v1`, `docs/fable-hp-d074-d076-spec-programme-20261112-v1` on origin, and worktree `/media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1`.
- Source refs: `integration/scenario-one-cadence-v1-qualified-20261112-v1` @ `e759193354b4613f88e3370a62ccaccf02c556cf`; `qualify/scenario-one-management-followon-readout-20261112-v1` @ `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4`; `docs/manager-d073-approved-scope-20261112-v1` @ `53dec8db19a9bdde20176b2f3abf508402c7f0af`.
- Lane `ResearchCopyQual391` (`/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`) is checked out at `dcb319f9` on `qualify/scenario-one-cadence-v1-after-harness-repair-20261112`; the board records it released by #402. Its cleanliness is re-checked before any lease.

## Conditions for the HP #413 bounded pre-authorisation

| Condition | Result |
|---|---|
| Both rows present and READY | PASS |
| Rows disjoint (lane None for both, different branches, different file sets, HP holds no Predator lane or Unity lease) | PASS |
| Native lint | PASS, 24 live tasks, exit 0 |
| Same file sets and branches as GitHub #413 and main coordination | PASS |
| No board owner conflict; no unrelated row edited | PASS |

All five pass at the time of publication. The HP parent must still re-check that live GitHub coordination matches this proof before it creates its docs worktree.

## Not done

No source worktree, no branch other than this evidence branch, no production edit, no Unity launch or lease, no build, no change to `main`, Manager-owned `active-tasks.md`, #382 or the dirty `CCF-Primary` checkout. This is a board registration only; it grants no acceptance.
