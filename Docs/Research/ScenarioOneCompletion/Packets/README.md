# Bounded implementation packets

**Status:** PROPOSED (AgentWorkflow state). None is authorised. The Manager issues each one with a fresh BASE, after the previous packet's integration.

| Packet | Title | Order | Depends on | Parallel with |
|---|---|---|---|---|
| `P1-INT` | Integrate pedagogy P0+P1 copy onto main | 0 | — | Sol M2 |
| `P2` | Crop Tree competitor reasoning + forecast panel | 1 | P1-INT | Sol M2 |
| `P3` | "What you are leaving" + cash dead-end warning + terminology fixes | 2 | P2 | Sol M2 |
| `H1` | Beginner playtest 1 + forester review (human; no packet file — run `../BeginnerPlaytestProtocol.md` and `../ForesterReviewProtocol.md`) | 3 | P3, asset packets 01/07 | P4 development |
| `P4` | Annual Review v2 + Forest Diary + map history | 4 | P3 | Sol Storms W1/W2 |
| `P5` | Progression restructure Tier A + Model 2 teaching copy | 5 | P4; M2 integration for the M2 part; J3 decision | — |
| `P7` | Plan comparison (marteloscope B-lite) | 6 (optional) | P3; Q1 decision | — |
| `W5` | Storm UI and teaching | 7 | Sol Storms W1–W4; PD-K1 | — |
| `P6` | Second intervention + completion + failure/recovery | 8 | Storms integrated; N1/L2/L3/O1 decisions | — |
| `P8` | Playtest fixes (template, per round) | after each H | H1/H2 findings | — |

Common header for every packet (AgentWorkflow standard task packet):

```text
PROJECT: CCF
MANAGER: <designated Overall Manager>
BASE: <origin/main SHA at issue time>     (this study: a8596df9c52669a36709f09e85dfe6568640af49)
CONTEXT COMMIT: <full context SHA>        (this study: a8596df9c52669a36709f09e85dfe6568640af49)
WORKTREE: <dedicated worktree; never /home/jer/CCF-main>
ROLE: Primary
```

Common rules (all packets):

- AGENTS.md technical rules apply: Unity 6000.6.0f1, the New Input System, URP, no package changes, preserve `.meta`, no scene/prefab saves unless stated.
- Disposable harnesses are copied from `Tools/Verification/` into `Assets/ForestPrototype/`, run in the mode recorded in `Docs/Verification/ScenarioOneGateModes.md`, then removed with their `.meta`.
- Presentation packets must leave **all** anchors unchanged. Record them in the handoff: growth-1 lifecycle neutral `7E57B9DAEF5BF4D0`, normal `35E2BF1C2F55C2DE`, completion `D7C4DDD36B53FCCE`; legacy `BFC55473C1506067`; Reference Y100 `7AD177B3CC2F73C7`. **Add M2/storm anchors once integrated.**
- Handoff format: AgentWorkflow **IMPLEMENTATION HANDOFF**, or **CLOUD HANDOFF — NOT UNITY-VERIFIED** when Unity was not run.
