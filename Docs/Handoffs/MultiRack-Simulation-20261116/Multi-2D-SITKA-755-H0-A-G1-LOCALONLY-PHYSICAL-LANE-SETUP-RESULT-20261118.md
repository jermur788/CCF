# Multi #755 — H0-A local-only G1 physical lane setup: Phase 0 result (BLOCKED FOR MUTATION)

**18 November 2026 · worker: Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code harness inside T3 Code, native Linux host `jer-laptop` · Manager: #730 · claim: [#755](https://github.com/jermur788/CCF/issues/755).**
**Registered [RUNNING / G1 PHASE0 READ-ONLY]:** [comment 6985647744](https://github.com/jermur788/CCF/issues/755#issuecomment-6985647744), 2026-11-18T17:04:22Z, against fresh GitHub main `34185b8b71ba280c89091290345e537ef107bdf4` (unchanged through the end of this task).
**Branch:** `docs/multi-755-g1-localonly-lane-result-20261118-v1`, created from that HEAD through the GitHub API only (no local branch, checkout or worktree).

## 0. Outcome

**BLOCKED FOR MUTATION. Phase 1 was NOT entered.** Nothing under `/media/jer/ZX20/CCF-Ops`, `/media/jer/Files/CCF` or any Git store, hook, lane, worktree, `lanes.json` or board row was created, edited or removed.

| Item | Result |
| --- | --- |
| Explicit actor-specific Manager G1 physical-setup grant | **ABSENT.** The only #755 Manager comment ([6985415897](https://github.com/jermur788/CCF/issues/755#issuecomment-6985415897)), the #730 note ([6985417966](https://github.com/jermur788/CCF/issues/730#issuecomment-6985417966)) and the #753 verdict say "PHASE0 ONLY UNTIL ACTOR-SPECIFIC GRANT". Re-checked at 17:08Z: no new Manager comment. The owner's H0-A choice is not the grant. |
| Phase 0 physical Ops state | Read directly; **unchanged since #753** (`board/active.md` and `lanes.json` both last written 2026-11-18T12:52:39Z); no H0 collision; no lock held; no Unity process. |
| Proposed topology | **Fully rehearsed in RAM** (`/dev/shm`, not under `/media`): seeding, containment, two sparse worktrees and the read-back all work (§4). |
| **New blocker found** | The installed `lane-status --preflight` **cannot return READY for the proposed topology.** The packet's mandatory "`lane-status --preflight` passes / exit 0" read-back is unattainable as written (§3, F1). Needs a Manager decision before any grant. |
| Recommendation | **GO FOR A MANAGER G1 GRANT DECISION** once D1 (read-back substitute) is settled. The grant itself must be issued by #730; a draft is in §8. **NOT** a G2 permit consideration yet. |

## 1. Identity and authority

- **Worker:** Claude Sonnet 5.5 as dispatched by the owner (packet preferred Codex Sol High or Claude Opus 5.5). Same worker identity that wrote the #753 preflight. **The Manager has not appointed me physical custodian**; board rows name the user `jermur788` as custodian, acting through assigned sessions. I act here as a Phase 0 reporter only.
- **Host:** `jer-laptop`, Linux 7.0.0-42-generic x86_64, unprivileged local user (uid 1000), no privilege escalation used. ZX20 (exFAT) 50 GB free; Files (ext4) 35 GB free, 93% used; installed `lane-status` agrees on ZX20 (49.05 GiB free against a 15.00 GiB floor).
- **Scratch:** RAM-backed `/dev/shm/ccf755` only (nothing on root `/`); removed at the end.
- **Claim boundary:** this claim confers no Ops authority and I took none.

## 2. Phase 0 physical facts (read-only), compared with #753

| Check | Observed (2026-11-18 ~17:05-17:09Z) | Versus #753 |
| --- | --- | --- |
| Board and `lanes.json` | mtimes 12:52:39Z; no file in Ops newer than 12:53Z; `lanes.json` sha256 `c17903ddbf9157ce092cb091d350822f6b8574cfaad7ba6c79f1040350885331`; board sha256 begins `2b99552cc50026d8` | Unchanged |
| Board lint | installed `packet-lint --board` on the live board: **PASS, 42 live tasks** | Matches 42 rows |
| Unity lock | `state/unity` holds only `guard` (0 B) and `history.jsonl` (299,478 B, last event 2026-11-15); `/proc/locks` shows **no advisory lock** on the `state/unity/guard` or `board` inodes | Same |
| Processes | no Unity, `dotnet`, `lane-status`, `unity-lock` or `packet-lint` running | Same |
| Re-read lane HEADs and cleanliness (non-protected lanes only) | #674 `5aa7b6ef`, 8 dirty lines; #687 `ce7bfeb6` clean; #697 `719ccbf8` clean; #690/#707 `55863f1b` clean; #652 checkout `4c7acc05` clean; `CCF-NonUnity` `5010613f` clean | **All identical to #753** |
| Protected #710 / #725 | **Not re-inspected and not opened.** Their board/`lanes.json` rows are unchanged (custody to Nov 23 / Nov 27). | n/a |
| Proposed paths | `.../Operations/CCF-H0A-Year0Viewer-20261118` and `.../Verification/CCF-H0A-Year0Viewer-20261118` **do not exist**; no other `*H0A*` directory; parent is `drwxrwxr-x jer:jer` on ext4 with 28.4 M free inodes; neither `Operations` nor `Files/CCF` is inside a Git work tree | Free, as in #753 |
| Authorised board custodian | Rows record custodian **USER `jermur788`** (via assigned Claude/Codex sessions, custodian-only). No H0 row, lane or lock exists. | Same |
| Collision for this task's changes | **None.** Phase 1 would only append two lane entries and two rows; all 42 existing rows and all existing lane entries stay byte-identical (§5.3). | n/a |

**Not reconciled here (by instruction), for the authorised canonical/Ops editor:** canonical `active-tasks.md` still shows #710 to Nov 19 and #725 to Nov 20 while physical Ops says Nov 23 and Nov 27; the board's #732 row text still says #725 to Nov 20; #674 is dirty (8 untracked lines) with an advisory expiry of 2026-11-18T18:00Z (about 50 min after this observation; D-091 says that is not a reclaim trigger); #652's board HEAD `a9f56d63` is an ancestor of its live HEAD `4c7acc05`; three older board rows name lanes absent from `lanes.json`. I touched none of them.

## 3. Installed CCF-Ops tooling findings

I read `_ccf.py`, `lane-status`, `packet-lint` and the relevant parts of `unity-lock` directly, then ran `lane-status` and `packet-lint` only in the read-only forms described in §7.

| # | Finding | Evidence |
| --- | --- | --- |
| **F1** | **`lane-status --preflight` can never be READY for a Files-ext4, isolated-store lane.** Readiness requires `target['zx20']` (line 71) and a worktree whose Git common dir equals the canonical `Repository.git` (lines 32, 46, 59). An H0 lane on Files ext4 with its own no-remote store fails both. | Source read. **Empirical:** a config holding only the existing #697 lane (Files ext4, isolated store, clean) returns `warnings: worktree belongs to unexpected repository; not registered in configured repository`, `BLOCKED`, **exit 1**. The A0 lane's "preflight READY" in board rows is a ZX20 canonical-store lane and is not comparable. |
| **F2** | **No sanctioned board / `lanes.json` writer is installed.** `README.md` states there is no reviewed board writer; none of `_ccf.py`, `lane-status` or `packet-lint` contains a write or lock primitive. The only `flock` is `unity-lock`'s `state/unity/guard`. | grep and read |
| **F2a** | **Established custodian practice** (task-local, not installed): `flock -x -n /media/jer/ZX20/CCF-Ops/board`, assert expected pre-hashes of board and `lanes.json`, build candidates, validate with `_ccf.board_read`, `board_conflicts` and `config_read`, atomic temp-write + `fsync` + `os.replace`, hash read-back. Seen in `CCF-735-.../txn735.py` and `CCF-712-.../board_lanes_txn.py` under `/media/jer/Files/CCF/Verification`. The lock is advisory and cooperative. | Read; not run |
| **F3** | **D-091 / no expiry is representable by omission.** `lanes.json` entries carry only `lane`, `path`, `purpose`, `unity` (free text elsewhere); the board has no expiry column; `config_read` and `board_read` parse no dates. `unity-lock` requires an ETA but only for Unity-capable lanes, so it does not touch `unity:false` H0 lanes. #652's D-091 source change is approved but live install is HELD; the installed `unity-lock` is unchanged (2026-10-15). | Source read |
| **F4** | **Board constraints for new rows:** one `Lane` and one `Branch` per row, so **two rows** (Core, Viewer); no empty cell (`None`/`N/A`); `HEAD` must be a 40-hex SHA or `TBD`; `Unity required?` = `No`; state from the fixed set; duplicate lanes or branches are flagged. | `board_read`, `board_conflicts` |
| **F5** | **`config_read` accepts Files-tree `unity:false` lanes** (strict mode) and leaves `git_stores` untouched; the isolated store is therefore **not registered** and the tool performs **no dual-store branch-conflict check** for H0 branches (same situation as #674/#687/#697/#710/#725). | Source read |
| **F6** | **`deployment.json` stays valid.** It pins everything in `lanes.json` except the operational `/lanes` array and treats `board/active.md` as live operational state; adding H0 lane entries does not disturb `storage`, `repository` or `git_stores`. | `deployment.json` `live_state`, `configuration_hashes` |

## 4. Push-containment design: tested in a RAM replica

A throwaway replica (`/dev/shm`, bare store, two worktrees, a local bare "remote", a dead `HTTPS_PROXY`, no network) tested each control. **Nothing under `/media` was written.** Credential output was discarded; only helper-invocation traces were counted.

| Test | Result |
| --- | --- |
| Global config alone | `credential.https://github.com.helper = !/usr/bin/gh auth git-credential` is invoked (1 invocation). |
| **Unscoped blank `credential.helper=` in the store config** | **0 invocations**: the packet's recipe works against the URL-scoped global helper. |
| `core.hooksPath`, `credential.helper=`, `push.default=nothing` | Seen identically **from both worktrees** (shared store config); `remote` list empty; no upstream on either branch. |
| Plain `git push` | `fatal: No configured push destination.` |
| Push to a local bare repo with the `pre-push` hook (exit 1) | **Blocked**, remote ref list stays empty. |
| Same push with `--no-verify` | **Succeeds.** The hook is cooperative, not a security boundary. |
| **Explicit `https://github.com/...` URL push with only the hook** | Git **connects before it runs `pre-push`** (the attempt reached the proxy). So the hook alone does not prevent an attempted connection. |
| **`url.<non-existent local path>.pushInsteadOf` for `https://github.com/`, `http://github.com/`, `git@github.com:`, `ssh://git@github.com/`, `git://github.com/`** | **All five explicit-URL pushes were rewritten to the non-existent local path and failed locally** (no GitHub URL in the error). This closes the connect-before-hook gap for GitHub URLs. |
| Correction | My first rewrite test showed no rewrite because my own second `git config` call overwrote the first value; the repeat in a clean repo and with `--add` works. |
| Seeding: one-shot `git fetch --no-tags --depth=1 file:///media/jer/Files/CCF/Repository.git refs/remotes/origin/main:refs/heads/local/h0a-base` into the new store | **Exit 0, 21 s, store 965 MB**, base `34185b8b`, **no remote created**. Containment was applied **before** the fetch. |
| Two sparse worktrees (`--no-checkout`, `sparse-checkout set --cone Tools/MultiRackSitkaYear0Viewer`, then checkout) | **24 KB each**, three root files only (`AGENTS.md`, `CLAUDE.md`, `.gitignore`); **`Assets/` physically absent**; `status` clean; distinct branches; hooksPath/credential/push settings identical. `sparse-checkout` sets `extensions.worktreeConfig=true` in the store config (harmless side effect, not a security control). |

**Residual risk (cannot be closed by Git config):** `--no-verify`, `git -c core.hooksPath=...`, editing the store config, `gh api`/`gh pr`/`curl` with the host's `gh` token (scope `repo`), a loaded `SSH_AUTH_SOCK` agent (set in this session; its contents were not queried), and copy/paste. Containment is therefore **structural plus procedural**: writer task packets must forbid pushes, `gh`/`curl` uploads and `--no-verify`/`-c` overrides, and Manager/reviewers must check GitHub for any new viewer path and read back `git config --local --list` before and after each writer session. Making the store `config` read-only after setup is an untested optional extra and is not claimed.

## 5. Exact prospective Phase 1 change set (PROPOSAL; none of it exists)

### 5.1 Paths (all on Files ext4; root `/` unused)

| Path | Purpose |
| --- | --- |
| `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` (`R`) | isolated lane root |
| `R/git/CCF-H0A-Year0Viewer.git` | bare, **remote-less** store; base ref `local/h0a-base` (shallow, depth 1) |
| `R/hooks/pre-push` | exits 1; mode 0555 |
| `R/ccf-h0a-core` | Core worktree, branch `feature/multi-h0a-core-year0-admission-20261118-v1` |
| `R/ccf-h0a-viewer` | Viewer worktree, branch `feature/multi-h0a-a-year0-viewer-20261118-v1` |
| `R/artifacts/` | packager output, **outside any git tree, never committed** |
| `R/.no-push-sink/` | **must never exist** (it is the `pushInsteadOf` blackhole target) |
| `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/custody/` | evidence: pre/post hashes, candidates, read-back, receipt |
| `ZX20/CCF-Ops/lanes.json` | **append two entries only** (`MultiH0ACore`, `MultiH0AViewer`; `unity:false`; `purpose` states "NO EXPIRY (D-091)", "NOT RUNNING", "no source/claim granted") |
| `ZX20/CCF-Ops/board/active.md` | **append two rows only**: `CCF-755-H0A-CORE-LANE`, `CCF-755-H0A-VIEWER-LANE` (State `READY`/reserved-not-running, `Unity required?` No, `HEAD` = the pinned base SHA, `Locks` naming only that lane's paths, no date) |

### 5.2 Ordered steps (rehearsed with the same commands under `/dev/shm`)

1. Re-resolve main and the Manager-named base SHA; confirm the paths above are still absent; take sha256 of board and `lanes.json`; **hold `flock -x -n` on `ZX20/CCF-Ops/board` through step 9**; refuse if any lock exists.
2. Create `R`, `R/git`, `R/hooks`, `R/artifacts` and the `custody` evidence directory.
3. `git init --bare` the store.
4. Write `R/hooks/pre-push` and `chmod 555`.
5. **Apply containment before any fetch:** `core.hooksPath=R/hooks`, `credential.helper=` (blank), `push.default=nothing`, and five `url.file://R/.no-push-sink/.pushInsteadOf` values (the URL forms in §4).
6. Seed: one-shot `git fetch --no-tags --depth=1 file:///media/jer/Files/CCF/Repository.git refs/remotes/origin/main:refs/heads/local/h0a-base`; assert the ref equals the pinned base SHA; assert `git remote` is empty.
7. For Core and Viewer: `git worktree add --no-checkout -b <branch> <path> local/h0a-base`, `sparse-checkout set --cone Tools/MultiRackSitkaYear0Viewer`, `checkout`.
8. Containment read-back **from each worktree**: branch, HEAD, clean status, empty remotes, no upstream, hooksPath, blank credential helper, `push.default`, five rewrite rules, `Assets/` absent, an explicit-URL push attempt that fails locally. No H0 source is created.
9. Build candidate `lanes.json` and board in the evidence directory; assert every pre-existing row and lane is byte-identical; validate with `_ccf.config_read`, `board_read` and `board_conflicts`; assert the pre-hashes still hold; atomic replace (temp + `fsync` + `os.replace`); hash read-back.
10. Final checks: `packet-lint --board` expecting PASS (44 live tasks); installed `lane-status` for information (expect the F1 warnings and `BLOCKED` for the two H0 lanes); write a custody receipt; **hand the empty lanes to the Manager, not to a writer**.

### 5.3 Affected versus unaffected

**Affected:** the two new lane directories and their evidence directory, the two new lane entries, the two new board rows. **Unaffected (must stay byte-identical):** every other board row and lane entry, `storage`, `repository` and `git_stores`, `deployment.json`, `state/unity`, the canonical store (read-only source of the seed fetch), #674's dirty tree, #652, #687, #690/#707, #697, and the protected #710/#725 stores (not opened).

### 5.3a Footprint

Store **965 MB** (shallow), worktrees **~24 KB each**, evidence small: about 1 GB of the 35 GB free on Files. The fixture is **not** imported by the custodian (see D3).

### 5.4 Rollback

Restore board and `lanes.json` from the recorded pre-images (hash-verified, same lock); remove only the exact created paths, and only by the custodian after a Manager release (D-091: no silent cleanup or timer-based reclaim). Any removal command is shown to the Manager first.

## 6. What the base tree does and does not contain (read-only, canonical store)

- Base commit `34185b8b`, the #639 commit `e302f15b` and fixture blob `8bf81a43` (32,567 B) **all exist in the canonical store**; `refs/remotes/origin/main` points at `34185b8b`.
- Main holds 5,862 files (1,167 MiB uncompressed; `Tools/` is 435 files, 6.9 MiB) and **no** `Tools/MultiRackSitkaYear0Viewer`, **no** approved H0 spec files (they are on the #750 docs branch), **no** P1 lab and **no** P0 lab. The fixture exists only on the #639 branch.
- Consequence: a tree seeded from main contains no protected P0 or P1 content; the "before/after protected-path comparison" for a JS-only tree therefore has to be defined against paths **outside** the lane (D5).

## 7. What was and was not done

**Done (read-only):** read the complete packet and #755/#730/#753 threads; read physical Ops files; read installed scripts; `stat`, `/proc/locks`, `ps`; `git --no-optional-locks` HEAD/status on the non-protected lanes; ran installed **`packet-lint --board`** on the live board and installed **`lane-status`** on RAM-only configs (one holding two RAM replica lanes, one holding only the #697 lane: its `du` and Git metadata calls touched another owner's lane, metadata only, no write); ran `git fetch` **from** the canonical store into a RAM store (read-only upload-pack on the canonical side); built and discarded the replica in `/dev/shm`.
**Not done:** any Phase 1 step or any write under `/media`; any Ops, lock or board change; any read of #710/#725 contents or a local fixture copy; any application source, JS, HTML or test; any build, browser launch, Unity, Windows, Host, `dotnet`, `node` or network service; any GitHub write other than the #755/#730 status comments, the API-created branch and this one file (GitHub reads through `gh api` were used for the packet, threads and main HEAD); main edit; L4 waiver. A dead-proxy environment was used on every push test to make a real GitHub push impossible.

**NO APPLICATION SOURCE WRITTEN / NO TESTS OR BUILDS / NO UNITY / NO WINDOWS / NO HOST / NO PHASE 1 MUTATION / NO PROTECTED-SOURCE ACCESS / NO PUBLIC PUSH / NO MAIN INTEGRATION / NO L4 WAIVER (L4 FAIL38 UNWAIVED).**

## 8. Gates, open decisions and the grant the Manager would have to issue

| Gate | Status |
| --- | --- |
| Phase 0 read-only checks | **DONE.** No collision; physical state matches #753. |
| Actor-specific Manager G1 physical grant | **MISSING, BLOCKED FOR MUTATION.** |
| Packet read-back "lane-status --preflight exit 0" | **UNATTAINABLE** for this topology (F1) until D1 is decided. |
| G2 local non-release scope, source claims, frozen allowlists | **NOT GRANTED**; Core/A claims remain separate and later. |
| G3 / G4 / L4 | Not requested; **L4 FAIL38 UNWAIVED**. |

| ID | Decision for #730 / owner | My recommendation |
| --- | --- | --- |
| **D1** | Read-back substitute for the unattainable `lane-status --preflight`: (a) accept the §5.2 step 8-10 read-back (containment read-back + `config_read`/`board_read`/`board_conflicts` PASS + `packet-lint --board` PASS + custody receipt) and record `lane-status` BLOCKED as expected; or (b) first commission a separately reviewed Ops patch that registers isolated stores (a source change to Ops; not part of #755). | **(a)** |
| **D2** | Custodian identity for Phase 1 (named actor/session) and whether it is me or another session. | Manager's choice; name it in the grant. |
| **D3** | Fixture seed: the custodian does **not** import it. The Core G2 packet should copy blob `8bf81a43` from the canonical store read-only with size/SHA-256 assertions, which is a Manager-granted read of the canonical store. | Keep it out of Phase 1. |
| **D4** | Seed depth: shallow depth 1 (965 MB) versus full-history clone (~4 GB). | Shallow. |
| **D5** | Meaning of P0 "L0-L3+ / zero new protected hit" for a JS-only tree with no P0 content (before/after hashes of which paths, held by whom). | Manager/security to define before G2. |
| **D6** | Whether to add a `.no-push-sink` rewrite and a read-only store `config` as standard H0 containment. | Add the rewrite (tested); skip config read-only. |
| **D7** | Reconciliation of the stale #710/#725 dates, #732 row, #674 and #652 drift by the authorised editor (separate from H0). | Separate task. |
| **D8** | Public-source policy remains: new viewer code stays local; no consent exists. | Unchanged. |

**Draft grant for #730 to adapt (not issued by me):** "[MANAGER #730 — #755 PHASE 1 G1 PHYSICAL SETUP GRANT] Host `jer-laptop`; custodian `<named actor/session>`; permitted mutations: exactly the §5.1 paths and the two lane entries and two board rows, under `flock` on the live board with pre-hash assertions, D-091 no expiry; read-back substitute per D1; **no application source, no G2, no writer start, no protected #710/#725 access**; hand empty lanes back to #730."

**Proposed future claims, for later only:** Core writer claim (T02-T04; allowlist `core/year0-core.js`, `core/CONTRACT.md`, `fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json`, `build/assemble-year0-viewer.py`, `tests/core/**`) and Viewer writer claim (T05-T07; `viewer/shell.template.html`, `viewer/viewer.css`, `viewer/viewer-render.js`, `viewer/viewer-camera-hit.js`, `viewer/viewer-inspector.js`, `tests/viewer/**`), each with its own actor-specific G2 grant, fresh preflight and a real [RUNNING]; one packager outputs only to `R/artifacts/`.

Issue #755 stays OPEN for Manager #730 review. Dashboard/Drive sync: **NOT VERIFIED.**
