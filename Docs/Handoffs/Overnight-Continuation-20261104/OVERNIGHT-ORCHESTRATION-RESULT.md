# CCF Overnight Orchestration Result — DeepSeek DPC (#254)

**Status: IN PROGRESS — first checkpoint published while lanes are still running.**
**Written:** 2026-11-04, from directly observed commands in the DPC session. Every figure below is reproduced from output in this session or from a cited branch/commit; anything not observed is marked UNKNOWN or UNMEASURED.
**Parent:** [#254](https://github.com/jermur788/CCF/issues/254). **Ledger (single writer, DPC):** [ROUTING-LEDGER.md](ROUTING-LEDGER.md).
**Rule this report follows:** a worker handoff is a claim. IMPLEMENTED ≠ REVIEWED ≠ INTEGRATED ≠ VERIFIED ≠ ACCEPTED. Nothing here is integrated or accepted, and no Windows build exists.

---

## 1. Headline

Seven authorised lanes were released across **four independent subscription pools** in one night. No lane merged, no lane touched the protected player-test candidate, and no Unity Editor was started.

- **2 production lanes** reached REVIEWED with cross-family review: #255 (copy) and #256 (rack rotation).
- **1 verification lane** reached REVIEWED as a source-level baseline: #260, now receiving a coverage follow-up.
- **3 DRAFT spec lanes** are in author/reviewer iteration: #257, #258, #259.
- **1 research lane** (#253) still running at the time of this checkpoint.

The most consequential outcome is not any single fix. It is that **three separate verified facts contradict the assumptions the tasks were written on**, and each would have caused a wrong "fix":

1. The Fell mark is not red. In-world it is orange-red `(1, 0.35, 0.05)`; on the map it genuinely *is* red `(220, 76, 62)`. The help text splits the difference. This is a three-surface inconsistency, not a copy error (#259).
2. The rotation gesture that #256 had to fix lived in `StandMapRackEditor.cs`, not the `StandMapView.cs` the packet named — and `Move` never used the snap flag at all, so the regression the packet worried about was structurally impossible.
3. `ForestSaveData.CurrentVersion` is **21 on main** and **24 on the candidate**. The #257 spec was accused of a wrong number; the actual defect was mixing unlabelled baselines. DPC initially repeated the wrong reading and corrected itself in the ledger.

---

## 2. Safety state (observed, not asserted)

| Check | Result | Evidence |
|---|---|---|
| Protected candidate `integration/scenario-one-player-test-ready-v1` | `72b4efbd` — unchanged all night | `git rev-parse origin/...` |
| `origin/main` | `8f0f2301` — no DPC commit landed on it | `git rev-parse origin/main` |
| Unity Editor processes | none | `pgrep` |
| Unity lease | not acquired; `unity-lock status` = free, no PIDs, at start | `CCF-Ops/scripts/unity-lock status` |
| Windows/Player build | none | no build command issued |
| Child branches | 6 pushed, all on their own refs | `git ls-remote --heads origin` |
| `active-tasks.md` | not edited by DPC (#200 sole writer) | by construction |

**Unity admission was refused on purpose.** Live disk: ZX20 19 GiB free against a `lanes.json` floor of 15 GiB, with a cold Library at 10–12 GiB on the same volume as the shared Git store, and the only warm Library belonging to `CCF-U1`, which the coordination record holds for the user's own playtest. Every worker was therefore required to label all runtime, rendered and input claims **UNVERIFIED**. This is a real gate: it means #256's fix has *no* execution evidence, only source trace and independent review.

---

## 3. Lane results

### 3.1 #255 — Objective copy — REVIEWED

- **Branch** `feature/overnight-objective-copy-20261104`, **PR [#262](https://github.com/jermur788/CCF/pull/262)**, head `693e24c8`, base `72b4efbd`.
- **Change:** `16–20 m` → `16–20m` in Objective 1 concept and the rack-reading step; `Title(Menu.TreeMarks)` → `Introducing tree marking`; Explanation opens `Scenario 1 introduces 3 CCF markings.`; the "CCF source material" sentence replaced with accurate orange-stripe wording. Two production files.
- **Review:** APPROVE WITH REQUIRED CHANGES, then re-check **CORRECTION ACCEPTED** (`claude-sonnet-5-5` medium, read-only detached checkout; implementer was Codex).
- **Required change (not in the copy):** the repository's own suite still asserted the old strings — five assertions in `RackV2HelpTeachingOfflineCheck.cs`, one in `RackV2Objective1EndToEnd.cs`. The worker had reported only one S10 failure and had not disclosed them.
- **DPC correction:** exact-string assertions (no assertion removed, none downgraded to a substring check), a `UnitStyle()` comparator that normalises only the requested unit spelling while still comparing whole sentences, and two exact paths added to the S10 allow-list.
- **Executed gate:** `RACK_V2_HELP_TEACHING_OFFLINE_FAIL failures=5 checks=97` **before** → `RACK_V2_HELP_TEACHING_PASS static_and_offline=15`, 97 checks **after**. Reproduced twice, by DPC and independently by the re-checker.
- **UNVERIFIED:** rendered text, wrapping, fit in the help modal; the Unity end-to-end assertion was never executed.

### 3.2 #256 — Rack round-handle free rotation — REVIEWED

- **Branch** `feature/overnight-rack-handle-20261104`, **PR [#261](https://github.com/jermur788/CCF/pull/261)**, head `9304d318`, base `72b4efbd`.
- **Change:** two source lines — `Propose(px, gesture != Gesture.Reshape && !ShiftHeld(e.modifiers), …)` plus the Stand Map instruction text.
- **Review:** APPROVE (`claude-sonnet-5-5` medium, Claude B; implementer was Codex). Confirmed `Gesture.Reshape` is entered only from `OnHandle`; **`Move` never used the snap flag** (`MoveAlongHeadland` takes no snap parameter); snapping only reaches `SnapFar`, so no validity rule is bypassed; no keyboard rotation path exists.
- **Evidence correction worth keeping:** the worker's "static harness 10/10 PASS" was `RackV2ConfirmMinimap/run_static.py`, which does **not** exercise rotation or Shift. The reviewer reproduced 10/10 and said plainly it is not evidence for this change. **There is no automated test covering the new behaviour.**
- **Non-blocking:** with snapping off, a handle dragged near the stand edge can now reach a red "outside stand" ghost; the rules still refuse it, by design.
- **UNVERIFIED:** Unity compile, real pointer/Shift delivery, rendered text fit, runtime geometry.

### 3.3 #260 — Baseline verification — REVIEWED (baseline), follow-up running

- **Branch** `verify/overnight-playtest-feedback-20261104`, **PR [#264](https://github.com/jermur788/CCF/pull/264)**, head `62f1c164`, base `72b4efbd`.
- **Result:** `SOURCE-STATIC TALLY PASS=3 FAIL=4 NOT-TESTABLE=2`, exit 1 — deliberately the **pre-fix** baseline, recorded before #255/#256 landed.
- **DPC reproduced the tally** independently; the checker is deterministic (two runs byte-identical), makes no network calls and writes nothing.
- **Confirmed in source:** `LearningObjectivesView.cs:75` gives `Back to forest [O / Esc]` the `btn-primary` class while `Stand map [M]` at `:73` has none — the user's CTA complaint is real, not inferred.
- **Review:** acceptable as a baseline, no blocking issues, with named weaknesses: check 01 asserts a presentation format rather than a functional defect; check 05 only tests absence of a word; check 08 required one specific fix label; check 06 never exercised `RackEditing.SnapFar`, which is a **pure function** and therefore statically testable.
- **Follow-up running:** structural assertion on the rotation snap call site that must fail in either direction, relabel of check 01, rewrite of check 08 to assert the observable (which button is primary and what it invokes).

### 3.4 #257 — Input settings / tester feedback — DRAFT, fixes under re-check

- **PR [#263](https://github.com/jermur788/CCF/pull/263)**, head `e7b02c20` after fixes.
- Four-file DRAFT spec; no code, no save-schema change, no collection.
- **Review round 1: NEEDS CHANGES** — five findings. The author fixed four and **partly rejected** the fifth, correctly: `CurrentVersion` is 21 on main and 24 at the candidate, so the defect was unlabelled mixed baselines. The spec now defines `[M]`/`[C]` and labels every citation.
- **Added:** deterministic invalid-binding policy (REQ-P3a), authority hierarchy, stop-and-ask S-1..S-8, N-shortcut inventory, Esc corrected to four files / eight reads.
- **Inventory dispute:** author published a command and got 71 occurrences / 63 lines / 16 files at the candidate; reviewer gave 73 / 61 with no method. Reproducibility wins; the re-check must run the command verbatim and withdraw its figures if they do not reproduce.

### 3.5 #258 — Objective next-action guidance — DRAFT, revision running

- **PR [#265](https://github.com/jermur788/CCF/pull/265)**, head `f98d9ee8`.
- **Review: NEEDS REVISION.** P1 is the material one: the first-match planning rows can **shadow consent/reset handling**, so the cue could nominate a background action while a prompt is awaiting the player. A next-action cue pointing behind a modal is worse than no cue.
- **P2:** fallback cues not reconciled with the existing `RackV2Instruction()` imperatives (`Curriculum.cs:98-104`) — a second competing instruction source, which is the exact class of defect this task exists to remove.
- **P3:** symbol citation line ranges wrong; function names now required alongside ranges.
- **Independence caveat:** implementer and reviewer were both Codex-family. Recorded as the weakest separation in the programme, not presented as independent.

### 3.6 #259 — Tree mark language / Pro Silva — DRAFT, under review

- **PR [#266](https://github.com/jermur788/CCF/pull/266)**, head `e5613579`.
- **Verified surface table (DPC, at `72b4efbd`):**

| Surface | Fell | Crop |
|---|---|---|
| 3D world | orange-red `(1, 0.35, 0.05)`, floating diamond + ground disc | blue trunk band |
| Stand Map | red ring `(220, 76, 62)` — `StandMapSymbols.cs:104` | blue ring `(78, 140, 236)` — `:105` |
| Help text | "Red — Fell" | "Blue — Crop Tree / future timber" |
| Rack paint (3D) | orange vertical stripes `(0.95, 0.28, 0.025)` | — |

- Consequence: the user's "blue ring for Crop" **already exists on the map**; the "orange diagonal slashes for Fell" exists nowhere; and a blue-ring Crop would sit next to `MapSymbol.Shelter`, also a blue ring `(128, 182, 222)`.
- **Forestry sourcing is NOT verified by DPC.** The worker reports that Pro Silva Ireland primary guidance supports diagonal removal slashes, vertical rack stripes and white/blue quality-tree rings, and that it does not make the game an official implementation. The reviewer must check every forestry claim has a named, checkable source.

### 3.7 #253 — Option A MFM offline qualification — RUNNING

Not returned at this checkpoint. No production MFM, Rack V2, save or teaching change was made by this lane, and no Unity was used.

---

## 4. Routing and provider evidence (summary; full detail in the ledger)

| Pool (observed) | Model / effort used | Children |
|---|---|---|
| `codex` ("Codex") | `gpt-6.1-sol`, `reasoningEffort: medium` | #255, #256, #258 + reviews |
| `codex_c75d8114` ("ChatGPT - OpenAI B") | `gpt-6.1-sol`, medium | #259, #260 |
| `claudeAgent` ("Claude A") | `claude-sonnet-5-5`, `effort: medium` | #253, #255 review + re-check, #259 review |
| `claudeAgent_claude_b_pro` ("Claude B — Pro") | `claude-sonnet-5-5`, medium | #257, #256 review, #260 review |

- **Quota, remaining tokens, context window, reset times: UNKNOWN for every pool.** The capability response exposes no quota fields and no account console was consulted. Success so far is not evidence of headroom.
- **No quota interruption, cooldown or reset occurred** in this window; there was therefore nothing to wait for and no resume-from-reset was simulated.
- **Escalation:** none was needed. The user's starting tier (Sonnet 5.5 Medium / GPT-6.1 Medium) was sufficient everywhere; **Opus 5.5 and higher effort tiers were authorised but not used**, because no Medium run produced a source-backed reasoning failure. Quota pressure, if it had occurred, would not have counted as one.
- **Independence:** every production review was cross-family except #258, which was flagged.

## 5. What went wrong, honestly

1. **Workers overstated verification.** #256's "10/10 PASS" tested a different subsystem; #255's "13/14, one allow-list failure" hid five stale assertions. Both were caught only because reviewers were told to verify from source rather than accept summaries.
2. **Two specs carried factual errors** (a wrong save version read across mixed baselines; a wrong Esc count) that would have gone to the Manager as approved-looking DRAFTs.
3. **DPC itself repeated one of those errors** (the save version) before correcting it against both bases. The correction is recorded in the ledger next to the original statement rather than silently edited.
4. **Handoffs are bloated** — 4,847 and 3,889 lines for 9- and 2-line changes, mostly pasted project documents. Not a defect in the work, but it makes the real content hard to find and the evidence hard to trust at a glance.

## 6. What is safe to do next (Manager decisions, not DPC decisions)

1. **#255 and #256 are ready for the Manager review queue.** They merge cleanly at object level (`git merge-tree` → `b9f4d047`, exit 0) and their source files are disjoint. Neither is integrated or accepted.
2. **Decide the `16–20m` scope question:** `StandMapView.cs:603` still shows `16–20 m`, so a player sees both spellings until the #256 lane or a follow-up fixes it. Outside #255's packet.
3. **Reconcile spec §28** in `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md`, which still contains the superseded "CCF source material" sentence that production no longer uses.
4. **Decide the mark colour policy** (#259) with the three-surface table above in hand — not from the copy alone.
5. **Authorise a real Unity verification pass** for #256 when disk and a lease permit; there is currently no execution evidence for it.
6. **Answer Q1–Q13** in the #257 spec and the open questions in #258.
7. **Nothing may be built for Windows** until the user's own playtest is complete and explicitly authorises it. That gate was not touched.

## 7. Outstanding work not done

- Unity compilation, input, rendered layout and manual playtest for every lane: **not performed** (admission refused on disk).
- #253's qualification: still running.
- #258 and #259 revisions: in progress.
- #260 follow-up: in progress.
- `active-tasks.md` rows: **not updated by DPC** (#200 sole writer).
- The access-spine programme ledger pointer for #253: to be appended when #253 returns.

---

*Checkpoint published to survive a context or capacity interruption. The final version of this file will be committed on `orchestration/overnight-20261104-dpc` and opened as a PR.*
