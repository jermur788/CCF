# TensorDock vs Paperspace for CCF Cloud Unity — 2026-10-28

**Status:** SUPPORTING INFRASTRUCTURE RESEARCH — NOT DECISION AUTHORITY  
**Scope:** Compare TensorDock with the current Paperspace RTX4000 candidate for CCF Unity development. Licensing remains outside scope by user instruction. No infrastructure is provisioned by this document.

## User clarification

The earlier 16 GB-first proposal was not itself a product requirement. It was a consequence of the first provider comparison. The relevant objective is now **best useful Unity capacity for the money**, followed by an empirical one-worker/two-worker CCF test. There is no reason to prefer 16 GB if a 30–32 GB machine is available at similar cost.

## Paperspace reference candidate

Paperspace RTX4000 in AMS1 is a fixed machine type:

- NVIDIA Quadro RTX 4000 (Turing), 8 GB VRAM;
- 8 vCPU;
- 30 GB system RAM;
- Intel Xeon Silver 4215R host class, 3.2 GHz base / 4.0 GHz max turbo for RTX4000 machines;
- 50 GB SSD by default, resizable to 2 TB;
- $0.56/hour compute;
- AMS1 (Amsterdam) officially supports RTX4000;
- compute billing stops when the machine is powered off; storage continues to bill;
- Linux is supported, but Paperspace's integrated desktop streaming is not the Linux path, so CCF would still add its own graphical streaming layer.

Primary sources:
- https://docs.digitalocean.com/products/paperspace/pricing/
- https://docs.digitalocean.com/products/paperspace/machines/details/machine-types/
- https://docs.digitalocean.com/products/paperspace/machines/details/availability/

## TensorDock architecture

TensorDock is a closer Unity fit than container GPU services because it provides **KVM/Libvirt virtual machines with dedicated GPU passthrough, root access and a full operating system**. Ubuntu 22.04 LTS is available with Nvidia drivers preinstalled; the newer API documentation also shows Ubuntu 24.04 as an instance-creation option.

TensorDock is a marketplace. GPU price, CPU type, location and host quality vary. The dashboard supports selecting a specific host node or a location-based deployment. Public documentation states more than 100 locations in more than 20 countries and explicitly references Germany, but the exact nearby-EU GPU inventory and quote must be checked live when signed in.

Primary sources:
- https://www.tensordock.com/
- https://www.tensordock.com/cloud-gpus.html
- https://console.tensordock.com/deploy
- https://dashboard.tensordock.com/api/docs/instance-creation

## TensorDock pricing model

TensorDock prices GPU, CPU, RAM and NVMe storage independently. Published typical component pricing (actual host quote varies) is:

- RTX A4000 16 GB GPU: typical $0.10/hour on the public pricing page; currently surfaced dashboard snapshots have ranged from roughly $0.08 to $0.22/hour depending on host/availability;
- vCPU: $0.003/hour each;
- RAM: $0.002/hour per GB;
- block NVMe storage: $0.00005/hour per GB;
- minimum 1 Gbps networking included.

An illustrative CCF shape matching Paperspace's CPU/RAM while materially exceeding its VRAM is:

- RTX A4000 16 GB VRAM;
- 8 vCPU;
- 30 GB system RAM;
- 250 GB NVMe.

At the published **typical** A4000 component price:

- GPU: $0.1000/h
- CPU: 8 × $0.003 = $0.0240/h
- RAM: 30 × $0.002 = $0.0600/h
- storage: 250 × $0.00005 = $0.0125/h
- illustrative running total: **$0.1965/hour**.

At a more expensive currently surfaced A4000 component price of $0.22/hour, the same complete shape is approximately **$0.3165/hour**.

These are **illustrative calculations, not a TensorDock quote**. The actual desired European host may charge different component prices and may not have this exact combination available.

Stopped TensorDock VMs retain storage and bill the storage portion only. At the published storage rate, 250 GB is about $0.0125/hour or about $9.13 for a 730-hour month. For 40 powered-on hours in a month, the illustrative complete monthly cost is roughly:

- about **$16.49/month** at the typical $0.10 A4000 GPU component price;
- about **$21.29/month** at a $0.22 A4000 GPU component price.

By comparison, Paperspace RTX4000 compute for 40 hours is $22.40; a 250 GB Paperspace disk has a maximum monthly charge around $10, yielding roughly **$32.40/month** at that usage level.

Primary sources:
- https://www.tensordock.com/cloud-gpus.html
- https://beta.dashboard.tensordock.com/
- https://docs.tensordock.com/virtual-machines/spot-instances
- https://docs.digitalocean.com/products/paperspace/pricing/

## Hardware comparison relevant to Unity

| Characteristic | TensorDock illustrative A4000 VM | Paperspace RTX4000 AMS1 |
|---|---|---|
| Virtualization | Full KVM/Libvirt VM, GPU passthrough | Persistent GPU VM |
| GPU | RTX A4000, Ampere, 16 GB VRAM (illustrative target) | Quadro RTX 4000, Turing, 8 GB VRAM |
| System RAM | Configurable; compare at 30 GB | Fixed 30 GB |
| vCPU | Configurable; compare at 8 | Fixed 8 |
| CPU model | Host-dependent | Xeon Silver 4215R class for RTX4000 |
| Disk | Configurable NVMe, host-local unless Core Compute/network storage is selected | Persistent disk; 50 GB default, resizable |
| Location | Host-marketplace dependent; nearby EU availability must be checked live | AMS1 fixed and officially supported |
| Running cost | Potentially ~$0.20–$0.32/h for the illustrative shape, but host-variable | $0.56/h fixed |
| Stopped cost | Storage only | Storage/add-ons only |
| Bandwidth | Minimum 1 Gbps advertised; no ingress/egress fees claimed | Free unlimited bandwidth for Machines |

For CCF, the stronger TensorDock A4000 GPU is useful but probably not the main reason to choose it. **CPU quality, RAM and storage latency are likely to dominate Unity import/compile/test throughput more often than raw GPU compute.** Paperspace has an advantage here because its RTX4000 CPU class is known in advance; TensorDock's CPU is host-dependent and therefore must be inspected before purchase.

## Reliability and persistence — TensorDock's main disadvantage

TensorDock advertises a 99.99% host uptime standard and requires planned maintenance notice. It also grades marketplace locations/hosts: a **Top Host** reflects operational track record, while a **Location Tier** reflects infrastructure quality such as power redundancy.

However, its ordinary distributed-compute FAQ explicitly states that **local storage is not replicated**. Hardware corruption can therefore lose the VM's local data. For CCF this is survivable only if:

- Git remains canonical;
- task work is pushed frequently;
- no valuable handoff/evidence exists only on the VM;
- optional offsite backup/checkpointing is used for expensive-to-recreate caches or artifacts if warranted.

TensorDock's newer **Core Compute** is more attractive where available: it uses more consistently reliable providers, adds network storage/static IP functionality, and can allow a stopped VM to become deployable on another server in the same location with the required GPU rather than being tied to one physical server. The exact EU Core Compute availability must be checked live.

Primary sources:
- https://console.tensordock.com/faq
- https://docs.tensordock.com/virtual-machines/introduction-to-core-compute-vms
- https://www.tensordock.com/cloud-gpus.html

## Remote graphical Unity access

Neither TensorDock nor the Linux Paperspace candidate gives CCF a decisive native Linux interactive-desktop advantage. TensorDock exposes SSH/port forwarding and supports Windows RDP, but for the intended Linux CCF workstation we should use the same graphical layer on both providers (for example GNOME/X11 + Sunshine host + Moonlight client) so the provider benchmark is not confounded by different streaming stacks.

## CCF recommendation after TensorDock review

**TensorDock should move ahead of Paperspace as the first provider to inspect for a real trial, provided the live dashboard shows a suitable nearby European host.** The preferred live host criteria are:

1. nearby Europe (Germany/Netherlands/UK/France or similarly low-latency location);
2. Core Compute if available, otherwise a Top Host with a high Location Tier;
3. RTX A4000-class or better GPU;
4. at least 8 vCPU with a modern/high-clock host CPU;
5. about 30–32 GB RAM;
6. at least 250 GB NVMe;
7. total running quote materially below Paperspace's $0.56/hour; a practical target is **≤ $0.35/hour** for the complete machine;
8. stopped storage cost acceptable;
9. sufficient port/network options for SSH, agent/MCP control and the chosen graphical streaming layer.

If a high-quality EU TensorDock host satisfies those conditions, it is the stronger first CCF trial because it gives more GPU headroom and potentially cuts running cost by roughly 40–65% relative to Paperspace. If only low-tier/distant/poorly specified hosts are available, use Paperspace AMS1 instead for predictable hardware and location.

## Trial design

Whichever provider is chosen, the trial remains:

1. one isolated CCF Unity worktree/Editor/agent — baseline;
2. two isolated CCF Unity worktrees/Editors/agents concurrently;
3. measure RAM/swap, CPU saturation, GPU/VRAM, disk I/O, compile/import/test/runtime wall time and remote-display usability;
4. decide whether one VM can safely become a two-Unity-worker resource.

No CCF Unity-lock policy changes until those measurements have been reviewed by the Manager.
