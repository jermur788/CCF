# Multi #725 — Adult Sitka Year 1 implementation result

**18 November 2026 · IMPLEMENTED locally · Tier 1 author checks recorded · Independent review pending**

The accepted 30 × 30 m synthetic stand advanced from Year 0 to Year 1. All 100 adult Sitka stems, including the 12 initially below 170 mm DBH, grew and aged from 30 to 31. Relative density rose from 0.609795828454288 to 0.6375836886879149. All 100 hazard rolls were evaluated and all stems survived. This is the source-derived 30 m experiment. Irish field forecasting and Unity numerical and geometry parity are outside its biological claim.

Default .NET L4 remains **FAIL38**, unwaived. The development result has no release qualification, independent verification, integration or acceptance verdict.

## Worker, authority and source custody

| Item | Readback |
| --- | --- |
| Sole worker | OpenAI Codex GPT-6.2 Sol High (`gpt-6.2-sol`, High reasoning), configured OpenAI Codex provider, native Linux T3 Code, `jer-laptop` |
| Session | Codex `01a1ed7e-ee03-7002-b47a-3c52812b7abd`, T3 `c3da2c3f-eee5-4e2c-9545-3ce9cfa244d4` |
| Authority | [Complete #725 packet](https://github.com/jermur788/CCF/blob/docs/multi-694-723-stagea-dev-policy-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-725-COMPLETE-APPROVED-F0-ANNUAL-SOURCE-CUSTODY-AND-IMPLEMENTATION-PACKET-20261118.md), [approved F0 P1 packet at 90583811](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md) |
| Grant and start | [Custodian grant](https://github.com/jermur788/CCF/issues/725#issuecomment-6971011854), [locked readback](https://github.com/jermur788/CCF/issues/725#issuecomment-6971022079), [Manager #694 approved start](https://github.com/jermur788/CCF/issues/725#issuecomment-6971077917), [actual High run registered before source edits or build](https://github.com/jermur788/CCF/issues/725#issuecomment-6972017648) |
| Lane and branch | `MultiAnnualF0Child725`, `feature/multi-p1-f0-annual-725-20261118-v1` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-725-F0Annual-20261118/ccf-725-src` |
| Local isolated store | `/media/jer/Files/CCF/Operations/CCF-725-F0Annual-20261118/git/CCF-725-F0Annual.git` |
| Local source commit | `df467dcc302eca564343e6da40f2fb88529a5d16` |
| Result tree | `4237ece5c4efcd167857c1cb29e16bbcc2279ad6` |
| Sole parent and starting HEAD | `35d26f5e64a00a400c700871927ccc2173370568` (R tree `33f84db2e7fdac527c343a758775757f49687c42`) |
| P1 subtree | `f05430dae612541b96512ed7230903aa0e6dd4cd` |
| Physical lease | Files/ext4 UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, retained through **20 November 2026, 18:00 UTC**, custodian-managed |
| Durable evidence | `/media/jer/Files/CCF/Verification/CCF-725-F0Annual-20261118/` |

The High run resolved the earlier Medium configuration blocker. Live preflight confirmed clean R, the dedicated common directory, no alternate or hardlinked Git objects, and no conflicting writer. The board parser accepted 41 task rows and the configuration parser accepted 23 lanes. The board row was set to `ACTIVE` under the shared exclusive flock with candidate lint and readback. `lane-status --preflight` reported its documented isolated-store warnings, “worktree belongs to unexpected repository” and “not registered in configured repository”. Direct identity and disjointness checks establish custody for this approved exception.

## Source changes

Paths below are relative to `Tools/MultiRackSitkaP1Lab/`.

| Path | Change |
| --- | --- |
| `src/Core/AnnualEngine.cs` | New trust-gated entry point, immutable snapshots and one locked in-memory transaction. Only the accepted availability sentinel permits entry to the existing bounded, no-follow manifest reader. A failure before publication leaves the same state live. A second advance on the same engine refuses `AnnualAlreadyAdvanced`. |
| `src/Core/AnnualRules.cs` | New adult rule adapter. Ordinal pre-growth Hegyi CI uses the inclusive 8 m cutoff and 0.5 m floor. Stems age and grow before crown relaxation, stand density, conditional seeded hazard and the survivors-only boundary pass. The adapter links the existing `SitkaGrowthModel` and `SimulationRandom` through the existing project. |
| `src/Cli/Program.cs` | Narrow `annual` dispatch. The accepted fixture takes an explicit derived fixture directory and emits a JSON report to stdout. Both the existing three-argument form and the new form preserve the original-domain refusal. |
| `tests/Core.Tests/AnnualTests.cs` | Ten new annual tests covering the frozen stand, perturbations, transaction failure and retry, concurrent callers, admission, growth and mortality edge cases. |
| `tests/Core.Tests/Program.cs` | Registers the new tests and clarifies the retained `AnnualGate` sentinel test. Its original assertions remain in place. |

The report carries all per-ID before and after states, neighbour counts and pre-growth CI, stand metrics before growth, after growth and after mortality, and ordered deaths. Death records carry year 1 and biological cause `self-thinning`, with `hazard` or `boundary` as the mechanism. The hazard uses pre-growth suppression and post-growth density, applies the source probability cap, and draws only when probability is positive. The boundary uses descending suppression then ordinal ID and preserves the last survivor of that pass. The internal test vectors exercise this path without expanding the catalog or accepting caller-provided state into the public engine. Each CLI invocation loads a fresh genesis for a new experiment. In-memory state has a one-year lifetime.

## Numerical comparison

Comparison source is [#723, pinned at 8890a730](https://github.com/jermur788/CCF/blob/8890a730d349cb7082be34c213503055b8059b1f/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-723-FIRST-YEAR-IMPLEMENTATION-READINESS-AND-OVERNIGHT-RESULT-20261118.md). The build uses SDK 8.0.133 and the linked source on Linux x64. The adapter uses source float operands and storage points, ordinal reductions, binary64 squared-DBH accumulation and the declared binary32 stand area. The displayed reference provides a rounding interval for comparison. Exact comparisons cover IDs, ages, pair count, outcomes and the 24-bit RNG numerators.

| Quantity | Compiled result |
| --- | ---: |
| Initial / final living stems | 100 / 100 |
| Initial stems/ha | 1111.111083984375 |
| Initial / post-growth Dq (cm) | 25.303288374529917 / 25.855791791844318 |
| Initial / post-growth BA/ha (m²/ha) | 55.87290918844456 / 58.33954510260964 |
| Directed Hegyi pairs | 1,622 |
| Maximum CI | 8.432238578796387 at P30-053 |
| Initial / post-growth RD | 0.609795828454288 / 0.6375836886879149 |
| Post-growth pressure | 0.09395922171978738 |
| Mean vulnerability | 0.035410183953663112 |
| Maximum hazard | 0.0030975518881867859 at P30-053 |
| Sum of hazards | 0.070626682769505345 |
| Hazard / boundary deaths | 0 / 0 |
| Closest survivor margin | 0.02139606510860858 at P30-032 |

All 800 displayed per-ID values fell within their reference rounding intervals. Ages and survival matched for all 100 stems. All 100 roll numerators matched an independent integer transcription. P30-001 model 1 gave **2780209** and its model-0 negative control gave **14399697**. The mean vulnerability, max hazard and hazard sum differed from #723 by −2, +3 and +2 binary64 ULPs respectively. The mean and max match #723 §5.6's sequential cross-check. Two fresh processes emitted byte-identical JSON with SHA-256 `574d87461af8ff2cb3559723c62f376f4339971728b0613c6aa069fe8bb5dd53`.

The initial test compared the adapter's BA/ha of `55.87290918844456` against `55.872911409` and failed. Computing BA/ha from the same starting DBHs with the exact decimal area 0.09 reproduces the #723 value. Using the manifest's actual area `3db851ec` reproduces the adapter result. The test now converts the displayed reference to the declared-area basis with its original tolerance. The production calculation stayed the same. Both test logs are retained.

## Tier 1 verification

Run from `Tools/MultiRackSitkaP1Lab` in the retained worktree. The qualification used the reserved Files lane for CLI home, NuGet and temporary storage.

```sh
build/build-p0-core-ref.sh
dotnet build MultiRackSitkaP1Lab.sln --nologo -v minimal --disable-build-servers
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
dotnet src/Cli/bin/Debug/net8.0/sitkap1.dll annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 fixtures/derived
```

- Final build passed with **0 warnings / 0 errors**. P1 test executable passed **43/43**, comprising 33 compatibility checks and 10 annual tests. The first run passed 41/42 and exposed the BA reference-area difference described above. The isolated P0 reference build produced expected DLL SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.
- Separate CLI checks passed for two clean Year 1 runs, six exit-2 refusals with empty stdout, missing fixture-directory usage (64), original read-only inspection, derived trust, and generator-to-frozen-fixture comparison. The six refusals were original domain with and without the directory argument, Unity-square domain, unknown case-sensitive domain, unknown fixture ID and same-size changed fixture bytes. Retained manifest and gate tests cover pending and mismatched trust, wrong model/year, duplicate ID/position, dead stem and malformed data.
- Controlled in-memory tests passed for exactly 8 m, the next representable distance above it, the 0.5 m distance floor, DBH/height/crown clamps, pressure below onset, zero-probability roll skipping, the 0.5 hazard cap, chronological growth before death, disjoint hazard and boundary victims, ordinal boundary ties, suppression priority, retention of the last boundary survivor even above RD 1, and complete stand loss when all stems are hazard victims. These include scalar and tiny-area probes that cannot enter through the trusted annual CLI.
- An injected error after calculation and before publish preserved the identical Year 0 snapshot and tree array. A retry committed once and matched a clean run. A repeat after success refused, and a concurrent pair of advance calls committed one year. Seed and year perturbations changed the roll, and a DBH perturbation changed CI and density while preserving the roll key.
- The 98-test P0 campaign, full default-runtime L4 campaign, Unity/Windows, cross-platform numeric equivalence, save/load, forestry interventions and later ecological years were outside this Tier 1 run. The frozen fixture's zero deaths establish its outcome. The death-path evidence comes from the controlled in-memory tests.

## Protected data and security

P0 L0 and L1 passed from the exact qualified pin `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`. All 52 P0 blobs and modes match tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`. L2 and L3 had equal empty pre/post status and ignored metadata receipts. L3+ matched every tracked P0 SHA-256, mode, size and modification timestamp. Of the R baseline, 5,939 tracked files outside the two authorised edits matched their preflight SHA-256. The committed diff contains exactly the five paths listed above.

The accepted derived fixture SHA-256 is still `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`. This covers the exact frozen fixture bytes. The trust catalog, `AnnualGate`, strict analyzer and all four CCF source pins match R and the retained tests. The original #710 refs and clean worktree status matched post-run readback. Primary's two user-dirty file hashes matched the custody receipt. Source, artifacts and test work stayed in the approved lane and evidence tree.

**Default L4 remains FAIL38.** A single accepted `annual` invocation under the unchanged strict analyzer and existing traced syscall set reproduced byte-identical stdout, zero protected hits and zero failed protected attempts. Its analyzer **failed** with two unresolved operations, one `ftruncate` on `memfd:doublemapper` and one `fchmod` on a socket. Both match the documented default runtime classes. That narrow trace does not rerun or qualify the full L4 campaign. Runtime defaults and the analyzer were preserved. [Manager limited-development policy](https://github.com/jermur788/CCF/blob/docs/multi-694-723-stagea-dev-policy-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-694-MANAGER-723-STAGEA-ACCEPTANCE-AND-LIMITED-DEVELOPMENT-POLICY-20261118.md)

## Retention and next action

The isolated branch is clean for tracked and untracked source. The built reference, P1 build outputs and the lane's local caches remain available for review. Disposable test fixtures and the extracted P0 build tree were removed by their owning test/build scripts. No build servers were left running. Durable evidence includes `handoff/source-identity.txt`, `handoff/local-source.patch`, `tests/core-tests.txt`, both CLI JSON reports, `tests/oracle-comparison.txt`, `tests/per-id-comparison.csv`, and the `guards/` receipts and trace. Source custody remains reserved through the approved expiry. This GitHub branch carries this report only. Dashboard synchronization is unconfirmed and remains an operational follow-up.

Manager #694 should inspect this result and arrange the one focused independent changed-lines and numerical review under a separate read-only grant. The local source commit and default L4 status remain the review inputs.
