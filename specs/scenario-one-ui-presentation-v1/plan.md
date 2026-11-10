# Scenario One UI Presentation V1 — Implementation Plan

**Status: APPROVED plan / execution NOT STARTED.** One coherent implementation across HUD, full-screen menu chrome, Work Plan and Annual Review. The starting repo/UI code must be rechecked against the current actual player-test feature branch and any later fixes before edits.

## Phase 0 — Preflight and ownership

1. Read live `Docs/Project/coordination/active-tasks.md`, `#361` personal test status, relevant open claims and current feature source. Check exact source branch/head, actual local worktree, dirty/ignored valuable files; never claim a lock from spec text.
2. Sol owns a **new isolated local UI source worktree** and one bounded UI file set after claim becomes RUNNING. Do **not** write directly in warm U3, Workbench or user playtest clone.
3. If an actual UI writer owns `Assets/ForestPrototype/UI/`, stop: arrange handoff/ownership before mutation. A docs-only spec branch is not the game source branch and cannot be substituted for it.
4. Make one initial state inventory/screenshots based on **current** source, not Fable's older CCF-358 capture. Confirm no missing UX already implemented in the 10 November source repairs.

## Phase 1 — Design system and presentation implementation (Sol)

- Build or adapt reusable small header/footer/menu navigation pieces without new global view state. Keep live root permissions and current next-action priority.
- Implement Year/Cash top-right + compact objective cue + truthful stage-aware keycaps; remove duplicated browse/global status.
- Merge inspection cards, preserve all detailed `[V]` information, source-backed plan warnings, selected neighbours/rank logic and MFM distinction.
- Reorganize Work Plan summary/quote/footer and annual Review three sections/recording/trends; retain underlying handlers and historical semantics.
- Optional in-world share overlays only after proof; do not expand scope if testing shows clutter.
- Keep diff mainly under `Assets/ForestPrototype/UI/` and presentation USS. If touching `ForestTreeMarkingManager.cs` would be necessary, request Manager approval of minimal typed projection first and avoid calculation mutation.

## Phase 2 — Cheap functional checks (Sol)

- Tier 1 compilation of touched source and focused offline/UI test coverage. Include negative source-to-handler paths and a meaningful mutated guard test for review-first, Rack-first X/C hidden, proposal vs Fell, and current-vs-history money/no-snapshot handling as appropriate. Do not weaken old assertions or run full unrelated test suites just for visuals.
- Get a genuine **nonblack** 720p and 1080p rendered candidate from the current source where legally possible. Check 2–3 key states per screen before expensive full coverage. If Unity unavailable due human Editor ownership or machine safety, report source-only and mark render NOT VERIFIED, not PASS.

## Phase 3 — One focused Tier-2 (Sol qualified operator)

One serialized machine lease and one screen matrix: ordinary walk; Rack-first; Crop Tree with many neighbours; Fell and MFM proposal; `[V]` details and bottom keys; open Work Plan with low-value harvest minimum and pending approvals; safe advance confirmation; Year1 unread annual review and acknowledgement; historical Year1/year2 browsing; missing data; 720p/1080p colour/grayscale; Legacy40/frozen Reference source/targeted runtime check where feasible. Verify actual current candidate, guards and no source drift. Do not rerun broad gameplay/forest calibration or release suite by default.

If user personal Editor session is still active, **no Unity ownership or Player build**. Source handoff can still be delivered.

## Phase 4 — Independent Fable review and Manager verdict

After Sol files and Markdown result are published, Fable independently inspects the actual source diff, relevant tests and real PNG artifacts (if available), and compares them to **each of the four committed SVGs**, not just to its earlier proposal. Fable is **review-only**, cannot fix UI in same lane or acquire Unity without new Manager lock. Its verdict PASS/NEEDS CHANGES/REJECTED with blockers, majors and minors goes to a separate GitHub Markdown handoff. Manager issues APPROVED/NEEDS CHANGES/REJECTED and decides integration independently.

## Required evidence and paths

- Sol: `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-UI-PRESENTATION-V1-IMPLEMENTATION-AND-QUALIFICATION-RESULT.md`.
- Fable: `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md`.
- Manager: `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-UI-PRESENTATION-V1-INTEGRATION-VERDICT.md`.

Every handoff pinpoints the exact source candidate; worktree/dirty status; test commands and outcomes; real capture paths and what screens they prove; failure/skip; save/Unity lock; revised UI still unaccepted by player until tested. An image without current-state trace is only design illustration.

## Stop cases

Stop on overlapping writer, wrong branch/head, protected dirty files at risk, no safe save isolation for Unity, broken old save/Reference, uncaught commitment/review behavior mutation, invented data, or disabled Back to forest. Use explicit `NOT RUN` for blocked machine checks. No Windows build.
