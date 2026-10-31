# Research Source Consolidation Audit 01 — DeepSeek Dispatch Handoff

## Purpose

Evaluate the complete CCF Game Dev research/source-file corpus and design a safe consolidation/archive plan that can materially reduce active storage and source clutter without losing original evidence, pagination/provenance or rollback capability.

This is **analysis and planning only**. Do not delete, move, merge, watermark, recompress or otherwise alter any current source file under this claim.

## Canonical packet

`Docs/Handoffs/Research-Source-Consolidation-Audit-01/TASK-PACKET.md`

Packet publication start: `488f093e9100091858f81947b588aad5c36d84f0` or a clean descendant after refreshing `main`.

## Difficulty

**4/5 — High.** The task is not technically difficult because of coding; it is difficult because it must inventory a broad multi-location corpus, distinguish exact duplicates from independent/superseded evidence, preserve citation provenance, calculate real storage savings and produce a reversible archival design.

## Best-suited model/provider

**DeepSeek — high-reasoning / long-context research agent**, with local filesystem access and PDF/document inspection capability.

If the DeepSeek worker cannot access the actual Game Dev project sources/local research archive and obtain file sizes/hashes, stop and report the missing access. Do not produce a speculative inventory from filenames alone.

## Lane

Suggested analysis lane:

- branch: `research/source-consolidation-audit-v1`
- worktree: `/media/jer/Videos/CCF-RESEARCH-CONSOLIDATION-AUDIT`
- production/source ownership: **none**
- writable repository surface: only `Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md` and narrowly necessary evidence/inventory text under the same handoff folder.

The research/source files themselves are read-only for this task.

## Required start

Before work:

1. refresh `main`;
2. read `Docs/Project/research-index.md`;
3. read `Docs/Project/coordination/active-tasks.md`;
4. read the task claim and this packet;
5. verify a clean dedicated branch/worktree;
6. post `[RUNNING — RESEARCH AUDIT]` with provider/model, branch/worktree and exact starting HEAD;
7. record which source locations are actually accessible.

No Unity lock is required. Do not launch Unity.

## Core questions the audit must answer

- Which source files are exact duplicates and where?
- Which are older/superseded versions that still have historical value?
- Which have already been absorbed into project-facing composites?
- Which must remain individually accessible because existing citations depend on their pagination or authority?
- Would one master PDF actually save meaningful bytes, or just reduce file count?
- Which pairs/small groups make better topic bundles?
- Which originals can move out of the active project-source set while remaining byte-identical in archive?
- Should derivative project-facing copies carry a cover-page/banner or watermark such as `PROJECT REFERENCE COPY — ORIGINAL ARCHIVED`, and how can that be done without obscuring content or confusing originals with derivatives?
- What exact archive/backup layout, SHA-256 manifest and page map would make cleanup reversible?
- How many GiB are reclaimed on the constrained active surface under each strategy?

## Required handoff

Publish:

`Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md`

Return either:

`RESEARCH SOURCE CONSOLIDATION PLAN COMPLETE — READY FOR USER / MANAGER DECISION`

or:

`BLOCKED — RESEARCH SOURCE CORPUS INCOMPLETE`

with the exact missing locations/files.

## Copy/paste dispatch

`Run Research Source Consolidation Audit 01 from Docs/Handoffs/Research-Source-Consolidation-Audit-01/TASK-PACKET.md using a DeepSeek high-reasoning agent. This is analysis/plan only: do not delete, move, merge, watermark, recompress or otherwise mutate any source. Start from clean current main at/after 488f093e9100091858f81947b588aad5c36d84f0 on branch research/source-consolidation-audit-v1 in /media/jer/Videos/CCF-RESEARCH-CONSOLIDATION-AUDIT; refresh research-index.md, active-tasks and the task claim, then post [RUNNING — RESEARCH AUDIT]. Inventory the complete Game Dev project-source corpus, Docs/Research, local CCF research archives and accessible Drive/distribution research copies; record paths, byte sizes, SHA-256 where possible, exact/near duplicate and superseded/composite relationships, citation/pagination dependencies and project role. Compare exact-dedup, topic-bundle PDFs, one master PDF, project-facing derivatives plus byte-identical archived originals, marked derivative reference copies, and cold/archive-container strategies. Measure actual active-space savings, total retained archive size, provenance/citation risk and rollback requirements. Preserve originals byte-for-byte and produce a SHA/page-map based reversible plan. Publish Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md and return RESEARCH SOURCE CONSOLIDATION PLAN COMPLETE — READY FOR USER / MANAGER DECISION or the exact access blocker. No Unity or Windows build.`

## User action required

Dispatch the copy/paste line above to the DeepSeek agent. No other user action is required until the audit returns with its recommended target state.
