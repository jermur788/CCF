# RACK-V2-03C — Publication Request

**Date:** 2026-10-27  
**Issued by:** Overall Manager  
**Target worker:** DeepSeek / owner of `feature/scenario-one-rack-v2-runtime-v1`

## Dispatch instructions

**SEND THIS HANDOFF TO:** the **same DeepSeek Game Dev implementation chat / agent that completed RACK-V2-03C** in `/media/jer/ZX20/CCF-RackV2-03C`.

**Do not send this to:** the S1-C worker, the 02B-R/Terra worker, Claude review, or a new implementation agent unless the original DeepSeek lane is unavailable and the Manager explicitly reroutes publication recovery.

**What to send:** send the contents of this file, or point that agent to:

`Docs/Handoffs/RACK-V2-03C/PUBLICATION-REQUEST.md`

**Purpose:** publication only. This is not a request to redo implementation or qualification.

**Expected return token:**

`RACK-V2-03C PUBLISHED — READY FOR MANAGER FINAL REVIEW`

The return must identify the GitHub-visible branch tip and the worker handoff commit.

## Request

Publish the already-qualified RACK-V2-03C lane to GitHub. **Do not rerun or change the qualified production implementation merely to satisfy publication.**

Required publication package:

1. Push branch `feature/scenario-one-rack-v2-runtime-v1`.
2. Publish exact implementation commit `2178d392dd0fd285a30383e0bcf4d0df1745a8af`.
3. Publish exact tested HEAD `84f801850acc26906a7cd8dd1a452d0a8da3f90a`.
4. Publish the worker implementation/qualification handoff at `Docs/Handoffs/RACK-V2-03C/HANDOFF.md` on that lineage, preserving qualification evidence, production-vs-harness delta, caveats, and evidence locations.
5. If either SHA cannot be preserved exactly, stop and explain why before any integration attempt. Do not substitute a newly rebuilt candidate silently.

## Existing Manager records

The Manager has already recorded the reported result on `main` in:

- `Docs/Handoffs/RACK-V2-03C/HANDOFF.md` — administrative tracking copy, explicitly marked publication-pending;
- `Docs/Handoffs/RACK-V2-03C/MANAGER-REVIEW.md` — technical result provisionally acceptable; integration blocked pending GitHub publication;
- `Docs/Handoffs/RACK-V2-Combined-Integration/TASK-PACKET.md` — blocked-ready continuation after publication.

The worker publication should preserve its own original handoff/evidence on the 03C branch. The Manager tracking copy on `main` does not replace the worker handoff tied to the implementation commits.

## Verification / rerun policy

If the exact qualified commits are pushed unchanged, **no new Unity run is required just because of the push**. The Manager will inspect the published branch, commits and handoff, then issue the final integration-readiness verdict.

Do not weaken, delete, or rewrite the legacy `RackUxRack` gate just to obtain green. Its F-10 disposition belongs to combined integration / G3 measurement-integrity review.

No Windows build is authorised.

## Exit

Return with:

`RACK-V2-03C PUBLISHED — READY FOR MANAGER FINAL REVIEW`

and identify the GitHub-visible branch tip plus the worker handoff commit.
