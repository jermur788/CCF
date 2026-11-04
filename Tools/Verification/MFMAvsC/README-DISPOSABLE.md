# MFMAvsC — DISPOSABLE / EXPERIMENTAL / OFFLINE (#271)

Research tooling for `Docs/Handoffs/Overnight-Continuation-20261104/MFM-A-VS-C-COMPARISON-HANDOFF.md`.

- **Not** a test, **not** a gate, **not** production code, **not** a policy default. It grants no approval.
- Stdlib-only Python port of C# at `72b4efbd`; no C# compiled, no Unity.
- Never copy under `Assets/`. Delete this directory once the Manager has consumed the handoff.
- Reads raw fixtures from `/media/jer/Files/CCF/Verification/` (read-only). The script aborts if their sha256 differs from #251 Appendix A.

Run (the `-I -B` matters on lane worktrees: no `__pycache__`):

    python3 -I -B mfm_a_vs_c_offline.py <outdir>
    ./rederive_from_raw.sh

Committed raw outputs: `output/results.json`, `output/run.log`, `output/rederivation.log`.
