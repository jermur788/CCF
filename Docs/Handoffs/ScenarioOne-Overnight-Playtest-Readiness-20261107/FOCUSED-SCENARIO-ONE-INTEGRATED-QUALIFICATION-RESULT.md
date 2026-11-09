# Focused Scenario One Integrated Qualification Result — #315 — 2026-11-09

**Verdict: NEEDS CHANGES.** Unity opened the exact combined candidate and compiled it with no C# error. The Workbench retirement gate passed all 107 in-Editor checks. The run still does not qualify, for two reasons:

1. **A genuine runtime defect in the candidate.** `ScenarioOneUiRoot` creates a UI Toolkit `Label` in a field initializer (`ScenarioOneUiRoot.cs:58`). Unity 6000.6 forbids that. The constructor throws, and `ClearNextAction()` then throws a `NullReferenceException` on every `LateUpdate`.
2. **The Workbench runner's honest drift gate reported FAIL.** The only drift is the known Unity-generated `.vscode/settings.json` rewrite, which the packet says must not be waived.

Per the packet's stop-on-first-blocking-fault rule, the focused A–O stage was **not run**. This is not a user playtest readiness claim, and independent review #316 was not started.

## 1. Identity and provenance

| Item | Value |
|---|---|
| Executor | Claude Code, Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`) via T3 Code on the local machine. Reasoning effort was not exposed to the session. Actual Git and Unity control: yes. One writer, no orchestrator, no sub-agents. |
| Unity-tested candidate | `182a1e5d3d4e65c87607ccfbed4d5036db3ef49d`. Production source `7dc327903d62e3ab14a09527cae01ab48e1659a6`. `git diff --name-status 7dc32790 182a1e5d` is exactly one added Markdown file (the #355 report). |
| Unity | 6000.6.0f1 (`f7f8ed4d1e24`), `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` |
| Lane and branch | Registered `/media/jer/ZX20/CCF-U3`. New local branch `qualify/scenario-one-focused-playtest-20261109-v1`, created non-forced at exactly `182a1e5d`. |
| Original U3 state | HEAD `52bb32a6` on `fix/reference-preview-player-restore-20261109-v1`. Tracked state was exactly the two known generated `.vscode/settings.json` hunks. |
| Files staging branch | `integration/scenario-one-focused-playtest-20261109-v1`. Remote tip was `182a1e5d` before and during Unity. Worktree clean and untouched during Unity. |
| Lock | Unchanged deployed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`. Lock id `57ce789d08a046578f733f785b6114fd`, task id `CCF-315-SINGLE-CANDIDATE-QUALIFICATION`. Acquired 15:04:27Z, strict-clean, `--head 182a1e5d`. Released owner-safe with that lock id, `forced: False`, status `free`. |
| Editor process | One Editor, pid 187623, about 135 s. No Editor process remains. |
| Disk | Fresh 36.16 GiB free on ZX20 at entry (floor 35 GiB). Quiet I/O (0.0 MB/s). Real monitor at 20 s intervals started before Unity, with 28 warn, 25 owner-stop and 20 absolute thresholds. 13 samples, minimum 36.16 GiB, no alert. Final 36.17 GiB. The monitor stopped cleanly. |
| Licence | Used the Unity Hub Flatpak entitlement file dated 2026-11-09 14:42Z through the runner's isolated scratch copy. Editor exit 0. Scratch copy removed by the runner (SHA prefixes only in the receipt). |

### Preflight mutation actually performed

The only change to the lane before the lease was one worktree-only `git restore --source=HEAD --worktree -- .vscode/settings.json`.

- It was made after saving the before-image.
- The before-image is `entry/vscode-before.diff` and `entry/vscode-before.settings.json`, SHA-256 `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb`.
- It matched exactly the two known hunks: `"*.unity": "yaml",` comma and `CCF.slnx` to `CCF-U3.slnx`.
- Nothing else was restored, staged, reset, stashed or cleaned.

## 2. Compile and Workbench C–G

| Gate | Result | Evidence |
|---|---|---|
| Unity import and compile (Tier 2, first) | **PASS.** Script compilation 15.5 s, asset pipeline refresh 29.2 s on the warm Library (no cold import). No `error CS` line and no "compilation failed" in the Editor log. The `StandMapSource` typed `SceneHandle?` source compiled. Package Manager resolved 65 packages from cache with no resolve drift. `Packages/manifest.json` and `packages-lock.json` are unchanged and tracked clean. | `run/workbench/WorkbenchRetirementVerification.log` |
| Workbench retirement gate (new game retired, old saves keep identity and access, deterministic regeneration, Site masks 6 and 7, Reference preview restore, scene reload, null and empty buildables) | **In-Editor assertions PASS.** 107 `WORKBENCH_PASS` lines, no `WORKBENCH_FAIL`, one terminal `WORKBENCH_RETIREMENT_VERIFY_PASS checks=107`. | log, `*.markers.txt` |
| **Runner verdict for that gate** | **FAIL, runner exit 1.** The single failure is `postrun_lane_drift:1`. The drift is `.vscode/settings.json`, modified, SHA-256 `2ccabfd4…c2c1c`. That is byte-identical to the known pre-restore file and the diff equals `entry/vscode-before.diff`. The drift gate was left strict as instructed and was not restored, waived or bypassed. | `WorkbenchRetirementVerification.json` |
| Enlarged80 and RackV2 persistence runner gates, Legacy40, frozen Reference Future v1, Site-mask and saved-teaching-flag gates beyond the Workbench harness | **NOT RUN.** Stopped before them (section 3). | none |

The historic #287 Workbench FAIL is unchanged and is not erased by this attempt.

## 3. The blocking defect (new in this candidate)

The Editor log contains these exceptions. The #353 C2 log for the same lane (`Build/Enlarged80/c2-plain-1.log`) contains none of the first two.

1. **`UnityException: VisualElementCreation is not allowed to be called from a MonoBehaviour constructor (or instance field initializer), call it in Awake or Start instead.`**
   - Seen twice, at `ScenarioOneUiRoot..ctor` (`Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:58`).
   - It was triggered from `ScenarioOneManager.Awake()` at `ScenarioOneManager.cs:460` (`AddComponent`).
   - Line 58 is `private readonly Label nextBadge = new Label("Next:") { … };`.
   - `git blame` attributes the line to `8fc4576f` (the #354 transfer of the approved #295 next-action cue source). `ScenarioOneUiRoot.cs` differs from C2 `b82cb240` by 193 added lines.
2. **`NullReferenceException` at `ScenarioOneUiRoot.ClearNextAction()` (`ScenarioOneUiRoot.cs:470`).**
   - 34 occurrences, all from `ApplyNextActionGuidance()` ← `LateUpdate()` (`:327`).
   - That is `nextBadge.RemoveFromHierarchy()` on a field that was never initialised. The cause is inferred from the stack, not isolated.
3. `ArgumentOutOfRangeException` in `UnityEditor.Search.SearchDatabase.GetDefaultSearchDatabase`. This is a Unity Editor internal. C2 has one too. It is not attributed to the candidate. Also `VBCSCompilerPreWarmer … /tmp` is environmental noise.

Meaning: the new Objective next-action guidance (A–O gates I and J, the CTA behaviour) cannot work as designed in the Editor, and the UI root may be only partly initialised. The Workbench harness does not exercise that UI, so its 107 checks are unaffected. It also shows why the earlier offline checks stayed green: the real-DLL compile only proves the code *binds*, not that Unity allows it at runtime.

Not verified:

- Whether the same exception occurs in a Player build.
- Whether it occurs with default Play Mode options. The Editor log says it entered Play Mode "with Reload Domain disabled" and "Reload Scene disabled".
- Which exact visible UI is lost.

No production code was changed.

## 4. A–O table for this exact candidate

| Gate | Status | Note |
|---|---|---|
| A Unity import and compile | **PASS** (editor compile only) | Section 2. No separate Player-script compile. |
| B V2 surface lifecycle | NOT RUN | |
| C Map geometry (Full Stand Map vs 3D headland and southern feeder) | NOT RUN | |
| D Full vs Mini invariance | NOT RUN | |
| E Legend and paths | NOT RUN | |
| F Performance and GC | NOT RUN | |
| G Five-rack diagram (real drawn figure) | NOT RUN | |
| H Diagram old-world guard (Legacy40, Reference v1, pre-V2) | NOT RUN | |
| I Card fit at 1280×720 and 1920×1080, colour and grayscale | NOT RUN | |
| J Native controls and safe default CTA | NOT RUN, **known blocked** by section 3 | |
| K Rack proposal vs Fell mark vs Work Plan | NOT RUN | |
| L Workbench retirement | **In-Editor assertions PASS (107/107); runner verdict FAIL on `.vscode` drift** | Section 2 |
| M Static feedback checker integrity | NOT RUN | #355 offline results remain source antecedents only. They are not claimed green for the combined tree. |
| N Save and load compatibility | **Partly** | The Workbench harness's old-save, Legacy40-style and null-buildable load checks passed inside its 107 assertions. The wider Legacy40, Enlarged80 V2, Reference v1 and Site-mask gates were not run. |
| O Final integrated regression | **NOT RUN / FAIL** | Blocked by section 3 and the runner FAIL |

No screenshots were captured. Nothing here is inferred from source.

## 5. Preservation and drift

- **Protected ignored pack** `Assets/InnerverseInteractive/` plus `.meta`: 27 of 27 files, SHA-256 identical before and after (`entry/pack-baseline.sha256`).
- **User saves** under `~/.config/unity3d/DefaultCompany/CCF/`: hashes identical. The runner used an isolated `XDG_CONFIG_HOME` on Files.
- **Original registered warm Workbench** `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`: HEAD `4d95e452`, the same eight dirty tracked files before and after (`.vscode`, five SectionFive materials, `Packages/manifest.json`, `Packages/packages-lock.json`). Not switched, cleaned or used.
- **Candidate bytes** after the run (`run/post-hashes.txt`): `manifest.json` `f2ff5f95…`, `packages-lock.json` `f43d9894…`, `ForestTest.unity` `6b1c4760…`. These are the committed bytes and git reports no change to them.
- **U3 end state:** on `qualify/scenario-one-focused-playtest-20261109-v1` at `182a1e5d`, **not clean**. Tracked drift is exactly `M .vscode/settings.json`, the same two hunks, preserved as evidence. No untracked file. The staged harness and its `.meta` were removed by the runner. `Library` kept warm. No Player build, no main merge.
- **Files staging worktree:** clean before this report. This report is its only addition, so this report-only commit is *not* the Unity-tested source HEAD.

## 6. Reproducibility

Evidence is in `/media/jer/Files/CCF/Verification/CCF-315-SingleCandidateQualification-20261109/` (`SHA256SUMS.txt` covers it). Key files:

- `holder.sh`: entry checks, disk monitor and lock acquire.
- `run_wb.sh`: the runner command.
- `entry/`: before-images and baselines.
- `run/holder.log`, `run/disk-monitor.log`, `run/unity-lock-*.txt`.
- `run/workbench/`: log, markers and JSON receipt.

The gate command, run in the lane under the lease:

```
python3 -I Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py WorkbenchRetirementVerification \
  Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs <evidence>/run/workbench \
  --expected-head 182a1e5d3d4e65c87607ccfbed4d5036db3ef49d --expected-branch qualify/scenario-one-focused-playtest-20261109-v1 \
  --owner "Claude Code (Sonnet 5.5) via T3 Code #315" --lock-id 57ce789d08a046578f733f785b6114fd \
  --pass-token WORKBENCH_RETIREMENT_VERIFY_PASS --task-id CCF-315-SINGLE-CANDIDATE-QUALIFICATION
```

Receipt verification: `RUNNER_RECEIPT_STATE FINAL runner_exit=1`, JSON SHA-256 `674dac0d648550c3888c0f8b052f8523fdb82ba874d02a6e172f959e11088bec`, `exit_code 1`, `failures ["postrun_lane_drift:1"]`.

Process note: my holder script recorded the wrong 32-hex string in its own `lock-id.txt` (a prefix of the commit hash). Its automatic release therefore failed with "no lock to release". I released manually with the correct id first (`run/unity-lock-release-manual.txt`), and the holder only stopped the monitor. The lock state ended `free`.

## 7. Severity and recommended next step (Manager's decision)

| Severity | Item |
|---|---|
| **Blocking** | `ScenarioOneUiRoot` field-initializer `Label` throws in Unity 6000.6, then a null `nextBadge` throws 34 times in `LateUpdate` (section 3). A small fix would construct the badge in `Awake` or on first use, without changing behaviour. It needs a Manager-approved bounded source repair, not a waiver. |
| Process | Unity rewrites tracked `.vscode/settings.json` on every launch, so the strict runner drift gate always reports FAIL on a lane that takes the hunk-restore exception. A Manager policy is needed (for example, accept this exact known generated diff in the gate, or run with the file restored by an approved step). I did neither. |
| Not run | Everything in section 4 marked NOT RUN, plus the Enlarged80, RackV2 persistence, Legacy40 and frozen Reference gates. |

After the source repair, one fresh lease would be needed to repeat the compile, Workbench and A–O stages. I did not reproduce the exception outside this one run, so it is a single observation of a clear mechanism.

**Stop for Manager #200.** #316 was not started.
