# CCF Multi 2D — Pre-2A (S0) Seam Extraction and 2D Integration Trunk — Requirements / Traceability Checklist

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1`
- **Feature ID:** `multi-2d-s0-seam-extraction-v1`
- **Spec status:** `APPROVED` — **OWNER APPROVED FOR SEQUENTIAL S0a → S0b → S0c EXECUTION** (2026-11-25; baseline `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1`; OQ-1..OQ-6 defaults + §4.6 Host-Compatibility Amendment Option A apply)
- **Classification:** `SPEC REQUIRED`
- **Checklist owner:** independent reviewer per packet (RG-S0a / RG-S0b / RG-S0c); integrator for closure
- **Implementation under test:** none yet (no branch, no HEAD, no evidence root exists)
- **Date:** 2026-11-25

**Authority.** This checklist traces `spec.md`. It is subordinate to the latest user instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md` and `Docs/Project/game-brief.md`. **Every row is deliberately unchecked.** A box may be ticked only by the independent reviewer (or by the integrator for closure rows), against evidence at the named location. The author never ticks one. HD = `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/`. GATE = `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh` and `Tools/MultiRackSitkaP1Lab/build/2d-gate/`. EXT = `/media/jer/Files/CCF/Verification/MultiRack-2D-S0/`.

## Approval preconditions

- [ ] Spec status is `APPROVED` by the owner before any task starts — evidence: spec header and owner approval record
- [ ] OQ-1 (asset directory) answered — evidence: spec §14 / approval record
- [ ] OQ-2 (gate location) answered — evidence: spec §14 / approval record
- [ ] OQ-3 (anchor and golden manifest) answered — evidence: spec §14 / approval record
- [ ] OQ-4 (evidence location) answered — evidence: spec §14 / approval record
- [ ] OQ-5 (safety-valve default) answered before S0b — evidence: spec §14 / approval record
- [ ] OQ-6 (RngSalts nested prefixes and substitution scope) answered before S0b — evidence: spec §14 / approval record
- [ ] Locked decisions A–D recorded in `Docs/Project/decision-log.md` by the Manager route (recommended) — evidence: decision-log entry
- [ ] Consolidated Phase 1 baseline `T0` verified, R3-approved and owner-accepted — evidence: owner acceptance record referenced in HD `S0A-TRUNK-CREATION-RECORD-<date>.md`
- [ ] The Manager has registered each packet's single writer in `Docs/Project/coordination/active-tasks.md` — evidence: live coordination file (not edited by any S0 branch)

## Acceptance criteria

- [ ] **S0-01** — the trunk was created exactly at `T0`; the first-parent history after `T0` is `--no-ff` merges or recorded reverts only; no trunk commit appears as a merge parent on `main` — evidence: HD `S0A-TRUNK-CREATION-RECORD-<date>.md`; `git ls-remote` / `git log --first-parent` output in the S0a handoff
- [ ] **S0-02** — `2d-gate.sh` runs G0 then G1–G7 in locked order with PASS markers, exits 0 only when all pass, and gives the specified codes 1/2/3/4/5/64 for planted violations, offline from a clean clone — evidence: GATE logs and the `selftest.sh` log summarised in HD `S0A-IMPLEMENTATION-HANDOFF-<date>.md`; raw logs in EXT
- [ ] **S0-03** — served `multi`, `multi.css` and `multi.js` SHA-256 = `R_x` (from the `T0` blob) = `P_x` (served at `T0`) = the committed file hash; headers identical except `Date` — evidence: GATE `s0a-multi-asset-identity.json`
- [ ] **S0-04** — G7 returns 4 for any path outside `Tools/MultiRackSitkaP1Lab/` and `Docs/` or on the deny-list; the cumulative S0 diff is clean; the `src/Ui/wwwroot` tree id equals `T0` — evidence: GATE logs per packet and at closure; self-test log
- [ ] **S0-05** — the route and continuation partial splits pass the line-multiset and block-order move proof with only the spec §5.2 exceptions; static fields stay in `MultiRoutes.cs` in order; no renames — evidence: GATE `s0a-move-table.json` plus the move-proof summary in the S0a handoff (full report in EXT)
- [ ] **S0-06** — `AnnualPipeline` order preserved with only M1/M2 (EXTRACTED), or `Advance` byte-identical to `T0` (RETAINED); a reviewer-confirmed decision record exists — evidence: HD `S0B-SAFETY-VALVE-DECISION-<date>.md`
- [ ] **S0-07** — `RngSalts.cs` centralises every RNG key prefix with byte-identical values; `RngSaltsTests` passes (frozen values, distinct, prefix-free except exactly the three nested `ADULT-MORT-*` pairs, collision freedom, new families strict) — evidence: Core test output summarised in the S0b handoff; G6 log
- [ ] **S0-08** — the Save v3 envelope contract (C-V3) is frozen in the approved spec; `IFeatureBlockCodec` has zero implementations; `SchemaVersion == 2`; `src/Core/Phase1/**` is unchanged; no v3 code — evidence: G4 log and grep results in the S0b handoff
- [ ] **S0-09** — native ES modules per spec §7; byte-exact reconstruction of the S0b-tip `multi.js`; acyclic local module graph; `svgRenderer` passes the contract; identical `document.body.outerHTML` at all scripted states (1280×720 and 1920×1080; MA-1 and MA-1-MIXED); loopback-only network — evidence: GATE `s0c-hook-inventory.json` and `s0c-renderer-callsite-map.json`; parity summary in HD `S0C-IMPLEMENTATION-HANDOFF-<date>.md`; screenshots and diffs in EXT
- [ ] **S0-REG** — zero behaviour change across S0: all 9/9 anchors equal spec §3.8; the v2 golden equals spec §3.7; all suites pass with counts ≥ baseline and no test weakened; payload budgets hold; the existing Edge suites pass; the protected listeners are unaffected — evidence: HD `S0-CLOSURE-<date>.md`; final GATE log (with `--browser`)

## Locked decision A — 2D integration trunk and gate

- [ ] A-1 trunk cut strictly from the accepted baseline `T0` — evidence: HD trunk creation record
- [ ] A-2 `main` stays 3D-only; no automatic merges to `main` — evidence: `git log origin/main --merges` excerpt in the closure handoff
- [ ] A-3 short-lived feature branches fork from the trunk with strict single-writer file sets — evidence: branch bases and the diff scope in each packet handoff
- [ ] A-4 one integrator rebases, gates, confirms independent non-author review, and merges `--no-ff` one branch at a time — evidence: the `INTEGRATION DECISION` block in each packet handoff
- [ ] A-5 freeze-on-red applied whenever the trunk gate is non-zero; repair or revert only — evidence: coordination record (Manager) and the integration decisions
- [ ] Gate stage G1 clean build 0 warnings / 0 errors (sln, UI csproj, Save tests csproj) — evidence: GATE log
- [ ] Gate stage G2 all Core / Save / headless-UI / safety suites, counts ≥ baseline — evidence: GATE log; `baseline-counts.json`
- [ ] Gate stage G3 9/9 frozen anchors via the manifest, including printed-only #7 and #9 — evidence: GATE log; `frozen-anchors.json`
- [ ] Gate stage G4 committed v2 golden pins and `src/Core/Phase1/**` frozen — evidence: GATE log
- [ ] Gate stage G5 payload budgets (view ≤ 524,288 B; ecology ≤ 153,600 B) — evidence: GATE log
- [ ] Gate stage G6 Core determinism grep (F1 zero; F2 only at the four allow-listed sites; no stray RNG key literal after S0b) — evidence: GATE log; `determinism-allowlist.txt`
- [ ] Gate stage G7 protected-path guard (allowed roots; explicit `Assets/`, `Packages/`, `ProjectSettings/` block; deny-list) — evidence: GATE log; `protected-paths.txt`
- [ ] Fail-closed semantics and exit codes 0/1/2/3/4/5/64 as specified — evidence: self-test log
- [ ] Runnable from a clean checkout with no network (isolation status recorded) — evidence: GATE log `NETWORK ISOLATION` line

## Locked decision B — S0a mechanical moves

- [ ] `/multi` HTML, CSS and JS lifted verbatim into a physically distinct asset tree (default `src/Ui/multi/`) — evidence: identity JSON; diff scope
- [ ] Served as `EmbeddedResource` only; no filesystem read; `644 B-01` guard still passes — evidence: GATE log (Core tests); csproj diff
- [ ] Loopback listener, capability URL, Host and Origin guards, CSP and headers, and the explicit route allow-list unchanged byte-for-byte — evidence: header comparison in the identity JSON; `ui-smoke-multiarea.sh` G-series log
- [ ] No second listener, no path traversal, no MIME guessing change — evidence: `Program.cs` and `PlayerApi.cs` untouched in the diff scope
- [ ] Extraction procedure (UTF-8 without BOM, LF only, no final newline, raw-literal dedent = 0) applied by script, not by hand — evidence: `raw-literal.py`; identity JSON
- [ ] Route handlers split into `MultiRoutes.cs` and `MultiApi.{View,Commands,Saves,History}.cs` inside the existing `internal sealed partial class PlayerApi` — evidence: move-proof summary
- [ ] Continuation commands split into `MultiAreaContinuation.Commands.<Subsystem>.cs`; the lock and the root commit stay in the root file — evidence: move-proof summary
- [ ] No logic edits, renames, statement reordering or whitespace churn beyond the move — evidence: move-proof summary

## Locked decision C — S0b engine seams

- [ ] `MultiAreaContinuation` remains the single lock and atomic root-commit authority; no module assigns live state — evidence: code review notes in RG-S0b
- [ ] `AnnualPipeline` / `IAnnualStep` / `YearInProgress` are fixed and compile-time ordered (AdultGrowth, Deadwood, Regeneration, YearRecord) with reserved empty Phase 3 disturbance and Phase 2B year-end summary slots — evidence: S0b handoff; decision record
- [ ] Safety-valve decision point DP-S0b executed and recorded as an explicit reviewed decision (no silent partial) — evidence: HD `S0B-SAFETY-VALVE-DECISION-<date>.md`
- [ ] `RngSalts.cs` centralisation and the automated uniqueness / prefix test — evidence: Core test output
- [ ] "Adding a subsystem" checklist recorded (root property, `HasUnexecutedWork`, options equality, save-codec registration, pipeline / Work-Plan slot) — evidence: spec §12 (approved text)
- [ ] `IFeatureBlockCodec` defined without any v3 implementation — evidence: grep results in the S0b handoff
- [ ] C-V3 frozen: schemaVersion 3; canonical envelope; ordinal-sorted typed blocks with `blockVersion`; fail-closed on unknown key or version; `genesisRecipe` = 2A, `economy` = 2B; v2 codec frozen; no silent migration; no in-place v2 rewrite — evidence: spec §6.6 (approved text)
- [ ] C-ECON frozen: Pending → Quote → Approve → Execute → Settle with an idempotent execution key; atomic with Execute; not inside annual biology — evidence: spec §6.6 (approved text)
- [ ] C-FENCE frozen: matching `sessionId` / `source` / `planStateRevision` stamps across split responses; never mix years — evidence: spec §6.6 (approved text)
- [ ] S0b merged with a green gate unlocks Phase 2B Economy engine coding (unlock recorded by the Manager) — evidence: S0b `INTEGRATION DECISION`; coordination record

## Locked decision D — S0c ES modules and map renderer plug

- [ ] Split into native `<script type="module">` files: `main.js`, `api.js`, `store.js`, `dom.js`, `map/contract.js`, `map/renderer-svg.js`, plus `modes/`, `panels/`, `dialogs/` — evidence: S0c diff scope; module graph output
- [ ] Zero npm, zero bundler, zero React/Vue/Blazor, zero forestry math in JS (D-020) — evidence: diff scope; no `package.json` added; RG-S0c notes
- [ ] `map/contract.js` defines mount / render / setSelection / focusArea / destroy with JSDoc; `renderer-svg.js` satisfies it — evidence: `ui-smoke-multiarea.sh` J log (`assertMapRenderer`)
- [ ] File-ownership map applied (integrator-owned `main.js`, `api.js`, `store.js`, `MultiRoutes.cs`, `AnnualPipeline.cs`, `RngSalts.cs`; lane-owned `Core/<Feature>/`, `panels/<feature>.js`, `dialogs/<feature>.js`) — evidence: spec §13; coordination record
- [ ] Headless Edge smoke at 1280×720 and 1920×1080 plus visual parity against the pre-split page — evidence: parity summary in the S0c handoff; EXT screenshots
- [ ] No module loaded from a remote origin — evidence: module-graph output; network logs
- [ ] Import graph proven acyclic without a bundler — evidence: `module-graph.mjs` output in the GATE log
- [ ] S0c merged with a green gate unlocks Phase 2A and Phase 2B UI coding (unlock recorded by the Manager) — evidence: S0c `INTEGRATION DECISION`; coordination record

## Hard exclusions (each must be shown absent)

- [ ] No behaviour change of any kind (gameplay, biology, constants, RNG, UI design) — evidence: S0-REG closure
- [ ] No Forest Maker, 0.64 ha presets or Save v3 codec (Phase 2A) — evidence: closure diff review
- [ ] No Economy or D-062 timber (Phase 2B) — evidence: closure diff review
- [ ] No Jay dispersal, storms or Species Wave 2 (Phase 3) — evidence: closure diff review
- [ ] No MapLibre or imports (Phase 4) — evidence: closure diff review
- [ ] No dynamic plugin loaders, DI frameworks or JS build pipelines — evidence: closure diff review
- [ ] Zero edits to Unity `Assets/` (including the linked `SimulationRandom.cs` and `SitkaGrowthModel.cs`) — evidence: G7 logs
- [ ] Zero edits to Compact30, Legacy40, Enlarged80, Reference Future v1 or Unity `ForestSaveData` — evidence: G7 logs
- [ ] Zero edits to the protected single-area `src/Ui/wwwroot/**` and legacy `Tools/MultiRackSitkaLab/**` — evidence: G7 logs; wwwroot tree id; P0 tree pin
- [ ] Zero edits to `Packages/`, `ProjectSettings/` and `main` — evidence: G7 logs; `origin/main` unchanged by S0
- [ ] Protected local listeners `:33767`, `:43649`, `:46593` never bound, killed or stopped — evidence: gate before/after listener record
- [ ] No S0 branch edits `Docs/Project/**` (including `coordination/active-tasks.md`) — evidence: G7 deny-list logs

## Review gates

- [ ] **RG-S0a** — independent non-author review of S0a on the exact gated HEAD — evidence: HD `S0A-INDEPENDENT-REVIEW-<date>.md`
- [ ] **RG-S0b** — independent non-author review of S0b, including confirmation of the safety-valve record — evidence: HD `S0B-INDEPENDENT-REVIEW-<date>.md`; HD `S0B-SAFETY-VALVE-DECISION-<date>.md`
- [ ] **RG-S0c** — independent non-author review of S0c, including parity and network evidence — evidence: HD `S0C-INDEPENDENT-REVIEW-<date>.md`
- [ ] Each packet merged alone with `--no-ff` after its review and a green gate on the rebased HEAD and the merge commit — evidence: the `INTEGRATION DECISION` blocks
- [ ] S0 closure record published after the final trunk gate (with `--browser`) — evidence: HD `S0-CLOSURE-<date>.md`

## Spec conformance

- [ ] Implementation matches the approved spec, plan and tasks — evidence: reviewer verdicts
- [ ] Every deviation is recorded in `spec.md` (amendment) and the packet handoff — evidence: spec history; handoffs
- [ ] Unresolved requirements are listed, not hidden — evidence: closure handoff

## Verdict

- **Spec conformance:** NOT VERIFIED (nothing implemented)
- **Implementation correctness:** NOT VERIFIED (nothing implemented)
- **Notes:** DRAFT quartet only. All rows above stay unchecked until independent evidence exists.
