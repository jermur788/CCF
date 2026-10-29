# MFM-08A source-authority blocker — Manager record

**Claim:** #129  
**Exact code base inspected by worker:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`  
**Worker outcome:** STOPPED BEFORE CODE  
**Manager status:** **BLOCKED ON BROWSER SOURCE-AUTHORITY REVIEW #146**  
**Date:** 2026-10-29

## What happened

The #129 worker correctly stopped during the packet's mandatory source inspection. No production code was written, no Unity gate was run, no `HANDOFF.md` success handoff exists, and the success token was withheld.

The worker created/retained the intended clean lane:

- branch `feature/mfm-v1-coverage-world-adapter-v1`
- worktree `/media/jer/Videos/CCF-MFM-08A`
- exact base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`

The worker did not claim `active-tasks.md`, so no production ownership was established.

## Manager inspection of the exact base

`ForestStartingStand` already contains two serialized scenario settings with explicit semantic tooltips:

- `pathCorridorRadiusMeters = 1.5f`: **"No-tree corridor half-width around every Dirt Path segment."**
- `clearingRadiusMeters = 2.2f`: **"No-tree radius around the work-area clearing, the player start and every built structure."**

The starting-stand generator actually uses `pathCorridorRadiusMeters` for every `Dirt Path*` transform and `clearingRadiusMeters` for the `Forest Clearing`, the `ForestPlayer` transform at generation time, and every `ForestBuildable` transform. These are therefore existing authoritative scenario settings for the starting-stand clearance semantics; copying the literals or reflecting private fields would still be unacceptable, but exposing them read-only is materially different from inventing new geometry.

The worker's claimed need for an entirely new clearing-radius declaration is therefore **not accepted as stated**. The real issue is the lack of a read-only production API and the meaning of the player-start part of `Site` during later coverage reads.

## Genuine unresolved point

At stand generation time the code reads `ForestPlayer.transform.position`. Later MFM coverage runs after the player has moved. No durable/persisted player-start anchor has yet been demonstrated on the exact base.

The accepted pure coverage contract describes `Site` among ground the prescription must never treat, and its implementation comment groups player start with built structures. Using the *current* player transform would make coverage move as the player walks and would not represent a stable site. Inventing a fixed start coordinate is forbidden. Silently dropping the player-start part would change the accepted contract meaning without a recorded disposition.

Therefore #129 must remain stopped until browser claim #146 resolves this authority question and republishes the exact allowed source/API surface.

## Browser review questions

#146 must determine, from canonical spec/decision history and inspected code:

1. Whether the existing `ForestStartingStand.pathCorridorRadiusMeters` and `clearingRadiusMeters` are the authoritative runtime source values for MFM Track/Clearing/built-structure Site masks. The Manager's current evidence strongly supports **yes**, via read-only accessors rather than duplicated literals/reflection.
2. Whether `Forest Clearing` should use the existing `clearingRadiusMeters` clearance semantics for MFM coverage rather than visual `lossyScale` values. The Manager's current evidence strongly supports **yes** unless a stronger accepted source says otherwise.
3. What `Site` means for the player-start area after game start. Preferred safe interpretation to test against authority: the moving live player is **not** a coverage mask; if no durable start-site authority exists, either (a) record a bounded clarification that the player-start clearance is a starting-stand generation artifact and MFM `Site` covers durable built/site objects only, or (b) explicitly introduce an authoritative non-moving start-site source before #129 resumes. Do not use the live player position.
4. Whether `ForestStartingStand.cs` may be added to #129's owned surface solely for minimal read-only accessors/authoritative-source exposure, with no generation/calibration behaviour change.

## Restrictions

No production code resumes until #146 returns. Do not delete the clean worktree. Do not map only Headland and silently omit required masks. Do not change coverage tolerances, calibration, save/schema, MFM-08 UI, Work Plan, S1-C/S1-D, or Windows build state.
