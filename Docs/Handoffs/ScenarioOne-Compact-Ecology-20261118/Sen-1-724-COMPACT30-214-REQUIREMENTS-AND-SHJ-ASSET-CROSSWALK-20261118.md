# Sen-1 #724 — Compact30 214-requirement crosswalk and S/H/J asset boundary

**STATUS: DOCUMENTARY CROSSWALK / DRAFT SPECS UNTOUCHED / NO APPROVAL / NO PRODUCTION.** 2026-11-18. Prepared by the DeepSeek `deepseek-v4.1-flash` (`opencode-go`) overnight orchestrator in T3 Code environment `ac150346-a782-45eb-8cae-4e08fe9bab79` on `jer-laptop`. This file is one of the three registered #724 outputs; it changes no requirement checkbox, no spec byte, no source, scene, asset, save, Ops record or canonical file.

## 1. Exact sources, method and count

Read at immutable objects, not from a moving worktree:

| Spec family | Branch object | `specs/` tree | `checklists/requirements.md` blob | Unchecked IDs |
| --- | --- | --- | --- | --- |
| Parent (`scenario-one-compact-complete-loop-v1`) | `fe29627df3cdea8f8edb85d885a1ba7c90d8fa38` | `86141fd5a25ac5c2ec0d34d4a6002d96dd32837d` | `bcd0d6789ce724ab43db0678653a88ff8fbcc06e` | **115** (`R-001`..`R-115`) |
| Planting (`scenario-one-compact-initial-planting-v1`) | same | `81c5037a3105d0dc165f84074093c58b1d183d57` | `649ae95736a877dfe85b31628093949178841875` | **33** (`CIP-001`..`CIP-033`) |
| Graphics (`scenario-one-compact-graphics-v1`) | same | `1c203d05ec9ddc99462b2eb900c7278b87ae9fe1` | `2524826f42b47d924ab9413b6639947c45fb6027` | **66** (`FR-01`..`FR-20`, `WF-01`..`WF-06`, `SA-01`..`SA-05`, `AC-01`..`AC-12`, `G573-M1`..`G573-M7`, `NEG-01`..`NEG-08`, `V-01`..`V-08`) |

Total **214** unique IDs, all present, none invented, none renumbered. The three files remain **DRAFT with 214 unchecked `- [ ]` lines**. This crosswalk quotes each original line's first sentence (markdown emphasis removed, long sentences elided with "…") as **literal intent**; it does not paraphrase IDs into new requirements and does not tick anything. Where a single checkbox contains several conjuncts, the whole ID remains one gate: all conjuncts must pass before that box could ever be checked.

The `Status` column is this review's evidence assessment, **not** the spec's own status: `CANDIDATE` = documentary/authority input exists and the item is not blocked by a missing byte/rights gate at this moment; `BLOCKED` = the item cannot be evidenced before an unsigned owner, rights, native, save or world-byte gate is cleared.

### Gate definitions (from accepted #711 sequencing)

- **S** — required **before Tier S content bytes**: owner/authority decision, exact path/rights/physical admission, or a pre-content spec/process condition.
- **W** — required **before the final whole-world signature**: static/native content evidence that closes or contributes to the eight frozen rows, including rights chain, imported render dependencies, scene binding and independent review.
- **P** — **only after** the personal world signature: runtime gameplay, Work Plan, Annual Review, save/load, historical compatibility, integrated qualification, Windows.

### Evidence classes

- **OA** — owner-accepted direction (D-083/D-084/D-085 / #672 receipts), intent only.
- **AS** — accepted or signed documentary policy (#677 signature, #684 reconciliation, accepted process rules), not implementation.
- **RP** — reviewed or reviewed-quality proposal (#676 candidate, #718 inventory, #688/#698 Stage 0 blueprints), static only.
- **NR** — native/Unity/Blender/runtime evidence **NOT RUN**.

### Evidence references

| Ref | Source |
| --- | --- |
| **E1** | [#676 construction candidate + manifest + 36-cell JSON + independent review](https://github.com/jermur788/CCF/blob/8617587ebb7e118183bdd3eec7de5b099c371fdd/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-676-COMPACT30-CONSTRUCTION-REPRODUCIBILITY-AND-OWNER-SIGNATURE-CANDIDATE-20261117.md) — signed 0 / candidate 80 / blocked 8, `world_digest=null` |
| **E2** | [#718 Sol read-only scene/URP/FBX inventory](https://github.com/jermur788/CCF/blob/db5da69281e731bdd9889da3e62ed87505314821/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-718-SOL-COMPACT30-READONLY-SCENE-INVENTORY-AND-PRODUCTION-GATES-20261117.md) |
| **E3** | [#688 H1/path Stage 0 blueprint](https://github.com/jermur788/CCF/blob/2a625018d04d13339cafe480bdccf1a670e5f9d2/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-688-NATURAL-ENTRANCE-FOOTPATH-ASSET-BUILD-BLUEPRINT-20261117.md) |
| **E4** | [#698 juvenile/shelter Stage 0 blueprint](https://github.com/jermur788/CCF/blob/7a8e575ec1e6dc6157f9dda4710c006564faf0f5/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-698-OAK-BEECH-JUVENILE-AND-SHELTER-ART-BUILD-BLUEPRINT-20261117.md) |
| **E5** | [#704 D-032 dossier](https://github.com/jermur788/CCF/blob/docs/sen-1-704-d032-evidence-worker-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-704-D032-ASSET-RIGHTS-STORAGE-DECISION-DOSSIER-RESULT-20261117.md) + [#708 independent review](https://github.com/jermur788/CCF/blob/47be4f9b5a5682dfcfb1d3c2e6bc5d5f6d59322e/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-708-INDEPENDENT-D032-DOSSIER-REVIEW-RESULT-20261117.md) |
| **E6** | Decision Log D-032/D-083/D-084/D-085 on `main` + #672 owner receipts |
| **E7** | [#677 signed CG-TRIG/save-policy D-A signature](https://github.com/jermur788/CCF/blob/94c9835aeef052281499bd765f3089ca2cdb46e2/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-677-MANAGER-CGTRIG-SAVE-POLICY-EXACT-BYTE-D-A-SIGNATURE-20261117.md) |
| **E8** | [#684 signed CG-TRIG reconciliation](https://github.com/jermur788/CCF/issues/684) (document quality accepted; policy NOT IMPLEMENTED) |
| **E9** | The three DRAFT four-file specs themselves at the tree/blobs above |
| **E10** | [#711 accepted gate order](https://github.com/jermur788/CCF/blob/docs/sen-1-700-manager-intake-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-700-711-ACCEPTED-WORLD-SIGNATURE-GATE-ORDER-AND-SUCCESSOR-PRECISIONS-20261117.md) + [#716 eight-blocker closure evidence](https://github.com/jermur788/CCF/issues/716) |
| **E11** | #718 five-file ceiling + #690 seventeen-path save-lane boundary |
| **E12** | [#652 Ops readiness: SOURCE APPROVED / LIVE INSTALL HELD](https://github.com/jermur788/CCF/issues/652) |

### Ownership / test codes

`OW` owner decision; `MG` Manager disposition; `RV` independent nonauthor review; `T1` static/authoring check; `T2` isolated content-native Unity check; `T3` later integrated gameplay/save qualification; `OPS` physical Ops/custodian.

### Summary

| Spec | IDs | Gate S | Gate W | Gate P | CANDIDATE | BLOCKED |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Parent | 115 | 11 | 16 | 88 | 15 | 100 |
| Planting | 33 | 11 | 11 | 11 | 14 | 19 |
| Graphics | 66 | 19 | 38 | 9 | 14 | 52 |
| **Total** | **214** | **41** | **65** | **108** | **43** | **171** |

## 2. Parent matrix — 115 unchecked requirements

| ID | Literal intent (condensed) | Evidence | Open decision | Affects | Gate | Status | Ref | Next |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **R-001** | Product owner/Manager selects OPEN-C1 first-use orientation and teaching-complete pathway for a pre-existing H1-style entrance, winding pedestrian-only path, no internal rack and first real… | OA+AS | T1 selected; arrival/assessment UI spec still DRAFT | S,G | S | BLOCKED | E6,E8 | OW+MG / spec approval |
| **R-002** | Independent nonauthor reviewer identifies blockers and returns DRAFT-quality verdict; ONLY product owner/Manager/save owner resolves decisions and Manager explicitly promotes status after… | AS | independent review of exact spec bytes | G | S | CANDIDATE | E9 | RV |
| **R-003** | #424 actual Objective 1 source/#443 owner handoff inspected at pinned head; one writer and machine/Unity lease rechecked before production; no local source verification inferred from #458. | RP | fresh one-writer + Unity lease | G | S | BLOCKED | E2,E12 | MG |
| **R-004** | proposed D-083 Compact-first-beta exception / D-065 / D-071 / D-076 and current-milestone first-demo scope reconciled in canonical files; all original old-world contracts explicitly… | AS | canonical reconciliation evidence | G | S | CANDIDATE | E6 | MG |
| **R-005** | Existing #460 20/30m one-rack scratch is documented as historical research only; new compact/no-rack geometry and complete authentic stand/path/entrance/G1… | RP | actual geometry/stock measurement | S,H | W | BLOCKED | E1,E2 | T2 |
| **R-006** | #508 30m source authentic Year0 189 Sitka on the scene 2.0m lattice and hypothetical H1 14 pre-existing omissions (175 only if approved), and zero extra source path stems through 2.4m, are… | RP | 189/175 model-source confirmation | S | W | CANDIDATE | E1 | T1 |
| **R-007** | Owner-accepted initial all-Sitka and 16 Nov OWNER-RATIFIED G1 2 Oak +2 Beech count first lesson stated separately from still-unsigned exact H1 polygon/source-tree roster/history… | OA | exact H1 polygon + roster unsigned | S,H | S | BLOCKED | E6 | OW |
| **R-008** | Economic/training feasibility records real source cash/Work Plan jobs, quiet years, owner/contractor options, optional supported training-site funding or suitable small-job local provider… | RP | F-A €12,000 runtime affordability witness | E | P | BLOCKED | E6 | MG+T3 |
| **R-009** | No simulated coefficient or Irish empirical number silently imported from transfer or research suggestion. | AS | no invented coefficient | G | S | CANDIDATE | E9 | MG |
| **R-010** | Following any separately authorised canonical authority change, source/manifest and one-context-SHA permanent Drive mirrors verified/read back; not the final release/dashboard check in… | AS | Drive/dashboard sync not verified | G | P | BLOCKED | — | OPS |
| **R-011** | New compact identity explicitly stored and validated: world/scenario/geometry/content/version are independently approved, known-ID dispatch never silently maps to Legacy40/Enlarged80. | AS+NR | Compact identity implemented | V | P | BLOCKED | E7 | T3 |
| **R-012** | Compact primary/temporary/backup files disjoint from original forest-save.json/.tmp/.bak and O2 slots, including failed load/recovery/roundtrip; exact original bytes unchanged. | AS+NR | disjoint save files | V | P | BLOCKED | E7 | T3 |
| **R-013** | Selected compact/legacy/O2 profile governs New/Continue/F9/F10/direct LoadData/reference preview and backup preflight before scene, live, PlayerPrefs, disk or ID mutation; profile-specific… | AS+NR | profile router preflight | V | P | BLOCKED | E7 | T3 |
| **R-014** | Legacy→compact wrong-world load rejects before any mutation, including a structurally valid Legacy40/Enlarged80 payload and forged/missing compact ID; old world, legacy prefs and… | AS+NR | wrong-world refusal | V | P | BLOCKED | E7 | T3 |
| **R-015** | Compact→legacy explicit foreign-ID rejection must execute before historical legacy newer-version best-effort and before any scene/Prefs/disk mutation; this changes legacy's currently… | AS+NR | foreign-ID refusal before legacy best-effort | V | P | BLOCKED | E7 | T3 |
| **R-016** | O2 independent slot/identity, future irregular-cohort/six-species premise, funds and history cannot be mistaken for compact/O1, including overlapping tree IDs, work IDs or years; both… | AS+NR | O2 slot isolation | V | P | BLOCKED | E7 | T3 |
| **R-017** | Malformed/null/unknown world geometry, tree/species/cell/planting order/individual, save identifiers, duplicate IDs and cross-profile graph links rejected by shared pre-mutation validation;… | AS+NR | shared pre-mutation validation | V | P | BLOCKED | E7 | T3 |
| **R-018** | Explicitly missing/wrong compact identity rejected despite otherwise valid geometry, even on a legacy route and before legacy best-effort is evaluated; compact unsupported/newer version… | AS+NR | missing/wrong identity fails closed | V | P | BLOCKED | E7 | T3 |
| **R-019** | Repeated compact save/load preserves actual tree/cell/state/annual/economy/IDs without duplicated events, purchases, marks or work. | AS+NR | round-trip fidelity | V | P | BLOCKED | E7 | T3 |
| **R-020** | Cross-slot temp/backup never promoted; corrupt compact primary offers only explicit user-chosen recovery from independently preflighted correct-profile backup; invalid/foreign backup,… | AS+NR | backup recovery explicit | V | P | BLOCKED | E7 | T3 |
| **R-021** | Inventory authentic v1–v24 fixture/source version, origin and hashes from Tools/Verification/MfmSaveProvenance, S1CObjectiveHistory, TeachingPersistence, RackV2Save; old-file replay, not… | RP+NR | authentic v1–v24 fixture replay | V | P | BLOCKED | E1,E7 | T3 |
| **R-022** | Compare exact protected before/after Legacy40/Enlarged80/frozen Reference Future v1 serialized identity, archive/world/schedule hashes and old-world IDs with no rebaseline; audit… | RP+NR | authentic v1–v24 fixture replay | V | P | BLOCKED | E1,E7 | T3 |
| **R-023** | Pure-Sitka original growth/model-0 identity/numeric anchors exactly unchanged before/after compact profile on named old candidate; no relaxed test or swallowed mismatch. | RP+NR | authentic v1–v24 fixture replay | V | P | BLOCKED | E1,E7 | T3 |
| **R-024** | Old five-rack plans/entrance tree IDs/teaching objective history/MFM prescription/work event IDs persist and remain readable. | RP+NR | authentic v1–v24 fixture replay | V | P | BLOCKED | E1,E7 | T3 |
| **R-025** | Old v24 one-way historical first-guided review latch remains authentic/false when missing; compact cannot import latch as own teaching or completion signal. | RP+NR | authentic v1–v24 fixture replay | V | P | BLOCKED | E1,E7 | T3 |
| **R-026** | New compact profile has a pre-existing H1-style entrance/visitor clearing and winding people-only route shown consistently in world and Stand Map; no internal extraction rack. | OA | exact H1/path geometry | S,H | S | BLOCKED | E6,E3 | OW+T2 |
| **R-027** | Approved OPEN-C1 first-use arrival/path/orientation workflow and OPEN-C7 durable assessment/G1 planting/review teaching-complete signal are independently verified before optional skip; no… | OA | T1 arrival/assessment signal | G,S | S | BLOCKED | E6,E8 | OW+MG |
| **R-028** | Player assessment and G1 exact-position planting markers do not fell on designation, manufacture an entry road or instantly create a juvenile. | OA | four authentic orders; no instant juvenile | R | P | BLOCKED | E6,E8 | T3 |
| **R-029** | Entrance and footpath are pre-existing Year0 features; if H1 tree omissions are authorised, the 14 affected original Sitka are removed at generation as documented history, not… | OA | 14 omissions are generation history | S | W | BLOCKED | E6 | T2 |
| **R-030** | Player can physically navigate the winding pedestrian-only path and access/inspect eligible trees throughout the bounded compact parcel and all four G1 planting markers, not only an… | OA+RP | route walkability native | H,S | W | BLOCKED | E3 | T2 |
| **R-031** | Only existing TreeMarkType None/Fell/CropTree, red/blue exclusivity and optional real supported habitat retention; no invented fourth habitat mark or auto crop-selection. | RP | no fourth mark | R | P | BLOCKED | E1 | T3 |
| **R-032** | Player undo/repaint/reassign before Work Plan approval preserves valid identities and eligible tree state. | OA | W-A approved-order policy | R | P | BLOCKED | E6 | T3 |
| **R-033** | Marks enter Pending when Work Plan opens; changed draft paint cancels/requotes stale Pending with event/order/source identity retained, price and status explained to player. | OA | W-A requote semantics | R | P | BLOCKED | E6 | T3 |
| **R-034** | Owner-approved OPEN-C8 policy enforces explicit Approved-order withdrawal/requote/reapprove OR safe block; no silent spray editing of Approved work, no false quote/order outcome. | OA | W-A withdraw/requote or block | R | P | BLOCKED | E6 | T3 |
| **R-035** | No new compact rack/rack corridor/orange entrance marks, phantom 2–5 racks, source-rack→target proposals, MFM halo/automatic selection or three-tree teaching-credit shortcut; old Enlarged80… | RP | no Compact rack | R,M | P | BLOCKED | E1 | T3 |
| **R-036** | Original pinned old-world MFM tests for five-rack teaching/coverage/prescription/proposals continue unchanged; retain all checks without claiming unfinished MFM-08/09/10 already passed. | RP+NR | old MFM tests unchanged | G | P | BLOCKED | E1 | T3 |
| **R-037** | Draft/selected vs Pending/Approved vs Completed/Failed/Cancelled all distinguishable in UI, map, history and saves. | RP | state legibility | M | P | BLOCKED | E1 | T3 |
| **R-038** | Marking/map preview alone generates no felling, payment, loss of canopy, ecological tick, event or Year-change. | RP | preview has no side effect | M | P | BLOCKED | E1 | T3 |
| **R-039** | Any NEW/UNVERIFIED current-tree BA (m²), selected biological volume (m³), residual retained BA, same-year canopy geometry and declared management-area units are sourced/qualified before… | RP | BA/volume sourced before display; C6 overlay part deferred | M,E | P | BLOCKED | E6 | T3 |
| **R-040** | First early G1 planting demonstrates honest four exact owned/reserved stocked orders (2 Oak +2 Beech), Pending quote, Work Plan approval, execution and actual material/provider/owner… | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-041** | Approved real work resolves at authoritative annual boundary once, records actual net revenues/costs/deadwood/retained wood correctly. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-042** | Historical #460 two one-rack scratch candidates (20m/30m) made three contractor jobs each (€7,500), sold €138.19/€193.42 and ended €4,622.44/€4,677.67 under historical work/economy. | RP | historical scratch comparators only | E | P | CANDIDATE | E1 | T3 |
| **R-043** | Sitka adult competition, growth/mortality/canopy, site and timber decisions remain active and observable. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-044** | Oak has differentiated adult/juvenile, light/site, browsing, planted individual and merchantability behavior at accepted evidence depth. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-045** | Beech has differentiated adult/juvenile, light/site, browsing, planted individual and merchantability behavior at accepted evidence depth. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-046** | Natural cohorts, seed supply/establishment/sapling promotion and planted exact juveniles advance in real years; planting cannot instantly become mature commercial tree. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-047** | Understorey bramble/bracken, browsing/protection and regeneration interactions are authoritative, with honest non-causal habitat-only representations. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-048** | Deadwood, retained habitat, removal outcomes and mortality records are state-driven and observable; graphics never invent ecology. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-049** | Candidate edge/competition/crown/seed influence classified honestly for a small genuinely bounded woodland with potentially open fields/roads outside; existing finite CI is correct if no… | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-050** | Annual model performs real authoritative year-by-year state update in existing order with persistable snapshots, no compact “light” engine. | OA+RP | runtime teaching/economy implementation | R,E | P | BLOCKED | E6 | T3 |
| **R-051** | Owner-approved D-074/OPEN-C5 new compact substantive intervention classifier requires bounded area, meaningful multiple actual retention/release judgements, real linked succeeded work and… | OA | M-A classifier; OD-16 DEFERRED | R,E | P | BLOCKED | E6 | OW+T3 |
| **R-052** | Three substantive cycles are Manager DRAFT test interpretation, require owner ratification before becoming completion rule; if accepted, real distinct episodes and at least one later… | OA | three-cycle quota NOT adopted | R | P | BLOCKED | E6 | OW+T3 |
| **R-053** | Approved D-074 spacing/observation policy (MD-02/MD-03) applied to real resolution years and observed/reassessed stand; no unapproved 3-year hard minimum, and quiet-year records never… | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-054** | Player may perform legitimate planting, shelter/protection, pruning, true later forestry treatment, optional other maintenance and choose no-work years, without automatic substantive CCF… | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-055** | S1-C D-073 L1 recorded, L2 observed, L3 derived, L4 authored sources and approved feedback-V2 provenance/missing-order limitations preserved; no mislabelled causation/duplicate event ID. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-056** | Annual review/acknowledgement only after real reading; no invented backdated or duplicated AnnualReviewAcknowledged events. | AS | event9 atomic acknowledgement | V,R | P | BLOCKED | E7 | T3 |
| **R-057** | Approved D-077 normal one-year advance remains; optional compact skip only after a distinct OPEN-C7 initial entrance/assessment/G1 planting/review teaching-complete persisted signal,… | AS | CT-3 hold all advance routes | V,R | P | BLOCKED | E7 | T3 |
| **R-058** | One-year advance and eligible skip both run every actual intervening annual ecological/financial/objective/snapshot tick. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-059** | D-077 4-year suggested reassessment and max-5 real annual ticks are navigation calibration, not required intervention spacing. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-060** | Skip executes ordinary ticks, stops at first storm, outcome, objective, lesson, mandatory review/Year25/Year100 gate; cannot create work, mark Fell or auto-acknowledge annual review. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-061** | Two separate parity oracles: direct manager N yearly ticks with NO acknowledgment and per-tick state/hash equality; UI acknowledged annual-click route compared semantically after legitimate… | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-062** | New compact versioned completion criteria policy owner-approved for substantive treatments, ecology/species/finance and reviews; no historical two-cycle, five-rack, v24 latch or old-world… | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-063** | Completion once, correctly persisted, history browseable and continued management/observation supported when legitimate. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-064** | Old-world completion and educational achievements unchanged; no retroactive false failure or retroactive compact credit. | OA+AS | runtime completion/review implementation | R,E | P | BLOCKED | E6,E7 | T3 |
| **R-065** | Full Stand Map and minimap show actual pre-existing H1-style entrance, winding pedestrian-only path, G1 four individual tree planting positions, current standing tree IDs/crowns/marks,… | OA | map content from signed world | M,S,H | W | BLOCKED | E6,E3 | T2 |
| **R-066** | Existing/current tree condition, draft marks, Pending/Approved and historical completed work have explicit legends, shapes and readable text independent of colour. | RP | legend independent of colour | M | W | BLOCKED | E1 | T2 |
| **R-067** | Optional read-only current-crown geometry overlay computes from exactly owner-approved OPEN-C6 SelectedOpeningRemovalSet = draft marks OR Pending OR Approved OR explicit single-status… | OA | C6 overlay deferred | M | P | CANDIDATE | E6 | OW later |
| **R-068** | Overlay labels selected input status and immediate conditional geometry (NOT future ecology); undo, cancel, approval/withdraw/requote or status-selector changes recompute/empty predictably… | OA | overlay labels/status | M | P | CANDIDATE | E6 | OW later |
| **R-069** | No MFM sample/target/candidate/proposal/coverage symbols, phantom orange rack entrance markers, false machine corridors or auto-work illustrations are shown in new compact. | RP | no MFM/rack symbols in Compact | M | W | BLOCKED | E1 | T2 |
| **R-070** | Actual scene geometry, species silhouettes/crowns, juvenile and mature-size visuals map to authoritative simulated states. | RP+NR | scene visuals map to state | S,H,J | W | BLOCKED | E1,E2 | T2 |
| **R-071** | Ground/understorey/deadwood/lighting/sound remain model-bound where causal; no shader-only false species stage. | RP+NR | model-bound surface/lighting | H,S | W | BLOCKED | E3,E2 | T2 |
| **R-072** | 1280×720 and 1920×1080 native map/arrival/footpath/G1 four planting-marker and later Fell/Crop mark flow, Work Plan/review legibility, grayscale, occlusion, collision, actual field reach… | RP+NR | 720p/1080p native legibility | M,S | W | BLOCKED | E2 | T2 |
| **R-073** | LOD/collision/scene import/material IDs/asset provenance/licences unchanged where protected; new art measured for performance. | RP+NR | provenance/licence/LOD/import | S,H,J | W | BLOCKED | E5,E2 | T2 |
| **R-074** | Target GPU/hardware/render conditions and draw calls, memory and frame-time measured; numerical budget adopted by owner AFTER evidence, no invented threshold in DRAFT. | RP+NR | measured performance budget AFTER evidence | S,H,J | W | BLOCKED | E2 | T2+OW |
| **R-075** | Fixed-seed before, conditional-preview and after actual annual work rendered 3D/map comparisons agree and are archived. | RP+NR | before/after render comparisons | S,H,J | W | BLOCKED | E2 | T2 |
| **R-076** | Only approved source lanes, registered claims and serialized machine-wide Unity ownership used; no user-protected saves/Editor interrupted. | AS | registered lane + machine-wide Unity owner | G | S | CANDIDATE | E12 | MG |
| **R-077** | Tier-1 targeted tests, focused Tier-2 feature qualification and one pinned-candidate Tier-3 integrated qualification all produce durable logs and GitHub handoffs. | AS | Tier1/2/3 evidence ladder | G | P | CANDIDATE | E9 | MG |
| **R-078** | Independent nonauthor source/rendered/data review signs exact candidate; Manager returns formal APPROVED / NEEDS CHANGES / REJECTED. | AS | independent nonauthor review + Manager verdict | G | W | CANDIDATE | E9 | RV+MG |
| **R-079** | No Windows build before user's personal exact-candidate test and explicit Windows-build approval. | AS | owner personal test before Windows | G | P | CANDIDATE | E6 | OW |
| **R-080** | Exact old O1 five-rack/Legacy40/frozen Reference and independent later O2 save paths remain unchanged after compact source; untouched canonical/historical data and no source competition… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-081** | No acceptance asserted from only static audit, mock preview, research inventory, screenshots without source state or compilation without runtime. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-082** | Final project status/dashboard publication and audit ingestion verified against receipts where used, distinct from R-010 canonical context/Drive mirror sync; never assert sync without… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-083** | #436 SI-02/03/05 and FR-02/06 reconciled by ONE APPROVED save/router owner; either compact added as third profile to #436 or #436 formally delegates O1; no simultaneous A-01 implementations. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-084** | Legacy future-schema best-effort remains as shipped without independent save-owner approval; compact newer/unknown version fails closed only under its newly accepted profile. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-085** | Every public/private load route including direct LoadData, Continue, F9/F10, New/Load and Reference preview invokes preflight BEFORE first… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-086** | Rejected compact load whose geometry is valid but intended compact ID absent/foreign fails without any changed live/PlayerPrefs/primary/.tmp/.bak checksums. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-087** | Both existing global namespaces CCF.Learning.v1.* and CCF.MenuHelp.v1.* remain exactly unchanged across compact new/progress/load/reject; all old five-rack step IDs never reused or renamed. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-088** | Preexisting legacy learning/help keys cannot complete compact teaching, unlock compact advancement or initialize compact achievement; legacy latch cannot be new-world proof. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-089** | Compact learning/help state uses approved disjoint keys or an explicitly approved equivalent policy; missing/foreign compact keys and rejected loads have no mutation or step backfill. | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-090** | Compact geometry ID/unknown IDs cannot silently resolve to either old 40 m/64 cells or 80 m/256 cells anywhere in source, cells, objectives, maps, reference or boundary; pre-v20 old saves… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-091** | Save owner selects independent compact root OR identity-neutral audited shared-field addition; frozen Reference Future v1, legacy and pure-Sitka hash/serialization exact pre/post or STOP… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-092** | Same-slot backup must independently pass all version/identity/content checks; explicit user recovery only; corrupt/foreign and stale temp never cross-copy, overwrite or mutate until… | AS+NR | save/history preservation implementation | V | P | BLOCKED | E7 | T3 |
| **R-093** | Owner-approved OPEN-C1 first-use arrival/assessment presentation and OPEN-C7 compact teaching-complete proof must follow genuine G1 2 Oak+2 Beech planting/Work Plan review or other approved… | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-094** | D-074/OPEN-C5 substantive episode policy remains owner-approved-only: define bounded no-rack management area, genuine manual retention/release decisions and linked succeeded work,… | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-095** | Native classifier negative examples: historical old-world rack clearance, new-world pre-existing H1 omissions, initial four G1 planting/shelter/protection, pruning, isolated token Fell and… | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-096** | D-077 FellTree navigation advice may use a real succeeded FellTree year in either profile (historical rack FellTree in old worlds only) without giving substantive-management credit. | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-097** | Work Plan selected-tree BA/volume and optional current-crown illustration may use common owner-selected SelectedOpeningRemovalSet only IF APPROVED under OPEN-C5/C6, otherwise separate named… | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-098** | Approved-order edits follow explicit OPEN-C8 withdraw/requote/reapprove or safe block policy; unchanged work history, no surprise execution from spray. | OA+AS | runtime teaching/work-plan implementation | R,V | P | BLOCKED | E6,E7 | T3 |
| **R-099** | [#460](https://github.com/jermur788/CCF/issues/460) measured two independently generated historical temporary one-rack 20/30m Unity research candidates (102 native checks reported PASS) and… | RP | historical #460 evidence only | G | W | CANDIDATE | E1 | T1 |
| **R-100** | Earlier #458/#459/#498/#499 DRAFT quality reviews and #460 research remain historical; #508 source screening, #513 independent browser guidance and [#515 Manager… | AS | review history retained | G | S | CANDIDATE | E9 | MG |
| **R-101** | Historical exact fixture provenance with originals/versions/hashes inventoried; replaying a compact-created "old" file does not count for v1–v24 proof. | RP | authentic fixture provenance | V | W | BLOCKED | E7 | T3 |
| **R-102** | OWNER-ACCEPTED OPTION 1, STILL UNIMPLEMENTED: compact begins all Sitka with no Oak/Beech adults, juveniles, prior cohort or seed donors; the player creates exactly 2 sessile Oak +2 Beech… | OA | owner Option 1 all-Sitka start | R,E | P | BLOCKED | E6 | T3 |
| **R-103** | Legacy MFM tests at exact pinned candidate retained, without falsely claiming not-yet-finished MFM-08/09/10 qualification. | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-104** | Every item worded "clear", "playable", "educational", "credible", "appropriate" or "meaningful" either has a defined measurable evidence set and approved criterion or is explicitly routed… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-105** | Owner-accepted Option A broadleaf establishment in complete first-demo Scenario One uses player-led early Oak AND Beech planting and later authentic care/management, not mandatory natural… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-106** | No naturally established Oak/Beech seedling by elapsed Year40 is required for success, and no objective, tooltip, tutorial, scoring, achievement or comparison claims it is guaranteed. | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-107** | FIRST PLANTING OWNER SEQUENCE / 16 NOV OWNER-RATIFIED 2+2 COUNT; #515 LOCATIONS STILL CONDITIONAL: ONE early G1 group of 2 sessile Oak +2 Beech, not required 4+4/two groups, after authentic… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-108** | Positive and negative authentic annual witnesses: surviving and lost planted individuals tracked by species and planting ID; browsing/ground vegetation/competition/light and plant-care… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-109** | #490 evidence provenance remains explicit: authentic #460 initial 189 Sitka, identical early states; two 40-tick 30m research arms, worker-reported 142/142 checks, one Oak/one Beech age42… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-110** | Distinguish biological age/seed potential and modelled DBH/height/crown from Unity 'Mature' interaction-stage and prefab choice; Year20/40 visible witness must actually show inspectable… | OA+RP | biological vs visual stage distinction | J,R | P | BLOCKED | E4,E6 | T3 |
| **R-111** | #496 source-vs-harness deadwood gate: #490 measured aggregate Sitka mortality but zero deadwood records in a fixture whose manager was reportedly disabled; these zeros do NOT establish… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-112** | Understorey bramble/bracken, browse/protection and canopy-light influence must be analysed from real source outputs when used to interpret broadleaf survival; #490 exported raw… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-113** | Later-year woodland walkthrough requires real annual reassessment of original G1 2+2 planted broadleaves, genuine protection/care, Sitka retention/release or justified no-Fell choices,… | OA+RP | broadleaf runtime/ecology evidence | E,R | P | BLOCKED | E6 | T3 |
| **R-114** | The proposed same original forest guided example and independent replay remains OPEN-C10 until user approves its exact workflow: separate safe saved identities/histories/learning state,… | OA | C10 replay deferred | M,R | P | CANDIDATE | E6 | OW later |
| **R-115** | Review-only proposed D-083 Compact-first-beta exception (review #579; obsolete #471 paused) canonical first-demo authority remains with Overall Manager #425, not edited/promoted by #516. | RP | canonical authority reconciliation | G | S | CANDIDATE | E6 | MG |

## 3. Planting matrix — 33 unchecked requirements

| ID | Literal intent (condensed) | Evidence | Open decision | Affects | Gate | Status | Ref | Next |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **CIP-001** | Latest owner intent preserved: all-Sitka initial forest, early player-led Oak and Beech planting, no mandatory natural Oak/Beech recruits by elapsed Year40 and no required later planting… | OA | no new owner choice | S,G | S | CANDIDATE | E6 | MG |
| **CIP-002** | Owner-ratified 30×30m bounded property design; signed world still pending is explicitly labelled OWNER RATIFIED BASELINE; final world UNSIGNED and not rebranded Legacy40/Enlarged80; old… | OA | 30×30 baseline labelled unsigned | S,G | S | CANDIDATE | E6,E9 | MG |
| **CIP-003** | Original #460 C-B-30 189-tree UNMODIFIED pre-layout inventory hash/stage pinned. | RP | 189 inventory source pin | S | W | CANDIDATE | E1 | T1 |
| **CIP-004** | No mandatory 2.4m through-rack or automatic 19-stem Year1 clearance. | OA | no mandatory rack/19-stem | S,H | S | CANDIDATE | E6 | OW |
| **CIP-005** | Winding people-only path is assessed from the entrance to two useful planting/observation sites without a central rack; actual tree/path collisions, human reach/visibility and route intent… | OA+RP | GQ-09 route / GQ-10 endpoint | H,S | S | CANDIDATE | E6,E3 | OW+T2 |
| **CIP-006** | Original #460 Year0 pre-rack C-B-30 189-tree inventory and geometry/hash inspected. | RP | pre-rack inventory inspected | S | W | CANDIDATE | E1 | T1 |
| **CIP-007** | Four proposed G1 2 Oak+2 Beech exact planting positions are source-screened candidates after hypothetical H1 entrance omissions; actual new-profile cell light, standing tree clearance,… | RP | four G1 positions source-screened | S | W | BLOCKED | E1,E4 | T2 |
| **CIP-008** | Only one G1 source-screened first-lesson planting group with 2 Oak+2 Beech is required. | OA | one 2+2 group only | S | S | CANDIDATE | E6 | OW |
| **CIP-009** | Each candidate site is inside actual stand ecology cell and is unique; duplicate open planting order/marker cases are rejected. | RP | unique in-cell sites | S | W | BLOCKED | E1 | T2 |
| **CIP-010** | Each site passes source current-tree ≥0.7m live stem admission and negative controls for near-stem, outside-boundary and known structure/road overlap, noting that this alone does not ensure… | RP | ≥0.7 m stem admission | S | W | BLOCKED | E1,E4 | T2 |
| **CIP-011** | ~2m between new saplings treated as provisional player legibility/competition preference, with measured actual site distances, not as an Irish forestry regulation or CCF engine limit. | RP | ~2 m provisional spacing | S | W | CANDIDATE | E4 | T1 |
| **CIP-012** | Actual site canopy/light/browsing/bramble/bracken and existing tree/shade relation measured where original data permits, otherwise listed NOT MEASURED; no guaranteed Oak/Beech survival. | RP | site canopy/light measured | S,J | W | BLOCKED | E1,E4 | T2 |
| **CIP-013** | Winding pedestrian path offers at least one credible inspection viewpoint for each planned planting group; path route through trees, possible turn/loop and player view remain NOT VERIFIED… | RP | inspection viewpoint per group | H,S | W | BLOCKED | E3,E4 | T2 |
| **CIP-014** | Species-specific actual nursery stock identity, possession and reservation verified before each exact-position planting mark; insufficient stock and species mismatch have negative checks. | RP | nursery stock identity | R | P | BLOCKED | E4 | T3 |
| **CIP-015** | Four individually tracked player-created Pending exact-position planting orders (2 Oak+2 Beech) require authentic Work Plan approval/resolution and annual history. | OA | four Pending exact-position orders | R | P | BLOCKED | E6,E8 | T3 |
| **CIP-016** | Per-tree protection and owner/contractor method are genuine editable choices, with source costs/eligibility and no forced shelter success; default all-sheltered is recommendation only. | OA | per-tree protection choice | R,J | P | BLOCKED | E6,E4 | T3 |
| **CIP-017** | Source gameplay calibration for 2 Oak (€11) +2 Beech (€9) = €20 stock-only, and four optional €5 shelters = up to €20 materials before work; indicative contractor planting four × €7.50 =… | RP | €20 stock + €20 shelters | E | P | CANDIDATE | E4,E6 | T3 |
| **CIP-018** | Owner time, contractor fees, stock/material costs, cash and status recorded as quote versus settled actual; legacy €2,500 per harvest job not mislabelled compact planting cost; OPEN-C5… | RP | quote vs settled actual | E | P | BLOCKED | E4 | T3 |
| **CIP-019** | All four planted individual juveniles retain correct source IDs, species, nursery age, protection/actor, planting positions, subsequent annual ecology and review history; never conflate… | RP | four juvenile identities retained | R,V | P | BLOCKED | E4 | T3 |
| **CIP-020** | Later annual reports include meaningful survival/loss, growth, light/browse/ground effects and honest unmeasured causes; no forced promotion, mature seed donors, broadleaf natural… | RP | annual survival/growth reporting | R,E | P | BLOCKED | E4 | T3 |
| **CIP-021** | A real later forestry management and no-work decision remains in the complete-loop design; a second planting lesson and compulsory Fell/thinning counts are NOT used to earn intervention… | OA | later management + no-work decision | R,E | P | BLOCKED | E6 | T3 |
| **CIP-022** | Normal enabled-manager mortality event/record/fallen visual is separately tested when relevant, and #490 manager-disabled zero-deadwood is NOT claimed as evidence of normal-player absence. | RP | mortality event visual | R | P | BLOCKED | E4 | T3 |
| **CIP-023** | Exact 30m geometry/save/router/PlayerPrefs, source history for pre-existing entrance clearing/path, professional vehicle/parking/loading, and any need for different plot size remain under… | RP | geometry/save/router/vehicle scope | G,S | S | CANDIDATE | E6,E7 | MG |
| **CIP-024** | Independent completed #508 Tier1 result is reviewed and incorporated at correct evidence tier: scene 2.0m lattice, 189 original Sitka, H1 12×10m with 14 proposed pre-existing omissions, 175… | RP | #508 Tier1 reviewed at right tier | S | W | CANDIDATE | E1 | T1 |
| **CIP-025** | Independent nonauthor review of verified feasibility and four-file spec issues APPROVED/NEEDS CHANGES/REJECTED for DRAFT quality only; no self-review or early implementation approval. | AS | independent nonauthor DRAFT review | G | S | CANDIDATE | E9 | RV |
| **CIP-026** | Owner personally playtests whether G1's four early planted saplings, field navigation and later real management decisions produce a meaningful complete loop; a separately taught later… | OA | owner playtest of loop | G,R | P | BLOCKED | E6 | OW |
| **CIP-027** | Game-world design includes one pre-existing entrance clearing connected to a legitimately permitted boundary approach. | OA | H1 design + legal boundary approach | H,S | S | CANDIDATE | E6,E3 | OW |
| **CIP-028** | Ordinary car parking and an occasional staged collection-truck event may use the same pre-existing entrance clearing at different times; during collection, cars/visitors are absent from the… | OA | parking / staged truck event | H,S | S | BLOCKED | E6,E3 | OW |
| **CIP-029** | Keep the owner's simple gameplay truck concept: suitably small collection truck backs into the existing clearing and exits forwards, with no internal turning or extraction rack. | OA | truck backs in, no rack | H,S | S | BLOCKED | E6,E3 | OW |
| **CIP-030** | Winding people-only path reuses source track (0 extra tree conflicts at tested widths through 2.4m in #508); actual Unity path walkability and route visibility to G1's four trees remain NOT… | RP | path reuse; native walkability | H,S | W | BLOCKED | E3 | T2 |
| **CIP-031** | All Sitka missing from proposed entrance/path footprint have explicit pre-existing stand-history or starting-generation omission provenance with exact tree IDs, changed canopy/light and… | RP | omission provenance per tree ID | S | W | BLOCKED | E1 | T2 |
| **CIP-032** | Retain H1 12×10m and a small staged collection truck as reversible game presentation. | OA | keep H1 + reversible truck presentation | S,H | S | CANDIDATE | E6 | OW |
| **CIP-033** | The player experiences a coherent arrival → parking/entry → walk/inspection → real planting → subsequent management loop; optional timber collection is a legitimate staged operation with… | OA | coherent arrival→planting loop | G,R | P | BLOCKED | E6 | OW+T3 |

## 4. Graphics matrix — 66 unchecked requirements

| ID | Literal intent (condensed) | Evidence | Open decision | Affects | Gate | Status | Ref | Next |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| **FR-01** | Graphics bind only to signed/authorised Compact world/H1/route/G1/profile version; no independent source-tree placement, lost manifest or extra saved identity. | RP | signed world binding | S | S | CANDIDATE | E9,E1 | MG |
| **FR-02** | All-Sitka start, exactly reviewed pre-world H1 historical omissions and untouched buffer survivors are faithfully depicted; original research 189→175 arithmetic is not misreported as actual… | OA+RP | H1 omissions faithful; 189≠175 | S | W | BLOCKED | E6,E1 | T2 |
| **FR-03** | Only one winding pedestrian route; no rack, rideable forestry extraction road, old five-rack teaching or imaginary timber traffic down path. | OA | one pedestrian route only | H,S | S | CANDIDATE | E6,E3 | OW |
| **FR-04** | Owner-directed 1.2 m physical core + ≤0.20 m visual-only verge per side on a signed curve (C0 or T1 + new 0.190 m cap, owner-pending) to a signed natural northern endpoint (A8 unsigned);… | OA | GQ-09 C0/T1 + GQ-10 A8 | H,S | S | BLOCKED | E6,E3 | OW+T2 |
| **FR-05** | Walking core, ribbon, verge and foliage display mask documented as separate footprints (analytic regions on one plane only if GQ-08 accepts); verge ≤0.20 m, visual-only, never a collider or… | RP | separate core/ribbon/verge/mask footprints | H | W | BLOCKED | E3 | T2 |
| **FR-06** | Any path-G1 collision/failure returns to approved manifest amendment and source rerun without silently moving 4 G1 points, 14 H1 source tree IDs or old saves. | RP | path–G1 conflict returns to manifest amendment | H,S | W | BLOCKED | E1,E3 | T2 |
| **FR-07** | Actual Compact H1/needle ground/path material with appropriate scale, roughness and grayscale contrast at H1/bend, preserving HUD/forestry mark distinction. | RP | ground material scale/roughness/grayscale | H | W | BLOCKED | E3 | T2 |
| **FR-08** | Compact materials are distinct from shared historical Forest Ground/Path and their Editor menu; no unapproved global camera/URP/fog change. | RP | distinct Compact materials; no global change | H | W | CANDIDATE | E3 | T2 |
| **FR-09** | Owner-selected separate Compact scene/hierarchy (D-085 OD-13) with no 2.6 m historical planks or their clones/consumers, and an approved parent source bridge for corridor, Stand Map,… | OA | separate Compact scene (OD-13); no planks | S | S | CANDIDATE | E6,E2 | MG |
| **FR-10** | Display-only vegetation mask downstream of VegetationSites leaves ScenarioOneClearance, preview, target IDs/counts and clearance summary unchanged. | RP | display mask leaves clearance source unchanged | H,R | W | BLOCKED | E3 | T2 |
| **FR-11** | Visible + suppressed per cell/per kind = source intended visual instances, including rush anchors and full-cell masks, with per-class footprint radius, deterministic Jitter after save/load,… | RP | visible+suppressed accounting deterministic | H,R | W | BLOCKED | E3 | T2 |
| **FR-12** | Reuse documented tracked first-party Sitka/understorey/deadwood, with missing/partial Ultimate Nature never mandatory and unreviewed ForestFloorV1 not inserted. | RP | tracked first-party assets; no mandatory Ultimate Nature | H,J | W | BLOCKED | E5,E2 | T2 |
| **FR-13** | H1 surface, Compact-only footpath, Compact-only forest floor, small Oak/Beech transplants and shelter have accepted source/model/texture/LOD scope and costs. | RP | H1/path/floor/transplant/shelter scope+cost | H,J | W | BLOCKED | E3,E4 | T2 |
| **FR-14** | Transplant displays true juvenile source height with species-appropriate non-miniaturised leaf/branch form, without changing actual growth/size or planting mechanics. | RP | juvenile true stock height/form | J | W | BLOCKED | E4 | T2 |
| **FR-15** | Shelter/guard visual accepted and bounded in upright, expired and failed states; current failed O1 overlap corrected without changing failedYear, Browse efficacy, growth or stem locations;… | RP | three guard states; fix 0.141 m O1 | J | W | BLOCKED | E4,E1 | T2 |
| **FR-16** | Owner-chosen W direction (16 Nov) accepted as an exact look from actual Compact native views, using the proposed #688 palette or an owner-revised one at 1.2 m + ≤0.20 m (not #577's 0.35 m… | OA | GQ-01 W look from native views | H | S | BLOCKED | E6,E3 | OW+T2 |
| **FR-17** | Any optional sky/fog/boundary scope explicitly accepted with technical camera isolation; no false biological seed donor or global leakage. | OA | optional sky/fog separately scoped | H | S | CANDIDATE | E6 | OW |
| **FR-18** | D-032 approved provenance/distribution/source policy and asset bytes/hashes verified before any tracked production binary/import; no unlicensed third party. | AS | D-032 rights before tracked binary | S,H,J | S | BLOCKED | E5 | OW |
| **FR-19** | Native H1/bend/interior/G1/later view captures at 1280×720 and 1920×1080 with HUD/quality/LOD/pack/world identity pinned; no old E80 capture substitution. | RP | native 720p/1080p captures pinned | ALL | W | BLOCKED | E2 | T2 |
| **FR-20** | New Compact same-machine baseline/after median/p95/frame/load/texture memory; independent Tier2/owner look pass and no Windows build before user personal test and separate explicit… | RP | performance baseline + owner look | ALL | W | BLOCKED | E2 | T2+OW |
| **WF-01** | Arrival: H1 appears pre-existing, natural edges; path mouth recognisable on first arrival; no phantom fells/stumps/jobs/income. | AS | H1 natural arrival + GQ-08 edge | H | W | BLOCKED | E3,E6 | T2 |
| **WF-02** | Movement: full narrow footpath traversable with normal player controller at H1, first bend, G1, terminus; no turning trap, forced departure or invisible collider. | AS | native walk at H1/bend/G1/terminus | H,S | W | BLOCKED | E3,E2 | T2 |
| **WF-03** | Assessment: from normal stand/inspection UI, real forestry marks and site information remain legible against visual environment. | AS | mark/site legibility against visuals | M,H | W | BLOCKED | E2 | T2 |
| **WF-04** | Planting: player receives stock, nominates actual four Pending 2 Oak+2 Beech orders, approves genuine Work Plan, annual completion creates real juveniles in expected world site; no earlier… | AS | genuine 2+2 order-to-juvenile flow | R | P | BLOCKED | E6,E8 | T3 |
| **WF-05** | Return/review: juvenile species/protection/clearance and subsequent annual changes visible without changing ecology or Browse, including saved/reloaded states. | AS | reload-true juvenile/protection view | J,R,V | P | BLOCKED | E4 | T3 |
| **WF-06** | Collection: no parked or permanent vehicle, yard, gate or road in H1 (D-085 OD-06); any occasional collection stage/mode event is separately scoped, implies no drivable footpath or road… | AS | no vehicle/yard/gate; staging separately scoped | H | S | CANDIDATE | E6 | OW |
| **SA-01** | Direct owner baseline 30×30m, H1, winding no-rack, 2 Oak+2 Beech count preserved; exact sites and width explicitly separately approved. | OA | owner baseline; sites/width separately approved | S,G | S | CANDIDATE | E6 | OW |
| **SA-02** | Canonical first-beta D-083 scope accepted/published under appropriate authority; historical D-082 Sitka experiment unchanged; E80 D-064/D-065/D-071/D-072 and Reference protected. | OA | D-083/84/85 published; D-082/E80/Reference preserved | G | S | CANDIDATE | E6 | MG |
| **SA-03** | Both parent and planting compact four-file specs reviewed/APPROVED for implementation; #584 PASS docs-only not mistaken for spec acceptance. | RP | parent + planting specs APPROVED | G | S | BLOCKED | E9 | MG |
| **SA-04** | Signed new Compact world manifest, tree roster/H1 buffer/path width/G1 positions and content version with source/native witness. | OA | signed manifest + roster/site native witness | S | W | BLOCKED | E1 | T2 |
| **SA-05** | Signed save three-profile contract respected, ISO-58 CG-TRIG hold and independent safe-save implementation gates closed before Compact active. | AS | signed save contract + ISO-58 gates before activation | V | P | BLOCKED | E7 | T3 |
| **AC-01** | Signed world/graphics identity reconciled with actual new Compact ID, H1/Route A/G1, 14 omissions + 8 buffer and 189/175 distinction; native roster/census PASS. | OA | identity/roster/census native PASS | S | W | BLOCKED | E6,E1 | T2 |
| **AC-02** | GQ-07 selected scene architecture removes old 2.6m planks from both visible and gameplay source Compact, and 1.2m candidate or signed replacement is physically walkable under actual spline,… | OA | scene architecture walkable; no planks | S,H | W | BLOCKED | E6,E3 | T2 |
| **AC-03** | New 720p and 1080p true Unity view from H1 and first bend shows natural nonroad path and sensible needle floor, positive color/grayscale distinction with forestry UI. | OA | native H1/bend look, colour + grayscale | H | W | BLOCKED | E3,E6 | T2 |
| **AC-04** | Pre/post native assert authoritative VegetationSites/clearance preview marker IDs/target counts/summary, ecology light and planting eligibility unchanged; separate Compact… | RP | mask leaves clearance/ecology unchanged | H,R | W | BLOCKED | E3 | T2 |
| **AC-05** | Compact has no rendered or gameplay-consumed historical plank/headland/approach/cutting system; old Legacy40/E80 pre-approach/approach, Rack V2 off/on and frozen Reference have… | RP | no historical plank/headland consumers | S,H | W | BLOCKED | E2 | T2 |
| **AC-06** | Exactly 2 Oak+2 Beech appear only after Completed orders; genuine ~0.6 m juveniles identifiable by morphology at 2–3 m and findable at 4–6 m through separately approved marker/inspection… | RP | 2+2 only after Completed; 2-3 m form, 4-6 m findability | J,R | P | BLOCKED | E4,E6 | T3 |
| **AC-07** | Approved first-party/source provenance, asset/LOD/hash/licence/storage report, no third-party dependence; D-032 followed and prior unadopted assets not silently imported. | AS | rights/provenance before import | S,H,J | S | BLOCKED | E5 | OW |
| **AC-08** | No-op/negative and real camera/URP/fog feasibility checks if lighting changes; otherwise neutral state and old-world global assets remain untouched. | RP | camera/URP/fog no-op feasibility | H | W | BLOCKED | E3 | T2 |
| **AC-09** | New Compact full baseline and after-performance protocol, same machine/quality/LOD/resolution/warm-up/repeats and accepted criteria; source graph+memory data provided. | RP | same-machine performance protocol | ALL | W | BLOCKED | E2 | T2 |
| **AC-10** | Initial all-Sitka and later true planted/review/no-work/intervention visuals correspond to actual annual history without fabricated recruitment or H1 harvest. | OA | visuals correspond to annual history | S,J,R | P | BLOCKED | E1,E6 | T3 |
| **AC-11** | Native profile isolated save/load, old world v1–v24 and frozen Reference content safety; incompatible content-version refuses without mutation, and no unapproved schema/Prefs changes. | AS | save/load + v1-v24 + version refusal | V | P | BLOCKED | E7 | T3 |
| **AC-12** | Independent qualified feature review and explicit owner visual acceptance, Manager integration verdict, correct no-Windows-build gate. | AS | independent feature review + owner acceptance | G | W | BLOCKED | E9 | RV+OW |
| **G573-M1** | vegetation display mask after site generation, clearance target/preview unchanged (FR-10/11, AC-04). | OA | mask after sites; clearance unchanged | H,R | W | BLOCKED | E3 | T2 |
| **G573-M2** | Owner-selected separate Compact scene (GQ-07 A) excludes historical 2.6m plank/track objects, clones and corridor/Stand Map/approval consumers entirely, with unchanged old states; the… | OA | separate scene excludes planks/consumers | S | S | CANDIDATE | E6 | MG |
| **G573-M3** | shared Forest Ground/Path materials & editor menu never edited/run; distinct compact materials (FR-08, AC-05). | RP | never edit/run shared ground/path menu | H | S | CANDIDATE | E3 | MG |
| **G573-M4** | camera Solid Colour/no URP additional data gate; global fog/sky not assumed, shared camera isolation protected (FR-08/17, AC-08). | RP | camera solid colour / fog isolation | H | W | BLOCKED | E3 | T2 |
| **G573-M5** | fully masked cells honest; verify rendered+suppressed accounting, deterministic Jitter/reload, rush anchors and Compact-scoped grass verifier while keeping legacy tests unchanged (FR-11,… | RP | masked-cell honesty + deterministic Jitter | H,R | W | BLOCKED | E3 | T2 |
| **G573-M6** | EVERY T-02..T-09 source/Unity/asset task blocked on spec APPROVED, owner decisions/lease (task list and AC-12). | AS | every asset task blocked on APPROVED spec | G | S | CANDIDATE | E9 | MG |
| **G573-M7** | known incomplete gitignored Ultimate Nature not required; provenance/source+pack-state pinned (FR-12/18, AC-07). | RP | Ultimate Nature not mandatory | H,J | W | CANDIDATE | E5 | T2 |
| **NEG-01** | choose 2.4m width with unchanged G1 → assert source predicted G1 collision and refuse/signature invalid, not omit plants or delete original Sitka. | RP | 2.4 m width would collide → refuse | H,S | W | BLOCKED | E3 | T2 |
| **NEG-02** | known source FAIL with existing ScenarioProtectionVisuals FAILED 28°/+0.25 m tube: O1 intrudes 0.141 m into 1.2 m core, O2 fails 0.20 m fringe by 0.155 m. | RP | known 0.141 m O1 / 0.155 m O2 FAIL | J,H | W | BLOCKED | E4,E1 | T2 |
| **NEG-03** | mask toggle leaves source VegetationSites/clearance target/preview/summary identical; fully masked cell has rendered+suppressed balance, rush behaviour and deterministic save/reload;… | RP | mask toggle preserves source | H,R | W | BLOCKED | E3 | T2 |
| **NEG-04** | default Compact with historical Dirt Path 2.6 m planks still rendering/consumed FAILS; historical E80/Legacy40 source, clone, active/render state or shared material mutation FAILS;… | RP | historical planks rendering = FAIL | S,H | W | BLOCKED | E2 | T2 |
| **NEG-05** | third-party Ultimate Nature absent + installed source state → compact first-party assets still render as designed; not dependent on local ignored package. | RP | third-party pack absent still renders | H,J | W | BLOCKED | E5 | T2 |
| **NEG-06** | Pending and failed planting orders create no phantom juvenile/shelter success; only real Completed order visual, reload retains true state. | RP | no phantom juvenile/shelter | J,R | W | BLOCKED | E4 | T3 |
| **NEG-07** | wrong content-version/legacy profile refused before mutate; old saves/Prefs and Reference unchanged. | AS | wrong version/profile refused before mutation | V | P | BLOCKED | E7 | T3 |
| **NEG-08** | cam post-process disabled/solid-color sky remains no-op until proven; no false "skybox works" pass. | RP | post-process/sky no-op until proven | H | W | BLOCKED | E3 | T2 |
| **V-01** | Review result for this DRAFT authored by independent nonauthor, exact four-file source review, with blockers/majors/minors and response. | AS | independent nonauthor four-file review | G | S | CANDIDATE | E9 | RV |
| **V-02** | Owner look choices and D-032 rights/storage decision recorded; open choices not marked accepted by Manager proxy. | OA | owner look + D-032 choices recorded | G | S | BLOCKED | E5,E6 | OW |
| **V-03** | The current Overall Manager (presently #700), acting within latest direct owner decisions and canonical authority, issues explicit APPROVED spec status after review and bound decisions; not… | AS | Manager APPROVED spec status after review | G | S | BLOCKED | E9 | MG |
| **V-04** | Each later source or Unity worker registered claim/worktree/branch/HEAD and exact file allowlist, physical lease where needed; user dirty Primary preserved. | AS | registered claim/worktree/lease per worker | G | S | CANDIDATE | E12 | MG |
| **V-05** | Implementation code/art/assets/scene and Unity verification on actual Compact are inspectable and independently reviewed; no history/source overwritten. | AS | inspectable implementation + independent review | G | W | BLOCKED | E9 | RV |
| **V-06** | Exact source delta and Tier2 native result reviewed by the current Overall Manager (presently #700); no unapproved weakened tests, no unnecessary whole-milestone Tier3 until… | AS | Manager review of exact delta/Tier2; no weakened tests | G | W | BLOCKED | E9 | MG |
| **V-07** | Owner has personally viewed/accepted final presentation in Unity and separately authorised any Windows build after Compact personal playtest. | OA | owner personal Unity acceptance + Windows authorisation | G | P | BLOCKED | E6 | OW |
| **V-08** | Dashboard and Drive mirror publication explicitly verified if claimed; otherwise NOT VERIFIED. | AS | dashboard/Drive verified if claimed | G | P | BLOCKED | — | OPS |

## 5. Duplicate, conflicting or stale guidance register

These are source-level tensions the Matrix surfaces. None is resolved here; none changes a spec byte.

| # | Conflict / duplication | Exact source | Consequence and required resolution |
| --- | --- | --- | --- |
| C-01 | **GQ-01 palette / overall look** vs D-085 OD-02 deterministic tree-ID variation | Graphics `FR-16` (owner-chosen W look from native views), `AC-03`, `AC-08`; D-085 OD-02 | The look is an owner choice. Until chosen or marked PROVISIONAL with reauthor, H material bytes cannot be authored. Deterministic per-ID variation is already an owner direction but does not choose the palette. |
| C-02 | **GQ-08 H1/path single-plane approach** | `FR-05`, `FR-07`, `NEG-01`; #688 §4.2 (one plane, inward masks) | Whether one analytic plane with named regions satisfies "separate footprints" is unresolved. #688's inward-only mask is the only reviewed method; native footprint/texel proof still required. |
| C-03 | **GQ-09 C0 vs T1 and the 0.190 m cap** | `FR-04`, `FR-06`; frozen #676 C0 `.095 m` cap; #688 §5 T1 table | T1 **fails** the frozen `.095 m` deviation test by design (`.188148148 m` departure). It cannot be called conforming. Choosing T1 creates a new versioned candidate and requires the owner to accept a new cap; C0 remains the comparator. |
| C-04 | **GQ-10 A8 endpoint** | `FR-04`; #688 §5 A8 `(-2.10, 13.50)`; D-085 OD-10 "almost all the way to the end", no numeric setback | OD-10 gives intent, not a coordinate. A8 and its 1.5 m centre setback remain unsigned. Any movement must not silently move G1 sites or omit stems. |
| C-05 | **GQ-11 guard silhouettes / inspect cue** | `FR-15`, `NEG-02`, `AC-06`; #698 A/B look + 4–6 m cue; D-085 OD-11 | Current source failed O1 still intrudes **0.141 m** into the 1.2 m core; O2 fails the 0.20 m fringe by **0.155 m**. These stay known FAILs until the Compact representation is natively measured. No cue may extend 3.5 m/8 m reach or flag unplanted sites. |
| C-06 | **"True planting" vs "no Year 0 juveniles/guards"** | R-028, R-102, CIP-008, CIP-015; `FR-15`, #698 opening | Not a contradiction but a sequencing trap: the starting world has four coordinate records and **zero** juveniles/guards; genuine 2+2 planting and guard states exist only after real orders/annual resolution. Any art preview must be a separate fixture and must never be serialized into the starting scene or placement JSON. |
| C-07 | **CT-3 annual-review hold vs generic acknowledgement wording** | R-056..R-061; #677 T-02/T-03; #684 table | Signed policy reads the existing Annual Review control as the only covering-year acknowledgement and CT-3 as a hold on every advance route. Older "generic acknowledgement" wording is read through #677/#684, not replaced. Still **NOT IMPLEMENTED**. |
| C-08 | **D-032 rights vs already-public bytes** | Decision Log D-032; #704; #708 F-1; corrected sheet §4 | Public visibility, archive hashes and 84/84 FBX matches are custody/provenance leads, not title. A branch push to `jermur788/CCF` publishes new bytes even without a PR/merge. |
| C-09 | **OD-16 M-A-SCOPE deferred vs "meaningful/substantive" wording** | R-051, R-052, R-094, R-104; D-085 OD-16; #684 "unchanged" | MA-2..MA-6 describe evidence but do not decide whether one acted-on stem suffices. R-051/R-094 cannot be checked until the owner decides M-A-SCOPE. No three-cycle quota and no invented stem count. |
| C-10 | **Stale graphics narratives** | Graphics spec passim (collection truck, possible shared scene, H1 stump question, D-083 described as unpublished); #688 §2 authority reconciliation | #688 documents that these passages are stale against D-085 and #675. They remain in the DRAFT and must be reconciled by the formal spec-approval act, not silently edited now. |
| C-11 | **189 → 175 → 57/50/31/37** | R-006, R-102; E1 roster `dac06a3b…`; E2 §"Four Sitka families" | 189 is the original research inventory, 175 the owner-selected roster after 14 pre-layout omissions, and 57/50/31/37 the **proposed** family split under #676's array order. None is a native scene count. |
| C-12 | **R-102 "no seed donors" vs R-106/CIP-020 recruitment wording** | R-102, R-106, CIP-020, CIP-021 | Consistent if read precisely: the start has no Oak/Beech adults/juveniles/cohort/seed donors, natural recruitment is not required for success, and any modelled recruitment must not be presented as guaranteed. Do not cite this as permission for automatic broadleaf recruits. |
| C-13 | **FR-12 "reuse tracked first-party" vs D-032 OPEN** | `FR-12`, `FR-18`, `AC-07`; #704/#708 | Reuse is conditional on actual rights and dependency closure; "tracked" is not "first-party rights-proved". FR-12 remains blocked while FR-18/AC-07 are open. |
| C-14 | **Signed §4.2 / #677 policy vs Compact runtime** | R-011..R-025, R-080..R-092; #677; #684; D-085 | The save policy is signed but **NOT IMPLEMENTED** for Compact. No matrix row treats signature as implementation, and no §4.2 byte may change. |
| C-15 | **Graphics `FR-19/FR-20` "no Windows build" vs Phase 3 scope** | `FR-20`, `G573-M6`, `V-07`; D-085 | Windows remains excluded until the owner personally tests the exact Editor candidate and separately authorises a build. No Tier S/H/J grant implies Windows. |

## 6. Exact S / H / J proposed source, scene and asset boundary

**Every path below is PROPOSED PATH, NOT AUTHORISED.** Absence was re-checked at 2026-11-18T01:25:03Z on `origin/main` `aaa6254b` and on D-073 `55863f1b`: all five Tier S paths, the `Scenes` folder, `Art/ScenarioOneCompact30.meta` and every `ScenarioOneCompact30` path are **absent** on both. This is a dated observation, not a reservation or lock.

### 6.1 Tier S — five and only five scene files (one writer)

| # | Exact path | Role | Witness contribution | Proposed cap |
| --- | --- | --- | --- | --- |
| S-1 | `Assets/ForestPrototype/Scenes.meta` | New folder meta; **one named owner** | Folder GUID binding | text, <1 KiB |
| S-2 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` | Separate inert scene: flat 30×30 m ground, H1/path visual binding, 175 tree references, four empty G1 records, spawn pose | rows 12, 83 contribution only | scene text; measured at authoring |
| S-3 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity.meta` | New scene GUID | row 83 binding | text |
| S-4 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json` | Derived placement/source/recipe/dependency record, **SHA-256-bound to reviewed successor M/C** | static reconstruction checks | <1 MiB proposed |
| S-5 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json.meta` | New data-asset GUID | row 83 binding | text |

Tier S **closes none** of the eight frozen rows. It contributes to row 12 (presentation fixture) and row 83 (scene/GUID/placement bytes) only. Ground/H1 (30/44) need H; guards (73/74/75) need J plus a separate witness; row 10 needs actual rights.

### 6.2 Tier H — proposed exact #688 entrance/path art allowlist

Root `E = Assets/ForestPrototype/Art/ScenarioOneCompact30/EntrancePath/`. Shared parent `Assets/ForestPrototype/Art/ScenarioOneCompact30.meta` has **exactly one named first writer**, either H or J, never both.

| Exact proposed path | Role | Depends on |
| --- | --- | --- |
| `Assets/ForestPrototype/Art/ScenarioOneCompact30.meta` | Shared art parent meta (one writer) | P-4 custody and exact creator |
| `E.meta`, `E/Editor.meta`, `E/Source.meta`, `E/Meshes.meta`, `E/Textures.meta`, `E/Materials.meta`, `E/Prefabs.meta`, `E/Docs.meta` | Folder metas (8) | same |
| `E/Editor/CompactEntrancePathBake.cs` + `.meta` | Deterministic Editor-only plane/mask/texture baker; no auto-run; no scene/save/registry mutation | GQ-08; source authorship; **is a source script, not a free tool** |
| `E/Source/EntrancePathRecipe.json` + `.meta` | Pinned curve/C0-T1, constants, seeds, importer settings | GQ-09; reviewed successor world input |
| `E/Meshes/CompactGroundEntrancePath.asset` + `.meta` | Four-vertex/two-triangle ground mesh; **no collider** | one plane method |
| `E/Textures/CompactGround_H1_Path_BaseDetailMask.png` + `.meta` | Original 2048² RGBA macro atlas / alpha detail mask | procedural, no donor pixels |
| `E/Textures/CompactNeedleSoil_Detail.png` + `.meta` | Original 512² periodic detritus detail | procedural |
| `E/Materials/CompactGroundEntrancePath.mat` + `.meta` | One Compact-only URP Lit material; shader GUID `933532a4fcc9baf4fa0491de14d08ed7`, AssetVersion script GUID `d0353a89b1f911e48b9e16bdc9f2e058` | authenticated installed URP bytes |
| `E/Prefabs/CompactEntrancePathVisual.prefab` + `.meta` | Identity MeshFilter+Renderer; **no collider/MonoBehaviour/runtime** | scene owner binds it |
| `E/Docs/EntrancePathAssets.json` + `.meta` | Author/input/rights attestation, GUIDs, bytes, hashes, notices | P-4/S-1 |
| `E/Docs/PROVENANCE.md` + `.meta` | Reproducibility and native receipt index | P-4/S-1 |

Proposed but **not approved** budget: two PNGs ≤20 MiB on disk, all other new source/assets ≤0.5 MiB, total family ≤**20.5 MiB**; expected uncompressed texture payload ≈17.33 MiB. Source master/durable custody proposed at `/media/jer/Files/CCF/Archives/ScenarioOneCompact30/EntrancePath/Source/` (not on ZX20); no LFS or `.gitattributes` change.

### 6.3 Tier J — proposed exact #698 juvenile/protection allowlist

Root `J = Assets/ForestPrototype/Art/ScenarioOneCompact30/JuvenilesProtection/`. Same single shared-art-parent writer rule.

| Exact proposed path | Role | Depends on |
| --- | --- | --- |
| `J.meta`, `J/Models.meta`, `J/Prefabs.meta`, `J/Materials.meta`, `J/Docs.meta` | Folder metas (5) | one writer |
| `J/Models/OakTransplant.fbx` + `.meta` | Sessile Oak 0.60 m stock transplant, ≤1,500 tri, 2 slots | GQ-11 silhouettes; rights |
| `J/Models/BeechTransplant.fbx` + `.meta` | European Beech 0.60 m stock transplant, same effort | same |
| `J/Models/ProtectionShelter.fbx` + `.meta` | Three tube states + two stake meshes, ≤512 tri | GQ-11; r ≤ 0.10 m |
| `J/Prefabs/OakTransplant.prefab` + `.meta` | Oak juvenile root/scale/pivot | native import |
| `J/Prefabs/BeechTransplant.prefab` + `.meta` | Beech juvenile root/scale/pivot | native import |
| `J/Prefabs/ShelterEffective.prefab` + `.meta` | 20 panels to 1.20 m, intact stake | state binding |
| `J/Prefabs/ShelterExpired.prefab` + `.meta` | Mixed 0.85/1.20 m, intact stake | state binding |
| `J/Prefabs/ShelterFailed.prefab` + `.meta` | Broken 0.28/0.35 m, snapped stake | current 0.141 m FAIL correction |
| `J/Materials/JuvenileStem.mat` + `.meta` | Low-gloss stem | URP Lit |
| `J/Materials/OakLeaf.mat` + `.meta` | Geometric lobed leaf, zero texture | native backface/normals |
| `J/Materials/BeechLeaf.mat` + `.meta` | Oval wavy leaf, zero texture | native backface/normals |
| `J/Materials/ShelterTube.mat` + `.meta` | Matte muted olive (A) or provisional alpha 0.55 (B) | GQ-11 A/B decision |
| `J/Materials/ShelterStake.mat` + `.meta` | Subdued brown stake | GQ-11 |
| `J/Docs/SourceManifest.md` + `.meta` | Author, inputs, hashes, export recipe, rights ledger | P-4/S-1 |

Proposed editable masters (outside Unity Assets): `JuvenilesProtection.blend`, `build_shelter.py`, `SourceManifest.md` under `/media/jer/Files/CCF/Archives/ScenarioOneCompact30/JuvenilesProtection/Source/`. Proposed but **not approved** budget: 2 MiB editable source + 1 MiB FBX + 0.25 MiB Unity assets = **3.25 MiB**, plus evidence.

**Year 0 is NO:** no planted Oak/Beech juvenile instance, no effective/expired/failed guard instance, no shelter collider, no phantom planting credit. The three guard states and the two juvenile species are future state-bound visuals only.

### 6.4 Separate guard/juvenile witness lane (proposed, not authorised)

Rows 73/74/75 cannot be closed by the five-file scene. Proposed, exact and optional:

- `Tools/Verification/Sen1Compact30GuardWitness/` — one disposable Editor tool + its exact paths, copy/run/remove with zero residue under `Assets/ForestPrototype/`;
- an ephemeral preview scene path **stated in the grant**, not referenced by `ScenarioOneCompact30.unity` or the placement JSON;
- no auto-start, no build-settings entry, no save/profile writes, no runtime materials/meshes embedded in the delivered scene.

If the owner does not grant this lane, rows 73/74/75 stay open and no guard state can be called native-proven.

### 6.5 Authoring/readback tool (proposed, not authorised)

Proposed only if manual placement is rejected: `Tools/Verification/Sen1Compact30SceneAuthoring/` containing one `.cs` + `.meta` and a receipt, run once against the pinned placement JSON, then removed with its generated `.meta`. It must be read-only on the JSON, deterministic, no `UnityEngine.Random`, no auto-start, no save/profile/registry access, no scene-embedded asset creation, and it must emit a before/after tracked-file diff. This path is **disjoint** from #690's `Tools/Verification/ScenarioReferenceVerification.cs` and `Tools/Verification/StandGeometry/StandGeometryVerification.cs`.

### 6.6 Non-overlap with #690 and historical worlds

The 17 #690 A0 paths are C# save/stand/manager/UI files under `Assets/ForestPrototype/` plus the two `Tools/Verification/` files above. **No S/H/J path in this document equals any #690 path**, and S does not touch `ForestTest.unity`, `Assets/Scenes/`, build settings, packages, project settings or the historical 52 dirt-path roots. Historical Legacy40, Enlarged80 Rack V2/MFM, frozen Reference Future v1 and saves v1–v24 remain read-only.

### 6.7 Rights/storage stage per tier (unsigned)

| Tier | Stage P before any byte | Stage S before signature | Stage X per channel |
| --- | --- | --- | --- |
| S | P-2 creator/rights-holder of scene+placement; P-3 push per tier; P-5 physical route; P-6 cap | S-1 scene source + GUID/dependency closure; S-2 URP notices; S-3 channel scope | X-1..X-7 as applicable |
| H | P-4 creator, inputs, editable master custody; 20.5 MiB cap **not** approved | S-1 imported asset/GUID/hash; S-2 URP/notices | X-1..X-7 |
| J | P-4 creator, silhouettes, masters; 3.25 MiB cap **not** approved | S-1 imported asset/GUID/hash; S-2 URP/notices | X-1..X-7 |

The four existing Sitka families (84 FBX, 69,546,784 bytes = 66.33 MiB; ~107.7 MiB with textures/media) are already public in ordinary Git, but **byte possession is not title** (#704/#708). Their P-1 internal-use grant is still unanswered.

## 7. The 175-placement authoring problem: manual vs disposable tool

| Criterion | Manual-only in the Editor | Separately consented disposable tool |
| --- | --- | --- |
| 175 ID-bound transforms × position/height/DBH/crown/family/LOD | Extremely tedious; high ID-to-object mismatch risk; hard to audit | Deterministic from pinned JSON; exact ID binding; machine-checkable |
| Reproducibility | Depends on operator discipline | Re-runnable; can emit per-ID receipt |
| Regression risk | Accidentally moving a stem or site is easy | Input-read-only; can assert zero site movement |
| Blast radius | No new script | New `.cs` + `.meta`; possible Unity-generated tracked writes |
| Auditability | Visual only | Before/after tracked diff + per-ID readback |
| Failure mode | Silent mismatch discovered late | Explicit STOP on any dependency mismatch |
| Current authority | Allowed by "manual default" | **Not authorised**; requires exact named grant |

**Recommendation for the future owner/Manager:** prefer **manual-only** for the first minimal fixture if it can be done honestly, because it adds no script surface. If manual placement cannot guarantee ID fidelity for 175 trees, grant the **single named disposable tool** above with its own exact allowlist, run/remove receipt, and a hard STOP on any tracked file outside S. Do **not** treat the tool as implied by the scene grant. Either way: no new 36-cell runtime serialization, no scene-embedded materials/meshes, no package/build-settings changes, and no auto-start harness left in a playable build.

## 8. Smallest proportionate native / physical evidence ladder (all NOT RUN)

### Tier 1 — static/authoring (documentary or Editor-independent)

| Check | Scope | Current |
| --- | --- | --- |
| 175 roster IDs, 19 fields, 36 cells vs frozen candidate | static recompute + SHA-256; roster `dac06a3b…`, cells `2441926a…` | static PASS by #718/#676; **not re-run natively** |
| Five S paths absent; folder/meta ownership | `git cat-file` on future parent immediately before grant | PASS at 2026-11-18T01:25:03Z (snapshot only) |
| Placement JSON derived from reviewed M/C | key schema, pins, hash binding | **NOT RUN** (file does not exist) |
| G1 four sites, zero planar tolerance | exact coordinates `(1.17,−0.47) (1.57,−2.73) (3.43,−0.07) (3.83,−2.33)` | static source check PASS; native site placement **NOT RUN** |
| H/J path lists disjoint, parent meta one writer | allowlist diff | PASS in this document; no bytes exist |
| No embedded material/mesh, no bootstrap, no profile/save identity | scene YAML inspection after authoring | **NOT RUN** |

### Tier 2 — isolated content-native (one authorised Unity lease)

| # | Check | Frozen rows addressed | Current |
| --- | --- | --- | --- |
| 2.1 | Import/open only the Compact scene; no historical clone | 83 | NOT RUN |
| 2.2 | 175 prefab bindings + all mesh/material/submesh/LOD path/GUID/fileID | 10, 12 | NOT RUN |
| 2.3 | Visual height vs persisted height, independent DBH at 1.3 m, crown extent, defect flag consistency, across all three LODs for four families | 12 | NOT RUN; uniform `ModelForHeight` scale is a known mismatch risk |
| 2.4 | Imported material/shader/keyword resolution; no missing/pink; active PC renderer | 30, 44, 73–75 | NOT RUN; static URP identity only |
| 2.5 | Ground/H1 surface, path core 1.2 m + ≤0.20 m verge, texel/footprint bounds, off-path walkability | 30, 44 | NOT RUN |
| 2.6 | Guards effective/expired/failed: r ≤ 0.10 m, 0.141 m O1 correction, no Year0 guard in scene; separate witness | 73, 74, 75 | NOT RUN; current source FAIL stands |
| 2.7 | Player capsule 0.30 m + 0.03 m skin walk/turn at H1, A7, A8, O1/B2; branch/trunk collision; free off-path | 12, 30, 44 | NOT RUN |
| 2.8 | 36 cell IDs/fields and 175×19 readback from the actual fixture | 83 | NOT RUN |
| 2.9 | 720p/1080p colour + grayscale H1/bend/interior/G1; no old E80 capture substitution | 12, 83 | NOT RUN |
| 2.10 | Chronology: no pre-world H1 omission becomes Fell/timber/history; no automatic recruits/marks | 83 | NOT RUN |
| 2.11 | Historical regression: Legacy40, Enlarged80 Rack V2/MFM, Reference Future v1, v1–v24 unchanged | 83 | NOT RUN |
| 2.12 | Performance baseline/after on the same machine | — | NOT RUN; no invented FPS threshold |

### Tier 3 — integrated runtime (after signature only)

Genuine 2 Oak + 2 Beech Pending→Approved→Completed orders at exact sites; refusal of off-site/wrong-species/duplicate; real stock/cash/shelter; annual ecology/browse/protection; Annual Review event9 and CT-3 hold; save/load and historical compatibility; owner personal Editor test; Windows only after separate authorisation. **All NOT RUN.**

### Eight frozen blockers — unchanged audit

| Row | Field | Tier that could contribute | Status |
| --- | --- | --- | --- |
| 10 | `B-W02-RIGHTS` tree prefab/LOD rights | S (scene dependency) + P/S rights | **BLOCKED** |
| 12 | `B-W02-SCALE` visual height/DBH/crown/defect | S + Tier 2 witness | **BLOCKED** |
| 30 | `B-W13-RENDER-DEPS` terrain surface/material | H | **BLOCKED** |
| 44 | `B-W13-RENDER-DEPS` H1 surface/edge | H | **BLOCKED** |
| 73 | guard effective | J + witness | **BLOCKED** |
| 74 | guard expired | J + witness | **BLOCKED** |
| 75 | guard failed | J + witness | **BLOCKED** |
| 83 | `B-W14-WHOLE` whole-world identity | S+H+J + all above + signature | **BLOCKED** |

`world_digest` remains **null**; 0 signed / 80 candidate / 8 blocked. Tier S closes none.

## 9. Read-only physical snapshot (as-of, not a lease)

Observed 2026-11-18T01:24–01:25Z on `jer-laptop`:

- No Unreal/Unity Editor process observed (`pgrep` returned none); latest `state/unity/history.jsonl` event is a **release** at 2026-11-15T22:44:25.912374Z; no current `lock.json` record observed. This is an observation, **not** a durable free-lock claim.
- Ops board `board/active.md` SHA-256 `b70eba8d65d31ffabc449f41a4b42c351a7c6308ef220eff6f8890aa0ede12cc`; `lanes.json` SHA-256 `e95e58785f9047d42e5720f2aff21acb1d6f294c8e7904eb93ae9dfffcb4ca22`.
- #690 A0 worktree `/media/jer/ZX20/CCF-Sen1-690-A0-20261117` is on `integration/sen-1-d073-shared-save-a0-20261117-v1` at `55863f1b7719011bba16b3f3ab377e379c0b988a` with **empty porcelain**; row `CCF-690-SEN1-SAVE-A0` is BLOCKED / SOURCE START HELD. No source edits observed.
- ZX20 free 50 GiB; root filesystem only ~2 GiB free at observation (environment fact; this documentation run wrote only to `/media/jer/Files/CCF-Worktrees`).
- #652 remains **SOURCE APPROVED / LIVE INSTALL HELD**. No Ops row, lane, board or lease was created or changed by this run.

## 10. Absent IDs, ambiguity and limitations

- **No absent or duplicate requirement ID.** 115 + 33 + 66 = 214 unique IDs parsed from the exact blobs above; every ID appears once in the matrices.
- The graphics checklist includes `G573-M*`, `NEG-*` and `V-*` IDs in addition to `FR/WF/SA/AC`. They are original checklist entries, not additions by this crosswalk.
- Requirement text is multi-conjunct; the quoted first sentence is a locator aid, not a replacement. All original wording remains in the DRAFT files.
- The matrix's `S/W/P` gate and `CANDIDATE/BLOCKED` status are **this review's assessment**, not a spec field and not a Manager verdict.
- No native, Unity, Blender, import, collision, render, performance, save or Windows evidence was produced. Every `NR` and `T2/T3` entry is honestly **NOT RUN**.
- The independent #721 reviewer verdict is **APPROVED (document quality)** with three non-blocking wording minors; Manager acceptance is still pending and does not itself change any item here.
- Dashboard/Drive synchronization remains **UNVERIFIED**; no mirror was read or written as authority.

