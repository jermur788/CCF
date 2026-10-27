# RACK-V2-02B-R — headland / fresh-world implementation: qualified

**HEADLAND IMPLEMENTED AND QUALIFIED — READY FOR INTEGRATION**

Qualified candidate: **`7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd`** (production code). Branch `feature/scenario-one-rack-v2-headland-v1`, base `c6342f79a3e89c040fadd232017a937132779385`. This handoff commit is documentation-only and sits on top of the qualified commit; the Unity gates were run on `7f708e4` exactly.

- GitHub path: `Docs/Handoffs/RACK-V2-02B-R/HANDOFF.md` on `origin/feature/scenario-one-rack-v2-headland-v1`
- Implementation commit: `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd`
- Handoff commit: the commit that last touches this file on the branch (see `git log -- Docs/Handoffs/RACK-V2-02B-R/HANDOFF.md`); an earlier blocker-state revision is `688e433`
- External evidence archive: `/media/jer/Files/CCF/Verification/RackV2-02B-R-20261027/` (`RUN-RECEIPTS.md` is the index)

## What changed (production)

- `ForestStartingStand.cs`: for a V2 world (`RackPlanningV2Active` = Enlarged80 and `rackPlanningModel == 1`) the outer-planting omission set also omits slots whose stem centre falls in the guarded headland/feeder/pad footprint, **before** mortality ranking and through the existing omission budget. The component runs at execution order 100 so `ScenarioOneManager.Awake` has set the world identity first, and it adds `ScenarioOneHeadlandPresentation` to its own object.
- `ScenarioOne/ScenarioOneHeadlandGeometry.cs` (new): the qualified constants (headland x −35.4, z ±37.7, width 2.6 m, feeder z −27.4, existing-path connection x −2.436166, pad diameter 4.4 m, 0.15 m stem-centre guard) and the analytic clearance test.
- `ScenarioOne/ScenarioOneHeadlandPresentation.cs` (new): one flat mesh using the existing `Forest Path` material at path-top height, no collider (the existing ground collides), shown or hidden each `LateUpdate` from the live `RackPlanningV2Active`.
- Nothing persisted: no headland coordinates, width, feeder coordinates or omitted IDs are saved. `rackPlanningModel` alone selects the V2 starting world. No save schema, scene, prefab, ScriptableObject, project setting or package changed. No 03C-owned file was touched (`ScenarioOneManager.RackPlan.cs`, `Racks/*`, rack runtime, work orders, map/editor, orange spray, objectives, Work Plan, MFM all untouched).

## Qualification

Lane: `/media/jer/ZX20/CCF-U1` (CCF-Ops-registered), detached at `7f708e4`, via the Manager-approved "existing configured Unity lane" route; read-only with respect to development, no rebaseline, no machine-wide config change. Unity lock `ceb78d2e…` acquired and released normally. U1 ended clean (0 uncommitted or untracked files).

| Packet item | Evidence | Result |
|---|---|---|
| 1 Fresh model-1 world has the headland | `RackV2HeadlandQualification` 124 checks: model-1 identity, route surface exists with no collider, every starting stem clear of the guarded footprint | PASS |
| 2 Exactly 1,344 living starting trees | same gate; offline adapter also 1,344 | PASS |
| 3 The qualified 42 stems absent, 42 replacements present | same gate; offline adapter matches every RACK-V2-02A ID, position, DBH and height | PASS |
| 4 Fresh model-0 Enlarged80 historical | same gate: historical omission set, no V2 surface | PASS |
| 5 v20/pre-V2 Enlarged80 load and re-save historical | same gate: v20-labelled load stays model 0, no surface, re-save stays model 0; `RackV2SaveVerification` 35 checks | PASS (see caveat 4) |
| 6 Legacy40 unchanged | `StandGeometryVerification` `5546B9B3B50673B2` | PASS |
| 7 Reference Future unchanged | `ScenarioReferenceVerification` `7AD177B3CC2F73C7`, `BFFF66015A6619DA` | PASS |
| 8 Protected fingerprints exact | all seven exact, none rebaselined: `9C77EB4BEE427FB9`, `5546B9B3B50673B2`, `94589C749852F359`, `92F9635566BF6AFE`, `7AD177B3CC2F73C7`, `BFFF66015A6619DA`, `702766DECE591E21` | PASS |
| 9 Path → feeder → headland coherent | 1280×720 and 1920×1080 captures at both junctions plus feeder and headland corridor views; keyboard traversal moved 4.29 m on the feeder and 4.20 m on the headland in 1 s | PASS (caveat 5) |
| 10 All five rack entrances usable | `RackV2AccessVerification` aims `PO0203`, `PO1204`, `PO2003`, `PO2803`, `PO3803` from standing points inside the headland width | PASS (caveat 3) |
| 11 Compile / import | Unity import and compile in every gate, no `error CS`; runtime-source type-compile 0 errors | PASS |
| 12 Rendered review at 720p and 1080p | `unity-access/evidence`, `unity-feature-retry/evidence`, `unity-diagnostic/evidence` | PASS (caveat 5) |
| 13 No collider/navigation issue | 0 blockers across 105 route-collider clearance probes (34 + 71 positions along the two route parts); surface has no collider; traversal gates above | PASS |

Also run on the candidate: `ScenarioOneCompletionVerification` growth-2 and historical-v18 variants.

## Caveats and findings (read before integrating)

1. **Awake-order change.** `ForestStartingStand` is now `[DefaultExecutionOrder(100)]`. The Legacy40, Enlarged80, Reference and both Completion fingerprints are unchanged, so no leak into historical generation was observed. Any future code that assumes the stand generates before default-order Awakes would be affected.
2. **Two harness defects, candidate unchanged.** (a) The first headland run failed at "fresh model 0 historical omission set" because the harness had left the player on the headland and `ForestStartingStand.CollectClearancePoints` clears trees around the player's current position; the retry restores the start position. (b) The first access run failed at `PO1204`: `ForestPlayer.interactionDistance` is 3.5 m and the harness stood only on the centreline, 3.82 m from `PO1204`. Run 2 also tries x −34.8 and −34.5. Both failed runs and the original sources are preserved in the archive.
3. **`PO1204` is the far entrance.** It stands at x −31.60, so it can be inspected from the headland only from the east part of the strip (aimed from x −34.8 at 3.22 m). The other four are 1.6–2.4 m from the centreline. This follows from the qualified geometry, which was not changed. 03C or the rack-start UX should not assume centreline reach.
4. **The v20 fixture is a relabelled capture.** The gate takes a current capture and sets `version = 20`; it is not a historical save file. `RackV2SaveVerification` (35 checks) and the exact historical fingerprints are the stronger evidence. A cold-start Continue from a real pre-V2 save file was not run.
5. **Review limits.** The captures are automated, from fixed vantage points, and were reviewed by the agent, not by a human player. An unexplained hard-edged dark rectangle beside the feeder was investigated with a diagnostic: the only large renderers there are the 80 m `Ground` plane and the headland mesh; the identical vantage in fresh model 0 has no such gap. It is read as bare forest floor beside the strip, an inference from screenshots. The headland and feeder do **not** appear on the minimap or Stand Map; map presentation was out of scope and is a follow-up for the map owner.
6. **Not run.** No cold-start (quit, relaunch, Continue) test of a saved V2 world, no standalone Player build, no multi-frame same-frame-transition stress beyond the three-frame settle in the gates. The host object (`Game State`) is active in `ForestTest.unity` and nothing destroys the stand component, by code inspection.
7. **Post-shutdown Editor hang** (documented known issue) hit two gates (Reference, Completion growth-2). A U1-scoped reaper SIGKILLed the Editor only after its own `Cleanup mono` marker and at least 90 s of log silence; both gates are PASS on required log tokens and are labelled `exit -9` in the receipts.

No Unity project asset, scene, setting or package was edited. No integration, PR, build, rebaseline or machine-wide CCF-Ops change was made.

**DASHBOARD SYNC: NOT DONE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.**
