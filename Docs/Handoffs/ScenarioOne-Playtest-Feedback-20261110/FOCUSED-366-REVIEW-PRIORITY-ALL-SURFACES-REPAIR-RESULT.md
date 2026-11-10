# #367 — Annual Review priority across Objectives and window guidance

**10 November 2026 · Parent [Manager #200](https://github.com/jermur788/CCF/issues/200) · Claim [#367](https://github.com/jermur788/CCF/issues/367).**

**IMPLEMENTED ON TASK BRANCH; author offline verification PASS. Ready for independent review. Not self-approved, integrated, Unity-qualified or user accepted.** This repairs R366-01; the original #366 NEEDS CHANGES verdict and historical FAIL evidence remain unchanged.

Binding scope: [Manager #366 → #367](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/MANAGER-366-TO-367-REVIEW-PRIORITY-ALL-SURFACES-REPAIR.md). Original adverse review: [#366 result](https://github.com/jermur788/CCF/blob/74e4493335e8ce0b529a5a05d1b0924c8ecd2399/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/INDEPENDENT-365-DELTA-REVIEW-AND-UNITY-QUALIFICATION-RESULT.md).

## Identity, ownership and pins

- Worker: **OpenAI GPT-6.1 Sol, High**, Codex harness in T3 Code; direct user dispatch. No delegation. The packet preferred the previous Anthropic author; this user dispatched the present Codex session instead. This worker is now an implementer and cannot independently approve this repair.
- Role: Primary. Task class: PACKET ONLY. Tier 1 offline/source only.
- Existing Files lane: `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`.
- Branch: `feature/scenario-one-player-feedback-overnight-20261110-v1`.
- Starting report-only HEAD: `94486d9b7641c6b6dbd8f50491517be7795aee4f`; preceding game/verifier source `f6745d1082e4f4088e30964a9c1bb2389f0ceb49`.
- Source-context snapshot: `63044dfbbeb869996ceecec8a47619dcba048b0b`; newer binding Manager packet controls this bounded repair.
- **Corrected source + verifier commit: `85568c4f60b09279a2e1692bd0e7f8593e1b9a0d`.**
- This document is the separate report-only successor. Its exact report pin is the commit in the pinned GitHub handoff URL posted to #367 and #200. Resolve locally with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/FOCUSED-366-REVIEW-PRIORITY-ALL-SURFACES-REPAIR-RESULT.md`.
- Live preflight: GitHub main coordination blob `2e3848a5ba04945480b822f1d0e86342b0ed22f1` explicitly assigned #367 READY; transitional ops coordination also read. #365 closed/HANDED-OFF, no earlier #367 worker claim, clean index/worktree including untracked, local HEAD equal remote, only one worktree on the feature branch. [Actual RUNNING receipt](https://github.com/jermur788/CCF/issues/367#issuecomment-6764995451).
- Storage: Files ext4 `/dev/sdb4`, approximately 7.8 GiB available at admission. No new checkout or worktree.

## Player-facing repair and exact consumers

One pure presentation rule now serves the walking HUD and current-action consumers: Rack V2 active, outside Reference preview, and either this lesson's acknowledgement missing **or the newest report unread**. No lesson/scoring/persistence predicate changed.

| Surface | Before: Objective2, earlier AccessReviewed=true, newest report unread | Corrected unread state | After newest ACK |
|---|---|---|---|
| Walking HUD | Already correctly Review the year | Same behaviour, now uses shared `ReviewFirst` selector | Three-zone checklist / Crop Tree guidance |
| Objectives What to do | Choose one Crop Tree in each zone | Read and acknowledge the Annual Review: open Work Plan [Tab] | Original short Crop Tree instruction |
| Objectives Full instructions | Could still direct Crop Tree selection under More detail | Same review-only text as What to do, so redundant Full instructions omitted | Original full lesson explanation restored |
| Objectives heading/banner | Numbered Choose promising trees heading, no newest-unread banner | Numbered lesson title retained; explicit **First: Review the year** banner | Banner removed; numbered lesson retained |
| Objectives progress / summary | Marking progress / lesson title | Review progress / Review the year summary; no completed selection boxes | Original lesson progress / summary |
| Map reason / target | Crop Tree reason beside review-navigation target | Review instruction / `map-work-plan` | Crop Tree reason / eligible waypoint or forest navigation |
| Work Plan reason / target | Crop Tree reason beside read-results target | Review instruction / `workplan-read-results` | Current lesson reason; no new approval/advance pulse |
| Annual Review reason / target | Crop Tree reason beside acknowledge target | Review instruction / `annual-review-acknowledge` | Current lesson reason; stale acknowledgement target removed |

Source callsites on the exact corrected commit:

- [`ScenarioOneRackBayText.cs:60`](https://github.com/jermur788/CCF/blob/85568c4f60b09279a2e1692bd0e7f8593e1b9a0d/Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs#L60): pure `ReviewFirst`, review progress, and existing HudWords now reusing that rule.
- [`LearningObjectivesView.cs:33`](https://github.com/jermur788/CCF/blob/85568c4f60b09279a2e1692bd0e7f8593e1b9a0d/Assets/ForestPrototype/UI/LearningObjectivesView.cs#L33): world/preview guard and both actual UI review predicates; full/short instruction, progress and summary consumers.
- [`LearningObjectivesView.cs:166`](https://github.com/jermur788/CCF/blob/85568c4f60b09279a2e1692bd0e7f8593e1b9a0d/Assets/ForestPrototype/UI/LearningObjectivesView.cs#L166): real card banner, What to do, Full instructions and progress references. Practice, extension checklist and session-inspection warning wait until after review so they cannot compete with the immediate instruction. The lesson concept and diagram remain explanatory context.
- [`ScenarioOneUiRoot.cs:448`](https://github.com/jermur788/CCF/blob/85568c4f60b09279a2e1692bd0e7f8593e1b9a0d/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs#L448): all three later-window reasons still read `CurrentShortInstruction`; review target now uses `learning.ReviewFirst` too. Objective1's existing rack-navigation branch is unchanged.

The new override is world guarded. Legacy40/model0/Reference preview retain their instruction selection and wording; they gain no newest-unread override or V2 review banner. The previously unguarded #365 banner is now V2-only, consistent with the original legacy card. CurrentTitle/CurrentTopicIndex/TeachingFacts/sequence numbering remain unchanged. After ACK, even if the latest report is read, a still-missing lesson-specific ACK continues to require review.

## Scope and protection

Exactly **3 production UI files** above and **6 focused verifier files**:

- Existing `hud_contract.py`, `run_checks.py`, `mutations.py`, `SOURCE-ALLOWLIST.md`.
- New `ConsumerPaths.cs`, `consumer_paths.py` under `Tools/Verification/PlayerFeedbackOvernight/`.

The source scope inventory has **5,782 tracked files: 5,775 identical, 7 changed**, plus the two new verifier files; no outside-allowlist change. The later report adds only this document. No Assets file added/deleted, no `.meta`/GUID, scene, prefab, ScriptableObject, Package, ProjectSettings, generated Unity folder or serialized asset changed.

`LearningObjectivesView.Sequence.cs`, teaching/scoring/crop selection, racks, geometry, ecology, economy, annual ACK actions, save code/schema and frozen Reference sources are byte-identical to base. No hash/anchor rebaseline. No new pulse on Approve, Confirm, Fell, Advance or prescription commitments.

Primary's unrelated `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications were preserved. The human #361 slot, warm U3, original dirty Workbench, 27 licensed pack, user save/preferences and frozen Reference were protected by **no task access or writes to those working sets**; no new external hash/runtime claim is made. No Unity process/lock access, scene/scenery or save modification, main merge, Player/Windows build, integration or acceptance.

## Verification and negative controls

Final gates ran on **clean committed `85568c4f`**, clean before and after, with all outputs in:

`/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-367-Repair-20261110/`

| Gate | Result |
|---|---|
| Unity-managed-reference C# compilation | **189 sources / 116 refs / exit 0 / 0 errors** |
| Original focused assertions | **160 PASS**, unchanged `Checks.cs`: 67 #363 + 93 #365 |
| New exact-source consumer assertions | **219 PASS**; combined total **379** |
| Original seven typed mutants | All seven killed; original expectations preserved |
| New typed newest-unread omission mutant | Compiles; consumer execution fails on the required named assertion |
| Source negatives | Original HudModel bypass refused; three new callsite bypasses refused by named contracts |
| Source contract / whitespace | PASS |
| Unity/render/native input/save-load | **NOT RUN** |

`Checks.cs` is byte-identical to base, SHA-256 `6fd053285b8c23b35d3d497586c439621018979cfbdd9bf944a4f7d6f78b20c0`.

The new runner compiles exact production property bodies, actual Objectives card emission statements, and the later-window reason/target branch plus actual Label output assignment. Plain-data inputs and simple label sinks replace Unity objects. It tests the current short/full instructions, banner/progress, detail omission and Map/Work Plan/Annual Review reason/target references, rather than only HudWords. Cases cover earlier lesson ACK + latest unread, after newest ACK, missing lesson ACK with latest read, Objectives3–5 unread/after ACK progress, no player, non-V2/preview guards, extension suppression, Objective1 card, final lesson and selector truth table. The existing completed-HUD check remains intact. The unchanged Objective1 navigation branch has an explicit source hash contract.

**Adverse case reproduced from immutable Git base:** `before-consumers-git.txt` exits **134** at `R367_LATEST_UNREAD_EARLIER_ACK_CURRENT_SHORT_REVIEW_FIRST`. Original UI slices come from `git show 94486d9b:<path>`. The baseline slices themselves do not call the new selector; the current plain-data assembly supplies the unchanged lesson helpers. The corrected consumer suite exits **0**.

**Named negative:** `review-first-omits-newest-unread` changes only `rackV2 && (lessonReviewPending || annualReviewUnread)` to `rackV2 && lessonReviewPending`. The typed mutant compiles, then exact source consumers fail at **`R367_LATEST_UNREAD_EARLIER_ACK_CURRENT_SHORT_REVIEW_FIRST`**. No generic LINQ failure is accepted for this new regression.

Three new source negatives also require specific failures:

- Dropping actual `ui.NeedsAnnualReview` at the view callsite → `R367_REVIEW_FIRST_BOTH_PREDICATES`.
- Reverting the short accessor to only lesson ACK → `R367_SHORT_SHARED_PRIORITY`.
- Replacing card What-to-do's short consumer with Crop words → `R367_CARD_SHORT_CONSUMER`.

The original seven mutants remain unchanged in purpose. Only the existing review mutant's **site text** follows the new helper call; it still sets the HUD review bool false and requires the original named assertion. The inherited no-feeder mutant still uses its historical generic Single/LINQ failure; it is not evidence for the new R366-01 negative. It was retained to avoid rewriting historical checks.

Evidence integrity correction: an initial inventory command exceeded its yield interval, so UI edits overlapped the slow capture. That capture and an earlier misleading baseline probe are retained under `capture-order-*`, `before-consumers.txt` and `capture-correction.txt` and are **not relied on** as pre-repair evidence. The three baseline UI files were replaced from immutable Git, the named adverse case rerun, then all final gates rerun against clean committed source. Early probe-development failures concerned the fake layout type/flag and are retained in the receipt; no production behaviour was changed to satisfy them.

Reproduction commands are in the source commit's [`SOURCE-ALLOWLIST.md`](https://github.com/jermur788/CCF/blob/85568c4f60b09279a2e1692bd0e7f8593e1b9a0d/Tools/Verification/PlayerFeedbackOvernight/SOURCE-ALLOWLIST.md). `committed-results.json` records command arrays and actual exits; `committed-compile.txt`, `committed-checks.txt`, `committed-mutations.txt`, `scope-check.json`, `source.diff` and `SHA256SUMS.txt` preserve final evidence.

No historical suite was weakened, rerun broadly or relabelled: strict Workbench exit1 FAIL, StandMapParity×2 FAIL, save03b FAIL, UxPedagogy 38 FAIL, #360 synthetic 171 PASS / 2 FAIL and 26 all-black captures remain as recorded. The original #366 adverse verdict is retained as history; this report supplies a new candidate for review.

## Remaining gates and Manager return

**Ready for review: Yes.** A genuinely independent reviewer, separate from this implementing session, must check the delta and measurement integrity. Because this repair was authored by OpenAI, Manager should assign the next different-provider review accordingly rather than treating another OpenAI author check as independent cross-provider acceptance. No reviewer was launched by this task and no gate is self-approved.

Unity is separately gated. Only after independent review and explicit #361 human Editor release, use the true global lock and all admission gates from the binding packet: warm source-exact lane, licence, >=35 GiB plus monitoring, verified disposable save/preferences, licensed pack/frozen Reference/old Workbench protection. Then perform the single bounded Tier2 qualification. No broader historical campaign or Player build is implied.

Exact expected Unity checks for that later authorised operator:

1. In a genuine Rack V2 Objective2 state whose first rack year is acknowledged, advance to a later unread Annual Review through normal play. Open Objectives [O], Map [M], Work Plan [Tab] and Annual Review. Expect the explicit Review the year banner, review-only immediate/full instructions, review progress, matching review navigation/ACK guidance, and no immediate Crop Tree action or completed three-zone boxes.
2. Acknowledge the newest report through its existing action, reopen those surfaces and walk the forest. Expect the normal current lesson, Objective2 Crop Tree instructions and three-zone HUD checklist to return. A still-missing lesson ACK must continue to gate.
3. Check genuine Objectives3–5 and original Legacy40/Reference-preview paths at 1280×720 and 1920×1080. Expect real stage progress after ACK, unchanged old-world words, and no unsafe commitment pulse. Do not inject completed stages as a substitute for real played state.

Known unresolved risk: real UI visibility, timing, folding, card height/layout and input/navigation have not been executed or rendered. The source-slice sinks do not execute TeachingFacts derivation, Unity events or root scope admission. Historical late-stage/runtime/visual risks remain outstanding; no save/load or performance claim is made.

**DASHBOARD SYNC: NOT AVAILABLE / downstream unverified. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO verified receipt.** No suitable publisher connector is exposed; Manager retains sole active-tasks authority. The exact GitHub handoff, closed claim and #200 return are the worker's authoritative task record. Manager must update live coordination and run/allow supported dashboard ingestion, then verify publication/readback. No generated dashboard or Manager-owned coordination file was edited.

Recommended next step: Manager #200 triages this exact source + report, assigns independent delta review and retains all Unity/user acceptance gates. **[HANDED-OFF] closes execution only.**
