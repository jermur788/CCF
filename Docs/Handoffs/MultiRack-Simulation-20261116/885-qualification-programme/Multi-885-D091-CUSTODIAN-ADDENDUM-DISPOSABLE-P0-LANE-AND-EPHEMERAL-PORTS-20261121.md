# Multi #885 — D-091 custodian addendum: disposable P0 isolation lane, ephemeral ports, prerequisite disposition

**Issued at 2026-11-21T20:27Z (observed clock `date --iso-8601=seconds` = `2026-11-21T20:27:10+00:00`).**
**This is a physical/custody addendum only.** It is **not** a Q1 execution lease, **not** a Stage I approval, **not** a Manager verdict, **not** a source release, and **not** permission to change `main`, Unity, Windows, saves, goldens or P0 source.

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-d091-addendum-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-CUSTODIAN-ADDENDUM-DISPOSABLE-P0-LANE-AND-EPHEMERAL-PORTS-20261121.md

## 0. Issuer identity and the appointment basis I actually verified

| Item | Value |
|---|---|
| Issuer | opencode-go / DeepSeek V4.1 Flash, T3 Code session `a8d35599-48af-4f1b-a652-26f5ff44da5e` — the same session that issued the admission receipt |
| Host | `DESKTOP-DL1BQI7`, Linux 6.18.58.1-microsoft-standard-WSL2 x86_64, `jer` uid 1000 |
| Appointment basis | The **owner's direct task message in this session** appointing this session as authorised D-091 physical Ops custodian and coordination-board owner, and the resulting published receipt `docs/multi-885-d091-admission-20261121-v1` (commit `b77f48fb`, blob `79f0690c`, sha256 `c6f1ebfb…b4d5`), which records that appointment in §0 |
| Revocation observed? | **None.** No owner message in this session, no live board entry and no document fetched today revokes or supersedes that appointment. The requesting session's own statement of *its* limits (**orchestrator only, must not issue physical permission**) is consistent with, and does not displace, this custody role |
| What the appointment does **not** cover | Forestry/engineering decisions, source approvals, Manager verdicts, review verdicts, Q1 execution approval, or any widening of the approved candidate `4cecf873` |
| Requesting session | DeepSeek V4.1 Flash orchestrator, thread `df357987-0f90-4f76-882b-f1f24ecbbb92` — a **different** session; treated as a request, not as authority |

I re-measured every fact in the request instead of adopting it. Where the request's figures and mine differ, mine are recorded below.

## 1. Live observations taken by me at issuance

| Observation | Measured value |
|---|---|
| Clock | `2026-11-21T20:27:10+00:00` |
| Disk | `/dev/sdd` on `/`, 1007 G total, 78 G used, **879 G available**, 9 % |
| #877 owner review | PID **1750123 ALIVE**, cwd `/home/jer/Development/CCF-Review-877/wt-owner/Tools/MultiRackSitkaP1Lab`, listener `127.0.0.1:33767` |
| #882 private playtest | PID **1806361 ALIVE**, cwd `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Launch/Tools/MultiRackSitkaP1Lab`, listener `127.0.0.1:43649` |
| #867 incident | PID 390578 **ABSENT** (unchanged; belongs to its owner) |
| Ports 33811 / 33841 | **not listening** (free at issuance) |
| Integration lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` exists, HEAD **`4cecf87339bea4d32907955a8d5625d1f27f96bd`**, branch `integration/multi-885-ma1-two-area-20261121-v1`, `git status --porcelain` **0 lines**, `.p0ref` present **read-only** (`SitkaLab.Core.dll`, `.sha256`, `provenance.txt`) |
| Disposable path | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121` **ABSENT** |
| Q1 scratch | `/home/jer/Development/CCF-885-Qualification-Scratch-20261121` **ABSENT** |
| Harness scratch | `/home/jer/Development/CCF-885-HarnessAuthor-Scratch-20261121` **ABSENT** |
| strace | **ABSENT** from PATH; `dpkg -l strace` → not installed; `apt-cache policy strace` → candidate `6.19+ds-0ubuntu5` from archive.ubuntu.com; `sudo -n true` → `sudo: interactive authentication is required` |
| Toolchain | python **3.14.4**; `sha256sum` = **uutils coreutils 0.10.0**; node at `/home/jer/.local/share/vite-plus/js_runtime/node/24.22.2/bin/node`; `/home/jer/.dotnet/dotnet` = **8.0.134**; system `dotnet` on PATH = **10.0.115** |
| P0 subtree at candidate | `git rev-parse 4cecf873:Tools/MultiRackSitkaLab` = **`f212c8f64fb28b83aa870c7b5b7c72e03051e46a`** — matches the protected-subtree expectation in the request |
| P0 reference expectation | the value `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` is present in tracked documentation at the candidate (including `Tools/MultiRackSitkaP1Lab/README.md` and the MA-1 author handoff), so it is a documented expectation, not an invented number |
| Integration status | The lane's HEAD **equals** the approved candidate exactly and the worktree is clean. That is a **state observation only**; I neither perform nor endorse Stage I verification, and no verdict is claimed here |

The one factual difference from the request: it reports disk as 879 G available; I measure the same value, but overall usage has moved from 77 G to 78 G used since my earlier receipt. No other discrepancy was found.

## 2. Dispositions (explicit)

### (i) One disposable P0 isolation worktree — **GRANTED** (permission to create; not created by me)

| Field | Grant |
|---|---|
| Path | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121` (verified absent) |
| Commit | **exactly** `4cecf87339bea4d32907955a8d5625d1f27f96bd` |
| Ref | **detached HEAD** at that commit. No branch ref — a disposable lane must not create a ref that could later be confused with the integration or author lane |
| Own artefacts | its own `.p0ref`, `bin/`, `obj/`, `TMPDIR`, and strace evidence **outside the repository tree** |
| Permitted action | run the **UNCHANGED** `Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh` in that worktree, and clean **only that disposable worktree's own** P1 outputs |
| Creator | the separate P0 isolation executor named in §4 — **not** this custodian and not the Q1 run worker |
| Cleanup | at documented completion the executor removes only this worktree (`git worktree remove` on its own path) and its own scratch; nothing else |
| Forbidden | editing any tracked file in any worktree; touching the integration, author (`CCF-Multi-885-MA1` @ `ac4c7b51`), reviewer (`CCF-Multi-885-Review-20261121` @ `58950a9a`) or any other worktree; deleting another lane's outputs; `git worktree prune` or any broad removal |

### (ii) Port policy — **GRANTED**, and this **supersedes** the fixed-port lines of my earlier receipt

The fixed requirements `127.0.0.1:33811` and `127.0.0.1:33841` recorded in `Multi-885-D091-PHYSICAL-ADMISSION-RECEIPT-AND-INTEGRATOR-SCOPE-20261121.md` §5 are **withdrawn and replaced** for this programme's lanes by:

- **loopback-only, OS-assigned ephemeral port** — no fixed port requirement anywhere;
- each run records the **actual bound endpoint, PID, owning session and source SHA**;
- **never** bind, proxy or forward to `#877` `127.0.0.1:33767` or `#882` `127.0.0.1:43649`, or any other process not owned by that run;
- stop **only your own PID**; never broad `pkill`, never kill a foreign listener.

This is a lease-policy amendment. It is **not** an execution approval: creating or running a server still requires the authorisation of the stage that owns it.

### (iii) Q1 qualification scratch — **RESERVED, NOT USABLE**; harness-authoring scratch — **GRANTED as physical space only**

| Item | Disposition |
|---|---|
| `/home/jer/Development/CCF-885-Qualification-Scratch-20261121` | **RESERVED and NOT authorised for use.** It exists for the future Q1 run lane, needs its own `TMPDIR`, and stays empty until a distinct Q1 execution packet exists. Reserved space is not permission to run |
| `/home/jer/Development/CCF-885-HarnessAuthor-Scratch-20261121` | **GRANTED** as physical scratch for harness **bytes**, with syntax-only checks permitted: `bash -n`, `python3 -m py_compile`, and static text inspection. **No engine start, no session, no listener, no build, no test execution** in this directory |
| Scope limit | Granting the directory does **not** authorise authoring, approving or adopting a Q1 harness. That authority belongs to the Q1 execution packet and the Manager. This addendum only settles where such files may physically live and be syntax-checked |

### (iv) Role separation — **RECORDED** (custody confirmation; role assignment itself is not mine to make)

| Role | Session / actor | Writing rights under this addendum |
|---|---|---|
| Custodian (physical/custody) | **this session** — opencode-go / DeepSeek V4.1 Flash | Issues custody grants and receipts; runs **no** builds, tests, harnesses or servers |
| Orchestrator | DeepSeek V4.1 Flash, thread `df357987-0f90-4f76-882b-f1f24ecbbb92` | Coordination only; may **not** self-issue physical permission |
| P0 isolation executor | a **separate** GPT-6.1 Sol / Codex session | Owns the disposable worktree of §(i); runs the unchanged isolation test; stops there |
| Q1 harness author | the orchestrator, as stated in the request (harness authoring only) | Writes harness **bytes** into the granted authoring scratch; no execution |
| Q1 run worker | a **different** GPT-6.1 Sol / Codex session | Only after a distinct Q1 packet; read-only against the integrated SHA; ephemeral loopback port |
| Independent reviewer | a **fresh** Claude Opus 5.5 session | Read-only review; no implementation, no custody |

No two roles above share a writable lane, a worktree, a port or an output root. **No fabricated lease expiry**: every lane above is open-ended under D-091 and ends on documented completion/release, owner-directed cancellation/handoff, or a genuine conflicting-writer/safety/authority stop. A genuine conflict is a **STOP**, not a negotiation.

## 3. Prerequisite disposition: strace — **(a) AUTHORISED, owner-executed only**

**L4 cannot run without `strace`; it is absent; this session has no root and `sudo -n` fails.** Disposition **(a)**, with binding conditions:

1. **Who may install:** the **owner only**, using the owner's own interactive `sudo` on `DESKTOP-DL1BQI7`. No agent session, including mine, may run the install, hold the password, or substitute another privilege mechanism.
2. **Exactly one package, pinned:** `strace` version **`6.19+ds-0ubuntu5`** from `archive.ubuntu.com`. No other package, no release upgrade, no repository configuration change, no service enablement.
3. **Recorded:** the installing party records the exact command, the resolved version, and the resulting `strace --version` output in that stage's handoff. If the installed version differs from the pin, that difference is reported, not silently accepted.
4. **Effect on lanes:** the install is a host-level prerequisite, not a lane grant. It confers no worktree, port or execution permission.
5. **If the owner declines or cannot install:** L4 is recorded **BLOCKED-ON-PREREQUISITE** and **no substitute tracing mechanism may be improvised** (no `ltrace` swap, no rewritten harness, no relaxed L4 criterion).

I do not claim this disposition makes L4 pass. It only removes the environmental absence.

## 4. Two risks recorded, not resolved — nothing may be edited to make them go away

1. **L4 self-test host compatibility.** The L4 self-test's expected-identity list is pinned to *this host's coreutils/bash/python syscall choices (Ubuntu 24.04), observed in #644*. This host runs **python 3.14.4** and **uutils coreutils 0.10.0**. The decoy self-test **may legitimately fail** for that reason. If it does, that is a **harness/host incompatibility to report**. It is **not** licence to edit `tests/p0-isolation.sh`, `tests/l4_analyze.py`, or the expected-identity list — those edits would require a new bounded source packet and a new independent review.
2. **Regenerated `.p0ref` identity.** Running the unchanged harness in the disposable worktree regenerates `.p0ref` there via `build/build-p0-core-ref.sh`. The generated reference must hash to **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`**, and the tracked P0 subtree `Tools/MultiRackSitkaLab` must stay byte-unchanged with tree **`f212c8f64fb28b83aa870c7b5b7c72e03051e46a`** (I verified that tree hash at the candidate myself, above). A mismatch is a **finding**, not a cue to regenerate, normalise or amend the expectation.

## 5. Exact preserved lanes (unchanged custody)

| Lane | Identity | This addendum |
|---|---|---|
| #877 owner review | PID 1750123, `127.0.0.1:33767` | Preserved; never bind/proxy/kill |
| #882 private playtest | PID 1806361, `127.0.0.1:43649` | Preserved; never bind/proxy/kill |
| #867 incident | PID 390578 absent | Untouched; belongs to its owner |
| Integration lane | `CCF-Multi-885-Integration-20261121` @ `4cecf873`, branch `integration/multi-885-ma1-two-area-20261121-v1` | Preserved; its `.p0ref`, `bin/`, `obj/` and outputs must **not** be deleted or reused |
| Author lane | `CCF-Multi-885-MA1` @ `ac4c7b51` | Preserved |
| Reviewer lane | `CCF-Multi-885-Review-20261121` @ `58950a9a` | Preserved |
| All other worktrees | as registered | Preserved |

## 6. What this addendum does not do

It does not approve Stage I, does not supersede the Manager's rulings, does not authorise Q1/Q2 execution, does not authorise the Q1 harness, does not merge or integrate anything into `main`, does not verify or re-verify the integrated candidate, and does not create any worktree, build, test run, session or listener. **No INTEGRATED, VERIFIED, REVIEWED or OWNER-ACCEPTED claim is made or implied here.**

**Next authorised action:** the separate P0 isolation executor creates the §(i) worktree once the owner has settled §3, runs the unchanged harness there, and reports the outcome — including a possible host-compatibility failure — without editing the harness.
