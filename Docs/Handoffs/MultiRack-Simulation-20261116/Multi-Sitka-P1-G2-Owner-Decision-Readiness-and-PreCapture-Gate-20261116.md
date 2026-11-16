# Multi P1-G2 — Owner decision readiness and pre-Unity capture gate (#595)

**Date:** 16 November 2026. **Claim:** [#595](https://github.com/jermur788/CCF/issues/595). **Binding packet:** [P1-G2 Manager owner-decision readiness packet](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-g2-owner-decision-readiness-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-G2-MANAGER-OWNER-DECISION-READINESS-PACKET-20261116.md). **Programme:** standalone 2D Multi-Rack Sitka P1 adult growth and natural mortality laboratory only. This is not Scenario One, not a game feature and not a P0 change.

**What this document is:** a decision brief. **It is not** a product decision, a spec approval, an independent whole-spec review, an implementation plan, a Decision Log entry or permission to use Unity.

**Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code on the owner's Windows host. This is not the GPT-6 or DeepSeek worker the packet suggested. The work is GitHub-only. The file was written to the remote branch through the GitHub Contents API, and no local worktree was created or claimed. The local `main` checkout was used read-only (`git fetch`, `git show`, `git rev-parse`). **Independence disclosure:** Claude Opus 5.5 wrote the [#562 technical proposals](https://github.com/jermur788/CCF/issues/562) (spec §9 original text) and the #555 repair. So this synthesis of §9 is **not independent** of the §9 author. Branch `docs/multi-sitka-p1-g2-owner-decision-readiness-20261116-v1`, starting HEAD `83f544cec7170f5fec2331e7d7777000ff2683bb`, over the #581 DRAFT candidate `5fa1b1f16e89d1c5efe04dd20f9a79d5250a7f89`. Live `main` at claim: `dcadc4342e41722bf5ff98841690278aeb6c7fba`.

Anchors such as `spec §9.3` refer to `specs/multirack-sitka-p1-adult-rules-comparator-v1/` at `5fa1b1f`, which is byte-identical on this branch.

---

## A. Executive summary

**What has already passed (and what each pass means):**

| Item | Status | Meaning, and limit |
|---|---|---|
| P0 no-growth lab, #567 save-response fix | Independent source/security review [#586](https://github.com/jermur788/CCF/issues/586): 0 blocking, 0 major. Targeted Windows Edge/Ubuntu WSL2 delta [#589](https://github.com/jermur788/CCF/issues/589): Manager APPROVED | Verified **for the tested P0 no-growth configuration only**. Not merged to `main` (observation: `main` has 0 paths under `Tools/MultiRackSitkaLab`). No `fsync` or power-loss durability |
| P0 base for any future P1 | Manager ruling in [#589 decision §2](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p0-589-manager-acceptance-and-p1-base-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-589-MANAGER-WSL2-EDGE-ACCEPTANCE-AND-P1-BASE-RULING-20261116.md): exact `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` | Settles **only which P0 source P1 would build on**. It grants no P1 authority |
| Owner product boundaries (D-082) | C-1A and C-7 CONFIRMED on 15 November ([Decision Log D-082](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md)) | Design boundaries only. **They do not approve the spec** |
| Four-file P1 DRAFT (G1) | Closed by the Manager as **manager-checked DRAFT documentation** ([#581 closure](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-581-manager-final-draft-verdict-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-581-MANAGER-DRAFT-CLOSURE-AND-P0-NEXT-EXECUTION-20261116.md)) | **DRAFT / NOT APPROVED.** 72 requirements unchecked, 0 checked. N01–N13 designed but **unrun**. C-2b and C-6 OPEN |

**What is not approved:** the P1 specification as a whole, every technical C-item (C-1, C-2a, C-3, C-4, C-5), C-2b parity, C-6 Unity time, any P1 code, fixture, capture, save format, cap or portability policy.

**First experiment boundary (D-082, unchanged):** start from an original, independently verified synthetic Sitka inventory. Use P0-like conditional stale-on-load. Label everything synthetic and **not a forecast of real Irish forest growth**.

**The next single gate is G2 (P1-G2), which is spec approval.** G2 needs both (a) an independent non-author review of the current candidate and (b) an explicit product-owner approval (spec §7.1, R-8).

### GO / NO-GO to owner spec approval: **NO-GO**

Reasons (all source-backed):
1. **No independent non-author review exists for the current whole candidate `5fa1b1f`.** The independence trail is as follows:
   - #556 independently reviewed the #555 repair.
   - #566 reviewed the #562 §9 proposals (APPROVED AS DRAFT QUALITY only).
   - #578 was a focused independent delta of #575.
   - The final #581 corrections were made by the #578 reviewer **on its own findings**, so they are author-side ([#581 closure §1](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-581-manager-final-draft-verdict-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-581-MANAGER-DRAFT-CLOSURE-AND-P0-NEXT-EXECUTION-20261116.md)).
   - The Manager's own check is described there as "a bounded manager check, not a second model's full formal review".
   - **Therefore #581 is not independent final whole-spec sign-off.**
2. The technical C-items that G2 must "resolve or timetable" (tasks P1-G2 row; plan G2 row) have no reviewer acceptance yet.
3. The DRAFT still carries **stale administrative text** about the P0 base and the review state (Section G.2). Approving it as it stands would approve known-stale lines.
4. One scope gap affects what "the first experiment" actually runs on (Section C, finding C-GAP). A reviewer should settle it before the owner is asked to approve.

**The owner does not need to decide anything new to keep P1 safe.** The one useful owner input now is OD-1 in Section E: whether to spend one docs-only review on P1 now, or park P1 behind Scenario One.

---

## B. Decision ownership and timing matrix

**Authority classes:**
- **OWNER-CONFIRMED** — the owner has decided.
- **MANAGER-SELECTED** — the Manager made a provenance or working ruling.
- **REVIEWER-REC** — a technical proposal awaits independent reviewer acceptance, then approval at G2.
- **OWNER-OPEN** — a genuine owner choice that has not been made.
- **POST-CAPTURE** — cannot be known until a real Unity capture exists.

### B.1 The nine register rows (spec §7, unchanged)

| Item | Class | Blocks | Minimum proof at its proper gate |
|---|---|---|---|
| **C-1A** fork and approval-after-load | **OWNER-CONFIRMED** (D-082). Technical identity, trust and replay depend on C-1/C-2a | Nothing as a product choice. Its implementation is tested at P1-03 | N01 (original fork refusal set), N09/N10 Mode A/B (`plan.md` §9). Any change to post-intervention import or persistent approvals needs a **new** owner decision (spec §7 row) |
| **C-1** identity, storage, lineage | **REVIEWER-REC** (spec §9.1 T-01…T-10). The P0 **base** part is now **MANAGER-SELECTED** by #589 (`a7f41983`). Integration of P0 into `main` is still undecided | P1-02 import; P1-03 persistence | File-security reviewer accepts or replaces §9.2 (trust table, derived catalog, root, host, serialization) and §9.4 (command IDs, G-01…G-08). Both happen before P1-02/P1-03, not before G2 |
| **C-2a** pre-capture format | **REVIEWER-REC**, "designable without Unity" (spec §7) | P1-01 capture. **Owner should not be asked about C-6 before this is reviewed** | Independent numerical reviewer accepts the Section D checklist (spec §5.1, §9.5, §9.6) |
| **C-2b** parity target | **POST-CAPTURE** | P1-02 acceptance | Real Unity Editor vectors, repeatability (R0/R1/R0), distances to thresholds, identical death/victim/plan sets (spec §5 C-2b). **Never a pre-capture owner choice** |
| **C-3** population | **REVIEWER-REC** (all living, Mature-equivalent). It becomes **OWNER-OPEN only if** someone proposes a non-parity subset mode | Fixtures; P1-01 | Below-threshold controls and all-living accounting (N04; spec §9.7) |
| **C-4** initial state | **REVIEWER-REC**. The seed value and ecological-year mapping are **synthetic choices (SA)** | Fixtures; P1-01 | Per-field provenance; age/clamp/dead/reseed negatives (spec §9.7) |
| **C-5** geometry, SI/H | Geometry is **REVIEWER-REC** (40 m/80 m only). SI/H is **REVIEWER-REC for a science reviewer** before P1-04 | Oracle geometry (P1-01); P1-04 comparison | Area-bit readback; edge fixtures (N05). SI derivation or a blocked comparison (N12) |
| **C-6** Unity machine | **OWNER-OPEN, NOT YET READY TO ASK** (Section E) | P1-01 | G2 approved + reviewed C-2a + a Manager/operator preflight + an owner slot after the Scenario One priority |
| **C-7** disclosure | **OWNER-CONFIRMED** principle (D-082). Exact rendering is unverified | Display at P1-05/exports | Independent UI/science wording review; N13 |

### B.2 Remaining §9.10 choices (spec §9.10 decision queue)

| Choice | Class | Earliest proper gate | Minimum proof |
|---|---|---|---|
| P0 implementation base (§9.10 #1) | **MANAGER-SELECTED** by #589 (`a7f41983`). The DRAFT text is stale (G.2) | Already decided | Next authorised spec-maintenance revision cites it. L0–L4 proof at P1-02 |
| Cross-host portability: A / A′ / B / C / checkpoint (§9.9, §9.10 #2) | **OWNER-OPEN** for the usability direction. **REVIEWER-REC** for which fields are class A | P1-03 format | A cross-host comparison or the injectable-provider tests (§9.9 tests 1–9). Option C needs C-2b first. A checkpoint is a separate, costed scope |
| File + directory `fsync`, root owner/mode (§9.2, §9.10 #7) | **REVIEWER-REC** (file-security reviewer accepts or drops each) | P1-03 | Security and runtime qualification on ext4. #589 explicitly did **not** test `fsync` |
| Trust-table values (§9.10 #4) | Technical (import owner prepares them, file-security reviewer checks them). Values pending | P1-02 import | Values prepared from `a7f41983`. Observation: `Tools/MultiRackSitkaLab/fixtures` has tree `ed1a0379…` at both `861b81d3` and `a7f41983`, so the fixture bytes did not change with the #567 fix |
| Canonical envelope; examples versus real bounds (§9.3, §9.11) | **REVIEWER-REC**. §9.11 strings are **illustrative only**. The smallest legal event is **UNDECIDED** | P1-03 grammar + golden vectors | Canonical grammar with golden examples. Then derive or drop `MaxEvents` |
| Caps, replay budget, time horizon (§9.3, §9.10 #3) | Caps are **unvalidated placeholders**. The replay-time budget is MANAGER-to-set after a benchmark. The **horizon is OWNER input** tied to a named experiment (OD-2) | P1-03 | The mandatory §9.3 load benchmark on each target platform. No cap is accepted before it |
| H2 per-event state-hash cost and hash design (§9.3 options a–d, §9.10 #8) | **REVIEWER-REC** (replay reviewer) | P1-03 envelope | The benchmark. Options b–d change the envelope and the G-vectors |
| Read-only reflection for capture (§9.5) | **MANAGER-SELECTED working ruling**, under four safeguards. It permits no run | P1-01 | R0/R1 bit-exact control |
| C-2b numerical proof + C-6 booking (§9.10 #5) | POST-CAPTURE / OWNER-OPEN | P1-01, P1-02 | As in B.1 |
| Field and GROWFOR evidence (§9.10 #6) | Unverified science | P1-04 | Science reviewer. Never presented as Irish validation |

---

## C. Minimal, falsifiable first experiment (recommendation, not policy)

**Research question, narrowed (spec §1).** In a closed, pure-Sitka, all-living, Mature-equivalent synthetic stand, does a separately versioned P1 adapter reproduce the pinned CCF adult rules per tree and per year, and what stand N/BA/RD trajectory results under no-thin and one thinning? This is **model-derived gameplay-rules reconstruction**, not an Irish forecast (spec §1, §2; D-082 C-7).

**Finding C-GAP: no original P0 fixture is source-legal geometry.** Here is what the source says:
- The pinned source admits only Legacy40 (40 m, 0.16 ha) and Enlarged80 (80 m, 0.64 ha) square stands (spec §5 C-5; `StandGeometryModel.cs:32, 34`).
- `oracle-quarter-ha` is a 50 m square. The demo and performance fixtures are polygons (spec §4.1 M-1).
- Polygon runs are "non-parity exploration", and a polygon extension "would have a separate mode/version and label" (spec §5.1). **That mode is not specified anywhere in the DRAFT.**

The consequence follows from two constraints:
- D-082 requires the **experiment** to start from an original fixture.
- Source parity can only be demonstrated on **derived** 40/80 m test fixtures.

So the first experiment on an original fixture is necessarily a **non-parity adapter extension**: the polygon area is used for RD. That is a model derivation (my synthesis of spec §4.1/§5.1/C-5), not a stated DRAFT conclusion. The independent reviewer should either (i) require the DRAFT to specify that labelled polygon-area mode before approval, or (ii) state that the first experiment's original-fixture runs are explicitly labelled non-parity. **Treat this as a review item, not an owner question yet.** It feeds OD-2.

**Recommended bounded sequence.** Each step requires its own released packet. None is released here.

| Step | Gate | Content | Inputs | Labels |
|---|---|---|---|---|
| 0 | G2 | Independent review + owner approval of a revised candidate | This dossier + DRAFT | — |
| 1 | G3 (needs C-6) | Unity Editor (Mono) capture on **derived** fixtures only: the Legacy40 sparse probe from `oracle-quarter-ha` (wiring; CI = 0, P = 0, spec §5.1), plus a small dense/edge/tie/singleton set on 40 m squares and one small-age vector (T-17) | Reviewed C-2a; C-3/C-4/C-5 geometry | **Test fixtures**, never original imports (D-082 (1)) |
| 2 | G4 | Adapter reproduces the step-1 vectors under the post-capture C-2b target | C-2b | Parity evidence for the capture host/runtime only |
| 3 | G5 | P1-owned persistence: compact archive, Mode A/B, refusal classes, benchmark-derived caps | C-1 accepted | — |
| 4 | Experiment | Original fixture import through the trust table: `sitka-flat-demo-v1` (153 living, 9 pre-inventory dead; RD ≈ 0.17, so no early density mortality) for wiring; `sitka-flat-perf-2000-v1` (1,893 living; RD ≈ 0.61, near the 0.6 onset) for density mortality. Schedules: no-thin and one thin, horizon from OD-2 | Steps 1–3 | **Non-parity polygon-area mode (C-GAP)**; C-7 label; "no recruitment or storms" |

The RD figures are reviewer arithmetic on synthetic fixtures, not runtime results (spec §5 m-8).

**Deliberately excluded from the first experiment:**
- Recruitment, storms, juveniles, species mix, Oak/Beech, racks, slope, Unity/3D, economy and production saves (spec §1).
- Any P0 thinning, approval or event history (D-082 (1)).
- **P1-04 GROWFOR B/N/H comparison.** It needs a separate even-aged cohort and a resolved SI/H convention (spec §5.3, §9.7). Recommend it as a **later** step, so that the first experiment cannot be mistaken for validation against Irish data.
- P1-05 full viewer, beyond a C-7-labelled export or report.

**Labels:**
- All-living, Mature-equivalent and site = 1 are a **gameplay calibration / synthetic assumption (SA)**.
- Initial crown is SA, and crown does not affect adult outputs on this route (spec §9.7, SDI).
- Seed and year mapping are SA.
- The density exponent and coefficient are British transfer, gameplay-calibrated (spec §3).

---

## D. Pre-Unity C-2a contract: completeness review (checklist of proposals, not a capture)

This lists what the technical capture owner must finish, and an independent numerical reviewer must accept, **before** the owner is asked for C-6 Unity time. Sources: spec §5.1, §6 item 2, §9.5, §9.6. **Nothing here licenses instrumentation or a run.**

**D.1 Identity and environment (header)**
- [ ] Full CCF source SHA and per-file blob digests. *Observation:* the `Assets` tree is `824a1394…` at both `5fa1b1f` and live `main` `dcadc434`, so the DRAFT's `fc18190b`/`f0031848` line citations still hold. The capture packet must still fix one exact commit.
- [ ] Exact Unity `6000.6.0f1` Editor and Mono build, compiler and build configuration, as observed rather than inferred (spec §5.1).
- [ ] **Capture host OS named in advance.** The DRAFT says a Linux Editor capture is Linux-only evidence (spec §3). It does not name the intended host. **Gap.**
- [ ] P0 pin `a7f41983` (#589), fixture file SHA-256 and P0 snapshot hash.
- [ ] Platform fingerprint fields (§9.9). Unreadable fields are recorded as `unavailable`, and libm identity is never invented.
- [ ] Sitka asset identity and digest, including `usePowerLawCrown: 0` (m-B).
- [ ] Harness revision; scene component inventory; `MortalityApplied` subscriber list (§9.5).

**D.2 Configuration readback (spec §6 item 2, R-7)**
- [ ] Growth model 2. `rngModelVersion` and `simulationSeed` are **read back**, never assumed (m-6). Choose an explicit seed: 20260914 is only a worked example.
- [ ] Regeneration route chosen and recorded: regeneration 2 needs a `ScenarioOneManager` (`ForestEcologyController.cs:483–486`).
- [ ] No spawner, storm 0, `timeLapseEnabled == false`, all Mature, Reference demand, no pruning.
- [ ] Geometry model and L accepted; `StandAreaHectares` area **bits** captured (0.16 or 0.64 ha). Any resize stops the capture. Enlarged80 approach-path readback.
- [ ] Species bound explicitly. Grid built. `EcologicalYear` advanced by exactly +1 per step (m-C).
- [ ] **Configuration A/B** proving the non-adult configuration does not change adult outputs (`plan.md` §9).

**D.3 Per-ID numerical records**
- [ ] Every float is a typed f32/f64 **raw-bit** datum plus a decimal that agrees exactly; decimal-only round-trip is refused (spec §5.1).
- [ ] Order of reads: (1) **pre-growth CI** via `InvalidateCompetition()` and then the getter (trap 2, §9.5); (2) **at-evaluation** S/p/roll only where the source evaluates them, otherwise `notEvaluated(reason)`; (3) post-growth stored DBH/H/crown; (4) `LastAdultMortality`; (5) survivor cache cross-check by read-only reflection under the four safeguards.
- [ ] Roll key is the literal `"ADULT-MORT-" + TreeId` with verbatim IDs and the post-increment year (m-5, m-F).
- [ ] Victim order from `MortalityApplied`; hazard first, then boundary (T-12).
- [ ] Ordinal ID ordering; inclusive 8 m boundary and ties; clamps DBH [1, 200], H [0.1, 60], crown ≥ 0.1 exercised (spec §3).
- [ ] Seven datum statuses. `unavailable` and `notEvaluated` never carry a number (§9.5).
- [ ] S, p and mean vulnerability are **BLOCKED** without an admitted instrumented copy (§9.6). The roll/p bracket check substitutes, and is not an observation.

**D.4 Derived fixtures and geometry**
- [ ] `corner-square-to-centred-xz-v1` with exact rational offsets. P0 `zMm` is elevation; nonzero values are refused (spec §5.1).
- [ ] The derived catalog is separate (`p1d-` IDs) and never lives in P0's `fixtures/synthetic/` (§9.2 T-03).

**D.5 Mandatory negative cases (N03, N05, N07)**
- [ ] NaN or infinity, a decimal/bit mismatch, an unknown version, a missing field, a duplicate key.
- [ ] A 50 m or resized stand, a hidden resize, a reflection-configured stand.
- [ ] Swapped RNG/growth selectors; S recomputed after growth; `≤` instead of `<`; a renamed or re-cased ID.
- [ ] R1 ≠ R0 on any bit blocks the oracle.

**D.6 Gaps the reviewer should close (model derivation, not DRAFT text)**
1. Name the capture host OS and Editor platform. State that the P1 adapter runs on CoreCLR .NET 8 (the P0 build config is net8.0, spec §9.2), so adapter parity against a Mono capture is **cross-runtime by construction**. C-2b must address that explicitly.
2. Fix the seed and the fixture list for step 1.
3. Decide whether the optional stale `version==23` harness repair is needed (spec §6 item 1, R-6). If a new dedicated harness is used, it is not needed.

**C-2b is decided only after real vectors exist. It is never a pre-capture owner choice.**

---

## E. Owner-facing decisions (at most three)

| # | Question for the owner | Alternatives and consequences | Recommended timing |
|---|---|---|---|
| **OD-1** | Should P1 continue on a **docs-only approval track now** (one independent G2 approval-readiness review, Section F), or be **parked** behind Scenario One? | **Continue:** costs one review, uses no Unity or machine, and does not compete with Scenario One. **Park:** nothing is lost; P0 stays verified and the DRAFT stays frozen. Either way, Scenario One remains first (D-060; milestone) | **Ready to ask now.** Recommended: **continue, docs only** |
| **OD-2** | Confirm the **first-experiment scope** in Section C: derived-fixture parity first; then original demo and performance fixtures under a **labelled non-parity polygon-area mode**; no-thin plus one thin; GROWFOR comparison later. State the **horizon in years** you want to see | This ties `MaxEcologicalYears` to a named experiment, as spec §9.3 requires. **It is not a cap acceptance.** A longer horizon raises replay cost (§9.3). Alternative: run the experiment only on derived square fixtures, which gives parity but is not an original inventory. That conflicts with the spirit of D-082 (1) and would need a new owner decision | **Ask in principle now, with OD-1.** Bind it at G2 after the reviewer settles C-GAP |
| **OD-3** | **Save portability direction:** is "a P1 save opens only on the computer and software setup that wrote it" acceptable for the first experiment? | **A (DRAFT default, unapproved):** simplest, with no false corruption reports. The cost is that a routine runtime, OS, CPU-flag or trust-table update can refuse an old save **even when the maths is identical** (§9.9). **A′:** fewer refusals, but needs evidence about which fields matter. **B:** covers cosmetic diagnostic differences only. As the #581 Manager noted, class-B provenance and trust-table changes need separate compatibility handling; B never makes an arithmetically different history portable. **C:** needs C-2b first and possibly a package. A Linux/Windows mathematics gap is unmeasured (§3) | **NOT YET READY TO ASK as a final choice.** Ask at G2, once the reviewer has fixed the class-A field list. A direction now ("same machine is fine for the first experiment") would help the reviewer |

**Explicitly NOT YET READY TO ASK:**
- **C-6 Unity time.** It needs an approved G2, a reviewed C-2a, a Manager/operator preflight and a slot after the Scenario One priority (spec §6 item 2).
- **C-2b**, which is post-capture.
- **C-3 subset alternatives**, unless someone proposes one.

**Reviewer/Manager queue (not for the owner):**
- C-1, C-2a, C-3, C-4, C-5 acceptance.
- `fsync` and root checks.
- Trust-table values.
- Envelope grammar and the smallest event.
- Benchmark and replay budget.
- H2 hash design.
- The class-A field list.
- The stale-text revision (G.2).

---

## F. P1-G2 evidence checklist and next dispatch

| Area | Verdict | Anchor |
|---|---|---|
| Whole four-file candidate `5fa1b1f` | **Needs technical contract review.** No independent non-author whole-candidate review exists | #581 closure §1; spec §7.1 |
| Owner product boundaries | **Ready**: C-1A and C-7 confirmed | D-082; spec §7 rows |
| C-1, C-2a, C-3, C-4, C-5 | **Needs technical contract review** | spec §9.1, §9.7; tasks P1-G2 row |
| C-GAP (original fixture versus source-legal geometry) | **Needs technical contract review** | spec §4.1 M-1, §5.1 |
| Stale P0-base and review-state text | **Needs a docs revision** at the next authorised spec-maintenance pass | spec lines 349, 377, 400, 712, 796, 805; plan 47, 75, 209; tasks 145; requirements 33 |
| Scope (OD-1, OD-2) | **Blocked on owner choice**, which is cheap and docs-only | Section E |
| Portability (OD-3), C-6 | **Not yet ready to ask** | spec §9.9; §6 item 2 |
| C-2b | **Post-capture only** | spec §5 |

**Single recommended next task, after the Manager accepts this dossier and the owner answers OD-1 with "continue":**

**Independent non-author G2 approval-readiness review of the P1 four-file candidate `5fa1b1f`, with C-2a pre-capture completeness as a mandatory section.**

**Scope:**
- Whole-candidate coherence.
- Accept, replace or time each of C-1, C-2a, C-3, C-4 and C-5.
- Settle C-GAP.
- Confirm the Section D checklist and close its gaps D.6 1–3.
- Fix the class-A field list needed for OD-3.
- List the exact text revisions needed, including the stale lines in G.2, for one later approval-revision task.

**Worker and lane:**
- **Difficulty: HIGH.**
- **Recommended worker:** Codex `gpt-6.1-sol` High. It authored none of spec §9, the least independently reviewed part of the current text. Its #552 material has since been independently reviewed (#554/#556) and repaired by others.
- **Fallback:** a fresh GPT-6 session holding no Manager role.
- **Not:** Claude Opus 5.5 (#562/#555/this dossier), DeepSeek V4.1 Flash (#578/#581) or Claude Sonnet 5.5 (#575) as primary.
- The Manager records whatever residual family conflict remains.
- **Lane:** browser/GitHub or T3, docs-only, on a new review branch.

**Output:** exactly one new review Markdown under `Docs/Handoffs/MultiRack-Simulation-20261116/`. No DRAFT edits, no Decision Log or board edits, no code, no Unity, no tests.

**This dossier does not create that task.** Nothing here releases implementation, Unity time, fixtures, P0 edits or mutation of the four DRAFT files.

---

## G. Evidence provenance and limitations

### G.1 Labels used

| Assertion | Label |
|---|---|
| DRAFT status, 72 unchecked / 0 checked, nine register rows, N01–N13 unrun | **Directly source-backed.** Observation on this branch: `grep -c "\- \[ \]"` = 72 and `"\- \[x\]"` = 0 in `checklists/requirements.md`. Register rows per spec §7 |
| D-082 content | Directly source-backed (`main` Decision Log row D-082) |
| P0 base `a7f41983`; not merged to `main` | Directly source-backed (#589 decision §2); git observation (0 paths on `main`) |
| Fixtures and Core unchanged from `861b81d3` to `a7f41983` | Git observation: tree ids `ed1a0379…` (fixtures) and `4a434396…` (`src/Core`) are equal. The diff touches only `LabHost.cs` and `HostTests.cs` |
| `Assets` tree identical at `5fa1b1f` and `main` `dcadc434` | Git observation: `824a1394…` at both |
| C-GAP, cross-runtime adapter note, sequence in Section C | **Model derivation** (this synthesis) |
| All-living / Mature / site 1 / seed / year mapping / crown init | **Gameplay calibration / synthetic assumption** (spec §9.7 SA) |
| Option A as the draft default | **Fallback-conservative default, unapproved** (spec §9.9) |
| Caps, horizon, smallest event, replay budget, class-A membership | **Unsupported; needs decision** (spec §9.3, §9.9) |

### G.2 Superseded and stale text in the DRAFT (exposed, not edited)

1. **P0 base.** These lines say the Manager "will re-pin after independent #567 disposition":
   - spec lines 377 (§9.1 T-01), 400 (§9.2), 712 (§9.8) and 796, 805 (§9.10 #1);
   - plan 47, 209; tasks 145; requirements 33.

   They are **superseded by the #589 ruling** selecting `a7f41983`. `861b81d3` remains a valid historical citation. The quoted L1 tree id `fa25ae6e…` (spec §9.2) belongs to `861b81d3`. At `a7f41983` the `Tools/MultiRackSitkaLab` tree is `f212c8f6…` (observation), because the Host source changed.
2. **Review state.** spec line 349 (§8 gate table) and plan line 75 say #575 "awaits one focused non-author delta review". That review happened (#578), and the #581 correction followed.
3. **Option B wording.** spec §9.9 calls option B "diagnostic-mismatch" only. The Manager's #581 caution stands: class-B provenance and trust-table differences need separate compatibility checks.

### G.3 Inspected, and not done

**Inspected (GitHub contents via `git fetch`/`git show`):**
- All four DRAFT files at this branch;
- the #595 packet and issue;
- the #581 Manager closure;
- the #589 Manager ruling;
- `main` Decision Log D-082 and the current milestone;
- live `main` `Docs/Project/coordination/active-tasks.md` (no #595 row or lock found);
- issues #565 and #425 (titles and state only).

**Not inspected in full:** #566, #575, #578 and #581 result documents beyond their summaries in the DRAFT and the #581 closure. This dossier relies on those summaries for review outcomes.

**NOT RUN:** Unity, .NET, tests, builds, WSL, browser UI, any capture, any benchmark, any P0 L0–L4 check, any trust-table preparation. **Dashboard/Drive mirror: NOT VERIFIED.** Git is canonical.

**Not changed:** the four DRAFT files, P0, `main`, the Decision Log, the milestone, the coordination board, the #565 ledger and every issue other than #595 (except notification comments on #565 and #425).
