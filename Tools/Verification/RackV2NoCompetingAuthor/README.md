# RACK-V2-08 — no competing first-person author

Authority: `Docs/Handoffs/RACK-V2-08/G3-DISPOSITION.md` and the direct-user bounded intro amendment in `INTRO-G3-AMENDMENT.md`.

`run_gate.py` stages disposable sources into `Assets/ForestPrototype`, uses Unity 6000.6.0f1 and a private Xephyr display, then removes every staged script/meta and copied licence. Inspect it before running. The caller must hold the machine-wide Unity lock for a clean registered Unity lane at the exact candidate. Run this script from that lane's copy, not the authoring worktree. All `--out` directories must be under `/media/jer/Files/CCF/Verification/`.

- `--gate guided-legacy`: explicit pre-v21 rack-model-0 Enlarged80 save fixture, retaining the complete native B/origin/swivel/reselect/cancel/confirm sequence and all guided/sequential progression assertions. Enlarged80 geometry alone never selects this branch. The existing native driver requires private display `:84`.
- `--gate guided-v2`: rack-model-1 Enlarged80, native B/click/Enter/deferred-entry refusal, unchanged authoritative rack/work signatures, native Fell/Crop/inspection, current rendered guidance, immediate Stand Map replacement, real map reset/confirmation controls, then the same sequential progression/round-trip assertions through the authoritative V2 teaching geometry. Requires the explicit V2 partial companion; its absence fails the gate.
- `--gate combined-legacy` / `--gate combined-v2`: full CombinedHudComposition gate with explicit model fixture, retained legacy first-forest-rack premise versus immediate V2 Plan Racks plus authoritative map confirmation. All unrelated HUD, focus, inspection, layout and performance sections remain active; leave `CCF_COMBINED_SECTIONS` unset.
- `--gate legacy40`: genuine fresh geometry-0/rack-model-0 world, native first-use/B/origin/pointer/reselect/cancel/confirm and ordinary pending work, followed by real approval/year advance/execution.

Example (lock already held):

```
python3 Tools/Verification/RackV2NoCompetingAuthor/run_gate.py --gate legacy40 --out /media/jer/Files/CCF/Verification/<task>/legacy40
```

The full model-0 rack-first curriculum uses historical Enlarged80 because the production Legacy40 world intentionally has no authored rack-first layout. The separate genuine Legacy40 gate proves its available first-person workflow and annual progression rather than inventing an Enlarged80 curriculum in a 40 m stand.

The sequential gate's later regrowth assertion remains protected; any extra model-specific disposition must be explicitly recorded before changing it. No full objective qualification, asset production, save/schema change or Windows/Player build is authorised here.

Preservation: run unchanged `RackV2ConfirmMinimap/run_gate.py --gate confirm` for confirmation/reopen/minimap and its existing real-pointer checks. Offline: NonUi, RackV2Editing, RackV2Spacing, SuggestedLayout, StandMap and RACK-V2-07 static checks. Compile runtime sources against installed Unity refs and compile staged harness sources with `UNITY_EDITOR` before Unity.
