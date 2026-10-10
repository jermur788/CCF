# S1-B INTEGRATION + HIDDEN CONSTRUCTION FRAMES — INTEGRATION HANDOFF

Integrator: Claude (Sonnet 5.5). Date: 2026-10-10 (Europe/Dublin). Verified in `/home/jer/CCF-s1a` on Unity 6000.6.0f1.

```text
INTEGRATION DECISION
Status: APPROVED (Manager-approved S1-B source integrated; one verification gate remains OPEN and is not attributable to the change)
Reviewed branch: task/s1-b-standalone-readiness
Reviewed HEAD: 4359e500c6b312ebcf957397b4fe53205f3f1ca4
Context commit (task base): 4a68d528bb9d01036de9d4df1739b7de2c8c6002
Reasons:
- Approved S1-B source integrated by fast-forward of BASE; production Assets/Packages/ProjectSettings bytes identical to the tested candidate b3ba9b8.
- Manager amendment applied as a separate commit; B06's pre-correction presentation is superseded and was not modified.
- All required gates pass except P2 interactive (see OPEN GATE).
Required fixes / remaining gates:
- OPEN: P2 interactive rendered layout (environment-dependent, unidentified cause).
- Native Windows technical smoke on a pilot candidate (none exists).
- Stand expansion to >= 80 x 80 m (D-056) before any pilot candidate.
Integration target: main (origin/main)
Integrated HEAD: the publication commit containing this handoff; its full SHA is reported with the push receipt (a commit cannot name itself).
Post-integration verification: below.
```

## What is integrated (kept distinct)

| Item | SHA |
|---|---|
| BASE / task context | `4a68d528bb9d01036de9d4df1739b7de2c8c6002` |
| Approved S1-B source (7 commits, fast-forward, history intact) | `4359e500c6b312ebcf957397b4fe53205f3f1ca4` |
| Tested candidate B06 source (code identical to approved tip) | `b3ba9b8900b1f62b721aee6508aba6d64db93bfd` |
| Building-frame correction (separate commit, D-055) | `fac782072f23e28518e8840c621112fe4074db45` |
| Canonical context commit (D-055, D-056, milestone, overview) | `8bed3996aefd1780c62744b648094efe5b394feb` |
| Evidence/handoff + coordination commits | the publication tip (see receipt) |

Method: `integration/s1-b-standalone-readiness` was created at BASE and the approved tip fast-forwarded onto it (no merge commit). The dirty `/home/jer/CCF-main` checkout was not used: its HEAD (`402a2b4`, 43 commits behind), status and all 11 dirty/untracked file hashes are identical before and after this task. No stash, reset or clean was run.

## Amendment 1 — unavailable building/construction frames hidden (D-055)

**Premise check (reported to and decided by the user).** The amendment said CCF has no player building options. The code did have one: `ForestBuildable` offered `[E] Build X (N Wood)` at seven sites 4–8 m from the player start, spending carried, stored or retained timber; it is saved in `data.buildables` and exercised by `ScenarioOneInteractionVerification`. The user chose "hide frames + disable the build action".

**Audit before the change** (`BuildingFrames/audit-before.txt`): seven `ForestBuildable` frame sets (Grinding Stone, Log Rack, Log Rack II, Saw Pit, Basic Shelter, Timber Sledge, Forestry Workbench) plus the Plank Rack's static foundation marker (Corner Post A/B, Foundation Marker); 7/7 sites offered the build prompt; a real `TryBuild` with exactly enough carried wood constructed the Workbench. The sites also carried solid root colliders (e.g. Saw Pit 3.4 x 1 x 2.2 m), so hiding visuals alone would have left invisible walls.

**Change** (`fac7820`, 3 files, +27/−4):

- `ForestBuildable.cs`: new `[SerializeField] bool constructionAvailable = false`. While off and unbuilt: unbuilt visual hidden, the site's colliders disabled, no raycast/prompt, `TryBuild` returns. `RestoreBuiltState(true)` still shows a structure built in an older save. Scene instances serialise no new value, so no scene change was needed for the seven sites.
- `Assets/Scenes/ForestTest.unity`: exactly two single-line edits for the Plank Rack, which is not a `ForestBuildable` and which nothing re-enables: its "Unbuilt Site" `m_IsActive 1→0` (go `2120840061`) and its root BoxCollider `m_Enabled 1→0` (fid `1217151875`). Diff audited: `+2/−2`, block-bounded, LF preserved, no reserialisation.
- `ScenarioOneInteractionVerification.cs`: asserts default-off (`TryBuild` neither builds nor spends timber), then enables the flag by reflection so the exact retained-timber debit check still runs.

**Audit after** (`BuildingFrames/audit-after.txt`, `active-renderers-diff.txt`): 0 frame renderers, 0 solid site colliders, 0 orphan frame-like renderers, 0 build prompts at any site, `TryBuild` no-op (carried wood unchanged). Active renderers 3875 → 3840: exactly 35 removed, **all under "Unbuilt Site"**, 0 added. 336 trees, shelters and the HUD are unchanged. Rendered before/after: `BuildingFrames/{before,after}-wide-from-start.jpg`, `…-site-timber-sledge-01.jpg`, `…-site-shelter-01.jpg`. After the change the start view is a clean clearing with the stand, ferns and HUD intact. The change introduced no compile error and no new missing-reference message: the only one in any gate log is "Prefab instance problem: UNS_Mushroom_Patch (Missing Prefab …)" for 16 instances of an Ultimate Nature pack asset that is absent from `/home/jer/CCF-s1a`; it appears identically in the before and after audits and in every other log, and is a property of this pack-less worktree, not of the change.

Retained and unchanged: source prefabs, built visuals, `ForestBuildable` build code, saves (`buildables` list, `built=false`), `ForestSceneBuilder` (Editor; re-running it would re-author the frames), tree shelters and planting markers. Known side effects: the hidden sites still reserve their footprint from starting-stand placement and the automatic planting search (unchanged behaviour); KeepForUse retained construction timber has no use for now.

## Amendment 2 — stand size (D-056) and pilot build status

STAND EXPANSION STATUS: **Accepted for next task / not implemented.** The 40 x 40 m stand is untouched. Reference Future v1 was not resized, regenerated, migrated or reinterpreted (archive blobs and anchors unchanged).

PILOT BUILD STATUS:

- **B06 — historical evidence only.** `CCF-S1-20261009-B06-b3ba9b8`, source `b3ba9b8900b1f62b721aee6508aba6d64db93bfd`, archive SHA-256 `c7d4da9d5316c3bb419b39f16e84827bc1a41ab07c75ade01316d063a1d22188`. Not modified, deleted or moved. Pre-correction and pre-expansion: **do not send to testers.**
- **B07: not started and not built.** The corrected B07 replacement was intentionally deferred per the amendment; the external pilot candidate is deferred until the >= 80 x 80 m stand is implemented and accepted. No Windows build, HDR quarantine or Wine run was performed in this task.

## Verification at `fac782072f23e28518e8840c621112fe4074db45`

Exact records: `results.json`, `regression-results.json`, `GateRecords/`. Raw logs/PNGs stay in ignored `Build/` with SHA-256s in `evidence-sha256.json`.

| Gate | Result |
|---|---|
| Unity import/compile | PASS, 0 compiler errors in every gate log |
| SessionMenuVerification | PASS |
| Teaching Copy batch ×2 + rendered | PASS, read-only hash `EBE7A228F630AE41` identical ×3 |
| MenuTutorialVerification ×2 / Clearance / Removal | PASS / PASS / PASS |
| P2 focused, batch ×2 | PASS, `F58FB0B1A421D28B` ×2 |
| P2 interactive (rendered layout) | **FAIL — OPEN GATE** (below) |
| P3 focused, batch ×2 + interactive | PASS, `F7C2FC966816BBAD` ×3 |
| Full current regression | **24/24 PASS** |
| Model2 completion | `702766DECE591E21` present |
| Reference Y100 / continuation | `7AD177B3CC2F73C7` / `9CDF21A541C5968D` present |
| Established modern anchors | every exercised 16-digit hash unchanged against the committed modern baseline (no gate differs) |
| `ScenarioOneInteractionVerification` | PASS including the new default-off and latent-mechanic assertions |

Save 19 and rng1/regen2/growth1/storm0 are unchanged; no ecology, economy, objective, completion or forestry-mechanic change.

Note on stale files: `Build/S1A` was reused from the S1-A worker. `menu-standalone-2.before-ground-fixture.json` (FAIL, 2026-10-09 19:26) is that worker's earlier fixture iteration, not part of this run; only files dated 2026-10-10 00:49–00:54 are this run's targeted records.

## OPEN GATE — P2 interactive rendered layout

`CropTreeCompetitorVerification --interactive` fails: `a world label is drawn under a panel at 1280` (competitor tag 1 sits on the top edge of the competitors panel at 1280 x 720). All simulation checks inside the same run pass and the determinism hash is correct.

| Run | Tree | Environment | Result |
|---|---|---|---|
| S1-B worker, 2026-10-09 22:27 | `85e3a80` (same production bytes as `4359e50`) | S1-B checkout, pack present | PASS |
| Integrator, 4 runs | `4359e50` | `/home/jer/CCF-s1a`, pack absent | FAIL ×4 (one original + 3 repeats) |
| Integrator A/B | `c01de2d` (S1-A integrated, **no S1-B code**) | `/home/jer/CCF-s1a` | FAIL |
| Integrator | `fac7820` | `/home/jer/CCF-s1b`, pack present | FAIL |

The failing screenshots at `c01de2d`, `4359e50` and `fac7820` are **pixel-identical** (0 differing pixels), in both checkouts. So the result is not caused by S1-B, not caused by the frame correction and not caused by the Ultimate Nature pack (my first hypothesis, disproved). It differs from the worker's passing capture in a slightly different camera pose, visible as shifted rings, foliage and tag 1 (`P2InteractiveInvestigation/diff-worker-pass-vs-now-fail.png`). Display, GPU and Unity version lines are identical to the passing run. **Root cause not identified.** The gate is not claimed passed; a follow-up should log the fixture's camera pose and decide whether the margin-based assertion needs a tolerance (a P2 harness decision, not made here).

## Native Windows / Wine boundary (unchanged)

Native Windows technical smoke remains **open**: readable text and build ID, real keyboard/mouse, walking, save/load, first-cycle usability, performance and stability. Wine is supplementary only and showed no readable UI text (LegacyRuntime font-face errors). Stage A remains **CONDITIONAL / not ready**.

## Context and publication

CONTEXT COMMIT: `8bed3996aefd1780c62744b648094efe5b394feb`. All 12 mirrors, both composites and the manifest use this one SHA; generation refused dirty canonical sources, stamps were verified on all 12 mirrors, and regeneration is byte-identical. Local bundle: ignored `Build/ProjectContext/` (15 files).

DRIVE / PROJECT ATTACHMENTS: **not claimed current.** The permanent Drive mirrors are plain `text/markdown` files updated in place by ID last time; the available Drive connector can create, rename and move files but cannot replace the contents of an existing file, so an in-place update was not performed at the time of this commit. Any later publication and read-back is recorded in `context-publication.json` if and when it happens. Older Drive/platform copies are older snapshots, not current.

Live coordination (`Docs/Project/coordination/active-tasks.md`) is updated in a separate coordination-only commit that does not advance the context commit.

## Unrelated work preserved

`/home/jer/CCF-main` (dirty, 43 behind): untouched, comparison above. `/home/jer/CCF-s1a`: used as the clean integration worktree (its S1-A task branch is unchanged at `c01de2d`). `/home/jer/CCF-s1b`: detached to `fac7820` for one gate and returned to `task/s1-b-standalone-readiness` at `4359e50`; its local P2 logs were backed up and restored, its demo HDR files were quarantined and restored by the approved wrapper with SHA-256 `8d19cc94…0fb88d` / `5b06744c…dd99` verified afterwards; Editor-normalised files were reverted after each run (diffs recorded in ignored `Build/S1B-Integration/*.diff`). No disposable harness or generated `.meta` remains under `Assets/`.

## Next recommended task

Dedicated Scenario One >= 80 x 80 m stand-expansion implementation packet, issued from the final clean integration/context SHA. It must inspect before writing and must not multiply values by four. Then full regression with spatial/performance validation, a new Windows pilot candidate from the enlarged committed source, the Sonnet 5.5 xhigh pre-pilot review of that candidate, native Windows technical smoke, and only then the beginner Stage-A pilot. Separately: investigate the P2 interactive drift.
