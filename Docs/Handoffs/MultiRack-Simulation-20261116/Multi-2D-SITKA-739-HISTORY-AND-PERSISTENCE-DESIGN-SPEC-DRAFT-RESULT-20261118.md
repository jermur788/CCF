# Multi #739 Lane D — History and persistence design specification DRAFT

**18 November 2026 · DRAFT ONLY · NOT APPROVED / IMPLEMENTED / VERIFIED / INTEGRATED**

**Claim:** [#739](https://github.com/jermur788/CCF/issues/739). **Manager:** [#730](https://github.com/jermur788/CCF/issues/730). **Shared-contract counterpart:** [#740](https://github.com/jermur788/CCF/issues/740).  
**Actual worker:** OpenAI GPT-6, ChatGPT Game Dev with GitHub connector/browser-only harness. No native host, worktree, CCF-Ops custody, Unity, physical session ID or configurable effort setting exposed.  
**Branch:** `docs/multi-739-d-spec-draft-20261118-v1` from freshly observed `main` starting commit `b7421c9a5cbf63a1409fe7ae8eed304fdfd4a7e9`.  
**GitHub write ceiling:** this one new handoff only. No changes to `specs/`, `Docs/Project/`, `Tools/`, any fixtures/save, board, source, tests or main.

## 1. Evidence and authority

This proposal implements the *documentation assignment* in the [complete #739 Lane D packet](https://github.com/jermur788/CCF/blob/docs/multi-730-parallel-workstreams-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-PACKET-20261118.md), not the game. It is subordinate to the latest owner direction, `main` Decision Log, Current Milestone and Game Brief. The separate [owner-approved P1 four-file technical specification](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md) and its [exact owner approval packet](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md) are existing approved **design authority for bounded packets**, not proof of implemented P1-03 saving. Their historical DRAFT paragraphs are not the current approval status. This new #739 supplement is **not approved**.

**Decision-name collision:** the separate Multi P1 product-approval D-084 was proposed on an unmerged Multi documentation lineage; current `main` D-084 is a **Scenario One** decision. Cite the above P1 owner packet for Multi authority, not `main` D-084. Canonical reconciliation belongs to Manager #730 and Lane E #740.

| Source | Verified/reported evidence, at its actual scope |
| --- | --- |
| [#723 Year-1 oracle and readiness](https://github.com/jermur788/CCF/blob/8890a730d349cb7082be34c213503055b8059b1f/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md) | Frozen synthetic 30×30 m fixture and source-derived calculation; excludes work, regeneration, storms, Unity and save. |
| [#725 local Year-1 author result](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md) | One trust-gated atomic in-memory Year 0→1 calculation. All 100 stems grow and age; zero deaths; JSON report has before/after per-ID and before/post-growth/post-mortality stand values. Each CLI invocation starts a fresh genesis; **no durable save or Year 2**. Local source not pushed or integrated into main. |
| [#729 independent changed-lines/numeric review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md) | APPROVED source/numeric review only. **M-1**: dead trees are not filtered before CI/growth/stand in Year-1 adapter; unreachable in current all-live one-year scope, **mandatory repair before Year 2**. |
| [#732 independent focused native qualification](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md) | Offline resumed build PASS (0 warnings/errors), byte-identical twice-run report, 700 per-tree values matching independent #729, P0 and tamper refusal PASS. Its initial network stop remains documented. Not full tests, save, Unity or L4. |
| [#735 immutable source custody](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md) | Original #725 source/evidence retained to 27 Nov 18:00 UTC; #710 repaired R independently to 23 Nov 18:00 UTC, no auto-purge. **Custody not #739 source permission**. |
| [P0 approved lab spec](https://github.com/jermur788/CCF/blob/docs/multi-p1-637-owner-approved-four-file-20261116-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md) | Separate synthetic no-growth calendar lab with its own IDs, plans, events, replay/format. P0 records are not P1 biological years. |
| [Lane E provisional interface](https://github.com/jermur788/CCF/issues/740) | Unapproved `MultiSitkaReadSnapshotV1` proposal: immutable detached read projection, ordinal IDs, source X/Z metres, stage-tagged values and no renderer mutation. E owns shared ABI and import choice. |

**Accepted owner policy:** `main` Decision Log D-082 permits P1's **original independently verified synthetic inventory only** as original input, disallows importing P0 post-intervention history, and requires on **successful load** one `PlanStale(SessionLoaded)` per open Approved/PendingWork plan, **zero load events if no such plan exists**. The synthetic experiment must say **NOT an Irish field forecast**. The existing full-P1 approval adopts a separate early **derived** 30 m stand, leaves genuine original polygons for a later P1 phase, and keeps 40/80 m Unity-square parity as separate later controls. No product choice is reversed here.

**Safety:** Default .NET L4 remains **FAIL38 and UNWAIVED**. No release/integration qualification, numerical Unity parity, saved game, accepted forestry forecast, real 2D UX or cross-host consistency is established. Evidence class: direct referenced worker/reviewer receipts for their own scope; all field/format/UX choices below are **proposals**, not current APIs.

## 2. Two separate deliverables: read-only HISTORY versus durable SAVE

### H0 — history from existing static Year 0 and Year 1 report (initial proposal)

A future view may show a **single frozen, trusted** Year 0↔Year 1 experiment result. This does not need a persistent save format, a long-running engine, an HTTP Host, a managed Unity save, or execution of another ecological year.

- **Year 0:** the accepted derived 30 m genesis and the completed report's `before` tree/stand state.
- **Year 1:** the report's `after-mortality` final state. `PostGrowth` is an **intermediate stage of Year 1**, not Year 2. Pre-growth competition indices were evaluated from the pre-growth population; do not mislabel them as post-year remeasures.
- **Selection identity:** exact, ordinal, case-sensitive tree IDs `P30-001`…`P30-100`. Tree selection and pairwise comparison are derived read operations, never UI pixel/index authority.
- **No mutations:** timeline, filter, tree inspection, view selection, hover, comparing stages and exporting a labelled trace must not call `Advance`, approve/execute work, modify a mark, append a canonical event, write a save or change engine state. A fresh CLI invocation restarts Year 0; it is **not** a historical continuation.
- **No stitched history:** never infer Year 2 from two independent Year 0→1 CLI runs or interpolate/extrapolate a predictive curve. No save and no historic management events are present. Unavailable data must be shown as unavailable.

**Exact compiled Year 0/Year 1 aggregate anchors from #725 (synthetic):**

| Measure | Year 0 before | Year 1 after mortality | Presentation rule |
| --- | ---: | ---: | --- |
| Living adult Sitka stems | 100 | 100 | Zero natural deaths, zero work fells |
| Age (all trees) | 30 | 31 | One increment exactly |
| Living N/ha | 1111.111083984375 | 1111.111083984375 | Recorded source f32-area calculation |
| Living BA/ha, m²/ha | 55.87290918844456 | 58.33954510260964 | **Compiled** #725 basis, not #723's exact-decimal-area preliminary value |
| Dq, cm | 25.303288374529917 | 25.855791791844318 | All living population |
| Relative density | 0.609795828454288 | 0.6375836886879149 | Post-growth/after-mortality identical because no death |
| Directed pre-growth neighbour pairs | 1,622 | No separately demonstrated post-year calculation | Stage-specific, do not invent second count |
| Hazard/boundary deaths | — | 0 / 0 | Absence of death events, not a lifetime guarantee |

Twelve Year-0 stems under **170 mm DBH** remain in biological CI, growth and stand density even though P0 work eligibility excludes them. A history filter may show work eligibility independently, never remove them from biological stand totals. Rounding is for display only; calculations/replay must preserve original float bits or exact round-trip numbers.

### H1 — later multi-year history (not implemented)

When Lane B separately scopes Year 2+, repairs #729 M-1, and publishes an approved engine contract, the timeline can expose each **committed** year-end forest plus optional named within-year checkpoints: `before-work`, `after-work/pre-growth`, `after-growth`, `after-mortality`. Current #725 has no work phase, so a work checkpoint must not be invented now. History can be reconstructed from a trusted genesis and committed events rather than storing every per-ID value forever. A history frame must belong to **one lineage + domain + fixture + year + stage**, not a row from another run.

### H2 — future persistent forest save/load (not implemented)

P1-03 durable persistence is a **separate later feature requiring approval, technical C-1 format/root/replay/security contract, Lane B/E interfaces, source ownership and actual tests**. No H0 screenshot, JSON report, static trace or UI history implementation counts as a P1 save/load. It must not write `Tools/MultiRackSitkaLab`, Scenario One saves or Unity assets.

## 3. Ownership / data contract matrix

| Data or operation | Owner | History consumer may do | Must not do |
| --- | --- | --- | --- |
| P0 fixture/format and original hash | P0 lab; P1 reviewed original trust catalog | Show verified provenance for original read-only input | Accept P0 calendar history as ecology, alter P0, infer self-reported file digest is authentic |
| Derived 30 m fixture identity, schema, seed, area, coordinates | P1 `DerivedManifest/FixtureAccess`, reviewed catalog | Bind static report to accepted exact fixture and show disclosures | Resize original P0, modify accepted fixture, silently update trust |
| One-year authoritative state/report | #725 `AnnualEngine`/`AnnualRules` (local unintegrated) | Inspect complete detached immutable frames only | Run extra year or change mutable engine records |
| Shared tree/snapshot fields, units, API and view identity | **Lane E #740** | Propose stage-tagged history extension | Freeze shared API/assembly/file ownership unilaterally |
| Renderer, projection, scale, hit tests | **Lane A #736** | Feed immutable history frame and selected ID | Make screen coordinates or glyph radii biological authority |
| Marking, proposal/approval/execution UX | **Lane C #738** | Display historical recorded states with provenance | Implement marks/work or treat proposed felling as actual |
| Annual growth, mortality, years >1 and ordering | **Lane B #737** | Read committed year/stage/outcomes | Choose biological chronology or implement M-1 fix |
| Read-only compare/timeline/provenance view | **Lane D #739 proposal** | Select, filter, derive differences, label non-authoritative export | Create or edit events or saves |
| Event archive, idempotency, replay, secure file operations | Future separately authorised P1-03 writer/security reviewer | Specify requirements and failure cases | Implement file format/Host, own physical board or claim PASS |
| Integration, source import, L4 release, canonical choice | **Lane E #740, Manager #730** | Flag blockers and propose interfaces | Merge local #725, waive L4, change main |
| Scenario One S1-C/D history and Unity save | Separate Scenario One owners | Reuse only conceptual truthfulness (record vs observation vs comparison) | Copy Unity save versions/semantics or assert shared persistence |

**Provisional frame proposal for #740 review, NOT an ABI:**

- `HistoryFrameRef` = explicit `domainTag` (one of `DerivedP1ThirtyMetreExperimentV1`, `OriginalPolygonExploratoryV1` or `DerivedUnitySquareParityV1`), reviewed fixture/catalog identity/digest, initialization/experiment lineage, ecological year, **year stage**, shared view schema version, optional real history/state tip. A **report-only** Year 1 frame must say report-only and must not invent a persisted event tip.
- `TreeHistoryRow` = stable case-sensitive ordinal `treeId`; species/demand/stage; source X/Z **metres** with declared origin/axes; age years; DBH centimetres; height and crown radius metres; explicit `alive`, status (live/dead/felled), optional pre-growth CI and source stage; natural death cause/year or actual work provenance only when recorded. Pre-inventory dead in future originals has unknown cause/year, not a fabricated mortality event.
- `StandHistoryRow` = all-living N, N/ha, BA, BA/ha, Dq, RD, pressure if supported; named stage and numeric units/precision/bits; action-eligible subset as a **different** metric if available.
- `Provenance` = `Synthetic` source class, exact domain and fixture, source/method/model/RNG versions **separately**, seed, area f32 bits, excluded systems, trust/qualification class and disclosure.
- `HistoryComparison` = two validated frame references plus per-ID/stand deltas labelled **derived comparison**, not a biological event, measured site result or independent forecast.

Use unchanged raw numeric bits or exact round-trip encoding for authoritative models. Display rounding never feeds back to simulation, saves or outcome decisions. E decides exact common type/schema and immutable projection implementation; #739 does not own it.

## 4. History UI cases and errors (DRAFT acceptance)

| Case | Expected visible behaviour / testable invariant |
| --- | --- |
| U01 Two-year overview | Precisely Year 0 and Year 1 completed; clear *synthetic, adult-only Sitka, not Irish field forecast, not Unity numerical/geometry parity* label. |
| U02 Tree selection | Select `P30-001` once, show the **same ID** in both frames with before/after DBH, H, crown, age; never match by UI list index/pixel. |
| U03 Aggregate change | 100→100 living, ages 30→31, BA/Dq/RD from `2; change labelled **derived**. |
| U04 Below eligibility | Show all 12 initially sub-170mm stems in both frames, not bio-filtered by P0 work threshold. |
| U05 Cause honesty | “No natural deaths recorded in Year 1”; no invented fell marks, interventions, deadwood sale or self-thinning victims. |
| U06 Intermediate stage | `pre-growth` CI from beginning-of-year living set; `post-growth` within Year 1, not a second annual frame. |
| U07 Historic work (future) | Mark/proposal/approval is not an executed fell; only a committed executed-work event reduces living count. |
| U08 Read-only | All navigation/selection/filter/export leave source state, revision, event tip, plans and command ledger unchanged. |
| U09 Missing provenance | Show “not recorded / unavailable,” not an unsupported explanation of cause, exact Irish provenance or quality. |
| U10 Corrupt/mismatched report | Duplicate ID, invalid stage/domain/catalog, unknown version or incomplete report refuses view activation and preserves last valid view; no silent re-run or repair. |
| U11 Different lineage | Compare requires same trusted experiment lineage and compatible stage/units; otherwise explicit refusal. |
| U12 After later valid load | Invalidate historical projection caches/selections tied to former active lineage; do not change old history or display stale frame as current. |
| U13 Domain distinction | Original P0 read-only import and future 40/80 Unity oracle controls do not gain an annual 30 m history by geometry inference. |
| U14 Trace export | Explicit non-authoritative `trace` tag plus source tip/fixture/provenance; never accepted as loadable game state. |

**Verification status:** no UI, browser screenshots, history API, Host or file save was built/tested by #739. These are future requirements, not observed passes.

## 5. Future event and year-snapshot policy (DRAFT)

An event records an actual **committed** transition; a snapshot describes the stand at a named year/stage; a comparison derives differences. They are three distinct data classes.

**Candidate canonical event envelope, subject to P1 C-1 review:** event-schema version, contiguous sequence, domain and initialization identity, kind, year before/after, expected forest revision, command ID/digest where appropriate, previous-event digest, pre-state hash, typed canonical payload, post-state hash, own SHA-256 digest. The existing owner-approved P1 `spec.md` `5.2/`9` proposes separate `P1-STATE-V1` and `P1-HISTORY-V1` hash domains. State hashes must not include their own history tip; history envelopes refer to state hashes, avoiding cycles. This is **not** a committed archive schema or cryptographic signature: a party able to rewrite the whole chain can rehash it.

**Event families for review:** experimental genesis, mark/plan state transitions, actual executed work, one **compound** yearly advancement with ordered child mortality records, and plan staleness/audit-only rejection with explicit origin. Do not replay child deaths as second independent mutations. #725's JSON is a calculation report, **not** an implemented event chain.

**Invariant proposals:**
- Tree IDs are stable, ordinal and case-sensitive through death/felling. Dead records remain inspectable but **excluded from future CI/growth/live stand totals**: mandatory #729 M-1 repair and dead-tree vector precede Year 2.
- Felling by committed management differs from biological cause `self-thinning` with optional mechanism `hazard`/`boundary`. Pre-inventory dead retains source unknown cause/year. Never invent cause, year or money.
- Plans move through distinct draft/proposed/approved/pending/executed/terminal stages. A proposed mark is not a Fell status; only an **executed** work event causes a fell and changes the living set. Neither history inspection nor annual advancement auto-executes an old approval.
- One completed closed-population annual transition must reconcile `N_end = N_start - work_fells - natural_deaths`. Basal area reconciles by subtracting executed-work BA at execution, adding post-work living BA growth, then subtracting post-growth biological-death BA. One death per tree; future dead trees never regrow.
- The future dedup design in existing P1 `9.4` distinguishes recorded rejection (may consume command ID) from structural refusal (does not); identical committed/rejected ID and digest retries must not change state/events, different payload with reused ID refuses. **These are P1 C-1 proposals, not current #725 CLI functionality.**

**Static H0 frame identity** is fixture/report/year/stage; **future H2** adds verified session/transaction/history tip. Never fabricate a history hash for H0 or compare raw hashes between two runs merely because their ecological state looks similar.

## 6. Persistent archive alternatives (not selected)

| Option | Content and benefits | Cost/risk | Decision |
| --- | --- | --- | --- |
| **A. Full annual per-tree archive** | Canonical complete before/after data for every tree-year; fast historical inspection. | Storage grows with number of trees × years, additional validation/migration surface. | Alternative, not selected. |
| **B. Compact genesis/current snapshot + ordered events + replay** | Year states reconstructed deterministically from pinned source and checked chain. Per-ID trace is a non-authoritative export. Aligns with existing P1 `9.3` design proposal. | Replay and repeated full-state hash work may become costly; model/platform drift can invalidate replay. | **Preferred for measured P1-03 review**, not author-approved format. |
| **C. Compact plus checkpoints** | Potential faster navigation and bounded replay. | Changes history trust/hash/format, checkpoint identity, capacity and split-run rules. | Explicit future alternative; not included in B or approved. |

No numeric cap is selected by #739. Existing P1 `9.3` labels 5,000 trees, 200 years, 64 MiB, 200,000 events and trace limits as **unmeasured design candidates**; its example byte envelopes are not a fixed grammar or guaranteed smallest legal event. Measure **both** annual model replay and full-state serialization/hash per event, including a maximum number of small audit events, peak memory, load time and failure under caps. Require Manager-approved replay-time budget and static preflight `ReplayBudgetExceeded` rather than uncontrolled blocking or truncation. Cache/per-year projections are derived and invalidated when lineage/tip changes, not authoritative state.

### Owner-confirmed successful-load semantics (not reopened)

After a **fully validated successful load**, create a fresh session epoch. For each *open Approved or PendingWork* plan, append **exactly one** `PlanStale(SessionLoaded)` in ordinal plan-ID order. If none exists, append **zero load events**. Already terminal plans stay terminal; no old plan may execute on the new session epoch. Staleness is a committed plan/history change but not a biological year. A future unconditional `SessionLoaded` audit or persistent approvals would require a **new owner decision**, not a silent technical optimisation.

**Split-run Mode A:** no open Approved/PendingWork plan at split; same legal actions must yield equivalent per-ID biology, year, model/RNG/seed and actual work/death outcomes. Under accepted conditional policy, load appends zero events. Raw canonical-history hashes equal **only if complete event streams including IDs/epochs/ordering are identical**, not just because biology matches.

**Split-run Mode B:** one or more open Approved/PendingWork plans at split; loaded branch has one stale event per plan. Compare biological and **legally equivalent** work outcomes, not raw chain hashes with an uninterrupted branch whose plan stales at a different time. Do not compare an uninterrupted branch that legally executes a plan with a loaded one that must refuse it as if they made the same decisions. No ad hoc “normalised” event hash by deleting load events.

## 7. Restore, checksums, failures and compatibility (DRAFT)

**Proposed fail-closed validation order**, to be reconciled with existing P1 `9.9` technical candidates before any source work:

1. Verify supported filesystem/OS capabilities and **P1-owned distinct file root**, safe descriptor-relative no-follow path and owner/mode controls. Reject symlink/hardlink/FIFO/device, traversal, caller-chosen path, renamed directory or unsupported platform. A filename suffix is not a trust anchor.
2. Strict canonical parse, size/cap/schema/version checks: malformed/truncated JSON, duplicate/unknown keys, bad types/float encodings, missing fields, duplicate IDs, invalid geometry/status/year and nonfinite values **refuse without live mutation**.
3. Verify **independently reviewed trust catalog** and exact source fixture file SHA-256, P0 snapshot hash where applicable, format, domain, fixture ID, original vs derived identity, geometry/origin/axes/area float bits, growth and RNG versions separately, seed, algorithm/source pins and event-history integrity. Do not accept self-declared hashes as independent trust, resize P0 or silently accept unknown model/version.
4. Check proposed full canonical history: contiguous event order, previous digests, state before/after, exact committed work, unique command IDs, deterministic mortality and no duplicate death/command. Recompute from **trusted genesis** into a **detached** forest, compare current snapshot and final history tip; no internal repair/defaulting.
5. Apply only a separately accepted platform-compatibility rule. P1's `Math.Exp`/`Math.Pow` may vary by runtime/library; existing P1 `9.9` provides conservative fingerprint A, narrower A′, diagnostic B and deterministic-math C as **unselected alternatives**. Platform fields are compatibility indicators, **not signatures**. Distinguish mismatched platform from replay mismatch; never claim Linux/WSL2/Mono/Windows bit identity without evidence.
6. Atomic activation only after all validation. A detached replay job must capture session generation/tip/revision and recheck under exclusive commit gate, including history-only rejection changes. Second load is refused or explicitly cancelled. Any error, timeout or competing command leaves active forest/plans/history/save unchanged and writes no replacement.
7. Save failures must be reported truthfully: pre-rename failure means old final bytes preserved and only own temp cleaned; rename succeeded but directory `fsync` failed means **new bytes may be installed; crash durability unknown** and no old-file rollback promise. Successful save followed by list refresh failure is **not** a failed save. Lost HTTP response means client outcome unknown; use idempotent request-ID/readback rather than applying a second annual command. Precise operation names and filesystem policy require independent security review.

**Proposed refusal classes, not current APIs:** `StructuralInvalid`, `UnsupportedVersion`, `SourceOrGenesisIncompatible`, `FingerprintMissingOrInvalid`, `PlatformFingerprintMismatch`, `ReplayMismatch`, `ReplayBudgetExceeded`, `LoadInProgress`, `SessionChanged`, `ReplayAborted` and `SaveCommittedDurabilityUnknown`. Do not claim a platform mismatch proves tampering; do not claim raw history hashes can be compared after different load staleness events. A checksum verifies equality with a trusted value, not authenticity of Irish measurements.

### Compatibility / migration table

| Input | History H0 | Future P1 save/load H2 | Rule |
| --- | --- | --- | --- |
| Accepted derived 30 m Year 0 and #725 Year 1 report | **Two-frame view eligible from frozen trusted report** | Report is **not** a save; cannot load as durable forest | Exact report/fixture binding; no invented event chain |
| Original P0 independently verified synthetic step-0 inventory | **Original-only static inspect**, no ecological Year 1 | Explicit verified read-only fork only, later true original polygon year engine | Preserve source P0 bytes, IDs, original geometry, no prior marks/history |
| P0 calendar-stepped/post-plan/post-intervention/felled snapshot | No invented P1 annual history | **Refuse initial P1 original-only fork** | New owner approval and conversion spec required to broaden |
| `DerivedUnitySquareParityV1` 40/80 legal reference | Read-only reference/oracle context only | No implied annual/save, separate trusted catalog, C-2b after genuine source capture and C-6 GO | Never infer 30 m Unity parity or import |
| Future P1 current-version trusted save | Not currently present | Detached replay, integrity check, D-082 plan staleness, atomic activation | Requires approved P1-03 |
| Unknown/older P1 archive or altered model/bit policy | Refuse/unavailable | **Refuse unchanged**, no silent defaults or upgrade | Versioned reviewed migration only |
| Scenario One/Legacy40/Enlarged80/Reference Future/Unity save | Outside lab | **Never auto-import, rewrite or downgrade** | Separate product and source authority |

A future migration requires approved old/new schema versions, source-byte preservation, explicit IDs/causes/year/model/seed/epoch mapping, independent round-trip/split-run evidence, changed-file allowlist and rollback. **None is specified or approved now.**

## 8. Proposed four-file SPEC REQUIRED successor (not created)

Proposed subordinate root `specs/multirack-sitka-p1-history-persistence-v1/`. Manager may instead amend the existing owner-approved four-file P1 package, but **must not silently supersede it**. This handoff does not write or approve either version.

| File | Required scope |
| --- | --- |
| `spec.md` | H0/H1/H2 goals and explicit phase gates; user-visible history, immutable frame/ID/stage/provenance, D-082 load rule, compatibility and failure policy, numbered acceptance. |
| `plan.md` | D0 static two-frame projector/view; D1 #740 shared ABI/renderer alignment; D2 #737 year>1 plus M-1; D3 independently approved P1-03 persistence/root/replay/security; D4 qualified history after real save/load. Native single-writer/review, L4 and rollback gates. |
| `tasks.md` | Bounded one-writer packets for immutable read projection + UI, negative fixture tests, Year-2 living-filter fix, event/command schema, compact archive and replay benchmark, file/durability fault tests, Mode A/B save/load, cross-platform gate and integrated history qualification. |
| `checklists/requirements.md` | Checked only against future actual evidence: trusted report and 100-tree oracle, ID continuity, all-living counts, view read-only and disclosure, stage, no invented events, future death/work, replay identity/epoch, atomic load, security, durability, migration/refusals. |

**Suggested acceptance matrix — ALL PROPOSED / NOT RUN BY #739:**

| ID | Requirement / oracle | Gate |
| --- | --- | --- |
| H01 | Exactly two annual frames in H0; 100→100 living, 30→31 ages, anchored BA/Dq/RD, zero deaths; 12 below eligibility still biological | D0 |
| H02 | All 100 ordinal IDs stable; per-ID before/after values align, distinct within-year stages and units | D0 |
| H03 | Read-only navigation, filtering, selection and export do not call annual engine or mutate revision/event/plan | D0 |
| H04 | Invalid report/domain/digest/version/duplicate-ID refuses; prior visible valid view preserved | D0 |
| H05 | Label synthetic closed adult Sitka, source-derived, **NOT** Irish field forecast or Unity parity on view and trace | D0 |
| H06 | P0 and 40/80 modes do not manufacture 30 m annual ecology; no protected P0/fixture modifications | D0/D1 |
| H07 | Year 2 input with a dead stem excludes it from CI/growth/live density; death/fell history only once | D2 after M-1 |
| H08 | Per-year work and natural mortality distinct; closed-population N/BA reconciliation, no planned-only tree loss | D2/D3 |
| P01 | Known trusted schema/source/fixture/model/numeric/area/seed replay exact; changed bytes/versions refuse before mutation | D3 |
| P02 | Mode A split-run biological/work equivalence; raw history only when complete canonical streams/epochs same | D3 |
| P03 | Mode B successful load one staleness per open Approved/PendingWork plan, **zero if none**, stale plans cannot execute | D3 |
| P04 | Command ID retry idempotent; same ID/different digest rejected, duplicate events/deaths impossible | D3 |
| P05 | Replay corruption, over-budget, concurrent load, cancellation or session race preserve prior active forest/save | D3 |
| P06 | Pre-rename failure vs post-rename dir-sync failure truthfully distinguished with actual filesystem fault evidence | D3 |
| P07 | Symlink/hardlink/FIFO/traversal/host security controls and full P0 isolation independently tested | D3 |
| P08 | Measured replay/cap limits and platform choice accepted; no guessed 200-year/64MiB guarantee | D3 |
| P09 | Loading different accepted lineage invalidates read caches and preserves historical frames read-only | D4 |

**Cost-aware verification:** D0 needs focused read-only report and UI negative checks, not a full default .NET L4 suite or Unity. D3 needs appropriate replay/file/security and regression qualification. A document with unchecked tests is **not** verification. Release still blocked by unwaived L4 FAIL38.

## 9. Cross-lane requests and Manager decisions

| Counterpart | Proposed interface request, not granted to #739 |
| --- | --- |
| **E #740** | Own immutable shared snapshot contract, frame/stage identity, numeric bits/units, source X/Z vs viewport mapping, report-only and durable-lineage distinction; review H0 projectability and integration/security gates. |
| **A #736** | Consume E-owned immutable frames by IDs; expose Year 0/1 playhead, comparison and conspicuous provenance; drawing/selecting never mutate biology. |
| **B #737** | Return committed year-end state plus explicit intermediate stages; fix and prove #729 M-1 before Year 2, specify future regenerated historical CI/death detail and annual sequence. |
| **C #738** | Separate historical *recorded* marking/plan approval from *executed* work; pending/proposed never become Fell status; UI history never submits a command. |
| **Manager #730** | Review this DRAFT independently, reconcile P1 approved spec and E interface, decide H0-only packet versus full history scope, archive/caps/portability/Host policy, physical source ownership and eventual independent L4/security solution. |

**Open decisions requiring subsequent explicit approval:** shared ABI and UI host; trustworthy import of static qualified report without protected local-source reads; P1-03 archive full/compact and cap/replay budget; event/command grammar and `fsync`/Host/file security; platform A/A′/B/C compatibility; versioned migration policy; any C-2b parity tolerance (only after legal Unity evidence) and C-6 owner Unity GO. **D-082's conditional stale-on-load policy is already accepted and is NOT open.**

**STOP if:** someone requests changing an E-owned shared API, #725/#710 source or board without native custody; asks to replay Year 2 before B's approved M-1 repair; attempts to turn a report into a save; silently migrates P0/Unity data; labels this lab a field forecast; or attempts to waive L4. Escalate to #730/#740, no speculative source fix.

## 10. Handoff status

- **Created:** one **DRAFT documentation/specification** result, no canonical changes and no four-file spec artifacts.
- **Evidence assessed:** the complete Manager #739 packet, live GitHub coordination and issue, approved P1/P0 specs, main decision authority, and #723/#725/#729/#732/#735 with #740's provisional interface. Protected physical source/evidence were **not** accessed; #739 did not independently rerun native tests.
- **Actual tests/builds/Unity/Host/save work by this worker:** **NONE**. #732's qualified Year 1 scope remains separately evidenced; no persistence validation; L4 still FAIL38.
- **Recommendation for independent review:** H0 truthful read-only static Year 0↔Year 1 first, then consider compact authoritative archive + replay under a later approved P1-03 gate, with no premature schema or portability promise.
- **Next:** Manager #730 reviews this with the separate A/B/C/E handoffs, returns APPROVED / NEEDS CHANGES / REJECTED on this *draft quality*, obtains decisions and releases only properly approved future packets. A Manager draft-quality verdict is **not** a self-authorised production start.
- **User action:** none for returning this draft to Manager; separate owner choices only when actually presented.
- **Dashboard/Drive sync:** not verified. **No main merge, Unity, Windows, integration or L4 waiver.**
