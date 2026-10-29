# Orchestration routing retrospective — PLAYER-TEST-OVERNIGHT-01

**Status:** `ORCHESTRATION RETROSPECTIVE COMPLETE — EVIDENCE READY FOR MANAGER REVIEW`
**Task:** read-only forensic reconstruction of the 2026-10-29 DeepSeek-orchestrated run.
**Retrospective worker:** OpenCode instance `opencode`, model `opencode-go/deepseek-v4.1-flash`, options `variant=max`, `agent=build`.
**Environment inspected:** T3 thread list/read/configuration/search records; parent transcript; GitHub claims and handoffs; Git worktrees/commits/diffs; evidence directories under `/media/jer/Files/CCF/Verification/`; Unity lock records.
**No product code, routing setting, provider setting, task, branch, subscription or original thread was modified.** This file is the only write.

---

## 1. Run identified

| Field | Value |
|---|---|
| Orchestration thread | `3296dda4-268e-4502-bd93-b4cd36d08731` — “Orchestrate Scenario One Player-Test Candidate” |
| Project | `b42e83a0-a78c-4420-ba40-18e8eee6709b` (CCF) |
| Orchestrator | `opencode` / `opencode-go/deepseek-v4.1-flash`; full-access/default; options `variant=max`, `agent=build` |
| Created / main worker activity | `2026-10-29T01:57:40Z`; delegated workers `02:16:27Z`–`11:20:38Z` |
| Parent run state | 8 runs; run 5 (`06:24:53Z`–`10:30:07Z`) was the main orchestration run; runs 2/3/6/7 were cancelled/restarted; run 8 is this retrospective (`14:37Z`) |
| Repository | `jermur788/CCF`; bare store `/media/jer/Files/CCF/Repository.git` |
| Orchestration worktree | `/media/jer/Videos/CCF-Orchestrator-PlayerTest`, branch `orchestration/scenario-one-player-test-ready-v1` |
| Integration worktree | `/media/jer/Videos/CCF-PlayerTestIntegration`, branch `integration/scenario-one-player-test-ready-v1` |
| Child/delegated threads found | 28 (18 completed, 10 failed) |
| Review threads | 6 (Claude A start; MFM-03, PARITY-06, RACK-V2-09, T-06, S1-C T-05 reviews) |
| Runtime task/attempt records | 28 delegated task threads; failed threads include exact run `requestedAt/startedAt/completedAt`; operator (DeepSeek) executions are parent-thread actions, not separate Runtime tasks |
| Major integration heads | `6d3228c` RACK-V2-09 merged; `a00ab65` T-06 merged; `a6ad231` RACK-V2-11; `1bd9083` RACK-V2-10; `dc4f59b` RACK-V2-12; `60fd902` RACK-V2-13; `238bcaa` RACK-V2-14 / Rack exit; `8b6c3b0` PARITY-06; `23fe4b8` PARITY-05; `aca74ac` S1-C T-03 |
| Major handoffs | `CLAUDE-A-START-REVIEW.md`, `RACK-V2-09/10/11/12/13/14`, `S1-D-Implementation-06-Unity`, `S1-ThreeSpeciesParity-P0-Implementation-06`, `PARITY-05` baseline/anchor files, `S1-C-Implementation-03`, `MFM-Implementation-03/04/05`, `S1-C-Implementation-05` |

**Scope exclusion:** user-created top-level threads after 13:00Z (for example the separate MFM-06 and S1-C T-05 Unity threads) are adjacent follow-up sessions, not children of #91. They are excluded unless evidence shows a parent/child link. The parent thread’s later run 8 is this retrospective.

## 2. User-supplied routing policy

Preserved from the parent thread. Wording is quoted where exact.

1. **Initial mandate, `2026-10-29T01:57:40Z`:**
   - DeepSeek is orchestration/integration lead, not product owner.
   - **First substantive delegate must be Claude A on Claude Opus, medium**; do not substitute.
   - Later bounded work may use **Claude B** and **OpenAI 6.1 Sao/Sol** as available.
   - High-risk save/reference/determinism work should use **cross-provider independent review** (if Claude implements, OpenAI reviews; if OpenAI implements, Claude reviews); a helper from the implementing thread is not independent.
   - Restore `/media/jer/ZX20` above the 15 GiB floor only through safe reversible relocation; never touch dirty/user/active/unrelated work.
   - Heavy Unity is serialized machine-wide; one writer per subsystem; integrate only qualified work.
   - **No Windows build.**
   - Continue until Rack, parity, S1-C, S1-D and MFM are integrated on one candidate, then run final Tier-3 and independent review.
2. **Storage override, `2026-10-29T02:18:03Z`:** “you can use the videos partition instead of the zx20”.
3. **Default Claude routing, `2026-10-29T06:22:39Z`:** “once the claude limits reset, i want you to default to using sonnet medium when using the claude subs unless the subtask is very complicated or sonnet gets stuck, otherwise use opus low”.
4. **Clarification, `2026-10-29T06:24:53Z`:** “no I mean default to sonnet medium, escalate to opus low, if opus low gets stuck, escalate to higher opus setting, use sol 6.1 at medium, or high depending on the task when it is available”.
5. **All-pools-out fallback, `2026-10-29T06:28:31Z`:** “also if all other subs are out use deepseek to try and continue”.
6. **Product decisions supplied during the run:**
   - S1-D T-03 cap: pre-completion cap hard only while planned/marked work exists (`09:21:34Z` orchestration record).
   - PARITY-05 browse anchors: accept the new hashes with exact-base attribution (question-tool answer before the authorised update; exact answer timestamp is not exposed in message search).

**What the user did not specify:** a numeric cost/credit budget, a fixed provider priority beyond the Claude default/escalation, a mandatory remaining-quota threshold, an account/pool distinction, or a per-task difficulty taxonomy.

## 3. Orchestrator-added policy

These were added or operationalised by DeepSeek; they are not direct user instructions:

- Mandatory Claude A Opus-medium start review, enforced before implementation.
- G2 schema/writer order: Rack v21 final → S1-C T-05 v22 → MFM-06 next; single writer for `ForestSaveValidation.cs`, `ScenarioOneManager.cs`, history/UI surfaces and guide files.
- One Unity lane (U1) and one machine-wide lock; storage floor and clean-lane checks before Editor.
- Capacity fallback order in practice: keep the WIP, reassign to another provider/model, or complete as DeepSeek operator when all delegated pools were unavailable.
- Model heuristics:
  - Claude Sonnet medium for bounded static/UI/copy work after the 06:22 policy.
  - Claude Opus low as escalation; higher Opus for high-risk save review.
  - Sol 6.1 medium/high for implementation and Unity qualification.
  - Claude A Opus for independent start/review work.
- Preservation of exact failed-work state: commit WIP checkpoints, push branches, record failed evidence, never reset/clean another owner’s work.
- Integration only after the relevant gate/handoff; the operator could run gates but was not treated as a fully independent reviewer.
- Require explicit user authorization before changing a protected anchor (used for PARITY-05).

## 4. Worker attempt inventory

Timestamps are UTC from T3 thread/run records. “Outcome” separates the thread’s own completion from later acceptance.

| # | Work unit | Provider / model / effort | Start → end | Status | Outcome / acceptance |
|---|---|---|---|---|---|
| 1 | Claude A start review | Claude `claudeAgent` / `claude-opus-5-5` / medium | 02:16:27 → 02:56:41 | completed | `OVERNIGHT PROGRAMME READY TO EXECUTE`; accepted |
| 2 | RACK-V2-09 field marking | Codex `codex` / `gpt-6.1-sol` / high | 02:31:40 → 03:08:44 | failed | Code and field-04 pass committed; stopped on usage limit (reset 6:38); accepted after Claude B takeover |
| 3 | MFM-03 coverage calibration | Claude B `claudeAgent_claude_b_pro` / `claude-opus-5-5` / high | 02:31:50 → 03:23:16 | completed | Qualified 60792dc; independently reviewed 73356e3; accepted |
| 4 | PARITY-06 broadleaf timber/pruning | OpenCode `opencode` / `openai/gpt-6.1-sol` / high | 02:50:32 → 03:11:11 | failed | WIP 4fb0d92 preserved; usage limit with no reset; accepted after handoff/review/Unity |
| 5 | MFM-03 independent review | Claude A / `claude-opus-5-5` / medium | 02:57:22 → 03:33:59 | completed | PASS 73356e3; accepted |
| 6 | RACK-V2-09 runtime/save takeover | Claude B / `claude-opus-5-5` / high | 03:14:25 → 03:57:08 | completed | field-05 99/10, runtime 14, save 35; 8f8316b/0399402; accepted |
| 7 | PARITY-06 independent review | Claude A / Opus 5.5 / medium | 03:15:22 → 03:50:59 | completed | PASS c1b5774; accepted |
| 8 | RACK-V2-09 independent review | Claude A / Opus 5.5 / medium | 03:28:27 → 04:03:22 | completed | PASS 04913c0; integrated |
| 9 | MFM-04 deterministic core | Claude B / Opus 5.5 / high | 03:29:29 → 05:08:10 | failed | WIP e635ef7; two session-limit failures; repaired by #10 |
| 10 | T-06 Unity qualification | Claude B2 / Opus 5.5 / high | 03:30:41 → 04:21:21 | failed | Q1/Q2 work and fixture diagnosis; 7am session limit; operator completed Q1–Q5; Claude A review PASS; accepted after test fix |
| 11 | RACK-V2-10 Help/teaching | Claude B3 / Opus 5.5 / high | 03:38:06 → 05:08:01 | failed | WIP db17d5d; 7am session limit; operator completed static 97/15; integrated |
| 12 | RACK-V2-11 persistence | Claude A / Opus 5.5 / medium | 03:45:30 → 04:27:12 | completed | Static harness ready; operator ran v2 30/coldstart 2/legacy40 7/save03b 35; integrated |
| 13 | T-06 fixture review | Claude A / Opus 5.5 / medium | 04:37:38 → 05:11:36 | completed | PASS 0addaf8; integrated |
| 14 | RACK-V2-12 functional | Claude B / Opus 5.5 / high | 04:55:59 → 05:26:04 | failed | Failed in 4 s on 7am session limit; committed 92a79e9 + uncommitted UI fix; operator completed |
| 15 | MFM-04 area-parity repair | Claude A / Opus 5.5 / medium | 04:56:07 → 05:34:12 | completed | 236/236, exact interval fix 71674e4/5b39001; accepted static |
| 16 | RACK-V2-12 functional retry | Claude A / Opus 5.5 / high | 05:04:34 → 05:22:42 | failed | Baseline diagnostics, then 7:10 session limit; operator completed a900b7f 173/24 + model0/legacy40 |
| 17 | RACK-V2-13 rendered review | Sonnet `claudeAgent` / `claude-sonnet-5-5` / medium | 06:35:54 → 07:06:02 | failed | Failed in 8 s with reset 7:10; operator completed visual PASS b77d898 |
| 18 | PARITY-06 Unity | Sol `codex` / `gpt-6.1-sol` / high | 06:40:35 → 10:44:58* | completed | 14/14 configs PASS at 6abfb59/283c9f3; accepted/integrated |
| 19 | MFM-05 Crop Tree candidates | Claude Opus low `claudeAgent` / `claude-opus-5-5` / low | 06:41:39 → 07:11:46 | failed | Failed in 7 s with reset 7:10; no work |
| 20 | MFM-05 Crop Tree candidates | Sol `codex` / `gpt-6.1-sol` / medium | 07:10:35 → 10:44:58* | completed | 95/95 + 236/236 regression, 4b54af0/eccc2fd; accepted static, not integrated |
| 21 | PARITY-05 harness reconciliation | Sol high | 07:17:50 → 10:44:58* | completed | Diagnosed stale Legacy fixture/float/hash drift; G1/G2 PASS, G3 blocked; operator baseline + user anchor decision; accepted |
| 22 | S1-C T-03 rendered | Sol high | 09:07:23 → 10:44:58* | completed | 279/0, T-02 246/0, 16 captures; e998d42/51d4fc5; integrated |
| 23 | PARITY-07 player truth | Sonnet medium | 09:08:15 → 09:41:13 | completed | 77/77 static; 1e7b941/c7e88c1; accepted static, Unity/render pending |
| 24 | S1-D T-03 cap decouple | Sonnet medium | 09:09:09 → 09:55:16 | completed | 48 checks + protected suites; user decision; af00d87/fa0d308; Unity pending |
| 25 | S1-D T-03 Unity | Sol high | 09:33:19 → 09:45:15 | failed | cap-v3 PASS, teaching-v2 PASS; usage limit reset 11:40; lesson anchor blocked; unresolved |
| 26 | S1-C T-05 v22 | Sonnet medium | 09:34:36 → 10:45:51 | completed | Candidate cd7b518; Opus review found 3 blocking bugs; fix 7ae0bbc, 115 checks; re-review PASS 410b6fe; not integrated |
| 27 | S1-C T-05 review | Opus 5.5 / medium | 09:57:31 → 10:55:59 | completed | Round 1 NEEDS CHANGES; round 2 PASS; accepted for v22 lineage |
| 28 | S1-D T-03 Unity completion | Sonnet medium | 09:57:07 → 11:20:38 | completed | Teaching fix; cap-v3/teaching-v2 PASS; RackLesson anchor 7.995 vs 8.00385 fails on base too; chain unresolved |

\* T3 thread `updatedAt` is a bulk 10:44:58 for several Codex threads; use the thread run records and per-run receipts, not `updatedAt`, as end times.

**Operator (DeepSeek) completions not counted as worker threads:** RACK-V2-09 field/runtime/save recovery, RACK-V2-10 static completion, RACK-V2-11 Unity, RACK-V2-12 end-to-end/harness, RACK-V2-13 visual review, T-06 Q1–Q5/fixture repair, PARITY-05 baseline attribution, MFM-04/05/07 static verification, and several integration gates. These are reconstructible from the parent thread but have no dedicated child-thread identity.

## 5. Routing decision timeline

### Decision 1 — mandatory first delegate
- Time: 01:57–02:16Z. Work: programme audit. Candidates: Claude A Opus medium (mandatory). Selected: `claudeAgent/claude-opus-5-5`, medium. Reason: packet/user mandate. Capacity: not applicable. Alternatives: none permitted. Result: PASS. Hindsight: clearly good; enforced exactly.

### Decision 2 — initial parallel programme lanes
- Time: 02:16–02:50Z. Work: Rack, MFM-03, PARITY-06. Selected: RACK-V2-09 Codex Sol high; MFM-03 Claude B Opus high; PARITY-06 OpenCode Sol high. Reason: capability fit, non-overlapping files, parallel static/Unity. Capacity: no direct provider quota observation; assumed available. Result: MFM-03 succeeded; Rack worked until Codex limit; P06 worked until OpenCode limit. Hindsight: reasonable parallel split, but two of three lanes hit capacity within 30–40 min.

### Decision 3 — RACK-V2-09 Codex → Claude B
- Time: 03:08Z. Previous: Codex run failed at 03:08:44 with reset 6:38. Selected: Claude B Opus high, starting from 1985a7b (field-04 PASS). Reason: direct capacity failure; preserve passed field evidence; take over runtime/save. Capacity: direct provider reset, exact; remaining unknown. Result: PASS 8f8316b/0399402. Hindsight: clearly good; no production rework, only a runner-owner fix.

### Decision 4 — MFM-03 review → Claude A
- Time: 02:57Z. Work: independent review of Claude B’s measurement. Selected: Claude A Opus medium, separate thread. Reason: independence; Claude A role. Capacity: none observed. Result: PASS. Hindsight: good; one provider family but separate thread; no blocking finding.

### Decision 5 — PARITY-06 OpenCode limit → WIP + later Sol/A review
- Time: 03:11Z. Previous: OpenCode failed with “usage limit reached”, no reset. Selected: preserve 4fb0d92; later Claude A review and Sol Unity. Reason: direct capacity; do not discard verified WIP. Capacity: direct event, reset unknown. Result: review PASS, Unity 14/14, integrated. Hindsight: clearly good salvage; the capacity reset was unknown, so waiting was not an option.

### Decision 6 — T-06 → Claude B2
- Time: 03:30Z. Work: Unity save qualification. Selected: Claude B Opus high. Reason: strong save/Unity work; Claude available. Capacity: no direct quota; Claude session limits followed at 03:51 (7am). Result: Q1/Q2 at 1a44526; fixture failure; test fix; operator final; review PASS. Hindsight: reasonable capability choice, but the lane crossed the Claude reset window and required operator completion.

### Decision 7 — RACK-V2-10 → Claude B3
- Time: 03:38Z. Work: Help/teaching static. Selected: Claude B Opus high. Reason: strong implementation. Capacity: no direct quota; session limit at 04:38 (7am). Result: WIP db17d5d; operator completed 97/15. Hindsight: capability fit but capacity timing poor.

### Decision 8 — RACK-V2-11 → Claude A
- Time: 03:45Z. Work: persistence static preflight/harness. Selected: Claude A Opus medium. Reason: save/persistence review strength. Capacity: no direct quota; completed before limit. Result: static ready; operator ran Unity. Hindsight: good.

### Decision 9 — T-06 review → Claude A
- Time: 04:37Z. Work: review test-only fixture fix. Selected: Claude A Opus medium. Reason: independence from Codex/Claude B implementers. Capacity: no direct quota; completed. Result: PASS. Hindsight: good.

### Decision 10 — RACK-V2-12 → Claude B, then Claude A, then operator
- Time: 04:55Z, 05:04Z. Work: functional E2E. Selected: Claude B Opus high; after 4 s session-limit failure, retried with Claude A Opus high; after 7:10 limit, operator completed. Reason: strong model for Unity E2E; retry due to capacity, not code. Capacity: direct session-limit events (7am/7:10am); remaining unknown. Result: operator a900b7f PASS 173/24 + model0/legacy40; accepted. Hindsight: the retry before the known 7:10 reset was predictable waste; the operator salvage was effective.

### Decision 11 — MFM-04 repair → Claude A
- Time: 04:56Z. Work: exact area parity after Claude B WIP. Selected: Claude A Opus medium. Reason: continuity of MFM-03 reviewer, precise measurement. Capacity: no direct quota. Result: 236/236. Hindsight: clearly good; strong model materially fixed the core.

### Decision 12 — RACK-V2-13 → Sonnet, then operator
- Time: 06:35Z. Work: rendered review. Selected: Sonnet medium (user default). Capacity: known 7:10 reset from prior messages; launched at 06:35, failed at 06:36. Result: operator completed visual review. Hindsight: questionable; should have waited until 07:10 or used a non-limited provider.

### Decision 13 — PARITY-06 Unity → Sol
- Time: 06:40Z. Work: 14-config Unity qualification. Selected: Sol high. Reason: Codex reset at 06:38; Unity/qualification strength. Capacity: direct reset time; started 2 min after. Result: 14/14 PASS. Hindsight: clearly good; best-timed capacity resume in the run.

### Decision 14 — MFM-05 → Opus low, then Sol
- Time: 06:41Z, 07:10Z. Selected: Opus low (user escalation tier) at 06:41; failed in 7 s with 7:10 reset; then Sol medium after 07:10. Reason: user default/escalation; then available Sol. Capacity: direct reset known; the first launch was premature. Result: Sol 95/95 + 236; accepted static. Hindsight: Opus-low attempt was avoidable; Sol was the right fallback.

### Decision 15 — PARITY-05 → Sol, then operator attribution/user decision
- Time: 07:17Z. Work: diagnose P05 Unity failures. Selected: Sol high. Reason: Unity/qualification capability; no Sonnet/Opus available. Capacity: no direct quota event at launch; Sol later remained available. Result: diagnostic blocked on anchors; operator reproduced the same hashes on `1d7445e`; user authorised new values. Hindsight: good technical work; the protected-anchor decision correctly escalated rather than silently rebaselined.

### Decision 16 — S1-C T-03 → Sol
- Time: 09:07Z. Work: rendered T-03. Selected: Sol high. Reason: Unity/rendered strength; Sonnet/Opus not chosen for a Unity-heavy gate. Capacity: no direct quota event. Result: 279/0 and 246/0; integrated. Hindsight: clearly good.

### Decision 17 — PARITY-07 / S1-D T-03 / T-05 → Sonnet; T-05 review → Opus
- Time: 09:08–09:57Z. Selected: Sonnet medium for static UI/logic (user policy); Opus medium for high-risk v22 save review. Reason: user default, high-risk escalation, independence from implementer. Capacity: Sonnet available after reset; Opus available. Result: P07 static 77; S1-D T-03 static 48 + decision; T-05 115 checks after Opus found 3 bugs and re-review PASS. Hindsight: mostly good; T-05 review was same provider family, not cross-provider as the packet preferred.

### Decision 18 — S1-D T-03 Unity → Sol, then Sonnet
- Time: 09:33Z, 09:57Z. Work: Unity cap/teaching/lesson. Selected: Sol high; after 11:40 usage limit, Sonnet medium completion. Reason: Sol Unity strength; Sonnet capacity. Capacity: direct Sol usage limit reset 11:40; Sonnet available. Result: cap-v3 PASS, teaching-v2 PASS, RackLesson anchor 7.995 vs 8.00385 fails on base too; unresolved. Hindsight: reasonable fallback; the lesson-anchor mismatch is a verification issue, not a routing failure.

## 6. Subscription / capacity timeline

No provider exposed a remaining percentage or absolute quota. The only direct capacity data were limit events and reset times.

| Time (UTC) | Provider / instance | Signal | Reset | Source | Orchestrator reaction | Later evidence |
|---|---|---|---|---|---|---|
| 03:08:44 | OpenAI Codex (`codex`) | “You’ve hit your usage limit… try again at 6:38 AM” | 06:38 | T3 worker failure / provider notification | RACK-V2-09 handed to Claude B | Correct; takeover PASS |
| 03:11:11 | OpenCode (`opencode`, OpenAI Sol model) | “The usage limit has been reached” | UNKNOWN — PROVIDER SIGNAL NOT OBSERVED | T3 worker failure | Preserved WIP; later Claude A review + Sol Unity | Correct; work accepted |
| 03:50:42 / 03:51:21 | Claude B (`claudeAgent_claude_b_pro`) | “You’ve hit your session limit · resets 7am (Europe/Dublin)” | 07:00 | Provider assistant message | T-06/MFM-04 lanes stopped; operator/resume later | Correct event; several premature resumes followed |
| 04:38:10 | Claude B | Same reset 7am | 07:00 | Provider message | MFM-04/RACK-V2-10 resume immediately | Failed in seconds; premature |
| 04:56:04 | Claude B | Same reset 7am | 07:00 | Provider message | RACK-V2-12 attempted anyway | Failed in 4 s; premature |
| 05:22:41 | Claude A (`claudeAgent`) | “resets 7:10am (Europe/Dublin)” | 07:10 | Provider message | RACK-V2-12 retry already attempted | Failed; window moved |
| 06:36:02 | Claude Sonnet (`claudeAgent`) | Same reset 7:10am | 07:10 | Provider message | RACK-V2-13 then ran by operator | Premature launch; avoidable |
| 06:41:46 | Claude Opus low (`claudeAgent`) | Same reset 7:10am | 07:10 | Provider message | MFM-05 then moved to Sol at 07:10 | Premature launch; Sol correct |
| 09:45:15 | OpenAI Codex (`codex`) | Usage limit; notification says try again 11:40 AM | 11:40 | T3 worker failure | S1-D T-03 Unity moved to Sonnet | Correct fallback; lane remains unresolved |
| 06:38/07:00/07:10/11:40 | All | Reset times only; no remaining quota | — | Provider/T3 | — | `UNKNOWN — PROVIDER SIGNAL NOT OBSERVED` for remaining capacity on every provider |

**Account/pool distinction:** T3 exposes `claudeAgent` and `claudeAgent_claude_b_pro` as separate provider instances; whether they share one Claude subscription pool is **UNKNOWN**. Codex and OpenCode expose the same `gpt-6.1-sol` model through different provider instances; whether they share a ChatGPT/OpenAI quota pool is **UNKNOWN**. No credential, token or account identifier is reproduced here.

## 7. Worker outcome analysis

**Counts supported by T3 records:**

- 28 delegated child threads: **18 completed, 10 failed**.
- **10/10 failures were capacity/limit events**, not code defects: 4 Claude “session limit” events, 1 Codex “usage limit”, 1 OpenCode “usage limit”, 1 Codex “usage limit” at 11:40, plus retries of the same Claude limit. None was a model refusal or coding failure.
- **9 of 10 failed attempts were salvaged** by another worker or the DeepSeek operator; 1 (S1-D T-03 Unity) remains unresolved at the RackLesson anchor.
- **6 review threads** completed; **1 found blocking defects** (S1-C T-05: three history bugs), followed by a fix and PASS. The other five reviews passed or accepted with non-blocking notes.
- **5 completed threads remain static-only or pending Unity/integration**: PARITY-07, MFM-04, MFM-05, S1-C T-05 (v22 not integrated), S1-D T-03 (Unity unresolved). S1-D T-03 Unity completion is additionally partial.
- **Operator work is undercounted**: at least 8 substantive recoveries were performed in the parent thread rather than as delegated workers (RACK-V2-09 runtime/save, RACK-V2-10 static, RACK-V2-11 Unity, RACK-V2-12 E2E, RACK-V2-13 visual, T-06 Q1–Q5/fixture, PARITY-05 baseline, plus integration gates).

**Patterns:**

- Sonnet medium performed well on bounded static/UI work: P07 77/77, S1-D T-03 48 checks, T-05 115 checks after repair. It did not independently catch the three T-05 history bugs; Opus review did.
- Opus review materially improved high-risk work: 1 review found three blocking save-history defects.
- Sol was the strongest sustained Unity/qualification lane: P06 14/14, S1-C T-03 279/0, P05 diagnosis; RACK-V2-09 field gate and code were produced before its limit.
- Opus low produced no usable work in this run: launched before reset, failed in 7 s. This is a capacity-timing failure, not evidence of low-effort model quality.
- Provider switching after capacity pressure worked well: 9/10 failed attempts salvaged; the main cost was elapsed time and duplicate failure records, not lost code.
- No evidence that a stronger model was used for unnecessarily simple work. The opposite risk appeared: Sonnet/Opus launches near reset windows were wasted.
- No direct evidence of context-length degradation; the parent used child threads, and the main failures align with capacity/tooling.

## 8. Routing decisions that appear effective

- **Mandatory Claude A Opus-medium start review** — exact user/packet compliance; returned a usable PASS and G0–G3 conditions.
- **RACK-V2-09 Codex → Claude B takeover at 1985a7b** — preserved the passed field evidence and completed runtime/save without production rework.
- **MFM-03 Claude B + Claude A review** — separate implementer/reviewer, exact measurement, PASS.
- **PARITY-06 WIP preservation after OpenCode failure** — no lost work; later review and Unity qualified 14/14.
- **PARITY-06 Unity on Sol immediately after the Codex 06:38 reset** — the best-timed capacity resume in the run; 14/14 PASS.
- **MFM-04 repair by Claude A** — the exact-area fix took 236/236; strong model materially improved correctness.
- **S1-C T-03 rendered on Sol** — 279/0, T-02 246/0, 16 captures, integrated.
- **T-05 implementation on Sonnet + independent Opus review** — the review found three real save-history bugs; the fix/re-review passed. This is the clearest evidence for independent review value.
- **PARITY-05 anchor escalation** — instead of rebaselining, the operator reproduced the same hashes on the exact base and asked the user; the user authorised the update with attribution.
- **One-writer/worktree discipline** — no evidence of two writers in the same subsystem; Unity remained serialized; no Windows build.

## 9. Routing decisions that appear questionable

- **Six Claude launches occurred before the known 07:00/07:10 reset** (T-06/MFM-04/RACK-V2-10 resumes at 04:37–04:38, RACK-V2-12 at 04:55, RACK-V2-12 Claude A at 05:04, RACK-V2-13 at 06:35, MFM-05 Opus at 06:41). Each failed in seconds. The reset time was already in the provider messages; a simple “do not launch before reset” guard would have avoided 6 failed threads.
- **Same-provider review for the highest-risk v22 save change:** Sonnet implemented T-05, Opus reviewed it. This is Anthropic→Anthropic, not the packet’s preferred cross-provider review for high-risk save/reference/determinism. The review was effective, but the independence guarantee is weaker.
- **Operator performed both completion and visual review for RACK-V2-13.** The operator’s R13 review was a real image inspection, but it is not independent of the operator’s R12 completion/integration work. The Rack exit therefore rests on operator-run gates without a separate final reviewer.
- **Operator completions are invisible as Runtime attempts.** RACK-V2-09 runtime/save, RACK-V2-10 static, RACK-V2-11 Unity, RACK-V2-12 E2E and T-06 Q3/Q4 were done in the parent thread. A future audit cannot see them as worker records or compare their model/cost.
- **Premature “completed” child turns.** The S1-D T-03 Unity completion thread signalled completed while a detached chain was still running; the orchestrator had to wake it and later inspect the evidence. T3’s thread `status=completed` was not a reliable task-completion signal.
- **Capacity error wording was not normalized.** “session limit”, “usage limit”, “rate limit”, and “Claude API rate limit reached” are treated alike in the parent notifications, but one had a reset time and one did not. This increased reconstruction cost.
- **No durable routing log during the run.** The orchestrator’s rationale lived in tool calls/chat; the retrospective had to infer route source and alternatives from delegate prompts and thread metadata.
- **The initial parallel plan assumed all three pools were available.** Two of three lanes hit limits within 40 minutes; a preflight capacity/reset check would have avoided the immediate failures.

## 10. Missing information that would have improved decisions

| Missing signal | Why it mattered | Likely effect |
|---|---|---|
| Remaining subscription capacity / percentage | Only reset times were exposed; “available” was inferred | Could have avoided six premature launches and chosen Sol/operator earlier |
| Reset timestamp in the parent notification, normalized to UTC, with timezone | Child messages had exact resets; parent notifications were vaguer | Faster, safer queuing |
| Account/pool distinction (Claude A vs B; Codex vs OpenCode) | Whether limits are shared was unknown | Could prevent cross-instance retries hitting the same exhausted pool |
| Task difficulty / risk class at selection | Model choice relied on human judgement | Better automatic Sonnet/Opus/Sol recommendation |
| Historical success/rework rate per model/task | No durable per-model outcome table | Avoid Opus-low for bounded static work if history shows no benefit; identify Sonnet’s save-review gap |
| Accepted outcome separate from worker “completed” | Thread status was misleading | Prevent dependent work starting on an unresolved task |
| Current verification state / anchor status | P05 hash mismatch and S1-D lesson anchor were discovered late | Route verification/anchor work earlier |
| Tool capability differences | Image review, Unity, native input, `.NET` only | Avoid assigning a capability-mismatched worker |
| Permission/runtime mode | Mostly full-access, but not systematically recorded | Prevent approval stalls |
| Context size/thread degradation | Parent reached 1003 items; not measured | Could explain late-run routing drift |
| Cost/reserve priority | User policy mentioned conserving stronger models but no budget | Harder to decide Opus vs Sonnet vs Sol |

## 11. Patterns useful for future routing

- Use **Sonnet medium** for bounded static/UI/copy work; use **Opus or Sol review** for save/history/determinism logic. Sonnet’s static work was reliable; its save-history design needed Opus review.
- Use **Sol 6.1 high** for Unity-native gates and long qualification chains; it sustained the longest Unity runs in this session.
- Use **Opus medium** for independent high-risk review; it found the only blocking design defects.
- **Do not launch a limited provider before its reset**, even for a “quick” task. Check the reset timestamp and queue or use another provider.
- **Preserve WIP immediately** when a worker fails; 9/10 failures were salvaged because branches/commits/patches survived.
- **Separate implementer and reviewer**; same-provider review is better than none, but cross-provider is the stronger rule for high-risk work.
- **Treat operator work as a first-class attempt** if the operator completes gates/implementation; otherwise telemetry is incomplete and independence is unclear.
- **Do not treat a child thread’s `completed` as task acceptance.** Require explicit gate/handoff/integration evidence.
- **Escalate protected-anchor changes to the user/Manager**, even when the evidence strongly suggests the old anchor is stale.

## 12. Recommended standard orchestration record

### Automatically recordable by T3

- Parent/child thread ID; delegated task ID; run/attempt ordinal; retry-of reference.
- Requested/started/completed timestamps in UTC; duration; reset timestamp and timezone.
- Provider instance ID; model ID; effort/reasoning option; runtime/interaction mode.
- Route source: user-launched, native subagent, `delegate_task`, queue/resume, operator/manual.
- Candidate set considered and rejected, if the orchestrator supplies it.
- Provider error code/message (normalized taxonomy: quota exhausted, rate limited, session limit, auth, tool failure, user cancel).
- Capacity event reference: exact provider message, reset time, remaining quota if provider exposes it.
- Previous attempt/outcome link; branch/worktree/commit; evidence/handoff path.
- Worker terminal status and whether the handoff/gate produced a pass marker.
- Tool availability/capability declaration for the attempt.

### Orchestrator-declared at selection time

- Task difficulty and risk class (routine / bounded / high-risk save-reference-determinism / architecture).
- Why the normal recommendation was overridden.
- Reserve strategy (for example “save Opus for final review”).
- Why an otherwise eligible worker was deliberately avoided.
- Independence requirement (same thread / same provider / cross-provider).
- Concurrency/one-writer constraints.
- Expected acceptance gate and definition of done.

### Outcome-derived later

- Accepted / accepted after minor rework / accepted after substantial rework / rejected / superseded / unresolved.
- Review findings and whether they were blocking.
- Integration commit and verification state.
- Realized difficulty and rework count.
- Protected-anchor movement and authorization reference.
- Whether the routing decision appears effective in hindsight.

## 13. Orchestration Decision Appendix assessment

Assuming the proposed appendix has: orchestration policy, decision log, capacity events, worker outcomes, end-of-session assessment.

**Proved useful:** policy section (separates user vs orchestrator), decision log, capacity events with reset times, worker outcome table, end-of-session assessment. The exact reset timestamps and provider/model/effort were the most valuable facts.

**Redundant:** manual duplication of provider/model/timestamps already in Runtime; free-text session titles; repeated “outcome=completed” statements that do not distinguish acceptance; per-decision prose that restates the task name.

**Missing:** route source, candidate set/rejections, independence requirement, task risk class, pre-run difficulty, acceptance/verification state, rework count, anchor-change authorization, workspace/branch, context size, provider account/pool alias, normalized error taxonomy, timezone on reset.

**Cannot be filled reliably by hand:** exact timestamps, candidate set, provider error text, reset times, final outcome, integration state. These must be automatic or linked to Runtime objects.

**Should be automatic:** IDs/timestamps/provider/model/options/error/reset/retry link/terminal status/evidence path. **Should stay declared:** risk class, override reason, reserve strategy, independence requirement. **Should stay outcome-derived:** acceptance, rework, review findings, integration, realized difficulty.

## 14. ARM-020/030/040/045/050/070 mapping

| Stage | Finding | Owner |
|---|---|---|
| ARM-020 — execution context observation | T3 captured thread/provider/model/options/timestamps/workspace but not context size, tool capability or acceptance state; operator work was invisible as an attempt | ARM-020 should capture thread context, workspace, tools, permissions, context size and operator/manual attempts |
| ARM-030 — subscription capacity observation | Only limit events and reset times were observed; no remaining quota; reset timezone and account/pool distinction missing | ARM-030 should standardize capacity events, reset timestamps/timezone, exact provider message, account/pool alias, and `UNKNOWN` when no quota signal exists |
| ARM-040 — predicted/realized difficulty | Difficulty was human/orchestrator judgement; the run did not compare predicted vs realized or tie rework to difficulty | ARM-040 should record pre-run difficulty/risk and post-run realized difficulty/rework |
| ARM-045 — authoritative accepted outcomes | Worker “completed” status was not acceptance; review/fix/integration decided acceptance | ARM-045 should own accepted/rejected/needs-changes/integrated states, linked to gates/reviews, not worker self-report |
| ARM-050 — advisory recommendations | Recommendations were policy/heuristic and often capacity-aware, but overrides and alternatives were not durable | ARM-050 should consume ARM-030/045 and emit an advisory with candidates, reasons, confidence and override logging |
| ARM-070 — operator explanation/read surfaces | The operator could explain decisions in chat but not read a durable capacity/outcome history | ARM-070 should expose decision/capacity/outcome timeline and “why this worker, why not that one” |

No ARM redesign is proposed here; these are ownership mappings only.

## 15. Spec questions

- **SPEC QUESTION — cross-provider independence:** Should high-risk save/reference/determinism work require a different provider family (Anthropic→OpenAI or OpenAI→Anthropic), not merely a different model/thread? The T-05 review was Sonnet→Opus (same provider family) while the packet preferred cross-provider. The review was effective but the guarantee is weaker.
- **SPEC QUESTION — known-reset launch rule:** When a provider has reported a reset time but no remaining quota, should T3 forbid (or warn on) launching that provider before the reset? Six Claude launches occurred before the known reset and failed in seconds.
- **SPEC QUESTION — operator-as-worker telemetry:** When the DeepSeek operator completes implementation or gates after provider exhaustion, should that be recorded as a first-class Runtime attempt with model/effort and independence status? Currently it is parent-thread action, invisible in the worker inventory, and can blur review independence.
- **SPEC QUESTION — acceptance gate:** Should T3 require an explicit accepted/rejected/integrated outcome before scheduling a dependent task, rather than treating `thread.status=completed` as sufficient?

## 16. Evidence limitations

- The retrospective runs inside the same parent thread, after the main orchestration run. Parent run 5 completed at `10:30:07Z`; the retrospective is run 8 at `14:37Z`.
- T3 thread `updatedAt` for several Codex threads is a bulk `10:44:58Z`, not the true run end; run `completedAt` and per-run receipts are used instead.
- Some failed threads (OpenCode PARITY-06, Codex S1-D T-03 Unity) had no message items after failure; the error text comes from parent notifications/transcript, not the child thread.
- No provider exposed remaining quota/percentage. All “capacity” statements here are reset/limit observations, not quota estimates.
- Account/pool aliases are unknown; provider instance IDs are used as privacy-safe aliases.
- Operator work (DeepSeek) is not a child thread/task in the Runtime record; it is reconstructed from the parent transcript, Git and evidence directories.
- Separate top-level threads created after 13:00Z are excluded as adjacent work unless linked to #91.
- Handoffs and Git commits are the acceptance evidence; a worker’s “completed” message alone is not treated as acceptance.
- Some product decisions were made through the question tool; their exact answer timestamps are not exposed by message search, so they are recorded by the orchestrator’s immediate follow-up timestamps.

## 17. Exact next action

1. Manager review this retrospective and decide whether to adopt the proposed automatic/declared/outcome-derived record split.
2. Add a T3/Runtime capacity preflight that blocks or warns on a provider launch before its reported reset, and normalizes quota/session/rate-limit errors.
3. Require cross-provider independence for high-risk save/reference/determinism reviews (SPEC QUESTION), or explicitly record a same-provider exception.
4. Make operator/manual completions first-class Runtime attempts with model/effort, scope and independence state.
5. Do **not** implement ARM changes from this retrospective; the ARM mapping is advisory only.

---

## Mandatory handoff

| Field | Value |
|---|---|
| Provider/model used | `opencode` / `opencode-go/deepseek-v4.1-flash`, options `variant=max`, `agent=build` |
| Environment inspected | T3 thread list/read/configuration/search; parent transcript; Git; handoffs; evidence directories; Unity lock records |
| Orchestration thread | `3296dda4-268e-4502-bd93-b4cd36d08731` |
| Date/time range | Worker activity `2026-10-29T01:57:40Z`–`11:20:38Z`; main parent run 5 `06:24:53Z`–`10:30:07Z`; retrospective run 8 from `14:37:15Z` |
| Child/worker threads | 28 delegated child threads: 18 completed, 10 failed |
| Runtime tasks/attempts | 28 delegated task threads; operator completions are parent-thread actions, not separate Runtime tasks |
| Repositories/worktrees | CCF bare store; CCF-Primary; orchestration and integration worktrees; worker worktrees for Rack, parity, S1-C/S1-D, MFM |
| Material routing decisions reconstructed | 18 |
| Backed by direct capacity evidence | 7 (Codex 6:38, OpenCode limit, Claude 7:00/7:10, Codex 11:40, and the Sol-after-reset choices) |
| Relying on inference/unknown capacity | 8; 3 launches were contrary to a known reset (RACK-V2-13, MFM-05 Opus, RACK-V2-12 Claude A; plus 3 resume attempts in the same premature window) |
| Worker outcome counts | 18 completed / 10 failed; 9/10 failures salvaged; 6 reviews (1 blocking); 5 completed threads static-only or pending Unity |
| Retrospective artifact | `Docs/Handoffs/PlayerTestReadiness-Overnight-01/RETROSPECTIVE-ROUTING-20261029.md` |
| Evidence not recovered | Exact remaining quota/percentage; OpenCode reset time; some failed-thread error items; operator task IDs; exact question-answer timestamps; account/pool distinction |
| Privacy/redaction | No credentials, tokens, pairing URLs, secrets or unnecessary account identifiers; provider/account aliases only |
| Repo cleanliness | One docs file added; no product code, routing, provider, task, branch or historical record changed; no Windows build |
| Project/runtime state changed | No product/runtime state changed; docs-only retrospective file |
| Proposed next action | Manager review; adopt the record split; add capacity reset preflight; require cross-provider independence for high-risk reviews; make operator attempts first-class; do not implement ARM changes |
