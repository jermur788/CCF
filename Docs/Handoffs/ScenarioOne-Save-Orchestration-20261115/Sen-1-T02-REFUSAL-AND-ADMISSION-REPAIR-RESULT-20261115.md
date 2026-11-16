# Sen-1 T-02R: refusal and admission repair (result)

**Claim [#563](https://github.com/jermur788/CCF/issues/563), orchestrator [#544](https://github.com/jermur788/CCF/issues/544), original task [#551](https://github.com/jermur788/CCF/issues/551).** Offline source repair only. This is not Unity verification, integration, native serialization or ISO qualification.

| Record | Value |
| --- | --- |
| Writer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness inside T3 Code, invoked as the implementation sub-agent under #544. **Effort: not observable from inside the session.** |
| Why this writer (fallback) | The packet's preferred writer was OpenAI GPT-6.1 Sol. **Both OpenAI pools had failed:** pool A was quota-refused until 18 Nov, and pool B hit its usage limit mid-task and made no edits (the lane was verified clean afterwards). The orchestrator's fallback rule and the Sen-1 amendment authorised Claude Opus 5.5 as fallback writer. |
| Independence note | The #557 reviewer whose findings this repairs was also Claude Opus 5.5, in a separate session with no shared history. This writer has read that review. **The packet's delta review should therefore come from a non-Anthropic model** if independence from both the reviewer and the writer is wanted. |
| Branch / worktree | `feature/scen1-shared-three-profile-save-v2` at `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` |
| Starting HEAD | `2353a792ee3f8f328ff7e1de3607a2c2333c8c87`, verified locally and on origin, with a clean worktree before editing |
| Repaired source commit | `5e21c944a319f7823469139e30e6663fe6b7df6c` (the four C# files only). This Markdown file is in the next commit; its SHA is recorded in the #563 [HANDED-OFF] receipt. Delta for review: `git diff 2353a792..5e21c944`. |
| Authority | [Manager repair packet](https://github.com/jermur788/CCF/blob/docs/sen-1-t02-manager-verdict-repair-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-T02-MANAGER-NEEDS-CHANGES-AND-REPAIR-PACKET-20261115.md); review `b1cdfeda`; signed contract `200add4e` (read with `git show`, not edited) |
| Durable evidence | `/media/jer/Files/CCF/Verification/SharedSaveT02R-563-20261116/` |

## Diff inventory (exactly four allowlisted C# files plus this Markdown file)

`git diff --numstat 2353a792`: controller +31/−7, validation +3/−1, Startup +7/−2, menu +18/−2. No new C# file, no `.meta`, no fifth file.

| File | Method(s) touched | Change |
| --- | --- | --- |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs` | `ContinueSavedForest` refusal branch, plus its comment | Restores the baseline `da962372` refusal: `continueChecked = true`, `continueAvailable = false`, `continueNote = problem ?? <existing fallback>`, then `StartupCard()`. Redraws only while the startup card is still open. |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` | `MainCard` Load callback; `SaveAndClose` | Load: a refusal sets the existing `sessionNote` and redraws `MainCard()`, with no close. Save: a pre-admission refusal keeps the current card (Confirm, or Main when no file exists), sets `sessionNote`, and adds the reason once to the card using the existing `Text` helper. |
| `Assets/ForestPrototype/ForestSaveController.cs` | `AdmittedSave.TryCreate`; `AdmittedSave.Apply` (new `out refusal` overload; the old signature delegates to it); `TryLoad`; `TrySave` (new `out refusal` overload; the old signature delegates to it) | Typed foreign-ID second check; Apply-time reason propagation; menu-visible Save reason. Reader, writer, archive, capture and the apply body are unchanged. |
| `Assets/ForestPrototype/ForestSaveValidation.cs` | `ValidateNewShape` only | Explicit `null` for any nested object in the new envelope is refused. Historical validators are unchanged; the generator check confirms "all 25 existing historical validator methods byte-identical". |

Source SHA-256 of the tested files (also in scratch, matching the worktree): controller `b569e8429c655e1a…`, validation `b21a46eb96609cc5…`, Startup `6231d968563c7d24…`, menu `db9a0d7f9a6ba6c8…`.

## Per-finding responses

### M-1 (MATERIAL): refusal is now visible on all three menu actions

- **Startup Continue:** `StandaloneSessionMenu.Startup.cs:89-90` (comment) and `:101-108`.
  - The refusal runs before admission, so the callback (dismiss intro, close help, `Close()`) has not run.
  - The branch marks the offer stale and calls `StartupCard()`. That draws Continue disabled (`:145`) with `continueNote` as the visible label (`:146-147`).
  - It moves focus to *Start New Scenario* (`:153`), so focus lands on an enabled button.
  - `continueChecked = true` stops `RefreshContinueState` from re-reading the file and overwriting the reason.
  - The `!IsOpen || !startupScreen` guard (`:102`) means a refusal can never redraw a menu that has already closed.
- **F10 menu Load:** `StandaloneSessionMenu.cs:123-129`.
  - The reason is no longer discarded with `out _`.
  - On refusal the menu stays open, the reason goes into the existing `sessionNote`, and `MainCard()` redraws. `MainCard` already renders the note (`:119`), the same surface `ResetTeachingProgress` uses.
  - No new panel, copy, button, keybinding or navigation.
- **Menu Save:** `StandaloneSessionMenu.cs:151-161` and `ForestSaveController.cs:165-197`.
  - `TrySave(onAdmitted, out refusal)` returns the same text it shows on the HUD.
  - `SaveAndClose` keeps the current card on a pre-admission refusal, sets `sessionNote`, and adds the reason with the existing `Text()` helper. A duplicate check means repeated Confirm presses show the reason only once.
  - *Cancel* returns to the main card, which then shows the same note.
  - A write failure after admission still closes the menu, as before, and shows "Save failed…" on the HUD, which `Close()` re-enables. The `!IsOpen` guard keeps that path from drawing onto a closed card.
  - The successful save flow is unchanged.

### m-1 (MINOR): typed foreign-ID second check, before any mutation

`ForestSaveController.cs:63-72`, inside `AdmittedSave.TryCreate`. It runs after `FromJson` and after the six unchanged raw validators, and before `new AdmittedSave(...)`. No world, scene, PlayerPrefs or file state is touched in `TryCreate`.

It refuses only when **all** of these hold:

- the selected profile is Historical;
- the parsed `version >= 10`;
- `scenarioOne != null`;
- `scenarioId` is non-empty;
- `scenarioId` differs ordinally from the chapter ID.

The refusal message is the same as the raw preflight's ("the save explicitly belongs to a different scenario").

What is unchanged:

- Absent, null and empty IDs and v1–v9 saves are untouched.
- The raw preflight is untouched.

One interaction to note: a file with **no** `version` key parses to the DTO default (24). So an absent-version file with an explicit foreign ID is refused here. The raw preflight already refuses the same file whenever it can parse it. **No claim of native parser equivalence is made.**

### m-2 (MINOR, latent): nested-object null guard in the new envelope

`ForestSaveValidation.cs:185-187`. `ValidateNewShape` used to return success for a `null` token of any reference type. It now refuses `null` wherever a nested object is expected.

What is unchanged:

- Strings, booleans, numbers and lists are handled before this line, so the "non-null string" and "null array record" rules are as they were.
- Scenario Two's raw `null` for `ccfCompactTeaching` is checked by its caller (`ValidateEnvelopeStructure`), never through `ValidateNewShape`, so O2 explicit-null is kept. Test (e) asserts this.
- The historical path never calls `ValidateNewShape`.
- Compact and O2 stay unavailable, because `ForestSaveProfiles` is not touched.

**Risk flagged for T-03/T-05:** in the game, `ScenarioOneManager.CaptureSaveData` sets `centuryReview = null` before the century review. Unity's serializer is expected to write a null `[Serializable]` class field as a default object, not `null`. This is UNVERIFIED offline. If native `JsonUtility.ToJson` ever writes `null` there, a future new-profile writer would produce envelopes that this rule refuses. The T-05 native check must cover this case.

### m-4 (MINOR): the specific apply-time reason is kept

- `ForestSaveController.cs:104-115`: the new `Apply(..., out string refusal)` returns the exact "Save not loaded: <reason>." that it already showed.
- `:404-411`: `TryLoad` passes that reason through, and falls back to "Save not loaded." only if no reason was set.
- `Load()` therefore re-displays the specific reason on the HUD instead of overwriting it.
- No second file read is added: `TryLoad` still reads once via `TryPrepareDiskLoad → TryReadSave`. No mutation happens before admission.

### m-3, N-1, N-2, N-3

These are not in this allowlist and were not touched. `ScenarioOneInteractionVerification.cs` still needs the Manager-commissioned migration before T-05.

## Tests (offline .NET harness only)

The harness is the T-02 resume-3 harness (`/media/jer/Files/CCF/Verification/SharedSaveT02-551-20261115/resume-3`), rebuilt in session scratch. The source-derived generator, the extracted boundary controller and the extracted menu methods are unchanged. Sources are copied from the worktree or extracted with `git archive`. The persistent-data sandbox is redirected to scratch. No real save or PlayerPrefs was touched. The fixture manifest reads archived save copies and Reference bytes read-only, and hash-checks them before and after.

| Run | Source | Harness | Result |
| --- | --- | --- | --- |
| Baseline reproduction | `2353a792` | original resume-3 | **352 PASS / 0 FAIL** |
| Candidate (final re-run from the worktree bytes) | repaired worktree | repaired harness | **460 PASS / 0 FAIL** |
| Control: old source against the repaired harness | `2353a792` | repaired harness | **FAILS**: it compiles, then aborts at its first failing check (m-2), after 108 PASS. The harness stops at the first failure, so per-repair failures are shown individually by the N-mutants below. |
| Control: repaired controller and validation with the two **old 2353a792 menu files** | hybrid | repaired harness | **FAILS** at "refused Continue marks Continue unavailable with the controller reason". This shows directly that the corrected definitions fail on silent refusal. |
| Candidate against the original harness | repaired worktree | original resume-3 | Does not compile: the old menu stub has no card children or `Label`. This is why the stub had to change. |

**Assertion accounting:** of the 352 baseline PASS lines, every assertion name is still present and passing **except the four regression-encoding definitions the review named** (`Tests.cs:60, 63, 69, 81`):

- "refused Continue retains menu/startup/help active state" (×4);
- "refused menu Load retains active UI" (×4);
- "refused menu Save retains active UI" (×1);
- "stale Continue does not close help/menu" (×1).

These asserted that a silent refusal passes. They were **replaced, not deleted**, by stricter definitions that fail on silent refusal: no teardown, Continue disabled, the controller's exact reason visible on the card, focus on an enabled button. The same Denial checks (world, scene, PlayerPrefs and file snapshots unchanged) still wrap every action.

Changed test definitions are kept in external scratch, as the packet requires:

- `changed-Tests.cs.diff`;
- `changed-MenuShell.source.diff`: the card now records drawn labels and buttons, and `StartupCard` is modelled from `Startup.cs:133-154`; it is not the real UI;
- `changed-Stubs.cs.diff`: a finder hook, and a clearly labelled SIMULATED parser-differential marker.

**One fixture correction:** the synthetic Compact envelope helper now sets `centuryReview` to a default object. It mirrors how Unity serializes a null `[Serializable]` field, which is UNVERIFIED natively. Without it, the Newtonsoft stub emits `null`, and the new m-2 rule correctly refuses that. The null variant is now an explicit negative instead.

### New required negatives (all PASS on the candidate)

- **(a) Stale Continue offer.** For each of {Compact envelope, O2 envelope, explicit foreign historical, corrupt `{`}:
  1. A valid offer is drawn with Continue enabled and focused.
  2. The file is replaced.
  3. Continue is clicked.

  Expected: world, scene, PlayerPrefs and file unchanged; menu, startup card and help still open; `continueAvailable = false`; `continueNote` equals the controller's own reason; the card is redrawn with Continue disabled and the reason visible; focus is on *Start New Scenario*.
- **(b) In-session Load refusal.** For the same four files: no close, no apply (Denial snapshot), `sessionNote` equals the controller reason, and the reason is visible on the redrawn main card.
- **(c) Menu Save pre-admission refusal** (foreign capture):
  - Confirm stays open with the reason shown exactly once, and no file is written.
  - A second Confirm press still shows it once.
  - Cancel shows the reason as the main-card note.
  - A fresh Confirm card shows it again.
  - The no-existing-file direct Save stays open, shows the reason, and creates no file.
- **(d) Typed foreign check.**
  - *ACTUAL offline evidence:* with a Newtonsoft-invalid token placed before `scenarioOne`, the raw preflight fails open (returns null). This reproduces the m-1 hypothesis.
  - Without a tolerant parser, those same bytes are refused as "Save file unreadable".
  - *SIMULATED (harness marker, not native evidence):* the stub parser is made to tolerate the token. Explicit foreign v10, v12 and v18 are then refused by the typed check with the exact message, on `TryCreate`, `CanLoad`, `TryLoad` (callback never runs, no mutation) and startup Continue (visible reason).
  - Controls still admitted under the same simulation: matching ID, empty, null, absent, v9 foreign and v1 foreign.
  - v24 is already refused by the existing full raw geometry parse; the differential window is v10–v19.
  - Without any simulation: plain foreign v12 is refused, and empty, null, absent and matching IDs are admitted.
- **(e) Nested null in the new envelope.**
  - Every nested-object position below the three roots is enumerated for Compact and O2, including `ccfWorldPayload.scenarioOne` and `.centuryReview`. Setting each one to `null` is refused.
  - O2 explicit-null teaching is still accepted.
  - A null string is still refused.
  - Historical nested null is still admitted by the historical preflight.
  - Compact and O2 are still unavailable.
- **(f) Apply-time change keeps the specific reason.** A finder hook changes the scene between the read-time and apply-time admission checks:
  - Definition change: `TryLoad` returns "Save not loaded: the active scenario does not match the selected save profile." The callback never runs and no mutation happens.
  - F9 `Load()`: the HUD message is that same text.
  - Context swap: "Save not loaded: the admitted save no longer belongs to the selected context."

  The old code returned "Save not loaded." in all three cases.

### Mutation sensitivity

Each mutant applies one source edit to a copy of the repaired worktree. The full generator and harness are re-run on it. A mutant counts as **KILLED** if the build or any assertion fails. Results are in `runs/mutants.log`. M1–M12 are the #557 review's mutants, with M3, M4 and M10 re-anchored to the new menu code. N1–N16 revert or weaken each repair.

**Result: 27 of 28 killed; 1 survivor (M11), which is equivalent.** Admit-before-close (M3, M4, M9, M10) and same-bytes / capture binding (M5, M9) are still killed. Every M-1, m-1, m-2 and m-4 repair is killed when reverted: N1, N2, N5 and N7 are exactly the old silent behaviours, and N11 and N14–N16 revert the minor fixes.

| Mutant | Outcome | First failing assertion |
| --- | --- | --- |
| `M1_no_raw_foreign` | **KILLED** | explicit foreign before version ordering foreign |
| `M2_no_envelope_marker` | **KILLED** | six canonical cross-profile placement o1-historical <- Compact SYNTHETIC |
| `M3_menu_load_close_first` | **KILLED** | refused menu Load keeps the menu open (no close, no apply) |
| `M4_startup_close_first` | **KILLED** | refused Continue keeps menu/startup/help open (no teardown) |
| `M5_no_capture_binding` | **KILLED** | capture modification cannot obtain trust |
| `M6_impure_capture` | **KILLED** | manager capture does not initialize competition/understorey |
| `M7_dup_replace` | **KILLED** | recursive duplicate key ccfSaveEnvelope |
| `M8_raw_empty_is_foreign` | **KILLED** | SYNTHETIC ambiguous/version historical preflight unchanged {"scenarioOne":{"scenarioId":""}} |
| `M9_admit_after_close_in_apply` | **KILLED** | apply consumes original admitted wood, never replacement file |
| `M10_menu_save_close_first` | **KILLED** | refused menu Save keeps the menu open on its Confirm card (no close) |
| `M11_null_teaching_ok_compact` | **SURVIVED** | harness 460/0 — equivalent: a null Compact teaching block still hits the later `(JObject)` cast, whose InvalidCastException is caught and refused (same survivor and dispo |
| `M12_no_payload_version_check` | **KILLED** | raw nested version 0 |
| `N1_startup_refusal_silent` | **KILLED** | refused Continue marks Continue unavailable with the controller reason: Save refused: a new-profile envelope cannot be loaded into the historical slot |
| `N2_startup_no_redraw` | **KILLED** | refused Continue redraws card: Continue disabled, reason visible, focus on Start New Scenario |
| `N3_startup_note_generic` | **KILLED** | refused Continue marks Continue unavailable with the controller reason: Save refused: a new-profile envelope cannot be loaded into the historical slot |
| `N4_startup_closes_help_on_refusal` | **KILLED** | refused Continue keeps menu/startup/help open (no teardown) |
| `N5_menu_load_silent` | **KILLED** | refused menu Load shows the reason on the redrawn main card: Save refused: a new-profile envelope cannot be loaded into the historical slot |
| `N6_menu_load_close_on_refusal` | **KILLED** | refused menu Load keeps the menu open (no close, no apply) |
| `N7_menu_save_silent` | **KILLED** | refused menu Save shows the reason on the Confirm card |
| `N8_menu_save_close_on_refusal` | **KILLED** | refused menu Save keeps the menu open on its Confirm card (no close) |
| `N9_menu_save_no_dedupe` | **KILLED** | repeated refused Confirm shows the reason once |
| `N10_trysave_reason_dropped` | **KILLED** | refused menu Save shows the reason on the Confirm card |
| `N11_no_typed_foreign` | **KILLED** | SIMULATED differential: typed check refuses explicit foreign v10: |
| `N12_typed_empty_is_foreign` | **KILLED** | SIMULATED differential control still admitted: empty the save explicitly belongs to a different scenario |
| `N13_typed_ignores_version` | **KILLED** | SIMULATED differential control still admitted: v9 foreign the save explicitly belongs to a different scenario |
| `N14_nested_null_accepted` | **KILLED** | o1-compact-v1 explicit-null nested object refused ccfWorldPayload.scenarioOne.centuryReview |
| `N15_generic_apply_reason` | **KILLED** | apply-time definition change keeps the specific reason: Save not loaded. |
| `N16_apply_refusal_not_returned` | **KILLED** | apply-time definition change keeps the specific reason: Save not loaded. |

### Additional offline typecheck against real Unity reference assemblies (still NOT Unity compilation)

This was a Roslyn build (`dotnet build`, C# 9, netstandard2.1) of all 191 non-Editor `Assets/**/*.cs` files. It referenced, read-only:

- the installed Unity 6000.6.0f1 `Managed/UnityEngine/*.dll` and NetStandard 2.1 references;
- package DLLs from `/media/jer/ZX20/CCF-U3/Library/ScriptAssemblies`, read only and not modified;
- the Unity `com.unity.nuget.newtonsoft-json` DLL.

| Source | Player defines | `UNITY_EDITOR` (+ UnityEditor DLLs) |
| --- | --- | --- |
| `2353a792` (control) | 0 errors | 0 errors |
| Repaired | **0 errors** | **0 errors** |
| Negative control (`label.textValue`) | 1 error, CS1061 as expected | — |

This is evidence that the new `card.Children()`, `Label` and `label.text` uses and the new overloads type-check against the real API surface. It is **not** a Unity compile: there is no Bee or ILPP, the compiler version differs, the asmdef layout differs, and the package DLLs come from another lane's Library.

### Hygiene

- `git diff --check`: clean.
- Changed paths: exactly the four C# files above, plus this file.
- No `.meta` and no other path.
- The signed spec blobs are not on this branch and were not touched.
- `active-tasks.md` and the CCF-Ops board were not touched.

## Not verified

- **Any Unity compilation, import, Editor or Player run.** Native UI Toolkit rendering, layout and focus on the redrawn cards are also unverified. One known gap: after an F10 Load refusal, `MainCard()` clears the card, so keyboard focus is dropped. That matches the existing `ResetTeachingProgress` redraw, and no focus behaviour was added because the grant forbids navigation changes. A Save-refusal reason appended to the main card (only in the no-existing-file case) appears below Quit, and its on-screen visibility at small sizes is unverified.
- **Native `JsonUtility` behaviour:** a real Newtonsoft/JsonUtility parse differential (m-1 is tested only by simulation), and null serialization of `[Serializable]` class fields (the m-2 risk above).
- **Native same-state serialization.**
- **Real save, PlayerPrefs or recovery behaviour.**
- **Genuine historical fixtures** beyond the 15-row read-only manifest the harness already uses.
- **All 63 ISO checklist items**, T-03, T-04 and T-05.
- **The m-3 Editor verification migration.**

## Next gate

One focused **non-author, preferably non-Anthropic** delta review of `git diff 2353a792..5e21c944`, limited to the four files. After that, the Manager gives a second verdict. #563 and #551 stay OPEN.
