# Scenario One shared save — Spec V2 approval candidate: independent nonauthor review

**Claim:** [#539](https://github.com/jermur788/CCF/issues/539) (V2-C), parent [#535](https://github.com/jermur788/CCF/issues/535). **Date:** 15 November 2026.

## 1. Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), running in the Claude Code harness inside T3 Code. Reasoning effort was set by the dispatching session. #539 asked for **medium**, but the reviewer cannot observe the effort setting directly. No sub-agents were used.
- **Independence:** the candidate was written by OpenAI GPT-6.1 Sol (`gpt-6.1-sol`) under #538, and so was the G0 audit (#537). This reviewer is a different provider, a different model and a different session. It did not write, edit or contribute to any candidate file, the G0 audit or the #530 draft. **The reviewer has not changed the candidate.** This report is the only file written.

## 2. Exact target

| Item | Value |
| --- | --- |
| Reviewed commit | **`117f76179e828244a8d084f63ae10070ace1c8cf`** (branch `docs/scen1-shared-save-final-spec-candidate-20261115-v1`) |
| Changed-policy base | `88a2db30c17d076730fad4ad0b1a3145f16faabc` (#530 DRAFT). The changed policy was read with `git diff 88a2db30 117f7617 -- specs/scenario-independent-save-isolation-v1 Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115` |
| Files read in full | `specs/scenario-independent-save-isolation-v1/{spec.md (256 lines), plan.md (88), tasks.md (92), checklists/requirements.md (116)}` and `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-SPEC-V2-APPROVAL-CANDIDATE-20261115.md` (166) |
| Audit checked against | G0 at `3c8d7a50a8f9f1baae417c46d7d0a06b68e495f8`, `scen1-SHARED-SAVE-G0-SOURCE-PROVENANCE-AUDIT-20261115.md` (1110 lines). §§1–10 read |
| Source for spot-checks | `fc18190b976f1363f1b5d1b0881810db2f8c6e39`, read with `git show fc18190b:<path>`. `Assets/` is identical between fc18190b and 117f7617 (empty `git diff --stat`) |
| Canon cross-check | `Docs/Project/decision-log.md` at fc18190b (D-056, D-064, D-065, D-071 and D-081 present; D-082 absent) |
| Owner rules used for test 2 | #538 issue body, "owner's approved design defaults" and its "Explicitly NOT owner-approved" list |

## 3. Verdict

# NEEDS CHANGES

There are **0 BLOCKER, 5 MATERIAL, 5 MINOR and 3 NIT** findings.

This verdict is only about whether the **document** is fit for an owner signature. It is not Unity verification, runtime evidence, integration acceptance or approval of the spec.

What passes: B-09, B-10 and B-11 are turned into requirements to close each gap, and nothing claims they are safe today (test 1). The historical admission defaults are unchanged, and OPTION L sits outside the signature (test 3). The preservation anchors are intact (test 4). All 63 IDs are present, in order 01–63, with none checked, matching the base (test 5, structural part). §0.3 explicitly excludes D-082, Unity, builds, compact promotion and O2 availability (test 6, mostly). Filename, envelope and counter literals are labelled RECOMMENDED (test 7, mostly). Every source citation I checked reproduces (test 8). Profile count, filenames, F5 behaviour and Reference scope agree across all four files (test 9).

What fails: five material problems would let a signer approve more than the document admits, or approve contracts that cannot be tested. They are listed below.

## 4. Findings

| ID | Severity | file:line | What is wrong | What would resolve it |
| --- | --- | --- | --- | --- |
| M-1 | MATERIAL | `spec.md:166` (FR-06), `spec.md:164` (FR-05), `spec.md:239` (Q-01), against `spec.md:240` (Q-02) and `spec.md:49` | The owner rule is "Historical saves, Continue visibility … preserved unchanged" (#538). The package still changes runtime behaviour in the historical profile, and nothing tells the signer. (a) FR-06 offers "Load validated backup" when the historical primary is **missing** but a `.bak` exists, so the startup now shows a load action where today it shows only no-save. (b) FR-05 removes the `File.Copy` fallback for *all* profiles. A historical Save that succeeds today on a platform without `File.Replace` would now refuse. (c) FR-05 adds "validate captured result before IO", a new possible refusal of a historical save. (d) Q-01 requires an explicit profile choice before Continue once several profiles are available. §0.3 item 1 presents historical preservation as untouched. | Add a "changes to historical-profile behaviour" list to §0.3 and register it in §0.1. For each of (a)–(d), either confirm it is within the owner rule or scope it out of the historical profile (for example, no backup offer when the historical primary is missing). Record the platform evidence needed before removing the fallback. |
| M-2 | MATERIAL | `spec.md:118`, `spec.md:50`, `checklists/requirements.md:75` (ISO-44), against `spec.md:249` and `spec.md:229` | The signature (§0.3 item 2) ratifies "Compact30=2", which §4.1 and ISO-44 bind to **30×30 m / 36 cells**. §8.4 says the compact 30 m world is outside this signature, and §8.2 B-11 admits the audit "does not choose 2 or 30m". #538 lists the geometry choice as not owner-approved. So signing a save policy would quietly fix a compact world dimension. | Either make the signed value only "2 = geometry key `compact30-v1`, dimensions supplied by the separately approved world manifest", or list "30 m / 36 cells" in §0.3 as something the signature explicitly ratifies. |
| M-3 | MATERIAL | `spec.md:239` (Q-01), `spec.md:50`, against `spec.md:54`, `spec.md:69` and `spec.md:230` (B-12) | Q-01 orders the menu "Compact Scenario One, Existing Scenario One, Scenario Two", and §0.3 item 2 signs it. Listing compact first is a first-demo prominence choice that anticipates #471/D-082, which the package says it excludes. | Keep the historical profile first, or as the default, until D-082 is decided, or make the Q-01 order conditional on D-082. Say in §0.3 that the signature does not set first-demo prominence. |
| M-4 | MATERIAL | `spec.md:101` (W-03), `spec.md:148` (SI-08), `spec.md:177` and `spec.md:221` (B-03), against `Docs/Project/decision-log.md:143` (D-081 at fc18190b) | D-081 canon requires the save spec to "reject wrong-scenario loads **before any mutation**". B-03 keeps the late-warning admission of foreign IDs in the historical slot. The only pre-mutation protection for the historical slot is that canonical compact/O2 envelopes fail the unchanged legacy validators. W-03 and SI-08 state that as fact, but line 177 only says "verify on candidate", and the outcome depends on unverified JsonUtility behaviour for an absent version (`spec.md:93`). SI-08 and line 177 also forbid adding any rejection to the legacy slot, so if ISO-42 fails there is no permitted fix. The spec never mentions D-081 in this context. | Name D-081 and state how it is satisfied (CCF-written envelopes are refused before mutation, proven by ISO-42). Mark the premise UNVERIFIED in W-03 and SI-08. Say what happens if ISO-42 fails: return to the owner with a narrow envelope-root refusal as an explicit OPTION, not an improvised fix. |
| M-5 | MATERIAL | `spec.md:133`–`135` (§4.2), `spec.md:187` (FR-11), `checklists/requirements.md:104` (ISO-58) | Three parts of the compact evidence contract cannot be tested as written. (a) No player action is defined that sets `arrivalAcknowledged` or `assessmentAcknowledged`. (b) Review acknowledgement counts only "if it proves this actual review was read; implement the narrow … seam where required", which hands the design to the implementer. (c) The player "designates the single teaching group through real Work Plan interaction" with no defined UI or event. Current source has exact-planting orders and an `OrderCancelled` event, but no group-designation mechanism. ISO-58 cannot be passed or failed against undefined triggers. | Define the observable trigger for each flag, the review-read signal and the designation interaction. Otherwise mark those parts of ISO-58 and §4.2 BLOCKED on the separate compact world/teaching spec, and keep them out of the §0.3 package. |
| m-1 | MINOR | `spec.md:14` (C-02 tag), `spec.md:49` | "Unchanged historical admission" is presented as an owner-fixed rule (C-02 OWNER-APPROVED; §0.3 item 1). #538 lists foreign-ID tightening, v10–v24 missing-ID classification and permanent future-version policy as **not** owner-approved. Keeping them as they are is the B-03/B-04/B-05 PRESERVE recommendation, not an owner rule. This inflates the tag. | Re-tag the admission part of C-02 and §0.3 item 1 as a preserve disposition under B-03/B-04/B-05 (§0.3 item 3). |
| m-2 | MINOR | `checklists/requirements.md:39` (ISO-20) | "Process-interruption qualification leaves old/new complete current or recoverable old bytes; unsupported guarantees remain unverified" is an open-ended escape. Which interruption cases must pass and which may only be recorded as limits is not defined. | List the interruption points that must pass, and state that any others are recorded as limits, not passes. |
| m-3 | MINOR | `checklists/requirements.md:25` (ISO-12), `spec.md:162` (FR-04) | The sentinel includes "visible/audio state" with no listed members, so it cannot be compared. | List the components, or drop them from the sentinel. |
| m-4 | MINOR | `spec.md:150` (SI-10), `checklists/requirements.md:112` (ISO-63) | The materiality rule applies "after first shipped use", but "shipped" is never defined. Windows builds are gated, and integration and personal playtest are separate events. | Name the trigger event, for example the first integration to `main` of a writer for that profile. |
| m-5 | MINOR | `checklists/requirements.md:23` (ISO-10), `checklists/requirements.md:30` (ISO-14) | "Same admission/restored facts" compares an unlisted set of facts. | Point to the FR-04 sentinel set, or list the facts. |
| n-1 | NIT | `spec.md:79`, `spec.md:219` | The filename "site" `ForestSaveController.cs:15` is the `Path.Combine` line. The literal `"forest-save.json"` is at `:9` (`SaveFileName`). The `:286` "kept as .bak" message text is at `:287`; the ternary starts at `:286`. | Cite `:9`/`:15` and `:286–287`. |
| n-2 | NIT | `spec.md:99` (W-02) | "F10/Continue load" is wrong. F10 toggles the session menu (`UI/StandaloneSessionMenu.cs:70`), and "Continue current forest [F10]" closes it (`:122`). The confirmation belongs to the menu Save (`:125`). F9 is the immediate load. | Reword to: F9 load, Continue (startup), and menu Save with replacement confirmation. |
| n-3 | NIT | `tasks.md:37` (T-02 Checks) | T-02 lists ISO-07–13, 41–44 and 46–49 as "static/offline" checks, but those items also carry Unity Tier 2, and a combined label needs every type to pass (`checklists/requirements.md:7`). T-02 can only progress them. | Say "static/offline portions of". |

No BLOCKER: every material issue can be fixed with wording or scope in a revised candidate, and none is a false claim of existing safety.

## 5. Results against the nine tests

1. **CONTRADICTED gaps:** PASS. B-09: `spec.md:81` ("B-09 is CONTRADICTED as an existing guarantee. New safe failure/recovery implementation and injection proof required"), `:227` ("GAP MUST CLOSE"). B-10: `:87` ("B-10 current universal safety CONTRADICTED. Need early selection/staged commit"), `:228`. B-11: `:88` ("B-11 drop-in extension safety CONTRADICTED. Exhaustive dispatch required"), `:229`. The tasks ledger at `tasks.md:83` agrees. I found no statement of existing safety. The one premise stated as fact without proof is the M-4 envelope refusal.
2. **Approval inflation:** mostly PASS. Overlay, "pre-existing" clearing and no road-safety claim all appear in #538's owner list. Inflation found: m-1. Signature scope that reaches beyond owner rules: M-2, M-3.
3. **Legacy admission:** PASS on defaults. FR-07 (`spec.md:170–179`), SI-05 (`:145`), B-03/04/05 (`:221–223`), plain historical geometry2 still rejected (`:93`, ISO-49), and OPTION L is outside the signature (`:54`, `:251`). The legacy-adjacent changes that remain are runtime UX and writer changes (M-1), not admission.
4. **Anchors:** PASS. `spec.md:205`: sha256 `789e3edd…b041fc`, `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, v1–v24, Legacy40=0 / Enlarged80=1, D-056/D-064/D-065/D-071. Source prefixes confirmed (spot-checks #16). D-081 six species kept (`:71`, ISO-29).
5. **63 ISO items:** structurally PASS. Both base and candidate have 63 items, IDs 01–63 in order, identical ID sequence (hashes match) and 0 checked. Not checkable as written: ISO-58 (M-5), ISO-20 (m-2), ISO-12 (m-3), ISO-63 (m-4), ISO-10 and ISO-14 (m-5). ISO-42 and ISO-07 are testable but have no defined remedy (M-4).
6. **§0.3 honesty:** mostly PASS. `spec.md:54` excludes code, Unity/Player/build, integration, OPTION L, corpus waiver, compact first-demo promotion, H1/G1/completion and O2 availability. No Windows build is authorised (`:207`, ISO-35). Scope creep: M-2 (world dimension), M-3 (D-082-adjacent order), M-1 (undisclosed historical behaviour changes).
7. **Smuggled decisions:** filenames, envelope keys, counters and prefixes are visibly RECOMMENDED (`spec.md:113`, `:125`, `:149`, C-09/C-10/C-14). Compact30=2 is labelled, but its 30 m dimension is not visibly treated as unratified (M-2). Materiality is labelled "proposed" (`:150`, C-18).
8. **Citations:** 22 citation groups checked (table below). All reproduce, with two off-by-one or anchor-line nits (n-1).
9. **Consistency:** PASS on three profiles, nine paths, eight presence combinations, six cross-placements, filenames (§4.1 only, referenced elsewhere), geometry (spec `:118`, plan `:28`, tasks `:46`, ISO-44), F5 no modal / F10 confirmation (spec `:99`/`:241`, plan `:67`, tasks `:57`, ISO-03) and historical-only Reference (spec `:189`, plan `:29`, tasks `:47`, ISO-52). Wording nits: n-2, n-3.

## 6. Citation spot-check (source at fc18190b, paths under `Assets/ForestPrototype/`)

| # | Cited in | Citation | What the source shows | Result |
| --- | --- | --- | --- | --- |
| 1 | spec:79 | `ForestSaveController.cs:15` | `SavePath => Path.Combine(Application.persistentDataPath, SaveFileName)`; literal at `:9` | Reproduces (path decision); literal is at :9 (n-1) |
| 2 | spec:79 | `UI/StandaloneSessionMenu.cs:77` | `Path.Combine(Application.persistentDataPath, "forest-save.json")` | Reproduces |
| 3 | spec:80, :223 | `ForestSaveData.cs:7–9` | `CurrentVersion = 24`; `public int version = CurrentVersion;` | Reproduces |
| 4 | spec:81, :227 | `ForestSaveController.cs:84–85` | `File.Copy(SavePath, BackupSavePath, true); File.Copy(temp, SavePath, true);` inside `PlatformNotSupportedException` catch (:81) | Reproduces |
| 5 | spec:81 | `ForestSaveController.cs:49` | `"Save failed … Your previous save is unchanged."` | Reproduces |
| 6 | spec:81 | `ForestSaveController.cs:286` | Ternary start at 286; "kept as {SaveFileName}.bak" text at 287 | Reproduces (n-1) |
| 7 | spec:82, :221 | `ScenarioOne/ScenarioOneManager.cs:1481(–1483)` | Foreign-ID condition 1481, `LogWarning` 1482, restore continues 1483 | Reproduces |
| 8 | spec:83 | `ForestSaveController.cs:324–337` vs `:268–274` | `LoadData(dto)` normalises `<=0` and runs `FindLoadProblem` with no raw validators; disk path runs six raw validators at 269–274 | Reproduces |
| 9 | spec:84 | `ScenarioOneManager.cs:1451–1456` | Public `RestoreSaveData(data)` → versioned overload; `TeachingWorldVersion++` at 1456 | Reproduces |
| 10 | spec:85 | controller `:134`; manager `:1398`, `:2525–2533` | `data.scenarioOne = scenario.CaptureSaveData()`; `EnsureUnderstoreyGrid()`; `understoreyCells.Clear()` … `InvalidateCompetition()` | Reproduces |
| 11 | spec:86 | manager `:725–740` | `referencePreviewActive = false` before load; on failure logs, sets `previewReturnData = null`, says "Returned" | Reproduces |
| 12 | spec:87 | `ForestStartingStand.cs:68–70`; `UI/StandaloneSessionMenu.cs:28`; `.Startup.cs:102–105` | Applies geometry and `Generate()` in Awake; `AfterSceneLoad` bootstrap; dismiss/close/`Load()` after `CanLoad` | Reproduces |
| 13 | spec:88, :229 | `StandGeometryModel.cs:34`; `ForestStartingStand.cs:83–89` | `model == Enlarged80 ? 80f : 40f`; only an `== Enlarged80` branch, else legacy lattice | Reproduces |
| 14 | spec:89, :225 | `ScenarioOne/ScenarioReferenceArchive.cs:121–128`; manager `:677` | `IdentityMatches(definition, ecology)`, no profile parameter; preview calls `IdentityMatches` | Reproduces |
| 15 | spec:90, :232 | `ScenarioOne/ScenarioOne.asset:15–16` | `scenarioId: scenario-one`, `definitionVersion: scenario-one-v13` | Reproduces |
| 16 | spec:91 | `UI/LearningObjectivesView.Curriculum.cs:9`; `UI/MenuHelpView.cs:11`; `LearningObjectivesView.cs:87–90`; `MenuHelpView.cs:68–71` | `"CCF.Learning.v1."`, `"CCF.MenuHelp.v1."`; enumerated `DeleteKey` resets | Reproduces |
| 17 | spec:93 | `ForestSaveValidation.Geometry.cs:14–34` | Pre-v20 only Legacy40; `IsKnown` check; v20+ explicit integer token | Reproduces |
| 18 | spec:93, :223 | `ForestSaveController.cs:346` | `version > CurrentVersion` → "loading best-effort" warning | Reproduces |
| 19 | spec:222 | `ForestSaveData.cs:57` | `public string scenarioId = "scenario-one";` | Reproduces |
| 20 | spec:43 | `ScenarioOne/ScenarioOneWorkOrder.cs:31`; `ForestSaveData.cs:217`; manager `:753–754` | `int workOrderId`; `string juvenileId`; `"beech"`/`"sessile-oak"` | Reproduces |
| 21 | spec:241 | `ForestSaveController.cs:33–34`; `UI/StandaloneSessionMenu.cs:125` | F5 → `Save()` immediately; menu Save `Confirm("Replace your existing forest save…")` | Reproduces |
| 22 | spec:233 | manager `:694/:707/:727` | `CaptureData()`; `referencePreviewActive = true`; `LoadData(previewReturnData,false)` | Reproduces |

No citation failed to reproduce. All paths exist at fc18190b as cited.

## 7. What this review did not do

- It did **not** edit, fix or reformat any candidate file. It checked no ISO item and changed no status, decision log, milestone, board, dashboard or Drive mirror.
- It did **not** run Unity, the Editor, a Player, a compiler, any test harness, a JsonUtility experiment or `File.Replace`/fault injection. It did not access any `forest-save*` file, PlayerPrefs, the Reference `.bytes` content or `/media/jer/ZX20/CCF-Ops/state/unity/`. It did not touch `/media/jer/Files/CCF-Primary` or `/media/jer/Files/CCF/Orchestration/CCF-Scen1-SharedSave-20261115`.
- It did **not** recompute the Reference SHA-256 or any fixture hash. Anchor values were checked as text against G0 and the spec only.
- It did **not** re-review text that was unchanged from #530, except where needed to judge the changes. It did not re-audit G0 beyond the citations listed above.
- It does **not** approve the spec, verify any runtime behaviour, accept integration or authorise implementation. Only an owner/authorised §0.3 signature on a revised candidate can approve it.
