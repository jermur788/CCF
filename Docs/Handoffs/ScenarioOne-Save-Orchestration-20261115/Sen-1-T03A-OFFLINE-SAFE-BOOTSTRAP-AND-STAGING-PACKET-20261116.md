# Sen-1 — T-03A offline safe bootstrap and staging packet

**Issued:** 16 November 2026, under the [Sen-1 overnight player-test readiness amendment](https://github.com/jermur788/CCF/blob/docs/sen-1-overnight-player-test-amendment-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-OVERNIGHT-PLAYER-TEST-READINESS-AMENDMENT-20261115.md) §B/P1A. **Orchestrator:** DeepSeek V4.1 Flash 1017 high, [#544](https://github.com/jermur788/CCF/issues/544). **Parent source stage:** T-02R, provisionally accepted — see §0.

## 0. The predicate that authorises this packet, with its evidence

The amendment precommitted that **if** an independent nonauthor delta review returned **APPROVED with zero blockers and zero material findings** on the exact repaired candidate, with the signed contract and allowed files unchanged, #544 may record provisional T-02 source acceptance and begin P1A without waking the owner.

| Predicate condition | Evidence | Met |
| --- | --- | --- |
| Nonauthor delta review | [#569](https://github.com/jermur788/CCF/issues/569), Claude Sonnet 5.5 high, author was Opus 5.5 | yes |
| Verdict APPROVED | `Sen-1-T02R-DELTA-REVIEW-20261116.md` at `6b571919` (branch `sen-1/t02r-delta-review-20261116-v1`) | yes |
| Zero blockers | 0 | yes |
| Zero material findings | 0 (2 minor, 4 nit) | yes |
| Exact candidate | `460fd51cb8972dd5b7819632c94e64d1070a98b7`, source `5e21c944` | yes |
| Allowed files only | exactly 4 C# + 1 Markdown | yes |
| Signed contract unchanged | the four blobs at `200add4e` are **absent** from this source branch, therefore unmodified | yes |
| Reviewer reproduced the evidence | 460/0 rebuilt from committed bytes, 557/0 with extra probes, 27/28 mutants with every kill a real assertion failure | yes |

**Recorded verdict:** *T-02 SOURCE PROVISIONALLY ACCEPTED FOR T-03A OFFLINE CONSTRUCTION ONLY — conditional #424 ruling in the Sen-1 amendment.* This is **not** Unity acceptance, **not** merge permission, **not** integration, **not** an independent review waiver, and it does **not** tick any of the 63 ISO items.

**Independence limitation carried forward:** author and reviewer are both Anthropic (Opus then Sonnet) because both OpenAI pools were exhausted. A cross-provider re-check is owed when an OpenAI pool returns.

**Known non-material residuals, all carried to T-05:** MINOR-1 refused menu Save with no existing file appends its reason below "Quit CCF…" instead of the top note; MINOR-2 the harness lives outside Git with no pinned hash; the parser-differential fail-open half is real but the JsonUtility-accepts-what-Newtonsoft-rejects case is only simulated; and the `centuryReview` null assumption holds for every writer observable so far but **no v24 or new-envelope writer output has ever been seen** — if T-04 builds the payload with Newtonsoft rather than `JsonUtility`, produced files would be refused by our own validator.

## 1. Purpose and hard ceiling

Implement **offline, fail-closed scaffolding only** for staged profile selection and target application, so that a later authorised world implementation has a safe seam to sit on. This is a **signed-feature implementation slice under the immutable approved save contract**, not approval of a new gameplay feature.

**In scope, and nothing beyond it:**

1. **Profile selection resolved BEFORE any load, generation or scene/forest creation.** The selected profile must be decided and validated before anything is built, so no world exists to corrupt if selection fails.
2. **An admitted staged-target application boundary** that cannot corrupt the previous world on refused or failed input. Refusal or failure must leave the previous world exactly as it was; a partial apply is refused rather than completed.
3. **Historical-only Reference preview/return safety**, if and only if safely separable: entry and return all-or-nothing or guarded retry, with the snapshot returned before any guard is dropped, and scene/pose/year/cash/orders/Prefs preserved on failure.
4. **Numeric reservation of geometry model value `2`** as an exhaustive-dispatch case that **refuses** unless a bound, signed world manifest exists. The value is a reservation in a switch, not an implementation of a world.
5. **Preference namespace isolation** for the new profile, following the registry already approved in T-01.

**Explicitly out of scope — implementing any of these is a stop condition:**

- Any 30×30 m / 36-cell compact world, H1 clearing, walking path, 2 Oak + 2 Beech group, post-H1 light, or G1 positions.
- Any CG-TRIG trigger, teaching-completion predicate or assessment.
- Enabling Compact or Scenario Two availability, or adding any profile choice to the player UI.
- New UI, new scene, prefab, serialized asset, ScriptableObject or `.meta`.
- Changing the signed four-file contract, the Decision Log, or any spec.
- Inventing a world manifest, authored content or ecological claim.

## 2. Lane, writer and ownership

- **Branch:** `feature/scen1-shared-three-profile-save-v2` (the existing sequential save branch — no second overlapping worktree writer).
- **Worktree:** `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115`.
- **Starting HEAD:** `460fd51cb8972dd5b7819632c94e64d1070a98b7`. Re-verify clean and unmoved before editing; if dirty or moved, STOP.
- **Writer:** one primary writer at a time, registered with actual model/provider/effort, [RUNNING] receipt and a **narrower per-file, per-method allowlist** posted before editing.
- **Allowlist:** drawn only from the existing T-00 reserved set and limited to existing files —
  `ForestSaveSessionContext.cs`, `ForestSaveProfiles.cs` (only for the early-selection seam and preference namespace), `ForestSaveController.cs` (only for the staged-application boundary and refusal mapping), `ScenarioOne/ScenarioOneManager.cs` (only for selection-before-creation ordering), `ScenarioOne/ScenarioOneSaveMigration.cs` (only for separating capture/world-initialisation), and optionally `ForestSaveValidation.Geometry.cs` for the geometry `2` refusal dispatch.
  **No new file, no new `.meta`, no fifth-plus file** without a Manager-signed scope. A request for one is a stop condition, not a decision the writer makes.
- **Protected, never modified:** `ForestSaveData.cs`, `ScenarioReferenceArchive.cs`, archive/reference bytes, all real saves and preference files, the four signed blobs at `200add4e`, `ScenarioOneInteractionVerification.cs`, everything under `#443`.

## 3. Required behaviour detail

- **Selection before construction:** the profile must be resolved, validated and refused-if-unavailable *before* any scene load, forest generation, understorey initialisation or save read that would establish world state. Prove the ordering in the caller map, not just the callee.
- **No partial apply:** if any step of applying a staged target fails or is refused, the previous world must still be the previous world. A half-applied scene is a failure of this task.
- **Refusal preserves:** UI active selection, camera/pose, year, cash, orders, preferences and the live world.
- **Geometry `2`:** exhaustive numeric dispatch that refuses with a clear reason when no bound manifest exists. It must not infer 30×30 m, 36 cells, or any authored content from the number.
- **Compact and Scenario Two remain unavailable** (`IsAvailable` false). Flipping either to make a test easier is a stop condition.

## 4. Required evidence

- Source-level **caller map** and **ordering proof**: who calls selection, who calls construction, and the exact first point at which world state could be mutated.
- **Negative sentinels**: refused selection → nothing constructed; failed staged apply → previous world byte-identical (state snapshot before/after); unbound geometry `2` → refused, no generation; unavailable profile → refused before any read.
- **Reference preview/return** negatives if implemented: refusal or failure on return leaves the previous state intact.
- **Offline harness** results with real counts, and confirmation that no existing negative was weakened or deleted.
- **Roslyn typecheck against the real Unity 6000.6 reference assemblies** (player and Editor defines) if the writer can run it — valuable, and still not a Unity compile.
- A record of what was **not** run. No claim of Unity compile, runtime, rendering, native serialization or ISO qualification.

## 5. Stop conditions

Stop and publish a durable blocker, rather than improvising, if any of these is true:

- early selection or staged application cannot be implemented without a new UI, scene, serialized asset, undisclosed behavioural contract, new storage semantics, unsigned world manifest or broader source edit;
- the work implies any product choice (compact geometry, H1/path/G1 content, CG-TRIG, teaching completion);
- a historical save or Reference behaviour would change;
- a protected file would need editing, or a new file/`.meta` is required;
- source ownership conflicts, the branch moved, or the worktree is dirty;
- a genuine missing fixture makes a required criterion unprovable.

In those cases deliver the **focused T-03 design/fixture/caller map** and stop source changes at the safe seam.

## 6. Review gate after delivery

One focused **nonauthor independent source review** of the T-03A delta before any downstream save-writer production change. Independence must be stated honestly, including any same-provider limitation while both OpenAI pools remain exhausted. T-04 atomic writer/recovery and T-05 native Unity isolation qualification remain **separately gated** and are not implied by this packet.

## 7. Output

- Source commits on the feature branch, exact head SHA.
- `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-T03A-SAFE-BOOTSTRAP-AND-STAGING-RESULT-20261116.md` on the same branch: model/provider/effort, narrower allowlist, per-requirement implementation with file:line, ordering proof, negative results, harness counts, typecheck result, and an explicit "not verified" section.
- `[RUNNING]` and `[HANDED-OFF]` receipts on the T-03A issue, left OPEN for the Manager.
- No board or `active-tasks.md` writes, no Unity, no merge, no Windows build.
