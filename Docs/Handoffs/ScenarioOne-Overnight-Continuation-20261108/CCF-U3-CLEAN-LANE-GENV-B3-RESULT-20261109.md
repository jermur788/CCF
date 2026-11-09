# CCF-U3 clean-lane G-env / Tree B3 result (#351)

**Outcome (final, after retry): BLOCKED at S4 after a successful cold import/compile — Unity rewrote tracked `.vscode/settings.json`. Compile: PASS (exit 0, no `error CS`). B3 diagnosis: UNKNOWN — 0 of 3 witness runs executed.**

## 0. Retry (user supplied a new licence; ~10:48–11:00 UTC) — supersedes §1 S4–S5 and §3

- **Licence cause:** Unity Hub is a Flatpak and wrote the new licence to `~/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses/UnityEntitlementLicense.xml` (10:47Z, sha256 `9f359ae8…`). The runner and my scripts copy from the old `~/.config/unity3d/Unity/licenses` (unchanged since 15 Sep, `24ebbb7a…`) and only if absent. I copied the new file **only into the lane's isolated scratch config** (`Build/Enlarged80/config/...`), replacing my own stale copy; the user's home licence was not touched. **The B3 runner `run_enlarged80.py` will still reuse whatever is in that scratch config, but any fresh lane would copy the stale `~/.config` file** — worth fixing before a re-run elsewhere.
- Re-claimed #351 RUNNING (`issuecomment-6738133217`); S1 repeated: UUID `18D6-896E`, ZX20 42.73 GiB, I/O avg60 4.1 %, new monitor started first (`run2/disk-monitor.log`, now observed a **real Editor PID** 91669, 20 s samples, min free 36.16 GiB), then lease `c5fb94fc36aa496d8d2aec8a5ac6d00b` via the unchanged `unity-lock` (HEAD already C, tracked clean).
- **Cold import + compile (10:50:34–10:59:45Z, ~9 min): licence valid, exit 0, "Exiting batchmode successfully", no `error CS`, 65 packages registered, ~6,090 assets imported.** Library = **6.6 GiB** (matches the 6–7 GiB estimate); ZX20 36.18 GiB free after.
- `Packages/manifest.json` (`f2ff5f95…`) and `packages-lock.json` (`f43d9894…`) **unchanged**. Staged harness: none (compile-only; no `-executeMethod`).
- **STOP condition hit:** tracked `.vscode/settings.json` modified by Unity's editor integration (`run2/vscode-settings-drift.diff`): trailing comma removed after `"*.unity": "yaml"` and `dotnet.defaultSolution` `CCF.slnx` → `CCF-U3.slnx`. `AGENTS.md` lists that file as tracked shared config. It looks benign (Unity's VS Code integration regenerates it per lane), but it is an unexpected tracked mutation outside the packet's expected set, so I stopped rather than endorse or revert it. It also makes the lane non-strict-clean, so a fresh lease would be refused until Manager decides (revert via `git checkout -- .vscode/settings.json` is Manager's call; I did not).
- **B3 witness: not run** (0 plain, 0 authored-pin). Candidate/scene/archive/Legacy40 anchors, tree identity, pose and CharacterController observations: none exist.
- Lease released (owner-matched, not forced), lock `free`, monitor stopped, no Editor left.
- **CCF-U3 now:** detached at C; tracked dirty = `.vscode/settings.json` only; new ignored `Library/` (6.6 GiB), plus earlier `Build/`, `Temp/`. Not cleaned.
- Evidence: `run2/` and `SHA256SUMS-run2.txt` in the evidence directory.

**Next action (one):** Manager rules on `.vscode/settings.json` — accept as a known benign Unity side effect and re-release a B3-only run (lease needs the file reverted or the lane's strict-clean check satisfied; the 6.6 GiB Library is already warm, so no second cold import), or decline.

---

## Earlier first attempt (superseded by §0 above)

**Outcome: BLOCKED at the Unity licence check (S4). B3 diagnosis: UNKNOWN — not attempted. Compilation: UNKNOWN — Unity never reached import.**

Worker: Claude Code (Sonnet 5.5, `claude-sonnet-5-5`) via T3 Code, local machine, 9 Nov 2026, 10:23–10:27 UTC.
Binding packet: `MANAGER-350-TO-351-CLEAN-LANE-UNITY-DECISION-PACKET-20261109.md` (origin/main `9d14d777`).
Evidence (small text only): `/media/jer/Files/CCF/Verification/CCF-U3CleanCandidateWitness-20261109/` (`SHA256SUMS.txt` covers 19 files).

## 1. What happened

| Step | Result |
|---|---|
| S0 ownership | PASS. CCF-U3 registered in `lanes.json` (`U3`, `unity:true`), listed as a worktree of the shared store, branch had no second checkout, no owner in `active-tasks.md`, lock free, no Editor. Claim #351 set `[RUNNING]` (comment `issuecomment-6737649554`). |
| S0 cleanliness | Tracked tree clean. Ignored items present (see §4): `Assets/InnerverseInteractive/` (5.2 MB licensed pack, Git-excluded; U1/U2 carry the same), `Logs/`, `UserSettings/`, `*.csproj`, `CCF-U3.slnx`. No `Library/`. C tracks nothing at any of those paths, so a checkout could not overwrite them. I treated them as preserved, non-blocking content. **Manager may disagree with that reading** — the packet says "valuable ignored files → STOP"; the pack is re-importable from the user's Asset Store cache per `RightsAudit.md`, but I did not verify that. |
| S1 disk + monitor | PASS. Device UUID `18D6-896E` (matches registry). ZX20 free **42.89 GiB** at entry (≥35, ≥27). I/O PSI avg60 5.6 % when sampled (it was 11.8 % a minute earlier, settling after my worktree creation on Files); only Firefox running. Monitor (`disk_monitor.py`, 20 s interval, alert 28 / SIGTERM 25 / SIGKILL 20 GiB, signals only PIDs in an owner pidfile fed by `pid_registrar.py`, which lists only Unity processes whose `-projectPath` is CCF-U3) was **proven with dummy processes** before use: SIGTERM at the stop threshold, SIGKILL of a SIGTERM-ignoring process at the kill threshold, an unrelated process untouched (`monitor-proof/`). **Not proven against a real Editor** — it never ran with one. |
| S2 transition | PASS. Start: branch `feature/scenario-one-player-test-non-ui-remediation-v1`, HEAD `fc8042c37939838a4a525b75e895016698b1cb80`. Ordinary `git checkout --detach 69c52906e973f6afa926c941cefa2bbc7b2a2412`, no force. After: HEAD = C, tracked clean, ignored set unchanged. C is **not** an ancestor of origin/main (it is on `origin/feat/workbench-111-b3-evidence-candidate-20261108-v1`), as expected for a candidate. |
| S3 lease | PASS. Unchanged deployed `unity-lock` accepted lane `U3`, `--branch HEAD`, `--head C`. Lease `6d6ff8efa7214b6b83cb3194eb9719c3`, task `CCF-351-U3-CLEAN-LANE-GENV-B3`, holder `Claude Code (Sonnet 5.5) via T3 Code #351`, pid 76865 (the monitor), ETA 13:25:46Z. No lock script change, no proposed lock deployed. |
| S4 cold import | **BLOCKED.** Editor `6000.6.0f1` batch launch (`-quit`, no `-executeMethod`, 8 GiB cap, isolated config under `Build/Enlarged80/config` with a copy of `~/.config/unity3d/Unity/licenses`) exited **198 within ~2 s**. |
| S5 shutdown | Lease released with matching owner/task/lock-id (not forced); lock verified `free`; monitor stopped; no Unity or monitor process left; ZX20 free **42.73 GiB** at exit. |

## 2. The blocker (verbatim from `run/import-compile.log`)

```
[Licensing::Module] Error: Access token is unavailable; failed to update
[Licensing::Client] Error: Code 404 while processing request (status: Found 0 entitlement groups and 0 free entitlements matching requested entitlement ids)
[Licensing::Module] Error: 'com.unity.editor.headless' was not found.
No valid Unity Editor license found. Please activate your license.
[ExitDontLaunchBugReporter] ... return code 198
```

The licence file used is `UnityEntitlementLicense.xml`, last modified 15 Sep 2026 14:16 (host and lane copies identical, 5434 bytes). This matches the earlier memory note of a stale-host-licence → exit 198 trap (#88). I did **not** attempt to re-activate, sign in, change Unity Hub state or edit licence files; those are the user's to do. Cause beyond what the log states (e.g. token expiry vs. missing headless entitlement) is UNKNOWN.

A harmless side note: `VBCSCompilerPreWarmer: Couldn't pre-warm ... /tmpUnityVBCSPreWarm_*.cs` appears at start-up. It is unrelated to the exit.

## 3. Requested observations

| Item | Status |
|---|---|
| Source HEAD actually used | `69c52906e973f6afa926c941cefa2bbc7b2a2412` (detached, CCF-U3) |
| Editor / licence | Editor launches; **licence invalid → exit 198** |
| UPM compile | UNKNOWN — not reached |
| Material / package changes | None observed. `Packages/manifest.json` sha256 `f2ff5f95…c2c1c`, `packages-lock.json` sha256 `f43d9894…e52b1`, identical before/after the launch; tracked porcelain empty after |
| Frozen archive / Legacy40 | Unchanged (tracked porcelain empty). Not Unity-verified |
| B3 runs (2 plain + 1 authored-pin) | **0 of 3 run.** No tree, anchor, pose, player or CharacterController observations exist. No `--candidate-sha` run was started. The `CaptureData()` observer-effect question stays UNKNOWN |
| Candidate / archive IDs | Candidate C above; no archive hash was computed (nothing ran) |
| Original warm Workbench, 8 protected files, deployed lock | Not touched |
| Cleaning / security tests / Tier-2 / build | None |

## 4. Leftovers on CCF-U3 (documented, not cleaned)

- Detached at C (was the feature branch at `fc8042c3`; branch ref untouched, so returning is a plain `git checkout feature/scenario-one-player-test-non-ui-remediation-v1`).
- New ignored: `Build/` (2.0 MB, includes a copy of the user's Unity licence directory) and `Temp/`.
- Pre-existing ignored (unchanged): `Assets/InnerverseInteractive{,.meta}`, `Logs/`, `UserSettings/`, `Assembly-CSharp*.csproj`, `CCF-U3.slnx`. `Logs/` and `UserSettings/` may carry a few bytes of new writes from the launch; not enumerated.
- No `Library/`, no staged harness, no `.meta` under `Assets/ForestPrototype/`.
- Reporting worktree `/media/jer/Files/CCF/Operations/CCF-U3CleanCandidateWitness-20261109` (branch `docs/ccf-u3-clean-candidate-witness-20261109-v1`); Files had ~11 GiB free.

## 5. Next action (one)

**User re-activates the Unity licence for this host** (Unity Hub sign-in / manual activation so that a headless batch launch passes the licence check). Then Manager may re-release #351 unchanged: CCF-U3 is already at C and clean, so the re-run starts at S1 (re-check disk, re-prove monitor on the real Editor, re-acquire the lease). Until then no Unity run can produce B3 evidence on any lane. Stopping for Manager #200.
