# #358 verification-only probes: V2 Stand Map witness and Section Five material pair

Disposable, **verification-only** tooling added by CCF #358. Nothing here is game code, nothing is compiled into a build, and the historic
`Tools/Verification/StandMap/StandMapParity.cs` is **untouched**: its two FAIL results from #357 stay FAIL (it assumes the pre-V2 world).
These files were run from the Files evidence copy (`/media/jer/Files/CCF/Verification/CCF-358-V2MapParityFinalUnity-20261109/probe/`) and are committed
here byte-identically (SHA-256 in the result report) so the runs can be reproduced. They were staged into `Assets/ForestPrototype/`, run, and removed
with their generated `.meta` by the runner (`driver/run_gate.py`, a copy of the #357 driver with only the lock task id changed).

| File | What it is | Needs Unity |
|---|---|---|
| `StandMapV2Witness.cs` | `StandMapV2WitnessLogic` (plain C#) compares what the REAL minimap and Stand Map canvases painted, item by item, one-to-one, against the model, the foundation's suggested routes and independently computed fractions; `StandMapV2Witness` / `StandMapV2WitnessRunner` (`#if UNITY_EDITOR`) plays a V2 new game at 1920x1080 and 1280x720, reads `StandMapCanvas.LastPlan`, captures the screen and, as an informational observation, toggles Plan Racks | logic: no; runner: graphical Editor |
| `StandMapV2WitnessOfflineCheck.cs` + `run_v2witness_offline.py` | Plain .NET proof of the logic on the real production draw-plan code: the honest plan passes with exactly 10 Mini-only items / 30 points; the historic counting reproduces `matched=9 missing=10 of 19`, `points=212 of 242`; 17 defective plans (missing shared item, shifted point, wrong or missing guidance, headland on the minimap, ...) and one duplicate that the historic many-to-one match accepts all FAIL the witness | no (.NET SDK 8+) |
| `SectionFiveMaterialPair.cs` | Renders the Section Five flora three ways in one frame from one camera: in-memory material state built from the **committed** `.mat` bytes, the **generated** bytes (what the Unity Editor writes back), and the live Editor-session material; compares pixels. It never writes an asset | graphical Editor |

## Run

```bash
# offline (any time); --lane is a checkout containing the production map sources
python3 -I Tools/Verification/StandMap/V2Witness/run_v2witness_offline.py --lane <repo root> --probe Tools/Verification/StandMap/V2Witness --out <scratch>

# in the Editor (clean lane, machine-wide Unity lock held): stage, run, remove
cp Tools/Verification/StandMap/V2Witness/StandMapV2Witness.cs Assets/ForestPrototype/
CCF_TEST_DIRECT_ENTRY=1 CCF_DIAG_DIR=<folder outside the repo> CCF_UI_SIZES=1920x1080,1280x720 \
  Unity -projectPath . -executeMethod StandMapV2Witness.Begin      # pass: STANDMAP_V2_WITNESS_PASS
rm Assets/ForestPrototype/StandMapV2Witness.cs Assets/ForestPrototype/StandMapV2Witness.cs.meta
# material pair: also CCF_MATPAIR_HEAD_DIR / CCF_MATPAIR_GEN_DIR (seven committed / seven generated .mat files named <Material>.mat); pass: MATPAIR_CAPTURE_PASS
```

## What the witness does and does not prove

* It proves, for the **default V2 new game outside planning mode**, that the minimap items with no Stand Map counterpart are exactly the foundation's five dashed
  routes (each one `RackBand` of 4 points and one `RackSuggested` line of 2 points, `SourceIndex` -1, at the route's own place) and that everything else shared is at the
  same place on both canvases and at the independently computed place.
* It does **not** replace the historic gate, test confirmed-rack or planning-mode parity (the planning-mode toggle is logged, not asserted), or prove anything about colour,
  the forest picture or the cell buttons.
* The material pair proves only what it renders: three flora prefabs from `RecentAssetVisualCatalog`, staged at one place, under the project's own URP asset (MSAA off).
  Unity's `Material.SetColor`/`GetColor` does not read a colour back bit-exactly, so the five `_Color`-only materials are asserted to ~1e-6 and the exact texture/`_AlphaToMask`
  differences only for the two leaf materials.
