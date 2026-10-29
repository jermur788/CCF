# MFM-08A Source-Authority Manager Review
**Label: BROWSER-CAPABLE**
**Claim #148**

## Purpose
Review the completed browser/GitHub source-authority work from claim #146 and issue an independent Manager verdict before MFM-08A implementation #129 resumes.

## Difficulty / model
- Difficulty: **3/5**.
- Preferred executor: OpenAI GPT-5.6 Sol browser/GitHub lane or equivalent strong browser reviewer.
- No local worktree, C#, Unity, save execution, or Windows build.

## Start gate
At task start refresh:
- claim #146 and its comments;
- the exact #146 `BROWSER-RESULT.md` / handoff and commits;
- claim #129 and its current task packet;
- live `Docs/Project/coordination/active-tasks.md`;
- accepted MFM spec/Decision Log authority relevant to coverage masks.

The user reports #146 completed, but when this handoff was published GitHub still showed #146 RUNNING. Do not infer the result. If the durable result is not yet published, remain blocked.

When the result is visible, mark #148 `[RUNNING]` with model/provider and starting `main` SHA. Register browser-only review ownership in live coordination if free.

## Review questions
Verify that #146 provides one coherent and evidence-backed answer to all of the following:
1. Authoritative runtime source for `Headland`, `Track`, `Clearing` and `Site` coverage masks.
2. Whether the existing `ForestStartingStand.pathCorridorRadiusMeters` and `clearingRadiusMeters` values are the authoritative scenario settings for the corresponding exclusions, and whether #129 may expose them through minimal read-only accessors.
3. Explicit disposition of the moving `ForestPlayer.transform.position` versus a durable player-start/site anchor. A moving player must not silently become a moving coverage exclusion.
4. Exact meaning of the `Site` mask after that disposition.
5. No invented coordinates, radii, visual-scale proxy or reflection access.
6. No coverage calibration/tolerance change, Work Plan semantic change, save/schema change or MFM-08 UI change.
7. Consistency with D-065: proposals are not Fell marks; Work Plan approval remains the commitment gate.

## Verdict
Issue exactly one verdict: **APPROVED / NEEDS CHANGES / REJECTED**.

If APPROVED:
- update #129's issue and canonical packet with the exact authorised source/API surface;
- state the exact continuation base/worktree/branch and any permitted `ForestStartingStand` accessor change;
- state whether every required mask source is now available;
- release #129 only if no source ambiguity remains;
- reconcile live coordination if ownership is free.

If NEEDS CHANGES or REJECTED, keep #129 blocked and publish the bounded correction required.

## Required result
Publish:
`Docs/Handoffs/MFM-08A-Source-Authority-Manager-Review-20261029/BROWSER-RESULT.md`

The result must include exact reviewed commits, verdict, authoritative mask-source table, player-start decision, #129 state, next executable action, user action required/none, and coordination status.

## Copy/paste dispatch line
`Run browser claim #148 from Docs/Handoffs/MFM-08A-Source-Authority-Manager-Review-20261029/BROWSER-HANDOFF.md. Start by refreshing #146, #129 and live active-tasks. Do not infer the #146 result: if its durable result is not published, remain blocked. Once visible, independently review the authoritative Track/Clearing/Site/player-start decisions and any permitted read-only ForestStartingStand accessor surface. Issue APPROVED / NEEDS CHANGES / REJECTED. If approved, republish and release the exact #129 Code contract; otherwise keep #129 blocked. Browser/GitHub only; no C#/Unity/save/schema or Windows build.`
