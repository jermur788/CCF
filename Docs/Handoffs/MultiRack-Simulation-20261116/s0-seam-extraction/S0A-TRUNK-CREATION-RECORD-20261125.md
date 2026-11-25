# CCF Multi 2D — S0a Trunk Creation Record (`integration/2d-sim`)

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (§4.1, §9 S0-01)
- **Date:** 2026-11-25
- **Recorded by:** S0a Primary Implementer (Gemini 4 Neon (XHigh)), relaying Coordinator-executed T-00 creation on owner authority
- **Status:** Trunk created cleanly at exact `T0`; S0a branch `feat/multi-2d-s0a-mechanical-extraction-20261125-v1` is **NOT REVIEWED, NOT INTEGRATED**

## 1. Baseline & Trunk Creation Evidence (T-00)

- **Trunk branch:** `integration/2d-sim` (`refs/heads/integration/2d-sim`)
- **Owner-accepted consolidated Phase 1 baseline (`T0`):** `47b27a8d3873b32726f315ce1182118da38c50b1` (tip of owner-accepted Phase 1 Step 2 Repair #2)
- **Creation command (executed by Coordinator on owner approval, 2026-11-25):**

```bash
git push origin 47b27a8d3873b32726f315ce1182118da38c50b1:refs/heads/integration/2d-sim
```

- **Verification output (`git ls-remote origin refs/heads/integration/2d-sim`):**

```text
47b27a8d3873b32726f315ce1182118da38c50b1	refs/heads/integration/2d-sim
```

- **Worktree verification at S0a start:**
  - `git rev-parse origin/integration/2d-sim` == `47b27a8d3873b32726f315ce1182118da38c50b1`
  - `git merge-base --is-ancestor 47b27a8d3873b32726f315ce1182118da38c50b1 HEAD` == `0`
  - No commits were added to `integration/2d-sim` at creation.

## 2. Locked Trunk Policy (Spec §4.1 A-1..A-5)

1. **A-1 (Exact baseline):** `integration/2d-sim` points strictly to `T0 = 47b27a8d3873b32726f315ce1182118da38c50b1`.
2. **A-2 (`main` 3D-only isolation):** `origin/main` remains strictly 3D-only; no commit from `integration/2d-sim` or any 2D feature branch is merged to `main`.
3. **A-3 (Single-writer feature branches):** Every 2D packet forks from the current `integration/2d-sim` tip under single-writer ownership.
4. **A-4 (Single-integrator `--no-ff` merges):** Only an appointed integrator rebases onto `integration/2d-sim`, verifies `2d-gate.sh` and an independent non-author review of the exact gated HEAD, and merges with `git merge --no-ff` one branch at a time.
5. **A-5 (Freeze-on-red):** Any non-zero gate result on `integration/2d-sim` freezes the trunk until a reviewed repair or `git revert -m 1` returns the trunk tip to exit `0`. History is never rewritten or force-pushed.
