# Research Source Consolidation Audit 01 — Manager Review

**Verdict:** **APPROVED — planning/audit only; no cleanup is authorised.**

**Reviewed branch/head:** `research/source-consolidation-audit-v1` @ `60196617fd0afcbfd45642df8c8d74b3a4eac1bd`

## Accepted findings

- The measured unique local research corpus is **214.17 MiB**.
- The audited research corpus is not stored on `/media/jer/Videos`; this consolidation work therefore frees **0 B on the constrained Videos volume**.
- Approximately **131 MiB** of single-copy/original evidence should first be copied byte-for-byte to the designated durable archive and verified by SHA-256 before any active-copy cleanup is considered.
- Approximately **42 MB** of redundant/stale active copies may be reclaimable only after that backup has been verified and under a separate approved execution plan.
- A master PDF or broad topic-PDF bundling is **not approved as the default storage-reduction strategy**: measured/reasoned benefit is negligible or negative, while provenance, citation and page-mapping complexity increases.
- The preferred model is a **small project-facing composite/reference set plus byte-identical archived originals**, with SHA-256 manifests and a restoration/page map.
- Any visible project-reference watermark/banner belongs only on a **derivative reference copy**, never on the evidential original. A cover/reference page is preferable to altering source pages.
- Broad Google Drive cleanup is **not authorised**. Only two Drive/local pairs were hash-verified byte-identical in this audit; the remaining Drive relationships are not strong enough to support deletion.
- The worker could not query the live ChatGPT Project-source item count, so no claim is accepted about the exact current platform attachment count.

## Scope and evidence disposition

The returned branch is scope-clean for the audit after its refreshed base: it contributes the handoff, inventory, page-map and SHA-256 manifest plus a branch-local coordination edit. No research/source content was mutated. No Unity work and no Windows build occurred.

Only the audit evidence artifacts are carried onto current `main`; the branch-local `active-tasks.md` is intentionally **not** merged because live coordination has moved on to MFM-08 closure #195.

## Decision boundary

This approval accepts the **inventory, analysis and recommended preservation-first plan**. It does **not** authorise any deletion, move, rename, PDF merge, watermarking of originals, recompression, Drive cleanup, replacement, or detachment of any source.

Any operation that changes or removes research/source files requires a separate execution packet with an exact source/destination manifest, pre-copy and post-copy SHA-256 verification, rollback procedure, measured expected space recovery, and explicit user/Manager approval.

## Manager disposition

`RESEARCH SOURCE CONSOLIDATION AUDIT 01 — MANAGER APPROVED / PLAN ONLY`

No production integration, Unity qualification, or Windows action is required for this audit.
