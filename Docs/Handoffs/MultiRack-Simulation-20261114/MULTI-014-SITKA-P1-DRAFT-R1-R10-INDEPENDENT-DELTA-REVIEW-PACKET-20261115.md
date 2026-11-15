# MULTI-014 — independent focused delta review of Sitka P1 DRAFT R-1…R-10 repairs

**Issued:** 15 November 2026  
**Execution claim:** [CCF #542](https://github.com/jermur788/CCF/issues/542) — **READY / NOT RUNNING**. No actual reviewer, physical machine lease, Unity process or worktree registered.  
**Scope:** independently verify the **four-file Sitka P1 adult-rules DRAFT documentation repairs only**, against the independent MULTI-012 review and Manager ruling. Return **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED**. Neither review nor an approval can mark a SPEC approved, accept C-1…C-7 on the owner's behalf, authorise P1-01…05, permit a Unity Editor capture, make Irish field-accuracy claims, integrate the game or run a Windows Unity Player build.
**Difficulty:** **HIGH** for narrow technical accuracy — cross-runtime numerical semantics, Unity source-derived annual order, P0 approval-epoch event sourcing, cross-file gates and source ownership.
**Recommended independent reviewer:** **OpenAI Codex Sol High in a fresh local T3 Code lane**, or another capable non-GPT-6, non-Claude-Opus reviewer with GitHub/source access. The original DRAFT was GPT-6; the independent MULTI-012 R-1…R-10 review **and** MULTI-013 repair were by **Claude Opus 5.5** in separate sessions. Codex previously authored the MULTI-009 source feasibility study; this is a *different* document-repair assessment, not independent scientific replication of that earlier Codex study. Disclose actual model/provider/host and any source-history involvement.

## 1. Exact candidates and authority

**Repaired DRAFT candidate** (locked input):
- **Source branch:** \`docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1\`
- **Candidate head:** \`8756c452cf76a8dd3169f2ab40c93349cb1edc60\`
- **Pre-repair reviewed DRAFT baseline:** \`50178f9df0641a75547fbeb29033d18a69a5cbb7\` (\`docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1\`)
- **Main CCF source snapshot** used in preceding source reviews: \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`; source/Unity truth cannot be inferred solely from this historical code snapshot.
- **GitHub custody checked directly by Manager:** repaired candidate versus pre-repair baseline is **exactly FOUR MODIFIED Markdown** (\`spec.md\`, \`plan.md\`, \`tasks.md\`, \`checklists/requirements.md\` under \`specs/multirack-sitka-p1-adult-rules-comparator-v1/\`) plus **ONE ADDED handoff Markdown**, zero other source/P0/Unity/coordination changes. [MULTI-013 handoff](https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-013-P1-ADULT-SITKA-DRAFT-R1-R10-DOCS-REVISION-HANDOFF-20261115.md). [MULTI-013 #536](https://github.com/jermur788/CCF/issues/536) was registered RUNNING and is now HANDED-OFF/CLOSED.
- **Findings authority:** [original MULTI-012 independent R-1…R-10 review](https://github.com/jermur788/CCF/blob/review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md), verdict NEEDS CHANGES (0 BLOCKING, 2 MAJOR, 8 MINOR); [Manager formal disposition/repair constraints](https://github.com/jermur788/CCF/blob/docs/manager-multi-012-p1-draft-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-012-P1-DRAFT-NEEDS-CHANGES-AND-REPAIR-DECISION-20261115.md). Parent [MULTI-010 accepted evidence plan](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md) remains evidence-level only; all seven C decisions and eight older m findings remain required.

**Review all four repaired files directly (same exact candidate):**
1. [DRAFT spec.md](https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md)
2. [DRAFT plan.md](https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md)
3. [DRAFT tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md)
4. [DRAFT checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md)

**Source grounding for narrow checks:** \`Assets/ForestPrototype/ForestEcologyController.cs\` (CI, source float change commentary, annual order, regen2/mgr prerequisite, storm, mortality); \`SimulationRandom.cs\`, \`ForestTree.cs\`; [approved P0 spec](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md) and qualified P0 source at \`feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1\` (\`LabSession.cs\`, \`Models.cs\`, \`LabFile.cs\`, \`LabDataStore.cs\`, \`.gitignore\`). **P0 lab source is not present on main**. Narrow inspection only; no costly full research or Unity regression reruns. Original Broad–Lynch/GROWFOR PDFs were absent on the previous review laptop; neither this review nor earlier arithmetic implies empirical validation.

## 2. Already-created reviewer branch, allowed output and worker registration

**Separate review branch, already created and verified untouched from candidate head:**
\`review/multi-014-p1-draft-r1-r10-delta-20261115-v1\`

**Proposed LOCAL worktree, NOT CREATED or RESERVED:**
\`/media/jer/Files/CCF/Operations/CCF-MULTI-014-SitkaP1-DeltaReview-20261115\` **only if** the reviewer is actually on the Linux laptop and ownership/host availability permits. A reviewer working on Windows/WSL should choose a separate WSL ext4 worktree after checking actual ownership. A remote GitHub-only read-only source lane is also permissible if it can independently inspect the required source and verify Markdown rendering. No Unity machine-wide lock required because no runtime/editor use is authorised.

**Only tracked writable output (exact handoff path, on reviewer branch):**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md\`

**Every other path READ-ONLY**, especially all four repaired DRAFT files, original MULTI-013 handoff, \`Assets/**\`, \`Tools/**\`, old approved P0 spec and its saves, GROWFOR research, \`Docs/Project/**\` including live \`coordination/active-tasks.md\`, main, frozen Reference Future and Scenario One. Do not edit draft files to pass review, reset/stash/clean/delete other workers' files, change Git safeties, terminate Ubuntu/Unity processes, or run a game/Player build.

**Before inspecting/testing/writing:**
1. Fresh-read this full packet and [#542](https://github.com/jermur788/CCF/issues/542), source handoff, original R findings and Manager decision. Refresh **live** coordination board and inspect actual worktree/host/branch availability; ensure the review branch starts at exact pinned repaired DRAFT and has no other writers.
2. Register **real reviewer provider/model, machine, worktree (or explicit GitHub-only no physical worktree), review branch, full starting HEAD, source snapshots and allowed read-only phase** as **[RUNNING] on #542 BEFORE source inspection, tests or report edits**. The issue does not grant board file ownership. If a conflict, report [BLOCKED], do not absorb dirty work.
3. The original draft author GPT-6 and the Claude Opus reviewer/repair author may provide source records but **must not perform this delta review**. Codex's historical LAB-06 research involvement is disclosed, and no external scientific revalidation is inferred from this delta.

## 3. Focused independent matrix R-1…R-10 — evidence, not a checklist rubber stamp

Grade each finding **PASS / FAIL / PARTIAL** with precise DRAFT file/section/line and source/review evidence, and severity of residual defect. If a correction creates a new material contradiction, include it even if outside an individual item.

### R-1 MAJOR — Unity captured oracle, C-2a/C-2b and no circular preconditions
- Does every relevant file treat **actual per-ID CCF Unity Editor (Mono) output from a pinned source run** as the *reference of record*, not \`float32\` language type or reimplemented \`.NET\` expressions? A pure .NET/CoreCLR harness is allowed **only after** verifying it reproduces the *same* Unity vectors; it cannot replace the initial capture.
- Is C-2a (units, raw float/double bits, per-ID/model/seed/year/source/Editor metadata, serialization and display policy) required **before** the oracle, while C-2b (bit parity when feasible, else independently reviewed tolerance/branch-margin policy) is required only **after** Unity capture and before P1-02 acceptance?
- Inspect conditional numeric branch/victim death, \`roll < p\`, 8 m radius inclusion, suppression ties, relative density thresholds, ordinal ID ordering and exact cause/year. **The mortality integer RNG hash is portable; the float-sensitive inputs are not**. Keep 94/153 and 981/1893 *scratch* differences labelled NOT Unity runtime observation.
- Owner-level *future* decision on short exclusive Unity lane must be explicit and must not be presented as already booked or prioritised over full three-species Scenario One.

### R-2 MAJOR — save/load approval binding vs biological/event-chain equality
- Does C-1A specify an **owner choice** between retaining P0 session-epoch approval binding and stale-on-load event policy, versus a distinct independently reviewed P1 approval policy?
- Does every file avoid demanding identical **raw canonical event hashes** for different actual event histories? Test **Mode A** (no open approval/pending-work plan at split) and **Mode B** (open plan, P0-style \`PlanStale(SessionLoaded)\` per affected plan). Verify that biological state, ecological year, work/harvest/mortality ledger and roll inputs are invariant under legally equivalent actions, but approval-history events may differ. No "P0 keeps approval active" workaround.
- **Specific residual watchpoint surfaced by Manager's direct revised-DRAFT readback:** The repaired text sometimes requires \`identical canonical hashes, allowing only for an approved, named load event\`. A recorded load event ordinarily alters a hash chain. Require an explicit *canonical projection/normalization* definition or separate histories before calling hashes equal; don't waive a material contradiction as merely editorial. Check \`spec.md\` §4, \`plan.md\` §5, \`tasks.md\` §4 and REQ-E06 against each other. If unresolvable without a product policy choice, label **NEEDS CHANGES** and give the minimum narrow doc repair.

### R-3 MINOR — Markdown and diagrams
- Verify actual **GitHub/GFM rendered** spec, plan, tasks and requirements have working inline code, plan \`\`\`text fenced diagram and tables; no literal backslash-escaped backticks outside code. Reviewer may use GitHub Markdown API or a local renderer; report actual result, not inferred.
- Reviewer must not alter the historical MULTI-011 handoff merely to make its code spans pretty.

### R-4 MINOR — gate crosswalk
- Unambiguously distinguish **plan G0 = completed evidence-plan review** from **P1-G2 = future product/spec approval (plan G2)** and **P1-S0 = G1**, P1-01..05 = G3..07. Avoid old **P1-G0** references that would imply conflicting status.
- **Specific potential residual found during Manager's direct readback:** \`checklists/requirements.md\` REQ-F09 still appears to say “no code before P1-G0 SPEC/product approval” although the approval gate was renamed P1-G2. Confirm against exact candidate (not prior memory) and classify severity/minimum repair. Do not change the file in this review.

### R-5 MINOR — decision owner/timing without premature acceptance
- For C-1 (including C-1A), split C-2a/C-2b, C-3/C-4/C-5/C-6/C-7, is the required decision **before capture / after capture / before P1-02 / before P1-03 / before P1-04 / before UI** clearly recorded across files?
- C-1A and C-6 are owner-level (fork/approval policy, Unity scheduling), C-7 wording needs owner approval; technical choices require independent review and later approved spec. C-2b must not be a P1-01 prerequisite.

### R-6 MINOR — stale schema23 verifier
- The old schema23 assertion in a historical Sitka verifier is a **separate optional/conditional** source verification repair. It gates new P1-01 oracle only if that particular verifier is reused, not any new dedicated capture. Never weaken the test to declare green.

### R-7 MINOR — valid narrow Unity capture configuration/causality
- Require valid regeneration manager/model configuration (regeneration model2 throws without manager), no active recruit/promotions via spawner, storm0, correct ecologicalYear-before-mortality-roll vs P0 inventoryYear mapping, fixed RNG model+seed and source growth model2/pure-Sitka Reference demand. What is independently evidenced and what is future proposed? An actual Unity A/B configuration check is a **future required test**, not currently passed.
- Verify **pre-growth CI** drives suppression, **post-growth RD** drives density hazard, with DBH/height/crown source clamps and exact event order. No Unity testing/scene edit is permitted during this review.

### R-8 MINOR — author cannot self-approve
- DRAFT author Overall Manager coordinates review but **owner explicit product decision plus independent evidence** are prerequisites to marking the four-file spec APPROVED. No model/issue author can indirectly issue P1 code authorisation through wording alone.

### R-9 MINOR — actual P0 branch lineage and shared files
- The qualified standalone P0 lab is on a feature/qualification branch and **not in main**; a future P1-02 candidate has to choose an exact P0 integration lineage/base. Proposed ignored \`.lab-p1/\` under P0 would touch shared \`.gitignore\`, requiring ownership or alternative isolated root. No automatic main merge/assumption.

### R-10 MINOR — verified editorial fixes
- The independent repair author says \`current-CFF\` occurred at the exact original source blob and was fixed; Manager's earlier negative assertion was not a reliable check. Verify exact revised text says \`current-CCF\`; treat original documented blob claim on its merits, no additional false edit.
- Crosswalk \`DRAFT\` vs \`OPEN\` status definitions distinguish unapproved proposals from unresolved choices. P0 \`oracle-quarter-ha\` 50 m square (corner-coordinate) fixture is a possible re-centred source-parity input, not an already runtime-qualified Unity fixture.

## 4. Verdict and exact publication

Return a **separate reviewed conclusion**:
- **APPROVED AS DRAFT QUALITY**: repairs adequately address all ten documented issues (and no new major inconsistency); it is suitable for *further Manager/product-owner decisions*, **not** approved model specification, Unity, game development or code.
- **NEEDS CHANGES**: state each exact remaining defect with severity, source file/line, smallest docs-only repair and no unnecessary source-test scope increase.
- **REJECTED**: material unrecoverable confusion/source-science misrepresentation; describe a different product framing. Do not use REJECTED for a narrow correctable typo.

Report must include real reviewer/source/host/claim/worktree metadata, full source refs, comparison findings R-1…R-10, independent Markdown GFM verification, m-1…m-8 preservation, C-1…C-7 still OPEN and owner-level scheduling/approval gates. Separate **directly observed** vs **source-derived** vs **scratch arithmetic** vs **untested**. Source PDFs and Irish empirical validation not requalified by this review.

**Only new tracked handoff:** \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md\` on the reviewer branch. After publication, GitHub **fetch/readback** and compare branch to exact repaired DRAFT candidate \`8756c452cf76a8dd3169f2ab40c93349cb1edc60\`: **exactly ONE ADDED Markdown report** and **ZERO changes** to repaired DRAFT or any source. Register [HANDED-OFF], close [#542](https://github.com/jermur788/CCF/issues/542), notify Manager #425, original independent reviewer #531 and repair #536. Return exact report URL and strongest outstanding decision. Manager then issues APPROVED / NEEDS CHANGES / REJECTED; *separately* product-owner C-1A/C-6/C-7 and future Unity scheduling decisions remain.

**Dashboard/Drive:** NOT VERIFIED; no mirror/context regeneration, Decision Log or live coordination writes.

## 5. Paste-ready dispatch to independent Codex

> Take ownership of **MULTI-014 / CCF #542** — **independent focused delta review of the repaired four-file Sitka P1 adult-rules DRAFT**. Follow the full GitHub packet: https://github.com/jermur788/CCF/blob/docs/multi-014-p1-draft-delta-review-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-SITKA-P1-DRAFT-R1-R10-INDEPENDENT-DELTA-REVIEW-PACKET-20261115.md . **Difficulty HIGH; recommended OpenAI Codex Sol High in a new T3 Code session.** Original GPT-6 authored the DRAFT, Claude Opus 5.5 both reviewed and repaired it, so you must be independent of those authors. You previously authored LAB-06 research; disclose that and independently verify this documentation delta without claiming independent validation of your own forest research. The review branch \`review/multi-014-p1-draft-r1-r10-delta-20261115-v1\` is **created but untouched** from repaired candidate \`8756c452cf76a8dd3169f2ab40c93349cb1edc60\`. Proposed **uncreated, unreserved** laptop worktree \`/media/jer/Files/CCF/Operations/CCF-MULTI-014-SitkaP1-DeltaReview-20261115\` only if actually on a free laptop; otherwise choose separate owned WSL-ext4 or GitHub-only read-only lane after live board/host preflight. Register **actual model/provider/host/worktree/branch/full starting HEAD** [RUNNING] on #542 before source inspection/test/report, BLOCKED on conflict. Read the four revised DRAFTs, original R-1…R-10 report, Manager ruling and repair handoff, verify all ten fixes with source evidence and GitHub Markdown rendering. Pay special attention to **Mode A named-load-event vs raw hash equality** and possible **REQ-F09 old P1-G0** gate reference. C-1…C-7 remain OPEN; **no P1 code, Unity session, spec/product approval, game integration or Windows game build**. Publish only \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-P1-ADULT-SITKA-R1-R10-INDEPENDENT-DELTA-REVIEW-20261115.md\` on reviewer branch, verify GitHub **one added Markdown**, close #542 HANDED-OFF and return an independent **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** verdict with minimal corrections if needed.
