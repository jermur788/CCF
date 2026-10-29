# Scenario One — Final Independent Review Packet

**Parent:** #91  
**Depends on:** `FINAL-TIER3-HANDOFF.md` PASS on one exact candidate  
**Status:** PREPARED / BLOCKED  
**Purpose:** independently review the final Tier-3 candidate/evidence before the Manager prepares the user's personal-playtest handoff.

## Difficulty and routing

- **Difficulty:** High — final cross-system conformance and evidence-quality review.
- **Reviewer:** **OpenAI 6.1 Sol**, cross-provider from the preferred Claude Opus final Tier-3 executor.
- Reviewer must not be the dominant author of the final fixes or Tier-3 evidence.

If OpenAI 6.1 Sol is unavailable when the Tier-3 handoff is ready, this review waits. Do not silently replace it with the same-provider executor for convenience.

## Start gate

Start only after:

- exact final candidate SHA/ref is GitHub-resolvable;
- final Tier-3 handoff exists and carries the success token;
- all mandatory feature handoffs/reviews are published;
- any repair applied after Tier-3 evidence has triggered the required affected reruns.

Review is read-only. No production or integration edits.

## Review questions

1. Does every milestone requirement have evidence on the exact candidate?
2. Are IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED and ACCEPTED kept distinct?
3. Is any evidence being inherited from an older candidate where the final delta could affect it?
4. Was the S1-C T-05 save/history chain rerun on the final combined candidate?
5. Are current-schema, old-save, Legacy40, Reference Future, unknown-species and malformed-data/no-mutation claims supported?
6. Are protected pure-Sitka/reference/hash anchors unchanged or explicitly authorised?
7. Does the three-species result avoid fabricated empirical precision?
8. Does MFM preserve player judgement, separate proposals from Fell marks, and keep Work Plan approval as the commitment gate?
9. Does the complete loop demonstrably support second/later interventions, completion and solvency?
10. Do rendered/interaction results cover both target resolutions and the material 3D/map transitions?
11. Are any native-input/runtime/rendering gaps stated rather than hidden?
12. Are all remaining issues genuinely non-blocking for a personal user playtest?

## Required output

Publish:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW.md`

Verdict: `APPROVED`, `NEEDS CHANGES`, or `REJECTED`.

For `NEEDS CHANGES`, identify each failed requirement and the smallest bounded repair/rerun needed. Do not implement it under this review task.

For `APPROVED`, state the exact candidate SHA and explicitly distinguish:

- ready for **user personal playtest**;
- not yet user-accepted;
- no Windows build authorised.

## Manager next step

Only after `APPROVED` may the Manager prepare/update:

`Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-HANDOFF.md`

and emit:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

## Copy/paste dispatch line

`Review the exact final Scenario One Tier-3 candidate as OpenAI 6.1 Sol using Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-INDEPENDENT-REVIEW-TASK-PACKET.md and the published FINAL-TIER3-HANDOFF.md. This is read-only independent review: verify milestone/spec/save/reference/rendered/completion evidence on the exact candidate, including the mandatory final-candidate S1-C T-05 rerun. Publish FINAL-INDEPENDENT-REVIEW.md with APPROVED / NEEDS CHANGES / REJECTED. Do not edit production and do not authorise/build Windows.`

**User action required now:** none; blocked on final Tier-3 PASS.
