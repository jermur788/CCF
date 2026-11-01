# MFM-11 — Independent Review, Round 2

**Claim:** #220 — Calibration Impact Qualification  
**Date:** 2026-11-01  
**Verdict:** `APPROVED`  
**Scope:** independent review of the revised qualification evidence and its measurement integrity; Manager acceptance remains outstanding.

## Identity, independence and inspected artifacts

Reviewer: **OpenAI Codex gpt-6.1-sol**, acting as the assigned independent re-reviewer, separate from the Mimo 2.6 Pro qualification implementer. Claude Sonnet 5.5 performed the first independent review at `2bb42a36` and returned `NEEDS CHANGES`. This review independently checks the revised evidence rather than treating the worker response as proof.

- Exact production candidate: `3acdf9cb635cdca79606335801e97f9d7dcce8b0`.
- Reviewed branch: `verify/mfm-11-calibration-impact-v1`.
- Reviewed U1 HEAD: `45cad2cf7a01edb8c345772adc8c1dab45ceea10`.
- Operative run-5 gate HEAD: `f642550c9171a6f2ccf22825d0284cabd8d6c13e`.
- Read the revised `HANDOFF.md`, `Mfm11CalibrationImpactGate.cs` and `run_gate.py` in full; inspected `MEASUREMENT-DESIGN.md`, the first review, project entry/context/coordination instructions, and the relevant approved MFM specification sections.
- The release/authority documents are absent from this U1 checkout. Read their committed versions using `git show`: Release Repin at `7b579067`, Task Packet at `9d26846a`, and MFM-03 Manager Review at `cae6e601`. The release repins execution to the exact candidate above. The user's latest instruction assigns this replacement reviewer.
- Inspected run 5 `unity.log`, `command.json`, `process-result.json`, `evidence/comparison-table.txt`, and both final screenshots under `/media/jer/Files/CCF/Verification/MFM11-CalibrationImpact-20261101/unity/run5/`. Checked raw named markers against the table and code, not just the handoff. Inspected run 4's comparison table to verify the superseded horizon defect; read the offline repeat transcripts and confirmed byte identity. Earlier runs are historical, not qualification substitutes.
- Inspected production coverage classification/read/derivation, nursery purchase and planting designation, Work Plan approval, annual execution, objective evaluation and completion rules to establish what the gate actually exercises.
- Independently inspected PARITY-09 exact-candidate Reference, density-anchor and Legacy40 completion results/markers under `/media/jer/Files/CCF/Verification/PARITY-09-20261101/`; read its handoff and Claude review at `87aee15f` for reuse boundaries and known red gates.

`git diff --name-only 3acdf9cb HEAD` contains seven files, all in this task's handoff directory or `Tools/Verification/Mfm11CalibrationImpact/`. **Production, assets, packages, settings and protected resources are byte-unchanged from the candidate.** Run 5's recorded harness and runner SHA-256 values match the reviewed files exactly.

## Prior findings and disposition

### 1. Equal horizons — RESOLVED

Run 5's actual rows are `PATH_A|year=2->5`, `PATH_B_RESULT|year=5` and `PATH_C_RESULT|year=5`. Gate source captures one year-2 `baselineState`, advances A three years, reloads that state before B and again before C, and advances each treated path until `baselineYear + 3`. The B result is measured before its later completion continuation; C restores the year-2 state after that continuation. These are actual ecological years, not labels supplied from the unused `targetYear` helper parameter.

Raw log anchors: baseline at line 2044; A at 2230–2245; B at 2577–2649; C at 4418–4490. All paths begin from the same post-access/post-sample-release baseline: 1,177 living stems, 25.471506 m² BA and 776,854 cents.

Independently recomputed from the recorded decimal table:

| Difference at year 5 | B minus A | C minus B |
|---|---:|---:|
| Living stems | −84 | +75 |
| BA, m² | −2.188589 | +2.012923 |
| Canopy | −0.018095 | +0.017367 |
| Light | +0.018095 | −0.017367 |
| Occupied regeneration cells | +20 | −17 |
| Cash, cents | −182,544 | −63,257 |

The revised handoff's figures agree. Run 4 genuinely had A at year 5 and B/C at year 3; those treatment comparisons are superseded.

### 2. Timber and economy — RESOLVED for this measured run

B and C both call `ApprovePendingWork` before reading `GetHarvestQuote(true)`. B also reads the all-orders quote. Actual post-baseline accounting selects events with `year > baselineYear`, removing the prior cumulative-baseline confound.

Run 5 log lines 2487–2515, 2591–2605, 4356 and 4432–4446 establish:

- **B:** quoted sold volume 12,537,667 cm³, quoted and resolved-event revenue 67,456 cents, resolved contractor cost 250,000 cents, stock cost 0. Arithmetic: `67,456 − 250,000 = −182,544 = 594,310 − 776,854`.
- **C:** quoted sold volume 888,719 cm³, quoted and resolved-event revenue 4,199 cents, resolved contractor cost 250,000 cents, stock cost 0. Arithmetic: `4,199 − 250,000 = −245,801 = 531,053 − 776,854`.
- The difference in sale revenue is `4,199 − 67,456 = −63,257`, exactly the C-minus-B cash difference. Both jobs incur the existing 250,000-cent minimum; the much smaller manual job earns less revenue against that same cost. A has no cash change over the three idle years.

The table reports the single shared deadwood record at 0.127555 m³ on all paths, retained timber 0 for B/C, and sold timber rather than additional retained-deadwood outcomes. The ordinary production settlement path and the measured revenues support this account. The volume fields are **post-approval quotes**, not a separately logged cumulative post-execution sold-volume ledger; the handoff correctly labels them as quotes. Its claim of coherent timber/deadwood outcomes is now supported at that stated precision.

### 3. Completion and planting — RESOLVED

Raw log line 2826 records three successful planting designations for each species. The gate calls production `TryPurchaseStock` and `TryDesignatePlanting`; production purchase charges cash/records stock, designation creates `PlantJuvenile` orders with status `Pending`, `ApprovePendingWork` validates and approves them, and `AdvanceYear` resolves them through normal planting logic. The gate does not inject broadleaf trees, objective facts, outcome state, cash or completed planting orders. This is an ordinary **cell-based** planting API path, not a native-pointer or exact-position planting UX test.

The later evidence proves actual successful work and presence, beyond the designation counter: `ScenarioOneObjectives.Evaluate` requires a successful/completed planting record plus current species presence for each introduced-species objective. Both pass. Juveniles being zero at year 25 does not negate that result: presence can be in living trees or regeneration cells.

Log line 3253 records the second intervention, 10 trees at year 17. Line 3973 records `outcome=Completed outcomeYear=25 finalYear=25 cash=348152`. Lines 3987–4099 report **8 objectives, 0 unachieved**:

| Objective | Current | Target | Achieved |
|---|---:|---:|---|
| minimum-year | 25 | 25 | True |
| retained-canopy | 893 | 240 | True |
| continuous-canopy | 0.901015 | 0.35 | True |
| regeneration | 223 | 12 | True |
| fallen-deadwood | 85.732323 | 0.08 | True |
| managed-opening | 1 | 1 | True |
| introduced-beech | 1 | 1 | True |
| introduced-sessile-oak | 1 | 1 | True |

Production sets `Completed` only when all objectives pass and the reviewed two-cycle sequence passes. No change to those rules is present. The revised explanation is correct: missing planting made the earlier harness unable to complete; it was not evidence that wider treatment broke completion. This proves one long-run Path-B witness, not completion for every possible manual strategy.

### 4. MFM-03 live/cross-year residual — live decision check RESOLVED; post-treatment parity remains NOT DEMONSTRATED

The actual year-2 post-Lesson-5 extension state has source CI 8.059146, mean covered CI 7.723093 across 13 areas, relative delta **4.169829%**, within the accepted 20% GAMEPLAY CALIBRATION tolerance (log 2102–2147). Whole-stand DBH drift and live-state counts are recorded there and again at years 5 and 25.

The two later failures to form that comparison are real:

- Year 5: `NotCovered=1, Excluded=13, Manual=1`, **0 Covered areas**, with `PlayerCompletedWork=13` (log 2708–2783).
- Year 25: `ReadCoverage` refuses because the sample's own release is no longer qualified: Crop Trees 2/3, spread false, least executed release 0.2618706 (log 4171–4201).

Source confirms the mechanism. `ScenarioOneCoverage.Classify` records completed-work reasons for nonliving managed stems and gives exclusions precedence over similarity classification. `TryReadPrescriptionWorld` additionally requires `ScenarioOnePrescription.Derive` to return a qualified sample. The empty set and later refusal are not missing log lines or evidence of a numerical CI failure.

**Disposition:** accept this as a documented structural limitation for this bounded qualification, subject to explicit Manager acceptance. The MFM-03 Manager Review requires an exact live-lineage check that calibration remains reasonable at the actual post-Lesson-5/extension state; that is the year-2 check. The Task Packet also requires cross-year inspection of drift, whole-stand and relevant live state; those outputs and treatment consequences are now recorded through completion. It does not establish a rule that differently managed areas must remain within the original representativeness tolerance forever. Excluding completed work prevents using those areas as untreated comparable targets again.

The handoff's broad phrase “provably unmeasurable” needs this precise reading: **source-versus-current-Covered parity is unavailable through the accepted live read at these treated states**. Competition numbers are not inherently impossible to compute. At year 5 the pure classifier still measures excluded areas; one could track fixed area IDs/survivors, or measure the untreated A branch later, as a separate diagnostic. Such a diagnostic would answer a different question and must not relabel excluded areas as Covered or bypass sample qualification. I do **not** require it to repeat this actual-extension qualification, and I do not treat the current evidence as proof of post-treatment CI parity or arbitrary delayed/repeated-extension calibration robustness. The 4.17% selected-Covered mean is also not an independent validation of the 20% threshold; N1–N6 remain in force.

### 5. Protected compatibility — adequate scoped external evidence; Manager acceptance of reuse remains

MFM-11 itself proves only save/load self-consistency: `AAD4776628E3D45F` before and after the round trip (log 4230–4258). Its null LegacyV15/V16 values are not protected-pin passes, and the round trip is not a resumed-years continuation test.

Unlike the first reviewer, I opened the cited exact-candidate evidence. PARITY-09's `reference-legacy40/results-reference-legacy40.json` records head `3acdf9cb635cdca79606335801e97f9d7dcce8b0`, exit 0, `REFERENCE_FUTURE_V1_PASS`, no assertion failures and no unexpected tree changes. Its raw markers establish frozen archive integrity, tamper negative control, milestone previews and deterministic year-50 continuation:

- Milestone hashes: `A564039D9B7CE31D`, `F7DF7DAB53B6FD32`, `D5E75D6D21D631AC`, `7AD177B3CC2F73C7`.
- Schedule: `56C8B99FA1E8DDD1`; archive-file FNV: `1A42C7BD0439E72F`.
- Continuation: current hash `EBD49ADEF18C3F36`, historical v15-layout hash `BFFF66015A6619DA`, deterministic true.

For broader Legacy40 coverage, PARITY-09's dedicated `density-anchors-sitka` PASS reproduces `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` and 24/24 exact controller victim sets. Its `completion-model2-legacy40` PASS reproduces protected layout `9C77EB4BEE427FB9`, completion year 25, and resumed/uninterrupted continuation equality. These are stronger support than a Reference marker alone.

**Disposition:** the protected condition can be supported by these scoped external checks plus unchanged production bytes and MFM-11's own save/load witness, once the Manager explicitly accepts the evidence reuse. **No new pin, rebaseline or duplicate Unity run is required by this review.** Do not describe the entire PARITY-09 suite as green: its mixed-density gate has a stale save-version assertion, timber gates abort on stale pins, and its handoff/review record other verification debt. Those broader PARITY-09 acceptance decisions are not silently resolved here; the cited Reference and dedicated anchor/completion runs themselves passed.

### 6. Handoff corrections — RESOLVED, with interpretation limits above

The canopy delta is corrected, run 4's unequal horizons are explicitly withdrawn, the cost anomaly is reconciled, the completion cause is corrected, and the two gaps are openly retained. `QUALIFICATION INCOMPLETE — GAP` is an honest worker status pending Manager disposition. There is no false success token in the revised verdict.

The separate measurement-design draft describes a wider manual comparator than the live gate: offline C uses the candidate set, whereas live C has only the three baseline sample Crop Trees and nine manual marks. The operative handoff/table expose the actual nine-mark path. I accept it as a plausible bounded manual alternative under the Task Packet, not as an equivalent-intensity manual implementation of B. The two final screenshots show restored Path C with cached treatment-review messages (including zero sample metrics/earlier 84-task text); they do not prove B completion or authoritative fresh UI state. This quantitative review makes no new rendered-UX or native-input qualification claim.

## Run integrity and cleanup

Run 5's raw marker is `MFM11_UNITY_PASS checks=47 shots=2`; I independently counted 47 check markers. Its process receipt records exit 0, 322.4 seconds, no failures, harness/licence cleanup and no remaining lane changes. The marker is not blanket proof of every pass condition: several gaps and reconciliation values are notes rather than hard assertions, and completion failure would currently be logged rather than fail the gate. Approval here relies on the inspected values for this exact run, not a claim that the harness automatically enforces the whole matrix on future runs.

Recomputed SHA-256 values match the handoff:

- `unity.log`: `89f61be02775d5549cfccca4266bdb56f5f4f2257a06101bce48aa7084ebdf95`.
- `comparison-table.txt`: `e3ea9b1c29e2dcd3ac9be2c1282a7b9f2c8decc30717132fdfe1a12346cebb07`.
- `process-result.json`: `09ac810227729c2f4ac64d67f56c0350227d356abedbc4fa250c0c87a1ccad45`.

At review preflight U1 was clean, including untracked files, at `45cad2cf`; Primary was also clean. Read-only `unity-lock status --json` reported `status: free`, metadata null and `unity_editor_pids: []`. No Editor, Xephyr or openbox was running, and neither staged gate/native-input harness nor their generated `.meta` files was present under `Assets/ForestPrototype/`. This review necessarily leaves its new Markdown file untracked for the orchestrator; it does not leave a dirty production lane or staged harness.

## Token supportability and exact remaining action

**`MULTI-SCALE CALIBRATION QUALIFIED` is supportable after Manager acceptance.** My recommendation is to accept the measured actual-extension check, explicitly retain post-treatment source-versus-Covered CI parity as **NOT DEMONSTRATED**, and accept the scoped exact-candidate PARITY-09 compatibility evidence. There is no measured material accepted-outcome break warranting `BLOCKED — CALIBRATION IMPACT`, and no additional mandatory measurement run or production retune is requested by this re-review.

The remaining action is a **Manager disposition**, not worker self-acceptance: record those two evidence-boundary decisions, accept/reject the qualification on this exact unchanged production candidate, and only then issue the success token/release downstream work as authorised. If the Manager instead requires arbitrary later extension-calibration qualification, that needs a bounded diagnostic brief with explicit target states and comparison sets; the current 4.17% value must not be stretched to answer it.

**I did not edit production, tests, thresholds, baselines, the handoff or the coordination ledger. I did not run Unity, self-accept the task, issue its token, commit, push, touch #194 or make a Windows build. The only file I wrote is this round-two review.** Publication/commit and any coordination/dashboard update remain with the orchestrator.
