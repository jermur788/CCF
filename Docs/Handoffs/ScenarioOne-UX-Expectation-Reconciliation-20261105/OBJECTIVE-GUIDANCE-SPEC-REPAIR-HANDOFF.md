# #282 Objective guidance DRAFT repair handoff

**Status:** DRAFT / HANDED-OFF — MANAGER REVIEW REQUIRED. No production approval, implementation, integration or player acceptance.

## Provenance, lane and exact diff

- Binding authority: top **LATEST OVERALL MANAGER RECONCILIATION — 5 NOV 2026** block in `OBJECTIVE-GUIDANCE-SPEC-REPAIR-PACKET.md` on live main; D-071 in `Docs/Project/decision-log.md:122` and current milestone govern the layout.
- Exact source/base: main `1e3e159b768dc4b0bfe6e410a7728a412d34e5db`, timestamp `2026-11-05T23:53:05Z`; context `81867c8940d7d07c952e8e49ca4a0cb93498e790`.
- Worktree: `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveSpec-20261104`.
- Branch: `docs/overnight-objective-guidance-spec-20261104`; HEAD before repair `a173192f733f9e6c56a9cebc02a51c1f8fec97d9`, matched origin branch. Initial fetch succeeded; porcelain status empty. No alternate checkout, merge, rebase, reset, clean or stash.
- Source read using `git show origin/main:<path>`; old docs branch is 348 commits behind source main, not an implementation base. Current helper contract supports the proposed projection; no new material helper mismatch requiring a product redesign was found in static inspection.
- Author role: primary design/spec worker. No child reviewer dispatched by this worker; independent review remains for orchestrator/Manager routing.

Only these five paths change in this repair commit (older PR #265 handoff remains untouched history):

1. `specs/scenario-one-next-action-guidance-v1/spec.md` — current-source ledger, all findings, cue/diagram contracts, future gates and decision table.
2. `specs/scenario-one-next-action-guidance-v1/plan.md` — conditional bounded architecture, explicit consent/input costs, sequence and ownership.
3. `specs/scenario-one-next-action-guidance-v1/tasks.md` — standalone T1–T5, own claim/lane registration required after approval.
4. `specs/scenario-one-next-action-guidance-v1/checklists/requirements.md` — unchecked release and acceptance gates; no prose PASS claims.
5. `Docs/Handoffs/ScenarioOne-UX-Expectation-Reconciliation-20261105/OBJECTIVE-GUIDANCE-SPEC-REPAIR-HANDOFF.md` — this new return.

The publication commit SHA is reported by the worker's final Git receipt rather than embedded self-referentially in this file. Normal commit/push updates PR #265; no issue/PR comment is authorised for this worker. Orchestrator handles claims, coordination, routing ledger, Manager and independent review. Dashboard/attachment publication not performed or verified here; no generated state edited.

## D-071 correction and known player expectations

Scenario One keeps the **existing southern-headland entrance and FIVE PARALLEL Rack V2 extraction racks**. D-067–D-069 are superseded historical research for Scenario One, not production alternatives. The older spec's conditional-direction wording is removed from all four artifacts. Visual “Option A” means current parallel racks, not historical two-family “MFM Option A”. No northern access spine, diagonal/herringbone 3+2, single-family spine entry or player-built extension is proposed. D-070 workbench retirement remains independent/out of scope.

Source orientation is not invented: `Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs:7–23` defines the longitudinal X=-35.4 headland and existing southern feeder Z=-27.4. The illustration must match that reference, not turn “southern entrance” into a new southern road. No new access topology is needed by the proposed illustration. Exact representative rack endpoints/capture remain subject to T1 approval.

#281/#282 expectation: Objectives should highlight Stand Map, map should highlight Plan Racks and then the actual next enabled action through Confirm, entrance marking, Work Plan and annual review; the lesson diagram should be map-like, not abstract rows. Current main retains primary Back to forest and abstract diagram data. This is **feature not written / expectation-scope gap**, not evidence that implemented #258 guidance failed. The draft repair does not fix the player experience, close #281 or erase historical static CTA FAIL 08. Work Plan is still the commitment gate; nothing automatically marks, approves or advances. O1 is the bounded first slice; later-objective intent remains explicit and incomplete.

## #270 finding disposition (all still require decision/evidence)

Detailed current line-cited facts are in spec S1–S14 at the exact source/base above. Table R and AC IDs are the repair's traceability, not acceptance results.

| Finding | Documentation disposition | Remaining gate |
|---|---|---|
| R-01 High — UiKit.Button focusable=false | S8/FR-6: explicitly scoped opt-in/filtering proposal; no global reversal or claim that keyboard already works. | D-07/Q1, T2, AC-K real input. NOT FIXED. |
| R-02 High — destructive primary, safe ghost | S4/FR-2: acknowledge actual classes; explicitly assign safe prompt restyle to future T2, never elevate destructive action. | D-08 approval, AC-P source/style and rendered gates. NOT FIXED. |
| R-03 High — Consent ∥ Reset | S5/FR-2: ambiguous → **no eligible primary** is mandatory/load-bearing; no Boolean precedence or fake modal ownership. | D-09, AC-X explicit cross-state source and Tier-2 checks; no lifecycle fix delivered. |
| Medium — guidance vs input ownership | Separate presentation scope from actual event capture/keyboard routing. | AC-P/H/K; no new implicit modal system. |
| Medium — map pointer-down clears prompts | Record eligible active-layer left pointer-down clearing; rederive instead of claiming pending-until-answer. | AC-P/X native dismissal and stale-target check. |
| Medium — first Help/onboarding cue | First cue belongs to visible safe Continue/page navigation; underlying map cue follows dismissal. | AC-H clean first-use and return flow. |
| Medium — Help force-closable | Context/SetScreen can close Help despite guarded user keys; invalidate/rederive safely. | AC-H forced close/navigation, no fall-through. |
| Medium — availability gates | Include RackPlanProblem, RackApprovalProblem, RackPlanningAvailable and existing confirmation/year/review gates. | AC-G blocked/unavailable variants with reasons. |
| Medium — unsaved rackMode | Derive from persisted world facts plus transient view/session state, not saved facts alone. | AC-S load/re-entry, no new keys. |
| Medium — 16 px regression risk | New elements only, actual panel-scale ≥16 physical px AND equivalent-role non-regression. | D-12, AC-V two-resolution measurement. |
| Medium — screen reader absent | Explicit deferral; visual text equivalent is not delivery or accessibility certification. | D-10 scope; no screen-reader PASS. |
| Medium — judgement-only figure | Reference/transform/tolerances/counts/recognition rubric introduced; reference still unapproved. | D-11/Q2, AC-D/V colour/grayscale at both sizes. |

Prior P1/P2/P3 FIXED statements are withdrawn in all four artifacts. These dispositions address the **specification defects in writing**; only Manager approval, independent review, implementation and actual gates can close the corresponding blockers. No self-review is substituted for the required opposite-provider review.

## Manager decisions / unresolved Q1–Q4

| Item | Recommendation / support | Risk and needed disposition | Genuine user choice? |
|---|---|---|---|
| Q1 / D-07 | Scoped focusable controls; proposed conflict-checked F6/Shift+F6 traversal, Tab remains Work Plan. If not approved, explicitly mouse-only first slice. | Production input change, not already fixed. Manager must approve exact binding/filtering/scope after #257 conflict check. | Not needed to draft; escalate only if unresolved preference/conflict changes intended product. |
| D-08 | Safe rack-prompt primary restyle in T2. | Consent UX changes; assign owner/files explicitly. | Manager scope decision. |
| D-09 | Keep no-primary ambiguity fallback; separate lifecycle repair if wanted. | Reachable Consent ∥ Reset has no eligible primary by design. | Manager disposition. |
| D-10 | Defer screen-reader delivery honestly. | No comprehensive accessibility claim. | Manager scope decision; user has not accepted a limitation by implication. |
| Q2 / D-11 | Static teaching-map illustration of D-071 family; approve geometry/source/capture fixture and numerical rubric first. | Not live Stand Map or new route; no approved endpoint fixture/render yet. | No geometry choice remains open; user only if requesting different representation. |
| D-12 | New-only physical-pixel floor plus role non-regression. | Measured scale required; source tokens not sufficient. | Manager decision. |
| Q3 / D-13 | O1 first, no new HUD widget; later mapping T4 separately released. | Whole-objective intent not delivered by first slice; later handler mappings REVIEW NEEDED. | Manager release decision unless intent materially changes. |
| Q4 | Pinned current main for design; Manager/integrator chooses fresh exact implementation base and registered lane after approval. | Do not implement from old docs branch; re-audit changed source contracts. | Manager/integrator, not user policy. |

All Q1–Q4 are **MANAGER REVIEW REQUIRED**, supported recommendations only. No self-approved focus/consent policy or representation change. No user action required for this DRAFT handoff. If policy cannot remain bounded, source changes materially, geometry needs a blocked spine or a genuine new representation choice arises, return NEEDS DECISION rather than implementing.

## Evidence classes and remaining gates

- **Verified source facts:** clean exact branch/base preflight; live main D-071/packet/coordination, issues #258/#281/#282, PR #265 and #270 review read; current helper/counter/prompt/gate/map-symbol/diagram contracts inspected. Spec S1–S14 records current source paths/line numbers. Static source is not runtime evidence.
- **Documentation integrity checks:** `git diff --check` and staged `git diff --cached --check` passed; staged path inventory matched exactly the five authorised files, with no unstaged diff at that check. Context object exists, source main timestamp matches the packet, and registered worktree/branch receipt matches dispatch. These checks do not count as compiler/tests/runtime/acceptance results.
- **Proposals:** FR-1–6, safe prompt styling, scoped focus/navigation, text floors, static reference/rubric and T1–T5 sequence. No production file changed, no policy approved.
- **Unverified / NOT RUN:** compiler, tests, Unity/Editor/Player, input reach/dispatch, rendered focus/styles, two-resolution/grayscale rubric, exact diagram reference, save/load/Legacy40/reference behaviour, resource/duplicate-action regressions, user clarity and acceptance. No screenshots fabricated. Existing static FAIL remains FAIL; no gate weakening.
- **Release gates:** Manager Q1–Q4/D-07–D-13 decisions and independent opposite-provider spec review; explicit spec approval; separately registered owned claims/lanes for T1 decisions/reference, T2 cues/focus/prompts, T3 teaching map, T4 later mapping if released and T5 qualification. Required candidate native-input and visual qualification uses exact HEAD at **1280×720 and 1920×1080** plus independent cross-provider review, protected compatibility and Manager/user acceptance before integration. Current worker must not dispatch or run those tasks.
- **Protected / untouched:** production C#/USS, Assets/meta, Packages, ProjectSettings, Project docs/live coordination, other specs, dashboard state, saves/user active playtest worktree, physical rack layout, frozen Reference Future v1, Legacy40, Unity lease and Windows builds. Historical PR handoff not rewritten. No GitHub comments posted.

**OBJECTIVE-GUIDANCE-SPEC-REVISED — MANAGER REVIEW REQUIRED**

Stop state: **DRAFT / HANDED-OFF**.
