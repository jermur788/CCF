# CCF Multi 2D — Phase 1 Save/Load End-to-End: Implementation and Verification

**Date:** 2026-11-24 (desktop-local lane, `DESKTOP-DL1BQI7`, WSL2 Ubuntu).
**Directive:** [owner one-shot desktop directive](https://github.com/jermur788/CCF/blob/docs/multi-2d-save-e2e-one-shot-owner-directive-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-ONE-SHOT-DEEPSEEK-OWNER-DESKTOP-DIRECTIVE-20261124.md).
**Sole writer:** `DeepSeek V4.1 Flash orchestrator` (desktop-local implementation thread; commits authored under
`DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>`). **Independent reviewer:** separate read-only
thread with no implementation or supervision role — see §11.

| Label | State |
|---|---|
| IMPLEMENTED | **YES** — one cohesive change, Core → API → in-page UI |
| REVIEWED | **PENDING** at first publication; verdict recorded in §11 when returned |
| INTEGRATED | **NO** — task-branch join only; no merge to `main` (not authorised) |
| VERIFIED | **YES for the stated surface**: 264/264 Core, 46/46 save contract, 273/273 map smoke, 47/47 real Edge end-to-end |
| ACCEPTED | **NO** — owner playtest has not happened; reviewer verdict is not owner acceptance |

**Branch:** `feat/multi-2d-phase1-save-e2e-20261124-v1`
**Base (exact):** `4b6d8070fe709f8b6600baedd39bb37d0cf5322d` (owner-accepted Map Pass 2, unmerged)
**A0 source (transplanted):** `8db5e09538474cec01374066be08d73f240de82c`
**Implementation head:** `ca750ab5787cad29300fd5e525583a8b00c34749` (this document's own commit follows it)
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
DTO rules, the engine `Restore` revalidates every restored unexecuted order with its own guard (`RequireLivingInArea`,
`RequirePrunable` at the restored year, `RequireClearCell`, `Phase2Planting.RequireValidBatch` against the live polygon
lattice). **Any** illegality fails the entire load and publishes nothing — nothing is repaired, dropped or partly applied.
Proven by `P1-SL-05a/b/c` (validator, exact rule named in the message) and `P1-SL-07` (a structurally valid save whose
planting point the engine alone can judge is refused with `Phase2PointOutsideArea` and the live session is byte-identical
afterwards).

## 4. Load policy and the safety properties it rests on

- **Capture under one lock.** `CaptureSave` takes the engine's single lock, checks the caller's full fence, and copies the
  whole committed root: both rosters (dead stems included), Crop marks, year and both revisions, plan-state revision, event
  ledger, executed-key receipts, pending plan, Stage 4 pruning/deadwood, Stage 5 cells/bands/accounts and Phase 2
  species/juveniles/shelters/patches/accounts. A stale fence captures nothing.
- **Identity is exact, never migrated.** Load requires the same model vector, genesis identity (experiment id, manifest
  content hash, generator id/version/seed, every area id, seed and area) and engine lineage (RNG, growth, P1 algorithm,
  area model, 230 mm threshold). Different forest/model/engine → `SaveGenesisMismatch` / `SaveModelMismatch` /
  `SaveLineageMismatch`, and the running session is untouched.
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
  backup only from a still-verifying primary, one atomic rename, directory fsync. `DurablyCommitted` vs
  `ReplacedDurabilityUnconfirmed` is reported to the player, never flattened into "saved".
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
| Core suite incl. nine anchors | `tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests` | **264/264 passed** (255 baseline + 9 new `P1-SL` cases) |
| Save contract suite | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **46/46 passed** (44 A0 + 2 NB1) |
| Two-area map smoke | `tests/ui-smoke-multiarea.sh` | **273 passed, 0 failed** (240 baseline + 33 Save/Load) |
| Single-area smoke | `tests/ui-smoke.sh` | **250 passed, 4 failed** — `L01`, `L02`, `L06`, `J31`; the **same four IDs fail on the unmodified base `4b6d8070`** (re-run in the Pass 2 worktree), so this is pre-existing drift, not a regression |
| Real browser end-to-end | `tests/phase1-save-ui/run-edge-e2e.sh` | **47 passed, 0 failed** across three server processes |
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

## 6. Real browser acceptance evidence (Save → Quit → Restart → Load → Continue)

Runner: `tests/phase1-save-ui/run-edge-e2e.sh <evidence-dir>` (three processes, real SIGINT shutdown, no stubs).

Evidence directory (outside the repository): `/mnt/c/Users/hp/AppData/Local/Temp/ccf-save-e2e-run3`
(`C:\Users\hp\AppData\Local\Temp\ccf-save-e2e-run3`), 47/47 assertions, `run.log` sha256 `5358824186edb1f6…`.

| Evidence | sha256 (prefix) | What it shows |
|---|---|---|
| `saves-empty-1280-1280x720.png` | `fabb13981c52…` | the dialog at 1280×720: fits with no horizontal scroll (`dialog 832×…`, table 792 px), Save control inside the viewport |
| `saves-saved-1280-1280x720.png` | `c3f2a9790fba…` | slot `edge-demo`, year 1, UTC time, "Ready to load", Load button |
| `saves-saved-1920-1920x1080.png` | `fc1234e753ad…` | the same dialog at 1920×1080 (832×473 inside 1920×1080) |
| `saves-restarted-1280-1280x720.png` | `1ddc24ac0734…` | after the process restart: a fresh Year 0 session lists the saved slot as loadable |
| `restored-map-1280-1280x720.png` | `da940ab2b8a7…` | the restored mixed map at the saved year |
| `restored-map-1920-1920x1080.png` | `b23246a7b665…` | the same restored map at 1920×1080 |
| `restored-consent-1280-1280x720.png` | `117ac4e69d28…` | the pending draft's consent dialog when continuing |
| `continued-map-1280-1280x720.png` | `fc292bd7b2f2…` | one real year later, with growth and the new history entry |
| `mixed-execute-confirm-1280x720.png` | `0811eb52cd0a…` | the Execute confirmation during the pre-save play |
| `edge-demo.ccf2d.json` | `9f862ff3df07…` | the saved file itself (354,740 B, schema 2, `saveGeneration` 1, `ecologicalYear` 1, `mad-ma1-mixed-demo-v1`) |
| `edge-demo-ma1.ccf2d.json` | `da451ba8007d…` | the all-Sitka MA-1 save (179,427 B) |
| `pre-save.json` | `24f873eaea93…` | the pre-save session id/year/tree counts the driver compares against |

Assertions that matter (all PASS): a real pointer event on the map is handled; the Fell/Plant/Shelter batch is queued
from the page origin; **Approve** and **Execute** are real button clicks on the real page; one real **Advance** click
commits a year; **Save** is a real click after real typing; the page states *"Saved and durably committed to disk"*; the
restarted server starts at Year 0 and lists the slot as loadable; **Load** demands an explicit replace confirmation and
then restores the saved year, the exact tree rosters of both areas and the pending draft; the pre-restart fence and the
pre-save token are both refused with `409 PlayerSessionMismatch`; a real **Advance** click then continues a genuine year
with changed DBH/height/crown; the Results pane gains the history entry; and the all-Sitka run shows another forest's slot
as *"Different forest, model or engine (SaveGenesisMismatch)"* with **no** Load button. No uncaught exception, no CSP
violation, no 5xx, no script error (deliberately provoked typed 4xx refusals are excluded and named in the driver).

## 7. Honest disclosures

- **One test pin was narrowed.** `644 B-01` ("P1 production source contains no file-writing API") was written when the lab
  wrote nothing at all. Save/Load makes exactly one reviewed writer real, so the guard now exempts
  `src/Core/Phase1/Phase1SaveSlots.cs` by name **and asserts that exactly one exemption exists**; every other file keeps the
  original ban. This is a deliberate, visible widening of a safety pin, not a weakening by omission; `tests/Phase1.Save.Tests`
  is the compensating suite.
- **Smoke harness changes:** `ui-smoke-multiarea.sh` now sets a hermetic `XDG_DATA_HOME` under its scratch dir, expects the
  truthful shutdown line per run, and carries the new Save/Load checks. No existing check was deleted or relaxed.
- **The two-area page's copy changed; the single-area page's did not.** `PlayerLabels.SessionOnly` (single-area) is
  untouched and still true for that page.
- **`tests/phase1-save-ui` needs Windows Edge and Windows Node.** It is a desktop acceptance tool, not a CI gate, and it
  lives beside the shell smoke rather than inside it.

## 8. Remaining limitations (not hidden, not blocking this feature)

- **Local single-user Linux/WSL2 persistence only.** The atomic rename + directory fsync path is verified on this host;
  nothing claims crash-proof journaling, a power-loss guarantee beyond the reported `DurablyCommitted` state, or any
  Windows-native file path.
- **No save-on-quit / resume-on-launch / autosave.** The player saves explicitly.
- **No UI slot deletion or rename.** Slots are replaced (confirmed) and recovered, not deleted from the page.
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

**PENDING at first publication.** The reviewer is a separate read-only thread with no part in the implementation, briefed
to return a canonical **APPROVED / NEEDS CHANGES / REJECTED** verdict on G2 (save safety) and G3 (parity), and told to
report blocking findings rather than repair them. Its verdict, URL and the disposition of any finding are appended here
before this branch is reported as REVIEWED.
