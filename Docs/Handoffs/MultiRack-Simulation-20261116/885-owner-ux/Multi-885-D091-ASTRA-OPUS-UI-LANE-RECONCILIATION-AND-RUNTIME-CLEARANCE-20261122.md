# Multi #885 — D-091 custody reconciliation and bounded runtime clearance (Astra/Opus UI lane)

**RECONCILED — CLEARED for one bounded `/multi` run, one result-file commit and one branch push.** Custody only; the custodian ran no build, no server and no review.

Issued **2026-11-22T16:47Z** (observed clock `2026-11-22T16:44:24+00:00`). Amends `…-PREADMISSION-AMENDMENT-20261122.md` (`28abb53d`) by adopting the existing lane and recording deviations.

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-astra-reconcile-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-D091-ASTRA-OPUS-UI-LANE-RECONCILIATION-AND-RUNTIME-CLEARANCE-20261122.md

## 1. Independently verified lane state

| Check | Observed |
|---|---|
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-AstraOpusUI-20261122` present, registered **once** as a worktree |
| Branch | `feat/multi-885-astra-opus-ui-20261122-v1` |
| HEAD | **`3e6a6add63e3838f770170e2ba2974b4bc1fe298`** |
| Parent of HEAD | **`a692e1bf…` exactly** — the reservation base, no rebase, no re-authoring |
| Working tree | `status --porcelain --untracked-files=all` → **0 lines** |
| Diff vs baseline | **2 files only**: `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` (+2164/−708) and the permitted result Markdown (174 lines) |
| Protected paths in the diff | **none** — no `tests/`, no `csproj`/`props`, no `wwwroot`, no `Core/` |
| `.p0ref` | pin **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`**, mode `r--r--r--`, copied 16:36, **not rebuilt** |
| Build artefacts | `sitkap1-ui.dll` 16:36:35, `SitkaP1.Core.dll` 16:36:34 — **newer** than `PlayerMultiApi.cs` (10:20:30), so the artefact corresponds to the committed source |
| Build servers | **none** (`VBCSCompiler`/`MSBuild`/`dotnet build`) |
| Lane processes / listener | **none** |
| Protected lanes | `:46593` (PID 839989 OwnerUX), `:33767` (1750123), `:43649` (1806361) — all alive; `#867` 390578 absent |
| Evidence directory | `…/CCF-885-Astra-UI-Evidence-20261122` **absent** (not yet created) |
| Remote | feat branch **absent** (404); `main` still **`3a67d098`** |
| Board | live `active-tasks.md` fetched: **no competing claim** for this lane or branch |

## 2. Allowlist verified independently by the custodian

I extracted the documented `mask_literals_v1.py` from the committed result Markdown and ran it myself (read-only):

| Run | Result |
|---|---|
| baseline `a692e1bf:…PlayerMultiApi.cs` | `34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16` |
| candidate lane file | **same hash** — no byte outside the three literal interiors changed |
| deliberate mutation of a code token (sanity test of the guard) | **guard rejects it** |

So the "three literals only" constraint holds at custody level. This is **not** a design or quality review; Astra's review remains outstanding.

## 3. Deviations recorded (not glossed)

| # | Deviation | Disposition |
|---|---|---|
| D1 | `t3_worktree_handoff` **timed out** (git status over `\\wsl.localhost` on the dirty primary), so the lane was created with WSL `git worktree add -b … <reserved path> a692e1bf…` | **ACCEPTED.** Exact base, exact reserved path, branch/path pre-checked absent, one lane, clean, no other lane touched. The timeout is the same cross-view artefact documented in the amendment §7 |
| D2 | **No separate Opus child thread**: the Astra thread `1f1a1a27` was re-bound to `claude-opus-5-5` by direct owner instruction after the gpt-6-astra session hit its limit | **ACCEPTED as an identity amendment.** The reserved "child" is realised as that thread while bound to Opus 5.5. Single writer preserved; no second writer may appear. Any further write must come from that same thread |
| D3 | The committed result Markdown (10:23) states "COMPILED: NO / dotnet build **not run — not admitted**", but build artefacts and `.p0ref` are dated **16:36** | **The document is stale.** Observed mtimes place the build **after** the 09:32 amendment, i.e. within the amended authority; the narrative that the build preceded the amendment is **not supported by the observed mtimes** and is not repeated here. The result file must be corrected to the observed truth |
| D4 | Build ran without `--disable-build-servers` and without an external `TMPDIR` | **Waived once**: no build-server process remains and no output escaped the lane. Any repeat must use both flags |
| D5 | Part J of `tests/ui-smoke-multiarea.sh` expects `reason-choice` and `(not a Crop Tree)` | **Confirmed pre-existing drift**: baseline `PlayerMultiApi.cs` contains **0** occurrences of each while the script references them **7** times. Unrelated to this change; `tests/` stays untouched and Part J must be recorded as pre-existing |

## 4. Clearance (the only actions authorised now)

1. **Adopt the lane** at `3e6a6add` — no second worktree, no re-authoring, no rebase.
2. **The existing build stands.** No second compile is needed while the source stays byte-identical to `3e6a6add`. Source edits remain outside this clearance; if a source change is genuinely required, stop and report. A rebuild of the *same unchanged bytes* after a tooling failure is permitted once, with the flags of D4 and a recorded reason.
3. **One bounded `/multi` run**: from a scratch stand-data copy, loopback **OS-assigned ephemeral port**, own PID, stopped afterwards; never bind or proxy `:33767`, `:43649`, `:46593`; no unattended parking; record endpoint/PID/session/source SHA.
4. **Evidence** to `/home/jer/Development/CCF-885-Astra-UI-Evidence-20261122` created mode `700`; only **already-installed** browser tooling (no new dependency, no package, no tool install); capability never written in publishable form and never placed in the result Markdown.
5. **`tests/` untouched**; Part J recorded as pre-existing baseline drift with the counts above.
6. **Update the result Markdown** `Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-OPUS-UI-IMPLEMENTATION-RESULT-20261122.md`: correct the COMPILED/SERVED/P0 lines to what actually happened, add the runtime evidence, keep the masking script and the equal masked hashes, and record D1–D5.
7. **Commit exactly that one file** (`git diff --name-only HEAD^..HEAD` must show only the result Markdown), then **push only** `feat/multi-885-astra-opus-ui-20261122-v1`.
8. **Push rules**: no `--force`, no tags, no other refs, no PR, no `main` merge or update. After pushing, verify `git ls-remote origin refs/heads/feat/multi-885-astra-opus-ui-20261122-v1` equals the local head **and** that remote `main` is still `3a67d098`.

## 5. Still forbidden

Source edits beyond the three literals (already committed), `tests/` edits, csproj/props/wwwroot/Core edits, P0 rebuild or `build-p0-core-ref.sh`/`p0-isolation.sh`, a second worktree, additional dependencies or tooling, a QA programme or extra author/reviewer, `main`, PR, board edit, force push, killing any protected process, deleting another lane's outputs, ownership transfer, arbitrary expiry.

## 6. Non-claims

No design approval, no review verdict, no acceptance, no Stage I or G2 status, no Q1 authorisation. This receipt adopts a lane, records five deviations honestly, and clears one bounded runtime plus one branch push so Astra can review. Nothing here endorses the UI's quality.
