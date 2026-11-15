# MULTI-013 — P1 adult Sitka DRAFT documentation repair (R-1…R-10)

**Issued:** 15 November 2026  
**Execution claim:** [#536](https://github.com/jermur788/CCF/issues/536) — **READY / NOT RUNNING**. No actual worker, physical host lease, worktree or source-edit permission is registered.  
**Manager decision:** [MULTI-012 NEEDS CHANGES ruling](https://github.com/jermur788/CCF/blob/docs/manager-multi-012-p1-draft-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-012-P1-DRAFT-NEEDS-CHANGES-AND-REPAIR-DECISION-20261115.md) — 0 blocking, 2 major, 8 minor.  
**Independent reviewer:** [MULTI-012 Claude Opus 5.5 report](https://github.com/jermur788/CCF/blob/review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md); [#531](https://github.com/jermur788/CCF/issues/531) HANDED-OFF/CLOSED.  
**DRAFT author:** OpenAI GPT-6 Overall Manager [#528](https://github.com/jermur788/CCF/issues/528), HANDED-OFF/CLOSED.  
**Difficulty:** **MEDIUM–HIGH**, because edits are confined to four Markdown files but concern actual Unity float semantics, deterministic event/approval replay, model versioning, shared file ownership and source/experiment claims.  
**Best-suited worker:** **OpenAI Codex Sol High in local T3 Code** for source-literate, cross-file documentation repair, or another capable, separate docs writer. Original reviewer was Claude Opus; original DRAFT author GPT-6. No particular provider owns ecology.  
**Authority:** The owner approved writing a P1 **DRAFT only** on 15 November. MULTI-010 approved only a P1 evidence *plan*. **No P1 spec, P1-01 runtime capture, LAB-07 coding, Unity session, game integration or Irish field-accuracy claim is authorised by this task.**

## 1. Exact input and editable output

**Original DRAFT to repair (pin):**
\`docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1\` at \`50178f9df0641a75547fbeb29033d18a69a5cbb7\`.

**Pre-created, untouched, writable docs repair branch (same exact initial commit):**
\`docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1\`.
Manager independently compared it against the DRAFT source: **zero ahead/zero file diff**. This branch is *not* based on the review branch and must not absorb the review report or unrelated main updates.

**Exactly four allowed existing file edits:**
1. \`specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md\`
2. \`specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md\`
3. \`specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md\`
4. \`specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md\`

**Exactly one allowed new report:**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-013-P1-ADULT-SITKA-DRAFT-R1-R10-DOCS-REVISION-HANDOFF-20261115.md\`

**Every other path READ ONLY**, including original MULTI-011 handoff, review report, \`Assets/**\`, \`Tools/**\`, P0 lab branch/source/tests/fixtures, GROWFOR/Research, \`Docs/Project/**\` including LIVE \`coordination/active-tasks.md\`, original approved P0 four-file spec, frozen Reference Future, game saves, dashboard and Drive. No merge, source code, Unity run, Windows game build, filesystem cleanup or machine-wide settings. No editing the reviewer’s worktree. All changes preserve **DRAFT / NOT APPROVED** labels, and all C-1…C-7 remain OPEN (except the technical substep timing is clarified without adopting final policy).

## 2. R-1 and R-2 — must be fixed before delta-review approval

### R-1 MAJOR: numerical reference, two-phase C-2 and Unity scheduling

**Source/report facts** (do not confuse with runtime qualification):
- CCF \`ForestEcologyController.cs:990–993,1021–1023\` documents hash/float changes with apparently equivalent source refactors under Unity Mono. Browsing harness and Scenario Two also document compiler-sensitive floating-point paths. These are actual source observations on pinned CCF main, not a newly executed Unity test.
- Reviewer independently probed two *plausible* floating-point evaluation behaviours on synthetic P0 fixtures: **94/153** demo and **981/1,893** performance first-year DBH increment bit results differ. **Neither probe mode is verified Unity output**. The source integer mortality RNG roll (\`SimulationRandom.Roll\`) is portable across runtimes when all inputs/version match.
- **Therefore a pure CoreCLR/.NET 8 harness, even when source-equivalent, is not an independent substitute for actual current-source Unity Editor runtime capture.** It can be cross-checked after Unity has established a reference.

Repair **all applicable sections**:
1. Explicitly name **pinned Unity Editor/Mono per-ID output capture as the reference of record** for P1-01. No code or Unity run under MULTI-013. If using a .NET speed-up harness, qualify it against the **same capture vectors** before trusting its results.
2. Split unresolved **C-2** into **C-2a** (before capture: units, exact source inputs, serialization and display policy, *raw float-bit recording in the oracle format*, ID/year/model/RNG/seed/source version fields) and **C-2b** (after Unity capture: independently reviewed parity target — exact bits when actually achievable, else tolerances **with explicit branch margins, identical death/plan/selection IDs and reproducible event policy**). C-2b **cannot** be deemed resolved before P1-01; remove contrary dependencies.
3. Rewrite the recommendation \`source-faithful binary32 = exact parity\` as a **hypothesis, not a proven guarantee**. Require the target implementation to preserve the source’s mixed float32/double steps *as far as verified* without claiming that language data type fixes compiler evaluation order. Separate pure-integer RNG bit roll from float-sensitive CI/DBH/height/crown/hazard thresholds.
4. **Owner-level future scheduling gate** for an exclusive, bounded Unity Editor capture lane, which shares finite machine/owner resources with the primary **three-species Scenario One** milestone. The spec is not authorised to reserve or execute Unity; the user can defer the lab and keep P0 intact.
5. Update \`spec.md\` §3/§5/§6/decision table/AC-P1-02–03, \`plan.md\` G2/G3/numeric tests, \`tasks.md\` P1-01/dependencies and \`checklists/requirements.md\` REQ-C01–C03/F01–F03, the decision register and all cross-file mentions. No major new theory or code.

### R-2 MAJOR: split-run biological equivalence versus approval epoch event history

**Source fact:** Existing P0 \`LabSession.cs:75–85\` marks every open APPROVED/PENDING_WORK plan **STALE on load** via a recorded \`PlanStale(SessionLoaded)\` event and a new epoch. In a no-restart run, its stale cause occurs at the later step. Therefore, with an open approval at save split, the **raw complete event hash differs even when subsequent biology is the same**. Do not promise impossible identical event hashes by comparing different event histories.

Repair in all applicable files:
1. Distinguish **biological and work outcome equivalence** (same per-ID biological state, ecological year, seed/model versions, expected mortality/fell IDs/causes/roll inputs and source provenance after equivalent legal actions) from **raw event-chain hash equality**, which holds only for an *identically ordered event history*.
2. Define two concrete test modes: **Mode A** save with **no open APPROVED/PENDING_WORK plan**, require same biological state and comparable canonical event hash if actual P1 event design produces same event stream; **Mode B** save with open plans, if P1 inherits P0 semantics, require one exact \`SessionLoaded\` stale event per affected plan and compare documented lifecycle effects, not identical raw hashes to an uninterrupted run.
3. Add **C-1A explicit OWNER choice**: whether P1 reuses P0's **session-epoch** approval binding and mandatory load-staleness, or defines a distinct explicitly reviewed P1 approval policy. P0 itself remains unchanged. No silently "persisting approval" as a fix.
4. Rewrite \`spec.md\` AC-P1-05 and C-1A, \`plan.md\` replay §5, \`tasks.md\` §4 negative cases and \`requirements.md\` REQ-E06/E03. Each must align; no uncompensated assertion that both whole-event hashes match under open-plan restart.

## 3. R-3…R-10 — narrow, cross-file documentation repairs

| Finding | Mandatory DRAFT-only disposition | Concrete proof |
|---|---|---|
| **R-3 — Markdown rendering** | Remove literal escape backslashes before Markdown backticks in all four files. Restore actual inline code and fenced \`\`\`text diagram in plan. Do not accidentally alter formula/evidence meanings. | Render-check plan diagram and representative inline code with GitHub/GFM or trustworthy equivalent; include observed evidence, not merely grep |
| **R-4 — gate IDs** | Eliminate completed plan **G0** versus blocked task **P1-G0** collision. Suggested: **P1-S0 = plan G1; P1-G2 = plan G2; P1-01…05 = plan G3…G7**. Replace all old references throughout four files consistently. | Search all \`G0\`/\`P1-G0\` and verify completed evidence remains different from awaiting spec approval |
| **R-5 — C decision owners/timing** | Add specific table distinguishing owner-level choices from technical-spec details, and **when** each is required. C-1A and C-6 require future owner decisions (the latter reserves scarce Unity time); C-2a before P1-01, C-2b after Unity reference; C-3/C-4 and geometry C-5 before fixtures; SI/H C-5 before B/N/H comparison; C-7 before UI/export and approved by owner. | All seven remain OPEN and do not form circular P1-01 prerequisites |
| **R-6 — stale verifier repair** | The old schema-23 assertion is a *separate* optional/conditional source-verification repair; not a compulsory part of P1-01. It gates **only if** reusing that harness. Do not weaken tests and do not edit the harness here. | Plan/tasks/spec/requirements correctly separate old verifier repair ownership from new oracle |
| **R-7 — Unity config and causal ordering** | Specify a bounded source capture configuration for closed adult rules. Regeneration model2 throws without \`ScenarioOneManager\`; explicitly select a valid source/scene manager/model route. Prevent spawner recruitment, set storm model0, map controller \`ecologicalYear\` (incremented before RNG roll) to P0 \`inventoryYear\` without assuming 2026 is ecology year, pin RNG seed/version; demonstrate source configuration is adult-output neutral with a scoped A/B. CI is **pre-growth**, RD is **post-growth**; reword REQ-C09 accordingly. Include clamps. No execution in this task. | Testable configuration/negative conditions and owner gate before any actual Unity capture |
| **R-8 — independent approval** | Manager who drafted may coordinate/issue verdict but cannot make spec executable through self-approval; **independent spec delta review plus explicit product-owner approval** required before implementation. | G2/decision register and handoff say so plainly |
| **R-9 — actual P0 branch and shared ignore** | Current \`main\` source snapshot does **not** include \`Tools/MultiRackSitkaLab\`. Name independently qualified P0 source branch/lineage and need to decide integration/P1 base. A new \`.lab-p1/\` ignore entry would touch shared **P0-owned \`Tools/MultiRackSitkaLab/.gitignore\`**; that path needs explicit ownership or a separate-path design. | No assumed P0-in-main; no P0 shared edit authorised |
| **R-10 — editorial** | Define DRAFT vs OPEN in crosswalk; note current P0 \`oracle-quarter-ha\` is 50 m square in corner coordinates, potentially re-centre for source-parity fixtures. Reviewer reported \`current-CFF\`, but **Manager direct GitHub readback of the pinned DRAFT shows \`current-CCF\` already**: verify exactly before editing and do not invent a typo. | Original content preserved if already correct; other verified slips repaired |

**Do not reopen already qualified topics** without a concrete new defect: the project authority, adult Sitka source equations, distinction between source growth model2 and the pure-Reference-demand model1 route, GROWFOR stand-level-only B/N/H and no Irish individual forecast, P0 immutable/schema1/calendar-only, and distinct P1 saved-state concept are supported by the independent review. Prior m-1…m-8 remain addressed except the superseded m-4 precision recommendation. Preserve exact fixture/population figures as *synthetic reviewer arithmetic*, not game runtime results.

## 4. Worker registration, safety and proof

**Actual local worktree:** **NONE assigned or created** by the Manager. If local T3 Code worker chooses the owner's Windows machine, work inside a **new private WSL ext4** checkout if free; if laptop, choose an unused independent docs worktree after reading live ownership. No global git setting, \`safe.directory\`, WSL configuration, compiler or Windows/Unity build needed. A GitHub-only docs lane is also suitable if it can inspect source and render Markdown. Record actual worker/provider/model/host/branch/worktree and starting full HEAD.

Before edits, fresh-read:
- this full packet, [#536](https://github.com/jermur788/CCF/issues/536), original full four-file DRAFT and independent R-1…R-10 report;
- live \`Docs/Project/coordination/active-tasks.md\`, current worktrees and owned paths; actual pinned repair branch head.
- register a **[RUNNING]** issue comment **before** inspecting source/running scratch verification/editing; if a collision, post **[BLOCKED]**, stop. Claim indicates execution presence only; no live board write delegated. Do **not** reset/stash/clean another worker's work or interrupt Unity processes.

**Test and publish:** read back all four edited Markdown documents and a single new handoff; ensure **exactly FOUR MODIFIED + ONE ADDED Markdown** against \`50178f9df0641a75547fbeb29033d18a69a5cbb7\`, no other tracked edits. Worker report must include a per-R finding crosswalk, exact file/section changed, the two repaired major tests, C-1…C-7 status/timing/owners, actual Markdown rendering observation, the source and numerical evidence limitations, and this task's explicit **DRAFT / NO CODE** status. Mark #536 **[HANDED-OFF]**, close and notify [#531](https://github.com/jermur788/CCF/issues/531) and [Manager #425](https://github.com/jermur788/CCF/issues/425). Do not declare independent reviewer approval before a separate R-1…R-10 delta review.

**No Windows Unity Player build. No P1 source coding or runtime/field-accuracy verification.** Dashboard/Drive mirror sync NOT VERIFIED; no canonical context/decision edit under this task.

## 5. Paste-ready dispatch

> Take ownership of **MULTI-013 / CCF #536** — repair the four-file Sitka P1 adult-rules **DRAFT specification, DOCS ONLY**, for independent MULTI-012 R-1…R-10. Complete binding packet: https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-repair-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-013-SITKA-P1-DRAFT-R1-R10-DOCS-REPAIR-PACKET-20261115.md. **MEDIUM–HIGH difficulty; recommended OpenAI Codex Sol High / T3 Code or equivalent source-literate documentation editor.** Existing untouched repair branch \`docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1\` is from exact original DRAFT \`50178f9df0641a75547fbeb29033d18a69a5cbb7\`, not the reviewer. Choose a new privately owned worktree after fresh live coordination and host/branch preflight; no location/lock reserved by Manager. Register **actual provider/model/host/worktree/branch/full starting HEAD as [RUNNING] on #536 BEFORE inspection/edit**. Update ONLY the four existing files under \`specs/multirack-sitka-p1-adult-rules-comparator-v1/\` and add ONLY \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-013-P1-ADULT-SITKA-DRAFT-R1-R10-DOCS-REVISION-HANDOFF-20261115.md\`. Major R-1: use **actual pinned Unity Editor capture as parity reference**, split C-2a pre-oracle vs C-2b post-Unity, .NET cross-check only, note Unity scheduling needs future owner approval. Major R-2: restart comparison must distinguish biological/causal equivalence from complete hash equality and specify P0-style \`PlanStale(SessionLoaded)\` when approvals are open; add C-1A owner choice. Fix R-3…R-10 including Markdown backticks/diagram, G0 labels, decision owners/timing, optional stale harness repair, oracle regen/storm/year setup, independent+owner spec approval, P0-not-on-main and \`.gitignore\` shared ownership, true editorial findings. **All C-1…C-7 remain OPEN and no LAB-07/P1-01…05 source coding/Unity run is authorised.** Verify GitHub render and **exactly four MODIFIED Markdown plus one ADDED handoff** against pinned DRAFT, report/close #536 HANDED-OFF and return for focused independent delta review. No P0/Unity/source/spec-other/coordination/Drive modifications or Windows game build.
