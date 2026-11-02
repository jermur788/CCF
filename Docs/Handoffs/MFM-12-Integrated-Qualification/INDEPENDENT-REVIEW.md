# MFM-12 (#225) — Independent cross-provider review

**Verdict: `NEEDS CHANGES`**

The qualification did not reach a PASS and the STOP was correct. S1 is a real production/spec defect, confirmed independently from source. The product needs a repair task, then a fresh qualification. This is not `REJECTED`: the rest of the inspected surface is consistent, and the defect is a missing presentation and wiring layer over a core that already exists and is tested offline. It is not a fault in the core.

Reviewer: Claude Sonnet 5.5 (`claude-sonnet-5-5`), Anthropic. The qualification worker and the static analyst were both OpenAI gpt-6.1-sol, so this review is cross-provider to both. Date: 2026-11-02. Read-only: no Unity, no lock, no repository edits, no repair. The only file written is this one.

---

## 1. Identity and ancestry (verified by command)

| Item | Result |
|---|---|
| Worktree / branch | `/media/jer/ZX20/CCF-U1` / `qual/mfm-12-integrated-v1` |
| `git rev-parse HEAD` | `95c12be59f7db0253a524cbaf923c1cc03671911` ✔ |
| `git status --short --untracked-files=all` at review time | empty ✔ |
| `git diff 508b3e81..95c12be5 -- Assets Packages ProjectSettings` | **empty** ✔ (the HEAD commit changes 11 files, all outside those paths: verification tooling and binding docs) |
| `3acdf9cb` ancestor of `508b3e81`; `508b3e81` ancestor of `HEAD` | both true ✔ |
| `git diff --stat 3acdf9cb..508b3e81 -- Assets Packages ProjectSettings` | exactly one file: `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` (13+/10−) ✔ |
| Commits `3acdf9cb..95c12be5` | `ad9a8f76` (#223 tooling), `7e7ba702`, `bbcefe06`, `95db20ae` (#224 USS), `9a4f8014`, `508b3e81`, `95c12be5`: matches the static analysis table |

Claim 3a holds: the only production change since accepted MFM-11 (`3acdf9cb`) is `ScenarioOneUi.uss`.

---

## 2. Blocking finding

### S1 — Source-sample fit check (spec §7.3) is never shown to the player — CONFIRMED, real production/spec defect

**Requirement.**
- `spec.md` §7.3 (lines 192-202) says: "Before wider approval, apply the derived rule back to the original sample and show how closely it reproduces the player's own treatment." The player must at minimum be able to see (a) an agreement/disagreement count or equivalent, (b) which sample decisions the rule would have made differently, and (c) relevant Share/removal consequences.
- The same obligation appears in §6 lesson-6 bullet 2 ("report how well the derived prescription reproduces the player's own sample decisions"), §16.5 ("The player can navigate to disagreements…") and §21 acceptance criterion 4 ("Fit check exposes disagreements with the player's sample").
- Checklist E4, E5 and J7 (`requirements.md:53,54,107`) carry the same obligation. The checklist boxes are all unticked, so nothing is pre-credited.

**What exists.**
- `ScenarioOnePrescription.FitResult` (`ScenarioOnePrescription.cs:143-171`) has `Agreements`, `PlayerOnly` and `RuleOnly` (lines 148-150), per-Crop-Tree `PlayerShare`/`RuleShare` and basal-area shares.
- `CheckFit` (lines 174-180 and 184-271) is a pure function. It is correct against the offline checks (`PrescriptionCoreOfflineCheck.cs:558-614`, step 07 PASS).

**What is missing: confirmed by my own search, not taken from the analysis.**
- `rg 'CheckFit|FitResult|PlayerOnly|RuleOnly|Agreements|Disagree' Assets` (excluding `.meta`) finds hits **only in `ScenarioOnePrescription.cs`**:
  - the definitions;
  - the internal forwarding overload call at line 179;
  - the `Agreements`/`PlayerOnly`/`RuleOnly` use at line 256.
- The only other callers are the offline harness under `Tools/Verification/`.
- Searching `Docs` and `specs` for "fit check" finds nothing besides spec/tasks/checklist text. There is no deferral note and no waiver.

**Manager path.**
- `CreatePrescription` (`ScenarioOneManager.PrescriptionApproval.cs:42-85`) calls `TryReadPrescriptionWorld`, which calls `Derive` (`:264`). It never calls `CheckFit`.
- It persists only `sampleCropTreeIds`, `sampleRemovalIds`, the coverage decisions and the approval state.
- There is no fit accessor on `ScenarioOneManager`.
- `ApprovePrescription` (`:118`) has no fit step.

**UI path.**
- The Stand Map treatment-review panel is built by `StandMapTreatmentReview.cs`, in this order:
  - `BuildSample` (`:207-214`) shows only the sample title and its metrics, plus "It is the source, not a target".
  - Then the legend, then `BuildAreas`, `BuildCandidates` and `BuildPrescription` (`:335-374`), which contains `BuildSpotCheck` (`:376`), `BuildProposals` (`:395`) and `BuildApproval` (`:418`).
  - None of these shows an agreement/disagreement count, player-only or rule-only trees, per-Crop-Tree Share, or removal/basal-area comparison.
- `rg 'Derivation|TargetShare|BasalCeilingShare|PlayerMinimumReleaseShare|SampleRemovedBasalShare' Assets/ForestPrototype/UI` returns nothing. The player is not shown even the derived rule's numbers.
- `ScenarioOneTreatmentReview.EvaluateGate` / `ApprovalGate` (`:433-453`) gates approval on: has a prescription, not already approved, preview valid, has proposals, spot-check done. Fit is not in the gate. "Before wider approval" is therefore not enforced either.

**Misleading player copy.** `UI/ScenarioOneTeachingSequence.cs:79` tells the player that "The game derives a prescription from the decisions you actually carried out, **checks it against your own sample, and shows** which rack-served areas are covered…". The check is never executed in the player flow, and its result is never shown. The copy promises a behaviour that does not exist. The spec treats the fit check as an honesty device, so this is a player-facing inaccuracy as well as a missing feature.

**Failure scenario.** A player completes the sample lesson and derives a rule that differs materially from their own decisions. For example, they removed a weak or distant tree the rule would not, and kept a strong competitor the rule would take. They open Treatment review, choose areas, prepare, spot-check and approve. At no point are they told the rule would have treated their own sample differently. Spec §21.4 and checklist E4/E5/J7 are unmet.

**Classification.**
- It is a product/spec defect: missing integration and presentation, plus a missing approval-path step.
- It is not verification-tool debt and not merely "unrun evidence".
- It also reveals a task-decomposition gap. `tasks.md` assigns fit only to MFM-04 ("fit disagreements" in offline tests, line 144). MFM-05 to MFM-09 do not list fit presentation. MFM-08's map list (sample, coverage, candidates, proposals, reasons/exclusions) omits it. Each task passed its own gate while the §7.3 end-to-end obligation fell between them.
- Fix scope belongs to Browser Overall. I make no repair and prescribe no design beyond the spec's minimum. Whatever repair lands must respect the existing invariants: no new automation, proposals not Fell marks, Work Plan remains the commitment gate.
- After repair, the exact-candidate chain re-qualifies. The UI/manager change touches surfaces that MFM-08, MFM-09 and the save/provenance work were qualified against. The inherited claims for those surfaces would then need re-evaluation under the "exact path set" rule used in STATIC-CONFORMANCE.

---

## 3. Non-blocking findings and limitations

**N1: Step 13 note understates what did not run.**
- The `summary.json` note and `failure-step13-classification.json` say "13/13 applicable mutants killed; M14 stale…".
- `run_mutation_controls.py` defines **25** mutants (M01-M25). On a stale M14 text the runner executes `return 2` at the top of the loop, so **M14-M25 (12 mutants) were never exercised**: M14, M15-M21, M24, M25, M22 and M23. Only M01-M13 ran.
- The log confirms it: the last line is `MUTANT TEXT NOT UNIQUE/PRESENT: M14…`, and there is no `mutants=… killed=…` summary line and no `MFM06_MUTATION_CONTROLS_PASS`.
- "13/13 applicable" is technically true for the 13 that ran, but it reads as if only M14 is missing. M15-M25 (default provenance, migration, hash ordering, disk-read prescription JSON check, restore) may be valid and are simply unevidenced.
- The M14 text staleness itself is verified: the mutation target is `CurrentVersion = 23;` (`run_mutation_controls.py:34`), but `ForestSaveData.cs:7` has `CurrentVersion = 24;`. This is verification-tool debt. No product defect is inferred. ✔
- The note's "save rejection covered by Unity save gate" refers to a gate that **did not run**. Until it runs, M14-M25's concerns are unevidenced in this package, not covered.
- Status in the package is `ACCEPTED_LIMITATION`, not PASS, which is correct. The wording should say "M01-M13 ran; M14-M25 not executed".

**N2: Step 11 `ACCEPTED_LIMITATION` is correctly a non-pass, with no product defect inferred.**
- `run_treatment_review_offline.py` pins `START = cfdc93bf…` (the MFM-08 start) and fails any file changed since then outside its MFM-08 allow-list (`boundary()` lines 41-50).
- On this lineage that always fails. The listed outside files are later accepted work: save, S1-D, UI and the Docs handoffs.
- The behavioural half ran 297 checks with 0 failed. The repeat run was byte-identical.
- The `failure-classification.json` category "verification tooling/runner problem" is supported. This is the stale boundary pin.
- Caveat: the 297/0 behavioural pass exercises the pure `ScenarioOneTreatmentReview` / `StandMapTreatmentReview` logic. It cannot show that a fit check exists, and it does not. Do not cite step 11 for E4/E5/J7.

**N3: Step 04 (three pure-Beech R2 misses) is an accepted, unchanged calibration limitation.** It is correctly recorded as `ACCEPTED_LIMITATION`, not PASS.

**N4: Held Unity lock.**
- `lock-after.json` and `offline-stop-lock-after.json` show the machine-wide lock `c2caa8620ee6436aaac675f597bd2574` (owner `codex-gpt-6.1-sol-MFM-12`) still **held**, with `pid: null` and no Editor processes.
- The coordinator was SIGKILLed after compile cleanup.
- If the lock is not released, it blocks all other heavy Unity work until the ETA (2026-11-03T16:00Z). I did not release it, since I am read-only. Browser Overall or the local orchestrator should release it deliberately.

**N5: Compile-gate side effect.**
- `unity/01-compile/editor-side-effects.patch` records that Unity rewrote the tracked `.vscode/settings.json` (a trailing comma and `dotnet.defaultSolution` changed to `CCF-U1.slnx`). It was reverted: `remaining_lane_changes: ""`, and the lane was clean at HEAD in `after-state.json` and at my review.
- `AGENTS.md` flags `.vscode/settings.json` as tracked shared configuration. The side effect is disclosed and cleaned, not a finding against the product.

**N6: Spec §15, "what fit was reported".** `PrescriptionSaveData` (`ForestSaveData.cs:322-343`) persists no fit result. Spec §15 says history must "eventually" be able to answer this, and it asks only for architecture support, not first-slice persistence. The persisted `sampleCropTreeIds` / `sampleRemovalIds` and the deterministic `CheckFit` make it recomputable. Any S1 repair should decide this deliberately rather than leave it implicit.

**N7: Static artefacts are drafts.** `STATIC-CONFORMANCE.md`, `GAPS.md` and `checklist-matrix.json` are explicitly drafts with every FRESH LOCAL REQUIRED row `PENDING`. I sampled E4/E5/J7 and the inheritance claims, and they match the source I read. I did not re-audit all 237 matrix rows.

---

## 4. Was the STOP correct?

**Yes.**
- The handoff's own stop conditions say to stop and return to Browser Overall before any repair "if any production/spec defect is found".
- S1 is an unambiguous production/spec defect that fails a §21 acceptance criterion directly.
- Continuing the Unity matrix would have produced rendered evidence for a product that cannot pass E4/E5/J7, and would have held the machine-wide Unity lock for no benefit.
- The worker did not repair or weaken anything. The Unity compile child was allowed to finish and clean up (`compile_pass: true`, harness and licence copy removed, host saves unchanged). Only the coordinator was killed, and no Editor was killed.

Minor process note: the worker states it "did not run a rendered gate to independently demonstrate that UI absence". That is honest. My finding does not depend on a rendered gate, because the UI is built entirely from the source I read and there is no fit element in it.

---

## 5. Honesty of the partial evidence package

No PASS claim in the package exceeds the evidence I could check, with the wording caveat in N1:

- `summary.json` records exactly one Unity step (`01-compile`, PASS, with a process-result receipt and `MFM06_COMPILE_PASS … saveVersion=24` in the log).
- Offline steps 04, 11 and 13 are `ACCEPTED_LIMITATION`; the others are PASS with logs.
- `screenshots-manifest.txt` correctly reports **0 PNGs** at both resolutions and no rendered gate.
- `GAPS.md` and `STATIC-CONFORMANCE.md` explicitly say "not an all-PASS matrix" and issue no product token.
- The handoff-required durable `HANDOFF.md` / `INDEPENDENT-REVIEW.md` have not been published to the repo. `Docs/Handoffs/MFM-12-Integrated-Qualification/` contains only the three binding docs. That is consistent with an incomplete qualification.

Things that are PASS, with their scope:

- **Compile**: PASS means the project compiles under Unity 6000.6.0f1 at the exact HEAD.
- **Offline PASS steps**: pure-logic, Unity-free checks on the candidate source (site, growth, timber, history, pruning, prescription core, candidates, coverage, approval boundary, save/provenance, parity08). They are not runtime or rendered evidence.
- **Offline step 07**: it passes only the *core* `CheckFit`. It is the evidence that makes S1 a wiring/presentation gap rather than a logic gap, and it must not be read as E4/E5 evidence.

---

## 6. Inheritance bounds (MFM-11 / MFM-10 / MFM-08 / MFM-09)

- **MFM-11.**
  - The "only `ScenarioOneUi.uss` changed" claim is verified (§1).
  - The MFM-11 calibration claim is correctly bounded. The extension-state CI delta of 4.169829% across 13 Covered areas is within the 20% gameplay-calibration tolerance.
  - **"Post-treatment source-vs-current-Covered CI parity remains NOT DEMONSTRATED"** is stated in the task handoff and kept in STATIC-CONFORMANCE. Nothing in the package promotes it. The qualification never reached the stage that could.
- **MFM-08 / MFM-09 / MFM-10.**
  - The static analysis uses per-row exact path sets and empty `git log <prior>..HEAD -- <surface>` checks, not blanket inheritance. Rendered UI is always fresh.
  - I verified the one fact that matters for the USS change: the only production delta since `3acdf9cb` is a stylesheet.
  - S1 undercuts inheriting MFM-08/09 end-to-end. Their gates passed on their own scope, but together they omit §7.3. After a repair that touches `StandMapTreatmentReview.cs`, `ScenarioOneManager.PrescriptionApproval.cs` or the teaching copy, their exact-path inheritance no longer holds for those surfaces.

---

## 7. Evidence gaps the documents might overstate

None of these is a product defect; all are unrun or incomplete evidence.

1. **The whole Unity matrix beyond compile did not run.** Not evidenced: save/load and old-save and malformed-data rejection before mutation; Legacy40 and Reference Future v1 live preservation; rendered 1280×720 and 1920×1080 distinctions; map and 3D interaction; Work Plan boundary at runtime; provenance through execution, Annual Review and history; S1-C/S1-D coexistence across two cycles; guided-v2; #199 minimap and focus contracts; and the **complete-loop Scenario One witness**. The handoff requires the complete-loop witness explicitly, and none exists.
2. **The mutation controls M14-M25** (N1). These include the Unity-independent save-validation mutants. They are unevidenced here, not "covered by the Unity save gate".
3. **Matrix rows flagged by the analyst's own GAPS.md**, which remain open even without S1: C3, D2, D3, D6, D8, D9, D10, F9, F10, F11, G3, M6.
4. **User-playtest-dependent items** are out of this role's scope: F11 and P6.
5. **Pure-Beech R2 limitation** (step 04) stands as a C-level limitation, 0.40 unchanged.

---

## 8. What I did not verify

- I did not run Unity, the offline harnesses, or any gate. I read logs and receipts only, and did not re-execute step 07, 11, 13 or the Unity compile.
- I did not re-audit all 237 rows of `checklist-matrix.json`, or the whole 133 kB of STATIC-CONFORMANCE. I verified S1, the lineage and inheritance claims, the E4/E5/J7 rows, and the evidence files listed in the brief.
- I did not independently verify the inherited MFM-08/09/10/11 Manager decisions in `origin`. I relied on the static analysis's citations of them and on the local path-diff results.
- I did not verify rendered behaviour, the 3D flow, saves, Work Plan runtime behaviour, economy, or any Reference Future / Legacy40 fingerprint.
- I did not check the claim in the brief that the host saves were unchanged beyond reading `process-result.json` (`host_saves_unchanged: true`, `disk_saves_created: []`).
- I did not check GitHub claim or coordination state for #225.

---

## 9. Summary for Browser Overall

- **Verdict: NEEDS CHANGES.**
- **Blocking: S1.** The Spec §7.3 fit check exists as a tested pure function (`ScenarioOnePrescription.cs:143-273`) but has no production caller and no UI. It is not in the approval gate (`ScenarioOneTreatmentReview.cs:433-453`), not in `StandMapTreatmentReview.cs:207-430`, and not in the manager approval path. The teaching copy (`ScenarioOneTeachingSequence.cs:79`) claims it exists. This fails checklist E4/E5/J7 and acceptance criterion §21.4.
- **STOP was correct.** No PASS claim exceeds the evidence, with N1 as a wording correction.
- **Follow-ups for the repair/requalification task:**
  - fix S1;
  - re-run the unrun Unity matrix against the repaired exact candidate;
  - repair the stale tool pins (step 11 `START`, step 13 M14) via a separate tooling change, so M14-M25 actually execute;
  - release the held Unity lock deliberately.
