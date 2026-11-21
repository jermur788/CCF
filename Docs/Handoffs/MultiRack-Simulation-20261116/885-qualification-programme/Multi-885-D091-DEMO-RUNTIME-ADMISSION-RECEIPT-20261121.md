# Multi #885 — D-091 one-time demo runtime admission receipt

**ADMITTED** — one-time, non-qualification gameplay demonstration lane at exact `4cecf87339bea4d32907955a8d5625d1f27f96bd`.
**Issued 2026-11-21T23:39Z** (observed clock `2026-11-21T23:39:05+00:00`).
**This is physical/custody admission only.** It is **not** Stage I approval, **not** Q1 authorisation, **not** a source release, **not** an empirical/ecological qualification, and **not** owner G2 acceptance. No demo run was performed by the custodian.

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-d091-demo-admission-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-DEMO-RUNTIME-ADMISSION-RECEIPT-20261121.md

## 0. Issuer, basis, and what was refreshed first

| Item | Value |
|---|---|
| Issuer | existing D-091 physical custodian — opencode-go / DeepSeek V4.1 Flash, T3 session `a8d35599-48af-4f1b-a652-26f5ff44da5e` |
| Host | `DESKTOP-DL1BQI7`, Linux 6.18.58.1-microsoft-standard-WSL2 x86_64, `jer` uid 1000, `/dev/sdd` 1007 G total / **879 G available** |
| Authority basis | Owner's direct instruction in this session to admit a one-time demo lane as the **existing** D-091 custodian, recorded against the prior custodian receipts `b77f48fb` (admission) and `efb188ac` (addendum). No revocation of the custodian role was found in this session, the live board, or today's fetches. No Sol session, orchestrator session or demo worker can self-issue this |
| Task packet | `docs/multi-885-owner-playable-demo-priority-20261121-v1` → `Multi-885-OWNER-PRIORITY-PLAYABLE-2D-DEMONSTRATION-NEXT-TASK-20261121.md` (Manager: reprioritisation, **not** a waiver of Stage I; prefer option (a): separate already-safe exact-SHA demo worktree with a copied verified pinned P0 binary) |
| Board refresh | live `Docs/Project/coordination/active-tasks.md` fetched; **no demo/playable-demo lane is registered**, so no competing claim exists |
| Option used | **(a)** — a new, separate demo worktree. The preserved integration worktree is **not** used as a runtime (option (b) unnecessary) |

## 1. Live safety verification (measured by me, not assumed)

| Check | Result |
|---|---|
| Loopback listeners | only `127.0.0.1:43649` (#882, PID 1806361) and `127.0.0.1:33767` (#877, PID 1750123) |
| #877 | PID 1750123 **ALIVE**, cwd `/home/jer/Development/CCF-Review-877/wt-owner/Tools/MultiRackSitkaP1Lab` |
| #882 | PID 1806361 **ALIVE**, cwd `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Launch/Tools/MultiRackSitkaP1Lab` |
| #867 | PID 390578 **ABSENT** (untouched; its owner's incident) |
| Integration lane | `CCF-Multi-885-Integration-20261121` @ **`4cecf873`**, branch `integration/multi-885-ma1-two-area-20261121-v1`, `git status` **0 lines** |
| Author lane | `CCF-Multi-885-MA1` @ `ac4c7b51`, clean, preserved |
| Reviewer lane | `CCF-Multi-885-Review-20261121` @ `58950a9a`, clean, preserved |
| Demo worktree path | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Demo-Playable-20261121` — **ABSENT** (fresh allocation) |
| Demo evidence root | `/home/jer/Development/CCF-885-Demo-Evidence-20261121` — **ABSENT** |
| Removed P0 isolation lane | `CCF-Multi-885-P0Isolation-Disposable-20261121` — **ABSENT**; not recycled (policy) |
| Worktrees registered | 29; none named `demo` |
| Existing demo branch refs | none local; two unrelated historical `remotes/origin` refs only |

**Pinned P0 binary verified by hash, not by trust:**

```
sha256  <integration>/.p0ref/SitkaLab.Core.dll
      = 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3   ← matches the required pin
sidecar SitkaLab.Core.dll.sha256 = same value
provenance p0QualifiedTree = f212c8f64fb28b83aa870c7b5b7c72e03051e46a
           p0QualifiedCommit = a7f41983c2f2f233b8fe999d1a5b10e72bdd2866
           sdk = 8.0.133
```

The Stage B cold-isolation failure (rebuilt `bcbdbf05…` vs pin `9a99f781…`) therefore does **not** taint this lane: the demo copies the **pinned** binary and never rebuilds it.

**Port reality confirmed at source:** `src/Ui/Program.cs:54` is `kestrel.Listen(IPAddress.Loopback, 0)` — the application already asks the OS for an ephemeral loopback port and ignores fixed-port configuration. Fixed `33811`/`33841` assumptions are therefore both obsolete and unimplementable, consistent with the earlier blocker `blk-5dcd40130d7cc1d4`.

## 2. Allocation

| Field | Granted |
|---|---|
| Worktree path | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Demo-Playable-20261121` |
| Commit | exact `4cecf87339bea4d32907955a8d5625d1f27f96bd` |
| Ref | **detached HEAD** — no branch ref, so a demo lane cannot be mistaken for integration, author or reviewer work |
| Created by | the **separate Codex demo worker** (§3). The custodian does not create it and does not run the demo |
| Source state | read-only; tracked files must remain byte-identical to `4cecf873`. `git status` must stay **0 lines** for tracked files at handoff |
| Unique writer | the demo worker only, one session, one run per §5 |

## 3. Actor separation

| Role | Actor | Rights |
|---|---|---|
| Custodian | this session (DeepSeek V4.1 Flash) | issues this receipt; runs nothing |
| **Demo worker (one-shot)** | a **separate Codex session** (GPT-6.1 Sol / `codex_3`), a fresh thread distinct from the Stage B P0 executor and from any Q1 run worker | creates the worktree, copies and verifies the P0 binary, compiles once, runs the demo, captures evidence, stops its own process |
| Orchestrator | DeepSeek V4.1 Flash orchestrator session | coordination only; cannot self-issue or widen this grant |
| Reviewer / author lanes | untouched | no rights here |

## 4. Permitted content — exactly what may be compiled or linked

**Unmodified source that may be compiled in the demo worktree** (byte-identical to the candidate; no edits):
`Tools/MultiRackSitkaP1Lab/src/Core/**`, `src/Cli/**`, `src/Ui/**` (`Program.cs` blob `ac659be7`, `PlayerApi.cs`, `PlayerMultiApi.cs` blob `42462366`, `wwwroot/app.js` blob `c1c50f0f`), `tests/**` read-only, `Directory.Build.props`, `global.json`.

**Compile-linked external files (read-only, no edits):**
`Assets/ForestPrototype/SitkaGrowthModel.cs` (blob `e4c73402`), `Assets/ForestPrototype/SimulationRandom.cs` (blob `bb83d7f8`), `Assets/ForestPrototype/TreeSpeciesDefinition.cs`, `Assets/ForestPrototype/Species/SitkaSpruce.asset`.

**UI and fixture inputs (read-only):**
`Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` (blob `8bf81a43`), `src/Ui/wwwroot/*` static assets, and the pinned `.p0ref` copy described in §5.

**Pinned P0 binary — copy, never regenerate:**

1. Copy `.p0ref/SitkaLab.Core.dll`, `.p0ref/SitkaLab.Core.dll.sha256` and `.p0ref/provenance.txt` **read-only** from `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121/Tools/MultiRackSitkaP1Lab/.p0ref/` into the **demo worktree's own** `.p0ref/`.
2. Immediately verify the **destination** hash equals `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; set the copy read-only (`chmod 444`).
3. **Forbidden:** any invocation of `build/build-p0-core-ref.sh`, `tests/p0-isolation.sh`, or any cold-isolation path in this lane; any hash mismatch must **STOP** the run and be reported, never normalised.

**One normal compile is permitted and required** — `dotnet build` of the Lab projects in the demo worktree only, using **`/home/jer/.dotnet/dotnet` (8.0.134)**, never the system `10.0.115` on PATH. Rationale: a fresh worktree has no `bin/` and the UI cannot start otherwise; this compile is **not** a P0 rebuild and must write only to the demo worktree's `bin/`, `obj/`. No `git clean`, no `dotnet clean` outside that worktree, no build-output deletion in any other lane.

## 5. Permitted runtime and demonstration scope

| Item | Grant |
|---|---|
| Listener | **loopback-only, OS-assigned ephemeral port** (`Listen(IPAddress.Loopback, 0)`); record the **actual bound endpoint, PID, session and source SHA** per run. **Never** bind, proxy or forward to `127.0.0.1:33767` or `127.0.0.1:43649`; no fixed-port requirement anywhere |
| Session lifecycle | start **one** server process; stop **only that PID**; no broad `pkill`; never kill or restart a foreign process |
| Allowed interaction | the **existing unmodified** two-area Lab UI/API: inspect areas and trees, mark Crop retain trees, order Fell with a written reason, inspect the pending Work Plan, **Approve (zero Felled)**, **Execute (exact intended IDs Felled, zero year advancement)**, then advance **1–3 genuine ecological years**, and inspect per-area counts, BA/stand metrics, competition, mortality and Work Plan history; one area may be left unmanaged, or a fresh independent no-work session used as the control |
| Evidence | screenshots or a short recording **only if available without new tooling**, plus raw API snapshots; written to `/home/jer/Development/CCF-885-Demo-Evidence-20261121`; **capability tokens / bearer secrets must be redacted** from anything published; screenshots must be legible and 4–6 sequential frames minimum for the action sequence |
| Handoff state | end at **READY FOR OWNER BROWSER TEST**. Any session left alive for the owner requires a **new specific reservation**; an unowned listener must not outlive the run |

**Forbidden in this lane:** any source edit or commit; any P0 rebuild or regeneration; `strace`/tracing tooling; the Q1 harness, Q1 scenarios, year-50 work or any Q1 scratch use; Unity; Windows build; any save-file or persistence change; `main` merge; new features, UX work, new forestry models; manual Fell-to-empty (D-093 remains); any test-suite expansion (a single targeted check after a **real** failure only); any claim of Stage I approval, qualification, calibration, empirical comparability, owner G2 or acceptance.

## 6. Preserved custody (must not be touched)

| Lane | Identity |
|---|---|
| #877 owner review | PID 1750123 @ `127.0.0.1:33767` |
| #882 private playtest | PID 1806361 @ `127.0.0.1:43649` |
| #867 incident | PID 390578 absent; its owner's business |
| Integration | `CCF-Multi-885-Integration-20261121` @ `4cecf873` — including its `.p0ref`, `bin/`, `obj/` and outputs, which the demo must neither reuse destructively nor delete |
| Author | `CCF-Multi-885-MA1` @ `ac4c7b51` |
| Reviewer | `CCF-Multi-885-Review-20261121` @ `58950a9a` |
| All other registered worktrees | preserved |

## 7. Stop conditions (immediate STOP, report, do not repair)

Wrong SHA or pin mismatch · any need to edit a protected or source file · a foreign process on the chosen endpoint · a user session already using the demo path · any demand for `build-p0-core-ref.sh`, cold isolation or tracing · Work Plan semantics that contradict the frozen Q0 description · a token/secret exposure risk · any urge to fix product code. Repair requires a new bounded source packet; the demo may not absorb it.

## 8. Dispatch-ready brief for the separate demo worker

> You are the **one-shot Codex demo worker** for CCF Multi #885. Physical receipt: `<this file URL>`. Do not change source, do not rebuild P0, do not trace, do not run Q1. Create `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Demo-Playable-20261121` as a **detached HEAD** worktree at `4cecf87339bea4d32907955a8d5625d1f27f96bd`; copy `.p0ref` (3 files) read-only from `CCF-Multi-885-Integration-20261121`; verify the copied `SitkaLab.Core.dll` sha256 is `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` and `chmod 444`; build once with `/home/jer/.dotnet/dotnet` (8.0.134); run the unmodified UI on the OS-assigned loopback port; record endpoint/PID/session/SHA; perform one Crop/Fell Work Plan (Approve 0 Felled, Execute exact IDs, no year change) across two areas, advance 1–3 real years, capture counts/BA/metrics/mortality/history and legible evidence to `/home/jer/Development/CCF-885-Demo-Evidence-20261121`; redact tokens; stop only your own PID; publish one Markdown handoff stating **DEMO OBSERVED** or **DEMO INCOMPLETE** with the exact SHA, what the owner still must test, and every defect observed. Never claim Stage I approval, qualification or acceptance.

## 9. Non-claims

**No INTEGRATED, VERIFIED, REVIEWED, QUALIFIED or OWNER-ACCEPTED claim is made here.** Stage I remains **NEEDS CHANGES** with L4 not run and N1 naturally-empty not observed; Q1 remains **BLOCKED**; the Stage B failure record is preserved unchanged. This receipt allocates a lane; it observes nothing about the demo itself.
