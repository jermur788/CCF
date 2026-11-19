# Multi #730 / #791 — Manager custody verdict, bounded B-01 admission and P0 library choice

**19 November 2026, 08:2x UTC. VERDICT: APPROVED #791 physical lease and narrow read-only Year1 export; B-01 READ-ONLY inspection authorised in new lane only; no source writes or .NET yet.**

## Evidence reviewed

Manager read [complete #791 result](https://github.com/jermur788/CCF/blob/docs/multi-791-files-source-lease-transfer-result-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/791-orchestration/Multi-791-FILES-SOURCE-LEASE-AND-YEAR1-NARROW-TRANSFER-RESULT-20261119.md). Physical lane **Multi789Y2Src**, path **/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/ccf-y2-10-src**, branch feature/multi-789-year2-10-engine-20261119-v1, booked at **4b81e71e**, row CCF-789-MULTI-YEAR2-10-SOURCE READY and **no active writer**. The custodian reports live shared nonblocking flock, 44 to 45 rows, lint zero conflicts, atomic write/readback, current board and lanes hashes. The 30 files (224,793 bytes) including fixture 32,567 bytes were verified against original #725 source by git blob-ID and file mode; original #725 retained source still clean and immutable under #735. Initial aborted staging-path attempt did no write and was fixed under exact separate temporary test-area authority. Manager has not physically independently re-run the custodian operations; verdict is based on its durable evidence.

## Manager acceptance

**APPROVED #791 custody outcome** as physical lease and one read-only source/fixture export ONLY. This does not approve a build, file edits, Q, independent source qualification, canonical integration or release. Close #791 as completed custody; #789 remains pending implementation phase.

## Dependency choice: B — qualified P0 binary, not a new P0 source export

The P1 project references P0 through a **prebuilt binary assembly** via P0CorePath and .p0ref/SitkaLab.Core.dll, not a ProjectReference. Exported P1 files alone cannot compile because .p0ref is absent.

**Manager selects Option B** — separately authorise ONE read-only transfer of the **existing, immutable P0 reference DLL (120,832 bytes), its .sha256 and provenance.txt** into the new lane under .p0ref, without modifying or rebuilding P0. Justification: minimal P0 exposure, no new P0 compilation policy/scope, and matching the exact binary dependency of the qualified Year1 candidate. Must verify P0 DLL hash, SHA sidecar, provenance, original file modes, source owner, and absence of unexpected additional bytes. Option A 52-file P0 Core source export is NOT approved; no broader #710 or other P0 source access, and no new P0 build. If the binary/provenance is stale, mismatched or missing required offline dependencies, STOP for Manager review rather than silently substitute another DLL.

**The owner's previous permission for one source+fixture export does NOT include this DLL.** The owner/#725 holder must expressly approve this extra three-file, read-only binary export, then a qualified physical custodian can perform it in the same reserved new lane. Do NOT read or copy the protected DLL before authorisation. No new lane/lease needed, but refresh actual status and ownership at use time.

### Exact requested holder permission

> I authorise one additional read-only transfer of the retained Year1 candidate's exact P0 reference bundle — SitkaLab.Core.dll, SitkaLab.Core.dll.sha256 and provenance.txt — into the already reserved Multi789Y2Src lane's .p0ref directory, with byte/hash/provenance verification and original files unchanged. No P0 source-tree export or rebuild is authorised.

## Phase B-01 Manager grant — CURRENTLY ACTIVE ONLY FOR EXPORTED P1 LANE FILES

**READ-ONLY technical map, no execution:** On the **new isolated** Multi789Y2Src lane at registered HEAD 4b81e71e, the orchestrator may dispatch one registered GPT-6.1 Sol/Codex technical source mapper after confirming active task ownership, clean lane and actual actor/provider. The mapper may read the existing **30 exported P1 tracked files and their pinned fixture bytes only**, plus Git metadata necessary to identify starting candidate, and may inspect P1-to-.p0ref project references and list the exact functions involved in dead-stem competition/CI target and neighbour, growth/crown/age, stand mean vulnerability, hazard rolls, density boundary and CLI call chain. Output a source-map Markdown under a new docs-only result branch or approved external evidence directory, not a source edit. Do not read original retained #725, any #710 source, #687, or currently missing P0 DLL, and do not touch #784/#770. Do not create test files, change code, run dotnet/restore/build/tests, copy more fixture bytes or mutate physical Ops. A broken/missing binary is a reported condition, not an excuse to self-fetch source.

**Source mapper must NOT mark the physical READY lane as C RUNNING**, because B-01 is read-only and does not own implementation. Submit actual B-01 map and any source-dependent allowlist/command differences for Manager adjudication before C implementation.

## Next source and Q authorisations (NOT yet signed)

Following additional P0 binary holder approval and verified three-file transfer, Manager will evaluate:
- **C sole writer:** one actual GPT-6.1 Sol source writer, on named lane with written [RUNNING] claim and official Ops transition. Proposed allowlist only src/Core/AnnualEngine.cs, src/Core/AnnualRules.cs, src/Cli/Program.cs, tests/Core.Tests/AnnualTests.cs, tests/Core.Tests/Program.cs, tests/Core.Tests/Runner.cs, plus NEW files under src/Core/ and tests/Core.Tests/, and three P1 .csproj only if compile reference requires. No P0/P1 fixture, golden, saved data, external API, global NuGet or unrelated tree edits. Original #725 remains immutable.
- **Offline commands:** individually authorised build and DLL tests, exact 45,980-byte Year1 golden ×2, original-domain/tamper/repeat refusal; proof network absent with unshare user+net namespace and local prepopulated NuGet; process-local HOME/TMPDIR/NUGET_PACKAGES/DOTNET_CLI_HOME/XDG cache on Files, not root. No unknown restore/network.
- **Independent reviewer and Q:** nonauthor distinct Claude reviewer before 1-to-2; later exact-candidate independent qualification with positive-pressure R-31/T-19 and RD boundary R-32/T-20 mutants and strict frozen Year1 regression.
No automatic grants from a clean worktree, technical B-01 report or an existing source-retention lease.

## RAM, storage and exclusions

Owner reports laptop lockups even without Unity. ONE heavy worker/build at a time; lightweight B-01 in browser/docs can run separately if it does not touch protected source outside the grant. Check fresh MemAvailable, swap, memory pressure and active heavy jobs before starting. New lane on Files ext4, previously 38 GiB free; verify remaining space before any build. No root-drive worktrees, substantial temp/log/build caches; do not touch unrelated dirty work or clean existing data. No #770 Scenario One or #784 Year0 Viewer, no Windows/Unity/Host, no main merge/public code, no GPT-6.2, no L4 waiver (FAIL38 UNWAIVED).

**User action:** approve exact three-file read-only P0 DLL reference transfer. All previously granted G0, normal agent posture, first source+fixture export and lane booking stay valid; do not ask again.

**Copy/paste dispatch (custodian, after owner authorisation):** Transfer only the exact retained P0 reference DLL, SHA sidecar and provenance from qualified #725 into the reserved Multi789Y2Src .p0ref, with immutable source and verified receipt. No source edits, builds, new source-tree export, root worktrees, Ops tool changes or other lanes. Publish one GitHub result and then let Manager authorise C/offline/Q.
