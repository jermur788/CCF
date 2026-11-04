# MFM-06 — Save / provenance foundation — HANDOFF

**Status:** implementation and qualification complete on the exact candidate below. **Independent review (#112, Claude Opus 5.5 Low) is still required before integration (#113).** The strict success token is therefore withheld; see the end.

## Identity

- Actual model/provider: **Claude Sonnet 5.5 (Claude Code via T3)** — *not* the routed OpenAI 6.1 Sol. The user reported Sol capacity exhausted and directed this session to execute the task. DeepSeek V4.1 Flash (opencode-go) was used for two mechanical helpers (runner adaptation; 22→23 pin relabel), both reviewed by the implementer. The independent reviewer must not be this thread.
- Branch / worktree: `feature/mfm-v1-save-provenance-v23` / `/media/jer/Videos/CCF-MFM-06`
- Starting HEAD: `ac876a64969752a3864034521f42559671264b30` (local umbrella head, unpushed at start; clean).
- Candidate SHA: `60f17ef95fb2fca2c04bc61e38ff10bedc1857d8` (this handoff is committed after it; production and verification code are identical to 60f17ef).
- Starting save version **22** (confirmed on ac876a6) → resulting **23**. MFM-06 owns v23.
- The branch is **not pushed** (pushing would also publish the ~42 unpushed umbrella commits beneath it); the reviewer reads the local worktree.

## Changed files (Assets, 10)

`ForestSaveData.cs` (v23, DTOs, enums), `ForestSaveValidation.cs` (semantic + JSON validators), `ForestSaveController.cs` (JSON check in `TryReadSave`, shared by Load and CanLoad), `ScenarioOne/ScenarioOneManager.cs` (capture/restore/new-game wiring), `ScenarioOneManager.Prescriptions.cs` (+.meta; storage only), `ScenarioOneSaveMigration.cs`, `ScenarioOneWorkOrder.cs`, `ScenarioManagementEvent.cs`, `ScenarioReferenceArchive.cs` (hash layout/canonicalisation). No scene, prefab, ProjectSettings, Reference resource or package file changed.

Tooling (new): `Tools/Verification/MfmSaveProvenance/` (offline check, mutation controls), `Tools/Verification/MfmSaveProvenanceUnity/` (compile/hash-dump/save gates + runner). Existing gates: 26 files relabelled 22→23 (precedent `d5dbe97`), and the three independent v20-layout comparators (`StandGeometry`, `RackV2Persistence`, `RackV2Save`) additionally strip the default v23 bytes, same rule as for v22.

## Schema (all inside `scenarioOne`, declared last and adjacent)

| Field | Meaning |
|---|---|
| `nextPrescriptionId` (default 1) | allocator; ids never reused |
| `prescriptions[]` → `prescriptionId`, `ruleVersion`, `calibrationVersion`, `sourceRackId`, `sourceYear`, `sourceAnchorX/Z`, `sampleCropTreeIds[]`, `sampleRemovalIds[]`, `coverageDecisions[]{unitId, choice}`, `approvalState` (1 Accepted, 2 Approved), `approvalYear` (-1 until approved) | spec §14 persisted set |
| order: `prescriptionId` (0 = ordinary), `prescriptionCoverageUnitId`, `prescriptionCropTreeId`, `prescriptionReason` | J1/J2/J3/I6 work-order provenance (rule version and source sample resolve through the prescription record) |
| event: `prescriptionId` | history linkage |

Not persisted: proposals, candidate markers, preview visuals (K6). Crop Tree decisions stay in the existing `cropTreeIds`.

## Compatibility rules

- Missing fields / any save < v23 ⇒ no prescription state. `NormalizePrescriptions` + `RestorePrescriptions` (version-gated) never derive anything from marks/orders; ordinary orders must carry no provenance (validator rejects it).
- A pre-v23 document that *claims* prescription state is refused.
- v23 documents must state the fields explicitly with correct JSON types, no duplicate keys.
- All validation (typed `Validate` + `ValidatePrescriptionsJson`) runs before any world mutation, and runs last so every older rule keeps its existing refusal reason/order. CanLoad and LoadData use the same `FindLoadProblem`; the disk path adds the JSON check in the shared `TryReadSave`.
- Hash identity: while a world has no prescription state, every world-identity hash (`WorldHash`, `CurrentWorldHash`, `LegacyCompatible`, V15–V18) strips the exact default v23 text and relabels the header exactly as before, so **no anchor is rebaselined**. A world that carries prescription state hashes its full current JSON and has no V15–V18 layout. The v22 `objectiveAchievements` strip and rack-plan rules are unchanged.
- Reference Future v1 resources: byte-identical vs ac876a6 (runner `git diff` check). Legacy40: covered by hash fixtures and the historical gates below. No production code creates a prescription (audited; mutant M22).

## Verification (all on the candidate unless stated)

| Gate | Result |
|---|---|
| Offline `run_offline_check.py` | `MFM06_SAVE_OFFLINE_PASS checks=169`: every pre-v23 fixture world (incl. parsed v21/v22 inputs, v22 history, rack state) reproduces the **ac876a6 baseline hashes byte for byte**; round trip of each field; absent-fields; stable ids; ~70 refusals each with a no-mutation check; JSON presence/type refusals; validation order; canonical ordering; wiring audits; Reference resources unchanged |
| Mutation controls `run_mutation_controls.py` | 23 deliberate defects, **23 killed, 0 survived** (first run's M23 was killed only by a compile error; the mutant was made semantic and the full set re-run) |
| Unity import/compile | PASS (`02`/`01-compile`, Unity 6000.6.0f1) |
| Unity `save` gate | `MFM06_UNITY_PASS checks=155`: real ForestSaveController; v23 in-memory + disk round trip byte-identical ×3 (stable ids/hash); v22-shaped file loads with world JSON and world hash identical to the ordinary world and no prescription/provenance on any order or event; 23 malformed v23 documents refused by CanLoad and Load with world/hash/trees/wood/prescriptions/links unchanged (every document also perturbs `wood` so partial application would show); 7 typed corruptions refused by LoadData with no mutation; good file loads afterwards |
| Unity `hash-dump` | 18 fixture lines through Unity `JsonUtility`, candidate vs base `ac876a6`: **identical** |
| Protected historical gates (Legacy40 selected as in S1-C) | `reference` PASS, `hardening` PASS, `interaction` PASS, `completion` PASS, `completion-model1` PASS, `n3` PASS |
| `progress` | harness marker `SCENARIO_ONE_PROGRESS_VERIFY_PASS`, Unity exit 0; runner reports `pass:false` solely because the gate left an isolated `forest-save.json.bak` (runner is strict about disk saves for non-`save` gates; host saves unchanged) |
| `density-save` | `DENSITY_QUAL_PASS` marker present; Unity did not exit and hit the 1500 s runner timeout after the pass marker (runner `pass:false`; no failure lines). Same post-pass hang pattern S1-C documented; **not** a clean exit and should be treated as residual |

Evidence: `/media/jer/Files/CCF/Verification/MFM-06-Unity/run2/` (`01-compile`, `02-save`, `03-*`, `04-base-hash-dump`). `run1` holds a first save run that failed on a null-`detail` bug in the disposable harness (fixed in 60f17ef) and a compile pass on 03790ab.

Cleanliness: U1 clean (0 changes) and restored to detached `ac876a6`; machine-wide Unity lock released (free, no Editor); harness/meta/licence copies removed by the runner; host saves unchanged in every run; no Windows build.

## Residuals / for the reviewer

1. `progress` and `density-save` runner `pass:false` as above (marker passes). Re-run on U1 if the integrator wants a clean exit.
2. Gates not run: `ScenarioOneInteraction` beyond the existing forwarder; Enlarged80 hash anchors are covered only by fixtures (hash-dump) and the `reference`/`n3` gates. Other relabelled pin gates (26 files) were relabelled but not executed (T-05 precedent ran a chosen subset).
3. Offline dotnet checks do not replace Unity; the Unity `save` gate stamps prescription state onto captured save data (no production creator exists until MFM-07).
4. `sourceRackId` is validated as an allocated rack id (`1 ≤ id < nextRackId`) and a Rack V2 world, not that the rack still exists (racks can be edited after approval; provenance must survive).
5. `PrescriptionReasonCode` mirrors `ScenarioOnePrescription.ProposalReason` (so the validator stands alone in offline harnesses); a sync assertion lives in the offline check.
6. Candidate not pushed; model deviation from Sol routing (see Identity).

## Review request (#112)

Claude Opus 5.5 Low, independent of this thread: inspect `git diff ac876a6 60f17ef -- Assets`, K1–K10 + J/I6, validation-before-mutation ordering, fabricated-provenance risk, v22 S1-C history preservation, hash/comparator handling, and re-run `run_offline_check.py` / `run_mutation_controls.py`. Verdict to `Docs/Handoffs/MFM-06/INDEPENDENT-REVIEW.md`.

**Status token not issued.** Implementation and qualification complete; independent review #112 pending. After an APPROVED review the token is: `PRESCRIPTION SAVE FOUNDATION QUALIFIED — READY FOR INDEPENDENT REVIEW` (as requested) / `…READY FOR MANAGER INTEGRATION REVIEW` (packet wording).
