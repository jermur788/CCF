# #442 INDEPENDENT REVIEW — Objective 2 whole-rack research and four-file DRAFT specification

**2026-11-12 | VERDICT: NEEDS CHANGES (for DRAFT evidence/spec quality only)**
**Not:** spec approval, product ratification of OPEN-1..5, implementation authority, Unity verification or user acceptance.

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 ACCEPTANCE (DRAFT research + four-file spec quality); G3 note on calibration/measurement claims
Reviewed branch: docs/manager-424-full-rack-objective2-research-20261112-v1
Reviewed HEAD: 7062566c3f2c16a1942ad91d64a1e962fb8dfe0c
Context commit: 39e2673f0f5851da29be2de5c02bb08142abea9c (origin/main at review start)
SPEC (SPEC REQUIRED only):
- ID / status: specs/scenario-one-objective2-whole-rack-sampling-v1 — DRAFT
- Conformance: NOT APPLICABLE (no implementation); draft-vs-authority reconciliation reviewed below
Reasons: 2 blocking, 6 major, 7 minor findings (section 3)
Required fixes / remaining gates: section 5
Integration target: none — documentation review only
```

## 1. Reviewer identity, scope and method

- **Worker:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code CLI (T3 Code harness) on the user's HP Windows machine; GitHub via `gh` in WSL as `jermur788`. **Independent of the #441 author** (OpenAI GPT-6); did not write or edit any #441 output.
- **Registration:** [#442 RUNNING comment](https://github.com/jermur788/CCF/issues/442#issuecomment-6838053421) posted **before** substantive review.
- **Output branch:** `docs/claude-442-objective2-whole-rack-review-20261112-v1` (created from the reviewed HEAD; this file is the only addition). GitHub API commits only.
- **Worktree: NONE.** The user's local checkout was used read-only via `git fetch` + `git show` on remote refs; no checkout/branch switch/modification. **Unity/Editor: NOT USED**; no lease; D-073 user session untouched. **No** production source, #440 files, `active-tasks.md`, decision-log, D-065 or D-076 edits.
- **Read in full:** all seven #441 files at the reviewed HEAD; D-065 approved spec on main (sections listed below); D-076 DRAFT spec at `docs/mfm-multicycle-rack-sample-amendment-20261112-v1`; decision-log rows D-065, D-066, D-071, D-073, D-074, D-076, D-077; AgentWorkflow review/handoff sections; research-index.
- **Source inspected (static only)** on C-01 `integration/scenario-one-c01-research-copy-candidate-20261112-v1` @ `a314d308315b5894263d30461a1e8b52ca0f3605`: `ScenarioOneRackSampleGeometry.cs`, `ScenarioOneCoverage.cs`, `ScenarioOneRackBay.cs`, `ScenarioOnePrescription.cs`, `ScenarioOnePrescriptionCalibration.cs`, `ScenarioOneCropTreeCandidates.cs`, `ScenarioEcologicalSnapshot.cs` (existence also confirmed for `ScenarioOnePrescriptionApproval.cs`, `ScenarioOneManager.PrescriptionApproval.cs`, `ScenarioOneCoverageWorld.cs`).
- **External sources re-checked:** DAFM WIS 2023–27 PDF (text-extracted), FR IPIN 13/06 PDF (text-extracted), Pro Silva Ireland tree-marking page, and Crossref/OpenAlex metadata + abstracts for the three DOIs. Repo canonical research: `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md`, `Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md`, `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`.

Abbreviations: **EV** = [evidence report](https://github.com/jermur788/CCF/blob/7062566c3f2c16a1942ad91d64a1e962fb8dfe0c/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md), **SP** = [spec.md](https://github.com/jermur788/CCF/blob/7062566c3f2c16a1942ad91d64a1e962fb8dfe0c/specs/scenario-one-objective2-whole-rack-sampling-v1/spec.md), **PL** = [plan.md](https://github.com/jermur788/CCF/blob/7062566c3f2c16a1942ad91d64a1e962fb8dfe0c/specs/scenario-one-objective2-whole-rack-sampling-v1/plan.md), **TK** = [tasks.md](https://github.com/jermur788/CCF/blob/7062566c3f2c16a1942ad91d64a1e962fb8dfe0c/specs/scenario-one-objective2-whole-rack-sampling-v1/tasks.md), **RQ** = [checklists/requirements.md](https://github.com/jermur788/CCF/blob/7062566c3f2c16a1942ad91d64a1e962fb8dfe0c/specs/scenario-one-objective2-whole-rack-sampling-v1/checklists/requirements.md), **C01** = `https://github.com/jermur788/CCF/blob/a314d308315b5894263d30461a1e8b52ca0f3605/Assets/ForestPrototype/ScenarioOne/`. Line numbers refer to those exact commits.

## 2. What the draft gets right (independently confirmed)

The draft is careful, honest about what was not measured, and largely accurate. Confirmed facts:

| Draft claim | Check | Result |
|---|---|---|
| Old lesson: exactly 3 Crop Trees, 8 m spacing, 30% first release, 40% BA safeguard (EV §2 L33–34) | C01 `ScenarioOneRackBay.cs` L20, L24, L26, L28 | **CONFIRMED** |
| Teaching footprint on **third** rack, ±12 m along, 9 m half-width (EV L30) | `ScenarioOneRackSampleGeometry.cs` L9–12, L33; `ScenarioOneRackBay.cs` L35, L38; D-065 spec §4.1 L109–111 | **CONFIRMED** (wording "after the third rack" is imprecise, m1) |
| Coverage = five half-neighbour strips + explicit outer remainders; strips are full-length, not finite reach (EV L33) | Geometry L14–15, L21–22 ("one full-length strip per rack"), L273–296 (`Strip(lower, upper, ax, az, h)` clipped only to the stand square) | **CONFIRMED** |
| 15 subareas; only the teaching rack's Centre area is the source (EV L33) | `ScenarioOneCoverage.cs` L13–15, L256–260 | **CONFIRMED** — but see M2 for what "Centre" actually is |
| MFM03 calibration 20/15/20%, min 30 stems, 16 m candidate spacing, 0.1 m area step = one-fixture gameplay calibration (EV L61) | `ScenarioOnePrescriptionCalibration.cs` L4–6, L73–75 | **CONFIRMED** |
| Prescription derives only from an executed, qualifying old sample (EV L34) | `ScenarioOnePrescription.cs` L6–21, L67–105 | **CONFIRMED** |
| Candidates are spacing-only, carry no quality signal (EV L35) | `ScenarioOneCropTreeCandidates.cs` L6–9 | **CONFIRMED** |
| Ecological snapshot is stand/species aggregate, not per-rack (EV L35, L81) | `ScenarioEcologicalSnapshot.cs` L5–42 (no rack/area field) | **CONFIRMED** |
| D-065: after-Lesson-5 insertion, individual confirmation, no bulk (EV L30) | D-065 spec P1 L85, §8.2 L226–230 | **CONFIRMED** |
| D-076 DRAFT FR-R1 keeps the first fixed three-tree demonstration (SP L12) | D-076 spec L27 | **CONFIRMED** |
| DAFM WIS Annex 2 pp. 40–44 is **broadleaf** guidance; PCT = vigour, straightness, quality, freedom from disease, even distribution; PCTs and removals marked in thinning control plots; chainsaw vs harvester marking distinction (EV L17) | WIS contents ("Annex 2 40 Silvicultural Guidance for Tending and Thinning of Broadleaves"); PCT definition and control-plot/marking passages in extracted text | **CONFIRMED** (one sub-claim not found, m2) |
| Pro Silva page: mark categories, quality/function/neighbours/stability/light/biodiversity (EV L19) | Page fetched | **CONFIRMED**. The page itself does **not** mention sample racks; EV correctly attributes that to the project's own audit |
| FR IPIN 13/06: example area marked to show thinning intensity, later inspections; site/operator variation (EV L20) | Extracted text: "feller select basis, with an example area marked to indicate the desired thinning intensity, subsequent site inspections…"; "sample marked as a reference for the harvester operator"; cab-visibility caveat | **CONFIRMED** |
| Pommerening, Maleki & Haufe 2021: 26 British marteloscopes; frame-tree agreement higher than competitor agreement (EV L22) | Crossref/OpenAlex: *Tamm Review*, FEM 501; abstract states exactly this | **CONFIRMED** |
| Pommerening et al. 2018: marking agreement only slight-to-fair (EV L23) | PLOS ONE 13; 36 marteloscopes; Fleiss' kappa slight-to-fair; weaker in crown thinning | **CONFIRMED** |
| "Never divide 1,344 by five"; all per-rack metrics NOT MEASURED (EV L48) | No numeric per-rack value is asserted anywhere in the seven files | **CONFIRMED — good practice** |

There is **no unwarranted numeric precision**. The 8 m/8 m, 30%/40% and MFM03 numbers are labelled consistently as calibration. The A/B distinction and the "B NOT AUTHORISED" boundary agree with D-065 §8.2.

## 3. Findings

### Blocking (must be fixed before #425 can make informed OPEN decisions)

**B1 — The whole-rack derivation rule is undefined at the point where it matters most: how many released Crop Trees feed the release target, and how they are aggregated.**
The current prescription takes the release target from the player's **least** executed release, floored at 30%, and the per-area removal ceiling from the sample's own removed-BA share, capped at 40% (C01 `ScenarioOnePrescription.cs` L20–21, L104–105: `PlayerMinimumReleaseShare = lesson.MinExecuted`). With three trees that all had to be released for lesson credit, that minimum is well defined. The draft deliberately allows a whole-rack source to contain Crop Trees with **justified no release** (SP FR-04 L31; RQ AC-T08 L31; EV §4 L54). Then a minimum taken over all Crop Trees is 0 or undefined, and one weak release among many sets the target for four racks. SP FR-07 L43 says only "released Share target derived from sample under accepted minimum". OPEN-3 (SP L31, L89; handoff §5) is framed as a **lesson-credit** question. It is also the core **transfer-rule** question, and the spec does not say so.
*Required:* add an explicit derivation sub-requirement covering (a) which source Crop Trees enter the statistic (e.g. only those with executed release; no-release Crop Trees recorded but excluded); (b) the aggregation (minimum, lower quantile or median) as a **new rule version**, not `MFM-V1-RULE-1`; (c) the minimum count of released Crop Trees before a rule exists ("insufficient evidence" otherwise; SP mentions this only qualitatively); (d) the **spatial unit** of the 40% cap: whole W_i, subarea, or each Crop Tree's neighbourhood. A 40% cap averaged over a whole rack strip can hide heavy local removal. D-065 spec §5 item 7 (L140) defines it per coverage unit. Then re-frame OPEN-3 so it covers both credit and derivation.

**B2 — The first-loop sequence does not map Objectives 2–5, and ignores D-074/D-077, which govern exactly the timing the draft proposes.**
(a) In C01, Objectives 2–5 are **one practical lesson on one rack area** (`ScenarioOneRackBay.cs` L4: "Scenario One teaching Objectives 2-5 are one practical lesson on ONE rack…"; D-065 spec §6 L150–153 lists Lessons 2–5). The #441 packet RQ6 explicitly asks to "show what the lesson becomes in Objectives 2–5". SP FR-05 L33 gives only a prose sequence ("next guided phase…"), with no objective/step table, no mapping of old step IDs to new ones, and no stated position for coverage/proposal review relative to the existing objectives.
(b) **D-074** (CONFIRMED, decision-log row) says not to present major thinning/release in consecutive years as the exemplary CCF cycle. **D-077** (APPROVED) makes time-skip ineligible while Pending/Approved orders exist and keeps D-074's spaced-intervention objective separately required. Neither is cited anywhere in the seven files. The proposed flow is: execute source work through the ordinary Work Plan, review it, then approve destination prescriptions and a **second** Work Plan "in the same first complete management teaching loop" (SP FR-05 L33; EV §5 L69; handoff §4 L33–38). Ordinary Work Plan resolution goes through the annual simulation (D-010), and the implementer must verify the exact current execution timing. So destination treatment is likely to land one year after the source treatment. Either that is the consecutive-year pattern D-074 warns against, or the two years must be explicitly classed as **one** intervention campaign.
*Required:* add a numbered Objective 2..5 (or renumbered) step table with old→new stable step IDs and the year in which each execution happens. State how source + destination execution counts under D-074 and how D-077 skip eligibility behaves between them. Raise this as a **new product gate (OPEN-6)**, because it is a genuine product choice (see section 4).

### Major

**M1 — The canonical repo research the packet required is not used, and it contains the closest direct analogue.**
The packet (L27) says to use the research index, "source evidence packs, book and original papers". The EV bibliography (§1) cites none of the repo's canonical sources. In particular:
- **CCF Book p. 75** (`CCF_Book_Definitive_Reference.md` L176): measure pre-harvest **sample plots**, mark in and around them, measure the marked trees' combined basal area and compare the reduction against a target to adjust marking intensity. This is the most direct published analogue of "demonstrate in a sample, then calibrate intensity across the stand". It belongs in EV §1 and in FR-07's justification.
- **CCF Book p. 87 / Pro Silva marking standard** (marking extraction L70): quality trees are "generally identified early — first thinning for broadleaves and **second for conifers**". This bears directly on whether a *first-cycle* whole-rack Sitka Crop Tree selection is realistic timing, or a teaching abstraction that should be labelled as such.
- **CCF Book p. 40** (L48): 16–20 m between racks in early transformation, context-dependent. That implies roughly 8–10 m served on each side, consistent with the 9 m teaching half-width and the D-071 "16–20 m recommendation". It is useful evidence for OPEN-1.
- **Research index, D-065 section:** the existing wording boundaries ("evidence-informed", "operator selection" not an Irish term, cab-visibility caution). EV restates them but does not cite their canonical home.

**M2 — Reusing the "Low/Centre/High" subareas for within-rack heterogeneity is unsound without re-segmentation, and the draft does not say so.**
The 15 areas are **not** thirds of each rack. Each strip is split at the **old teaching anchor ±12 m** (`ScenarioOneCoverage.cs` L13–15, L455; `ScenarioOnePrescriptionCalibration.cs` L30–32 and L73–74, where `SegmentHalfLengthM` is set to `ScenarioOneRackBay.HalfLengthM`). So every rack's "Centre" is a 24 m band aligned to rack 3's anchor, and Low/High are whatever length remains. SP FR-06 L37, PL Phase 3 L47, RQ AC-E02/AC-G04 and EV §5 L63 propose reusing these for heterogeneity detection, adding only "(or revised v2 area segmentation)". *Required:* state that whole-rack mode needs a **new segmentation rule** (e.g. equal along-rack lengths or a fixed-length raster on each finite W_i) as a new calibration version. Make AC-E02/AC-E09 test against that rule, not the anchor-tied thirds.

**M3 — Prior workload evidence is ignored, so the A-vs-B choice (OPEN-5) is presented as unknowable.**
D-065 spec §8.2 L230 records that MFM-01 estimated "on the order of **several dozen** candidates" at default scale (16 m spacing). It also says that if user testing finds this tedious, the project stops for a product decision. EV §6 and SP §6 label every k, f, m_i as NOT MEASURED. That is correct for the new geometry, but the existing order-of-magnitude estimate should be cited as the prior. Note the probable direction too: whole-rack source plus full-length destinations should be **larger** than MFM-01's estimate. TK T03 should measure it first, because OPEN-5 depends on it.

**M4 — Jones et al. (2026) is partly misattributed and its most relevant results are unused.**
The DOI resolves to *Transformation of Sitka spruce to continuous cover forestry: Results of a thinning trial* (FEM 620; Jones, Short, Vítková, Wilson, Ní Dhubháin). Its abstract reports crown thinning vs GDT vs low thinning in two Irish Sitka stands, 2010–2023: basal area recovers rapidly to about 55 m²/ha, crown-thinned trees have the **highest slenderness** (a stability concern), and there is **no natural regeneration after 14 years**. EV L24 cites it for "Irish quality/Q-tree selection and crown release are documented". The Q-tree claim should be attributed to the supplied composite only. The draft should also use Jones for (a) a stated **stability caveat**: whole-rack crown release can raise slenderness, an unmodelled consequence the game must not hide or invent; (b) "treatment does not imply regeneration" in player copy; (c) supporting D-076's refreshed-evidence-per-cycle stance, since BA rebounds quickly.

**M5 — Three cited "supplied" evidence documents cannot be verified from the repository.**
*Six Papers Relevance Evidence Extraction*, *Delegated Marking and Operator Selection* audit and *CCF Practice Economics and Transformation Evidence Composite* (EV L19, L23, L24) are not under `Docs/` or `specs/` on main. The research index says the D-065 audit sources "are not reproduced here". RQ G-01 asks a reviewer to confirm "six-paper evidence" is cited correctly, which no repository reviewer can do. *Required:* give their storage location and hash (Files/Drive path), or downgrade the claims that rest only on them to "unverified secondary".

**M6 — The authority label "D-073 protected personal playtest" is wrong in substance.**
D-073 (decision-log) is a **product priority**: multi-year decision → outcome review, with "Retain — why?" deferred. It is not a playtest lease. SP FR-15 L63, PL L10/L20 and the handoff L7 use "D-073" as shorthand for the user's protected Editor session or candidate. Meanwhile D-073's real content is directly relevant to FR-13's annual review of all treated racks, and is not cited for it. *Required:* name the protected session by its claim/issue (e.g. the #382 / Manager #424 user-test lane). Cite D-073 (and S1-C L1–L4 provenance) under FR-13.

### Minor

- **m1** EV §2 L30: "initial sample after the **third** confirmed rack" should read "**on** the third rack (ordered by offset along the common normal)" (D-065 §4.1 L109).
- **m2** EV L17: "control plots roughly one per homogeneous unit" was **not found** in my extracted WIS text (extraction may be incomplete). Cite the exact page/table, or remove it. WIS also has **Element 3 CCF** (§9: "tree marking for thinning", "felling of additional trees to release frame trees"; Annex 4 CCF Conversion Management Plan). That section concerns conifer transformation and is a better analogue for Sitka than broadleaf Annex 2; consider citing it alongside.
- **m3** Cite IPIN 13/06 by its real identifier (Forestry Commission *Internal Project Information Note 13/06*), and note its explicit caveat that cab visibility makes feller-select crown assessment unsuitable at some sites. That caveat supports keeping the player as the 3D judge in mode A.
- **m4** Label Pommerening et al. 2021 as a *Tamm Review* that includes the marteloscope analysis, so readers do not take it for a dedicated field trial.
- **m5** "v24" teaching latch (EV L83; RQ AC-P02): the schema version was not verified by #441 or this review. Write "the D-066 teaching-completion latch (version to be verified on the implementation HEAD)".
- **m6** The #441 handoff does not use the AgentWorkflow *Standard worker handoff* fields (Base / Context commit / HEAD / GITHUB HANDOFF COMMIT). The start head is only "recorded in that issue". Add the exact SHAs for auditability.
- **m7** SP §0 L10 lists D-066 as "history latch" without saying it must stay one-way and not become whole-rack credit. FR-14 L61 says this, but repeat it at the authority list, because it is a protected-save constraint.

## 4. OPEN-1..5 (plus new OPEN-6): reviewer recommendations for #425/user

These are recommendations. **None is ratified by this review.**

| Gate | Draft recommendation | Reviewer recommendation |
|---|---|---|
| **OPEN-1** finite reach / outer strips | Finite centreline reach, no off-end extrapolation, outer leftovers manual | **Agree.** Also adopt the existing C01 rule that an outer strip's free edge mirrors its interior half-gap (Geometry L21) instead of inventing a new outer rule. Cap each W_i longitudinally at the confirmed entrance→far segment. Anything beyond is an explicit remainder (Manual). Cite CCF Book p. 40 (16–20 m spacing) as context, not as a rule. |
| **OPEN-2** source choice | Guided central default; changed only when comparison motivates it | **Prefer fixed third (central) rack for the first cycle in V1**, with player-chosen sources deferred to D-076 later cycles. This keeps D-065 continuity, makes old-save handling simpler and removes one versioned code path. If #425 wants choice in cycle 1, require the comparison display before selection. |
| **OPEN-3** 30% per tree / no-release | No forced count; keep protections | **Agree, and widen the question (B1):** 30% is a per-tree target **only for Crop Trees the player chooses to release**. No-release Crop Trees are valid and recorded but excluded from the transfer statistic. A rule exists only above a minimum number of released Crop Trees. Keep the 40% hard cap per coverage unit **and** check it per Crop Tree neighbourhood. |
| **OPEN-4** execute source before prescription | Yes; earlier view advisory only | **Agree.** This matches D-065 (`ScenarioOnePrescription.cs` L67–68) and the CCF Book p. 75 practice (marked sample → measured intensity). |
| **OPEN-5** individual confirmation vs delegated | Keep individual confirmation (A) | **Agree**, conditioned on T03 measuring the real queue length first (M3). If the measured workload is large, return to the user with the number before considering B. Do not pre-build B. |
| **OPEN-6 (new)** first-loop timing under D-074/D-077 | Not raised | **Product choice required:** (a) classify source execution (year N) + destination execution (year N+1) as **one** intervention campaign for D-074 purposes; or (b) destination treatment waits for the next suggested management year (D-077's 4-year calibration), with proposals re-derived then. Reviewer leans to **(a)** for teaching continuity, with copy that explains it is one staged intervention. Either way it must be written into the step table (B2). |

## 5. Required changes for re-review (checklist for the #441 successor author)

1. B1: add an FR-07 derivation sub-requirement (selection, aggregation, minimum released count, spatial unit of 40% cap); re-frame OPEN-3; add matching RQ tests (e.g. AC-E01b: a source with mixed release/no-release Crop Trees derives a rule from released trees only; AC-E01c: below the minimum count, the result is "insufficient evidence").
2. B2: add the Objective step table with old→new stable IDs and execution years; cite D-073, D-074 and D-077; add OPEN-6; add RQ tests for skip eligibility between source and destination work.
3. M1: add the CCF Book pp. 40, 75, 87 and the research-index D-065 section to EV §1 with evidence classes.
4. M2: specify the new segmentation rule; amend FR-06, PL Phase 3, AC-E02, AC-E09 and AC-G04.
5. M3: cite the D-065 §8.2 MFM-01 estimate; make T03 measure k, f and m_i before OPEN-5 is decided.
6. M4: re-attribute the Jones 2026 claim; add the stability and no-regeneration caveats.
7. M5: give locations/hashes for the three supplied documents, or downgrade the claims that depend on them.
8. M6: fix the D-073 label.
9. Minor m1–m7.

Re-review can be a **delta review** of these items only. The confirmed material in section 2 does not need re-checking unless it changes.

## 6. Verification statement

- **Verified by this review:** the source and authority facts in section 2 (static read of exact commits); external citations via fetched pages, extracted PDF text and DOI metadata/abstracts.
- **Not verified:** any Unity/runtime behaviour, any per-rack quantity (still NOT MEASURED, as the draft states), the current save schema version, the three "supplied" off-repo documents, and the WIS "one control plot per homogeneous unit" sub-claim.
- **#418** authentic prescription-resolution provenance: **remains an open, independent gate**. Nothing here bears on it, and the draft treats it correctly (SP FR-16 L65; RQ G-04).
- **No test was run.** Dashboard/Drive mirror synchronisation: **NOT VERIFIED**.

## 7. Routing

- **Manager #424:** record the NEEDS CHANGES verdict, then route one bounded docs-only revision of the #441 outputs against section 5 (any capable author; a delta re-review should come from a non-author).
- **Manager #425 / user:** no decision is needed until the revision lands. Then OPEN-1..6 as in section 4.
- **#440:** untouched; this review introduces no UI ownership change.
- **User action now:** none.

Claim: [#442](https://github.com/jermur788/CCF/issues/442) → HANDED-OFF on publication of this file.
