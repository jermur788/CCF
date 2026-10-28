# RACK-V2-08 — Remove Competing First-Person Rack Authoring

**Task ID:** RACK-V2-08  
**Mode:** bounded implementation + focused qualification  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (APPROVED)  
**Prerequisite:** RACK-V2-07 APPROVED / INTEGRATED through PR #48 / merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`; Manager review `Docs/Handoffs/RACK-V2-07/MANAGER-REVIEW.md`  
**Minimum base:** current `main` must descend from `0cbd59ab392f2a75e6af823d45411f5c18b8bbf9`.

## Goal

Make the Stand Map the only authoring surface for Rack V2 geometry in Enlarged80 Objective 1 by removing/disabling every competing first-person rack-creation entry point while preserving ordinary individual-tree marking and the world interactions still required by later Objective 1 steps.

Success exit:

`NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`

## Required player-facing result

For Enlarged80 / Rack V2 Objective 1:

- no first-person key, prompt, hint, panel or interaction may create a new rack or increment rack-plan progress;
- the lower HUD/help/control copy must no longer advertise the old `[B] Create rack` authoring route for the V2 path;
- attempting any former first-person rack-authoring route must produce no rack and no rack count change;
- the Stand Map `Plan Racks` workflow remains the only rack geometry author;
- ordinary individual Fell/Red and Crop Tree/Blue marking remains functional;
- existing world interaction needed for later entrance-tree marking is preserved rather than broadly disabling forestry interaction.

Legacy40 compatibility remains intact: do not remove a legacy route that is still required for the Legacy40 viable progression path unless the existing implementation already cleanly scopes that route away from Legacy40.

## Scope

Inspect and change only the narrow first-person rack-authoring surfaces required to satisfy the approved spec, expected to include as applicable:

- rack-authoring key/input routing;
- HUD lower-hint/control text;
- first-person rack creation panel/prompt;
- Objective 1 first-person rack authoring handlers;
- narrow curriculum copy/availability guards directly tied to that authoring path;
- focused verification under `Tools/Verification/RackV2NoCompetingAuthor/` (or equivalent bounded location).

Reuse the authoritative Rack V2 plan and current Objective 1 state. Do not create replacement rack state.

## Explicit exclusions

- No RACK-V2-09 orange field-marking implementation.
- No RACK-V2-10 Help expansion beyond removing obsolete first-person rack-authoring instructions required by this task.
- No RACK-V2-11 save/schema work.
- No MFM implementation.
- No spacing/confirmation/minimap redesign.
- No species/ecology/economy changes.
- No whole Objective-1 qualification.
- No Windows/Player build.

## Authority note

The accepted OPEN-3 rule is already implemented/qualified through RACK-V2-05 and RACK-V2-07. Older D-064/spec text may still contain stale wording saying OPEN-3 is unresolved; this task does not depend on OPEN-3 and must not stop on that stale text. Do not edit Decision Log/spec files in this implementation task.

## Verification

### Static / offline

Prove at minimum:

- all discoverable Enlarged80/V2 first-person rack-authoring entry points are disabled or removed;
- no V2 HUD/control/help text still advertises the obsolete rack-authoring key/path;
- no remaining first-person code path can add to the authoritative `RackPlan` or change `PlannedRackCount` for V2;
- Stand Map authoring remains intact;
- Legacy40 compatibility guards remain intact;
- ordinary Fell and Crop Tree marking paths are not disabled;
- runtime source compile succeeds;
- protected RACK-V2-05/06/07 behavior is not rebaselined.

### Unity / focused runtime

Under the machine-wide Unity lock on the exact candidate:

1. In a fresh Enlarged80 Scenario One Objective 1 state, attempt every former first-person rack creation route (key, prompt/panel, interaction handler as applicable). Confirm no rack is created and the rack-plan count is unchanged.
2. Confirm Stand Map `Plan Racks` can still create/edit racks normally.
3. Confirm the obsolete V2 `[B] Create rack` hint/control copy is absent.
4. Confirm ordinary individual Fell/Red and Crop Tree/Blue marking still work through their existing path.
5. Confirm RACK-V2-07 confirmation/minimap behavior remains green with a bounded preservation gate or unchanged protected gate.
6. Run a Legacy40 compatibility check sufficient to prove its viable progression path was not accidentally removed.

Use existing supported 1280×720 and/or 1920×1080 rendered evidence where UI copy is part of the assertion. This is functional qualification, not final appearance review.

## G3 / test integrity

Do not weaken or delete protected verification merely because the product behavior changed. If an existing assertion explicitly encodes the superseded first-person-authoring requirement, stop and flag it for Manager G3 review before changing that protected assertion. Prefer adding focused tests around the new absence contract.

## Coordination / ownership

Before production edits:

- refresh current `main`, claim and live `Docs/Project/coordination/active-tasks.md`;
- follow `Docs/Handoffs/RACK-V2-08/COORDINATION-AMENDMENT.md`;
- mark the task claim `[RUNNING]` with actual agent/model/provider, branch, worktree and starting HEAD;
- use one writable lane/worktree and one primary writer;
- preserve all unrelated dirty work.

Before Unity, refresh live coordination and acquire the machine-wide lock normally. MFM-03 #39 may run independently; never bypass an actual Unity owner.

## Routing / continuation

Primary: T3 Code / OpenAI Codex, strongest available local Unity-capable reasoning model. Prefer OpenAI capacity first rather than consuming Sonnet 5.5 subscription quota for routine implementation.

Watcher: OpenCode / DeepSeek v4.1 Flash from task start if available. The watcher may monitor quota/context/tool limits and recommend/coordinate a model continuation, but remains read-only unless formally reassigned as the single writer. A provider/model limit is not itself a blocker: preserve the same branch/worktree and one-writer contract, record the transition in the claim, and continue with another capable model before stalling.

## Stop conditions

Return to Manager with an explicit blocker if:

- removing the V2 first-person author route would also remove a Legacy40 route with no clean geometry-model guard;
- ordinary Fell/Crop marking cannot be preserved without a broader interaction redesign;
- a protected assertion materially contradicts the accepted RACK-V2-08 requirement and needs G3 approval;
- a conflicting live writer owns a required file;
- any protected Rack V2 behavior regresses.

## Handoff

Publish:

`Docs/Handoffs/RACK-V2-08/HANDOFF.md`

Include starting main, implementation commits, exact tested candidate, final tip, changed files, former route inventory, verification proving all V2 authoring routes are gone, Legacy40 result, Fell/Crop preservation, Unity lock evidence, limitations and any model/provider transitions.

At publication rename the task claim `[HANDED-OFF]`, link the handoff and close it.

## Success exit

`NO COMPETING RACK AUTHOR — READY FOR MANAGER REVIEW`
