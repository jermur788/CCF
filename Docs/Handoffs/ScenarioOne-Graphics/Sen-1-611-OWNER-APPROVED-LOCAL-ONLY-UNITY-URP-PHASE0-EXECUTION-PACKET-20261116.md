# Sen-1 #611 — OWNER-APPROVED local-only Unity URP material look, executable Phase-0 packet

**Date:** 2026-11-16. **Scenario One Manager:** actual OpenAI GPT-6, browser/GitHub. **Claim:** [#611](https://github.com/jermur788/CCF/issues/611). **Status:** OWNER-AUTHORISED / READY FOR LOCAL PREFLIGHT — NOT RUNNING. The owner directly approved the **exact quoted narrow Phase-0 exception + interim D-032 allowance** in #611 at 14:36 UTC. This packet makes that permission executable; it does **not** authorise general production.

## 1. Purpose and authority

**Plain English:** test the existing #605 tuned worn-earth footpath texture in real Unity 6000.6.0f1 URP, on a temporary flat sample surface viewed at human-player height and neutral light. Determine how its normal map, 1 m tiling, colour and small needle/grit details actually look. This is **not** creation of the real Compact30 path, H1 or scene.

Owner's exact scope is retained in [#611 issue body](https://github.com/jermur788/CCF/issues/611): the named Phase-0 local-only exception overrides DRAFT and open D-032 **for this disposable preview only**. Exact #605 authored v2 maps may be temporarily imported in an isolated worktree/Library. Every imported PNG/material/scene/meta/Blender binary remains **local, untracked and uncommitted**. Only this text packet, the later Markdown result and nonbinary evidence references may enter Git. **No production scene, gameplay, project-global graphics, shared materials, saves, other profiles or builds. No licence/release approval.**

D-032 remains OPEN globally and `specs/scenario-one-compact-graphics-v1/` remains **DRAFT** with 20 FR, 12 AC, 66 unchecked. Ordinary T-02/T-03/T-04 remain BLOCKED. Do not promote isolated visual findings to a Unity-integrated Compact beta.

## 2. Routing, difficulty, exact lanes

- **Difficulty:** MEDIUM-HIGH: local Unity 6000.6.0f1 URP setup and machine-wide lease, safe isolated import and reliable native image evidence; no gameplay code.
- **Best suited:** OpenAI GPT-6.1 Sol High through **T3 Code local Unity lane** (or actually capable equivalent worker); no browser-only review can complete this.
- **Claim:** existing [#611](https://github.com/jermur788/CCF/issues/611), not a new duplicate.
- **Proposed exclusive branch:** `experiment/sen-1-611-compact-wornearth-urp-phase0-20261116-v1`, created **only after** verifying no existing branch/owner conflict, based on inspected clean current integration main HEAD and pinned in the [RUNNING] claim. Do not quietly repin after starting.
- **Proposed worktree:** an entirely new, isolated **Windows CCF Operations** worktree named `CCF-Sen-1-611-CompactWornEarth-URP-Phase0` under the host's actually verified worktree root, with its **own Library** and independent, isolated safe Editor/Prefs configuration as required by local CCF-Ops protocols. Do not assume `C:\Users\hp\.t3\worktrees` is the canonical root; record actual absolute path before work. Do not touch primary worktree or dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`.
- **Scratch source:** `C:\Users\hp\Documents\CCF-Scratch\Sen-1-597-WornEarth-v2-20261116\` (41 files plus `SHA256SUMS` as reported by #605). Exact tuned source branch/result: [#605 result](https://github.com/jermur788/CCF/blob/docs/sen-1-597-v2-worn-earth-material-tuning-result-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-597-V2-WORN-EARTH-FOOTPATH-MATERIAL-TUNING-RESULT-20261116.md).
- **Windows Blender:** owner newly installed Blender at 14:35 UTC; its actual executable/version/startup must be verified if used. No Blender modelling, original material remake, new .blend or source edits are required for a direct five-map Unity import. Linux Blender 4.0.2 from #577 is historical verified capability only.
- **Unity host:** currently **UNKNOWN**. Verify actual Windows Unity availability; if only Linux Unity is available, **STOP** and obtain separately recorded owner permission for Windows→Linux hash-verified transfer before proceeding. Do not assume shared folders or machine-wide lease free.

## 3. Mandatory preflight and stop-before-edit gate

1. Refresh live `main:Docs/Project/coordination/active-tasks.md`, #611, #424, physical `CCF-Ops` board, active writers/branches/worktrees and exact current source baseline. Confirm no assigned graphics/Unity owner conflict. Read `AGENTS.md` and host CCF-Ops instructions without modifying protected files.
2. Verify Windows source directory exists, read `PROVENANCE.md`, `REPLAY.md`, generator/source maps, manifest. Independently verify **41/41** `SHA256SUMS` (or report a precise mismatch and STOP); record hash of manifest and all five selected maps. Retain unchanged original and review folders. Any transfer needs written consent and source/destination hash proof.
3. Verify installation and actual versions of Unity Editor and URP; inspect local `Packages/manifest.json` and lock, and establish how isolation works without mutating shared `ProjectSettings`, `Packages`, quality/global graphics or source. Verify Blender command/version if using it, with no installation or downloads on this packet.
4. Reserve exclusive graphics/scene ownership, separate worktree and own Library; secure machine-wide heavy Unity lease **before launching Unity** under CCF-Ops, with accurate PID/process and save/Prefs custody preflight. Do not kill another owner's Unity or bypass the lease. If any preflight fails, add [BLOCKED] to #611, identify precise missing authority and publish short stop report; **do not** import or open Editor.
5. Post **[RUNNING] before tracked edits/Unity** on #611 naming **actual worker provider/model**, machine, branch/worktree, starting HEAD, Library/Prefs paths, source/hash outcome, physical lease ownership and file allowlist. Claim does not override live coordination lock.
6. Keep temporary imported files, .meta, .mat and .unity **outside Git-tracked source/history**. A local isolated Unity `Assets` directory may hold disposable untracked import files **only within this experiment worktree and only under the owner's interim exception**, and must never be staged or committed. Verify `git status --porcelain` and proposed tracked paths before each Git operation. **Never git clean/reset/stash/discard other work.** If safe exclusion/isolation cannot be guaranteed, STOP and choose a separate untracked throwaway Unity project with matching URP version only after describing any comparability difference in the report.

## 4. Allowed native test (one focused Tier 1)

- Create a **disposable FlatPlane Compact material-look test scene**, not `ForestTest.unity`, not any real Compact scene, and not a shipping scene. Use 1 m tile scale, fixed neutral daylight, a player-height camera and fixed URP settings from the isolated project. Do not change global URP/Quality/Graphics assets.
- Import exactly the existing v2 maps as scratch; verify Unity normal importer settings and OpenGL/Y+ (green-channel inversion only if controlled observation supports it). Compare neutral height/normal disabled vs active and optional intentionally inverted normal as a labelled negative control. Record exact import setup/sRGB/roughness interpretation, normal strength and shader.
- Capture real native output at **1280×720 and 1920×1080**, including overhead 3×3, player-height oblique and closer detail view. Evaluate 1 m patch repetitions/vertical columns, dark twig-like needles, grey-brown cast, exaggerated embossed grit, seam tiling and visibility beside a *clearly labelled illustrative* forest-floor reference if used. Do not invent true H1/G1 clearance/performance/geometry results.
- Optional Blender inspection only if it adds value and is reproducible; do not create/commit .blend or change asset source. This task is not an approved asset-generation pipeline.
- Actual v2 judgment: **SUITABLE FOR LATER DESIGN REVIEW**, **NEEDS CHANGES**, or **UNQUALIFIED**, with visual screenshots/evidence refs and failure severity. This is native experimental quality, **not production material APPROVED**, not released Unity asset, not an independent v2 production review.
- No T-02 scene baseline, no T-03 art production, no T-04 path integration, no Route A/H1/G1 edits, no protection/planting controls, no annual ecology, save/load, game build or full Tier 2/3 tests.

## 5. Hard exclusions, stopping and protection

No tracked `Assets/` files or new binary source files; no third-party/Asset Store packs or licensing assumptions; no `ForestTest.unity`, Legacy40, Enlarged80 Rack V2/MFM, Reference Future v1, historical v1–v24 saves, `Forest Ground.mat`, `Forest Path.mat`, older tracks/planks, Compact source generation, ProjectSettings, Packages, global URP/volume/quality assets, real profile routing/boot or save controllers. No Windows Player/build. No renderer/import claims without native evidence. If the Editor, source hash, ownership, custody or licence boundary changes, STOP and escalate.

## 6. Exact GitHub handoff and verification

- **One new result Markdown only**: `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-COMPACT-WORNEARTH-PHASE0-UNITY-LOOK-RESULT-20261116.md`.
- Record actual worker/model/host/branch/worktree/start HEAD; local CCF-Ops lock acquire/release (no guessed times), Editor and package versions, asset SHA manifest, import defaults and deliberate changes, camera/lighting/shader, images/captures with full path and SHA-256 outside Git, v1/v2 comparison if available, negative control and observable judgment, any defects/stops, memory/performance only if measured, all protected-file checks.
- Final tracked diff must be **one added Markdown**. Read back GitHub result, show compare and mark #611 [HANDED-OFF]/close when work is actually complete. **Do not mark handed-off merely because this packet exists.**
- Release/record Unity machine lease and ensure no stray Editor/PID or changed protected save/Prefs paths. Dashboard/Drive sync explicitly state observed/unknown.
- Manager #424 will inspect native result and issue APPROVED / NEEDS CHANGES / REJECTED **for experiment evidence**, then decide whether a further independent visual review/material adjustment or actual graphics spec approval is warranted. Still **no Windows build** before owner personal Scenario One playtest and explicit authorisation.

## 7. Copy/paste dispatch message

> Take ownership of **CCF Sen-1 #611** local-only URP material Phase-0. Read full binding handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-611-owner-approved-phase0-executable-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-611-OWNER-APPROVED-LOCAL-ONLY-UNITY-URP-PHASE0-EXECUTION-PACKET-20261116.md . The owner explicitly approved the **narrow #611 local-only D-032 interim import and graphics DRAFT Phase-0 exception only**. Do live GitHub + physical CCF-Ops preflight, verify actual Windows Blender/Unity/URP host, #605 41-file hash manifest, worker model, clean exclusive branch/worktree/Library and machine Unity lease. Register [RUNNING] before Unity/import. Use a disposable Compact-look test plane, capture native 720p/1080p URP material views and normal-Y+ control, publish the exact one-file Markdown result, release machine lease. No tracked binaries, production scenes/Assets, old worlds/saves, full graphics implementation or Windows build.

**User action required after dispatch:** launch/send the copy/paste message to a genuinely local capable T3 Code/Unity worker. **No additional owner product decision** for this bounded experiment; further policy/spec decisions remain separate.
