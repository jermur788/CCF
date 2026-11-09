# #353 — Reference Preview player-pose restore: implementation and focused Unity result

**9 November 2026. Status: HANDED-OFF (implemented + one focused Unity witness PASS). Not integrated, not accepted. Verdict and next step belong to Manager #200.**

Worker: Claude Sonnet 5.5 (`claude-sonnet-5-5`) via Claude Code in T3 Code, effort `low` (session setting). Lane: registered `U3`, `/media/jer/ZX20/CCF-U3`. Packet: `MANAGER-352-TO-353-REFERENCE-PREVIEW-POSE-CORRECTION-PACKET-20261109.md`.

## 1. Commits

| Role | SHA |
|---|---|
| Starting candidate C (unchanged, not rewritten) | `69c52906e973f6afa926c941cefa2bbc7b2a2412` |
| **C2 — tested source candidate** (one file, +11 lines) | `b82cb2405fa245ed49e48eb363b4b664b1d85d5e` |
| Report commit (docs only, on top of C2; not itself Unity-tested) | the branch tip carrying this file |

Branch: `fix/reference-preview-player-restore-20261109-v1`, created normally from exact C (no force/reset). Original branch ref `feature/scenario-one-player-test-non-ui-remediation-v1` still at `fc8042c3`.

## 2. Source change (only file: `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`, `EndReferencePreview`)

```diff
         if (player != null)
         {
+            // Move a CharacterController the same way MoveToReferenceView does:
+            // with it disabled, then re-enabled. Writing transform.position while
+            // it is enabled can leave its internal position at the preview
+            // viewpoint, and the next controller.Move would snap the player back.
+            CharacterController controller = player.GetComponent<CharacterController>();
+            bool controllerWasEnabled = controller != null && controller.enabled;
+            if (controllerWasEnabled)
+                controller.enabled = false;
             player.transform.position = previewPlayerPosition;
             player.RestoreLook(previewPlayerRotation, previewPlayerPitch);
+            if (controllerWasEnabled)
+                controller.enabled = true;
+            Physics.SyncTransforms();
         }
```

Mirrors the entry path (`MoveToReferenceView`: disable → write → enable). Differences from entry: the controller's *previous* enabled state is preserved rather than forced on; one `Physics.SyncTransforms()` is called. Saved look (`RestoreLook`), camera rotation, `previewReturnData` / world reload, and the reference world are untouched. Not modified: `ForestPlayer`, `ForestStartingStand`, harness, packages, scenes, materials, ProjectSettings, saves, seed/RNG, frozen Reference Future v1 / Legacy40, expected data.

## 3. Preflight and protected state

- Live: `unity-lock status` free, no Unity Editor PID (only Unity Hub); `active-tasks.md` (origin/main) lists #353 READY, no conflicting owner; #353 claim set `[RUNNING]` before any edit (comment receipt on issue).
- U3 tracked state was exactly the two known `.vscode/settings.json` hunks (same hunks as #351; only context width differs); nothing staged, no stash. Before-image, diff and hashes: `.../CCF-ReferencePreviewPoseFix-20261109/pre/`. Worktree-only `git restore --source=C -- .vscode/settings.json` of that one path, per the packet.
- Ignored licensed Innerverse pack (27 files incl. root `.meta`): hashes identical before and after, and identical to the #351 baseline. No clean, stash, reset, cache clearing or deletion. Warm Library, `Build/`, `Temp`, `Logs`, `UserSettings` preserved.
- Package pins before = after: manifest `f2ff5f95…966c2c1c`, packages-lock `f43d9894…b3e37e52b1`.
- Original registered Workbench, its eight protected files/index/Library, standalone recovery and frozen archive were not touched.

## 4. The one Unity run

- Entry (12:20:40Z): ZX20 `/dev/sdc2` (UUID 18D6-896E) free **36.17 GiB** (≥35); write I/O 0.0 MB/s over 6 s; no foreign Editor; licence scratch bytes identical to the Flatpak Unity Hub licence (not printed, not copied elsewhere).
- 20 s disk monitor (28 warn / 25 owner-only SIGTERM / 20 SIGKILL): 11 samples during the run, all `OK`, min 36.16 GiB, no actions. Final free 36.17 GiB.
- Lease: **unchanged deployed strict-clean `unity-lock`**, lane `U3`, branch `fix/reference-preview-player-restore-20261109-v1`, head C2, lock id `fa65ffb3918241f59efbf6134ed4d7e7`; released normally (`forced: False`), no Editor PIDs after.
- Command: `run_enlarged80.py c2-plain-1 --b3-mode plain --candidate-sha b82cb2405fa245ed49e48eb363b4b664b1d85d5e` (orchestrator `run_c2.sh`). **One run only**; no cold import, no duplicate plain runs, no authored-pin series.
- Result: runner `status=PASS`, exit 0, 172.8 s, `ENLARGED80_VERIFY_PASS checks=12`, 0 `error CS` lines, B3 evidence `EVIDENCE_COMPLETE`, 1600 B3 lines, 0 missing record kinds, `unavailable=0`, `slotModelFailures=0`, `archiveUnchanged=True`, `harnessFailed=False`, 14 resets, 35 pose records. The harness does not decide B3 (`b3Verdict=NOT_DECIDED_BY_HARNESS`).

## 5. Observed evidence versus the #351 failure signature

Player position (`kind=POSE`), authored start is `(-0.43, 0.03, 5.25)`:

| Record | #351 plain-1 (C, FAIL) | #353 c2-plain-1 (C2, PASS) |
|---|---|---|
| post-EndReferencePreview +2 | authored start | authored start |
| **reset13-entry** | **(-13, 0.08, -9)** — preview viewpoint | **(-0.43, 0.03, 5.25)** |
| reset13-pre-Generate / reset14 entry / pre-Generate | not extracted for this report | (-0.43, 0.03, 5.25) |

CharacterController at every pose record in this run: `ccEnabled=True`, bounds centre `(-0.43, 0.93, 5.25)` = position + 0.9 up (consistent with the transform; no stale offset), `autoSyncTransforms=False` (not enabled by this change).

- The #351 failing assertion, `scratch world returned to the deterministic Enlarged80 start`, **passed**; no `_FAIL` line.
- Identity pre and post identical: repo HEAD = candidate = C2; editor 6000.6.0f1; `Assets/Scenes/ForestTest.unity` git blob `acbb98cb…`, sha256 `6b1c4760…c785`; manifest and lock hashes as in §3.
- Frozen archive disk sha256 `789e3edd…b041fc` (git blob `c14c1caa…`) before = after; Legacy40 anchors before = after (`startingStand=A564039D9B7CE31D;0=A564039D9B7CE31D;20=F7DF7DAB53B6FD32;50=D5E75D6D21D631AC;100=7AD177B3CC2F73C7`).
- Reference preview check passed: "frozen 40 m archive at original positions; exact 80 m world restored".

## 6. Post-run state

- Tracked diff in U3: **only** `.vscode/settings.json`, the same two Unity-generated hunks (comma after `*.unity`; `CCF.slnx` → `CCF-U3.slnx`). Left in place, not reset (a new lease will be refused while dirty; restoring from C2 as before clears it).
- Staged harness `Enlarged80Verification.cs` and its `.meta` removed; no untracked non-ignored files; no other tracked, package, scene, material or archive change.
- U3 is left on the feature branch (HEAD advanced by this report commit), Library warm, lease free.
- Evidence (small; no licence bytes, no Library): `/media/jer/Files/CCF/Verification/CCF-ReferencePreviewPoseFix-20261109/` (`pre/`, `run/`, `result/` with `c2-plain-1.json`, `.log`, `SHA256SUMS.txt`, `run_c2.sh`).

## 7. Classification

| Level | Status |
|---|---|
| Implemented | **Yes** — C2, one file |
| Unity-verified | **Yes, narrowly** — one plain 80B/B3 harness run on Linux Unity 6000.6.0f1, batch mode, PASS |
| Integrated | **No** — nothing merged or PR'd; branch only |
| Accepted | **No** — Manager's decision |

## 8. Limits — what was NOT verified

- One run only. A single PASS cannot show the previous failure was deterministic in the way 2/2 FAILs did; the contrast rests on #351's two plain FAILs plus this one PASS, same Library/licence/lane, C → C2 as the only source change.
- The mechanism (stale CharacterController position corrected by disable/enable and `SyncTransforms`) remains a source-based inference; this run shows the symptom is gone, not which line fixed it. The two parts (toggle vs. sync) were not separated.
- Harness-driven (batch, no rendering, no real input). No interactive playtest: real mouse/keyboard `Update`/`controller.Move` timing, a real player pressing H/Map and walking away, and look/pitch/camera restoration were not exercised. The B3 recorder has no post-H pose record other than the +0/+1/+2 and reset entries.
- Full A–O (0/15), Tier-2 suites, the three-run diagnostic, authored-pin series, Windows/Linux player build, save/load persistence checks were not run, by instruction.
- Compile is evidenced by the successful run and zero `error CS` lines in the log; no separate compile-only gate.
- Not examined: other callers/paths that restore a player pose (outside scope).

## 9. For Manager #200

Recommended next (Manager's call): review the 11-line diff; decide whether to run the standard integration gates and whether to take C2 forward; keep the `ForestStartingStand` current-player clearance-anchor policy question separate. No product-policy change was made.
