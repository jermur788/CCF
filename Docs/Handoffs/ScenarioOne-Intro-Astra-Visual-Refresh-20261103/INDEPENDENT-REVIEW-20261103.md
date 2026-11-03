# #244 — Independent review: Scenario One intro visual refresh and Unity Editor startup exception

**Date:** 2026-11-03
**Reviewer:** Claude Sonnet 5.5 (Medium), read-only. Cross-provider to the OpenAI Astra implementation. No Unity run, no edits to the candidate, harness or production, no lock change.
**Reviewed branch / HEAD:** `feature/scenario-one-intro-astra-visual-refresh-20261103` @ `9b64d7a93840663c35f06f9952e1e9dd7641764f` (implementation commit `6bff87ddf2b39573ed0998c3621e820cb9cd3770`; base `72b4efbd1c653fa2231f632a79030a83ff76f833`, merge-base confirmed).
**Main at review:** `ef2faa3a`. No context commit was stated in the packet.
**Evidence root:** `/media/jer/Files/CCF/Verification/ScenarioOneIntroAstra-20261103/` (read only).
**Lane:** `/media/jer/ZX20/CCF-Intro-Astra-20261103` (read only; clean at `6bff87dd`).

## 1. Result

```text
INTEGRATION DECISION
Status: APPROVED — for the narrow waiver question only (see scope below). NOT a gate PASS.
Gate: G3 MEASUREMENT INTEGRITY (proposed waiver of one exact Unity-internal exception).
      G2 ACCEPTANCE and G4 INTEGRATION are NOT granted by this review.
Reviewed branch: feature/scenario-one-intro-astra-visual-refresh-20261103
Reviewed HEAD: 9b64d7a93840663c35f06f9952e1e9dd7641764f
```

**The onboarding gate FAILED, in both runs.** Each run ended `ONBOARDING_FAIL checks=288 failures=1 shots=6`, Unity exit 1, with no `ONBOARDING_PASS`. `process-result.json` records `pass: false` for both. That is the true result and stays the true result.

**Is it safe to waive the one Unity Editor startup exception for the #243 acceptance decision? Yes, narrowly and conditionally.**

- The exception is Unity's own QuickSearch startup-indexing bug.
- It has no CCF frame.
- It is not a candidate defect.
- Nothing in the 288 passing checks, the six renders or the Continue test shows it affecting anything the gate is meant to test.
- It is byte-identical to a signature already documented on `main` and seen on four earlier runs in a different lane, with a documented lane-state cause and a confirmed fix.

The harness is not modified. The assertion is not filtered. This is a Manager acceptance decision made on top of a failed gate, not a reinterpretation of the gate.

### Waiver conditions (all must hold)

1. **Exactness.** The waiver covers only a log entry whose full text matches signature SHA-256 `c6401236fe8905a9570890370f44ea47760c648f2ff3bdec3cea9a283fac147f`. That is the `ArgumentOutOfRangeException` through `SearchInit.IndexationOnStartup` → `SearchDatabase.GetDefaultSearchDatabase` → `SearchDatabase+<EnumerateAll>` at `SearchDatabase.cs:351`. It occurs once per run, at Editor startup. Any other exception, error or assert, or a second occurrence, voids it.
2. **Gate record unchanged.** Keep `ONBOARDING_FAIL` and exit 1 in all records. Describe the result as "gate FAILED 288/289; one Unity-internal exception waived by Manager decision on independent evidence". Never as "PASS" or "green".
3. **No harness change.** `NO_EXCEPTIONS_OR_ERRORS` stays as it is. It is not weakened, filtered or silenced.
4. **Visual acceptance is separate.** This waiver does not accept the visuals. Section 4 gives reviewer observations; the Manager and user decide acceptance.
5. **Corrections to the handoff wording** (section 3, item 1).

A repair of this lane's Editor search state plus a clean rerun is the only way to turn this into a real PASS. It is not required for the waiver. No third run is requested here.

## 2. Exception classification (Test 1 and Test 2)

### What the logs show

| Item | run-01 | run-02 |
|---|---|---|
| Log | `run-01/unity.log` (24,592 lines) | `run-02/unity.log` |
| Exception at line | 16523 (`Start Indexing on Editor startup`, line 16522) | 1059 (`Start Indexing on Editor startup`, line 1058) |
| Count of `*Exception:` log entries | 1 | 1 |
| Stack text vs `Docs/Verification/Enlarged80Calibration/editor-search-baseline.json` | byte-identical | byte-identical |
| Signature SHA-256 | `c6401236…` | `c6401236…` |
| Harness record | `NO_EXCEPTIONS_OR_ERRORS` fail; 288 `ONBOARDING_CHECK_PASS`; `ONBOARDING_FAIL` at line 24504 | same; `ONBOARDING_FAIL` at line 7657 |
| `process-result.json` | exit 1, `pass_marker` false, 710.7 s | exit 1, `pass_marker` false, 97.2 s |

### Findings

1. **The stack is wholly Unity.** Every frame is `System.Collections.Generic.List`, `System.Linq`, `UnityEditor.Search.*` or `UnityEditor.EditorApplication.Internal_CallDelayFunctions`. It is an Editor `delayCall`. There is no `Assets/…` frame and no CCF type. `UnityEditor` is not part of a Player.
2. **Timing: correct the handoff's phrase.** The handoff says it was logged "before any CCF code runs". That is not accurate.
   - The log order is: play mode entered → `STARTING_STAND` (`ForestStartingStand.Awake`) → `CCF_BUILD_ID` / `CCF_DIAGNOSTICS` (`StandaloneSessionMenu.Start`) → `GPU Resident Drawer created` → `Start Indexing on Editor startup` → the exception → `CCF_STARTUP_SCREEN_OPEN` → the first check `NORMAL_STARTUP`.
   - That is the same interleaving the healthy 25 Oct logs show (`ScenarioOneThreePageOnboardingV1-20261025/run-04`: `STARTING_STAND` line 1510, `Start Indexing` 1550, `CCF_STARTUP_SCREEN_OPEN` 1556).
   - The accurate wording is: no CCF frame; fired from the Editor's delayed-call queue once, at startup, after CCF's `Awake`/`Start` diagnostics and before the first gate check.
   - Interleaving with CCF's startup logs is a scheduling coincidence of the Editor's delay queue, not causation.
3. **Other errors, warnings and exceptions.** I read both logs in full by pattern:
   - `error CS`: 0 in both runs. `NullReference`: 0. Unity crash or fatal markers: 0.
   - Run-01 has 565 `warning CS` lines (fresh import) and run-02 has 0.
   - Unity startup noise present in both, none an exception and none a gate assertion:
     - `VBCSCompilerPreWarmer: ERROR … /tmp…` (the Unity pre-warm stub write failed).
     - `[Licensing::Module] Error: Access token is unavailable` (offline licensing; entitlement then "Successfully resolved").
     - `[vulkan] Failed to load device function …` (optional Vulkan extensions).
     - `[Warning] Evict-before-extend` (5–7 lines).
     - URP Terrain shader "All subshaders removed".
   - One observation outside this question: `Problem detected while opening the Scene file: 'Assets/Scenes/ForestTest.unity'` and `Prefab instance problem: 'UNS_Mushroom_Patch (Missing Prefab with guid: d0fa3c49b2c9cd84bb5a644c53ef025e)'`, 16 instances.
     - It is a warning, not a gate assertion.
     - It is absent from the 25 Oct logs but present in the 2 Nov `parity07-r5` PASS log. It was therefore already present on the lineage and is not introduced by #243.
     - The GUID appears only in `ForestTest.unity`, `MixedSpeciesTest.unity` and `UltimateNatureSetup.md`; I found no tracked asset or `.meta` carrying it. This is unrelated to onboarding. I flag it only for awareness.
4. **The 288 checks are real.**
   - Counted directly: `checks.txt` has 288 `ONBOARDING_CHECK_PASS` and 1 `ONBOARDING_CHECK_FAIL`.
   - The `checks.txt` of run-01 and run-02 are byte-identical (`c6ed77d9…`).
   - All six screenshots are byte-identical between runs.
   - Passing checks include:
     - `NORMAL_STARTUP`, `NO_ONBOARDING_BEHIND_STARTUP`, `RESOLUTION`;
     - `PAGE_1/2/3`, `*_MODAL_INPUT`, `LIVE_TITLE_*`;
     - `PAGE1_EXACT_RENEWAL_SENTENCE`, `PAGE1_EXISTING_HEADING_ACCENT`, `PAGE1_RENEWAL_BELOW_OPENING_COPY`;
     - `PAGE2_CARD4_EXACT_COPY_INSIDE_CARD`;
     - `ACTUAL_CAPTURE_SIZE_*`;
     - `PURE_UI_WORLD_HASH_UNCHANGED A0E0B2A51CC9274A A0E0B2A51CC9274A`;
     - `CONTINUE_AVAILABLE`, `CONTINUE_NO_ONBOARDING_WITH_FRESH_PREFS`, `CONTINUE_INPUT_RESTORED`, `CONTINUE_WORLD_PRESERVED`.
   - The harness handler registered on `Application.logMessageReceived` records only `Exception`, `Error` and `Assert` log types (`OnboardingVerification.cs` line 275).
   - The new checks the candidate added to `OnboardingVerification.cs` are purely additive (+23 lines). I confirmed `NO_EXCEPTIONS_OR_ERRORS` is unchanged on the branch diff.

### Why this exception cannot have compromised the tested behaviour

- **Mechanism.** The throw aborts only the delayed `IndexationOnStartup` callback, so Editor-Search background indexing never started. There is no `[Indexing] Starting Initial Indexing for Assets` line in either failing run. The healthy 25 Oct runs have it, and 2 Nov `parity07-r5` has it too (line 1105 of that log).
- **What it affects.** It affects Editor project search only. It does not affect UI Toolkit rendering, `Texture2D` capture, input events, `StandaloneSessionMenu`, save/load or the world.
- **Evidence it did not disturb them.** Both runs produced byte-identical renders and identical check results; the world hash is unchanged by navigation; the Continue path reloaded the saved world and did not replay onboarding.
- **Player boundary.** The code is in the `UnityEditor` namespace, so a Player build cannot execute it.

### Cause: what is proven and what is not

**Proven:**

- The signature is identical to one already recorded on `main` (`editor-search-baseline.json`, SHA `c6401236…`).
- The same exception hit four runs in a different lane, `CCF-S1ObjOverflow` (`ScenarioOneObjectivesOverflow-20261102/unity/parity07`, `-r2`, `-r3`, `-r4`). Runs r1–r3 failed only on it. Run r4 also failed with blank captures after a copied `Library/Search` cache was tried, so a repair should replace the settings only and not copy `Library/Search`.
- The prior independent handoff `Docs/Handoffs/ScenarioOneObjectives-Overflow-Repair/HANDOFF.md` attributes it to that lane's generated, gitignored `UserSettings/Search.settings` being `{}` after an out-of-memory-killed first import. Replacing it with healthy same-project settings gave a clean `parity07-r5` (1031 PASS, world hash `A0E0B2A51CC9274A`, which matches this gate's hash).
- `Docs/Handoffs/RACK-V2-09/INDEPENDENT-REVIEW.md` also records the same exception, in lane `CCF-RackV2-09`, as Unity QuickSearch and "not candidate code".
- The Astra lane matches that precondition exactly:

| | Astra lane (`CCF-Intro-Astra-20261103`) | Healthy lane (`CCF-U1`) |
|---|---|---|
| `UserSettings/Search.settings` | 2 bytes: `{}` | 889 bytes |
| `UserSettings/Search.index` | absent | present |
| `Library/Search/*.index` | none after two runs | 67,948,544 B index artifact |

The 25 Oct run-04 and the 26 Oct `VisualFixup` run-03, both passing, ran on `CCF-U1` (checked in their `command.json`).

**Not proven:**

- No Unity run on this lane after repair, so cause → fix is not demonstrated here. A run to prove it is forbidden by the packet and by the disk floor.
- The evidence rules out a candidate cause. It does not experimentally prove the repair.

That residual uncertainty concerns why the Editor throws, not whether the throw affected the candidate, which the evidence answers independently (above). I therefore judge it sufficient for a narrowly scoped acceptance-decision waiver.

## 3. Handoff accuracy corrections (conditions 5)

1. Replace "logged before any CCF code runs" with the accurate timing in section 2 item 2.
2. The handoff says the cause is "this lane's Editor/Library state … not proved". Add the stronger, now-documented basis: `UserSettings/Search.settings` is `{}` and `Search.index` is missing in this lane, matching the precedent above.
3. Its "Pages 2–3 at 1920×1080 … not opened individually" is superseded: I opened all six renders.
4. The README's success criterion (exit 0, `ONBOARDING_PASS`, zero `ONBOARDING_CHECK_FAIL`) is not met and must not be reported as met.

## 4. Visual review (Test 3)

**Directly viewed:** all six real Unity renders (`run-02/evidence/page-{1,2,3}-{1280x720,1920x1080}.png`, hashes `692efe3d`, `9c837403`, `8b064964`, `c59501b9`, `b1b35924`, `1da6f2c4`; each equals its `run-01` twin) and the three 1920×1080 comparison sheets (reference left, render right).

**Not opened individually:** the three 1280×720 comparison sheets. I compared the raw 1280×720 renders directly. Reference hashes used by the sheets (`9454b0fe…`, `0a723a7d…`, `53ed0a9b…`) match the branch's `20261103-art-manifest.json`.

**Automated assertions vs. images:** the text-fit, exact-sentence, colour-equality and card-containment results are the harness's. The rest below is my own reading of the images. I did not do a pixel diff.

### Page 1 — "What is Continuous-Cover Forestry?"

- **New line.**
  - The exact sentence is present and rendered in the teal heading accent. The class is the existing `onboarding-accent`, and no new colour token was added.
  - It sits on its own line between the opening paragraph and the divider, and does not wrap at 1280×720.
  - The original two sentences are intact. The footer and navigation are live UI.
- **Forest age distribution.**
  - Left: dense, uniform mature Sitka.
  - Middle: several tall retained Sitka at the back and middle, broadleaves of mixed height, and small conifer regeneration at the front-centre around a small log-pile opening. It reads as selective openings, not clearfell.
  - Right: tall mature Sitka, mid-sized Oak/Beech-type broadleaves and small conifers scattered front to back.
  - I did not see a uniform front row of saplings, a clear-felled front or a mature-only back-row pattern. It meets the packet's "continuous forest slice" requirement.
- **Observation.** The art frame is compressed 322→289 design px in height with unchanged width, so the three forest panels are about 10% vertically squashed compared with the reference. Trees look marginally squatter. No slice is removed. The handoff says "compressed" but does not state the distortion; I regard it as a minor note, not a defect.

### Page 2 — "How CCF works over time"

- Five cards in the approved order, with the revised small icons. The card 2 "retain" icon is tree plus check mark. The card 3 "logs" icon is three rings, one per log end, and is not doubled.
- There is no blue halo on the retained sapling, no stacked process icons in cards 2 and 3, and no scenario-specific text.
- Card 4 reads "Encourage natural regeneration and establish other species for a diverse forest." in full, inside the card, at the same font size as the others. There is no clipping or overlap. At 1920×1080 the right end of its longest line is about 10 px from the card edge — tight but inside, and `PAGE2_CARD4_EXACT_COPY_INSIDE_CARD` passes at both sizes.
- Card 5's six-line text fits inside its card.
- The Oak/Beech seedling photograph is unchanged.

### Page 3 — "Scenario One — Transforming a Sitka Plantation"

- The right forest contains several tall, imposing retained mature Sitka (about three prominent, others behind) alongside identifiable broadleaves and smaller regeneration. It is not all-juvenile and not native-only.
- The tallest crowns rise slightly above the divider rule into the lead-text band, but they do not touch the lead text. This matches the approved reference composition, and the art frame was deliberately raised (y=225, h=365) to avoid clipping the tips. No clipping is visible.
- Live Goal, Initial/Target state and Key idea text, Back and Begin are intact at both sizes.
- The reference images show Next/Begin as washed-out mock-up buttons. The live buttons are the qualified V2 teal primary style, which is correct for live UI.

**Conclusion.** The renders match the approved composition with only the intended differences (added Page 1 line and spacing, live controls). I found no visual defect that blocks acceptance. Final visual acceptance remains with the Manager and user.

## 5. Source, assets and scope (Test 4)

**Changed set** (`72b4efbd..9b64d7a9`, 17 files; only this set, no stray paths):

- *Production UI (code):* `OnboardingIcon.cs`, `ScenarioOneOnboardingCopy.cs`, `ScenarioOneOnboardingView.cs`.
- *Runtime art:* `page-{1,2,3}.png` under `UI/Resources/Onboarding/`. No `.meta` changed, so GUIDs are preserved.
- *Verification tooling:* `OnboardingVerification.cs` (additive checks only), `make_comparisons.py` (new `--refresh-20261103` flag; 25 Oct path preserved), `prepare_20261103_art.py` (new).
- *Spec/reference records:* `production-copy.md`; new `references/20261103-art-manifest.json`, three `20261103-page-N-approved.png`, `20261103-reference-addendum.md`.
- *Documentation:* `IMPLEMENTATION-HANDOFF.md`, `MANAGER-HANDOFF-20261103.md`.

**Boundaries.**

- No simulation, economy, save-format, Stand Map, MFM, Reference Future v1 or Legacy40 file is touched.
- The 25 Oct `page-N-locked.png` and `final-hashes.json` are unchanged (confirmed by diff).
- The three runtime atlas SHA-256 values computed from the branch blobs match both the handoff and the manifest (`b775eb95…`, `95ccacdf…`, `83596a3d…`).
- The user's save was untouched: isolated config and save, and `PURE_UI_WORLD_HASH_UNCHANGED`.

**Reference provenance — one item for the Manager.** The selected source images (`9454b0fe…`, `0a723a7d…`, `53ed0a9b…`) differ from the three hashes in `HANDOFF.md` section 2 (`c082fd5a…`, `ee3a8fb0…`, `8e289b21…`). The handoff's own section 8 and the reference addendum document this: the latest user-identified Astra originals take precedence, and the addendum states the older ZIP hashes do not describe them. I can confirm that the repo records are internally consistent. I cannot independently confirm which set the user last approved. Please confirm with the user.

**Cleanup receipts.**

- `process-result.json` for both runs: `harness_removed: true`, `licence_copy_removed: true`, `cleanup_failures: []`.
- The lane `git status` is clean. No `OnboardingVerification*` file remains under `Assets/` in the lane, and no `.ulf` licence file is in either run's config.
- Unity's Editor side-effects (`.vscode/settings.json` and three `Woodland_Forbs_01` `.mat` files) are preserved as `unity-editor-side-effects.patch` and were restored. They are not in the commit.
- No Unity process is running on the machine. `lease-01.json` still says `held` (owner Astra-243, lock `7256c06f…`, `unity_editor_pids: []`). I did not touch it; per the packet only the original owner may release it.
- `/media/jer/ZX20` showed about 28 GB free when I checked. That is more than the packet's stated ~14 GB, but I did not act on it: no Unity admission is implied here.

## 6. Remaining gates and limits

- **Not verified by this review:** a clean gate run; OS-native pointer and keyboard delivery (harness uses queued New Input System events); no Windows/Player build; personal playtest not complete.
- **Still required before integration:** Manager disposition of this waiver; Manager/user visual acceptance; G4 integration decision. #243 must not be called green or integrated on the strength of this review.
- **Optional, separately authorised:** repair this lane's `UserSettings/Search.settings` and run one confirmatory pass when Unity use is next authorised. This would convert the evidence from "strongly attributed" to "demonstrated".

## 7. Return

Return to Overall Manager for formal disposition: **APPROVED — narrow exception waiver only; the gate remains FAILED**.
