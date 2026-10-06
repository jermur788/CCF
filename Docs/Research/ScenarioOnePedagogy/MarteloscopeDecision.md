# Decision paper 1 — Marteloscope training

**PRODUCT DECISION REQUIRED.** Detail: `MarteloscopeConcept.md`, `TrainingStandSpecification.md`, `LongTermTrainingFlow.md`.

## Question

Should marteloscope-style training be **(a)** part of Scenario One's opening, **(b)** a separate tutorial scenario, or **(c)** a reusable practice mode?

## Options

| | (a) Part of Scenario One | (b) Separate scenario | (c) Reusable practice mode |
|---|---|---|---|
| What the player does | Marks the real forest at Year 0; the review before approval is the "marteloscope" | Plays a dedicated training stand from a menu | From the Work Plan: "Practise on a copy" → mark plans A/B/C → compare → optionally simulate 5/20 years → return |
| Retry and compare | No | Yes | Yes |
| Long-term consequence | Only by playing on | Yes | Yes |
| Practice before *later* interventions | No | No (fixed stand) | **Yes** (copy of the current forest; step C2) |
| New content | none | a scenario definition, menu, possibly an authored stand | none for the Year-0 copy |
| Technical precedent | — | — | Reference Future preview (capture, load, block saving, restore) |
| Main risks | No safe place to make mistakes | Needs a scenario front end that does not exist; duplicates setup | Sandbox isolation (saves, objectives, learning); restore timing (proven requirement: JSON copy plus frame yield) |
| Owner overlap | UI only | Scene/scenario assets (high-conflict) | `ScenarioOneManager`, `ForestSaveController` (save blocking), UI |

## Recommendation

**(c) Reusable practice mode, in two steps:**

1. **C1:** practice on the deterministic Year-0 stand (fixed ids, so the training cases and error library work).
2. **C2:** practise on a copy of the current forest, so it is useful before every later intervention. This directly reinforces repeated management.

Keep (a): the real first intervention still gets the residual-stand review (Packet 3). Reject (b) for now.

## Conditions

- Must not write saves, change objectives, change learning progress (beyond "practice used") or touch Reference Future v1.
- **Sequence after Sol's growth-model/save integration**: it touches the save controller and the manager.
- No score (`DecisionMatrix.md` §2.5).

## What the user needs to decide

1. Accept (c)? Or prefer starting with only the residual review in the real forest (a)?
2. May practice plans be "carried back" as proposed marks into the real forest?
3. Which long-term horizons: 5 and 20 years (recommended), plus 50?
