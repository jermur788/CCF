# Claude B review: Scenario One management cadence and two advance options V1

**Claim:** [#396](https://github.com/jermur788/CCF/issues/396) · **Packet:** `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-CCF-CADENCE-TWO-ADVANCE-OPTIONS-DESIGN-REVIEW-PACKET.md` · **Date:** 2026-11-12

**Verdict: NEEDS CHANGES.** The product intent (D-074/D-075) can be built on the existing annual authority without a new simulation, a new store or a save schema change. The draft does not yet pin down five things an implementer needs: how skipped years treat review acknowledgment, what parity reference it is tested against, the target-year formula, when skip is available during teaching, and the stop set. All five are specified below. None needs new ecology or persistence.

## Reviewer identity and provenance (actual)

| Item | Value |
|---|---|
| Actual model / effort | **Claude Opus 5.5 (`claude-opus-5-5`), low reasoning-effort setting**, Claude Code. The packet preferred Sonnet 5.5 Medium. The user dispatched this session directly, so no cost escalation was requested. |
| Independence | This session did not write the cadence spec, the V2 feedback spec or the C-01 source. |
| Quota | UNKNOWN |
| Spec head inspected | `docs/scenario-one-research-followon-roadmap-20261112-v1` @ `0819501a` |
| Source head inspected | `feature/scenario-one-research-copy-v1-20261112` @ `a314d308`. `main` @ `2ce6ce64` has no `Assets/` change since merge-base `50042bbe`, so the feature branch is the newest production source. |
| Method | Static `git show` / `git grep` only. **No Unity, no compile, no runtime run.** No production code, spec, active-tasks, save or reference was changed. |
| Not inspected in depth | D-076 four-file draft (I applied only the interface check the packet asks for); the #394 audit body (I re-derived the facts it cites from source instead). |

Labels used below: **[V]** verified in source at `a314d308`; **[D]** reviewer derivation; **[A]** assumption I have not verified.

---

## 1. Source facts the design must respect

| # | Fact | Where |
|---|---|---|
| S1 [V] | `AdvanceYear()` runs: guards (ecology, `CanManage`, cash/harvest), resolves **Approved** orders, one `ecology.AdvanceOneYear()`, then juveniles, understorey, deadwood, report, `annualReviewSeen = false`, a `YearAdvanced` event, an ecological snapshot and `EvaluateProgress()`. | `ScenarioOne/ScenarioOneManager.cs:1264–1387` |
| S2 [V] | `AdvanceYear()` does **not** check `annualReviewSeen`. The "acknowledge before next year" gate exists only in the UI. | `UI/ScenarioOneUiRoot.cs:663–670` (`AdvanceFromWorkPlan`: `RackPlanProblem` → `NeedsAnnualReview` → `ShowReview`) |
| S3 [V] | `MarkAnnualReviewSeen()` sets the flag **and** writes an `AnnualReviewAcknowledged` event whenever `ShouldRecordAcknowledgement` is true. For a quiet year with no `WorkResolved` that is always true, because the year differs. So a manual player writes **one ack event for every year**. | `ScenarioOneManager.cs:391–401`; `ScenarioOneCompletionRules.cs:63–70` |
| S4 [V] | `managementEvents` is serialized in `ForestSaveData`, and `WorldHash` hashes `JsonUtility.ToJson(data)` with only layout-version stripping. Every ack event therefore changes the world hash and all later `eventId`s. | `ForestSaveData.cs:65`; `ScenarioReferenceArchive.cs:221–260` |
| S5 [V] | The ecology RNG is stateless per year: `new System.Random(Seed(model, simulationSeed, year, salt))`. Batching does not change draw order as long as the same code path runs for the same years. | `SimulationRandom.cs:37` |
| S6 [V] | Storms resolve inside `AdvanceOneYear` with no player gate (`resolvingStorm`, `pendingStormDeadwood` are internal). | `ScenarioOneManager.cs:529–551` |
| S7 [V] | `ReviewYear = max(MinimumCompletionYear=25, CenturyReviewYear=100) = 100`. `CanManage` blocks only on `Failed` or year ≥ 100. A `Completed` outcome (possible from year 25) does **not** block further advance. | `ScenarioOneManager.cs:2360`, `2390–2398`; `ScenarioOneDefinition.cs:77–78` |
| S8 [V] | `HasReviewedTwoCycleSequence` accepts any two distinct years with a successful, matching completed order and an ack in between. Planting followed by planting the next year qualifies. | `ScenarioOneCompletionRules.cs:36–48` |
| S9 [V] | `ScenarioInterventionHistory` treats any year with at least one succeeded task as an "intervention". The economy separately counts felling years for `FirstThinning`/`SecondThinning`/`LaterThinning`. | `ScenarioInterventionHistory.cs:10–13, 99–100`; `ScenarioOneEconomyAdapter.cs:164` |
| S10 [V] | `ScenarioCycleState` (S1-D L3) is documented as **never naming an interval and never a gate**, and its `AdvisoryLabel` says "not a schedule, a recommendation or a requirement". | `ScenarioCycleState.cs:8–14, 50` |
| S11 [V] | Teaching sequence: lessons 0–5 need player action in the world (access, Crop Trees, release, deadwood, rack review, extension, planting). Lesson 6 needs `Year > PlantingYear` **and** an ack dated after planting. Lesson 7 means done. | `UI/ScenarioOneTeachingSequence.cs:128–152`; `UI/LearningObjectivesView.Sequence.cs:57–110` |
| S12 [V] | Field marks join the Work Plan as Pending orders only when the Work Plan opens (`SetWorkPlanOpen(true)` → `AddMarkedTreesToWorkPlan`). | `ScenarioOneManager.cs:2316–2324` |
| S13 [V] | MFM prescriptions are stored with `approvalState`. `AdvanceYear` does not read or write them. | `ScenarioOneManager.Prescriptions.cs:12` |

---

## 2. Ranked findings

### Blocking

**B-1 — The spec never says how skipped years treat review acknowledgment, and its parity requirement cannot be met as written.** FR-A3/AC-4 require skip to equal "N one-year runs" with the same seed. The only UI one-year route requires an acknowledgment before each next year (S2), and each acknowledgment writes a hashed event (S3, S4). So a skip with no acks per year can never match a UI-manual run byte for byte. A skip that calls `MarkAnnualReviewSeen()` for each year would match, but it would write N **fabricated** `AnnualReviewAcknowledged` events. Those events feed completion chronology (S8) and lesson 6 (S11), which FR-A6/AC-6 forbid.

*Required change:*
- (a) Skip **never calls `MarkAnnualReviewSeen`** for intermediate years. It calls `manager.AdvanceYear()` directly, after its own pre-checks.
- (b) It leaves `annualReviewSeen = false` at the destination, so the normal Review opens and the player's one real acknowledgment is recorded at the destination year.
- (c) Define two parity contracts (see §5).

Derivation [D] that (a) does not change completion: skip is allowed only when no Approved/Pending work exists and the latest review is acknowledged (B-4). So no `WorkResolved` occurs inside a skip, and the ack covering the previous plan already exists. The destination ack still sits between that plan and any future plan, so `HasReviewedTwoCycleSequence` and `EstablishmentReviewed` reach the same truth values. **This must be proven by T3, not assumed.**

**B-2 — There is no concrete target-year formula, so "next management year" is untestable.** FR-A2 leaves it open. Proposed deterministic, read-only policy (no persistence, derived from saved events):

```text
const int ReviewGapYears = 4;     // gameplay calibration (cited 3–6 yr indicative range); NOT a forestry prescription
const int MaxTicksPerSkip = 5;

int? QualifyingYear(events, orders) =
    max year Y such that ∃ WorkResolved e: e.year == Y, outcome == Succeeded,
    matching Completed order o, o.type == FellTree, o.resolvedYear == Y
    // FellTree only: excludes PlantJuvenile, PruneTree, RemoveRegeneration, SalvageDeadwood.
    // Rack-access felling counts (it is a stand-scale cut in Year 1); see open product choice P-2.

SkipTarget Compute(state):
    if (!SkipAvailable(state, out reason)) return Disabled(reason)          // B-4
    q = QualifyingYear(...)
    candidate = (q == null) ? state.Year + ReviewGapYears                  // no stand treatment yet → plain observation interval
                            : q + ReviewGapYears
    if (candidate <= state.Year) candidate = state.Year + ReviewGapYears   // already past the indicative point: next interval, never "overdue"
    target = min(candidate, state.Year + MaxTicksPerSkip, ReviewYear /*100*/)
    if (state.Outcome == Active && state.Year < MinimumCompletionYear && target > MinimumCompletionYear)
        target = MinimumCompletionYear                                      // land on Year 25 so completion is reviewed there
    return Target(target, elapsed = target - state.Year,
                  reason = q == null ? "indicative observation interval"
                                     : $"{ReviewGapYears} years after your Year {q} felling — a point to reassess, not a felling date")
```

Accept 4 / 5 as gameplay calibration for **new** Scenario One play, presented with the S1-D advisory wording. Because of S10, the cadence feature must be described as a separate L4 *navigation suggestion*. It must not change `ScenarioCycleState`, which stays interval-free.

**B-3 — The set of runtime stop conditions is incomplete.** FR-A4/A5 name categories ("new risks/events") but no checks. Storms resolve silently (S6), and `Completed` does not block advance (S7). Required exact **post-tick** stop set, evaluated after every `AdvanceYear()` returns true:
1. `Outcome` changed (Completed or Failed) this tick.
2. `ObjectiveAchievements.Count` grew this tick. [V] There is no objective event type: achievements are a separate persisted list (`ScenarioOneManager.ObjectiveHistory.cs:12, 47–56`). A `ScenarioCompleted` event also covers stop 1.
3. Storm victims this tick (`ecology.LastStormPerformance.Victims > 0`) or any new `stormEvents` record.
4. `CenturyReviewed` recorded (year 100).
5. Teaching lesson index changed. Defensive only: should be unreachable given B-4.
6. `AdvanceYear()` returned false. Report its `feedback` and the years already completed. No rollback.

Then stop at the target. Every stop opens the normal Review for the stop year with an "elapsed k of N years, stopped because …" line. Optional extra: stop when cash falls below an approved threshold [A, product choice P-4].

**B-4 — The spec does not say when skip is available during teaching.** Lessons 0–5 need same-year player action, and lesson 6 is satisfied by one year plus a real ack (S11). Skip must be **disabled while `ScenarioOneTeachingSequence.Current(facts) < 7`**, with the plain reason "Finish the current lesson first; Advance 1 Year is still available." This also covers early Rack Objective 1. Full pre-check set (all must hold; otherwise disabled with the first failing reason):
- Teaching complete (`Current == 7`), or the sequence does not apply [A: confirm Legacy40 resolves to 7 or "not applicable" rather than being stuck below 7].
- `!NeedsAnnualReview`.
- `RackPlanProblem` empty.
- No order with status **Approved** or **Pending**. Count Pending including invalid ones; this is stricter than `AdvanceGuard`, deliberately.
- Skip is offered **only inside the open Work Plan**, so field marks have been imported (S12).
- `!ReferencePreviewActive`.
- `CanManage()` true, ecology present, `!referenceAuthoring`.
- No open map editor, consent or reopen dialogs.

**B-5 — D-076/MFM interface check: the spec needs one explicit negative clause.** Source verdict [V+D]: a loop of `AdvanceYear()` resolves only Approved orders, reads no prescription (S13), creates no orders and imports no marks (marks import only on Work Plan open). B-4 forbids Approved/Pending at entry. So skip **as designed** cannot auto-reuse a historical MFM prescription, commit proposed work, choose a rack or tree, resolve new marks or create per-rack history. The spec must still say this as FR-A4b, with a T3 negative test, so that a later implementer does not "help" by re-applying the last prescription. The "management review year" is a prompt to reconsider rack-area sampling and stand response. Any multi-cycle representative-sample data contract is an **external D-076 dependency**: the cadence feature does not need it and must not invent it.

### Major

**M-1 — Do not change the completion predicate in this feature.** Old saves can already have completed with consecutive-year cycles (S8). Changing `HasReviewedTwoCycleSequence` would alter completion year and world identity, and therefore Reference Future v1 and Legacy40 expectations. Recommendation: **guide-only cadence for V1.** A spaced "major intervention" objective, if wanted, needs a separate spec gated on save version or world identity, so that pre-feature saves keep the old predicate. Product choice P-1.

**M-2 — "Intervention" wording.** Do not relabel `ScenarioInterventionHistory` ordinals as "thinnings". The cadence reason text must say "felling" (FellTree years, B-2) and must not count planting or protection as a "management intervention".

**M-3 — Execution model.** Run the batch **one tick per frame** (coroutine or Update-driven loop) using the same manager instance, with input locked behind an "Advancing year k of N" overlay that cannot be cancelled mid-tick. Allow cancellation between ticks. No async thread, no second simulation, no new Editor. [A] Per-tick cost is today's one-year cost. Measure the 5-tick wall time on the Tier 2 head.

**M-4 — Save/load mid-run.** A save taken during a skip must be impossible (input locked) or must happen only between ticks. Because each tick is a complete `AdvanceYear`, any between-tick save is an ordinary save and needs no new field. There is no "skip in progress" persistence: after a reload the player is simply at year k with an unread review.

**M-5 — Review legibility.** At the destination the Review shows the destination year, plus a compact "years Y+1…Y+k" list that links to the existing year browser (`TryShowReviewYear`). This should be reused by the V2 feedback feature (§7 of that spec), not duplicated. Quiet years stay **unread-but-recorded**. They have reports and snapshots but no ack event, and the history UI must not show them as "acknowledged".

### Minor

- m-1: FR-A4 "insufficient disk/test conditions" is not a runtime player condition. Move it to qualification.
- m-2: The label should be "Skip to Year {target} (k years)", with the reason line from B-2. Hide skip rather than disabling it only when the scenario is Failed or at Year 100.
- m-3: AC-10 "no repeated-click automation denial" is unclear. Replace with "skip is idempotent against double-click: one request at a time".
- m-4: The plan's Stage 2 "Prefer Sol 6.1" is a routing note, not a spec requirement. Keep it out of the acceptance list.

---

## 3. Data contract (no persistence)

```text
static class ScenarioManagementCadence            // pure, read-only, L4 navigation suggestion
    SkipTarget Compute(ScenarioOneManager m, TeachingFacts f)
    record SkipTarget(bool Available, string Reason, int TargetYear, int Elapsed, int? QualifyingYear)

class TimeSkipController (UI)                     // thin loop over manager.AdvanceYear()
    pre: Compute().Available
    for k in 1..Elapsed: one tick per frame; if !AdvanceYear() → stop(error); if PostTickStop() → stop(reason)
    end: ShowReview() with elapsed summary; never MarkAnnualReviewSeen
```

There is no new `ForestSaveData` field, event type, `HashLayout` change or save-version bump. If any of these becomes necessary, stop and write a new spec (FR-A8 already says this; keep it).

## 4. Product choices still open (Manager/user)

- **P-1:** Guide-only cadence (recommended) versus a new spaced-cycle objective for new worlds only.
- **P-2:** Does Year-1 rack-access felling start the 4-year clock? Recommended: yes, because it is simple and honest. The alternative is to count only non-rack FellTree years, which needs rack geometry in the calculator.
- **P-3:** Gap 4 / cap 5 (recommended) versus gap 5 / cap 6.
- **P-4:** Whether to add a low-cash stop.

## 5. Reduced implementation and test path

1. **Spec revision** (Manager): fold B-1 to B-5 and M-1 to M-5 into FR-A2 to A6, record P-1 to P-4 decisions. Then the spec can be approved.
2. **Source** (one writer): `ScenarioManagementCadence.cs` (pure), skip controller in `WorkPlanView`/`ScenarioOneUiRoot`, Review elapsed-years line. No manager or simulation change expected beyond reading existing state. If `AdvanceYear` itself must change, that is a red flag.
3. **Tier 1 (focused, offline or EditMode):**
   - **Parity A (exact):** same seed and decisions; skip of N years versus N direct `manager.AdvanceYear()` calls with no acks. Expect byte-identical `CaptureData()` JSON and `WorldHash` after **every** tick, for N = 1..5, from at least two starting states (post-teaching quiet; post-felling Year q).
   - **Parity B (UI-manual):** skip versus UI one-year with an ack each year. Expect equality after removing `AnnualReviewAcknowledged` events and renumbering `eventId`. Compare also `annualReports`, `ecologicalSnapshots`, cash, trees, deadwood, juveniles, objective achievements and outcome year. The **only** permitted difference is the ack events and the `eventId` renumbering.
   - **Completion equivalence:** a fixture whose completion falls inside the skip window. Expect the same outcome year in A and B, the skip stops in that year, and `HasReviewedTwoCycleSequence` gives the same result.
   - **Negative cases:** skip disabled for each B-4 condition (Pending, Approved, unread review, lessons 0–6, preview, Failed, Year 100). Skip stops for each B-3 condition, including a seeded storm year. An MFM-approved prescription plus a skip produces no new orders or approvals (B-5). An error injected at tick k leaves years 1..k intact and reports k.
4. **Tier 2 Unity** (exact head, exclusive lease): 1280×720 and 1920×1080 skip from a quiet state across 4–5 years; the year browser shows each year; save and reload at the destination; one Legacy40 load and one frozen Reference Future v1 hash check (unchanged, since no simulation code changed); 5-tick wall time.
5. No Tier 3, no Windows/Player build before the user's personal test.

## 6. Verification statement

Every **[V]** item above was read in source at `a314d308`. Nothing was compiled or run. The **[D]** claims (B-1 completion invariance, B-5 no-auto-work) follow from those reads but are **unproven until the T3 tests in §5 pass**. The **[A]** items (Legacy40 teaching index, per-tick cost, low-cash threshold) need checking by the implementer.
