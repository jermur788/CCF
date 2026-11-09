# CCF-U3 B3-only follow-up result (#351)

**Outcome: HANDED-OFF. 3 of 3 witness launches completed with EVIDENCE COMPLETE (independently verified inline). No B3 causal verdict is made here — that is Manager's.**

Worker: Claude Code (Sonnet 5.5, `claude-sonnet-5-5`) via T3 Code, local, 9 Nov 2026, 11:10–11:23 UTC.
Binding packet: `MANAGER-351-COMPILE-PASS-VSCODE-RESTORE-AND-B3-ONLY-RELEASE-20261109.md`.
Evidence (no licence/token content): `/media/jer/Files/CCF/Verification/CCF-U3CleanCandidateWitness-20261109/b3-only/` (`SHA256SUMS.txt`, 26 files).

## 1. Gates

| Gate | Result |
|---|---|
| G0 live checks | PASS. Lock free, no Editor/Hub competitor, UUID `18D6-896E`, I/O avg60 0.7 %, ZX20 **36.18 GiB** (≥35), CCF-U3 HEAD = C `69c52906…`, Library present, no staged changes, report lane clean at `02313907`. Package pins `f2ff5f95…` / `f43d9894…` as expected. Scratch licence fingerprint equals the Hub Flatpak licence (`9f359ae8…`, first 8 bytes of hash only; contents never read out). Claim set `[RUNNING — B3-only continuation]`. |
| Pack baseline | `Assets/InnerverseInteractive/` + `.meta`: **27 files, 225,793 bytes**, per-file SHA-256 in `pack-baseline.sha256`. |
| `.vscode/settings.json` diff gate | PASS. Diff vs C's blob (`9b835a42…`) was **exactly two hunks**: `"*.unity": "yaml",`→`"yaml"` (trailing comma) and `dotnet.defaultSolution` `CCF.slnx`→`CCF-U3.slnx`. Before-image saved (`vscode-before-restore.json`, sha256 `2ccabfd4…`; C blob `04d629e4…`). |
| G1 restore | `git restore --source=C --worktree -- .vscode/settings.json` (that path only). Bytes then `cmp`-equal to C's blob; tracked porcelain empty; nothing staged; pack hashes unchanged. |
| G2 monitor + lease | Monitor (20 s samples; 28/25/20 GiB) started **before** any Editor; it listed the real Editor PIDs each run. Lease `bc04a2ac9a1c49e586ad775c8f0e9ce1` (task `CCF-351-U3-B3-ONLY`, lane `U3`, `--branch HEAD --head C`) from the **unchanged** `unity-lock`, acquired against the clean lane. Min free during runs 36.17 GiB. |
| G3 close | Lease released (owner-matched, not forced), lock `free`, monitor stopped, no Editor/monitor process left, ZX20 **36.17 GiB** free. |

## 2. B3 launches (all `--candidate-sha 69c52906e973f6afa926c941cefa2bbc7b2a2412`, one lease, warm Library, no cold import, serial)

| Label | Mode | Gate (80B) | Runner witness | UNAVAILABLE | Time |
|---|---|---|---|---|---|
| `b3-plain-1` | plain | **FAIL** (`scratch world returned to the deterministic Enlarged80 start`) | EVIDENCE_COMPLETE | 0 | 136.5 s |
| `b3-plain-2` | plain | **FAIL** (same message) | EVIDENCE_COMPLETE | 0 | 151.6 s |
| `b3-pin-1` | authored-pin | **PASS** | EVIDENCE_COMPLETE | 0 | 115.2 s |

The harness compiled on first use in all runs (no `error CS`). The 80B FAIL in plain mode is the gate's own assertion, i.e. the behaviour under diagnosis, not an infrastructure failure, so I did not stop on it. I did not rerun any launch.

**Inline verification (script, not the runner label), all three runs:** pre and post `IDENTITY` records have `repoHead == candidate == C`; Editor `6000.6.0f1`; scene `Assets/Scenes/ForestTest.unity` gitBlob `acbb98cb…` sha256 `6b1c4760…` identical pre/post and across runs; both package pins pre/post equal to the CCF-U3 file hashes; Reference-Future archive `diskSha256 789e3edd…` (git blob `c14c1caa…`) identical before/after in every run, `archiveUnchanged=True`; Legacy40 anchors identical before/after (`A564039D…;0=A564039D…;20=F7DF7DAB…;50=D5E75D6D…;100=7AD177B3…`); every required record kind present. Disqualifiers: none.

## 3. Diagnostic observations (facts only)

- Plain run 1 and plain run 2 are **identical** in per-reset tree id hashes, world hashes, anchor hashes and entry poses (reproducible).
- **Resets 1–12: plain and pinned are identical** (same tree-id hash, count, world hash, 112-anchor list hash `2e737158…`). Player is at the authored start `(-0.43, 0.03, 5.25)` throughout.
- The reference preview moves the player to `(-13, 0.08, -9)` and the end-preview restores `(-0.43, 0.03, 5.25)` (all three runs; `post-EndReferencePreview+0/+1/+2`). But at **reset 13 entry the player is at `(-13, 0.08, -9)` in all three runs**.
- **Reset 13/14 diverge:** plain, player stays at `(-13,-9)` through `pre-Generate`; tree-id hash `d7ad89b0…` (vs baseline `b771d4e1…`), anchor list `c159bc88…` (vs `2e737158…`), world hashes `A223B45C…` / `A71A4579…`. Pinned, the harness moves the player to the authored start at `reset13-after-authored-pin` before Generate, and tree ids, anchors and world hash return to the baseline (`71481A02…`; reset 14 `F5E3E114…`).
- CharacterController state is the same in every record: enabled, centre `(0,0.9,0)`, size `0.6×1.8×0.6`, `autoSyncTransforms=False`.
- Counts: plain 14 resets / 35 pose records; pinned 14 / 49 (extra `after-authored-pin` records).

**Interpretation limits.** The data show the generated trees and clearance anchors differ when the player is not at the authored start at Generate, and that the pin removes the difference. That is consistent with player-position-dependent generation, but this run does not establish *why* the player is at `(-13,-9)` at reset 13 entry in plain mode, whether normal play can reach that state, or whether `CaptureData()` observation affects values (source-inferred risk, runtime effect UNKNOWN; reset-13/14 world hashes differ from earlier ones even in the pin run for reset 14, which I did not explain). B3 verdict: **not decided by me or the harness**.

## 4. `.vscode/settings.json` side effect

Unity re-wrote the same two hunks after the first launch (checked after run 1 and after run 3; identical to the before-image diff, `vscode-diff-after-*.diff`). No third difference. Per packet I did not restore it mid-lease and the runner did not need a second lease. **Left dirty at handoff** (`vscode-after-run.json`).

## 5. End state

- CCF-U3: detached at C; tracked dirty = `.vscode/settings.json` only (known two hunks); nothing staged; `manifest.json`/`packages-lock.json` unchanged; no harness `.cs`/`.meta` left in `Assets/ForestPrototype/`.
- Licensed pack + `.meta`: byte-identical to baseline (27 files). Ignored `Build/` (now also holds the three B3 `.log/.json`), `Library/`, `Logs/`, `UserSettings/`, `Temp/` untouched. Original warm Workbench, global lock, other lanes untouched. No cleanup, no build, no integration.

## 6. Next action (one)

Manager reviews §3 and decides the B3 verdict / follow-up (e.g. why the player is at `(-13,-9)` at reset 13 entry in plain mode). CCF-U3 needs `.vscode/settings.json` restored (same one-file restore) before any further strict-clean lease.
