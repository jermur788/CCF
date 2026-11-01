# Scenario One Objectives-Window Overflow — Task Packet

**Claim:** #224  
**Parent:** #221 / #200  
**Local orchestration:** #203  
**Classification:** **PACKET ONLY — bounded UI/layout repair**  
**Difficulty:** Medium — narrow UI defect with rendered regression obligations.  
**Best-suited provider/model:** SOL/CODEX local implementation lane; independent review by Claude/DeepSeek.  
**Exact current production candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Suggested branch:** `fix/scenario-one-objectives-overflow-v1`  
**Worktree:** dedicated clean local worktree assigned by #203.  
**User action required:** none.

## Problem

The independently reproduced guided-v2 gate fails `current objective fits` at both 1280×720 and 1920×1080 on the current lineage. The failure predates #199, so #199 did not cause it, but it remains a real player-facing defect for the integrated user-playtest candidate until repaired or disproved by a fresh exact-candidate reproduction.

## Required sequence

1. Fresh-preflight live ownership, exact branch/HEAD, clean worktree, Editor state and machine-wide Unity lock.
2. Reproduce the objective-fit failure on exact `3acdf9cb...` before production edits.
3. Inspect the rendered failure and authoritative objective content. Preserve accepted objective semantics/copy.
4. Apply the smallest robust layout solution. Reflow/scroll/container sizing is preferred over hiding or truncating required objective content.
5. Rerun guided-v2 at both target resolutions.
6. If overlapping UI surfaces are touched, rerun proportionate Combined HUD and PARITY-07 rendered regressions. If Rack/minimap surfaces are touched, rerun the relevant Rack confirmation/minimap gate.
7. Independent review of the production diff and rendered evidence.

## Hard constraints

- Do **not** weaken, delete or skip `current objective fits` or equivalent rendered assertions merely to obtain green.
- Do not shorten/remove accepted objective meaning without Manager authority.
- No Rack/MFM workflow semantic changes.
- No save/schema, protected anchor/reference, ecology/economy/calibration changes.
- Preserve the accepted #199 estimate/minimap/focus contracts and MFM-08/09 current UI behavior.
- No Windows build.
- #194 NO ACTION.

## Stop conditions

Stop for Manager disposition if the only plausible fix requires a product-content change, a new navigation model, or a substantial UX redesign rather than bounded layout/reflow.

## Handoff

Publish:

`Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/HANDOFF.md`

Include exact base/final commit, production diff, before/after screenshots and gate counts, any visual trade-off, independent review verdict, lock/cleanup state, and whether the production commit is ready for integration.

Success token:

`SCENARIO ONE OBJECTIVES OVERFLOW REPAIRED — READY FOR MANAGER REVIEW`

Copy/paste dispatch:

`Run claim #224 from Docs/Handoffs/ScenarioOne-Objectives-Overflow-Repair/TASK-PACKET.md on exact 3acdf9cb635cdca79606335801e97f9d7dcce8b0. First reproduce the guided-v2 current-objective-fit overflow at 1280x720 and 1920x1080, then make the smallest production layout/reflow repair without weakening the assertion or changing objective semantics. Rerun guided-v2 plus proportionate Combined/PARITY-07 regressions, obtain independent review, and return the handoff. No Windows build; #194 NO ACTION.`
