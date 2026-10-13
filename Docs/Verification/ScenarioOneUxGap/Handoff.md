# Scenario One UX gap patch after the B07 review

Worker role: Scenario One UX / teaching implementation (cloud, code-only).
Branch: `claude/dazzling-pascal-s0jzlz`. Base implementation: `claude/stoic-mccarthy-gtzaub` @ `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`
(the branch was fast-forwarded from `c1564b8`, the commit the B07 review examined).
Context commit: the task packet named none, so no context commit was locked; `Docs/Project/*` was read as it stood at `b4894cc`.

**This is not Unity-verified.** No Unity Editor, no rendering and no input were available. Everything below that says "run" was
run on plain .NET or by the compiler only; everything marked **UNITY GATE** still has to be run.

## Provenance of the findings

The B07 review document is not in the repository. The finding IDs (B-1…B-3, H-1…H-10) and their one-line labels come from the task
packet, plus the packet's own numbers (about 770 words over five modal intros; small text at 1280×720 and the 1024×768 default).
The "prior status" column is therefore **inferred from those labels and from reading `c1564b8`…`b4894cc`**, not from the review text.
If the review defines any finding more narrowly or more broadly, re-check that row.

## Gap matrix (Blocker / High)

| ID | Label | State at b4894cc | Action |
|---|---|---|---|
| B-1 | completion / loop presentation | ALREADY ADDRESSED: completion card, management history, year drill-down, HUD intervention count, `NextStepHint` | Preserved. Only the stale "Open the Century Review in the Work Plan" pointer fixed. |
| B-2 | later-cycle teaching | PARTLY ADDRESSED: `NextStepHint`, first-intervention prompt, stale-order card exist; the lesson cue appended to them was still the map curriculum | Cue reordered (below). |
| B-3 | completion moment | ALREADY ADDRESSED: "SCENARIO ONE COMPLETE · YEAR N" summary opens the review of that year | Preserved, unchanged. |
| H-1 | role / goal | STILL PRESENT: first screen was a five-paragraph HUD explanation; F10 card said "private first-cycle test / manage one cycle" | Implemented: role briefing; stale wording replaced. |
| H-2 | silent wasted year | STILL PRESENT: Advance allowed with only pending orders | Implemented: inline warning + three choices. |
| H-3 | stacked modal intros | STILL PRESENT: five auto-opening modals; Annual Review → help → acknowledgement | Implemented. |
| H-4 | teaching order | STILL PRESENT: lesson 1 was an 11-step map curriculum and drove the persistent cue | Implemented: first cycle first. |
| H-5 | broadleaf rationale | PARTLY ADDRESSED: Oak/Beech market and ladders now explain *how* broadleaves are valued; nothing explains *why* the objectives ask for them | **Not implemented.** No accepted rationale wording exists in the Game Brief / Decision Log, and the packet forbids inventing ecology. Needs Manager wording. |
| H-6 | history / review | ALREADY ADDRESSED (history, drill-down) | Preserved. |
| H-7 | cause attribution | PARTLY ADDRESSED: `NotCausalNote` and non-felling death counts exist | Remainder (persisted juvenile death cause) is on the packet's do-not-implement list. |
| H-8 | legibility | STILL PRESENT (predicted from static review) | **No change.** See "Legibility". |
| H-9 | autosave | STILL PRESENT | Excluded by the packet. |
| H-10 | teaching-state scope | STILL PRESENT: help/lesson progress is per device (PlayerPrefs), not per forest | No persistence redesign. Reset added; Learning screen now says progress is per device. |

## What changed

Commits on top of `b4894cc`, oldest first:

1. `9a865db` refactor: split help text and lesson list into pure partial files. `MenuHelpView` and `LearningObjectivesView` each become
   two halves of one partial class so plain .NET can compile the wording. No public member renamed; every moved line is identical.
2. `c89cebc` feat: role briefing + stale first-cycle wording.
3. `7203fee` fix: warn before advancing with unapproved work.
4. `9b01a7b` ux: simplify first-use help and put the first cycle first.
5. `58e7a67` ux: refresh controls, terminology and stale scenario guidance.
6. `51e240f` ux: add tutorial and help progress reset.

### Role / goal
First use of the Walking HUD now shows "Your role · Scenario One" (101 words): small Sitka spruce plantation; aim is to begin converting it
toward continuous-cover forestry; decisions play out over years; choose trees to keep/remove and commission work in the Work Plan; no single
perfect prescription; revisit as the forest changes. No DBH, competition, regeneration or browsing detail. "More detail" and F1 show the full
help, which now opens with the same role paragraph. F10 card: "CCF · Scenario One", repeated-management wording.

### Wasted-year guard
Pressing Advance with pending (valid, approvable) orders and **no approved orders** shows an inline card in the Work Plan: "You have planned
work that has not been approved." with **Back to the plan**, **Approve pending work** (the player's own click) and **Advance anyway**.
Nothing is approved automatically and waiting stays possible. The rule is `AdvanceGuard` (pure). The guard is on the Work Plan button only;
`ScenarioOneUiRoot.AdvanceFromWorkPlan` and `ScenarioOneManager.AdvanceYear` are unchanged, so every harness that calls them behaves as before.
Deliberate boundary: if *some* work is approved the year does something and there is no question; unapproved orders then wait as before.

### Teaching order
Lessons are now: 1 Tree inspection and Crop Trees, 2 Felling, 3 Work Plan and Annual Review, 4 Stand Map, then Pruning, Deadwood, Planting,
Clearance. The HUD "Next lesson" cue is the first step left in that order, action steps before reading acknowledgements, so a new player sees
"Inspect a living tree", then Crop Tree, fell mark, Work Plan, approve, result, review, and only then the map. All 42 step ids are unchanged
(saved progress is preserved; pinned by the offline check). The cue is cached by completed-step count (it used to walk the lesson list every
frame). No year gate, no new state machine.

### Help / modals
Walking HUD (101 words), Tree Inspection (65), Stand Map (41) and Work Plan (50) open short first-use screens: 257 words, down from the
packet's ~770 over five. The Annual Review opens **no** help screen; its reading guidance is inside the review beside the acknowledgement
button. The full text (826 words) is kept and reachable by "More detail", F1 and the Help buttons.

### Controls / terminology
Startup Controls / Help card and the full Walking HUD help share `ControlsReference`: adds left-click inspection (already active, previously
undocumented), X fell mark, C Crop Tree mark with competitor assessment, 1–5 competitors (1–2 planting stock), G planting mode, U clearance.
"CI" is spelled out ("competition index") on the card and defined in the inspection help; residual stand and basal area are defined on the Work
Plan's "WHAT YOU ARE LEAVING" card and in its help; browsing is defined as deer eating young shoots; the HUD line reads "Deer browsing".

### Stale copy
- "Open the Century Review in the Work Plan" → "The Century Review is in the Annual Review."
- Unreachable year-one `TutorialHint` chain removed (production behaviour identical: the UI root is added in `Awake` whenever playing).
- "private first-cycle test" / "Manage one cycle" replaced.
- The harvest-cost lesson no longer claims "no broadleaf timber market is configured" for every forest. It appends a sentence from
  `BroadleafMarketActive`: Growth Model 2 → oak and beech have timber prices; otherwise only Sitka earns timber income. Growth Model 1 is still
  described truthfully as having no broadleaf market. The inspection card and Work Plan harvest card were already state-driven and are unchanged.

### Tutorial reset
F10 → "Reset tutorial/help progress…" → confirm. Clears the `CCF.MenuHelp.v1.*` and `CCF.Learning.v1.*` preferences only and makes the open Learning view follow.
It does not touch the forest, `forest-save.json`, the saved annual-review acknowledgement or objectives. It is on the F10 card, **not** on the
startup Controls / Help card, because `StartupScreenVerification` asserts that card "offers only Back".

Manual fallback (no code): delete PlayerPrefs keys with prefixes `CCF.MenuHelp.v1.` and `CCF.Learning.v1.` for company `DefaultCompany`, product `CCF`
(Windows: `HKEY_CURRENT_USER\Software\DefaultCompany\CCF`; Linux: `~/.config/unity3d/DefaultCompany/CCF/prefs`). Do not delete `forest-save.json`
unless a fresh forest is also wanted.

### Legibility (no code change, on purpose)
Arithmetic only (Unity's logarithmic match formula; **not rendered**). In-game panel: reference 1600×900, match 0.5. Startup/session menu panels:
reference 1600×900, match 0 (width).

| Window | In-game scale | Startup/menu scale | 13 px chip | 15 px body | 22 px startup button (menu) |
|---|---|---|---|---|---|
| 1024×768 (Player default window) | 0.739 | 0.640 | 9.6 px | 11.1 px | 14.1 px |
| 1280×720 | 0.800 | 0.800 | 10.4 px | 12.0 px | 17.6 px |
| 1920×1080 | 1.200 | 1.200 | 15.6 px | 18.0 px | 26.4 px |

The prediction in the packet is arithmetically right for 1024×768. I found no change that is defensible without seeing it: a larger reference or a
different match changes every layout, and +1 px on buttons/chips risks the header rows (the Annual Review header already relies on its title
wrapping). Recommendation, for a native test first: decide whether 1024×768 is a supported window, and if so test a lower reference resolution or
match-height for narrow aspect ratios. New elements added here use existing classes (≥ 15 px).

## Preserved

Ground-report "Why:", non-prescriptive inspection, Crop Tree competitor panel and release forecast, reversible X/C marking, pending → approved →
resolved orders, MONEY, WHAT YOU ARE LEAVING, minimum-visit explanation, Annual Review WORK DONE / MONEY / FOREST, Stand Map role, "opportunity,
not guaranteed regeneration" wording, no overall score, the single first-review acknowledgement, management history, year drill-down, first-
intervention prompt, completion summary, stale-order card and HUD intervention count. Save stays v20; the eight objectives and Reference
Future are untouched.

## Verified here (code-only)

- `Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod`: no new errors (7 = baseline) at every commit. `--mode tools`: no new errors (43 = baseline).
  This is a compile against a Unity 2021.3 reference approximation, not Unity's compiler.
- `Tools/Verification/ScenarioOneUx/run_ux_check.py`: `SCENARIO_ONE_UX_PASS checks=176` (plain .NET; wording, lesson order and cue, saved
  step ids, first-use word budget, controls coverage, jargon placement, model-aware market sentence, advance-guard rule, and a source scan that
  seven obsolete phrases are gone — all seven are present at `b4894cc`).
- `git diff --check` clean; changed-file audit: no ecology, density, species, growth, economy, timber, save-schema, objective, Reference Future,
  scene, prefab, asset, ProjectSettings or Packages file touched; no `Find*ByType` call added.

Environment note: `dotnet` was not installed. `builds.dotnet.microsoft.com` is denied by the session's egress policy (403), so the SDK came from the
distribution's apt package instead. Nothing was done to get around the denied host.

## UNITY GATES STILL REQUIRED

The following harnesses assert behaviour this patch changes. `MenuTutorialVerification` (Annual Review block, lines ~255–266) and
`ScenarioOneTeachingCopyVerification` (topic number) were edited to match; **neither edit has been run**. Run with the existing stage → run →
remove workflow (`Tools/Verification/run_s1a_gates.py` for the first two; interactive Editor for the first):

- `MenuTutorialVerification` (interactive), `ScenarioOneTeachingCopyVerification`, `StartupScreenVerification`, `S1B/SessionMenuVerification`,
  `ScenarioOneCompletion/P3/WorkPlanResidualVerification`, `ScenarioOneCompletion/P2/CropTreeCompetitorVerification`.

Manual checks, at **1280×720 and 1920×1080** in the Editor Game view, then on **native Windows** (with the Player's 1024×768 default window too):

1. F10 → Reset tutorial/help progress… → Cancel: nothing changes. Again → Confirm: a note says your forest and save were not changed; `forest-save.json` is untouched.
   Close the menu: "Your role · Scenario One" opens (about 100 words). "More detail" shows the full Walking HUD help ending with the keys line. Continue closes it; it does not reopen. F1 reopens the full help.
2. HUD status shows "Next lesson: Inspect a living tree · [O] Objectives" (not the map). Inspect a tree with E, then with left-click (both work; the first click after Esc only recaptures the mouse).
   The short Tree Inspection screen opens once. The card reads "competition index", not "CI". Press C, then X on a neighbour; the cue advances in the order inspect → Crop Tree → fell mark → Work Plan → approve → result → review.
3. Wasted-year guard. Mark a tree (X), press Tab, press **Advance one year** with nothing approved: the warning card appears, the year does not change.
   *Back to the plan* hides it. *Approve pending work* approves and the card disappears; Advance then proceeds with no card. Repeat to *Advance anyway*: the year passes, "No work was resolved this year", and the order is still pending.
   Press Tab to close and reopen the Work Plan while the card is showing: the card is gone. With any order approved, Advance shows no card.
4. First Annual Review: opens with **no** help screen over it; the footer shows the reading prompt (wraps to 2–3 lines at 1280×720) above "I've read the annual results"; Help [F1] opens the full text and Esc closes only help; the acknowledgement still gates the next year.
5. Layout at both sizes: first-use screens with the new "More detail" button, the warning card inside the Work Plan, the Annual Review footer, the startup Controls / Help card (16 rows scroll, Back stays visible), the O screen (topic 1 expanded), the Work Plan "WHAT YOU ARE LEAVING" subtitle.
6. O screen → Felling → "Read the harvest costs": new-game forest ends "only Sitka spruce earns timber income…"; a Growth Model 2 forest (three-species Unity gates) ends "oak and beech stems have timber prices…".
7. Year-100 pointer (optional): the Work Plan feedback reads "The Century Review is in the Annual Review." (also covered by the historical continuation harness).
8. Native Windows technical smoke of text size and input remains the open Stage-A gate; nothing here changes that.

## Not implemented / needs a decision

- H-5 broadleaf rationale wording (Manager/research).
- Partial approval: when some work is approved, other pending orders still wait silently (a lighter non-blocking note is possible; not requested).
- Legibility change (needs rendered/native evidence first); whether 1024×768 is a supported window.
- Autosave; per-tree growth history; persisted juvenile death cause; Advance N Years; success objectives or a two-intervention requirement; overall score;
  UI scale and mouse sensitivity settings; failure-system redesign (all on the packet's do-not-implement list).
- `Docs/Verification/S1B/TesterREADME.md` still describes the pilot as a "first-cycle" test; it was not edited (outside this packet).

## Merge note for the density worker

Touches `ScenarioOneManager.cs` in two places only: the `TutorialHint` property (dead branch removed) and one `CanManage` feedback string. No other
shared simulation file. New files: `AdvanceGuard.cs`, `ControlsReference.cs`, `MenuHelpView.Text.cs`, `LearningObjectivesView.Curriculum.cs` (+ `.meta`),
and `Tools/Verification/ScenarioOneUx/`.
