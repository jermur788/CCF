# CCF Project Decision Log

## Purpose

Record accepted decisions, current directions, proposals, deferrals and open questions. Suggestions from a chat, research report or agent must not silently become requirements.

Implementation facts belong in the Unity Project Overview. Current scope belongs in the Current Milestone; temporary task ownership belongs in live coordination.

## Status labels

- **Confirmed** — explicitly accepted by the user.
- **Current direction** — guides planning but may still change.
- **Proposed** — suggested but not accepted.
- **Deferred** — intentionally postponed.
- **Rejected** — considered and declined.
- **Open** — a decision is still required.
- **Superseded** — retained for history but replaced by a later decision.

## Acceptance audit — 2026-10-02

The Overall Manager reviewed the available conversation history before Revision 5. D-001–D-021 reflect accepted project decisions; D-022–D-024 are current directions; D-025–D-030 reflect accepted multi-agent direction; D-031–D-034 remain open. D-035–D-037 and D-040 were explicitly confirmed on 2026-10-02. D-038 preserves the accepted Valheim reference omitted from the first refreshed brief. D-039 was opened concerning pre-existing unmerged technical work and was resolved on 2026-10-02 (stack integrated and verified).

Historical dates remain broad where exact dates were not reconstructed.

The accepted canonical-context sync packet on 2026-10-02 updates D-007, D-009 and D-010 and confirms D-041: simulation-first progression, separate worker/execution choices, and a genuinely bounded Scenario One property. The earlier three-stage roadmap is retained below as superseded history.

The user explicitly accepted the Reference Future v1 contract on 2026-10-02; it is recorded as D-042 after the combined ecology package was integrated and verified on main.

On 2026-10-04 the user's interactive Scenario 1 smoke passed and the Scenario 1 integration was merged to main and verified. The final-integration packet records this as D-043 (functionally complete; final asset/presentation acceptance pending) and the Scenario 1 deferral list as D-044. The D-023 notes are updated because browsing is now integrated.

## Decisions

| ID | Date | Topic | Status | Decision or position | Implication / notes |
|---|---|---|---|---|---|
| D-001 | 2026 | Engine | Confirmed | Use Unity. | Current project is implemented in Unity. |
| D-002 | 2026 | Core concept | Confirmed | Build a 3D sustainable forestry / land-management game with physical presence. | Forestry is the first reference domain; long-term scope may broaden. |
| D-003 | 2026 | Forestry approach | Confirmed | Continuous-cover forestry is central to Scenario One. | Maintain cover while creating regeneration, structural diversity and timber outcomes. |
| D-004 | 2026 | Player experience | Confirmed | **decide → advance time → walk landscape → observe/experience consequences → understand why → decide again**. | Consequences must be visible in 3D, not only reports. |
| D-005 | 2026 | Reference scenario | Confirmed | Irish Sitka plantation conversion toward continuous-cover forestry. | Ireland is reference content, not an engine assumption. |
| D-006 | 2026 | Scenario sequencing | Confirmed | Complete Scenario One before broad scenario/biome generalisation. | Avoid premature regional abstraction. |
| D-007 | 2026-10-02 | Gameplay stages | Confirmed | Simulation-first progression: Stage 1 Forestry Management Simulation; Stage 2 Expanded Land-Management Simulation; Stage 3 Optional Manual / Hybrid Solo; Stage 4 Physical Cooperative World. | Broaden simulation/menu-resolved land management before introducing optional manual execution; cooperation is later. Supersedes H-004. |
| D-008 | 2026 | Stage 1 time | Confirmed | Discrete annual ecological steps. | Seasons/finer time may follow for physical survival play. |
| D-009 | 2026-10-02 | Work architecture | Confirmed | decision → task → labour/material/tool requirements → execution method → authoritative world result. | Contractor versus landowner is separate from simulated versus manual execution. Contractor simulation, landowner simulation, optional manual solo and later cooperative manual share authoritative ecology/construction results. |
| D-010 | 2026-10-02 | Work Plan | Confirmed | Review, cost, choose contractor or landowner, approve and resolve work through simulation; do not spatially design the forest. | Spatial decisions are made while walking. Landowner work can be menu-simulated; manual physical execution is not required in Stage 1. |
| D-011 | 2026 | Progression | Confirmed | No XP ladder as main progression. | Capability, costs, logistics, tools, processing and choices drive progression. |
| D-012 | 2026 | Tools/crafting | Confirmed | Recognisable tools; no primitive stone-age crafting ladder. | Keep survival/construction plausible. |
| D-013 | 2026 | Tree marks | Confirmed | Red Fell and Blue Crop Tree/future timber are mutually exclusive. | Persistence details belong in Overview. |
| D-014 | 2026 | Pruning | Confirmed | Designate Crop Trees; generate work for eligible trees rather than marking individual cuts. | Eligibility remains authoritative. |
| D-015 | 2026 | Planting | Confirmed | Purchased stock placed at the actual ground position chosen by the player. | Implementation/persistence belongs in Overview. |
| D-016 | 2026 | Planting clearance | Confirmed | Approximately 1 m² local clearance/vegetation-treatment area. | Research recommends spot vegetation control; later causal interpretation needs acceptance/implementation. |
| D-017 | 2026 | Felling outcomes | Confirmed | Sell/extract, retain fallen deadwood, or keep useful construction timber. | Inventory/accounting belongs in Overview. |
| D-018 | 2026 | Reference Future | Confirmed | V1 is a frozen authored v12 reference, not an optimum/forecast. | Do not silently rewrite archive/hashes. |
| D-019 | 2026 | Reference compatibility | Confirmed | Newer code may load/preview frozen v12 while current Scenario One has later schemas. | Distinguish historical archive from current behaviour. |
| D-020 | 2026 | Presentation authority | Confirmed | Habitat/art/audio derives from authoritative state; do not invent causal states. | Ring-barking/windthrow/defect consequences require matching simulation state. |
| D-021 | 2026 | UI | Confirmed | Add choices inside interfaces rather than permanent shortcut accumulation. | Prefer hotbar/tool/context patterns. |
| D-022 | 2026 | Multiplayer | Current direction | Single-player first; cooperation later. | Preserve reasonable boundaries without speculative networking. |
| D-023 | 2026 | Browsing | Current direction | Recruitment bottleneck through species/stage damage and suppressed height; shared response for cohorts and exact juveniles. | Browsing & Protection v1 integrated on main 2026-10-04 (Scenario One background pressure 0.2, shelters). Browse history/form damage and fencing gameplay are not implemented (D-044). |
| D-024 | 2026 | Understorey | Current direction | Investigate causal bramble, bracken and graminoid cover on the ecology grid, competition and bramble browse concealment. | Exact parameters need acceptance. |
| D-025 | 2026-10-02 | Multi-agent | Confirmed | Dynamic OpenAI/Anthropic workers/reviewers; no permanent disciplinary ownership. | Assign by capability, tools, context, availability and quality. |
| D-026 | 2026-10-02 | Overall Manager | Confirmed | Currently ChatGPT Game Dev project; user may reassign. | Maintains direction, boundaries and handoff review. |
| D-027 | 2026-10-02 | Repository truth | Confirmed | Git canonical; Drive one-way collaboration mirror. | Permanent mirrors carry source/commit/timestamp stamps. |
| D-028 | 2026-10-02 | Worktrees | Confirmed | Separate writable local worktrees; no concurrent writes to one Unity worktree. | Serialized assets default single-writer. |
| D-029 | 2026-10-02 | Capability | Confirmed | Environment/tool access, not model identity, defines capability. | Chat, Desktop/MCP, Code/cloud and OpenAI sessions may differ. |
| D-030 | 2026-10-02 | Verification language | Confirmed | Branch completion differs from integration/verification/acceptance. | Use shared integration states. |
| D-031 | 2026 | Open source | Open | Release/community regional variants under consideration. | Code/data/media licences need separate treatment. |
| D-032 | 2026 | Binary storage | Open | Decide large first-party FBX/PNG/Blender policy, including Git LFS. | No `.gitattributes` at baseline; no storage-policy change authorised. |
| D-033 | 2026 | Survival intensity | Open | Decide food/shelter/health/weather pressure. | Support the management loop. |
| D-034 | 2026 | Multiplayer timing | Open | Decide when cooperation becomes an active milestone. | Do not build networking prematurely. |
| D-035 | 2026-10-02 | Context location | Confirmed | Canonical context in `Docs/Project/`; root `AGENTS.md`/`CLAUDE.md`. | Instruction sources in `instructions/`; research index in project docs. |
| D-036 | 2026-10-02 | VS Code | Confirmed | Keep tracked shared `.vscode/settings.json`; local differences use user settings/external `.code-workspace`. | No routine dirty worktree-specific settings. |
| D-037 | 2026-10-02 | Legacy instructions | Confirmed | Retire root `AI_Instructions` after porting valid rules. | Do not let old prototype guidance compete with current direction. |
| D-038 | 2026 | Creative reference | Confirmed | Valheim informs exploration, gathering, building, progression and cooperation, not a specification. | Preserve useful experience without incompatible mechanics. |
| D-039 | 2026-10-02 | Technical stack | Confirmed | Resolved: integrate. The save/RNG/batch/edge stack is in `main` (merge `8ae7a25`) and was verified together with the combined ecology package at `b1e6c51`. | Was Open (review/integrate, revise or park; frozen during setup). Independent review still required for future save/determinism/reference-sensitive work. |
| D-040 | 2026-10-02 | Context generation | Confirmed | One full Git SHA snapshots the complete context/instruction-source set; deterministic committed-source composites/mirrors; live locks excluded. | No dirty content under an old SHA; lock changes do not require reattachment. |
| D-041 | 2026-10-02 | Scenario One property | Confirmed | Scenario One uses a genuinely bounded property. | Retains the Irish forestry reference scenario. |
| D-042 | 2026-10-02 | Reference Future v1 contract | Confirmed | Reference Future v1 is a frozen, immutable historical archive. Verification covers archive integrity against original embedded historical data, load/preview of frozen milestones, and deterministic continuation of historical saves using the current ecology. Exact reproduction of historical biology by later ecology versions is not required, and no versioned historical ecology engine is retained. | Integrity must not be tested by reserializing historical saves through current save classes. Continued biology may diverge diagnostically. A reference for a new model would be a new version, not an edit to v1. Detail: `Docs/ReferenceFutureContract.md`. |
| D-043 | 2026-10-04 | Scenario 1 status | Confirmed | Scenario 1 is **functionally complete**: integrated on main at `dc975e4` with save v15, browsing default 0.2, shelters, contractor/landowner execution, timber yield/economy, completion verification, and the Plantation02 Sitka, forest-floor and compact-brash presentation. Automated completion gate, 27/27 integration regression, post-merge gates and the user's interactive smoke all passed. | Final asset/presentation acceptance is still pending (next: Scenario 1 Final Asset Acceptance Review). Astra ForestFloorV1 is an external candidate delivery, not automatically adopted. Anchors: neutral `BFC55473C1506067`, Scenario One `3485B6630C9EA448`, completion `568922E1A6D73CDD`, Reference Y100 `7AD177B3CC2F73C7`. |
| D-044 | 2026-10-04 | Scenario 1 deferrals | Deferred | Outside the Scenario 1 minimum unless accepted elsewhere: deer fencing production feature, production understorey ecology, vegetation-control simulation expansion, storms/windthrow, adult suppression mortality, browse-history/form-damage persistence, fence deterioration, pruning-quality premium, broadleaf market, manual forestry, co-op. | Each needs its own accepted packet. The v16 browse-history/form-damage document is a proposal only. |
| D-045 | 2026-10-04 | Scenario 1 presentation | Confirmed | Final presentation acceptance PASS; the presentation baseline is integrated on main at `ec938b0` (ten-state rendered review, track texture, HUD readability). ForestFloorV1 is not adopted. | Simulation follow-ups stay open, including the RNG model policy for new games (finding on unmerged `task/post-scenario1-systems-readiness` @ `7f58618`). |

## Superseded early directions

| ID | Topic | Status | Earlier position | Superseded by |
|---|---|---|---|---|
| H-001 | First prototype | Superseded | Small woodland, one tree type, harvesting/construction/regeneration/save-load. | Implemented project progressed beyond it; current scope is Milestone. |
| H-002 | Time progression | Superseded | Continuous acceleration/seasons/discrete time undecided. | D-008 annual Stage 1; later stages remain open. |
| H-003 | Forestry economics | Superseded in part | Budgets/timber prices undecided. | Scenario One contains economy; calibration remains adjustable. |
| H-004 | Gameplay stages | Superseded | Stage 1 Management Simulation; Stage 2 Hybrid Solo; Stage 3 physical cooperative world. | D-007 simulation-first four-stage roadmap, with expanded land-management simulation before optional manual execution. |

## New decision template

### D-XXX — Decision title

- **Date:**
- **Status:**
- **Decision:**
- **Reason:**
- **Implications:**
- **Supersedes:**
