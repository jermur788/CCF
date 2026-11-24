# Phase 1 A1 — Ops Custody Readback Not Performed: Wrong Host

**Issued:** 2026-11-24T20:47Z by the DeepSeek V4.1 Flash orchestrator.
**Answering:** [Manager Ops custody readback request](https://github.com/jermur788/CCF/blob/docs/multi-phase1-a1-physical-ops-custody-readback-request-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-A1-MANAGER-OPS-CUSTODY-READBACK-REQUEST-20261124.md).
**Result:** **READBACK NOT PERFORMED — PERFORMER IS ON THE WRONG HOST.**
**Manager decision option reached:** **C — physical registry inaccessible from the performing host; no safe normal registration path from here.**
**A1 remains HELD.** No writer, source branch, worktree or RUNNING claim exists.

---

## 1. Why this is not the requested readback

The request names its recipient explicitly:

> "current custodian with real access to the original Ops host and established `CCF-Ops/lanes.json` registry; **not the DeepSeek orchestrator on `DESKTOP-DL1BQI7`**"

This session is that DeepSeek orchestrator, on that host. I did not simulate, infer or reconstruct the readback, and I will not present this document as custody evidence. It is a blocker receipt.

## 2. Host evidence (read-only, 2026-11-24T20:42–20:43Z)

| Probe | Observation |
|---|---|
| `hostname` / `whoami` | `DESKTOP-DL1BQI7` / `jer` |
| Kernel | `6.18.58.1-microsoft-standard-WSL2` |
| `/media/jer` | **absent** — no `ZX20`, no `Files` |
| WSL block devices (`lsblk`) | `sda` 722.9M, `sdb` 97.4M, `sdc` 2G swap, `sdd` 1T `/mnt/wslg/distro` — **no ZX20 volume** |
| Windows volumes (`Get-Volume`) | `C:` NTFS Fixed, `F:` CD-ROM, one hidden NTFS volume — **no `NetacPortableSSD`** |
| Windows disks (`Get-Disk`) | one `KBG30ZMV256G TOSHIBA NVMe`, 256 GB — **no external Ops SSD attached** |
| `command -v unity-lock ccf-ops packet-lint` | **none installed**; `/usr/local/bin` empty |
| `find / -name lanes.json` (whole fs) | only **evidence copies** under `/home/jer/CCF-Dev/…` (see §4) |
| `find / -type d -iname CCF-Ops` | only **dashboard test fixtures** (`…/tests/fixtures/ccf-ops`) |
| `~/.ssh` | **does not exist** — no configured remote route to the Ops host |

The Ops volume is not merely unmounted: the physical SSD is not attached to this machine. No amount of local inspection can produce the registry.

## 3. Canonical registry definition recovered (secondary evidence only)

A historical custody evidence copy on this host records the deployed schema. This corroborates the request's expectations; it is **not** a live read.

| Field | Recovered value |
|---|---|
| schema | `1` |
| storage root | `/media/jer/ZX20` |
| volume UUID | `18D6-896E`, `exfat` |
| device | `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2` |
| repository | `/media/jer/Files/CCF/Repository.git` |
| board | `CCF-Ops/board/active.md` |
| custody receipt host | recorded as "Claude Code via T3 Code, **jer-laptop**" |

**Unresolved path discrepancy — for the custodian to settle on the host.** Two forms appear in project records:

- [`OPS-UNITY-LOCK-01`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/OPS-UNITY-LOCK-01/TASK-PACKET.md) and the live `main` A1 row: `/media/jer/ZX20/CCF-Primary/CCF-Ops/lanes.json`
- the 2026-11-18 custody copy: `CCF-Ops/…` directly under the ZX20 volume

The request already states the path "must be confirmed by the custodian on the actual host". This discrepancy is a concrete reason for that requirement, and it is reported rather than guessed.

## 4. Newest reachable lane copy — and why it cannot answer the question

The only `lanes.json` files on this host are evidence snapshots from a 2026-11-18 H0-A custody transaction:

| Copy | Timestamp (UTC) | Lanes |
|---|---|---|
| `…/CCF-H0A-Year0Viewer-20261118/custody/before/lanes.json` | 2026-11-18T17:27:58Z | 23 |
| `…/custody/lane-status-record/lanes.json` | 2026-11-18T17:28:31Z | 25 |
| `…/custody/after/lanes.json` | 2026-11-18T17:28:32Z | 25 |

Multi-related entries in that snapshot, all from 2026-11-17/18: `MultiP0Proof674`, `MultiP0P1Integ687`, `Sen1SaveA0NonUnity690`, `MultiP1ChildB697`, `Sen1SaveA0ZX20NonUnity707`, `MultiP1ModeRepair710`, `MultiAnnualF0Child725`, `MultiH0ACore`, `MultiH0AViewer`.

**No Map Pass 1, Map Pass 2, Phase 1 A0 or Phase 1 A1 lane appears.** That is expected — those phases are from 2026-11-20 to 2026-11-24 and this copy is six days older.

**This must not be read as `NO MATCHING ENTRY`.** It is a stale transaction snapshot, not the live registry, and it cannot establish current custody. Recording it as a clean read would be exactly the "absence of evidence treated as proof of absence" failure the grant forbids. Custody of Map Pass 1/2, A0 and A1 therefore remains **UNKNOWN**.

## 5. What was not done

- No registry read (impossible here), no registry edit, no release, no expiry.
- No Unity lock, no `unity-lock acquire`, no bypass flag, no process killed.
- No replacement or shadow registry created; no fabricated lane rows.
- No source work, no A1 branch/worktree, no writer appointed, no RUNNING registration.

## 6. Exact next action

On the machine with the `ZX20` volume attached (the Ops host, recorded as `jer-laptop`), an authorised custodian runs only status operations with the normal tooling, then records:

1. actual hostname, custodian identity/role, UTC timestamp;
2. the confirmed live registry path and mount identity (`/media/jer/ZX20`, UUID `18D6-896E`);
3. the live lane inventory for any **Map Pass 1, Map Pass 2, Phase 1 A0, prospective A1, or broad `Tools/MultiRackSitkaP1Lab/**`** claim — each owner, status, scope, or an explicit "no matching entry" *from the live file*;
4. whether a new dedicated single-writer, source-only, non-Unity lane on `DESKTOP-DL1BQI7` may be registered, by whom, and what receipt will be returned;
5. the permanent GitHub URL of that receipt.

On that receipt I re-run A1 admission and proceed under the already-issued conditional G4 grant. Until then A1 stays HELD, and Phase B does not inherit anything from this.

**Verdict:** `A1 PHYSICAL ADMISSION HELD — OPS CUSTODY STILL UNKNOWN — READBACK REQUEST MUST BE EXECUTED ON THE OPS HOST`.
