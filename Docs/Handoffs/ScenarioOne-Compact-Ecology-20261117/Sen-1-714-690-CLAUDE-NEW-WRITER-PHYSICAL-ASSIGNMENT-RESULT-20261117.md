# Sen-1 #714 — Claude new-writer physical assignment result (assignment only)

**RESULT: PASS. One writer is assigned to the A0 row. In-lock lint passed. The unmodified post-assignment preflight returned READY / exit 0 with no warnings on the A0 lane. #690 stays SOURCE START HELD / NOT RUNNING.** 2026-11-17.

This is a physical Ops custody result only. It grants no source, merge, test, Unity, save or Windows permission. Session `8b161722` may start only after a separate Manager #700 **[APPROVED TO START]** that names it. The later source review must be **non-Claude / nonauthor**.

## Actors and authority
- **Physical custodian actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, Linux host `jer-laptop`, OS user `jer`, session `68e4e51d-5147-4563-bebf-6298a5426f35`.
  - It is a different session from source candidate `8b161722`, revoked `fe9ea65b`, refusing/transfer `cc5a3b84`, and earlier #714 custodian `92e52eeb`.
  - It is not a #690 source candidate.
- **Named custodian:** USER `jermur788`.
- **Registration:** [#714 comment 6964094822](https://github.com/jermur788/CCF/issues/714#issuecomment-6964094822), created 2026-11-17T23:29:58Z. That was before the lock was taken at 23:31:13.790Z and before any write.
- **Packet:** [Manager #700 assignment-only packet](https://github.com/jermur788/CCF/blob/fa297f0073209f1f10af99879227f3321763d76e/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-700-714-NAMED-CLAUDE-A0-ASSIGNMENT-ONLY-PACKET-20261117.md) at `fa297f00`. Copy sha256 `22000657…`.
- **Assigned writer:** Anthropic Claude Opus 5.5, Claude Code in T3 Code, `jer-laptop`, session **`8b161722-5c96-4e4f-aa9b-1cd9df4c0dee`**, per its [READINESS-ONLY receipt](https://github.com/jermur788/CCF/issues/690#issuecomment-6963560917).

## Custody expiry: owner decision, with a disclosed conflict
**New expiry: `2026-11-19T00:00:00Z`.** It is ADVISORY / MANUALLY SUPERVISED — NOT ENFORCED BY TOOL. USER jermur788 is the personally supervising custodian.

How the expiry was chosen:
1. In this session at about 23:28Z, I asked the owner directly to choose a new UTC cutoff and confirm supervision. They chose 2026-11-19T00:00Z and confirmed they will supervise it.
2. I then found [Manager #700 comment 6964051383](https://github.com/jermur788/CCF/issues/714#issuecomment-6964051383), posted at 23:27:41Z. It records the owner approving **2026-11-18T05:00:00Z**.
3. I put the conflict back to the owner. They explicitly chose **2026-11-19T00:00Z (the later decision) as superseding** 05:00Z.

The row records both values and the supersession. **Manager #700 must note this change** before issuing any grant. If Manager does not accept the later expiry, the custodian should re-edit the row under the flock or release at or before 2026-11-18T05:00Z. No timer, monitor or automation was configured.

## Pre-write checks (registration pins, re-verified under the lock)
| Check | Observed |
|---|---|
| origin/main at registration | `1284d9d450b0ab8501a2985331e6d9f13c0932b0` |
| Board pre-write hash | `2a8dbdca8886f80283b50338f3cd3212adaf4449aa61ad1fb6f5fe9323a3ce9b` (= #714 result, = registration) |
| Lanes pre-write hash | `75df505feacfab8a6e06f331b2c4a7ef57d16deb0ed898b6f844b2a926ff3627` (= #714 result, = registration) |
| Ops scripts | `lane-status` `e7102a83…173f`, `_ccf.py` `ab4343ef…de001`, `packet-lint` `a776ec42…3e43`, `deployment.json` `8c66b290…7b18`; same bytes as #714, unchanged before and after |
| `_ccf.board_read` / `board_conflicts` | 38 rows, 22 lanes, conflicts `[]` |
| A0 row | State `BLOCKED`; Owner begins `UNASSIGNED / NOT RUNNING`; Lane `Sen1SaveA0ZX20NonUnity707`; Branch `integration/sen-1-d073-shared-save-a0-20261117-v1`; HEAD `55863f1b…`; that lane and branch each appear in exactly one row; lane registered once, `unity:false` |
| Candidate already on board? | No (0 rows mention `8b161722` before the write) |
| A0 worktree `/media/jer/ZX20/CCF-Sen1-690-A0-20261117` | attached to the A0 branch at `55863f1b7719011bba16b3f3ab377e379c0b988a`; 0 porcelain entries including untracked files; no upstream |
| A0 branch checkouts across both stores | exactly 1 (the A0 worktree) |
| D-073 checkout `CCF-S1-ResearchCopy-V1-Qual` | `55863f1b`, clean |
| Donor checkout `CCF-Scen1-SharedSave-T01-20261115` | `209a08e9f220841aaec81385d4cb7b437fa5089f`, clean |
| Unity state dir | only `guard` and `history.jsonl` (no active lock record) |
| Live claims | #690 open, SOURCE HELD, `8b161722` READINESS ONLY, no RUNNING; #714 open; no other source writer on the board or in main `active-tasks.md` |

## Locked transaction
Command: `flock -x -n -E 75 /media/jer/ZX20/CCF-Ops/board ./op.sh EVDIR`. `op.sh` takes Git snapshots and runs the gates, then runs `python3 -B assign714.py`. The transaction script follows the #712 pattern: hash-guarded, backups taken, candidate validated, `os.replace`, readback.

- **Lock:** board directory inode 1399769, held **23:31:13.790Z → 23:31:23.232Z**.
  - Released and independently re-acquired free with `flock -x -n … true` at 23:31:23.238Z.
  - The `flock` exit code was 0.
- **Rehearsal:** a dry run (`DRY=1`, stopping before writes, without the lock) was run at 23:30:59Z on the same pinned bytes. Board and lanes were unchanged afterwards.
- **Candidate validation before writing:**
  - `board_read`: 38 rows, conflicts `[]`.
  - `packet-lint --board <candidate>`: `PASS … (38 live tasks)`, exit 0.

### Exact changes
**Board `board/active.md`.** Exactly one line changed: line 114, row `CCF-690-SEN1-SAVE-A0`. The script verified that every other row is identical. Fields changed:
- **Owner:** now `PROSPECTIVE SOURCE WRITER ASSIGNED, SOURCE START HELD / NOT RUNNING: Anthropic Claude Opus 5.5 (claude-opus-5-5), Claude Code in T3 Code, jer-laptop, session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee (READINESS-ONLY receipt …6963560917); assigned 2026-11-17 by #714 assignment custodian USER jermur788 (actor session 68e4e51d) per Manager #700 packet fa297f00. Former writer fe9ea65b REVOKED AND RELEASED via #713; its void approval does not transfer`.
- **Harness / Model / Quota source:** `T3 Code / Claude Code` / `Claude Opus 5.5 (claude-opus-5-5)` / `Claude (owner-selected; Sol/Codex unavailable)`.
- **Locks:** two exact substring replacements.
  - `DETACHED at 55863f1b; A0 branch NOT created` became `A0 branch integration/sen-1-d073-shared-save-a0-20261117-v1 created by #714 at 55863f1b and attached, clean, local only, no upstream`. This fixes stale text O-1 from the earlier #714 result.
  - The old "NO writer … former advisory expiry 2026-11-18T00:00:00Z lapsed …" clause became: sole prospective writer `8b161722` assigned under the flock; NO active source lease and SOURCE START HELD until the Manager #700 agent-specific APPROVED TO START; advisory expiry `2026-11-19T00:00:00Z` ADVISORY / MANUALLY SUPERVISED - NOT ENFORCED BY TOOL, custodian USER jermur788 (owner-approved about 23:29Z, superseding the Manager-recorded 2026-11-18T05:00:00Z); old 2026-11-18T00:00Z expiry released.
- **Open gate / Dependency / Next action:** Manager #700 review, then a separate grant naming `8b161722`. After that, RUNNING on #690, a fresh unmodified preflight READY, and an expiry recheck come before any merge. The later source review is non-Claude / nonauthor. Dependency is prefixed with `#714 assignment result; `.
- **Unchanged:** State (`BLOCKED`), Lane, Branch, HEAD, Subsystem, Unity required? (`No`), queue position and tier.

**`lanes.json`.** Exactly one lane changed, `Sen1SaveA0ZX20NonUnity707`, and only its `purpose` field. The lane set, `path`, `unity:false` and all non-lane keys are identical. The file uses the round-trip `json.dumps(indent=2)+'\n'` format, which was verified byte-exact before the edit. Two substring replacements:
- `A0 branch … NOT created by #707.` became `… not created by #707; created and attached at 55863f1b by #714 (local only).`
- `expiry 2026-11-18T00:00:00Z ADVISORY` became `prospective writer session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee (SOURCE START HELD) assigned by #714; old 2026-11-18T00:00:00Z expiry released; expiry 2026-11-19T00:00:00Z ADVISORY`. This fixes stale text O-2.

Neither the packet nor the deployed Ops docs define a dedicated lanes.json writer. The edit used the same flocked, hash-guarded transaction pattern as the #712 Phase 2 lanes edit.

### Hashes
| File | Before | After |
|---|---|---|
| `board/active.md` | `2a8dbdca8886f80283b50338f3cd3212adaf4449aa61ad1fb6f5fe9323a3ce9b` | `0c69a9aefdaefc8a3554730671397b2bc6c75caa39eaf945ee94e6d629760f51` |
| `lanes.json` | `75df505feacfab8a6e06f331b2c4a7ef57d16deb0ed898b6f844b2a926ff3627` | `e95e58785f9047d42e5720f2aff21acb1d6f294c8e7904eb93ae9dfffcb4ca22` |

### Readback and in-lock lint (inside the lock)
```
post board 0c69a9aefdaefc8a3554730671397b2bc6c75caa39eaf945ee94e6d629760f51
post lanes e95e58785f9047d42e5720f2aff21acb1d6f294c8e7904eb93ae9dfffcb4ca22
board lines changed [113] expected [113]            (0-based index; file line 114)
lanes changed ['Sen1SaveA0ZX20NonUnity707'] set equal True only purpose differs True non-lane keys equal True
readback rows 38 conflicts [] rows w/ session ['CCF-690-SEN1-SAVE-A0'] other rows identical True
== IN-LOCK LINT 2026-11-17T23:31:18.676436Z
PASS: board schema and duplicate checks (38 live tasks)
in-lock lint exit=0 2026-11-17T23:31:18.805024Z
txn_end 2026-11-17T23:31:18.805047Z RESULT=PASS
```
The lint invocation was `cd /media/jer/ZX20/CCF-Ops/scripts && python3 -B ./packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md`. That is the normal invocation, without `-I`, so it fixes the #714 lint gap.

**Single-writer check:** exactly one board row mentions `8b161722`. `board_conflicts` returns `[]`. The lane and branch are each assigned once. The A0 branch is checked out in exactly one worktree across both stores.

**Git snapshots under the lock, before vs after:** canonical refs, legacy refs, canonical worktrees, legacy worktrees and the three checkouts (A0, D-073, donor) are all **unchanged**. No Git operation was performed.

## Unmodified post-assignment preflight
Command:
```
cd /media/jer/ZX20/CCF-Ops/scripts && python3 -B ./lane-status --preflight --lane Sen1SaveA0ZX20NonUnity707 --branch integration/sen-1-d073-shared-save-a0-20261117-v1
```
It ran 23:31:29Z → 23:35:54Z, after the lock was released. The board, lanes, deployment and script hashes were identical before and after the run.
```
Storage: READY / PASS | free=53086126080 bytes (49.44 GiB) | threshold=16106127360 bytes (15.00 GiB)
Sen1SaveA0ZX20NonUnity707 | /media/jer/ZX20/CCF-Sen1-690-A0-20261117 | device=/dev/sdd2 | ZX20: YES | unity=False exists=True registered=True
  branch=integration/sen-1-d073-shared-save-a0-20261117-v1 HEAD=55863f1b7719011bba16b3f3ab377e379c0b988a dirty=False ahead/behind=None Library=False disk~=2.0G owner=PROSPECTIVE SOURCE WRITER ASSIGNED, SOURCE START HELD / NOT RUNNING: Anthropic Claude Opus 5.5 (claude-opus-5-5), … session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee … (CCF-690-SEN1-SAVE-A0)
  warnings: none
READY
exit=0
```
The full output is `preflight-post-assignment.txt` (sha256 `ba28312e80327d5fb21fe59fe7adc50899112289f2092ccaa7c551f6404dd4c9`).
- The A0 lane has **no warnings**. Other lanes show their own existing warnings in the inventory, but `--preflight` readiness is computed only from the target lane.
- This READY was observed **after** the assignment. The pre-assignment READY from the earlier #714 run was not reused.

**After the preflight:** the A0 worktree still has 0 porcelain entries, and `git ls-remote origin` finds 0 matches for the A0 branch, so it remains local only.

## Not done
- Did not post RUNNING on #690 or change #690.
- No source edits, merge, commit to the A0 branch, push of the A0 branch, branch switch, tests, Unity, Windows or save actions.
- Did not edit any other board row or lane.
- No `main` or `active-tasks.md` edits.
- Dashboard and Drive sync were not verified.
- The user's two pre-existing modifications in `/media/jer/Files/CCF-Primary` (`AGENTS.md` and `Docs/Operations/StorageArchitecture.md`) were left untouched.

## Next steps
1. **Manager #700** reviews this physical evidence and must explicitly accept or reject the owner's later expiry, 2026-11-19T00:00Z, which supersedes 05:00Z.
2. If accepted, Manager #700 issues a separate **[APPROVED TO START]** that names session `8b161722-5c96-4e4f-aa9b-1cd9df4c0dee`.
3. That session then:
   - posts RUNNING on #690;
   - rechecks the expiry;
   - verifies the A0 branch is at `55863f1b` and clean;
   - gets a fresh unmodified preflight READY / exit 0;
   - starts only the bounded A0 save/Cadence merge: 17 exact files, no Compact B/C, Unity, Windows or `main`.
4. The source review afterwards must be non-Claude / nonauthor.
5. USER jermur788 supervises the expiry: release or document a lawful renewal at or before 2026-11-19T00:00:00Z.

## Evidence
`/media/jer/Files/CCF/Verification/Sen1-714-690-ClaudeWriterAssignment-20261117/`:
- `packet-700-714.md`
- `reg.md` and `reg-comment.txt`
- `board-active.at-registration.md` and `lanes.at-registration.json`
- `assign714.py` and `op.sh` (the scripts)
- `op.log` (the full locked transcript)
- `active.md.{before,candidate,after}.md` and `lanes.json.{before,after}.json`
- `board.diff` and `lanes.diff`
- `refs-*-{before,after}.txt`, `worktrees-*-{before,after}.txt` and `checkouts-{before,after}.txt`
- `preflight-{pre,post}-hashes.txt` and `preflight-post-assignment.txt`
- `RESULT.md` (this file)
