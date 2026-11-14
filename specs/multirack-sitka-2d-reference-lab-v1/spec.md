# Multi-Rack Research — Irish Sitka 2D Reference Laboratory V1

- **Feature/spec ID:** \`MULTIRACK-IRISH-SITKA-2D-LAB-V1\`
- **Status:** **DRAFT — NOT APPROVED; IMPLEMENTATION NOT YET AUTHORISED**
- **Owner/author:** dedicated Multi-Rack Simulation Research Manager (OpenAI GPT-6, documentation-only)
- **Created:** 14 November 2026
- **Source task:** [#483](https://github.com/jermur788/CCF/issues/483)
- **Scope:** entirely **standalone/offline 2D synthetic single-Sitka research simulator**; no Unity, game-world save format, production ecology or shared code changes
- **Product authority:** user's direct Sitka-first, flat-stand, incremental-evidence direction; subordinate to Game Brief, Decision Log and Current Milestone. This draft is not a decision-log entry or a change to Scenario One.
- **Research:** \`GROWFOR_Technical_Evidence_Extraction_v1.md\` supplied Game Dev evidence; [CCF research index](https://github.com/jermur788/CCF/blob/main/Docs/Project/research-index.md) (D-048, Broad & Lynch 2006, GROWFOR caveats); [#472 validation ladder](https://github.com/jermur788/CCF/blob/research/multirack-samsara2-validation-20261113-v1/Docs/Handoffs/MultiRack-Simulation-20261113/SAMSARA2-MARTELOSIM-CCF-VALIDATION-METHODOLOGY-AND-EVIDENCE-GAPS-20261113.md); [#475 returned research and corrected review](https://github.com/jermur788/CCF/blob/docs/multirack-manager-475-research-verdict-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-475-IRISH-MARTELOSCOPE-DEEP-RESEARCH-REVIEW-VERDICT-20261114.md); [#479 blocked raw-archive review](https://github.com/jermur788/CCF/blob/docs/multirack-manager-479-alternate-gbif-review-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-479-GBIF-BLOCKED-AUDIT-AND-ALTERNATE-ACCESS-VERDICT-20261114.md).
- **Implementation target (proposed, not locked):** \`Tools/MultiRackSitkaLab/\` as a new isolated directory under CCF; actual local worker must inspect source tree/worktrees, route/pin branch and claim ownership before edits.
- **Outcome:** research engineering prototype and independent validation harness **not a player-ready Scenario One or verified Irish growth model**.

## 1. Problem and justified early implementation boundary

The eventual game has a **shared individual-tree forest state** visible as both a **2D management map and 3D walkable forest**. A player walks a representative rack, marks individual trees, sees all decisions on the Stand Map, receives guarded proposals elsewhere, inspects the proposals in 3D, approves a Work Plan and observes genuine multi-year outcomes. Full five-rack sample-to-comparable-area transfer and 3D synchronization **remain a later spec**, not part of this lab.

The owner now wants a **simple Irish Sitka-only starting point**—flat land first, complexity gradually added by species and site—using the strongest existing Irish Sitka/GROWFOR research. A true Irish mapped marteloscope would be valuable, but [#479](https://github.com/jermur788/CCF/issues/479) ended with **raw records NOT INSPECTED** and no plot verified as import-ready. No verified plot coordinates, complete IDs/DBH or explicit data licence for a chosen real plot have been obtained. **Do not block a synthetic research prototype on that external data-access gap, but do not invent a real plot.**

The prototype first answers **internal engineering correctness**: can one tree table support spatial visualization, marks, proposal/approval/execution distinctions, stand arithmetic and reproducible history? A second, later-gated phase can compare annual Sitka dynamics to existing CCF implementation and independent Irish aggregate evidence. These are distinct from true **empirical individual-tree prediction accuracy**.

## 2. Goals, in-scope phases and non-goals

### P0 — independently implementable after spec approval (no further external research)

A **synthetic flat Irish Sitka research stand** with stable individual-tree IDs, deterministic generated or explicitly imported synthetic source rows, true local XY, measured-style-but-synthetic DBH/height/status fields and explicit plot boundaries. One local 2D view and one headless domain model expose the exact same trees and statuses. Users can select and mark real IDs in this synthetic stand, inspect change sets and queue a **proposed/approved/pending/executed** thinning experiment without implying actual real-world felling. Deterministically compute and record adult count, basal area (m² and m²/ha), density, quadratic mean diameter and cut/retained statistics. Provide snapshot/replay across mock calendar years **without inventing biological growth**. Source class must always say **SYNTHETIC TEST FOREST**.

### P1 — conditional technical/model qualification and annual biological stepping

After engineering P0 passes and a read-only source/evidence inspection gate, add a **versioned, deterministic, explicitly labelled annual Sitka test model** for adult DBH/height/competition and separated natural mortality, with optional cohorts/light/regeneration only under their own evidenced feature review. Prefer **parity of any reusable/current CCF Sitka logic** to a new unproven ecology system; evidence and source declarations are mandatory. Validate stand-level trends with Broad & Lynch/GROWFOR **as aggregate reference within its supported pure even-aged Sitka domain**. A laboratory model that diverges is recorded as a research hypothesis; it does not silently override CCF production or become a second game authority. If model identity/parity cannot be established, P1 is **BLOCKED** and P0 remains useful.

### Explicitly out of scope

No import of a purported exact Curraghchase/Donadea/Oughaval/Glenart or other measured plot; no GBIF account, vendor download or restricted field data. No Oak, Beech, sycamore, mixed species, topographic slope, water/wind, soil stratification, real timber prices, contractor/economy integration, canopy photorealism, 3D, Unity, Rack V2 geometry/persistence, five-rack extrapolation, automatic Crop Tree quality selection, autonomous approval, real old-save migration or Windows/Player build. No claims about Irish forestry industry standard thinning intensity or site thresholds from synthetic game parameters.

**Scenario One remains unaffected:** its first external complete-loop demo still needs Sitka/Oak/Beech comparable depth, S1-C/S1-D and relevant accepted MFM requirements. Sitka-only here is a deliberately separate research build, never a reduced Scene One scope.

## 3. Authoritative state and architecture contracts

**A1: Single local test forest.** One canonical \`ForestState\`/immutable snapshot has \`forestId\`, \`labSchemaVersion\`, \`sourceClass=Synthetic\`, \`simulationYear\`, \`standAreaPolygon\` (local metres), \`siteProfile\` with \`slope=0\` and explicitly tagged site parameters, and collection of unique \`TreeState\`: stable \`treeId\`, \`species=Sitka\`, \`xM,yM,zM=0\`, age/year, DBH cm, height m, status live/dead/felled, optional measured vs synthetic provenance flags, and any later model fields with units. A plotted circle is a projection of TreeState, **not a separate UI-only tree**.

**A2: Layered separation.** \`Domain\` houses trees, geometry, selections, action events, area/BA arithmetic and deterministic state transition. \`Models\` handles optional/versioned P1 annual ecology, never UI. \`Ui\` is a browser/2D map backed by the domain through a small local-only API/serializer. \`Verification\` generates fixtures and exercises headless invariants. No \`Assets/\`, UnityEngine references or game save files. Target .NET C# core and a minimal local-only 2D browser front end, subject to actual tooling audit; do **not** create a parallel Unity runtime. Any future interface adapter is a separate spec/integration decision.

**A3: Synthetic fixtures.** A seeded deterministic generator or checked-in **small synthetic** fixture (the fixture is unambiguously authored, not real field data) supplies N, age, diameter/height distributions, spacing policy and plot boundary. **All distributions/values are explicitly tagged TEST FIXTURE/GAMEPLAY CALIBRATION** unless anchored to a particular Irish source. A convenient **100m × 100m** computational rectangle and a small fixed adult count are allowed as test fixture choices **only**, not as claim about Irish forestry. Configuration must reject nonpositive DBH/height, invalid years/units, duplicate IDs, non-finite/out-of-bounds points (unless explicit edge ledger) and missing polygon. Support seed, input checksum, output checksum and reproducible ID sorting.

**A4: Stand arithmetic** uses legitimate non-overlapping plot area (shoelace polygon area with metre coordinates). Living qualifying adults only: \`BA_i = π (dbhCm_i / 200)^2 m²\`; sum unique stems once, \`N/ha = N / areaHa\`; \`BA/ha = sumBA / areaHa\`; \`DqCm = sqrt(sum(dbhCm_i²) / N)\`. DBH threshold is explicit in the fixture manifest; do not hardcode GROWFOR/marteloscope adult cutoffs as universal. Any top-height statistic must declare an **age/tree sampling rule**, not invent one. Dead and felled trees remain stable identities with proper ledger disposition, not automatically removed from history.

**A5: Decision/authority pipeline** uses explicit status stages:
\`UNMARKED → PLAYER_MARKED → PROPOSED → APPROVED → PENDING_WORK → EXECUTED\`, with \`REJECTED/CANCELLED\` or \`MANUAL_REVIEW\` alternatives. **No physical state change for mark, proposal, preview, approval or pending work.** At P0 only an explicit **Execute Test Work** action changes live status to felled, once per unique living eligible tree; it creates an event with source state/year, player intent, actual ID list and after-state/checksum. This UI label is a test action, NOT real contractor completion. Undo of a pending mark is allowed; executed work is immutable history in the event stream and must not be undone by changing only the map.

**A6: Years & persistence.** P0 may increment an experiment calendar to link decisions and snapshots, but growth, death, recruitment and harvest **must not occur without an explicit authoritative event**. Save lab-only versioned JSON with **distinct format and directory** from CCF production \`ForestSaveData\`; never parse, open or overwrite protected saves. Replay events deterministically and verify snapshots after reload. P1 annual advance must use the actual living set after all past executed work, process each annual step once, apply no silent harvest, preserve stable historical IDs and record versioned model/profile/calibration source for every result. A no-treatment year is not a no-growth year in P1.

**A7: Future spatial adaptability.** Keep local XY, polygon, unique IDs, time and action history independent of rendering. Later rack IDs, manual sample source, polygon masks, 3D rendering, species parameters and terrain are additional adapters, not built into P0. Preserve source/proposal/work execution distinctions required by later multi-rack gameplay, and no automatic coverage or source→rack clone in V1.

**A8: User-facing scientific honesty.** Every screen/report displays \`SYNTHETIC TEST FOREST\`, model name/version, whether P0 **NO BIOLOGICAL GROWTH** or P1 experimental ecology, selected site parameters and any missing/unsupported fields. Never present resulting annual values as measured Irish outcomes, crop-quality advice, Irish thinning rules or proof of multi-rack extrapolation.

## 4. P0 2D map and interaction requirements

- Local-only desktop browser application (no sign-in/network dependency). Read actual forest-state values from a single core, 2D top-down plot in metres. Pan/zoom, visible scale bar, whole plot bounds, hover/selection showing stable tree ID, species, x/y, DBH, height, live/felled, provenance and mark/work stage. Distinct visible layers for live, selected, proposed, approved/pending, executed/stump; color alone never sole distinction.
- One-click or explicit tree-ID selection; multi-select proposals with immediately updated **preview metrics** (N, BA, BA fraction of current live stand and affected IDs). Proposed preview must never change actual inventory, year, biological response or cash.
- Separate **Approve Experiment Plan** and **Execute Test Work** actions with an explicit review table of exact affected IDs, counts and BA. Must reject dead IDs, duplicate IDs, cancelled marks, out-of-scope stems and stale snapshots. Aborted/rejected work leaves authoritative inventory unchanged.
- Local controls: regenerate/reset only disposable lab fixture with confirmation; switch to saved synthetic fixture; load P0 local file; advance/replay test year; export provenance manifest + event log + stand summary. Never offer “Open Scenario One save.”
- Reasonably usable for small test fixture and scaled independent performance fixture; no requirement for 3D, polished game art, map GPS, full accessibility certification or automatic rack proposal layer at this stage.

## 5. P1 evidence and biology gate

P1 **cannot** be declared scientifically correct solely because annual numbers look plausible. Before any model-writing:
1. Inspect current CCF Sitka adult/height/density/mortality code and accepted D-048/D-059 compatibility records at exact source HEAD, including model-version conditions. Identify the source method and deterministic test vectors. **Do not derive current code truth from the historical Unity overview alone.**
2. Compare against supplied **GROWFOR technical extraction** for the precise state \`(B,N,H)\`, age, site-index definition (top height at age 30), thinned vs unthinned and valid data range; explicitly note lack of proven irregular-CCF individual-tree calibration, older source vs executable-version caveat and British max-density transfer. Never call a copied pure Sitka height curve a “GROWFOR-based individual-tree model.”
3. Choose a bounded implementation strategy from audited source: (i) **faithful independent test adapter/port** of accepted CCF Sitka mechanics with deterministic parity vectors, or (ii) **explicitly experimental** alternative kept isolated behind a labelled model variant and independent spec addendum. If current CCF source contains unverified/out-of-domain formulas, identify/label them rather than endorse them.
4. Require annual ecological account by tree ID: before/after DBH, height, alive status, growth and natural mortality (never conflated with harvest), competition neighbourhood and source profile; if light/regeneration unsupported, mark **NOT IMPLEMENTED** instead of invented numbers.
5. Benchmark pure even-aged Sitka **stand-level** N, BA, top height and growth trends against Irish GROWFOR/Broad–Lynch evidence within documented applicable age/site/stand conditions. Model-vs-reference difference, units, time horizon and limitations must be reported, never silently tuned on the same holdout. GROWFOR cannot validate the individual neighbour release, 3D crown, continuous-cover regeneration or Irish uneven-aged repeated-thinning accuracy. L4 real observed same-tree validation remains **NOT VALIDATED** unless legally usable longitudinal field observations are obtained.

**P1 separate go/no-go:** after P0 PASS, an independent model/source evidence review must approve the specific current CCF baseline, intended parity vector/science labels and model adapter. If absent, implementation remains P0-only—this does not mean further general Deep Research is needed for map/marking/BA calculations.

## 6. Acceptance criteria (P0 binding once approved)

- **AC-01** New standalone build/test files exist only in the agreed \`Tools/MultiRackSitkaLab/\` lane (or separately manager-approved non-overlapping folder); no Unity source/Assets, scene, package, prefabs, player saves, current specs, decisions, coordinator board or frozen Reference Future touched.
- **AC-02** Same stable tree ID, XY, DBH, status and action stage in headless state and displayed 2D map; exact single data authority, no duplicate biological store.
- **AC-03** Same seed, fixture/config/schema version produces identical initial JSON bytes and metric summaries across consecutive runs and after lab save/load/replay.
- **AC-04** Exact BA, density, area, Dq and unique selected removal BA independently verified from hand-computable fixtures including dead/stump/duplicate handling and non-square polygon; finite values.
- **AC-05** Mark → propose → approve/pending without executing changes **zero living count, BA, tree state, event harvest, costs or growth**; confirm explicit one-time execution changes precisely the eligible selected IDs with correct BA and event history.
- **AC-06** Reject repeated execution, stale ID, already dead tree, duplicate order/tree, invalid polygon or unit, unexpected earlier snapshot, and overflow/NaN without corrupting state. Cancellation and manual-review are safe.
- **AC-07** A saved experiment is versioned and isolated, with sourceClass Synthetic immutable provenance; CCF production saves and frozen Reference are not searched, read or written.
- **AC-08** UI allows plotting, filtering, inspection, selection, proposal preview, separate approval/execution and replay with non-color status labels; screenshots and a scripted browser/demo path verified on actual app.
- **AC-09** Test fixture says \`SYNTHETIC TEST FOREST\`, model is \`NO BIOLOGICAL GROWTH\` for P0, and no misleading GROWFOR/Irish prediction claim. An “Advance Year” in P0 is explicitly **calendar/event replay only**.
- **AC-10** No real Irish dataset import, no marteloscope “exact replica” label, no multi-rack auto-prescription or Unity synchronization shipped under P0.
- **AC-11** Negative-case, performance, source ownership, dependency/licence, test evidence and reusable data/API contract documented; independent bounded review concludes **APPROVED/NEEDS CHANGES/REJECTED**. P0 verification is **not** acceptance of scientific predictions or future game integration.

P1 cannot PASS by satisfying P0 alone. Its metrics, model/age domain, parity and holdout gates must be separately reviewed before implementation and labelled claims.

## 7. Decision gates, ownership and exclusions

**No blocking further external research for P0.** We already have adequate information to implement a synthetic flat-stand engineering harness after this DRAFT is independently reviewed and approved. **More research/verification is required for Irish empirical ecological accuracy, P1 numerical model choice, true marteloscope input and long-run multi-rack transfer.**

Product-level open decisions that must be explicit at **DRAFT review/approval**, not silently guessed by code worker:
- **OPEN-L1:** Is this an *independent research/engineering tool only* (recommended), with no promise to ship as the player interface, or should a later spec cover a distributable 2D game mode? This draft assumes research-only.
- **OPEN-L2:** Confirm P0 **no biological stepping** and P1 separately gated (recommended) versus source-parity Sitka growth required in the first implementation. This draft recommends P0 first to avoid false science/complexity.
- **OPEN-L3:** Confirm isolated local .NET + browser UI versus alternate simple implementation after host inventory; must remain Unity-free, preserve shared-state API and be testable locally.
- **OPEN-L4:** Whether any future field inventory can be loaded remains **UNRESOLVED/NOT P0** until GBIF/Teagasc actual data/rights pass; no automatic decision.
- **OPEN-L5:** Exact stand stocking/DBH defaults are intentionally **engineering fixture values**, not accepted Irish ecology constants; adopting an Irish evidence-constrained baseline is a separate calibration step.

**Authorisation:** DRAFT forbids production/implementation work. After independent source/science/UX/ownership review, a delegated overall product owner must explicitly approve V1/P0 scope, advance to APPROVED, and issue an actual bounded implementation claim/packet in a new safe lane. The Scenario One #424 manager retains sole live coordination writer and Unity scheduling authority. No Windows build before user personal playtest + explicit authorisation; this stand-alone tool does not request one. This spec itself grants no host ownership or implementation lease.

## 8. Verification ladder and success meaning

- **L0 data reproducibility:** P0 synthetic fixture provenance, schema, seed, ID membership, polygon and same-values replay → independently testable now.
- **L1 internal software correctness:** mark/approval/execution, area, basal area, unique removal and annual history → independently testable now.
- **L2 behavioural ecology:** P1 source-qualified CCF Sitka test model and directional thinning/competition checks → later; not guaranteed by P0.
- **L3 stand-level quantitative comparison:** reference Irish GROWFOR/Broad–Lynch within valid pure even-aged Sitka domain and model/version caveats → later controlled study, not identical individual-tree futures.
- **L4 Irish predictive field accuracy:** repeated real measured after-treatment outcomes with adequate source/holdout rights → not available/verified here. Never claim based on synthetic P0 or Samsara/GBIF initial-state match.

**End-state of first implementation:** a real, inspectable 2D individual-tree research UI with deterministic action/event accounting and no false ecological claims—ready for source-evidence-supported annual model work. This is useful parallel work but is **not** the promised final 2D↔3D Multi-Rack game.
