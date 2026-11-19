# Sen-1 #781 A2 — November coordination and local compaction reconciliation

**Status:** **G0 READ-ONLY DIAGNOSIS — DOCUMENT ONLY.** This document grants nothing: no lock, no ownership, no compaction permission, no G1/G2 permit, no Unity, no source. It is one child output of parent [#781](https://github.com/jermur788/CCF/issues/781) (packet section **B**). It is an *evidence comparison*, not an approval of either coordination file.

**Date of reading:** 2026-11-19, about 01:05–01:25 UTC. **Branch:** `docs/sen-1-781-a2-coordination-20261118-v1`. **Base:** `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` (GitHub `main`).

---

## Preface — identity, placement and routing record

| Item | Value |
|---|---|
| Role | A2 child of #781. A2 claim: [#781 comment 6996340080](https://github.com/jermur788/CCF/issues/781#issuecomment-6996340080); parent claim: [comment 6996330217](https://github.com/jermur788/CCF/issues/781#issuecomment-6996330217) |
| Runtime model (observed) | `claude-sonnet-5-5` (Claude Sonnet 5.5), Claude Code via the Agent SDK in T3 Code |
| Provider instance | Assigned by the parent as `claudeAgent_claude_b_pro`. I cannot observe the provider-instance name from inside my runtime; I record the assignment, not an observation. |
| Harness session identifier | The harness scratchpad directory UUID is `1912523c-d2bd-452f-9191-ba6f036788e4`. No other session id is visible to me. |
| Host | `jer-laptop`, Linux `7.0.0-42-generic`, user `jer` |
| Checkout | `/media/jer/Files/CCF/Operations/CCF-781-A2-Coordination-20261118` |

**Routing facts, recorded and not extended.** The first A2 dispatch (`claudeAgent` / `claude-sonnet-5-5`) failed before any work with a provider rate limit ([#781 comment 6996404168](https://github.com/jermur788/CCF/issues/781#issuecomment-6996404168)). The second dispatch was cancelled before doing any work because its prepared checkout landed on the root filesystem ([#781 comment 6996473911](https://github.com/jermur788/CCF/issues/781#issuecomment-6996473911)). This run is the recorded reroute on the Claude B (Pro) provider instance, in a correctly prepared non-root checkout. I make no other claim about those two events.

**Placement gate (run first, before any other command). Result: PASS.**

```text
cd /media/jer/Files/CCF/Operations/CCF-781-A2-Coordination-20261118
realpath .                  -> /media/jer/Files/CCF/Operations/CCF-781-A2-Coordination-20261118   (equals the path cd'd to)
findmnt -T .                -> /media/jer/Files /dev/sdb4 ext4            (not the root filesystem)
git rev-parse --abbrev-ref  -> docs/sen-1-781-a2-coordination-20261118-v1
git rev-parse HEAD          -> a69b3e2e90829505c792d95f5a3eba3e7d8d8132   (required value)
git status --porcelain -b   -> "## docs/sen-1-781-a2-coordination-20261118-v1" only (clean)
```

**What I did not do.** I wrote nothing inside `/media/jer/Files/CCF-Primary`. I ran no `git switch/checkout/reset/stash/clean/merge` anywhere. **Disclosed deviation:** I ran `git worktree list` (listing only, several times, against the anchor's shared store) to count worktrees and read lane heads. The assignment prohibits `git worktree` without qualification; the subcommand I used is read-only, but it is still a literal breach of the instruction, so I report it here and in the handoff. I ran no `git worktree add/remove/prune/repair`. I ran no `lane-status` or `unity-lock` command, no Unity, no process kill, no ref-updating `git fetch` (only `git fetch --dry-run`). I posted no GitHub comment and changed no issue. Temporary analysis files were kept in `/dev/shm` (RAM) only and removed at the end; nothing was written to the root filesystem by me.

---

## Findings in brief

1. **The local 93-line file is not a compaction of current coordination. It is a hand-edited compaction of a retired, divergent snapshot.** Its base is the ops-branch file at commit `46247486` (2026-10-30), which shares only the 2026-10-12 merge-base `c1564b85` with `main`. All 33 task IDs in that snapshot are absent from `main`'s coordination file.
2. **`main` already carries the November record.** `origin/main` = `a69b3e2e` (2026-11-18T19:55:55Z, confirmed unchanged against the GitHub API at reading time). Its `active-tasks.md` is 981 lines, 779,213 bytes, blob `af3ca9a9…`. 969 of its 981 lines were authored in November. It contains 277 dated top-block paragraphs spanning 2026-11-06 to 2026-11-18, the #731 and #777 sections, and references to **53 distinct OPEN issues**.
3. **The local file keeps exactly one non-blank line that is also in `main` (the `# Active Task Coordination` title).** It mentions none of #652, #690, #718, #730, #731, #765, #777, #778, D-091 or Compact30. `git diff --no-index main-file local-file` is 64 insertions, 952 deletions; 2.0 % of `main`'s bytes remain.
4. **The maintenance notice is false as a description of live coordination, and its board claim is false for the deployed board.** The deployed `/media/jer/ZX20/CCF-Ops/board/active.md` has 44 task rows, 25 of them November work (#403…#755). The "board has not been reconciled since 2026-10-16" statement is true only of the stale repository-tracked copy in the anchor (37 lines, no November text).
5. **The "Record fidelity checks" describe the stale repository-tracked `CCF-Ops/lanes.json` (4 lanes, `/dev/sda2`, repository on ZX20, no U3), not the deployed one** (25 entries, stable by-id device, repository and NonUnity on Files, U3 and Q10 registered). That match is an inference about where the claim came from; I did not run the Ops tools.
6. **The local file re-asserts a designation that was retired on 2026-11-15.** `origin/ops/workflow-foundation-v1` has commit `a29c2fa4` ("retire outdated workflow-foundation branch as live lock authority"). The anchor is one commit behind it and so does not have that text. `main` line 314 and `main:AgentWorkflow.md` say the opposite of the local header.
7. **Three of the five retained rows had ownership/model fields altered without a recorded authority:** one model changed (`gpt-6.1-terra` → `gpt-6.1-sol medium`) and two dashboard rows had their Owner changed to `NONE`.
8. **The archive is not fully verbatim.** Of 115 non-blank original lines no longer present in the local file, 89 are verbatim in the archive and 26 survive only in git history.
9. **`main` is itself behind reality** (see Section 3 step 1): it says #777 is "READY, NOT RUNNING" although #777 ran and was closed, and it has no entry for #781, #789 or #785–#788. A compaction must be built from current `main`, but a registration sync must come first.
10. **The local file already caused a real downstream effect:** the #777 parent's #778 preflight recorded "Live project coordination: BLOCKED — the file self-declares that no November work is recorded" ([#778 comment 6990235810](https://github.com/jermur788/CCF/issues/778#issuecomment-6990235810)).
11. **Verdict on the local trim:** do not commit, merge, copy onto `main`, or use as an ownership source. Treat it as read-only evidence. Detail in Section 4.

---

## Evidence pins

All values were read by me during this run unless marked otherwise.

| Artifact | Pin |
|---|---|
| Binding packet (`docs/sen-1-731-chas-gpt6-manager-20261118-v2`, fetched with `gh api contents`) | 16,142 bytes, sha256 `5f9a0550c82f2bc02dd4a42f53a32321f6e5069e57b7d34d39954d08c48d1ca4` |
| `origin/main` | `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`, 2026-11-18T19:55:55Z; GitHub API at reading time returned the same SHA, and the last commit touching the coordination file is the same SHA |
| `main:Docs/Project/coordination/active-tasks.md` | blob `af3ca9a9f448a5b83a0c7fc82220db2c78f79e90`; `wc -l` 981; 779,213 bytes; sha256 `cd38c93a0729f624abf34ba31ac1861e1a2b46b0a063a3ee2af759cad3c1672d` |
| `main:Docs/Project/decision-log.md` (D-091 at line 165) | blob `cbe75b49e471293d5573c38111ccfe94366e5fbc`; sha256 `0e17d2e49b8aed30174eae65248658af3f809691b238bc92564408342eead243` |
| `main:Docs/Project/AgentWorkflow.md` (§Live task locks, line 284) | sha256 `3e3db30aa18049b292fecc1c067d6faae094c645ca1600fd72a8d941941da4e3` |
| Anchor `/media/jer/Files/CCF-Primary` | branch `ops/workflow-foundation-v1`, HEAD `a42eaadc42bda6d474365f39b67a78a9b14bb043` (2026-11-18T23:22:52Z, local only), **ahead 1 / behind 1** of `origin/ops/workflow-foundation-v1` |
| Anchor `AgentWorkflow.md` (committed ops-lineage text) | sha256 `6aa823401ff3b75883b482bbaf558182192586dc3a22a2078a61715d9109ed5f` (differs from `main`; still designates the ops branch at line 339; the anchor's `decision-log.md` has no D-091 by `grep`) |
| `origin/ops/workflow-foundation-v1` tip | `a29c2fa480f1077e12434dcf6679f2f5d9465cdc` (2026-11-15T21:13:20Z); file blob `2673f4b690e8152e43335d9bcf8bc2a75f9aab82`, 210 lines, 140,103 bytes, sha256 `e1f80488b68493ca624ce8918a595ad91d9699af1ba870459a492acec49b085a` |
| Anchor committed file (`HEAD:`) | blob `7c7c5f325419ef6878038d0906c6614fcbb20651` (last changed by `46247486`, 2026-10-30), 207 lines, 138,815 bytes, sha256 `d81d720e2f6d3170def4c694c2b08b9e54ada7a011ad9b840dda9d94419a5064` |
| **Local proposed compaction** (uncommitted) | `…/coordination/active-tasks.md`, 93 lines, 15,555 bytes, sha256 `ec842f269258d64aec3fee6b23f8c56ef1b8fa63822caf2e4472d39611afbc8f` (matches the parent's pin), mtime 2026-11-18 19:52:56Z, on-disk mode 0755 while the index mode is 100644 (`core.fileMode` is `false`, so Git ignores the difference) |
| **Local archive** (untracked) | `…/coordination/archive/active-tasks-2026-10.md`, 111 lines, 124,637 bytes, sha256 `4b9faa3eae8d8a389ddcc8dc0d8a754f545a193a6dd9e6805626778a75510b22` (matches the parent's pin), mtime 19:50:36Z |
| Dirty `AGENTS.md` | sha256 `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd` (2-line change in the storage-policy bullet) |
| Dirty `Docs/Operations/StorageArchitecture.md` | sha256 `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22` (13-line change) |
| Untracked note 1 | `PostStage1CCFWorktreeEvidenceDisposition-20261118.md`, sha256 `fc8cff37496aaeca63f66dccb490f5e250c8812909460bb70b08a860145d81be` |
| Untracked note 2 | `Stage1StorageRecoveryExecution-20261118.md`, sha256 `188fd66dad072912f1266a9f7ac927e5745a8b3c204e69b5346df9af576a9651` |
| Untracked note 3 | `StaleWorktreeDiskPressureHandoff-20261118.md`, sha256 `c2d9f78fe82f4fd74d4b5552b6c1b1df13145fa96c5f293922ad1c8873e39691` |
| Deployed `/media/jer/ZX20/CCF-Ops/lanes.json` | sha256 `c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e`, 12,179 bytes, mtime 2026-11-18T17:28:31Z |
| Deployed `board/active.md` | sha256 `4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994`, 141 lines, 110,616 bytes, mtime 2026-11-18T17:28:31Z |
| Deployed `deployment.json` | sha256 `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` |
| Repo-tracked copy `CCF-Primary/CCF-Ops/lanes.json` | sha256 `53d3ed9c79adbe032b406d1f1ea4342613532b4c432e6df2c5d354f9719e77f9` (4 lanes; last change `3614783e`, 2026-10-15) |
| Repo-tracked copy `CCF-Primary/CCF-Ops/board/active.md` | sha256 `cb6bcc40e10a4668168bed9b90e5a7f9e3bb167997f0b5248e6fd850d3e2b24c`, 37 lines, zero `2026-11` strings |
| GitHub issue snapshots (JSON, first 16 hex of sha256) | #781 `d04895068d911e5d` (11 comments, last 2026-11-19T01:06:16Z); #731 `27467b56fcd648ba` (29 comments, last 2026-11-18T23:52:56Z); #777 `35a861326b583d6d`; #778 `02e1703f65877e56`; #652 `a8d58bc759f0e336`; #789 `176ad8391e41ecd3` |
| Storage (observed) | `/media/jer/Files` `/dev/sdb4` ext4, 92 % used, about 39.9 GB free; ZX20 `/dev/sdc2` exfat; `/` 85 % used, about 36.6 GB free |

I re-hashed the local file, archive, `AGENTS.md` and `StorageArchitecture.md` at the end of the run; all four hashes were unchanged. The anchor's `.git` worktree directory showed a changed directory mtime (01:10:29Z) during my reads. The `index` file (mtime 2026-11-18T23:22:52Z) and `HEAD` were untouched, so the likely cause is a transient lock file from a read-only `git` command (mine or another actor's); I cannot distinguish them.

---

## Part D — Why the local file lacks the November rows (source ancestry)

### D.1 Verified ancestry facts

| Fact | Evidence |
|---|---|
| The anchor branch and `main` diverged on **2026-10-12** | merge-base `c1564b857972ee18917e8a461c457533a54cc3f1` (2026-10-12T15:34:18+01:00) for both `HEAD` and `origin/ops/workflow-foundation-v1` |
| Since then `main` has **1,849** commits the anchor lacks; the anchor has **62** commits `main` lacks | `git rev-list --count HEAD..origin/main` / `origin/main..HEAD`. The 62 span 2026-10-12 → 2026-10-30, plus one on 2026-11-18 (`a42eaadc`) |
| `main`'s coordination file has **543** commits in total (528 since the merge-base) | `git log origin/main -- <file>` |
| `main`'s file was **dormant from 2026-10-11 to 2026-10-27** (28 lines at the merge-base; first new commit `896198b1`, 2026-10-27T09:07:07Z) | Coordination for 2026-10-16 → 2026-10-26 was kept on the ops branch because `AgentWorkflow.md` (ops lineage, line 339) designated `ops/workflow-foundation-v1:…/active-tasks.md` as the physical record for that transition |
| From 2026-10-27 `main` resumed, ahead of everything the ops branch later did | 26, 23, 49 commits on 10-27/28/29 and 543 in total; 969 of the 981 current lines are November-authored (`git blame`) |
| The anchor file's **last substantive ops-branch change** is `46247486` (2026-10-30) and its last dated entry is 2026-10-26 | `git blame` of the local file: 42 lines from October commits (last `46247486`, 5 lines), **51 lines uncommitted** |
| The ops branch **formally retired itself** on **2026-11-15** | `origin/ops/workflow-foundation-v1` commit `a29c2fa4` rewrites lines 2–7 of its own file to a "SUPERSEDED AUTHORITY DESIGNATION" naming `main:…/active-tasks.md` as the single live authority, and strikes through the ops-branch designation. `main` line 314 ("AUTHORITATIVE LIVE LOCK RECORD — 2026-11-15 ~21:12 UTC (MANAGER #424 RECONCILIATION)") says the same |
| The anchor **does not have `a29c2fa4`** | `HEAD...origin/ops` is ahead 1 / behind 1: ahead is `a42eaadc`, behind is `a29c2fa4`. `a29c2fa4`, `46247486` and `a42eaadc` are none of them ancestors of `main` |
| Zero of the 33 October task IDs exist in `main`'s file | exact-string search; also 1 of the 65 non-blank local lines exists verbatim in `main` |
| October code from those rows **is** in `main` | e.g. `36eafce`, `51d47dd`, `1398887`, `6bdcdd5`, `d330317`, `fc6fbb4`, `8f9ed80`, `f6c5d3a`, `8942b5c` are ancestors of `origin/main` (checked with `merge-base --is-ancestor`). The *code* was integrated; the *coordination rows* never were |

### D.2 Cause

The local file is a **mixture of three effects**, and I can separate them:

1. **Stale base (verified).** It was made from the committed ops-branch file at `46247486` (207 lines — the "207" in the report), in a checkout that is 1,849 commits behind `main` and one commit behind its own upstream.
2. **Divergent branch (verified).** The two files are separate lineages with no shared coordination content after 2026-10-12. The October rows were only ever recorded on the ops branch; November rows were only ever recorded on `main`.
3. **Hand-authored trim (verified for the facts, inferred for the intent).** 51 lines are uncommitted and written on 2026-11-18 at about 19:50–19:52Z (archive 19:50:36Z, file 19:52:56Z); five of the 33 rows were rewritten with changed field values, and a new header notice, physical-lane table and "fidelity checks" were added. The file bears no author attribution and there is no commit, so **who or what authored it is UNKNOWN**.

**Inference, flagged.** The author most plausibly worked from the anchor's own instructions: the anchor's `AGENTS.md` says to read `active-tasks.md` at task start, and the anchor's committed `AgentWorkflow.md` still says the ops branch is the designated record. A worker following those files would read the October snapshot and see no November work. The notice "No November work is recorded here" is then literally true of that file and false of the project. This inference explains the pattern; it is not proved.

**Timeline of the downstream effect (verified).** Local archive and file written 19:50–19:52Z → `main` commit `a69b3e2e` 19:55:55Z → Manager #731 "COORDINATION INTEGRITY HOLD — DO NOT COMMIT/INTEGRATE AS-IS" 19:59:19Z ([#731 comment 6989955232](https://github.com/jermur788/CCF/issues/731#issuecomment-6989955232)) → the #777 parent's preflight, 20:00–20:40Z, read the anchor file and recorded the gate "Live project coordination: BLOCKED" in [#778 comment 6990235810](https://github.com/jermur788/CCF/issues/778#issuecomment-6990235810). The #777 parent stated the anchor HEAD as `46247486` and "behind 1", so it read the stale anchor copy, not `main`.

Main's own rule is explicit on this point: `main:AgentWorkflow.md` line 288 says "A stale or dirty local coordination copy does not override a newer live GitHub registration."

---

## Section 1 — Discrepancy tables

Legend: **VERIFIED** = I read the evidence myself; **INFERENCE** = reasoned from verified facts; **UNKNOWN** = not determinable from this host.

### 1.1 Statements in the local 93-line file against evidence

| # | Local statement (line) | Compared against | Verdict |
|---|---|---|---|
| L-01 | Header (5–7): the ops-branch file is the "temporary authoritative physical record". | `a29c2fa4` (2026-11-15) retired it on `origin/ops`; `main` line 314 and `main:AgentWorkflow.md` §Live task locks name `main` as the only authority. | **FALSE since 2026-11-15** (VERIFIED). The anchor lacks `a29c2fa4`. |
| L-02 | Notice (9): "**No November work is recorded here.** … SEN-1, MULTI, save-spec v2 and the per-task worktrees … is not described by this file or by the CCF-Ops board." | `main` file: 336 `2026-11-` strings, 277 dated paragraphs. `git worktree list` shows 215 worktrees. | True of the local file; **false as a statement about live coordination** (VERIFIED). |
| L-03 | Notice (9) and line 92: the CCF-Ops board "was last reconciled 2026-10-16 and equally does not describe November work". | Deployed board: 141 lines, 44 task rows of which 25 are November (#403, #406, #409, #412, #413, #460, #544, #629, #633, #635, #638, #647, #652, #674, #683, #687, #690, #697, #710, #715, #720, #725, #732, #755 ×2). Its header sentence is stale (still says "Reconciled 2026-10-16"), but the rows are not. | **FALSE for the deployed board** (VERIFIED). True for the stale repo-tracked copy (37 lines, 0 November strings). |
| L-04 | Check 1 (62): `lanes.json` pins `/dev/sda2`; ZX20 is mounted from `/dev/sdd2`; `unity-lock status` and `lane-status` fail. | Deployed `lanes.json` `storage.device` is a stable `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2`, UUID `18D6-896E`. ZX20 is now `/dev/sdc2` exfat. `/dev/sda2` appears in the **repo-tracked** copy (sha `53d3ed9c`). I did not run either Ops tool. | **Not supported by the deployed config** (VERIFIED). Match with the repo copy is an **INFERENCE**. Whether the tools fail today is **UNKNOWN to A2** (A1's lane; the parent reported `lane-status --storage-only` PASS on `/dev/sdc2` at 00:5xZ). |
| L-05 | Check 2 (63): `lanes.json` points `repository` and NonUnity at ZX20 and omits U3. | Deployed: `repository` `/media/jer/Files/CCF/Repository.git`; `NonUnity` `/media/jer/Files/CCF/Operations/CCF-NonUnity`; **U3 registered** (`/media/jer/ZX20/CCF-U3`, path exists); **Q10 registered** (`/media/jer/ZX20/CCF-Q10`, path exists). Repo-tracked copy has exactly the described defects. | **Not supported by the deployed config** (VERIFIED). |
| L-06 | Table (49–54): five paths with branch/HEAD. | `git worktree list`: U1 `e7f086a1`, U2 `e26516ff`, U3 `4db486db`, Integration `8f9ed802` match. The anchor HEAD is now `a42eaadc`, not `46247486`. | Four lane rows **VERIFIED**; one row **stale**. |
| L-07 | The lanes table lists four ZX20 lanes (U1, U2, U3, Integration). | Deployed `lanes.json` has 25 entries: 16 on ZX20, 9 on Files (24 of 25 paths exist; the absentee is `Sen1SaveA0NonUnity690`, a prospective reservation). | **Omits 12 registered ZX20 lanes and all 9 Files lanes** (VERIFIED). |
| L-08 | Unity ownership (39): "No live task in this record holds U1, U2 or U3." | `state/unity` contains `guard` and `history.jsonl` only, no `lock.json`. The parent reported `unity-lock status` → free. I did not run the tool or inspect processes. | **Plausible, not verified by A2**. |
| L-09 | Check 4 (65): rows removed were "terminal, consumed or superseded, or parked". | See Section 2. 21 rows removed outright; 4 carried a stated open or unclaimed gate in their own text (rows 19, 24, 30, 47); row 43 is `BLOCKED (handed back)`. | **Not demonstrated by the file** (partly INFERENCE-supported, see Section 2). |
| L-10 | Archive (90): "every row and narrative paragraph removed … verbatim". | 89 of 115 removed/changed non-blank lines are verbatim in the archive; **26 are not**. | **Overstated** (VERIFIED). |
| L-11 | Retained rows (19–23): carried over. | Cell diff vs `HEAD:`: model `gpt-6.1-terra` → `gpt-6.1-sol medium` (ART-SPACING-POLISH); Owner `T3 Code / deepseek-v4.1-flash (repair owner)` → `NONE` (CONTROL-TOWER); Owner `T3 Code / deepseek-v4.1-flash` → `NONE` (PROJECT-DETAIL-PAGES); new assertion that ZX20 "reports 45 GB free" (SCENARIO-TWO). | **Ownership/model fields changed with no recorded authority** (VERIFIED). |
| L-12 | Line 45: per-task T3 worktrees `.t3/worktrees/…` "are now the normal execution surface". | 23 of 215 worktrees are under `/home/jer/.t3/worktrees/CCF-Primary` (root filesystem). Owner directive of 2026-11-18T22:02Z forbids new development on root ([#781 comment 6992993906](https://github.com/jermur788/CCF/issues/781#issuecomment-6992993906)). | **Obsolete guidance** (VERIFIED). |
| L-13 | Line 93: "739 directories" of evidence. | 741 at reading time. | Trivial drift. |
| L-14 | Line 49 (anchor row): "two modified tracked files … and three untracked notes". | Now three modified tracked files (the file itself is the third) plus four untracked paths (three notes and the archive directory). Matches the parent's seven-item pin. | Consistent after the compaction itself was added. |

### 1.2 Live claims: GitHub against `main`, local file, deployed board and `lanes.json`

State, title, owner, model and session are read from the live issue (snapshot ~01:05Z). "In main" counts lines of `main`'s file that mention the number.

| Issue | State / title (live) | Owner / model / session (from the claim) | Latest relevant comment | `main` file | Local file | Deployed board | `lanes.json` |
|---|---|---|---|---|---|---|---|
| **#731** | OPEN `[RUNNING — CHAS / GPT-6 SCENARIO ONE OVERALL MANAGER]` | OpenAI GPT-6, ChatGPT Game Dev GitHub connector, browser-only, no physical custody; "session internal identifier unavailable, not invented" ([transfer 6989435275](https://github.com/jermur788/CCF/issues/731#issuecomment-6989435275)) | 2026-11-18T23:52:56Z routing correction | 13 mentions + section lines 960–973 | **0** | 1 (inside the #652 row) | none |
| **#777** | **CLOSED** 2026-11-18T21:23:05Z `[HANDED-OFF / PARTIAL — SOURCE AND UNITY GATED]` | Parent `opencode-go/deepseek-v4.1-flash` variant high, T3 `d7c8c0e9-ac8b-413f-9943-cf2b164ef829`; children #774 Sol author, #767 Opus world reviewer, #775 recheck ([RUNNING 6990050116](https://github.com/jermur788/CCF/issues/777#issuecomment-6990050116)) | Manager accepted the partial handoff 21:22:37Z (6992026591) | 2 mentions + section lines 975–981, **still says "READY, NOT RUNNING"** | **0** | 0 | none |
| **#778** | OPEN `[READY — TEST-ONLY / CONDITIONAL PHYSICAL PREFLIGHT]` | No writer assigned; Manager grant absent | BLOCKED preflight 20:10:14Z; Manager confirms still blocked 21:22:39Z | 1 mention (line 979) | **0** | 0 | none |
| **#781** | OPEN `[READY — G0 READ-ONLY / G1-G2 HELD]` | Parent `opencode-go/deepseek-v4.1-flash` high, T3 `26ad9ad0-c04f-4125-b4d4-ad79f3b59220`; A1 `codex/gpt-6.1-sol`; A2 `claudeAgent_claude_b_pro/claude-sonnet-5-5`; Q `claudeAgent_claude_b_pro/claude-opus-5-5` (not yet dispatched) | 2026-11-19T01:06:16Z placement correction | **0** (post-dates `main`) | 0 | 0 | none |
| **#652** | OPEN `[SOURCE APPROVED — DEPLOYMENT HELD / FINAL EXFAT V6 REVIEW OPEN]` | Author Codex `gpt-6.1-sol` `9bcdfde6…`; reviewer Sonnet 5.5 `5b89dd08…`; installer preflight Opus 5.5 `a368d8d5…` (read-only); board custodian Opus `81a16309…`; NO LIVE INSTALL | Preflight result NEEDS CHANGES 13:19:54Z | **66 lines** (103 string hits) | **0** | row `CCF-652-OPS-READINESS` READY, reserved / not running / source blocked | none |
| **#789** | OPEN `[READY — SPEC FIRST / SOURCE HELD]` (title stale; comments show Stage A RUNNING 23:05Z → COMPLETE 00:35Z → Manager review APPROVED (doc quality) 00:47Z → overnight handoff 01:01Z) | Manager #730 (GPT-6 GitHub); parent DeepSeek V4.1 Flash `opencode-go`; no source grant | 2026-11-19T01:01:07Z | **0** (post-dates `main`) | 0 | 0 | none (its three worktrees `/media/jer/ZX20/CCF-789-*` appear in `git worktree list` but not in `lanes.json`) |
| #690 | OPEN `[APPROVED TO START — CONDITIONAL / CLAUDE #690 A0 NOT YET RUNNING]` | Exact conditional sole writer Claude Opus 5.5; custodian user `jermur788` | — | **53 lines** | 0 | row `CCF-690-SEN1-SAVE-A0` BLOCKED | `Sen1SaveA0ZX20NonUnity707` (advisory expiry text), `Sen1SaveA0NonUnity690` (path absent) |
| #718 | OPEN `[D-089 … W12 + GQ + UNITY LEASE STILL HELD]` | Unity Tier S held | — | 28 lines | 0 | 0 | none |
| #765 | OPEN `[RUNNING — GPT-6 BROWSER DOCS ONLY]` | GPT-6 browser writer | — | 3 lines | 0 | 0 | none |
| #730 | OPEN `[RUNNING / GPT-6 GITHUB MANAGER]` | Multi manager | 2026-11-19T01:01Z | 18 lines | 0 | 5 | Multi lanes (#674, #687, #697, #710, #725, #755 ×2) |
| #785 / #786 / #787 / #788 | OPEN `READY` (asset programme) | Not RUNNING | — | **0** | 0 | 0 | none |
| #779 | OPEN `[HANDED-OFF — G1b REMEDIATION / STOP FOR #730 SIGNATURE]` | Multi | — | 0 | 0 | 0 | none |

Observations from the table:

- **Every row an OPEN issue gives with RUNNING or a conditional sole-writer status is absent from the local file.** Presence in the local file would not have been a lock either; the point is that a reader of the local file cannot discover any of them.
- **`main` is not complete.** It lacks #781, #789 and #785–#788 and still shows #777/#778 as not running. #781's own claims live only in GitHub issue comments. This does not make the local file better; it means the base for any compaction needs a registration sync first (Section 3 step 1).
- **Board and `lanes.json` carry November custody that `main`'s file barely mentions:** #697 appears on the board 5 times and 0 times in `main`; #710 14 times vs 6; #725 9 vs 2. A compaction that looks only at `main`'s file would under-protect these.

### 1.3 Deployed `lanes.json`: facts for the U3/Q10 claim

`lanes.json` (sha256 `c1d14719…`) has **25** lane entries; the repository path is `/media/jer/Files/CCF/Repository.git`; `git_stores` lists the Files canonical store and the legacy store. U3 and Q10 are both present and both paths exist. The parent reported "20 registered lanes" from `lane-status`. My JSON count of entries is 25. **I did not run `lane-status`, so I cannot explain the difference between 20 and 25** (UNKNOWN; A1's lane). November lanes are present: `MultiP0Proof674`, `MultiP0P1Integ687`, `Sen1SaveA0NonUnity690`, `MultiP1ChildB697`, `Sen1SaveA0ZX20NonUnity707`, `MultiP1ModeRepair710` (expiry text extended to 2026-11-23T18:00:00Z), `MultiAnnualF0Child725` (expiry text 2026-11-27T18:00:00Z, immutable retention), `MultiH0ACore`, `MultiH0AViewer` ("NO EXPIRY (D-091)").

---

## Section 2 — Per-entry preservation / ownership provenance map

Labels: **TERMINAL** (finished by its own text), **CONSUMED** (folded into a later task or integrated; own text or ancestry says so), **SUPERSEDED** (replaced by a named later entry), **STILL-LIVE** (open gate or live owner/custody), **UNKNOWN**. "Archive" means the local untracked archive; "git" means the commit that holds the original (`46247486` on the ops branch, in the shared store, unless stated).

### 2.1 The 33 task rows of the October snapshot (`HEAD:` lines 17–49)

Disposition against the local file: **K** = retained in the live table (rewritten), **P** = kept as a one-line "parked gate" bullet, **R** = removed to the archive (verbatim).

| Row (HEAD line) | Own state | Local | Code in `main`? | Class | Preserved by | Note |
|---|---|---|---|---|---|---|
| SCENARIO-ONE-ONBOARDING-V1-ART-SPACING-POLISH (17) | REVIEWING, owner NONE, "user acceptance pending" | K, **model changed** | `c74dce0`: **no**; 1 unique commit on local branch `feature/scenario-one-three-page-onboarding-v1`, no remote | **STILL-LIVE (human gate) / UNKNOWN** | git; **local branch ref only** for the commit | Only copy of the work is an unpushed ref |
| SCENARIO-ONE-ONBOARDING-V1-VISUAL-FIXUP (18) | REVIEWING, "acceptance pending" | K | `089dbeb`: yes | **CONSUMED in code; acceptance UNKNOWN** | git; code on `main` | |
| SCENARIO-ONE-UI-VISUAL-SYSTEM-V2 (19) | READY, owner NONE, "G4 integration and playtest separate, not claimed" | R + one-line summary | `36eafce`, `0805ffb`: yes | **CONSUMED in code; formal closure UNKNOWN** | archive, git, `main` | |
| DASHBOARD-SYNC-WORKFLOW-RULE-V1 (20) | DONE | R | `e1a5548`: **no** (ops-only) | **TERMINAL** | archive, git (ops branch only) | The rule commit never reached `main` by SHA |
| DASHBOARD-CONTROL-TOWER-TASK-NAV-V1 (21) | REVIEWING, owner deepseek-v4.1-flash | K, **owner → NONE** | `fbf59ea`: **not in the shared store at all** | **UNKNOWN** | git (row text only) | Commit lives in some other repository |
| DASHBOARD-PROJECT-DETAIL-PAGES-V1 (22) | REVIEWING, owner deepseek-v4.1-flash | K, **owner → NONE** | `3197af5`: **not in the shared store** | **UNKNOWN** | git (row text only) | as above |
| SCENARIO-ONE-UI-VISUAL-SYSTEM-V1 (23) | READY; "Consumed by candidate `51d47dd`" | R | `5e0935b`, `51d47dd`: yes | **CONSUMED** | archive, git, `main` | |
| SCENARIO-ONE-UI-NONUI-INTEGRATION-RECONCILIATION-V1 (24) | READY; "independent fresh-provider review … **still outstanding**; user playtest next" | R + one line "Superseded by the onboarding line" | `1398887`: yes | **CONSUMED in code; open review gate UNKNOWN** | archive, git | An open review gate was dropped from the live table |
| SCENARIO-TWO-SPECIES-QUANTITATIVE-CLOSURE-AND-RUNTIME-V1 (25) | WAITING; U2; no writer | K, **state text rewritten** | `e26516f`: **no**; 2 unique commits, local branch only; U2 worktree is at `e26516ff` (VERIFIED) | **STILL-LIVE** | git; local branch ref; U2 lane | "ZX20 45 GB free" is undated and stale |
| SCENARIO-ONE-UX-PEDAGOGY-V1 (26) | DONE-PENDING-LEDGER, consumed by combined integration | R | `fc6fbb4`: yes | **CONSUMED / TERMINAL** | archive, git, `main` | |
| SCENARIO-ONE-MAP-MINIMAP-RACK-V1 (27) | WAITING … "CONSUMED BY …COMBINED-INTEGRATION" | R | `59ab507`: yes | **CONSUMED** | archive, git, `main` | |
| SCENARIO-ONE-PLAYER-TEST-REMEDIATION-V1-COMBINED-INTEGRATION (28) | DONE for this task; fast-forwarded by row 30 | R | `d330317`: yes | **TERMINAL / SUPERSEDED by 30** | archive, git, `main` | |
| SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1 (29) | DONE: INTEGRATED 2026-10-22 | R | yes | **TERMINAL** | archive, git, `main` | |
| SCENARIO-ONE-PROFESSIONAL-MAP-INTEGRATION-V1 (30) | APPROVED FOR USER PLAYTEST, "NOT user-accepted"; next gate the user's Unity playtest | R | `6bdcdd5`: yes | **SUPERSEDED (INFERENCE)**: later candidates and the D-073 playtest line (#412 CLOSED) are on `main` | archive, git, `main` | The row's own open gate is not closed in the file |
| SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1 (31) | READY; "Integrated … at `51d47dd`" | R | `fc8042c`, `51d47dd`: yes | **CONSUMED** | archive, git, `main` | |
| CCF-S1-SEQUENTIAL-OBJECTIVES-IMPLEMENTATION-V1 (32) | DONE-PENDING-LEDGER; owner NONE | R | partly in `main` | **TERMINAL** | archive, git | |
| CCF-S1-GUIDED-ONBOARDING-RACK-UX-FIX-1 (33) | DONE-PENDING-LEDGER | R | partly | **TERMINAL** | archive, git | |
| CCF-S1-GUIDED-ONBOARDING-RACK-UX-V1 (34) | DONE-PENDING-LEDGER | R | partly | **TERMINAL** | archive, git | |
| CCF-S1-GUIDED-ONBOARDING-RACK-UX-REVIEW-V1 (35) | DONE-PENDING-LEDGER | R | partly | **TERMINAL** | archive, git | |
| SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-COMPLETE-V1 (36) | DONE-PENDING-LEDGER; integration complete | R | partly | **TERMINAL** | archive, git | |
| CCF-DENSITY-INTEGRATION-INPUT (37) | READY, owner NONE | P | `271193c`: yes | **UNOWNED GATE; CONSUMED in code** | archive, git, board row (READY, Unassigned), `main` | |
| CCF-CANDIDATE-V2 (38) | READY, owner NONE | P | `c20c6dc`: yes (also `origin/integration/scenario-one-final-candidate-v2`) | **UNOWNED GATE; CONSUMED in code** | archive, git, board, `main` | |
| CCF-T2-OPERATIONAL-PILOT (39) | WAITING | P | n/a | **UNOWNED GATE (STILL-LIVE-as-unowned)** | archive, git, board | |
| CCF-COMPONENT-BA (40) | READY-FOR-INTEGRATION | P | `1e642a2`: yes | **UNOWNED GATE; CONSUMED in code** | archive, git, board, `main` | |
| CCF-GROWFOR (41) | READY-FOR-INTEGRATION | P | `a94c664`: yes | **UNOWNED GATE; CONSUMED in code** | archive, git, board, `main` | |
| CCF-TIER3-PREFLIGHT (42) | PRESERVED | P | `a5e4ddc`: yes | **UNOWNED GATE** | archive, git, board | |
| CCF-TIER3-REPIN (43) | **BLOCKED (handed back)** | R, **not parked** | `e8ca1f8`: yes | **SUPERSEDED (INFERENCE)** by rows 45–47 and `main` rows #231/#240 "APPROVED / CLOSED" | archive, git, `main` | Removed while its own state is BLOCKED; the file never says it closed |
| CCF-MODEL1-HISTORICAL-FIXTURE (44) | DONE-PENDING-LEDGER | R | n/a | **TERMINAL** | archive, git | |
| CCF-TIER3-QUALIFICATION (45) | DONE (awaiting Manager) | R | `f6c5d3a`: yes | **TERMINAL** (Manager acknowledgement is on `main` via #231 APPROVED / CLOSED — INFERENCE) | archive, git, `main` | |
| CCF-TOOLING-SUCCESSOR (46) | DONE (pinned) | R | `8942b5c`: yes | **TERMINAL** | archive, git, `main` | |
| CCF-S1-CANDIDATE-8F9ED80 (47) | DONE (pinned); open gate "Windows candidate B08 build, native Windows smoke, owner playtest, independent release review, G4" | R | `8f9ed80`: yes; Integration lane still at `8f9ed802` | **TERMINAL as a pin; gates UNKNOWN** | archive, git, `main`, physical lane | `main` currently forbids any Windows build before an explicit user playtest and separate authorisation, so the gate text is not safely "closed" |
| CCF-MODEL2-LANE (48) | PENDING INDEPENDENT REVIEW since 2026-10-16 | P | `b81f737`: yes | **UNOWNED GATE; review UNKNOWN** | archive, git, board | |
| CCF-SITKA-GROWTH-EVIDENCE-V1B-RECORD-SYNC (49) | DONE | R | n/a | **TERMINAL** | archive, git | |

Counts: 21 removed (R), 5 retained and rewritten (K), 7 parked (P). Of the 21 removed, 16 are TERMINAL or CONSUMED with no open gate stated in their own text, 4 carry a stated open or unclaimed gate (rows 19, 24, 30, 47) and 1 is `BLOCKED` (row 43). **None of the 33 rows is recorded in `main`'s file;** they are preserved only by the archive, git, `origin/ops` and the board's October rows (the deployed board still carries rows 20–38, e.g. `SCENARIO-TWO-…` WAITING, `CCF-DENSITY-INTEGRATION-INPUT` READY).

### 2.2 Non-row entries of the October snapshot

| Entry (HEAD lines) | Local treatment | Preserved by | Class |
|---|---|---|---|
| Authority designation, lines 5–7 | **kept verbatim** | git | **SUPERSEDED** 2026-11-15 by `a29c2fa4` / `main` line 314 (kept by the local file, contrary to the retirement) |
| Line 9 (anchor path note) | rewritten | **git only** (not in archive) | informational |
| Line 13 (HEAD-pin explanation) | rewritten (extended) | git only | informational |
| "Unity ownership" history, lines 53–83 (16 dated paragraphs) | replaced by a two-paragraph summary | archive (verbatim) | **TERMINAL** (history) |
| "Physical lanes (permanent path roles)", lines 85–104: rows for `CCF-Primary`, U1, U2, Integration, **NonUnity, Review, Repository.git, DensityInstrumentation, Growfor, Tier3Preflight**, plus detached-worktree and topology notes | replaced by a new observed table | **git only** (not in archive) | **UNKNOWN**: the six dropped ZX20 paths are all absent today (`CCF-Review`, `CCF-DensityInstrumentation`, `CCF-Growfor`, `CCF-Tier3Preflight`, `CCF-NonUnity`, `CCF-Repository.git` do not exist under ZX20), so removal was probably retirement, but no entry records it |
| Reconciliation holds, lines 108–114 | kept (verbatim) | local file, git | STILL-LIVE as references / evidence holds |
| Hold bullets 115–116 and evidence lines 118, 120 | rewritten | **git only** | informational |
| Dated chronology, lines 122–207 (≈40 paragraphs, 2026-10-18 → 2026-10-26) | replaced by four one-line "recent changes" | archive (verbatim) | **TERMINAL** (history) |

Together with the five rewritten rows, these account for the 26 original lines that are **not verbatim in the archive**: 5 rows (17, 18, 21, 22, 25), line 9, line 13, 13 lines in 85–100 plus 102 and 104, and lines 115, 116, 118, 120. They survive in git (`46247486` is in the shared store and on `origin/ops`).

### 2.3 What the local file omits from `main` (silent non-carry)

The local file carries one line in common with `main`; every `main` entry is therefore omitted. I did not classify each of `main`'s 514 non-blank lines individually (Section 5). By structure and live issue state:

| Group in `main`'s file | Size | Live or not (by live issue state) |
|---|---|---|
| Lines that mention **at least one OPEN issue** | 297 lines, ≈617 KB of 779 KB | **Presumed STILL-LIVE** until shown otherwise; 53 distinct OPEN issues (Appendix A) |
| Lines that mention only CLOSED issues | 151 lines, ≈149 KB | Candidates for archive, each still needs the checks in Section 3 |
| Lines with no issue number | 66 lines, ≈12 KB | Rules, headings, hard boundaries: keep by default |
| Section "Coordination rules" (645–656), "Hard boundaries" (920–959), line 314 authority statement | — | Rules, **STILL-LIVE** |
| Sections 960–981 (#731, #777) | 22 lines | **STILL-LIVE** (and #777 text is out of date, see Section 1.2) |

---

## Section 3 — Safe forward-only compaction procedure (from CURRENT `main`)

This is a procedure for **Manager #731 (or an assigned integrator)**; AgentWorkflow makes `active-tasks.md` single-writer. Nothing here grants that role. It starts from `main`, never from the local or ops-branch file, and is designed so a live November claim cannot disappear without a failing check.

**Principle: default KEEP. A line leaves the live file only when every test below passes; any UNKNOWN keeps it.**

**Step 0 — Pin the input.**
Fetch `main` from GitHub (not from the anchor). Record `M0` = commit SHA, the file's blob SHA and sha256, and line/byte counts. The current pin would be `a69b3e2e` / blob `af3ca9a9…` / sha256 `cd38c93a…`. Use a fresh checkout on a verified non-root filesystem (Files has about 39.9 GB; keep the 10 GiB floor). Never use the anchor as a working copy.

**Step 1 — Registration sync first, as its own commit.**
Before any removal, the Manager records what `main` lacks: the #777 closed/partial outcome, #778 current state, #781 parent and children (A1, A2, Q), #789, #785–#788, and the current Multi lane list. Compaction then carries no new facts. This keeps "add" and "remove" separate, and it fixes the base, so `M1` replaces `M0`.

**Step 2 — Deterministic inventory.**
Split the file into units: each top-block bold paragraph, each table row, each heading plus its paragraphs, each bullet. For every unit record: stable id, original line range, byte length, sha256, every `#N`, branch, path and SHA it names, and its disposition. Write the manifest into the head of the archive file so the change stays two files.

**Step 3 — Classification tests (all must pass to archive a unit).**
- **T1 Issue state.** Read each referenced issue live (`gh issue view`). A unit referencing any OPEN issue is KEEP. On `main` today that rule alone keeps 297 of 514 non-blank lines. A failed or ambiguous read is KEEP.
- **T2 Physical custody.** Search the deployed `lanes.json` and `board/active.md` (read-only, hashes pinned) for every issue number, lane name, branch, path and session id in the unit. Any hit keeps the unit or requires the unit's facts to be restated in a kept unit. This protects #697, #710, #725 and the #755 H0-A lanes, which the board carries more than `main` does.
- **T3 Gates, holds, expiries.** Any unit asserting an unreleased hold, "HELD", "NO LIVE INSTALL", sole-writer or conditional-writer status, custody, expiry or D-091 reasoning is KEEP unless an explicit later kept unit restates it. `main` lines 37, 39, 63, 134, 139, 141 and 145 contain 2026-11-19/20 expiry text; they must not disappear before their custodians release (D-091: no arbitrary expiry, no stale-agent reclaim).
- **T4 Protected set.** Never archive: line 314 (authority), "Coordination rules", "Hard boundaries", the #731/#777 sections, the live-table header, and any unit naming #652, #690, #718, #730, #731, #765, #770, #777, #778, #781, #789, #785–#788, #641, #425, #382, #443 until its owner records release.
- **T5 Supersession must be named.** "Superseded" requires a specific kept unit that restates the outcome. Inference from SHA ancestry alone is not enough.

An indicative upper bound only, not a classification: lines mentioning only closed issues plus lines with no issue number total ≈161 KB, about 21 % of `main`'s 779 KB. A reduction toward 15 KB is **not reachable lawfully** by these tests.

**Step 4 — Lossless archive.**
Write archived units as exact byte slices, in original order, in `Docs/Project/coordination/archive/`, with the header `M1 SHA, blob, line range, unit id`. The live file may gain only a short notice and per-group pointer stubs, each marked ADDED. Do not edit any retained unit.

**Step 5 — Mechanical proofs (repeatable offline by an independent reviewer).**
1. **Multiset identity:** the units of `M1` equal retained ∪ archived, each exactly once, ignoring only ADDED lines.
2. **Diff limit:** `git diff M1..compact -- active-tasks.md` shows only deletions of listed archived units plus ADDED lines; zero modified retained bytes.
3. **Issue coverage:** every OPEN issue number present in `M1` is still present in the live file.
4. **Physical coverage:** every lane and board row in the pinned `lanes.json`/board snapshots maps to a retained unit or to a recorded physical-only reason.
5. **No resurrected authority:** no unit designates `ops/workflow-foundation-v1` as a record.
6. **Counts and hashes** for `M1`, the compact file and the archive are recorded.

**Step 6 — Independent review.**
A nonauthor reviewer, different session and provider from the author, rebuilds the manifest from `M1` and re-runs the proofs and the live issue reads. Verdict APPROVED / NEEDS CHANGES / REJECTED, as for any other packet.

**Step 7 — Publish with compare-and-swap.**
Push a branch; only the Manager merges. Immediately before the merge, re-read `main`; if the coordination file's blob is not the pinned `M1` blob, **STOP** and rebuild from the new `main` (the file received 34 commits on 2026-11-18 alone). No force, no silent rebase, no "take ours/theirs".

**Step 8 — The anchor.**
Do not use the anchor's dirty file, archive or ops-branch history as an input. The owner or a physical custodian decides separately what happens to the anchor. A safe preparation is a hash-pinned read-only copy of the three dirty files, the archive and the three notes into a new `/media/jer/Files/CCF/Verification/` folder, made by an authorised custodian task. Do not stash, reset, clean, merge or fast-forward the anchor to "fix" it without that authorisation. Until the anchor is reconciled, every worker must read coordination from GitHub `main` (`gh api` or `git show origin/main:…`), as `main:AgentWorkflow.md` line 288 already requires.

**Step 9 — October-lineage leftovers.**
The 33 October rows exist only in the ops lineage. Do not import them into `main`'s live file. Reference `46247486`, `a29c2fa4` and the archive. Two rows have unique unpublished commits (`c74dce0`; `e26516f` plus one more): ask the owner/Manager for a disposition (publish the ref, or declare it abandoned) before any branch pruning. Never delete those refs.

**Stop conditions.** Any ambiguous classification; any unreadable issue; a concurrent writer to `main`'s file during the window; a lanes/board hash different from the pin without re-running T2; a storage floor breach; or any request to include the local trim as an input.

---

## Section 4 — The exact risk of accepting the 207→93 trim as-is

I use "as-is" to cover three possible actions: committing the local file on the ops branch, copying it over `main`'s file, or letting workers keep treating it as the live record.

| # | Risk | Evidence and effect | Severity |
|---|---|---|---|
| R-1 | **Wholesale loss of November authority from the live record** if it replaces `main`'s file | 981 → 93 lines; 779,213 → 15,555 bytes (98.0 % removed); 952 deletions. Removes the #731 and #777 sections, the 2026-11-15 authority statement, `#652` (103 hits), `#690` (87), `#718` (35), `#730` (21), `#765`, D-091 text (9) and 297 lines that reference OPEN issues. Git would still hold everything, so this is not irrecoverable; the harm is that nothing a worker reads would show it. | **Critical** |
| R-2 | **False "no one owns anything" signal → dual writers** | The notice says no November work exists and the board is equally empty. Both are false (Section 1.1 L-02, L-03). Affected conditional/sole-writer or held statements would vanish: #690 sole Claude A0 writer (board BLOCKED; lane on ZX20 at `55863f1b`), #718 Unity Tier S HELD, #652 NO LIVE INSTALL, #765 RUNNING, #730 Multi custody (H0-A lanes "NO EXPIRY (D-091)"; #710 R custody; #725 immutable retention). The #778 BLOCKED gate shows this effect has already occurred once. | **Critical** |
| R-3 | **Authority designation reversed** | Local header re-asserts the ops branch as authoritative, retired by `a29c2fa4`. `main:AgentWorkflow.md` line 288 states a stale or dirty local copy does not override live GitHub registration. The anchor's own ops-lineage `AgentWorkflow.md` line 337 (not present in `main`'s version) adds that a task is READY only when the board and the authoritative record agree and that disagreement means NOT READY. A worker who follows the anchor's text and sees two disagreeing records is therefore told to treat dispatch, including #778's test start, as NOT READY. | **High** |
| R-4 | **Mechanically unmergeable** | Anchor is behind `origin/ops` by `a29c2fa4`, which edits the same header lines. Taking "ours" restores the retired designation; taking "theirs" yields a fourth variant. Neither is derived from `main`. | **High** |
| R-5 | **Wrong-artifact diagnosis enshrined** | Fidelity checks 1–3 match the stale repo copy of `lanes.json` (4 lanes, `/dev/sda2`) not the deployed one (25 entries, by-id, Files, U3 and Q10 registered). If a coordination record states them as fact, someone may "repair" the deployed config toward the stale copy; the packet's G1 text forbids copying a stale `lanes.json` over current physical state. | **High** |
| R-6 | **Unrecorded ownership edits** | Two Owner cells set to `NONE`, one model changed, one undated physical fact (ZX20 "45 GB free"; the parent observed 343 GiB). These are the "silent deletion of ownership" the Q review is asked about, in miniature. | **Medium** |
| R-7 | **Archive overstates its completeness** | 26 original lines not verbatim in the archive (Section 2.2). Anyone relying on "verbatim" would miss the old physical-lane rows and four evidence lines. | **Medium** |
| R-8 | **Time-sensitive custody statements disappear** | `main` carries 2026-11-19/20 expiry text; `lanes.json` carries `2026-11-19T18:00:00Z` for #687 and #697 and an already-passed `2026-11-19T00:00:00Z` advisory text for #707. Under D-091 none of these may be mechanically cleared or reclaimed; the prose that explains that disappears with a trim. | **Medium** |
| R-9 | **Unpublished work has only a local-ref pointer** | `c74dce0` and `e26516f` (+1) exist only on local branch refs with no remote; the two retained rows are their only coordination pointer. | **Medium** |
| R-10 | **Obsolete operating guidance** | Local line 45 calls `.t3/worktrees/…` the normal surface, in conflict with the 2026-11-18T22:02Z no-root directive. Anchor HEAD is stale. On-disk mode 0755 vs index 100644 (Git ignores it here because `core.fileMode` is `false`). | **Low** |

**What is not lost:** nothing in git. The October rows are in `46247486`/`origin/ops` and (89 of 115 lines) in the archive; `main`'s history is intact. The risk is operational: live ownership and gating text stops being visible, and the local file would become a second, contradictory record.

---

## Section 5 — What remains UNKNOWN or unverifiable from this host

| # | Item | Why unknown |
|---|---|---|
| U-01 | Who or which session authored the local trim, and on what instruction | No commit or attribution; only mtimes (19:50–19:52Z). #731's hold calls it "a local worker". |
| U-02 | Whether `lane-status` / `unity-lock status` fail today, and the real root cause of "mount identity mismatch" | I did not run them (A1's lane; avoids side effects). The parent reported a PASS on `--storage-only` and a slow full run. #777 recorded a failure at 20:00–20:40Z against a deployed `lanes.json` that was already changed at 17:28Z. |
| U-03 | Which `CCF-Ops/lanes.json` the mismatch text came from | I show the stale repo copy matches the description; I do not show the tool was run from there. |
| U-04 | Why the parent saw **20** registered lanes and I count **25** entries | `lane-status` not run by me. |
| U-05 | Where commits `fbf59ea` and `3197af5` (dashboard rows) live, and who owns those two rows | Not in the shared store; no GitHub link in the rows. |
| U-06 | Whether rows 19, 24, 30, 47's open gates (acceptance, independent review, Windows B08) were ever formally closed | `main` holds the code but no closure record that I could find; I did not search every Manager comment. |
| U-07 | Whether `c74dce0` / `e26516f` unpublished work is intentionally local-only, and whether anyone is working there | No process or editor inspection performed. |
| U-08 | Current Unity Editor processes and lock state | `state/unity` has no `lock.json`; I ran no `ps` and no `unity-lock`. |
| U-09 | Whether `main` has moved since `a69b3e2e` | Verified unchanged at about 01:10Z; later moves are possible. |
| U-10 | Dashboard and Google Drive mirror state | Not checked. |
| U-11 | Per-line classification of `main`'s 514 non-blank lines | Not performed. I gave a structural map and counts (Section 2.3); the manifest in Section 3 step 2 is the correct tool and should be built by the Manager's compaction lane. |
| U-12 | Whether the anchor's `StorageArchitecture.md` date for moving the anchor to Files (2026-11-04) or the file's date (2026-10-21) is right | Two dated statements conflict; not material to the finding. |
| U-13 | The meaning of the `updatedAt` 18:51Z on #652 versus its last comment at 13:19Z | Probably a title edit; not investigated. |

---

## What the parent must not claim from this document

- That the local compaction is approved, safe, or an acceptable base.
- That `main` is complete or current: it lacks #781, #789, #785–#788 and has stale #777/#778 text.
- That any physical Ops tool fails or passes today: I did not run them.
- That the deployed `lanes.json` is correct or safe to change: I only compared its contents to the local file's claims.
- That a registration, lock, lane or expiry is released, extended or reclaimed. No G1/G2 permission follows.
- That the October code or rows are formally closed: ancestry shows the code is in `main`, not that a Manager closed the rows.
- That dashboard or Drive mirrors reflect any of this.

---

## Appendix A — OPEN issues referenced in `main`'s coordination file (53)

Count is the number of non-blank lines of `main`'s file that mention the issue. Titles are the live GitHub titles at reading time (truncated).

| # | Lines | Title (live) |
|---|---|---|
| 74 | 1 | WAITING — NON-BLOCKING UNITY FOLLOW-UP — PARITY-03 Q10 |
| 91 | 2 | RUNNING — Scenario One player-test readiness, two-lane continuation |
| 201 | 1 | ACTIVE UMBRELLA — DEEPSEEK HIGH COORDINATES ONLY |
| 203 | 6 | HISTORICAL / MIMO #313 OWNS #294 OVERNIGHT |
| 241 | 1 | HANDED-OFF — orchestration ledger backfill |
| 281 | 15 | PLAYTEST FEEDBACK — OPEN |
| 287 | 35 | NEEDS CHANGES — NOT UNITY QUALIFIED / DIRTY LANE (Workbench Tier-2) |
| 291 | 1 | NON-BLOCKING — transient Files read-only incident |
| 292 | 2 | DRIVE 13/13 APPROVED — ATTACHMENT DEPENDENCY OPEN |
| 293 | 2 | D-072 REGEN NEEDED / PLATFORM ATTACHMENTS PENDING |
| 294 | 4 | MIMO #313 SERIAL OVERNIGHT PREPARED |
| 299 | 4 | NEEDS CHANGES — #309 VERIFIER NEXT |
| 309 | 15 | NEEDS CHANGES — N-1/N-2 GUARDS |
| 318 | 14 | HANDED-OFF / MANAGER NEEDS CHANGES — RUNNER R1 |
| 320 | 10 | HANDED-OFF / MANAGER NEEDS CHANGES — F-1/F-2 |
| 361 | 28 | USER FEEDBACK RECEIVED / NOT ACCEPTED |
| 382 | 42 | CONDITIONAL GO / USER NOT STARTED — personal Editor playtest |
| 383 | 10 | OPEN / PARTIAL — cleanup and migration audit |
| 384 | 2 | READY / CHAT READ-ONLY — OS drive and hardware migration review |
| 386 | 2 | DEFERRED / POST-DEMO |
| 395 | 1 | DEBT / NOT RUNNING — nursery warning visibility |
| 418 | 10 | READY / AFTER #417, NOT RUNNING |
| 424 | 63 | HISTORICAL / MANAGEMENT TRANSFERRED TO #425 |
| 425 | 26 | RUNNING / GPT-6 — Overall Development Manager |
| 438 | 12 | G2 FRESH-FIXTURE WITNESS COMPLETE / G5 PENDING |
| 439 | 2 | USER INTENT ACCEPTED / SPEC #441 READY |
| 443 | 17 | RUNNING CLAIM / G3 APPROVED — G4 NOT YET VERIFIED |
| 447 | 3 | READY / NOT RUNNING — T3 Fable Flow |
| 471 | 1 | PAUSED / NOT RUNNING |
| 544 | 21 | HANDED-OFF / SEN-1 OVERNIGHT COMPLETE — shared save |
| 548 | 12 | NEEDS CHANGES / OPEN — OPS NOT DEPLOYED |
| 553 | 8 | BLOCKED / NOT RUNNING — sync CCF-Ops view to main T-02 reconciliation |
| 565 | 1 | READY / NOT RUNNING — standalone 2D Sitka simulator |
| 611 | 11 | DEFERRED / LINUX ASSETS SELECTED |
| 620 | 1 | READY / NOT RUNNING — OWNER PREFLIGHT |
| 639 | 14 | BOARD RESERVED / AWAITING CLAUDE RE-PREFLIGHT — Multi P1-F0 30m |
| 640 | 22 | READY / NOT RUNNING — successor Manager intake |
| 641 | 23 | RUNNING / GPT-6 — Multi 2D Sitka P1 Manager |
| 652 | 66 | SOURCE APPROVED — DEPLOYMENT HELD |
| 671 | 3 | RUNNING — successor Overall Manager after #640 |
| 672 | 4 | READY / NOT RUNNING — Compact30 world owner signature |
| 688 | 19 | STAGE0 HANDED-OFF / STAGE1 PRODUCTION GATED — H1 entrance kit |
| 690 | 53 | APPROVED TO START — CONDITIONAL / CLAUDE #690 A0 NOT YET RUNNING |
| 698 | 5 | STAGE0 MANAGER APPROVED — Oak/Beech juvenile |
| 704 | 18 | NEEDS CHANGES — D-032 dossier |
| 708 | 2 | HANDED-OFF — NEEDS CHANGES |
| 718 | 28 | D-089 TEMP LOCAL AUTHORING TOOL APPROVED / W12 + GQ + UNITY LEASE STILL HELD |
| 730 | 18 | RUNNING / GPT-6 GITHUB MANAGER — Multi |
| 731 | 13 | RUNNING — CHAS / GPT-6 SCENARIO ONE OVERALL MANAGER |
| 763 | 6 | HANDED-OFF — DRAFT DOCS DELIVERED |
| 765 | 3 | RUNNING — GPT-6 BROWSER DOCS ONLY |
| 770 | 3 | MULTI #730 G0 DOCS-ONLY ADOPTED / 3D IMPLEMENTATION HELD |
| 778 | 1 | READY — TEST-ONLY / CONDITIONAL PHYSICAL PREFLIGHT |

## Appendix B — OPEN issues **not** referenced anywhere in `main`'s file (25)

`#237, #429, #491, #492, #501, #517, #564, #596, #644, #649, #651, #722, #742, #743, #744, #745, #746, #776, #779, #781, #785, #786, #787, #788, #789`. Most are `READY / NOT RUNNING` queue items; #722 is `BLOCKED / NATIVE OPS CUSTODY`; #776 and #779 are Multi `HANDED-OFF`; #781, #785–#788 and #789 are the 2026-11-18/19 programmes. Absence from `main` does not by itself mean a claim is unowned; each such issue holds its own claim comments.

## Appendix C — Method and side-effect disclosure

Read-only commands only: `gh api` / `gh issue view` / `gh issue list` GETs; in the anchor `git rev-parse`, `git log`, `git show`, `git blame`, `git diff`, `git status --porcelain`, `git ls-files`, `git cat-file`, `git merge-base`, `git worktree list`, `git config` and `git fetch --dry-run`; `sha256sum`, `stat`, `wc`, `grep`, `find`, `ls`, `findmnt`, `df`; and inline Python run with `-I` on copies held in `/dev/shm`. The `git status`/`git diff` calls can in principle refresh an index stat cache; the anchor's `index` mtime was unchanged (2026-11-18T23:22:52Z) and the four content hashes were identical before and after. No working-tree byte of the anchor changed. Because `git blame` was run on the working-tree file, its "uncommitted" line count (51) is a count of lines the working tree changed relative to the committed lines.
