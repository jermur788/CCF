# HANDOFF — Cloud Unity Worker V1 Proof Packet Publication — 2026-10-28

**Status:** PUBLICATION REQUEST — BLOCKED ON LOCAL DRAFT ACCESS  
**Task claim:** #77  
**Target agent/model/provider:** TTree Code or equivalent local filesystem-capable coding lane; no Unity execution required  
**Target source draft:** local uncommitted `Docs/Operations/CloudUnityWorkerV1ProofPacket.md` in the `ops/workflow-foundation-v1` worktree  
**Canonical research:** `Docs/Research/CloudUnityDevelopmentOptions-20261028.md` and `Docs/Handoffs/CloudUnityDevelopmentResearch-20261028/HANDOFF.md` on current `main`

## Purpose

Promote the locally drafted Cloud Unity Worker V1 proof packet into GitHub-visible canonical project history without committing it from a stale worktree or losing unrelated dirty work.

The user reports that the draft is approximately 275 lines and targets TensorDock at roughly 30–32 GB RAM / 8 vCPU / RTX A4000 / >=250 GB NVMe / <= $0.35 per hour, with live EU-host gating, benchmark phases P0–P6, a measurement schema, stop conditions, and a destroy/retain cost policy. This report is evidence only until the local file is inspected and published.

## Preflight

1. Refresh live `Docs/Project/coordination/active-tasks.md` before claiming ownership.
2. Inspect the exact local worktree containing the uncommitted draft. Preserve every unrelated dirty/untracked file; do not stash, clean, discard or absorb unrelated work.
3. Verify the local draft path and current bytes.
4. Verify current `main` and note that the GitHub branch `ops/workflow-foundation-v1` is stale relative to current `main`; do **not** simply commit/push the draft from that stale branch.
5. Create/use a fresh dedicated publication branch/worktree from current `main` and copy only the intended proof-packet content into it.
6. Reconcile the packet against the current cloud-Unity research on `main`, especially the later TensorDock-vs-Paperspace findings. Do not silently rewrite user decisions or turn recommendations into authorised spend.

## Required packet state before publication

The published packet must remain an operational proof packet, not an authorisation to provision infrastructure. It must clearly preserve these gates:

- exact live TensorDock EU host/quote must be recorded before provisioning;
- maximum spend must be explicitly authorised;
- Git credential method must be chosen;
- Unity activation/licence method must be chosen as an execution prerequisite, without reopening the policy/legal discussion unless requested;
- post-proof destroy-vs-retain policy must be chosen;
- no change to the current CCF machine-wide Unity-lock policy occurs merely because the packet exists;
- no Windows build is authorised;
- Git remains canonical and TensorDock local storage is disposable/non-authoritative.

## Recommended defaults for Manager review

These are recommendations only, not accepted decisions:

- **Host gate:** EU, preferably Core Compute; otherwise Top Host + strong location tier; RTX A4000 or better; 8+ vCPU; 30–32 GB RAM; >=250 GB NVMe; complete quote <= $0.35/h.
- **Proof spend cap:** $15 total provider spend for the initial one-worker + two-worker proof, with an immediate stop if the cap would be exceeded before useful evidence is obtained.
- **Git credentials:** dedicated fine-grained GitHub credential or dedicated SSH key scoped to the CCF repository, stored only on the proof VM and revoked/removed when the VM is destroyed. Do not copy a broad personal credential bundle.
- **Unity activation:** use the ordinary user-authorised Unity activation/sign-in flow required to launch the exact Unity 6000.6.0f1 Editor on the proof VM; packet execution must stop if activation cannot be completed cleanly.
- **Destroy/retain:** default to destroy after evidence, pushes and handoff are verified; retain only if the user explicitly decides the worker will immediately continue into production use. A retained TensorDock disk is convenience/cache, never canonical state.

## Exit criteria

Publish a GitHub-visible handoff containing:

- exact source path and source worktree inspected;
- publication branch and starting `main` HEAD;
- published packet commit SHA;
- reconciliation notes versus the current research;
- confirmation that unrelated dirty work was untouched;
- whether the five execution decisions remain open or were explicitly resolved by the user;
- explicit statement that no provider was provisioned and no spend occurred during publication.

After publication, mark task claim #77 `[HANDED-OFF]`, link the handoff, and close the issue.

## Stop conditions

Stop and report `[BLOCKED]` if the local draft cannot be inspected, ownership conflicts exist, its content materially conflicts with current project authority/research, or publishing it would require modifying/cleaning unrelated local work.
