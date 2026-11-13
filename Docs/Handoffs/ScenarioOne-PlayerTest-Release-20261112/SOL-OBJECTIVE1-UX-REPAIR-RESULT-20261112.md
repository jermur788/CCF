# #443 Objective 1 UX repair — implementation and G5 result

**13 November 2026. Implemented on the feature branch; ready for independent source/evidence review.** The five mandatory G5 proof groups pass using the completed portions of the full route plus the affected-assertion follow-up. The first run remains **FAIL**, as recorded below. This is neither integration approval nor user acceptance.

- **G1–G3 actor:** Anthropic Claude Opus 5.5. **G4–G5 actor:** OpenAI GPT-6.1 Sol, after the user-attested documented serial transfer from the stopped Claude session. No subagents were started.
- **Branch:** `feature/scenario-one-objective1-ux-repair-20261112-v1`.
- **Only writable worktree / Unity lane:** `/media/jer/ZX20/CCF-S1-Objective1UX-20261112`, Ops `Objective1UX443`.
- **Implementation base:** `55863f1b7719011bba16b3f3ab377e379c0b988a`. **Clean takeover HEAD:** `2d94cc26f4047352c2794713029ae7e8cbc82858`.
- **Canonical context snapshot in this lane:** `63044dfbbeb869996ceecec8a47619dcba048b0b`.
- **Exact final candidate:** **`1aa0fa9a43a945cf01e145d281f2d15f38999105`**, pushed. The commit publishing this result is documentation only and does not replace the tested source candidate.
- **Authority:** G3 bounded spec at `2d94cc26`; Manager G3 verdict on `origin/docs/manager-424-objective1-g3-approval-20261113-v1`; G5 packet on `origin/docs/manager-424-objective1-t3-fable-orchestration-20261113-v1`; latest direct implementation packet.
- **Durable evidence root:** `/media/jer/Files/CCF/Verification/CCF-443-Objective1UX-20261112/` (abbreviated **E** below).

## Change and boundaries

The field cue now visits confirmed entrances south to north, breaking equal-Z ties by stable rack ID. A rack's displayed visit number includes marked racks, so spraying out of order never renumbers another entrance. HUD, contextual spray prompt (including already-marked trunks) and minimap destination use the same ordinal. The minimap projects the actual entrance tree from `TryGetRackFieldTarget`, without changing the player's stored waypoint, plan records or orders. Its static destination ring remains clear with Reduce Motion ON; the first appearance can gently emphasize its border with motion OFF.

The minimap side increases from 196 to 310 UI units; the bottom control bar reserves its width. The HUD keeps a rack arrow visible inside the ordinary waypoint arrival radius until the rack is marked. Onboarding gets a session-only initial Objectives cue; O1's Stand Map CTA omits the `Next:` badge. Objectives typography and the description/diagram columns, Plan Racks card, help backdrop, help keycaps, 5/5 Tab keycap, Work Plan typography and Annual Review screen-aware instruction are repaired within the approved file set.

Exactly 13 production paths changed, all on the G3 allowlist:

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
```

Tests/runner/harness are under `Tools/Verification/Objective1UxRepair/`. No scene, prefab, ScriptableObject, ProjectSettings, package, material, `.meta`, save-schema or frozen Reference change is included. `TrySprayRackEntrance`, spray aim, equipment gating, `ScenarioOneTeachingSequence.cs`, both read-only LearningObjectives partials, Work Plan/Annual Review gates and all geometry remain unchanged. The offline gate checks the spray/aim/equip method bytes against the base and Work Plan C# changes as class names only. No source file in another worktree was edited. No GitHub issue or live coordination edit was made.

## First-arrow investigation: verified mechanism and remaining uncertainty

**Verified source facts:** the target getter already required a confirmed V2 plan, not a first spray or equipped marker. No first-spray prerequisite or persistent help latch was found. Stable-ID iteration could select a distant northern rack first on a differently authored plan. The minimap never projected that field target. Separately, `WalkingHudView.Refresh` hid the arrow whenever distance was less than half an ecology cell, even for a still-unmarked rack; that can remove a first arrow before spraying. The repair applies that arrival suppression only to ordinary waypoints and keeps rack guidance until marking. The new spatial visit order repairs the distant-first field sequence.

**G2 measurement defect:** the copied witness used `Q("hud-waypoint")`, which looks for an element *name*. That string is a USS class; the actual name is `hud-waypoint-panel`. The prior result described an ancestor-visibility issue, but inspection found this concrete wrong-name lookup. G5 queries the correct panel and arrow and counts rendered yellow pixels inside the arrow rectangle. It does not equate target text with a visible arrow.

**Limit:** the user's original save/edited plan was not loaded. Its specific missing arrow from the central clearing was not reproduced, and the arrival-radius finding does **not** establish the historical clearing cause (the fresh fixture's first target is 53 m away). The earlier G2 capture also showed that fresh-world arrow. Do not describe this work as a proven diagnosis of that exact historical session. The repaired fresh central-clearing route does satisfy the required rendered first-arrow proof, at zero sprays, at both resolutions. Reproduction of the original-world symptom remains an explicit review/user-retest limitation.

## O1 acceptance disposition

Capture names below exist with **both** `-1280x720.png` and `-1920x1080.png` suffixes. **R1** = `E/g5-run1-FAIL-diagram-toggle-and-reference-harness/evidence/`; **R2** = `E/g5-run2-diagram-negative-delta/evidence/`. PASS means the bounded criterion was checked as described, not user acceptance.

| Item | Status | Evidence / disposition |
|---|---|---|
| O1-01 | PASS | R2 `g01-third-intro-page`, `g02-initial-objectives-cue-static`, `g03-initial-objectives-cue-motion`; native initial border assertion, offline latch/closure/menu probes. Cue ends on first Objectives visit; no saved latch. Existing 1.2 s tint code retained; ON remains default. |
| O1-02 | PASS | R2 `g04-objectives`: highlighted Stand Map CTA, no visible Next badge; native assertions. Other contexts retain their badges. |
| O1-03 | PASS | R2 `g04-objectives`, `g04c-objectives-detail-open`: screen title 36 UI units, lesson title 28, navigation 21, instruction 20. Both sizes reviewed. |
| O1-04 | PASS | R2 `g04c-objectives-detail-open`, `g04d-objectives-final-anchor`; toggle visible without scrolling, 3× open/close and Esc/reopen per size, diagram position within 2 UI units. Long detail/lower diagram notes still use the existing scroll view; no claim that all long content fits simultaneously. R1 toggle-below-viewport defect corrected. |
| O1-05 | PASS | R1 `g05-stand-map-help`, `g06-stand-map-help-detail`; exact copy probes, actual help-renderer token/prose probes and native M/Esc keycap checks. Back to forest stays ordinary button-name wording, never a keycap. |
| O1-06 | PASS | R1 `g07-plan-racks-cue-static`, `g08-plan-racks-cue-motion`; existing selector already chose `map-plan-rack` before Rack Mode/plan; native highlight assertion and safe-pulse source contract. No extra target added to pulse allowlist. |
| O1-07 | PASS | R1 `w01-map-confirmed` and Plan Racks captures; right instruction card has restrained accent edge and stronger heading. Map/legend unchanged. |
| O1-08 | DEFERRED | Rotation cursor explicitly deferred by Manager; rack editor file untouched. Angle snapping also deferred. |
| O1-09 | DEFERRED | Lower-left access/rack geometry is an unapproved separate design investigation. No mask, access or entrance geometry change. |
| O1-10 | PASS | R1 `w02-tree-marking-intro`, `g12-work-plan-first-help`; native backdrop alpha 0.94, opaque foreground, M/O/Tab/P and backdrop click contained; Esc/Continue operate. Both sizes captured. |
| O1-11 | DEFERRED | Wording set 1 unchosen; orange sentence byte-preserved and checked offline. |
| O1-12 | DEFERRED | Unconfirmed “Now that you've confirmed” lead not applied; existing “After Confirm Rack Plan” remains. |
| O1-13 | PASS | Exactly the audited one-mark explanation sentence removed. Compiled copy probe preserves the entire “Marking cuts nothing…” sentence. Orange/red/blue table and in-world refusal rules remain. Wording set 2 rewrite remains deferred. |
| O1-14 | PASS | R1 `w03-before-first-spray`: actual arrow, Rack 1 label/distance and minimap destination in one frame at central spawn with 0/5. Shared getter and projection contracts pass. Static ring with Reduce Motion ON; first-appearance emphasis code checked. Historical-world limitation above applies. |
| O1-15 | PASS | R1 `w05-after-first-spray`, `g09-top-rack5-before-second-spray`, `g10-top-second-count2-next-rack2`, `g16-edited-plan-ordinal-*`; real sprays and own stable-ID/Pending-order checks. Visit ordering is presentation only. |
| O1-16 | PASS | Actual surfaces 248×248 / 372×372; area ratios **2.511187× / 2.495204×** against the specified baselines; panel overlap checks pass for bottom bar, right column, reticle. See measurement table. |
| O1-17 | PASS | R1 `g11-five-of-five-tab-keycap`: actual Tab keycap in HUD instruction; no completion logic/copy rewrite. Renderer scoped to Objective 1. |
| O1-18 | PASS | R1 `w07-work-plan-pending-not-approved`, `g13-work-plan-approved`; title/navigation/section/body hierarchy, group spacing, distinct unclipped Approve and Advance. C# changes are class names only. |
| O1-19 | PASS | R1 `g14-annual-review-before-acknowledgement`, `g15-annual-review-acknowledged`: “Read and acknowledge the Annual Review.” Latest results acknowledged; another annual advance refused before acknowledgement. Earlier-year read-only code unchanged, not separately driven in this focused run. |
| O1-20 | PASS | R1 Annual Review captures and compiled exact-copy probe: only “Read WORK DONE, MONEY and FOREST.” removed. Remaining “One thinning…” wording set 3 is unchanged. |

## Tier 1 commands and actual results

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

## G5 runs, screenshots and mandatory proofs

1. **R1 full route:** `E/g5-run1-FAIL-diagram-toggle-and-reference-harness/`, candidate `8bcc06f7b1842d972ba144dd664f13acc69bd54a`. **Overall FAIL**, exit 1, 326.4 s, 374 checks before failure, zero association mismatches, no save written. It completed onboarding → Objectives → Stand Map/confirm → first clearing cue → bottom/top out-of-order sprays → 5/5 → Work Plan approval → actual annual Advance/acknowledgement → second reversed-ID plan → old-world control. Visual review found the detail toggle below the 720p viewport. The final Reference capture failed because the harness incorrectly required a visible walking minimap during preview.
2. **R2 affected-assertion follow-up:** `E/g5-run2-diagram-negative-delta/`, final candidate `1aa0fa9a43a945cf01e145d281f2d15f38999105`. **PASS**, exit 0, 148.2 s, 106 checks, zero mismatches, no save written. Mode `CCF_O1443_MODE=diagram-negative`: initial onboarding/cues, corrected Objectives layout/3× fold/Esc cycles, old-world and both Reference screenshots. The only production difference from R1 is the Objectives detail placement; field guidance, association, minimap, copy, Work Plan and annual code are byte-identical. Those R1 proof groups were not needlessly repeated. The corrected harness excludes Reference from the *walking minimap size* assertion while retaining the Reference no-rack-target assertion.

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

## Save, lease and cleanup receipts

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
- No extra wording choice, rotation cursor/snapping, geometry, Objective 2/MFM mechanic or gate change was implemented. Deferred items need their separate authority.
- The independent source/evidence review and Manager integration verdict belong to orchestrator #447. No merge or issue posting was done here. Dashboard/Drive synchronization is **NOT VERIFIED** and remains for the orchestrator/Manager; live coordination was deliberately not edited.

**Worker assessment:** implementation and bounded mandatory proof matrix ready for independent review, with the original-world diagnosis limitation explicit. Recommended next step: independent review of exact candidate `1aa0fa9a43a945cf01e145d281f2d15f38999105`, R1's successful core proofs plus honest failure, and R2's correction/negative-control PASS; then Manager verdict and the user's separate personal re-test. Do not call this integrated or accepted.
