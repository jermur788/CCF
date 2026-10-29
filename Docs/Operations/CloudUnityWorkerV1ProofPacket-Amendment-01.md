# Cloud Unity Worker V1 — Proof Packet Amendment 01

**Status:** BINDING AMENDMENT TO DRAFT PACKET; STILL NOT AUTHORISED FOR PROVISIONING OR SPEND  
**Date:** 2026-10-28  
**Applies to:** `Docs/Operations/CloudUnityWorkerV1ProofPacket.md` in PR #78  
**Manager review record:** PR #78 comment dated 2026-10-28  
**Task claim:** #79

This amendment corrects the five Manager-review findings on the published draft packet. Where this amendment conflicts with the original packet, **this amendment controls**. Nothing here authorises provisioning, provider spend, Unity licence activation, Windows builds, production CCF edits, or a change to the project-wide Unity execution policy.

## A1. Unity execution policy — replace P4 concurrency semantics

The original packet's P4 wording that permits simultaneous heavy Unity workloads is superseded.

The cloud VM may host **two isolated resident Unity Editors/checkouts at the same time** for the purpose of measuring memory, VRAM, CPU and interactive-residency overhead. However, **heavy Unity execution remains serialized on that VM** under the existing machine-wide Unity ownership rule. Heavy execution includes cold/warm import, compilation/test batches, runtime verification gates, scene/runtime measurement passes and any other phase that materially exercises Unity as an execution workload.

### Revised P4 — two-editor residency and serialized contention proof

Use two isolated benchmark checkouts, each with its own `Library/`, `Temp/`, `Logs/`, `obj/`, HOME/XDG state and save state.

1. Establish the one-worker warm baseline from P3.
2. Start Editor A and allow it to become idle after import/compile. Record resident RAM, VRAM, CPU and disk state.
3. Start Editor B in the second isolated checkout and allow it to become idle. Record the incremental resident RAM/VRAM/CPU cost and any swap or resource pressure caused by dual residency.
4. With both Editors resident, acquire/retain the cloud-VM Unity execution ownership lock for **one heavy phase at a time**. Run the representative heavy gate in Editor A while Editor B remains idle; record timing/resource impact versus P3.
5. Return Editor A to idle, then run the same heavy gate in Editor B while Editor A remains idle; record timing/resource impact versus P3.
6. Do not start overlapping import/test/runtime phases. If a future experiment needs true simultaneous-heavy execution, stop and obtain a separate Manager/workflow-policy decision and packet.
7. Record crashes, hangs, swap, OOM, responsiveness and resource contention from the second resident Editor even though heavy phases are serialized.

The acceptance question is therefore: **can one 30–32 GB GPU VM keep two isolated Editors resident while preserving acceptable serialized heavy-run performance and interactive usability?** It is not a proof that two heavy Unity jobs may execute concurrently.

Any original reference to “simultaneous two-worker gate execution”, “start both gates together”, or equivalent is superseded by this section.

## A2. Mandatory task claim, live coordination refresh and cloud-host ownership lock

Before **any provisioning action that can incur cost** or **any Unity execution on the cloud VM**, the executor must:

1. have a dedicated open GitHub task-claim issue for the proof execution;
2. refresh current `main`, the claim, and live `Docs/Project/coordination/active-tasks.md`;
3. record executor/model/provider, exact repository revision, benchmark candidate SHA, cloud host identity, checkout paths and execution state on the task claim;
4. create/update a live coordination row identifying the cloud VM as an owned execution resource and naming the current owner;
5. verify that no conflicting cloud-host owner or machine-wide Unity execution owner exists before starting a heavy Unity phase;
6. mark the claim `[RUNNING]` before Unity execution and `[BLOCKED]` if ownership or policy cannot be established.

`active-tasks.md` remains live ownership authority. The task claim tracks execution presence only and does not override live locks.

This amendment therefore supersedes the original “out of scope: changing `active-tasks.md`, lanes, branches or locks” language **only to the extent necessary for execution coordination and resource ownership**. Product/gameplay ownership remains unchanged.

## A3. Canonical result publication — GitHub handoff required

The local evidence directory remains staging only:

`/media/jer/Files/CCF/Verification/CloudUnityWorkerV1-<UTC>/`

A local `HANDOFF.md` there is **not sufficient completion evidence**. Before the proof can be called handed off, the executor must publish a substantive Markdown handoff to GitHub under a canonical path such as:

`Docs/Handoffs/CloudUnityWorkerV1-Proof-<UTC>/HANDOFF.md`

That GitHub handoff must include:

- task claim and executor/model/provider;
- exact CCF SHA and checkout identities;
- selected host identity/location/shape and dated quote;
- authorised spend cap and actual provider spend;
- Unity version and licence-activation result without secret material;
- network/access configuration summary;
- one-worker cold/warm results;
- two-editor residency measurements and serialized heavy-run comparison;
- remote graphical test results;
- anomalies/stop conditions;
- teardown state;
- local evidence staging path and any durable uploaded evidence references;
- recommendation and remaining unknowns;
- explicit statement that no product acceptance/integration is claimed.

Local logs/screenshots may be referenced from the GitHub handoff, but the durable conclusion must exist in Git.

## A4. Benchmark checkouts are detached/scratch and non-production

The benchmark VM receives **no production write authority**.

Preferred layout:

- one clone pinned to the exact benchmark candidate SHA;
- two detached benchmark worktrees/checkouts at that exact SHA, or explicitly named scratch trial branches if a branch is technically required by tooling;
- separate `Library/` and state directories for each checkout.

Rules:

- do not use a moving production branch as a benchmark target;
- do not make gameplay/product changes in either benchmark checkout;
- do not merge, rebase or force-push production branches from the cloud VM;
- disposable harness material must follow the existing copy/run/remove pattern and must not grant production authorship;
- if an unexpected source change occurs, stop the benchmark, preserve the diff as evidence, and return to the Manager rather than treating the VM as an implementation lane.

The original example `trial/cloud-worker-1` / `trial/cloud-worker-2` branch names are therefore optional scratch identities only, not writable production lanes.

## A5. Secure network/access baseline

Before exposing SSH or graphical access, configure a minimum secure baseline:

1. **default-deny inbound** at provider firewall/security-group level and host firewall where available;
2. **SSH key authentication only** for project access; password-only SSH is prohibited;
3. root remote login disabled unless an unavoidable provider bootstrap step requires it, after which it must be disabled before benchmarking;
4. no Unity Editor, Unity licensing service, MCP server, agent control plane, Sunshine web/control surface, VNC/RDP service or similar control endpoint exposed directly to the public Internet;
5. graphical/control access must use a private overlay network, VPN/tunnel, SSH tunnel, or equivalently constrained authenticated path;
6. Sunshine/Moonlight traffic must be restricted to the approved private/tunnel path rather than open Internet listeners;
7. Git/TensorDock/Unity credentials remain outside tracked files and benchmark logs;
8. record the access method and firewall posture in the final GitHub handoff without publishing secret keys, tokens or licence data.

If the selected provider cannot satisfy this baseline without materially broad public exposure, stop and return to the Manager before Unity execution.

## A6. Revised acceptance criteria

The proof is complete only when all original non-conflicting acceptance criteria **and** the following are satisfied:

1. execution had an identifiable GitHub task claim and live cloud-host ownership row;
2. heavy Unity execution was serialized despite dual Editor residency;
3. dual-residency resource overhead and serialized heavy-run performance were measured separately;
4. benchmark checkouts remained detached/scratch and non-production;
5. secure network baseline was in place before interactive/control exposure;
6. the substantive result was published as a GitHub Markdown handoff;
7. provider spend stayed within an explicitly user-authorised cap;
8. no Windows build or CCF production integration was performed.

## A7. Execution authority remains closed

This amendment fixes the packet design only. The packet remains **DRAFT — NOT AUTHORISED FOR PROVISIONING OR SPEND** until the user explicitly resolves the five pre-provisioning decisions already listed in the original packet, including the exact live host/quote and maximum spend.