# RACK-V2-10 Static Manager Review

**Handoff:** `Docs/Handoffs/RACK-V2-10-Static-Manager-Review-20261029/BROWSER-HANDOFF.md`  
**Label:** `BROWSER-CAPABLE`  
**Claim:** `#157`

## Purpose

Independently review the pushed static/offline RACK-V2-10 help/teaching candidate before any later rendered acceptance. This is a browser/GitHub-only review. It owns no production files and must not infer Unity/rendered qualification.

## Exact review target

- Implementation base: `6d3228c6ce7cbb1c3ec7b99339dd7a38c92a4c22`
- Exact production candidate: `db17d5df167d6d13cded0518da67f507a7f771ee`
- Published docs-only handoff successor: `6c07c21d55b53f92e053f2a83605e031227ba2a3`
- Worker handoff: `Docs/Handoffs/RACK-V2-10/HANDOFF.md`
- Implementation claim: `#94`

## Review requirements

1. Compare `6d3228c` to exact production candidate `db17d5df` and confirm the production scope is limited to RACK-V2-10 help/teaching.
2. Verify Tree Marks help exposes only the three currently used mark meanings: orange rack/machine-path removal, red Fell, blue Crop Tree/future timber.
3. Verify orange is framed as Scenario One's chosen convention/calibration and that the game's one-entrance-tree marking action is disclosed as a teaching simplification rather than standard universal practice.
4. Verify the 16–20 m wording remains conditional and stand-specific, and five racks remains Scenario One calibration. Reject `18 m ideal`, `always five racks`, or universal spacing claims.
5. Verify Objective 1 sequencing is: plan on Stand Map -> Confirm Rack Plan -> mark entrances in the forest -> review/approve in Work Plan -> advance year. Confirm stable lesson IDs and saved progress semantics are unchanged.
6. Inspect the `MenuTutorialVerification` change and confirm it only excludes the intentionally conditional Tree Marks screen from the existing normal-menu count; it must not weaken the five ordinary menu assertions.
7. Confirm no Rack geometry/marking/save/schema/S1-C/S1-D/MFM/Parity/Reference Future production change.
8. Review the reported offline evidence: `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97` and `RACK_V2_HELP_TEACHING_PASS static_and_offline=15`. Treat them as worker evidence, not rendered proof.
9. Issue exactly one verdict: `APPROVED — STATIC PHASE ONLY`, `NEEDS CHANGES`, or `REJECTED`.

## Output

Publish:

`Docs/Handoffs/RACK-V2-10-Static-Manager-Review-20261029/BROWSER-RESULT.md`

If approved, record that rendered/Unity acceptance remains owned by the later Rack qualification path and does not block this static review verdict from being useful.

## Boundaries

No C#/Unity, no production edits, no save/schema changes, no Windows build.

## Difficulty / routing

- Difficulty: **2/5** — bounded copy/UI static review with important wording-authority checks.
- Best-suited reviewer: **OpenAI GPT-5.6 Sol browser/GitHub lane** or equivalent strong browser reviewer, independent of the Claude Opus implementation lane.

## Dispatch line

`Run browser claim #157 from Docs/Handoffs/RACK-V2-10-Static-Manager-Review-20261029/BROWSER-HANDOFF.md. Pin exact production candidate db17d5df167d6d13cded0518da67f507a7f771ee and base 6d3228c6ce7cbb1c3ec7b99339dd7a38c92a4c22. Review only the RACK-V2-10 static/offline help-teaching implementation and evidence, especially orange/red/blue mark scope, Scenario-One-specific spacing/five-rack wording, first-tree simplification disclosure, Objective 1 ordering, stable lesson IDs and the MenuTutorialVerification narrowing. Publish BROWSER-RESULT.md with APPROVED — STATIC PHASE ONLY / NEEDS CHANGES / REJECTED. Browser/GitHub only; no C#/Unity or Windows build.`

**User action required:** dispatch the line above to a browser chat.
