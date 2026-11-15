# Multi — remaining technical specification design for Sitka spruce annual growth and natural mortality

**Date:** 15 November 2026 | [GitHub task #562](https://github.com/jermur788/CCF/issues/562) **READY / NOT RUNNING**.  
**Purpose:** finish the source-grounded and independently reviewable **file safety, event-history and numerical-capture design** for the standalone Sitka growth/mortality experiment. Do not implement it.  
**Difficulty:** **HIGH** — cross-system data provenance/trust, event replay, local-file security, floating-point source parity and scientifically honest forest-model assumptions.  
**Recommended author:** **Codex Sol High** in a new T3 Code documentation-only session with C#/Unity source-reading competence, then a **distinct independent non-author reviewer** such as Claude Sonnet 5.5 High. Disclosure: earlier expanded DRAFT was authored by Codex; Opus subsequently reviewed and repaired ten errors; Sonnet independently accepted them. Reusing Codex for bounded authorship is permissible; it cannot sign off its own new technical decisions.

## 1. Product decisions already settled — do not reopen

The owner's **15 November 23:25 “approved”** confirmed three specific first-experiment choices. They have been formally recorded in **[Decision Log D-082 on canonical main](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md)**, integrated via [PR #561](https://github.com/jermur788/CCF/pull/561):

1. **Original verified synthetic Sitka forest** as the only starting inventory; no imported existing thinning, felling, saved work approvals or intervention history. Future source-legal **40m or 80m P1-owned derived test squares** are a separate explicitly labelled capture-fixture path, never passed off as original P0 fixtures.
2. **Existing-lab-style session-bound approval safety:** after successful save/load, every open Approved/PendingWork plan becomes stale, with **one** named \`PlanStale(SessionLoaded)\` event per affected plan in ordinal ID order; no such plan means **zero load events**. Fresh approval is needed before work; a separate persistence policy would require a new owner decision.
3. **Scientific honesty:** future laboratory overview, graphs, exports and reports conspicuously identify a **synthetic experimental closed adult pure-Sitka model, NOT a forecast/prediction of real Irish forest growth**. Current wording is editable without weakening the approved warning or excluded processes.

These are **accepted design boundaries** only. They do not approve the four-file spec, code, fixture creation, source modification, an exclusive Unity Editor session, any Player build or actual Irish predictive validity. **C-6 machine time remains OPEN and unbooked.** Scenario One complete Sitka/Oak/Beech loop remains the first external demo. Frozen Reference Future v1 and the original no-growth P0 lab must remain unchanged.

## 2. Exact candidate and previous review evidence

**Authoring source:** owner-reconciled, still unapproved four-file DRAFT from branch \`docs/sitka-annual-growth-accepted-owner-choices-20261115-v1\`, pinned full head \`9081e4591fbae90192149756bcb16e8570e26e47\`. **Precreated writer branch:** \`docs/sitka-annual-growth-technical-contract-resolution-20261115-v1\`, created directly from that exact head, **clean 0-diff** before work. This is NOT main and is NOT an implementation branch.

Read the four DRAFT files under \`specs/multirack-sitka-p1-adult-rules-comparator-v1/\`:
- \`spec.md\`
- \`plan.md\`
- \`tasks.md\`
- \`checklists/requirements.md\`

Ground against prior evidence, without rerunning it:
- [#556 independent Sonnet technical DRAFT-quality review](https://github.com/jermur788/CCF/blob/ad5d06a07be74f50a7b488e23055b733da248b42/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-GROWTH-TEN-FINDING-INDEPENDENT-DELTA-REVIEW-20261115.md) — all ten repairs PASS, 0 blocking/major, six editorial/technical-choice residuals.
- [#558 six minor notes reconciled on GitHub](https://github.com/jermur788/CCF/blob/docs/sitka-annual-growth-six-minor-clarifications-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-GROWTH-SIX-MINOR-DRAFT-CLARIFICATIONS-HANDOFF-20261115.md) — no Unity/source tests or owner technical choices silently selected.
- [#559 owner-choice four-file handoff](https://github.com/jermur788/CCF/blob/docs/sitka-annual-growth-accepted-owner-choices-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-GROWTH-APPROVED-EXPERIMENT-SCOPE-FOUR-FILE-DRAFT-RECONCILIATION-20261115.md). Its owner-decision text is **not** a fresh independent source-validation exercise. Keep every nine-row C-decision register synchronized: C-1A/C-7 owner CONFIRMED; C-1/C-2a/C-2b/C-3/C-4/C-5/C-6 remain at their existing technical/resource gates.
- Pinned CCF biology/geometry source formerly reviewed at \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`; qualified standalone no-growth P0 lab source was at \`2659db99e317d4aa93b51cf97131a61eab5fdeec\`. Source may now be newer; record actual current source pin inspected and **do not** claim historical text is a new Unity execution.

## 3. High-value technical contracts to settle or tightly bound

### A. Secure original source inventory and separate experimental files (technical C-1)

Specify candidate P1-owned immutable trust tables for (i) original approved P0 fixture imports and (ii) separately versioned **new derived 40m/80m square capture fixtures**. The original trusted table must bind fixture ID, file SHA-256, canonical P0 snapshot hash and reviewed provenance revision **independently of** the co-located \`MANIFEST.sha256\`. Runtime must reject a fixture even if its data and co-located manifest were forged consistently. Never fabricate actual hash bytes or claim P0 verifies its manifest.

Specify where P1 lives relative to P0's **separately qualified feature branch rather than game main**, how P1 service/root/save extension and fixture import are separated, and the allowed source ownership/branch integration sequence. Protect the P0 \`.gitignore\`, data directory, Host and qualification hashes. Document handle-relative no-follow file operations, hardlink/special-file/race refusal, typed canonical JSON/schema versions, UTC/time-independent deterministic serialization, origin/Host/CSRF/loopback protection and max-size refusal. No weaker native-OS fallback without review.

Compare **full per-ID annual archive** versus **compact state-hash and discrete-outcome archive with deterministic replay and inspectable/exported per-ID trace**, including 2,000 and 20,000 tree budgets at 1/10/40 years; choose a defensible **technical proposal** for C-1 or clearly leave bounded alternatives for independent review. P0's 64 MiB/20,000-tree caps do not automatically transfer. Ensure no unbounded event-history failure and a real negative test for over-capacity without corrupt save.

**Rejected-command ID handling (source of #556 D-6):** explicitly decide, as a **technical proposal subject to independent replay/security review**, whether an audit-only rejected command consumes its command ID, what is hashed in state versus the event digest, how identical- and conflicting-ID retries behave, and which golden tests disambiguate. No audit-only failed request may mutate biology. Decide consistently with the separately accepted load staleness semantics, without inventing a projection hash policy.

### B. Real source-capture observability before any Unity time (technical C-2a)

Make a precise, versioned numeric capture contract for the present source: per-tree ID, ecological year, original P0 ID verbatim, growth/RNG model, seed, source/asset/Editor/Mono pin, source-legal 40m/80m square, typed **f32 raw 8-hex and f64 raw 16-hex** values with round-trip decimals and units, branch-skipped values marked \`notEvaluated\`, source ordering (pre-growth CI; DBH/height/age; crown; post-growth density; actual mortality hazard S/p/roll and victim sequence), strict integer RNG key \`"ADULT-MORT-" + TreeId\`, source asset \`usePowerLawCrown: 0\`, species/grid readback, exactly +1 ecological year, and no storm/recruitment/spawner.

**Critical private-method trap:** \`ForestEcologyController.GetCompetitionIndex\` can lazily recalculate after a mortality event and remove victim cache entries. Source \`ApplyAdultDensityMortality\` can **skip S/p/roll entirely when density pressure ≤0** and S/p are method locals, not public fields. No capture protocol may call a public getter after death and claim to have observed growth-driving CI, invent \`0\` for an unevaluated local, or reconstruct a doomed tree's actual S. Specify observation points and a safe **verification-only instrumented source copy** proposal, with explicit source owner/reviewer, allowed source-copy scope, what happens if instrumenting is not allowed or perturbs results, and an **uninstrumented same-Editor A/B control** (numerically exact where source can justify it). A source copy is NOT authorised for creation here; you are documenting whether it is necessary/admissible and what future evidence is required.

**Do NOT decide C-2b** numeric tolerance/parity until actual Editor/Mono reference values exist. Do not treat a .NET-only or Python reconstruction as authoritative Unity results. Where capture feasibility is genuinely impossible without source changes, return **BLOCKED technical gate with evidence**, not a fake oracle.

### C. Source-domain assumptions and forestry comparison (technical C-3, C-4, C-5)

Define exactly the *closed pure-Sitka all-living Mature-equivalent, Reference-demand* population. All live stems, including below-P0-thinning-threshold IDs, participate in growth/CI/natural death; action-eligible subset separately reported. Label the forced Mature-equivalent stage, source site factor 1, synthetic zero-competition crown/seed/year/age initialisation and pre-inventory dead unknown-cause/year as **synthetic/gameplay assumptions**, not measured Irish values.

Pin source-legal centred Legacy40 = **0.16 ha** and Enlarged80 = **0.64 ha**; original 50m quarter-ha P0 fixture is a **derived 40m sparse *wiring* probe with 0.16 ha source area**, not its original area/stand-density parity. Other dense/edge/8m/singleton controls must be separate P1 capture fixtures, never written to P0; check Enlarged80 approach-path mutations. The top-height diagnostic can use an explicitly versioned **largest-DBH 100 stems/ha definition**, not necessarily the source paper's field estimator; original Broad–Lynch top-height field method remains **NOT VERIFIED**. Any GROWFOR 2006 stand B/N/H comparator is strictly **model-to-model**, with even-aged synthetic SI/domain caveats, never per-tree or Irish field validation from 113 volume-equation plots.

Where an exact source truth cannot be determined from text, give an evidence class (**direct source check, source-derived inference, synthetic assumption, future observed Unity evidence, unsupported-needs decision**) and set a named future gate.

## 4. One author, exact write scope, correct review exit

**Writer claim:** [#562](https://github.com/jermur788/CCF/issues/562) currently READY/NOT RUNNING. **Actual host/worktree NOT ASSIGNED and no machine lease.** A new private laptop ext4 or WSL ext4 docs-only worktree may be created **only after** checking current live \`Docs/Project/coordination/active-tasks.md\`, registered worktrees, ownership and actual branch HEAD. A GitHub-only writer is also acceptable if all required source checks can be performed. Register **actual worker/provider/model/host/worktree or none, branch and full starting HEAD, editable scope as [RUNNING] on #562 BEFORE source inspection/tests/writing**. On conflict post [BLOCKED] and stop; never clean/stash/reset/overwrite unrelated dirty work. Task claim does not override live board ownership.

**Only allowed existing tracked files:** four DRAFTs in \`specs/multirack-sitka-p1-adult-rules-comparator-v1/\`: \`spec.md\`, \`plan.md\`, \`tasks.md\`, \`checklists/requirements.md\`.

**Only allowed new tracked file, exact output path:**
\`Docs/Handoffs/MultiRack-Simulation-20261114/Multi-Sitka-Annual-Growth-Technical-Contract-Resolution-Handoff-20261115.md\`.

**Everything else READ-ONLY**: \`Assets/**\`, \`Tools/**\`, P0 lab/fixtures, original approved specs, old handoffs, main Decision Log, current milestone, live coordination board, user saves, frozen Reference Future, canonical instructions/context sources, Drive and dashboards. No Unity, compiler, source edit, fixture creation, browser run, file integration, Windows build or machine lock.

**Acceptable handoff evidence:** an explicit technical decision/tradeoff matrix and source pin; any technical choices made remain proposals until independently reviewed; all 13 N01–N13 checks remain proposed and no requirement box checked; four C tables remain byte-identical, C-1A/C-7 owner CONFIRMED, C-6/C-2b OPEN; list any still-blocked source/finance/science/Unity constraint. GitHub read back all five files, optional GFM render, compare working branch against exact starting DRAFT HEAD: **exactly FOUR MODIFIED Markdown and ONE ADDED Markdown**, no other paths. Publish report, mark #562 [HANDED-OFF] and close, notify Overall Manager [#425](https://github.com/jermur788/CCF/issues/425) and #559. The Manager then conducts a proportionate **independent technical-spec delta review**, and only later seeks explicit full four-file specification approval. No code/Unity auto-release.

**Context/Drive:** canonical D-082 already merged on main; generated instruction-context/Drive mirrors and platform attachments require separate clean-SHA synchronization, **NOT VERIFIED**. They are not changed by this task.

## Paste-ready dispatch

> Take ownership of **CCF #562 — Finish the technical file-safety, save/replay and numerical-reference design for the standalone Sitka annual growth and mortality experiment**. Full GitHub handoff: https://github.com/jermur788/CCF/blob/docs/manager-sitka-growth-technical-design-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/Multi-Sitka-Annual-Growth-Technical-Design-Resolution-Packet-20261115.md. **Difficulty HIGH; recommended Codex Sol High in T3 Code** for source-literate docs only. Read accepted main Decision Log D-082: owner approved original verified synthetic forest, P0-like conditional staleness of open thinning approvals on load, and explicit synthetic-not-Irish-forecast disclosure. These are NOT for re-decision. Use precreated clean branch \`docs/sitka-annual-growth-technical-contract-resolution-20261115-v1\` from the owner-reconciled four-file DRAFT. No worktree/Unity slot exists: fresh-check live board/host/worktrees; register actual worker/model/host/worktree/full HEAD as [RUNNING] #562 before inspection. Document technical C-1 trusted file/root/secure history and rejected-ID/archive choice, C-2a typed float source-capture and the feasibility/permission of future verification-only source instrumentation, and C-3/C-4/C-5 population/initialisation/40m-80m source square/H-SI limits. Preserve C-2b post-Unity and C-6 owner machine time **OPEN**. Modify only the four DRAFT Markdown files and add only \`Docs/Handoffs/MultiRack-Simulation-20261114/Multi-Sitka-Annual-Growth-Technical-Contract-Resolution-Handoff-20261115.md\`. No code, Unity, P0, fixture, build, game save, board or main edit. Verify exactly 4 MODIFIED +1 ADDED Markdown on GitHub, close #562 HANDED-OFF and return source-grounded proposed technical contracts for independent review.
