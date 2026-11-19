# Multi #791 — Year2–10 physical source custody and offline admission: result

**19 November 2026, 08:05 UTC — RESULT: CUSTODY VERIFIED; SOLE-WRITER LEASE ADMISSION BLOCKED ON ONE CONDITION; GRANTS PROPOSED. NO SOURCE TOUCHED.**

This is the #791 physical admission result. It does not implement Year2–10, does not edit or copy protected source, and runs no .NET. The four-file specification at `a2509922` is owner-approved (G0) and Manager-APPROVED, but implementation remains held until a physical lane exists and phase-specific grants are signed.

## Actors

| Role | Actual identity |
|---|---|
| Parent / coordinator | DeepSeek V4.1 Flash, `opencode-go/deepseek-v4.1-flash`, native OpenCode on T3 Code, `jer-laptop`, `/media/jer/Files/CCF-Primary` |
| Physical Ops custodian (#791) | **GPT-6.1 Sol**, Codex harness, T3 Code, `jer-laptop`; observed session `01a1f2ce-aa81-7762-b500-770518e7dd0f`; T3 child thread `…delegate-task:7f33827c-8aae-47d7-ae01-52e3cbe272fb` |
| Claim | [#791 RUNNING — PHYSICAL CUSTODY ONLY](https://github.com/jermur788/CCF/issues/791#issuecomment-7003242340), 2026-11-19T07:57:34Z |
| Base | `origin/main` `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` |
| Child actors | none. No Claude reviewer or Q started; no source task exists for them yet |

Custodian findings were independently re-measured by the parent before publication. Where the two differ, the difference is stated.

## Result in one sentence

The Year1 source custody, the ZX20 physical identity, the live lock state and the Videos volume are all **healthy and verified**; the single obstacle is that the **live Ops lane validator refuses any lane outside ZX20 or the Files CCF tree**, so a Videos sole-writer lease cannot be recorded without a small authorised Ops-tool change.

## Verified evidence

| # | Check | Result | Evidence |
|---|---|---|---|
| 1 | ZX20 stable identity | **PASS** | `/media/jer/ZX20` = `/dev/sdc2`, exfat, UUID `18D6-896E`. Configured device `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2` → `/dev/sdc2`; both node and by-id path have `st_rdev` `2082`. |
| 2 | Live Ops validator | **PASS** | Live `_ccf.storage_check(root, uuid, fstype, device)` → `/media/jer/ZX20`. Live `unity-lock status` → `Unity lock: free`, `metadata: None`, `unity_editor_pids: []`. `state/unity/lock.json` absent; `guard` inode predates this run (2026-10-16), so the status read created and changed nothing. |
| 3 | Overnight "mount identity mismatch" | **STALE COPY, NOT A LIVE FAULT** | The repo copy `/media/jer/Files/CCF-Primary/CCF-Ops/{lanes.json,_ccf.py}` (2026-11-03/05) pins `/dev/sda2` and compares device **strings**. The live Ops (2026-11-05 onward) pins the by-id path and compares **device numbers** (`same_device`) plus a unique-UUID check. Record-fidelity defect in the anchor copy; no bypass was used and none is needed. |
| 4 | Live board | **PASS** | `board/active.md`, 44 data rows, SHA-256 `4fa750eb…`, mtime unchanged `2026-11-18T17:28:31Z`. **No row for #789 or #791.** `CCF-725-MULTI-F0-ANNUAL` = `DONE-PENDING-LEDGER`, owner "EXCLUSIVE IMMUTABLE SOURCE CUSTODY: owner jermur788 via NEW #735 native T3 Codex gpt-6.1-sol medium, session 01a1eeb5…; no source writer". |
| 5 | #725 original source | **PASS** | `ccf-725-src` HEAD `df467dcc302eca564343e6da40f2fb88529a5d16`, branch `feature/multi-p1-f0-annual-725-20261118-v1`, `git status --porcelain` empty, **5,944 tracked files**, bare store `git/CCF-725-F0Annual.git` present, `receipts/LEASE.md` intact. Retention to **2026-11-27T18:00Z** is custody, not a write lease. |
| 6 | Videos volume | **PASS** | `/media/jer/Videos` = `/dev/sdb1`, **ext4**, UUID `60e276b9-4abb-4325-a913-f4c43c14294b`, **412.62 GiB free**, writable, no `CCF` directory, nothing created. |
| 7 | Host capacity | **PASS** | MemAvailable ≈ 13.0 GiB, memory PSI `some`/`full` `avg10=0.00`, no blender/unity/dotnet/codex process above 50 % CPU. One-heavy-worker rule held throughout; no build ran. |
| 8 | Fixture pin | **PASS (metadata)** | `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` blob size **32,567 B**, matching published digest `bcaaa5c3…`. Size read from the Git tree; contents not opened. |
| 9 | Golden pin | **PASS (location)** | No tracked 45,980-byte blob exists in the source tree; the golden is the CLI **stdout** report. Byte-identical copies exist at `/media/jer/Files/CCF/Verification/CCF-725-F0Annual-20261118/tests/annual-{1,2}.json` and `…/CCF-732-Year1Qual-20261118/worker-resume/runs/04-golden-{1,2}.stdout`, each 45,980 B (`574d8746…`). Contents not read. |
| 10 | Repaired-R delta | **PASS** | `git diff --name-only 35d26f5e HEAD` = exactly five paths, all under `Tools/MultiRackSitkaP1Lab/` (see C scope). The custodian's earlier `git log --name-only -1 35d26f5e` showed four *shell-script* paths because that command lists the R commit's own diff, not the R→HEAD delta. No discrepancy in the source. |
| 11 | **Videos lane admission** | **FAIL — LIMITING CONDITION** | Live `_ccf.config_read` on a simulation copy with one non-unity Videos lane raised exactly: `ValueError: path escapes ZX20: /media/jer/Videos/CCF-Multi791`. |

## The one limiting condition

`config_read` admits lanes only under `/media/jer/ZX20` or the Files CCF tree:

```python
elif not lane['unity'] and Path(lane['path']).resolve().is_relative_to(DURABLE_CCF.resolve()):
    resolved = str(Path(lane['path']).resolve())
else:
    resolved = str(on_storage(lane['path'], root))   # raises 'path escapes ZX20'
```

A `/media/jer/Videos/...` lane therefore cannot be registered, and without registration no lease, board row or linted custody can exist. Per the handoff I did **not** bypass or mutate the validator, and did not force ownership.

### Proposed bounded correction (requires its own authorisation; not applied)

Extend the durable-source-root policy from one Files root to a small explicit list, validated exactly as ZX20 is:

1. `_ccf.py`: add `VIDEOS_ROOT = Path('/media/jer/Videos')`, an `on_videos(path)` bound to that root using the same `st_dev` check as `on_storage`, and a `source_roots` entry in `lanes.json` carrying `{root, uuid: 60e276b9-4abb-4325-a913-f4c43c14294b, fstype: ext4, device: /dev/disk/by-id/…-part1}` — validated with `same_device` + `require_unique_uuid` so a rebadged device node cannot impersonate the volume.
2. `config_read`: allow a **non-unity** lane under any configured source root; Unity lanes stay ZX20-only. `repository`, `board` and `git_stores` rules are unchanged.
3. Tests: accept a valid Videos non-unity lane; reject a Videos path with the wrong UUID, a Unity lane on Videos, a path outside every configured root, and a duplicate.
4. `lane-status` continues to report `ZX20: NO` for such a lane explicitly rather than failing.
5. Authorisation: this touches the shared Ops tool, so it needs its own bounded task, one writer, independent diff review and the canonical board flock/lint/atomic-readback transaction. It must **not** be bundled into #791 or the engine.

Scope is roughly 20–30 lines plus tests. Alternative if the owner prefers no Ops-tool change: keep the engine lane on Files (ext4, 38 GiB free) — workable for a ~100 MB .NET tree, but contrary to the "Videos for heavy source" policy.

## Proposed grants (candidate text for the Manager to sign; nothing here is granted)

### B-01 — READ-ONLY source map (custodian → author, no writes)

Read-only access to the retained `ccf-725-src` at `df467dcc` for the exact closure below; git metadata, file contents read, **no** writes, copies, renames, builds or runs; output is a source map with per-file SHA-256 and the exact referenced-project closure.

Seed paths (relative to `Tools/MultiRackSitkaP1Lab/`):
`MultiRackSitkaP1Lab.sln`; `src/Cli/Program.cs`, `src/Cli/SitkaP1.Cli.csproj`; `src/Core/AnnualEngine.cs`, `src/Core/AnnualRules.cs`, `src/Core/SitkaP1.Core.csproj`; `tests/Core.Tests/Program.cs`, `Runner.cs`, `AnnualTests.cs`, `SitkaP1.Core.Tests.csproj`; `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`.
Plus only the P0 `Tools/MultiRackSitkaLab/src/Core` files that `SitkaP1.Core.csproj` actually references (to be enumerated by B-01, not guessed).
Forbidden: every other path, `.git` internals, global NuGet configuration, #710, #784 and #770 trees.

### C — writer allowlist (proposed; finalised from B-01)

Writes confined to the new lane copy of `Tools/MultiRackSitkaP1Lab/`:
`src/Core/AnnualEngine.cs`, `src/Core/AnnualRules.cs`, `src/Cli/Program.cs`, `tests/Core.Tests/AnnualTests.cs`, `tests/Core.Tests/Program.cs`, `tests/Core.Tests/Runner.cs`, any **new** files under `src/Core/` or `tests/Core.Tests/`, and the three P1 `.csproj` files only where a compile reference genuinely requires it.
Forbidden: P0 `MultiRackSitkaLab` sources, the fixture bytes, golden bytes, `.git` config, NuGet global config, save/persistence code, any public API/ABI surface, and anything outside the lane.
One writer only; sequential commits, one year per atomic step; no thinning, recruitment, saving or work-plan semantics.

### Offline .NET command list (proposed)

Environment: `unshare --user --map-current-user --net`; `env -i` with `HOME`, `TMPDIR`, `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `XDG_CACHE_HOME` all inside the Videos lane; `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1`; `-p:NuGetAudit=false`; pinned cleared-source `NuGet.Config`. Proven by a downed `lo` and failing DNS/TCP before any build.

1. `dotnet build Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln --nologo -v minimal --disable-build-servers -p:NuGetAudit=false -p:RestoreConfigFile=<lane>/offline/NuGet.Config` — SDK 8.0.133 as used by #725.
2. `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`
3. `dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived` — **twice**, requiring byte-identical 45,980-byte stdout with SHA-256 `574d8746…`.
4. Original-domain refusal: the frozen P0 annual command must exit 2 with `REFUSED OriginalAnnualRefused` and empty stdout.
5. Tamper: a one-byte copy of the fixture must exit 2 with `REFUSED FileHashMismatch`.
6. Repeat-advance: `AnnualAlreadyAdvanced`, no mutation.
Ceiling: one build attempt (one retry only for a non-network environmental fault), golden ×2, refusals ×1 each. No network, no restore from remote, no package add.

### Independent review and Q

- **Reviewer (nonauthor):** Claude Sonnet 5.5 or Opus 5.5, read-only on the pushed lane branch, changed-lines plus numerical/source review; must be a different session from the author. Must pass **before any 1→2**.
- **Q:** a separate Claude Opus 5.5 session, running the offline list above on the frozen candidate commit, verifying R-31/T-19 positive-pressure mutants and R-32/T-20 density-boundary mutants actually fail, plus death chronology, rollback, idempotency, singleton/empty and 0→10 sequential transitions.
- Both must record actual session IDs; no substitution.

## Required owner action (I cannot self-grant)

1. **#725 holder grant** — issue, on #791, an explicit signed statement on this model: *"I authorise one read-only export of the #725 qualified source at `df467dcc`, limited to the B-01 seed closure above plus the P1 fixture `p1d-30m-sitka-adult-v1.sitkap1fx.json` (32,567 B, `bcaaa5c3…`), into the new Videos lane, with hash receipts, leaving the original immutable and unedited; and one sole-writer lease on that lane for the engine author."*
2. **Lane decision** — either authorise the bounded Ops-tool change above so a Videos lane can be registered, or direct that the engine lane uses Files ext4 instead.
3. Manager #730 then signs B-01, C, offline and Q grants **after** the lease exists.

## Status

| | |
|---|---|
| Custody verified | **yes** |
| Physical lease | **NO — one condition** |
| Source read/copy | **no** |
| .NET run | **no** |
| IMPLEMENTED / QUALIFIED / INTEGRATED / ACCEPTED | no / no / no / no |

## Not done (and not claimed)

No protected source content read, copied, hashed or moved. No fixture bytes read. No .NET, build, restore or test. No worktree, clone, directory, cache or `bin/obj` created anywhere, including Videos. No board, `lanes.json`, `deployment.json` or `state/` mutation (board SHA-256 unchanged, 44 rows). No branch or push from the custodian. No main merge, release, Unity, Windows or Host. No #784 Viewer or #770 Scenario One contact. No GPT-6.2. `L4 FAIL38` UNWAIVED. Dashboard/Drive sync not verified.

## Boundaries preserved

Original #725 and #710 custody untouched. #735 retention intact to 27 Nov 18:00Z. One heavy worker only. No root-drive development.
