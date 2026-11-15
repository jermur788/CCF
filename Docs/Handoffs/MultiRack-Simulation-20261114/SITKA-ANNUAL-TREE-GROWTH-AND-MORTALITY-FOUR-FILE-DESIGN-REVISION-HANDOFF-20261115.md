# #552 — standalone Sitka annual tree-growth and natural-mortality design revision

**Date:** 15 November 2026. **Worker assessment: READY FOR INDEPENDENT DESIGN REVIEW. DRAFT / NOT APPROVED.** This is the documentation handoff requested by [#552](https://github.com/jermur788/CCF/issues/552), not a specification acceptance or implementation result. All C-1…C-7 remain OPEN, including C-1A and C-2a/C-2b. C-2b cannot close without actual future Unity data. Full comparable-depth Sitka/Oak/Beech Scenario One, repeated interventions and multi-scale forestry remain the complete first external demo; personal candidate playtest and explicit build approval still precede any Windows Player build.

## Worker, authority and immutable candidate

| Item | Actual record |
|---|---|
| Worker / role | OpenAI Codex `gpt-6.1-sol`, High reasoning, T3 Code through Codex harness; primary documentation/specification design author |
| Provider/quota | Configured Codex provider; backend quota accounting not independently exposed; no provider/model switch or delegated worker |
| Independence | This session did not author the prior source studies/reviews. This author **cannot** supply the subsequent independent assessment of its own #552 revision |
| Host | `jer-laptop`, Linux `7.0.0-42-generic`, user `jer` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-552-SitkaAnnualDesign-20261115`, privately created after preflight; docs-only, no Unity lane/lease |
| Storage | Files mount `/dev/sdb4`, label `Files`, ext4, 129 GiB available at preflight. Historical policy device name differs; actual mount identity checked |
| Owned branch | `docs/sitka-annual-growth-design-resolution-draft-20261115-v1`, the precreated writer branch; Manager dispatch branch untouched |
| Starting BASE | `3af7b7ff30804bc9aff0d19ea394b9c45946c5a8`, clean; initial difference zero |
| **Four-file design revision HEAD** | **`a5d2d867acfb253953764955fba2dbff9a2424bd`**. This report is added in a following docs-only publication commit. Its full published tip, exact five-file readback/diff and closure receipts are recorded on #552; the report does not claim to contain its own circular commit hash |
| Context commit | `62e39ec621e38a84030c08b35a171e297056c9ed`, latest enumerated canonical-context-set commit in the baseline ancestry; context consulted at this pin, no context/mirror regeneration |
| Binding packet | [Published packet](https://github.com/jermur788/CCF/blob/f8268f5adaa3b66daef59b78a07230e3caf816cb/Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-ANNUAL-TREE-GROWTH-AND-MORTALITY-DESIGN-RESOLUTION-PACKET-20261115.md), dispatch branch pin `f8268f5adaa3b66daef59b78a07230e3caf816cb` |
| Actual registration | [RUNNING registration before draft/source inspection or edits](https://github.com/jermur788/CCF/issues/552#issuecomment-6901413317) |
| Classification / authority | SPEC REQUIRED feature, **DRAFT** design phase only; owner's “Continue the design work” and #552 packet authorise exactly four modified specs plus this added handoff, not accepted mechanics, implementation or execution |

Fresh preflight read #552, the binding packet, live GitHub workflow-designated `ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md`, the physical coordination record and local CCF-Ops board. No #552 active owner or competing branch/worktree was found; the remote writer branch remained at BASE, proposed path absent, no matching Unity/source task process. A missing board row does not grant authority: this bounded assignment comes from the user's direct dispatch and binding packet; issue presence changes no board lock. No live coordination edit, worktree cleanup, process kill, stash or reset was performed.

Unrelated coordination-anchor edits to `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were observed before work and preserved. No other worktree was edited. This result is neither merged nor integrated into main.

## What changed and why

The four reviewed drafts had source rules and safety boundaries but left import, initial state, numeric capture and annual event contracts too abstract for the next design review. This pass supplies concrete **review proposals**, retaining the original-only vs post-intervention, session-stale vs distinct persistence, isolated service vs shared Host and synthetic crown/SI alternatives. It corrects stale review-readiness text: MULTI-016 completed the baseline DRAFT-quality gate, while this expanded design requires its own independent review.

1. **Original immutable fork and storage:** a trusted revision-pinned catalog verifies complete original-file SHA-256 **and** canonical P0 snapshot hash. Proposed v1 rejects post-executed/calendar/marked/approved P0 states, rather than silently stripping their history. P1 uses distinct format/schema/extension and a P1-owned sibling service/root/ignore proposal, independent launch and loopback-origin scope. No P0 `.gitignore` edit. Shared Host reuse is a reviewed alternative with extra regression/ownership costs. Exact P0 feature-branch/P1 base lineage remains a prerequisite before adapter work.
2. **Biological inputs and geometry:** all live synthetic Mature-equivalent Sitka/Reference, including below-action-threshold stems, enter CI/RD/growth/mortality; action subset remains separate. Copy per-tree fixture ages, explicitly declare ecology year0/seed/RNG/growth versions; use a labelled synthetic source zero-CI crown target with floor. Pre-inventory Dead has unknown/null cause/year/crown and is no new death event. Recenter square XY exactly before source x/z casts, preserve elevation meaning and actual f32 source area. Three-tree quarter-ha is sparse; dense/threshold fixtures must be separate. Hidden geometry resize is a stop, polygons are non-parity.
3. **Numeric capture:** candidate versioned C-2a manifest records exact source/asset/harness/runtime/config/fixture identity, units, f32 8-hex/f64 16-hex words plus agreeing round-trip decimals, conversion/storage/evaluation order, per-ID annual trace and actual branch/RNG call provenance. Reject malformed/nonfinite/missing/version/bit/type mismatches. Skipped values are not fabricated zeros. Instrumented output must equal an uninstrumented control; unavailable intermediates stay unavailable. Binary32 declarations and earlier scalar calculations do not prove Mono parity. C-2b remains a post-capture decision.
4. **Annual ledger and persistence:** compound annual transaction stages year/CI/growth/age/crown/post-growth RD/hazard/boundary/death/plan invalidation atomically, never replays child deaths as second mutations. Work executes before next growth; approval/mark/queue cannot fell. Proposed command IDs prevent duplicate advances. Separate state/history digests avoid circular hashing and make no local-authenticity promise. Model recomputation validates replay before live activation. Proposed load stales each open approved/pending plan once, records audit, and needs fresh legal approval; all still C-1A owner choice. Mode A/B compare equivalent biology/legal work; raw histories match only when complete payloads/order/hashed session data are identical. No projection/hash normalization adopted.
5. **Scientific comparator:** source Class III height/site1, British density transfer and gameplay hazard remain distinct from Irish measurements. Proposed top-height diagnostic defines rounding/k/ties/empty cases but claims no original field-method equivalence. SI30 is declared/inferred only from synthetic, domain-checked stand inputs or comparison is blocked. Deliberately even-aged B/N/H comparison remains model-to-model; 113 plots were volume-equation calibration, not untouched final-growth holdout. Exact visible scientific label remains an owner-review proposal.
6. **Future gates and negative controls:** plan N01–N13 names positive/negative observations, future owner/reviewer, task and requirement; task map and 12 added unchecked requirements trace them. Oracle configuration proposes pure Sitka/Reference/growth2/RNG1, valid regeneration2 manager, no spawner/cohorts/storm/pruning, explicit year map, configuration A/B and uninstrumented control. Exact source/capture pin and geometry feasibility still need review. No P1-01 task or Unity booking was created. Scarce machine scheduling needs separate owner permission after Scenario One priority.

## Files changed — exact writable scope

Four **MODIFIED** Markdown files:

- [spec.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md): §4.1 fork/storage, §5.1 initialization/geometry/capture, §5.2 ledger/replay, §5.3 H/SI, OPEN decision/timing register and status.
- [plan.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md): sibling service tradeoff, gate status, identical decision register, §9 future configuration and N01–N13 controls.
- [tasks.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md): historical quality vs new review, decision register and concrete task-to-contract/negative-test ownership.
- [requirements.md](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md): identical decision register, refined isolation/label requirements and 12 additional **unchecked** acceptance checks.

One **ADDED** Markdown: this exact handoff. No Assets/Tools/P0 fixture or source, Unity YAML/metas/serialized assets, packages/ProjectSettings, approved P0 spec, previous handoff, canonical context, coordination, main, game saves, Reference Future, dashboard or Drive file was changed. No binary asset or licence footprint.

## Decision-resolution table — all OPEN

| Decision | Review proposal, not an accepted default | Decision owner and when needed | Evidence needed | Status |
|---|---|---|---|---|
| **C-1A** fork and approval-after-load | Original verified fixture only; no P0 plans/calendar events imported; P0-style session-bound approvals stale on load | **Product owner**, before final spec approval and any import/plan-persistence work | Original-vs-post-execution and stale-vs-persistent approval alternatives; Mode A/B and safe reapproval design | **OPEN — proposed only** |
| **C-1** identity, storage and lineage | P1 format/version/extension; sibling P1-owned service/root/ignore; immutable source; no P0 write | Technical spec owner + independent file-security reviewer before persistence/import; Manager fixes exact P0 lineage and P1 base before P1-02 | Root/Host/handle-relative I/O threat model and source branch inventory; no assumption P0 is on main | **OPEN** |
| **C-2a** pre-capture format | Versioned typed f32/f64 raw-bit records, exact inputs/units/order and round-trip policy | Technical capture owner + independent numerical reviewer, before P1-01 | Spec §5.1 capture contract and invalid/missing/version refusal examples | **OPEN — designable without Unity** |
| **C-2b** parity acceptance | Bits where demonstrated; otherwise reviewed tolerance with branch guards and identical discrete results | Independent numerical reviewer + Manager, **after actual P1-01 Unity capture**, before P1-02 acceptance | Actual captured vectors, repeatability, distances to thresholds and death/victim/plan sets | **OPEN — cannot close now** |
| **C-3** population | All living synthetic Mature-equivalent Sitka/Reference in biology; action subset separately labelled | Technical source/spec reviewer before fixture/oracle; **owner** for a non-parity subset alternative | Below-threshold controls and all-living CI/RD/growth accounting | **OPEN** |
| **C-4** initial state | Source ages; explicit ecological year/seed/versions; synthetic zero-CI crown; pre-inventory dead, unknown cause/year; source site=1 | Technical spec reviewer before fixtures | Per-field provenance and age/clamp/dead/reseed negative controls | **OPEN** |
| **C-5** geometry and SI/H | Centred source square; explicit x/y→x/z adapter; polygons non-parity; diagnostic H and synthetic SI caveats | Technical reviewer before oracle geometry; independent science reviewer before P1-04 SI/H comparison | Source origin/area bits, edge fixtures, original H-method gap and SI derivation | **OPEN** |
| **C-6** oracle and machine | Actual pinned Unity Editor/Mono oracle first; .NET secondary; no machine lease | **Product owner** separately permits exclusive time after higher-priority complete-loop Scenario One; Manager/operator preflight before scheduling P1-01 | Approved configuration, bounded A/B, exact host/HEAD/normal lease and cost | **OPEN — no lease or source task** |
| **C-7** scientific language | “CCF adult rules — closed pure-Sitka experimental population. Synthetic inputs. NOT an Irish field forecast.” | **Product owner**, before final spec acceptance and any visible report/chart/UI/export | Consistent proposed wording and stand-only reference/domain labels | **OPEN — proposed only** |

**Owner alternatives are preserved.** Continue-design permission does not choose C-1A/C-6/C-7. Technical choices need independent review at their stated gate. C-2b cannot be final now, and its absence does not make the pre-capture design circular. P1-S0/G1 baseline quality COMPLETE is distinct from this revision's pending review. P1-G2/G2 future product/spec approval **NOT PASSED**; P1-01…05/G3…G7 **NOT AUTHORISED**; plan G0 evidence-plan stage COMPLETE.

## Sources actually consulted and limits

- [MULTI-009 source study](https://github.com/jermur788/CCF/blob/2212544e90fc5c48195f708c525e58be8a8cd936/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md), source/evidence mapping and vector classification; [MULTI-010 independent source/science review](https://github.com/jermur788/CCF/blob/3d9699dc9665dd853bda88dbf6b1f1bd5e94530e/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md), domain and m-1…m-8 conditions.
- [MULTI-012 findings](https://github.com/jermur788/CCF/blob/effc9f6cb9c592fb65a96aed640013c0157dbab6/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md), R-1…R-10; [MULTI-016 final review](https://github.com/jermur788/CCF/blob/2fb66c448e93a47af9f275f4f3bf696c8194c73e/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-016-P1-MODE-A-HISTORY-AND-GATE-FINAL-INDEPENDENT-REVIEW-20261115.md), Mode A/B/hash and P1-G2 repair; [Manager quality/product gate](https://github.com/jermur788/CCF/blob/c963d6f38a3e3a6c90d8f1c53eeecfd3f74f0a66/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-016-P1-DRAFT-QUALITY-ACCEPTANCE-AND-OWNER-PRODUCT-GATE-20261115.md).
- **Narrow direct source text at CCF `fc18190b976f1363f1b5d1b0881810db2f8c6e39`:** `Assets/ForestPrototype/ForestEcologyController.cs` area/bounds 67–78, FindTrees/ordinal/dead exclusion 860–890, growth 1003–1038, mortality 1062–1158, crowns 1229–1245; `SitkaGrowthModel.cs` height/density/hazard 30–114; `TreeSpeciesDefinition.cs` crown radius 246–255; `ForestTree.cs` growth/crown/age clamps 623–641. This is a historical design/source pin, **not an asserted current runtime result**. Future capture must repin actual source.
- **Narrow direct P0 text at `2659db99e317d4aa93b51cf97131a61eab5fdeec`:** `Tools/MultiRackSitkaLab/src/Core/{Models.cs,Metrics.cs,LabSession.cs,LabFile.cs}` input fields/bounds, action metric, fresh epochs/load staleness/calendar-only step and strict header; original `fixtures/synthetic/oracle-quarter-ha.sitkalab.json` square/three-tree values. P0 lab tree listed at this pin; no P0 process/tests run.
- Canonical context at `62e39ec621e38a84030c08b35a171e297056c9ed`: AgentWorkflow, Decision Log, Current Milestone, Game Brief, Unity overview and research index consulted. [GROWFOR evidence extraction](https://github.com/jermur788/CCF/blob/62e39ec621e38a84030c08b35a171e297056c9ed/Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md) source-role/SI/top-height/113-volume-data passages inspected. Primary forestry PDFs/field datasets/current GROWFOR executable were **not re-audited**. New assumptions are identified as synthetic/design choices, not research facts.

### Prior-review repair preservation

| Repair | Preservation in this revision |
|---|---|
| R-1 | Unity/Mono oracle of record, code-generation caveat, C-2a before/C-2b after capture, .NET secondary only |
| R-2 | Mode A load audit and later-approval epoch counterexamples; Mode B one stale event and legal-action caveat; independent chains, no unconditional equality |
| R-3 | Real code spans/fence; GitHub GFM HTML rendering checked |
| R-4 | G0 evidence COMPLETE, P1-S0/G1 baseline quality, P1-G2/G2 future owner approval, P1-01…05/G3…G7 unreleased |
| R-5 | Identical nine-row decision/owner/timing/evidence/status register in all four files |
| R-6 | Historical schema23-vs24 repair separate and optional unless the old harness is reused |
| R-7 | Pre-growth suppression vs post-growth density; valid manager/regeneration/no-spawner/storm0/year mapping and A/B |
| R-8 | Independent non-author review plus explicit product owner; no author self-approval |
| R-9 | Qualified P0 feature lineage absent from inspected main; exact P1 base choice pending; sibling root, no implied P0 ignore edit |
| R-10 | Defined DRAFT/OPEN meanings and complete m-1…m-8 crosswalk; sparse square and editorial fixes retained |

m-1 stage-sensitive height, m-2 site1, m-3 unknown prior death, m-4 numeric capture, m-5 geometry, m-6 RNG identity, m-7 volume-data attribution and m-8 all-living/density/SI gaps all remain in the spec's original trace table and companion requirements. This author checks preservation, **does not independently accept those repairs or the new design**.

## Verification actually performed

**Documentation-only checks. No simulation/Unity execution.**

- Reviewed all four starting DRAFT files, revised contents and diff against BASE; no other tracked paths permitted. `git diff --check` PASS. Scope/status/relative-path and heading-target checks performed with off-repo Python using the standard library; no repository verification tools or tests added.
- Rendered the exact four revised Markdown texts using GitHub `POST /markdown`, `mode=gfm`, `context=jermur788/CCF`. Parsed returned HTML for table structure, real code/fence elements, literal backticks and companion links. The API omits browser-generated heading IDs; relative fragments were checked against slugs derived from rendered heading text, **not clicked in a browser**. No browser qualification performed.

| File | Table rows (including headers) | Code spans | Pre blocks | Unchecked / checked boxes |
|---|---|---|---|---|
| spec.md | 7 / 7 / 9 / 10 / 5 | 143 | 0 | 0 / 0 |
| plan.md | 5 / 10 / 10 / 14 | 30 | 1 (architecture diagram) | 0 / 0 |
| tasks.md | 5 / 8 / 9 / 10 / 8 | 31 | 0 | 0 / 0 |
| requirements.md | 10 | 36 | 0 | **72 / 0** |

- Decision registers are byte-identical across all four; nine rows cover C-1A, C-1, C-2a, C-2b, C-3…C-7, all explicitly OPEN. No obsolete P1-G0 token, escaped backtick artefact or checked requirement box. Existing Mode A/B and conditional history equality retained. N01–N13 are **test designs**, never reported as executed tests.
- Final publication procedure: fetch/read back **all five** files from GitHub at the exact published tip and compare bytes/SHA-256 with local files; compare that tip against `3af7b7ff30804bc9aff0d19ea394b9c45946c5a8` and require exactly **four MODIFIED + one ADDED Markdown**, zero other paths; render/read this handoff and recheck links. The actual final tip/readback result is recorded in #552's closure receipt only after completion. A failed readback blocks closure.

Durable off-repo documentation receipts: `/media/jer/Files/CCF/Operations/CCF-552-DesignReceipts-20261115/` contains RUNNING/publication bodies, exact rendered HTML, documentation-check records and GitHub readback/diff evidence. Helper scripts there perform documentation operations only and are not tracked project code.

**Not run / not established:** C# compilation, dotnet or lab suites, mutation/fixture execution, browser qualification, Unity Editor/Mono capture, annual simulation, runtime mortality/replay/security/P0 regression/performance, GROWFOR rerun, field validation, game integration, user acceptance or Windows build. No source/fixture/test/capture bytes were created. No simulation/save change exists; proposed formats and rules have no implementation authority.

## Handoff, limitations and next gate

**SPEC-DRIVEN HANDOFF:** `MULTI-P1-ADULT-SITKA-RULES-COMPARATOR-V1` / **DRAFT**. Assigned #552 documentation design revision complete; **new design not independently reviewed**, acceptance criteria only specified, implementation/spec conformance **NOT VERIFIED**. All unresolved requirements and owner/technical timing are in the decision register. No approved specification was changed.

**Recommended next step:** Manager #425 assigns a **new independent non-author source/science/spec reviewer**, preferably source-literate across event/file safety, floating-point capture and stand-model evidence. Give that reviewer this immutable candidate/report, exact five-file scope, the binding packet, baseline and prior repair chain. Request **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED**, with concrete findings, not product/spec acceptance. After that review, the Manager puts actual C-1A/C-7 and remaining technical choices to the owner; C-6 scheduling remains separate and Scenario One takes priority. Do not dispatch P1-01/source work now.

Key limits for that reviewer: source-capture observability without changing Mono arithmetic, actual 50 m source geometry feasibility, future parser/canonical golden vectors and file-capacity qualification, diagnostic H vs original field method, and synthetic age/SI comparability. None is hidden by a claimed parity/forecast PASS.

**Dashboard sync: NOT VERIFIED. Published dashboard reflects task: NOT VERIFIED. Drive sync: NOT VERIFIED.** Packet forbids coordination/canon/dashboard/Drive edits or regeneration; no publisher was run and no generated state manually edited. Manager must reconcile authorised operational state and verify later publication separately. Closing #552 as **HANDED-OFF** records delivery for independent review, not acceptance, integration or dashboard completion.
