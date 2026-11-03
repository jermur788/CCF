# Scenario One intro visuals — isolated staged integration

**Date:** 2026-11-03. **Status:** STAGED / MANAGER-APPROVED / CURRENT USER PLAYTEST BASELINE UNCHANGED.  
**Claim:** [#245](https://github.com/jermur788/CCF/issues/245). **Parent implementation:** [#243](https://github.com/jermur788/CCF/issues/243). **Independent review:** [#244](https://github.com/jermur788/CCF/issues/244).

## Exact publication

The source branch `feature/scenario-one-intro-astra-visual-refresh-20261103` was inspected at reviewed tip `9b64d7a93840663c35f06f9952e1e9dd7641764f`. GitHub compare showed this branch was **3 commits ahead, 0 behind**, of published player-test base `integration/scenario-one-player-test-ready-v1` at `72b4efbd1c653fa2231f632a79030a83ff76f833`.

Browser Overall created **new** `integration/scenario-one-intro-visual-refresh-v1` directly at that exact reviewed source commit and fetched the resulting branch to confirm the same commit (Git tree `11d9473691cf442fceff488c782eaa9b50e27624`). **No cherry-pick, rebase, merge rewriting, local workspace write, Unity run or change in qualified bytes** occurred. Exactly the reviewed 17 changed paths are included (3 runtime art atlases, 3 onboarding UI code files, focused verification and non-destructive reference/manifest/handoffs). No protected simulation/save/Stand Map/Rack/MFM/Reference Future/Legacy40 files differ.

**The existing user-playtest branch was separately rechecked and stays unchanged at its original head.** The user's in-progress test is therefore not silently switched.

## Acceptance / evidence boundary

Manager approval: [MANAGER-FINAL-REVIEW-20261103.md](MANAGER-FINAL-REVIEW-20261103.md). Both focused Unity onboarding runs remain **formal FAIL**: 288/289 each, `ONBOARDING_FAIL`, Editor exit 1, due to once-per-run known UnityEditor.Search indexer startup exception. The Manager has accepted only an **exact-signature exception-specific waiver for the acceptance decision**, following independent read-only review. The test and its logs remain unmodified; **do not label them green**. All other onboarding checks passed and six rendered screenshots were independently inspected. The original implementation has not had a clean gate run.

No new qualification was necessary solely to publish the identical Git tree as a new ref. The user may still report visual concerns, and the personal playtest is **not completed or accepted**.

## Next and safety

Do not update `integration/scenario-one-player-test-ready-v1` or direct the user to switch test branches mid-session without an explicit decision coordinated with the current user playtest. The newly staged candidate is available under `integration/scenario-one-intro-visual-refresh-v1`; it is not the current user-playtest baseline. Future promotion to the main integration lineage must preserve exact candidate identity and observe live ownership.

The **held #243 Unity lock** remains separate from browser-only staging. Only its **original holder** can inspect deployed machine lock/process metadata and release the exact lease; record receipt on #243. The independent reviewer reported ~28 GB free at its check; recheck rather than assume future capacity. No further Unity is requested now.

**No Windows/Player build** before user personally completes Scenario One playtest and explicitly authorises it afterwards. No user acceptance or personal playtest completion is inferred. #194 NO ACTION.

Success token: `INTRO VISUAL REFRESH STAGED — PUBLISHED PLAYTEST BASELINE UNCHANGED`.
