# #555 — Sitka annual growth and mortality DRAFT: repair of four major and six minor findings

**Date:** 15 November 2026  
**Worker assessment: READY FOR FOCUSED INDEPENDENT DELTA REVIEW.** The specification stays **DRAFT / NOT APPROVED**. All of C-1…C-7, including C-1A and C-2a/C-2b, remain **OPEN**, and C-2b still cannot close before a real Unity capture. This is a documentation repair only. It adds no code, runs no Unity or lab, changes no P0 fixture and makes no game build. The full three-species Scenario One remains the first external demo.

## Worker, authority and pins

| Item | Record |
|---|---|
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in local T3 Code, dispatched directly by the user |
| Independence | This model also wrote the [#554 findings](https://github.com/jermur788/CCF/blob/a6bf2d01ff4ce53e63796406c89323dfc9fb2099/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-AND-MORTALITY-INDEPENDENT-DESIGN-REVIEW-20261115.md) and parts of earlier baseline text. Per the packet, the delta review must be by a reviewer who is **neither Codex nor Claude Opus** (recommended: Claude Sonnet 5.5 High) |
| Host / worktree | `jer-laptop`; new private worktree `/media/jer/Files/CCF/Operations/CCF-555-SitkaAnnualGrowth-TenFindingRepair-20261115` (Files, ext4); no Unity lane or lease |
| Branch / base | `docs/sitka-annual-growth-ten-finding-draft-repair-20261115-v1`, starting at `61e53074bd5593603d3c7ab324eca1c1487ac9dc` (0 ahead at start) |
| Claim | [#555 RUNNING registration](https://github.com/jermur788/CCF/issues/555#issuecomment-6902596726), posted before inspection or edits |
| Live board | `origin/main` `a9b89ab8` `Docs/Project/coordination/active-tasks.md`: no #555 row, no conflicting writer, no other worktree on the branch |
| Binding inputs | [Repair packet](https://github.com/jermur788/CCF/blob/docs/manager-sitka-annual-growth-ten-finding-disposition-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-TEN-FINDING-DRAFT-REPAIR-PACKET-20261115.md); [Manager NEEDS CHANGES ruling](https://github.com/jermur788/CCF/blob/docs/manager-sitka-annual-growth-ten-finding-disposition-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-ANNUAL-GROWTH-DESIGN-REVIEW-NEEDS-CHANGES-20261115.md) |
| Source pins (static text only) | CCF `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; P0 lab `2659db99e317d4aa93b51cf97131a61eab5fdeec`. These are historical snapshots, not present-day Editor or lab results |

Unrelated coordination-anchor edits (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md` in `/media/jer/Files/CCF-Primary`) existed before this work and were left untouched.

## Finding-by-finding disposition

Evidence classes: **DSC** = direct source check at a historical pin; **INF** = source-derived inference; **EST** = rough model estimate; **DOM** = reviewer domain explanation, not checked against the primary source; **PROP** = design proposal, not adopted.

| Finding | Packet remedy | Disposition | Where (four DRAFT files) | Evidence |
|---|---|---|---|---|
| **M-1** Only 40 m / 80 m stands; P1 needs its own capture fixtures | All accepted | Stated that the source admits only Legacy40 (40 m, 0.16 ha) and Enlarged80 (80 m, 0.64 ha), and that no reflection or production workaround is allowed. Added a **P1-owned derived capture-fixture catalog**, separate from original P0 import and never written into P0. `oracle-quarter-ha` is now only the derived Legacy40 sparse probe `oracle-quarter-ha-legacy40-v1` (trees at −15/0/+15 m, **source area 0.16 ha**, CI 0, P 0), and is not area parity. Dense and edge probes go on 40 m, with optional 80 m and an Enlarged80 approach-path readback. Removed the 50 m feasibility wording and the "candidate first source-parity fixture" claim | spec §4.1 (derived catalog), §5 C-5, §5.1 adapter, probe and area paragraphs, §6 item 2; plan G3, §9 configuration, fixture set, **N05**; tasks C02, §9 rows, closing paragraph; requirements REQ-D05, **REQ-D11**; C-5 register row | DSC `StandGeometryModel.cs:32,34`, `ForestEcologyController.cs:79–107`; PROP |
| **M-2** Independently pinned trust anchor | All accepted | Proposed a **P1-owned reviewed trust table**, compiled into P1 or shipped as immutable packaged configuration, never read from the P0 data directory. Each record binds fixture ID, file SHA-256, snapshot hash and the P0 revision, which is provenance metadata with no runtime Git check. Named the preparing owner and reviewer; a new record means a new version. No hash values are invented. Added the consistent fixture-plus-`MANIFEST.sha256` forgery refusal and the statement that hashes prove integrity, not authorship. Derived fixtures have their own trust table | spec §4.1 step 1 and trust-anchor block; plan **N01**; tasks §9 P1-02; requirements **REQ-B08**; C-1 register row | DSC `FixtureCatalog.cs:20–44`; PROP |
| **M-3** Pre-growth CI capture timing | All accepted | Added observation points: a pre-advance CI read for every living ID; S, p, roll inputs and roll recorded at evaluation, including victims; a post-step survivor cache cross-check without the lazy getter, failing closed if unreadable; and an instrumented-vs-uninstrumented same-Editor control. The method is left to the future source owner. No capture or feasibility is claimed. Added the four N03 negatives | spec §5.1 competition observation block, §6 item 2; plan §9 configuration and A/B, **N03**; tasks C13, §9 P1-01; requirements **REQ-C13**; C-2a register row | DSC `ForestEcologyController.cs:429–443, 1811–1819`; PROP |
| **M-4** Load staling faithful to P0 | **Preferred** remedy taken (drop the unconditional event); the retained-audit alternative was not added | §5.2 is now the **P0-like conditional** option: one `PlanStale(SessionLoaded)` per open plan in plan-ID order, and **zero** load events when none is open. An unconditional `SessionLoaded` event is described only as a separate unapproved alternative that would make raw equality across a load impossible. Mode A now says the restricted identity case **is** reachable across a no-open-plan load, with later epoch-bound approvals as the exception. The C-1A row is identical in all four files and stays OPEN | spec §4 Option 1, Mode A, §5.2 stale-on-load; plan Mode A bullet, **N09/N10**; tasks §9 P1-03; requirements REQ-E03, **REQ-E12**; C-1A register row | DSC `LabSession.cs:76–85, 144–149`; PROP |
| **m-A** File capacity | Accepted | Bytes per tree-year for full versus compact archives; 1 / 10 / 40-year budgets at 2,000 and 20,000 trees (≈4 years to 64 MiB for a full archive at 20,000 trees). P0 caps are **not** inherited automatically. The compact archive recomputes on replay, and per-ID inspection is by recomputation or an optional external trace. Over-capacity fails closed before commit | spec §4.1 capacity block; plan **N02**; tasks §9 P1-03; requirements REQ-B09; C-1 row | DSC `Models.cs:24,28`; **EST** |
| **m-B** Crown effect and branch | Accepted | Crown is not an input to CI, DBH, H or mortality in this adult-only route, so it affects crown outputs only; marked as domain-specific. Pinned `usePowerLawCrown: 0` and the linear branch; C-2a records the asset identity | spec §5.1 crown row and capture header; requirements REQ-D10, REQ-C13 | DSC `TreeSpeciesDefinition.cs:246–253`, `SitkaSpruce.asset:25`; INF |
| **m-C** No-op year | Accepted | Bind the species explicitly, read back the species and grid, assert exactly +1 year per step, otherwise FAIL. Added an N03 negative | spec §5.1 year block, §6 item 2; plan §9, N03; tasks C12, §9 P1-01; requirements REQ-C13; C-2a row | DSC `ForestEcologyController.cs:460–465` |
| **m-D** Hash field membership | Accepted | Field table covering state hash, biological projection and history digest: the current session epoch is **not** hashed state, while approval epochs, the command-ID set and the ledger are. Spelled out the effect of `ExecutionRejected`, `PlanStale`, a no-plan load and the always-on variant. No cycle. Mode A rule unchanged | spec §5.2 table; plan N08; tasks §9 P1-03; requirements REQ-E11 | PROP |
| **m-E** Top-height definition vs field estimation | Accepted | Distinguished the 100-largest-DBH-per-hectare definition from one-tree-per-0.01 ha subplot estimation; labelled DOM; equivalence still NOT VERIFIED | spec §5.3; plan **N12**; tasks §9 P1-04; requirements **REQ-F10**; C-5 row | DOM |
| **m-F** Literal RNG key | Accepted | Literal `"ADULT-MORT-" + TreeId`, verbatim P0 IDs, recorded RNG model, seed and year; a renamed or re-cased ID is a negative case; no substitute ID hash | spec §5.1 per-year record and RNG-identity block; plan **N07**; tasks C06; requirements REQ-C14; C-2a row | DSC `ForestEcologyController.cs:1103` |

**Nothing was rejected.** For M-4 the packet left a choice, and the preferred narrow remedy was taken. No owner choice was made.

## N01–N13 future-test crosswalk (proposed, none run)

| Row | After repair |
|---|---|
| N01 | Embedded P1 trust table; consistent fixture-plus-manifest forgery refused; derived fixtures through their own catalog |
| N02 | Isolated service and hostile paths unchanged; P1-approved caps and archive layout; over-capacity fails closed before commit |
| N03 | Pre-advance CI, at-evaluation victim S/roll, post-death lazy-getter trap, no-perturbation control, species/grid/+1 year, malformed typed bits |
| N04 | Unchanged: all-living synthetic Mature-equivalent eligibility |
| N05 | 40/80 m legal geometry; derived Legacy40 sparse probe with 0.16 ha **source** area; dense/edge fixtures outside the P0 directory; approach-path check |
| N06 | Unchanged: source annual causality, no duplicate release |
| N07 | Literal `"ADULT-MORT-"` key with verbatim ID; integer RNG; strict comparison and mortality order |
| N08 | Hash field table, event digest, idempotent replay, archive replay by recomputation |
| N09 / N10 | P0-like conditional staling: zero events with no open plan, exactly one per open plan; honest hash outcomes |
| N11 | Unchanged: BA/N reconciliation at actual sizes |
| N12 | Definition versus field-estimation caveat; SI/domain blocking unchanged |
| N13 | Unchanged: synthetic non-forecast labels; P0 no-growth unchanged |

## Still OPEN — owner and technical choices

- **Owner:** C-1A (original-only versus post-intervention fork; P0-like conditional staling versus a distinct P1 approval policy), C-6 (Unity time, behind Scenario One) and C-7 (wording).
- **Technical, at the gates in spec §7.1:** C-1 (format, root, trust table, derived catalog, caps, hash fields), C-2a (capture format, observation points, RNG key, asset/year readback), C-3, C-4 and C-5. **C-2b** is decided only after a real capture.
- Every proposal in this repair is **PROP**, and none is adopted. P0 lineage and the P1 base branch still need a Manager decision before P1-02.

## Files changed

Four **MODIFIED** files in `specs/multirack-sitka-p1-adult-rules-comparator-v1/`: [spec.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md), [plan.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md), [tasks.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md), [checklists/requirements.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md). Each also gained a #555 status paragraph and updated G1 / P1-S0 status text.

One **ADDED** file: this handoff. Nothing else changed: no `Assets/**`, `Tools/**`, P0 fixture, `MANIFEST.sha256`, P0 `.gitignore`, earlier handoff or review, `Docs/Project/**`, main, saves, dashboard or Drive.

## Verification actually performed (documentation only)

- Every text replacement was applied by an off-repo script that required each original string to occur exactly once; it aborts without writing on any mismatch. `git diff --check` passed.
- The four decision registers (nine rows each) are byte-identical: the same SHA-256 prefix `3be4cd5a3f71a489` in all four files.
- No `P1-G0`, no escaped backticks, no "P0-style" wording, no `±25`, no 50 m parity claim, and **0 checked** requirement boxes.
- GitHub GFM rendering, readback and the base comparison are recorded in the #555 closure comment: four MODIFIED plus one ADDED, compared against `61e53074`.

**Not run or established:** compilation, simulation, Unity Editor or Mono capture, P0 lab tests, browser checks, GROWFOR, field data, game integration or Windows build. All N01–N13 rows remain **future proposed tests**.

## Next gate

Manager #425 reviews this repair and dispatches a **focused independent delta review of M-1…M-4 and m-A…m-F** by a reviewer who is **neither the Codex #552 author nor Claude Opus**. That review can grant DRAFT QUALITY only. It cannot approve the P1 specification or book Unity time.

**Dashboard / Drive sync: NOT VERIFIED.** No coordination, decision log or milestone edits.
