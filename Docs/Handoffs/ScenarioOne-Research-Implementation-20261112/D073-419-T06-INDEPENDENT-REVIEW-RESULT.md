# D-073 T-06 — independent browser / GitHub review result (#419)

**Date:** 2026-11-12. **Reviewer:** ChatGPT GPT-6, GitHub connector, browser-only (High requested; runtime thinking setting not independently observable). **Verdict: NEEDS CHANGES — one material FR-10 presentation-provenance conformance problem.** This is a bounded **G2 source/test-harness review** and evidence-integrity audit; it is **not** a finding that Unity failed, an independent G3/pixel PASS, a main/integration approval, or user acceptance.

**Owner/authority:** #419; binding [Manager packet](https://github.com/jermur788/CCF/blob/docs/manager-d073-t06-browser-dispatch-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-419-D073-T06-INDEPENDENT-BROWSER-REVIEW-PACKET-20261112.md). This reviewer had GitHub read/write for the single authorised new result file, but **no Predator filesystem, Unity, screenshots, logs, local tree, physical board, or lease access**. No gameplay, tests, approved spec, main, #382 or Windows/Player build was changed.

## 1. Evidence classification and independently established identity

- **G2 DIRECT — GitHub:** Approved four-file [D-073 spec and checklist](https://github.com/jermur788/CCF/blob/docs/manager-d073-approved-scope-20261112-v1/specs/scenario-one-multiyear-decision-outcome-feedback-v2/spec.md), approved status and Manager #409 verdict inspected. Scope: one in-scroll read-only Annual Review card; no additional save, event, ACK, causal ecology, currency, per-rack histories or D-074/D-076 implementation.
- **G2 DIRECT — GitHub:** The source branch `feature/scenario-one-d073-annual-review-v1-20261112` is **identical** to submitted candidate `77a56a4461f8ab2d313082af50442ce24dc54471` (live compare ahead=0, behind=0). #414 exact helper blob **`2c7bdb3c6b6f13790a325ebabc187c497b335b52`** is present. #415 added the mapper (401 lines), its meta and offline tests; #416 added **50 lines, zero removals** in `AnnualReviewView.cs`, then handoff only. #416's worker reports 53 helper, 132 mapper and zero-error type-binding checks: **reported, not executed by this reviewer**.
- **G2 DIRECT — GitHub:** Qualifier publication branch `qualify/scenario-one-d073-year-comparison-unity-20261112-v1` is exactly **seven commits ahead** of the source, with only eight changed paths: seven new `Tools/Verification/YearComparison` files plus the #417 report, and one additional verifier-driver path in that same tooling folder (eight paths total). **No changed production Assets, Packages or ProjectSettings** in source-versus-qualifier compare. The reviewer branch `docs/d073-t06-independent-browser-review-20261112-v1` was created from the qualifier publication head `2b0bbf1f5aa14f48e982fc79f12c69ad76d88ed4`; the compare was identical before this file's publication.
- **G2 DIRECT — GitHub:** Final runtime5 runner `0d624c61bb8852850a07b4b33d83e740a032c86c` differs from the publication branch by a new Markdown report and **driver docstring only**. Compared executable Python text from the first `import argparse` onward: byte-for-byte identical. The final runner itself is **not** the unchanged production source candidate.
- **G2 DIRECT — GitHub:** `D073BaselineAnnualReviewView.cs` equals the pre-D073 baseline `AnnualReviewView.cs` at `6baee47bd677f48e763b7e8c10092eaf45ba4c79` after exactly replacing class/constructor name `AnnualReviewView` with `D073BaselineAnnualReviewView`; **entire 52,292-character renamed file matches**. This independently validates the baseline class's source lineage, not that the Unity A/B test was executed.
- **G3 WORKER-REPORTED, NOT INSPECTED:** #417 Codex gpt-6.1-sol HIGH reports five Unity 6000.6.0f1 Editor runs; runs 1–4 **overall FAIL/exit 1**, run 5 **exit 0, 99 checks, 18 Game PNG captures** across 1280×720 and 1920×1080, no compiler errors, known unrelated SearchDatabase startup exception, normal leases, unchanged 37 host / 4,011 production hashes, clean end state. The actual raw outputs are Predator-local, and **none of those raw logs, screenshots, JSON audit bytes, save bytes or machine processes were independently examined here**.
- **G3 boundary:** #408's 143 cadence checks are prior unchanged-feature evidence, **not** a newly run D-073 qualification. The new five-run #417 sequence is worker testimony supported by a source-reviewable harness, not independently reproduced Unity.

Links: [#414](https://github.com/jermur788/CCF/issues/414) · [#415](https://github.com/jermur788/CCF/issues/415) · [#416](https://github.com/jermur788/CCF/issues/416) · [#417 report](https://github.com/jermur788/CCF/blob/qualify/scenario-one-d073-year-comparison-unity-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CODEX-417-D073-YEAR-COMPARISON-UNITY-QUALIFICATION-RESULT.md).

## 2. Blocking finding — B-1, Major, FR-10 and presentation half of AC-3/AC-6

**Independent source finding: provenance metadata is constructed correctly but not displayed at the required value resolution.**

The approved **FR-10** explicitly requires **RECORDED for copied selected/latest snapshot values** and **DERIVED for arithmetic differences**, with every displayed value correctly identified. In `ScenarioOneYearComparisonText.AddStandRow` and `AddSpeciesMeasure`, each comparison row correctly stores `ValueProvenance = Recorded`, `DifferenceProvenance = Derived`, and an overall `Provenance = Derived`. The 132-assertion mapper fixture checks the two separated metadata properties.

But `AnnualReviewView.BuildYearComparison` (candidate lines 201–234) renders the whole mixed row as one `row.Text`, and calls `AddTagged(parent, row.Provenance.Value, row.Text, ...)` **only once**. The existing `AddTagged` paints one provenance chip for that entire text line. It never consumes `ValueProvenance` or `DifferenceProvenance`. For example a line of the form `Standing trees: 100 → 105 (+5)` has only **DERIVED**, although 100 and 105 are copied recorded snapshots and +5 is derived. The table header also has separate metadata that the view does not render as separate provenance labels.

This is a **user-visible provenance classification ambiguity in the precise D-073 feature**, not an invented ecological value, save corruption, or proof of a Unity crash. An earlier `[RECORDED]` introductory sentence describes snapshot origins but does not label each mixed value as FR-10 specifies. The pre-existing review used a similar whole-row difference convention, but the **approved D-073 specification expressly requires finer provenance**; inheriting a pattern is not authority to silently relax it. The worker's tests verify mapper metadata and text, **not how the combined values/chips are actually rendered**. A green capture count cannot resolve the source-level omission.

**Required Manager disposition:** Either (A) route a bounded UI-presentational correction so the two observation values are visibly classified RECORDED and the arithmetic change DERIVED, retaining the exact one-card/scroll/wording/units contract; add a focussed UI test/assertion that inspects actual rendered chips/labels rather than only mapper fields, then proportionately qualify the changed exact candidate; **or** (B) explicitly amend the approved FR-10/AC intent after a Manager product ruling to authorise a whole-row DERIVED comparison with context-level RECORDED provenance. Reviewer does **not** recommend silent weakening of existing assertions or any save/event change. No reviewer production edit made.

**Verdict impact:** **NEEDS CHANGES** for strict D-073 conformance; severity **Major, bounded presentation**. No evidence warrants REJECTED, reverting the implementation, or re-running the whole cadence/Tier-3 suite.

## 3. Per-requirement independent matrix

**Legend:** G2 = independently inspected GitHub source/harness; `PASS` means its stated static/source requirement is supported, not tests rerun. G3 = raw Editor/PNG/save/lease **NOT INSPECTED** by this reviewer throughout; #417's claimed results are noted separately. `FAIL` is a source-level acceptance issue, not an independently observed Unity test failure.

| ID | G2 source verdict | G3 direct verdict | Evidence, severity and qualification |
|---|---|---|---|
| FR-1 | PASS | NOT INSPECTED | New card appended to existing `scroll`; #417 checks original ScrollView ancestor/content membership. No second screen. |
| FR-2 | PASS | NOT INSPECTED | Existing year buttons, `ShowLatest`, `TryShowYear`, ACK guard, title, footer and close remain unchanged; #417 worker reports native action parity. |
| FR-3 | PASS | NOT INSPECTED | Sole `Build(manager.HistoryYears, year.Year)`, then `Describe`; no new live forest, intervention delta, prior snapshot source in card. |
| FR-4 | PASS | NOT INSPECTED | Mapper filters `RecordedWork` by `work.Year == viewedYear`, succeeded/failed/cancelled event statuses only. |
| FR-5 | PASS | NOT INSPECTED | Ambiguous rows excluded first, unresolved second, missing/inconsistent order successes counted as event-only with explicit warning; helper checks duplicate event/order identity. |
| FR-6 | PASS | NOT INSPECTED | Exact snapshot year guards, no fallback; latest and selected kept distinct. |
| FR-7 | PASS | NOT INSPECTED | Five stand measures; canopy percent and percentage-point conversion, no underlying mutation. |
| FR-8 | PASS | NOT INSPECTED | Four separate species counts; one-sided species absence names the missing year and no zero substitute. |
| FR-9 | PASS | NOT INSPECTED | No-year hidden, latest one-line, cannot-compare precedence, missing selected/latest, and no-work state. |
| FR-10 | **FAIL — B-1** | NOT INSPECTED | Mapper provenance correct; rendered combined values receive one DERIVED tag, losing distinct RECORDED/DERIVED presentation. **Major.** |
| FR-11 | PASS | NOT INSPECTED | Explicit no-causation caveat, conditional prior-treatment pointer, no new advice/attribution. |
| FR-12 | PASS | NOT INSPECTED | No currency/money in mapper/card; existing financial column untouched. |
| FR-13 | PASS (harness code) | NOT INSPECTED | Actual Year4→6 parity harness uses raw, unnormalised `Dump` card rows; reported comparison result not witnessed. |
| FR-14 | PASS (source) | NOT INSPECTED | Guarded `Refresh` returns before history read; one Build on rebuild, no new manager/event/save writes; `openDetails` remains UI-local. |

| Acceptance | G2 source/harness verdict | G3 direct verdict | Evidence, severity and qualification |
|---|---|---|---|
| AC-1 | PASS | NOT INSPECTED | No completed year returns before builder; worker says real fresh world tested. |
| AC-2 | PASS | NOT INSPECTED | One latest hint; original ACK footer/callback intact; 720/1080 native claims not witnessed. |
| AC-3 | **FAIL in provenance presentation B-1; otherwise PASS** | NOT INSPECTED | Five valid difference computations and elapsed years; mixed snapshot numbers receive only a DERIVED row chip. |
| AC-4 | PASS | NOT INSPECTED | Source checks all missing/ambiguous states; reported 132 controls and rendered negative images remain worker-reported. |
| AC-5 | PASS | NOT INSPECTED | Ambiguous/unresolved disjoint warning and event-only handling source-inspected. |
| AC-6 | **FAIL in comparison row provenance B-1; otherwise PASS** | NOT INSPECTED | Species measures and one-sided absence present, but same combined mixed-value provenance display. |
| AC-7 | PASS (harness design) | NOT INSPECTED | 52k-character exact renamed pre-feature view, two paths compare `Canon` and current-world hash; native input reported separately. |
| AC-8 | PASS (harness design) | NOT INSPECTED | Authentic Year4 fixture created through production APIs; raw `manualCards == skipCards`, not semantic-normalised card data. |
| AC-9 | PASS (oracle design) | NOT INSPECTED | Code asserts Screen.width/height, scroll membership, button/footer bounds, wheel and controls; PNG dimensions/readability/local screenshots cannot be directly vouched for. |
| AC-10 | PASS (archive oracle design) | NOT INSPECTED | Worker-reported genuine host v9/Legacy40 and frozen v12 anchors; raw save and logs not possessed by browser reviewer. |
| AC-11 | PASS | NOT INSPECTED | Source forbids money/causal/survival/area/interval claims, distinguishes modelled regeneration map squares. |
| AC-12 | PASS (source cross-check) | NOT INSPECTED | Work column's four shared succeeded categories count `year.ResolvedWork` events, as harness compares. Regeneration removals instead use report field; mismatch is intentionally disclosed, not forced equal. |

## 4. Four stopped runs, corrected oracles, and final result

**G2 DIRECT:** inspected the actual historical runner commits and diffs in sequence:

| Run | Runner | Published result (G3 worker report) | Independent G2 judgement of subsequent change |
|---|---|---|---|
| 1 | `b0775321e1494c60449bbdc3200db9b9cce8ea10` | **FAIL / exit 1** after prior named cases | Initial equality to UI Toolkit **logical panel** dimensions was the wrong physical-resolution oracle. |
| 2 | `7b8ca89276f901a47045f49125c1fdfdbbcd66ef` | **FAIL / exit 1** | Changed to `Screen.width/height` and native Year3 path; next issue was too-strict `parent == contentContainer`. |
| 3 | `ada4a0dd84e26b02305528ce3799b1d17badaee6` | **FAIL / exit 1** | Added attachment diagnostics, retained strict parent assertion; the stale-reference hypothesis was not proved. |
| 4 | `e6d80838acc2536b9b31af0469b8444ff8ebabd5` | **FAIL / exit 1** | New `GetFirstAncestorOfType<ScrollView>() == originalScroll && contentContainer.Contains(card)` accurately tests FR-1. Loop then assumed latest selection is a numeric year. |
| 5 | `0d624c61bb8852850a07b4b33d83e740a032c86c` | **PASS / exit 0; 99 checks, 18 PNGs** | Loop now stops at the existing `viewYear == -1` follow-latest sentinel and requires Later disabled; bounded by history count. |

The final scroll test is neither a permissive screen-wide presence test nor a waiver of scroll containment: it retains the specific original ScrollView identity plus content membership. The final resolution oracle checks `Screen.width/height`; the report's **independent PNG pixel audit** was local and is **NOT DIRECTLY INSPECTED** here. The sentinel correction aligns to source `StepYear`, not a change to desired navigation. All four failures remain failures; do not retroactively count them as completed suite passes. All three corrections are legitimate *as code*, with **no detected essential acceptance relaxation**. Their alleged G3 execution and final 99/18 tally remain worker-reported.

The source-described OS path requests xdotool pointer motion/clicks, wheel clicks and Escape through `d073_native_input.py` on a private Xephyr display; the receiver checks a `done-####` receipt. Some AC-7 A/B baseline tests intentionally invoke Unity `Clickable.Invoke` programmatically; those must **not** be misrepresented as OS-native tests. The separately named AC-9 native path does use the OS driver, with claimed state changes after the actions. The original Unity SearchDatabase startup exception is **reported as pre-existing**, not called a clean Console nor a D-073 gameplay crash; zero compiler error lines is a different, narrower statement.

## 5. Historical honesty, compatibility and other boundaries

- **Authentic versus synthetic:** The test source builds a Year4 teaching/management fixture via real Rack V2 geometry, field marking, Work Plan execution, release, inspection, nursery purchases, planting and annual steps. It does not set a fake stage index. The AC-4/AC-5 negative cases intentionally mutate copied in-memory records; they are **synthetic negative controls, not genuine old-save proof**. This design is independently visible; exact runtime outputs are not.
- **Manual/skip parity:** The report honestly records different raw world hashes (manual `6DF6B75E8B957F8C`, skip `8B0BDF75F9BC3816`) and one extra manual-path Year5 ACK (538 versus 537 events). Source test compares **unaltered card dumps directly**. The separate worker-reported semantic normalization removed only ACK entries and renumbered event IDs to compare non-ACK fields. It is **not raw-world equivalence**, and the report does not claim it is.
- **Old save:** Worker reports a real host file `forest-save.json.bak` v9/year39, SHA-256 `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3`, 361 tree IDs/positions preserved and Legacy40 retained. The verifier actually reads `CCF_D073_OLD_SAVE`, tests version/year and loads; it does **not** fake the version. **The file bytes and full tree-field audit are worker-reported, not browser-inspected.**
- **Reference:** The test verifier decompresses the raw reference archive and compares embedded milestone world hashes at Years 0/20/50/100 and schedule to pinned values, with in-memory tamper negative control. Worker reports actual four-year preview/return passes, not a re-generated archive or model-0 rebaseline. **Raw archive and local output not independently inspected.**
- **Money and coverage:** Event counts are not tree-identity confirmations; the new card shows no grouped harvest-money distribution. Existing Work column's `regenerationRemovalTasks` is a report field, not necessarily equal to event-resolved removal jobs. [#418](https://github.com/jermur788/CCF/issues/418) separately records a **static, not runtime-proven** issue: an MFM `WorkResolved` event may omit nonzero `prescriptionId` while the retained order has one. The helper's `ValidOrder` then rejects verified order linkage. D-073's event-only **NOT RECORDED** warning remains conservative and should not claim executed coverage; `ExecutedCoverage` and `PossibleTreatmentYears` are not displayed. #418 must remain a separate gate.
- **Performance and preservation:** single guarded Build on source; no new schema/event/ACK writes. Worker reports 5 normal Unity lease releases and exact hash restoration; **physical board, Editor process, cleanup and custody not independently witnessed**, so these cannot be upgraded to local G3 direct proof.

## 6. Smallest safe follow-up and Manager decision request

**Required for B-1:** Manager decide a **tiny one-file UI provenance fix** (preferred), with one focused rendered-provenance assertion and proportional compile/720p/1080p witness on the *new* candidate, **or** intentionally approve and document a changed FR-10 presentation contract. No blanket 143-case cadence rerun or Tier-3 regression requested. The exact user-facing arithmetic and ecology must not be changed to satisfy a cosmetic label.

**Optional G3 evidence-integrity witness, if Manager requires independent local corroboration before integration:** One authorised read-only Predator witness **without Unity, Editor lease, test rerun, production edit, or machine cleanup**. Inspect existing `/media/jer/Files/CCF/Verification/D073-YearComparison-417-20261112/`: (1) runtime1–5 result JSON and final log exit/token lines, including all four exit1 outcomes and the Search exception; (2) `runtime5/evidence` 18 PNG files plus screenshot-audit dimensions/hashes, particularly an earlier-year caveat/foldout/footer pair at both resolutions; (3) `runtime5/driver/native-actions.jsonl` with before/after private-desktop receipts for real pointer/wheel/Escape; (4) genuine-v9 audit and frozen-reference/hash receipts, and 5 release/end-state manifests. Publish a short GitHub Markdown witness with exact artifact path/digests, independent PASS/FAIL/NOT INSPECTED per observed item. **Do not upload private save bytes** or claim an unperformed Unity run. This would attest the original #417 candidate only; any B-1 source fix requires its own proportionate UI requalification.

**Manager return:** **NEEDS CHANGES — B-1 Major**, G2 source conformance; #417 final Unity result remains **WORKER-REPORTED PASS, raw G3 NOT INSPECTED**. No integration, main/#382 modification, Reference Future change, or Windows build. Parent #412 / Overall Manager should record the disposition and choose the bounded correction/witness; this independent reviewer has no authority to integrate.

**Dashboard sync:** NOT VERIFIED. **Drive mirrors:** NOT UPDATED / NOT VERIFIED. **Physical Ops board and Unity lock:** NOT INSPECTED / NOT CHANGED. **User action required:** none from this reviewer; Manager follow-up needed.
