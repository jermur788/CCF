# Sen-1 #675 Manager — #690 G-PHYS preflight accepted, A0 donor scope selected, still NO SOURCE START

**DECISION: ACCEPT THE EXACT 2026-11-17 READ-ONLY PREFLIGHT. G-PHYS NOT GREEN; A0 remains BLOCKED.**
**A0 first parent:** `55863f1b7719011bba16b3f3ab377e379c0b988a` on `integration/scenario-one-personal-playtest-d073-20261112-v1`, reviewed Manager-accepted D-073 *conditional personal-Editor candidate*, not merged or product accepted.
**Donor second parent:** `209a08e9f220841aaec81385d4cb7b437fa5089f` on `feature/scen1-shared-three-profile-save-v2`, separately signed shared-save V2 implementation / T-01–T-03A and accepted #638 geometry + #647 two fixtures.

## 1. Sources actually inspected

- [#690 preflight result](https://github.com/jermur788/CCF/issues/690#issuecomment-6958634814): actual registered Codex GPT-6.2 Sol on Linux, source read-only, verified clean unique source checkout at donor, pinned local refs and bare-store objects, no A0 branch/checkout or physical board row/lease, prospective ext4 52-GiB path unused. Clean D-073 source readback, Multi/2D work disjoint. This is **point-in-time evidence, not permission to create a source worktree**.
- [#638 CLOSED source incorporation](https://github.com/jermur788/CCF/issues/638#issuecomment-6927675533), accepted within donor only; [#647 CLOSED two-fixture incorporation](https://github.com/jermur788/CCF/issues/647#issuecomment-6930279973), accepted within donor only. Old physical Ops rows remain historical READY/RESERVED and BLOCKED/RESERVED despite closed claims. No active #544/#638/#647 source writer observed. This is the *actual* stale-row cleanup authority, not a retroactive approval.
- [#553 physical board/custody repair PASS on 16 November](https://github.com/jermur788/CCF/blob/28f294d96c33b4e4cbf1a2ea2941ed30b75dc6a2/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-553-ROW460-REPAIR-PHYSICAL-RECONCILIATION-RESULT-20261116.md) is historical, narrower three-file custody. [#652 live Ops install HELD](https://github.com/jermur788/CCF/issues/652#issuecomment-6932841407); do not install/alter code of Ops or Unity.
- Independent fresh GitHub commit compare: source merge-base `50042bbe97734b031210ddebb603b5ec00ff459d` → exact donor has **15 `Assets/` files, 2 `Tools/` fixtures, 136 `Docs/` files, and 12 `specs/` files**. These are donor-side ancestral differences, NOT the exact final first-parent candidate diff. Three production paths changed on *both* source lines: `ForestSaveController.cs`, `ForestStartingStand.cs`, `ScenarioOne/ScenarioOneManager.cs`. Textually clean dry-run merge does NOT prove semantic compatibility.
- [#689 read-only admission](https://github.com/jermur788/CCF/blob/53eec5a9a5dba83785928880a9b02ea4aa91691c/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-689-COMPACT-PLAYABLE-CORE-EXACT-SOURCE-AND-SPEC-ADMISSION-20261117.md) / [#690 original G-BASE ruling](https://github.com/jermur788/CCF/blob/docs/sen-1-675-689-base-decision-a0-688-asset-blueprint-verdict-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-690-D073-SAVE-A0-SEMANTIC-RECONCILIATION-AND-PHYSICAL-GATE-PACKET-20261117.md) remain binding: old worlds, Cadence, three-profile closed save envelope, v1–v24, Reference and Compact switched-off world protected. No source test or Unity run at this Manager decision.

## 2. A0 DONOR TREE SCOPE — explicit technical Manager ruling

**Approved *prospectively*, conditional on later G-PHYS and source-start approval:**
1. Retain D-073 exact candidate as **first parent**, save donor as **second parent** of one true reconciliation merge, preserving Git ancestry without flattening history. No main merge.
2. In the *resulting first-parent tree diff*, admit only the following **15 existing/new Assets files** supplied by signed-save donor plus any narrowly necessary semantic reconciliation **inside those same files**:
   - `Assets/ForestPrototype/ForestSaveController.cs`
   - `Assets/ForestPrototype/ForestSaveEnvelope.cs` and `ForestSaveEnvelope.cs.meta`
   - `Assets/ForestPrototype/ForestSaveProfiles.cs` and `ForestSaveProfiles.cs.meta`
   - `Assets/ForestPrototype/ForestSaveSessionContext.cs` and `ForestSaveSessionContext.cs.meta`
   - `Assets/ForestPrototype/ForestSaveValidation.Geometry.cs`
   - `Assets/ForestPrototype/ForestSaveValidation.cs`
   - `Assets/ForestPrototype/ForestStartingStand.cs`
   - `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
   - `Assets/ForestPrototype/StandGeometryModel.cs`
   - `Assets/ForestPrototype/StandWorldBoundary.cs`
   - `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs`
   - `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs`.
3. Admit the **two already independently reviewed #647 source fixtures** needed for the signed-save compatibility/proof:
   - `Tools/Verification/ScenarioReferenceVerification.cs`
   - `Tools/Verification/StandGeometry/StandGeometryVerification.cs`.
   Their donor blobs must be checked against the previously accepted #647 exact source result. Do not rewrite baselines merely to make checks green; include native only when separately permitted. Tests and fixtures **remain unverified on the reconciled D-073 candidate**.
4. **EXCLUDE ALL 136 donor `Docs/` and ALL 12 donor `specs/` files from the *resulting first-parent tree diff***. Their historical commits and exact bytes remain accessible in the donor **second-parent ancestry and existing durable GitHub handoffs**. The signed 15-Nov save spec and 17-Nov Compact supplement remain binding through their independent exact-byte signatures; this selective A0 tree scope neither deletes authority nor rewrites those immutable approvals. Do not merge old overlapping canonical board, stale Decision Log, unapproved DRAFT spec, old owner declarations or any inherited docs that might overwrite newer owner D-085.
5. No additional `Assets/`, `Tools/`, `Docs/`, `specs/`, `Packages/`, `ProjectSettings/`, scene, prefabs, assets or root file changes are authorised in the A0 candidate. **One NEW external GitHub Markdown source result** is separately permitted on its own docs-only result branch *after actual source work*, without opening the main candidate's file ceiling. If a test needs new fixture/source/test changes outside the 17-path ceiling, STOP and ask Manager for a narrowly approved amendment, not a quiet expansion.
6. Implementation technique is worker-selectable only if safe: one merge with the desired first and second parents; restore/retain **all first-parent Docs/specs paths byte-for-byte and preserve D-073 untouched Assets outside the exact ceiling**; reconcile the 3 overlapping production files by inspecting both intended API semantics, not blindly accepting theirs/ours. Verify exact two parents, exact 17 first-parent changed-path ceiling, absence of old product-canon drift, no unknown new paths, and independent source review. A raw default merge of all 165 donor files is **NOT APPROVED**, and ancestry alone is not a licence to import old Docs/specs.
7. Always preserve **Compact `IsAvailable=false`**; no C-01/Objective1 (#443) held UX branch import; no B/C Work Plan, CT-3 or M-A code, no source overwrites from old checkout, no Unity/player save/Prefs actions. Proportionate Tier1 semantic/T02/T03A/#629 and fixture tests, Roslyn typecheck only with independently verified permitted assemblies; mark anything NOT RUN honestly. No test weakening.

**Why this is a bounded scope:** This unifies the actual two accepted source lines, carries previously accepted save fixtures and leaves product/spec authority on their immutable authoritative branches. It is NOT code implemented, compiled, reviewed or integrated by signing this *scope*.

## 3. G-PHYS disposition, exact next source ownership

**G-PHYS remains BLOCKED.** Prospectively reserve a sole **A0 source integration writer** with exclusive write custody of the 15 Assets and 2 verification fixtures **only** on new candidate branch `integration/sen-1-d073-shared-save-a0-20261117-v1`. Prospective unused Files/ext4 workspace from preflight: `/media/jer/Files/CCF/Worktrees/CCF-Sen1-690-A0-20261117`, canonical bare Git store `/media/jer/Files/CCF/Repository.git`. The directory **does not exist yet**; this is a candidate, not a granted lane/lease. The original #544 donor checkout and D-073 research/Editor candidate worktrees remain read-only/protected.

**Manager AUTHORISES a separate, bounded PHYSICAL BOARD MAINTENANCE operation only, not A0 code or new worktree:** [#693 physical Ops child](https://github.com/jermur788/CCF/issues/693), [full exact physical packet](https://github.com/jermur788/CCF/blob/docs/sen-1-690-a0-physical-reservation-and-donor-scope-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-693-A0-PHYSICAL-CUSTODY-AND-OPS-BOARD-RECONCILIATION-PACKET-20261117.md). An actually assigned authorised local Ops-board custodian must register first, compare current deployed board/lanes state and identity, then **mark old #638 and #647 physical reservations historical/released** while retaining exact original trace/history, and add **ONE new prospective A0 single-owner reservation** only if deployed supported NonUnity/Files source slot can be registered without #652 live Ops install or unsafe configuration. Verify 17-column schema/lint and all other board rows/lanes untouched, including Multi. If the available deployed tool cannot safely represent Files path, STOP and present precise blocker; **do not install #652** to fix it unilaterally.

After actual physical PASS, Manager #675 verifies its GitHub durable result, current main single-writer row and explicit source custody release, confirms real source worker/provider, actual NonUnity source slot/lease and no stale competing owner, and only **then** publishes an A0-specific **[APPROVED TO START]** permit. The old #638/#647 closures alone are not a source permit; #553 historical PASS does not reserve A0. Authorised A0 **production** begins solely after that separately published and linked decision. Unity/physical Editor lease remains HELD due to owner-restricted #652 installation and separate gate.

**No user decision now** for physical row correction or this donor-scope selection. A genuinely unavailable safe Files lane may require user-authorised Ops change; until then A0 remains BLOCKED. Owner asset path/look/A8 rights remain independently open.

## 4. Management status and follow-up

A0 [#690](https://github.com/jermur788/CCF/issues/690) **BLOCKED / NOT RUNNING** after source-read-only preflight; selected G-BASE and explicit donor 17-file scope are MANAGER-DECIDED. [#693](https://github.com/jermur788/CCF/issues/693) **READY / NOT RUNNING, actual physical board maintenance only**. Keep #638/#647 source DONE historical, do not reopen old worker claims or forge acceptance. No A0 worktree exists, no live Ops mutation yet, no source/Unity/Windows.

The separate Compact footpath [#688](https://github.com/jermur788/CCF/issues/688) Stage0 preproduction was accepted but actual asset stage blocked; graphics DRAFT [#691](https://github.com/jermur788/CCF/issues/691) ready. World #676 SIGNED0/CANDIDATE80/BLOCKED8, 36 ecology candidates, digest=null; three Compact specs DRAFT, OD-16 M-A-SCOPE deferred; #677 signed CG-TRIG technical policy not yet implemented. Dashboard local and Drive/context mirrors NOT VERIFIED/SYNCED.
