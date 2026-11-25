# CCF Multi 2D — S0a Independent Review (RG-S0a)

**Verdict: APPROVED** (CRITICAL 0 · MAJOR 0 · MINOR 3, one pre-existing · NIT 7; none blocking)

> Independent non-author review of the S0a packet (mechanical moves and 2D trunk gate) on the exact pushed HEAD. **NOT INTEGRATED.** This review only reviews. It does not merge, tag or open a PR.

## 1. Reviewer identity and provenance

| Item | Value |
|---|---|
| Reviewer | **Claude Opus 5.5** (`claude-opus-5-5`), effort `xhigh`. Fresh review sub-agent thread dispatched by Coordinator thread `8dd9322f-5d34-43cc-9c17-8764f50752bf`. |
| Independence | Non-author. The S0a author is Gemini 4 Neon (XHigh), per the implementation handoff. This reviewer wrote no S0a code, test, gate, spec text or handoff. |
| Host | `DESKTOP-DL1BQI7`, WSL2, Ubuntu 26.04.1 LTS, kernel `6.18.58.1-microsoft-standard-WSL2` |
| Toolchain | .NET SDK `8.0.134`, Node `v26.13.1`, Python `3.14.4`, git `2.53.0`, strace `6.19`. Default `ln` is `uutils coreutils 0.10.0`; GNU coreutils 9.7 is installed as `/usr/bin/gnu*`. |
| Review window (UTC) | 2026-11-25T17:40:24Z – 2026-11-25T18:46Z |
| Reviewed HEAD | `38301d0af01054ae6c7659473c5f47776db55ad0` (`feat/multi-2d-s0a-mechanical-extraction-20261125-v1`; local = `origin` = `ls-remote`) |
| Trunk / T0 | `origin/integration/2d-sim` = `47b27a8d3873b32726f315ce1182118da38c50b1` (unchanged; merge-base of HEAD) |
| Spec | Adopted quartet at `49f8aaaf3f830e7e3a3860a39005ef6e262e42cf` (first commit on the branch). Compared with the approved docs-branch draft `origin/docs/multi-2d-s0-seam-extraction-spec-20261125-v1` = `4c7b1395e644406a20057b5657d8804e2c9f7645`. |
| `origin/main` | `bdeb6a324acaf4129c10fb0846faac8b64974360`. It moved one 3D docs commit past the brief's `cc2757a7` (only `Docs/Project/instructions/shared-project-instructions.md`). T0 and none of the six S0a commits are reachable from `main`. |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0a-Review-20261125`, branch `review/multi-2d-s0a-mechanical-extraction-20261125-v1`. Clean at start and after every gate, self-test and probe run. |
| Evidence (mode 700) | `/home/jer/Development/CCF-Multi-2D-S0a-Review-Evidence-20261125/`. Reviewer scripts are archived in `scripts/`. |
| Scratch `TMPDIR` | `/home/jer/Development/CCF-S0a-Review-Tmp-20261125` (outside every repo). `/tmp` is a tmpfs with only 701 MB free, so it was not used. The gate's G0 accepts any `TMPDIR` outside the repository. |
| P0 seed | `CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref`: three regular files, `nlink=1`, mode 444. `SitkaLab.Core.dll` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. `provenance.txt` has the exact `p0QualifiedCommit=a7f41983…`, `p0QualifiedTree=f212c8f6…` and `dllSha256=9a99f781…` lines. |

## 2. Commands run and exit codes

Each command ran from the review worktree with the brief's environment (`DOTNET_ROOT`, `PATH`, `CCF_P0REF_DIR`) and the external `TMPDIR` above.

| # | Command (script in `scripts/`) | Exit | Result / evidence |
|---|---|---:|---|
| 1 | `git fetch origin --prune`; `git rev-parse` / `git ls-remote` of feature, trunk, main and docs refs (`01-state.sh`) | 0 | HEAD = `38301d0a…` locally, on `origin` and in `ls-remote`. Trunk = `47b27a8d…`. No remote review branch yet. |
| 2 | `git diff --name-status -M 47b27a8d..HEAD` plus per-path empty-diff and tree-pin proofs (`02-scope.sh`) | 0 | 34 paths, all under the 3 allowed prefixes. Section 4.1 has the details. |
| 3 | Quartet diff against the approved draft (`03-specdiff.sh`, `04-specdiff-save.sh`) | 0 | `spec/quartet-docs-vs-head.diff` |
| 4 | `python3 -I 20-rawlit.py` (reviewer's own raw-literal derivation; does not use `raw-literal.py`) | 0 | `rawlit-independent.json` |
| 5 | `python3 -I 22-moveproof-indep.py` (reviewer's own block cover; does not use the author's table) and `24-boundaries.py` | 0 | `moveproof-independent.json` |
| 6 | **`Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base origin/integration/2d-sim <EV>/gate`** (`10-gate.sh`), 17:44:35Z–17:48:05Z | **0** | `gate.stdout.log`, `gate/`, `gate-run-meta.txt` |
| 7 | **`Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh <EV>/selftest`**, run 1 (`12-selftest.sh`), 17:49:04Z–18:09:24Z | **1** | 14/15. C10 was lost to the pre-existing `NB1c` flake in G2 (MIN-3); every other case and every exit code passed. `selftest.stdout.log`, `selftest/` |
| 8 | `python3 -I 30-mp-mutants.py`: 1 control and 11 mutants against the author's `move-proof.py` | 0 | `move-proof-mutants.log`, `probes/move-proof-mutants.json` |
| 9 | `python3 -I 32-allowance-probes.py`: 28 crafted inputs to the gate's verbatim tolerance snippets | 0 | `allowance-probes.log`, `probes/allowance-probes.json` |
| 10 | `bash 33-seed-probes.sh`: the real gate (HEAD, disposable clone) against 11 invalid seeds and one P0-tree tamper | 0 | `seed-probes.log`, `probes/seed/` |
| 11 | `bash 40-t0-oracle.sh`: T0 and HEAD clone builds (0 warnings / 0 errors each), `ui-smoke.sh` at untouched T0 and at HEAD, first differential HTTP probe, `NB1c` repeat runs (6 at T0, 6 at HEAD) | 0 | `t0-oracle.log`, `t0-oracle/` |
| 12 | `bash 43-mixed-control.sh`: corrected differential probe, T0↔HEAD (twice for `ma-1-mixed`) plus T0↔T0 and HEAD↔HEAD controls, for `ma-1` and `ma-1-mixed` | 0 | `mixed-control.log`, `t0-oracle/mixed-control/` |
| 13 | **`selftest.sh <EV>/selftest2`, run 2, quiet host** (no concurrent reviewer work; `16-selftest2.sh`), 18:15:00Z–18:36:36Z | **0** | **`OVERALL: PASS (15/15 passed, 0 failed; all exit codes 1,2,3,4,5,64 proven)`**. `selftest2.stdout.log`, `selftest2/`, `selftest2-run-meta.txt` |
| 14 | `bash 34-noseed-probe.sh` (twice): real gate with **no seed** in a disposable clone, which takes the cold-build path | 0 (gate exit **3** both times) | `probes/noseed-run1/`, `probes/noseed/` |
| 15 | `bash 35-coldbuild-debug.sh`, `36-coldbuild-gate-trace.sh`: builder in a clone, outside and inside `unshare -rn`, and via the gate's internal G1 worker | 0 | `probes/noseed-run1/cold-*.log`, `probes/noseed-trace/` |
| 16 | `bash 45-nb1c-t0.sh 20` and `bash 46-nb1c-ab.sh 40`: `NB1c` repeat runs at T0, then T0 and HEAD interleaved | 0 | `t0-oracle/nb1c-t0-extra/`, `nb1c-ab.log` |
| 17 | `gh pr list` (feature head, review head, base trunk); `git ls-remote --tags`; live `active-tasks.md` via `gh api` (`50-gh.sh`, `52/53-coord*.sh`) | 0 | Section 4.10 |

## 3. Findings

The S0a code is a faithful mechanical move, and every reproduced check passes. **There are no CRITICAL or MAJOR findings.** All findings below are non-blocking.

| ID | Severity | Finding | File / line | Reproducible evidence |
|---|---|---|---|---|
| **MIN-1** | MINOR | **Some spec adoption deviations are unrecorded or inconsistent.** Besides header/status/approval and §4.6, adoption commit `49f8aaaf` changed two normative texts. (a) **G7** now also allows `specs/multi-2d-`. §16 records this under "Adoption & Approval" as owner-approved. (b) **§4.5** self-test cloning changed from `git clone --local --no-hardlinks` to `git clone --shared --no-checkout` plus sparse checkout. §16 does not list this change at all. Also, **S0-04** (§9, line 618) and **§11** (line 643) still say only `Tools/MultiRackSitkaP1Lab/` and `Docs/` may change. Read literally, that disagrees with the adopted G7 and with this very branch. The handoff's §7 says the only deviation is "Amendment 1". No behaviour or pin is affected: the `--shared` clone never writes the source object store, and the self-test checks the real tree's status and HEAD before and after. | `specs/multi-2d-s0-seam-extraction-v1/spec.md:249,274,618,643,702`; `S0A-IMPLEMENTATION-HANDOFF-20261125.md:150` | `spec/quartet-docs-vs-head.diff`, hunks `@@ -239,13 +240,13 @@` and `@@ -266,11 +267,31 @@` |
| **MIN-2** | MINOR | **The self-test does not plant a violation for every stage or packet sub-check** (§4.5: "one violation per stage"). Missing: **G5** (payload budgets); the S0a **asset-identity** sub-check; the S0a **move-proof** sub-check; the `ui-smoke.sh` **baseline allowance**; the **build-output-safety / p0-isolation** tolerances; the G7 **DENY_PREFIX** list (C11's `src/Core/Phase1` edit trips G4 before G7 runs); the **missing-seed** path; and seed variants other than one tampered DLL. The reviewer's probes exercised all of these, and every one failed closed (section 4.4, 4.6, 4.7). So there is no current defect. But the committed self-test will not catch a regression in these checks on later merges. | `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh:157-313` | `selftest/selftest-summary.json` (15 cases); reviewer probes 8–10 |
| **MIN-3** | MINOR (pre-existing; outside S0a's write surface) | **A known order-dependent Save test makes the gate and self-test non-deterministic.** The frozen `NB1c` asserts `generations[1] - generations[0] == 1`. Which of its two threads takes `LockSlot` first is not fixed, and `pendingBytes` carries its own embedded `SaveGeneration`, so the losing order yields `3`. The gate hard-fails G2 on any Save failure, which is correct fail-closed behaviour. But on the trunk, freeze-on-red turns a spurious `NB1c` failure into a trunk freeze. **Observed by the reviewer:** self-test run 1 lost case C10 to `NB1c` ("expected 1, got 3"; G2 FAIL before the planted G3 violation). The reviewer's lightweight move-proof probe was running at the same time. **Interleaved on a quiet host, T0 failed 3/40 and HEAD failed 3/40**, always "expected 1, got 3". Earlier sequential runs gave T0 0/6 and 0/20, and HEAD 1/6. So the flake is **pre-existing at T0 and unchanged by S0a**, at roughly 7.5% per gate run. The author disclosed this as a pre-existing observation (handoff §7). S0a may not edit tests, and the S0a code path does not touch `Phase1SaveSlots` or the test. | `tests/Phase1.Save.Tests/Program.cs:670-703` (frozen, 0-line diff) | `selftest/case-C10-g3-anchor/g2-save-tests.log:47`; `t0-oracle.log` (NB1c section); `t0-oracle/nb1c-*.log` |
| NIT-1 | NIT | Unexpected termination keeps the raw non-zero exit code instead of normalising it to 1 (§4.3 maps unexpected errors to 1). Codes such as 127/128/137 are possible, and the netns worker's code is passed on verbatim. Exit 0 is impossible without `GATE_CLEAN_EXIT=1`, so this is never fail-open. | `2d-gate.sh:199-221`, `1590-1614` | inspection |
| NIT-2 | NIT | A tampered P0 tree with a valid seed exits **1** (G1 FAIL). §4.6(2) lists the tree check under seed validation, which exits 3. Both are fail-closed, and 1 arguably classifies tampering better. | `2d-gate.sh:467-471` | `seed-probes.log` S12: exit 1, `HEAD:Tools/MultiRackSitkaLab tree=fe08039b… != pinned f212c8f6…` |
| NIT-3 | NIT | HOST-NA acceptance of `C7-cold-build` / `C8-rebuild` matches on labels only, and the exit code of `build-output-safety.sh` is ignored. A C7/C8 failure for a reason other than the documented host SHA `bcbdbf05…`, or a crash after C6, is accepted the same way. G3 still pins anchor #8 independently, and C1–C6 plus `P0:` remain required. | `2d-gate.sh:747-777` | `allowance-probes.log` (BOS "C7/C8 absent entirely" → accepted) |
| NIT-4 | NIT | Brittleness in `move-proof.py`. The `Advance` slice is hard-coded as `t0_lines[984:1155]` (T0 985–1155, two banner lines plus `Advance` at 987–1155) rather than found by member name. The static-field regex accepts only `private const` / `private static readonly`, and the core field regex accepts only lowercase names. All of these are correct at T0, as independently confirmed in section 4.7, but later packets will move lines. The handoff's "Advance (lines 985..1155)" is likewise inexact. | `build/2d-gate/move-proof.py:209,238,249`; handoff `:118` | `moveproof-independent.json` (`advance.t0DocStart=987`, `t0Sig=993`, `t0End=1155`) |
| NIT-5 | NIT | The new G7 prefix `specs/multi-2d-` also admits edits to earlier **approved** 2D specs. That includes `specs/multi-2d-phase1-time-history-core-v1/spec.md`, which §3.8 calls the canonical anchor table. G3 checks the manifest and test literals, not that table. This is a policy choice for the owner or Manager: consider allowing only the active packet's spec directory. | `build/2d-gate/protected-paths.txt:7` | inspection |
| NIT-6 | NIT | In the asset-identity live GET, `proc.stdout.readline()` can block past its 20 s deadline if the server prints nothing, and stderr is a PIPE that is never drained. The failure mode is a hang, never a pass. | `2d-gate.sh:960-982` | inspection |
| NIT-7 | NIT | `--headless-only` returns 5 even when the packet does not need tier B. §4.3 describes 5 as the result when a packet does require it. This is stricter and harmless, and self-test C15 relies on it as the clean-clone positive control. | `2d-gate.sh:1710-1715` | `selftest/case-C15-headless-only.stdout.log` |

**Counts:** CRITICAL 0 · MAJOR 0 · MINOR 3 (one of them pre-existing) · NIT 7.

## 4. Reproduced checks

### 4.1 Pushed identity and scope (brief check 1): PASS

- After `git fetch origin --prune`, local HEAD = `origin/feat/…` = `ls-remote` = `38301d0a…`. Trunk = `47b27a8d…`. The merge-base of HEAD and the trunk is `47b27a8d…`, and HEAD is not in the trunk or in `main`.
- `git diff --name-status -M 47b27a8d..HEAD` gives **34 paths**, none outside `Tools/MultiRackSitkaP1Lab/**`, `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/**` or `specs/multi-2d-s0-seam-extraction-v1/**`. Every new file is mode 100644, except the four new scripts (`2d-gate.sh`, `move-proof.py`, `raw-literal.py`, `selftest.sh`) at 100755. No existing file's mode changed. There is one rename: `PlayerMultiApi.cs → multi/multi.js` (R056, a Git similarity artefact of the `git mv`).
- **Empty diffs, proven per path:** `src/Ui/wwwroot/**`, `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Core/Phase1/**`, `fixtures/**`, `Tools/MultiRackSitkaLab/**`, `build/build-p0-core-ref.sh`, `src/Core/SitkaP1.Core.csproj`, `tests/Core.Tests/Phase2Tests.cs`, **all of `tests/`** (0 paths), `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**`, `.gitignore` and the `.sln`. The only `build/` changes are the 10 new `2d-gate` files.
- **Tree pins:** `HEAD:Tools/MultiRackSitkaLab` = `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (= T0). `HEAD:…/src/Ui/wwwroot` = `dabdbd7fa07cd01148af14464c45ceea02548378` (= T0). `src/Core/Phase1` = `fe970e3f…` (= T0). `fixtures` = `a8dc956b…` (= T0).
- Six commits: spec adoption → gate → asset extraction → UI split → Core split → handoffs. The order is revertable and matches plan §1.2.

### 4.2 Spec adoption (brief check 2): PASS, with MIN-1

- Status is `APPROVED — OWNER APPROVED FOR SEQUENTIAL S0a → S0b → S0c EXECUTION (2026-11-25)` in all four files. The approval provenance line names T0 `47b27a8d…`, the OQ-1..OQ-6 defaults and Amendment 1. The checklist has **80 `[ ]` and 0 `[x]`**.
- The diff against the approved draft `4c7b1395` covers headers, status, base SHA, approval provenance, the plan entry-point flag, the tasks T-A5 file set and the new §4.6/§16. It also includes the **G7 allow-list** and **§4.5** changes described in MIN-1. There is no change to §5–§15 normative content: S0a scope, serving rules, verbatim rule, move rules, S0b/S0c, acceptance criteria and stop conditions are untouched.
- **§4.6 is within Owner Option A as relayed in the brief.** It preserves anchor #8, the P0 tree, the P0 digest and every other pin. It forbids edits to `SitkaP1.Core.csproj`, `build-p0-core-ref.sh`, `build-output-safety.sh`, `p0-isolation.sh`, `ui-smoke.sh` and `Phase2Tests.cs` (confirmed: 0-path diffs). It adds only gate-side handling. The reviewer cannot read the owner's ruling text itself and judges §4.6 against the brief's statement of it.

### 4.3 Verbatim asset identity (brief check 3): PASS

The reviewer's own script reads the T0 blob `0c3f7ad9273666ce8bf751337f62ea4cb8d8e2d4` (438,984 B, 7,521 lines, UTF-8, no BOM, 0 CR). For each asset it finds exactly one exact opener and the first following column-0 `""";` closer, with no `$`, no indented closer and no `"""` inside the content. It joins the content with LF and adds no trailing LF.

| Asset | T0 content lines | Bytes | `R_x` (reviewer) | Committed blob = value | Worktree = value | CR / BOM / final LF | Served (gate, live) |
|---|---|---:|---|---|---|---|---|
| `multi.html` | 1337–2212 (876) | 57,612 | `f9d72dad1d2271e6d6f26c132da93b0257e43cb11a85521c748d98d7d3821868` | yes | yes | 0 / no / no (last `0x3e`) | `f9d72dad…`, 200, headers match |
| `multi.css` | 2216–2899 (684) | 57,702 | `f3b2b68a7547acc78444f6389db0f9f39080589950a54417506c4aaf036f00a5` | yes | yes | 0 / no / no (last `0x7d`) | `f3b2b68a…`, 200, headers match |
| `multi.js` | 2903–7519 (4,617) | 247,066 | `fbc7c8179dbd42605a55d620fbc903b0f4ccbad2805f33270f28fa7eec9de520` | yes | yes | 0 / no / no (last `0x3b`) | `fbc7c817…`, 200, headers match |

- In `s0a-multi-asset-identity.json` (mode `s0a-full`, `t0Commit` `47b27a8d…`), `rSha256 = pSha256 = sSha256 = committedSha256` equals the reviewer's `R_x` for all three assets, and the byte counts match.
- **Independent `P_x` at untouched T0.** The reviewer built T0 `47b27a8d` in a disposable clone with 0 warnings and 0 errors, started it on loopback and requested the three routes. Results: `multi` `f9d72dad…` (57,612 B), `multi.css` `f3b2b68a…` (57,702 B), `multi.js` `fbc7c817…` (247,066 B), so **`P_x = R_x`** for all three. The response headers apart from `Date` are identical between T0 and HEAD for every asset and every guard response. The 8 headers are `cache-control: no-store`, `content-length`, the exact CSP, `content-type`, `cross-origin-resource-policy: same-origin`, `referrer-policy: no-referrer`, `x-content-type-options: nosniff` and `x-frame-options: DENY`. Evidence: `t0-oracle/mixed-control/t0-vs-head/transcript-*.json`.
- `src/Ui/multi/.gitattributes` is exactly `* -text\n`. `git check-attr` reports `text: unset` for all three files.
- The JS hash differs from the spec's informational `62c29114…`. That value was measured at `adea7af`, before Repair #2, so the difference is expected. The spec marks it informational only.

### 4.4 Gate quality and live run (brief check 4): PASS, with MIN-2, MIN-3 and NIT-1, -3, -6, -7

**Code reading.**

- **Order:** G0 runs, then G1 and G2 inside `unshare -rn`, then G3, G4, G5, G6 and G7, then the B policy, then the listener check, then the final marker check. Each stage emits `GATE <stage> PASS`. The final verdict is recomputed from `gate.log`: any missing PASS marker or any FAIL marker gives exit 1.
- **Exit 0:** possible only on the final path with `GATE_CLEAN_EXIT=1`. The `EXIT` trap turns rc 0 without a clean exit into 1.
- **Exit codes:** 2 for G0 preconditions, 3 for a missing tool, SDK, Edge, GNU shim or seed, 4 for G7, 5 for `--headless-only`, 64 for usage.
- **Offline build:** a scratch `NuGet.Config` with `<clear/>` is used. `NETWORK ISOLATION: PROVEN` is printed only on the `unshare -rn` path.
- **Listeners:** recorded outside the netns before and after; one that disappears fails the gate.
- **§4.6 seed validation:** the seed directory must not be a symlink. Each file must be a regular non-symlink file with `nlink==1`. The DLL SHA must equal the manifest's anchor #8, the `.sha256` prefix must match, and the three exact provenance lines must be present. The P0 tree is checked first, and the SHA is re-verified after install. An invalid seed exits 3.
- **Cleanup:** the gate deletes only its own scratch directory, P1 `src/**/bin|obj` and `tests/**/bin|obj`, and `.p0ref`, which it re-installs from the seed. It never touches P0.

**Live run (exit 0).**

- `G0 PASS`, then `GATE G1 PASS (P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781…)`.
- Three builds, each `0 Warning(s) / 0 Error(s)`.
- `G2 PASS`: Core 277/277; Save 50/50; multiarea `OVERALL: PASS (363 passed, 0 failed)`; `ui-smoke.sh` PASS-BASELINE-DRIFT 250/4 `{L01,L02,L06,J31}` with G11b PASS; cli 23/0; build-output-safety C1–C6 and `P0:` PASS with C7/C8 HOST-NA; p0-isolation PASS; asset identity PASS; move-proof PASS.
- `NETWORK ISOLATION: PROVEN`, then G3–G7 PASS, then `GATE B NOT RUN`, then `VERDICT: PASS (exit=0 …)`.
- Afterwards: `.p0ref/SitkaLab.Core.dll` = `9a99f781…`, worktree `git status --porcelain` empty, and the protected listeners `127.0.0.1:{33767,43649,46593}` present before and after.

**Self-test (selftest.sh at exact HEAD, real worktree untouched).**

- **Run 2 (quiet host): exit 0, `15/15`, codes `[1, 2, 3, 4, 5, 64]`.** Results: C01/C02 → 64; C03 (dirty), C04 (evidence inside repo), C05 (bad base) → 2; C06 (no `strace`) and C07 (bad seed) → 3; C08 (compile error) → 1 at G1; C09 (dropped test) → 1 at G2; C10 (tampered anchor) → 1 at `GATE G3 FAIL` after `GATE G2 PASS`; C11 → 1 at G4; C12 → 1 at G6; C13 → 4 at G7; C14 (suppressed marker) → 1 with `fail-closed marker check failed`; C15 → 5.
- **C15 is the clean-clone positive control.** In a fresh `--shared` sparse clone, G0–G7 all printed `PASS`, with `NETWORK ISOLATION: PROVEN`. G2 there reported Core 277/277, Save 50/50, multiarea 363/0, ui-smoke 250/4 with the baseline IDs, cli 23/0, build-output-safety PASS, p0-isolation PASS, identity PASS and move-proof PASS. This confirms the offline clean-clone claim.
- **Run 1: exit 1, 14/15.** C10 stopped at G2 because the frozen Save test `NB1c` failed ("expected 1, got 3"). The planted G3 violation was never reached, so the case's expected `GATE G3 FAIL` pattern was missing. The exit code was still 1, so the gate was fail-closed. Cause: the pre-existing order race described in MIN-3, which the reviewer reproduced at untouched T0. It is not S0a gate logic.
- After both runs: the real worktree status was empty, `.p0ref` = `9a99f781…`, and the listeners were intact.

**Seed probes (real gate, exit 3 every time, G2 never reached).** Symlinked DLL; hard-linked DLL (`nlink=2`); provenance hard-linked outside the directory (`nlink=2`); provenance missing the tree line; wrong provenance commit; bad `.sha256` file; 1-byte DLL flip; no `provenance.txt`; seed directory is a symlink; seed directory missing; `provenance.txt` is a directory. The P0-tree tamper case exits 1 (NIT-2). The seed and the real worktree are unchanged.

**No seed (real gate, disposable clone): exit 3 both times, G2 never reached.** In run 2, and when the G1 worker was invoked directly, the real cold builder ran with rc 0 and produced `bcbdbf05d1f36334…`. G1 then failed with `… != frozen pin 9a99f781…; supply verified CCF_P0REF_DIR/--p0ref-dir per spec §4.6`, which is exactly the §4.6(3) behaviour. In run 1, which started straight after self-test run 1, the builder returned rc 1 with an empty log. The gate still exited 3. The cause was not identified: it did not recur in three further runs (outside the netns, inside `unshare -rn`, and via the gate). It never produced exit 0.

### 4.5 Counts, anchors, golden, budgets, determinism, protected paths (brief check 5): PASS

- **Counts** equal the baseline-counts minima: Core `277/277`, Save `50/50`, multiarea `363/0`, cli `23/0`.
- **`ui-smoke.sh`** is exactly `250 passed, 4 failed`. The failing IDs are `L01` (360 CSS px one-column grid), `L02` (960 px), `L06` (640 px) and `J31` (page witness exit 1), with `PASS G11b` (byte-identical `wwwroot`). The author's untouched-T0 log `…/CCF-Multi-2D-S0a-Evidence-20261125/t00b-baseline/ui-smoke.stdout.log` shows the same four IDs on the same lines (19, 21, 29, 259), the same `OVERALL: FAIL (250 passed, 4 failed)`, and `PASS G11b`. **The reviewer also reproduced this independently.** It ran `tests/ui-smoke.sh` against its own build of untouched T0 in a disposable clone and got `250 passed, 4 failed`, with `{L01,L02,L06,J31}` failing and `PASS G11b`. Its full PASS/FAIL ID list is identical to the HEAD clone's list and to the author's `t00b-baseline` list (`diff` empty). Evidence: `t0-oracle/t0-ui-smoke.stdout.log`, `t0-oracle/head-ui-smoke.stdout.log`.
- **Anchors:** 9/9 verified. #1–#6 match their source pins exactly once. #7 `P2-03 M-08 Q6 hash 1/1/1/1/0 776a68df… | 1/1/1/1/1 776a68df…` appears exactly once. #8 is checked as 3 source pins plus the DLL SHA `9a99f781…`. #9 `P2-10 MA-1-MIXED-v1 30-year fingerprint 93e9abda…` appears exactly once. Every manifest value equals spec §3.8.
- **v2 golden:** `PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding`, `24,703 B`, `c0ea8a46…`, `9629ac16…`, `SchemaVersion = 2`, and an empty `src/Core/Phase1` diff against both the base and T0.
- **Budgets:** 5/5. M11; W137 (view 401,715 B / ecology 41,931 B); W149 (403,008 / 42,223); W258 (447,450 / 37,149 at Year 15); M13.
- **G6:** 65 files scanned. F1 = 0, F2 allowed = 4 (`AnnualContinuation.sessionId`, `MultiAreaContinuation.CaptureSave`, `MultiAreaContinuation.NewSessionId`, `Phase1SaveSlots.NewTempPath`), F2 unlisted = 0, RNG literal violations = 0. The allow-list holds exactly the four §3.6 sites, `NewSessionId` first.
- **G7:** 35 paths (34 plus the rename's old path), 0 violations. The wwwroot tree and the P0 tree are pinned.

**Is the ui-smoke allowance narrow and evidence-backed? Yes.** It is an exact set of four IDs. It needs at least 250 passes, `PASS G11b`, and a FAIL-line count equal to the summary count. In 9 crafted cases (section 4.6 below), every change outside that envelope is rejected: an extra ID, an ID swapped for an unknown one, G11b failing, 249 passes, a count mismatch, a missing summary or an early crash. A fully fixed baseline (254/0) passes. The cause is pre-existing #885 drift in the frozen `wwwroot/app.{css,js}` against the frozen `ui-smoke.sh`; both are untouched by S0a (0-path diffs).

### 4.6 Host-compatibility substitution quality (brief check 6): PASS, with NIT-3

- **Real C1–C6 and `P0:` runs.** The unmodified `tests/build-output-safety.sh` (0-path diff) printed `PASS C1…C6` with exit 4 and the victim unchanged, then `PASS P0: tracked-file digest 9e218ca7… unchanged`. **C7/C8 really ran** and printed `FAIL C7-cold-build: exit 0 sha bcbdbf05…` and `FAIL C8-rebuild: … bcbdbf05…`, which is exactly Host Fact 1. The gate records both as HOST-NA, then restores `.p0ref` and re-verifies it as `9a99f781…`. Nothing is skipped silently: both appear in `gate-summary.json` `hostNa`.
- **p0-isolation.** The run uses the clone's own unmodified `tests/p0-isolation.sh` (exact HEAD in a disposable `--shared` sparse clone). The only substitutions are (i) the clone's `build-p0-core-ref.sh` replaced by a shim that checks the P0 tree, copies the seed and re-checks the SHA, (ii) a `PATH` shim of 12 `/usr/bin/gnu*` symlinks (each checked to exist, else exit 3), and (iii) the five .NET env vars. The output was L0, L1, `L4 self-test PASS: synthetic self-test: 23/23 passed; decoy 13/13 … 0 unresolved`, L2, L3, L3+ (52 tracked P0 files), `L4 PASS … protected hits=0 unresolved=0 failed protected attempts=0`, `P1 tests: 277/277`, `OVERALL: PASS`. The clone was removed.
- **What the shim gives up, and the compensating evidence.** The shim means the cold builder's own syscalls are not traced on this host. The real cold builder did run in C7/C8 under `build-output-safety.sh`, and the `P0:` digest and status were unchanged afterwards.
- **P0 untouched across G2.** P0 status/ignored snapshots are identical before and after (both 0 bytes). Tree is `f212c8f6…`, digest `9e218ca7…`, DLL `9a99f781…`.
- **Tolerance snippets** (verbatim heredocs from HEAD, 28 cases, all as expected). For build-output-safety, a C3 or `P0:` FAIL, a missing C6, or an unknown `FAIL C9` is rejected. For p0-isolation, decoy 12/13, decoy 10/10, 1 unresolved, `protected hits=1`, L4 unresolved 2, failed-protected 1, a missing `L3+`, or `OVERALL: FAIL` is rejected. For G5, a missing W258, an M11 FAIL, or an extra FAIL M13 is rejected.
- **No existing test or build script was edited:** 0 paths under `tests/`, and 0 for `build-p0-core-ref.sh` and `SitkaP1.Core.csproj`.

**§4.6 assessment.** P0 seed, C7/C8 HOST-NA, the p0-isolation clone shim, the GNU coreutils shim and the `ui-smoke.sh` baseline allowance are each **narrow** (exact files, labels and IDs; everything else remains a hard FAIL). Each is **evidence-backed**: reproduced here on this host, including the actual `bcbdbf05…` cold-build output and the T0 smoke log. Each is **within the owner's Option A as stated in the brief**: no pin changed, and no existing script or product file was edited. The one residual gap is NIT-3, the label-based C7/C8 acceptance.

### 4.7 Mechanical-move proof (brief check 7): PASS, with NIT-4

- **The author's `move-proof.py`** (gate G2 and standalone): `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1246 non-empty lines); core(7 files, 8 blocks, 1503 non-empty lines)`.
- **Reviewer's independent block cover.** It covers exact lines, blank lines included, without using the author's table.
  - **UI:** `MultiRoutes.cs` = T0[21..135] + [1019..1116]. `MultiApi.View.cs` = [137..164] [401..785] [927..993] [1145..1235] [1312..1326]. `MultiApi.Commands.cs` = [995..1017] [1307..1310]. `MultiApi.Saves.cs` = [787..925] [1118..1143]. `MultiApi.History.cs` = [166..399] [1237..1305]. Blocks are in increasing order in every file. There are 0 unmatched HEAD lines apart from the 8-line `MultiEmbeddedAsset` helper, which comes last in `MultiRoutes.cs`. There are 0 uncovered non-blank T0 lines. The 3 initialiser lines are the only replacements. Every block starts and ends on a member boundary. Each gap between blocks is exactly one blank line, and no file body starts or ends with a blank line.
  - **Core:** root = T0[76..455] + [985..1571]. Crop [457..502], Fell [504..550], Prune [552..624], Clear [626..673], Planting [675..732], Plan [734..983]. The prefix and suffix of the root file are identical to T0, with 0 unmatched and 0 uncovered lines.
- **Statics and fields.** All **27** `/multi` static/const fields (`MaxIdLength … MaxHistoryPageSize`, `MultiFenceMembers … MultiRecoverMembers`, `MultiPageHtml/Css/Js`, `MultiLabels`) are in `MultiRoutes.cs` in T0 order. There are **0** in `MultiApi.*.cs`, checked with a broad regex covering any access modifier, `static` and `const`. There are **0** field-like declarations in `MultiAreaContinuation.Commands.*.cs`. The root file keeps all 13 instance fields, including `sync` and `root`.
- **`Advance`** (located by name: doc 987, signature 993, end 1155 at T0) is **byte-identical** at HEAD (459/465/627). There is one overload.
- **`PlayerMultiApi.cs`** is absent at HEAD. There is no `MultiAreaPage` anywhere in `src/` (it appears only as quoted "old" text in the move table). **`MultiSaveAsync`** (41 lines) is identical to T0. It keeps `using (saveSlots.LockSlot(slot))` closing before `await WriteJsonAsync`, so the F-01 fix is preserved.
- **Cross-part static initialisation order.** The `PlayerApi.cs` statics (`PageHtml/Css/Js`, `Labels`, `RequestJson`, `ResponseJson`) read no `/multi` static, and no `/multi` static reads them. So moving the `/multi` statics into a file that may compile before `PlayerApi.cs` cannot change behaviour.
- **Mutation test of the author's tool** (1 control and 11 mutants, each one commit in a disposable clone). The control passes. **All 11 mutants are killed (exit 1):** swapping two statics; moving a static into `MultiApi.Saves.cs`; a trailing space in a member; a blank line inside a member; a blank line between two members of a block; swapping two `Advance` statements; renaming `UnmarkCrop`; reordering two History members; a duplicated statement; changing `>` to `>=`; moving a field into a command partial; moving `NewSessionId` into a command partial.

### 4.8 HTTP and security surface (brief check 8): PASS

- **Code.** `Program.cs` and `PlayerApi.cs` (listener, bind check, capability, Host/Origin/admission guards, CSP and headers, `WriteAssetAsync`) have 0-path diffs. The route table (33 routes) and every handler, DTO and JSON record are inside verbatim-moved blocks (section 4.7).
- **The only non-move code edits** are the three `MultiPage*` initialisers, now `MultiEmbeddedAsset("SitkaP1.Ui.multi.multi.{html,css,js}")`, and the 8-line `MultiEmbeddedAsset`. It mirrors `EmbeddedAsset` and throws `InvalidOperationException` when a resource is missing, which fails closed at type initialisation. The `SitkaP1.Ui.csproj` change is one `ItemGroup` with `<None Remove="multi/**" />` and three explicit `EmbeddedResource`/`LogicalName` lines. The gate's G1 check found no duplicate `LogicalName`.
- **No filesystem read.** `644 B-01` passes (both cases). It enumerates every `src/**/*.cs`, so it covers the new files. There is no second listener and no MIME inference.
- **Live headers** for all three assets equal `headersNoDate` in the identity JSON (gate G2).
- **Differential T0 ↔ HEAD HTTP probe** (`41-httpdiff.py`, `43-mixed-control.sh`). The T0 build and the HEAD build (each a disposable clone) were each started on loopback and driven with the **same 64-step script**. Each request is derived from that server's own previous response.
  - Steps covered: 3 assets plus an `Origin` variant; 10 guard probes (wrong Host 403, foreign Origin 403, POST on asset 405, HEAD 405, unknown/case-shifted/trailing-slash/`..`/wrong-capability 404, no-slash prefix 308, query string); reads of view, manifest, saves, history/index and structure; competition, crop/mark, pending/add in A and B, a stale-fence 409, missing consent 400, malformed JSON 400, 413, 415, approve, execute, execute replay 409, Year+1, preview-review, skip +3, history/year, prune add/remove, cell, hit, clear add/remove, plant/shelter/spot-clear, planting/remove, approve/cancel, consented Year+1 and three more Year+1 steps; then the final view, history and structure.
  - Comparison: status, headers apart from `Date`, and **body bytes**. Only the per-process session id and capability token are normalised in bodies; the port is normalised only in headers.
  - **Result: `ma-1` 64/64 byte-identical. `ma-1-mixed` 64/64 byte-identical in two independent T0↔HEAD pairings.** The T0↔T0 and HEAD↔HEAD controls are also 64/64, so the probe can tell identical from different.
  - *Disclosure:* the reviewer's first run of this probe reported `ma-1-mixed` 58/64. That was a probe bug: the port string was also replaced inside bodies, which corrupted numeric JSON values that happened to contain those digits. The T0↔T0 control showed the same symptom, the "differing" bodies did not parse as JSON, and the corrected run (above) supersedes it. Evidence: `t0-oracle/httpdiff/` (first run) and `t0-oracle/mixed-control/` (corrected run).

### 4.9 No regressions (brief check 9): PASS

- No test was removed or weakened: 0 paths changed under `tests/`. No test references the moved files by path; a grep for `PlayerMultiApi`, `MultiAreaPage`, `MultiRoutes`, `MultiApi.` and `src/Ui/*.cs` in `tests/` finds nothing, so no source-path check became vacuous.
- There is no fixture, P0, Phase1 or `wwwroot` edit.
- Route and command behaviour: see section 4.8, the anchors (9/9) and the suites.

### 4.10 Branch, PR and coordination state (brief check 10)

- The review worktree was clean before this document. The feature branch is pushed and unmoved at `38301d0a…`. The trunk is unmoved at `47b27a8d…`. There is **no PR** with head feat/review or with base `integration/2d-sim`, and **no tag** matching `s0a` or `38301d0a`.
- **Process observations.** These are not S0a implementation findings, because S0a may not write `Docs/Project/**`.
  - The live GitHub `Docs/Project/coordination/active-tasks.md` on `main` (blob `39d4ffd3…`, fetched with `gh api`) has **no row** registering the S0 trunk or the S0a single writer, as E-0 requires.
  - `decision-log.md` has no entry for locked decisions A–D. The spec lists this as recommended.
  - The trunk creation record asserts that T0 is owner-accepted but does not link a GitHub-tracked owner acceptance record. The R3 approval of `47b27a8d` exists (`origin/review/multi-2d-phase1-time-history-ui-20261125-v1-r3` @ `fa2bde3b`, verdict APPROVED), but it says "NOT INTEGRATED, NOT ACCEPTED" as of 13:04Z.

## 5. Checklist rows covered by this review (not ticked in the spec; the review does not edit the spec)

| Row | Status here |
|---|---|
| S0-01 | Trunk at T0 confirmed with `ls-remote`. No commit after T0 on the trunk yet. Not on `main`. |
| S0-02 | PASS. Locked order, markers and exit 0 on the gate; self-test run 2 15/15 with codes 1/2/3/4/5/64 (run 1 14/15 because of the pre-existing `NB1c` flake, MIN-3). Clean-clone offline run shown by self-test C15 (G0–G7 PASS in a fresh sparse clone under `unshare -rn`). See MIN-2. |
| S0-03 | PASS. `R_x` = committed = served at HEAD = `P_x` served at untouched T0, all three checked by the reviewer. Headers equal apart from `Date`. |
| S0-04 | PASS for the S0a diff (0 G7 violations, wwwroot tree = T0). Wording inconsistency: MIN-1. |
| S0-05 | PASS (author's tool, reviewer's independent cover, 11/11 mutants killed). |
| S0-REG (S0a slice) | PASS headless: 9/9 anchors, golden, counts at or above baseline, budgets, listeners, no test change. The Edge tier is not required for S0a and was not run (`B NOT RUN`). |
| Gate G1–G7, fail-closed codes, offline clean checkout | PASS (sections 4.4–4.5) |
| Locked decision B rows (asset tree, embedding, unchanged surface, scripted extraction, both splits, no logic/rename/reorder/churn) | PASS (sections 4.3, 4.7, 4.8) |
| Hard exclusions relevant to S0a (Assets, wwwroot, P0, Packages, ProjectSettings, main, listeners, Docs/Project) | PASS (sections 4.1, 4.4) |
| "Every deviation is recorded in spec.md and the handoff" | Partial: MIN-1 |

## 6. Verdict

**APPROVED.** The exact HEAD `38301d0a…` does what S0a must do with zero behaviour change, and every reproduced check passes:

- The `/multi` HTML, CSS and JS bodies are byte-identical at all four points: the T0 C# literal, the T0 server, the committed files and the HEAD server.
- The route and continuation splits are pure contiguous moves. This was proved independently and with 11/11 mutants killed.
- A 64-step HTTP drive of T0 and HEAD, run for two experiments, returns byte-identical responses.
- All 9 anchors, the v2 golden, all suite counts and all budgets hold.
- The gate is fail-closed: exit 0 on HEAD; 15/15 self-test on a quiet host; 11/11 invalid seeds and the no-seed case exit 3; 28/28 tolerance probes behave as specified.
- The §4.6 substitutions are narrow, evidence-backed and keep every frozen pin.

The three MINOR findings are non-blocking. Two are about traceability and self-test coverage, not behaviour. The third is a pre-existing frozen-test flake that S0a may not touch and does not change.

### Non-blocking follow-ups (recommended for the integrator-owned `build/2d-gate/**` and spec docs in S0b or a docs repair)

1. MIN-1: add the §4.5 self-test cloning change to spec §16. Align the S0-04 and §11 wording with the adopted G7 allow-list. Correct the handoff's "only Amendment 1" statement, either in the S0 closure record or in a docs-only repair.
2. MIN-2: add self-test cases for G5 (a missing budget ID), the asset-identity sub-check (a one-byte change to `multi.css` committed in the clone), the move-proof sub-check (a swapped static), the `ui-smoke` allowance (an unknown FAIL ID), a G7 `DENY_PREFIX` path that reaches G7, missing seed with a cold-build mismatch (exit 3), and symlink/hard-link seeds.
3. MIN-3: ask the owner or Manager to authorise a separate, test-only repair that makes `NB1c` order-independent. For example, assert uniqueness and monotonicity rather than `== 1`, or make both writers' embedded generations equal. Until that lands, the integrator should agree a re-run rule for an isolated `NB1c` "expected 1, got 3" failure before the trunk depends on freeze-on-red. S0a cannot make this change, because tests are frozen.
4. NIT-1 to NIT-7 as listed. The most useful are NIT-4 (locate `Advance` and fields by name in `move-proof.py` before S0b moves lines) and NIT-3 (assert the documented host SHA for C7/C8).

## 7. INTEGRATION DECISION

```
INTEGRATION DECISION: APPROVED
Reviewed HEAD: 38301d0af01054ae6c7659473c5f47776db55ad0
Gate: exit 0 — G0–G7 PASS, NETWORK ISOLATION: PROVEN, P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781…, B NOT RUN;
      Core 277/277, Save 50/50, multiarea 363/0, cli 23/0, ui-smoke 250/4 {L01,L02,L06,J31} + G11b PASS,
      build-output-safety C1–C6+P0 PASS (C7/C8 HOST-NA), p0-isolation PASS (decoy 13/13, hits 0, unresolved 0);
      9/9 anchors, v2 golden 24,703 B/c0ea8a46…/9629ac16…/SchemaVersion=2, 5/5 budgets, G6 F1=0/F2_unlisted=0, G7 0 violations.
      Selftest: run 2 (quiet host) exit 0, 15/15, codes [1,2,3,4,5,64]; run 1 exit 1, 14/15 (C10 lost to pre-existing NB1c flake, MIN-3).
      Independent: R_x = P_x(T0 served) = committed = S_x for html/css/js; 64-step T0↔HEAD HTTP drive byte-identical (ma-1, ma-1-mixed);
      move proof reproduced + 11/11 mutants killed; 11/11 invalid seeds and no-seed -> exit 3.
Conditions / blocking findings: Blocking findings: none (CRITICAL 0, MAJOR 0). Non-blocking: MIN-1, MIN-2, MIN-3 (pre-existing), NIT-1..7.
      Integration conditions (Coordinator/Manager route, outside S0a's write surface): before the --no-ff merge,
      (a) re-run 2d-gate.sh on the exact rebased HEAD (trunk unchanged at 47b27a8d, so no rebase is expected) and on the merge commit;
      (b) record or link the owner acceptance of T0 and the S0a single-writer registration per E-0 (absent from live active-tasks.md at review time);
      (c) agree how an isolated NB1c "expected 1, got 3" G2 failure (~7.5%/run, pre-existing at T0) is handled before relying on freeze-on-red;
          a re-run is not a code change, and no test may be edited in S0a.
```

## 8. Review hygiene

- **No S0a code, test, gate, spec, handoff or feature-branch file was modified by this review.** The only change is this one new file on `review/multi-2d-s0a-mechanical-extraction-20261125-v1`.
- All probes ran in disposable `--shared` clones under the external `TMPDIR`, and every clone was removed afterwards. The review worktree's `git status --porcelain` was empty after the gate, the self-test and every probe. `.p0ref` = `9a99f781…`. The seed directory is unchanged (`nlink 1`, mode 444).
- No process was killed by name. Servers started by probes were stopped by their own `Popen` handle. The protected listeners were never touched.
- Browser tier B was not run (not required for S0a). Nothing was verified on an Ubuntu 24.04 + SDK 8.0.133 host (the cold P0 rebuild is HOST-NA here). No Unity was used.
