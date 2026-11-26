# Phase 2B-1 — C1 gate-config delta: independent review

- **Date:** 2026-11-26
- **Reviewer role:** independent read-only reviewer (fresh thread; did not implement or coordinate C1).
- **Status of this document:** an independent opinion. It does **not** authorise a merge; the integrator decides.

## Reviewed objects

| Role | SHA |
|---|---|
| Base for the delta (`integration/2d-sim` trunk, `HEAD^2`) | `00dcd196158c8df764824b2e9ceb30849ef27382` |
| C1 config commit (`HEAD^1`; parent `e97b3780`, the Phase 2B-1 feature HEAD) | `5913cf83ddff3bbe63c5b4cba9b665ce8dc8cf20` |
| Merge under review (`HEAD`) | `f890ba8df364f88e8724b423b0289a61f35b0348` |

Delta range: `00dcd196..f890ba8d` (gate scope: `-- Tools/MultiRackSitkaP1Lab/build/`). The C1 commit by itself is `e97b3780..5913cf83`, which is also exactly the same three files.

Worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2B-C1-Review-20261126`, branch `review/multi-2d-phase2b-c1-gate-retirement-20261126-v1`.

## Guard

`GUARD OK`: clean `git status --porcelain`. `HEAD` = `f890ba8d…`, `HEAD^1` = `5913cf83…`, `HEAD^2` = `00dcd196…`. After `git fetch origin`, `origin/main` = `bdeb6a324acaf4129c10fb0846faac8b64974360`.

## Method (what was actually run)

1. `git status --porcelain`, `git rev-parse HEAD HEAD^1 HEAD^2`, `git fetch origin`, `git rev-parse origin/main`.
2. `git diff --stat|--name-only|--name-status 00dcd196..HEAD` (whole tree and `build/`); `git diff 00dcd196..HEAD -- build/` read in full; `git diff --stat e97b3780 5913cf83`; `git diff 5913cf83 HEAD` for the three files, to see what the trunk side brought in.
3. `git diff --quiet 00dcd196 HEAD --` for `2d-gate.sh`, `move-proof.py`, `frozen-anchors.json`, `protected-paths.txt`, `determinism-allowlist.txt`, `rng-salts-patterns.json`.
4. I read `2d-gate.sh` lines 600–740 (suite count checks) and 1181–1218 (G2 sub-checks 9 and 10), `fail_stage()` and `GATE_DATA_DIR`. I also read `selftest.sh` `run_full_case()`, `mut_move_proof()` and the other S0c mutators, and the head of `move-proof.py` with all of its `sys.exit` paths.
5. `grep -rn gateEnforce Tools/MultiRackSitkaP1Lab/`; `grep -c '^run_full_case '` at base and HEAD; a field diff of `(id, exit, marker)` for every `run_full_case` line; a conflict-marker grep; a key-by-key JSON diff of the move table at base and at HEAD.
6. `json.load` on every `build/2d-gate/*.json`; `bash -n` on `selftest.sh` and `2d-gate.sh`.
7. Q3: ran `move-proof.py` in a scratch clone (`git clone --shared` to `$HOME/tmp/c1rev/scratch-clone`), then removed the clone. Commands and results are below.
8. `grep -c 'T.Add('` in `tests/Core.Tests/Phase2BEconomyTests.cs`; checked that `Program.cs` registers it; ran `git merge-base --is-ancestor` on the SHAs named in the `retirement` note.

### Q3 commands (scratch clone, outside the repo)

```bash
C=$HOME/tmp/c1rev/scratch-clone
git clone --quiet --shared --no-checkout "$WT" "$C" && cd "$C"
git checkout --quiet f890ba8df364f88e8724b423b0289a61f35b0348
T=Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-move-table.json
MP=Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py
T0=47b27a8d3873b32726f315ce1182118da38c50b1
# [A] gateEnforce=true (python json rewrite, indent=2), commit, untampered
python3 "$MP" --repo . --base $T0 --head HEAD --table "$T"
# [B] + C22 tamper: insert "            // selftest move-proof block tamper" after first "        lock (sync)" in
#     MultiAreaContinuation.Commands.Crop.cs, commit
python3 "$MP" --repo . --base $T0 --head HEAD --table "$T"
# [C] checkout --detach 00dcd196 (table there already has gateEnforce=true), untampered
python3 "$MP" --repo . --base $T0 --head HEAD --table "$T"
# [D] 00dcd196 + same C22 tamper, commit
python3 "$MP" --repo . --base $T0 --head HEAD --table "$T"
# [E] checkout --detach f890ba8d as committed (gateEnforce=false)
python3 "$MP" --repo . --base $T0 --head HEAD --table "$T"
rm -rf "$C"
```

| Run | State | rc | Last line |
|---|---|---|---|
| A | HEAD, `gateEnforce=true`, untampered | **1** | `FAIL [core]: contiguous block core-cmd-clear (T0 lines 626..673) not found in order in …Commands.Clear.cs from line 7` |
| B | HEAD, `gateEnforce=true`, C22 tamper | 1 | **same `core-cmd-clear` failure** (the Crop tamper is never reached) |
| C | trunk `00dcd196`, untampered | 0 | `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1263 non-empty lines); core(7 files, 8 blocks, 1337 non-empty lines)` |
| D | trunk `00dcd196`, C22 tamper | 1 | `FAIL [core]: contiguous block core-cmd-crop (T0 lines 457..502) not found in order in …Commands.Crop.cs from line 7` |
| E | HEAD as committed (`false`) | 1 | `core-cmd-clear` failure: `move-proof.py` itself ignores `gateEnforce`; only `2d-gate.sh` reads it |

The scratch clone was removed, and the review worktree's `git status --porcelain` was still empty afterwards.

## Q1–Q9

| # | Result | Evidence |
|---|---|---|
| Q1 Scope discipline | **PASS** | `--stat … -- build/` shows exactly 3 files (+23/−6), all `M`. `--name-only 00dcd196..HEAD` has nothing under `Assets/`, `Packages/`, `ProjectSettings/`, `src/Ui/wwwroot/`, `src/Core/Phase1/`, `fixtures/` or `Docs/Project/`. The other paths are Phase 2B-1 feature files from `e97b3780`, which is an ancestor of `HEAD^1`: `src/Core/MultiArea/**`, `tests/Core.Tests/**`, `specs/multi-2d-phase2b-economy-v1/**` and one `Docs/Handoffs/` file. `2d-gate.sh` and `move-proof.py` are unchanged (`git diff --quiet` = 0). `e97b3780..5913cf83` is exactly the same 3 files. |
| Q2 Retirement is the switch, not a deletion | **PASS** | `s0a-move-table.json:4` `"gateEnforce": false`; the parts and blocks are unchanged (the key diff shows only `gateEnforce` and `retirement`). The only gate consumer is `2d-gate.sh:1184-1197`: the `if [[ "$enforce" == "1" ]]` wraps only the move-proof call and its `fail_stage`. Sub-check 10 (S0c `module-graph.mjs`/`js-reconstruct.mjs`, `2d-gate.sh:1199-1215`) and `emit_marker G2 PASS` sit outside that block, so all earlier G2 sub-checks are unaffected. |
| Q3 Machinery works when enabled | **PASS with exception** (tamper detection) / **FAIL** (untampered pass at HEAD) | Tamper → rc 1 at HEAD (B) and at trunk (D, failing on `core-cmd-crop`). An untampered clone with `gateEnforce=true` does **not** pass at HEAD (A, rc 1 on `core-cmd-clear`) and passes only at trunk `00dcd196` (C). This is the Phase 2B-1 drift that motivated C1, so the brief's "untampered passes at HEAD" expectation cannot hold for this HEAD. |
| Q4 C22 non-vacuous | **FAIL** (tamper discrimination) / PASS (wiring) | (a) `selftest.sh:443-453` tampers Crop.cs ✔; (b) `selftest.sh:454-463` forces `gateEnforce=True` in the clone only and commits it ✔; (c) `selftest.sh:524` expects rc `1` and `S0a move-proof failed` ✔. The marker is emitted only by `2d-gate.sh:1193`, so C22 **would fail** if the gate stopped running move-proof. It still discriminates the gate-wiring regression. However, run A shows that with `gateEnforce=true` alone, HEAD already fails on `core-cmd-clear` before the Crop block is checked. **C22 would pass with no tamper at all**, so its expected failure comes from pre-existing Phase 2B-1 drift, not from the planted tamper (finding F1). |
| Q5 No other guard weakened | **PASS** | `frozen-anchors.json`, `protected-paths.txt`, `determinism-allowlist.txt`, `rng-salts-patterns.json` and `2d-gate.sh` (G0–G7 and B logic) are unchanged. In `baseline-counts.json` only `suites.core.minPassed` and a new `note` key changed; `save`, `uiSmoke*`, `cliSafety`, `buildOutputSafety`, `p0Isolation` and `payloadBudgetChecks` are identical. `selftest.sh` changed only in the `mut_move_proof` body and the C22 description string. There are 16 `run_full_case` invocations at both base and HEAD, and the `(id, exit, marker)` fields are identical for all 16. |
| Q6 Re-baseline sound and tightening | **PASS** (291/291 reproduction NOT TESTED) | `2d-gate.sh:615` fails if `failed > maxFailed or passed != total or passed < minPassed`, which is a floor plus a separate all-pass and max-failed check. `Phase2BEconomyTests.cs` has 9 `T.Add(` registrations and `tests/Core.Tests/Program.cs` adds `Phase2BEconomyTests.Register();`, so 282 + 9 = 291. Raising the floor strictly tightens the check and cannot mask a regression: a count of 282–290 now fails, where it previously passed. I did not run `dotnet`, so the 291/291 figure is the prior reviewer's and I did not reproduce it here. |
| Q7 Merge preserved S0c | **PASS** | `git diff 00dcd196..HEAD -- src/Ui/` is empty. `module-graph.mjs`, `js-reconstruct.mjs`, `s0c-hook-inventory.json` and `s0c-renderer-callsite-map.json` are present, and `src/Ui/multi/` holds `api.js commands.js dialogs/ dom.js main.js map/ modes/ multi.css multi.html panels/ store.js`. At HEAD, `selftest.sh:523` has S0c's C21 (multi.css), `:524` has C1's C22, and `:530-532` have C27/C28/C29. There are 0 conflict markers. HEAD's `selftest.sh` matches the trunk version except for the C1 hunks. |
| Q8 Notes are true | **PARTIAL** | The notes do not claim the check runs by default (`reason` explains why it cannot), and they correctly name C22 as force-enabling it in a disposable clone. The baseline `note` (`baseline-counts.json:10`) is accurate. But `retained` (`s0a-move-table.json:9`) says "so the mechanism stays covered", which overstates: per F1, C22 covers the gate wiring, not move-proof's tamper detection. Separately (F2), the SHAs `aa5f1850`, `742bd904` and `158cdd69` cited as "reviewed and merged" are the review-doc commits and are **not** ancestors of HEAD. The underlying S0a/S0b/S0c work is evidently present, so this is imprecise rather than false. No secrets, tokens or URLs were added (grep found none). |
| Q9 Anything else that weakens the gate | **F1 only** | No active guard became unable to fail and no threshold became unreachable. The only weakened assertion is the selftest coverage of move-proof's detection logic (F1). Because move-proof no longer gates trunk, this weakens no active guard. Its only effect is that a future regression in `move-proof.py` would go undetected until someone re-baselines the table and re-enables it. |

## Findings

- **F1 (selftest discrimination; moderate). `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh:443-466`, `:524`.**
  - **Problem:** At HEAD, force-enabling `gateEnforce` is enough by itself to make move-proof fail on `core-cmd-clear`, because Phase 2B-1 edited the moved Clear, Fell, Prune, Planting and Plan blocks. The planted Crop tamper is never evaluated, so C22's expected `rc=1` and `S0a move-proof failed` come from pre-existing drift.
  - **What C22 still proves:** the gate runs move-proof when it is enabled and fails closed through `fail_stage G2 1`.
  - **What it no longer proves:** that move-proof detects a tampered moved block.
  - **Fix options:** run move-proof in the clone with `--head 00dcd196` (the last trunk commit where it passes, run C) plus a tamper commit on that line. Or keep C22 as a wiring check, add a direct `move-proof.py` pass/fail pair against `00dcd196`, and reword the description.
- **F2 (documentation precision; nit). `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-move-table.json:7-9`.**
  - **Problem:** `reason` cites review-document commits (`aa5f1850`, `742bd904`, `158cdd69`) as if they were the merges, but none of them is an ancestor of HEAD. `retained` says "the mechanism stays covered".
  - **Fix:** say "covered for gate wiring only" or cite the actual merge commits. This needs no change to gate behaviour.

## LIMITS (not executed)

- I did not run `2d-gate.sh` or `selftest.sh`; the brief prohibited it and there is no `.p0ref` seed in this worktree. As a result, C22's end-to-end behaviour (including whether it reaches G2 sub-check 9 after G1 and the suite checks), every other selftest case, and the full G0–G7/B pipeline at HEAD are **NOT TESTED**. My F1 inference rests on direct `move-proof.py` runs A and B, which use the same invocation the gate makes at `2d-gate.sh:1191`.
- I did not run `dotnet` or any test suite, so the Core `291/291` count is **NOT TESTED** here.
- I did not run Stage B (browser parity) or the S0c node checks, so they are **NOT TESTED**.
- I did not verify the S0a/S0b/S0c merge commits by SHA, only that the review-doc SHAs are not ancestors.
- I made no Unity, runtime or rendered verification; none is applicable to this delta.

INTEGRATION DECISION: APPROVED WITH CONDITIONS — (1) the integrator must not record C22 as proving move-proof tamper detection: at HEAD, C22 is satisfied by the pre-existing `core-cmd-clear` drift (F1). Either accept and document it as a gate-wiring check, or fix C22 to run against `00dcd196` in a follow-up bounded change, and correct the `retained`/`reason` wording (F1/F2) at the same time. (2) The full `2d-gate.sh` and `selftest.sh` must pass green (including C22 = PASS and Core ≥ 291/291) as the integrator's post-merge duty before anyone calls this integrated.

## R2 — F1/F2 wording remediation re-review (2026-11-26)

Fresh independent reviewer. I did not write the C1 delta, the first review, or the remediation. Effort: medium, with no escalation.

| Item | Value |
|---|---|
| Remediation parent (C1 delta head reviewed above) | `f890ba8df364f88e8724b423b0289a61f35b0348` |
| Remediation commit (not checked out; read via shared objects) | `4541778940a6a7c1db5bda3523e264c9094c9b41` on `integrator/2d-phase2b-c1-gate-retirement-20261126-v1` |
| Review worktree HEAD at start | `46fd0d289f304dc0618e662c6895f279b7a828cb` (clean) |
| `origin/main` after fetch | `bdeb6a324acaf4129c10fb0846faac8b64974360` |

GUARD OK: `git status --porcelain` was empty, HEAD was `46fd0d28…`, and `origin/main` was `bdeb6a32…`.

### Method (what was actually run)

1. `git diff --name-only f890ba8d..45417789` and `git diff f890ba8d..45417789`, read in full. I also checked that `git rev-parse 45417789^` = `f890ba8d…`.
2. I counted `run_full_case` invocations at both SHAs with `grep -c '^run_full_case '`, which gives 16, and with an invocation-only pattern that skips the `run_full_case() {` definition (line 283). Both give 16 at both SHAs. I also listed every invocation line at `45417789`.
3. `git log --first-parent --format='%H %s' origin/integration/2d-sim`, a per-SHA check of membership in that first-parent list, and `git branch -a --contains` for `aa5f1850`, `742bd904`, `158cdd69`, `2af3fb1c`, `5648813d` and `00dcd196`.
4. `git show 45417789:…/s0a-move-table.json | python3 -m json.tool` and `git show 45417789:…/selftest.sh | bash -n /dev/stdin`.
5. `git diff --stat 00dcd196..45417789 -- Tools/MultiRackSitkaP1Lab/build/`, plus diffs of `build/2d-gate.sh`, `build/2d-gate/move-proof.py`, `baseline-counts.json` and `src/Ui/`. I also ran a key-by-key JSON comparison of `s0a-move-table.json` at `00dcd196` vs `45417789`, and checked the `payloadBudgetChecks` lines in `2d-gate.sh` and `baseline-counts.json`.
6. R3: in a scratch `git clone --shared` at `$HOME/tmp/c1r2/scratch-clone` (outside the repo), I forced `gateEnforce=true`, optionally applied the exact C22 tamper (`// selftest move-proof block tamper` after the first `        lock (sync)` in `Commands.Crop.cs`), committed, and ran `move-proof.py --repo <clone> --base <t0Commit> --head HEAD --table <table>`. That is the same invocation as `2d-gate.sh:1191`. I then removed the clone. The review worktree's `git status --porcelain` was still empty afterwards.

| Run | Rev | Tamper | rc | First FAIL/PASS line |
|---|---|---|---|---|
| A | `45417789` | no | 1 | `FAIL [core]: contiguous block core-cmd-clear (T0 lines 626..673) not found in order in …Commands.Clear.cs from line 7` |
| B | `45417789` | yes | 1 | same `core-cmd-clear` failure (the Crop tamper is never reached) |
| C | `00dcd196` | no | 0 | `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1263 non-empty lines); core(7 files, 8 blocks, 1337 non-empty lines)` |
| D | `00dcd196` | yes | 1 | `FAIL [core]: contiguous block core-cmd-crop (T0 lines 457..502) not found in order in …Commands.Crop.cs from line 7` |

### R1–R8

| Check | Result | Evidence |
|---|---|---|
| R1 Scope | **PASS** | `--name-only f890ba8d..45417789` = exactly `build/2d-gate/s0a-move-table.json` and `build/2d-gate/selftest.sh`. |
| R2 Text-only | **PASS** | The diff changes only the `retirement.reason`/`retained` JSON strings, the `git commit -m` message string inside `mut_move_proof()` (it is a git commit message, not a Python string; the Python heredocs are unchanged), the C22 description argument, and 2 added `#` comment lines. Case ID, expected exit `1`, marker regex `S0a move-proof failed`, mutator name, function bodies and `gateEnforce` are unchanged. There are 16 `run_full_case` invocations at both SHAs. |
| R3 F1 accurate | **PASS** | Runs A/B: at the remediated HEAD, `move-proof.py` fails on `core-cmd-clear` whether or not the tamper is applied. So the new text ("gate-wiring check, NOT a tamper-discrimination test… HEAD already drifts from T0 inside core-cmd-clear") is true. Runs C/D independently reproduce the claimed `00dcd196` pair (clean PASS, tamper FAIL on `core-cmd-crop`). |
| R4 F2 accurate | **PASS** | `2af3fb1c` (S0a), `5648813d` (S0b) and `00dcd196` (S0c) are each on the `origin/integration/2d-sim` first-parent chain, with matching `merge(2d-sim): S0a/S0b/S0c …` subjects. `aa5f1850`, `742bd904` and `158cdd69` are not on it; each is contained only by its `review/multi-2d-s0{a,b,c}-…` branch. The note now labels them "merge commits" and "reviews" respectively. |
| R5 No new overclaim | **PASS (one wording nit, non-blocking)** | `reason`, `retained`, the C22 comment and the C22 description promise no tamper detection and no green gate. The `00dcd196` demonstration they cite is real (runs C/D). No secrets, tokens or URLs were added. Nit: "the case would fail" / "what makes this case fail" means "the gate run inside the case exits 1". In selftest terms, C22 *passes* when that happens. A reader can work this out from the context, and it does not change any claim. |
| R6 Valid JSON and shell | **PASS** | `json.tool` on the `45417789` blob → OK. `bash -n` on the `45417789` `selftest.sh` blob → OK. |
| R7 Nothing else weakened | **PASS** | `00dcd196..45417789 -- build/` = the same 3 files as the C1 delta (`baseline-counts.json`, `s0a-move-table.json`, `selftest.sh`). `gateEnforce: false` (`s0a-move-table.json:4`), and the JSON key comparison shows only `gateEnforce` changed and `retirement` added. In `baseline-counts.json`, only `core.minPassed` 282→291 plus a `note` changed (identical at `f890ba8d` and `45417789`), and no `payloadBudgetChecks` line changed. `2d-gate.sh` and `move-proof.py` have a 0-line diff. `src/Ui/` has a 0-line diff. |
| R8 Residual condition | **Only the post-merge gate run remains** | With F1/F2 closed, the only open item is condition (2) from the first review. The integrator must run `2d-gate.sh --browser` and `selftest.sh` green after the merge (including C22 = PASS as a wiring check, and Core ≥ 291/291). I did not, and should not, run it here. |

### LIMITS (not executed)

- `2d-gate.sh` and `selftest.sh` were not run (prohibited by the brief; there is no `.p0ref` here). C22's end-to-end behaviour and every other case are **NOT TESTED**. The R3 conclusion rests on direct `move-proof.py` runs that use the gate's own invocation.
- No `dotnet` or test suite was run, so Core 291/291 is **NOT TESTED**.
- Browser/Stage B and S0c node checks were not run, so they are **NOT TESTED**.
- No Unity, runtime or rendered verification was done; none applies to this delta.
- The remediation branch was never checked out, merged or cherry-picked. It was read only through `git diff`/`git show` and a disposable scratch clone, which has been removed.

R2 INTEGRATION DECISION: F1/F2 REMEDIATION ACCEPTED — wording now matches what the checks prove (C22 = gate wiring only; correct merge SHAs), the change is text-only, and the only open item is the integrator's post-merge green `2d-gate.sh --browser` + `selftest.sh` run.
