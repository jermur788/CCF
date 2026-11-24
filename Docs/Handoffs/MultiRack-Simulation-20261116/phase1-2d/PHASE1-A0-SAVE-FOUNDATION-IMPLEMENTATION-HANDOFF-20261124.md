# 2D Phase 1 A0 — Save Foundation: Implementation Handoff

**Date:** 2026-11-24  
**Task:** P1-A0, isolated 2D save contract (spec `multi-2d-phase1-save-time-history-v1`, APPROVED 2026-11-24 13:53 UTC)  
**Implementing worker:** Claude Opus 5.5 (`claude-opus-5-5`), single writer, under the DeepSeek V4.1 Flash orchestrator  
**Branch / worktree:** `feat/multi-2d-phase1-a0-20261124-v1` in `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-A0-20261124`  
**Base (context) commit:** `cf6fe7a52aa096e824827c568f43fe05724dcce9` (Phase 2 baseline)  
**Code commit:** `3c52e2881dc32baecb47e40007196e6c380e9dbb` (all source and tests). This handoff is in the next commit on the same branch.

| Status | Value |
|---|---|
| IMPLEMENTED | Yes, for the A0 contract components listed below |
| TESTED | Contract-level only: 29/29 standalone tests pass, plus two mutation checks |
| REVIEWED | **No.** No independent review yet |
| INTEGRATED | **No.** Not joined with the map branch, engine, API or UI |
| VERIFIED (gameplay) | **No.** A real running forest cannot yet be saved or restored |
| ACCEPTED | **No** |

## 1. Implemented

All code is in new files under `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/`. The project's default compile glob picks them up. No `.csproj` change was needed; gate 1 below confirms this.

- **Versioned envelope.** Format marker `ccf-2d-multiarea-save` and integer schema version `1`. The header holds:
  - the save label;
  - the saved time in UTC;
  - a save-generation counter (int64);
  - the ecological year;
  - the model vector (adult / mortality / crown / regeneration / species);
  - the immutable genesis identity: experiment ID, manifest content hash, generator ID/version/seed, and each area's ID, simulation seed and exact area.

  The checksum is `sha256(header||payload)`, taken over the exact canonical bytes of both.
- **Complete mutable-root DTO** (`Phase1SaveState`). It mirrors `MultiAreaRoot` field for field, using the same names, order and numeric types. It covers:
  - the common year and all three revisions;
  - every area roster, including dead trees and their death records;
  - Crop marks;
  - the event ledger with per-area before/after and Fell records;
  - executed-key receipts;
  - pending Fell orders and the approved plan;
  - the full `AnnualHistory`, including natural deaths;
  - **Stage 4:** pruning history, pending/approved Prune orders, prune log, deadwood and `NextDeadwoodNumber`;
  - **Stage 5:** options, mast seed, every cell with its bands, pending/approved clears, clear log, annual accounts with promotions and per-cell establishment, and block credits;
  - **Phase 2:** options, both mast seeds, adult species identity, natural broadleaf bands, exact planted juveniles, shelters, spot patches, pending/approved polymorphic planting orders, work log, the three sequence numbers, and annual accounts (mast, natural, planted, density).
- **Not saved:** the `/multi` view JSON, canopy and ground caches, the transient session ID, and live fences. Source tokens appear only as *recorded provenance* inside the approved plan and the executed receipts (see boundary finding B1).
- **Lossless numbers:**
  - float32 uses the existing `{"k":"f32","b":"8 hex"}` convention.
  - Authoritative doubles use the same rule as `{"k":"f64","b":"16 hex"}`.
  - int64 values (generator seed, save generation, DBH threshold) are stored as `{"k":"i64","b":"16 hex"}`.
  - Ints are exact JSON integers, range-checked to int32.
  - NaN and Infinity are refused on encode and decode. Signed zero and subnormals keep their bits.
  - Free-text reasons are stored as `{"k":"utf8","b":"hex"}` of strict UTF-8, so quotes, backslashes, tabs, accents and emoji survive. A lone surrogate is refused.
- **Deterministic encoding** (`Phase1SaveCodec`):
  - fixed key order and no whitespace;
  - lists keep engine order, or ordinal order where the engine holds a dictionary (executed keys, pruning, species);
  - explicit type tags `plant` / `shelter` / `spotClear` for planting orders.
- **Strict decoding.** In order:
  1. size cap (64 MiB, checked before parsing);
  2. `StrictJson` (reused read-only), which refuses a BOM, duplicate keys, depth over 16, non-integer numbers, integers beyond ±(2^53−1) and trailing content;
  3. format and schema read first, so the refusal message is clear;
  4. exact key sets for every object;
  5. unknown enum and tag refusal;
  6. re-encode and require **byte equality** with the input (a single canonical form);
  7. checksum;
  8. validator;
  9. optional exact model-vector and genesis match.

  Model 0 and other older states are never upgraded. A mismatch gives `SaveModelMismatch` or `SaveGenesisMismatch` with an explanatory message.
- **Validator** (`Phase1SaveValidator`). Structural invariants read from the engine source at `cf6fe7a5`, each cited inline in the code. It contains no ecology formula and repairs nothing.
- **Slot store** (`Phase1SaveSlots`):
  - One dedicated directory. Slot names must match `^[a-z0-9][a-z0-9-]{0,47}$`, minus device names.
  - Every path is confined to the directory and checked.
  - Reads go through the existing `NoFollowRead.ReadExact`, which refuses a symlink in any path component, a non-regular file and a hard-linked file, and enforces the exact size.
  - A save is decoded before any byte is written.
  - Write sequence: an O_EXCL temp file (mode 0600) → fsync → read back and compare → refresh the prior-good backup → atomic `rename(2)` (direct libc, never `File.Move`'s copy fallback) → directory fsync.
  - The backup is refreshed only from a primary that still verifies, so a corrupt file never displaces a good backup.
  - `RecoverFromBackup`, `Inspect` and `RemoveLeftoverTemps` are provided.
  - "Written" and "durably committed" are reported separately: `DurablyCommitted` vs `ReplacedDurabilityUnconfirmed`.
  - Replacing an existing slot needs `replaceExisting: true`; otherwise `SlotExists`.
  - Writes are supported on Linux x64 only. Other platforms are refused (`PlatformUnverifiable`), with no fallback.
- **Internal A1 seam** (`Phase1RootMapping`, `internal`): pure field copies between the DTO and the root, `Capture(root, models, stage4Options)` and `Restore(state, vector)`. It is compile-verified only (see Not proven).

## 2. Tested (exact commands and observed output)

Environment: WSL Ubuntu, .NET SDK 8.0.134 (global.json pins 8.0.133 with latestPatch), run from `Tools/MultiRackSitkaP1Lab` with `PATH=/home/jer/.dotnet:$PATH`. HEAD at gate time was `cf6fe7a5` plus uncommitted new files only; those files were then committed unchanged as `3c52e288`.

**Gate 1** `dotnet build src/Core/SitkaP1.Core.csproj -v q --nologo` →
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
**Gate 2** `dotnet run --project tests/Phase1.Save.Tests -c Debug` →
```
Phase1 save tests — slot scratch: /tmp/ccf-phase1-save-tests.iUVVox
PASS roundtrip: Phase 2 with approved plan, every region, exact by reflection
PASS roundtrip: Phase 2 with every kind of pending work (Fell, Prune, Clear, Plant/Shelter/SpotClear)
PASS roundtrip: model 0/0/0/0/0 save keeps no Stage 4/5/Phase 2 parts
PASS determinism: re-encoding and rebuilding give byte-identical output
PASS precision: float/double bits, negative zero, subnormal, max, int64 above 2^53 and free text survive exactly
PASS refuse: other format marker or schema version
PASS refuse: model-vector mismatch both ways; model 0 is never upgraded
PASS refuse: unsupported or tampered model vector, and later-model state in a model 0 save
PASS refuse: genesis identity mismatch (content hash, area seed, experiment)
PASS refuse: duplicate JSON key
PASS refuse: unknown enum values (work action, planting order type, work-log kind)
PASS refuse: missing and unknown fields
PASS refuse: trailing bytes, whitespace, BOM, escapes and non-canonical spelling
PASS refuse: over-deep nesting
PASS refuse: oversized input before parsing
PASS refuse: non-finite float32/float64 on decode and encode; overflowing integer
PASS refuse: corrupt checksum and edited content
PASS refuse: inconsistent but well-formed saves (validator)
PASS fail-closed: a refused decode changes neither its input nor previously loaded state
PASS slots: illegal names refused (traversal, absolute, separators, dots, case, device, length, empty)
PASS slots: directory must be absolute, existing and not a symlink
PASS slots: save, list, load, explicit replace and prior-good backup
PASS slots: invalid bytes are never written
PASS slots: symlink escape refused for load, save and backup, outside file untouched
PASS slots: a fault at every write stage leaves the previous good slot and backup intact, no temp left
PASS slots: a temp file left by a killed process is ignored, reported and removable
PASS slots: missing primary → refused, then recovered from the prior-good backup
PASS slots: corrupt primary is refused, never promoted over a good backup, and recoverable
PASS slots: a directory or bad backup in place of a slot file is refused
29/29 passed, 0 failed
scratch removed: True
```
The slot tests use `Directory.CreateTempSubdirectory` under `/tmp`, outside the repository, and remove it.

**Gate 3** `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj -v q --nologo` → `Build succeeded. 0 Warning(s) 0 Error(s)`. The existing Core suite was **not run**, as the packet directs.

**Gate 4** `git status --porcelain=v1 --untracked-files=all` listed only the 11 new files below as `??`. `git diff --stat` was empty: no tracked file changed. `bin/` and `obj/` of the new test project are git-ignored and not committed.

**Mutation checks** (run in this worktree, files restored byte-for-byte, verified by SHA-256):
- M1: the reader was changed to read a tree's `zM` from `xM`. At least 8 tests failed, starting with the round-trip tests (the output was cut at 8 lines). The failing tests reported `SaveNotCanonical` or the wrong refusal.
- M2: the backup refresh was changed to skip verifying the current primary. The "corrupt primary never promoted over a good backup" test failed (28/29).

## 3. Not proven (A1 and later)

- **A real running forest cannot yet be saved or restored.** No engine command captures or restores the root. All tests use hand-built synthetic states; they are contract fixtures, not engine output and not ecologically plausible.
- `Phase1RootMapping` (root ↔ DTO) **compiles but is not runtime-tested**. The test assembly cannot reach internal engine types, and the packet forbids InternalsVisibleTo. A1 must test the full chain against a real `MultiAreaContinuation` run with planting, promotion, mortality, marks and work: capture → encode → decode → restore → structural equality, plus continued-`Advance` parity with an uninterrupted control (P1-01).
- Load policy A (approved plan → unapproved draft), new session fence, stale-token refusal, engine revalidation of orders, and history/skip controls are not implemented (P1-04 to P1-09).
- Durability is not tested against real power loss. "Disk full" and crashes are simulated with an injected `IOException` at each write stage. The `rename(2)` failure path and the `ReplacedDurabilityUnconfirmed` path are not exercised.
- No player-visible save/load exists. No HTTP or UI work was done.

## 4. Limitations

- The 64 MiB cap is an estimate. A 100-year Phase 2 save has not been measured. Stage 5 per-cell establishment arrays grow with years × cells.
- Identifier strings must be printable ASCII without `"` or `\` (the StrictJson rule). Engine IDs satisfy this, and anything else is refused at encode, never mangled. Only reasons use the UTF-8 hex text form.
- Only the uniform Stage 4 site-productivity field can be saved. `Stage4Options.SiteProductivity` is a code delegate, so capture refuses any other field.
- Slot writes and reads are Linux x64 only (same rule as `NoFollowRead`).
- Writes resolve paths by name inside the dedicated directory. A local process with write access to that directory could still race the symlink pre-check (TOCTOU) between check and rename. Reads are descriptor-anchored and safe. The directory must be dedicated and server-owned.
- The validator's invariants come from engine code at `cf6fe7a5`. Map Pass 1/2 changed `MultiAreaContracts.cs`, `MultiAreaContinuation.cs` and other files, and added `MultiAreaEcology.cs`. A1 must recheck the DTO and validator against the reviewed map candidate.

## 5. Boundary findings and interpretations (for the orchestrator)

- **B1 — Recorded source tokens.** "Do not save source tokens or fences" was read as *no live session fence*. The engine's executed-key replay compares `executed.Source == fence.Source`, and the approved plan carries `Source`. Both are saved as `recordedSource`: provenance only, never accepted as a fence. A load always creates a new session, so no old fence matches. The orchestrator or Manager should confirm this reading.
- **B2 — Old session ID inside recorded IDs.** Engine PlanIds have the form `{sessionId}:ma-plan:{n}`. Executed keys are minted from them. So recorded PlanIds and keys in the event ledger contain the *old* session ID as text. Nothing treats it as live state, but A1 must decide replay semantics for pre-load execution keys after a load.
- **B3 — Engine/algorithm version gate.** The model vector is the five specified switches. Token lineage (`RngModelVersion`, `GrowthModelVersion`, `P1AlgorithmVersion`, `AreaModelVersion`) is recorded but not part of the load expectation. If an engine change that keeps the same vector must refuse old saves, A1 should add an explicit engine-version field. That needs a schema decision, so it was not done here.
- **B4 — Encodings added beyond F32.** The `f64`, `i64` and `utf8` tags extend the existing F32 bit-string convention. Authoritative doubles (stand summaries, accounts, cell areas) and free text make them necessary.
- **B5 — UI text for B.** `MultiAreaLabels.MultiSessionOnly` ("Session only — nothing is saved") will become false once B ships saving. B must change it; A0 did not touch it.
- No forbidden edit was needed. No existing file was modified.

## 6. Exact file list

| File | Purpose |
|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs` | Format marker, schema version, limits, value tags, model vector, genesis identity, load expectation |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveState.cs` | Public complete-root DTO, field for field with the engine records |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveEnvelope.cs` | Header (label, UTC, generation, year, models, genesis), envelope, decoded result |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveCodec.cs` | Canonical writer and strict reader; checksum; model/genesis gate |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveValidator.cs` | In-memory structural consistency checks before activation |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveSlots.cs` | Named slots: confinement, no-follow reads, atomic durable replace, backup, recovery |
| `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1RootMapping.cs` | Internal A1 seam: root ↔ DTO field copies (compile-verified only) |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | Standalone opt-in test console (not in `.sln`, not registered in Core.Tests) |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs` | The 29 tests |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Samples.cs` | Synthetic representative states (Phase 2 approved and pending, model 0) |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/TestSupport.cs` | Runner, reflection deep-equality by bit pattern, mutate-and-reseal helpers |
| `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A0-SAVE-FOUNDATION-IMPLEMENTATION-HANDOFF-20261124.md` | This handoff |

## 7. Next

1. Independent review of `3c52e288`: correctness, missing root fields, validator invariants and slot durability.
2. A1, after the map-ownership gate. Rebase or cherry-pick onto the reviewed map candidate. Re-diff the DTO against the changed engine records. Wire `Capture` and `Restore` under the engine lock with a fresh session and load policy A. Then runtime-test the mapping and continuation parity.
