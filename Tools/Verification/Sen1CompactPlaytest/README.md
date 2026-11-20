# Compact30 personal Editor test composition

Gameplay base: `8cb22de077b93b4c7af076e19936f3170d6ab48b`.
Existing visual delta: `0709d05a79756d3b72ccbd0452bd0252a75c3804` →
`5cbc26a4073ffa60db0a3c442e237eaa473ad3ff`, exactly 74 production paths.
The visual input includes both path/fence and spruce floor; no blank-starter source is imported.

Run `python3 Tools/Verification/Sen1CompactPlaytest/verify_composition.py` for read-only
exact-input, full protected-base, tree/GUID and serialized scene checks. Run the existing
`Tools/Verification/Sen1CompactE2Reuse/run_offline.py` for the 36 site-rule checks with TMPDIR
on authorised scratch storage.

Under an actual exclusive deployed Unity lease, copy
`Sen1CompactPlaytestVerification.cs` and the existing
`../Sen1CompactE2Reuse/Sen1CompactE2ReuseVerification.cs` into `Assets/ForestPrototype/`.
Run Unity 6000.6.0f1 through the authenticated non-root Hub sandbox with
`-executeMethod Sen1CompactPlaytestVerification.Begin`. The wrapper checks the actual
Editor menu helper, imported meshes/materials/components and scene structure, then runs
the unchanged E2 management/negative gate. It writes no save or scene and exits the Editor.
Remove exactly the two copied scripts and their generated `.meta` files after exit.
Never leave these auto-start harnesses in the owner's playable project.

For the personal GUI test, launch this exact project via the authenticated Hub context,
choose **CCF → Player Test → Play Compact30**, then focus the Game view. This menu explicitly
opens `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`; historical startup and
build settings are unchanged. Stopping Play discards this in-memory Compact session.
Compact persistence and full scenario completion remain outside the source grant.

Automated checks do not establish mouse/keyboard feel, rendered quality, owner acceptance,
historical save runtime compatibility or a release qualification. The task result records
actual outcomes and remaining gates separately.
