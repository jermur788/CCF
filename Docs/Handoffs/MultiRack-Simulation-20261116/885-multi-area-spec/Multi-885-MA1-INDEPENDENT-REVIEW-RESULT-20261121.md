> Published by the DeepSeek V4.1 Flash orchestrator from the reviewer's own returned review file, verbatim
> except one character-encoding repair in the title. The reviewer authored the verdict; the orchestrator did not.

# Independent nonauthor review — CCF Multi #885 MA-1

**Verdict: APPROVED for the bounded source / offline / HTTP / DOM-stand-in review gate.**
**Blocking findings: none.** One non-blocking wording finding, N1 below.
This is not integration, Manager G4 acceptance, owner G2 acceptance, scientific validation, or native browser qualification.

Reviewed code: 58950a9aa74067f22c9fe322b27f42e036ae2e01.
Base: e11dd5192d7a89abf859613ce92814bdd6338ea2.
Author documentation: 88189cb1dbab3dbcf9ef745b1ef4d2c5623f948b, read with git show.
Reviewer: separate GPT-6.1 Sol session, high reasoning effort; no authorship, no fixes, no delegates.
Date: 2026-11-21.
Own detached worktree:
 /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121

## Blocking findings

None (B1..Bn: empty).

## Non-blocking findings

### N1 ? Results introduction overstates annual evaluation for an already-empty area

File: Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:1560
Absolute file:
 /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121/Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:1560

Reproduction:
1. Run the outside-repository scratch harness described below. It constructs a validated test-only two-area experiment, with an extremely dense pair in MA-A and the normal MA-B. It searches a deterministic seed under the real AnnualRules; no production ecology is changed.
2. Advance once so MA-A naturally loses its two trees, then advance again while MA-B remains populated.
3. The second advance has MA-A NoLivingStems=true and a null annual report. The exact committed renderHistory function, evaluated in Node against that real view, produces:
   "Latest year 1 ? 2. Each area ran the unchanged annual rules on its own trees."

Observed: the introduction says every area ran annual rules.
Expected: describe populated areas as evaluated, and naturally empty areas as carried forward.

The adjacent per-area row correctly states the carry-forward rule. The engine correctly skips AnnualRules for the empty area (MultiAreaContinuation.cs:374), preserves its dead roster, and advances the shared year. This is a low-severity text defect, not an atomicity or ecological defect. No fix made.

Evidence: history-label.mjs, history-label.log, empty-view.json, probes/Program.cs in /tmp/ccf-ma1-review/.

## Scope, custody and protection

- git diff --name-status base..candidate contains exactly 15 allowed source/test paths, all under Tools/MultiRackSitkaP1Lab. Seven new Core/MultiArea files; new PlayerMultiApi.cs, MultiAreaTests.cs and ui-smoke-multiarea.sh; permitted modifications to PlayerApi.cs, Program.cs, app.js, Core.Tests/Program.cs and ui-smoke.sh.
- Core.Tests/Program.cs changes by exactly one registration line.
- No .csproj, fixture, .p0ref, protected Core arithmetic, P0, Assets, Packages, ProjectSettings, main, or unrelated tracked changes.
- Review worktree was created from the primary repo at the exact requested commit; it is still detached at that commit and clean including untracked files.
- The author and all CCF-Multi-882 worktrees were not written. The only read of the author's worktree was copying its existing ignored .p0ref directory into this review worktree.
- No commits, pushes, branch updates, fixes, package additions, P0 rebuild, Unity launches or protected-process signals.
- Only smoke-owned UI processes were launched, on OS-assigned loopback ports; scripts stopped their own PIDs. No reviewer UI process remains.
- Protected PIDs 1750123 and 1806361 remain alive on 127.0.0.1:33767 and :43649. Port :46311 remains unbound. PID 390578 was not restarted.
- Current GitHub active-tasks.md and issue #885 were fetched read-only. The live board has a manager registration; the more specific implementation lane and handoff are in #885's comments. The approved MA-1 spec was read from f8bbcf4679f7f22bc88b851da6394b26a4db1985. The user's current review packet controls custody.

Protection hashes independently checked before build and at completion:
- .p0ref/SitkaLab.Core.dll:
  9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
- fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json:
  bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4

## Gates actually reproduced

| Gate | Result |
|---|---|
| Solution build, SDK 8.0.134 | 0 warnings, 0 errors |
| Explicit UI build | 0 warnings, 0 errors |
| Core tests | 135/135, including 108 existing plus 27 new |
| tests/ui-smoke.sh, first run | 287/287 |
| tests/ui-smoke.sh, repeat | 287/287 |
| tests/ui-smoke-multiarea.sh, first run | 127/127 |
| tests/ui-smoke-multiarea.sh, repeat | 127/127 |
| Independent scratch Core probes | 59/59, repeated with the same result after adding view export |
| Independent manifest/geometry script | 16/16 |
| Results-label probe | Reproduced N1 |

No product-gate failure or flakiness was observed in these repeat runs. This does not prove flakiness is impossible.

Scratch helper failures, retained honestly:
- The first identity script assumed areaHa was a scalar rather than the manifest's typed {"k":"f32","b":"..."} value. It stopped after six passing assertions. Correcting only the external decoder yielded 16/16; identity-first-attempt.log retained.
- The first isolated renderHistory probe omitted the renderer's fixed helper and raised ReferenceError. Supplying that external helper reproduced N1; history-label-first-attempt.log retained.
- An early read-only coordination search failed because WSL rg was absent and a regex was split by shell quoting. The read was rerun successfully using a literal PowerShell here-string, gh and grep. No repository mutation resulted.

## Atomicity and owner safeguards

Source evidence:
- MultiAreaContinuation.cs:16 ? one MultiAreaRoot contains all area states, events, executed-key map, pending/approved plan, annual/work/plan revisions and history.
- :51 ? one sync lock.
- :222 ? Execute enters that lock.
- :248 ? entire approved batch revalidated before roster building; checks area, action, duplicate, exact living ID, Crop protection and reason.
- :268 ? reject a batch emptying any populated area before building any roster.
- :282 ? build every area's next roster and before/after summaries without mutating old roots.
- :307 / :312 ? afterAreaBuilt / beforePublish fault seams.
- :319 ? one reference assignment publishes rosters, event, work revision, executed key and cleared approval.
- :343 / :393 ? annual calculation similarly builds all area results under the one lock and publishes once.

The independent scratch harness references the built Core DLL through its existing friend assembly name. It does not compile replacement production code. It checks root reference identity and serialized content, including the private executed-key map, rather than only checking living counts.

Its 59 assertions cover:
- faults after area A, after area B and immediately before Execute publication; nothing published, no key recorded; retry commits exactly once;
- unminted key, altered replay PlanId/source, correct identical replay;
- wrong-area Add, dead ID, duplicate Add, stale source and stale plan revision;
- deliberate private test-instance corruption of approved orders: wrong area, duplicate ID, unknown ID, dead ID, invalid action and blank reason. Every refusal occurs before afterAreaBuilt is called;
- a combined plan that would empty A while leaving survivors in B: whole-plan refusal and preserved approval;
- pending and approved work both require consent; consent cancels the whole plan at zero additional Fell;
- faults during consented annual advance preserve the entire root and its existing work ledger;
- natural-empty area is carried forward unchanged, dead Crop marks removed, other area advances;
- all-empty Year+1 refusal;
- direct unchanged AnnualRules reports, computed in reverse area evaluation order, match actual per-area reports exactly;
- competition on all currently living trees leaves the root identical.

These tests use reflection only on disposable in-memory instances to reach otherwise unreachable malformed-approved-plan cases. No source/test file in the repo was edited.

## Per-area ecology and identity

MultiAreaAnnual.Step calls unchanged AnnualRules.Calculate with the area's own AnnualState, SimulationSeed and geometry-derived AreaHa (MultiAreaContinuation.cs:31, :366). There is no second growth or mortality implementation. Next-state construction follows AnnualContinuation. Work receipts/views call unchanged WorkRules.Summarize (:302, :525). That helper and AnnualRules.Stand use the same count, float division, ordinal roster accumulation, squared DBH, Dq, basal-area and relative-density arithmetic.

MultiAreaProjection.cs:13 projects only the selected area's trees, Crop marks and orders into unchanged PlayerCompetitionDiagnostics and PlayerMapScene. AnnualRules receives only those trees. No cross-boundary neighbours are injected. The committed tests also compare all 244 initial competition reports against annual-rule CI bits and identify 13 actual cross-boundary pairs.

Independent Python checks of the generated published manifest establish:
- body SHA-256 c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95;
- exactly two non-overlapping polygons (touching allowed), different vertex counts and sizes;
- independent integer shoelace areas and exact float32 hectare bits;
- all 244 points strictly inside their own polygon;
- globally unique IDs, including case-insensitive comparison, correct area prefixes and no P30 family;
- distinct ecological seeds, stocking, age structure and DBH structure.

The generator authors a 915 m? pentagon with 171 trees and a 1,500 m? quadrilateral with 73 trees. It does not read or slice P30. Initial ages are 24 in A and 29/47 in B. Generation is deterministic.

## Single-area change and disclosure

PlayerApi.cs:198 still parses the strict competition request, checks the session/source/plan-state fence, then calls the unchanged pure diagnostic. Only the Crop-required refusal was removed. Dead/unknown refusals and Host/capability/method/Origin/content-type/body guards remain. There is no mutation or automatic Crop designation.

The three ui-smoke.sh edits are narrow: K29 now positively verifies read-only non-Crop inspection and unchanged state; a page-witness wait requires non-Crop competition; J24 ignores only the read-only competition endpoint when locating the last command. No unrelated guard assertion was removed.

MultiAreaContracts.cs:108 onward and MultiAreaExperiment.cs:21 supply the synthetic/not surveyed, not comparable, site UNKNOWN, excluded edge neighbours, no treatment transfer and session-only statements. PlayerMultiApi.cs renders those statements in the disclosure/chips/details; its static page also says light, seed, regeneration, site, money and saving are NOT MODELLED (:441). No positive comparability, treatment-transfer or scientific-validation claim found. N1 is the one wording overstatement found.

## Exact verification commands

All build/test commands below ran in WSL Ubuntu bash, not Windows dotnet. Log redirections below are the ones actually used. The unchanged smoke scripts were inspected before execution.

Initial custody:

~~~bash
git -C /home/jer/Development/CCF-HP status --short
git -C /home/jer/Development/CCF-HP remote -v
git -C /home/jer/Development/CCF-HP worktree list
git -C /home/jer/Development/CCF-HP worktree add --detach   /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121   58950a9aa74067f22c9fe322b27f42e036ae2e01
~~~

Build and regression:
~~~bash
export TMPDIR=/tmp/ccf-ma1-review
export DOTNET=/home/jer/.dotnet/dotnet
export NODE=/home/jer/.local/share/vite-plus/bin/node
mkdir -p "$TMPDIR"
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121/Tools/MultiRackSitkaP1Lab
if test ! -e .p0ref; then
  cp -a /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-MA1/Tools/MultiRackSitkaP1Lab/.p0ref .p0ref
fi
sha256sum .p0ref/SitkaLab.Core.dll fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json
"$DOTNET" --version
"$DOTNET" build MultiRackSitkaP1Lab.sln > "$TMPDIR/solution-build.log" 2>&1
"$DOTNET" build src/Ui/SitkaP1.Ui.csproj > "$TMPDIR/ui-build.log" 2>&1
"$DOTNET" tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$TMPDIR/core-tests.log" 2>&1
tests/ui-smoke.sh "$TMPDIR/single-run1" > "$TMPDIR/single-run1.console.log" 2>&1
tests/ui-smoke-multiarea.sh "$TMPDIR/multi-run1" > "$TMPDIR/multi-run1.console.log" 2>&1
tests/ui-smoke.sh "$TMPDIR/single-run2" > "$TMPDIR/single-run2.console.log" 2>&1
tests/ui-smoke-multiarea.sh "$TMPDIR/multi-run2" > "$TMPDIR/multi-run2.console.log" 2>&1
~~~

Independent external probes (sources retained beside this report):
~~~bash
cd /tmp/ccf-ma1-review/probes
/home/jer/.dotnet/dotnet run --project ReviewProbes.csproj > /tmp/ccf-ma1-review/probes.log 2>&1
# Repeated after adding only export of the real naturally-empty view:
/home/jer/.dotnet/dotnet run --project /tmp/ccf-ma1-review/probes/ReviewProbes.csproj > /tmp/ccf-ma1-review/probes.log 2>&1
python3 -I -B /tmp/ccf-ma1-review/identity.py > /tmp/ccf-ma1-review/identity.log 2>&1
/home/jer/.local/share/vite-plus/bin/node /tmp/ccf-ma1-review/history-label.mjs > /tmp/ccf-ma1-review/history-label.log 2>&1
~~~

Final scope/custody:
~~~bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Review-20261121
git diff --name-status e11dd5192d7a89abf859613ce92814bdd6338ea2 58950a9aa74067f22c9fe322b27f42e036ae2e01
git rev-parse HEAD
git status --porcelain=v1 --untracked-files=all
git symbolic-ref -q HEAD
sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json
ps -p 1750123,1806361 -o pid,lstart,args
ss -ltnp | grep -E ':33767|:43649|:46311'
~~~

The outside-repo scope allowlist script passed 15/15 paths. Full diff is scope.txt; final clean status is final-status.txt.

## Author evidence versus reviewer observations

The author's final code SHA, 15-path inventory, protected hashes, manifest hash, two-area counts, zero-warning builds and 135/287/127 gate counts match my independent observations. The narrow single-area behavior change and declared packaging deviations also match the inspected diff. I observed no additional product-test failures.

I did not independently reproduce the author's 17/17 headless Edge run, eight historical smoke repeats, past failed attempts, historic process custody or screenshots. They remain author evidence. The handoff distinguishes author testing, review, integration and owner acceptance correctly. N1 slightly qualifies its otherwise accurate player-facing honesty.

## Not verified and required handoff limits

- No real rendered browser/layout verification in this review. Smoke page witnesses use a DOM stand-in. The isolated N1 renderer probe likewise does not qualify browser layout.
- No trusted/native mouse, keyboard, touch, human use, screen reader, OS high contrast, real browser zoom, or other browsers.
- No Unity import/runtime, Unity parity, Windows build, save/load, crash durability, multi-process behavior or persistence (session-only by design).
- No scientific/ecological validation, empirical comparability, G1, treatment transfer or site calibration.
- No independent reproduction of the author's rendered 1280x720 / 360 / 640 evidence.
- No owner acceptance or merge/integration decision.

This report and raw evidence are outside the repo as explicitly required by the read-only/no-commit/no-push review custody. A Manager must publish this result as a GitHub-tracked Markdown handoff and handle coordination/dashboard updates to satisfy AgentWorkflow's administrative closure requirement. No publication or dashboard synchronization is claimed here.
