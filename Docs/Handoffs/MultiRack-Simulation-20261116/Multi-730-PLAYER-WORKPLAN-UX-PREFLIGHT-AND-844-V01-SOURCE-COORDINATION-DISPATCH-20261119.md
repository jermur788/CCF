# Multi #730 — Parallel playable 2D Work Plan UX preflight and Scenario One V-01 source coordination

**19 November 2026 | owner-directed next task | one manager-level handoff | NO 2D engine source edit | ONE AREA / Stage5 design approved**

## Why this task exists

Owner requests a useful parallel task while #841's **exact qualified source branch** is being published for direct Manager inspection; owner also requests that the Scenario One 3D Manager source-sharing issue be sorted out without allowing it to own or block unrelated 2D gameplay. This handoff has **two nonoverlapping outcomes**: (A) a real player-workflow UX/source adapter implementation-ready **design and acceptance packet**, not speculative source coding; (B) an explicit narrow source-owner decision from [current Scenario One Manager #844](https://github.com/jermur788/CCF/issues/844). The only cross-programme subject is two unchanged public C# files, not authority over Multi.

**Existing binding source/decisions:**
- [Approved Stage5 four-file design §1–7 (on #823/#837 contract branch)](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/spec.md), [plan](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/plan.md), [tasks](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/tasks.md), [26R+24X not-yet-fully-verified checklist](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md).
- [#841 independent Tier-2 G2 result](https://github.com/jermur788/CCF/blob/docs/multi-841-internal-g2-one-parent-result-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/841-orchestration/Multi-841-INTERNAL-G2-ACTUAL-FELL-WORKPLAN-YEAR1PLUS-IMPLEMENTATION-AND-INDEPENDENT-Q-RESULT-20261119.md), local-only commit `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6`, qualified 62/62 and 17 nonauthor probes / 2,113 checks reported, unchanged ten no-work digests.
- [#730 scoped Manager NEEDS CHANGES ruling](https://github.com/jermur788/CCF/blob/docs/multi-730-841-manager-qualified-engine-verdict-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-841-G2-IMPLEMENTATION-QUALIFICATION-MANAGER-VERDICT-20261119.md): §8 intended design aligned, exact source private, unresolved V-01 source-owner half, A-1 stale legacy `Advance(year, revision)` must be closed **before any UI caller/wiring**, after independent delta checks.
- [Exact source-publication transport packet](https://github.com/jermur788/CCF/blob/docs/multi-730-841-source-publication-review-packet-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-841-EXACT-QUALIFIED-SOURCE-ISOLATED-PUBLICATION-AND-REVIEW-HANDOFF-20261119.md), awaiting a real desktop custodian publication receipt (not RUNNING by default).
- [Original #844 V-01 question](https://github.com/jermur788/CCF/issues/844#issuecomment-7023892548); as inspected at handoff, there is **no source-owner answer** in #844. Outgoing #824 was asked but has not approved.
- #842 source and #843 forestry review: G1 positive comparable targets NOT justified; #828 remains DRAFT, G1 DISABLED/Manual/NotCovered. D-093: 170mm informational only and no transaction may Fell the final live tree.

## Track A — Player-facing Stage5 2D UX implementation-ready packet (PARALLEL, DOCS ONLY)

**Class: SPEC-REQUIRED for actual new UI workflow; this activity is only approved-spec design elaboration and acceptance mapping, not implementation. Difficulty: MEDIUM. Suggested provider: GitHub/browser-capable low-cost UX/spec worker, with Manager #730 reviewing; distinct branch/docs-only writer.**

**Product outcome to make demonstrable on a future real source-connected 2D UI:** view the **one undivided** synthetic 30m Sitka area; inspect committed year/fixture/scope, real current exact tree IDs and available size/spatial/competition features and genuinely observed stand values; select real *living* IDs manually with reason per ID (optional evidence-backed local sample without geographic subdivision); deliberately add PendingWork, review whole batch, separately approve (zero Fell), separately Execute one real atomic Fell, display same-year typed event/count/BA and true Year+1 survivor outcomes. Unqualified sample cannot invent G1 matches; display Manual/NotCovered. No physical rack, corridor, automatic entire-stand Fell, fake economic prices, persistent history or Unity.

**Output:** one standalone **reviewable GitHub Markdown** with:
1. A source-current vs preview/diagnostic data-flow and minimal UI state model mapping exact approved Stage5 requirements (R-01...R-26, especially R-03/R-05/R-11...R-18/R-22) to *actual confirmed #841 API names when source ref arrives*, otherwise label `UNKNOWN pending source inspection` instead of inventing method signatures.
2. One screen-by-screen flow or text wireframes: current source/committed frame → per-tree inspect/selection/rationale → pending work → whole-plan approval confirmation → separate execute and all-or-nothing result → year advance with consent/stale-cancel warning → inspect survivors and recorded actual previous year.
3. UX negative/error states: wrong-case/dead/unknown/duplicate ID; exact stale source token/annual vs work revision; pre-Fell approval creates no change; all-live Fell atomic refusal; execution replay; no-work; missing source/read refusal; G1 DISABLED; A-4 already-felled vs typo; A-1 old advance-caller as **blocking** until fixed.
4. Accessibility/qualification matrix including keyboard, screen reader and touch, no colour-only semantics, 360px and 200% zoom; no fabricated test PASS, fixture, screenshot or native evidence.
5. Exact proposed later implementation **allowlist** for a single UI author (not an authorisation), possible interface boundary from published source, and a smallest genuine player demonstration. Do not create a second simulation engine; show biological output only after engine-authoritative transaction.
6. Owner-ready open choices **only if materially unresolved**; do not reopen D-093 or one-area/rack decisions.

**Allowed writes now:** one new GitHub docs-only artifact under `Docs/Handoffs/MultiRack-Simulation-20261116/` on a fresh branch. **Not allowed:** writing C#, TS, Node UI, tests, fixtures, `specs/**` status, original #805, `active-tasks.md` without its single owner, main; running .NET/Unity/build; accessing internal private #841 lane or requiring external/ZX20. This preflight should proceed even if V-01 is unresolved; an actual source-connected UI writer remains gated.

**Quality:** check GitHub published #841 source branch when available; if source not visible, document interface gaps rather than speculating. Manager reviews document against approved four-file design and independent source result. Do not claim complete Stage5 qualification because 26R/24X remain unverified.

## Track B — Resolve #844's narrowly scoped V-01 source-owner permission (CROSS-MANAGER / NO CODE)

Multi #730 is sole 2D feature Manager. Scenario One #844 controls source ownership for *two pre-existing 3D files* linked unchanged to 2D builds; it has **no veto over independent 2D UX planning and no authority to choose 2D mechanics**.

Request one explicit response in #844: `APPROVE IN PRINCIPLE`, `NEEDS CHANGES` with exact competing claim/technical risk, or `DENY`, for **read-only/compile-linked input use only** of:
- `Assets/ForestPrototype/SitkaGrowthModel.cs`, Git blob `e4c73402d46bb3b75a9cba2e4622857a913fac92`;
- `Assets/ForestPrototype/SimulationRandom.cs`, Git blob `bb83d7f8468002ab13a6aec6caf548e0bf3ad34b`.

Product owner reportedly gave their own half in the actual #841 desktop session; owner now explicitly requests resolving source coordination. We have not seen the literal in-run grant. Files remain owned by Scenario One; a #844 reply cannot retroactively authorise any #841 preapproval work, transfer file ownership, permit `Assets/**` edits, skip code review, unblock native builds/Unity or wipe historic V-01/V-02 variance. #844 manager should identify any real source-conflict, file contract or source-change hazard *once*; if none, grant prospective narrow unchanged reuse and avoid repeated permission reviews of the same hashes. If manager refuses or doesn't answer, continue Track A and stop only new source-dependent compile/use; escalate concrete residual issue to owner/#425 rather than leaving the whole 2D UI planning idle.

## Status, dispatch and completion gates

- **Both tracks are READY, not worker-RUNNING.** Publish actual claim/provider/branch and handoff when started; do not assert actors, worktrees or desktop availability.
- Track A can run independently in browser on GitHub and produce a strong UX/pre-implementation result before #841 source push. Track B is a request to an already-running independent manager; not a second 3D source worker.
- Track A acceptance: one GitHub docs-only packet, readback verified, no speculative APIs/changed source, concrete shortest playable route, 26R+24X evidence mapping, direct #730 handoff.
- Track B acceptance: written direct #844 reply, with identity and precise hashes/grant/exceptions linked in #730; no self-assumed approval.
- Later actual UI coding is separately scoped under **approved Stage5 specification, A-1 fix, source/physical permission, precise writable lane/branch and independent qualification**. Source publication to a review branch is for evaluation, NOT main integration or access grant.
- Preserve 100 exact original tree IDs, Year0–10 no-work digests, #804 PARK/main-BLOCKED, L4 FAIL38, V-01/V-02 historical variance, no Windows build before owner playtest approval, no ZX20.
- No Drive mirror or dashboard publication claimed.

## Paste-ready cross-chat handoff — Track A

> **Multi 2D #730 / Stage5 player-visible Work Plan UX preflight (DOCS ONLY, MEDIUM).** Full direct handoff: https://github.com/jermur788/CCF/blob/docs/multi-730-player-ui-and-v01-coordination-dispatch-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-PLAYER-WORKPLAN-UX-PREFLIGHT-AND-844-V01-SOURCE-COORDINATION-DISPATCH-20261119.md . Provider: one GitHub/browser UX/spec worker, not a C# writer; claim READY → RUNNING on #issue when actually starting; fresh `docs/multi-stage5-player-workplan-ux-preflight-20261119-v1` branch from observed main; sole allowed write one new docs handoff, no source/spec status/fixture/Unity/physical/main. Read approved four-file Stage5 design and #841 handoff, propose concrete accessible one-area inspect → per-ID reason → PendingWork → whole approval → separate atomic Execute → true Year+1 UI and test matrix; mark unpublished API names UNKNOWN until code branch can be inspected. G1 DISABLED, D-093 fixed; A-1 blocks new UI caller/wiring. Check live coordination/locks and existing issue before action. Verify docs branch one-file GitHub readback/compare and report result to #730. If actual source access, permission or concurrency is missing, continue docs-only design without inventing qualification. Independent Manager review follows. No need for owner action to prepare this packet.

## Paste-ready cross-chat handoff — Track B

> **Scenario One #844 source-owner coordination only; NOT 2D implementation review.** Same binding full packet: https://github.com/jermur788/CCF/blob/docs/multi-730-player-ui-and-v01-coordination-dispatch-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-PLAYER-WORKPLAN-UX-PREFLIGHT-AND-844-V01-SOURCE-COORDINATION-DISPATCH-20261119.md . Product owner requests resolution without blocking unrelated 2D. Please answer on #844 `APPROVE IN PRINCIPLE / NEEDS CHANGES / DENY` for only the two exactly pinned unchanged public `Assets/ForestPrototype` C# blobs used read/compile-only by standalone 2D. If conflict, state concrete technical/source ownership constraint, no generic audit. No 3D manager ownership of Multi mechanics, no Assets write/Unity/Windows/main or retroactive permission. Return a direct comment URL for #730; do not start a code worker.

**Owner action now:** none required to prepare or dispatch docs-only Track A; #844 manager response is separately required for source reuse, not for UX preflight.
