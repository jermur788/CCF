# CCF 2D Simulator: Phase 2B-2b Part 2 Independent Review (Dispositions UI + Save v3 Economy Block + Deadwood Wiring)

- **Date:** 2026-11-27
- **Reviewer:** Claude Opus 5.5 (`claude-opus-5-5`), Claude Code under T3 Code. Independent read-only review sub-agent. I did not write this change.
- **Review lane:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2B2b-UI-Review-20261127`
- **Review branch:** `review/multi-2d-phase2b2b-dispositions-ui-v3save-20261127-v1`
- **Candidate reviewed:** `cd907446282ca6a0f12fdbc1d33dc03076f6ac58` (`feat/multi-2d-phase2b2b-dispositions-ui-v3save-20261127-v1`; `origin` tip matched this SHA when fetched)
- **Base:** `24871551ae906b6911b48bb17c252c023bdea502` (`origin/integration/2d-sim`; base is an ancestor of the candidate)
- **Commits in range:** `7ab0afad` (core/deadwood/S1/codec), `2f0f2a1f` (UI/routes/tests), `cd907446` (handoff doc)

All paths below are relative to `Tools/MultiRackSitkaP1Lab/` unless they start with `Docs/`.

## 1. Scope

I compared base `24871551` with candidate `cd907446` against requirements A (the `/multi` screen and HTTP routes), B (the Save v3 economy block) and C (deadwood and the S1 rule). I also checked the writable fence and the handoff document's claims.

## 2. What I inspected

- `git diff --name-status` and `--stat` for the full range: 28 files, +2637/−114.
- **Fence:**
  - Every changed path is inside the authorised set: `src/Core/Phase1/**`, the three named `src/Core/Saves` files, the two `MultiAreaContinuation` files, the two Economy files, `src/Ui/{MultiRoutes,MultiApi.*}.cs`, `src/Ui/multi/**`, `tests/**`, and the `phase2b-economy` handoff folder.
  - No changes under `Assets/**`, `Packages/**`, `ProjectSettings/**`, `src/Ui/wwwroot/**`, `SpeciesWave2`, `WoodlandImport`, `NaturalProcesses`, `build/2d-gate/**` or `Docs/Project/**`.
- **JS module count:** `git ls-tree -r` lists 16 `.js` files under `src/Ui/multi` at both the base and the candidate, so no new module was added. My brief says "14"; the repo has 16 at base. That count includes files outside the brief's definition of a module, and the unchanged count is what matters.
- **Full code read** of these diffs:
  - `EconomyFeatureBlockCodec.cs` (new, all 981 lines)
  - `SaveV3Codec.cs`, `FeatureBlockRegistry.cs`, `GenesisRecipeBlockCodec.cs`, `Phase1SaveFormat.cs`
  - `MultiAreaContinuation.cs`, `MultiAreaContinuation.Commands.Plan.cs`, `MultiAreaContinuation.Commands.Economy.cs`, `EconomyQuoteCalculator.cs`
  - `MultiApi.{Commands,Saves,View}.cs`, `MultiRoutes.cs`
  - every `src/Ui/multi` JS/HTML/CSS diff
  - every test diff
- **Supporting code read:**
  - `FelledStemDeadwood.Materialise`
  - `Phase1SaveValidator` Stage 4 deadwood rules (`NextDeadwoodNumber == Deadwood.Count + 1`, sequential `DW####`, dead tree of its own area)
  - `Phase1RootMapping` deadwood round trip, including `Cause`
  - `MultiAreaModelSelection.Phase2WithEconomy` and `IsStage4`
  - the v2-load-under-Economy refusals (`MultiAreaContinuation.cs:679`, `:704`)
  - `ReadMultiSavesAsync` slot listing
- **JS arithmetic audit:** I searched the `src/Ui/multi` diff for arithmetic on money, m³, hours or prices and found none. Every figure displayed is a server `*Display` string or a server count.
  - The only literals are fallback strings (`'€0.00'`, `'0.000 m³'`) shown when a server line is absent.
  - Per-year retained m³ and per-species totals for History are computed in C# (`MultiApi.View.cs` `ProjectEconomyHistoryYear`, `RetainedTimberTally.FromStems`), not in JS.
- **Handoff document:** I spot-checked it against the code (§5 below).

## 3. Checks actually run (in my worktree only)

Toolchain: `$HOME/.dotnet` SDK 8.0.134. Host load average was about 0.5 to 1.0.

| Check | Command | Result |
|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln -c Release` | PASS, 0 warnings, 0 errors |
| Save test build | `dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj -c Release` | PASS, 0 errors |
| Core.Tests | `dotnet run -c Release --no-build --project tests/Core.Tests` | **314/314 passed**, exit 0 |
| Phase1.Save.Tests | `dotnet run -c Release --no-build --project tests/Phase1.Save.Tests` | **52/52 passed**, exit 0. Includes `golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding`; the test asserts `24703` bytes at `tests/Phase1.Save.Tests/Program.cs:736`. |
| EconomyDispositions | `dotnet run -c Release --project tests/Core/EconomyDispositions` | **22/22 passed**, exit 0 |
| Worktree cleanliness | `git status --short` after the runs | clean |

**Environment note (honest disclosure):**
1. I first ran `build/build-p0-core-ref.sh` in my lane. It wrote only the git-ignored `.p0ref/`, but it produced hash `bcbdbf05…`, which did not match the pin.
2. I replaced it with the pinned copy (sha256 `9a99f78131a34803…`) from `/home/jer/.ccf-orch/standmap-p1-opus/tier1-clone-30fa269c/Tools/MultiRackSitkaP1Lab/.p0ref`.
3. All results above use that pinned copy. No tracked file changed.

**Not run, by instruction:**
- the Edge/CDP suite `tests/phase2b2b-ui/run-phase2b2b-edge.sh`
- `build/2d-gate/selftest.sh`
- `build/2d-gate.sh --browser`

I started no servers and used none of ports 33767, 43649 or 46593.

## 4. Findings

### BLOCKING

None.

### SHOULD-FIX (non-blocking for this integration; fix in a follow-up)

**SF-1. The economy block decode does not cross-check against the rest of the restored root.**

`EconomyFeatureBlockCodec.Decode` / `ValidateInvariants` (`src/Core/Phase1/EconomyFeatureBlockCodec.cs:247-277`, `:880-932`) check the economy block on its own terms: cash/ledger reconciliation, per-settlement sums, volume conservation, retained tally and minutes. They do not reconcile it with the rest of the root:

- A settlement whose `executionKey` has no matching `Executed` record is silently accepted (`:263-269` only attaches matches).
- An Economy-era `Executed` record with no settlement is not detected.
- The top-level `ledger` is checked only by count (`:924`) and total sum (`:927-930`). It is not checked to be the in-order concatenation of the settlements' `ledgerEntries`.
- `yearEndSummaries.Count` is not tied to `EcologicalYear`.
- Settlement `deadwoodIntents` are not reconciled with the `fell`-cause Stage 4 deadwood records.

The engine's own saves are consistent, and every engine-made save tested here round-trips. However, the v3 outer checksum can be recomputed (the tests do so with `ResignV3`), so a hand-edited file can load into an internally inconsistent economy history.

Recommendation: add these cross-checks in `Decode`, refusing with `SaveInconsistent`. That makes the "refuse, never coerce" guarantee hold across the whole root.

**SF-2. A superseded Edge suite is left broken.**

`tests/phase2b2a-economy-ui/drive-economy.mjs:380-395` still asserts `!hasKeepForUse` and the `sell-and-extract` disclaimer text, which this change deliberately replaces. The handoff admits this (§6 Residual Risk 1).

It is not wired into `build/2d-gate/**` (`grep` finds no reference), so the gate is unaffected. Still, `tests/**` is inside the fence, and a known-failing suite should be retired or updated rather than left in place.

### NON-BLOCKING / NITS

- **NB-1. Dead probe in a test.** `tests/Core.Tests/Phase2A2Tests.cs:194-195` builds `badCash` and asserts only `badCash != econJson`. It is never decoded. The real cash-tamper check that follows (`cashMismatch`, refusing `SaveInconsistent`) is meaningful. Remove the dead probe or decode it.
- **NB-2. Two different refusal codes for a Money-Off save with an empty economy block.** At `tests/Core.Tests/Phase2A2Tests.cs:223` the default registry refuses `MissingKey`, because block decode runs before the new `EconomyEnabled:false` plus block cross-check in `SaveV3Codec.cs` (+221-228). A Money-Off save carrying a *valid* economy block refuses `SaveInconsistent`. Both fail closed. The code depends on what the forged block contains; consider running the presence check before block decode so the code is deterministic.
- **NB-3. The inner capture can still produce a v2 envelope for a Forest Maker Economy root.** `MultiAreaContinuation.CaptureSave` (`src/Core/MultiArea/MultiAreaContinuation.cs:570-583`) now permits Forest Maker plus Economy. Its result is safe only through `CaptureSaveBytes`, which routes to v3 with the economy block (`:599-602`). The only production caller is `CaptureSaveBytes` (`src/Ui/MultiApi.Saves.cs:130`). A future direct caller passing that envelope to `Phase1SaveCodec.Encode` would drop economy state. Consider making `CaptureSave` private, or documenting that it must only be used through `CaptureSaveBytes`.
- **NB-4. Pending dispositions reset after load.** By design (Policy A), pending non-default Fell dispositions reset to Sell & Extract after a v3 load (`EconomyFeatureBlockCodec.cs:317-333`). The rows show the active disposition, and approval is dropped anyway. This matches the stated engine-handoff policy; I note it here for owner awareness.
- **NB-5. Execute-time S1 guard has no direct test.** The guard at `src/Core/MultiArea/MultiAreaContinuation.Commands.Plan.cs:258-261` is only defence in depth. The choice-time and quote-time guards are each tested, including an injected-state probe.

## 5. Handoff document spot-check

| Handoff claim | Matches the code? |
|---|---|
| File list (28 paths) | Yes |
| Diff stat (27 code/test files, 2440 insertions, +197 for the doc = 2637) | Yes |
| Test counts 314 / 52 / 22 | Yes; I reproduced them independently (Release build; the author used Debug) |
| v2 golden 24,703 B | Yes; the assertion is present and passes |
| S1 guard sites (`Commands.Economy.cs:117`, `EconomyQuoteCalculator.cs:274`, `Commands.Plan.cs:258`) | Yes |
| Policy A restore behaviour | Yes |

The "Not Verified" section is honest. It lists no Unity work, no full gate, and no browser-driven built-in Save dialog.

The SDK-hash residual risk (Risk 3) matches what I saw: a local `build-p0-core-ref.sh` run does not reproduce the pinned hash. The Edge result (66/66) is **implementer-reported only**; I did not run it.

## 6. Requirement verdicts

**A. `/multi` screen and HTTP routes: PASS (code inspection plus core tests; browser layout NOT TESTED by me).**
- Route `POST api/multi/economy/disposition` is registered (`src/Ui/MultiRoutes.cs`). It has bounded `disposition` text and a strict case-sensitive enum parse that refuses `EconomyDispositionUnknown` (`src/Ui/MultiApi.Commands.cs` +208-217).
- The UI has three disposition buttons with Sell & Extract as the default fallback, and no `<input>` under Keep for Own Use (`src/Ui/multi/panels/work-plan.js` +37-87). Approved rows show a read-only pill. Buttons are disabled while blocked or plan-locked (`api.js` +517).
- Retained m³ comes from the engine (`MultiApi.View.cs` `ProjectRetainedTimber` / `ProjectStemDisposition`).
- The existing behaviours are preserved: approval rejection, fence, contractor-only felling, the €2,500 floor highlight (`data-floor-applied` is kept), and the calibrated-estimate disclaimer (reworded, still honest).
- The top strip gains `Retained` alongside Cash and Landowner (`multi.html` +68-71).
- Dispositions and retained m³ appear inside the existing History Financial Ledger section, which is shown only when Money is On (`time-history.js` +499-525). No new History button was added in the diff, and the Money-Off `NOT RECORDED` path is unchanged.
- I found no JS arithmetic.
- **NOT TESTED by me:** the 1920×1080 / 1280×720 layout, horizontal scrolling, column count and single-History-button counts in a real browser. These are implementer-reported only.

**B. Save v3 economy block: PASS.**
- Codec: key `economy`, `CurrentBlockVersion = 1`, `CanRead(v) => v == 1` (`EconomyFeatureBlockCodec.cs:232-236`).
- Encoding is canonical and fail-closed:
  - fixed-order key arrays are enforced at every level (`CanonicalKeys`, `:936-946`)
  - i64 values use tagged 16-hex encoding and f32 values use bit encoding
  - decode re-encodes and requires exact string equality (`:337-339`)
  - an unknown enum refuses `SaveEnumUnknown` (`:970-976`)
  - an unknown key refuses `UnknownKey` (tested)
  - a bad version refuses `SaveBlockVersionUnsupported` (tested)
  - a missing block with `economyEnabled:true` refuses `SaveRequiredBlockMissing` (`SaveV3Codec.cs`, `MultiAreaContinuation.cs` +623-625)
- Registered in `FeatureBlockRegistry.CreateDefault` (`:28`).
- A Save → fresh slot reload → `RestoreFromV3` round trip preserves Money on/off, cash, used and remaining minutes, retained m³ (total and species count), the thinning-stage count, settlements, ledger and year-ends. It also keeps the felled deadwood, and the save continues deterministically for 2 years (`Phase2A2Tests` and `Phase1.Save.Tests`, which I ran). Mismatched `EconomyOptions` refuse `SaveOptionsMismatch`.
- v2 stays frozen: 24,703 B golden, v2 load under Economy refused (`MultiAreaContinuation.cs:679`, `:704`). Built-in plus Economy refuses `SaveEconomyRequiresV3` (`:575-577`), and the UI surfaces this.
- Cross-structure tamper consistency is incomplete; see SF-1. It does not block this decision because engine-made saves cannot reach those states.
- The "server restart" leg is covered only by the implementer's Edge run, which I did not run. In the unit tests, a fresh `SaveV3Slots` reload with a fresh session is the closest equivalent.

**C. Deadwood and S1: PASS.**
- In Execute, settled `DeadwoodIntents` are materialised into `Stage4.Deadwood` with `NextDeadwoodNumber` advanced. If Stage 4 is null, Execute refuses rather than dropping the intents (`MultiAreaContinuation.Commands.Plan.cs:258-270`).
- `Materialise` rejects duplicate trees and numbers records from the current counter. This keeps the validator invariant `NextDeadwoodNumber == Deadwood.Count + 1` and the sequential `DW####` ids. Tests check the ids, the cause `fell`, the f32 volume bits, and that a replay does not duplicate records.
- The S1 rule is fail-closed at the choice (`Commands.Economy.cs:117-119`), at quote/approve via `ComputeCore` (`EconomyQuoteCalculator.cs:274-276`, tested with an injected Stage-4-null root), and at Execute.
- Restored roots cannot bypass it: `PendingFellDispositions` always decodes empty (`EconomyFeatureBlockCodec.cs:331`).
- `/multi` and v3 restore use `Phase2WithEconomy`, which has `IsStage4` true (`Stage4Models.cs:83`, `MultiAreaContinuation.cs:619`, `MultiSessionHost.cs:94/116`).
- The volume identity and ledger sum are kept: the calculator is unchanged apart from the S1 guard, and the codec re-validates `sale + residual + retained + deadwood == total` and `sum(ledgerEntries) == net` per settlement (`:901-914`).

## 7. What I did not verify

- The headless Edge suite (`tests/phase2b2b-ui`) and therefore: real-browser layout at 1920×1080 and 1280×720, no horizontal scrolling, at most 3 columns, exactly 1 History button, zero console/CSP errors, and the real server-restart Save → Load path. These are **implementer-reported only**.
- `build/2d-gate.sh` (with or without `--browser`) and `build/2d-gate/selftest.sh`.
- Mutation testing; I ran none.
- Unity, Blender or any 3D path. None was touched or needed.
- That the 6 JS files' text changes contain no regressions in flows I did not read end to end (for example, the full saves dialog load flow beyond the diff).

INTEGRATION DECISION: APPROVE
