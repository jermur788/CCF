# P8 — Playtest fixes (template; one packet per playtest round)

```text
TASK PACKET
GOAL: Fix the comprehension failures observed in beginner playtest <n> and the
      MISLEADING items from the forester review, within copy/UI scope.
BRANCH: task/scenario-one-p8-playtest-<n>
```

**Start authority:** the playtest findings report (`BeginnerPlaytestProtocol.md` §9) and the forester review summary (`ForesterReviewProtocol.md` §5), triaged by the Manager into: copy, UI, decision, ecology. **Only the copy and UI items enter this packet.**

**Scope rule:** each misunderstanding code observed in ≥ 2 participants, and each MISLEADING item, gets one fix line in the packet:

```
FINDING: <code or item> · participants: n/6 · evidence: <timestamps/quotes>
FIX: <exact string / UI change>
FILES: <files>
CHECK: <string gate / rendered check / re-observation in the next playtest>
```

**Owned files:** `UI/*`, copy strings in `ForestTreeMarkingManager.cs`, `ClearancePreview.cs` and `VegetationClearance.cs`, as listed per fix.

**Locked:** manager, ecology, save, objectives, economy values (send those findings to the Manager as decisions).

**Exclusions:** new mechanics; new teaching systems; anything that changes anchors.

**Save impact:** none.

**Test plan:** string gates updated; rendered checks at three resolutions for changed screens; anchors unchanged.

**Manual review:** the playtest facilitator confirms each fix addresses the observed behaviour (not just the quote).

**Stop conditions:** a finding that needs a mechanic or decision → return to the Manager.

**Handoff:** IMPLEMENTATION HANDOFF with a finding → fix table.
