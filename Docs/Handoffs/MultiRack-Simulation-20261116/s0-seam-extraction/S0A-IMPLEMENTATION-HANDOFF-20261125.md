# CCF Multi 2D — S0a Implementation Handoff (Mechanical Moves + 2D Trunk Gate)

## IMPLEMENTATION HANDOFF

- **Status:** **NOT REVIEWED, NOT INTEGRATED** (all 80 checklist items in `specs/multi-2d-s0-seam-extraction-v1/checklists/requirements.md` remain `[ ]` unchecked for independent review)
- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (Adopted Status: `APPROVED`, including §4.6 Host-Compatibility Amendment Option A)
- **Packet:** S0a ("Accept and refactor — ZERO BEHAVIOUR CHANGE")
- **Author / Primary Implementer:** Gemini 4 Neon (XHigh) (delegated from Coordinator thread `8dd9322f-5d34-43cc-9c17-8764f50752bf`)
- **Date:** 2026-11-25
- **Worktree (single writer):** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0a-20261125`
- **Branch:** `feat/multi-2d-s0a-mechanical-extraction-20261125-v1`
- **Trunk / Baseline (`T0`):** `origin/integration/2d-sim` = `47b27a8d3873b32726f315ce1182118da38c50b1`
- **External evidence directory (mode `700`):** `/home/jer/Development/CCF-Multi-2D-S0a-Evidence-20261125/`
- **Companion records:**
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-TRUNK-CREATION-RECORD-20261125.md`
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-HOST-COMPATIBILITY-AMENDMENT-20261125.md`

---

## 1. Commit Breakdown (Revertable Granularity per `plan.md` §1.2)

| # | Commit SHA | Subject | Scope |
|---|---|---|---|
| 1 | `49f8aaaf3f830e7e3a3860a39005ef6e262e42cf` | `docs(spec): adopt approved S0 seam-extraction quartet with §4.6 host-compatibility amendment` | `specs/multi-2d-s0-seam-extraction-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` (80 `[ ]`, 0 `[x]`) |
| 2 (A-C1) | `ba2dd59cca108bc6fcb57bb6749ed57b86e68900` | `build(2d-gate): add fail-closed 2D trunk gate and self-test` | `build/2d-gate.sh`, `build/2d-gate/{frozen-anchors.json,protected-paths.txt,determinism-allowlist.txt,baseline-counts.json,selftest.sh}` |
| 3 (A-C2) | `8e4c4daa8696ab4740704219e85980ae4678f5c3` | `refactor(ui): extract /multi page verbatim into embedded assets` | `src/Ui/multi/{multi.html,multi.css,multi.js,.gitattributes}`, `build/2d-gate/{raw-literal.py,s0a-multi-asset-identity.json}`, `SitkaP1.Ui.csproj`, `PlayerMultiApi.cs` (§5.2(i)–(ii) only) |
| 4 (A-C3) | `58f55c7bbe6933a02cc0615c5a267f029dc9067f` | `refactor(ui): split /multi PlayerApi partial into route files` | `git mv PlayerMultiApi.cs MultiRoutes.cs`, `MultiApi.{View,Commands,Saves,History}.cs`, `build/2d-gate/{move-proof.py,s0a-move-table.json}` |
| 5 (A-C4) | `f089965ef03ca29bfd92df3dd53cb10c581021a4` | `refactor(core): split continuation Work Plan commands into partial files` | `MultiAreaContinuation.cs`, `MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs`, Core rows in `s0a-move-table.json` |
| 6 (T-A5) | *(this commit — exact pushed HEAD reported in handoff reply)* | `docs(handoff): publish S0a trunk record, host-compat amendment and implementation handoff` | Three Markdown records under `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/` |

*(Note: final `build/2d-gate.sh` and `build/2d-gate/selftest.sh` were re-executed on Commit 6's exact pushed HEAD before pushing; raw logs reside in `/home/jer/Development/CCF-Multi-2D-S0a-Evidence-20261125/final-gate/` and `/home/jer/Development/CCF-Multi-2D-S0a-Evidence-20261125/final-selftest/`.)*

---

## 2. Explicit Path Reconciliation (Adopted Spec Governs)

Per owner direction, the adopted specification's exact owned paths (`spec.md` §5, §8, OQ-1..OQ-4; `plan.md` §1.2, §2) govern over informal summary names in the dispatch brief:

| Item | Informal dispatch summary | Adopted spec canonical path (implemented) |
|---|---|---|
| Trunk gate & data | `tests/2d-gate.sh` | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh` + `Tools/MultiRackSitkaP1Lab/build/2d-gate/**` |
| Extracted `/multi` assets | `src/Ui/Multi/index.html` | `Tools/MultiRackSitkaP1Lab/src/Ui/multi/{multi.html,multi.css,multi.js,.gitattributes}` |
| UI route partials | `src/Ui/Routes/...` | `Tools/MultiRackSitkaP1Lab/src/Ui/{MultiRoutes.cs,MultiApi.View.cs,MultiApi.Commands.cs,MultiApi.Saves.cs,MultiApi.History.cs}` |
| Continuation command splits | `WorkPlan`/`Planting`/`Clearing`/`TimePacing` | `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs` |

---

## 3. Gate Verification Summary (T-00b Baseline vs S0a)

### 3.1 Stage-by-Stage Results (`build/2d-gate.sh --base origin/integration/2d-sim`)

| Stage | Result | Key Evidence |
|---|---|---|
| **G0** Preflight | `PASS` | Clean working tree, external `TMPDIR` & evidence dir, base `47b27a8d3873b32726f315ce1182118da38c50b1`, tools verified (`dotnet 8.0.134`, `node v26.13.1`, `python3 3.14.4`, `git 2.53.0`, `strace 6.19`, `curl`, `ss`) |
| **G1** Clean build (0W/0E) | `PASS` | `P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; `MultiRackSitkaP1Lab.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj` all `0 Warning(s), 0 Error(s)` offline inside `unshare -rn`; 0 duplicate `EmbeddedResource` `LogicalName`s |
| **G2** All test suites + sub-checks | `PASS` | All 7 suites meet/exceed T-00b baseline; S0a `/multi` asset identity `PASS`; S0a move-proof `PASS`; P0 tree/digest/status untouched before & after |
| **G3** 9/9 Frozen SHA-256 anchors | `PASS` | 9/9 verified against `build/2d-gate/frozen-anchors.json` (source cross-check #1–#6 & #8, Core stdout #7 & #9, DLL hash #8) |
| **G4** Committed v2 golden saves | `PASS` | `PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding` (24,703 B, file `c0ea8a46…`, checksum `9629ac16…`), `SchemaVersion = 2`, `src/Core/Phase1/` diff empty |
| **G5** Payload budget checks | `PASS` | 5/5 budget IDs `PASS` (`M11`, `W137`, `W149`, `W258`, `M13`; view ≤ 524,288 B, ecology ≤ 153,600 B) |
| **G6** Core determinism grep | `PASS` | `65` Core `.cs` files scanned; `F1 = 0`, `F2_allowed = 4` (`NewSessionId`, `sessionId`, `CaptureSave`, `NewTempPath`), `F2_unlisted = 0` |
| **G7** Protected-path guard | `PASS` | 0 violations; `src/Ui/wwwroot` tree == `dabdbd7fa07cd01148af14464c45ceea02548378`; P0 tree == `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| **B** Browser tier | `NOT RUN` | Optional / not required for S0a (required for S0c) |
| **Network Isolation** | `PROVEN` | G1 + G2 executed inside `unshare -rn` (`ip link set lo up`, zero outbound route, scratch offline `NuGet.Config`) |
| **Protected Listeners** | `PRESERVED` | `127.0.0.1:33767`, `127.0.0.1:43649`, `127.0.0.1:46593` verified listening via `ss -tln` outside netns before and after |

### 3.2 Per-Suite Counts (Baseline T-00b vs S0a)

| Suite | T-00b Baseline (`47b27a8d`) | S0a Result | Notes |
|---|---|---|---|
| `Core.Tests` | `277/277 passed, 0 failed` | `277/277 passed, 0 failed` | Includes `644 B-01` (zero page file reads) and anchors #1–#9 |
| `Phase1.Save.Tests` | `50/50 passed, 0 failed` | `50/50 passed, 0 failed` | Includes v2 golden (24,703 B) |
| `tests/ui-smoke-multiarea.sh` | `363 passed, 0 failed` | `363 passed, 0 failed` | Includes all 5 budget checks (`M11`, `W137`, `W149`, `W258`, `M13`) |
| `tests/ui-smoke.sh` | `250 passed, 4 failed` (`L01,L02,L06,J31`; `G11b PASS`) | `250 passed, 4 failed` (`L01,L02,L06,J31`; `G11b PASS`) | Exact pre-existing T0 `#885` drift per §4.6; 0 unexpected failures |
| `tests/cli-safety.sh` | `23 passed, 0 failed` | `23 passed, 0 failed` | `OVERALL: PASS` |
| `tests/build-output-safety.sh` | `C1–C6 + P0:` PASS; `C7/C8` HOST-NA | `C1–C6 + P0:` PASS; `C7/C8` HOST-NA | Verified `.p0ref` saved, restored, and re-verified (`9a99f781…`) |
| `tests/p0-isolation.sh` | `OVERALL: PASS` (decoy `13/13`, `hits=0`, `unres=0`) | `OVERALL: PASS` (decoy `13/13`, `hits=0`, `unres=0`) | Disposable `--shared` sparse clone + verified seed shim + GNU coreutils 9.7 shim |

### 3.3 9/9 Frozen SHA-256 Anchors (All Verified)

| # | Anchor | Expected & Observed SHA-256 | Verification |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `Model0FingerprintTests.cs` pin + Core PASS |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `Model0FingerprintTests.cs` pin + Core PASS |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `Stage4Tests.cs` pin + Core PASS |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `Stage5Tests.cs` pin + Core PASS |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `Stage5Tests.cs` pin + Core PASS |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `Stage5GridStudy.cs` pin + Core PASS |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | Core stdout pair `h0 == h1` exact |
| 8 | P0 Core binary `.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | 3 source pins + DLL SHA-256 exact |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | Core stdout single exact |

---

## 4. `/multi` Verbatim Extraction & Byte-Identity Proof (S0-03)

Gate input committed at `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-multi-asset-identity.json` (`mode: "s0a-full"`, T0 source blob `0c3f7ad9273666ce8bf751337f62ea4cb8d8e2d4`).

| Route | File (`LogicalName`) | T0 Content Lines | Bytes | Last Byte | `R_x == P_x == S_x == committed` SHA-256 | Headers (excl. `date`) |
|---|---|---|---|---|---|---|
| `multi` | `src/Ui/multi/multi.html` (`SitkaP1.Ui.multi.multi.html`) | `1337..2212` (876 lines) | `57,612` | `0x3e` | `f9d72dad1d2271e6d6f26c132da93b0257e43cb11a85521c748d98d7d3821868` | Exact match (9 headers) |
| `multi.css` | `src/Ui/multi/multi.css` (`SitkaP1.Ui.multi.multi.css`) | `2216..2899` (684 lines) | `57,702` | `0x7d` | `f3b2b68a7547acc78444f6389db0f9f39080589950a54417506c4aaf036f00a5` | Exact match (9 headers) |
| `multi.js` | `src/Ui/multi/multi.js` (`SitkaP1.Ui.multi.multi.js`) | `2903..7519` (4,617 lines) | `247,066` | `0x3b` | `fbc7c8179dbd42605a55d620fbc903b0f4ccbad2805f33270f28fa7eec9de520` | Exact match (9 headers) |

All three files are pinned via `src/Ui/multi/.gitattributes` (`* -text`), contain `0` CR bytes, no UTF-8 BOM, and no trailing LF.

---

## 5. Move-Proof Summary (S0-05)

Verified by `Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py` against `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1` using `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-move-table.json` (full JSON report saved externally at `/home/jer/Development/CCF-Multi-2D-S0a-Evidence-20261125/move-proof-report.json`):

- **UI split (`PlayerMultiApi.cs` deleted via `git mv` → 5 partial files):**
  - `MultiRoutes.cs` (244 lines), `MultiApi.View.cs` (601 lines), `MultiApi.History.cs` (313 lines), `MultiApi.Saves.cs` (175 lines), `MultiApi.Commands.cs` (36 lines).
  - `14` contiguous blocks, `1,246` non-empty lines verified: exact line-multiset equality accounting ONLY for §5.2 (i)–(iv); original relative block order preserved; all 27 static/const fields (`MaxIdLength..MaxHistoryPageSize`, `MultiFenceMembers..MultiRecoverMembers`, `MultiPageHtml/Css/Js`, `MultiLabels`) remain in `MultiRoutes.cs` in exact `T0` order (`0` in `MultiApi.*.cs`).
  - `MultiSaveAsync` F-01 invariant (`using (saveSlots.LockSlot(slot))` released before `await WriteJsonAsync`) moved verbatim untouched into `MultiApi.Saves.cs`.
- **Core split (`MultiAreaContinuation.cs` → root + 6 command partial files):**
  - `MultiAreaContinuation.cs` (1,044 lines) + `MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs` (53, 54, 80, 55, 65, 257 lines).
  - `8` contiguous blocks, `1,503` non-empty lines verified: exact line-multiset equality accounting ONLY for §5.2 (iv) partial-file headers/footers; `0` field declarations in command partials; `Advance` (lines `985..1155` at `T0`) verified byte-identical to `T0`.

---

## 6. Fail-Closed Self-Test Summary (`build/2d-gate/selftest.sh`, S0-02)

Executed in isolated `--shared --no-checkout` sparse clones outside the repository (real working tree untouched before and after):

| Case | Planted Violation | Expected Exit | Observed Exit | Result |
|---|---|---:|---:|---|
| `C01-usage` | Bad CLI flag `--not-a-real-flag` | 64 | 64 | `PASS` |
| `C02-usage-mutex` | Mutually exclusive `--browser --headless-only` | 64 | 64 | `PASS` |
| `C03-g0-dirty` | Untracked file in working tree | 2 | 2 | `PASS` |
| `C04-g0-ev-in-repo` | Evidence directory inside repository | 2 | 2 | `PASS` |
| `C05-g0-bad-base` | Unresolvable `--base` ref | 2 | 2 | `PASS` |
| `C06-g0-missing-tool` | `strace` absent from `PATH` | 3 | 3 | `PASS` |
| `C07-g1-bad-seed` | Invalid/tampered `.p0ref` seed | 3 | 3 | `PASS` |
| `C08-g1-compile` | `#error` compile error in Core | 1 | 1 | `PASS` |
| `C09-g2-dropped-test` | Core test count dropped (`276 < 277`) | 1 | 1 | `PASS` |
| `C10-g3-anchor` | Tampered Anchor #1 in `frozen-anchors.json` | 1 | 1 | `PASS` |
| `C11-g4-phase1` | Edit to frozen `src/Core/Phase1/Phase1SaveFormat.cs` | 1 | 1 | `PASS` |
| `C12-g6-f1-random` | Planted `new System.Random(0)` in `src/Core/` | 1 | 1 | `PASS` |
| `C13-g7-assets` | Planted file under forbidden `Assets/` | 4 | 4 | `PASS` |
| `C14-missing-marker` | Suppressed `GATE G7 PASS` marker in `2d-gate.sh` | 1 | 1 | `PASS` |
| `C15-headless-only` | Explicit `--headless-only` partial mode | 5 | 5 | `PASS` |

**Selftest verdict:** `15/15 PASS`, `0 failed`; all exit codes `[1, 2, 3, 4, 5, 64]` proven.

---

## 7. Deviations, Host-NA & Verification Commands for Independent Review

- **Deviations from original DRAFT spec:** Only Owner-approved **Amendment 1 (§4.6 Host-Compatibility Option A)** as documented in `S0A-HOST-COMPATIBILITY-AMENDMENT-20261125.md` and adopted in `specs/multi-2d-s0-seam-extraction-v1/spec.md` (`49f8aaaf`). Zero behavioural, gameplay, biology, RNG, save-format, or UI changes.
- **Pre-existing test observation (frozen, untouched):** Under heavy parallel CPU contention, frozen `tests/Phase1.Save.Tests/Program.cs` test `NB1c` (lines 670–703) can observe a thread-scheduling race when `pendingBytes` (`SaveGeneration = 4`) acquires `LockSlot` ahead of `model0Bytes`, yielding `generations[1] - generations[0] == 3` instead of `1`. `Phase1.Save.Tests` was not modified in S0a; `build/2d-gate/selftest.sh` runs full end-to-end cases `C10`–`C15` sequentially in foreground to avoid parallel CPU contention and eliminate background `SIGINT` masking.
- **Not directly verified:** Browser tier Stage `B` (not required for S0a); cold P0 binary rebuild on an Ubuntu 24.04 + SDK 8.0.133 machine (`HOST-NA` on HP WSL2 Ubuntu 26.04 + SDK 8.0.134).
- **Exact reviewer reproduction commands (WSL2):**

```bash
export DOTNET_ROOT=/home/jer/.dotnet
export PATH=/home/jer/.dotnet:/home/jer/.local/share/vite-plus/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
export CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref

Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base origin/integration/2d-sim /tmp/s0a-review-gate
Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh /tmp/s0a-review-selftest
python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py \
  --repo "$(git rev-parse --show-toplevel)" \
  --base 47b27a8d3873b32726f315ce1182118da38c50b1 \
  --head HEAD \
  --table Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-move-table.json --part all
```

---

**INTEGRATION STATUS:** **NOT REVIEWED, NOT INTEGRATED.** Ready for independent non-author review (RG-S0a) on the exact pushed HEAD of `feat/multi-2d-s0a-mechanical-extraction-20261125-v1`.
