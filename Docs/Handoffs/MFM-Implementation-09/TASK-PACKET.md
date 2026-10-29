# MFM-09 — Teaching Sequence / Work Plan Integration

**Parent:** #91  
**Spec:** `specs/multiscale-forestry-management-v1/` MFM-09  
**Status:** PREPARED / NOT DISPATCHED  
**Purpose:** insert the required `Extend your treatment` teaching step after existing Lesson 5 and explain prescription-generated pending work through the ordinary Work Plan without changing the commitment boundary.

## Difficulty and routing

- **Difficulty:** Medium–High — curriculum state and shared Work Plan presentation must consume two other feature APIs without regressing stable saved IDs.
- **Preferred worker:** Claude Sonnet 5.5 Medium / Anthropic.
- **Cross-provider review:** OpenAI 6.1 Sol for teaching-ID/approval-boundary review if the diff touches shared state logic rather than copy/layout only.

## Start gate / base

Do not start until:

1. MFM-07 proposal/approval boundary is accepted/integrated;
2. MFM-08 map/3D review UX is accepted/integrated;
3. S1-D T-04 guide monotonicity is accepted first on the shared teaching/current-state files;
4. S1-D T-05 previous-vs-proposed Work Plan presentation is accepted or its exact API/ownership is explicitly handed to this task;
5. exact current integration SHA is published and no overlapping teaching/Work Plan writer is active.

This ordering is recorded in `Docs/Handoffs/PlayerTestReadiness-Overnight-01/CROSS-FEATURE-OWNERSHIP-AND-SEQUENCING.md`.

Suggested branch/worktree: `feature/mfm-v1-teaching-workplan-v1` / `/media/jer/Videos/CCF-MFM-09`.

Create a child claim only when the gate is true.

## Required scope

- add a new stable teaching ID/IDs for `Extend your treatment` after existing Lesson 5;
- preserve every existing saved teaching step ID; visible numbering may change but persisted keys may not be renamed/reused;
- teach sample → derived prescription → coverage/candidate/proposal review → prescription approval → ordinary Work Plan;
- Work Plan clearly groups/explains prescription-generated pending ordinary work using the existing S1-D previous/pending presentation;
- preserve ordinary contractor/landowner/economy path;
- make prescription approval visibly distinct from Work Plan approval/commitment;
- preserve manual management alternative;
- use player-facing `apply your sample treatment to comparable areas` framing, not `operator selection` as formal Irish terminology.

## Exclusions / invariants

- no proposal state as Fell marks;
- no automatic Work Plan approval/execution;
- no hidden automation replacing player judgement;
- no renaming/reusing existing lesson IDs;
- no second Work Plan/history surface;
- no forestry calibration changes;
- no broadleaf quantitative treatment;
- preserve MFM v23 provenance, PARITY save compatibility, Legacy40 and Reference Future;
- no Windows build.

## Verification

- offline step-ID inventory before/after proves no rename/reuse;
- guide monotonicity/reload tests preserve completed earlier lessons;
- generated pending work is grouped/explained with prescription provenance while remaining pending until Work Plan approval;
- old saves with no MFM state do not invent a completed extension lesson;
- rendered teaching + Work Plan states at 1280×720 and 1920×1080;
- ordinary contractor/landowner/economy path regression passes.

## Handoff

Publish `Docs/Handoffs/MFM-Implementation-09/HANDOFF.md` with exact base/end SHA, step-ID table, API dependency mapping, rendered evidence and regressions.

Success token: `SCENARIO ONE EXTENSION FLOW QUALIFIED`

## Copy/paste dispatch line

`Run MFM-09 from Docs/Handoffs/MFM-Implementation-09/TASK-PACKET.md only after MFM-07/08 plus S1-D T-04/T-05 are accepted on one exact published integration candidate. Use Claude Sonnet 5.5 Medium by default. Add new stable Extend-your-treatment IDs after Lesson 5, preserve all existing saved IDs, explain prescription-generated pending work through the existing Work Plan grouping, and keep Work Plan approval as the commitment gate. Render at both target resolutions and publish the exact candidate/handoff. No Windows build.`

**User action required now:** none; dependency-gated, not dispatched.
