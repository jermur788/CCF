# Independent review: Scenario One Research-Informed Management Learning V1 (four-part draft spec)

**Reviewer:** Claude Opus 5.5 (Claude Code in T3 Code), independent of the GPT-6 Manager-authored draft.
**Date:** 12 November 2026.
**Claim:** [#389](https://github.com/jermur788/CCF/issues/389).
**Branch and path:** `docs/claude-review-research-management-spec-20261112-v1`, at `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md`.

**Status:** Review only. This is not spec approval, not implementation authority and not a Manager verdict.

---

## 1. Executive verdict: **NEEDS CHANGES**

**What the verdict covers:** the four-part DRAFT (spec, plan, tasks and requirements checklist) as a candidate for product-owner approval. Even after these changes are made, approval would not be implementation acceptance.

**Retain:**
- The product intent: be honest about what Share measures, reject a single "correct" competitor, state the wind-model limitation, and avoid grid jargon.
- The protected-contract section (spec §3).
- The non-goals (spec §5).
- The demo boundary.
- The rule that optional retention reasons have no effect on credit, Share, marks, MFM or the Work Plan.

These parts are well drafted and consistent with D-059/D-060/D-065/D-066 and the 11 November Manager disposition.

**Why NEEDS CHANGES rather than APPROVED** (each point is detailed in section 3):

1. **BLOCKING: FR-07's preferred persistence design breaks a contract the spec never mentions.** The current world/reference identity hash is built from the full save JSON, which includes every `ScenarioManagementEvent`. Adding fields to that class, or appending player-intention events, changes world identity unless the most protected hash-layout code is extended again. The project's own precedent (D-066) keeps teaching and progression state *out* of world identity, and keeps per-tree Objective 5 decisions *session-only*.
2. **MAJOR: the baseline is stale in three ways.**
   - FR-03's main change, removing "regenerating cells" from player copy, has been on `main` since 18 October 2026.
   - Research handoffs #387 and #388 have been returned and closed, but the spec and tasks still list them as READY / NOT RUNNING and treat them as open gates.
   - #388 shows that separating natural from planted locations is *not* a text-only change.
3. **MAJOR: FR-06 duplicates an existing decision surface.** The Objective 5 "[J] Retain here / [K] Release further" interaction already exists. The spec doesn't address the overlap.
4. **MAJOR: the verification and lane apparatus is out of proportion to the work.** There are 69 checklist items, Medium/High local Unity lanes, and independent-review gates, while the defensible near-term deliverable is a handful of strings.

**Recommended shape:**
- **V1** = one post-demo copy change, drawing on #387 and #388.
- **The retention-reason feature** = an optional prototype, session-only first.
- **Durable persistence** = deferred to its own spec, designed so it is excluded from world identity.

The case for doing nothing beyond truthful copy is strong. #387 itself concludes that "the existing ability simply to leave a neighbour unmarked already supports the choice without any new UI".

---

## 2. Source access and method register

| Class | What was actually read | Uncertainty |
|---|---|---|
| Object of review | All four files on `docs/scenario-one-research-informed-spec-20261112-v1` (tip `275d5028`): `spec.md` (208 lines), `plan.md` (129), `tasks.md` (182), `checklists/requirements.md` (130). Cross-references were compared | Low |
| Review packet / claim | The packet on the reviewer branch (tip `00d945a3`); issue #389 | Low |
| Canonical authority (live `main`, fetched 12 Nov 2026; tip `4ef650ad`) | `decision-log.md` (D-003…D-072 rows read selectively, with D-042/059/060/061/064/065/066/071/072 in detail), `current-milestone.md`, `game-brief.md`, `research-index.md` (relevant sections), `unity-project-overview.md` (grep), the **live** `coordination/active-tasks.md` (head entries; #389 READY recorded by Manager #200), the MFM V1 spec, the S1-C spec (FR-2/4/8/10/12), the S1-D spec, and the Rack Bay Lesson V1 design | Low–medium (decision log and overview read selectively) |
| Repository code (read-only, `main`) | `ScenarioManagementEvent.cs`; `ForestSaveData.cs` (`CurrentVersion = 24`, `nextManagementEventId`, `ScenarioObjectiveAchievement`, `centuryReview`); `ForestSaveValidation.cs` (event-ID validation); `ScenarioReferenceArchive.cs` (`WorldHash`/`HashLayout`/strip functions); every `ScenarioManagementEventType.*` consumer (grep, with the relevant ones read); `ScenarioManagementHistory.cs` (year view); `ScenarioOneManager.cs` (event recording, snapshot regeneration count); `ScenarioOneObjectives.cs`; `CompetitionShare.cs`; `ScenarioOneUiFacts.cs`; `RegenerationDiagnosis.cs`; `WalkingHudView.cs`; `TreeInspectionView.cs`; `ScenarioOneTeachingSequence.cs`; `ScenarioOneRackReview.cs`; `LearningObjectivesView.Sequence.cs`; `WorkPlanOverview.cs` | Medium. Static reading only, with no compile or runtime. Unity was not opened |
| Research | Six original papers (111-page compendium, SHA-256 `4f1d7557…69336cc5e13`), spot-checked yesterday (P1 pp.3–14, P2 Table 3/abstract, P3 conclusions, P4 parameter status, P5 Hegyi remark, P6 wind/Ireland mentions); the extraction PDF; the #387 report (`IRISH-UK-TREE-RETENTION-RELEASE-WIND-EVIDENCE.md`, read in full through §8); the #388 report (`YOUNG-TREES-AND-REGENERATION-LANGUAGE-IRISH-EVIDENCE.md`, §1–§3 and copy sections); the six-paper Manager disposition; the post-demo retention design note | Medium for the papers (spot-checked, not every page); medium for #387/#388 (Manager review pending; their external citations were **not** independently re-fetched by me) |
| **Not read** | The four CCF evidence composites (only their research-index entries); `CCF_Book_Definitive_Reference.md`; the original Irish marking pages 84–87; the external sources #387/#388 cite; the complete active-tasks file (it exceeds 300 KB) | Scientific claims below that depend on these are attributed to #387/#388, not verified by me |

**Method:** I compared the spec's assumptions against actual source code, not against prior specs. Every architectural claim below cites a file and line on `main`. I ran no Unity, compilation or tests.

---

## 3. Ranked objections

### BLOCKING

**B-1. FR-07's event-reuse design conflicts with world-identity hashing and the D-066 persistence precedent.**

*Where:* `spec.md` §FR-07, line 134: "represent a recorded retention intention as a new typed `ScenarioManagementEvent` …". Line 136: "using existing fields where compatible and adding explicit named fields when absent". `plan.md` §4 item 2 says one existing event timeline is "safest".

*Evidence (observed on `main`):*

1. **There are no fields for the data the spec needs.** `ScenarioManagementEvent` (`Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs:40–70`) has one `targetTreeId` and no field for a second (neighbour) tree, a reason code, a reason version or a record/revise/clear action. So "using existing fields" can't satisfy FR-07. New fields are mandatory.
2. **New fields change every saved event, and so the identity hash.** `ScenarioReferenceArchive.CurrentWorldHash` hashes `HashLayout(JsonUtility.ToJson(data))` (`ScenarioReferenceArchive.cs:224–227`). `HashLayout` keeps pre-existing identities stable only by string-stripping the *exact default text* of each later schema's additions (`:246–276`), for example `.Replace(",\"prescriptionId\":0", "")`. The comment at `ScenarioManagementEvent.cs:67–68` says the v23 field was "declared last so the hash compatibility path can remove exactly this default text". Any new event field appears in *every* serialized event, so every world identity changes unless `HashLayout` gains another version-specific strip branch. That applies to both the default-prescription and the prescription-carrying branch. `ScenarioReferenceArchive` is protected Reference Future / Legacy compatibility code.
3. **A recorded intention would change world identity.** A non-biological player note would enter the hashed `managementEvents` list. D-066 set the opposite precedent: the teaching latch "never participates in a world/reference identity, even when true" (`ScenarioReferenceArchive.cs:279–281`; D-066 "excluded from protected/world identity hashes"). Verification code asserts world-hash equality across previews and inspections (`ScenarioOneInteractionVerification.cs:85`; `ScenarioHabitatPresentationVerification.cs:115,174`). An intention recorded *during tree inspection* would make "inspection does not alter the world" untrue by construction.
4. **The project already deliberately keeps per-tree retain decisions out of the save.** `ScenarioOneRackReview.cs:5`: "Session-only evidence of real inspections and decisions, not an ecological or saved progression system". Objective 5's "Retain here / Release further" decisions are not saved. Only the aggregate D-066 latch is durable. FR-07 would reverse that minimal-persistence decision at a finer, per-neighbour-pair grain without naming the precedent.
5. **Consumers would mostly tolerate a new type, so the spec's STOP condition looks at the wrong risk.** Every type-filtered consumer I found (completion rules, cycle state, intervention history, objectives, learning sequence, rack stage) selects specific types (`WorkResolved`, `AnnualReviewAcknowledged`, `YearAdvanced`, and so on), so a new enum value would be ignored there. `YearHistoryView.Events` collects *all* events in a year (`ScenarioManagementHistory.cs:316–320`), so any future renderer of that list must handle the new type. The HUD and rack-stage caches recompute on any event-count change (`WalkingHudView.cs:170`, `ScenarioOneManager.Racks.cs:62`). That is harmless but means more churn. Event-ID validation is uniqueness and positivity only (`ForestSaveValidation.cs:155–170`). So the spec's STOP trigger ("if additive event entries would disturb other existing consumers") inspects the wrong risk. The real hazard is identity hashing and the persistence policy, and the spec's acceptance criteria (AC-16–AC-25, AC-43, QUAL-07) never test `WorldHash` stability.

*Fix:*
- Remove "preferred `ScenarioManagementEvent`" from the spec.
- Make the persistence choice an explicit product decision with three options (section 8, D-2):
  - (a) **no persistence**;
  - (b) **session-only**, reusing the `ScenarioOneRackReview` pattern;
  - (c) **durable**, as a separate `retentionIntentions` list on `ScenarioOneSaveData` with its own ID allocator, excluded from `HashLayout` by one whole-field strip (the D-066 pattern), read by an S1-C adapter as player-authored L1. That keeps S1-C's single *management-history* authority intact, because intentions are annotations, not management events.
- If (c) is chosen, add acceptance criteria:
  - `WorldHash`/`CurrentWorldHash` unchanged with and without intentions;
  - old saves hash identically;
  - Reference Future archive verification unchanged.

### MAJOR

**M-1. FR-03 is largely already implemented. The remaining parts are not "text only".**

*Where:* `spec.md` FR-03 (lines 89–97); `tasks.md` I-02 (line 64: "text only"); `checklists/requirements.md` AC-06/AC-07/AC-10.

*Evidence:*
- `ScenarioOneUiFacts.cs:85–95` already maps objective ID `regeneration` to **"Areas with young trees"** and `reference-regeneration` to **"Areas with young regeneration"**, for both live and Century Review rows (`AnnualReviewView.cs:776–777`). `PlainText` rewrites stored "cell(s)" as "map square(s)". This landed on 2026-10-18 (`aad4e6f1`, "wording only - objective says what it counts").
- Review and history lines already say "areas with young regeneration" (`ScenarioEcologyReviewLines.cs:46–47`; `ManagementHistoryText.cs:132,143,216`).
- The internal `displayName` "Maintain regenerating cells" (`ScenarioOneObjectives.cs:65`) and "Regenerating cells" (`:170`) remain. **This must stay that way.** `ScenarioCenturyReview.referenceComparisons[].displayName` is *persisted* (`ForestSaveData.cs:74`), so changing evaluation labels would alter newly saved Year-100 JSON and so the world hash. The existing presentation-by-ID mapping is the correct pattern.
- Remaining jargon I found: "regeneration cohorts" in `WorkPlanOverview.cs:193`. #388 adds that "Establish planted Oak/Beech" overstates its predicate (presence somewhere plus one successful planting).
- "Distinguish natural young growth from planting" (FR-03, spec §1 item 5) needs **new aggregation**. #388 §3 shows that per-species `regenerationCells`, `plantedRegenerationCells` and `plantedJuveniles` are "not additive partitions of unique global locations". A natural-only unique-location count doesn't exist. That is a snapshot or derivation change, not copy.

*Fix:* Rewrite FR-03 as a *residual* audit:
- the existing label (optionally change the constant to #388's "Areas with young growth");
- the "cohorts" string;
- the Oak/Beech label;
- one Help sentence.

Then:
- State that internal `displayName` strings and objective IDs stay byte-identical, and that all changes are presentation mapping.
- Split the natural/planted breakdown into a deferred FR-03b.
- AC-07 should name the persisted Century Review `displayName` explicitly.

**M-2. The research gates and handoff states are stale; the returned research changes the recommendations.**

*Where:* `spec.md` line 11 ("Claims #387 and #388 are READY/NOT RUNNING"); `tasks.md` lines 12 and 22; checklist line 123 ("Browser reports not returned"); Gate G1.

*Evidence:* #387 and #388 are `[HANDED-OFF]` and CLOSED, with reports published on their branches (Manager review pending). Their conclusions materially affect FR-01/04/06:
- #387 §6 supplies ready-to-use copy options A–D. It recommends A or D and says that leaving a neighbour unmarked already supports retention without new UI.
- #387 §3 flags that "canopy shelter" must not imply mechanical wind protection.
- #388 §1/§3 recommends "Areas with young growth" and finds that edge/open labels for specific points **need game data**.

*Fix:*
- Update the status lines.
- Make G1 "Manager disposition of #387/#388" rather than "returned".
- Fold the recommended copy into FR-01/04/05 as the candidate wording, so it isn't redispatched.

**M-3. FR-06 overlaps the existing Objective 5 retain/release decision; key definitions are missing.**

*Where:* `spec.md` FR-06 (lines 118–130), FR-08 item 4 (line 161); checklist AC-20, AC-33.

*Evidence:* Objective 5 already asks the player, per Crop Tree, "[J] Retain here / [K] Release further" (`ScenarioOneRackBayText.cs:39,54`; `LearningObjectivesView.Sequence.cs:128,152`; session-only per `ScenarioOneRackReview.cs:5`). FR-06 adds a second, differently scoped retain concept: per neighbour, with a reason. The spec doesn't reconcile the two:
- which keys;
- what happens when both are visible;
- whether "Retain here" should simply gain the reason vocabulary.

That conflicts with packet question 15 (duplicate explanation systems, extra clicks) and with spec §6's own "no third management mode" spirit.

*Missing definitions:*
- **"Neighbour associated with a confirmed player Crop Tree"** is undefined. It should be "a living tree in that Crop Tree's `CompetitionShare` set (≤ 8 m, `HegyiCutoffMeters`)", and the spec must say whether that means only the visible top five or all of them.
- **Broadleaf neighbours** of Sitka Crop Trees, arguably the most meaningful retention case, aren't discussed.
- **AC-20 "meaningful state change"** is undefined.
- **FR-08 item 4 / AC-33** ("derived change only if original values recorded") can never be satisfied as designed, because FR-07 stores no Share at recording time. Either store Share-at-record (as `targetValueAtAchievement` does for objectives) or delete the item.

*Fix:*
- Design FR-06 as an *extension of the existing Objective 5 decision* (add an optional reason to "Retain here"), or explicitly justify a separate neighbour-level control.
- Define the association rule.
- Drop or ground the derived-Share item.

**M-4. FR-01 under-specifies what Share does *not* model, and existing copy contradicts FR-01/02's aim.**

*Where:* `spec.md` FR-01 (lines 65–75).

*Evidence:*
- `CompetitionSample` (`CompetitionShare.cs:6–24`) carries ID, position, DBH, living, planned-fell and in-footprint flags. It has **no species, height, crown or light direction**. Share is species-blind (Hegyi DBH ratio over distance, within 8 m), and other Crop Trees count in the denominator (Rack Bay design §"What Share means").
- FR-01 says "size/distance" but never states the omissions that matter most to a forester: crown position, height, species and shade tolerance.
- Live Lesson 3 copy says "**being small is not a reason to fell**" (`ScenarioOneTeachingSequence.cs:46`). Within crown release that is defensible, because small neighbours carry little Share. As a bare statement, though, it reads as a rule against thinning from below, the method P2 and #387 note is most familiar to UK and Irish staff. The spec's "no single right answer" goal should cover existing copy, not only new copy.
- On the other side, much of FR-01 already exists. Lesson copy labels the 30% target and 40% safeguard as "gameplay calibration", and 3 trees / 8 m as "teaching choices … not a forestry prescription" (`ScenarioOneTeachingSequence.cs:44,49`). The real change is small.

*Fix:*
- Add one clause listing what Share ignores.
- Add a requirement to review the "being small" sentence (for example, "In this crown-release lesson, small neighbours usually add little Share").
- State that the real change is roughly two sentences, using #387 option A or D.

**M-5. FR-04 leaves the door open to a hidden spatial classifier; the model has none, and "open" is already a light-band label.**

*Where:* `spec.md` FR-04, line 106 onward: "An edge/open classification requires existing source-supported spatial geometry or a separately qualified read-only derivation".

*Evidence:*
- No edge or margin classification exists in the ecology code.
- The ground panel already gives model-observed explanation per spot: light value plus `LightBand` (deep shade / shade / partial shade / **open** / bright, from thresholds at `RegenerationDiagnosis.cs:139–141`), browsing state, a regeneration summary and a causal "Why:" line (`WalkingHudView.cs:236–246`; `ScenarioOneUiFacts.cs:32–50`).
- P6's "open" (bare-land situation) and the game's light-band "open" (light 0.35–0.60) would collide.
- #388 marks general shelter/edge/open text "COPY READY as general help" and point labels "NEEDS GAME DATA".

*Fix:*
- Make FR-04 V1 Help-only, using #388's proposed sentence.
- Remove the "separately qualified read-only derivation" option from V1 (defer it).
- Add an AC that P6 situation terms are not shown next to the light-band "open" label in a way that reads as one classification.

**M-6. Verification and lanes are out of proportion to the work.**

*Where:*
- checklist: 69 checkbox items (REQ-01–09, AC-01–43, RESEARCH-AC-01–07, QUAL-01–10);
- `plan.md` A2 (Medium local Sol/Codex Unity lane) and A3;
- `tasks.md` M-02 (a full string/serializer inventory before copy);
- Q-02 Tier 2 covering copy.

*Evidence:* With M-1 and M-4 applied, the approved Wave A deliverable is about 4–6 strings in 3–4 files. AC-03 (two alternative Share sets get equal credit) tests *existing* behaviour. The lesson already uses a Share sum rather than tree IDs (Rack Bay design §Stage B), and `Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py` already covers the rack-area analysis offline. AC-16–AC-43 (28 items) apply only if persistence is approved, yet they sit in the same approval checklist.

*Fix:*
- Split the checklist into **V1-copy** (≈8 checks: string inventory, no internal ID/`displayName` change, wording against #387/#388, 1280/1920 captures of the 3–4 affected panels, no-hash-change confirmation) and **Optional-feature** (conditional).
- Make AC-03 an offline regression in the existing non-UI harness.
- Drop the Medium Unity-lane classification for copy.
- Ride Wave A on the next milestone Tier-3 rather than giving it its own Tier 2.

### MINOR

- **m-1.** `tasks.md` line 54 ("existing reason-code vocabulary") and `plan.md` A2 ("bind existing model Share and state to concise reason labels") are ambiguous. They could be read as auto-generating reasons from Share, which FR-01's "Not permitted" list forbids. Name the vocabulary, or delete the phrase.
- **m-2.** In the FR-06 reason labels, `canopy-shelter` ("Keep canopy cover / potential shelter") risks implying wind or mechanical shelter (#387 §3, E04/E08). Prefer "Keep canopy cover / shade". Consider adding "Keep species/mixture" for a broadleaf neighbour (Pro Silva Ireland retention for species diversity, per #387 E03; P1's native-trees-in-non-native-matrix), clearly marked as intention only.
- **m-3.** FR-08's requirement to print "No retention reason was recorded" for every historical intervention without one adds noise to every pre-feature intervention. Show it only where a retention note could apply (that is, if the feature exists and the player opens a Crop Tree's history).
- **m-4.** FR-05 is right. Prefer a single Help/limitations sentence plus the external-test note. Don't put a wind line on the Lesson 3 panel, which is already dense at 1280×720 (the competitor-tag overlap gate is recorded in the Unity Project Overview).
- **m-5.** In spec §8, Q1, "Manager recommends persistence if later review is the real educational benefit" pre-empts the product decision without evidence of benefit. B0 (the UX comparison) should come first, as the plan itself says.
- **m-6.** The six-paper Manager disposition says the "~27% near-boundary" figure was not reproduced. It is recorded implementation fact in `unity-project-overview.md` (Enlarged80 section: tree-edge shares 0–5 / 5–10 / >10 m = 27 / 17 / 56%). This is relevant to the FR-09 edge-handling tests.

### NOTE

- The spec's protected-contract section (§3) and non-goals (§5) are accurate against the decision log and code. I found no line that silently promotes research into a mandatory mechanic: every new behaviour is gated on G0, and the demo freeze is respected consistently across all four files.
- FR-09 and RESEARCH-AC-01–07 are well bounded (edge handling, cohort boundary, a "NO USEFUL SIGNAL" success outcome). Keep them optional.
- FR-02's separation of states (Crop Tree / considered competitor / Fell mark / MFM proposal / approved work / executed result) is valuable and matches D-065. Keep it as a copy-review checklist item.

---

## 4. Requirement-level disposition

| FR | Disposition | Rationale | Dependencies |
|---|---|---|---|
| FR-01 Share explanation | **REVISE** | Small real change: add what Share ignores (species, height, crown, light); review the "being small is not a reason to fell" line; use #387 copy A or D. Most calibration labelling already exists | Manager disposition of #387 |
| FR-02 Diverse marking | **KEEP (merge into FR-01 as a copy/state checklist)** | Already true by design (Share sum). Verify through an offline regression, not new work | None |
| FR-03 Regeneration vocabulary | **REVISE → residual audit; SPLIT FR-03b (natural/planted breakdown) → DEFER** | Main label done 2026-10-18; keep internal `displayName`/IDs byte-identical; the breakdown needs new aggregation (#388) | #388 disposition |
| FR-04 Shelter/edge/open | **REVISE → Help-only; DEFER location labels** | No classifier exists; the ground panel already explains per spot; avoid colliding with "open" | #388 disposition |
| FR-05 Wind limitation | **KEEP** | One sentence plus an external-test note; two-sided wording per #387 | None for the base fact |
| FR-06 Retention reason | **SPLIT: FR-06a UX comparison (KEEP, docs/mock-up); FR-06b in-game control (DEFER)** | Overlaps Objective 5 [J]/[K]; association rule undefined; #387 says leaving trees unmarked already covers the choice | D-1/D-2 |
| FR-07 Persistence | **REVISE (remove preferred event design) and DEFER** | B-1: world-hash and D-066 conflicts; if durable, use a separate hash-excluded list | D-2, D-3; separate spec |
| FR-08 History integration | **DEFER** | Depends on FR-07; item 4 unachievable as designed; m-3 noise | FR-07 |
| FR-09 Offline metrics | **KEEP (optional)** | Well bounded; may conclude NO PANEL | Fixture export |

---

## 5. Data and architecture audit (retention intention)

| Question | Evidence on `main` | Finding |
|---|---|---|
| Can existing fields hold the intention? | One `targetTreeId`; no second tree, reason, version or action fields (`ScenarioManagementEvent.cs:40–70`) | **No.** New fields required |
| Enum extension safe? | Append-only with an explicit numeric value precedent (`AnnualReviewAcknowledged = 9`, `:15–16`) | Yes, if appended as `= 10` |
| Event-ID allocation | Monotone `nextManagementEventId` saved, with a load guard using max+1 (`ScenarioOneManager.cs:18,1532–1534,2477`); the MFM approval transaction rolls back by count (`ScenarioOneManager.PrescriptionApproval.cs:178–207`) | Usable. An intention written during an approval transaction would be rolled back, but the UI doesn't overlap, so this is low risk |
| Validation | Unique positive IDs only (`ForestSaveValidation.cs:155–170`) | Not a blocker |
| S1-C / S1-D / completion consumers | All type-filtered (completion rules, cycle state, intervention history, objectives, learning sequence) | Ignored there. `YearHistoryView.Events` includes all types (`ScenarioManagementHistory.cs:316–320`), so future renderers must handle it |
| WorkResolved / money / plan counts | Filter `WorkResolved`/`OrderCreated` explicitly | Unaffected |
| **World / reference identity** | `CurrentWorldHash` = hash of `HashLayout(full save JSON)`; per-version default-text strips (`ScenarioReferenceArchive.cs:224–281`) | **Breaks** with new fields (all saves) and with recorded intentions (annotated saves), unless extended. Not covered by any spec acceptance criterion |
| Precedent for player decisions | Objective 5 per-tree decisions are session-only (`ScenarioOneRackReview.cs:5`); the D-066 latch is the only durable teaching fact, excluded from identity | FR-07 reverses this precedent without saying so |
| Persisted labels | `centuryReview.referenceComparisons[].displayName` persisted (`ForestSaveData.cs:74`) | Copy changes must stay presentation-only (relevant to FR-03) |

**Comparing the three options honestly:**

| Option | Effort | Interoperability | Risk |
|---|---|---|---|
| (a) No feature | Zero | — | None. #387 says leaving trees unmarked already supports the choice |
| (b) Session-only note, reusing the `ScenarioOneRackReview` pattern | Low (UI plus an in-memory dictionary) | Visible during the session and lesson; not in history | Low; no save or hash work. Cannot satisfy durable later-year review |
| (c-1) New `ScenarioManagementEvent` type plus fields (spec's preference) | High | Shares the event ID space and year | **High**: protected hash-layout edits, world-identity change, a new save version, Reference Future/Legacy regression and independent review |
| (c-2) Separate `retentionIntentions` list on `ScenarioOneSaveData`, own allocator, whole-field hash strip (D-066 pattern) | Medium | S1-C adapter reads it as player-authored L1; joins by year and tree IDs | Medium: still a save version and S1-C adapter, but **one** strip, no per-event default text, and no identity change |

Lifecycle questions the spec answers well:
- append-only revise and clear;
- old saves mean "not recorded";
- a dead or felled tree keeps its history but is not shown as live.

Questions the spec leaves open:
- Crop Tree **designation removed then re-added** (is the old intention active again?);
- a neighbour that **moves out of the 8 m set** because a Crop Tree died;
- **carried-over Pending work** that fells a neighbour with an active intention (FR-06 rightly gives no veto, but the history should show both facts in order).

---

## 6. Forestry and evidence crosswalk

| Claim used by the spec | Class | Source | Note |
|---|---|---|---|
| Frame-tree selection agrees more than competitor selection (p < 0.001) | Transferred empirical (GB) | P1 compendium pp.10–13, verified | Not a player-quality metric |
| Low thinning gets more agreement than crown thinning; UK and Ireland staff more familiar with low thinning | Transferred empirical plus author assertion | P2 abstract and p.23 (compendium), verified | Supports M-4 on the "being small" copy |
| P2 κ categories (Stoyan scale) | Method | P2 Table 3, verified | Never a score |
| Two-way wind considerations (exposure after release; slender drawn-up trees if unthinned) | Irish guidance plus transferred | P1 pp.5,7 (verified); Teagasc and Forest Research per #387 E08/E09 (not re-fetched by me) | Not simulated (storm model 0) |
| Irish thinning effect on overturning moment not significant | Direct Irish small study | #387 E10 (not re-fetched) | Blocks any "release improves stability" claim |
| Retaining competitors for habitat, timber, seed or species reasons is legitimate practice | Irish professional guidance plus transferred | #387 E02/E03/E05/E06; P1 §1.1 | Intention only; no game effect |
| Share = Hegyi DBH ratio over distance, ≤ 8 m, species-blind | Model fact | `CompetitionShare.cs`; Rack Bay design | The spec should state the omissions |
| 30% release target / 40% basal-area safeguard / 3 trees / 8 m | Gameplay calibration | Rack Bay design; MFM spec | Already labelled in-game |
| "Young growth" vs establishment vs recruitment | Irish guidance | #388 S1–S7 (not re-fetched) | The game counts presence locations, not establishment |
| Shelter / edge / open as gradients, not fixed bands | Transferred concept plus Irish guidance | P6; #388 §3 | No in-game classifier |
| P3 structural indices as biodiversity measures | Unproven (P3's own words: "yet to be proved") | P3 summary and conclusions, verified | Descriptive only (FR-09) |
| P4/P5 parameters, Finnish ingrowth, foreign gap sizes | Not transferable | P4/P5/P6, verified | Correctly excluded by spec §5 |

**Remaining gaps:**
- Manager disposition of #387/#388.
- Independent re-verification of their external Irish citations (needed before shipping *non-obvious* claims; not needed for copy option D or the storm-model-0 fact).
- No Irish evidence on the educational value of recorded retention reasons (a UX question, not a forestry one: B0/FR-06a answers it).
- **No further browser research is warranted** for the V1 copy.

---

## 7. Minimal revised implementation sequence

| Step | Content | Lane / owner | Tests | Cost note |
|---|---|---|---|---|
| 0 (now, docs) | Manager disposes #387/#388; spec revised per this review (status lines, FR-03 residual, FR-07 options, split checklist) | Manager, docs branch | Docs review | No Unity |
| 1 (post-demo, one change) | **Copy V1:** Share-omission clause plus #387 option A or D; reword "being small"; one wind sentence (Help/limitations); #388 general shelter/edge/open Help sentence; residual jargon ("regeneration cohorts", Oak/Beech label); optional label change to "Areas with young growth" in the `ScenarioOneUiFacts` constant | One UI owner, one small PR, file list ≈ `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiFacts.cs`, `WorkPlanOverview.cs`, the Help source | Tier 1: string inventory; no `displayName`/ID diff; `WorldHash` unchanged on a fixture; 1280/1920 captures of the affected panels; source-only review. Rides on the next milestone Tier 3 | ≈ one Low-difficulty session. A separate Tier 2 or independent review would cost more than the change |
| 2 (optional) | FR-06a mock-up comparing no prompt with a reason added to "[J] Retain here" | Manager / UX docs | Product verdict | No code |
| 3 (only if step 2 says "session") | Session-only reason on the existing Objective 5 decision | UI owner | Tier 1 plus one interaction capture | No save |
| 4 (only if "durable") | New spec: hash-excluded `retentionIntentions` list, S1-C L1 adapter, lifecycle cases from section 5 | Save/history owner; independent review | Tier 2: roundtrip, old saves, `WorldHash` equality, Reference Future archive check, MFM non-interference | High; justified only by demonstrated value |
| Parallel optional | FR-09 offline metrics | Analyst, no Unity | RESEARCH-AC | Independent of steps 1–4 |

**The case for stopping after step 1** is strong:
- it delivers most of the educational value (honest Share, no single answer, the wind limitation, clearer regeneration language);
- it touches no save, hash, MFM or lesson predicate;
- #387 judges the existing "leave unmarked" freedom sufficient.

---

## 8. Product decisions for Manager / user

1. **D-1: V1 scope.** *Preferred:* copy-only post-demo (step 1). *Alternative:* the full package. *Consequence of the alternative:* save, hash and independent-review cost for unproven benefit.
2. **D-2: Retention reason.** Options: none / session-only / durable. *Preferred:* none for V1; decide after the FR-06a mock-up and the personal playtest. *If durable:* see D-3.
3. **D-3: Durable storage (only if D-2 = durable).** *Preferred:* a separate list excluded from world identity (D-066 pattern), **not** `ScenarioManagementEvent`. *Consequence of the event route:* protected `HashLayout` changes and a world-identity change for annotated saves.
4. **D-4: Copy choices.**
   - Share copy: #387 option **A** (fuller) or **D** (lowest-claim). *Preferred:* D on the lesson panel, A in Help.
   - Regeneration label: keep "Areas with young trees", or switch to #388's "Areas with young growth". *Preferred:* switch (a one-constant change).
5. **D-5: Wind wording placement.** *Preferred:* Help/limitations plus the external-test note; not on the Lesson 3 panel.

The spec already answers these correctly, so I'm not reopening them: no change to 30%/40%; no storm-model activation; no biodiversity score; no marteloscope grading.

---

## 9. Orchestration evidence summary

| Field | Value |
|---|---|
| Requested | Claude Opus 5.5, Medium (packet) |
| Executed model / provider | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code; single session; no subagents or delegates |
| Executed effort | UNKNOWN (not reported to the session) |
| Tools | `git fetch`/`git show`/`git grep` against `origin/*` refs (read-only); GitHub CLI (`gh`) for the issue and the Contents API publication; the GitHub MCP server failed to connect (HTTP 400), so it wasn't used. No local worktree, Unity, build or test |
| Branch / path | `docs/claude-review-research-management-spec-20261112-v1` : `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md` |
| Edits | This one file only; issue #389 status comments and title. No spec, code, canonical or coordination edits |
| Time / tokens / cost | UNKNOWN |
| Readback | Recorded in #389 after publication |

---

## 10. Hand-off status

- **Report URL:** https://github.com/jermur788/CCF/blob/docs/claude-review-research-management-spec-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-INDEPENDENT-FOUR-PART-SPEC-REVIEW.md
- **Claim #389:** marked [HANDED-OFF] and closed only after publication and readback (see the issue).
- **Status:** the Overall Manager issues the final APPROVED / NEEDS CHANGES / REJECTED disposition. This review does not authorise implementation, merge or spec approval.
