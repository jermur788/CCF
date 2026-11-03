# Stand Map Screenshot Discovery — Local Read-Only Handoff

**Claim:** #237  
**Parent visual task:** #232  
**Parent programme:** #91 / Browser Overall #200 / local orchestration #203  
**Date:** 2026-11-03  
**Status:** READY — may run in parallel with #231 because it is read-only and requires no Unity

## Purpose

Find the best existing screenshots from recent CCF verification runs that show the **current top-down Stand Map canopy presentation**, so they can be used as visual references for #232 (species canopy differentiation, especially making Sitka spruce obviously coniferous from above).

This is evidence discovery only. Do not modify the running Tier-3 candidate, production, verification evidence, Unity, worktrees or canonical screenshots.

## Difficulty / routing

- **Difficulty:** Low — read-only filesystem search, image triage and evidence indexing.
- **Best-suited executor:** local DeepSeek/SOL/Codex utility lane with normal filesystem access.
- **Unity:** not required; do not start it or acquire the machine-wide lock.

## Primary search roots

Start with recent evidence under:

`/media/jer/Files/CCF/Verification/`

Known recent roots from the accepted MFM-12 return include:

- `MFM12-IntegratedQualification-Rerun-20261102/`
- `MFM12-IntegratedQualification-Rerun-20261102-attempt3/`
- `MFM12-IntegratedQualification-Rerun-20261102-attempt3-resume2/`
- `MFM12-IntegratedQualification-Rerun-20261102-guided-recheck/`

Also inspect recent roots for:

- MFM-12 fit-replay / Treatment Review evidence;
- Combined HUD evidence;
- PARITY-07 rendered evidence;
- guided-v2 evidence;
- Rack V2 / Stand Map / MapPlanning evidence;
- current #231 Tier-3 evidence, **only if the folder is readable without touching the active run**;
- other recent `/media/jer/Files/CCF/Verification/` folders whose names suggest StandMap, Rack, MapPlanning, canopy, map, minimap or species rendering.

Use only read operations on existing files.

## Search strategy

1. Enumerate recent PNG/JPG files recursively, prioritising filenames containing terms such as `map`, `stand`, `standmap`, `canopy`, `rack`, `treatment`, `review`, `guided`, `combined`, `parity`, `species`, `minimap`.
2. Do not trust filenames alone. Inspect image dimensions and preview/contact-sheet candidates where practical.
3. Prefer images showing the **full Stand Map** rather than only menus or HUD chrome.
4. Also capture useful images of the minimap or species legend if the crown shapes are readable.
5. Do not copy, rename, edit or overwrite canonical evidence. If a contact sheet is useful, create it in a separate temporary discovery directory outside canonical evidence.

## What to return

Rank the best screenshots and record for each:

- absolute path;
- source verification run/task;
- image dimensions;
- whether it shows full map, minimap, or legend;
- whether individual tree crowns are large/clear enough to judge silhouette;
- whether Sitka/conifer crowns are visible;
- whether Oak/Beech comparators are visible;
- whether overlays/marks obscure the canopy;
- why it is or is not useful as a design reference.

Identify the **best 1–3 files** to use as direct visual references for #232.

If no suitable existing screenshots exist, state that clearly and identify the smallest later capture needed after #231 completes.

## Hard boundaries

- no Unity launch;
- no machine-wide Unity lock;
- no project/worktree edits;
- no production changes;
- no modification of #231 evidence while it is active;
- no deletion/cleanup of evidence;
- no Windows build;
- #194 remains NO ACTION.

## Durable return

Publish:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/SCREENSHOT-DISCOVERY-HANDOFF.md`

If a repository docs write lane is unavailable, post the same result to issue #237 as a comment and leave the filesystem untouched.

Success token:

`STAND MAP SCREENSHOT REFERENCES FOUND — READY FOR VISUAL DESIGN`
