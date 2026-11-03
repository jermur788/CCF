# MFM-12 — Integrated Qualification — Independent Review

**Claim:** #225
**Production candidate:** `e42bb9f437e4fcf20c5b753dacd1136470daf33e`
**Qualification HEAD reviewed:** `0cd7b5ffceaf554c014ff0d95ede7beb17d565cb`
**Review mode:** read-only. The reviewers did not run Unity, acquire the lock, stage harnesses or edit the repository.

## Reviewer identity and independence

| Round | Reviewer | Independence |
|---|---|---|
| 1 — full architecture/spec-conformance review | Claude Sonnet 5.5 (`claudeAgent`), cross-provider | Independent of the qualification and #230 repair threads. Returned **NEEDS CHANGES**, evidence/receipt only. |
| 2 — delta confirmation | OpenAI GPT-6.1-Sol (`codex_c75d8114…`), cross-provider | Independent of the qualification evidence producer. Substituted for Claude after the Claude API quota was exhausted. Did not produce this qualification evidence. Was not the reviewer of the #230 repair; the substantive architecture/spec review remains the Claude round 1. |

Claude's round-2 attempts failed with "Claude API rate limit reached" after the round-1 review had been completed. The OpenAI delta review explicitly records this substitution below.

## Final verdict

**APPROVED — INDEPENDENT REVIEW APPROVED FOR MANAGER ACCEPTANCE.**

The round-1 review found no production/spec/save/calibration/protected-anchor/player-judgement defect and no task-packet stop condition. Its three blocking findings were evidence and receipt defects, not product defects. All three were fixed and independently confirmed in round 2.

The product token `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` is **not** issued by this review. It remains Browser Overall's decision after Manager review. P5 publication and P6 personal playtest remain outstanding.

## 1. Identity / ancestry verified

Round 1 reproduced:

```text
$ git -C /media/jer/ZX20/CCF-U1 rev-parse --abbrev-ref HEAD; git rev-parse HEAD
qual/mfm-12-integrated-v2
85051922e394bf4a4723946dc97d9b2fddc136bc
$ git ls-remote origin refs/heads/integration/scenario-one-player-test-ready-v1
e42bb9f437e4fcf20c5b753dacd1136470daf33e
$ git merge-base --is-ancestor e42bb9f4 85051922 && echo ancestor
ancestor
$ git diff --stat e42bb9f4..85051922 -- Assets Packages ProjectSettings
(empty)
$ git diff --stat 508b3e81..e42bb9f4 -- Assets Packages ProjectSettings
4 files, +213/-17 (the accepted #230 repair)
$ git diff --stat a5a1483f..e42bb9f4 -- Assets Packages ProjectSettings
(empty)
$ git diff --stat 508b3e81..e42bb9f4 -- Assets/ForestPrototype/ScenarioOne/ScenarioOnePrescription.cs
(empty; CheckFit core unchanged)
```

Round 2 reproduced the same ancestry at `0cd7b5ff`, confirmed the production diff is empty, and confirmed save schema v24 at both evidence heads.

The qualification branch adds only verification tooling and docs:

- `Tools/Verification/Mfm12IntegratedQualification/` (matrix, runners, accepted MFM-11 witness);
- a treatment-review boundary allow-list extension;
- the newest-licence selection in the three-species runner;
- the bounded post-verdict reaper in the calibration runner;
- the durable HANDOFF and this review.

`Mfm11CalibrationImpactGate.cs` is byte-identical to `origin/verify/mfm-11-calibration-impact-v1` (sha256 `f4d3a536…006f`). `run_extension_gate.py` differs from the accepted MFM-09 runner only in task id.

## 2. Round 1 — full independent review (Claude Sonnet 5.5)

### Verdict

**NEEDS CHANGES — narrow, evidence and receipt only.** No production, spec, save-schema, calibration, protected-anchor or player-judgement defect; no stop condition triggered.

### Blocking findings

**B1 — Guided-v2 rendered evidence was blank.**
- All 47 PNGs in `attempt3/unity/10-guided-v2/evidence` were fully transparent (alpha max 0); every 1280×720 file was 16,170 B and every 1920×1080 file 36,314 B.
- The gate's 535 checks are in-memory layout/behaviour assertions; it has no pixel or size assertion, so the blank captures did not fail the gate.
- The HANDOFF's "all required gates assert visible pixels / 154 PNGs" claim was therefore false.
- The accepted #230 guided-v2 run on identical production bytes produced normally rendered captures, so this was not a general product rendering failure.

**B2 — Lock/cleanup receipt was stale.**
- The HANDOFF said the lock was released/FREE and nothing was running while the guided-v2 recheck was still holding lock `98c789a0…` in the lane.
- No ownership violation was found; the same owner legitimately held it. The receipt was simply not true at review time.

**B3 — The draft pre-asserted approval.**
- The HANDOFF contained "INDEPENDENT REVIEW APPROVED" before any review verdict existed.

### Non-blocking findings (round 1)

- **N1** — rule-only and invalid-fit presentation are not live/rendered: the live fixture is `agree=18 playerOnly=1 ruleOnly=0`, the rule-only loop is vacuous, the invalid case is refused by earlier sample validation, and `Refusal.FitUnavailable` is referenced only in production (no gate).
- **N2** — treatment-review captures at scroll-top hide the Approve control; `08-conflict-manual-fell` and `08b-conflict-open-work` are byte-identical at each size. Pending vs approved is visible in step 07 at both sizes.
- **N3** — step 17 is Legacy40 history, not MFM prescription history; the witness marks Annual Reviews and candidates programmatically. What is shown on the MFM path is approval-time history linkage, provenance through Pending → carried over → Work Plan approval → execution, and a later intervention at year 17. No single MFM world demonstrates two-cycle Annual Review/history.
- **N4** — wording nits: `R2` is single-year all-stem loss, not R²; witness cash is cents; the 154 breakdown omits the extension gate's 16 and double-counts fit inside 29; `mfm11-final` is post-approval, not completion-year; P5/P6 and F11 needed clearer open status.
- **N5** — offline `summary.json` was overwritten by the Unity attempt and the offline phase does not gate on `--head`.
- **N6** — protected-anchor coverage is a subset; M7 rests partly on unchanged-surface reasoning.
- **N7** — `RackUxShots` can emit blank captures without failing; a pixel assertion is a separate tooling improvement.
- **N8** — live MFM gates run pure-Sitka worlds; Oak/Beech exclusion is offline plus MFM-10 carry-over.
- **N9** — licence copies in evidence roots and the unpublished branch/handoff.
- **N10** — fit results are not persisted; §15's eventual "what fit was reported" is recomputable, not stored.

### Round-1 reproduced checks

- All 17 final Unity pass markers from raw logs; zero `FAIL`, zero `error CS`, zero typed exceptions; the only exception-like lines are expected malformed-save messages.
- All 14 offline logs, including the three density misses against the accepted record.
- Pixel analysis of all 154 PNGs and all 47 guided-v2 images.
- Steps 02, 03, 04, 06, 07, 08 and 17 raw check lines; host-save SHA-256s; lock file/history and process list.
- Fit replay: agreement summary, player-only row, Share/basal-area consequences, "comparison not score" framing, invalid fit and refusal-with-no-mutation, restoration.
- Boundaries: no auto-confirm/bulk confirm, 3D `[C]`/`[N]`/manual override, proposals not Fell marks, Pending-only prescription approval, Work Plan commitment, ordinary execution, durable provenance.
- Persistence: ~30 malformed/old-save refusals before mutation, v24 provenance reload, Legacy40/E80 validity, frozen Reference Future archive/preview/continuation.
- Calibration: complete-loop witness, year-25 completion with 8 objectives, economy reconciliation, baseline CI 4.169829 % within the 20 % gameplay-calibration tolerance, and post-treatment CI parity **NOT DEMONSTRATED**.

## 3. Round 1 conditions and disposition

1. Cite the guided-v2 recheck as the rendered evidence, attach a receipt, mark the blank attempt superseded, and correct the pixel/154 claims. **Done.**
2. Release the lock, re-verify processes/lane and rewrite the cleanup receipt. **Done** (release `2026-11-03T02:12:25Z`).
3. Replace the pre-asserted verdict. **Done.**
4. Optionally correct N1/N3/N4 wording and strip licence copies. **Done.**

## 4. Round 2 — delta confirmation (OpenAI GPT-6.1-Sol)

**Verdict: APPROVED — delta confirmation. No new blocking finding.**

- **B1 RESOLVED.** The RC receipt matches the raw log: 535 passes, 0 failures. All 47 PNGs are non-constant, fully opaque; 23 × 1280×720 and 24 × 1920×1080; timestamps 01:57:45Z–02:02:59Z, inside the receipted window. Harness hashes match committed sources. The A3 blank set is explicitly superseded and excluded. The final 154-image set was independently counted and pixel-checked. The handoff now distinguishes guided-v2 layout assertions from independent capture-content checks.
- **B2 RESOLVED.** Lock history records the normal release of `98c789a0…` at `2026-11-03T02:12:25.386623Z`; current metadata is FREE. No Editor, Xephyr, openbox or verification driver is running. U1 is clean including untracked files; recheck harnesses and `.meta` are absent.
- **B3 RESOLVED.** The handoff records round-1 NEEDS CHANGES and PENDING DELTA REVIEW; approval wording appears only as a target.
- Identity: U1 is `qual/mfm-12-integrated-v2` at `0cd7b5ff`, descending from `e42bb9f4`; production diff empty; save v24.
- Raw-log spot checks: step 08 fit (`agree=18 playerOnly=1 ruleOnly=0`, invalid refusal without mutation); steps 02/07 proposal-vs-Fell, Pending, Work Plan commitment, provenance reload/execution; step 04 old/malformed saves; step 05 frozen reference; step 09 completion year 25 and CI parity NOT DEMONSTRATED; step 17 Legacy40 history.

Remaining non-blocking findings:

- N1/N3/N4 coverage limits are honestly disclosed: zero live rule-only iterations, no separate invalid-fit rendering, no gate reference/execution of `Refusal.FitUnavailable`, and no single MFM world demonstrating two-cycle Annual Review/history. Component evidence supports the required boundaries; it does not prove those missing scenarios.
- Fit-history reporting, stale approval-frame messaging, F11 user workload testing, the accepted Beech losses and the CI limitation remain carried.
- Publication, P5 and P6 remain outstanding at the time of the delta review; this file resolves the publication item.

### Reviewer statement for the durable record

> **APPROVED — delta confirmation.** I, OpenAI GPT-6.1-Sol, independently reviewed qualification HEAD `0cd7b5ffceaf554c014ff0d95ede7beb17d565cb` against production candidate `e42bb9f437e4fcf20c5b753dacd1136470daf33e`, without producing qualification evidence or modifying the workspace. Claude Sonnet 5.5 completed round 1 with NEEDS CHANGES; I substituted for delta confirmation after Claude's quota exhaustion. B1, B2 and B3 are resolved: the replacement guided captures are contentful and correctly receipted, lock/cleanup state matches current observations, and approval is no longer pre-asserted. No new blocking finding was identified. Disclosed fit/history/render coverage gaps remain non-blocking limitations, not demonstrated coverage. This approval supports Manager acceptance review; it does not issue the final product token or satisfy publication, personal-playtest or external-test gates.

## 5. Carried limitations

- Three pure-Beech reduced-simulator R2 (single-year all-stem loss ≤ 0.40) misses: `0.423 / 0.407 / 0.417` against the unchanged `0.40` (accepted C-level limitation).
- Post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**.
- MFM-11 extension-state calibration is accepted only within its **gameplay-calibration** boundary.
- F11 candidate workload remains a human/user-test obligation.
- Rule-only and invalid-fit presentation are not separately live/rendered; `Refusal.FitUnavailable` has no gate execution.
- No single MFM-prescription world demonstrates two-cycle Annual Review/history; step 17 is the accepted Legacy40 three-cycle history path.
- Fit results are not persisted; spec §15's eventual fit-history question is recomputable, not stored.
- P5 publication and P6 personal playtest are external.

## 6. What the reviewers did not verify

- Neither reviewer re-ran Unity or any gate; everything rests on persisted logs, receipts and PNGs.
- Round 1 opened 30 of 154 PNGs and 3 of 47 guided images; the rest were checked programmatically for content, not visually. Round 2 opened representative guided images at both sizes and pixel-checked the final set.
- Issue #225, `active-tasks.md` and the exact Manager repin were not re-verified by the reviewers.
- `CheckFit` algorithmic correctness was not re-derived; it is unchanged and previously accepted, and only its reuse was confirmed.
- The MFM-10 live species guard, S1-D lineage content and Rack V2 geometry code were not re-derived beyond gate output.
- The 25 mutation controls were not re-run; only the `25/25 killed` marker was checked.
- F11 user workload, P6 playtest, external testing and the absence of a Windows build were not verified beyond finding none in the evidence.

## 7. Final disposition

`MFM-12 QUALIFICATION PASS / INDEPENDENT REVIEW APPROVED / READY FOR MANAGER ACCEPTANCE`

Not issued by the reviewers or by #225 (Browser Overall authority):

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`
