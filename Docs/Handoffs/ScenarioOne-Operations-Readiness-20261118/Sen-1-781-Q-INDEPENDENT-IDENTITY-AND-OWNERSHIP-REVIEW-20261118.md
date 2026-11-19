# Sen-1 #781 Q — Independent identity and ownership review (G0)

**Status:** G0 review, document only. It grants nothing: no G1 or G2 action, no lock, no install, no compaction, no ownership change.
**Verdict (one line, repeated at the end):** VERDICT: NEEDS CHANGES - for diagnostic and recovery design only

---

## 1. Reviewer identity, placement and routing

| Item | Value |
|---|---|
| Role | Q, independent nonauthor reviewer of #781 A1 and A2 ([Q claim 6996340389](https://github.com/jermur788/CCF/issues/781#issuecomment-6996340389)) |
| Runtime model (observed from my own runtime) | `claude-opus-5-5` (Claude Opus 5.5), Claude Code via the Claude Agent SDK, running in T3 Code |
| Provider instance | Assigned by the parent as `claudeAgent_claude_b_pro`. I cannot see the provider-instance name from inside my runtime, so this is the assignment, not something I observed. |
| Session identifier visible to me | harness scratchpad session UUID `7066a359-5d2d-467a-96e4-933288644055`. I did not use the scratchpad. I cannot observe the T3 thread id the parent records for me. |
| Host | `jer-laptop` (from `hostname`), Linux 7.0.0-42-generic |
| Checkout | `/media/jer/Files/CCF/Operations/CCF-781-Q-Review-20261118` |
| Branch / base | `review/sen-1-781-q-ops-admission-20261118-v1`, base and starting HEAD `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` (= `origin/main` when I read it) |
| Authorship | I wrote neither A1 nor A2. I did not take part in the parent's draft. I am a sibling dispatched by the same parent, not a child of either author. |

**Reroute record (this fact only, no further claim):** the first Q dispatch, on the Claude A (`claudeAgent`) provider instance, failed before any work with a provider rate limit (5-hour limit). This session is the recorded reroute on the Claude B (Pro) instance, with the same model.

**Placement gate, run first (PASS):**

```text
realpath .                      -> /media/jer/Files/CCF/Operations/CCF-781-Q-Review-20261118
findmnt -T . -o TARGET,SOURCE,FSTYPE -n -> /media/jer/Files /dev/sdb4 ext4
git rev-parse --abbrev-ref HEAD -> review/sen-1-781-q-ops-admission-20261118-v1
git rev-parse HEAD              -> a69b3e2e90829505c792d95f5a3eba3e7d8d8132
git status --porcelain --branch -> ## review/sen-1-781-q-ops-admission-20261118-v1...origin/main   (clean)
```

**What I did not do:** no `lane-status` or `unity-lock` in any form, wrapped or not; no install, edit, acquire, release or force; no write to A1, A2, the parent document, installed Ops, `lanes.json`, the board, `deployment.json`, `state/`, coordination files, source or Unity; no write inside `/media/jer/Files/CCF-Primary`; no `/tmp`, `/dev/shm` or root scratch (all analysis ran as in-memory `python3 -I -B` and read-only shell commands); no GitHub comment or issue change.

**Disclosed side effects, beyond my single commit and push:**
1. I ran `git fetch origin docs/sen-1-731-chas-gpt6-manager-20261118-v2 main` once, in my own checkout, to read the packet at its exact ref. That can write fetched objects, `FETCH_HEAD` and remote-tracking refs into the shared store at `/media/jer/Files/CCF/Repository.git`. `origin/main` stayed `a69b3e2e`, and `refs/remotes/origin/docs/sen-1-731-chas-gpt6-manager-20261118-v2` is `b35595f9`. Taken strictly, this goes beyond "no git write except my own commit", so I record it.
2. I ran one read-only `git --git-dir=/media/jer/Files/CCF/Repository.git worktree list`, to count T3 checkouts on root.

## 2. Inputs reviewed, with pins

| Input | Pin |
|---|---|
| Binding packet @ `b35595f904ac6944a6aa1e56a303279e6b2cd610` | sha256 `5f9a0550c82f2bc02dd4a42f53a32321f6e5069e57b7d34d39954d08c48d1ca4` (same value as A2's pin) |
| A1 document (branch tip `2b14d860`) | 1,659 lines, sha256 `07e9058e930f66e569461a940302f42ef69207fe50fcc3a255a963ecb74322ba` |
| A2 document (branch tip `2b08d661`) | 419 lines, sha256 `1d974d37e07a2d5367b0223e9301c3e41eeab9fae69e35aef5d72f74b8808336` |
| Parent draft (untracked in the parent worktree, on HEAD `a69b3e2e`) | 185 lines, sha256 `3145a163cf34d818a54d36385dae181e5ec5e863a96abb8cd524ac9c3487e6c2` |
| Live `main` decision log, D-091 | line 165 of `origin/main:Docs/Project/decision-log.md` |
| `main:AgentWorkflow.md` | line 264 (reviewer independence), line 288 (a stale local copy does not override a live registration) |
| #781 comments | read in full through `6996770592` (2026-11-19T01:24:03Z) |

## 3. What I verified myself and what I accepted from the authors

### 3.1 Independently verified (read-only, 2026-11-19 ~01:25–01:35Z)

- **Physical identity.** ZX20 is `/dev/sdc2` exfat `18D6-896E`. Files is `/dev/sdb4` ext4 `2283b135-…`. `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2` → `/dev/sdc2`. `lsblk` shows UUID `18D6-896E` on exactly one device. `uptime -s` gives a boot at 2026-11-19 00:39:30 (local time, which is UTC in November), and the udev by-id links are dated 00:24. Either way, the host re-enumerated after #778's 20:00–20:40Z observation window. That supports A1's kernel-enumeration explanation; it does not prove it.
- **Deployed byte pins match A1** for `lanes.json` `c1d14719…`, `deployment.json` `8c66b290…`, `_ccf.py` `ab4343ef…`, `lane-status` `e7102a83…`, `unity-lock` `54cf0a95…` and `board/active.md` `4fa750eb…`.
- **Lock state, by reading files only.** `state/unity` holds only `guard` (0 bytes, sha256 `e3b0c442…`, mtime 2026-10-16) and `history.jsonl` (415 records, sha `dfb50644…`, last record a normal release on 2026-11-15T22:44Z). There is no `lock.json`, and no process has an `argv[0]` basename of exactly `Unity`. This is a snapshot, not a permit.
- **Source and projection provenance.** A (`CCF-OpsReadiness-652-20261116`) is clean at `4c7acc05…`. The two staged five-file projections (`Sen1-752…/staged`, `Sen1-754…/staged`) are byte-identical to each other. Four of their files equal A's `_ccf.py` `24a32146…`, `unity-lock` `8fa0c5ad…`, `self-test.py` `1bd03f15…` and `unity-operator.md` `e44e7eb8…`; their `README.md` `3baf029e…` differs from A's. Deployed `unity-lock` equals R's old version. Deployed `_ccf.py` is older than A's. `CCF-Ops/` has 0 tracked files on `origin/main`.
- **Code reading.** I read deployed `_ccf.py`, `unity-lock` and `lane-status` in full, plus the `_ccf.py`/`unity-lock` diff from deployed (D) to A. A1's descriptions of `storage_check`, `same_device`, `guarded()` (`mkdir` + `a+` + `LOCK_EX`), exact-Lane owner matching and the storage-only exit semantics are accurate.
- **Counts.** `lanes.json` has 25 entries: 16 on ZX20, 9 on Files. The board has 46 table lines, which is 44 data rows. 25 of those rows carry `CCF-4xx…7xx` task IDs (November work) and 19 are October-lineage rows.
- **A2 ancestry and compaction facts.** merge-base(`a42eaadc`, `origin/main`) = `c1564b857972ee18917e8a461c457533a54cc3f1`. 1,849 commits are in `main` but not the anchor, and 62 the other way. `46247486` is not an ancestor of `main`. `HEAD:` coordination is 207 lines, last changed by `46247486` (2026-10-30). The local file is 93 lines, sha `ec842f26…`, and the archive sha is `4b9faa3e…`. `main`'s file is 981 lines, sha `cd38c93a…`. `a29c2fa4`'s subject is "retire outdated workflow-foundation branch as live lock authority". `main` still describes #777 as "READY, NOT RUNNING", while #777 is CLOSED (2026-11-18T21:23:05Z).
- **A2's three ownership edits.** A cell diff of `HEAD:` against the local file confirms them: ART-SPACING-POLISH Model `gpt-6.1-terra` → `gpt-6.1-sol medium`; CONTROL-TOWER Owner `T3 Code / deepseek-v4.1-flash (repair owner)` → `NONE`; PROJECT-DETAIL-PAGES Owner `T3 Code / deepseek-v4.1-flash` → `NONE`. I checked one new-looking retained-row claim ("eight U1 local files archived and cleared with user authorisation"). It is carried over from original `HEAD:` text, not invented.
- **A2's archive gap.** 115 non-blank original lines were removed or changed. 26 of them are not present even as substrings of the archive (27 by exact-line match; the difference is method only). Either way, the archive's "verbatim" claim is false.

### 3.2 Accepted from the authors without re-running

- A1's E07–E09 transcripts: storage-only PASS on D, exit-2 mismatch on R, a PASS from the schema-2 verifier, the wrapped `unity-lock` readback, and the 55 s timeout. I did not re-run these; I was prohibited from doing so.
- A1's 50-file D manifest stability and its E10–E12 issue/owner readbacks.
- A2's `git blame` November-line count (969/981), its 53 OPEN issues and 297 lines, and its per-row Section 2 classifications.
- **The parent's "20 lanes was a `head` truncation" explanation.** No transcript exists, and I cannot reproduce it without running `lane-status`.

## 4. Findings, ranked by severity

### Q-H1 — HIGH (A1 C1/C2/C5/C6; parent §4.3, §9.2 G1-a): non-deployed Ops copies can act on the live Unity lock, and R's stale pin is currently what stops R doing so

**Evidence (static code and config reading; I executed nothing):**
- Every copy of `unity-lock` hard-codes `DEFAULT = ROOT/'CCF-Ops/state/unity'` (line 16 in D, R, A, F and O). Deployed `README.md` line 44 says this is intended: "The production state path is fixed … across all lanes, source checkouts and clones."
- Admission (`config_read`) and owner readback (`board_read`) use the **copy's own sibling** `lanes.json` and `board/active.md`. The `--config` restriction (`unity-lock` lines 97–99) applies only to an *alternate* config. A copy's own default config is not restricted.
- **R** (the primary's `ops/workflow-foundation-v1` copy) fails at `storage_check` only because it pins `/dev/sda2`. Its repository `/media/jer/ZX20/CCF-Repository.git` and stores are also absent.
- **A** (the approved #652 source checkout) appears to pass `config_read` today. Its device is the by-id pin. Its repository `/media/jer/Files/CCF/Repository.git` exists. Its lanes are U1/U2 (both exist on ZX20), Integration, and NonUnity at the absent but ZX20-relative `/media/jer/ZX20/CCF-NonUnity`, which `on_storage` admits. Its board `…/CCF-OpsReadiness-652-20261116/CCF-Ops/board/active.md` sits under `/media/jer/Files/CCF` and is admitted by A's new `on_project_storage`. That board is the 37-line October board (sha `cb6bcc40…`, the same bytes as R's). F and O on `ZX20/CCF-Temp` carry the same by-id 4-lane config and existing boards. I did not trace their code line by line.
- **Consequences, if anyone ran those copies:**
  - Run from A, `lane-status` would print storage READY / PASS, list 4 lanes and show October owners. That is a confident readout that omits 21 registrations and all November custody.
  - Run from A, `unity-lock release --force --owner X --reason Y --manager-approval <any non-empty text>` would delete the **live** `lock.json`. In A, the force path skips the Editor-running and PID-alive checks that A added to normal release.
  - Run from A, `unity-lock acquire --no-expiry` would write a record that the deployed reader treats as `malformed`.
- **Parent §9.2 G1-a** proposes making a successor of `ops/workflow-foundation-v1` pass: "persistent by-id device, … correct current lane list supplied by the physical custodian". Doing that removes R's protective failure. It also creates another tracked lane registry that will go stale exactly the way R's did. R's stale copy is what #778 read, and that read is the incident under review.

**Assessment.** A1's root cause is correct. What it misses is that "copy diverges from deployed" is a live hazard and not only a documentation defect. A1's C1 negative witnesses ("correct bytes on wrong device must fail", "wrong Git store must still refuse") do not cover **the right device reached through the wrong copy**. No proposed remedy has been executed, and nothing here shows a past misuse. This is a design defect in the recovery plan, not an incident.

**Required correction.** See §6 (A1-1, parent P-9, P-6, P-15).

### Q-H2 — HIGH (parent §4.2, §5): unsupported completion and "pass" claims for the full inventory and the lock readback

- Parent §4.2 (around line 88) says: "A full `lane-status` completes, … it reported **25** registered lanes …, all ZX20 lanes resolving to `/dev/sdc2` … independently confirmed by parsing `lanes.json` and by A1."
  - The parent retains no transcript, exit code, elapsed time or output hash.
  - A1's only bounded run **timed out at 55 s with no output** (E09).
  - A1 and I confirmed the 25 **entries in `lanes.json`**. Neither of us confirmed a completed `lane-status` run, so "independently confirmed … by A1" cannot attach to completion.
- The §4.2 heading says: "The deployed Ops installation currently passes its own storage and lock readback". Line 87 then cites an unmodified `unity-lock status --json` exit 0.
  - That command is write-capable (`mkdir` + guard `a+`).
  - The packet says "do not run an unsafe status command if that command mutates state".
  - The parent's run must be recorded as a **disclosed deviation**, not as evidence of a "pass".
- Parent §5 (line 146) says: "that open did not alter any byte or record". The parent gives no before/after evidence. I observed `guard` at 0 bytes with mtime 2026-10-16 and `history.jsonl` unchanged since 2026-11-15T22:44Z. That is **consistent with** no byte change; it does not prove there was none (atime and transient state are not covered). The parent must cite this or similar evidence, or drop the sentence.

### Q-M1 — MEDIUM (A1 C1 and residuals; parent §4.1, §9.1): the identity guard pins ZX20 only; Files-backed admission uses the path prefix alone

**Evidence:**
- In D, `on_durable_ccf` (`_ccf.py` lines 36–41) checks only that a path resolves under `/media/jer/Files/CCF`. It checks no UUID, fstype, by-id device or mount target. The configured repository and all 9 Files lanes are admitted this way. A also admits a Files-hosted board this way.
- Nothing in D, A or A's self-test pins `2283b135-…`. The only occurrence is narrative text on the board.
- When Files is unmounted, `resolve(strict=True)` usually fails closed. But if `/media/jer/Files/CCF` were ever recreated on root while Files is unmounted (for example by a `mkdir -p` from any tool), admission would succeed **onto the root filesystem**, which the owner's no-root directive forbids.
- Separately, A1 line 85 says A "has stricter project-path admission". That is true for lanes: A closes D's `permit_off_storage_lanes` read-only bypass. But A **widens** board admission from ZX20-only to the Files CCF tree.

**Sufficiency of "by-id + UUID + fstype + mount target + unique UUID" for ZX20**, as the Q questions ask:
- It is sufficient as a **guard against mistakes**: kernel renumbering, a wrong disk, a duplicate visible clone, a remount elsewhere. A changed enclosure or USB bridge changes the by-id path and fails closed, which is correct.
- It is not an anti-adversary control:
  - a `dd` clone in an enclosure that reports the same model and serial passes while the original is detached;
  - the exFAT volume serial is 32 bits and can be set by user tools;
  - the check runs once at command start, so a long `lane-status` or acquire has a time-of-check/time-of-use window if the device is swapped mid-run.
- These are acceptable residuals for a single-user laptop, but they must be stated rather than implied away.
- No proposal weakens an existing negative case. A1 C1 correctly refuses to pin `/dev/sdc2`.

### Q-M2 — MEDIUM (A1 C2; parent §9.2 G1-d): the projected `unity-lock` keeps a free-text force-release that can clear an occupied lock

- In A (and so in both projections), `release --force` needs only a non-empty `--manager-approval` string. It skips the new `editors()` and recorded-PID checks that ordinary release now enforces.
- A's self-test exercises force only against stale or malformed fixtures. It has no case for "force while a live Editor or live recorded PID exists".
- This is not a regression from D, which has the same force gate. But C2 is described as "implementing accepted D-091 no-expiry", and parent G1-d calls it a "D-091 no-expiry install candidate". D-091 forbids forced lock clearing except through owner-directed cancellation or a genuine safety stop, and the tool cannot verify that.
- No proposed step uses force. The gap is in the negative-witness list and in the naming.

### Q-M3 — MEDIUM (parent §2; claim comment 6996340389): the independence description is no longer accurate

- The Q claim says Q's provider instance is "separate from the A2 instance". After A2's reroute, **A2 and Q are both on `claudeAgent_claude_b_pro`**, with Anthropic models (Sonnet 5.5 and Opus 5.5).
- Independence holds at the level of session and model: I am not a child of A1 or A2 (AgentWorkflow line 264). It does not hold at the level of provider or model family for the A2↔Q pair. For A1 (Codex/GPT-6.1 Sol) it is cross-provider.
- The parent's §2 wording ("different session and model from both") is true. It must not be read as cross-provider independence for A2, and the stale claim-comment wording must not be repeated.

### Q-L1 — LOW (parent §4.1): "Device identity has been relabelled twice"

Nothing was relabelled. The kernel name changed: `/dev/sda2` historically, `/dev/sdd2` as **reported** by #778 (not verified), and `/dev/sdc2` now. A1 found no evidence of a UUID or label change. Today `/dev/sdd` is the SD/MMC reader.

### Q-L2 — LOW (parent §3.1): typo and count drift

- "verified clean at `a42aeadc`" is not a Git object (`git rev-parse --verify` fails). The anchor HEAD is `a42eaadc`, which comment 6996473911 spells correctly.
- The parent says "25 T3 per-task checkouts … under `/home/jer/.t3/worktrees/`". At 01:29Z I count 23 directories, with 23 registered in the shared store; A2 also reports 23. Removing three from 25 would leave 22. Re-count with a timestamp, or say "about".

### Q-L3 — LOW (parent §4.4)

- "source is a clean 4-lane proposal" is wrong. A's `lanes.json` is a stale 4-lane source fixture: its NonUnity path `/media/jer/ZX20/CCF-NonUnity` is absent, and it has no U3 or Q10. It is not a registry and not a proposal for one.
- "changed … after its own receipt … without a refreshed receipt" needs A1's qualifier: lane edits fall outside the receipt's hash by design, and the non-lane projection still matches.

### Q-L4 — LOW (A2 Appendix C): `git fetch --dry-run` listed as read-only

A dry-run fetch does not update refs. It may still transfer objects into the shared object store. A2's main text wording ("no ref-updating `git fetch`") is accurate; Appendix C's "Read-only commands only" is slightly too strong.

### Q-N1 — NOTE (informational, for C6 / G1-c)

Deployed `lane-status --preflight` requires `zx20=True` (line 71). It can therefore never return READY for any Files-backed non-Unity lane: the 9 Files lanes, including the Multi and #690 custody lanes. `lane-status` cannot be the availability oracle for those lanes in its current form. This is not an overclaim by anyone.

### Verification limits: honest or smoothed over?

- **A1's wrapped `unity-lock status`** (E08) is a **legitimate, clearly labelled limitation**, not an overclaim. The wrapper changes only `mkdir` and the guard's `a+` → `r`, adds an audit hook, and keeps the original guard inode and `flock`. A1 states it is "not a literal unmodified CLI PASS" (line 30) and lists unmodified status under "Not directly verified" (line 115).
- **A1's 55 s timeout** is stated plainly, with a transcript, and is not converted into a completion claim.
- **A2** states that it ran no Ops command and inspected no lock or process state (L-08, U-02, U-08), and marks lock-free as "Plausible, not verified by A2".
- **A2's read-only `git worktree list`** is disclosed in the preface and the method appendix. It had no side effect beyond what any `lane-status` run also performs. This is acceptable as a recorded deviation, and no correction is required.
- The only smoothing over is in the **parent** (Q-H2).

### Does A2's conclusion hold?

**Yes.** Every specific fact the Q brief lists was independently verified (§3.1):
- the hand-edited trim of a stale October snapshot from the anchor branch;
- divergence from `main` at the 2026-10-12 merge-base `c1564b85`;
- three retained rows with Owner or Model changes and no recorded authority;
- 26 original lines not verbatim in the archive;
- "must not be committed".

Two nuances, neither requiring a change:
- "No recorded authority" is a negative finding bounded by what A2 searched; A2 frames it that way.
- The local notice's "board last reconciled 2026-10-16" matches the deployed board's own stale header sentence. A2's L-03 already separates the header from the rows.

A2's forward-only procedure respects D-091: keep by default, protect expiry text, never reclaim. It never touches the anchor and forbids using the trim as input. I found no step in it that could mutate a dirty tree or silently delete post-October ownership.

### Contradiction surface between A1 and A2: compatible

The figures do not conflict. They count different things:
- `lanes.json` has **25 lane entries** (16 ZX20 + 9 Files). A1 and A2 both count this, and I verified it.
- The board has **44 data rows** (A1's "44 table rows", A2's "44 task rows"). A2's "25 November rows" are the 25 rows with `CCF-4xx…7xx` IDs; the other 19 are October-lineage. I verified both counts.
- The two "25"s are a **coincidence of different populations**: lane registrations versus board task rows. They must not be conflated.
- "20" is a parent display artefact that only the parent attests. It is not a property of any file: `lanes.json` parses to 25. Its stated cause (piping through `head`) is plausible and unverified.

## 5. Statements that must be removed or reworded

**Parent draft (sha `3145a163…`):**

| # | Location | Current text (abridged) | Required change |
|---|---|---|---|
| P-1 | §4.1 heading | "Device identity has been relabelled twice" | "Kernel device name has changed (historical `/dev/sda2`; `/dev/sdd2` reported by #778, unverified; `/dev/sdc2` now); no UUID or label change found" (Q-L1) |
| P-2 | §4.1, last paragraph | "the deployed guard is correct precisely because …" | "the deployed ZX20 identity check fails closed on kernel renumbering because …; it does not pin the Files mount (Q-M1), and it does not stop a non-deployed copy from acting on live state (Q-H1)" |
| P-3 | §4.2 heading | "currently passes its own storage and lock readback" | "storage-only check passes; `unity-lock status` reported `free` (snapshot only, from a write-capable command)" |
| P-4 | §4.2 `unity-lock` bullet | unmodified run cited as evidence, exit 0 | mark it as a disclosed deviation (write-capable command), cite A1 E08 as the controlled readback, and cite Q §3.1's guard and history evidence |
| P-5 | §4.2 full-run bullet | "A full `lane-status` completes … all ZX20 lanes resolving to `/dev/sdc2` … independently confirmed … by A1" | "The parent reports one full run viewed through `head`; no complete transcript, exit code or elapsed time is retained, so completion is parent-attested and unverified. A1's bounded run timed out at 55 s. **25** is the `lanes.json` entry count, verified by A1 and Q." |
| P-6 | §4.3, last sentence | "the fail-closed behaviour is *correct*, and the stale pin is the defect" | add: "the stale pin is also currently the only thing stopping R's scripts from acting on the live lock state with a stale registry; it must not be 'fixed' in isolation (Q-H1)" |
| P-7 | §4.4 table, `lanes.json` row | "source is a clean 4-lane proposal" | "source is a stale 4-lane fixture (absent NonUnity path, no U3/Q10), not a registry" (Q-L3) |
| P-8 | §4.4, sentence after table | "changed on 2026-11-18 after its own receipt … without a refreshed receipt" | add "lane edits are outside the receipt hash by design; non-lane projection `ed642a52…` still matches" |
| P-9 | §9.1 heading and paragraph | "nothing needs to be 'fixed' in the deployed guard today" | "No change is needed to make `--storage-only` pass today." Then list the residuals: Files identity not pinned (Q-M1), non-deployed copies can act on live state (Q-H1), free-text force gate (Q-M2), status is write-capable (A1 C5) |
| P-10 | §5, `unity-lock` bullet | "that open did not alter any byte or record" | cite before/after evidence (e.g. Q §3.1) as "consistent with no byte change", or delete |
| P-11 | §6, last sentence | "which the parent resolved as its own `head` truncation artefact" | "which the parent attributes to its own `head` truncation (parent-attested, no transcript)" |
| P-12 | §9.2 G1-d | "D-091 no-expiry install candidate" | "no-expiry acquisition-mode candidate; D-091 compliance of the whole custody route is **not** established (force path, Q-M2)" |
| P-13 | §3.1 | "`a42aeadc`"; "25 T3 per-task checkouts" | `a42eaadc`; timestamped count (23 at 01:29Z) (Q-L2) |
| P-14 | §2 | Q/A2 independence | state that A2 and Q share provider instance `claudeAgent_claude_b_pro` and the Anthropic family; independence is by session and model; do not repeat the claim comment's "separate from the A2 instance" (Q-M3) |
| P-15 | §9.2 G1-a | "Repository-side Ops source sync … persistent by-id device, … correct current lane list supplied by the physical custodian" | rewrite per §6 A1-1: no tracked copy may become a second live registry. Before any copy's pin is corrected, the tools must refuse live-state mutation (and refuse to present their own lanes or board as live) when run from anywhere other than the deployed root, with a negative witness for each of R, A and a CCF-Temp copy |

I found nothing else in the parent draft that states or implies an installed fix, all owners absent, a lane safely reclaimable, or any G1/G2 permission. §8 and the "NOT GRANTED" markers in §9 are accurate.

**A1 (sha `07e9058e…`):** no sentence needs removing. The additions are in §6.
**A2 (sha `1d974d37…`):** one word-level rewording (A2-1 in §6).

## 6. Minimum bounded corrections, by author

**A1 (one bounded correction in its own lane and file):**
- **A1-1 (Q-H1).** Add a finding: non-deployed copies (R, A, F/O and the CCF-Temp replicas) share the fixed live `state/unity` path but use their own `lanes.json`/board. A's config appears to satisfy `config_read` today (static reading). R is blocked only by its stale pin. Add to C1 and C2 (and to C5/C6 where relevant) the negative witness: *"run from any non-deployed copy (repository checkout, #652 source checkout, CCF-Temp scratch), acquire/release/force must refuse to touch live state, and lane-status must refuse to present that copy's lanes/board as live; correcting R's storage pin is forbidden until that guard exists."* In C1, state that R's current failure is protective.
- **A1-2 (Q-M1).** Add as a stated residual, and as a C1 negative witness: *"Files unmounted, `/media/jer/Files/CCF` present on root, or wrong Files UUID/fstype must refuse repository, Files-lane and board admission."* Qualify line 85: A narrows lane admission but widens board admission to Files CCF. Add one sentence on the limits of the ZX20 checks (mistake guard, not anti-clone; checked once per command).
- **A1-3 (Q-M2).** Add to C2's negative witnesses: *"`--force` while a Unity Editor or live recorded PID exists"* as an explicit decision item. Note that the projection's force gate is free text that the tool cannot verify against D-091.

**A2 (one bounded correction in its own lane and file):**
- **A2-1 (Q-L4).** In Appendix C, reword "Read-only commands only: … `git fetch --dry-run`" to say that it updates no refs but may have added objects to the shared object store.

**Parent (before consolidating; not an author correction, but required for consolidation):** apply P-1 to P-15, carry Q-H1's interim operating rule as a **recommendation only**, and record this verdict in §7.
- **Interim operating rule:** until A1-1's guard exists, invoke only `/media/jer/ZX20/CCF-Ops/scripts/*`, never a source or scratch copy, for any status or lock operation.

**Delta recheck.** The same reviewer (this Q session, or failing that one recorded Q reroute with the same model and no authorship of A1 or A2) will perform **one** explicit delta recheck of the A1-1, A1-2, A1-3 and A2-1 corrections and of the parent's P-1 to P-15, against new pinned hashes.

## 7. Answers to the packet's Q questions, in brief

| Question | Answer |
|---|---|
| Can A1's C1–C7 admit a wrong device or mount, or bypass the fail-closed guard? | Not the wrong ZX20 device: by-id + `st_rdev` + UUID + fstype + target + unique UUID fails closed on renumbering, a duplicate clone or a remount. **But** the Files mount is unpinned (Q-M1), and C1 as the parent translated it (G1-a) would let a non-deployed copy pass the guard and act on live state (Q-H1). No proposal weakens an existing negative case. |
| Could any recovery release occupied Unity custody, take #690/#718/Multi #730 custody, or force a lock? | No proposed step does. C3 and G2-c preserve all 25 entries and carry #690/#710/#725/#755/#730 custody, and elapsed expiry text must not make a lane READY. The residual path is tool-level: a free-text `--force` that skips Editor/PID checks (Q-M2), reachable from non-deployed copies (Q-H1). |
| Conflict with D-091? | No proposal adds expiry, reclaim or forced clearing. Calling the projection a "D-091 candidate" overstates it (P-12). |
| Dirty tree, primary user work, post-October ownership? | No proposed step mutates the anchor. A2's procedure is keep-by-default and forbids using the trim as input. I re-hashed all seven protected anchor files at ~01:35Z. All match the parent's pins (`1bf4286b`, `6bebe495`, `ec842f26`, `fc8cff37`, `188fd66d`, `c2d9f78f`, `4b9faa3e`), and the anchor's `git status` still shows the same 3 modified and 4 untracked entries. |
| Source versus installed provenance honest; NO LIVE INSTALL respected? | Yes in A1 and A2: deployed `unity-lock` is old, deployed `_ccf.py` is older, the eight-file candidate is `NOT_DEPLOYED`, and the two projections are not installed. All of this is verified by me. Nothing was installed. The parent's G1-d naming is the only overstatement. |
| Verification limits honest? | A1: yes, the wrapper is a legitimate labelled limitation and the 55 s timeout is reported as a timeout. A2: yes, including the disclosed `git worktree list`. Parent: no (Q-H2). |
| A2's conclusion? | Holds. Not overstated (§4). |
| Parent overclaims? | Yes: P-1 to P-15, mainly Q-H2. |
| 20 / 25 / 44 contradiction? | Compatible: 25 lanes and 44 board rows (25 November + 19 October) are different populations, and 20 is a parent-only display artefact. |

## 8. Files changed by Q

Only this file: `Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-781-Q-INDEPENDENT-IDENTITY-AND-OWNERSHIP-REVIEW-20261118.md`, in one commit on `review/sen-1-781-q-ops-admission-20261118-v1`. Dashboard and Drive mirrors: NOT VERIFIED.

VERDICT: NEEDS CHANGES - for diagnostic and recovery design only
