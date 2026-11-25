# CCF Multi 2D — Pre-2A (S0) Seam Extraction and 2D Integration Trunk — Bounded Tasks

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1`
- **Feature ID:** `multi-2d-s0-seam-extraction-v1`
- **Spec status:** `DRAFT` — **no task below may be dispatched or started until the spec is `APPROVED`**
- **Classification:** `SPEC REQUIRED`
- **Task list owner:** Spec author (draft); Overall Manager (dispatch); appointed 2D integrator (integration tasks)
- **Authoring base (context) commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc`
- **Last updated:** 2026-11-25

**Authority.** Tasks are implementation units derived from `spec.md` and `plan.md`. They create no product decision and remain subordinate to the latest user instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md` and `Docs/Project/game-brief.md`. If a task exposes a material ambiguity, the worker stops and returns to the Manager/spec.

**Conventions for every task.**

- **Working directory:** commands run from `Tools/MultiRackSitkaP1Lab/` of the packet's feature-branch worktree.
- **Scratch locations:** `TMPDIR` and every evidence directory must be outside the repository (`$EV` below).
- **Gate evidence:** `$EV` holds the raw gate evidence; durable raw evidence is copied to `/media/jer/Files/CCF/Verification/MultiRack-2D-S0/<packet>/<UTC stamp>/`.
- **Line numbers:** taken from `plan.md` they are informational; regenerate them at `T0`.
- **Writers and files:** single writer per packet; files outside the task's file set are forbidden.
- **Review:** no self-review, no self-approval, and no checklist box is ticked by the author.

---

## Pre-S0

### T-00 — Create `integration/2d-sim` at the accepted baseline

- **Task ID:** T-00 · **Status:** TODO · **Packet:** pre-S0
- **Owner role:** 2D integrator (coordination-capable route permitted to push branches)
- **Goal:** create the 2D trunk exactly at the owner-accepted consolidated Phase 1 baseline `T0`.
- **Exact file set:** none (ref creation only). Record: `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-TRUNK-CREATION-RECORD-<YYYYMMDD>.md`.
- **Exclusions:** no commit on the trunk; no touch to `main`; no `active-tasks.md` edit (that is the Manager's job).
- **Entry:** spec `APPROVED`; `T0` named in a written owner acceptance (Phase 1 Step 2 R3-approved and consolidated); Manager lane registration.
- **Verification:** `git fetch origin --prune && git cat-file -t <T0>` prints `commit`; `git push origin <T0>:refs/heads/integration/2d-sim`; `git ls-remote origin refs/heads/integration/2d-sim` prints `<T0>`.
- **Evidence:** the trunk creation record (T0 SHA, acceptance reference, `ls-remote` output, UTC time).
- **Exit / completion:** the remote trunk equals `T0`; the record is committed on the S0a branch (task T-A5).
- **Review gate:** checked under RG-S0a (S0-01).
- **Stop and ask if:** `T0` is not owner-accepted; there is more than one candidate baseline; `integration/2d-sim` already exists at a different SHA.

### T-00b — Capture the baseline at `T0` (read-only)

- **Task ID:** T-00b · **Status:** TODO · **Packet:** pre-S0 (feeds S0a)
- **Owner role:** S0a writer
- **Goal:** record the values the gate compares against.
- **Exact file set:** none committed in this task (values flow into `build/2d-gate/baseline-counts.json`, `frozen-anchors.json` and `s0a-multi-asset-identity.json` in T-A1/T-A2).
- **Entry:** T-00 done; a clean worktree at `T0`.
- **Verification commands (at `T0`):**
  ```bash
  build/build-p0-core-ref.sh
  dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers
  dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers
  dotnet build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --disable-build-servers
  dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll | tee "$EV/core.log"
  dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --no-build | tee "$EV/save.log"
  tests/ui-smoke.sh "$EV/ui-smoke" && tests/ui-smoke-multiarea.sh "$EV/ui-smoke-multiarea"
  tests/cli-safety.sh "$EV/cli" && tests/build-output-safety.sh "$EV/bos" && tests/p0-isolation.sh "$EV/p0iso"
  grep -E 'P2-03 M-08 Q6 hash|P2-10 MA-1-MIXED-v1 30-year fingerprint' "$EV/core.log"
  sha256sum .p0ref/SitkaLab.Core.dll
  ss -tln | grep -E ':(33767|43649|46593)\b' || true
  dotnet --version; node --version; python3 --version
  ```
  Plus the served `/multi` route bodies and headers (start the built UI, GET `multi`, `multi.css`, `multi.js` with valid Host and capability, record SHA-256 and headers except `Date`). Also check that `node --experimental-vm-modules -e "import('node:vm').then(v=>process.exit(typeof v.SourceTextModule==='function'?0:1))"` exits 0.
- **Evidence:** `$EV` (off-repo); the summary goes into the S0a handoff.
- **Exit / completion:** pass counts, summary-line formats, budget-check IDs, anchor lines (#7 and #9 equal to spec §3.8), `P_x` and tool versions are all recorded.
- **Stop and ask if:** any suite fails at `T0`; anchor #7 or #9 differs from spec §3.8; there is no Windows Edge or Windows node for the browser tier; `vm.SourceTextModule` is unavailable.

---

## Packet S0a — Mechanical moves + trunk gate

Branch `feat/multi-2d-s0a-mechanical-moves-<YYYYMMDD>-v1`, forked from the trunk tip (`T0`).

### T-A1 — Gate script, data and self-test (commit A-C1)

- **Task ID:** T-A1 · **Status:** TODO · **Owner role:** S0a writer
- **Goal:** a fail-closed `2d-gate.sh` running G0 then G1–G7 in the locked order with the exit codes of spec §4.3, plus `selftest.sh`.
- **Exact file set:** `build/2d-gate.sh`; `build/2d-gate/frozen-anchors.json`, `protected-paths.txt`, `determinism-allowlist.txt`, `baseline-counts.json`, `selftest.sh`.
- **Exclusions:** no edit to any existing test or build script; no network; the protected listeners are never touched.
- **Entry:** T-00b complete.
- **Verification:**
  ```bash
  bash -n build/2d-gate.sh
  build/2d-gate.sh --base origin/integration/2d-sim "$EV/gate"      # expect exit 0 on the unchanged tree
  build/2d-gate/selftest.sh "$EV/selftest"                           # expect each planted violation → its specified code
  ```
- **Evidence:** `$EV/gate/gate-summary.json`, `$EV/selftest/` (summarised in the S0a handoff).
- **Exit / completion:** gate exit 0 on clean `T0` content; the self-test shows codes 1, 2, 3, 4, 5 and 64 for their planted cases; the anchor manifest matches the test literals for #1–#6 and #8; `NETWORK ISOLATION` status recorded.
- **Review gate:** RG-S0a (S0-02, S0-04).
- **Stop and ask if:** a locked stage cannot be implemented without editing an existing test or script; the gate needs network; fail-closed behaviour cannot be demonstrated for a stage.

### T-A2 — Verbatim extraction of the `/multi` page (commit A-C2)

- **Task ID:** T-A2 · **Status:** TODO · **Owner role:** S0a writer
- **Goal:** move the three raw-string constants into `src/Ui/multi/` byte-for-byte, serve them as embedded resources, and prove triple hash equality (spec §5.3).
- **Exact file set:**
  - `src/Ui/multi/multi.html`, `multi.css`, `multi.js` and `.gitattributes`
  - `build/2d-gate/raw-literal.py`, `build/2d-gate/s0a-multi-asset-identity.json`
  - `src/Ui/SitkaP1.Ui.csproj` (new ItemGroup only)
  - `src/Ui/PlayerMultiApi.cs` (lines 53–55 initialiser swap, new `MultiEmbeddedAsset` helper, removal of `MultiAreaPage`)
- **Exclusions:** `src/Ui/wwwroot/**`, `PlayerApi.cs`, `Program.cs`; any change to the route table, MIME, headers or CSP.
- **Entry:** T-A1 committed.
- **Verification:**
  ```bash
  python3 build/2d-gate/raw-literal.py --rev <T0> --path Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs --name Html --out-sha256
  python3 build/2d-gate/raw-literal.py --rev <T0> --path … --name Css --out-sha256
  python3 build/2d-gate/raw-literal.py --rev <T0> --path … --name Js  --out-sha256
  for f in multi.html multi.css multi.js; do git show HEAD:Tools/MultiRackSitkaP1Lab/src/Ui/multi/$f | sha256sum; done
  LC_ALL=C grep -c $'\r' src/Ui/multi/multi.*        # expect 0
  for f in src/Ui/multi/multi.*; do tail -c1 "$f" | od -An -tx1; done   # last byte must NOT be 0a
  build/2d-gate.sh "$EV/gate"                          # G2 S0a sub-check: served S_x == R_x == P_x == committed
  ```
- **Evidence:** `build/2d-gate/s0a-multi-asset-identity.json` (committed); gate log.
- **Exit / completion:** S0-03 holds for all three routes; headers are identical except `Date`; the `644 B-01` Core test still passes (no file API added).
- **Review gate:** RG-S0a (S0-03).
- **Stop and ask if:** a closing delimiter has leading whitespace; any constant is interpolated or appears twice; `P_x ≠ R_x`; the BOM, CR or trailing-newline rules cannot all be met; embedding would require reading from disk.

### T-A3 — Split the `/multi` `PlayerApi` partial (commit A-C3)

- **Task ID:** T-A3 · **Status:** TODO · **Owner role:** S0a writer
- **Goal:** distribute the `PlayerApi` partial members into `MultiRoutes.cs` and `MultiApi.{View,Commands,Saves,History}.cs` per `plan.md` §2.1 with zero logic delta.
- **Exact file set:** `git mv src/Ui/PlayerMultiApi.cs src/Ui/MultiRoutes.cs`; new `src/Ui/MultiApi.View.cs`, `MultiApi.Commands.cs`, `MultiApi.Saves.cs`, `MultiApi.History.cs`; `build/2d-gate/move-proof.py`; `build/2d-gate/s0a-move-table.json`.
- **Exclusions:** `PlayerApi.cs`; renames; signature or statement changes; reformatting.
- **Entry:** T-A2 committed (the constants are already extracted).
- **Verification:**
  ```bash
  python3 build/2d-gate/move-proof.py --base <T0> --head HEAD --table build/2d-gate/s0a-move-table.json --part ui
  dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers     # 0 warnings / 0 errors
  build/2d-gate.sh "$EV/gate"
  ```
- **Evidence:** the move-proof report (summary in the handoff; full report off-repo); the gate log.
- **Exit / completion:** S0-05 (UI part): the line multiset and block order hold with only the spec §5.2 exceptions; all `/multi` static fields are in `MultiRoutes.cs` in original order; every member at `T0` is classified.
- **Review gate:** RG-S0a (S0-05).
- **Stop and ask if:** a member at `T0` is not in the table; a static field would have to move out of `MultiRoutes.cs`; a `using` change alters overload resolution (build warning or error).

### T-A4 — Split the continuation Work Plan commands (commit A-C4)

- **Task ID:** T-A4 · **Status:** TODO · **Owner role:** S0a writer
- **Goal:** move the command methods into `MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs` per `plan.md` §2.2.
- **Exact file set:** `src/Core/MultiArea/MultiAreaContinuation.cs` (removals by move only); the six new `MultiAreaContinuation.Commands.*.cs`; `build/2d-gate/s0a-move-table.json` (rows added).
- **Exclusions:** fields, constructors, `Advance`, Save/Load, reads and helpers stay; no biology file.
- **Entry:** T-A3 committed.
- **Verification:**
  ```bash
  python3 build/2d-gate/move-proof.py --base <T0> --head HEAD --table build/2d-gate/s0a-move-table.json --part core
  build/2d-gate.sh "$EV/gate"         # includes all 9 anchors, the v2 golden and the `644 B-01` guard
  ```
- **Evidence:** the move-proof report; the gate log.
- **Exit / completion:** S0-05 (Core part) holds; the gate exits 0.
- **Review gate:** RG-S0a (S0-05).
- **Stop and ask if:** any field or initialiser would move; any command depends on a moved/unmoved ordering of instance initialisers.

### T-A5 — S0a author verification and handoff

- **Task ID:** T-A5 · **Status:** TODO · **Owner role:** S0a writer
- **Goal:** full author run and a GitHub-tracked handoff.
- **Exact file set:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-IMPLEMENTATION-HANDOFF-<YYYYMMDD>.md`; the trunk creation record from T-00.
- **Verification:** `build/2d-gate.sh "$EV/gate"` (exit 0); `build/2d-gate/selftest.sh "$EV/selftest"`; `git diff --name-only origin/integration/2d-sim...HEAD` is limited to the T-A1–T-A5 file sets; `git status --short` is empty.
- **Exit / completion:** the handoff uses the AgentWorkflow `IMPLEMENTATION HANDOFF` block and states `NOT REVIEWED, NOT INTEGRATED`. No checklist box is ticked.
- **Stop and ask if:** any S0-REG item regresses.

### RG-S0a — Independent review of S0a

- **Owner role:** independent non-author reviewer (AgentWorkflow route: GPT-6.1 Sol, or an independent Opus 5.5 route). Never the author's thread and never a coordination-only route.
- **Scope:** S0-01 to S0-05 and S0-REG on the exact HEAD; the identity JSON; the move-proof report; the gate and self-test logs; the diff scope.
- **Output:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-INDEPENDENT-REVIEW-<YYYYMMDD>.md` with verdict APPROVED or NEEDS CHANGES.

### T-A6 — Integrate S0a

- **Task ID:** T-A6 · **Status:** TODO · **Owner role:** 2D integrator
- **Goal:** rebase, gate, `--no-ff` merge, post-merge gate and push, per `plan.md` §3.2.
- **Entry:** RG-S0a APPROVED on `H1`; trunk green.
- **Verification:** `git range-diff` shows only `=` entries (or the review is redone on `H2`); `build/2d-gate.sh` exits 0 on `H2` and on the merge commit `M`; `git push origin integration/2d-sim` is a fast-forward.
- **Evidence:** the `INTEGRATION DECISION` block appended to the S0a handoff.
- **Stop and ask if:** a non-trivial rebase conflict occurs; the post-merge gate is non-zero (do not push; return the branch).

---

## Packet S0b — Fixed engine seams + frozen contracts

Branch `feat/multi-2d-s0b-engine-seams-<YYYYMMDD>-v1`, forked from the trunk tip after T-A6.

### T-B1 — `RngSalts.cs`, literal substitution and uniqueness test

- **Task ID:** T-B1 · **Status:** TODO · **Owner role:** S0b writer
- **Goal:** centralise every RNG key prefix with byte-identical values and prove uniqueness and prefix rules (spec §6.4; `plan.md` §2.5).
- **Exact file set:**
  - `src/Core/RngSalts.cs`
  - the listed substitution lines only in `src/Core/AnnualRules.cs`, `src/Core/MultiArea/Phase2MixedDensity.cs`, `src/Core/MultiArea/Phase2Rules.cs` and `src/Core/MultiArea/Stage5Mast.cs` (if OQ-6 (b) = yes)
  - `tests/Core.Tests/RngSaltsTests.cs`
  - one registration line in `tests/Core.Tests/Program.cs`
  - the G6 registry pattern in `build/2d-gate.sh` / `build/2d-gate/determinism-allowlist.txt`
- **Exclusions:** no key value change; no other token on substituted lines; no other test edit.
- **Entry:** S0a merged and the trunk green; OQ-6 answered.
- **Verification:**
  ```bash
  git diff -U0 origin/integration/2d-sim -- src/Core/AnnualRules.cs src/Core/MultiArea/Phase2MixedDensity.cs \
    src/Core/MultiArea/Phase2Rules.cs src/Core/MultiArea/Stage5Mast.cs      # only the plan §2.5 lines
  build/2d-gate.sh "$EV/gate"     # Core tests incl. RngSaltsTests; G3 all 9 anchors; G6 no stray key literal
  ```
- **Evidence:** the Core log lines for `RngSaltsTests`; the gate log.
- **Exit / completion:** S0-07 holds; anchors #4, #5, #7 and #9 (RNG-dependent) are unchanged.
- **Review gate:** RG-S0b (S0-07).
- **Stop and ask if:** a prefix-freedom failure appears outside the three allow-listed `ADULT-MORT-*` pairs; a generated tree or area ID starts with `GENERIC-` or `QUOTA-`; any anchor moves.

### T-B2 — `IFeatureBlockCodec` interface (no implementation)

- **Task ID:** T-B2 · **Status:** TODO · **Owner role:** S0b writer
- **Exact file set:** `src/Core/Saves/IFeatureBlockCodec.cs`.
- **Exclusions:** `src/Core/Phase1/**`; any v3 encoder, decoder or registry implementation; any reference from v2 code.
- **Verification:** `git grep -nE ':\s*IFeatureBlockCodec\b' -- src` (no hits); `git grep -n 'SchemaVersion = 3' -- src` (no hits); `git diff --name-only origin/integration/2d-sim -- src/Core/Phase1` (empty); build 0/0.
- **Exit / completion:** S0-08 holds for the code part.
- **Stop and ask if:** the interface cannot compile against existing types only.

### T-B3 — `AnnualPipeline` extraction attempt and decision point DP-S0b

- **Task ID:** T-B3 · **Status:** TODO · **Owner role:** S0b writer
- **Goal:** route `Advance` through `AnnualPipeline.Steps` using only relocations M1 and M2, or RETAIN per the safety valve (spec §6.2–§6.3; default per OQ-5).
- **Exact file set:** `src/Core/MultiArea/AnnualPipeline.cs`; `src/Core/MultiArea/MultiAreaContinuation.cs` (`Advance` member only); `build/2d-gate/s0a-move-table.json` (Advance rows); `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-SAFETY-VALVE-DECISION-<YYYYMMDD>.md`.
- **Exclusions:** any biology or rules file; any relocation other than M1/M2; any test edit.
- **Entry:** T-B1 and T-B2 committed; OQ-5 answered.
- **Verification:**
  ```bash
  python3 build/2d-gate/move-proof.py --base <trunk-tip-before-S0b> --head HEAD --member MultiAreaContinuation.Advance --allow M1,M2
  build/2d-gate.sh "$EV/gate"     # G3 9/9, G4 golden, Core incl. P1-TH-02 skip-vs-manual parity
  ```
- **Evidence:** the safety-valve decision record (outcome `EXTRACTED` or `RETAINED (Tn)`, trigger evidence, reviewer confirmation slot).
- **Exit / completion:** S0-06 holds in one of its two forms; the record exists. There must be no silent partial.
- **Review gate:** RG-S0b confirms the record **before** merge.
- **Stop and ask if:** any trigger T1–T6 fires (→ RETAIN, record, continue only with the rest of S0b); a second extraction attempt seems needed (owner decision required).

### T-B4 — S0b author verification and handoff

- **Task ID:** T-B4 · **Status:** TODO · **Owner role:** S0b writer
- **Exact file set:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-IMPLEMENTATION-HANDOFF-<YYYYMMDD>.md`.
- **Verification:** full gate exit 0; the diff scope is limited to the T-B1–T-B4 sets; the handoff confirms the C-V3, C-ECON and C-FENCE text in the approved spec is unchanged.
- **Exit / completion:** the handoff is published, `NOT REVIEWED, NOT INTEGRATED`.

### RG-S0b — Independent review of S0b

- **Owner role:** independent non-author reviewer (as RG-S0a).
- **Scope:** S0-06, S0-07, S0-08 and S0-REG; the M1/M2 equivalence argument or RETAIN byte-identity; RngSalts values and test; zero codec implementations; contract text presence.
- **Output:** `S0B-INDEPENDENT-REVIEW-<YYYYMMDD>.md` plus reviewer confirmation inside the safety-valve decision record.

### T-B5 — Integrate S0b

- **Owner role:** 2D integrator. Same procedure as T-A6.
- **Completion note:** after a green post-merge gate, the Manager may admit **Phase 2B Economy engine** lanes under the Phase 2B spec (unlock only).

---

## Packet S0c — Native ES modules + map renderer plug

Branch `feat/multi-2d-s0c-es-modules-<YYYYMMDD>-v1`, forked from the trunk tip after T-B5.

### T-C1 — Parity driver and reference capture

- **Task ID:** T-C1 · **Status:** TODO · **Owner role:** S0c writer
- **Goal:** a headless-Edge parity driver for 1280×720 and 1920×1080 × MA-1 and MA-1-MIXED, with the scripted states of spec §7.5, run first against the **S0b trunk tip** as the reference.
- **Exact file set:** `tests/s0c-parity/run-parity-edge.sh`, `tests/s0c-parity/drive-parity.mjs`.
- **Exclusions:** every other test; any page file.
- **Verification:** `tests/s0c-parity/run-parity-edge.sh --mode reference "$EV/parity-ref"` (body outerHTML, screenshots and a network log per state).
- **Evidence:** the reference capture off-repo; its manifest of state hashes is summarised in the S0c handoff.
- **Stop and ask if:** Windows Edge or Windows node are unavailable (no substitute browser); any state is not deterministic across two reference runs.

### T-C2 — Inventory: mapping, layers, hooks, renderer call sites

- **Task ID:** T-C2 · **Status:** TODO · **Owner role:** S0c writer
- **Exact file set:** `build/2d-gate/s0c-hook-inventory.json`, `build/2d-gate/s0c-renderer-callsite-map.json`.
- **Goal:** confirm the `plan.md` §2.3 section→module mapping at the trunk tip; list every upward or cyclic call (function, call sites, from-layer → to-layer) and every call from outside `map/` into map functions with its contract method.
- **Exit / completion:** the inventories are complete and reviewed-ready.
- **Stop and ask if:** the inventory exceeds 20 hook functions; a module would import none of its natural dependencies (layering inverted); a section needs splitting beyond 17a/17b; a top-level `const` initialiser references another module's binding upward.

### T-C3 — Perform the split (P1–P6 only)

- **Task ID:** T-C3 · **Status:** TODO · **Owner role:** S0c writer
- **Exact file set:**
  - `src/Ui/multi/{main.js, api.js, store.js, dom.js, commands.js}`
  - `src/Ui/multi/map/{contract.js, renderer-svg.js}`
  - `src/Ui/multi/modes/interactive.js`
  - `src/Ui/multi/panels/{inspector.js, fell-draft.js, work-plan.js, time-history.js}`
  - `src/Ui/multi/dialogs/{saves.js, execute-confirm.js}`
  - `src/Ui/multi/multi.html` (script-tag line only)
  - `src/Ui/multi/multi.js` (removed)
  - `src/Ui/MultiRoutes.cs` (asset route lines only)
  - `src/Ui/SitkaP1.Ui.csproj` (EmbeddedResource lines only)
- **Exclusions:** `multi.css`; all `api/multi/*` handlers; Core; `wwwroot/**`; any re-indentation, renaming or logic change.
- **Verification:** build 0/0; `git diff origin/integration/2d-sim -- src/Ui/multi/multi.html` shows exactly one changed line (the script element); `git diff -- src/Ui/multi/multi.css` is empty.
- **Exit / completion:** every original section exists in exactly one module, in order, with original indentation.
- **Stop and ask if:** any transformation outside P1–P6 is needed; the CSP or any guard would need a change.

### T-C4 — Static proofs and headless harness update

- **Task ID:** T-C4 · **Status:** TODO · **Owner role:** S0c writer
- **Exact file set:** `build/2d-gate/module-graph.mjs`, `build/2d-gate/js-reconstruct.mjs`, the G2 wiring lines in `build/2d-gate.sh`, `build/2d-gate/s0a-multi-asset-identity.json` (`mode` → `s0c-css-plus-html-delta`), `tests/ui-smoke-multiarea.sh` (M-series module asset checks, per-module `node --check` via scratch `.mjs`, J-loader via `vm.SourceTextModule`, `assertMapRenderer(svgRenderer)`).
- **Verification:**
  ```bash
  node build/2d-gate/module-graph.mjs src/Ui/multi            # acyclic; local specifiers only; all reachable from main.js
  node build/2d-gate/js-reconstruct.mjs --modules src/Ui/multi --hooks build/2d-gate/s0c-hook-inventory.json \
       --callsites build/2d-gate/s0c-renderer-callsite-map.json --expect <S0b-trunk-tip>:Tools/MultiRackSitkaP1Lab/src/Ui/multi/multi.js
  tests/ui-smoke-multiarea.sh "$EV/ui-smoke-multiarea"
  ```
- **Exit / completion:** the reconstruction is byte-identical; the graph is acyclic; every M, G, W and J check passes, including the payload budgets.
- **Stop and ask if:** the reconstruction cannot be made exact without new transformation kinds; any J check would need its assertion changed (only the loader and asset-route checks may change).

### T-C5 — Browser tier, parity and handoff

- **Task ID:** T-C5 · **Status:** TODO · **Owner role:** S0c writer
- **Exact file set:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0C-IMPLEMENTATION-HANDOFF-<YYYYMMDD>.md`.
- **Verification:**
  ```bash
  build/2d-gate.sh --browser "$EV/gate"                                         # exit 0 incl. both existing Edge suites
  tests/s0c-parity/run-parity-edge.sh --mode compare --ref "$EV/parity-ref" "$EV/parity"
  ```
- **Exit / completion:** S0-09 and S0-REG hold: identical `document.body.outerHTML` at every state, both viewports and both experiments; screenshots identical or reviewer-accepted; only loopback requests in the network logs.
- **Stop and ask if:** any outerHTML difference appears; any non-loopback request appears.

### RG-S0c — Independent review of S0c

- **Owner role:** independent non-author reviewer (as RG-S0a).
- **Scope:** S0-09 and S0-REG; the reconstruction proof; the module graph; the hook and call-site inventories; the parity evidence; the network logs; the HTML one-line delta; the unchanged CSP.
- **Output:** `S0C-INDEPENDENT-REVIEW-<YYYYMMDD>.md`.

### T-C6 — Integrate S0c

- **Owner role:** 2D integrator. Same as T-A6, but the gate must run with `--browser`.
- **Completion note:** after a green post-merge gate, the Manager may admit **Phase 2A UI** and **Phase 2B UI** lanes (unlock only).

---

## Closure

### T-Z — S0 closure record

- **Task ID:** T-Z · **Status:** TODO · **Owner role:** 2D integrator
- **Exact file set:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0-CLOSURE-<YYYYMMDD>.md`.
- **Verification:** `build/2d-gate.sh --browser` on the final trunk tip (exit 0); cumulative `git diff --name-only <T0>..origin/integration/2d-sim` within the allowed roots; all 9 anchors and the v2 golden equal spec §3.7–§3.8.
- **Exit / completion:** S0-REG closure evidence is published; the spec status moves are proposed to the Manager (`VERIFYING` → `VERIFIED` only after independent verification).

---

## Task summary

| Task | Packet | Status | Owner role | Depends on |
|---|---|---|---|---|
| T-00 | pre-S0 | TODO | integrator | spec APPROVED; T0 accepted |
| T-00b | pre-S0 | TODO | S0a writer | T-00 |
| T-A1 | S0a | TODO | S0a writer | T-00b |
| T-A2 | S0a | TODO | S0a writer | T-A1 |
| T-A3 | S0a | TODO | S0a writer | T-A2 |
| T-A4 | S0a | TODO | S0a writer | T-A3 |
| T-A5 | S0a | TODO | S0a writer | T-A4 |
| RG-S0a | S0a | TODO | independent reviewer | T-A5 |
| T-A6 | S0a | TODO | integrator | RG-S0a |
| T-B1 | S0b | TODO | S0b writer | T-A6; OQ-6 |
| T-B2 | S0b | TODO | S0b writer | T-B1 |
| T-B3 | S0b | TODO | S0b writer | T-B2; OQ-5 |
| T-B4 | S0b | TODO | S0b writer | T-B3 |
| RG-S0b | S0b | TODO | independent reviewer | T-B4 |
| T-B5 | S0b | TODO | integrator | RG-S0b |
| T-C1 | S0c | TODO | S0c writer | T-B5 |
| T-C2 | S0c | TODO | S0c writer | T-C1 |
| T-C3 | S0c | TODO | S0c writer | T-C2 |
| T-C4 | S0c | TODO | S0c writer | T-C3 |
| T-C5 | S0c | TODO | S0c writer | T-C4 |
| RG-S0c | S0c | TODO | independent reviewer | T-C5 |
| T-C6 | S0c | TODO | integrator | RG-S0c |
| T-Z | closure | TODO | integrator | T-C6 |
