# scen1 — Manager #424 verdict on #484 offline boundary-competition research

**Date:** 14 November 2026, UTC  
**Manager / reviewer:** OpenAI GPT-6, Scenario One Implementation Manager [#424](https://github.com/jermur788/CCF/issues/424).  
**Worker:** Anthropic Claude Sonnet 5.5 via Claude Code, local T3 Code; actual worker correctly registered [before work on #484](https://github.com/jermur788/CCF/issues/484#issuecomment-6874426598). Different from packet recommendation; actual registration is controlling.  
**Worker status:** [#484](https://github.com/jermur788/CCF/issues/484) **HANDED-OFF / CLOSED**.  
**Manager verdict:** **APPROVED — TIER 1 OFFLINE RESEARCH EVIDENCE, WITH BOUNDED HYPOTHETICAL-CONTEXT LIMITATIONS.** This is not a production/specification/Unity/real-landscape acceptance.  
**Next gate:** one **owner product decision** — is the small managed plot physically part of a larger surrounding forest, or an isolated bounded property surrounded by open land? The answer determines whether a *separate* native Tier 2 boundary-context comparison is worth preparing. No new native work is released here.  
**Naming preference:** new Scenario One handoff filenames and visible document titles begin with **`scen1`**; preserve historic paths unchanged.  
**Dashboard/Drive:** NOT VERIFIED.

## 1. Inspected GitHub evidence and custody

Actual pushed [worker branch](https://github.com/jermur788/CCF/tree/research/scenario-one-focal-ci-offline-20261114-v1) compared with its registered original main start: **two commits ahead, exactly nine new/added allowed files, zero existing source, Unity assets, Project/canonical specs, live coordination, save/profile or historical reference changes**. These files were read from GitHub by this Manager:
- [#484 final handoff](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/TIER1-FOCAL-BOUNDARY-CI-HANDOFF-20261114.md)
- [Full results report](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/FOCAL-BOUNDARY-CI-TIER1-RESULT-20261114.md)
- [Script](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Tools/Research/ScenarioOneFocalBoundaryCI/focal_boundary_ci.py), [tests](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Tools/Research/ScenarioOneFocalBoundaryCI/test_focal_boundary_ci.py), [README](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Tools/Research/ScenarioOneFocalBoundaryCI/README.md)
- `Tools/Research/ScenarioOneFocalBoundaryCI/results/{results.json,focal-rows.csv,external-stems.csv,run-meta.json}` are the four additional authorised new files.

The original #460 local research evidence directory was *worker-inspected read-only* and verified against pinned results/harness hashes. **61/62** manifest entries matched; the sole later-written closure receipt mismatch is not an input. Eight real focal tree IDs and all 82/189 source recorded finite competition indices were reconciled at a single **Year 0 before-access-felling** stage. The worker reports 89 original files retained size, mtime and mode; Manager cannot independently see host filesystem or physical worktree cleanliness from GitHub. No raw original tree data were copied into GitHub.

The worker reports **30/30** unit tests and three byte-identical deterministic result runs. Direct source reconstructions differ from recorded native finite CI by at most **2.4×10⁻⁶**. The Manager read the script/test source and compared resulting small derived rows; **the Manager did not rerun local Python, raw-data tests, Unity or protected hashes**, so these are worker-reported execution evidence reviewed in GitHub, not a second independently executed measurement.

## 2. Core quantitative result and correct interpretation

Current source `ForestEcologyController.UpdateCompetition` has an **8 metre Hegyi neighbour cutoff**, with each living neighbouring tree's DBH ratio weighted by inverse distance. Calculation tested original 20m and 30m all-age-20-Sitka candidates from #460, and one **HYPOTHETICAL surrounding forest** extending source 1.9m lattice with target stocking ~2,100/ha, thinned deterministically, extra Sitka aged 20, mean original candidate DBH (no field-survey evidence outside property). The external-tree example is **a sensitivity hypothesis, not a reconstructed real landscape**.

| Candidate / real focal | Distance to edge | Verified finite CI | CI with hypothetical forest | Change |
| --- | ---: | ---: | ---: | ---: |
| 30m P0000 | 0.60m | 1.8469 | 6.1360 | +4.2892 |
| 30m P1504 | 0.60m | 3.4087 | 6.2904 | +2.8817 |
| 20m P0101 | 2.75m | 5.4487 | 8.4977 | +3.0490 |
| 20m P0708 | 2.81m | 6.9917 | 10.8096 | +3.8179 |
| 30m P0209 | 3.60m | 6.1785 | 7.4254 | +1.2469 |
| 20m P0604 | 7.07m | 5.4590 | 5.7749 | +0.3159 |
| 20m P0504 | 8.83m | 6.0594 | 6.0594 | zero |
| 30m P0807 | 13.65m | 6.0762 | 6.0762 | zero |

The result is **material for some sampled edge trees if comparable external trees really exist**. A finite-versus-buffered CI difference is *not* a demonstrated 3D canopy/light change, annual DBH change, mortality, seed arrival, storm effect, Oak/Beech success or 40-year simulated outcome. Some #460 original DBH jitter was generated under a finite-world assumption; a genuine buffered forest could have a different starting inventory. Near-boundary focal anchors, especially the 30m corner P0000, are not representative of whole-stand means; no error-percentage generalisation is warranted.

**Crucial negative control:** if the real landscape outside the bounded plot is **open** and no external trees exist, the finite CI is correct. Both zero-external controls and >8m interior controls passed. The study should never be cited as proving that “the game's edge modelling is wrong”. A two-species instead of three-species forest would not change the missing-neighbour mechanism if a larger real forest surrounds the managed parcel.

## 3. Manager review of scope and evidence

| Gate | Verdict |
| --- | --- |
| Original raw-data access/source-stage reconciliation | **PASS — worker-provenance verified by #460 hashes and report; not re-opened locally by Manager** |
| Source arithmetic / focal-selection audit | **PASS for the stated narrow study**; original source CI is reproduced closely, sources and assumptions explicit |
| Numerical negative controls | **PASS worker-reported** (zero-external equals finite, interior and >cutoff no effect, ID/schema validation) |
| Reproducibility and code location | **PASS source/readback; actual local execution worker-reported** |
| Changed paths / single-writer scope | **PASS GitHub: exactly nine new allowed files only** |
| Hypothetical outside-forest input | **DESIGN HYPOTHESIS; not a real measured neighbour dataset** |
| 30–40-year growth / three-species visual maturity | **NOT TESTED** |
| Unity runtime, visual comparison, save-router, Reference Future replay | **NOT RUN** |
| Learning benefit, actual landscape ownership/exposure | **UNKNOWN / OWNER DECISION** |
| Integration, spec, user playtest / Windows build | **NOT AUTHORISED** |

No new actual forestry coefficients, changes to D-059 three-species parity, D-060 complete first external demo, D-065 MFM, D-071 five-rack heritage, D-077 not-yet-executed skip design, protected Legacy40/Enlarged80/model-0/v1–v24 saves or frozen Reference Future v1 are made.

## 4. Owner product choice (STOP before native dispatch)

**Option A — Managed plot WITHIN a larger forest.** The player marks and manages 20/25/30m *teaching area*, while real adjacent surrounding trees, canopy/competition and seed context may exist and should influence biology. This best fits an immersive “manage a small part of a Sitka plantation” narrative and may enable more representative CCF interactions, but requires verified support for management polygon vs simulated wider forest, ecologically true neighbours, map/eligibility, 3D navigation, site/seed/density denominators and separate safe save world identity. The Tier 1 numbers justify *considering*, not immediately executing, one further native check. **Manager recommendation for owner consideration: A**, subject to whether the added engineering effort serves the demo and can be made safely disjoint from historical worlds.

**Option B — Isolated bounded property with open land outside.** Finite/no-external CI is physically appropriate. This has a much simpler footprint and avoids inventing forest beyond the boundary, but creates a deliberately edge-dominated, unusually tiny training property. It should be described as an *educational training abstraction*, not a self-contained typical Irish CCF production stand. **If owner selects B, do not commission a buffer-specific native Tier 2 study**; focus later on real 30–40-year biological growth and graphical maturity *within the selected bounded-world assumption*.

**NOT an acceptable unlabelled blend:** surround the playable area with decorative forest meshes and claim they shelter/seed/compete biologically. Decorative backdrop may improve aesthetics but does not correct model biology.

**Product question for the owner:** “Should the compact Scenario One teaching area be a managed part of a larger living forest (A), or a genuinely isolated small property with open land beyond it (B)?” No accepted user answer is recorded yet. No Tier 2 code/Unity/physical machine lease is released by this verdict.

## 5. Next steps and ownership

1. **Now:** Manager records #484 **APPROVED for Tier 1 research evidence** and closes its review disposition without reopening completed worker claim. No duplicate independent study or repeated test is needed to re-prove original eight rows.
2. **Owner chooses premise A or B**; do not silently substitute Manager recommendation for acceptance.
3. **If A:** Manager first prepares a **disjoint, smallest-possible technical feasibility SPEC/packet** (forest-world context and save isolation are cross-system and therefore SPEC REQUIRED before production); if a disposable native measurement can be safely performed without new production geometry, split narrow research qualification from broader architecture work. Separately claim local T3 Code owner/branch/worktree, inspect current protected machine state, acquire exclusive Unity lease and issue exact Tier 2 packet before any native run. Do not silently build a persistent halo or mutate reference saves.
4. **If B:** no buffer experiment; retain small-site abstraction and plan separate evidence-bounded 30–40-year Sitka/Oak/Beech outcome/visual witness only after scene/profile and safety gates, without inventing growth predictions or species reduction.
5. Reconcile future guided-versus-independent same-start **two-run gameplay** with separate save/router, years/history and comparison UX under a four-file **DRAFT first**, then independent approval before coding. Neither A nor B itself approves that feature.

**User action required:** owner answer to A/B only.  
**Dashboard/Drive mirrors:** not verified.  
**Naming convention for all future authored Scenario One handoffs:** title and filename prefix **`scen1`**. Existing published GitHub handoff links remain unchanged.
