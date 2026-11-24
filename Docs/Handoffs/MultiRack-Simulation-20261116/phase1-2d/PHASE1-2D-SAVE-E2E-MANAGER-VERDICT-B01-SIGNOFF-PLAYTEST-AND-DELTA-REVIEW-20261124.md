# 2D Phase 1 Save/Load — Overall Manager Verdict, 644 B-01 Sign-off, Owner Playtest and Final-Delta Review

**Issued:** 2026-11-24 after the DeepSeek Save/Load E2E completion report.  
**Decision:** **APPROVED for bounded 2D Save/Load task-branch delivery and owner playtest preparation**, with an **explicitly restricted 644 B-01 source-file exception**. **No G4 mainline integration or gameplay acceptance.** The review's APPROVED verdict was on the earlier repaired head; a narrow independent delta review of the post-approval code is **still required before labelling the final exact candidate independently reviewed**.  
**Manager evidence mode:** GitHub source and handoff inspection; did not run local .NET, Edge, destructive build-output harness or 3D/Unity.  
**Owner action:** conduct an interactive owner playtest of this separate 2D branch when convenient. No Windows build authorised.

## 1. Authoritative candidate and inspected evidence

- **Accepted preceding map source:** `feat/multi-map-integration-pass2-20261124-v1` @ `4b6d8070fe709f8b6600baedd39bb37d0cf5322d`, accepted as unmerged baseline.
- **Reviewed A0 save contract input:** `feat/multi-2d-phase1-a0-20261124-v1` @ `8db5e09538474cec01374066be08d73f240de82c`.
- **New task branch:** `feat/multi-2d-phase1-save-e2e-20261124-v1`. **Final source SHA** `a20640ae4fa5f025e68394bffa386236d02106e8`. **Final branch tip** `7e1b1eaa0784560fdb50bce0b9f0b57c8d820f84` (documentation-only commits after the source).
- **[DeepSeek implementation, tests and review chain](https://github.com/jermur788/CCF/blob/7e1b1eaa0784560fdb50bce0b9f0b57c8d820f84/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-SAVE-E2E-IMPLEMENTATION-AND-VERIFICATION-20261124.md)**. First Opus 5.5 review of `ca750ab...` found two blocking defects, repaired at `803bdd834a0181b7754e411c34381c024fa26f60`; fresh-thread Opus read-only re-review returned **APPROVED** on the repair and reproduced both F1 approved-order validation and F2 backup-recovery safety. Additional source changes at **`a20640ae...` occurred after this re-review**. The reviewer identified a false generation uniqueness claim, and it was repaired rather than hidden.
- **GitHub diff verified:** `4b6d8070..7e1b1eaa` = 7 commits ahead, 23 file changes, all under `Tools/MultiRackSitkaP1Lab/**` plus one phase1 Markdown handoff. No `Assets/**`, protected `.p0ref` or P0 lab source changes in this Git comparison.
- **Reported final-code local results from handoff:** Solution and UI builds 0 warnings/0 errors; Core **267/267** including frozen anchors; Save contract **48/48**; two-area smoke **276/276**; real headless Microsoft Edge **63/63** over four server processes and both 1280×720 / 1920×1080, including mixed forest Save→server quit→restart→Load→continue, restored unapproved draft, stale token refusal, damaged-primary backup recovery. `cli-safety` PASS. These local results are cited as worker observations, not independently rerun by browser Manager.
- **Disclosed exceptions:** single-area `ui-smoke.sh` **250 PASS / 4 FAIL** (`L01`, `L02`, `L06`, `J31`), matching the unmodified MapPass2 baseline according to paired comparison. `build-output-safety.sh` **C1–C6 PASS**, **C7/C8 FAIL** due to pinned P0 DLL cold rebuild differing by path-dependent debug directory; the harness leaves a rebuilt `.p0ref` and operator restored the protected pin after running it. `p0-isolation.sh` NOT RUN. No independent Manager verification of those environmental claims and **no failed gate is silently converted to PASS**.
- Branch and local writer reported **clean and released**, with old unrelated listeners intact; GitHub proves only remote tree/commit state, not current desktop processes.
- **Feature semantics:** genuine player-visible local WSL2 Save/Load, complete authoritative 2D root mapping, new post-load session fence, previous approved plans shown as unapproved pending draft, no silent approval, strict schema/genesis/model/option matching, honest durability/refusal and explicit backup recovery. No autosave, Windows-native/cloud saving, new skip controller or new paged history viewer.

## 2. Bounded 644 B-01 sign-off — APPROVED

**What changed:** In `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` under the named `644 B-01` scanner, the previous blanket prohibition on any production file/path writing API was narrowed to exempt **exactly one** named C# source file: `src/Core/Phase1/Phase1SaveSlots.cs`. The guard still searches every other `src/**/*.cs` for the same list of forbidden `File.*`, `FileStream`, `Directory.*` and `StreamWriter` tokens, and **`T.Eq(1, exempt)`** asserts that there is exactly one exception. The original guard on writing generated output to a regular file descriptor is unchanged. The exempt store's separate contract suite now tests its actual file behavior.

**Manager verdict:** **APPROVED AS A SINGLE-FILE, 2D SAVE-ONLY EXCEPTION**, retrospectively bringing this specific content edit inside the authorised task scope. The one-shot packet had granted `tests/Core.Tests/Program.cs` for registration only, so this clarification is required and is not waived by having passing tests. Approval applies to the exact implementation `a20640ae...` and the exact guard above. It does **not** approve exempting any additional source, bypassing protected P0 files, relaxing the rest of B-01 or changing the `Tools/MultiRackSitkaLab` generator/output policy.

**Reason:** Requiring a Save feature while banning the only approved save-store file API is internally inconsistent. This is the narrowest explicit exception, with no silent deletion of the guard and with a separate save safety suite. It is preferable to hiding file calls or falsely declaring a green test without persistence. The scan is token-based, not a formal proof against reflection/indirect IO: retain exact scope and independent review; no broad claim of filesystem safety beyond the demonstrated store behavior.

## 3. Independent delta-review packet — precise, no new feature scope

**Purpose:** independently assess post-approval changes from **`803bdd834a0181b7754e411c34381c024fa26f60` → `a20640ae4fa5f025e68394bffa386236d02106e8`**, rather than reopen all Phase 1 design or re-run a costly generic integration suite without reason.  
**Difficulty:** MEDIUM, because save locking/recovery are user-data integrity paths.  
**Provider:** an independent code/review-capable worker on **a separate thread from the writer** and from any helper that edited `a20640ae`. Review-only, no production edits.  
**Base:** accepted Map source `4b6d8070...`; reviewed repaired code `803bdd83...`; **exact final code** `a20640ae...`; branch `feat/multi-2d-phase1-save-e2e-20261124-v1` remains owned only for read-only final review, with source writer lane released.  
**Review worktree:** use a clean **detached SHA** or a separate, explicitly owned review-only worktree; do not reopen released write lane, alter reviewed feature SHA or overwrite existing process.  
**Review scope:** modified `Phase1SaveSlots.cs`, `PlayerMultiApi.cs`, `Phase1SaveIntegrationTests.cs`, `Phase1.Save.Tests/Program.cs`, `tests/phase1-save-ui/drive.mjs` and `run-edge-e2e.sh`; compare any changed handoff claims. Re-inspect `tests/Core.Tests/Program.cs` B-01 exception in the full candidate as a bounded check (though it predates the delta) and confirm it is exactly one source-file exemption. Read other code only as needed for contracts.  
**Specific adversarial checks:**
1. Per-slot gate spans generation-read → engine capture → atomic save without lock inversion; two simultaneous saves of the same slot really get distinct generations, with the correct backup. Independently run or review the new `NB1c` regression and attempt the old interleaving.
2. Recovery of a *missing* primary cannot clobber a competing new primary; valid newer primary is never displaced by backup, even if listing/confirmation becomes stale. Report in-process-only lock and multi-process limitation honestly.
3. Mutually exclusive Load/Restore confirmations and actual fourth browser phase: user explicitly confirms Restore backup and the recovered save is then loaded; no success over corrupted/invalid input; no stale-session bypass.
4. `P1-SL-07c` pins the actual `PruneBroadleafTargetInvalid` cause; `P1-SL-09` includes real Stage4 option mismatch. Tests must detect the intended mutations, not merely any refusal.
5. No unrelated UI or forestry drift; all nine anchors retained and no P0/.p0ref writes. Record exact tested SHA and what could not be run, rather than copying implementer's numbers.
6. Check whether the final save-source changed after `a20640ae`; if yes, restart the review on actual candidate and inform Manager. No review result is permission to merge to `main`.

**Verdict required:** `APPROVED / NEEDS CHANGES / REJECTED`, G2/G3 supported where touched. Publish one permanent GitHub Markdown review with **DIRECT URL**, exact tested SHA, own reproduced evidence, specific prior test baselines and limitations. A delta blocker returns to the same bounded save feature, not a general tooling refactor. No Unity, no Windows game build.

**Start gate:** This is a read-only review. Do not require laptop ZX20; use existing desktop local review custody and current live coordination. Do not claim a new source writer or alter main until Manager inspection.

## 4. Owner playtest (separate acceptance)

**Owner playtest candidate:** `feat/multi-2d-phase1-save-e2e-20261124-v1` @ exact source `a20640ae4fa5f025e68394bffa386236d02106e8` (branch tip `7e1b1eaa...` adds handoff prose). **This is an owner-facing standalone 2D lab, not a 3D Windows build.** The final delta review above is needed for fully independently reviewed final-head status; it need not be confused with the owner's personal usability feedback.

Use the approved WSL2 desktop launch instructions in `Tools/MultiRackSitkaP1Lab/README.md` from this candidate. For a disposable playtest use a fresh dedicated `XDG_DATA_HOME`, preserving any real player saves; use the single app server's capability URL, not an old stale server/session. Start `--multi-experiment=ma-1-mixed`.

**Shortest genuine manual exercise:** 
1. On `/multi`, perform a real Fell or Plant/Shelter Work Plan, approve and Execute, Advance 1 Year.
2. Open **Saved games**, name a new slot, Save. Verify the dialog reports year and committed status.
3. Exit the local app/server normally, restart it from the same folder / same `XDG_DATA_HOME`, return to `/multi`, and Load that slot after its explicit replacement confirmation.
4. Verify the mixed trees and year return, Work Plan/drafts are truthful, and a previously Approved-but-unexecuted plan requires new approval instead of retaining execution rights.
5. Advance one more real year and/or Execute newly approved work. Try a second Save under the same slot: it should require explicit overwrite confirmation; a corrupt slot should never silently count as saved.
6. Report what is clear/confusing in the slot listing, save/load buttons, confirmation text and whether the map remains usable at the normal desktop viewport. Do not edit or corrupt a valued save to test recovery; reserve such testing for disposable slots.

**Owner acceptance report:** PASS / FAIL / NOTES in a GitHub Markdown handoff, with source SHA and any concrete failed actions. The prior owner's map-only acceptance as an unmerged baseline is not automatic acceptance of Save/Load. No Windows build before the owner has also separately approved that gate.

## 5. Next actions and scope separation

- **Now:** Manager has signed off the exact B-01 change. Run one narrow final-source delta review, not a full new implementation task. Owner may playtest the branch as an explicitly unmerged candidate; avoid calling final exact head *independently approved* until that review returns.
- **After owner playtest and positive delta verdict:** return an explicit Manager G4 main/shared integration decision if desired; no automatic source merge, PR, tag, Drive publication or 3D gameplay claim. There is no blanket obligation to merge the 2D tool into Unity.
- **Later work, still not implemented:** separate Phase 1 `Skip to Suggested Review` controller (4-year advisory Fell anchor, max 5 actual annual steps) and paged/derived History viewer; do not pull in forest maker, budgets, labour or 3D.
- **Known harness debt:** C7/C8 build-output-safety FAIL remains a separate, environment-specific P0 pin/path problem, with `p0-isolation.sh` NOT RUN. Since running this harness rewrites a pinned DLL, **do not re-run it or touch P0 from this playtest packet**. If it blocks a future distinct qualification, obtain an explicitly bounded tooling repair with protected-pin and owner authority first. Treat it as FAIL/NOT RUN rather than a silent waiver.
- **Residual save limitations:** Linux-x64/local single-process, no cross-process slot gate, identity-only list loadability (later options mismatch still refused), no autosave, no Windows-native support, no malicious-file recovery other than verified backup. Do not overclaim durability or multi-user concurrency.

**Status as of Manager verdict:** `IMPLEMENTED: YES`; `REVIEWED: APPROVED for repair at 803bdd83; exact-final delta pending`; `INTEGRATED: NO (main)`; `VERIFIED: worker-tested end to end at a20640ae; reviewer exact-final delta pending`; `ACCEPTED: NO (save owner playtest pending)`.

**Manager sign-off: 644 B-01 exactly-one-source-file SaveStore exception APPROVED for this candidate; no unrelated safety waiver.**
