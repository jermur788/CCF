# HANDOFF — Cloud Unity Development Infrastructure Research — 2026-10-28

**Status:** RESEARCH COMPLETE — MANAGER REVIEWED; NO INFRASTRUCTURE PROVISIONED  
**Primary research:** `Docs/Research/CloudUnityDevelopmentOptions-20261028.md`  
**TensorDock comparison:** `Docs/Research/TensorDockPaperspaceComparison-20261028.md`  
**Authority:** Supporting infrastructure research only. It does not alter Unity execution locks, current milestone scope, product behaviour, licensing policy, or implementation authority.

## User clarification

The earlier **16 GB first** sequence was not an independent requirement; it was a consequence of the first provider options. The current objective is to obtain the **best useful Unity capacity for the money**, then test one and two concurrent independent CCF Unity workers before deciding whether a larger machine is needed. Licensing remains deliberately outside scope.

## Updated research verdict

### TensorDock — first live-inventory challenger

TensorDock now deserves the first live-availability check. It provides full KVM/Libvirt VMs with dedicated GPU passthrough rather than containers, configurable CPU/RAM/NVMe, Ubuntu with Nvidia drivers, per-second billing, and stopped storage-only billing.

An illustrative 8-vCPU / 30-GB RAM / 250-GB NVMe / RTX A4000 16-GB VM is approximately **$0.1965/hour** using TensorDock's published typical A4000/component rates. Even with a more expensive currently surfaced A4000 component price of $0.22/hour, that illustrative shape is about **$0.3165/hour**. These are calculations from published component rates, **not guaranteed quotes**; actual price, CPU model, quality and availability vary by host and location.

TensorDock's main disadvantage is marketplace variability and the explicit absence of replication for ordinary local storage. Prefer **Core Compute** if a suitable nearby European location/GPU exists; otherwise prefer a **Top Host + high Location Tier** and keep Git/offsite publication authoritative.

### Paperspace — predictable fallback/control

Paperspace RTX4000 AMS1 remains a strong fixed option at **$0.56/hour**: 8 vCPU, 30 GB RAM, Turing RTX4000 8 GB VRAM, known Xeon Silver 4215R CPU class and official Amsterdam availability. It is less variable than TensorDock but materially more expensive if a good EU TensorDock host is available.

### Other retained options

- **AWS g4dn.xlarge Ireland:** mature operational/control baseline; 4 vCPU/16 GiB/T4; strongest turnkey Linux remote-workstation path, but poor raw value versus the current alternatives.
- **Akamai RTX 4000 Ada:** strong fixed GPU option, but the 16-GB Small configuration no longer has a special reason to lead after the user removed the 16-GB-first constraint.
- **boat.dev:** complementary CPU-only implementation/review/agent pool, not a Unity GPU replacement.
- **Runpod/Vast:** experimental container GPU lanes, not the reference Unity workstation.

## TensorDock live-host acceptance criteria

Before provisioning, inspect the signed-in live TensorDock inventory for:

1. nearby Europe (Germany/Netherlands/UK/France or comparable latency);
2. Core Compute if available, otherwise Top Host + high Location Tier;
3. RTX A4000-class or better GPU;
4. at least 8 vCPU with a modern/high-clock host CPU;
5. about 30–32 GB RAM;
6. at least 250 GB NVMe;
7. complete running quote ideally **≤ $0.35/hour** and materially below Paperspace's $0.56/hour;
8. acceptable stopped-storage cost and networking/port options.

If those conditions are met, TensorDock is the preferred first real CCF trial. If they are not, use Paperspace RTX4000 in AMS1 as the predictable fallback.

## Trial once a provider is selected

- exact Unity 6000.6.0f1 and selected CCF candidate HEAD;
- one isolated worktree/Editor/agent baseline;
- then two isolated worktrees/Editors/agents concurrently;
- measure RAM/swap, CPU saturation, GPU/VRAM, disk I/O, import/compile/test/runtime wall time and graphical-streaming usability;
- stop on crashes/hangs, persistent swapping, severe interaction latency or disproportionate wall-time regression;
- retain the existing machine-wide Unity-lock policy until Manager review of the benchmark evidence explicitly changes it.

## User action required

No purchase has been made. The only remaining information that cannot be established reliably from public pages is the **actual current nearby-European TensorDock host inventory and quote**, which is exposed through the signed-in deployment inventory/API.