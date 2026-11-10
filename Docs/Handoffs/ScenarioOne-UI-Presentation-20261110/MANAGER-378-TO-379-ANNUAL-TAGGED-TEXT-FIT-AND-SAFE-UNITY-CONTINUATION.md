# ONE SOL DISPATCH — systematic Annual Review tagged-text readability and one safe Unity continuation

**READY / NOT RUNNING.** A new full UI redesign, another Fable review, and redundant broad tests are **NOT authorised**. The latest source-exact Unity #378 run **PASSed Money name/value rows** and **FAILed source-labelled Work/Forest free text** at both 1280×720 and 1920×1080. This packet **fixes the shared component once** and verifies its *whole Annual Review usage*, not one failing sentence at a time. Overall integration remains NEEDS CHANGES.

## Purpose, authority, difficulty, provider

**Purpose:** make all *source-labelled free-text* Annual Review content readable with provenance/qualifiers present, both when source chips are toggled on and off, including long Work and Forest report lines in expanded details. Preserve the real Year 2/3 historical facts, the derived/recorded/current/unknown distinctions and actual signed amounts; the repaired Money rows must remain intact.

- [Manager #200 NEEDS CHANGES ruling](MANAGER-378-TAGGED-TEXT-FAIL-DISPOSITION.md)
- [Sol #378 actual Unity evidence, six screenshots and label-bounds failure](https://github.com/jermur788/CCF/blob/74a7d89e/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-378-ANNUAL-REVIEW-MONEY-ROW-REPAIR-AND-UNITY-RESULT.md)
- [Approved integrated Scenario One UI spec and acceptance checklist](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/spec.md)
- [Fable #373 original source review](https://github.com/jermur788/CCF/blob/4dffb37d6d8d440218e8bd12062eb4ea2d06b790/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md) is historic evidence on an earlier candidate, **not** the repaired candidate. User explicitly cancelled a second Fable review.

**Difficulty:** production change **MEDIUM** (localized shared Annual Review display helper) with **HIGH** end-to-end custody (isolated Unity fixture, separate personal Editor fork, source/provenance and native checks). **Best suited OpenAI Sol/Codex High local T3 Code**. No Anthropic/extra reviewer.

## Claim / exact source / ownership

- **Claim [#379](https://github.com/jermur788/CCF/issues/379): READY / NOT RUNNING.**
- **Frozen exact preceding game source:** `ebe061bae8d3f9da1e810b4c49f49199db8b3d61` (passed #378 Money layout and retained #377 inspection).
- **Prepared isolated GitHub source branch:** `fix/scenario-one-ui-annual-tagged-text-20261110-v1` from exactly that game source, not main, not user test.
- **Proposed isolated single-writer source worktree:** `/media/jer/Files/CCF/Operations/CCF-S1-UI-TaggedText-Repair-Sol` **NOT created or claimed merely by naming**.
- **Writable production scope after admission:** `Assets/ForestPrototype/UI/AnnualReviewView.cs`, specifically the shared `AddTagged` row composition and only essential Annual Review local helper styling in `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`. Add a bounded consumer/probe in `Tools/Verification/ScenarioOneUiPresentation/`. **Do not edit `UiKit.Line`, `TaggedLine` Money layout, `TreeInspectionView`, `Tag` provenance meanings or actual history/gameplay/finance/saves/approval logic** unless truly required and the Manager first rules on scope. Do not touch old verifier assertions, scenes, metas, package, rack, forest simulation or frozen Reference.
- **Existing warmed dedicated Unity lane:** `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual`, from prior #378 after normal release. It is **not currently leased** and not assumed to be available, clean or safe. Never use personal `/media/jer/ZX20/CCF-U3`, original dirty Workbench, user save or personal #361 branch.
- **Exact output Markdown:** `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-379-ANNUAL-TAGGED-TEXT-FIT-AND-UNITY-RESULT.md`.

## Phase A — source component repair and broad-enough presentation checks

**Preflight:** refresh LIVE `Docs/Project/coordination/active-tasks.md`, #379, #378 source/machine release, active UI writers, #361 personal Editor ownership. Verify starting branch, exact SHA, all tracked/untracked/ignored user files, disk, lock registry. Own one new isolated UI source worktree only when safe; mark #379 [RUNNING] with actual model/provider, branch/worktree, starting full HEAD and source ownership; otherwise [BLOCKED] with facts. Do not reuse old source-owner registration.

**Inspected root cause:** `AnnualReviewView.AddTagged` still does:
```csharp
VisualElement row = UiKit.Box("row");
Tag(row, kind);
Label label = UiKit.Add(row, text, textClass);
label.style.flexShrink = 1;
label.style.flexGrow = 1;
label.style.flexBasis = 0;
```
Inside a three-card Annual Review, the always-visible `ScenarioOneTeachingSequence.SourceQualifier` and optional provenance chip occupy horizontal width. The text can collapse to ~43–68 logical px, splitting “Standing”, “Harvest job”, and regeneration lines into fragments. #378 already solved the corresponding `TaggedLine` money problem by separating provenance, label and amount vertically; **do not regress it**.

**Implement one systematic component fix:**
1. In Annual Review only, create a stable **tagged free-text entry** with a *source metadata line* (source qualifier, optional chip) and a **separate full-width body line**. Do not render source qualifier and main text as competing horizontal siblings. Prefer a small `annual-text-entry` plus `annual-text-source` style, or equivalent, with `min-width: 0`, normal wrapping, vertical growth/scroll and clear spacing. Keep text unchanged, maintain return `Label` and previously used classes/provenance hooks `.prov` / `.prov-*`; keep source chips hidden only when `Show data sources` is OFF, while retaining always-visible source qualifier exactly as current contract.
2. Audit **all AnnualReviewView.AddTagged call sites**, not just two examples: headline Work and Forest, historical Work records, group details, regeneration, storm/windthrow, uncertain/reconstructed notes and other recorded sections. All use the same robust row layout; do not hide or clip a long label to make a screenshot pass. Check any special style/parent rows and avoid changing independent amount/money rows or their sign.
3. Preserve exact `Work done / Money / Forest changes` headings and factual values/provenance from selected year; no invented full net annual money, no direct Annual Review year advance and no inappropriate ACK for historical year. No model, source record, history or annual contract edits.
4. Add cheap **actual helper/consumer** tests for source text composition and metadata structure with Recorded/Derived/Current, long texts, parent headline vs expanded detail, sources toggled ON/OFF; a negative that restores horizontal shared row must be **killed**. A screenshot-sized estimate or text-only source contract is not a real render PASS; the test should validate full-width sibling structure, no horizontal label squeeze, and preserved original strings/chips. Be proportionate and avoid padding with 100 permutations.
5. Typed source compile, current small regression suites 160/219/18 plus original presentation, #374 consumer, #377 fit and #378 money-layout tests **selectively**, no expensive whole-forest/progression marathon. No existing assertion deletion/weakening or silent verifier rebaseline. If a relevant test genuinely fails, identify as product regression or checker drift and return appropriate result.
6. Commit/push exact repaired **production source** before Unity; release the source single-writer registration. No uncommitted code/test changes are to be qualified.

## Phase B — one *freshly admitted* focused Unity continuation, conditional

**Safety first:** previous #378 Unity success **does not grant new lease**. Check live #361 user Editor owner and registered global lease, correct ZX20 mount/device/UUID, project at clean exact new candidate, 15 GiB reserve, 27 ignored licensed assets, six frozen protected items, U3 and Workbench before/after tracked+binary diffs. The previous task's 36 origin save/Prefs hashes and path routing are only a baseline: reverify a disposable actual `Application.persistentDataPath`, `PlayerPrefs`, F5/F9 path for *this* process and preserve user bytes with outside backups where required. Acquire a new exclusive machine-wide Unity lock with branch/HEAD/worktree. If any admission item fails, record **SOURCE FIXED / UNITY BLOCKED — NOT RUN**, no Editor, no second warm lane or fallback. No Windows/Player build.

**One mandatory first render gate (not five serial one-line retries):** On exact repaired candidate, load the **same genuine #369 Year 2** archived save in verified disposable slot, native F9, mark P0709 with X, open Work Plan, genuinely approve its one pending job then **separately** advance to Year 3, open Year 3 Annual Review. At **1280×720 and 1920×1080** capture original nonblack PNGs and all relevant actual UI Toolkit **natural text bounds**. Capture (a) sources OFF latest unread, (b) sources ON latest, (c) Earlier Year 2 source-labelled history, (d) expanded detailed records with source labels ON, scrolling to genuinely displayed Work and Forest entries. Include **full row census** of text labels in Annual Review source-labelled entries, highlighting longest labels and any natural text bounds narrower than a reasonably readable threshold. At minimum measure:
- `Standing trees 1195 → 1195` (old 53.75×131.25 at 720 / 67.5×90 at 1080);
- `Harvest job · year 3: ... settled together as one job` (old ~45×323.75);
- `Areas with young regeneration: 73 (was 47) — Sitka spruce 73` (old ~43.75×323.75);
- all four **Money** headline name/value/signs and qualifiers retaining the already-passed #378 layout, and major/longest work/forest text cases.
**PASS requires** readable normal wrapping (not letters split into narrow vertical strips), intact text/provenance, complete displayed amounts, no overlap/clips, correct scroll and accurate selected-year data **at both sizes with sources ON/OFF**. A green driver/pass counter alone is NOT this visual pass.

If **any** material tagged text still unreadable, **STOP** and publish screenshots/bounds, without moving to a new unrelated matrix or in-place production edit. If PASS, continue **only remaining original #376–#378 NOT RUN cases** in this same Editor session where practicable: Objective1 locked Planting/Pruning/Nursery notes and pointer restrictions; real late-stage MFM candidate native C/N from genuine fixture if available (no forcing state); Shift+V reverse held correctly; missing 720p Condition detail; important cursor/focus and rank occlusion; Legacy40/frozen Reference compatibility; full F5/F9 if disposable state verified. Prior #377 17-page Crop inspection and #378 repaired Money lines do not need a full repeat beyond a small regression smoke check.

**Input integrity:** distinguish true native key/pointer interaction from verifier-invoked callbacks, scroll setting and diagnostic camera teleport. Do not call callback execution a native pointer PASS. Do not introduce fake annual report, cash, year, forest or MFM stage merely to fill tests. If authentic fixture inaccessible, mark NOT RUN.

**G3 harness integrity:** older Unity tests expecting removed source panels remain documented separately; never delete/soften them to report PASS. Use additive current-candidate layout measurement and negative controls. If a new, unrelated gameplay defect emerges, STOP for Manager ruling.

## Handoff / release

Write and publish `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-379-ANNUAL-TAGGED-TEXT-FIT-AND-UNITY-RESULT.md` with exact production HEAD, ownership/lease paths, source diff and tester negative controls, native/callback distinction, screenshot/bounds and complete PASS/FAIL/PARTIAL/NOT RUN matrix, all protected user save hashes and asset manifests, honest remaining gates. Commit/push raw screenshots/measurements in its evidence directory when possible; never substitute earlier-candidate PNGs. On completion release writer + machine lease, close #379 **[HANDED-OFF]**, return to Manager #200. **No G4 integration decision, no main merge, no external demo, no Windows build.** No second Fable review.

**Dashboard:** GitHub source/handoff is canonical; dashboard downstream sync/readback NOT VERIFIED unless separate evidence shows ingestion.

## ONE USER COPY/PASTE DISPATCH LINE

> Sol / T3 Code (Codex High): Execute the single Manager #379 Annual Review source-tagged text repair + safe Unity continuation packet at https://github.com/jermur788/CCF/blob/docs/manager-ui-378-tagged-text-disposition-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-378-TO-379-ANNUAL-TAGGED-TEXT-FIT-AND-SAFE-UNITY-CONTINUATION.md . After live single-writer preflight on the prepared isolated branch/worktree, fix the **shared AnnualReviewView.AddTagged horizontal source-label/Work/Forest text squeeze across all call sites**, preserving already-PASS Money labels/signs, Crop inspection, all source qualifiers and yearly facts. Compile and run focused consumer + negative checks; commit source and release writer. **Only with a newly verified exclusive Unity lease, correct mounted ZX20, safe separate warm lane and fully isolated user saves/Prefs**, capture source ON/OFF Work/Money/Forest + expanded records at 720p and 1080p and inspect all tagged-label bounds, not just one example. STOP on any unreadable row; otherwise continue previously NOT RUN matrix in the same safe session as practicable. Publish the exact GitHub report, release machine, close #379 and return Manager #200. **No Fable, no redundant full crop/forest retest, no personal player-test change, no merge or Windows build.**

**User action:** one paste into Sol. No other agent/model interaction required for this task.
