# #366 — Independent #365 repair-delta review and conditional Unity qualification

**10 November 2026. Claim:** [#366](https://github.com/jermur788/CCF/issues/366). **Return to:** [Manager #200](https://github.com/jermur788/CCF/issues/200).
**Authority:** [binding Manager handoff](https://github.com/jermur788/CCF/blob/5ddb86594ae9ca892dbeeba499c6ee7eaedcf593/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/MANAGER-365-TO-366-INDEPENDENT-DELTA-REVIEW-AND-CONDITIONAL-UNITY-QUALIFICATION.md).

## Verdicts

**SOURCE DELTA: NEEDS CHANGES.** The walking HUD repairs pass their focused checks, but the new short-instruction path omits the newest-unread-review predicate. In Objective 2 with an earlier access review acknowledged and a later Annual Review unread, the Objectives card and Map/Work Plan/Annual Review guidance still ask for Crop Trees while the HUD and highlighted action ask for review. This violates the Manager's explicit any-unread-review priority. Finding R366-01 below is a source-proven defect, not a speculative layout objection.

**UNITY: NOT RUN.** Stopped at the mandatory adverse-source gate. Independently, the human #361 Editor slot has not been explicitly released. No Editor, Play Mode, Player build, Unity lease acquisition, save/reload or candidate switch was attempted. All requested real 720p/1080p visibility and runtime checks remain NOT VERIFIED.

**Integration decision: NEEDS CHANGES, G2/G3 repair-delta scope only.** No integration or acceptance is implied. No production source was modified by this reviewer.

## Reviewer, ownership and exact pins

- Actual reviewer: **OpenAI GPT-6.1 Sol, High reasoning, Codex harness, local T3 Code**. One reviewer, no subagents.
- Independence: #363's broader base was OpenAI-authored; Claude Fable 5.2 independently reviewed it in #364 and authored #365's repair and new checks. This OpenAI review is independent of the **Claude-authored repair delta**, not a claim of different-provider independence from every historical author.
- New collision-free sparse docs-only lane: `/media/jer/Files/CCF/Operations/CCF-IndependentPlayerFeedbackDelta-20261110`; branch `docs/ccf-366-player-feedback-delta-review-20261110-v1`. Starting clean HEAD / live coordination: `5ddb86594ae9ca892dbeeba499c6ee7eaedcf593`. [RUNNING receipt](https://github.com/jermur788/CCF/issues/366#issuecomment-6764434760). Main was fetched again before handback and remained at that pin. Context-source commit: `4790c20918ca2a667b04d80262142d9b4151b60e`.
- Reviewed old source: `bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49`.
- Reviewed repaired game/verifier source: **`f6745d1082e4f4088e30964a9c1bb2389f0ceb49`**.
- Author's report-only tip: `94486d9b7641c6b6dbd8f50491517be7795aee4f`. Read the [#365 report](https://github.com/jermur788/CCF/blob/94486d9b7641c6b6dbd8f50491517be7795aee4f/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/FOCUSED-364-HUD-AND-DIAGRAM-REPAIR-RESULT.md), [#364 adverse report](https://github.com/jermur788/CCF/blob/ed87e0b6286321be5815fa2e9c62166f294ac29b/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/INDEPENDENT-363-AUDIT-AND-FOCUSED-UNITY-QUALIFICATION-RESULT.md), approved four-file spec and Manager clarification.
- Unity-tested source in this task: **NONE**. U3 remains the old qualification v3 at `fb599eecde983552dec4f06fb9efcf8e38397aed`.

## R366-01 — P2: review-first words diverge between the HUD and the other current-action surfaces

**Trigger:** Rack V2, access work open, its earlier Annual Review acknowledged, still selecting the Objective 2 sample; a later year's newest Annual Review is unread. For example, fewer than three valid Crop Trees after another year advance. These are distinct real predicates: `NeedsReviewAcknowledgement` is false at lesson index 1 when `AccessReviewed` is true, while `ui.NeedsAnnualReview` reads the newest report's `AnnualReviewSeen` flag.

1. [`CurrentShortInstruction`, lines 54–58](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/LearningObjectivesView.cs#L54-L58), checks **only** `NeedsReviewAcknowledgement`, then calls `HudInstruction`. It does not consume `ui.NeedsAnnualReview` or the review-aware `HudWords` result.
2. The actual [`HudInstruction` path](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs#L285-L289) calls [`ScenarioOneRackBayText.Hud`](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs#L39-L48). With `AccessReviewed=true`, that returns **“Choose one Crop Tree in each zone, 8 m apart; include both rack sides.”**
3. [`RefreshNextActionGuidance`, lines 448–453](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs#L448-L453), assigns that short instruction as the reason in Map, Work Plan and Annual Review. The very next condition correctly selects a review-navigation/acknowledgement target using **both** predicates. The reason is then displayed as `next-action-reason`.
4. The [Objectives card](https://github.com/jermur788/CCF/blob/f6745d1082e4f4088e30964a9c1bb2389f0ceb49/Assets/ForestPrototype/UI/LearningObjectivesView.cs#L163-L170) also uses `CurrentShortInstruction` as “What to do”; its review banner likewise tests only the lesson acknowledgement. Hiding the separate reason on this screen does not fix its card.
5. The walking HUD correctly calls `HudWords` with both predicates and instead says **“Review the year” / “Next: Read and acknowledge the Annual Review: open the Work Plan [Tab].”** Thus the current action is contradictory across surfaces.

Independent source-slice probe output, using the exact extracted `CurrentShortInstruction`, `NeedsReviewAcknowledgement` and `HudInstruction` members with supplied facts, and the compiled candidate's actual pure helpers:

```text
lesson index=1; lesson acknowledgement pending=False; latest Annual Review unread=True
actual HudWords title=Review the year
actual HudWords instruction=Next: Read and acknowledge the Annual Review: open the Work Plan [Tab].
extracted CurrentShortInstruction=Choose one Crop Tree in each zone, 8 m apart; include both rack sides.
REQUIRED REVIEW-FIRST GUIDANCE: FAIL (Crop Tree reason beside review action)
exit=1
```

This is **source-slice evidence, not a running Unity view or authenticated progressed world**. The defect follows directly from the linked predicates and callsites; the probe only confirms their word selection.

**Required repair:** make the short current-action words honor the same V2 review-priority decision as the HUD, including any newest unread Annual Review; use those consistent words on the card and window guidance. Preserve old-world behavior and normal lesson guidance after acknowledgement. Add a focused regression for `AccessReviewed=true`, newest review unread, Objective 2, covering the actual short-instruction/card/guidance path rather than only `HudWords`. Repeat with no unread review to prove Crop Tree guidance returns. No stage, scoring or save change is needed. This reviewer did not implement it.

## Scope audit and the remaining repair items

The repair commit itself (`9cbe8be4` → `f6745d10`) changes exactly **nine production UI files + six verification files**. The requested full range `bfd8e2a4` → `f6745d10` additionally contains the earlier report-only update to `OVERNIGHT-UX-SPEC-IMPLEMENTATION-RESULT.md`: 16 paths overall, not an unexplained tenth production file. `f6745d10` → `94486d9b` adds only the #365 report. No scoring, scenario/geometry authority, tree masks, scene, prefab, save schema, Packages, ProjectSettings, Reference archive or metadata is changed by the repair.

All nine production paths were read and compared:

| Path under `Assets/ForestPrototype/UI/` | Independent source assessment |
|---|---|
| `LearningObjectivesView.Sequence.cs` | Actual `HudModel` passes lesson index, both review predicates, V2/not-preview eligibility, player/bay and authentic progress to `HudWords`; all three output fields reach the widget. B1/B2 walking-panel repair is sound. |
| `ScenarioOneRackBayText.cs` | Objective 2 alone gets the three zone boxes and Crop Tree key after reviews. Objectives 3–5 retain their own release/inspection progress plus one location line, without that key/checklist. Both unread predicates suppress the crop block; Objective 5 pending release review has appropriate words. No-player/no-bay and non-V2 fallbacks are explicit. Fourth-mark scoring remains unchanged and covered by the focused check. |
| `TeachingDiagramPlan.cs` | B3 passes conservative declared-box clearance and glyph estimates below. Five genuinely uneven dashed rack suggestions and five hollow entrances remain. N9 adds distinct dashed/solid/striped-tree/harvested-with-stumps lifecycle shapes without changing state semantics. Render fit remains unverified. |
| `Map/StandMapLegend.cs` | N1/N2/N3: hollow suggestions including 0/5 and filled entrance rows are under Your plan; hollow row disappears when all drawn targets are marked; grouping preserves insertion order. Shared group headings across worlds are expressly allowed by Manager. No new V2 infrastructure source is introduced. |
| `Map/StandMapCanvas.cs` | N4: 17-unit no-wrap labels; first label extends left, last right, middle centers using percentage translation. Anchors retain the shared frame math. Source and rotated anchor calculations checked; viewport clipping/actual font layout remains unverified. |
| `ScenarioOneUiRoot.cs` | N5 walking-border fallback is V2-gated. Safe-target allowlist, modal/Help/preview suspension, one target and reduced-motion default remain intact. N6 shorter window reason contains R366-01. No automatic approval or focus action added. |
| `LearningObjectivesView.cs` | N6 extension checklist is emitted once rather than again under Full instructions; safeguards and Objective 5 reload warning remain visible. New short-selector review priority is defective as above. |
| `WorkPlanView.cs` | N7 label says before approval only if an open harvest/salvage order is Pending; otherwise approved work. `IsOpen` contains Pending/Approved only. Total still uses actual `job.CostCents`, minimum adjustment remains conditional/computed, scope covers all eligible jobs, settlement re-quotes. No pricing/approval change. |
| `LearningObjectivesView.Curriculum.cs` | N8 repeated entrance phrase removed; still identifies the headland-side entrance, with no eligibility change. |

The non-V2 `HudWords` pass-through was also compared with the pre-#363 HUD words. The manager's V2 predicate requires the explicit model and Enlarged80; Reference preview is separately excluded. Scoring and bay derivation authorities are unchanged. Old-world **rendered compatibility** is not proved by these source checks.

Annual Review provenance/disclosure and one contextual primary were inspected in their unchanged #363 paths: the source toggle keeps history selection, changes presentation only, and retains plain uncertainty qualifiers; an earlier year cannot acknowledge the latest; Walk is primary only with the acknowledgement footer hidden. The pulse allowlist excludes Approve, Confirm, Advance and Fell. These are source observations, not new runtime qualification.

### Diagram and map layout arithmetic

Independent arithmetic over the compiled candidate's exported figure data checked **all five backed labels**, five rack bands/numbers, access/track casings, pads and clearing. Assumptions: conservative segment bounding boxes, at least 30 units per text line, longest line × 0.6 × font size. These assumptions are not measured Unity glyph metrics.

| Backed label | Declared box `(x,y,w,h)` | Smallest conservative clearance, design units | Estimated glyph width / available |
|---|---|---:|---:|
| Headland | 104,100,120,30 | 7.820 | 105.6 / 120 |
| Link track | 104,343.980,140,30 | 3.560 | 132.0 / 140 |
| Existing clearing, two lines | 300,176,120,60 | 3.886 | 105.6 / 120 |
| Forest track | 320,470,175,30 | 24.590 | 158.4 / 175 |
| Old path | 350,260,130,30 | 6.000 | 105.6 / 130 |

At the fixed 1600×900 reference and design scale 1, these positions pass; their projected clearances at 720p are multiplied by 0.8 (Link track about 2.85 physical pixels). At other figure widths, `TeachingDiagram.OnFrameGeometry` scales positions and widths while flooring text at 20 units. For example, at width 780 the Link track box is 109.2 units wide while the same conservative estimate at the text floor is 120: a reflow risk, **not an observed runtime failure**. The full-width V2 layout and actual resolved size must be checked in Unity.

Map labels project from 17 UI units to about 13.6 physical pixels at 720p, 20.4 at 1080p. Three anchor positions were independently exported at 480/800 map sides and rotations 0/0.6/1.2 radians. Horizontal grow-left/center/grow-right positioning does not itself establish rotated-label collision or viewport safety. Diagram readability, lifecycle wrapping and map label fit remain NOT VERIFIED.

## Offline reruns and checker audit

Clean Git-object export: `/tmp/ccf366-delta-20261110-aw2o8lxr`, 218 selected source/tool files, with **zero post-run byte mismatches** against `f6745d10`. No author's scratch folder was overwritten. Exact original scripts were used with their documented output/scratch overrides; no checker or production edit was needed.

| Independently executed gate | Result |
|---|---|
| Typed runtime-source compiler against existing Player managed assemblies | **189 sources / 116 references / compiler exit 0 / 0 errors** |
| Source-text HUD contract + focused assertions | **PASS, 160 = 67 original + 93 new** |
| Seven typed mutations | **7 rejected**, every mutated runtime assembly compiled |
| `hud-model-bypasses-selector` source negative | **REFUSED, 3 findings** |
| Original synthetic feeder raster witness | **188.32 / 96.89**, removed-feeder control **0.00 / 0.00**; not Unity evidence |
| Additional reviewer source-slice priority check | **FAIL**, R366-01 reproduced |
| Additional in-memory short-instruction negative | **SURVIVES** the original source contract; coverage gap confirmed |

Seven original mutant names: `unsafe-pulse`, `no-teaching-overlay`, `no-feeder`, `hud-crop-prompt-in-objectives-3-5`, `hud-crop-prompt-during-unread-review`, `old-existing-clearing-box`, `legend-entrance-ungrouped`. The last four and safety/overlay mutations reject on their expected assertion text. **Qualification of the author's “own named assertions” claim:** `no-feeder` still rejects on generic LINQ `Sequence contains no matching element`, not an explicit feeder-specific assertion. It is a real kill, but the discriminator remains weaker than advertised.

The six verification files were inspected: `Checks.cs`, `SOURCE-ALLOWLIST.md`, `hud_contract.py`, `mutations.py`, `pixel_check.py`, `run_checks.py`. Original 67 assertions are unchanged apart from scratch destination plumbing; new tests cover selector outputs, conservative label geometry/old-box controls, legend order, lifecycle shapes, constants and quote/curriculum helpers. They do not execute the real scene-backed view. The source contract pins exact text in `HudModel` and selected N5/N6 lines; it is not a semantic proof of every instruction route. Replacing `CurrentShortInstruction` **in memory only** with an unconditional Crop Tree sentence leaves `hud_contract.problems(...) == []`. That explains the green suite alongside R366-01; no evidence was weakened or rebaselined.

Compiler wrapper caveat: it prints the nested compiler's exit but does not propagate it as its own process status. Here the printed compiler status, not merely the wrapper exit, was verified as 0. Endpoint tests round-trip a shared transform; actual 3D agreement rests on inspected shared constants until Unity. The Pillow raster uses a synthetic flat background and cannot prove canopy contrast, real Game view positivity, grayscale comprehension or target-resolution readability. The reviewer-written source-slice/geometry probes are supporting evidence, not independently accepted replacement verification code.

## Unity stop, machine state and protection receipt

**Stop order:** R366-01 fails Phase A; Unity is therefore forbidden by this packet. Separately, fresh #361 comments and live coordination contain no explicit human machine release. A free lock does not supersede that ownership. No request to take the human slot was made.

Read-only observations, with optional Git index locks disabled for protected lanes:

- Real `python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json`: **free**, metadata null, no Unity Editor PIDs, at checkpoint and completion. **No lease acquired; no owner ID to release.** Unity Hub presence is not Editor ownership.
- U3 unchanged on old `qualify/scenario-one-onboarding-focus-20261109-v3` / `fb599eec`; all **seven generated material files and `.vscode/settings.json` remain dirty and byte-preserved** across the protection checkpoint. No restore, stash, reset, clean or new qualification branch.
- Original registered Workbench remains on `4d95e45245d660e24b91e26754d9c57949bf3384` with its **eight original dirty paths** and matching checkpoint hashes. Source lane remains clean at `94486d9b`.
- **27/27 ignored licensed U3 assets** match the earlier #363 protected SHA-256 inventory; checkpoint-to-completion hashes also match. Three tracked Reference-related JSON/meta paths were hashed without writes. A separate final receipt verifies the actual frozen `ScenarioOneReferenceFutureV1.bytes` and its `.meta` against both the #363 inventory and reviewed Git objects: both match. Four inventoried save/preference paths were checkpointed without writes; no mismatch at completion.
- ZX20 available **38,575,669,248 bytes (~35.93 GiB)**; Files **8,355,594,240 bytes (~7.78 GiB)** at checkpoint. This is a capacity observation, not admission. No Unity storage monitor was started because execution was not admitted.
- Licence validity, actual isolated `Application.persistentDataPath`, prefs isolation and actual F5/F9 destinations: **NOT VERIFIED / admission not reached**. Hashing existing valuables does not establish a disposable play profile or complete safe backup.

The protection checkpoint was taken during this read-only review and rechecked at completion; it is not retroactive proof of Unity admission. `protection-before.json` records each finite dirty path and its SHA-256; `protection-after.json` records **zero hash mismatches and zero lane changes**. No production/source, scene, package, Reference, material, preference or save writes occurred.

**Runtime matrix:** real nonblack 720p/1080p views, all five diagram racks/labels/lifecycle, connecting feeder Full/Mini/3D endpoints and contrast, teaching words/waypoint, authentic Objectives 2–5, review/ACK/resume, safe pulse/reduced motion, history disclosure/primary, quote/approval and Legacy40/Reference behavior are all **NOT RUN / NOT VERIFIED** in this task. No before/after Unity captures exist. No synthetic map was presented as a real screenshot.

Original historical failures remain unchanged and were not rerun: strict Workbench runner exit 1, StandMapParity ×2, save03b, UxPedagogy 38 FAIL, #360 synthetic 171 PASS / 2 FAIL. The 26 black #360 captures remain invalid as visible evidence. No large historical campaign, Windows/Player build, main merge, external demo or user acceptance.

## Raw evidence and reproducibility

External archive: `/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-366-IndependentDelta-20261110/`. It contains full-range and repair-only diffs, export path inventory/integrity, compile project, original rerun stdout, per-mutant rejection logs, source-slice probe source/output, contract-gap negative, exported figure/rotated map anchors, geometry arithmetic and protection receipts. Supporting probe projects remain external; only this Markdown is published on the reviewer branch.

| Evidence | SHA-256 |
|---|---|
| `compile.txt` | `4f967a31d456ff8d90a3e1c14075f3243569e3435d6f77db811e81cae42b5cea` |
| `checks.txt` | `ec11aa25bcfb9851c3917f56aacd16ad54847e99bc38e349df6a07971eb99f30` |
| `mutations.txt` | `9ed38658992b06a6eccdb2910bf15eb35ea2a745cf15cdb1973146aa94a3d908` |
| `pixels.txt` | `a058aec2bdaa7e3a5c6530b271b828d1cd108fdd9cd45b1bc6c984f6c7200c45` |
| `review-priority-probe.txt` | `5e2315ce45a3510c02f77a586050577626554efc7a2827dc71a623c08068b923` |
| `contract-gap.txt` | `42f2a46fa1e8973d16d3f8e80271f34773b6843f02e3297270714798820ba71c` |
| `geometry-review.txt` | `0f5ddb1095c4ee9314e1e26c9e331212f79cbd7b1ba0e99e9446af471a84c622` |
| `protection-before.json` | `7e8e2d74ef54ab646a3e17c5f33aea676762f86ddecbbd563c334c9996a8dbba` |
| `protection-after.json` | `cb79b2ec976ce911e8ec8ef5c0323568ba25f66978663e8f6594ba4e0b2be6ae` |
| `frozen-reference-receipt.json` | `42a3cbf980c7d36806ad0dfd8f029b6d581bd03ae54a17a8e0681613958e946f` |
| `SHA256SUMS.txt` (full external evidence manifest) | `ab31d75448c8e0d6ca8d9277a8026c87dd95ec9b7f1abab5fafa595abce3ac3c` |

Reproduction uses the four commands in the candidate's `Tools/Verification/PlayerFeedbackOvernight/SOURCE-ALLOWLIST.md`, changing only destinations to this task's `compile`, `checks`, `mutants` and `pixel-scratch` directories via the documented arguments/environment variables. `dotnet run --project <evidence>/probe/probe.csproj -v q` reproduces the source-slice failure while the recorded clean export exists. It does not start Unity.

## Handback to Manager #200

**NEEDS CHANGES**, not rejected: the bounded repair is mostly sound, but R366-01 leaves the explicit product decision inconsistent across current-action surfaces. Route one minimal source/test repair, then an independent delta re-check. Only after a passing source gate and explicit #361 handback may one fresh guarded warm Unity qualification be considered; all actual lease/licence/storage/save/protected-file checks still apply.

#366 execution is handed off for closure with this durable report. No further source or Unity work is running. Manager retains sole ownership of `active-tasks.md` and the separate integration decision. **Operational follow-up outstanding:** Manager coordination/dashboard publication and downstream ingestion synchronization were not performed or claimed by this reviewer; this handoff/issue result must be reflected there.

Reviewer: OpenAI GPT-6.1 Sol High, Codex harness in T3 Code.
