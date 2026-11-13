# #443 Objective 1 UX repair: independent source and G5 evidence review

**13 November 2026. Verdict for exact candidate `1aa0fa9a43a945cf01e145d281f2d15f38999105`: NEEDS CHANGES.**

The repair is mostly sound. Scope is clean. Spray association is unchanged and proven on both plans. The visible first arrow, Rack 1 label, distance and minimap target from the central clearing are shown in rendered pixels at both resolutions. The minimap enlargement is measured and fits.

Two findings stop an APPROVED verdict:

- **F-1 (blocking, rack-identity presentation).** After each spray, the HUD message and the Work Plan status line still name the **stable** rack ID ("Marked the entrance tree PO0203 for rack 5.") straight after the prompt called the same tree **Rack 1**. The author's own native receipts for the reversed-ID plan show this on all five sprays. It contradicts the Manager's binding rule that every surface uses the same ordinal for one physical tree. The out-of-order case ends up claiming the sprayed tree was another rack.
- **F-2 (blocking for the O1-07 PASS claim).** The Plan Racks card's accent edge does not render. A more specific existing USS rule overrides its colour, and no capture shows the Plan Racks editing card. O1-07 is FAIL as evidenced, not PASS.

Both repairs are small; see "Smallest bounded repair". This verdict is not integration approval and not user acceptance.

## Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), in T3 Code, as the review sub-agent for orchestration #447 on review claim #449. No subagents were started. I posted nothing to GitHub issues and did not edit `active-tasks.md`.
- **Did not author the code under review.** All G4 production edits, the G5 harness/runner and both G5 runs were by OpenAI GPT-6.1 Sol (author's result `e2e5afe4`).
- **Disclosure:** G1–G3 of #443 (the G2 #438 witness and the G3 bounded spec at `2d94cc26`, which this review checks against) were written by an earlier Claude Opus 5.5 session in this same project memory. Where a defect comes from the spec's own wording rather than the implementation, I say so (C-3). That earlier work is not given the benefit of the doubt here.
- **Review branch:** `docs/objective1-443-independent-review-20261113-v1`, from `origin/main` at `6c13a9e1`. This file is its only change.
- **Unity:** not launched. The Unity lock was not acquired. Nothing was written in the author's lane or on ZX20. The feature branch was not checked out anywhere: I inspected it with `git show`/`git diff` and a `git archive` extract.
- **Reviewer scratch (durable):** `/media/jer/Files/CCF/Verification/CCF-449-Objective1-IndependentReview-20261113/`. Contents: `cand/` and `base/` extracts, `offline-rerun/` + `offline-rerun.log`, `reviewer-probe/` + `reviewer-probe.log`, `probe-scripts/`.

## Method (what I actually did)

1. Read the Manager G3 verdict, the #447 orchestration packet (G5 requirements), the G3 spec and the author's result.
2. Read the full production diff `55863f1b..1aa0fa9a` (13 files), the delta `8bcc06f7..1aa0fa9a`, and the whole native harness `Objective1UxNative.cs`.
3. Hashed `TrySprayRackEntrance`, `TryRackSprayAim` and `EquipRackSpray` in base and candidate: byte-identical.
4. Re-ran the author's `run_offline.py` against the candidate extract with a git shim pointing at the shared repository and `HEAD`→`1aa0fa9a`: **PASS, 21 contracts + 36 executable probes**.
5. Wrote and ran my own 10-check probe against the extracted production methods (`reviewer-probe.log`, PASS): three and seven racks, reversed IDs, unknown ID → 0, locked southern rack, near-tie, marked rack in the middle, aimed tree versus cue, non-entrance tree.
6. Parsed both runs' `native-receipts.json` and the Unity logs. Checked staged-harness SHA-256 against the git blobs (R1 = `8bcc06f7`, R2 = `1aa0fa9a`, both match). Diffed the older checkers' FAIL sets, base versus candidate.
7. **Looked at the screenshots myself.** 720p: w01, w02, w03, w05, g02, g04, g04c, g06, g07, g09, g10, g11, g13, g14, g15, g16-ordinal-1, g17. 1080p: w03, g10, g16-ordinal-2, g18. Pixel-sampled w01 (card border) and w02 (backdrop) with PIL.
8. Re-ran the three protected save manifests (`sha256sum -c`, all exit 0). Re-hashed the frozen Reference files in the base and candidate extracts (identical, and equal to `g5-reference-before.sha256`). No Unity Editor process present.

## O1 acceptance matrix (reviewer's own disposition)

R1 = `g5-run1-FAIL-diagram-toggle-and-reference-harness/evidence/` (`8bcc06f7`); R2 = `g5-run2-diagram-negative-delta/evidence/` (`1aa0fa9a`).

| ID | Reviewer | Evidence I looked at, and notes |
|---|---|---|
| O1-01 | PASS | Source: latch set on Objectives entry, reset on intro, not persisted. Offline guidance probes (rerun). R2 g02 720p: border cue visible. Native border ≥2 px assertion is meaningful (default panel border is thinner). Motion-ON capture g03 not viewed by me. |
| O1-02 | PASS | Source: badge suppressed only for `objective-primary-cta` in O1 V2. R2 g04 720p: Stand Map CTA highlighted, no `Next:` badge. Native assertion present. |
| O1-03 | PASS | R2 g04/g04c 720p: screen title 36 > lesson title 28 > tabs 21; instruction line not smaller than the secondary line. The "More detail" toggle is small and low-contrast (C-5). |
| O1-04 | PASS | R2 receipts: diagram at identical x/y in all 3 cycles at both sizes; toggle-in-viewport assertion added after R1's failure. g04c 720p viewed: details flow in the left column, diagram fixed. |
| O1-05 | PASS (cosmetic nits) | Copy deletions verified in source and by probe. R1 g06 720p: M, M, Esc keycaps; Back to forest stays plain words. Word-per-label rendering gives wide, uneven word gaps and a detached comma (`[M] , [Esc]`), and leaves an orphaned leading "It" (C-2, C-3). |
| O1-06 | PASS | Existing selector already targets `map-plan-rack`; safe-pulse file unchanged. R1 g07 720p: `Next: Plan Racks` highlighted. w01 shows the target moved on after confirmation. g08 (motion) not viewed. |
| O1-07 | **FAIL** | **F-2.** The accent edge is overridden (pixel-sampled: 4 px grey `rgb(66,77,81)`, not accent `rgb(110,163,157)`). No capture of the Plan Racks *editing* card exists. The heading-weight rule only reaches the selected-rack heading. |
| O1-08 | DEFERRED | `StandMapRackEditor.cs` unchanged. |
| O1-09 | DEFERRED | No geometry/access/mask files touched. |
| O1-10 | PASS (reservation) | Native: alpha 0.94, opaque foreground, keys and backdrop click contained, Esc closes. R1 w02 720p sampled: underlying HUD text renders at ≈64/255 on ≈13/255 (contrast ≈1.9:1). Strongly dimmed but faintly discernible, because UI blending happens in linear colour space (N-1). |
| O1-11 | DEFERRED | Orange sentence byte-equal (probe). |
| O1-12 | DEFERRED | "After Confirm Rack Plan" lead unchanged (visible in w02). |
| O1-13 | PASS | Exactly the one sentence removed; "Marking cuts nothing…" intact (probe; w02). In-world refusals remain (`RackSprayPrompt`, `MarkPromptText`). |
| O1-14 | PASS | R1 w03 at **both** sizes viewed: yellow arrow, "Rack 1 entrance · 53 m", minimap ring and "Rack 1" label in one frame, player at genuine spawn, 0/5, help dismissed. Arrow-pixel receipts 219 (720p) and 487 (1080p). Static ring only; the motion-OFF first-appearance emphasis is source-only. |
| O1-15 | **NOT PASS until F-1** | HUD, spray prompt, minimap and 3D cue agree (w05, g09, g10, g16; receipts; probes). The post-spray message and Work Plan footer use the stable ID (F-1). |
| O1-16 | PASS | Receipts: 248×248 @720p and 372×372 @1080p, ratios 2.511 / 2.495. Bottom bar, right column and reticle rectangles are all non-null and clear of the minimap. Screenshots show no clipping in O1. Later-objective risk is N-3. |
| O1-17 | PASS (cosmetic) | R1 g11 720p: Tab keycap rendered. The keycap starts its own line and the comma is detached (C-2). |
| O1-18 | PASS | R1 g13 720p: title > tabs > section headings > body; Approve and Advance distinct and unclipped. `WorkPlanView.cs` change is two class names only (verified). |
| O1-19 | PASS for pending state; see F-3 | R1 g14: "Read and acknowledge the Annual Review." and gate refusal asserted natively. **After acknowledgement (g15) the same line stays**, telling the player to acknowledge what is already acknowledged (F-3). |
| O1-20 | PASS (spec nit) | Only the first sentence removed. The remaining sentence now has a dangling "them" (C-3); that comes from the G3 spec, not the implementation. |

## Rack-identity safety matrix

| Case | HUD arrow/label | Spray prompt (aimed tree) | Minimap ring/label | 3D cue | Post-spray message / Work Plan footer | Association | Evidence |
|---|---|---|---|---|---|---|---|
| Fresh suggested plan, IDs = physical order | Rack 1→2… | own ordinal | same ordinal | same tree | stable ID (= ordinal here, so it agrees) | own rack, 149 Pending | R1 w03/w05/g09/g10; receipts `FIRST_SPRAY_LOWEST`, `SECOND_SPRAY_TOP`, `SPRAY_FOLLOWING_CUE`; geometry hash equal |
| Out of order: bottom, then top second | next stays Rack 2 | top says Rack 5, kept after marking | Rack 2 | Rack 2 tree | "for rack 5" (agrees only because IDs = order) | own rack 5 | R1 g10 (both sizes viewed), receipts |
| Reversed IDs (`AddRack` north→south; visit 1..5 = stable 5..1) | Rack 1..5 by position (g16 viewed) | `Rack n entrance` asserted per spray | Rack 1, Rack 2 (viewed) | same tree | **"for rack 5" after spraying Rack 1; "for rack 1" after Rack 5. Disagrees (F-1)** | own stable ID, own frozen removal set, Pending, asserted per spray | receipts `SECOND_PLAN_ORDINAL`/`SECOND_PLAN_SPRAY` (all 5 messages recorded) |
| Equal entrance Z | tie by stable ID | same | same | same | (F-1 applies) | unchanged | offline probe; reviewer probe (near-tie by Z) |
| First visit-order rack's target invalid or missing | fail-closed: no rack arrow (falls back to a stored map waypoint if any) | prompt shows the problem on that tree | no ring (ordinal 0) | hidden | n/a (spray refused) | never redirected | source; offline probe. Problem text is not displayed anywhere (N-4, pre-existing) |
| Locked (harvested) southern rack | skipped; numbering keeps it | "Rack already harvested" | consistent | consistent | n/a | unchanged | reviewer probe; source |
| Already-marked rack | not cued; keeps its number | `Rack n entrance · Orange · already marked` | not shown | hidden | n/a | `NoChange` | g10; probes |
| Fewer or more than five racks | ordinals 1..n | same | same | same | (F-1) | unchanged | reviewer probe (3, 7). Confirm requires exactly 5 (`ScenarioOneManager.RackPlan.cs:243`) |
| Non-entrance tree | n/a | "Not a rack entrance" (never borrows the cue ordinal) | n/a | n/a | refusal | none | reviewer probe |

**Conclusion on Q2.** The visit index is presentation only. No path makes a spray count against a rack other than the aimed tree's own: `TrySprayRackEntrance` is byte-identical to base and resolves tree → own rack. HUD, prompt, minimap and 3D cue cannot disagree, because all read `TryGetRackFieldTarget`/`RackVisitIndex` over the same `ConfirmedRacks` snapshot in the same frame (`StandMapSource.Model` refreshes once per frame). The remaining disagreement is the feedback text (F-1). Yes: the reversed-ID evidence shows an own-ID and own-Pending-order check per spray (`EditedIdPlan`, five `SECOND_PLAN_SPRAY` receipts, `mismatches: []`).

## Negative-control matrix

| Control | Reviewer | Evidence |
|---|---|---|
| Old world (model 0) shows no Tree Marks help, no rack field or minimap target | PASS (narrow fixture) | R2 g17 720p viewed; native assertions. The fixture is a fresh snapshot with **no** racks, switched to Legacy. Adequate only because every new path is gated on `RackPlanningV2Active`; an old world that already has legacy racks was not loaded. |
| Old-world input not modified, isolated disk slot not written | PASS | Native assertion; `saveWritten: false` in both runs. |
| Frozen Reference Future v1 files byte-identical | PASS (re-verified) | I re-hashed base and candidate extracts: `91cf5059…` / `789e3edd…`, equal to the before-file. No diff under `*Reference*`. Run receipts exit 0. |
| Reference preview has no rack target | PASS | R2 assertion; g18 1080p viewed. |
| Protected personal saves (host37, backup81, production4011) | PASS (re-verified now) | All three manifests exit 0 on my rerun; run receipts exit 0 before and after. |
| Unity lock and processes | PASS | `g5-lock-free-final.json` free; no Unity Editor running at review time. |

## Findings, ranked

### F-1: Post-spray feedback names the stable rack ID, contradicting the visit ordinal (Major, BLOCKING)

- **Where:** the message text is built at `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs:419` (`$"Marked the entrance tree {id} for rack {rackId}."`, `rackId` = stable ID). `ForestTreeMarkingManager.RackEntrances.cs:157` shows it via `SetMessage(result.Message)`. It is then rendered for 2.5 s in the HUD message row (`WalkingHudView.cs:262`) and stays as `manager.Feedback` in the Work Plan status line (`WorkPlanView.cs:154`; R1 `work-plan-visible-text.txt` line 971 shows exactly this string).
- **Failure scenario (native receipts, R1):** reversed-ID plan. The prompt says `Rack 1 entrance · [Left-click] Spray orange`; after the click, `TransientMessage` = "Marked the entrance tree PO0203 for rack 5." For the top tree the prompt says Rack 5 and the message says "for rack 1". That is the player's original complaint (far tree called Rack 1), now arising inside one interaction. On the fresh suggested plan, IDs equal the visit order, so this never shows; the fixture hid it.
- **Why blocking:** the Manager's binding clarification requires the same ordinal on every surface for one physical tree, and out-of-order spraying must not claim "that sprayed tree was another rack". The repair introduced the inconsistency: the base labelled everything by stable ID.
- **Why the harness missed it:** `SprayAndRecord` records `marks.TransientMessage` but asserts nothing about it, and every g16 capture is taken before the spray.

### F-2: O1-07 accent edge does not render; claimed PASS without matching evidence (Major for the claim, BLOCKING the O1-07 PASS)

- **Where:** `ScenarioOneUi.uss:689` `.map-planning-instructions { border-left-color: var(--accent) … }` (specificity 0,1,0) loses to the existing `ScenarioOneUi.uss:265` `.modal-map .rack-panel { border-color: var(--map-rule); }` (0,2,0), applied to the same `map-edit-card`.
- **Observed:** R1 `w01-map-confirmed-1920x1080.png`: the card's left border is a 4 px grey band `rgb(66,77,81)` (the width applied, the accent colour did not), against a 1 px right border of the same grey. The result doc cites "w01 and Plan Racks captures", but the harness takes no capture between Plan Racks and Confirm, so the instruction card the player complained about (#7) was never shown. `.map-planning-instructions .rack-heading` only styles the heading that appears when a rack is selected.
- **Effect:** O1-07's "restrained accent edge" is not delivered; "visibly distinct" rests only on a slightly thicker grey rule.

### F-3: Annual Review keeps saying "Read and acknowledge" after acknowledgement (Minor, non-blocking; recommended in the same loop)

- **Where:** `ScenarioOneUiRoot.cs:560`, `if (scope == review.Root) reason = ScenarioOneRackBayText.ReviewOnScreenInstruction;`. It is unconditional for every V2 review scope, including after acknowledgement and when browsing earlier years read-only.
- **Observed:** R1 `g15-annual-review-acknowledged-1280x720.png` shows "Read and acknowledge the Annual Review." with the acknowledge button already gone.
- **Fix:** apply only while `NeedsAnnualReview` (latest year not yet acknowledged); otherwise keep the previous reason.

### Non-blocking observations

- **N-1 (O1-10 rendered occlusion).** `--backdrop` alpha 0.94 blends in linear space. Bright HUD text behind the help window still renders at ≈25 % perceived brightness (≈1.9:1 contrast). That is a large improvement on 0.64, but large bold text is faintly discernible. The native check measures the style alpha, not rendered contrast. This needs the user's own judgement in the re-test; no change requested.
- **N-2 (Stand Map Destination diamond).** `StandMapSource.cs:218` rewrites the **shared** model's `HasWaypoint`/`Waypoint`. During entrance marking, the full Stand Map's amber "Destination" diamond moves to the rack entrance (w01 shows it at Rack 1) while the player's stored waypoint (e.g. "Selected destination: H10") is unchanged. This is consistent with the HUD, which already overrode, and nothing is persisted. The source comment's "never changes the player's chosen waypoint" is true of storage, not of what the map draws. Worth a line in the user re-test notes.
- **N-3 (larger minimap and later objectives).** The minimap top now sits at about 568/900 reference units. The planting hotbar (`.hud-prompt` at 56 % + `.hud-hotbar` right-aligned) ends close to that line, and a two-line hotbar could touch the minimap. Planting is not Objective 1 and was not exercised. Check it when the later-objective HUD is next qualified.
- **N-4 (pre-existing, no arrow with no reason).** If the first visit-order unmarked rack has an invalid target, guidance fails closed by design. But `RackFieldStatus`, which carries the problem text, is not displayed anywhere (`ForestTreeMarkingManager.RackEntrances.cs:24`; no readers). Not introduced here; out of the allowlist's intent. Noted for the first-arrow investigation.
- **N-5 (composite evidence).** No full-route green run exists on the final candidate: R1 FAILED at its last capture, and R2 is a delta. I verified that carrying the proofs over is sound (Q4), but a new candidate is needed anyway, so the full route should run green once on it.

### Cosmetic nits (not blockers)

- **C-1.** "…within 3.5 / m" wraps at 720p in the HUD waypoint hint. **Pre-existing:** the G2 base capture `g2-witness-run1/evidence/w03-before-first-spray-1280x720.png` shows the same wrap. A non-breaking space in `WalkingHudView.cs` would fix it. It does not affect any acceptance item.
- **C-2.** The keycap renderers make one Label per word or segment. In Stand Map help that gives wide, uneven word gaps and a comma detached from its keycap (`M , Esc`). In the 5/5 HUD the Tab keycap starts a new line followed by " , then advance".
- **C-3.** Consequences of the G3 spec's exact-deletion wording, authored by Claude, not Sol: `AnnualReviewReadPrompt` now reads "…: when you have read **them**, …" with no antecedent (`MenuHelpView.Text.cs:78`). Stand Map help now opens a sentence/paragraph with an orphaned "It cannot mark…". These belong with deferred wording set 3 and a Stand Map copy follow-up, not this repair.
- **C-4.** The minimap "Rack n" label reuses the `.keycap` token (`HudMinimapView.cs:66`), so a place name looks like a key to press.
- **C-5.** The Objectives "More detail" toggle is small and low-contrast at 720p (R2 g04).

## Answers to the seven questions

1. **Scope: clean.** `git diff --name-status 55863f1b..1aa0fa9a` shows 13 production files, all on the allowlist, plus the G3 doc and new `Tools/Verification/Objective1UxRepair/*`. `StandMapRackEditor.cs`, `ScenarioOneTeachingSequence.cs`, both read-only LearningObjectives partials, `ScenarioOne/`, `Racks/` and `ForestSave*` are unchanged. No `.meta`, scene, prefab, asset, ProjectSettings or package change. The spray, aim and equip methods are byte-identical. Reduce Motion default ON is unchanged. There is no Objective 2/MFM, cursor, snapping, geometry, unchosen-wording or "Now that you've confirmed" change. `WorkPlanView.cs` is class names only (2 lines). Work Plan/Annual Review gate logic is unchanged; only guidance text changed.
2. **Rack identity:** see the matrix. Association is safe. The ordinal is consistent across HUD, prompt, minimap and cue. **F-1** is the one disagreeing surface.
3. **Core complaint: proven in pixels for the fresh world at both sizes** (w03 viewed at 720p and 1080p: graphical arrow, "Rack 1 entrance · 53 m", minimap ring + "Rack 1"). Bottom-first gives 1/5 and next Rack 2 (w05). Top as second spray gives 2/5, next still Rack 2, and the far tree is labelled Rack 5 before and after (g09, g10). The author's mechanism analysis is correct as far as it goes:
   - arrival suppression is real but only within 2.5 m (`cellSizeMeters` 5 × 0.5), so it cannot explain a 53 m clearing case, which the author states;
   - stable-ID ordering explains the "far rack labelled Rack 1" direction, not the absence;
   - the missing minimap projection is real.

   I found no further source path in the current code that hides the arrow for a walking player with a confirmed V2 plan and a valid first target. Help, modal and latch state do not gate it. The HUD root hides only when a screen, the Work Plan or the Reference preview is open. Equip state is irrelevant. The objective state matters only via `RackPlanConfirmed`/V2. The only remaining "no arrow" paths are: not walking; plan unconfirmed or reopened; and the fail-closed invalid first target (N-4). None of these matches the reported "arrow appeared after the first spray". The G2 base capture also shows the arrow present in a fresh world before the repair. **The result doc words the limit accurately:** original world not loaded, historical cause unknown, no proven diagnosis claimed.
4. **Carry-over: sound.** `git diff 8bcc06f7..1aa0fa9a -- Assets/` touches only the `LearningObjectivesView.cs` detail placement inside the Objectives card builder. `CurrentInstruction`/`CurrentShortInstruction`, the HUD, minimap, marking, Work Plan and review code are byte-identical. The harness delta only adds the toggle assertion, the delta mode and the Reference-preview exclusion. Staged-harness hashes match the git blobs. Fixtures: trunk-side teleport is declared and actually exercises the old <2.5 m arrow suppression (g16 shows the arrow at 2 m). Reverse-order `AddRack` is a valid "IDs differ" plan, built through the API rather than the editor UI. The model-0 fixture has no racks (narrow; see the negative matrix). Vacuity: the overlap checks would pass vacuously on a missing element, but every receipt rectangle is non-null. The arrow-pixel count sits inside a dark panel, so it is not vacuous. The real gap is the unasserted feedback message (F-1).
5. **Minimap: supported.** 248×248 and 372×372; 2.511× and 2.495×; no overlap with bottom bar (720p bar right edge 1003 < minimap panel x 1013; 1080p 1505 < 1519), right column or reticle. No text clipping in the O1 captures I viewed. The "3.5 / m" wrap is pre-existing, cosmetic and outside the minimap (C-1). Later-objective hotbar risk is N-3.
6. **O1 table:** see my matrix. I disagree with PASS on O1-07 (F-2) and O1-15 (F-1), and qualify O1-10 and O1-19. The Tier 1 probes are meaningful: they compile the real production methods with fake neighbours. They do not cover feedback text or USS specificity. **Older-checker characterisation is honest:** NextActionGuidance, RackV2ConfirmMinimap and ScenarioOneFiveRackDiagram have the same failing check names on base and candidate (the five-rack pins differ only in the pinned hash value). RackV2HelpTeaching goes from 10 to 11 failures, and the only new one is "the help states mark exclusivity and that marking cuts nothing", the approved O1-13 deletion.
7. **Negative controls: PASS** (see matrix), with the narrow old-world fixture noted.

## Smallest bounded repair (same #443 writer)

1. **F-1.** Preferred: a **one-line copy change outside the allowlist**, which needs an explicit Manager #424 scope ruling. In `ScenarioOneManager.RackPlan.cs:419`, drop the stable number: `feedback = $"Marked the entrance tree {rack.entranceTreeId}.";`. This fixes both the HUD message and the Work Plan footer, changes no logic, and no test depends on the string (`git grep` finds no other use). Allowlist-only fallback: in `TrySprayRackEntrance`, show `"Rack {RackVisitIndex(targetRack.Id)} entrance marked."` for `Applied` results instead of `result.Message`. That fixes only the HUD line, leaves the Work Plan footer naming the stable ID, and needs the offline byte-identity gate relaxed for that one line. I recommend the one-line manager copy change.
2. **F-2.** Add `.modal-map .map-planning-instructions { border-left-color: var(--accent); }` (or equivalent specificity) in `ScenarioOneUi.uss`.
3. **F-3 (recommended, cheap).** In `ScenarioOneUiRoot.cs:560`, apply `ReviewOnScreenInstruction` only while `NeedsAnnualReview`.

**Reruns required:**

- **Tier 1:** `run_offline.py`, plus new probes for (a) the post-spray feedback containing no stable rack number, or the visit ordinal if the fallback is used, and (b) the F-3 condition. `compile_gate.py`, the allowlist check (updated if the Manager grants the one file), and the old checkers' FAIL-set comparison.
- **Tier 2:** one **full-route** run of the corrected harness on the new candidate (≈5.5 min; also closes N-5), at both resolutions, adding:
  - in `EditedIdPlan` and the first plan: assert `marks.TransientMessage` and `manager.Feedback` contain no `rack {stableId}` differing from the ordinal. Capture **within 2.5 s after each spray** (at least ordinals 1 and 5 of the reversed plan) so the message row is visible;
  - a capture of the **Plan Racks editing card** (after Plan Racks, before Confirm), plus a pixel or `resolvedStyle.borderLeftColor` assertion that the left edge is the accent;
  - if F-3 is fixed, `g15` asserts the acknowledged review no longer shows "Read and acknowledge".
- Then an independent **delta** re-review.

## Verified myself versus taken from the author's receipts

- **Verified myself:**
  - production scope and protected-file invariants;
  - method byte-identity;
  - the full diff semantics of all 13 files;
  - `8bcc06f7→1aa0fa9a` delta;
  - harness code paths and vacuity;
  - offline rerun PASS and my own probe PASS;
  - the screenshots listed under Method, with pixel samples for F-2 and N-1;
  - receipt contents, including the F-1 messages, the arrow pixels, the minimap rectangles and per-spray association;
  - older-checker FAIL-set diffs;
  - Reference hashes and protected-save manifests (rerun now);
  - staged-harness hashes;
  - lock-free receipt and no running Editor.
- **Taken from the author's receipts and logs, not re-observed by me:** the native runtime facts themselves, namely that the input was real OS input, the actual year advance and cash change, the gate refusal, modal containment results and per-run save-integrity before/after. These come from Unity logs and receipts I read but could not re-execute without Unity. Captures not viewed by me: g01, g03, g04b, g04d, g05, g08, g12, w04, w07 and most 1080p twins.

## Not verified

- Original personal-world reproduction (author's stated limit; agreed).
- On-screen rendering of the F-1 message: the string is proven by receipts, and the source shows it in the HUD message row for 2.5 s and in the Work Plan footer. No screenshot exists because every capture was taken more than 2.5 s after the spray or before it. The capture in the rerun list settles it.
- Motion-OFF temporal behaviour (minimap first-appearance tint, 1.2 s border tint): source only.
- Earlier-year read-only Annual Review navigation, save/load round trip after marks, an old world containing legacy racks, the later-objective HUD with the enlarged minimap (N-3), Windows/Player build. None was required for this bounded G5, apart from the F-1, F-2 and F-3 reruns above.

**Native checks requested from the orchestrator:** only the Tier 2 rerun described above, after the bounded repair. No separate read-only native run is needed on `1aa0fa9a`, because F-1 is already settled by the author's own receipts and F-2 by the rendered pixels.
