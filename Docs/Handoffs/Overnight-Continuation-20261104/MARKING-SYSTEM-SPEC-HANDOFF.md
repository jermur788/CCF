# #259 — Marking System Research and Draft Spec Handoff

**Output status: DRAFT. Docs only; no implementation, independent approval or integration.**

- **Task:** [#259](https://github.com/jermur788/CCF/issues/259), under overnight #254.
- **Worker / role:** OpenAI Codex, `gpt-6.1-sol`, medium; research + DRAFT-spec writer.
- **Routing provenance:** **EVENT 007**, assigned pool **`codex_c75d8114`** (dispatch evidence supplied by parent; see appendix).
- **Assigned and used worktree:** `/media/jer/Files/CCF/Operations/CCF-Overnight-MarkSpec-20261104`.
- **Branch:** `docs/overnight-mark-language-spec-20261104`.
- **Base, verified clean at start:** `8f0f230190830855112e3e7a1d7544db94a7f591`.
- **Context commit:** `9067554795b662966e243e09bb724aca9bd15649`, latest context-set change in base ancestry (including instruction sources/generator).
- **Inspected candidate:** `72b4efbd1c653fa2231f632a79030a83ff76f833`; source read through `git show 72b4efbd:PATH`, no checkout of candidate or protected integration branch.
- **Git common store verified:** `/media/jer/Files/CCF/Repository.git`.
- **GitHub handoff path:** this tracked file; publication commit is the PR head containing this path (use PR Commits / `git log -1 --format=%H -- <path>`; no self-referential SHA invented).
- **Date / source access:** 2026-11-04 UTC.

## Result and smallest change

Created exactly four DRAFT artifacts in [`specs/scenario-one-tree-mark-language-v1/`](../../../specs/scenario-one-tree-mark-language-v1/spec.md) and this handoff. Structure follows `specs/README.md`, `_template/` and the existing access-spine four-file draft. No source, serialized Unity asset, `.meta`, package, setting, save, reference, active task record or routing ledger was edited. No asset deleted. No other lane read/modified. Initial worktree had no unrelated changes.

Read root AGENTS, canonical workflow/decisions/milestone/brief/overview/research context, authoritative live GitHub coordination and the overnight master packet. The live task record reserves separate scopes for #255–#260, with #200 sole coordinator; the direct dispatch narrows this worker to its five paths. Current Milestone/Overview contain dated earlier states; pinned code controls implementation facts, including save **v24**, not the older v20 Overview summary. D-013 and approved Rack V2 §§25–28 retain red-Fell teaching authority until explicit amendment; this draft does not amend them.

## Current visual truth — source evidence at the exact candidate

All references in C1–C11 below are **at `72b4efbd1c653fa2231f632a79030a83ff76f833`**. They are file:line citations from numbered `git show` output. They establish what code/materials request, not what a running Editor/player actually displayed. Runtime/render/input findings remain **UNVERIFIED**.

| ID | Claim / evidence | Source at candidate | Confidence / boundary |
|---|---|---|---|
| C1 | Rack paint is four curved **vertical orange trunk patches**, at four quarter-turn sides, bottom 1.0 m and top `min(1.85, height×0.75)`. Colour `(0.95, 0.28, 0.025)` uses URP Unlit. | `Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs:183–225` | High, direct geometry/material code. Actual visibility, clipping and hue UNVERIFIED. |
| C2 | Loose Fell and Crop both receive the same above-crown rotated cube (“Diamond”, 0.45 m scale, `height+0.6`) and ground cylinder (“Disc”, 1.2 m diameter scale). Fell has **no diagonal trunk slashes** in `CreateMarker`. | `Assets/ForestPrototype/ForestTreeMarkingManager.cs:580–623` | High, direct constructor. Not an observed silhouette or visibility test. |
| C3 | “Red Fell” is **not a uniform source-level colour truth**: 3D Fell default and assigned HarvestMark material are orange-toned `(1, .35, .05)`. Both ForestTest and MixedSpeciesTest assign the same material GUID; the material branch uses it as-is. | `ForestTreeMarkingManager.cs:11–12,639–664`; `Assets/ForestPrototype/Materials/HarvestMark.mat:67–69`, `.meta:2`; `Assets/Scenes/ForestTest.unity:5911–5913`; `Assets/Scenes/MixedSpeciesTest.unity:5911–5913` | High for stored RGB and material wiring. “Orange-toned” describes values, not calibrated screen colour. These manager paths are under `Assets/ForestPrototype/`. No red override found in inspected paths; no claim about arbitrary uninspected runtime modification. |
| C4 | Crop additionally gets **Blue Trunk Band**, a cylinder around the trunk at `min(1.6, height×0.5)` with radius scale derived from DBH; blue `(0.15, .4, .95)`. Thus a blue ring/band is already represented, along with the extra diamond/disc. | `Assets/ForestPrototype/ForestTreeMarkingManager.cs:12,608–619,645–648`; scene colour assignments as C3 | High source confidence; continuous visible coverage on every tree mesh UNVERIFIED. |
| C5 | Full-map Fell/Crop marks are **red / blue rings**, not slashes or the curriculum's squares/diamonds. Only living marked trees in the Marks overlay get rings; size follows crown radius with minimum 6 px radius. | `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:166–175`; `StandMapSymbols.cs:106–107`; `StandMapLegend.cs:108–118` (latter two in same `UI/Map/`) | High, direct draw/style/legend code. Rendered clarity UNVERIFIED. |
| C6 | Minimap omits individual Fell/Crop glyphs. Their styles have `InMini=false`; draw code also restricts Marks overlay to full map. HUD uses shared `StandMapSurface(MapDetail.Mini)`. | `Assets/ForestPrototype/UI/Map/StandMapSymbols.cs:106–107,118–119`; `StandMapDrawPlan.cs:166–175`; `Assets/ForestPrototype/UI/HudMinimapView.cs:46–54,82–88` | High, direct filters/wiring; no screenshot/input proof. |
| C7 | Orange **painted entrance state is not separately drawn on either map**. Map tree mark enum is only None/Fell/Crop; source maps only those tree statuses. Rack map records carry geometry/locked/selected state, not `entranceMarked`. Selected full-map entrance is a **blue square**, not an orange field-paint icon. | `Assets/ForestPrototype/UI/Map/StandMapModel.cs:48–61,83–100`; `StandMapSource.cs:217–225,279–292`; `StandMapDrawPlan.cs:237–249`; `StandMapSymbols.cs:94` | High within inspected shared map path. A route/selection cue must not be described as proof that a tree has been sprayed. |
| C8 | Rack routes are related geometry cues, not paint: suggestions faint dashed, confirmed player racks amber solid, harvested/locked racks pale solid. Full and mini share source; mini hides unconfirmed draft player geometry. | `Assets/ForestPrototype/UI/Map/StandMapSymbols.cs:81–97`; `StandMapSource.cs:264–292`; `StandMapDrawPlan.cs:102–142`; `Assets/ForestPrototype/UI/HudMinimapView.cs:33` | High source confidence; actual route/world alignment UNVERIFIED here. |
| C9 | Help explicitly lists orange vertical rack-removal stripes, “Red — Fell”, “Blue — Crop Tree / future timber”; explains one entrance tree, four sides per spray, mutually exclusive marks and Work Plan approval. Curriculum line 56 instead describes Fell squares / Crop diamonds. | `Assets/ForestPrototype/UI/MenuHelpView.Text.cs:18–29,93–100`; `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:56,117` | High, direct text. Current page layout/swatches UNVERIFIED. #255 owns copy changes elsewhere; this task did not inspect or modify its lane. |
| C10 | Work-order cues are a separate transition risk: `PlannedFellMarkers` uses orange-toned FellColour, diamond+disc for non-rack open FellTree orders, violet double diamonds/wider disc for legacy rack groups. Rack V2 orders with `rackId>0` are explicitly skipped so automatic cues do not impersonate sprayed entrances. Comments call these “red”; actual constants control. | `Assets/ForestPrototype/Racks/PlannedFellMarkers.cs:5–18,65–74,107–119` | High source confidence; full pending/approved interaction timing and map representation after consumption UNVERIFIED. Do not use an old comment about session-only grouping as V2 persistence truth. |
| C11 | Saved meanings are separate from colours: None=0/Fell=1/CropTree=2; root lists marked/crop IDs; rack entrance mark derives from `entranceMarked` and `entranceTreeId` and is kept out of loose Fell ID set. Candidate schema is v24; legacy rack model remains 0. | `Assets/ForestPrototype/ForestTree.cs:18–23,75–99`; `ForestSaveData.cs:7,33–34,46–49`; `ForestTreeMarkingManager.RackEntrances.cs:33–40`; `ForestTreeMarkingManager.cs:502–522,553–575`; `RackPlanningModel.cs:1–11` (all under `Assets/ForestPrototype/`) | High source-level contract; no save/load round trip or hash measurement run here. |

**Plain conclusion:** the requested “orange diagonal slashes for Fell, blue ring for Crop” is not an accurate complete description of the current visuals. Blue band/ring already has partial support; diagonal Fell slashes do not. Red is the help/full-map convention while the inspected 3D material is orange-toned. Copy alone would conceal these differences. Do not claim all surfaces already share the requested shapes.

Browsable source example: [mark constructor at pinned candidate](https://github.com/jermur788/CCF/blob/72b4efbd1c653fa2231f632a79030a83ff76f833/Assets/ForestPrototype/ForestTreeMarkingManager.cs#L580). Reproduce any row with `git show 72b4efbd:PATH | nl -ba`; no branch switch is needed.

## Forestry evidence — primary sources and confidence by claim

Sources were retrieved directly on 4 November 2026. HTML was read as text plus links; PDF text was extracted in memory with Poppler `pdftotext -layout`. No source binary or image was added to Git or another lane. PDF visual artwork was **not** audited; no precise photographic hue/dimension is inferred. The PDF skill was consulted for the linked guide.

| Source ID | Source / provenance | Role / retrieval |
|---|---|---|
| P1 | Pro Silva Ireland, [Tree marking](https://www.prosilvaireland.com/about-ccf/tree-marking/), sections “Tree marking in CCF”, “Trees to be retained”, “Trees to be removed or modified”. | Direct publisher primary practice guidance. `www` page HTTP success; non-`www` homepage initially 403. HTML SHA-256 of retrieved page: `d137c7f55a7d44e3e6d111893676d8e04a6d0e285ee4009261b4569e22ebe4df` (dynamic page hash, not an edition identifier). Site's search API independently returned page ID 13308 and linked this page. |
| P2 | Pro Silva Ireland (2026), *A Quick Guide to Continuous Cover Forestry Practice in Ireland*, **06 — Tree selection and marking in CCF**, [publisher PDF](https://prosilvaireland.com/wp-content/uploads/2026/05/06-Tree-Selection-and-Marking-in-CCF.pdf). | Linked from P1. Printed pp. **10–11** are PDF page **6**; printed pp. **12–13** are PDF page **7** (two-page spreads). Publisher/2026/support attribution on closing page. 4,389,682 bytes; SHA-256 **`e3f363f4fdb77f87e89efc381b489bf183a834237c662a2ec17602b7e778a4ea`**. Successful with browser User-Agent; default-user-agent retry 403; browser-UA retry successfully reproduced relevant spread text. Poppler emitted `Syntax Warning: Invalid Font Weight`, not an extraction failure. No visual-layout claim made. |
| P3 | Pro Silva Europe, [Pro Silva Principles](https://www.prosilva.org/close-to-nature-forestry/pro-silva-principles/). | Direct organisational principles webpage retrieved. Supports broad natural-process/ecosystem/economic framing only; does not establish these Irish paint symbols as a universal Europe-wide system. Linked principles PDF not inspected. |
| R1 | [Canonical photographed-book reference](../../Research/CCFBook/CCF_Book_Definitive_Reference.md), marking sections; [earlier four-page extraction](../../Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md), §§A/E/F/H/I. | Repository-authorised normal reference, with known containing-book identity gaps and provisional photo audit. This task used committed text; did not re-open photographs or read external research lanes. P1/P2 now independently corroborate the relevant marking descriptions, without proving the unidentified book's bibliographic identity. |

| Claim ID | Claim supported / deliberately limited | Evidence | Confidence and bucket |
|---|---|---|---|
| F1 | Tree marking communicates remove/modify/retain decisions to harvesting crews; tree selection considers neighbours, stability, light and management aims. | P1 “Tree marking in CCF” / “Selective felling”; P2 printed pp. 2–3, 9. | High as documented practitioner guidance. **Sourced established practice**, not proof of optimal outcome in every stand. |
| F2 | Pro Silva Ireland describes its own system as standardised for Irish CCF; P2 heading is “Irish tree marking standard for CCF thinning operations”. P2 states eight categories, hot usually orange/pink and cool white/blue. | P1 “Tree marking in CCF”; P2 pp. 10–11. | High that publisher says this. **Sourced guidance**. No independent adoption survey, statutory/mandatory national status, standards-body accreditation, or universal Pro Silva/European mandate established. |
| F3 | Individual removal trees: diagonal slashes around stem at about 2 m, visible all sides; hot family **usually** orange/pink. | P1 “Trees selected for removal”; P2 p. 12. | High direct primary text. **Sourced practice**; source does not mandate the game's exact orange RGB, slash count or UI symbol. |
| F4 | Rack removal: coloured vertical stripes on four sides indicate proposed machine path; generally first thinning, also later network extension/adjustment. | P1 “Removal to accommodate harvesting and extraction”; P2 p. 12. | High direct primary text. **Sourced practice**; not proof that one entrance tree is the full operational marking requirement. |
| F5 | Retained high-quality trees: white/blue ring around stem at about 2 m, visible at distance, protect from damage, refresh in subsequent markings. | P1 “Retain on the basis of high timber quality”; P2 p. 10. | High direct primary text. **Sourced practice**; the game selecting blue or using lower paint height for short trees is calibration/abstraction, not a measured convention. |
| F6 | Retained trees demarcating permanent rack/headland routes have **two parallel horizontal** marks, facing route. This is a different status from vertically striped trees designated for rack removal. | P1 “Retain to demarcate permanent rack or headland routes”; P2 p. 11; R1 p. 87. | High direct primary text plus repository corroboration. **Sourced practice**; not a fourth mark to implement in this introduction. |
| G1 | Exactly three meanings, map-authored geometry, first entrance-tree-only spray, one action applying four sides and Work Plan approval sequence are the game's teaching model. | D-064 / approved Rack V2 §§20–29; P1/P2 do not establish that workflow. | High as accepted game-rule provenance. **Simplified game convention**, not Irish operational standard. |
| G2 | Source-informed orange slashes / blue ring can be a proposed game visual policy without changing Fell/Crop/rack saved meanings. | C1–C11 plus F3–F5; proposed spec FR-1–FR-10. | Medium design feasibility; **simplified game convention** requiring approval and future tests. Not an implemented or proven safe migration. |
| K1 | Exact shades, stroke count/width, recognition distances, contrast backing, map icons and draw/performance thresholds are design choices requiring evidence. | Spec FR-4 / O4, not forestry-source measurements. | Explicit **deliberate gameplay calibration**; effectiveness UNVERIFIED, performance UNMEASURED. |

Permissible wording: **“Game markings informed by Pro Silva Ireland practice.”** When discussing research, attribute the guide's own “standard” title directly. Do **not** say “the simulator implements the official Pro Silva standard”, “all CCF uses these colours”, or “three marks are the complete standard”. No such compliance/adoption assessment was performed. No external diagram was copied; publication availability is not asset-licensing permission.

## Draft recommendation and decisions requiring approval

The draft recommends exactly three introductory meanings: orange **vertical** rack-removal entrance stripes; orange **diagonal** individual Fell slashes; blue **ring** for retained/favoured Crop Trees. Shape plus label is mandatory; two orange meanings cannot rely on colour differentiation. Keep field entrance painting distinct from route lines, selected editing entrances, retained route-side trees and actual cleared/executed routes.

Manager/user must resolve the spec's **O1–O4** before implementation:

1. **O1:** approve the narrow D-013/Rack V2 visual/copy amendment; preserve saved meanings and existing authority until then.
2. **O2:** approve replacement of scoped floating diamonds/discs and settle consumed-proposal → pending/approved-work cues without losing work visibility or fabricating paint state.
3. **O3:** approve corresponding full-map shapes and derived entrance-painted display, with explicitly route-only minimap and unchanged Legacy40 rendering. No minimap icon expansion is assumed.
4. **O4:** approve visual distances/colours/size and performance/accessibility acceptance method after prototype evidence; proposed 3.5/5/15 m recognition and 30 m diagnostic are gameplay choices.

The plan/tasks are drafted dependencies, all production tasks BLOCKED. No marker colour, control, gameplay, work approval, save field or frozen reference was changed. New runtime-generated visuals would later reconstruct from the same semantic data; a same-schema intention does not itself establish hash/save safety.

## Verification performed and remaining gates

**Performed (docs/static only):** clean branch/base/common-store checks; `git show` inspection at exact candidate; material GUID wiring inspection; primary HTML and guide text retrieval; requirement/acceptance matrix authored; Markdown/local-link and file-scope checks; `git diff --check`. No C# compilation is necessary or claimed for a docs-only diff. No runtime harness, save manipulation, Unity Editor, Player or build was run. Source retries/errors are retained above; they are not successful browser/render tests.

Future exact steps and expected observations are in spec **AC-1–AC-7**, every row at **1280×720 and 1920×1080**, after fresh authorised Unity 6000.6.0f1 admission. These cover four-sided paint, light/shade/crowding, grayscale and three colour-vision simulations plus human review, full map and minimap visibility policies, help fit, native Linux input, current/genuine-old-save and work-state transitions, Legacy40/frozen reference and same-fixture performance. Harnesses must be inspected before copy/run/remove; protect original saves and remove generated temporary `.meta` files. Durable outputs belong on Files.

**All runtime/rendered/input claims remain UNVERIFIED; all performance/contrast/recognition measurements UNMEASURED.** Independent research/spec review has not occurred. No author approval or qualification token is issued. The protected integration/player-test branch was untouched and no merge is authorised by this handoff.

## Explicit unknowns and operational follow-up

- Actual on-screen colours, shader/import outcome, paint clipping/coverage on varied trunks, occlusion and distance readability; no screenshots rendered or measured.
- Help swatch/widget layout and text fit at either resolution; glyph visibility, contrast and real colour-blind player comprehension.
- Real keyboard/mouse/controller paths, work-order transition timing, save/load equivalence, hash/anchor preservation and performance impact of the proposed migration; no runtime verification.
- Statutory/formal national adoption status, independent usage prevalence, contractor/site variation and any European-wide paint mandate; no inference from publisher self-description.
- The photographed book's full bibliographic identity/edition/date and unresolved footnote; not repaired by finding a newer guide with matching practice. Source graphics' reuse rights not investigated.
- Final shades, dimensions, recognition distances and performance budget; all require O4 disposition. Legacy40's unchanged-path approach requires actual regression proof.
- Independent other-provider review and Manager/user approval; not self-assigned to this child. DPC should route the published exact PR head for source/forestry review.
- Dashboard ingestion/publication state was not inspected or changed. #200/DPC must record this DRAFT/PR in authorised coordination and confirm dashboard synchronisation; this worker cannot claim operational closure from its own document publication.
- Provider quota/reset/context usage, total tokens and duration beyond visible receipts remain UNKNOWN. No Unity admission, disk headroom or machine-wide lease state was checked or implied.

## V2 Orchestration Decision Appendix — EVENT 007

| Field | Evidence / value |
|---|---|
| Journal event | **EVENT 007** (parent dispatch supplied in user packet). The worker did not edit or independently attest the parent's append-only ledger. |
| Pool / provider | **`codex_c75d8114`**, OpenAI Codex, as supplied in dispatch; runtime confirms Codex harness. |
| Actual model / effort | **`gpt-6.1-sol` / medium**, runtime instruction and assigned packet agree. No model switch/escalation or child delegation. |
| Task complexity / risk | High research/spec scope: source-versus-copy discrepancy, forestry attribution, colour-independent recognition and saved/work-state boundaries. Docs-only route avoids an unauthorised product implementation. |
| Selection rationale / alternatives | Parent selected the permitted Medium Codex pool. The child can perform local Git/source inspection, primary-source HTTP retrieval, Markdown authoring and gh publication. Parent's comparison with Sonnet/other pools and exploit/explore metrics are **UNKNOWN**, not reconstructed. |
| Observed correction | Static audit corrected the issue shorthand “red Fell”: help/map are red, but assigned 3D material/default is orange-toned. Also exposed map ring versus curriculum square/diamond mismatch. This is evidence-driven correction, not a model-capability failure or escalation. |
| Tool/retry evidence | `python` alias unavailable; used installed `python3`. Non-www homepage/default-UA requests returned 403; www publisher page and browser-UA guide retrieval succeeded. Search-engine output supplied no forestry evidence used here. PDF Poppler font-weight warning disclosed. No dependency installation. |
| Capacity / quota / reset / context | **UNKNOWN**; no available trustworthy pool metrics consumed. No cooldown, reset wait, model replacement or claimed wakeup. |
| Tokens / cost / elapsed | **UNKNOWN** aggregate. Starting command receipts and source access are visible in session; no invented token/cost/duration total. |
| Research numeric provenance | P2 byte count/hash from retrieved bytes; pp. 10–13 → PDF spreads 6–7 from Poppler form-feed page enumeration. RGB/shapes/dimensions from numbered candidate source, never screenshot inference. Three marks and test distances are design constraints/proposals, not empirical forestry counts/distances. |
| Ownership / candidate | Assigned lane verified clean at `8f0f2301`; common store verified; `72b4efbd` source-only reads. Five owned Markdown paths; no coordination/ledger/other-lane mutation. |
| Unity / review / outcome | Unity deliberately forbidden/deferred. **DRAFT**, awaiting independent other-provider review, O1–O4 and Manager disposition; no merge/integration/acceptance claim. |

**Routing handback for DPC:** append EVENT 007 outcome using this source correction, primary-source evidence, exact published PR/head and honest UNKNOWN capacity metrics. Arrange independent research review; #200 owns authority/coordination/dashboard updates. Do not release production marking work from this DRAFT.
