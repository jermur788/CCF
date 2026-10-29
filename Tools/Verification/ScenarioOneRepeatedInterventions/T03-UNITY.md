# S1-D T-03 Unity qualification

Run only in assigned registered U1 under the normal `Sol-S1D-T03` / `S1-D-T03`
lock at the exact clean candidate SHA. Unity 6000.6.0f1; no Player/Windows build.

```bash
python3 -B Tools/Verification/ScenarioOneRepeatedInterventions/run_teaching_cap_gate.py \
  --gate cap --project /media/jer/ZX20/CCF-U1 \
  --expected-head FULL_CANDIDATE_SHA --lock-id LOCK_ID \
  --out /media/jer/Files/CCF/Verification/RUN/cap
```

The runner stages source from this checkout into the separate Unity lane,
records its SHA-256, starts an isolated configuration/private Xephyr display,
and removes every staged harness/meta and copied licence. It retains patches
for known shared VS Code/three Forb material import rewrites before restoring
the original bytes. Unexpected lane changes, changed host save digests,
remaining disk saves, failed markers or nonzero Editor exits fail the receipt.
It refuses another task's lock, dirty/wrong HEAD, an occupied display and an
existing Editor. It does not acquire or release the lock or change a checkout.

`cap` exercises production Rack V2: five-rack confirmation, the field-marking
approval guard, marked access execution, three distributed Crop Trees, a
bounded mixed first release, review, and a later cumulative over-40% release.
Both Work Plan buttons and actual approval/advance methods are checked. No
cached teaching facts or completed work records are injected. The sample
analysis comes from the real live trees/orders. In-memory reload checks preserve
the advisory. Forced `ApplyMortality("self-thinning", year)` is a verification
fixture to shrink the living denominator, not an ecology/storm calibration claim.
Six actual removals near one Crop Tree leave the distant Crop Tree unreleased;
sold-only and short mixed variants are tested separately. Three Crop Trees and
one living non-Crop Tree are retained for the one-more-mark/plan checks.

An empty plan's Approve action normally returns false with "no valid pending
work". That normal no-op is checked separately from the cap refusal. Ordinary
outside-footprint work then proves approval succeeds while the footprint has no
actionable work. No production no-work behaviour is changed.

Hard/advisory states are captured at 1280×720 and 1920×1080 with message wording,
button states and bounds checked. Inspect the images as well; the harness waits
for first-use help before closing it, and asserts it is closed before capture.
Callbacks and renderer checks do not establish OS-native pointer delivery.
The T-04 guide index is logged diagnostically, never repaired by this gate.

Additional `--gate` values use unchanged existing harnesses:

- `lesson`, `menu`: explicit valid model-0 Enlarged80 fixture before `Start`,
  because these gates exercise the original curriculum. No production override.
- `teaching`: production Rack V2, rendered TeachingCopy.
- `rack`: production Rack V2 runtime lifecycle regression.
- `event-id`: integrated S1-D T-06 load/CanLoad/disk-load/no-mutation fixture.
- `history`, `history-browse`: protected S1-C T-03 and T-02 rendered fixtures;
  these exercise Start New themselves, so direct gameplay entry is unset.

`menu` also requires the accepted Stage 7 fixture, read without modification:

```bash
--menu-stage7-save /media/jer/Files/CCF/Verification/ScenarioOneMenuTutorialFixtureReconciliation-20261023T151109Z/fixtures/stage7-save.json
```

The path and SHA-256 are retained in the receipt directory. Do not regenerate
or modify a fixture or protected expected value to obtain a pass. Release the
normal lock only after all Editors/import workers exit and U1 is clean.
