# Closed ledger

One compact Markdown file per closed task, named DATE-TASK-ID.md. No transcripts, token telemetry or reasoning. Do not migrate all history. The pilot is explicitly simulated and is not evidence of production integration.

Required fields (colon fields, no empty values; use None/N/A):

```text
Task ID:
Purpose:
Worker route:
Base:
Final HEAD:
Verification performed:
Verdict:
Integrated?:
Canonical impact:
Evidence path:
Date:
```

Use exact full SHAs. Verdict reports only actual gates; Integrated? is Yes/No/Simulated. Canonical impact distinguishes proposed future changes from accepted context changes. Evidence paths must be on ZX20. Remove a board row only after its closed ledger record exists. Ledger is append-oriented; correct mistakes explicitly instead of rewriting prior verdicts silently.
