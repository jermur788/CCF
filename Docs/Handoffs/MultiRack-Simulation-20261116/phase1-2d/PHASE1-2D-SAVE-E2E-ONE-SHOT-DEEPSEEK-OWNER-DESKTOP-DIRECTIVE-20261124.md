# CCF Multi 2D — One-Shot End-to-End Save/Load Implementation

**Issued:** 2026-11-24, following owner's direct instruction at 20:53 UTC.  
**Recipient / orchestrator:** DeepSeek V4.1 Flash. **Difficulty: HIGH.**  
**Implementer:** one qualified, named desktop-local .NET/C# and browser-UI worker, routed by DeepSeek; **independent reviewer:** separate implementation-independent thread, preferably Sonnet/Opus or other capable model.
**Binding owner instruction:** *Development is happening on the desktop, not the laptop where ZX20 is located. Stop delaying desktop-only save work for a physical custody receipt from that laptop. Complete the save functionality end to end in one shot.*  
**Authority / supersession:** This is the **latest owner instruction** and a task-specific G4/G5 scope and admission amendment. It **supersedes** the laptop-ZX20 physical Ops preflight and A1→B separate-admission staging **for this desktop-only, non-Unity, 2D Save/Load task only**. Earlier Manager instructions requiring a ZX20 receipt for this task are withdrawn. It does **not** weaken desktop local one-writer ownership or any active Unity/3D/other-host locks.
**Decision:** **APPROVED TO ADMIT ON DESKTOP LOCAL PRECHECK** — **not yet RUNNING**; DeepSeek must actually claim the new branch/worktree with one writer and a live coordination row before making source edits. No approval to merge to `main`, no PR, no owner playtest claimed, no Windows game build.

## 1. Deliverable: a real player can save, quit, reload, and keep playing

**Single implementation + final qualification cycle.** Do not stop after A0, NB1/NB2 fixes, API plumbing, proof-of-concept codec or another planning handoff. Carry one cohesive change through **Core persistence → authoritative session restore → secure HTTP routes → usable in-page Save/Load controls → real browser verification → independent review → durable final GitHub handoff**. Repair reviewer-identified blocking defects in the same bounded lane and return final reviewed candidate, rather than opening a fresh series of A1/B permissions.

The minimum genuine demonstration must actually happen:
1. On the desktop's running `/multi` map, start **MA-1-MIXED** (Sitka, Oak, Beech); queue, approve and Execute real Work Plan activity, including planting a broadleaf juvenile if eligible, then advance a genuine year.
2. Enter a valid save-slot name and **Save** from the player UI. Confirm the slot exists in the server-managed restricted local directory and is fully verified; show label, year, model/genesis identity, saved UTC and committed/uncertain disk status honestly.
3. Quit/restart the local server, start a fresh browser/session and choose **Load** from the same UI. Restore the **complete authoritative forest root**, not a JSON view, while preserving all recorded trees, juveniles, seeds, Work Plan events, accounts, history, model options and exact numbers.
4. Show the restored state on the map; the old session token and all old action fences refuse mutations (including Execute replay). Restore executed receipts as data, but mint a fresh session authority. Any formerly approved unexecuted plan becomes an **unapproved visible draft** needing the player to Approve again, or load refuses unchanged if conversion is not safe.
5. Advance another **real** ecological year and show continued authentic growth/regeneration/work state, matching an uninterrupted-control simulation under identical operations. Prove there was no reset, silent migration or invented event.

This is **complete Save/Load functionality for the supported desktop WSL2/Linux-x64 2D lab**, not a Windows-native file API port or Unity/3D save integration. The server remains bound to its existing loopback capability URL, with no new file explorer, arbitrary path selector, remote client or browser export/import.

**Scope boundary to avoid inflation:** This one-shot instruction concerns **player-usable Save/Load**, including enough annual continuation and historical-record preservation to prove persistence. The separately approved Phase 1 *Skip to Suggested Review* controller and new paged/derived History viewer remain later Phase 1 work unless the owner explicitly asks to include them. Do not report the whole Phase 1 Save/Time/History milestone complete simply because Save/Load passes. Keep the existing `Advance 1 Year` player flow working; do not replace its ecological logic.

## 2. Exact starting context (immutable source inputs)

**Primary combined Map Pass 1+2 source:** `feat/multi-map-integration-pass2-20261124-v1` @ **`4b6d8070fe709f8b6600baedd39bb37d0cf5322d`**. This is owner-accepted as an **UNMERGED** base; independent Pass 1 review `1578096ae8385db645bb6c8dd567d70d33c60c36`, Pass 2 review `50f2a134acc95729feb5c8b0b674d8cf643c77ab`, G2/G3 PASS, 255/255 Core × 2, 240/240 smoke, four-cell real Edge review, nine protected anchors. [Map owner closure and file release](https://github.com/jermur788/CCF/blob/60d0c29d60840ef7e46687f94dbbf682806a749b/Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/MULTI-MAP-INTEGRATION-PASS2-CLOSURE-AND-SHARED-FILE-RELEASE-20261124.md).

**Reviewed save foundation:** `feat/multi-2d-phase1-a0-20261124-v1` @ **`8db5e09538474cec01374066be08d73f240de82c`**, originally forked from Phase 2 `cf6fe7a52aa096e824827c568f43fe05724dcce9`. Relative to Phase 2 it adds seven new `src/Core/Phase1/*.cs`, four new standalone Save test files, one historical implementation handoff; **no modifications or deletes**. [A0 independent APPROVED closure](https://github.com/jermur788/CCF/blob/5d7489e57e16a1d28e7aa210a27496b820811f95/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A0-CLOSURE-HANDOFF-20261124.md). Contains schema **v2**, strict deterministic codec, `Phase1SaveSlots`, `Phase1RootMapping` (compile-only), `Phase1SaveValidator`, 44/44 contract tests.

**Approved Phase 1 product contract:** [spec](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/spec.md), [plan](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/plan.md), [tasks](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/tasks.md), [requirements](https://github.com/jermur788/CCF/blob/docs/multi-2d-phase1-approved-orchestration-20261124-v1/specs/multi-2d-phase1-save-time-history-v1/checklists/requirements.md). Six owner-approved A defaults remain binding for persistence.

**Current desktop preflight evidence:** [DeepSeek blocked-admission report](https://github.com/jermur788/CCF/blob/c26bf61270c08a0abef0c20af77c31f70fb0791a/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A1-ADMISSION-BLOCKED-PHYSICAL-OPS-CUSTODY-20261124.md). Host `DESKTOP-DL1BQI7` (WSL2 Ubuntu, .NET 8.0.134) had both exact candidates, clean previous Map/A0 worktrees, zero A0 target collisions, and no new A1 branch/worktree. The only failed preflight was laptop ZX20 availability, **now explicitly removed for this specific desktop source-only task by the owner**. This report is historical blocked evidence, **not** a currently effective laptop-dependency hold.

**New source branch to claim:** `feat/multi-2d-phase1-save-e2e-20261124-v1`.  
**Exact new branch base:** `4b6d8070fe709f8b6600baedd39bb37d0cf5322d` — NOT `main`, NOT a docs branch and NOT the Map Pass2 checklist closure commit.  
**New exclusive desktop WSL2 worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-SaveE2E-20261124`.  
**A0 transplant:** bring over the eleven *reviewed source/test additions only* from `8db5e095...`, diff and preserve content before any deliberate fix; do not cherry-pick unrelated documentation commits or replace existing Map files. Confirm zero destination collisions first.

## 3. Desktop-only operational rules (owner-approved replacement for laptop Ops receipt)

**Do not contact or wait on laptop ZX20, `/media/jer/ZX20`, `lanes.json`, remote `unity-lock` or an Ops-host custody receipt for this non-Unity desktop save work.** The laptop's physical registry governs its own machine's Unity/Ops use; it is not a practical admission gate for an unrelated desktop WSL2 source-only lane. No one is authorised here to modify that remote registry, acquire/release a laptop Unity lock, or assert its state.

**Do perform one fast real desktop-local preflight**, then implement:
- Fetch live [`Docs/Project/coordination/active-tasks.md`](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), latest owner/Manager handoffs and the full committed context stamp. Check no newly admitted conflicting desktop writer since the previous report. **This handoff explicitly supersedes earlier `A1 HELD — ZX20` rows for Save/Load only.**
- Inspect actual desktop WSL2 Git worktree list, branch HEADs, clean status, any source writer/active tasks, and process working directories; check no overlapping writes. Do not treat divergent local primary `main` (`cbf3a618` in previous observation, 1864 ahead/59 behind `origin/main`) as the feature base. If a new, real local conflicting writer exists, stop its overlapping action and resolve ownership; **absence of ZX20 is not a conflict**.
- Record one named writer/provider, branch/worktree, exact source SHA, relevant file allowlist, and RUNNING claim in the **live GitHub coordination file** via the authorised manager/coordinator write route before source edits. Do not manufacture a historical physical receipt or claim a lock was inspected when it was not.
- Protect existing unrelated listening processes/ports (`33767`, `43649`, `46593`), current Map branches, A0 worktree and other workers. Use an OS-assigned new local loopback port for actual browser tests; do not kill another worker's process or acquire a Unity lock.
- If the exact source changed, proposed new worktree already contains unrelated modifications, or another real desktop writer owns one of the allowed paths, stop that conflicting action and report the actual cause. Do not repeat the laptop-XZ20 blocker.
- No arbitrary-time expiry or forced reclaim of named active workers (D-091); ownership ends on documented completion/release or an actual safe handoff.

**Single-shot orchestration:** DeepSeek may sequence coding, tests and review internally, but this is **one complete task packet**, **one exclusive writer/lane**, **one final review/repair cycle as needed**, **one GitHub implementation and verification handoff**. No new owner permission or phase packet should be sought merely for moving from Core to API to Save/Load UI inside the allowlist. The independent reviewer remains separate from the implementation thread. Review verdict does not imply owner playtest.

## 4. Exact source/test write allowance — G4 shared candidate and UI Save feature

One named source writer only, after desktop local admission. **Writeable:**

1. `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**` — reviewed A0 save contract and additional narrow helpers; include NB1/NB2 fixes.
2. `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` — complete root capture/transactional restore under single lock, new session fence and old-token refusal.
3. `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs` — minimal real session wrapper if needed.
4. `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContracts.cs` — change *only* save/session-only labels and directly related response contracts when real Save/Load exists.
5. `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs`, `src/Ui/PlayerMultiApi.cs`, `src/Ui/Program.cs` — capability-protected save-slot API, current session swap, and accessible Save/Load dialog/panel in the existing `/multi` map page. `PlayerMultiApi.cs` already contains the `MultiAreaPage` HTML/CSS/JS strings; preserve unrelated Map Pass2 behavior.
6. `Tools/MultiRackSitkaP1Lab/README.md` — narrow truthful Save/Load instructions; do not rewrite unrelated historical content.
7. `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/**`; **new** `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1SaveIntegrationTests.cs`; `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` (registration only); `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh` (save-specific smoke); **new** `Tools/MultiRackSitkaP1Lab/tests/phase1-save-ui/**` for real Edge driver evidence/helpers if necessary. Narrow updates to tests that explicitly pin false session-only copy are allowed only in the above named existing test files; if other test pins require edits, stop that **one path** for a concrete scope amendment but continue allowed code.
8. New **GitHub Markdown** handoff/evidence within `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/**` on the implementation branch.

**Read-only:** all other `Tools/MultiRackSitkaP1Lab/src/Core/**` including existing Stage4/Stage5/Phase2 ecological maths, all other `src/Ui/wwwroot/**` and `src/Ui/SitkaP1.Ui.csproj`, other tests, `strictJson` implementation and canonical specs except reading approved contracts. Reuse existing works instead of duplicating simulations.

**Excluded outright:** `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Tools/MultiRackSitkaLab/**` P0, `.p0ref/**` reference generation/modification, frozen models/fixtures, Legacy40, Compact30, Reference Future v1, owner archives, Unity Editor, Windows build, external remote clients, new generator, labour/economy, multi-user accounts, invented ecology, changes to standalone 3D saves, PR/merge into `main`, force-push. No broad test harness or dependency upgrade. Protected reference pin must remain byte-identical.

**Scope expansion rule:** if unavoidable outside this list, record the exact missing file/reason and seek one bounded Manager correction; do not use that as a reason to stop safe progress on the already approved vertical slice.

## 5. Implementation requirements — must all exist before calling Save/Load complete

### A. Authoritative state and persistence

- Adapt A0 schema v2, codec, validator, root mapping and named slots onto the exact Map Pass 2 source. Capture all authoritative mutable `MultiAreaRoot` fields **under one root lock**; preserve model-vector, manifest/genesis, seeds, all area rosters, revisions, events, executed keys, pending/approved plan intent, Stage4/5/Phase2 and annual records. Capture not a presentation snapshot.
- Use the already implemented strict deterministic canonical encoding; exact schema/genesis/model lineage must match, with clear typed refusals. No auto-upgrade; no 3D save import.
- Slot paths remain server-managed, lower-case/named and confined to a dedicated app-data directory. Explicit player confirmation to overwrite or load over current unsaved work. List slots and show label/year/UTC/status without exposing filesystem paths; deny arbitrary path, symlink/hard-link, corrupt, duplicate, malformed and oversized input.
- Preserve A0 **F2** decision: confirmed overwrite may recover from unreadable primary while never displacing a valid backup with corrupt data. Give the player an honest corrupt-primary/valid-backup warning. Backup recovery must be explicit and safe.
- **NB1 MUST FIX:** on initial absent slot, use no-replace publish even for `replaceExisting:true`, and serialize same-slot mutations; prove concurrent no-clobber. Preserve atomic-write/durability error signaling. Never claim save success on failed or indeterminate flush.
- **NB2 MUST FIX:** reject invalid restored orders, including Fell on Crop-marked tree, dual Fell+Prune, duplicate or invalid work, and any other engine-illegal order. **Fail the entire load unchanged** rather than repairing or discarding them.
- When loading, validate and construct a complete candidate root **before** publishing a new session atomically; on any file/model/state/plan failure preserve the old live session and UI authority.
- On successful load, use a fresh random session ID not derivable from file or colliding with loaded plan-ID prefixes; invalidate all former browser fences, including Execute replay, while preserving executed idempotency receipts as historical data. Restore `PlanStateRevision` precisely. Any previously approved and unexecuted work becomes **visible Pending/unapproved** and needs fresh explicit Approve; do not auto-cancel it or retain execution authority. If a safe conversion cannot be proven, refuse load.
- Maintain the single shared `MultiAreaContinuation` instance behind the Work Plan adapter and all six existing Pass2 write routes. No separate stale API session should remain after load or restart; no torn view/frame across `view`/`stage4`/`ecology`. No grant to change growth, recruitment or annual model semantics.

### B. Real player Save/Load UI (not just backend endpoints)

- Provide an obvious **Save**, **Load** or **Saved games** control in `/multi`, with a small, keyboard-operable slot dialog/panel that fits at **1280×720** as well as **1920×1080**. Show slot list, year, saved time and compatibility/verification state, plain-error messages and confirmation for replace/load.
- Save request uses current fenced session state; stale source, old session or an in-flight irreversible command must not save unexpected state. Load swaps the authoritative session only after validation; require explicit confirmation that it will replace unsaved current play, and explain pending reapproval. On return, refresh the full composite map in one fresh server state, clear old selection/preview/draft/active requests, and require the new fence.
- Explicitly distinguish **Saved/durably committed**, **write replaced but durability unconfirmed**, **invalid/corrupt**, **backup available**, **wrong forest/model**, and **load refused; current session unchanged**. Do not show false success or a silently empty slot. No accidental click→overwrite, no path upload UI.
- Remove *all relevant false Save-is-unavailable session copy* **only once the feature is real**, particularly `MultiSessionOnly`, `MultiChipSessionOnly`, `PlayerMultiApi.cs`, `Program.cs`, and README. This save feature must not claim remote/cloud backup or guarantee crash durability beyond the proven Linux host behavior.
- Retain existing Pass 1+2 four map modes, six existing Fell/Prune/ClearUndergrowth/Plant/Shelter/SpotClear Work Plan order types, fence/Origin/Host/body guards, tree focus/selection and overlays. No unrelated map design redo. Test that map work executes before and after loading.
- Ensure Save/Load is player-accessible through the current supported WSL2 loopback host; use real Microsoft Edge CDP or equivalent actual browser, not just mocked JavaScript/HTTP.

### C. Tests and concrete acceptance evidence

**Run on exact final candidate SHA**, document environment/commands/logs/NOT RUN and independent reviewer verdict:
- A0 save contract regression (`44/44` at baseline), plus dedicated save integration cases: root field-by-field equality, approved→pending, old-token refusal, receipt preservation, Stage4/5/Phase2/mixed state, multi-year continuation parity against uninterrupted control (only **actual** annual ticks, not new skip behavior).
- Negative cases: corrupt checksum/file, unknown schema, missing/duplicate JSON, model or genesis mismatch, file-size limit, malformed path/name, unsafe restored Fell/Prune plan, symlink/hardlink, disk/write fault, new-slot race and concurrent same-slot saves, backup survival, load failure leaves active session byte-equivalent and commands working.
- Core solution and UI clean build; full prior Core test suite (Pass2 baseline **255/255**), `tests/ui-smoke-multiarea.sh` (Pass2 baseline **240/240**) plus new save tests. Do not weaken old tests to achieve green.
- Preserve/recheck **all nine frozen anchors**: P0 pin, model-0, Stage4, Stage5, Phase2 Q6 and MA-1-MIXED 30y. No frozen reference or protected fixture changes. Verify git diff against `4b6d8070...` is solely inside this packet's allowlist.
- **Real local browser acceptance:** save from `/multi` (MA-1 and MA-1-MIXED), quit/restart actual process, reload slot, inspect restored map/work plan/juveniles/annual year, force an old token refusal, continue Execute or Advance and compare authoritative results. Check 1280×720 and 1920×1080; no console/CSP/5xx errors except deliberately triggered typed refusals; no save capability URL leak. Existing default/map performance budgets remain true.
- Demonstrate at least one valid backup recovery, one invalid-load nonmutation, one overwritten-slot explicit confirmation and a formerly approved work plan visibly pending after load.
- Independent review by someone who did not write or supervise the implementation code as its helper; must return canonical **APPROVED / NEEDS CHANGES / REJECTED**, note G2 save safety and G3 parity evidence. If NEEDS CHANGES, perform tight repair within the **same approved lane** and rerun affected gates; a reviewer must re-review the repair.

**Done means:** a user can Save / Quit / Relaunch / Load / Continue on the actual desktop 2D map, the complete authoritative forest state persists and remains coherent, stale commands fail, and the entire user flow is independently reviewed and verified. Do not claim DONE from passing serialization tests, compilation, or an untested button. If a blocked safety condition is genuinely encountered, return a clear factual blocker rather than fabricating a PASS.

## 6. Reporting / single-shot closure

At task start report only **actual** local desktop claim facts: named writer/provider, branch/worktree/HEAD, live coordination RUNNING row and checks, then proceed without another owner loop. One bounded code lane and one independent final review; avoid procedural pauses between Core/API/UI.

At finish, publish **one primary permanent GitHub Markdown closeout** on the feature/docs branch with final source SHA, file diff, save-file safety tests, exact .NET/Edge results, proof of **Save→Restart→Load→Continue**, A0 NB1/NB2 disposition, open limitations, independent reviewer URL/verdict, current ownership release and next recommended task. Additional independent review Markdown is permitted when required for true independence; it is not a new implementation phase. Label `IMPLEMENTED`, `REVIEWED`, `INTEGRATED`, `VERIFIED`, `ACCEPTED` distinctly.

**Integration semantics:** this is a single-feature *task-branch join* of reviewed A0 additions onto MapPass2. It is NOT a merge to `main`, NOT a 3D integration, and NOT owner playtest/acceptance merely because review succeeds. The combined save branch will remain available as the next owner playtest candidate; no Windows game build. DeepSeek retains orchestrator responsibility and the Overall Manager retains later mainline/G4 merge decision.

**Previous grant supersession (specific, explicit):**
- `PHASE1-A1-MANAGER-G4-CONDITIONAL-SHARED-FILE-GRANT-20261124.md` §3.2 (laptop physical Ops receipt) **superseded** for this desktop-only Save/Load source work.
- Its separate `A1 then Phase B` permission gates are **consolidated only for Save/Load UI**, under this one packet. Skip/review-history UI are **not** silently added.
- `PHASE1-A1-MANAGER-OPS-CUSTODY-READBACK-REQUEST-20261124.md` is historical/withdrawn as a prerequisite for this task; no Ops-host request is needed to start these source edits.
- Existing single-writer/no-conflict, protected anchors, independent review, owner Work Plan approval, Unity locks and no-main-merge rules remain mandatory.

**Final manager instruction:** Treat the desktop as the development machine. Do the real local conflict check, claim one writer, then **finish player Save/Load end to end in one shot**.
