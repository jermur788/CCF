# Sitka annual tree-growth and natural-mortality simulation — ten-finding documentation repair packet

**Issued:** 15 November 2026.  
**Claim:** [#555](https://github.com/jermur788/CCF/issues/555) — **READY / NOT RUNNING**; no worker, host, worktree or Unity machine lease assigned.  
**Task in plain English:** Correct ten source-informed faults in the **four-file DRAFT design for the standalone Sitka spruce annual growth and natural-mortality laboratory**. **Docs only**; no code, Unity, P0 lab or game change.  
**Difficulty:** **HIGH**, because four major defects affect actual supported stand sizes, file integrity trust, timing-sensitive mortality inputs, and approval/event replay; six smaller faults affect capacity, crown behaviour, no-op years, hashing, forestry top-height and exact RNG identity.  
**Recommended author:** original design worker **Codex Sol High / local T3 Code** or equivalent source-literate specialist, one primary documentation writer. A **different independent reviewer, preferably Claude Sonnet 5.5 High**, should assess the repaired delta. The original #552 author was Codex Sol High; the #554 reviewer Claude Opus 5.5 wrote parts of earlier baseline text. **Neither the author nor that Opus findings reviewer should sign off the focused delta.** State actual identities.

## 1. Authority and exact immutable sources

**Owner instruction:** Continue the design work. This authorises documentation, NOT selecting final technical or owner choices, Unity scheduling, source implementation, or game integration.

**Source of repair (immutable):** \`61e53074bd5593603d3c7ab324eca1c1487ac9dc\`, branch \`docs/sitka-annual-growth-design-resolution-draft-20261115-v1\`, containing #552 four-file DRAFT and one new author handoff. [Original author handoff](https://github.com/jermur788/CCF/blob/61e53074bd5593603d3c7ab324eca1c1487ac9dc/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-AND-MORTALITY-FOUR-FILE-DESIGN-REVISION-HANDOFF-20261115.md).

**Binding independent findings:** [#554 report, 4 major, 6 minor](https://github.com/jermur788/CCF/blob/a6bf2d01ff4ce53e63796406c89323dfc9fb2099/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-AND-MORTALITY-INDEPENDENT-DESIGN-REVIEW-20261115.md). Its review branch has only ONE ADDED Markdown against #552 source; issue #554 HANDED-OFF/CLOSED. Reviewer source text pins: CCF \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`, qualified original no-growth 2D lab \`2659db99e317d4aa93b51cf97131a61eab5fdeec\`. These are **historical source snapshots, not present-day Editor results**.

**Binding Manager ruling:** [NEEDS CHANGES — 0 BLOCKING, 4 MAJOR, 6 MINOR](https://github.com/jermur788/CCF/blob/docs/manager-sitka-annual-growth-ten-finding-disposition-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-ANNUAL-GROWTH-DESIGN-REVIEW-NEEDS-CHANGES-20261115.md). The Manager re-read the independent report, publication delta and select CCF/P0 code: the allowed square geometries, lazy competition getter, mortality cache removal, source P0 catalog hashing and P0's conditional plan-stale events are directly source-supported. The six minor findings are reviewer/source-derived or proposed estimates, not measured runtime facts.

**No new owner decision is required for the correction pass.** Retain all seven C-1…C-7 decisions and subchoices C-1A/C-2a/C-2b **OPEN**, plus exact future decision owners/gates. No interpretation of a proposed choice becomes approved because its document is more complete.

**Game milestone priority:** complete three-species Sitka/Oak/Beech Scenario One with repeated interventions and multi-scale forestry remains the first external demo. No Windows Player build before user personally playtests the candidate and explicitly authorises it; no Unity machine time for this task.

## 2. Exact branch, ownership, allowed writes and handoff

**Pre-created clean repair branch:** \`docs/sitka-annual-growth-ten-finding-draft-repair-20261115-v1\`, created from exactly \`61e53074bd5593603d3c7ab324eca1c1487ac9dc\` with initially **0 commits/differences ahead**. Do not use the independent review branch as edit baseline. Do not merge main or absorb any unrelated dirty change.

**Physical worktree:** NOT CREATED / NOT ASSIGNED / NOT RESERVED. On the Linux laptop, choose a new private Files/ext4 worktree after live coordination/host/worktree preflight, or a separate WSL-ext4 worktree if on Windows. GitHub-only docs editing acceptable if source can be inspected exactly and publication verified. No Unity lease or source machine lock is requested.

**Only four existing files may be modified**, in \`specs/multirack-sitka-p1-adult-rules-comparator-v1/\`:
1. \`spec.md\`
2. \`plan.md\`
3. \`tasks.md\`
4. \`checklists/requirements.md\`

**Only added tracked file permitted (exact durable report path):**
\`Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-GROWTH-MORTALITY-FOUR-MAJOR-SIX-MINOR-DRAFT-REPAIR-HANDOFF-20261115.md\`

**Everything else READ-ONLY.** In particular \`Assets/**\`, \`Tools/**\`, P0 fixture files, \`MANIFEST.sha256\`, P0 \`.gitignore\`, previously published handoffs/reviews/approved specs, \`Docs/Project/**\` including live board, Decision Log and milestone, Context, reference/production/user saves, main, dashboard/Drive. No fixture bytes, new test programs, source-code edits or Windows build.

**Before inspecting source/doing checks/editing:** (i) fresh-read task #555 and this full packet; (ii) fresh-read **LIVE** \`Docs/Project/coordination/active-tasks.md\` plus physical worktree listings/host and actual branch pin; (iii) confirm no different writer owns files, no dirty worktree is about to be overwritten; (iv) post actual **provider, model, host, worktree or explicit GitHub-only none, branch, starting full HEAD, phase and allowed files as [RUNNING] on #555**. Claim records presence but does NOT grant file/Unity ownership. On conflict, post [BLOCKED] and stop; do not reset, stash, clean, steal a lock or kill a process.

## 3. Four MAJOR corrections — source-contract specificity and falsifiable acceptance

### Major A — game geometry supports only 40 m/80 m, and P1 needs its own capture fixtures

Source evidence: \`StandGeometryModel.cs:32,34\` permits only Legacy40 (40 m, **0.16 ha**) and Enlarged80 (80 m, **0.64 ha**); \`ForestEcologyController.ApplyStandGeometry\` is the route, rejects unknown versions; the Editor override accepts known versions only. A 50 m /0.25 ha game-source stand **cannot** be configured on that source without production changes. Do **not** plan to use reflection or production changes as a sneaky workaround.

**Repair across spec §4.1/§5.1/geometry and plan §9 N05, tasks and REQ-D11:**
- Separate the immutable **original verified P0 fixture import** from a **P1-owned *derived capture-fixture* catalog** for new source-legal square geometry. P1-owned fixtures are designed/test fixtures, NOT a claim they are original P0 inputs, and are not added to P0 fixture directory. Both paths need a clearly reviewed trust anchor and version/provenance, without inventing bytes or hashes now.
- Original P0 \`oracle-quarter-ha\` is still 50 m/0.25 ha in P0. If its three trees at ±15 m/centre are re-centred into a proposed **Legacy40 40m/0.16ha** derivative, explicitly mark **different source area and separate derived fixture identity**. This is sparse layout/height/wiring control, **not 50m area parity**, not a density-mortality qualification. \`N05\` positive case and \`REQ-D11\` MUST use source-legal area and present the origin transformation honestly.
- Specify new independently owned dense/threshold/8m/ID-edge/singleton source-legal **40m** fixtures and optional **80m** comparators, pending later implementation approval. Source \`Enlarged80\` may apply the approach-path algorithm; future capture must verify no unexpected hand-placed tree move/exclusion. Neither new test fixture nor output file is actually created in this task.
- Do not falsely say the original-only import restriction implies new P1 derived capture fixtures cannot exist. They are separate schemas/pathways and cannot be written back to P0. Check the design's geometry, area/XY→x/z conversion and original source-area f32 computation consistently.

**Pass test:** every proposed positive parity capture uses supported 40m/80m and correct 0.16/0.64ha source area, no 50m source run; N05 rejects impossible size, hidden resize and spurious polygon equivalence; P1 capture-fixture location, identity and trust policy named without touching P0.

### Major B — trusted original fixtures need an independently pinned anchor

Direct P0 source evidence: \`FixtureCatalog.LoadFrom\` hashes arbitrary loaded fixture bytes and does **not** read or enforce the co-located \`fixtures/synthetic/MANIFEST.sha256\`; that manifest can be forged together with a fixture. No runtime Git installation may be assumed.

**Repair spec §4.1, plan N01, requirements REQ-B08, tasks:**
- Propose a **P1-owned, reviewed trust table** in source or packaged immutable/configuration data **not read from the imported P0 data directory**. Each record binds original \`fixtureId\`, complete original **file SHA-256**, **canonical P0 snapshot hash** and a separately pinned P0 repository revision used to prepare the table. The revision is provenance **metadata**; runtime validates data against embedded reviewed values, not a live Git query.
- State which independently qualified process/owner checks and publishes that table before source deployment, and how version/new fixtures require separate review. Derived P1 source-capture fixtures have a distinct trusted P1 catalog and approved own schema; cannot masquerade as original fixtures.
- In N01 explicitly test attacker changes BOTH fixture AND co-located manifest consistently: still refuse because embedded P1 anchor does not change.
- Keep honesty: SHA-256 comparison establishes integrity relative to trusted anchor, **not authorship signature**, authenticity of a forest or measured Irish tree data. Do not invent hash values in docs; label pending exact verified values.

**Pass test:** no path using an unverified same-directory manifest as trust root; original immutable P0 fixture import, separately derived P1 fixtures, mismatched snapshot/file hashes and both-forged cases have named refusals.

### Major C — time the capture of growth-driving competition values before mortality

Direct source: \`GetCompetitionIndex\` lazily calls \`UpdateCompetition\` when \`competitionCurrent\` is false (\`ForestEcologyController.cs:1811–1819\`). \`OnTreeMortality\` removes victim cache entries and invalidates competition (\`:429–443\`). A getter AFTER \`AdvanceOneYear\` can return **post-growth/post-death recalculated CI**, not actual pre-growth CI driving growth and S. Victim cache values can be lost entirely.

**Repair source-capture contract C-2a, spec §5.1, plan future A/B/N03, requirements REQ-C13 and task capture:**
- Pin time-specific observation points: before annual advancement, get/record per-ID pre-growth CI under actual source competition path, fixed storm=0, no intervening action or state change; explicit forced cache-update/recompute step must be reviewed so it cannot alter results or source Mono floats. Store immutable per-ID CI and required S at actual evaluation (S must **not** be reconstructed from post-death tree/getter). Record pre-growth values for eventual victims.
- Cross-check survivors' retained cache entries after the year directly **without calling lazy public getter**, and require raw-bit agreement with captured preadvance CI when source ordering expects identity. If source cache is absent or invalidated without access, the capture should fail closed/mark unavailable, not invent a value.
- Include a future **uninstrumented same-Editor control** proving that observing intermediates did not change 80-year/one-year source output; the exact method/instrumentation is future source-owner approved. Be explicit if direct source access or targeted capture instrumentation is required; this doc task has no Unity execution or production code changes.
- Add N03 deliberate negatives: (1) getter after a mortality year yields different post-death CI, (2) dead victim's S reconstructed instead of captured, (3) missing pre-step value incorrectly serialized as zero, (4) instrumented-vs-uninstrumented mismatch.

**Pass test:** observation time/value ownership is unambiguous; no claim of verified bits or capture feasibility before actual future Unity experiment.

### Major D — make approval-on-load proposal faithful to actual P0 or name an alternative

Direct P0 source \`LabSession.FromFile\` creates a new session epoch, then appends **only** conditional \`PlanStale(SessionLoaded)\` events for currently open Approved/PendingWork plans. With none open, P0 appends **NO** event. Existing proposed §5.2 adds an unconditional \`SessionLoaded\` audit event, so calling it "P0-style" is inaccurate and breaks restricted Mode A identical-history example across a no-open-plan load.

**Preferred narrow documentation repair, without owner acceptance:**
- **Drop unconditional \`SessionLoaded\` audit emission** from the **proposed P0-like option** in §5.2. State precisely: after successful detached validation, session epoch changes; one \`PlanStale(SessionLoaded)\` event per open plan, in deterministic order; if none open, **zero load events**. Unchanged P0 code.
- If author proposes retaining unconditional load audit for a separate future design, explicitly name it **another unapproved alternative**, not P0-style. It would make raw event-history equality across any load impossible, and must state that in Mode A/N09 and owner decision register. The work need not add such an alternative merely for variety.
- Make all **four copied C-1A decision-register entries identical** and explicit: owner to decide original-fixture fork versus post-intervention import and P0-like conditional staling versus distinct new approval policy. All remain **OPEN — proposed only**.
- Mode A: equivalent biology/legal work, and raw-history equality only when full canonical event/session payloads match. Under P0-like conditional zero-event load, the restricted identity case **may be reachable**, but later approvals with different epoch-bound payloads are exceptions. Mode B: exactly one per affected plan, new legal approval required; no stale plan execution. Do not assert whole snapshot hash matches if it includes session-bound data. Reconcile N09/N10, REQ-E12, spec/plan/tasks.

**Pass test:** choosing actual P0-like behaviour can no longer secretly select a different audit policy; no code or owner choice adopted.

## 4. Six MINOR corrections — align all related files and test controls

**Minor A — file capacity, no invented benchmarking.** P0's source caps include **20,000 trees** and **64 MiB maximum file bytes**, but P1's independent service may choose separately approved caps. Reviewer's approximate 10 typed datums × 80 bytes × 20k ≈ **16 MB per annual record**, 64MiB in **~4 years**; this is a **rough model estimate**, not a measurement or verified byte count. Add explicit **proposed serialized bytes per tree-year, annual snapshot/event overhead, multi-year bound and safe failure handling**; compare archival strategies: full per-tree yearly payloads versus compact annual state hashes + discrete death/work events plus reproducible model recomputation. Show what information is needed for actual per-ID inspection if compact. Design N02 fail-closed capacity/oversize cases and no surprise truncate/unsafe save. Do not silently adopt P0 cap as P1 rule or weaken replay integrity.

**Minor B — crown effect and formula branch.** In the constrained source adult CI/growth/density mortality route, current crown radius does not itself feed Hegyi CI/DBH/H/mortality (source-derived, not global forestry truth); it may affect canopy/light in excluded subsystems. Mark synthetic C-4 crown initialisation as affecting crown outputs only **within this domain**. Source Sitka asset \`usePowerLawCrown: 0\` selects the current linear formula; pin asset/hash/version in C-2a if claiming \`max(0.1,0.9415+0.07635*DBHcm)\` under zero competition. Explicit alternative unaccepted.

**Minor C — species/grid/no-op year.** \`ForestEcologyController.AdvanceOneYear\` warns and returns without increment when species cannot resolve or grid is absent (\`:460–465\`). Under future no-spawner capture, explicitly bind/read back Sitka species and initialized grid; **assert ecologicalYear incremented exactly one**, otherwise FAIL, never call run complete. Include relevant future config/N03 negative.

**Minor D — canonical state and audit-history field ownership.** Define a field-membership table for (i) P1 persisted snapshot/state hash (including exact tree/list, ecological year, models/seed, plan state, committed command IDs, fixture identity as applicable), (ii) current session epoch and epoch-bound approvals, (iii) biological-only projection, and (iv) canonical history event/envelope digest. Say exactly whether audit-only \`ExecutionRejected\`, \`SessionLoaded\` (if a separately proposed variant), and plan-stale events change **state** or **history** hashes and which bytes are deterministic across reload. No cycle via pre/post state hashes. Preserve previously accepted Mode A: **never demand raw history hash identity absent complete canonical history identity**. Do not choose hidden owner load semantics.

**Minor E — top height field definition vs estimation.** Current proposed top-height diagnostic (mean height of largest-DBH 100 stems/ha with k/rounding/ties) resembles standard top-height *definition*, but a field *estimator* may use the top-DBH tree per 0.01-ha subplot; they need not agree in clumped/small plots. Reviewer calls this domain knowledge **not independently verified against Broad–Lynch primary definitions**. Note distinction in §5.3, N12, REQ-F10; keep original publication's exact sampling/field-method equivalence **NOT VERIFIED**; do not present synthetic H as validated Irish measurement.

**Minor F — literal mortality RNG identity.** Source uses \`"ADULT-MORT-" + tree.TreeId\` (no normalized ID) at pinned \`ForestEcologyController.cs:1103\`; P0 \`treeId\` must become CCF \`TreeId\` **verbatim**, preserve case/whitespace as allowed by validated P0 schema and ordinal ID ordering. C-2a records exact RNG model, simulation seed, ecological year and this string key, with a negative case where renaming/casing changes roll; no invented stable alternative hash.

## 5. Work products, review and acceptance checklist

**Every changed DRAFT remains DRAFT / NOT APPROVED.** Restore the four copied C decision tables byte-identically if changed. All **C-1…C-7 OPEN**, with C-1A and C-2a/C-2b. No product decision on initial fork, load approval, labels or Unity machine time; zero checked requirements. Proposed P1 capture is actual Unity Editor/Mono **after** a source/ownership/scheduling decision, .NET only if requalified against exact captured vectors; **C-2b cannot be final before capture**.

**Required 13-row future-test crosswalk:**
- **N01**: independently rooted original P0 table and consistent fixture+manifest forgery refusal; separately derived P1 fixtures.
- **N02**: isolated service, hostile paths, explicit capacity refusal and limits.
- **N03**: preadvance CI/victim S, post-death lazy getter trap, nonperturbing CI/roll capture, species+grid/year advance, malformed typed bits.
- **N04**: all-living synthetic Mature-equivalent eligibility retained.
- **N05**: 40/80 source legal geometry, recentered sparse Legacy40 probe with 0.16ha *source* area, dense/edge capture fixtures not in P0 directory, approach path sensitivity.
- **N06**: source annual causality, no duplicate release.
- **N07**: literal \`ADULT-MORT-\` + verbatim ID, integer RNG + strict comparison/mortality order.
- **N08**: state hash field list, event digest and idempotent annual replay, bounded archival.
- **N09/N10**: no-open/open approval branches, conditional P0-style load staleness, epoch and honest hash outcomes.
- **N11**: BA/N reconciliation at actual growth/death/cut sizes, unaffected.
- **N12**: top-height definition versus field-estimation uncertainty and SI/domain blocking.
- **N13**: synthetic non-Irish-forecast labels and P0 no-growth unchanged.

**Do not rerun the full forestry evidence programme or make new Unity claims.** Use targeted source text review only; all 13 groups remain **future proposed tests**, never claim they passed.

**Publication proof:** GitHub-readback each of five files; render all four GFM or explicitly state what was checked; check \`P1-G0\` obsolete gate remains absent and no escaped Markdown backticks. Compare against exact base \`61e53074bd5593603d3c7ab324eca1c1487ac9dc\`: **exactly FOUR MODIFIED Markdown + ONE ADDED Markdown report, no other tracked changes**. Handoff explicitly lists M-1…M-4 and m-A…m-F individually, accepted/rejected proposed remedies, evidence class and unresolved owner choices. Register [HANDED-OFF] on #555, close, comment Manager #425 and prior independent #554.

**Next:** Manager checks the repair and dispatches a **focused independent delta reviewer different from both Codex author and Claude Opus findings reviewer** (recommend Claude Sonnet 5.5 High or equivalent). That review can qualify **DRAFT QUALITY**, not approve actual P1 spec or book the Unity machine.

**Dashboard/Drive mirror:** NOT VERIFIED; no sync/decision-log change.

## 6. Paste-ready message to documentation repair author

> Take ownership of **[GitHub #555](https://github.com/jermur788/CCF/issues/555) — Correct ten defects in the standalone Sitka annual tree-growth and natural-mortality simulation design DRAFT (documentation only).** Complete binding packet: https://github.com/jermur788/CCF/blob/docs/manager-sitka-annual-growth-ten-finding-disposition-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-TEN-FINDING-DRAFT-REPAIR-PACKET-20261115.md. **Difficulty HIGH; recommended Codex Sol High/T3 Code**, the original design author, for exact source-informed repair. The independent reviewer will later be a different model/provider from Codex and Claude Opus. Your precreated repair branch is \`docs/sitka-annual-growth-ten-finding-draft-repair-20261115-v1\`, untouched from exact original DRAFT \`61e53074bd5593603d3c7ab324eca1c1487ac9dc\`; worktree NOT CREATED/ASSIGNED. Fresh-check live coordination and actual worktrees/branch before any inspection/edit. Register **actual model/provider/host/worktree/full starting HEAD [RUNNING]** on #555; BLOCKED on a collision. Repair **M-1…M-4 and m-A…m-F** across only the four existing DRAFT files; add only \`Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-GROWTH-MORTALITY-FOUR-MAJOR-SIX-MINOR-DRAFT-REPAIR-HANDOFF-20261115.md\`. Ensure supported 40/80m stand geometry, P1-owned square capture fixtures and independently trusted originals; pre-growth CI/S observation without post-death lazy-getter trap; accurate conditional P0-like load-stale events; six minor fixes and N01–N13 trace. **DRAFT/NOT APPROVED, all owner/technical C choices OPEN**. No code, Unity Editor, P0 or fixture writes, game build, coordination/main/Drive edits. Verify GitHub readback exactly four MODIFIED+one ADDED Markdown and report precise source-backed corrections; #555 HANDED-OFF/CLOSED and return the durable report URL.
