# Sen-1 + Multi #822: Independent nonauthor review of the lean instruction amendments and of shared-engine timing

**19 November 2026 · REVIEW RESULT / ADVISORY / NOT ADOPTED.** This is not a canonical instruction change, approval, context publication or implementation grant. The owner and #425 decide adoption.

| Field | Value |
|---|---|
| Claim | [#822](https://github.com/jermur788/CCF/issues/822), registration [comment 7018518487](https://github.com/jermur788/CCF/issues/822#issuecomment-7018518487) |
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness inside T3 Code, on the owner's Windows 11 host. Not a cloud sandbox. Not the GPT-6 author. |
| Starting `main` | `fe96b8b83bcab637136d7cf8b98d60bfa2d92626` |
| Packet read | `docs/portfolio-lean-instructions-cloud-review-handoff-20261119-v1` @ `9b55f477` |
| Proposal read | `docs/portfolio-lean-project-instructions-proposal-20261119-v1` (proposal Markdown, full text) |
| Audit read | `docs/portfolio-development-efficiency-audit-20261119-v1` (full text) |
| Output | this one file on `review/sen-1-multi-822-independent-lean-instructions-review-20261119-v1`. No other path was written. |

## Summary of the two verdicts

- **I. Instruction amendment as written: NEEDS CHANGES.** The problem is real and the author's diagnosis is mostly right. The three-file form is the wrong remedy: it states the same rules three times, puts two near-identical copies into the same generated ChatGPT composite, adds new recorded rituals, and removes no existing overhead. Replace it with **one short block in `shared-project-instructions.md`**, plus **two subtractive/clarifying edits in `AgentWorkflow.md`** and **one deletion in `chatgpt-role.md`**. Text is in §I.5.
- **II. Shared engine: PREPARE CONTRACTS NOW, MERGE LATER, with one refinement.** Convergence has **already started at the kernel level**: Multi's engine compile-links Unity's `SitkaGrowthModel.cs` and `SimulationRandom.cs` (§II.1). Recommended target: **option B**. Unity's Unity-free rule kernels become the shared library. Each product keeps its own annual orchestrator and scenario model until a trigger is met. Do not extract `ForestEcologyController.AdvanceOneYear` now. The time-sensitive decision is **before Multi builds its own Work Plan/Fell/history execution in Stage 5 G2**. That is where duplication would start, and it is also the cheapest point to reuse.

---

# PART I: Lean development Project Instructions

## I.1 What I inspected directly, and the limits

**Directly inspected:**
- Current `main` versions of `shared-project-instructions.md`, `chatgpt-role.md`, `claude-role.md`, `AgentWorkflow.md`, `AGENTS.md`, `CLAUDE.md`.
- `Tools/ProjectContext/generate_project_instructions.py` (`build_composite`).
- The full proposal and the full audit.
- GitHub issue bodies and comment timelines (public REST API) for #652, #802, #809, #813, #815, #820, #789, #804, #811, #812, #814, #816, #817, #818, #819, #822.

**Worker-reported only:** all test counts, Unity runs, deletions and lock states quoted from issues. I did not rerun anything.

**Cannot be verified:** token, cost or time totals, and counterfactual savings. I did not read the two retrospectives linked in packet §2D beyond what the audit quotes.

## I.2 The problem and the author's diagnosis

**Problem: confirmed.** In one day's sample, 100 comments from 19 Nov across the inspected issues averaged about 280 words each, and **55 of them carry a "Dashboard/Drive … NOT VERIFIED" line**. That line reflects a closure rule nobody is meeting (§I.4 F2). Examples:
- #652 has 38 comments over 16–19 Nov, with at least four independent reviews and two custodian runs. Its source was APPROVED on 16 Nov, and live deployment was still HELD on 19 Nov.
- A one-comma JSON defect in #813 was found at 13:49 and fixed at 15:54 (Manager recheck at 16:36), because it went back through the single authorised writer with a full correction packet.
- #812 → #816 → #817 produced a four-file draft spec, an independent review and a correction within about two hours. The owner's one-sentence clarification ("no difference between one rack and no rack") then superseded the geometry.

**Diagnosis: mostly right, but incomplete in three ways.**
1. **Not all detours were manager drift.** The #802 scope (inventory of all 51 Ops copies, deletion of whole obsolete folders and worktrees) was **owner-directed** (#652 comments of 09:12 and 09:23 UTC, 19 Nov). No Manager instruction would have prevented it. The lever there is an owner-facing cost question, not a worker rule.
2. **The positive example came from the owner, not from instructions.** #809 moved from packet to Manager disposition in about 75 minutes (10:24 → 11:37) because the **owner** wrote "MINIMUM TESTS TO GET PLAYER TEST GOING" and named M1–M3. The repeatable lesson is that a **named minimum witness** makes work fast. The proposal mentions this but does not make it the core mechanism.
3. **Some blockers cannot be fixed by prompts.** Unity licensing failed in the worker's launch context (#820). Host/physical permissions, and the D-091 no-expiry lease policy, are owner policy. Only the owner can relax D-091, and the proposal correctly does not try. The Manager's first #820 diagnosis ("owner must sign in to Unity Hub") was wrong and the owner corrected it. A "diagnose once, then hand the owner one concrete question" rule helps here. A longer classification taxonomy does not.

## I.3 What each clause would and would not have changed

| Case | Would change | Would not change |
|---|---|---|
| #652 / #754 Ops installer | "Do not convert a blocked operation into an open-ended tool-hardening programme" would have discouraged further scratch/v6/v7 rounds after source APPROVED on 16 Nov. | The live-install hold itself (owner risk decision plus physical gates). The escape clause "unless the defect directly prevents the selected milestone" would have let a lock-tool defect be argued as blocking Unity, which is how #652 grew. |
| #802 cleanup | Nothing material, because it was owner-directed. | The 51-copy inventory and the deletion scope. Only an owner-facing cost estimate before starting would have changed it. |
| #809 E1 | "Smallest honest working demonstration" restates what worked. | It already worked without the clause, because the owner named the witness. |
| #813 one comma | "For minor reversible editorial/format fixes, use direct readback … without reflexively creating a new agent/claim" is aimed at this. | The file is a scene-adjacent Unity asset under single-writer ownership, so the clause as worded does not authorise the Manager to fix it. The two hours are the cost of the single-writer rule, which is correct for scenes. Cheaper correction routing (a short message to the existing writer, not a new packet) is the realistic gain. |
| #815 reuse | Little. Reuse already happened under the owner's reuse request: 13 existing shared paths were reused. | The real defect was **scope reconciliation**: 3 of the 13 shared files were not recorded. A REUSE/ADAPT/EXCLUDE table does not catch that. A diff-vs-grant check at handoff does. |
| #820 licensing | "Bounded diagnosis … return one concise blocker" fits. | Licensing in the worker's session, and the D-091 lease reconciliation the Manager added before the next acquire. |
| #812 → #817 | "When the owner clarifies purpose … supersede irrelevant assumptions promptly." | The root cause was drafting a four-file spec and booking a review **before asking the owner one ambiguous question**. Shared instructions already say "Ask focused questions when answers materially change the work", so this was a compliance failure, not missing text. |

## I.4 Findings, ranked by severity

**F1. MAJOR: triplication, and duplication inside the same composite.**
- `build_composite()` concatenates **role header + authority block + shared core**. The `AgentWorkflow.md` body is **not** in either generated Project Instructions composite, only in attachments and mirrors.
- So proposal §A (shared) and §B (chatgpt-role) would both sit in the **same** ChatGPT composite as two variant wordings of outcome-first, REUSE/ADAPT/EXCLUDE and blocker triage. §C restates them a third time in AgentWorkflow.
- That costs tokens on every Manager turn, gives three places for wording to drift, and contradicts AgentWorkflow's own "do not duplicate" principle.
- Most of the substance already exists:
  - shared: "Small playable improvements … no speculative infrastructure", and "avoid unnecessary tests for minor reversible changes";
  - AGENTS: "Do not silently rewrite a working system";
  - claude-role: "do not build a competing implementation unless explicitly requested";
  - AgentWorkflow: tier escalation only on "a failed gate / a concrete newly discovered risk / …";
  - chatgpt-role and AgentWorkflow: "Do not force an independent review after every handoff".
- **Fix:** put the genuinely new rules once, in shared (which reaches both composites). Add nothing to chatgpt-role.

**F2. MAJOR: the proposal only adds and subtracts nothing.** The largest standing ceremony in the canonical text is left untouched:
- AgentWorkflow *Dashboard synchronization*: "A substantial task is not fully closed until this synchronization is verified", plus Manager workflow step 9. This is the source of the boilerplate in 55% of sampled comments.
- The durable-handoff and paste-ready-dispatch rule is stated in five files (shared, chatgpt-role, AgentWorkflow, AGENTS, claude-role). Shared and chatgpt-role duplicate it inside the same ChatGPT composite.

An anti-overhead change should remove at least as much as it adds. **Fix:** in §I.5 below.

**F3. MAJOR: no priority-elevation rule, which is the most important missing lever.**
- The proposal says non-feature work proceeds "unless … the owner separately prioritises it". It does not make the owner decision the **only** way to start tooling, security, Ops, audit or process work.
- #818 shows the owner already made exactly this call.
- **Fix:** non-feature work starts only by owner choice, or as the minimum fix for one specific defect that blocks the current task. Request it with a three-part question: what it unblocks, rough cost, consequence of deferral.

**F4. MAJOR: the escape clause re-opens the #652 path.**
- "…unless the defect directly prevents the selected milestone" lets a manager argue that any lock or Ops defect prevents Unity work.
- **Fix:** "then fix only what unblocks that specific action". This limits the size of the work, not only whether it starts.

**F5. MAJOR: the three-way HARD STOP / SAFE-LANE / DEFER classification adds a reporting ritual and contains one wrong category.**
- "Source/test evidence unavailable for a required acceptance claim" is listed as a **HARD STOP**. That is a limit on what can be *claimed*, not a reason to stop *work*, and labelling it a hard stop encourages halting.
- The workable core is simpler: stop the blocked action, diagnose once, name who can clear it, continue other authorised work, and do not re-investigate a characterised blocker without new evidence.

**F6. MINOR→MAJOR: mandatory REUSE/ADAPT/EXCLUDE records will become another document artefact, and they miss the real reuse failures.**
1. #815 failed at scope reconciliation (see I.3).
2. Multi #789's compile-linking of two **public, byte-identical `main`** Unity files was recorded as grant variance V-01 and is one of two reasons **main integration is BLOCKED** (#804). The process penalised the reuse it now wants to encourage.

**Fix:**
- Make reuse the default.
- Name reused files in the existing OWNED/SHARED files packet fields; no new table.
- State the legitimate exceptions: incompatible semantics or units; a protected model or reference that must stay unchanged (AGENTS/D-042: "A new model gets a new reference version"); engine/UI coupling that would leak into pure rules; a deliberately independent oracle.
- Treat reading or compile-linking **unprotected** public `main` source as ordinary reuse. Grants should restrict writes and explicitly named protected inputs.

**F7. MINOR (but real contradiction): reviewer independence.**
- AgentWorkflow *Roles* says: "A provider-native helper or child of the implementing worker does not count as an independent reviewer for that parent task."
- Actual practice uses parent-spawned reviewer children: #802 "Internal nonauthor review … child thread of this parent", #815 "three-round Opus internal review", and the #818 `orchestrate-work` one-parent flow. Those children found real defects (#802 D05 custody wording, wrong specimen count, and others).
- The proposal's "one accountable substantial task with bounded internal repair/review" would codify the practice without resolving the contradiction.
- **Fix** (in §I.5): define independence by authorship and input, and have the packet name the reviewer arrangement in advance for the high-risk classes.
- **This is a relaxation of written policy toward existing practice and needs explicit owner/#425 decision.**

**F8. MINOR.** The process-measurement paragraph is fine in spirit, but it is a standing instruction to collect metrics. Keep it out of canonical text and run it as the one-off pilot measurement the audit proposes (§D there).

**F9. MINOR.** No change is needed to `AGENTS.md`, `CLAUDE.md` or `claude-role.md`. Their existing rules are consistent with the corrected text. I agree with the proposal's choice not to touch Game Brief or the forest-mechanics Decision Log *for process*. The shared-engine priority in Part II is product architecture and does belong in the Decision Log.

## I.5 Minimal recommended final text

### A. `Docs/Project/instructions/shared-project-instructions.md`: replace the `## Development approach` section

Keep its second paragraph ("Separate ecology rules/data …") unchanged. Replace the first paragraph with:

> Small playable improvements; simplest maintainable solution for the milestone; no speculative infrastructure. Preserve reasonable future multiplayer/growth/testing options without building unnecessary systems.
>
> **Priority.** Default work is the owner's current player-visible feature or simulation outcome. Each substantial task names that outcome and its smallest real demonstration (for example: compile, enter Play, walk, one screenshot, no save mutation). Tooling, Ops, security hardening, process documents, audits and broad test expansion are not feature progress. Start them only when the owner chooses them, or as the minimum fix for one specific defect that blocks the current task, and then fix only what unblocks that task. To propose such work, ask the owner once: what it unblocks, rough cost, and what happens if it waits.
>
> **Blockers.** When a gate blocks an action, stop that action, diagnose once, name the exact blocker and who can clear it, and continue other already-authorised work. Never bypass or self-grant ownership/locks, approvals, save or frozen-reference protections, required independent review or release gates. Do not re-investigate a blocker that is already characterised unless new evidence appears.
>
> **Reuse.** Extend existing working code before writing a parallel version, and name the reused files in the packet. Write new code only for a stated reason: incompatible semantics or units, a protected model/reference that must stay unchanged (add a new model version instead), engine/UI coupling that would leak into pure rules, or a deliberately independent test oracle. Reading or compile-linking unprotected public `main` source is ordinary reuse; grants restrict writes and explicitly named protected inputs.
>
> **Purpose changes.** If an ambiguous word or requirement would change a spec, ask the owner before drafting. When the owner clarifies purpose, drop superseded work promptly. An intermediate step (tooling, a one-area slice, a document) is reported as that, never as the outcome it supports.
>
> **Long-term architecture.** The 2D and 3D products are intended to share one underlying forestry simulation engine eventually (Decision Log). That intent does not authorise engine extraction or refactoring inside feature tasks; convergence work needs its own owner-approved task.

(≈330 words in one place, versus ≈1,000 words across three files in the proposal. The last paragraph is the shared-engine guard asked for in packet §3A Q7.)

### B. `Docs/Project/AgentWorkflow.md`: two edits, no new section

1. **Roles → Reviewer.** Replace the sentence "A provider-native helper or child of the implementing worker does not count as an independent reviewer for that parent task." with:
   > A reviewer is independent when it did not author the change and works from the candidate, packet/spec and evidence rather than the author's conclusions. For ordinary Tier 1/2 feature work, a reviewer launched by the implementing parent on a different model may satisfy this. For save format/migration, RNG/determinism or frozen-reference changes, destructive deletion, canonical instruction changes and integration to `main`, the packet must name a reviewer arrangement approved by the Manager before work starts.
2. **Dashboard synchronization.** Replace the two paragraphs beginning "A substantial task is not fully closed …" and "If the dashboard or publisher is unavailable …" with:
   > Dashboard synchronization is a Manager batch activity at status or milestone updates, not a closure condition for individual tasks. Workers do not report dashboard state.

   Change Manager workflow step 9 to match: "At status or milestone updates, run the dashboard synchronization; record a failure once as an operational issue."

### C. `Docs/Project/instructions/chatgpt-role.md`: one deletion, no addition

Delete the first two paragraphs of `## Durable handoffs`: "Require every substantive …" and "Whenever the Manager dispatches work …". `shared-project-instructions.md` already carries both rules and is in the same ChatGPT composite. Keep only the final sentence on local-only handoffs, or move it to shared.

**If the owner wants an even smaller change:** adopt §A only. It delivers most of the routing benefit, because it reaches both composites.

## I.6 Safety and authority check

The corrected text explicitly preserves:
- ownership and single-writer locks;
- approvals (G1–G5);
- save and frozen-reference protections;
- required independent review;
- release gates;
- the D-091 lease policy (unchanged; nothing here introduces lease expiry);
- truthful state labels (an intermediate step is never reported as the outcome).

Elevating speculative work now requires an owner choice or a specific blocking defect, and even then the fix is limited in size. The only relaxation is F7 (reviewer independence), flagged for an explicit owner/#425 decision. The high-risk classes keep a Manager-approved reviewer.

## I.7 Adoption plan

1. Owner/#425 decide A (minimum) or A + B + C, and decide F7 explicitly.
2. One Manager-owned GitHub edit on a docs branch touching only the chosen files. Re-read the current blobs first. One focused readback of the diff, with no multi-agent policy study. G4 by #425 with owner confirmation.
3. Merge. At that one context commit, run the generator, verify the manifest hashes, publish the mirrors and refresh both Project Instructions composites and attachments, as AgentWorkflow already requires.
4. In-flight tasks keep their locked context. #730 and #731 apply the new context from their **next** packet. There is no mid-task reload and no lock change.

## I.8 Success tests

1. **New Ops/tooling defect during a gameplay task.** A worker finds a lock-script defect while the Unity lease is free. *Expected:* the worker fixes nothing outside the packet, reports the exact defect once, and continues the feature. The Manager asks the owner a three-part question. No new hardening issue opens without the owner's answer.
2. **One-character artefact defect.** *Expected:* the Manager sends a short correction request to the existing writer (no new packet or issue). Strict validation plus readback; no new reviewer.
3. **Ambiguous product noun** (like "rack"). *Expected:* one owner question before any spec draft or review booking.

**Watch for:**
- a feature task that silently grows a tooling subtask;
- a high-risk change reviewed only by an author-spawned child without the packet naming it;
- frozen hashes or old saves changing.

**Measure only what is observable,** over the next two features per programme:
- issues/comments per accepted outcome;
- time from owner request to first real witness;
- the share of comments carrying status boilerplate.

---

# PART II: When 2D and 3D should share one simulation engine

## II.1 Inspected architecture facts

**Unity 3D (`main`, inspected source):**
- **The rule kernels are already Unity-free.** About 55 simulation, economy and data files under `Assets/ForestPrototype` have no `using UnityEngine`. Examples: `SitkaGrowthModel.cs`, `SimulationRandom.cs`, `RegenerationModel.cs`, `AdultMortalityAccount.cs`, `MixedDensityModel*.cs`, `SpeciesSiteResponse.cs`, `WorkEconomy/*` (`ForestryWorkDomain`, `ForestryWorkCalculator`, `ForestryPriceBook`), `TimberYield/*`, `ScenarioManagementHistory.cs`, `ScenarioOnePrescription*.cs`, `ForestSaveData.cs`, `ForestSaveValidation*.cs`.
- **"No `using UnityEngine`" does not mean "linkable".**
  - `WorkEconomy` and `TimberYield` reference no Unity-coupled types.
  - History, approval and cycle state depend on `ScenarioOneWorkOrder` / `ScenarioManagementEvent`, whose only Unity dependency is one `Vector3` each. In `ScenarioManagementHistory`, `ScenarioOnePrescriptionApproval`, `ScenarioCycleState` and `ScenarioInterventionHistory`, every `ScenarioOneManager` mention is a comment, not code. `ScenarioInterventionHistory` does read one constant from `ForestEcologyController` (`HegyiCutoffMeters`).
  - `MixedDensityModel` and `ForestPlanting` reference `ForestEcologyController`, `TreeSpeciesDefinition` (a `ScriptableObject`) and `ForestEcologyCell`.
  - `JuvenileEcologyRules`, `StandGeometryModel` and `BrowsingConditions` use only `Mathf` / `Vector2` / `Vector3`, which are easy to shim. `StormModel` uses `Transform`.
- **The annual orchestrator is engine-coupled.** `ForestEcologyController.AdvanceOneYear()` is a 13-step sequence on a `MonoBehaviour` (2,191 lines). Tree state lives on `ForestTree` `MonoBehaviour`s, gathered with `Object.FindObjectsByType`. Competition model 2 calls `FindFirstObjectByType<ScenarioOneManager>()`.
- **Model versioning is extensive.** RNG 0/1, regeneration 0/1/2, growth 0/1, storm 0/1, and geometry Legacy40/Enlarged80 (plus Compact30 in candidates). Save schema v20. Many frozen anchors, for example Reference Future v1 Y100 `7AD177B3CC2F73C7` (overview §Reference Future v1, D-042/D-046).

**Multi 2D:**
- **Inspected (pushed P0/P1 foundation `feature/multi-p1-639-f0-30m-foundation-20261116-v1`):** standalone **.NET 8** C# (`SitkaLab.Core`, host, web UI). Build settings: `Deterministic`, `InvariantGlobalization`, `TreatWarningsAsErrors`.
- **Worker-reported, from the #789 result Markdown; the engine source is local-only on ZX20 and not on GitHub, so not inspected:**
  - `SitkaP1.Core.csproj` **compile-links `Assets/ForestPrototype/SitkaGrowthModel.cs` and `SimulationRandom.cs`** from the repo root (byte-identical to `main` `e4c73402` / `bb83d7f8`).
  - Mortality rolls use `(RNG model, "ADULT-MORT-"+id, year, seed)`. This is the **same key** as Unity's `ApplyAdultDensityMortality` (`ForestEcologyController.cs`, the `SimulationRandom.Roll(... "ADULT-MORT-" + tree.TreeId ...)` call).
  - Stand metrics and the annual order are re-implemented in Multi's own `AnnualRules.cs`, for a Sitka-only adult stand.

**Conclusion from evidence.** The two products already share the growth/mortality kernel and the RNG by source link. They differ in orchestration, state model, fixtures and scope.

**Not established, and needing a test:**
- **numerical equality across runtimes.** Unity Mono/IL2CPP versus .NET 8 RyuJIT. `NextHeight` and `Roll` return `float`, and float evaluation can differ between runtimes;
- **equal annual sequencing.** Per the #789 scope, Multi models adult Sitka only, without regeneration, competition model 2 or storms;
- equal precision of the stand metrics. #789 notes "stems/ha is float then widened".

## II.2 Options compared

| Option | Reused | Rewritten / risked | Verdict |
|---|---|---|---|
| **A.** One deterministic domain engine with 2D/3D adapters, now | Everything, eventually | `AdvanceOneYear` and `ForestTree` state extracted from `MonoBehaviour`s; every frozen hash at risk; both feature streams stalled | **Not now.** It is the right end state only if both products need the *same* annual model. |
| **B.** Shared kernel library (biology, RNG, work/economy, timber, history types) with scenario-specific orchestrators | Unity's Unity-free kernels, which already exist and are already partly shared | Small shims (position struct for `Vector3`, species data without `ScriptableObject`); no orchestration change | **Recommended target for the next milestones.** |
| **C.** Common contracts (tree identity, year snapshot, event/Fell/death cause, Work Plan states) while each computes separately | Concepts and serialisation | Duplicate implementations accumulate | **Do alongside B for things not yet shareable as code.** On its own it invites drift. |
| **D.** Permanent separate engines | Nothing | Two models to calibrate and maintain | **Reject as a default.** It is justified only if Multi deliberately serves as an independent oracle, and the owner's stated priority goes against it. |

## II.3 Which source should be authoritative

**Unity's kernels.** They are multi-species, calibrated, versioned, save-integrated and already the source Multi links. A from-scratch core is not justified.

Multi contributes:
- **test discipline:** golden vectors, mutation-tested assertions, the two-process determinism check;
- **the extrapolation logic** (sample → comparable areas), which Unity does not have.

That logic should be written **against shared kernel types** from the start.

**Inventory a future architect should complete** (REUSE / ADAPT / EXCLUDE; facts I could not settle are marked ?):
- **REUSE as-is:** `SitkaGrowthModel`, `SimulationRandom`, `AdultMortalityAccount`, `RegenerationModel` constants, `WorkEconomy/*`, `TimberYield/*` (?: confirm there are no transitive Unity references by compiling them in a net8 project).
- **ADAPT (shim):**
  - `ScenarioOneWorkOrder`, `ScenarioManagementEvent`, `ScenarioManagementHistory`, `ScenarioOnePrescriptionApproval`, `ScenarioCycleState` (replace `Vector3`), and `ScenarioInterventionHistory` (also move the `HegyiCutoffMeters` constant);
  - `JuvenileEcologyRules`, `StandGeometryModel` (replace `Mathf`);
  - `MixedDensityModel` (decouple from the controller and `TreeSpeciesDefinition`).
- **EXCLUDE for now:**
  - the `ForestEcologyController` orchestration, `ForestTree`, `StormModel` (uses `Transform`) and the save classes. These are bound to the protected Legacy40/Enlarged80/Reference Future contracts;
  - Multi's `AnnualRules` orchestration (?: compare it with Unity step 4b once the source is on GitHub).

## II.4 When convergence should begin

| Phase | Trigger | Necessary or optional |
|---|---|---|
| **Interface/contract notes** | Now, at low cost. | Optional but cheap. |
| **Kernel parity proof** (§II.5) | Now, as one small task outside both feature streams. | **Necessary** before any further sharing. |
| **Multi reuses Unity work/history types instead of writing its own** | **Before Multi Stage 5 G2 implementation starts** (Work Plan approval → Fell → once-only execution). | **Necessary, and time-sensitive.** This is where a second Work Plan/history implementation would otherwise be written. |
| **Dual-consumer shared library** (one `net8`/`netstandard2.1` source set compiled by Unity and Multi) | (a) Compact30 E2 passes native Tier 2 with its save/profile policy signed (#820 and #690); (b) Multi G2 executes once-only Fell plus the next annual step; (c) the parity proof passes. | **Necessary.** |
| **Orchestrator extraction / production migration** (Unity annual step runs on the shared engine) | All of the above, plus either a second real scenario that needs the same annual sequence or Multi's multi-area extrapolation needing full three-species ecology. Also an approved new model/reference version, so frozen anchors stay untouched. | **Necessary for this phase only.** Optional overall: it may never be worth it if Multi stays Sitka-adult-only. |

**Neither feature stream waits on this sequence.** E1/E2 and the Multi G1/extrapolation work proceed unchanged. The only constraint on them is "reuse the existing types at G2".

## II.5 The smallest safe feasibility experiment

**A cross-runtime kernel parity test.** It is read-only, reversible, and changes no save or biology.
1. In a new test-only folder (for example `Tools/Verification/SharedKernelParity/`), a `net8.0` console project compile-links `SitkaGrowthModel.cs`, `SimulationRandom.cs`, `ForestryWorkCalculator.cs` and `TimberYieldCalculator.cs` from `main` without modifying them.
2. It evaluates a fixed input grid and writes a canonical output hash:
   - heights for ages 1–120;
   - `RelativeDensity` / `AnnualDeathProbability` grids;
   - 10,000 `Roll` calls across models 0 and 1;
   - a few work/timber calculations.
3. A disposable Unity Editor script (following the standard copy/run/remove workflow) computes the same grid in Mono, and in IL2CPP only if a Player build is in scope.
4. **Pass:** identical hashes. **Fail:** document which functions differ and why (float evaluation). That outcome itself decides whether the shared library must use `double` or fixed precision, or carry a per-runtime tolerance contract.

**Cost: small** (a single bounded task). It touches no production files, and rejecting it means deleting one folder. A second step, still optional: compile the ADAPT list with a 10-line `Vector3` shim. This measures exactly how much of the Work Plan/history domain is portable before Multi G2 is designed.

## II.6 Preserving existing functionality, models, saves and reference behaviour

- **Frozen references** (Reference Future v1, Legacy40, calibrated anchors): never re-routed through shared code under the same model version. Any shared-engine path that changes arithmetic ships as a **new model version**, as D-042/D-046 already require.
- **Enlarged80 / Compact30 saves:** shared kernels must stay byte-compatible in behaviour (parity hashes) before Unity consumes a moved source file. Moving a `.cs` file inside `Assets/` must keep its `.meta` GUID. The safest route: keep files where they are and have Multi link them, as it does today.
- **Multi's 100-tree golden lineage:** stays bound to its current linked kernel bytes. A kernel change requires a recorded golden re-baseline decision, never a silent update.
- **Ownership:** shared kernel files get one named owner (#731's Unity lane, since they live in `Assets/`). Multi consumes them read-only. Any change to a shared kernel runs both products' existing fast tests (Tier 1).
- **Scenario differences stay as data or model selectors:** density, species roster, geometry and one-area versus five-rack layout are not forked into the code.

## II.7 Roadmap

| When | What |
|---|---|
| **Do now** | Record the decision (below). Run the §II.5 parity test as one small task. Let Multi integrate its existing compile-link of the two public files without further grant ceremony (F6). |
| **Before Multi Stage 5 G2 design** | Check `ScenarioOneWorkOrder` / `ScenarioManagementEvent` / `ScenarioManagementHistory` / `ForestryWorkDomain` for reuse. Shim if feasible; record the reason if not. |
| **After E2 native Tier 2 + signed save policy, and Multi G2** | A shared library source set, compiled by both products. Contract tests. |
| **Defer until a second scenario or Multi needing full ecology, plus a new model version** | Extract the annual orchestrator from `MonoBehaviour`s. |

## II.8 How to record the priority

- **Decision Log:** add one entry. "Accepted direction: 2D and 3D share one forestry simulation engine eventually. Stage 1 is shared Unity-free kernels; orchestrator extraction is deferred to the triggers in #822 §II.4; no immediate implementation authority."
- **Project Instructions:** the one-sentence guard in §I.5 A.
- **Game Brief and Current Milestone:** no change yet.
- **Architecture note:** a short `Docs/Architecture/shared-engine.md` only when the parity test reports.

## II.9 Strongest counterargument

**Divergence is cheap now and expensive later, so start option A at once.** The case:
- Multi is about to build Work Plan/Fell/history and multi-area extrapolation;
- every month of separate orchestration adds re-convergence cost;
- Unity's `FindObjectsByType`-based annual loop will only accrete more coupling.

**My answer:**
- The cheap part of convergence (kernels) has already happened.
- The time-sensitive part (work/history types) is handled by the Stage 5 G2 reuse checkpoint.
- The expensive part (orchestrator extraction) would put frozen anchors at risk and stall the beta for a benefit that depends on Multi ever needing three-species ecology, which is not yet established.

**Caveat:**
- if the parity test fails badly (float divergence across runtimes), even kernel sharing needs a precision decision first;
- if Multi's extrapolation turns out to need regeneration/competition, the orchestrator extraction trigger arrives earlier.

---

## Evidence classification

| Class | Items |
|---|---|
| **Directly inspected** | Canonical instruction files and generator on `main` `fe96b8b8`; proposal; audit; packet; Unity C# source on `main` (UnityEngine usage, type references, `AdvanceOneYear`, kernel signatures); pushed Multi P0/P1 foundation layout and build props; issue timelines listed in §I.1; the 55/100 boilerplate count (my own count over the 19 Nov comments I downloaded) |
| **Worker-reported** | All test counts and PASS states; the Multi Year2–10 engine design, compile-links and RNG keying (#789 result Markdown); #802 deletions; #820 licensing facts |
| **Inferred** | Cost attributions in §I.2–I.3; the likely float-divergence risk; the effect of the proposed wording |
| **Cannot be verified** | Token, time and cost totals; counterfactual savings; the local-only Multi engine source; any Unity runtime behaviour |

**Nothing in this review modifies production code, canonical instructions, simulation engines, Unity, Ops, Drive or the dashboard.**
