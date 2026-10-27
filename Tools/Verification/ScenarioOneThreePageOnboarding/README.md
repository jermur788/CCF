# Three-page onboarding verification

Explicit disposable Editor harness; never installed in a build. Tests only this UI feature.

Inspect `OnboardingVerification.cs` and `run_gate.py` before running. Acquire the normal machine-wide Unity lock on the clean, exact candidate first. The driver copies the harness into `Assets/ForestPrototype/`, launches Unity 6000.6.0f1 on a private Xephyr display with isolated XDG configuration, then removes the harness and generated `.meta` and copied licence. It unsets the direct-entry bypass so normal startup runs.

The harness refuses disk-save testing unless `Application.persistentDataPath` is inside this run's isolated `config/` directory. It refuses an existing save there. It creates one fresh save and runs the real startup Continue path. The user's normal save/preferences are not accessed. This is a callback/API test plus real panel hit-target checks and queued New Input System F1/Escape/arrow events; no OS-native mouse or keyboard delivery is claimed.

Coverage: normal startup; two supported resolutions; New Scenario opens Page 1; forward/back sequence and Begin; modal/player/cursor states; live exact titles; Page 2 generic teaching; Page 3 both species; old intro/detail hidden; all label heights; world hash unchanged by UI navigation; genuine Save/Continue with reset first-use preferences and unchanged world; session pause/restore both during onboarding and in the forest; contextual Help stays separate; no runtime errors/exceptions. Six actual Unity screenshots are required. No ecological year is advanced.

```bash
python3 Tools/Verification/ScenarioOneThreePageOnboarding/run_gate.py \
  --out /media/jer/Files/CCF/Verification/ScenarioOneThreePageOnboardingV1-20261025/run-01 \
  --display :84 --screen 2560x1440 --timeout 900
```

Success requires Unity exit 0, `ONBOARDING_PASS`, zero `ONBOARDING_CHECK_FAIL`, six screenshots and cleanup receipts. Then build six comparison sheets with `make_comparisons.py`, inspect each and record forestry fidelity separately. Compilation or functional assertions alone cannot qualify the visual implementation.

Unity may rewrite settings/materials and texture-importer metadata. Inspect every diff. The user's four original settings/material edits are separately preserved and may only be temporarily restored under the user's explicit 2026-10-25 approval; restore their exact bytes before handoff. Never discard unrelated changes or leave a disposable script or `.meta` in Assets.
