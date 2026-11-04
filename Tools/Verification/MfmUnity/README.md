# MFM-04 / MFM-05 Unity compile and non-mutation gate

Authoring-only escalation: Claude Sonnet was rate-limited; OpenAI GPT-6.1 Sol authored this gate. Base `01a28364fc442937d367077f0cd67d45e87069e7`, branch `qual/mfm-04-05-unity-v1`, inspected context commit `63e95ba2175f7e239a278c616681b286719821f8`. MFM-04 reviewed `1b6ba3e` and MFM-05 reviewed `4b54af0` are on the transplanted T-03/T-05 lineage. This tooling changes no production code or calibration. A pre-existing untracked task draft was inspected and revised; its original Git blob is `603c7a6db39b04f1b3459d7a60bf60c7cfd0196a` (stored locally with `git hash-object -w`; recover with `git show <blob>` while retained by Git).

**No Unity run or lock acquisition was performed during authoring.** Source/API inspection and Python syntax checks are not a Unity compilation pass. Independent G3 review and actual U1 execution remain required. The parent/integrator owns coordination, evidence publication and dashboard synchronization; this authoring task does not push.

## Later execution on U1

The caller first integrates this tooling onto the intended candidate and prepares a clean `/media/jer/ZX20/CCF-U1` at that exact full SHA. The caller must already hold the machine-wide lock with `task_id=MFM-04-05`, a nonempty assigned owner, `lane=/media/jer/ZX20/CCF-U1`, and `head` equal to the candidate SHA. Keep the lock until the runner returns. This runner only queries the lock; it never acquires/releases it or kills another Editor. Do not use it while T-05 holds the lock. No Windows build.

Run in a shell on the prepared U1 lane (the lock ID must come from the caller's already-held lock):

```bash
python3 -B /media/jer/ZX20/CCF-U1/Tools/Verification/MfmUnity/run_mfm_unity_gate.py \
  --project /media/jer/ZX20/CCF-U1 \
  --expected-head <FULL_CANDIDATE_SHA_INCLUDING_THIS_TOOLING> \
  --lock-id <ALREADY_HELD_MFM_04_05_LOCK_ID> \
  --out /media/jer/Files/CCF/Verification/MFM-04-05-Unity/<UNUSED_RUN_ID>
```

Replace all angle-bracket values. `--preflight-only` checks ownership, source identity, project version and storage without staging or launching Unity. Default private Xephyr display is `:86`; choose another unused display with `--display :87` if necessary. Requires the installed Unity 6000.6.0f1, Xephyr, openbox, an existing local Unity entitlement and at least 15 GiB free on U1's volume. There is no package installation or build step.

The runner refuses a dirty/wrong lane, wrong HEAD/lock/task, running Editor, occupied display, mismatched candidate harness/runner, or reused evidence directory. It stages only `MfmUnityCompileCheck.cs` into `Assets/ForestPrototype/`, sets isolated `XDG_CONFIG_HOME`, `/tmp/` for licensing sockets and `CCF_DIAG_DIR`, removes inherited `CCF_*` scenario overrides, then launches the exact Editor with `-executeMethod MfmUnityCompileCheck.Begin`. Compilation errors fail even if the entry method never starts (missing PASS or timeout also fails).

## Measurement contract

- Open `Assets/Scenes/ForestTest.unity` in play mode and wait for `ScenarioOneUiRoot`, its manager, ecology and save controller.
- Require the complete reviewed calibration signature, including `MFM03-CAL-1` / `MFM-V1-RULE-1`.
- Before and after each MFM call, capture `ScenarioReferenceArchive.WorldHash(saves.CaptureData())`, complete scenario JSON, tree count/marks and work orders. Assert equality in the same frame, including approval status and persisted scenario state. This is an intra-call identity check, not a cross-run hash anchor.
- Classify geometry/live stems/orders; repeat to check determinism. Exercise suggestions and both inspection-target APIs, including invalid IDs and traversal boundaries. An inspection target must remain an open spacing-only suggestion; it must not mark/designate, approve or schedule anything.
- Exercise `EvaluateSafeguard(null, stems, nonEmptyProposalIds, 0.4)` and require `InvalidInput && !AllWithin`. Also exercise valid coverage with empty removals, one covered stem and an unknown target, and an invalid coverage object.

### Live-input strategy

The fresh production world has no confirmed rack plan. Before the measurement baseline, the harness finds the five live entrance trees used by the qualified MFM-02 fixture (`PO0203 PO1204 PO2003 PO2803 PO3803`), checks their z rows against that fixture, calls the existing `AddRack` / `ConfirmRackPlan` APIs and derives geometry from `manager.ConfirmedRacks`. This creates pending access orders only during labelled setup; no work is approved or executed. If a confirmed plan already exists, it is used directly. Draft partial plans fail rather than silently substituting geometry. `ScenarioOneRackBayReader.Read` cross-checks the same confirmed sample.

Stems use live IDs, world positions, diameters, species, living/stump state and Crop/Fell flags; felling orders use the same conversion as `ScenarioOneRackBayReader`. Infrastructure masks, management features and manual area choices are empty because this is a deliberately partial adapter. Calibration is unchanged. Valid coverage and at least one candidate/covered target are mandatory for this fixture; a skipped positive path cannot pass. After all checks (or a caught failure), `LoadData(originalCapture, false)` restores the original in-memory world and identity is checked again. Restoration is outside the MFM measurement baseline.

All setup/capture/restore is in memory. Never call `Save`, `Load`, or write `forest-save.json`. The process uses an isolated persistent directory. The runner hashes the normal host save/bak/tmp before and after, refuses a pass if a save appears in the isolated config, and removes its copied licence.

## Evidence and cleanup

`process-result.json` must report `pass: true`, Unity exit 0, a logged `MFM_UNITY_PASS`, no compilation/check failures, unchanged host saves, no disk saves, removed harness/meta/licence and no remaining lane changes. `unity.log` contains each check and labelled input provenance; `command.json` records candidate, lock and harness hash. All evidence/config output stays under `/media/jer/Files/CCF/Verification/`.

The runner retains a patch of known import side effects, restores only the original `.vscode/settings.json` and SectionFive material bytes, and removes only its staged harness/meta. Unexpected changes fail and remain for inspection. Timeout, SIGTERM and Ctrl-C trigger cleanup. SIGKILL, power failure or a filesystem failure cannot guarantee cleanup: inspect the lane and manually remove only this task's staged `Assets/ForestPrototype/MfmUnityCompileCheck.cs` and `.meta` before releasing it. Never leave the auto-start harness in a playable build.

## Limits

This is a focused compile/API/non-mutation gate, not MFM integration acceptance. MFM-04/05 add no rendered UI. It does not execute a forestry prescription, approve a proposal, test save/load persistence, validate offline digests, prove CI parity, qualify cross-year drift, or establish ecological/calibration validity. The full production infrastructure-mask/management-feature adapter and post-Lesson-5 state are outside this task. No protected anchor is rebaselined. The fixture/restore lifecycle, actual Unity type binding, import behaviour and platform persistent-path isolation remain unverified until the later locked U1 run. A fixture or restoration mismatch fails; it must not be hidden by relaxing assertions. Ordinary SIGTERM/Ctrl-C cleanup is supported; force termination remains a manual inspection risk.
