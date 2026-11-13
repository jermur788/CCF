# Direct user decision — Objective 1 F-1 player-facing rack numbering and correction release

**13 November 2026 ~08:35 UTC | USER DIRECT PRODUCT DECISION / BOUNDED F-1 SOURCE SCOPE EXCEPTION | G5 NATIVE RECHECK NOT YET RUN**

## 1. User intent and distinction

The user has explicitly chosen: **the bottom/southern extraction rack adjacent to the access pad is player-facing "Rack 1"**, the next rack north is Rack 2, and so forth through **top/northern "Rack 5"**. This is the initial teaching order for Scenario One. The player should never hear that the sprayed bottom Rack 1 is "rack 5" merely because it was created fifth.

The same conversation accepts **no underlying rack identity refactor** at this stage; rack numbering for player teaching is a display/navigation matter. Persistent ID, rack-to-entrance-tree linkage, frozen removal sets and order references remain unchanged. The user considers this navigation arrow a **guided teaching feature**, not a requirement that all future scenarios use an arrow or identical orientation. No policy for Scenario Two or later is ratified here.

This decision resolves the **product wording/numbering question only**, not a Unity safety test or Manager integration verdict. It supersedes the previous request to wait for a Manager policy choice about whether the message should omit a stored ID. It does **not** authorise arbitrary source/refactor changes.

## 2. Exact F-1 bounded implementation authority (user-directed)

**Existing writer and lane only:** #443 Objective 1, actual serial Sol 6.1 writer under #447 Fable orchestrator, branch `feature/scenario-one-objective1-ux-repair-20261112-v1`, worktree `/media/jer/ZX20/CCF-S1-Objective1UX-20261112`. Last user-reported candidate `324d6a19`, lane clean and Unity free; worker must re-check its actual branch/HEAD/dirty status, physical lease and live coordination, not rely on this GitHub-only document.

**Narrow allowlist addition only for F-1:** `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs`, **only the feedback copy produced by `MarkRackEntrance` and `ClearRackEntranceMark`**, currently around lines 419 and 433. Existing #443/G3 approved file list otherwise unchanged. The file was read directly on the #443 feature branch; current text interpolates `rackId` into feedback and thus leaks persisted identity as a player-facing number.

**Required observable behaviour:**
- When the player approaches/sprays **physical bottom Rack 1**, the HUD arrow, minimap marker, in-world target, action confirmation and any surfaced Work Plan status must not call it rack 5. The player-facing ordered five racks remain bottom→top **1, 2, 3, 4, 5** regardless of creation order.
- **Safest bounded implementation:** remove stored `rackId` from these two generic manager-generated feedback strings (for example: `Marked the entrance tree PO0203.` and `Cleared the entrance mark.`), keeping tree ID if useful. The existing presentation/UI owns the *correct* spatial ordinal and can continue showing "Rack 1". This resolves the F-1 contradiction without moving numbering into the authoritative model.
- **Alternative only if already supported by existing pure presentation helper with no extra source changes:** show the correct spatial display ordinal in those same two feedback messages. **Do not** invent/duplicate new ordering logic in the manager, change `RackSaveData.rackId`, renumber saved records, change `RackPlanOperationResult.RackId`, or touch geometry, tree membership, orders, `TrySprayRackEntrance`, save schema or execution logic merely to make feedback contain a numeral.
- Player-facing status must not regress in an edited/reordered/reversed-ID rack plan, including out-of-order sprays. If a true ordinal in feedback cannot be obtained inside this strict two-string scope, omit the misleading number; **do not** delay this narrow correction for a general rack-numbering refactor.

## 3. Checks and workflow gate

Independent [#449 review](https://github.com/jermur788/CCF/issues/449) returned **NEEDS CHANGES**: F-1 source feedback inconsistency, F-2 missing Plan Racks accent edge and F-3 stale Annual Review acknowledgement prompt. Orchestrator [#447](https://github.com/jermur788/CCF/issues/447) reports offline F-2/F-3+harness repair completed, but **not yet Unity-qualified**. Re-use that exact single writer and source worktree; do not create a second writer/Editor lease.

After F-1's two-string copy correction, complete the already planned **one targeted Unity pass at 1280×720 and 1920×1080**, including:
1. Visible initial yellow arrow, bottom Rack 1 and minimap target **before** first spray.
2. Bottom-first then top-second gives exactly **2/5**, distant top labelled Rack 5 and next target Rack 2.
3. Every marked/cleared feedback and Work Plan line matches the user-facing bottom-to-top meaning, **or contains no spurious stored rack number**; includes negative reversed-ID/edited-plan fixture.
4. Rack↔tree↔Pending order stable ID mapping, Legacy40/frozen Reference preservation, F-2 accent and F-3 post-acknowledgement instruction.
5. Independent **non-author F-1/F-2/F-3 delta source/evidence re-review**, then explicit **Manager #424 integration verdict**. Tests and final state may not be marked PASS before real evidence exists.

**Hard exclusions:** no larger refactor, no automatic renumber/persisted ID migration, no Objective 2, no UI/course redesign or Scenario Two implementation, no rack geometry shortening, no Windows build or automatic main merge, no editing `Docs/Project/coordination/active-tasks.md` by this documentation worker. #424 alone owns that live file.

## 4. Handoff / next action

- **Purpose:** resolve the F-1 product ambiguity so the #447 orchestrator can resume #443 without another Manager question on the same user decision.
- **Difficulty:** Low (two feedback strings, but High-consequence identity regressions require focused negative tests). **Best-suited author:** same registered OpenAI GPT-6.1 Sol local C#/Unity worker, with Claude Opus independent delta reviewer; no new task claim because this is an authorised narrow continuation of existing **[RUNNING] #443**.
- **Exact writer lane:** `feature/scenario-one-objective1-ux-repair-20261112-v1` / `/media/jer/ZX20/CCF-S1-Objective1UX-20261112`. **Orchestration:** #447. **This handoff lane:** GitHub docs-only `docs/objective1-f1-user-rack-number-disposition-20261113-v1`, no worktree/Unity/source edits.
- **Expected resulting code and test handoff:** existing `Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-OBJECTIVE1-UX-REPAIR-RESULT-20261112.md` as appropriately updated/successor on feature branch, linked on #443; orchestration final handoff on #447. Manager #424 records its formal integration verdict when evidence is ready.
- **Copy-paste dispatch:** "Resume existing CCF #443 F-1 under #447, keeping the same Sol single-writer branch/worktree and G3 exclusions. Binding direct-user display-number ruling: https://github.com/jermur788/CCF/blob/docs/objective1-f1-user-rack-number-disposition-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/USER-OBJECTIVE1-F1-RACK-NUMBER-DISPLAY-DECISION-20261113.md . Bottom/access-pad rack is always player-facing Rack 1, numbering physically upward to Rack 5; never alter persisted IDs/tree/order links. Authorised F-1 source scope is solely the Mark/Clear feedback strings in ScenarioOneManager.RackPlan.cs; omit stored rack IDs in generic feedback (or use only a pre-existing pure display ordinal, without adding model logic). Re-check host/lock, perform the one 720p+1080p targeted Unity pass with negative reversed-ID source and F-2/F-3, get independent delta review, then Manager #424 integration verdict. No Objective 2, save/geometry refactor, main merge or Windows build."
- **User action now:** none; this direct instruction supplies the product intent and bounded copy-only exception. **Dashboard/Drive sync:** unverified.
