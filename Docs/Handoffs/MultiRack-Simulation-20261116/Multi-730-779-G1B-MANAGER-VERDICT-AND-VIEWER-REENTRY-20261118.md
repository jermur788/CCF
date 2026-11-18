# Multi #730 — Manager verdict on #779 enforced-agent boundary

18 November 2026. **APPROVED as substantive admission-remediation evidence; G1b COMPLETE SECURITY ACCEPTANCE NOT GRANTED; G2 SOURCE HELD.** This concerns standalone 2D Multi Year0 only, not #770 or Scenario One.

## Inspected evidence

- [Complete #779 DeepSeek result](https://github.com/jermur788/CCF/blob/docs/multi-730-779-deepseek-g1b-confinement-result-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-779-ENFORCED-AGENT-TOOL-CONFINEMENT-RESULT-20261118.md), including recorded four GPT-6.1 Sol witness rounds and frozen launcher/policy checks.
- [#776 original source-admission stopped result](https://github.com/jermur788/CCF/blob/docs/multi-730-h0a-gated-end-to-end-orchestration-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-H0A-END-TO-END-GATED-YEAR0-2D-VIEWER-ORCHESTRATION-RESULT-20261118.md).
- The GitHub-only Manager can read the exact published report, not independently run or inspect native evidence files/active process/physical Ops directly. Reported witness evidence is not the same as a new Manager native qualification.

## What passed

Compared with #776, whole-agent mount namespace plus actual tool-gateway restrictions are a meaningful confinement improvement, not voluntary per-command sandboxing. Recorded final witness checks show out-of-lane file reads/writes denied; shell/webfetch/websearch/Drive/subagent unavailable; only lane files and private model runtime mounted; credentials for GitHub/SSH not mounted; read-only policy; stdin EOF; and every tested inherited descriptor (including 65536, 70000, 200000) closed. Independent witnesses caught multiple real defects and their corrections. No production code or protected/fixture bytes were touched.

## Why G2 is still held

The result itself records its final verdict as **CONFINEMENT NOT PROVEN**. Four residuals require an explicit decision:

1. **R1 model-provider token** remains readable by the sandbox agent; brokered model transport not present.
2. **R2 defective opencode_worktree_remove tool** still reaches implementation despite deny. Mount limits its reach, but destructive safety of the permitted lane was deliberately not tested; no assumption of full block.
3. **R3 symlink escape** bypasses tool external-directory checks; unmounted targets are stopped by namespace, not all mounted-path scenarios.
4. **R4 network/abstract socket reach** survives at OS level for model transport, without an enforced destination allowlist; no exposed model tool route was demonstrated, but future escapes have an egress path.

**Manager adopts a restrictive entrypoint rule, not code permission:** a future source writer must be an independently registered confined **whole-process CLI session**, never a native T3-delegated thread (which inherits full access). Any change to launcher/policy requires independent requalification. The owner has NOT accepted these remaining risks by silence.

## Next security choice and development path

**A — recommended: further host/platform restriction.** Obtain genuine network endpoint allowlist or brokered model transport and neutralise/disable the escaping destructive worktree tool at host/gateway boundary; independently test exact final writer entrypoint. Ask for specific host-setting authority before mutation.

**B — explicit owner risk acceptance.** Owner affirmatively accepts all four named residuals for local non-public Year0 only, under the pinned whole-process CLI boundary, then Manager may separately assess named actor-specific grants. No assumed security waiver.

**C — defer.** Preserve reserved worktrees; no source.

G2 B/F/C/A/Q are **NOT** signed. Even after decision, refresh live physical ownership/leases and obtain a named independent #687 Engine.cs single-file partial pre-hash custodian grant; a separately signed fixed #639-derived fixture copy grant; actual restricted Core writer grant; independent Core review/freeze; subsequent separate Viewer writer grant; independent offline Firefox/Chromium qualification; B post-hash. #710/#725 strictly protected. L4 .NET FAIL38 unwaived. No source/HTML/JS/test/runtime/Unity/Windows/Host/main/Save/public code. Viewer is not implemented or verified yet. Dashboard/Drive sync unverified.

**User decision requested:** A, B or C. No further general viewer documentation review is needed.
