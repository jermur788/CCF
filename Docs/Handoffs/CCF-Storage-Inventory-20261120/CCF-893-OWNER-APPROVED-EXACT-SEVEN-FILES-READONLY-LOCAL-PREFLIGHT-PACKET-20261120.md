# CCF #893 — Owner-approved, exact-seven **READ-ONLY** Files worktree safety preflight

**Owner approval:** 2026-11-20 22:34:19 UTC — owner's “approved” to the immediate question asking for a “read-only local safety preflight of those seven exact worktrees, with no deletion, movement or archiving authorised yet.” This is **NOT a cleanup/removal grant**.  
**Class:** PACKET ONLY — current host state investigation; one worker, no game feature/Unity writes.  
**Difficulty:** MEDIUM (seven shared-store worktrees with owner/process/ignored-file risks; no Unity).  
**Status:** **OWNER-APPROVED / READY, NOT RUNNING**. No local worker was started by the browser Manager; no physical reservation, host lock or native success is claimed.  
**Issue:** https://github.com/jermur788/CCF/issues/893.  
**Suggested provider:** one local T3 Code/OpenCode DeepSeek V4.1 Flash read-only worker, or GPT-6.1 Sol/Codex with legal direct access to the intended Linux host. The provider is a recommendation, not a claim of launch.  
**Owner/coordinator:** #425 Overall Manager; #802 one-orchestrator Ops retirement retains its own custody; #892 research is closed, not a deletion grant.  
**Host:** verify `jer-laptop`, not a remote desktop/WSL or a different machine.  
**Source/reference:** [#892 Manager review](https://github.com/jermur788/CCF/blob/docs/manager-892-redundancy-review-verdict-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-892-MANAGER-RESEARCH-REVIEW-AND-FILES-FIRST-PREFLIGHT-DISPOSITION-20261120.md), [#892 306-row decisions TSV](https://github.com/jermur788/CCF/blob/docs/ccf-892-browser-redundancy-review-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/892-browser-redundancy-review/CCF-892-BROWSER-REDUNDANCY-DECISIONS.tsv) (blob `0f0dd8a68a1b528cbc82126d324f6d360627d83b`), [20 Nov all-drives inventory](https://github.com/jermur788/CCF/blob/docs/ccf-all-drives-inventory-report-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-ALL-DRIVES-READONLY-INVENTORY-REPORT-20261120.md).

## 1. Exact authorisation boundary — seven paths and nothing else

The paths below are the **entire target set**. Use exact root + full HEAD; read all other folders as *coordination metadata only*, not as new cleanup targets. All seven are registered in the canonical shared store `/media/jer/Files/CCF/Repository.git`. The last inventory indicated clean/pushed, but that is a snapshot and is **not** current proof.

| Register ID | Exact absolute path | Inventory HEAD (full SHA) | Branch | Inventory allocated bytes |
|---|---|---|---|---:|
| `CCF-INV-0128` | `/media/jer/Files/CCF/Operations/CCF-Overnight-FeedbackVerify-20261104` | `6efba8fdc35f22098faf205a5f7d14ccf65a3e1a` | `verify/overnight-playtest-feedback-20261104` | 1,232,261,120 |
| `CCF-INV-0129` | `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104` | `693e24c8891637470052031d5023883b922e9334` | `feature/overnight-objective-copy-20261104` | 1,232,629,760 |
| `CCF-INV-0131` | `/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104` | `9304d318f346b281a19cb805feebc6dc1a95008e` | `feature/overnight-rack-handle-20261104` | 1,232,502,784 |
| `CCF-INV-0132` | `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` | `d3ba4fce3994d5c59b51786c6671dd654791e7d1` | `integration/scenario-one-overnight-ui-qualification-v1` | 1,233,047,552 |
| `CCF-INV-0102` | `/media/jer/Files/CCF/Operations/CCF-MFM08Closure` | `911a8a34ffbaa2004697ffeabdfe21edcb82eb2d` | `verify/mfm-08-tier2-closure-v1` | 1,230,462,976 |
| `CCF-INV-0103` | `/media/jer/Files/CCF/Operations/CCF-MFM08Qual` | `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727` | `qual/mfm-08-unity-v1` | 1,231,458,304 |
| `CCF-INV-0104` | `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101` | `43e263708d8f517337181c92d0d78ed442803950` | `verify/mfm-10-species-guard-v1` | 1,231,798,272 |

**Snapshot total:** **8,624,160,768 allocated bytes (8.62 decimal GB)** on Files. That is only a possible recovery amount, **not actual freed space**. Inspect current sizes read-only if useful; record variance rather than editing.

**Critical distinction:** `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` is **NOT** the owner-unconfirmed, protected ZX20 `CCF-UIQualification-20261104` warm-Library playtest copy. Do **not** touch ZX20. The personal #361/#382 copies and all U1/U2/U3/Integration, `CCF-Primary`, `Repository.git`, `CCF-Ops`, #890/#891 and active Multi remain protected.

## 2. Initial safety preflight, before probing target tree content

1. Confirm actual laptop host, `/media/jer/Files` mountpoint, `UUID=2283b135-bf37-42d6-921f-2fb861d0a8d4` (as measured 20 Nov; **verify live**, don't trust `/dev/sdX` mapping), and that the seven paths resolve as real directories on this same Files filesystem with **no symlink or unexpected nested mount**. If identity differs, STOP affected path; do not mount, remount or repair.
2. Refresh **current** `Docs/Project/coordination/active-tasks.md` from GitHub and actual local Ops board/lease/writer state. Check task claims, owner activity and current physical occupancy, including separate #802. If any of the seven is assigned/active or a Git registry writer is active, mark HOLD and skip that path. GitHub issue comments are not native physical leases.
3. Verify actual `#893` owner-approved scope and that this worker is the **one** read-only preflight researcher, not a #802 remover or Unity agent. If a G0 inspection claim needs registration, follow current coordination process without touching live guarded CCF source/Unity or changing `active-tasks.md` except through its canonical owner. Distinct write ownership for the new GitHub report only.
4. No Unity Editor, Hub operations, imports or builds; no collision with #891 art activity. The seven path checks may be carried out while art development runs only if the live ownership/process checks show no interference.

## 3. Read-only checks **for each of the seven**; do not expand scope

Record per ID:

- **Identity:** host, realpath, Files UUID, common Git directory, current branch name, full `HEAD`, and `git worktree list --porcelain` registration. Unexpected branch/HEAD or shared-store mismatch => HOLD, do not reset/checkout/fetch.
- **Cleanliness / private data:** compare staged, unstaged and untracked tracked content using `GIT_OPTIONAL_LOCKS=0 git status --porcelain=v1 --untracked-files=all` (or equivalently read-only Git). **Enumerate ignored items explicitly**, e.g. `GIT_OPTIONAL_LOCKS=0 git status --porcelain=v1 --ignored=matching --untracked-files=all` and targeted directory metadata scan for `Library/`, `Build/`, saves, `UserSettings/`, local fixture logs, archives, third-party assets, backup files, hidden files or licence/provenance artefacts. Do not read or publish personal save content; report existence/type/size and source of protection. Never assume `Library absent` is a complete ignored-file proof.
- **Current use:** current local task owner, live CCF-Ops board, active agent claim, running process/cwd/open FDs into exact root, Unity/Git lease conflicts. If tools cannot reliably inspect processes/FDs (or permissions deny), record **UNKNOWN**, not false/no-owner.
- **Reconstructibility:** confirm expected full commit object in canonical Git store **and** on GitHub; is the exact branch on origin? If not, is HEAD reachable by a *named immutable* remote ref or archived GitHub commit? Check for local-only refs, unique stash/reflog-only commits, object/branch mismatches and worktree-relative evidence. **No fetch, push, checkout, branch creation/deletion, GC or reset.** Record a *future only* reconstruction command with an exact SHA and proposed new path; do not execute it.
- **Exact allocation:** read-only per-root allocated size, identify embedded bulky ignored or binary evidence if any. Do not follow symlinks or traverse external mounts. In case of surprising cache/evidence or modified/untracked/ignored files, mark HOLD / NEEDS ARCHIVE, not candidate removal.
- **Per-path classification:** `PREFLIGHT-PASS / OWNER-DELETE-DECISION-PENDING`, `HOLD`, or `UNKNOWN`. A PASS means observations at one instant only; **it is NOT SAFE-TO-DELETE**. Any inconclusive ownership, project personal data, open process, mount ambiguity, ignored file needing retention, unpushed work or permission blockage => HOLD or UNKNOWN. Record evidence citations/log excerpts, not ungrounded claims.

**Permitted local operations:** bounded observational `hostname`, `findmnt`, `lsblk`, `df`, `du -x`, `stat`, `readlink`, `lsof` or `fuser` when usable, `find` with mount constraint, and **read-only** Git subcommands with `GIT_OPTIONAL_LOCKS=0`, plus GitHub read/publish of findings. Reading may still affect access times; do not claim literal zero disk writes. Write any temporary notes only outside audited roots and only if essential and allowed by local safe-work conventions. No script installation in CCF-Primary/Operations/Repository.git or Unity assets.

## 4. Forbidden actions — hard stops

**NO `git worktree remove` (even non-force), `rm`, `mv`, archive, compression, file deletion, `git clean`, `git prune`, `git gc`, `git repack`, `git fetch`, `git push`, checkout, reset, stash, branch/tag/ref deletion, permission changes, mount/remount, symlink mutation, Unity Editor/build, Unity lock acquisition or release, source/scene/asset/save/Prefs changes, merges or Windows build.** No space reclamation is possible under this packet.

No blanket scan of the 45 Files candidates, the remaining 6 November cohort, #802's Ops-lock copies, root T3, legacy Git, player builds, `CCF/Verification`, warm ZX20 Libraries or any other project. Read shared board metadata only as necessary for safe decisions.

## 5. Deliverables and handoff

**One isolated GitHub documentation branch based on current main**, with no production files modified. Publish only these exact new paths:

1. `Docs/Handoffs/CCF-Storage-Inventory-20261120/893-seven-files-preflight/CCF-893-SEVEN-FILES-LOCAL-READONLY-PREFLIGHT-RESULT.md`: host/mount proof, authority and live-ownership checks, per-path current state, evidence links, any blocked/unknown facts, and a ranked **owner decision list**. Clearly state `PRECHECK ONLY / ZERO DELETED / ZERO FREED`. Record actual provider/model and credentials/roles only at non-sensitive level.
2. `Docs/Handoffs/CCF-Storage-Inventory-20261120/893-seven-files-preflight/CCF-893-EXACT-SEVEN-PATH-PREFLIGHT-RESULTS.tsv`: exactly seven rows with source ID, exact absolute path, inventory and observed full HEAD/branch, mount UUID, allocated bytes, staged/unstaged/untracked counts, ignored-file evidence, local-owner/FD/process status, remote recoverability, exit code / preflight state, reasons, restoration reference and next owner gate.
3. Durable GitHub result handoff or a clearly designated result section in the result Markdown; read back exact files, compare docs branch to base, post direct report links to [#893](https://github.com/jermur788/CCF/issues/893) and [#425](https://github.com/jermur788/CCF/issues/425). Notify #802 **only if an actual overlap/custody condition is discovered**. Mark #893 `HANDED OFF / READY FOR OWNER DECISION` only when evidence complete; otherwise `PARTIAL / HELD`. The single local research worker must release any actual inspection claim.

**No full test suite required** — this is seven old worktree checks, not Unity gameplay qualification. GitHub/host observational verification is the whole task. Do not claim a completed safe-deletion run or revise #383/#385 approval scope.

## 6. Next owner gate

After evidence is published and Manager has checked each row, request **separate explicit owner approval for deletion of exact named PASS paths, if any**. If granted later, #425 coordinates one authorised native Git metadata writer and checks any overlap with #802; the separate execution packet can use only individually approved **non-force** `git worktree remove <exact path>` after an immediate fresh final preflight. The owner has **not** approved that future phase.

## 7. Complete paste-ready local-worker dispatch (not auto-started)

> **CCF #893 — SEVEN EXACT FILES WORKTREES, READ-ONLY LOCAL PREFLIGHT.** Owner approved the *inspection only* 2026-11-20 22:34:19 UTC. Read the complete DIRECT GitHub handoff first: **https://github.com/jermur788/CCF/blob/docs/ccf-893-seven-files-worktree-readonly-preflight-20261120-v1/Docs/Handoffs/CCF-Storage-Inventory-20261120/CCF-893-OWNER-APPROVED-EXACT-SEVEN-FILES-READONLY-LOCAL-PREFLIGHT-PACKET-20261120.md**. Difficulty MEDIUM. Provider: local T3 Code/OpenCode DeepSeek V4.1 Flash or authorised GPT-6.1 Sol/Codex, one read-only worker on verified `jer-laptop`; the one GitHub docs result writer owns only the two `893-seven-files-preflight` output paths on its own new docs branch from main. Scope EXACT IDs `0128,0129,0131,0132,0102,0103,0104` under `/media/jer/Files/CCF/Operations/`; no other dirs. Start by confirming Files UUID, current GitHub active-tasks, real physical Ops board/ownership and #802, and read each exact Git worktree's branch/HEAD, full tracked/untracked/ignored state, active process/cwd/FDs, evidence and remote commit/ref reconstruction. Preserve owner playtests, separate #891 Unity assets and #890 code. On any path ambiguous/dirty/active/contains unique evidence, HOLD and do not touch. **NO removal, deletion, move, archive, Git worktree operation, checkout/fetch/clean/GC, branch reset, Unity, source or save writes, and no main merge.** Publish and read back exact seven-row TSV + concise Markdown evidence, comment on #893 and #425, release the read-only claim, and report `PREFLIGHT PASS / HOLD / UNKNOWN` per path. Never say safe-to-delete or freed space. User action: review the results for a *separate* later exact-path deletion approval; none is granted now.
