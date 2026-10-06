# Scenario 1 menu teaching (Part N)

Base: clearance correction `8c5380dd95b397281bedc9bd153a69086b9aad9f`. Context: `691dd18a56c0da21cb08909e22ac0d0625d556b1`. Branch: `task/scenario-one-menu-tutorial`. Separate worktree: `/home/jer/Documents/ChatGPT/Local Game Dev/CCF-menu-tutorial`. Main and the user smoke worktree are untouched.

The later [learning-objectives follow-up](Scenario1LearningObjectives.md) adds M for Map, X for Fell and O for a persistent, self-paced checklist. Its checks are recorded separately from the original Part N evidence below.

## Player flow

One contextual introduction appears when each screen first becomes relevant: HUD at start, Tree Inspection on first inspection, Map on first map visit, Work Plan on first planning visit, Annual Review only after actual annual results exist. Players may visit these in their own order; no opening dump of all five screens is imposed. Each explanation covers purpose, useful information, available decisions and return controls. Help/F1 revisits it (the HUD Help button uses Tree Inspection help while a tree is open); Escape dismisses only Help before returning to the underlying screen. Help releases the mouse and pauses forest interaction through the existing auxiliary-panel flag.

Tree help explains DBH, crown/light, competition, Crop Tree and Fell. It explicitly distinguishes cell ground light from crown light and gives no prescribed tree selection. Map help teaches layer → cell → information → waypoint → return → HUD direction/distance → direct inspection. The map remains diagnosis/navigation only, with no remote forestry operations.

Work Plan help distinguishes deciding in the forest from reviewing/approving execution and cost. It covers labour/materials, timber income, minimum contractor charge, available executors, nursery purchase and approval versus later execution. Annual Review help follows WORK DONE, MONEY and FOREST and asks what to inspect next.

## Review learning gate

Opening an empty Year 0 review cannot satisfy learning. After the first actual annual report, further year advances from the Work Plan are locked until the player explicitly acknowledges reading the annual results. The first advance opens the review automatically. The player may return to the forest to inspect outcomes before acknowledging; a clear Work Plan link returns to the unread results. Once acknowledged, further annual cycles unlock.

This is a UI learning gate, not a new simulation stage or biological rule. The supplied packet contains Part N only; the existing project has no separate later-stage tutorial architecture to extend. Direct simulation APIs, completion/reference harnesses and cost/biology rules keep their contract. The existing `annualReviewSeen` save field stores the acknowledgement, with no schema change. Already-acknowledged saves stay unlocked; saves with unread results retain the UI gate.

## Presentation preferences

First-use introductions are remembered per local player/device in five namespaced PlayerPrefs entries (`CCF.MenuHelp.v1.*`), outside forest save data. They do not enter world hashes or Reference state. Learning a menu avoids repeat popups across reloads and new forests, while Help remains available. No reset-all menu is added; this task does not create an options framework. These preferences are separate from the per-forest annual-results acknowledgement.

## Files and safety

New `UI/MenuHelpView.cs` and meta; existing UI root/HUD/modal views/theme; a bounded guard/hint update in ScenarioOneManager; disposable `Tools/Verification/MenuTutorialVerification.cs`. No serialized scene, prefab, calibration, dependency or save-schema changes. Local settings/material edits in the user's clearance smoke worktree are preserved. This task has its own Library and isolated verification save configuration.

## Verification

Final import/compile, menu playthrough, interaction, both completion models, save-hardening and Reference gates PASS. Results and selected images are in [the verification record](Verification/Scenario1MenuTeaching/README.md). An initial disposable fixture compile failed because it attempted a nonpublic Unity button API; a navigation-submit diagnostic did not trigger the unfocused button. Queued mouse down/up likewise did not reach the runtime panel in this Editor fixture. The final fixture invokes the production Clickable callback through reflection, and queues keyboard state events. This verifies callback behavior, not physical mouse delivery; manual clicking remains required. The failed attempt remains in ignored build evidence, and is not an acceptance pass. The disposable fixture demonstrates a beginner path through all five interfaces, queued F1/Escape input and production UI callbacks, waypoint creation without teleporting, first-report acknowledgement gate, both unread and acknowledged save/load states, one-time introductions and Reference isolation. It captures each introduction at 1280×720, 1600×900 and 1920×1080. Source inspection cannot certify beginner comprehension; a user playthrough remains the final readability check.

## Manual smoke

1. Open this worktree in Unity 6000.6.0f1, ForestTest, Play. A first-use HUD introduction explains status, objectives and ground reports. Close it; press F1 to revisit without changing the world.
2. Walk to any living tree and press E. Read the inspection introduction, then examine the actual fields and mark meanings. Close Help, then E to return.
3. Press M. Read Map help, choose a layer and cell, read its information, Set waypoint, then close with M/Escape. Follow the HUD direction/distance and inspect the site. No remote management or teleport occurs.
4. Press Tab. Read Work Plan help, review your jobs/executors/costs, approve chosen work and advance a year.
5. Close Annual Review help, read WORK DONE/MONEY/FOREST and explicitly acknowledge the results. Try returning to Work Plan before acknowledgement: further advances remain locked with a link to the review. After acknowledgement they unlock.
6. Save/load; acknowledgement remains. Reopening learned menus does not force introductions, but Help/F1 still works. Reference preview has no help overlay.

The task is implemented and verified on its local task branch; not pushed, merged or integration-approved. Human mouse input and beginner comprehension remain smoke-review checks.
