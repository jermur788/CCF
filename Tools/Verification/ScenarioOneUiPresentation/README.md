# Scenario One UI presentation checks

This is a Tier 1 source/type check, **not Unity runtime or visual qualification**.
The probe compiles exact `UiKit.MenuNavigation`, `UiKit.KeyPrompts`, inspection
paging/rank-hint and Annual Review headline methods against a plain element sink.
It verifies the menu routes and reused named controls, Rack-first suppression,
prompt semantics, forward/backward page reachability, selected-year facts and
explicit missing data. Five compiled mutations must fail their named assertions.
The sink has no forest, preferences, save or Unity window. It cannot prove USS
support, element geometry, pointer/focus, physical key delivery, performance or
correct capture provenance. Original regression verifiers remain unchanged.

```bash
SOURCE=/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Sol
EVIDENCE=/media/jer/Files/CCF/Verification/ScenarioOneUiPresentationV1Sol-20261110
python3 -B "$SOURCE/Tools/Verification/ScenarioOneUiPresentation/run_checks.py" \
  --src "$SOURCE" --out "$EVIDENCE/presentation-qualified"
```

The existing `ScenarioOneNonUi/compile_runtime_sources.py` needs **absolute**
`--src`, and a verified existing Player Managed assembly directory. It prints
`exit`/`errors` but does not propagate compilation failure as its own exit code;
inspect that output and require a produced assembly. The handoff records the
exact command and local Managed input. Compilation is syntax/type evidence only.

After compilation, run the unchanged `PlayerFeedbackOvernight/run_checks.py`,
`review_reset.py`, `mutations.py` and `mutations_370.py` as recorded in the handoff.
These cover source consumers and named negative guards, not actual save/load.
All output belongs on Files. No harness is copied under Assets for these checks.

Unity remains a separate conditional gate: explicit human Editor handback,
mounted/healthy ZX20, registered exclusive machine lease, exact warm-lane
candidate, protected licensed assets/Workbench/reference and independently
verified disposable persistent data/preferences/F5-F9 destinations. No Windows
build. Follow the full Manager packet; these instructions do not grant a lease.
