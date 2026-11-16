# Sen-1 T-03A independent source review (delta 460fd51c..09ade4c4)

**Claim [#571](https://github.com/jermur788/CCF/issues/571), orchestrator [#544](https://github.com/jermur788/CCF/issues/544), author claim [#570](https://github.com/jermur788/CCF/issues/570).** Offline source review only. Not a Unity compile, not a runtime test, not integration. It ticks none of the 63 ISO items.

## Verdict

**APPROVED for the T-03A delta, as a fail-closed interim seam. 0 BLOCKER / 0 MATERIAL / 4 MINOR / 4 NIT.**

The approval is narrow. It does **not** say the packet's requirements 1 and 2 are fully met. The scene still builds a world through two unguarded siblings, and a part-way load failure still leaves a changed live world. Both are named, recorded and caused by the allowlist or the Unity lifecycle (see §7). T-03B must close them. Nothing here authorises T-04, T-05, merge or any Unity run.

| Record | Value |
| --- | --- |
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code harness inside T3 Code. **Effort: set by the dispatching harness; not observable from inside the session.** |
| Independence | **Same provider as the author** (author: Anthropic Claude Opus 5.5). This is **not cross-provider independence.** The T-02R delta reviewer was also Sonnet 5.5, but it did not write T-03A. A cross-provider re-check of T-02R and T-03A is still owed when an OpenAI pool returns. |
| Reviewed | Branch `feature/scen1-shared-three-profile-save-v2`, head `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1`, source `b730459a`. Delta base `460fd51cb8972dd5b7819632c94e64d1070a98b7`. |
| Method | Committed bytes only, exported with `git archive` into a scratch tree. Docs read with `git show`. The source worktree was not touched. Nothing was read from `/media/jer/Files/CCF-Primary`. |
| Evidence | `/media/jer/Files/CCF/Verification/SharedSaveT03A-571-Review-20261116/` |

## 1. Counts

| Check | Result |
| --- | --- |
| Author's `cand-src` C# files vs committed bytes | identical (the only apparent diffs were my loop splitting two `.mat` filenames with spaces; no C# differs) |
| Offline harness (T-02R tests + T-03A tests), rebuilt from committed bytes | **748 PASS / 0 FAIL** (matches the author) |
| Original T-02R tests, unchanged, on the **new** source (only `Debug.LogException` stub added) | **460 PASS / 0 FAIL**. The sorted PASS lines are line-for-line identical to the T-02R harness on `460fd51c` (empty diff). |
| T-02R harness on `460fd51c` | 460 / 0 |
| Roslyn ordering proof (`ordering.dll`) | **18 PASS / 0 FAIL**, same as the author |
| Reviewer probe (one extra assertion block, §5) | 752 PASS / 0 FAIL on committed source |
| Mutants T1–T16 (author's list, my strict classifier) | **15 killed, 1 survived (T7).** T4–T16 alone: **12 of 13 killed.** |
| Reviewer-designed mutants X1–X8 | **8 of 8 killed** |
| Unity 6000.6 reference typecheck, **player** defines | **0 errors** (192 files, 125 refs) |
| Same, **Editor** defines | **0 errors** (192 files, 229 refs) |
| Extra: Editor defines **including** the 9 `/Editor/` folder files | **0 errors** (201 files, 229 refs) |
| Typecheck of `460fd51c` as control | 0 / 0 / 0 |
| Typecheck positive control (`Debug.LogException` renamed) | **1 error**, CS0117 at `ForestSaveController.cs:150`. The check can fail. |

**The typecheck is not a Unity compile.** It is Roslyn against the real `UnityEngine*.dll`, `UnityEditor*.dll` and the CCF-U3 ScriptAssemblies. It has no asmdef layout, no ILPP, no domain reload and no import. The `UNITY_EDITOR` define set still omits `/Editor/` files unless I add them, which I did as a third run.

## 2. Harness changes: neither weakens a check

I diffed the T-02R harness template against the T-03A template, file by file.

- `Tests.cs`: **+1 line** (the call into `T03ATests.Run`). Nothing deleted or edited.
- `Stubs.cs`: a `Debug.LogException` stub, and a `ThrowAfterPartialApply` flag plus counter. The flag is off by default.
- `generator/Program.cs`: the stubbed `ApplyAdmittedData` body gains a branch that runs **only** when the flag is set. With the flag off it is the same statements as before.
- `prepare.py`: adds the new test file to the compile list.
- `MenuRoutes`, `PreviewRoutes`, `CaptureManager`, `BoundaryController`, `FixtureTests`, `Schema`, `ArchiveExtractor`, both shell sources and both baseline sources: **byte-identical.**

The author named two changes. There are in fact three files carrying them (the stub, the flag, the generator branch), but they are the same two ideas. The 460-on-new-source run above is the real proof that nothing was loosened.

**Limit the author did not overstate but a reader should know:** the harness replaces `ApplyAdmittedData` with a stub, so the real apply body, `Destroy` timing and real exception sites are unobserved. The harness extracts the **real** `AdmittedSave.Apply`, `TrySave`, `CaptureData` and `TryPrepareCapture` from source, so the lock logic itself is genuinely exercised.

## 3. Ordering proof: holds inside the manager only; partly cosmetic for the scene

**What holds.** `ScenarioOneManager.Awake` consults `TryAuthorizeWorldConstruction` at `ScenarioOneManager.cs:465-467`, before every world mutation in that method. The mutations are the four `ecology.*Model` assignments, `ApplyStandGeometry`, `InitializeNewScenario` and `InitializeRackPlanForNewWorld`, all in the `else if` branch at `:468`. The gate is pure (`ForestSaveSessionContext.cs:47-68`). My mutant T13 (a world mutation placed before the gate) was killed, but **only by the Roslyn ordering proof**; the harness has no manager `Awake` to run.

**What does not hold for the scene.** Two siblings are unguarded, as the author says:

- `ForestStartingStand.Awake` (`ForestStartingStand.cs:65-70`, `[DefaultExecutionOrder(100)]`) calls `ecology.ApplyStandGeometry(...)` and `Generate()` unconditionally. It runs **after** the manager's `Awake` (default order 0), so the manager's refusal is already made by then, but nothing reads it.
- `ForestEcologyController.Awake` (`:189-196`) calls `RebuildGrid()` and adds `StandWorldBoundary`. Neither it nor the manager has an execution-order attribute, so their relative order is not fixed by the code.

So on a refused selection the manager builds nothing, yet the grid, boundary, geometry and the full starting stand are still built. The accurate statement is: **the gate is real and correctly placed inside `ScenarioOneManager.Awake`, and it is partly cosmetic for "no world exists to corrupt".** The gate is also **unreachable today**: `BeforeSceneLoad` always seals Historical and `ScenarioOne.asset` carries `scenario-one` (the only definition in Assets). Both files are outside the allowlist, so this is a named, recorded seam (result doc §3 table, §7 item 1), not a defect. The result doc's §4 line "Refused selection → nothing constructed" is true of the pure gate and overstated for the scene (MINOR-2).

## 4. The fail-safe claim (judged hardest)

**Claim.** A part-way load failure refuses the load and disables Save until a whole save loads or a new forest starts; the previous save stays byte-identical.

**What I verified in the committed code.**

- `ApplyAdmittedData` has exactly one return, `return true` at its end (`ForestSaveController.cs:830`). So no exception-free partial path slips past the lock today.
- `AdmittedSave.Apply` catches every `System.Exception` from the mutating call (`:136-139`), logs it, sets `incompleteWorldReason`, shows a refusal and returns false (`:150-155`). Admission and teardown still precede the first mutation.
- `TrySave` refuses on the lock at `:208`, after the profile check and **before** `CaptureData()` and any temp-file write. The only writer (`WriteSaveAtomically`, called only at `:226`) is behind it. The harness hashes all nine slot paths plus sentinels before and after F5, `TrySave` and menu-style `TrySave`: unchanged.
- A refused foreign load keeps the flag; a whole disk load clears it; `Restart` reloads the scene and builds a new controller (it is not `DontDestroyOnLoad`; only the menu host is), so it clears it.

**Can a player be left unable to save?** Yes, for a session, by design: from a part-way failure until a whole load or a fresh forest. **It cannot strand a player permanently**, and it is not a regression against the old behaviour, where the half world was saved over the good file on the next F5. The ways out exist and work:

1. F9 or the menu "Load your same-build save", if that file is whole.
2. F10 → "Start a fresh Scenario One test…" (`StandaloneSessionMenu.cs:121`, `Restart` at `:162`). The menu is reachable except during a Reference preview.
3. Quit and relaunch. The flag is in memory only; nothing is persisted, so the next launch starts clean and the old save is untouched.

**Is the message honest?** Mostly. "Save refused: the last load stopped part-way, so this forest may be incomplete. Load a save or start a new forest first." is accurate. The load message ("Save not loaded: it stopped part-way. This forest may be incomplete, so saving is off…") opens with "Save not loaded", which understates that the live world was partly replaced. The second sentence corrects it. It does not tell the player how to start a new forest (F10).

**Discoverability gap (MINOR-3).** The message lives for 3 s (`SetMessage`, `messageTimer = 3f`). The two menu entry points close their UI inside `onAdmitted`, **before** apply (`StandaloneSessionMenu.Startup.cs:95-100`, `StandaloneSessionMenu.cs:126`). On a part-way failure they then return silently (`!IsOpen`), so the refusal reaches the player only as a brief HUD flash. After a failed startup **Continue** the player is in a hybrid world with no startup screen and learns Save is off only when they press F5. Not stranding, but a sharp edge.

**Reference preview round trip.**

- Wrongly **re-enabling**: I could not produce it. A capture records the flag (`:310`), an admission built from it carries the flag (`:626` → `:85`), and a whole apply sets the flag to that carried value (`:144`). So preview entry from an incomplete world clears the flag while the preview guard blocks disk saves, and the return restores it. Mutants T3, T4 and X1 each break one link of this chain and are killed.
- Wrongly **leaving disabled**: a whole apply of a whole capture clears it, so a retry after a failed return unlocks. Tested.
- **Pre-existing, not new:** if a captured return world fails to apply every time, the player is held in preview (F10 and disk load/save are refused during preview). That is the T-02R retry design plus an unchanged `Open()` guard. The lock adds nothing to it.
- **Entry failing part-way (disclosed seam, MINOR-4):** `ScenarioOneManager.cs:717-719` still says "rejected before anything in the world changed" and sets "preview was not opened". With the new catch, `Apply` can return false **after** partial mutation, so that comment is now false: the player's own forest may be half-replaced and the local `returnData` snapshot is dropped. Disk is protected by the lock. The manager is allowlisted for `Awake` only.

**Over-conservative case.** The flag is set on any exception from the mutating call, including one raised before any mutation. A player whose world is actually whole could then be unable to save it. Improbable (the first statements are `Find*` calls), so I record it only as part of MINOR-3.

**Judgement.** The fail-safe is correct for what it claims (disk protection) and does not claim world preservation. It is not MATERIAL, because no permanent stranding path exists and recovery is available in-session. It is a real player-visible cost that T-03B should address with persistent feedback.

## 5. Mutation completion

Script read first. The author's `mutants_t03a.py` computed `killed = ('rc=0' not in stdout) or ...`. **A harness build failure (exit 4) would therefore count as a kill.** I wrote `mutants_review.py`, which checks that `harness.dll` was built, then classifies by assertion failure or crash, then ordering-proof failure, else SURVIVED. All 16 mutants and all 8 of mine built.

| Mutant | Result | By |
| --- | --- | --- |
| T1 partial apply propagates | killed | harness crash (injected exception escapes) + ordering proof |
| T2 no save lock | killed | harness assertion (F5 wrote) + ordering proof |
| T3 capture launders flag | killed | harness assertion |
| T4 success always clears | killed | harness assertion |
| T5 no flag on failure | killed | harness assertion |
| T6 partial reported loaded | killed | harness assertion |
| **T7 refused load clears flag** | **SURVIVED** | 748/0 |
| T8 historical admits 2 | killed | harness assertion |
| T9 unbound 2 admitted | killed | harness assertion |
| T10 default fallthrough | killed | harness assertion |
| T11 gate ignores seal | killed | harness assertion |
| T12 gate ignores chapter | killed | harness assertion |
| T13 mutate before gate | killed | **ordering proof only** (harness 748/0) |
| T14 prefs for unavailable | killed | harness assertion |
| T15 owns ignores menu-help | killed | harness assertion |
| T16 lock after capture | killed | **ordering proof only** (harness 748/0) |
| X1–X8 (mine) | **8/8 killed** | harness assertions |

X-mutants: restore drops carried flag; failure shows no HUD message; success never clears; gate case-insensitive; owns-key ignores registration; null profile treated as historical; Save refusal silent; flag set only when `showMessage`.

**T7 is a genuine coverage gap, not equivalent.** It clears the flag in the refusal branch inside `Apply` (`ForestSaveController.cs:123-129`). The author's test "refused (foreign) load keeps the flag" is refused earlier, in `TryReadSave`, so it never reaches that branch. I added one probe (`tpl-probe/T03ATests.cs`, `ReviewProbe`): set the flag, build an admission, swap `Current` so the context mismatches, call `Apply`, and assert refusal plus flag kept. On committed source it passes (**752/0**). Under T7 it fails (`FAIL T03A PROBE refusal inside Apply keeps the incomplete-world flag`). So production code is correct and the harness should gain this check. The branch is practically unreachable through `TryLoad`/`RestoreCapturedData`, which run `TryPrepare` immediately before `Apply`, hence MINOR.

T13 and T16 are killed only by a syntactic proof. That is a legitimate kill under the author's own design, but it is weaker than a behavioural one, and the harness cannot observe `Awake` at all.

## 6. Other requirements

**Geometry 2 (req 4).** `ValidateGeometryBinding` (`ForestSaveValidation.Geometry.cs:29-44`) names 0, 1, the reserved 2 and a refusing `default`. 0 and 1 are admitted for Historical only; **2 is refused for Historical, Compact, Scenario Two and null**; everything else is "unknown". Historical admit/refuse parity with the baseline validator holds for versions 1/19/20/23/24/25 × models −3..5, with identical reasons except model 2. `StandGeometryModel.cs` is not in the diff, so `IsKnown(2)` stays false and `Label(2)` stays `unknown(2)`. The refusal text contains no "30", "36" or "cell". New-profile envelopes cannot reach this validator today (profiles unavailable), so "every profile" is shown by the unit dispatch, not the live path. **Latent seam, recorded:** `StandGeometryModel.StandSizeMeters` still returns 40 m for any value but 1, and `ForestStartingStand.Generate` still falls back to Legacy. A direct `ApplyStandGeometry(2)` would silently build a 40 m world. No validated load can get there.

**Out-of-scope items (req 5).** The diff is five C# files and one Markdown file. No `.meta`, scene, prefab, asset, UI, profile chooser, compact world, H1, path, G1, CG-TRIG, teaching completion or availability flip. `Compact` and `ScenarioTwo` still construct with `isAvailable: false` (`ForestSaveProfiles.cs:65,71`, not in the diff).

**Scope and protection (req 6).** `git diff --name-status` shows exactly the five allowlisted files and the result doc. `ForestSaveData.cs`, `ScenarioReferenceArchive.cs`, `ScenarioOneInteractionVerification.cs`, `ForestSaveValidation.cs` and `ScenarioOneSaveMigration.cs` are untouched. The four signed blobs are absent from the branch tree. `git diff --check` is clean.

## 7. Is each "stopped at the seam" legitimate?

The record has to separate three cases: the allowlist, the Unity lifecycle, and the author simply stopping. I found **no case of the third.**

| # | Seam | Why | Judgement |
| --- | --- | --- | --- |
| 1 | Gate `ForestStartingStand.Awake` and `ForestEcologyController.Awake` | Neither file is on the packet allowlist, and the packet makes a fifth-plus file a stop condition. | **Legitimate: allowlist.** |
| 2 | True off-scene staging with the old world kept (FR-04) | Unity finishes `Destroy` at end of frame, so a same-frame reapply cannot be proven to restore the world. FR-04 itself says return a revised candidate rather than claim a rollback. A staged world also needs scene design outside the allowlist. | **Legitimate: lifecycle and allowlist.** The row label "Implemented as fail-closed refusal plus a Save lock" is generous: the live world **is** changed. The packet says "a half-applied scene is a failure of this task", so this requirement is **not met**, only contained. The author said so in §2 and §6. |
| 3 | Manager preview code (player feedback on failed return, keep snapshot when entry fails) | Manager is allowlisted for `Awake` only. | **Legitimate: allowlist.** |
| 4 | Geometry consumers (`StandGeometryModel.cs`, `ForestStartingStand.cs`) | Not allowlisted. | **Legitimate: allowlist.** |
| 5 | Wiring preference consumers (`LearningObjectivesView*`, `MenuHelpView`) | Not allowlisted. The new `TryGetPreferencePrefixes` and `OwnsPreferenceKey` have **no callers**; they are dead seam code until wired. | **Legitimate: allowlist.** |
| 6 | `ScenarioOneSaveMigration.cs` unchanged | Capture was already observational; no split was needed. Checked: nothing in the diff needs it. | **Correctly not done.** |

The packet's §5 says to publish a blocker and stop when staged application cannot be done inside scope. The author shipped a conservative interim lock instead and recorded the seam list. That is a reasonable judgement and stays inside the allowlist, but it is a departure the Manager should know about.

## 8. Findings

**BLOCKER / MATERIAL: none.**

**MINOR-1: Genuine mutation survivor T7; a branch has no test.** `ForestSaveController.cs:123-129`. The pre-mutation refusal inside `Apply` is not covered by "refused load keeps the flag". Failure scenario: a later edit clears the flag there and the harness stays green. Production code is correct (probe 752/0). Fix: add the probe block to `T03ATests.cs`.

**MINOR-2: Ordering claim is partly cosmetic for the scene.** `ForestStartingStand.cs:65-70` and `ForestEcologyController.cs:189-196` run regardless of the manager's refusal. Scenario: a future second profile is selected but refused; the manager logs an error and builds nothing, yet an 80 m grid and stand are generated and the half-initialised manager keeps running (behaviour untested). Result doc §4 "nothing constructed" overstates this. Unreachable today. Closes in T-03B.

**MINOR-3: Part-way failure is easy to miss.** `ForestSaveController.cs:152-155` (3 s HUD only), with `StandaloneSessionMenu.Startup.cs:95-100` and `StandaloneSessionMenu.cs:126` closing their UI before apply. Scenario: startup Continue fails part-way; the startup screen is gone, the player plays on in a hybrid world, and only F5 reveals saving is off. The opening "Save not loaded" understates the change to the live world, and no text names F10 as the way to a fresh forest.

**MINOR-4: Stale comment and message after the new catch.** `ScenarioOneManager.cs:717-719`. "Rejected before anything in the world changed … preview was not opened" is no longer guaranteed when `Apply` returns false. Scenario: archive apply fails part-way; the manager says nothing changed while the HUD says the forest may be incomplete, and the player's snapshot is gone. Disclosed seam; disk is protected.

**NIT-1: A false return with no exception is neither locked nor cleared.** `ForestSaveController.cs:140-145`. Unreachable today (`ApplyAdmittedData` always returns true). A future early `return false` after mutation would slip past the lock. Treat `!applied` as incomplete defensively.

**NIT-2: Bootstrap inventory is incomplete.** Result doc §3 lists four `RuntimeInitializeOnLoadMethod` sites. There are seven. The three extra are `AfterSceneLoad` verification scripts (`ScenarioHabitatPresentationVerification.cs:62`, `RecentAssetReviewVerification.cs:19`, `ScenarioOneInteractionVerification.cs:24`). The conclusion (nothing but the selection seam runs before scene `Awake`) still holds.

**NIT-3: Comment overstates signature status.** `ForestSaveValidation.Geometry.cs:22` says "The signed contract reserves…". The `200add4e` spec blob labels itself "APPROVAL CANDIDATE V2 — NOT APPROVED" and C-14 "awaiting signature". The authority for the reservation is the packet, and I could not verify the signature itself from the blobs.

**NIT-4: The author's mutation script counts build errors as kills.** `mutants_t03a.py`. Not a defect in the delta, and none of the 16 mutants failed to build, but any harness-only "all killed" claim from that script should be re-run with the strict classifier.

## 9. Not verified

- No Unity Editor or Player, no compile, import, domain reload or engine callback order (`SubsystemRegistration` → `BeforeSceneLoad` → `Awake`). The gate's fail-closed dependence on `BeforeSceneLoad` sealing is a Unity assumption unproven here.
- The real `ApplyAdmittedData`, `Destroy` timing and real exception sites. Whether `Debug.LogException` plus the HUD text reads well at real HUD scale.
- Whether any exception thrown inside a Unity lifecycle callback during apply (for example `OnDisable` after `SetActive(false)`) is swallowed by Unity before reaching the catch.
- Native `JsonUtility` behaviour (the harness uses a Newtonsoft stub).
- That the signed contract was in fact signed; I read only the blobs.
- Cross-provider independence. No real save, PlayerPrefs or archive was touched. No ISO item, board or active-tasks write.

## 10. Reproduce

```
R=/media/jer/Files/CCF/Verification/SharedSaveT03A-571-Review-20261116
$R/run_harness.sh $R/src-09ade4c4 $R/harness-template $R/work-control control     # 748/0
$R/run_harness.sh $R/src-09ade4c4 $R/tpl-probe $R/work-probe-control probe        # 752/0
python3 $R/mutants_review.py            # T1-T16, strict classifier
python3 $R/mutants_extra.py             # X1-X8
python3 -I $R/unity_typecheck.py $R/csonly-09ade4c4 $R/tc/p player              # 0 errors
python3 -I $R/unity_typecheck.py $R/csonly-09ade4c4 $R/tc/e editor              # 0 errors
```
