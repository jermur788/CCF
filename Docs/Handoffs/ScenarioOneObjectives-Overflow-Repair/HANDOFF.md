# Scenario One Objectives Overflow Repair (#224) — Handoff

**Status:** COMPLETE — bounded production repair and rendered regressions returned for Browser Overall review.
**Success token:** `SCENARIO ONE OBJECTIVES OVERFLOW REPAIRED — READY FOR MANAGER REVIEW`

## Binding identity

| Field | Value |
|---|---|
| Claim | #224 (parent #203 / #200 / #221) |
| Authority | `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/LOCAL-EXECUTION-HANDOFF-20261102.md` and `TASK-PACKET.md` on `main` |
| Exact starting candidate | `bbcefe06e0d6a0dce38f6eb01d622dca7677495c` |
| Branch | `fix/scenario-one-objectives-overflow-v1` |
| Worktree | `/media/jer/ZX20/CCF-S1ObjOverflow` (dedicated clean lane, registered as `S1Obj` in `CCF-Ops/lanes.json`) |
| Final production commit | `95db20ae30c0db101c9fa596457eb7e2285cbcaf` |
| Execution provider/model | `opencode-go/deepseek-v4.1-flash` (the requested SOL/CODEX lane was not available in this session; the actual lane is recorded honestly) |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneObjectivesOverflow-20261102/` |

Coordinator note: live coordination was refreshed from `origin/main` (`Docs/Project/coordination/active-tasks.md`, state after #223 approval / #224 release). It was not edited here because #200 is the sole writer during the two-lane split, matching the #223 precedent.

## Reproduction before any production edit

On exact `bbcefe06…` the guided-v2 gate reproduced the accepted failure family:

| Gate run | Result |
|---|---|
| `unity/guided-v2-before-r2` | **533 PASS / 2 FAIL**, 535 checks, 47 shots |
| Failures | `current objective fits (1280, 720)` and `current objective fits (1920, 1080)` |
| Failing stage | `sequence-06-regeneration-and-protection` — lesson **6. Extend your treatment** |

The `#current-objective` card was clipped mid-sentence in “What good practice looks like” and the progress line was cut off at both sizes. Instrumented layout values at 1920×1080 (UI units on the 1600×900 reference) were:

- root `1600×900`; scroll viewport bottom `849.2`; card `879.2` tall with bottom `1020.9`.
- At 1280×720 compact: card `905` tall with bottom `1033.8`; viewport bottom `861.3`.

The whole card was therefore taller than the modal viewport and the screen. This is the player-facing defect, not a stale assertion.

An earlier attempt was killed by the OS OOM killer during the lane's first fresh `Library` import (`exit=-9`, 976 s). That is an environment failure, not a production result; the clean reproduction above followed on the warm `Library`.

## Production repair

Single production file changed:

`Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` — 13 insertions, 10 deletions.

| Change | Reason |
|---|---|
| `.objective-part-text` font size `var(--fs-reading)` → `var(--fs-body)` | The longest lesson’s six text paragraphs no longer fit at reading size. Body size is the project’s normal dense-copy size and remains legible at 720p. |
| `.objective-words` basis `480` → `580` (compact `460` → `580`) | Reflow: use the wide Objectives window for more words per line. |
| Objective part heading/text margins tightened (`14/5` → `8/2`, `6` → `3`) | Remove vertical slack without removing content. |
| Teaching diagram basis `800` → `780` (compact `800` → `780`) | Make room for the wider words column; the diagram stays above its minimum label sizes at both target widths. |
| `.modal-objectives { height: 96%; }` and `.compact .modal-objectives { height: 98%; }` | The longest objective needs a slightly taller viewport; no other modal changes. |

No copy, title, instruction, practice text, progress line, teaching picture or objective meaning changed. No `display: none`, `overflow` clipping, `max-height`, ellipsis, truncation or semantic shortening was added. The rendered-fit assertion was not edited; `Tools/` is unchanged from `bbcefe06`.

## Regressions after the repair

| Gate | Result |
|---|---|
| guided-v2 (`unity/guided-v2-after`) | **535 PASS / 0 FAIL**, 535 checks, 47 shots; `GUIDED_ONBOARDING_PASS` |
| Combined HUD v2 (`unity/combined-v2`) | **465 PASS / 0 FAIL**, 24 shots; `CHC_DONE pass=465 fail=0` |
| PARITY-07 rendered (`unity/parity07-r5`) | **1031 PASS / 0 FAIL**, 36 shots; `PARITY07_UNITY_HARNESS_PASS`; world hash `A0E0B2A51CC9274A` before and after |
| Rack/minimap | No Rack or minimap source/style was touched. The guided-v2 gate still exercised the V2 rack planning/approval/advance flow, and the Combined HUD gate still passed its minimap sections. A separate Rack confirmation gate was therefore not required by the packet. |

Guided-v2 after screenshots (`.../guided-v2-after/evidence/sequence-06-regeneration-and-protection-1280x720.png` and `...-1920x1080.png`) show the whole card, every objective section and the progress line `Treatment review on the Stand Map · nothing is committed until you approve the Work Plan`, with the teaching picture intact.

## Environment/tooling notes

- **Parity-07 r1–r3** failed only `NO_CONSOLE_ERRORS` with `ArgumentOutOfRangeException` at `UnityEditor.Search.SearchDatabase.EnumerateAll` (QuickSearch indexing on Editor startup). The stack is entirely Unity Editor code; no project code appears.
- The lane’s generated, gitignored `UserSettings/Search.settings` was `{}` after the OOM-killed first import. A copied same-project `Library/Search` cache made captures blank (r4); replacing the generated search settings with the healthy same-project settings produced the clean r5 run. This is an Editor environment repair, not a production or commit change.
- The run lane was left with that healthy gitignored user setting. No tracked side-effect file (`.vscode/settings.json`, Section Five materials) remains modified.

## Independent review

| Field | Value |
|---|---|
| Reviewer/provider | `claudeAgent` — Claude Sonnet 5.5 |
| Effort | `high` |
| Mode | Read-only independent review; no edits, no commits, no Unity, no lock changes |
| Verdict | **APPROVE** |
| Blocking findings | **None** |
| Findings | Low residual: the Parity-07 QuickSearch environment caveat above (Editor-level, r5 credible; only r5 exercises the repaired CSS, so no pre-change parity run exists). Low visual trade-off: 18 px objective body text and the taller modal are slightly smaller/taller than before but legible at both resolutions. |
| Confirmed | Production diff is one USS file; `Tools/` is unchanged so #223 repairs are intact; no save/schema, ecology/economy/calibration, Rack/MFM, protected-reference or C# change; the fit assertion and objective copy are intact; the repair is bounded and ready for Manager integration. |

## Unity lock lifecycle and cleanup

| Phase | Lock |
|---|---|
| Preflight / reproduction / repair | `6b03caa1e9d74ed3bd56ab7c4a35d82d` held from 2026-11-02T17:46:17Z at `bbcefe06…`; released normally after the production commit was created |
| Post-repair regressions | `7da9e056ceb24faf96fc4960d9224256` held from 2026-11-02T18:41:58Z at `95db20ae…`; released normally after all Unity gates |
| Final state | **free**, metadata None, no Unity Editor PIDs |

Cleanup: no staged verification harness or generated `.meta` remains under `Assets/ForestPrototype/`; the worktree is clean at `95db20ae…`; stale generated `Temp`/`Logs` were removed. The inactive Q10 `Library`/`Temp` cache (not source) was removed earlier to keep the shared ZX20 Unity SSD above its storage threshold; no user work was touched.

## Boundaries observed

- No change to Rack or MFM workflow semantics.
- No change to save/schema, protected anchors, frozen Reference Future v1, ecology, economy or calibration.
- No change to objective meaning or accepted copy.
- #194 not touched.
- No Windows build.
- MFM-12/#225 not started.

## Recommendation

Browser Overall should review this handoff, `95db20ae…` and the evidence root, then integrate #224. After integration, repin MFM-12/#225 to the resulting exact production candidate.

`SCENARIO ONE OBJECTIVES OVERFLOW REPAIRED — READY FOR MANAGER REVIEW`
