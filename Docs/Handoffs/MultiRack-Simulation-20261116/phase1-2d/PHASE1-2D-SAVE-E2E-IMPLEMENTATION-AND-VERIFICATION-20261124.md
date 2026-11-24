# CCF Multi 2D — Phase 1 Save/Load End-to-End: Implementation and Verification

**Date:** 2026-11-24 (desktop-local lane, `DESKTOP-DL1BQI7`, WSL2 Ubuntu).
**Directive:** [owner one-shot desktop directive](https://github.com/jermur788/CCF/blob/docs/multi-2d-save-e2e-one-shot-owner-directive-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-ONE-SHOT-DEEPSEEK-OWNER-DESKTOP-DIRECTIVE-20261124.md).
**Sole writer:** `DeepSeek V4.1 Flash orchestrator` (desktop-local implementation thread; commits authored under
`DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>`). **Independent reviewer:** separate read-only
thread with no implementation or supervision role — see §11.

| Label | State |
|---|---|
| IMPLEMENTED | **YES** — one cohesive change, Core → API → in-page UI |
| REVIEWED | **YES.** First review on `ca750ab` was NEEDS CHANGES (2 blocking); both were repaired and the repair re-review returned **APPROVED**. The reviewer's one required correction and its non-blocking items are dispositioned in §3.2/§11 |
| INTEGRATED | **NO** — task-branch join only; no merge to `main` (not authorised) |
| VERIFIED | **YES for the stated surface**: 264/264 Core, 46/46 save contract, 273/273 map smoke, 47/47 real Edge end-to-end |
| ACCEPTED | **NO** — owner playtest has not happened; reviewer verdict is not owner acceptance |

**Branch:** `feat/multi-2d-phase1-save-e2e-20261124-v1`
**Base (exact):** `4b6d8070fe709f8b6600baedd39bb37d0cf5322d` (owner-accepted Map Pass 2, unmerged)
**A0 source (transplanted):** `8db5e09538474cec01374066be08d73f240de82c`
**First implementation head (reviewed):** `ca750ab5787cad29300fd5e525583a8b00c34749`
**Reviewed repair head:** `803bdd834a0181b7754e411c34381c024fa26f60` (re-review verdict APPROVED)
**Final code head (this document's code head):** `a20640ae4fa5f025e68394bffa386236d02106e8` (post-approval corrections in §3.2)
**Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-SaveE2E-20261124`

---

## 1. What a player can now do

On the desktop's `/multi` map (both `ma-1` and `ma-1-mixed`), with the capability URL the process prints:

1. Play: select trees, queue Fell/Prune/ClearUndergrowth/Plant/Shelter/SpotClear, Approve, Execute, Advance a real year.
2. Open **Saved games** in the header, type a slot name, **Save current forest**.
3. Quit the server, start it again, open the same URL, open **Saved games**, choose **Load** on that slot and confirm.
4. The whole forest comes back at its saved year; the session has a new authority, so every earlier browser token is
   refused; a formerly approved but unexecuted plan is visible as an **unapproved pending draft**; the player continues
   with the same Advance/Work Plan controls.

Nothing else is added: no path entry, no upload/download, no cloud or remote copy, no save-on-quit, no resume-on-launch,
no merge to `main`.

## 2. What changed (exact file list, all inside the directive's §4 allowlist)

Added (A0 transplant, content-identical to `8db5e095…` except the NB1/NB2 repairs noted below):

```
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveFormat.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveState.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveEnvelope.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveCodec.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveValidator.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveSlots.cs
Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1RootMapping.cs
Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Phase1Save.Tests.csproj
Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Program.cs
Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/Samples.cs
Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/TestSupport.cs
```

Modified:

| File | Change |
|---|---|
| `src/Core/MultiArea/MultiAreaContinuation.cs` | `CaptureSave` (fenced, under the single lock), `SaveExpectation`, `Restore` (validate → build → revalidate restored orders → approval→draft → publish under a NEW session id, drop canopy/ground caches), root-aware prune helper and candidate-tree helpers |
| `src/Core/MultiArea/MultiAreaContracts.cs` | new `MultiAreaLoadResult`; truthful save/session copy (`MultiSessionOnly`, `MultiChipSessionOnly`, both "not modelled" notes) and the Save/Load UI labels |
| `src/Ui/PlayerApi.cs` | constructor takes the slot store |
| `src/Ui/PlayerMultiApi.cs` | four capability-protected routes; the Saved games dialog (HTML/CSS/JS) with slot list, honest commit state, overwrite tick, explicit replace-play confirmation, backup recovery, page reset after load |
| `src/Ui/Program.cs` | opens the server-managed slot folder (XDG data dir, mode 0700), truthful console lines, truthful shutdown line |
| `README.md` | narrow truthful Save/Load section and honest boundaries |
| `tests/Core.Tests/Phase1SaveIntegrationTests.cs` (new) | 9 engine-level integration cases |
| `tests/Core.Tests/Program.cs` | registration + the `644 B-01` guard narrowed to the one reviewed persistence seam |
| `tests/ui-smoke-multiarea.sh` | 33 Save/Load checks, hermetic `XDG_DATA_HOME`, truthful shutdown-line check |
| `tests/phase1-save-ui/drive.mjs`, `run-edge-e2e.sh` (new) | real Edge (CDP) driver and the three-process Save/restart/Load/continue runner |

`git diff --stat 4b6d8070 -- .` = 8 modified files + 4 new paths, 23 tracked files staged in total. **No file outside
`Tools/MultiRackSitkaP1Lab/` changed**; no `Assets/**`, `Packages/**`, `ProjectSettings/**`, P0 lab, `.p0ref` content,
frozen reference, Unity scene, PR or `main` change.

## 3. A0 defect disposition

**NB1 — initial-slot race (FIXED).** `Phase1SaveSlots.Save` now publishes with the atomic no-clobber rename/link whenever
the slot was **absent** when the mutation started, even when the caller passed `replaceExisting: true`; only a slot this
call saw as existing is replaced by `rename(2)`. Mutations of one slot name are serialized in-process by a per
(directory, slot) gate, and the folder is created only by the one reviewed `OpenOrCreate` factory. Proven by
`tests/Phase1.Save.Tests` "NB1a" (a competing writer inside the `BeforeReplace` window survives untouched under
`replaceExisting:true`) and "NB1b" (two concurrent saves of one slot: both succeed, the slot is one complete save and the
other is its verified backup, no temp left). The A0 `F3` no-clobber case is unchanged and still passes.

**NB2 — illegal restored orders (FIXED, fail-closed).** `Phase1SaveValidator` now refuses, before any engine call: a Fell
order naming a retained Crop Tree, one tree carrying both a Fell and a Prune (or duplicate Fell) across pending and
approved work, one ground cell cleared twice, and two planting orders of the same kind at the same exact point. Beyond the
DTO rules, the engine `Restore` revalidates **every order the restored Work Plan will actually hold** with its own guard
(`RequireLivingInArea`, `RequirePrunable` at the restored year, `RequireClearCell`, `Phase2Planting.RequireValidBatch`
against the live polygon lattice). A saved approval is merged into the pending draft **before** that re-check runs, so
orders that were approved in the file are covered too (§3.1 repair R1). **Any** illegality fails the entire load and
publishes nothing — nothing is repaired, dropped or partly applied. Proven by `P1-SL-05a/b/c` (validator, exact rule named
in the message), `P1-SL-07` (a structurally valid save whose planting point only the engine can judge),
`P1-SL-07b` (the same illegal Plant **inside the approved plan**) and `P1-SL-07c` (an approved Prune whose target is not the
engine's scheduled lift) — each refused with the live session byte-identical afterwards.

## 3.1 Independent review findings and their repair

The first independent review (Opus 5.5, separate read-only thread) returned **NEEDS CHANGES** with two blocking defects
and eight non-blocking observations. Full text in §11. Both blockers were repaired in `803bdd83` inside this lane:

**R1 — approved orders skipped the engine re-check (blocking, fixed).** `Phase1RootMapping.Restore` moves a saved
approval's orders into `SavedApproval` and empties the approved lists, so the old order of operations validated a root
whose approved lists were already empty and then merged those orders back as pending **unchecked**. `Restore` now builds
the candidate with `WithSavedApprovalAsDraft` first and validates that candidate, so an engine-illegal approved Plant or
Prune fails the whole load exactly like a pending one. New tests `P1-SL-07b` and `P1-SL-07c` use the reviewer's own
out-of-polygon Plant case and an invalid approved Prune target, and assert the session is byte-identical afterwards.

**R2 — one-click backup recovery could destroy a valid newer save (blocking, fixed).** `RecoverFromBackup` used to rename
the backup over any primary, and the page offered it next to **Load** on every slot with a backup. It now verifies the
backup, then refuses with `SlotPrimaryValid` while the primary still decodes — recovery is a last resort for a damaged
slot, never a way to roll a good save back. The listing exposes a `recoverable` flag (only when the primary does not
verify and the backup does), the page shows **Restore backup** only for such a slot and asks for an explicit confirmation
before sending it. New: save-suite `R2`, three smoke checks (a valid slot is not recoverable, recovery over it is refused
with the primary unchanged, a corrupt slot is recoverable and recovers) and two browser assertions.

**Also repaired from the review (non-blocking):**
- The saved deterministic options are now compared with the running session's (Stage 4 growth phases, Stage 5
  grid/browse/mast seed, Phase 2 seeds and genesis species) and a difference refuses with `SaveOptionsMismatch`; the Stage 4
  phases used to be silently taken from the session while the other options came from the file. `P1-SL-09`.
- `SaveGeneration` is read under the slot's own gate (`Phase1SaveSlots.NextGeneration`), so two concurrent saves of one
  slot cannot claim the same generation.
- `BackupLoadable` no longer reads `true` for a slot with no backup (the UI never used it alone, but the field was
  misleading).
- The browser acceptance now also proves the keyboard path (Enter on the header button, Tab to Save, Enter to commit), the
  explicit overwrite tick and its refusal without the tick, the approved-in-file → unapproved-draft policy with an Approve
  control and no approved plan, the absent **Restore backup** control on a valid slot, and a stale-fence **load** refusal
  returning the unchanged session. The placeholder `check(true, …)` the reviewer flagged is now a real assertion.

**Not changed, disclosed instead:** the save-folder dependency at startup and the in-process-only slot gate (§8); the
`644 B-01` test-pin edit still needs Manager sign-off (§7).

## 3.2 Re-review and post-approval corrections

The narrow re-review of the repair (fresh independent thread, same model) returned **APPROVED**: it re-ran the F1 repro
(`REFUSED Phase2PointOutsideArea`, session unchanged), probed every F2 recovery combination (valid/corrupt/missing primary,
good/bad backup, directory and symlink at the path) against the required outcomes, confirmed the new tests fail if their
fix is reverted, and confirmed no change outside the save/load paths. **No new blocking defect.**

It also found one **false claim** and five non-blocking items. Disposition:

| Item | Disposition |
|---|---|
| R3. `SaveGeneration` uniqueness was claimed but not guaranteed (`NextGeneration` released the slot gate before `Save` took it, with the engine capture in between) | **Fixed properly** rather than re-worded: `Phase1SaveSlots.LockSlot` holds the per-slot gate across the generation read, the engine capture and the write (the gate is re-entrant, and no path takes the engine lock and then a slot gate, so there is no lock cycle). The save handler now uses that scope. New test `NB1c` proves two scoped saves of one slot record consecutive generations. |
| R4. Missing-primary recovery used the clobbering rename, unlike NB1's no-clobber rule | **Fixed:** recovery publishes no-clobber when the primary did not exist, so a slot another process created in that window survives (`SlotExists`). |
| R5. Closing the dialog hid the load confirmation but not the recovery one, and the two confirmations did not hide each other | **Fixed:** closing hides both, and opening one hides the other. |
| R6. The browser covered recovery only by the absence of the control | **Fixed:** a fourth browser phase truncates the slot's primary, then drives **Restore backup** through its confirmation and loads the recovered save (8 new assertions). |
| R7. `P1-SL-07c` accepted any refusal code; `P1-SL-09` covered only the Stage 5 branch | **Fixed:** 07c pins `PruneBroadleafTargetInvalid` with an in-range but unscheduled 4.0 m target on an Oak; 09 also covers the Stage 4 growth-phase branch. |
| R2b. The slot list marks a slot "valid" without the options check, so a save with different options would offer **Load** and then be refused | **Deferred and disclosed** (§8): it needs a server run with different options for the same experiment, which the supported launch cannot produce. |
| R2c. Two server processes sharing one folder | Already disclosed (§8); unchanged. |

## 4. Load policy and the safety properties it rests on

- **Capture under one lock.** `CaptureSave` takes the engine's single lock, checks the caller's full fence, and copies the
  whole committed root: both rosters (dead stems included), Crop marks, year and both revisions, plan-state revision, event
  ledger, executed-key receipts, pending plan, Stage 4 pruning/deadwood, Stage 5 cells/bands/accounts and Phase 2
  species/juveniles/shelters/patches/accounts. A stale fence captures nothing.
- **Identity is exact, never migrated.** Load requires the same model vector, genesis identity (experiment id, manifest
  content hash, generator id/version/seed, every area id, seed and area), engine lineage (RNG, growth, P1 algorithm, area
  model, 230 mm threshold) **and the same deterministic options** (Stage 4 growth phases, Stage 5 grid/browse/mast seed,
  Phase 2 seeds and genesis species). Different forest/model/engine/options → `SaveGenesisMismatch` /
  `SaveModelMismatch` / `SaveLineageMismatch` / `SaveOptionsMismatch`, and the running session is untouched.
- **Build first, publish once.** `Restore` validates, rebuilds the entire candidate root, revalidates restored orders and
  converts any saved approval to a pending draft **before** one assignment publishes it. It then mints a **new 128-bit
  random session id** (never derived from the file) and drops the canopy/ground read-out caches, so neither a stale fence
  nor a stale cached map can survive the load.
- **Approval is not restored.** The saved approval's orders (`PlanId`, execution key and approval token are deliberately
  never written) return as ordinary pending orders; `PlanStateRevision` is restored exactly, so the player must Approve
  again under the new session. Executed receipts remain as recorded history, and an old `(planId, executionKey)` retry is
  refused as `WorkExecutionKeyConflict` instead of felling twice (`P1-SL-02`, `P1-SL-03`).
- **Slot store.** Strict lowercase names, one dedicated folder (symlink-refused in every component), no-follow reads with a
  single-link regular-file requirement and a 64 MiB cap, validate-before-write, temp + fsync + read-back compare, prior-good
  backup only from a still-verifying primary, one atomic rename, directory fsync. Recovery of that backup is refused
  while the primary still verifies (`SlotPrimaryValid`). `DurablyCommitted` vs `ReplacedDurabilityUnconfirmed` is reported
  to the player, never flattened into "saved".
- **API.** `GET api/multi/saves`, `POST api/multi/save`, `POST api/multi/load`, `POST api/multi/saves/recover` — all behind
  the unchanged guards (Host, capability, route/method, Origin, content type, 16 KiB body, exact member names). The list
  exposes no filesystem path; a save requires the explicit `replaceExisting` member; a load requires the current fence.

## 5. Exact verification (this head, this host)

Environment: `DESKTOP-DL1BQI7`, WSL2 Ubuntu, `~/.dotnet` **SDK 8.0.134** (system default 10.0.115 cannot satisfy the
`global.json` 8.0.133 pin), node v26.13.1, Microsoft Edge (Windows, headless CDP).

| Gate | Command | Result |
|---|---|---|
| Solution build | `dotnet build MultiRackSitkaP1Lab.sln` | **0 warnings, 0 errors** |
| UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj` | **0 warnings, 0 errors** |
| Core suite incl. nine anchors | `tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests` | **267/267 passed** (255 baseline + 12 new `P1-SL` cases) |
| Save contract suite | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **48/48 passed** (44 A0 + 2 NB1 + 1 recovery guard + 1 scoped generation) |
| Two-area map smoke | `tests/ui-smoke-multiarea.sh` | **276 passed, 0 failed** (240 baseline + 36 Save/Load) |
| Single-area smoke | `tests/ui-smoke.sh` | **250 passed, 4 failed** — `L01`, `L02`, `L06`, `J31`; the **same four IDs fail on the unmodified base `4b6d8070`** (re-run in the Pass 2 worktree), so this is pre-existing drift, not a regression |
| Real browser end-to-end | `tests/phase1-save-ui/run-edge-e2e.sh` | **63 passed, 0 failed** across four server processes |
| CLI safety | `tests/cli-safety.sh` | **PASS** (P0 tracked digest unchanged) |
| Builder/output safety | `tests/build-output-safety.sh` | C1–C6 **PASS**, P0 digest **PASS**; **C7/C8 FAIL — pre-existing, environment-limited** (see §9); the run leaves a rebuilt `.p0ref`, which was restored to the pinned binary afterwards |
| P0 isolation (`p0-isolation.sh`) | not run | **NOT RUN** (§9) |

The nine protected anchors are asserted inside the Core suite and all pass: model-0 single-area `7e3a2a86…`, model-0
two-area `a692a6d5…`, Crown C2 `814e8165…`, Stage 4 `8a926776…`, Stage 5 `1/1/1/1` `c4b670ed…`, Stage 5 5.0 m purity
`c67edc6b…`, Phase 2 `1/1/1/1/0`/`1/1/1/1/1` `c4b670ed…`, Q6 `776a68df…` (identical for `1/1/1/1/0` and `1/1/1/1/1`), MA-1-MIXED
30-year `93e9abda…`, plus the P0 Core pin `9a99f781…` and the P0 tracked-file digest `9e218ca7…`.

### 5.1 What the new tests actually prove

`P1-SL-01` — a genuinely played MA-1-MIXED forest (executed Fell + planted Oak juvenile + shelter, one real year, then a
pending Fell and a scheduled Prune) is captured, encoded, decoded, restored into a fresh session, and compared with an
uninterrupted control **byte for byte through the canonical save encoding** (only the session-minted plan ids are
normalised, because they are authority, not forest state). Both forests then advance two more real years with identical
results at each step. `P1-SL-02` — fresh session id, restored year/revisions/ledger, old fence refused for Advance and
Work Plan edits, old execution key refused as a replay. `P1-SL-03` — approved-in-file comes back pending, same
`PlanStateRevision`, re-approval mints a new plan id and executes. `P1-SL-05a/b/c` — NB2 rules. `P1-SL-06/07/08` — a foreign
identity, an engine-illegal restored order and a stale fence each leave the session byte-identical and still commandable.
`P1-SL-07b/07c` — the same fail-closed guarantee for an order that was **approved** in the file (both review-blocking cases).
`P1-SL-09` — a save whose deterministic options differ is refused with `SaveOptionsMismatch`.

## 6. Real browser acceptance evidence (Save → Quit → Restart → Load → Continue)

Runner: `tests/phase1-save-ui/run-edge-e2e.sh <evidence-dir>` (three processes, real SIGINT shutdown, no stubs).

Evidence directory (outside the repository): `/mnt/c/Users/hp/AppData/Local/Temp/ccf-save-e2e-repair3`
(`C:\Users\hp\AppData\Local\Temp\ccf-save-e2e-repair3`), **63/63 assertions** across four server processes,
`run.log` sha256 `1af8dbaf4ef3…`. Earlier runs are kept at `…/ccf-save-e2e-run3` (first head) and the repair runs before
the post-approval corrections.

| Evidence | sha256 (prefix) | What it shows |
|---|---|---|
| `saves-empty-1280-1280x720.png` | `fabb13981c52…` | the dialog at 1280×720: fits with no horizontal scroll (dialog 832 px, table 792 px), Save control inside the viewport |
| `saves-saved-1280-1280x720.png` | `ab3307461016…` | slot `edge-demo`, year 1, UTC time, "Ready to load", Load button |
| `saves-saved-1920-1920x1080.png` | `cbd354f8f399…` | the same dialog at 1920×1080 (832×473 inside 1920×1080) |
| `saves-overwrite-1280-1280x720.png` | `70e6ec076073…` | the confirmed overwrite of the same slot after the explicit tick |
| `saves-restarted-1280-1280x720.png` | `5013aae8aad2…` | after the restart: a fresh Year 0 session lists the slot as loadable, with **no** Restore backup control |
| `restored-map-1280-1280x720.png` | `a713f8d645d3…` | the restored mixed map at the saved year, the approved plan visible as an unapproved draft |
| `restored-map-1920-1920x1080.png` | `2147ede8bfa9…` | the same restored map at 1920×1080 |
| `restored-consent-1280-1280x720.png` | `4d66a6d1fa76…` | the pending draft's consent dialog when continuing |
| `continued-map-1280-1280x720.png` | `fc292bd7b2f2…` | one real year later, with growth and the new history entry |
| `saves-corrupt-1280-1280x720.png` | `4add8931fadb…` | the damaged slot: "Corrupt — not loadable (JsonInvalid)", Load withheld, **Restore backup** offered |
| `saves-recovered-1280-1280x720.png` | `3c6218f277a4…` | after the explicit recovery: the slot "Ready to load" again |
| `recovered-map-1280-1280x720.png` | `22a93032154f…` | the recovered save loaded back on the map at its year |
| `mixed-execute-confirm-1280x720.png` | `0811eb52cd0a…` | the Execute confirmation during the pre-save play |
| `edge-demo.ccf2d.json` | `cb76b02a84cd…` | the saved file (schema 2, year 1, `mad-ma1-mixed-demo-v1`); the recovery phase leaves the earlier generation as the primary |
| `edge-demo-ma1.ccf2d.json` | `993018c8d24a…` | the all-Sitka MA-1 save |
| `pre-save.json` | `772e6be8994a…` | the pre-save session id/year/tree counts the driver compares against |

Assertions that matter (all PASS): a real pointer event on the map is handled; the Fell/Plant/Shelter batch is queued from
the page origin; **Approve** and **Execute** are real button clicks; one real **Advance** click commits a year; the Saved
games dialog opens from the header button **by keyboard** and a **keyboard** save commits ("Tab from the slot name reaches
the Save control"); the page states *"Saved and durably committed to disk"*; saving the same slot without the tick warns and
writes nothing, and the tick then commits the overwrite; the restarted server starts at Year 0, lists the slot and offers
**no** Restore backup control because the primary verifies; **Load** demands explicit replace-play confirmation and restores
the saved year, both tree rosters and a plan that was **approved in the file** as an **unapproved pending draft** with an
Approve control and no approved plan; a stale-fence **load** returns 409 `PlayerSessionMismatch` with the unchanged restored
session; the pre-restart fence and pre-save token are refused for commands; a real **Advance** click continues a genuine year
with changed DBH/height/crown; the Results pane gains the history entry; **a damaged primary is listed as corrupt with Load
withheld and Restore backup offered, the confirmation names the damaged primary, the recovery commits, the slot becomes
loadable, and the recovered save loads back at its year**; and the all-Sitka run shows the other forest's slot as *"Different
forest, model or engine (SaveGenesisMismatch)"* with no Load button. No uncaught exception, no CSP violation, no 5xx, no
script error (deliberately provoked typed 4xx refusals are excluded and named in the driver).

## 7. Honest disclosures

- **One test pin was narrowed, and it is OUTSIDE the literal "registration only" grant for that file.** `644 B-01`
  ("P1 production source contains no file-writing API") was written when the lab wrote nothing at all. Save/Load makes
  exactly one reviewed writer real, so the guard in `tests/Core.Tests/Program.cs` now exempts
  `src/Core/Phase1/Phase1SaveSlots.cs` by name **and asserts that exactly one exemption exists**; every other file keeps the
  original ban. This is a deliberate, visible widening of a safety pin, not a weakening by omission, and
  `tests/Phase1.Save.Tests` is the compensating suite — but the directive grants that file for "registration only", so this
  edit **needs the Overall Manager's bounded sign-off** rather than being reported as inside the allowlist (independent
  review made the same point). Reverting it is not a safe alternative: it would either fail the build gate or force the save
  store to disguise its file calls from the guard.
- **Smoke harness changes:** `ui-smoke-multiarea.sh` now sets a hermetic `XDG_DATA_HOME` under its scratch dir, expects the
  truthful shutdown line per run, and carries the new Save/Load checks. No existing check was deleted or relaxed.
- **The two-area page's copy changed; the single-area page's did not.** `PlayerLabels.SessionOnly` (single-area) is
  untouched and still true for that page.
- **`tests/phase1-save-ui` needs Windows Edge and Windows Node.** It is a desktop acceptance tool, not a CI gate, and it
  lives beside the shell smoke rather than inside it.
- **Review-repair scope:** the repair commit `803bdd83` is the code under re-review. Its own discipline is the same as the
  first head: no file outside the §4 allowlist, no `main` merge, no PR.

## 8. Remaining limitations (not hidden, not blocking this feature)

- **Local single-user Linux/WSL2 persistence only.** The atomic rename + directory fsync path is verified on this host;
  nothing claims crash-proof journaling, a power-loss guarantee beyond the reported `DurablyCommitted` state, or any
  Windows-native file path.
- **No save-on-quit / resume-on-launch / autosave.** The player saves explicitly.
- **No UI slot deletion or rename.** Slots are replaced (confirmed) and recovered, not deleted from the page.
- **The save folder is a startup dependency.** If `$XDG_DATA_HOME`/`$HOME` yields no usable, non-symlinked folder, the UI
  process refuses to start (exit 2, `SlotDirectoryInvalid`) and the single-area page is lost with it. Chosen deliberately:
  a player must never see a Save control that cannot work. Other IO faults at that point still surface as an unhandled
  exception rather than a typed refusal (review observation 2).
- **The per-slot gate is in-process only.** Two server processes sharing one folder could still lose a confirmed replace
  (the loser's file would not become the backup). This host runs one server; the store's no-clobber rename still protects a
  slot that did not exist, including during backup recovery (review observations R2c/R4).
- **The slot list marks a slot loadable from its identity only.** A save whose *options* differ (for example a different
  Stage 4 growth-phase table for the same experiment) would be listed as "Ready to load" and then refused with
  `SaveOptionsMismatch`. The supported launch cannot produce two different option sets for one experiment, so this is a
  latent UI wart rather than a reachable one (review observation R2b).
- **Corrupt-file refusal codes are the store's own** (`JsonInvalid`, `SaveUtf8Invalid`, `SaveNotCanonical`,
  `SaveChecksumMismatch`, `SaveInconsistent`, `SaveTooLarge`, `SaveSchemaUnsupported`). The page shows the code, but there
  is no player-facing repair beyond *Restore backup*.
- **`build-output-safety.sh` C7/C8 FAIL on this host, pre-existing.** The harness cold-rebuilds the pinned P0 Core DLL and
  requires the qualified SHA `9a99f781…`. The rebuild here is reproducible in place (C8 = C7 = `bcbdbf05…`) but differs from
  the pin, i.e. that build is not path-independent (the PDB/debug directory embeds the build path, and this lane builds from
  a different worktree directory than the qualified one). The P0 sources are untouched (tracked digest `9e218ca7…`
  unchanged, C1–C6 all PASS). The harness also **leaves its rebuilt `.p0ref` in place**, which breaks the CSProj pin for
  later builds; the pinned reference was restored from the read-only A0 copy immediately after the run.
- **`p0-isolation.sh` NOT RUN** for the same reason: it cold-rebuilds the pin and would fail on that rebuild, having already
  been shown not to be a P1-code question. The P0-untouched property is instead evidenced by the unchanged P0 tracked-file
  digest, `git status` on both labs and the zero diff outside `Tools/MultiRackSitkaP1Lab`.
- **Reviewer independence is by thread/model only** (see §11), which is weaker than a cross-vendor designation.
- **Not tested:** screen readers, zoom/DPR other than the two viewport sizes above, other browsers, a killed-process
  mid-write (the A0 fault-injection cases cover the store's stages, not an OS kill), 3D/Unity saves, Windows-native builds.

## 9. Ownership release

The single-writer lane is **released** on publication of this handoff: branch
`feat/multi-2d-phase1-save-e2e-20261124-v1` is pushed and unmoved, the worktree is clean, no server, Edge process, lock or
listener owned by this lane remains, and protected listeners `:33767`, `:43649`, `:46593` and every pre-existing
`sitkap1-ui` process were never touched. Laptop ZX20 / Ops registry / `lanes.json` / `unity-lock` were not contacted or
modified.

## 10. Next recommended task

Owner playtest of this branch as the Phase 1 Save/Load candidate (Save → quit → relaunch → Load → continue on `ma-1-mixed`),
then the separately-specified Phase 1 remainder: the *Skip to Suggested Review* controller and the paged/derived History
viewer. Neither is part of this Save/Load packet and neither is implemented here.

## 11. Independent review

**Reviewer:** Opus 5.5 (`claudeAgent/claude-opus-5-5`, effort xhigh), run as a separate read-only delegating thread with no
implementation or supervision role and none of the implementer's reasoning; separation is **by model and thread only**, which
is weaker than a cross-vendor designation. Thread:
[Independent review — Phase 1 Save/Load E2E](t3-thread://v2/030a041c-34a1-4375-b3ab-fe0ff5333ea9/thread:delegated-task:command%3Amcp%3A4df00662-3f4a-4824-b69f-45498757470e%3Adelegate-task%3A245ae5bd-43b4-4bc4-9cd3-7a2e9b365f48)
(not a GitHub-hosted document; the substance is recorded below and the reviewer's own files were left outside the repo at
`/tmp/rv-save-probe/`).

**First verdict on `ca750ab`: NEEDS CHANGES.**
- **G2 (save safety): FAIL.** (i) Orders that were approved in the file skipped the engine re-check, so an engine-illegal
  approved order could reach the restored Work Plan (NB2 not fully closed). (ii) **Restore backup** replaced a valid newer
  save with the older backup on one unconfirmed click — normal-path player data loss.
- **G3 (parity/data fidelity): PASS.** "P1-SL-01 shows the captured root is byte-identical after the round trip and matches an
  uninterrupted control for two more real years. I also checked every field of `MultiAreaRoot` and its Stage 4, Stage 5 and
  Phase 2 state records against `Phase1RootMapping.Capture`, and every one is captured."
- **Scope compliance: PASS** by path, with the `644 B-01` content caveat now disclosed in §7.
- **Handoff honesty:** two overclaims were identified and are corrected above — "revalidates *every* restored order" (§3/§4,
  README) and "Restore backup … safe"; plus undisclosed keyboard coverage and four demonstrations that were HTTP-only. All
  are now either fixed or stated.
- **Would the reviewer let a player use it?** *"For the plain flow, yes… I'm not approving it yet because of finding 2 …
  Finding 1 means the claimed NB2 fail-closed guarantee doesn't hold for approved plans."*

**Disposition:** both blockers repaired in `803bdd83` (§3.1 R1/R2) with new tests at every level, the four non-blocking
items that were cheap and safe were also repaired, and the remainder is disclosed rather than hidden.

### Re-review of the repair — APPROVED

**Reviewer:** a second, fresh independent delegate (same model, read-only, no implementation role; separation by thread
only). Thread:
[Re-review — Phase 1 Save/Load repair](t3-thread://v2/030a041c-34a1-4375-b3ab-fe0ff5333ea9/thread:delegated-task:command%3Amcp%3A4df00662-3f4a-4824-b69f-45498757470e%3Adelegate-task%3Aeeb9e87d-af00-4be3-aca0-cd8d3bf71ad9).
It reviewed `ca750ab..803bdd83`, at head `2dde7348`, clean worktree, and wrote nothing into the repository.

- **VERDICT: APPROVED.**
- **F1 closed: YES.** "Re-running the earlier repro (approved Plant at (-50,-50) …) now returns `REFUSED
  Phase2PointOutsideArea`. The engine's `root` object and session id are unchanged afterwards." It also reasoned that the
  validator guarantees the merged lists are exactly the approved orders, so all four kinds are checked, and that `root` is
  assigned only after every check passes.
- **F2 closed: YES.** It probed seven primary/backup combinations; recovery is refused for a valid primary (byte-identical
  afterwards), still works for corrupt or missing primaries, and is refused for a directory or symlink path. The page's
  `recoverable` gate is the only route to the control.
- **New blocking defects: none.** Its option-equality check found that a save this engine captures always passes, and that
  the four new tests would fail if their fix were reverted.
- **Required correction:** one claim was false — that concurrent saves get distinct generations. Repaired properly (R3 in
  §3.2). Five non-blocking observations were dispositioned in §3.2 (four fixed, one deferred and disclosed).
- It noted its own limitation: "this was not a blind review, and the separation from the first reviewer is by thread only
  (same model)".

**Final state of this branch:** IMPLEMENTED, REVIEWED (APPROVED after one repair cycle), **not INTEGRATED** (no `main`
merge, no PR) and **not ACCEPTED** (no owner playtest). The handoff's earlier overclaims are corrected in §3.1/§3.2.
