# Learning objectives and map controls

Follow-up to the Part N menu teaching at `466456f7148498a611da56695401bf58da606a16`, on `task/scenario-one-menu-tutorial` in `/home/jer/Documents/ChatGPT/Local Game Dev/CCF-menu-tutorial`. Context remains `691dd18a56c0da21cb08909e22ac0d0625d556b1`. Main is unchanged. The user confirmed the current inspection layout is readable; the proposed inspection-layout fix was removed before this work.

## Controls

- **M:** open/close Stand Map. It does not mark trees.
- **X:** toggle a Fell proposal while aiming at a living tree.
- **C:** toggle a retained Crop Tree designation.
- **O:** open/close Learning objectives. The HUD also has Map and Objectives buttons.
- **Tab:** Work Plan. **E:** tree inspection/return. **F1:** contextual menu help.

Prompts, buttons, help text and the existing scenario guide use the new controls. X also replaces M in the older marking interface so its prompts match its input.

## Teaching and progression

The checklist has eight parent objectives and 42 sub-objectives, in a suggested simple-to-more-involved order: Stand Map (11 steps), inspection/Crop Trees, felling, pruning, fallen deadwood, planting/protection, vegetation clearance, and Work Plan/Annual Review. Each parent expands to explain purpose, controls, decisions and expected results. Only the first unfinished topic expands initially; other topics remain available. The HUD shows the next learning step separately from the forest's scenario-success objectives.

The map objective begins with opening the map, selecting a cell and reading Light. Next come setting a waypoint, returning to the forest, arriving from outside the destination cell and inspecting the site. Later steps cover Regeneration, Browsing/protection and Fell/crop marks, then an explicit acknowledgement of the map's limits. Setting a waypoint on the player's current cell cannot complete the travel step. The map remains diagnostic/navigation only.

Forestry lessons separate proposals, work planning, approval and successful annual results. Felling explains contractor minimum charges and material choices. Pruning explains the game's configured eligibility/recovery rules, batch selection, costs and later lift results. Deadwood distinguishes sale, construction stock and a retained fallen stem, then teaches annual reporting and a site visit. Planting covers ground observations, nursery stock, exact sites, executors, shelter costs and later outcomes. Clearance explains the preview's targets, standing-tree exclusion, costs, approval and a later site check.

Action steps update from actual UI actions or authoritative existing tree/order/event state. Reading steps use an explicit acknowledgement after their introductory prerequisite; displaying text does not prove comprehension. A failed job does not complete a successful-results step. Existing saved management history may satisfy prior practice when its plan or results are viewed.

There is no first-year deadline or learning-completion gate on forestry. Operations are optional practice for when they fit the player's management decisions; no specific tree is prescribed. The existing first Annual Review acknowledgement remains the only teaching gate on further UI year advances. No scenario success thresholds, simulation rules, costs, ecology, frozen Reference, or save schema change.

Learning progress uses `CCF.Learning.v1.<step>` PlayerPrefs, independently of forest saves and world hashes, like the existing device-local first-use menu preferences. It persists across sessions, years and forests on that device. It is deliberately a player's skills checklist, not a per-forest management target. It is not transferred with a forest save to another device. Reference preview does not record learning progress.

## Manual smoke

1. Open this worktree in Unity 6000.6.0f1, ForestTest, Play. M opens the map and never places a Fell mark. Return with M. Aim at a tree: X toggles Fell, C toggles Crop Tree; their labels explain the distinction.
2. Open Objectives with O or the HUD button. Read the expanded Map topic. Complete the first cell/layer tasks; progress changes without advancing a year. Collapse it and open another topic. Scroll to reach all explanations.
3. Set a waypoint in another cell. Close with M, follow the HUD distance and direction, arrive, then inspect the site. Reopen Objectives to check navigation progress. Try a waypoint on your current cell before practising travel: it must not credit travel.
4. Try later map layers in any year. Revisit explanations when needed. No first-year deadline or forced return popup occurs.
5. When appropriate, choose forestry work. Check that a pending plan, approval and successful annual result complete different sub-objectives. A failed result must not count as success. Read material/pruning/executor text before acknowledging those explanations.
6. Save/load or restart Play. Learning progress remains; the forest save and its existing review acknowledgement still behave normally. Check Reference preview does not add learning progress.

## Verification

Five final Unity checks PASS: the menu/learning playthrough (66.36 s), scenario lifecycle (42.57 s), model-1 completion (57.32 s), frozen Reference (69.19 s), and model-0 completion (57.33 s). Rendered checks cover 1280×720, 1600×900 and 1920×1080. Both completion hashes and the Reference continuation hash remain unchanged. [Results and selected images](Verification/Scenario1LearningObjectives/README.md) record the final runtime source hashes and verification markers. The disposable fixture backs up/restores all learning preference keys as well as the existing help preferences and isolated save files. As in the original Part N fixture, button callbacks are exercised directly; physical mouse delivery and beginner comprehension require the human smoke review. Navigation arrival uses a positioned test fixture, while M/O/X use queued keyboard input and map creation is checked not to teleport the player.

User-session editor settings and seven bilberry/forb material changes are preserved outside this change. No scenes, prefabs, packages or project settings are changed. This work stays on the local task branch; it is not merged or pushed.
