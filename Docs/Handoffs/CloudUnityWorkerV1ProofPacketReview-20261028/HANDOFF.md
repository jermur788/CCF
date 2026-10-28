# HANDOFF — Cloud Unity Worker V1 Proof Packet Review — 2026-10-28

**Status:** NEEDS CHANGES — PR #78 MUST NOT MERGE YET  
**Task claim:** #79  
**Target agent/model/provider:** TTree Code or equivalent GitHub/local coding lane  
**Target branch:** `docs/cloud-unity-worker-v1-proof-packet`  
**Target PR:** #78  
**Scope:** docs-only correction; no provisioning, spend, Unity execution, gameplay change or global workflow-policy change

## Manager review decision

PR #78 is structurally sound and contains only the intended proof packet plus publication result, but the packet is not executable as written because it conflicts with current CCF Unity coordination policy and omits required execution handoff/claim mechanics.

## Required corrections

1. **P4 concurrency:** keep two independent Editors/checkouts resident concurrently if desired, but all heavy Unity import/test/runtime execution on the proof VM must remain serialized under the current machine-wide Unity policy. Measure the second resident Editor's RAM/VRAM/CPU/disk impact and compare serialized heavy runs with one-editor baselines. Do not claim this proves true simultaneous-heavy throughput. Any future simultaneous-heavy benchmark needs a separate explicit workflow-policy decision outside this packet.
2. **Task claim and live coordination:** before provisioning or Unity execution, the proof worker must refresh `Docs/Project/coordination/active-tasks.md`, create/use the execution task claim, and record the cloud host as an owned Unity resource/lock for the proof. The packet may not declare `active-tasks.md` entirely out of scope for execution.
3. **Canonical exit artifact:** substantive proof results must be published as a GitHub Markdown handoff. A local `/media/jer/Files/CCF/Verification/...` path may be used for raw/staged evidence but is not sufficient as the final handoff.
4. **Benchmark checkout authority:** use detached/non-production benchmark checkouts or explicitly scratch trial branches with no production write authority. Preserve separate `Library/`, user state and save state for each Editor.
5. **Network baseline:** default-deny inbound; SSH key authentication only; do not expose Unity, MCP or Sunshine control surfaces directly to the public Internet. Use a private overlay/tunnel or equivalently constrained access and record the access method.

## Preserve

Do not weaken the live TensorDock EU-host gate, <= $0.35/hour complete-quote target, 30–32 GB / 8 vCPU / RTX A4000-or-better / >=250 GB target, $15 proposed-but-unauthorised planning cap, measurement schema, stop conditions, Git-canonicity rule, disposable TensorDock local storage rule, no-Windows-build rule, or five pre-provisioning user decisions.

## Completion

Update the packet on the existing PR branch, update the publication result if needed so it accurately describes the corrected packet, then mark PR #78 ready for review. Publish a GitHub-visible correction result/handoff, mark claim #79 `[HANDED-OFF]`, link the result and close the claim. Do not provision TensorDock as part of this correction.
