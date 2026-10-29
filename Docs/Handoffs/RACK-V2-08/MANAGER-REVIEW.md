# RACK-V2-08 — Manager Review

## Decision

**APPROVED — READY FOR INTEGRATION**

Task: **RACK-V2-08 — remove competing first-person rack planning and make Stand Map authoritative for Rack V2 while preserving model-0/Legacy40 compatibility.**

Reviewed handoff: `Docs/Handoffs/RACK-V2-08/HANDOFF.md` on branch `feature/scenario-one-rack-v2-no-competing-author-v1`.

- Production implementation: `6f3c77dc5675c5388c4bc482bd1d65be38a1f4f5`
- Repaired-main merge: `3550326f4521a6a68a8f42502ee233cd027e451b`
- Exact Unity-qualified candidate: `0904f0f6bb23b55758e3e61724f89aba5ee58ef9`
- Published handoff tip: `c0379bd` (docs-only descendant)
- Resume base: `cf413c9a9059091ed26d691299a87076b7a48f5d`, containing the integrated Combined HUD layout repair.

## Scope review

The production delta is bounded to eight Rack/Scenario One/UI files. `RackPlanningV2Active` is the guard: Rack V2 first-person `RackTool` update/request/enter/confirm routes are inert, an already-active legacy preview is released if the world changes into V2, V2 help/HUD/startup/start guidance points to Stand Map `Plan Racks`, and legacy/model-0 copy and behavior remain available. No Stand Map implementation, save/schema, scene/prefab/material, ecology/economy, MFM, package/settings or Windows build change is included.

The implementation therefore satisfies the approved RACK-V2-08 boundary without absorbing RACK-V2-09 field marking, RACK-V2-10 broader Help work or RACK-V2-11 persistence.

## Qualification review

Required resume gates are satisfied on exact candidate `0904f0f`:

- Guided + Sequential model-0: **PASS 600/0**, including the seven previously-unwaived real-OS-input assertions.
- Guided + Sequential Rack V2: **PASS 553/0**.
- Combined HUD model-0 at 1280x720 and 1920x1080: **PASS 450/0**.
- Combined HUD Rack V2 at both resolutions: **PASS 455/0**.
- Legacy40 native first-person route and ordinary work: **PASS**.
- RACK-V2-07 confirm/reopen/minimap preservation: **PASS 213**.
- Runtime and Editor/harness compilation: **0 errors**.
- Editing, spacing, suggested-layout, non-UI and protected-assertion integrity checks: **PASS**.

The Combined HUD layout repair remains effective: the prior Stand Map caption overflow/panel clipping failure families are absent in both models.

## Rerun disclosures

### Model-0 Guided first attempt

The first post-merge attempt reached 594 passes and then failed the native real-input rotation chain; the same candidate and unchanged driver subsequently passed 600/0. The worker retained both runs and did not weaken, skip or rebaseline any assertion. High machine load was observed during the failure but was not proven causal.

**Manager disposition:** accepted as a disclosed nondeterministic execution failure, not a product-gate failure. The required assertions passed on the exact candidate. The failed run remains evidence and is not erased or reclassified as a confirmed infrastructure cause.

### Rack V2 Combined HUD first attempt

The first run produced uniform-white captures and failed the screenshot validity checks; immediate rerun on the same candidate produced real captures and passed 455/0. Model-0 Combined HUD passed first run.

**Manager disposition:** accepted as an invalid-capture execution attempt followed by a valid passing run. No assertion was weakened.

## Stand Map preservation scope

`StandMapParity` and the RACK-V2-05 real-pointer editing gate were not rerun. This does not block RACK-V2-08 approval. Neither is named as a mandatory gate in the RACK-V2-08 task packet/resume packet, and the Stand Map editing implementation is outside this task's production diff. The required Stand Map preservation is directly exercised by the Rack V2 Guided path (`M` -> `Plan Racks`, reset/confirm), Combined HUD, RACK-V2-07 confirmation/minimap preservation, plus the editing/spacing/suggested-layout static/offline suites.

This is a proportionate preservation decision, not a waiver of those gates for future tasks that modify Stand Map editing.

## Static-check base disclosure

The RACK-V2-07 static check flags `RackTool.cs` against its older default base because changing that file is the intentional RACK-V2-08 implementation. The bounded `--base 6f3c77d` run passes 10/10 and proves no protected rack/save source drift after the RACK-V2-08 production commit and repaired-main merge. Accepted.

## Storage / Unity operational blocker

The worker moved only identified stale scratch directories and clean merged worktrees to `/media/jer/Videos/CCF-ZX20-overflow/` under the user's explicit instruction, byte-verified the copies and repaired/rechecked worktree registrations. No unrelated dirty work was deleted.

After qualification, ZX20 has approximately **11.5 GiB free**, below the **15 GiB Unity lock floor**. The Unity lock is free, but the next Unity task must not attempt acquisition until sufficient disk space is restored. This is an operational blocker for the next Unity lane, not a RACK-V2-08 qualification defect.

## Integration decision

**APPROVED.** Open an integration PR from `feature/scenario-one-rack-v2-no-competing-author-v1` to current `main`. Preserve current Manager-owned `Docs/Project/coordination/active-tasks.md` rather than importing a stale worker queue snapshot. No additional Unity rerun is required solely because current main has advanced by docs/coordination changes after the tested merge base; there is no newer production overlap in the eight RACK-V2-08 production files.

After merge, RACK-V2-08 status becomes:

- IMPLEMENTED: **YES**
- FEATURE QUALIFIED: **YES**
- MANAGER REVIEWED: **YES — APPROVED**
- INTEGRATED: **pending PR merge**
- WINDOWS BUILD: **NO — prohibited until user playtest and explicit authorisation**

Next Rack work must be selected from the approved Rack V2 sequence after integration. Any Unity execution remains blocked until the 15 GiB storage floor is restored.
