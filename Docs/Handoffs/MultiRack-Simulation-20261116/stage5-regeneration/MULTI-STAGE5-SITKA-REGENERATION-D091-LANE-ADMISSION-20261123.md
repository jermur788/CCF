# MULTI Stage 5 Sitka regeneration — D-091 lane admission

**Verdict: ADMISSION PASS — one exclusive source lane, one writer.**
**This receipt records custody only. It claims no implementation, verification or owner acceptance.** The implementation outcome is in `MULTI-STAGE5-SITKA-REGENERATION-IMPLEMENTATION-20261123.md` next to this file.

| Item | Value |
|---|---|
| Recorded | 2026-11-23 (UTC); lane observed clean at 20:23:49Z |
| Lane provisioned by | the dispatching coordinator (task packet "CCF Multi-Rack / 2D Sitka Engine — STAGE 5 IMPLEMENTATION") |
| Receipt written by | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code. The sole implementation writer |
| Authority | Stage 5 spec quartet `specs/multi-stage5-sitka-regeneration-v1/`, promoted to **APPROVED** (owner approval 2026-11-23) on `docs/multi-stage5-sitka-regeneration-spec-20261123-v1` @ `85d85a8d9bdbdf3ae867a3225597c5f084d77773`; D-091 custody |
| Code base | `e7c1a57da519ed1ee03ccfbad240196a66df0905` (`feat/multi-stage4-ui-catchup-20261123-v1`). The owner chose this base; `main` does not contain it |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-20261123` (WSL ext4) |
| Branch | `feat/multi-stage5-sitka-regeneration-20261123-v1`, created at `e7c1a57d`. It had no remote copy at admission |
| Sole writer | one Claude Opus 5.5 session. It spawned no subagents |
| Previous attempt | It failed at a provider gate on another provider and produced no commits, branch, files or remote ref |
| Unity | not required, not authorised, not touched |

## 1. Live checks before work

| Check | Observation |
|---|---|
| Lane state | `git status --short` returned 0 lines through WSL git. `HEAD` was `e7c1a57da519ed1ee03ccfbad240196a66df0905` on the named branch |
| Remote branch | `git ls-remote --heads origin feat/multi-stage5-sitka-regeneration-20261123-v1` returned no ref |
| `origin/main` | `37ceb264b5431fe254d67e5702af39bf66cddc1d` |
| Live coordination | `Docs/Project/coordination/active-tasks.md` on `origin/main` is blob `cf44a0750ddacc94d6592f4ab4c39ed137b2dafd` (last changed by `37ceb264`, 2026-11-23T18:23:42Z). It was fetched at 2026-11-23T20:23:49Z and read again at 20:57:27Z with no change. It lists no other writer on `Tools/MultiRackSitkaP1Lab/src/Core/**`. The active Owner UX lanes are UI-only and explicitly exclude `src/Core/**` |
| Volume | `/dev/sdd` on `/`, 1007 G total, 890 G available |
| Toolchain | `/home/jer/.dotnet/dotnet` 8.0.134. `global.json` pins 8.0.133 with `latestPatch`. The system `dotnet` 10.x and Windows `git.exe` were not used |
| P0 reference | Copied from `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123/Tools/MultiRackSitkaP1Lab/.p0ref`. SHA-256 is `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, which matches `P0CoreExpectedSha256` in `src/Core/SitkaP1.Core.csproj`. `.p0ref/` is ignored by `Tools/MultiRackSitkaP1Lab/.gitignore:8` |
| Protected listeners | `127.0.0.1:33767`, `:43649` and `:46593` were all listening, observed with `ss -ltn`. None were touched |
| Parent checkout | `/home/jer/Development/CCF-HP` (unrelated user art edits) was neither read for writing nor modified |

## 2. Physical grant

| Resource | Granted |
|---|---|
| Writable source | `Tools/MultiRackSitkaP1Lab/src/Core/**`, with new files under `src/Core/MultiArea/` |
| Writable tests | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/**` |
| Writable spec | `specs/multi-stage5-sitka-regeneration-v1/**`, for the approval promotion only |
| Writable documents | `Docs/Handoffs/MultiRack-Simulation-20261116/stage5-regeneration/**` |
| Temporary docs worktree | `/tmp/stage5-docs` at the docs branch. It was created, used for the approval commit, then removed with `git worktree remove` (no `--force`) |
| Build | `/home/jer/.dotnet/dotnet` 8.0.134 with `TMPDIR=/tmp/stage5-tmp`. `build-p0-core-ref.sh` must not be run |

## 3. Read-only or forbidden

- **Read-only:** `src/Ui/**` (including `PlayerMultiApi.cs`, `PlayerApi.cs`, `Program.cs` and `wwwroot`), `.sln`, `global.json`, `Directory.Build.props`, fixtures, `.p0ref` contents (copy-in only), every other worktree or branch, `main`, and the Unity sources at `5e79e986` (read with `git show` only).
- **Forbidden:**
  - Unity and its assets: `Assets/**`, `Packages/**`, `ProjectSettings/**`, `*.meta`, the Unity lock.
  - Frozen worlds and saves: Compact30/#886, Legacy40, Enlarged80, Reference Future v1, historical saves, save schemas.
  - Out-of-scope ecology: Oak/Beech, planting/shelters, economy, storms, graminoids, browse concealment, stem-form damage.
  - Repository operations: new packages, Git LFS, merge to `main`, PR, force push, tags, history rewrite, branch deletion, a second writer.

## 4. Stop conditions (from the task packet)

- A read-only Unity check contradicts the approved spec in a way that changes product behaviour.
- An overlapping `src/Core` writer appears.
- A frozen hash or the P0 pin drifts at Regeneration 0.
- The work needs UI, Unity or save-schema changes.
- `S5-07` cannot reach tolerance without changing a locked owner decision.
- The `.p0ref` hash mismatches, or the build is not 0/0.

**Outcome:** the `S5-07` condition was triggered during qualification. The implementation handoff reports it. No other stop condition was triggered.
