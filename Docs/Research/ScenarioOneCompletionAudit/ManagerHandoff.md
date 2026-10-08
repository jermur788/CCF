# Manager handoff — Scenario One completion audit

**Type:** CLOUD HANDOFF — NOT UNITY-VERIFIED. Docs-only static audit. No production, canonical, save, scene or package change; nothing merged. All Unity results cited are from the records named, not re-run here.

**Rechecks if main moves:** main was `869ee92` at start and at the end of the task. If main changes before this is acted on, recheck: (1) whether P3 or P1 content landed (changes the S1-A scope); (2) whether the MenuTutorial failure was classified; (3) completion anchors listed in S1-A; (4) any storm activation change (would make storms relevant to the beginner test).

---

SCENARIO ONE COMPLETION AUDIT — MANAGER HANDOFF

INSPECTED MAIN:
- `869ee92a983a1af5fc470392eccc7557fbe45def` (re-fetched at the end: unchanged). Save 19; new games RNG 1 / regeneration 2 / growth 1 / storm 0. P2 Crop Tree competitor reasoning is integrated (automated gates PASS; human smoke not performed). P1 pedagogy copy is not on main.

P3 STATE CONSIDERED:
- `task/work-plan-residual-stand-poststorm` @ `0ab73994153d55d08adbd65f928926e6e9165f7c`: CANDIDATE, one commit on main, Unity-verified on the branch (offline PASS, batch ×2 hash `F7C2FC966816BBAD`, rendered PASS at three sizes; regression 23/24 with one unclassified MenuTutorial failure). Treated as the intended state, not as main. Resolves residual stand, money and the cash dead-end warning. Leaves: pre-purchase warning, recovery route, terminology, readability, everything in review/history/progression/completion.

FEATURE-COMPLETE BLOCKERS:
- No second-look prompt and no second-cycle requirement; completion is met by one felled tree plus five conditions the forest meets by itself (`ScenarioOneObjectives.cs:54–96`).
- Hard "Failed" outcomes (cash exactly €0; Year 100) contradict a learning scenario (`ScenarioOneManager.cs:2297–2303`).
- Annual Review shows only the latest year; no places to inspect, no diary, no cell history, self-thinning never named.
- Forester review not done; beginner tests not done.
- Decisions not recorded: E1, N1/N2/L3, O1, O2, sapling price authority, J3.
- Canonical docs do not record P2.

BEGINNER-TEST BLOCKERS:
- P3 not integrated; MenuTutorial failure unclassified; P2/P3 human smokes not performed.
- CCF never defined on main; P1 copy not integrated (and its clearance sentence is false under Model 2, so it must be ported with changes).
- Raw species ids in the objectives a tester reads first; objectives visible only as a count; the 25-year horizon is not explained.
- Nursery purchases can still enter the cash dead end without a prior warning.
- No standalone build ever produced; no visible save/load, quit or new-forest action; help progress is per device; Player performance never measured; Asset Store licence for a private build unchecked.

FORESTER-REVIEW BLOCKERS:
- Everything above, plus S1-C (history) and S1-D (two cycles, no hard fail): a forester must see repeated management, not one thinning.
- Mixed-species SDI mortality confirmed in code (`ForestEcologyController.cs:1010–1056` applies the Sitka density line to every living tree): diagnose; fix or disclose.
- A one-page reviewer sheet of what is modelled, calibrated [C] or absent.

P1 PEDAGOGY:
- CANDIDATE `1a36ea9` (save v16), never integrated. Valuable and still needed. PORT WITH CHANGES in S1-A: re-base to v19, apply the P2 merge notes, replace the Model-1 clearance sentence with the model-aware explanation everywhere, and land its harness fixes so the regression runner stops overlaying sources from an unmerged commit.

ANNUAL REVIEW:
- Main: WORK DONE / MONEY / FOREST columns + storm card + TRENDS + objectives. MISSING: REGENERATION and DISTURBANCE sections, self-thinning deaths, PLACES TO INSPECT. All derivable from saved data; no save change. Packet S1-C.

FOREST DIARY / HISTORY:
- Stored: yearly snapshots and reports; every management event with year, cell, position, tree, species, costs; mortality cause/year; deadwood; planted records; storm events. Visible: latest review, 12-year cash/canopy bars, objectives. Not stored: per-cell ecological history, yearly standing volume, Crop Tree CI history. Minimum useful implementation: read-only whole-forest timeline + Stand Map cell history + PLACES TO INSPECT, no save change (S1-C).

OBJECTIVES / PROGRESSION:
- HUD shows "Forest objectives N of 8" only; `[O]` opens the learning checklist; learning progress is per device. Fix visibility in S1-A (no logic change). Defer the S1–S12 stage engine and per-forest progress until beginner-test evidence. No universal score exists today; keep it that way (record E1).

SECOND INTERVENTION:
- Implemented: nothing. Proposed: trigger + model D + per-forest progress + "no work needed" event. Smallest closure (S1-D): a teaching-only trigger computed from current state and saved events, and completion requiring a second resolved cycle of any work type ≥ 5 years after the first release, plus cover/capital/renewal guardrails. No save change; completion anchor changes with authorisation.

ECONOMY / CASH RECOVERY:
- Dead end is real (cash < €2,500 minimum before a harvest with timber as the only income). P3 warns before approval and explains after the fact; S1-A adds the pre-purchase warning; S1-B adds "Start a new forest". With those, Scenario One has no hidden trap. Recovery route (standing sale / net settlement) is a product decision that can wait for forester feedback. Lowest cash on the current save-19 stack is not recorded; re-verify in S1-D (last recorded €6,139.75 under growth 1 / regeneration 1). S1-D's "any work type" second cycle keeps completion reachable after a first thinning.

SAPLING PRICE:
- Player pays €4.50 / €5.50 (definition [D]); price book holds €0.95 / €1.00 (wholesale [E]). Inert today (no cash path reads the book for these items), latent hazard. Recommend: record the definition price as the authority and let the adapter override the book (no cash change), economy-owner approval, re-gated with S1-D. Not a test blocker.

STORMS:
- Core accepted and integrated dormant (D-050); new Scenario One games are storm 0. Storm UI already on main (bands with "storms off", forecast wording, review card, salvage, waypoint; Storm UI gate PASS). Nothing storm-related blocks Scenario One external testing. Old W5 is superseded for Scenario One; storm teaching belongs to a later storm scenario. Observe in the beginner test whether "storms off" labels confuse.

MANUAL UI ACCEPTANCE:
- P2 human smoke: pending. P3 human review: pending. MenuTutorial Help-Escape: unclassified. P1 copy: never rendered on v19. Model 2 bramble/bracken recognition and human clearance decisions: open. Worker-worktree smoke gates: open. Standalone Player run: never done.

OBSOLETE / SUPERSEDED PACKETS:
- Fully superseded: pedagogy P2 and P3; readiness P2; readiness W5 (for Scenario One); `PostScenarioRoadmap.md` Stage-2-first order (if forestry-first is confirmed).
- Partly superseded: pedagogy P0 (→ S1-A), P5 (→ S1-C), P4 practice mode (→ defer), P7 objective redesign (→ S1-D), wind/competition labels; readiness P3 (terminology/readability → S1-A), P5 (copy shipped; visibility → S1-A; stage engine deferred).
- Port with changes: pedagogy P1 and readiness P1-INT (→ S1-A), readiness P4 (→ S1-C), readiness P6 (→ S1-D).
- Defer: pedagogy P6, P8, P9; readiness P7; SCN-0; expansion waves. Port as written: P8 playtest-fix template only.

RECOMMENDED IMPLEMENTATION PACKETS:
1. S1-A Teaching, terminology and objectives — P1 port with Model-2-correct clearance copy, tracked harness fixes, species display names, visible objectives and 25-year horizon, pre-purchase cash warning, 13 px readability. UI/copy only; anchors unchanged.
2. S1-B Private test build — reproducible standalone build with SHA stamp, session menu (save, load, new forest, reset help, quit), Player profiling Years 0–25, licence check. No gameplay change.
3. S1-C Annual Review v2, Forest Diary and place history — six sections incl. self-thinning and PLACES TO INSPECT with waypoints, whole-forest timeline, Stand Map cell history. Read-only; no save change.
4. S1-D Second look, second cycle and completion — teaching-only trigger, completion over two separated cycles with guardrails and a surviving broadleaf, no hard fail, descriptive completion view. No save change; authorised new completion anchor.

WORK TO DEFER UNTIL TESTING:
- Stage engine S1–S12, plan comparison, assistance changes, approve-all summary, per-cause planted-loss line (unless S1-C has room), storm-off label wording, asset packets 01/07 and bramble/bracken recognition fixes, competition label thresholds; recovery route, sample plots, minimum-charge level and holding size until forester feedback.

WORK TO DEFER UNTIL SCENARIO TWO:
- Scenario package v0 (scenario id in save, start snapshot, inherited-history boundary, scenario select), per-scenario completion/assistance profiles, per-forest progress record, "no work needed" event, plan comparison everywhere, training stand, storm branch scenario and storm activation policy. Multi-species foundation and site map wait for Scenario Three (except the SDI diagnosis, which runs now).

EARLIEST EXTERNAL TEST POINT:
- After P3 is integrated and S1-A and S1-B are integrated, with the MenuTutorial failure classified, P2/P3 human smokes done, a green regression from tracked sources, and one internal pilot. Scope: a private beginner test of the first cycle (Years 0 to about 10). S1-C and S1-D are built next, informed by its findings.

BIGGEST CURRENT PRODUCT RISK:
- Scenario One can be "completed" without ever practising continuous-cover forestry (one felled tree, no second look), while effort keeps going into explanation UI before any real player has tried the game. Every thinning on 0.16 ha also loses money, which may teach "never thin"; only real testers can say whether the P3 explanation is enough.

BIGGEST CURRENT TECHNICAL RISK:
- No standalone Player build has ever been produced or profiled, and the visuals depend on a gitignored Asset Store pack. Second: verification depends on harness sources overlaid from an unmerged commit (`1a36ea9`), and one interactive gate (MenuTutorial) is red without a diagnosis. Separate ecology-correctness risk: Sitka density mortality applied to broadleaves.

STATUS:
READY FOR MANAGER REVIEW
