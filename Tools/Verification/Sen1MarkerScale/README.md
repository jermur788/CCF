# Sen-1 #876 marker scale gate

Disposable Unity gate for F-1: Crop Tree (C) and Fell (X) mark visuals were children of
Compact30 Sitka roots scaled about 10-14x, so the 1.2 m ground disc became about 14 m wide and the
diamond floated far above the tree. The fix in `ForestTreeMarkingManager.CreateMarker` cancels the
tree root's world scale on the mark root.

## Run (exclusive Unity lease only)

1. Copy `Sen1MarkerScaleVerification.cs` into `Assets/ForestPrototype/`.
2. Launch Unity 6000.6.0f1 for the lane with a task-local `XDG_CONFIG_HOME`, `-batchmode`,
   `-executeMethod Sen1MarkerScaleVerification.Begin` and:
   - Compact: `SEN1_MARKER_MODE=compact`.
   - Historical: `SEN1_MARKER_MODE=historical CCF_TEST_DIRECT_ENTRY=1` with
     `CCF_STAND_GEOMETRY=0` (Legacy40) or `1` (Enlarged80).
   - `SEN1_MARKER_OUT=<evidence dir>` receives `markers.csv` and `verdict.txt`.
3. Exit code 0 and `SEN1_MARKER_VERIFY_PASS` in the log mean every check passed.
4. Remove the copied script **and** its generated `.meta`. Never leave it in a playable project.

The gate writes no scene and calls no save API. Root scales it changes (unit and 6x14x9
non-uniform cases) exist only in play mode.

It checks, per mark: world scale 1, 1.2 m ground disc at ground level, small diamond 0.6 m above
`tree.Height`, blue chest-height trunk band only on Crop Trees, no colliders, the requested mark
state, and that unmarking destroys the visual. In Compact it also checks three Crop Trees (count 3),
repeated unmark/re-mark, and that a mark follows a moved tree. In historical worlds it checks that every
tree root is unit-scaled and that the mark local scale is exactly (1,1,1), as before the fix.

It does not prove rendered appearance, player input, year advance, save/load or owner acceptance.
