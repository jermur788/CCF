# Sen-1 #890 — existing three-page onboarding reused, CODE ONLY; parallel art safe

**Date:** 2026-11-20. **Owner instruction:** implement in parallel while another agent works on Unity assets, **without running Unity**.  
**Classification:** PACKET ONLY; smallest existing feature hookup.  
**Result:** SOURCE IMPLEMENTED / GITHUB READBACK CHECKED / UNITY NOT RUN / NOT INTEGRATED / NOT OWNER-ACCEPTED.  
**Implementation provider:** ChatGPT GPT-6, browser GitHub connector, actor `jermur788`; internal effort/session UNKNOWN.  
**Issue:** https://github.com/jermur788/CCF/issues/890. **Original binding packet:** https://github.com/jermur788/CCF/blob/docs/sen1-890-reuse-onboarding-packet-20261120-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-890-REUSE-THREE-PAGE-ONBOARDING-COMPACT30-REPAIR-PACKET-20261120.md.

## Exact source identity

- Parent: #886 repaired Compact30 source `81e65590fe15832cfc95c74ef0f02fcfb91d10d1`.
- Branch: `fix/sen1-890-compact30-onboarding-codeonly-20261120-v1`.
- Production commit: `bb1c5cec89c9d08a33009858394698484b7a24d5`.
- **Sole changed source file:** `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`.
- Sole change in `Start()` ~line 148: removed the `!Compact30Session.IsActive` exclusion from the existing **fresh, year-zero, empty-work/empty-review and no StandaloneSessionMenu** startup condition. The subsequent call to `help.ShowScenarioIntroduction()` remains unchanged.

Old condition:
```csharp
if (FindFirstObjectByType<StandaloneSessionMenu>() == null && !Compact30Session.IsActive && manager != null && manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0 && manager.AnnualReports.Count == 0)
```

New condition:
```csharp
if (FindFirstObjectByType<StandaloneSessionMenu>() == null && manager != null && manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0 && manager.AnnualReports.Count == 0)
```

**Rationale:** Existing `ScenarioOneOnboardingView`, `ScenarioOneOnboardingCopy`, `MenuHelpView.ShowScenarioIntroduction()` and all `Resources/Onboarding/page-{1,2,3}.png` already exist and were inspected at the parent commit. The previous condition excluded direct-entry Compact. Existing standalone `StartNewScenario()` explicitly calls onboarding; keeping the no-StandaloneSessionMenu guard avoids calling it twice. No new text/art/navigation/visual system was added. The older onboarding spec `specs/scenario-one-three-page-onboarding-v1/` remains the same; no status was promoted.

## Readback and validation actually performed

- Fetched changed C# blob back from GitHub: `ae7ab19d2f13e4b7a645b70260dec83dd8ad5826`, matches intended guard and still contains `help.ShowScenarioIntroduction()`.
- GitHub compare **base #886 → source branch after implementation**: ahead 1 / behind 0, exactly one changed file, one line added / one line deleted. No compiled source outside this file changed.
- **NOT PERFORMED:** local filesystem/project checkout, `dotnet` compilation, Unity 6000.6.0f1 import, test harness, live UI/rendered pages, physical ops lease, owner-test or historical save qualification. Github static readback proves repository source, not runtime behavior. No save, UserPrefs or physical file was touched by this session.

## Parallel art-source boundaries

The other planned work is #891, Opus 5.5 WG Mossy Forest Floor visual pilot (no native registration at last GitHub board read). Its intended files are ground-material/art/pilot scene, not this UI C# file. The #890 source branch is **not** a branch for the art worker to modify and creates **no Unity Editor contention**. #891 must use its own legally admitted art lane/branch and retain exclusive Unity lock when applicable. Branches may both descend from #886 and be integrated **later by one authorised integrator** after the art worker releases its lane/Unity lease. Do not edit source UI from #891 or combine while source/asset ownership is unclear. If #891 or another native worker touches `ScenarioOneUiRoot.cs`, STOP and reconcile the exact diff before any combine.

For later source integration use **the source commit only** (`bb1c5cec...`); it contains the one-line UI change and no art. Do not cherry-pick an entire unrelated donor history, overwrite a scene, or automatically merge to main. Verify the final combined candidate's exact source/asset identities and compile-linked scope.

## Exact downstream checks (short, not full T3)

After lawful single-machine Unity admission in a **separate integration/qualification step**, inspect a new Compact30 Editor launch from `CCF → Player Test → Play Compact30`:
1. Existing Page 1 ("What is Continuous-Cover Forestry?"), Page 2 ("How CCF works over time"), Page 3 ("Scenario One — Transforming a Sitka Plantation") appear in sequence, use existing illustrations and Back/Next/Begin, and block movement/marking/Tab until Begin.
2. Begin returns to the actual same #886 Compact30 forest, with H1/walking, C/X, no-rack rules and tree physics unchanged; screens do not double-open.
3. Existing standalone Start New still shows introduction only once; supported saved-game Continue does not replay it. Compact30 has no save/load implementation in this interim source, so do not invent one.
4. If combined with the #891 art surface, inspect at least one short walk/path interaction and that the onboarding does not obscure the owner visual test. Preserve the existing minimal #891 qualification requirements and owner's test-before-review priority.

Protect Legacy40/Enlarged80, exact saves, frozen Reference Future v1 and determinism; source-only change gives no evidence those tests passed. No Windows build or main merge without later explicit gates.

## Disposition and continuation

**Code change ready for later integration/Unity verification; not a playable compiled release on its own.** #425 Overall Manager or actual authorised delivery manager should coordinate one integrator, not start a second Unity Editor while #891 art worker holds it. Owner may continue art evaluation in parallel; **no further owner decision is needed to keep #890 source code isolated**. Do not call #890 verified/accepted until actual combined Unity check and owner feedback.

**Direct handoff:** https://github.com/jermur788/CCF/blob/fix/sen1-890-compact30-onboarding-codeonly-20261120-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-890-CODE-ONLY-ONBOARDING-REUSE-IMPLEMENTATION-AND-PARALLEL-ART-HANDOFF-20261120.md
