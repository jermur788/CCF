# Independent review dispatch — Irish Sitka 2D Reference Laboratory V1 DRAFT

**Date:** 14 November 2026  
**Reviewer task:** [#485](https://github.com/jermur788/CCF/issues/485) — READY / NOT RUNNING, no review worker registered or evidence reviewed by a second author yet.  
**Original draft author:** dedicated Multi-Rack Simulation Manager, OpenAI GPT-6 GitHub-docs-only, [#483](https://github.com/jermur788/CCF/issues/483). Reviewer **must be a different independent session/model author**.  
**Difficulty:** MEDIUM–HIGH; independent product/engineering/science honesty review, not live coding.  
**Best reviewer:** Claude Opus High (independent reasoning + GitHub read access) or a different GPT-6 High ChatGPT GitHub research session; neither is automatically assigned.  
**Research/operational boundary:** Scenario One #424 source/Unity work has priority. This review writes **one new Markdown review file** on its own branch; no \`active-tasks.md\`, source, specs, saved games, real marteloscope inputs, dataset downloads, Unity or Windows builds.

## 1. Exact four-file author candidate to review READ-ONLY

All four were added and fetched/read back on author branch \`docs/multirack-sitka-2d-reference-lab-draft-20261114-v1\`, exactly four Markdown paths, nothing else:

- [Feature draft spec](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-2d-reference-lab-draft-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md)
- [Implementation plan](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-2d-reference-lab-draft-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/plan.md)
- [Task breakdown](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-2d-reference-lab-draft-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/tasks.md)
- [Requirements checklist](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-2d-reference-lab-draft-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/checklists/requirements.md)

**Version control:** reviewer must resolve and pin the author branch HEAD **before** reviewing, verify only those four added files, and detect any concurrent author edits. No review-to-author mutation. The draft is **DRAFT**, not an implementation authorization.

**Canonical authority:** [Game Brief](https://github.com/jermur788/CCF/blob/main/Docs/Project/game-brief.md), [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) (including D-048, D-059, D-065, D-076 and D-078–D-081), [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), [Spec workflow](https://github.com/jermur788/CCF/blob/main/specs/README.md) and live [active-tasks](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md) read-only. Do **not** infer Unity candidate qualification from GitHub publication. The owner wants an eventual 2D/3D same-state multi-rack system, but Sitka-first flat-stand research must not reduce the three-species first-demo requirement of Scenario One.

**Scientific sources:** [CCF research index](https://github.com/jermur788/CCF/blob/main/Docs/Project/research-index.md) and Game Dev attached \`GROWFOR_Technical_Evidence_Extraction_v1.md\`, which describes Broad & Lynch's Irish pure even-aged Sitka stand-state \`B,N,H\` model and caveats; [#472 validation ladder](https://github.com/jermur788/CCF/blob/research/multirack-samsara2-validation-20261113-v1/Docs/Handoffs/MultiRack-Simulation-20261113/SAMSARA2-MARTELOSIM-CCF-VALIDATION-METHODOLOGY-AND-EVIDENCE-GAPS-20261113.md); [#479 real GBIF data access blocker](https://github.com/jermur788/CCF/blob/research/multirack-gbif-irish-plot-raw-audit-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/GBIF-IRISH-RAW-MARTELOSCOPE-PLOT-DATA-COMPLETENESS-AND-RECONSTRUCTION-AUDIT-20261114.md). No paper/GBIF source has yet proved an exact real Irish plot import.

## 2. Reviewer test matrix (report each criterion separately)

**R1. Product scope and ownership:** Does the DRAFT clearly distinguish a laboratory from the final 3D player experience and separate approved Scenario One? P0 cannot write Unity or old saves. No new production "authority" from this doc. Does proposed \`Tools/MultiRackSitkaLab/\` avoid other live tool files including the standalone Text Editor? No false local lane/host claim or \`active-tasks\` ownership.

**R2. Engineering viability:** Is P0 a precise, compilable/testable unit on its own—versioned synthetic input, unique IDs/XY, stable state and deterministic event history, maths, local UI, explicit execute and save isolation? Flag any underspecified implementation choice that would cause unsafe guesswork or hidden second source of biological state. Don't demand full game polish.

**R3. No-growth P0 semantics:** The user wants an accurate ecological simulator eventually, but **P0 intentionally implements NO BIOLOGICAL GROWTH** so no unverified physics are invented. Does draft make this tradeoff clear and useful? Is its annual calendar accurately labelled as replay/calendar only? Should P0 be allowed to produce “year by year consequences” only once P1 uses source-qualified biology? No simulated harvest due merely to preview/approval.

**R4. Maths/conservation:** Verify \`BA=Σπ(DBHcm/200)^2\`, N/ha, Dq, area/masks, unique tree/state identity, dead/felled handling, duplicates, stale approvals, proposal-only no-work, immutable work events, snapshot hash/save provenance. The exact P0 acceptance tests must be independently reproducible without Irish field assumptions. If formula wrong, blocker.

**R5. Forestry science limits:** GROWFOR/Broad–Lynch models **stand-level** basal area, stocking and top height for pure even-aged Sitka in a domain (age/site/thinning/history) and does **not** establish individual-tree competition, irregular continuous-cover growth, crown shape or regeneration. Source/version/limitations and 2024 sitka efficacy caveat should remain research not fiat. D-048 current CCF Sitka source requires actual current repository inspection before claiming parity; no invented P1 coefficients.

**R6. Separate data/source/rights:** No Irish marteloscope is currently demonstrated to have verified exact per-tree IDs, coordinates, DBH and plot polygon in an inspected public archive; #475 Deep Research overclaimed and #479 failed to access raw archive. Synthetic forest must always say \`SYNTHETIC\`, and raw GBIF, Teagasc or Coillte data cannot be inserted without a separate rights/data audit. No made-up ecological “typical” N/BA/DBH combination.

**R7. Upgrade and integration boundary:** Verify that P1 has genuine separate model-source/parity and evidence review before any annual predictive claims; no automatic “same engine” assertion just because a 2D prototype runs. Subsequent connected 2D+3D workflow and full five-rack sample→targets remain SPEC REQUIRED in their own scope, not silently part of P0. Preserve three-species Scenario One requirement.

**R8. Proportional tests:** Are Tier 1 headless and one actual browser/host Tier 2 sufficient for P0, avoiding heavyweight Unity/Tier 3/Windows builds? Test negative/out-of-bounds/stale plan and status differentiation. P0 should not be judged on L4 Irish predictive accuracy, which is unavailable.

**R9. Formal spec structure:** All four files exist, acceptance criteria and stop rules clear, task dependency chain meaningful; OPEN-L1..L5 are not silently ratified. Who can approve P0 must be named without improperly borrowing #424 authority. If acceptance requires material owner choices, label them. Be honest which part is immediately implementable after approval.

## 3. Required independent output and verdict

**Reviewer result branch created from main and initially untouched:** \`docs/multirack-sitka-lab-independent-review-result-20261114-v1\`. Worker must fresh-verify branch HEAD/no overlap, then register actual provider/model/branch/current starting HEAD and **worktree NONE** as **[RUNNING] on [#485](https://github.com/jermur788/CCF/issues/485)** before editing. GitHub docs only.

**Exact sole output path:** \`Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-V1-DRAFT-SPEC-REVIEW-20261114.md\` on that reviewer branch.

Required structure: source/versions inspected, severity-tagged list of actual defects with precise \`spec/plan/tasks/checklist\` references, disposition R1–R9, clear **APPROVED AS DRAFT-QUALITY / NEEDS CHANGES / REJECTED** verdict, separate recommendation whether P0 is ready for owner approval, and explicitly **NO AUTOMATIC P1 OR UNITY CODE RELEASE**. If no blockers, recommend formal P0 status approval without claiming you personally ratified it.

After writing, fetch/read back and compare exactly one new Markdown path versus registered starting HEAD; no other changes. Comment [HANDED-OFF], close #485 and notify #483/Manager with durable result URL. Dashboard/Drive **NOT VERIFIED** unless proven.

## 4. Dispatch line for an independent new chat

> Take ownership of **CCF #485**, independent review of the Irish Sitka 2D Reference Laboratory V1 four-file DRAFT. Read full packet at https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-independent-review-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-2D-LAB-V1-DRAFT-INDEPENDENT-REVIEW-PACKET-20261114.md . This is a **standalone synthetic Sitka 2D research/engineering lab, not Scenario One Unity implementation**. Check all four draft files on \`docs/multirack-sitka-2d-reference-lab-draft-20261114-v1\`, cite source authority/Irish GROWFOR limits, review data/replay/BA/status conservation and P0 NO GROWTH versus gated P1 ecology, surface any unsafe assumptions or ambiguous OPEN-L1..L5. Provide R1–R9 and verdict APPROVED AS DRAFT-QUALITY / NEEDS CHANGES / REJECTED. Register your actual worker + reviewer branch and starting HEAD/worktree NONE on #485 before writing. Publish exactly \`Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-V1-DRAFT-SPEC-REVIEW-20261114.md\` on \`docs/multirack-sitka-lab-independent-review-result-20261114-v1\`, read back and one-file compare, close claim as HANDED-OFF, then return to Manager. **No code, Unity, production saves, approved-spec/canonical/coordination edits, model runs, dataset downloads or Windows build.**
