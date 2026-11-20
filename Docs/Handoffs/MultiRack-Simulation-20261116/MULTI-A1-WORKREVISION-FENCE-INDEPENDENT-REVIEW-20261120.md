# MULTI-A1-WORKREVISION-FENCE — INDEPENDENT REVIEW

**Verdict: APPROVED**, for exactly these two scopes:

1. **Source review of the exact candidate** `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`.
2. **Tier-2 delta qualification on `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`.**

This verdict does **not** state or imply integration, merge to `main`, Unity verification, UI wiring, Stage-5 acceptance or Manager approval. Completion still requires Manager adjudication (#730).

There are no CRITICAL or MAJOR findings. Two MINOR findings concern process and provenance, and neither asks for a candidate code change. Five NITs are listed in §8.

---

## 0. Identity, inputs and isolation

| Item | Value |
|---|---|
| Reviewer | **Claude Opus 5.5** (`claude-opus-5-5`), **`claudeAgent` pool**, **T3 Code child session** (Claude Code harness) |
| Role | Named independent non-author reviewer, CCF issue #846, Track A |
| Authorship | I did not write, edit, commit or push any part of the candidate |
| Claim / lane record | https://github.com/jermur788/CCF/issues/846#issuecomment-7025309392, read through `gh api` (read-only). Names writer `gpt-6.1-sol` (Codex `codex_3`), reviewer `claude-opus-5-5` (`claudeAgent`) and a non-authoring DeepSeek orchestrator |
| Binding packet | branch `docs/Multi-A1-and-Player-Adapter-Parallel-Packets-20261119-v1` @ `2e72cc89f6cdedf6eb6130832364c7f881cdd88c` (live `git ls-remote` equals the local ref), file blob `9c821cac9a9dcff0ee1f4f8f709f18d5abaa37b7` |
| Candidate commit | `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`. Live `git ls-remote origin refs/heads/fix/multi-a1-workrevision-fence-20261119-v1` returns this SHA |
| Base | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6`, the candidate's only parent |
| Base engine tree | `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4` — matches the brief |
| Candidate engine tree | `6059b62dcb9b2159d97ce189597998d70fab8dab` — matches the brief |
| P0 subtree (`Tools/MultiRackSitkaLab`) at base and candidate | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`, the qualified P0 tree |
| **Clean review checkout** | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-Review`, made with `git clone -q --no-hardlinks /home/jer/Development/CCF-HP …` then `git checkout -q --detach 4bb809732fb6…` |
| Mutation scratch clone | `/home/jer/.ccf-orch/a1-review/mutation-clone`, outside every repository and worktree (see §4) |
| Scripts / raw logs | `/home/jer/.ccf-orch/a1-review/scripts/`, `/home/jer/.ccf-orch/a1-review/logs/` |
| Host / tools | WSL2 Ubuntu, kernel `6.18.58.1-microsoft-standard-WSL2`; git 2.53.0; .NET SDK **8.0.134** (`global.json` requests `8.0.133` with `rollForward: latestPatch`; 8.0.134 is the only SDK in `/home/jer/.dotnet/sdk`) |
| Environment | `DOTNET_ROOT=/home/jer/.dotnet`, `PATH=/home/jer/.dotnet:$PATH`, `TMPDIR=/home/jer/.ccf-orch/tmp`, which is outside the repo |
| Live coordination (read-only) | live `main` = `7758e17c882fad01970d2f2fb8c09586bbcab5c1`. Live `Docs/Project/coordination/active-tasks.md` blob is `6b8e70f5ea0c82ab9b996b3b6ba013a3a7780a26`, identical to the blob cited by the packet and the claim. It has 0 matches for `#846`, `Multi-A1`, `WorkRevision`, `AnnualContinuation` or `8741a32`, so no conflicting lock is recorded and the lane is recorded only in the #846 claim |
| Not touched | The author worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-WorkRevision-Fence` was never opened. No git write ran in `/home/jer/Development/CCF-HP`; afterwards the primary is still `main` @ `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`, with empty `git status --porcelain` from WSL. Primary reads were limited to `git cat-file`, `rev-parse`, `show`, `worktree list`, `config --local user.*`, `ls-remote` and `gh api` |

### P0 pinned binary: restored by copy, never rebuilt

Source: `/home/jer/CCF-Dev/Multi-20261119/y2engine/Tools/MultiRackSitkaP1Lab/.p0ref/`. It holds three regular files with link count 1, owned by `jer`. Its `provenance.txt` records `p0QualifiedCommit=a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, `p0QualifiedTree=f212c8f64fb28b83aa870c7b5b7c72e03051e46a`, `sdk=8.0.133` and `dllSha256=9a99f781…34b3`. I copied the three files into `<review checkout>/Tools/MultiRackSitkaP1Lab/.p0ref/` with `cp`:

```text
--- p0ref verification
SitkaLab.Core.dll: OK
actual=9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
pin_match=yes
…/.p0ref/SitkaLab.Core.dll type=regular file links=1
…/.p0ref/SitkaLab.Core.dll.sha256 type=regular file links=1
…/.p0ref/provenance.txt type=regular file links=1
```

The pin `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` verified, so I did not stop. I edited none of: the pin, `build-p0-core-ref.sh`, P0 source or the golden digests.

---

## 1. Check 1 — source review of the exact diff; allowlist

```text
$ git rev-list --count 8741a328..4bb80973
1
$ git rev-list --parents -n1 HEAD
4bb809732fb691f15630e1fbc5ea20dca6dbbe2f 8741a32843b739b0587e4c6f3a3ee7fc890ea7f6
$ git diff --name-status 8741a328 4bb80973          # whole repository, not just the lab
M	Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
M	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
$ git diff --numstat 8741a328 4bb80973
4	1	Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
168	0	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
$ git diff --summary 8741a328 4bb80973              # empty: no mode change, create or delete
$ git diff --check 8741a328 4bb80973                # clean
```

Comparing `git ls-tree -r` blobs across all of `Tools/MultiRackSitkaP1Lab` at base and candidate, only two lines differ:

```text
< 100644 blob 63c41542e875581c7ae75b41746b05e3960d7b54	Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
> 100644 blob 87903271b2246311ee0d356ad6979b77c6b1b43a	Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
< 100644 blob 6082a38ecfe2a2b8f6d7c537e48a6a16c7bc6033	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
> 100644 blob 5d13adb021e2a58eb7767643ca8de04da150d953	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
```

The entire production change is one hunk:

```diff
@@ -232,17 +232,20 @@ internal sealed class AnnualContinuation
     internal AnnualReport Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)
     {
         lock (sync)
         {
             if (root.Pending.Count > 0 || root.ApprovedUnexecuted is not null)
                 throw new P1Refusal("WorkPendingConsentRequired",
                     "pending or approved-but-unexecuted work must be cancelled with explicit consent before the year advances");
-            return AdvanceLocked(expectedYear, expectedRevision, root.WorkRevision, beforeCommit);
+            if (root.WorkRevision != 0)
+                throw new P1Refusal("ContinuationWorkRevisionRequired",
+                    "after a Fell, advancing the year requires an explicit expected work revision; use the work-revision overload");
+            return AdvanceLocked(expectedYear, expectedRevision, 0, beforeCommit);
         }
     }
```

**Result.**

- **The two-file allowlist held.** There is exactly one commit, and nothing else changed anywhere in the repository. Specifically unchanged: `AnnualRules.cs`, `AnnualEngine.cs`, `WorkContract.cs`, both `Program.cs` files (CLI and tests), all `.csproj` files, `global.json`, `Directory.Build.props`, `build/build-p0-core-ref.sh`, fixtures, the manifest, the P0 subtree (`f212c8f6` at both commits), `Assets/**`, `specs/**`, `Docs/**` and coordination files.
- **The golden digests were not touched.** The `Digests` literal line (`ContinuationTests.cs:11`) is byte-identical at both commits; its sha256 is `bdb32d112e4d6949bc37082c967fd031d9ae65b70c92cdbe92902d7e648ceff5` in both.
- **The four-argument overload and `AdvanceLocked` are unchanged**; both sit outside the hunk.

**Minimality.** This is the smallest reasonable change that meets the packet: one guard, plus replacing the tautological argument with a constant.

- The guard alone (mutant M2, §4) is behaviour-equivalent.
- The literal `0` alone (M3) would also fail closed, but only with the pre-existing `ContinuationWorkRevisionMismatch` code. The packet asks for "a precise refusal code", so the separate guard is justified.
- The literal `0` follows the packet's instruction not to "silently inject live workRevision". It also makes `AdvanceLocked`'s fence meaningful again as defence-in-depth.

The test change adds four tests and two private helpers. They map one-to-one onto the packet's mandatory test list and use the already-registered `WorkContractTests.Register`; no registration, project or third-file change was needed. A-2, A-3 and A-4 were not touched, nothing was added to the adapter or Host, and the overload was not removed.

---

## 2. Check 2 — defect re-derived independently at BASE

### Source at BASE (blob `63c41542`)

```text
192:            int workRevision = checked(root.WorkRevision + 1);                                   // Execute
221:            var snapshot = new ContinuationSnapshot(new AnnualState(year, Array.AsReadOnly(trees)), root.Snapshot.Revision);
226:            root = new WorkRoot(snapshot, workRevision, Array.AsReadOnly(root.Events.Append(committed).ToArray()),
233:    internal AnnualReport Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)
238:                throw new P1Refusal("WorkPendingConsentRequired",
240:            return AdvanceLocked(expectedYear, expectedRevision, root.WorkRevision, beforeCommit);  // legacy
269:        if (expectedWorkRevision != root.WorkRevision)                                         // AdvanceLocked
```

**Why the defect exists.**

1. A real `Execute` (a Fell) publishes a new root. That root keeps the ecological year (`year` comes from `state.EcologicalYear`) and keeps the annual revision (`root.Snapshot.Revision`, line 221). Only `WorkRevision` moves (line 192, published at line 226). Its roster now has one dead tree and one more event.
2. At base, the legacy two-clock overload passed **the live `root.WorkRevision` itself** as the "expected" work revision (line 240). So the only work-revision fence (line 269) evaluated `root.WorkRevision != root.WorkRevision`, which is always false.
3. A caller holding a pre-Fell `(year, annualRevision)` therefore still matched the year fence and the revision fence after the Fell. That caller then advanced the ecological year over a roster and event ledger it had never observed. The only refusal left was the pending/approved check, and an executed batch is neither pending nor approved.

### Runtime demonstration at BASE

This used a reviewer-only probe, `ReviewProbe.cs`, which existed in the scratch clone only. It registers one observation-only test through a `[ModuleInitializer]`, so no committed test file was edited. Excerpt from log `B0P_base_plus_probe.tests.log`, at HEAD `8741a328`:

```text
PROBE pre-Fell view: year=0 annualRev=0 workRev=0 living=100
PROBE post-Fell frame: year=0 annualRev=0 workRev=1 living=99 events=1
PROBE legacy Advance(0,0) SUCCEEDED: report YearBefore=0 YearAfter=1 Before.Living=99; committed year=1 annualRev=1 workRev=1
PROBE four-argument Advance(0,0,0) REFUSED code=ContinuationWorkRevisionMismatch; frame year=0
```

The probe also confirms that the modern four-argument overload does fence a stale work revision, as the packet says.

### The same probe on the CANDIDATE

Log `CP_candidate_plus_probe.tests.log`, at HEAD `4bb80973`:

```text
PROBE pre-Fell view: year=0 annualRev=0 workRev=0 living=100
PROBE post-Fell frame: year=0 annualRev=0 workRev=1 living=99 events=1
PROBE legacy Advance(0,0) REFUSED code=ContinuationWorkRevisionRequired message="after a Fell, advancing the year requires an explicit expected work revision; use the work-revision overload"; frame year=0 annualRev=0 workRev=1 living=99
PROBE four-argument Advance(0,0,0) REFUSED code=ContinuationWorkRevisionMismatch; frame year=0
```

### The path is closed, not moved

After the change, `AdvanceLocked` has exactly two callers:

- `AnnualContinuation.cs:243` passes the literal `0`, reachable only when `root.WorkRevision == 0` under the same lock.
- `AnnualContinuation.cs:260` passes the caller-supplied `expectedWorkRevision`.

No call anywhere still feeds `root.WorkRevision` back as an expected value. `AnnualEngine` has no Fell, work or Execute API (grep: none), and `WorkContract.cs` never calls `Advance`. The only route that advances the year after a Fell is now the four-argument overload, and its fence is non-tautological.

---

## 3. Check 3 — correctness edge cases

- **Exception precedence.** Pending or approved-unexecuted work still produces `WorkPendingConsentRequired` first (unchanged); the new guard comes after it.
  - This matches the four-argument overload, where the consent check precedes `AdvanceLocked`'s fences.
  - The new test `pending and approved advance retain consent precedence before and after Fell` pins this order. Mutant M4 (guard moved first) makes it fail (§4).
  - Both refusals have zero mutation, so either order fails closed. The chosen order preserves base behaviour.
- **The literal `0`.**
  - Inside the same `lock (sync)`, the guard guarantees `root.WorkRevision == 0` whenever line 243 runs. The literal therefore equals the old argument; M2 (guard kept, base argument restored) passes 66/66.
  - It is not a silent injection of the live revision. It is a fixed statement that a two-clock caller can only mean "no Fell has happened".
  - It is also defence-in-depth: with the guard deleted (M3), the literal alone still fails closed through `ContinuationWorkRevisionMismatch`.
- **Lock scope.**
  - The guard and `AdvanceLocked` run in one critical section, so there is no time-of-check/time-of-use gap.
  - Every mutator takes the same `sync`: `AddPendingFell` L82, `RemovePendingFell` L104, `ApprovePlan` L121, `CancelApprovedPlan` L139, `Execute` L154, `Advance` L235/L254.
  - So does every reader: `Snapshot` L51, `Frame` L59, `CurrentToken` L71, `HasUnexecutedWork` L76.
  - `root` is the only mutable field; `sync`, `simulationSeed`, `areaHa` and `identity` are `readonly`.
  - A concurrent `Execute` and legacy `Advance` serialise. If the advance runs first, the year moves and the approved plan becomes stale (`WorkPlanStaleSource`, L172). If the Fell runs first, legacy refuses.
- **Can `WorkRevision != 0` happen without a Fell? No.**
  - `WorkRevision` is set in exactly two places: the constructor (L45, `0`) and the `Execute` publish (L226, value `checked(root.WorkRevision + 1)` from L192).
  - Every other `root = root with { … }` (L97, L111, L131, L143, L283–285) leaves `WorkRevision` alone.
  - `Execute` publishes only after these checks: non-empty orders (L175), no duplicates (L182), every order a live exact-case tree (L184), and not the whole live stand (L190). So each publish fells at least one tree and appends exactly one event, which means `Events.Count == WorkRevision` always holds.
  - The replay path (L161–165) and the fault seam (L219) publish nothing.
- **Can `WorkRevision` decrease or reset? No.**
  - No code decrements or resets it, and `checked` prevents wrap-around.
  - `root` is private and is assigned only inside this class; no restore, deserialisation or setter path exists.
  - So the legacy overload stays permanently fail-closed for the rest of a session after its first Fell, even for a caller with a fresh `(year, revision)`. That is exactly the packet's "once any Fell has changed WorkRevision". The new test pins it at `WorkContractTests.cs:268–269` after a real year advance.
- **Are the new and old codes distinguishable? Yes.**
  - `ContinuationWorkRevisionRequired` and `ContinuationWorkRevisionMismatch` are different strings with different messages.
  - `T.Refuses` uses exact `e.Code == code` (`Runner.cs:14`).
  - M3 proves the tests tell them apart: "expected refusal ContinuationWorkRevisionRequired, got ContinuationWorkRevisionMismatch".
- **Production callers in `src/`.**
  - The only `.Advance(` call is `src/Cli/Program.cs:67` (`engine.Advance()`). There, `engine` is an `AnnualEngine` (L66) — the one-year engine — not `AnnualContinuation`.
  - `AnnualContinuation` is `internal`. Its only factory is the internal `AnnualEngine.ContinueInMemory()` (`AnnualEngine.cs:72–76`, commented "Private experiment seam: no CLI route or public continuation contract").
  - `InternalsVisibleTo` names only `SitkaP1.Core.Tests` (`AnnualEngine.cs:3`), and nothing under `Assets/**` references it.
  - **Honest impact:** no production caller changes behaviour. Only test callers can reach the legacy overload, and all 62 original tests pass unchanged — none of them called it after a Fell, which is also why the defect went unnoticed.
- **Refusal precedence inside the legacy path (INFO, see F-6).** After a Fell, a legacy caller whose year is also stale, or a session where natural mortality has since reached zero live stems, now gets `ContinuationWorkRevisionRequired` rather than `ContinuationYearMismatch` or `ContinuationNoLivingStems`. Every one of these is a zero-mutation refusal, and the new code is the actionable one.

---

## 4. Check 4 — mutation check (scratch copy only)

**Location.** `/home/jer/.ccf-orch/a1-review/mutation-clone` sits outside every repository and worktree. I created it with `git clone -q --shared` from my review clone. On this host (git 2.53.0) the clone ended up with **its own independent object store**: a 971 MB pack with link count 1, a different inode from the review clone's pack, and no `objects/info/alternates` file. It therefore shares nothing with the review clone.

**Procedure** (`run-mutants.sh` and `mutate.py`). Each mutant is applied to the candidate file in the scratch working tree only and is never committed. Before each case `bin/` and `obj/` are deleted, then `qualify.sh` runs a fresh build and the tests. Afterwards the file is restored with `git checkout -- <file>`, and the script asserts `git status --porcelain --untracked-files=all` is empty. `mutate.py` refuses to run unless its anchor occurs exactly once. The final state of the scratch clone is HEAD `4bb80973` with an empty status.

### M1 (required): revert only the `AnnualContinuation.cs` hunk; candidate tests unchanged

```text
M1 restored blob=63c41542e875581c7ae75b41746b05e3960d7b54 base blob=63c41542e875581c7ae75b41746b05e3960d7b54
 Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs | 5 +----
 1 file changed, 1 insertion(+), 4 deletions(-)
git status before build:  M Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
== M1_revert_annualcontinuation_hunk: build_rc=0 test_rc=1
    0 Warning(s)
    0 Error(s)
-- totals:
64/66 passed, 2 failed
-- FAIL lines:
FAIL work: legacy advance after a real Fell refuses without changing any session state — expected refusal ContinuationWorkRevisionRequired, but the call succeeded
FAIL work: explicit work-revision advance rejects stale, rolls back faults and accepts fresh after Fell — expected refusal ContinuationWorkRevisionRequired, but the call succeeded
```

**The new regression tests are not vacuous.** With the base production code, both fix-detecting tests fail for the right reason: the stale legacy call *succeeds*. All 62 original tests still pass, and all ten digests stay identical.

### Additional reviewer mutants

Each starts from the candidate file; the exact diffs are in `logs/<name>.mutation.diff`.

| Mutant | Change | Totals | Failure output |
|---|---|---|---|
| M2 | guard kept; argument restored to `root.WorkRevision` | **66/66** | none — equivalent mutant, so the literal `0` is defence-in-depth rather than load-bearing |
| M3 | guard removed; literal `0` kept | 64/66 | both fix tests: `expected refusal ContinuationWorkRevisionRequired, got ContinuationWorkRevisionMismatch: expected work revision is stale` |
| M4 | guard moved before the pending/approved check | 65/66 | `FAIL work: pending and approved advance retain consent precedence before and after Fell — expected refusal WorkPendingConsentRequired, got ContinuationWorkRevisionRequired: …` |
| M5a | correct code, but first republishes an identical root (`root = root with { };`) | 64/66 | both fix tests: `assertion failed: refusal keeps the exact immutable session root` |
| M5b | correct code, same root reference, but clears the executed-key map in place first | 64/66 | both fix tests: `assertion failed: all session bytes unchanged` |
| M5c | correct code, but genuinely advances the year first (partial commit, then refuse) | 64/66 | both fix tests: `assertion failed: refusal keeps the exact immutable session root` |

Every mutant run built with 0 warnings and 0 errors, and every one kept the ten digests identical to the baseline.

---

## 5. Check 5 — full qualification on the clean checkout

Commands, run in `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-Review/Tools/MultiRackSitkaP1Lab` with the environment from §0:

```text
dotnet build MultiRackSitkaP1Lab.sln
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
```

Raw results from `logs/candidate-clean.*`, `date_utc=2026-11-20T00:02:52Z`, HEAD `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`, engine tree `6059b62dcb9b2159d97ce189597998d70fab8dab`. This was a fresh clone with no prior `bin/` or `obj/`.

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
Time Elapsed 00:00:06.11
build_rc=0

66/66 passed, 0 failed
test_rc=0
```

`TreatWarningsAsErrors=true` is set in `Directory.Build.props`. All 66 tests printed `PASS`, including the four new ones:

```text
PASS work: legacy no-work advance preserves all ten existing yearly digest literals
PASS work: legacy advance after a real Fell refuses without changing any session state
PASS work: explicit work-revision advance rejects stale, rolls back faults and accepts fresh after Fell
PASS work: pending and approved advance retain consent precedence before and after Fell
```

The ten digest lines were emitted by the original `continuation: sequential fixture years and recorded deterministic digests` test, which drives the legacy two-clock `Advance(year-1, year-1)` path:

```text
YEAR 1 DIGEST df2d44fdf9a879bd8191b89597f712ca68bacb62ff17b6033b918d9e9f5586f1 DEATHS 0
YEAR 2 DIGEST 23a1d889f551a840ae2ab7e689ab6b977dc1b7dabbf187fe7207aeae561c6abd DEATHS 0
YEAR 3 DIGEST 953fc7ef660e66cc5a76e89a01d12aa2524d4e5d6d4d5bb04867d4700bb56afc DEATHS 1
YEAR 4 DIGEST 6fc220c377555edc15892613c6468a3ca00884b310f85ad80d8102f7eef3f5ba DEATHS 0
YEAR 5 DIGEST cbd51e38889da9cf65376d38b03a0befe49f398a125802c7cb85d1079992a487 DEATHS 0
YEAR 6 DIGEST 01cfb6560386799d01dd3ef7c8a1b8d98b2cff6e4fc054e24509d6ca308b796e DEATHS 1
YEAR 7 DIGEST c6baab550107d0978ade9f3107ac715e9dcc0a28a3a01d27b70f53accb59c646 DEATHS 2
YEAR 8 DIGEST 61879c1e79aaf9c5b503d874a38837807da80d01bb6f645560a842beb03e7ab0 DEATHS 4
YEAR 9 DIGEST faaea476fcdaec744e18fec2953bbe8125b56a8e3d52f826e30fdc4aa956169a DEATHS 5
YEAR 10 DIGEST 10d31913531c627089a01e5c66aeea0c6fee4efad74e31d31f579e5264bcd7d5 DEATHS 0
```

**Digest comparison.** I reduced these lines to `Y<n> <digest>` and compared the result byte for byte with the supplied baseline, saved as `scripts/baseline-digests.txt`:

```text
CMP: IDENTICAL
DIFF: no differences
01db908ae7459e682d464eb6a9ca9deb250b70d21451b52e882aaa08f6cbe8ad  candidate-clean.digests.txt
01db908ae7459e682d464eb6a9ca9deb250b70d21451b52e882aaa08f6cbe8ad  ../scripts/baseline-digests.txt
10 candidate-clean.digests.txt
```

All ten YEAR 1..10 digests are **character-identical** to the baseline.

**Checkout hygiene.** `git status --porcelain` was empty before and after the run. The only ignored entries afterwards are P1-owned: `.p0ref/`, plus `bin/` and `obj/` under `src/Cli`, `src/Core` and `tests/Core.Tests`. Nothing appeared under P0 or anywhere else, and no `ccf-p1-tests.*` directory was left in `TMPDIR`.

**Independent base control** (scratch clone, pure `8741a328`, run `B0_base_control`): build 0 warnings / 0 errors, **`62/62 passed, 0 failed`**, ten digests IDENTICAL to the baseline. This confirms the original 62-test count independently, so the candidate's 66 is 62 original plus 4 new.

---

## 6. Check 6 — test-quality review

**Does `RefusesWithoutMutation` prove zero mutation? Yes; it is not self-fulfilling.**

- `root` is the single mutable field of `AnnualContinuation`, and every state change reassigns it. Reference identity of `root` before and after the action (`WorkContractTests.cs:420`) is therefore a complete witness that nothing was published.
- The pre-action `JsonSerializer` byte image compared with the post-action image (L417 vs L421) independently catches in-place mutation of nested state.
- `ReadRoot` reads the live field through reflection on every call, with no caching.
- I demonstrated each mechanism with a targeted mutant:
  - M5a (identical-content republish) and M5c (real partial commit) are caught by the reference check.
  - M5b (in-place `Executed.Clear()`, same reference) is caught *only* by the byte check.
- **Weakness (NIT F-3):** the eleven per-field assertions at L422–432 run only after L420 has proved `after` is the same object as `before`. They compare an object with itself and cannot fail. M5b confirms this: `before.Executed.Count == after.Executed.Count` stayed true after the clear.

**Is the reflection into `ContinuationTests.Digests` acceptable? Yes — a minor coupling, not a weakness (NIT F-4).**

- If the field is renamed, `GetField` returns null and `.GetValue(null)` throws `NullReferenceException`. That is a loud FAIL, never a silent pass.
- `T.Eq(10, digests.Length, …)` guards against an emptied array.
- Reusing the one golden source avoids creating a second copy of protected golden literals that could drift, and it respects the two-file allowlist.
- The same reasoning applies to reflecting into private `AnnualContinuation.root`, which avoids adding a production access seam.

**Are the new assertions specific enough? Yes.**

- `T.Refuses` uses exact code equality.
- M3 shows the tests fail when the code is `…Mismatch` instead of `…Required`.
- M4 shows they fail when the order of the refusals changes.
- M1's failures show the call *succeeding* — which is exactly the defect.
- The fault-seam probes (`callbackReached`) prove that each refusal fires before the annual fault seam.
- The replay probes prove that same-key idempotency survives the refusal, the fault and the next year.

**Would any new test pass with the fix removed?** Under M1, two of the four new tests still pass. Both do so by design:

- `legacy no-work advance preserves all ten existing yearly digest literals` is a *preservation* test. `WorkRevision` stays 0, so the fix never engages. Its job is to catch an over-broad fix. It also overlaps the original digest test in `ContinuationTests.cs:18–42`, adding per-year work-revision, event and unexecuted-work assertions.
- `pending and approved advance retain consent precedence before and after Fell` is a *precedence* test. `WorkPendingConsentRequired` fires before the guard at both base and candidate. It catches a wrong-order fix (M4).

Of the other two, `explicit work-revision advance …` detects the fix only through its final legacy assertion (L268–269). Its four-argument assertions exercise unchanged code and also pass at base. `legacy advance after a real Fell …` detects the fix through its first refusal. No new test is vacuous with respect to its stated purpose.

---

## 7. Check 7 — no test weakened, deleted, skipped or re-baselined

Base vs candidate blob comparison for every test file:

```text
UNCHANGED AnnualTests.cs base=d36d2afedab826958f511b754649ee61abcb43cc cand=d36d2afedab826958f511b754649ee61abcb43cc
UNCHANGED ContinuationTests.cs base=b3f15c327ded52790ee88e137da3f36e1b12aa4e cand=b3f15c327ded52790ee88e137da3f36e1b12aa4e
UNCHANGED Program.cs base=86952c504dcb64d3bf0da971eaea6b4b557c2a71 cand=86952c504dcb64d3bf0da971eaea6b4b557c2a71
UNCHANGED Runner.cs base=8c10dddc09aa61e427970b598f566e312409e159 cand=8c10dddc09aa61e427970b598f566e312409e159
UNCHANGED SitkaP1.Core.Tests.csproj base=9b66edd692ea4fc0fcfb06556c2b3f6542508496 cand=9b66edd692ea4fc0fcfb06556c2b3f6542508496
CHANGED WorkContractTests.cs base=6082a38ecfe2a2b8f6d7c537e48a6a16c7bc6033 cand=5d13adb021e2a58eb7767643ca8de04da150d953
UNCHANGED tests/build-output-safety.sh
UNCHANGED tests/cli-safety.sh
UNCHANGED tests/l4_analyze.py
UNCHANGED tests/p0-isolation.sh
```

- `WorkContractTests.cs` numstat is `168 0`: purely additive, so no existing line was modified or removed.
- The only deleted line in the entire diff is the production line being fixed: `-            return AdvanceLocked(expectedYear, expectedRevision, root.WorkRevision, beforeCommit);`.
- The only added comment lines are the two explanatory comments at `WorkContractTests.cs:186` and `:410`. No `T.Add` was commented out, and no early-return or skip was added.

Registered test names:

```text
base T.Add count: 62  candidate T.Add count: 66
names only in base (removed/renamed): (none)
names only in candidate (added):
"work: explicit work-revision advance rejects stale, rolls back faults and accepts fresh after Fell"
"work: legacy advance after a real Fell refuses without changing any session state"
"work: legacy no-work advance preserves all ten existing yearly digest literals"
"work: pending and approved advance retain consent precedence before and after Fell"
duplicate names in candidate: (none)
```

The golden `Digests` literal is byte-identical (§1). **Result: nothing was weakened, deleted, skipped or re-baselined.**

---

## 8. Findings

| ID | Severity | Location | Finding | Concrete consequence |
|---|---|---|---|---|
| F-1 | **MINOR** (provenance/process, not code) | commit `4bb80973` metadata | Author and committer are both `DeepSeek V4.1 Flash 1017 <jermur788@users.noreply.github.com>`, inherited from the primary repository's local `user.name`/`user.email`; all its worktrees share them, and the WSL global identity is empty. The #846 claim and the writer receipt instead name `gpt-6.1-sol` (Codex `codex_3`) as the sole writer and the DeepSeek orchestrator as non-authoring. | Git history alone cannot attribute the patch to the registered writer, so the Manager must adjudicate writer identity and author/reviewer independence from the claim and receipt, not from `git log`. My independence is unaffected: Claude Opus 5.5 is neither identity. Correcting the metadata would need a new commit, which is outside reviewer scope. |
| F-2 | **MINOR** (process hazard, outside the candidate) | recommended invocation `wsl.exe -d Ubuntu -- bash -lc "<cmd>"` | `--` hands the command line to the default login shell, which expands `$VAR` and `$(…)` inside the double-quoted command, in its own working directory (here the primary checkout), before the inner bash runs. **This happened in this review:** `$R` expanded to empty, and `$(git rev-parse …)` ran (read-only) in `/home/jer/Development/CCF-HP`. The inner `set -e` stopped at the failed clone before any checkout. I verified the primary unchanged: `main` @ `cbf3a618`, clean. | A lane that writes `cd "$WT" && git <write>` in this form can silently run git writes in the primary checkout. Use `wsl.exe -d Ubuntu -e bash -c '…'` or a script file. The writer receipt says its commands used `--`; its reported results are reproduced independently here, so the verdict is unaffected. |
| F-3 | NIT | `tests/Core.Tests/WorkContractTests.cs:422–432` | The per-field assertions run only after `ReferenceEquals(before, after)` (L420) has passed. They compare an object with itself and can never fail; the actual proof is L420 plus the byte check at L421. M5b shows an in-place executed-map mutation is caught only by L421. | They overstate coverage to a reader; correctness is unaffected. Optional fix: capture the scalars before the action, or remove them. |
| F-4 | NIT | `tests/Core.Tests/WorkContractTests.cs:187–188`, `:411–412` | Reflection into private `ContinuationTests.Digests` and `AnnualContinuation.root`. | A minor coupling to private names that fails loudly on rename. It keeps a single golden source and the two-file allowlist. Acceptable as is. |
| F-5 | NIT (receipt accuracy, outside the candidate) | `/home/jer/.ccf-orch/846-candidate.md` | Reports `AnnualContinuation.cs (+3/-1)`; the actual numstat is **+4/−1** (net +3). | Cosmetic. The GitHub receipt should quote +4/−1. |
| F-6 | NIT / INFO (no change requested) | `src/Core/AnnualContinuation.cs:240–242` | After any Fell, the legacy overload reports `ContinuationWorkRevisionRequired` before `AdvanceLocked`'s year, revision or no-living-stems codes. Because `WorkRevision` never decreases, it stays unusable for the rest of that session even with a fresh `(year, revision)`. | Intended by the packet; all these are zero-mutation refusals. Track B's adapter contract should map this code to "use the work-revision overload", not "your work revision is stale". |
| F-7 | NIT (optional) | `src/Core/AnnualContinuation.cs:233` | The legacy overload still has no `<summary>`. Its new "no-work-only entry point" contract is stated only in the refusal message. | Affects learner maintainability only. Not required by this packet. |

There are **no CRITICAL and no MAJOR findings**. No finding asks for a change to the candidate's production code or tests before Manager adjudication.

---

## 9. Verdict

**APPROVED** — scoped to the **source review of the exact candidate `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`** and the **Tier-2 delta qualification on `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f`**.

What the verdict rests on:

- The packet's defect is real at base. I showed it both from source and at runtime.
- The four-line fix closes it at the only tautological fence, without moving it elsewhere.
- No-work legacy behaviour is byte-identical: the ten golden digests are unchanged.
- Pending/approved consent precedence and the four-argument token-fenced route are preserved.
- The new refusal code is precise and tested by exact equality.
- The new tests fail against the reverted hunk (M1). They also detect wrong-code (M3), wrong-order (M4) and refuse-after-mutation (M5a, M5b, M5c) variants.
- Build: 0 warnings, 0 errors. Tests: 66/66. The 62 original tests are unchanged and still pass.
- The allowlist held, and no protected path changed.

F-1 requires Manager attention for provenance, but it is not a defect in the candidate. I do not claim integration, a merge, Unity, UI, Stage-5 acceptance or Manager approval.

---

## 10. What remains UNVERIFIED

- **Unity** import, compilation, runtime and rendering: not applicable and not run. No Unity code changed. P1 Core compiles `Assets/ForestPrototype/SitkaGrowthModel.cs` and `SimulationRandom.cs` read-only; both are unchanged, and the existing test `source: linked CCF files are byte-identical to the pinned blobs` PASSed.
- **Integration**: no merge, rebase or integration with `main` was attempted. The candidate diverges from `main` by design, per the packet.
- **Stage-5 UI, the Track B player-session adapter and connected UI**: out of scope; not examined.
- **The lab's shell harnesses were not run.** I inspected them first.
  - `tests/build-output-safety.sh` moves `.p0ref` aside and rebuilds it through `build-p0-core-ref.sh`.
  - `tests/p0-isolation.sh` deletes `.p0ref`, `bin` and `obj`, then rebuilds.
  - Rebuilding the pin is forbidden here, and this host cannot byte-reproduce it.
  - `tests/cli-safety.sh` exercises only the unchanged CLI, which has no path to `AnnualContinuation`.
- **P0 reference provenance beyond its checksum**: the binary was copied from the y2engine lane and checked only against pin `9a99f781…`. It was not rebuilt from source.
- **SDK**: I built with .NET SDK 8.0.134 through `rollForward: latestPatch`. No build with exactly 8.0.133 was done.
- **Concurrency after a Fell**: the interleaving of `Execute` and legacy `Advance` was reasoned from the single `sync` lock, not stress-tested. The existing concurrency test covers only the no-work legacy path.
- **Actual authorship**: I cannot verify which agent produced the patch text (see F-1).
- **GitHub-tracked receipt and #730 handoff publication**: not my task, and not verified.
- **Manager adjudication**: pending. This review is not Manager approval.

---

## 11. Process notes and artefacts

- I made no commits, pushes, GitHub comments or PR changes. I edited nothing under `Docs/**`, coordination files, `active-tasks.md`, `Assets/**` or P0, and ran nothing on Unity or Windows. I changed nothing in the candidate branch or the author worktree, and ran no git writes in the primary checkout.
- `ReviewProbe.cs` and every mutation existed only in the scratch clone, and each was removed afterwards with a verified clean status. The review checkout is at HEAD `4bb80973` with an empty `git status --porcelain --untracked-files=all`. It holds only the git-ignored `.p0ref/`, `bin/` and `obj/`.
- Scripts in `/home/jer/.ccf-orch/a1-review/scripts/`:
  - `qualify.sh` — restores `.p0ref` by copy, verifies the pin, then builds and tests.
  - `run-mutants.sh` — base control, probe runs and mutants.
  - `mutate.py` — exact single-occurrence mutation.
  - `ReviewProbe.cs`
  - `baseline-digests.txt`
- Logs in `/home/jer/.ccf-orch/a1-review/logs/`:
  - `<run>.env.txt`, `.build.log`, `.tests.log` and `.digests.txt` for each of: `candidate-clean`, `B0_base_control`, `B0P_base_plus_probe`, `CP_candidate_plus_probe`, `M1_revert_annualcontinuation_hunk`, `M2_…`, `M3_…`, `M4_…`, `M5a_…`, `M5b_…`, `M5c_…`.
  - `<mutant>.mutation.diff` for each mutant, plus `M1_….mutation.txt`.
  - `live-active-tasks.md`, the read-only copy of the live board.
- The review checkout (about 1.2 GB) and the scratch clone (about 1.1 GB) are disposable reviewer artefacts. They have been left in place for re-inspection.
