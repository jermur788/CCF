# Claude A routing and orchestration evidence ledger — research-learning V1

**Status:** TEMPLATE / NOT STARTED. **Owner:** Claude A, one-writer, append-only. **Claim:** https://github.com/jermur788/CCF/issues/390. **V2 contract:** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/PlayerTestReadiness-Overnight-01/ORCHESTRATION-EVIDENCE-APPENDIX-V2.md.

**Do not backfill.** An entry below is a template, not observed provider capacity, execution, a PASS or an integration result. Observations must be recorded contemporaneously as OBSERVED FACT; model-stated rationales as ORCHESTRATOR CLAIM; later interpretation as RETROSPECTIVE INFERENCE. UNKNOWN is permitted and preferred to guessed usage/quota/reset/energy.

## Starting constraints (user-provided 2026-11-12, not runtime measurements)

- Target: implement complete *approved copy-only* Research-Informed Management Learning V1 against latest UI-improved candidate from #382, preserving its worktree and old user playtest state.
- Claude A / Fable 5.2 orchestrator; GPT-6.1 Sol Medium preferred implementation through authorised Sol account pools; Claude B Sonnet 5.5 independent review, Opus only as justified authorised exception.
- Product authority and active-tasks writer: Manager #200. No overriding locks, safety, user playtest, old-save/frozen Reference or Windows build gate.
- All account quotas, reset windows, context pressure, available tool capabilities, current Unity leases and latest remote refs: **UNKNOWN until personally observed at start**.

## Required starting observation (fill at actual launch)

- Start timestamp / timezone: UNKNOWN
- Actual Claude A provider/model/effort: UNKNOWN
- Orchestrator branch/starting HEAD; one-writer proof: UNKNOWN
- Exact newest UI source candidate and registered Unity Editor lane; clean/owner proof: UNKNOWN
- Current active-tasks/main/issue status: UNKNOWN
- Capacity table per pool (Claude A; Sol account 1; Sol account 2; Claude B): state AVAILABLE/BUSY/COOLDOWN/LOGIN-UNAVAILABLE/UNKNOWN, quota source/confidence/reset, capacity-to-completion, switching costs
- Context limit/used/compactions: UNKNOWN
- Optimisation: correctness and full completion, then minimised rework, tokens/cost/time; no test weakening.
- Prepared child claims/locks and whether #382 user lane is free: UNKNOWN

## Append-only event template

### EVENT — <actual timestamp>

- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE
- **Task / parent / why now:** 
- **Actual provider, model, effort, account pool / capacity and reset source:**
- **Alternatives considered, selected route, risk, exploit/explore and justification:**
- **Context pressure/compaction / switching, retry or checkpoint cost:**
- **Claim [READY]/[RUNNING]/[BLOCKED] / branch / source HEAD / exact worktree / locked file scope / Unity lease:**
- **Inputs and approved product/source boundary:**
- **Outputs, implementation/verification, actual command/test/screenshot evidence, PASS/FAIL/NOT RUN:**
- **Handoff/report URLs and status, accepted/rejected/pending:**
- **Measured calls/tokens/wall time/resources if available; otherwise UNKNOWN:**
- **Failure class, rework, salvage and next action:**

## End-of-run summary (create only from logged events)

Count work attempted/completed/rejected/blocked, accounted actual model+pool usage and switches, token/time where measured, test failures and owner gates, independent review, exact integration and protected-identity evidence, user-required action, dashboard source update and downstream sync/readback separately. Never infer Unity PASS, accepted spec or completed work from an issue title.

---

# LIVE ENTRIES (appended by Claude A from actual start; nothing above this line is an observation)

## Starting observation — 2026-11-12 01:30 UTC

- **Evidence class:** OBSERVED FACT unless marked.
- **Start timestamp:** 2026-11-12T01:30:46+00:00 (host clock, UTC).
- **Actual Claude A provider/model/effort:** Anthropic `claude-fable-5-2`, effort `low` (as reported by the harness), Claude Code inside T3 Code, provider instance "Claude A".
- **Orchestrator checkout:** `/media/jer/Files/CCF-Primary` on `ops/workflow-foundation-v1` @ `46247486`, with two pre-existing uncommitted user edits (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) left untouched. Ledger/report writer worktree created: `/media/jer/Files/CCF/Operations/CCF-390-Orchestration-Docs` on `docs/claude-a-fable-orchestration-research-copy-20261112-v1` @ `95eb5639`. (The handoff expected "no local orchestration worktree"; one was created because the ledger must be committed from somewhere. One writer: Claude A.)
- **Newest UI candidate:** `integration/scenario-one-personal-playtest-candidate-20261111-v1` local = remote = `b62446caf40fc2ac939d55a6bea6e201f41de700`; game source `2aaacad68ed186dbe14531f31ce5bb3a87811441`; `git diff 2aaacad6 b62446ca -- Assets Packages ProjectSettings` empty. Lineage contains #377 `c9c7d010`, #378 `ebe061ba`, #379 `2aaacad6` (post-#373 Sol repairs). Only other remote branches containing `2aaacad6` are the #379 fix branch and `qualify/scenario-one-ui-residual-tier2-20261111-v1` (= `2aaacad6`, no successor commits). No later UI successor found.
- **Registered Unity lane:** `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` on `qualify/scenario-one-tagged-text-379-20261111` @ `2aaacad6`, `git status` clean. Not opened, not modified. No process matching "Unity" running. ZX20 mounted, 25 GiB free. User #382 playtest: issue says USER NOT STARTED; whether the user intends to use the lane imminently is UNKNOWN.
- **main / active-tasks:** `origin/main` @ `20b8c81b`; no row naming #390 as running, no competing writer for the UI copy files. active-tasks.md NOT edited by Claude A (Manager #200 sole writer).
- **Capacity table:** Claude A — AVAILABLE, quota/reset UNKNOWN. Sol pool A (T3 provider "Codex", `gpt-6.1-sol`) — listed AVAILABLE by the harness, quota UNKNOWN. Sol pool B ("ChatGPT - OpenAI B", `gpt-6.1-sol`) — listed AVAILABLE, quota UNKNOWN. Claude B ("Claude B — Pro", `claude-sonnet-5-5`, `claude-opus-5-5`) — listed AVAILABLE, quota UNKNOWN.
- **Context limit/used/compactions:** UNKNOWN / UNKNOWN / 0 so far.
- **Optimisation:** correctness and full completion first, then minimal rework and cost; no test weakening.

## EVENT 1 — 2026-11-12 01:31 UTC — child claim and source lane

- **Evidence class:** OBSERVED FACT.
- **Task:** create C-01 child claim and isolated writer lane (handoff step 2).
- **Outputs:** issue [#391](https://github.com/jermur788/CCF/issues/391) [RUNNING]; worktree `/media/jer/Files/CCF/Operations/CCF-S1-ResearchCopy-V1-Sol`, branch `feature/scenario-one-research-copy-v1-20261112` @ `b62446ca`, clean. Allowed file set and protected list are in #391. #390 retitled [RUNNING] with a preflight comment.
- **Unity lease:** none taken.
- **ORCHESTRATOR CLAIM (source pre-inventory, read-only grep by Claude A):** "Areas with young trees" already ships in `ScenarioOneUiFacts.cs`; residual candidates are `AnnualReviewView.cs` "regeneration cohort(s) removed", curriculum "tallest natural seedling cohort", curriculum topic 2 "Being smaller or suppressed is not by itself a reason to remove a tree", and no existing wind-not-simulated Help sentence. `ManagementHistoryText.cs` parses lines by the prefix "Areas with young regeneration", so those history strings were placed out of bounds.

## EVENT 2 — 2026-11-12 01:33 UTC — implementation dispatch

- **Evidence class:** OBSERVED FACT (dispatch); ORCHESTRATOR CLAIM (rationale).
- **Actual provider/model/effort/pool:** OpenAI `gpt-6.1-sol`, reasoningEffort `medium`, Sol pool A (T3 provider "Codex"), default service tier. Executed model as reported by the provider: recorded at completion.
- **Alternatives considered:** pool B (identical model; held in reserve for a quota/cooldown on A); Claude A self-implementing (rejected: role separation, and it would leave no independent author for Claude B to review against). Exploit, not explore: LOW-difficulty task on the default route.
- **Claim / lane:** #391, single writer, worktree and branch as in Event 1. Unity lease: none; Unity forbidden in the packet.
- **Inputs:** the four-part approved spec on this branch; packet text is in the T3 delegated task and summarised in #391.
- **Outputs:** pending.
- **Measured calls/tokens/wall time:** UNKNOWN.

## EVENT 3 — 2026-11-12 01:37 UTC — Sol implementation returned

- **Evidence class:** OBSERVED FACT for commits/diff; author results are AUTHOR CLAIMS until reviewed.
- **Provider/model:** `gpt-6.1-sol` medium, pool A. No quota message, retry, restart or pool switch observed. Tokens/wall time UNKNOWN.
- **Outputs:** source `02cd3ec9084a0327dddf61075fa5bb49f0907568`, report head `f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1`, pushed (ls-remote matches), worktree clean. [Author report](https://github.com/jermur788/CCF/blob/f32be4c5bd7711ab93d92f249f4f02b4f21a3bb1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-IMPLEMENTATION-RESULT.md).
- **Claude A diff inspection (word-diff):** 8 files: six allowed UI files, one verifier pin (`ScenarioOneTeachingCopyVerification.cs`), the report. No file under `Assets/ForestPrototype/ScenarioOne` changed. One non-literal change: `ScenarioOneUiFacts` adds a display `.Replace("Establish planted ", "Planting completed; species present: ")`.
- **Author claims:** presentation checks 157 PASS baseline and final, 5 mutants killed; offline compile 189 sources 0 errors; Rack help checker keeps pre-existing failures plus an expected scope-whitelist failure. Unity/screenshots/runtime hash NOT RUN.
- **ORCHESTRATOR CLAIM (questions routed to review, not pre-judged):** whether narrowing the "being small/suppressed" lines dropped a teaching point; whether the planting label reads naturally; whether the display Replace is consumed anywhere non-display.

## EVENT 4 — 2026-11-12 01:37 UTC — independent review dispatch

- **Evidence class:** OBSERVED FACT.
- **Provider/model/effort/pool:** Anthropic `claude-sonnet-5-5`, effort medium, provider "Claude B — Pro" (separate account from Claude A). Opus not used; no justification for escalation at LOW difficulty.
- **Claim:** [#392](https://github.com/jermur788/CCF/issues/392) [RUNNING]; harness-isolated checkout; report branch `review/scenario-one-research-copy-v1-claude-b-20261112`. Read-only on source. Unity lease: none.
- **Outputs:** pending.

## EVENT 5 — 2026-11-12 01:41 UTC — Claude B verdict NEEDS CHANGES; rework dispatched

- **Evidence class:** OBSERVED FACT (verdict, push); reviewer findings are REVIEWER CLAIMS accepted by Claude A for rework.
- **Reviewer:** `claude-sonnet-5-5` medium, "Claude B — Pro". No quota event observed. Tokens/time UNKNOWN.
- **Review report:** [6038d85c](https://github.com/jermur788/CCF/blob/6038d85cac7bb6b85c1dde9bf3280a099ea959c8/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-INDEPENDENT-REVIEW.md) on `review/scenario-one-research-copy-v1-claude-b-20261112`.
- **Findings:** 1 major (stale pins in `Tools/Verification/WorkPlanResidualVerification.cs` for changed Work Plan strings, unreported by author); 3 minor (planting label double colon / "completed" on an open line; raw displayName shown in Annual Review "Objective first reached"; a removed Help sentence the spec did not ask to remove); notes (removal count wording, term used before introduced, optional sentence omitted, RackV2HelpTeaching failures pre-existing on baseline). Scope, protected identity and truthfulness judged sound. Reviewer reran presentation checks 157/157; did not rerun the offline compile; Unity NOT RUN, AC-7 NOT VERIFIED.
- **Failure class:** incomplete author grep for pinned strings (verification debt), plus copy polish. Salvage: all work kept.
- **Route:** rework loop 1 to the same author route, `gpt-6.1-sol` medium pool A (context continuity is in the report, cheapest route; pool B still in reserve). Claude A did not fix findings itself (role separation in the #390 handoff).
- **Next:** delta review by Claude B of the rework, then Unity qualification if the lease is free.
