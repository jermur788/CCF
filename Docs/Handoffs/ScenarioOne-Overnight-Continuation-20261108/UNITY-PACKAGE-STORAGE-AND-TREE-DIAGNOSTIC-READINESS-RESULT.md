# #333 — Unity package, storage and tree-B3 diagnostic readiness (READ-ONLY RESULT)

**Status:** HANDED-OFF to Mimo #332 / Manager #200. **Recommendation only. Nothing here is implemented, approved or released.**
**Claim:** [#333](https://github.com/jermur788/CCF/issues/333). **Packet:** [MANAGER-READONLY-UNITY-PROVENANCE-AND-TREE-READINESS-PACKET-20261108.md](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-READONLY-UNITY-PROVENANCE-AND-TREE-READINESS-PACKET-20261108.md).
**Worker (actual):** Claude Opus 5.5 (`claude-opus-5-5`), Claude provider, through Claude Code inside T3 Code, as a child research agent of the local orchestrator. Effort **high**. Git user `jermur788`. No other provider, quota or effort is claimed.
**Times (host `date -u`):** lane admitted 2026-11-08T00:39:49Z. Measurements taken 00:38:40Z–01:06Z. Result written about 01:10Z.
**Lane:** branch `docs/ccf-unity-provenance-readiness-20261108-v1` from fresh `origin/main` `c0513d6ac84dce4750747596bd9dacfddfd2da36`. Worktree `/media/jer/Files/CCF/Operations/CCF-UnityReadinessReadOnly-20261108`. Common store `/media/jer/Files/CCF/Repository.git`.
**Durable evidence:** `/media/jer/Files/CCF/Verification/CCF-UnityReadinessReadOnly-20261108/` (README, `SHA256SUMS.txt`, read-only scripts and their outputs).

## NOT RUN

The following were **not run**: Unity Editor or Player (batch or windowed), the licence, the `unity-lock` lease (the state file was read directly, without its guard/flock), any Library creation, warm-up, deletion or move, any package, material, scene or ProjectSettings edit, backup restore, checkout/reset/clean/stash/HEAD switch in any lane I don't own, integration, merge or cherry-pick, PR #300, saves, Reference v1, Legacy40, Windows build, and writes to `active-tasks.md`. **Nothing here is Unity-verified.** Every runtime statement below is either source-based or an offline replay, and is labelled as such.

## Summary

| # | Check | Outcome |
|---|---|---|
| 1 | Preservation of 8 dirty paths | **8/8 PASS** (measured twice, 00:40Z and 01:04Z). Bytes match backups and manifest. HEAD blobs match `head-versions/`. Index untouched. One stat-cache side effect is disclosed below. |
| 2 | Package/material provenance | Manifest/lock diffs are exactly 3 direct packages plus transitive `com.unity.sysroot.base`, 1.1.0→1.1.1. The **trigger is UNPROVEN**: the Editor's built-in manifest pins 1.1.0 as both version and minimum. All 5 material hunks trace to specific URP 17.6.0 source lines. Options A/B/C are below. **None is selected.** |
| 3 | Capacity | ZX20 has **21.622 GiB** free (unchanged since 2026-11-07T01:48:43Z). A new cold ZX20 lane would end at about 13.2 GiB, below the 15 GiB hard floor: **not admissible**. Warm reuse ends at about 21.6 GiB, 1.6 GiB above the 20 GiB STOP. |
| 4 | Tree B3 | **Offline replay, source-based, cause still unproven:** the swap is reproduced exactly by one moved clearance anchor. With the player at the authored start (−0.43, 5.25) the replay gives the start capture. With the player at (−13, −9), one of only two possible Reference-preview viewpoints, it gives the final capture. A minimal Tier-1 witness protocol is proposed. |
| 5 | Future Unity gate | The narrow environment qualification + B3 probe is separate from Workbench C–G Tier-2 and from full A–O. Manager decisions D1–D6 are required first. |

## 1. Preservation audit — measured

Lane: `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`. HEAD `4d95e45245d660e24b91e26754d9c57949bf3384`, attached to `feature/scenario-one-workbench-retirement-20261105`, the same value locally and on origin. Git dir is `Repository.git/worktrees/CCF-WorkbenchTier2-Registered-20261106`. All git calls used `GIT_OPTIONAL_LOCKS=0`. I used only `rev-parse`, `diff-files`, `diff-index --cached`, `ls-files`, `ls-tree`, `cat-file` and `log`, plus a Python reader that opens files with `O_NOATIME`. I did not run `git status`.

- At 00:40:48Z, `diff-files` showed **exactly the eight expected paths**. There were 0 untracked (excluding ignored) and 0 staged changes (`01-diff-files.txt`).
- Backup manifest: `Workbench287MorningClosure-20261107/SHA256SUMS.txt`, sha256 `9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`. This matches the value its README declares.
- For every path: working-tree sha256 = `dirty-backup/` sha256 = manifest. Committed HEAD blob bytes = `head-versions/` = manifest. Working tree ≠ HEAD. Size and mtime were identical before and after my read (`01-preservation-audit.json`). A rerun at 01:04:41Z gave identical results (`01-preservation-audit-rerun.json`).

| Path | HEAD blob | Working-tree blob | Working-tree sha256 | mtime (UTC, 2026-11-07) |
|---|---|---|---|---|
| `.vscode/settings.json` | `9b835a4253d4…` | `b234ccdeafaa…` | `61d36e47afac…` | 00:57:24.86 |
| `Packages/manifest.json` | `ca552487a531…` | `9fb01b723ad4…` | `f2ff5f95f9e9…` | 00:49:10.35 |
| `Packages/packages-lock.json` | `8c0c09786b12…` | `c26dfa4f236e…` | `f43d9894af3c…` | 00:49:10.36 |
| `…/Bilberry_Leaves.mat` | `5544e43bcc1c…` | `17e48ee32bc7…` | `106f237bf5d8…` | 01:45:20.07 |
| `…/Bilberry_Stems.mat` | `86ebdf139de7…` | `51c3ac270961…` | `1fd084251b82…` | 01:45:20.07 |
| `…/Forb_Flower_Centre.mat` | `cdf56b2cad14…` | `d84d5c6c3443…` | `39385672532b…` | 01:45:20.07 |
| `…/Forb_Leaves.mat` | `ffa72edc2493…` | `564ae72807fc…` | `e2d888926519…` | 01:45:20.08 |
| `…/Forb_Stems.mat` | `7df5cb7b6961…` | `3b3141323060…` | `b32b7b5f507c…` | 01:45:20.07 |

Full 40/64-hex values are in `01-preservation-audit.json`. **Timeline (measured, from mtimes and the lock history):** lease acquired 00:45:32Z → manifest and lock rewritten 00:49:10Z, during the cold import → `.vscode` rewritten 00:57:24Z → materials rewritten 01:45:20Z, near the end of the session → lease released 01:49:05Z → docs commit `4d95e452` at 01:50:57Z. `5085e8af`, the HEAD recorded in the lease, is a direct ancestor of `4d95e452`.

**Disclosed side effect (measured, harmless to bytes):** at 01:04Z, `diff-files` reported 5,209 paths, up from 8. I compared the index's stat records against live `lstat` without reading file contents (`05-workbench-lane-stat-field-diff.json`):
- 5,201 paths differ **only in inode number**;
- the 8 dirty paths differ in size and/or mtime as expected;
- 523 match fully.

The Linux exFAT driver assigns inode numbers at inode-cache load time. My `du`/`find` scans of four Libraries (about 116k files) most likely evicted the cached inodes. **No tracked bytes, mtimes or sizes changed, and the index file was not written** (its mtime is still 2026-11-07T08:39:36.146Z). Consequence for future workers: on ZX20, a high `diff-files` count is not evidence of dirt, and a later index-writing `git status` will only refresh stat data. Content comparison (hash) is the authoritative check. Root-cause attribution to my scan is inferred, not proven.

The user primary's `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` and the original standalone clone were not touched.

## 2. Package and material provenance — read-only

### 2a. Packages (measured)

`Packages/manifest.json` (HEAD→dirty): `com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64` and `com.unity.toolchain.linux-x86_64-linux` change 1.1.0→1.1.1. `packages-lock.json`: those three entries plus their `com.unity.sysroot.base` dependency, and the depth-1 `com.unity.sysroot.base` entry, all change 1.1.0→1.1.1. Nothing else changes. The git package `com.kitwright.unity.mcp` keeps hash `585ada203ff25b1fbea23821e159afa4a930d481`.

Cache identity (measured): lane `Library/PackageCache` now holds only the 1.1.1 dirs: `sdk.linux-arm64@5043731cb64f`, `sdk.linux-x86_64@920bc08129cd`, `sysroot.base@0a249da1e1ab`, `toolchain…@95ffda8fe8d9`. Each `package.json` says 1.1.1. Together they use 396,886,016 B (0.370 GiB) allocated. The other three warm Libraries (MainIntegration, U1, UIQualification) hold the 1.1.0 hash `sdk.linux-x86_64@6d12be270055`. Every other ZX20 lane manifest still says 1.1.0. Git history has never contained a 1.1.1 manifest. The global UPM cache is **UNAVAILABLE**: `~/.cache/unity3d/cache` is absent, and the #287 run used a redirected config/home.

Event receipt (measured, `Workbench287Tier2-20261107/compile.log`): the first UPM resolve (line 129, 41.28 s) registered **1.1.0**. A second resolve after the first domain reload (line 256, 40.56 s, "Lock file was modified") registered **1.1.1**. The reload's `ProcessInitializeOnLoadMethodAttributes` took 4,734 ms. The log, at level 2, does not name a requester.

Editor built-in manifest (measured): `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Data/Resources/PackageManager/Editor/manifest.json`, sha256 `3b72306d…5d39`. It lists all four packages with `"version": "1.1.0"` and `"minimumVersion": "1.1.0"`. So the rewrite is **not** explained by an Editor minimum-version bump.

**Trigger: UNPROVEN.** Source checks ruled out the obvious in-project candidates:
- `Unity.SysrootPackage`'s only `[InitializeOnLoadMethod]` just logs a warning;
- KitWright's Newtonsoft bootstrap `Client.Add` is only called when Newtonsoft is missing, and it is present (3.2.2);
- KitWright's `UpdateChecker` `Client.Add` targets its own git URL, and that hash did not change.

Remaining candidates, all unverified: Editor-internal Linux build-support or toolchain logic, or another UPM request during InitializeOnLoad. Proving it requires a Unity run with verbose UPM logging.

### 2b. Materials (measured diff; source-attributed; equivalence NOT assumed)

All five materials use URP Lit (`m_Shader guid 933532a4fcc9baf4fa0491de14d08ed7`, version 10). In URP 17.6.0, as found in the lane's `Library/PackageCache/com.unity.render-pipelines.universal@b9a66914c09e`:

| Hunk | Files | Source line that writes it | Rendering relevance (inferred, not verified) |
|---|---|---|---|
| `_MainTex` 0 → same texture GUID as `_BaseMap` | Bilberry_Leaves, Forb_Leaves | `BaseShaderGUI.SetMaterialKeywords` l.937–941 copies `_BaseMap` into the legacy `_MainTex` | URP Lit samples `_BaseMap`. `_MainTex` is a legacy alias, which may matter to legacy-API or lightmapper consumers. Not visually proven. |
| `_AlphaToMask` 0 → 1 | Bilberry_Leaves, Forb_Leaves (both `_AlphaClip: 1`) | `SetupMaterialBlendModeInternal` l.1027–1034 (opaque + alphaClip ⇒ alphaToMask) and l.1127–1129 | Alpha-to-coverage only acts with MSAA. Both `PC_RPAsset` and `Mobile_RPAsset` have `m_MSAA: 1` (off). Camera-level overrides are unknown. |
| `_Color` float representation (e.g. 0.16 → 0.15999997) | Bilberry_Stems, Forb_Flower_Centre, Forb_Stems | l.943–944 copies `_BaseColor` into `_Color`. `_BaseColor` itself does not change. | Difference ≤ about 4×10⁻⁸ (float32 round-trip). Legacy alias only. |

These are validation writes from `MaterialPostprocessor`/ShaderGUI. **Inference:** any URP validation or reimport of these five materials will very likely rewrite them again, whatever the SDK version. Restoring the HEAD bytes would recover the baseline bytes but would not keep them clean, and would not prove visual equivalence.

`.vscode/settings.json`: Unity's IDE integration removed a trailing comma and renamed `dotnet.defaultSolution` to the lane-named `.slnx`. This is local tool configuration, not rendering.

### 2c. Options for Manager (none selected or applied)

| Option | What | Preconditions (all required) | Known risks / unknowns |
|---|---|---|---|
| **A** Restore exact tracked HEAD, with a "do-not-resolve-past" policy | In a later authorised run, restore the 8 paths to `4d95e452` bytes. The preserved copies stay as evidence. Run only if the Editor stops rewriting. | (1) 8/8 preservation re-verified immediately before the restore. This result verified it at 01:04Z, but it must be re-checked. (2) Manager approves the "tracked 1.1.0 only" policy and the restore. (3) **A dry-run proves no rewrite.** This is NOT available today, because the trigger is unproven. It needs one Unity probe with verbose UPM logging and before/after hashes of manifest and lock, with STOP if either changes. (4) Accept that the materials will likely be re-dirtied by URP validation. Material state must then be reported separately, never auto-restored. | The SDK may be rewritten again (trigger unknown). The Library's PackageCache holds 1.1.1, so a re-resolve swaps about 0.37 GiB and reimports package assets. Visual equivalence stays unproven. |
| **B** Deliberate reviewed 1.1.1 update | A separate source PR changes the three direct packages and the transitive sysroot to 1.1.1, optionally committing the URP-validated material bytes. | (1) Manager approves the package change as a source change, with the reason that 6000.6.0f1 resolves to 1.1.1. (2) Independent review of the lock diff. (3) Native/Linux-player toolchain consequences acknowledged: this is a build toolchain, and no player build is allowed before the user playtests. (4) Material bytes committed only after a rendered player-view comparison of the two leaf materials. (5) Every lane that later imports at 1.1.0 treated as a different environment. | Changes the environment of all future qualifications. Old evidence at 1.1.0 is not directly comparable on toolchain grounds. Renders are still unverified. |
| **C** Alternative clean machine or isolated lane | Qualify in a fresh lane cloned at the exact candidate, with its own Library, and leave the registered lane untouched as evidence. | (1) Capacity: **not met on this machine today** (§3). Needs at least 28.40 GiB free on ZX20 before entry, i.e. 6.78 GiB to reclaim, which only Manager/user can authorise. (2) A package policy (A or B) is **still required**: a fresh import of 6000.6.0f1 is expected to show the same rewrite (inferred from the second-resolve receipt). (3) New registered lane plus lease owner. | Does not remove the package question. Uses the most disk. |

## 3. Capacity — measured vs inferred

Measured (read-only `df -B1`, `du -s -B1`, `stat -f`; details in `03-capacity-measurements.txt`). ZX20 is exFAT with a **128 KiB cluster**, so allocated size is about 2.2× the apparent Library size.

| Item | Bytes | GiB | Kind |
|---|---:|---:|---|
| ZX20 free (00:38:40Z, 01:04:41Z; unchanged since #287 final at 2026-11-07T01:48:43Z) | 23,216,128,000 | **21.622** | measured |
| Files free at 00:38:40Z / after my worktree / at 01:04:41Z (other workers created lanes) | 30,459,355,136 / 29,218,414,592 / 26,683,838,464 | 28.368 / 27.212 / **24.851** | measured |
| Root fs (`/tmp`) free | 5,048,844,288 | 4.702 | measured |
| Workbench lane Library, allocated (apparent 3,177,747,466; 29,013 files) | 7,058,227,200 | **6.574** | measured |
| MainIntegration / U1 / UIQualification Library, allocated | 7,216,824,320 / 7,250,771,968 / 7,210,270,720 | 6.721 / 6.753 / 6.715 | measured |
| Workbench worktree excluding Library, allocated | 1,968,046,080 | 1.833 | measured |
| #287 single cold import, peak consumption | 7,050,756,096 | 6.567 | measured (historical log) |
| #287 all gate runs after import | 21,626,880 | 0.020 | measured (historical log) |
| This lane's docs worktree on Files / evidence | 1,239,212,032 / about 98,304 | 1.154 / ~0 | measured |

Projection against the floors (**27 GiB cold entry / 20 GiB Manager STOP / 15 GiB hard floor**). All rows are inferred:

| Scenario | ZX20 free after | vs floors | Admissible? |
|---|---:|---|---|
| S1 warm reuse of the registered lane, no package change | ≈ 21.60 | above STOP by 1.60 | Only if Manager accepts that lane and its package state (D2/D3) |
| S2 warm reuse + Option A re-resolve to 1.1.0 (+0.37 package dirs, ≤ ~0.4 reimport) | ≈ 20.85–21.25 | above STOP by ≤ 1.25 | Marginal. Requires a disk STOP monitor at 20 GiB |
| S3 new cold ZX20 lane (1.83 worktree + 6.57 import) | ≈ 13.22 | **below 15 hard floor**. Fails cold entry by 5.38 | **NO** |
| S4 pre-free needed so a cold lane ends ≥ 20 GiB | needs ≥ 28.40 before entry | shortfall 6.78 | Reclaiming one inactive Library (6.72 GiB) leaves 28.34: **still 0.06 short**. Two would be enough (≈ 35.06) |

Proposals only (nothing reserved, deleted or moved):
- **(P1)** Prefer warm reuse with a 20 GiB STOP monitor (at #287's 20 s polling cadence).
- **(P2)** If a cold lane is ever required, Manager/user must authorise reclaiming **two** inactive Libraries. Under the storage policy, delete them rather than archive. Candidates are MainIntegration, U1 and UIQualification, all at 1.1.0 and none registered as active.
- **(P3)** Reserve ≤ 1 GiB on Files for evidence. #287 used 59 MB. Note Files fell 2.5 GB in 25 min from concurrent workers, so re-measure at admission.
- **(P4)** Keep harness temp output off `/tmp`, which has only 4.7 GiB free.

## 4. Tree B3 (P0604 lost / P1209 gained) — source, replay, protocol

### 4a. What the captures show (measured)

Captures in `Workbench287Tier2-20261107/DIAG-enlarged80-gate-modified-harness-run{4,5}/…/diag-{start,final}.json`:
- run4 start/final sha256: `9b5e9f0d…29d0c8ea` / `3fbdc138…07fa4aa`;
- run5 start/final sha256: `6905220c…0788283a` / `9d8acf02…205428ae`.

Each capture has 1,344 trees. 1,343 identities are shared and their records are identical. P0604 is lost and P1209 is gained. `scenarioOne.understoreyCells` also differs. **There is no player pose field:** the save schema has no player key. The records are identical because both runs used the same deterministic generator and the same inputs, apart from one clearance anchor, which is not captured.

### 4b. Production mechanism (source-based, lane-equivalent)

`ForestStartingStand.GenerateEnlarged80` (main `c0513d6a`) generates the Legacy40 core as a 21×21 lattice at the **scene-serialized spacing of 2 m** (the code default 1.9 is overridden in `ForestTest.unity`). Its fixed omission budget is 105:
1. slots inside any clearance anchor are omitted first;
2. the budget is then filled with the lowest-`FnvHash("omit-i")` remaining slots.

`CollectClearancePoints()` takes, at generation time:
- every active `Dirt Path*` transform (radius 1.5 m), which includes runtime `StandApproachPath` planks;
- `Forest Clearing` (2.2 m);
- **the live `ForestPlayer.transform.position` (2.2 m)**;
- every `ForestBuildable`, including inactive ones (2.2 m).

The lane adds `&& !buildable.IsRetiredFromStartingWorld`, the only difference from main. The retired workbench sits on the Forest Clearing anchor, so excluding it changes no core slot.

The harness's `Enlarged80Verification.ResetWorld` (at `5085e8af`) destroys the trees, applies geometry, resets, calls `stand.Generate()` and initialises the scenario. **It does not restore the player pose.** Section G calls `TryBeginReferencePreview(100)`, which runs `MoveToReferenceView`: it disables the CharacterController, moves the player to an odd-integer candidate (x, 0.08, z), and re-enables the controller. `EndReferencePreview` then assigns `player.transform.position = previewPlayerPosition` **with the controller enabled**. The project has `m_AutoSyncTransforms: 0` and `ForestPlayer.Update` calls `controller.Move`.

Generally documented Unity behaviour (not verified here): a transform write to an enabled CharacterController can be overwritten by its next `Move` while auto-sync is off. That would leave the player at the Reference viewpoint for later `ResetWorld` calls (sections I and final).

### 4c. Offline replay (read-only, `04-b3-*.txt`; scripts in evidence `scripts/`)

- Replayed core positions match the captures exactly: P0604 (−12.1465, −8.0724), P1209 (−2.0547, 3.9100).
- The **authored player start (−0.43, 5.25) is 2.106 m from P1209**, inside 2.2 m. No static anchor lies within 13 m of P0604. Hash ranks are P0604 113 and P1209 186. The lowest-hash living slot is P0600 (rank 68), so the swap is **not** a budget shift. It is one slot leaving a clearance zone and another entering one.
- The authored scene anchors from `ForestTest.unity@4d95e452` (52 path planks, clearing, 7 buildables, player), plus **one inferred runtime anchor covering P0009**, reproduce the **start capture's 336-tree core set exactly**. That inferred anchor matches the earlier live finding that the track approach removes P0009 and admits P0600. 16 prefab instances were not expanded.
- A grid search (0.5 m over ±20 m, plus 0.05 m within ±3 m of P0604) finds that the **final capture is reproduced only with the player anchor in x ∈ [−14.3, −11.6], z ∈ [−10.0, −7.8]**, at least 17.3 m from the authored start.
- A `MoveToReferenceView` replay against frozen Reference v1 year-100 finds **only two eligible viewpoints: (−7, −11), with score −6.08 excluding Light, and (−13, −9), with −6.73**. The decompressed copy has sha256 `f2bbb431…5600`; the tracked `.bytes` blob `c14c1caa…` is unchanged and identical on main and the lane. The archive does not store Light; the light term adds up to +6 and decides the winner.
- **Player at (−13, −9) reproduces the final capture exactly** (lost P0604, gained P1209). Player at (−7, −11) does not (4 lost, 4 gained).

**Source-based mechanism (strong, falsifiable):** after section G, the player stays at the Reference viewpoint (−13, 0.08, −9), and the next `ResetWorld` generates with that clearance anchor.

**Unproven:**
- that the player was actually there;
- why the restore did not hold (the CharacterController sync, or something else);
- that Light selected (−13, −9);
- what the runtime P0009 anchor is.

**This does not establish a production bug.** In real play the player pose is legitimately an input to new-world generation. Whether reset determinism should exclude player pose is a design decision for Manager. B3 is **not** passed, and no fixture, hash, generator, transform or save change is proposed.

### 4d. Minimal future Tier-1 witness protocol (evidence and logging only; separate future packet)

Verifier-side only (under `Tools/Verification/`, the disposable harness). Production source stays unchanged. The approved Workbench verifier `a4019e6b` already logs `ENLARGED80_RESET_INPUT model=… player=… sites=…` before each `ResetWorld`. Extend the witness to record:

1. **Identity (pre and post):** repo HEAD, candidate SHA, active scene path and the scene file's git blob, Editor version, sha256 of `Packages/manifest.json` and `packages-lock.json`, PackageCache dir hashes for the 4 Linux packages, and the run's UTC start time.
2. **Player pose** at: harness start; every `ResetWorld` entry, immediately before `stand.Generate()`; before and after `TryBeginReferencePreview`; after `EndReferencePreview`; and one and two frames after `EndReferencePreview`. Record `transform.position` with "R" formatting, `CharacterController.enabled`, and the controller's `center`/`bounds`, plus `Physics.autoSyncTransforms`.
3. **Clearance anchors** as `CollectClearancePoints()` will see them at each Generate: list of (kind, name, x, z, radius). The harness recomputes this read-only with the same Find calls, sorted. Record the list's sha256 and the count of core slots inside each anchor.
4. **Tree identity:** count, sorted ID list sha256, and the per-record hash used by `ScenarioReferenceArchive.CurrentWorldHash`. Explicitly log the core clearance-omitted set and the hash-filled set.
5. **Seed and state:** `simulationSeed`, `rngModelVersion` and any static RNG state the harness can read. Where it cannot be read, record "UNAVAILABLE".
6. **Reset order:** one numbered log line per step (destroy trees → ApplyStandGeometry → ResetForDeterministicRun → Generate → InitializeNewScenario), with the frame count.
7. **Archive preservation:** sha256 of `ScenarioOneReferenceFutureV1.bytes` and of the Legacy40 fixtures, before and after; any change fails.
8. **Duplicate runs:** two identical runs from the same Library and candidate, plus one variant where the harness pins the player to the authored start before every `ResetWorld`. This is a harness-only control. Expected if the mechanism holds: the variant shows no swap and both plain runs show P0604/P1209.
9. **Pass/fail of the witness** is "evidence complete", not "B3 passed". B3 disposition stays with Manager.

## 5. Realistic future Unity gate

| Gate | Scope | Prerequisites |
|---|---|---|
| **G-env** narrow environment qualification + B3 witness (Tier-1) | One Editor session. Compile, package/material STOP checks, the §4d witness runs only | Manager D1–D6 below |
| **Workbench C–G Tier-2** | #287 focused qualification on the approved verifier `a4019e6b` | G-env passed. Package policy settled |
| **Integrated A–O (15 gates)** | Integrated candidate (#334 plan) | Integrated candidate exists (none does today). All 15 gates are NOT RUN |

**What Manager would have to accept (D1–D6):**
- **D1 Candidate:** source `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29` (published `03b484d10b`). Measured: relative to lane HEAD `4d95e452` it changes only `Tools/Verification/**` and Docs, with **no Assets/Packages/ProjectSettings change**, and `4d95e452` is its ancestor.
- **D2 Package policy:** A, B or C (§2c). Without one, a run cannot be told apart from environment drift.
- **D3 8-path disposition:** keep the paths dirty during G-env (they don't overlap D1's diff), or restore them under Option A. Either way, re-verify 8/8 by hash immediately before.
- **D4 Lane reuse:** the registered lane `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` is the **only capacity-admissible** lane (S1/S2). It holds a 6.574 GiB Library at 1.1.1 imported from identical Assets. Reuse needs an explicitly authorised fast-forward `4d95e452 → a4019e6b`, which touches no protected path, and Manager acceptance that its Library state (1.1.1 packages, validated materials) is the environment under test. MainIntegration, U1 and UIQualification are at unrelated HEADs (`9a6ed000`, `e7f086a1`, `a1e15b52`) and are not candidates.
- **D5 Owner and lock:** one named owner. Acquire `unity-lock` with the lane `WorkbenchTier2Reg287` (already in `lanes.json`) and task #333-follow-on. Today: lock free (`lock.json` absent) and no Unity process (00:38Z).
- **D6 Disk:** a STOP monitor at 20 GiB on ZX20, measured at admission (about 21.6 GiB expected).

**Cannot be determined without Unity:** the SDK rewrite trigger and whether 1.1.0 holds; whether material validation re-dirties the materials on this candidate; the runtime Light values and so which Reference viewpoint is chosen; the actual player pose after `EndReferencePreview`; the identity of the runtime P0009 anchor; rendered equivalence of the leaf materials.

**Required no-build sequence (proposal):** Manager D1–D6 → re-verify 8/8 → acquire lock → record identity → G-env compile with verbose UPM log → STOP on any manifest/lock change unless the policy allows it → §4d witness runs (2 plain + 1 pinned-pose control) → release lock → hash the evidence on Files → hand back. **No Linux/Windows player build** before the user personally playtests a Manager-approved exact candidate and gives explicit build consent.

## Recommendation (no implementation)

1. Manager decides the package policy first: **Option B** if 6000.6.0f1 is meant to stay the pinned Editor, since its resolver demonstrably lands on 1.1.1; **Option A** only after a verbose-UPM probe shows 1.1.0 holds. Do not use C on this machine without authorising reclaim of two Libraries.
2. Treat the five material rewrites as URP 17.6.0 validation output. Decide whether to commit them under B with one rendered comparison, rather than restoring them repeatedly.
3. Run B3 as the §4d evidence-only witness in G-env on the registered lane, after the authorised fast-forward to `a4019e6b`. The offline prediction to falsify: post-preview pose at (−13, 0.08, −9), and no swap when the pose is pinned.
4. Keep B3 NOT PASSED and the 8 paths protected until those decisions are made.
