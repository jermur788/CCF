# #460 — Compact one-rack native feasibility study result: BLOCKED at Unity lease

13 November 2026. **Result: NON-UNITY PREFLIGHT HANDED OFF; NATIVE STUDY INCOMPLETE / BLOCKED — UNITY LEASE.** No authentic compact candidate has been generated. C0-03/C0-04 native acceptance is **NOT MET**. This report is a bounded blocked result permitted by the binding packet, not a feasibility PASS, DRAFT approval or production implementation.

## Worker, authority and exact source

- Actual worker: **OpenAI GPT-6.1 Sol, High**, Codex harness in local T3 Code on Linux. Thread `b16301b9-6641-4b7a-8bad-e41944f217dd`. No delegation or second worker.
- Direct user assignment: take #460, register before work, obtain #424's exclusive Unity lease, preserve source/saves, publish this report only; no implementation/refactor.
- [Binding packet](https://github.com/jermur788/CCF/blob/docs/manager-425-459-native-feasibility-gate-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/MANAGER-425-459-NATIVE-FEASIBILITY-STUDY-PACKET-20261113.md), blob `119961ac0eb445188ee6c920b21b7a2590a505a8`; [#459 review](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-458-independent-delta-review-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/CLAUDE-ONE-RACK-458-INDEPENDENT-DELTA-REVIEW-20261113.md), blob `9f8a18e3ef8ca17406ad6f15231b0b8d85d2c544`. #460's Opus-strategy addendum was read: planning evidence only, no T-0/F-1/F-2/F-3 release.
- Research branch: `research/compact-one-rack-native-feasibility-20261113-v1`. Starting source HEAD: **`6c13a9e182774945bf391bc8d4845ce74d3e5e88`**. Canonical context snapshot within that baseline: `62e39ec621e38a84030c08b35a171e297056c9ed`. The packet does not provide a separate context SHA; this is the latest commit affecting the canonical context/instruction set, identified locally, not invented Manager approval.
- Actual isolated worktree: `/media/jer/ZX20/CCF-CompactOneRackFeasibility-20261113`. Absent before setup; safely created from the precreated remote research branch; clean at the exact baseline before and after preflight. Separate from #443, no Library copied, no Editor opened.
- Engine required by source: **Unity 6000.6.0f1 (f7f8ed4d1e24)**. Manifest specifies URP 17.6.0, Input System 1.20.0, Test Framework 1.8.0. These are inspected project settings, **not a verified Editor/import run**.
- Final report commit/remote readback is recorded in #460's publication comment. Source research stays pinned to the starting SHA; later main coordination commits are not incorporated as simulation changes.

## Registration and the unpassed lease gate

[Actual worker registration](https://github.com/jermur788/CCF/issues/460#issuecomment-6856534720) preceded research edits/execution. [Checkout receipt](https://github.com/jermur788/CCF/issues/460#issuecomment-6856585129) names the actual clean checkout. Claim remains **BLOCKED — UNITY LEASE / NON-UNITY PREFLIGHT**, not RUNNING native.

[Explicit prospective #424 request](https://github.com/jermur788/CCF/issues/424#issuecomment-6856535287) asks for the sole-writer coordination entry, confirmation of #443/user custody, lane registration instructions and a bounded exclusive lease for one disposable native fixture. **No affirmative #424 grant was received at this handoff.** Latest live GitHub coordination readback had blob `bdadea84f17514b0bd06a109fa6e33af97fbe15f`, including the separately user-authorised HP #461 entry; it contained **no #460 entry**. That other worker's single-entry exception does not authorise this worker to edit coordination or acquire Unity.

Physical preflight and end checks both returned `unity-lock: free; metadata: null; unity_editor_pids: []`. The designated new worktree is absent from deployed `CCF-Ops/lanes.json`. Free physical resources do not replace #424 permission or Ops registration. No acquire/force/release was attempted, no process was killed, and there is no Editor exit code because no Editor was launched. Initial free space was about 60 GB on ZX20 and 155 GB on Files; no cold Library was built.

#443's latest comments reported #450 reviewed and awaiting Manager disposition. Its actual worktree remained clean at `acb4e1111fdc9aa9b295413038eda4e03fe56402`; no ownership transfer or new source base was supplied to this study. Read-only comparison of the generator, geometry, ecology and economy-adapter files found the generator clearance predicate differs: #443 additionally excludes `IsRetiredFromStartingWorld` buildables. The other three sampled files matched. This is a **four-file comparison, not whole-branch equivalence**. Consequently baseline scene/buildable clearance must be recorded explicitly in any future study; #443's starting stand cannot be silently substituted.

## Verified source constraints and measurement design

Evidence classes below mean **DIRECT SOURCE**, **MODEL DERIVATION / UNVERIFIED IN UNITY**, **GAMEPLAY CALIBRATION**, **UNMEASURED**, or **BLOCKED**. There are no DIRECT UNITY MEASUREMENTS in this report.

| Inspected fact — DIRECT SOURCE | Consequence for the permitted native study |
| --- | --- |
| [StandGeometryModel](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/StandGeometryModel.cs) recognises only Legacy40 and Enlarged80. New-game policy selects Enlarged80; the Editor override also rejects unknown IDs. `ApplyStandGeometry` rejects unknown models and sets the ecology grid from that model. | 40 m / 64 cells and 80 m / 256 cells are comparison baselines. Neither is a new compact world. Changing only generator width/depth cannot establish a coherent new property. |
| [ForestStartingStand](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ForestStartingStand.cs) is a MonoBehaviour. It generates from lattice IDs, deterministic FNV jitter/omissions and live scene clearance anchors, then calls real canopy and seed recomputation. Defaults include age 20, 1.9 m spacing, 21 lattice positions per axis and target 336 on the legacy branch; Enlarged80 has its own core/outer route. | These are source calibration inputs, not measured compact stock. There is no stand-generation random seed slider: ID-derived hashes and actual anchor positions matter, separately from ecology's simulation seed. A managed port would need a distinct evidence label and native parity. |
| [RackPlan](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs#L20) gates V2 on Enlarged80 and requires exactly five racks at Confirm. [RackGeometry](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/Racks/RackGeometry.cs) has a 2.4 m default corridor; [headland geometry](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs) has fixed Enlarged80 coordinates/clearance. | A single drawn rack is not a confirmed one-rack starting world. The 2.4 m width and old headland coordinates are source settings, not approved compact values. Do not weaken the five-rack gate for this study. |
| [Approach](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/StandApproachPath.cs#L35) is generated for Enlarged80 only. [StandMapSource](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/UI/Map/StandMapSource.cs) reads ecology bounds, actual tree positions and scenery. | A proposed smaller boundary may disconnect or omit the existing approach/headland. Map appearance, engine geometry and physical passability need native agreement. |
| [RackBay](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs) uses a 24 m teaching length, 9 m V2 half-width, three spaced Crop Trees, 30% initial release and 40% removed-BA safeguard. | Existing lesson calibration is not a universal access reach, whole-parcel crop prescription or new compact success policy. Preserve it; measure compact eligibility separately. |

**Architectural finding:** public production geometry/new-game APIs cannot express a third compact property and one-rack V2 lesson. **Unresolved:** whether temporary isolated runtime instrumentation can generate useful, internally coherent candidate measurements without permanent engine changes. No native attempt was permitted, so this report does **not** prove that a production refactor is necessary, choose an architecture, or implement the Opus recommendations. If a future native probe cannot do this within disposable instrumentation, the packet requires stopping and reporting the prerequisite.

### Required two-candidate comparison — not populated with invented worlds

| Required independent candidate | Input configuration / geometry / seed / real IDs | Eligible standing stems on each rack side; BA/volume; route/edge/economy | Native result |
| --- | --- | --- | --- |
| C-A | **NOT GENERATED.** Dimensions, stock and rack remain unset; no save is used as a compact alias. | UNMEASURED | BLOCKED — UNITY LEASE |
| C-B, independently generated alternative | **NOT GENERATED.** No invented second configuration or guessed IDs. | UNMEASURED | BLOCKED — UNITY LEASE |

These are **outstanding study slots**, not two completed candidate configurations. No claim is made for net eligible hectares, stems/ha, removal mixes, crop-selection feasibility, access connectivity, collision clearance, shortest inspection route, performance, or a final candidate recommendation. Human walking, spraying/marking minutes, legibility and instructional clarity remain **UNMEASURED**; even a later computed route length will not establish those observations.

For the authorised continuation, export per candidate all actual IDs/species/age/DBH/height/crown/stage/eligibility/position and exact generator settings, scene anchors and ecology seed; separate the union of clearance/access from the two standing-management polygons. Use disjoint left/right eligible areas and count each corridor/clearance overlap once. Export nearest collision/path/edge distances, crown influence and seed-neighbourhood diagnostics. A screenshot or tree count alone does not prove reach/inspect access to every eligible stem.

## Ecology, basal area and timber: source findings, no candidate totals

- Correct current per-tree basal area is **π × (DBH_cm / 200)² m²**; the RackBay source uses this conversion. Sum on the declared polygon and divide by its area in hectares to obtain m²/ha. For an explicitly selected removal set, retained BA = current live BA minus selected live removal BA; do not count historical stumps or the same corridor twice. No compact BA totals or percentages were measured.
- [ForestTree](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ForestTree.cs#L235) computes biological stem volume as `DBH_cm² × 0.00007854 × height_m × FormHeightRatio`. Current three species assets use 0.5 form ratio. This is the existing simplified volume model; biological stem volume is **not** sale-assortment volume.
- [TimberYield configuration](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs#L49) allocates actual merchantable logs: selected Sitka sawlog specification includes 4.9 m nominal length and 200 mm small-end diameter, pallet >=140 mm, stake 70–130 mm small end, and pulp >=70 mm, with length/quality/residual rules. No all-sawlog classification or flat €72/m³ sale is justified from starting DBH alone.
- The source generator spawns its default species at one canonical age; it does not itself author an older mixed Oak/Beech cohort. The inspected Sitka maturity function returns zero at age 20 and increases toward full maturity at 30; Oak/Beech source maturity spans 40–60 years. These are **source model settings**, not newly researched Irish biological constants. Actual initial juveniles/broadleaf seed sources and scene contribution must be inventoried natively; no older cohort was fabricated.
- [Annual ecology](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ForestEcologyController.cs#L458) remains one shared engine: competition/canopy, adult growth, crown relaxation/density mortality, juvenile processes, mast/seed, establishment/promotion and other annual updates. Species-specific crowns, seed kernels, growth/browse responses and the exact planted-juvenile path must stay in implementation scope for Sitka/Oak/Beech.
- The canopy calculation uses current crown influence at 5 m cell centres with half-cell reach; Hegyi competition has an 8 m cutoff. Source seed cutoffs are 150 m Sitka, 80 m Oak, 50 m Beech. Small-boundary/clearance changes therefore affect substantial fractions of the neighbourhoods. These facts justify native edge diagnostics, **not** a year-after canopy forecast or a proven compact seed supply.
- Potential to revisit a changed forest is supported by the existing annual machinery as an implementation capability. Actual changed-world returns and later justified interventions in a compact parcel are **NOT RUN**, and must not be inferred from that capability.

## C0-04 economics — useful supplementary evidence, not compact viability

**DIRECT SOURCE + historical GAMEPLAY CALIBRATION:** [ScenarioOne.asset](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset) has €12,000 starting cash, €2,500 minimum commissioned harvest job, €45/hour non-harvest contractor rate, 2,400 owner minutes/year, €5 shelter material, €4.50 Beech / €5.50 Oak nursery stock and Year 25 historic completion horizon. These are **not approved compact cash/age/completion values or current market quotations**.

[Economy adapter](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOneEconomyAdapter.cs) deduplicates valid tree IDs and quotes valid annual harvest orders as one job; owner production felling is ineligible. It accounts for sold, retained, deadwood and residual material separately. [Manager approval and settlement](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs#L1235) checks available cash, resolves approved work, applies the grouped harvest settlement once after the linked world operations succeed, then runs the annual ecology. Quotes are not actual settlements.

[Work calculator](https://github.com/jermur788/CCF/blob/6c13a9e182774945bf391bc8d4845ce74d3e5e88/Assets/ForestPrototype/WorkEconomy/ForestryWorkCalculator.cs#L145) applies the €2,500 as a **floor on the job's harvesting/forwarding cost**, not a surcharge added to that variable cost and not a per-tree charge. Separate actual harvest years/jobs can incur the floor again; there is no native visit itinerary measurement here. Upfront cost must be available before timber receipts. Negative net contribution is allowed; valid approval guards prevent spending future receipts. Low positive cash can strand a later job before the source's zero-cash failure condition.

### Executed pure fixture

Existing `Tools/Verification/Stage1WorkEconomyFoundationVerification.cs` was inspected before execution. It has no scene opening, PlayerPrefs, filesystem save, or world operation. **Seven byte-identical copied .cs inputs** (six WorkEconomy files plus this fixture) ran in an external .NET project using installed **SDK 10.0.114 / net10.0**; no NuGet package sources or added dependencies. Result:

`STAGE1_ECONOMY_VERIFY_PASS fixtures=66 assertions=420 evidenceSha256=2085CEEC5DE02838967B8F4B39A14993E28AE97BE5C3A6F070709E42156AD2F4`

Exit **0**. This validates the copied pure calculator/fixture on .NET, **not** Unity compilation, native JsonUtility, ScenarioOneEconomyAdapter execution, compact yield, planting in a scene, settlement or complete-loop solvency. Generic fixture tree IDs/tonnages are test data, not observed compact trees.

### Static cash sensitivity — MODEL DERIVATION / UNVERIFIED IN UNITY

Using only the source's selected Sitka prices (€38/47/68/95 per tonne for pulp/stake/pallet/sawlog) and selected 890 kg/m³ conversion, the hypothetical **sold-log volume needed merely to equal a €2,500 floor** is:

| All sold logs hypothetically one assortment | Model volume to equal floor |
| --- | ---: |
| Pulp | 73.92 m³ |
| Stake | 59.77 m³ |
| Pallet | 41.31 m³ |
| Sawlog | 29.57 m³ |

Calculation: `2500 / (selected €/tonne × 0.89 tonnes/m³)`. These are algebraic breakpoints, **not attainable candidate yields, merchantability predictions or guaranteed break-even jobs**. Whole-stem residues/retention, variable work costs, other work/stock and grading can increase the actual requirement. All-sawlog is an optimistic accounting bound only; the source prices are dated evidence/calibration, not a new market estimate.

Another explicitly hypothetical floor-dominated trace, with **zero receipts and zero other costs**, leaves historical €12,000 cash at €9,500 / €7,000 / €4,500 / €2,000 after one / two / three / four harvest jobs. The fifth €2,500 upfront payment would be refused. This demonstrates repeat-visit pressure, not a feasible three-cycle scenario: one access job and two later jobs are not automatically three silvicultural cycles. Native candidate clearance and 1–2 defensible modest removal mixes per candidate are **NOT RUN**.

Owner planting/protection can avoid a contractor wage payment but still consumes real simulated time, nursery stock and shelter material; source owner wage/equipment shadow values are zero. Nursery purchases are charged through the scenario offer once, while planting consumes existing stock. The pure book contains different nursery price entries; do not substitute those for `TryPurchaseStock`'s live offer or double-purchase stock. The adapter uses pure work profiles; stale definition planting-minute fields are not observed human timings. No-work intervals do not themselves commission a harvest or pay timber income; ecology can continue annually. Their actual compact cash/world traces remain unverified.

## Protected state and cleanup receipts

External evidence root, actually created on Files:
`/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/`.

- `binding-packet.md`, `live-coordination-readback.md`: exact fetched reference content.
- `source-audit-sha256.json`: **30** pinned context/source/species/Reference-schedule/settings inputs and SHA-256s.
- `pure-source-sha256.json`, `pure-economy/`, `pure-economy.log`: **7** copied C# inputs, external project/closed NuGet config and full .NET output. The SDK reported first-run development-certificate initialization; no Unity profile initialization occurred.
- `cash-model-derivation.json`: exact decimal formulas/inputs; no invented forest fixture.
- `worktrees-before.json`, `worktrees-after-preflight.json`, `default-unity-profile-before.json`, `default-unity-profile-after.json`, `ops-lock-before.json`, `ops-lock-after.json`, `preflight-cleanup.json`: physical comparisons.

At **15:01:56 UTC**, all four observed worktree HEAD/status/diff records matched their preflight records; all 30 audited source hashes and 7 pure-source copies matched. The default `/home/jer/.config/unity3d/DefaultCompany/CCF` profile had **10 files**, all byte-identical before/after. `forest-save.json` and `.tmp` were absent at both checks; `.bak`, prefs and archived save were present and unchanged. **Absence is not a save-hash PASS.** This limited inventory does not attest every isolated historical/user slot elsewhere.

Preserved unrelated work: Primary's two original modified files (`AGENTS.md`, storage architecture); local main worktree's original eight settings/material modifications; clean #443 checkout. GitHub main has separately advanced coordination; this worker did not change it. No reset, stash, clean, overwrite, asset deletion, source merge/cherry-pick or shared Ops edit.

**Zero Editor runs; zero scripts/metas staged in Assets; no autostart introduced; no scene/prefab/ProjectSettings/package source change; no game save or Prefs writes; no Windows build.** Existing native economy harness was inspected but not copied/run: it sets EditorPrefs, opens ForestTest and performs in-memory LoadData/restoration, so “memory only” would not remove the lease/profile requirement.

**Native isolation remains NOT ESTABLISHED.** Source save paths use `Application.persistentDataPath/forest-save.json` plus `.tmp/.bak`; learning/help use global `CCF.Learning.v1.*` / `CCF.MenuHelp.v1.*`; an Editor harness can also use EditorPrefs. The worktree alone does not isolate any of these. Before future native execution, independently verify real save, PlayerPrefs **and EditorPrefs/profile** locations and review every harness write. Backups alone do not authorise touching originals.

Protected deterministic/lifecycle/world-hash/Reference archive replay gates: **NOT RUN**. Byte-preserving source/profile checks are not substitutes. No hash was rebaselined and no test was weakened.

## Open decisions, gates and exact continuation owner

- **OPEN-C1:** player-planned versus preauthorised access remains open; existing five-rack Confirm cannot be used as one-rack evidence.
- **OPEN-C2:** both authentic configurations, dimensions/stock/route/area/edge/eligibility/performance remain unmeasured. No footprint recommended.
- **OPEN-C3:** older Oak/Beech versus juveniles-only remains a product choice; three-species biology stays in scope, O2 irregular-cohort/six-species identity stays distinct.
- **OPEN-C4:** no save/router/ID/schema/Prefs policy chosen; protected pre-mutation identity and historical hashes still require the separate approved work.
- **OPEN-C5:** cash/BA/classifier and episode spacing remain open. Three substantive cycles remains **Manager DRAFT interpretation**, not an owner-ratified numeric rule. D-074/D-077 are unchanged.
- **OPEN-C6:** no removal-set status/overlay/hardware policy chosen. An immediate current-crown illustration cannot be presented as future biology.
- **OPEN-C7/C8:** no compact teaching-complete/cadence integration or approved-order edit policy chosen. No borrowing legacy stage 7, learning keys or v24 credit.
- Future OPEN-1–OPEN-6, #436 and all four DRAFTs remain untouched; #459 E-1–E-7 and the refactor strategy stay amendment/review inputs only.

**Next owner action: Manager #424** must answer the linked lease request, reconcile its live coordination and #443/user custody, and explicitly authorise/register a safe native window/lane. This worker must then refresh exact source/branch/disk/process/lock state, establish independently checked profile isolation and inspect disposable instrumentation before staging anything. If no safe window exists, #460 remains BLOCKED; do not issue a native PASS or begin production prerequisites.

If released, continue the original bounded two-candidate study, not a full Tier-3 queue: one inspected disposable Editor fixture, authentic candidate exports, two-sided access/area/BA/volume/seed/crown diagnostics, real quote-versus-settlement traces and no-work/re-entry evidence; copy/run/remove receipt and process/lock closure. Stop if permanent engine changes are necessary. Amend **this same report only** with genuine observations, then fetch/readback and compare against the original baseline. Separate document author/amendment, independent delta review, owner decisions and Manager approval still follow.

Worker assessment: **ready for review as a blocked preflight report only; not ready for native acceptance or implementation.** #460 stays open/BLOCKED after publication, preserving continuation. No integration/feature acceptance is requested.

**DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** No dashboard ingestion/publication or Drive mirror readback was performed, and no generated dashboard file was edited. #424 must record the blocked result in its authoritative coordination and follow the supported dashboard publication/readback process; this outstanding operational action is not silently closed.
