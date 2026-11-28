# Part 1 integration receipt | Natural Old-Age Tree Death and Smooth Late-Life Growth

**Integrated and verified on GitHub** on 2026-11-28. Codex Medium, existing single-writer `integration/2d-sim` lane. [Manager G4 packet](https://github.com/jermur788/CCF/blob/4cbdcfe6cc4a57ed578e140f84af416a599f4d80/Docs/Handoffs/MultiRack-Simulation-20261116/senescence-late-growth/MANAGER-INTEGRATION-PACKET-PART1-20261128.md) and [independent Part 1 review](https://github.com/jermur788/CCF/blob/a96c9c6cc4390e4c5d833fe936962c27c1227cf5/Docs/Handoffs/MultiRack-Simulation-20261116/senescence-late-growth/INDEPENDENT-REVIEW-PART1.md).

## Admission and commit history

- The sole registered checkout of the integration branch was clean at `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Integration-20261125`. The owner confirmed no other integration was running. The freshly checked default-branch coordination file contained no matching lane or feature entry. The integration Git directory and branch ref had no competing Git locks, and the process check found an idle shell in the checkout. The local writer registered `[RUNNING]` at 13:09:23 UTC under an exclusive `flock` at `.git/integration-2d-sim.writer.flock`. The unrelated untracked files in the separate primary checkout were preserved.
- Merge commit `07f91fb425467274a538a8bfa887f4bd40082712` has parents `525697fda1fb2f981ba56a92d68bc55f3f2e1b2a` (baseline) and `693826a11e911312d96df62d83ec26c4dbd9bb67` (approved feature), in that order. The common ancestor was `b2006767cbe70fa2c822497470908251a0fd6ffd`. The 10 baseline-side paths and 19 feature additions were disjoint.
- Final tested and pushed commit `0668b066ce6f6bb4f7c22b47667341888cf59782` has the merge as its sole parent. Its diff from the baseline is exactly 19 byte-identical approved additions and two authorized test-file modifications. The worktree was clean after qualification. [Integration commit](https://github.com/jermur788/CCF/commit/0668b066ce6f6bb4f7c22b47667341888cf59782)

## Test isolation correction

- `Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/SitkaP1.NaturalProcesses.Tests.csproj` adds `<Compile Remove="../../../src/Core/NaturalProcesses/Senescence/**/*.cs" />` after the existing recursive inclusion. The existing standalone executable continues to compile its prior source set. The new sources have their own Senescence executable and depend on Species Wave 2 sources.
- `Tools/MultiRackSitkaP1Lab/tests/Core/NaturalProcesses/HygieneAndParityTests.cs` updates JW-T64 to require the existing include and the exact removal as its two compile items. The project, package and binary reference assertions and all other NaturalProcesses assertions were retained. [Compatibility diff](https://github.com/jermur788/CCF/commit/0668b066ce6f6bb4f7c22b47667341888cf59782)

## Minimum Necessary Testing

All checks ran on the final candidate with SDK `8.0.133` on WSL Ubuntu. The existing protected P0 tree was `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`. The P0 Core DLL, checksum record and provenance matched `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.

| Check | Result |
| --- | --- |
| `dotnet clean` and `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | PASS, build 0 warnings and 0 errors |
| Core test executable | 326/326 |
| Frozen-anchor verifier from the committed G3 gate, using the same Core stdout | 9/9 |
| Standalone Senescence Release build and executable | 42/42, pinned digest `313e0f5a795bd7774752affb673927cc875bc6d89b9b988ddaadf3efa4e4bbb6` |
| Standalone NaturalProcesses Release build and executable | 49/49, including JW-T64 |

Browser, Unity, Windows, safety-script self-tests, unrelated suites and another independent engine review were outside this packet. Part 1 is dormant library code. AnnualPipeline, RngSalts, saves and UI wiring are deferred to a separately authorized Part 2. Mortality rates remain modelling choices.

## Publication and lane release

GitHub advertised the exact expected integration baseline immediately before the normal push. Git ref readback and the GitHub API confirmed the final candidate on `integration/2d-sim` and the unchanged protected `main` at `bdeb6a324acaf4129c10fb0846faac8b64974360`. The integration writer released the local lease normally at 13:11:15 UTC and a nonblocking acquisition confirmed it was free. The lane is released for Manager routing.

Raw evidence is local at `/home/jer/Development/CCF-Senescence-P1-Integration-20261128.vRHuN5`. Dashboard synchronization is outstanding. The accessible publisher had zero workflow runs and its project update did not reflect this integration.
