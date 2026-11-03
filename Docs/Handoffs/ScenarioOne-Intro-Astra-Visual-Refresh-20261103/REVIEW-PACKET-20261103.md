# #244 — Independent review of Scenario One intro refresh and Unity Editor startup exception

**Issued:** 2026-11-03 by Overall Manager #200.  
**Classification:** review-only, no Unity or implementation. **Difficulty: Medium** — classifying two failing runs and checking six rendered references requires careful evidence inspection.  
**Recommended reviewer:** Claude Sonnet 5.5 Medium (cross-provider to OpenAI Astra implementation), read-only T3 Code/local evidence access.  
**Claim:** https://github.com/jermur788/CCF/issues/244  
**Implementation claim:** https://github.com/jermur788/CCF/issues/243  
**Implementation source:** `feature/scenario-one-intro-astra-visual-refresh-20261103`; base: published `integration/scenario-one-player-test-ready-v1` at task start.  
**Local lane:** inspect the existing **clean** verification lane `/media/jer/ZX20/CCF-Intro-Astra-20261103` **read-only**; avoid creating another heavy worktree. Claim #243's worker remains sole production writer until Manager close. Inspect latest live `Docs/Project/coordination/active-tasks.md` first.

## Manager checkpoint

**NEEDS MORE EVIDENCE; no product defect established.** The onboarding gate **FAILED twice**, each time **288/289**. The sole failing assertion `NO_EXCEPTIONS_OR_ERRORS` caught `ArgumentOutOfRangeException` reportedly entirely in Unity's Editor search indexer on startup (`UnityEditor.Search.SearchDatabase.GetDefaultSearchDatabase` / `SearchInit.IndexationOnStartup`). No CCF frames were reported. A previous 25 October run on another lane passed, but lane-specific causation has **not** been proved. The Manager selects **option (a), a narrowly documented, exact-exception waiver** *only if independently substantiated*. No harness filter, hidden assertion exclusion, or third full Unity run is authorised.

## Evidence to examine

- Return: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md` on the feature branch.
- Raw proof: `/media/jer/Files/CCF/Verification/ScenarioOneIntroAstra-20261103/run-01/`, `run-02/`, `comparisons/` (six side-by-side sheets).
- Source/test: `ScenarioOneOnboardingCopy.cs`, `ScenarioOneOnboardingView.cs`, `OnboardingIcon.cs`, `Resources/Onboarding/page-{1,2,3}.png`, and `Tools/Verification/ScenarioOneThreePageOnboarding/`.
- Approved reference manifest `specs/scenario-one-three-page-onboarding-v1/references/20261103-art-manifest.json` with actual used image hashes; older 25 October references remain historical.

## Mandatory review tests (without executing Unity)

1. Locate exact exception(s) and timings in both Editor logs. Prove whether all frames belong to Unity Editor Search indexing and whether the event predates CCF play logic; count every other warning/error/exception and distinguish normal Editor startup noise. Inspect gate result/check files directly. **Retain `ONBOARDING_FAIL` and exit 1 in report**.
2. Determine if the exception could have compromised image rendering, UI input, save/Continue tests or genuine app runtime. If attribution is uncertain, recommend **NEEDS CHANGES**, not a waiver. Never weaken `NO_EXCEPTIONS_OR_ERRORS` in code.
3. Open **all six** actual Unity images (both 1280×720 and 1920×1080 for pages 1, 2 and 3) and compare to approved sources/sheets. Verify Page 1 exact highlighted renewal line and visual forest slices (no front sapling row or clearfell implication), Page 2 no blue halo/stacked icons/card-4 overflow, Page 3 imposing retained mature Sitka and mature broadleaves, no clipping. Distinguish automated assertions from images directly viewed.
4. Review changed-file/asset boundaries, provenance/hashes, cleanup receipts and preserved original historical reference files. No gameplay/save/Stand Map/Reference Future/Legacy40 drift; report any uncertainty.
5. Output **APPROVED** only for an independently proven, exact Unity Editor startup exception exclusion from the Manager *acceptance decision*, **not** a green gate. Otherwise **NEEDS CHANGES** (or REJECTED if warranted). Explain any visual discrepancies precisely.

## Return and operational safety

Publish `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/INDEPENDENT-REVIEW-20261103.md` on a **non-overlapping docs-only review branch** with issue comment and link, or provide complete read-only result for Manager publication. Do not write in #243's feature worktree. Mark claim [RUNNING] before substantial work with reviewer/provider/lane/head; [HANDED-OFF] on completion.

**Held lock:** #243 owner must inspect live machine state then **release only its own exact Unity lease**, using deployed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock` and record receipt. Browser Manager cannot perform local release. No Unity Editor should remain; do not force-unlock another owner. Current ZX20 free space ~14 GB, below 15 GiB admission floor: **do not launch Unity or delete arbitrary files**. Request a separately authorised safe cleanup plan if another Unity run later becomes necessary.

No integration or formal waiver before independent review and Manager verdict. Personal Scenario One playtest remains underway; **no Windows build** before its completion and explicit post-playtest user authorisation. No full Tier-3 rerun.

## Paste-ready dispatch

> Review CCF #244 read-only: https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/REVIEW-PACKET-20261103.md . Inspect #243's two raw Unity logs, six actual screenshots, approved references, verification check and source/asset delta. Confirm or reject the *exact* UnityEditor.Search startup-exception-only waiver without changing the harness or running Unity. Publish INDEPENDENT-REVIEW-20261103.md with APPROVED / NEEDS CHANGES / REJECTED, link #244, and return to Browser Overall.