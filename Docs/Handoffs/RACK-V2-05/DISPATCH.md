# RACK-V2-05 — Dispatch Record

Canonical task packet: `Docs/Handoffs/RACK-V2-05/TASK-PACKET.md`

Manager selection: this is the largest currently authorised single overnight task on the Rack V2 critical path.

Execution model strategy:
- Primary: T3 Code / strongest local SOL-CODEX Unity-capable model; GPT-6.1 Terra xhigh preferred for interactive/UI work when available.
- Mandatory watcher from task start: OpenCode / DeepSeek v4.1 Flash or strongest available DeepSeek.
- The watcher must transition execution to another suitable model/agent before quota, context, tool or provider limits stall the task, while preserving one writer, one branch/worktree and exact ownership.

No Windows build.
