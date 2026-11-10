# Fable independent review — Scenario One UI Presentation V1 (#373)

## Verdict: NEEDS CHANGES

This is a reviewer assessment of Sol's source candidate. It is **not** an integration approval and not a player-acceptance statement. The Manager (#200) makes the integration decision.

The candidate does what it set out to do in source: it keeps every gameplay guard, changes no simulation, economy, save or reference data, and reorganises the HUD, inspection card, menus, Work Plan and Annual Review along the approved layout. I found no way for the new UI to approve work, advance a year, acknowledge a review or mark a tree that the old UI could not.

It still needs changes, for two separate reasons:

1. **Required proof is missing.** No Unity run exists for this candidate. Real 1280×720 and 1920×1080 screenshots, physical keyboard and cursor behaviour, text fit, performance and save reload are **NOT VERIFIED**. The acceptance checklist says a visual check can only pass with a genuine Unity render, so UI-AC19 and UI-AC20 cannot pass on this evidence.
2. **Source defects were found** that should be repaired before a Unity session is spent on this exact candidate. Three are major: a Work Plan explanation that became unreachable, MFM candidate keys that are no longer shown where the player needs them, and an inspection card whose first page is estimated to be at or over its fixed height with clipping and no scroll.

Nothing here supports REJECTED. The design direction is sound and the repairs are small and local to the UI files.

## Reviewer, independence and lane

| Item | Actual value |
|---|---|
| Provider / model | Anthropic Claude Fable, `claude-fable-5-2`, Claude Code harness inside T3 Code |
| Session | Top-level thread `mcp:launch:f47955829fd655f7b969a1ba45e13579201ff38b87ad341fc0c3fee9605ae9f3`, no parent thread. Not a child or helper of Sol's thread `822e9b9d-…` |
| Interruption | The first run stopped after pre-flight at 20:25 UTC on an Anthropic usage limit. The user resumed the same thread. No finding or verdict existed before the interruption; the review below starts from the diff |
| Second interruption | The same limit stopped the thread again at about 21:00 UTC, after this report was written and all probes had finished, but before it was committed. The user resumed the same thread at 21:11 UTC. T3 moved it to a second Anthropic provider instance (`claudeAgent_claude_b_pro`) and passed the earlier work across as a summary, not as full history. Before publishing I re-read this whole report, re-checked F-01 to F-04 and the pre-flight facts against the source and the evidence folder, and added two small points to F-15. Model and provider are unchanged; Sol had no part in the continuation |
| Role | Review only. Game source read-only. No source fix was made |
| Worktree / branch | `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-FableReview`, `review/scenario-one-ui-presentation-v1-fable`, clean at start |
| Source reviewed | `749c5c4f93ede68356a90c42dfaeff9df0aec060` (one commit on base `4db486db7a132bd01725442ef33e00f702c4dff9`) |
| Source-lineage context | `63044dfbbeb869996ceecec8a47619dcba048b0b`, plus the approved spec on `spec/scenario-one-ui-presentation-20261110-v1` at `99c3bed1` |
| Author report read | `199b5a6edae36f27d554cdc142a8d7797d11350e`, which is the source commit plus one report file |
| Owned output | This file only |
| Unity | NOT OWNED, NOT RUN. No Player, no build, no save or preferences access, no device mounted |

**Independence, stated plainly.** I am independent of Sol's #372 implementation: different provider, separate session, no shared context. I am **not** independent of the design lineage. Earlier Claude Fable sessions wrote the #362 UX review, the #364 audit and the #365 HUD repair that sits in the base history, and the poster files used as the style reference live in the `CCF-362-FableUXReview-20261110` evidence folder and appear to have been built there. The Manager should weigh that when reading the visual comments. It does not affect the source findings, which cite file and line.

## Pre-flight facts I checked myself

- Live GitHub `main` coordination was read at `bd6a2155` at start and again at `16fd7009` before publishing. The Manager's entry at `16fd7009` records this review as running. I did not edit it.
- #372 is CLOSED / HANDED-OFF. `ui-writer-372.json` reads `RELEASED`. The author branch tip is still `199b5a6e`. Source is frozen.
- The only files changed between base and candidate are 11 files under `Assets/ForestPrototype/UI/` and 3 new files under `Tools/Verification/ScenarioOneUiPresentation/`. No scene, prefab, material, `.meta`, settings, package, save, ecology or reference file changed. No existing test file changed.
- `RefreshNextActionGuidance`, `ApplyNextActionGuidance`, `AnimateGuidance` and `HandleKeys` in `ScenarioOneUiRoot.cs` are byte-identical to base (hashed separately, not taken from the author's receipt).
- `/media/jer/ZX20` is not mounted. #361 is still open with the user's Editor not released.

## Findings

Severity: **Major** = fails or is likely to fail an acceptance check in a core flow; fix before the render gate. **Medium** = fix in the same repair, or accept with a recorded Manager decision. **Minor** = small, can be batched. **Info** = no action required from Sol.

Every "reproduction" below is a source-derived state to check in Unity. None was run in Unity.

### Major

**F-01 — Locked Work Plan sections lost their explanation and cannot be opened.**
Where: `WorkPlanView.cs:201-210` (`Card`), `:446-452` (`LockedCard`).
What: `Card` now returns a `Foldout` that starts closed, and puts the sub-text inside the foldout. `LockedCard` then calls `SetEnabled(false)` on that foldout. A disabled foldout cannot be clicked open, so "Available in a later objective." is never shown. The comment above `LockedCard` states the old rule: "greyed and disabled with one short explanation, never hidden."
Reproduce: new Enlarged80 game, Objective 1, open the Work Plan. "Planting", "Pruning" and "Nursery — planting stock" should appear as greyed headings with no reason and an arrow that does nothing.
Two existing Unity harnesses assert this rule (`Ccf359V2Closure.cs:681` `LOCKED_SECTIONS_ARE_DISABLED_AND_EXPLAIN`, `UxPedagogyShots.cs:485`). Both test that the note's text exists under the locked element, so they may still pass while the note is invisible. A render is the reliable check.
Minimal fix: for locked cards, add the note as a visible label on the outer card (outside the foldout), or build locked cards the old way without a foldout.

**F-02 — MFM candidate keys are no longer shown on the inspection view.**
Where: `TreeInspectionView.cs:95-99` (bottom-bar hints), `:181` (the only place `reviewContext.Hint` is now shown); strings at `ScenarioOneTreatmentReview.cs:34-36`; handler at `StandMapTreatmentReview.cs:513-527`.
What: at base, inspecting an MFM candidate replaced the card's key row with "[C] Confirm as Crop Tree  [N] Reject candidate" (or "[N] Reopen candidate"). The candidate moves that text to detail page 3, "Condition and model limits", as muted body text. The bottom bar shows a generic "[C] Crop Tree <id>" and never shows `[N]`. The `[N]` handler still works, so this is a live command with no prompt in the place the player decides.
Reproduce: accept a prescription so candidates exist, inspect a candidate tree. Expect no `[N]` on the card's first page or bottom bar; it appears only after pressing V twice.
Minimal fix: append `reviewContext.Hint` to `ControlHints`, and keep it on the first page next to the candidate chip.

**F-03 — The first inspection page is estimated to be at or over the card's fixed height, and overflow is clipped. NOT VERIFIED; needs a render.**
Where: `ScenarioOneUiRoot.cs:288-294` (height cap), `ScenarioOneUi.uss:194-195` (`overflow: hidden`, children never shrink), `TreeInspectionView.cs:106-143` (page content, page indicator added last).
What: the UI scales from a 1600×900 reference (`ScenarioOneUiRoot.cs:109-112`), so 720p and 1080p share one layout. The card may be about 722 units tall (900 − 70 above − 108 below). Pages are fixed lists of labels; nothing measures them against that height. My line-count estimate for a Crop Tree's first page is about 620 units with no Fell marks anywhere, about 730 once any Fell mark exists in the stand (the stand-wide forecast and the storm sentence are added), about 775 inside the teaching area (the rack sentence is added), and more for an MFM candidate or proposal. The estimate has roughly ±10% error. The page indicator is the last element, so it is the first thing cut off.
Why it matters: this is the Objective 3 state — inspect a Crop Tree, mark a neighbour, keep inspecting. The cursor is locked, so clipped text cannot be scrolled to.
Reproduce: Objective 3, mark one tree with X, inspect a Crop Tree that has four or more neighbours inside the teaching area, read the bottom of the card.
Minimal fix: move the Fell-mark forecast and storm sentence to the existing draft-plan page and leave a one-line pointer; put the page indicator at the top of the card; then confirm in a render. A layout-time check that splits an over-tall page would be the durable version.

### Medium

**F-04 — Teaching status sentences are glued onto the "[E] Close" prompt.**
Where: `TreeInspectionView.cs:99`, `UiKit.cs:134-152`, text from `LearningObjectivesView.Sequence.cs:149-164`.
What: the rack prompt is appended to the hint string, and the prompt parser treats everything between two keys as the first key's verb. I ran the exact method with the real strings:

```text
[E]  Close · Outside the teaching footprint: removals here earn no lesson credit.
[E]  Close · Teaching rack: near zone, right side.
[E]  Close · Teaching rack: near zone, right side. Review: 1/3 tree decisions, 2/6 stand portions. Review this tree and surrounding retained stand.
[J]  Retain here
[K]  Release further (mark relevant competitors first).
```

This applies to every tree inspected during teaching stages 1–4. The same sentence is already on the card's first page.
Minimal fix: append only the key part of the prompt (`[J] …  [K] …`), or nothing when it has no keys.

**F-05 — Work Plan groups collapse while being edited, and warnings start hidden.**
Where: `WorkPlanView.cs:201-210`; headings at `:286`, `:306`, `:328`, `:473`, `:526`, `:550`.
What: the open/closed state is remembered by heading text, and six headings contain a live count ("Harvest and salvage job · 7 stem(s)"). Removing a stem, adding pruning tasks or removing a planting spot changes the heading, so the group the player is working in closes. Separately, every group starts closed, including "Orders needing attention" and the harvest card that holds the contractor minimum, the eligibility line ("Cannot proceed: …") and the commitment note. They were always visible at base.
Reproduce: open "Harvest and salvage job", press Remove on one stem.
Minimal fix: key the state by a fixed id per section; start "Orders needing attention" open, and any section that contains a problem line.

**F-06 — The [V] pages drop information the old card had.**
Where: `TreeInspectionView.cs:164-183`, `:185-209`; base file lines 213, 215-216, 227, 262.
Dropped: (a) the model-limit sentence for Oak and Beech when storms are on, "Storm events are a simplified shared model, not a species wind-tolerance band."; (b) `CropTreeCompetition.DistributionSentence` ("One neighbour contributes a large share…"), now called from nowhere; (c) the signed percentage change beside "competition now → after"; (d) the hint on an unmarked tree that marking it as a Crop Tree shows its neighbours.
Changed: the "If felled now" timber estimate used to sit beside the card for any tree being considered for felling. It is now the last pages (one or two Shift+V presses back from the first page, or four to nine V presses forward), and it is also shown for Crop Trees, which the old card deliberately did not do.
Minimal fix: restore (a) on the condition page and (b) on the neighbour page; decide (c), (d) and the Crop Tree estimate deliberately.

**F-07 — The new probe does not test the callers, so several report claims rest on reading alone.**
See "Tests" below. In short: 120 of the 157 checks are one modular-arithmetic property; the probe compiles six isolated methods against stand-in classes; no test touches `WorkPlanView`, `WalkingHudView.Refresh`, `TreeInspectionView.Refresh` or `AnnualReviewView.Refresh`. Eight plausible regressions I introduced in a disposable copy were run through all of the author's gates; the results are in the table in that section.
Minimal fix: add consumer-level checks for the hint string, the headline year selection, the acknowledgement guard and the locked-card note, in the style of the existing `ConsumerPaths.cs`.

**F-08 — Existing Unity harnesses assert the structure this candidate replaced; the report names no exclusions.**
Where: `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs:250,308,387,419,445`; `ScenarioOneUx/Qualification/UxQualification.cs:990-1057`; `ScenarioOneRackUx/RackUxShots.cs:218-224,382-386`; `ScenarioOneUxPedagogy/UxPedagogyShots.cs:737-743`; `ScenarioOneCompletion/P2/CropTreeCompetitorVerification.cs:317,342`; `ScenarioOneTeachingCopyVerification.cs:186`.
What: these look up `inspect-card` and `inspect-detail` (gone), expect "If felled now" and "Stem volume" in the same panel (now on different pages), expect the competitor list as a separate right-hand panel, and expect the status panel to hide and return. None was run. They will fail or need deliberate re-baselining when Unity is available. UI-AC21 asks for "proportionate regression PASS or exclusions named".
Minimal fix: none in production. The repair handoff should list which harnesses are superseded; changing them is a measurement-integrity (G3) decision for the Manager.

### Minor

**F-09 — X and C are advertised on trees where the handler refuses them.** `TreeInspectionView.cs:78-79,97-98` checks only living / not Crop Tree. `ForestTreeMarkingManager.Mark` refuses a tree with an orange rack-entrance mark (`:231`) and refuses C on a rack-removal tree (`:237`). The walking prompt gets this right ("Orange · rack entrance marked"); the inspection bar says "[X] Mark to fell <id>".

**F-10 — Rank number tags can now be drawn over the objective panel and the key bar.** `ScenarioOneUiRoot.cs:144-147` hides a tag when it is under the card or under `hud.RightColumn`. The comment says that column holds the objective widget; it now holds only Year/Cash. The objective widget (top-left, 620 wide) and the new bottom bar are not in the list.

**F-11 — "pending" in the Work Plan header is Open minus Approved.** `WorkPlanView.cs:135`. `CalculateTotals` (`ScenarioOneManager.cs:2337-2356`) counts every open order as open but counts an order as approved only when it has no validation message. An approved order that has since become invalid is therefore shown as pending.

**F-12 — The Annual Review detail foldout closes on every rebuild.** `AnnualReviewView.cs:151` always creates it closed. Ticking "Show data sources" (`:59`) rebuilds the page, so the detail the labels belong to disappears as the player turns the labels on.

**F-13 — Annual headline wording.** "Recorded work materials" carries the DERIVED label (`AnnualReviewView.cs:186`), and the detail column calls the same figure "Tree shelters (material)". Canopy is shown as a percentage here and as 0.00 in the detail lines and trend strip. "Forest changes" lists levels for the year; only standing trees is shown as a change. Recorded closing cash is not in the headline.

**F-14 — A neighbour with a Fell mark that is not yet a job shows no mark in its row.** `TreeInspectionView.cs:193` reads `entry.PlannedFell`, which in the per-tree report means "valid open felling order" (`CompetitorAssessment.cs:257-271`, `CropTreeCompetition.cs:17-20`). Marks become orders when the Work Plan is opened. So right after pressing X on neighbour 2, the tree is red in the forest, the bar says "Remove Fell mark", and row 2 shows nothing. The card does explain this in words ("No valid open neighbour-felling jobs…"), and the behaviour is inherited from base, so this is minor.

**F-15 — Smaller HUD and menu points.** Help [F1] is no longer shown or clickable anywhere on the walking HUD (the key still works). On the Stand Map, "Set waypoint to teaching area" moved from the header to the side column (`StandMapView.cs:196`); when it is the guided next action, the "Next:" badge is inserted before it in a vertical column and will sit above the button, not beside it. With the mouse freed, the old HUD had clickable Map / Objectives / Help buttons; the new one has none. `[O]` appears twice (widget and bar). "Year N" and "Cash" render at different sizes because `.hud-status .body` (`uss:475`) outranks the new `.hud-status Label` (`uss:603`). The Rack-first filter also removes `[X] Plan optional salvage` for an aimed windthrow stem, although that handler has no Rack-first rule (`ForestTreeMarkingManager.cs:154`). Diagram A shows Tab / M / O in the bar while inspecting; the candidate omits them there although the keys work. After a failed scenario the old strip read "· failed"; the walking HUD now has no such cue (base `WalkingHudView.cs:167-168`), and the reason is shown only in the Work Plan and Annual Review. Because the bar keeps the first label for each key, an aimed prompt that carries "[Tab] Plan" or "[Tab] Buy saplings" (`ForestPlayer.cs:931-943`) replaces the "[Tab] Work Plan" label.

**F-16 — Leftovers that make the code harder for a learner to follow.** Stale comments describing the old layout at `WalkingHudView.cs:5-6,11,16,35,138-147` and `ScenarioOneUiRoot.cs:146,221`. Members kept only as shims: `ToggleDetail`, `ShowsDetail`, `EstimateWidth`, `ShowStatus(true)`, the always-hidden `prompt` label and `SetPromptMaxWidth`. A hidden label used as a string holder (`rackKeys`, `WalkingHudView.cs:327-329`). A trailing space added to a string to force a refresh (`:249`). A scale calculation that is always 1 (`:197-201`). A whole-tree label query four times a second (`ScenarioOneUiRoot.cs:277-278`).

### Info

**F-17 — Two older static checkers were already stale at base; the candidate adds drift.** `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` check 08 fails at base (routing pin: "writes target 18 times; expected 14") and fails at the candidate on an earlier rule (the Objectives header caption changed from "Stand map [M]" to "Stand Map [M]"). `Tools/Verification/NextActionGuidance/check_sources.py` crashes at base and candidate on a removed comment anchor; one more of its rows now fails for the same caption. The candidate did not turn a passing gate into a failing one. Any future repair of check 08 must now also re-review the Objectives header, which that checker pins.

**F-18 — Proposal and candidate chips use the rack colour token.** `uss:625` uses `--rack` (purple), the same border as the Work Plan "RACK" chip. The text is distinct, so colour is not the only signal. The poster note suggested amber.

## Audit of the topics named in the dispatch

| Topic | Result from source |
|---|---|
| HUD shows Year and Cash only | Yes. `WalkingHudView.cs:158-159`. Browsing band, shelter counts, phase, intervention count, task count and the three buttons were removed from the strip. Local browsing remains in the ground report |
| Objective and review-first priority | Unchanged. `HudModel`, the guidance selector and its consumers are untouched; 219 consumer checks pass |
| Contextual valid keys, non-clickable prompts | Yes in structure. Prompts are labels with picking off. Keys come from the existing prompt strings. Exceptions: F-02, F-04, F-09, F-15 |
| Rack-first hides X and C | Yes, in three places (`WalkingHudView.cs:228,250`; `TreeInspectionView.cs:73,97-98`). Note that `RackFirstStageActive` is a display rule by its own comment (`ScenarioOneManager.Racks.cs:52-54`): the marking keys still work in that stage, as at base |
| One card; V and Shift+V paging | One card, right side. Paging wraps both ways (`TreeInspectionView.cs:45-53`; root `:223-224`). Fit is F-03 |
| Ranks and competition denominator | Correct. Shares are labelled as shares of the local competition index; the remainder is `1 − ShareOfFirst(n)` and only when the index is above zero; only existing ranks are advertised |
| MFM versus Fell | Distinct chips and words; a proposal is never called a mark or a job. Key prompts regress (F-02) |
| Wind off is not "safe" | Yes. The forecast string says "(storms off)", the first page adds "exposure does not establish wind safety", the condition page says "This is not a wind-safety assessment" |
| Fell-mark forecast versus open-job estimate | Sol's reading of the data is right and I confirmed it independently: the per-tree estimate uses valid open felling orders; `TreatmentOutcome` uses Fell marks. The two are labelled separately. Residual gap: F-14 |
| Quote, minimum, approval, advance | `AdvanceDecision`, `OnAdvanceClicked`, `BuildHarvest`, `QuoteScopeNote` and the footer conditions are unchanged. Approve is static; the pulse allowlist is unchanged and its mutant is still killed |
| Shared menu order and Back | Same six controls in the same order on all four screens. Named guidance buttons are reused, not recreated. Esc closes each screen, including the Work Plan (`ScenarioOneManager.cs:593`) |
| Annual selected-year work, money, snapshot, missing data | Headlines read the same `year` object as the detail columns. Missing report, money or snapshot each produce an explicit "not recorded" line and no zero. No automated test would notice if the wrong year were passed (see mutant m1) |
| Recorded materials versus nursery and net | The headline shows income, contractor work, materials, and "Income less contractor work", and says in words that a full net is not recorded. That is honest. Wording nits in F-13 |
| Latest unread acknowledgement versus history | The acknowledgement footer and its callback guard are unchanged. Back is enabled and no longer styled as primary |
| Provenance and details preserved | Toggle off by default; detail columns, prior treatment, storm, history, trends, objectives all still built. F-12 is a usability point |

## Acceptance checks

"PASS (source)" means the source satisfies the check as far as reading and offline probes can show. It never includes rendering or input.

| ID | Disposition | Basis |
|---|---|---|
| UI-AC01 | PASS (source); render NOT VERIFIED | Objective widget and review-first selector unchanged |
| UI-AC02 | PASS (source); render NOT VERIFIED | Year and Cash only. No automated guard (mutant m6) |
| UI-AC03 | PASS (source) with findings; legibility NOT VERIFIED | Rack-first suppression and non-clickable prompts hold. F-04, F-09, F-15 |
| UI-AC04 | NOT VERIFIED | One card exists in source. Overlap and fit need a render; F-03 |
| UI-AC05 | PASS (source) | Two denominators kept apart in every string |
| UI-AC06 | PASS (source); native NOT VERIFIED | Selection code untouched; share overlay recorded as NO-GO and omitted, which the spec allows without a render |
| UI-AC07 | FAIL (source, narrow); fit NOT VERIFIED | F-06 dropped items, F-02 demoted keys, F-03 clipping risk |
| UI-AC08 | PASS (source); render NOT VERIFIED | F-14 noted |
| UI-AC09 | PASS (source); native NOT VERIFIED | |
| UI-AC10 | PASS (source); native NOT VERIFIED | No automated guard (mutant m4) |
| UI-AC11 | FAIL (source, narrow) | F-01 loses a context outright; F-05 |
| UI-AC12 | PASS (source) | Quote methods unchanged; F-11 noted |
| UI-AC13 | PASS (source); native NOT VERIFIED | |
| UI-AC14 | PASS (source) | |
| UI-AC15 | PASS (source) | By reading; see mutant m1. F-13 noted |
| UI-AC16 | PASS (source) | By reading; see mutant m1 |
| UI-AC17 | PASS (source); native NOT VERIFIED | F-12 noted |
| UI-AC18 | PASS (source); native NOT VERIFIED | Guard unchanged; see mutants m3, m4 |
| UI-AC19 | NOT VERIFIED | No Unity screenshot of this candidate exists |
| UI-AC20 | NOT VERIFIED | No native input, focus, overlap, long-text or performance run |
| UI-AC21 | NOT VERIFIED | File boundary holds (UI only). MFM prompt regression F-02. No Unity regression run; exclusions unnamed (F-08) |
| UI-AC22 | PASS for the source-only lane | Claim, release and "no Unity, no build" are consistent with the records I could read |
| UI-AC23 | PARTLY MET | Sol result and this review are on GitHub. Manager disposition pending. Dashboard NOT VERIFIED |
| UI-AC24 | PASS | Posters opened and hashed; errata below |

## Poster errata

| # | Erratum | Disposition |
|---|---|---|
| 1 | Poster keys (`[F]`, Work Plan shortcuts) | PASS. X and C only; no new binding. The one new input read is Shift with V |
| 2 | Two percentage denominators | PASS. No "total competition 54 %" row; remainder computed from the index |
| 3 | Fell-marked neighbour with "No change yet" | PASS (source). The card states which data each estimate uses. F-14 is the remaining rough edge |
| 4 | Unverified operations | PASS. No new job type or rate |
| 5 | Made-up totals, no global score | PASS. No hard-coded figure in the added lines; no biodiversity or browsing figure in the strip |
| 6 | "Continue to Year 6" | PASS. Not present; advance stays in the Work Plan |
| 7 | Approval is a commitment | PASS (source). Static button, unchanged guards and warning |
| 8 | On-tree share labels conditional | PASS. Not added; existing rank rings and numbers untouched |
| 9 | Menu consistency | PASS (source). Four destinations, Help, Back on every screen |
| 10 | Colour not the only signal | PASS (source). Every chip has words. F-18 noted |
| 11 | No false wind or outcome precision | PASS (source). See "Wind off" above. The Work Plan makes no forest-outcome promise |

## Visual references

All comparison below is between **source** and reference pictures. No picture of the running candidate exists.

| File | Local pixels | Local SHA-256 | Matches Manager upload? |
|---|---|---|---|
| `poster-3-ui-update-sim.png` (user's preferred) | 3344×1882 | `b1cd05cd898d1f9263d15afb80c091c1f3b8488b6a5e19a2b37d2a94feffefd9` | No (upload: 2048×1152, `b4400d9b…`) |
| `poster-1-visual-reference.png` | 3072×2048 | `3e183b41705eaf3818690cb44c020e1af039d700a00dc8b83bad6cd33d28b4bf` | No (`94c9a096…`) |
| `poster-2-update-specification.png` | 3072×2048 | `f6ac1e4866432fdd674fcf4a4cd019620d63767afc6be6ac82d1c5279368e18f` | No (`3a03eea5…`) |
| `poster-3-ui-update.png` | 3344×1882 | `456384561e1b142c2944725544b8033bb0ec8d5af7bdf15eecd046a414112b42` | No (`97231805…`) |

These agree with Sol's manifest. The folder also holds the HTML each poster was rendered from and a `build.sh` that renders at 2× (the HTML sizes are 1672×941 and 1536×1024). The local files are therefore different encodes from the 2048-wide uploads. I cannot show they are pixel-equivalent and do not claim it.

I opened the preferred poster (downscaled and in three crops) and rendered the four spec SVGs with ImageMagick as previews. What the preferred poster shows: dark charcoal and teal; objective top-left; Year and Cash top-right; one right-hand tree card; a bottom key bar; a Work Plan table with a stepper; an Annual Review with three headline boxes. It also shows the errata items literally: `[F] Mark to fell`, "Competition (growth reduction)", "Total competition 54%", a red-marked neighbour beside "No change yet", per-hectare rates, and "Continue to Year 6".

Source against the four diagrams:

- **A, HUD.** Positions match: objective left, strip right, card right, bar bottom. Differences: the diagram's bar keeps Tab / M / O while inspecting; the diagram's card shows "Marks: Fell 1 | Crop 2" in the plan block, which the candidate leaves in the separate bottom-left summary.
- **B, Work Plan.** Same parts, different arrangement: the diagram puts operations and quote side by side and shows each group's status on its closed row; the candidate stacks the quote above closed groups whose headings carry counts but no pending/approved status.
- **C, Annual Review.** Matches closely: three headline boxes, detail below, acknowledgement separate from Back. The diagram's "Net for the year" is replaced by the narrower, honestly labelled "Income less contractor work".
- **D, navigation.** Matches.

Whether any of this looks right on screen is NOT VERIFIED.

## Tests

### Repeated on the exact candidate, in my own evidence folder

| Gate | My result | Same as author |
|---|---|---|
| Type check, 189 sources / 116 references | `exit=0 errors=0`, `check.dll` produced | Yes |
| Existing feedback checks | PASS 160 | Yes |
| Existing consumer slices | PASS 219; HUD source contract PASS | Yes |
| Annual unread lifecycle | PASS 18 | Yes |
| Existing guard mutants | 8 killed, 4 source negatives refused | Yes |
| Annual-reset guard mutant | Killed by `R370_YEAR2_NO_WORK_REPORT_UNREAD` | Yes |
| New presentation probe | PASS 157; 5 mutants killed | Yes |

The type check is Roslyn against an existing Player's assemblies. It is not a Unity import or Editor compile.

### What the new probe does and does not show

`Tools/Verification/ScenarioOneUiPresentation/` copies six methods out of the source (`MenuNavigation`, `KeyPrompts`, `NextPage`, `NeighbourHint`, `BuildHeadlines`, `Column`) and compiles them against small stand-in classes. That is a fair way to exercise those six methods and the README says clearly that it proves no layout or input.

Limits the report does not spell out:

- 120 of the 157 passes are the same property (`NextPage` wraps) for page counts 1 to 40. About 25 distinct assertions remain.
- `BACK_NEVER_ACKNOWLEDGES_OR_ADVANCES` checks that a stand-in `CloseAll` was called. It cannot see acknowledgement or year state.
- `MONEY_FROM_SELECTED_YEAR` checks that `BuildHeadlines` prints the object it is given. Which year is given is decided in `Refresh`, which the probe does not compile.
- The stand-in `AddTagged` discards the provenance kind, so Recorded / Derived labels are not asserted.
- Paging content — which facts are on which page, the page count, the reset on a new tree — is not tested.
- Nothing covers `WorkPlanView`, the HUD refresh, the inspection hint string, the map header or the objectives header.
- The eight "original" mutants all sit in files this change did not touch. They show the old guards still work; they do not guard the new code.

### Reviewer mutants

To turn "not covered" from an opinion into evidence, I copied the candidate's C# to a disposable folder outside the repository, introduced one plausible regression at a time, and ran every author gate against the copy (type check, 160 + 219 + HUD contract, 18, 157). A survivor means no offline gate would notice that regression. It is not a defect in the candidate.

Two controls were run first through the same script to prove it reads the mutated copy: the author's own "wrong-year money" mutant was killed by the presentation probe, and "Approve pulses" was killed by the existing feedback checks.

| Mutant | Regression introduced | Code is | Type check | 160 + 219 + HUD | 18 | 157 | Result |
|---|---|---|---|---|---|---|---|
| m1 | `Refresh` passes the latest year to the headlines while an earlier year is selected | new | ok | PASS | PASS | PASS | **Survived** |
| m2 | Both callers stop hiding X and C in Rack-first (HUD passes `false`; inspection hints ignore the stage) | new | ok | PASS | PASS | PASS | **Survived** |
| m3 | The acknowledge button's "not an earlier year" callback guard is removed (the footer is still hidden for earlier years) | kept from base | ok | PASS | PASS | PASS | **Survived** |
| m4 | Back to forest is disabled while the latest review is unread | changed line | ok | PASS | PASS | PASS | **Survived** |
| m5 | Data-source labels shown by default | kept from base | ok | PASS | PASS | PASS | **Survived** |
| m6 | Deer-browsing band added back to the Year/Cash strip | new | ok | PASS | PASS | PASS | **Survived** |
| m7 | Every neighbour row labelled "Open fell job" | new | ok | PASS | PASS | PASS | **Survived** |
| m8 | "Approve Work Plan" also advances the year | kept from base | ok | PASS | PASS | PASS | **Survived** |

All eight survived. So the statements "selected-year money", "Rack-first suppression", "Back enabled before acknowledgement", "Year and Cash only", "open-job labels" and "approval and advance stay distinct" are true of this candidate by my reading, but no offline gate would catch their loss. m3, m5 and m8 show the same gap already existed for code this change kept.

### Existing checkers the author did not run

Run by me at the candidate and, for comparison, on a text export of base: see F-17. Logs are in the evidence folder.

## Accuracy of the author's report

Sol's report is careful about its limits and does not claim a render or a Unity run. The points below are where I would correct or qualify it.

| Claim in the report | My finding |
|---|---|
| "Existing test assertions unmodified" | True |
| Work Plan "keeps all … warnings and editing/cancellation controls" | Not fully: the locked-section explanation is unreachable (F-01) |
| [V] pages "retain site/light/growth … wind/model limitations" | Mostly: four items dropped (F-06) |
| "Keys identify the actual aimed/chosen marking target" | True for the target; not for whether the handler will accept it (F-09), and MFM keys are missing (F-02) |
| "PASS 157" | True, with the weighting above |
| "All eleven errata were considered" | Agreed; all eleven pass in source |
| Acceptance table "Source PASS" for UI-AC07 and UI-AC11 | I disposition both FAIL (narrow) |
| Regression scope | The report lists what was run. It does not mention the two stale static checkers or the Unity harnesses that no longer match (F-08, F-17) |

## Recommended next steps

1. One bounded source repair by a single UI writer: F-01, F-02, F-03 (move the forecast, move the page indicator), F-04, F-05, F-06 (a) and (b). F-09 to F-16 can ride along or be batched.
2. Add a few consumer-level checks alongside (F-07), so the repaired claims are held by a test.
3. Manager decision on the superseded Unity harnesses and the two stale static checkers (F-08, F-17) before any Tier-2 is planned.
4. A short re-review of the repair diff. It does not need another full review.
5. Then one focused, guarded Unity Tier-2 on the repaired candidate, covering at least: Objective 1 Work Plan (F-01); Objective 3 Crop Tree inspection with a Fell mark, inside the teaching area, all pages forward and back (F-03, F-04); an MFM candidate (F-02); Work Plan edit and remove (F-05); latest unread review with Back then acknowledge; an earlier year; 720p and 1080p, colour and grayscale.

I am not asking for a large retest, and nothing here approves an external demo or a Windows build.

## Reviewer decision record

```text
INTEGRATION DECISION
Status: NEEDS CHANGES   (reviewer assessment; the G4 integration decision is the Manager's)
Gate: G2 acceptance — source conformance and sufficiency of evidence for UI Presentation V1
Reviewed branch: feature/scenario-one-ui-presentation-v1-sol
Reviewed HEAD: 749c5c4f93ede68356a90c42dfaeff9df0aec060
Context commit: 63044dfbbeb869996ceecec8a47619dcba048b0b (source lineage) + approved spec at 99c3bed1
SPEC (SPEC REQUIRED only):
- ID / status: specs/scenario-one-ui-presentation-v1 — APPROVED FOR BOUNDED IMPLEMENTATION
- Conformance: NOT VERIFIED overall (render and native checks missing); FAIL (narrow) on UI-AC07 and UI-AC11 in source
Reasons:
- No Unity evidence for this candidate: UI-AC19, UI-AC20 and the render half of most checks.
- Source findings F-01, F-02, F-03 (major) and F-04 to F-08 (medium).
Required fixes / remaining gates:
- Source repair of F-01 to F-06; test additions for F-07; Manager decision on F-08 and F-17.
- Guarded Unity Tier-2 on the repaired candidate; G3 if any harness is changed; G4 at integration.
Integration target: none proposed by this review
Integrated HEAD: n/a
Post-integration verification: n/a
```

```text
CLOUD HANDOFF — NOT UNITY-VERIFIED
Base: 4db486db7a132bd01725442ef33e00f702c4dff9
Context commit: 63044dfbbeb869996ceecec8a47619dcba048b0b
HEAD: 749c5c4f93ede68356a90c42dfaeff9df0aec060 (reviewed source); this report is one docs commit on top
Branch: review/scenario-one-ui-presentation-v1-fable
GitHub handoff path: Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md
GitHub handoff commit: recorded in the #373 closing comment
Files changed: this report only
Serialized Unity files touched: none
Package/ProjectSettings changes: none
MUST-NOT-CHANGE HASHES / ANCHORS:
- Not rerun. No simulation, save, RNG or reference file differs between base and candidate.
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
Tests actually run: offline only — type check; 160 + 219 + HUD contract; 18; existing guard mutants; presentation 157 + 5; two older static checkers; reviewer prompt probe; reviewer mutants.
UNITY GATES STILL REQUIRED:
- Everything in "Recommended next steps" item 5; UI-AC19, UI-AC20, UI-AC21.
Suggested commands/menu actions:
- None until the user releases the Editor and the Manager admits a lane.
Expected observable behaviour:
- As listed per finding under "Reproduce".
```

## Commands and evidence

Evidence root (outside the repository): `/media/jer/Files/CCF/Verification/ScenarioOneUiPresentationV1FableReview-20261110/`

| What | Where |
|---|---|
| Reviewer registration, and the interrupted-run record | `reviewer-373.json`, `reviewer-373.blocked-by-quota-20261110T2028Z.json` |
| Repeated gates | `repeat-compile.log`, `repeat-feedback.log`, `repeat-reset.log`, `repeat-reset-mutant.log`, `repeat-presentation.log`, `repeat-feedback-mutants.log`, outputs under `repeat-749c5c4f/` |
| Older static checkers, candidate and base | `existing-baseline-postfix-candidate.log`, `existing-baseline-postfix-BASE-4db486db.log`, `existing-nextaction-candidate.log` |
| Prompt probe (real strings through the exact `KeyPrompts`) | `reviewer-probes/keyprompts/` |
| Reviewer mutants | `reviewer-probes/mutants/run_reviewer_mutants.py`, `run.log`, `out/summary.json` |
| SVG copies, previews, poster crops | `visual-refs/` |

Commands, from the review worktree (`SRC`) with `EV` as the evidence root:

```bash
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$SRC" --out "$EV/repeat-749c5c4f/compile"
ASM="$EV/repeat-749c5c4f/compile/bin/Release/netstandard2.1/check.dll"
python3 -B Tools/Verification/PlayerFeedbackOvernight/run_checks.py "$ASM" "$EV/repeat-749c5c4f/feedback" "$SRC"
python3 -B Tools/Verification/PlayerFeedbackOvernight/review_reset.py "$ASM" "$EV/repeat-749c5c4f/reset" "$SRC"
python3 -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$SRC" --out "$EV/repeat-749c5c4f/presentation"
CCF_FEEDBACK_COMPILE="$EV/repeat-749c5c4f/compile" CCF_FEEDBACK_MUTANTS="$EV/repeat-749c5c4f/feedback-mutants" \
  python3 -B Tools/Verification/PlayerFeedbackOvernight/mutations.py
python3 -B Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
python3 -B Tools/Verification/NextActionGuidance/check_sources.py
python3 -B "$EV/reviewer-probes/mutants/run_reviewer_mutants.py" "$SRC" "$EV/reviewer-probes/mutants/out"
```

All were run with `UseSharedCompilation=false`. The base comparison for the older checkers used a text-only `git archive` export in a scratch folder with a private index file; no worktree, branch or checkout was created and the review worktree stayed clean throughout.

## Not done

- No Unity, Player, build, save, preferences or device access.
- The Unity harnesses listed in F-08 were read, not run.
- Line-count height estimates in F-03 are arithmetic, not measurements.
- **Dashboard sync: NOT VERIFIED.** No publisher was established in this lane and no generated dashboard file was touched. The Manager must reconcile live coordination and confirm ingestion.
