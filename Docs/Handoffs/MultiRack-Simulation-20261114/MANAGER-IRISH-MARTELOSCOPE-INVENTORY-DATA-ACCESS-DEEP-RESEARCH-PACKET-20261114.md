# Multi-Rack SIM — Irish marteloscope tree-by-tree inventory, lawful data access and later verification: Deep Research handoff

**Issued:** 14 November 2026  
**Task claim:** [#475 — Irish marteloscope exact spatial-inventory and data-rights research](https://github.com/jermur788/CCF/issues/475) — **READY / NOT RUNNING**. New worker must register actual identity before report edits.  
**Issuing role:** dedicated Multi-Rack Simulation Research Manager (separate from Scenario One delivery).  
**Research difficulty:** **HIGH**, because a site description or plot-level graph is not equivalent to inspectable **tree-by-tree XY + DBH**, and rights, field measurement provenance and repeated outcomes must be established through multiple organisations/archives.  
**Recommended provider:** **ChatGPT Deep Research**, or equivalently capable evidence-intensive research agent with strong primary-source verification; **not a quick ordinary search**. A GitHub-capable researcher/publisher may separately commit the final research report.  
**Scope:** research only; no development or contacting anybody.  
**Packet author's branch:** \`docs/multirack-irish-marteloscope-inventory-deep-research-packet-20261114-v1\` (GitHub docs-only).  
**Dedicated researcher result branch, created from main and initially empty:** \`research/irish-marteloscope-inventory-and-rights-20261114-v1\`.  
**Research worktree:** NONE. No Unity, Java/R/Samsara, physical host, model execution or programming task.  
**Only permitted tracked research result:** \`Docs/Handoffs/MultiRack-Simulation-20261114/IRISH-MARTELOSCOPE-INVENTORY-COMPLETENESS-RIGHTS-AND-CONTACT-SHORTLIST-20261114.md\` on the dedicated researcher branch. Do not edit this packet, specs, product authority, active coordination, code, files under \`Assets/\`, old branches or saves.

## 1. Owner's actual question and intended use

The project owner proposes to start the independent Multi-Rack simulator as an **Irish Sitka spruce, flat/simple-site, individual-tree 2D simulation laboratory**. An **authentic Irish marteloscope** may be a better initial-state anchor than inventing all tree positions and sizes, provided there is enough lawful data to reconstruct the **actual measured tree IDs, XY locations, species and diameters**. Build a simple 2D research stand first, increase ecological and species/site complexity in justified stages, and eventually connect the same tree/decision state to the planned walkable **3D forest**. This research decides neither a game design nor an implementation algorithm.

**Research must answer two distinct questions**:

**A. Exact reconstruction feasibility now.** Can we obtain and lawfully analyse at least one **named Irish plot's actual individual-tree measurements** with sufficient resolution and provenance to reproduce the existing site in 2D? If yes, identify precisely which archive/file/field combinations, tree-count denominator, access conditions, quality gaps and steps make it feasible. If not, do **not** invent missing coordinates or claim a faithful plot.

**B. Real external comparison later.** Which **one or two organisations or named data stewards** would be best to approach *later*—with the owner's separate approval—for an original tree inventory, optional marking exercise records, actual executed harvest history, **repeated individually mapped field remeasurements**, plot/site metadata and explicit permission to use the data in a research simulator and potentially an eventual public/commercial game?

**Synthetic fallback:** If existing public data give only species/stem-size/spacings but not complete XY/IDs/plot geometry, recommend a **clearly labelled synthetic Irish reference stand** constrained by appropriate Irish empirical ranges; it is not a reconstruction and does not validate individual-tree growth. This fallback must not depend on a hypothetical source file or silently assert that a real plot has been digitised.

**Separate distinctions:** educational **virtual marks** ≠ **executed felling**; initial measured forest ≠ a future forecast; full mapped adult inventory ≠ “every stem and seedling”; GROWFOR stand-level comparisons ≠ individual-tree, canopy and regeneration validation.

## 2. Mandatory starting evidence — read sources, then verify the claims live

### GitHub CCF prior work (not automatically fresh truth)

1. [#452 prior comprehensive marteloscope datasets/rights report](https://github.com/jermur788/CCF/blob/docs/research-marteloscope-dataset-feasibility-20261113-v1/Docs/Handoffs/Research/DEEP-RESEARCH-MARTELOSCOPE-VIRTUAL-CCF-FEASIBILITY-RESULT-20261113.md) and [independent review](https://github.com/jermur788/CCF/blob/docs/manager-425-marteloscope-research-verdict-20261113-v1/Docs/Handoffs/Research/MANAGER-425-MARTELOSCOPE-DEEP-RESEARCH-REVIEW-VERDICT-20261113.md). Its site/data list is a **documentary lead**, not a downloaded, inspected stem table or data licence. Its originally submitted PDF had a clipped table; do not invent missing cells.
2. [#472 Samsara2/martelosim L0–L4 validation-methodology report](https://github.com/jermur788/CCF/blob/research/multirack-samsara2-validation-20261113-v1/Docs/Handoffs/MultiRack-Simulation-20261113/SAMSARA2-MARTELOSIM-CCF-VALIDATION-METHODOLOGY-AND-EVIDENCE-GAPS-20261113.md). This already distinguishes static correctness, model comparison and independent field predictive accuracy. **This task is NOT another Alpine/Samsara2 simulation study.**
3. [#466 actual CCF five-rack research fixture](https://github.com/jermur788/CCF/blob/research/multirack-five-rack-native-evidence-20261113-v1/Docs/Handoffs/MultiRack-Simulation-20261113/FIVE-RACK-NATIVE-GEOMETRY-COMPARABILITY-WORKLOAD-EVIDENCE-RESULT-20261113.md) is **an invented game forest with audited saved tree data**, NOT a measured Irish marteloscope or the owner's saved playtest. Use only as an example of the ID/polygon/evidence standard. Its 24×18m scripted teaching bay is not a verified whole-rack player-managed sample.
4. [CCF research index](https://github.com/jermur788/CCF/blob/main/Docs/Project/research-index.md), and existing user-supplied **GROWFOR technical evidence extraction** / growth-density/species composite are context for Irish growth-yield sources, not a substitute for direct per-tree mapped observations.
5. Project authority: preserve accepted Scenario One milestone and three-species parity; this separate Sitka-first **research order is not authorisation to simplify or rewrite Scenario One** or change main, Decision Log, Current Milestone or approved spec.

### Initial independent/public sources (verify rather than blindly copy)

- [Teagasc: The marteloscope training network](https://teagasc.ie/crops/forestry/advice/management/continuous-cover-forestry/the-marteloscope-training-network/): Teagasc states that every tree in its training plots is numbered and measured and measurements/estimated timber-biodiversity values go into tablet software; at that page's publication, **three Teagasc and six Coillte plots** were reported. Explicitly names **Curragh Chase (Limerick), Oak Park conifer and Oak Park broadleaf (Carlow)**. The page links named **Jonathan Spazzi** as a further-information contact. These are **data-existence leads**, not verified available downloadable inventories or commercial rights. Confirm whether that count is now stale.
- [2022 Teagasc announcement](https://teagasc.ie/crops/forestry/news/launch-marteloscope-training-programme-php/?c=98): tree locations mapped, tree IDs labelled at Oak Park; check exact current workshop/admin/ownership route.
- [Integrate Network marteloscopes and I+](https://integratenetwork.org/marteloscope/) and [scientific spatial tree-microhabitat dataset paper](https://pmc.ncbi.nlm.nih.gov/articles/PMC9836569/): national plot coverage, data schema, plot maps, possible site steward contacts, and a published **GBIF subset of 42,078 observations from 111 plots** (figures refer to that published dataset *version*, not necessarily a current count). [GBIF archive lead](https://www.gbif.org/dataset/2e102194-f384-4712-89a4-5db7a3fc409a) / DOI \`10.15468/ocof3v\`. **Locate the exact Irish plot records and any dataset-level licensing**, don't infer all 300+ network sites are public or that a general global CC BY statement licenses individual restricted files. Timber quality/price and all marks may not be in the GBIF subset.
- [Pro Silva Ireland, CCF practice case studies](https://prosilvaireland.com/wp-content/uploads/2025/03/2a.-Purser-CCF-in-Private-Sector-Ireland-March-2025.pdf) identifies **Kilteel (Co Kildare)** as a Sitka/Oak managed **Tree Marker Training Forest**, with management/inventory history. **Do not automatically call it an EFI-standard marteloscope or assume full XY/DBH/tablet data exist.** Explore only as a **qualified Sitka-first contact/source lead**. The same report describes **Knockrath** and **Cloragh** repeated *stand inventories*—potential later field-validation lead, but no verified same-tree XY repeated records.
- Check current official Teagasc/Coillte/Pro Silva/EFI information and primary dataset descriptions. Prefer official institutional, archive metadata, source-code docs and peer-reviewed papers; use media summaries only to discover leads.

**Avoid unsupported starting claim:** “Kilteel is the best measured marteloscope.” That is **not proven**. It is Sitka-relevant, but named Oak Park and Curragh Chase are more clearly established formal marteloscopes. The open GBIF Irish plot may be a stronger immediately researchable option even if its species mix is less ideal. No ranked conclusion until data checked.

## 3. Research tasks in priority order

### R1. Locate and catalogue actual Irish marteloscope sites

Build a **named site table**, including Oak Park conifer and broadleaf, Curragh Chase, Kilteel (explicit classification check), the reported Coillte six and any Irish sites visible on current Integrate/I+ maps. **State the observation date** of any plot-count total. For each, record site steward, plot code, species composition if verified, true plot size and shape, region, sampled date(s), official source link, mapped-inventory existence and archive path. If site name/plot code is withheld, say so; never invent a downloadable CSV or tree count.

### R2. Audit the best **actually accessible** stem dataset, not just descriptions

Inspect current GBIF archive documentation and the Irish record subset. Where permitted using normal public downloads, analyse the actual metadata/fields or sufficiently representative raw rows; name **archive version, stable download URL/DOI, rights, file names, field names, exact plot ID, exact tree count, missingness and accessible units**. Distinguish plot-centre geolocation from **within-plot relative XY**; existence of geographical centroid does NOT prove every tree can be mapped. Verify source coordinate origin, axes and orientation, whether a polygon/plot dimensions are present, species identification and DBH scale, height/crown completeness, adult inclusion threshold and duplicate/estimated field status.

If a browser/Deep Research session cannot download and inspect the raw rows, record **NOT INSPECTED** and do **not** invent count/columns/completeness or call “exact reconstruction GO”. This task authorises only publicly accessible, licence-compatible data inspection—not login, access circumvention, scraping restricted I+ accounts, redistributing data or committing raw tree inventories.

If another named public Irish plot better meets the standard, analyse that too. Prefer **two inspectable plots** if feasible, but a rigorous one-plot result beats speculative breadth.

### R3. Evaluate *precise 2D reconstruction* feasibility

For each promising site, answer explicit YES / CONDITIONAL / NO / UNKNOWN:
1. All **mapped adult trees** in plot represented? Unique stable per-tree IDs and known inclusion DBH threshold (often ≥7.5cm but must verify plot-specific).
2. Actual **within-plot X/Y** for each stem and unambiguous units/reference/origin/orientation, with plot outline and exclusions? Any only indicative maps, grid ticks or non-georeferenced screenshots?
3. Original **DBH, species, status** for each stem? Measurement dates, height, crown dimensions, age and juvenile layers? Distinguish genuine measurements vs assumptions, regressions and virtual/generated trees.
4. Tree count and **recomputable plot basal area, BA/ha and N/ha** from same valid sampled area, with raw row/checkable source? Handle stumps, deleted trees, trees outside polygon, missing/negative measurements, multi-stems.
5. Rights for private/offline analysis, derived schematic visuals, distribution/public reporting and **later commercial game incorporation**—independently; written permission or licences needed if not explicit.
6. Whether data supply real **marks** and either *actual executed cut tree IDs* or **same-stem remeasurements**. Only these latter observations could support future ecological prediction testing; plot inventory alone is initial-state truth.
7. Confidence grade: VERIFIED RAW-DATA / VERIFIED DOCUMENTATION-ONLY / PLAUSIBLE LEAD / UNKNOWN / NO-GO. **Do not say exact if source XY or plot boundaries are missing.**

Define “exact” with honesty: **exact to the recording protocol for all qualifying measured stems**, not an exact 3D forest including unmeasured crowns, seedlings, understory and landscape terrain. Visual renderings and interpolated attributes remain artistic/estimated.

### R4. Rank candidate datasets for two purposes

Give **two separately ranked shortlists**, not “one best” based on size:
- **Best immediate 2D stand baseline:** legally accessible complete species+ID+XY+DBH+polygon, Sitka preference, quality/auditability.
- **Best future ecological verification dataset:** repeated actual field remeasurements + executed interventions, same-tree histories, independent holdout opportunity and field site/weather metadata.

State plot area and count **only if actually verified**. A large one-time inventory may lose to a smaller full spatial record; a longitudinal stand might lack XY and still be useful for *stand-level* validation. Use transparent weighted criteria if helpful but do not falsely score unknown data as zero or confirmed positive.

### R5. Identify **exactly one or two** sensible future contact targets — DO NOT CONTACT

Select the strongest one or two **organisations/data stewards**. Likely candidates to **verify**: **Teagasc Forestry Development (Oak Park/Curragh Chase; official site identifies Jonathan Spazzi as contact)**; and either **Coillte/EFI Integrate I+ dataset steward** for formal plot exports, or **Pro Silva Ireland / Kilteel organiser** if the latter has tree-level Sitka data. Contact ranking must follow actual custody, credible access and rights evidence, **not personal prominence**.

For each target provide:
- Official contact person/role **only if published and verified**, official institutional email or webpage (don't guess emails), organisation and source link; distinguish facilitator from actual data controller and check if referral is needed.
- Target plot(s), which measurements are sought, why they matter, likely format (CSV/Excel/I+ export only if evidenced), the exact ask for permissible study/data-sharing conditions, and how to use later publication/credit. Include **a concise UNSENT enquiry draft** for later user approval, requesting: stem ID, species, plot-relative XY, DBH/height, recording date, plot polygon, coordinates reference, crown/regen if possible, initial and remeasurement datasets, real cuts vs virtual marks, and rights for internal research/derived/public/commercial use. Ask for a **minimal safe sample/schema first** if they cannot release full data.
- Do not send any email, message, form, connection request, registration, purchase or dataset-access application. The user will decide on outreach after report review.

### R6. Synthetic fallback and stop/go recommendation

If no real plot passes 2D exact-data gate, outline a minimal **synthetic Irish Sitka flat site** using clearly cited Irish stand-level DBH/density/height constraints (GROWFOR and current CCF evidence as appropriate) and explicitly synthetic individual positions/crowns. Propose validation of numerical outputs against Irish stand-level evidence, **not** false field-level reconstruction. Preserve the option to replace starting tree table with a lawfully acquired mapped marteloscope later without changing model core or identities. **Do not implement, calibrate or publish synthetic CSV under this task.**

Conclude with one of:
- **GO (bounded, research only):** at least one permitted, directly inspected, complete mapped stand can be reconstructed for research purposes; list precise dataset/version/permissions. No Unity/build authorisation.
- **CONDITIONAL:** documented data likely exist but raw records, boundary, fields, right to use or licence unverified; recommend direct steward request or limited public archive verification before any exact build.
- **NO-GO exact real plot now:** critical XY/IDs/plot rights unavailable; synthetic prototype is the lower-risk option, with contact candidates retained.

## 4. Required final report format and sources

The single Markdown report, at **exact path** \`Docs/Handoffs/MultiRack-Simulation-20261114/IRISH-MARTELOSCOPE-INVENTORY-COMPLETENESS-RIGHTS-AND-CONTACT-SHORTLIST-20261114.md\`, must contain:

1. **Executive choice**: whether a real Irish plot can presently be faithfully represented tree by tree in a 2D CCF research simulator, and why.
2. **Source/evidence ledger** (live official sites and archives, date consulted, primary links, observed vs documentation-only vs inferred vs unknown; compare/repair key #452 gaps).
3. **Named sites matrix** with plot IDs/years/species/tree counts/area/measurement types/rights where supported; classify Kilteel properly.
4. **Actual raw dataset audit** of GBIF Irish plot(s) if possible; field-level schema, ID/XY/polygon/DBH mapping, known missingness, eligibility cutoff and how to reproduce N and BA. If not accessible, precise reason and next gate.
5. **Exact reconstruction feasibility rubric and per-site verdicts**, clearly declaring which fields would have to be imputed.
6. **Two purpose-specific ranked lists** (static baseline vs longitudinal accuracy) with justified limitations.
7. **One or two recommended official contact routes and two or fewer unsent enquiry drafts**, prioritised by credible dataset custody; no outreach.
8. **Recommended first executable proof** (only as a future separately approved task): versioned lawful 2D plot import and stat recheck, acceptance criteria and missing resources/rights; separate synthetic alternative.
9. **Evidence gaps and no-go gates**: no exact tree placement from stand totals/heatmaps; no imaginary raw file; no predicted future labelled observed; no player marks labelled executed; no Martelosim forecast treated as Irish ground truth; no arbitrary rights assumption.
10. **Practical recommendation** for owner/manager and **dashboard/Drive sync** (NOT VERIFIED unless actually checked).

**Minimum useful evidence**: cite precise first-party Teagasc/EFI/GBIF/author or named official plot documents; for numerical claims show file/field and calculation, not just prose. Avoid dumping a large generic literature overview or repeating #452. **No need to provide an Irish marteloscope recommendation if none passes actual access checks.** Preserve “unknown” where necessary.

## 5. Ownership, registration, safety and publication gates

- **Researcher claim:** [#475](https://github.com/jermur788/CCF/issues/475) exists as READY / NOT RUNNING. Before writing the result or invoking a separate implementation researcher, **register actual provider/model and tool capability, branch start HEAD and worktree NONE in a GitHub issue comment**, change issue to RUNNING if permissions allow. If Deep Research has no GitHub write tool, report that limitation and actual worker identity to this Manager **for truthful registration before drafting the report**; do not claim self-registration that did not occur.
- **Sole writer/path:** assigned result branch \`research/irish-marteloscope-inventory-and-rights-20261114-v1\` starts from verified main. Fresh-read branch HEAD/compare and result path, ensure no competing researcher edits. This is a **new research task**; #452 and #472 are historical completed research, not branches to rewrite. No local worktree claimed.
- **Permitted:** browser/academic/official primary-source research; **lawful public dataset metadata/record inspection where the provider and licence allow**; compute from read-only public rows in a bounded research workspace if available; all dataset/method/version/rights recorded. Do not commit raw underlying records or restricted files to repo. If unable to inspect archive, document that limitation instead of bypassing.
- **Forbidden:** contacting data custodians; account creation, restricted download/licence circumvention, data redistribution; **any Unity, source/Assets, code edits, approved spec/Decision Log/Current Milestone, \`active-tasks.md\` or physical Ops writes**, saved games, T3 code execution, CAPSIS/Samsara model runs, or Windows build. No changes to independent Scenario One manager's delivery scope/locks. No production or forest-parameter decision.
- **Publication:** GitHub-capable worker adds **only the specified Markdown result**, fetches/read backs and compares its result branch against pinned start, verifying exactly one added Markdown file and no unrelated edits. Marks #475 **[HANDED-OFF] / CLOSED** and links direct result for independent Multi-Rack Manager review **APPROVED AS RESEARCH / NEEDS CHANGES / REJECTED**. If Deep Research cannot write GitHub, **deliver the full sourced report in the research chat and tell Manager explicitly**; Manager publishes to exact branch/path with actual author attribution, verifies, then closes; **local/chat-only document is not a completed GitHub handoff**.
- No new contact/outreach or build authorisation follows automatically. Do not create further claims or run code unless Manager explicitly issues a fresh packet after review.
- Dashboard/Drive mirror is one-way from canonical Git, and its synchronization is **NOT VERIFIED** unless independently checked.

## 6. Paste-ready single-message dispatch

> **DEEP RESEARCH — CCF Irish marteloscope tree-by-tree data access**. Take CCF [#475](https://github.com/jermur788/CCF/issues/475), read the FULL handoff at https://github.com/jermur788/CCF/blob/docs/multirack-irish-marteloscope-inventory-deep-research-packet-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-IRISH-MARTELOSCOPE-INVENTORY-DATA-ACCESS-DEEP-RESEARCH-PACKET-20261114.md. Confirm whether **any named Irish marteloscope has a lawful, accessible complete per-tree ID, true within-plot XY, species, DBH and plot-polygon inventory** sufficient for faithful 2D reconstruction, especially Oak Park conifer/broadleaf, Curragh Chase, the public Integrate/GBIF Irish plot, and conditional Sitka Kilteel training forest. Do not assume published descriptions or I+ map pictures grant raw files. Separately identify whether real cut records and repeated same-tree field measurements exist for future validation. Rank real plot candidates by immediate 2D data readiness and independently by future longitudinal verification value. Select **one or two credible official data stewards with verified contact routes and UNSENT enquiry drafts**; do not contact them. If no plot qualifies, propose a clearly synthetic Irish Sitka first stand constrained by Irish evidence, not a fictional exact reconstruction. Distinguish raw-inspected/data licence verified from documentary/unverified, cite primary sources, record rights and missingness, and provide a specific GO/CONDITIONAL/NO-GO. Register actual model/provider, research branch and HEAD/worktree NONE as RUNNING on #475 before report edits if GitHub access exists; if not, tell Manager your actual identity/capability so truthful registration and publication can be done. Publish or return the **full sourced result** at \`Docs/Handoffs/MultiRack-Simulation-20261114/IRISH-MARTELOSCOPE-INVENTORY-COMPLETENESS-RIGHTS-AND-CONTACT-SHORTLIST-20261114.md\` on \`research/irish-marteloscope-inventory-and-rights-20261114-v1\`, with one-file GitHub readback and claim closure only after durable publication. **Research only: no outreach, coding, Unity, source/approved-spec/coordination changes, restricted dataset redistribution, saves or builds.** Return results for independent Manager review.
