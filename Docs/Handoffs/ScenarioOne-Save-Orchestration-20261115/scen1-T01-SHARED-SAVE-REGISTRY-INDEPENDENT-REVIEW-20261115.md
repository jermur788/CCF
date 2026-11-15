# Scenario One shared-save T-01 — independent non-author source review

**VERDICT: APPROVED FOR T-02 CONSTRUCTION** — 0 BLOCKER, 0 MATERIAL, 3 MINOR, 2 NIT.

This verdict covers **source safety for the next stage only**. It is not Unity verification, runtime evidence, integration acceptance, ISO conformance or approval of the spec. The Manager #424 verdict is still required before any T-02 handoff.

## Reviewer identity and independence

- Reviewer: **Anthropic Claude Opus 5.5** (`claude-opus-5-5`), effort **medium** (owner-preauthorised in #550), Claude Code harness in T3 Code.
- Claim: [#550](https://github.com/jermur788/CCF/issues/550). Parent [#544](https://github.com/jermur788/CCF/issues/544); implementation [#549](https://github.com/jermur788/CCF/issues/549).
- Independence: the author was OpenAI GPT-6.1 Sol in a separate session. This review uses a different provider and a different session. I did not write, edit or advise on this code, and I fixed nothing.
- Worktree: `/media/jer/Files/CCF/Operations/CCF-550-T01Review`, branch `review/scen1-t01-save-registry-independent-20261115-v1`, created at the review target. My only repository write is this file.

## Reviewed commit

- Target: **`da962372a7d45d8d049e9ad0dd2a657e5c339a2a`** (`feature/scen1-shared-three-profile-save-v2`).
- Parent `f52dec246d8e68608150e60f7e26f3104a38a836`, whose parent is the audited baseline `fc18190b976f1363f1b5d1b0881810db2f8c6e39`. Linear history; no merge commit.
- `git diff --stat fc18190b da962372`: the 9 declared files (3 new `.cs`, 3 new `.meta`, `ForestSaveController.cs` 8 lines, `UI/StandaloneSessionMenu.cs` 2 lines, 1 result document), plus `Docs/Project/coordination/active-tasks.md`, which comes from `f52dec24` (the Manager's board commit), not from the author.
- Contract: `200add4e6e5116268b79ef63131ff9b836f14c5d:specs/scenario-independent-save-isolation-v1/spec.md` §4.1, §4.2, §4.3 SI-01..SI-10, §5 FR-01/FR-10/FR-13. I read the signed text directly.
- Scope ruling: `c62523b980421ea40e05570ed2c4b9602e00ac67:Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-T00-ACCEPTANCE-T01-BASE-OWNERSHIP-OPS-RULING-20261115.md`.

## Findings

| # | Severity | file:line | Finding | What resolves it |
|---|---|---|---|---|
| F-1 | MINOR | `Assets/ForestPrototype/ForestSaveProfiles.cs:103-111` | `ForestSaveProfiles.GetPaths(profile, root)` is public and does **not** call `CanUse`. It returns the reserved Compact/O2 primary, `.tmp` and `.bak` paths. The comment says it "grants no operation permission", and no live code calls it (only `ForestSaveSessionContext.GetSelectedPaths:47-51`, which checks `CanUse` first and throws). So nothing reaches the compact or O2 slot today. But it is a ready-made way around `CanUse` for a T-02/T-04 implementer who wants "the path for profile X". | Before T-02 starts, the packet must state that admission, reading and writing get paths **only** through `ForestSaveSessionContext.GetSelectedPaths`/`CurrentPaths`, and that a direct `GetPaths` call in any operational route is a review failure. Optionally rename it (e.g. `GetReservedPathsForInspection`) or guard it in T-02. No T-01 source change is required. |
| F-2 | MINOR | `Assets/ForestPrototype/ForestSaveEnvelope.cs:13-14` | The comment says `ccfCompactTeaching` is an "explicit null for O2". Unity's serializer (and so `JsonUtility`) does not support null for fields of custom `[Serializable]` class type: `ToJson` writes a default object and `FromJson` produces an instance. So a typed round-trip through this class cannot give the signed §4.2 O2 `null`, and typed deserialisation cannot tell a null from a default object. This rests on documented Unity behaviour; I have **not verified it in Unity**. The file header (lines 4-6) already requires raw-key checks before trusting values, and nothing live uses the type, so T-01 is not at risk. | In T-02, check `ccfCompactTeaching` presence and nullness on the **raw JSON**, never on the deserialised object. In T-04, write O2 `null` by raw emission, not `JsonUtility.ToJson(ForestSaveEnvelope)`. Both packets should name this, and ISO evidence should include an O2-null raw round-trip. |
| F-3 | MINOR | `Assets/ForestPrototype/ForestSaveSessionContext.cs:62`, `:81-82`; consumers `ForestSaveController.cs:13-17`, `UI/StandaloneSessionMenu.cs:77` | If a second record is ever made available (`IsAvailable: true`) before the T-03 explicit-choice bootstrap exists, `CreateInitialSelection` returns an empty context, `BeginInitialWorldCreation` throws at `BeforeSceneLoad`, and `CurrentPaths` then throws on **every** historical F5/F9/menu path access. This fails closed, which is correct (no wrong slot, consistent with H-04). But one edited flag would silently turn into "historical save/load broken". This is not reachable at this commit: availability is set at compile time and only Historical is `true`. | T-03 (or whichever stage first binds Compact/O2) must make the availability flip and the explicit-choice bootstrap one change, with an offline test that historical `CurrentPaths` still resolves after the choice. Record this as a T-03 entry condition. |
| F-4 | NIT | `Assets/ForestPrototype/ForestSaveProfiles.cs:24` | `"CCF.UI.ReduceMotion"` is **not a signed-spec literal**. It matches the unmerged #443 candidate (`acb4e111:Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:61`, `:217`) and is absent from `origin/main`. It is authorised as an inventory sentinel by the Manager ruling §3 ("including sentinel for additive global ReduceMotion"), has no consumer, and is not a copied hunk (see test 7). | No T-01 change. The future #443 integration packet must update or remove this constant if the key changes there. |
| F-5 | NIT | Author report `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-T01-SHARED-SAVE-PROFILE-REGISTRY-RESULT-20261115.md:116`, `:124` | The "all eight virtual primary-presence combinations: 64 assertions" group is close to tautological. The registry never looks at file presence, so the matrix only shows that a hash set contains the strings put into it, and that availability is constant. Calling this "proving independent path visibility" overstates it. About 20% of the 310-assertion count shows nothing beyond the fixed path table. | No source change. Do not count this group as slot-isolation evidence for any ISO item. Real presence/corruption isolation belongs to T-02/T-04 runtime evidence. |

No BLOCKER or MATERIAL finding survived verification.

## The nine tests

### 1. Literal fidelity — PASS (one non-spec literal, authorised by the ruling)

Each literal in `ForestSaveProfiles.cs:9-24`, `:58-72` and `ForestSaveEnvelope.cs` compared with signed `spec.md` (line numbers are in the 200add4e blob):

| Literal | Code | Signed source | Result |
|---|---|---|---|
| `scenario-one`, `scenario-two` | `:9-10` | §4.1 line 147-149 | match |
| `o1-historical` | `:11` | §4.1 line 147 "internal router profile `o1-historical`" | match (see OBS-2) |
| `o1-compact-v1` / `scenario-one-compact-v1` / `compact30-v1` / `scen1-compact-h1-g1-v1` | `:12`, `:63-64` | §4.1 line 148 | match |
| `o2-working-mixed-v1` / `scenario-two-working-mixed-v1` | `:13`, `:69` | §4.1 line 149 | match |
| `o2-working-mixed-geometry-v1` / `o2-working-mixed-content-v1` | `:70` | §4.1 line 149 "reserved …" | match (see OBS-1) |
| `forest-save.json`, `forest-save-scenario-one-compact-v1.json`, `forest-save-scenario-two.json` | `:59`, `:64`, `:70` | §4.1 lines 147-149 | match |
| `.tmp` / `.bak` sidecars | `:124-125` | §4.1 line 151 | match |
| six SI-09 prefixes | `:15-20` | SI-09 line 180 | match, byte-exact incl. trailing dot |
| Historical world/geometry/content = `null` | `:59` | §4.1 "no persisted new profile/world ID" | consistent |
| envelope root keys, 7 identity keys, 6 evidence keys, 4 planting keys | `ForestSaveEnvelope.cs:11-14`, `:23-29`, `:39-44`, `:50-53` | §4.2 lines 157-166 | match |
| `SupportedSchemaVersion=1`, `SupportedPayloadVersion=24`, `SupportedEvidenceVersion=1` | `ForestSaveEnvelope.cs:20-21`, `:37` | §4.2 "schemaVersion=1, payloadVersion=24", "evidenceVersion integer1"; `ForestSaveData.cs:7` `CurrentVersion = 24` | match |
| `CCF.UI.ReduceMotion` | `:24` | **not in signed spec**; Manager ruling §3 | F-4 |

There is no Compact30 numeric `2`, no dimensions, no cell count, no H1/G1 content and no geometry-number support, so §4.1's "numeric dispatch reservation only" is respected by leaving it out. The three unavailable-reason strings are display text, not routing keys.

### 2. Historical slot safety — PASS (I traced it in the source)

- `ForestSaveProfiles.Historical` (`:58-60`) is the only record built with `isAvailable: true`. The records are `static readonly` and their properties are get-only. `All` (`:75`) is declared after the three fields, so C# static field initialisation order fills it with non-null records.
- `CreateInitialSelection` (`ForestSaveSessionContext.cs:54-67`) picks the single available record and calls `TrySelect("o1-historical")`, which succeeds. The same function runs from the static initialiser (`:12`) and from the `SubsystemRegistration` reset (`:69-74`). It does not touch `Application` or the disk, so it is safe in a static initialiser.
- `CurrentPaths` (`:15`) → `GetSelectedPaths(Application.persistentDataPath)` → `CanUse(Historical)` is true → `GetPaths` → `Path.Combine(Application.persistentDataPath, "forest-save.json")`. Before T-01 this was `Path.Combine(Application.persistentDataPath, SaveFileName)` with `SaveFileName = "forest-save.json"` (base `ForestSaveController.cs:9`, `:15`) and the literal in the menu (base `StandaloneSessionMenu.cs:77`). Same call, same arguments, so the string is the same. `.tmp`/`.bak` are `Primary + ".tmp"`/`".bak"` (`ForestSaveProfiles.cs:124-125`), the same as the old `SavePath + ".tmp"`/`".bak"`.
- `GetSelectedPaths` does not require the seal, so edit-mode or pre-`BeforeSceneLoad` callers still resolve historical. With domain reload disabled, a context sealed in an earlier Play session is replaced at `SubsystemRegistration`, and a sealed context still resolves the same path.
- The only `Assets/` code consumers of the filename are the two edited properties (`git grep 'forest-save' da962372 -- Assets` also shows only `README.md:50` and the registry). The hardcoded paths under `Tools/Verification/` are not compiled, are out of T-01 scope, and still point at the same historical file.
- I compared the full diff of both consumers myself: apart from the property bodies and the removed constant, no method bytes change.

### 3. Fail-closed — PASS, with F-1 noted

Compact and ScenarioTwo are compile-time `isAvailable: false`. There is no setter, registration API or binding hook. `CanUse` accepts only the three registered instances (`IsRegistered` uses `ReferenceEquals`), so a caller-built lookalike with `isAvailable: true` is refused. `TrySelect` refuses them, `TryBeginWorldCreation` refuses an unselected or unavailable context, and `GetSelectedPaths` throws `InvalidOperationException` instead of falling back. No live save/load/continue/start route mentions Compact or ScenarioTwo: `git grep` finds no reference to any new type outside the three new files and the two edited properties. The only way to resolve a reserved path without `CanUse` is `GetPaths` (F-1), which nothing calls.

### 4. Selection ordering — PASS

`TrySelect` returns false as its first check when `WorldCreationStarted` (`:19-23`). That includes reselecting the same ID, and a refusal leaves the earlier selection untouched. `TryBeginWorldCreation` requires `CanUse(SelectedProfile)` (`:42`), so it cannot seal an unavailable or null profile. `Current` has a private setter; it is only replaced by the private `SubsystemRegistration` callback, which Unity runs before `BeforeSceneLoad`. Nothing re-opens a sealed context. The `new ForestSaveSessionContext()` constructor creates empty, inert contexts that cannot be installed as `Current`. Engine callback ordering is documented Unity behaviour but **not verified here**.

### 5. Envelope is data-only — PASS, with F-2 noted

`ForestSaveEnvelope.cs` has only fields and `const` values: no methods, no IO, no serializer calls. Nothing anywhere references `ForestSaveEnvelope`, `ForestSaveEnvelopeIdentity`, `ForestCompactTeachingEvidence` or `ForestCompactPlantingEvidence` (whole-tree `git grep`). Its root keys `ccfSaveEnvelope`/`ccfWorldPayload`/`ccfCompactTeaching` do not overlap any `ForestSaveData` field. So `JsonUtility.FromJson<ForestSaveData>` reading an envelope would not fill historical fields from envelope keys. Whether the historical validators then refuse it is ISO-42, which is UNVERIFIED by the spec itself and out of T-01 scope. The historical DTO is used only as a nested field type and is unchanged.

### 6. Preservation anchors — PASS (checked independently)

`git diff --quiet fc18190b da962372` on `Assets/ForestPrototype/ForestSaveData.cs` (blob `56065ea164da688327aabe8707e026af9ff1b484`) and `Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs` (blob `56fe54af8a712137255f484160b976577975915c`) shows both unchanged. No changed path matches Reference/archive/`.asset`/`.unity`/`.prefab`/ProjectSettings/Packages. The signed spec folder is not present on the candidate branch, so nothing there was edited.

### 7. #443 isolation — PASS

`acb4e111` is not an ancestor of `da962372`, and `fc18190b..da962372` contains only two commits, both single-parent. I compared every added line longer than 12 characters in the T-01 `Assets/` delta (173 unique) with every added line in `merge-base(fc18190b, acb4e111)..acb4e111 -- Assets` (2,324 unique). The only shared lines are boilerplate: `fileFormatVersion: 2`, `return false;`, `using System;`, `using System.Collections.Generic;`. There is no retirement or preview-pose logic in the T-01 files (`grep -i 'retire|previewpose'` is empty). The only #443-derived item is the `ReduceMotion` key string (F-4), which the ruling authorises as an inventory sentinel.

### 8. Scope — PASS

There is no admission or validation code, no raw-JSON handling, no foreign-identity preflight, no staging/rollback/preview, no preference rewiring, no writer/recovery change and no geometry dispatch. The new helpers contain no `File.`/`Directory.`/`PlayerPrefs.`/`SceneManager.`/`JsonUtility.` calls. No path outside the allowlist changed apart from the Manager's own board commit `f52dec24`. The 63 ISO boxes at `200add4e` (`checklists/requirements.md`): 63 unchecked, 0 checked. The candidate branch does not carry that folder, so it cannot have checked any. The `.meta` files use the two-line format already used across `Assets/ForestPrototype`.

### 9. Offline check credibility — real but narrow; the report does not overstate Unity

I inspected `/media/jer/Files/CCF/Verification/SharedSaveT01-549-20261115/`. The SHA-256 of `Program.cs` (`f59aac5f…`), `UnityStubs.cs` (`4896cf85…`) and `offline-run.log` (`002e3329…`) match the author's manifest. I did **not** re-run the harness.

| Property | What the harness shows | What it does not show |
|---|---|---|
| 2 Historical slot | The compiled `CurrentPaths.Primary` equals `Path.Combine(stubbed persistentDataPath, "forest-save.json")`. The consumers' `SavePath` expression text is checked through the Roslyn syntax tree, and `preservation_check.py` checks the whole-file diff. | The real Unity `persistentDataPath`. Controller `TempSavePath`/`BackupSavePath` are checked only through `GetPaths`, not through the controller's own text. My manual trace above covers this. |
| 3 Fail-closed | Unknown, unbound and forged records refuse; `GetSelectedPaths` throws. | It shows `GetPaths` resolving reserved paths (F-1) but does not flag it. |
| 4 Ordering | The seal, post-seal refusals and the reset/seal callbacks are invoked **through reflection**. | The engine's `SubsystemRegistration` → `BeforeSceneLoad` → scene `Awake` ordering. |
| 5 Data-only | Exact field names and types, no public instance methods, root disjoint from the parsed DTO, no serializer calls in the three helpers. | That nothing else references the envelope (checked by my whole-tree grep, not by the harness). Real `JsonUtility` behaviour (F-2). |

The harness compiles only the three new helpers against 17 lines of hand-written stubs (`Application`, `RuntimeInitializeOnLoadMethodAttribute`, an empty `ForestSaveData`). The controller and menu are only parsed for C# 9 syntax. **Stub compilation is not Unity compilation**, and the author's report says so plainly. It does not claim Unity compilation, import or runtime. By reading the code I confirmed the helpers use only C# ≤ 7.3 and .NET Standard 2.1 APIs (`Array.AsReadOnly`, `IReadOnlyList<T>`, out-vars, expression bodies), so a language-level Unity compile failure is unlikely. That is still an inference, not evidence. The presence-matrix overstatement is F-5.

## OBS-1 / OBS-2 adjudication

**OBS-1 — DISMISSED.** `o2-working-mixed-geometry-v1` and `o2-working-mixed-content-v1` are signed literals. Signed `spec.md` §4.1, line 149, Geometry/content column, reads: "reserved `o2-working-mixed-geometry-v1` / `o2-working-mixed-content-v1`; no geometry number, dimensions, definition or content manifest exists by this proposal. Binding absent ⇒ deny all O2 operations". The spec leaves the O2 **binding, geometry number, dimensions and manifest** to separate approval, not the key strings. The code holds only the reserved strings, with no number, dimensions or content, and O2 cannot be used. No severity. The inertia risk is handled by SI-10: identity freezes only when a writer for that profile is first integrated to main, and the separate O2 approval may supersede these reserved keys before then.

**OBS-2 — DISMISSED (shown not to persist).** `o1-historical` is a signed literal: §4.1 line 147, "internal router profile `o1-historical`; no persisted new profile/world ID". Every place it flows (`git grep -E 'ProfileId|profileId' da962372 -- Assets`):
- `ForestSaveProfiles.cs:11` constant → `:59` record `ProfileId` → `:77-85` ordinal comparison in `TryGet`;
- `ForestSaveSessionContext.cs:17-24` `TrySelect` argument, `:65` initial selection.

It never becomes a filename: the historical filename is the separate literal `"forest-save.json"` (`:59`), and `GetPaths` uses `PrimaryFileName`, never `ProfileId`. It is never part of a pref key: the historical prefixes are the separate `CCF.Learning.v1.`/`CCF.MenuHelp.v1.` constants. It never goes into an envelope: `ForestSaveEnvelopeIdentity.profileId` (`ForestSaveEnvelope.cs:25`) is never assigned by any code, and §4.2 envelopes are new-profile only. It never goes into a save: `ForestSaveData.cs` is unchanged and the new types have no IO. It stays an in-memory identifier only.

## Citation spot-checks

| Claim checked | Source | Result |
|---|---|---|
| Author: base `f52dec24` = `fc18190b` + coordination file only | `git log --format='%H %P'`; diff-stat | confirmed |
| Author: "only three path-property substitutions" in the controller | `git diff fc18190b da962372 -- …ForestSaveController.cs` | confirmed (one removed constant + 3 property bodies) |
| Author: new `.meta` GUIDs `7d0a0235…`, `de9f7f84…`, `0001aaf0…` | the `.meta` blobs | confirmed |
| Ruling §3: "ForestSaveData.cs and ScenarioReferenceArchive.cs remain READ-ONLY" | anchor blob diff | confirmed unchanged |
| Ruling §3: ReduceMotion sentinel required | ruling text at `c62523b9` | confirmed, so F-4 is a NIT, not a violation |
| Spec §2: original filename at `ForestSaveController.cs:9/:15`, `StandaloneSessionMenu.cs:77` | base `fc18190b` diff hunks | confirmed |
| `ForestSaveData.CurrentVersion = 24` | `ForestSaveData.cs:7` | confirmed |
| `CCF.UI.ReduceMotion` absent on `origin/main` | `git grep origin/main` | confirmed absent |
| Spec FR-13: "Both current filename call sites must use the same registry query" | `ForestSaveController.cs:13`, `StandaloneSessionMenu.cs:77` both `ForestSaveSessionContext.CurrentPaths.Primary` | confirmed |
| 63 ISO items, 0 checked | `200add4e:…/checklists/requirements.md` | confirmed 63 / 0 |

## What this review did not do

- No Unity Editor, Player, import, Unity compilation, build or runtime. No callback-ordering or `JsonUtility` behaviour was observed (F-2 rests on documented Unity serializer behaviour).
- No save file, sidecar, PlayerPrefs or EditorPrefs was opened, hashed or touched. No historical-fixture same-state/byte equivalence test.
- Did not re-run the author's .NET harness or `preservation_check.py`; I inspected their source and checked artifact hashes only.
- Did not touch `/media/jer/ZX20/CCF-Ops/state/unity/`, `/media/jer/Files/CCF-Primary` (beyond the session's starting directory, unmodified), the parent orchestration worktree, `specs/`, `Docs/Project/`, `Assets/` or `CCF-Ops/`.
- Did not check any ISO item, change any status, or write the board, Decision Log, dashboard or Drive mirror.
- No approval of the spec, of T-02 release, of integration, or of the #443 reconciliation. The Manager #424 verdict is still required before a T-02 handoff.
