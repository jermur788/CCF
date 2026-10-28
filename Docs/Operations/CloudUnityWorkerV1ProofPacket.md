# Cloud Unity Worker V1 — TensorDock Proof Packet

**Status:** DRAFT — NOT AUTHORISED FOR PROVISIONING OR SPEND  
**Date:** 2026-10-28  
**Task class:** PACKET ONLY (infrastructure proof; no product, save, ecology or gameplay change)  
**Provider:** TensorDock configurable KVM VM, live EU host to be selected  
**Authority:** Supporting infrastructure execution plan only. It does not alter the Unity execution lock policy, current milestone scope, licensing policy, save format, product behaviour or implementation authority.

## Packet summary

| Field | Value |
|---|---|
| Publication base | `acb068d66d7b095a28d629651ee0bbbe06a132ac` (`origin/main`, 2026-10-28) |
| Benchmark base | `30fba2cdd7beded4007c8085e1fa7d58d4152dba` |
| Context commit | `444a11688529d736fa8d984c18aff95a7b968c1c` |
| Candidate HEAD | `30fba2cdd7beded4007c8085e1fa7d58d4152dba` |
| Repository | `https://github.com/jermur788/CCF.git` |
| Unity | `6000.6.0f1` Linux Editor, exact version only |
| Host shape | TensorDock KVM, RTX A4000 16 GB or better, 8 vCPU, 30–32 GB RAM, ≥250 GB NVMe, Ubuntu 22.04 |
| Location | Nearby EU (Germany / Netherlands / UK / France or comparable latency) |
| Running quote target | ≤ **$0.35/hour** complete machine, materially below Paperspace's $0.56/hour |
| Maximum approved spend | **NONE — user must set before provisioning.** Proposed planning cap: $15 total, not authorised. |
| Verification tier | Tier 1 infrastructure benchmark (not a product qualification) |
| Unity lock | Existing machine-wide CCF lock unchanged; this VM is not a CCF writable lane |
| Evidence destination | `/media/jer/Files/CCF/Verification/CloudUnityWorkerV1-<UTC>/` |

**Do not provision this machine, create a TensorDock account charge, or activate a Unity licence until the user explicitly approves the live host and a maximum spend.**

## 1. Objective

Determine empirically whether one TensorDock GPU VM can serve as a useful CCF Unity development worker, and whether two independent CCF Unity workers can run concurrently on that same VM without unacceptable contention.

The proof answers four questions:

1. Is a nearby EU TensorDock host with real GPU passthrough, acceptable CPU/RAM/NVMe and a complete quote ≤ $0.35/hour available?
2. How long do cold import, warm import, compile, a representative runtime gate and an interactive Editor session take on one worker?
3. What happens to RAM, swap, CPU, GPU/VRAM, disk I/O and wall time when a second independent worktree and Editor run concurrently?
4. Is the remote graphical experience good enough for CCF Editor work?

It does **not** qualify CCF gameplay, rebaseline any hash, change save policy, prove Windows parity, or authorise two-worker concurrency on local CCF lanes.

## 2. Authority and boundaries

Reference research:

- `Docs/Research/CloudUnityDevelopmentOptions-20261028.md`
- `Docs/Research/TensorDockPaperspaceComparison-20261028.md`
- `Docs/Handoffs/CloudUnityDevelopmentResearch-20261028/HANDOFF.md` at current `origin/main` (`1d1e627`)

The current handoff supersedes the earlier 16 GB-first direction. This packet therefore targets **30–32 GB system RAM**, not 16 GB.

In scope:

- live TensorDock inventory inspection and quote capture;
- one isolated benchmark VM;
- Unity installation and licence activation on that VM;
- one-worker and two-worker benchmark runs;
- remote graphical-access test;
- evidence export, cost record and teardown.

Out of scope:

- purchasing a reserved plan, commitment or long-term instance;
- production CCF development, integration, review or acceptance on the VM;
- changing `Docs/Project/coordination/active-tasks.md`, lanes, branches or locks;
- changing Unity, package, URP, input or save configuration;
- Windows builds, native input qualification or user playtests;
- licensing analysis or account-policy decisions;
- copying a local `Library/` to the VM;
- storing credentials, tokens, licence files or account secrets in tracked files.

## 3. Host selection gate (P0 — no spend)

Inspect the signed-in TensorDock inventory and record a dated receipt or screenshot. Select a host only if all of the following hold:

1. Location is nearby Europe with acceptable latency from the user's working location; record the location and measured round-trip time.
2. Preference order: **Core Compute** if available; otherwise **Top Host** with a high **Location Tier**. Record host/node identity and tier.
3. GPU is **RTX A4000 16 GB or better**, dedicated passthrough, not shared or emulated.
4. At least **8 vCPU**, with the actual host CPU model recorded.
5. **30–32 GB** system RAM.
6. At least **250 GB NVMe**. Host-local storage is acceptable only with the Git/publication rules in section 6.
7. Complete running quote is **≤ $0.35/hour** and materially below Paperspace's $0.56/hour.
8. Stopped-storage cost is recorded and acceptable.
9. SSH, required ports and the chosen graphical-streaming path are possible.
10. Ubuntu 22.04 with Nvidia drivers is available; 24.04 only if the exact Unity 6000.6.0f1 Linux requirements are confirmed at provisioning time.

If no host satisfies all ten conditions, **stop** and report. Paperspace RTX4000 AMS1 is the documented fallback, but switching to it requires a revised packet or explicit user approval.

## 4. Machine and software baseline

- Ubuntu 22.04 LTS x86-64, GNOME on **X11** for the graphical test.
- Nvidia proprietary driver with working NVENC; confirm with `nvidia-smi` and an actual Sunshine encode.
- Unity **6000.6.0f1** Linux Editor only; no other Unity version on the machine.
- Git, Python 3.10+, standard build tools and an SSH server.
- One non-root `ccf` user for project work; root only for system installation.
- Timezone UTC for all logs and timestamps.

A valid Unity licence is a prerequisite. Licence activation is the user's responsibility; record only that activation succeeded, never the licence value. If activation cannot be completed legitimately, stop the trial before benchmarking.

## 5. Disk and cache layout

Host-local storage is not replicated. Treat the VM as replaceable.

```text
/opt/unity/6000.6.0f1/          Unity Editor installation
/home/ccf/ccf-main/             primary clone at the exact candidate HEAD
/home/ccf/ccf-w1/               worktree 1 (one-worker baseline)
/home/ccf/ccf-w2/               worktree 2 (two-worker phase)
/home/ccf/state/w1/             isolated HOME/XDG state and saves for worker 1
/home/ccf/state/w2/             isolated HOME/XDG state and saves for worker 2
/home/ccf/logs/                 benchmark logs before export
/home/ccf/evidence/             evidence staging before export
```

Rules:

- Each worktree has its **own** `Library/`, `Temp/`, `Logs/` and `obj/`. Never copy or share a `Library/` between worktrees.
- Use a separate `XDG_CONFIG_HOME` and, where practical, a separate `XDG_CACHE_HOME` per worker to isolate Editor state and `forest-save.json`.
- The Unity package download cache may be warmed once; record whether it was shared or isolated in the run.
- `Library/`, `Temp/`, `Logs/` and `obj/` are disposable and are not evidence.
- No valuable data may exist only on the VM. Push commits and export evidence frequently.
- A 250 GB disk is the working minimum for two worktrees, Unity, caches and logs. Stop and report if free space falls below 15% during the trial.

## 6. Git and ownership

- Canonical source is Git (`https://github.com/jermur788/CCF.git`), not the VM.
- Clone and check out the exact candidate HEAD `30fba2c`; do not benchmark a moving branch.
- Worker 1 and worker 2 use separate worktrees from the same clone and separate branch names created for the trial only, for example `trial/cloud-worker-1` and `trial/cloud-worker-2`.
- The VM is **not** a CCF writable lane and receives no integration, review or acceptance authority.
- No production commits are required or expected. If a benchmark needs a disposable harness copy, follow the documented copy/run/remove workflow and remove the generated `.meta`.
- Push any accidental production change immediately to a trial branch and report it; do not merge or force-push.
- The existing machine-wide Unity-lock policy remains unchanged for all local CCF lanes. The only concurrency permitted by this packet is inside this isolated VM during phase P4; it is not precedent for local lanes.

## 7. Agent and access setup

- SSH key access for the user; no password-only login.
- Optional agent CLI or MCP integration may be installed for convenience, but it must be idle during timed runs so agent overhead does not contaminate measurements.
- Credentials for Git, TensorDock and Unity are supplied through the user's normal secret mechanism and are never written into the repository, benchmark scripts or evidence.
- Record the installed agent/version only if it is used; otherwise state "none".

## 8. Remote graphical access

Use one consistent layer so the provider comparison is not confounded:

1. GNOME on X11 with the Nvidia driver.
2. **Sunshine** host on the VM and **Moonlight** client on the user's laptop.
3. Verify hardware encoding is actually used; software encoding invalidates the interactive-latency result.
4. Test at 1920×1080 first. Record resolution, bitrate, codec, measured/estimated latency and any dropped frames.
5. If Sunshine cannot work on the selected host, use `x11vnc` only for diagnosis and record that the primary graphical path failed.

An SSH-only result is incomplete; the proof requires an interactive Editor session.

## 9. Benchmark phases

### P0 — Inventory and quote

Complete section 3. Capture host identity, components, location, tier, quote, stopped cost and latency. No provisioning.

### P1 — Provision and baseline

Install OS updates, driver, Unity, Git and SSH access. Verify:

- `nvidia-smi` reports the expected GPU and VRAM;
- CPU model, core count, RAM and swap;
- disk capacity and a simple read/write throughput check;
- Ubuntu and driver versions;
- Unity starts and the exact project compiles.

Record all versions and the exact Unity build.

### P2 — One worker, cold

1. Clone at `30fba2c` into `ccf-main`; create `ccf-w1` worktree.
2. Start with no `Library/`.
3. Run the first batch import/compile and record wall time and peak resource use.
4. Open `ForestTest` in the Editor once and confirm compile/console state.
5. Run one deterministic, non-destructive runtime gate, preferably `ScenarioReferenceVerification.Begin`.
6. Run `CCFIntegrationVerificationTemp.Begin` only after inspecting its save backup/restore behaviour; record that it can touch `forest-save.json`.
7. Remove any copied harness and generated `.meta`.

### P3 — One worker, warm

Repeat the P2 timed steps twice with the warm `Library/`. Report mean and spread. Do not hide a failed or anomalous run.

### P4 — Two workers, concurrent

Use `ccf-w1` and `ccf-w2`, each with its own `Library/`, isolated state and Editor.

1. **Staggered:** start worker 2 sixty seconds after worker 1 and run the same gates.
2. **Simultaneous:** start both Editors and the same gates together.
3. Record contention, swapping, OOM, crashes, hangs and wall time against the P3 baseline.
4. Do not run local CCF Unity work at the same time.

### P5 — Interactive graphical check

Open the project in the Editor through Sunshine/Moonlight. Check scene navigation, selection, UI responsiveness, text legibility, frame pacing and input latency. Record the user's qualitative verdict and the streaming statistics.

### P6 — Evidence and teardown

Export logs, measurement tables, screenshots, quote receipt and handoff. Verify the export before destroying or stopping the VM.

## 10. Measurements

Record per run:

| Field | Detail |
|---|---|
| Phase / worker | P1–P5, worker 1 or 2 |
| Start/end UTC | Exact timestamps |
| Wall time | Import, compile, gate, total |
| Peak RSS | System RAM used by the Editor process |
| Swap | Peak and sustained swap-in/out |
| CPU | Utilisation and saturation; host CPU model |
| GPU / VRAM | Utilisation and peak VRAM |
| Disk | Read/write throughput and observed stalls |
| Exit status | Editor and gate exit codes |
| Log path | Exact evidence path |
| Anomalies | Exceptions, retries, OOM, crashes, hangs |

Use one consistent sampling method for all runs and record it. Do not compare hashes across machines unless the documented configuration conditions match.

## 11. Stop conditions

Stop and report if any of the following occurs:

- no qualifying host or quote exceeds the approved cap;
- GPU passthrough is absent, `nvidia-smi` fails, or the GPU differs from the quote;
- Unity licence activation cannot be completed legitimately;
- cold import exceeds 60 minutes, or the Editor crashes/hangs twice in the same phase;
- sustained swap above 2 GB, OOM, or persistent interactive unresponsiveness;
- two-worker wall time for the same gate exceeds 2× the one-worker baseline, or the remote display becomes unusable;
- free disk falls below 15%;
- evidence cannot be exported safely;
- any production, save, lane, lock or credential boundary would be crossed.

Stopping the trial is a valid result; do not relax a condition to obtain a green run.

## 12. Cost and teardown

- Power off or destroy the VM immediately after evidence export. A stopped TensorDock VM continues to bill storage.
- Default disposition: **destroy** the VM after export, because local storage is not replicated.
- Retaining the VM requires explicit user approval and a recorded monthly storage cost.
- Record total powered hours, storage hours and estimated cost.
- If the trial is abandoned, tear down the same day and record the reason.

## 13. Acceptance criteria and handoff

The trial is complete when:

1. the live host/quote evidence is captured;
2. one-worker cold and warm measurements are recorded;
3. staggered and simultaneous two-worker measurements are recorded;
4. the Sunshine/Moonlight interactive check is recorded;
5. stop conditions are either not triggered or reported honestly;
6. the VM is stopped or destroyed and the cost is recorded;
7. a handoff recommends one of: usable one-worker machine, usable two-worker machine, unusable, or needs a different provider/shape.

Handoff path:

```text
/media/jer/Files/CCF/Verification/CloudUnityWorkerV1-<UTC>/HANDOFF.md
```

The handoff must state exact candidate HEAD, Unity version, host identity, quote, measurements, anomalies, cost, teardown state, remaining unknowns and a recommendation. It must not claim product acceptance or integration.

## 14. Decisions required before provisioning

The user must provide:

1. approval of the exact live host and quote;
2. a maximum spend, including storage;
3. the Git credential method for the private repository;
4. the Unity licence activation method;
5. whether the VM is destroyed or retained after the trial.

Proposed defaults for review only, **not authorised decisions**:

- **Spend cap:** $15 total provider spend for the initial one-worker and two-worker proof; stop immediately if the cap would be exceeded before useful evidence is obtained.
- **Git credentials:** a dedicated fine-grained GitHub credential or dedicated SSH key scoped to the CCF repository, stored only on the proof VM and revoked when the VM is destroyed. Never copy a broad personal credential bundle.
- **Unity activation:** the ordinary user-authorised Unity activation/sign-in flow required to launch the exact Unity 6000.6.0f1 Editor; stop if activation cannot be completed cleanly.
- **Destroy/retain:** destroy after evidence, pushes and handoff are verified. Retain only if the user explicitly decides the worker will immediately continue into production use. A retained TensorDock disk is convenience/cache, never canonical state.

Until all five decisions are explicitly resolved, this packet remains **DRAFT AND UNAUTHORISED**.
