# Manager #424 — Scenario One D-073 personal Editor test: CONDITIONAL GO

**Date:** 12 November 2026. **Manager verdict: APPROVED for ONE CONDITIONAL PERSONAL UNITY EDITOR PLAYTEST on the exact candidate below.** This is **not** whole-Scenario-One/Tier-3 qualification, project `main` integration, external-demo readiness, Player/Windows build permission, user playtest completion or user acceptance. **No one has personally played the candidate.** All conditions below must pass on the live Predator desktop before/during the user's session, or the GO is void and the launch must STOP.

**Source of authority:** User-assigned Scenario One Delivery Manager [#424](https://github.com/jermur788/CCF/issues/424) reviews [Fable #412 consolidated report](https://github.com/jermur788/CCF/blob/bc15e697f1a166c4faa6dadeca053e4a06d19574/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/FABLE-412-D073-PERSONAL-EDITOR-TEST-CANDIDATE-RESULT.md), independent G2 source [#426](https://github.com/jermur788/CCF/issues/426), scoped G3 Unity [#427](https://github.com/jermur788/CCF/issues/427), independent raw-evidence [#435](https://github.com/jermur788/CCF/issues/435), launch/custody [#437](https://github.com/jermur788/CCF/issues/437) and independent [#421 preflight matrix](https://github.com/jermur788/CCF/blob/docs/d073-personal-playtest-browser-gates-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/BROWSER-421-D073-PERSONAL-TEST-READINESS-MATRIX.md). This manager used GitHub source/reviewer handoff only; local raw bytes, GUI and machine were checked by distinct Predator workers, **not mounted or re-executed here**.

## 1. Exact release and no-change contract

- **GitHub branch:** `integration/scenario-one-personal-playtest-d073-20261112-v1`.
- **Exact candidate tree:** `55863f1b7719011bba16b3f3ab377e379c0b988a`.
- **Qualified game production source:** `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72`; one-file `AnnualReviewView.cs` correction (+42/−0). Candidate adds only documentary material to game production source.
- **GitHub lineage independently checked by #424:** 22 ahead/0 behind prior #382 candidate and no merge. No change of `main` or original #382 worktree.
- **Exact playtest Unity lane:** `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` (ResearchCopyQual391), reported clean/warm and parked on candidate.
- **Exact owner-operated launcher:** `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh`. Last published SHA-256 `1d43336ea6830b6fb053ebc94b439a289e72da2bf1dcb4bd7ed1684b53623b29`. A different or modified launcher is **not** covered.
- **Separate valued-data backup:** `/media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112/`; #437 reported verified copies of 81 files under both host Unity configuration roots. **The backup is outside the disposable profile.**
- **Disposable profile:** `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF/`. The actual previously launched process reported `Application.persistentDataPath`, three save destinations and PlayerPrefs here; native F5 wrote here and host game-data paths did not change.
- **Engine:** Unity 6000.6.0f1 on Predator. No direct Unity Hub launch; only exact guarded launcher.

The child #437 last published custody receipt showed the 37 valued host files and 4,011 protected production hashes byte-identical, an Editor-free machine with unlocked Unity lane, and the parent's physical Ops row WAITING with the **user** named as playtest owner. Historical status is not a new lock. The launcher must freshly prove live ownership/eligibility.

## 2. Manager's review and rationale

**APPROVED, narrowly.** #426 independently closed #419's major FR-10 source issue: separate Year A and Year B **RECORDED** observations plus **DERIVED** difference. #427 ran actual revised-source Unity 720p/1080p rendering, negative control failing the unrepaired view 170/209 times, native controls, old v9/frozen Reference fixtures and nonmutation. #435 read raw evidence and approved despite the preserved initial 42 PASS/5 FAIL run. Four first-run failures were invalid duplicate-event fixtures; one 720p probe lost clicks during OS pointer displacement. Independent review discovered a *second* lost click on an unrelated year button and corroborated non-product/test-harness timing; **the moving-pointer cause remains unknown**. No blanket #417/#408 re-test was ordered.

**Known limits remain explicit:**
1. #437's one real isolated Editor session used private display 1280×720, showed three intro pages and rendered forest, and F5 wrote to disposable profile. **Real normal-desktop opening and 1080p intro/forest weren't separately proved**. These are live human first-visible stop gates, **not accepted results**.
2. #437's original launcher cleanup returned nonzero because an extra check expected the global Unity window-layout preference to remain unchanged; Unity changed that file and the **pre-session copy exists in backup**. The 37 valued game files and complete game-data paths stayed unchanged. The final launcher changed only the over-broad host-inventory cleanup check, retaining the same Editor/start/isolation/lease logic; its new bytes passed `bash -n`, a real-desktop `--check-only` and a missing-DISPLAY negative test, **but a full Editor lifecycle with those final bytes is NOT RUN**. This is accepted as bounded risk for a *guarded internal personal* test, not a fully verified launcher. Unexpected failure or incomplete cleanup is **STOP and report**, not an invitation to bypass the guard.
3. **Native F9 load NOT RUN** on this new isolated profile. The shared `SavePath` destination was inspected, not a human F9 behaviour test. Do not use F9 with valued data or treat it as certified. Use only the fresh disposable profile; test F9 later only if the actual save isolation has remained proven and you are willing to lose that throwaway progress.
4. Native pointer interference: watch `By species` and earlier/later-year clicks; unexpected clicks/disabled UI or moving pointer = record and stop if material.
5. Redundant old table-header line is **MINOR / NON-BLOCKING**; do not edit the now-qualified source just to remove it. Baseline S1C T-04 checker already FAILS on three untagged items and has one additional new caption; retain as narrowly scoped tooling debt. Older #382 findings and #418 unverified genuine MFM event/prescription mapping remain disclosed. Any UI claim that unrecorded work was executed = **STOP**.
6. No separate local source/tree/release verification executed by Manager; results are independently published worker/reviewer evidence and directly inspected GitHub ancestry/file diff. Dashboard/Drive synchronization **UNVERIFIED**.

## 3. Mandatory live release conditions (NO-GO if any fail)

Before Editor launch, the **user on Predator** must close all competing Unity Editors, keep the older #382 Editor lane untouched, run the **exact** script's `--check-only` on the normal desktop and receive **`CHECK PASS`**. The guard must confirm expected source/branch/clean worktree, launcher identity, Unity installation/version, current mounted ZX20 and 15 GiB free, licensed assets, actual no-other-Editor/real machine-wide lock eligibility, intact originals/backup, and disposable save profile. If output says STOP or any unexpected state, **do not open Unity** and return the exact terminal output. Do not bypass checks, force locks, overwrite files or run Unity from Hub. The launcher, not this browser Manager, must acquire its lease for the session and release only after normal Editor exit.

Inside the guarded launched Unity Editor, the user checks that the three-page introduction is readable and first forest view is visibly rendered. **Black/blank intro or forest, incorrect forest geometry, save/prefs path doubt, misleading work history, unexpected destructive work, serious native input or UI blocker ⇒ immediately exit Play Mode without saving, close Editor normally, leave terminal active to complete its checks, and report. Do not press F5 in that condition.** If cleanup fails or changes valuable files, do not run a second session or manually delete/restoring paths; report and protect evidence.

A complete human first-visual gate and eventual gameplay experience are **user observations**, not inherited automatic test results. If the exact candidate or launcher changes, this conditional GO expires and new review is necessary. No Windows build regardless of test outcome without a subsequent explicit user decision.

## 4. Paste-ready user playtest instructions — valid only while live gates pass

1. On the **Predator Linux desktop**, close other Unity Editors. Open a terminal and run:
   ```bash
   /media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh --check-only
   ```
   **Must show `CHECK PASS`.** Any STOP/error means no launch; share the output.
2. Run exactly:
   ```bash
   /media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh
   ```
   Keep the terminal open. Do **not** open the candidate through Unity Hub, which bypasses the save/lock guard.
3. In Unity, check scene `Assets/Scenes/ForestTest.unity`, press **Play**, **Start New Scenario**, navigate three intro pages with Next / Back / Begin. Confirm textured ground, trees and UI. **If black/blank or unsafe, stop immediately without F5/save.**
4. Suggested short genuine user route: Objectives (**O**) → Stand Map (**M**) → plan/confirm existing western-headland five parallel racks → walk and paint entrances (**P**) → inspect a tree (**E**), mark one Crop (**C**) and Fell (**X**) → Work Plan (**Tab**) **Approve**, then separate **Advance** → Annual Review read/ACK → select an earlier year and compare with latest. Observe separate **RECORDED** on Year A and Year B versus **DERIVED** difference, expand `By species`, toggle `Show data sources` and scroll to caveat. Optional: multi-scale sample, Oak/Beech planting, another intervention.
5. Note any nonregistering clicks, movement of OS pointer, confusing header/data, unexpected history, black visuals, save risks or other reproducible problems. Record screen size, year, what was clicked and Console errors/screenshots if safe. Do not assume a failed click is harmless.
6. To finish, leave Play Mode, close Unity normally, **do not save scene changes** if prompted; allow guarded terminal to finish integrity checks and release the machine-wide lease. Report actual result and whether save/hash checks passed. Do not manually clean/force-release on failure.

## 5. Follow-on programme

- **Next owner: user.** Personal playtest is now authorised **conditionally**, not requested to be marked complete. On feedback, #424 reviews runtime/UX issues and makes bounded task packets where actual product defects are supported.
- #418 authentic MFM event/order/prescription provenance verification remains **READY/NOT RUNNING**, separate from #412. Do not start it on the current machine/user playtest lane while the user may run this protected Editor session; assign a distinct safe lane only after live locks and owner consent. Do not change save/event/hash policy without #425/user decision.
- The initial personally playable candidate is **not** the full complete-loop three-species external demo; subsequent S1-C/S1-D, MFM, parity, Workbench and genuine Tier-3 milestone gates remain separately owned. #425 owns product-authority files.
- #424 updates live GitHub coordination after fresh SHA; #412 stays HANDED-OFF/CLOSED. No new Fable orchestration, no game source/main merge and no Windows/Player build authorised.
- Dashboard published readback/Drive sync not verified; no canonical context-source file changed as part of this docs-only verdict.

**User action required:** run the two guarded commands and personally play only if live preflight and first-visible checks pass; then provide feedback. No additional chat/agent dispatch needed.
