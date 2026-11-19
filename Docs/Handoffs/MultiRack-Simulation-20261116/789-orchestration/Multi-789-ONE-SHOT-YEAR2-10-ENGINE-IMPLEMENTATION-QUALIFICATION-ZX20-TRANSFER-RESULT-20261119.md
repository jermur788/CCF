# Multi #789 — one-shot Year2–10 implementation, qualification and ZX20 transfer: result

**19 November 2026, 09:35 UTC.**

**Engine:** implemented, independently reviewed, independently Q-qualified, archived and restore-tested. **Viewer:** archived with full history and restore-tested. **Originals:** untouched and retained.

| | |
|---|---|
| IMPLEMENTED | **yes** — private in-memory continuation, 10 sequential years |
| QUALIFIED | **yes** — independent Claude Opus 5.5 Q PASS on the frozen candidate |
| INTEGRATED | **no** — no main merge, no public push |
| ACCEPTED | **no** — Manager/owner acceptance is not claimed |
| RELEASED | **no** |
| L4 .NET | **FAIL38 UNWAIVED** |

## 1. Actors (actual, no substitutions)

| Role | Identity |
|---|---|
| Parent / coordinator | DeepSeek V4.1 Flash, `opencode-go/deepseek-v4.1-flash`, native OpenCode, `jer-laptop` |
| B-01 mapper | GPT-6.1 Sol, Codex, T3 Code |
| C writer (M-1) | GPT-6.1 Sol, Codex, T3 Code — primary Codex account |
| C writer (M-1 repair, Year2–10) | GPT-6.1 Sol, Codex, T3 Code — **second authenticated Codex instance** after the primary account hit its usage ceiling at 09:0x UTC (recorded in the commit that used it) |
| Independent M-1 reviewer | Claude Sonnet 5.5, Claude Code in T3 Code |
| Independent Q | Claude Opus 5.5, Claude Code in T3 Code |

No GPT-6.2 was used. When the primary Codex account was exhausted the same model was continued on the second authenticated account; that is a credential failover, not a model substitution, and it is disclosed here and in the commit.

## 2. Physical gates

- Files lease `Multi789Y2Src` at `/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/ccf-y2-10-src`, branch `feature/multi-789-year2-10-engine-20261119-v1`, booked at `4b81e71e` and transitioned `READY → ACTIVE` under an exclusive nonblocking `flock` on `/media/jer/ZX20/CCF-Ops/board` (board 45 rows, lint PASS, zero conflicts, board SHA-256 `f11db763…` → `4d76f9a3…`).
- Original #725 retained source `df467dcc` was read only by the earlier export and never modified; #710 untouched.
- Offline proof: `unshare --user --map-current-user --net`, loopback DOWN, DNS failure, process-local `HOME`/`TMPDIR`/`NUGET_PACKAGES`/`DOTNET_CLI_HOME`/`XDG_CACHE_HOME` on Files, cleared-source NuGet config. No package references exist, so restore needs no network.
- One heavy worker at a time throughout: no concurrent model, build or archive job.

## 3. B-01 read-only source map (folded in; #798 closes)

Mapped against the lane at `4b81e71e`, relative to `Tools/MultiRackSitkaP1Lab/`:

| Area | File:lines | What it does / boundary |
|---|---|---|
| CLI admission, Year1 | `src/Cli/Program.cs:56–90`, `src/Core/Domain.cs:19–34`, `src/Core/AnnualEngine.cs:45–57`, `FixtureAccess.cs:148–159` | 3/4-arg annual dispatch; domain parse before usage; `AnnualGate` yields `OriginalAnnualRefused`, `DomainUnavailable`, catalog/trust refusals in order |
| Fixture load | `FixtureAccess.cs:104–138` | ID/catalog/domain/status, bounded no-follow read, length, SHA, strict parse, embedded ID |
| Transaction fence | `AnnualEngine.cs:68–79` | Refuses any non-zero year; calculate-then-assign |
| CI | `AnnualRules.cs:15–18, 65–80` | All rows became targets; only same-ID exclusion as neighbour; **f32** CI, 8 m inclusive, 0.5 m floor |
| Growth / area | `AnnualRules.cs:82–97`; `DerivedManifest.cs:45–48,244–246` | Response `1/(1+CI/5)`, f32 storage, area bits `3db851ec` |
| Crown / age | `AnnualRules.cs:89–96`; `CcfSourcePins.cs:26–30` | Height from pre-increment age, crown from post-growth DBH, 0.15 relaxation, then age++ |
| Stand / mean S⁴ | `AnnualRules.cs:99–114` | Counts every supplied row; stems/ha is float then widened |
| Hazard | `AnnualRules.cs:27–44` | Positive pressure only; roll only when p>0; `(RNG model, "ADULT-MORT-"+id, year, seed)` |
| RD boundary | `AnnualRules.cs:117–138` | Strictly above threshold; descending suppression; ordinal tie-break; last survivor |
| Dead rows | `AnnualEngine.cs:7–15,20–25`, `AnnualRules.cs:16–18,29–32,35–55,120–137` | Archival representation; **all** the M-1 exposures |
| Tests | `ContinuationTests.cs`, `AnnualTests.cs`, `Runner.cs` | Custom runner, not `dotnet test` |

Allowlist check result: the C scope was structurally sufficient; no file outside it was required.

## 4. C changes

**M-1** (`a79e647`, `85870a7`, `b3b3ca1`): dead stems excluded as CI targets and neighbours, from before/post-growth stand metrics, from the mean-vulnerability sum **and divisor**, from hazard candidacy and rolls, and from RD-boundary survivor counting; growth guarded; archived rows retained with death metadata and ordinal order. CI remains f32; the `<=` boundary and last-survivor rule untouched.

**Year2–10** (`11f11076`, `343e8526`, `6f6547bd`, `4aa5128e`, `1feb0f9f`, `48d21d62`, `cef9bc83`, `818be11c`, `e03152c2`, `6f9d2e99`): new internal `AnnualContinuation` (48 lines) + 6 lines in `AnnualEngine` + `ContinuationTests` (148 lines). Immutable snapshot + revision; expected-year and expected-revision fences checked before any work; zero-living refused as `ContinuationNoLivingStems`; calculate-then-publish under a lock; test-only fault seam before publication. No public API, no CLI change, no persistence, no recruitment/work plan/Host. `src/Cli` is untouched, so the original command is byte-identical.

## 5. Independent M-1 review — APPROVED

Claude Sonnet 5.5, nonauthor, found **no source defect** and confirmed all seven exclusion points, then built mutants in scratch. First pass returned NEEDS CHANGES because four mutants survived the suite; the writer added assertions and the re-review confirmed each now fails:

| Mutant | Caught by |
|---|---|
| dead in before-growth stand | `before stand excludes archived stem: expected 2, got 3` |
| dead in mean-S⁴ divisor | `living-only mean S^4: expected 7.0807e-06, got 4.7204e-06` |
| dead in sum only / sum+divisor | same assertion, `0.0312571` / `0.0208381` |
| final ordinal `OrderBy` removed | `merged report has ordinal IDs` |
| dead in post-growth stand | T-20, `dead excluded: expected 2, got 3` |

The reviewer also re-ran the Year1 golden itself (45,980 B, `574d8746…`) and the original-domain refusal (exit 2, empty stdout).

## 6. Year2–10 outcome

50/50 tests, 45 existing + 5 new. Ten genuine sequential transitions, year 1..10 with revision 1..10, no jumps, byte-identical repeat runs. The committed fixture produced **13 deaths** in years 3, 6, 7, 8 and 9; each death's `deathYear`, post-growth age/dimensions, cause and mechanism are asserted, and every archived row is compared for complete equality in every later year. Synthetic evidence extends to year 12 (boundary death at year 1, frozen thereafter), proving the ten-year horizon is not a cap. Empty, archived-only and naturally all-dead stands refuse distinctly; singleton advances; stale year/revision refuse; concurrent duplicates commit once; injected failure leaves state and fences identical and a retry commits once.

Per-year continuation digests (from the author handoff; these are regression anchors, not independent proof):

| Year | Commit | SHA-256 (truncated) | Deaths |
|---|---|---|---|
| 1 | `11f11076` | `df2d44fd…` | 0 |
| 2 | `343e8526` | `23a1d889…` | 0 |
| 3 | `6f6547bd` | `953fc7ef…` | 1 |
| 4 | `4aa5128e` | `6fc220c3…` | 0 |
| 5 | `1feb0f9f` | `cbd51e38…` | 0 |
| 6 | `48d21d62` | `01cfb656…` | 1 |
| 7 | `cef9bc83` | `c6baab55…` | 2 |
| 8 | `818be11c` | `61879c1e…` | 4 |
| 9 | `e03152c2` | `faaea476…` | 5 |
| 10 | `6f9d2e99` | `10d31913…` | 0 |

## 7. Independent Q — PASS

Claude Opus 5.5, distinct nonauthor session, on frozen candidate `6f9d2e99`:

- isolation verified (`lo` DOWN, `DNS_FAIL`); harness byte-identical to the lane before and after;
- build 0 warnings/0 errors, **50/50**, identical across two processes;
- Year1 golden twice: 45,980 B, `574d8746…`; original-domain refusal exit 2, empty stdout;
- ten steps with fences; skipping ahead refused; two engines in step byte-identical;
- 13 deaths, all `self-thinning/hazard`, distinct IDs, correct `deathYear`, frozen thereafter, no re-death;
- zero-living refusal without mutation, singleton to revision 5, stale fences, 40×8-thread duplicate stress with exactly one winner, mid-sequence failure then one retry;
- M-1 mutants rebuilt: dead-in-before fails 4 tests, both normaliser mutants fail 2 each;
- scope audit: 5 files changed, all internal, no CLI/save/Host/recruitment code;
- full ten-year trajectory hash `cd5c1eb8…` (637,067 bytes) identical in two processes.

**Disclosed limits from Q:** no boundary death occurs in the ten fixture years (boundary freezing is covered only by the synthetic test); Q's own probe did not catch the normaliser mutants (the author's R-31 test and the year-4 digest did); seven environment-dependent tests cannot run in a scratch copy without git/P0; Year1 was compared against the expected digest rather than a pre-change rebuild; Unity not run and not relevant.

## 8. ZX20 transfer — created, verified, restore-tested

Destination `/media/jer/ZX20/CCF-Transfers/Multi-20261119/`, carrier exFAT UUID `18D6-896E`, **1,237,496,210 bytes (1.15 GiB)** total.

| Artefact | Bytes | SHA-256 |
|---|---:|---|
| `viewer/ccf-year0-viewer-20261119.gitbundle` | 1,202,316,983 | `9e6e2904…` |
| `viewer/ccf-year0-viewer-snapshot-20261119.tar` | 1,669,120 | `964f0209…` |
| `viewer/ccf-year0-viewer-evidence-20261119.tar` | 30,883,840 | `d17856e9…` |
| `engine/ccf-y2-10-engine-20261119.gitbundle` | 83,783 | `4733de42…` |
| `engine/ccf-y2-10-lane-snapshot-20261119.tar` | 2,232,320 | `e6b89e57…` |
| `engine/ccf-y2-10-evidence-20261119.tar` | 296,960 | `7a1fb72b…` |

`SHA256SUMS`, `MANIFEST.txt` (per-file bytes and hashes, carried heads, exclusions) and `RESTORE.md` (Linux/WSL2 steps, Windows notes, verification targets) are at the root. `sha256sum -c SHA256SUMS` on the carrier: **9/9 OK**.

Free space after copy: **341.41 GiB (71.6 % free)** against a 16 % floor — PASS with a very large margin.

### Restore rehearsal (ext4, required, and it found a real defect)

**Defect found and fixed:** the viewer source store is **shallow** (boundary `34185b8b`). A bundle taken directly from it advertises a complete history but omits `local/h0a-base`'s parent `55d4fb89`, so `git clone` fails. The first archive was therefore **not restorable**. It was rebuilt from a complete-history staging repository (main store + the three H0A refs), which is why it is 1.2 GB rather than 1.0 GB. Without the mandated rehearsal this would have shipped as a broken archive.

Rehearsal results on ext4 at `/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/restore`:

- `git bundle verify`: complete history for both units;
- viewer clone HEAD `c4124b63…`, worktree `f3537fe3…`, `git fsck --full` clean;
- engine clone HEAD `6f9d2e99…`, `git fsck --full` clean, 14 commits, working tree clean after the snapshot restore;
- `.p0ref/SitkaLab.Core.dll` = `9a99f781…` (120,832 B);
- package `year0-viewer.html` = `0b69cc1d…` (127,831 B); untracked fixture restored at mode **444**, 32,567 B, `bcaaa5c3…`.

**No deletion of originals.** This is a verified copy, not a move.

## 9. Deviations and disclosures

1. **Compile-linked public files.** The lane has no `Assets/`, but `SitkaP1.Core.csproj` compile-links `Assets/ForestPrototype/SitkaGrowthModel.cs` and `SimulationRandom.cs` from `$(CcfRepoRoot)`. They are byte-identical to public `origin/main` (`e4c73402…`, `bb83d7f8…`) and were staged from public source into a work directory; no protected bytes were read and nothing extra entered the lane. This is wider than the literal grant wording ("P1 + referenced P0 Core") and the grant text should be amended to include the two public compile inputs.
2. **Qualification harness.** The project's suite requires `Tools/MultiRackSitkaLab`, `Assets/` and commit `a7f41983` in history, none of which the narrow export carries. All builds and tests ran in a scratch full-repo checkout at that commit with the exact candidate mirrored in. The lane remained the source of truth and is byte-identical to the mirror.
3. **Credential failover** as described in §1.
4. **Q independence** is genuine: a different model, different session, nonauthor, with its own probe and mutant rebuilds.

## 10. Not done

No main merge, no public product-code push, no release, no Unity, no Windows build, no Host, no save/persistence, no recruitment/marking/work plan, no Scenario One or #770 contact, no #784 viewer source edit, no #710/#687 access, no bulk deletion, no L4 waiver, no GPT-6.2. Dashboard/Drive sync not verified.

## 11. Claims

#789 implementation/qualification/transport complete; #798 B-01 mapping complete and folded in; #730 remains the Manager record. The engine candidate is local and unintegrated: integration and acceptance remain separate decisions.
