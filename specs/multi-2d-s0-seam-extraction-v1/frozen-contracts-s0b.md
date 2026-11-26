# CCF Multi 2D — S0b Frozen Cross-Lane Contracts & Integrator Subsystem Checklist

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (Adopted Status: `APPROVED`, 2026-11-25)
- **Feature ID:** `multi-2d-s0-seam-extraction-v1`
- **Packet:** S0b — Fixed Engine Seams & Frozen Contracts
- **Date:** 2026-11-25
- **Normative Authority:** `specs/multi-2d-s0-seam-extraction-v1/spec.md` — specifically **§6.5** (`IFeatureBlockCodec`), **§6.6** (`C-V3`, `C-ECON`, `C-FENCE`), **§12** ("Adding a subsystem" integrator checklist), and **§13** (Post-S0 ownership map).

> **NON-AMENDMENT NOTICE:** This document consolidates and **restates** the frozen S0b cross-lane contracts and integrator checklist from `spec.md` §6.5, §6.6, §12, and §13 as a single reference artifact for post-S0 parallel lanes (Phase 2A, Phase 2B, Phase 3, Phase 4). **It does NOT amend `spec.md`.** If any wording ever appears to differ, `spec.md` (subordinate to the user's latest direct instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md`, and `Docs/Project/game-brief.md`) governs.

---

## 1. Frozen Contract C-V3 — Save v3 Feature-Block Envelope (`spec.md` §6.5–§6.6)

*Implemented later by Phase 2A. S0b ships only the zero-implementation interface seam `Tools/MultiRackSitkaP1Lab/src/Core/Saves/IFeatureBlockCodec.cs`.*

- **`schemaVersion` = `3` (integer).** Strict canonical envelope preserving every v2 structural discipline: strict UTF-8, no BOM, duplicate keys refused, exact key sets, canonical re-encode byte equality, size cap, and checksum over all canonical bytes except the checksum object.
- **Top-level `blocks` object:** keys are **ordinal-sorted** and unique. Each block value contains **exact** keys `blockVersion` (integer `>= 1`) and `data`.
- **Fail-closed decoding (zero silent coercion):**
  - unknown block key → **refused**;
  - unknown or unsupported `blockVersion` (`CanRead(blockVersion) == false`) → **refused**;
  - missing block required by the session's model selection → **refused**;
  - any extra or missing envelope key → **refused**;
  - nothing is ever ignored, defaulted, or silently repaired.
- **Block ownership:**
  - `genesisRecipe` — owned by **Phase 2A**;
  - `economy` — **reserved for Phase 2B** (absent until Phase 2B ships).
- **Save v2 frozen read-only:** `Phase1SaveCodec` (`Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**`, `Phase1SaveFormat.SchemaVersion = 2`) stays **frozen read-only**. No v3 field leaks into v2, and `src/Core/Phase1/**` is never edited for v3.
- **No silent v2→v3 migration:** a v2 save file is either refused or converted only by an explicit, user-visible action that writes a **new** v3 slot or file. A v2 file is **never rewritten in place**.
- **Deferred to Phase 2A spec:** exact refusal codes, format marker string, and per-block payload schemas. All rules above are frozen now.
- **Interface seam shipped in S0b (`Tools/MultiRackSitkaP1Lab/src/Core/Saves/IFeatureBlockCodec.cs`):**
  - `string BlockKey { get; }`
  - `int CurrentBlockVersion { get; }`
  - `bool CanRead(int blockVersion);`
  - `string Encode(MultiAreaRoot root);`
  - `MultiAreaRoot Decode(string canonicalBlockJson, int blockVersion, MultiAreaRoot restoredSoFar);`
  - Pure .NET, aligned with `System.Text.Json`, zero UI/HTTP dependencies, **zero implementations** and **zero references from v2 code** in S0b.

---

## 2. Frozen Contract C-ECON — Phase 2B Atomic Work-Plan Economy Seam (`spec.md` §6.6)

*Implemented later by Phase 2B (engine coding unlocked after S0b merges with a green gate; persistence waits for Phase 2A C-V3).*

- **Primary attachment — atomic Work Plan execution:** `Pending → Quote → Approve → Execute → Settle`.
- **Quote:** a pure, read-only computation over the pending orders and the current committed `MultiAreaRoot`, bound to `PlanStateRevision`.
- **Approve:** freezes the quote into the approved plan.
- **Settle (atomic with Execute):** computed under the **same** `sync` lock acquisition as `Execute` and published in the **same single `root` assignment** — physical forestry work and economic settlement commit together or not at all.
- **Idempotency via existing execution key:** Settle is keyed by the existing engine-minted execution key (`planId + ":execute"`). Replaying `Execute` with the same key returns the stored receipt **including** its settlement and **never re-settles**. A key conflict is refused, as today. Retries can therefore never double-charge or double-pay.
- **Annual boundary:** Economy is **not** injected arbitrarily into annual biology. Its only annual attachment is the reserved read-only `ReservedPhase2BYearEndSummaryStep` slot in `AnnualPipeline.Steps` (`spec.md` §6.2).
- **Deferred to Phase 2B spec:** D-062 Oak/Beech/Sitka timber math, pricing tables, and ledger schemas.

---

## 3. Frozen Contract C-FENCE — Revision-Fenced Split API (`/stand` + `?lanes=`) (`spec.md` §6.6)

*Implemented later by Phase 2A / 2B / 4 UI and API lanes (including 0.64 ha / multi-lane stands).*

- **Uniform fence stamp:** every future composed read (e.g. `/stand` plus `?lanes=` lane-filtered views) and every per-lane or per-feature readout carries the **exact same `fence` object** as today's `/multi` responses:
  - `sessionId`
  - full `source` token (`domain`, `experimentId`, `manifestContentHash`, `areaIds`, `ecologicalYear`, `annualRevision`, `workRevision`, `rngModelVersion`, `growthModelVersion`, `p1AlgorithmVersion`, `areaModelVersion`, `actionEligibleDbhThresholdMm`)
  - `planStateRevision`
  *(matching `MultiFenceMembers` in `src/Ui/MultiRoutes.cs`).*
- **Single-lock response & atomic client composition:** each HTTP response is produced under **one** `sync` lock acquisition. The browser composes a frame **only** from responses whose fences are **byte-identical**. On any fence mismatch, the browser discards the whole frame and re-reads; it never mixes years, work revisions, or plan-state revisions across responses.
- **Server-side stale-fence refusal:** per-lane reads accept a fence and refuse stale fences with the existing typed refusal codes (following `MultiAreaContinuation.ReadCompositeUiFor(source, planStateRevision)`).

---

## 4. "Adding a Subsystem" Integrator Checklist (`spec.md` §12)

When a post-S0 lane adds an engine subsystem `<Feature>`, the parallel lane writes only its own files and the **integrator** verifies and applies every shared-file item:

1. **Root property:** append a nullable `<Feature>State? <Feature> = null` **last** to the positional `MultiAreaRoot` record (after `Phase2`). It must remain `null` on every root whose model selection lacks `<Feature>`, so existing roots and all 9/9 frozen anchors remain byte-identical with the feature off.
2. **`HasUnexecutedWork`:** if `<Feature>` has player orders, extend `MultiAreaRoot.HasUnexecutedWork` (`MultiAreaContinuation.cs`) with `|| (<Feature>?.Pending….Count ?? 0) > 0`. `Advance`'s consented cancellation must clear the feature's pending and approved orders, and `Execute` must revalidate them all-or-nothing.
3. **Options equality:** extend `RequireSameOptions` (`MultiAreaContinuation.cs`) with `Same<Feature>`, ensuring a save can never be continued under different feature options.
4. **Save codec registration:** Save v2 is frozen and never gains `<Feature>`. Register an `IFeatureBlockCodec` (ordinal-unique `BlockKey` + `CurrentBlockVersion`) in the v3 registry once Phase 2A implements C-V3. Until C-V3 is implemented, any feature carrying persistent state must make Save **refuse** with a typed refusal code — it must **never** silently drop state.
5. **Pipeline / Work-Plan slot:** annual effects attach to a named `IAnnualStep` slot in `AnnualPipeline.Steps` (either a reserved slot or a new slot added by reviewed spec amendment). Player actions become Work Plan order types flowing `Pending → Approve → Execute` (and `Quote → Settle` under C-ECON where economic) — never a direct live-state mutation.
6. **Determinism, budgets, fences and tests:**
   - register any new RNG key family in `src/Core/RngSalts.cs` (strictly prefix-free against all existing entries, zero allow-list additions);
   - preserve all 9/9 frozen SHA-256 anchors with `<Feature>` off;
   - keep view/readout DTOs within payload budgets (view `<= 524,288 B`, ecology `<= 153,600 B`);
   - stamp all UI readouts with C-FENCE (`sessionId`, `source`, `planStateRevision`);
   - add tests in the lane's own `tests/Core.Tests/<Feature>Tests.cs`, with the single registration line in `tests/Core.Tests/Program.cs` applied by the integrator.

---

## 5. Post-S0 Ownership Boundary (`spec.md` §13)

| Owner | Files (relative to `Tools/MultiRackSitkaP1Lab/` unless rooted at `Docs/` or `Assets/`) |
|---|---|
| **Integrator-owned (locked by decision D)** | `src/Ui/multi/main.js`, `src/Ui/multi/api.js`, `src/Ui/multi/store.js`, `src/Ui/MultiRoutes.cs`, `src/Core/MultiArea/AnnualPipeline.cs`, `src/Core/RngSalts.cs` |
| **Integrator-owned (shared infrastructure)** | `src/Ui/multi/{dom.js, commands.js, map/contract.js, multi.html, multi.css}`; `src/Ui/SitkaP1.Ui.csproj`; `src/Core/MultiArea/MultiAreaContinuation.cs` (single lock, fields, `Advance`, Save/Load); `src/Core/Saves/IFeatureBlockCodec.cs`; `build/2d-gate.sh` and `build/2d-gate/**`; registration lines in `tests/Core.Tests/Program.cs` |
| **Parallel-lane-owned** | `src/Core/<Feature>/**`; `src/Ui/multi/panels/<feature>.js`; `src/Ui/multi/dialogs/<feature>.js`; `src/Ui/multi/modes/<feature>.js`; `src/Ui/MultiApi.<Feature>.cs`; `src/Core/MultiArea/MultiAreaContinuation.Commands.<Feature>.cs`; `tests/Core.Tests/<Feature>Tests.cs` |
| **Phase 4 lane (later)** | `src/Ui/multi/map/renderer-maplibre.js` (behind `map/contract.js`) |
| **Forbidden on the 2D trunk** | `src/Ui/wwwroot/**`, `src/Core/Phase1/**`, `fixtures/**`, `Tools/MultiRackSitkaLab/**`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/**` |

---

## 6. Unlock Scope (`spec.md` §6.7)

Merging S0b to `integration/2d-sim` with independent non-author review (`RG-S0b`, including confirmation of `DP-S0b`) and a green gate unlocks **Phase 2B Economy engine coding** in a parallel lane under the Phase 2B spec. Phase 2B persistence remains gated on Phase 2A's C-V3 implementation (§4 item 4), and parallel Phase 2A / 2B UI coding remains gated on S0c merge.
