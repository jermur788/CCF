# Scenario One minimal legitimate completion-path audit v1

## 1. Executive conclusion

**Classification: C — MINIMAL PATH MATERIALLY UNDERCUTS D-060.** Audit only; no objective, outcome, year, ecology, economy, save, scene, UI or default-policy change.

Implementation: `claude/stoic-mccarthy-gtzaub` at `b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a`. Canonical decision context: `321791b8203bc709c57615fd4d6a19614d78db24`; D-060 calls for learning through repeated intervention and adaptation by completion. Those docs describe older integrated implementation; inspected b4894cc source is authoritative for this branch's capabilities.

**Established minima on the default seeded new game: year 25, one resolved annual Work Plan, one successfully felled tree, two successful exact-position plantings (one Oak and one Beech), and one review acknowledgement.** The player can advance 24 years without management, acknowledge the first idle-year review once, then resolve all three tasks together in year 25. Both broadleaves remain juveniles with no adult broadleaf trees or broadleaf regeneration cells at completion. There is no post-intervention revisit/adaptation requirement before the Completed verdict.

The same run demonstrated a second intervention at year 26 and a third at year 29, including carrying the third order pending for two years and resolving it after a stale warning. Normal disk save/load passed at years 10, 25 and 29 with unchanged world hashes, event counts, derived intervention history and outcome/year. 48 executable checks passed. The history honestly reports one intervention at completion, but still presents success. That transparency exposes the shallow path; it does not by itself naturally teach repeated adaptation by completion.

Environment: Unity 6000.6.0f1, Linux Editor batch/nographics, unchanged default new Scenario One (Enlarged80, RNG 1, regeneration 2, growth 1, storm 0). This is an executable public-API/UI-handler witness, not human comprehension, mouse/keyboard, rendered forest inspection, native Windows or Player qualification.

## 2. Eight objective definitions

Exact predicate transcription from `ScenarioOneObjectives.Evaluate`: `Add` sets `achieved = current >= target`. Geometry-aware extensive targets are 4× their serialized Legacy40 values. In the following table `S` is the latest ecological snapshot and `original` is `Species(S, originalSpeciesId)`.

| ID | Exact current-value predicate | Target, default Enlarged80 |
|---|---|---:|
| `minimum-year` | `S.year` | `definition.MinimumCompletionYear` = 25 |
| `retained-canopy` | `original != null ? original.livingTrees : 0` | `EffectiveMinimumRetainedOriginalTrees(geometry)` = 240 |
| `continuous-canopy` | `S.meanCanopy` | `MinimumMeanCanopy` = 0.35 |
| `regeneration` | `S.occupiedRegenerationCells` | `EffectiveMinimumRegenerationCells(geometry)` = 12 |
| `fallen-deadwood` | `S.deadwoodVolumeM3` | `EffectiveMinimumDeadwoodVolumeM3(geometry)` = 0.08 m³ |
| `managed-opening` | `felled ? 1 : 0` | 1 |
| `introduced-beech` | `planted && present > 0 ? 1 : 0` | 1 |
| `introduced-sessile-oak` | `planted && present > 0 ? 1 : 0` | 1 |

`felled` means an event with `eventType == WorkResolved && outcome == Succeeded && taskType == FellTree`, OR a work order with `type == FellTree && status == Completed`.

For each distinct nonempty species ID in `definition.ShopEntries`, `planted` means an event with `eventType == WorkResolved && outcome == Succeeded && taskType == PlantJuvenile && speciesId == speciesId`, OR a work order with `type == PlantJuvenile && status == Completed && speciesId == speciesId`. `present` is `species.livingTrees + species.regenerationCells + species.plantedJuveniles`, or zero without a species census. Presence is not an adult-tree, age, basal-area, recruitment-success or planted-lineage-only requirement. A previously planted species can qualify through natural presence after the individual planting dies.

`EvaluateProgress` calls private `SetOutcome(Completed, S.year, ...)` only while outcome is Active and all evaluated objectives are achieved. The audit does not invoke that private method. Retained-canopy counts all current living members of the original species, not exclusively individually tracked original stems; the label is broader than an original-ID survival predicate.

## 3. Dependency graph

| Objective | Earliest condition can hold | Prerequisite / normal action | Resolved work? | Consequence / repetition requirement |
|---|---|---|---|---|
| Minimum year | 25 | 25 annual advances from year 0 | No | Time only; no second intervention |
| Retained canopy | 0 | Sufficient living Sitka at latest census | No | Can be inherited/maintained without new work |
| Continuous canopy | 0 | Mean canopy ≥ 0.35 | No | Current ecological condition; not a before/after improvement |
| Regeneration | Potentially baseline; otherwise an annual census | ≥12 occupied cells of any species | No | Natural cohorts count; no broadleaf-recruitment quota |
| Fallen deadwood | After first retained fell or natural mortality | ≥0.08 m³ remaining at census | Not necessarily | Retaining one felled stem can share managed-opening action; natural deaths also count |
| Managed opening | First resolution, year 1 onward | Mark a living tree, import, approve, advance | **Yes** | One successful fell; no minimum opening area/stand-response/repeated plan |
| Introduced beech | First planting resolution, year 1 onward | Buy stock, choose legal ground point, approve, advance; species remains present | **Yes** | One exact juvenile suffices; no later observation period |
| Introduced sessile oak | Same | Same for Oak | **Yes** | Same |

```text
new forest ── annual advances ──────────────── year >= 25
     ├── retained Sitka + mean canopy ────────┐
     ├── natural occupied regeneration ───────┤
     └── natural deaths / retained fell ──────┤
mark/import one fell ─┐                       ├─ all eight -> automatic completion
buy/place one Oak ───┼─ approve -> resolve ───┤
buy/place one Beech ─┘    + annual ecology ───┘
                 first annual review acknowledgement gates next UI advance
                 second intervention has no edge into completion
```

Save/load persists the state/event/order/census chain; it creates no objective prerequisite. Pruning, crop-tree marks, salvage, regeneration removal and a particular sale outcome have no independent completion objective. Exact planting applies its normal local clearance automatically; this is not a separately commissioned clearance requirement.

## 4. Minimal legitimate completion path

1. Enter the normally generated year-0 forest; do not reset, load a fixture or modify any model settings. Baseline: 1,344 Sitka, canopy 0.9873368, no regeneration or deadwood.
2. Advance using the Work Plan UI handler to year 1 without work. The next UI advance is blocked until the Annual Review is acknowledged. Acknowledge once through the actual review handler.
3. Advance normally to year 24 with no intervention. At year 10 perform an isolated ordinary disk Save/CanLoad/Load roundtrip, then continue. This roundtrip is verification, not a gameplay prerequisite.
4. At year 24 mark the first living P-prefixed tree in stable ID order for felling, import the mark into the Work Plan and choose RetainAsFallenDeadwood. Pending work does not satisfy managed-opening.
5. Buy exactly one `sessile-oak-sapling` (€5.50) and one `beech-sapling` (€4.50). Choose legally clear ground positions, landowner-simulated planting and a shelter each (€5 material per shelter). Pending planting markers do not satisfy either introduction objective.
6. Approve the single combined plan. Approval alone still satisfies none of its three work-dependent objectives.
7. Advance once through the UI handler. Work resolves, the normal annual ecology/juvenile updates run, and the latest census causes automatic Completed at year 25.

| Objective at completion | Recorded current / target |
|---|---:|
| Minimum year | 25 / 25 |
| Retained original-species canopy | 938 / 240 |
| Mean canopy | 0.902876139 / 0.35 |
| Occupied regeneration cells | 231 / 12 |
| Fallen deadwood volume | 140.733185 / 0.08 m³ |
| Managed opening | 1 / 1 |
| Introduced Beech | 1 / 1 |
| Introduced Sessile Oak | 1 / 1 |

At year 24, before any work, four environmental objectives already hold: 983 living Sitka, canopy 0.9113246, 229 regeneration cells, 121.920418 m³ deadwood. In this trajectory natural regeneration first meets its threshold at year 1 (23 cells); natural deadwood first meets its threshold at year 8 (0.676347 m³). These are observations for this run, not new timing rules.

All planning and state changes use existing normal public gameplay actions: marking and import, stock purchase, exact planting, approval, `ScenarioOneUiRoot.AdvanceFromWorkPlan`, `AcknowledgeAnnualReview`, and disk `Save`/`CanLoad`/`Load`. No private invocation, fabricated save, direct ecological mutation, forced completion, debug API, policy/model override or test-only reset.

The harness chooses planting sites by reading the same stand/cell/stem state available to inspection and tries normal validation; it does not teleport the player or edit cell state. Finding a site programmatically demonstrates legal gameplay state reachability, not how easily a novice can discover that site. Annual UI screens are opened normally; no walking or comprehension is claimed.

## 5. Earliest completion year

**25 is both a proven lower bound and an attained earliest completion year.** The predicate requires `snapshot.year >= 25`; normal annual advance increments by one from 0. The witness meets all remaining predicates in the year-25 census. Exactly 25 annual advances are needed, even though 24 of them contain no work.

Year 30 is not a completion rule. Year 100 is the separate Century Review/management horizon and failure boundary for a still-active incomplete scenario. Neither the audit's scheduling nor older harness convenience timings add a gameplay requirement.

## 6. Minimum intervention count

**One plan, one fell, two planting events and two purchased saplings are attained lower bounds.** Zero successful fell cannot satisfy managed-opening. Each distinct shop species needs successful planting evidence, and one work order has one species: at least two successful planting tasks are necessary. An exact-position task reserves/uses one sapling. All three tasks can resolve in the same annual plan, so a second plan is not needed. No pruning, explicit crop-tree marking, salvage, separately scheduled regeneration removal or further planting was needed.

**One acknowledgement is the attained UI-path lower bound.** A year-25 new-game path necessarily passes the first annual report and tries to advance again. The normal UI blocks that next advance until acknowledgement. The flag persists thereafter. Twenty-five review screens open automatically, but only one acknowledgement is enforced; those automatic openings must not be described as 25 meaningful reviews. No acknowledgement of the year-25 completion review is needed to attain Completed.

A resolved annual Work Plan is counted here as one annual group of successful resolved work, matching the actual derived intervention history. Reopening, editing or approving the same plan is not an additional intervention. Work resolution precedes exactly one ecological annual step; planted-juvenile advancement, understorey and deadwood updates, then ecological snapshot and completion evaluation follow it. Completion therefore cannot precede that annual ecology step, but does not require a later annual step after planting.

## 7. Repeated-management capability

After the year-25 completion/save-load check, one further living tree is marked for retained-deadwood felling, approved, and resolved at year 26. History has two interventions and cash is €6,980. A third tree is marked in year 26, left pending while advancing through years 27 and 28, and displayed as stale with age ≥2 when Work Plan is opened. The order remains open rather than deleted. Approval and advance resolve it in year 29; history has three interventions, cash €4,480, and no source changes. The year-29 normal disk roundtrip preserves this history and the original Completed/year-25 outcome.

`CanManage()` accepts Completed while `CurrentYear < ReviewYear`; it rejects Failed and management at/after the century horizon. Supporting repetition and requiring repetition are distinct. Later probes are explicitly outside the minimal completion witness and cannot be counted towards its objectives or its at-completion experience.

## 8. Shallow-path attempts

| Attempt | Evidence / result |
|---|---|
| No management for 24 years | Allowed through normal UI after one acknowledgement; no successful intervention history |
| One token felling | One stem is sufficient; only one successful fell recorded at completion |
| Smallest broadleaf stock | One exact juvenile per species suffices; no adult broadleaf or recruited broadleaf cohort needed |
| Marking/planning instead of work | Both introduced predicates and managed-opening stay 0 while Pending |
| Approval instead of resolution | All three work predicates remain 0 before annual resolution |
| All required work in one year | One combined plan resolves all three tasks in year 25 and completes immediately |
| No later establishment/response period | Both juveniles survive the normal initial annual step; no extra year after that step required |
| No meaningful regeneration caused by thinning | Before the first fell, 229 natural Sitka regeneration cells already qualify; the criterion is not causal to that fell |
| No second intervention before completion | Proven by history containing exactly one year-25 intervention |

This is a shallow legal route, not a save/debug exploit or a claim that natural growth never changes the forest. Retaining the token fell adds deadwood, but natural mortality had already exceeded the deadwood threshold. Alternative sell/extract and unsheltered variants were not run and are not needed to prove the attained minima.

The first acknowledgement is a UI learning gate only. Direct `manager.AdvanceYear()` could bypass it in a harness; this audit deliberately uses the UI advance handler. Acknowledging at year 1 clears the persistent flag for all later years, even if the first actual work takes place much later. Showing a review is automatic after each UI advance; reading/understanding it is not measured or required repeatedly.

## 9. Completion-summary result

The actual completion wording begins “The Scenario One objectives were met in year 25” and “You carried out 1 intervention,” followed by year 25: 1 felled, 2 planted (1 Beech, 1 Sessile Oak, both sheltered). It shows:

- Living adult trees: 1,344 → 938, all 938 remaining adults Sitka.
- Basal area: 40.5 → 77.2 m²/ha; canopy 0.99 → 0.90.
- Regeneration cells: 0 → 231; fallen deadwood: 0 → 140.7 m³.
- Cash: €12,000 → €9,480; timber income €0; displayed work/material cost €2,510; minimum cash €9,480.
- 425 non-felling biological deaths recorded. Storm model is 0; generic summary wording lists possible causes rather than proving a storm.

The underlying species census additionally shows one living planted juvenile of each broadleaf species, no adult broadleaf and no broadleaf regeneration cell. The displayed “Living trees at completion” line lists adults only. Summary cost plus €10 nursery stock purchases reconciles to €2,520 total cash expenditure. Its at-completion intervention has before year 24 and after year 25, with no later observation year available at that point.

History is derived from actual events, annual reports, ecological snapshots, work orders and deadwood. A single plan is visibly a single intervention, not padded by idle annual advances. Counts are descriptive; the completion verdict is unchanged by a shallow history. `NextStepHint` after completion offers continued management toward Century Review, rather than requiring another adaptation before the completed verdict.

Financial caveat: `CompletionSummary.TotalCostCents` sums annual contractor costs and successful resolved events' `stockCostCents`. The latter stores planting material/shelter charges; nursery purchase costs are separately recorded on StockPurchased events. Consequently summary cost is not necessarily the full starting-to-closing cash expenditure. Report actual closing cash separately; this audit does not correct presentation.

## 10. Save/load, failure and stale orders

**Executed PASS:** ordinary disk Save, read-only CanLoad validation and Load at year 10, at Completed/year 25, and after the third intervention/year 29. Each load preserves the existing world hash, history fingerprint, number of management events, current year, outcome and outcome year. No duplicate intervention or completion event was introduced. Save version remains 20; the original user `forest-save.json`, backup and temporary file existence/bytes are independently unchanged. Audit saves live under ignored task-local `Build/MinimalCompletionAudit/late/config`.

**Executed stale-order PASS:** the third felling order is left pending for two years, remains open and is surfaced as stale, then normally approves/resolves. Beyond that interval, retention/no-expiry and terminal failure are code-inspection findings, not an executed century-long pending-order or failure run.

Source inspection: `Failed` is terminal for this scenario instance: normal planning/advance reject it, and `EvaluateProgress` only transitions while Active. A normal reset/new scenario is a separate session. Failure occurs on an Active incomplete scenario with zero cash, positive contractor rate and no Approved order, or on reaching the review horizon incomplete. This audit does not force failure or deliberately exhaust the budget.

Pending orders have no automatic age-expiry/delete policy. Age ≥2 years is surfaced by `StaleOrders`; invalid targets/validation messages are also surfaced. Target mortality, changed affordability or other validation can still stop a particular order resolving, so "indefinitely retained" does not mean indefinitely valid/executable. Stale age itself is not a completion predicate or a mandatory rejection. Reopening Work Plan revalidates normally. A stale pending order may be left open while other work resolves; only successful felling/planting evidence counts.

## 11. Intended experience against D-060

| Experiential step | Completion requirement / minimal-path exposure |
|---|---|
| Inspect and assess stand | Useful for choice; no objective requires a recorded inspection or sound assessment |
| Choose crop trees | Optional; no crop-tree/pruning objective |
| Thin/remove | One resolved felling is required; scale/quality not judged |
| Approve work | Required for normal plan resolution |
| Advance | Required to reach ≥25 and resolve work |
| Review consequences | One acknowledgement before the second UI advance; later comprehension/reading optional |
| Introduce broadleaves | Both species must have successful planting history and current presence; one juvenile each can suffice |
| Revisit changed stand | Changed state exists and review opens; walking/inspection not enforced |
| Make another meaningful decision | Supported, suggested by history, but not a completion dependency |

The world is materially changed by natural annual simulation before completion: mortality, growth, canopy and regeneration changes are measurable. However, a player can attain completion without inspecting that response, selecting crop trees, revisiting after an earlier intervention, or adapting a second intervention. The only actual management occurs during the completing annual advance; no post-intervention observation interval is required. Thus a changed forest exists, but the rules do not ensure the player experiences the intended repeated-management loop before success. This specific gap warrants C under the packet's D-060 wording. History communicates shallowness honestly, but neither gates that experiential gap nor withholds the Completed verdict.

## 12. Risks and limits

- One default seeded stand/path is a reachability witness and, when lower bounds are met, proves those minima; it is not a universal prescription or every-seed success guarantee.
- Current branch qualification and species-parity blockers remain separate. A legal path here does not approve the branch or an external demo.
- No Player/native-Windows/UI rendering/performance/novice-comprehension gate was run for this audit.
- No full regression rerun: production bytes remain unchanged and this task is an audit, not integration.
- `UNS_HDRI.hdr` was excluded from the task-local Unity import environment because its 16K × 8K import repeatedly exhausted available memory in earlier qualification. Dependency audit found no CCF reference to GUID `83d5c6c30d68b7e4498cfef8c6e2a35a`. It remains preserved in canonical source/package content and was not deleted. Both ignored licensed source files were restored with SHA-256 unchanged after Editor exit; no missing-reference or null-reference exception occurred in this audit.
- Generated local editor/material drift is restored only under the previously qualified exact normalization rules; all other changes are stop conditions. Original user saves are protected by isolated config and checked independently.

## 13. Recommendation

**Manager decision required: YES.** Review the gap between (a) supported repeated management and (b) completion achieved with no repetition or adaptation. Decide explicitly whether an external demo may award completion on this legal route, or whether an independently authorised design/presentation response is necessary. This audit does not recommend or implement a particular new objective. Do not declare the external demo ready from this result.

Do not silently add a second-intervention objective, change the eight predicates, move completion to year 30/100, or introduce a score. Return this audit and the exact witness to Overall Manager. Any completion-policy change needs an explicit product/design decision; the separate repeat-management support can be reviewed on its own merits.

## Evidence and reproduction

Executed 2026-10-14 in `/media/jer/Files/CCF-verify-overnight-b4894cc`, branch `audit/scenario-one-minimal-completion-v1`, exact source b4894cc. The `late` route passed; the optional `early` scheduling branch of the disposable harness was **not run** and supplies no evidence. Unity process exited 0. Filtered original `MINIMAL_` records (trailing whitespace trimmed only; values/ordering unchanged) are committed alongside this report in `MinimalCompletionLatePath_v1.txt`; they include annual snapshots, objective values, pending/approved states, completion text and all 48 checks. Full local log remains in ignored `Build/MinimalCompletionAudit/late/unity.log` (SHA-256 `aaab423a3ba1671a87b8c1bded2cf4239b278df19de0b0063274a96d4df65454`).

Cleanup: the sole tracked Unity drift was the known `.vscode/settings.json` trailing-comma removal/worktree solution name. After verifying semantic equality apart from that local solution setting, it was restored byte-for-byte to HEAD. No material, scene, prefab, source, settings, package or fixture change remained. The disposable Assets script and `.meta` were removed. Final pre-report status was clean. HDR restoration record is local `Build/S1B/demo-hdr-quarantine-record.json`; original user-save digest receipt is local `Build/MinimalCompletionAudit/late/original-saves.json`. Neither protected main checkout nor other Unity session was modified.

Harness: `Tools/Verification/ScenarioOneCompletion/ScenarioOneMinimalCompletionAudit.cs`. Copy into `Assets/ForestPrototype/`, run `ScenarioOneMinimalCompletionAudit.Begin` in the Editor, remove that copy and its `.meta` after exit. Use an isolated `XDG_CONFIG_HOME` with a Unity license copy so disk Save/Load cannot touch existing user saves. Use this checkout's own Library. The normal command accepts `CCF_MINIMAL_ROUTE=late` (24 idle years then combined plan) or `early` (plan in year 0 then wait); this variable controls audit scheduling only. Do not set any ecology/geometry model override. Run through the approved demo-HDR quarantine wrapper. Never ship the temporary Assets copy in a Player.

Production diff: none. Canonical/context changes: none. Report, filtered original audit evidence and disposable verification tooling only.
