# #356 — Next-action UI lifecycle fix and one controlled Unity requalification — RESULT

**Date:** 9 November 2026. **Executor:** Claude Sonnet 5.5 (`claude-sonnet-5-5`) in Claude Code inside T3 Code, local; one owner for both serial phases.
**Binding packet:** `MANAGER-315-TO-356-UI-LIFECYCLE-DRIFT-POLICY-AND-ONE-UNITY-REQUALIFICATION-20261109.md`. **Claim:** [#356](https://github.com/jermur788/CCF/issues/356).

## Verdict: **BLOCKED — STOPPED AT THE DRIFT-POLICY BOUNDARY. NOT YET READY FOR INDEPENDENT #316.**

* The #315 blocker **is fixed and proven in a real Editor**: 0 `VisualElementCreation`, 0 `NullReferenceException`, 0 `ClearNextAction`, 0 `error CS`, and the `Next:` badge renders.
* Workbench **107/107 in-Editor PASS**, but the **strict runner FAILED (exit 1)**; that stays a FAIL. The sole runner failure is the known two-hunk `.vscode/settings.json` rewrite (classified separately below, not relabelled).
* One focused gate, **Objective 1 end to end, PASSED (173 checks, 24 native captures)**.
* I then **stopped**: that Editor run also rewrote **five Section Five `.mat` files**. The Manager policy only authorises restoring the exact `.vscode` hunks and says "any other tracked change → STOP". No restore of anything else was made. The rest of A–O is **NOT RUN**. No production fault was found.
* #287 Workbench remains **NOT FULLY QUALIFIED** (runner FAIL). No main merge, no #316, no Player build.

## 1. Identities

| Item | Value |
|---|---|
| Phase A worktree / branch | `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, `integration/scenario-one-focused-playtest-20261109-v1` |
| Original tip (report-only) | `0925dea90817c24bd31f27114459fb786a50e8b5` (parent, Unity-tested #315 candidate: `182a1e5d3d4e65c87607ccfbed4d5036db3ef49d`) |
| **Exact tested source commit** | **`045966d2f8e26368e30a79f14a1e34167952b95e`** (pushed; `ls-remote` equal before Unity) |
| This report | report-only successor of `045966d2` (3-file source diff above; no source file changes after it) |
| Unity lane | `/media/jer/ZX20/CCF-U3`, new local branch `qualify/scenario-one-ui-lifecycle-20261109-v2` @ `045966d2` (non-forced `git switch -c`); old `qualify/scenario-one-focused-playtest-20261109-v1` still `182a1e5d` |
| **Actual lock ID** | **`570114e72b784531953b6d34623507c0`**, parsed from the JSON metadata and re-verified with `status --json` (owner, task `CCF-356-UI-LIFECYCLE-REQUALIFICATION`, lane, branch, head all matched) |
| Lock held / released | acquired 15:48:03Z, released 15:57:16Z by that ID (`release exit=0`, `forced: False`); `status` then `free`, no Editor |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-356-UiLifecycleRequalification-20261109/` (`evidence-manifest.sha256`, 127 files, 55 MiB) |

## 2. Phase A — source and pin (offline)

Diff from `0925dea9` is **exactly three files** (+43/−5): `ScenarioOneUiRoot.cs`, `run_headland_check.py`, `test_stage_inventory_guard.py`.

**Production edit (the two Manager-approved changes only):**
```diff
-    private readonly Label nextBadge = new Label("Next:") { pickingMode = PickingMode.Ignore };
+    private Label nextBadge;
 ...
     private void Start()
     {
+        nextBadge = new Label("Next:") { pickingMode = PickingMode.Ignore };
         manager = GetComponent<ScenarioOneManager>() ?? ...
```
UI root blob `cf5707e3…` → `3d3b79c4…`; SHA-256 `cf5c4c36040645b84bc4b633d7b82453a364ebcc54571c1df6ee48e18376adfc`. `ClearNextAction`, `ApplyNextActionGuidance`, `LateUpdate` and everything else are untouched. (`nextBadge` is referenced only at the two existing sites; `Start` runs before the first `LateUpdate`.)

**Integrated-only pin** (`--integrated` mode; stage mode, the #297/#296 inventory and the #355 `StandMapSource` pin are unchanged):
* approved D-072 root stays recoverable at `9789369d` as blob `cf5707e3`; today's file must equal that plus exactly the two edits, and the pinned SHA-256 and blob id;
* a semantic rule rejects a badge created in a field initializer;
* the other D-072 UI files are now **nine** byte-identical files; the final "protected vs BASE" list drops the UI root **only in integrated mode** (it is pinned instead).

| Check | Result |
|---|---|
| CTA `check_baseline.py --rotation-state post-fix --instruction-state post-fix` (SDK 8.0.133 pinned cwd, real Unity 6000.6 DLLs) | **PASS=10 FAIL=0 NOT-TESTABLE=2**, check 08 PASS with `0 compile errors` (this is the one real-DLL compile; no separate one run) |
| `run_headland_check.py --integrated` | exit 0, **49** `PASS SOURCE` (42 + 7 new) and `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0` |
| `test_stage_inventory_guard.py --integrated` (sparse clone at `045966d2`) | **10 mutations** (8 existing + `i9` root changed by a space, `i10` badge back to field initializer), 2 clean runs, FAIL=0; each caught by its intended assertion |
| Historical stage guard on the original fixture (`--commit 9789369d`, edited runner) | 20/20 mutations caught, clean runs `39` source / `92/92`: **historic stage mode unchanged** |
| Stage mode on the combined tree | exit 1 at `stage 1 … StandMapSource.cs`: **expected stage-scope mismatch, still not a pass** |

The 884-negative/112-benign campaign, five-rack stage suite and `check_sources.py` were **not** re-run (no new discrepancy). Logs in `phaseA/`.

## 3. Phase B — Unity (U3, one lease, two serial Editor sessions)

**Entry preflight (all passed before any change):** ZX20 36.17 GiB free (≥35), I/O 0.0 MB/s, 20 s monitor (28/25/20 GiB) started and confirmed sampling, lock free, no Editor, U3 on the old qual branch @ `182a1e5d` with sole dirt = `.vscode/settings.json` (SHA-256 `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb`, **full value equal** to #315's; `git diff` byte-identical to #315's; HEAD blob SHA `04d629e4…`), nothing staged/untracked, licence pack 27/27 hashes OK, saves OK, original Workbench worktree's eight dirty entries unchanged, Hub licence dated 14:42Z. Before-image + diff kept; **one-path** worktree restore to HEAD; lane clean; then the new branch at `045966d2`.

### 3.1 UI startup gate — **PASS**
From the first Editor session's log (`run/workbench/WorkbenchRetirementVerification.log`):

| Pattern | Count |
|---|---|
| `VisualElementCreation` / `is not allowed` | 0 |
| `NullReferenceException` / `ClearNextAction` | 0 (#315: 34 NREs) |
| `error CS` | 0 |
| other `Exception` | 1: `ArgumentOutOfRangeException` inside Unity's `SearchDatabase.EnumerateAll` (`SearchInit.IndexationOnStartup`), engine code, **also present in the #315 log**; not CCF code |

The real UI root exists and is driven (the harness resolves `ScenarioOneUiRoot` and uses it for 107 checks; the startup screen opened). It was **not** a separate pre-Workbench Editor launch: the Workbench session is the first Editor session and was gated on this scan before anything else ran. Capture `c01…1280x720` and `c05…1920x1080` (Objective 1 gate) show the `Next:` badge rendered beside the primary button: the object that crashed in #315.

### 3.2 Workbench — two distinct verdicts, both kept
* **In-Editor behavioural: 107 `WORKBENCH_PASS`, `WORKBENCH_RETIREMENT_VERIFY_PASS checks=107`.** Editor exit 0.
* **Strict runner: FAIL, exit 1**, `failures: ["postrun_lane_drift:1"]`, `RUNNER_RECEIPT_STATE FINAL runner_exit=1` (receipt JSON SHA-256 `444c3b58…e4103f`). The runner was not edited, bypassed or suppressed. This is **not** a runner PASS, not strict-clean and does not qualify Workbench or Tier 2.

### 3.3 Environmental classification of the `.vscode` drift (separate from the runner verdict)
The contract was met **exactly**: the runner's own `postrun_drift` lists only `.vscode/settings.json`; its after-SHA-256 is the full `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb`; `git diff` is byte-identical to #315's two hunks (the YAML trailing comma and `CCF.slnx`→`CCF-U3.slnx`); the Packages manifest/lock and scene hashes equal #315's post-run values and HEAD; nothing staged; no Editor C# error. → **`KNOWN EDITOR-GENERATED U3 SETTINGS DRIFT`** at that point. Used only to continue testing. One per-iteration restore (receipt `01`, `run/vscode/01-*`, before-SHA `2ccabfd4…`) ran after Unity stopped, via a guard script that refuses unless that file is the sole dirt, nothing is staged and the SHA matches. No `skip-worktree`, `clean`, runner change or hidden rollback.

### 3.4 Objective 1 end-to-end gate — **PASS**
`RackV2Objective1/run_gate.py` (copied with only `TASK` changed to this task's lock task id, as the repo's README prescribes; lane via `CCF_GATE_LANE`), `--native --normal-startup`: **`RACKV2_O1E2E_PASS checks=173 shots=24`**, exit 0, 304 s, 0 `VisualElementCreation`/NRE/`ClearNextAction`/`error CS`, no leftover staged files. Real OS key/click events: Start New Scenario → Plan Racks → edit → confirm five racks → field-mark five entrances → Work Plan approval (refused before marking; marking approves nothing) → advance year → Annual Review → Objective 2 Crop Tree → real `Save/CanLoad/Load` with exact rack/order signatures. 24 PNGs (twelve states × 1280×720 and 1920×1080, colour only) in `run/gate-objective1-e2e/evidence/`. I viewed `c01` and `c05`: the five-rack lesson diagram, the Full Stand Map with the existing V2 headland/feeder legend entry, and the `Next:` badge. This gate also has one benign `BadWindow` X error line from the private display driver.

### 3.5 **New drift that stopped the run**
After that gate `git status` shows `.vscode/settings.json` (same SHA `2ccabfd4…`) **plus five tracked `.mat` files** under `Assets/ForestPrototype/Art/SectionFive/` (`Bilberry_Leaves`, `Bilberry_Stems`, `Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`; 9 insertions/9 deletions, e.g. `_MainTex` texture reference restored, `_AlphaToMask` 0→1). These are the known Unity re-serialisation side effects, and **all five are byte-identical to the same files in the protected original eight-dirty Workbench worktree** (`run/drift-after-gate2/materials-vs-workbench.txt`), so deterministic and Editor-generated. They did **not** appear in the Workbench session; they appeared in the Objective 1 session. The packet's drift policy authorises restoring only the exact two `.vscode` hunks and says to **STOP** on any other tracked change. I did not restore, reset, or run any further gate. Packages manifest/lock and scene tracked-identical; nothing staged or untracked; Library intact (6.6 GiB).

## 4. A–O matrix on exact `045966d2` (PASS / FAIL / NOT RUN; nothing carried from other candidates)

| Gate | Result | Evidence / note |
|---|---|---|
| A Unity import/compile | **PASS** | 0 `error CS` in both Editor logs; `SceneHandle?` source (#355) compiled in Unity 6000.6; Packages/scene tracked-identical |
| B V2 surface lifecycle | **NOT RUN** | V2 world and headland legend observed in the Objective 1 gate, but no V2↔non-V2 transition test |
| C Map geometry vs 3D | **NOT RUN** | no paired map/3D capture or dark-edge pixel check |
| D Full vs Mini invariance | **NOT RUN** | offline headland only (source-level, 92/92) |
| E Legend / paths | **NOT RUN** | legend visible in `c05` colour only; no grayscale or observer test |
| F Performance | **NOT RUN** | |
| G Five-rack diagram | **NOT RUN** | diagram visible at 1280×720 in `c01`; no byte-pin checker run on the combined tree (stage checker fails by design), no grayscale |
| H Old-world diagram guard | **NOT RUN** | |
| I Card fit / guidance | **NOT RUN** (partial observation only) | 24 captures at both sizes exist; `GuidedOnboardingVerification`/`UxPedagogyShots` and measured card bounds were not run |
| J Native controls / safe focus | **NOT RUN as a full gate**; Objective 1 route native input **PASS** | real keys/clicks pass in the 173 checks; Help-close frame suppression, focus order and preview suppression untested |
| K Work Plan / rack boundary | **PASS for the Objective 1 route only** | approval refused before marking; marking approves nothing; approval = exactly the frozen rack trees; MFM sample-treatment boundary NOT RUN |
| L Workbench retirement | **107/107 in-Editor PASS; runner FAIL (exit 1)** | not qualified; includes the six production Site masks and old/new workbench identity |
| M CTA / feedback checker | **PASS, source-static only** | 10/0/2 on this tree; independent #309 approval status not assessed here |
| N Save/load compatibility | **NOT RUN** beyond the current-schema V2 save/load round trip (**PASS** in the gate) | Legacy40, frozen Reference Future v1 and old-save fixtures NOT RUN |
| O Final integrated regression | **NOT RUN** | |
| Also: Reference Preview return pose, Enlarged80/B3, annual/old-save behaviour | **NOT RUN** on this source | |

## 5. Protected state after release (`run/final-state.txt`)
ZX20 **36.14 GiB** free at the end (minimum sampled 36.14, **0** alerts, 30 samples at 20 s; monitor exited on its stop file). Lock `free`, no Editor, no staged harness files, licence copies removed by both runners (only audit logs remain in the evidence config dirs). Licence pack 27/27 hashes OK. User saves (`forest-save.json.bak`, `save-backup-20261104`) hashes OK. Original eight-dirty Workbench worktree unchanged. U3 is left **on the new branch @ `045966d2` with `.vscode/settings.json` + the five `.mat` files dirty**; deliberately not restored.

## 6. Limits and open questions for Manager #200
* Real Editor evidence exists for only two sessions; no interactive human playtest; frame times were not measured.
* **Decision needed:** may the Manager extend the exact-byte policy to the five documented Section Five `.mat` rewrites (byte-identical to the protected Workbench worktree) so that the remaining A–O gates (headland/map, G/H, I, Legacy40/Reference, persistence) can run under a fresh lease, or does Manager prefer a different handling of those materials?
* U3 stays dirty, so any next lease needs that ruling and a fresh preflight; no cold Library or new worktree is implied.
* #287 is still NOT FULLY QUALIFIED; #316 not started; nothing merged to `main`; no Player build.
