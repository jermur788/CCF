# Scenario One UX patch: Unity qualification (source `84e881a`)

Start with `Handoff.md`. Evidence is in `evidence/`; the 75 screenshots (25 states x 1280x720, 1920x1080, 1024x768) stay in the ignored `Build/` folder of the worktree (see `evidence/ui-screenshots-index.md`).

| File | What it is |
|---|---|
| `evidence/gate-results.md` | the six required gates (and the two extra ones) with the run that counts |
| `evidence/harness-checks.txt` | all 92 checks of the fresh-player harness (reset flow, cue order, guard paths, review, controls, copy, input, preservation) |
| `evidence/perf-cpu-only.txt` | CPU-only timings and allocation (frame timing is invalid on the software-rendered display) |
| `evidence/text-sizes.csv` | measured physical text sizes per state and window size |
| `evidence/startup-gate-diagnosis.md` | why the rendered startup gate fails, with the A/B on the pre-patch code |

Tools in `Tools/Verification/ScenarioOneUx/Qualification/`: `UxQualification.cs` (disposable harness), `run_qualification.py` (stage / run / clean), `make_startup_diagnostic.py`. Interactive runs need a display where focus cannot be taken from the Editor (a nested X display worked; a shared desktop did not).
