# Multi #791 — Files source lease and narrow Year1 transfer: result

**19 November 2026, 08:20 UTC — RESULT: LEASE BOOKED, NARROW EXPORT COMPLETE AND VERIFIED, ORIGINAL IMMUTABLE. NO IMPLEMENTATION, NO .NET.**

This closes the physical admission task for #789. Implementation remains unauthorised until the Manager signs B-01, then C and the offline/review/Q grants.

## Authority in force

| Instrument | Where |
|---|---|
| Owner G0 + Option A | [#791 comment 08:05Z](https://github.com/jermur788/CCF/issues/791#issuecomment-7003126758) |
| Manager Files-lane decision | `docs/multi-730-791-files-lane-decision-20261119-v1`; [#791 08:04Z](https://github.com/jermur788/CCF/issues/791#issuecomment-7003383668) |
| **Owner/#725 holder export grant** | [#791 08:05Z](https://github.com/jermur788/CCF/issues/791#issuecomment-7003402270) — quoted verbatim: *"I authorise one read-only export of the retained qualified Year1 source and pinned fixture into a new, separately reserved Year2–10 worktree on Files. Preserve the original source unchanged, verify transferred hashes, and require separate implementation and offline-testing grants."* |

Scope of that grant, as recorded by the Manager: one read-only export from the immutable retained #725 source, limited to the B-01 seed/source-reference closure established in #791 (P1 source/solution/test project, and only actually referenced P0 Core files) plus **one** pinned 32,567-byte fixture; per-file and fixture hashes; no original writes; no #710 or #687 access. It creates no lease itself and grants no C edit, no dotnet, no review or Q.

## Actors

| Visit | Model / harness | Session | Outcome |
|---|---|---|---|
| #791 admission (Videos) | GPT-6.1 Sol, Codex, T3 Code | `01a1f2ce-aa81-7762-b500-770518e7dd0f` | custody verified; lease blocked on validator |
| Files lease, first attempt | GPT-6.1 Sol, Codex, T3 Code | `01a1f2dc-a282-7592-a058-4ff31ea4a143` | preflight PASS, then **stopped**: script staged lint copies under a path outside its stated allowlist. No write. |
| Files lease, continuation | GPT-6.1 Sol, Codex, T3 Code | `01a1f2dd-ba84-7970-883b-c565f5f77c25` | booked the lease, exported, verified |
| Parent / coordinator | DeepSeek V4.1 Flash, native OpenCode | this thread | defined allowlist, re-verified every claim |

The stop at the first attempt was correct behaviour, not a fault: the custodian refused to widen its own authority. The missing authorisation was supplied narrowly (the Ops tool's own sanctioned `/media/jer/ZX20/CCF-Temp` isolated-test area, created and removed inside one `finally` block) and nothing else changed.

## The lease

| Field | Value |
|---|---|
| Lane label | `Multi789Y2Src` |
| Path | `/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/ccf-y2-10-src` |
| Branch | `feature/multi-789-year2-10-engine-20261119-v1` |
| HEAD | `4b81e71eaa3d0d614c319a38afe8b9e030c61bda` |
| Board row | `CCF-789-MULTI-YEAR2-10-SOURCE`, State `READY`, Unity required `No`, Lane `Multi789Y2Src` |
| Board transaction | exclusive nonblocking `flock` on `/media/jer/ZX20/CCF-Ops/board`; 44 → **45** rows; lint PASS; zero conflicts; atomic write + readback |
| Board SHA-256 | before `4fa750eb…` → after `0baf348dcc1e9e4a757a40db83c8c2982c406dfa0f82096132572513bafcd566` |
| lanes.json SHA-256 | before `c1d14719…` → after `aa3d465832cc3f37e9a6f971480055a3b008d63057adf1b8de2a8bbaf51c2c97` |
| Remote | none (no push path exists) |
| Working tree | clean, 30 tracked files, 0 untracked |

The row records "RESERVED SOLE-WRITER LEASE — no active writer — custodian GPT-6.1 Sol Codex session `01a1f2dd…`" and open gate "B-01/C/offline/Q grants unsigned; no implementation or .NET authority". The lease is a booking, not an active writer claim.

## The export

| | |
|---|---|
| Files | **30** tracked `Tools/MultiRackSitkaP1Lab/**` files |
| Bytes | **224,793** |
| Verification | all 30 lane blob IDs **and file modes** identical to the source tree at `df467dcc`; zero missing, zero extra, zero content or mode differences |
| Fixture | `…/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, **32,567 B**, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` |
| Receipts | per-file SHA-256 and byte count in `/media/jer/Files/CCF/Verification/Multi-791-Files-Lease-20261119/receipts.json` |
| Method | verified by comparing Git blob IDs (lane tree vs source tree), so the protected source was **not re-read** to confirm the copy |

The five repaired-R delta files are present: `src/Cli/Program.cs`, `src/Core/AnnualEngine.cs`, `src/Core/AnnualRules.cs`, `tests/Core.Tests/AnnualTests.cs`, `tests/Core.Tests/Program.cs`. Also included: the solution, `Directory.Build.props`, `global.json` (SDK 8.0.133), the three project files, `Runner.cs`, the P1 build/safety scripts, `README`, and `evidence/README.md`.

## Original preserved

`/media/jer/Files/CCF/Operations/CCF-725-F0Annual-20261118/ccf-725-src` remains at `df467dcc302eca564343e6da40f2fb88529a5d16`, working tree clean (zero changes), 5,944 tracked files, `receipts/LEASE.md` intact. It was opened read-only. #735 custody to 2026-11-27T18:00Z is unchanged. #710 is untouched.

## Open scope decision (B-01)

The P1 projects reference P0 **as a pre-built binary**, never a `ProjectReference`:

```
<Reference Include="SitkaLab.Core"><HintPath>$(P0CorePath)</HintPath></Reference>
```

`build/build-p0-core-ref.sh` rebuilds that reference from the pinned P0 subtree (`f212c8f6…` / commit `a7f41983…`) into `.p0ref/`. The retained tree still holds `.p0ref/SitkaLab.Core.dll` (120,832 B) with `SitkaLab.Core.dll.sha256` and `provenance.txt`.

**Nothing from P0 was exported**, because the grant names *source* and the reference is a build artefact; the grant's "only actually referenced P0 Core files" therefore has no source-level referent in this design. Consequence: **the lane cannot compile yet.** B-01 must choose one of:

- **(a)** export the P0 Core source subset (52 files, 777,548 B) so the reference can be rebuilt offline under the later build grant; or
- **(b)** export the existing `.p0ref/` artefacts (DLL + sha + provenance) as a pinned binary reference, with the DLL digest recorded in the lease receipt; or
- **(c)** rule that the writer builds `.p0ref` from an offline P0 source already available elsewhere, with no export.

Recommendation: **(a)**, because it keeps the lane self-contained, lets the writer rebuild deterministically from the pinned tree, and avoids depending on a binary whose provenance cannot be re-derived inside the lane. (b) is smaller but pins a DLL the writer cannot rebuild.

## Proposed B-01 — READ-ONLY source map (post-lease)

Read-only on the new lane at `4b81e71e`, no writes, no builds. Deliver a source map with per-file SHA-256 covering: the 30 exported files, the exact P0 Core file set chosen above, the fixture, and the compiled P0 reference identity. Output answers three questions: which files implement CI target/neighbour selection, growth/age/crown, stand metrics, mean vulnerability, hazard candidate/roll, and RD ordering; where the healthy/dead discriminator (M-1) sits; and the exact call path from the CLI `annual` dispatch to mortality. No inference of coefficients.

## Proposed C — writer allowlist

Writes confined to the new lane:

- `src/Core/AnnualEngine.cs`, `src/Core/AnnualRules.cs`, `src/Cli/Program.cs`
- `tests/Core.Tests/AnnualTests.cs`, `tests/Core.Tests/Program.cs`, `tests/Core.Tests/Runner.cs`
- **new** files under `src/Core/` and `tests/Core.Tests/`
- the three `.csproj` files only if a compile reference genuinely requires it

Forbidden: the fixture bytes, any golden bytes, `.p0ref` (beyond reading it), `.git` internals, the P0 source tree (except read-only if (a) is chosen), save/persistence code, any public API/ABI surface, and everything outside the lane. One writer; one year per atomic commit; no thinning, recruitment, saving or work-plan semantics.

## Proposed offline .NET command list

Environment: `unshare --user --map-current-user --net`; `env -i` with `HOME`, `TMPDIR`, `DOTNET_CLI_HOME`, `NUGET_PACKAGES`, `XDG_CACHE_HOME` all inside the lane; `DOTNET_CLI_WORKLOAD_UPDATE_NOTIFY_DISABLE=1`; `-p:NuGetAudit=false`; cleared-source `NuGet.Config`. Proof of isolation (downed `lo`, failing DNS/TCP) before any build.

1. offline build: `dotnet build Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln --nologo -v minimal --disable-build-servers -p:NuGetAudit=false -p:RestoreConfigFile=<lane>/offline/NuGet.Config` — SDK 8.0.133
2. `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`
3. golden ×2: `dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived` — byte-identical 45,980 B stdout, SHA-256 `574d8746…`
4. original-domain refusal: exit 2, empty stdout, `REFUSED OriginalAnnualRefused`
5. tamper refusal: one-byte fixture copy → exit 2, `REFUSED FileHashMismatch`
6. repeat-advance: `AnnualAlreadyAdvanced`, no mutation

Ceiling: one build attempt (one retry only for a non-network environmental fault), golden ×2, refusals ×1 each. No network, no remote restore, no package add. Golden and refusals are measured against the exported fixture and the published digests, not regenerated to fit.

## Proposed independent review and Q

- **Reviewer (nonauthor):** Claude Sonnet 5.5 or Opus 5.5, read-only on the lane branch, changed-lines plus numerical review. Must PASS **before any 1→2**.
- **Q:** a separate Claude Opus 5.5 session on the frozen candidate, running the offline list, plus R-31/T-19 positive-pressure and R-32/T-20 density-boundary mutants that must fail, death chronology/freeze, rollback, idempotency, singleton/empty, and 0→10 sequential non-jump transitions.
- Both record actual session IDs; no substitution.

## Receipts

| Item | Location |
|---|---|
| Preflight | `/media/jer/Files/CCF/Verification/Multi-791-Files-Lease-20261119/logs/continuation-preflight.json` |
| Apply | `…/logs/continuation-apply.txt` |
| Verify | `…/logs/continuation-verify.txt` |
| Per-file hashes | `…/receipts.json` |
| Board/lanes before | `…/board-before.md`, `…/lanes-before.json` |
| Transaction script | `…/lease.py` |

## Status

| | |
|---|---|
| Physical lease | **YES** — booked, linted, read back |
| Narrow export | **YES** — 30 files, blob-identical |
| Original immutable | **YES** — clean at `df467dcc` |
| .NET run | **no** |
| Implementation | **no** |
| IMPLEMENTED / QUALIFIED / INTEGRATED / ACCEPTED | no / no / no / no |

## Not done

No build, restore, test or `.NET` invocation of any kind. No engine source edit or new source file. No git push, no remote added, no GitHub product code. No change to the shared Ops tool. No root-drive worktree, clone, cache, `bin/obj`, NuGet, temp or log. No #710 or #687 access. No #770 Scenario One or #784 Year0 viewer contact — a pre-existing untracked `Tools/MultiRackSitkaYear0Viewer/fixtures/` directory in `ccf-h0a-core` (dated 2026-11-18 22:41Z) was observed and left untouched. No Unity, Windows, Host, main merge or release. No GPT-6.2. `L4 FAIL38` UNWAIVED. Dashboard/Drive sync not verified.

## Host receipt at completion

MemAvailable 13,383,651,328 B; memory PSI `some avg10=0.00 avg60=0.08`; Files available 38,603,022,336 B; no heavy worker running. One heavy worker at a time throughout.
