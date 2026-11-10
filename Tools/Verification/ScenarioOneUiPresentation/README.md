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

#374 repair consumer checks (additive; earlier checks unchanged):

```bash
python3 -B Tools/Verification/ScenarioOneUiPresentation/repair_consumers.py --src "$PWD" --out /media/jer/Files/CCF/Verification/ScenarioOneUiPresentationRepair374-20261110/consumers
```

Runs the actual `LockedCard` and inspection `ControlHints` consumer statements in a plain C# element sink, including disabled-note visibility and eight candidate/cursor/Rack-first combinations. Twelve additional source contracts pin caller wiring, selected-year/history guards, Year/Cash strip, nonmarkable predicates, separate Approve/Advance, section identity, warnings and occlusion. Ten negative controls (three executed consumer mutations, seven source-contract mutations) must fail for their named reason. These are deliberately distinguished: source contracts do not execute AnnualReviewView.Refresh, marking eligibility, HUD refresh or approval. No Unity layout, input, scrolling, save/load or forest behavior is proved. The older 157 checks remain unchanged, including their repeated page-wrap checks.
