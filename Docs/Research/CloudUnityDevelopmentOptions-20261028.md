# Cloud Unity Development Options — 2026-10-28

**Status:** SUPPORTING INFRASTRUCTURE RESEARCH — NOT DECISION AUTHORITY  
**Scope:** CCF Unity development throughput; no licensing analysis; no infrastructure provisioned.  
**Research baseline:** `main` observed at `7b78a528e3d563f33a6bbf650cde5695c430e2a8` before publication.  

## CCF requirement

The immediate bottleneck is access to Unity-capable execution, not generic code editing. CCF currently develops on Linux with Unity 6000.6.0f1. The desired experiment is deliberately small:

1. provision a **16 GB system-RAM GPU machine**;
2. run one representative CCF Unity worker and measure it;
3. run two independent CCF Unity workers/worktrees concurrently on the same 16 GB machine and measure contention;
4. move to 32 GB only if the measurements justify it.

The workstation should provide a persistent Linux environment, GPU acceleration, persistent local project/Unity cache, SSH/agent access, interactive graphical access, and a clean way to stop compute when idle. Existing CCF single-writer/worktree ownership remains required. The present machine-wide Unity-lock rule is not changed by this research.

Unity's published Unity 6 requirements matter to provider selection: Ubuntu 22.04/24.04 on x64 with Nvidia/AMD graphics is supported, with GNOME/X11 or supported Wayland combinations. Unity also states that the Editor-supported form factor is a workstation/laptop running without an emulation, container, or compatibility layer. The latter makes full virtual machines a cleaner baseline than GPU Docker/Pod services for this experiment.

Sources:
- Unity 6.0 system requirements: https://docs.unity.com/ko-kr/engine/6000.0/manual/get-started/install-and-upgrade/getting-started-installing-unity/system-requirements
- Unity 6.3 system requirements (same workstation/container boundary): https://docs.unity.com/en-us/engine/6000.3/manual/get-started/install-and-upgrade/getting-started-installing-unity/system-requirements

## Shortlist

| Provider / shape | System resources relevant to first test | Approx running price | Persistence / access | CCF assessment |
|---|---|---:|---|---|
| **Akamai Cloud RTX 4000 Ada x1 Small** | **4 CPU, 16 GB RAM, RTX 4000 Ada, 500 GB storage** | **$0.52/h Europe** | Dedicated VM; normal cloud VM administration. Linux graphical streaming would be added by us. | **Tier 1. Very strong exact-fit candidate.** Newer workstation GPU and large included disk at almost the same hourly cost as AWS G4dn. Need to confirm actual GPU capacity in the preferred nearby European region before purchase. |
| **TensorDock custom VM + RTX A4000-class GPU** | Configurable CPU/RAM/storage; exact **16 GB RAM** is supported; RTX A4000 typical GPU component price about $0.10/h; Ubuntu 22.04 template with Nvidia drivers | Marketplace / host-variable. Illustrative 8 vCPU + 16 GB + 100 GB + typical A4000 is roughly **$0.16/h**, not a guaranteed quote. | Full virtual machines with GPU passthrough and SSH. Local storage is explicitly **not replicated**. Linux graphical streaming would be added by us. | **Tier 1, best price-performance experiment if a suitable EU host is available.** Marketplace availability/reliability and non-replicated local storage are the trade-off. Git must remain canonical and frequent pushes are mandatory. |
| **AWS EC2 g4dn.xlarge, eu-west-1** | **4 vCPU, 16 GiB RAM, T4 16 GB VRAM**, 125 GB local NVMe; persistent EBS added separately | **$0.587/h (~€0.52/h current Ireland list-price trackers)** | Mature EC2/EBS operations. AWS DCV is a strong turnkey Linux GPU remote-desktop route. | **Tier 1 control/baseline.** Most predictable and closest region; older/weaker GPU and storage costs make raw value lower than Akamai/TensorDock. Best operational baseline. |
| **Paperspace RTX4000, AMS1** | 8 vCPU, **30 GB RAM**, RTX4000 8 GB VRAM | **$0.56/h** | Persistent VM; compute billing stops when powered off; storage continues. Linux machines are SSH-oriented and Paperspace desktop streaming is Windows-only; new Windows templates are retired. | **Tier 2 / 30 GB fallback.** Excellent raw price but cannot run the requested exact 16 GB first experiment and needs a third-party Linux streaming layer. |
| **machine0 gpu-4000ada-1** | 8 vCPU, **32 GB RAM**, RTX 4000 Ada 20 GB, 500 GB disk | **$0.836/h** | Persistent Ubuntu/NixOS VM, static IP, CLI/MCP, suspend/snapshot/resume; Codex/Claude integration is unusually strong. | **Tier 2.** Excellent agent orchestration, but overprovisioned and more expensive for the first CCF Unity test. No provider-native Linux interactive graphics advantage identified. |
| **Runpod RTX A5000 Pod** | Current published example: 9 vCPU, 25 GB RAM, A5000 24 GB VRAM | **$0.27/h** | GPU Pod/container model; official GPU-accelerated Ubuntu desktop templates exist; persistent volume/network storage has separate economics and stopped GPU availability can change. | **Experimental.** Attractive price and graphical access, but container architecture is outside Unity's published supported Editor form factor. Useful later if a controlled trial proves reliable. |
| **Vast.ai** | Marketplace-dependent | Often very cheap | Linux Docker-container instances; marketplace host variability; remote graphical desktop is DIY. | **Experimental / lower priority** for the same Unity-support reason as Runpod, with additional variability. |
| **boat.dev large** | 8 vCPU, 16 GB RAM, 125 GB disk, **no GPU** | **$0.072/h** plus plan minimum/credit model | Persistent full Ubuntu VM, SSH, 60fps virtual desktop, snapshots/forks, agents/devtools preinstalled | **Complementary, not a Unity-GPU replacement.** Strong candidate for cheap C# implementation, repository work, docs, review and non-Unity tooling so expensive GPU minutes are reserved for Editor work. |

## Provider notes

### Akamai Cloud

Akamai's current European pricing page lists the RTX 4000 Ada x1 Small VM at $0.52/hour with 4 CPU, 16 GB RAM and 500 GB storage; the Medium tier is 8 CPU/32 GB at $0.67/hour. The European cloud footprint includes Amsterdam, Frankfurt, London, Paris, Stockholm and others, but the public regional-availability page does not prove that every GPU SKU is stocked in every listed region at every moment. Capacity must be checked at provisioning time.

Sources:
- https://www.akamai.com/cloud/pricing/europe
- https://www.akamai.com/cloud/pricing/gpu
- https://techdocs.akamai.com/cloud-computing/docs/how-to-choose-a-compute-instance-plan
- https://www.akamai.com/why-akamai/global-infrastructure/availability

### TensorDock

TensorDock is more relevant than a generic GPU-container marketplace because its product is virtual machines backed by GPU passthrough. Current pricing is à la carte: typical RTX A4000 GPU component pricing is shown at $0.10/hour, RTX 3090 $0.20/hour and RTX 4090 $0.35/hour, while CPU/RAM/storage are separate. The dashboard supports Ubuntu 22.04 with Nvidia drivers. Its own FAQ explicitly says storage is local and not replicated; this makes Git/offsite publication essential and means the machine should be treated as replaceable even if it is persistent in normal operation.

Sources:
- https://www.tensordock.com/cloud-gpus.html
- https://console.tensordock.com/
- https://console.tensordock.com/faq
- https://docs.tensordock.com/virtual-machines/how-to-ssh-into-your-instance
- https://docs.tensordock.com/virtual-machines/introduction-to-core-compute-vms

### AWS

The control candidate remains `g4dn.xlarge`: 4 vCPU, 16 GiB RAM and one 16-GB T4. Current Ireland on-demand pricing is $0.587/hour in current price trackers. Its strength is operational maturity and the graphics-workstation path: EC2 G4dn is explicitly positioned for remote graphics/game-streaming workloads, and AWS DCV provides a mature remote-display route. EBS-backed persistent project/cache storage can be retained while compute is stopped.

Sources:
- https://docs.aws.amazon.com/ec2/latest/instancetypes/ac.html
- https://aws.amazon.com/ec2/instance-types/accelerated-computing/
- https://costfluent.com/tools/instances/aws/g4dn.xlarge

### Paperspace

Paperspace Machines are persistent VMs; RTX4000 currently costs $0.56/hour and the machine type has 8 vCPU/30 GB RAM. AMS1 supports RTX4000. Compute billing stops on shutdown, but storage remains chargeable. The important CCF drawback is interactive Linux access: current documentation says desktop streaming is only available for Windows, while new Windows templates have been retired since July 2024. A Linux CCF workstation therefore needs an additional graphical streaming layer.

Sources:
- https://docs.digitalocean.com/products/paperspace/pricing/
- https://docs.digitalocean.com/products/paperspace/machines/details/machine-types/
- https://docs.digitalocean.com/products/paperspace/machines/details/availability/
- https://docs.digitalocean.com/products/paperspace/machines/how-to/connect/

### machine0

machine0 is compelling as an agent-native persistent VM system: Ubuntu/NixOS, static IPs, CLI/MCP management, profiles that inject MCPs/credentials/prompts, snapshots/suspend/resume, and Claude Code/Codex integration. Its entry GPU tier is 8 vCPU/32 GB RAM/500 GB with RTX 4000 Ada at $0.836/hour, so it does not fit the requested 16 GB first test and is more expensive than the primary candidates.

Source:
- https://machine0.io/

### Runpod / container GPU clouds

Runpod's raw price-performance is excellent: its current pricing lists RTX A5000 Secure Cloud around $0.27/hour with 25 GB system RAM and 24 GB VRAM. Runpod also now supports GPU-accelerated desktop-style Pod templates, making graphical use more realistic than a purely terminal-only service. However Pods are explicitly created from container images/templates, and persistent-volume behavior is tied to Pod/host lifecycle; a stopped Pod can also lose immediate access to its original GPU if that slot is rented by someone else. For CCF, this should be a deliberate experimental lane rather than the reference workstation because Unity's supported Editor configuration excludes container layers.

Sources:
- https://www.runpod.io/pricing
- https://docs.runpod.io/runpodctl/reference/runpodctl-remove-pods
- https://docs.runpod.io/pods/troubleshooting/zero-gpus

### boat.dev

boat is CPU-only, but remains attractive for the architecture around the Unity lane. Current published pricing is $0.072/hour for 8 vCPU/16 GB/125 GB and $0.20/hour for 16 vCPU/32 GB/251 GB. Sandboxes are full persistent Ubuntu VMs with SSH, a 60fps desktop, Docker, snapshots/forks and common agent/dev tools. A later CCF architecture can use boat for implementation/review/docs and send only candidate commits to the GPU Unity workstation for Editor qualification.

Sources:
- https://boat.dev/
- https://boat.dev/compare

## Linux graphical access layer

For providers without a good native Linux desktop protocol, a reasonable common experiment is **GNOME/X11 + Sunshine host + Moonlight client**. Sunshine currently supports Linux/Ubuntu 22.04+ and Nvidia NVENC-capable GPUs; Moonlight documents Sunshine as a Linux-capable host. This would make Akamai, TensorDock and Paperspace comparable at the remote-display layer. It still needs a real cloud-VM proof before being considered production CCF infrastructure.

Sources:
- https://docs.lizardbyte.dev/projects/sunshine/master/
- https://github.com/moonlight-stream/moonlight-docs/wiki/Setup-Guide

Parsec is not the Linux-host solution: Parsec's current documentation says Linux may connect as a client but cannot host.

Source:
- https://support.parsec.app/hc/en-us/articles/32381568346644-Hardware-and-Software-Compatibility

## Recommended experiment order

**Primary recommendation:** do not buy/provision several services at once. First compare two 16-GB full-VM candidates that answer different questions:

1. **Akamai RTX 4000 Ada Small** — strongest fixed-spec/value candidate if nearby European GPU capacity is available.
2. **AWS g4dn.xlarge Ireland** — operational-control baseline with the cleanest remote-workstation path.
3. **TensorDock 16-GB custom A4000-class VM** — test if a suitable high-quality EU host is available; potentially much cheaper, but marketplace/reliability risk is higher.

For each candidate, use the same CCF benchmark packet:

- exact Unity 6000.6.0f1 and project revision;
- same project disk/cache policy;
- one-worker cold/warm startup, compile, EditMode/PlayMode and representative rendered/runtime task;
- same task with two independent writable worktrees/Editor instances;
- record system RAM/swap, CPU saturation, GPU/VRAM, disk I/O, task wall time, Editor stability and remote-display usability;
- stop if two-worker contention creates persistent swapping, crashes/hangs, unacceptable interactive latency, or disproportionate wall-time regression.

Only after those measurements should 32 GB be considered. Paperspace RTX4000 and Akamai RTX 4000 Ada Medium are natural 30/32-GB follow-ups; machine0 is a higher-cost agent-native option.

## Current verdict

**Best exact 16-GB fixed-provider candidate:** Akamai RTX 4000 Ada Small, subject to nearby GPU capacity confirmation.  
**Best operational/control baseline:** AWS g4dn.xlarge Ireland.  
**Best potential low-cost full-VM value:** TensorDock, subject to EU host quality/availability and acceptance of non-replicated local storage.  
**Best complementary non-Unity agent platform:** boat.dev.  
**Best cheap experimental GPU lane:** Runpod, but not the reference Unity workstation because it is container-based.

No provider has been selected and no CCF workflow/Unity-lock policy is changed by this report.