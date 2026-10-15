# Tier-1 dummy workflow

This packet is prepared and its chain is simulated by tests/self-test.py using a separate scratch board/ledger, never the live board. No worker thread/Unity/game change is needed. Base/context pin the supplied context; the simulated worker handoff records the exact executing lane HEAD.

Sequence: manager packet → packet-lint PASS → lane-status → scratch board ACTIVE → write simulated worker handoff → scratch board VERIFY → mock verification against the packet → compact ledger entry (Verdict: SIMULATED PASS; Integrated?: Simulated; Canonical impact: None) → remove row only after ledger exists. All intermediate board snapshots and handoff/ledger remain under ZX20/CCF-Temp and a compact receipt is retained in CCF-Verification/WorkflowFoundation. This proves mechanics, not independent review or production acceptance.
