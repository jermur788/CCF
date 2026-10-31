# Research Source Consolidation Audit 01 — Task Packet

## Status

**READY — ANALYSIS / PLAN ONLY. NO DELETION, RELOCATION, MERGE, WATERMARK OR SOURCE MUTATION IS AUTHORISED BY THIS PACKET.**

## Purpose

Audit the complete CCF Game Dev research/source-file corpus and produce a concrete, reversible plan to reduce storage and project-source clutter while preserving evidential provenance and original source integrity.

The user is specifically open to several outcomes, including:

1. combining many PDFs into one larger PDF;
2. combining only selected pairs/small groups of overlapping PDFs;
3. keeping project-facing consolidated/composite copies while moving exact originals to a clearly identified backup/archive location;
4. creating visibly marked **derivative reference copies** (for example a cover-page or watermark such as `PROJECT REFERENCE COPY — ORIGINAL ARCHIVED`) while preserving original research files byte-for-byte in archive;
5. retaining some important sources individually where consolidation would make citation, provenance or future audit materially worse.

This task decides **what should be done and why**. It does not perform destructive cleanup.

## Authority / project context

- Git remains canonical for canonical project documentation and implementation truth.
- `Docs/Project/research-index.md` is the current research-source index and must be read first.
- Existing project policy already uses project-facing composites while preserving originals in archive locations; do not reinterpret prior consolidation as permission to delete originals.
- Research recommendations remain supporting evidence, not decision authority.
- Preserve exact filenames, source identity, page/section provenance and evidence classification wherever a source has been used in project decisions, specs, research composites or verification.

## Exact starting Git state

Start from clean `main` at or descended from:

`5736563ce00fc4826402b608e9e97f62aed3aab2`

Refresh `main`, this packet, `Docs/Project/research-index.md`, and live `Docs/Project/coordination/active-tasks.md` before beginning. If the research index or archival policy materially changed, use the newer canonical state and record it.

## Preferred executor

- **Provider/model:** DeepSeek-class high-context research/reasoning agent.
- **Difficulty:** **4/5** — broad corpus inventory, duplicate/overlap analysis, provenance preservation, storage economics, and reversible archive design across Git/project sources/local archive/Drive mirrors.
- **Required capabilities:** local filesystem metadata and hashing; PDF/document inspection; Git read access; ability to read the Game Dev project sources or their local/archive equivalents. If the agent cannot access the actual source corpus and sizes, it must stop and report the missing access rather than invent an inventory.

## Scope — corpus to inventory

Treat the following as **seed locations / known surfaces**, not an exhaustive list. Discover the actual current corpus.

### A. Game Dev project/source attachments

Inventory every current Game Dev project source file, including PDFs and Markdown evidence packs/composites. Known examples include:

- `Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf`
- `CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf`
- `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf`
- `Scenario Two Species Evidence Closure.pdf`
- `2006_VOL63_NO1&2-ss.pdf`
- `Primary-Source Audit of the CCF Sitka Spruce Growth Evidence Base.pdf`
- `CCF-SITKA-GROWTH-EVIDENCE-AUDIT-V1B.pdf`
- `Delegated Marking and Operator Selection in Irish, UK and European Forestry.pdf`
- `Model Capability and Cost Research — Repository Execution Report.pdf`
- the four established CCF evidence composites and other attached research Markdown/PDFs.

Do not assume this list is complete.

### B. Repository research/documentation

At minimum inspect:

- `Docs/Project/research-index.md`
- `Docs/Research/**`
- research-related canonical references and extraction records referenced by the index
- any manifests, source maps, provenance tables or hashes that constrain consolidation.

Repository Markdown is generally low-storage and often canonical; do not recommend deleting canonical Git records merely to save local disk space.

### C. Local/archive research copies

Where accessible, inventory the current research/document locations referenced by canonical project docs, including:

- `~/Documents/CCF Game/`
- `/media/jer/Files/CCF/Research/`
- other current CCF research/source folders discovered from the storage README/current machine configuration.

Do not touch Unity worktrees, Libraries, verification evidence, user personal files, or unrelated folders.

### D. Drive/distribution archive

Where connector/local mirror access exists, inspect the structure and metadata of:

- `CCF — Shared Project Context/03 — Ecology Research`

Drive is a distribution/archive mirror, not implementation authority. Do not edit or delete Drive files in this task.

## Required inventory fields

Create a row for every research/source artifact discovered, with at least:

- exact filename;
- path/location(s);
- file type;
- byte size;
- SHA-256 where local bytes are accessible;
- exact-duplicate group, if any;
- probable near-duplicate / superseded / incorporated-into-composite relationship;
- title/subject;
- source class: original primary source / secondary research report / synthesis / composite / audit / canonical extraction / historical working artifact / other;
- current project role: retained individual / project-facing composite / archived original / canonical Git reference / distribution copy / unclear;
- whether cited by `research-index.md`, decision log, spec, canonical research file or handoff;
- whether page-specific citations/provenance depend on the file retaining its original pagination;
- whether the file is safe to consolidate as a derivative while retaining the original;
- whether the file should remain individually accessible;
- estimated reclaimable bytes under each proposed strategy.

## Duplicate and overlap analysis

Distinguish clearly between:

1. **exact duplicate bytes** — same SHA-256;
2. **same document, different container/export** — visually/content-equivalent but different bytes;
3. **superseded version** — older audit/report retained for history;
4. **partial overlap** — one report incorporates sections from another;
5. **composite absorption** — claims have already been rationalised into a project-facing composite with provenance back to originals;
6. **independent evidence** — similar topic but distinct source authority and should not be treated as redundant.

Do not label similar subject matter as duplicate unless the content relationship is demonstrated.

## Consolidation strategies to evaluate

Evaluate at least the following, with actual projected storage savings and risks.

### Strategy A — Minimal intervention / exact-dedup only

Keep all logically distinct files. Remove only redundant local/project copies that are byte-identical and already preserved in a designated archive. Recommend this only where references can be updated without losing source availability.

### Strategy B — Topic bundles

Create a small number of topic PDFs from selected related documents, for example growth/density, ecology/regeneration, browsing/protection, forestry practice/management, while originals move to archive.

For every proposed bundle specify:

- member files and order;
- whether pages should be concatenated unchanged;
- separator/cover pages;
- original-file-to-bundle page map;
- how existing citations can still resolve;
- exact originals that remain in archive;
- projected working-set space saved.

### Strategy C — One master research PDF

Evaluate a single consolidated PDF only if technically and evidentially sensible. Report total page count, expected size, navigation/bookmark design, page-mapping burden, citation breakage risk, update cost and whether it would actually reduce bytes.

Do **not** recommend a one-file solution merely because it reduces file count.

### Strategy D — Project-facing derivatives + byte-identical originals in archive

Keep slim project-facing derivatives/composites and move originals out of the active project-source set into a designated backup/archive. Evaluate whether this best matches the project's existing composite/archive pattern.

### Strategy E — Marked derivative reference copies

Evaluate a visible marking scheme only for **derivative copies**, never originals. Possible treatment:

- cover page and/or unobtrusive watermark: `PROJECT REFERENCE COPY — ORIGINAL ARCHIVED`;
- archive path / source ID / original SHA-256;
- derivative build date;
- statement that pagination/provenance mapping is preserved or a page-map is supplied.

Report whether watermarking risks obscuring figures/text or changing OCR/search usability. Prefer a cover-page/banner approach if safer.

### Strategy F — Archive container / cold backup

Evaluate moving originals to a backup volume or archive container with a manifest. Do not assume ZIP will materially compress PDFs; measure or estimate based on trial compression of representative files. Consider separate archive folders versus a tar/zip/7z container, recoverability, corruption blast radius, partial retrieval, and checksum validation.

## Preservation requirements

Any recommended future execution plan must preserve:

- at least one byte-identical copy of every original research source that has evidential/provenance value;
- SHA-256 manifest for originals before cleanup;
- original filename and source title;
- page-level citation provenance or an explicit old-page → consolidated-page map;
- historical versions where the project intentionally distinguishes V1A/V1B or similar audit stages;
- canonical Git references and research-index semantics;
- the distinction between project-facing composite/derivative and original source;
- ability to restore the pre-cleanup active corpus from archive without relying on memory.

Never watermark, recompress, OCR-rewrite, merge, linearize or otherwise alter the **only** preserved copy of an original.

## Storage accounting

Measure/report separately:

- current active project/source footprint;
- current local research footprint;
- exact duplicate bytes across locations;
- archive footprint that must remain somewhere;
- space reclaimed on the constrained active disk/project-source surface;
- total storage across all devices after consolidation;
- temporary working space required to build merged PDFs;
- worst-case rollback space requirement.

The user wants to free space, so recommendations must distinguish **moving bytes off the active device** from merely combining files without reducing total bytes.

## Risk classification

For every proposed action classify:

- **GREEN** — reversible and low provenance risk;
- **AMBER** — useful but requires citation/index changes or careful page mapping;
- **RED** — threatens original-source provenance, canonical references, or recoverability; do not recommend without a strong reason.

## Required deliverable

Publish:

`Docs/Handoffs/Research-Source-Consolidation-Audit-01/HANDOFF.md`

The handoff must contain:

1. executive recommendation;
2. complete inventory summary and total bytes;
3. exact duplicate groups;
4. near-duplicate/superseded/composite relationships;
5. files that should stay individual and why;
6. proposed bundles/merged PDFs, if any;
7. archive/backup design;
8. derivative watermark/cover-page recommendation, if useful;
9. current vs projected storage table for each strategy;
10. citation/provenance migration plan;
11. checksum/manifest/rollback plan;
12. ordered future execution steps;
13. explicit list of actions requiring user/Manager approval;
14. anything the agent could not access or verify.

Also include a concise **recommended target state** such as:

- active project sources: N files / X GiB;
- archived byte-identical originals: N files / Y GiB at exact path;
- project-facing composites/bundles: N files / Z GiB;
- estimated active-space reclaimed: X GiB;
- restoration method: exact command/process + manifest location.

## Output verdict

If the corpus is fully inspected and a safe plan is produced, return:

`RESEARCH SOURCE CONSOLIDATION PLAN COMPLETE — READY FOR USER / MANAGER DECISION`

If the agent cannot inspect the complete corpus, return:

`BLOCKED — RESEARCH SOURCE CORPUS INCOMPLETE`

and list exactly which locations/files are inaccessible.

## Prohibited in this task

- deleting files;
- moving files out of their current locations;
- removing project sources;
- editing Drive archive content;
- overwriting originals;
- merging PDFs in place;
- watermarking originals;
- changing research conclusions, evidence classes or citations;
- editing gameplay/production code;
- Unity work;
- Windows build.
