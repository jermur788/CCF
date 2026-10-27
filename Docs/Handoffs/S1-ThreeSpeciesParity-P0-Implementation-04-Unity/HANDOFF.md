# PARITY-04 — Focused Adult Species Unity Qualification — Handoff

**Task:** `PARITY-04-UNITY-QUALIFICATION` · **Claim:** [#27](https://github.com/jermur788/CCF/issues/27) · **Date:** 2026-10-27  
**Recommendation:** **APPROVE** the focused adult qualification, subject to Manager review.  
**Exit:** `ADULT SPECIES PARITY REQUALIFIED — READY FOR MANAGER REVIEW`

## Identity, authority and scope

- Packet: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04-Unity/TASK-PACKET.md`, from current main at task start.
- Spec: `SPEC-scenario-one-three-species-parity-p0-V1`, APPROVED; `specs/scenario-one-three-species-parity-p0/spec.md`, principally FR-3 and the adult portion of AC-3. This does not certify all parity acceptance criteria.
- Worker provenance: Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic / Claude Code in T3 Code) performed preflight, static gates, Unity import/compile and Legacy40. After its provider interruption, the user instructed the same thread to continue; OpenAI Codex `gpt-6.1-sol`, xhigh reasoning, through the configured Codex provider in T3 Code recovered that evidence and performed Enlarged80, preservation/cleanup and publication. Both executors are recorded on claim #27; no delegated reviewers or competing Unity workers were launched.
- Starting main / exact tested candidate / final tested HEAD: **`81a037e35126bbc256fcda076019af47e8e0954f`**. This contains PARITY-04 merge `71b5ae4c83f3ce6d489cdec81f3306abb54d9df1` and its Manager review.
- Verification lane: `/media/jer/ZX20/CCF-U1`, detached HEAD. It was clean before import/compile.
- Publication branch: `verify/scenario-one-three-species-parity-adult-unity-v1`, based on the exact tested candidate. Its only change is this Markdown handoff; the publication commit is not a newly tested production candidate.
- Editor: **Unity 6000.6.0f1**, revision `f7f8ed4d1e24`.
- Verification-only: no production or verification target was changed to obtain PASS. No retuning, assertion weakening, source-contract refresh, expected-value edit, package/schema change, protected-anchor rebaseline or Windows build.

## Results

| Gate | Result |
|---|---|
| Integrated calibration/source contract | `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` |
| Source normalizer self-tests | `SOURCE_NORMALIZATION_PASS` |
| Site infrastructure | `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` |
| Fixture controls | `FIXTURE_CONTROLS_PASS checks=9 static-contracts=7 NOT_UNITY_VERIFIED` (offline marker; Unity results below) |
| Unity import/compile | PASS; exit 0; no C# errors |
| Adult gate, Legacy40 | `BROADLEAF_GROWTH_VERIFY_PASS checks=30`; exit 0 |
| Adult gate, Enlarged80 | `BROADLEAF_GROWTH_VERIFY_PASS checks=30`; exit 0 |

Both geometries verified accepted Oak/Beech adult constants, exact live site scalar 1.0, species-neutral canopy/light for identical geometry, shared Hegyi cutoff 8 m, retained species-law behavior, deterministic replay/save-load continuation, and the protected Sitka comparator.

### Height diagnostics

Oak: 21.45 m age 50, 28.18 m age 75, 32.76 m age 100.  
Beech: 23.37 m age 50, 30.24 m age 75, 34.65 m age 100.

Beech remains below the provisional age-50/100 bands by 1.63 m / 0.35 m respectively. This is the already accepted `ACCEPTED_PROVISIONAL_HEIGHT_BAND_MISS (PARITY-02 U-11)` calibration limitation and is diagnostic, not a pass/fail target. No retuning occurred.

## Preservation and release

Before execution, 5,079 tracked files and ten real user save/preferences files were hashed. After both gates:

- zero tracked byte changes;
- zero real user-file changes;
- no disposable adult verification harness/meta remained;
- the attributable `.vscode/settings.json` Editor normalization was restored byte-for-byte;
- no production/species asset/scene/prefab/material/package/ProjectSettings/save-schema/verification-source/expected-value/protected-fingerprint/Reference Future v1 change;
- U1 was clean and the Unity lock was released normally, with no Editor process remaining.

After release, MFM-02 subsequently acquired U1; this task did not interfere with that worker.

Durable evidence root: `/media/jer/Files/CCF/Verification/Parity04-UnityQualification-20261027T204332Z/`.

## Scope boundary

This handoff qualifies PARITY-04 adult-species verification only. It does not certify the full parity spec, Scenario One demo readiness, Player/native-input performance, or Windows build readiness.

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO.**
