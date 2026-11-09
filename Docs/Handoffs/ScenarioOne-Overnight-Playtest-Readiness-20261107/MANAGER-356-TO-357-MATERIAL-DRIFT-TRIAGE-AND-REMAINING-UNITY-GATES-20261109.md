# Manager #200 → #357 — Exact Section Five material-drift triage and remaining Scenario One Unity gates

**Date:** 9 November 2026. **Manager verdict on #356:** bounded badge lifecycle **APPROVED at source and observed real-Unity startup scope**; full integrated qualification **NEEDS CHANGES / NOT READY FOR #316**.  
**New execution claim:** [#357](https://github.com/jermur788/CCF/issues/357) **READY / NOT RUNNING**, no present writer/Editor/lease established by this packet.  
**Binding evidence:** [#356 exact report](https://github.com/jermur788/CCF/blob/a3bba8e189e6d9fddb6a7e1f3edf7f82fddd91f4/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/FOCUSED-SCENARIO-ONE-UI-LIFECYCLE-REQUALIFICATION-RESULT-20261109.md). [A–O matrix](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/FOCUSED-PLAYTEST-QUALIFICATION-BROWSER-MATRIX-20261106.md).

## 1. Verified source scope and honest current evidence

GitHub comparison confirms #356 source commit `045966d2f8e26368e30a79f14a1e34167952b95e` changed **exactly three files** relative to #315 report tip: `ScenarioOneUiRoot.cs` (+2/-1), integrated headland-pinning script and its two extra negative controls. Its `a3bba8e189e6d9fddb6a7e1f3edf7f82fddd91f4` successor changed only the #356 Markdown handoff. The repair constructs `nextBadge` at the start of `Start()`, not in the MonoBehaviour initializer; no CTA selection source changed. Worker-reported offline: CTA 10/0/2, headland 49 source + 92 C#, integrated 10/10 negative mutations, historic stage 20/20.

Reported real Unity 6000.6.0f1 on exact `045966d2`: 0 runtime VisualElementCreation / NullReferenceException / ClearNextAction / C# error (the earlier 34 NREs gone); rendered `Next:` badge; Workbench 107/107 internal Editor behavioural checks PASS but **strict Workbench runner FINAL exit1 FAIL** solely on identical two-hunk `.vscode/settings.json` drift; Objective 1 native path `RACKV2_O1E2E_PASS checks=173 shots=24`, including normal startup, five racks, entrance marks, Work Plan approval, annual review, save/load. It produced 24 genuine colour captures at 720p and 1080p. These are **scoped observations**; the full map geometry, Full vs Mini, old-world guard, material-dependent rendering, grayscale, card fit, performance, Legacy40 and frozen Reference Future v1 require remaining checks. **A–O overall incomplete; not yet qualified.**

#356's second Editor invocation also left five dirty tracked Section Five `.mat` files, plus the same `.vscode` settings change. Worker reports those five outputs are byte-identical to the *protected original Workbench's dirty material files* and discloses actual YAML mutations including `_MainTex` texture references and `_AlphaToMask` 0→1. Manager has **not independently inspected those uncommitted local bytes**, and identity with another dirty lane **does not prove visual equivalence or benign semantics**. Worker correctly stopped without restoring them. Worker-reported global lease `570114e72b784531953b6d34623507c0` released normally, status free/no Editor, ZX20 36.14 GiB free with 20-sec monitoring/no alerts, 27 licensed ignored files, saves, original eight-dirty Workbench all preserved. Refresh everything locally; these are prior observations, not current admission.

## 2. Binding material policy: exact restore permitted, equivalence not pre-approved

**Decision:** Authorise **one narrowly controlled worktree-only restoration** of the *five exact known material paths* and the existing exact two-hunk `.vscode/settings.json` in **U3 alone**, to the **unchanged currently checked-out candidate HEAD**, after a full evidence-preserving and byte-exact preflight. This grants an operational route to test further **without** canonically accepting generated material edits, weakening the strict runner, or deleting preserved dirty work.

The five material basenames, and only these (under the verified Section Five Art folder), are:

1. `Bilberry_Leaves.mat`
2. `Bilberry_Stems.mat`
3. `Forb_Flower_Centre.mat`
4. `Forb_Leaves.mat`
5. `Forb_Stems.mat`

**Before any restore,** in read-only mode, confirm **all** of:
- Correct registered U3 path, canonical common store, exact source HEAD `045966d2f8e26368e30a79f14a1e34167952b95e`, branch `qualify/scenario-one-ui-lifecycle-20261109-v2`, source branch `integration/scenario-one-focused-playtest-20261109-v1` report tip `a3bba8e1...`, no other owner, no Editor/lease.
- `git status --porcelain=v1 -uall` contains exactly the above five full paths and `.vscode/settings.json` as modified, **nothing staged, deleted, renamed or untracked**. Determine exact full path by actual Git, not a guessed directory; no wildcard/path-prefix permission. Inspect ignored items and valuable project files separately, do not include them as dirty cleanup candidates.
- Preserve the complete **before-image bytes**, index state, per-path SHA-256 and Git blob identity, full binary/text diff against HEAD, original previous #356 drift receipts, and the Workbench protected material dirty SHA-256 identities in an immutable new Files evidence directory. Comparison of five U3 after-image bytes to prior Workbench dirty copies must be exact *per file*, with verified source identity; never modify the original Workbench. Compare `.vscode` SHA-256 to the known `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb` and its exact known two-hunk diff.
- Classify each material **property-level difference** (e.g. `_MainTex` GUID, `_AlphaToMask`, shader/render queue, keywords, texture property). Resolve whether the changed GUIDs are actual known project assets and capture the before/after material state. The presence of any unrecognized GUID, unlisted property/value difference, a changed shader, unexplained side effect, uncertain origin, user-authored edit, extra dirty path or failed hash match is a **hard STOP without restore** and a Manager return.
- Since `_MainTex` and `_AlphaToMask` can affect **visual output**, no claim of benign/environmental *rendering equivalence* is pre-approved, even if all five match previous dirty Workbench bytes. Record both committed and rewritten states, and label any previous #356 screenshots potentially **post-Editor-material-state** until the actual material visibility can be assessed.

Only when every check above succeeds, **restore these six exact named tracked paths to the same existing U3 HEAD in the worktree only** after saving their originals: never index reset/stash/checkout of other paths, `git clean`, `update-index --skip-worktree`, assume-unchanged, modify ignores or package/material commits. Recheck strict-clean, U3 ignored 27-file licensed asset hashes, frozen reference archive, old Workbench eight dirty paths, scene, packages, saved games. If ANY byte differs from the preflight classification, stop. A future per-Editor-invocation restore while a valid lease is held must repeat the same exact 6-path allowlist/known hashes and evidence capture; it is never automatic nor part of a runner's PASS logic.

**Runner/provenance decision unchanged:** Original Workbench `run_workbench_retirement.py` **must not be edited, bypassed, silenced or allowed to call an exit-1 run PASS**. The earlier strict runner recorded `postrun_lane_drift:1` on `.vscode` and remains a FAIL. Any additional runner reporting material drift also remains FAIL in its raw receipt. A separately documented `KNOWN REPRODUCIBLE EDITOR MATERIAL REWRITE — VISUAL SIGNIFICANCE UNDER REVIEW` may permit **continuing independent tests** on the exact unchanged source after safe restoration; it does not clear Workbench Tier-2 acceptance or transform material-affected visuals into verified screenshots. No canonical change to original material bytes or source SHA.

## 3. One local task, no redundant tests — exact lane and source

**Difficulty HIGH** because dirty tracked materials have potentially substantive rendering impact plus one exclusive Unity system lease and saved-world/reference obligations. **Best-suited local T3 Code Unity/Git investigator**, GPT-6.1 Sol High / capable Claude Opus or Sonnet High; record actual model, provider, effort. No new code or visual design allowed. One owner.

The source has already been Unity-compiled and verified at its own scope, so **reuse exact HEAD and existing warm U3**:
- Existing U3: `/media/jer/ZX20/CCF-U3`
- Existing local branch: `qualify/scenario-one-ui-lifecycle-20261109-v2`
- **Exact Unity candidate HEAD:** `045966d2f8e26368e30a79f14a1e34167952b95e`. Do **not** move to the report-only successor, rerun source staging, create a new branch, merge, change game C# or packages.
- Existing Files report/source lane remains intact/read-only until final Markdown publication: `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, branch `integration/scenario-one-focused-playtest-20261109-v1`, report tip `a3bba8e189e6d9fddb6a7e1f3edf7f82fddd91f4`. If changed/unowned state uncertain, stop, no rebase.
- Mandatory registration, owner, actual HEAD/branch, Git status/index/ignored, permitted six changes only, licence, 27-pack, 35 GiB free ZX20, quiet I/O, Files evidence capacity, warmed Library, original eight-dirty Workbench, old user saves and global machine `unity-lock` free with no Editor process. Run a real <=20 sec disk monitor with 28 GiB warn, 25 GiB controlled owner-stop, 20 GiB absolute stop, 15 GiB floor; do not create another Library/cold import.
- Before actual substantive local work post task #357 `[RUNNING]` with actual provider/model/effort, branch/worktree/starting source HEAD and worktree six-path before-image inventory. Manager alone edits `Docs/Project/coordination/active-tasks.md`.

If material triage passes and exact six-path restoration yields strictly clean U3, acquire **one new** exclusive lock using the actual ID returned and verify owner/task/lane/head/branch metadata. Persist the correct ID to file and verify owner-safe final release; never use a source-prefix as the lock ID or force another owner's process.

## 4. Remaining Unity A–O — actual evidence rather than scope inflation

Run only the **unverified remainder**, starting with low-risk, value-dense qualification while keeping source/lease/branch fixed. **Do not rerun** the 107 Workbench assertions, 173 Objective 1 end-to-end controls, #355/#356 CTA, 884/112 source campaign, historical stage-pinned five-rack checker, 20/10 mutation controls or multiple B3 plain loops **unless a new causal fault requires a narrowly selected replay**. Carry their **same-exact-source** antecedent results with exact limited status (in-Editor PASS versus runner FAIL distinct).

Focus on:
- actual Enlarged80 and RackV2 persistence/annual, Legacy40, frozen Reference Future v1, historical saved-world/site masks, native Reference Preview return pose — full protected archive/hash evidence;
- V2 headland/feeder physical 3D vs Full Stand Map, Mini invariance, legend/path distinction, five-rack illustrative geometry and absence in Legacy40/pre-V2/Reference worlds;
- Objective card fit/no-scroll at 1280×720 and 1920×1080, native safe Help/focus/Next button, colour and **grayscale** captures, Work Plan commitment versus planned racks/Fell marks, MFM boundary, bounded performance/GC/scene lifecycle.

**Material-sensitive visual rule:** Since Editor changes can reassign texture or alpha clipping properties, capture the exact material states **at each visual observation** (pre-Editor HEAD hashes, post-Editor hashes, shader/texture identities) and directly document visible effects with native captures. A test rendered after an uncommitted material rewrite is still an authentic observation of that Unity session, **but its equivalence to the committed candidate is NOT ASSUMED**. Mark any unresolved affected *visual correctness* item **NOT VERIFIED / NEEDS CHANGES** for independent #316 and Manager unless a before/after visual comparison or otherwise credible direct rendering/asset evidence resolves it. Do not quietly claim all A–O PASS based on a green runtime marker alone. If unexpected texture disappearance, clipping/opacity change or unapproved material mapping is observed, **STOP** and hand back instead of repairing gameplay/materials under this task.

**Continue after expected exact material rewrite** only when the six-path condition is still satisfied, no game/Editor C# error or actual blocker, and operator captures raw log/diff/hash. If a subsequent strict-clean runner requires a clean index/worktree, this packet permits exactly one manual evidenced six-path worktree-only restore before the next run, not inside the runner and never making the prior runner PASS. Stop on any extra/changed path, wrong hash, disappearance of licensed assets, licence/lock/disk problem or material uncertainty. Maximise already tested same-source evidence; no unbounded retest loops.

## 5. Handoff and verdict discipline

Publish precisely one permanent Markdown result after lease is properly released:

`Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/FOCUSED-SCENARIO-ONE-MATERIAL-DRIFT-AND-REMAINING-QUALIFICATION-20261109.md`

on existing clean Files staging branch (report-only successor of `a3bba8e1...`), if safe. Report:
- six-path *exact* before/after/full hashes, property-level YAML semantics, original dirty Workbench match (without touching it), visual impact classification, and each worktree-only restore;
- exact Editor-tested source HEAD/branch and untouched source/scene/packages/archives, real lease true ID/acquire and owner-safe release/free state, 20-second monitor/min disk, licence/27-pack, original Workbench, user saves;
- A–O every gate **PASS / FAIL / NOT RUN / NOT VERIFIED** with real screenshot/log refs, specifically distinguishing inherited 107/107 Editor Workbench PASS but *strict runner exit1 FAIL*, inherited 173 Objective1 native PASS, and any new material-sensitive visual caveat;
- first actual blocking fault (if any); whether native, Legacy40, Reference v1, save, geometry, card fit, input, performance evidence is qualified; unresolved observations;
- final **READY FOR MANAGER / INDEPENDENT SOURCE-EXACT INTEGRATION REVIEW** only if all mandatory gates are genuinely supported and no unresolved rendering blocker, else **NEEDS CHANGES / BLOCKED**. Explicitly say that #316 must not start automatically and #287 full qualification remains Manager-held.

Post #357 [HANDED-OFF] with direct durable result URL and close if published, even if result BLOCKED; otherwise [BLOCKED] with truthful reason. **Return to Manager #200.** No `main` merge, no human playtest release, no Windows/Linux Player build; user authorization for Windows remains unmet.

## Paste-ready local dispatch

> T3 Code: Execute [CCF task #357](https://github.com/jermur788/CCF/issues/357) under the [binding Manager material-drift triage and remaining Unity qualification packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MANAGER-356-TO-357-MATERIAL-DRIFT-TRIAGE-AND-REMAINING-UNITY-GATES-20261109.md). First inspect the exact six dirty tracked U3 files read-only, preserve complete before-images and hashes, verify five Section Five materials match the protected prior Workbench dirty evidence and document the actual texture/alpha-to-mask YAML effects; do not presume rendering equivalence. If all strict path/hash/owner checks pass, restore only those six specific files to the unchanged U3 HEAD in the worktree, prove clean and acquire one fresh real exclusive Unity lease after 35GiB/licence/27 licensed files/old saves/monitor checks. On exact source `045966d2` run ONLY missing A–O gates (Legacy40, frozen Reference v1, persistence, Full/Mini/3D, old-world five-rack/card fit, native/help/focus/colour+grayscale, performance), reusing prior 107/107 Editor and 173 Objective1 tests at their exact scoped statuses. The Workbench runner's raw exit1 is still FAIL. For any five-material rewrite preserve and compare exact source-vs-runtime material state and label visual gates NOT VERIFIED until direct evidence resolves it; stop first genuine blocker. Release owner-safe, publish named GitHub result, close #357 and return to Manager. No source edits, broad reruns, main merge, #316 or build.

**User action required:** paste one dispatch into local T3 Code. No product decision or Windows-build authorisation is requested.
