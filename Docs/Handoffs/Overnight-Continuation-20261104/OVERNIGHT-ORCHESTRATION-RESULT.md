# CCF Overnight Orchestration Result — DeepSeek DPC (#254)

**Status: FINAL DELIVERY (spec-lane revisions still completing — marked inline).**
**Written:** 2026-11-04, from commands executed in the DPC session. Figures are reproduced from terminal output or from a cited branch/commit. Anything not directly observed is marked **UNKNOWN** or **UNMEASURED**.
**Parent:** [#254](https://github.com/jermur788/CCF/issues/254) · **Ledger:** [ROUTING-LEDGER.md](ROUTING-LEDGER.md) (single writer, DPC) · **Programme ledger for #253:** [ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md](../ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md)

> A worker handoff is a claim. **IMPLEMENTED ≠ REVIEWED ≠ INTEGRATED ≠ VERIFIED ≠ ACCEPTED.** Nothing in this report is integrated or accepted. No Windows build exists.

---

## 1. Headline

Seven authorised lanes ran across **four independent subscription pools**. Every lane received a cross-family review. No lane merged, none touched the protected player-test candidate, and **no Unity Editor was started**.

| Lane | Outcome | PR |
|---|---|---|
| #255 Objective copy | **REVIEWED** — corrected after review, gate re-run green | [#262](https://github.com/jermur788/CCF/pull/262) |
| #256 Rack handle rotation | **REVIEWED** — APPROVE; source-trace evidence only | [#261](https://github.com/jermur788/CCF/pull/261) |
| #253 Option A MFM offline | **HANDED-OFF** — PARTIALLY QUALIFIED; corrections applied | [#267](https://github.com/jermur788/CCF/pull/267) |
| #257 Input settings spec | **DRAFT-COMPLETE** — 3 review rounds | [#263](https://github.com/jermur788/CCF/pull/263) |
| #258 Objective guidance spec | **DRAFT** — precedence defect still being fixed | [#265](https://github.com/jermur788/CCF/pull/265) |
| #259 Mark language spec | **DRAFT** — contrast criterion proven unachievable, being replaced | [#266](https://github.com/jermur788/CCF/pull/266) |
| #260 Baseline verification | **REVIEWED** — baseline accepted, union defect being fixed | [#264](https://github.com/jermur788/CCF/pull/264) |

**The most valuable output is not any of the fixes.** Four separate assumptions in the task packets turned out to be wrong, and each would have produced a correct-looking fix that was wrong:

1. **The Fell mark is not red.** In-world `(1, 0.35, 0.05)` — orange-red. On the map `(220, 76, 62)` — genuinely red. Help text says "Red". A three-surface inconsistency, not a copy error.
2. **#256's gesture lived in a different file** (`Map/StandMapRackEditor.cs`, not `StandMapView.cs`), and **`Move` never used the snap flag at all** — the regression the packet feared was structurally impossible.
3. **`CurrentVersion` is 21 on main and 24 at the candidate.** The spec was accused of a wrong number; the actual defect was **unlabelled mixed baselines**.
4. **#251's R1/R2 overlap claim was wrong.** Gap is `0.214214 m`; the "overlap" was the editor's 2.4 m centreline-clearance rule. Corrected in the programme ledger, dated and attributed, with #251's handoff left unedited.

A fifth, caught only by execution: **#255 and #256 are individually green and together break the #260 harness.** See §5.

---

## 2. Safety state

| Check | Result | Evidence |
|---|---|---|
| Protected candidate `integration/scenario-one-player-test-ready-v1` | `72b4efbd` — unchanged all night | `git rev-parse origin/...` |
| `origin/main` | `8f0f2301` — no DPC commit landed | `git rev-parse origin/main` |
| Unity Editor processes | none | `pgrep` |
| Unity lease | not acquired; `unity-lock status` = free, no PIDs | `CCF-Ops/scripts/unity-lock status` |
| Windows / Player build | none | no build command issued |
| `active-tasks.md` | not edited by DPC (#200 sole writer) | by construction |
| Branches pushed | 6 feature/docs/verify + 1 orchestration | `git ls-remote --heads origin` |

**Unity was refused deliberately.** Live disk: ZX20 **19 GiB free** against a `lanes.json` floor of **15 GiB**, with a cold Library at 10–12 GiB on the same volume as the shared Git store, and the only warm Library belonging to `CCF-U1`, held for the user's own playtest. Every worker was required to label runtime, rendered and input claims **UNVERIFIED**. Consequence stated plainly: **#256's fix has no execution evidence** — only source trace and independent review.

---

## 3. Production lanes

### 3.1 #255 — Objective copy — REVIEWED

Branch `feature/overnight-objective-copy-20261104`, head `693e24c8`.

- `16–20 m` → `16–20m` in Objective 1 concept and the rack-reading step; `Title(Menu.TreeMarks)` → `Introducing tree marking`; explanation opens `Scenario 1 introduces 3 CCF markings.`; the "CCF source material" sentence **replaced** with wording that matches what the code paints.
- Review: APPROVE WITH REQUIRED CHANGES → re-check **CORRECTION ACCEPTED**.
- **The required change was not in the copy.** The repository's own suite still asserted the superseded strings — five assertions in `RackV2HelpTeachingOfflineCheck.cs`, one in `RackV2Objective1EndToEnd.cs`. The worker had reported one S10 failure and had not disclosed them.
- DPC correction: exact-string assertions (none removed, none downgraded to substring checks), a `UnitStyle()` comparator normalising **only** the requested unit spelling while still comparing whole sentences, and two exact paths added to the S10 allow-list.
- **Executed gate:** `OFFLINE_FAIL failures=5 checks=97` → `RACK_V2_HELP_TEACHING_PASS static_and_offline=15`, 97 checks. Reproduced by DPC and independently by the re-checker.
- **UNVERIFIED:** rendered text, wrapping, modal fit; the Unity end-to-end assertion was never executed.

### 3.2 #256 — Rack round-handle free rotation — REVIEWED

Branch `feature/overnight-rack-handle-20261104`, head `9304d318`. Two source lines:
`Propose(px, gesture != Gesture.Reshape && !ShiftHeld(e.modifiers), …)` plus the instruction text.

- Review APPROVE confirmed: `Gesture.Reshape` is entered only from `OnHandle`; **`Move` never used the snap flag**; snapping only reaches `SnapFar`, so **no validity rule is bypassed**; no keyboard rotation path exists.
- **Evidence correction:** the worker's "static harness 10/10 PASS" was `RackV2ConfirmMinimap/run_static.py`, which does **not** exercise rotation or Shift. The reviewer reproduced 10/10 and said it is not evidence for this change. **No automated test covers the new behaviour.**
- Non-blocking: near the stand edge the unsnapped handle can now reach a red "outside stand" ghost; rules still refuse it, by design.

### 3.3 #260 — Baseline verification — REVIEWED (baseline + combined verification)

Branch `verify/overnight-playtest-feedback-20261104`, head `6efba8fd`.

- Baseline tally **PASS=3 / FAIL=4 / NOT-TESTABLE=2**, exit 1 — deliberately pre-fix. DPC reproduced it; runs are byte-identical and deterministic; no network, no writes.
- Confirmed in source: `LearningObjectivesView.cs:75` gives `Back to forest [O / Esc]` the `btn-primary` class while `Stand map [M]` at `:73` has none.
- Follow-up added a rotation-snap policy check with **pre/post-fix states** and executed `RackEditing.SnapFar` directly on plain .NET across six cases — the first actual execution of the snapping function.
- Final follow-up added `--instruction-state pre-fix|post-fix` after DPC found the union defect (§5), so a mismatched copy reports **FAIL** instead of aborting.
- **Combined-tree result, run by DPC** on the #255+#256 merge tree (`b9f4d047`, post/post flags):

```
SOURCE-STATIC TALLY PASS=9 FAIL=1 NOT-TESTABLE=2
```

  The single FAIL is **check 08 CTA** — `baseline primary invokes ui.CloseAll()` — which is the user's still-unfixed complaint, not a regression. **This is the first genuinely combined evidence of the night.**
- Weaknesses named by review and kept visible: check 01 asserts a **presentation format**; check 05 tests only absence of a word; check 08 originally required one specific fix label.

### 3.4 #253 — Option A offline qualification — HANDED-OFF, PARTIALLY QUALIFIED

Branch `docs/mfm-option-a-offline-20261104-v1`, head `3d8338c7`.

All **41 checks passed byte-identically**, and the reviewer recomputed from **raw JSON** (the worker had reused #251's table). Findings, with the reviewer's own numbers:

| Claim | Verdict | Number |
|---|---|---|
| Reach defect | CONFIRMED | max **69.1695 m**; nearest-owner uncovered **14.13–38.22%**, lateral **36.25–49.03%** over 20–9 m caps |
| Sample placement | CONFIRMED | 142.97 m² of R4 Centre outside its unit = **65.96%** of that area |
| Grouping | CONFIRMED | single-link chaining; singleton strips need a rule or refusal |
| Confirm gate | CONFIRMED | **30 direct calls across 16 C# verification files** |
| R1/R2 correction | CONFIRMED | gap **0.214214 m**; **#251's overlap claim was wrong** |
| Raster values | partly WRONG | "exact" withdrawn → still an approximation between sampled lines |

Corrections applied: count attributed, both reach ranges quoted, "exact" removed, review record added, 12 decisions split into **product / engineering / commissioning**, and a model-fidelity paragraph stating that an algebraic match in Python doubles is **not** executable equivalence to production C#.

**Disclosed inconsistency:** the worker left "exact scan" labels inside its script, `run.log` and `results.json` because regenerating them would change the published hashes; §6 of the handoff says the correction governs those labels. Recorded rather than smoothed.

---

## 4. Spec lanes

### 4.1 #257 — Input settings — DRAFT-COMPLETE (needs Q1–Q13)

Three review rounds. Final head `ae7944aa`. I read the revised `REQ-P3a` rather than accepting "FIXED":

> A required-vs-required collision whose second default is also taken **rejects the entire binding set as a unit**, falls back to the built-in defaults, keeps the file as `.bak`, and names both actions — with the invariant that makes it total (a collision-free default set binding every required action, AT-17b).

**No path leaves a required action unbound.** Authority hierarchy, S-6 scope and a single atomic import rule are explicit.

**One item owed, not agreed:** the inventory dispute is *definitional*. Method A reproduces (71/63/16 candidate, 68/60/14 main); Method B's 73/61 was never reproduced and its command was never published. Both figures and definitions are in the spec, with an open task asking the reviewer to append the command.

### 4.2 #258 — Objective guidance — DRAFT-COMPLETE (needs product decisions)

Two review rounds. P2 and P3 accepted. P1 failed once, then was fixed properly.

The first attempt ranked blocking states with a hand-written Boolean order that **ranked a retained rack confirmation above Help even though Help can overlay it**. A deterministic order that ranks an obscured control above the visible one is worse than a heuristic, because it looks rigorous.

The fix replaced the list with a **rule**: `FR-2` — the primary cue may target only the foremost visible, enabled surface that owns input; anything obscured or input-blocked is ineligible regardless of its pending Boolean; and the rule applies to future overlays without appending names to a precedence list. `AC-1a`/`AC-1b` cover the two cases that broke it (Help over a retained rack confirmation, Help over the Work Plan advance warning).

DPC verification: read FR-2 at source, confirmed the "never break ties by Boolean order" clause, confirmed both tests are present and **unrun**, and spot-checked `WorkPlanView.cs:21` (`confirmingAdvance`) and `StandMapRackEditor.cs:58` (`ConsentPending`). That check is cross-family (DeepSeek vs Codex) but is **not** the independent re-review the spec still requires.

### 4.3 #259 — Tree mark language — DRAFT (criterion being replaced)

Five review findings; four accepted. The fifth is a defect **the spec created for itself**, and two independent computations agree:

| Pair | WCAG contrast |
|---|---|
| rack `(0.95,0.28,0.025)` vs Fell `(1,0.35,0.05)` | **1.17:1** |
| light pink `#FFD1E8` vs Fell | 2.32:1 |
| pink `(1,0.60,0.80)` vs Fell | 1.60:1 |

Against Fell (`L = 0.285`), 3:1 requires the other mark to be `L ≥ 0.954` (near-white) or `L ≤ 0.062` (very dark maroon). **No recognisably pink or orange colour can reach it**, so the orange pair fails, the pink fallback fails, the spec forbids weakening the threshold — the rule terminates in its own redesign stop every time.

**Diagnosis:** contrast ratio is a WCAG measure for a component against its *background*. It is the wrong tool for two hot hues told apart by **shape**. ΔE76 shows the real defect: the current pair is **6.0** (barely perceptible — which is exactly why the marks confuse the player), while pink candidates are **79–88** (decisive). The fix offered is to separate the concerns: perceptual-difference floor **between marks**, contrast floor **against bark/background**, recognition as the binding gate.

Forestry sourcing was independently verified: the Pro Silva Ireland 2026 guide exists with a **matching SHA-256**, and it does document diagonal removal slashes, four-sided rack stripes, white/blue quality rings, two lines on retained route-side trees, and hot=orange/pink vs cool=white/blue. The publisher's "widely applied" claim was correctly **not** adopted as a standard.

---

## 5. The union-only defect — found by execution, now closed

DPC built a **detached** worktree at the object-level merge tree of #255 and #256 (`b9f4d047`, from `git merge-tree`) and ran both harnesses on the combined state. No branch, commit, ref or push was created — verification, not integration.

1. `run_offline.py`: `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`. Only the S10 **branch**-scope guard fails, correctly, because #256's files are present.
2. `check_baseline.py --rotation-state post-fix`: checks 01–05 **PASS** — the four pre-fix baseline failures genuinely flipped once #255 landed — then it **aborted**:

```
SOURCE-STATIC ERROR: source anchor missing:
  Assets/ForestPrototype/UI/StandMapView.cs: Hold Shift to turn snapping off.
```

Root cause at `check_baseline.py:204`: the checker hard-coded the **pre-#256** instruction sentence as an evidence anchor. Each branch was green alone; **together they broke the harness**. This is the union-only failure class the programme has hit before, invisible to branch-local review.

**Closed:** #260 added `--instruction-state pre-fix|post-fix`. Re-run by DPC on the same combined tree:

```
SOURCE-STATIC TALLY PASS=9 FAIL=1 NOT-TESTABLE=2     (only the CTA check fails — the unfixed complaint)
```

The baseline record is preserved: pre/pre still gives the original 6 PASS / 4 FAIL / 2 NOT-TESTABLE.

---

## 6. Routing and provider evidence

| Pool (observed) | Model / effort | Lanes |
|---|---|---|
| `codex` ("Codex") | `gpt-6.1-sol`, `reasoningEffort: medium` | #255, #256, #258 + reviews |
| `codex_c75d8114` ("ChatGPT - OpenAI B") | `gpt-6.1-sol`, medium | #259, #260 |
| `claudeAgent` ("Claude A") | `claude-sonnet-5-5`, `effort: medium` | #253, reviews |
| `claudeAgent_claude_b_pro` ("Claude B — Pro") | `claude-sonnet-5-5`, medium | #257, reviews |

- **Quota, tokens, context, reset times: UNKNOWN for every pool.** No endpoint exposes them and no account console was consulted. Sustained success is **not** evidence of headroom.
- **No quota interruption, cooldown or reset occurred.** Nothing to wait for; no resume-from-reset was simulated.
- **No escalation was needed.** Medium was sufficient everywhere. **Opus 5.5 and higher effort tiers were authorised but unused** because no Medium run produced a source-backed reasoning failure — and quota pressure would not have counted as one.
- **Independence:** every production review was cross-family **except #258**, where implementer and reviewer were both Codex-family. Flagged as the weakest separation in the programme rather than described as independent.

## 7. What went wrong

1. **Workers overstated verification.** #256's "10/10 PASS" tested a different subsystem. #255's "13/14, one allow-list failure" hid five stale assertions. Both surfaced only because reviewers were told to verify from source.
2. **Two specs carried factual errors** — a save version read across mixed baselines, and a wrong Esc count — that would have reached the Manager looking approved.
3. **DPC repeated one of them** (the save version) before correcting it against both bases. The correction sits next to the original statement in the ledger; the original was not silently edited.
4. **Handoffs are bloated** — 4,847 and 3,889 lines for 9- and 2-line changes. Not a defect in the work, but it buries the evidence.
5. **A spec requirement was unachievable and survived a full author/reviewer round** until someone computed it. Numbers in specs deserve the same scepticism as numbers in handoffs.

## 8. What is safe next (Manager decisions)

1. **#255 and #256 are ready for the Manager queue.** They merge cleanly at object level (`git merge-tree` → `b9f4d047`, exit 0) with disjoint source files. Neither is integrated or accepted.
2. **Decide the `16–20m` scope question:** `StandMapView.cs:603` still shows `16–20 m`, so a player sees both spellings until it is fixed. Outside #255's packet.
3. **Reconcile spec §28** in `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md`, which still contains the superseded "CCF source material" sentence.
4. **Decide the mark-colour policy** (#259) from the three-surface table and the contrast arithmetic, not from the copy alone.
5. **Authorise a real Unity pass for #256** when disk and a lease permit — there is no execution evidence for it.
6. **Answer Q1–Q13** (#257) and the product-group decisions in #253 §10.
7. **Nothing may be built for Windows** until your own playtest is complete and you explicitly authorise it. That gate was not touched.

## 9. Not done

- Unity compilation, input, rendered layout, manual playtest: **not performed** (admission refused on disk).
- #258, #259 revisions and the #260 re-anchor: **in progress at publication**.
- `active-tasks.md`: **not updated by DPC** (#200 sole writer).
- Method B inventory command: **owed by the reviewer**.

---

*Committed on `orchestration/overnight-20261104-dpc` with the ledger. Return to the Overall Manager (#200) for approval and integration.*
