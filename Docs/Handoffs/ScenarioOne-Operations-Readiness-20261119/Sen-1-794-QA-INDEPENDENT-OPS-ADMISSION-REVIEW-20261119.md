# #794 independent QA review: #792 containment-first Ops admission package

**Status: NEEDS CHANGES.** This reviews the **read-only admission package**: the #792 author document, the parent #794 evidence and the parent's G1 grant request. It is **not** a review of a frozen source candidate. No source candidate exists, because G1 has not been authorised. Nothing here approves any source change. G1 and G2 remain HELD.

## 1. Actor and environment

- **Actual actor:** Anthropic `claude-opus-5-5`, running in the Claude Code harness inside T3 Code. Host `jer-laptop`, Linux user `jer`, uid 1000, 2026-11-19 UTC. I did not author any reviewed material. I am not the #652 source writer and not the #QS source-candidate reviewer.
- **Claim comment:** https://github.com/jermur788/CCF/issues/794#issuecomment-7003768656
- **Worktree and branch:** `/media/jer/Videos/CCF-794-QA-Review`, branch `review/sen-1-794-qa-ops-admission-20261119-v1`, based on origin/main `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. This file is my only tracked output.
- **Scratch:** `/media/jer/Videos/CCF-794-Scratch/qa/`. Every command ran with `TMPDIR=/media/jer/Videos/CCF-794-Scratch/qa/tmp`. Videos is `/dev/sdb1`, ext4, UUID `60e276b9…`.
- **Not done:** no `unity-lock` subcommand against live or default state; no `--force` or `--manager-approval` against live state; no writes under ZX20, CCF-Primary, `/home` or `/tmp`; no source edits; no install or deploy; no Unity; no Windows; no packages.

## 2. Objects reviewed (exact identities)

| Object | Identity |
|---|---|
| #792 author document | commit `23b60fcd4b8fc7c216aa4e60cebe03dc3c724a39` (parent `a69b3e2e`), one file, 303 lines, blob sha256 `0d3492c0dfa9395376c3d950c2bf4dfc06ddac0bfa168c4a379274031b989bab` |
| Author receipt `…/Sen1-794-Ops-Containment-Admission-20261119/author/` | `harness.py` `92b8559a…`, `commands.json` `043a50dc…`, `result.json` `7fc7ea80…`, `receipt.json` `3c86263a…`, live-before = live-after `6c863937…` (these match the hashes the receipt itself records) |
| Parent evidence `…/parent/` | `harness.sh` `088ee016ea04…`, `harness-output.log` `1d017f9f7fc5…`, `storage-identity.txt` `57b38f39140c…`, `ops-and-live-witness.txt` `06d55887bc5c…` |
| Parent G1 grant request | #652 comment 7003714806 (posted 2026-11-19T08:21:13Z) |
| Admitted source | `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`, branch `ops/sen-1-652-unity-lease-readiness-20261116-v1`, HEAD `4c7acc051e262c3fe301049b51ee7fc48a9f99a2`, clean (0 porcelain lines). Hashes: `_ccf.py` `24a32146…`, `unity-lock` `8fa0c5ad…`, `self-test.py` `1bd03f15…`, `lanes.json` `f73d1c08…` |
| Deployed Ops (read only) | `/media/jer/ZX20/CCF-Ops`: `_ccf.py` `ab4343ef…`, `unity-lock` `54cf0a95…`, `lanes.json` `aa3d4658…`, `deployment.json` `8c66b290…`, `board/active.md` `0baf348d…`. All match the parent's 08:20Z witness. |
| Physical board row | `CCF-652-OPS-READINESS` = `READY / RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED`. Prospective writer: gpt-6.1-sol medium, session `9bcdfde6…`. Row HEAD field: `a9f56d63`. |

**Live state witness**

| | Listing | `history.jsonl` |
|---|---|---|
| Before (08:23Z) | `guard` (0 B) + `history.jsonl` only; no `lock.json` | sha256 `dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa`, 299478 B |
| After (this review's final check) | same listing; no `lock.json` | sha256 `dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa`, 299478 B |

The before and after values are identical.

## 3. Independent verification

### A. State constant re-derived from the bytes

```
$ PYTHONPATH=/media/jer/ZX20/CCF-Ops/scripts python3 -B -c "import runpy;m=runpy.run_path('/media/jer/ZX20/CCF-Ops/scripts/unity-lock');print(m['ROOT'],m['DEFAULT'])"
/media/jer/ZX20 /media/jer/ZX20/CCF-Ops/state/unity          [exit 0]
$ PYTHONPATH=<admitted>/CCF-Ops/scripts python3 -B -c "...run_path('<admitted>/CCF-Ops/scripts/unity-lock')..."
/media/jer/ZX20 /media/jer/ZX20/CCF-Ops/state/unity          [exit 0]
```

(`runpy` loads the module without running it as `__main__`. The deployed `__pycache__/_ccf.cpython-312.pyc` dates from 2026-11-11, so it was there before this review.)

From the source bytes (`unity-lock` lines 16, 98–114):

- The state path is `state = on_storage(a.test_state_dir or DEFAULT)`, where `DEFAULT = ROOT/'CCF-Ops/state/unity'`. **`--config` never chooses the state path.** It only decides whether `--test-state-dir` is mandatory.
- **`--test-state-dir` is optional** whenever `--config` resolves to the script's own sibling `../lanes.json`. It is required, and must be under `ROOT/CCF-Temp`, only for an *alternate* config. No environment variable overrides ROOT or DEFAULT.

**Additional result not in either document.** Run unmodified, the admitted source's own sibling config passes every pre-guard gate today:

```
$ PYTHONPATH=<admitted>/CCF-Ops/scripts python3 -B -c "import _ccf; cfg,board=_ccf.config_read(Path('<admitted>/CCF-Ops/lanes.json')); ...; _ccf.on_storage(ROOT/'CCF-Ops/state/unity')"
source sibling config_read OK; storage /media/jer/ZX20 /dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2
board resolves to /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops/board/active.md
on_storage(DEFAULT)= /media/jer/ZX20/CCF-Ops/state/unity                     [exit 0]
```

I ran the same read-only check on every `CCF-Ops/scripts/unity-lock` found within depth 5 under Files/CCF, CCF-Primary and Videos:

| Copy | Result |
|---|---|
| `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops` | **passes config_read; DEFAULT = live** |
| `/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops` | **passes config_read; DEFAULT = live** |
| `/media/jer/Files/CCF-Primary/CCF-Ops` | fails: `ZX20 mount identity mismatch` (stale `/dev/sda2` pin). This is the stale "protective" pin that the #794 packet warns about. |

So two unmodified, non-deployed copies would operate on the live state today if anyone ran them. That includes the checkout the G1 writer will branch from.

- Running `status` from either copy would open the live `guard` with `a+` and take an exclusive flock.
- Running `acquire --no-expiry` from the admitted source would write a record that the **deployed** reader classifies as malformed (see B).

None of these commands were executed. No `__pycache__` was created in any of those directories, and the source and LeaseFix checkouts are still clean.

### B. Force and approval path (source `unity-lock` 166–180)

- **What force requires:** `--force`, non-empty `--owner` and `--reason`, and a `--manager-approval` whose `.strip().lower()` is not one of `{'', 'none', 'n/a', '-'}`. Any other text is accepted; I verified that `ok` works.
- **What force does not check:** no identity, signature, nonce, expiry, issue reference or lock-ID binding. `--lock-id` is not required, so force removes whatever record exists at that moment (a time-of-check/time-of-use gap). Force does **not** call `editors()` or check the recorded PID. It appends to `history.jsonl` and then unlinks.
- **Ordinary release** compares three strings (owner, task_id, lock_id) against `lock.json`. It refuses while any Unity process is running or the recorded PID still exists (source only; the deployed bytes lack both refusals). The three strings identify the record; they do not authenticate the caller.
- **Who can read the identifiers:** on live ZX20, exFAT mount options make every file `-rwxr-xr-x`, so any local user can read `lock.json`. Mutation also needs write access to the state directory, so ordinary or forced release is open to **any same-uid process**, not to other uids.
- **Deployed vs source (verified by diff):** deployed `read_state` lists `eta` among the required non-empty strings, so it reads a source-format `eta:null, no_expiry:true` record as **malformed**. Deployed ordinary release refuses malformed records, so only force can clear them. The parent's rollback/upgrade hazard claim is supported by the bytes.

### C. Files admission (`on_durable_ccf`) and every caller

- `_ccf.py:36–41`: `Path(path).resolve()`, then `DURABLE_CCF.resolve(strict=True)`, then `is_relative_to`. This is a **resolved-prefix test only**: no fstype, UUID, device (`st_dev`/`st_rdev`), mountpoint or uniqueness check. Symlink escapes and sibling-prefix tricks such as `CCF-sibling` are rejected.
- `strict=True` means `/media/jer/Files/CCF` must exist. An empty, unmounted mountpoint therefore fails closed by accident. The real exposure is a populated directory tree on another filesystem at that path, or a different device mounted there.
- **Callers in the source:**
  - `_ccf.on_project_storage:47`, reached from `config_read` for the repository (`:129`), non-Unity lanes (`:146`) and the board (`:162`);
  - **`environment.sh:12`**, which checks `/media/jer/Files/CCF/{Verification,Builds}` before exporting paths. The author's caller inventory (B4) omits this one.
- **Related code that does not call `on_durable_ccf`:**
  - `lane-status:37` uses `DURABLE_CCF` for the `off_durable` label, which is prefix-only reporting.
  - `self-test.py:121,188–189` uses `DURABLE_CCF` for fixtures.
  - The deployed `_ccf.py` differs: it calls `on_durable_ccf` only for the repository, uses a bare `is_relative_to` for non-Unity lanes, and requires the board on ZX20.

### D. Independent hazard reproduction (both (i) and (ii))

- **Script:** `/media/jer/Videos/CCF-794-Scratch/qa/qa-repro.sh` (sha256 `7d6ba2f8…`). **Log:** `/media/jer/Videos/CCF-794-Scratch/qa/repro/repro.log` (sha256 `bbd36cd9…`).
- **Synthetic paths:** all under `/media/jer/Videos/CCF-794-Scratch/qa/repro/`. ROOT is `…/repro/synroot` and FILES_ROOT is `…/repro/fakefiles`. They are disjoint from the parent's `/media/jer/Videos/CCF-Ops` and the author's `…/author/synthetic-root`.
- **Copies:** two `cp -a` copies of the admitted source, at `repro/copies/one/CCF-Ops` and `repro/elsewhere/deeper/two/CCF-Ops`. `sed` changed only the `ROOT` and `FILES_ROOT` lines; the diff shows exactly two changed lines, and `unity-lock` is byte-identical to the source.
- **Safety stop:** the script aborts unless each copy's `DEFAULT` equals the synthetic path and no `/media/jer/ZX20` literal remains in its `_ccf.py`.
- **Fixture:** a synthetic Git repository with a worktree lane, plus a synthetic sibling `lanes.json` (Videos UUID, ext4, `/dev/sdb1`).
- **Scaffolding (same limitation as the author's):** a scratch-PATH `findmnt` wrapper rewrites only `target` to synroot for paths under synroot. Source device, fstype and UUID stay real, and `lsblk` uniqueness is real.

Command results (exit codes as observed):

| Step | Command | Exit | State after |
|---|---|---|---|
| runpy DEFAULT (both copies) | — | 0 | both resolve `…/repro/synroot/CCF-Ops/state/unity` |
| ONE `acquire --eta +45m` (no `--test-state-dir`, default config) | | 0 | `guard`, `lock.json` `abc5bd1d…` (mode 664) |
| TWO `status --json` | | 0 | sees ONE's custody |
| TWO `acquire --no-expiry` | | 2 | refused, `lock exists`: same lock universe |
| TWO `release --force --manager-approval "N/A"` | | 2 | refused (placeholder) |
| **TWO `release --force --owner qa-rogue --reason "QA: no approval exists" --manager-approval "ok"`** | | **0** | lock removed; `history.jsonl` `c8b8e273…` records `"manager_approval": "ok"` verbatim |
| ONE `acquire --no-expiry` | | 0 | `lock.json` `a3b654b7…` |
| **TWO ordinary `release` with owner/task/lock_id read from `lock.json`** | | **0** | lock removed; history `df471ad6…` |
| Q-M1: copy ONE `on_durable_ccf(fakefiles/CCF/nonunity-on-videos)` | | 0 | admitted; `st_dev` 2065 (Videos) vs real Files 2068: **different device** |
| Q-M1: copy ONE `config_read` with that non-Unity lane | | 0 | accepted; no Files fstype/UUID/device check runs |

Result: **7/7 checks passed.** Live `history.jsonl` was `dfb50644…` before and after the run, with no live `lock.json`. Scratch footprint was 427,448 bytes (`du -sb`).

**Q-M1 caveat:** substituting FILES_ROOT makes the device demonstration partly tautological. What it shows is that no device-identity code exists on that path. A real physical substitution (unmounting Files, or bind-mounting another device) needs root and was **NOT TESTED**. Section 3A covers the Q-H1 side without any substitution.

## 4. Consistency and overclaim (E, F)

### E. Parent 28/28 vs author 22/22

The two harnesses are different and neither result claims the other's. Both are consistent with the bytes; every hazard path they assert matches my reading and my reproduction.

- **Author 22:** 16 command-exit checks plus 6 state assertions. Seven of the commands are plumbing: the deployed-DEFAULT runpy, three git setup calls and two copy-DEFAULT runpys. The author says plainly that this is "not 22 independent production tests".
  - The `findmnt` wrapper rewrites `target` for *every* call, which is broader than needed. It is disclosed as scaffolding and is not a physical-mount qualification.
  - It **does not weaken** the Q-H1 claim: §3A shows the unmodified source passing `config_read` with no wrapper.
  - It does not touch Q-M1, because `on_durable_ccf` never calls `findmnt`.
  - It would mask any `storage_check` target mismatch, and no such qualification is claimed.
  - "Production suite: NOT RUN" and "Physical unmount/substitution: NOT TESTED" are stated. **The disclosure is adequate.**
- **Parent 28:** 2 git setup checks, 3 `test -e/-f` existence checks, 1 cleanup, and 8 baseline lifecycle/negative gates. The rest are hazard checks. Two of the four headline claims in the grant request are **not among the 28 assertions**: the Q-M1 prefix admission and the unmodified DEFAULT constant were only printed. The grant's "(28/28 checks, 0 failures)" placed above all four claims therefore overstates how they were tested. It also calls the copies "byte-copies", but `_ccf.py` was edited and `lanes.json` replaced. Both points are LOW and are disclosed inside `harness.sh`, but not in the grant text.
- **Parent scope deviation:** the parent harness used ROOT=`/media/jer/Videos`, a real mountpoint. Its synthetic state therefore lives at `/media/jer/Videos/CCF-Ops`, **outside** its declared scratch `/media/jer/Videos/CCF-794-Scratch`. The harness begins with `rm -rf "$ROOTV/CCF-Ops"`, an unconditional recursive delete at the top level of a shared disk. The directory still exists now (empty `state/unity`, mtime 08:19:07Z), so the log's final synthetic state cannot be re-observed. This contradicts the parent claim that every work product is in scratch, and sits poorly with the packet's "no deletes" rule.

### F. Does "approved installed executable/config identity" close Q-H1?

**No, not against a hostile same-uid actor. It closes only accidental cross-copy execution.**

- Any same-uid process can write `/media/jer/ZX20/CCF-Ops/state/unity/lock.json` or `history.jsonl` directly (exFAT gives no per-file protection).
- It can edit the installed script itself, run a copy that skips the check, or recreate any local marker.
- README already says the lock "is not a security boundary".

The two documents differ in how honestly they say this:

- **Author:** honest. "Same-uid writable scripts cannot provide a strong hostile-user boundary … containment protects accidental cross-copy execution."
- **Parent grant request:** does **not** say it. It describes A1a/A1b as binding to an admitted root and requiring a "verifiable approval artefact", with no threat-model limit. A signer could reasonably think hostile local access is being closed.

**Design ambiguity in both documents.** "Identity" must mean *installed location*, not content hash. If identity means a content hash, an unmodified byte copy of the deployed tree, which is exactly the Q-H1 case, would pass. A workable containment test, offered as a suggestion only, would be:

- `Path(__file__).resolve()` must sit under `ROOT/'CCF-Ops'/'scripts'`;
- the config must resolve to `ROOT/'CCF-Ops'/'lanes.json'`;
- both must be on ROOT's `st_dev`;
- any other location must use an explicit `--test-state-dir` under `CCF-Temp`, so a copy fails closed before `guarded()`.

**Q-M2 options and same-uid actors.** In this deployment no option distinguishes same-uid actors, unless key material is held off-host by the human owner.

- **Parent option (ii)**, a GitHub comment "verified against a pinned expected author": **every** actor here posts as `jermur788`. The Manager verdict 7003141766, the parent's comments, and this review's comments all share that login. An author pin cannot tell the Manager from any agent.
- **Option (i)**, signing: the parent says this "needs no new secret custody in the tool". That is false for HMAC, and Ed25519 still needs Manager-side key custody. A key on the same uid can be read by any agent.
- **Author option (1):** the author recommends it "conditional on key custody and replay-store design approval", which is the honest framing.

## 5. Allowlist vs physical board ceiling (G)

The board row's ceiling has two tiers:

- **D-091 ceiling (886bea5d):** `scripts/unity-lock`, `tests/self-test.py`, `README.md`, optional `roles/unity-operator.md`, and narrow `tests/fixtures/` paths only if proven necessary.
- **Retained prior ceiling, "only for distinct previously evidenced #652 defects":** `_ccf.py`, `lane-status`, `packet-lint`, `lanes.json`, `deployment.json`, `environment.sh`, `tests/fixtures/**`, and the 20261116 result document.

| Proposed file | Author #792 | Parent grant | Covered by ceiling? |
|---|---|---|---|
| `scripts/unity-lock` | yes | yes | yes (D-091) |
| `tests/self-test.py` | yes | yes | yes |
| `README.md` | yes | yes | yes |
| `roles/unity-operator.md` | yes | yes | yes (optional) |
| `scripts/_ccf.py` | yes (A1a/A1c) | yes | **No for Q-H1/Q-M1.** These are new #781 G0 defects, not "previously evidenced #652 defects". Needs an explicit Manager ceiling amendment. |
| `scripts/lane-status` | **not proposed** | yes | **Same amendment needed.** Only `:37` (prefix label) is relevant; justify at function level or drop. |
| `roles/execution-manager.md` | **yes** | no | **Not covered by any tier. Drop**, or get an explicit amendment. |
| `tests/fixtures/**` | no | yes (A1d, "stale literal `/dev/sda2` fixture") | **Drop.** The wildcard exceeds D-091's "narrow … if proven necessary", and the admitted source has **no** `/dev/sda2` literal anywhere under `tests/` (verified by grep; the author says the same). The stale literals are in the **deployed** `self-test.py:70,78,79,110`, which is not a source file. |
| `Docs/Handoffs/ScenarioOne-Operations-Readiness-20261119/**` | no | yes | **Not covered.** The ceiling names one 20261116 result file. Replace the wildcard with one exact result path and add it explicitly. |
| `environment.sh` | no | no | Covered only under the prior tier. No edit is needed if `on_durable_ccf` is fixed centrally, but A2 must test it (it is a caller). |

## 6. Naming and consistency defects (H)

These must be resolved before any grant is signed. The grant template still contains `<named actor>`.

| Item | #792 author document | Parent grant request (#652/7003714806) | Board row / parent claim |
|---|---|---|---|
| Proposed branch | `ops/sen-1-792-containment-first-g1-20261119-v1` | `ops/sen-1-794-containment-first-20261119-v1` | row branch `ops/sen-1-652-unity-lease-readiness-20261116-v1`; parent claim names none |
| Proposed worktree | `/media/jer/Videos/CCF-792-G1-Source` | `/media/jer/Videos/CCF-794-Source` | — |
| Base commit | **not stated**; the document never cites the admitted source HEAD | `4c7acc05` | row HEAD `a9f56d63` (stale: the source is 3 commits ahead, including code commit `4d03f5b5`). Neither document flags this. |
| Sole writer | "OpenAI gpt-6.1-sol / codex, subject to Manager appointment" (no effort or session given; the author's own thread ID is "unavailable") | "an OpenAI GPT-6.1 Sol/Codex native actor", with handoff from the `9bcdfde6` lineage | row: gpt-6.1-sol **medium**, session `9bcdfde6…` |
| Phase labels | A1d = self-test fixture observations; A2 = verification | A1d = stale `tests/fixtures/**` literal; A2 = negative tests | — |
| Q-M2 recommendation | option (1) offline signed one-use artifact | (ii) GitHub comment + (i) signature | — |
| Allowlist | includes `execution-manager.md`, excludes `lane-status` | includes `lane-status`, `fixtures/**`, `Docs/**` | — |

## 7. Claims audit (I)

- **No fix, install, deployment or source PASS is claimed by either document.**
  - The author states "No fix, integration or deployment acceptance is claimed".
  - The parent reports "SOURCE BLOCKED".
- **The parent's "no protected byte modified" claim is consistent with my observation.** Live history, board, lanes.json, deployment.json and deployed scripts all match the parent's 08:20Z witness hashes.
- **Claims not supported as written:**
  - the grant's "28/28 checks" framing over claims that were not asserted (E);
  - "byte-copies" (E);
  - "(ii)+(i) needs no new secret custody" (F);
  - the parent's A1d premise that a stale fixture literal exists in the source (G).

## 8. Findings

| # | Severity | Finding | Evidence |
|---|---|---|---|
| F-1 | **HIGH** | The author document and the grant request disagree on branch, worktree, base, writer identity, phase labels, allowlist and Q-M2 recommendation. A grant signed against one would not match the evidence in the other. | §6 |
| F-2 | **HIGH** | The allowlist exceeds the physical board ceiling. `roles/execution-manager.md` and `Docs/Handoffs/…/**` are uncovered. `tests/fixtures/**` exceeds the narrow-fixture terms. `_ccf.py` and `lane-status` are retained only for *previously evidenced* defects, and Q-H1/Q-M1 are new. Neither document asks for an explicit ceiling amendment. | §5; board row text |
| F-3 | **HIGH** | The package does not inventory the copies that can reach live state today, and gives no interim containment instruction. The admitted source checkout (the proposed G1 base) and `CCF-OpsLeaseFix-20261115` both pass `config_read` unmodified and resolve DEFAULT to live state. Running `status` from them write-opens and flocks the live guard. A source `acquire --no-expiry` would create a lease the deployed reader treats as malformed. The Q-H1 demonstrations used patched copies only. | §3A |
| F-4 | MEDIUM | The grant request does not state that containment closes only accidental cross-copy execution and not hostile same-uid access. The author document does. | §4F |
| F-5 | MEDIUM | The parent's Q-M2 recommendation is unsound here. Every actor posts as `jermur788`, so an author-pinned GitHub comment authenticates no one. "No new secret custody" is wrong for HMAC. It also diverges from the author's recommendation. | §4F |
| F-6 | MEDIUM | "Approved installed executable/config identity" is ambiguous. A content-hash identity would let an unmodified byte copy through (the Q-H1 case). It must be pinned as installed *location* plus device, with `--test-state-dir` required everywhere else. | §4F |
| F-7 | MEDIUM | The parent's A1d targets a nonexistent source defect: no `/dev/sda2` literal exists under the source's `tests/`. The stale literals are in deployed `self-test.py`. | §5 |
| F-8 | MEDIUM | The author document never pins the admitted source commit or branch, or a base for the proposed G1 branch. It gives file hashes only. Neither document flags that the board row HEAD `a9f56d63` is stale against source `4c7acc05`. | §6 |
| F-9 | MEDIUM | The parent harness wrote synthetic state to `/media/jer/Videos/CCF-Ops`, outside its declared scratch, and runs an unconditional `rm -rf` there. Residue remains and is undisclosed in the grant/claim. | §4E |
| F-10 | LOW | The grant's "28/28" framing covers two claims that were only printed, not asserted, and calls the copies "byte-copies" although `_ccf.py` was edited. | §4E |
| F-11 | LOW | The Q-M1 demos (both harnesses) are tautological under FILES_ROOT substitution. The "unmounted Files" scenario needs a populated `/media/jer/Files/CCF` tree because of `strict=True`. Physical substitution was not tested. Wording should be made precise. | §3C, §3D |
| F-12 | LOW | The author's `on_durable_ccf` caller inventory omits `environment.sh:12`. | §3C |
| F-13 | LOW | The parent never mentions that force skips the Editor/PID safety check, or that force is not bound to a lock ID. The author covers both. Also, `status` takes an exclusive flock on the live guard, so it is not just an `a+` open: a contention risk worth stating. | §3B |

**Counts: HIGH 3 / MEDIUM 6 / LOW 4.**

## 9. Bounded corrections required before a G1 grant is signed

These are documentation-only. No source, board, lanes or state change is implied.

1. Publish **one reconciled grant text**, readback-verified, that both the #792 author and the #794 parent endorse. It must name: one branch; one worktree; base `4c7acc051e262c3fe301049b51ee7fc48a9f99a2`; the exact writer (provider, model, effort, session ID), including whether custody transfers from session `9bcdfde6`; one set of phase labels; and the board row HEAD/branch refresh the custodian must make (F-1, F-8).
2. Restate the allowlist as **exact file paths** with a function-level reason for each. Drop `tests/fixtures/**` and `roles/execution-manager.md`, or justify them concretely. Replace `Docs/Handoffs/…/**` with one exact result path. Label `_ccf.py`, `lane-status` and the result path as an **explicit ceiling amendment** to Manager #731 amendment 886bea5d covering new defects Q-H1/Q-M1 (F-2, F-7).
3. Add the inventory of live-capable copies from §3A, and an **interim containment instruction** pending G2: never run `unity-lock` (including `status`) or `environment.sh` from `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116` or `CCF-OpsLeaseFix-20261115` without `--test-state-dir`. Require the G1 writer's A2 harness to prove the candidate fails closed when run from a Files/CCF checkout (F-3).
4. In the grant text, state the threat model: containment prevents accidental cross-copy execution; it is not a same-uid security boundary. Define "identity" as installed location plus device, not content hash (F-4, F-6).
5. Withdraw the "(ii)+(i), no new secret custody" recommendation, or correct it. Record that a GitHub author pin cannot authenticate anyone because all actors post as `jermur788`. Leave Q-M2 as an open owner decision, with `--force` failing closed until it is decided (F-5).
6. The parent should disclose the out-of-scratch synthetic root `/media/jer/Videos/CCF-Ops` and the `rm -rf` in its result, and leave removal to the owner. Correct the "28/28" and "byte-copies" wording (F-9, F-10).
7. Make the minor wording fixes F-11 through F-13. These are non-blocking but should go into the same revision.

## 10. Verdict

**NEEDS CHANGES.** This verdict applies strictly to the admission package as evidence for a future G1 grant.

The technical diagnosis holds: Q-H1, Q-M2, Q-M1 and the guard write are all confirmed independently from the bytes and by my own reproduction (§3D, 7/7). The package is not yet fit to support a signed grant, because:

- the two documents propose different branches, worktrees, writers and allowlists;
- the allowlist exceeds the physical board ceiling without an amendment request;
- the copies that can reach live state today, including the proposed base checkout, are not inventoried or contained.

No source change is approved; none exists. G1 and G2 remain HELD.
