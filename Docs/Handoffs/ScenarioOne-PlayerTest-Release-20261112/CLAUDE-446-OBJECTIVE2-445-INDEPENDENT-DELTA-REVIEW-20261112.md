# #446 — Independent delta review of #445 Objective 2 whole-rack DRAFT corrections

**Verdict: NEEDS CHANGES (narrow, documentation-only) — for DRAFT revision quality only.**
This verdict is not product acceptance, spec approval, implementation authority or Unity evidence. OPEN-1..OPEN-6 stay **undecided**.

## 0. Review record

| Field | Value |
|---|---|
| Actual review actor | Claude **Opus 5.5** (`claude-opus-5-5`), Anthropic, via Claude Code (local CLI). This is the same model and role as the #442 reviewer. It is **not** the #445 author (OpenAI GPT-6). |
| Claim | [#446](https://github.com/jermur788/CCF/issues/446). [RUNNING] was registered before substantive work in [comment 6839630852](https://github.com/jermur788/CCF/issues/446#issuecomment-6839630852). |
| BASE (pinned #442 reviewed revision) | `7062566c3f2c16a1942ad91d64a1e962fb8dfe0c` |
| CONTEXT / reviewed HEAD (#445 final) | `568e63934cbf591b235a9f344c80e95c2f172899` (origin tip of `docs/objective2-whole-rack-442-revision-20261112-v1` matched at start) |
| Supporting read-only refs | `origin/main` decision log, D-065 spec and CCF Book reference; C-01 candidate `integration/scenario-one-c01-research-copy-candidate-20261112-v1` @ `a314d308315b5894263d30461a1e8b52ca0f3605`; #442 review @ `6da73e67` |
| Output branch | `docs/claude-446-objective2-delta-review-20261112-v1`, created from `568e6393` (vacant at start). It holds this one added Markdown file. |
| Own handoff commit | The GitHub commit that adds this file (recorded on #446 at handoff) |
| Worktree | **None.** No checkout was used. Content was read with `git show` / `git diff` on fetched objects, and this file was published through the GitHub Contents API. |
| Tools | git (fetch/show/diff/grep on remote refs), `gh api` (issues, contents), local text search. No web fetch: external sources were not re-fetched because their revised wording matches the #442 §2/§3 verification. |
| Tests run | **0.** No per-rack fixture was measured, no Unity was used and no save was touched. All RQ boxes remain unchecked, correctly. |
| #418 | Separate open gate. Not assessed and not inferred. |
| Draft status | **Remains DRAFT.** |

**Abbreviations.** EV = `Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md`. SP / PL / TK / RQ = `specs/scenario-one-objective2-whole-rack-sampling-v1/{spec,plan,tasks,checklists/requirements}.md`. RH = the #445 revision handoff `OBJECTIVE2-442-DRAFT-REVISION-HANDOFF-20261112.md`. All line numbers refer to `568e6393` unless stated.

## 1. Scope and ownership check (packet item 6) — PASS

- `git diff --name-status ff85a598..568e6393` (the worker's own range) shows exactly six Markdown paths: RH added, and EV/SP/PL/TK/RQ modified.
- `7062566c..ff85a598` adds only the manager packet `MANAGER-OBJECTIVE2-442-NEEDS-CHANGES-DOCS-REVISION-PACKET-20261112.md`. That packet predates the worker and is **not** a worker edit.
- There are no Source/Assets/Packages/ProjectSettings/Unity changes.
- The historical #441 handoff file is byte-identical between BASE and HEAD (empty diff). That satisfies m6's "do not rewrite history".

## 2. Fifteen-row disposition

| # | Finding | Changed location (HEAD `568e6393`) | Result | Remaining risk |
|---|---|---|---|---|
| **B1** | Released-only derivation, minimum count, aggregation, spatial BA cap | SP FR-04 L33; **FR-07 / 07a / 07b / 07c L73–79**; EV §5 "B1" L81, L83; PL Phase 3; TK T05/T06; RQ AC-T06 L29, **AC-E01b/c/d L50–52, AC-E06/E06b L41, L53** | **PARTIAL** | Requirements (a)–(d) of #442 are all met (see §3.1). One gap remains in the item the packet asked about explicitly: the **year and boundary of the "pre-intervention" baseline for destination tiles**, and which earlier removals count as "already executed in the intervention", are undefined (**R-1**). Rule-identity persistence tests are thin (r-3). |
| **B2** | Objectives 2–5 step table, old→new IDs, years, D-073/074/077, OPEN-6 | **SP FR-05 L35, FR-05A L37–54, FR-05B L56–63**, FR-13 L95; EV "B2" L89–93; PL §C2 L67–75; TK T07/T09; RQ **AC-Y01–Y06 L67–72** | **PARTIAL** | The table, the N/N+1/M chronology, the D-073/074/077 citations and an explicitly undecided OPEN-6 are present and mostly correct. Two items still need fixing: **R-2**, a factual error (the existing `extend-treatment` lesson is omitted and declared to have "no old equivalent"), and **R-3**, OPEN-6 B does not say what "refreshed source evidence" at year M means, so B is not yet an honest, implementable alternative. |
| **M1** | CCF Book pp. 40/75/87 and Research Index | EV §1 L25–26; SP §0 L12; FR-07c L79 | **PASS** | Checked against `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md` L48 (p. 40: 16–20 m, conditional), L174–176 (p. 75: sample plots, ≤20% pre-harvest BA, increment needs two inventories) and `RacksAndTreeMarking/...Source_Extraction_v1.md` L70 (p. 87: second thinning for conifers, any stage possible). Evidence tiers are honest. The ≤20% vs 40% distinction is a useful addition. |
| **M2** | Anchor ±12 m Low/Centre/High vs new finite tiles | SP FR-06 L67; EV §5 L77; PL Phase 3; TK T03/T05; RQ AC-G04 L16, AC-E02 L37, AC-E09 L44 | **PASS** | It is correctly stated that the old bands are not thirds. A versioned candidate (4 equal longitudinal tiles × 2 sides, masked) is described with Partial/Manual handling, and the old 15 IDs are protected. Tile count and minimum power remain calibration items, which is acceptable. |
| **M3** | MFM-01 "several dozen" prior; measure k/f/m_i before OPEN-5 | SP §6 L105; EV §6 L97; PL Phase 0; TK T03; RQ AC-U05 L91 | **PASS** | Matches D-065 spec L230. It is correctly labelled as an old-system order of magnitude. The text "direction not measured" is more cautious than #442's "probably larger", and that is acceptable. |
| **M4** | Jones 2026 attribution, stability and no-regeneration | EV §1 L24, §4 L68; PL boundary note | **PASS** | The revised claims match the abstract verified in #442 (BA ≈55 m²/ha, highest crown-thinned slenderness, no natural regeneration after 14 yr). The Q-tree claim is moved to the composite and marked unverified secondary. No wind coefficient is invented. Not re-fetched. |
| **M5** | Off-repo attachments: identity or downgrade | EV §1a L30–40; EV §1 L19, L23, L24; RQ G-01 L6 | **PASS** | Filenames, pages, bytes and SHA-256 are listed. The draft says explicitly that no stable shared location exists, and claims unique to attachments carry **UNVERIFIED SECONDARY — SOURCE ACCESS REQUIRED**. This reviewer could **not** check the hashes (the attachments are not in the repo). |
| **M6** | D-073 is a product decision, not a playtest lease | SP FR-05B L58, FR-13 L95, FR-15 L99; EV §7 L105; PL §A L7; RQ AC-P03 L78, AC-Y05 L71 | **PARTIAL (cosmetic)** | The main correction is done and D-073 is now cited under FR-13 with S1-C L1–L4. **Two leftover mislabels:** PL §B L20 "user **D-073 test candidate**" and EV §2 L50 "the user's live **D-073 test candidate**" (**R-4**). |
| **m1** | "on the third rack" | EV §2 L44 | **PASS** | "initial sample **on** the third confirmed rack, sorted by offset along the common normal". |
| **m2** | DAFM plot-frequency withdrawn; Annex 2 / Element 3 | EV §1 L17 | **PASS** | The frequency claim is withdrawn and the Element 3 CCF / Annex 4 conifer analogue is added with a scope limit. |
| **m3** | IPIN 13/06 identifier and cab caveat | EV §1 L20 | **PASS** | Correct identifier. The cab-visibility limitation is tied to keeping the player as the judge. |
| **m4** | Pommerening 2021 as Tamm Review | EV §1 L22 | **PASS** | Labelled as a review that includes the 26-site analysis, not a trial. |
| **m5** | Save schema not asserted as v24 | EV §7 L107; SP FR-14 L97; PL §D; TK T04 L16; RQ AC-T09 L32, AC-P02 L77 | **PASS** | No positive v24 claim remains. The only "v24" mentions are negations ("not a guessed v24"). |
| **m6** | #441 audit metadata as addendum | RH §4 | **PASS** | BASE `14e2ee64` appears in the #441 issue body. CONTEXT `39e2673f` and HEAD `7062566c` match the #442 header L11–12. `7062566c` adds exactly the #441 handoff file. The original file is unedited. |
| **m7** | D-066 latch one-way in authority list | SP §0 L10, FR-05A L54, FR-14 L97; RQ AC-T09, AC-P02, AC-Y06 | **PASS** | "**one-way** … (**never whole-rack credit**)" appears at the authority list. This is consistent with D-066 (decision-log L113: "once complete it is monotone"). |

**Totals:** 11 PASS, 3 PARTIAL, 1 cosmetic PARTIAL (M6). **0 FAIL.** Both blocking findings are substantially corrected but not fully closed.

## 3. Detail for the blocking items

### 3.1 B1 — what is now correct

- **(a) Source selection.** Only Crop Trees with positive, executed, provenance-valid release enter the statistic. Justified no-release trees are recorded and excluded (SP FR-07 L73; EV L81).
- **(b) Aggregation.** Minimum / lower quantile (with level) / median are presented as unratified alternatives with sensitivity replay, under a new rule version distinct from `MFM-V1-RULE-1` (FR-07b L77).
- **(c) Minimum count.** r≥3 is explicitly *calibration, not evidence*. Below it the result is "Insufficient source evidence", with no forced fell (FR-07a L75; AC-E01c L51).
- **(d) Spatial unit.** ≤40% is a candidate per new tile **and** per containing rack, with a separate neighbourhood severity check whose number is left open (FR-07c L79; AC-E06b L53).
- Shared competitors are counted once for BA and jobs but may contribute to several Shares (FR-07 L73; AC-E01b L50).
- OPEN-3 now covers both credit and derivation (FR-04 L33; §8 L125).
- AC-E01b/c/d and AC-E06b supply the negatives #442 asked for: mixed release, below threshold, Pending-only competitors, invalid or stale denominators, an over-cut tile under a safe mean, and unresolved #418.
- The source denominator year is well defined: the state before the annual Advance that resolves N (FR-05A L41; FR-07 L73).

### 3.2 R-1 (B1, required): destination-tile baseline year and "intervention" boundary

FR-07c L79 and EV L83 define the cap ratio against "the authenticated pre-intervention same-state baseline". The numerator includes removals "already executed in the intervention". Neither text says:

1. **Which year's state is the destination tile's denominator.** Destination proposals are derived and approved in N, but under OPEN-6 A they execute at N+1, after one more growth and mortality tick (and under B, at M).
2. **Where an "intervention" begins and ends for cumulative counting.** Under OPEN-6 A the campaign spans N and N+1. Manual felling done in a destination tile during N, or in an earlier year, could count or not count depending on that definition.
3. **Whether the cap is re-checked when the destination order resolves.** AC-E11 L46 re-validates at approval only. Mortality or storm loss between approval and execution can lower the denominator.

An implementer would have to invent all three. **Required fix:** define the destination baseline (for example, "eligible BA in the tile at the annual state in which the prescription is approved, re-evaluated at resolution") and define the intervention window for each OPEN-6 option. Add one RQ negative case where growth or mortality between approval and resolution moves a tile across the cap. Writing this as policy alternatives tied to OPEN-3/OPEN-6 is acceptable. A ratified number is **not** required.

### 3.3 R-2 (B2, required): existing `extend-treatment` lesson omitted from the old→new map

FR-05A L49–51 labels the four-target coverage and prescription/Work Plan stages "**New** … (no old step/credit equivalent)". That is wrong for the C-01 code the draft says it inspected:

- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs` L95 defines `ExtensionId = "extend-treatment"` (MFM-09, titled "6. Extend your treatment"). L103–105 inserts it into `PresentedLessons` between `response` and `recruitment`. L110 offers it only while `Current(f) == 5`. `LearningObjectivesView.Sequence.cs` L88–89 derives `ExtensionApplies` and `ExtensionApproved` (any approved prescription).
- This is also an internal contradiction in the draft. EV §2 L44 and SP §0 L14 already acknowledge the D-065 "extension after Lesson 5", yet the FR-05A table says no such stage exists.
- **Consequences for an implementer.** The map gives no instruction on whether `extend-treatment` is replaced, kept for old worlds, or renumbered. It does not address that `ExtensionApproved` is "any approved prescription", which a whole-rack prescription would satisfy. It also does not cover the displayed renumbering of `recruitment` / `establishment` / `transformation` (presented as 7–9 in extension worlds) once new stages are inserted. #442 B2(a) asked specifically for the "position for coverage/proposal review relative to the existing objectives".

**Required fix:** add an `extend-treatment` row (presentation ID, the `ExtensionApplies`/`ExtensionApproved` facts and `Current==5` gating) mapped to the proposed whole-rack stages. Say what happens to it in old Enlarged80 worlds versus new ones, and how lessons 6–8 are numbered and gated afterwards. This remains a proposal, so no decision is required.

**Related clarification (non-blocking, fold into the same edit).** The "Verified existing device-saved practice step IDs" column pairs topic-based Learning keys (`LearningObjectivesView.Curriculum.cs` topics 2–4, prefix `CCF.Learning.v1.`) with guided lessons. The keys exist, but the **pairing is interpretive** and should be labelled "related". Lesson 4 `habitat` is the sold-vs-deadwood lesson (`DeadwoodExecuted` gate). The table omits the `deadwood.*` keys and says nothing about whether the deadwood choice survives in whole-rack source work.

### 3.4 R-3 (B2, required): OPEN-6 B "refresh source evidence" is undefined

SP FR-05B L62, EV L93, PL §C2 L73 and AC-Y04 L70 say option B defers destination work to M and must "reassess source state/proof" or "refresh source and target evidence". It is not stated whether B:

- **B-i** re-uses the year-N derived rule, re-validating source fit and destination comparability at M under explicit staleness rules; or
- **B-ii** requires a new source demonstration at M.

B-ii is a second intervention in the source rack. FR-05B L63 says that would be the D-076 later cycle, which is "not the deferred second half of a first campaign". So with B-ii, option B collapses into "no destination work in the first loop". The two readings give the user materially different products.

Separately, D-077 makes Skip ineligible until the current teaching sequence is complete (decision-log L135; SP FR-05B L60 notes this). If the destination stage sits inside the teaching sequence, deferral to M under B means repeated Advance 1 Year presses, not Skip. B should state that consequence.

**Required fix:** describe B-i and B-ii (or choose one as the defined meaning of B) so that OPEN-6 is an honest choice. Do **not** select A or B.

### 3.5 R-4 (M6, cosmetic, required in the same pass)

Replace "D-073 test candidate" at PL L20 and EV L50 with the protected-session wording used elsewhere ("user-owned protected Scenario One Editor session / Manager #424 / historical #382").

## 4. Residual observations (non-blocking; may be folded into the same revision)

- **r-1 Existing completion predicate.** `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence` (C-01 L36–49) completes on succeeded work in any two distinct resolved years with an acknowledgement between them. Rack year + source N already satisfy it, so OPEN-6 A does not change scenario completion. D-077 P-1 keeps it unchanged. FR-05B should name this predicate and say that D-074's objective-level spacing is a separate, still-unspecified predicate on which OPEN-6 A's "one campaign" classification depends. TK's boundary note already lists "define D-074 achievement policy" as a later gate.
- **r-2 OPEN-6 A copy.** A should keep #442's note that player copy explains N/N+1 as one staged intervention, so the D-074 "consecutive years" message is not contradicted on screen. FR-05B L61 partly covers this.
- **r-3 Rule-identity persistence.** FR-07b L77 stores the aggregation method, denominator policy, source IDs/year and calibration signature. No RQ item tests that these, and an "Insufficient source evidence" result, round-trip identically through save/reload. AC-P01/P02 are generic. Add a clause to AC-E01d or AC-P02.
- **r-4 Hashes.** The §1a SHA-256 values are recorded but were not independently verified by this review.

## 5. Source precision and calibration (packet item 5)

- **Transfer statistic.** The sample-to-four calculation is specified well enough for an informed OPEN-3 choice: pool definition, three named aggregators, threshold semantics, de-duplication and failure states. No imaginary numbers are required.
- **BA cap.** The numerator and denominator are defined per tile and per rack with masking and de-duplication. Only the destination-year boundary (R-1) is unsound.
- **Empirical statements.** I found no ungrounded empirical statement in the changed text. Game numbers (30%, 40%, r≥3, 4-tile, 20/15/20, D-077 4-yr/5-tick) are consistently labelled GAMEPLAY CALIBRATION or candidate.
- **Test/checklist contradictions.** None between SP and RQ. AC-Y04 correctly says no pass can be claimed while OPEN-6 is unratified.

## 6. Unratified-but-documented vs unresolved corrections

- **Not drafting failures (properly documented product questions):** OPEN-1..OPEN-6 as set out in SP §8 L117–145 and RH §5. The #442 §4 reviewer leanings are reproduced as "NONE RATIFIED". This review neither adopts nor rejects any of them.
- **Unresolved corrections (drafting defects):** R-1 (B1 year/boundary), R-2 (B2 `extend-treatment` mapping), R-3 (OPEN-6 B definition), R-4 (two stale D-073 labels). All four are small, docs-only edits to SP/EV/PL/RQ. None needs new external research, fixture measurement or a product decision.

## 7. Routing

- **Manager #424:** record NEEDS CHANGES (narrow) and route one bounded docs-only fix of R-1..R-4 (plus, optionally, r-1..r-3) on the #445 branch or a successor. A short delta re-check of only those items by a non-author is enough; the M1–M5 and m1–m7 dispositions above need not be repeated.
- **Manager #425 / user:** no decision is needed yet. Once R-1..R-4 land, the draft is ready to route for OPEN-1..6 product decisions. That routing is **not** implementation acceptance.
- **Objective 1 #443** remains the implementation priority. This review touched no Objective 1, source, Unity, save, active-tasks, decision-log, D-065 or D-076 file.
- **User action now:** none. Dashboard/Drive sync: **NOT VERIFIED**.

Claim [#446](https://github.com/jermur788/CCF/issues/446) → HANDED-OFF on publication of this file.
