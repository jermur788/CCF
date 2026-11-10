# #374 bounded UI source repair — Manager #200 handoff

IMPLEMENTED ON TASK BRANCH; NOT UNITY-VERIFIED; NOT ACCEPTED OR INTEGRATED.
Independent repaired-delta review: USER-DEFERRED / NOT RUN. No reviewer was launched.

- Worker: T3 Code / OpenAI gpt-6.1-sol, actual session medium effort (packet requested High; no provider or model switch performed).
- Role: sole UI source writer, PACKET ONLY repair under revised Manager dispatch.
- Base / reviewed candidate: `749c5c4f93ede68356a90c42dfaeff9df0aec060`.
- Source candidate: `c683e9eecdbf87960b591c8acabec3139a8cca57`.
- Context: original source lineage `63044dfbbeb869996ceecec8a47619dcba048b0b`; approved integrated UI spec and revised Manager #373→374 repair packet control scope. Latest direct user instruction cancels second review.
- Branch: `fix/scenario-one-ui-presentation-fable-review-20261110-v1`.
- Isolated worktree: `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Repair`.
- Evidence: `/media/jer/Files/CCF/Verification/ScenarioOneUiPresentationRepair374-20261110/`.

## Preflight and ownership

Refreshed live authoritative `ops/workflow-foundation-v1:Docs/Project/coordination/active-tasks.md`. It has no contemporary UI presentation owner row; did not edit Manager's single-writer coordination file. Binding packet expressly dispatches this isolated source claim. #372 and #373 CLOSED; local author/reviewer registrations RELEASED. Prepared remote repair branch was exact reviewed candidate, no existing checkout at proposed path. Created isolated worktree, clean including ignored files, about 200 GiB available on Files. Claimed #374 RUNNING before edits; local `ui-writer-374.json` records actual provider, scope and candidate. No Unity lease acquired. #361, U3 and the original author/reviewer worktrees were not modified.

## Finding disposition

FIXED below means source repair supported by inspection/offline checks; never rendered acceptance.

| Finding | Disposition and evidence |
|---|---|
| F-01 | FIXED: locked sections use a direct disabled card, heading and explanation, with no closed foldout ancestor. Executed exact LockedCard in element sink; removing note is killed. |
| F-02 | FIXED: actual review context hint appears on overview and locked-cursor bottom hints; ordinary Crop hint yields to candidate actions, returns when context has no actions. Uses existing Confirm/Reject/Reopen strings. Executed eight candidate/cursor/Rack-first combinations; deleting MFM hint is killed. Real input remains unverified. |
| F-03 | PARTIAL: page index/navigation moved before content; verbose Fell forecast and storms-off qualification moved to existing draft page with an immediate warning/path. All pages remain keyboard reachable. Actual card fit NOT VERIFIED. |
| F-04 | FIXED: only bracketed key suffix of rack teaching prompt enters bottom key bar; prose stays on card. Executed hint consumer; restoring prose attachment is killed. |
| F-05 | FIXED: section identity uses fixed heading prefix before live `·` count/species suffix. Attention and harvest start open on rebuild, retaining eligibility, contractor minimum and commitment warnings. Source contracts pin both. No native edit/collapse test. |
| F-06 | FIXED source omissions: broadleaf storm caveat on condition page, distribution summary on neighbour detail, signed index change on draft, ordinary-tree Crop discoverability. Timber estimate deliberately remains available even for Crop Trees with explicit “no instruction to fell”; no change to grading/model. V/Shift+V accesses details. |
| F-07 | FIXED bounded test gap: additive repair_consumers.py executes LockedCard and actual hint statements, plus 12 caller/source contracts and 10 negative controls. Selected-year, history ACK, Back, strip, nonmarkable and separate approval guards are source contracts, not executed full Unity consumers. This distinction is intentional and not runtime proof. |
| F-08 | DEFERRED by Manager: old Unity harnesses unchanged; exclusions and successor proposal below. |
| F-09 | FIXED source guards: inspection X/C excludes orange entrances; C excludes V2 rack-removal IDs. Walking bar removes refused Crop action on removal trees. Handler semantics unchanged. Nonmarkable predicate mutation killed by source contract. |
| F-10 | FIXED source guard: objective widget and bottom action bar join occlusion regions. Real 3D overlap NOT VERIFIED. |
| F-11 | FIXED: pending header directly counts open orders with Pending status; invalid Approved orders no longer inflate pending. Existing valid-approved total is retained; attention list exposes problems. |
| F-12 | FIXED: annual detailed records use existing openDetails state and change callback; source-toggle rebuild preserves expansion. Deliberate year navigation still clears expansion as before. |
| F-13 | FIXED relevant facts: material amount labelled RECORDED; explanation identifies tree shelters and separate nursery purchases. Retained “Recorded work materials” label preserves existing assertion. Canopy headline, detailed canopy line and trend use percentages; change uses percentage points. No closing cash headline or invented full net added. |
| F-14 | FIXED: read-only CompetitorAssessment.HasFellMark resolves existing scene-tree index/ID; neighbour row distinguishes unqueued Fell mark from valid open job. Does not modify assessment or orders. |
| F-15 | PARTIAL/selective: F1 Help restored to hints, failed-scenario cue restored as message, invalid walking Crop hint fixed. Clickable HUD menu, duplicate O/Tab wording, optional salvage Rack-first visibility, waypoint badge alignment and typography deferred; no controller redesign. |
| F-16 | DEFERRED nonblocking legacy shims/comments; no broad cleanup. |
| F-17 | DEFERRED historical failures retained: check_baseline check08 already failed base (18 target writes vs expected14); candidate also changes pinned caption. NextActionGuidance checker crashes on removed comment anchor at both base/candidate, with caption drift. Not rerun/repaired or reported PASS here. |
| F-18 | DEFERRED nonblocking: distinct candidate/proposal words retained; purple token unchanged. |

## Actual offline verification

All outputs are under evidence root above. Type compilation references an existing Player's assemblies; it is not Unity import/Editor compilation. No test accessed user saves/preferences.

| Gate | Receipt |
|---|---|
| Runtime typed compilation | exact-compile.log: 189 sources, 116 references, exit0, errors0 |
| Existing focused suite | exact-feedback.log: PASS160; exact consumer slices PASS219; HUD source contract PASS |
| Annual unread lifecycle | reset.log: PASS18; unchanged production lifecycle slices |
| Annual reset negative | reset-mutant.log: expected failure R370_YEAR2_NO_WORK_REPORT_UNREAD |
| Existing feedback negatives | feedback-mutants.log: 8 compiled mutants killed, 4 source negatives refused; unchanged assertions |
| Existing presentation slices | exact-presentation.log: PASS157 and 5 compiled slice mutants killed; repeated page-wrap assertions unchanged |
| New repair consumer probe | exact-consumers.log: locked note + eight hint states; 12 source contracts; 10 negative controls killed (3 executable, 7 source) |
| Scope / whitespace | git diff --check PASS; all changed runtime files under UI; no serialized asset, meta, package, project settings, gameplay, ecology, economy, save, rack handler, RNG or frozen Reference change |

An intermediate presentation probe failed because its unchanged money test pins the old “Recorded work materials” text. Kept that label, corrected its provenance and added explicit shelter explanation; final original probe passes. Initial new-probe fixture errors were corrected before final receipts. Failed intermediate logs retained; none represents a runtime acceptance claim.

Reproduce from repair worktree, using a fresh durable OUT directory:

```bash
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out "$OUT/compile"
python3 -B Tools/Verification/PlayerFeedbackOvernight/run_checks.py "$OUT/compile/bin/Release/netstandard2.1/check.dll" "$OUT/feedback" "$PWD"
python3 -B Tools/Verification/PlayerFeedbackOvernight/review_reset.py "$OUT/compile/bin/Release/netstandard2.1/check.dll" "$OUT/reset" "$PWD"
python3 -B Tools/Verification/ScenarioOneUiPresentation/run_checks.py --src "$PWD" --out "$OUT/presentation"
python3 -B Tools/Verification/ScenarioOneUiPresentation/repair_consumers.py --src "$PWD" --out "$OUT/consumers"
CCF_FEEDBACK_COMPILE="$OUT/compile" CCF_FEEDBACK_MUTANTS="$OUT/mutants" python3 -B Tools/Verification/PlayerFeedbackOvernight/mutations.py
```

## Unchanged older verification and successor measurements

Do not treat these old structural gates as passing the new layout: CombinedHudComposition.cs:250/308/387/419/445; UxQualification.cs:990–1057; RackUxShots.cs:218–224/382–386; UxPedagogyShots.cs:737–743; CropTreeCompetitorVerification.cs:317/342; ScenarioOneTeachingCopyVerification.cs:186. Their obsolete assumptions include inspect-card/inspect-detail names, two independent right panels, estimate and volume simultaneously present, and hidden/restored status panel. UxPedagogyShots:485 and Ccf359V2Closure:681 note-existence checks do not establish rendered visibility.

Proposal for a separately reviewed verification-only task: preserve the same facts/eligibility/target assertions, measure one card and every V page instead of two panels; assert rank-to-tree identity and click/keyboard target agreement; measure persistent Year/Cash and actual objective/keybar occlusion; assert locked explanation has a visible noncollapsed ancestry and rendered text. Add negative controls for dropped pages, wrong target, hidden note and lost historical data. Run genuine 720p/1080p captures and native keys. No harness or threshold was changed here; G3 remains separate.

## Remaining gates / Manager action

Unity NOT RUN; no player-test environment changes, Editor/Player launch, mount, save/prefs access, integration/merge or Windows build. Protected deterministic hashes not executed or rebaselined; source outside UI/tests/report is byte-identical to base.

After separate safe admission only: on exact candidate in Unity6000.6.0f1, inspect Objective1 locked sections; actual candidate Confirm/Reject/Reopen; Objective3 Crop Tree with Fell neighbour inside teaching area; cycle all pages both ways; remove a harvest order while expanded; inspect contractor/eligibility warnings; toggle Annual Review sources with details open; select a distinct earlier year, Back before ACK, then latest ACK; compare 720p/1080p colour/grayscale and rank labels behind objective/bar. Expected observations are the repaired source behaviors above, with no lost information or accidental execution. Native focus/input, clipping, performance, late states, historical save reload and integration remain NOT VERIFIED.

Files changed: six existing UI C# files (AnnualReviewView, CompetitorAssessment, ScenarioOneUiRoot, TreeInspectionView, WalkingHudView, WorkPlanView), test README and new repair_consumers.py; this report only afterward. No assets added/deleted. No protected hash intentionally changed.

Unrelated Primary AGENTS.md and Docs/Operations/StorageArchitecture.md edits preserved. No old author/reviewer worktree or user Unity lane touched.

Source writer RELEASED at publication; #374 HANDED-OFF and closed. Ready for Manager source/evidence disposition only; not independent re-review or acceptance. Stop here; #375 stays cancelled.

DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED. GitHub claim and Manager #200 receive result; no publisher/ingestion path established in this source-only lane. Manager must reconcile authoritative live record and verify ingestion/audit/readback; no generated dashboard files edited.
