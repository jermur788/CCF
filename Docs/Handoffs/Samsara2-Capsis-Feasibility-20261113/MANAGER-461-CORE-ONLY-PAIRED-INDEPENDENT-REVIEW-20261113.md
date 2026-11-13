# Manager independent review — #461 Samsara2 controlled offline comparisons

**Date:** 13 November 2026  
**Worker handoff reviewed:** [#461 source result](SAMSARA2-CORE-ONLY-PAIRED-FOLLOWUP-RESULT-20261113.md)  
**Manager verdict:** **APPROVED FOR CONTROLLED OFFLINE RESEARCH COMPARISON ONLY**. The verdict is scoped to a tested comparative workflow and honest qualitative observations; not an external scientific validation, Irish growth calibration, gameplay design decision, code integration, or authority to reuse Prelenfrey data in a distributed game.

## Evidence inspection and change scope

- Independently fetched the full worker result, #461 issue timeline, #424 comments and live coordination record, exact #461 research commit, and GitHub compare against the worker's registered starting repository state.
- The worker's research branch is **one commit ahead** of its stated start and changes **exactly one new Markdown result**, with no committed Unity or CCF source, assets, approved specs, saves or scripts.
- A separate **prior** main-branch coordination commit was inspected. It touches **only active-tasks.md**, adds a prospective #461 entry and explicitly confines the user-transferred write to that entry. The worker's RUNNING registration and handoff follow it in GitHub chronology. This corrects the missing prior board entry from #456; do **not** call #461 a repetition of that process violation.
- Current live coordination entry still labels #461 **PROSPECTIVE — RUNNING**, despite the claim being **HANDED-OFF/CLOSED**. Manager #424 remains board writer for subsequent state updates. Requires an explicit **closed/handed-off** correction; no more live research ownership or resource lease should be implied.
- I did **not** independently access or re-run the HP WSL machine, the pinned local binaries or the off-repo 518-item hash manifest. Numerical results, runtime/memory and exact absence of Unity processes are **worker-reported local evidence**, not independently executed by this Manager.

## Gate assessment

| Area | Review |
|---|---|
| Gate 0 ownership / branch | **PASS from GitHub evidence**, including contemporaneous, scoped user transfer of a single advance board-entry write, prospective entry before RUNNING, separate HP lane registration and research-only scope. Historical board row now needs closing. |
| One fixed starting state | **WORKER-REPORTED PASS.** A single martelosim preparation gave the same inventory and same initial tree table/hash across all fifteen runs, and all initial N/G/V/sapling/light fields are identical. Downstream randomness remains. |
| Core-only measurement | **WORKER-REPORTED PASS with denominator qualification.** exportCoreOnly=true counts the model's 1.17-hectare rectangular core, not the separate official 1.10-hectare marteloscope polygon. No generated peripheral trees in this prepared inventory. Avoid calling it the precise historical 1.10 ha plot or claiming the two per-hectare areas are interchangeable. |
| Treatment design | **PASS for controlled qualitative model comparison.** 5 replicates each for no-cut, large silver-fir removal under focal-guided weights, and broad thinning; 2026/2036 treatments end 2056, same +0/+10/+30-year intervals as #456 with changed absolute years. Treatments have near-equal binding ~20% first-cut basal area. |
| Outcome stability | **Supported within one preparation draw.** The prior directional differences in tree size structure, basal area order, ground irradiance, mortality and species trajectories hold. The #456 apparent T1 increase in saplings over no-cut **does not hold clearly**: 529±18 against 524±15 saplings/ha. Five repetitions are adequate for exploratory direction comparisons but not universal uncertainty bounds. |
| Focal crop-tree attribution | **PARTIAL / NOT DIRECTLY VERIFIED.** Actual Samsara2 script outputs only count 59 focal trees and do NOT export their IDs; true first-treatment focal adjacency remains UNKNOWN. The source-available LGPL CAPSIS kernel MultiCriteriaThinner2 was used through inheritance as a no-cut preview **PROXY**, without claimed decompilation. The proxy exactly matched real T2 broad thinning but did **not** match real T1 cuts (Jaccard ~0.32). |
| Honest T1 treatment label | **MANDATORY CORRECTION ACCEPTED:** real T1 cut choices are about 19–21 overwhelmingly large silver fir trees (DBH roughly 63–68cm) whereas proxy focal selections are smaller (mean DBH ~20cm), and the proxy cut distances favour *greater* not lower distance to its focal trees. Treat it as **large silver-fir removal under focal-guided weights**, NOT verified local crop-tree release. Do not claim actual-model crop-tree adjacency is proven absent solely by the non-equivalent proxy. |
| Scope and licences | **No committed product code changes**. No licence for the demo data or bundled third-party assets confirmed. The LGPL kernel-based probe stayed off-repo. Samsara2 model Java source was not supplied in the GPL-licensed module installer; do not repeat the earlier mistaken static-audit claim. No import of GPL model code into Unity permitted. |
| Durability / closure | **PASS:** expected exact path and GitHub result published; claim HANDED-OFF/CLOSED. Off-repo run logs, scripts and manifests cited; dashboard/Drive not verified. |

## Quantitative examples — *reported model output*, not Irish evidence

Five-run means after thirty years, core-only model area 1.17ha:

| Model measure | No-cut | Large fir removal | Broad thin |
|---|---:|---:|---:|
| Remaining basal area (m²/ha) | 49.56 | 41.17 | 35.71 |
| Ground irradiance (MJ/m²/yr) | 9.27 | 9.39 | 20.68 |
| Cumulative mortality (N/ha) | 476 | 287 | 124 |
| Saplings (N/ha) | 524±15 | 529±18 | 452±17 |

Differences demonstrate the **model's sensitivity to which trees are selected**, not the ecological truth for Ireland. They are conditional on one stochastic preparation of the Alpine site (other preparation draws changed sapling count ~4%). No claim that this is a field validation or proof of causal magnitude across seed configurations.

## Decision and integration boundaries

**GO:** retain Samsara2 22961 + martelosim 0.1.3 as an **offline scientific comparison tool** for controlled experiments, with shared preparation, core-only denominator, declared simulation years, multiple replicates, actual cut-tree exports and qualified focal-tree attribution. In CCF research, compare only qualitative directional behaviour after explicitly identifying non-equivalent species/sites and gameplay abstractions.

**NO-GO:** implementing Samsara2 in Unity; copying Java source/coefficients; changing Irish Sitka/Oak/Beech models, parity expectations, Rack V2, MFM, staged Work Plan decision gate, S1-C review/S1-D interventions, saves, or frozen Reference Future. Do not claim successful game integration or change product decisions/specs based on this handoff.

For the user's **potential Prelenfrey expansion**, a *separate* product feasibility decision is required. A preserved mapped inventory can inform a faithful 2023 starting arrangement, but the actual forestry plot versus 1.17ha rectangle, real DEM/terrain, height/crown gaps, historical climate/soil data and especially inventory redistributability must be established before calling it an exact, shippable real-world reconstruction. This research does not grant rights to the bundled field data.

## Manager disposition / next step

1. **APPROVED** the bounded original research result. #461 stays HANDED-OFF/CLOSED.
2. **Manager #424 must close the live #461 board row**, without changing unrelated locks/writers. This is a bookkeeping correction to the previously authorised prospective entry; do not make the user re-run the study or change execution chronology. #456 historical board exception should also be reconciled separately.
3. **No automatic further Samsara2 tests dispatched.** The next useful move is a product-level decision whether to prioritise (A) offline model-comparison methodology for a CCF ecology spec, or (B) a future Prelenfrey scenario with necessary rights/terrain audit. Either requires independent approval and should not distract from the active Scenario One milestone. Do not create busywork to improve the already-sufficient feasibility verdict.
4. Preserve local logs/toolchain and manifest on the HP host, record any eventual second-party rerun as separate evidence. No unattended automation or background host processes claimed.

**Status:** researched and reviewed, **GO offline comparator only**; no production integration, no Unity verification or Windows build. **Dashboard/Drive sync: NOT VERIFIED.** **User action:** none for review completion; no further research currently released.
