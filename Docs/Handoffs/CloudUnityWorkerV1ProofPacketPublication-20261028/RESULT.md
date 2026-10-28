# RESULT — Cloud Unity Worker V1 Proof Packet Publication — 2026-10-28

**Status:** PUBLISHED TO PUBLICATION BRANCH; TASK CLAIM #77 HANDED OFF  
**Request:** `Docs/Handoffs/CloudUnityWorkerV1ProofPacketPublication-20261028/HANDOFF.md` on `main` at `acb068d`  
**Task claim:** #77

## Source inspected

- **Source worktree:** `/media/jer/ZX20/CCF-Primary`
- **Source branch:** `ops/workflow-foundation-v1`
- **Source draft:** `Docs/Operations/CloudUnityWorkerV1ProofPacket.md` (untracked local draft)
- **Source draft SHA-256:** `595a4538c4e14a426b17be5f17efa1b82c619bb554957a4cecf4a45640dd3271`
- **Source draft size:** 275 lines

The source draft was inspected byte-for-byte before copying. Nothing was copied from the stale `ops/workflow-foundation-v1` branch history.

## Publication

- **Starting `main` HEAD:** `acb068d66d7b095a28d629651ee0bbbe06a132ac`
- **Publication worktree:** `/media/jer/ZX20/CCF-CloudPacketPub`
- **Publication branch:** `docs/cloud-unity-worker-v1-proof-packet`
- **Published packet path:** `Docs/Operations/CloudUnityWorkerV1ProofPacket.md`
- **Published packet commit:** `4f9d113ec80bd0069c6181f9903130a46e61550f`
- **Published packet SHA-256:** `8aad79b93a6df89e1b35b9b5f83e38d7fa7a881e51f660d1766681bb52261c22`
- **Published packet size:** 283 lines

`Docs/Operations/` did not exist on `main`; it was created by this publication with only the proof packet.

## Reconciliation against current research and authority

- Current `main` research (`Docs/Research/TensorDockPaperspaceComparison-20261028.md` and the revised research handoff) supersedes the 16 GB-first direction. The packet targets TensorDock at **30–32 GB RAM**, 8 vCPU, RTX A4000 or better, ≥250 GB NVMe, ≤ $0.35/hour, with a live EU-host gate.
- The packet's **Benchmark base** and **Candidate HEAD** remain `30fba2c`; the publication base is recorded separately as `acb068d`. No moving branch may be benchmarked.
- Context commit `444a116` was re-verified as the latest canonical-context commit on `main`; later commits were coordination, research, handoff or packet-publication only.
- The packet preserves every required gate: live host/quote, explicit maximum spend, Git credential method, Unity activation prerequisite, destroy-vs-retain, unchanged machine-wide Unity-lock policy, no Windows build, Git canonical and TensorDock local storage disposable.
- The request's recommended defaults were added only as **proposed, not authorised** decisions: $15 total proposed cap, dedicated scoped Git credential, ordinary Unity activation, destroy after verified evidence.
- No recommendation was converted into authorised spend or scope.

## Unrelated work

The source worktree `/media/jer/ZX20/CCF-Primary` remains dirty exactly as found, with these files untouched:

- modified: `AGENTS.md`
- modified: `Docs/Project/coordination/active-tasks.md`
- untracked: `Docs/Operations/CloudUnityWorkerV1ProofPacket.md` (source draft)
- untracked: `Docs/Operations/StorageArchitecture.md`
- untracked: `Docs/ScenarioTwoPreProductionAudit.md`

No stash, clean, reset, checkout, commit or deletion was applied to the source worktree. The publication worktree contains only the published packet and this result.

## Decisions still open

All five execution decisions remain open:

1. exact live TensorDock EU host and quote;
2. maximum authorised spend;
3. Git credential method;
4. Unity licence activation method;
5. destroy-vs-retain after the trial.

The packet remains **DRAFT — NOT AUTHORISED FOR PROVISIONING OR SPEND**.

## No provisioning or spend

No TensorDock account action, host reservation, VM provisioning, Unity licence activation or provider spend occurred during this publication. No Unity execution occurred. No Windows build was produced. No lane, lock or `active-tasks.md` entry was changed.
