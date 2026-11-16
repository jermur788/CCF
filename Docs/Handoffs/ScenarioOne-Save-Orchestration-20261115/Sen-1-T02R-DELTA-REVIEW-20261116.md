# Sen-1 T-02R — independent delta review (#569)

**Claim [#569](https://github.com/jermur788/CCF/issues/569) · orchestrator [#544](https://github.com/jermur788/CCF/issues/544) · repair claim [#563](https://github.com/jermur788/CCF/issues/563) · original task [#551](https://github.com/jermur788/CCF/issues/551).** Offline source review only. Not Unity compilation, runtime, native serialization, integration or ISO qualification.

| Record | Value |
| --- | --- |
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code harness inside T3 Code. Effort setting: not observable from inside the session. |
| **Independence (stated, not glossed)** | **Same provider as the author. NOT cross-provider independent.** The repair author was Claude Opus 5.5; the #557 review whose findings were repaired was also Claude Opus 5.5. I am a different model tier in a fresh session with no shared history, and I did not use the author's report as evidence (see "Method"). I do not claim cross-provider independence. |
| Target | Branch `feature/scen1-shared-three-profile-save-v2`, head `460fd51cb8972dd5b7819632c94e64d1070a98b7`, source commit `5e21c944`, reviewed baseline `2353a792ee3f8f328ff7e1de3607a2c2333c8c87` |
| Delta | `git diff 2353a792..460fd51c` = exactly 4 C# files + 1 Markdown (+291/−12) |
| Authority read | Signed contract `200add4e` (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, via `git show`); Manager repair packet and the #557 review (via `git show` from their branches). No file under `/media/jer/Files/CCF-Primary` was read for project docs. |
| This file | New branch `sen-1/t02r-delta-review-20261116-v1`, created from `460fd51c`. Nothing was pushed to the feature branch. |

## Verdict

**APPROVED for the repaired delta, as offline source evidence only.**

Findings: **BLOCKER 0 · MATERIAL 0 · MINOR 2 · NIT 4.**

### P0 predicate

**The P0 predicate is MET on this exact candidate (`460fd51c`): zero blockers and zero material findings.** I looked for a material finding on every item in the brief and did not find one. I am not stretching to satisfy the predicate. The two MINOR findings are real and are listed below, but neither changes the predicate. One residual (centuryReview, T-05) is explicitly not closed by this review.

The predicate covers this offline source candidate only. It says nothing about Unity behaviour, which no one has run.

## Method (what I did, and what I did not trust)

1. Read the committed C# delta line by line (`git diff 2353a792..460fd51c -- Assets`), and the surrounding unchanged code that the delta depends on.
2. Rebuilt the offline harness myself. Source: `git archive 460fd51c` (all `*.cs` under `Assets` and `Tools/Verification`) into scratch. Harness: a copy of the author's durable template `/media/jer/Files/CCF/Verification/SharedSaveT02R-563-20261116/harness-template/`. The harness is **not in Git** (see MINOR-2). I read its test diff and its stubs rather than trusting the summary.
3. Re-ran the author's 28-mutant script against my own copy and compared outcomes line by line with the author's `runs/mutants.log`.
4. Wrote and ran my own extra probes (`ReviewerProbes`, +97 assertions) in a separate harness copy. These were not in the author's harness.
5. Ran the author's Roslyn typecheck against the installed Unity 6000.6 reference DLLs (read-only).
6. Inspected committed Unity-written save files for how `centuryReview` is serialized.

The source worktree was not modified (`git status` clean, `HEAD` unchanged). No Unity, Editor, real save or PlayerPrefs access. One disclosure: the harness's fixture manifest reads three archived host-save copies (`/media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112/...`) read-only and hash-checks them before and after. I did not open them myself, but my harness runs did read them through the author's manifest. These are archive copies, not the live save.

## Results

| Check | Result |
| --- | --- |
| Harness rebuilt from committed source bytes, candidate | **460 PASS / 0 FAIL** (matches the claim) |
| Candidate + my extra probes | **557 PASS / 0 FAIL** (460 + 97 `REVIEWER` assertions; 104 version×ID combinations) |
| Old source `2353a792` vs the repaired harness | **FAILS** after 108 PASS, at `o1-compact-v1 explicit-null nested object refused ccfWorldPayload.scenarioOne.centuryReview` |
| Repaired controller/validation + the two **old** menu files vs the repaired harness | **FAILS** after 192 PASS, at `refused Continue marks Continue unavailable with the controller reason` |
| Mutants (28) | **27 KILLED / 1 SURVIVED (M11).** Every kill is an assertion failure (`System.Exception: FAIL …`), not a build error. Per-mutant outcomes are **identical** to the author's `runs/mutants.log`. |
| Roslyn typecheck vs real Unity 6000.6 refs (192 files) | **0 errors** under player defines (125 refs) and under `UNITY_EDITOR` (229 refs) |
| `git diff --check 2353a792..460fd51c` | clean |
| Allowlist | exactly `ForestSaveController.cs`, `ForestSaveValidation.cs`, `UI/StandaloneSessionMenu.cs`, `UI/StandaloneSessionMenu.Startup.cs`, plus the one Markdown. Nothing under `ForestSaveData.cs`, `ForestSaveProfiles.cs`, `ScenarioOne/` (archive, Reference resources), `ScenarioOneInteractionVerification.cs`, `Docs/Project/coordination/`, any `.meta`. |
| Compact / O2 | still unavailable: `ForestSaveProfiles.cs` untouched, and `ValidateSelectedJson` still ends in an unconditional refusal for new profiles |

Harness file hashes (SHA-256, first 16 hex) as I used them, so the Manager can pin them: `Tests.cs d4cecad3906c1198`, `Stubs.cs fb3b82d6c9c4e805`, `MenuShell.source 262d85d94ae30a93`, `BoundaryController.cs 812da73a89ae9387`, `generator/Program.cs c29f8e18d47bd3ca`, `mutants.py 7a70725eb6b63495`, `run_harness.sh daca5dbdc11b8a0d`. Committed source hashes match the author's record: controller `b569e8429c655e1a`, validation `b21a46eb96609cc5`, menu `db9a0d7f9a6ba6c8`, Startup `6231d968563c7d24`.

### Mutant M11 (the claimed harmless survivor) — verified, not accepted

M11 replaces `if (root["ccfCompactTeaching"].Type != JTokenType.Object)` with `if (false)`. I checked the claim two ways:

- **By reading:** with the guard gone, a `null` Compact teaching block reaches `ValidateNewShape(null, typeof(ForestCompactTeachingEvidence), …)`. The new m-2 line returns `"… requires an object, not null"`, so the file is refused anyway. (On the old code the later `(JObject)` cast threw `InvalidCastException`, which is caught and also refused.)
- **By running:** my probe `R1` shows the candidate refuses Compact null teaching with `"Compact requires an object teaching block"`.

So M11 is equivalent in admit/refuse outcome. It differs only in the refusal wording. The claim holds. Note this means m-2 made the old guard redundant for the null case; that is harmless duplication (NIT-3).

## 1. M-1 — visible refusals (verified in code and tests)

All three routes now refuse visibly, with no teardown and no apply. Verified in committed code, not only in the harness:

| Route | Code | Observed behaviour |
| --- | --- | --- |
| Startup **Continue** | `StandaloneSessionMenu.Startup.cs:100-109` | A refusal happens before admission, so the callback (dismiss intro, close help, `Close()`) has not run. The branch sets `continueChecked = true`, `continueAvailable = false`, `continueNote = problem ?? "Saving is not available in this scene."`, then `StartupCard()`. `StartupCard` draws Continue disabled (`:145`), shows the note (`:146-147`) and focuses *Start New Scenario* (`:153`, an enabled button). `RefreshContinueState` returns early on `continueChecked`, so the reason is not overwritten. The `!IsOpen || !startupScreen` guard (`:102`) stops a redraw onto an already-closed menu. |
| F10 **Load** | `StandaloneSessionMenu.cs:124-129` | `TryLoad(Close, out problem)`; on refusal with the menu still open, `sessionNote = problem; MainCard();`. `MainCard` already renders `sessionNote` (`:119`), the same surface `ResetTeachingProgress` uses. `Close()` clears `sessionNote` (`:97`), so a stale note cannot leak into the next open. |
| Menu **Save** | `StandaloneSessionMenu.cs:154-161`, `ForestSaveController.cs:165-197` | `TrySave(onAdmitted, out refusal)` returns the exact string it shows on the HUD. A pre-admission refusal returns **before** `onAdmitted` and before any file I/O. The Confirm card stays, `sessionNote` is set, and the reason is appended once (dedupe by label text; `UiKit.Add` returns a `Label`, so the `label.text` test is valid against the real API — typecheck passes). |

**Wording (no invention).** I extracted every string literal in the delta. Apart from one, all already existed at `2353a792` or in the baseline `da962372`: `"Saving is not available in this scene."`, `"Save refused: "`, `"Save not loaded: "`, `"Save not loaded."`, the "Save failed: the file could not be written…" text and `"the save explicitly belongs to a different scenario"`. `continueNote = problem ?? "…"` is byte-for-byte the baseline `da962372` and `origin/main` line. The only new wording is `" requires an object, not null"`, a validator path message for the new-format envelope, the same family as the existing `" requires an object"`. No new button, key, card layout or panel.

**Test integrity (the four silent-success assertions).** The old `Tests.cs:60/63/69/81` compared a UI sentinel and so passed on silence. They were replaced, not deleted, by `StartupRefusalVisible` and the Load/Save checks. The new checks require: no teardown (menu, startup and help still open), `continueAvailable == false`, `continueNote` equal to the controller's own reason, Continue disabled, reason present in the drawn labels, focus on an enabled button, and no close on Load/Save. They **fail on silence**, shown two independent ways: the hybrid control (old menu files) fails at the Continue assertion, and mutants N1–N10 (each reinstates one silent behaviour) are all killed by assertion failure. The old `Sentinel()` helper is now unused (NIT-1).

The new checks are stricter than the old sentinel. The old one compared only `IsOpen|startupScreen|help|continue state`. The new one keeps the first four and adds the visible-reason and focus checks, so nothing was hollowed.

**Model fidelity caveat.** The menu methods under test are extracted from the real source by the generator, so `MainCard`, `SaveAndClose` and `ContinueSavedForest` are the real bytes. `StartupCard` and the card/label/button widgets are hand-written stubs (`MenuShell.source`). So "focus on Start New Scenario" and "Continue disabled" are asserted against the author's model of `StartupCard`. I checked that model against the real `StartupCard` by reading `Startup.cs:133-154`, and it matches. It is still a model.

## 2. m-1 — typed foreign-ID second check

`ForestSaveController.cs:63-72`, inside `AdmittedSave.TryCreate`, after the six raw validators and before `new AdmittedSave`. No mutation state is reachable from `TryCreate`.

It refuses only when **all** of these hold: profile is Historical, `data.version >= 10`, `data.scenarioOne != null`, `scenarioId` is nonempty, and it differs ordinally from the chapter ID.

**Over-reach hunt (I tried to reject a legitimate old save and could not).**

- By construction, the raw preflight (`ForestSaveValidation.cs:50-55`) already refuses *any* string at path `scenarioOne.scenarioId` that is nonempty and differs from the chapter ID, **without a version condition**. The typed check is a strict subset of that rule (it adds `version >= 10`). So on any file the raw reader parses successfully, the typed check can never change an outcome.
- It differs only where the raw reader throws `JsonException` and fails open. That is exactly the differential window the Manager asked to close.
- My sweep (`R3`/`R4`, 13 versions {0, −3, 1, 5, 9, 10, 12, 18, 19, 20, 22, 23, 24} × 8 IDs {matching, empty, null, absent, `Scenario-One`, ` scenario-one`, `scenario-two`, `foreign`}): every typed refusal is also a raw refusal, and matching, empty, null and absent IDs are never typed-refused at any version. 104 combinations, all pass.
- The author's 15-row genuine fixture manifest (v8, v9, v12 frozen Reference ×4, v19 ×4, v20 ×3, v21 ×2, v22) shows `rawCandidate`, `typedCandidate` and baseline all `None` (admitted, unchanged), with source hashes unchanged before and after.
- A file with no `version` key parses to the DTO default 24, so an absent-version file with an explicit foreign ID is refused. The raw preflight refuses the same file whenever it can parse it, so there is no new loss. The author disclosed this.
- `TryCreate` also runs on the Save path (capture JSON). A legitimate capture has `scenarioId == ChapterId` (`CanOperateOnSelectedWorld` already requires that), and `scenarioOne == null` skips the check.

**Is the parser-differential case evidence or conjecture?** **Still conjecture, half proven.** What is *actual* evidence: the raw preflight fails open on a Newtonsoft parse error (`catch (JsonException) { }`, `Validation.cs:59`), reproduced offline by the `ACTUAL Newtonsoft` assertion. What is *not* evidence: that Unity's `JsonUtility` accepts any input Newtonsoft rejects. The harness gets there with a stub parser that tolerates an invented `@` marker, and the author labels those assertions `SIMULATED`. I agree with that labelling. The typed check is therefore defence-in-depth against an unproven but plausible differential. It cannot fire on any input the raw reader can parse, and it needs real Unity to demonstrate that a trigger exists. That is T-05.

## 3. m-2 — nested null in the new envelope

`ForestSaveValidation.cs:185-187`: `if (token.Type == JTokenType.Null) return path + " requires an object, not null";` replaces `… && !type.IsValueType) return null;`.

- **Reach.** `ValidateNewShape` is called only from `ValidateEnvelopeStructure` (`:82, :93, :118`). I checked all callers (`git grep`). The historical path never calls it. Historical nested nulls stay admitted (assertion `SYNTHETIC historical nested null not tightened`, two shapes).
- **Strings, booleans, numbers, enums, lists** return before this line, so "null string refused" and "null array record refused" are as before.
- **Scenario Two teaching-null** is checked by the caller (`:100-102`, anything but `Null` ⇒ refuse) and never passes through `ValidateNewShape`. Assertion `O2 explicit-null teaching still accepted after nested-null guard` passes, and `O2 explicit-null rejects {object,"null",false,0,[]}` still pass.
- **Nested positions.** The harness enumerates every nested object position in the Compact and O2 samples and nulls each one; all are refused. N14 (revert the guard) is killed at `…scenarioOne.centuryReview`.
- **Compact / O2** remain unavailable because the function is only reachable for profiles that `ValidateSelectedJson` refuses unconditionally.
- **Spec consistency.** The signed spec has no sentence that literally says "nested object may not be null". The requirement comes from the Manager packet (m-2) plus `requirements.md` ISO-40: *"Missing/null/blank/case-variant/duplicate/unknown/mismatch input refuses for new profiles before mutation; arbitrary DTO defaults cannot grant presence"*, and the spec's new-profile "Reject; never normalize" column. A null nested object that `JsonUtility` would quietly turn into a default record is the case ISO-40 names. The rule is consistent with the signed contract and with the packet; it is not an invention.

## 4. m-4 — apply-time reason reaches the player

`Apply(…, out refusal)` (`Controller.cs:108-119`) builds `"Save not loaded: <reason>."`, shows it as before, and returns it. `TryLoad` (`:404-411`) passes it through and falls back to `"Save not loaded."` only when `Apply` set none (the apply-phase failure after teardown, unchanged). `Load()` (`:397-400`) then re-displays that same specific text instead of overwriting it. No second file read (`TryLoad` still reads once via `TryPrepareDiskLoad`), and no mutation before admission. The old one-argument `Apply` and `TrySave` signatures delegate to the new overloads, so existing callers compile (typecheck passes). Tests (f) and mutants N15/N16 cover definition change, F9 `Load()` HUD text and context swap; both mutants are killed.

## 5. THE NEW RISK — the `centuryReview` fixture assumption

**The assumption.** The harness's Newtonsoft stub writes `null` for an unset `[Serializable]` class field. Under the new m-2 rule the harness's own Compact/O2 sample would be refused. The author changed the sample to carry `centuryReview = new ScenarioCenturyReview()` (a default object) on the belief that Unity's `JsonUtility.ToJson` writes a default object, not `null`, for such a field. That belief was labelled **UNVERIFIED**.

**Why this is the most likely way the repair is wrong:** if real `JsonUtility` ever wrote `null`, every legitimately produced new-format save would be refused by the validator the repair just tightened, and the harness change would have hidden exactly that. I treated it as the highest-risk item.

**What I found.**

1. **In-repo evidence that the assumption is correct for the v19–v22 writers.** `ScenarioOneManager.CaptureSaveData` (`ScenarioOneManager.cs:1453-1454`) sets `centuryReview` to `null` until the year-100 review, so the capture really does pass `null` to `ToJson`. I then read eight Unity-written saves that contain `scenarioOne`:
   - tracked in this repo at `460fd51c`: `Docs/Verification/StormsWindthrowIntegration/Evidence/StormCoreVerification/resolved_storm.json`, `…/StormRngWrite/world-0.json`, `…/StormSalvageVerification/salvage_resolved.json` (all v19, labelled "tracked storm verification writer output; no user-save provenance");
   - four Verification-run outputs: `S1C-T05-Unity-20261029/fixtures/v20-y5.json`, `v21-y1.json`, `v21-y0-rack.json` and `S1C-T08-20261031/fixtures/recorded-v22-year3-save.json` (test-run config directories, not the live save).

   In **all** of them, `scenarioOne.centuryReview` is the default object `{"year":0,"outcome":0,"completedYear":-1,"referenceId":"aspirational-design-targets","referenceScheduleHash":"","referenceComparisons":[],"managementComparisons":[]}`, and the **entire JSON contains zero `null` tokens** (I walked every node). That is the strongest direct evidence available offline: `JsonUtility` wrote a default object for the exact field in question, from a capture that held `null`.
2. **Consistent with the production restore code.** `ScenarioOneManager.cs:1556` treats `data.centuryReview == null || data.centuryReview.year < ReviewYear` as "no review". That `year < ReviewYear` test exists precisely because the serializer hands back a default object (`year = 0`), never `null`.
3. **Consistent with the signed schema.** §4.2 defines `ccfWorldPayload` as the *existing version24 ForestSaveData shape*, and `plan.md:25` fixes the writer as same-state `JsonUtility.ToJson(data, true)` bytes. The signed contract therefore describes a payload that `JsonUtility` writes, which contains no JSON nulls. A null nested object is by definition not that shape, and ISO-40 says defaults cannot grant presence.
4. **Which positions are actually exposed.** The DTO graph has only two non-enum class-typed nested fields I could find, `scenarioOne` and `centuryReview`; the Vector/Quaternion fields are structs (never null, and a null struct was already refused). The m-2 rule can only matter to those two positions plus whatever lies below them, and the real outputs show both are objects.

**Judgement.** The assumption is **consistent with the signed schema and supported by eight real writer outputs**, so I do not believe the stricter rule rejects legitimately *produced* new-format saves, and I do not rate this a finding. The fixture change is a correct reflection of real serializer behaviour, not a way of hiding a failure. The null variant is kept as an explicit negative (`N14` is killed).

**Residual risk for T-05 — not closed by this review.** The evidence has these limits, and T-05 must close them natively, in Unity 6000.6:

- The eight files are **v19–v22**, written by earlier code. No real **v24** save, and no file written through the **new envelope writer** (which does not exist yet — T-04), has been observed. v23/v24 additions are unobserved.
- **Writer composition is the real exposure.** The rule is safe only if the future new-profile writer builds the payload with `JsonUtility.ToJson`. If T-04 serialises the DTO with Newtonsoft (`JsonConvert`) or re-emits it through a `JObject` round-trip that preserves nulls, every produced file would be refused by this validator. T-04/T-05 acceptance must pin the composition method.
- The result also depends on the Unity version and the serializer behaviour at the pinned editor; I verified neither Unity version identity of the earlier writers nor any behaviour change.

Required native T-05 checks: (a) write a real new-profile envelope from a world with `centuryReview == null` (any year before 100), then assert the file passes `ValidateEnvelopeStructure` with zero JSON `null` tokens outside the O2 teaching field; (b) the same after year 100 with a populated review; (c) a hand-edited `centuryReview: null` is refused (negative); (d) assert the writer path is `JsonUtility`-based and not Newtonsoft-based; (e) O2 teaching-null still accepted.

## Findings

### MINOR-1 — No-existing-file menu Save refusal puts the reason at the bottom of the card, and its visibility is unverified

`StandaloneSessionMenu.cs:154-161`. With no existing save file, `SaveAndClose` is called directly from `MainCard`, so there is no Confirm card. On refusal it sets `sessionNote` but does not redraw `MainCard`; it appends the reason with `Text(problem)`, so the reason lands **below "Quit CCF…" and the F1/F10 footer**, at the bottom of the card. The `sessionNote` slot near the top of the card (`:119`) is not updated until a redraw.

*Failure scenario:* a player with no saved forest in a small window or high UI scale presses "Save current forest and return…" while a refusal condition holds. The reason is rendered, but below the fold of the scroll view, so the click appears to do nothing. The author discloses this gap. The offline model has no layout, so I cannot confirm or deny it. The existing-file case (Confirm card) puts the reason after the Cancel button on a short card and is not affected in the same way.

*Why not MATERIAL:* the reason is rendered on an existing surface using the existing `Text` helper, nothing silent remains, the refusal still precedes any close or write, and the Manager packet asked only for "an existing-style reason in the existing menu note/card". Visibility at small sizes is a native-UI question. A simple fix, if Unity shows a problem, is `MainCard()` for the no-file case, as the Load route does.

### MINOR-2 — The test evidence is not reproducible from Git and has no pin for the harness bytes

The 460/0 and 27/28 results come from a harness that lives only in `/media/jer/Files/CCF/Verification/SharedSaveT02R-563-20261116/`. It is outside Git. The handoff records the SHA-256 of the *source* files but not of `Tests.cs`, `Stubs.cs`, `MenuShell.source` or `mutants.py`. Worse, `harness-template/tested-source-sha256.json` still holds the **old baseline** hashes (its controller entry `c4230929…` is the SHA of `2353a792`'s controller, not `460fd51c`'s), so the one machine-readable "tested source" record in that folder is stale and, if used, would mislead.

*Failure scenario:* a later reviewer, or the Manager, tries to reproduce "460/0" from the handoff alone; the harness can be silently edited between runs with no hash to detect it, and the stale JSON says the tested controller was the baseline one.

*Mitigation here:* I recorded the harness hashes above, and my rebuild matched the claimed counts exactly, so the claims are reproduced, not merely asserted. The Manager should either copy the harness into a pinned location or record those hashes.

### NIT-1 — Dead `Sentinel()` in the harness
`MenuShell.source:50` still defines `Sentinel()`; nothing calls it any more. Harmless; remove it so a future reader does not assume the old silent-success check still runs.

### NIT-2 — Handoff line citations drift by 1–2 lines
For example the handoff cites `Startup.cs:101-108` and `:102` for the guard; the actual refusal block is `:100-109`. Cosmetic.

### NIT-3 — The M11 guard is now redundant
`ValidateEnvelopeStructure`'s explicit `Type != Object` check for Compact teaching duplicates what m-2 now enforces through `ValidateNewShape`. The two refusals differ only in wording. Keep it (it gives the clearer message) but know that deleting it would not be caught by the harness.

### NIT-4 — A refusal message set while the menu is open is shown again after the menu closes
`Apply`/`TrySave` call `SetMessage` while `ForestSaveController` is disabled by the open menu. Its `messageTimer` only advances in `Update`, so closing the menu with F10 can show the same refusal text on the HUD again. Not a regression and arguably helpful; noting it so nobody mistakes it for a bug during Unity play.

## Not verified (and why)

- **Any Unity compile, import, Editor or Player run.** My Roslyn typecheck (0 errors, both define sets) is a type check against real Unity reference DLLs from `/media/jer/ZX20/Unity/6000.6.0f1` and package DLLs from another lane's `Library/ScriptAssemblies` (read-only). It is not a Unity build: no Bee/ILPP, a different compiler, a different asmdef layout.
- **Native UI Toolkit behaviour:** layout, scrolling, focus after redraw, and whether the F10 Load note and the no-file Save reason are actually visible. After an F10 Load refusal, `MainCard()` clears the card, so keyboard focus is dropped; this matches how `Open()` shows `MainCard` today (no `FocusWhenReady` there), so it is not a regression, but I did not see it run.
- **Native `JsonUtility`:** whether any input exists that Newtonsoft rejects and `JsonUtility` accepts (m-1 is conjecture), and null serialization for a **v24** save written through a new-envelope writer (the residual above).
- **Real save, PlayerPrefs, recovery and F5/F9 keyboard flows.** No ISO ticks. T-03, T-04, T-05 and the m-3 verification-caller migration (`ScenarioOneInteractionVerification.cs`, not in the allowlist) remain open.
- **Dashboard, board, `active-tasks.md`:** not touched, not read for state.

## Recommendation to the Manager

The delta does what the Manager packet required and I could not break it offline. It is **APPROVED as the repaired T-02R delta** and meets the **P0 predicate**, which is what authorises the Sen-1 amendment's conditional T-03A offline construction. Keep the following attached to that authorisation, not dropped:

1. **Same-provider review.** Treat this as same-provider independence, not cross-provider. If the owner requires a non-Anthropic check before the *next* gate (T-05 native qualification, or any release claim), schedule it then.
2. **T-05 gate:** carry the five `centuryReview` / writer-composition checks above into the T-04/T-05 acceptance list, and pin the writer to `JsonUtility`.
3. **Evidence pin (MINOR-2):** copy the harness into a pinned location or record the hashes above.
4. **MINOR-1:** have the first Unity run check the no-file Save refusal at a small window size.
