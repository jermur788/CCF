# Manager #425 — Claude compact Scenario One refactor and sim-core strategy: planning-evidence disposition

**Date:** 13 November 2026. **Verdict: ACCEPT AS SUPPORTING TECHNICAL PLANNING EVIDENCE ONLY — NOT ADOPTED AS AN APPROVED SPEC OR PRODUCTION TASK.** Neither the user nor this document approves the suggested refactors, test infrastructure, engine changes or router architecture.

**Reviewed source:** [Claude Opus 5.5 combined technical recommendation](https://github.com/jermur788/CCF/blob/docs/compact-scenario-one-refactor-sim-core-strategy-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/CLAUDE-COMPACT-SCENARIO-ONE-REFACTOR-AND-SIM-CORE-STRATEGY-20261113.md). Independent docs-branch GitHub compare against `main` confirms **exactly one new Markdown file**, no production code or canonical authority change. The author describes read-only static inspection of `main` only; the current Objective 1 #443 branch, Unity runtime, offline builds and time-savings were not independently tested by this Manager review.

## Assessment

**Architectural framing is useful and proportionate for subsequent product approval consideration:**
- Keep Unity and the existing Sitka/Oak/Beech forestry/economy/history engine. No pre-demo engine migration, speculative generic eight-scenario platform, `ScenarioOneManager`/`ForestEcologyController` split, global assembly refactor, service locator or sweeping rewrite.
- If approved after the current gates, sequence minimal foundations **F-1 explicit known-geometry dispatch**, **F-2 one router/explicit save-profile identity check**, **F-3 disjoint configurable tutorial/help keys**. Refactor only the code already touched by those accepted tasks; share just enough with eventual O2 rather than duplicate Scenario One logic. Formal #436 versus compact router ownership remains **OPEN-C4 / D-4**. The save-owner must decide schema envelope, foreign-ID precedence before historic best-effort future-version handling, legacy hash preservation and PlayerPrefs key policy; no writer may choose these alone.
- Where already pure, prefer narrow plain-C# deterministic validation/geometry/quote routines behind existing Unity entry points. Preserve exact float semantics, `JsonUtility`, tested model ordering, all saved hashes and old five-rack behaviour. Source-level reuse is not proof of Unity parity.
- **T-0 proposed shared offline test project** can remove repeat setup overhead, but it is a **different proposed task**, not a prerequisite silently added to #460. The report's 188-file / 78 without `using UnityEngine` count is a *static search indicator*, not proof that all 78 are runtime/serializer-independent (five have fully qualified `UnityEngine.`; transitive Unity dependencies still need examination). No actual test runtime, CI speedup or hash preservation has been measured. Build a separate scoped cost/benefit case before treating T-0 as adopted or permanent.

## Reconcile with already-published C0-03/C0-04 study (#460)

The [binding #460 native-feasibility packet](MANAGER-425-459-NATIVE-FEASIBILITY-STUDY-PACKET-20261113.md) remains unchanged: **READY / NOT RUNNING**, no local worktree, save access, Unity process or exclusive machine lease granted. Genuine source-dependent **UnityEngine/MonoBehaviour** `ForestStartingStand` / scene/ecology claims still require a guarded disposable Unity check and **#424's exclusive Editor lease**. Plain `dotnet` studies or a disposable source-faithful stand-planner may contribute **MODEL DERIVATION / OFFLINE EVIDENCE**; without Unity candidate parity they must **not** be labelled direct Unity measurements, physically reachable gameplay or playtest. A test project under `Tools/Verification/` may not be committed under #460's exact **one Markdown report only** allowlist. No retroactive authorisation of T-0 or tool changes by attaching this research.

**No need to stall #460** while D-1–D-3 are debated: its worker can inspect existing offline test patterns, produce labelled source/static/cash evidence and, after #424's safety/lease signoff, run strictly scoped native candidate qualification. If authentic candidates cannot be built without structural production edits, STOP, document that dependency and return to Manager; **do not smuggle F-1/T-0 into research as unapproved production refactoring**.

**Future amended DRAFT (separate claimed documentation task):** incorporate #459 corrections E-1–E-7, actual C0-03/C0-04 data, and this Opus recommendation as a **proposed** staged architecture section. Consider whether a separate verification-only T-0 is proportionate *before* source approval and whether it needs its own approved packet/branch/claim and #424 file/lease check. Keep complete Sitka/Oak/Beech annual engine, own-world saves, S1-C/S1-D and genuine full loop mandatory. Run bounded independent delta-review of newly changed requirements before owner/save-owner decisions, explicit Manager spec approval and source release.

## Decisions held open — no approval by this review

| Suggested ID | Question | Current disposition |
| --- | --- | --- |
| **D-1** | Adopt minimal shared, plain-C#-on-touch architecture? | **PROPOSED**, to evaluate at amended-DRAFT approval. No production refactor approved. |
| **D-2** | Committed `Tools/Verification` shared `dotnet` project vs Unity test infrastructure? | **OPEN**. The first is a sensible bounded recommendation, but needs accepted project-structure scope and explicit owner/Manager permission. |
| **D-3** | Allow verification-only T-0 before compact spec approval? | **OPEN**, requires separate narrow claim, proof of production-file noninterference and fresh #424 lane check; #460 not broadened. |
| **D-4** | One save-router owner and final schema/slot policy? | **OPEN-C4**, Manager/save owner after evidence. No competing router writers. |

**Preservation:** genuine old v1–v24/Legacy40/Enlarged80, frozen Reference Future v1, pure-Sitka model-0, five-rack Rack V2/MFM, actual full three-species annual ecology, Work Plan/economy and history, original save hashes and no Windows build without personal playtest/authorisation.

**What passed:** GitHub one-file documentation publication verified and recommendation is coherent with minimal architecture principle. **Not verified:** feasibility fixtures, `dotnet` test build, native Unity parity, hash equivalence, hardware/runtime savings or current #443 branch. **Next:** #460 separately claimed feasibility under #424 Editor safety; later amended four-file DRAFT and bounded delta-review before owner decisions. **User action required now: none. Dashboard/Drive sync: NOT VERIFIED.**
