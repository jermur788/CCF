# Sitka P1 adult-rules comparator — DRAFT requirements and traceability checklist

**Status: ALL IMPLEMENTATION REQUIREMENTS UNTESTED / NOT APPROVED.** This is a DRAFT review checklist, not a record of passed tests. Related [spec](../spec.md), [plan](../plan.md), [tasks](../tasks.md).

**Authorization:** The owner approved *writing these four P1 DRAFT files only*, on 15 November 2026. MULTI-010 independently approved an **evidence plan** with zero blocking/major research-review defects, but *not a new annual model, scientific calibration, implementation, Unity source operation, or game integration*. Checkboxes remain unchecked unless a future approved-source handoff supplies verified evidence and a manager formally accepts it.

## A. Product and specification-gate requirements

- [ ] **REQ-A01:** Separate **DRAFT** spec explicitly distinguishes evidence-plan review quality, user draft permission, later spec approval, LAB-07 implementation, runtime verification and game user acceptance; no self-approval.
- [ ] **REQ-A02:** The purpose is an **adult-only, Mature-equivalent, closed, pure-Sitka CCF-rule experimental comparator**, NOT an Irish field forecast or a replacement production growth engine.
- [ ] **REQ-A03:** P0 retains calendar-only \`Advance experiment step\`, fixed original age/DBH/height, synthetic labels, exact old fixture bytes/hashes/events/schema1, save/Host and Windows/WSL usage; P1 is opt-in and separate.
- [ ] **REQ-A04:** Scenario One remains the *complete* three-species Sitka/Oak/Beech external demo with accepted MFM and repeated-intervention gates. This lab does not cut scope or authorise Windows Unity Player builds.
- [ ] **REQ-A05:** Reviewer addresses all **m-1…m-8** and **C-1…C-7** with actual source references/conditions; no default silently inserted as an accepted policy.
- [ ] **REQ-A06:** External measured plot access, non-Sitka/young stages, regeneration, browsed juveniles, storms, 3D, rack layout, contractor/economy, P1 forestry advice and CCF production saves are **OUT OF SCOPE**.
- [ ] **REQ-A07:** Separate independent **DRAFT quality** review before Manager/owner can approve P1 specification; later task claims and locks before work.

## B. Distinct data identity and immutable P0 contract — C-1

- [ ] **REQ-B01:** Distinct P1 root file \`format\`, schema version, file extension and guarded save-root policy decided and reviewed; P0 strict v1 reader refuses P1 and P1 does not automatically accept P0 except explicit read-only verified source import.
- [ ] **REQ-B02:** Fork provenance includes **original P0 synthetic fixture file SHA-256, P0 snapshot hash, tree IDs, age/inventory time, declared source class and original tree values**; no rewriting original P0 fixture, hash, event chain or saved state.
- [ ] **REQ-B03:** P1 must preserve exact synthetic site/polygon/XY provenance, with explicit adapter coordinate conversion; source P0 \`zMm=0\` is elevation, not Unity horizontal z.
- [ ] **REQ-B04:** Policy for a fork from original fixture vs a current P0 *post-execution* snapshot (and transfer of plan/felling history) is decided **before** a P1 reader is written; no hidden migration of P0 plans.
- [ ] **REQ-B05:** Distinct P1 canonical annual event grammar, ordered hashes/replay, exact version/refusal and safe source-file handling; unknown versions/field order/type refuse closed, never coerce to v1.
- [ ] **REQ-B06:** P1 saves alone can be written; no free path, no traverse/absolute/CCF-shaped filename, no symlink/hardlink/FIFO/device blocking, no out-of-lab temp/write, no untrusted Host/Origin/CSRF; unsupported native platforms fail closed.

## C. Source numeric precision and deterministic biological execution — C-2

- [ ] **REQ-C01:** Independently captured actual CCF source-runtime oracle at pinned exact source revision and model/seed/version, separate from nine Python formula reconstructions and historical saved hashes.
- [ ] **REQ-C02:** Explicit policy for **binary32 source float operations + mixed double expressions**, evaluation/rounding order, total annual state serialization and exact float round-trip vs independently reviewed tolerances (m-4); no annual authoritative rounding to integer mm.
- [ ] **REQ-C03:** Numeric comparisons include \`roll < p\`, 8 m cutoff, tie/order, RD 0.6/1 and one-tree boundary; no undetected discrete switch of mortality victims/plan states under "within tolerance".
- [ ] **REQ-C04:** Per-tree annual DBH growth matches Sitka potential/max/Ci50 rule and source clamps under declared stage/site and pre-growth CI; source SiteProductivity **equals 1** unless source changes are separately reviewed (m-2).
- [ ] **REQ-C05:** Hegyi uses living non-stump neighbours, source metres/correct spatial axis, inclusive 8 m threshold and stable ID order; review coordinate-centering/float-edge sensitivity (m-5).
- [ ] **REQ-C06:** Mature-equivalent only: stage-sensitive CCF height getter invalidates a generic Young/Sapling parity claim; unsupported stages rejected (m-1).
- [ ] **REQ-C07:** CCF Sitka Class III \`E(age)\` relative-height step, age chronology, height clamp and **SI30 distinction** from Broad–Lynch explicitly checked; synthetic P0 initial heights need no fabricated Irish site class.
- [ ] **REQ-C08:** Crown target/relaxation uses post-growth DBH and pre-growth CI in source order, with initial radius labelled assumption, not inferred historical crown.
- [ ] **REQ-C09:** Per-tree natural mortality uses source post-growth density, suppression, source RNG model (not growth model), seed/year/full ID, correct order and cause/year; density boundary guards (single tree allowed to remain RD>1) checked (m-6).
- [ ] **REQ-C10:** Source growth **2** for new games, model **1** equivalent pure-Sitka Reference-demand density route, growth **0** preserved for old/frozen references; reject unsupported mixed/non-Reference/old-source targets, do not claim generic model2 parity.
- [ ] **REQ-C11:** Felling before annual advance changes the next CI; no cut before explicit Execute and no second same-year surviving-tree DBH release. Post-growth density mortality and felling have distinct sources.

## D. Stand population, source state, geometry and top-height — C-3/C-4/C-5

- [ ] **REQ-D01:** Define CCF **all-living** population for biological CI/RD/growth, including below P0 action threshold; separately report P0 \`actionEligibleAdult\` subset, never silently use it as CCF biological denominator (m-8).
- [ ] **REQ-D02:** State what "Mature-equivalent" means for each live synthetic P0 stem irrespective of original DBH threshold, including obviously small trees; reject non-Sitka/non-Reference/pruned/phase/storm/recruitment conditions. This is a research abstraction, not field stage validation.
- [ ] **REQ-D03:** P0 \`Dead\` entries (nine demo, 107 performance by independent count) have **no recorded cause/year**; an explicit synthetic **pre-inventory dead** mapping is decided, excluded from current living/CI/RD and never counted as newly simulated natural death (m-3).
- [ ] **REQ-D04:** Initial crown, age, source model/RNG version, seed, site productivity 1, absence of history and derived float state are recorded with provenance; no fabricated empirical initial crown or biological cause (m-2/m-3).
- [ ] **REQ-D05:** **Square centred source-parity stand** vs **P0 non-square polygon** comparison explicitly split; any polygon-area use is an adapter extension, not source bit equivalence (m-5).
- [ ] **REQ-D06:** Top-height H sample convention and ties/rounding/empty-stand cases stated; **original Broad–Lynch field top-height inventory rule NOT VERIFIED** and cannot be called exact equivalence until primary-source confirmation.
- [ ] **REQ-D07:** CCF Class III height E(30)=20.4 anchor versus historical stand-model SI30; declaration/inference of synthetic initial SI and domain acceptance including P0 fixture height mismatch (m-8).
- [ ] **REQ-D08:** Separate sparse/dense/threshold/tiny-area fixtures; reviewer source arithmetic found demo RD~0.1698, performance RD~0.6085 with all-living polygon-area formula, but neither is a current CCF runtime oracle.
- [ ] **REQ-D09:** Planting, juvenile promotion, recruitment, storms, canopy/light site variation and mixed-species population are explicitly **NOT IMPLEMENTED**. Closed-population annual accounting must never claim full-world CCF parity.

## E. Annual causality, per-ID ledger and persistence

- [ ] **REQ-E01:** Each modelled annual transaction records input source hash/config, ecological year, before/after per-ID age/DBH/height/crown/live status, CI/S, applied increments, density, p and RNG roll, source cause/year, immutable original P0 identity and separate work-event provenance.
- [ ] **REQ-E02:** Natural death is distinct from executed work/felling; no timber/work reward from natural death, no duplicate deceased ID, no duplicate death event after reload, felling can create deadwood so deadwood alone is not a cause oracle.
- [ ] **REQ-E03:** P1 work-plan DRAFT→PROPOSED→APPROVED→PENDING_WORK→EXECUTED/REJECTED/CANCELLED/STALE semantics are separate from P0; non-executed plans never change living status/BA.
- [ ] **REQ-E04:** Biology/death of a reserved plan tree causes explicitly atomic cancellation/staleness in P1, not an invalid halfway P0-shaped snapshot.
- [ ] **REQ-E05:** For a closed no-recruitment population, \`N_end=N_start−executedFell−naturalDeaths\`; BA change partitioned by actual growth and cut/death time/DBH, with both all-living and P0-actionable subsets.
- [ ] **REQ-E06:** Replaying an ordered P1 annual chain from its pinned model/source/fixture recovers identical state and event hash; \`one year → save → restart → load → next year\` equals \`two uninterrupted years\`, including death IDs, approvals and rolls.
- [ ] **REQ-E07:** Invalid/foreign/corrupt/oversize/linked/unknown-version data or unsatisfied preconditions refuse atomically without state changes, fake success or weak save fallback; correct HTTP status and no out-of-root orphan temp under malicious folder swaps.
- [ ] **REQ-E08:** Unsupported future model versions/timing rules fail closed; no silently upgraded P0 or old frozen game saves.
- [ ] **REQ-E09:** Linux x64 and owner-Windows Ubuntu WSL2 source file/security and 127.0.0.1-only Host checks are retained and requalified for touched paths. Native Windows lab file API not claimed supported.

## F. Source oracle and scientific comparator — C-6/C-7

- [ ] **REQ-F01:** P1-01 oracle capture lane/owner/actual host/branch/HEAD and permitted Unity-source test route selected and explicitly authorised **after SPEC APPROVED**; no implicit Unity/Editor lease.
- [ ] **REQ-F02:** Historical source version mismatch (CCF save schema **24** vs a Sitka verifier \`version==23\` assertion) is owned as a **separate verification repair** with independent review, not quietly weakened/waived.
- [ ] **REQ-F03:** Independent parity review covers V01–V09 arithmetic plus actual runtime vectors (V13 presently UNAVAILABLE), with precise source float/double, 8m, RNG and mortality branch decisions; event victim sets must match where faithful claims are made.
- [ ] **REQ-F04:** Existing \`Tools/Verification/Growfor\` \`BroadLynch2006_Sitka\` comparator reused/requalified, not overwritten or called current GROWFOR. Historical **24+18** tests only establish reconstruction internal consistency.
- [ ] **REQ-F05:** B/N/H comparisons use pure even-aged stand and domain-matched site/age/SI/thin controls; distinguish model-to-model differences from measured validation and from current per-ID CCF dynamics.
- [ ] **REQ-F06:** The 113 2006 nonresearch observations are attributed specifically to **volume-equation calibration**, not used as an untouched final-growth-forecast holdout (m-7). Cite PDF-relative vs printed journal pagination correctly; full manual/2013/2017/executable identity remain unverified unless checked.
- [ ] **REQ-F07:** Other unsupported claims: Irish individual/cause mortality, irregular continuous-cover transformation, crown/competition physiology, mixed stands, long-term measured predictions and verified licensed marteloscope raw data; **never** present them as PASS without separate evidence.
- [ ] **REQ-F08:** Every P1 UI/figure/export bears "**CCF adult rules — closed pure-Sitka population; NOT an Irish field forecast**" and source/model/RNG/fixture/stand-domain flags; P0 retains its existing no-growth banner.
- [ ] **REQ-F09:** All P1-01, P1-02, P1-03, P1-04 and P1-05 substantive tasks and independent reviews have separate GitHub Markdown handoffs, factual test provenance, owners/claims/locks and Manager dispositions; no code before P1-G0 SPEC/product approval.

## G. Independent quality-review checklist for this DRAFT (not implementation)

- [ ] **DR-01:** Verify every existing implementation/spec and reference URL resolves on the stated GitHub ref; P0 authorised scope and latest direct user DRAFT-only approval are not conflated with implementation authority.
- [ ] **DR-02:** Independently reconcile all eight m-1…m-8 minor correction dispositions against MULTI-010 and scientific/source truth.
- [ ] **DR-03:** Independently assess all seven C-1…C-7 unresolved decisions, proposed defaults and whether any blocker remains before future P1 implementation approval.
- [ ] **DR-04:** Cross-check \`spec.md\`, \`plan.md\`, \`tasks.md\`, \`requirements.md\` for scope drift, agreement on source models/years/precision/area/versions, P1-01 oracle dependency and safe separate data namespace.
- [ ] **DR-05:** Review failure/negative tests, numeric float/threshold sensitivity and all-living vs P0-eligible population; no new scientific precision or source constant is presented as Irish research.
- [ ] **DR-06:** Verdict **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED**, with blocking/major/minor findings and exact minimal corrections; neither reviewer nor author can grant P1 implementation approval.
- [ ] **DR-07:** Report isolated GitHub branch one-path handoff/readback and no source/Unity/spec-other files modified; claim closed HANDED-OFF; dashboard/Drive state honest.

## H. Decision register and actual status

| Condition | Future decision needed | Review-only proposal in current draft | Actual acceptance |
|---|---|---|---|
| **C-1** | Distinct format/import/fork, plan history | P1-only format+directory, immutable P0 hash and explicit import | **OPEN** |
| **C-2** | Numeric/serialization/exactness | Float32 source order with explicitly retained double calculations and roundtrip | **OPEN** |
| **C-3** | Biological population | All living Mature-equivalent in CI/RD/growth, actionable subset separate | **OPEN** |
| **C-4** | Initial stage/crown/dead/RNG/site | Mature; synthetic crown; site=1; pre-inventory dead; fixed seed/year | **OPEN** |
| **C-5** | Area/coordinates/SI/H | Source-matched square parity; polygon extension; SI declared; H definition unverified | **OPEN** |
| **C-6** | Current-source oracle/test lane | Bounded Unity or independently verified .NET oracle, stale verifier separately repaired | **OPEN** |
| **C-7** | Labels/empirical limit | Explicit synthetic closed CCF-rules non-forecast wording | **OPEN** |

**No acceptance boxes are checked.** The separate manager/owner SPEC approval is still pending; **no LAB-07 implementation, Unity work, integration or Windows game build has been authorised.** The first external demo remains the full three-species Scenario One. Dashboard and Drive mirror synchronisation **NOT VERIFIED**.
