# Multi #885 — D-091 owner playtest reservation, renewal after duplicate-launch incident

**RENEWED — RESERVED.** One fresh live owner playtest session for the same operator, on the same built demo lane, **no rebuild**, with a hardened single-launch protocol. Custody only; the custodian started nothing.

Issued **2026-11-22T00:08Z** (observed clock `2026-11-22T00:07:56+00:00`). Supersedes the earlier reservation `docs/multi-885-d091-owner-playtest-reservation-20261122-v1` commit `4a9e641b` for this operator only.

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-d091-owner-playtest-renewal-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-OWNER-LIVE-PLAYTEST-RESERVATION-RENEWAL-20261122.md

## 1. Incident record (operator-reported, independently re-checked)

| Reported | Independently observed at issue |
|---|---|
| A second supervisor was started because the first had not yet written its private PID file | Consistent with the stale artefacts found: `own-server.pid`, `owner-supervisor.py`, `access-private.json`, `owner-private.json`, `runtime-private.log` in the evidence root |
| The duplicate exited on seeing an existing private file | No duplicate process remains |
| One owned listener PID **2206517** on `127.0.0.1:43019`, stopped by SIGTERM | PID **2206517 ABSENT**; `:43019` **free** |
| Only #877/#882 remain listening | Confirmed: only `127.0.0.1:33767` (PID 1750123) and `127.0.0.1:43649` (PID 1806361) |
| No source or protected process touched, no capability exposed | Demo lane `4cecf873`, status **0 lines** including untracked; `.p0ref` pin unchanged; no demo-lane process; no orphaned `tmp-owner-run` children; no capability in any published artefact |

**Custody verdict on the incident:** the operator stayed inside its own lane, killed only its own PID, and exposed nothing. The **defect was procedural** — a launch design that depended on an asynchronous supervisor's PID file, where a missing file looked like "not started yet" and invited a second launch. That procedure is corrected below; the lane itself is undamaged and needs no repair.

## 2. Live verification at issue

| Check | Value |
|---|---|
| Host / clock | `DESKTOP-DL1BQI7`, `2026-11-22T00:07:56+00:00`, 878 G free |
| Demo lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Demo-Playable-20261121`, HEAD **`4cecf87339bea4d32907955a8d5625d1f27f96bd`**, detached, tracked+untracked status **0 lines** |
| Pinned P0 | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, mode `r--r--r--` |
| Built UI | `src/Ui/bin/Debug/net8.0/sitkap1-ui.dll` present (23:55) → **rebuild not permitted and not needed** |
| Evidence root | `/home/jer/Development/CCF-885-Demo-Evidence-20261121`, mode `drwx------`, 43 entries |
| Preserved lanes | #877 `1750123:33767`, #882 `1806361:43649`, #867 390578 absent (not released), integration `4cecf873`, author `ac4c7b51`, reviewer `58950a9a`, 30 worktrees |

## 3. Operator

Thread **`031c83ff-a41d-4e14-afc4-2048acded1cf`** — "Multi-885 Playable 2D Demo Priority", `codex_3` / **gpt-6.1-terra**. Same operator as the previous reservation. **Exactly one listener** may exist at any time; the completed one-shot run session must not start a concurrent one; any different operator must declare its own thread id and stay inside this receipt.

## 4. Hardened launch protocol (this replaces the failed procedure)

**Invariant: one launch, synchronously observed. A missing PID file is never a licence to start a second process.**

1. **Pre-launch assertions** (all must hold, else stop and report):
   - no listener on any demo endpoint, including the abandoned `:43019` and `:37729`;
   - no process whose cwd is the demo lane;
   - the demo lane is at `4cecf873` with 0 status lines and the P0 pin above;
   - the built `sitkap1-ui.dll` exists.
2. **Rotate own private artefacts first**: overwrite `own-server.pid`, `access-private.json`, `owner-private.json` (or remove them) **before** starting, so stale state can never be mistaken for a live one. Only the operator's own files may be removed; nothing else in the evidence root.
3. **Start synchronously in the foreground** from `<demo lane>/Tools/MultiRackSitkaP1Lab`:
   `TMPDIR=/home/jer/Development/CCF-885-Demo-Evidence-20261121/tmp-owner-run /home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived`
   using `/home/jer/.dotnet/dotnet` **8.0.134** only. Read the bound endpoint from **this process's own stdout** with a bounded wait; do **not** delegate the launch to an asynchronous supervisor whose PID file you must poll.
4. **Record** the PID, the actual loopback endpoint, the session id and the source SHA. Write `own-server.pid` atomically (temp file + rename), never as a partial write.
5. **Never start a second supervisor.** If the endpoint is not yet visible, wait on the existing process's output or inspect `ss -ltnp` for its listener — do not launch again. A second launch is a stop condition, and the newer PID is the one to stop, keeping exactly one listener.
6. **Keep alive** while the owner tests — the only reason the listener may outlive the operator's turn. **No unattended overnight parking:** if the owner does not connect, stop the process and report.
7. **Cleanup on owner completion or cancellation:** `SIGTERM` that PID only, confirm the endpoint is gone, leave lane and evidence in place. No broad `pkill`, no foreign listener touched.

## 5. Capability and owner access

A **fresh** capability is generated by this launch. It is handed to the owner **privately in chat only** — never in GitHub, Markdown, logs, screenshots or the evidence inventory; the previous run's URL/capability is dead and must not be reused. Local copies mode `600` inside the `700` evidence root, overwritten atomically as in §4.2.

Owner session scope is unchanged: open the private `127.0.0.1:<ephemeral>` link, inspect A and B, Crop a retain tree, add Fell with a reason, read the plan, **Approve (0 Felled, year unchanged)**, **Execute (only intended IDs, year unchanged)**, advance 1–3 real years, inspect counts/BA/Dq/RD/competition/mortality/history. The published `MA-A-003` / `MA-A-004` sequence remains valid. Full-width screenshots or a recording **only if no new tooling is needed** — that is the standing remedy for the DEMO INCOMPLETE evidence gap.

**Forbidden:** rebuild, source or rule change, save changes, Fell-to-empty, Q1 scenarios, year-50 work, Unity, Windows build, capability publication, and any claim of G2 acceptance, Stage I approval, qualification or calibration.

## 6. STOP conditions

Endpoint already in use · any demo-lane process detected before launch · lane not at `4cecf873`, dirty, or pin/artefact mismatch · a need to build or restore · capability exposure risk · a second launch attempt · owner collision with #877/#882 activity. On any of these: stop, report the exact fact, take no workaround.

## 7. Non-claims

No playtest has occurred. This is a custody renewal after a procedural incident, not a repair of the demo evidence gap and not an acceptance. Stage I remains **NEEDS CHANGES**; Q1 remains **BLOCKED**; the read-only disk-audit grant is separate and unaffected.
