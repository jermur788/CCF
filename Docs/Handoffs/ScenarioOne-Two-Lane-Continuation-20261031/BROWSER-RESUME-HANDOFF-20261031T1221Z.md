# Scenario One Browser / GitHub Manager Lane 03 — Resume Handoff — 2026-10-31 12:21 UTC

**Parent umbrella:** #200 / #91  
**Bounded execution claim:** #202  
**Counterpart local orchestrator:** #201 / resume claim #203  
**Lane:** browser/GitHub only; no local worktree  
**Difficulty:** Medium — multiple returned handoffs, one product/verification contract conflict, and integration/release sequencing; no local Unity work  
**Best-suited provider/model:** ChatGPT browser/GitHub Manager lane, GPT-5.6 Sol or equivalent Manager-capable browser model  
**User action after publication:** give the dispatch line at the end of this handoff to the already-running browser chat or a replacement browser chat in this project.

## Purpose

Resume the browser/Manager side of the Scenario One two-lane programme after the returned #201 orchestration run. The browser lane is responsible for Manager verdicts, safe GitHub integration/release decisions, task/claim/packet preparation, and the live coordination record. It owns no local C#/Unity/compiler/runtime/build work.

This handoff does **not** replace #200. It is a bounded continuation packet under #202 so the browser chat has an exact current decision set.

## Live-authority boundary

At task start, refresh:

1. `Docs/Project/coordination/active-tasks.md`;
2. #200, #202, #201, #203;
3. #156, #196, #198, #199;
4. current `main` and the published Scenario One integration ref;
5. any comments or commits newer than this handoff.

If fresh evidence conflicts with this packet, live canonical state wins. #200 remains the sole primary writer for `active-tasks.md` during the two-lane split. Do not infer local cleanliness, Unity availability, Editor state or runtime behavior from GitHub alone.

The user's explicit **NO ACTION** restriction on Research Source Consolidation Audit #194 remains in force unless the user explicitly lifts it.

## Returned #201 evidence to review

### #196 — MFM-08 MapPlanning Contract Reconciliation 01

Returned branch: `verify/mfm-08-mapplanning-contract-reconcile-v1` @ `eb4be600`  
Verification patch: `eef1bdf9`  
Production candidate unchanged: `e5fb424635f16182748efcb0539e8a49539495a9`

Observed returned evidence:

- verification-only diff in `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs`;
- original five tabs remain asserted by name and order;
- `Treatment review` is asserted only as the additive sixth Rack Planning V2 tab;
- historical/non-V2 five-tab branch remains explicit;
- complete authoritative gate: `RACK_V2_MAP_PLANNING_UNITY_PASS checks=168 shots=8`;
- 168/168 checks, 8 captures, Unity exit 0, no `error CS`;
- no production/save/schema/anchor/reference change;
- lock released normally, lane clean, no Editor.

### Manager direction for #196

**Expected verdict: APPROVED**, unless fresh evidence supersedes the returned handoff.

Reason: the patch reconciles the stale exact-five assertion without weakening the original contract, adds stronger name/order coverage, and the full authoritative gate is green on the exact MFM-08 production candidate.

Required browser actions if approved:

1. publish a durable Manager review under `Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/`;
2. comment/close #196 as Manager APPROVED;
3. re-evaluate #156 using the already accepted MFM-08 feature evidence plus #196 closure;
4. if no other MFM-specific blocker exists, upgrade #156 to **Manager APPROVED / READY FOR INTEGRATION**;
5. perform exact overlap/ancestry review before integrating the MFM-08 production delta onto the current Scenario One integration lineage;
6. keep verification-only patches distinct from production integration unless the verification harness itself is intentionally promoted by the normal verification policy.

## #198 — Guided-v2 v24 / D-066 Contract Reconciliation 01

Returned branch: `verify/guided-v2-v24-contract-reconcile-v1` @ `59b278f7`  
Verification commits: `bac20456`, `1b8127a2`  
Exact production start unchanged: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

Observed returned evidence:

- verification-only changes in `SequentialObjectivesVerification.cs` and `GuidedOnboardingVerification.cs`;
- v23 pins reconciled to current v24;
- pre-D-066 reload expectation replaced by the durable first-guided-review latch while retaining non-persistent session detail;
- V2 guided harness now performs the accepted five entrance marks before approval rather than expecting the post-marking lesson prematurely;
- `guided-v2`: 535 PASS / 0 FAIL, 48 captures, `GUIDED_ONBOARDING_PASS`;
- `guided-legacy`: 569 PASS / 0 FAIL, 62 captures, `GUIDED_ONBOARDING_PASS`;
- one first legacy real-input driver failure was preserved and classified ENVIRONMENT/RUNNER; clean retry passed on the same tree;
- no production/schema/anchor/reference change;
- lock released normally, lane clean, no Editor.

### Manager direction for #198

**Expected verdict: APPROVED**, unless fresh evidence supersedes the returned handoff.

Required browser actions if approved:

1. publish durable Manager review under `Docs/Handoffs/GuidedV2-v24-Contract-Reconciliation-01/`;
2. comment/close #198 as Manager APPROVED;
3. retain the real-input timing issue as workflow debt, not a feature blocker and not a reason for model escalation;
4. record that current guided-v2 and guided-legacy contracts are green on the reconciled verification lineage.

## #199 — Scenario One Combined HUD / Focus Regression Repair 01

Partial branch: `fix/scenario-one-combined-hud-focus-regression-v1` @ `42742a8a`  
Exact start: `ce673714...`

Observed partial result:

- Sonnet 5.5 Medium implemented a **fixture-only** correction so the Combined gate no longer tries to crop-mark Rack-owned trees;
- no production `Assets/` file has yet changed;
- combined-v2: **453 pass / 10 fail / 24 captures**;
- combined-legacy: **440 pass / 8 fail / 24 captures**;
- the earlier 1920 V2 `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN` false failures are gone;
- a real 1920 competitor-list clipping defect is now visible (`lastLabelBottom=915`, `panelBottom=896`);
- remaining estimate/minimap and FOCUS_04/06/07 failures trace to PARITY-07's deliberate implementation choice to hide the minimap for ordinary estimate inspections, while the protected Combined contract requires the minimap to step aside only for the competitor list;
- repository evidence shows PARITY-07 made that implementation trade-off, but no later Decision Log/user acceptance was found that supersedes the protected Combined minimap behavior.

### Manager direction for #199

Choose **Option 1: bounded production layout repair**.

Authorise the local lane to satisfy **both** accepted/protected contracts rather than changing one to make the other pass:

- restore minimap visibility for ordinary estimate inspection;
- retain the accepted PARITY-07 estimate information/copy unless a later direct user/Decision Log instruction changes it;
- retain PARITY-07 central-prompt/detail-column clearance requirements;
- keep the protected Combined assertions semantically unchanged;
- repair the exposed 1920 V2 competitor-list clipping in the same bounded UI/presentation pass if it shares the affected layout surface;
- prefer responsive/reflow/layout adjustments over copy deletion or test relaxation;
- do not touch Rack geometry/marking/persistence, MFM behavior, save/schema/anchors, ecology or economy.

Do **not** select, absent new authority:

- Option 2 (shorten accepted estimate copy);
- Option 3 (supersede protected Combined assertions merely to fit the current implementation);
- Option 4 (defer the known regression indefinitely).

### Verification required for the released #199 continuation

The local child packet should require at minimum:

1. before/after root-cause and production diff;
2. full `combined-v2` and `combined-legacy` at 1280×720 and 1920×1080;
3. PARITY-07 rendered/UI qualification sufficient to prove estimate/detail/prompt clearance is preserved;
4. affected minimap/inspection/focus regressions;
5. proportionate Rack/guided regression where the modified UI surface overlaps those flows;
6. independent review by a thread/provider independent of the production implementer;
7. clean worktree, Editor and Unity-lock return;
8. no Windows build.

If a layout solution cannot satisfy both protected contracts without a genuine product decision, stop and return the smallest concrete conflict. Do not weaken tests or spend a stronger model merely to bypass authority.

## Programme sequencing after these verdicts

After #196 approval:

- #156 may be upgraded if its previously accepted feature evidence remains valid and #196 was indeed the final MFM-specific blocker;
- browser lane performs safe exact-lineage integration review and integrates only the approved production delta;
- MFM-09 may then be repinned/released against the exact combined integration SHA.

Keep the established complete-loop programme dependencies visible:

- S1-C T-08 final history qualification;
- S1-D T-08 multi-cycle witness;
- S1-D T-09 independent conformance review;
- MFM-09 → MFM-10 `SPECIES GUARD QUALIFIED`;
- PARITY-09 / MFM-11 as dependencies permit;
- MFM-12 integrated qualification;
- #199 must be green before final Tier-3 while its protected Combined contract remains applicable;
- final Scenario One Tier-3 only when all required accepted systems are on one exact candidate.

Do not silently reprioritise pre-Tier-3 debt ahead of the established S1-C/S1-D/MFM critical path unless live evidence shows it directly blocks the next gate. The browser lane may release #199 opportunistically when the serialized Unity lane would otherwise be idle or when its UI regression blocks an upcoming qualification.

## Browser lane responsibilities after release

- remain sole primary writer for `Docs/Project/coordination/active-tasks.md`;
- keep #91, #200, #202, #201/#203 and child claims reconciled;
- inspect every returned handoff and issue `APPROVED / NEEDS CHANGES / REJECTED`;
- create/repin exact local packets when dependencies clear;
- perform safe browser-side integration only after exact evidence and overlap review;
- never infer local/Unity cleanliness from GitHub state;
- preserve frozen Reference Future v1 and Legacy40 compatibility;
- keep IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED / USER PLAYTESTED distinct;
- no Windows build before the user's personal playtest and explicit post-playtest authorisation.

## Required outputs

1. Manager review for #196 and issue disposition.
2. Manager review for #198 and issue disposition.
3. Explicit #199 Option-1 disposition and bounded continuation packet/release to #203/#201.
4. #156 status update/integration decision if #196 closes its final blocker.
5. Updated `active-tasks.md`, #91/#200/#202 status and downstream exact next action.
6. A concise return to the user stating what changed, what passed/failed, what it means, next step, user action required, and dashboard/coordination sync status.

## Copy/paste dispatch line

`Take Browser Manager Resume 01 under claim #202, parent browser lane #200. Refresh active-tasks.md, #200/#202/#201/#203, #156/#196/#198/#199 and current integration/main before acting. Review the returned #201 evidence and issue durable Manager verdicts: expected APPROVED for #196 (MapPlanning 168/168, 8 captures) and #198 (guided-v2 535/0; guided-legacy 569/0) unless newer evidence supersedes them. If #196 is approved and remains the final MFM-specific blocker, upgrade #156 and perform the exact safe integration/release work. For #199 choose the bounded production layout-repair direction: preserve protected Combined minimap/focus assertions, retain accepted PARITY-07 estimate content and central-prompt clearance, restore minimap visibility for ordinary estimate inspection, and repair the exposed 1920 competitor-list clipping; do not shorten copy or supersede assertions merely to get green. Publish/release the bounded local continuation to #203/#201, remain the sole active-tasks writer, preserve Reference Future/Legacy compatibility, and do no local Unity/build work. #194 remains NO ACTION. No Windows build.`