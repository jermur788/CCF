# Oak/Beech production calibration: anchor follow-up (overnight queue job B3, Tier 1)

**Question.** Are the previously missing anchor observations enough to close the feature qualification without repeating the growth campaign?

**Answer: yes, for the items the queue named. Growth classification stays: acceptable B (qualified, with the intentional anchor consequences and Manager decisions already listed in `Handoff.md`).** Nothing was rebaselined.

## Identity

| | |
|---|---|
| Source | `task/oak-beech-ordinal-production-calibration-v1` @ `796da99029190ef71e68d5fa722f25240e696318` |
| Run on | `verify/oak-beech-production-calibration-796da99` @ `2052a4ad4a0ac381e039a4a696dece7d19f31395` (harness and docs only: `git diff 796da99 HEAD -- Assets Packages ProjectSettings` is empty; the storm runner also reports `production_source_unchanged: true`) |
| Worktree | `/media/jer/Files/CCF-verify-oak-beech-796da99` (own Library; demo-HDR wrapper) |
| Unity | 6000.6.0f1, batch, Linux, one Editor at a time, isolated config folder, historical anchors in Legacy40 as the repository runners set it (D-056) |

## What was missing and why

`evidence/phase0-anchors.md` listed Model2 start/year-1 and storm anchors 2 and 3 as "not observed". The earlier attempt ran those three gates through the generic runner, which does not set the gates' output-folder variables, so each died in about a minute with `ArgumentNullException` (`anchors/` receipts of the first packet), not with a verdict. This run used the repository's own runners (`run_model2.py`, `run_storms.py`), which set `CCF_MODEL2_OUTPUT` and `CCF_STORM_OUTPUT`. Three runs, one Editor each, about three minutes of Editor time.

## Result: all observed, all unchanged

| Anchor | Value | Observed in | Verdict |
|---|---|---|---|
| Model2 start (v18 layout) | `FA855239CDDA32D8` | `Model2Verification` (`MODEL2_V18_COMPAT_ANCHOR`) | **unchanged** |
| Model2 year 1 (altered-site fixture, v18 layout) | `02334804F65C0234` | same line (`alteredSiteOneYear`) | **unchanged** |
| Storm anchor 1 (off, year) | `8333BAA4126E8A09` | `StormCoreVerification` (`STORM_OFF_ANCHOR`) | unchanged (also seen earlier) |
| Storm anchor 2 (resolved) | `84796FAEF211823C` | `StormCoreVerification` and `StormReplayVerification` | **unchanged** |
| Storm anchor 3 (continued / future) | `52037B943B62DD39` | same | **unchanged** |

Gates: `Model2Verification` PASS (**1,400 checks**, 77 s, exit 0), `StormCoreVerification` PASS (107 checks, 60 s, exit 0, 24 storm victims), `StormReplayVerification` PASS (47 s, exit 0). The two storm gates assert their hashes literally, so a PASS is itself the observation; the values are also printed in the logs (`evidence/anchors-followup/*-markers.txt`). No `error CS` in any log. Receipts: `evidence/anchors-followup/*-result.json` (run head, branch, staged-gate SHA-256, status, exit code).

## Not repeated, as instructed

Controlled trajectories, save/load, the 24-gate regression and the release-response campaign. Their results in `Handoff.md` stand: nothing under `Assets/`, `Packages/` or `ProjectSettings/` changed.

## Intentional isolated growth hash movements (recorded, not rebaselined)

Unchanged from `Handoff.md` section 2A; this packet added no new movement:

| Anchor | Before | After |
|---|---|---|
| Model2 completion world hash (`ScenarioOneCompletionVerification`, v18 compat) | `702766DECE591E21` | `D99EC9FC2C635064` |
| Reference continuation (v15 layout hash, `ScenarioReferenceVerification`) | `9CDF21A541C5968D` | `BFFF66015A6619DA` |

Old-rows A/B in the first packet proved the calibration is their only cause. The frozen Reference Future v1 archive anchors did not move.

## Why B and not A, and what is still open

- **B, not A:** the Manager decisions in `Handoff.md` stand (accept the two anchor changes, the promotion-height kink x1.67 Oak / x1.92 Beech, old saves picking up the new rows on resume, and the two gate-expectation fixes). None of them is answered by this run.
- **Still not observed on this head** (outside this job's list, and interactive UI gates): the P2 determinism anchor `F58FB0B1A421D28B` and P3 `F7C2FC966816BBAD`. They belong to the integrated candidate's qualification, where the expected values are to be set.

## Reproduce

Inside `Tools/Build/S1B/with_demo_hdr_quarantined.py`, on a quiet machine, with `Build/UnderstoreyRecruitment/config/unity3d/Unity/licenses` present:
`python3 Tools/Verification/UnderstoreyRecruitment/run_model2.py --gate Model2Verification`,
`python3 Tools/Verification/WindthrowV1/run_storms.py --gate StormCoreVerification`, then `... --gate StormReplayVerification` (the replay reads the core's output).
