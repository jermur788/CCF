# Manager #482 independent review — #479 blocked Irish GBIF archive and possible GBIF-hosted alternate

**Date:** 14 November 2026  
**Actual reviewer:** OpenAI ChatGPT GPT-6, GitHub/browser-only independent Multi-Rack Simulation research reviewer; no local worktree and no native machine/Unity lease. [Reviewer claim #482](https://github.com/jermur788/CCF/issues/482) registered before writing this document.  
**Worker handoff reviewed:** [#479 public-archive read attempt and report](https://github.com/jermur788/CCF/blob/research/multirack-gbif-irish-plot-raw-audit-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/GBIF-IRISH-RAW-MARTELOSCOPE-PLOT-DATA-COMPLETENESS-AND-RECONSTRUCTION-AUDIT-20261114.md). The execution claim is **already HANDED-OFF/CLOSED**, not currently an active T3 host process.  
**Verdict:** **APPROVED AS AN HONEST BLOCKED-ACCESS AUDIT ONLY**. Its negative result and stop condition are credible; **raw Irish tree inventory verification failed/NOT INSPECTED, and a real Irish marteloscope is NOT approved as ready for 2D reconstruction**. This does not reject the research idea or mean there are no Irish mapped stems.  
**Latest owner instruction:** Stop retrying the original INRAE publisher endpoint; consider **GBIF's own stored occurrence-download route** only as a conditional *different* pathway, with no claim it works for this dataset and no automatic authenticated request. Leave **data readiness BLOCKED** until actual original Irish records are retrieved and inspected.

## 1. Distinguish historical execution completion from the open blocker

- The #479 worker registered as **GPT-6 remote ChatGPT sandbox**, NOT the proposed Sol/T3/HP WSL owner, and reported isolated GitHub/doc-only changes. It attempted public downloads and logged **curl DNS resolution failures** for official GBIF and INRAE hosts in that environment, plus a separate download error. These are **execution-environment failures**, not proof the publisher server is universally down, a 404, empty country data, revoked licence or no plot in Ireland.
- The report correctly preserved unknown Irish plot IDs, counts, coordinates, polygon, species and DBH; no invented results, no BA/ha computation, no fixture. Its readback/one-file branch comparison was recorded in the #479 handoff. Neither that report nor this review has obtained raw data.
- **Claim status:** #479 remains **HANDED-OFF/CLOSED**, since the assigned bounded attempt was honestly finished and published. **Evidence/data-access status:** **BLOCKED / RAW ARCHIVE NOT INSPECTED**. Do not reopen #479 or relabel it as an imported-plot PASS. If a later differently scoped access attempt is authorised, register a **new separately owned claim/branch/worktree** after fresh ownership check, rather than editing the old report or retrying it indefinitely.
- Frozen source instruction: **Do not repeat** \`http://dynids.toulouse.inra.fr:8180/ipt/archive.do?r=trems_integrate\` or the HTTPS variant in the same failed environment. The GBIF listing points to this original publisher endpoint; its being listed does **not** mean the archive bytes are currently reachable.

## 2. GBIF-hosted occurrence export: documented capability versus unproven dataset recovery

**Independently checked official sources (not archive bytes):**
1. [GBIF: API Downloads](https://techdocs.gbif.org/en/data-use/api-downloads) confirms that a GBIF occurrence download can be requested with a dataset and country predicate in **Darwin Core Archive** (\`DWCA\`) format. Creation requires **a registered GBIF user and authenticated request**. Result compilation occurs asynchronously and yields a download file when complete.
2. [GBIF: Occurrence download formats](https://techdocs.gbif.org/en/data-use/download-formats) confirms that standard GBIF DWCA exports have interpreted \`occurrence.txt\` and original \`verbatim.txt\`, and can include original **verbatim extension tables** where available and requested. \`meta.xml\`, \`rights.txt\` and \`citations.txt\` provide schema, licence and citations.
3. The [GBIF API Downloads 'Verbatim extension data' section](https://techdocs.gbif.org/en/data-use/api-downloads) specifically documents the optional extension URI **\`http://rs.tdwg.org/dwc/terms/MeasurementOrFact\`** in a DWCA request. A request must explicitly include \`verbatimExtensions\` where needed; a simple occurrence CSV is not an adequate substitute for original DBH/height measurements.
4. [GBIF Data Processing](https://techdocs.gbif.org/en/data-processing/) describes publisher data ingestion, interpretation and verbatim preservation. This makes GBIF-hosted extraction a scientifically sensible **independent access route** from the inaccessible *publisher's* original ZIP, subject to actual ingestion and retention.

**Important limits:** Documentation establishes a **supported platform workflow**, not that this particular dataset has recoverable **Ireland occurrence rows**, a complete \`MeasurementOrFact\` extension, usable \`verbatimCoordinates\`, a real plot polygon or unrestricted distribution rights. The GBIF browser's occurrence-search filter is discovery, **not** proof that a complete per-plot inventory was retrieved. Neither this Manager nor #479 created a GBIF account, issued an authenticated download request, received a generated DWCA file or inspected the actual original stem rows. **Alternate success: NOT TESTED.**

**Conditional alternative, if separately authorised:**
- Query only the dataset key **\`2e102194-f384-4712-89a4-5db7a3fc409a\`** and **country=IE** through **GBIF occurrence search** for discovery; treat counts and filters as provisional.
- Request **GBIF-hosted DWCA with original verbatim terms and explicit \`MeasurementOrFact\` verbatim extension**, dataset/country predicates and correct authenticated account/citation permissions. Do not request simple CSV and assume it preserves all measurements. Account creation, use of account credentials or triggering an authenticated export requires a distinct authorised user action/workstream; **none is authorised/executed here**.
- On receipt, preserve GBIF download DOI/citation, original publisher/dataset version where recorded, exact export query, zip SHA-256, \`meta.xml\`, \`rights.txt\`, per-plot \`occurrenceID\`, measured species/DBH/height/status, \`verbatimCoordinates\` syntax, and extension join relationships. Separate original Irish plot-relative coordinates from GBIF's site-level latitude/longitude.
- **Strict stop gates**: zero/ambiguous Irish original plot IDs; missing/empty extension DBH; missing real within-plot XY, axis/origin/units; missing verifiable true boundary; unclear rights; or synthetic/regenerated records passed off as field measurements. Any of these prevents an “exact mapped plot” GO.
- If the GBIF-hosted export itself fails or omits critical fields, record one bounded failure and **STOP**. Do not loop publisher retries, infer hidden plot data or fabricate reconstruction. A direct **future, user-approved** data-steward query or labelled synthetic Irish Sitka research fixture remains an alternative, not a fallback asserted to be authentic.

## 3. Review-level tests and disposition

| Gate | Verdict | Meaning |
|---|---|---|
| #479 correct worker identity, bounded task completion and manuscript | **PASS** | One report on assigned branch; execution ended, no T3/Unity ownership invented. |
| Public GBIF registration, dataset metadata and CC BY licence mentioned | **PASS as documentation** | This licenses only the specific published released records, with attribution; not unrestricted I+/Teagasc/Coillte files. |
| Publisher archive bytes/Irish original stem rows retrieved | **FAILED ACCESS / NOT INSPECTED** | DNS/download failure in executing sandbox; remote service status not established. |
| Original Irish plot IDs, mapped XY, tree-by-tree species/DBH and real boundary verified | **NOT TESTED / BLOCKED** | No data/geometry; no valid import target chosen. |
| Actual executed marks and longitudinal same-stem observation for ecology | **NOT TESTED / NO EVIDENCE** | No real-world predictive accuracy claim. |
| Independent GBIF-hosted DWCA route proven for this dataset | **NOT TESTED** | Technical docs permit it; archive/extension content not verified. |
| No illegal/unsafe retry, game code, Unity, saves or contact action | **PASS for Manager review scope** | No original publisher URL request, account/GBIF export request, Unity, source, specs, coordination or outreach by this reviewer. |

**Final verdict: APPROVED AS A CORRECTLY BLOCKED DATA-ACCESS HANDOFF ONLY.** This is **not** a verified public Irish marteloscope inventory, not a current field-data accuracy demonstration and not an approval to build a precise 2D replica. Do not import #475's erroneous assertion that Curraghchase, Donadea, Oughaval and Glenart were “verified raw data.” A 2022 paper reported one Irish plot in the published 111-plot extract; the identity/current count still require actual records.

## 4. Manager recommendation and current release state

1. Respect the owner's stop instruction. **No more attempts at the original INRAE \`:8180\` archive** from this workstream; no automatic alternate credentialed GBIF export. This is a legitimate external access blocker, not a reason to keep a running job alive.
2. Preserve [#479](https://github.com/jermur788/CCF/issues/479) closed and its **raw-source readiness BLOCKED**. This independent review is the review receipt and independent disposition.
3. An eventual separately approved small follow-up may use **GBIF's own DWCA occurrence export**, not the publisher link, with explicit extension request. That is the only presently documented technical recovery route. It needs a GBIF account and a worker able to obtain and inspect original rows; no such resource or request has been released now.
4. The user can later authorise institutional outreach to **Teagasc** or **Coillte** if the public export fails or cannot provide polygons/measurements. No contact has been made; do not claim the organisations will provide data.
5. Meanwhile a **transparent synthetic flat Sitka stand** calibrated against independently justified Irish stand-level evidence is a possible independent research design, but it is *not* a reconstructed Irish marteloscope, and no specification or implementation is authorised here.
6. Do not block the separate Scenario One compact complete-loop delivery or modify its protections. All source, Unity, exact real plot, ecological calibration and owner product decisions remain with separately approved owners/specs.

**Dashboard/Drive synchronization: NOT VERIFIED.** No user action is required to close the research-audit review; authenticated GBIF export or institutional contact would require a future explicit user decision.
