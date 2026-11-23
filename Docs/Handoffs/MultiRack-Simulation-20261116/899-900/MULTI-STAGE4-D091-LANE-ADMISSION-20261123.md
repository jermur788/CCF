# MULTI Stage 4 — D-091 admission for the adult parity lane

**Verdict: ADMISSION PASS — one exclusive source lane, one writer.**
**IMPLEMENTED: NO. VERIFIED: NO. OWNER ACCEPTED: NO.** Custody only.

| Item | Value |
|---|---|
| Issued | 2026-11-23T08:12:11Z (observed clock) |
| Issuing actor | opencode-go / DeepSeek V4.1 Flash — orchestration and custody only |
| Authority | Owner Stage 4 dispatch; D-095; approved Stage 4 quartet at `d27d770d` |
| Base | `4e60c8ab806df56441497b682ccf783badc6e0ea` (Stage 1–3; not merged) |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4-AdultParity-20261123` |
| Branch | `feat/multi-stage4-adult-parity-20261123-v1` (new, from the base only) |
| Sole writer | one `claudeAgent / claude-opus-5-5` agent, `effort=medium` |
| Limit fallback | `codex_3 / gpt-6.1-sol`, `reasoningEffort=medium`, only after a real provider limit, resuming the committed checkpoint |
| Reviewer | separate provider/context (GPT-6.1 Sol), read-only, after the candidate exists |
| Lease | open-ended under D-091; no fabricated expiry |
| Unity | not required, not authorised |

## 1. Live checks before admission

| Check | Observation |
|---|---|
| Volume | `/dev/sdd` on `/`, 1007 G total, **894 G available** |
| Lane path | absent before creation; created by this receipt |
| Branch | new; no remote copy |
| Base | `4e60c8ab…` present, `main` does **not** contain it |
| New worktree | HEAD `4e60c8ab…`, `git status --porcelain` 0 lines |
| Toolchain | `/home/jer/.dotnet/dotnet` 8.0.134; system `/usr/bin/dotnet` is 10.0.115 and must not be used |
| P0 reference | copied from the Stage 1–3 lane; SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, matches the csproj pin |
| Baseline | Core tests **160/160 passed**; Ui builds **0 warnings, 0 errors** on the unchanged base |
| Live listeners (protected) | PIDs 839989, 1750123 (#877), 1806361 (#882) — untouched |
| Competing writer | Stage 1–3 lane closed at `4e60c8ab`; no writer on the new branch |
| Coordination | `origin/main` has no Stage 4 registration; this receipt is the lane record |

## 2. Physical grant

| Resource | Granted |
|---|---|
| Writable source | `Tools/MultiRackSitkaP1Lab/src/Core/**` |
| Writable tests | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/**` and, if needed, a new `tests/**` shell script |
| Writable document | one new Markdown handoff under `Docs/Handoffs/MultiRack-Simulation-20261116/899-900/` |
| Generated output | this lane's `.p0ref/`, `src/**/bin|obj`, `tests/**/bin|obj` |
| Build | `/home/jer/.dotnet/dotnet` 8.0.134; do **not** run `build-p0-core-ref.sh` (8.0.134 produces `bcbdbf05…` and breaks the pin) |
| Optional sanity | build `SitkaP1.Ui` only; no server launch is required for this lane |

## 3. Read-only / forbidden

- **Read-only:** `src/Ui/**` (including `PlayerMultiApi.cs`), `Assets/**`, fixtures/goldens, `.sln`, `global.json`, build scripts, P0, saves, `main`, the Stage 1–3 branch, every other worktree and every live listener.
- **Forbidden:** Unity/Editor; Compact30/Legacy40/Enlarged80/Reference Future v1; save schema; `/multi` layout/SVG/CSS/overlay; packages; deleting files; merge to `main`; inventing decay, habitat, timber grade or economy; changing model 0 output; editing `src/Ui` to make Core compile (stop and report).

## 4. Stop conditions

Missing/second writer · protected path needed · Unity or UI change needed · model 0 hash or P0 pin drift · public contract break that forces a UI edit · build fails with the pinned P0 · owner cancellation.

## 5. Non-claims

No implementation, qualification, review or acceptance is claimed. The baseline run proves the unchanged base compiles and its 160 tests pass.
