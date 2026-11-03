# MFM-12 — Integrated MFM Qualification — Full Rerun RETURN

**Claim:** #225
**Parent:** #91 / Browser Overall #200 / local orchestration #203
**Date:** 2026-11-03
**Status:** **QUALIFICATION COMPLETE — INDEPENDENT REVIEW ROUND 1: NEEDS CHANGES (EVIDENCE/RECEIPT ONLY); FIXES APPLIED FOR DELTA REVIEW**
**Local verdict:** `MFM-12 QUALIFICATION PASS — PENDING DELTA REVIEW`

The final product token `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` is **not** self-issued. Browser Overall decides it after reviewing this return.

This is a **fresh full qualification** from the beginning of the A–P matrix on the exact post-#230 integration candidate. It does not continue the stopped MFM-12 run. No production, package, ProjectSettings, save-schema, calibration, protected-anchor, Rack, species-scope, ecology/economy or Work Plan change was made under #225.

## 1. Exact identity

| Field | Value |
|---|---|
| Exact production candidate | `e42bb9f437e4fcf20c5b753dacd1136470daf33e` (`integration/scenario-one-player-test-ready-v1`) |
| Branch | `qual/mfm-12-integrated-v2` |
| Worktree / Unity lane | `/media/jer/ZX20/CCF-U1` (dedicated, clean) |
| Qualification HEAD (verification tooling/docs only) | `85051922e394bf4a4723946dc97d9b2fddc136bc` |
| Earlier qualification heads used for evidence | `8a24be72e43c399d72b9050ceadc3dba8d5b69c0` (offline), `f9c36f6ab13814401d0d89b16efbde65753adbbf` (Unity 01–11) |
| Production diff candidate → final qualification HEAD | **empty** (`git diff e42bb9f4..85051922 -- Assets Packages ProjectSettings`) |
| Orchestration | `opencode-go/deepseek-v4.1-flash` |
| Independent reviewer | Claude Sonnet 5.5 (cross-provider; see `INDEPENDENT-REVIEW.md`) |
| Unity | 6000.6.0f1; save schema v24 |
| Evidence roots | `/media/jer/Files/CCF/Verification/MFM12-IntegratedQualification-Rerun-20261102/` (offline); `...-attempt3/` (Unity 01–11); `...-attempt3-resume2/` (Unity 01, 12–17); `...-guided-recheck/` (contentful guided-v2 rendered recheck); `...-attempt2/` and `...-attempt3-resume/` are retained superseded diagnostic attempts |
| Final lock state | released normally `2026-11-03T02:12:25Z` (lock `98c789a06cb54f2a92b961bd348cffa3`); status FREE; no Editor/Xephyr/openbox/driver (see §11) |
| Windows build | none; #194 NO ACTION |

The qualification branch adds only:

- `Tools/Verification/Mfm12IntegratedQualification/` (matrix, runners, accepted MFM-11 witness);
- two verification-tool allow-list/runner repairs described in §11;
- this durable return and `INDEPENDENT-REVIEW.md`.

The production tree under the final qualification HEAD is byte-identical to the exact candidate.

## 2. Verdict summary

The complete fresh matrix is green on the exact candidate, with the two carried accepted boundaries preserved:

- MFM-11 extension-state calibration is accepted within its **gameplay-calibration** boundary (baseline CI delta `4.169829%`, within the accepted 20%).
- Post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED** (the post-treatment Covered set is empty because treated units become `PlayerCompletedWork`; the witness records the reason explicitly).
- The three pure-Beech reduced-simulator R2 (single-year all-stem loss ≤ 0.40) misses (`0.423 / 0.407 / 0.417` against the unchanged `0.40`) remain the accepted C-level limitation.
- The repaired #230 fit replay is present in the live player flow, inspectable, gated before wider approval, and honest about invalid/unavailable state.

## 3. Current-candidate offline matrix

Command (caller need not hold the Unity lock for the offline phase):

```bash
cd /media/jer/ZX20/CCF-U1
env -u TMPDIR PYTHONDONTWRITEBYTECODE=1 python3 -B \
  Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head 8a24be72e43c399d72b9050ceadc3dba8d5b69c0 \
  --lock-id 50a94c2f960048729f5237effbe9e170 \
  --owner opencode-deepseek-v4.1-flash-MFM-12 \
  --phase offline --keep-going
```

Result: `MFM12_MATRIX_DONE steps=14 failures=0`.

| Step | Result | Marker / key evidence |
|---|---|---|
| 01-site | PASS | `SITE_INFRASTRUCTURE_PASS checks=63` |
| 02-growth-integrated | PASS | integrated calibration checkpoint run |
| 03-timber | PASS | `THREE_SPECIES_TIMBER_PASS checks=73` |
| 04-density | **ACCEPTED LIMITATION** | `COMPOSITE_DENSITY_FAIL checks=107 failures=3`; only `LONG_RUN_L40_B_BEECH_DENSE` R2 `0.423`, `LONG_RUN_L40_F_BEECH_YOUNG` R2 `0.407`, `LONG_RUN_E80_F_BEECH_YOUNG` R2 `0.417`; `0.40` unchanged |
| 05-history | PASS | `THREE_SPECIES_HISTORY_PASS checks=74` |
| 06-pruning | PASS | `PARITY06_PRUNING_PASS checks=3946 failures=0` |
| 07-prescription-core | PASS | `MFM04_PRESCRIPTION_CORE_PASS checks=240 failed=0`; byte-identical repeat |
| 08-candidates | PASS | `MFM05_CROP_TREE_CANDIDATES_PASS checks=95 failed=0`; byte-identical repeat |
| 09-coverage-world | PASS | `MFM08A_COVERAGE_WORLD_PASS checks=102 failed=0`; byte-identical repeat |
| 10-approval-boundary | PASS | `MFM07_APPROVAL_BOUNDARY_PASS checks=125 failed=0`; byte-identical repeat |
| 11-treatment-review | PASS | `MFM08_TREATMENT_REVIEW_PASS checks=312 failed=0`; repeat byte-identical; boundary clean after the qualification-tooling allow-list extension |
| 12-save-provenance | PASS | `MFM06_SAVE_OFFLINE_PASS checks=172` |
| 13-save-mutation-controls | PASS | `mutants=25 killed=25 survived=0` |
| 14-parity08-save | PASS | `PARITY08_SPECIES_OFFLINE_PASS checks=23` |

Logs: `.../logs/offline/`. Raw outputs: `.../offline/`.

## 4. Current-candidate Unity matrix

Evidence binding:

- Steps **01–11** were run at qualification HEAD `f9c36f6a` under lock `87733f5c7584490eb16ff63d2af3db3d`.
- Steps **01, 12–17** were re-run at qualification HEAD `85051922` under lock `98c789a06cb54f2a92b961bd348cffa3` after the verification-runner repairs in §10.
- Production bytes are identical at every qualification HEAD (`git diff e42bb9f4..<HEAD> -- Assets Packages ProjectSettings` is empty).

Command shape (steps 01–11):

```bash
cd /media/jer/ZX20/CCF-U1
env -u TMPDIR PYTHONDONTWRITEBYTECODE=1 python3 -B \
  Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head f9c36f6ab13814401d0d89b16efbde65753adbbf \
  --lock-id 87733f5c7584490eb16ff63d2af3db3d \
  --owner opencode-deepseek-v4.1-flash-MFM-12 \
  --phase unity --keep-going \
  --root /media/jer/Files/CCF/Verification/MFM12-IntegratedQualification-Rerun-20261102-attempt3
```

Command shape (steps 01, 12–17):

```bash
cd /media/jer/ZX20/CCF-U1
env -u TMPDIR PYTHONDONTWRITEBYTECODE=1 python3 -B \
  Tools/Verification/Mfm12IntegratedQualification/run_matrix.py \
  --lane /media/jer/ZX20/CCF-U1 \
  --head 85051922e394bf4a4723946dc97d9b2fddc136bc \
  --lock-id 98c789a06cb54f2a92b961bd348cffa3 \
  --owner opencode-deepseek-v4.1-flash-MFM-12 \
  --phase unity --keep-going \
  --root /media/jer/Files/CCF/Verification/MFM12-IntegratedQualification-Rerun-20261102-attempt3-resume2 \
  --only 01-compile,12-parity07,13-interaction,14-mixed-density-l40,15-broadleaf-l40,16-broadleaf-e80,17-history-l40
```

| Step | Result | Marker / key evidence |
|---|---|---|
| 01-compile | PASS | `MfmCompileGate`, Unity exit 0, harness/licence removed, lane clean, host saves unchanged |
| 02-approval | PASS | `MFM07_UNITY_PASS checks=106` |
| 03-coverage-world | PASS | `MFM08A_UNITY_PASS checks=56` |
| 04-save | PASS | `MFM06_UNITY_PASS checks=159` |
| 05-reference | PASS | `REFERENCE_FUTURE_V1_PASS`; `REFERENCE_FUTURE_V1_CONTRACT_PASS archive=True preview=True continuation=True replay=NotRequired` |
| 06-hardening | PASS | `SAVE_HARDENING_VERIFY_PASS`; malformed saves rejected before mutation |
| 07-extension | PASS | `MFM09_UNITY_PASS checks=261 shots=16` |
| 08-treatment-review (+ fit) | PASS | `MFM08_UNITY_PASS checks=412 shots=29` |
| 09-calibration-witness | PASS | `MFM11_UNITY_PASS checks=47 shots=2` |
| 10-guided-v2 | PASS | `GUIDED_ONBOARDING_PASS`; the matrix attempt produced 47 blank captures (one-off capture glitch), so the rendered evidence is the focused recheck at head `85051922`: 535 pass / 0 fail, 47 contentful captures (23 at 1280×720, 24 at 1920×1080) in `...-guided-recheck/` |
| 11-combined-v2 | PASS | `CHC_DONE pass=465 fail=0 shots=24` |
| 12-parity07 | PASS | `PARITY07_UNITY_HARNESS_PASS shots=36 passes=1031 failures=0` |
| 13-interaction | PASS | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` |
| 14-mixed-density-l40 | PASS | `MIXED_DENSITY_VERIFY_PASS checks=34` |
| 15-broadleaf-l40 | PASS | `BROADLEAF_TIMBER_VERIFY_PASS assertions=66` |
| 16-broadleaf-e80 | PASS | `BROADLEAF_TIMBER_VERIFY_PASS assertions=66` |
| 17-history-l40 | PASS | `SCENARIO_HISTORY_VERIFY_PASS assertions=43` |

Full matrix: 17/17 Unity steps PASS, 14/14 offline steps PASS or accepted limitation.

## 5. Repaired fit replay in the live player flow (#230 carry)

Step 08 stages the candidate's accepted #230 fit gate. Live results included:

- `current source-sample fit replay is available`;
- `reading the fit replay changes no mark, order, event, id, cash or hash`;
- `the fit replay contains sample removal decisions [agree=18 playerOnly=1 ruleOnly=0]`;
- `player-only row is individually inspectable [P0716]`;
- `the fit replay carries Crop Tree and basal-area context`;
- `the review card shows the fit summary and Share/removal consequences`;
- `per-Crop-Tree player-vs-rule Share is shown`;
- `fit is presented as a comparison, not a forestry-correctness score`;
- `a changed sample makes the current fit replay invalid` and `wider approval is refused when the fit replay is invalid, with no mark/order/event/id/cash change`;
- `restoring the sample restores a valid current fit replay`;
- `an unavailable fit view blocks the approval gate`.

Rendered fit captures at both sizes: `07a-fit-comparison-1280x720.png`, `07a-fit-comparison-1920x1080.png`, `07b-fit-disagreement-1280x720.png`, `07b-fit-disagreement-1920x1080.png`.

## 6. Candidate / judgement / proposal / Work Plan boundaries

Step 08 confirms:

- candidates are spacing-only inspection prompts, not quality judgements;
- the map never confirms; `[C]` in 3D confirms through the ordinary mark path; `[N]` rejects as a session decision; manual designation remains available;
- `[C]` toggles the ordinary mark back and leaves no review-owned state;
- proposals are rendered as `PROPOSAL — not a Fell mark and not yet work`; proposal = triangle, candidate = square;
- a manual Fell mark is untouched by the review, and a manually marked tree is held back from proposals;
- `Approve` is off before a real 3D spot-check; a map waypoint alone does not satisfy it; the handler re-checks the gate;
- prescription approval creates ordinary Pending orders only, with prescription/coverage-unit/Crop Tree/closed-reason provenance;
- opening the Work Plan approves nothing; ordinary Work Plan approval is the commitment step; approval is not execution.

Step 02 independently confirms the same boundary at the manager/domain level (`MFM07_UNITY_PASS checks=106`), including refusal cases that leave the world, marks, orders, ids, cash and stored prescription unchanged.

## 7. Persistence / compatibility and protected anchors

Step 04 (`MFM06_UNITY_PASS checks=159`) proves, on the exact candidate:

- malformed/ambiguous new save data is rejected **before** mutation: missing allocator, wrong-typed prescription id, missing order provenance, v22 header carrying prescription state, truncated document, duplicate key, duplicate prescription id, impossible source rack, unknown rule version, order naming an unknown prescription, provenance on an ordinary order;
- every refusal leaves world, hash, trees, wood, prescriptions and links unchanged;
- approved prescription provenance and its id allocator reload without creating new pending work or proposals;
- missing old fields mean no prescription state; no migration fabricates prescriptions.

Step 06 (`SAVE_HARDENING_VERIFY_PASS`) independently repeats rejection-before-mutation for empty, duplicate-tree-ID, unknown-species and related malformed saves.

Step 05 protects the frozen reference:

- `REFERENCE_FUTURE_V1_CONTRACT_PASS archive=True preview=True continuation=True replay=NotRequired`;
- `REFERENCE_CONTINUATION_PASS year50=v12 year100=v24 deterministic=True continuedHash=EBD49ADEF18C3F36 legacyV15LayoutHash=BFFF66015A6619DA`;
- `REFERENCE_FUTURE_V1_PASS`.

Legacy40 and pre-feature Enlarged80 remain valid on the exact candidate: step 14 MixedDensity, steps 15/16 BroadleafTimber L40/E80, and step 17 History L40 all pass. Step 09 additionally preserves the save/load world hash `AAD4776628E3D45F`.

## 8. Complete Scenario One witness

Step 09 (`MFM11CalibrationImpactGate`) is the explicit complete-loop witness on the exact candidate. All cash figures in this witness are integer cents:

| Stage | Observed |
|---|---|
| Initial forest / access | Enlarged80 Rack V2 world, 1344 living trees, cash 1,200,000 cents (€12,000); five racks added and confirmed; rack access approved through the Work Plan and executed (year 1) |
| Sample teaching | three distributed sample Crop Trees P0810/P1206/P1214; sample release planned and executed (year 2); sample release passed |
| Baseline | year 2, 1177 living, BA 25.471506 m², cash 776,854, canopy 0.970165, light 0.029835, regen 51 |
| Extend treatment | 17 spacing candidates, 13 coverage areas, prescription 1 prepared, 3D spot-check recorded, prescription approved |
| Ordinary work | 84 Pending orders carrying prescription provenance; Work Plan approval 84; ordinary year execution; cash/timber reconciled (`67456 − 250000 = −182544` cents, i.e. −€1,825.44; observed delta matches) |
| Later intervention | beech 3 + oak 3 planted; second intervention 10 trees at year 17 |
| Completion | `outcome=Completed outcomeYear=25 finalYear=25 cash=348152`; 8 objectives, 0 unachieved (managed opening 1/1; introduced beech 1/1; introduced sessile-oak 1/1; retained canopy 893/240; continuous canopy 0.901015/0.35; regeneration 223/12; fallen deadwood 85.73/0.08; minimum-year 25/25) |

The witness also records the accepted MFM-11 boundary explicitly:

- baseline CI parity delta `4.169829%` within the accepted 20% **GAMEPLAY CALIBRATION** tolerance;
- post-B year 5: `UNAVAILABLE — 0 covered areas`, exclusion reason `PlayerCompletedWork=13`, `cross-year CI parity NOT DEMONSTRATED`;
- year 25: `UNAVAILABLE` because the sample's own release is not executable at that state; `cross-year CI parity NOT DEMONSTRATED`.

## 9. Repeated-cycle provenance / history

Step 17 (`SCENARIO_HISTORY_VERIFY_PASS assertions=43`) exercises three management cycles on the accepted **Legacy40** path (`CCF_STAND_GEOMETRY=0`):

- cycle 2 resolves as a new entry with its own snapshots; the first intervention is unchanged;
- a fresh order is not stale; an old pending order is surfaced with its age after two years and resolved in cycle 3;
- no two-cycle special case; no stale orders remain once resolved;
- both stems left as deadwood by plan 2 are on record for that year; other deaths match an independent recount;
- save is version 24; reload produces identical history with no extra records;
- all six marked trees across the three cycles retain their completed work orders after reload;
- completion summary is for the completion year, totals match the interventions, wording is produced, and management stays available after completion until the Century Review.

This step is **Legacy40 history**, not an MFM-prescription world. Step 07 independently confirms prescription provenance survives Pending → carried over → Work Plan approval → execution, and that manual work carries no prescription provenance; step 02 confirms one `OrderCreated` history event per generated order and its reload linkage. No single MFM-prescription world was run through a two-cycle Annual Review/history surface; that remains an evidence-coverage limitation (see §13).

## 10. Rendered evidence

154 contentful PNGs were captured under the final evidence roots. The gates that assert rendered output check both 1280×720 and 1920×1080, visible pixels, in-window layout and no critical clipping/overlap. `guided-v2` is a layout/behaviour gate with no pixel assertion of its own; its contentful captures come from the focused recheck and were verified for content independently.

| Area | Captures |
|---|---|
| Fit comparison / disagreement (inside the 29 treatment-review captures) | 2 at each size (`07a`, `07b`) |
| Treatment review (sample, coverage, candidates, 3D confirm/reject, proposal vs Fell, conflict, spot-check, approve, pending, Work Plan) | 29 total: 14 at 1280×720, 15 at 1920×1080 |
| Extension / Work Plan pending and approved | 16: 8 at each size |
| Calibration witness post-approval state | 2 (one at each size) |
| Guided onboarding v2 (focused recheck; the first attempt's PNGs were blank) | 47: 23 at 1280×720, 24 at 1920×1080 |
| Combined HUD v2 | 24: 12 at each size |
| PARITY-07 rendered | 36: 18 at each size |

Total final contentful rendered set: 154 PNGs (29 + 16 + 2 + 47 + 24 + 36). The blank guided-v2 attempt is retained only as diagnostic evidence, marked `SUPERSEDED.md`, and is not counted. The guided-v2 recheck receipt is `...-guided-recheck/RECEIPT.json` (535 `GUIDED_PASS`, 0 fail, 47 contentful captures, head `85051922`, lock `98c789a0…`).

Visual/semantic distinctions verified by the gates include: sample, coverage, candidate, proposal, approved work and executed result; candidate/proposal glyph form distinct from Red Fell, Blue Crop Tree and Orange Rack marks; coverage/exclusion reasons readable; no critical overlap/clipping on objective, history, review and Work Plan surfaces.

## 11. Incidents, retries and cleanup

No production defect was found. The following environment/tooling incidents occurred and were resolved without product change:

1. **Host OOM during first Unity compile (attempt 2).** The Editor was OOM-killed at 278 s with the host under memory pressure. The lane was left clean; no partial pass was counted. The run was restarted on a warm Library after memory recovered.
2. **Calibration witness did not exit after its verdict (attempt 2).** The verification runner waited indefinitely. The runner was given the same bounded post-verdict reaper already used by the accepted MFM-08 runner; the witness itself was unchanged. Re-run passed (`MFM11_UNITY_PASS checks=47 shots=2`).
3. **Combined-v2 left known Unity import side effects (`.vscode/settings.json`, SectionFive materials).** The subsequent preflights correctly refused the dirty lane. The tracked side effects were restored byte-exactly from Git and the affected steps were re-run. No user work was discarded.
4. **Three-species runner copied a stale local licence.** `run_qualification.py` hard-coded the older `~/.config` licence instead of the newest installed licence used by the other runners, causing Unity exit 198. The runner now selects the newest of the two known licence locations, matching the accepted runners. No test assertion changed.
5. **Guided-v2 rendered captures were blank in the matrix attempt.** The gate's 535 behavioural/layout checks passed, but all 47 PNGs were fully transparent. A focused re-run of the same gate on a fresh display at head `85051922` produced 47 contentful captures at both sizes with `GUIDED_ONBOARDING_PASS`. Prior accepted guided-v2 runs and the recheck both render normally, so this was a capture/window-focus glitch, not a product rendering failure. The matrix gate result remains PASS; the recheck is the rendered evidence.

The offline matrix runner overwrites its `summary.json` when a later Unity phase uses the same root. The per-step logs were preserved and a reconstructed `offline-summary-final.json` was written in the offline evidence root; no step was reclassified. The offline phase's `--head` argument is advisory (the offline scripts do not check out or gate on it); the lane HEAD was `8a24be72` for that run and its production diff to the candidate is empty.

Cleanup receipt:

- final lane clean at `85051922` including untracked files (only the durable HANDOFF is untracked at handoff);
- no staged harness or `.meta` under `Assets/ForestPrototype/`;
- no Unity Editor, Xephyr, openbox or native-input driver running;
- lock `98c789a06cb54f2a92b961bd348cffa3` released normally at `2026-11-03T02:12:25Z`; final state FREE, metadata None, no Editor PIDs; lane clean; no Xephyr/openbox/driver;
- host saves unchanged by every Unity gate (`host_saves_unchanged: true`; the guided-v2 recheck was independently compared with the recorded pre-run hashes);
- no copied Unity licence files remain in the durable evidence roots;
- no Windows build; #194 NO ACTION.

## 12. Checklist disposition (A–P)

| Section | Disposition on this candidate |
|---|---|
| A Architecture | PASS — individual tree/ecology-cell state remains authoritative; Management/Map ↔ 3D interaction; no stand-level biological state added |
| B Scenario One prerequisites | PASS — Rack V2 geometry/world identity verified; sample/coverage derive from confirmed racks; no parallel authored rack source |
| C Teaching preservation | PASS — Lessons 2–5 behaviour green in guided-v2 and extension; 8 m teaching spacing and 8 m Hegyi remain separate; 30%/40% safeguards covered by candidate/coverage gates |
| D Sample and coverage | PASS — deterministic teaching rack, fixed 432 m² teaching rectangle, corridor exclusion, explicit Covered/Not covered/Excluded/Manual states and reasons |
| E Prescription derivation | PASS — deterministic derivation from executed sample decisions; fit replay live; disagreements individually inspectable; no quality ranking |
| F Crop Tree candidates | PASS for the product boundary — spacing-only, deterministic, individually confirmed/rejected, 3D-grounded, manual override, no bulk confirm; **F11 (user-tested workload) remains open** |
| G Species honesty | PASS — Sitka-only quantitative logic; Oak/Beech exclusions remain explicit (MFM-10 carry plus current three-species gates) |
| H Proposal state | PASS — proposals are not Fell marks, not player marks, not imported by opening or approving the Work Plan; manual state wins and conflicts are surfaced |
| I Approval / execution | PASS — prescription approval = Pending only; Work Plan = commitment; ordinary contractor/economy path; durable provenance |
| J Explainability | PASS — prescription/version/sample/coverage/Crop Tree/reason provenance; Why proposed/marked uses observable state; disagreements and uncovered areas inspectable |
| K Persistence / compatibility | PASS — v24; old/missing fields mean no prescription; malformed saves rejected before mutation; Legacy40 and pre-feature Enlarged80 valid; frozen Reference Future protected |
| L UX | PASS — required distinctions, 720p/1080p rendering, no critical clipping/overlap, #199 minimap/focus contracts green, guided-v2 green |
| M Calibration / safety | PASS — MFM-11 accepted boundary carried; 40% safeguard, viable economy, timber/deadwood/regeneration/light coherent, completion reachable at year 25 |
| N Teaching / scope | PASS — extension begins after Lesson 5; full manual management remains; no Windows build; external testing not claimed |
| O Evidence / wording | PASS — gameplay-calibration labels retained; evidence-backed analogues separated; no hidden-quality or unrestricted-machine-judgement claim |
| P Final gate | P1–P3 met on this return; P4 is the independent review (round 1 NEEDS CHANGES on evidence/receipt only; delta re-review pending); P5 is Browser Overall publication and is not self-issued; P6 (personal user playtest) remains external |

## 13. Unresolved / carried limitations

- Three pure-Beech reduced-simulator R2 (single-year all-stem loss ≤ 0.40) misses (`0.423 / 0.407 / 0.417`) against the unchanged `0.40` are the accepted C-level limitation.
- Post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.
- MFM-11 extension-state calibration is accepted only within its **gameplay-calibration** boundary.
- F11 candidate workload remains a human/user-test obligation; it was not manufactured here.
- The live fit fixture produces a player-only disagreement only (`agree=18 playerOnly=1 ruleOnly=0`). The rule-only presentation is supported by the UI mapping and the offline model/source checks, but the live harness loop over rule-only rows ran zero iterations and the rendered rule-only card was not separately captured. The explicit invalid-fit card is asserted at gate level (synthetic view) and blocks approval, but was not separately screenshotted. The rendered captures cover the valid player-only disagreement and the approval-ready state. Wording here is deliberately narrower than the #230 summary.
- The dedicated manager `Refusal.FitUnavailable` branch is code-present but no gate references or executes it; the stale-sample live case is refused earlier by authoritative sample validation (`SampleNotDerived`/`SampleChanged`). The UI gate blocks approval with the fit reason and wider approval is refused with no mutation. This matches the #230 acceptance caveat and is not claimed as live-proven.
- The fit result is not persisted. Spec §15's eventual "what fit was reported" history question is recomputable from the stored sample/removals and the deterministic core, but no saved fit record was added in V1 and no history surface reports it.
- MFM prescription provenance is demonstrated through prescription approval, Pending work, Work Plan approval, ordinary execution and carried-over Pending rows (step 07), and the witness's second intervention/completion (step 09). Step 17's three-cycle history and reload identity are the accepted Legacy40 history path, not an MFM-prescription Annual Review history. No single world combined an MFM prescription with a two-cycle Annual Review/history surface; this is an evidence-coverage limitation, not a contradictory result.
- The `10-approve-ready` capture at 1280×720 is scrolled to the card top; the Approve control and spot-check state are asserted in the live UI tree but are not visible in that particular frame. The 1920×1080 capture and the step 07 pending/approved captures show the boundary clearly.
- The binding `TASK-PACKET.md` is absent from the integration branch tree (it exists on `main`/the qualification lineage); this rerun read the binding packet from `main`. This is a documentation-packaging observation, not a product finding.
- The three-species Unity batch gates are run through a user-systemd scope with `MemoryMax=8G`; this is the accepted runner behaviour and is not a product constraint.
- Copied Unity licence files left by the isolated-config runners were stripped from the durable evidence roots before handoff; none remain.

## 14. Independent review

`INDEPENDENT-REVIEW.md` in this folder records the fresh cross-provider Claude Sonnet 5.5 review of the exact candidate and the evidence package. Round 1 returned **NEEDS CHANGES**, narrow and evidence/receipt only, with no production/spec defect and no stop condition; its conditions were the guided-v2 blank capture, the stale lock/cleanup receipt and the pre-asserted verdict. Those conditions were fixed, and a short delta review was requested. The published file includes both the round-1 findings and the delta outcome.

## 15. Exact return

`MFM-12 QUALIFICATION PASS — PENDING DELTA REVIEW`

Target on approval (not yet issued):

`MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE`

Not issued here (Browser Overall authority):

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`
