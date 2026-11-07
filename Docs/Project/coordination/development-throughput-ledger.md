# Development throughput and critical-path learning ledger

**Owner:** Overall Manager #200. **Purpose:** append-only management evidence for understanding elapsed delivery time, recurring blockers, safe workflow improvements and measurable process changes. **Not** a source spec, a Unity task claim, a replacement for live `active-tasks.md`, or a claim that any worker has a real lease. Canonical Git remains implementation authority. The separate Mimo #313/#322 execution ledgers remain worker-owned and must not be rewritten by this Manager ledger.

**Conventions:**
- Each new retrospective gets a numbered **append-only entry**; never replace earlier evidence/metrics with new guesses. Corrections are later entries linked to the original.
- Tag figures **measured event-window**, **inferred critical-path contributor**, **worker-reported**, or **unmeasured**.
- Calendar elapsed, active worker/model runtime, Unity lease, Manager/product queue wait and review duration are separate; overlapping issue lifetimes must never be added as worker-hours.
- Avoid turning the record into extra approval bureaucracy. Lightweight timestamps/causes should come from claim lifecycle comments that workers already post.

---

## Entry 001 — 2026-11-07 08:48 UTC — post-playtest rack/Objective/map delivery expansion (5–7 November)

**Question:** Why did an apparently small rack update occupy approximately two calendar days of work before the next playtest was ready?

**Full source-backed Manager retrospective:** [Two-day rack update bottleneck audit](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Throughput-Audit-20261107/MANAGER-RACK-UPDATE-TWO-DAY-RETROSPECTIVE.md).

**Scope correction:** Rack V2 core integrated/conformance-passed on 29–30 October ([#99](https://github.com/jermur788/CCF/issues/99), [#185](https://github.com/jermur788/CCF/issues/185)); post-5-Nov playtest feedback [#281](https://github.com/jermur788/CCF/issues/281) expanded scope to Objective cues, Full Stand Map headland/feeder depiction, truthful five-rack diagram, hidden Workbench retirement/old saves and protection/verification. The apparent rack task was actually a cross-screen, cross-save, cross-verifier playtest-readiness continuation.

| Observed milestone | Calendar or runtime window | Confidence / interpretation |
|---|---|---|
| Feedback to current audit cut | 5 Nov 12:30 to 7 Nov 08:48 = **44h18m** | **Measured wall clock**, not continuous coding, and not total agent hours |
| Revised guidance spec from issue create to approval | 5 Nov 12:30 to 6 Nov 08:03 = **19h33m** | **Measured issue window**, includes stops for Manager/product approval; active writing/wait split **UNMEASURED** |
| Diagram/source+checker stage from task create to approval | 6 Nov 07:59 to 23:29 = **15h30m** | **Measured issue window**, overlaps other stages; multiple author/reviewer cycles, not one source writer |
| Unity-lock canonical-store recovery issue window | 6 Nov 23:52 to 7 Nov 00:15 = **22m** | **Measured operations claim window**, excludes earlier preflight and later review |
| Mimo actual overnight session | 7 Nov ~00:29 to ~07:30 = **7h01m** | **Worker-reported orchestration span**, exact [#313 ledger](https://github.com/jermur788/CCF/blob/docs/mimo-scenario-one-overnight-20261107-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MIMO-OVERNIGHT-EXECUTION-LEDGER.md) |
| Genuine Unity exclusive lease | 7 Nov 00:45:32–01:49:05 = **63.5m** | **Worker-reported exact lease timestamps**, no editor beyond lease |
| Actual cold Unity import/compile inside lease | 7 Nov 00:47:38–00:57:35 = **9.9m** | **Worker-reported exact run timestamps**, not the explanation for two days |
| Overnight CTA checker author/review/repair stage | 7 Nov ~03:20–07:10 = **3h50m** | **Observed stage window**, includes reviewer/provider reroutes, not pure coding hours |

**Top causes / evidence-backed:**
1. **Late user-visible scope and truth mismatch (HIGH):** map did not show existing 3D headland/feeder; diagram work initially STOPPED, product decision shifted deferred C to explicit B, #298 initial four blocking spec findings. [#281](https://github.com/jermur788/CCF/issues/281), [#294](https://github.com/jermur788/CCF/issues/294), [first source-fidelity stop](https://github.com/jermur788/CCF/blob/docs/deepseek-orchestration-20261105-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/OVERNIGHT-EXECUTION-RESULT.md).
2. **Verifier-framework review/fix spiral (HIGH):** five-rack source checker required #306→#307→#308→#310; Objective CTA checker had original 50 true false greens, 26 later cases and 57 residual interpolation/decoy cases, with more author/review cycles pending. [#307](https://github.com/jermur788/CCF/issues/307), [#305](https://github.com/jermur788/CCF/issues/305), [#314](https://github.com/jermur788/CCF/issues/314).
3. **Unity preflight and historic test debt (HIGH):** standalone clone not eligible for real machine lease until #311/#312 recovery, old save v23 verifier pin vs actual v24, false-negative lifecycle, runner false-greens and licence cleanup, a genuine unexplained 1-tree substitution, package 1.1.0→1.1.1 rewrites/dirty eight tracked files and insufficient disk for second cold Library. [#287](https://github.com/jermur788/CCF/issues/287), [#317](https://github.com/jermur788/CCF/issues/317), [#315 dossier](https://github.com/jermur788/CCF/blob/docs/mimo-scenario-one-overnight-20261107-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/FOCUSED-INTEGRATION-READINESS-DOSSIER.md).
4. **Serial product/Manager/reviewer handoffs (MEDIUM–HIGH elapsed; mixed necessary and improvable):** safe separation caught real defects, but choices/approval spread over multiple sessions extended critical path. No measured separate waiting-hour total.
5. **Quota/adjacent administrative work (SECONDARY):** two OpenAI access failures over ~10 minutes directly logged in #313; context-mirror/storage/research tasks also occurred but cannot each be attributed to this critical path.

**Decisions for future process experimentation (not game product changes):**
- Obtain target-vs-current visual acceptance and a source-to-render map of actual 3D/map/diagram representation **before** coding; bundle all consequential product questions at one gate.
- Preflight checkers/old versioned fixtures and actual legitimate Unity lane/disk/package cache **on clean parent source** before final qualification.
- Design checker architecture to match contract (C# semantic/AST analysis for member/interpolation semantics) or bound lexical claims; keep negative mutations, intentional red baselines and real native checks, never weaken tests to get green.
- Separate large verifier-platform redesign and context publication from narrowly defined playable UX delivery; retain one final exact-head Unity/integration gate.
- Preserve safety: exclusive Unity lock, canonical-store lane, no unilateral cleanup/overwrite, 15 GiB disk floor, frozen Reference Future, Legacy40/save compatibility, independent reviewer and final Manager verdict.
- Record real timing statuses on future work; do not infer hours of model coding from issue open-to-close.

**Suggested metrics to capture on each future task/child:** first actual start, author active end, reviewer start/end, Manager/product waiting windows, block reason and start/end, exact-hash candidate, protected tests applied, novel false-green count, number of review-repair loops, real Unity lease duration/import duration, disk/cache cost, source lines/files changed, separate final integration acceptance. **Missing in this audit:** exact active model-hours, per-task token cost, deliberate queue-wait hours, true exclusive critical-path attribution. Do not report percent time split absent those data.

**Status at audit:** #313 overnight publication complete; #318 source repair/#319 independent review underway or advancing under Mimo #322; #320/#321 later conditional; #315 integrated A–O all NOT RUN. The browser Manager did not touch active Mimo ledgers, feature source, Editor or CCF-Ops and does not claim local machine verification.

**Manager action:** use this ledger to assess later delivery-duration improvements against actual time/defect data. No new feature/product or Unity permission issued. See full retrospective for recommendations and caveats.

---

## Entry 002 — 2026-11-07 morning — proposal for a lightweight standard timing record (NOT YET AN EXECUTION MANDATE)

**User question:** Would more consistent task timing data allow the Overall Manager to explain delivery bottlenecks and compare ways of doing similar work faster? **Yes.** [Entry 001](#entry-001--2026-11-07-0848-utc--post-playtest-rackobjectivemap-delivery-expansion-57-november) could reconstruct calendar windows but could not infer actual active model time, waiting for Manager/product decisions, or exclusive critical-path costs. This is an operational data-capture **proposal**, not a new Unity/feature authorisation and not a retroactive edit to [#313 overnight ledger](https://github.com/jermur788/CCF/blob/docs/mimo-scenario-one-overnight-20261107-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MIMO-OVERNIGHT-EXECUTION-LEDGER.md) or the current [#322 ledger](https://github.com/jermur788/CCF/blob/docs/mimo-scenario-one-morning-closure-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MIMO-MORNING-CLOSURE-LEDGER.md).

### Minimal row at each meaningful task status change

| Field | Requirement / example | Why |
|---|---|---|
| `timestamp_utc` | ISO-8601 actual observed time, e.g. `2026-11-07T09:05:00Z` | Enables elapsed duration; avoid issue-comment publication timestamp as surrogate for actual work start unless explicitly labelled |
| `claim`, `stage`, `attempt` | `#320`, `author`, `1` | Distinguishes loops, independent reviews, reroutes and child tasks |
| `event` | `READY`, `RUNNING_START`, `RUNNING_END`, `WAITING_MANAGER_START`, `WAITING_PRODUCT_START`, `WAITING_PROVIDER_START`, `BLOCKED_START`, `RESUMED`, `REVIEW_START`, `REVIEW_END`, `HANDED_OFF`, `MANAGER_VERDICT`, `UNITY_LEASE_ACQUIRED`, `UNITY_LEASE_RELEASED` | Creates time windows instead of a static current status |
| `actor`, `provider/model/effort` | Actual worker instance, observed model and effort or `UNKNOWN` | Allows fair routing comparisons; no fabricated quota/reset data |
| `candidate`, `lane` | Exact Git ref, actual worktree and ownership where applicable | Prevents mixing work performed on different source versions |
| `cause_code` | One primary blocker: `PRODUCT`, `MANAGER`, `SOURCE_DEFECT`, `VERIFIER_DEFECT`, `UNITY_ENV`, `STORAGE`, `LOCK_WORKTREE`, `PROVIDER_QUOTA`, `REVIEW`, `OTHER`; `NONE` if unblocked | Makes recurring delays comparable while preserving the detailed free-text reason |
| `outcome`, `evidence_url` | `PASS/FAIL/NOT_RUN/BLOCKED`, GitHub result/log reference | Ensures no timing total is detached from proof |
| `clock_source` | `worker_receipt`, `tool_log`, `issue_created_at`, or `estimate` | Distinguishes measured execution windows from elapsed GitHub issue lifetime |

**Optional only when available:** actual subprocess command duration; `tool_wait_start/end`; token/cost measures when the provider exposes them; Unity cold-import duration and storage footprint; number of previously passing tests re-run; independent reviewer-new false greens; `scope_change` (initial approved scope, new approved task count, reason). Do not require a manual timesheet or per-token accounting from models without those measurements.

### Derivations and reporting rules

- **Wall-clock lead time:** first accepted request to exact accepted/qualified handoff (or report incomplete). This is not sum of issue lifetimes.
- **Active session elapsed:** paired `RUNNING_START/END` minus explicit pauses, reported as *session elapsed*, not precise model CPU time. If a worker does not expose internal execution and tool wait, mark them **UNMEASURED**.
- **Waiting per cause:** paired WAIT/BLOCK start and resume/end events, with overlapping events deduplicated when constructing the critical path. Unpaired time windows must be labelled incomplete rather than assumed continuous work.
- **Review cycles:** count actual independent review → defect → repair → re-review loops, and classify source/UI defect vs checker/fixture defect separately.
- **Unity:** calculate genuine lease-acquire to lease-release, Editor import command duration, actual test duration separately; never infer Unity runtime from open issue duration. Disk/lock safety unchanged.
- **Delivery report:** show a timeline, critical-path phases, total active session time (may overlap across independent workers), waiting reasons, review-loop count, final status and unmeasured intervals. **Never add overlapping intervals** or assert a precise percentage when timestamps are absent.

### Low-overhead adoption recommendation

1. **Pilot on the next two new features**, using the existing GitHub task claim start/stop/handoff comments and Mimo ledger append-only entries. Orchestrator automatically writes an event row when it changes a claim; subworkers provide actual event times in their result. Avoid changing active Mimo #322 instructions mid-run or historical entries.
2. Keep the canonical `active-tasks.md` as **single-writer ownership/lock authority**, not an event-history sink. Mimo owns its execution ledger; Manager owns this throughput/analysis ledger and the shared Overall-Dev summary. No competing writers.
3. After two pilots, compare whether the record can actually explain waiting vs execution, and whether additional fields justified their overhead. If successful, formalise the event vocabulary in orchestration instructions; if not, simplify rather than increase process burden.

**Status:** PROPOSED to Overall Manager, not yet mandated for current worker; no existing test, acceptance, ownership, lock or Unity policy is relaxed.

---
