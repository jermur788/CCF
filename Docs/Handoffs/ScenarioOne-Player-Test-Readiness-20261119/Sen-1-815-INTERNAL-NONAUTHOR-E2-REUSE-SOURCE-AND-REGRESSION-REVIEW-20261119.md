# Sen-1 #815 — Internal nonauthor E2 reuse source and regression review

> **Parent note (19 Nov 2026).** This file preserves the independent reviewer's first-round report verbatim below. It was produced by Claude Opus 5.5 on a different provider, read-only, against source `83eb87a` / docs `052905a9`. The candidate advanced to `b452e1a9` while the review was in progress; the author has since addressed the findings and a separate delta recheck is appended at the end. A reviewer verdict is not owner acceptance, not a Manager #731 spec approval and not a Unity qualification.

**Reviewer:** Claude Opus 5.5 (nonauthor, different provider from the DeepSeek parent). Read-only. No Unity Editor run, no save written, nothing pushed, no candidate file edited.
**Reviewed (frozen):** source `83eb87a183bc19680724a49b31833c6d61f8bb34` against base `159f0ff8`; docs `052905a943a36bf6557dded7dcd03b544a337fd3`. Packet: `docs/sen-1-731-chas-gpt6-manager-20261118-v2` …`-IMPLEMENTATION-PACKET-20261119.md`. Scene evidence read from `origin/sen-1/813-compact30-e1-visual-layout-fix-20261119-v1` @ `be60c174`.

## Verdict: **NEEDS CHANGES**

| Critical | Major | Minor | Notes |
|---|---|---|---|
| 0 | 6 | 8 | 6 |

**Candidate status: PARTIAL.** A–C are mostly built as source and compile-clean, but there is real five-rack leakage, the shared-file quote change is ungated, the refusal path falls back to a live non-Compact world, and the packet's Manager approval gate was not met. There is no runtime evidence.

## Provenance warning (read first)

The candidate was **not frozen** while I reviewed it:
- The core worktree had uncommitted edits in 5 files on top of `83eb87a`. These add a Compact guard to `TryDesignateExactPlanting`, Pending re-designation, and HUD rack-prompt suppression.
- `origin/feature/sen-1-815-…-v1` is now at `b1554578`, three commits past the frozen HEAD (`01b58a3b`, `38910aca`, `b1554578`).
- The docs worktree is at `9321e6f9` (one more commit) and has two untracked result files.

This review covers **83eb87a / 052905a9 only**. I exported HEAD with `git archive` and reviewed that copy. The later commits touch some of the findings below (they hide the HUD objective widget and the Objectives buttons). They are **unreviewed** and need a delta recheck. As far as I can see in that diff, `RackTool` (B), the J-key rack decision and the quote-freeze gating are still not addressed.

## Findings

### Major

**M-1 — Five-rack teaching and rack tooling still leak into Compact (packet §2 "EXCLUDE COMPLETELY").**
- The always-visible HUD objective widget is fed from the five-rack sequence: `UI/WalkingHudView.cs:182` → `LearningObjectivesView.Sequence.cs:236-265`. In Compact `CurrentTopicIndex` stays 0 because no racks exist, so the widget permanently shows "create access racks / [B]". Clicking it opens the five-rack Objectives screen (`WalkingHudView.cs:49`).
- The rack-stage prompt and notes are not gated: `WalkingHudView.cs:253`, `:334-336`.
- `RackTool` is still added to the player (`UI/ScenarioOneUiRoot.cs:107-111`). Its `canUse` (`Racks/RackTool.cs:80`) has no Compact check, so B → rack → rack fell orders still works in Compact. `WorkPlanView.BuildRacks` then renders them.
- With a tree inspected, J also calls `learning.DecideRackInspection` (`ScenarioOneUiRoot.cs:212-213`) while `Compact30Session` uses J for the assessment.
- The Stand Map still shows `ui.Learning.Summary` (`StandMapView.cs:815`) and the rack-planning route.
- The G0 record's "No five-rack teaching leakage" claim is therefore **false at HEAD**.
- *Smallest fix:* in Compact, don't add `RackTool` (or make `canUse` false); gate J/K `DecideRackInspection`, the objective widget, the Objectives screen and Stand Map rack planning/summary on `!Compact30Session.IsActive`. Add harness negatives: B and M cannot create a `RackGroup`, and the HUD contains no rack text.

**M-2 — The "approved quote freeze" is an ungated shared behaviour change, and it doesn't freeze the charge.**
- `ScenarioOneManager.cs:2296-2300` skips re-quoting every Approved planting order in **every** world (Legacy40, Enlarged80, old saves). It is not tied to `ApprovedWorkOrdersImmutable`.
- Old saves that load with Approved planting orders now keep their stored `estimatedCostCents` instead of being re-quoted. The approval and advance cash gates (`:1262`, `:1303`, `CanAdvanceYearNow` via `CalculateTotals :2373`) then use that stale value.
- Meanwhile resolution re-quotes and charges the live price (`:1328`, `ResolvePlanting :1902`, `cashCents += work.LedgerCashDelta :1946`). The displayed "frozen" Compact quote is therefore not what gets charged.
- There is no regression evidence for this change.
- *Smallest fix:* gate it with `if (ApprovedWorkOrdersImmutable && order.status == Approved)`. In Compact, either charge the frozen quote or fail the order with an explicit message when the live quote differs.

**M-3 — The roster refusal leaves a live, half-bound non-Compact world (packet A: "refuse rather than fallback").**
- `Compact30Bootstrap.cs:85-90` returns *after* `state` was activated and `InitializeNewScenario` ran, and without adding `Compact30Session`.
- `Compact30Session.IsActive` stays false, so every Compact bypass is off: five-rack UI and intro, free G/F/U planting and felling, all on a treeless 30 m grid.
- If an authored stem is missing, `:145-151` silently spawns a placeholder (repair, not refusal).
- The digest check that guards this path formats `float` with `ToString("0.000")` (`Compact30SiteRules.cs:232-237`). It was only verified on .NET 8, not Unity Mono.
- *Smallest fix:* load and validate the roster and sites **before** creating `state`. On any refusal, never activate the management components and show a refusal overlay. A missing authored stem should be a refusal, not a placeholder.

**M-4 — The packet's G0 approval gate was not met before source edits.**
- The packet requires a Manager #731 APPROVED/IMPLEMENTING subset plus file-specific grants before production C# edits: §1 l.17, §4 l.61, l.69, and §5 G0 l.73 ("No source editing without both").
- The parent recorded its own subset and granted itself the 10 shared files (`…ORCHESTRATOR-SPEC-SUBSET-AND-SOURCE-GRANT-RECORD…` §1 and §4), explicitly "NOT A MANAGER SIGNATURE".
- That record is honest, but it is not the required authority. This review is not a substitute.
- *Fix:* #731 ratifies or rejects the recorded subset and the per-file grants before any further integration step.

**M-5 — Tier-1 evidence is far below the packet, and there is no regression evidence for 10 shared files.**
- The 36 offline checks cover only the pure roster and site rules.
- No runtime test ran. There is no mutation demonstration (packet §5 l.75).
- No Legacy40, Enlarged80 or Reference Future suite ran, even though `ScenarioOneManager`, `ForestTree.Awake`, `ForestPlayer`, the UI views and the quote path changed.
- G0 §3 lists "Regression witness" suites that were never executed.
- The disposable Editor harness has no negatives for:
  - cancel → replace;
  - Failed → replace;
  - multi-year covering year (2+2 across years);
  - Completed-slot refusal;
  - CT-3 called twice in one frame;
  - free placement;
  - rack leakage;
  - outside-H1 arrival;
  - any old-world behaviour beyond two constants.
- The README is honest (NOT RUN, HELD), so this is missing evidence, not a false PASS.
- *Fix:* add those negatives to the harness. Under a lawful lease, run it together with the historical suites listed in packet §1.

**M-6 — Approved-order immutability is service-level for the bulk setters only.**
- `SetPlantingExecution` and `SetPlantingShelters` (`ScenarioOneManager.cs:890-913`) refuse only while an *Approved* planting order exists. With only Pending orders they still overwrite every per-site method and shelter choice. The Compact UI hides the buttons, but the API doesn't refuse.
- Together with M-2, the packet's "all Approved editing APIs (including bulk) refuse … changed work gets new Pending ID, quote and reapproval" is only partly met.
- *Fix:* in Compact, refuse the bulk setters unconditionally.

### Minor

- **m-1 CT-3 is evaluated against stale state.** `CanAdvanceYear` and `OnAnnualReviewAcknowledged` read the group that is reconciled only in `Compact30Session.Update` (`Compact30Session.cs:144`, `:305-324`). Two `AdvanceYear()` calls in the same frame after the final 2+2 resolves bypass the hold. *Fix:* call `ReconcileGroup()` at the top of both. Normal UI play is not affected, because clicks are separated by frames.
- **m-2 Free exact placement is not refused at service level at HEAD.** `TryDesignateExactPlanting` (`ScenarioOneManager.cs:105`) has no Compact guard. Only the G key route is closed (`ForestPlayer.cs:263`). The uncommitted worktree edit adds the guard, which confirms the gap.
- **m-3 Felled authored trees are deactivated.** `Compact30Session.cs:326-331` turns the felled tree into an inactive `GameObject`, so the stump drops out of every `FindObjectsByType(Exclude)` consumer (map, inspection, history). The primitive stump has no identity. *Fix:* disable the authored renderers and keep the `ForestTree` active.
- **m-4 Bootstrap depends on undocumented Awake order.** `Compact30Bootstrap.cs:69-75` assumes ecology `Awake` runs before manager `Awake` on activation. If not, `StandWorldBoundary` is added at Enlarged80 size and never removed. The manager `Awake` also briefly builds an Enlarged80 grid and a V2 rack plan, which the second `InitializeNewScenario` then resets. *Fix:* activate ecology first, then add the manager, or explicitly destroy any `StandWorldBoundary`.
- **m-5 No confirmation on cancellation.** "Cancel approval" (`WorkPlanView.cs:611`) has no confirmation step; the packet says "user-confirmed".
- **m-6 Scene provenance mislabelled.** G0 §1 cites `sen-1/813-compact30-e1-scene-20261119-v1 @ be60c174`. That branch does not exist; `be60c174` is on `sen-1/813-compact30-e1-visual-layout-fix-…`. The runtime also binds to the E1 lineage that is still uncorrected (JSON comma) and not owner-released (packet l.21, l.67). Only the sites JSON avoids the malformed bytes, and that is done correctly.
- **m-7 Felling uses the unadapted harvest economy.** Compact felling uses the five-rack `GetHarvestQuote` / harvesting-system economy unchanged. No no-rack eligibility review was done (packet B, last sentence). NOT VERIFIED.
- **m-8 Spawner runtime species skip readiness reporting.** `ForestTreeSpawner.ConfigureForRuntime` adds species after `Awake`, so `ReportUnusableSpecies` never runs for them (`ForestTreeSpawner.cs:69-77`). Harmless today because the assets are byte-identical copies, but there is no readiness check.

### Notes

- **n-1** The four G1 coordinates are labelled E1-provisional: `Compact30Sites.json` has status `E2_CANDIDATE_FIXTURE_E1_PROVISIONAL_NOT_SIGNED_PRODUCTION_GEOMETRY`, and the spec record §3 and §5 says the same. Confirmed.
- **n-2** The roster digest reproduces independently: `dac06a3b…cca1`, 9,481 bytes, 176 lines. The 175 roster ids exactly equal the 175 `P####_…` prefab names in the scene, all direct children of `Compact30World/Trees`. Name matching will work.
- **n-3** The Resources species and definition copies are byte-identical to `Species/*.asset` and `ScenarioOne.asset` below the header. All 11 new `.meta` GUIDs are unique. IDs and defaults are unchanged.
- **n-4** The player spawns at (2, 0.1, −8), inside H1. Arrival therefore needs no walk, which is consistent with "Start in H1". The harness `AcknowledgeArrival` passes only because of that spawn point.
- **n-5** The 175 authored Sitka never change shape as they grow (`visuallessAuthored`), so annual consequences are visible only in data and juveniles. This weakens D.
- **n-6** `ScenarioOneCompact30` is not in `EditorBuildSettings`, so the candidate is Editor-only. Acceptable for E2.

## Required explicit statements

- **Accidental rack/MFM leakage:** PRESENT at HEAD (M-1): HUD objective widget, rack prompt, `RackTool` B, J-key rack decision, Stand Map rack summary and planning. I found no MFM sample entry: the prescription flow is not reachable because the teaching index stays 0, but this was not exhaustively traced. Two-cycle completion is correctly suppressed (`ScenarioOneManager.cs:2438`). The V2 rack model is reset to Legacy by the second `InitializeNewScenario`.
- **Save/old-world negative evidence:** static only.
  - `StandGeometryModel.IsKnown` is still 0/1. `ForestSaveValidation.Geometry.cs:18` and `ForestSaveController.cs:302` are unchanged. No #690 file was touched.
  - The Compact scene has no `ForestSaveController`, and `StandaloneSessionMenu` refuses to open without one (`StandaloneSessionMenu.cs:83`), so there is no save route.
  - Old-world behaviour **is** changed by M-2. None of the old suites were run.
- **Test weakening:** none found. No existing verification file was modified. New tests only add checks.
- **Compile/scene provenance:**
  - Reproduced by me: offline 36/36 PASS, and `CloudTypeCheck --mode prod` `TYPECHECK_CLEAN` (6 baseline errors, 0 new).
  - The Unity 6000.6 assembly compile claim was **not reproduced** by me.
  - Scene provenance: see m-6.
  - Bind order, `Resources` loading, tree matching and colliders look correct by reading, except M-3 and m-4. **NOT VERIFIED at runtime.**
- **Unsupported claims:**
  - G0 "No five-rack teaching leakage" (false, M-1).
  - "freeze approved work at the service and UI routes" (partial, M-2/M-6).
  - "no free placement" (UI-only at HEAD, m-2).
  - G0 "Regression witness" column (none executed, M-5).
  - Self-recorded "IMPLEMENTING subset" standing in for the Manager gate (M-4).
  - Editor NOT RUN and SAVE INTEGRATION HELD are stated honestly. No PASS overclaim on Unity, save, completion or playability was found.

## A–D against the packet

- **A (enter/assess/select):** implemented as source. The bind, roster digest, Y-in-H1 arrival, J-then-Y assessment and non-gating 3-Sitka Crop Tree count are present. Gaps: M-1 leakage, M-3 fallback.
- **B (stock/designation/Work Plan):** implemented as source. Existing nursery, four fixed sites with full clearance rules, refusal before both acknowledgements, distinct Pending orders with per-site method and shelter, no early juvenile, normal approval. Gaps: M-2, M-6, m-2, m-5, m-7.
- **C (annual/T1):** implemented as source. Real resolution, order → juvenile linkage by site/species/year, Y* = latest successful `resolvedYear`, the single existing ack bound through an event, CT-3 in both `AdvanceYear` and `CanAdvanceYearNow`, and no credit from Pending/Approved/Failed/cancelled. Gaps: m-1; in-memory only (HELD, by design); zero runtime evidence.
- **D (advisory):** thin but present. A three-line intervention history and a guarded N no-work decision; no M-A/OD-16 award. Gaps: n-5, m-7, a minimal history surface.
- **Enough for an E2 source candidate?** A–C will be, once M-1, M-2, M-3 and M-6 are fixed. D is acceptable as advisory.

## Status and single next gate

**PARTIAL.**

**Next gate:** Manager #731 ratifies or rejects the self-recorded A–C+D subset and the 10 shared-file grants (M-4). If ratified, the author fixes M-1, M-2, M-3, M-5 (harness negatives) and M-6 on a new frozen HEAD that includes or supersedes `b1554578`, for this reviewer's delta recheck. Only after that does the lawful-lease Unity Tier-2 run make sense.
