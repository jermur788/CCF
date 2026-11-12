# Scenario One — CCF Management Cadence and Two Advance Options V1

**Status: APPROVED DESIGN / RELEASEABLE FOR BOUNDED IMPLEMENTATION; NOT IMPLEMENTED / VERIFIED.**  
**Updated:** 2026-11-12, Manager #200 following [independent #396 NEEDS CHANGES review](https://github.com/jermur788/CCF/blob/docs/claude-b-cadence-time-advance-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-CADENCE-TIME-ADVANCE-SPEC-REVIEW.md) and accepted Manager product-policy decision D-077. This four-file corrected contract supersedes the earlier DRAFT, **not** the original S1-D accepted save/completion policies.

**Authority:** D-074 periodic CCF management learning, D-075 two advance options, D-077 bounded policy; D-073 management decision/outcome feedback; D-065 MFM V1 and D-076 representative racks. **Goal:** reduce unnecessary one-year clicks **between legitimate later management reviews**, preserving complete annual model truth and user judgment.

## 1. Product contract

The Work Plan offers two **optional**, player-invoked choices when valid:
- **Advance 1 Year** retains its existing behaviour, validation, warning and annual review.
- **Skip to Next Management Year** displays `Skip to Year Y (N years)`, a plain-language reason and an explicit confirmation. It advances **at most five ordinary years** to a suggested future **management reassessment** point or an earlier real stop. The suggestion is **not a felling schedule, a compulsory intervention, a new objective or an automatic treatment**.

The game still runs its authoritative annual forestry update each year. A user can do warranted planting, protection or maintenance between major thinning events by choosing one-year mode; no hard minimum work interval is introduced. Completing the learning sequence currently precedes availability of the multi-year skip; enabling safe time-skipping between *earlier* tutorial objectives is a **separate stage-aware follow-on**, not a reason to bypass immediate player tree/rack decisions.

**First-release boundary:** no change to existing teaching-completion events, `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence`, `ScenarioCycleState`, `ScenarioInterventionHistory` ordinal definitions, objectives/cash/forestry coefficients, event enum, save schema, JSON hash layout, or reference archives. **D-074's user-required *spaced major-intervention objective*** is separately tracked in a new four-part design: this safe navigation feature **does not by itself satisfy that substantive objective redesign**. Do not silently mark that broader learning goal complete.

## 2. Direct source baseline, inspected statically

On the reviewed post-Fable Scenario One code, `ScenarioOneManager.AdvanceYear()` (lines 1264–1387) resolves existing Approved work, advances forestry exactly once, updates juvenile/understorey/deadwood, writes one annual report, a YearAdvanced event and ecological snapshot, evaluates objectives, and sets `annualReviewSeen=false`. `ScenarioOneUiRoot.AdvanceFromWorkPlan()` enforces a UI-only mandatory review gate. `ScenarioOneManager.MarkAnnualReviewSeen()` writes a real `AnnualReviewAcknowledged` event. Those events affect the saved world fingerprint and chronology. Therefore no fake or synthetic review acknowledgment is possible.

`WorkPlanView` creates Pending orders from field marks when the Work Plan opens; MFM prescription approval creates **Pending** work, which a separate Work Plan approval commits. The authoritative annual simulation does not read a past MFM prescription. Current teaching stages 0–6 may require actual player decisions or a genuine review; stage 7 is the conservative first-release boundary. S1-C already has year browser, saved reports and source-provenance labels.

**Evidence:** static code + independent review; no time-skip feature exists yet, no Unity PASS inferred.

## 3. Exact recommended-year calculation — gameplay calibration, not forestry rule

Read-only, deterministic `ScenarioManagementCadence.Compute` uses saved/current authoritative facts, never writes a schedule.

- `ReviewGapYears=4` and `MaxTicksPerSkip=5` are **GAMEPLAY CALIBRATION** inspired by indicative 3–6-year CCF management practice, **not** prescribed/validated Irish intervention intervals. Human labels say “suggested time to reassess”, never “now thin”.
- Find latest year `q` with a unique, valid `WorkResolved` **Succeeded** event linked to a matching **Completed FellTree** order of that resolution year. This includes rack-access felling **solely as a calendar/navigation anchor**; it **does not make access felling a qualifying substantial stand intervention** for the separate D-074 spaced objective. Exclude planting, pruning, protection, clearance and salvage.
- `candidate=(q == null) ? CurrentYear+4 : q+4`. If `candidate <= CurrentYear`, use `CurrentYear+4`; never show a past/overdue target. `target=min(candidate, CurrentYear+5, ReviewYear /*100*/)`.
- If current year is below `MinimumCompletionYear=25` and `target>25`, clamp to Year 25; any objective/outcome transition is an additional runtime stop. `target>CurrentYear` is mandatory.
- Display `Skip to Year {target} ({target-CurrentYear} years)`, showing last felling year when present; if none, say “suggested observation interval”. State why it is a suggestion, not a mandatory treatment date.
- If a matching unique record is unavailable, use the no-qualifying-felling fallback; never construct phantom felling or infer unseen player marks. Any ambiguous corrupt identity fails safely rather than guessing (source worker verifies conservative validation).

Do not import any new real-world rate, stand forecast or per-rack ecological prediction. `ScenarioCycleState` remains interval-free (S1-D advisory design).

## 4. Eligibility — all conditions must hold

`SkipAvailable` returns a **read-only** result with the first meaningful ineligibility reason. **Initial V1 is intentionally conservative:** current teaching `ScenarioOneTeachingSequence.Current(facts)==7`, with a current-world safe completed teaching state (including Legacy40; if not provable, disable skip rather than break older path). No pending player-action lesson and no “the next objective is automatically completed” claim. User-facing disabled note: “Finish the current lesson first. Advance 1 Year is still available where permitted.”

Also require: (1) the **Work Plan is open**, so field marks have passed the ordinary import path; (2) no existing **Pending or Approved** work orders, including invalid/queued ones; (3) latest review is genuinely acknowledged: `!ui.NeedsAnnualReview` plus saved annual status where applicable; (4) no RackPlanProblem or active rack approval/consent/edit/other dialog, no unresolved mandatory first review; (5) no ReferencePreviewActive/reference-authoring state, game modal or other active authoring; (6) ecology/controller present, management and year advance allowed, neither Failed nor Year 100; (7) non-zero future target.

`Completed` may allow later management in today's game until Year100, but **a new Completed transition during the skip stops immediately**. At a later user invocation a completed nonterminal world may choose normal one-year progression, subject to current guards; do not silently disable legitimate continued management across old saves without evidence.

The skip **cannot** auto-apply an earlier MFM prescription, stage proposals, mark Fell trees, select a rack/sample, confirm Crop Trees, approve a prescription, import marks while executing, approve a Work Plan, resolve unattended approved work, buy stock or write per-rack history. A targeted management year only returns control for the player's current judgement. D-076 later representative rack management is an external feature, not part of this time controller.

## 5. Execution and annual review semantics

- On user confirmation, validate all guards **again**. Disable conflicting controls, prevent double clicks, lock other save/load/input operations during active ticks and show “Advancing year k of N”. No background thread, alternate simulation, direct year counter change or aggregate shortcut.
- Execute **exactly one** `ScenarioOneManager.AdvanceYear()` per frame/tick on the same authoritative manager instance. Do not call `AdvanceFromWorkPlan()` for intermediate years (it opens Review and has acknowledgment guards). **Never call `MarkAnnualReviewSeen()` or emit an `AnnualReviewAcknowledged` event on behalf of the player**. Each quiet skipped year remains a saved annual record and snapshot but **not acknowledged**.
- Check stop conditions **before** each tick and **after** every successful tick; stop at earliest: `Outcome` changed to Completed/Failed; number of recorded objective achievements increased; `LastStormPerformance.Victims>0` or a new storm event; changed teaching lesson index; Year 25 or Year 100 boundary; controller/cash/manage guard failure; user cancellation **between** ticks; target reached; or other actual pending player decision. Do not treat a low cash balance alone as a special new stop (no accepted threshold).
- A failed `AdvanceYear()` does not roll back earlier successful steps. Halt immediately, show exact number of years actually completed, failed step and the real manager feedback. State no-years-advanced when k=0. No mid-annual-tick cancellation, save/load or Editor teardown; only safe between-tick cancellation.
- End by opening the **normal latest-year Annual Review** at the true stop year. It shows a **presentation-only** elapsed-years summary (years Y+1…Y+k, exact stop reason, links via existing year browser) and the recorded provenance; do not add a history data store or fake earlier acknowledgment. The player may acknowledge **one real destination-year review** after reading; its single event will be saved normally. Reuse this summary in required D-073 feedback V2, avoiding two independent summary panels.
- All intermediate years, deaths, survival, ecology, money and objective checks are already recorded via standard annual writes. Actual 3D forest after the skip reflects every executed action; no claim of unrecorded per-rack historical ecology.

## 6. Exact qualification oracles

**A. Direct-simulation parity (strict byte-for-byte)**: From one authentic identical initial save and seed, run N=1…5 direct `manager.AdvanceYear()` calls **without any acknowledgments**, and compare to skip controller N=1…5 successful ticks. At **every intermediate year** and destination *before destination ack*, capture identical `ForestSaveData` JSON and `ScenarioReferenceArchive.CurrentWorldHash` (plus per-year annual reports, ecological snapshots, events, objective history, cash, trees, deadwood, planted juveniles, outcome). Use at least one post-teaching quiet state and one post-felling state. If a triggered stop cuts N short, compare to direct for exactly k ticks.

**B. Actual UI click-through parity (semantic, not byte-for-byte):** compare skip to one-year UI advancement with **real user acknowledgments each year**. Their whole-world hashes must **not** be asserted identical: UI action legitimately writes `AnnualReviewAcknowledged` events and different later `eventId` values. After **removing only those acknowledgment events and consistently renumbering event IDs**, compare all other saved semantic facts, chronological events, annual reports, snapshots, trees, money, mortality, juveniles, objectives and outcome; inspect that no relevant event linkage was corrupted.

**C. Honest reviewed-cycle equivalence:** a fixture with true two-cycle `HasReviewedTwoCycleSequence` completion and a separate fixture with an outcome reached **inside** the skip range; verify the retained completion predicate, same outcome year and first completed/failed stop. Destination ack is a real player action, not a fabricated event.

**D. Negative cases:** every eligibility guard (lesson stages 0–6, Pending/Approved including MFM, unread review, preview, modal, Year100, failed), seeded storm victims/record, new objective, new completion/failure, Year25 cap, injected failure on step k, cancellation between ticks, double-click, and historical prescription/mark never auto-applied.

**E. Exact Unity:** 1280×720 and 1920×1080, k quiet years, progress overlay, history year browser, real save/reload at destination, authentic old Legacy40 and frozen Reference Future v1 compatibility and full record/year hash proof, five-step wall time/machine safety. Qualify exact source candidate before Manager integration. No Tier 3 until milestone qualification.

## 7. Separate **required** objective redesign (outside V1 source changes)

D-074 is **not** reduced to advisory-only as a final product goal. This V1 implements optional calendar navigation, while a new four-file **Spaced Major Intervention Learning Objective** draft must determine the educational acceptance boundary for new Scenario One worlds, distinguish major stand thinning/release from rack access/planting, and **leave pre-feature completed saves and frozen references unchanged**. The objective's exact minimum spacing and history policy require an independent product/spec approval and authentic save evidence. No extra goal or timed treatment is implemented by the current V1 source claim.

## 8. Exit/ownership

**Implementation now authorised under this corrected spec, subject to live lane/ownership preflight and Manager issue/packet.** Suggested bounded writer: Sol 6.1 Medium local source on the **isolated accepted C-01 integration lineage**, not diverged `main` or personal playtest #382. No new save fields/events, annual simulation edits, new biological values or automatic work. Any discovered need for these is a **STOP and Manager spec amendment**, not silent implementation. Keep original Rack V2/five parallel racks, initial MFM three Crop Trees, 30%/40% safeguards, species scope, Legacy40 and frozen Reference Future v1. No Windows/Player build until user personal playtest and explicit authorisation.
