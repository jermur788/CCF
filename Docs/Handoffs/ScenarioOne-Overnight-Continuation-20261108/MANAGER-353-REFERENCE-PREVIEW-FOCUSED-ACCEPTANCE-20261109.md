# Manager #200 — #353 Reference Preview pose restore: scoped acceptance

**Date:** 2026-11-09  
**Decision:** **APPROVED for the bounded production source correction and its focused Tier-1 Unity witness; NOT YET INTEGRATED or whole-feature/user-accepted.**  
**Execution claim:** [#353](https://github.com/jermur788/CCF/issues/353) — HANDED-OFF, closed.  
**Worker evidence:** [#353 exact report](https://github.com/jermur788/CCF/blob/52bb32a6e36c7e0528f679cf97b3276a83537500/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/REFERENCE-PREVIEW-POSE-FIX-FOCUSED-UNITY-RESULT-20261109.md).

## Manager's GitHub-side inspection

- Reviewed the exact worker handoff and the issue's RUNNING/HANDED-OFF receipts.
- Compared **candidate C** `69c52906e973f6afa926c941cefa2bbc7b2a2412` to **tested C2** `b82cb2405fa245ed49e48eb363b4b664b1d85d5e`: **one** production file, `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`, **11 added lines, zero deletions**. Inspected `EndReferencePreview` directly in GitHub at C2.
- The change restores the saved player position with any enabled `CharacterController` temporarily disabled, preserves its previous enabled state, restores look/pitch, and calls `Physics.SyncTransforms()`. It does not change the reference-save data, tree generation, scene, packages or frozen archives.
- Compared tested C2 to report branch tip `52bb32a6e36c7e0528f679cf97b3276a83537500`: **one Markdown report only**, with no further production source changes.
- Prior #351 plain diagnostic failed twice in the same reset-13 pose/start checkpoint while its pinned control passed. #352 supplied a code-level trace for the preview-restore asymmetry; that mechanism remains a source inference, not engine-internal proof.

## Evidence accepted (not independently re-executed by Manager)

- **ONE** warm Unity 6000.6.0f1 batch run at explicitly pinned C2: PASS, `ENLARGED80_VERIFY_PASS checks=12`, zero C# compile errors; B3 evidence complete with no unavailable fields, and the previously failing deterministic 80B start check passed.
- Authored player start retained at post-preview +0/+1/+2 and **reset13 entry**, whereas #351's plain C run showed the old preview viewpoint at reset13.
- Reported stable reference/Legacy40 anchors, archived bytes, scene blob, package pins and 27-file licensed asset pack.
- Reported exclusive U3 Unity lease owner-safe released, disk monitor samples all OK, no Editor running at handoff. U3 remains warm, with only the known two Unity-generated `.vscode/settings.json` hunks left dirty; no authority to reset or reuse it without a fresh scoped admission.

**Verification classification:** implementation accepted, targeted Linux Unity batch witness accepted, **integration not performed**, interactive Reference Preview restoration and full Scenario One candidate **not verified**. The source-based explanation may be correct but disable/enable versus physics sync was not isolated. The reported handoff and GitHub-source diff suffice for this bounded G2 decision; no second plain run, mechanism-isolation campaign or automatic independent review is ordered.

## Disposition and next gate

**No new runtime task, no monitor re-arm and no Unity rerun now.** Preserve the tested C2 source candidate and report. Do not merge the whole historical branch or any diverged multi-feature PR into main. Carry this exact one-file fix into the eventual deliberately assembled Scenario One integrated candidate, with owner/ancestry/conflict review and G4 integration approval at that time. Check interactive preview exit, position/look and reference compatibility as part of the *existing* integrated/player-facing qualification, rather than a separate duplicate test programme. Existing Workbench, objective guidance/map, CTA verification and integrated A–O gates remain separate; A–O is **not passed** by this witness.

The original protected warm Workbench, its eight dirty tracked files/index/Library, deployed unity-lock and frozen Reference Future v1/Legacy40 remain out of scope; no cleanup, global lock replacement, builds, Windows build, user playtest acceptance or save baseline change authorised. User action **none now**. Dashboard source and downstream ingestion require their own verified status update.
