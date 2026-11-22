# Plan — implement owner 2D UX changes before infrastructure work
**Status: APPROVED direction, implementation pending physical D-091 registration.** Read `spec.md` and source candidate `4cecf87339bea4d32907955a8d5625d1f27f96bd`.

**Stage 0 — narrow source start:** DeepSeek orchestrator refreshes live board, #882/#885 processes and D-091, secures one *new exact-SHA detached or named* source worktree with single Sol writer and an exact `index.html`/`app.css`/`app.js` writable lease. No old worktree overwrite, P0 rebuild or new multi-agent QA tree. If the local disk audit has a conflicting cleanup, preserve the new lane.

**Stage 1 — graphics + map behaviour first:** Worker inspects actual Unity Stand Map read-only and current 2D SVG/CSS source; make spruce canopy/ground/marks substantively better; put compass and vertical +/−/Fit directly over map; implement several smaller zoom increments with stable centering. Commit a coherent intermediate source change rather than wait for tests.

**Stage 2 — choice of tree / user action:** Reuse read-only any-tree competition endpoint; make reference tree persistent while candidate 1–5 cycles and highlights; add obvious one-click candidate Fell proposal button. Keep real tree ID, Crop and pending/approval guards.

**Stage 3 — remove UI friction:** Eliminate compulsory “Other” form; use neutral exact text `Owner-selected felling` only when the owner supplies no optional note/reason; conditional release label only on explicit contextual choice. Simplify routine IDs, visual cues and Work Plan layout; no Core/schema edit.

**Stage 4 — source handoff, not test programme:** Check actual changed source syntax/ability to build *only if needed to deliver an operable artifact*, commit one exact source SHA, publish short GitHub Markdown with U01–U09 DONE/PARTIAL and any blocker and an owner-access route that does not expose private bearer token. No independent reviewer, broad tests, Stage I/L4/Q1/Unity/Windows; don't describe source commit as player verification or owner acceptance.

**Order of priorities if limited:** U01 graphics, U02 map controls, U03 incremental zoom, U05 competitor Fell picking, U06 reason simplification; then U04,U07,U08,U09. U10/U11 OPEN elsewhere. Don't substitute additional planning for executable source.

**Stop conditions:** active overlapping writer in same 3 paths, denied D-091 physical lease, wrong exact base/P0, need to change protected Core/API/saves, another process using session, risk of destroying owner work. Stop affected action and return precise concrete constraint, not broad tooling investigation.