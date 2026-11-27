# Phase 2B-2a Combine — Independent Delta Review (S0c lock retirement)

- **Reviewer:** claude-opus-5-5 (independent delta reviewer, medium effort), read-only on code
- **Date:** 2026-11-27
- **Branch reviewed:** `origin/integrator/2d-phase2b2a-s0c-lock-retirement-20261126-v1` @ `6a4a1a5a55d96993e644fe7dcb3b4bbe3c9e8e47`
- **Review target:** integrator-prep commit `bc2e8abc0a4803254206b3e72cc72aee53372273` ("build(2d-gate): retire S0c T0 asset byte-identity and js-reconstruct locks for Phase 2B-2a"), parent `9f480ecd`
- **Trunk merge:** `6a4a1a5a` = merge of `bc2e8abc` + `origin/integration/2d-sim` @ `aa958310f30cc9687633416ea769e4b543561528`
- **Out of scope:** feature `ff13097e` (Phase 2B-2a Economy UI). It was already reviewed and APPROVED at `review/multi-2d-phase2b2a-economy-ui-20261126-v1` `bf93df60`.
- **File location:** this file sits next to `bf93df60`'s `phase2b-economy/PHASE2B2A-INDEPENDENT-REVIEW-20261126.md`. The packet suggested a `phase2b2a-economy-ui/` folder, but that folder does not exist, so the existing folder and the `PHASE2B2A-` prefix are used instead.

## Method and what was actually verified

- I read `git show bc2e8abc` in full: 4 files under `Tools/MultiRackSitkaP1Lab/build/`, +83/−54.
- **Merge containment:** `git diff aa958310 6a4a1a5a -- Tools/MultiRackSitkaP1Lab/build` is byte-identical to `git diff bc2e8abc^ bc2e8abc` (290 lines each, `cmp` equal). `bc2e8abc` touches only that directory. The merges added nothing else to the gate.
- I read the gate code at `6a4a1a5a` in a disposable detached worktree (since removed): `2d-gate.sh` G2 sections 8–10, `fail_stage`, the internal-mode call site, `module-graph.mjs`, `s0a-multi-asset-identity.json`, `s0c-hook-inventory.json`, `protected-paths.txt` and `selftest.sh`.
- I ran targeted probes only:
  - `gateEnforce` parse behaviour under `set -euo pipefail` with `local x; x="$(…)"`;
  - the legacy-tag regex against tag variants.
- I re-ran no suites. I relied on the supplied evidence:
  - `prep/gate-run` on `bc2e8abc`: VERDICT PASS exit 0;
  - `prep-r4` selftest on HEAD=`6a4a1a5a` BASE=`aa958310`: 29/29 PASS.
- I did not inspect stages G0/G1/G3–G7/B beyond confirming that this commit leaves them unchanged. Their PASS lines appear in the evidence (C26 and the prep gate-run).

## Q1 — What was removed or relaxed (complete list)

**Removed** from asset-identity mode `s0c-css-plus-html-delta`, now renamed `post-s0-live-serving`:

1. multi.css HEAD-blob SHA-256 had to equal T0 `R_css`, recomputed via `raw-literal.py --rev t0 --name Css`, and equal the pinned `css.rSha256`.
2. The recomputed T0 `R_html` had to equal the pinned `html.rSha256`.
3. T0 multi.html had to contain exactly one classic `<script src="multi.js" defer></script>`.
4. HEAD multi.html had to equal the T0 html with exactly that one line replaced by the module tag. This was the 1-line delta lock.
5. The `t0Commit` field was dropped from the `g2-s0a-asset-identity.json` evidence output. This is informational only.

**Relaxed:**

6. The live multi.css `content-length` header is now `len(HEAD css_blob)` (`2d-gate.sh:1112-1115`) instead of the pinned T0 value `57702`. This is the same derivation already used for HTML and modules. Body equality still applies, and every other header value (cache-control, connection, CSP, content-type, CORP, referrer-policy, nosniff, x-frame-options) is still compared verbatim against the pinned list. This relaxation follows unavoidably from removing item 1.
7. `js-reconstruct.mjs` (S0b-tip multi.js byte reconstruction) used to run whenever the file existed. It now runs only when `s0c-hook-inventory.json` `gateEnforce` is truthy, and that file sets it to `false` (`s0c-hook-inventory.json:6`; `2d-gate.sh:1200-1214`).

**Added / tightened:**

- multi.html must contain exactly one `<script type="module" src="multi/main.js"></script>` (`2d-gate.sh:1047`).
- multi.html must contain no legacy classic multi.js tag (`2d-gate.sh:1049`).
- If `js-reconstruct.mjs` exists but the inventory file is missing, G2 now fails.

**Conclusion:** Items 1–4 and 7 are exactly the stale T0 locks on multi.css, multi.html and js-reconstruct that the Owner authorised retiring. Items 5–6 follow directly from them. No other guard changed. The following are untouched by the diff:

- module-graph (14-module acyclic graph and layer order);
- G0–G7 and B;
- Core floor, Save, multiarea, ui-smoke 250/4 {L01,L02,L06,J31}, CLI;
- 9/9 anchors, v2 golden, 5/5 budgets, determinism grep;
- protected-path fence, wwwroot tree ID, Save Edge, Step 2 Edge;
- exit codes and case IDs.

Case IDs C21, C27 and C29 and their expected exit codes (1) are unchanged. Only their patterns and mutators changed.

## Q2 — Are the remaining asset checks real and fail-closed?

- **BOM/CR/no-trailing-LF:** still checked on the HEAD blobs of multi.css and multi.html, with `sys.exit` on violation. Real. For modules, `module-graph.mjs` checks BOM and CR; it does not enforce no-trailing-LF. That is unchanged.
- **Module tag:** an exact-bytes count must equal 1. Separately, `module-graph.mjs` (unconditional) requires exactly one `<script>` element, byte-equal to the module tag, with no inline body. Real.
- **Legacy-tag regex:** matches quoted `src="multi.js"` or `src='multi.js'` case-insensitively. It misses unquoted `src=multi.js` and `./multi.js` (probe). This does not matter in practice, because module-graph's exactly-one-script check would reject any second script, and live `multi.js` must return 404. See N2.
- **multi.js absence:** `git cat-file -e HEAD:…/multi.js` must fail. A live `GET multi.js` must return 404. `module-graph.mjs` also rejects the file and any `routes.Add("multi.js"`. Real (C28 exercises the HEAD check).
- **Live bodies vs HEAD blobs:**
  - `multi.css` and `multi`: status 200, body equal to the HEAD blob, headers equal to the expected list.
  - Every committed `.js` under `src/Ui/multi`: same checks, with headers from the JS template and content-length derived from the blob.
  - Zero committed modules fails the check.
  - Real and fail-closed. Each mismatch exits non-zero, `check_out` capture fails, and `fail_stage G2 1` is called (`2d-gate.sh:1166`).
- **Header/CSP comparison:** a sorted, case-lowered, Date-stripped list must equal exactly. Extra, missing or changed headers all fail. CSP is pinned verbatim.
- **gateEnforce parsing** (`2d-gate.sh:1204`), probed under `set -euo pipefail`:
  - Missing file: explicit `fail_stage G2 1`.
  - Invalid JSON or a non-object root: python exits 1, and the assignment line `js_recon_enforce="$(…)"` sits apart from `local`, so `set -e` aborts the internal worker. The worker is called plainly (`2d-gate.sh:1233-1234`), not in a conditional context. With no `GATE G2 PASS` marker, the outer marker check fails closed.
  - `"false"` as a string: truthy, so js-reconstruct runs and fails. This is the fail-closed direction.
  - Missing key, `null`, `0` or `[]`: falsy, so the check is skipped. This direction is lenient (see N1). It is the same pattern as the existing `s0a-move-table.json` move-proof switch (`2d-gate.sh:1176`), and any edit to the file would be a reviewed diff under `build/2d-gate/`.

## Q3 — Is the js-reconstruct skip honest?

Yes. When the switch is off, the gate logs `G2: S0c js-reconstruct.mjs retired/skipped (s0c-hook-inventory.json gateEnforce=false)` (`2d-gate.sh:1213`). It writes no PASS line and no reconstruct JSON. Both the prep gate-run and the C26 clean run show this exact line, and `g2-s0c-reconstruct.*` is absent from their evidence. `GATE G2 PASS` is then emitted on the strength of the other G2 checks. The skip line sits directly before that marker, so the log does not imply that reconstruction passed.

`module-graph.mjs` is still unconditional (`2d-gate.sh:1190-1197`, guarded only by the file existing, as before). Both runs show `MODULE-GRAPH PASS: 14 modules, acyclic, …`.

## Q4 — Do C21, C27 and C29 prove what they claim?

- **C21** (`selftest.sh:408-421`, `:541`): the mutator changes `MultiPageCss` in `MultiRoutes.cs` to `…MultiEmbeddedAsset(…).Concat(new byte[]{(byte)' '}).ToArray()`. `assert old in s` guarantees the edit applies. `System.Linq` is available because `ImplicitUsings` is enabled in `Tools/MultiRackSitkaP1Lab/Directory.Build.props:10`, and `MultiRoutes.cs:38-40` already uses `.Concat/.ToArray`. The committed multi.css is untouched, so the BOM/CR checks pass and execution reaches the live GET.
  - Evidence: G1 builds, G2 Core/Save/multiarea/ui-smoke/CLI/build-output/p0-isolation pass, then the asset check fails. `case-C21…stdout.log:21` contains `post-s0-live-serving: live multi.css body or headers mismatch`.
  - That pattern occurs nowhere else, so the case cannot pass vacuously. It genuinely proves that served bytes diverging from the HEAD blob are caught.
- **C27** (`selftest.sh:425-431`, `:548`): the mutator appends `void (() => import('./store.js'));`. After `stripCommentsAndStrings`, `import(` survives (only the specifier string is blanked), so `/\bimport\s*\(/` matches.
  - Evidence: the asset check passes (`post-s0-live-serving PASS … 14 ES modules`). The served main.js equals its HEAD blob because the tamper is committed and embedded. Then `MODULE-GRAPH FAIL: main.js: dynamic import() is forbidden` appears, followed by `ERROR [G2]: S0c module-graph.mjs failed`.
  - The ui-smoke run on the tampered main.js still passes, because the import is never invoked. Non-vacuous.
- **C29** (`selftest.sh:442-457`, `:550`): the mutator forces `gateEnforce=true` in the clone's inventory and also bumps one `originalLine`.
  - Evidence: module-graph PASS, then `G2: running S0c js-reconstruct.mjs...`, then `ERROR [G2]: S0c js-reconstruct.mjs failed`.
  - The description, the comment at `selftest.sh:440-441` and the retirement note all label this correctly as a wiring check, not tamper discrimination: HEAD already drifts from S0b-tip, so the case would fail even without the line bump. This follows the C22 precedent. It does prove that the `gateEnforce=true` path still runs the tool and maps failure to exit 1. It does not prove that js-reconstruct can still discriminate a tamper. That is accepted by the retirement intent and stated honestly.

## Q5 — Overclaims

None found that are material.

- The new PASS line says "T0 asset byte-identity lock retired" and lists exactly what is checked.
- The phrase "unchanged security headers/CSP" is accurate: every header value except the derived content-length is pinned.
- The retirement notes in both JSON files match the code.
- The commit message describes the change accurately.
- Minor wording: `retiredUtc` holds a date, not a timestamp (nit). `s0a-multi-asset-identity.json` still carries `t0Commit` and the T0 `rSha256` values, which `post-s0-live-serving` no longer uses. They are harmless historical context.

## Findings

| ID | Severity | Location | Finding |
|---|---|---|---|
| N1 | non-blocking | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh:1204` | `gateEnforce` uses Python truthiness with default `False`. A missing key, `null`, `0` or `[]` silently skips js-reconstruct, and non-bool values are not rejected. This matches the existing move-table switch (`:1176`), and the file is review-gated. Optional hardening: require the key and `isinstance(v, bool)`, or fail. |
| N2 | non-blocking | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh:1049` | The legacy-tag regex requires a quoted, exact `multi.js` and misses `src=multi.js` and `src="./multi.js"`. Fully covered in practice by module-graph's exactly-one-`<script>` rule and the live multi.js 404. |
| N3 | non-blocking (pre-existing) | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh:916,1166` | The asset sub-check reports its reason via `sys.exit(msg)`, which goes to stderr, while `check_out` captures only stdout. As a result `gate.log` shows `S0a /multi asset identity failed: ` with an empty reason, and the reason appears only on the gate's stderr/stdout log (as seen in C21). This structure predates the commit. |
| N4 | nit | `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-multi-asset-identity.json:6`, `s0c-hook-inventory.json:8` | `retiredUtc` holds a date-only value. |

Blocking findings: none.

INTEGRATION DECISION: APPROVED
