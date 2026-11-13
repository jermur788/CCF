# #443 Objective 1 UX repair: independent delta review (F-1, F-2, F-3)

**13 November 2026. Verdict for exact candidate `e115b54053fe96fd2c0b85d0516aa15cb65b1387`: APPROVED (bounded delta review).**

The repair closes all three findings from the first review:

- F-1 is closed in the post-spray feedback and Work Plan line.
- F-2: the accent edge now renders at both sizes.
- F-3: the "Read and acknowledge" line now clears after acknowledgement.

The production delta since `1aa0fa9a` is exactly the three intended edits. Spray association code is byte-identical to the base. Run 4 is a genuine full-route PASS on this candidate. I found no blocking defect.

One new **non-blocking** finding needs a Manager or user decision before the player re-tests with a hand-drawn or edited rack plan:

- **D-1:** the Stand Map editing screens (and the later Treatment Review) still name racks by their stored creation number.

The user's F-1 decision limits this repair to two strings, so I do not hold this candidate on D-1.

This verdict is not integration approval and not user acceptance. Manager #424 owns the integration decision. The player's own re-test is still to come.

## Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), in T3 Code, as the review sub-agent for orchestration #447 on review claim #449. I started no subagents, posted nothing to GitHub issues and did not edit `active-tasks.md`.
- **Did not author the code under review.** All G4 production edits, the repair loop and runs R1–R4 were by OpenAI GPT-6.1 Sol.
- **Disclosure:**
  - The first review (`d5024fc3`, NEEDS CHANGES on `1aa0fa9a`) was by an earlier Claude Opus 5.5 session.
  - The G3 bounded spec (`2d94cc26`) was by another Claude Opus 5.5 session.
  - I have no memory of either session's reasoning; I read the first review's report. Where its conclusions turned out to be incomplete, I say so (D-1).
- **Branch:** `docs/objective1-443-independent-delta-review-20261113-v1`, from `origin/docs/objective1-443-independent-review-20261113-v1`. This file is its only change.
- **Unity:** not launched. The lock was not taken, and no Unity process was running when I checked. Nothing was written in the author's lane or on ZX20. The feature branch was inspected with `git diff`/`git show` and a `git archive` extract only.
- **Reviewer scratch:** `/media/jer/Files/CCF/Verification/CCF-449-Objective1-IndependentReview-20261113/delta/`. It holds `cand/` (e115b540 extract), `base/` (55863f1b extract), `probe-scripts/run_offline_patched.py`, `offline-rerun/` and `offline-rerun.log`.

## Closure table

| Finding | Status | Evidence I looked at |
|---|---|---|
| **F-1** stable rack ID in post-spray feedback (blocking) | **CLOSED** (as defined: HUD message, Work Plan status line, manager feedback, clear-mark copy) | Source: `ScenarioOneManager.RackPlan.cs:419` → `$"Marked the entrance tree {rack.entranceTreeId}."`, `:433` → `"Cleared the entrance mark."`; whole file equals base except those two lines. R4 receipts: 10 `POST_SPRAY_FEEDBACK` + 40 `FEEDBACK_ORDINAL_ASSERTION` (all pass), `feedbackFailures: []`, `mismatches: []`. Viewed `second_plan_spray-post-spray-rack-1-stable-5-1280x720.png` (HUD "Marked the entrance tree PO0203.", prompt "Rack 1 entrance · Orange · entrance already marked") and `…-rack-5-stable-1-1920x1080.png` ("Marked the entrance tree PO3803.", prompt "Rack 5 entrance …"). Own-ID/own-Pending checks hold (`SECOND_PLAN_SPRAY` ×5, `SECOND_PLAN_FINAL`). Other surfaces that still show stored IDs are listed separately as D-1. |
| **F-2** Plan Racks accent edge overridden (blocking O1-07) | **CLOSED** | USS `:689` is now `.modal-map .map-planning-instructions` (0,2,0, later than `:265`). Viewed `g08a-plan-racks-editing` at both sizes. Pixel-sampled: a continuous accent column `rgb(111,163,157)`, 2 px wide at 720p (x 935–936, 362 px tall) and 4 px at 1080p (x 1423–1426, ≈585 px tall), against card fill `rgb(27,35,38)` (≈5.6:1 contrast). The other three edges stay a 1 px grey `rgb(66,77,81)`. |
| **F-3** "Read and acknowledge" persists after acknowledgement (non-blocking) | **CLOSED** | Source: `ScenarioOneUiRoot.cs:560–562`, control-flow traced (below). Viewed `g14` 720p ("Read and acknowledge the Annual Review.", button present) and `g15` at both sizes ("Choose one Crop Tree in each zone, 8 m apart; include both rack sides.", footer gone). `ANNUAL_REVIEW_INSTRUCTION` receipts ×4 agree. `AnnualReviewView.cs` and all gate code are unchanged against base. |

## Scope check

- `git diff 1aa0fa9a..e115b540 -- Assets` changes three files in total, +6/−4 lines:
  - the USS selector (F-2);
  - the `ScenarioOneUiRoot.cs` condition (F-3);
  - the two feedback strings in `ScenarioOneManager.RackPlan.cs` (F-1).
- **Manager file:** `git diff 55863f1b..e115b540` of `ScenarioOneManager.RackPlan.cs` is exactly the two `feedback =` lines. Nothing else changed: no return value, no `RackPlanOperationResult.Applied(feedback, rackId)` argument, no ID and no logic. This is the change the direct-user F-1 decision permits.
- **14 production paths** in `55863f1b..e115b540`: the 13 G3 paths (the spec list minus `StandMapRackEditor.cs`, as narrowed by Manager #424) plus `ScenarioOneManager.RackPlan.cs`. There are no `.meta`, scene, prefab, asset, ProjectSettings, Packages or Reference paths. `ScenarioOneTeachingSequence.cs` and both read-only LearningObjectives partials are unchanged (offline contract re-run).
- **Spray, aim and equip:** the `TrySprayRackEntrance()` body is byte-identical to base (SHA-256 `0428e04b…` on both sides). `TryRackSprayAim` and `EquipRackSpray` are also identical.
- **USS reach:** the class `map-planning-instructions` is added in exactly one place (`StandMapView.cs:595`, `map-edit-card`). The new selector only adds an ancestor qualifier, so it can affect no other rack panel. Other `.modal-map .rack-panel` cards keep `--map-rule`; the author's offline specificity contract checks this and I re-ran it.

## F-1: rack identity after the fix

**Mark and spray path.**

- Neither `TrySprayRackEntrance` nor `MarkRackEntrance` puts a stored rack number into anything the player sees.
- The refusals on that path name no number: "Equip the spray marker…", "Aim at the entrance trunk…", "That tree is not a rack entrance…", "The entrance tree already carries a Fell mark…", "This rack entrance is not marked."
- The only refusal that would name a stored ID is `"There is no rack {rackId}."` (`:403/:429`). It is unreachable from the spray, because the ID comes from `ConfirmedRacks` itself.
- The HUD line (`WalkingHudView.cs:178`), the spray prompt (`ForestTreeMarkingManager.RackEntrances.cs:191/201`) and the minimap label (`HudMinimapView.cs:120`) all use the visit ordinal.
- The Work Plan shows manager feedback only after 5/5. Before that, its line is the marking-count advisory (no rack number). The V2 Work Plan lists orders as "Fell {treeId}". The legacy "Extraction rack {Id}" heading (`WorkPlanView.cs:439`) is fed only by `RackGroups`, which V2 never fills (`ScenarioOneManager.Racks.cs:108`).

**Clear mark.** `ClearRackEntranceMark` has no caller anywhere in `Assets/`, so the player cannot currently reach it. Its new copy is checked offline only, which the author discloses.

**Surfaces that still print a stored ID: see D-1.** They are on the Stand Map rack *editor*, its status line, the plan-edit manager feedback, the harvested-lock status and the Treatment Review strip titles. None of them appears during the confirmed-plan marking interaction.

**Is the new harness assertion real?** Yes:

- `CheckFeedbackOrdinal` uses `\brack\s+([0-9]+)\b` (case-insensitive). The old strings fail it (`OfflineProbe.cs:91–96` proves this both ways), and neutral copy passes.
- On its own the regex check would pass vacuously on an empty string. The same spray step, however, separately requires:
  - `marks.TransientMessage` non-empty;
  - the HUD message row visible, with text equal to that message;
  - Work Plan feedback and `manager.Feedback` non-empty.

  So an empty message fails the run.
- A failure is collected in `feedbackFailures` and turned into the run failure, so it is not just logged.
- Limits: it only recognises the form "rack N", and the Work Plan label is refreshed while hidden. Both are disclosed, and adequate for this regression.

**Post-spray timing.** Each spray's two captures came 0.649–1.194 s after the input request (receipts), inside the 2.5 s message window. I viewed one capture at each size for the reversed plan's Rack 1 and Rack 5.

## F-2 measurement: R3 failure and the `e115b540` correction

- R3 (`a5cf0ef5`) failed only on `resolvedStyle.borderLeftWidth >= 3`. At 720p UI Toolkit resolved the 3-unit edge to 2.5 units: 3 × 0.8 = 2.4 device px, rounded to 2 px. The colour was already correct (receipt `[0.431,0.639,0.616,1]`). No association mismatch, no feedback failure and no save write occurred.
- `e115b540` changes only the harness and README. It:
  - keeps the exact resolved-colour assertion;
  - replaces the unit check with `resolvedWidth × scale ≥ floor(3 × scale)` device pixels;
  - adds a rendered-pixel check: accent pixels over at least 80 % of `expectedWidthPx × card height` in the captured image.
- **This is a legitimate correction, not a loosened assertion.** The width bound is satisfied by the true quantised width. A regression to a 1 px edge would fail at both sizes; a 2 px USS edge would fail at 1080p (rounds to 2 < 3). A wrong colour fails both the style check and the pixel check. The new pixel check is stricter than anything R1/R3 had. R4 measured 712 = 2 × 356 at 720p and 2304 = 4 × 576 at 1080p.
- The harness comment says "3 px at 1.2" where the observed width is 4 px. The author discloses this; it is cosmetic.
- **Frank visibility judgement:** the edge is *visibly* present, not just technically. It is the only teal line in that column and clearly distinct in hue from the grey rules.
  - It is restrained, as the spec asked. At 720p it is 2 px, and the card sits nested inside the outer "Planning Racks — Active" panel, so it reads as a modest accent rather than a strong separator.
  - The card still has no bold heading of its own in the unselected state; heading weight applies only to the selected-rack heading, as the first review noted.
  - Whether this is "distinct enough" is a user judgement for the re-test. It meets the O1-07 wording.

## F-3: condition correctness

New condition: `scope == review.Root && NeedsAnnualReview && Q<Button>("annual-review-acknowledge")?.parent.style.display == DisplayStyle.Flex`.

- **Inline versus resolved style.** `AnnualReviewView.Refresh` (`:133`) explicitly sets the footer's *inline* `style.display` to `Flex` or `None` on every non-early-out refresh. No USS class hides the footer. Testing inline style therefore tests exactly what the view owns.
  - **Ordering:** `SetScreen(Review)` calls `review.Refresh(true)` before `RefreshNextActionGuidance()` (`ScenarioOneUiRoot.cs:711–721`).
  - On acknowledgement, `review.Refresh(true)` runs and then `learning.Refresh(true)`, which re-derives guidance (`LearningObjectivesView.cs:234`). So the value read is never stale on open or on acknowledgement.
- **Null or differently built footer.** If the button is missing, the expression is null, the comparison is false, and the override is skipped. That does not lose the pending instruction: while the latest year is unread, `ReviewFirst` is true, and both fallback reasons on that branch resolve to `ui.AnnualReviewInstruction`. On the Review screen that is "Read and acknowledge the Annual Review." (`LearningObjectivesView.cs:40,61`; `ScenarioOneUiRoot.cs:49`). The override is therefore close to redundant while pending and fails safe.
- **States:**
  - Latest year pending: shown (g14, receipts).
  - After acknowledgement: cleared; the next-objective line resumes (g15, receipts).
  - Earlier year opened while the latest is pending: the footer becomes `None`, but year navigation does not call `RefreshNextActionGuidance`. The line keeps its last text, "Read and acknowledge the Annual Review.", which is still true. If guidance is re-derived, the screen-aware fallback gives the same text. Either way no Work Plan [Tab] direction appears.
  - Earlier year opened after acknowledgement: no override.
  - The last two cases are source-only; native browsing was not driven.
- **Gate logic untouched:** `git diff 55863f1b..e115b540` of `AnnualReviewView.cs` and `ScenarioOneManager.cs` is empty. The acknowledge callback's `showingEarlierYear` guard and the review-first advance refusal are unchanged. R4 still asserts the refusal of a second advance.
- **Test-fidelity note (cosmetic):** the offline stub's `DisplayStyle` defaults to `Flex`, whereas Unity's unset inline style is `StyleKeyword.Null`. The probe cannot represent "never refreshed". Production always refreshes before reading, so nothing is wrong today.

## Regression and evidence quality (R4)

| Check | Result | How |
|---|---|---|
| R4 head = candidate | PASS | `result.json` `head` = `candidate.txt` = `e115b540…`; status PASS, exit 0, 300.5 s; log token `OBJECTIVE1_UX_NATIVE_PASS checks=522 mismatches=0 saveWritten=False` |
| Staged harness = git blob | PASS | `staged-sources.json` SHA-256 `8e68e1df…` = `git show e115b540:…/Objective1UxNative.cs \| sha256sum` |
| Zero-spray clearing frame | PASS | Viewed `w03` at both sizes: yellow arrow, "Rack 1 entrance · 53 m", minimap "Rack 1" ring, 0/5, help closed. `ARROW_PIXELS` 219 / 487; `SPAWN` (−0.43, 5.25) |
| Bottom first → 1/5, next Rack 2 | PASS | Viewed `w05` at both sizes: 1/5, "Rack 2 entrance · 19 m", minimap "Rack 2", prompt "Rack 1 entrance · Orange · entrance already marked" |
| Top second → 2/5, next Rack 2, far tree Rack 5 | PASS | Viewed `g10` at both sizes: 2/5, "Rack 2 entrance · 52 m", minimap "Rack 2", prompt "Rack 5 entrance · Orange · entrance already marked"; `AFTER_TOP_SECOND_SPRAY` |
| Minimap 2.3–2.6× area, no overlap | PASS | Ratios 2.511 / 2.495. I recomputed panel-versus-bottom-bar, right-column and reticle intersections for all 28 `MINIMAP_AREA` receipts (14 states × 2 sizes): 0 overlaps, no null comparison rectangle |
| Old-world and frozen Reference controls | PASS | Viewed `g17` 720p (legacy objective, no Tree Marks or rack target) and `g18` 1080p (Reference banner, no HUD target). `NEGATIVE_CONTROLS` all true, `diskSaveWritten:false`. Reference files re-hashed in base and candidate extracts: `91cf5059…`, `789e3edd…` |
| Reversed-ID association | PASS | 5 × `SECOND_PLAN_SPRAY` (each marks only its own stable rack); `SECOND_PLAN_FINAL` 30/18/34/36/31 Pending = own removal sets; geometry hash `307C1ED538EA0D3E` unchanged |
| Offline gate (reviewer rerun) | PASS | `run_offline.py` from `e115b540`, path/git-shimmed to my extract (no checkout): **47 probes + 27 contracts**, exit 0, matching the author's count |
| Protected saves | PASS | `cd / && sha256sum -c --quiet` on `host37`, `backup81`, `production4011`: all exit 0 (rerun now) |
| Unity log | OK | The only exception is the known Editor `SearchDatabase` startup `ArgumentOutOfRangeException` (also in G2/R1). The `UIRenderDevice` frames are `Debug.Log` stack traces, not errors |
| Lock / processes | OK | `lock-free-final.json` free, `unity_editor_pids: []`; no Unity process at review time |

**Does the delta invalidate anything the first review approved?** No. Outside the three edits, the production code under `Assets/` is byte-identical to `1aa0fa9a`. The F-3 edit touches only the review-scope reason, and g14 confirms the pending state still shows. R4 re-exercised the whole route, so the first review's carry-over argument (N-5) is no longer needed.

## Status of the first review's non-blocking notes

| Note | Status now |
|---|---|
| N-1 backdrop alpha 0.94: faint bright text still discernible | **Carried forward.** Unchanged; user judgement in the re-test. |
| N-2 Stand Map "Destination" diamond follows the rack projection while the stored waypoint is unchanged | **Carried forward.** Unchanged. |
| N-3 enlarged minimap versus the later-objective planting hotbar | **Carried forward.** Not exercised (not Objective 1). |
| N-4 invalid first target: guidance fails closed, and `RackFieldStatus` problem text is shown nowhere | **Carried forward.** Pre-existing; unchanged. |
| N-5 no full-route green run on a final candidate | **Resolved** by R4. |
| C-1…C-5 cosmetic | **Carried forward.** C-1 "3.5 / m" wrap still visible at 720p in R4 `w03`/`w05`/`g10`. |

## New findings, ranked

### D-1: stored rack numbers still label racks on the Stand Map editor and in the Treatment Review (Moderate; NON-BLOCKING for this bounded candidate; needs a Manager/user disposition)

- **Where:**
  - `StandMapView.cs:613`: selected-rack heading `"Rack " + selected.Id` while the plan is open for editing.
  - `StandMapRackEditor.cs:405,465`: status "Moved/Rotated/Deleted rack {id}."
  - `ScenarioOneManager.RackPlan.cs:141,190,233`: "Added/Updated/Deleted rack {rackId} …".
  - `ScenarioOneManager.RackEditing.cs:125,441,461`: confirm refusal "Rack {id} is not valid…", restore.
  - Later stages: `StandMapRackEditor.cs:204` "Rack {hit} has been harvested…" and `ScenarioOneTreatmentReview.cs:202` "Rack {RackId} · … strip".
- **Failure scenario:** a player hand-draws racks north to south, or edits or reopens a plan so that creation order differs from south to north. On the Stand Map, the top rack they just drew and selected is headed "Rack 1" and the status says "Added rack 1 to the plan". After confirmation, the HUD arrow, spray prompt and minimap call the bottom rack "Rack 1" and that same top rack "Rack 5". Reopening the plan after marking shows the stored numbers again. After harvest, the Treatment Review strips use the stored numbers. This is the user's original complaint in a different place. It is invisible on the suggested-layout fixture, where stored IDs equal the visit order. The reversed-ID fixture was built through `AddRack` calls, so the Stand Map labels for it were never captured.
- **Why it does not block this candidate:**
  - The confirmed-plan marking interaction (HUD, prompt, minimap, post-spray message, Work Plan) is now consistent, which is what F-1 and the Manager's binding rule cover.
  - Fixing the map labels needs files outside the approved set: `StandMapRackEditor.cs` is expressly excluded, and `ScenarioOneManager.RackEditing.cs` and `ScenarioOneTreatmentReview.cs` are not listed.
  - The user's decision says not to delay the narrow correction for a general renumbering.
- **Why it matters:** the user also wrote that "the player-facing ordered five racks remain bottom→top 1, 2, 3, 4, 5 regardless of creation order". If the re-test uses the player's own (edited) world, they may see this.
- **Correction to the first review:** its statement that "the remaining disagreement is the feedback text" was incomplete. It should have listed these planning and review surfaces.
- **Smallest options for the Manager/user:**
  - (a) accept and disclose as a known limit for the re-test;
  - (b) authorise a follow-up to drop the stored number from editor and plan-edit copy, the way F-1 did;
  - (c) authorise a display-ordinal helper for the map editor and Treatment Review.

  I recommend (a) now, plus a separately scoped (b) later.

### Cosmetic / test-fidelity (not blockers)

- **D-2:** the harness comment and README parenthetical say the 1080p edge is "3 px". The observed width is 4 px; 3 px is only the lower bound. Already disclosed in the result doc.
- **D-3:** the offline F-3 stub defaults `style.display` to `Flex`, so it cannot model Unity's unset inline style (see F-3).
- **D-4:** after browsing an earlier Annual Review year, the review instruction label is not re-derived. Its text stays correct, but if a future change makes the instruction depend on the footer, it would go stale. Note only.

## Result document accuracy (`2432ba7e`)

- **Accurate:**
  - the exact candidate, the 14 paths and the two-line diff;
  - R1 and R3 recorded as FAIL with causes, and R3's units error described correctly;
  - the R4 timings, measurements and receipt contents;
  - the minimap figures;
  - the save, lease and drift handling;
  - the unreproduced original-world clearing arrow, stated as a limit, not a diagnosis;
  - the clear mark not driven natively;
  - earlier-year browsing not driven natively;
  - older checkers not re-run for this delta, with their historic failures not waived;
  - no approval, integration or acceptance claimed.

  The disclosure that the Work Plan line was refreshed while hidden, and shows the advisory before 5/5, is honest.
- **Omissions (minor, non-blocking):**
  - It does not say that `ClearRackEntranceMark` currently has no in-game caller.
  - Like the first review, it does not mention the D-1 surfaces.
  - The statement "four feedback surfaces checked after each [spray]" is literally true. Before 5/5, though, the Work Plan surface shows the advisory, not the manager feedback, so the effective coverage of that surface is the two sprays that reach 5/5. The doc says this elsewhere.

## Verified myself versus taken from receipts

- **Verified myself:**
  - all scope diffs and allowlist membership;
  - the manager-file whole-file comparison;
  - spray, aim and equip byte-identity;
  - the source-wide search for stored-ID player text (F-1, D-1);
  - the USS selector reach;
  - the F-3 control flow, refresh ordering and null behaviour;
  - the harness assertion logic, including vacuity;
  - the R3 → R4 harness diff;
  - R4 `result.json`, candidate and staged-harness hash;
  - parsing of all R4 receipts, with the overlap recomputed;
  - the screenshots listed below, plus pixel samples of `g08a` at both sizes;
  - the offline gate rerun (PASS 47 + 27);
  - the protected-save manifests (exit 0);
  - the Reference hashes;
  - the Unity log exception scan;
  - no running Editor.
- **Screenshots I viewed (R4):**
  - `second_plan_spray-post-spray-rack-1-stable-5` (720p) and `…-rack-5-stable-1` (1080p);
  - `g08a` (both sizes);
  - `w03` (both);
  - `w05` (both);
  - `g10` (both);
  - `g14` (720p);
  - `g15` (both);
  - `g17` (720p);
  - `g18` (1080p).
- **Taken from receipts and logs, not re-observed:**
  - that the input was real OS input, and the capture timings;
  - the hidden Work Plan label text;
  - the annual execution and cash change, and the second-advance refusal;
  - old-world unmodified and slot unwritten;
  - lease acquisition and release, Editor drift restoration and licence-copy removal;
  - `compile_gate.py` PASS (not rerun by me).
- **Not viewed by me:** the remaining 69 of the 84 R4 PNGs.

## Not verified

- The original personal-world "no arrow from the clearing" symptom. It is unreproduced and its cause is unknown, as the author states.
- How the Stand Map editor labels an edited or reversed plan in practice (D-1). This is source analysis only; no capture exists.
- Native clear-mark (no caller), and native earlier-year browsing during or after acknowledgement.
- Motion-OFF temporal emphasis, a save/load round trip after marks, an old world that already holds legacy racks, the later-objective HUD with the enlarged minimap, and a Windows/Player build.
- Human perception of the F-2 edge and the N-1 backdrop. These are user judgements.

**Native check that would settle D-1 cheaply, if wanted:** in a fresh V2 world, draw five racks north to south on the Stand Map, select the top one (capture its heading), confirm, then capture the field prompt at that same trunk. No production change is required to observe it.
