# Six-paper forest biometrics and CCF methods — original-source integrity and evidence register

**Registered:** 2026-11-11. **Status:** Supporting methods research / transfer evidence only. **Approval:** No Scenario One rules, numerical targets, UI lessons, save schema, or player-facing claims authorised here.

All six original PDFs are separately present as **Game Dev project sources** in this conversation's supplied source set, alongside an indexed 111-page combined source volume and a 24-page extraction report. The filenames below are the exact names of the copies inspected for this register; some original filenames contain spelling errors. These SHA-256 values describe the **exact project-source bytes available in this review**, not a newly verified permanent Drive mirror or a publisher's official release checksum. Verify the corresponding bytes if they are moved or renamed. The original journal PDFs were not added to Git.

| ID | Original paper (DOI) | Exact provided PDF filename | Pages | SHA-256 |
|---|---|---|---:|---|
| P1 | Pommerening, Maleki & Haufe (2021), *Individual-based forest management*; 10.1016/j.foreco.2021.119677 | `individual based forest management.pdf` | 12 | `9e10179c80a6155223e0b24c8e2b4bdecdd293b6830af44c125bdbdf3c0422fc` |
| P2 | Pommerening et al. (2018), *Rating experiments in forestry*; 10.1371/journal.pone.0194747 | `Rating experiments in forestry.pdf` | 20 | `52b3edbb47dc6cde4dccd321b7664e9bf4aad065cb52525c96085a22202c1ae9` |
| P3 | Pommerening (2002), *Approaches to quantifying forest structures*; 10.1093/forestry/75.3.305 | `approaches to quantifying forest structures.pdf` | 20 | `4e404758eb1fcc14b196a2a5b93f275ad97651cfe02f3451d7e36edcbddc2f5f` |
| P4 | Pommerening et al. (2021), *CanopyShotNoise*; 10.1080/01431161.2021.1944695 | `CanopyShotNoise   An individual-based tree canopy modelling framework for projecting remote-sensing data and ecological sensitivity analysis.pdf` | 30 | `0970a559420c27a1d7a2e43f8b3e9979904bc93de6a56fb5b34e8dfa2a8c5328` |
| P5 | Myllymäki et al. (2024), *A Bayesian approach to projecting forest dynamics and related uncertainty*; 10.1016/j.ecolmodel.2024.110669 | `a beyesian approach toproject forest dynamcs.pdf` | 14 | `d41c5a6f9fb7a9b48856fe9bd71126e5525888a80c2dc52d90f7924cf9cd08db` |
| P6 | Pommerening, Szmyt & Duchiron (2024), *Revisiting silvicultural systems*; 10.1016/j.tfp.2024.100597 | `revisiting siliculturar systems.pdf` | 13 | `3e6f9fe6f52c2ebb621538901fd1682f6cdb1b582b7d4aecd92b1fda25cd72dc` |

**Derivative sources / verification inputs** (distinct from the original PDFs):
- `CCF_Six_Complete_Forestry_Research_Papers.pdf` — 111 pages, 109 original + two index/cover pages; SHA-256 `4f1d75578d25e8dd9621474ee98beb26afb8890e87106e19ac68c69336cc5e13`.
- `CCF_Six_Papers_Relevance_Evidence_Extraction.pdf` — analytical report, not primary evidence; SHA-256 `f8a699d96d5146d05f74ec1c5b2b2ba2206986665b0019472c61a3ba3d29f8bf`.
- `CCF_Six_Paper_Evidence_Extraction.md` — source Markdown for extraction; SHA-256 `2a89d7bedebbbc90e470254c6d64c26c61a4719d73d2fa602252edd76d431ff0`.

## Bias/independence and transfer warnings

- **Common authorship:** all six include Arne Pommerening. This is an overlapping research network, **not six independent lines of validation**. Treat as methods evidence and selected field/experimental observations, not an exhaustive representation of CCF practice.
- **P1/P2:** UK marteloscope data, with relevant Sitka and some Beech trials. P1 finds future crop-tree selection agreement generally higher than competitor selection; P2 compares low vs crown thinning and reports low agreement on exact marked trees. The two papers answer related but distinct questions; do not cite P2 alone for P1's crop-tree/competitor result. Neither statistic is a test of forestry quality or a player scoring coefficient.
- **P1 wind caveat:** stem slenderness and the effects of both dense unthinned neighbourhoods and release have context-dependent implications. Default Scenario One uses **storm model 0**, so neither pathway has simulated storm consequences. Do not frame release as automatically wind-safe or wind-dangerous; this is a documented simulation limitation.
- **P3:** structural species mingling and diameter differentiation may be described as spatial measures. They are **not independently validated biodiversity/habitat scores**; study boundary representation first, especially on an 80 m stand.
- **P4/P5:** modelling frameworks or fitted parameters outside Ireland are not accepted as Irish Sitka/Oak/Beech biology. P5 reports little advantage in one Hegyi-versus-RBA comparison; no replacement competition model authorised.
- **P6:** regeneration system taxonomy is conceptual; extraction rack geometry is not interchangeable with silvicultural regeneration strips. In player language refer to **areas with young trees / woodland regeneration**; the game's underlying grid is an internal representation, not a physical forestry site unit.

## Existing CCF sources required before making design claims

Read these *alongside* P1–P6 rather than elevating one school to exclusive authority:

1. `Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md` and `Docs/Research/RacksAndTreeMarking/CCF_Rack_Planning_and_Tree_Marking_Source_Extraction_v1.md` — photographed Irish tree-marking pages 84–87; publication identity remains unresolved, so do not present as a validated national regulation or universally mandatory standard.
2. `CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md` — Irish CCF transformation/wind/economics and qualification of imported examples; source-by-source checks remain required.
3. `CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`, `CCF_Ecology_and_Regeneration_Evidence_Composite.md` — existing accepted model evidence and more local context.
4. Current Multi-Scale Forestry Management V1 research index — UK machine-cab visibility concern; use this as a limitation of delegated competitor selection, while the game retains **player confirmation/review** and the Work Plan gate.

## Adoption status

**Accepted mechanics are unchanged.** The current 30% local competition-release lesson and related MFM ranking remain **accepted gameplay calibration** unless a later approved spec changes them. Future, separate proposals may consider recorded reasons for retaining a neighbour, a read-only structural signal (only after fixture tests), wind communication, and an offline Hegyi/RBA comparison. No new pre-demo objective, lesson pass condition, save field, simulation coefficient or frozen-reference change is authorised by this register.

**Repository publication status:** this register is published on a **docs-only proposal branch**, not merged into canonical main. A merge affecting canonical project context requires the normal context regeneration, manifest verification and one-way Drive mirror process.
