# CCF Agent Entry Point

Before substantial work, read:

1. `Docs/Project/AgentWorkflow.md` — authority, context commit, worktrees, task roles, handoffs and integration.
2. `Docs/Project/decision-log.md` — accepted/current/open decisions.
3. `Docs/Project/current-milestone.md` — current scope.
4. `Docs/Project/game-brief.md` — long-term player/game vision.
5. `Docs/Project/unity-project-overview.md` — verified implementation facts and local verification guidance.
6. `Docs/Project/research-index.md` when ecology/research context is relevant.
7. `Docs/Project/coordination/active-tasks.md` at task start when live Git access is available.

AgentWorkflow owns source authority/conflict rules and multi-agent coordination.

## Repository and Unity rules

- Use only tools actually available in the current session.
- Inspect relevant existing files before editing.
- State the smallest reasonable change and the files you intend to touch.
- Explain material architecture, gameplay, rendering, save-data, package or project-structure trade-offs before changing them.
- Do not modify unrelated files. Check Git status and report unrelated changes before substantial work; preserve user work.
- Use Unity 6000.6.0f1 and the packages actually installed.
- Use the New Input System; do not introduce legacy `UnityEngine.Input` unless explicitly requested.
- Keep materials/shaders URP-compatible.
- Do not edit generated folders: `Library`, `Temp`, `Logs`, `obj`.
- Do not manually edit generated solution/project files without a specific reason.
- Preserve `.meta` files and GUID relationships.
- Do not delete assets unless explicitly authorised.
- Prefer built-in Unity features when they adequately solve the problem.
- Avoid unnecessary managers, singletons, service locators, frameworks and dependency injection.
- Write straightforward, descriptive C# suitable for a learner to maintain. Keep scripts focused on one responsibility where practical.
- Avoid premature optimisation. Comments should explain non-obvious reasons, not narrate every line.
- Do not silently rewrite a working system merely because another architecture looks cleaner.
- Do not add packages, Git LFS or external dependencies without approval.
- Do not alter frozen Scenario One Reference Future v1 unless explicitly authorised.
- `.vscode/settings.json` is tracked shared configuration; do not make routine local edits to it.
- If local VS Code settings differ, use user settings or an external `.code-workspace` outside the repo.
- Each Unity worktree has its own `Library`; do not copy/share one between worktrees.
- Build one small, playable improvement at a time. Do not implement a whole loop or speculative multiplayer infrastructure without task scope requiring it.

## Ecology and communication

Keep ecological simulation rules reasonably separate from graphics and controls where that clearly helps maintenance/testing. Do not invent ecological facts: flag assumptions needing research and distinguish established knowledge, simplified simulation rules and deliberate gameplay calibration. Planting trees is not automatically environmentally beneficial. Forestry should create visible consequences for regeneration, canopy, habitat and productivity.

Assume the user is learning Unity/C#. Explain unfamiliar concepts briefly, prefer one clear recommendation over unnecessary alternative architectures, and inspect available project information before asking for it. Distinguish verified facts, assumptions, proposals, actual changes and unresolved questions. Never claim completion without making and verifying the change.

## Verification

Compilation alone is not sufficient. After changing gameplay:

- run appropriate compiler/test/runtime gates;
- explain exactly what changed;
- give exact Unity test steps or commands;
- state expected observable behaviour;
- report anything not directly verified.

Important behavioural checks include movement/interaction, harvesting a tree only once, correct resource awards and construction consumption, regeneration, and persistent world changes after save/load.

For disposable scripts under `Tools/Verification/`, follow the documented copy/run/remove workflow and ensure no temporary script or generated `.meta` remains under `Assets/ForestPrototype/`.

### Serialized Unity assets

- Scenes, prefabs, ScriptableObjects, ProjectSettings and package/dependency files are high-conflict and single-writer by default.
- Avoid broad reserialization or opening/saving unrelated scenes/prefabs.
- Cloud/code-only workers should not hand-edit Unity YAML unless the task explicitly requires it and the diff can be reviewed.
- Never resolve a Unity YAML/serialized-file conflict mechanically without understanding both sides.
- Keep `.meta` files with their assets and preserve GUID relationships.

### Blender, binaries and asset production

- Do not assume Git LFS is configured; no LFS/storage-policy change is allowed without approval.
- Before adding large binary asset families, report expected footprint and preserve source/licensing provenance.
- Every substantial asset brief should specify biological/game state, visual purpose/reference, scale, variants, LOD, Unity requirements, biological boundary, performance target, source/licence and acceptance criteria.
- Asset generation is not completion. Completion is: **source asset → Unity import → material/prefab/LOD → representative scene → rendered player review → performance/behaviour check**.

### Verification harness warnings

- Inspect a harness before running it; do not assume identical side effects.
- The disposable integration harness is copied from `Tools/Verification/` into `Assets/ForestPrototype/`, run, then removed together with its generated `.meta`.
- It can touch `forest-save.json`; do not run it where the local save matters unless its backup/restore behaviour is acceptable.
- Auto-start verification scripts must never remain in a playable build.
- Deterministic/lifecycle hashes are comparable only under the same verified Editor/configuration conditions described by the harness documentation.

## Multi-agent

No model family permanently owns code, Unity, Blender, assets or testing. Follow the current task packet, context commit and temporary ownership from `Docs/Project/coordination/active-tasks.md` (or the lock state copied into the task packet).

Do not call task-branch work integrated or verified until required integration gates have passed. Permanent Google Drive context mirrors are read-only collaboration copies. Make project-context changes in Git and regenerate mirrors; do not edit permanent mirrors as the source of truth.

## Legacy instruction migration audit

The useful rules from root `AI_Instructions` are represented before its retirement:

| Legacy section | Current owner |
|---|---|
| Technical setup (Unity/URP/Input/scenes/controller/Linux/VS Code; inspect facts) | Unity Project Overview; repository rules above |
| Small playable improvements, simplest maintainable solution, staged scope, multiplayer restraint | Game Brief; shared instructions; repository rules |
| Pre-change inspection, smallest change/files, trade-offs, unrelated-work protection | Repository rules; AgentWorkflow pre-write checks |
| Unity version, URP/Input, generated files, serialized assets, metadata, deletion, packages, built-ins | Repository/serialized-asset rules |
| Clear learner-maintainable C#, focused scripts, simple design, optimisation/comments, no silent rewrites | Repository rules |
| Ecology separation, evidence/assumptions, planting/site suitability, visible consequences | Ecology section; shared instructions; Game Brief |
| Git status, preserve user work, actual behavioural verification and observable test steps | Repository/verification rules; AgentWorkflow |
| Plain language, concepts, focused recommendations, facts/proposals/unverified distinctions | Communication section; shared instructions |
| Early first-playable loop | Superseded history H-001 in Decision Log; current staged loop in Game Brief |

The early prototype direction is preserved as history rather than promoted over the accepted current milestone. Migration source: accepted Revision 5 Drive drafts; the task packet authorises this repository migration.
