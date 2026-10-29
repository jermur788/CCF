# Browser handoff — MFM-08A source-authority review

**Claim:** #146  
**Status:** PREPARED FOR BROWSER EXECUTION  
**Parent:** #91 / #129  
**Purpose:** resolve the exact authoritative runtime sources and semantics for MFM-08A infrastructure masks so #129 can resume without invented geometry or a silent contract change.  
**Difficulty:** **3/5** — browser/GitHub-only authority reconciliation; no local implementation, but the result changes which world geometry MFM coverage is allowed to trust.  
**Best-suited model/provider:** OpenAI GPT-5.6 Sol browser/GitHub lane or equivalent strong browser-capable reasoning model.  
**Lane:** canonical `main`; browser only; no local worktree; no Unity; no production C#.

## Read first

1. `Docs/Project/coordination/active-tasks.md` — refresh live ownership first.
2. #129 and its current packet: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/TASK-PACKET.md`.
3. Manager blocker record: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/SOURCE-AUTHORITY-BLOCKER-20261029.md`.
4. Approved MFM package: `specs/multiscale-forestry-management-v1/spec.md`, `plan.md`, `tasks.md`, checklist as needed.
5. Exact base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, especially:
   - `Assets/ForestPrototype/ForestStartingStand.cs`
   - `Assets/ForestPrototype/ScenarioOne/ScenarioOneCoverage.cs`
   - `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs`
   - `ScenarioOneHeadlandGeometry` and any authoritative path/stand-map/buildable geometry APIs.
6. Relevant MFM-02/03/04 handoffs, issues and calibration evidence where they define access/clearing exclusions.

## Known facts that must not be lost

On the exact base `ForestStartingStand` declares and uses:

- `pathCorridorRadiusMeters = 1.5f`, described as the no-tree corridor half-width around every Dirt Path segment;
- `clearingRadiusMeters = 2.2f`, described as the no-tree radius around the work-area clearing, player start and every built structure.

Its `CollectClearancePoints()` applies those real fields to Dirt Path transforms, `Forest Clearing`, the `ForestPlayer` transform at generation time, and `ForestBuildable` transforms. Treat these as evidence that the values already have starting-stand authority. The blocker is API/semantic exposure, not permission to copy literals or use reflection.

The unresolved point is the player-start site: later MFM coverage has no demonstrated durable start transform. The live `ForestPlayer` position moves and must not be treated as a fixed site merely because stand generation sampled it once.

## Required decision/review

Issue one durable browser verdict and record the binding source mapping for each accepted `ScenarioOneCoverage.MaskKind`:

- **Headland** — confirm existing public `ScenarioOneHeadlandGeometry` source and exact shape/radius semantics.
- **Track** — determine whether the real Dirt Path transforms plus read-only `ForestStartingStand` corridor-radius accessor are the authoritative runtime source. Do not accept duplicated `1.5f` literals or reflection.
- **Clearing** — resolve whether `Forest Clearing` transform plus `ForestStartingStand.clearingRadiusMeters` is the authoritative MFM exclusion. Explain why Stand Map visual scale is or is not relevant; do not silently substitute a visual radius for management semantics.
- **Site / built structures** — determine whether `ForestBuildable` transforms plus the same clearing-radius authority are valid.
- **Player-start portion of Site** — inspect canonical authority. If no durable non-moving start-site source exists, choose and record one of these dispositions:
  1. **Clarify contract:** player-position clearance is a starting-stand generation artifact, not a later MFM coverage mask; `Site` in runtime MFM means durable site/buildable geometry only. The moving player is never a mask. This is the Manager's preferred safe interpretation if consistent with accepted spec/history.
  2. **Require a durable start-site authority:** define the smallest authoritative non-moving source/API before #129 resumes. Do not invent a coordinate or persist a new field unless existing authority clearly requires it; if persistence/product scope is needed, return BLOCKED rather than expanding silently.

Also decide whether #129 may add **minimal read-only accessors** to `ForestStartingStand.cs`. If approved, constrain them to exposing already-existing authoritative settings/geometry; no generation behaviour, serialized values, calibration, scenes or prefab values may change.

## Browser outputs

Publish:

`Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-RESULT.md`

The result must include:

- exact canonical/spec/base evidence inspected;
- verdict **APPROVED / NEEDS CHANGES / BLOCKED**;
- a table: mask kind → authoritative source/API → geometry/radius semantics → code surface #129 may read/edit;
- explicit player-start disposition;
- whether a Decision Log/spec/plan/core-comment clarification was required and its exact commit;
- corrected `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/TASK-PACKET.md` if #129 is unblocked;
- #129 issue title/status/comment and copy/paste continuation line;
- live coordination update if ownership is free; if another browser task currently owns `active-tasks.md`, do not race it—post a reconciliation note instead;
- user action required or none.

## Restrictions

Browser/GitHub only. Do not implement the accessors or adapter. Do not change coverage tolerances/calibration, proposal/Work Plan semantics, MFM-08 UI, save/schema, S1-C/S1-D, scenes/prefabs or Reference Future. No Windows build.

## Copy/paste dispatch line

`Run browser claim #146 from Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-HANDOFF.md. Resolve the #129 source-authority STOP on exact base c8cfae8c: inspect the accepted MFM contract and actual ForestStartingStand/coverage APIs, decide authoritative Track/Clearing/Site sources, explicitly dispose the moving-player vs durable player-start ambiguity, and decide whether minimal read-only ForestStartingStand accessors are allowed. Browser/GitHub only; do not write C#/Unity. If approved, correct #129's packet and release its exact continuation. No Windows build.`
