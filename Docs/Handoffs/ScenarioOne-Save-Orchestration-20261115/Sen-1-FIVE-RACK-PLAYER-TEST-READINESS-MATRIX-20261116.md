# Sen-1 — Five-rack Scenario One personal player-test readiness matrix (Candidate A)

**Date:** 15–16 November 2026 (UTC). **Role:** read-only readiness analyst, the research sub-agent under orchestrator [#544](https://github.com/jermur788/CCF/issues/544). This implements §B/P2 of the [Sen-1 overnight player-test readiness amendment](https://github.com/jermur788/CCF/blob/docs/sen-1-overnight-player-test-amendment-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-OVERNIGHT-PLAYER-TEST-READINESS-AMENDMENT-20261115.md).
**Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`) in T3 Code. I started no subagents.
**Branch:** `sen-1/five-rack-readiness-matrix-20261116-v1`, created from `origin/main` at `660492c62682f1ab68793ef68fb45bfd6cba2965`. This file is the branch's only change.

## Verdict: **CONDITIONAL** for one personal Editor playtest of Candidate A

**This is a documentation inventory, not a new native pass.** I did not launch Unity, take a lease or run the launcher (not even `--check-only`). I did not read or hash any host save, any PlayerPrefs, or the disposable profile's contents. Every PASS below is **inherited** evidence from earlier qualification on Candidate A or its direct ancestors, and each one is cited.

Candidate A is the safest playable Scenario One we have. Its code, lane and launcher are unchanged since the Manager's 12 November conditional GO, and the user has already played it once, up to the first Annual Review. It is **not READY**, for four reasons:

1. **The 12 November GO covered ONE session.** That session happened (21:27–22:07 UTC). A second session needs a fresh #424 confirmation (amendment §A.6: "historical D-073 conditional Editor GO is historical evidence, not a current live lease").
2. **The launcher's own cleanup has never completed in a real session.** On 12 November the terminal was closed: the receipt says `launcher_cleanup=NOT_RUN(terminal closed)`, and an agent did the cleanup on the user's word. That cleanup also found **two Editor-drift paths that the launcher's 10-path allow-list does not cover**. A normal exit is therefore likely to end in `STOP: unexpected lane changes preserved` (see §3.4).
3. **The disposable profile is no longer fresh.** It holds a `prefs` file written at 22:07 on 12 November. First-use screens and lesson steps the user saw will not reappear unless reset (see §3.4).
4. **The physical Ops board (#553) is unsynchronised.** Its `CCF-412` row still says WAITING / NOT STARTED, and the file was last modified on 13 November. Amendment §A.5 forbids a Unity launch until a designated physical owner has checked the board, lane, lease and Prefs. For a personal playtest that owner is the user, but the check has not happened.

None of these is a game defect. All of them can be cleared by the owner and #424 without any source change.

Compact / D-082 / CG-TRIG (Candidate C) is **not approved and not playable**. Nothing here assesses it.

## 0. Three candidates, kept separate

| | Candidate A — historical five-rack personal candidate | Candidate B — shared-save source | Candidate C — compact world |
|---|---|---|---|
| Ref | `integration/scenario-one-personal-playtest-d073-20261112-v1` | `feature/scen1-shared-three-profile-save-v2` | none (D-082/#471, OPEN-C1…C8) |
| Head (verified with `git ls-remote` on 2026-11-15) | `55863f1b7719011bba16b3f3ab377e379c0b988a` (production source `339517ed`) | `2353a792ee3f8f328ff7e1de3607a2c2333c8c87` | — |
| Status | Manager #424 CONDITIONAL personal-Editor GO, 12 Nov, for one session; played once | T-02 **NEEDS CHANGES** (#557 M-1), repair #563; **unintegrated, not Unity-qualified** | **On hold**; specs not approved |
| Lineage | 74 commits not on `main`: Rack V2 + MFM + S1-C/S1-D + cadence V1 + D-073 year comparison + #372–#391 UX | Built on `main` (`f52dec24`). **It does NOT contain Candidate A's 74 commits.** `git diff 55863f1b 2353a792 -- Assets` is 50 files, +1276/−2975. B has no `ScenarioManagementCadence.cs`, `ScenarioOneYearComparisonText.cs` or `ScenarioOneManagementFollowonReadout.cs` | — |
| Playable now? | Yes, conditionally (this report) | **No.** It is a different game build from A, lacking the D-073 and cadence features | **No** |

**Consequence:** no B evidence (T-01, T-02, the 352-assertion baseline, mutants) says anything about A, and no A evidence says anything about B. B is not a newer A; it is a sibling on another lineage.

## 1. Evidence sources used

| Tag | Evidence | Applies to |
|---|---|---|
| GO | [Manager #424 conditional GO](https://github.com/jermur788/CCF/blob/docs/scenario-one-manager-424-d073-playtest-go-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-PERSONAL-EDITOR-TEST-CONDITIONAL-GO-20261112.md) | exact A |
| F412 | [Fable #412 consolidated result](https://github.com/jermur788/CCF/blob/bc15e697f1a166c4faa6dadeca053e4a06d19574/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/FABLE-412-D073-PERSONAL-EDITOR-TEST-CANDIDATE-RESULT.md): #426 G2, #427 Tier-2 720p/1080p, #435 evidence review, #437 launch proof | exact A (source `339517ed`) |
| U12 | [User live feedback, 21 findings](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-LIVE-PERSONAL-PLAYTEST-FEEDBACK-20261112.md), plus the local run receipts `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/runs/20261112T212725Z-624305/` | exact A, real user session |
| W438 | [#438 native rack-entrance witness](https://github.com/jermur788/CCF/blob/docs/scenario-one-438-rack-entrance-native-witness-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-438-RACK-ENTRANCE-NATIVE-ID-WITNESS-RESULT.md) | exact A at `55863f1b`, default suggested layout only |
| C408 | [#408 cadence V1 Unity qualification](https://github.com/jermur788/CCF/blob/dcb319f9eb93b16d60a739bd907c65de1d4204e7/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-CADENCE-V1-FIXED-CANDIDATE-UNITY-QUALIFICATION-RESULT.md), 143 sub-cases | ancestor `e7591933` |
| Q417 | #417 99-check / 18-image Tier-2, re-witnessed read-only by [#423](https://github.com/jermur788/CCF/blob/docs/d073-417-existing-artifacts-witness-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CODEX-423-D073-417-EXISTING-ARTIFACTS-WITNESS.md) | ancestor `77a56a44`; superseded for the Annual Review view by #427 |
| S381 | `Docs/Handoffs/ScenarioOne-Playtest-Finish-20261111/SOL-381-SCENARIO-ONE-PLAYTEST-READINESS-RESULT.md` on A | ancestor `b62446ca` (#382 candidate) |
| P357/P360 | #357 persistence v2 / cold / Legacy40 PASS; #360 old-world / v24 PASS (in A's history under `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-2026110{7,9}/`) | ancestors. **The save files are byte-identical from #360 to A** (§3.3) |
| R450 | [#450 independent delta review](https://github.com/jermur788/CCF/blob/docs/objective1-450-rack-marks-independent-review-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE1-450-RACK-MARKS-INDEPENDENT-DELTA-REVIEW-RESULT-20261113.md) | #443 candidate `7633eaf1`, **not A**; cited only for code shared byte-for-byte with A |

## 2. Deliverable 1 — player-path blocker matrix for Candidate A

Legend: **PROVEN** = existing qualification evidence on exact A, or on an ancestor whose relevant files are byte-identical to A. **PROVEN-USER** = the user did this on exact A on 12 Nov (an observation, not an instrumented pass). **UNPROVEN** = no evidence on A. **BROKEN** = a confirmed defect on A.

| # | Player step | Status | Evidence | Notes / player-visible risk |
|---|---|---|---|---|
| 1 | Guarded launch → Editor → Play → title → **Start New Scenario** → three intro pages → forest renders | **PROVEN** (private 1280×720 display) + **PROVEN-USER** (real desktop) | F412 §6 (#437 proof); U12 header ("rendered through the three intro pages") | 1080p first view: user-observed only. Launcher cleanup with the final launcher bytes: **UNPROVEN** (§3.4) |
| 2 | **Objectives (O)** | **PROVEN-USER**; no functional defect | U12 #1–#4 | UX only: O not emphasised; heading hierarchy; teaching diagram moves after "More detail"/Esc (U12 #4). Non-blocking |
| 3 | **Stand Map (M) → Plan Racks → five parallel racks → Confirm Rack Plan** | **PROVEN** (W438 with "Reset to suggested layout"; S381 row 28 "Entry / Rack V2 / five-rack map" PASS) + **PROVEN-USER** | W438 fixture; geometry fingerprint `307C1ED538EA0D3E` unchanged from confirm to 5/5 | UX: rotation cursor/snap (U12 #8). Open design question (U12 #9): the bottom rack extends left of the main track. **Not a defect, not approved for change** |
| 4 | **Physically mark the five rack entrance trees (P, aim ≤ 3.5 m, left-click)** | **PROVEN for association** on the default layout. **BROKEN (presentation)** for edited or redrawn plans. **UNPROVEN** for the user's own route | W438: 5/5 sprays each marked the owning rack; orders equal removal sets (30/18/34/36/31). Source on A: `ForestTreeMarkingManager.RackEntrances.cs` `TryGetRackFieldTarget` walks `ConfirmedRacks` in **stable-ID order**; the HUD label is `$"Rack {fieldRack.Id} entrance · {distance:0} m"` (`UI/WalkingHudView.cs`); the arrow hides inside half a cell of the target | **Known #443 conflict (§3.2).** After any rack is deleted and re-created, or racks are drawn in another order, the arrow and "Rack N" label follow saved IDs, not the physical bottom→top order. That is the user's 12 Nov "far Rack 1" report (U12 #14). Marks still land on the right rack. A's minimap shows **no rack target** (W438). Counter "Rack entrances marked n / 5" counts persisted flags |
| 5 | **Inspect (E), Crop (C), Fell (X)** | **PROVEN** (scoped) | S381 row 29; MFM08 412 checks incl. native E/C/N at 1080p | 720p native C/N and per-card pointer restrictions: **PARTIAL** (S381 §3, inherited) |
| 6 | **Work Plan (Tab) → Approve Work Plan** (separate from Advance) | **PROVEN** + **PROVEN-USER** | W438: 149 Pending, nothing auto-approved; C408 4b order cases; U12 #15–#17 | UX: dimming and typography (U12 #16–#17). Non-blocking |
| 7 | **Advance one year** | **PROVEN** | C408 rows 28–42 (quiet / direct / skip, N = 1–5, exact canonical hashes); C408 4a input lock during advance | — |
| 8 | **First Annual Review → read → "I've read the annual results"** | **PROVEN** + **PROVEN-USER** | F412 §4 cases b–f, both sizes; #427 negative control (unrepaired view failed 170/209); U12 #18–#20 | **Misleading prompt** (U12 #18): the HUD says "First: read and acknowledge the annual results in Work Plan [Tab]" (`LearningObjectivesView.cs` `ReviewFirstInstruction`) while the player is already on the review. Copy only; the acknowledgement gate works. Fixed on #443 (F-3), **not on A** |
| 9 | **Year comparison** (earlier vs latest year, RECORDED vs DERIVED, By species, Show data sources, caveat) | **PROVEN** | F412 §4 (409 checks per size); #435 APPROVED | **Watch item:** pointer-loss clicks on "By species" / "Earlier year" / "Later year"; cause unknown (GO §2.4). The old table header is redundant (minor) |
| 10a | **No-work year(s):** Advance with nothing approved, or **"Skip to Year N (k years)"** in the Work Plan footer | **PROVEN** | C408 quiet N = 1–5 direct vs skip parity; `UI/WorkPlanView.cs:177` | Unread reviews correctly block the next advance (C408 4b "unread latest review"). D-074's 3–6-year spacing is a product direction, not a lock in A |
| 10b | **Second intervention** (new marks → approve → advance → second review) | **PROVEN (scoped, inherited)** | S381 row 33: S1-D natural Year 25 completion and Year 17 intervention PASS | **PARTIAL** on exact candidates: idle/stale/history/post-completion (S381 §3). The Legacy40 history harness **FAILS** on a stale expectation (S381 line 53), which is a harness issue, not a product defect. The user has not done this yet |
| 11 | **Save (F5 or F10 → Save)** | **PROVEN** (writes only the disposable profile) | #437 native F5 into `…/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF/`; C408 "destination disk save/reload state" PASS (full canonical JSON + hash identical, API-driven) | The user did **not** save on 12 Nov: the profile has `prefs` but no `forest-save.json` (directory listing only, contents not read) |
| 12 | **Continue from title / F9 Load / F10 Load → restore** | **UNPROVEN natively on exact A** | GO §2.3 and F412 §7: "Native F9 load NOT RUN on this new isolated profile". Startup Continue exists (`UI/StandaloneSessionMenu.Startup.cs:12, 90–97`; refusal shown via `ContinueNote`). C408 4a proves F5/F9/F10 reach their handlers and are refused during a skip. P357/P360 persistence PASS on byte-identical save files | Risk is low because the save code is byte-identical since #360 (§3.3), but a **human Continue → restored-world check has never been run on A**. The pack below makes it a deliberate, last, throwaway step |
| 13 | **Return to forest after restore** (walk, marks and orders visible, year and cash correct) | **UNPROVEN** natively; source-level equality PROVEN | C408 disk reload: identical JSON/hash; "disk reload removes session-only summary" PASS | Expect the cadence session summary to be gone after reload (by design) |
| 14 | **Oak/Beech parity** (planting, three-species growth) | **PROVEN (inherited PARITY-09)** | S381 row 31; MFM11 witness includes 3 Beech + 3 Oak planting | Optional in this test. Density runner = ACCEPTED_LIMITATION (pure-Beech R2 misses), inherited |
| 15 | **Visible annual consequences** (canopy, cleared racks, felled trees, cash, review numbers) | **PROVEN (scoped)** | C408 full records incl. ecology / juveniles / deadwood / cash; F412 year comparison | Look for these yourself; there is no automated visual oracle for the 3D forest after a year |
| 16 | **Legacy40 / frozen Reference Future v1 untouched** | **PROVEN** | C408 rows 125–133 (Legacy40 refusal, four frozen anchors, archive identity); F412 case g (v9 old save + Reference) | The personal test opens neither. Do not use the Reference preview to "test" it |

**Hard blockers in the player path: 0 functional.** One **presentation-correctness hazard** (row 4, edited plans) and one **UNPROVEN user-critical step** (rows 12–13, Continue/restore). Both are handled by the test pack's route and stop signs rather than by code.

## 3. Deliverable 2 — hidden-risk check

### 3.1 Startup/menu path vs the historical single-save layout, and contradictions from Candidate B

- **A uses the historical single save:** `forest-save.json` + `.tmp` + `.bak` under `Application.persistentDataPath` (`Assets/ForestPrototype/ForestSaveController.cs:9–19`). There is no profile selection and no envelope. Payload `ForestSaveData.CurrentVersion = 24` (`ForestSaveData.cs:7`).
- **B keeps the same file name for its Historical profile.** In `ForestSaveProfiles.cs:59` on B, Historical → `"forest-save.json"`. B adds `forest-save-scenario-one-compact-v1.json` and `forest-save-scenario-two.json`, both refused as unavailable. B's writer still emits plain `JsonUtility.ToJson(data)` (`ForestSaveController.cs:158` on B). `ForestSaveEnvelope.SupportedPayloadVersion = 24`, and `ForestSaveData.cs` is identical on A and B. **I found nothing in B that changes the meaning of A's startup/Continue path or file layout.**
- **Cross-candidate hazards (not contradictions; avoid them):**
  - B lacks A's cadence and D-073 code. **Never** open a save written by A with a B build, or the reverse. B's new admission and validation (#557 M-1, #563 repair) are unqualified, and silent refusals were exactly the M-1 finding.
  - The launcher pins lane, branch and `HEAD_SHA = 55863f1b…` (`launch-personal-playtest.sh` lines 7–9). B cannot be launched through it by accident.
  - Future risk: if B, once integrated, is ever run with `XDG_CONFIG_HOME` pointed at `…/PersonalPlaytest-D073/profile/config`, it would treat A's file as a Historical save. No such path exists today.
- **B's T-02 defects (#557 M-1 silent refusals on Continue/F10 Load/Save) are NOT present in A.** A's startup Continue sets a visible `continueNote` on refusal (`StandaloneSessionMenu.Startup.cs:94–97`). A was never subject to B's admission code.

### 3.2 Objective 1 / #443 UX and rack-identity conflict

- **Present in A: yes.** #443 (`feature/scenario-one-objective1-ux-repair-20261112-v1` @ `acb4e111`, 15 commits on top of A, 15 Asset files, +200/−38) is **FROZEN for overlapping writes / integration hold**. A contains none of its fixes. I read #443 only; nothing was integrated or cherry-picked.
- **What A does** (confirmed in A's source, diffed against #443):
  1. Field target = first unmarked rack **by stable ID** (`TryGetRackFieldTarget`); #443 changes this to south→north `RackVisitOrder`.
  2. Labels expose stored IDs: HUD `"Rack {Id} entrance"`, spray prompt `"Rack {rack.Id} entrance · [Left-click] Spray orange"`, manager feedback `"Marked the entrance tree {tree} for rack {rackId}."` (`ScenarioOneManager.RackPlan.cs:419`).
  3. The arrow hides once within half a cell of the target, even before spraying (`WalkingHudView.cs`; #443 keeps it until sprayed).
  4. The minimap never shows a rack target (W438).
- **Effect on the player path:** step 4 only. On a **fresh "Reset to suggested layout" plan** the IDs 1–5 run south→north, so labels and order match (W438). On an **edited plan** the "Rack N" label and arrow order can point at a far rack. This is the user's own 12 Nov report and the user-decided F-1 rule ("bottom rack is always player-facing Rack 1": [decision](https://github.com/jermur788/CCF/blob/docs/objective1-f1-user-rack-number-disposition-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/USER-OBJECTIVE1-F1-RACK-NUMBER-DISPLAY-DECISION-20261113.md)) is **violated by A**. Association safety is not affected: `TrySprayRackEntrance` is byte-identical between A and #443, W438 showed 0 mismatches on A, and R450 showed correct association on a reversed-ID plan with the same function.
- **Mitigation without touching #443:** in the test, use **Reset to suggested layout** and do not delete or redraw racks. If the plan is edited, ignore the "Rack N" label and follow the counter. Re-testing the user's original "missing arrow" symptom belongs to #443 and is out of scope for A.
- The other 20 U12 findings (pulses, keycaps, dimming, typography, copy, Objective 2 rule #439) are UX or product items. Most are addressed on frozen #443 or are open decisions. **None blocks a personal test of A**; they will simply be seen again.

### 3.3 Which inherited checks still hold, and which do not

**Still valid for A (exact head unchanged; verified `git ls-remote` and the lane's `rev-parse HEAD` = `55863f1b…`, lane clean):**
- Everything under F412 (#426, #427, #435, #437 launch proof): exact production source `339517ed`.
- W438 association witness: run on exact `55863f1b`.
- **Save/persistence evidence from #357/#360 and C408 "disk save/reload".** `git diff fb599eec..55863f1b`, `b62446ca..55863f1b`, `e7591933..55863f1b` and `77a56a44..55863f1b` over `ForestSaveController.cs`, `ForestSaveData.cs`, `UI/StandaloneSessionMenu.cs` and `UI/StandaloneSessionMenu.Startup.cs` are **all empty**.
- C408 cadence cases: A adds only the byte-identical #406 readout (`d86a68a1`) plus the D-073 view and mapper. C408 itself remains on `e7591933`, so it is inherited, not exact-candidate.
- S381 inherited scopes (Rack V2, MFM08/11, PARITY-09, S1-D), with their own PARTIAL/FAIL residuals unchanged.

**Invalidated or no longer true, and not to be claimed:**
- **"The first session starts fresh"** (launcher `README.md` §4). False now: the profile `prefs` file was written at 22:07 on 12 Nov. A keeps lesson steps (`CCF.Learning.*`) and first-use screen flags (`MenuHelp`) in PlayerPrefs (`LearningObjectivesView.cs:107–127`, `MenuHelpView.cs:64–70`).
- **"Launcher cleanup restores all known Editor drift."** The 12 Nov agent cleanup found `Bilberry_Type_Cover_01/Materials/Berry_Calyx.mat` and `…/Bilberry_Berries.mat` dirty. Neither is in the launcher's 10-path `allowed` array. So the launcher's normal exit would print `STOP: unexpected lane changes preserved` and exit 1, and the **next** `--check-only` would refuse with `Lane contains changes`.
- **"Lease released by the launcher after a normal exit."** Never shown with the final launcher bytes. 12 Nov: `launcher_cleanup=NOT_RUN(terminal closed)`, release done by an agent on the user's statement (`agent-completed-cleanup/result.txt`).
- **Physical Ops board row `CCF-412-PREDATOR-FABLE-A-D073`** ("WAITING… NOT STARTED"): stale (`/media/jer/ZX20/CCF-Ops/board/active.md`, modified 2026-11-13 15:49).
- **The host-37 / backup-81 hash baselines** are from 12 Nov. Any later change to a valued host save makes `--check-only` STOP (correctly). I did not hash them, so this is **UNVERIFIED**.

**Never applicable to A (must not be borrowed):** all Candidate B evidence: T-01 registry, T-02 admission, the 352-assertion baseline, the 11/12 mutants, #557, and the upcoming #563 / T-03A / T-04A. Also all #443/#450 native results except where they exercise code byte-identical to A (`TrySprayRackEntrance` only).

### 3.4 Stop signs the existing runbook does not warn about

| # | Stop sign | Where it comes from | What to do |
|---|---|---|---|
| S-1 | Terminal ends with `STOP: unexpected lane changes preserved; contact operator.` and exit 1 after a **normal** Unity close | Two SectionFive `.mat` paths not in the launcher allow-list (§3.3) | **Expected, not a save-safety failure**, if `lane-status.txt` lists only `.mat` files under `Assets/ForestPrototype/Art/SectionFive/` and/or `.vscode/settings.json`, and `host37-check.txt` shows 37 × OK. Do **not** `git checkout` the lane yourself; report the receipt directory. The lease is still released (the release runs before the final exit code) |
| S-2 | `--check-only` → `STOP: Lane contains changes` | Follows S-1 on a later attempt | Stop; ask the operator to restore the known drift paths. Do not discard them yourself |
| S-3 | `--check-only` → `STOP: Valued host files differ…` or `Host game-data inventory changed` | Baselines are three days old; any other game run that used the host profile changes them | Stop. This is the guard working. Needs an operator to re-baseline after confirming the backup |
| S-4 | Exit code **198** after launch | Stale host Unity licence (seen in earlier CCF runs); the launcher copies the newest `UnityEntitlementLicense.xml` | Launcher prints `STOP: Unity licence exit 198. No retry.` Refresh the licence in Unity Hub, then re-run `--check-only` |
| S-5 | First-use help screens and lesson steps are **missing**, or Objectives show steps already done | Profile `prefs` carried over from 12 Nov | Not a bug. To see them again: **F10 → "Reset tutorial/help progress…" → confirm** (`UI/StandaloneSessionMenu.cs:128`). This clears only local help/lesson prefs in the disposable profile; the forest and save are untouched (`ScenarioOneUiRoot.cs:747–756`) |
| S-6 | No pulsing guidance anywhere | `CCF.UI.ReduceMotion` defaults to **on** (`ScenarioOneUiRoot.cs:214`, `GetInt(…, 1)`) | Expected. Pulses only appear with Reduce motion off. Static highlights should still show |
| S-7 | HUD arrow says "Rack 1" but points at a far or top rack, or appears only after your first spray | §3.2 (A labels by stored ID; arrow hides near the target; no minimap rack target) | Known #443 issue. Keep marking; check the counter rises by exactly 1 per new entrance. **STOP only** if a spray marks a tree that is not an entrance, the counter jumps by more than 1, or the Work Plan shows orders for a rack you did not mark |
| S-8 | Closing the terminal while Unity is open | 12 Nov: the receipt was lost and the lease needed agent release | Keep the terminal open until it prints `Receipts: …`. Ctrl-C is trapped and does not release the lease |
| S-9 | Pressing **Continue / F9** after a session that never saved | Profile has no `forest-save.json` | Expected: "No save found (press F5 to save)" / Continue unavailable with a note. Not a failure |

## 4. Deliverable 3 — paste-ready personal test pack (Candidate A)

> **Personal Editor playtest — historical five-rack Scenario One (Candidate A).**
> Branch `integration/scenario-one-personal-playtest-d073-20261112-v1`, commit `55863f1b7719011bba16b3f3ab377e379c0b988a`. Internal Editor test only: no Windows build, not acceptance.
> **Precondition:** #424 has re-confirmed the GO for a second session (§5 gate E).
>
> **A. Before launching (Predator, normal desktop terminal)**
> 1. Close every Unity Editor and Unity Hub project window. Leave the older #382 lane alone.
> 2. Run:
>    `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh --check-only`
>    It must print `CHECK PASS`, followed by the candidate @ `55863f1b…`, the lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, the private config path and the backup path. `--check-only` checks the tools, the ZX20 mount, exFAT, 15 GiB free, branch/HEAD/clean lane, no leftover proof helper, the 4,011 production hashes, the 37 valued host files, the 81-file backup, the host game-data inventory, a free lock with no Editor PIDs, `DISPLAY`, no symlinks in the private profile, and a licence. It takes **no** lease, copies nothing and opens nothing.
>    **Any `STOP:` line → do not launch. Paste the exact output back.** (See S-2, S-3.)
> 3. Launch: the same command without `--check-only`. It acquires the machine-wide Unity lease (4 h ETA), copies the licence into the private profile and opens `Assets/Scenes/ForestTest.unity`. **Keep the terminal open.** Never open the project from Unity Hub, which bypasses the private save profile and the lock.
>
> **B. Save and profile isolation**
> - All saves and PlayerPrefs go only to `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF/` (`forest-save.json`, `.bak`, `.tmp`, `prefs`). Your real host saves are not used. An independent backup is at `/media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112/`.
> - This profile **already has prefs from your 12 Nov session** and no saved forest. For first-time screens: in Play mode, **F10 → "Reset tutorial/help progress…" → confirm**, then start.
>
> **C. Controls**
> WASD walk (Shift run, Space jump) · Mouse look (Esc frees the mouse; click to recapture) · **O** Objectives · **M** Stand Map / Plan Racks · **P** equip/stow rack spray (aim ≤ 3.5 m, left-click) · **E** inspect (V / Shift+V pages) · **X** Fell mark (proposal only) · **C** Crop Tree · **1–5** a Crop Tree's competitors · **G** planting spot · **U** plan vegetation clearance · **Tab** Work Plan · **F1** help · **F5 / F9** save / load · **F10** session menu (new, save, load, reset tutorial, quit) · **Esc** close panel.
>
> **D. Route (about 30–45 min)**
> 1. **Play** → title → **Start New Scenario** → three pages (Next / Back / Begin). **The forest must show textured ground and trees.** If it is black or blank: STOP (E1).
> 2. **O** → read Objective 1 → **M** → **Plan Racks** → **Reset to suggested layout** → **Confirm Rack Plan**. Do not delete or redraw racks in this run (S-7).
> 3. Dismiss "Introducing Tree Marking". Walk to each of the five entrances (bottom one first, next to the access pad, then upwards). **P**, aim at the cued trunk, left-click. The counter should rise 0 → 5, one per entrance.
> 4. Optional: **E** on a tree, **C** one Crop Tree, **X** one Fell tree.
> 5. **Tab** → read → **Approve Work Plan** → then, separately, **Advance one year**.
> 6. **Year 1 Annual Review:** read Work done / Money / Forest → **I've read the annual results**. In the year comparison, pick the earlier year vs the latest. Check that each year value has its own **RECORDED** tag and the difference has **DERIVED**. Open **By species**, toggle **Show data sources**, scroll to the caveat.
> 7. **No-work years:** **Tab** → approve nothing → **Skip to Year N (k years)** (or Advance once). Read and acknowledge each new review. Check the forest and the numbers change believably, with no invented work.
> 8. **Second intervention (optional):** mark a few Fell/Crop trees (or follow the Objective 2 sample) → Approve → Advance → read the next review. The history should show only work you actually did.
> 9. **Save / Continue (throwaway, do last):** **F5** (or F10 → Save) → note the year, cash and a landmark → leave Play mode → Play again → title → **Continue**. Check the same year, cash, rack marks and approved/done work, and the forest as you left it. Walk a little. The cadence summary text may be gone; that is expected. Optionally try **F9**. This step has **never been run by a human on this candidate**; report exactly what you see.
> 10. **Finish:** leave Play mode → close Unity normally → **do not save scene changes** → wait for the terminal to print `Receipts: …`.
>
> **E. What to look at**
> Forest, ground and UI readability at your resolution. Cleared rack corridors after the first year. Felled vs standing trees. Canopy and young-growth change across skipped years. Cash vs the Work Plan quote. Annual Review labels (RECORDED / DERIVED). Any history line claiming work you did not do. Year-comparison buttons that ignore clicks. Oak/Beech, only if you plant.
>
> **F. Critical stop signs: stop marking/approving; exit Play mode WITHOUT saving; close Unity normally; keep the terminal open**
> - E1: black or blank intro or forest; wrong or empty forest geometry.
> - E2: a spray marks a non-entrance tree; the counter jumps by more than 1; the Work Plan shows rack orders for a rack you did not mark; anything approved or felled without your Approve.
> - E3: an Annual Review or history shows work you did not do (#418 provenance is still unverified).
> - E4: Continue loads a different year or forest, or loses marks or orders.
> - E5: any doubt about which save/profile path is in use; any message mentioning your real home save path.
> - E6: repeated lost clicks or a moving mouse pointer during year-comparison clicks.
> - Terminal ends with a STOP. **For S-1 (drift `.mat` only, 37 × OK), report it but do not treat it as a save failure.** For anything else, do not run a second session and do not clean up yourself.
>
> **G. Issue report template**
> ```
> Candidate: 55863f1b (A) | Screen size: ____ | Reduce motion: on/off
> Step (D1–D10): ____ | Year: ____ | Cash: ____
> What I did (keys/clicks): ____
> What I expected: ____
> What happened: ____
> Severity: STOP (E1–E6) / wrong-but-continued / cosmetic
> Repeatable? yes/no/untried | Screenshot: path or none
> Terminal end: CHECK PASS? y/n | final lines | Receipts dir: ____
> host37 all OK? y/n | lane-status lines: ____
> ```

## 5. Gate table

| Gate | Status | What was actually executed in this task | Evidence / remaining condition |
|---|---|---|---|
| **A. Functional (player path)** | **PREPARED-ONLY** (inherited PASS) | Nothing executed. Source/doc inventory, `git diff` lineage checks | 0 functional blockers. Presentation hazard on edited plans (§3.2). Continue/restore UNPROVEN natively (rows 12–13) |
| **B. Rendered / visual** | **PREPARED-ONLY** (inherited PASS) | Nothing executed | #427 720p/1080p; #437 private 720p intro/forest; user real-desktop first view 12 Nov. Real-desktop 1080p remains a live first-visible gate |
| **C. Save / Reference / legacy** | **PREPARED-ONLY** (inherited PASS) | Read-only `git diff` proving the save files are byte-identical since #360; directory listing of the disposable profile (names only) | C408 Legacy40 + frozen Reference anchors; F412 case g; native F9/Continue NOT RUN on A |
| **D. Editor lane + lease** | **RAN (read-only snapshot only)** | At ~2026-11-15 late UTC: ZX20 mounted exFAT, 52 GiB free; lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` on the right branch, HEAD `55863f1b…`, `git --no-optional-locks status` clean, no `Ccf437LaunchProof.cs`; `unity-lock status --json` → `free`, 0 Editor PIDs; `unity-lock` unchanged since 2026-10-15 and CLI-compatible with the launcher; launcher SHA-256 `1d43336e…3b29` = published | **A snapshot, not a held or continuously proven lease.** Physical board #553 is stale. Must be re-proved at launch by `--check-only` + launcher acquire |
| **E. Personal-test safety** | **UNAVAILABLE → CONDITIONAL** | Read-only inspection of the launcher, README and 12 Nov receipts | Needs: (1) #424 re-confirmation of the GO for a second session; (2) owner `--check-only` = CHECK PASS (host hashes UNVERIFIED by me); (3) the S-1 drift-path expectation understood or fixed by an operator; (4) the board row reconciled by its owner |

**Tests executed in this task:** none of the game, Unity, launcher or hashes. **Read-only commands executed:** `git fetch`/`ls-remote`/`show`/`diff`/`log`/`merge-base`; `git --no-optional-locks` branch/HEAD/status in the A lane; `findmnt`/`df`; `unity-lock status --json` and `--help`; `sha256sum` of the launcher script only; `ls` of the launcher directory, run receipts and disposable profile directory; `cat` of the launcher, README and 12 Nov receipt files (no save or prefs contents).

## 6. Three highest-risk gaps

1. **Launcher cleanup is unproven and will likely STOP on drift it does not allow-list.** See `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh` (`allowed=(…)` in `cleanup()`) against `…/runs/20261112T212725Z-624305/agent-completed-cleanup/drift-paths.txt` (`Berry_Calyx.mat`, `Bilberry_Berries.mat`) and `result.txt` (`launcher_cleanup=NOT_RUN`). It looks like a failure even though it is not one, and it blocks the next `--check-only`. Owner/operator fix: extend the allow-list under a reviewed launcher change (which **voids the GO's launcher identity** and needs #424 re-approval), or accept S-1 as documented.
2. **Continue/restore has never been run natively by a human on A** (GO §2.3; F412 §7). The code is byte-identical to the #357/#360-qualified save path, and C408 shows API save/reload JSON/hash equality. The real title → Continue → forest path is still UNPROVEN. Mitigated by doing it last in a throwaway profile (pack D9, stop E4).
3. **The rack-identity presentation hazard on edited plans** (A's `ForestTreeMarkingManager.RackEntrances.cs` `TryGetRackFieldTarget` ID order; `UI/WalkingHudView.cs` `"Rack {fieldRack.Id} entrance"`). It contradicts the user's F-1 rule, and the fix sits on frozen #443. Mitigated by "Reset to suggested layout" plus no edits (pack D2, S-7). Association safety is proven (W438, R450 shared function).

## 7. What I could not verify

- Whether `--check-only` passes **now**: I did not hash the 37 valued host files, the 81-file backup or the 4,011 production files, and did not check the host inventory or licence freshness.
- The contents of the disposable profile `prefs` (exactly which lessons/screens are marked seen).
- Whether the user's 12 Nov session ended cleanly from the Editor's side, beyond the receipt's `normal-shutdown-log(22:07:40Z)` line (I did not read the 2.8 MB `Editor.log`).
- The live state of the physical board and lease **at launch time**: my lock reading is a point-in-time snapshot.
- The user's original "no arrow before first spray" route on their own edited plan: never reproduced (W438, R450).
- Real-desktop 1080p first view; native 720p C/N; per-card pointer restrictions; idle/stale/post-completion history on exact A (inherited PARTIAL).
- The Dashboard / Drive mirror: **UNVERIFIED**.

## 8. Handoff

- **Next owner: #424 Manager**, which decides on a second-session GO (gate E) and, optionally, routes the S-1 launcher allow-list question to an operator. Then the **user**, for the personal test using §4.
- **Minimum user action:** none until #424 re-confirms; then run `--check-only` and, on CHECK PASS, the §4 route.
- No source, `active-tasks.md`, #544 ledger, CCF-Ops board, saves or PlayerPrefs were edited. No Windows build. #443 stays frozen. Candidates B and C are untouched and not claimed playable.
