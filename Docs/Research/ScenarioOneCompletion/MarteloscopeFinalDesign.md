# Marteloscope — final design (Workstream Q)

**Status:** recommendation for a **PRODUCT DECISION**. Revisits `MarteloscopeDecision.md` (pedagogy branch), which recommended **(c) reusable practice mode**, in two steps: Year-0 copy, then a copy of the current forest.

## 1. The three options

| | A. Part of Scenario One | B. Optional practice inside Scenario One | C. Separate training mode |
|---|---|---|---|
| What it is | The real first intervention, with rich review | "Practise on a copy" of *your* forest from the Work Plan; return unchanged | A menu-level mode on a deterministic training stand |
| Safe to make mistakes | No | Yes | Yes |
| Long-term consequence | Only by playing on | Yes (on the copy) | Yes |
| Authored teaching cases | No | No: only what your forest happens to contain | **Yes**: cases placed on purpose |

## 2. Challenging "C, reusable practice mode"

The Manager's current direction is C. This study **mostly agrees**, but with three corrections.

**Correction 1 — practising on a copy of the real forest becomes an oracle.** The simulation is deterministic, and every new game uses one fixed seed (`simulationSeed = 20260914` [REPO]). A practice copy advanced 5–20 years therefore shows the player's **exact** future: every self-thinning death, every recruit, and, once storms exist, **every storm year and every tree that will blow down** (`StormTeaching.md` §2). Practising on your own forest and walking its future converts uncertainty into a lookup. That undermines the core loop *decide → advance → observe → understand* (D-004) and the storm lesson "one storm does not prove…". The earlier recommendation's step C2 ("practise on a copy of the current forest") should be **dropped or restricted**.

**Correction 2 — B without time is cheap and valuable.** *Comparing alternative mark sets on the real forest, without advancing time*, is the essence of a marteloscope exercise: mark, compare immediate consequences, choose. It is not an oracle: only the immediate, already-deterministic estimates (release, residual stand, pattern, cost) are shown. It reuses P2/P3 entirely.

**Correction 3 — the cases the packet asks for cannot all come from Scenario One.** Poor-form competitors, bramble patches, exposed trees and broadleaf seed sources do not exist in the Scenario One stand at Year 0 (`TrainingStandV2.md`). A training mode that teaches them needs an **authored** stand. That *is* option C.

## 3. Recommendation: B-lite now, C later

| Step | What | When | Save impact |
|---|---|---|---|
| **Q1 — Plan comparison (B-lite)** | In Scenario One's Work Plan: "Compare plans". The player stores up to 3 mark sets (A/B/C) by marking in the forest, switches between them, and sees P2/P3 summaries side by side. **No time advance. No copy.** Choosing a set restores those marks | After P3 | None (mark sets session-only); or a small per-forest save later |
| **Q2 — Training Stand mode (C)** | A separate mode on an authored deterministic stand; full loop with advance, walk, reset | After Model 2 and storms (cases depend on them), and after an authored-stand content packet | Isolated training save slot (or none: always starts fresh) |
| Rejected | Practice copy of the *real* forest with time advance | — | — |

## 4. The C training experience (30–60 minutes, deterministic)

Session structure: five exercises of 8–12 minutes. Each follows

**observe → mark → review consequences → execute → advance → walk the future result → reset → try another approach.**

| # | Exercise | Stand zone (`TrainingStandV2.md`) | Plans the player is invited to compare | What the review shows (no score) | Advance | Needs |
|---|---|---|---|---|---|---|
| 1 | **Choose and release** | Zone A: excellent Crop Tree, harmless suppressed neighbour, real competitor | Release the real competitor vs remove the small neighbour vs both | Crop Tree CI change; removals not near a Crop Tree; residual capital | 5 yrs: walk to the Crop Tree, compare DBH growth and crown | Now |
| 2 | **Same volume, different forest** | Zone B: uniform pole stand | Spread removal vs one group of equal volume | Pattern facts, light, release | 10 yrs: walk both; regeneration in the group gap | Now |
| 3 | **The big tree question** | Zone C: large competitor beside a Crop Tree; seed source | Remove the large competitor vs keep it (seed, capital) | Release vs capital vs seed retained | 10 yrs | Form flag for the "poor form" variant (decision) |
| 4 | **Young trees and the ground** | Zone D: regeneration patch, dense bramble/bracken, browse | Clear vs not clear vs shelter planting | M2 survival losses by cause; clearance targets | 5 yrs | **Model 2** |
| 5 | **Wind and edges** | Zone E: exposed slender tree, gap edge | Heavy edge opening vs light touch | Stability bands | Advance through an **authored training storm** (explicitly labelled as an exercise) | **Storms** |

Rules:

- **Deterministic:** fixed stand, fixed seed; Exercise 5's storm uses the harness-style forced event, *only* in training mode, and the UI says "Training storm". The same marks always give the same results.
- **Reset** restores the stand exactly (world-hash check, as the pedagogy harness did with `RESIDUAL_STAND_WORLD_RESTORED`).
- **No universal score.** After each attempt: "Attempt A / Attempt B" columns by dimension (`MultidimensionalFeedback.md`). An optional *practitioner commentary* per exercise explains why foresters often do X, in mechanism language, never "correct answer".
- **Isolation:** training never writes the Scenario One save, never touches objectives, learning progress or the Reference Future. F5 blocked.

## 5. Decisions required

1. Accept B-lite (Q1) in Scenario One, and C (Q2) as a later separate mode? (Recommended.)
2. Drop "practise on a copy of my forest with time advance"? (Recommended: drop; it is an oracle under a fixed seed.)
3. Allow an authored "training storm" in C only? (Recommended: yes, labelled.)
4. Commission an authored training stand (content packet, serialized assets)? Needed for C.
