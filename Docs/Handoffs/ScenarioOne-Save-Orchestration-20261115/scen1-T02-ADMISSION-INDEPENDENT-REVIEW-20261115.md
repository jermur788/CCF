# T-02 raw admission and pure capture: independent source review

**Issue #557 (parent #544; reviewed task #551). T-02 source stage only. This is not integration, Unity verification or ISO qualification.**

| Record | Value |
| --- | --- |
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, fresh session with no author history. **Requested effort: medium. Observed effort: cannot be verified from inside the session.** Implementer was OpenAI GPT-6.1 Sol, so this review is cross-provider. |
| Reviewed head | `2353a792ee3f8f328ff7e1de3607a2c2333c8c87` (source commit `84a631d0`, followed by the result-doc commit) |
| Delta base | `da962372a7d45d8d049e9ad0dd2a657e5c339a2a` |
| Contract | `specs/scenario-independent-save-isolation-v1/{spec,plan,tasks,checklists/requirements}.md` at `200add4e6e5116268b79ef63131ff9b836f14c5d`, read with `git show`. The four files are absent from the reviewed branch, so they were not modified there. |
| Method | Read the full `git diff da962372..2353a792` and the surrounding callers. Reproduced the implementer's offline harness in scratch from the committed bytes. Ran 12 source mutants through that harness. No Unity, no Editor, no real save or PlayerPrefs access. The source worktree was not written. |

## Verdict

**NEEDS CHANGES** for the T-02 source stage. The narrow fixes are within the T-02 ceiling.

Findings: **BLOCKER 0 · MATERIAL 1 · MINOR 4 · NIT 3.**

The admission core is sound, and I found no route where a refused load mutates the world before refusing:

- the raw new-envelope check;
- the explicit-foreign historical preflight;
- binding admission to one read of the bytes;
- typed-route closure;
- pure capture.

The single MATERIAL finding is a player-visible regression in the menu repair. It goes beyond the "no visual or wording change" grant, and the result doc does not disclose it.

## Adversarial checks performed

### 1. Ordering: admission versus the first mutation on every route

| Route | Admission chain (head line numbers) | First mutation | Result |
| --- | --- | --- | --- |
| F9 / public `Load` | `Load:376 → TryLoad:381 → TryPrepareDiskLoad → CanOperateOnSelectedWorld → File.ReadAllText:416 (single read) → AdmittedSave.TryCreate → TryPrepare` | `ApplyAdmittedData` (ecology / approach), reached only from `Apply:107` | Admission comes first. ✔ |
| Startup Continue | `ContinueSavedForest → TryLoad(onAdmitted)`; `Apply` runs `TryPrepare` again, then `onAdmitted:104` (dismiss intro, close help, `Close()`, which restores `Time.timeScale` and re-enables components), then apply | Help/menu/timeScale changes only after admission | Admission comes first. ✔ Refusal feedback regressed (**M-1**). |
| F10 menu Load | `TryLoad(Close, out _)` (`StandaloneSessionMenu.cs:123`) | `Close` only after admission | Admission comes first. ✔ Refusal is silent (**M-1**). |
| Menu Save / F5 | `TrySave`: context and preview guard → pure capture → `TryCreate` + `TryPrepare` → `onAdmitted:165` → writer | Temp write only after admission | ✔ |
| Public `LoadData:537` | `CanOperateOnSelectedWorld` → `TryPrepareCapture` (`ConditionalWeakTable` provenance, same context and path, `source.Json == ToJson(data)`) → admission over the **captured string**, never over the caller's DTO | apply | Arbitrary, cloned or modified DTOs refuse. ✔ |
| Public `ScenarioOneManager.RestoreSaveData` (both overloads) | Unconditional refusal with a log; no state is touched | none | Bypass closed. ✔ |
| `RestoreAdmittedSaveData` | `TryGetManagementData` requires `applying && ReferenceEquals(manager, applyingManager) && MatchesCurrentContext()` | `TeachingWorldVersion++` sits after that check | ✔ |
| Reference preview entry | Historical-only gate → `TryPrepareReference` (frozen-hash check over original embedded raw text, then `TryCreate`/`TryPrepare`) → pure `CaptureData` → `CanRestoreCapture` → `Apply` → only then preview flags, pose and Work Plan | admitted apply | ✔ `ExtractReferenceWorld` is a faithful copy of `ScenarioReferenceArchive.Load`'s extractor, so its hash agrees by construction. |
| Preview return | Internal `RestoreCapturedData` (same-session provenance); the guard and snapshot are cleared only on success | admitted apply | ✔ A failed return keeps the preview guard and the snapshot, with no false "Returned". |
| Backup | No backup loader exists; backup is only named in the unreadable-file message | n/a | ✔ No auto-promotion. FR-06 recovery belongs to T-04. |

I also checked that the validators do not mutate the DTO. `FindLoadProblem`, `Validate*` and the raw validators contain no writes. This matters because the management subrestore now re-parses the admitted string instead of reusing `data.scenarioOne`. The two are equivalent only if nothing between parse and restore writes to the DTO, and nothing does.

### 2. Raw admission (new envelope)

`ValidateEnvelopeStructure` (`ForestSaveValidation.cs:63`) checks the original raw string with `JObject.Load` and `DuplicatePropertyNameHandling.Error`. That setting applies to nested objects, and mutant M7 confirmed the duplicate check is real. The validator also checks:

- trailing content after the root object;
- exactly three ordinal root keys, and exactly seven identity keys;
- `JTokenType.Integer` for every int/enum field, rejecting Float even when the value is `1.0`;
- a signed 32-bit range check, with BigInteger overflow caught;
- defined enum values;
- unknown keys at every level of the reflected field graph;
- a present raw `null` for Scenario Two teaching versus an object for Compact.

A typed DTO round trip cannot satisfy these checks, because every check reads the `JToken`. There is one hole: explicit `null` is accepted for any nested reference-type record (**m-2**). It is latent, because `ValidateSelectedJson:29` refuses every new profile unconditionally, and Compact and Scenario Two availability stay `false`.

### 3. Historical preservation

`ValidateHistoricalIdentityJson` streams the raw JSON. It rejects only in two cases:

- all three envelope root keys are present at depth 1;
- a **string** token at path `scenarioOne.scenarioId` is nonempty and differs ordinally from the chapter ID. Duplicate occurrences are each inspected, so a foreign value hidden behind a later duplicate is still caught.

Absent, `null` and `""` values are not rejected. Mutant M8, which treats empty as foreign, is killed by the harness's synthetic empty-ID control. The six original raw validators are unchanged and run in their original order. Version ≤0 is still normalized to 1 after the raw validators. The future-version warning is retained. I found no historical over-reach.

Under-reach exists only as a parser-differential fail-open (**m-1**).

### 4. Capture purity

- `CaptureSaveData` no longer calls `EnsureUnderstoreyGrid`.
- `CaptureData` now only adds a `ConditionalWeakTable` entry. That is private provenance, not world state.
- The `CaptureWorldData` body is unchanged apart from its rename.
- Startup initialization still happens via `Start → EnsureBaselineSnapshot → EnsureUnderstoreyGrid` (manager `:2518–2520`).
- Later resets initialize inside `InitializeNewScenario`, gated by `worldSetupComplete`.

The "refusal sentinel" (a capture that the save then refuses) is inert: `TrySave` refuses before any IO, and the capture itself changed nothing. Mutant M6, which puts the old initialization back, is killed.

### 5. Single boundary

Every world-applying path goes through `AdmittedSave.Apply → private ApplyAdmittedData`. The only `FromJson<ForestSaveData>` calls in production are inside `AdmittedSave`, which works on its own private readonly string, and inside the unchanged archive hash helpers. `RestoreAdmittedSaveData` cannot be driven without an in-flight `Apply` of the same manager. I found no public or typed path to mutation that skips the boundary.

### 6. Menu repair

- **Admit → close → apply:** correct. Mutants M3, M4, M9 and M10 are all killed.
- **Same-bytes binding:** real. `TryReadSave` performs exactly one `File.ReadAllText`. The `AdmittedSave` keeps that string private and readonly, and `TryPrepare` / `TryGetManagementData` re-parse that same string. Startup Continue does not reuse the `CanLoad` result. This is a genuine binding, not a comparison of two re-reads.
- **Grant compliance: exceeded on the refusal path** (**M-1**). Button labels, layout, navigation and keybindings are unchanged.

### 7. Evidence honesty

- I rebuilt the harness in scratch from the committed source, with the sandbox redirected to scratch. The result was **352 PASS / 0 FAIL**, and the five changed files hash-match `tested-source-sha256.json`. The claim reproduces.
- **Mutation testing:** 12 hand-written source mutants. **11 were killed.** The survivor, M11 (removing Compact's "teaching must be an object" check), is equivalent: a null teaching block still fails the later `(JObject)` cast, and that exception is caught and refused. The negatives are therefore not tautological for:
  - foreign-ID rejection;
  - the envelope marker;
  - duplicate keys;
  - the empty-ID admission control;
  - capture binding;
  - capture purity;
  - nested version;
  - admit-before-close on all three menu actions and in `Apply`.
- **Caveat:** the menu-refusal assertions (`Tests.cs:60,63,69,81`) compare against the *candidate's* chosen behaviour ("continue state unchanged"), not against baseline. They actively encode the M-1 regression as a pass.
- The result doc correctly says it has **not** done Unity compilation, runtime, native `JsonUtility`, or native-serialization equivalence. I am not upgrading that. Its route table (row "Startup Continue") and the allowlist row for `StandaloneSessionMenu.Startup.cs` describe refusal as "retains current card" and "no … new UI". That is literally true, but it omits that the baseline refusal message and the disabling of Continue were removed. This is an overstatement by omission (part of M-1).
- The doc honestly says that existing verification callers now fail closed and need an authorised migration.

### 8. Protection

- The delta is exactly 5 C# files plus 1 Markdown file (`git diff --name-status`).
- `ForestSaveData.cs`, `ScenarioReferenceArchive.cs`, the Reference resources, the registry, the session context, the envelope types and all `.meta` files are unchanged.
- No new files, metas or tools.
- `ForestSaveProfiles.Compact` / `ScenarioTwo` still have `isAvailable: false`.
- Signed blob IDs at `200add4e`: spec `8ac55102`, plan `4ba91d90`, tasks `88052dc5`, checklist `505ab550`. None of the four exists on the branch.
- Real save, PlayerPrefs and Reference bytes are **not verified by me** beyond the facts that the diff touches no resource and that I did not open them.

## Findings (most severe first)

### M-1 — MATERIAL — Menu load refusals became silent; this removes the baseline refusal message and exceeds the grant

**Where:**

- `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs:99-103`. The refusal branch now only calls `Debug.LogWarning`. Baseline `da962372` `Startup.cs:95-99` set `continueChecked = true`, `continueAvailable = false` and `continueNote = problem`, then called `StartupCard()`. That rendered the reason (`:141-142`) and disabled Continue.
- `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs:123`. `TryLoad(Close, out _)` discards the problem. `TryLoad` (`ForestSaveController.cs:381-383`) does not call `SetMessage` on a preparation failure, so nothing is shown. Baseline did `Close(); saves.Load();`, and `Load` showed "Save refused: …" / "Save not loaded: …" on the HUD.
- Related, lower impact: a refused menu Save (`SaveAndClose:145`) calls `SetMessage`. But the controller is disabled while the menu is open (`StandaloneSessionMenu.cs` `Open`: `saves.enabled = false`), so `OnGUI` is not drawn. The Confirm card stays up, and clicking Confirm appears to do nothing until the menu is closed.

**Failure scenario:**

1. The title screen opens and `CanLoad` passes.
2. The save is then replaced before the click: a second session, a copied file, or a foreign-ID or envelope file in the historical slot. Any reason that `TryLoad` refuses but the cached offer did not see will do.
3. The player clicks **Continue**. Nothing visible happens, Continue stays enabled, and every further click fails silently.

In the F10 menu, **Load your same-build save** likewise does nothing, with no reason given. Baseline showed the reason in both cases. Removing a displayed message changes visible copy and behaviour, which the two-file grant forbids. It also weakens truthful-failure reporting.

**Fix (inside the ceiling):**

- In the Continue refusal branch, restore the baseline state update and `StartupCard()`. This is safe because it happens before any teardown.
- In MainCard, show the returned problem, for example `sessionNote = problem; MainCard();`, or call `SetMessage` so it appears after Close.
- Update `Tests.cs:60/63` to assert the baseline refusal note instead of "state unchanged".

### m-1 — MINOR — The explicit-foreign preflight fails open on a Newtonsoft parse failure, and the late warning was deleted

**Where:**

- `Assets/ForestPrototype/ForestSaveValidation.cs:59`: `catch (JsonException) { }` → admit.
- `ScenarioOneManager.cs`: baseline `:1481-1482` foreign warning removed.

**Failure scenario:** take a v10–v18 historical file with no competition model. None of the raw validators does a full Newtonsoft parse for such a file (geometry needs v≥20, storm v≥19, competition needs model 2). Suppose it contains syntax that Unity `JsonUtility` tolerates but Newtonsoft rejects, placed before `scenarioOne`. The streaming reader throws before it reaches `scenarioId`. Admission continues, and `JsonUtility` restores `scenarioId: "scenario-two"` silently. Baseline at least warned.

I could not demonstrate a concrete differential input offline, because that needs native `JsonUtility` (verdict: PLAUSIBLE). The game's own writer never produces such files.

**Fix:** add a typed post-parse check, still before any mutation, in `AdmittedSave.TryCreate` after `FromJson`:

```
data.version >= 10 && !string.IsNullOrEmpty(data.scenarioOne?.scenarioId) && data.scenarioOne.scenarioId != profile.ChapterId
```

It rejects only explicit foreign values, so it adds no inference.

### m-2 — MINOR (latent) — The new-envelope shape validator accepts explicit `null` for nested record objects

**Where:** `ForestSaveValidation.cs:185`: `if (token.Type == JTokenType.Null && !type.IsValueType) return null;`

**Failure scenario:** a Compact envelope with `"ccfWorldPayload":{…,"scenarioOne":{…,"centuryReview":null}}`, or `null` for any other nested `[Serializable]` record, passes the shape check. `JsonUtility` then normalizes that null into a default instance. This is the null/absent-versus-default confusion that spec §4.2 and FR-07 ("Reject; never normalize") forbid for new profiles. `scenarioOne` itself is caught explicitly at `:96`; other records are not.

The finding is latent today, because `ValidateSelectedJson:29` always refuses new profiles. It must close before any T-03 enable. A narrow fix is to reject null for non-string reference types inside `ValidateNewShape`.

### m-3 — MINOR (packet gap) — The in-Assets Editor gate now false-fails

**Where:** `Assets/ForestPrototype/ScenarioOneInteractionVerification.cs:133,143,289,315,331,378,437,452`. This is an `#if UNITY_EDITOR` opt-in gate. It passes cloned DTOs to `LoadData`, which is now refused, and calls `RestoreSaveData`, which is now a no-op.

**Failure scenario:** the next Unity verifier runs that gate and gets red results, or vacuous ones after a no-op restore, that look like gameplay regressions.

FR-01 requires explicit migration of direct verification callers. The T-02 packet did not allocate these files, and the worker disclosed the issue. **This is a packet gap, not a T-02 demand:** the Manager should allocate the migration before T-05.

### m-4 — MINOR — A generic message overwrites the specific apply-time refusal

**Where:** `ForestSaveController.cs:385` and `:376`. When `Apply` refuses inside `TryPrepare`, it has already called `SetMessage("Save not loaded: <reason>.")`. `TryLoad` then sets `problem = "Save not loaded."`, and `Load()` calls `SetMessage(problem)`, replacing the reason.

**Failure scenario:** the scene state changes between the two `TryPrepare` calls, for example the active definition or the tree count. The player then sees "Save not loaded." with no cause.

**Fix:** return Apply's problem, or skip the second `SetMessage` when `Apply` has already shown one.

### N-1 — NIT — `TryPrepareReference` does not catch every exception

**Where:** `ForestSaveController.cs:463-464`. The catch list omits `InvalidDataException` (gzip), `NullReferenceException`, `InvalidCastException` and `FormatException`.

**Failure scenario:** an unexpected archive shape would throw out of `TryBeginReferencePreview` instead of refusing. In practice this is unreachable, because the archive has already passed hash verification in `ScenarioReferenceArchive.Load`.

### N-2 — NIT — The foreign-ID preflight also inspects `scenarioOne` in v1–v9 files

**Where:** `ForestSaveValidation.cs:52`. Baseline ignores `scenarioOne` for v<10.

**Failure scenario:** none with genuine files, because v1–v9 writers never emitted `scenarioOne`. This is noted only for completeness of the over-reach audit.

### N-3 — NIT — A refused `CanRestoreCapture` gives no player feedback

**Where:** `ScenarioOneManager.cs:702`. Preview entry returns `false` without setting `feedback`, unlike the other refusal branches. This is cosmetic.

## Could not verify

- Unity compilation, import and runtime. Native `JsonUtility` behaviour for absent, null, 0 and negative fields, and for duplicates or parser differentials (m-1).
- Native same-state serialization equivalence.
- The real Awake/Start/reset callback order for understorey initialization.
- Native UI focus and rendering on refusal.
- Real save, PlayerPrefs and Reference bytes on this machine. I did not read or write them.
- Genuine v1–v7, v10–v11, v13–v18 and v23–v24 fixtures. None is available, as the worker also states.
- The T-03 staging, T-04 writer/recovery and T-05 ISO items are out of scope. All 63 ISO boxes remain unchecked.

## Reproduction (scratch only)

The commands below ran under this session's scratchpad. The evidence directory and the source worktree were not modified.

```bash
git archive 2353a792 Assets/ForestPrototype Tools/Verification | tar -x -C $S/src
# copy resume-3 harness/generator to $S/h; redirect csproj paths and Stubs.cs persistentDataPath to $S
dotnet run --project $S/h/gen/generator.csproj -- $S/src $S/h
dotnet run --project $S/h/harness.csproj -c Release      # TOTAL PASS 352; FAIL 0
python3 $S/mut.py $S                                     # 12 mutants: 11 killed, M11 equivalent
```
