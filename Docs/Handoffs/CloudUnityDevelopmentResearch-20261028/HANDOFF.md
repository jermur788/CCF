# HANDOFF — Cloud Unity Development Infrastructure Research — 2026-10-28

**Status:** RESEARCH COMPLETE — MANAGER REVIEWED IN CHAT; NO INFRASTRUCTURE PROVISIONED  
**Research document:** `Docs/Research/CloudUnityDevelopmentOptions-20261028.md`  
**Research publication commit:** `af02d53b00919b74fb34b9b40b7cc83def73c2d8`  
**Authority:** Supporting infrastructure research only. It does not alter Unity execution locks, current milestone scope, product behaviour, licensing policy, or implementation authority.

## User-selected test direction

The intended proof is to start at **16 GB system RAM**, measure **one CCF Unity worker**, then test **two concurrent independent CCF Unity workers** on the same machine before deciding whether 32 GB is needed. Licensing is deliberately outside this research scope per the user's instruction.

## Research verdict

The deeper provider comparison changes the initial shortlist:

1. **Akamai Cloud RTX 4000 Ada x1 Small** — strongest fixed-spec exact-16-GB candidate: 4 CPU, 16 GB RAM, RTX 4000 Ada, 500 GB storage, Europe pricing $0.52/hour; GPU availability in the desired nearby European region must be confirmed at provisioning time.
2. **AWS EC2 g4dn.xlarge, eu-west-1 Ireland** — control/baseline candidate: 4 vCPU, 16 GiB, T4 16 GB; current Ireland on-demand price $0.587/hour; strongest mature Linux remote-workstation route via AWS DCV/EBS.
3. **TensorDock custom 16-GB GPU VM** — potentially substantially cheaper full-VM option; exact CPU/RAM/storage are configurable and A4000-class GPUs are inexpensive, but it is a host marketplace and TensorDock explicitly provides no local-storage redundancy. A high-quality nearby EU host must be selected and Git/offsite publication treated as mandatory.

**Paperspace** remains a good ~30-GB fallback, but does not provide the requested exact 16-GB GPU test and requires a third-party Linux graphical streaming layer. **machine0** is excellent for agent-native CLI/MCP orchestration but starts at 32 GB/$0.836/hour for GPU. **Runpod/Vast** are cheap experimental GPU options, but their container architecture is outside Unity's published supported Editor form factor and is therefore not the preferred reference lane. **boat.dev** remains useful as a cheap CPU-only implementation/review/agent pool feeding candidate commits to the Unity machine.

## Suggested Linux display standard

Where the provider lacks a strong native Linux graphics protocol, test GNOME/X11 + Sunshine on the VM and Moonlight on the user's laptop. Sunshine supports Ubuntu 22.04+ and Nvidia hardware encoding. AWS DCV remains the control implementation for AWS.

## Next executable step

No purchase or provisioning is authorised by this handoff. When the user chooses to proceed, create a bounded **Cloud Unity Worker V1 proof packet** for one 16-GB provider. The packet should specify:

- exact provider/region/machine SKU;
- exact CCF candidate HEAD and Unity 6000.6.0f1;
- persistent disk/cache layout;
- Git branch/worktree ownership;
- agent/MCP setup;
- remote graphical-access method;
- one-worker and two-worker benchmark phases;
- RAM/swap/CPU/GPU/VRAM/disk/wall-time measurements;
- stop conditions for instability or unacceptable contention;
- teardown/stop policy so idle GPU compute does not continue billing.

Do not change the existing machine-wide Unity-lock policy until the benchmark evidence has been reviewed and a separate management decision explicitly changes it.

## User action required

None for the research result. The next user decision is which 16-GB provider to trial first; based on current evidence, compare **Akamai** against the **AWS Ireland control**, with **TensorDock** as the low-cost marketplace challenger.