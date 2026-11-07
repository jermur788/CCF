# #287 — Workbench retirement Phase-3 Tier-2 Unity qualification: RESULT

**Date:** 2026-11-07 (UTC, worker clock). **Claim:** [#287](https://github.com/jermur788/CCF/issues/287). **Orchestration:** Mimo overnight [#313](https://github.com/jermur788/CCF/issues/313).
**Document status:** worker report. It is **not** an independent review and **not** Manager acceptance. Manager #200 alone issues APPROVED / NEEDS CHANGES / REJECTED.

## 0. Verdict (read this first)

> **NOT QUALIFIED — the pinned verifier candidate `5085e8af` FAILS two of its three Unity gates, and the failures are defects in verifier code, not (as far as measured) in production code. Recommendation: NEEDS CHANGES (verifier-only repin), then a short re-run.**

| Item | Result on the **pinned candidate** (HEAD `5085e8af`, unmodified) |
|---|---|
| Real cold import / compile | **PASS** — exit 0, 0 `error CS` |
| `WorkbenchRetirementVerification` (the new Tier-2 lifecycle gate) | **FAIL** at check 36 (35 PASS first); sections C–G **NOT RUN** on the pinned file |
| `MfmCoverageWorldUnityGate` (independent six-vs-seven Site inventory) | **PASS** — `MFM08A_UNITY_PASS checks=57` |
| `Enlarged80Verification` (oracle parity) | **FAIL** at its 7th check (6 PASS first): stale `saved.version == 23` pin; the same pin is in parent `8b1946dc` (production `CurrentVersion` has been 24 since 2026-10-29) |
| `ScenarioReferenceVerification` default (Enlarged80 new game) | integrity PASS, then **FAIL** — `archive.Matches()` requires a live Legacy40 world by design (D-056) |
| `ScenarioReferenceVerification` with `CCF_STAND_GEOMETRY=0` (documented Editor-only override) | **PASS** — `REFERENCE_FUTURE_V1_PASS` |
| `RackV2PersistenceVerification` (mode v2) | **FAIL at entry** — stale `CurrentVersion == 23` pin; modes legacy40/coldstart **NOT RUN** for the same reason |
| `RackV2Objective1EndToEnd` (native, 720p + 1080p) | **PASS** — `RACKV2_O1E2E_PASS checks=173 shots=24` |
| Dedicated native render of the workbench site / ghost selection prompt | **NOT RUN** (no such harness in the candidate; see §6) |

**Separately and explicitly NOT part of the candidate verdict — diagnostic copies.** To find out whether the failures hide a production problem, I ran *out-of-repo, modified copies* of two harnesses (diffs in evidence, labelled `DIAG-`). With four small harness corrections the lifecycle gate **passed all 103 checks**, and the diagnostic Enlarged80 copy passed everything except its last "scratch world returned to start" comparison. These runs are **not** qualification evidence for `5085e8af`; they show that the production behaviour is consistent with the policy and that the verifier needs a repair commit that the Manager must repin. No source, verifier, scene or asset in the registered worktree was edited by me.

## 1. Provenance and candidate

- **Worker:** T3 delegate worker, provider **claudeAgent (Claude A)**, model **claude-sonnet-5-5**, effort **HIGH** (set by the Mimo orchestrator).
- **Lane:** `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` (registered `WorkbenchTier2Reg287`, git-common-dir `/media/jer/Files/CCF/Repository.git`), branch `feature/scenario-one-workbench-retirement-20261105`.
- **Runtime candidate HEAD:** `5085e8afd5abb90533e834dd6bbe7444b23973a4` (verified `git rev-parse HEAD` at start and unchanged at the end). **Parent/base:** `8b1946dcec00587971d1f7c80004a47ebaa88515` (source-approved; `origin/feature/…` is still at this SHA; `git merge-base --is-ancestor` origin→HEAD passes). Source implementation `57f10ff0`, base `50042bbe`.
- **Verifier delta of `5085e8af` (4 files, +328 −2; production, scene and assets byte-identical to `8b1946dc`):** `MfmCoverageWorldUnityGate.cs` (independent `BuildId != "workbench-01"` Site filter + 7-buildables/6-masks assertion), `Enlarged80Verification.cs` (independent exclusion at the footprint loop), new `WorkbenchRetirementVerification.cs` (262 lines), new `run_workbench_retirement.py` (60 lines).
- **Editor:** Unity 6000.6.0f1 (f7f8ed4d1e24), Linux. All batch gates `-batchmode -nographics`; the Objective 1 gate ran windowed on a private Xephyr display `:84` (never `:0`).
- **Evidence directory (Files, durable):** `/media/jer/Files/CCF/Verification/Workbench287Tier2-20261107/` (§9).

## 2. Admission, lease and disk

| Time (UTC) | Step | Result |
|---|---|---|
| 00:44:49 | Worktree identity, clean status (`--ignored` empty), origin ancestry | PASS |
| ~00:45 | #287 comments refreshed: no other `[RUNNING]` after the 00:28Z Mimo order | no collision |
| ~00:45 | **[RUNNING]** posted: <https://github.com/jermur788/CCF/issues/287#issuecomment-6699801097> | before any Unity |
| 00:45:27 | Disk gate: ZX20 `/dev/sdc2` exfat rw, 30,277,238,784 B = **28.20 GiB** free (≥ 27 GiB) | PASS |
| 00:45:32 | `unity-lock acquire` — owner `mimo-287-claude-sonnet-5-5`, task `287`, lane `WorkbenchTier2Reg287`, full HEAD, ETA 06:45:31Z | **held, `lock_id` = `fa8b4b65bc1b4dda8f21f4b6bcb8c82d`** (receipt: `lease-acquire.txt`) |
| 01:49:05 | `unity-lock release` (same owner/task/lock_id, reason "#287 qualification complete, Editor stopped") | **released, `forced: False`**; `status` afterwards: `free`, no Editor PIDs (`lease-release.txt`, `lease-status-after-release.txt`) |

Disk readings (bytes free on ZX20; GiB = ÷2³⁰): 00:45:27 **30,277,238,784 (28.20)** → cold import → 00:57:44 **23,237,754,880 (21.64)** → all later runs flat 23,215–23,240 M (**21.62–21.64 GiB**) → 01:48:43 final **23,216,128,000 (21.62 GiB)**. Cold-import cost 7.05 GB (6.57 GiB); `Library/` = 6.6 GiB. The 20 GiB operational STOP threshold and 15 GiB hard floor were never reached (minimum observed 21.62 GiB). Full series: `disk-log.txt`, `disk-monitor-compile.log`.

No commit/branch switch, no second Library, no cache deletion, no all-lane `du`, no Player/Windows build, no `--force`.

## 3. Real compile / import (PASS) and process deviations

- **Cold import, attempt 1: exit 198** — the `$HOME` entitlement `UnityEntitlementLicense.xml` is stale (sha256 prefix `24ebbb7a9e29`, 2026-09-15; log: *Code 404 … 0 entitlement groups*). No `Library/` was created (`compile-attempt1-exit198-stale-home-licence.log`). Remedy: seed the isolated `XDG_CONFIG_HOME` from the **Unity Hub licence copy** (sha256 prefix `19ae436f9017`, 2026-11-05), the practice recorded in earlier gate notes. The licence copies were deleted from the evidence directory after use (only sha prefixes kept).
- **Cold import, attempt 2: PASS** — 2026-11-07T00:47:38Z → 00:57:35Z, `-job-worker-count 2`, under `systemd-run --user --scope -p MemoryMax=8G -p MemorySwapMax=256M`. **Exit 0**, `error CS` count **0**, Asset Pipeline Refresh 535.657 s, Library 6.6 GiB. Subsequent gate runs reused that same Library on the same HEAD.
- **Editor rewrites of tracked files (not committed, not part of this result commit; patches in evidence):** after the cold import `.vscode/settings.json` (trailing comma, `dotnet.defaultSolution` renamed to this lane's slnx), `Packages/manifest.json` and `Packages/packages-lock.json` (**Unity auto-resolved `com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64`, `com.unity.toolchain.linux-x86_64-linux` from 1.1.0 to 1.1.1**); after the windowed run five Section Five `.mat` files (known rewrite). **Residual:** the runtime evidence therefore ran with the Editor-resolved 1.1.1 linux SDK packages, which differ from the tracked manifest. This is an Editor-driven package-resolution change, not a candidate change; flagged for the Manager's package-policy view.
- **Runner defects observed (candidate files untouched):** `run_workbench_retirement.py` crashes in its own `finally` after every Unity run on this machine: it SHA-256-reads every entry of the copied licence folder and the Editor re-creates `licenses/packages/` (a directory) → `IsADirectoryError`. The harness `.cs`/`.meta` removal precedes that line and the Editor had already exited, so no side effect, but the runner's `.json` receipt and `.markers.txt` are not written; I extracted markers with the same filter by hand (`markers.manual.txt`) and recorded the crash (`workbench-gate/RUNNER-CRASH-NOTE.txt`). The runner also hard-codes `~/.config/unity3d/Unity/licenses`, so I invoked it through a tiny out-of-repo wrapper (`python3 -I -B`) that points `Path.home()` at a scratch home on Files holding the Hub licence. The wrapper changes no repository file.
- **Enlarged80 gate run was refused once** by the runner's "A Unity Editor is already running": the previous diagnostic Editor was still flushing the Library after its verdict (≈ 8 min, state D). It was mine (child of my runner) and exited by itself; I waited and did not kill anything. A subsequent run went normally.
- **Windowed Objective 1 gate** was run with an out-of-repo copy of `Tools/Verification/RackV2Objective1/run_gate.py` with exactly three adaptations (diff in `run_gate_287.adaptation.diff`): lock-owner task `RACK-V2-12` → `287` (the check still demands a held lock naming this lane and exact HEAD), the dirty-lane check allows only the three documented Editor-rewritten paths, and `HERE` points at the repo directory for `native_input.py`. I did not change the lock service, my lease identity or any repository file.
- The runners were *not* run under a cgroup memory cap except the compile; no OOM or swap pressure occurred (15 GiB RAM).

## 4. Pinned-candidate results in detail

### 4.1 `WorkbenchRetirementVerification` — **FAIL at check 36 (35 PASS)**
Log: `workbench-gate/WorkbenchRetirementVerification.log`, markers `workbench-gate/markers.manual.txt`. The gate throws on the first failure by design.

PASS before the failure (35): scene loaded, isolated persistent path, **seven buildables with exactly one carrying `RetiresFromStartingWorld` = `workbench-01`**; **independent expected inventory (every scene buildable except the literal id) = six**: `grinding-stone-01, log-rack-build-01, log-rack-build-02, saw-pit-01, shelter-01, timber-sledge-01`; fresh Enlarged80 new game = **1,344 trees**; workbench retired, unbuilt, unavailable; **no active renderer and no enabled collider** (`activeRenderers=0 enabledColliders=0`); `CaptureData().buildables` omits `workbench-01` and lists exactly the six; **six Site masks** from the production coverage adapter; baseline-equivalent (workbench un-retired) world: seven buildables, **seven Site masks**, **identical 1,344 tree ids/positions/diameters** with and without the retired workbench, different hash only by the omitted entry, and *baseline capture minus the one `workbench-01` entry equals the new-game hash exactly*; deterministic regeneration; old save naming `workbench-01` (`built=false` and `built=true`) loads, identity preserved, not retired, re-capture keeps exactly one `workbench-01` and seven entries, historical built state round-trips, **seven Site masks**, visible exactly when built.

**FAIL (check 36):** `old save: every buildable with a workbench prerequisite sees it met []`.
**Diagnosis (measured, not assumed):** the assertion selects `Buildables().Where(b => b.HasPrerequisite)` — *every* buildable that has *any* prerequisite — and requires all to be met. In that saved world only the workbench is built; `log-rack-build-02` and `grinding-stone-01` require the **Saw Pit**, which is unbuilt, so they are correctly unmet. Dependency graph read from the loaded world (`DIAG-workbench-gate-modified-harness-run1…/log`): workbench ← {saw-pit-01, timber-sledge-01, shelter-01, log-rack-build-01} (met=True); saw-pit-01 ← {log-rack-build-02, grinding-stone-01} (met=False, correctly). This is a **verifier scope defect**, not a production regression.
**NOT RUN on the pinned file (consequence of the early stop):** §C disk round trip of a fresh new-game save, §D empty/null-buildables negative case, §E Reference Future v1 preview enter/exit (years 0/20/50/100), §F Year-0/Year-3 hashes (continuous vs through a save/load), §G Legacy40 fresh-world accounting.

### 4.2 `MfmCoverageWorldUnityGate` — **PASS, `MFM08A_UNITY_PASS checks=57`**
Log `mfm-coverage-world-gate/MfmCoverageWorldUnityGate.log`. Includes the new independent assertion `new world: seven scene buildables, the independent inventory is six Site masks [buildables=7 site=6]`, `every infrastructure source exists in the live world [track=104 clearing=1 site=6]`, `MFM08A_NOTE masks track=104 clearing=1 site=6 headland=4`, **production masks equal an independent construction from the live scene (areas, stems, net areas)**, all four mask kinds present, moving the player changes no mask, natural-regeneration flags, determinism across reads, and a full Enlarged80 Rack V2 flow in the new world (5 racks planned, plan confirmed, **149 tasks approved through the Work Plan and executed**, sample release executed). This is the narrow Rack V2 / Work Plan non-regression in a new world with the workbench retired.
Caveat: Track/Headland/Clearing masks are asserted equal to an independent construction, but no separate seven-buildable baseline count of those three kinds was recorded in this gate, so "unchanged versus the historical world" is shown by independence of construction, not by a before/after number.

### 4.3 `Enlarged80Verification` — **FAIL at its 7th check (6 PASS)**
Log `enlarged80-gate/Enlarged80Verification.log`. Passed: policy/geometry, **1,344 trees = 336 core + 1,008 outer** with the independent workbench exclusion (closest pair 0.9678 m), core preserved 335/336 ids/positions (the documented P0009/P0600 track-approach note), deterministic starting world `ENLARGED80_START_WORLD_CURRENT_HASH 71481A025AB269B2`, edge-band checks, construction sites remain unavailable. **FAIL:** `an Enlarged80 capture is v23 + geometry 1` — a **stale pin**: `ForestSaveData.CurrentVersion` is 24 and the identical line is present in `8b1946dc` (it cannot have passed on main since save v24, 2026-10-29). Not related to the workbench change.

### 4.4 Reference Future v1 / Legacy40 (existing unmodified harness `ScenarioReferenceVerification`)
- Default run (fresh Enlarged80 new game): archive integrity **PASS** (`REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule=56C8B99FA1E8DDD1 archiveFileFnv=1A42C7BD0439E72F`, negative control PASS), then **FAIL** `the archive loader did not verify every frozen milestone for this scenario`. Cause: `archive.Matches(...)` requires the live world to be Legacy40 (`ScenarioReferenceArchive.Matches`, D-056); this harness is meant to run with the Editor-only `CCF_STAND_GEOMETRY=0` override. Evidence `scenario-reference-gate-run1-default-enlarged80-fails-Matches-by-design/`.
- With `CCF_STAND_GEOMETRY=0`: **`REFERENCE_FUTURE_V1_PASS`** — archive hashes pinned and matching (year 0 `A564039D9B7CE31D`, 20 `F7DF7DAB53B6FD32`, 50 `D5E75D6D21D631AC`, 100 `7AD177B3CC2F73C7`), previews at 0/20/50/100 reproduce those hashes with 336/352/401/517 trees, continuation `REFERENCE_CONTINUATION_PASS year50=v12 year100=v24 deterministic=True continuedHash=EBD49ADEF18C3F36 legacyV15LayoutHash=BFFF66015A6619DA` (both match the protected anchors recorded by RACK-V2; BFFF… is the intended post-ordinal-growth anchor, not 9CDF…). Evidence `scenario-reference-gate-legacy40-env/`.
- **Frozen archive bytes:** SHA-256 of `ScenarioOneReferenceFutureV1.bytes` (`789e3edd…b041fc`), `ScenarioOneReferenceFutureV1.md` (`fd9e3c79…adf9ec`) and `Docs/ReferenceFutureContract.md` (`239ed039…5e27b9`) recorded before and after all Unity runs: **identical** (`frozen-reference-sha256-before.txt` / `-after.txt`); `git status` shows no change to them.

### 4.5 `RackV2PersistenceVerification` (existing harness; historical v20/v21 and Legacy40 saves)
Run once in mode `v2` (after supplying the required `CCF_DIAG_DIR`): **FAIL at its entry assertion** `this gate qualifies save v23 (no schema change expected)` — `CurrentVersion` is 24. Stale pin, independent of the workbench. Modes `legacy40` and `coldstart` were **not run** because they share that entry check. First invocation also failed only because `CCF_DIAG_DIR` was missing (`rackv2-persist-v2-run1-missing-CCF_DIAG_DIR`).

### 4.6 `RackV2Objective1EndToEnd` — **PASS, `RACKV2_O1E2E_PASS checks=173 shots=24`**
Native, windowed Editor on private Xephyr `:84`, real OS key/click events from the real startup screen; 311 s, exit 0, no reaper. Covers: fresh Enlarged80 Rack V2 world (1,344 stems), walk and Stand Map, plan/edit/confirm five racks, field marking before approval (D-064/OPEN-2), spraying five entrances, Work Plan approve/execute, Annual Review, Objective 2, real `Save()`/`CanLoad()`/`Load()` persistence. **24 screenshots = 12 states × 1280×720 and 1920×1080** (`o1e2e/evidence/c01…c12`, `captures.txt`); I inspected c01 (720p) and c09 (1080p): real renders, correct HUD, entrance mark, minimap. Summary-line noise: the harness's own `Check` stack context during IMGUI repaints and one Editor search-indexing `ArgumentOutOfRangeException` at startup (also present in earlier accepted logs).

## 5. Diagnostic copies — NOT qualification evidence

Purpose: determine whether the verifier failures conceal a production problem. Each copy is staged from outside the repo through the same runner, HEAD unchanged. Diffs: `diagnostic-harness-copies/*.diff`. **None of this is a PASS for the pinned candidate.**

### 5.1 Four harness corrections to `WorkbenchRetirementVerification` and the outcome
| Diag run | Cumulative correction | Outcome |
|---|---|---|
| run1 | (1) scope the prerequisite assertion to buildables whose `requiredBuildable` *is* the workbench (reflection), assert exactly 4 and all met, and assert the Saw Pit dependents stay unmet; log every prerequisite edge | prerequisite checks **PASS**; next FAIL (check 41): *new-game save file does not contain workbench-01* (`checks=40`) |
| run2 | (2) before section C re-apply what `ForestBuildable.Awake` does on a scene reload (`SetRetiredFromStartingWorld(true)`, `RestoreBuiltState(false)`) — `ResetWorld` does not reload the scene, and production starts a new game by `StandaloneSessionMenu.Restart` → `LoadScene` | save omits `workbench-01` **PASS**; next NullReferenceException (`checks=42`) in the gate's own `Check` helper |
| run3 | (3) `Check` null-safe for `detail` (`CanLoad`'s `out problem` is null on success); (4) added baseline-equivalent Year-0/Year-3 hash logging | **`WORKBENCH_RETIREMENT_VERIFY_PASS checks=103`** |

In run1 the offending save file shows the harness cause directly: the "fresh" save contained `workbench-01 built:true` because section B had left that old-save state in place (`DIAG-workbench-gate-modified-harness-run1…/config/…/forest-save.json`).

### 5.2 Measurements from the passing diagnostic run (labelled hashes)
Run `DIAG-workbench-gate-modified-harness-run3`, `CurrentWorldHash` (geometry-aware), Enlarged80, same Editor/config:

| Label | Hash |
|---|---|
| **New game start (workbench retired)** | **`71481A025AB269B2`** (also `ENLARGED80_START_WORLD_CURRENT_HASH` in the Enlarged80 gate) |
| **New game Year 0** | `71481A025AB269B2` (= start) |
| **New game Year 3** (continuous = through a save/load) | **`044C4578E3E8DBA9`** (also `ENLARGED80_YEAR3_CURRENT_HASH`) |
| Baseline-equivalent start (workbench un-retired, same code) | `94589C749852F359` — **exactly the protected pre-#284 anchor** recorded by RACK-V2-03C/Combined (not the older `04A78188A6F8E6F9` quoted in the #284 handoff, which pre-dates Rack V2/save v24) |
| Baseline-equivalent Year 3 | `92F9635566BF6AFE` — **exactly the protected anchor** (not `081DB58CE3034648`) |
| Legacy40 fresh new game / baseline-equivalent (current-hash, informational) | `46D86442D8D5C9A0` / `771017CBC5C825F5` |

Reading: with the workbench un-retired the current code reproduces the protected Enlarged80 start and Year-3 anchors **exactly**, and the new-game hashes differ only by the omitted `workbench-01` `buildables` entry (proven in-gate: baseline capture minus that one entry equals the new-game hash; 1,344 identical tree ids/positions/diameters). The #284 "old expected labels" `04A78188…/081DB58C…` are stale and the handoff's re-baseline instruction should cite `94589C74…/92F96355…` as the pre-change anchors and `71481A02…/044C4578…` as the new-world labels — **a Manager decision; nothing here was rebaselined.**
Other passing diagnostic items (103 checks): old-save `built=false/true` retention with the four workbench-dependents met and the two Saw-Pit dependents unmet; fresh new-game `Save()`/`CanLoad()`/`LoadData()` round trip (six Site masks, same trees, same hash, still retired, re-save stable, file contains no `workbench-01`); negative cases (empty and null `buildables` retire the workbench — documented "absent means retired"); **Reference Future v1 preview enter/exit at years 0/20/50/100**: the reference workbench is restored while previewing (one `workbench-01`, milestone `verifiedFrozenWorld`), and after each exit the workbench is retired again with `activeRenderers=0 enabledColliders=0`, **exact new-game hash, same trees, six Site masks, buildable ids unchanged**; three resumed years equal three uninterrupted years with the workbench still retired; Legacy40 fresh world 336 trees, workbench retired, six masks, same 336 trees with and without the retired workbench.

### 5.3 Diagnostic `Enlarged80Verification` — passes through its 12th check, then fails its last one
With only the `version == ForestSaveData.CurrentVersion` correction the copy passes (12 check groups, including: new-world 1,344 trees / start hash `71481A025AB269B2` / Year-3 hash `044C4578E3E8DBA9` deterministic across save/load; v20 Enlarged80 save loads over Legacy40; v19 Legacy40 save into an Enlarged80 world; construction sites unavailable; **Reference preview from Enlarged80: frozen 40 m archive at original positions and the exact 80 m world restored**; Stand Map 256 cells A–P × 1–16; first-cycle intervention ordering) and then fails the final `scratch world returned to the deterministic Enlarged80 start` (`final=23CF72B3FDD59D2D` vs `71481A025AB269B2`). Two further attempted corrections (re-applying the `Awake` retirement state; re-setting `StandApproachExpected = true`) **did not change** that hash, so neither is the cause. Semantic diff of the two captures (`DIAG-enlarged80-gate-modified-harness-run4/config/…/diag-start.json` vs `diag-final.json`): `buildables` identical (six, workbench omitted in both); **one tree identity differs** (`P0604` in the start capture, `P1209` at (−2.05, 3.91) in the final one; 1,344 each) and the understorey cells differ accordingly. The state leaking into the final `ResetWorld` is **not proven** (a candidate is the player/start position moved by the earlier intervention steps); the workbench is not implicated by buildable content. **Unresolved verifier state-dependence; needs a Manager-directed repair, not a worker waiver.**

## 6. Required items — PASS / FAIL / NOT RUN

| # | Packet item | Status on pinned `5085e8af` | Note |
|---|---|---|---|
| 1 | Real compile/import evidence | **PASS** | exit 0, 0 `error CS`, 6.6 GiB Library |
| 2a | New Enlarged80 game: one policy-retired workbench, no visible/colliding/interactive site, capture omits ID, six independent Site masks | **PASS** | the pinned gate's checks before the early stop (§4.1) |
| 2a′ | Path/headland/clearing masks unchanged | **PASS (independent construction), no before/after count** | MFM08A track=104 clearing=1 headland=4; see §4.2 caveat |
| 2b | Old save with `workbench-01` loads; retained; no duplicate/lost id | **PASS** (checks 1–35) | the dependents sub-assertion **FAIL** (verifier defect); diag PASS |
| 2c | Reference preview then exit: restore/retire, no leak, frozen bytes | **NOT RUN on the pinned gate**; frozen bytes **PASS**; `REFERENCE_FUTURE_V1_PASS` (Legacy40 override) **PASS**; preview enter/exit with retirement **diag PASS only** | |
| 2d | Fresh new-game save round trip, workbench omitted | **NOT RUN on the pinned gate**; **diag PASS** | |
| 2e | Negative case (empty/no buildables retires) | **NOT RUN on the pinned gate**; **diag PASS** | |
| 3 | Independent six-vs-seven Site inventory | **PASS** (MFM08A pinned; the pinned lifecycle gate's "independent expected inventory is six", "six Site masks" and "seven Site masks" checks) | |
| 4 | Enlarged80: tree count/identities/positions/omissions and `CurrentWorldHash` at start and Year 3 vs historical labels | **start PASS (1,344 identical, hash measured); Year 3 NOT RUN on the pinned gate** (diag measured); `Enlarged80Verification` pinned **FAIL** (stale pin) | new labelled values §5.2; historical labels not silently replaced |
| 5 | MFM-08A Site/Track/Headland/Clearing parity | **PASS** | 57 checks |
| 6 | Narrow Rack V2 / Objective 1 / Work Plan non-regression, no new rack topology | **PASS** | MFM08A (149 tasks) + native O1E2E 173 checks |
| 7 | Historical compatibility: old v24 + older worlds, Legacy40, frozen Reference Future v1 | **PARTIAL**: frozen bytes PASS; Reference/Legacy40 PASS; synthetic old-save retention PASS; real v19/v20 saves **diag PASS only**; `RackV2PersistenceVerification` legacy40/coldstart **NOT RUN** (stale pin FAIL at entry) | no user save touched |
| 8 | Collider/interactability, hidden presentation, no ghost selection prompt | **PASS as assertions** (`activeRenderers=0 enabledColliders=0`, `ConstructionAvailable=false`); **native render of the workbench location / prompt: NOT RUN** | the candidate has no native workbench-site harness; O1E2E captures do not frame the site; I wrote none (that would change the candidate) |
| 9 | 1280×720 and 1920×1080 captures | **PASS** (Objective 1 states, 24 PNGs) | not a workbench-specific capture |

Tallies: pinned gates run on `5085e8af`: WorkbenchRetirement 35 PASS / 1 FAIL / remainder NOT RUN; MFM08A 57 PASS / 0 FAIL; Enlarged80 6 PASS / 1 FAIL / remainder NOT RUN; ScenarioReference (override) PASS; RackV2Persistence v2 0 PASS / 1 FAIL (entry); O1E2E 173 PASS / 0 FAIL. Diagnostic copies (not candidate): WorkbenchRetirement 103 PASS / 0 FAIL; Enlarged80 12 PASS / 1 FAIL.

## 7. Recommended verifier repair (for Manager repin — NOT applied by me)

Proposed verifier-only changes, exactly as exercised in the diagnostic copies (`diagnostic-harness-copies/WorkbenchRetirementVerification.cs.diff`, `Enlarged80Verification.cs.diff`):
1. `WorkbenchRetirementVerification`: scope the prerequisite assertion to workbench dependents (and assert Saw Pit dependents stay unmet when the Saw Pit is unbuilt); re-apply the `Awake` new-game state (`SetRetiredFromStartingWorld(true)`, `RestoreBuiltState(false)`) before the fresh-new-game save section; make `Check` null-safe. None weakens an assertion; the first is stricter than the original.
2. `Enlarged80Verification`: update the stale `version == 23` pin to `ForestSaveData.CurrentVersion` (24) and resolve the last-check state leak (cause unproven; do not simply delete the check without Manager review).
3. `RackV2PersistenceVerification` and any other gate pinned to v23 are separately stale (outside #287 scope; record for the programme).
After the repin, a re-run needs only a fresh lease and **no new cold import** if the lane and its Library are kept (this lane's `Library/` is intact at 6.6 GiB; I did not delete it).

## 8. Residual risks and Manager/independent-review notes

- **Not qualified.** Do not integrate `5085e8af` on this result. The production source is unchanged from `8b1946dc`; the measured production behaviour matched the policy in every diagnostic check, but a diagnostic pass is not a qualification pass.
- The new verifier code (`WorkbenchRetirementVerification.cs`) was authored without having been run against Unity before this session; it contained four defects. A separate independent review of the *repaired* verifier is advisable before it becomes an acceptance gate.
- Editor-driven package resolution (linux SDK 1.1.0 → 1.1.1) and Section Five material rewrites occurred; they are uncommitted worktree changes (left in place, not reset) and **must not** be committed with this result.
- Unresolved Enlarged80 final-check state leak (§5.3).
- No dedicated native workbench-site render and no native ghost-prompt capture (§6 #8).
- Reference-future/Legacy40 evidence used the documented Editor-only `CCF_STAND_GEOMETRY=0` override; no player save or `~/.config/unity3d/DefaultCompany/CCF` was touched (every run used an isolated `XDG_CONFIG_HOME` on Files).
- The runner-crash and stale-licence findings (§3) will recur for the next worker using these runners with the Hub licence.
- Memory: only the compile ran under a cgroup cap.

## 9. Evidence index (all under `/media/jer/Files/CCF/Verification/Workbench287Tier2-20261107/`)

| Path | Content |
|---|---|
| `lease-acquire.txt`, `lease-release.txt`, `lease-status-after-release.txt`, `lock_id.txt` | lease lifecycle |
| `disk-log.txt`, `disk-monitor-compile.log`, `disk-readings.txt` | storage readings |
| `compile.log`, `compile.start/.end/.exit`, `compile-attempt1-exit198-stale-home-licence.log`, `compile-attempts-note.txt` | cold import |
| `editor-rewrites-after-cold-import.diff`, `editor-rewrites-after-windowed-run-materials.diff` | Editor side effects on tracked files (not committed) |
| `workbench-gate/` | pinned lifecycle gate log, markers, runner-crash note |
| `mfm-coverage-world-gate/` | pinned MFM08A log (57 PASS) |
| `enlarged80-gate/` | pinned Enlarged80 log (stale v23 pin) |
| `scenario-reference-gate-run1-…/`, `scenario-reference-gate-legacy40-env/` | Reference Future v1 default (fails by design) and Legacy40-override PASS |
| `rackv2-persist-v2-run1-missing-CCF_DIAG_DIR/`, `rackv2-persist-v2/` | persistence gate attempts (stale v23 pin) |
| `o1e2e/` (`RackV2Objective1EndToEnd.log`, `result.json`, `summary-lines.txt`, `evidence/c01…c12-*-1280x720.png` / `-1920x1080.png`, `captures.txt`) | native Objective 1 gate |
| `DIAG-workbench-gate-modified-harness-run1-fails-at-check41/`, `…-run2/`, `…-run3/` | diagnostic lifecycle runs (103 PASS in run3) |
| `DIAG-enlarged80-gate-modified-harness-*` (several) incl. `-run4/config/…/diag-start.json` and `diag-final.json` | diagnostic Enlarged80 runs and semantic diff inputs |
| `diagnostic-harness-copies/` | the exact modified harness copies and their diffs against the repo files |
| `run_gate_287.adaptation.diff` | the three-line adaptation of the Objective 1 runner |
| `frozen-reference-sha256-before.txt`, `-after.txt` | frozen Reference Future v1 bytes (identical) |

## 10. Final state

- Lease: **released**; `unity-lock status` = **free**, no Editor PIDs. No Editor, Xephyr or openbox process of mine remains.
- Worktree: HEAD `5085e8af` unchanged; no staged harness or `.meta` under `Assets/ForestPrototype/`; uncommitted Editor rewrites (`.vscode/settings.json`, `Packages/manifest.json`, `Packages/packages-lock.json`, five Section Five `.mat`) are intentionally **not** part of the result commit. `Library/` (6.6 GiB) retained. Old clone `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` and the Files backups untouched.
- ZX20 free at hand-back: **21.62 GiB**.
