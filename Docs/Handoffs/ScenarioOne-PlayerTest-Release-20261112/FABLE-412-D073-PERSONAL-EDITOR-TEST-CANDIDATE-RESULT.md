# Fable #412 — D-073 updated personal Editor test candidate: consolidated result

**12 November 2026. Status: HANDED-OFF FOR MANAGER DECISION.** All authorised phases of the Manager one-go packet ran. Every mandatory gate that this programme owns is green. This report asks the Overall Manager for APPROVED / NEEDS CHANGES / REJECTED and a conditional **PERSONAL EDITOR TEST GO** or refusal for one exact candidate.

This is **not** a public or external demo, a Windows/Player build, Tier 3, a merge to `main`, or user acceptance. No human has played this candidate.

## 1. Exact candidate

| Item | Identity |
|---|---|
| Candidate branch | `integration/scenario-one-personal-playtest-d073-20261112-v1` |
| Candidate commit | `55863f1b7719011bba16b3f3ab377e379c0b988a` |
| Qualified production source | `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72` (the candidate commit adds one handoff document only; `Assets`, `Packages`, `ProjectSettings`, `Tools` identical) |
| Start source | `feature/scenario-one-d073-annual-review-v1-20261112` @ `77a56a4461f8ab2d313082af50442ce24dc54471` |
| Production change | `Assets/ForestPrototype/UI/AnnualReviewView.cs`, +42 / −0, one file |
| Ancestry | Linear. 22 ahead / 0 behind the #382 candidate `b62446caf40fc2ac939d55a6bea6e201f41de700` (20 to `77a56a44`, then the repair and its handoff). No merge. |
| User Editor lane | `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` (lane id ResearchCopyQual391), on the candidate branch, clean, warm Library |
| Launcher | `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh`, SHA-256 `1d43336ea6830b6fb053ebc94b439a289e72da2bf1dcb4bd7ed1684b53623b29` |

## 2. Verdict matrix

| Gate | Owner | Result | Evidence |
|---|---|---|---|
| FR-10 one-file repair (#419 B-1, correction A) | #422, Codex gpt-6.1-sol | Done; offline gates PASS (typecheck 0 errors, helper 53/53, mapper 132/132) | [#422 handoff](https://github.com/jermur788/CCF/blob/feature/scenario-one-d073-fr10-visible-provenance-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CODEX-422-D073-FR10-VISIBLE-PROVENANCE-REPAIR-RESULT.md) |
| Independent G2 source review | #426, Claude Opus 5.5 (Claude A) | **APPROVED**; B-1 closed at source level; 0 Blocker, 0 Major | [#426 report](https://github.com/jermur788/CCF/blob/docs/d073-fr10-independent-g2-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OPUS-426-D073-FR10-INDEPENDENT-G2-REVIEW.md) `f6dea892` |
| Rendered-label oracle and targeted Tier-2 Unity, 1280×720 and 1920×1080 | #427, Codex gpt-6.1-sol then Claude Sonnet 5.5 | **PASS after follow-up**; first run was FAIL (section 4) | [#427 report](https://github.com/jermur788/CCF/blob/qualify/scenario-one-d073-fr10-visible-provenance-unity-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CODEX-427-D073-FR10-ORACLE-AND-TIER2-UNITY-RESULT.md) `ab92cbdf` |
| Independent review of oracle and Tier-2 evidence | #435, Claude Opus 5.5 (Claude A) | **APPROVED**; 0 Blocker, 0 Major | [#435 report](https://github.com/jermur788/CCF/blob/docs/d073-fr10-independent-g2-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OPUS-435-D073-FR10-ORACLE-AND-TIER2-EVIDENCE-REVIEW.md) `fb047b29` |
| Existing #417 artifacts, read-only witness (original candidate only) | #423, Claude Sonnet 5.5 | 5 points WITNESSED, custody PARTLY | [#423 witness](https://github.com/jermur788/CCF/blob/docs/d073-417-existing-artifacts-witness-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CODEX-423-D073-417-EXISTING-ARTIFACTS-WITNESS.md) `0744cb5d` |
| Candidate branch, user lane, backup, launch proof | #437, Codex gpt-6.1-sol | **PASS** | [#437 report](CODEX-437-D073-PERSONAL-PLAYTEST-CANDIDATE-LANE-AND-LAUNCH-PROOF.md) `45288809` |
| Browser readiness matrix | #421, GPT-6 (browser) | Published before the repair: "no new-candidate GO until #412 evidence closes the gates". Read and applied in section 6. It did not inspect the repaired source. | [#421 matrix](https://github.com/jermur788/CCF/blob/docs/d073-personal-playtest-browser-gates-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/BROWSER-421-D073-PERSONAL-TEST-READINESS-MATRIX.md) `a764da44` |

## 3. What the repair does

Each year-comparison measure row (stand and species) now shows three separate entries: the Year A value and the latest Year B value, each with its own `RECORDED` tag from the mapper's `ValueProvenance`, and the difference with its own `DERIVED` tag from `DifferenceProvenance`. With "Show data sources" off, the tag chips are hidden and the dated values and warnings stay, which the reviewer confirmed is what FR-10 asks. Mapper, helper, styles, UiRoot, save/hash, events and tests of earlier tasks were not changed.

## 4. New Unity evidence on the exact source

Required cases, final state:

| Case | 1280×720 | 1920×1080 |
|---|---|---|
| a. Compile/import of exact candidate, 0 compiler errors | PASS | PASS |
| b. Sources ON: separate RECORDED observations and DERIVED difference, stand and species rows | PASS (409 checks) | PASS (409 checks) |
| c. Sources OFF: chips hidden, values and warnings kept | PASS (409 checks) | PASS (409 checks) |
| d. Native foldout, scroll, caveat, year controls, latest hint | **FAIL in first run**, PASS with corrected probe | PASS |
| e. Missing snapshot, ambiguous snapshot, event-only warning | PASS | PASS |
| f. ACK/footer priority; viewing changes no saved state, hash or events | PASS | PASS |
| g. Genuine v9 old save and frozen Reference anchors | PASS | PASS |

- **Negative control:** the same oracle run against the unrepaired `77a56a44` view failed 170 of 209 checks for exactly the B-1 symptoms.
- **First run, recorded as it happened:** 47 cases, 42 PASS / 5 FAIL. The failures stay on record unedited.
  - **F-1 (one case):** an added 720p native check that closes the species foldout failed. Follow-up replayed the unchanged original probe 15 of 15 times PASS (9 candidate, 6 unrepaired base), and human-paced clicks behaved the same on both. The independent reviewer then showed from driver timings that the pointer was being moved away during a ~25 s window of the first run and that **two** clicks were lost, one on the "Later year" button that the repair does not touch. Disposition: test-harness defect, not a regression. **What moved the pointer is not known.**
  - **F-2 (four cases):** an extra fixture reused a management-event ID and was correctly rejected by save validation. A valid distinct-ID fixture was added in a new file and passes with sources ON and OFF at both sizes.
- **Not re-run after the follow-up:** b, c, e, f, g at 1080p and f, g at 720p rely on the first run. The reviewer accepted this because neither production code nor the tests for those cases changed in between.
- **No old assertion was edited, deleted or weakened** (reviewer: 16 files added, none modified; 7 inherited files byte-identical to #417).

## 5. Historical evidence (original candidate `77a56a44` only)

The #417 qualification (99 checks, 18 screenshots, old v9 save, frozen Reference v12, five normal lease releases) was re-witnessed read-only from raw files. It supports the unrepaired candidate and inherited systems. It does not by itself qualify the repaired source. One gap: the "4,011 production hashes unchanged" verdict for #417 rests on that worker's own receipt.

Inherited from #381/#382 and still open, unchanged by this programme: locked-card pointer hits and native C/N at 720p not fully proved, idle/stale history and post-completion management not run, several older offline checks FAIL or PARTIAL. See issue #382.

## 6. #421 mandatory gates — status

| #421 gate | Status |
|---|---|
| Exact qualified new source, independently reviewed, isolated branch and lane | Met (sections 1, 2) |
| Named human owner and exclusive Unity lease | Board row names the user as playtest owner pending Manager GO; the launcher takes and releases the lease per session. **User must run `--check-only` at launch time.** |
| Save/prefs isolation **and** full backup | Both. Launch proof showed `Application.persistentDataPath`, `forest-save.json`, `.bak`, `.tmp` and PlayerPrefs resolving inside `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/profile/config/unity3d/DefaultCompany/CCF/`; a native F5 wrote there and nothing in the host location changed. Independent backup of 81 host files, byte-verified, at `/media/jer/Files/CCF/Archives/PersonalPlaytest-D073-HostBackup-20261112/`. |
| Do not rely on the one-slot `.bak` | Met by the separate backup and the disposable profile |
| Host integrity and space | 37 valued host files hash-identical before/after every session; 4,011 production hashes identical; ZX20 68 GiB free |
| Exact-source rendered evidence at both sizes, old-save/Reference smoke, #417 witness | Met (sections 4, 5) |
| Manager conditional GO for this exact source | **Outstanding. This report requests it.** |
| First visible gate: intro and forest render | Worker proof at 1280×720 on a private display: intro page 1 legible, forest renders. The user's own first look on the real desktop is still the binding gate. |

## 7. Defects, risks and NOT RUN

- **Watch item:** missed clicks on the "By species" foldout or the Earlier/Later year buttons. Cause of the first-run pointer displacement unknown.
- **Minor, source (#426 m-1):** the old one-line table header is still drawn above columns that now repeat the same captions. Redundant, mislabels nothing. Left as is so the qualified source did not move.
- **Minor, tooling (#426 m-2):** an older S1C T-04 view audit already fails on the base (3 offenders) and the repair adds a 4th. Needs a test-only allowlist entry by a test owner.
- **Note (#426 N-1):** "N year(s) later" is a calculated phrase shown under a RECORDED chip, as the approved spec layout draws it. Mapper text, outside this repair.
- **Note:** each measure now takes about four short lines, so the open species section is roughly twice as tall. Screenshot review at 720p found every value, tag and the fixed caveat reachable by scrolling.
- **Oracle gaps (#435 m-2):** no overlap check between value, heading and tag; chips on non-measure lines and elements outside the mapper's output are not checked.
- **Launcher:** after its host-inventory check was corrected, only `--check-only` was re-run; the final script bytes have not driven a full Editor session. The proof session used a private display, not the real desktop.
- **Host write outside the valued set:** Unity rewrote its global Editor window layout (`~/.config/unity3d/Preferences/Editor-5.x/Layouts/current/default-6000.dwlt`). Not a game save; original bytes are in the backup.
- **NOT RUN:** full #417 99-check and #408 143-case suites, Reference biological continuation, native F9 load, 1080p intro/forest proof, any gameplay beyond intro → forest → F5 on the candidate lane, Player/Windows build, any human playtest.
- **#418** (MFM order/event ID) is untouched. If the history card shows work you did not do, treat it as a stop.
- **Dashboard / Drive mirror synchronisation: UNVERIFIED.** No canonical mechanism was run.

## 8. Custody and protection

- **Unity lock:** 9 leases across #427 and #437, all released normally, none forced, no Editor killed. One lease was orphaned when a worker hit a usage limit and was closed out normally by its successor. Lock FREE at handoff; no Editor running.
- **#382 preserved:** branch `integration/scenario-one-personal-playtest-candidate-20261111-v1` still `b62446ca`; its worktree `CCF-S1-Playtest-Readiness-Sol` and Editor lane `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` (`2aaacad6`) were not opened or written.
- **`main`** not modified. `/media/jer/Files/CCF-Primary` still has only its two pre-existing dirty files.
- **Lane drift:** Editor runs dirtied ten known files (`.vscode/settings.json`, three Forb, two Bilberry, four Fungi materials); bytes archived, restored to HEAD by path. Lane clean.
- **Ops board:** row `CCF-412-PREDATOR-FABLE-A-D073` updated at start and at handoff (lint PASS both times); `CCF-413-HP-…` set DONE-PENDING-LEDGER. Manager-owned `active-tasks.md` not edited.
- **Accounts, recorded plainly:** the dispatch names Claude A for the parent; T3 reported the parent thread on Claude B — Pro. Children: Codex gpt-6.1-sol on both OpenAI subscriptions (three usage-limit cut-offs), Claude Sonnet 5.5 and Claude Opus 5.5 on Claude A.

## 9. Evidence locations

- `/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/` (Tier-2, negative control, follow-up)
- `/media/jer/Files/CCF/Verification/D073-FR10-426-20261112/` (source review gate re-runs)
- `/media/jer/Files/CCF/Verification/D073-PersonalPlaytest-437-20261112/` (launch proof)
- `/media/jer/Files/CCF/Verification/D073-YearComparison-417-20261112/` (historical, untouched)

## 10. Paste-ready launch message for the user (valid only after Manager conditional GO)

> **Personal Editor playtest — D-073 candidate `55863f1b`.** Internal test only. Your real saves are backed up and the game will save into a separate throwaway profile.
>
> 1. Close any other Unity Editor. In a terminal on the Predator run:
>    `/media/jer/Files/CCF/Operations/PersonalPlaytest-D073/launch-personal-playtest.sh --check-only`
>    It must print `CHECK PASS`. If it prints `STOP`, do nothing else and send the message back.
> 2. Launch with the same command without `--check-only`. Keep the terminal open. Always launch this way; opening the project from Unity Hub skips the save protection and the lock.
> 3. In Unity (scene `Assets/Scenes/ForestTest.unity`) press **Play**, then **Start New Scenario**. Read the three intro pages (Next / Back / Begin). You should then see the forest floor and trees.
>    **If the intro or forest is black or blank: stop, leave Play Mode without saving, close Unity, and report it. Do not press F5.**
> 4. Suggested 15–30 minute route: Objectives (O) → Stand Map (M), plan and confirm five parallel racks from the western headland → walk in 3D and paint rack entrances (P) → inspect a tree (E), mark one Crop (C) and one Fell (X) → Work Plan (Tab), **Approve**, then separately **Advance** one year → Annual Review: read, then acknowledge → open an earlier year and compare with the latest.
> 5. In the year comparison check that each "End of Year" value carries its own **RECORDED** tag and the difference carries **DERIVED**; open "By species"; toggle "Show data sources"; scroll to the caveat. Optional: an MFM sample and a second intervention.
> 6. Please note anything like: a click on "By species" or the year buttons that does not register, history showing work you did not do, anything unreadable at your screen size.
> 7. F5/F9 only touch the throwaway profile. To finish: press Play again to leave Play Mode, close Unity normally, do not save scene changes if asked, and let the terminal finish; it checks your real saves are unchanged and releases the lock.
>
> Known limits: this is not the external demo build; some older checks from #381 remain open; no one has played this candidate yet. You decide acceptance after playing.

## 11. Requested Manager decision

1. APPROVED / NEEDS CHANGES / REJECTED for candidate `55863f1b` (source `339517ed`).
2. Conditional PERSONAL EDITOR TEST GO, or refusal, for the lane and launcher in section 1.
3. Reconcile `active-tasks.md` and the dashboard; decide whether m-1 (redundant header line) should be removed in a later change.
