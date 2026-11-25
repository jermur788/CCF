# CCF Multi 2D — Phase 1 Step 1 (Time Pacing, History Query & Woodland Structure Core): Independent Delta Re-Review R2

**Date:** 2026-11-25 (UTC). The lane name keeps the 20261124 task date.
**Verdict:** **APPROVED.**
**Target state:** **IMPLEMENTED ON TASK BRANCH — INDEPENDENTLY REVIEWED APPROVED — READY FOR STEP 2 UI — NOT INTEGRATED, NOT ACCEPTED.**

> **EXPLICIT STATE:** The feature is **NOT MERGED**, **NOT INTEGRATED** and **NOT OWNER-ACCEPTED**. This re-review ticks no row in `specs/multi-2d-phase1-time-history-core-v1/checklists/requirements.md`. It records evidence for the Overall Manager.

---

## 1. Reviewer identity and separation

| Item | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`, Anthropic), running in Claude Code under T3 Code |
| Role | Independent, non-author delta re-reviewer. No implementation role. |
| Implementer | Neon (`antigravity_2` / Gemini 4 Neon), a different provider and a different thread. Commit `5fe2fabe` carries the shared local Git identity `DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>`, not Neon's name. I accept the brief's attribution to Neon (see N-9). |
| Separation | I did not write, edit or advise on any spec, source, test or handoff in this feature or its repair. I spawned no subagents. |
| Writes | Only this file, committed on the R2 review branch. Gate builds wrote only to the review worktree's git-ignored `bin/`/`obj/`. Probes and mutations ran in a disposable `git clone --shared` at `/home/jer/ccf-p1th-r2-tmp/clone` (outside the repository). I deleted that clone at the end. |

## 2. Lane and HEADs

| Item | Value |
|---|---|
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistory-ReReview-R2-20261124` |
| Review branch | `review/multi-2d-phase1-time-history-core-20261124-v1-r2`, at `5fe2fabe` before this file |
| Repaired feature HEAD (under review) | `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (`feat/multi-2d-phase1-time-history-core-20261124-v1`; same SHA on `origin`) |
| Previously reviewed feature HEAD | `54761edc2ca0f66698182a309eb497ad318f34ea` (first review: NEEDS CHANGES, review HEAD `edd38568655604da655f4307858dc330a8db750b`) |
| Base | `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` |
| Toolchain | `~/.dotnet/dotnet` SDK `8.0.134`. System dotnet `10.0.115` (`/usr/bin/dotnet`) not used. |
| `TMPDIR` | `/home/jer/ccf-p1th-r2-tmp` (outside the repository) |

## 3. Delta scope

`54761edc..5fe2fabe` is one commit: `5fe2fabe test(core): close Opus F-1/F-2 Step 1 test gaps`. `54761edc` is an ancestor of `5fe2fabe`.

| Path | Change |
|---|---|
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs` | +39 / −57, mode `100644` unchanged |

No other path changed. There are no spec, handoff, Core source, Save.Tests, UI, Unity or coordination edits. `git diff --summary` is empty. The changes in the file are:

1. `using System.Diagnostics;` removed.
2. `BaseCommitSha` constant removed.
3. P1-TH-08 body replaced. The git working-tree/untracked scan became a hermetic built-code assertion, and the test name changed (see §5).
4. `VerifyWorkPlanOpenRefused` extended. Each of Pending and Approved now loops over targets `{current+4, current+6, current}` and asserts `StopReason`, `StopDetail`, `DestinationYear`, zero steps and a byte-identical canonical root for each target (see §6).
5. `FindRepoRoot` and `RunGit` helpers removed.

## 4. Start-of-review checks (run by me)

| # | Check | Result |
|---|---|---|
| 1 | `git rev-parse HEAD` | `5fe2fabe9073d7b72711f4ab48553c785351d7bc` — **PASS** |
| 1 | `git status --short` | empty — **PASS** (still empty after the gate run and at end of review, before this file was written) |
| 2 | `git diff --name-only 54761edc..5fe2fabe` | exactly `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs` — **PASS** |
| 3 | `git diff 54761edc..5fe2fabe` | inspected in full; summarised in §3, §5 and §6 — **PASS** |
| 4 | `git diff --name-only 5438c94c..HEAD` | 12 paths, the same 12 as at `54761edc`. Paths under `src/Ui/`, `src/Core/Phase1/`, `Assets/`, `Packages/`, `ProjectSettings/` or `Docs/Project/coordination/` (lab-relative or repo-relative): **0** (grep exit 1) — **PASS** |
| 5 | `sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` — **PASS** at start and end. The directory and files are read-only. The DLL was never rebuilt. |
| 5 | `origin/main` (`git ls-remote`) | `cc2757a730ad9451e3d6e3680dffd123171a5df5` — **PASS** at start and end (untouched) |
| 5 | Listeners `ss -ltnp` | `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361), `:46593` (pid 839989): all `LISTEN` at start and end. Each answered HTTP (404 on `/`). I never stopped them. — **PASS** |

## 5. F-1 verification (major: git-dependent P1-TH-08) — **CLOSED**

**Removal confirmed (from the diff and a grep of the file at `5fe2fabe`):**

- The old git-dependent P1-TH-08 body is gone, as are `BaseCommitSha`, `FindRepoRoot` and `RunGit`, and `using System.Diagnostics;`.
- A grep of `Phase1TimeHistoryTests.cs` for `Process|git|Environment\.|Directory|File\.|AppContext|BaseDirectory|Path\.` returns **no match**.

**The replacement is hermetic.** The new P1-TH-08 is named "Core boundary stays hermetic (SchemaVersion 2, zero UI/ASP.NET/Unity references)". It reads only:

- the compile-time constant `Phase1SaveFormat.SchemaVersion`;
- `typeof(MultiAreaContinuation).Assembly.GetReferencedAssemblies()`, which is metadata of the already-loaded `SitkaP1.Core` assembly;
- three `typeof(...).Assembly` identity comparisons.

It uses no git, subprocess, file system, working-tree state, environment variable, path or historical commit.

**Untracked-UI negative probe (disposable clone, not the review lane):**

| Run | Clone state | Result |
|---|---|---|
| R0 control | unmodified `5fe2fabe` | build 0 W / 0 E; **277/277 passed, 0 failed** |
| P1 probe | untracked `src/Ui/REVIEW-R2-PROBE-UNTRACKED.txt`, plus one comment line appended to tracked `src/Ui/Program.cs` (`git status`: ` M …/src/Ui/Program.cs`, `?? …/REVIEW-R2-PROBE-UNTRACKED.txt`) | build 0 W / 0 E; **277/277 passed, 0 failed** — **PASS** |
| P1-OLD contrast | same probe state, with only `Phase1TimeHistoryTests.cs` swapped back to its `54761edc` version | **276/277**: `FAIL P1-TH-08: zero changes under src/Ui/, … — forbidden src/Ui/ edit: Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs` |

After the probe I removed the untracked file and ran `git checkout` on `Program.cs` and the test file. The clone was then clean against HEAD (`git diff --quiet HEAD`, empty status).

**The replacement asserts something real (liveness probes in the same clone, each with a clean `--no-incremental` build and then a restore to clean):**

| Run | Mutation | Result |
|---|---|---|
| P08-A | `SitkaP1.Core.csproj` gains `<FrameworkReference Include="Microsoft.AspNetCore.App" />`, plus one internal `typeof(Microsoft.AspNetCore.Http.HttpContext)` use | **276/277**: `FAIL P1-TH-08 … Core must not reference ASP.NET Core: Microsoft.AspNetCore.Http.Abstractions` — **KILLED** |
| P08-B | `Phase1SaveFormat.SchemaVersion = 2` → `3` | **275/277**: `FAIL P1-TH-04 … save schema version stays 2: expected 2, got 3` and `FAIL P1-TH-08 … save schema version remains 2: expected 2, got 3` — **KILLED** |

The actual referenced-assembly list of the unmodified `5fe2fabe` `SitkaP1.Core.dll` (read with `dotnet fsi`, `Assembly.LoadFile(...).GetReferencedAssemblies()`) is: `System.Runtime`, `System.Collections`, `System.Text.RegularExpressions`, `SitkaLab.Core`, `System.Text.Json`, `System.Linq`, `System.Text.Encoding.Extensions`, `System.Collections.Concurrent`, `System.Security.Cryptography`, `System.Threading`, `System.Memory`, `System.Runtime.InteropServices`. That is 12 entries, so `referenced.Length > 0` is meaningful. None of them matches `Microsoft.AspNetCore*`, `SitkaP1.Ui`/`sitkap1-ui`, `UnityEngine*` or `UnityEditor*`. The test checks each prefix, case-insensitively.

**Path-scope proof** stays where the first review placed it: the reviewer/integration gate. My check 4 (§4) is that gate for `5438c94c..5fe2fabe`.

## 6. F-2 verification (minor: WorkPlanOpen label and ordering unasserted) — **CLOSED**

**Code reading (`Phase1TimeHistoryTests.cs:884-918` at `5fe2fabe`):**

- The Pending and Approved blocks each loop over `{year+4, year+6, year}`. `year` is read from the view (`vPending.Source.EcologicalYear` and `vApproved.Source.EcologicalYear`).
- For every target, each block asserts:
  - `T.Eq(TimePacingStopReason.WorkPlanOpen, r.StopReason, …)`, which rules out `InvalidTargetYear` for `+6` and `+0`;
  - `T.Eq(MultiAreaTimePacingConstants.WorkPlanOpenLabel, r.StopDetail, …)`, an exact string equality;
  - `!ReachedTarget && CompletedYears.Count == 0 && DestinationYear == year`;
  - `AssertBytes(before, Canonical(session), …)`, a byte-identical canonical root.
- This runs for all 6 order families through P1-TH-03a: 36 `AdvanceYears` refusals plus 12 previews. The previous assertions (Pending and Approved, `+4`, `WorkPlanOpen`, zero steps, zero mutation, preview blocked) are all still present. The new set is a strict superset.

**M02 reproduction (disposable clone).** The cited guard is `src/Core/MultiArea/MultiAreaTimePacing.cs:257`, `if (MultiAreaTimePacing.HasOpenWork(root))`. The match is a single occurrence; line 166 is the separate `Preview` guard and was not touched. The mutation is the single replacement `if (MultiAreaTimePacing.HasOpenWork(root) && startYear < 0)`. This condition is never true but not constant, so it builds under warnings-as-errors.

| Run | Source | Tests | Result |
|---|---|---|---|
| M02 | guard disabled | repaired (`5fe2fabe`) | build 0 W / 0 E; **276/277, 1 failed**: `FAIL P1-TH-03a … Fell Pending StopDetail (target 4): expected Work plan open — execute, cancel, or remove all pending or approved work before skipping ahead., got WorkPendingConsentRequired: advancing the year with unexecuted work needs explicit consent to cancel all of it at zero Fell` — **KILLED** |
| M02-OLD contrast | guard disabled | `54761edc` version | **277/277** — survives, reproducing the first review's F-2 observation |
| M02b (isolates ordering) | guard disabled **and** the in-loop `WorkPendingConsentRequired` refusal relabelled to `WorkPlanOpenLabel` (`:338`), so the label assertion alone cannot catch it | repaired | **276/277, 1 failed**: `FAIL P1-TH-03a … Fell Pending advance refused (target 6): expected WorkPlanOpen, got InvalidTargetYear` — **KILLED** |

M02 shows that the label assertion kills the cited mutation. M02b shows that the `+6`/`+0` ordering assertions independently kill the subtler variant, in which the in-loop refusal is relabelled. The clone was restored to clean after each run.

## 7. Gates rerun by me on `5fe2fabe` (exact commands and counts)

```bash
export PATH="$HOME/.dotnet:$PATH"
export TMPDIR=/home/jer/ccf-p1th-r2-tmp
mkdir -p "$TMPDIR"
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistory-ReReview-R2-20261124/Tools/MultiRackSitkaP1Lab
dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers --no-incremental   # --no-incremental added for a guaranteed fresh compile
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj
dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers
# extra, because `dotnet run` does not print build warnings:
dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers --no-incremental
```

| Gate | Exit | Observed result |
|---|---|---|
| `.sln` build | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Core suite | 0 | **277/277 passed, 0 failed**. All ten `P1-TH-*` PASS, including `PASS P1-TH-08: Core boundary stays hermetic (SchemaVersion 2, zero UI/ASP.NET/Unity references)`. No `FAIL` line. |
| Save suite (`dotnet run`) | 0 | **50/50 passed, 0 failed**; `scratch removed: True` |
| UI build | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| Save tests project build (extra) | 0 | **Build succeeded — 0 Warning(s), 0 Error(s)** |

Run window: 2026-11-25T01:27:39Z – 01:28:57Z. The Core count is unchanged at 277 because P1-TH-08 was replaced in place. I confirm the author's reported counts: 277/277 and 50/50.

## 8. Nine frozen anchors (observed in my Core run)

| # | Anchor | Printed line in my Core log | Result |
|---|---|---|---|
| 1 | Single-area Model 0 | `model0 single-area fingerprint 7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | **EXACT** |
| 2 | Multi-area `0/0/0/0` | `model0 multi-area fingerprint a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | **EXACT** |
| 3 | Multi-area `0/0/1/0` (C2) | `model1 multi-area fingerprint 814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | **EXACT** |
| 4 | Stage 4 `1/1/1/0` | `S4 stage4 run fingerprint` and `S5 1/1/1/0 full-run fingerprint` `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | **EXACT** |
| 5 | Stage 5 `1/1/1/1` | `S5 1/1/1/1` and `P2-01 1/1/1/1/0 full-run fingerprint` `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | **EXACT** |
| 6 | Stage 5 5.0 m purity | `S5-07A 5.0 m purity fingerprint c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | **EXACT** |
| 7 | Phase 2 Q6 | `P2-03 M-08 Q6 hash 1/1/1/1/0 776a68df…ce22d \| 1/1/1/1/1 776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | **EXACT** (compared by me against the spec value; the suite asserts only h0 == h1) |
| 8 | P0 Core binary pin | `P2-01 P0 Core binary 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, plus `sha256sum` at start and end | **EXACT** |
| 9 | MA-1-MIXED 30-year | `P2-10 MA-1-MIXED-v1 30-year fingerprint 93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | **EXACT** (compared by me; the suite asserts only run1 == run2) |

All nine anchors are unchanged. That is expected, since the delta touches no source.

## 9. Residual check

- **No other test weakened.** The diff touches only the P1-TH-08 body and `VerifyWorkPlanOpenRefused`. The latter now asserts a strict superset of what it asserted before. No test was removed, renamed (apart from the P1-TH-08 description string) or disabled, and the count stays 277. In the first review, every killed mutation (M01, M03, M05–M12, M14–M18) was caught by P1-TH-01/02/03a/03b/05/06, never by P1-TH-08. Those assertions are unchanged or extended, so the kill set cannot shrink. I re-ran M02 and its M02b variant only (see §11).
- **No source behaviour change.** The delta contains no file under `src/`, and the nine anchors are exact.
- **Spec quartet and handoff claims unchanged.** `git diff --name-only 54761edc..5fe2fabe -- specs Docs` is empty.
- **No new environment dependence.** The repaired file uses no git, process, file system or environment access (§5 grep, P1 probe). The wider Core suite still contains pre-existing git-dependent P0/source-trust tests (the `Git` helper in `tests/Core.Tests/Program.cs`). Those predate base `5438c94c` and are outside this delta.
- **First-review nits N-1..N-7** are informational and stay open as recorded there. They were not required.

**New informational notes (none blocking):**

| ID | Severity | Observation | Suggested owner action |
|---|---|---|---|
| N-8 | nit (documentation drift) | Three documents still describe the removed mechanism: the implementation handoff at `54761edc` (`PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md`), its row `P1-TH-08` ("zero tracked or untracked changes under `src/Ui/` …"), and its risk 3. There is no in-tree Markdown repair handoff for `5fe2fabe`. This was expected, because the brief required a tests-only delta. Spec §8 and checklist row P1-TH-08 still define *scope compliance*. That criterion is satisfied by the reviewer gate (§4 check 4), not by the test named P1-TH-08, which now proves the hermetic Core boundary. | The Manager records the repair (5fe2fabe, +39/−57, F-1/F-2 closure) in coordination or the integration record. P1-TH-08 checklist evidence should cite the reviewer/integration `git diff --name-only 5438c94c..5fe2fabe` gate. Once Step 2 commits stack, that path gate must be applied to the Step 1 range only. |
| N-9 | nit (provenance) | The commit author on `5fe2fabe` is the shared local identity `DeepSeek V4.1 Flash orchestrator`, not Neon. Git alone does not record the implementer's identity. | Implementer attribution relies on the T3/coordination record. |
| N-10 | nit (coverage boundary) | `GetReferencedAssemblies()` lists only direct metadata references that are actually used. A project or package reference with no type use, or a transitive reference through the pinned `SitkaLab.Core.dll`, would not appear. This is acceptable: an unused reference has no coupling, and P0 is pinned by hash. | None. |

**Count by severity (this delta):** blocker 0 · major 0 · minor 0 · nit 3 (informational).

## 10. What I verified myself vs what I accepted

**Verified myself:**

- Everything in §3–§9: start checks, the full delta diff, all four gates plus the Save project build, all nine anchors, the untracked-UI probe and its old-test contrast, the P08-A/P08-B liveness probes, the M02 / M02-OLD / M02b mutations, and the referenced-assembly listing.
- End-of-review state (§12).

**Accepted without independent proof:**

- The implementer's identity (Neon), as given in the brief.
- The first review's §5–§7 evidence for unchanged source behaviour (P1-TH-01..07, G2). Source is byte-identical to `54761edc` for every path except the test file, so that evidence still applies.

## 11. NOT TESTED

- **The other 16 first-review mutations (M01, M03–M18):** NOT RE-RUN. I judged them unaffected because the assertions that killed them are unchanged or extended (§9).
- **Multi-threaded concurrency:** no stress test (as in the first review).
- **UI/HTTP/browser:** Step 1 has none. No server was started; the protected listeners were only observed.
- **Unity Editor/runtime:** not applicable and not run.
- **Integration onto `main`:** not simulated. This belongs to the Manager.
- **Save suite under mutation:** not run. No probe touched Save-only behaviour, apart from P08-B, which I checked through the Core suite only.

## 12. End-of-review state (2026-11-25T01:40:12Z)

- Review worktree HEAD `5fe2fabe9073d7b72711f4ab48553c785351d7bc`. `git status --short` was empty before this file was written.
- `.p0ref/SitkaLab.Core.dll` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (read-only directory, never rebuilt).
- Listeners `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361) and `:46593` (pid 839989) all `LISTEN`.
- `origin/main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5`; `origin/feat/…-v1` = `5fe2fabe9073d7b72711f4ab48553c785351d7bc`. Neither was touched.
- Disposable clone `/home/jer/ccf-p1th-r2-tmp/clone` deleted. It was never registered as a worktree. Only logs remain in `/home/jer/ccf-p1th-r2-tmp/` (1.4 MB, outside the repository).

## 13. INTEGRATION DECISION

```text
INTEGRATION DECISION
Verdict:          APPROVED
Target state:     IMPLEMENTED ON TASK BRANCH — INDEPENDENTLY REVIEWED APPROVED — READY FOR STEP 2 UI — NOT INTEGRATED, NOT ACCEPTED
Feature:          MULTI-2D-PHASE1-TIME-HISTORY-CORE-V1 (SPEC-multi-2d-phase1-time-history-core-V1)
Reviewed HEAD:    5fe2fabe9073d7b72711f4ab48553c785351d7bc (feat/multi-2d-phase1-time-history-core-20261124-v1)
Previous HEAD:    54761edc2ca0f66698182a309eb497ad318f34ea (first review NEEDS CHANGES, edd38568)
Base:             5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43
Delta:            1 commit, tests only: tests/Core.Tests/Phase1TimeHistoryTests.cs (+39/−57)
F-1 (major):      CLOSED — git-dependent P1-TH-08 and helpers removed; hermetic replacement; untracked-UI probe 277/277
                  (old test 276/277); replacement live (ASP.NET-ref probe and SchemaVersion-3 probe both fail it)
F-2 (minor):      CLOSED — StopDetail label + {+4,+6,+0} ordering asserted for Pending and Approved; M02 now KILLED
                  276/277 (P1-TH-03a), M02b ordering variant KILLED 276/277; M02 vs old tests survives 277/277
Gates (reviewer): .sln 0W/0E; Core 277/277; Save 50/50; Ui 0W/0E; Save project 0W/0E; 9/9 anchors exact;
                  .p0ref 9a99f781…34b3; listeners 33767/43649/46593 alive; base..HEAD forbidden paths 0.
New findings:     0 blocker · 0 major · 0 minor · 3 informational nits (N-8 doc drift, N-9 provenance, N-10 coverage boundary)
Merge/PR/tag:     NONE performed. main and the feature branch untouched.
State:            NOT MERGED · NOT INTEGRATED · NOT OWNER-ACCEPTED
```

The feature is **NOT MERGED**, **NOT INTEGRATED** and **NOT OWNER-ACCEPTED**.
