# Orchestration routing ledger — 2026-11-05 (append-only)

Labels: **OBSERVED FACT** (directly read/measured this run), **ORCHESTRATOR CLAIM** (my decision/assertion), **RETROSPECTIVE INFERENCE** (reasoned, not measured). Quota, context size, reset times, tokens, runtime and cost are **UNKNOWN** unless stated. Note: the V2 evidence appendix (`ORCHESTRATION-EVIDENCE-APPENDIX-V2.md`) was **not retrievable** at `Docs/Handoffs/PlayerTestReadiness-Overnight-01/` on main (HTTP 404); this ledger follows the label scheme in the resume packet only.

## E-001 — Orchestrator identity
- OBSERVED FACT: the running session is **Claude Sonnet 5.5, effort medium** (provider instance `claudeAgent_claude_b_pro`, T3 Code), not DeepSeek High as the dispatch requested. DeepSeek is not asserted to be running; the packet itself says its release is not evidence a DeepSeek session exists.
- ORCHESTRATOR CLAIM: acting as orchestrator-only under #203/#201/#91 at the user's direct instruction; no implementation, tests, Unity or acceptance performed.

## E-002 — Writer / ownership check
- OBSERVED FACT: branch `docs/deepseek-orchestration-20261105-v1` HEAD at start `e0cf821d2b129d944bff51d02eff98a6276ceb49`; `Docs/Handoffs/Orchestration-20261105/` did not exist on it (no prior ledger). Main HEAD `83d3a2041bddb58ddab12ddb00c17a1a8a2dd98c` (2026-11-05T18:11:38Z).
- OBSERVED FACT: latest #203 comments are from 2026-11-04 (DPC overnight + Manager handback); no 5 November orchestrator comment. Live active-tasks (read from main) lists no running orchestrator; #200 is sole writer.
- ORCHESTRATOR CLAIM: no concurrent orchestration writer found; this is the only ledger writer.

## E-003 — Live queue (from active-tasks, #292/#282 issues)
- OBSERVED FACT: #292 READY/NOT RUNNING; #282 SPEC ONLY READY/NOT RUNNING; #291 OPEN non-blocking; #287 WAITING (no user playtest release); #289 closed (local generation only).
- OBSERVED FACT: ZX20 guard repaired (#290); Unity lock/Unity not touched by me.

## E-004 — #292 capability validation (gate before routing)
- OBSERVED FACT: in this session the Google Drive connector's `update_file` accepts only metadata fields (title, description, parentId, star, readOnly, restrictions, etag); it has **no `file_uri` or content parameter**. Connector tools available: search, metadata, read, download, create, copy, update (metadata), share, trash.
- OBSERVED FACT: the #289 worker (same connector family) reached the same conclusion earlier today.
- RETROSPECTIVE INFERENCE: a Claude child on provider `claudeAgent*` would receive the same connector surface; a Codex child is unlikely to have the claude.ai Drive connector at all. Not tested by launching a child.
- ORCHESTRATOR CLAIM: no worker with a **validated** in-place content-upload route exists; per packet gate 2/5 and fallback, #292 is **not dispatched** and **not marked RUNNING**. No Drive file touched, no duplicate created.

## E-005 — Routing alternatives considered for #292
| Option | Assessment |
|---|---|
| Sol Medium child via connector | no upload path (E-004); would only return BLOCKED at cost |
| Sonnet Medium child via claude.ai connector | same surface as E-004 |
| Sonnet Medium child driving Drive web UI (Claude in Chrome / computer use) | possible capability, but operates the user's signed-in Drive and 13 uploads on their behalf; needs explicit user go-ahead; capability UNVERIFIED |
| User manual: Drive "Manage versions → Upload new version" x13 + attachment swap | works today; user effort |
| Opus 5.5 | no justification (not a lock/disk/quota issue); not considered further |

## E-006 — #282 (spec repair)
- ORCHESTRATOR CLAIM: remains READY; routing (docs-only Sol Medium author, later Sonnet Medium review) deferred this run because the current user instruction prioritises #292 and does not restate #282; awaiting go-ahead or Manager preference. No capacity measured (UNKNOWN).

## E-007 — Not done (by role / by rule)
No code, tests, Unity, Windows build, playtest checkout access, active-tasks edit, self-acceptance, or merge. #287 stays WAITING; #194 NO ACTION; #291 non-blocking, unrouted.

---

# E-008..E-016 — overnight chain #294 run 2 (2026-11-06)

## E-008 — Orchestrator identity (this run)
- OBSERVED FACT: the running session is **DeepSeek V4.1 Flash** (`opencode-go`, model `deepseek-v4.1-flash`) in T3 Code, full-access runtime, acting as **orchestrator only** under #294/#203. This differs from E-001, which recorded an earlier Claude Sonnet 5.5 orchestrator session; both are true for their own runs.
- OBSERVED FACT: Manager #200 is a separate GitHub actor; no Manager runtime is reachable from this session. Manager verdicts can only be requested, not produced, here.

## E-009 — Live authority read at run start
- OBSERVED FACT: main `1dd9650dca066cc87221a71fac1ab56803f680a2` (2026-11-06T00:29Z); binding amendment `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/OVERNIGHT-SEQUENTIAL-EXECUTION-AMENDMENT.md`; #294 OPEN with Manager update 00:26Z recording user playtest finished; active-tasks reconciled 2026-11-06.
- OBSERVED FACT: #282 showed the 00:15Z [HANDED-OFF] DRAFT record at PR #265 head `aade967d`; no A2 review branch existed before this run.

## E-010 — Stage A (already complete, verified)
- OBSERVED FACT: PR #265 head `aade967d937623b51991a2fd65444bbde5cc03f9`; five authorised paths only; DRAFT; handoff present. Stage A's gate (tracked DRAFT, current-source citations, no wrong geometry) was satisfied by the prior run; not re-authored here.

## E-011 — Stage A2 independent review (dispatch 1)
- OBSERVED FACT: created docs-only review lane `docs/overnight-guidance-review-20261106-v1` at `/media/jer/Files/CCF/Operations/CCF-Overnight-GuidanceReview-20261106`, from main `1dd9650d`; no other worktree on that branch.
- OBSERVED FACT: reviewer provider `claudeAgent` (Claude A), model `claude-sonnet-5-5`, effort `medium`; completed. Commit `9f6d4c97be48731aa37995a05fdbce0c303930c3`, review file `OBJECTIVE-GUIDANCE-INDEPENDENT-SPEC-REVIEW.md`.
- OBSERVED FACT: verdict **APPROVE WITH CHANGES**, 2 blocking (B1 undefined "Review rack guidance" control; B2 missing Consent∥Reset safe selection order), 7 non-blocking (N1–N7). No Manager approval claimed.

## E-012 — Stage A2 repair pass
- OBSERVED FACT: same author provider instance `codex_c75d8114-7118-4224-8bf2-b9b62637e1fb` (ChatGPT — OpenAI B), `gpt-6.1-sol`, `medium`; completed. Branch head `e5fe1761744d880798fd7cc685362f1e28b7c60b`.
- OBSERVED FACT: B1 resolved by decision (b) — visible reason + no eligible primary, no invented control; B2 adds no-default/no-restored focus, safe-first traversal (`map-consent-decline` then `map-edit-reset-cancel`), no destructive focus restoration, native-input AC; N1–N6 fixed in text; N7 retained. Five authorised files only; `git diff --check` clean; status DRAFT.

## E-013 — Stage A2 re-review (dispatch 2)
- OBSERVED FACT: fresh reviewer session, same provider/model/effort; commit `4d89e02d0c75adb82864408083a14b45c4c0c420`, file `OBJECTIVE-GUIDANCE-INDEPENDENT-SPEC-REVIEW-2.md`.
- OBSERVED FACT: verdict **PASS WITH NON-BLOCKING NOTES**, **0 blocking**; B1/B2 resolved; no regressions; three minor wording observations (O1–O3); explicitly not Manager approval.

## E-014 — Read-only later-stage preflight (no writes, no Editor)
- OBSERVED FACT: Unity lock status via `python3 scripts/unity-lock status` = `free`, metadata `None`, `unity_editor_pids: []`.
- OBSERVED FACT: ZX20 `/dev/sdc2` 32G available (94% used), above the 15 GiB floor; Files `/dev/sdb4` 88G available.
- OBSERVED FACT: Stage C lane `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` exists, clean, branch `feature/scenario-one-workbench-retirement-20261105`, HEAD `8b1946dcec00587971d1f7c80004a47ebaa88515` == origin.
- ORCHESTRATOR CLAIM: these facts support a later Stage C preflight but do **not** constitute a lease; no Unity process, Editor, Player or build was started.

## E-015 — Gate state at end of this run
- OBSERVED FACT: spec still DRAFT; no Manager G4/G5 verdict exists on #282/#294; Stage B/D lanes are not registered in active-tasks and were **not** created.
- ORCHESTRATOR CLAIM: the chain stops at the **Manager spec-approval gate (A2)**. Restart requires (1) Manager verdict on the exact head `e5fe1761`, (2) Q1 decision (scoped F6/Shift+F6 vs mouse-only first slice), (3) #200 registration of the Stage B lane/branch. No bypass was attempted.

## E-016 — Not done (by role / by rule)
No spec authoring, code, tests, Unity, Editor, Player, Windows build, save access, active-tasks edit, merge, approval or self-review. No Stage B implementation and no Stage C qualification were started. Ledger branch is append-only in this run.
