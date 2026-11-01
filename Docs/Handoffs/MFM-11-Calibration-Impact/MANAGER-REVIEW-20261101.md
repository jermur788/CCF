# MFM-11 — Manager Review — 2026-11-01

**Claim:** #220  
**Browser Manager:** #200 / #221  
**Exact production candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`  
**Qualification branch:** `verify/mfm-11-calibration-impact-v1`  
**Operative run:** run 5 at gate HEAD `f642550c9171a6f2ccf22825d0284cabd8d6c13e`  
**First independent review:** Claude Sonnet 5.5 — `NEEDS CHANGES`  
**Round-2 independent review:** OpenAI Codex `gpt-6.1-sol` high — `APPROVED`  
**Manager verdict:** **APPROVED**

## Decision

The revised MFM-11 qualification is accepted on exact `3acdf9cb...`.

**Success token issued:**

`MULTI-SCALE CALIBRATION QUALIFIED`

No production retune is authorised or required by this decision.

## Evidence-boundary decisions

### 1. Actual extension-state calibration check accepted; post-treatment CI parity remains NOT DEMONSTRATED

The required live MFM-03 residual is satisfied at the actual post-Lesson-5 extension state: year 2 source-vs-current-Covered competition-index delta is **4.169829%** across 13 covered areas, within the accepted 20% **GAMEPLAY CALIBRATION** tolerance.

The later source-vs-current-Covered comparison is explicitly **NOT DEMONSTRATED** and is not promoted to a pass:

- at year 5 the treated comparable areas are classified out of `Covered` because completed work is an exclusion (`PlayerCompletedWork=13`), leaving zero current Covered comparison areas;
- at year 25 the live prescription read refuses after the source sample no longer satisfies its release qualification.

This is accepted as a structural limitation of this bounded qualification, not evidence that post-treatment parity holds. The accepted design does not require treated units to remain untreated comparable targets forever. Any future claim about arbitrary delayed/repeated-extension calibration robustness would require a separately defined diagnostic with explicit states and comparison sets.

### 2. Scoped exact-candidate PARITY-09 compatibility evidence accepted

MFM-11 itself proves save/load self-consistency. For protected Reference Future / Legacy40 compatibility, the Manager accepts reuse of the exact-candidate PARITY-09 evidence because production bytes are unchanged and the cited dedicated gates passed on `3acdf9cb...`:

- frozen Reference Future v1 archive/preview/continuation;
- pure-Sitka density anchors and exact controller victim sets;
- Legacy40 model-2 completion/continuation evidence.

This does **not** declare the entire PARITY-09 verification suite globally green; its separately recorded stale verification pins and accepted limitations remain debt.

## Why the calibration gate passes

The revised operative run corrects the first review's blocking evidence gaps:

- equal horizon at year 5 for sample-only, representative-prescription and manual paths;
- ordinary economy reconciles exactly through quoted/resolved revenue and the existing 250,000-cent minimum job cost;
- timber/deadwood claims are now measured at the stated precision;
- complete-loop reachability is demonstrated with `Outcome.Completed` at year 25 and **8/8 objectives**, including ordinary-path Beech ×3 and Oak ×3 planting and a second intervention at year 17;
- the wider representative path materially changes stems, basal area, canopy/light and regeneration, but does not break the accepted canopy/completion outcome;
- no production, protected anchor, save schema or gameplay calibration was changed by the qualification.

Run-5 marker: `MFM11_UNITY_PASS checks=47 shots=2`.

## Interpretation limits retained

- The 20% CI tolerance remains **GAMEPLAY CALIBRATION**, not empirical forestry.
- The representative path is a bounded Scenario One witness, not proof that every manual strategy or later extension state will succeed.
- The manual comparator is a plausible bounded alternative, not an equivalent-intensity manual recreation of the representative prescription.
- Rendered screenshots in MFM-11 are not promoted into a new UI/native-input qualification claim.

## Downstream release

MFM-11 acceptance releases **MFM-12 — Integrated verification and independent review** for preparation/execution under a fresh claim and exact-candidate repin.

MFM-12 must carry the separately recorded verification-debt requirements: stale v23 verification pins/history wiring must be repaired and affected gates rerun; guided-v2 must be reconciled; the Objectives-window overflow remains a real pre-Tier-3 UX defect unless a fresh exact-candidate gate disproves it.

No Windows build. Frozen Reference Future v1 and Legacy40 remain protected. Research Source Consolidation #194 remains **NO ACTION**.
