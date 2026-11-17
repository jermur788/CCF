# Sen-1 #714 — A0 one-ref bootstrap, mandatory preflight and replacement custody result

**RESULT: PASS (ONE-REF BOOTSTRAP + PRE-ASSIGNMENT PREFLIGHT READY / exit 0) / BLOCKED (WRITER ASSIGNMENT: no replacement session nominated) — A0 row unchanged BLOCKED / UNASSIGNED / NOT RUNNING; one prepared, UNASSIGNED A0 branch; #690 SOURCE HELD.** 2026-11-17.

This is a physical Ops custody result only. It grants no source, merge, test, Unity, save or Windows permission and does not transfer the suspended `fe9ea65b` approval.

## Registration (before any write)
- **Custodian (actual):** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, Linux host `jer-laptop`, OS user `jer`, session `92e52eeb-4e52-43e4-8099-ca022e78c809`. This is a new session: not #713's `cc5a3b84`, and not former writer `fe9ea65b`. It acted as the **#714 physical custodian only** and is not a #690 source candidate.
- **Registration comment:** [#714 RUNNING](https://github.com/jermur788/CCF/issues/714#issuecomment-6963175702), created 2026-11-17T22:44:18Z. The lock was acquired at 22:44:57Z, so registration came before any write. The comment header says "22:46Z"; that was an estimate, and GitHub's `created_at` is authoritative.
- **Starting refs:** origin `main` was `a9f271a9c861fc00444d93a852b46494cca13a33`. The packet was read in full at `docs/sen-1-700-manager-intake-20261117-v1` @ `c7806fda`, packet blob `f76ff905`. Copy and sha256 `e0562583…` are in the evidence folder.
- **Live claims refreshed:**
  - #690: open, SOURCE HELD.
  - #712 and #713: closed.
  - #714: open, with no earlier claim comment.
  - `main:Docs/Project/coordination/active-tasks.md` carries the Manager #700 22:35Z entry that routes #714.
  - No RUNNING source writer.

## Deployed Ops rule check (packet step 3 precondition)
The deployed `README.md`, `AGENTS.md` and `CLAUDE.md` in `/media/jer/ZX20/CCF-Ops` have no rule against creating a branch ref before assignment in an existing registered lane. The relevant rules are:
- no per-task *worktree* creation (none was created);
- do not switch *another owner's* checkout (the row was UNASSIGNED; the custodian is USER jermur788, who dispatched this);
- do not bypass preflight (it was run unmodified);
- a branch must not be writable in both stores (checked).

Neither store has `core.hooksPath` or a non-sample hook. **Verdict: permitted.** No further owner decision is needed on this point.

## Locked physical operation
Command: `flock -x -n -E 75 /media/jer/ZX20/CCF-Ops/board ./op714.sh EVDIR`. The common board-directory lock (inode 1399769) was held from **22:44:57.427Z to 22:45:16.995Z**, and was free again at 22:50:13Z.

**Phase 0 (locked readback). All gates PASS:**
- Board sha256 `2a8dbdca8886f80283b50338f3cd3212adaf4449aa61ad1fb6f5fe9323a3ce9b` and lanes.json `75df505feacfab8a6e06f331b2c4a7ef57d16deb0ed898b6f844b2a926ff3627` matched the registration pins. Multi #712's changes are present and preserved.
- `_ccf.board_read`: 38 rows, 22 lanes, `board_conflicts` = [].
- Row `CCF-690-SEN1-SAVE-A0`: State `BLOCKED`, Owner begins `UNASSIGNED / NOT RUNNING`, Lane `Sen1SaveA0ZX20NonUnity707`, Branch `integration/sen-1-d073-shared-save-a0-20261117-v1`, HEAD `55863f1b…`. The lane appears in exactly one row and is registered once with `unity:false`. There is no active lease.
- The A0 ref was absent from the canonical store, the legacy store and their remote-tracking refs, and was checked out in no worktree.
- Worktree `/media/jer/ZX20/CCF-Sen1-690-A0-20261117` was DETACHED at D-073 `55863f1b7719011bba16b3f3ab377e379c0b988a`, with 0 porcelain entries including untracked files.
- The D-073 checkout `CCF-S1-ResearchCopy-V1-Qual` (clean, `55863f1b`) and the donor checkout `CCF-Scen1-SharedSave-T01-20261115` (clean, `209a08e9`) were as expected.

**Lint gap:** the in-lock board lint invocation failed to *start*. I ran `packet-lint` with `python3 -I`, and isolated mode drops the script directory, so `_ccf` could not be imported. The script did not gate on that line. The official lint was rerun correctly (`python3 -B`) right after release, at 22:45:25Z, on the same board bytes `2a8dbdca…`: **`PASS: board schema and duplicate checks (38 live tasks)`, exit 0**. Since the board was never written in this task, that lint covers the unchanged locked board.

**Phase 1: the only write, run once:**
```
git -C /media/jer/ZX20/CCF-Sen1-690-A0-20261117 switch -c integration/sen-1-d073-shared-save-a0-20261117-v1 55863f1b7719011bba16b3f3ab377e379c0b988a
→ Switched to a new branch 'integration/sen-1-d073-shared-save-a0-20261117-v1'   (exit 0)
```
Post-write checks, all PASS:
- **Canonical refs diff:** exactly one added line, `refs/heads/integration/sen-1-d073-shared-save-a0-20261117-v1 55863f1b7719011bba16b3f3ab377e379c0b988a`.
- **Canonical worktree list diff:** exactly the A0 entry, `detached` → `branch refs/heads/integration/sen-1-d073-shared-save-a0-20261117-v1`.
- **Legacy store:** refs and worktrees unchanged.
- **A0 worktree:** clean (0 entries), HEAD `55863f1b`. The index entry hash `5250151e…` is unchanged.
- **Branch config:** no upstream. Reflog: `branch: Created from 55863f1b…`.
- **Other checkouts:** D-073, donor and Primary unchanged. Primary keeps the user's two modifications, `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`, untouched.
- **Ops files:** board, lanes.json, deployment.json, lane-status, `_ccf.py` and packet-lint hashes unchanged.

No merge, cherry-pick, commit, file edit, test, push or donor mutation happened. The A0 branch is **local only**; `git ls-remote origin` finds 0 matches.

## Mandatory preflight (unmodified, after bootstrap, before any assignment)
Command: `cd /media/jer/ZX20/CCF-Ops/scripts && python3 -B ./lane-status --preflight --lane Sen1SaveA0ZX20NonUnity707 --branch integration/sen-1-d073-shared-save-a0-20261117-v1`. The tools were unmodified: `lane-status` `e7102a83…173f` and `_ccf.py` `ab4343ef…de001`, the same bytes as #713 and pinned in deployment.json. It ran 22:45:32Z → 22:49:53Z, and board and lanes hashes were identical before and after.
```
Storage: READY / PASS | free=53086126080 bytes (49.44 GiB) | threshold=16106127360 bytes (15.00 GiB)
Sen1SaveA0ZX20NonUnity707 | /media/jer/ZX20/CCF-Sen1-690-A0-20261117 | device=/dev/sdd2 | ZX20: YES | unity=False exists=True registered=True
  branch=integration/sen-1-d073-shared-save-a0-20261117-v1 HEAD=55863f1b7719011bba16b3f3ab377e379c0b988a dirty=False ahead/behind=None Library=False disk~=2.0G owner=UNASSIGNED / NOT RUNNING. … (CCF-690-SEN1-SAVE-A0)
  warnings: none
READY
exit=0
```
The full output is `preflight-post-bootstrap.txt` (sha256 `54a54266…e894`). The `board branch/HEAD differs` blocker from #713 is gone. Other lanes still show their own existing warnings in the inventory, such as dirty U1/U3/Q10, but `--preflight` readiness is computed only from the target lane, which has **no warnings**. This READY was taken **before** any writer assignment, against the UNASSIGNED row. It is current as of 22:49:53Z and is not the old #707 READY.

## Writer assignment (packet steps 5–6): BLOCKED, not performed
At 22:46–22:50Z, custodian USER jermur788 was asked in-session whether to assign a replacement now. They chose **"Leave UNASSIGNED"**: no replacement source session exists yet, so there is no full provider/model/session to verify and no new supervised UTC cutoff. I therefore:
- made **no board write** (row unchanged, board `2a8dbdca…`);
- made **no lanes.json write** (`75df505f…`);
- set **no expiry**. The 2026-11-18T00:00:00Z deadline stays RELEASED and was not reactivated;
- did not assign myself.

**Orphan-prepared branch warning:** exactly one A0 branch now exists, `integration/sen-1-d073-shared-save-a0-20261117-v1` @ `55863f1b`, local in the canonical store and attached to the clean ZX20 worktree. It is **UNASSIGNED**. No actor may merge into it, edit it, commit to it, push it or switch it until:
1. a real replacement is assigned under the board flock with a new custodian-supervised expiry;
2. Manager #700 issues a new agent-specific [APPROVED TO START];
3. that agent registers [RUNNING A0 SOURCE] and gets a fresh preflight READY/exit 0.

## Observations for the next assignment (not changed here)
- **O-1:** the A0 row's Subsystem/Locks prose (column 11) still says "DETACHED at 55863f1b; A0 branch NOT created". That is now stale; the branch is created and attached, and the Branch/HEAD columns are correct. #714 had no authority for a non-assignment row edit. The future one-row assignment edit should correct this text.
- **O-2:** the `lanes.json` purpose text for `Sen1SaveA0ZX20NonUnity707` still cites "expiry 2026-11-18T00:00:00Z". That expiry was released in #713. The packet says no lanes.json change while the registration stays valid, so I left it. The custodian or Manager should decide whether the next assignment refreshes it.
- **O-3:** the old #707 note said the source agent would create the branch as its first source action. Under the #714 procedure the branch now exists, so the replacement's first action becomes: verify the branch is at `55863f1b` and clean, then get preflight READY.

## Next steps
1. Manager #700 reviews this physical evidence. #714 is handed off OPEN.
2. Owner nominates a real replacement session (recommended: GPT-6.2 Sol/Codex High) and personally sets a new supervised UTC cutoff.
3. A registered custodian assigns exactly that session in the A0 row under `flock -x -n /media/jer/ZX20/CCF-Ops/board`, keeps it ADVISORY / MANUALLY SUPERVISED, and reruns lint and preflight. Post-assignment, the row's Branch/HEAD still match, so READY is expected but must be observed.
4. Manager #700 issues a new agent-specific conditional [APPROVED TO START].
5. The source agent registers and gets preflight READY before any merge.

There is no main merge, Unity or Windows step. Dashboard and Drive sync were not verified.

## Evidence
`/media/jer/Files/CCF/Verification/Sen1-714-690-OneRefBootstrap-20261117/`:
- `op714.sh` (sha256 `e1c903c1…`) and `op714.log` (`96944048…`);
- `refs-*-{before,after}.txt`, `refs-CCF.diff`, `worktrees-*`, `worktrees-CCF.diff`, `checkouts-*`, `ops-hashes-*`, `a0-index-*`;
- `board-active.at-registration.md`, `lanes.at-registration.json`;
- `lint-post-bootstrap.txt`, `preflight-{pre,post}-hashes.txt`, `preflight-post-bootstrap.txt`, `final-state.txt`;
- `packet-714.md`.
