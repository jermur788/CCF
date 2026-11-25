# CCF Multi 2D — S0a Host-Compatibility Amendment Record (Owner Option A)

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (§4.6)
- **Adopted spec commit:** `49f8aaaf3f830e7e3a3860a39005ef6e262e42cf`
- **Date:** 2026-11-25
- **Authority:** Owner Ruling Option A (2026-11-25), triggered by T-00b read-only baseline capture at untouched `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1`
- **Host:** HP WSL2 (`Linux DESKTOP-DL1BQI7 6.18.58.1-microsoft-standard-WSL2`), Ubuntu 26.04.1 LTS, .NET SDK `8.0.134`, Node `v26.13.1`, Python `3.14.4`, `strace 6.19`, `git 2.53.0`

## 1. Observed Host & Baseline Facts at Untouched `T0`

During T-00b read-only baseline capture at exact `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1`, two host-toolchain incompatibilities and one pre-existing T0 baseline drift were observed without making any code edits:

### 1.1 Host Fact 1 — P0 Core Binary Cold Rebuild vs Frozen Anchor #8

- **Frozen Anchor #8 pin:** `.p0ref/SitkaLab.Core.dll` SHA-256 = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (built originally under Ubuntu 24.04 + SDK `8.0.133`, pinned in `src/Core/SitkaP1.Core.csproj`, `tests/Core.Tests/Phase2Tests.cs`, and `tests/build-output-safety.sh`).
- **Observed on HP WSL2 host:**
  - `build/build-p0-core-ref.sh` under installed SDK `8.0.134` deterministically produces `bcbdbf05d1f36334fbbf113da70355272952bd9dc9046a9bb78e858e61d697b0`.
  - Under Microsoft's standalone `8.0.133` Linux tarball, it deterministically produces `50e6115ac764e83f140aa4f7a400061f09ec1d041d652c3663253d4e1286d60b` (differing in 72 PDB-derived bytes from the Ubuntu 24.04-built pin).
- **Impact:** Cold rebuilding `.p0ref/SitkaLab.Core.dll` on this host cannot reproduce `9a99f781…`, blocking `SitkaP1.Core.csproj`'s build-time `P0CoreExpectedSha256` check, `tests/build-output-safety.sh` cases `C7-cold-build`/`C8-rebuild`, and the cold-rebuild step inside `tests/p0-isolation.sh`.

### 1.2 Host Fact 2 — Ubuntu 26.04 `uutils coreutils 0.10.0` & MSBuild/Runtime FDs under `strace -f`

- Ubuntu 26.04 ships Rust `uutils coreutils 0.10.0` as default `/usr/bin/{ln,chmod,touch,...}` alongside GNU coreutils 9.7 at `/usr/bin/gnu*`.
- Default `uutils` `ln`/`chmod`/`touch` emit different syscalls than Ubuntu 24.04 GNU coreutils, causing unmodified `tests/p0-isolation.sh`'s L4 decoy check to match `10/13` under default `PATH`, whereas prepending a temporary `PATH` directory symlinking `/usr/bin/gnu{ln,chmod,touch,mv,cp,rm,mkdir,truncate,stat,cat,ls,head}` yields `13/13 distinct expected operation identities matched, 0 unresolved`.
- Additionally, `tests/p0-isolation.sh` runs `dotnet build` under `strace -f` without disabling MSBuild/VBCSCompiler background daemons or `.NET` runtime memfd/diagnostics descriptors. Exporting `MSBUILDDISABLENODEREUSE=1`, `UseSharedCompilation=false`, `DOTNET_CLI_DO_NOT_USE_MSBUILD_SERVER=1`, `DOTNET_EnableWriteXorExecute=0`, and `DOTNET_EnableDiagnostics=0` eliminates background hangs and unresolved FDs (`protected hits=0 unresolved=0 failed protected attempts=0`, exit `0`, `OVERALL: PASS`) with zero script edits.

### 1.3 Pre-existing `T0` Baseline Fact — `tests/ui-smoke.sh` Single-Area `#885` Drift

- At unmodified `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1`, `tests/ui-smoke.sh` produces `250 passed, 4 failed` with exact failing IDs `{L01, L02, L06, J31}` and `PASS G11b` (byte-identical `src/Ui/wwwroot/{index.html,app.css,app.js}`).
- This exact 4-check drift was already documented at unmodified base `4b6d8070` in `Docs/Handoffs/MultiRack-Simulation-20261116/PHASE1-2D-SAVE-E2E-IMPLEMENTATION-AND-VERIFICATION-20261124.md` (§6/§8, line 195), caused by `#885` updates to frozen `src/Ui/wwwroot/{app.css,app.js}` while both `src/Ui/wwwroot/**` and `tests/ui-smoke.sh` are frozen in S0a.

## 2. Owner Option A Preserved Pins & Exact Substitutions

### 2.1 Zero Pin Changes & Zero Existing-Script/Product Edits

Every frozen pin remains **byte-for-byte unchanged**:
- Anchor #8 (`.p0ref/SitkaLab.Core.dll`): `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`
- P0 qualified commit: `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`
- P0 qualified tree (`HEAD:Tools/MultiRackSitkaLab`): `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`
- P0 tracked-files digest (52 tracked files): `9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8`
- Single-area `src/Ui/wwwroot` tree ID: `dabdbd7fa07cd01148af14464c45ceea02548378`

Zero edits were made to `src/Core/SitkaP1.Core.csproj`, `build/build-p0-core-ref.sh`, `tests/build-output-safety.sh`, `tests/p0-isolation.sh`, `tests/ui-smoke.sh`, `tests/Core.Tests/Phase2Tests.cs`, or any other existing test/build script.

### 2.2 Gate Mechanisms Implemented in `build/2d-gate.sh`

1. **Verified read-only `.p0ref` seed (`CCF_P0REF_DIR` / `--p0ref-dir <dir>`):**
   - Validates that `SitkaLab.Core.dll`, `SitkaLab.Core.dll.sha256`, and `provenance.txt` are regular files (`stat.S_ISREG`, non-symlink, `st_nlink == 1`), that `SitkaLab.Core.dll` SHA-256 == `9a99f781…`, that `provenance.txt` holds exact `p0QualifiedCommit`, `p0QualifiedTree`, and `dllSha256` pins, and that `HEAD:Tools/MultiRackSitkaLab == f212c8f6…`.
   - Missing or invalid seed (or cold-rebuild mismatch when no seed is supplied) exits **3 (`PREREQUISITE MISSING`)**, never `0`.
   - Valid seed installs cleanly into `.p0ref/`, re-verifies SHA-256 after install, and emits `GATE G1 PASS (P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3)`.
   - Seed source used on this host (read-only): `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref`.
2. **Offline NuGet isolation inside `unshare -rn`:**
   - G1 writes a scratch `offline-nuget.config` containing `<packageSources><clear /></packageSources>` and exports `RestoreConfigFile` so builds inside `unshare -rn` never probe user-global `nuget.org` while still failing closed if any `PackageReference` is added.
3. **`tests/build-output-safety.sh`:**
   - Backs up `.p0ref`, runs unmodified `tests/build-output-safety.sh`, immediately restores and re-verifies `.p0ref` (`9a99f781…`), requires `PASS` on `C1`–`C6` and `P0:`, fails on any unexpected `FAIL`, and records `C7-cold-build` and `C8-rebuild` as `HOST-NA`.
4. **`tests/p0-isolation.sh`:**
   - Runs unmodified `tests/p0-isolation.sh` inside a disposable `--shared --no-checkout` sparse clone of exact `HEAD` (containing P1, P0, and the four `CcfSourcePins` files) with:
     - a clone-local `build-p0-core-ref.sh` shim installing the verified seed;
     - a `PATH` shim of `/usr/bin/gnu{ln,chmod,touch,mv,cp,rm,mkdir,truncate,stat,cat,ls,head}` (exiting **3** if any `gnu*` binary is absent);
     - the five `.NET` env vars above.
   - Requires exit `0`, `L0/L1/L2/L3/L3+/L4/OVERALL: PASS`, decoy `13/13`, `protected hits=0`, and `unresolved=0`. Only the cold-rebuild step is recorded `HOST-NA`.
5. **Mandatory P0-untouched proof before & after G2:**
   - Verifies `HEAD:Tools/MultiRackSitkaLab == f212c8f6…`, tracked P0 digest == `9e218ca7…`, `git status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab` identical before/after, and `.p0ref/SitkaLab.Core.dll == 9a99f781…`.
6. **`tests/ui-smoke.sh`:**
   - Runs in full; requires `>= 250` passing checks, `PASS G11b`, and zero failures outside `{L01, L02, L06, J31}`. Any other failure or pass-count drop fails G2 (exit `1`).

## 3. Exact `HOST-NA` Disclosure List

1. `build-output-safety.sh` cases `C7-cold-build` and `C8-rebuild`: `HOST-NA` (frozen Ubuntu 24.04 + SDK 8.0.133 Anchor #8 binary pin `9a99f781…` is not cold-reproducible on Ubuntu 26.04 + SDK 8.0.134; `C1`–`C6` and `P0:` PASS; verified `.p0ref` restored and re-verified).
2. `p0-isolation.sh` cold-rebuild step: `HOST-NA` (verified seed shim used in disposable sparse clone; all isolation checks `L0`, `L1`, `L2`, `L3`, `L3+`, `L4` synthetic 23/23, decoy 13/13 with GNU coreutils 9.7 shim, and traced P1 build/tests/CLI with `protected hits=0`, `unresolved=0` PASS with exit `0`).
3. Browser tier Stage `B`: `NOT RUN` (not required for S0a per `spec.md` §4.2 / §8; required for S0c).

## 4. Reviewer Verification Checks

An independent reviewer can verify this amendment on this host or a clean Ubuntu 24.04 + SDK 8.0.133 host:
1. `git diff origin/integration/2d-sim...HEAD -- Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh Tools/MultiRackSitkaP1Lab/tests/ Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj` → **empty**.
2. `CCF_P0REF_DIR=<verified-p0ref> Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base origin/integration/2d-sim <external-ev>` → **exit `0`**, `NETWORK ISOLATION: PROVEN`, `G0`–`G7` all `PASS`.
3. `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base origin/integration/2d-sim --p0ref-dir <tampered-dir> <external-ev>` → **exit `3` (`PREREQUISITE MISSING`)**.
4. `CCF_P0REF_DIR=<verified-p0ref> Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh <external-ev>` → **exit `0`**, `15/15 PASS`, exit codes `[1, 2, 3, 4, 5, 64]` proven.
