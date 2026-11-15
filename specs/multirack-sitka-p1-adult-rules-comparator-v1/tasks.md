# Sitka P1 adult-rules comparator — DRAFT task breakdown

**Status: DRAFT; NOT IMPLEMENTATION AUTHORISED.** [spec.md](spec.md), [plan.md](plan.md), [requirements](checklists/requirements.md). Direct owner 15 November 2026 permission extends only to producing and independently reviewing these **four files**, not accepting them. The table is a proposed roadmap **after** explicit future P1 spec/product approval.

## 1. Tasks and gates

| ID | Work and scope | Difficulty / recommended provider | Dependency/ownership gate | Deliverable and actual qualification |
|---|---|---|---|---|
| **P1-S0** | **NOW:** independent cross-file P1 DRAFT review, including all reviewer m-1…m-8 and C-1…C-7; identify ambiguity and product choices; NO implementation | **HIGH**, independent non-author Claude Opus 5.5 High or suitably capable independent forestry+systems reviewer | MULTI-011 GitHub handoff; read-only source and evidence; own fresh docs review branch/claim | One independently published review Markdown, verdict **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED**; **does not** approve implementation or resolve open product decisions automatically |
| **P1-G0** | Owner/Manager scope decisions, resolve/review C-1…C-7 and DRAFT changes, approve SPEC if desired | **HIGH**, product owner + Overall Manager, with independent review evidence | P1-S0 accepted, no material unreviewed product deviation; exact file/source IDs | Separate accepted product/spec decision and authority update, approved four-file files on a new safe branch; **cannot be inferred from this DRAFT** |
| **P1-01** | Pin current CCF source and **capture actual runtime parity oracle** in restricted pure-Sitka reference-demand model2 route; provenance, f32+double operations, annual ordering and exact per-ID outputs; repair stale schema23 verifier assertion separately without weakening | **HIGH**, Sol/Codex High local source/Unity verification or reviewed source-equivalent .NET harness; independent method reviewer | **P1-G0** and explicit allowed source/Unity lane, actual clean branch/HEAD, live ownership, machine lease if Unity; C-2…C-6 resolved | Verified current-source fixture inputs, actual per-ID CI/DBH/H/crown/p/roll/victims and model policies, repeatable source oracle, negative mutations/threshold guards, signed handoff. No game/source release |
| **P1-02** | Isolated adult-only P1 model adapter with versioned identity, source-faithful float/branch/order and explicit unsupported-domain refusal; immutable P0 input | **HIGH**, one primary local C# writer (Sol/Codex High or equivalent) + separate numerical reviewer | **P1-01 accepted oracle**, own exclusive P1 Core file set and separate worktree; no P0 schema edits | Actual adapter per-ID parity for model1 and pure-Sitka Reference-demand model2, actor/year deterministic victims, no-growth P0 regression; independent source review |
| **P1-03** | Distinct P1 yearly transaction, event hash chain, work-plan staleness, atomic natural death/felling, P1-only safe file root/save/load/replay | **HIGH**, SAME single P1 C# Core/persistence writer as P1-02 or carefully serialised handoff; independent file/security reviewer | **P1-02** and approved C-1 format + P1 state semantics, reviewed file-safety contract; avoid concurrent source ownership | Per-ID annual ledger with exact year and provenance, no double deaths, plan safety, unknown-version/refusal, split-run save replay equal to continuous run, P0 bytes/hashes unchanged |
| **P1-04** | Historical pure even-aged **stand-level** \`BroadLynch2006_Sitka\` B/N/H comparator, no-thin/one-thin/repeated-low-thin domain-labelled trajectories | **HIGH**, independent forestry/evidence analyst, scientific and numerical independent reviewer | Stable **P1-02** source output/units; may run in parallel with P1-03 in separate read-only evidence lane | B/N/H comparison report and source/evidence matrix, SI/site and measured definition caveats, no Irish per-tree field-accuracy claim; reuse 24+18 suite as internal consistency only |
| **P1-05** | Minimal opt-in P1 2D UI/exports, no-growth P0 isolation, platform security and true browser workflow | **MEDIUM–HIGH**, UI/C# integrator only after API contract stable; independent real-browser reviewer | **P1-03** + **P1-04** science labels; protected UI/Host ownership, local Windows/WSL access or dedicated Linux x64 lane | P1 labelled year-by-year plot and per-ID/stand ledger, correct source/domain/errors, pass P0 exact tests and scoped P1 interactions, Windows Edge/WSL and loopback/security evidence; external-demo/game integration not inferred |

**Only P1-S0 is the logical next review after the DRAFT publication.** Even P1-S0 requires a separately created claim and task packet; it is not active just because this row exists. G0 and P1-01…05 are **BLOCKED / NOT RELEASED**.

## 2. Pre-task source/ownership protocol (all future workers)

Before local inspection/tests/edits, the worker must:
1. Read the latest direct owner decision, [Manager LAB-06 evidence-plan verdict](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md), exact APPROVED spec **if one exists**, current \`Docs/Project/decision-log.md\`, \`current-milestone.md\`, and **live** \`coordination/active-tasks.md\`.
2. Inspect actual source branch and **full HEAD** and the physical/remote worktree. Check other active source owners, dirty work, disk, running processes/Unity lease. **No issue claim grants a file lock**, and a lack of board row is not affirmative ownership.
3. Register actual model/provider, machine/WSL distro if relevant, branch, worktree, starting full HEAD, current exact code/snapshot and allowed files as **[RUNNING]** before edits; mark **[BLOCKED]** if collisions exist. Use one writable lane per owner/subsystem. Never stash/clean/discard/kill unrelated work.
4. Follow authorized phases for heavy Unity. Qualification is against exact source candidate. Do **not** run a Windows Unity build without personal Scenario One candidate playtest + explicit owner authorisation. A Windows Edge browser using the standalone WSL lab is a different task.
5. At handoff, publish exact GitHub Markdown report with tests/negative controls/current status labels, obtain GitHub readback and diff; mark claim HANDED-OFF and close. Manager independently reviews and issues APPROVED/NEEDS CHANGES/REJECTED before releasing next task.

## 3. Proposed P1-01 parity vectors and source gates (before implementation)

**Current baseline in Codex/Opus source audit:** main \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`, CCF schema 24, new-game growth2, all-living density, game SiteProductivity=1, GrowthModel2 pure Reference Sitka model1-like adult density route. This is *historically inspected*, not an irrevocable current source version.

### Runtime-capture required cases
- C01 scalar DBH/CI = **Codex V01/V02**, also neighbour-matched full stand for both;
- C02 8 m inclusive border, beyond-cutoff, same XY floor, ordinal-ID/tie accumulation and world-coordinate origin;
- C03 relative Sitka height at age20→21, threshold/stage Mature and explicit P0 synthetic H/SI mismatch;
- C04 crown current value after growth, pre-growth CI, target and relaxation; initial crown is synthetic and separately stated;
- C05 RD 0.6 onset / RD1 boundary on all living, and explicitly all-living versus P0 actionable subset;
- C06 low/high suppression and density hazard, RNG model0 and 1 plus exact full ID/year seed, victim order/cause/year and singleton tiny-area escape from RD≤1;
- C07 executed one-neighbour fell **before** annual step versus approval-only and after-year fell; no second same-year release; natural mortality separate from work felling;
- C08 pre-inventory \`Dead\` and \`Felled\`, no duplicate death awards/forged cause, no storm/juveniles or mixed demand;
- C09 first year vs second/long years, direct-run vs save/restart continuation, model1/pure-Sitka model2 and growth0 diagnostic rejection; source-format/unknown-model refusal;
- C10 negative source controls: alter CI sort/float evaluation, invalid origin geometry, swap all-living to action-only RD, RNG model confusion, missing cause/year, duplicate death, serial rounding drift, P0 migration.

**Important:** V01–V09 were checked by independent Python arithmetic only, V10–12 derived, V13 current Unity parity and V14 field holdout **NOT AVAILABLE**. Oracle collection must turn V13 into runtime evidence before declaring parity. V14 is not available and not needed for honest *CCF-rules research comparator* claims, but is mandatory for any Irish field-accuracy claim.

**No weakened historical verifier:** Unity source save schema 24 vs Sitka harness version 23 assertion is a known stale test, not proof of game defect. Any correction/repair has its own source writer, baseline control and independent review. If not authorised, runtime parity capture remains blocked rather than forging results.

## 4. Proposed P1-02/03 acceptance and negative checks

### State and source equivalence
- P1 reads only a verified **P0 synthetic** source with pinned file+snapshot hashes; never modifies the original fixture/snapshot or P0 \`Advance experiment step\` semantics.
- All P1 living trees are explicitly **Mature-equivalent**, pure Sitka/Reference demand; no site variation, pruning, growth phases, storms, recruits, mixed species. Reject unsupported source states with clear reason.
- All living trees (even 9.6–9.8% below actionable P0 threshold) contribute to CI/RD/growth. P0 actionable metrics remain separately named. End-of-year updates honor source clamp and numeric policy.
- Initial \`Dead\` trees have no claimed historical field cause/year; retain explicit \`pre-inventory\` synthetic provenance and exclude from ecological deaths. No newly simulated natural death is confused with work felling.
- CCF source uses **float32 values and mixed double intermediates**; future oracle must establish a safe serialization/rounding policy, including numerical-near-branch outcomes and deterministic year/ID RNG. No new arbitrary coefficients or "better" ecology.

### Event/replay and file security
- Explicit one-way P0→P1 import, distinct version/file root; P0 reader cannot accept P1, and malformed/unknown P1/foreign CCF save is never loaded as synthetic P1.
- A full annual event includes pre/post tree changes, mortality roll/cause/year, felling work provenance, plan staleness as needed, source config and canonical P1 hash; reconstruct from named fixture and ordered ledger.
- No felling before valid **Execute**; mark/propose/approve/queue do not change living state; an executed work plan fells exact reserved IDs; natural death never awards timber; approval/session revision must invalidate on biological year and plan-tree natural death.
- Continuous two-step run and one-step-save-restart-load-step yield identical P1 tree list/state/event chain under pinned reference. Invalid/duplicate/dead IDs fail atomic; no partial time advancement or hidden post-failure mutation.
- P1 files only in a guarded lab root on supported Linux x64 / Windows Ubuntu WSL2, never writes production forest saves; secure open/read/write/rename/unlink, no directory cross-write on race, no FIFO hangs, no symlink/hardlink escape, maximum size, controlled Host/Origin/CSRF/127.0.0.1 binding and true save response. Native unsupported OS fails closed, no path-based weak fallback.

## 5. Proposed P1-04 stand-science design

- Reuse \`Tools/Verification/Growfor\` (historical tests **24/24 +18/18** from independent scratch), but verify its actual source and version against \`GROWFOR_Technical_Evidence_Extraction_v1.md\`. It is named **BroadLynch2006_Sitka**, **not** necessarily the distributed current GROWFOR executable.
- Report \`B\` in m²/ha, \`N\` stems/ha and \`H\` (declared top-height definition and missing original sampling equivalence) under matched area/age/site-index/thinning history. P0 uses all living only for newly named P1 biological values, not the P0 action metric.
- Include one unthinned, one conventional thinning and one permitted repeated-low-thinning model scenario with domain metadata; if a scenario is irregular/out-of-domain, report comparison as exploratory and not validation.
- The 113 nonresearch observations were used in **volume-equation calibration**, not an untouched full final growth-model holdout. Published NFI marginal ranges/aggregate errors are not P1 thresholds, and GROWFOR offers no cause-specific mortality/individual-tree validation.
- If the output diverges from Broad–Lynch, record direction/magnitude and sources. No tuning to match reference or applying calibration to CCF production without separate research/product approval. Real field holdout absent.

## 6. Proposed P1-05 browser and regression qualification

- UI has a visible **opt-in experimental P1 mode**, never auto-advances biology in P0, and displays the full scientific honesty label on screen and exports.
- Show original synthetic fixture, measured-style but synthetic initial data, source-model/RNG versions, age/year, per-ID DBH/height/crown and natural/work-felling separation, all-living and actionable subset N/BA, B/N/H stand aggregates and applied domain limitations.
- Use only source-derived/adapter-derived values, no duplicate browser-side growth/mortality algorithms; no false GROWFOR authority on per-ID selection or "Irish thinning prescriptions".
- P0 standalone build, Core 82/82, Host 13/13, fixture manifest, schema and real browser semantics remain unchanged. Security/mutation/replay checks targeted to the actual touched path. Do not require a full Unity Tier-3 programme for an isolated lab, and do not replace P0 Chromium evidence with a merely headless non-UI test.
- Windows Edge on Ubuntu WSL2 at literal \`http://127.0.0.1:5187/\` is a verified P0 route; P1 requires its own actual Windows browser check and no LAN exposure. No native Windows lab-file support claim.

## 7. Existing independent findings and readiness status

| Review item | Draft task disposition | Required future gate |
|---|---|---|
| m-1 stage-sensitive height | Mature-equivalent only in spec/negative cases | P1-01 source capture; reject non-Mature |
| m-2 site=1 current behaviour | Source parity, not optional experimental productivity | Oracle source pin |
| m-3 P0 pre-existing dead without cause/year | Synthetic pre-inventory source status and no new mortality event | P1-03 state format/replay |
| m-4 float32/double mixed math | C-2 numeric/roundtrip/branch policy | P1-01/G2 spec decision |
| m-5 square CCF vs polygon P0, origin | source-matching square fixtures; polygon adapter not parity | P1-01 fixtures/G2 |
| m-6 RNG model independent of growth model | separate versions and deterministic rolls | P1-01/03 |
| m-7 113 stands volume-model use and page bases | corrected provenance statements | P1-04 science audit |
| m-8 fixture threshold/density/SI gaps | separate dense, sparse, top-height and below-threshold vectors | P1-01 |

**Current readiness:** P1-S0 spec review is next; **P1-G0 pending**, P1-01…P1-05 **NOT RELEASED**. A later reviewer may recommend a smaller P1 experiment or NO-GO. P0 remains fully usable under its existing Windows/WSL limits, and the complete Scenario One first external demo remains the primary game milestone. Dashboard/Drive NOT VERIFIED.
