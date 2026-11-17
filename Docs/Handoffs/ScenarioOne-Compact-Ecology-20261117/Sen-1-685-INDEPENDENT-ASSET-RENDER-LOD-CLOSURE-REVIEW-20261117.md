# Sen-1 #685 — Independent review of the asset/render/LOD provenance and world-blocker closure dossier

**Verdict: APPROVED — DOCUMENT / STATIC SOURCE-EVIDENCE QUALITY ONLY.**
**Severity: CRITICAL 0 / MAJOR 0 / MINOR 4 / NIT 2 / NOTE 4.**
This review closes no content, signs no world bytes, approves no production route and integrates nothing. Frozen #676 stays **SIGNED 0 / CANDIDATE 80 / BLOCKED 8**, cells **0 / 36 / 0**, `world_digest = null`. It remains **SIGNATURE-BLOCKED**.

## 1. Identity, authority and pinned target

| Item | Value |
| --- | --- |
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), medium effort, Claude Code via T3 Code, local Linux host. GitHub-API-only review. No subagents. |
| Independence | Different family from the author (OpenAI `gpt-6.1-sol`, Codex). I did not author, edit or advise on the target. |
| Registration | [RUNNING comment on #685](https://github.com/jermur788/CCF/issues/685#issuecomment-6956859331), posted before this file was written. |
| Starting main | `254d8b0eed446680e9c215bbf86959fea9437e9b` (same as the author base). |
| Reviewer branch / sole new file | `review/sen-1-685-asset-render-rights-independent-20261117-v1` / this file only. |
| Target | Commit `bb7a4c56dcfca6a31b6d25a3b6667d747c9ab11f`, the only commit on `docs/sen-1-685-asset-render-rights-content-evidence-20261117-v1`. Parent is `254d8b0e`; ahead 1, behind 0. It adds one file, blob `33eb5a04c86f7471695ee82c9a3d57368d04563e`, at `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-685-ASSET-RENDER-LOD-PROVENANCE-AND-WORLD-BLOCKER-CLOSURE-RESULT-20261117.md`. |
| Binding packet | [#685 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-675-676-verdict-685-render-rights-dispatch-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-685-RENDER-PROVENANCE-LOD-AND-BLOCKED-WORLD-CONTENT-EVIDENCE-PACKET-20261117.md), blob `1802dbdd9946ec4c784232a76133f41ea7c0c508`, read completely. |
| Live state read | `active-tasks.md` at main (#685 row: READY, author/reviewer paths match); #675, #676 (closed, HANDED-OFF), #684 and #685 comments. No other #685 reviewer was observed. The author has posted RUNNING on #685, but no handoff comment yet (NOTE-4). |
| Not touched | Local checkout, target branch, frozen #676, #684 paths, Assets, `.meta`, scenes, C#, saves, specs, the board and the Decision Log. |

## 2. What I independently reproduced (all PASS)

Every check below reads GitHub objects at `254d8b0e`, at frozen `8617587e` or at #676 review `16783336`.

1. **Appendix identity.** All 78 appendix rows (path to blob) match the `254d8b0e` recursive tree, which is not truncated: 78/78.
2. **Frozen trio.** Manifest blob `6c562f34…` is 308748 bytes, SHA256 `342e1afc…a677`. Companion blob `afd28d1b…` is 82840 bytes, SHA256 `2441926a…6123`. Dossier blob `fb02244b…` is 88048 bytes, SHA256 `f553fff0…0f26`. All three match the target exactly.
3. **Counts and digest.** `fields_after` is 0/80/8 and `cells_after` is 0/36/0. `world_digest` is `null`.
4. **Blocker crosswalk.** Exactly eight rows have `after_status: BLOCKED`: 10 (B-W02-RIGHTS), 12 (B-W02-SCALE), 30/44/73/74/75 (B-W13-RENDER-DEPS) and 83 (B-W14-WHOLE). The field names and blocker IDs match the target table word for word.
5. **Four families.** Frozen `/visual_assignment/asset_set` names the same four unpruned prefabs and GUIDs as the target (`6fc4c8b0…`, `c54cc46c…`, `b3a74f27…`, `69a172cf…`). Each prefab `.meta` GUID matches.
6. **LODGroups.** Each of the four prefabs has `screenRelativeHeight` 0.55 / 0.2 / 0.04.
7. **Manifests.** Unpruned LOD0–2 heights and triangle counts match exactly for all four families: Pole 8.000006675720215 m (10552/5198/1905); Young_02 10.500009536743164 m (7044/3640/1404); Mature_01 28.011241912841797 m (49888/25576/9042); Benchmark 26.0 m (44050/19690/6762). Benchmark `dbh_m` is 0.48 and `unity_tested` is `false`.
8. **Eleven materials.** Each `.mat` has `m_Shader {fileID:4800000, guid:933532a4fcc9baf4fa0491de14d08ed7, type:3}` on its `!u!21` object. Each also has `m_Script {fileID:11500000, guid:d0353a89b1f911e48b9e16bdc9f2e058, type:3}` on a separate `!u!114` object. The eleven material GUIDs, read from each `.mat.meta`, match the target table. The target correctly classes these as shader and script references, not material GUIDs. This is F-2's wording correction to the frozen B-W13 reason, which calls them "material GUIDs".
9. **Pipeline configuration.** ProjectVersion is `6000.6.0f1 (f7f8ed4d1e24)`. URP 17.6.0 and Input System 1.20.0 are in both manifest and lock. GraphicsSettings `m_CustomRenderPipeline: {fileID: 0}`. QualitySettings assigns Mobile `5e6cbd92…` and PC `4b83569d…`, which match `Mobile_RPAsset.asset.meta` and `PC_RPAsset.asset.meta`.
10. **Save and scaling source.** `TreeSaveData` has `ageYears`, `heightMeters`, `diameterCm`, `crownRadiusMeters`, `crownBaseHeightM`, the pruning fields and the mortality fields. `ForestTree.cs` `ApplyVisualOverride` starts at line 398, and the uniform `Vector3.one * (targetHeight / polishedNaturalHeight)` is at line 459. `SetTrunkShape` starts at line 671; the target cites lines 671–678. `DisplayStage` uses 3.5 m and `poleVisualMaxHeightM` (default 12).
11. **Rights record.** `RightsAudit.md` (blob `2e9011a2…`) scopes itself to the private compiled candidate `CCF-S1-20261009-B06-b3ba9b8`. It records the 2026-10-09 owner confirmation and excludes standalone asset/source distribution. No root licence exists in the tree; the only notice path is `Tools/Build/S1B/Notices`. The CropTreePruning README (blob `4775c146…`) supports each claim the target makes about it: prototype art, derived from existing geometry, three LODs, metre scale, no animation or colliders exported.
12. **Drift check.** Of the 333 `git_object` dependencies, 52 overlap the appendix and all 52 are blob-identical. Of the other 22 that differ from main, 3 are base-authority documents that later commits changed: the decision log, current milestone and active-tasks. The other 19 exist only on their own pinned branch commits: specs and handoffs. None of the 22 is an Asset or C# file. The 14 `raw_local_file` dependencies are off-Git, as the target says.
13. **F-1, F-3 and the notes.** The F-1 native values (0.9798264503479004 / 0.0201735682785511 / strict 0.020173566415905952), cells 7/8/15 and the 2e-6 tolerance match #676 review §F-1. The F-3 values (+1.599 m⁻¹, 0.625 m, C1 not C2) match §F-3. All eight attributed margins (O1/O2/B1/B2 core and verge, P1507 0.47274058646982764 / 0.2727405864698276) match frozen manifest values to the printed precision. The 13 N-2 receipt IDs equal `/owner_decision_receipts` exactly. `ScenarioSiteProfiles.TryGet` is at line 30.
14. **Scope.** The target makes no licence inference, native PASS, world digest, scene creation, wire field, frozen edit or production dispatch. Row 83 is kept dependent. The proposed `ScenarioOneCompact30.unity` is correctly called absent. Its NOT RUN list covers the whole packet §4 list.

## 3. Findings

### M-1 — MINOR — Row 12: source evidence for "hidden scale distortion" is incomplete

The conclusion that the current routes cannot independently express DBH and crown is correct. But the evidence leaves out three static facts:

- **Prefab normalisation.** Each unpruned prefab rescales its nested FBX instances to roughly 1 m tall, with x = y = z. Pole_01 has 9 × `m_LocalScale` = 0.125 (1/8). Benchmark has 9 × 0.03846154 (1/26). Each `LODGroup` has `m_Size` ≈ 1.0. This is positive evidence of **no** hidden non-uniform distortion inside the prefab. It also means `polishedNaturalHeight` is about 1 m, not the manifest height. Future native tests and LOD-threshold reasoning need this fact.
- **Plantation route.** `ModelForHeight` (ForestTree.cs:340–349) sends trees below 20 m to the plantation catalog when `usePlantationVisuals` is set. That route scales through `PlantationTreeVisual.Apply` (PlantationTreeVisual.cs:35–39), which is also uniform (`height / Base.authoredHeight`).
- **Serialized threshold.** `poleVisualMaxHeightM` is a serialized field (`[Min(3f)]`, default 12). The 12 m switch is a default that scenes can override, not a constant.

**Requested:** cite all three facts in the next revision or the row-12 test plan. None of them changes the B-W02-SCALE status.

### M-2 — MINOR — The core-first stand-in proposal omits the existing primitive route, and below Mature that route is not dimensionally honest

The packet asks whether built-in primitive stand-ins "can truthfully carry stage". The target proposes a new route but does not say that `ForestTree.cs` already has one:

- **Mature.** `MatureThickness = diameterCm/100` (line 278), and `MatureCanopyScale = (2R, 2.2R, 2R)` (line 280). Trunk diameter and crown radius are already driven independently.
- **Sapling and Young.** `ApplyStageShape` (lines 599–618) and `CurrentHeight` (lines 264–274) use **fixed** heights of 1.1 m and min(2.7, H) m. Trunk thickness is 0.45·D and 0.7·D. The canopy is a fixed (1.1, 1.2, 1.1) for Sapling, and 0.6 × Mature scale for Young.
- **Crown depth.** Canopy vertical extent is 2.2R, tied to crown radius rather than to `crownBaseHeightM`.

So reusing today's primitives would misstate height and DBH for every Sapling and Young tree. That includes any initial 0.6 m Oak/Beech shown on this route.

**Requested:** state which existing behaviours a Compact-only route must replace: fixed stage heights, stage DBH multipliers, and crown depth tied to radius. Then the proposal will not read as "already available by reuse".

### M-3 — MINOR — The material dependency matrix stops at the eleven tree materials; the GUID pair is used project-wide and Route B does not avoid it

- **Static count.** 93 of the 94 tracked `.mat` files at `254d8b0e` reference shader GUID `933532a4…`. That includes the protected E80 `Forest Ground.mat` and `Forest Path.mat`. All 94 reference script GUID `d0353a89…`. The single exception is `HarvestMark.mat`, which uses shader `650dd9526735d5b46b79224bc6e94025`.
- **Route B.** Frozen `/terrain/surface` and `/H1/surface` name shader "Universal Render Pipeline/Lit". A texture-free URP Lit material therefore depends on the same unauthenticated package implementation. Route B reduces the art and texture inputs. It does **not** clear B-W13-RENDER-DEPS. The target should say so plainly.
- **Package source.** `packages-lock.json` records URP 17.6.0 with `"source": "builtin"`. The future custody/lease request should name where the Editor installation provides built-in packages, as well as any project package cache. I have not verified that location.
- **Earlier native use.** The same GUIDs rendered in earlier accepted native evidence for the E80 scene (D-043/D-045). That is supporting context. It is **not** authentication of the package bytes, and I make no such claim.

**Requested:** add the project-wide count and the Route-B dependency sentence to the matrix.

### M-4 — MINOR — The packet-mandated CropTreePruning README is not pinned

Packet §2 names `Assets/ForestPrototype/Art/CropTreePruning/Docs/README.md` and requires exact pinned Git refs and paths. The target cites the README's content, and that content is accurate. But the README does not appear in the appendix or in any link. **Requested:** add it at `254d8b0e`, blob `4775c14680ab3f2024d9b5992a5d69cef9ea8907`. `Asset_Manifest.json` and `Coverage.md`, which the README itself cites, are optional additions.

### NIT-1 — N-6 field name

N-6 calls the field `browsePressure`. The source field is `backgroundBrowsePressure` (`ScenarioOneDefinition.cs:58`), as #676 review N-6 records.

### NIT-2 — The appendix does not list meta GUIDs

The target says "The appendix records inspected unpruned meta GUIDs". The appendix lists paths and blob IDs only. The FBX `.meta` GUIDs, such as Benchmark LOD0 `5bf72954…`, are not listed. Either add them or reword the sentence.

### NOTE-1 — F-3 chose the owner-choice branch

Packet §3 allows this ("or an explicit owner-choice"). It is acceptable, and the target correctly declines to carry margins forward. The owner page could label the #676 reviewer's length-weighted tangent example as one candidate for the curve-revision option, still with no margins claimed.

### NOTE-2 — Configured default quality is visible in source

QualitySettings has `m_CurrentQuality: 1`, which selects PC and `PC_RPAsset` `4b83569d…`. Effective per-platform and runtime quality remains NOT VERIFIED, as the target says.

### NOTE-3 — Root licence

There is no root `LICENSE`. The scoped B06 audit covers private compiled builds only. I agree with the target that neither file presence nor the README establishes Compact rights.

### NOTE-4 — Author handoff comment

At review time #685 had the author's RUNNING comment but no author handoff comment. The parent or Manager should record the author handoff along with this verdict.

## 4. Packet coverage check

| Packet requirement | Status |
| --- | --- |
| Identity and registration before edit; immutable base; sole file | PASS |
| Seven rows plus dependent row 83, with blocker IDs preserved | PASS (reproduced) |
| Rights: what establishes them and what is missing; no licence inferred | PASS. M-4 is traceability only. |
| Bounded option (a) reuse and option (b) neutral, both conditional | PASS. M-3: B does not clear the shader dependency. |
| All-LOD fidelity, measurable test matrix, native NOT RUN | PASS with M-1 and M-2 |
| Shader vs script vs material GUID types; package/pipeline matrix; cache not claimed | PASS with M-3 |
| Native tests: 720p/1080p, guard states, 1.2 m core + ≤0.2 m verge, 0.6 m Oak/Beech | PASS, all labelled NOT RUN |
| Row 83: exact inputs, order, no phantom scene/digest/wire field | PASS |
| F-1, F-2, F-3 and N-1 to N-6 | PASS. NIT-1 is on N-6. |
| No new empirical forestry/economic claims; tolerances labelled as proposals | PASS |
| Frozen #676 untouched; future reauthoring needs full exact-blob review | PASS |

## 5. Remaining gates (unchanged by this review)

1. **Rights.** The owner nominates exact rights for the four families, textures and source, including compiled versus raw use. The owner also nominates rights for any Compact material or guard, and the storage treatment under D-032 (still Open) and OD-13.
2. **Route choice.** The owner or Manager chooses Route A (reuse) or a limited Route B neutral production packet. Either needs separate independent production approval.
3. **Custody and lease.** An authorised physical operator lane is needed for the URP shader/script package bytes and notices, with package-cache or built-in custody and an Ops/Unity lease. #652 HELD must not be bypassed.
4. **Native witnesses.** Import, shader binding and active pipeline readback; all-LOD dimension, transition and collider checks; 720p/1080p visuals; guard states; path, capsule and navigation H1→A8; save isolation; 36-cell effective readback at the exact candidate.
5. **F-3.** The owner decides on A7 naturalness: keep the 0.625 m bend for native visual assessment, or authorise a curve revision with every margin recomputed.
6. **Reauthoring.** A new complete world candidate needs an explicit canonical schema, exact dependency hashes and a **new complete exact-blob independent review**.
7. **Digest and attestation.** The world digest is computed only after every input exists and has been reviewed. The owner's personal exact-byte attestation follows. The #677 save-policy signature does not attest content.
8. **Next author revision.** Fold M-1 to M-4 and NIT-1/NIT-2 into that revision. Frozen #676 stays unedited.

## 6. NOT RUN by this reviewer

Unity Editor/IL2CPP, import or compile, shader binding, render, LOD or collider runs, capsule navigation, native 36-cell readback, Save/Load, tests, FBX decoding, package-cache or Editor built-in package inspection, any rights or licence audit of external files, physical Ops or lease, Windows build, dashboard and Drive mirrors. I did not inspect the off-Git `raw_local_file` dependencies. The F-1 and F-3 values are checked only against prior review text and frozen manifest values; I did not recompute them.
