# Multi #747 — independent five-lane technical specification review

18 November 2026. **Verdict: NEEDS CHANGES — INDEPENDENT SPEC REVIEW ONLY.**
**Findings: 5 total — BLOCKING 0 / MAJOR 3 / MINOR 2.**

The proposed architecture is suitable for targeted consolidation. The shared contract still permits incompatible implementations of missing diagnostics, candidate eligibility and comparison identity. Correct F1–F3 in the consolidated specification before approving the affected interfaces. F4–F5 are small corrections in that same pass. This does not reject the five-lane direction, approve any product DRAFT, or authorise implementation.

## 1. Reviewer, authority and actual work

- **Actual completing reviewer:** OpenAI `gpt-6.1-sol`, xhigh reasoning, Codex harness in T3 Code; identity/settings supplied by the current runtime. GitHub connector authenticated as jermur788. A separate nonauthor session/model from the five authors recorded as GPT-6 / ChatGPT Game Dev. Same-provider overlap is disclosed; cross-provider independence is **not** claimed. No subagents or delegated reviewers.
- The preceding Claude session registered but stopped at a rate limit without publishing a result. The [current RUNNING registration](https://github.com/jermur788/CCF/issues/747#issuecomment-6981149072) supersedes its completing-worker identity; its historical registration is preserved.
- **Fresh main baseline:** `e8e84027faa45d42316cd01bda34ad36393fd069`. The previously created permitted branch `docs/multi-747-independent-cross-spec-review-20261118-v1` was checked live and remained exactly at this main SHA, with no result commit. It was retained; no alternate branch or local worktree was created.
- **Authority:** [complete #747 packet](https://github.com/jermur788/CCF/blob/056178dfa0b9793c8c27d900872a6807ea11d59e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-747-FIVE-LANE-INDEPENDENT-CROSS-SPEC-AND-INTERFACE-REVIEW-PACKET-20261118.md) and direct user dispatch; one independent documentary review and this sole new Markdown. No authority to edit A–E, canonical decisions/specs, source, physical coordination or protected stores.
- **Inspection:** complete A–E drafts, complete required approval/implementation/review/qualification/policy reports below, Manager reviews, main D-082/D-084, current milestone and live GitHub coordination. Relevant approved P1 spec §3 and §5.2 were additionally checked. No direct #725 source inspection or recomputation of its numerical witness.
- Preliminary local entry-document/status reads in the coordination anchor found unrelated modified `AGENTS.md`, `Docs/Operations/StorageArchitecture.md`, and three untracked storage-operation documents. Preserved; no local edits. Subsequent review/publication used GitHub documentation. No code, tests, build, native source/worktree access, Host, Unity, Windows, integration or L4 waiver.

The live main coordination file still contains older Multi status paragraphs and no #747 entry. It is not a fresh physical custody measurement. This review uses the explicit #747 documentary assignment and live issue registration; it claims no source/physical lock. Manager must reconcile operational publication; this worker does not acquire or edit such a lock.

## 2. Exact sources and evidence boundary

Every A–E file was read in full. These immutable links pin the reviewed revisions, rather than mutable branch tips.

| Draft | Reviewed commit | File |
| --- | --- | --- |
| A #736 | `b8c68af8886673110f027db59877d57279789aa3` | [Complete A draft](https://github.com/jermur788/CCF/blob/b8c68af8886673110f027db59877d57279789aa3/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-736-READONLY-2D-WOODLAND-VIEWER-SPEC-DRAFT-RESULT-20261118.md) |
| B #737 | `da37b23bf97553e114ff7d0bab855612abcd9b9a` | [Complete B draft](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md) |
| C #738 | `911535e75548cc70d94b20e4089e6e6ab6fc8837` | [Complete C draft](https://github.com/jermur788/CCF/blob/911535e75548cc70d94b20e4089e6e6ab6fc8837/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-RESULT-20261118.md) |
| D #739 | `c21d8edaf4583c95e372cc3fdb6e1fdf718d5616` | [Complete D draft](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md) |
| E #740 | `671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c` | [Complete E draft](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md) |

Other complete mandatory reports read: [owner four-file approval](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md); [#725 implementation](https://github.com/jermur788/CCF/blob/6e51dcb6c86eab78b99b4b05e08c2c44e0ffdfa9/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-EXACT-R-ADULT-SITKA-YEAR1-IMPLEMENTATION-RESULT-20261118.md); [#729 independent source/numerical review](https://github.com/jermur788/CCF/blob/8310a38086425a5b223315c8f83181dd77eee1eb/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-729-INDEPENDENT-EXACT-R-YEAR1-FIVE-FILE-SOURCE-AND-NUMERICAL-REVIEW-20261118.md); [#732 original STOP and offline PASS resume](https://github.com/jermur788/CCF/blob/88e82cd0cc3e60dcc777720332200a88231d4b8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-732-EXACT-R-YEAR1-FOCUSED-INDEPENDENT-NATIVE-QUALIFICATION-RESULT-20261118.md); [#694 limited-development policy](https://github.com/jermur788/CCF/blob/916531789d6d6273ca134f951b18ba992b800063/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-694-MANAGER-723-STAGEA-ACCEPTANCE-AND-LIMITED-DEVELOPMENT-POLICY-20261118.md); [Manager's four-draft review](https://github.com/jermur788/CCF/blob/e7d7e19e4d819e79177edffa3535eee972a1fb8e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-MANAGER-FOUR-DRAFT-DOCUMENT-QUALITY-AND-CROSS-LANE-REVIEW-20261118.md). Also read [C final Manager acceptance](https://github.com/jermur788/CCF/issues/738#issuecomment-6980775909), [E revised Manager acceptance](https://github.com/jermur788/CCF/issues/740#issuecomment-6980960264), and [#735 actual custody result](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md).

Canonical context basis is [main at the recorded baseline](https://github.com/jermur788/CCF/tree/e8e84027faa45d42316cd01bda34ad36393fd069/Docs/Project), including Decision Log D-082 and D-084, Current Milestone, Game Brief and AgentWorkflow. This is the observed main snapshot, not an invented new context-commit declaration. Main D-084 is **Scenario One Compact30**. The separate Multi owner approval is cited by its dated receipt; its unmerged “D-084” does not replace main's meaning. Main D-082's historical DRAFT status is not evidence that the later, separately approved P1 four-file package lacks its recorded owner approval.

**Evidence reused, not rerun:** #725 is local commit `df467dcc302eca564343e6da40f2fb88529a5d16`, tree `4237ece5c4efcd167857c1cb29e16bbcc2279ad6`, parent R `35d26f5e64a00a400c700871927ccc2173370568`. #729 reviewed the five changed files and independently matched 700 per-tree values and 100 RNG numerators. #732's resumed focused offline qualification matched the exact CLI output and original/tamper refusals; it did **not** rerun the 43/98 suites. Its earlier unexpected NuGet/SDK network contact remains part of the report. Nothing in this review upgrades author receipts to new reviewer execution.

Protected fixture: 32,567 bytes, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`. Protected old `annual` CLI stdout: 45,980 bytes, SHA-256 `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`. That second hash applies **only to the existing CLI bytes**, not the proposed read schema, a state/history digest or a save.

## 3. Findings and precise corrections

### F1 — MAJOR: diagnostics and stage fields do not yet admit the promised static Year0 view without invention

**Locations:** [E §3.2, CI/neighbour rows, lines 78–79](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md#L78-L79), [E §3.3, stage/numeric/diagnostic rows, lines 92–104](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md#L92-L104); consumers [A §3.4 and §4](https://github.com/jermur788/CCF/blob/b8c68af8886673110f027db59877d57279789aa3/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-736-READONLY-2D-WOODLAND-VIEWER-SPEC-DRAFT-RESULT-20261118.md#L144-L169), [D §2 H0](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md#L30-L55).

E types per-tree pre-growth CI as f32 and neighbour count as a nonnegative integer without an unavailable/not-evaluated case. By contrast, A allows a Year0-only fixture view with unavailable CI/stand metrics, and B requires dead entries not to be evaluated in later years. E §6 allows absent audit fields, but the field table does not specify how that is encoded or validated. Encoding unknown as zero invents a biological observation; rejecting it forces unnecessary annual calculation before a static viewer. E also leaves stems/ha as “source f32 or declared numeric kind”, although #729 establishes the existing count/float calculation as f32.

**Correction — E owns, A/B/D confirm:** define a field-availability table for Genesis, annual Before, PostGrowth and AfterMortality. Required tree inventory values stay required. Unevaluated diagnostics need a distinct explicit absence/reason; zero remains a valid computed value. Year0-only fixture metrics may be unavailable. For each observed numeric field freeze its kind and units: source f32 DBH/height/crown/CI/area and stems/ha, f64 BA/Dq/RD reductions, integer counts and RNG numerator. Empty-stand null/not-applicable policy is added only when B's separate decision is approved.

Use one complete row roster and its matching aggregate per observation stage. Other annual audit aggregates, if bundled, must be separately stage-keyed and must not be presented as metrics computed from those rows. Distinguish the start-state observation year from the target annual-step year; E's “Before=Year0 / AfterMortality=Year1” is the first witness, not a permanent definition for all future steps. Intermediate PostGrowth observations may be exposed from a **completed** transaction only.

**Targeted future probe:** Year0 fixture alone opens with all 100 trees and unavailable diagnostics; zero-CI singleton is distinguishable from unevaluated dead/unknown CI; stage/aggregate mismatches refuse. No annual engine call is needed to make this probe pass. **Gates:** shared H0 contract now; dead/empty cases only before B integration.

### F2 — MAJOR: E's eligibility hint cannot satisfy C's exact-source eligibility requirement

**Locations:** [E §3.2 actionableHint, line 82](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md#L82), [C §4.3 atomic batch, line 141](https://github.com/jermur788/CCF/blob/911535e75548cc70d94b20e4089e6e6ab6fc8837/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-RESULT-20261118.md#L141), [C §5 ReadCurrentSnapshot and future submission, lines 149–158](https://github.com/jermur788/CCF/blob/911535e75548cc70d94b20e4089e6e6ab6fc8837/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-RESULT-20261118.md#L149-L158), and C C-13 / OPEN-C1.

C correctly refuses marking when eligibility is unknown and rejects an unsupported hint. E offers only an optional boolean plus reason. It does not bind that hint to a rule/version, exact imported threshold representation or evaluation snapshot/stage. Exact DBH bits alone cannot tell C which approved action rule was used. A consumer could either disable all candidate marking or independently hardcode “170 mm”, creating the duplicate authority the drafts prohibit.

**Correction — E/C with Manager acceptance:** choose one reviewed eligibility projection producer in the shared adapter. Provide either explicit Unknown or a rule/version + exact threshold/unit/comparison policy + evaluated full observation key + per-ID eligible/ineligible/reason result. Compare exact current DBH under the imported rule without display or annual integer-mm rounding. Keep biological membership separate. Report-only eligibility means suitability for an ephemeral sketch of that observation, **never current execution permission**; actual execution must revalidate the entire set at the then-current command authority.

**Targeted future probe:** exact below/at/above-threshold values with identical rounded labels; dead tree; missing/unsupported rule; an old evaluation key with new DBH; a mixed valid/invalid batch. All-or-nothing C changes only local annotations. **Gate:** candidate marking, not an inspection-only Year0/H0 viewer. Unknown eligibility must not prevent inspection.

### F3 — MAJOR: E's comparison guard is weaker than D's lineage fence; callback revision scope also needs one definition

**Locations:** [E §4 ReadComparison, line 116](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md#L116), [E §3.1 identity fields, lines 49–62](https://github.com/jermur788/CCF/blob/671a77ca3ce913ecc2ffc362cb7f6c328e9bbb2c/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-740-SHARED-SNAPSHOT-AND-INTEGRATION-READINESS-SPEC-DRAFT-RESULT-20261118.md#L49-L62), [D §4 U11–U12, lines 105–106](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md#L105-L106), [A §4.1 selection callback](https://github.com/jermur788/CCF/blob/b8c68af8886673110f027db59877d57279789aa3/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-736-READONLY-2D-WOODLAND-VIEWER-SPEC-DRAFT-RESULT-20261118.md#L171-L189), [C §4.3 and §5](https://github.com/jermur788/CCF/blob/911535e75548cc70d94b20e4089e6e6ab6fc8837/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-RESULT-20261118.md#L123-L158).

E's explicit logical comparison contract checks matching domain and fixture identity. D requires the **same trusted experiment lineage**. Two runs can share a fixture while differing in seed, models or later work history. E's signature as written is insufficient to enforce D's refusal. “snapshotRevision” in A's callback is also not explicitly the globally scoped snapshot identity or a local counter; a counter reused after a restart is insufficient for C's stale-event rejection.

**Correction — E owns:** define one opaque, immutable observation key resolving the complete binding: schema, fixture/domain, source bundle/experiment lineage, generation, model/numeric policy and seed, observed year/stage and committed revision. Give H0 a trusted fixture/report-bundle binding and report-byte digest/provenance, explicitly `ReportOnly`; no fabricated event/state tip or active-session requirement. A selection notification, C draft/overlay and D frame/detail all carry that same key. A may keep its existing callback shape if the revision argument is specified as this full key.

Historical comparison must validate common lineage/model/seed/geometry/numeric compatibility and explicitly permitted stage pairs; a generation change invalidates active UI subscriptions/drafts. Any deliberate cross-experiment comparison is a separately labelled future feature, not a relaxation of H0. For a static viewer, inspecting two frames from the same verified report bundle does not require a live engine or durable session.

**Targeted future probe:** same fixture but different seed/model/lineage refuses H0 comparison; stale callback with the same small revision number after a new generation is ignored; valid Year0/Year1 pair from one accepted bundle compares; C freezes on a key change. **Gate:** shared multi-frame/selection interface; keep the first single-frame viewer minimal.

### F4 — MINOR: distinguish chronological tree age from ecological year of death

**Location:** [B S2 dead-stem policy, line 58](https://github.com/jermur788/CCF/blob/da37b23bf97553e114ff7d0bab855612abcd9b9a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-737-MULTIYEAR-ANNUAL-ENGINE-SPEC-DRAFT-RESULT-20261118.md#L58), read with S4 phase E and B-R05.

The text says a tree dying in Year `k` has final post-growth size and “age `k`”. Taken literally, that would replace the original age-30 cohort's biological age with the experiment year. S4's one-age increment makes the intended behaviour recoverable, so this is a local wording defect rather than evidence the whole algorithm is wrong.

**Correction — B/Manager:** say “final post-growth age at death” and store it separately from `deathYear=k`. For this cohort, a controlled death during ecological Year2 has age 32 and deathYear 2; subsequent years retain both under the proposed freeze policy. Do not assert an actual Year2 victim or numeric future outcome. Confirm freeze policy before Year2 implementation.

### F5 — MINOR: repair already-identified citations and H0 frame wording during consolidation

**Locations:** [D §4 U03, line 97](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md#L97), [D §8 H01, line 190](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md#L190), [D U01, line 95](https://github.com/jermur788/CCF/blob/c21d8edaf4583c95e372cc3fdb6e1fdf718d5616/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-739-HISTORY-AND-PERSISTENCE-DESIGN-SPEC-DRAFT-RESULT-20261118.md#L95), [C §1, line 11](https://github.com/jermur788/CCF/blob/911535e75548cc70d94b20e4089e6e6ab6fc8837/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-738-TREE-MARKING-INTERACTION-SPEC-DRAFT-RESULT-20261118.md#L11).

Replace D's malformed section-2 pointer with a reference to its §2 compiled aggregate table. Replace “two annual frames” / “Year0 and Year1 completed” with **“Year0 initial observation and Year1 completed observation”**; optional PostGrowth remains inside Year1. Close C's malformed §5.2 citation backticks, including the evidence-class sentence. Manager already identified the main editorial defects. Correct together; no separate worker/review cycle.

## 4. Five-lane compatibility and minimum ownership

Matrix entries assess design compatibility, not product approval. “Aligned” means no additional conflict found in that pair's stated boundary; findings name the remaining consolidation work.

| Lane | A viewer | B annual engine | C marking | D history/save | E shared contract |
| --- | --- | --- | --- | --- | --- |
| **A** | Sole projection/hit-test/primary inspection | F1; read committed output only | Aligned checklist fallback; F3 key | F1/F3 observation binding | F1/F3 |
| **B** | — | Correct proposed sequence; M-1 mandatory | Aligned no mark→biology; F2 eligibility | Aligned committed history; F4 age | F1/F3, pending B policies |
| **C** | — | — | Local reversible candidates only | Aligned no local-draft events; F3 key | F2/F3 |
| **D** | — | — | — | H0/H1/H2 separated; F5 cleanup | F1/F3 |
| **E** | — | — | — | — | G0–G4 correction sound; F1–F3 |

**Single writers required in a future packet:** one shared adapter/bit codec writer; B owns biological transitions; A owns renderer, camera, hit-test and primary inspection; C owns batch checklist, local marks and undo; D owns history presentation and later separately gated archive design. A/E define one read-only annotation input for C, with A's file writer implementing A's side. C must not independently modify A's hit-test or UI file. If a shell is shared, assign its writer explicitly before coding. No source files or physical writers are allocated by this review.

## 5. Minimum corrected contract and H0 path

Recommendations below are proposed consolidation content, not accepted APIs.

1. One complete immutable observation: header/key, all 100 ordinal tree IDs, unchanged positions, sourced age/alive/DBH/height/crown, and only actually available stage-matched metrics/diagnostics. No mutable engine collection and no partial 99-tree stand masquerading as complete.
2. For this fixture only: raw integer XY millimetres, south-west origin, +X east/+Y north; declared `p1-square30-corner-to-centred-xz-v1`: signed int64 subtract 15000, binary64 divide 1000, one f32 cast for X/Z. If both representations exist, validate **forward-computed f32 bits**. Inverting f32 X/Z does not generally recover exact integer millimetres; never demand that reverse equality or round-trip pixels into biology. A's X/Z +15 presentation is a view transform, not new source precision.
3. Use area bits `3db851ec` for reductions. Nominal 900 m² and mathematical 0.09 do not replace that f32 divisor. DBH/crown/area bits survive projection; display rounding never becomes eligibility or simulation input. BA/Dq/RD come from source reports; A does not sum crown circles or filtered visible trees to invent stand metrics.
4. Static H0 has no command port, annual advance, event append, durable save or Host. Missing Year1 remains disabled. Exact imported Year1 may be projected losslessly from the already qualified report after a separate lawful export/admission decision; do not run #725 or reconstruct it from #723's rounded table.
5. A sends only an inspection ID plus the shared observation key. C uses its accepted-as-provisional accessible ordered-ID checklist for batches; direct on-map modifier multiselect can wait. C's separate immutable overlays carry the base key and local edit revision. No mark means work, approval, execution or death.
6. H0 starts with **Year0 alone** after host/transport and contract approval. Add a second verified Year1 frame when safely supplied. All 100 Year0 IDs, including the 12 below threshold, remain inspectable. Keep disclosures and health-unavailable text, correct crown **radius** scaling, keyboard inventory, Fit/pan/zoom and stable selection. D then adds truthful two-observation comparison; no B ten-year runner, C marks or H2 save is a prerequisite.

For H0's first host choice, recommend an offline static reader with a specifically approved bounded delivery mechanism (for example packaged exact data). This does not assume browser file:// fetch, arbitrary user JSON, a development server or a new dependency is allowed. Owner/Manager must choose the actual delivery/packaging. If that choice becomes a loopback Host or webview, apply its own gates; do not hide it inside “static”.

## 6. B Year2 technical assessment and gating

B S3–S5 correctly specify the necessary source-derived order: living-at-start targets **and neighbours** → pre-growth CI/suppression → one age/DBH/height update → crown relaxation from post-growth DBH and cached CI → post-growth RD/pressure → conditional hazard → survivors-only density boundary → single atomic publish. Preserve fixed seed, target year, full ordinal ID and RNG model; p=0 does not draw. Simultaneous hazard selection cannot recalculate pressure/mean after each victim. Boundary uses pre-growth suppression then ordinal ID, strictly RD>1 and count>1; no forced last-survivor extinction. Zero living and a singleton are distinct.

No new biological formula defect was established from these drafts. B's ten no-work years are an acceptance horizon, not game lifespan, ecological calibration or Irish forecasting. Stable dead rows must not become targets, neighbours, growing/ageing trees, BA, hazard candidates or duplicate deaths. Prior deaths affect **next** year's CI, not a second release-growth pass in the same year.

**Before Year2 code/enablement:** approve dead-age/final-state and empty-stand policy, settle public opt-in command/revision semantics, obtain separate source custody/allowlist, then fix and independently review M-1. Prove unchanged old Year1 CLI bytes, a controlled death-present Year1→Year2 transition, dead-neighbour omission equivalence, no repeated death, singleton/empty behaviour, exact source chronology and transactional rollback/retry. Continue full internal state precision; never feed displayed JSON back into B. Concurrent requests need one publish, and an injected precommit failure must preserve state/events/view revision.

#729 M-2 synchronised concurrency and M-3 uncapped hazard/RD=1 remain nonblocking notes for the old one-year feature. They are useful targeted probes when extending the corresponding branches; no retrospective demand to rerun the old broad suites. Future-year numeric totals must come from an independently checked execution/oracle, not this document.

## 7. Owner/Manager decision register — NOW versus LATER

“NOW” means before approving/dispatching the affected slice, not a demand for the user to answer during this documentary review.

| When / affected slice | Decision | Recommended disposition / owner |
| --- | --- | --- |
| NOW — H0 | Offline static delivery vs loopback Host vs desktop/webview | Static Year0 first; actual bounded data delivery/packaging chosen by owner + #730. Host/new dependency is not implicitly approved. |
| NOW — shared contract | F1/F3 observation key, stage availability, numeric kinds, report-only provenance; one adapter/codec and shared UI writer | #730/E consolidate with A/B/D. No need to invent durable history hashing for H0. |
| NOW — C only | Exact eligibility projection, REMOVE_CANDIDATE-only vs broader vocabulary, hard-disable vs warning-only sketch, stale freeze, bounded undo | #730/C/E obtain applicable acceptance. Recommend one removal-candidate type, hard-disable unknown/ineligible, atomic batches, checklist and no automatic rebase. H0 inspection can precede this. |
| NOW — B only | Empty continuation vs terminal state, freeze-at-death age/final sizes, opt-in CLI/guard names and successor semantics | #730/B with owner for material workflow choice. Recommend retaining dead final state; settle empty semantics explicitly. Preserve old annual command/refusals. |
| NOW — any future source import | Lawful export or admitted source lineage, exact isolated file set and physical writer | Native custodian + #730, separately granted. No reading protected #725 just because this review is complete. |
| NOW — canonical promotion | Multi decision-ID crosswalk | #730/authorised canonical editor with owner authority: allocate unused ID, retain main Scenario One D-084 and link historical Multi receipt. No renumbering by reviewer. |
| LATER — real work | Authoritative plan/command lifecycle, current whole-set execution validation, idempotency, natural-death vs Fell records | Separate approved management implementation/security scope. Local C marks do not supply it. |
| LATER — H2 | Archive/full vs compact+replay/checkpoints, measured limits/time budget, platform policy, root/format/Host/durability/migration | D/E/security owner under separate approved spec and G3 decision before coding. No 64MiB/200-year promise without measurement. |
| LATER — G3/G4 | New security-sensitive surface; default L4 resolution and canonical/public/release exposure | Explicit owner/security policy/remediation plus independent qualification; no waiver by #747. |
| LATER — parity | Legal 40/80 source capture, C-2b tolerances and C-6 Unity GO | Separate owner-authorised task; no Unity/Windows dependency for H0 review. |

**Settled, not reopened:** D-082 successful load appends exactly one `PlanStale(SessionLoaded)` per open Approved/PendingWork plan, ordinal plan-ID order; zero load events if none; no stale terminal resurrection. H0 performs no load activation and creates none of these events. H2 replay of a recorded stream must not simulate a second user load. Compare raw history hashes only for identical complete streams, not merely equal biology; no deletion of load events to manufacture equality.

## 8. Security, custody and verification economy

E's revised §8 correctly preserves [#694's narrow historical permission](https://github.com/jermur788/CCF/blob/916531789d6d6273ca134f951b18ba992b800063/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-694-MANAGER-723-STAGEA-ACCEPTANCE-AND-LIMITED-DEVELOPMENT-POLICY-20261118.md):

- **G0:** documentary reconciliation; no code authority.
- **G1:** separate lawful physical source/export admission; protected originals remain immutable.
- **G2:** a new specifically authorised isolated local **non-release** feature/prototype candidate may be considered with P0 L0–L3+ protection and honest known FAIL38. #694's Year1 permit does not automatically grant UI, Year2 or imports. Stop on new unresolved safety/protected hits; no analyzer weakening.
- **G3:** new Host/network/durable-save/security-sensitive filesystem or write surface requires a new explicit security-scope decision and approved contract **before code**.
- **G4:** canonical main/public/distribution/release/external use is blocked by default L4 FAIL38 UNWAIVED pending distinct owner-authorised remediation/policy and independent qualification. Prototype success is not L4 PASS or canonical integration approval.

Do not turn this into either a blanket prohibition on all separately authorised local development or a generic prototype exception for Host/saves. New public source exposure also needs its actual publication decision. Preserve original P0, domain/refusal behaviour, pins, fixture and source custody.

Custody is reported from [#735](https://github.com/jermur788/CCF/blob/816387ae05ffed4ac24e2560a08256068e459efd/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-735-725-QUALIFIED-YEAR1-SOURCE-CUSTODY-RETENTION-RESULT-20261118.md), not remeasured: original R #710 through **23 November 2026 18:00 UTC**; qualified #725 through **27 November 2026 18:00 UTC**, **no auto-purge**. These are custodial disposition dates, not fresh worker rights or authority to impose arbitrary agent time limits.

**Proportionate future evidence, NONE RUN HERE:**

| Changed component | Focused acceptance evidence |
| --- | --- |
| Read adapter/H0 | Fixture/report digest admission, all 100 IDs, forward XY→XZ bits, typed numeric/availability/stage cases, source aggregate match, wrong lineage/domain/version and immutable-copy negatives |
| A renderer | Known metre/radius scale, overlap/tie hit-test, keyboard list, resize/DPR, small viewport, visible disclosure, no mutation routes |
| C local marks | Exact threshold boundary/provenance, full-key stale callback, grouped undo/discard, atomic mixed batch, zero work/events/biology |
| B continuation | M-1 dead-target/neighbour and death-present continuity, unchanged Year1 witness, hazard/boundary edges, rollback/retry/concurrent publish, fixed-runtime ten-step repeat |
| D H0 / H2 | H0 only: two observations/no event/save mutation. H2 later: separate replay, Mode A/B, staleness, file adversarial and truthful pre/postrename faults under G3 |
| Security/integration | Scoped tests only when separately permitted; unchanged analyzer and P0 isolation; G4 security qualification remains outstanding |

Reuse #725/#729/#732 witnesses; do not rerun 43/98 tests, broad L4 archaeology, builds or Unity solely to review prose. Software behaviour, release safety and empirical forestry validation cannot be established by this documentary result.

## 9. Handoff and exact disposition

**Status: NEEDS CHANGES — INDEPENDENT SPEC REVIEW ONLY.**
Five findings: **0 BLOCKING, 3 MAJOR (F1–F3), 2 MINOR (F4–F5)**. “No BLOCKING” means no demonstrated fatal architecture/policy contradiction requiring rejection of the approach; the affected interfaces still require the major corrections before their specification can be approved.

Manager #730 should consolidate F1/F3 with E plus the consuming A/D boundaries, F2 with E/C, and F4/F5 as local B/C/D edits in the consolidated four-file draft. Targeted documentary readback of those corrections is sufficient for this review's next disposition; do not launch another general five-worker cycle. Present only actual owner choices for acceptance, then issue separate bounded packets when their gates are satisfied. A Year0 read-only slice need not await C/B/H2.

**Changed files:** this sole new Markdown. A–E, approved P1 files, decisions, source, physical board, protected worktrees and unrelated local work are untouched. Actual verification here is full documentary reading/cross-checking and GitHub publication/readback/one-file comparison; no executable tests, builds, rendered player/browser checks, Unity, Windows, Host, source integration or L4 work. Publication commit and exact comparison are supplied in the issue handoff after readback; this document cannot embed its own commit SHA without another commit.

**Remaining:** F1–F5 consolidation, applicable product/spec approval, source/data admission and future component-specific gates. Default L4 remains FAIL38 UNWAIVED. Source remains locally implemented/reviewed/focused-native-qualified at its recorded Year1 scope, not main-integrated or released. **Dashboard/Drive synchronization: NOT VERIFIED; published dashboard reflects this review: NOT VERIFIED.** Authorised Manager/publisher must reconcile live coordination and verify downstream publication; this one-file reviewer has no board/canonical write grant.

Return this result to #747 and Manager #730 as **HANDED-OFF**, leaving Manager disposition distinct from reviewer publication. No product DRAFT is approved.
