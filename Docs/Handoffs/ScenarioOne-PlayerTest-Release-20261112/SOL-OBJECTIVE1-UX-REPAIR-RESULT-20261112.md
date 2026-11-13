# #443 Objective 1 UX repair — implementation and G5 result

**13 November 2026. Repair loop 1 implemented; final full-route native run PASS, 522 checks at 1280×720 and 1920×1080. Ready for independent delta source/evidence review.** R4 below supplies the complete route on one frozen candidate. Earlier R1 and R3 remain **FAIL**; their evidence and causes are preserved. The independent review of the earlier candidate returned **NEEDS CHANGES**; no new review approval, integration or user acceptance is claimed.

- **G1–G3 actor:** Anthropic Claude Opus 5.5. **G4–G5 actor:** OpenAI GPT-6.1 Sol, after the user-attested documented serial transfer from the stopped Claude session. No subagents were started.
- **Branch:** `feature/scenario-one-objective1-ux-repair-20261112-v1`.
- **Only writable worktree / Unity lane:** `/media/jer/ZX20/CCF-S1-Objective1UX-20261112`, Ops `Objective1UX443`.
- **Implementation base:** `55863f1b7719011bba16b3f3ab377e379c0b988a`. **Clean takeover HEAD:** `2d94cc26f4047352c2794713029ae7e8cbc82858`.
- **Repair-loop-1 takeover:** clean at `324d6a1950a33b900492418bea5eb8c597cbc177`, matching origin; live lane registration/coordination read and physical lock free with no Editor PIDs before edits. No unrelated work was present.
- **Canonical context snapshot in this lane:** `63044dfbbeb869996ceecec8a47619dcba048b0b`.
- **Exact final candidate:** **`e115b54053fe96fd2c0b85d0516aa15cb65b1387`**, pushed before R4. F-1 production commit: `a5cf0ef5b63465d0c31c94a94009a20f98264814`; `e115b540` corrects only the border measurement harness/README. The later result publication changes documentation only and does not replace the tested candidate.
- **Authority:** G3 bounded spec at `2d94cc26`; Manager G3 verdict on `origin/docs/manager-424-objective1-g3-approval-20261113-v1`; G5 packet on `origin/docs/manager-424-objective1-t3-fable-orchestration-20261113-v1`; latest direct implementation packet.
- **Durable evidence root:** `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/` (abbreviated **E** below).

## Change and boundaries

The field cue now visits confirmed entrances south to north, breaking equal-Z ties by stable rack ID. A rack's displayed visit number includes marked racks, so spraying out of order never renumbers another entrance. HUD, contextual spray prompt (including already-marked trunks) and minimap destination use the same ordinal. The minimap projects the actual entrance tree from `TryGetRackFieldTarget`, without changing the player's stored waypoint, plan records or orders. Its static destination ring remains clear with Reduce Motion ON; the first appearance can gently emphasize its border with motion OFF.

The minimap side increases from 196 to 310 UI units; the bottom control bar reserves its width. The HUD keeps a rack arrow visible inside the ordinary waypoint arrival radius until the rack is marked. Onboarding gets a session-only initial Objectives cue; O1's Stand Map CTA omits the `Next:` badge. Objectives typography and the description/diagram columns, Plan Racks card, help backdrop, help keycaps, 5/5 Tab keycap, Work Plan typography and Annual Review screen-aware instruction are repaired within the approved file set.

Exactly 14 production paths changed: the original 13 G3 paths plus the direct-user two-feedback-line F-1 exception:

```text
Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs
Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs
Assets/ForestPrototype/UI/LearningObjectivesView.cs
Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs
Assets/ForestPrototype/UI/StandMapView.cs
Assets/ForestPrototype/UI/Map/StandMapSource.cs
Assets/ForestPrototype/UI/HudMinimapView.cs
Assets/ForestPrototype/UI/WalkingHudView.cs
Assets/ForestPrototype/UI/ActiveObjectiveHudWidget.cs
Assets/ForestPrototype/UI/MenuHelpView.cs
Assets/ForestPrototype/UI/MenuHelpView.Text.cs
Assets/ForestPrototype/UI/WorkPlanView.cs
Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss
Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs
```

Tests/runner/harness are under `Tools/Verification/Objective1UxRepair/`. No scene, prefab, ScriptableObject, ProjectSettings, package, material, `.meta`, save-schema or frozen Reference change is included. `TrySprayRackEntrance`, spray aim, equipment gating, `ScenarioOneTeachingSequence.cs`, both read-only LearningObjectives partials, Work Plan/Annual Review gates and all geometry remain unchanged. The offline gate checks the spray/aim/equip method bytes against the base and Work Plan C# changes as class names only. No source file in another worktree was edited. No GitHub issue or live coordination edit was made.

## First-arrow investigation: verified mechanism and remaining uncertainty

**Verified source facts:** the target getter already required a confirmed V2 plan, not a first spray or equipped marker. No first-spray prerequisite or persistent help latch was found. Stable-ID iteration could select a distant northern rack first on a differently authored plan. The minimap never projected that field target. Separately, `WalkingHudView.Refresh` hid the arrow whenever distance was less than half an ecology cell, even for a still-unmarked rack; that can remove a first arrow before spraying. The repair applies that arrival suppression only to ordinary waypoints and keeps rack guidance until marking. The new spatial visit order repairs the distant-first field sequence.

**G2 measurement defect:** the copied witness used `Q("hud-waypoint")`, which looks for an element *name*. That string is a USS class; the actual name is `hud-waypoint-panel`. The prior result described an ancestor-visibility issue, but inspection found this concrete wrong-name lookup. G5 queries the correct panel and arrow and counts rendered yellow pixels inside the arrow rectangle. It does not equate target text with a visible arrow.

**Limit:** the user's original save/edited plan was not loaded. Its specific missing arrow from the central clearing was not reproduced, and the arrival-radius finding does **not** establish the historical clearing cause (the fresh fixture's first target is 53 m away). The earlier G2 capture also showed that fresh-world arrow. Do not describe this work as a proven diagnosis of that exact historical session. The repaired fresh central-clearing route does satisfy the required rendered first-arrow proof, at zero sprays, at both resolutions. Reproduction of the original-world symptom remains an explicit review/user-retest limitation.

## O1 acceptance disposition

Capture names below exist with **both** `-1280x720.png` and `-1920x1080.png` suffixes. **R4** = `E/g5-run4-full-e115b540/evidence/`. This is the current worker evidence disposition; PASS is bounded verification, not independent approval or user acceptance.

| Item | Status | Evidence / disposition |
|---|---|---|
| O1-01 | PASS | R4 `g01-third-intro-page`, `g02-initial-objectives-cue-static`, `g03-initial-objectives-cue-motion`; native initial border assertion, offline latch/closure/menu probes. Cue ends on first Objectives visit; no saved latch. Existing 1.2 s tint code retained; ON remains default. |
| O1-02 | PASS | R4 `g04-objectives`: highlighted Stand Map CTA, no visible Next badge; native assertions. Other contexts retain their badges. |
| O1-03 | PASS | R4 `g04-objectives`, `g04c-objectives-detail-open`: screen title 36 UI units, lesson title 28, navigation 21, instruction 20. Both sizes reviewed. |
| O1-04 | PASS | R4 `g04c-objectives-detail-open`, `g04d-objectives-final-anchor`; toggle visible without scrolling, 3× open/close and Esc/reopen per size, diagram position within 2 UI units. Long detail/lower diagram notes still use the existing scroll view; no claim that all long content fits simultaneously. R1 toggle-below-viewport defect corrected. |
| O1-05 | PASS | R4 `g05-stand-map-help`, `g06-stand-map-help-detail`; exact copy probes, actual help-renderer token/prose probes and native M/Esc keycap checks. Back to forest stays ordinary button-name wording, never a keycap. |
| O1-06 | PASS | R4 `g07-plan-racks-cue-static`, `g08-plan-racks-cue-motion`; existing selector already chose `map-plan-rack` before Rack Mode/plan; native highlight assertion and safe-pulse source contract. No extra target added to pulse allowlist. |
| O1-07 | PASS (F-2 repaired) | R4 `g08a-plan-racks-editing` before Confirm, both sizes visually reviewed. Resolved accent `rgb(110,163,157)`; 712 / 2304 actual accent pixels. Initial R1 PASS claim was rejected by independent review; R3 exposed a harness width-units error, preserved below. |
| O1-08 | DEFERRED | Rotation cursor explicitly deferred by Manager; rack editor file untouched. Angle snapping also deferred. |
| O1-09 | DEFERRED | Lower-left access/rack geometry is an unapproved separate design investigation. No mask, access or entrance geometry change. |
| O1-10 | PASS with visual reservation | R4 `w02-tree-marking-intro`, `g12-work-plan-first-help`; native backdrop alpha 0.94, opaque foreground, M/O/Tab/P and backdrop click contained; Esc/Continue operate. Both sizes captured. The reviewer found faint underlying bright text despite strong dimming; user judgement remains required. |
| O1-11 | DEFERRED | Wording set 1 unchosen; orange sentence byte-preserved and checked offline. |
| O1-12 | DEFERRED | Unconfirmed “Now that you've confirmed” lead not applied; existing “After Confirm Rack Plan” remains. |
| O1-13 | PASS | Exactly the audited one-mark explanation sentence removed. Compiled copy probe preserves the entire “Marking cuts nothing…” sentence. Orange/red/blue table and in-world refusal rules remain. Wording set 2 rewrite remains deferred. |
| O1-14 | PASS | R4 `w03-before-first-spray`: actual arrow, Rack 1 label/distance and minimap destination in one frame at central spawn with 0/5. Shared getter and projection contracts pass. Static ring with Reduce Motion ON; first-appearance emphasis code checked. Historical-world limitation above applies. |
| O1-15 | PASS (F-1 repaired) | R4 `w05-after-first-spray`, `g09-top-rack5-before-second-spray`, `g10-top-second-count2-next-rack2`, `g16-edited-plan-ordinal-*`, and 20 immediate post-spray images. Ten sprays across fresh/reversed plans; four feedback surfaces checked after each (40 assertions), all without a rack number. Own stable IDs/frozen removal sets/149 Pending orders per plan preserved. |
| O1-16 | PASS | Actual surfaces 248×248 / 372×372; area ratios **2.511187× / 2.495204×** against the specified baselines; panel overlap checks pass for bottom bar, right column, reticle. See measurement table. |
| O1-17 | PASS | R4 `g11-five-of-five-tab-keycap`: actual Tab keycap in HUD instruction; no completion logic/copy rewrite. Renderer scoped to Objective 1. |
| O1-18 | PASS | R4 `w07-work-plan-pending-not-approved`, `g13-work-plan-approved`; title/navigation/section/body hierarchy, group spacing, distinct unclipped Approve and Advance. C# changes are class names only. |
| O1-19 | PASS (F-3 repaired) | R4 `g14-annual-review-before-acknowledgement` says “Read and acknowledge the Annual Review.” R4 `g15-annual-review-acknowledged` no longer does: existing next-objective instruction resumes. Asserted and visually reviewed at both sizes. Review-first refusal still passes. Earlier-year/read-only conditional cases pass offline; native historical browsing was not driven. |
| O1-20 | PASS | R4 Annual Review captures and compiled exact-copy probe: only “Read WORK DONE, MONEY and FOREST.” removed. Remaining “One thinning…” wording set 3 is unchanged. |

## Repair loop 1 — F-1, F-2, F-3 and one green full route

**Authority:** the direct-user [F-1 rack-number decision](https://github.com/jermur788/CCF/blob/docs/objective1-f1-user-rack-number-disposition-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/USER-OBJECTIVE1-F1-RACK-NUMBER-DISPLAY-DECISION-20261113.md) adds the fourteenth production file solely for the feedback produced by `MarkRackEntrance` and `ClearRackEntranceMark`. Bottom→top teaching ordinals remain 1→5; persistent identity remains unchanged. The earlier [independent review](https://github.com/jermur788/CCF/blob/docs/objective1-443-independent-review-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE1-443-INDEPENDENT-SOURCE-AND-G5-REVIEW-RESULT-20261113.md) returned NEEDS CHANGES on `1aa0fa9a` (F-1/F-2 blocking, F-3 recommended).

- **F-1:** exactly two source lines changed in `ScenarioOneManager.RackPlan.cs`; no other byte in that file differs from the base. `git grep` found no checker depending on production retaining the old strings. The old wording in feedback-matcher test examples is retained to reject leaked IDs. The offline allowlist compares the entire manager file to the base with exactly the two replacements; extra edits, unchanged copy and changed result identity are rejected. Both exact strings have a separate source contract.
- **F-2:** the selector specificity repair already present at takeover `324d6a1950a33b900492418bea5eb8c597cbc177` remains unchanged. Native R4 captures the actual editing card before Confirm and verifies its resolved accent colour and rendered border pixels.
- **F-3:** the existing `324d6a19` conditional override remains unchanged: only pending/latest acknowledgement with its footer displayed receives the review instruction. R4 verifies the pending and acknowledged states at both sizes. No acknowledgement/advance gate changed.

Exact F-1 production diff (all other lines, return values and `RackPlanOperationResult.RackId` are unchanged):

```diff
-        feedback = $"Marked the entrance tree {rack.entranceTreeId} for rack {rackId}.";
+        feedback = $"Marked the entrance tree {rack.entranceTreeId}.";
-        feedback = $"Cleared the entrance mark for rack {rackId}.";
+        feedback = "Cleared the entrance mark.";
```

### Current Tier 1

All commands used `python3 -B`, in the assigned lane, with durable output only on Files. The final harness checks passed before freezing/pushing `e115b540`:

```bash
python3 -B Tools/Verification/Objective1UxRepair/run_offline.py --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/tier1-loop1b-border-probes
python3 -B Tools/Verification/Objective1UxRepair/compile_gate.py --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/tier1-loop1b-border-compile
```

**PASS: 47 executable probes + 27 contracts; compile PASS: 193 sources / 261 installed assembly references, zero errors (two warnings).** Allowlist PASS: `E/tier1-loop1b-allowlist/final-result.json`, 14 production paths, exact two-line manager exception, unchanged spray/aim/equip methods, read-only curriculum/teaching and class-only Work Plan mapping. Earlier successful iterations remain under `tier1-loop1b-probes`, `tier1-loop1b-compile`, `tier1-loop1b-final-probes`, `tier1-loop1b-final-compile`; their output was not overwritten. The older checker failures below are historical, unchanged/unwaived; those suites were not repeated for this two-string/harness-only delta.

### Failed R3 and final R4

- **R3:** `E/g5-run3-full-a5cf0ef5-FAIL-border-width-measurement/`, frozen candidate `a5cf0ef5b63465d0c31c94a94009a20f98264814`. **FAIL**, exit 1, 162.1 s, 108 checks before the planning-card assertion; no association mismatch or save write. The harness incorrectly required `resolvedStyle.borderLeftWidth >= 3` at 720p. The declared 3-unit edge resolved to 2.5 UI units at scale 0.8, i.e. two actual pixels. Resolved colour was correct; I viewed `FAIL-state-1280x720.png` and sampled two full accent columns (`rgb(111,163,157)`). The runner exited before its directory was renamed; no evidence was removed or relabelled PASS.
- **Measurement correction:** commit `e115b540` changes only the harness and README. It keeps the exact resolved accent-colour assertion, checks the scaled width against `floor(3 × scale)` physical pixels, and additionally requires accent pixels across at least 80% of the expected edge area. This corrects the units error and adds rendered proof; no production change or acceptance waiver. The lower bound is 2 px at 720p / 3 px at 1080p; **observed** widths are 2 px / 4 px. The older harness comment/README parenthetical used the lower-bound 3 px at 1080p; it must not be mistaken for the observed 4 px measurement.
- **R4:** `E/g5-run4-full-e115b540/`, exact frozen candidate `e115b54053fe96fd2c0b85d0516aa15cb65b1387`. **FULL ROUTE PASS**, exit 0, **300.5 s, 522 checks**, zero association mismatches, zero feedback failures, no save written. No `diagram-negative` mode, skips or expected-failure pass. This one route covers onboarding through real approval/annual execution/acknowledgement, reversed-ID plan, old-world load and frozen Reference preview, at both resolutions. Editor PID **820625** exited normally; no reaper was needed.

Exact R4 launch (the harness selects and asserts both render sizes inside the private 1920×1080 display):

```bash
python3 -B Tools/Verification/Objective1UxRepair/run_gate.py --name Objective1UxNative --method Objective1UxNative.Begin --files Objective1UxRepair/Objective1UxNative.cs --native --normal-startup --screen 1920x1080 --pass-token OBJECTIVE1_UX_NATIVE_PASS --fail-token OBJECTIVE1_UX_NATIVE_FAIL --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/g5-run4-full-e115b540
```

R4 contains **84 original PNGs, 163,543,850 bytes**, with no binary additions to Git. `result.json`, `evidence/native-receipts.json`, `Objective1UxNative.log`, `driver/native-actions.jsonl`, `staged-sources.json`, `verified-summary.json` and `visual-review.json` preserve verdict, input, staged source identity, measurements and what I inspected. The immediate feedback capture now runs at **both** sizes for every successful spray, with two settle frames after resizing. Twenty captures occurred **0.649–1.194 s after the OS input request**, inside the unchanged 2.5 s gate. Ten post-spray receipts record transient/HUD text, the actual refreshed Work Plan status label, and manager feedback; all 40 number assertions pass. The Work Plan is refreshed while hidden (explicitly recorded as not visible), preserving first-use help; its pre-completion line is the existing marking-count advisory. The manager and HUD copy are neutral after all ten sprays, so an advisory cannot mask a leaked stable number.

### Current rendered proof index

All filenames below are relative to **R4 `evidence/`** and exist with **both** `-1280x720.png` and `-1920x1080.png` suffixes. I viewed the original key screenshots at both sizes before this PASS assessment; the exact viewed list is in `visual-review.json`.

| Proof | Screenshot stems | Verified observation |
|---|---|---|
| 1. Initial clearing, no spray | `w03-before-first-spray` | Genuine spawn X/Z −0.43/5.25; 0/5, help closed; yellow arrow **219 / 487 pixels**, `Rack 1 entrance · 53 m`, matching minimap Rack 1 marker. Tree PO0203, entrance Z −35.271. |
| 2. Bottom first, top second | `w05-after-first-spray`; `g09-top-rack5-before-second-spray`; `g10-top-second-count2-next-rack2` | Bottom gives 1/5 and next Rack 2 (19 m); top gives 2/5 and next still Rack 2 (52 m), while aimed/marked far tree remains Rack 5. |
| 3. Live F-1 feedback / reversed IDs | `first_spray_lowest-post-spray-rack-1-stable-1`; `second_spray_top-post-spray-rack-5-stable-5`; `second_plan_spray-post-spray-rack-1-stable-5`; `second_plan_spray-post-spray-rack-5-stable-1` | Neutral live messages at both sizes. Reversed Rack 1/tree PO0203/ID 5 captured at **0.723 / 1.122 s**; Rack 5/tree PO3803/ID 1 at **0.649 / 1.031 s**. All five reversed trees retain their own IDs and Pending orders; see table below and `SECOND_PLAN_FINAL`. |
| 4. F-2 planning card, before Confirm | `g08a-plan-racks-editing` | Visible accent edge; resolved RGBA **[0.431,0.639,0.616,1]** at both sizes; resolved width 2.5 / 3.333 UI units, physical width **2 / 4 px**; **712 / 2304** sampled accent pixels over heights **356 / 576 px**. |
| 5. F-3 Annual Review | `g14-annual-review-before-acknowledgement`; `g15-annual-review-acknowledged` | Before: “Read and acknowledge the Annual Review.” After: existing “Choose one Crop Tree in each zone, 8 m apart; include both rack sides.” The acknowledgement prompt is gone; gate refused a second advance before acknowledgement. |
| 6. Minimap / negatives | `w03-before-first-spray`; `g17-old-world-no-rack-target`; `g18-reference-future-untouched` | Minimap fit measurements below; old-world has no Tree Marks or rack target, input unchanged and slot unwritten. Frozen Reference preview opens/closes with no rack target or save write; both source-file hashes unchanged. |

| Physical visit | Stable ID | Entrance tree | Own Pending orders |
|---|---|---|---|
| 1 | 5 | PO0203 | 30 |
| 2 | 4 | PO1204 | 18 |
| 3 | 3 | PO2003 | 34 |
| 4 | 2 | PO2803 | 36 |
| 5 | 1 | PO3803 | 31 |

All 149 orders equal their owner's frozen removal set and include that owner's entrance tree. Fresh-plan geometry/order hash before and after marking: **`307C1ED538EA0D3E`** both times. Real Year-1 execution completed all 149 jobs, harvested the own removal trees, locked racks and applied the quoted **−177446 cents (−€1774.46)**; the reversed fixture remains all Pending.

| Resolution | Surface (x,y,w,h), px | Area / specified baseline | Overlap |
|---|---|---|---|
| 1280×720 | 1017,458,248,248 | **2.511187326×** (157×156 baseline) | None. Bottom bar 11,665,992,45; right column 1005,11,264,45; reticle 638,358,3,3. |
| 1920×1080 | 1525,688,372,372 | **2.495203750×** (235×236 baseline) | None. Bottom bar 17,997,1488,69; right column 1507,17,396,67; reticle 958,538,5,5. |

The native overlap check uses the whole minimap panel; panel bounds and all non-null comparison rectangles are in `MINIMAP_AREA` / `BOUNDS` receipts. These ratios are computed from actual surface dimensions against the stated approximate witness baselines, not the nominal USS ratio.

### Loop-1 save custody, leases and final cleanup

Before **and** after each R3/R4 launch, `cd / && sha256sum -c --quiet` on each absolute `host37.sha256`, `backup81.sha256`, `production4011.sha256` manifest under `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/` exited **0**. Each run has `save-integrity-before.json`, `save-integrity-after.json`, `reference-before.sha256` and `reference-integrity-after.txt` (exit 0). Both frozen hashes equal the values in the historical receipt section below. Fresh isolated `XDG_CONFIG_HOME` and save locations were enforced for each run; the player's profile/world was never loaded or reproduced.

| Run | Lease ID | Live holder PID | Outcome |
|---|---|---|---|
| R3 / `a5cf0ef5` | `84eac46d2190430eaff9520fe1e77602` | 815883 | Normal exact-owner/id release after Editor exit, then holder stopped; `lock-release.json`, `lock-free.json`. |
| R4 / `e115b540` | `a450b87d1e084c61b7bd2de06ec95382` | 820369 | Normal exact-owner/id release after Editor exit, then holder stopped; `lock-release.json`, `lock-free-final.json`. |

Owner/task: **`OpenAI GPT-6.1 Sol task443` / `#443`**. R4 lease acquired 2026-11-13 08:57:32 UTC; it records exact branch/lane/HEAD. The first detached sleep attempt exited before acquisition and was rejected as not live; no lease or Editor was created by that attempt. No other owner's lease/Editor was killed or force-released; Ops environment was never sourced. Neither completed Editor needed reaping and no Scene Backup dialog appeared.

Each runner removed its staged `.cs` and `.meta`. `licence-copy-removed.json` records licence-copy deletion after each run; final search found none under evidence configs. Exact known Editor drift was saved in each `editor-drift.patch` and restored by path: R3 `.vscode/settings.json` only; R4 that file plus nine Section Five `.mat` files (`restored-editor-paths.json`). No clean/reset/stash was used. **R4 `lock-free-final.json`: free, `unity_editor_pids=[]`; `lane-clean-before-result.txt`: empty.** Source stays frozen; the following publication is documentation only.

## Initial Tier 1 commands and actual results (historical)

All Python commands used `python3 -B`; no `__pycache__` was left in the lane. No earlier tests were modified or weakened.

```bash
python3 -B Tools/Verification/Objective1UxRepair/run_offline.py --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/tier1-delta-probes
python3 -B Tools/Verification/Objective1UxRepair/compile_gate.py --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/tier1-delta-compile
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src /media/jer/ZX20/CCF-S1-Objective1UX-20261112 --out /media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/tier1-runtime-compile
```

- Focused offline: **PASS, 36 executable probes + 21 source contracts**. Actual production visit/target/spray methods, guidance-selection statements and help-token renderer are extracted for execution with small fake neighbours; real copy and transform compile directly. Includes reversed IDs, out-of-order spray, stable tie break, invalid-target fail-close, legacy/unconfirmed controls, latch/suspension, text preservation, projection and nominal size/reservation. It does not prove real raycasts/orders/rendering; G5 supplies those checks.
- Installed-Editor-assembly syntax/type gate: **PASS**, 193 sources including the native harness, zero errors (`tier1-delta-compile.log`). Earlier runtime-only gate: 192 runtime sources, zero errors (`tier1-runtime-compile.log`). Unity's own import/compile subsequently ran without compiler errors in both native runs.
- `git diff --name-only 55863f1b..HEAD` production allowlist: **PASS**, the 13 paths listed above. Read-only/serialized/package/reference boundaries pass.
- `tier1-frozen-8bcc06f7.log` records the first frozen candidate. The correction reran focused probes and compilation before the second native run (`tier1-delta-probes.log`, `tier1-delta-compile.log`).

These existing commands ran on **untouched `2d94cc26` before edits** and again after the initial implementation:

| Command (under `Tools/Verification/`) | Untouched baseline | Candidate | Interpretation |
|---|---|---|---|
| `RackV2SuggestedLayout/run_offline.py` | PASS, 147 | PASS, 147 | Relevant layout foundation unchanged. |
| `RackV2Editing/run_offline.py` | PASS, 61 | PASS, 61 | Relevant editing foundation unchanged. |
| `NextActionGuidance/check_sources.py` | FAIL: stale button-case assertion, then missing `else return; // Later-objective` substring exception | Same | Pre-existing textual checker drift; not repaired or waived. |
| `RackV2ConfirmMinimap/run_static.py` | FAIL, one historic base-diff inventory assertion | Same | Pre-existing old-base inventory includes intervening work. |
| `RackV2HelpTeaching/run_offline.py` | FAIL: stale static predicates/base inventory; 10/97 pure checks expect old field wording | Same, plus **one expected new conflict** with O1-13's approved deletion (11/97) | The extra failure is disclosed as a current approved-copy contract conflict, not called pre-existing. New exact-copy probes check the approved wording; old checker remains unchanged. |
| `ScenarioOneFiveRackDiagram/run_five_rack_check.py` | FAIL, 33 source/pin checks; standalone compile misses `ScenarioOneRackBay` | Same | Pre-existing pinned-byte/dependency drift. No diagram production source changed by #443. |

Full command output is in `E/tier1-baseline-2d94cc26/` and `E/tier1-existing-candidate/`. The old five-rack runner automatically wrote dated output under its historical D072 path; after those processes ended, only these two newly generated directories were relocated into E. Mapping: `E/tier1-relocated-legacy-runner-output.json`. Its logs preserve their original paths.

## Earlier G5 runs, screenshots and mandatory proofs (historical)

1. **R1 full route:** `E/g5-run1-FAIL-diagram-toggle-and-reference-harness/`, candidate `8bcc06f7b1842d972ba144dd664f13acc69bd54a`. **Overall FAIL**, exit 1, 326.4 s, 374 checks before failure, zero association mismatches, no save written. It completed onboarding → Objectives → Stand Map/confirm → first clearing cue → bottom/top out-of-order sprays → 5/5 → Work Plan approval → actual annual Advance/acknowledgement → second reversed-ID plan → old-world control. Visual review found the detail toggle below the 720p viewport. The final Reference capture failed because the harness incorrectly required a visible walking minimap during preview.
2. **R2 affected-assertion follow-up:** `E/g5-run2-diagram-negative-delta/`, then-final candidate `1aa0fa9a43a945cf01e145d281f2d15f38999105`. **PASS**, exit 0, 148.2 s, 106 checks, zero mismatches, no save written. Mode `CCF_O1443_MODE=diagram-negative`: initial onboarding/cues, corrected Objectives layout/3× fold/Esc cycles, old-world and both Reference screenshots. The only production difference from R1 is the Objectives detail placement; field guidance, association, minimap, copy, Work Plan and annual code are byte-identical. Those R1 proof groups were not needlessly repeated. The corrected harness excludes Reference from the *walking minimap size* assertion while retaining the Reference no-rack-target assertion.

Both used the checked-in `run_gate.py`, `--native --normal-startup --screen 1920x1080 --grace 15`, disposable `Objective1UxNative.cs`, `Objective1UxNative.Begin`, required token `OBJECTIVE1_UX_NATIVE_PASS`, failure token `OBJECTIVE1_UX_NATIVE_FAIL`. R2 additionally passed `--env CCF_O1443_MODE=diagram-negative`. The harness selects and verifies actual 1280×720 and 1920×1080 render sizes inside that private display. See `Tools/Verification/Objective1UxRepair/README.md` for exact runnable commands.

**Fixtures:** menu/map buttons invoke their existing Clickable; keyboard and spray clicks are actual OS input recorded in `driver/native-actions.jsonl`. The central-clearing position is the genuine new-game spawn, unchanged until the first-cue capture. Later trunk approaches place the player beside the target and aim through the real spray raycast; this is not a claimed continuously walked route. The second plan uses ordinary `AddRack` calls in reverse spatial order, then confirms; no IDs are rewritten. The old-world fixture is a fresh snapshot loaded with rack model 0, not a valuable personal save.

| Mandatory proof | Verdict | Exact screenshot files (relative to R1/R2 evidence directories above) |
|---|---|---|
| 1. Clearing, intro dismissed, 0 sprays, graphical arrow + Rack 1/distance + minimap | PASS | R1 `w03-before-first-spray-1280x720.png`; R1 `w03-before-first-spray-1920x1080.png`. Player X/Z −0.43/5.25; target PO0203, entrance Z −35.271; “Rack 1 entrance · 53 m”. Arrow contains **219 / 487 yellow pixels** in its own rendered rectangle. |
| 2. Bottom first; top second, count 2/5, guide still Rack 2, far prompt Rack 5 | PASS | R1 `w05-after-first-spray-1280x720.png`, `w05-after-first-spray-1920x1080.png`; `g09-top-rack5-before-second-spray-1280x720.png`, `g09-top-rack5-before-second-spray-1920x1080.png`; `g10-top-second-count2-next-rack2-1280x720.png`, `g10-top-second-count2-next-rack2-1920x1080.png`. Last pair also visibly retains “Rack 5 entrance · Orange · entrance already marked”. |
| 3. Second plan, IDs differ, own tree/own Pending work | PASS | R1 `g16-edited-plan-ordinal-1-stable-5-1280x720.png` and `-1920x1080.png`; corresponding `ordinal-2-stable-4`, `ordinal-3-stable-3`, `ordinal-4-stable-2`, `ordinal-5-stable-1` pairs. All five real sprays check own stable ID, frozen removal set and Pending orders. |
| 4. Measured minimap rectangles and overlap | PASS | R1 first-arrow pair plus `MINIMAP_AREA` receipts for every ordinary walking capture. Surface/bottom bar/right-column/reticle rectangles recorded at both sizes. |
| 5. Old-world / frozen Reference controls | PASS | R2 `g17-old-world-no-rack-target-1280x720.png`, `g17-old-world-no-rack-target-1920x1080.png`; `g18-reference-future-untouched-1280x720.png`, `g18-reference-future-untouched-1920x1080.png`. No Tree Marks/rack target on old-world load, input snapshot unchanged, disk slot unwritten; Reference preview has no rack target and closes without saving; frozen files hash-identical. |

The second plan's physical order and identities were:

| South→north visit | Stable rack ID | Entrance tree | Own Pending orders |
|---|---|---|---|
| 1 | 5 | PO0203 | 30 |
| 2 | 4 | PO1204 | 18 |
| 3 | 3 | PO2003 | 34 |
| 4 | 2 | PO2803 | 36 |
| 5 | 1 | PO3803 | 31 |

Total 149; each order set equals that rack's frozen removal IDs. Before/after marking geometry/order signatures match. The first plan's annual execution completed 149 jobs, made every frozen rack tree a stump, locked the racks and applied the quoted **−€1774.46** cash change. A second annual advance was refused until the actual acknowledgement button was used.

| Resolution | Baseline surface | Actual surface (x,y,w,h), screen px | Actual area / baseline | Overlap |
|---|---|---|---|---|
| 1280×720 | 157×156 | 1017, 458, 248, 248 | **2.511187326×** | None with bottom bar, right column or reticle |
| 1920×1080 | 235×236 | 1525, 688, 372, 372 | **2.495203750×** | None with bottom bar, right column or reticle |

Ratios use the stated witness baselines, whose original sizes are approximate; these are not claimed as more accurate baseline measurements. The resulting size is within 2.3–2.6× at both sizes; it is not rounded up to “2.5×”. Source-only nominal ratio `(310/196)^2` is distinct from these rendered measurements.

`E/g5-core-summary.json` extracts the core receipts while preserving R1's FAIL status. Full evidence is each run's `evidence/native-receipts.json`, Unity log, `result.json`, `summary-lines.txt`, `staged-sources.json` and native-action log. R1 contains 57 PNGs (~102 MB); R2 contains 18 PNGs. They remain in durable evidence storage; no large binary screenshots were added to Git. R1 was renamed only after its runner exited; internal log paths still identify its original `g5-run1` location.

## Earlier save, lease and cleanup receipts (historical)

Each protected manifest ran as `cd / && sha256sum -c --quiet <absolute-manifest>`; all three exited 0 before and after both runs:

- `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/host37.sha256`
- `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/backup81.sha256`
- `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/production4011.sha256`

Receipts: `E/g5-save-integrity-before.json`, R1 `save-integrity-after.json`, `E/g5-run2-save-integrity-before.json`, R2 `save-integrity-after.json`. The real user's profile was not used. Each run enforced `Application.persistentDataPath` inside its fresh isolated `XDG_CONFIG_HOME`; neither wrote `forest-save.json` even there.

| Lease | Exact candidate | Holder | Receipt / outcome |
|---|---|---|---|
| `fbee85624eb34886bbd728a073347b9c` | `8bcc06f7…` | live sleep PID 702380 | `E/g5-lock-acquire-8bcc06f7.json`; R1 `lock-release.json`; exact owner/task/id release, no force; holder stopped after release. |
| `a936a3be85784c53bfc68559909729a2` | `1aa0fa9a…` | live sleep PID 707535 | `E/g5-lock-acquire-1aa0fa9a.json`; R2 `lock-release.json`; exact owner/task/id release, no force; holder stopped after release. |

Owner on both: `OpenAI GPT-6.1 Sol task443`, task `#443`. Both Editors exited normally (702717 for R1; R2 PID recorded in its `unity.pid`); neither needed reaping. The Ops environment was not sourced. Licence copies were deleted (`licence-copy-removed.json` in each run; final search found none in the evidence run configs). Staged harness `.cs`/`.meta` files were removed. Each run preserved `editor-drift.patch`; only exact known `.vscode/settings.json` and nine Section Five material paths were restored, never clean/reset/stash. R2 lists restored paths in `restored-editor-paths.json`.

Final machine receipt **`E/g5-lock-free-final.json`: `status=free`, `unity_editor_pids=[]`**. Lane was clean before this documentation commit (`E/g5-lane-clean-before-handoff.txt`). Final publication audit is `E/final-publication-audit.json`.

Frozen-file SHA-256 before/after (both after receipts exit 0):

```text
91cf50595e9b3337438323ef09609a70d7de11374569dd9fb3c6359fbdb798f0  ScenarioOneReferenceScheduleV1.json
789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc  ScenarioOneReferenceFutureV1.bytes
```

Receipts: `E/g5-reference-before.sha256` and each run's `reference-integrity-after.txt`. The Editor's SearchDatabase startup exception also appears in the earlier G2 log; it did not interrupt the harness. It is not a production compiler failure.

## Limits and handoff

- Original personal-world clearing-arrow failure remains unreproduced; its exact historical cause is unknown. The concrete source suppression mechanism, stable-ID ordering defect and missing minimap projection are distinguished above from that unknown. Independent review should assess this limitation, not assume the user's session was disproved.
- No literal Legacy40 personal save load, historical earlier-year acknowledgement navigation, disk save/load roundtrip after new marks, Player/Windows build, Tier 3, general performance/FPS qualification, integrated-main test or user acceptance was performed. The synthetic old-world model-0 load and frozen Legacy40-based Reference preview are the bounded negative controls. Existing persistence/association implementation was not changed.
- Motion ON/OFF states were captured and the preserved 1.2 s tint code checked; no video or temporal pulse-period measurement was produced. Diagram details/lower notes can still scroll as ordinary long content; its visible anchor no longer jumps.
- Clear-mark feedback has an exact offline contract; an actual native clear action was not driven.
- Independent-review reservations remain: faint bright text under the 0.94 help backdrop; full Stand Map destination follows the rack projection while stored waypoint remains unchanged; later-objective planting-hotbar fit, invalid-target problem disclosure, and cosmetic keycap/wording issues were not changed or newly qualified.
- No extra wording choice, rotation cursor/snapping, geometry, Objective 2/MFM mechanic or gate change was implemented. Deferred items need their separate authority.
- The independent source/evidence review and Manager integration verdict belong to orchestrator #447. No merge or issue posting was done here. Dashboard/Drive synchronization is **NOT VERIFIED** and remains for the orchestrator/Manager; live coordination was deliberately not edited.

**Worker assessment:** repair loop 1 and the complete R4 proof matrix are ready for **independent F-1/F-2/F-3 delta review** on exact candidate `e115b54053fe96fd2c0b85d0516aa15cb65b1387`. R1/R3 failures remain preserved; R2 remains a historical delta. The player’s own world was not reproduced. Independent approval, Manager #424 integration verdict and user acceptance remain outstanding; no merge or issue posting was performed.
