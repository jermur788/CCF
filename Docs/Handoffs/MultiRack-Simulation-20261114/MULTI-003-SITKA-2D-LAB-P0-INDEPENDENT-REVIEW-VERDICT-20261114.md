# MULTI-003 — Sitka 2D Reference Laboratory P0: INDEPENDENT ENGINEERING REVIEW VERDICT

**Date:** 14 November 2026 · **Claim:** [#497](https://github.com/jermur788/CCF/issues/497) · **Source claim:** [#494](https://github.com/jermur788/CCF/issues/494) · **Packet:** [MULTI-003 dispatch](https://github.com/jermur788/CCF/blob/docs/multi-003-sitka-lab-p0-independent-review-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-ENGINEERING-REVIEW-DISPATCH-20261114.md)

## Verdict: **APPROVED — FOR ENGINEERING P0 ONLY**

There are no critical or major findings. I record six minor findings and four notes (§3). None of them blocks P0, and none corrupts forest state, replay or the browser/host boundary. Two are real defects in the lab-data store (F-1 and F-2), but only a local process running as the same user and writing files into the gitignored `.lab-data/` folder can trigger them. The browser/HTTP side cannot reach them. I recommend one small optional hardening task (§6).

**APPROVED does not mean:** integration into Scenario One or Unity, a scientifically accurate Irish forestry model, a real marteloscope, user acceptance, readiness for P1 ecology, or a Windows/WSL/Firefox qualification.

---

## 1. Reviewer identity, lane and what physically ran

| Item | Value |
|---|---|
| Reviewer | **Anthropic Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in local T3 Code. Not the MULTI-002 author (Claude Sonnet 5.5), and no context was shared with that session. |
| Registration | [#497 RUNNING comment](https://github.com/jermur788/CCF/issues/497#issuecomment-6877207012), posted **before** any local test or report edit |
| Host | `jer-laptop`, native Linux 7.0.0-42, Intel i5-6300HQ (4 cores), 15 GB RAM, Files drive (ext4) |
| Review worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-003-SitkaLab-Review-20261114`. I created it fresh at 12:22 UTC; the path was absent before. |
| Review branch / pinned start | `review/multi-003-sitka-2d-lab-p0-independent-20261114-v1` @ `f33641f85ec8146ae26b91e4d5ea0b89bfba40f0` (0 ahead of the candidate) |
| Candidate | `feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1` @ `f33641f8…` (code commit `2a76260e…`, approved-spec base `c92af201…`). **Re-checked on origin at the end of the review: unchanged.** |
| Preflight | Live `active-tasks.md` on `main` (`fc18190b`) has no MULTI-002/003, #494/#497 or SitkaLab row. I took no board lock and made no board edit. No `dotnet`, Unity or Chromium process was running. I checked the author's worktree read-only (clean at `f33641f8`) and did not use, edit or clean it. |
| Toolchain | .NET SDK 8.0.133 / runtime 8.0.33 (pinned by `global.json`), Node v26.13.1, Chromium 156.0.8078.79, Python 3.12.3 |
| Off-repo evidence | `/media/jer/Files/CCF/Verification/MULTI-003-SitkaLab-Review-20261114/`, with `sha256sums.txt` covering 38 files. SHA-256 of that manifest: `b82a81009a9aa50eb0886a4238ea046f83e6b75b1111efa95ac48897b8adeaa2` |
| Source edits | **None** to the candidate or the review branch. Probes and mutants ran only in disposable copies under the session scratch directory, made with `git archive HEAD`. My probe and mutant scripts are kept under `…/reviewer-scripts/`. |

**Scope diff (independently re-derived):** `git diff --name-status c92af201 f33641f8` gives **51 files, all `A`dded**: 50 under `Tools/MultiRackSitkaLab/**` plus the one MULTI-002 handoff. Nothing under `Assets/`, `Packages/`, `ProjectSettings/`, `specs/`, `Docs/Project/` or the root `.gitignore`, and no `.meta` files.

### What I ran myself (REPRODUCED) and what I only read (WRITER CLAIMED)

| Gate | Command (run from `Tools/MultiRackSitkaLab` in the review worktree unless noted) | Result | Class |
|---|---|---|---|
| Clean build | `dotnet build MultiRackSitkaLab.sln` (no prior `bin/obj`) | exit 0, **0 warnings, 0 errors**, 13.7 s | **REPRODUCED** |
| Core suite | `dotnet run --no-build --project tests/Core.Tests` | exit 0, **55 passed / 0 failed** | **REPRODUCED** |
| Host suite | `dotnet run --no-build --project tests/Host.Tests` | exit 0, **8 passed / 0 failed** | **REPRODUCED** |
| Fixture manifest | `sha256sum -c fixtures/synthetic/MANIFEST.sha256` | 3/3 OK | **REPRODUCED** |
| Fixture regeneration | `dotnet run --no-build --project tools/FixtureTool -- <scratch>` then `cmp` | all 3 fixtures and the manifest **IDENTICAL** | **REPRODUCED** |
| Schema | `python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json` | 3 OK, 6/6 negative controls rejected | **REPRODUCED** |
| Mutation controls (all 17, once) | `python3 -I -B tests/run-mutation-controls.py <scratch> <json>`, run from an off-repo `git archive` copy | baseline green; **17 of 17 CAUGHT** (§4) | **REPRODUCED** |
| Reviewer extra mutants (22) | my own runner, same mechanism, off-repo | 14 CAUGHT, 7 SURVIVED, 1 rerun → SURVIVED (§4) | **NEW, reviewer** |
| Reviewer probes | throwaway console project referencing the unmodified Core/Host, off-repo | 28 probe checks; findings F-1, F-2, F-5 (§3) | **NEW, reviewer** |
| Tier 2 browser | `EVIDENCE_DIR=<reviewer evidence>/browser node tests/Ui/run-browser-tests.mjs` | exit 0, **15 passed / 0 failed**, headless Chromium | **REPRODUCED (headless only)** |
| 2,000-tree speed | browser UI-13 on this host; Core-only timing in the probe | browser **max 157 ms / mean 145 ms** (21 samples); Core preview + full view build max 5.3 ms | **REPRODUCED** |
| Writer's 169 ms max | handoff §5 | not re-measured as such; mine is 157 ms on the same host model | WRITER CLAIMED |
| Writer evidence integrity | `sha256sum -c sha256sums.txt` in `…/MULTI-002-SitkaLab-P0-20261114/` (read-only) | 29/29 OK; manifest hash equals the claimed `d459cffb…` | **REPRODUCED (integrity only)** |
| Headed browser, Firefox, Windows, WSL | — | not run | **NOT TESTED** |

I inspected these screenshots myself: `desktop-06-after-execute.png` and `narrow-390x844-selected.png` from my own run. They show the honest banners, a legend with shape plus letter (status is never colour alone), square stumps after execution, and a stacked single-column layout at 390 px. This was a reviewer look at real browser output. It is not a user acceptance session.

---

## 2. Source review by risk area

**A. Spec fidelity and custody: PASS.** There is one authority, `ForestSnapshot` (`src/Core/Models.cs:104-145`). The browser draws `Views.Build` projections of it (`src/Core/Views.cs:50-87`) and has no second tree store. Persisted values are integers only, in the exact A9 field orders (`CanonicalJson.cs:91-142, 199-229`). The fixed `.lab-data` root is gitignored. There are no growth fields, no PRNG, no Unity references and no network clients.

**B. Plan state and all-or-nothing: PASS.**
- Every live operation runs inside a `Tx` that applies events to a **clone** and commits state, events and hash together (`LabSession.cs:109-142`). A failed guard therefore cannot leave a partial mutation, whatever the guard order.
- `Engine.Apply(TestWorkExecuted)` calls `CheckExecute` before its felling loop (`Engine.cs:239-254`). `CheckExecute` (`Engine.cs:57-96`) checks the plan stage, then revision, then epoch, then duplicates, missing, not-live, below-threshold and unreserved trees.
- `LabSession.ExecutePlan` (`:229-245`) runs the same check first. On failure it appends exactly one audit-only `PlanExecutionRejected` event, with offending IDs and pre-hash equal to post-hash.
- The queue guard is now strict (`Engine.cs:209`).
- A tree can be in only one open plan: guards in create, propose, approve and queue, the unmark refusal, and the validator.

**C. Event chain, replay and epochs: PASS, with note N-1.**
- Every snapshot-changing action appends one event. Stale cascades are separate `PlanStale` events, which is consistent with A5 listing "stale" as an action.
- Replay re-derives every post hash, enforces a gap-free `eventSeq` and the prev/post chain, and runs the **same** `Engine.Apply` guards as live play (`LabSession.cs:15-43`).
- Load runs parse (header gate → strict JSON → typed keys in exact order → validator → canonical byte equality → snapshot hash). It then replays from the checked-in fixture, requires the replayed final hash to equal `snapshotHash`, and stales every APPROVED/PENDING_WORK plan through a recorded `SessionLoaded` event (`LabSession.cs:76-89`).
- My probes C1–C5 forged events that are **hash-consistent** but illegal: re-queue of a PENDING plan (the M17 shape), execute of a DRAFT, stale of a DRAFT, mark of a dead tree, and double execution. Replay or validation refused every one. Probe C6 checked a two-session history (approve → save → load → approve another → execute → reload): it replays cleanly and the first plan is STALE.

**D. Lab saves and file IO: PASS for static paths; minor F-1 and F-2.**
- Name-shape rejection happens before any file-system access (proved by a recording seam).
- Unlisted names are refused. Static symlinks, symlinked roots and FIFOs reached through a symlink are refused before open.
- Writes use `CreateNew` plus `rename`, so they never follow a symlink.
- CCF-shaped JSON inside the root fails at the header gate, with nothing written.
- No production save path appears in the source.

**E. Host / browser boundary: PASS.**
- Kestrel listens on `IPAddress.Loopback` only.
- The guard (`LabHost.cs:93-135`) allows only GET/POST/HEAD and refuses non-loopback peers. It requires `Host` to be exactly `127.0.0.1:<port>` and `Origin` to match exactly when present. It refuses cross-site `Sec-Fetch-Site`. POST also needs `Origin`, `X-Sitka-Lab: 1` and a JSON content type.
- There is no CORS allow header. The CSP is `default-src 'none'`, there is a 1 MB body limit, and responses carry no stack traces.
- My probes also refused the `127.0.0.1.`, `[::1]`, `127.1`, `0x7f000001` and port-less Host variants, `Origin: null`, and static traversal (`/%2e%2e/`, `..%2f`, NUL, `/.lab-data/`).
- `app.js` writes text with `textContent`/`createElement` only (the single `innerHTML = ''` is a clear). It does no forestry arithmetic: every N, BA, Dq, area and fraction it shows comes from the API.

**F. Geometry, fixtures and stand maths: PASS, with note N-2.**
- Geometry uses Int128 cross products. Closed-segment intersection catches touching and collinear overlap; adjacent fold-back is rejected. The point-in-polygon test is boundary-inclusive and exact.
- I recomputed the 3-tree oracle by hand: BA = π(0.10² + 0.15² + 0.20²) = **0.227765 m²**; on 0.25 ha, BA/ha = **0.911062**, N/ha = **12**, Dq = √(2900/3) = **31.0913 cm**; removing the 40 cm stem takes **0.125664 m²**.
- The demo polygon's shoelace area is **5,812.5 m²**.
- The 162- and 2,000-tree fixtures are formula-generated (`tools/FixtureTool/Program.cs`, no PRNG). I did **not** hand-audit them stem by stem.

**G. Verification strength:** see §4. **H. Performance and UX:** PASS; see §1 and N-3. **I. Ownership:** see §5.

---

## 3. Findings

No critical. No major.

| ID | Severity | Where | Finding and reproduction |
|---|---|---|---|
| **F-1** | Minor (local DoS) | `src/Core/LabDataStore.cs:27-31` (`IsRegularFile`), `:93`, `:109-111`; `src/Host/LabHost.cs:205-221` | **A FIFO in `.lab-data/` is treated as a regular file.** On Linux .NET, `FileInfo.Attributes` does not mark a FIFO, so `IsRegularFile` returns true. Probe D1: `mkfifo .lab-data/pipe.sitkalab.json`. The name is **listed**, and `ReadSave` **blocks** (no return within 3 s). Probe E10, over HTTP: `POST /api/saves/load {"fileName":"pipe.sitkalab.json"}` never returns. It holds `st.Gate`, so a concurrent `GET /api/state` also times out and the **whole lab host hangs** until a writer opens the FIFO. Precondition: a same-user local process creates a special file inside the gitignored lab root. The HTTP API cannot create one. |
| **F-2** | Minor (residual race) | `src/Core/LabDataStore.cs:108-111` | **Race between the symlink check and the open.** `ReadSave` checks `LinkTarget(full)` and then calls `File.ReadAllBytes(full)`, which follows symlinks. Nothing ties the open handle to the checked entry. Probe D2: a background thread swaps the entry between a regular file and a symlink to an outside file using atomic renames. Over 8 s, `ReadSave` returned the **outside file's bytes 7,867 times** (18,677 lab reads, 60,880 refusals). A hard link (probe D3) is read too. The header gate then rejects non-lab content, and no file content is returned to the client. However, a valid lab file elsewhere could be loaded, and a FIFO target hangs as in F-1. The AC-07 claim "symlink escapes fail **before file open**" therefore holds for static symlinks only, not under a concurrent same-user local writer. |
| **F-3** | Minor (test claim) | `tests/Core.Tests/LabDataStoreTests.cs:76` | `SymlinkAppearingAfterListing_IsStillRefusedBeforeOpen` does not test what its name says. Its forced link fires on the **root** check, as its own comment admits, so the per-entry check at `LabDataStore.cs:108` is never exercised. Mutant X08 removes that check and **survives**. |
| **F-4** | Minor (test gap; same pattern as M17) | `Engine.cs:233`, `Engine.cs:89`, `SnapshotValidator.cs:70`, `LabDataStore.cs:93/108` | Several guards are covered only because an outer layer already enforces the same rule. No test fails when each is removed: X01 (PlanStale stage guard), X06 (`others.Contains` in execute), X16 (validator double reservation), X08 and X22 (store filters). **None is a live defect**: my probes show the unmutated code refuses those cases. But a later refactor of the outer layer would lose the property silently, exactly as M17 nearly did. |
| **F-5** | Minor (robustness) | `src/Core/LabDataStore.cs:36-42`; `src/Host/LabHost.cs:103-107` | Saving onto a name squatted by a directory (`.lab-data/squat.sitkalab.json/`) returns **HTTP 500** and **leaves a `*.tmp-<guid>` file** in `.lab-data/` (probe E9), because `WriteAtomic` does not delete its temp file when `File.Move` throws. Separately, `Deny()` always reports `code:"Forbidden"`, even for 405, 413, 415 and 500. |
| **F-6** | Minor (spec wording) | spec A3 versus `tools/FixtureTool/Program.cs` | Spec A3 asks for "checked-in, **hand-audited** immutable synthetic fixtures". Only the 3-tree oracle is hand-audited. The 162- and 2,000-tree fixtures are reviewed closed-form index arithmetic: they regenerate byte-identically and are recomputed in tests, but are not audited stem by stem. The writer disclosed this. I accept it for engineering P0, but the Manager should know that the wording is not literally met. (Cosmetic: the FixtureTool comment says "quadrilateral" for a 5-vertex ring.) |

**Notes (no action required for P0):**
- **N-1, replay epoch rule.** Replay uses the epoch of the **most recent** `PlanApproved` event as the guard for later queue/execute events (`LabSession.cs:30`). This is sound because load stales every approval and reset wipes plans; probe C6 shows a multi-session history replays. It relies on an implicit invariant, though, since session boundaries are recorded only when an approval was outstanding. Keep this in mind if P1 adds new session-changing actions.
- **N-2, corrupt-state test seam.** The `ForTestingUnvalidated` seam is `internal`, visible only to `SitkaLab.Core.Tests`. The rejection reasons it drives (RevisionMismatch, EpochMismatch, TreeDuplicate, TreeMissing, TreeNotLive, TreeBelowThreshold, TreeNotReserved) are **not reachable through the live public API**: stale cascades pre-empt the revision/epoch cases, P0 has no mortality or growth, and reserved trees cannot be unmarked. Through a crafted save file, the same guards refuse during replay (C1–C5). Every reachable public path (not pending, already executed, stale, cancelled) has a public-API test. So seam-only coverage hides no feasible public attack path. Its weakness is F-4, not N-2.
- **N-3, UI.** Terminal plan cards still show all six action buttons, dashed. This is a deliberate demo choice (handoff §8.4) but it is noisy. The "review table" is an ID list plus a Core preview line rather than a table. The scale-bar label can overlap the boundary line. All cosmetic.
- **N-4, test hygiene.** The Core tests never delete their `Directory.CreateTempSubdirectory("sitkalab-test-")` folders. My runs (including mutation runs) created 216 such folders and I removed them. 192 older ones in `/tmp` predate this review and were left alone.

---

## 4. M17 and mutation coverage (separate finding)

**What happened originally (writer evidence, integrity-checked):** run 1 caught **16 of 17**. **M17 survived:** with `plan.Stage != Approved` removed from the queue guard, the other checks still refused DRAFT/PROPOSED (null base revision), CANCELLED/REJECTED/STALE (released marks) and EXECUTED (felled trees). Only a **PENDING_WORK re-queue** slipped through, and no test asserted it. The writer added "queue twice / queue cancelled / queue executed" assertions to `IllegalTransitions_…` and re-ran **M17 alone** (CAUGHT). The other 16 were not re-run after that test edit.

**Independent full re-run (this review):** I ran all 17 once on an off-repo copy at `f33641f8`. The baseline was green. Result: **17 of 17 CAUGHT**, M17 by `StateMachineTests.IllegalTransitions_AreRejectedAndAppendNoStateChangingEvent` (`mutation/full17-rerun.json`, sha256 `47e7b3ce…`). The test edit did not weaken the original 16. The runner only mutates its copy (I verified `shutil.copytree(ROOT, scratch)` and that all writes go to `scratch`), and I additionally ran it from a `git archive` copy.

**Was M17 a one-off?** No, the same layered-guard pattern recurs. My 22 extra mutants (`mutation/reviewer-extra-22.json`, sha256 `8f5d563d…`; X16 rerun `reviewer-extra-X16-rerun.json`):

- **Caught (14):** replay prev-hash, eventSeq and final-hash checks; load-stale; listing membership; CSRF header; POST-requires-Origin; Dq as arithmetic mean; body limit; approval revision capture; duplicate-key check; release on reject; stale-others-on-execute; replay epoch source.
- **Survived (8):**
  - X01, X06, X08, X16 and X22 survive only because an outer layer enforces the same rule (F-4).
  - X10 (Sec-Fetch-Site) is untested in the dotnet suites but **is** covered by browser test UI-14 ("cross-site fetch metadata" → 403).
  - X12 (non-loopback peer) cannot be tested from a loopback-only listener.
  - X20 (live approval epoch) is unreachable, because the API always supplies the live epoch.

**Conclusion:** the M17 gap is closed. The original 16 still catch. No surviving mutant points to a live defect in the current code. The F-4 survivors are a maintenance risk for later refactors, not a P0 correctness failure.

---

## 5. Acceptance matrix (engineering P0)

| AC | Verdict | Basis and limits |
|---|---|---|
| AC-01 only the lab lane touched | **PASS** | 51 added files re-derived (§1); no Unity, specs, board or root `.gitignore` |
| AC-02 same ID/XY/DBH/status/stage in Core and map | **PASS** | single authority (source); UI-01/02/06 reproduced |
| AC-03 identical canonical bytes/hash; stale-on-load differs | **PASS** | fixture regen identical; manifest; save/load byte-identity and stale-hash tests reproduced. Pinned runtime 8.0.33 on one host only |
| AC-04 exact BA/N/area/Dq; boundary, outside, bow-tie, below-threshold | **PASS** | oracle and demo area hand-recomputed; geometry source reviewed; tests and M02/M03/M09/M13/X13 caught |
| AC-05 mark→queue changes nothing; execute fells all and only | **PASS** | tests and UI-06 reproduced; Tx/clone source |
| AC-06 fail-closed matrix | **PASS** | public paths tested; seam-only reasons unreachable live and refused on replay (N-2, C1–C5) |
| AC-07 versioned isolated saves; traversal/symlink before open; header gate | **PASS with F-1/F-2 caveats** | static traversal/symlink/FIFO-via-link refused before open; header gate proven (M06). The before-open property does not hold under a concurrent same-user local writer (F-2), and a FIFO placed directly in the root hangs the host (F-1) |
| AC-08 UI plotting/filter/inspect/select, Core-only metrics, separate actions, replay, non-colour labels, screenshots | **PASS (headless Chromium only)** | 15/15 reproduced; screenshots inspected. Native `<select>` values are set by script; no headed browser or second browser |
| AC-09 SYNTHETIC / NO BIOLOGICAL GROWTH; calendar-only step | **PASS** | banners in UI, export and README; M08 caught; age/DBH/height unchanged in tests and UI-09 |
| AC-10 no real data, replica label, auto-prescription or Unity | **PASS** | source and README scan; host scan test |
| AC-11 negatives, perf, ownership, deps, docs; independent verdict | **PASS** | this report. No third-party packages (stdlib, in-box ASP.NET Core, Node/Python built-ins only) |
| REQ-U09 2,000 trees ≤ 2 s | **PASS** | 157 ms max measured on this host |

| Task | Verdict | Note |
|---|---|---|
| LAB-01 preflight | **PASS (documentary)** | Owner-approved provider/host/worktree substitution recorded on #494. There was **no live board row or lock** for MULTI-002 or MULTI-003; this review does not supply or imply board authority. |
| LAB-02 Core schema/fixtures/maths | **PASS** | F-6 wording caveat |
| LAB-03 state machine, atomic, events, saves | **PASS** | F-1/F-2/F-5 are minor hardening items |
| LAB-04 map, host, parity, loopback | **PASS (headless)** | — |
| LAB-05 Tier 1 + Tier 2 independent qualification | **PASS** | this review |

**Confidence:** high for Core state, replay, atomicity, maths and the host guard (source reading, reproduced suites, 39 mutants, forged-history probes). Medium for UI breadth (one headless engine; scripted dropdowns). The preflight and board gap stays as documented: no live coordination lock existed, and this review grants none.

---

## 6. Next action

1. **Manager:** you may accept an independent **P0 engineering qualification** of `f33641f8` while the lab stays isolated under `Tools/MultiRackSitkaLab/`. This acceptance is not a merge, Scenario One integration, user acceptance, or a P1 go.
2. **Optional, non-blocking:** one narrow author-repair packet (not a reimplementation):
   - F-1/F-2: in `ReadSave`, reject non-regular file types (FIFO, socket, device) and tie the open handle to the checked entry, e.g. open first, then verify the handle's type and resolved path inside the root.
   - F-5: delete the temp file when `Move` fails, and give `Deny()` accurate codes.
   - F-3/F-4: rename or fix the misnamed test, and add forged-history replay tests (C1–C3 shape) plus targeted tests that kill X01/X06/X08/X16/X22.
   - Afterwards: a focused recheck of those items only.
3. Separately, if the Manager wants it: a headed or second-browser smoke run. This is not required for P0.

**No source, Unity, specification, board, production save, real data or Windows build was touched by this review.** Dashboard/Drive synchronisation: **NOT VERIFIED.**
