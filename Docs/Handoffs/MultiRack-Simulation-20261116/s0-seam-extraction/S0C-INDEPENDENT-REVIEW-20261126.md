# CCF Multi 2D — RG-S0c Independent Review (S0c native ES modules + `map/contract.js`)

## Review identity

| Field | Value |
|---|---|
| Review gate | **RG-S0c** (`specs/multi-2d-s0-seam-extraction-v1/tasks.md`) |
| Spec | `SPEC-multi-2d-s0-seam-extraction-V1` (APPROVED, incl. Post-Adoption Amendment 1 §4.6 Option A) |
| Reviewed branch | `feat/multi-2d-s0c-js-modules-20261126-v1` (reviewed via fork `review/multi-2d-s0c-js-modules-20261126-v1`) |
| **Reviewed HEAD** | **`8bebbdbe5e6fb179a382984b25f0c07bebc964c3`** |
| Base / context commit | `5648813d9ccbe8d38e9fbe5df38d62c810af4a22` (`origin/integration/2d-sim`, S0b tip) |
| T0 | `47b27a8d3873b32726f315ce1182118da38c50b1` |
| Date | 2026-11-26 |
| Reviewer | Independent non-author review sub-agent — Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, for coordinator thread `c283eca0-…` |
| Author | Gemini 4 Neon (High) — not the reviewer |
| Effort used | **medium**, no escalation to `high` |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0c-Review-20261126` (clean at HEAD before and after every run) |
| Reviewer evidence (external, mode 700) | `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/` → `reviewer-gate/`, `reviewer-gate.log`, `rv-base-ref/`, `rv-base-ref.log`, `rv-static/`, `rv-static.log`, `rv-extract.log`, `rv-selftest/`, `rv-selftest.log`, `reviewer-ss-{before,after}.txt`, `rv-scripts/` |

> **Update 2026-11-26 (R2):** the `NEEDS CHANGES` verdict below is **superseded** by the R2 delta re-review of repair HEAD `ee3f92b4b003f61f8d33d972732d4f4c1632d9f3`. Its result is **`APPROVED`**. See [Delta Re-review R2 (repair 1)](#delta-re-review-r2-repair-1) at the end of this file.

**Verdict (R1, superseded): `NEEDS CHANGES`.** The product change is correct. The ES-module split, the contract, serving and parity all pass on my own runs, and I found **no product defect**. One gate-integrity defect blocks approval: the gate's fail-closed **`selftest.sh` fails 24/26 at this HEAD** (`OVERALL: FAIL … missingCodes=[0]`). S0c-REG requires selftest PASS, and spec S0-02 requires every exit code to be proven. The fix is bounded and harness-only (D-1, plus the recommended D-2). Three non-blocking observations follow.

---

## 1. What I ran myself (I did not rely on the author's evidence)

1. **An independent S0b reference capture.** I did not use the author's `parity-ref/`. I made a disposable `git clone --shared` + sparse checkout (outside the repo, under `rv-work/`) at `49bcd8a4` (T-C1). I first confirmed that `49bcd8a4` differs from base `5648813d` **only** by `tests/s0c-parity/{run-parity-edge.sh,drive-parity.mjs}`, so its `/multi` page is the untouched S0b page. I built `SitkaP1.Ui` there and ran `run-parity-edge.sh --mode reference rv-base-ref` → `OVERALL PARITY: PASS (36/36 outerHTML byte-identical; png identical=35, png diff=1)`, `REF_RC=0`. That single PNG diff is **base vs base** (see §4).
2. **The full gate with the browser tier, comparing against MY reference.** I ran it with `S0C_PARITY_REF=<my rv-base-ref>`, so Stage B is a real S0b→S0c comparison and not the default runA/runB self-check:
   ```bash
   S0C_PARITY_REF=/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/rv-base-ref \
   CCF_P0REF_DIR=…/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref \
     Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --browser --p0ref-dir "$CCF_P0REF_DIR" \
     --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 …/CCF-Multi-2D-S0c-Evidence-20261126/reviewer-gate
   ```
   My first attempt stopped at G0 with exit 3 (`dotnet SDK 10.0.115 does not satisfy global.json`), because my login PATH picked up the wrong SDK. That was an environment fault, not a product fault. I set `DOTNET_ROOT`/`PATH` to `~/.dotnet` (8.0.134, as the author's handoff §7 says) and re-ran on the same clean tree. The log of the failed attempt is kept as `reviewer-gate-attempt1-prereq.log`.
3. **Both static proofs, run directly** (`module-graph.mjs`, `js-reconstruct.mjs --expect 5648813d:…/multi.js`). I recomputed the base blob size and hash with `git show | wc -c | sha256sum`.
4. **An independent line-multiset diff (my own Python, not the author's tool).** I compared every line of the 13 split modules against base `multi.js`, reversing only `export ` and `hooks.` prefixes and the two `svgRenderer.{mount,render}()` calls.
5. **A DOM-selector set comparison**, base `multi.js` against the modules.
6. **Diff reads** of `MultiRoutes.cs`, `SitkaP1.Ui.csproj`, `multi.html`, `multi.css`, `PlayerApi.cs` dispatch, `2d-gate.sh`, `move-proof.py`, `s0a-move-table.json`, `s0a-multi-asset-identity.json`, `ui-smoke-multiarea.sh` and `drive-parity.mjs`.
7. **`build/2d-gate/selftest.sh`**, the gate's fail-closed self-test (result in §2).
8. **Listener before/after** records, a leftover-process check, a capability-token scan of `5648813d..8bebbdbe`, and `git ls-remote` of the protected refs.

---

## 2. Per-criterion verdicts (observed in my own runs)

| Criterion | Verdict | Observed evidence (reviewer run) |
|---|---|---|
| **S0c-01** Native `<script type="module">`, zero npm/bundler/framework, zero inline scripts | **PASS** | `multi.html` delta is exactly one line: `-<script src="multi.js" defer></script>` / `+<script type="module" src="multi/main.js"></script>` (57,612 → 57,625 B). `MODULE-GRAPH PASS: 14 modules, acyclic, all reachable from main.js, 1 module script tag`. Gate S0c asset identity checks the HEAD HTML equals T0 HTML with only that tag replaced. No `package.json`, lockfile, bundler config or framework import exists in the diff, which touches only `Tools/MultiRackSitkaP1Lab/**` + one handoff. M01 asserts no `<script>` (inline) element. |
| **S0c-02** `map/contract.js` + `map/renderer-svg.js` decouple rendering; zero forestry math in JS (D-020) | **PASS** | `contract.js` matches spec §7.3: the same JSDoc, `MAP_RENDERER_METHODS = Object.freeze(['mount','render','setSelection','focusArea','destroy'])`, and an `assertMapRenderer` that throws `TypeError`. `svgRenderer = Object.freeze({ mount(){return initFloorDefs();}, render(){return renderMap();}, setSelection(){}, focusArea(){}, destroy(){} })`. These are 1:1 delegates plus no-ops that S0c never calls, which §7.3 permits. The harness check `assertMapRenderer(svgRenderer)===svgRenderer` and `assertMapRenderer({})` throwing `TypeError` ran inside multiarea 363/0. **D-020:** my multiset diff found **115 added lines, all structural**: 14 provenance comments, 49 `import {…} from '…';` lines, the 19-entry sealed `hooks` registry, 19 `hooks.X = X;` assignments, the 7-line `svgRenderer` adapter and 2 blank lines. The only removed lines are `'use strict';`, `(function () {` and `})();`. **No arithmetic or logic line was added.** |
| **S0c-03** Explicit allow-list serving, correct MIME, CSP/Host/Origin/capability guards intact, invalid module paths 404, no disk reads | **PASS** | `MultiRoutes.cs`: 14 `(route, MultiEmbeddedAsset(logicalName))` tuples, registered by `foreach` into the `routes` `Dictionary<string,Route>(StringComparer.Ordinal)` with `"text/javascript; charset=utf-8"`. The `multi.js` route is removed. `MultiEmbeddedAsset` throws at type-init if a logical name is missing, so the server fails closed. The csproj has 14 matching `EmbeddedResource … LogicalName` entries (all present, no duplicates; G1 duplicate-LogicalName check PASS). Dispatch (`PlayerApi.HandleAsync`, unchanged) runs `SetSecurityHeaders` → Host guard → capability `FixedTimeEquals` → exact `routes.TryGetValue`, else 404 → method → Origin guard. There is no static-file middleware and no `File.*` read. `git diff 5648813d..HEAD -- src/Ui/*.cs` touches **only** `MultiRoutes.cs` (+19/−2). `PlayerApi.cs`, which holds `SetSecurityHeaders`, `WriteAssetAsync` and the guards, has 0 changes. Live gate check: all 14 modules return 200 with the committed bytes and the T0 JS header set (CSP etc.) except Content-Length; `multi.js` → **404**. Parity network logs show **0** 4xx/5xx across 4×34 requests. |
| **S0c-04** Zero edits to Core/wwwroot/Assets/Packages/ProjectSettings/Docs-Project/main; zero `data:image/webp`; zero capability tokens in Git | **PASS** | `git diff --name-only 5648813d HEAD -- src/Core src/Ui/wwwroot Assets Packages ProjectSettings Docs/Project` → **0** files. G7 PASS (70 paths; wwwroot tree `dabdbd7f…`; P0 tree `f212c8f6…`). `git grep data:image/webp HEAD -- src/Ui` → 0; M08 PASS. `git log -p 5648813d..8bebbdbe` matches for `/s/[0-9a-f]{64}` → **0**. `git ls-remote`: `main`=`bdeb6a32…`, `integration/2d-sim`=`5648813d…` (unchanged), feature=`8bebbdbe…`. |
| **S0c-05** Save Edge 63/63 and Step 2 Edge 67/67, 0 console errors / 0 CSP / 0 4xx-5xx | **PASS** | Stage B: `stage-b-save-edge` **OVERALL: PASS (63 passed, 0 failed)**; `stage-b-step2-edge` **OVERALL: PASS (67 passed, 0 failed)**. S0c parity, per cell (ma-1, ma-1-mixed × 1280×720, 1920×1080): `7 passed, 0 failed`; zero console errors, zero CSP violations, zero HTTP 4xx/5xx, `nonLoopback: 0`, 34 requests per cell, all `http://127.0.0.1:<port>`. |
| **S0c-06** (= spec **S0-09** composite: ES modules, byte-identical reconstruction, acyclic local graph, `assertMapRenderer`, outerHTML parity at both viewports/experiments, loopback-only) | **PASS** | No criterion named S0c-06 is defined in the spec or the brief, so I mapped it to S0-09, the S0c acceptance row. `JS-RECONSTRUCT PASS: byte-identical … (247066 B, 4617 lines, sha256=fbc7c8179dbd42605a55d620fbc903b0f4ccbad2805f33270f28fa7eec9de520, P4=49 sites across 19 hooks, P5=8 sites)`. I independently recomputed the base blob: 247,066 B, the same sha256. Parity against **my own** S0b reference: **`htmlIdenticalCount 36 / htmlDiffCount 0`**; PNG 33/36 identical, 3 accepted (§4). |
| **S0c-REG** Gate exit 0, G0–G7 PASS, 0W/0E, suites, anchors, budgets, selftest, P0 seed, servers stopped | **FAIL** (selftest only; every other item PASS) | `VERDICT: PASS (exit=0; NETWORK ISOLATION: PROVEN; P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781…)`. G0–G7 + B all PASS. G1: sln / Ui / Save builds `0 Warning(s) 0 Error(s)`. G2: Core **282/282**; Save **50/50**; ui-smoke-multiarea **363/0**; ui-smoke **250/4** (exact `{L01,L02,L06,J31}`, G11b PASS; the frozen baseline in `baseline-counts.json` is unchanged); cli-safety **23/0**; build-output-safety C1–C6+P0 PASS, C7/C8 HOST-NA (Amendment 1), `.p0ref restored & re-verified`; p0-isolation decoy 13/13, hits 0. G3 **9/9** anchors. G4 v2 golden 24,703 B. G5 **5/5** budgets. G6 F1=0, RNG violations 0. G7 0 violations. Listeners identical before and after; no `sitkap1-ui`/`msedge`/parity process left. **Selftest (`build/2d-gate/selftest.sh`, ~45 min, external `TMPDIR`): `OVERALL: FAIL (24/26 passed, 2 failed, missingCodes=[0])`, exit 1.** C14 and C26 fail. See D-1. |

---

## 3. Findings

### Blocking

**D-1 (Major, gate integrity — S0c-REG / S0-02). `build/2d-gate/selftest.sh` fails 24/26 at HEAD and no longer proves exit code 0.**
- *Observed:* `rv-selftest.log`:
  - `FAIL C14-missing-marker (expected=1 actual=1) … (output missing pattern: fail-closed marker check failed)`
  - `FAIL C26-clean-uismoke-exact-four (expected=0 actual=1)`
  - `OVERALL: FAIL (24/26 passed, 2 failed, missingCodes=[0])`, `EXIT=1`.
- *Root cause:* `2d-gate.sh` has had a Stage B policy since base `5648813d` (lines ~1866–1883): `BROWSER_REQUIRED=1` whenever `build/2d-gate/s0c-hook-inventory.json` or `tests/s0c-parity/run-parity-edge.sh` exists. Without `--browser`, the gate then fails `GATE B FAIL … browser tier is required for this packet` with exit 1. Neither trigger file exists at base, and the S0b handoff records 26/26. S0c adds both files, but `selftest.sh` is unchanged (0-line diff). As a result:
  - C26, the only clean exit-0 case, runs without `--browser` and now exits 1. That removes exit-0 coverage.
  - C14 still exits 1, but from the Stage B policy instead of the fail-closed marker check it is meant to prove. So the missing-marker protection is no longer tested.
  - The case logs `case-C26…/gate.log` and `case-C14…stdout.log` both show G0–G7 PASS followed by `GATE B FAIL`.
- *Not a product defect:* the gate policy is correct and must **not** be weakened. The self-test is what needs to adapt to it.

**D-2 (Minor, gate-coverage regression in the same file; fix together with D-1).** `selftest.sh` C21 (`mut_asset_identity`) still appends to `src/Ui/multi/multi.js`. At HEAD that file is deleted, so the case now only proves "legacy `multi.js` re-added → exit 1" (`S0c mode: retired … multi.js still exists at HEAD`). It no longer proves that tampering with a served asset is caught. No self-test case tampers a committed ES module or the hook/call-site inventories.

### Non-blocking observations

**O-1 (Minor, process — file-set deviation, ACCEPTED).** The T-C4 exact file set names `module-graph.mjs`, `js-reconstruct.mjs`, "the G2 wiring lines in `2d-gate.sh`", `s0a-multi-asset-identity.json` and `ui-smoke-multiarea.sh`. The author also changed `build/2d-gate/move-proof.py` and `s0a-move-table.json`, and extended the `s0c-css-plus-html-delta` Python block in `2d-gate.sh` beyond plain wiring. These changes are required for the gate to keep passing after the P6 `MultiRoutes.cs` change that spec §7.2 authorises. Each one keeps or tightens enforcement (rulings in §3.1). They follow the S0b `advanceExtraction` precedent. No action is required; the integrator should note the deviation in the S0c `INTEGRATION DECISION`.

**O-2 (Nit, gate hardening, non-blocking).** `move-proof.py`'s S0c expansion replaces the T0 lines at `staticFieldT0Line` (55) and `routeAddT0Line` (75) without asserting their old content. `oldStaticFieldName` is read into the table but never checked. I checked by hand that T0 (`47b27a8d:PlayerMultiApi.cs`) line 55 is `private static readonly byte[] MultiPageJs = …` and line 75 is `routes.Add("multi.js", …)`, so the current table is correct. A future edit to the table with a wrong line number could still silently swap out a real line. The swapped-in text would remain exact and reviewed, and the full-multiset equality would still hold. Suggested hardening for a later gate pass (not S0c): assert that `adjusted_t0[ln-1]` contains `oldStaticFieldName` / `routes.Add("multi.js"`.

**O-3 (Nit, observation).** `js-reconstruct.mjs` drops everything after the `// S0c P4 hooks registry` marker in `store.js` and after the `// S0c P5 svgRenderer adapter` marker in `renderer-svg.js` without checking it, and it does not cover `map/contract.js`. Code appended after those markers would escape the byte proof. I read those tails and `contract.js` in full. They contain only the 19 `null` hook entries, the 7-line adapter and the §7.3 contract, and my independent multiset diff (§2 S0c-02) confirms nothing else was added. **No defect at this HEAD.** Future packets that edit these modules should not rely on js-reconstruct alone. After S0c it no longer applies, because the base `multi.js` is gone.

### 3.1 Gate-harness deviation rulings

| Change | Ruling | Reason |
|---|---|---|
| `build/2d-gate/move-proof.py` (+29) and `s0a-move-table.json` (+32, `parts.ui.s0cModuleRoutes`) | **ACCEPTED** | The allowance is active only when `s0cModuleRoutes.mode == "s0c-module-routes"`. It expands exactly two T0 line numbers into the 17 + 2 literal replacement lines in the committed table, and those lines are byte-equal to the HEAD `MultiRoutes.cs` lines I read. Every other kept line is still verified (a) as an in-order contiguous block via `find_subsequence(effective_lines(...))` and (b) by **full line-multiset equality** across `MultiRoutes.cs` + `MultiApi.*.cs`, so any extra, missing or changed line fails. The static-field invariant now expects `MultiPageModules` in the same position. A real edit cannot pass unless it is spelled out in the table, and the table is itself reviewed (O-2 is the only weakness). Gate result: `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1263 non-empty lines); core(7 files, 8 blocks, 1337 …)`. |
| `s0a-multi-asset-identity.json` mode → `s0c-css-plus-html-delta` | **ACCEPTED** | T-C4 names this exact change. The S0c branch is **stricter** than before. CSS must equal T0 `R_css`, with BOM/CR/trailing-LF rules. HTML must equal T0 HTML with exactly one tag substitution (T0 must contain the old tag exactly once). `multi.js` must not exist at HEAD. A live server must return the exact bytes + headers for `multi`, `multi.css` and every committed `src/Ui/multi/**/*.js`, using the T0 JS header template, and 404 for `multi.js`. Each failure calls `sys.exit(…)`, which fails closed. |
| `tests/ui-smoke-multiarea.sh` (+93) | **ACCEPTED** | M01 now expects the module tag and still forbids inline `<script>` and inline `style`. M04 checks all 14 module routes (200 + exact MIME) and the 404 on `multi.js`. M05 runs `node --check` on each served module. M08 now scans all 14 modules for `data:image/webp`. The J-loader moved from `vm.runInContext(multi.js)` to `vm.SourceTextModule`, with a linker that refuses non-relative specifiers, other origins, paths outside `multi/` and redirects. `assertMapRenderer` checks were added. **No J/G/W assertion text was changed or removed.** The diff touches only the M-series lines, the loader and the node flags. The total is still exactly **363** checks, which equals the frozen `uiSmokeMultiarea.minPassed` in the unchanged `baseline-counts.json`. |
| `build/2d-gate.sh` (+142) | **ACCEPTED** | The diff is confined to (a) the `s0c-css-plus-html-delta` branch above and (b) two `cat … >&2` / `log_line` lines that surface module-graph / reconstruct output. Stage order, G0 preflight, G1 offline `unshare -rn` builds, G3 anchors, G4 golden, G5 budgets, G6 determinism grep, G7 protected-path/wwwroot/P0-tree guards and exit-code classes are **unchanged**. The Stage B parity hook (`run-parity-edge.sh` with `fail_stage B` on non-zero) was already present at base. |
| `tests/s0c-parity/drive-parity.mjs` `sessionId` canonicalisation | **ACCEPTED — does not mask behaviour** | After the initial render it reads the live `sessionId` from `#facts` (`dt` "Session" → `code.full`). It fails the cell if the id is missing or shorter than 16 chars. It replaces only that exact string and its exact 12-char `expandable()` prefix + `…`. `MultiAreaContinuation.NewSessionId()` gives a fresh random 32-hex id per server process, so it cannot be byte-stable across two boots, even base vs base. No other canonicalisation is applied to `outerHTML`. `redactCap` is used only on network/console log text, never on the hashed HTML. A behaviour change that altered *where* or *whether* the session id appears would still show up, because the placeholder position is compared. |

---

## 4. PNG pixel-diff ruling

My Stage B comparison (S0c HEAD against **my** S0b reference) found 3 of 36 screenshots with tiny pixel differences. All 36 `outerHTML` were byte-identical.

| Cell / state | Differing px | bbox `[x0,y0,x1,y1]` | Where | Same-build reproduction |
|---|---|---|---|---|
| `ma-1-mixed 1280×720 03-fell-draft-pending` | 15 / 921,600 | `[909,5,924,12]` | Dashed gold border corner of the topbar "Skip → Yr 4" button | **Identical diff (15 px, same bbox) in my base-vs-base reference self-determinism run** (`rv-base-ref/self-diffs/`) |
| `ma-1-mixed 1920×1080 04-approved-plan` | 8 / 2,073,600 | `[1242,1066,1498,1067]` | 2-px strip at the bottom viewport edge (legend / work-plan pane scroll edge) | Same 8 px / bbox in the author's S0c-vs-S0c `parity-selfcheck` and in both author gate runs |
| `ma-1 1920×1080 03-fell-draft-pending` | 4 / 2,073,600 | `[1431,19,1444,19]` | One row at the dashed border of the same "Skip" button | Same 4 px / bbox in the author's standalone compare. It is the same artefact class (dashed-border rasterisation) as the base-vs-base 15-px case |

I viewed the archived diff PNGs. Every changed pixel lies on a dashed-border edge or a viewport/scroll edge, with no text, glyph, icon, map or layout change. Identical diffs occur between two runs of the **same** build, including the untouched base. The DOM is byte-identical in every case, and CSS is byte-identical to T0.

**Ruling: ACCEPTED as non-semantic headless-Edge rasterisation artefacts** under spec §7.5, with the bounding boxes recorded above. The author's diffs (`22 px [1262,13,1263,172]` and `8 px` in `final-gate/`, and `4 px` in `parity/`) are the same class and are also accepted.

---

## 5. Listener honesty and repository hygiene

- `ss -tln` before and after my gate run: **identical**. Only the WSL DNS listeners (`127.0.0.53%lo:53`, `127.0.0.54:53`, `10.255.255.254:53`). `:33767`, `:43649`, `:46593` were never bound. No `sitkap1-ui`, `msedge` or parity process survived.
- The review worktree was clean (`git status --porcelain` empty) before, during and after every gate/selftest run. The only file I add is this review.
- `/home/jer/Development/CCF-HP` (`main`) and the feature worktree were not touched.
- Capability tokens: 0 in Git history `5648813d..8bebbdbe`. The browser tier's `run-edge-e2e.sh` writes per-process capability URLs into **external** evidence logs (a known, pre-existing behaviour). I redacted them in my own evidence copies (`/s/<capability>`). Those tokens were for servers that had already stopped.

## 6. Not verified / residual risk

- The C7/C8 cold P0 rebuild on Ubuntu 24.04 + SDK 8.0.133 was not run. It is HOST-NA under Amendment 1 §4.6, as in S0a/S0b.
- Real (non-headless) Edge, other browsers and DSF ≠ 1 were not exercised. Parity covers the 36 specified states only.
- The noise PNG diffs are ruled on evidence (same-build reproduction). I did not root-cause them inside Edge.
- After the merge, a plain `2d-gate.sh --browser` without `S0C_PARITY_REF` runs Stage B parity in **self-check** mode (runA vs runB on the same build), not against S0b. This is correct for trunk after S0c, but it is no longer a refactor-equivalence proof. The S0b→S0c proof is this review plus the author's compare run.
- Merge / post-merge gate (T-C6) is the integrator's job and is not covered here.

## 7. Checklist sign-off (reviewer-observed only)

- [x] Reviewed exact HEAD `8bebbdbe5e6fb179a382984b25f0c07bebc964c3`; tree clean
- [x] Own gate run `--browser --base 5648813d` exit 0, G0–G7 + B PASS
- [x] Own independent S0b reference; parity 36/36 outerHTML identical
- [x] Reconstruction byte-identical; base sha recomputed
- [x] Module graph: 14 modules, acyclic, reachable, one module tag, static relative imports only
- [x] Independent line-multiset + selector-set diff: structural additions only, no logic, no DOM selector change, indentation preserved
- [x] Zero `eval` / `new Function` / dynamic `import(` / re-export / `data:` / external URLs (only the `SVG_NS` constant)
- [x] Route allow-list, logical names, MIME, guards unchanged; `multi.js` 404
- [x] `multi.html` one-line delta; `multi.css` byte-identical (sha `f3b2b68a…`)
- [x] Protected paths 0 edits; `origin/main` and `origin/integration/2d-sim` unchanged
- [x] Save Edge 63/63, Step 2 Edge 67/67
- [x] Harness deviations ruled (all accepted); PNG diffs ruled (accepted)
- [x] Listeners unchanged; no surviving servers; 0 tokens in Git
- [ ] **Gate selftest PASS — NOT MET:** observed 24/26, `missingCodes=[0]` (D-1, D-2)

---

> **SUPERSEDED (history).** The R1 decision block below was replaced on 2026-11-26 by the R2 `INTEGRATION DECISION` (`APPROVED`) in [Delta Re-review R2 (repair 1)](#delta-re-review-r2-repair-1).

```text
INTEGRATION DECISION
Status: NEEDS CHANGES   [SUPERSEDED 2026-11-26 by R2 — APPROVED]
Gate: G2 (acceptance) — RG-S0c independent review of packet S0c, incl. 2d-gate.sh G0–G7 + browser tier B + gate selftest
Reviewed branch: feat/multi-2d-s0c-js-modules-20261126-v1
Reviewed HEAD: 8bebbdbe5e6fb179a382984b25f0c07bebc964c3
Context commit: 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 (origin/integration/2d-sim, S0b tip)
SPEC (SPEC REQUIRED only):
- ID / status: SPEC-multi-2d-s0-seam-extraction-V1 / APPROVED (+ Post-Adoption Amendment 1)
- Conformance: FAIL on S0-02 / S0c-REG (gate selftest 24/26, exit 0 unproven); PASS on S0-09, §7.1–§7.5, D-020 and all other regression gates
Reasons:
- BLOCKING D-1: build/2d-gate/selftest.sh OVERALL: FAIL (24/26, missingCodes=[0]); C14 and C26 hit the pre-existing S0c browser-required Stage B policy instead of their intended checks.
- Reviewer gate run exit 0: G0–G7 + B PASS; Core 282/282, Save 50/50, multiarea 363/0, ui-smoke 250/4 {L01,L02,L06,J31}, CLI 23/0, 9/9 anchors, 5/5 budgets, 0W/0E, P0 seed 9a99f781… restored.
- js-reconstruct byte-identical to base multi.js (247,066 B, sha256 fbc7c817…); independent multiset diff shows structural additions only.
- 36/36 document.body.outerHTML identical vs reviewer's own S0b reference; Save Edge 63/63; Step 2 Edge 67/67; 0 console/CSP/4xx-5xx; loopback only.
- Explicit 14-route allow-list, exact MIME, guards/CSP unchanged, multi.js 404; protected paths untouched.
- Gate-harness deviations accepted (enforcement preserved or tightened); 3 PNG diffs accepted as non-semantic Edge artefacts (bboxes recorded).
Required fixes / remaining gates:
1. (D-1) Update ONLY Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh so it honours the S0c Stage B policy without weakening 2d-gate.sh:
   (a) C26 (clean case) must again prove exit 0 with the exact-four ui-smoke pattern. Run it with --browser (S0C_PARITY_REF unset → self-check parity), or use another path that really reaches VERDICT: PASS exit 0;
   (b) C14 must again fail on "fail-closed marker check failed". Run it with --browser, or check that the marker assertion is reached under --headless-only and expect its real code/pattern;
   (c) do not change BROWSER_REQUIRED, the B-required failure or any expected code to make cases pass.
2. (D-2) In the same file, make the asset-identity tamper case target a committed ES module (e.g. append to src/Ui/multi/main.js → expect "S0a /multi asset identity failed"/exit 1). Optionally add a case that tampers s0c-hook-inventory.json → expect "S0c js-reconstruct.mjs failed"/exit 1. Keep the legacy-multi.js re-add check as its own case if desired.
3. Re-run and publish in an updated handoff: selftest.sh → OVERALL: PASS (all cases, missingCodes=[]), plus 2d-gate.sh --browser --base 5648813d exit 0 on the new HEAD.
4. Delta re-review (RG-S0c R2) of the selftest.sh diff and the selftest evidence only. Product files and parity need no re-review if they are unchanged (verify with an empty diff of src/** and tests/s0c-parity/** vs 8bebbdbe).
- Optional later hardening (non-blocking): O-2 (assert T0 placeholder content in move-proof), O-3 (bound js-reconstruct tail stripping).
- After APPROVED: T-C6 integrator merge (--no-ff) with green `2d-gate.sh --browser` on the rebased HEAD and the merge commit.
Integration target: origin/integration/2d-sim
Integrated HEAD: <only after integration>
Post-integration verification:
- Integrator to run build/2d-gate.sh --browser --base <pre-merge trunk> on the merge commit.
```

---

## Delta Re-review R2 (repair 1)

| Field | Value |
|---|---|
| Delta | `8bebbdbe5e6fb179a382984b25f0c07bebc964c3..ee3f92b4b003f61f8d33d972732d4f4c1632d9f3` (3 commits: `36834d69`, `0615a677`, `ee3f92b4`) |
| **Re-reviewed HEAD** | **`ee3f92b4b003f61f8d33d972732d4f4c1632d9f3`** |
| Date | 2026-11-26 |
| Reviewer | Same independent non-author reviewer as R1 — Claude Opus 5.5 (`claude-opus-5-5`) |
| Effort | **medium**, no escalation |
| Verification worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0c-R2Verify-20261126` (detached at `ee3f92b4`; clean before and after) |
| Reviewer evidence (external, mode 700, capability URLs redacted) | `/home/jer/Development/CCF-Multi-2D-S0c-Evidence-20261126/r2/` → `selftest/`, `selftest.log`, `gate/` + `gate.log` (run 1), `gate2/` + `gate2.log` (run 2), `ss-*.txt`, `pre.txt`, `post.txt`, `scripts/`; aborted attempt kept as `r2-attempt1-sigint-ignored/` |
| Repair count | **1** |

**R2 verdict: `APPROVED`.** Both selftest defects are fixed without weakening the gate. I accept the new transient-retry guard. My own selftest is 29/29 PASS with every exit code proven, and my own `--browser` gate run exits 0.

### R2.1 The delta is product-free

- `git diff --name-status 8bebbdbe..ee3f92b4` lists exactly 2 files: `M Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh` (`--numstat` +56/−8; the brief said "+46/−10") and `M Docs/Handoffs/…/S0C-IMPLEMENTATION-HANDOFF-20261126.md` (+44/−2).
- `git diff --stat 8bebbdbe ee3f92b4 -- src tests build/2d-gate.sh build/2d-gate/*.{json,mjs,py} Assets Packages ProjectSettings Docs/Project` → **empty**. `2d-gate.sh`, every module, route, csproj, parity driver, inventory, identity JSON, the move-proof, Core, wwwroot, Assets, Packages, ProjectSettings and Docs/Project are all unchanged. The R1 product verdict therefore stands as written.
- `git log -p 8bebbdbe..ee3f92b4` contains **0** capability tokens.
- `git ls-remote` before and after my runs: `main` = `bdeb6a324acaf4129c10fb0846faac8b64974360`, `integration/2d-sim` = `5648813d9ccbe8d38e9fbe5df38d62c810af4a22` (both unchanged), feature = `ee3f92b4…`.

### R2.2 D-1: **FIXED**

- **C26** now runs `mut_noop --browser`, with `S0C_PARITY_REF` explicitly `unset` at the top of `selftest.sh`, so parity runs its single-argument self-check. Observed `case-C26…stdout.log`: `GATE G7 PASS`, `GATE B PASS`, `VERDICT: PASS (exit=0; NETWORK ISOLATION: PROVEN; P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781…)`, ui-smoke `PASS-BASELINE-DRIFT (250 passed, 4 known … L01,L02,L06,J31; G11b PASS)`. Stage B in that case: Save Edge `63 passed, 0 failed`; Step 2 Edge `67 passed, 0 failed`; parity `36/36 outerHTML byte-identical`. Exit 0 is again genuinely proven by a full clean run.
- **C14** now runs `mut_missing_marker --headless-only`. In `2d-gate.sh` the final marker check runs **before** the `--headless-only` exit 5, so the suppressed marker is what produces exit 1. Observed: `GATE B NOT RUN` → `fail-closed marker check failed: missing=['G7'], failed=[]` → `VERDICT: FAIL (exit=1)`. **C15** (the same flag, no mutation) gives `VERDICT: PARTIAL (exit=5)`. This control shows C14's exit 1 comes only from the missing marker, not from the Stage B policy.
- `BROWSER_REQUIRED`, the B-required failure and all expected exit codes are untouched, since `2d-gate.sh` has a 0-line diff.

### R2.3 D-2: **FIXED**

Every new case reaches the check it names, and the observed failure is the specific enforcement message, not a generic crash:

| Case | Mutation | Observed failure (my run) | Genuine? |
|---|---|---|---|
| C21 | appends ` /* … */` (no trailing LF) to committed `multi.css` | `S0c mode: multi.css no longer matches T0 R_css` → `ERROR [G2]: S0a /multi asset identity failed` → exit 1 | Yes. Because no LF is added, the earlier BOM/CR/LF rule passes and the T0 SHA comparison is what fires. |
| C27 | appends `\n// selftest main.js tamper` to committed `main.js` | asset identity PASS (live == committed, as designed), then `JS-RECONSTRUCT FAIL: byte mismatch at line 4617 (reconstructed 247094 B vs expected 247066 B)` → `S0c js-reconstruct.mjs failed` → exit 1 | Yes. The +28 B is exactly the appended text, so the byte proof is the check that catches a served-module tamper. |
| C28 | re-adds retired `multi.js` | `S0c mode: retired …/multi.js still exists at HEAD` → exit 1 | Yes. This keeps R1's surviving check as its own case. |
| C29 | `hooks[0].callSites[0].originalLine += 1` in `s0c-hook-inventory.json` | `JS-RECONSTRUCT FAIL: P4 reverse mismatch at line 421: expected "hooks.clearCompetition" in "…hooks.competitionTag();"` → exit 1 | Yes. It fails on the targeted P4 rewrite, not on a JSON parse error. |

No pre-existing case was weakened. The diff changes only C14 (adds `--headless-only`), C21 (target and pattern), C26 (adds `--browser`) and the case banner text, and adds C27–C29. C01–C13, C15–C20 and C22–C25 are byte-unchanged. The collation now requires `len(rows) != 29` → FAIL, and missing exit codes still fail.

### R2.4 Transient-retry guard (`run_full_case`): **ACCEPTED**

Reasons:
1. **It never accepts a failure.** On a match it runs `continue`, which deletes `case_ev` and re-runs the **whole** unmodified `2d-gate.sh` case on the same mutated clone. A case passes only when a later complete attempt meets its expected exit code and pattern on its own. After the last attempt the final FAIL row stands.
2. **It is bounded.** The guard shares one `attempt` counter (1..3) with the pre-existing NB1c retry, so the combined maximum is 3 runs per case and no extra loop is added.
3. **It can hide only non-determinism, never a deterministic regression.** A real defect that reproduces would fail all 3 attempts. The guard's signature is also narrow. It needs the B-stage parity failure line in `gate.log`, a failed-request list that is exactly one element (`GET …/multi` `net::ERR_CONNECTION_REFUSED`; the `: [{` anchor right after the label rules out a multi-element list), 9/9 states, 0 console errors, 0 CSP violations and no `FAIL HTML DIFF`. A server that never listens would capture no states. A regression that makes the page or modules fail would show module 404s, console errors or HTML diffs. The refused-first-navigation pattern matches a known machine fact: Windows-side Edge reaching a WSL `127.0.0.1` listener is refused for ~0.3–0.7 s after bind, and `run-parity-edge.sh` waits only for the URL to be printed, not for Windows-side reachability.
4. **It lives only in `selftest.sh`.** The gate itself still fails closed on this transient, which is the safe direction.
5. **Did it fire in my run? No.** `ERR_CONNECTION_REFUSED` appears 0 times in my `selftest.log`. The pre-existing NB1c retry fired once (`retrying C13-g7-assets (attempt 2/3) … NB1c`), and C13 then passed with exit 4.

Non-blocking observations on the guard (no action needed for S0c):
- **O-4 (Nit).** The positive greps (`captured all 9`, `zero console errors`, `zero CSP`) are not scoped to the cell that had the refusal. A run where cell A had the transient refusal while cell B failed for another reason (for example a console error) would also be retried. That costs at most a re-run, so it can only reduce flake detection, never pass a defect. Tightening it to "no `^FAIL ` line other than the one refused-request line" would make it exact.
- **O-5 (Minor, future hardening).** The root fix belongs in the parity harness: a Windows-side reachability retry with a timeout before `Page.navigate` in `drive-parity.mjs` / `run-parity-edge.sh`. That would also remove the flake from real gate runs. Out of scope for this harness-only repair.

### R2.5 My independent re-run (same machine, no concurrent load, external `TMPDIR`)

**Selftest:** `TMPDIR=<ev>/r2/tmp-selftest Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh --p0ref-dir <9a99f781 seed> <ev>/r2/selftest`, 11:39:13Z → 12:39:49Z (~61 min, including one NB1c retry):

```text
PASS C01 … C29 (every case expected == actual)
OVERALL: PASS (29/29 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven)
EXIT=0
```
`selftest-summary.json`: `missingCodes=[]`.

**Gate:** `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --browser --p0ref-dir <seed> --base 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 <ev>` (no `S0C_PARITY_REF`, so self-check parity):
- **Run 1, exit 1:** G2 `Phase1.Save.Tests` 49/50, `FAIL NB1c: … expected 1, got 3`. This is the exact signature of the documented frozen NB1c order race (~7.5% at T0 and HEAD alike). It is not caused by the delta, which touches no product or test code. Log kept as `r2/gate.log`.
- **Run 2, exit 0:** `VERDICT: PASS (exit=0; NETWORK ISOLATION: PROVEN; P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a3…)`. G0–G7 + B PASS. Core 282/282, Save 50/50, multiarea 363/0, ui-smoke 250/4 `{L01,L02,L06,J31}` G11b PASS, CLI 23/0, build-output-safety C1–C6+P0 PASS (C7/C8 HOST-NA), `.p0ref restored & re-verified`, asset identity PASS, MOVE-PROOF PASS, MODULE-GRAPH PASS (14 modules), JS-RECONSTRUCT PASS (247,066 B, sha256 `fbc7c817…`), G3 9/9 anchors, G4 v2 golden 24,703 B, G5 5/5 budgets, G6 F1=0 / RNG violations 0, G7 PASS. **Save Edge 63/63; Step 2 Edge 67/67; parity 36/36 outerHTML byte-identical.** All 8 parity drives had 0 console errors, 0 CSP violations, 0 failed or 4xx/5xx requests, and 34 loopback-only requests each.
- Run 2 parity was self-check mode (the same build twice). It noted 6 PNG diffs (3–22 px), all on the dashed border of the "Skip → Yr 4" button or the bottom viewport edge, the same artefact class accepted in R1 §4. One new bbox is the 5-px `[1448,12,1450,13]` at 1920×1080 `03-fell-draft-pending`, at the same button. Because they are build-vs-same-build, they are non-semantic by construction.

**Hygiene:** `ss -tln` was identical before, during and after every run. `:33767`, `:43649` and `:46593` were never bound. No `sitkap1-ui`, `msedge`, parity or gate process was left over (none existed before either). `git status --porcelain` was empty for the verification worktree and the real feature worktree (still at `ee3f92b4`) before and after. `/home/jer/Development/CCF-HP` was not touched. Capability URLs in 12 external evidence logs (pre-existing `run-edge-e2e.sh` behaviour) were redacted to `/s/<capability>`.

**Reviewer-side incident (disclosed):** my first selftest launch (`setsid nohup … &`) inherited SIGINT/SIGQUIT/SIGHUP as ignored (`SigIgn=0x7`). As a result, every full case failed `ui-smoke-multiarea` Z01 ("still running 15 s after SIGINT", exit 137). That was my launch environment, not the repair. I stopped only that process session, kept its logs as `r2-attempt1-sigint-ignored/`, and re-ran from scratch with SIGINT/HUP/PIPE/XFSZ reset to default. Only SIGQUIT stayed ignored, and nothing in the suite sends SIGQUIT. The result above is from that clean re-run (multiarea 363/0 in every full case).

### R2.6 Checklist

- [x] Delta = 2 files, product-free; `2d-gate.sh` unchanged; protected refs unchanged
- [x] D-1: C26 real exit 0 with `--browser`; C14 fails on the marker (exit 1), with C15 as the exit-5 control
- [x] D-2: C21 / C27 / C28 / C29 each hit their specific enforcement message; no case weakened; count = 29
- [x] Retry guard: re-runs only, bounded to 3 shared attempts, narrow signature; did not fire in my run
- [x] Own selftest `OVERALL: PASS (29/29 …)`, exit 0, `missingCodes=[]`
- [x] Own gate `--browser` exit 0; Save Edge 63/63; Step 2 Edge 67/67; parity 36/36
- [x] Listeners untouched; no leftover processes; worktrees clean

```text
INTEGRATION DECISION
Status: APPROVED
Supersedes: the RG-S0c R1 INTEGRATION DECISION "NEEDS CHANGES" of 2026-11-26 for HEAD 8bebbdbe5e6fb179a382984b25f0c07bebc964c3 (kept above as history)
Date: 2026-11-26
Gate: G2 (acceptance) — RG-S0c delta re-review R2 of packet S0c repair 1, incl. own 2d-gate.sh --browser run and own gate selftest
Reviewed branch: feat/multi-2d-s0c-js-modules-20261126-v1
Reviewed HEAD: ee3f92b4b003f61f8d33d972732d4f4c1632d9f3
Delta reviewed: 8bebbdbe5e6fb179a382984b25f0c07bebc964c3..ee3f92b4b003f61f8d33d972732d4f4c1632d9f3 (build/2d-gate/selftest.sh + S0c implementation handoff only)
Context commit: 5648813d9ccbe8d38e9fbe5df38d62c810af4a22 (origin/integration/2d-sim, S0b tip)
Repair count: 1
SPEC (SPEC REQUIRED only):
- ID / status: SPEC-multi-2d-s0-seam-extraction-V1 / APPROVED (+ Post-Adoption Amendment 1)
- Conformance: PASS on S0-02 / S0c-REG (selftest 29/29, all exit codes proven) and, unchanged from R1, PASS on S0-09, §7.1–§7.5, D-020 and every other regression gate
Reasons:
- D-1 FIXED: C26 --browser reaches VERDICT: PASS exit 0; C14 --headless-only fails on "fail-closed marker check failed: missing=['G7']" (exit 1; C15 control exit 5). 2d-gate.sh BROWSER_REQUIRED policy unchanged (0-line diff).
- D-2 FIXED: C21 multi.css → "no longer matches T0 R_css"; C27 main.js → js-reconstruct byte mismatch; C28 legacy multi.js re-add; C29 inventory → P4 reverse mismatch. No case weakened; collation requires 29 rows.
- Transient ERR_CONNECTION_REFUSED retry ACCEPTED: selftest-only, re-runs the full case (never accepts a failure), bounded to 3 attempts shared with NB1c, narrow single-request signature; it did not fire in the reviewer run.
- Reviewer selftest: OVERALL: PASS (29/29 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven), exit 0, missingCodes=[].
- Reviewer gate --browser --base 5648813d: exit 0, G0–G7 + B PASS; Core 282/282, Save 50/50, multiarea 363/0, ui-smoke 250/4 {L01,L02,L06,J31}, CLI 23/0, 9/9 anchors, 5/5 budgets; Save Edge 63/63, Step 2 Edge 67/67, parity 36/36 outerHTML identical (first attempt hit the documented frozen NB1c flake, exact signature; second attempt clean).
- Product files byte-unchanged since R1, so the R1 product findings (no product defect; harness deviations and PNG diffs accepted) carry forward.
Required fixes / remaining gates:
- None for S0c review.
- Non-blocking, later: O-2 and O-3 (from R1); O-4 (scope the retry guard's PASS greps per cell); O-5 (Windows-side reachability retry before Page.navigate in the parity harness).
- Next: T-C6 integrator merge (--no-ff) into origin/integration/2d-sim with a green build/2d-gate.sh --browser on the rebased HEAD and on the merge commit. Record the O-1 file-set deviation in the merge record.
Integration target: origin/integration/2d-sim
Integrated HEAD: <only after integration>
Post-integration verification:
- Integrator to run build/2d-gate.sh --browser --base <pre-merge trunk> on the merge commit (and selftest.sh if the merge changes any gate file).
```
