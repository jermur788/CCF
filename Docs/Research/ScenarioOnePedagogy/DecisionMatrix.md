# Decision matrix — accepted vs proposed (Workstream G1, G2 #5)

**Status:** classification for Manager/user review. This file **upgrades nothing** to a decision. Every item marked PRODUCT DECISION REQUIRED returns to the user.

Classes:

- **ALREADY ACCEPTED** — in the Decision Log or verified implementation.
- **SUPPORTED DIRECTION** — consistent with accepted decisions and evidence, but not yet decided in this form.
- **NEW PROPOSAL** — from this work; can be accepted or rejected without breaking anything accepted.
- **PRODUCT DECISION REQUIRED** — a genuine choice between alternatives with trade-offs.
- **FUTURE / DEFERRED** — consciously not now.

## 1. Matrix

| # | Item | Class | Basis | Recommendation |
|---|---|---|---|---|
| 1 | CCF central to Scenario One; decide → advance → walk → understand | ALREADY ACCEPTED | D-003, D-004 | — |
| 2 | Fell / Crop Tree exclusive marks; Crop Tree = retained future timber | ALREADY ACCEPTED | D-013, D-014 | Keep. Crop Tree ≈ Irish Q-tree [PRAC §11.6] |
| 3 | Work Plan reviews and approves; spatial decisions are made walking | ALREADY ACCEPTED | D-010 | Residual block must stay a *review*, not a designer |
| 4 | Map is diagnosis/navigation only | ALREADY ACCEPTED (implemented) | `StandMapView`; menu teaching | Keep |
| 5 | No universal correct prescription | ALREADY ACCEPTED | Game brief "Do not imply one universally correct forestry prescription" | Basis for "no universal score" |
| 6 | Presentation derives from authoritative state | ALREADY ACCEPTED | D-020 | Basis for the honest clearance/wind wording |
| 7 | Positive selection as the teaching order ("start with what you keep") | SUPPORTED DIRECTION | D-013/D-014 + [PRAC §11.5] | Accept as tutorial principle |
| 8 | Tutorial/learning progress per forest vs per device | **PRODUCT DECISION REQUIRED** | current: per device (PlayerPrefs), by design | **Per forest** for stage progress (save field, after Sol); keep per device for "help seen" |
| 9 | Clearance preview always shown on ground aim | **PRODUCT DECISION REQUIRED** | current behaviour; teaching risk (audit) | Show only after U is pressed (clearance mode). Update ClearanceVerification in the same packet |
| 10 | Honest clearance wording (no seedling-benefit claim) | NEW PROPOSAL | D-020, D-024 still direction, D-047 open item | Accept (copy only) |
| 11 | Ground "Why" names seed limitation | NEW PROPOSAL | derivable seed rain | Accept (presentation) |
| 12 | Crop-Tree release readout separate from stand average | NEW PROPOSAL | harness T1 vs T2 inversion | Accept (Packet 2) |
| 13 | Neighbour-relationship line in Tree Inspection | NEW PROPOSAL | derivable Hegyi term | Accept (Packet 2) |
| 14 | Highlight of a Crop Tree's strongest neighbours in the world | **PRODUCT DECISION REQUIRED** | borderline prescriptive | Show all neighbours shaded by strength, or not at all. Defer until 13 is playtested |
| 15 | Residual-stand "What you are leaving" block in Work Plan | NEW PROPOSAL | D-010 compatible | Accept (Packet 3), compact |
| 16 | Marteloscope / practice mode | **PRODUCT DECISION REQUIRED** | `MarteloscopeDecision.md` | Reusable practice mode (sandbox), Year-0 copy first |
| 17 | Bio Tree designation | **PRODUCT DECISION REQUIRED** | `BioTreeDecision.md` | Not now; revisit with habitat-tree state |
| 18 | Forest Diary Phase 1 (derived, no save) | NEW PROPOSAL | `MonitoringDecision.md` | Accept |
| 19 | Permanent sample plots (save) | **PRODUCT DECISION REQUIRED** | `PermanentPlotDecision.md` | Phase 2, after Sol's save work |
| 20 | Tutorial assessment: multi-dimensional, no score | **PRODUCT DECISION REQUIRED** (§2.5) | research + game brief | Multi-dimensional, no score |
| 21 | Scenario objectives redesign (competence-based; second intervention; planting framing) | **PRODUCT DECISION REQUIRED** | audit A2 | See §2.6. Changes the completion anchor; needs explicit authorisation |
| 22 | Mandatory planting of both broadleaves | **PRODUCT DECISION REQUIRED** | `RegenerationTeaching.md` §3 | Keep for now and explain why (copy). Revisit with #21 |
| 23 | Minimum years between interventions for the tutorial (N) | **PRODUCT DECISION REQUIRED** (calibration [D]) | `RepeatedInterventionDesign.md` | N = 5 |
| 24 | "No thinning this year" explicit choice | NEW PROPOSAL (save: event) | agency, [PRAC §2.10] | Accept with #8 |
| 25 | Intervention purpose note (release / harvest / regeneration / improvement) | NEW PROPOSAL (save: event field) | [PRAC §13.1] | Later, with the diary |
| 26 | Wind label recalibration (all trees "high" now) | **PRODUCT DECISION REQUIRED** (calibration [D]) | harness: 336/336 "high" | Recalibrate after Sol's height model; until then, hide the band or show H/D instead |
| 27 | Competition label discrimination (328/336 "crowded") | **PRODUCT DECISION REQUIRED** (calibration [D]) | harness | Prefer the numeric "% growth withheld" sentence; reconsider the thresholds |
| 28 | Contractor minimum vs bounded property (every early thinning loses ≥ €800) | **PRODUCT DECISION REQUIRED** (economy owner) | harness: whole-stand notional value €1,702 < €2,500 | Not a pedagogy decision. If kept, explain it in copy |
| 29 | Stem form / quality state (wolf trees, form-based Crop Tree choice) | FUTURE / DEFERRED | UNSUPPORTED now | Do not fake it; one honest sentence |
| 30 | Causal understorey competition (clearance benefit) | FUTURE / DEFERRED (D-024 current direction; D-047 open item) | — | Lesson becomes possible when implemented |
| 31 | Windthrow, harvest damage, contractor quality | FUTURE / DEFERRED (D-044) | [PRAC §2.9, §15.3] | — |
| 32 | Rack / extraction-route infrastructure | FUTURE / DEFERRED | [PRAC §11.3, §12.3] | Stage 2 land management |
| 33 | Gate run-mode manifest (interactive vs batch) | NEW PROPOSAL (process) | Workstream F | Accept |
| 34 | Pro Silva synthesis added to research index as supporting evidence | NEW PROPOSAL (context) | `CanonicalUpdateProposal.md` | Accept |

## 2. Decision papers

Papers 1–4 are separate files: `MarteloscopeDecision.md`, `BioTreeDecision.md`, `MonitoringDecision.md`, `ResidualStandDecision.md`.

### 2.5 Decision paper 5 — Tutorial scoring

**Question:** should the tutorial or practice mode give a score?

| Option | For | Against |
|---|---|---|
| A. Universal score (e.g. "CCF score 73 %") | Familiar game feedback; easy to compare | Contradicts the evidence that several stand structures are credible [PRAC §2.2, §12.1, §19 "do not make a marteloscope score the one correct answer"]. Contradicts the game brief. Invites optimisation of a hidden formula |
| B. Pass/fail per lesson | Clear progression | Requires "correct" answers that do not exist for most marking decisions |
| **C. Multi-dimensional review + observed-behaviour lesson completion** | Matches practitioner marteloscopes (economic/silvicultural/ecological criteria shown separately [PRAC Luxembourg, §2.5]); teaches trade-offs; lesson steps complete on *doing*, not on *being right* | Less "gamey"; needs good copy |

**Recommendation: C.** Lessons complete when the player performs and reviews the action. Reviews show several dimensions, with no total. A plan is called *mistaken* only against the player's own stated purpose. **PRODUCT DECISION REQUIRED** (the user owns "how the game feels").

### 2.6 Objective redesign (supporting note for #21)

The current eight objectives are counters, and four are met by inaction (`ObjectiveGraph.md`). A redesign is a product decision with anchor consequences:

- `ScenarioOneCompletionVerification` asserts the completion year and outcome, and its hash includes management history. Any objective change changes the **completion anchor** (`6F84AF319D301F87` for model 1) and must be explicitly authorised (AGENTS.md: must-not-change anchors).
- Candidate competence-based objectives (proposal only): ≥2 interventions separated by ≥N years; Crop Trees retained and released at least once; canopy above threshold throughout; a species not supplied by the original plantation established (replaces "plant Beech and Oak"); deadwood retained.
- **Recommendation:** do not change objectives in the first packets. Introduce the arc and feedback first (Packets 1–3, 5), playtest, then decide.
