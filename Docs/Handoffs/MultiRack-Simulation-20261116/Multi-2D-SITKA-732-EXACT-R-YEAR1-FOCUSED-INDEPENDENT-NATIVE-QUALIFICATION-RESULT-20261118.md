# Multi #732 — exact R Year-1 focused independent native qualification: RESULT

**Date:** 18 November 2026 UTC. **Returned to:** Manager #730. **Claim:** [#732](https://github.com/jermur788/CCF/issues/732).

## VERDICT: BLOCKED — STOP rule triggered by an unexpected network call during grant step 2

Golden-output, refusal and tamper qualification were **NOT RUN**. This is not a FAIL of the #725 candidate. Every check made before the stop passed. The only executed step that produced build output, the P0 reference build, reproduced the pinned DLL exactly. Nothing in this result counts as native Year-1 qualification.

## Worker identity and authority

| Item | Value |
|---|---|
| Worker | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, native Linux, host `jer-laptop`. Session `76502b90-e137-4d93-b79d-992881bb0b30`. Effort setting not observable. |
| Independence | Not the #725 author (`01a1ed7e-…`). Not the #732 custodian (`c33954eb-…`). Same model family as the #729 reviewer but a different session. |
| RUNNING | [#732 comment 6977892670](https://github.com/jermur788/CCF/issues/732#issuecomment-6977892670), 11:57:56Z, posted before any lane access |
| Grant / start | [custodian GRANT](https://github.com/jermur788/CCF/issues/732#issuecomment-6977730799) (`custody/GRANT.md` sha256 `3055fc01…`; custody `SHA256SUMS` re-verified OK) and [Manager start](https://github.com/jermur788/CCF/issues/732#issuecomment-6977841386) |
| Lane | `MultiYear1Qual732`, `/media/jer/Files/CCF/Operations/CCF-732-Year1Qual-20261118/ccf-732-qual` |
| Candidate | Detached `df467dcc302eca564343e6da40f2fb88529a5d16`, parent R `35d26f5e64a00a400c700871927ccc2173370568`, tree `4237ece5c4efcd167857c1cb29e16bbcc2279ad6`, clean |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-732-Year1Qual-20261118/worker/` (`SHA256SUMS`) |

## What was executed (direct native output)

| # | Grant step | Command | Result |
|---|---|---|---|
| 1 | Before-guards (read-only) | `worker/guard.sh before` (Git reads, sha256sum -c, pgrep, find) | All PASS (details below) |
| 2 | P0 ref build, once | `build/build-p0-core-ref.sh` in the grant's `env -i` block, 11:59:40–11:59:49Z | **exit 0**, stderr empty, `SitkaLab.Core.dll` sha256 **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` = expected**. The script's own input-before/after check passed. The output stayed in the git-ignored `.p0ref/`. |
| — | Post-step-2 cache inspection | find/ls/sha256sum on lane `home/ nuget/ tmp/` | **Network contact found → STOP** |
| 3 | `dotnet build MultiRackSitkaP1Lab.sln …` | — | **NOT RUN** (stopped) |
| 4 | Golden annual run ×≤2 (`574d8746…`) | — | **NOT RUN** |
| 5 | Original-P0 refusal (exit 2, `OriginalAnnualRefused`) | — | **NOT RUN** |
| 6 | Tampered-copy refusal (exit 2, `FileHashMismatch`) | — | **NOT RUN**. No `$OP/tmp/tamper` was created. |
| 7 | After-guards | `worker/guard.sh after` | All PASS, identical to before |

## STOP trigger (direct evidence)

During step 2 the SDK made outbound HTTPS requests to `api.nuget.org`. The lane HOME, which is otherwise fresh, now holds:

- the NuGet HTTP cache `home/.local/share/NuGet/http-cache/…$ps:_api.nuget.org_v3_index.json/`, which contains:
  - `service_index.dat`, 9,431 B, 11:59:42.62Z;
  - `repository_signatures_5.0.0.dat`;
  - `vuln_index.dat`;
  - `vuln_data_base.dat`, 842,176 B, 11:59:44.61Z;
  - `vuln_data_update.dat`.
  These are NuGet Audit vulnerability-data downloads made during restore; hashes are in `stop/http-cache.sha256`.
- the SDK workload-advertising manifests under `home/.dotnet/sdk-advertising/8.0.100/` (66 files, 7 workload manifests, 11:59:43.8Z). These come from the CLI's background advertising-manifest update. The env block does not suppress it.
- `NUGET_PACKAGES` (`nuget/`) is still **empty**. No package was downloaded, and the build has no PackageReferences. Restore did not *need* the network.

The grant's STOP rules list "an unexpected network call or restore happens", and the Manager permit says "network restore … is STOP, not workaround". So the network contact is a STOP. Continuing would have required an environment or command change that the grant does not list (see the remedy below), so I stopped instead of working around it.

**Pre-existing condition, not caused by this worker:** the #725 author lane shows the same artefacts at 07:33:41–42Z. It has the identical `api.nuget.org` http-cache (5 files, same sizes) and `home/.dotnet/sdk-advertising/` (`stop/725-lane-same-artefacts.txt`, read-only listing). So the #725 author build receipts were also produced with these network calls, and the #725 `LEASE.md` ("no Host/network") and author report do not mention them. **Manager to note:** the network-free status of the author's .NET build/test receipts is unproven in the same way.

## Guards: before vs after (direct native output)

| Guard | Before | After |
|---|---|---|
| Qual worktree HEAD / parent / tree / detached / tracked status | df467dcc / 35d26f5e / 4237ece5 / detached / 0 changes | identical |
| Qual tracked-file sha256 (5,944 files) | `guard-before/qual-tracked.sha256` | byte-identical (`0baa2952…` manifest) |
| Qual ignored files | none | `.p0ref/` only (allowed build output) |
| Qual fixture tree / store refs | — | identical |
| #725 worktree HEAD/branch/status, refs, index sha256 `1ebc531b…` | clean | identical |
| #725 evidence (79) + receipts (16) vs custodian manifests | OK | OK |
| #710 R HEAD `35d26f5e`, tree `33f84db2`, branch, status, refs | clean | identical |
| #710 evidence (42) + receipts (2) | OK | OK |
| CCF-Primary two user-dirty hashes | OK | OK |

No protected path was written. No tracked file changed. No Git write, board write or lanes write happened. All writes went to lane `home/ tmp/`, `ccf-732-qual/…/.p0ref/`, `worker/` and the session scratchpad.

**Residual process:** `VBCSCompiler` (pid 1189565) was started by the build inside step 2, which does not pass `--disable-build-servers`. It was still idle in the lane at the after-guard. It exits on its own idle timeout. Killing it is not a listed command, so I left it for the custodian to confirm at release. (The "before" `procs-dotnet.txt` line matching my own shell is a pgrep `-f` self-match artefact. No dotnet process ran before step 2.)

## Proportionate remedy (proposal for Manager #730 / custodian — not applied)

A grant amendment would let a re-run go fully offline without changing source or runtime-switch semantics. Choose one option:
- (a) add `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1` to the env block and `-p:NuGetAudit=false` to both builds; or
- (b) a lane-local `NuGet.Config` with `<clear/>` packageSources, passed explicitly; or
- (c) wrap the dotnet steps in a no-network namespace (`unshare -rn`), if permitted.

Each needs explicit enumeration, because the grant lists the exact command lines and prohibits `DOTNET_*` changes. The step-2 build is already used up, but its DLL output in `.p0ref/` is verified, so a re-run can go straight to step 3. The custodian should also decide whether the lane HOME caches are kept as evidence (recommended) or purged before a re-run.

## Status terminology

#725 remains **IMPLEMENTED LOCALLY** and **INDEPENDENTLY SOURCE/NUMERIC REVIEWED (#729)**. It is **NOT independently native-qualified**. It is not integrated, Unity-compatible, a release or a field forecast. **L4 FAIL38 remains UNWAIVED**; nothing here touches L4. **M-1** dead-stem filtering is still mandatory before Year 2. The source is still local and not pushed.

## Evidence index

`worker/`:
- `running.md`, `guard.sh`;
- `guard-before/`, `guard-after/`;
- `runs/01-p0ref.{meta,stdout,stderr,outputs.sha256}`;
- `stop/{lane-cache-listing.txt,http-cache.sha256,725-lane-same-artefacts.txt}`;
- `RESULT.md`, `SHA256SUMS`.

---

## Resume (offline, amendment 1) — 18 November 2026 UTC

### RESUME VERDICT: PASS — all remaining enumerated #732 checks (steps 3–6) passed natively, fully network-isolated, with all guards unchanged

This section reports **only the resumed checks**. The BLOCKED section above is kept verbatim, and the Git history (`4f6061b2`) also preserves it. The step-2 P0 reference build is **not** re-reported: it was consumed with a PASS in the original run and was **not rerun**. This is a native qualification result returned to Manager #730 for verdict. It is **not** an integration, Unity, release or L4 result.

### Resume worker identity and authority

| Item | Value |
|---|---|
| Worker | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, native Linux, host `jer-laptop`. Session **`32188791-3d36-433f-9b06-12025918440a`**. Reasoning effort is not observable from inside the session. |
| Independence | This is a new session. It is NOT `76502b90-…` (original #732 worker and amend-1 custodian), NOT `c33954eb-…` (original #732 custodian) and NOT `01a1ed7e-…` (#725 Codex author). |
| RUNNING | [#732 comment 6978413590](https://github.com/jermur788/CCF/issues/732#issuecomment-6978413590), 12:21:31Z. It was posted before any lane access. I re-read the comments after posting: there was no competing RUNNING-RESUME. |
| Authority | [AMEND-1 grant](https://github.com/jermur788/CCF/issues/732#issuecomment-6978264004): `custody/amend-1/GRANT-AMEND-1.md` sha256 `15143886ada381c3…`, with amend-1 `SHA256SUMS` re-verified OK. [Manager #730 APPROVED TO RESUME OFFLINE](https://github.com/jermur788/CCF/issues/732#issuecomment-6978297258). [Manager offline-resume packet](https://github.com/jermur788/CCF/blob/docs/multi-730-732-offline-resume-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-MANAGER-732-NETWORK-STOP-VERDICT-AND-OFFLINE-RESUME-PACKET-20261118.md) at tip `85ea3ffe`. The original `custody/GRANT.md` (`3055fc01…`) and custody `SHA256SUMS` are OK. |
| Lane / candidate | `MultiYear1Qual732`, detached at `df467dcc302eca564343e6da40f2fb88529a5d16`, parent R `35d26f5e64a00a400c700871927ccc2173370568`, tree `4237ece5…`, 0 tracked changes. Lease expiry is 2026-11-20T18:00Z (not reached). |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-732-Year1Qual-20261118/worker-resume/` (`SHA256SUMS`). The original `worker/` (48 files, `SHA256SUMS` OK), `custody/`, `offline/` and `.p0ref/` were only read. |

### Wrapper used for every dotnet invocation (exactly as AMEND-1 §3.2)

Each run started in `$OP/ccf-732-qual/Tools/MultiRackSitkaP1Lab` with this prefix:

`unshare --user --map-current-user --net -- env -i PATH=/usr/bin:/bin HOME=$OP/home DOTNET_CLI_HOME=$OP/home NUGET_PACKAGES=$OP/nuget TMPDIR=$OP/tmp LANG=C.UTF-8 DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1 DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1 <command>`

`OP=/media/jer/Files/CCF/Operations/CCF-732-Year1Qual-20261118`. No dotnet process ran outside this wrapper, and no other environment variable or switch was added.

### Executed steps (all numbers are direct native output from this session)

| # | Command (after prefix) | Time (UTC) | Result | Expected | Verdict |
|---|---|---|---|---|---|
| 7a | Before-guards (`worker-resume/guard.sh before`; read-only) | 12:21:58 | All PASS (see guards) | unchanged | PASS |
| 3 | `dotnet build MultiRackSitkaP1Lab.sln --nologo -v minimal --disable-build-servers -p:NuGetAudit=false -p:RestoreConfigFile=/media/jer/Files/CCF/Operations/CCF-732-Year1Qual-20261118/offline/NuGet.Config` (attempt 1 of 1; **no retry used**) | 12:23:29.7–12:23:41.6 | **exit 0, `0 Warning(s)`, `0 Error(s)`**, stderr empty. 3 projects restored offline (~112 ms each). It built SitkaP1.Core, SitkaP1.Core.Tests and sitkap1 (the Core.Tests DLL was **built, not run**). | exit 0 / 0 W / 0 E | **PASS** |
| 4 | `dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived` (run 1 of 2) | 12:24:05 | **exit 0**, stdout 45,980 B, sha256 **`574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`**, stderr empty | exit 0, `574d8746…` | **PASS (exact)** |
| 4 | same command (run 2 of 2, repeatability) | 12:24:42 | exit 0, same sha256, **byte-identical** to run 1 (`cmp`) | — | PASS |
| 4b | `python3 -I worker-resume/cmp732.py` (read-only): run-1 JSON vs #729 reviewer `model-output.txt` + `mine.csv` (#729 `SHA256SUMS` OK) | 12:24 | Stand Before/PostGrowth/After (living, stems/ha, BA/ha, QMD, RD) all **exactly equal**. PostGrowthDensityPressure `0.09395922171978738` and MeanVulnerability `0.03541018395366311` are equal. **700 per-tree values** (DBH, H, crown, CI, S, p, roll) for 100 IDs, plus neighbours, age 31 and no death: **0 mismatches**. Year-1 deaths: 0. | match #729 | **PASS** |
| 5 | `dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual OriginalPolygonExploratoryV1 oracle-quarter-ha` | 12:24:42.7 | **exit 2**, stdout **empty** (0 B), stderr `REFUSED OriginalAnnualRefused: original P0 fixtures are read-only in F0; annual advance waits for the later P1-POLY polygon engine` | exit 2, `OriginalAnnualRefused`, empty stdout | **PASS** |
| 6 | `cp -r fixtures/derived "$OP/tmp/tamper"`. Then one same-size byte change in the copy only, with `python3 -I`: offset 3648, P30-001 `heightMm` `23070`→`23071` (`'0'`→`'1'`). Size stays 32,567 B and the JSON stays valid. The copy's sha256 went `bcaaa5c3…`→`a0c22433…`. The original stays `bcaaa5c3…`. Then the step-4 command with `"$OP/tmp/tamper"` as the fixture directory. | 12:24:59–12:25:22.8 | **exit 2**, stdout empty, stderr `REFUSED FileHashMismatch: derived fixture SHA-256 differs from the catalog` | exit 2, `FileHashMismatch` | **PASS** |
| 7b | After-guards (`worker-resume/guard.sh after`) | 12:25:28 | All PASS, identical to before (see guards) | unchanged | PASS |

On step 6 prep: my first `python3 -I` attempt searched for a needle (`"HeightM":23.07`) that does not exist in the fixture's integer-mm format. Its assert stopped it **before any write**, and the copy was confirmed still `bcaaa5c3`. The second attempt made the single change above. This recorded failure was in a preparation step only. The step-6 CLI command ran **once**.

**Build outputs (informational):** `sitkap1.dll` `effe2a4e…`, `SitkaP1.Core.dll` `e58dab51…`, `SitkaP1.Core.Tests.dll` `99dcef1e…`. The bundled `SitkaLab.Core.dll` in the CLI output is **`9a99f781…`**, the exact pinned P0 reference DLL.

**Cumulative #732 ceiling after this resume:**
- step 2: 1/1, consumed in the original run and not rerun;
- step 3: 1/1, retry unused;
- step 4: 2/2;
- step 5: 1/1;
- step 6: 1/1.

No #732 execution authority remains.

### Network and offline evidence (direct, before vs after)

| Check | Before (12:21:58Z) | After step 3 (mid-check) | After all (12:25:28Z) |
|---|---|---|---|
| Offline `NuGet.Config` sha256 `fa1d75e1…`, dir 0500 / file 0444, 231 B | OK | — | OK |
| `home/.local/share/NuGet/http-cache` vs `worker/stop/http-cache.sha256` | 5 files, all OK | 5 files, all OK | 5 files, all OK, **no new files** |
| `home/.dotnet/sdk-advertising` vs `custody/amend-1/sdk-advertising.sha256` | 66 files, all OK | 66, OK | 66, OK, **no new files** |
| `nuget/` (`NUGET_PACKAGES`) | empty | empty | empty |
| `.p0ref/` vs `custody/amend-1/p0ref.sha256` (DLL `9a99f781…`, provenance `556317ad…`, `.sha256` `c0670d30…`) | OK | — | OK |
| `dotnet` / `VBCSCompiler` processes (`pgrep -x`) | none | none | **none** (`--disable-build-servers` worked; there was no residual server) |

The only new lane files during the whole resume are inside the write ceiling:
- 7 zero-byte local NuGet lock files in `tmp/NuGetScratchjer/lock/` (12:23:31–32Z);
- MSBuild temp dirs in `tmp/`;
- `tmp/tamper/` (1 file);
- `bin/`/`obj/` under `src/Cli`, `src/Core` and `tests/Core.Tests`.

No HOME cache, http-cache or advertising artefact was created. Together with the namespace, this is direct evidence that the resumed steps made no external restore or SDK network contact. The namespace itself was not re-probed by this worker: the custodian's `netns-probe.txt` covers that, and probing was not on my command list.

### Guards: before vs after (direct native output; `worker-resume/guard-{before,after}/`)

| Guard | Before | After |
|---|---|---|
| Qual HEAD / parent / tree / detached / tracked status | df467dcc / 35d26f5e / 4237ece5 / detached / 0 | identical |
| Qual tracked-file sha256 (5,944 files; manifest `0baa2952…`) | byte-identical to custodian amend-1 pre and original worker | byte-identical |
| Qual fixture tree, P0 tree, store refs | = custodian amend-1 pre | identical |
| Qual ignored paths | `.p0ref/` only | `.p0ref/` plus the six `{src/Cli,src/Core,tests/Core.Tests}/{bin,obj}/` (allowed) |
| #725 HEAD/branch/status, refs, index `1ebc531b…`; evidence (79) + receipts (16) | = custodian; OK | identical; OK |
| #710 R `35d26f5e` / tree `33f84db2`, branch, status, refs; evidence (42) + receipts (2) | = custodian; OK | identical; OK |
| CCF-Primary two user-dirty hashes | OK | OK |
| `lanes.json` (read-only sha256) | `b16bfcad…` = custodian | `b16bfcad…` |
| Board `active.md` (read-only sha256) | `d98c3156…` ≠ custodian `8d3c17e1…` | `d98c3156…` |

**Board note:** between the custodian's 12:11Z readback and my 12:22Z check, `active.md` changed (mtime 12:22:44Z). I diffed it against `custody/post/active.md`. The **only** changed line is the unrelated CCF-652-OPS-READINESS row, which a separate physical board custodian refreshed (session `81a16309-…` per its text; #652 comment 6978301260). The CCF-732, CCF-725 and CCF-710 rows and `lanes.json` are unchanged, so there is no lane or lease conflict. The diff is in `board-active-diff-vs-custody.txt`. I made no board or lanes write and did not take the flock: I only ran read-only `sha256sum`, `grep` and `diff`. The CCF-732 row still says WAITING; updating it is the custodian's job.

### What was not done (scope)

The following were not done:
- a second P0 build;
- the 43- or 98-test suites, repeat-advance, p0-isolation, L4, strace/ltrace or the analyzer;
- any tracked-file edit, local Git write, or board/lanes write;
- any write to the #725/#710 lanes, Repository.git, CCF-Primary or Scenario One;
- any simulation flag or coefficient change;
- Unity, Windows, push/merge (other than this result file) or an L4 waiver;
- any process kill;
- any cleaning of the http-cache, `sdk-advertising` or `worker/` evidence.

This result file was updated with a GitHub Contents API commit on the existing branch. No local Git write was made. Build outputs, `tmp/` and `tmp/tamper` are left for custodian release.

### Status terminology after the resume

- **Direct native output from this session:** steps 3–6 and all guards above.
- **From the original run, not rerun:** the step-2 P0 DLL PASS.
- **From the #729 reviewer:** the comparison model (`model729.py` output). It was consumed read-only and not re-derived.
- **Author receipts not re-verified here:** the #725 author's 43-test and repeat-advance results. These remain author receipts plus #729 source review only.
- **Network provenance:** the original #725 author builds remain *not demonstrated network-free*, as noted in the BLOCKED section. This resume does not change that.

If Manager #730 accepts this result, #725 `df467dcc` becomes **natively qualified for the focused Year-1 checks above** under offline conditions. It is still NOT integrated, NOT Unity-verified and NOT a release or field forecast. **Default L4 FAIL38 remains UNWAIVED.** **M-1** dead-tree filtering is still mandatory before Year 2. NuGet audit suppression applied to this offline qualification run only. It is not a release or security exemption. The final verdict, integration and custody release belong to Manager #730 and the custodian. Dashboard/Drive sync was not verified.

### Resume evidence index (`worker-resume/`)

- `running.md`, `guard.sh`, `cmp732.py`
- `guard-before/`, `guard-after/`, `mid-after-03/`
- `board-readonly-check.txt`, `board-active-diff-vs-custody.txt`, `board-readonly-after.txt`
- `runs/03-sln-build.{meta,stdout,stderr}`, `runs/03-build-outputs.sha256`
- `runs/04-golden-{1,2}.{meta,stdout,stderr}`, `runs/04-golden-1.vs-729.txt`
- `runs/05-original-refusal.{meta,stdout,stderr}`
- `runs/06-tamper-prep.txt`, `runs/06-tamper-refusal.{meta,stdout,stderr}`
- `RESUME-SECTION.md`, `SHA256SUMS`
