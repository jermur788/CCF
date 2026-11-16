# CCF #598 — Independent full-candidate P1 approval-readiness review

**Verdict: NEEDS CHANGES.** The current four-file candidate is a useful DRAFT, but is **not approval-ready**. Findings: **1 BLOCKING, 4 MAJOR, 5 MINOR, 0 NIT**. One bounded four-file documentation correction is the minimum next task. No specification, P1 implementation, product policy, Unity allocation or integration is approved by this review.

## Review identity and custody

| Item | Actual record |
|---|---|
| Task / date | [#598](https://github.com/jermur788/CCF/issues/598), 16 November 2026 |
| Worker | OpenAI Codex **gpt-6.1-sol**, T3 Code/Codex harness; **medium** reasoning effort exposed by this session. Preferred High was not the active setting and is not claimed |
| Account / host | GitHub `jermur788`; Windows hp |
| Role / independence | Fresh reviewer session; no prior contribution or inherited author context in this conversation. The same provider/model authored #552: **provider/model overlap is a potential independence conflict**, not concealed. I cannot attest to worker identity outside this session. I did not author #562/#575/#578/#581/#595 in this session; their conclusions are not treated as self-proving. No author helper or delegated reviewer was used |
| Registration | [Actual pre-inspection RUNNING registration](https://github.com/jermur788/CCF/issues/598#issuecomment-6917292382) |
| Review branch | `review/multi-sitka-p1-g2-full-candidate-independent-20261116-v1` |
| Starting HEAD / candidate | **`5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89`** |
| Isolated source worktree | `C:/Users/hp/AppData/Local/Temp/ccf-598-review-source`; initially clean; no Unity opened or lease acquired |
| Existing checkout | `C:/Users/hp/Documents/GitHub/CCF`, `main` at `04defad6f28c33156cc986dd0685dcb1998073a4`, initially clean; other worktrees preserved |
| Live state | Fetched origin; read live #598 and comments, remote review HEAD, absent output path, and `origin/main:Docs/Project/coordination/active-tasks.md` before candidate inspection. No competing #598 claimant observed. Live main later read at `afad222f607a2dbc05de04a030ba3d8a1cf6cb71` |
| Context | No context commit supplied by the packet. Current context-set commit identified from origin/main history: `7801d9d4f6163f7d196c7d28f6b34a9e7876cf0d`; authority includes live D-082. Candidate remains fixed, not silently rebased to current main |
| Only tracked write | This new review Markdown. Specifications, P0, game source, fixtures, canonical context, coordination and ledger read-only |

The requested source review is credible at the session level with the above model-family limitation. This is not an assertion that a different provider reviewed #552. The findings below critically challenge inherited #552/#562 material, including capture feasibility and legal fixture coverage.

## Evidence read and preservation

All four current files were inspected, including their acceptance criteria, task gates, N01–N13 matrix, nine-row registers and technical appendices:

- [spec.md at candidate](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md)
- [plan.md at candidate](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md)
- [tasks.md at candidate](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md)
- [requirements.md at candidate](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md)

Earlier evidence was read directly, rather than relying on #595's summaries: [#566 full review](https://github.com/jermur788/CCF/blob/7bca1903227832d7097f41ad9ce8487e5d42b346/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-Annual-Growth-Technical-Proposals-Independent-Review-20261116.md), [#578 full delta review](https://github.com/jermur788/CCF/blob/5bd4b1dbf43d8b5676592002db9015327c38bbc6/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-575-Focused-Independent-Delta-Review-20261116.md), [#581 author correction](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-578-Final-Bounded-Documentation-Correction-Result-20261116.md), its [Manager closure](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-581-manager-final-draft-verdict-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-581-MANAGER-DRAFT-CLOSURE-AND-P0-NEXT-EXECUTION-20261116.md), [#595 dossier](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-g2-owner-decision-readiness-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-G2-Owner-Decision-Readiness-and-PreCapture-Gate-20261116.md), and [#589 exact-base ruling](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p0-589-manager-acceptance-and-p1-base-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-589-MANAGER-WSL2-EDGE-ACCEPTANCE-AND-P1-BASE-RULING-20261116.md). Their issue bodies/comments were fetched, along with #552's authorship record. Repository workflow, decision log/D-082, milestone, brief, overview and research index informed the authority/science checks. Historical links beyond these inspected sources were not exhaustively validated.

Independent Git/text checks found:

| Check | Observation |
|---|---|
| Decision registers | Exactly nine rows in each file; identical across all four and to #575 `53f3cd25`. SHA-256 of LF-joined rows with final LF: `d6365b4fe851f2984d7062f7238d689731652c14b255bb380a202b8fe54810e5` |
| Requirements | **72 unchecked, 0 checked** |
| N01–N13 | Designed/unrun in the text; no N14. No execution is inferred from their presence |
| Game source identity | `Assets` tree **`824a1394c69404c847a9841d5f531e398589730b`** identical at candidate, `fc18190b` and fetched origin/main |
| Qualified prospective P0 base | **`a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`**, per #589. P0 subtree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| P0 drift since documentation reference | `861b81d3..a7f41983` changes only `LabHost.cs` and `HostTests.cs` under P0; fixture tree identical: `ed1a037966295153fafe7853086e2bef7d8025f9` |
| Approval boundary | D-082 confirms original synthetic input, conditional load staling and disclosure only. G1 history is not G2 approval. #581 is author-side repair; #595 is non-independent synthesis |

Source references below use game files under `Assets/ForestPrototype/` at the reviewed candidate (identical to the cited historical game source), and P0 files under `Tools/MultiRackSitkaLab/` at the full #589 base above. All spec/plan/task/checklist line numbers are at the reviewed candidate.

## Findings and minimum corrections

### B01 — BLOCKING: original-import experiment has no executable geometry contract

**Locations:** `spec.md:100–121, 162, 181–187, 311`; `plan.md:179–191`; `checklists/requirements.md:67, 125` (REQ-D05/D11).

The draft correctly says polygons are non-parity and derived legal squares are tests. It does **not** define how an accepted original inventory actually advances: §5.1 reserves that for a later separately versioned extension. This is a missing first-experiment contract, not evidence that all geometry warnings were omitted.

Direct source evidence: [StandGeometryModel.cs:32](https://github.com/jermur788/CCF/blob/5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89/Assets/ForestPrototype/StandGeometryModel.cs#L32) knows only Legacy40/Enlarged80; `ForestEcologyController.cs:92–105` rejects unknown models and applies 40/80 m. Area is the evaluated f32 expression at `:73`. Original fixture JSON at `a7f41983` gives:

| Original fixture | Actual footprint | Exact polygon area | Population from JSON |
|---|---|---:|---|
| `oracle-quarter-ha` | 50×50 m square, corner coordinates 0…50,000 mm | 0.25 ha | 3 live, all age 28 |
| `sitka-flat-demo-v1` | Five vertices: (0,0), (80,0), (95,60), (30,75), (0,50) m | 0.58125 ha | 162 total; 153 live, 9 dead, 15 live below threshold |
| `sitka-flat-perf-2000-v1` | **200×100 m rectangle**, represented as a four-vertex polygon | 2 ha | 2,000 total; 1,893 live, 107 dead, 182 live below threshold |

These are static JSON counts and shoelace-area arithmetic, not Unity output. No original footprint is a legal source square. Recentering the sparse three-tree probe into Legacy40 preserves its useful wiring role but changes area to 0.16 ha and creates a derived fixture. Using 0.25/0.58125/2 ha for RD is an **adapter extension**. Using 0.16/0.64 instead changes original stand semantics. Neither establishes general exact game parity for an original stand. Isolated formulas may agree; the whole original-geometry claim cannot.

**Minimum correction:** specify two distinct claim domains in all four files. Recommend retaining derived 40/80 m fixtures for source parity and proposing an explicitly versioned **original-inventory exploratory mode**, with original immutable provenance, exact polygon area, a named area-to-numeric conversion, a named XY→XZ/origin conversion, flat-input refusal, all-live membership, and no clipping, resizing or inferred extra trees. Its RD/N/ha and outputs must say **source-inspired/non-parity geometry** in screen, trace and export. Cross-reference those rules in acceptance checks. This is a recommendation for later approval, **not adoption of #595's polygon proposal**. A compliant lower-scope alternative is to allow originals only for inspection and explicitly leave annual original experiments blocked until a separate extension spec; do not present that as a completed first growth experiment.

Minimum future negatives: original presented as derived or vice versa; unsupported 50 m source setup; area silently replaced with a bounding square; recenter/scale/clip changing original identity; wrong XY/elevation mapping; nonzero elevation; forged transform/area bits; omission of non-parity export label. The original-only first experiment remains useful for synthetic thinning/release/accounting comparisons once this contract exists. It cannot validate Irish forecasts. **OD-2 scope/horizon is premature until this correction is reviewable.** D-082 needs no reinterpretation or weakening.

### M01 — MAJOR: C-2a does not settle which oracle evidence is mandatory

**Locations:** `spec.md:200–209, 260, 328–329` versus `:390–391, 654, 673–682`; `plan.md:78, 189, 215`; `tasks.md:30, 144`; REQ-C13 at `checklists/requirements.md:122`.

§5.1 and AC-P1-02 require at-evaluation S/p and the complete intermediate trace; unavailable required values fail closed. §9 says a minimum oracle needs no instrumentation and defers bracket sufficiency to C-2b. A bracket using a **future P1-02** adapter is not evidence available when accepting P1-01, which gates that adapter. The reader cannot tell whether a correct Tier-1 capture passes G3 or remains blocked.

Source confirms the distinction: `ForestEcologyController.cs:1084` can return before S; S, mean vulnerability and probability are private locals at `:1086–1103`. `GetCurrentSuppression` (`:1825–1831`) is a different f32 calculation. Integer-only `SimulationRandom.Roll` (`SimulationRandom.cs:41–55`) can be recomputed exactly for fixed key/year/seed/model; that does not observe the actual call branch or p.

**Minimum correction:** choose and document the **evidence scope before capture**. Recommended technical basis: a Tier-1 **stored-output/CI/account/death-set witness**, with unavailable locals explicitly unsatisfied and no claim of S/p bit parity; an independently admitted Tier-2 copy is needed for intermediate observations. Align AC-P1-02, G3/N03 and REQ-C13 with that distinction. If full S/p observation remains mandatory, make failed/unapproved instrumentation an explicit G3 stop. Do not defer this acceptance-scope choice to C-2b. Later C-2b may assess output tolerances/brackets, but brackets cannot prove p equality and must cover mechanism/count/order outcomes, including boundary victims. No tolerance or death-set equivalence is approved now.

### M02 — MAJOR: bare R0 cannot supply the required victim-order comparison

**Locations:** `spec.md:645, 665–669, 679`; `plan.md:181, 189`; `tasks.md:144`.

R0 has “no hooks; only public final-state reads”, yet R1/R2 must equal R0 on **victim order**. Source `KillDensityVictims` (`ForestEcologyController.cs:1138–1157`) applies the temporary list then discards it. `ForestTree.ApplyMortality` (`ForestTree.cs:551–562`) stores cause/year and emits `MortalityApplied`, but stores no order. `AdultMortalityAccount` stores counts, not ordered IDs. Final state cannot reconstruct the sequence. Recomputing expected order in a harness would not be an R0 observation.

**Minimum correction:** make the no-hook control compare only genuinely observable final stored state, per-ID deaths and account values. Define a separate minimal public-event observer control for order, then compare richer observation against it; document its perturbation limitation. Alternatively admit the same minimal order subscription in both controls and accurately redefine R0. Pin subscriber inventory and clean up subscriptions. Preserve repeat R0 and exact stored-output comparison. Never fill an unobserved R0 order from R1 or an adapter. The control design is source-settleable; its neutrality remains an actual Unity result.

### M03 — MAJOR: portability A′/B can be read as bypassing provenance and trust policy

**Locations:** `spec.md:448, 732–733` versus `:772, 775`; `plan.md:194`; `tasks.md:145`.

The fixed-source/model/archive/trust checks are independent of arithmetic. But B allows a “class-B/C difference” whenever A matches, and A′ gates “only on class A” and records the rest. A removed/changed trust record must still refuse (§9.2); an unknown model/schema cannot become acceptable because a replay happens to reproduce hashes. The #581 Manager already warned about this; the current candidate still lacks the normative precedence.

**Minimum correction:** separate an unconditional **format/model/source/trusted-genesis compatibility gate** from the arithmetic-platform policy. A/A′/B/C must never waive it. Restrict cheap B to class-C diagnostic differences after compatibility passes; any supported class-B transition needs an explicitly versioned reviewed compatibility rule. Unknown/removed/changed fixture trust cannot be silently re-anchored. Do not classify an unchanged-arithmetic corrupt file as a platform fault merely because its diagnostic OS text differs.

Minimum future negatives: matching A but altered model/source/trust digest, removed fixture record, unknown archive, and corrupt bits plus changed diagnostic text; all refuse without activation or file change. Accepting a class-C-only successful replay says nothing about cross-arithmetic continuation. Owner portability choice remains open; no checkpoint or package is selected.

### M04 — MAJOR: post-rename fsync failure contradicts old-file preservation

**Locations:** `spec.md:241, 456, 574`; REQ-E11 at `checklists/requirements.md:127`.

The proposed writer does temp write → file `fsync` → `renameat` over final → directory `fsync`, then says “on any failure” unlink the temp, while the general save contract promises the prior file is preserved on failure. If rename succeeded and directory fsync fails, the final name already refers to the new bytes. The temp name no longer exists; unlink cannot restore the old file. This follows from operation order, not a measured filesystem fault.

The [#589 ruling](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p0-589-manager-acceptance-and-p1-base-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-589-MANAGER-WSL2-EDGE-ACCEPTANCE-AND-P1-BASE-RULING-20261116.md) explicitly limits qualified P0 success to rename commit, with no fsync/power-loss claim. P0 `NoFollowFiles.cs:145–177` tracks `renamed`; the #567 Host fix distinguishes committed save from later listing failure. P1 must preserve that honesty when adding durability.

**Minimum correction:** define pre-rename failure (old file intact; clean own temp), committed rename with confirmed durability, and committed rename with **durability unconfirmed** after directory-fsync failure. Give the latter truthful distinct response/state semantics; do not report rollback or automatically replay a biological command. Do not invent a backup/rollback architecture to satisfy an impossible blanket guarantee. Keep root owner/mode and file/directory fsync as defensible proposed requirements, subject to this correction and later independent runtime qualification. Future fault cases must cover file-fsync, rename and post-rename directory-fsync separately, plus response loss and optional list failure.

### m01 — MINOR: capture datum statuses and branch labels are not closed

**Locations:** `spec.md:647–650`; `plan.md:189`; REQ-C13.

`evaluationInferred` appears in :647 but is absent from the seven allowed statuses at :650. Also :648 groups crown intermediates/requested height with mortality locals and calls them `notEvaluated` when P≤0, although growth/crown run **before** the mortality early return (`ForestEcologyController.cs:478–481, 1237–1243`). With P>0, p is evaluated even when it is zero; the **roll** is what short-circuits at :1103.

**Minimum correction:** publish one finite status vocabulary and allowed per-datum record shapes, with reason required and no numeric payload for unavailable/not-evaluated. Distinguish inferred branch metadata from observed evaluation. Mark inaccessible evaluated crown/height locals unavailable, not skipped. Preserve exact f32/f64 word width, finite-only decoding, signed zero, agreeing round-trip decimals, unit/type checks, strict keys and version refusal. Add compact positive/refusal examples before P1-01; do not confuse §9.11 byte examples with capture schema/golden vectors.

### m02 — MINOR: external-build isolation proof still contains an impossible/vacuous lane

**Locations:** `spec.md:406–417`; `plan.md:199, 209`; REQ-B07.

Under mechanism (a), §9.2 now points L2–L4 at an external `git archive` extraction. That extraction has no Git index, so `git status` there is not a working-tree proof; using its absolute path in the original repository's Git pathspec is not a history proof either. Ordinary P0 compilation in that extraction writes bin/obj, contradicting an empty L4 on the whole extraction unless those outputs are redirected too. In addition, `inotifywait` alone cannot prove “no process opened for writing” as strongly as the L4 column claims.

**Minimum correction:** keep L0/L1 tied to the protected repository P0 subtree. For an external extraction use a hashed input manifest and supported write observation rather than Git status; distinguish immutable source inputs from explicitly disposable outputs and redirect outputs if a zero-write input-root claim is retained. Keep the repository subtree separately protected. State the actual syscall/watch coverage and limitations; a generic `strace -e trace=file` capture includes reads, so “empty” must mean decoded write-intent operations. No isolation mechanism or L0–L4 runtime proof was executed by this review.

### m03 — MINOR: fingerprint refusal precedence remains contradictory

**Locations:** `spec.md:748–754, 761, 783–784`; `checklists/requirements.md:83`.

The reader first rejects malformed JSON/duplicate keys, while test 4 expects truncated/duplicate fingerprint data to reach `FingerprintMissingOrInvalid`. Unknown fingerprint version is required to yield `UnsupportedVersion` but has no explicit place in the stated sequence. Host capability is numbered last while described as startup-before-open.

**Minimum correction:** distinguish syntactically malformed JSON from a well-formed but invalid fingerprint record; define exact precedence for duplicate keys, fingerprint version and field shape, and put startup capability outside/before file processing. Align table and negative examples. None may mutate session or disk.

### m04 — MINOR: required tiny-area singleton oracle cannot occur in the legal source domain

**Locations:** `spec.md:106, 185, 273, 336`; `plan.md:124, 183, 193`; `tasks.md:59`.

The source's count≤1 escape is real, but 40/80 m only plus post-growth DBH≤200 cm cannot produce a singleton RD>1. Static upper-bound arithmetic from `SitkaGrowthModel.RelativeDensity` gives `(1/0.16)*(200/25)^2.063/1868 ≈ 0.244106`; 80 m is lower. A “tiny-area” Unity fixture would need the unsupported geometry workaround the draft prohibits.

**Minimum correction:** distinguish a legal-square singleton runtime case from the generic source-derived count≤1/tiny-area property. Keep the latter as source inspection or a separately labelled function-level mathematical probe; do not demand a fabricated legal-square Unity witness, relax the geometry gate, or force universal RD≤1 in a port. This is coverage feasibility, not a requested production change.

### m05 — MINOR: present-tense authority/base/review status is stale

**Locations:** `spec.md:81–85, 100, 349, 377, 400, 407, 712, 796, 805`; `plan.md:47, 75, 209`; `tasks.md:28, 145`; `checklists/requirements.md:33, 149`.

The #589 base is already selected and #578/#581 already happened. The current §4 still calls C-1A owner choice OPEN/offers persistent approvals despite §7/D-082 confirmation. Historical revision banners may remain historical; operative instructions cannot present those choices as live defaults. Tree `fa25ae6e…` is historical `861b81d3`, not the prospective P1 P0 tree `f212c8f6…`.

**Minimum correction:** add one current authoritative disposition and update operative forward references across four files; retain dated history and exact prior review limits. Cite #589's full source SHA, preserve fixed candidate/source distinctions, and preserve the nine accepted register rows rather than rewriting owner decisions. Do not assume P0 merged to main.

## Technical disposition and timed gates

| Condition | Independent assessment of technical proposal | What must remain open |
|---|---|---|
| C-1 identity/trust/Host | Sibling P1 root/service/ignore, fixed source base, separate original and derived trust tables, bounded descriptor reads, strict separate formats, exact Host/Origin/P1 header and port exclusion are technically adequate design directions. P0 `FixtureCatalog.cs:20–44` hashes bytes it reads, not an independent anchor; `LabFile.cs:125–138` supplies strict parse/canonical/hash checks. Do not substitute the co-located manifest | M03/M04 and m02/m03 corrections; reviewed trust values, actual root/no-follow/Host/race qualification; no tested P1 implementation |
| C-1 replay/commands | Compound annual event and one-time death children, recorded-rejection consumes ID, refusal consumes nothing, dedup before revision, history-derived dedup and generation-fenced atomic load activation are coherent proposals | Real canonical grammar/golden G-01…G-08; failure/concurrency qualification; caps, time horizon and replay budget. Snapshot/history hashes are integrity checks, not signatures |
| C-2a | Header/source/asset/host/Editor/Mono pins, actual RNG/seed/year readback, pre-step invalidation+CI, survivor post-step cache read under four reflection safeguards, raw typed bits and R0/R1 are sound components | **Not technically complete until M01/M02/m01/m04 are corrected.** Before scheduling, release packet fixes actual host OS/build, Mono identity, source/harness blobs, seed and exact derived fixtures, readback/refusal forms and observable evidence scope |
| C-2b | Correctly postcapture. CoreCLR/.NET implementation versus Mono reference is a cross-runtime comparison even on one host | Real repeatable vectors; reviewed numeric margins and identical discrete sets/order. No exactness/tolerance chosen here |
| C-3 | **Technically adequate:** every living stem, including below action threshold, synthetic Mature/Sitka/Reference; actionable subset separate | Runtime membership/accounting negative cases; no owner subset choice needed unless scope changes |
| C-4 | **Technically adequate:** copied synthetic per-ID age; synthetic initial crown, ecology year/seed; pre-inventory dead with null cause/year/crown and no new death; site1 source rule | Actual configuration readback and initialization/trust vectors. Site1 is inspected source behaviour, not measured soil/site index |
| C-5 | Legal-square derived geometry path is adequate; original path needs B01. Diagnostic top-k H, explicit k/ties/nulls, inferred-SI-or-blocked-comparison are defensible proposals | Primary Broad–Lynch field-estimation equivalence, admissible SI/equation/domain and comparator identity before P1-04; no fabricated field precision |
| C-1A / C-7 | **Owner-confirmed D-082**, preserved: originals only, fresh session epoch and conditional one-stale-per-open-plan/zero otherwise; synthetic/not-Irish-forecast disclosure | Technical implementation and visible wording/rendering qualification; no reopening owner decision |
| C-6 | **OPEN and unbooked** | Separate explicit owner machine/slot permission after reviewed pre-capture contract and spec approval; later operator preflight/normal exclusive lease |

Biological source checks support the core: ordinal enumeration (`ForestEcologyController.cs:863–885`), inline inclusive 8 m CI and distance floor (`:964–996`), growth/age then crown then mortality (`:467–481, 1017–1036, 1229–1244`), post-growth RD with retained pre-growth suppression, hazard then suppression/ordinal boundary removals (`:1082–1134`), and mortality cause/year (`ForestTree.cs:551–562`). Site productivity is initialized to 1 (`ForestEcologyCell.cs:48`, off-grid fallback at controller `:1957–1960`). Sitka linear crown branch and asset selection support synthetic zero-CI crown initialization. Crown affects canopy, which is excluded from this adult route; it is not an adult DBH/CI/mortality input here. No general full-world parity follows.

Configuration A/B (valid regeneration2+manager versus admissible regeneration0, no spawner/cohorts, storm0, time-lapse off) is a sensible **proposed** adult-neutrality check. Its actual neutrality cannot be established from labels or source reading. Reflection may read pinned private dictionaries after return with fail-closed member resolution and ordinal ID copying; it must not configure an illegal stand or manufacture missing victim values. Instrumented R2 must preserve expressions and publish its diff, compare stored outputs against controls, and disclose that matching outputs cannot prove every intermediate unchanged. No R2 source copy is authorised here.

The remaining science restrictions are appropriate: Class III height is not Broad–Lynch SI30; diagnostic top-height definition is not proven equivalent to subplot field estimation; 113 plots relate to volume calibration rather than held-out longitudinal forecasts. GROWFOR/Broad–Lynch comparisons are model-to-model and domain-labelled, with no per-ID hazard/crown truth target. The #595 dossier occasionally groups site1 with synthetic choices; the candidate's more precise distinction—source site1 versus synthetic Mature/crown/seed—is retained here.

Caps and replay cost remain proposals: **5,000 trees, 200 years, 200,000 events, 64 MiB, 32 MiB trace are not supported limits**. The compact shape is reasonable; §9.11 strings are illustrative, not a grammar or golden vectors. Full-state serialization/hashing per event and adversarial small events must enter the later load budget. No timing, benchmark, smallest legal event, alternate hash design or checkpoint is accepted. A bounded writer/benchmark will be needed at P1-03 under a separate packet; no new simulation was run for this review.

## Single next safe task and owner queue

**Next:** Manager dispatches **one bounded docs-only correction of the four existing P1 DRAFT files** for B01, M01–M04 and m01–m05, with a concise finding-to-line disposition. Keep DRAFT, the nine register rows, 72 unchecked boxes and N01–N13 unrun; no code, fixtures, tests or Unity. Reconcile the proposed original non-parity mode, the capture evidence tiers/controls/schema, unconditional provenance gate, save commit/durability semantics and feasibility/status text. Prefer a focused changed-lines review of that material correction; do not repeat a whole historical documentation audit or merely recount illustrative byte strings.

Then the Manager can present a concrete candidate for explicit owner G2 consideration. This review grants **no P1 implementation approval**. Approval status changes and production choices remain with their recorded authorities.

- **OD-1:** no new question is needed; the user's present instruction already authorises this docs review. It is not permission to implement.
- **OD-2:** defer final first-experiment scope/horizon until B01 is written and reviewed. No arbitrary 200-year requirement is derived here. Parking remains possible; original experiments remain useful under honest extension boundaries.
- **OD-3:** defer portability selection until the non-waivable provenance gate and accurate A/A′/B semantics are reviewable. Class-A architecture/runtime/math-library identity and a bounded feature-class proposal are conservative candidates, not a demonstrated minimal causal field set. That set requires capture/platform evidence; no optional probe digest is adopted without its versioned argument list. C/checkpoint portability is separate scope.
- **C-2b:** only after real Unity capture; it never blocks the pre-capture design circularly.
- **C-6:** no request or physical booking now. Separate explicit owner permission is required later.

## Verification, publication and handoff limits

**Performed:** Git/source/issue inspection; static JSON inventory and polygon-area arithmetic; register row equality/SHA-256 and checkbox counts; source-tree/P0 fixture-tree identity and bounded diff inspection; scalar singleton upper-bound arithmetic. Small inspection helpers lived outside the repository. These are source/document checks, **not project tests or a biological simulation**.

**NOT RUN:** Unity/Editor/Mono/.NET; compiler, project tests or mutation suites; lab service/browser UI/WSL; save/load or user-save access; Windows/Player build; numerical capture, benchmarks, platform arithmetic probes; P0 physical L0–L4/build-isolation qualification. No runtime PASS or current parity is claimed. Older qualified results are attributed to their authors/Manager and limited to their tested source/configuration.

Publication acceptance is one new Markdown relative to `5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89`, pushed on the assigned review branch, followed by GitHub compare and byte readback; the exact publication commit/readback receipt is recorded in the #598 HANDED-OFF comment. No PR/main merge is needed or authorised. Leave **#598 OPEN** for Manager disposition and notify #595/#565/#425 with new comments only.

**Dashboard and Google Drive mirror: NOT VERIFIED.** Synchronization is an outstanding operational item for the Manager; no generated state or coordination file was changed by this worker. The required GitHub review document supplies the tracked handoff; it does not imply dashboard ingestion.

**Worker assessment:** ready for Manager review of this **NEEDS CHANGES** verdict. No source implementation, integration, runtime qualification or product acceptance is implied.
