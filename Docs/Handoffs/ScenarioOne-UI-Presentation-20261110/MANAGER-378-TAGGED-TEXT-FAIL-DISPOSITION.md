# Manager #200 — #378 Annual Review source-tagged Work/Forest text readability verdict

**Date:** 10 November 2026. **VERDICT: NEEDS CHANGES / NOT INTEGRATED OR USER ACCEPTED.** One previously failing Money layout is now genuinely qualified at two resolutions; source-labelled non-money lines are not. This is a bounded UI readability defect, not a change in simulation, source authority or historical year data.

## Evidence assessed

- [Sol #378 report, six actual Unity PNGs and measured label rectangles](https://github.com/jermur788/CCF/blob/74a7d89e/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-378-ANNUAL-REVIEW-MONEY-ROW-REPAIR-AND-UNITY-RESULT.md).
- Exact Unity-tested game candidate: `ebe061bae8d3f9da1e810b4c49f49199db8b3d61` on the #378 repair branch; Unity 6000.6.0f1, actual 1280×720 and 1920×1080 with clean separate source/machine lanes.
- GitHub commit comparison confirms one commit changing only `AnnualReviewView.cs`, annual-only USS additions, and additive `annual_rows.py`. Original Money rows were modified locally to stack provenance, names and right-aligned signed amounts; `UiKit.Line`, `Tag`, `AddTagged`, `BuildHeadlines`, underlying financial values and source types remained unchanged.
- Manager independently fetched actual repaired `AnnualReviewView.AddTagged` and `TaggedLine`, plus `UiKit` and USS. `AddTagged` still puts the always-visible source qualifier, optional provenance chip and a zero-basis elastic text label side by side in a `row`; three equal Annual Review columns restrict horizontal space. That matches the reported UI symptom. **This is a source-backed diagnosis, not a claim that the Manager personally decoded the PNG pixels**; the linked operator captures and resolved UI bounds constitute the supplied render evidence.
- Source claims: 189 runtime sources compile with zero reported errors and warnings; focused 160, 219, 18, 157 checks and existing negatives pass; new 30-case Money row consumer and a compiled negative control pass. Manager did not independently rerun local Editor/compilers.

## Passed vs failed vs incomplete (retain source evidence)

**Genuine partial pass at both resolutions:** four Money headline names, provenance/qualifier and positive/negative amounts now readable with right-aligned signed subtotal (prior label ~34 logical px; repaired ~331/328 px), underlying annual money authoritative. Earlier Year 2 selected while top status correctly says current Year 3; source toggle keeps selection and shows chips; earlier-year ACK hidden and latest still unread. #377 Crop Tree overview/pagination passed earlier on byte-identical source (not redone and not claimed as a fresh #378 render). A bounded latest unread Back/ACK flow passed at 1080 and Work Plan approval separately from year advance at 720.

**Confirmed player-facing failure at both sizes:** with `Show data sources` enabled, Forest Changes `Standing trees 1195 → 1195` text measured ~53.75×131.25 at 720 and 67.5×90 at 1080, splitting a word into fragments. Expanded Work/Forest recorded rows also show narrow text: `Harvest job ... settled together as one job` ~45×323.75, `Areas with young regeneration ...` ~43.75×323.75 at 720. This is **not normal compact line wrapping**; the source-labelled text width is inadequate.

**Still NOT RUN/PARTIAL:** pointer-native full acceptance; earlier Objective 1 locked Planting/Pruning/Nursery note+pointer checks; actual MFM candidate native C/N from valid fixture; held Shift+V reverse; one 720p detail page; Legacy40/frozen Reference running compatibility; full grayscale and scene/world rank overlaps; full F5/F9 roundtrip in current candidate. None may be invented as PASS from prior source tests.

**Safety:** Sol reports new-process verified isolated save/F5/F9/Prefs destinations, 36 valued user file hashes equal pre/post, 27 licensed + six protected assets match, separate U3/dirty Workbench unchanged, correct mounted ZX20 and free space, exclusive Unity lease released and Editor exited normally. Previous successful lease does not grant a future lease; re-preflight. [#378](https://github.com/jermur788/CCF/issues/378) is CLOSED/HANDED OFF, source+Unity ownership released.

## Manager decision and efficiency rule

**NEEDS CHANGES** for current UI. **Retain Money and Crop inspector passes**. Do not redo the full feature from scratch, commission another Fable review (explicitly user-cancelled), or perform repeated single-label patches.

Create [#379](https://github.com/jermur788/CCF/issues/379): one Sol source repair of the **shared Annual Review non-money `AddTagged` presentation**, then **one** conditional Editor continuation. Improve the source-labelled row component wherever Annual Review uses it (headlines, expanded Work/Forest, details, teaching/storm/uncertainty where present), without changing the underlying history/data/provenance. A source qualifier and badge must have their own properly sized row; the full text should use the card width *independently* of qualifier/badge. It is acceptable for the cards to grow vertically/scroll rather than force unreadably narrow body widths.

**Before Unity** the worker must inspect every call site of `AddTagged` and source-labelled text in Annual Review, add a positive and negative consumer-level test of shared row composition, and confirm no row uses the old side-by-side width squeeze. This is not proof of pixel fit. Under a freshly admitted exclusive Unity lease, the **first render gate tests all affected Work/Money/Forest row types and expanded records, with source toggle ON and OFF, at both 720p and 1080p**, including longest examples; collect actual PNGs and UI bounds for each visible label. STOP if any unreadable fragmentation, preserve failure proof. If first gate PASS, finish only outstanding unqualified states in same run when safe and relevant. Never rerun #377's full 17-page crop fit sweep.

**No change to** actual annual work/financial values, source attribution, selection of historical year, ACK/Back semantics, approval/year advance, general UiKit layout, ecology/economy/save/rack, old/reference world or previous test assertions; avoid hiding chip or qualifier to buy width.

- **Claim:** #379 READY / NOT RUNNING.
- **New prepared source branch:** `fix/scenario-one-ui-annual-tagged-text-20261110-v1`, cut from exactly the #378 tested source.
- **Proposed isolated Files writer worktree:** `/media/jer/Files/CCF/Operations/CCF-S1-UI-TaggedText-Repair-Sol`, not created or claimed.
- **Existing separate warmed Unity lane:** `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual`, usable only with current source, user Editor release, correct mount/space, safe saves/Prefs, protected assets and registered exclusive lock re-established. No current authority inferred.
- **[Single binding Sol packet](MANAGER-378-TO-379-ANNUAL-TAGGED-TEXT-FIT-AND-SAFE-UNITY-CONTINUATION.md)** and required output `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-379-ANNUAL-TAGGED-TEXT-FIT-AND-UNITY-RESULT.md`.

**Final current integration verdict: NEEDS CHANGES — DO NOT INTEGRATE, NO WINDOWS BUILD.** User #361 personal playtest still open/unaccepted. No second Fable review.

**Dashboard:** GitHub canonical record to be reconciled by Manager; downstream dashboard ingestion/readback remains **NOT VERIFIED**.
