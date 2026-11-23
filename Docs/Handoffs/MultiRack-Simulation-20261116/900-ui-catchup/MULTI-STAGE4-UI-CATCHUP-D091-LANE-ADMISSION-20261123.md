# MULTI Stage 1–4 `/multi` UI catch-up — D-091 lane admission

**Verdict: ADMISSION PASS — one exclusive source lane, one writer.**
**IMPLEMENTED: NO. VERIFIED: NO. OWNER ACCEPTED: NO.** Custody only.

| Item | Value |
|---|---|
| Issued | 2026-11-23T09:28:50+00:00 (observed clock) |
| Issuing actor | opencode-go / DeepSeek V4.1 Flash 1017 — orchestration and custody only |
| Authority | Owner packet "CCF 2D / MULTI SIMULATOR — STAGES 1–4 UI CATCH-UP HANDOFF", 2026-11-23 (owner product decisions §2, locked); D-095 Stage 4; D-091 custody |
| Code base | `500a50aea4bd049f231f05d73828fabdc0a7a85c` (`feat/multi-stage4-adult-parity-20261123-v1`, Stage 4 APPROVED focused G3 by `3238846d`, not merged) |
| UI base | `1e4e02215d373e224fe6a74074ae3f62983db242` (#885 Astra/Opus UI) — **verified ancestor**: `git merge-base` of the two branches equals the #885 head, so no merge or reconciliation commit is required |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123` |
| Branch | `feat/multi-stage4-ui-catchup-20261123-v1` (new, from `500a50ae` only; no remote copy) |
| Sole writer | one `opencode / claude-opus-5-5` agent, `variant=high` |
| Limit fallback | **none authorised** — the owner named Opus 5.5 for implementation; a provider failure stops and reports |
| Reviewer | `opencode / gpt-6.1-sol`, `variant=high`, separate provider/context, read-only, after the candidate exists |
| Lease | open-ended under D-091; no fabricated expiry |
| Unity | not required, not authorised |

## 1. Live checks before admission

| Check | Observation |
|---|---|
| Volume | `/dev/sdd` on `/`, 1007 G total, **893 G available** |
| Lane path | absent before creation; created by this receipt |
| Branch | new; `git ls-remote --heads origin feat/multi-stage4-ui-catchup-20261123-v1` returned no ref |
| Base | `500a50ae` present locally; `main` does not contain it |
| New worktree | `git status --porcelain` 0 lines after creation |
| Toolchain | `/home/jer/.dotnet/dotnet` 8.0.134 (system `dotnet` 10.0.115 must not be used); `node` v26.13.1 via vite-plus |
| P0 reference | copied from the Stage 4 lane; SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; `.p0ref/` is git-ignored (`.gitignore:8`) |
| Live listeners (protected) | `:33767` pid 1750123, `:43649` pid 1806361, `:46593` pid 839989 — all listening and untouched at admission |
| Competing writer | none: no remote branch, Stage 4 lane closed and clean at `500a50ae`, `main` clean at `cbf3a618` |
| Browser | no Chromium/Firefox/Playwright install in WSL; Windows **Microsoft Edge** exists at `/mnt/c/Program Files (x86)/Microsoft/Edge/Application/msedge.exe` and is the only real browser available |

## 2. Physical grant

| Resource | Granted |
|---|---|
| Writable source | `Tools/MultiRackSitkaP1Lab/src/Core/**` (including `MultiArea/**`), `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` |
| Writable tests | `Tools/MultiRackSitkaP1Lab/tests/**` (C# suites and shell harnesses) |
| Writable spec | new `specs/multi-stage4-ui-catchup-v1/**` quartet, authored from the owner packet |
| Writable document | `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/**` |
| Generated output | this lane's `src/**/bin|obj`, `tests/**/bin|obj`, screenshots placed under the granted handoff directory |
| Build | `/home/jer/.dotnet/dotnet` 8.0.134; do **not** run `build-p0-core-ref.sh` |
| Server | `/multi` on an OS-assigned loopback port only, from this lane, stopped cleanly afterwards |

## 3. Read-only / forbidden

- **Read-only:** `src/Ui/PlayerApi.cs` (guards, capability routes, page policy — a change here is a stop condition), `src/Ui/Program.cs`, `Assets/**`, fixture binaries and goldens, `.sln`, `global.json`, `Directory.Build.props`, build scripts, `.p0ref`, saves, `main`, every other worktree/branch, every live listener.
- **Forbidden:** Unity/Editor; `Compact30` (#886), `Legacy40`, `Enlarged80`, `Reference Future v1`; any save-format change; a permanent 2D save file; Stage 5 content (seedlings, brambles, bracken, browsing), Oak/Beech, economy, storms; side-by-side untreated twin UI; forestry/canopy/light/pruning arithmetic in page JS or CSS; flipping the frozen `0/0/0` default of the Core continuation ctor; adding packages or a repo-level npm dependency; deleting files; merge to `main`; force push; `/s/<token>/` capability URLs in committed docs; second writer.

## 4. Stop conditions

Missing/second writer · a change needed outside the physical grant (especially `PlayerApi.cs`) · Unity or `Assets/` change needed · any frozen hash or P0 pin drift · public contract break that forces a UI edit outside the grant · build fails with the pinned P0 · real-browser verification impossible · owner cancellation.

## 5. Non-claims

No implementation, qualification, review or acceptance is claimed here. The base was not rebuilt by this receipt; the Stage 4 lane's recorded 180/180 and 0/0 builds are inherited evidence, not a fresh observation.
