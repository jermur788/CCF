# Multi #885 — one-shot DeepSeek orchestration: Stage I closure attempt, Q1 harness authoring, and precise gate-blocked status

**Role:** DeepSeek V4.1 Flash — **orchestrator only**. Not the D-091 custodian, not a source implementer, not the Manager approval authority, not the independent reviewer, not the Q1 run worker.

**Issued:** 2026-11-21 (observed clock, WSL2 `DESKTOP-DL1BQI7`).
**Binding dispatch:** [one-shot DeepSeek Stage I → Q2 execution dispatch](https://github.com/jermur788/CCF/blob/docs/multi-885-oneshot-deepseek-stage1-to-q2-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-ONE-SHOT-DEEPSEEK-STAGE-I-CLOSURE-TO-Q2-RESULTS-DISPATCH-20261121.md).

> **Headline: no multi-year simulation was run, and none may lawfully have been run.** Stage A succeeded with a real custodian grant. Stage B is stopped by one missing operating-system package that only the owner may install. Stage C (the Manager Stage I verdict) and Stage D's run approval are external authorities that no agent in this lane may issue. Stages E and F therefore have no lawful input. Everything that could be produced without those authorities was produced: the custodian addendum, a complete frozen-input verification, the executable Q1 harness bytes with a SHA-256 manifest, and this reconciliation.

---

## 1. What this document is, and is not

| Claim | Status |
|---|---|
| Custodian addendum for the disposable P0 lane and ephemeral ports | **VERIFIED** — issued by the custodian, read back by me |
| P0 isolation L0–L4 executed | **NOT RUN** — blocked on the `strace` prerequisite (owner-executed install, custodian-authorised) |
| Stage I technical qualification | **STILL NEEDS CHANGES** — unchanged; nobody in this lane may approve it |
| Q1 harness bytes frozen and disclosed | **PRODUCED** — authored, statically verified; **NOT executed, NOT runnable-verified** |
| Q1 run approval | **NOT GRANTED** — Manager authority, not obtained |
| Q1 physical execution lease | **NOT GRANTED** — reserved and explicitly **not usable** by the custodian |
| Multi-year simulation results | **NONE EXIST** — zero sessions, zero years, zero trees measured |
| Independent review | **NOT STARTED** — there is nothing to review |
| Owner G2 player acceptance | **PENDING** — untouched by this session |

Nothing in this document is a simulation result, an approval, an integration into `main`, or an owner acceptance.

---

## 2. Live facts I verified myself before doing anything

Observed `2026-11-21T20:25:00Z` and re-checked through the session, all read-only.

| Fact | Value |
|---|---|
| Host | `DESKTOP-DL1BQI7`, Linux `6.18.58.1-microsoft-standard-WSL2`, `jer` uid 1000 |
| OS | **Ubuntu 26.04.1 LTS (Resolute Raccoon)** — *newer than the Ubuntu 24.04 profile several frozen expectations were pinned to* |
| Disk | `/dev/sdd` on `/`, 1007 G total, 879 G available; **`/tmp` is a separate 3.9 G tmpfs** |
| Protected `#877` | PID **1750123 ALIVE**, `127.0.0.1:33767`, cwd `CCF-Review-877/wt-owner/...` |
| Protected `#882` | PID **1806361 ALIVE**, `127.0.0.1:43649`, cwd `CCF-Multi-882-Launch/...` |
| `#867` incident | PID 390578 **ABSENT** — untouched |
| Integration lane | `CCF-Multi-885-Integration-20261121` at **`4cecf87339bea4d32907955a8d5625d1f27f96bd`**, branch `integration/multi-885-ma1-two-area-20261121-v1`, `git status` **0 lines**, `.p0ref` read-only |
| Origin ref | `refs/heads/integration/multi-885-ma1-two-area-20261121-v1` = `4cecf873…` (`git ls-remote`) |
| P0 subtree at candidate | `git rev-parse 4cecf873:Tools/MultiRackSitkaLab` = **`f212c8f64fb28b83aa870c7b5b7c72e03051e46a`** |
| P0 reference pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (`src/Core/SitkaP1.Core.csproj`) |
| Manifest hash pin | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` (`MultiAreaTests.cs:20`, multi-area smoke `:30`) |
| Seeds (source-verified, not assumed) | MA-A **885101**, MA-B **885202**, generator **8850001** (`MultiAreaGenerator.cs:28,41,45`) |
| Startup line (source-verified) | `Two-area MA-1 simulator: http://127.0.0.1:{port}/s/{capability}/multi` (`Program.cs:75`); `kestrel.Listen(IPAddress.Loopback, 0)` (`Program.cs:54`) |
| Serialisation | responses camelCase, enums as strings, and non-finite floats emitted as **named literals, not thrown** (`PlayerApi.cs:53-58`) |
| SDK | `/home/jer/.dotnet/dotnet` **8.0.134**; system `dotnet` on PATH **10.0.115** (must not be used) |
| **`strace`** | **ABSENT** — no binary, no package, `sudo -n` refuses (`interactive authentication is required`) |
| Toolchain | python **3.14.4**; `sha256sum` is **uutils coreutils 0.10.0**; bash **5.3.9**; gcc/make present |

**The Ubuntu-version fact matters.** The frozen L4 self-test pins its expected syscall identities to "this host's coreutils/bash/python syscall choices (Ubuntu 24.04), observed in #644". This host is 26.04 with a Rust coreutils and python 3.14. A self-test failure here is a **harness/host incompatibility to report**, not a licence to edit the harness.

---

## 3. Stage A — D-091 custodian addendum: **PASSED**

I did not issue this grant and could not have. I routed the request to the session that holds the owner's recorded D-091 custodian appointment (T3 thread `a8d35599-48af-4f1b-a652-26f5ff44da5e`, which issued the original admission receipt). That session re-measured the host itself and issued a narrow addendum; I then read the published file back and checked each clause against my own observations.

**Addendum:** [Multi-885-D091-CUSTODIAN-ADDENDUM-DISPOSABLE-P0-LANE-AND-EPHEMERAL-PORTS-20261121.md](https://github.com/jermur788/CCF/blob/docs/multi-885-d091-addendum-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-CUSTODIAN-ADDENDUM-DISPOSABLE-P0-LANE-AND-EPHEMERAL-PORTS-20261121.md)
Branch `docs/multi-885-d091-addendum-20261121-v1`, commit **`efb188ace4dc5489c7a0c8852d5cf5e323280335`**, one added Markdown, from live `main` `3a67d098`.

What it actually grants, in its own terms:

| Item | Disposition |
|---|---|
| Disposable P0 isolation worktree | **GRANTED** — `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121`, **detached HEAD** at exactly `4cecf873`, own `.p0ref`/`bin`/`obj`/`TMPDIR`; run **only** the unchanged `tests/p0-isolation.sh` there; clean only its own outputs |
| Fixed ports `33811`/`33841` | **WITHDRAWN** and replaced programme-wide by loopback-only OS-assigned ephemeral ports, with endpoint/PID/session/SHA recorded per run; never `:33767` or `:43649`; stop only your own PID |
| Q1 qualification scratch | **RESERVED — NOT USABLE** until a distinct Q1 packet exists |
| Harness-authoring scratch | **GRANTED as physical space only** — `/home/jer/Development/CCF-885-HarnessAuthor-Scratch-20261121`, `bash -n` / `python3 -m py_compile` only; no engine, session, listener or build |
| `strace` prerequisite | **(a) AUTHORISED — owner-executed only**: exactly `strace 6.19+ds-0ubuntu5` from `archive.ubuntu.com`, installed by the owner with the owner's own interactive sudo; **no agent may run the install or hold the password** |
| Preserved lanes | `#877`, `#882`, `#867`, integration, author (`ac4c7b51`), reviewer (`58950a9a`) — all explicitly preserved |

Two risks the custodian recorded and refused to resolve by editing anything:

1. The L4 **self-test may legitimately fail** on this host's 26.04 / python 3.14 / uutils coreutils profile. That is a finding, not a repair authorisation.
2. The regenerated disposable `.p0ref` must hash to `9a99f781…e1934b3` and the tracked P0 subtree must stay `f212c8f6…051e46a`; a mismatch is a finding.

The addendum also restates something worth keeping straight: the **P1 spec §3.5 / README "never rebuilt on this path"** wording is a **Manager interpretation**, not something a custody grant can settle. The custodian's reading is that the D-091 receipt forbids rebuilding *except through* the pinned `build-p0-core-ref.sh`, which is exactly the script `p0-isolation.sh` invokes, and that the disposable lane only changes *whose* `.p0ref` is regenerated.

---

## 4. Stage B — P0 isolation execution: **BLOCKED, NOT DISPATCHED**

The custodian sequenced the lane explicitly: the executor creates the worktree **once the owner has settled §3**. §3 is not settled, because the one required package is absent and only the owner may install it.

What is missing, precisely:

```
strace        : not installed (no binary, no dpkg entry)
apt candidate : 6.19+ds-0ubuntu5   (archive.ubuntu.com reachable, HTTP 200)
sudo -n       : "interactive authentication is required"
```

The owner has been given the exact command. Nothing else is missing for Stage B: the worktree path is granted and still absent, the candidate is present and clean, and the executor has deliberately **not** been dispatched, because dispatching it now would be a launch against a known blocker.

**If the owner installs the pinned package**, Stage B proceeds as one bounded Sol/Codex task: create the granted detached worktree at `4cecf873`, verify byte identity and the P0 tree hash, put `/home/jer/.dotnet` first on `PATH` so the harness's bare `dotnet` resolves to 8.0.134, run `tests/p0-isolation.sh` **unchanged**, and publish real L0–L4 output including the generated P0 hash, the tracked-P0 before/after comparison, the self-test result, process cleanup and every bound endpoint.

**If the owner declines**, Stage B is recorded `BLOCKED-ON-PREREQUISITE` and no substitute is improvised — no `ltrace` swap, no rewritten harness, no relaxed L4 criterion.

**Prediction I am putting on record now, so it cannot be quietly reinterpreted later:** the most likely non-trivial outcome is that L0–L3 pass again and the L4 **self-test** fails on this host's 26.04 / python 3.14 / uutils-coreutils syscall profile, producing `OVERALL: FAIL`. That would leave Stage I open and require a Manager decision about the harness's pinned expectations — a test-fixture question, not a product defect, and not something any executor may fix on its own initiative.

---

## 5. Stage C — Manager Stage I verdict: **NOT OBTAINED (external authority)**

I cannot approve Stage I, and did not. The Manager's own ruling is explicit that Stage I closure may become *APPROVED WITH N1 FOLLOW-UP OPEN* only after a real L4 PASS. With L4 unrun, the correct state is unchanged: **NEEDS CHANGES**, four gates PASS, two incomplete.

### Exact request to Manager #885 (copy-paste)

> **#885 Manager — Stage I closure request after the custodian addendum.** Custodian addendum published: `efb188ac` ([addendum](https://github.com/jermur788/CCF/blob/docs/multi-885-d091-addendum-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-CUSTODIAN-ADDENDUM-DISPOSABLE-P0-LANE-AND-EPHEMERAL-PORTS-20261121.md)) — disposable exact-`4cecf873` lane granted, fixed ports withdrawn in favour of loopback OS-ephemeral, Q1 scratch reserved and **not** usable, harness-authoring scratch granted for syntax checks only. **New environmental facts:** the host is Ubuntu **26.04.1** with python 3.14.4 and uutils coreutils 0.10.0, and `strace` was **not installed at all**; the custodian authorised exactly one pinned package (`6.19+ds-0ubuntu5`), owner-executed only. L4 therefore remains **NOT RUN** at the time of this request. If the owner installs it, a separate Sol executor runs the unchanged harness in the disposable lane and publishes the real L0–L4 result; the self-test may legitimately fail on this host's toolchain profile. Please record: (1) whether a **harness/host incompatibility** outcome is a Stage I disposition distinct from a product defect, and which of *bounded test-fixture update packet* / *L4 permanently NOT VERIFIED* / *pinned toolchain requirement* you would require if the self-test fails; (2) that the P1 spec §3.5 "never rebuilt on this path" wording remains your interpretation, since the custodian declined to overrule it; (3) that N1 naturally-empty remains **deferred, not waived**. No Stage I approval is claimed or implied anywhere in this lane.

---

## 6. Stage D — Q1 harness bytes: **PRODUCED, NOT EXECUTED**

### Who produced it, and what that does and does not mean

| Role | Actor | Did |
|---|---|---|
| Harness **author of record** | me, DeepSeek V4.1 Flash orchestrator | specified the task from the frozen packet and the real API source, re-hashed every file independently, and read the selection, driver and request-body code myself |
| Harness **byte author** | a separate **Claude Opus 5.5** session (`claudeAgent`, effort high), delegated by me | wrote the seven files into the custodian-granted scratch |
| Run worker | **nobody** | not assigned; the Q1 lease is not granted |
| Reviewer | **nobody** | nothing executed to review |

Neither the byte author nor I may be the Q1 run worker, and neither of us may be the independent reviewer of results these bytes produce. That separation is now recorded and must survive into Stage E.

### What was verified, and what was not

**Verified by me, independently of the author's claims** (`verify-harness.sh`, read-only, no engine started):

- all seven files present; `bash -n` on both shell files and `python3 -m py_compile` on all three Python files pass;
- `sha256sum -c SHA256SUMS` verifies, and my own recomputed digests agree with the author's manifest — **no discrepancy**;
- all eleven `api/multi/...` routes used by the harness exist as quoted literals in `PlayerMultiApi.cs:61-80`;
- all twelve fence source-token members are present;
- the four frozen pins (`manifest c66d566c…`, seeds `885101`/`885202`/`8850001`) appear;
- **no** `pkill`, `pgrep -`, fixed-port `33811`/`33841`, `5187`, `git push`, `git commit`, `apt-get` or `sudo` anywhere;
- all seven scenario identifiers are wired;
- only `q1-driver.sh` launches the engine — no Python component does;
- the selection implements float32 emulation, the 8 m cutoffs, rank layers, first-occurrence dedup without replacement, the Hegyi floors and the Crop-on-competitor-list defect abort;
- the driver echoes a fence read immediately before every POST, never approves an empty order set, dumps the disclosure before Execute, stops only a PID it has re-verified as its own, and enforces both budgets.

I also read the three riskiest routines in full rather than trusting a grep: `q1-select.py`'s float32 distance and Hegyi arithmetic, `q1-driver.sh`'s process-ownership and POST discipline, and `q1-record.py`'s request-body builder — the last of which requires the view's source token to match the twelve-member set **exactly**, refuses any null member, and aborts the run otherwise. That is the correct fail-closed behaviour for the most fragile part of the protocol.

**Not verified — and it cannot be, here:** no engine was started, so no capability URL was parsed, no fence was ever accepted by the real guard, no response was parsed, no float bits were recovered from real JSON, no replay was compared, no SIGINT stop was observed, and the built-in selection `selftest` never ran. `SHA256SUMS` proves byte identity, not correctness. **These bytes are AUTHORED, not runnable-verified.**

### Disclosed assumptions and one open design question

The README lists fifteen source-anchored assumptions with `file:line` references and contradictions C1–C9. The ones that matter to a reviewer:

1. **C2 — S's year-10 Crop set. This needs a Manager answer.** The dispatch to me said S's year-10 Crop set "may differ". Q0 revision 3 §6 steps 5(a) and 10 say S keeps the **surviving year-0 Crops and adds none**, and the engine both refuses re-marking (`CropAlreadyMarked`) and refuses ordering a still-marked tree (`CropTreeRetained`). The harness therefore implements Q0 step 10 and recomputes the competitor lists and removal sequence from the year-10 state against that surviving Crop set. This is the defensible reading, but it is an **interpretation of the frozen protocol, not a test of it**, and a Manager should confirm it before the run is approved.
2. **C1 — Origin.** The guard admits `http://127.0.0.1:<port>` with the scheme (`PlayerApi.cs:108`). The harness sends that form; a bare `127.0.0.1:<port>` would be refused 403.
3. **C4 — projection.** Cross-repeat comparison is a canonical re-emission of the **HTTP JSON** with member order and exact number text kept. It is not Q0's default-options serialisation, because over HTTP only the UI serialiser exists.
4. **C5 — replay.** The engine accepts a replay only with the original source token, so the replay probe and `q1-replay.sh` re-send the recorded body and substitute only the session id. This is consistent with "never cache a fence" because it is a replay of a recorded exchange, not a fresh command.
5. **C6 — Q0 "Repeats (ii)" is not implemented**: it requires the internal types, i.e. a repository code addition, which this programme forbids.
6. **C7 — empty-stand Dq.** The engine reports `0` for an empty area; the readable column shows `null` per the null rule and the engine's bits are kept alongside.
7. **C9 — W12 runs the legacy CLI**, which is an engine run and needs the same authorisation as the rest; it is off by default.

### One correction I owe the byte author

The author reported that my verifier failed one check and that the fault was mine. It was: I wrote `grep -qE 'noLivingStems\|NoLivingStems'`, and under `-E` a backslash-escaped pipe is a literal pipe character. I fixed my verifier and it now passes; the harness was never at fault. The author correctly declined to insert a dummy string to satisfy a broken check.

For the record on a second point: an **empty regular file** existed at the granted scratch path when the author started. That file was mine — created accidentally by a `touch` in one of my own read-only probes — and the author removed it only after confirming it was empty and not a link. It was not a marker, and nothing was lost.

### Files and hashes

Hashes below were recomputed by the orchestrator from the files themselves; they are not copied from the author's manifest.

| File | Lines | Bytes | SHA-256 (independently computed) |
|---|---|---|---|
| `q1-driver.sh` | 608 | 30684 | `37f2c823fc06dc44cfc0ee023ca4894b141c45a02a1fffaf54acc9b95e7186a0` |
| `q1-select.py` | 363 | 16232 | `b54e30ea735730a90e31b102c821c6f69deda553e2c0d412592974bbd5944e70` |
| `q1-record.py` | 1145 | 59315 | `4de1c3c12564dd7d00e52343a5bf6ef773ca797db049453a7ca72f14e68844d0` |
| `q1-analyse.py` | 759 | 39486 | `b7f8740809617228d5cd14c386e36412108897ab42ff8cfcf11bfdf9b15b6165` |
| `q1-replay.sh` | 84 | 4584 | `6bf673154164eb21291363a699be7d8ef48425f9ffbc6a6a2a284b2427736d13` |
| `README.md` | 301 | 23984 | `5a6747e70e8f3ece5e61b9b9d0ceba1beb4eea9041f0b854d7beca9a7cab574e` |
| `SHA256SUMS` | 6 | 472 | `329464988d7d807ef0490bee49ea96aeaddfac37b3bd6f116f2882ef01c4ab47` |

The author's `SHA256SUMS` agrees with every recomputed digest (`sha256sum -c` verified separately).

### `SHA256SUMS`

````text
37f2c823fc06dc44cfc0ee023ca4894b141c45a02a1fffaf54acc9b95e7186a0  q1-driver.sh
b54e30ea735730a90e31b102c821c6f69deda553e2c0d412592974bbd5944e70  q1-select.py
4de1c3c12564dd7d00e52343a5bf6ef773ca797db049453a7ca72f14e68844d0  q1-record.py
b7f8740809617228d5cd14c386e36412108897ab42ff8cfcf11bfdf9b15b6165  q1-analyse.py
6bf673154164eb21291363a699be7d8ef48425f9ffbc6a6a2a284b2427736d13  q1-replay.sh
5a6747e70e8f3ece5e61b9b9d0ceba1beb4eea9041f0b854d7beca9a7cab574e  README.md
````


### Full bytes

### `q1-driver.sh` — 608 lines, 30684 bytes

SHA-256 `37f2c823fc06dc44cfc0ee023ca4894b141c45a02a1fffaf54acc9b95e7186a0`

````bash
#!/usr/bin/env bash
# q1-driver.sh -- Multi #885 Q1 multi-year simulation driver: process lifecycle + ordered transcript.
#
# AUTHORED, NOT EXECUTED. Scratch harness component; not a reviewed repository feature. Not approved to run:
# the Q1 packet is BLOCKED until its section 10 conditions clear and a distinct run worker is assigned.
#
# One fresh UI process (= one fresh in-memory session) per (scenario, repeat). Every request goes through curl with
# --noproxy '*', Host 127.0.0.1:<port> and, for POST, Origin http://127.0.0.1:<port> (PlayerApi.cs:107-108).
# Every POST body echoes the fence of the view read immediately before it (q1-record.py body); every exchange is
# appended to transcript.jsonl and checked live (q1-record.py exchange). The driver never re-sorts, never edits a
# seed, rule, fixture, source file or expectation, and never extends the horizon.
#
# Usage:
#   q1-driver.sh run   --run-dir DIR [--scenarios "C L M D S S1 I"] [--probes] [--w12] [--lab DIR] [--dotnet PATH]
#   q1-driver.sh probes --run-dir DIR [--lab DIR] [--dotnet PATH]      (optional valid-case probes only)
#   q1-driver.sh w12    --run-dir DIR [--lab DIR] [--dotnet PATH]      (README drift: both CLI annual forms once)
# Exit: 0 run finished (verdict in run-verdict.json); 2 usage/preflight; 20 run ABORTED (INCOMPLETE, see ABORTED.json).
# Sourcing this file (q1-replay.sh does) defines the functions without running anything.
set -euo pipefail

HARNESS_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd -P)"
PY=(python3 -I -B)
DEFAULT_LAB="/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121/Tools/MultiRackSitkaP1Lab"
LAB="$DEFAULT_LAB"
DOTNET_BIN="/home/jer/.dotnet/dotnet"
EXPECTED_DOTNET_VERSION="8.0.134"
EXPECTED_SOURCE_SHA="4cecf87339bea4d32907955a8d5625d1f27f96bd"
PINNED_MANIFEST_HASH="c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95"
PINNED_SEEDS="MA-A 885101, MA-B 885202, generator 8850001"   # checked by q1-record.py against every view/manifest
FIXTURE_NAME="p1d-30m-sitka-adult-v1.sitkap1fx.json"
PROTECTED_PIDS=(1750123 1806361)      # #877 and #882: never signalled, never touched
PROTECTED_PORTS=(33767 43649)
FORBIDDEN_ROOTS=("/home/jer/Development/CCF-HP" "/home/jer/Development/CCF-HP-Worktrees")
HORIZON=50
YEAR_BUDGET_US=$((60 * 1000000))
SCENARIO_BUDGET_US=$((30 * 60 * 1000000))
CANONICAL_ORDER=("C" "L" "M" "D" "S" "S1" "I")
HARNESS_FILES=(q1-driver.sh q1-select.py q1-record.py q1-analyse.py q1-replay.sh README.md)

RUN_DIR=""
H=""                 # the harness copy inside the bundle (what actually runs)
FIXTURE_DIR=""
UI_DLL=""
CLI_DLL=""
SERVER_PID=""
SERVER_LABEL=""
PORT=""
CAP=""
SDIR=""
SEQ=0
LAST_RESP=""
LAST_STATUS=""
LAST_VIEW=""
SESSION_OUTCOME=""
SESSION_CAUSE=""
LAST_YEAR=0
SESSION_T0=0
DIE_FILE=""
LOCK_HELD=0
STOP_CLEAN=""

# ------------------------------------------------------------------------------------------------ small helpers

now_us() { local t="${EPOCHREALTIME//[!0-9]/}"; printf '%s' "$((10#$t))"; }
utc() { date -u +%Y-%m-%dT%H:%M:%SZ; }
log() { local m; m="$(utc) $*"; printf '%s\n' "$m" >&2; if [[ -n "$RUN_DIR" && -d "$RUN_DIR/logs" ]]; then printf '%s\n' "$m" >> "$RUN_DIR/logs/driver.log"; fi; }
json_str() {
  local s="$1"
  s="${s//\\/\\\\}"; s="${s//\"/\\\"}"; s="${s//$'\n'/\\n}"; s="${s//$'\r'/\\r}"; s="${s//$'\t'/\\t}"
  printf '"%s"' "$s"
}
rec() { "${PY[@]}" "$H/q1-record.py" "$@"; }
in_list() { local x="$1"; shift; local y; for y in "$@"; do [[ "$x" == "$y" ]] && return 0; done; return 1; }

die_run() {
  local msg="$1"
  log "ABORT (run INCOMPLETE): $msg"
  local f="${DIE_FILE:-${RUN_DIR:-.}/ABORTED.json}"
  if [[ -n "$RUN_DIR" && -d "$RUN_DIR" ]]; then
    printf '{"verdict":"INCOMPLETE","aborted":true,"cause":%s,"session_dir":%s,"last_seq":%s,"time":%s}\n' \
      "$(json_str "$msg")" "$(json_str "${SDIR:-none}")" "$SEQ" "$(json_str "$(utc)")" > "$f"
    if [[ -z "$DIE_FILE" ]]; then cp -- "$f" "$RUN_DIR/run-verdict.json"; fi
  fi
  exit 20
}

usage() { sed -n '2,20p' "$HARNESS_DIR/q1-driver.sh" >&2; exit 2; }

# ------------------------------------------------------------------------------------------------ own-process discipline

own_pid_ok() {
  # True only for the process this script started: not protected, alive, and running our UI dll on our fixture copy.
  local pid="$1" cmdline
  [[ -n "$pid" && "$pid" =~ ^[0-9]+$ ]] || return 1
  in_list "$pid" "${PROTECTED_PIDS[@]}" && return 1
  [[ -r "/proc/$pid/cmdline" ]] || return 1
  cmdline="$(tr '\0' ' ' < "/proc/$pid/cmdline")"
  [[ "$cmdline" == *"$UI_DLL"* && "$cmdline" == *"$FIXTURE_DIR"* ]] || return 1
  return 0
}

cleanup() {
  local rc=$?
  if [[ -n "$SERVER_PID" ]] && kill -0 "$SERVER_PID" 2> /dev/null; then
    if own_pid_ok "$SERVER_PID"; then
      log "cleanup: SIGINT to own process $SERVER_PID ($SERVER_LABEL)"
      kill -INT "$SERVER_PID" 2> /dev/null || true
      local i
      for i in $(seq 1 150); do kill -0 "$SERVER_PID" 2> /dev/null || break; sleep 0.1; done
      if kill -0 "$SERVER_PID" 2> /dev/null && own_pid_ok "$SERVER_PID"; then
        log "cleanup: own process $SERVER_PID still alive 15 s after SIGINT; SIGKILL to that PID only"
        kill -KILL "$SERVER_PID" 2> /dev/null || true
      fi
    else
      log "cleanup: PID $SERVER_PID no longer matches our own process; NOT signalled"
    fi
  fi
  if [[ "$LOCK_HELD" == 1 ]]; then rm -f -- "$RUN_DIR/.lane-lock/owner"; rmdir -- "$RUN_DIR/.lane-lock" 2> /dev/null || true; fi
  exit "$rc"
}

acquire_lock() {
  # A second writer in the lane is an abort condition: the lock is an atomic mkdir.
  if ! mkdir -- "$RUN_DIR/.lane-lock" 2> /dev/null; then
    echo "STOP: $RUN_DIR/.lane-lock exists: another writer holds this lane (second writer -> run INCOMPLETE)" >&2
    exit 20
  fi
  LOCK_HELD=1
  printf '%s %s\n' "$$" "$(utc)" > "$RUN_DIR/.lane-lock/owner"
}

start_server() {
  local label="$1" dir="$2" out err i line re
  out="$RUN_DIR/logs/$label.stdout"; err="$RUN_DIR/logs/$label.stderr"
  [[ ! -e "$out" && ! -e "$err" ]] || die_run "server log for $label already exists (re-run into a new run-id)"
  # Job control gives the process its own process group with the default SIGINT action (as in the smoke test).
  set -m
  ( cd -- "$dir" && exec "$DOTNET_BIN" "$UI_DLL" "$FIXTURE_DIR" ) > "$out" 2> "$err" &
  SERVER_PID=$!
  set +m
  SERVER_LABEL="$label"
  in_list "$SERVER_PID" "${PROTECTED_PIDS[@]}" && die_run "the OS handed us a protected PID number ($SERVER_PID); refusing to continue"
  re='^Two-area MA-1 simulator: http://127\.0\.0\.1:([0-9]+)/s/([0-9a-f]{64})/multi$'
  PORT=""; CAP=""
  for i in $(seq 1 600); do
    while IFS= read -r line; do
      if [[ "$line" =~ $re ]]; then PORT="${BASH_REMATCH[1]}"; CAP="${BASH_REMATCH[2]}"; break; fi
    done < "$out"
    [[ -n "$PORT" ]] && break
    kill -0 "$SERVER_PID" 2> /dev/null || break
    sleep 0.1
  done
  [[ -n "$PORT" ]] || die_run "$label: the process printed no 'Two-area MA-1 simulator' line (see logs/$label.stderr)"
  in_list "$PORT" "${PROTECTED_PORTS[@]}" && die_run "$label bound a protected port $PORT"
  (( PORT >= 1024 )) || die_run "$label bound a privileged port $PORT"
  own_pid_ok "$SERVER_PID" || die_run "$label: PID $SERVER_PID does not look like our own UI process"
  printf '%s\t%s\t%s\t%s\n' "$label" "$SERVER_PID" "$PORT" "$(utc)" >> "$RUN_DIR/logs/ports.tsv"
  log "started $label: own PID $SERVER_PID, 127.0.0.1:$PORT"
}

stop_server() {
  local out err rc=0 i last
  STOP_CLEAN="false"
  [[ -n "$SERVER_PID" ]] || return 0
  out="$RUN_DIR/logs/$SERVER_LABEL.stdout"; err="$RUN_DIR/logs/$SERVER_LABEL.stderr"
  own_pid_ok "$SERVER_PID" || die_run "refusing to signal PID $SERVER_PID: not our own UI process"
  kill -INT "$SERVER_PID"
  for i in $(seq 1 300); do kill -0 "$SERVER_PID" 2> /dev/null || break; sleep 0.1; done
  if kill -0 "$SERVER_PID" 2> /dev/null; then
    log "$SERVER_LABEL: still running 30 s after SIGINT; SIGKILL to own PID $SERVER_PID"
    own_pid_ok "$SERVER_PID" && kill -KILL "$SERVER_PID" 2> /dev/null || true
  fi
  wait "$SERVER_PID" 2> /dev/null || rc=$?
  last="$(tail -n 1 -- "$out" 2> /dev/null || true)"
  if [[ "$rc" == 0 && "$last" == "Session ended; nothing was saved." && ! -s "$err" ]]; then STOP_CLEAN="true"; fi
  log "stopped $SERVER_LABEL: exit $rc, last line '$last', stderr bytes $(wc -c < "$err"), clean=$STOP_CLEAN"
  printf '%s\tstopped\texit=%s\tclean=%s\t%s\n' "$SERVER_LABEL" "$rc" "$STOP_CLEAN" "$(utc)" >> "$RUN_DIR/logs/ports.tsv"
  SERVER_PID=""
}

# ------------------------------------------------------------------------------------------------ HTTP + transcript

# http KIND METHOD ROUTE [BODYFILE]: one exchange, recorded and checked. Sets LAST_RESP and LAST_STATUS.
http() {
  local kind="$1" method="$2" route="$3" body="${4:-}" n slug resp req="" code crc=0 xrc=0 t0 t1 maxt=60
  SEQ=$((SEQ + 1)); printf -v n '%05d' "$SEQ"
  slug="${route//\//_}"
  resp="$SDIR/raw/$n-$method-$slug.json"
  # A Year+1 may take long; a timeout is an UNCONFIRMED outcome (abort), so its ceiling is above the 60 s year budget.
  if [[ "$route" == "api/multi/year/advance" ]]; then maxt=120; fi
  local -a args=(--silent --show-error --noproxy '*' --max-time "$maxt" -o "$resp" -w '%{http_code}' -X "$method"
                 -H "Host: 127.0.0.1:$PORT")
  if [[ "$method" == "POST" ]]; then
    req="$SDIR/raw/$n-request.json"
    cp -- "$body" "$req"
    args+=(-H "Origin: http://127.0.0.1:$PORT" -H "Content-Type: application/json" --data-binary "@$req")
  fi
  t0="$(now_us)"
  code="$(curl "${args[@]}" "http://127.0.0.1:$PORT/s/$CAP/$route" 2>> "$SDIR/curl.stderr")" || crc=$?
  t1="$(now_us)"
  [[ -f "$resp" ]] || : > "$resp"
  [[ "$code" =~ ^[0-9]{3}$ ]] || code=0
  rec exchange --session-dir "$SDIR" --seq "$SEQ" --kind "$kind" --method "$method" --route "$route" --request "$req" \
      --response "$resp" --status "$code" --t0 "$t0" --t1 "$t1" --curl-rc "$crc" > /dev/null || xrc=$?
  if [[ "$xrc" != 0 ]]; then die_run "exchange $SEQ ($method $route) failed a live check (exit $xrc); see $SDIR/transcript.jsonl"; fi
  LAST_RESP="$resp"; LAST_STATUS="$code"
}

read_view() { http read-view GET "api/multi/view"; LAST_VIEW="$LAST_RESP"; }

# post_cmd KIND ROUTE [body args]: read the view, echo its fence, act. (The next read happens before the next act.)
post_cmd() {
  local kind="$1" route="$2" body
  shift 2
  read_view
  body="$SDIR/raw/next-body.json"
  rec body --view "$LAST_VIEW" --route "$route" "$@" > "$body" || die_run "could not build the $route body from the view"
  http "$kind" POST "$route" "$body"
}

refusal_code() { rec get "$LAST_RESP" refusal.code 2> /dev/null || printf 'MISSING'; }

expect_200() {
  if [[ "$LAST_STATUS" != 200 ]]; then
    SESSION_OUTCOME="INCOMPLETE"
    SESSION_CAUSE="unexpected refusal $(refusal_code) (HTTP $LAST_STATUS) at: $1"
    log "$SESSION_CAUSE"
    return 1
  fi
}

write_session_json() {
  local scenario="$1" repeat="$2"
  printf '{"scenario":%s,"repeat":%s,"outcome":%s,"cause":%s,"last_committed_year":%s,"pid":%s,"port":%s,"exchanges":%s,"process_stop_clean":%s,"pinned":%s}\n' \
    "$(json_str "$scenario")" "$(json_str "$repeat")" "$(json_str "$SESSION_OUTCOME")" "$(json_str "$SESSION_CAUSE")" \
    "$LAST_YEAR" "$(json_str "$SERVER_PID_RECORDED")" "$(json_str "$PORT")" "$SEQ" "$(json_str "$STOP_CLEAN")" \
    "$(json_str "$PINNED_SEEDS; manifest $PINNED_MANIFEST_HASH")" > "$SDIR/session.json"
}

# ------------------------------------------------------------------------------------------------ scenario wiring

S1_TAKE_A=""
S1_TAKE_B=""
SERVER_PID_RECORDED=""

# schedule SCENARIO YEAR -> the intervention token for that year, or nothing.
schedule() {
  case "$1:$2" in
    "L:0") echo "L" ;;
    "M:0") echo "M" ;;
    "D:5") echo "L" ;;
    "S:0") echo "L" ;;
    "S:10") echo "L-SURVIVING-CROPS" ;;
    "S1:0") echo "S1" ;;
    "I:0") echo "L-MA-A-ONLY" ;;
    *) echo "" ;;
  esac
}

intervene() {
  local scenario="$1" y="$2" token="$3" tag sel src area tree reason
  printf -v tag 'y%02d' "$y"
  local -a sargs
  case "$token" in
    "L") sargs=(--mode L) ;;
    "M") sargs=(--mode M) ;;
    "L-SURVIVING-CROPS") sargs=(--mode L --crops-from-view) ;;
    "L-MA-A-ONLY") sargs=(--mode L --areas MA-A) ;;
    "S1") sargs=(--mode ALL --take "MA-A=$S1_TAKE_A" --take "MA-B=$S1_TAKE_B") ;;
    *) die_run "unknown intervention token $token" ;;
  esac
  read_view
  cp -- "$LAST_VIEW" "$SDIR/intervention-$tag-view.json"
  sel="$SDIR/selection-$tag.json"
  src=0
  "${PY[@]}" "$H/q1-select.py" "${sargs[@]}" --scenario "$scenario" --year "$y" < "$LAST_VIEW" > "$sel" || src=$?
  case "$src" in
    0) ;;
    3) die_run "selection DEFECT at $scenario $tag (a Crop on a competitor list or an inconsistent view); see $sel" ;;
    4) SESSION_OUTCOME="NOT-CONSTRUCTIBLE"; SESSION_CAUSE="S1 not constructible: $(rec get "$sel" not_constructible)"; log "$SESSION_CAUSE"; return 1 ;;
    *) die_run "q1-select.py failed (exit $src) at $scenario $tag" ;;
  esac
  rec tsv "$sel" marks_to_issue areaId treeId > "$SDIR/raw/marks-$tag.tsv" || die_run "cannot list Crop marks"
  rec tsv "$sel" orders areaId treeId reason > "$SDIR/raw/orders-$tag.tsv" || die_run "cannot list orders"
  # 5(a) Crop marks first: areas ordinal, Crops ascending ordinal TreeId.
  while IFS=$'\t' read -r -u 3 area tree; do
    post_cmd crop-mark "api/multi/crop/mark" --area "$area" --tree "$tree"
    expect_200 "Crop mark $area $tree ($scenario $tag)" || return 1
  done 3< "$SDIR/raw/marks-$tag.tsv"
  # 5(b) orders in sequence order.
  while IFS=$'\t' read -r -u 3 area tree reason; do
    post_cmd pending-add "api/multi/pending/add" --area "$area" --tree "$tree" --reason "$reason"
    expect_200 "AddPending $area $tree ($scenario $tag)" || return 1
  done 3< "$SDIR/raw/orders-$tag.tsv"
  # 5(c) empty selected set: do NOT approve.
  if [[ ! -s "$SDIR/raw/orders-$tag.tsv" ]]; then
    printf '{"scenario":%s,"year":%s,"result":"no intervention","selection":%s}\n' "$(json_str "$scenario")" "$y" \
      "$(json_str "selection-$tag.json")" > "$SDIR/no-intervention-$tag.json"
    log "$scenario $tag: empty selected order set -> no intervention (Approve not called)"
    return 0
  fi
  post_cmd approve "api/multi/plan/approve"
  expect_200 "Approve ($scenario $tag)" || return 1
  # 6. Disclosure before Execute, from a fresh read.
  read_view
  rec disclosure --selection "$sel" --view "$LAST_VIEW" --scenario "$scenario" --year "$y" --out "$SDIR/disclosure-$tag.json" \
    || die_run "disclosure check failed at $scenario $tag (Approved.Orders != frozen selection)"
  post_cmd execute "api/multi/plan/execute"
  expect_200 "Execute ($scenario $tag)" || return 1
  [[ "$(rec get "$LAST_RESP" execution.replayed)" == "false" ]] || die_run "first Execute answered as a replay at $scenario $tag"
  return 0
}

advance_year() {
  local y="$1" nn code
  post_cmd advance "api/multi/year/advance" --consent false
  if [[ "$LAST_STATUS" == 200 ]]; then
    printf -v nn '%02d' "$((y + 1))"
    cp -- "$LAST_RESP" "$SDIR/reports/year-$nn.json"
    read_view
    cp -- "$LAST_VIEW" "$SDIR/view-year-$nn.json"
    LAST_YEAR=$((y + 1))
    return 0
  fi
  code="$(refusal_code)"
  if [[ "$code" == "ContinuationNoLivingStems" ]]; then
    SESSION_OUTCOME="LAWFUL-STOP"
    SESSION_CAUSE="both areas empty: Advance refused ContinuationNoLivingStems at year $y (last committed year $y)"
  else
    SESSION_OUTCOME="INCOMPLETE"
    SESSION_CAUSE="unexpected Advance refusal $code (HTTP $LAST_STATUS) at year $y"
  fi
  log "$SESSION_CAUSE"
  return 1
}

session_body() {
  local scenario="$1" y token year_t0 dt
  for (( y = 0; y < HORIZON; y++ )); do
    year_t0="$(now_us)"
    token="$(schedule "$scenario" "$y")"
    if [[ -n "$token" ]]; then intervene "$scenario" "$y" "$token" || return 1; fi
    advance_year "$y" || return 1
    dt=$(( $(now_us) - year_t0 ))
    if (( dt > YEAR_BUDGET_US )); then
      SESSION_OUTCOME="BUDGET-STOP"; SESSION_CAUSE="year $y -> $((y + 1)) took $((dt / 1000)) ms > 60 s (last committed year $LAST_YEAR)"
      log "$SESSION_CAUSE"; return 1
    fi
    if (( $(now_us) - SESSION_T0 > SCENARIO_BUDGET_US )); then
      SESSION_OUTCOME="BUDGET-STOP"; SESSION_CAUSE="scenario exceeded 30 min (last committed year $LAST_YEAR)"
      log "$SESSION_CAUSE"; return 1
    fi
  done
  return 0
}

run_session() {
  local scenario="$1" repeat="$2"
  SDIR="$RUN_DIR/sessions/$scenario/r$repeat"
  [[ ! -e "$SDIR" ]] || die_run "session directory $SDIR already exists (use a new run-id)"
  mkdir -p -- "$SDIR/raw" "$SDIR/reports"
  SEQ=0; LAST_YEAR=0; SESSION_OUTCOME="COMPLETE"; SESSION_CAUSE=""
  start_server "$scenario-r$repeat" "$SDIR"
  SERVER_PID_RECORDED="$SERVER_PID"
  rec header-session --file "$RUN_DIR/header.json" --scenario "$scenario" --repeat "$repeat" --pid "$SERVER_PID" \
    --port "$PORT" --started "$(utc)" || die_run "header update failed"
  SESSION_T0="$(now_us)"
  http manifest GET "api/multi/manifest"
  read_view
  cp -- "$LAST_VIEW" "$SDIR/view-year-00.json"
  session_body "$scenario" || true
  stop_server
  if [[ "$STOP_CLEAN" != "true" && "$SESSION_OUTCOME" == "COMPLETE" ]]; then
    SESSION_OUTCOME="INCOMPLETE"; SESSION_CAUSE="process did not end cleanly (exit 0 + 'Session ended; nothing was saved.' + empty stderr)"
  fi
  write_session_json "$scenario" "r$repeat"
  log "session $scenario r$repeat: $SESSION_OUTCOME ${SESSION_CAUSE:+($SESSION_CAUSE)} last committed year $LAST_YEAR"
}

compare_repeats() {
  local scenario="$1" rc=0
  rec compare "$RUN_DIR/sessions/$scenario/r1" "$RUN_DIR/sessions/$scenario/r2" \
    --out "$RUN_DIR/sessions/$scenario/projection-compare.json" > /dev/null || rc=$?
  [[ "$rc" == 0 ]] || die_run "PROJECTION MISMATCH between $scenario r1 and r2 (see sessions/$scenario/projection-compare.json)"
  log "projection equal: $scenario r1 == r2"
}

# ------------------------------------------------------------------------------------------------ optional probes

probe_record() {
  # probe_record NAME EXERCISED(true|false) EXPECTED OBSERVED PASS(true|false|n/a)
  printf '{"probe":%s,"attempted":true,"exercised":%s,"expected":%s,"observed":%s,"pass":%s,"last_seq":%s}\n' \
    "$(json_str "$1")" "$2" "$(json_str "$3")" "$(json_str "$4")" "$(json_str "$5")" "$SEQ" >> "$SDIR/probes.jsonl"
}

run_probes() {
  local a_tree tree code saved
  SDIR="$RUN_DIR/sessions/PROBES/r1"
  [[ ! -e "$SDIR" ]] || die_run "probe session directory exists"
  mkdir -p -- "$SDIR/raw" "$SDIR/reports"
  SEQ=0; LAST_YEAR=0; SESSION_OUTCOME="COMPLETE"; SESSION_CAUSE=""
  start_server "PROBES-r1" "$SDIR"
  SERVER_PID_RECORDED="$SERVER_PID"
  rec header-session --file "$RUN_DIR/header.json" --scenario PROBES --repeat 1 --pid "$SERVER_PID" --port "$PORT" --started "$(utc)"
  http manifest GET "api/multi/manifest"
  read_view
  # P2 Approve-then-Cancel: 0 Fell, plan-state +1, orders not returned (all asserted by the live checker).
  a_tree="$(rec living-ids "$LAST_VIEW" MA-A | head -n 1)"
  post_cmd probe "api/multi/pending/add" --area MA-A --tree "$a_tree" --reason "Q1 probe P2 approve-then-cancel"
  post_cmd probe "api/multi/plan/approve"
  post_cmd probe "api/multi/plan/cancel"
  if [[ "$LAST_STATUS" == 200 && "$(rec get "$LAST_RESP" view.pending)" == "[]" ]]; then
    probe_record "P2 approve-then-cancel" true "200; approved null; pending []; 0 Fell; plan-state +1" "HTTP $LAST_STATUS" true
  else probe_record "P2 approve-then-cancel" true "200" "HTTP $LAST_STATUS $(refusal_code)" false; fi
  # P1 Fell-to-empty: every living MA-B tree ordered -> Execute refused WorkWouldLeaveNoLivingStems; approval stays.
  read_view
  rec living-ids "$LAST_VIEW" MA-B > "$SDIR/raw/p1-ids.txt"
  while IFS= read -r -u 3 tree; do
    post_cmd probe "api/multi/pending/add" --area MA-B --tree "$tree" --reason "Q1 probe P1 fell-to-empty"
  done 3< "$SDIR/raw/p1-ids.txt"
  post_cmd probe "api/multi/plan/approve"
  post_cmd probe "api/multi/plan/execute"
  code="$(refusal_code)"
  read_view
  if [[ "$code" == "WorkWouldLeaveNoLivingStems" && "$(rec get "$LAST_VIEW" view.approved)" != "null" ]]; then
    probe_record "P1 fell-to-empty refusal" true "409 WorkWouldLeaveNoLivingStems; approval stays; nothing felled" "$code; approval present" true
  else probe_record "P1 fell-to-empty refusal" true "409 WorkWouldLeaveNoLivingStems" "$code" false; fi
  post_cmd probe "api/multi/plan/cancel"
  # P4 replay of an executed key: the identical recorded request again -> 200 Replayed=true, no change.
  read_view
  a_tree="$(rec living-ids "$LAST_VIEW" MA-A | head -n 1)"
  post_cmd probe "api/multi/pending/add" --area MA-A --tree "$a_tree" --reason "Q1 probe P4 replay"
  post_cmd probe "api/multi/plan/approve"
  post_cmd probe "api/multi/plan/execute"
  saved="$SDIR/raw/p4-execute-request.json"
  cp -- "$SDIR/raw/$(printf '%05d' "$SEQ")-request.json" "$saved"
  read_view
  # The engine answers a replay only for the original source (MultiAreaContinuation.cs:229-233), so the replay
  # deliberately re-sends the recorded request; kind replay-probe exempts it from the fence-echo check only.
  http replay-probe POST "api/multi/plan/execute" "$saved"
  if [[ "$LAST_STATUS" == 200 && "$(rec get "$LAST_RESP" execution.replayed)" == "true" ]]; then
    probe_record "P4 replay of an executed key" true "200 Replayed=true; no second Fell" "Replayed=true" true
  else probe_record "P4 replay of an executed key" true "200 Replayed=true" "HTTP $LAST_STATUS $(refusal_code)" false; fi
  # P3 consented advance with pending work: no consent refused; consent cancels all work at 0 Fell.
  read_view
  a_tree="$(rec living-ids "$LAST_VIEW" MA-A | head -n 1)"
  post_cmd probe "api/multi/pending/add" --area MA-A --tree "$a_tree" --reason "Q1 probe P3 consent"
  post_cmd probe "api/multi/year/advance" --consent false
  code="$(refusal_code)"
  post_cmd probe "api/multi/year/advance" --consent true
  if [[ "$code" == "WorkPendingConsentRequired" && "$LAST_STATUS" == 200 \
        && "$(rec get "$LAST_RESP" 'view.annualHistory[-1].cancelledUnexecutedWork')" == "true" ]]; then
    probe_record "P3 consented advance with pending work" true "409 WorkPendingConsentRequired then 200 CancelledUnexecutedWork=true, 0 Fell" "$code then 200" true
  else probe_record "P3 consented advance with pending work" true "409 then 200" "$code then HTTP $LAST_STATUS" false; fi
  [[ "$LAST_STATUS" == 200 ]] && LAST_YEAR=1
  # P5 all-empty Year+1 refusal: not reachable by player action on canonical data (P1 shows Fell cannot empty an area).
  post_cmd probe "api/multi/year/advance" --consent false
  if [[ "$LAST_STATUS" == 200 ]]; then
    LAST_YEAR=$((LAST_YEAR + 1))
    probe_record "P5 all-empty advance refusal" false "ContinuationNoLivingStems" \
      "not exercised on canonical data: areas populated, Advance committed; witness only if natural mortality empties both areas in a scenario" "n/a"
  else
    probe_record "P5 all-empty advance refusal" true "ContinuationNoLivingStems" "$(refusal_code)" "$([[ "$(refusal_code)" == ContinuationNoLivingStems ]] && echo true || echo false)"
  fi
  stop_server
  write_session_json PROBES r1
  log "probes done: $SDIR/probes.jsonl"
}

run_w12() {
  local w="$RUN_DIR/w12" rc
  mkdir -p -- "$w/cwd"
  [[ -f "$CLI_DLL" ]] || { printf '{"w12":"not run: CLI dll missing"}\n' > "$w/w12.json"; return 0; }
  rc=0; ( cd -- "$w/cwd" && exec timeout 120 "$DOTNET_BIN" "$CLI_DLL" annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 ) \
    > "$w/form-readme.stdout" 2> "$w/form-readme.stderr" || rc=$?
  local rc2=0; ( cd -- "$w/cwd" && exec timeout 120 "$DOTNET_BIN" "$CLI_DLL" annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 "$FIXTURE_DIR" ) \
    > "$w/form-with-dir.stdout" 2> "$w/form-with-dir.stderr" || rc2=$?
  printf '{"w12":"both README CLI annual forms run once","form_readme":{"args":"annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1","exit":%s},"form_with_dir":{"args":"annual DerivedP1ThirtyMetreExperimentV1 p1d-30m-sitka-adult-v1 <fixtureDir>","exit":%s}}\n' \
    "$rc" "$rc2" > "$w/w12.json"
  log "W12: README form exit $rc, with-directory form exit $rc2"
}

# ------------------------------------------------------------------------------------------------ preflight

check_run_dir() {
  local real root
  mkdir -p -- "$RUN_DIR"
  real="$(cd -- "$RUN_DIR" && pwd -P)"
  for root in "${FORBIDDEN_ROOTS[@]}"; do
    case "$real/" in "$root"/*) echo "STOP: run directory $real is inside $root" >&2; exit 2 ;; esac
  done
  RUN_DIR="$real"
}

preflight_runtime() {
  local v
  (( BASH_VERSINFO[0] >= 5 )) || { echo "STOP: bash >= 5 required (EPOCHREALTIME)" >&2; exit 2; }
  command -v curl > /dev/null && command -v python3 > /dev/null && command -v sha256sum > /dev/null \
    || { echo "STOP: curl, python3 and sha256sum are required" >&2; exit 2; }
  [[ -x "$DOTNET_BIN" ]] || { echo "STOP: $DOTNET_BIN is not executable" >&2; exit 2; }
  v="$("$DOTNET_BIN" --version)"
  [[ "$v" == "$EXPECTED_DOTNET_VERSION" ]] || { echo "STOP: dotnet $v != $EXPECTED_DOTNET_VERSION (the system dotnet must never be used)" >&2; exit 2; }
  UI_DLL="$LAB/src/Ui/bin/Debug/net8.0/sitkap1-ui.dll"
  CLI_DLL="$LAB/src/Cli/bin/Debug/net8.0/sitkap1.dll"
  [[ -f "$UI_DLL" ]] || { echo "STOP: $UI_DLL missing (this harness never builds)" >&2; exit 2; }
}

preflight_bundle() {
  local f sha lab_head lab_dirty
  [[ -z "$(ls -A -- "$RUN_DIR")" ]] || { echo "STOP: $RUN_DIR is not empty (use a new run-id; nothing is overwritten)" >&2; exit 2; }
  acquire_lock
  mkdir -p -- "$RUN_DIR/harness" "$RUN_DIR/logs" "$RUN_DIR/sessions" "$RUN_DIR/analysis" "$RUN_DIR/fixture"
  for f in "${HARNESS_FILES[@]}" SHA256SUMS; do cp -- "$HARNESS_DIR/$f" "$RUN_DIR/harness/$f"; done
  ( cd -- "$RUN_DIR/harness" && sha256sum -c --quiet SHA256SUMS ) || die_run "harness bytes do not match SHA256SUMS"
  H="$RUN_DIR/harness"
  lab_head="$(git -C "$LAB" rev-parse HEAD)"          # read-only identity check of the integrated source
  [[ "$lab_head" == "$EXPECTED_SOURCE_SHA" ]] || die_run "source HEAD $lab_head != pinned $EXPECTED_SOURCE_SHA"
  lab_dirty="$(git -C "$LAB" status --porcelain --untracked-files=no -- . | wc -l)"
  [[ "$lab_dirty" == 0 ]] || die_run "the integrated lab has $lab_dirty modified tracked files"
  cp -- "$LAB/fixtures/derived/$FIXTURE_NAME" "$RUN_DIR/fixture/$FIXTURE_NAME"
  FIXTURE_DIR="$RUN_DIR/fixture"
  rec header-init --file "$RUN_DIR/header.json" \
    --set "source_sha=$lab_head" --set "manifest_content_hash_pinned=$PINNED_MANIFEST_HASH" \
    --set "pinned_seeds=$PINNED_SEEDS" \
    --set "harness_sha256sums=$(tr '\n' ';' < "$RUN_DIR/harness/SHA256SUMS")" \
    --set "dotnet=$DOTNET_BIN $("$DOTNET_BIN" --version)" --set "node=not used" --set "kernel=$(uname -srm)" \
    --set "host=$(uname -n)" --set "worktree_lab=$LAB" \
    --set "ui_dll_sha256=$(sha256sum < "$UI_DLL" | cut -d' ' -f1)" \
    --set "core_dll_sha256=$(sha256sum < "$LAB/src/Ui/bin/Debug/net8.0/SitkaP1.Core.dll" | cut -d' ' -f1)" \
    --set "fixture_sha256=$(sha256sum < "$FIXTURE_DIR/$FIXTURE_NAME" | cut -d' ' -f1)" \
    --set "run_dir=$RUN_DIR" --set "started=$(utc)" --set "horizon=$HORIZON" \
    --set "budget=60 s per year, 30 min per scenario" || die_run "header-init failed"
}

# ------------------------------------------------------------------------------------------------ main

main() {
  local mode="${1:-}" scenarios="C L M D S S1 I" probes=0 w12=0 s ok rc
  [[ -n "$mode" ]] || usage
  shift
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --run-dir) RUN_DIR="$2"; shift 2 ;;
      --scenarios) scenarios="$2"; shift 2 ;;
      --probes) probes=1; shift ;;
      --w12) w12=1; shift ;;
      --lab) LAB="$2"; shift 2 ;;
      --dotnet) DOTNET_BIN="$2"; shift 2 ;;
      *) usage ;;
    esac
  done
  [[ -n "$RUN_DIR" ]] || usage
  trap cleanup EXIT
  trap 'exit 130' INT TERM
  check_run_dir
  preflight_runtime
  preflight_bundle
  log "run $RUN_DIR: harness AUTHORED-NOT-EXECUTED bytes, verified against SHA256SUMS; mode $mode"
  case "$mode" in
    probes) run_probes; exit 0 ;;
    w12) run_w12; exit 0 ;;
    run) ;;
    *) usage ;;
  esac
  local -a wanted=() order=()
  read -r -a wanted <<< "$scenarios"
  for s in "${wanted[@]}"; do in_list "$s" "${CANONICAL_ORDER[@]}" || die_run "unknown scenario $s"; done
  for s in "${CANONICAL_ORDER[@]}"; do in_list "$s" "${wanted[@]}" && order+=("$s"); done
  if in_list "S1" "${order[@]}" && ! in_list "S" "${order[@]}"; then die_run "S1 is sized only from S's realised counts in the same run; include S"; fi
  if in_list "I" "${order[@]}" && ! in_list "C" "${order[@]}"; then log "note: I without C in this run -> isolation check not possible"; fi
  local isolation="not-run"
  for s in "${order[@]}"; do
    if [[ "$s" == "S1" ]]; then
      rc=0; ok="$("${PY[@]}" "$H/q1-analyse.py" s1-size --session "$RUN_DIR/sessions/S/r1")" || rc=$?
      if [[ "$rc" != 0 ]]; then
        mkdir -p -- "$RUN_DIR/sessions/S1"
        printf '{"scenario":"S1","outcome":"NOT-RUN","cause":%s}\n' "$(json_str "S did not realise both interventions: $ok")" > "$RUN_DIR/sessions/S1/not-run.json"
        log "S1 not run: $ok"; continue
      fi
      S1_TAKE_A="$(printf '%s\n' "$ok" | sed -n 's/^MA-A=\([0-9][0-9]*\)$/\1/p')"
      S1_TAKE_B="$(printf '%s\n' "$ok" | sed -n 's/^MA-B=\([0-9][0-9]*\)$/\1/p')"
      [[ -n "$S1_TAKE_A" && -n "$S1_TAKE_B" ]] || die_run "S1 sizing output not understood: $ok"
      log "S1 sized from S r1 realised stems: MA-A=$S1_TAKE_A MA-B=$S1_TAKE_B"
    fi
    run_session "$s" 1
    run_session "$s" 2
    compare_repeats "$s"
    if [[ "$s" == "S1" && -f "$RUN_DIR/sessions/S1/r1/selection-y00.json" ]]; then
      "${PY[@]}" "$H/q1-analyse.py" s1-check --s "$RUN_DIR/sessions/S/r1/selection-y00.json" \
        --s1 "$RUN_DIR/sessions/S1/r1/selection-y00.json" > "$RUN_DIR/sessions/S1/s1-vs-s-selection.json" \
        || die_run "S1 year-0 Crop set / sequence differ from S year 0 (nondeterministic selection input)"
    fi
    if [[ "$s" == "I" && -d "$RUN_DIR/sessions/C/r1" ]]; then
      rc=0; "${PY[@]}" "$H/q1-analyse.py" isolation --i "$RUN_DIR/sessions/I/r1" --c "$RUN_DIR/sessions/C/r1" \
        > "$RUN_DIR/analysis/isolation.json" || rc=$?
      if [[ "$rc" == 0 ]]; then isolation="equal"; else isolation="DEFECT"; log "ISOLATION DEFECT: see analysis/isolation.json"; fi
    fi
  done
  if [[ "$probes" == 1 ]]; then run_probes; fi
  if [[ "$w12" == 1 ]]; then run_w12; fi
  rc=0; "${PY[@]}" "$H/q1-analyse.py" all --run-dir "$RUN_DIR" --isolation "$isolation" || rc=$?
  log "analysis exit $rc; verdict in $RUN_DIR/run-verdict.json (PRODUCED at most; never VERIFIED by this harness)"
  exit 0
}

if [[ "${BASH_SOURCE[0]}" == "$0" ]]; then
  main "$@"
fi
````

### `q1-select.py` — 363 lines, 16232 bytes

SHA-256 `b54e30ea735730a90e31b102c821c6f69deda553e2c0d412592974bbd5944e70`

````python
#!/usr/bin/env python3
"""q1-select.py -- Multi #885 Q1 frozen selection rule (a), Crop-tree release.

AUTHORED, NOT EXECUTED. Scratch harness component; not a reviewed repository feature.

A pure function of ONE intervention-time view read on stdin (either the GET api/multi/view payload
{"view": ...} or the bare view object). It issues no request, reads no other file and keeps no state.

Specification: Q1 packet section 5 = Q0 revision 3 section 6 "Selection rule" steps 1-4 (computation),
5(a)/5(b) (mark and order issue order), step 10 (S year-10 keeps the surviving year-0 Crops) and the S1
definition (first n_A / n_B entries of the same year-0 deduplicated all-layer sequence).

  1. Crop set: living trees by DBH descending, ties by ordinal TreeId; walk once; accept when the float32
     E3 distance to every accepted Crop is > 8 m.
  2. Frozen competitor lists: per Crop, living neighbours with E3 distance <= 8 m (AnnualRules.cs skips
     distance > 8), ranked by the float32 Hegyi term D_j / max(1, D_crop) / max(0.5, d) descending, ties by
     ordinal TreeId. A Crop on any list is a reported DEFECT (exit 3).
  3. Removal sequence per area (areas in ordinal AreaId order): rank layer k = for each Crop in ascending
     ordinal Crop TreeId, its rank-k competitor if any; layers concatenated; deduplicated at first occurrence;
     a dropped duplicate is NOT replaced and the Crop does NOT advance.
  4. L = deduplicated layer-1 entries; M = deduplicated entries of layers 1-3; ALL + --take AREA=n = first n
     deduplicated entries over all layers (S1). Fewer than n entries => NOT CONSTRUCTIBLE (exit 4), never
     substituted.

float32 arithmetic is emulated by rounding every single IEEE operation (computed exactly enough in binary64)
to binary32 with struct.pack('<f'). For +, -, *, / and sqrt on binary32 operands, binary64 evaluation
followed by one rounding to binary32 equals the correctly rounded binary32 result (53 >= 2*24 + 2).
Input float32 values are recovered from their JSON text with exact rational rounding (fractions.Fraction),
cross-checked against struct; a disagreement is a DEFECT.

Exit codes: 0 selection written; 2 usage/input error; 3 DEFECT (abort the run); 4 S1 not constructible.
"""
import argparse
import hashlib
import json
import math
import struct
import sys
from fractions import Fraction

HEGYI_CUTOFF_M = 8.0      # AnnualRules.cs:6  HegyiCutoffM = 8f
HEGYI_DISTANCE_FLOOR_M = 0.5  # AnnualRules.cs:7  HegyiDistanceFloorM = 0.5f
HEGYI_DBH_FLOOR_CM = 1.0  # AnnualRules.cs:79 MathF.Max(1f, target.DbhCm)
MAX_LAYER_M = 3           # Q0 rev3 section 6 step 4: M = layers 1-3

EXIT_OK, EXIT_USAGE, EXIT_DEFECT, EXIT_NOT_CONSTRUCTIBLE = 0, 2, 3, 4
SCENARIOS = ("C", "L", "M", "D", "S", "S1", "I", "PROBES")


class Num:
    """A JSON number kept as its exact source text (no binary rounding, sign of zero kept)."""
    __slots__ = ("text",)

    def __init__(self, text):
        self.text = text


def _pairs(pairs):
    out = {}
    for key, value in pairs:
        if key in out:
            raise ValueError("duplicate JSON member: " + key)
        out[key] = value
    return out


def _constant(name):
    raise ValueError("bare JSON constant not allowed: " + name)


def loads(raw):
    return json.loads(raw.decode("utf-8"), object_pairs_hook=_pairs, parse_float=Num, parse_int=Num,
                      parse_constant=_constant)


def f32_bits_exact(text):
    """Correctly rounded binary32 bit pattern of a decimal JSON number, by exact rational arithmetic."""
    neg = text.lstrip().startswith("-")
    q = Fraction(text)
    if q == 0:
        return 0x80000000 if neg else 0
    a = abs(q)
    e = a.numerator.bit_length() - a.denominator.bit_length()
    if _pow2(e) > a:
        e -= 1
    while _pow2(e + 1) <= a:
        e += 1
    if e < -126:
        e = -126                      # subnormal range: fixed scale 2^-149
    scaled = a / _pow2(e - 23)
    m = scaled.numerator // scaled.denominator
    rem = scaled - m
    if rem > Fraction(1, 2) or (rem == Fraction(1, 2) and m % 2 == 1):
        m += 1
    if m >= (1 << 24):
        m >>= 1
        e += 1
    if e > 127:
        raise OverflowError("binary32 overflow: " + text)
    if m < (1 << 23):                 # subnormal (or rounded up exactly to the smallest normal below)
        bits = m
    else:
        bits = ((e + 127) << 23) | (m - (1 << 23))
    return bits | (0x80000000 if neg else 0)


def _pow2(k):
    return Fraction(1 << k) if k >= 0 else Fraction(1, 1 << -k)


def f32(x):
    """Round one binary64 value to binary32 (round to nearest even), returned as a Python float."""
    return struct.unpack("<f", struct.pack("<f", x))[0]


def f32_from_bits(bits):
    return struct.unpack(">f", bits.to_bytes(4, "big"))[0]


def hex32(x):
    return struct.pack(">f", x).hex()


def ordkey(text):
    """.NET StringComparer.Ordinal order: UTF-16 code units."""
    return text.encode("utf-16-be")


class Defect(Exception):
    pass


def num_f32(value, where):
    if isinstance(value, str):
        raise Defect("named float literal %r at %s; selection cannot use it" % (value, where))
    if not isinstance(value, Num):
        raise Defect("expected a JSON number at %s" % where)
    bits = f32_bits_exact(value.text)
    via_struct = struct.unpack(">I", struct.pack(">f", float(value.text)))[0]
    if bits != via_struct:
        raise Defect("float32 parse disagreement at %s: exact %08x, struct %08x" % (where, bits, via_struct))
    return f32_from_bits(bits), "%08x" % bits


class Tree:
    __slots__ = ("tree_id", "x", "z", "dbh", "dbh_bits", "x_bits", "z_bits")


def e3_distance(target, other):
    """AnnualRules.cs:73-75: float dx = target.XM - tree.XM; float dz = ...; (float)Math.Sqrt(dx*dx + dz*dz)."""
    dx = f32(target.x - other.x)
    dz = f32(target.z - other.z)
    s = f32(f32(dx * dx) + f32(dz * dz))
    return f32(math.sqrt(s))


def hegyi_term(crop, other, distance):
    """AnnualRules.cs:77 (tree.DbhCm / MathF.Max(1f, target.DbhCm)) / MathF.Max(0.5f, distance), float32."""
    ratio = f32(other.dbh / max(HEGYI_DBH_FLOOR_CM, crop.dbh))
    return f32(ratio / max(HEGYI_DISTANCE_FLOOR_M, distance))


def living_trees(area):
    trees = []
    seen = set()
    for i, t in enumerate(area["trees"]):
        where = "areas[%s].trees[%d]" % (area["areaId"], i)
        tid = t["treeId"]
        if tid in seen:
            raise Defect("duplicate tree ID " + tid)
        seen.add(tid)
        if not tid.startswith(area["areaId"] + "-"):
            raise Defect("tree %s is not prefixed by its area %s" % (tid, area["areaId"]))
        living = t["isLiving"]
        if living is not (t["death"] is None):
            raise Defect("isLiving disagrees with death for " + tid)
        if not living:
            continue
        tree = Tree()
        tree.tree_id = tid
        tree.dbh, tree.dbh_bits = num_f32(t["dbhCm"], where + ".dbhCm")
        tree.x, tree.x_bits = num_f32(t["xm"], where + ".xm")
        tree.z, tree.z_bits = num_f32(t["zm"], where + ".zm")
        trees.append(tree)
    return trees


def select_area(area, mode, take, crops_from_view, scenario, year):
    area_id = area["areaId"]
    living = living_trees(area)
    by_id = {t.tree_id: t for t in living}
    existing_marks = list(area["cropTreeIds"])
    out = {"areaId": area_id, "treated": True, "living_count": len(living), "existing_crop_marks": existing_marks}

    # Step 1 (or step 10 for S at year 10).
    acceptance = []
    if crops_from_view:
        for cid in existing_marks:
            if cid not in by_id:
                raise Defect("view Crop mark %s in %s is not a living tree" % (cid, area_id))
            acceptance.append(by_id[cid])
        crop_source = "view-marks (Q0 rev3 section 6 step 10: surviving year-0 Crops, no new Crop)"
    else:
        if existing_marks:
            raise Defect("area %s already carries Crop marks %s at a fresh-Crop intervention" % (area_id, existing_marks))
        ranked = sorted(living, key=lambda t: (-t.dbh, ordkey(t.tree_id)))
        for cand in ranked:
            if all(e3_distance(cand, c) > HEGYI_CUTOFF_M for c in acceptance):
                acceptance.append(cand)
        crop_source = "computed (Q0 rev3 section 6 step 1)"
    crops = sorted(acceptance, key=lambda t: ordkey(t.tree_id))
    crop_ids = {c.tree_id for c in crops}
    out["crop_source"] = crop_source
    out["crop_acceptance_order"] = [{"treeId": c.tree_id, "dbhCm_f32": c.dbh_bits} for c in acceptance]
    out["crops"] = [c.tree_id for c in crops]

    # Step 2: frozen competitor lists from the same state.
    lists = []
    for crop in crops:
        entries = []
        for other in living:
            if other.tree_id == crop.tree_id:
                continue
            d = e3_distance(crop, other)
            if d > HEGYI_CUTOFF_M:
                continue
            if other.tree_id in crop_ids:
                raise Defect("Crop %s appears on the competitor list of Crop %s (d=%s)" % (other.tree_id, crop.tree_id, hex32(d)))
            entries.append((hegyi_term(crop, other, d), other, d))
        entries.sort(key=lambda e: (-e[0], ordkey(e[1].tree_id)))
        lists.append({"cropTreeId": crop.tree_id, "list": [
            {"rank": r + 1, "treeId": o.tree_id, "dbhCm_f32": o.dbh_bits, "distanceM_f32": hex32(d), "term_f32": hex32(term)}
            for r, (term, o, d) in enumerate(entries)]})
    out["competitor_lists"] = lists

    # Step 3: rank layers, concatenation, first-occurrence deduplication without replacement.
    depth = max((len(l["list"]) for l in lists), default=0)
    sequence = []
    for k in range(1, depth + 1):
        for l in lists:                       # lists are in ascending ordinal Crop TreeId
            if len(l["list"]) >= k:
                sequence.append({"layer": k, "cropTreeId": l["cropTreeId"], "rank": k, "treeId": l["list"][k - 1]["treeId"]})
    seen = {}
    dedup, dropped = [], []
    for pos, entry in enumerate(sequence):
        entry = dict(entry, pos=pos)
        if entry["treeId"] in seen:
            dropped.append(dict(entry, first_pos=seen[entry["treeId"]]))
            continue
        seen[entry["treeId"]] = pos
        dedup.append(entry)
    out["sequence"] = sequence
    out["deduplicated"] = dedup
    out["dropped_duplicates"] = dropped

    # Step 4: intensity.
    not_constructible = None
    # Copies, so the reason text never leaks into the frozen deduplicated sequence (S vs S1 compare it).
    if mode == "L":
        selected = [dict(e) for e in dedup if e["layer"] == 1]
    elif mode == "M":
        selected = [dict(e) for e in dedup if e["layer"] <= MAX_LAYER_M]
    else:
        need = take.get(area_id)
        if need is None:
            raise ValueError("--take is required for every treated area in mode ALL (missing %s)" % area_id)
        if len(dedup) < need:
            not_constructible = {"need": need, "have": len(dedup), "shortfall": need - len(dedup)}
            selected = []
        else:
            selected = [dict(e) for e in dedup[:need]]
    for e in selected:
        e["reason"] = ("Q1 %s y%d rule (a) Crop release: layer %d rank %d competitor of Crop %s"
                       % (scenario, year, e["layer"], e["rank"], e["cropTreeId"]))
    out["selected"] = selected
    out["selected_count"] = len(selected)
    out["not_constructible"] = not_constructible
    out["marks_to_issue"] = [] if crops_from_view else [c.tree_id for c in crops]
    return out


def parse_take(items):
    take = {}
    for item in items or []:
        if "=" not in item:
            raise ValueError("--take expects AREA=N")
        k, v = item.split("=", 1)
        if not v.isdigit():
            raise ValueError("--take count must be a non-negative integer")
        take[k] = int(v)
    return take


def main(argv):
    ap = argparse.ArgumentParser(description="Q0 rev3 rule (a) selection on one view read from stdin")
    ap.add_argument("--mode", required=True, choices=("L", "M", "ALL"))
    ap.add_argument("--areas", default="", help="comma-separated treated AreaIds (default: every area)")
    ap.add_argument("--take", action="append", help="AREA=N for mode ALL (S1)")
    ap.add_argument("--crops-from-view", action="store_true", help="S year 10: Crop set = surviving view marks")
    ap.add_argument("--scenario", required=True, choices=SCENARIOS)
    ap.add_argument("--year", required=True, type=int)
    args = ap.parse_args(argv)
    if args.crops_from_view and args.mode != "L":
        print("--crops-from-view is defined only for L's rule (Q0 rev3 step 10)", file=sys.stderr)
        return EXIT_USAGE
    if args.take and args.mode != "ALL":
        print("--take is only valid with --mode ALL", file=sys.stderr)
        return EXIT_USAGE

    raw = sys.stdin.buffer.read()
    result = {"tool": "q1-select.py", "status": "AUTHORED-NOT-EXECUTED harness; selection output",
              "algorithm": "Q0 rev3 section 6 rule (a) steps 1-4, 5(a)/(b) issue order, step 10, S1",
              "input_sha256": hashlib.sha256(raw).hexdigest(), "mode": args.mode, "scenario": args.scenario,
              "year": args.year, "crops_from_view": args.crops_from_view, "defects": []}
    try:
        take = parse_take(args.take)
        doc = loads(raw)
        view = doc["view"] if isinstance(doc, dict) and "view" in doc else doc
        result["take"] = take or None
        result["view_ecologicalYear"] = int(view["source"]["ecologicalYear"].text)
        result["view_planStateRevision"] = int(view["planStateRevision"].text)
        if result["view_ecologicalYear"] != args.year:
            raise Defect("view year %d differs from --year %d" % (result["view_ecologicalYear"], args.year))
        area_ids = [a["areaId"] for a in view["areas"]]
        if area_ids != sorted(area_ids, key=ordkey):
            raise Defect("view areas are not in ordinal AreaId order: %s" % area_ids)
        treated = [s for s in args.areas.split(",") if s] or area_ids
        for t in treated:
            if t not in area_ids:
                raise ValueError("unknown treated area " + t)
        result["treated_areas"] = [a for a in area_ids if a in treated]
        areas_out = []
        for area in view["areas"]:
            if area["areaId"] in treated:
                areas_out.append(select_area(area, args.mode, take, args.crops_from_view, args.scenario, args.year))
            else:
                areas_out.append({"areaId": area["areaId"], "treated": False,
                                  "existing_crop_marks": list(area["cropTreeIds"])})
        result["areas"] = areas_out
        # 5(a): marks for treated areas, areas ordinal, Crops ascending ordinal. 5(b): orders in sequence order.
        result["marks_to_issue"] = [{"areaId": a["areaId"], "treeId": t}
                                    for a in areas_out if a["treated"] for t in a["marks_to_issue"]]
        result["orders"] = [{"areaId": a["areaId"], "treeId": e["treeId"], "reason": e["reason"]}
                            for a in areas_out if a["treated"] for e in a["selected"]]
        result["per_area_selected"] = {a["areaId"]: (a["selected_count"] if a["treated"] else 0) for a in areas_out}
        result["no_orders_in_area"] = [a["areaId"] for a in areas_out if a["treated"] and a["selected_count"] == 0]
        result["no_intervention"] = len(result["orders"]) == 0
        nc = {a["areaId"]: a["not_constructible"] for a in areas_out if a["treated"] and a.get("not_constructible")}
        result["not_constructible"] = nc or None
    except Defect as d:
        result["defects"].append(str(d))
        sys.stdout.write(json.dumps(result, ensure_ascii=True, separators=(",", ":")) + "\n")
        print("DEFECT: " + str(d), file=sys.stderr)
        return EXIT_DEFECT
    except (ValueError, KeyError, TypeError, AttributeError, OverflowError) as e:
        print("q1-select.py input/usage error: %s: %s" % (type(e).__name__, e), file=sys.stderr)
        return EXIT_USAGE
    sys.stdout.write(json.dumps(result, ensure_ascii=True, separators=(",", ":")) + "\n")
    return EXIT_NOT_CONSTRUCTIBLE if result["not_constructible"] else EXIT_OK


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
````

### `q1-record.py` — 1145 lines, 59315 bytes

SHA-256 `4de1c3c12564dd7d00e52343a5bf6ef773ca797db049453a7ca72f14e68844d0`

````python
#!/usr/bin/env python3
"""q1-record.py -- Multi #885 Q1 transcript recorder, live invariant checker, projection and normaliser.

AUTHORED, NOT EXECUTED. Scratch harness component; not a reviewed repository feature. stdlib only.

Every JSON document is parsed with numbers kept as their exact source text (class Num), so null stays null,
0 stays the token "0", -0 keeps its sign and no value passes through a lossy conversion. float32 values are
recovered as 32-bit patterns (8 hex, big-endian word of the binary32 value, as struct.pack('>f') or the uint of
struct.pack('<f')) by exact rational rounding, double values as 64-bit patterns (16 hex). Named literals written
by the server's AllowNamedFloatingPointLiterals option ("NaN", "Infinity", "-Infinity") are recorded as
NAMED:<literal> and flagged as anomalies, never coerced.

Subcommands (see README.md):
  get FILE PATH                     print one value of a JSON file (dotted path, [i] / [-1] indexes)
  body --view FILE --route R ...    build one POST body that echoes the fence of the view in FILE
  exchange ...                      append one exchange to transcript.jsonl and run the live checks
  disclosure ...                    write the pre-Execute disclosure and check Approved.Orders == selection
  compare DIR_A DIR_B               cross-session projection comparison (repeat gate / replay gate)
  rebind ...                        replay helper: substitute only the leading session id of a recorded body
  records SESSION_DIR OUT_DIR       normalised per-tree / per-area / Fell CSVs for one session
  header-init / header-session      run header bookkeeping
  selftest                          offline float-bit unit checks (no engine)

Exit codes: 0 ok; 1 comparison mismatch; 2 usage; 20 ABORT RUN (defect/500/second writer/increase);
21 STOP SESSION (recorded, run continues).
"""
import argparse
import csv
import hashlib
import json
import os
import re
import struct
import sys
from fractions import Fraction

EXIT_OK, EXIT_MISMATCH, EXIT_USAGE, EXIT_ABORT_RUN, EXIT_STOP_SESSION = 0, 1, 2, 20, 21

PINNED_MANIFEST_HASH = "c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95"
PINNED_AREA_SEEDS = {"MA-A": 885101, "MA-B": 885202}
PINNED_GENERATOR_SEED = 8850001
SESSION_ID_RE = re.compile(r"^[0-9a-f]{32}$")
FENCE_SOURCE_MEMBERS = ("domain", "experimentId", "manifestContentHash", "areaIds", "ecologicalYear",
                        "annualRevision", "workRevision", "rngModelVersion", "growthModelVersion",
                        "p1AlgorithmVersion", "areaModelVersion", "actionEligibleDbhThresholdMm")
VIEW_ROUTE = "api/multi/view"
MANIFEST_ROUTE = "api/multi/manifest"
COMMAND_ROUTES = ("api/multi/crop/mark", "api/multi/crop/unmark", "api/multi/pending/add",
                  "api/multi/pending/remove", "api/multi/plan/approve", "api/multi/plan/cancel",
                  "api/multi/plan/execute", "api/multi/year/advance")
READ_POST_ROUTES = ("api/multi/competition",)
F32_ZERO = "00000000"

# ---------------------------------------------------------------------------------------------- JSON with exact numbers


class Num:
    """A JSON number kept as its exact source text."""
    __slots__ = ("text",)

    def __init__(self, text):
        self.text = text

    def __eq__(self, other):
        return isinstance(other, Num) and other.text == self.text

    def __hash__(self):
        return hash(self.text)

    def __repr__(self):
        return "Num(%s)" % self.text


def _pairs(pairs):
    out = {}
    for key, value in pairs:
        if key in out:
            raise ValueError("duplicate JSON member: " + key)
        out[key] = value
    return out


def _constant(name):
    raise ValueError("bare JSON constant not allowed: " + name)


def loads(raw):
    if isinstance(raw, bytes):
        raw = raw.decode("utf-8")
    return json.loads(raw, object_pairs_hook=_pairs, parse_float=Num, parse_int=Num, parse_constant=_constant)


def load_file(path):
    with open(path, "rb") as fh:
        return loads(fh.read())


def emit(value):
    """Canonical re-emission: member order and number text preserved; strings escaped one fixed way."""
    if isinstance(value, dict):
        return "{" + ",".join(json.dumps(k, ensure_ascii=True) + ":" + emit(v) for k, v in value.items()) + "}"
    if isinstance(value, list):
        return "[" + ",".join(emit(v) for v in value) + "]"
    if isinstance(value, Num):
        return value.text
    if value is None:
        return "null"
    if value is True:
        return "true"
    if value is False:
        return "false"
    if isinstance(value, str):
        return json.dumps(value, ensure_ascii=True)
    if isinstance(value, int):
        return str(value)
    raise TypeError("cannot emit %r" % (value,))


def ival(value):
    if not isinstance(value, Num) or not re.fullmatch(r"-?[0-9]+", value.text):
        raise ValueError("expected an integer JSON number, got %r" % (value,))
    return int(value.text)


def getpath(doc, path):
    cur = doc
    for part in re.findall(r"[^.\[\]]+|\[-?\d+\]", path):
        if part.startswith("["):
            cur = cur[int(part[1:-1])]
        else:
            cur = cur[part]
    return cur


# ---------------------------------------------------------------------------------------------- float bit recovery

def _pow2(k):
    return Fraction(1 << k) if k >= 0 else Fraction(1, 1 << -k)


def f32_bits_exact(text):
    """Correctly rounded binary32 bit pattern of a decimal number text, by exact rational arithmetic."""
    neg = text.lstrip().startswith("-")
    q = Fraction(text)
    if q == 0:
        return 0x80000000 if neg else 0
    a = abs(q)
    e = a.numerator.bit_length() - a.denominator.bit_length()
    if _pow2(e) > a:
        e -= 1
    while _pow2(e + 1) <= a:
        e += 1
    if e < -126:
        e = -126
    scaled = a / _pow2(e - 23)
    m = scaled.numerator // scaled.denominator
    rem = scaled - m
    if rem > Fraction(1, 2) or (rem == Fraction(1, 2) and m % 2 == 1):
        m += 1
    if m >= (1 << 24):
        m >>= 1
        e += 1
    if e > 127:
        raise OverflowError("binary32 overflow: " + text)
    bits = m if m < (1 << 23) else ((e + 127) << 23) | (m - (1 << 23))
    return bits | (0x80000000 if neg else 0)


ANOMALIES = []


def note_anomaly(kind, where, detail):
    ANOMALIES.append({"kind": kind, "where": where, "detail": detail})


def b32(value, where=""):
    """float32 field -> 'null' | 'NAMED:<lit>' | 8-hex bit pattern."""
    if value is None:
        return "null"
    if isinstance(value, str):
        note_anomaly("named-float32-literal", where, value)
        return "NAMED:" + value
    if not isinstance(value, Num):
        raise ValueError("float32 field %s is not a number: %r" % (where, value))
    bits = f32_bits_exact(value.text)
    via_struct = struct.unpack(">I", struct.pack(">f", float(value.text)))[0]
    if bits != via_struct:
        note_anomaly("float32-double-rounding-disagreement", where, "%s exact %08x struct %08x" % (value.text, bits, via_struct))
    return "%08x" % bits


def b64(value, where=""):
    """double field -> 'null' | 'NAMED:<lit>' | 16-hex bit pattern (float() is correctly rounded)."""
    if value is None:
        return "null"
    if isinstance(value, str):
        note_anomaly("named-float64-literal", where, value)
        return "NAMED:" + value
    if not isinstance(value, Num):
        raise ValueError("double field %s is not a number: %r" % (where, value))
    return struct.pack(">d", float(value.text)).hex()


def f32_value(bits_hex):
    return struct.unpack(">f", bytes.fromhex(bits_hex))[0]


def f64_value(bits_hex):
    return struct.unpack(">d", bytes.fromhex(bits_hex))[0]


def txt(value):
    return "null" if value is None else (value.text if isinstance(value, Num) else str(value))


# ---------------------------------------------------------------------------------------------- views and payloads

def view_of(payload):
    return payload.get("view") if isinstance(payload, dict) else None


def fence_of(view):
    return {"sessionId": view["sessionId"], "planStateRevision": view["planStateRevision"], "source": view["source"]}


def living_counts(view):
    return [sum(1 for t in a["trees"] if t["isLiving"]) for a in view["areas"]]


def stand_counts(view):
    return [ival(a["stand"]["livingCount"]) for a in view["areas"]]


def source_ints(view):
    s = view["source"]
    return ival(s["ecologicalYear"]), ival(s["annualRevision"]), ival(s["workRevision"])


def area_index(view):
    return {a["areaId"]: i for i, a in enumerate(view["areas"])}


def tree_map(area):
    return {t["treeId"]: t for t in area["trees"]}


def ordkey(text):
    return text.encode("utf-16-be")


# ---------------------------------------------------------------------------------------------- `get` and `body`

def cmd_get(args):
    doc = load_file(args.file)
    try:
        value = getpath(doc, args.path)
    except (KeyError, IndexError, TypeError):
        print("MISSING")
        return EXIT_USAGE
    if isinstance(value, (dict, list)):
        print(emit(value))
    else:
        print(txt(value) if not isinstance(value, bool) else ("true" if value else "false"))
    return EXIT_OK


def _cell(value):
    if isinstance(value, bool):
        return "true" if value else "false"
    text = txt(value) if not isinstance(value, (dict, list)) else emit(value)
    if "\t" in text or "\n" in text or "\r" in text:
        raise ValueError("value contains a tab or newline; refusing to emit it as TSV")
    return text


def cmd_tsv(args):
    """One line per element of the array at PATH: the named fields, tab-separated (plain JSON or harness JSON)."""
    items = getpath(load_file(args.file), args.path)
    for item in items:
        print("\t".join(_cell(item[f]) for f in args.fields))
    return EXIT_OK


def cmd_living_ids(args):
    view = view_of(load_file(args.file))
    area = view["areas"][area_index(view)[args.area]]
    for t in area["trees"]:
        if t["isLiving"]:
            print(t["treeId"])
    return EXIT_OK


def cmd_body(args):
    payload = load_file(args.view)
    view = view_of(payload)
    if view is None:
        print("body: FILE is not a view payload", file=sys.stderr)
        return EXIT_USAGE
    source = view["source"]
    for m in FENCE_SOURCE_MEMBERS:
        if m not in source or source[m] is None:
            print("body: view source lacks %s" % m, file=sys.stderr)
            return EXIT_ABORT_RUN
    if sorted(source) != sorted(FENCE_SOURCE_MEMBERS):
        print("body: view source has members %s, expected exactly %s" % (sorted(source), sorted(FENCE_SOURCE_MEMBERS)), file=sys.stderr)
        return EXIT_ABORT_RUN
    body = {"fence": fence_of(view)}
    r = args.route
    if r in ("api/multi/crop/mark", "api/multi/crop/unmark", "api/multi/pending/remove", "api/multi/competition"):
        body["areaId"], body["treeId"] = args.area, args.tree
    elif r == "api/multi/pending/add":
        body["areaId"], body["treeId"], body["reason"] = args.area, args.tree, args.reason
    elif r in ("api/multi/plan/approve", "api/multi/plan/cancel"):
        pass
    elif r == "api/multi/plan/execute":
        approved = view["approved"]
        if approved is None and not (args.plan_id and args.key):
            print("body: execute needs an approved plan in the view", file=sys.stderr)
            return EXIT_ABORT_RUN
        body["planId"] = args.plan_id or approved["planId"]
        body["executionKey"] = args.key or approved["executionKey"]
    elif r == "api/multi/year/advance":
        if args.consent not in ("true", "false"):
            print("body: advance needs --consent true|false", file=sys.stderr)
            return EXIT_USAGE
        body["consentToCancelWork"] = args.consent == "true"
    else:
        print("body: unknown route " + r, file=sys.stderr)
        return EXIT_USAGE
    for k, v in body.items():
        if v is None:
            print("body: member %s would be null" % k, file=sys.stderr)
            return EXIT_USAGE
    sys.stdout.write(emit(body))
    return EXIT_OK


# ---------------------------------------------------------------------------------------------- live exchange checks

class Violation(Exception):
    def __init__(self, severity, message):
        super().__init__(message)
        self.severity = severity


def abort(message):
    raise Violation("ABORT", message)


def require(cond, message):
    if not cond:
        abort(message)


def load_state(path):
    if os.path.exists(path):
        with open(path, "r", encoding="utf-8") as fh:
            return json.load(fh)
    return {"session_id": None, "area_ids": None, "genesis_living": None, "last_living": None,
            "last_view_file": None, "felled": {}, "natural": {}, "extinction_year": {}, "first_carry_forward_year": {},
            "manifest_checked": False, "seq": 0}


def save_state(path, state):
    tmp = path + ".tmp"
    with open(tmp, "w", encoding="utf-8") as fh:
        json.dump(state, fh, sort_keys=True, indent=1)
    os.replace(tmp, path)


def check_manifest(raw):
    text = raw.decode("ascii")
    cut = text.rindex(",\"contentHash\":\"")
    body_hash = hashlib.sha256((text[:cut] + "}").encode("ascii")).hexdigest()
    manifest = loads(raw)
    require(body_hash == manifest["contentHash"] == PINNED_MANIFEST_HASH,
            "MANIFEST HASH MISMATCH: body %s, member %s, pinned %s" % (body_hash, manifest["contentHash"], PINNED_MANIFEST_HASH))
    seeds = {a["areaId"]: ival(a["simulationSeed"]) for a in manifest["areas"]}
    require(seeds == PINNED_AREA_SEEDS, "manifest area seeds %s differ from pinned %s" % (seeds, PINNED_AREA_SEEDS))
    require(ival(manifest["generator"]["seed"]) == PINNED_GENERATOR_SEED, "manifest generator seed differs from pinned 8850001")


def check_view_common(view, state, where):
    sid = view["sessionId"]
    require(isinstance(sid, str) and SESSION_ID_RE.match(sid), "%s: sessionId %r is not 32 lowercase hex" % (where, sid))
    if state["session_id"] is None:
        state["session_id"] = sid
    require(sid == state["session_id"], "%s: sessionId changed within the session" % where)
    ids = [a["areaId"] for a in view["areas"]]
    if state["area_ids"] is None:
        require(ids == sorted(ids, key=ordkey), "%s: areas not in ordinal AreaId order" % where)
        state["area_ids"] = ids
    require(ids == state["area_ids"], "%s: area list changed" % where)
    require(view["source"]["manifestContentHash"] == PINNED_MANIFEST_HASH, "%s: view source manifest hash differs from pinned" % where)
    require(view["experiment"]["manifestContentHash"] == PINNED_MANIFEST_HASH, "%s: experiment manifest hash differs from pinned" % where)
    require(ival(view["experiment"]["generatorSeed"]) == PINNED_GENERATOR_SEED, "%s: generatorSeed is not 8850001" % where)
    for a in view["areas"]:
        require(ival(a["simulationSeed"]) == PINNED_AREA_SEEDS.get(a["areaId"]), "%s: %s seed is not pinned" % (where, a["areaId"]))
        for t in a["trees"]:
            require(t["isLiving"] is (t["death"] is None), "%s: isLiving/death disagree for %s" % (where, t["treeId"]))
        living = set(t["treeId"] for t in a["trees"] if t["isLiving"])
        for c in a["cropTreeIds"]:
            require(c in living, "%s: Crop mark %s is not a living tree" % (where, c))
    lc = living_counts(view)
    require(lc == stand_counts(view), "%s: roster living count %s != stand livingCount %s" % (where, lc, stand_counts(view)))
    if state["genesis_living"] is None:
        state["genesis_living"] = lc
    if state["last_living"] is not None:
        for i, (old, new) in enumerate(zip(state["last_living"], lc)):
            require(new <= old, "%s: LIVING COUNT INCREASED in %s: %d -> %d (no recruitment exists)" % (where, ids[i], old, new))
    # Every death must be accounted for by a recorded Fell or natural death (checked after this exchange's updates).
    return lc


def check_deaths_accounted(view, state, where):
    for a in view["areas"]:
        for t in a["trees"]:
            tid = t["treeId"]
            known = tid in state["felled"] or tid in state["natural"]
            if t["isLiving"]:
                require(not known, "%s: %s is living again after a recorded death" % (where, tid))
            else:
                require(known, "%s: %s died without a recorded Fell or natural death (second writer?)" % (where, tid))


def stand_text(area_or_change, key="stand"):
    return emit(area_or_change[key])


def check_command(route, req, payload, prev, state, where):
    view = view_of(payload)
    y0, ar0, wr0 = source_ints(prev)
    y1, ar1, wr1 = source_ints(view)
    psr0, psr1 = ival(prev["planStateRevision"]), ival(view["planStateRevision"])
    idx = area_index(view)
    if route == "api/multi/crop/mark":
        require(psr1 == psr0 + 1 and (y1, ar1, wr1) == (y0, ar0, wr0), where + ": mark must move only the plan-state revision")
        require(req["treeId"] in view["areas"][idx[req["areaId"]]]["cropTreeIds"], where + ": Crop mark absent after mark")
    elif route == "api/multi/crop/unmark":
        require(psr1 == psr0 + 1 and (y1, ar1, wr1) == (y0, ar0, wr0), where + ": unmark must move only the plan-state revision")
    elif route == "api/multi/pending/add":
        require(psr1 == psr0 + 1 and (y1, ar1, wr1) == (y0, ar0, wr0), where + ": add must move only the plan-state revision")
        expect = prev["pending"] + [{"areaId": req["areaId"], "treeId": req["treeId"], "action": "Fell", "reason": req["reason"]}]
        require(emit(view["pending"]) == emit(expect), where + ": pending list is not previous + the added order")
    elif route == "api/multi/pending/remove":
        require(psr1 == psr0 + 1, where + ": remove must move the plan-state revision")
    elif route == "api/multi/plan/approve":
        require((y1, ar1, wr1) == (y0, ar0, wr0), where + ": Approve changed year or a revision")
        require(psr1 == psr0 + 1, where + ": Approve must move the plan-state revision by 1")
        require(living_counts(view) == living_counts(prev), where + ": Approve changed a living count")
        require(emit(view["events"]) == emit(prev["events"]), where + ": Approve changed the event ledger")
        require(view["pending"] == [], where + ": Approve left pending orders")
        ap = view["approved"]
        require(ap is not None, where + ": no approved plan after Approve")
        sid = view["sessionId"]
        require(ap["planId"] == "%s:ma-plan:%d" % (sid, psr1), where + ": PlanId %r != sessionId:ma-plan:%d" % (ap["planId"], psr1))
        require(ap["executionKey"] == ap["planId"] + ":execute", where + ": executionKey != PlanId + ':execute'")
        require(emit(ap["orders"]) == emit(prev["pending"]), where + ": Approved.Orders differ from the pending orders")
        require(emit(ap["source"]) == emit(view["source"]), where + ": approved source differs from the current token")
    elif route == "api/multi/plan/cancel":
        require(view["approved"] is None and psr1 == psr0 + 1, where + ": cancel must clear approval and move plan-state +1")
        require(emit(view["pending"]) == emit(prev["pending"]), where + ": cancel returned orders to pending")
        require(emit(view["events"]) == emit(prev["events"]) and (y1, ar1, wr1) == (y0, ar0, wr0), where + ": cancel changed work state")
        require(living_counts(view) == living_counts(prev), where + ": cancel felled a tree")
    elif route == "api/multi/plan/execute":
        exe = payload["execution"]
        require(exe is not None, where + ": 200 Execute without an execution result")
        if exe["replayed"] is True:
            require(emit(view) == emit(prev), where + ": a replayed execution changed the view")
            require(any(emit(e) == emit(exe["event"]) for e in prev["events"]), where + ": replay receipt is not a recorded event")
            return
        ap = prev["approved"]
        require(ap is not None, where + ": Execute committed without a prior approved plan")
        require(y1 == y0 and ar1 == ar0 and wr1 == wr0 + 1 and psr1 == psr0 + 1,
                where + ": Execute must keep year and annual revision, move work revision +1 and plan-state +1")
        require(view["approved"] is None and len(view["events"]) == len(prev["events"]) + 1, where + ": Execute ledger/approval wrong")
        ev = exe["event"]
        require(emit(ev) == emit(view["events"][-1]), where + ": execution event != last ledger event")
        require(ev["planId"] == ap["planId"] and ev["executionKey"] == ap["executionKey"] == exe["executionKey"],
                where + ": event PlanId/key differ from the approved plan")
        require(ival(ev["ecologicalYear"]) == y0 and ival(ev["workRevision"]) == wr1, where + ": event year/work revision wrong")
        require([c["areaId"] for c in ev["areas"]] == state["area_ids"], where + ": event areas not in area order")
        orders_by_area = {}
        for o in ap["orders"]:
            orders_by_area.setdefault(o["areaId"], {})[o["treeId"]] = o
        for i, change in enumerate(ev["areas"]):
            aid = change["areaId"]
            require(emit(change["before"]) == stand_text(prev["areas"][i]), where + ": %s event Before != pre-Execute stand" % aid)
            require(emit(change["after"]) == stand_text(view["areas"][i]), where + ": %s event After != post-Execute stand" % aid)
            fell_ids = [f["treeId"] for f in change["fells"]]
            require(fell_ids == sorted(fell_ids, key=ordkey), where + ": %s Fell records not ordinal by TreeId" % aid)
            require(set(fell_ids) == set(orders_by_area.get(aid, {})), where + ": %s felled set != approved orders" % aid)
            trees = tree_map(view["areas"][i])
            for f in change["fells"]:
                tid = f["treeId"]
                require(f["areaId"] == aid and f["cause"] == "fell" and f["mechanism"] == "manual-work-plan", where + ": Fell record fields wrong for " + tid)
                require(ival(f["ecologicalYear"]) == y0 and ival(f["workRevision"]) == wr1, where + ": Fell record year/revision wrong for " + tid)
                require(f["planId"] == ev["planId"] and f["executionKey"] == ev["executionKey"], where + ": Fell record PlanId/key != its event for " + tid)
                require(f["reason"] == orders_by_area[aid][tid]["reason"], where + ": Fell reason differs from the order for " + tid)
                require(tid not in state["felled"] and tid not in state["natural"], where + ": DOUBLE APPLICATION: %s already dead" % tid)
                d = trees[tid]["death"]
                require(d is not None and ival(d["ecologicalYear"]) == y0 and d["cause"] == "fell" and d["mechanism"] == "manual-work-plan",
                        where + ": felled tree %s does not carry Death(year %d, fell)" % (tid, y0))
                state["felled"][tid] = {"area": aid, "year": y0}
    elif route == "api/multi/year/advance":
        require(y1 == y0 + 1 and ar1 == ar0 + 1 and wr1 == wr0, where + ": Advance must move year and annual revision +1 only")
        hist = view["annualHistory"]
        require(len(hist) == len(prev["annualHistory"]) + 1 and len(hist) == y1, where + ": history length != committed years")
        require(emit(hist[:-1]) == emit(prev["annualHistory"]), where + ": earlier history entries changed")
        rec = hist[-1]
        had_work = bool(prev["pending"]) or prev["approved"] is not None
        require(ival(rec["yearBefore"]) == y0 and ival(rec["yearAfter"]) == y1 and ival(rec["annualRevisionAfter"]) == ar1
                 and ival(rec["workRevision"]) == wr1, where + ": history record clocks wrong")
        require(rec["cancelledUnexecutedWork"] is had_work, where + ": cancelledUnexecutedWork != (work was pending/approved)")
        annual = payload["annual"]
        require(annual is not None and ival(annual["yearBefore"]) == y0 and ival(annual["yearAfter"]) == y1, where + ": annual result clocks wrong")
        require([a["areaId"] for a in annual["areas"]] == state["area_ids"], where + ": annual areas not in area order")
        prev_living = living_counts(prev)
        for i, (ar, hr) in enumerate(zip(annual["areas"], rec["areas"])):
            aid = ar["areaId"]
            require(hr["areaId"] == aid, where + ": history area order differs")
            rep = ar["report"]
            prev_area, cur_area = prev["areas"][i], view["areas"][i]
            if rep is None:
                require(hr["noLivingStems"] is True and prev_living[i] == 0, where + ": %s null report but area was populated" % aid)
                require(hr["naturalDeaths"] == [] and emit(hr["before"]) == emit(hr["after"]) == stand_text(prev_area),
                        where + ": %s carry-forward stand/deaths wrong" % aid)
                require(emit(cur_area["trees"]) == emit(prev_area["trees"]), where + ": %s carried-forward roster changed" % aid)
                state["first_carry_forward_year"].setdefault(aid, y1)
                continue
            require(hr["noLivingStems"] is False and prev_living[i] > 0, where + ": %s non-null report with noLivingStems" % aid)
            require(ival(rep["yearBefore"]) == y0 and ival(rep["yearAfter"]) == y1, where + ": %s report years wrong" % aid)
            require(emit(rep["before"]) == stand_text(prev_area), where + ": %s Year+1 Before != previous committed stand (event After)" % aid)
            require(emit(rep["before"]) == emit(hr["before"]) and emit(rep["after"]) == emit(hr["after"]) == stand_text(cur_area),
                     where + ": %s report/history/view stands disagree" % aid)
            require(emit(rep["deaths"]) == emit(hr["naturalDeaths"]), where + ": %s report deaths != history natural deaths" % aid)
            rows = rep["trees"]
            row_ids = [r["before"]["treeId"] for r in rows]
            require(row_ids == sorted(row_ids, key=ordkey) and len(rows) == len(prev_area["trees"]), where + ": %s audit rows not the ordinal roster" % aid)
            require(emit([r["after"] for r in rows]) == emit(cur_area["trees"]), where + ": %s view roster != report After rows" % aid)
            prev_trees = tree_map(prev_area)
            for r in rows:
                tid = r["before"]["treeId"]
                w = "%s %s y%d" % (where, tid, y1)
                if r["before"]["death"] is not None:
                    require(ival(r["neighbours"]) == 0 and b32(r["preGrowthCi"], w) == F32_ZERO and r["suppression"] is None
                            and r["deathProbability"] is None and r["mortalityRoll"] is None,
                            w + ": dead carried row lacks the placeholder CI=0/neighbours=0/null S,p,u")
                    require(emit(r["after"]) == emit(r["before"]), w + ": dead carried row changed")
                    continue
                require(prev_trees[tid]["isLiving"], w + ": evaluated row for a tree that was dead")
                require(tid not in state["felled"], w + ": FELLED TREE EVALUATED (would enter neighbour sets)")
                s, p, u = r["suppression"], r["deathProbability"], r["mortalityRoll"]
                require((s is None) == (p is None), w + ": S and p null-ness differ")
                p_bits = b64(p, w + ".p") if p is not None else None
                p_positive = p is not None and not p_bits.startswith("NAMED") and f64_value(p_bits) > 0.0
                require((u is not None) == p_positive, w + ": u must be non-null exactly when p > 0")
            deaths = rep["deaths"]
            for d in deaths:
                tid = d["treeId"]
                require(ival(d["ecologicalYear"]) == y1 and d["cause"] == "self-thinning" and d["mechanism"] in ("hazard", "boundary"),
                        where + ": natural death %s does not carry year %d / self-thinning / hazard|boundary" % (tid, y1))
                require(tid in prev_trees and prev_trees[tid]["isLiving"], where + ": natural death of a non-living tree " + tid)
                require(tid not in state["natural"] and tid not in state["felled"], where + ": DOUBLE DEATH " + tid)
                state["natural"][tid] = {"area": aid, "year": y1, "mechanism": d["mechanism"]}
            require(ival(rep["after"]["livingCount"]) == ival(rep["before"]["livingCount"]) - len(deaths),
                    where + ": %s After living != Before living - natural deaths" % aid)
            if ival(rep["after"]["livingCount"]) == 0:
                state["extinction_year"].setdefault(aid, y1)
        for i, aid in enumerate(state["area_ids"]):
            felled = sum(1 for v in state["felled"].values() if v["area"] == aid)
            natural = sum(1 for v in state["natural"].values() if v["area"] == aid)
            require(felled + natural == state["genesis_living"][i] - living_counts(view)[i],
                    where + ": %s Fell + natural deaths != genesis - living" % aid)


def cmd_exchange(args):
    sdir = args.session_dir
    state_path = os.path.join(sdir, "state.json")
    transcript = os.path.join(sdir, "transcript.jsonl")
    state = load_state(state_path)
    seq = state["seq"] + 1
    require_seq = args.seq == seq
    with open(args.response, "rb") as fh:
        raw = fh.read()
    req_text = ""
    if args.request:
        with open(args.request, "rb") as fh:
            req_text = fh.read().decode("utf-8")
    status = int(args.status)
    line = {"seq": args.seq, "kind": args.kind, "method": args.method, "route": args.route,
            "request_file": os.path.relpath(args.request, sdir) if args.request else None,
            "request_sha256": hashlib.sha256(req_text.encode("utf-8")).hexdigest() if args.request else None,
            "request": req_text if args.request else None, "status": status, "curl_rc": int(args.curl_rc),
            "response_file": os.path.relpath(args.response, sdir), "response_sha256": hashlib.sha256(raw).hexdigest(),
            "response_bytes": len(raw), "t0": args.t0, "t1": args.t1, "refusal": None, "error": None, "checks": "ok"}
    verdict = EXIT_OK
    where = "seq %d %s %s" % (args.seq, args.method, args.route)
    try:
        require(require_seq, "%s: transcript sequence gap (expected %d)" % (where, seq))
        require(int(args.curl_rc) == 0, "%s: curl exit %s: outcome UNCONFIRMED" % (where, args.curl_rc))
        require(status != 500, "%s: HTTP 500 OutcomeUnconfirmed -> run INCOMPLETE" % where)
        payload = loads(raw) if raw else None
        if isinstance(payload, dict) and isinstance(payload.get("error"), dict):
            line["error"] = payload["error"].get("code")
        require(status in (200, 409), "%s: HTTP %d (guard refusal %s): the harness sent a malformed or unadmitted request"
                % (where, status, line["error"]))
        if args.route == MANIFEST_ROUTE:
            require(status == 200, where + ": manifest not readable")
            check_manifest(raw)
            state["manifest_checked"] = True
        else:
            view = view_of(payload)
            require(view is not None, where + ": response carries no view")
            prev = load_file(os.path.join(sdir, state["last_view_file"]))["view"] if state["last_view_file"] else None
            if args.method == "POST":
                req = loads(req_text)
                if args.kind != "replay-probe":
                    require(prev is not None and emit(req["fence"]) == emit(fence_of(prev)),
                            where + ": POST fence is not the fence of the view just read (cached or reconstructed fence)")
                require(req["fence"]["sessionId"] == (prev or view)["sessionId"], where + ": fence sessionId != session")
            lc = check_view_common(view, state, where)
            refusal = payload.get("refusal")
            line["refusal"] = refusal["code"] if refusal else None
            line["year"], _, _ = source_ints(view)
            line["planStateRevision"] = ival(view["planStateRevision"])
            line["living"] = lc
            if args.method == "GET" or status == 409 or args.route in READ_POST_ROUTES:
                if prev is not None:
                    require(emit(view) == emit(prev), where + ": view changed without a committed command of this harness "
                            "(second writer or unexplained change)")
                require((status == 409) == (refusal is not None), where + ": status/refusal disagree")
            else:
                require(refusal is None, where + ": 200 with a refusal")
                require(prev is not None, where + ": a command before any view read")
                check_command(args.route, req, payload, prev, state, where)
            check_deaths_accounted(view, state, where)
            state["last_living"] = lc
            state["last_view_file"] = os.path.relpath(args.response, sdir)
    except Violation as v:
        line["checks"] = "ABORT: " + str(v)
        verdict = EXIT_ABORT_RUN
    except (ValueError, KeyError, TypeError, IndexError) as e:
        line["checks"] = "ABORT: response not in the expected shape: %s: %s" % (type(e).__name__, e)
        verdict = EXIT_ABORT_RUN
    if ANOMALIES:
        line["anomalies"] = list(ANOMALIES)
    with open(transcript, "a", encoding="utf-8") as fh:
        fh.write(json.dumps(line, ensure_ascii=True, separators=(",", ":")) + "\n")
    state["seq"] = seq
    save_state(state_path, state)
    if verdict != EXIT_OK:
        print(line["checks"], file=sys.stderr)
    print(json.dumps({"seq": args.seq, "status": status, "refusal": line["refusal"], "error": line["error"],
                      "year": line.get("year"), "living": line.get("living"), "checks": line["checks"]}, separators=(",", ":")))
    return verdict


# ---------------------------------------------------------------------------------------------- disclosure

def cmd_disclosure(args):
    sel = json.load(open(args.selection, "r", encoding="utf-8"))
    view = load_file(args.view)["view"]
    ap = view["approved"]
    orders = [{"areaId": o["areaId"], "treeId": o["treeId"], "reason": o["reason"], "action": o["action"]} for o in ap["orders"]]
    expected = [{"areaId": o["areaId"], "treeId": o["treeId"], "reason": o["reason"], "action": "Fell"} for o in sel["orders"]]
    per_area = {}
    for o in orders:
        per_area[o["areaId"]] = per_area.get(o["areaId"], 0) + 1
    match = orders == expected
    doc = {"disclosure": "Q0 rev3 section 6 step 6 - written BEFORE Execute", "scenario": args.scenario, "year": args.year,
           "selection_input_sha256": sel["input_sha256"], "mode": sel["mode"], "crops_from_view": sel["crops_from_view"],
           "areas": [{k: a.get(k) for k in ("areaId", "treated", "crops", "crop_source", "crop_acceptance_order",
                                              "competitor_lists", "deduplicated", "dropped_duplicates", "selected_count")}
                     for a in sel["areas"]],
           "approved_planId": ap["planId"], "approved_executionKey": ap["executionKey"],
           "approved_orders": orders, "approved_per_area_counts": per_area, "approved_orders_equal_selection": match}
    with open(args.out, "w", encoding="utf-8") as fh:
        json.dump(doc, fh, ensure_ascii=True, indent=1)
    if not match:
        print("DISCLOSURE: Approved.Orders differ from the frozen selection", file=sys.stderr)
        return EXIT_ABORT_RUN
    return EXIT_OK


# ---------------------------------------------------------------------------------------------- projection

def normalise_ids(value, sid, problems, where):
    """Replace the exact leading sessionId + ':' of every planId / executionKey with 'SESSION:'."""
    if isinstance(value, dict):
        out = {}
        for k, v in value.items():
            if k in ("planId", "executionKey") and isinstance(v, str):
                if v.startswith(sid + ":"):
                    v = "SESSION:" + v[len(sid) + 1:]
                else:
                    problems.append("%s.%s does not start with the session id" % (where, k))
            out[k] = normalise_ids(v, sid, problems, where + "." + k)
        return out
    if isinstance(value, list):
        return [normalise_ids(v, sid, problems, "%s[%d]" % (where, i)) for i, v in enumerate(value)]
    return value


def projection(line, sdir, sid, problems):
    """The Q1 packet section 6 projection of one exchange (request and response)."""
    where = "seq %s" % line["seq"]
    proj = {"kind": line["kind"], "method": line["method"], "route": line["route"], "status": line["status"]}
    if line["request"]:
        req = loads(line["request"])
        if isinstance(req.get("fence"), dict):
            if req["fence"].get("sessionId") != sid:
                problems.append(where + ": request fence sessionId != session id")
            req["fence"] = dict(req["fence"], sessionId="SESSION")
        proj["request"] = emit(normalise_ids(req, sid, problems, where + ".request"))
    with open(os.path.join(sdir, line["response_file"]), "rb") as fh:
        raw = fh.read()
    if line["route"] == MANIFEST_ROUTE:
        proj["manifest_sha256"] = hashlib.sha256(raw).hexdigest()
        return proj
    payload = loads(raw) if raw else None
    if not isinstance(payload, dict) or "view" not in payload:
        proj["error"] = emit(payload.get("error")) if isinstance(payload, dict) else emit(payload)
        return proj
    view = payload["view"]
    if view["sessionId"] != sid:
        problems.append(where + ": view sessionId changed")
    refusal = payload.get("refusal")
    proj["refusalCode"] = refusal["code"] if refusal else None
    proj["annual"] = emit(payload.get("annual"))
    for key in ("areas", "source", "experiment", "planStateRevision", "annualHistory"):
        proj["view." + key] = emit(view[key])
    for key in ("pending", "approved", "events"):
        proj["view." + key] = emit(normalise_ids(view[key], sid, problems, where + ".view." + key))
    proj["execution"] = emit(normalise_ids(payload.get("execution"), sid, problems, where + ".execution"))
    ap = view.get("approved")
    if ap is not None:
        if not re.fullmatch(re.escape(sid) + r":ma-plan:[0-9]+", ap["planId"]):
            problems.append(where + ": PlanId format")
        if ap["executionKey"] != ap["planId"] + ":execute":
            problems.append(where + ": executionKey != PlanId + ':execute'")
    for ev in view["events"]:
        for change in ev["areas"]:
            for f in change["fells"]:
                if f["planId"] != ev["planId"] or f["executionKey"] != ev["executionKey"]:
                    problems.append(where + ": Fell record PlanId/key != its event")
    return proj


def read_transcript(sdir):
    with open(os.path.join(sdir, "transcript.jsonl"), "r", encoding="utf-8") as fh:
        return [json.loads(l) for l in fh if l.strip()]


def session_id_of(sdir, lines):
    for line in lines:
        if line["route"] != MANIFEST_ROUTE and line["status"] in (200, 409):
            with open(os.path.join(sdir, line["response_file"]), "rb") as fh:
                return loads(fh.read())["view"]["sessionId"]
    return None


def first_difference(a, b, path=""):
    """Locate the first structural difference and classify number-text-only differences."""
    if type(a) is not type(b):
        return path, "type", None
    if isinstance(a, dict):
        if list(a) != list(b):
            return path, "member-names-or-order", None
        for k in a:
            r = first_difference(a[k], b[k], path + "." + k)
            if r:
                return r
        return None
    if isinstance(a, list):
        if len(a) != len(b):
            return path, "length %d vs %d" % (len(a), len(b)), None
        for i, (x, y) in enumerate(zip(a, b)):
            r = first_difference(x, y, "%s[%d]" % (path, i))
            if r:
                return r
        return None
    if isinstance(a, Num):
        if a.text == b.text:
            return None
        same64 = b64(a) == b64(b)
        try:
            same32 = f32_bits_exact(a.text) == f32_bits_exact(b.text)
        except OverflowError:
            same32 = False
        kind = "serialiser-only (bit patterns equal)" if (same64 and same32) else "value"
        return path, kind, {"a": a.text, "b": b.text, "a_f64": b64(a), "b_f64": b64(b)}
    return (None if a == b else (path, "value", {"a": a, "b": b}))


def load_outcome(sdir):
    p = os.path.join(sdir, "session.json")
    if os.path.exists(p):
        with open(p, "r", encoding="utf-8") as fh:
            return json.load(fh).get("outcome")
    return None


def cmd_compare(args):
    report = {"a": args.a, "b": args.b, "equal": False, "within_session_problems": {}, "mismatches": [], "notes": []}
    la, lb = read_transcript(args.a), read_transcript(args.b)
    sa, sb = session_id_of(args.a, la), session_id_of(args.b, lb)
    pa, pb = [], []
    proj_a = [projection(l, args.a, sa, pa) for l in la]
    proj_b = [projection(l, args.b, sb, pb) for l in lb]
    report["within_session_problems"] = {"a": pa, "b": pb}
    n = min(len(proj_a), len(proj_b))
    for i in range(n):
        x, y = proj_a[i], proj_b[i]
        if x == y:
            continue
        for key in sorted(set(x) | set(y)):
            if x.get(key) != y.get(key):
                entry = {"seq": la[i]["seq"], "field": key}
                xa, yb = x.get(key), y.get(key)
                if isinstance(xa, str) and isinstance(yb, str) and xa[:1] in "{[" and yb[:1] in "{[":
                    diff = first_difference(loads(xa), loads(yb))
                    if diff:
                        entry["path"], entry["class"], entry["detail"] = diff
                else:
                    entry["a"], entry["b"] = xa, yb
                report["mismatches"].append(entry)
        if len(report["mismatches"]) > 50:
            report["notes"].append("mismatch list truncated at 50")
            break
    if len(proj_a) != len(proj_b):
        oa, ob = load_outcome(args.a), load_outcome(args.b)
        msg = "transcript lengths differ: %d vs %d (outcomes %s / %s)" % (len(proj_a), len(proj_b), oa, ob)
        stopped = [o for o in (oa, ob) if o and o != "COMPLETE"]
        if stopped and not report["mismatches"]:
            report["notes"].append(msg + "; common prefix equal; a stop truncated one session")
            report["prefix_only"] = True
        else:
            report["mismatches"].append({"field": "length", "detail": msg})
    report["compared_exchanges"] = n
    report["equal"] = not report["mismatches"] and not pa and not pb
    out = json.dumps(report, ensure_ascii=True, indent=1, default=str)
    if args.out:
        with open(args.out, "w", encoding="utf-8") as fh:
            fh.write(out)
    print(out)
    return EXIT_OK if report["equal"] else EXIT_MISMATCH


def cmd_rebind(args):
    """Replay: substitute ONLY the leading old session id in fence.sessionId and planId/executionKey."""
    with open(args.body, "rb") as fh:
        body = loads(fh.read())
    if "fence" in body:
        if body["fence"]["sessionId"] != args.old_sid:
            print("rebind: recorded fence sessionId != recorded session", file=sys.stderr)
            return EXIT_ABORT_RUN
        body["fence"]["sessionId"] = args.new_sid
    for k in ("planId", "executionKey"):
        if k in body:
            if not body[k].startswith(args.old_sid + ":"):
                print("rebind: recorded %s does not start with the recorded session id" % k, file=sys.stderr)
                return EXIT_ABORT_RUN
            body[k] = args.new_sid + body[k][len(args.old_sid):]
    sys.stdout.write(emit(body))
    return EXIT_OK


# ---------------------------------------------------------------------------------------------- normalised records

PER_TREE_COLUMNS = ["scenario", "repeat", "areaId", "yearBefore", "yearAfter", "treeId", "row_kind",
                    "ageYears_before", "ageYears_after", "dbhCm_before_f32", "dbhCm_after_f32", "heightM_before_f32",
                    "heightM_after_f32", "crownRadiusM_before_f32", "crownRadiusM_after_f32", "xm_f32", "zm_f32",
                    "neighbours", "preGrowthCi_f32", "suppression_f64", "deathProbability_f64", "mortalityRoll_f32",
                    "death_year", "death_cause", "death_mechanism",
                    "dbhCm_before", "dbhCm_after", "heightM_after", "crownRadiusM_after", "preGrowthCi",
                    "suppression", "deathProbability", "mortalityRoll"]
NOT_EVALUATED = "not-evaluated"
CARRY_MARKER = "carried-forward-no-per-tree-audit"


def committed_advances(sdir):
    """(transcript line, payload) of every committed Year+1, in commit order, from the transcript itself."""
    out = []
    for line in read_transcript(sdir):
        if line["route"] == "api/multi/year/advance" and line["status"] == 200:
            out.append((line, load_file(os.path.join(sdir, line["response_file"]))))
    return out


def per_tree_rows(sdir, scenario, repeat):
    rows = []
    for line, payload in committed_advances(sdir):
        annual = payload["annual"]
        yb, ya = txt(annual["yearBefore"]), txt(annual["yearAfter"])
        for ar in annual["areas"]:
            aid, rep = ar["areaId"], ar["report"]
            if rep is None:
                row = {c: "n/a" for c in PER_TREE_COLUMNS}
                row.update(scenario=scenario, repeat=repeat, areaId=aid, yearBefore=yb, yearAfter=ya, treeId="", row_kind=CARRY_MARKER)
                rows.append(row)
                continue
            for r in rep["trees"]:
                b, a = r["before"], r["after"]
                w = "%s/%s/%s y%s %s" % (scenario, repeat, aid, ya, b["treeId"])
                dead_carried = b["death"] is not None
                death = a["death"]
                row = {"scenario": scenario, "repeat": repeat, "areaId": aid, "yearBefore": yb, "yearAfter": ya,
                       "treeId": b["treeId"], "row_kind": "dead-carried" if dead_carried else "evaluated",
                       "ageYears_before": txt(b["ageYears"]), "ageYears_after": txt(a["ageYears"]),
                       "dbhCm_before_f32": b32(b["dbhCm"], w), "dbhCm_after_f32": b32(a["dbhCm"], w),
                       "heightM_before_f32": b32(b["heightM"], w), "heightM_after_f32": b32(a["heightM"], w),
                       "crownRadiusM_before_f32": b32(b["crownRadiusM"], w), "crownRadiusM_after_f32": b32(a["crownRadiusM"], w),
                       "xm_f32": b32(b["xm"], w), "zm_f32": b32(b["zm"], w),
                       "neighbours": NOT_EVALUATED if dead_carried else txt(r["neighbours"]),
                       "preGrowthCi_f32": NOT_EVALUATED if dead_carried else b32(r["preGrowthCi"], w),
                       "suppression_f64": NOT_EVALUATED if dead_carried else b64(r["suppression"], w),
                       "deathProbability_f64": NOT_EVALUATED if dead_carried else b64(r["deathProbability"], w),
                       "mortalityRoll_f32": NOT_EVALUATED if dead_carried else b32(r["mortalityRoll"], w),
                       "death_year": txt(death["ecologicalYear"]) if death else "null",
                       "death_cause": death["cause"] if death else "null",
                       "death_mechanism": death["mechanism"] if death else "null",
                       "dbhCm_before": txt(b["dbhCm"]), "dbhCm_after": txt(a["dbhCm"]), "heightM_after": txt(a["heightM"]),
                       "crownRadiusM_after": txt(a["crownRadiusM"]),
                       "preGrowthCi": NOT_EVALUATED if dead_carried else txt(r["preGrowthCi"]),
                       "suppression": NOT_EVALUATED if dead_carried else txt(r["suppression"]),
                       "deathProbability": NOT_EVALUATED if dead_carried else txt(r["deathProbability"]),
                       "mortalityRoll": NOT_EVALUATED if dead_carried else txt(r["mortalityRoll"])}
                rows.append(row)
    return rows


STAND_FIELDS = ("stemsPerHa", "basalAreaM2PerHa", "quadraticMeanDbhCm", "relativeDensity")
TRAJECTORY_COLUMNS = (["scenario", "repeat", "areaId", "year", "state", "livingCount", "hazardDeaths", "boundaryDeaths",
                       "cumNaturalDeaths", "cumFell", "fellAtThisYear", "extinctionYear", "firstCarryForwardYear"]
                      + [f + "_f64" for f in STAND_FIELDS] + ["postGrowthDensityPressure_f64", "meanVulnerability_f64",
                       "meanDbhCm_harness_f64"] + list(STAND_FIELDS) + ["postGrowthDensityPressure", "meanVulnerability",
                       "meanDbhCm_harness"])


def mean_dbh(area):
    """Harness arithmetic mean DBH of the living roster, in double over float32 values, roster order; empty -> None."""
    vals = [f32_value(b32(t["dbhCm"])) for t in area["trees"] if t["isLiving"]]
    if not vals:
        return None
    total = 0.0
    for v in vals:
        total += v
    return total / len(vals)


def year_views(sdir):
    """Committed state at each year y: the view after the y-th committed Advance (year 0 = first view read)."""
    views = {}
    for line in read_transcript(sdir):
        if line["route"] == VIEW_ROUTE and line["status"] == 200 and line.get("year") is not None:
            views.setdefault(line["year"], os.path.join(sdir, line["response_file"]))
    return views


def trajectory_rows(sdir, scenario, repeat):
    advances = {ival(p["annual"]["yearAfter"]): p for _, p in committed_advances(sdir)}
    fells = fell_rows(sdir, scenario, repeat)
    views = year_views(sdir)
    rows = []
    cum_nat, cum_fell = {}, {}
    ext, cf = {}, {}
    for y in sorted(advances):
        for ar in advances[y]["annual"]["areas"]:
            if ar["report"] is None:
                cf.setdefault(ar["areaId"], y)
            elif ival(ar["report"]["after"]["livingCount"]) == 0:
                ext.setdefault(ar["areaId"], y)
    for y in sorted(views):
        v = load_file(views[y])["view"]
        p = advances.get(y)
        for i, area in enumerate(v["areas"]):
            aid = area["areaId"]
            rep = p["annual"]["areas"][i]["report"] if p else None
            hz = bd = 0
            if rep is not None:
                hz = sum(1 for d in rep["deaths"] if d["mechanism"] == "hazard")
                bd = sum(1 for d in rep["deaths"] if d["mechanism"] == "boundary")
            cum_nat[aid] = cum_nat.get(aid, 0) + hz + bd
            fell_y = sum(1 for f in fells if f["areaId"] == aid and int(f["ecologicalYear"]) == y)
            living = ival(area["stand"]["livingCount"])
            if y == 0 and p is None:
                state = "genesis"
            elif p is None:
                state = "unknown"
            elif rep is None:
                state = "carried-forward"
            elif ival(rep["after"]["livingCount"]) == 0:
                state = "extinct-this-year"
            else:
                state = "living"
            row = {"scenario": scenario, "repeat": repeat, "areaId": aid, "year": y, "state": state, "livingCount": living,
                   "hazardDeaths": hz if (p is not None and rep is not None) else (NOT_EVALUATED if p is not None else "n/a"),
                   "boundaryDeaths": bd if (p is not None and rep is not None) else (NOT_EVALUATED if p is not None else "n/a"),
                   "cumNaturalDeaths": cum_nat[aid], "cumFell": cum_fell.get(aid, 0), "fellAtThisYear": fell_y,
                   "extinctionYear": ext.get(aid, "none"), "firstCarryForwardYear": cf.get(aid, "none")}
            cum_fell[aid] = cum_fell.get(aid, 0) + fell_y
            stand = area["stand"]
            for f in STAND_FIELDS:
                row[f + "_f64"] = b64(stand[f])
                row[f] = txt(stand[f])
            if living == 0:
                row["quadraticMeanDbhCm"] = "null"      # mean over an empty set; engine placeholder bits kept in _f64
            if rep is not None:
                row["postGrowthDensityPressure_f64"] = b64(rep["postGrowthDensityPressure"])
                row["meanVulnerability_f64"] = b64(rep["meanVulnerability"])
                row["postGrowthDensityPressure"] = txt(rep["postGrowthDensityPressure"])
                row["meanVulnerability"] = txt(rep["meanVulnerability"])
            else:
                for k in ("postGrowthDensityPressure_f64", "meanVulnerability_f64", "postGrowthDensityPressure", "meanVulnerability"):
                    row[k] = NOT_EVALUATED
            m = mean_dbh(area)
            row["meanDbhCm_harness_f64"] = "null" if m is None else struct.pack(">d", m).hex()
            row["meanDbhCm_harness"] = "null" if m is None else repr(m)
            rows.append(row)
        # cumFell in the row is the count BEFORE this year's intervention; fellAtThisYear is the year-y Execute.
    return rows


FELL_COLUMNS = ["scenario", "repeat", "ecologicalYear", "workRevision", "planId_normalised", "executionKey_normalised",
                "areaId", "treeId", "cause", "mechanism", "reason", "record_matches_event"]


def fell_rows(sdir, scenario, repeat):
    lines = read_transcript(sdir)
    sid = session_id_of(sdir, lines)
    rows = []
    for line in lines:
        if line["route"] != "api/multi/plan/execute" or line["status"] != 200:
            continue
        payload = load_file(os.path.join(sdir, line["response_file"]))
        exe = payload["execution"]
        if exe["replayed"] is True:
            continue
        ev = exe["event"]
        for change in ev["areas"]:
            for f in change["fells"]:
                rows.append({"scenario": scenario, "repeat": repeat, "ecologicalYear": txt(f["ecologicalYear"]),
                             "workRevision": txt(f["workRevision"]),
                             "planId_normalised": f["planId"].replace(sid + ":", "SESSION:", 1),
                             "executionKey_normalised": f["executionKey"].replace(sid + ":", "SESSION:", 1),
                             "areaId": f["areaId"], "treeId": f["treeId"], "cause": f["cause"], "mechanism": f["mechanism"],
                             "reason": f["reason"],
                             "record_matches_event": f["planId"] == ev["planId"] and f["executionKey"] == ev["executionKey"]})
    return rows


def write_csv(path, columns, rows):
    with open(path, "w", newline="", encoding="utf-8") as fh:
        w = csv.DictWriter(fh, fieldnames=columns, extrasaction="raise", lineterminator="\n")
        w.writeheader()
        for r in rows:
            w.writerow(r)


def cmd_records(args):
    meta = json.load(open(os.path.join(args.session_dir, "session.json"), "r", encoding="utf-8"))
    sc, rp = meta["scenario"], meta["repeat"]
    os.makedirs(args.out_dir, exist_ok=True)
    write_csv(os.path.join(args.out_dir, "per_tree_year.csv"), PER_TREE_COLUMNS, per_tree_rows(args.session_dir, sc, rp))
    write_csv(os.path.join(args.out_dir, "trajectories.csv"), TRAJECTORY_COLUMNS, trajectory_rows(args.session_dir, sc, rp))
    write_csv(os.path.join(args.out_dir, "fells.csv"), FELL_COLUMNS, fell_rows(args.session_dir, sc, rp))
    if ANOMALIES:
        json.dump(ANOMALIES, open(os.path.join(args.out_dir, "record-anomalies.json"), "w"), indent=1)
    return EXIT_OK


# ---------------------------------------------------------------------------------------------- header bookkeeping

def cmd_header_init(args):
    doc = {"status": "PRODUCED by an AUTHORED harness; not reviewed, not verified", "pairs": {}}
    for kv in args.set or []:
        k, v = kv.split("=", 1)
        doc["pairs"][k] = v
    doc["session_start_order"] = []
    with open(args.file, "w", encoding="utf-8") as fh:
        json.dump(doc, fh, indent=1, sort_keys=True)
    return EXIT_OK


def cmd_header_session(args):
    with open(args.file, "r", encoding="utf-8") as fh:
        doc = json.load(fh)
    doc["session_start_order"].append({"scenario": args.scenario, "repeat": args.repeat, "pid": args.pid,
                                       "port": args.port, "started": args.started})
    tmp = args.file + ".tmp"
    with open(tmp, "w", encoding="utf-8") as fh:
        json.dump(doc, fh, indent=1, sort_keys=True)
    os.replace(tmp, args.file)
    return EXIT_OK


def cmd_selftest(_args):
    cases = {"0": "00000000", "-0": "80000000", "1": "3f800000", "0.1": "3dcccccd", "1E-45": "00000001",
             "1.4E-45": "00000001", "3.4028235E+38": "7f7fffff", "16777217": "4b800000", "-2.5": "c0200000",
             "1.17549435E-38": "00800000"}
    bad = [(t, "%08x" % f32_bits_exact(t), want) for t, want in cases.items() if "%08x" % f32_bits_exact(t) != want]
    if b64(Num("-0")) != "8000000000000000" or b64(Num("0.1")) != "3fb999999999999a":
        bad.append(("b64", "", ""))
    print("selftest " + ("PASS" if not bad else "FAIL %r" % bad))
    return EXIT_OK if not bad else EXIT_MISMATCH


# ---------------------------------------------------------------------------------------------- CLI

def main(argv):
    ap = argparse.ArgumentParser(description=__doc__.split("\n")[0])
    sub = ap.add_subparsers(dest="cmd", required=True)
    p = sub.add_parser("get"); p.add_argument("file"); p.add_argument("path"); p.set_defaults(fn=cmd_get)
    p = sub.add_parser("tsv"); p.add_argument("file"); p.add_argument("path"); p.add_argument("fields", nargs="+")
    p.set_defaults(fn=cmd_tsv)
    p = sub.add_parser("living-ids"); p.add_argument("file"); p.add_argument("area"); p.set_defaults(fn=cmd_living_ids)
    p = sub.add_parser("body"); p.add_argument("--view", required=True); p.add_argument("--route", required=True)
    p.add_argument("--area"); p.add_argument("--tree"); p.add_argument("--reason"); p.add_argument("--consent")
    p.add_argument("--plan-id"); p.add_argument("--key"); p.set_defaults(fn=cmd_body)
    p = sub.add_parser("exchange")
    for a in ("--session-dir", "--kind", "--method", "--route", "--response", "--status", "--t0", "--t1", "--curl-rc"):
        p.add_argument(a, required=True)
    p.add_argument("--seq", required=True, type=int); p.add_argument("--request", default="")
    p.set_defaults(fn=cmd_exchange)
    p = sub.add_parser("disclosure"); p.add_argument("--selection", required=True); p.add_argument("--view", required=True)
    p.add_argument("--scenario", required=True); p.add_argument("--year", required=True, type=int); p.add_argument("--out", required=True)
    p.set_defaults(fn=cmd_disclosure)
    p = sub.add_parser("compare"); p.add_argument("a"); p.add_argument("b"); p.add_argument("--out"); p.set_defaults(fn=cmd_compare)
    p = sub.add_parser("rebind"); p.add_argument("--body", required=True); p.add_argument("--old-sid", required=True)
    p.add_argument("--new-sid", required=True); p.set_defaults(fn=cmd_rebind)
    p = sub.add_parser("records"); p.add_argument("session_dir"); p.add_argument("out_dir"); p.set_defaults(fn=cmd_records)
    p = sub.add_parser("header-init"); p.add_argument("--file", required=True); p.add_argument("--set", action="append")
    p.set_defaults(fn=cmd_header_init)
    p = sub.add_parser("header-session"); p.add_argument("--file", required=True)
    for a in ("--scenario", "--repeat", "--pid", "--port", "--started"):
        p.add_argument(a, required=True)
    p.set_defaults(fn=cmd_header_session)
    p = sub.add_parser("selftest"); p.set_defaults(fn=cmd_selftest)
    args = ap.parse_args(argv)
    return args.fn(args)


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
````

### `q1-analyse.py` — 759 lines, 39486 bytes

SHA-256 `b7f8740809617228d5cd14c386e36412108897ab42ff8cfcf11bfdf9b15b6165`

````python
#!/usr/bin/env python3
"""q1-analyse.py -- Multi #885 Q1 offline diagnostics, trajectory tables, isolation and S1 sizing.

AUTHORED, NOT EXECUTED. Scratch harness component; not a reviewed repository feature. stdlib only. Reads only the run
bundle; never contacts the engine. Every diagnostic is a DIAGNOSTIC only (Q1 packet section 8): none establishes
empirical validity and none is a pass/fail gate for the engine. No p-values are computed (census, not sample).

Subcommands:
  all --run-dir RUN [--isolation equal|DEFECT|not-run]   analysis/ CSVs, diagnostics.md, anomalies.md, run-verdict.json
  isolation --i SESSION_I --c SESSION_C                  I vs C MA-B byte/bit equality (exit 1 on any difference)
  s1-size --session SESSION_S                            prints MA-A=n / MA-B=n from S's realised Fells (exit 4 if not sizable)
  s1-check --s SEL_S_Y00 --s1 SEL_S1_Y00                 S1 and S share the year-0 Crop set and deduplicated sequence
  transcript-tsv --session DIR                           seq, kind, method, route, request_file per exchange (for replay)
"""
import argparse
import csv
import glob
import importlib.util
import json
import math
import os
import struct
import sys

_HERE = os.path.dirname(os.path.abspath(__file__))
_spec = importlib.util.spec_from_file_location("q1_record", os.path.join(_HERE, "q1-record.py"))
R = importlib.util.module_from_spec(_spec)
_spec.loader.exec_module(R)

SCENARIOS = ["C", "L", "M", "D", "S", "S1", "I"]
CHECKPOINTS = [1, 5, 10, 20, 50]
B_AREA = "MA-B"
ISOLATION_EXCLUDED = ["WorkRevision", "PlanStateRevision", "event ledger (Events)", "CancelledUnexecutedWork",
                      "SessionId", "Pending/Approved (MA-A orders only)", "MA-A area and MA-A report"]

# ---------------------------------------------------------------------------------------------- float32 emulation


def f32(x):
    return struct.unpack("<f", struct.pack("<f", x))[0]


def fb(bits_hex):
    return R.f32_value(bits_hex)


C_1_2 = f32(1.2)        # AnnualRules.cs:10 PotentialDbhGrowthCm = 1.2f
CI50 = 5.0              # AnnualRules.cs:9  Ci50 = 5f
DBH_SCALE = 120.0       # AnnualRules.cs:11 DbhGrowthScaleCm = 120f


def e3_distance(xt, zt, xo, zo):
    dx = f32(xt - xo)
    dz = f32(zt - zo)
    return f32(math.sqrt(f32(f32(dx * dx) + f32(dz * dz))))


def grow_dbh(d, ci, site):
    """AnnualRules.Grow DBH in coded float32 order (AnnualRules.cs:86-91)."""
    response = f32(1.0 / f32(1.0 + f32(ci / CI50)))
    taper = min(max(f32(1.0 - f32(d / DBH_SCALE)), 0.0), 1.0)
    potential = f32(f32(C_1_2 * site) * taper)
    return min(max(f32(d + f32(potential * response)), 1.0), 200.0)


def ulp32(x):
    bits = struct.unpack(">I", struct.pack(">f", abs(x)))[0]
    return struct.unpack(">f", struct.pack(">I", bits + 1))[0] - abs(x)


# ---------------------------------------------------------------------------------------------- session discovery

def sessions(run_dir):
    out = []
    for path in sorted(glob.glob(os.path.join(run_dir, "sessions", "*", "r*", "session.json"))):
        meta = json.load(open(path, "r", encoding="utf-8"))
        out.append((meta, os.path.dirname(path)))
    return out


def session_dir(run_dir, scenario, repeat=1):
    d = os.path.join(run_dir, "sessions", scenario, "r%d" % repeat)
    return d if os.path.exists(os.path.join(d, "transcript.jsonl")) else None


def manifest_of(sdir):
    for line in R.read_transcript(sdir):
        if line["route"] == R.MANIFEST_ROUTE and line["status"] == 200:
            return R.load_file(os.path.join(sdir, line["response_file"]))
    raise ValueError("no manifest exchange in " + sdir)


def reports(sdir):
    """[(yearBefore, areaId, report-or-None)] in commit order."""
    out = []
    for _, payload in R.committed_advances(sdir):
        for ar in payload["annual"]["areas"]:
            out.append((R.ival(payload["annual"]["yearBefore"]), ar["areaId"], ar["report"]))
    return out


def rosters_by_year(sdir):
    """{year: {areaId: {treeId: tree}}} from the committed year views."""
    out = {}
    for y, path in R.year_views(sdir).items():
        view = R.load_file(path)["view"]
        out[y] = {a["areaId"]: R.tree_map(a) for a in view["areas"]}
    return out


# ---------------------------------------------------------------------------------------------- E3 neighbourhoods

class Neighbourhood:
    """Fixed positions (trees never move) -> per tree, the ordinal list of (neighbourId, f32 distance <= 8 m)."""

    def __init__(self, area_trees):
        self.pos = {t["treeId"]: (fb(R.b32(t["xm"])), fb(R.b32(t["zm"]))) for t in area_trees}
        ids = sorted(self.pos, key=R.ordkey)
        self.neigh = {}
        for i in ids:
            xi, zi = self.pos[i]
            lst = []
            for j in ids:
                if j == i:
                    continue
                d = e3_distance(xi, zi, *self.pos[j])
                if d <= 8.0:
                    lst.append((j, d))
            self.neigh[i] = lst

    def ci(self, target, living, dbh, scale=None):
        """Hegyi CI as AnnualRules.Competition: float32 accumulation over living neighbours in ordinal order."""
        d_t = dbh[target] if scale is None else f32(dbh[target] * scale)
        acc, n = 0.0, 0
        for j, d in self.neigh[target]:
            if j not in living:
                continue
            d_j = dbh[j] if scale is None else f32(dbh[j] * scale)
            acc = f32(acc + f32(f32(d_j / max(1.0, d_t)) / max(0.5, d)))
            n += 1
        return acc, n


def hw2(sdir, anomalies, label):
    """H-W2(a) CI scale invariance, H-W2(b) DBH increment reproduction, plus the offline E3 equality integrity check."""
    manifest = manifest_of(sdir)
    site = fb(manifest["models"]["siteProductivityInEngine"]["b"])
    lam = {"1.1": f32(1.1), "1.5": f32(1.5)}
    hoods = {}
    res = {"a": {k: {"evaluated": 0, "rejects": [], "max_abs_dev": 0.0} for k in lam},
           "b": {"evaluated": 0, "rejects": []}, "e3": {"evaluated": 0, "mismatches": []}}
    for yb, aid, rep in reports(sdir):
        if rep is None:
            continue
        rows = rep["trees"]
        if aid not in hoods:
            hoods[aid] = Neighbourhood([r["before"] for r in rows])
        hood = hoods[aid]
        living = {r["before"]["treeId"] for r in rows if r["before"]["death"] is None}
        dbh = {r["before"]["treeId"]: fb(R.b32(r["before"]["dbhCm"])) for r in rows}
        for r in rows:
            b = r["before"]
            tid = b["treeId"]
            if (fb(R.b32(b["xm"])), fb(R.b32(b["zm"]))) != hood.pos.get(tid):
                anomalies.append("%s %s y%d %s: position changed" % (label, aid, yb, tid))
            if b["death"] is not None:
                continue
            ci, n = hood.ci(tid, living, dbh)
            res["e3"]["evaluated"] += 1
            eng_ci, eng_n = fb(R.b32(r["preGrowthCi"])), R.ival(r["neighbours"])
            if struct.pack(">f", ci) != struct.pack(">f", eng_ci) or n != eng_n:
                res["e3"]["mismatches"].append("%s y%d %s: offline CI %s n=%d vs engine %s n=%d"
                                               % (aid, yb, tid, struct.pack(">f", ci).hex(), n, R.b32(r["preGrowthCi"]), eng_n))
            for key, l in lam.items():
                if f32(dbh[tid] * l) < 1.0:
                    continue
                ci_l, _ = hood.ci(tid, living, dbh, scale=l)
                dev = abs(ci_l - eng_ci)
                a = res["a"][key]
                a["evaluated"] += 1
                a["max_abs_dev"] = max(a["max_abs_dev"], dev)
                if dev > 1e-5 * max(eng_ci, 1.0):
                    a["rejects"].append("%s y%d %s CI=%r CI_lambda=%r" % (aid, yb, tid, eng_ci, ci_l))
            pred = grow_dbh(dbh[tid], eng_ci, site)
            obs = fb(R.b32(r["after"]["dbhCm"]))
            res["b"]["evaluated"] += 1
            if abs(obs - pred) > ulp32(pred):
                res["b"]["rejects"].append("%s y%d %s obs %s pred %s" % (aid, yb, tid, struct.pack(">f", obs).hex(), struct.pack(">f", pred).hex()))
    return res


# ---------------------------------------------------------------------------------------------- H-W3

def hw3(sdir):
    windows = {}
    for yb, aid, rep in reports(sdir):
        if rep is None:
            continue
        for d in rep["deaths"]:
            y = R.ival(d["ecologicalYear"])
            w = "%d-%d" % (((y - 1) // 10) * 10 + 1, ((y - 1) // 10) * 10 + 10)
            for key in ((aid, w), (aid, "whole"), ("both", w), ("both", "whole")):
                c = windows.setdefault(key, {"hazard": 0, "boundary": 0})
                c[d["mechanism"]] += 1
    out = []
    for (aid, w), c in sorted(windows.items()):
        nat = c["hazard"] + c["boundary"]
        flag = w != "whole" and nat >= 10 and c["boundary"] > 0.5 * nat
        out.append({"area": aid, "window": w, "hazard": c["hazard"], "boundary": c["boundary"], "natural": nat,
                    "flag_boundary_dominated": flag})
    return out


# ---------------------------------------------------------------------------------------------- H-W4

def point_segment_mm(px, py, ax, ay, bx, by):
    dx, dy = bx - ax, by - ay
    L2 = dx * dx + dy * dy
    t = 0.0 if L2 == 0 else max(0.0, min(1.0, ((px - ax) * dx + (py - ay) * dy) / L2))
    cx, cy = ax + t * dx, ay + t * dy
    return math.hypot(px - cx, py - cy)


def edge_and_strata(manifest):
    """{treeId: (areaId, genesisAge, edge 0/1)}; edge = genesis integer-mm distance to own polygon edge < 8 m (Q0 H-W4)."""
    out = {}
    for area in manifest["areas"]:
        poly = [(float(R.ival(p["xMm"])), float(R.ival(p["yMm"]))) for p in area["polygonMm"]]
        for t in area["trees"]:
            px, py = float(R.ival(t["xMm"])), float(R.ival(t["yMm"]))
            d = min(point_segment_mm(px, py, *poly[k], *poly[(k + 1) % len(poly)]) for k in range(len(poly)))
            out[t["treeId"]] = (area["areaId"], R.ival(t["ageYears"]), 1 if d / 1000.0 < 8.0 else 0)
    return out


def jacobi_eigenvalues(a):
    a = [row[:] for row in a]
    n = len(a)
    for _ in range(100):
        off = max((abs(a[i][j]), i, j) for i in range(n) for j in range(n) if i != j)
        if off[0] < 1e-300:
            break
        _, p, q = off
        theta = 0.5 * math.atan2(2 * a[p][q], a[q][q] - a[p][p])
        c, s = math.cos(theta), math.sin(theta)
        for k in range(n):
            akp, akq = a[k][p], a[k][q]
            a[k][p], a[k][q] = c * akp - s * akq, s * akp + c * akq
        for k in range(n):
            apk, aqk = a[p][k], a[q][k]
            a[p][k], a[q][k] = c * apk - s * aqk, s * apk + c * aqk
    return [a[i][i] for i in range(n)]


def solve3(m, v):
    a = [m[i][:] + [v[i]] for i in range(3)]
    for c in range(3):
        p = max(range(c, 3), key=lambda r: abs(a[r][c]))
        a[c], a[p] = a[p], a[c]
        for r in range(3):
            if r != c:
                f = a[r][c] / a[c][c]
                a[r] = [x - f * y for x, y in zip(a[r], a[c])]
    return [a[i][3] / a[i][i] for i in range(3)]


def ols_edge(samples):
    """samples: [(y, edge, D)] -> dict with coefficient on edge, interior mean, or NOT ESTIMABLE with reason."""
    n_edge = sum(1 for _, e, _ in samples if e == 1)
    n_int = len(samples) - n_edge
    base = {"n_edge": n_edge, "n_interior": n_int}
    if n_edge < 2 or n_int < 2:
        return dict(base, estimable=False, reason="(i) fewer than 2 edge or interior trees")
    X = [[1.0, float(e), math.log(d)] for _, e, d in samples]
    y = [s[0] for s in samples]
    xtx = [[sum(r[i] * r[j] for r in X) for j in range(3)] for i in range(3)]
    xty = [sum(r[i] * yy for r, yy in zip(X, y)) for i in range(3)]
    sv = sorted(math.sqrt(max(0.0, e)) for e in jacobi_eigenvalues(xtx))
    if sv[-1] == 0 or sv[0] <= 1e-9 * sv[-1]:
        return dict(base, estimable=False, reason="(ii) rank-deficient design (singular values %r)" % sv)
    coef = solve3(xtx, xty)
    interior_mean = sum(yy for (yy, e, _) in samples if e == 0) / n_int
    if interior_mean <= 0:
        return dict(base, estimable=False, reason="(iii) interior mean <= 0", interior_mean=interior_mean)
    return dict(base, estimable=True, a=coef[0], edge_coef=coef[1], c=coef[2], interior_mean=interior_mean)


def hw4(sdir_c):
    manifest = manifest_of(sdir_c)
    info = edge_and_strata(manifest)
    strata = sorted({(a, age) for a, age, _ in info.values()})
    ci_out, g_out = [], []
    reps = {(yb, aid): rep for yb, aid, rep in reports(sdir_c)}
    for t in (0, 10):
        for (aid, age) in strata:
            rep = reps.get((t, aid))
            if rep is None:
                ci_out.append({"area": aid, "cohort": age, "t": t, "outcome": "NOT ESTIMABLE", "reason": "no report for year %d" % t})
                continue
            samples = []
            for r in rep["trees"]:
                b = r["before"]
                if b["death"] is not None or info[b["treeId"]][1] != age:
                    continue
                samples.append((fb(R.b32(r["preGrowthCi"])), info[b["treeId"]][2], fb(R.b32(b["dbhCm"]))))
            fit = ols_edge(samples)
            entry = dict(area=aid, cohort=age, t=t, **fit)
            if not fit["estimable"]:
                entry["outcome"] = "NOT ESTIMABLE"
            else:
                b_, m = fit["edge_coef"], fit["interior_mean"]
                entry["outcome"] = "NO BIAS" if b_ >= 0 else ("BIAS" if abs(b_) > 0.20 * m else "SMALL")
            ci_out.append(entry)
    rost = rosters_by_year(sdir_c)
    for (aid, age) in strata:
        if 0 not in rost or 10 not in rost:
            g_out.append({"area": aid, "cohort": age, "outcome": "NOT ESTIMABLE", "reason": "year 0 or 10 roster missing"})
            continue
        r0, r10 = rost[0][aid], rost[10][aid]
        samples, group = [], {0: [0, 0], 1: [0, 0]}
        for tid, t0 in r0.items():
            if info[tid][1] != age or not t0["isLiving"]:
                continue
            e = info[tid][2]
            group[e][1] += 1
            t10 = r10[tid]
            if t10["isLiving"]:
                group[e][0] += 1
                d0 = fb(R.b32(t0["dbhCm"]))
                samples.append((fb(R.b32(t10["dbhCm"])) - d0, e, d0))
        fit = ols_edge(samples)
        entry = dict(area=aid, cohort=age, retained_share_edge="%d/%d" % tuple(group[1]),
                     retained_share_interior="%d/%d" % tuple(group[0]), **fit)
        if not fit["estimable"]:
            entry["outcome"] = "NOT ESTIMABLE"
        else:
            g, m = fit["edge_coef"], fit["interior_mean"]
            entry["outcome"] = "NO ADVANTAGE" if g <= 0 else ("GROWTH ADVANTAGE" if g > 0.20 * m else "SMALL")
        g_out.append(entry)

    def overall(entries, yes, no):
        outs = [e["outcome"] for e in entries]
        if any(o == "NOT ESTIMABLE" for o in outs):
            return "INCOMPLETE"
        if all(o == yes for o in outs):
            return "CONFIRM"
        if all(o == no for o in outs):
            return "REJECT"
        return "MIXED"
    return {"strata": strata, "ci": ci_out, "ci_overall": overall(ci_out, "BIAS", "NO BIAS"),
            "growth": g_out, "growth_overall": overall(g_out, "GROWTH ADVANTAGE", "NO ADVANTAGE")}


# ---------------------------------------------------------------------------------------------- W5

B0 = 20.4 / (1.0 - math.exp(-0.042 * 30.0)) ** 1.563   # SitkaGrowthModel.cs ClassIIIb0 (cancels in the ratio)


def envelope(age):
    a = max(1, age)
    return B0 * (1.0 - math.exp(-0.042 * a)) ** 1.563


def w5(sdir):
    rost = rosters_by_year(sdir)
    if 0 not in rost:
        return {"evaluated": 0, "defects": [], "clamp_excluded": []}
    lo, hi = struct.pack(">f", f32(0.1)).hex(), struct.pack(">f", 60.0).hex()
    clamp = set()
    for y in rost:
        for aid, trees in rost[y].items():
            for tid, t in trees.items():
                if R.b32(t["heightM"]) in (lo, hi):
                    clamp.add(tid)
    rho0 = {}
    for aid, trees in rost[0].items():
        for tid, t in trees.items():
            rho0[tid] = fb(R.b32(t["heightM"])) / envelope(R.ival(t["ageYears"]))
    evaluated, defects = 0, []
    for y in sorted(rost):
        if y == 0:
            continue
        tol = 8 * y * 2.0 ** -24
        for aid, trees in rost[y].items():
            for tid, t in trees.items():
                if not t["isLiving"] or tid in clamp:
                    continue
                evaluated += 1
                drift = abs((fb(R.b32(t["heightM"])) / envelope(R.ival(t["ageYears"]))) / rho0[tid] - 1.0)
                if drift > tol:
                    defects.append("%s y%d %s drift %.3e > %.3e" % (aid, y, tid, drift, tol))
    return {"evaluated": evaluated, "defects": defects, "clamp_excluded": sorted(clamp, key=R.ordkey)}


# ---------------------------------------------------------------------------------------------- integrity, W11

def integrity_and_w11(sdir, per_tree, traj, fells):
    problems = []
    last = {}
    for row in traj:
        a = row["areaId"]
        if a in last and row["livingCount"] > last[a]:
            problems.append("living count increased in %s at year %s" % (a, row["year"]))
        last[a] = row["livingCount"]
    ids = [f["treeId"] for f in fells]
    if len(ids) != len(set(ids)):
        problems.append("a Fell ID appears more than once in the ledger")
    expected_rows = markers = 0
    for _, _, rep in reports(sdir):
        if rep is None:
            markers += 1
        else:
            expected_rows += len(rep["trees"])
    got_rows = sum(1 for r in per_tree if r["row_kind"] in ("evaluated", "dead-carried"))
    got_markers = sum(1 for r in per_tree if r["row_kind"] == R.CARRY_MARKER)
    w11 = {"expected_rows": expected_rows, "logged_rows": got_rows, "expected_markers": markers,
           "logged_markers": got_markers, "match": expected_rows == got_rows and markers == got_markers}
    if not w11["match"]:
        problems.append("W11 row-count mismatch %r" % w11)
    return problems, w11


# ---------------------------------------------------------------------------------------------- interventions

INTENSITY_COLUMNS = ["scenario", "repeat", "ecologicalYear", "areaId", "stems_before", "stems_after", "removed_stems",
                     "removed_stem_fraction", "ba_before_f64", "ba_after_f64", "removed_ba_m2_per_ha",
                     "removed_ba_fraction", "dq_before", "dq_after", "dq_shift", "mean_dbh_before_harness",
                     "mean_dbh_after_harness", "mean_dbh_shift"]


def intensities(sdir, scenario, repeat):
    lines = R.read_transcript(sdir)
    rows = []
    for k, line in enumerate(lines):
        if line["route"] != "api/multi/plan/execute" or line["status"] != 200:
            continue
        payload = R.load_file(os.path.join(sdir, line["response_file"]))
        if payload["execution"]["replayed"] is True:
            continue
        pre = lines[k - 1]
        if pre["route"] != R.VIEW_ROUTE:
            raise ValueError("Execute not preceded by its view read in " + sdir)
        pre_view = R.load_file(os.path.join(sdir, pre["response_file"]))["view"]
        post_view = payload["view"]
        ev = payload["execution"]["event"]
        for i, ch in enumerate(ev["areas"]):
            b, a = ch["before"], ch["after"]
            nb, na = R.ival(b["livingCount"]), R.ival(a["livingCount"])
            bab, baa = R.f64_value(R.b64(b["basalAreaM2PerHa"])), R.f64_value(R.b64(a["basalAreaM2PerHa"]))
            dqb, dqa = R.f64_value(R.b64(b["quadraticMeanDbhCm"])), R.f64_value(R.b64(a["quadraticMeanDbhCm"]))
            mb, ma = R.mean_dbh(pre_view["areas"][i]), R.mean_dbh(post_view["areas"][i])
            rows.append({"scenario": scenario, "repeat": repeat, "ecologicalYear": R.txt(ev["ecologicalYear"]),
                         "areaId": ch["areaId"], "stems_before": nb, "stems_after": na, "removed_stems": nb - na,
                         "removed_stem_fraction": (nb - na) / nb if nb else "null",
                         "ba_before_f64": R.b64(b["basalAreaM2PerHa"]), "ba_after_f64": R.b64(a["basalAreaM2PerHa"]),
                         "removed_ba_m2_per_ha": repr(bab - baa), "removed_ba_fraction": repr((bab - baa) / bab) if bab else "null",
                         "dq_before": repr(dqb) if nb else "null", "dq_after": repr(dqa) if na else "null",
                         "dq_shift": repr(dqa - dqb) if (nb and na) else "null",
                         "mean_dbh_before_harness": repr(mb) if mb is not None else "null",
                         "mean_dbh_after_harness": repr(ma) if ma is not None else "null",
                         "mean_dbh_shift": repr(ma - mb) if (ma is not None and mb is not None) else "null"})
    return rows


def matched_retained(run_dir, group, windows=CHECKPOINTS):
    """Q0 section 6 matched retained set over [0, t] across every session in `group` (r1)."""
    out = []
    dirs = {s: session_dir(run_dir, s) for s in group}
    if any(d is None for d in dirs.values()):
        return [{"group": group, "note": "a session is missing"}]
    rost = {s: rosters_by_year(d) for s, d in dirs.items()}
    felled = set()
    for s, d in dirs.items():
        felled |= {f["treeId"] for f in R.fell_rows(d, s, "r1")}
    for t in windows:
        if any(t not in rost[s] or 0 not in rost[s] for s in group):
            out.append({"group": group, "window": [0, t], "note": "not available (a session did not commit year %d)" % t})
            continue
        for aid in rost[group[0]][0]:
            genesis = rost[group[0]][0][aid]
            ids = [tid for tid in sorted(genesis, key=R.ordkey) if tid not in felled
                   and all(rost[s][0][aid][tid]["isLiving"] and rost[s][t][aid][tid]["isLiving"] for s in group)]
            entry = {"group": group, "window": [0, t], "area": aid, "matched_set_size": len(ids),
                     "share_of_genesis": len(ids) / len(genesis) if genesis else None}
            ctrl = group[0]
            for s in group[1:]:
                diffs = []
                for tid in ids:
                    dd = lambda sc: fb(R.b32(rost[sc][t][aid][tid]["dbhCm"])) - fb(R.b32(rost[sc][0][aid][tid]["dbhCm"]))
                    diffs.append(dd(s) - dd(ctrl))
                entry["mean_dD_%s_minus_%s" % (s, ctrl)] = (sum(diffs) / len(diffs)) if diffs else None
            out.append(entry)
    return out


# ---------------------------------------------------------------------------------------------- isolation

def isolation(sdir_i, sdir_c):
    diffs = []
    vi, vc = R.year_views(sdir_i), R.year_views(sdir_c)
    years = sorted(set(vi) & set(vc))
    if set(vi) != set(vc):
        diffs.append("committed years differ: I %s vs C %s" % (sorted(vi), sorted(vc)))

    def b_area(view):
        return next(a for a in view["areas"] if a["areaId"] == B_AREA)
    for y in years:
        a, c = R.load_file(vi[y])["view"], R.load_file(vc[y])["view"]
        if R.emit(b_area(a)) != R.emit(b_area(c)):
            diffs.append("View.Areas[MA-B] differs at year %d" % y)
    c0 = R.emit(b_area(R.load_file(vc[0])["view"])) if 0 in vc else None
    for line in R.read_transcript(sdir_i):
        if line["route"] == R.VIEW_ROUTE and line.get("year") == 0 and c0 is not None:
            if R.emit(b_area(R.load_file(os.path.join(sdir_i, line["response_file"]))["view"])) != c0:
                diffs.append("View.Areas[MA-B] at year 0 (seq %d, during the MA-A intervention) differs from C year 0" % line["seq"])
    ri = {(yb, aid): rep for yb, aid, rep in reports(sdir_i) if aid == B_AREA}
    rc = {(yb, aid): rep for yb, aid, rep in reports(sdir_c) if aid == B_AREA}
    for key in sorted(set(ri) | set(rc)):
        if R.emit(ri.get(key)) != R.emit(rc.get(key)):
            diffs.append("Annual.Areas[MA-B].Report differs for year %d -> %d" % (key[0], key[0] + 1))
    if years:
        hi, hc = R.load_file(vi[years[-1]])["view"]["annualHistory"], R.load_file(vc[years[-1]])["view"]["annualHistory"]
        for k, (x, z) in enumerate(zip(hi, hc)):
            bx = next(e for e in x["areas"] if e["areaId"] == B_AREA)
            bz = next(e for e in z["areas"] if e["areaId"] == B_AREA)
            if R.emit(bx) != R.emit(bz):
                diffs.append("AnnualHistory[%d].Areas[MA-B] differs" % k)
    return {"equal": not diffs, "compared_years": years, "differences": diffs[:200], "excluded_fields": ISOLATION_EXCLUDED,
            "comparison": "canonical JSON re-emission with exact number text (number text equal => bit patterns equal)"}


# ---------------------------------------------------------------------------------------------- subcommands

def cmd_isolation(args):
    res = isolation(args.i, args.c)
    print(json.dumps(res, indent=1))
    return 0 if res["equal"] else 1


def cmd_s1_size(args):
    sdir = args.session
    try:
        meta = json.load(open(os.path.join(sdir, "session.json"), "r", encoding="utf-8"))
    except OSError:
        print("S session missing")
        return 4
    for tag in ("y00", "y10"):
        sel_path = os.path.join(sdir, "selection-%s.json" % tag)
        if not os.path.exists(sel_path):
            print("S never reached its %s intervention (outcome %s)" % (tag, meta.get("outcome")))
            return 4
        sel = json.load(open(sel_path, "r", encoding="utf-8"))
        executed = any(l["route"] == "api/multi/plan/execute" and l["status"] == 200 and l.get("year") == int(tag[1:])
                       for l in R.read_transcript(sdir))
        if sel["orders"] and not executed:
            print("S %s intervention selected orders but no Execute committed" % tag)
            return 4
    counts = {"MA-A": 0, "MA-B": 0}
    for f in R.fell_rows(sdir, "S", "r1"):
        if int(f["ecologicalYear"]) in (0, 10):
            counts[f["areaId"]] += 1
    print("MA-A=%d\nMA-B=%d" % (counts["MA-A"], counts["MA-B"]))
    return 0


def cmd_s1_check(args):
    a = json.load(open(args.s, "r", encoding="utf-8"))
    b = json.load(open(args.s1, "r", encoding="utf-8"))
    keys = ("crops", "crop_acceptance_order", "competitor_lists", "sequence", "deduplicated", "dropped_duplicates")
    diffs = []
    for x, y in zip(a["areas"], b["areas"]):
        for k in keys:
            if x.get(k) != y.get(k):
                diffs.append("%s.%s" % (x["areaId"], k))
    print(json.dumps({"same_crop_set_and_sequence": not diffs, "differences": diffs}, indent=1))
    return 0 if not diffs else 1


def cmd_transcript_tsv(args):
    for line in R.read_transcript(args.session):
        print("\t".join([str(line["seq"]), line["kind"], line["method"], line["route"], line["request_file"] or ""]))
    return 0


def fmt(x):
    if isinstance(x, float):
        return "%.6g" % x
    return str(x)


def cmd_all(args):
    run = args.run_dir
    adir = os.path.join(run, "analysis")
    os.makedirs(adir, exist_ok=True)
    anomalies, verdict_reasons = [], []
    all_tree, all_traj, all_fell, all_int = [], [], [], []
    session_summ = []
    diag = {"hw2": {}, "hw3": {}, "w5": {}, "w11": {}, "integrity": {}}
    for meta, sdir in sessions(run):
        sc, rp = meta["scenario"], meta["repeat"]
        label = "%s/%s" % (sc, rp)
        session_summ.append((label, meta))
        if meta["outcome"] not in ("COMPLETE", "LAWFUL-STOP"):
            if sc != "PROBES":
                verdict_reasons.append("%s outcome %s: %s" % (label, meta["outcome"], meta.get("cause")))
        if meta.get("process_stop_clean") != "true":
            anomalies.append("%s: process stop not clean (exit 0 + 'Session ended; nothing was saved.' + empty stderr)" % label)
        if sc == "PROBES" or sc.startswith("REPLAY"):
            continue
        per_tree = R.per_tree_rows(sdir, sc, rp)
        traj = R.trajectory_rows(sdir, sc, rp)
        fells = R.fell_rows(sdir, sc, rp)
        all_tree += per_tree
        all_traj += traj
        all_fell += fells
        all_int += intensities(sdir, sc, rp)
        problems, w11 = integrity_and_w11(sdir, per_tree, traj, fells)
        diag["integrity"][label] = problems
        diag["w11"][label] = w11
        anomalies += ["%s: %s" % (label, p) for p in problems]
        if rp == "r1":
            diag["hw2"][sc] = hw2(sdir, anomalies, label)
            diag["hw3"][sc] = hw3(sdir)
            diag["w5"][sc] = w5(sdir)
    anomalies += ["record: %s %s %s" % (a["kind"], a["where"], a["detail"]) for a in R.ANOMALIES]
    R.write_csv(os.path.join(adir, "per_tree_year.csv"), R.PER_TREE_COLUMNS, all_tree)
    R.write_csv(os.path.join(adir, "trajectories.csv"), R.TRAJECTORY_COLUMNS, all_traj)
    R.write_csv(os.path.join(adir, "fells.csv"), R.FELL_COLUMNS, all_fell)
    R.write_csv(os.path.join(adir, "intensities.csv"), INTENSITY_COLUMNS, all_int)

    compares = {}
    for sc in SCENARIOS:
        p = os.path.join(run, "sessions", sc, "projection-compare.json")
        if os.path.exists(p):
            c = json.load(open(p, "r", encoding="utf-8"))
            compares[sc] = "equal" if c["equal"] else ("prefix-only" if c.get("prefix_only") else "MISMATCH")
            if compares[sc] != "equal":
                verdict_reasons.append("repeat projection for %s: %s" % (sc, compares[sc]))
    replays = {}
    for p in sorted(glob.glob(os.path.join(run, "replay", "*", "compare.json"))):
        replays[os.path.basename(os.path.dirname(p))] = json.load(open(p, "r", encoding="utf-8"))["equal"]
    if args.isolation == "DEFECT":
        verdict_reasons.append("isolation I vs C: DEFECT")
    hw4_res = hw4(session_dir(run, "C")) if session_dir(run, "C") else None
    triple = matched_retained(run, ["C", "L", "M"]) if all(session_dir(run, s) for s in ("C", "L", "M")) else []
    pairs = {s: matched_retained(run, ["C", s], windows=[10, 50]) for s in ("D", "S", "S1", "I") if session_dir(run, s) and session_dir(run, "C")}

    def removed(sc, year):
        return {r["areaId"]: r for r in all_int if r["scenario"] == sc and r["repeat"] == "r1" and int(r["ecologicalYear"]) == year}
    gd = []
    lm, mm = removed("L", 0), removed("M", 0)
    for aid in sorted(set(lm) | set(mm)):
        if aid in lm and aid in mm:
            ok = float(mm[aid]["removed_ba_m2_per_ha"]) > float(lm[aid]["removed_ba_m2_per_ha"])
            gd.append("%s: M removed BA %s vs L %s -> %s" % (aid, mm[aid]["removed_ba_m2_per_ha"], lm[aid]["removed_ba_m2_per_ha"],
                                                         "genuinely different" if ok else "NOT genuinely different in area " + aid))
    s1 = []
    s0, s10, s1r = removed("S", 0), removed("S", 10), removed("S1", 0)
    if s1r:
        s_ids = {(f["areaId"], f["treeId"]) for f in all_fell if f["scenario"] == "S" and f["repeat"] == "r1"}
        s1_ids = {(f["areaId"], f["treeId"]) for f in all_fell if f["scenario"] == "S1" and f["repeat"] == "r1"}
        for aid in ("MA-A", "MA-B"):
            ns = sum(int(d[aid]["removed_stems"]) for d in (s0, s10) if aid in d)
            bas = sum(float(d[aid]["removed_ba_m2_per_ha"]) for d in (s0, s10) if aid in d)
            n1 = int(s1r[aid]["removed_stems"]) if aid in s1r else 0
            ba1 = float(s1r[aid]["removed_ba_m2_per_ha"]) if aid in s1r else 0.0
            s1.append({"area": aid, "S_removed_stems": ns, "S1_removed_stems": n1, "stems_equal": ns == n1,
                       "S_removed_ba_sum_at_removal": bas, "S1_removed_ba": ba1, "ba_abs_diff": ba1 - bas,
                       "ba_rel_diff": (ba1 - bas) / bas if bas else None,
                       "S_only_ids": len({t for a, t in s_ids if a == aid} - {t for a, t in s1_ids if a == aid}),
                       "S1_only_ids": len({t for a, t in s1_ids if a == aid} - {t for a, t in s_ids if a == aid}),
                       "overlap_ids": len({t for a, t in s_ids if a == aid} & {t for a, t in s1_ids if a == aid}),
                       "timing": "S: years 0 and 10; S1: year 0"})
    elif os.path.exists(os.path.join(run, "sessions", "S1", "not-run.json")):
        s1.append(json.load(open(os.path.join(run, "sessions", "S1", "not-run.json"))))
    for sc, res in diag["hw2"].items():
        if res["e3"]["mismatches"]:
            anomalies.append("%s: offline E3 CI recomputation differs from the engine in %d rows (first: %s)"
                             % (sc, len(res["e3"]["mismatches"]), res["e3"]["mismatches"][0]))
    for sc, res in diag["w5"].items():
        if res["defects"]:
            anomalies.append("%s: W5 height-ratio drift beyond 8*t*2^-24 in %d tree-years (defect in the E6 description)" % (sc, len(res["defects"])))
    for traj_row in all_traj:
        if traj_row["state"] in ("extinct-this-year",) or (traj_row["state"] == "carried-forward" and traj_row["year"] == traj_row["firstCarryForwardYear"]):
            anomalies.append("EVENT (preserved): %s/%s %s year %s %s" % (traj_row["scenario"], traj_row["repeat"], traj_row["areaId"], traj_row["year"], traj_row["state"]))

    md = ["# Q1 diagnostics (DIAGNOSTICS ONLY -- not validation, not a gate)", "",
          "Synthetic closed-canopy Sitka model. Not ecological validation, not an Irish field forecast, not MA-2 comparability, "
          "not owner-accepted. Produced by an AUTHORED harness; nothing here is REVIEWED or VERIFIED.", "",
          "## Sessions", "", "| session | outcome | last year | cause |", "|---|---|---|---|"]
    md += ["| %s | %s | %s | %s |" % (l, m["outcome"], m.get("last_committed_year"), m.get("cause", "")) for l, m in session_summ]
    md += ["", "## Repeat projection (Q1 section 6)", ""] + ["- %s: %s" % (k, v) for k, v in compares.items()]
    md += ["", "## Transcript replay (W11 b)", ""] + (["- %s: %s" % (k, "equal" if v else "MISMATCH") for k, v in replays.items()] or ["- not run"])
    md += ["", "## Isolation I vs C (MA-B)", "", "- %s; excluded fields: %s" % (args.isolation, ", ".join(ISOLATION_EXCLUDED))]
    md += ["", "## Checkpoints (r1; years 1/5/10/20/50 from the all-year log)", "",
           "| scenario | area | year | living | BA m2/ha | Dq cm | mean DBH (harness) | cum natural | cum Fell |", "|---|---|---|---|---|---|---|---|---|"]
    for r in all_traj:
        if r["repeat"] == "r1" and r["year"] in CHECKPOINTS:
            md.append("| %s | %s | %s | %s | %s | %s | %s | %s | %s |" % (r["scenario"], r["areaId"], r["year"], r["livingCount"],
                      r["basalAreaM2PerHa"], r["quadraticMeanDbhCm"], r["meanDbhCm_harness"], r["cumNaturalDeaths"], r["cumFell"]))
    md += ["", "Stand mean DBH is shown only alongside the retained-tree and selection-shift components below; it is never "
           "growth-response evidence on its own.", "", "## Realised intensity and selection-only shift", "",
           "See intensities.csv (removed stems/BA and fractions from the engine's per-area Before/After; mean DBH and Dq shift "
           "at each Execute, year unchanged, no growth).", "", "## M vs L genuinely different (strict, both areas)", ""] + ["- " + g for g in gd]
    md += ["", "## Matched retained sets (Q0 section 6)", "", "```", json.dumps(triple, indent=1), json.dumps(pairs, indent=1), "```",
           "", "Frenchpark: sign only of the mean paired increment difference above; these are synthetic stands, not yield plots, "
           "and are not comparable to Frenchpark's site, age or duration.", "", "## S vs S1", "", "```", json.dumps(s1, indent=1), "```"]
    md += ["", "## H-W2 (a) CI invariance to uniform DBH scaling; (b) DBH increment from D and CI; E3 integrity", ""]
    for sc, res in diag["hw2"].items():
        md.append("- %s: (a) %s; (b) evaluated %d, rejects %d; E3 offline equality evaluated %d, mismatches %d" % (
            sc, "; ".join("lambda %s evaluated %d rejects %d max|dev| %s" % (k, v["evaluated"], len(v["rejects"]), fmt(v["max_abs_dev"]))
                          for k, v in res["a"].items()),
            res["b"]["evaluated"], len(res["b"]["rejects"]), res["e3"]["evaluated"], len(res["e3"]["mismatches"])))
    md += ["", "## H-W3 hazard vs boundary (flag: boundary > 50 % of natural in a window with >= 10 natural deaths; "
           "a hazard-dominated result does not validate the [C] constants)", ""]
    for sc, rows in diag["hw3"].items():
        md += ["- %s %s %s: hazard %d boundary %d%s" % (sc, r["area"], r["window"], r["hazard"], r["boundary"],
                                                       " FLAG" if r["flag_boundary_dominated"] else "") for r in rows]
    md += ["", "## H-W4 (session C; descriptive, model-specific; ln D is a chosen adjustment; no causal claim, no p-values)", "",
           "```", json.dumps(hw4_res, indent=1, default=str), "```"]
    md += ["", "## W5 height envelope ratio drift", ""] + ["- %s: evaluated %d, defects %d, clamp-excluded %d" % (
        sc, r["evaluated"], len(r["defects"]), len(r["clamp_excluded"])) for sc, r in diag["w5"].items()]
    md += ["", "## W11 retention", ""] + ["- %s: %r" % (k, v) for k, v in diag["w11"].items()]
    w12p = os.path.join(run, "w12", "w12.json")
    md += ["", "## W12 README drift", "", "- " + (open(w12p).read().strip() if os.path.exists(w12p) else "not run")]
    probes = os.path.join(run, "sessions", "PROBES", "r1", "probes.jsonl")
    md += ["", "## Valid-case boundaries", ""] + (["- " + l.strip() for l in open(probes)] if os.path.exists(probes) else ["- probes not run"])
    with open(os.path.join(adir, "diagnostics.md"), "w", encoding="utf-8") as fh:
        fh.write("\n".join(md) + "\n")
    with open(os.path.join(adir, "diagnostics.json"), "w", encoding="utf-8") as fh:
        json.dump({"diag": diag, "hw4": hw4_res, "triple": triple, "pairs": pairs, "s1": s1, "genuinely_different": gd}, fh, indent=1, default=str)
    with open(os.path.join(adir, "anomalies.md"), "w", encoding="utf-8") as fh:
        fh.write("# Q1 anomaly register (nothing filtered)\n\n" + ("\n".join("- " + a for a in anomalies) or "- none recorded") + "\n")

    aborted = os.path.exists(os.path.join(run, "ABORTED.json"))
    verdict = "INCOMPLETE" if (aborted or verdict_reasons) else "PRODUCED"
    with open(os.path.join(run, "run-verdict.json"), "w", encoding="utf-8") as fh:
        json.dump({"verdict": verdict, "claim_ladder": "PRODUCED at most; not REVIEWED, not VERIFIED, not OWNER ACCEPTED",
                   "aborted": aborted, "reasons": verdict_reasons, "isolation": args.isolation, "repeat_projection": compares,
                   "replays": replays}, fh, indent=1)
    print(verdict)
    return 0 if verdict == "PRODUCED" else 1


def main(argv):
    ap = argparse.ArgumentParser(description="Q1 offline analysis (AUTHORED, NOT EXECUTED)")
    sub = ap.add_subparsers(dest="cmd", required=True)
    p = sub.add_parser("all"); p.add_argument("--run-dir", required=True)
    p.add_argument("--isolation", default="not-run", choices=("equal", "DEFECT", "not-run")); p.set_defaults(fn=cmd_all)
    p = sub.add_parser("isolation"); p.add_argument("--i", required=True); p.add_argument("--c", required=True); p.set_defaults(fn=cmd_isolation)
    p = sub.add_parser("s1-size"); p.add_argument("--session", required=True); p.set_defaults(fn=cmd_s1_size)
    p = sub.add_parser("s1-check"); p.add_argument("--s", required=True); p.add_argument("--s1", required=True); p.set_defaults(fn=cmd_s1_check)
    p = sub.add_parser("transcript-tsv"); p.add_argument("--session", required=True); p.set_defaults(fn=cmd_transcript_tsv)
    args = ap.parse_args(argv)
    return args.fn(args)


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
````

### `q1-replay.sh` — 84 lines, 4584 bytes

SHA-256 `6bf673154164eb21291363a699be7d8ef48425f9ffbc6a6a2a284b2427736d13`

````bash
#!/usr/bin/env bash
# q1-replay.sh -- Multi #885 Q1 transcript replay against a FRESH process + projection comparison (W11(b)).
#
# AUTHORED, NOT EXECUTED. Scratch harness component; not a reviewed repository feature.
#
# Re-sends every exchange of one recorded session, in order, to a new UI process (a new in-memory session). Each
# recorded POST body is sent verbatim except for ONE substitution: the recorded session id is replaced by the new
# session id in fence.sessionId and as the leading "<sessionId>:" of planId / executionKey (q1-record.py rebind).
# Nothing else is rebuilt: the recorded source token and planStateRevision are sent as recorded, so any divergence
# of the new session is refused by the engine or caught by the live fence-echo check, and then shows as a mismatch.
# The original and the replay are then compared under the Q1 section 6 projection (q1-record.py compare).
#
# Usage: q1-replay.sh --run-dir RUN --session sessions/<scenario>/r<k> [--name NAME] [--lab DIR] [--dotnet PATH]
#   The run must be finished (its lane lock released). Output: RUN/replay/<NAME>/ (transcript, raw, compare.json).
# Exit: 0 projection equal; 1 mismatch (REPLAY FAILED); 2 usage/preflight; 20 replay aborted (see REPLAY-FAILED.json).
set -euo pipefail

REPLAY_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd -P)"
# shellcheck source=q1-driver.sh
source "$REPLAY_DIR/q1-driver.sh"

replay_main() {
  local session="" name="" orig out old_sid new_sid="" seq kind method route reqfile body rc
  while [[ $# -gt 0 ]]; do
    case "$1" in
      --run-dir) RUN_DIR="$2"; shift 2 ;;
      --session) session="$2"; shift 2 ;;
      --name) name="$2"; shift 2 ;;
      --lab) LAB="$2"; shift 2 ;;
      --dotnet) DOTNET_BIN="$2"; shift 2 ;;
      *) sed -n '2,18p' "$REPLAY_DIR/q1-replay.sh" >&2; exit 2 ;;
    esac
  done
  [[ -n "$RUN_DIR" && -n "$session" ]] || { sed -n '2,18p' "$REPLAY_DIR/q1-replay.sh" >&2; exit 2; }
  RUN_DIR="$(cd -- "$RUN_DIR" && pwd -P)"
  orig="$RUN_DIR/$session"
  [[ -f "$orig/transcript.jsonl" ]] || { echo "STOP: no transcript at $orig" >&2; exit 2; }
  [[ -n "$name" ]] || name="$(printf '%s' "$session" | tr '/' '-')-$(date -u +%Y%m%dT%H%M%SZ)"
  out="$RUN_DIR/replay/$name"
  [[ ! -e "$out" ]] || { echo "STOP: $out exists (choose a new --name)" >&2; exit 2; }
  trap cleanup EXIT
  trap 'exit 130' INT TERM
  preflight_runtime
  acquire_lock
  H="$RUN_DIR/harness"
  ( cd -- "$H" && sha256sum -c --quiet SHA256SUMS ) || { echo "STOP: bundle harness bytes do not match SHA256SUMS" >&2; exit 2; }
  FIXTURE_DIR="$RUN_DIR/fixture"
  [[ -f "$FIXTURE_DIR/$FIXTURE_NAME" ]] || { echo "STOP: bundle fixture copy missing" >&2; exit 2; }
  mkdir -p -- "$out/raw" "$out/reports"
  SDIR="$out"
  DIE_FILE="$out/REPLAY-FAILED.json"
  SEQ=0; LAST_YEAR=0; SESSION_OUTCOME="COMPLETE"; SESSION_CAUSE=""
  old_sid="$(rec get "$orig/$(rec get "$orig/state.json" last_view_file)" view.sessionId)"
  [[ "$old_sid" =~ ^[0-9a-f]{32}$ ]] || { echo "STOP: recorded session id not found" >&2; exit 2; }
  # The transcript is JSON Lines; turn it into one TSV of the fields needed to re-send, without re-sorting.
  "${PY[@]}" "$H/q1-analyse.py" transcript-tsv --session "$orig" > "$out/replay-plan.tsv" \
    || die_run "cannot read the recorded transcript"
  start_server "replay-$name" "$out"
  SERVER_PID_RECORDED="$SERVER_PID"
  while IFS=$'\t' read -r -u 3 seq kind method route reqfile; do
    if [[ "$method" == "GET" ]]; then
      http "$kind" GET "$route"
    else
      [[ -n "$new_sid" ]] || die_run "replay reached a POST before any view gave the new session id"
      body="$out/raw/rebound-body.json"
      rec rebind --body "$orig/$reqfile" --old-sid "$old_sid" --new-sid "$new_sid" > "$body" \
        || die_run "recorded body $reqfile could not be rebound"
      http "$kind" POST "$route" "$body"
    fi
    if [[ -z "$new_sid" && "$route" == "api/multi/view" ]]; then new_sid="$(rec get "$LAST_RESP" view.sessionId)"; fi
    if [[ "$route" == "api/multi/year/advance" && "$LAST_STATUS" == 200 ]]; then LAST_YEAR=$((LAST_YEAR + 1)); fi
  done 3< "$out/replay-plan.tsv"
  stop_server
  SESSION_OUTCOME="$(rec get "$orig/session.json" outcome 2> /dev/null || printf 'UNKNOWN')"
  SESSION_CAUSE="replay of $session"
  write_session_json "REPLAY:$session" "replay"
  rc=0
  rec compare "$orig" "$out" --out "$out/compare.json" > /dev/null || rc=$?
  if [[ "$rc" == 0 ]]; then log "REPLAY EQUAL under the projection: $session -> replay/$name"; exit 0; fi
  log "REPLAY MISMATCH: $session -> replay/$name (see compare.json)"
  exit 1
}

replay_main "$@"
````

### `README.md` — 301 lines, 23984 bytes

SHA-256 `5a6747e70e8f3ece5e61b9b9d0ceba1beb4eea9041f0b854d7beca9a7cab574e`

````text
# Multi #885 Q1 multi-year simulation harness — AUTHORED, NOT EXECUTED

**Status: AUTHORED, NOT EXECUTED.** These bytes have not been run, reviewed, approved or verified. No engine was
started, no session was created, no listener was opened and no test was executed while writing them. The only checks
made were `bash -n`, `python3 -m py_compile`, `sha256sum -c` and static inspection against the source at
`4cecf87339bea4d32907955a8d5625d1f27f96bd`. The Q1 packet is **BLOCKED — NOT AUTHORISED TO RUN** until its section 10
conditions clear and a distinct run worker is assigned. This is a scratch harness, **not** a reviewed repository
feature, and must never be described as one.

Author: Claude Opus 5.5 (`claude-opus-5-5`), harness byte author only (not custodian, Manager, run worker or
reviewer). Specification: the Q1 execution packet (`docs/multi-885-q1-execution-packet-20261121-v1`) and Q0
revision 3 (blob `c927c12e…`, SHA-256 `021c6ff8…d58c`, verified on fetch), which the packet binds as its protocol basis.

## Files

| File | Role |
|---|---|
| `q1-driver.sh` | process lifecycle (own PID only, SIGINT, clean-exit assertion), scenario wiring C/L/M/D/S/S1/I, budgets, optional probes and W12, ordered transcript via `q1-record.py exchange` |
| `q1-select.py` | the frozen Q0 rev3 rule (a) selection: a pure function of one intervention-time view on stdin |
| `q1-record.py` | exact-number JSON parsing (null/0/-0 preserved), float32/float64 bit recovery, the per-exchange live invariant checker, POST body builder (fence echo), disclosure, projection comparison, replay rebinding, normalised CSV records |
| `q1-analyse.py` | offline integrity re-checks, trajectories, realised intensities, H-W2/H-W3/H-W4/W5/W11/W12, matched retained sets, S vs S1, isolation, S1 sizing, run verdict |
| `q1-replay.sh` | re-sends one recorded transcript to a FRESH process and compares under the same projection |
| `SHA256SUMS` | `sha256sum` over the six files above |

Dependencies: bash ≥ 5 (for `EPOCHREALTIME`), curl, python3 standard library, sha256sum, coreutils (`timeout`
for W12 only). Node is not used. No package is installed; no non-loopback address is contacted.

## Usage (for the future, authorised run worker)

```bash
H=/path/to/harness        # this directory, unmodified (the driver copies it into the bundle and re-checks SHA256SUMS)
RUN=/home/jer/Development/CCF-885-Qualification-Scratch-20261121/<run-id>   # must be new/empty, outside CCF-HP and its worktrees

# Full Q1 run: 7 scenarios x 2 fresh sessions, in canonical order C L M D S S1 I; optional probes and W12.
"$H/q1-driver.sh" run --run-dir "$RUN" [--probes] [--w12] \
    [--lab /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121/Tools/MultiRackSitkaP1Lab] \
    [--dotnet /home/jer/.dotnet/dotnet]

# Subset (S1 requires S in the same run; isolation requires C and I):
"$H/q1-driver.sh" run --run-dir "$RUN" --scenarios "C I"

# Valid-case probes only, or W12 only (each into its own new run directory):
"$H/q1-driver.sh" probes --run-dir "$RUN-probes"
"$H/q1-driver.sh" w12 --run-dir "$RUN-w12"

# Reproducibility evidence (after the run has finished and released its lane lock):
"$RUN/harness/q1-replay.sh" --run-dir "$RUN" --session sessions/C/r1 [--name C-r1-replay1]

# Re-run the offline analysis on a finished bundle (read-only on sessions/):
python3 -I -B "$RUN/harness/q1-analyse.py" all --run-dir "$RUN" --isolation equal|DEFECT|not-run
```

Exit codes. Driver: `0` finished (see `run-verdict.json`), `2` usage or preflight refusal, `20` run ABORTED
(INCOMPLETE; `ABORTED.json`). Replay: `0` projection equal, `1` mismatch, `20` replay aborted (`REPLAY-FAILED.json`).
`q1-record.py`: `20` abort run, `21` stop session, `1` mismatch. `q1-select.py`: `3` DEFECT, `4` S1 not constructible.

## What one session does

1. Preflight (once per run). Bash ≥ 5. `dotnet --version` must be exactly `8.0.134` at `/home/jer/.dotnet/dotnet`;
   the system dotnet is never used. The UI dll must already exist (the harness never builds). The lab HEAD must
   equal the pinned SHA (`git rev-parse`/`git status`, read-only) with no modified tracked files. The run directory
   must be empty and outside the main checkout and the worktrees. An atomic `mkdir` lane lock is taken. The six
   files and `SHA256SUMS` are copied to `<run>/harness/` and verified, and that verified copy is what runs. The
   fixture is copied into `<run>/fixture/` and passed as a directory. `header.json` is written.
2. A fresh process starts with `set -m` (own process group, default SIGINT), cwd = the session directory. The
   harness reads PID, port and capability from the exact line
   `Two-area MA-1 simulator: http://127.0.0.1:<port>/s/<cap>/multi`. It refuses a protected PID or port, a port
   below 1024, or a process whose `/proc/<pid>/cmdline` does not name our dll and fixture copy. Every port is
   recorded in `logs/ports.tsv`.
3. `GET api/multi/manifest`. The SHA-256 of the body without `contentHash` must equal `contentHash`, which must
   equal `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`. Seeds MA-A 885101 / MA-B 885202 and
   generator 8850001 must match. Any mismatch aborts the run.
4. For each year y = 0…49: if the scenario schedules an intervention at y (L/M/S/S1/I at 0, D at 5, S at 10):
   read view → `q1-select.py` → for each Crop: read view, `crop/mark` → for each order: read view, `pending/add`.
   An empty order set means no Approve and a `no-intervention-yNN.json` record. Otherwise: read view,
   `plan/approve` → read view and write `disclosure-yNN.json` (Approved.Orders must equal the frozen selection) →
   read view, `plan/execute`. Then read view, `year/advance` with `consentToCancelWork:false`, keep
   `reports/year-NN.json`, read view, keep `view-year-NN.json`.
5. Stops (recorded, never worked around). `ContinuationNoLivingStems` ⇒ `LAWFUL-STOP`. A year over 60 s or a
   scenario over 30 min ⇒ `BUDGET-STOP`, recording the last committed year and the cause. An unexpected 409 ⇒
   `INCOMPLETE` for that session. The horizon is never extended and no repeat is ever dropped.
6. SIGINT to our own PID only. A clean stop requires exit 0, last stdout line `Session ended; nothing was saved.`
   and empty stderr. After 30 s the harness sends SIGKILL to that same PID (recorded as not clean). It never uses
   name-based process killing or name-based process lookup. PIDs 1750123 and 1806361 are never signalled.
7. After both repeats, `q1-record.py compare` runs, and any projection mismatch aborts the run. After S, S1 is
   sized from S r1's realised Fells at years 0 and 10. After S1 r1, its year-0 Crop set and sequence must equal
   S's. After I, the isolation check runs against C r1. Finally `q1-analyse.py all` runs.

### Live checks per exchange (`q1-record.py exchange`; abort ⇒ run INCOMPLETE)

- curl failure or timeout (outcome unconfirmed) ⇒ abort. HTTP 500 ⇒ abort. Any other status except 200/409 ⇒ abort
  (a guard refusal means the harness sent a malformed request).
- `sessionId` must be 32 lowercase hex, constant, and equal to every fence sent. Every POST fence must be
  byte-equal (exact number text) to the fence of the view read immediately before. The only exemption is the
  `replay-probe` kind (see C5).
- **Second writer.** Every GET view, and every 409 view, must be identical to the previous committed view. Every
  death must be explained by a recorded Fell or natural death. No death is ever undone.
- Living counts per area are non-increasing (an increase aborts). Roster living count must equal
  `stand.livingCount`. Crop marks must be on living trees.
- Approve: year and all revisions unchanged except plan-state +1; living counts and ledger unchanged; pending
  empty; `PlanId == sessionId:ma-plan:<post-Approve planStateRevision>`; `key == PlanId + ":execute"`; orders equal
  the previous pending list.
- Execute: year and annual revision kept, work revision +1, plan-state +1, one new event. Event Before/After equal
  the pre/post committed stands. Fell records are ordinal by TreeId and are exactly the approved set. Each record
  has cause `fell`, mechanism `manual-work-plan`, year y, the new work revision, and the event's PlanId/key.
  Each felled tree carries `Death(y, fell)` and was alive before (no double application).
- Advance: year and annual revision +1, work revision kept, history +1 with earlier entries unchanged, and
  `cancelledUnexecutedWork` equal to whether work existed. For each area:
  - Null report ⇔ `noLivingStems` ⇔ the area was empty, and the roster is carried unchanged.
  - Otherwise, report Before equals the previous committed stand (the event After when an Execute preceded).
    Report After equals the history After and the view stand. Report deaths equal history natural deaths, each
    carrying y+1, `self-thinning`, `hazard|boundary`.
  - Audit rows are the ordinal roster and equal the next roster. Dead carried rows have CI bits `00000000`,
    neighbours 0 and null S/p/u. No felled tree is evaluated (so none enters a neighbour set).
  - S is null iff p is null; u is non-null iff p > 0.
  - After living = Before living − deaths, and Fell + natural = genesis − living.

## Output bundle (packet section 9, plus the additions marked +)

```
<run-id>/
  harness/   the six files + SHA256SUMS (verified copy; this copy runs)
  header.json                     source SHA, manifest hash, harness hashes, dotnet/node/kernel/host, worktree, dll and
                                  fixture SHA-256, session start order (pid, port, time)
  fixture/                       + the scratch copy passed as the fixture directory
  sessions/<sc>/r<k>/transcript.jsonl      one line per exchange, in issue order (request text verbatim, status,
                                           response file + SHA-256, timing, live-check result)
  sessions/<sc>/r<k>/reports/year-NN.json  the committed Year+1 command payload (Annual + View)
  sessions/<sc>/r<k>/view-year-NN.json     the committed view at year NN (NN=00 is genesis)
  sessions/<sc>/r<k>/disclosure-yNN.json   pre-Execute disclosure
  sessions/<sc>/r<k>/raw/                + every request and response body, numbered by exchange
  sessions/<sc>/r<k>/{selection,intervention-*-view,no-intervention}-yNN.json, state.json, session.json  (+)
  sessions/<sc>/projection-compare.json  + r1 vs r2 projection report
  sessions/PROBES/r1/probes.jsonl        + optional valid-case probes
  analysis/  trajectories.csv per_tree_year.csv fells.csv diagnostics.md anomalies.md
             intensities.csv diagnostics.json isolation.json   (+)
  logs/      server stdout/stderr per session, ports.tsv (every bound port), driver.log
  w12/       + optional W12 outputs
  replay/<name>/                         + q1-replay.sh outputs (transcript, raw, compare.json)
  run-verdict.json / ABORTED.json        + PRODUCED or INCOMPLETE, with reasons
```

CSV conventions: float32 columns `*_f32` hold 8-hex bit patterns; double columns `*_f64` hold 16-hex patterns; the
plain columns hold the exact JSON number text. `null` = not evaluated by the engine (never 0); `not-evaluated` =
the placeholder CI 0 / neighbours 0 / null S,p,u of a dead carried audit row; a carried-forward area-year has one
row with `row_kind=carried-forward-no-per-tree-audit` and no per-tree rows; `NAMED:<literal>` = a non-finite value
written as a string (also listed in `anomalies.md`).

## Projection (repeat gate and replay gate)

For every exchange in order, the following must be equal: kind, method, route and status. The request must also
be equal after the fence `sessionId` becomes `SESSION` and the leading `sessionId:` of planId/executionKey becomes
`SESSION:`. The response comparison then covers:

- **Byte-equal (canonical re-emission, exact number text):** `annual`, `view.areas`, `view.source`,
  `view.experiment`, `view.planStateRevision`, `view.annualHistory` and `refusal.code`.
- **Equal after the same leading-id normalisation:** `view.pending`, `view.approved`, `view.events` (with the
  Fell records) and `execution`.
- **Checked within each session and reported as defects, not normalised:** the session-id format and constancy,
  the PlanId and key formats, and Fell-record/event agreement.
- **Not compared:** `scenes` and `labels` (derived presentation) and the raw `sessionId`.
- **Manifest:** byte-equal by SHA-256.

Nothing is re-sorted. On a mismatch, the report names the first differing path and classifies a difference in
number text alone, with equal f32 and f64 bit patterns, as `serialiser-only`.

## Assumptions verified against source (`4cecf873`; paths relative to `Tools/MultiRackSitkaP1Lab/` unless noted)

1. Routes, all verified as literals: `src/Ui/PlayerMultiApi.cs:61-80`. The view `api/multi/view` is read at
   `:83-87`. Command payloads are `{view, scenes, refusal, execution, annual}` (`:114-115,201-202`); the view
   payload is `{view, scenes, labels}` (`:198-199`).
2. Required POST members are the full fence plus the route members (`PlayerMultiApi.cs:28-40`). The request must
   have no null and no missing member (`:157-167`), no duplicate member (`PlayerApi.cs:323-336`) and no unknown
   member, because `UnmappedMemberHandling.Disallow` applies (`PlayerApi.cs:46-50`). IDs must be 1–64 characters
   without whitespace, and a reason at most 2000 characters (`PlayerMultiApi.cs:170-184`). Reasons generated by
   `q1-select.py` are shorter than 200 characters and contain no tab or newline.
3. The fence is `{sessionId, source, planStateRevision}`, with `source` exactly the 12-member
   `MultiAreaSourceToken` (`src/Core/MultiArea/MultiAreaContracts.cs:12-27`). The harness echoes `view.source`
   verbatim, as the smoke test does (`tests/ui-smoke-multiarea.sh:165`).
4. Response JSON is camelCase, writes named float literals as strings and writes enums as strings
   (`PlayerApi.cs:53-58`). Hence the work action serialises as `"Fell"` (`src/Core/WorkContract.cs:12-16`), and
   NaN/Infinity become strings. Under the camelCase rule, `XM`/`ZM` serialise as `xm`/`zm`, which the page reads
   as `tree.xm` and `p.xm` (`PlayerMultiApi.cs:1149,1328,1603`). `IsLiving` is a serialised computed property
   (`src/Core/AnnualEngine.cs:12`), used as `t['isLiving']` by the smoke test (`:167,191`). A static cross-check
   (`check/member-names.py`, outside the bundle) maps all 78 source member names the harness reads to a C#
   record or manifest-writer key.
5. Status codes are 200/409 (`PlayerApi.cs:263-266`), 500 OutcomeUnconfirmed (`:180-186`), 403 for Host/Origin
   (`:124-129,161-167`) and 404 for the capability (`:131-137`). Guard errors are `{"error":{code,message}}`
   (`:431`).
6. The process binds `Listen(IPAddress.Loopback, 0)` (`src/Ui/Program.cs:54`) and prints the two-area line
   (`:75`). It ends with `Session ended; nothing was saved.` and exit 0 (`:80-81`). Admitted Host is
   `127.0.0.1:<port>` and admitted Origin is `http://127.0.0.1:<port>` (`PlayerApi.cs:107-108`). `set -m` before
   start follows `tests/ui-smoke-multiarea.sh:72-76`.
7. `sessionId` is 16 random bytes as lowercase hex, i.e. 32 characters (`MultiAreaContinuation.cs:56-57`).
   `PlanId = sessionId:ma-plan:<PlanStateRevision+1>`, the revision Approve publishes (`:190-194`; smoke `:263`).
   `key = PlanId + ":execute"` (`:510`).
8. Approve, Cancel, Execute and Advance transitions follow `MultiAreaContinuation.cs:181-405`:
   - Execute: `Death(y, "fell", "manual-work-plan")` (`:293`; `WorkContract.cs:50-54`), Fell records ordinal
     (`:305`), and work revision +1 and plan-state +1 (`:319-327`).
   - Advance refusal order: fence, then consent, then all-empty (`:345-351`).
   - Carry-forward gives a null report with `NoLivingStems:true` and Before = After = the empty Summarize
     (`:372-379`).
   - Crop marks on dead trees are cleared (`:380-384`).
   - Plan-state +1 on Advance only with work or a lost Crop (`:401`).
9. Mark/Add refusals are `CropAlreadyMarked` (`:100-101`), `CropAlreadyPlannedForFell` (`:96-98`) and
   `CropTreeRetained` (`:152-153`). The replay rule requires the same PlanId and the original `fence.Source`
   (`:229-235`), and the empty-area guard is `WorkWouldLeaveNoLivingStems` (`:268-275`).
10. E3 distance and the Hegyi term match `src/Core/AnnualRules.cs:67-82`:
    - float32 `dx`, `dz` and `dx*dx+dz*dz`, then `(float)Math.Sqrt`;
    - neighbours are skipped when `distance > 8f`;
    - the term is `(D_j / MathF.Max(1f, D_i)) / MathF.Max(0.5f, d)`, with float32 accumulation over living trees
      in ordinal order.

    The same arithmetic appears in `src/Core/PlayerCompetitionDiagnostics.cs:41-48`.
11. Growth constants `1.2f`, `120f` and `Ci50 = 5f` are at `AnnualRules.cs:6-11`, and the coded order at `:86-91`.
    Site = `1.0f` comes from the manifest member `models.siteProductivityInEngine` (`MultiAreaExperiment.cs:157`),
    written as `{"k":"f32","b":"<8 hex>"}` (`src/Core/StrictJson.cs:165`).
12. A hazard death carries year y+1 (`AnnualRules.cs:15,49`), as does a boundary death (`:138`). u is drawn only
    when p > 0 (`:42-44`). Dead carried audit rows have neighbours 0, CI 0f and null S/p/u (`:61-64`). Audit
    rows are ordinal by Before TreeId (`:64`).
13. The height envelope is `ClassIIIb0` with b2 0.042 and b3 1.563, and `NextHeight` uses `Math.Max(1, age)`
    (`Assets/ForestPrototype/SitkaGrowthModel.cs:37-60`, repository root).
14. Seeds are at `src/Core/MultiArea/MultiAreaGenerator.cs:28,41,46`. Manifest members `areas[].simulationSeed`,
    `generator.seed`, `polygonMm[].xMm/yMm`, `trees[].xMm/yMm/ageYears` and `contentHash` (last) come from
    `MultiAreaExperiment.cs:137-199`. The body-hash rule is the smoke test's (`tests/ui-smoke-multiarea.sh:135-143`).
15. The UI dll path is `src/Ui/bin/Debug/net8.0/sitkap1-ui.dll` (smoke `:28`). The CLI `annual` forms (3 args ⇒
    usage exit 64 after admission; 4 args ⇒ open and advance) are at `src/Cli/Program.cs:56-81`, and the README
    line is `README.md:153`.

## Packet / source contradictions and resolutions (recorded, not silently chosen)

- **C1 — Origin value.** The task brief says "Host and Origin must be exactly `127.0.0.1:<port>`". The source
  admits Origin `http://127.0.0.1:<port>` (`PlayerApi.cs:108,161-167`); a bare `127.0.0.1:<port>` Origin would
  be refused with 403. The harness sends Host `127.0.0.1:<port>` and Origin `http://127.0.0.1:<port>`.
- **C2 — S year-10 Crop set. Needs Manager confirmation.** The brief says "L's rule again at year 10 on the
  year-10 state; the year-10 Crop set may differ". Q0 rev3 §6 is narrower. Step 5(a) says the surviving year-0
  marks stay in place and are not re-issued. Step 10 says S keeps the surviving year-0 Crops, freezes their lists
  from the year-10 state, takes layer 1, and adds no new Crop. The source makes a recomputed Crop set
  unexecutable without an unmark rule that no document authorises: re-marking is refused as `CropAlreadyMarked`,
  and ordering a still-marked old Crop is refused as `CropTreeRetained`. The harness implements Q0 rev3 step 10
  (`q1-select.py --crops-from-view`), because the packet binds selection to Q0 rev3. The year-10 Crop set
  therefore differs from year 0 only by Crops that died, cleared by the engine at `:380-384`.
- **C3 — "PlanStateRevision-at-approve".** This is read as the revision Approve publishes (fence revision + 1),
  per Q0 §2.5(2), the source (`:190-191`) and smoke test `:263`.
- **C4 — Serialiser of the projection.** Q0 §2.5(4) names `SerializeToUtf8Bytes` with default options (author
  tests). Over HTTP only the UI's `ResponseJson` bytes exist. The projection is therefore a canonical
  re-emission of the HTTP JSON (member order and exact number text kept), not the default-options bytes. Both
  repeats pass through the same serialiser, so equality is meaningful. The bit patterns are recorded separately.
- **C5 — "Never cache or reconstruct a fence" versus replay.** The engine answers a replay only for the original
  `fence.Source` (`:229-235`), which no longer matches the current view. Probe P4 therefore re-sends the exact
  recorded request (`kind=replay-probe`; exempt only from the fence-echo check). `q1-replay.sh` likewise re-sends
  recorded bodies with only the session id substituted. That is the definition of a replay, and the live check
  then requires each re-sent fence to equal the replay session's freshly read view (deterministic or abort).
- **C6 — Q0 Repeats (ii)**, "compare against `MultiAreaAnnual.Step` per area in reverse order", needs internal
  types (InternalsVisibleTo), which are out of scope (packet §1). It is not implemented, and the packet §6 gate
  does not include it.
- **C7 — Empty-stand means.** For an empty area the engine reports the all-zero `AnnualStand`
  (`WorkContract.cs:132`). The packet requires null for means over an empty set. Dq is therefore written as
  `null` in the human column, with the engine's placeholder bits kept in `quadraticMeanDbhCm_f64`. The harness
  mean DBH is `null`. stems/ha, BA and RD keep the engine values (zero totals).
- **C8 — Choices the packet leaves open.**
  - An unexpected 409 in the scripted flow stops only that session (INCOMPLETE). A guard status (400/403/404/
    405/413/415) aborts the run as a harness defect.
  - The isolation check is not in the packet's abort list, so a difference is recorded as DEFECT and the run
    verdict becomes INCOMPLETE.
  - The year budget is measured from the start of the year's commands (including its intervention) to the
    committed Year+1 and its view read. The curl ceiling for Year+1 is 120 s, so a timeout means an unconfirmed
    outcome (abort).
  - A not-constructible S1 is stopped before any S1 mark (`NOT-CONSTRUCTIBLE`), rather than run partially.
  - W5 excludes a tree that touches the 0.1 m or 60 m clamp in any year.
  - H-W3 is reported per area and for both areas pooled.
  - Matched retained sets are computed for {C, L, M} (windows 1/5/10/20/50) and pairwise C vs D/S/S1/I
    (windows 10/50).
- **C9 — W12 executes the legacy CLI** (`--w12` or `w12` mode). That is an engine run and needs the same
  authorisation as the rest.

## Limitations

- **Never executed.** Logic errors, wrong guesses about runtime JSON (for example the exact text of large or tiny
  doubles), timing behaviour and performance are all unknown. The first real run may expose harness defects; the
  live checker fails closed in that case.
- The offline E3/CI and growth recomputation (H-W2, the E3 integrity line in `diagnostics.md`) assumes .NET
  evaluates float32 expressions without FMA contraction or extended precision (RyuJIT SSE scalar). A mismatch is
  reported as an anomaly, never "fixed".
- "Felled IDs absent from every neighbour list" is checked indirectly, because neighbour IDs are not exposed. The
  live check confirms each felled tree is dead in the next report's Before (E3 skips non-living trees). The
  offline check recomputes every evaluated row's CI and neighbour count from the living set and compares bits.
- The projection compares JSON re-emissions, not the raw response bytes. Raw bytes are kept and hashed in the
  transcript.
- Analysis CSVs can be large (14 sessions × ~50 years × ~244 rows). The raw per-response files dominate the bundle.
- None of the diagnostics validates the model. These are synthetic stands.

## Not verified (execution forbidden in the authoring task)

Nothing has been run:

- No engine was started and no session, request, transcript, replay or analysis output exists.
- No runtime JSON shape was observed; all shapes are inferred from source and the smoke test.
- Float-bit recovery was not exercised on real responses, and `selftest` was not run.
- Budget timing, process start/stop, SIGINT handling, lane-lock behaviour and every abort path are unexercised.
- The selection output has not been compared with any engine competition diagnostic.
````

### The gate that still stands

These bytes are the *object* a Manager would approve, not an approval. Stage D remains blocked on three things that are not mine to give: the Q1 packet flipped to **APPROVED TO RUN** on `4cecf873`, the physical Q1 execution lease (the custodian reserved the root and explicitly refused its use until a distinct Q1 packet exists), and a named, distinct run worker. Nothing above shortens that list by one item.



---

## 7. Stage E — multi-year simulation: **BLOCKED (no results exist)**

Nothing was run. Specifically: **zero** engine processes, **zero** sessions, **zero** ecological years, **zero** trees measured, **zero** Fell commands, and no N1 naturally-empty witness attempt. This is not a documentation gap — it is the whole of the Stage E outcome.

The chain that blocks it is short and every link is external:

| # | Requirement | Owner | State |
|---|---|---|---|
| 1 | Real L4 P0 isolation result | separate Sol executor, after the owner installs `strace` | **NOT RUN** |
| 2 | Stage I Manager disposition | Manager #885 | **NOT OBTAINED** |
| 3 | Q1 packet flipped to **APPROVED TO RUN** on `4cecf873` | Manager #885 | **NOT GRANTED** |
| 4 | Q1 physical execution lease | D-091 custodian | **REFUSED FOR NOW** — scratch reserved, explicitly not usable |
| 5 | Distinct qualified run worker | orchestrator routing + Manager | **NOT DISPATCHED** — would be a launch against a known blocker |

Because `Program.cs` exposes the two-area page only over loopback and the engine has no CLI route, any simulation requires starting a server in the reserved Q1 scratch lane. That lane is reserved and **not** authorised for use. Running anyway would have been the single most serious violation available in this session; it was not done.

**What it would have produced, and what remains unproven:** seven scenarios (`C, L, M, D, S, S1, I`) × two fresh sessions × up to 50 ecological years, every year recorded, per-tree DBH/CI/height/crown/mortality and every Fell receipt, cross-repeat determinism under the session-normalised projection, MA-B isolation between `I` and `C`, and an honest N1 empty-area witness only if natural mortality actually empties an area. None of that has any evidence, because none of it ran.

---

## 8. Stage F — independent review and next improvement: **BLOCKED**

There are no results to review, so no reviewer was dispatched and no verdict exists. A review of an empty result set would be a fabrication.

When Stage E eventually produces a bundle, the reviewer must be a **fresh Claude Opus 5.5 session** — not this orchestrator, not the harness author's session, not the run worker, and not a helper of any of them — and must separate software correctness from ecological validity, with the standing caveats that these are **synthetic authored stands**, site is **UNKNOWN**, MA-2 empirical comparability remains **G1 NO-GO**, and regeneration, windthrow, browsing, canopy light, timber grade, economics and cross-area coupling are **NOT MODELLED**.

---

## 9. Stage matrix

| Stage | Outcome | Evidence class |
|---|---|---|
| A — custodian addendum | **PASSED** | VERIFIED (issued by custodian, read back by orchestrator) |
| B — P0 isolation L4 | **BLOCKED — `strace` absent**, owner-executed install authorised | NOT RUN |
| C — Manager Stage I verdict | **NOT OBTAINED** | external authority |
| D — Q1 harness bytes | **PRODUCED** | authored + statically verified; NOT executed |
| D — Q1 run approval + lease | **NOT GRANTED** | external authority (two separate owners) |
| E — multi-year simulation | **BLOCKED**, zero runs | none |
| F — independent review | **BLOCKED**, nothing to review | none |
| G2 — owner player acceptance | **PENDING** | untouched |

---

## 10. Ledger

Canonical append-only ledger: `/home/jer/Development/ccf-885-orchestration/routing-ledger.jsonl` (48 records at the time of writing; mine appended as 47–48, never rewriting anyone else's rows).

| Record | Decision | State |
|---|---|---|
| 46 | `dec-885-d091add-20261121T2027Z` | VERIFIED — custodian addendum |
| 47 | `dec-885-A-custodian-20261121T2025Z` | VERIFIED — addendum routed, received, read back |
| 48 | `dec-885-B-p0-isolation-20261121T2032Z` | BLOCKED, NOT DISPATCHED — `blk-strace-absent-6.19` |

Blocker identity `blk-strace-absent-6.19` is stable while the evidence is "package absent, `sudo -n` refuses". It resolves only on new evidence from its owner: an observed install with the resolved version and `strace --version` output.

---

## 11. Outstanding defects and risks (none repaired here)

1. **L4 self-test host drift** — pinned to an Ubuntu 24.04 syscall profile; this host is 26.04 with uutils coreutils and python 3.14. May produce a legitimate FAIL. Requires a Manager disposition, not an edit.
2. **P0 isolation cannot run without an out-of-band OS package** — the programme's preflight assumed `strace` was present; it is not. An environmental assumption failed, not a code defect.
3. **Q1 harness is unexecuted** — every byte is disclosed and statically checked, but no fence, no serialiser and no projection has been exercised against a live server. Calling it runnable would be an overclaim.
4. **N1 naturally-empty served-history branch** — still **NOT OBSERVED**, still deferred, still not waived.
5. **Stage I closure** — unchanged at NEEDS CHANGES until at least L4 and the Manager disposition land.
6. **`/tmp` is a 3.9 GB tmpfs on this host** — a full `/tmp` silently breaks builds, harnesses and authoring. Any future worker must use `/home/jer/...` scratch, never `/tmp`, for bulk output.

---

## 12. What is needed next, and from whom

| # | Action | Who | Blocks |
|---|---|---|---|
| 1 | Install exactly `strace=6.19+ds-0ubuntu5` and report `strace --version` | **Owner**, own interactive sudo | Stage B |
| 2 | Run unchanged `tests/p0-isolation.sh` in the granted disposable lane and publish L0–L4 | separate Sol executor (dispatched once 1 lands) | Stage C |
| 3 | Record the Stage I disposition, including possible self-test host drift | Manager #885 | Stage D/E |
| 4 | Flip the Q1 packet to APPROVED TO RUN on `4cecf873` | Manager #885 | Stage E |
| 5 | Grant the Q1 execution lease on the reserved scratch root | D-091 custodian | Stage E |
| 6 | Approve the harness bytes and name the run worker | Manager #885 | Stage E |
| 7 | Independent review of the real result bundle | fresh Claude Opus 5.5 session | Stage F |
| 8 | G2 player test | Owner | acceptance |

A concrete risk-reduction proposal for step 6: approve the harness **conditionally**, with one bounded diagnostic session first — a single fresh process, one `year/advance`, no scenario, under the Q1 lease — to prove capability parsing, fence echo, float-bit recovery and null handling before committing to fourteen sessions. That is cheaper than discovering a harness bug at year 37 of scenario `S`.
