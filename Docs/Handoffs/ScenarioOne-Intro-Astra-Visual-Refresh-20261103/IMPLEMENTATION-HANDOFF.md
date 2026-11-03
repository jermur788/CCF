# Scenario One — Astra intro visual refresh: implementation handoff (#243)

**Date:** 2026-11-03
**Task:** [#243](https://github.com/jermur788/CCF/issues/243) — see `HANDOFF.md` in this folder on `main`.
**Branch:** `feature/scenario-one-intro-astra-visual-refresh-20261103` (also local lane branch `verify/scenario-one-intro-astra-20261103`, same commit)
**Implementation commit:** `6bff87ddf2b39573ed0998c3621e820cb9cd3770` on base `72b4efbd` (published `integration/scenario-one-player-test-ready-v1` head at handoff creation).
**Status:** **IMPLEMENTED; UNITY-RENDERED AT 1280×720 AND 1920×1080; GATE VERDICT = FAIL BY THE HARNESS'S OWN CRITERIA (1 of 289 checks, Unity-internal exception, reproduced twice). NEEDS MANAGER DISPOSITION. NOT INTEGRATED. NO WINDOWS BUILD.**

Two agents contributed: an earlier Codex session made the commit and started the gate; this Claude Sonnet 5.5 session waited for that gate, inspected the evidence, reran it once, built comparison sheets and wrote this handoff. No code was changed by this session.

## 1. What changed (UI/art only)

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/Resources/Onboarding/page-{1,2,3}.png` | Replaced runtime art atlases with art prepared from the three approved 3 Nov screenshots (Editor chrome removed; forest regions and blurred margins only). Existing `.meta`/GUIDs untouched. |
| `Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs` | Added `Renewal` constant (exact Page 1 sentence); Page 2 card 4 text now "Encourage natural regeneration and establish other species for a diverse forest." |
| `Assets/ForestPrototype/UI/ScenarioOneOnboardingView.cs` | Page 1: live `onboarding-accent` line (name `onboarding-renewal`) under the opening paragraph; divider moved to y=291; forest art compressed 322→289 design px, no slice removed. Page 2: card 4 font 18 like the others. Page 3: art frame taller (y=225, h=365) so the tall retained Sitka crowns are included. `Art()` gained an optional destination rect. |
| `Assets/ForestPrototype/UI/OnboardingIcon.cs` | "retain" icon: tree plus check mark (no leaf/glow); "logs" icon: concentric rings. |
| `Tools/Verification/ScenarioOneThreePageOnboarding/` | Harness checks for the new sentence/colour/position, card-4 fit and Page 2 genericity; `prepare_20261103_art.py`; `make_comparisons.py --refresh-20261103`. |
| `specs/scenario-one-three-page-onboarding-v1/` | `production-copy.md` update; new `references/20261103-*` records. **Prior 25 Oct references and hashes are unchanged history.** |

No simulation, economy, save-format, Reference Future v1, Legacy40, world-anchor, Stand Map or MFM code was touched.

**Exact Page 1 sentence** (verbatim, `onboarding-accent` teal, no new colour token):
"The forest is renewed gradually over time, creating a mixed-aged, more resilient and productive woodland."

Runtime atlas SHA-256 (also in `20261103-art-manifest.json`):
- page-1: `b775eb9515dbb4a8e0916359e3f15f3ecb36701e70ddc33e832a2fece21b01ec`
- page-2: `95ccacdf87039b48a4e2cf8705e7da9d26c72a82ad0a1ee193b0cf286022c8ee`
- page-3: `83596a3d02b4a15f88e73d18587dea95cd5e39c86e4906c73502b8db7c488168`

**Source-image note.** `20261103-art-manifest.json` records the actual selected originals (the latest approved Astra images, per the handoff's section 8 precedence). The ZIP hashes in `HANDOFF.md` section 2 describe an earlier set and are not claimed for these bytes.

## 2. Verification actually performed

Disposable harness `Tools/Verification/ScenarioOneThreePageOnboarding/run_gate.py`, Unity 6000.6.0f1, private Xephyr display, isolated config/save (the user's save and playtest were not touched), under machine lock `7256c06f…` (owner Astra-243, lane `/media/jer/ZX20/CCF-Intro-Astra-20261103`, HEAD `6bff87dd`).

| Run | Result |
|---|---|
| `run-01` (fresh Library import, 711 s) | 288 pass, **1 fail**, 6 screenshots |
| `run-02` (warm Library, 97 s) | 288 pass, **1 fail**, 6 screenshots — same failure |

Evidence root: `/media/jer/Files/CCF/Verification/ScenarioOneIntroAstra-20261103/` (`run-01/`, `run-02/`, `comparisons/`, `unity-editor-side-effects.patch`). Large evidence stays external per AgentWorkflow; this file is the GitHub copy of the findings.

**The one failure:** `NO_EXCEPTIONS_OR_ERRORS` — an `ArgumentOutOfRangeException` whose entire stack is `UnityEditor.Search.SearchDatabase.GetDefaultSearchDatabase` ← `SearchInit.IndexationOnStartup` (Editor "Start Indexing on Editor startup"), logged before any CCF code runs (`run-02/unity.log` line 1059). It contains no project frames. The same check passed in all four 25 Oct onboarding runs on a different lane, so the cause appears to be this lane's Editor/Library state, but I did **not** prove that (no baseline run on this lane — see section 4).

Under the README ("Success requires Unity exit 0, `ONBOARDING_PASS`, zero `ONBOARDING_CHECK_FAIL`"), **both runs are formally failed** (Unity exit 1, no pass marker). I did not edit the harness to filter the exception.

Checks that passed at **both** 1280×720 and 1920×1080 (from `run-02/evidence/checks.txt`):
- Normal startup; no onboarding behind startup; exact capture size; modal input on each page; live titles; old intro hidden.
- Page 1: exact renewal sentence, existing heading accent colour, placed below the opening copy; all labels fit (`text-overflows.txt` empty).
- Page 2: card-4 exact copy inside the card; page stays generic (no scenario controls).
- Page 3: three shared columns, baselines, equal widths, both species named.
- Back/Next hit targets; Begin restores forest input and cursor.
- Continue path with a saved forest (run once, at 1920×1080, after the Begin check): `CONTINUE_AVAILABLE`, `CONTINUE_NO_ONBOARDING_WITH_FRESH_PREFS` (onboarding not replayed), `CONTINUE_INPUT_RESTORED`, `CONTINUE_WORLD_PRESERVED`.
- Pure-UI navigation leaves the world hash unchanged (`A0E0B2A51CC9274A` before and after); a saved forest is valid.

Screenshots (SHA-256 prefix, `run-02/evidence/`): p1-720 `692efe3d`, p1-1080 `9c837403`, p2-720 `8b064964`, p2-1080 `c59501b9`, p3-720 `b1b35924`, p3-1080 `1da6f2c4`. Six side-by-side sheets with full hashes are in `comparisons/` (`comparison-manifest.json`).

**Visual review (by this session, from the real Unity renders):** Page 1 matches the approved composition; the teal line sits cleanly between the paragraph and the divider at both sizes and does not wrap at 1280. Page 2: five cards, revised icons, no blue glow, no doubled icons in cards 2–3, card 4 text fully inside. Page 3: several tall mature Sitka with Oak/Beech and regeneration on the right; no clipping. I viewed Page 1 at 1920×1080 and Pages 1–3 at 1280×720 directly; Pages 2–3 at 1920×1080 are covered by the automated checks and comparison sheets but were not opened individually by me.

## 3. Not verified

- A clean (exit 0, `ONBOARDING_PASS`) gate run.
- OS-native mouse/keyboard delivery (the harness uses queued New Input System events and real panel hit-target checks only).
- Pixel-exact equality with the screenshots; the live-UI architecture means close visual match, with the intended differences (the added Page 1 line and its spacing).
- No ecological year advanced; no Player/Windows build (user playtest in progress).

## 4. Decision needed from the Overall Manager

1. **Disposition of the Unity-internal exception.** Options: (a) accept 288/289 with a documented waiver; (b) approve a narrowly scoped harness filter for exceptions whose stack is wholly inside `UnityEditor.Search` before the harness starts; (c) rerun on a lane where the Search index exists (the 25 Oct lane) once free space allows.
2. **Storage:** `/media/jer/ZX20` is at ~14 GB free, below the 15 GiB `unity-lock` floor. I did not delete anything. Release/cleanup of this lane's Library and the lock (`7256c06f…`) is left to the Manager.
3. Independent review before integration; do not auto-merge into the playtest candidate.

## 5. Housekeeping

- The harness `.cs`/`.meta` and the Unity licence copy were removed by the driver (verified: no `OnboardingVerification.cs*` under `Assets/ForestPrototype/` in the lane; no `licenses` directory under `run-02/config`).
- Unity rewrote `.vscode/settings.json` and three `Woodland_Forbs_01` materials; these are Editor side-effects, **not** part of the commit. The patch was preserved as `unity-editor-side-effects.patch` and the four paths restored by exact path; the lane is clean.
- `Docs/Project/coordination/active-tasks.md` was not edited (Manager is sole writer); #243 status should move to HANDED-OFF by the Manager.
