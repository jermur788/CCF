# 2D Phase 1 A0 — Save Foundation: Implementation Handoff

**Date:** 2026-11-24  
**Task:** P1-A0, isolated 2D save contract (spec `multi-2d-phase1-save-time-history-v1`, APPROVED 2026-11-24 13:53 UTC)  
**Implementing worker:** Claude Opus 5.5 (`claude-opus-5-5`), single writer, under the DeepSeek V4.1 Flash orchestrator  
**Branch / worktree:** `feat/multi-2d-phase1-a0-20261124-v1` in `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-A0-20261124`  
**Base (context) commit:** `cf6fe7a52aa096e824827c568f43fe05724dcce9` (Phase 2 baseline)  
**Code commit:** `3c52e2881dc32baecb47e40007196e6c380e9dbb` (all source and tests). This handoff is in the next commit on the same branch.  
**Repair R1 code commit:** `09a6ded6` (fixes for review verdict NEEDS CHANGES on `21df07c2`). See **§8 Repair R1**; where §1–§5 below differ from §8, §8 is current (schema version, header, approved plan, slot publish, validator).

| Status | Value |
|---|---|
| IMPLEMENTED | Yes, for the A0 contract components listed below, plus the R1 repairs in §8 |
| TESTED | Contract-level only: 44/44 standalone tests pass after R1 (was 29/29), plus R0 and R1 mutation checks |
| REVIEWED | R0 `21df07c2` reviewed independently: **NEEDS CHANGES** (5 blocking). R1 `09a6ded6` **not yet re-reviewed** |
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
- *(R1: B1 resolved by reviewer ruling — the approved plan now keeps orders only; see §8. B3 resolved by F5 — lineage is in the header and the expectation; see §8.)*
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

1. ~~Independent review of `3c52e288`~~ Done: NEEDS CHANGES. Now: bounded re-review of R1 `09a6ded6` against the five findings and the additional items (§8).
2. A1, after the map-ownership gate. Rebase or cherry-pick onto the reviewed map candidate. Re-diff the DTO against the changed engine records. Wire `Capture` and `Restore` under the engine lock with a fresh session and load policy A. Then runtime-test the mapping and continuation parity.

## 8. Repair R1 (code commit `09a6ded6`, on top of `21df07c2`)

**Scope.** Modifications to the existing A0 files only: the seven `src/Core/Phase1/*.cs` files and `tests/Phase1.Save.Tests/{Program,Samples,TestSupport}.cs`, plus this handoff. No new file, no existing non-A0 file, no `.csproj`/`.sln`/test registration, no `StrictJson.cs`, no `Assets/**`, no `Docs/Project`, no UI text. No package added.

### 8.1 Finding → resolution → evidence

| Item | Change | Result | Test (in `tests/Phase1.Save.Tests/Program.cs`) and observed evidence |
|---|---|---|---|
| **F1** slot names accept a trailing newline | The one `NamePattern` regex (used by `ValidateSlotName` and by `ListSlots`; both reviewer-cited sites at old lines 91/105 use it) is now `\A[a-z0-9][a-z0-9-]{0,47}\z`. | **PASS** | `F1 slot names: trailing, leading and embedded newlines…`: `alpha\n`, `\nalpha`, `al\npha` refused by `ValidateSlotName`, `Save`, `LoadBackup`, `Inspect`, `RecoverFromBackup` and `RemoveLeftoverTemps`; nothing written; a file named `beta\n.ccf2d.json` is not listed. `\r\n` and `\r` variants were added to the illegal-names test. **Mutation:** with the pattern reverted to `^…$`, this test and the illegal-names test FAIL (42/44). |
| **F2** invalid UTF-8 escapes as `InvalidOperationException` | `Decode` checks `System.Text.Unicode.Utf8.IsValid` before parsing and refuses with `SaveUtf8Invalid`; any `InvalidOperationException` from `StrictJson.Parse` is also translated into `SaveUtf8Invalid`. `StrictJson.cs` is untouched. | **PASS** | `F2 invalid UTF-8…`: high-bit byte in a string, truncated sequence in a key and an overlong encoding → `SaveUtf8Invalid` from `Decode` (with and without an expectation); `Save` of such bytes is refused and leaves no file or temp; a slot primary with bit rot → `Load` refused, `Inspect` reports `PrimaryRefusal = SaveUtf8Invalid` (no throw); `Save(replaceExisting: true)` over it **succeeds** (the bug was that it could not), `BackupUpdated = false`, the good backup is kept, and no temp file is left. **Mutation:** with the gate and translation removed, this test FAILS with the original untyped `Cannot transcode invalid UTF-8 JSON text to UTF-16 string` (43/44). *Note:* the brief says Save over a corrupt primary "must produce typed refusals"; the save itself correctly succeeds, and the corrupt file is reported, never promoted. |
| **F3** non-replace save is check-then-rename | `replaceExisting: false` now publishes with `renameat2(AT_FDCWD, temp, AT_FDCWD, slot, RENAME_NOREPLACE)`; `EEXIST` → `SlotExists`. If the filesystem or glibc lacks it (`EINVAL`/`ENOSYS`/missing symbol), `link(2)` (also atomic, also `EEXIST`) publishes the same inode and the temp name is unlinked. File fsync, read-back verification and directory fsync are unchanged. `replaceExisting: true` keeps `rename(2)` and the prior-good backup. | **PASS** (renameat2 path); link fallback **NOT TESTED** | `F3 no-clobber…`: the fault hook parks writer 1 at `BeforeReplace` (temp written, fsynced, verified); writer 2 creates the slot in that window; writer 1 then gets `SlotExists`; writer 2's bytes survive; no backup invented; writer 1's temp is removed; only the slot file remains and it loads through the single-link reader; a confirmed replace afterwards refreshes the backup. **Mutation:** with the publish reverted to `rename(2)`, this test FAILS (43/44). The link fallback cannot be forced on the test filesystem (`/tmp` supports `RENAME_NOREPLACE`). Fallback caveat: between `link` and `unlink` the slot briefly has two links, so a concurrent no-follow read may refuse `HardLinked` for that instant; it never sees wrong bytes. |
| **F4a** accounts not tied to the year | `Stage5.Accounts` and `Phase2.Accounts` must have exactly one account per committed year, `(i → i+1)` in order (`Stage5Annual.Step` / `Phase2Annual.Step` append one per Advance). | **PASS** | `F4a accounts…`: all Stage 5 accounts removed at year 3, all Phase 2 accounts removed, a missing year and a reordered list → each refused **by the account rule** (the test checks the refusal message names the intended rule). The Phase 2 fixture previously had only 2 accounts in year 3 (an engine-impossible state); it now has 3. |
| **F4b** mast seeds arbitrary | `Stage5.MastSeed == options.Stage5.MastSeed ?? unchecked((int)genesis.GeneratorSeed)`; `Phase2.OakMastSeed/BeechMastSeed == options.Phase2.X ?? Stage5.MastSeed` (constructor + `Phase2Annual.Genesis`). | **PASS** | `F4b mast seeds…`: Stage 5 seed 999, Oak 999 and Beech 42 → refused by the mast-seed rule; positive control: naming 999 in the options makes it legal and round-trips. Fixture now uses the derived path for Stage 5 and Beech and an explicit Oak seed. |
| **F4c** Phase 2 lattice not tied | Per area, Phase 2 cells must equal Stage 5 cells position by position (count, column, row), because `Phase2Annual` indexes by position (`RequireSameAreas`; `next2[c]` built from `cells[c]`). | **PASS** | `F4c lattice…`: swapped cells, a dropped cell and a changed column → refused by the lattice rule. |
| **F4d** `PlanStateRevision` only `>= 0` | A **lower bound** derived from every engine writer (comment in `RequirePlanStateRevision`): the engine only increments it, by 1, in add/remove/mark/unmark/approve/cancel/execute and in a cancelling or crop-losing Advance. Removes/unmarks make an upper bound impossible. Bound = 2 per event (approve + execute) + 1 per cancelled year + 1 if approved + 1 per crop mark + max(A, B) adds, where A counts one add per execution, cancelled year and approval plus every pending order, and B counts each executed Fell order, prune record, clear record, approved and pending Fell/Prune/Clear order, plus `NextOrderNumber − 1` planting orders. | **PASS** | `F4d plan-state revision…`: approved sample 24 → 1 and 24 → 19 refused, exactly 20 accepted; pending sample 23 → 18 refused, 19 accepted; model 0 with one Crop mark 1 → 0 refused. Fixtures moved to 24/23 (model 0 to 1) because the R0 values were below what their own content proves. |
| **F4e** deaths and fells untied | A tree death with cause `FellCause.Harvest` (`"fell"`) must have exactly one fell record; every fell record must have the engine's cause/mechanism and its tree must have died by Fell **in the event's year**; a tree is felled at most once; a natural death is never a Fell. | **PASS** | `F4e deaths and fells…`: a natural death relabelled as Fell (no record), a fell whose tree died in a different year, a fell whose tree died naturally, and a fell record with a different cause → each refused by its intended rule. The fixture's fake `"Harvest"/"Felled"` strings were replaced with the real `FellCause` constants. |
| **F4f** recorded provenance unchecked | Each executed receipt's token must name this experiment (domain `MultiAreaSitkaExperimentV1`, experiment ID, **manifest hash = genesis hash**, area list), be exactly the forest at execution (`year = annual revision = event year`, `work revision = event revision − 1`, because `Execute` requires `plan.Source == Token(root)`), and carry the header lineage; the receipt's orders must be exactly its event's fell records (area, tree, reason). Events may not go back in time. | **PASS** (receipts); approved-plan part **N/A by B1** | `F4f provenance…`: wrong hash, experiment, area list or domain; years/revisions out of range (0/0/0, 7/7/1, 2/2/0); receipt lineage differing; receipt orders for another tree, with another reason, or with an extra order → each refused by its intended rule. "An approved plan from year 0 is refused" no longer applies: under ruling B1 the approved plan carries no token at all. |
| **F5** no engine/algorithm lineage | New public `Phase1EngineLineage(RngModelVersion, GrowthModelVersion, P1AlgorithmVersion, AreaModelVersion, ActionEligibleDbhThresholdMm)` in the header **and** in `Phase1SaveExpectation`; compared exactly in `RequireExpected` → `SaveLineageMismatch` with an explanatory message. `Phase1EngineLineage.Current(models)` builds the running engine's lineage exactly as `MultiAreaContinuation` does (for A1 to pass as the expectation). **Schema version bumped 1 → 2**: the shape changed (header field added, approved plan reduced); no v1 save exists outside A0 test scratch, and the bump turns any stray A0-candidate file into a clear `SaveSchemaUnsupported` instead of a confusing `MissingKey`. | **PASS** | `F5 lineage…`: Phase 2/model 0/C2 area-model strings equal the engine's; each of the five fields changed in the expectation → `SaveLineageMismatch` (Phase 2 and model 0); tampering the header lineage of a save with receipts → refused (receipt lineage disagrees); a null lineage in the expectation → `SaveExpectationInvalid`. `refuse: other format marker or schema version` now refuses schema 1, 0 and 3. |
| **B1 ruling** | `Phase1ApprovedPlan` is now `(Orders)` only; the codec writes `"approved":{"orders":[…]}`. `Phase1RootMapping.Capture` drops PlanId/key/source; `Restore` never rebuilds an approval: it returns the root with `Approved = null` and empty approved Prune/Clear/planting lists, and hands the saved approved Fell/Prune/Clear/planting orders back as `Phase1SavedApprovalDraft` for A1 to rehydrate as an **unapproved pending draft**. Executed-key receipts keep their recorded token (accepted as inert provenance). | **PASS** (codec/validator); mapping compile-only | `B1 approved plan…`: DTO has only `Orders`; JSON has no minted plan ID or key; an injected `planId` key → `UnknownKey`; an approval with no order of any kind → refused (Approve refuses an empty plan). |
| Symlinked ancestors | Constructor walks every path component from the directory up to `/` and refuses any symbolic link. | **PASS** | `robustness: a symlinked ancestor…`: `scratch/linked-parent → outside/real-parent`, constructing on `linked-parent/slots` → `SlotDirectoryInvalid` naming the component; the real path is accepted. |
| Null `Models` → NRE | Validator null-checks header parts: `Models` → `SaveModelUnsupported`; `Lineage`, `Genesis`, null manifest hash → `SaveInconsistent`; null label/time → existing typed codes. | **PASS** | `robustness: null header parts…`. |
| `Inspect` on a directory | A directory at the primary/backup path is reported `Exists = true`, refusal `SlotNotAFile`. | **PASS** | `robustness: Inspect reports a directory…` (an absent slot is still "does not exist"). |
| Vacuous test | The R0 "previously loaded state untouched" assertion over a pure decode could not fail (`Decode` takes a `ReadOnlySpan` and returns new immutable records), so it was **replaced**. | **PASS** | `fail-closed: refused slot loads and inspections change no file on disk`: with a corrupt primary and a model-mismatched backup, refused `Load`, `LoadBackup` and `Inspect` leave every file's name and bytes identical (no repair, recovery or deletion). |
| De-alias fixtures | Stage 5 account ints now 2/3/4/5/6/7/8/9 (were 1/0/1/1/0/…/2/0), `meanBramble` ≠ `establishmentAccepted`, density hazard/boundary 1/2 and 8/9 (were 0/0), and other neighbouring equal values (planted account, block credits, coordinates, band years, growth phase factors) made distinct. | **PASS** | All round-trip tests pass by exact reflection; a swap between any two de-aliased neighbours would now fail them. |
| Golden checksum | Pinned SHA-256 of the canonical Phase 2 approved sample. | **PASS** | `golden: pinned SHA-256…`: file SHA-256 `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7`, embedded checksum `9629ac1648fabd85702fc4105075cebe9f69e884a816edcce4c441a3866c9086`, 24 703 bytes. |
| Mutation tests hit the intended rule | New `T.RefusesRule(fragment, …)` requires `SaveInconsistent` **and** a message naming the intended rule, so no F4 mutation passes because an unrelated rule refused it. | **PASS** | Used by every F4 test, the F5 tamper test and the B1/null tests. |

### 8.2 Gates (run on the committed tree `09a6ded6`, WSL Ubuntu, .NET SDK 8.0.134, from `Tools/MultiRackSitkaP1Lab`)

1. `dotnet build src/Core/SitkaP1.Core.csproj -v q --nologo` → `Build succeeded. 0 Warning(s) 0 Error(s)`.
2. `dotnet run --project tests/Phase1.Save.Tests -c Debug` → **`44/44 passed, 0 failed`**, `scratch removed: True`, exit 0. All 29 R0 tests still pass (one renamed: the replaced fail-closed test); 15 tests are new (F1, F2, F3, F4a–f, F5, B1, three robustness, golden).
3. `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj -v q --nologo` → `Build succeeded. 0 Warning(s) 0 Error(s)`. The long Core suite was **not run**, as directed.
4. `git diff --name-status 21df07c2..09a6ded6` → `M` only, on the 10 A0 files listed in §8 Scope (this handoff is changed in the following docs commit).
5. `git status --porcelain` → clean after commit.

**R1 mutation checks** (scratch script outside the worktree; each fix reverted alone, run, then restored; `git status` showed only the intended changes before commit): F1 regex → 2 FAIL; F2 gate removed → 1 FAIL with the original untyped exception; F3 publish reverted → 1 FAIL.

### 8.3 Refused, not done, or still not proven

- **Not done by instruction:** UI text (B5: `MultiSessionOnly`, `MultiChipSessionOnly`, `PlayerMultiApi`, `Ui/Program.cs`, README) — recorded only. A1 (capture under the engine lock, session fence, load policy, history, skip) — not implemented. The 64 MiB cap is unchanged (the measured 1.22 MB peak cited by the reviewer is far below it; no reason to lower it in a bounded repair).
- **Superseded by B1:** the F4f item "an approved plan from year 0 is refused" — the approved plan no longer has a recorded source to check.
- **NOT TESTED:** the `link(2)` + unlink fallback for filesystems without `RENAME_NOREPLACE`; the `ReplacedDurabilityUnconfirmed` path; real power loss.
- **NOT PROVEN (unchanged from §3):** no real running forest is saved or restored; `Phase1RootMapping` (including the new `Phase1SavedApprovalDraft` split) is compile-verified only; all fixtures are synthetic. The F4 invariants were read from engine source at `cf6fe7a5` and must be rechecked against the reviewed map candidate in A1 (especially the `PlanStateRevision` writer list and `FellCause` use).
- **Boundary findings:** none. Every fix fit inside the existing A0 files; no existing non-A0 file was edited.
