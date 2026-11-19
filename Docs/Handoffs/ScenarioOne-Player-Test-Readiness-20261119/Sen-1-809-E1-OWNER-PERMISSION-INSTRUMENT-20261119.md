# Sen-1 #809 — Single narrow owner permission for the first Compact30 E1 Editor preview

**Status: UNSIGNED / NOT OPERATIVE.** This is the smallest single permission needed to turn the already owner-directed Compact30 Tier S direction (D-086–D-090) into one runnable Linux Unity Editor preview. It grants nothing until the owner actually replies with the approval line in §6. No editor, scene byte, source or Unity lease exists under this instrument yet.

Parent claim: [#809](https://github.com/jermur788/CCF/issues/809). Scene/content lane: [#718](https://github.com/jermur788/CCF/issues/718). Date: 19 November 2026.

## 1. Worker and physical facts (verified)

| Item | Actual value |
|---|---|
| Worker | DeepSeek V4.1 Flash, OpenCode harness inside T3 Code, native parent, no separate manager/reviewer |
| Session | `ses_e0c9a59bbffeHZiIFIV3Xcu4JV` |
| Host | `jer-laptop`, Linux 7.0.0-42-generic |
| Parent docs worktree | `/media/jer/Videos/CCF-809-Parent-20261119` (non-root, mounted `/dev/sdb1`, 411 GiB free) |
| Parent branch / HEAD | `docs/sen-1-809-e1-minimum-editor-preview-20261119-v1` / `089e1e4c` |
| Proposed scene worktree | `/media/jer/ZX20/CCF-Sen1-718-Compact30-Content-20261117` (ZX20 SSD, `/dev/sdc2`, 342 GiB free) — matches D-090 |
| Proposed scene branch | `sen-1/718-compact30-e1-scene-20261119-v1`, base `origin/main` `159f0ff8` |
| Files floor | `/media/jer/Files` 34 GiB free — above the 10 GiB floor |
| Unity | `/media/jer/ZX20/Unity/6000.6.0f1/Editor`, project version `6000.6.0f1` |
| Deployed custody | `/media/jer/ZX20/CCF-Ops/scripts/unity-lock status` → **free**, no Unity Editor PID observed |
| Old unsafe copies | No old non-deployed `unity-lock` invoked; no `environment.sh` sourced; no `--force` |

**Lease problem stated honestly:** the installed deployed `unity-lock` requires a timezone `--eta` and cannot express D-091's no-expiry custody. Installing the #652 no-expiry patch is a HELD Ops action and is not part of E1. This instrument therefore asks for the packet's expressly authorised **human-supervised exclusive alternative** instead of mutating installed Ops state.

## 2. Content permission requested (Tier S only)

Author exactly the five D-088/D-090 paths, and no others:

1. `Assets/ForestPrototype/Scenes.meta`
2. `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`
3. `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity.meta`
4. `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json`
5. `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json.meta`

Content is the **already owner-directed provisional world**, explicitly labelled a local E1 preview, never the signed production world:

- 30 × 30 m flat continuous ground at `y = 0`, closed `[−15,+15)` layout, invisible edge containment.
- The **#676 candidate 175-row Sitka roster** (`sha256 dac06a3b…`, UNSIGNED) at its exact `x/z` and height, using the four existing first-party Sitka pruning prefabs with the manifest's deterministic FNV base selection and height scaling.
- H1 opening `x[−9,+3] z[−15,−5]` (12 × 10 m), visually distinct worn-earth apron, no stumps, no vehicles.
- Original **C0** footpath through A0–A8, 1.2 m core + 0.2 m verge, provisional worn-earth surface, ~0.625 m A7 bend preserved and flagged for owner visual judgement.
- Four **empty intended G1 sites** O1/O2 (sessile Oak) and B1/B2 (European Beech) at the manifest's exact coordinates, marked with simple stone posts and labels reading `PENDING — Oak` / `PENDING — Beech`. **No juveniles, no planting, no Oak/Beech simulation.**
- Player spawn `(2.0, 0.1, −8.0)`, yaw `16.94°`, existing `ForestPlayer` + `CharacterController` + camera `(0, 1.65, 0)`, existing New Input System.
- Existing first-party materials (`Forest Ground`, `Forest Path`, `Stone`) reused unmodified; no new art assets, no H/J kits, no #787 render proofs.

## 3. Temporary local-only tooling (D-089)

- One temporary builder: `Assets/ForestPrototype/Editor/Compact30E1SceneBuilder.cs` (+ generated `.meta`).
- One temporary smoke runner defined in the same file: enters Play, checks spawn/ground/movement/bounds, captures one screenshot.
- Both are **local-only, never committed or pushed**, and are removed with their `.meta` after the checks, with a before/after `git status` receipt. Only the five Tier S files remain.
- D-089 already approves this class of tool; this instrument confirms the exact paths for E1.

## 4. Lane and Unity custody requested

- Create the ZX20 scene worktree and branch in §1; **this session is the sole writer** of the five files and the temporary tool.
- Record #718 E1 scene-lane custody and the Unity lease in the #809/#718 issue record before any file write or Editor launch.
- **Human-supervised exclusive machine-wide Unity use:** the owner is present and supervising; no other Unity Editor or agent may run Unity on this machine during the E1 build and smoke; I verify no Unity PID before launch and after exit; I release explicitly in the issue record when E1 is finished. **No time limit** (D-091), no fabricated distant expiry.
- The installed `CCF-Ops` lock state is **read-only**; this instrument does not install #652, does not mutate the board, lanes or ledger, and does not alter any existing lease.

## 5. Explicitly NOT granted

- Full E2 loop, save integration, Compact profile persistence, Work Plan, annual simulation, browsing/regeneration.
- Signed production world, world digest, W§12 final signature, D-032 full art policy.
- Any Oak/Beech juvenile, guard mesh, shelter, or #787 art acceptance.
- Edits to `ForestTest.unity`, `MixedSpeciesTest.unity`, Legacy40, Enlarged80 or frozen Reference Future v1.
- Windows/Player build, closed beta, external distribution.
- Any `#690` save path, any `CCF-Ops` install, any other agent's lane.

## 6. Owner approval line

Reply with exactly this (or an explicit correction):

> **I approve the Sen-1 #809 E1 permission instrument dated 19 November 2026: the five Tier S Compact30 scene/placement files, the provisional 175-Sitka/H1/C0/A8/4-empty-G1 preview, the two temporary local-only Editor files with audited removal, the ZX20 #718 scene lane worked by this DeepSeek V4.1 Flash session, and human-supervised exclusive machine-wide Unity use with no time limit. I understand this is an E1 local preview, not the signed production world, not E2 and not a Windows build.**

Owner: ____________________  UTC: ____________________

## 7. What happens immediately after signature

1. Record the signed instrument, lane and lease in #809/#718.
2. Create the ZX20 worktree from `origin/main`.
3. One narrow nonauthor check of the filled grant inside this parent (no separate review campaign), run while Unity imports.
4. Build the five files, run M1 import/compile, M2 Play smoke + screenshot, M3 isolation check.
5. Publish the scene branch, the result Markdown with the three checks, and a 5–10 minute owner walk checklist.

**Verdict target:** `E1 EDITOR PREVIEW BUILT — SMOKE PASS — READY FOR OWNER PERSONAL TEST`, or `E1 PARTIAL / BLOCKED` with the exact real missing gate.
