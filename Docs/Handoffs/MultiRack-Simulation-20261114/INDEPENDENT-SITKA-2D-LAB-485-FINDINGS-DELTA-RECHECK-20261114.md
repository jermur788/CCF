# Independent delta re-check — Sitka 2D lab DRAFT: #485 findings D-01 to D-18

- **Re-check task:** [#488](https://github.com/jermur788/CCF/issues/488)
- **Date:** 14 November 2026
- **Reviewer (actual worker):** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code session in T3 Code.
  - I was the original [#485 reviewer](https://github.com/jermur788/CCF/blob/616bd5376986b668589553d4971de7ac2888df41/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-V1-DRAFT-SPEC-REVIEW-20261114.md), which the packet explicitly permits.
  - I am a non-author of both the #483 draft and the #486 repair (both OpenAI GPT-6).
  - **Worktree NONE.** I used GitHub and git reads only, plus one docs-only write. No code, tests, Unity, host lease or data access.
  - Registration: [#488 comment](https://github.com/jermur788/CCF/issues/488#issuecomment-6874624403).
- **Binding packet:** [MANAGER-SITKA-2D-LAB-485-FINDINGS-REPAIR-AND-INDEPENDENT-DELTA-RECHECK-20261114.md](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-486-delta-recheck-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-2D-LAB-485-FINDINGS-REPAIR-AND-INDEPENDENT-DELTA-RECHECK-20261114.md) (packet branch HEAD `dc182cc5`).
- **Reviewer branch:** `docs/multirack-sitka-lab-486-delta-review-result-20261114-v1`, starting HEAD `fc18190b976f1363f1b5d1b0881810db2f8c6e39` (= `origin/main`, 0 ahead / 0 changed at registration).

## Verdict

**APPROVED AS REVISED DRAFT QUALITY.**

- **All 18 findings PASS.** Each of the eight MEDIUM findings (D-01 to D-08) and ten LOW findings (D-09 to D-18) is genuinely repaired in the actual revised text, and the four files agree with each other.
- **No new blocking or MEDIUM-level regression was found.**
- I list seven **non-blocking LOW clean-ups** (N-01 to N-07) in §4. They are wording or naming leftovers and one under-stated replay rule. None of them would force unsafe implementation guesswork, because the spec already contains the guarding rule in each case. They can be folded into the approval record or a small edit without another independent review.

**This is a DRAFT-quality verdict only. It is not formal P0 APPROVED implementation authority.**

- The spec remains `DRAFT`.
- Formal P0 APPROVED status, and the resolution of **OPEN-L1, OPEN-L2 and OPEN-L3**, belong to **the user or the designated Overall Manager acting for the user**.
- **OPEN-L4 and OPEN-L5 remain deferred and are not P0.**
- **NO AUTOMATIC P0, P1 OR UNITY CODE RELEASE.**
- P0 remains synthetic, flat, Sitka-only, engineering-only and **NO BIOLOGICAL GROWTH**.
- No Irish marteloscope import is possible while #479 raw access is blocked.
- P1 annual ecology and the full 2D/3D multi-rack game remain later specs.

**Recommendation to the Manager or user:** the DRAFT is now of sufficient quality to be considered for formal P0 approval. If it is approved, issue a new local implementation packet and claim only after that authorisation.

## 1. Versions inspected and scope check

| Item | Ref | Check |
|---|---|---|
| Original reviewed author candidate | `754a467c220bde0beef7998256baae785b06fb07` | baseline of #485 |
| Revised author branch `docs/multirack-sitka-lab-485-review-repairs-20261114-v1` | **pinned HEAD `e085ede824872a6a57abf5856be09ffc41e6ce60`** (= HEAD registered in the packet) | 6 commits over `754a467c` (`ee7ed17b`, `93e3ebd5`, `9451a0e2`, `8d67a0f8`, `81efaeb3`, `e085ede8`). `git diff --name-status 754a467c e085ede8` shows **exactly 4 `M` paths** under `specs/multirack-sitka-2d-reference-lab-v1/` and 0 other paths. HEAD was unchanged at publication. |
| `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` | @e085ede8 | all read in full (149 / 62 / 43 / 65 lines) |
| Status line | `spec.md` L4 | still **DRAFT — NOT APPROVED; IMPLEMENTATION NOT YET AUTHORISED** |

No canonical sources were re-read beyond what #485 already pinned at `fc18190b` (main is unchanged). The only main-side fact I re-used is the AgentWorkflow approval authority (user = product owner; Overall Manager approves specs).

## 2. D-01 to D-18 disposition matrix

| # | Sev. | Result | Evidence in the revised text (file §/line @e085ede8) | Notes |
|---|---|---|---|---|
| **D-01** | MED | **PASS** | `spec.md` A5 L52–67: separate `markState` (UNMARKED/PLAYER_MARKED), `planState` (8 states) and biological `TreeState.status`; 9-row transition table with guards and "Forest effect"; "at most one open plan"; sorted IDs stored; **"P0 does not support MANUAL_REVIEW"**; plan ID lists cannot be edited (cancel or reject, then a new plan ID); terminal non-executed plans release reservations; "Undo" is defined. Echoed in `plan.md` G3, `tasks.md` LAB-03, `requirements.md` REQ-D01/D05. | Every transition has one source state, one action and one guard. Cancel and reject have no forest effect. Mark state and plan state are not conflated. LOW: N-05 naming. |
| **D-02** | MED | **PASS** | `spec.md` A5 L69 ("strict all-or-nothing … never silently filters or fells a subset … `PlanExecutionRejected` audit-only"); §4 L94; AC-05 ("all and only"); AC-06; `plan.md` G3; `tasks.md` LAB-03 and checklist L24; `requirements.md` REQ-D03. | All four files agree. A rejection has equal prev/post snapshot hashes and leaves tree status, metrics and the successful-work ledger unchanged (L71). |
| **D-03** | MED | **PASS** | `spec.md` A5 L60–65, L71: approval captures `baseForestRevision` + `sessionEpoch`; another execution or a calendar step increments the revision and makes approved/pending plans STALE; reset/load starts a new epoch; no in-place re-approval; both tokens compared before mutation. A9 L82: epoch is runtime-only, and saved approved/pending plans **become STALE with a separate event on activation**. Tests: `requirements.md` REQ-D04 (revision/epoch mismatch, calendar advance, load-reset), REQ-U07; `plan.md` G3; `tasks.md` checklist L25/L29. | All four triggers (execution, step, reset, load) are covered. The load rule is explicit and unconditional, so it does not depend on epoch-value comparison alone, which closes the token-bypass case. LOW: N-02 (epoch value generation and replay source should be stated). |
| **D-04** | MED | **PASS** | `spec.md` A3 L44: checked-in hand-audited fixtures, **no PRNG in P0**, warning about `System.Random`. A9 L82: UTF-8 no BOM, LF, compact; fixed root, snapshot, vertex, tree, plan and event property orders; ordinal sorting; integer-only persistence (no floats); SHA-256 lowercase over canonical `snapshot` bytes; `eventSeq` + prev/post hashes; replay re-derives **every** post hash; byte equality scoped to the same fixture, schema, pinned runtime and logical snapshot. AC-03 separates the clean no-plan reload byte-equality case from intentional stale-plan transitions. Echoed in `plan.md` G2, `tasks.md` LAB-02 and checklist L20, `requirements.md` REQ-C07/D06. | Implementable. Persisting only integers removes the float-formatting problem. LOW: N-01 (state that every snapshot-changing action is an event), N-06 (ID character set and escaping). |
| **D-05** | MED | **PASS** | `spec.md` A4 L48–50: boundary-inclusive; any outside stem rejects the fixture; **no edge exception ledger**; `adultThresholdMm` declared per fixture; living `dbhMm >= threshold` are eligible; below-threshold stems displayed, excluded from N, BA, N/ha, BA/ha and Dq, and non-selectable for work. Conversions: `dbhCm=dbhMm/10`, `areaM2=areaMm2/1,000,000`, `areaHa=areaM2/10,000` (verified correct). AC-04; `requirements.md` REQ-C02/C04/C06; `tasks.md` checklist L22. | Consistent across files. Integer-mm geometry makes the on-segment test exact. |
| **D-06** | MED | **PASS** | `spec.md` A6 L75: fixed gitignored `Tools/MultiRackSitkaLab/.lab-data/`; `.sitkalab.json` chosen only from a lab-generated list; "no free-path picker/API"; rejects absolute paths, traversal, symlink escape and outside canonical paths **before opening bytes**; requires an exact `format`/`labSchemaVersion`/`sourceClass` header before parsing forest or events; rejects `ForestSaveData` shapes. A9 L84: binds 127.0.0.1 only, restrictive CORS/Host/Origin, no network or telemetry, explicit negative tests. "No CCF save impact" section L86. `plan.md` package tree adds `.lab-data/`; `requirements.md` REQ-U06/U08; AC-07. | Negative cases are explicit and there is no generic file picker. LOW: N-03 (AC-07 "fail before file read" is impossible for CCF-shaped *content* inside the root). |
| **D-07** | MED | **PASS** | `spec.md` A4 L48: single simple ring, no holes, ≥3 distinct non-collinear integer-mm vertices, non-zero area, no self-intersection or self-touching of non-adjacent edges, either winding, intersection check **before** area, overflow-safe arithmetic, absolute shoelace; bow-tie rejects the fixture. AC-04/06; `requirements.md` REQ-C01; `plan.md` G2/G5; `tasks.md` LAB-02. | Overflow safety matters: 100 m = 10⁵ mm, so cross products reach about 10¹⁰, which is beyond Int32. The spec requires checked arithmetic. |
| **D-08** | MED | **PASS** | `spec.md` §7 L129–137: decision table with an owner, route and timing for each OPEN item; **OPEN-L1/L2/L3 MUST RESOLVE BEFORE P0 APPROVED**; L4/L5 **DEFERRED, NOT P0** with owners. Approval: user, or designated Overall Manager acting for the user, per AgentWorkflow; **the draft author must not self-approve**; reviewer verdict ≠ APPROVED; the current authorised board writer registers lanes; explicitly no assumption that #424 or a past one-off exception grants approval. `plan.md` G0; `tasks.md` LAB-00 and Dispatch rules; `requirements.md` REQ-P06. | No product decision is ratified unilaterally. Recommendations are labelled as recommendations. |
| **D-09** | LOW | **PASS** | `spec.md` A1/A3 `ageAtInventoryYears` immutable; A6 L73 button text "Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)", DBH, height, age and mortality unchanged, invalidates plans; AC-09; `requirements.md` REQ-U07; `tasks.md` checklist L29. | LOW leftover: §2 L26 "mock calendar years" and §8 L144 "annual history" (N-04). |
| **D-10** | LOW | **PASS** | `spec.md` A2 L42, A4 L50, A9 L82, §4 L93, AC-08 ("all displayed metrics equal Core API values"); `plan.md` G4/G5; `requirements.md` REQ-U01/U03/U05. | The UI/Core equality test is explicit. |
| **D-11** | LOW | **PASS** | `spec.md` A3 L46 (all P0 values **ENGINEERING TEST FIXTURE**, explicitly not "GAMEPLAY CALIBRATION"; the "unless anchored" escape is removed); A8 title "Synthetic Sitka test stand (flat)"; Evidence classification L88; `requirements.md` REQ-P02/E04. | No typical-Irish values are asserted. LOW leftovers (N-04): §2 L26 "synthetic flat Irish Sitka research stand" and "deterministic generated"; §2 L34 "synthetic game parameters"; REQ-P02 "synthetically generated". |
| **D-12** | LOW | **PASS** | `spec.md` A4 L50 "No P0 top-height output; P1 must define its sampling rule from relevant GROWFOR evidence." | The checklist has no P0 top-height item. It only mentions top height under P1 (REQ-M05), which is consistent. Optional: add one negative checklist line (N-07). |
| **D-13** | LOW | **PASS** | `spec.md` §4 L93 "P0 has no cash or costs"; A5 L69 "no cash/economics"; AC-05 no longer mentions costs. | No test depends on economics. |
| **D-14** | LOW | **PASS** | `spec.md` A9 (trust boundary), L86 "No CCF save impact / backward compatibility", L88 "Evidence classification", §7 L139 "Stop conditions"; status still DRAFT. | Template sections are present. Nothing has been silently dropped. |
| **D-15** | LOW | **PASS** | `plan.md` Effort L52 "one sole implementation writer for P0 Core, Host and UI (no parallel UI subowner)"; `tasks.md` LAB-04 "SAME SOLE P0 SOURCE WRITER"; `requirements.md` REQ-E05. | No file still recommends concurrent writers. |
| **D-16** | LOW | **PASS** | `spec.md` §4 L96; `plan.md` G5; `tasks.md` LAB-05; `requirements.md` REQ-U09: a 2,000-tree fixture, selection, inspector and Core-preview refresh in ≤2 s on a documented or registered host and browser. | Measurable and honestly machine-dependent. |
| **D-17** | LOW | **PASS** | Fixed-string count of backslash-backtick lines: `754a467c` had 12/5/1/4 (spec/plan/tasks/checklist); **`e085ede8` has 0/0/0/0**. `plan.md` L15/L28 are real `` ```text `` / `` ``` `` fences. | Markup is valid. Cosmetic only: a double blank line at `spec.md` L80–81, and a blank line splits the `requirements.md` map list at L43. Both render fine. |
| **D-18** | LOW | **PASS** | `spec.md` L10 links the canonical `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md` on main (attachment noted only as an alternate); `requirements.md` REQ-E02. §5 still restricts GROWFOR to stand-level `B,N,H`, so there is no per-tree spatial ecology claim. | Matches the research index. |

## 3. Focused R2 / R4 / R9 re-check

- **R2 Engineering viability: now PASS** (was NEEDS CHANGES).
  - The state machine, atomicity, staleness tokens, canonical bytes, geometry, eligibility, save root and host trust are each specified tightly enough for a single P0 writer to implement and test without product guesswork.
  - The remaining items N-01 to N-06 are clarifications an implementer can follow from the existing hash-chain and STALE-on-load rules.
- **R4 Maths and conservation: PASS.**
  - Formulas are unchanged and correct.
  - The added unit conversions (mm→cm, mm²→m²→ha) are correct.
  - N=0 is shown as "not applicable", which avoids a Dq division by zero.
  - Removal BA comes from unique eligible IDs. Dead and felled trees keep their identity.
  - Integrity contracts (stale approvals, immutable events, snapshot hash, replay) are now defined (D-02, D-03, D-04).
- **R9 Formal spec structure: PASS** (was NEEDS CHANGES).
  - The approver is correctly named, and open decisions have owners, routes and timing.
  - Stop conditions are in the spec, rendering is fixed, and the single-writer rule is consistent.
  - **Immediately implementable after formal approval:** LAB-01 to LAB-05 (P0) only. LAB-06 and LAB-07 (P1) stay gated, and LAB-08 is out of scope.
- **Unchanged areas** (R1, R3, R5, R6, R7, R8 passed in #485): §1, §2 P1, §5, §8 and the scope and science boundaries were not weakened by the revision. Scenario One three-species protection (L36), GROWFOR stand-level limits (§5) and L4 NOT VALIDATED (§8, L88) are intact. **No regression.**

## 4. Non-blocking LOW clean-ups (new or residual; no re-review required)

Each item quotes the exact text and gives the smallest change. None of them adds forestry physics or scope.

- **N-01 (event completeness), `spec.md` A5 L71 / A9 L82.**
  - The snapshot contains `plans` and per-tree `markState`, and replay "rederives and checks **every** post hash". However, the spec does not say outright that *every* snapshot-changing action is an event.
  - Add: *"Every snapshot-changing action (mark, unmark, plan create/propose/approve/queue/reject/cancel/stale, execute, experiment step) appends exactly one event; each event's `prevStateHash` equals the previous event's `postStateHash`; replay from the fixture must reproduce the saved snapshot hash."*
  - A9's "event-specific payload objects use fixed fields documented in the lab's versioned schema" can stay as an implementer deliverable reviewed at LAB-05.
- **N-02 (epoch value and replay), `spec.md` A9 L82.**
  - Safety is already covered by "saved approved/pending plans … become STALE … on activation". However, the epoch's value type is not stated. If it is a per-process counter, it could numerically repeat across processes. The persisted `approvalSessionEpoch` also has to be reproducible on replay.
  - Add: *"`sessionEpoch` is an opaque runtime value; on load, all saved APPROVED/PENDING_WORK plans are made STALE unconditionally regardless of epoch value; replay takes `approvalSessionEpoch` from the approve event payload, never from the current runtime."*
- **N-03 (AC-07 wording), `spec.md` AC-07 L117.**
  - "CCF-shaped JSON, path traversal and symlink escapes fail before file read." Path and symlink escapes can fail before open, but a CCF-shaped file placed inside `.lab-data/` can only be rejected after its header is read.
  - Change to: *"Path traversal and symlink escapes fail before file open; CCF-shaped or wrong-format content inside the lab root fails at the format/schema header check before any forest/event parsing, with no write."*
  - Apply the same split to A9 L84 ("reject attempted CCF-save input … before file open").
- **N-04 (leftover wording), `spec.md` §2 L26, L34; §8 L143–144; `requirements.md` REQ-P02.**
  - Replace "synthetic flat Irish Sitka research stand … deterministic generated or explicitly imported synthetic source rows" with "synthetic flat Sitka test stand from checked-in synthetic fixtures".
  - Replace "mock calendar years" with "calendar-only experiment steps".
  - Replace "synthetic game parameters" with "synthetic fixture values".
  - In §8, change L0 "seed" to "fixture SHA-256" and L1 "annual history" to "experiment-step history".
  - Change REQ-P02 "synthetically generated" to "checked-in synthetic".
- **N-05 (naming consistency), `spec.md`.**
  - A1 L40 says `standAreaPolygon` is in "(local metres)", but A4 and A9 persist `{xMm,yMm}`. Change to "(integer millimetres)".
  - The transition table L60 says it stores "`sessionEpoch`", but A9 persists `approvalSessionEpoch`. Use `approvalSessionEpoch`.
  - A1 says status "live/dead/felled" and A5 says "living/dead/felled". Pick one enum spelling and state that felled and below-threshold trees persist `markState=UNMARKED`.
- **N-06 (ID characters), `spec.md` A9.** Byte identity also depends on JSON string escaping. Restrict `treeId`, `planId`, `forestId` and `fixtureId` to `[A-Za-z0-9_-]` (or ASCII printable without `"` or `\`) so that escaping cannot vary.
- **N-07 (optional checklist line), `requirements.md` Core integrity.** Add "No P0 top-height output", which mirrors spec A4. REQ-D04's "revised fixture" is undefined: either define it ("a load/reset of any fixture") or drop it.

## 5. Write scope and verification

This re-check adds exactly one file:
`Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md`
on `docs/multirack-sitka-lab-486-delta-review-result-20261114-v1`.

No author branch, #485 report, spec, canonical doc, `active-tasks.md`, source, Unity asset, save, data or build was changed. Dashboard/Drive mirror sync: **NOT VERIFIED**.
