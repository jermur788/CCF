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
