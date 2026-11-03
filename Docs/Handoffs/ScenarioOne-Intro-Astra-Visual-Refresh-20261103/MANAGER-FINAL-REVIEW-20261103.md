# Scenario One three-page introduction — Manager disposition (2026-11-03)

**Overall Manager verdict: APPROVED for bounded visual implementation with an explicit, narrow Unity Editor startup-exception waiver.** This is **not** a clean onboarding gate PASS, not a completed user playtest, and not authorisation for a Windows/Player build.

## Evidence reviewed

- [#243 implementation](https://github.com/jermur788/CCF/blob/feature/scenario-one-intro-astra-visual-refresh-20261103/Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md), exact published base `72b4efbd`, production change `6bff87dd`, reviewed feature tip `9b64d7a9`.
- [#244 cross-provider independent review](https://github.com/jermur788/CCF/blob/review/scenario-one-intro-astra-independent-20261103/Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/INDEPENDENT-REVIEW-20261103.md), Claude Sonnet 5.5 Medium, read-only; the six actual Unity screenshots, both failing runs, reference atlas/manifest and source diff were inspected by the reviewer.
- Mainline previously captured the exact Unity Editor Search exception signature in `Docs/Verification/Enlarged80Calibration/editor-search-baseline.json`.

## Gate truth and waiver

Both 1280×720 / 1920×1080 focused onboarding runs: **288 checks passed, 1 failed**, `ONBOARDING_FAIL`, exit 1. **Formally FAILED, not green.** The only failed assertion was `NO_EXCEPTIONS_OR_ERRORS` from one `ArgumentOutOfRangeException` in Unity Editor search indexer per run.

I **APPROVE a Manager-acceptance waiver for this precise log exception only**, exact SHA-256 `c6401236fe8905a9570890370f44ea47760c648f2ff3bdec3cea9a283fac147f`, occurring once per run during Editor search indexing. The independent reviewer confirmed identical signatures to known baseline, no CCF stack frames, no other exception/assert, no `error CS`, byte-identical 288 passing checks and six renders across runs, intact Continue and unchanged world. Any other error or repeated/changed exception voids this waiver. Neither the original harness nor its pass criteria is changed or weakened.

**Timing correction:** the exception is logged **after** CCF `Awake`/`Start` diagnostics and **before** the first gate assertion. The implementation handoff's claim “before any CCF code runs” is wrong. The exact cause of this lane's search-indexer fault is not experimentally established. The lane's empty `UserSettings/Search.settings` and missing `Search.index` match documented earlier failure conditions, but no new Unity run was authorised or performed.

## Visual/product verdict

**APPROVED for the approved onboarding presentation change, subject to the user's ongoing real playtest findings.** The independent review opened all six raw rendered screenshots and the three 1920×1080 comparisons. Page 1's exact new teal sentence is present; the mixed-age forest progression shows retained overstorey and avoids a front-edge young-tree-only/clearfell reading. Page 2 no longer shows the blue halo or doubled icons; card 4 copy fits. Page 3 shows several imposing retained Sitka with established broadleaves and smaller regeneration; live navigation/Begin/Continue checks pass. No forestry, model, save, Rack, MFM, Reference Future v1 or Legacy40 change.

**Accepted presentation cautions:** Page 1 art is compressed ~10% vertically to accommodate the added line; Page 2 card 4 has ~10px right-edge margin at 1080p; the already-existing `ForestTest.unity` missing-prefab warning is outside #243 scope. Reassess only if user flags a visible issue during playtest; these did not require retesting/expanding scope.

**Image provenance:** earlier ZIP/source image hashes in `HANDOFF.md` §2 are historical. The latest user-approved Astra originals take precedence under the same handoff §8 and current reference addendum; the actual selected originals are recorded in `specs/scenario-one-three-page-onboarding-v1/references/20261103-art-manifest.json`. Reviewer confirmed internal consistency, not personal knowledge of the source chat. This Manager accepts the documented latest-source precedence without falsely asserting those image bytes equal the earlier ZIP. User feedback may still supersede.

## Integration and machine safety

The feature is a direct descendant of the existing published playtest base, so it can be promoted without reconstruction or further Unity solely for identical Git bytes. **Do not fast-forward `integration/scenario-one-player-test-ready-v1` while the user is actively playtesting that exact candidate.** Prepare/use a separate integration/staging ref, keep the current user test reproducible, then coordinate switching to the newly accepted intro only after the user agrees to change the test baseline.

#243's original Unity lease `7256c06f…` remains **reported HELD**. Only that owner can safely check the deployed lock service/Editor process state and release its own exact lease; no force release. Reviewer found ~28 GB free at last check, superseding an older ~14 GB estimate, but a fresh admission check is still mandatory for any future Unity run. No Unity run is required for byte-identical staging.

#244 review can close as handed off/approved. #243 implementation can close as Manager-approved when staging/status recorded; lock release remains an explicit operational follow-up. No Windows build until completed personal Scenario One playtest and explicit post-playtest user authorisation.

**No Windows build. #194 NO ACTION.**
