# CCF player-facing text inventory and content architecture — 13 November 2026

**Research status:** SOURCE-SAMPLED / PARTIALLY EXTRACTED, supporting design evidence only. **Not a complete player-facing string census, accepted product copy, Unity qualification or authority to edit game code.** Dedicated worker [#473](https://github.com/jermur788/CCF/issues/473); owner for later Scenario One integration is [#424](https://github.com/jermur788/CCF/issues/424).

## Authority and inspected baselines

- **Read-only main baseline:** `fc18190b976f1363f1b5d1b0881810db2f8c6e39` (live main when this task began 13 November). GitHub `main` is *checked-in source*, not proof of the currently user-launched or built Unity candidate.
- **Separate, unmerged candidate:** `feature/scenario-one-objective1-ux-repair-20261112-v1`, owned by #443/#450/Manager #424 for Objective 1. Only representative changed copy was compared; candidate copy is tagged `unmerged-candidate` and is **not** silently promoted into the audited main catalogue or verified as current in-game presentation.
- **Other accepted direction:** Decision Log D-064 Rack V2, D-065 multi-scale treatment, D-071 existing parallel five-rack layout, D-072 honest headland/map presentation, D-078–D-081 eight independent forestry scenarios and saved identity boundaries. DRAFT compact Scenario One and subsequent scenarios require their own product decisions and source; neither is a current world in the catalogue.
- **Protected:** Reference Future v1; original/Legacy40 save compatibility; user-playtest source and machine leases; objective/lesson IDs, `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, save formats, Work Plan and rack/tree identities. This work made **no edits** to any of these.

Authoritative planning sources consulted from the live repository: `AGENTS.md`, `Docs/Project/AgentWorkflow.md`, `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`, `unity-project-overview.md`, live `coordination/active-tasks.md`, the #473 packet, and related #424/#443/#450/#425/#471 claims.

## Quantitative inventory — denominator honesty

| Measure | Verified research result | What it is not |
|---|---:|---|
| Main C# files read and blob-anchored in `Tools/ContentReview/source-map.json` | **28** | All files that may render game text |
| Quoted-literal lexical candidates in those 28 files | **2,934** | 2,934 player-visible strings: this includes style selectors, keys and internal identifiers |
| Starts of interpolated string expressions in those files | **225** | All generated/conditional texts |
| Navigable pages in prototype catalogue | **79** | Entire game screens or exact Unity paginated layout |
| Field records in versioned `catalog.json` | **268** | Exhaustive Unity text coverage |
| Audited checked-in main-source fields | **265** | Guaranteed shipped/build-time displayed strings |
| Explicitly unmerged #443 candidate fields | **3** | Automatically accepted copy |
| Current source-extracted literal-like fields | **235** | Independently rendered/validated |
| Conditional fields | **28** | All actual branches |
| Illustrative generated templates | **5** | Validated runtime formatters |
| Future scenario / compact draft authored pages | **0** | Evidence that these stories or worlds exist |

**Unknown coverage is genuinely unknown**, not zero. Serialized, unlocated, explicitly excluded and manually deferred strings have not been completely enumerated, so the source-manifest coverage-category counters for those classes are placeholders and **must not be interpreted as no missing text**. Coverage ratio against all displayed text **NOT MEASURED**; no claim of 100% or of all panels scanned.

The 79 catalogue pages comprise three real introduction pages; nine Objectives topics; **48** saved Objectives steps; **eight** ScenarioOneTeachingSequence lessons; seven F1/help page contexts; 19 controls rows grouped into one page; and representative Work Plan, Annual Review and candidate pages. The 268 records include per-field `source.path`, `source.branch`, line where recorded, status, order, variant ID, stable *content* ID, original/current text, immutable step/lesson relationship if applicable, and named token inventory. Source metadata describes the source baseline, not exact first-person runtime display.

## Surface and provenance map

| User-facing group | Source(s), inspected at main | Extraction and remaining risks |
|---|---|---|
| Three-page introduction | `Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs` (Titles, Leads, Footers, Principles, Process*, InitialState, TargetState, KeyIdea); `ScenarioOneOnboardingView.cs`; `specs/scenario-one-three-page-onboarding-v1/production-copy.md` | Source-extracted all three core pages and most text arrays. The view additionally embeds eyebrows, action buttons, diagrams, `REPEATED SELECTIVE MANAGEMENT`, and a **render-time line break insertion** via `lead.Replace("managed. Instead","managed.\\nInstead")`; final pixel spacing is not encoded by the catalogue. Source copy is locked by the existing approved copy document; prototype edits are not approved replacements. |
| Objectives topics / saved steps | `LearningObjectivesView.Curriculum.cs`, `LearningObjectivesView.cs`, `LearningObjectivesView.Sequence.cs` | Nine topics and 48 step IDs extracted. `rack.read`, `rack.create`, `rack.layout`, `rack.approve`, `rack.map` have legacy/general versus Rack V2 overrides. `PreferencePrefix = "CCF.Learning.v1."` and every saved step ID remain read-only. Derived condition text, progress indicators, layout control and completion predicates remain C#. |
| Guided teaching and Objective HUD | `ScenarioOneTeachingSequence.cs`, `ScenarioOneRackBayText.cs`, `ActiveObjectiveHudWidget.cs`, `ScenarioOneObjectives.cs`, `ScenarioOneUiRoot.cs` | Eight lesson `Id/Title/Concept/Action/Caption/HudAction/Practice` sets extracted. Rack-bay instruction/HUD/progress are dynamic, depending on sample, annual work, count/share and stage: **manual adapter/deferred**, not globally mutable prose. The Objectives labels and prompt boxes are not fully enumerated. |
| Help and first use | `MenuHelpView.Text.cs`, `MenuHelpView.cs`, `ControlsReference.cs` | Extracted title/summary/F1 text contexts, Rack V2 explanation, tree-mark help and 19 controls key/description pairs; walking HUD F1 default includes explicit `{{keysLine}}` insertion anchor. First-use seen flags use local `CCF.MenuHelp.v1.` preference identities; text edits must not alter unlock conditions. `ControlsReference.RowsFor(rackPlanningV2)` swaps B/M/P content according to world profile. |
| Work Plan | `WorkPlanView.cs`, `WorkPlanOverview.cs`, `ScenarioOneTreatmentReview.cs`, `ScenarioOneWorkPlanFocus.cs` | Only representative summary title, calculated status and owner-time templates in prototype. Missing: task-specific quotes, outcome choices, error/warning prompts, treatment proposal/confirmation, affordability, buttons, executor and pending/approved/failure states. Game-provided numbers, money rounding, task legitimacy and approval semantics must remain authoritative. |
| Annual outcomes and management history | `AnnualReviewView.cs`, `ManagementHistoryText.cs`, `ScenarioOneCompletionGuidance.cs` | Template examples for current/historical year headings, no-review prompt and three columns included. Most provenance labels, comparison math, dynamic forest/history rows, failure and storm records, historical stage differences, skip summaries and completion conditions remain outside the extracted seed. Keep `RECORDED/CURRENT/DERIVED/RECONSTRUCTED/SESSION-ONLY` epistemic distinctions. |
| Map / planning / tree inspection | `StandMapView.cs`, `StandMapTreatmentReview.cs`, `TreeInspectionView.cs`, `WalkingHudView.cs`, `HudMinimapView.cs`, `TeachingDiagram.cs` | Inspected source surfaces and lexical candidates, but **not** comprehensively exported. Spatial/rack/tree associations, units, sample vs proposed treatment, dynamic selection, old-world guards, focus, diagram and map labels require a dedicated catalogue/adapter phase. |
| Startup/session and common UI | `StandaloneSessionMenu.Startup.cs`, `StandaloneSessionMenu.cs`, `UiKit.cs` | Inspected. Real title `CCF`, `Scenario One`, `Sitka Plantation to Continuous-Cover Forest`, keyboard and session choices require further extraction and native-context tests. |
| Serialized/visual-only copy | Relevant `Assets/Scenes/*.unity`, prefabs/ScriptableObjects/Resources/UXML/USS and onboarding artwork | **NOT COMPLETELY AUDITED.** `ScenarioOneUi.uss` is styling; authored lettering baked in images is not editable from JSON. Before total coverage claims, scan serialized text fields and visibly separate text-as-art replacement needs. |

Some classes beyond this 28-file read set can emit player-facing text (other `Assets/ForestPrototype` C#, scenario definitions, ecology notices, work-order validation, Unity UI assets). Search of filenames and repository tree discovered many source surfaces; a reliable all-assets lexical census and runtime witness were not performed. A future audit must explicitly classify each remaining source string as **collected / conditional / generated / serialized / unlocated / out of scope**, record why and retain true unknown counts.

## Representative exact source/profiling evidence

- **Onboarding:** page-1 heading `What is Continuous-Cover Forestry?` and page-3 `Scenario One — Transforming a Sitka Plantation` originate in `ScenarioOneOnboardingCopy.Titles`; the lead is separate; continuation text appears again in the live `ScenarioOneOnboardingView`.
- **Saved objectives:** `rack.create` is an immutable Step ID. Main base text begins `Press B in the forest.` but Enlarged80 Rack V2 override reads `Open the Stand Map [M] and choose Plan Racks.` The same save progress key must continue to resolve both world profiles.
- **Rack V2/Legacy40:** `ControlsReference.RowsFor(bool rackPlanningV2)` uses P for the current rack spray tool and hides B from the Rack V2 control list; legacy B stays for earlier model/world compatibility. Merely replacing `"Press B"` everywhere would teach the wrong workflow.
- **Candidate-only #443:** Objective 1 amended text refers to the `first designated rack-removal tree at the headland-side entrance`, and the candidate changed certain Stand Map/help wordings. These examples are **separate labelled records**, not main or merged copy. The source candidate may evolve; future integration must refresh before choosing canonical wording.
- **Generated messages:** `WorkPlanView.Refresh` assembles current year, task counts, euros and owner time; `AnnualReviewView.Refresh` selects current, earlier-year and empty state headings. `{{year}}`, `{{latestYear}}`, `{{openCount}}` etc. are *proposed integration-time named placeholders*, not placeholders already used in C#.
- **Teaching states:** `ScenarioOneRackBayText.Instruction/Hud/Progress` synthesise fractions, percentages and near/middle/far / completed-state wording. Preserve genuine game facts and condition predicates; do not turn these calculations into editable claims.
- **Data authority:** saved learning IDs, rack IDs, tree marks, management event identity, `PlayerPrefs` and frozen Reference data are not language strings. No prototype field modifies them.

## Alternatives and design recommendation

| Option | Strength | Limitation / decision |
|---|---|---|
| **Versioned static JSON catalogue + typed Unity read-only adapter** | Portable, diffable, explicit source provenance, no licence/server; supports standalone file picker + export; independent scenario pages | Requires a **one-time approved adapter** at each selected C# call site; must validate tokens and fall back safely. **Recommended.** |
| ScriptableObject per page/field | Familiar Unity authoring, typed serialization | Requires Unity to edit; GUID/serialized asset conflicts and harder external browser roundtrip. Not suitable for owner's outside-Unity workflow. |
| Native Unity Localization Tables | Engine tooling, plural/locale framework, potential eventual translations | More packages/build/editor integration and service complexity than required for editable English source; would still need external authoring import. Consider later if multilingual scope is approved. |
| Direct C# find/replace or JSON-to-C# text patcher | Easy seeming migration | **Rejected.** Destroys provenance, risks saved IDs, escaped/interpolated/conditional text and source conflicts with #443; no safe automatic equivalence. |
| Local server/database/cloud CMS | Could manage approvals/sync | Adds process, credentials, security and maintenance for no current V1 benefit. Use direct browser import/download and Git reviews. |

**Selected design for DRAFT review:** `ccf.content.v1` array-based, versioned portable JSON, stable non-gameplay content IDs, explicit scenario + page + display-field + condition variant, `baselineText`, mutable `text`, source provenance, named token signatures and no approval at export. Prototype uses plain JS/CSS/HTML and bundled `catalog.js` to support `file://`. CCF remains source-of-truth for conditions, computations, game state, save progress and dynamic formatters. Later adapter should read only approved content from a typed build-time Unity `TextAsset` or equivalent after an explicit Manager gate; absent/malformed fields fall back to original literals and log diagnostics without writing saves.

**Revision/conflict policy proposal:** immutable content identity and provenance per audited baseline; successive exports bump draft revision, never silently append unknown content IDs or mark approved. Import accepts only compatible baselines; otherwise source refresh and three-way review are needed. Git tracks review decisions. `draft-local-export` is intentionally not an approval stamp. Do not open the exported file as HTML.

**Encoding/rendering:** UTF-8 JSON, Unicode/em dash/arrow/newlines/backslashes and apostrophes preserved; named token *order and multiplicity* validated; no HTML/rich text in V1. The browser inserts draft copy with `textContent` / textarea values, not `innerHTML`. Unity later receives plain text and its own safe typed formatting; no arbitrary format expressions or C# code injection. Currency/number/unit/plural rounding stays owned by the existing simulation/locale functions, so an unreviewed edit cannot change `m²/ha` meaning or money calculations. Placeholder text is a hint, **not** a gameplay value.

## Eight-scenario and profile separation

Confirmed chapter **teaching order** is: **O1 Inherited Sitka Plantation → O2 Working Mixed Forest → O4 New Farm Woodland → O3 Neglected Broadleaf Wood → O7 Exposed Upland Forest → O6 Atlantic Oakwood → O5 Wet Woodland and River Valley → O8 Complete Irish Forest Estate**. The catalogue has eight corresponding independent scenario navigation entries (plus a ninth, explicitly empty compact-O1 draft). Only O1's inspected main baseline has page data. Original case IDs O1–O8 and separate world/save identity must never be inferred from the navigation position. Current five-parallel-rack O1 and any compact O1 proposal remain **distinct world/profile text** until product authority reconciles them.

## Coverage expansion and future integration audit

1. Re-run `node Tools/ContentReview/source-audit.mjs . --json` from a *full read-only checkout* after every candidate change. It checks the 28 known C# source Git blobs, reports changed/missing files and new/unindexed sources, and lists likely text fields in scenes, prefabs and assets. Compare with `source-map.json`. **Command not run in this GitHub-only worker.**
2. Extract candidate new or changed text with exact file/line, source world profile, condition, display field, dynamic tokens, source/reference and protected identity; reconcile #443/#450 first. Do not mechanically sweep double-quoted literals into player-visible content.
3. Exercise runtime states after the adapter exists: current O1 Enlarged80, legacy B/Legacy40, frozen Reference, intro/first-use, each Objective step, pending/approved/completed plan and no-work year, earlier/history year, completed scenario, saved/loaded world. Record a provenance-backed screen coverage matrix, not inferred string counts.
4. Qualify `1280×720` and `1920×1080` text wrapping and contrast in Unity, including long Unicode, keycaps, dynamic amounts/units, competing overlays, old-world/profile guards, and untouched progression. No Windows build without user approval.
5. Cross-scenario content must be authored *after* its own approved spec; adapters use stable per-scenario IDs and must never route a foreign save or copied lesson key to another forest.

## Unresolved decisions / constraints for #424 and #425

- Which already reviewed #443 wording is canonical after its independent status and eventual integration? **UNDECIDED in this worker.**
- How will a future compact O1 profile's label and rack count be authored without copying stale five-rack instructions? **No shipped compact content exists; this worker does not decide.**
- Exact adapter file batch, whether resource import occurs build-time only and the full screen census: **proposed**, requires separate approved scope/ownership.
- Human authoring approval and copy variants for the eight-case curriculum: **not granted** by research, prototype or the owner's request to make an editor.
- Native/serialized source completeness: **not established**, further audit and Unity witness required.

**Overall finding:** a usable external authoring interface can exist now without changing the game, but **a catalogue export is not a safe live Unity text update** until source coverage, identity-preserving adapters and runtime profile verification are separately approved and qualified.
