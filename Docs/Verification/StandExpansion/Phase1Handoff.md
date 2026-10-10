# IMPLEMENTATION HANDOFF — Phase 1 audit, STOP for Manager decision

```text
Task: Scenario One stand expansion to >= 80 x 80 m (D-056)
Worker: Claude (Sonnet 5.5)
Role: Primary implementation worker
Branch: task/scenario-one-80m-stand
Base: 11c3596072a0422d5426291e370129d209c56342
Context: 8bed3996aefd1780c62744b648094efe5b394feb
HEAD: reported with the commit receipt (docs and tooling only; no production change)
Worktree: /home/jer/CCF-s1-80m (own Library on /media/jer/ZX20/CCF-s1-80m-local-output/Library)
```

Status: **PHASE 1 COMPLETE — STOPPED BEFORE IMPLEMENTATION. Decision required: Yes.** Implementation was not started, because the audit found the packet's stop conditions. Full audit: [StandExpansionAudit.md](StandExpansionAudit.md).

## PHASE-1 AUDIT

**Current geometry:** 40 × 40 m ground box and four 5 m ridge boxes at ±20 m; ecology 5 m cells, 8 × 8 = 64, centred on the origin; player start (−0.43, 0.1, 5.25); 52-plank path; 9 × 9 m work clearing; seven hidden, non-solid construction sites in the central area. `standSizeMeters = 40` and the starting-stand values are serialized in the scene.

**Current tree count / stems per ha:** 336 trees on 0.16 ha = 2,100 stems ha⁻¹ (DBH mean 15.6 cm, height mean 11.6 m, all age 20). 46.1 % of trees stand within 5 m of an edge, 32.4 % within 5–10 m and only 21.4 % beyond 10 m; 66.7 % are within the 8 m competition cutoff of an edge.

**80 m density-preserving candidate (benchmark only, Editor):** 1,344 trees, 256 cells, 2,100 stems ha⁻¹. Trees 0–5 / 5–10 / > 10 m from the edge: 22.5 % / 20.0 % / 57.5 %; true interior (> 8 m) 65.4 %. Mean Hegyi competition 6.55 / 9.02 / 9.67 by band (edge trees about 32 % lower than interior). Cell light is near-closed canopy everywhere at year 0 (0.005–0.013), so the edge shows in competition, not light. Renders: `Evidence/run1/`.

**Save/reference finding:** the geometry change needs a persisted geometry identity (save-schema change). No geometry is saved; six kinds of cell-indexed record exist; the validator checks only `index < cellCount`. Probes in an 80 m grid: a real v19 Model-2 save is **rejected**; a pre-Model-2-style save is **accepted and silently misplaced**; `ScenarioReferenceArchive.Matches` ignores geometry and **Reference Future v1 opens successfully but wrongly** (its heaviest-regeneration cell at (−12.5, −17.5) lands at (−32.5, −37.5)). `definitionVersion` must not be bumped (the archive pins v12 and accepts live v12/v13).

**Economy/objective finding:** with a proportional first thinning (48 of 336 vs 192 of 1,344 trees) the contractor fee stays at the €2,500 minimum, underlying labour cost goes €119 → €494, timber revenue €283 → €1,185, net cash change −€2,217 → −€1,315, and the fee is 8.8× vs 2.1× the revenue. Absolute thresholds shrink in meaning: 60 retained originals 17.9 % → 4.5 %, 3 regeneration cells 4.7 % → 1.2 %, century targets 120 trees / 12 cells 35.7 % / 18.8 % → 8.9 % / 4.7 %. These are Manager calibration decisions; none was changed.

**Performance finding (Editor, Linux only; no Player/Wine/native claim):** annual step ecology-only about 0.12 s → 1.0 s; full manager step 0.19 s → 1.6–1.8 s steady state (first steps 2.2–2.6 s); `RecomputeCanopy` 10.7 → 205.6 ms (cells × trees, 16× iterations, native position call in the inner loop); rendered frame 22.96 → 35.87 ms median (43.6 → 27.9 fps); process RSS +149 MB at start. Stocking was **not** reduced.

**Decision required: Yes.** See Options below.

## OPTIONS FOR THE MANAGER

1. **Versioning (recommended):** approve a `standGeometryModel` field (save v20; 0 = Legacy40 for missing field and ≤ v19; 1 = Enlarged for new games; Reference Future v1 = 0), geometry applied before validation and cell restore on load, geometry-aware validation, `Matches` requiring model 0, `definitionVersion` unchanged, and a `CCF_STAND_GEOMETRY=0` replay switch so every legacy gate and anchor stays valid. Alternatives not recommended: replace 40 m outright (breaks the immutable Reference contract and all saves) or a separate Enlarged scene/scenario ID (needs scene switching on load).
2. **Economy/objectives:** keep all absolute values and accept the consequences, or authorise a separate calibration packet (candidates: minimum harvest-job fee, retained originals, regeneration cells, reference targets, owner minutes). Not to be done implicitly inside the implementation task.
3. **Performance:** accept the measured Editor cost, or authorise the behaviour-preserving cell × tree loop optimisation first (cache position/height/crown per rebuild, bucket cells by reach, avoid the redundant second canopy rebuild), proven bit-exact against the existing anchors; independently, a Player/Windows measurement before deciding. Frame-time options need a Player build.
4. **Central-area preservation:** re-parameterising the generator preserves **none** of the 336 current positions (IDs change, so jitter and mortality ranking change). If the Manager wants the current central stand preserved, the Enlarged set should generate the existing 336-tree block unchanged and fill the outer ring (about 1,008 stems) at the same stocking; edge trees of the old block may then change their neighbour-based DBH class.

Suggested sequencing for the next packet: (1) versioning plus replay switch with zero behaviour change; (2) Enlarged parameter set, scene objects, generator; (3) validation, new labelled anchors, map/waypoint playthrough and Player/Windows performance; (4) approved calibration changes.

## IMPLEMENTED GEOMETRY

None (stopped at Phase 1). Stand dimensions, ecology cells, tree count, starting stems/ha and edge-band distribution above describe the **current** world and the **benchmark candidate**, not an implemented change.

FILES CHANGED: added `Docs/Verification/StandExpansion/` (audit, this handoff, `Evidence/`), `Tools/Verification/StandExpansion/StandExpansionAudit.cs` and `run_stand_audit.py`. No production file touched.

SERIALIZED FILES CHANGED: none. The measurement harness builds the 80 m candidate in play mode only; the scene file is never saved. Editor-normalised `.mat`/`.vscode` files were reverted before commit.

CENTRAL 40M PRESERVATION: not achieved by re-parameterising (0 of 336 positions reappear; 296 trees in the old footprint); needs the design in Option 4.

MAP / WAYPOINT RESULT: not tested in an implemented stand. On paper the map fits (16 × 40 px = 640 px beside a 330 px panel in a 1600 × 900 reference panel; labels A–P × 1–16); legibility and refresh cost at 256 cells are unmeasured.

FIRST-CYCLE PLAYTHROUGH: not performed (no implemented stand). Arithmetic only: crossing 80 m is about 20 s walking at 4 m/s. The scripted benchmark did run a proportional thinning and three annual advances through the real manager path in both worlds without failures.

PERFORMANCE BEFORE / AFTER: section 5 of the audit (Editor only; "after" is the unsaved candidate benchmark).

SAVE / LOAD: no change. Observed behaviour of the current code against an 80 m grid is in the audit (section 6).

REFERENCE FUTURE: not touched; archive blobs and anchors unchanged. Compatibility probes were read-only observations in a scratch play session.

NEW STAND ANCHORS: none created (no implemented enlarged stand).

## P2 / P3 / REGRESSION

P2 (inherited open gate), recorded honestly as two states:

- **Start of task** (fresh worktree, first run, 2026-10-10 08:17): batch ×2 and interactive all **PASS**, hash `F58FB0B1A421D28B`. The interactive capture is pixel-identical to the S1-B worker's 2026-10-09 22:27 passing capture and differs from the 00:30–02:30 failing captures. The known tag/panel overlap did **not** reproduce.
- **End of task** (same worktree, 08:28): batch ×2 **PASS** (`F58FB0B1A421D28B`), interactive **FAIL** with the same `a world label is drawn under a panel at 1280`; its capture is pixel-identical to the overnight failing capture (0 differing pixels). So the inherited failure **did reproduce** after intervening runs, and it is **not** caused by anything in this task (no production file changed; the only runs in between were the read-only measurement harness).
- **Diagnostic only, fixture untouched:** three further interactive runs with the X pointer parked at its original spot (489, 823), at screen centre (960, 540) and near the top-left all **FAILED** identically. The pointer position is therefore not the discriminator (the pointer was restored to (489, 823) afterwards). The persisted P2 config holds only PlayerPrefs, analytics values and licences, no window layout.
- Net observation for the follow-up: in this worktree the only pass was the first graphical run; every later interactive run failed (5 of 5), and the passing and failing captures are each internally pixel-identical, so the state is **bimodal and persistent**, not noisy. Cause still unidentified; the pass is not claimed as fix and the fail is not attributed to the stand expansion. The P2 fixture was not changed.

P3, full 24-gate regression, SessionMenu, Teaching Copy, MenuTutorial, Clearance, Removal, ScenarioOneInteractionVerification and the Model2 / Reference Y100 / continuation hashes were **not rerun**: production code is byte-identical to `11c3596` (verified at S1-B integration), so they were not applicable to a docs-and-tooling change.

CONSTRUCTION-FRAME POLICY: unchanged; hidden sites remain hidden and are still reserved as 2.2 m exclusion zones by the starting-stand generator.

UNRELATED WORK PRESERVED: `/home/jer/CCF-main` untouched; `/home/jer/CCF-s1a` and `/home/jer/CCF-s1b` not used. The shared `.git` metadata of `/home/jer/CCF` was not edited (the worktree's `Library`/`Build` symlinks appear as untracked and were never staged).

KNOWN LIMITATIONS: Editor-only measurements; one proportional thinning case; candidate world lacks the scenario's learning objectives (harness re-initialisation) and per-cell understorey visuals; decorative pack assets absent in this worktree.

DECISIONS NEEDED: Options 1–4 above.

READY FOR MANAGER REVIEW: **Yes — as a Phase 1 audit and decision request.** Implementation is **not** started.

RECOMMENDED NEXT STEP: approve Option 1 and issue a versioning-first packet (zero-behaviour-change replay switch), then the Enlarged parameter set, with calibration and optimisation decided separately.
