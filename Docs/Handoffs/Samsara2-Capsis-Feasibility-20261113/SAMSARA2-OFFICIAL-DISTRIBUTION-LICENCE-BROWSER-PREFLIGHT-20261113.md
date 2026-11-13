# Samsara2 official distribution / licensing — browser-only preflight

**Date:** 13 November 2026  
**Manager decision:** **OPTION 3 — BROWSER LICENCE AND INSTALLER AVAILABILITY CHECK ONLY**. This document **does not release #456**.  
**Status of #456:** WAITING / NOT RUNNING; no worker registered, local worktree, installer, Java/R/CAPSIS execution, Unity access, or authorisation for CPU-intensive research.  
**Parent:** [Samsara2 research handoff](SAMSARA2-CAPSIS-RESEARCH-FEASIBILITY-HANDOFF-20261113.md); [conditional experiment packet](SAMSARA2-OFFLINE-FEASIBILITY-SPIKE-CONDITIONAL-PACKET-20261113.md).  
**Evidence mode:** public official pages reviewed in browser; no vendor registration, downloads, licence file inspection, correspondence or installed execution.

## Verified from official published pages

1. **General Capsis public distribution:** https://capsis.cirad.fr/capsis/download describes a web/demo release, most recent listed installer `capsis-setup-web-release-6may2025.jar`, made for evaluation and containing specific listed models (`Abial`, `Fagacees`, `Genloader`, `ModisPinaster`, `Mountain`, `Organon`, `Salem`, `SamsaraLightLoader`, `Twoe`). **Samsara2 is not in that enumerated web release**. Registration form asks name, email, organisation/country and motivation; an email with the link is sent automatically. Registration for this general installer is **not evidence of licence to use or redistribute Samsara2**.
2. **Critical nuance: the current INRAE martelosim guide *does* publicly advertise a Samsara2-specific installer link.** At https://martelosim.lessem.forge.inrae.fr/articles/martelosim.html , its “Installing Capsis” section says “download capsis” and names **`capsis-Samsara2_4.2.7-22711_setup.jar`**. The current stable martelosim requires Capsis version **4.2.7-22711** and Java **1.8**. This is *potentially an official alternative* to requesting a custom installer. The public browser search verified the guide text, **not** that the actual link is currently accessible, what registration it requires, the binary's integrity/hash, or the included Samsara2 licence. Do not infer that Samsara2 can only be obtained privately.
3. **Model-specific rights:** https://capsis.cirad.fr/capsis/documentation/faq says core Capsis4 is LGPL; growth-model modules have **custom individual licences** under their module directories. A module **without an explicit distribution licence is not freely distributable** and belongs to its author. Neither a public link nor a free Capsis core implies model source-redistribution or a CCF embedding licence.
4. **Authors / contact:** https://capsis.cirad.fr/capsis/models lists Samsara2 as “a spatially explicit, individual based forest simulation model” and cites **Benoit Courbaud — benoit.courbaud@inrae.fr — INRAE LESSEM, Grenoble**. https://capsis.cirad.fr/capsis/documentation/howcanigetcapsis instructs users seeking an otherwise undistributed model to contact its author. In the event the special martelosim download fails or licence conditions are absent, author inquiry is the conservative next action.
5. **Wrapper is not the model licence:** https://martelosim.lessem.forge.inrae.fr/ and https://inrae.r-universe.dev/martelosim/doc/manual.html label **martelosim** under MIT and provide an R package install route. Those rights do not transfer to Samsara2 or Capsis.
6. **Resource impact:** https://martelosim.lessem.forge.inrae.fr/reference/prepare_samsara.html specifies `memory_perc=75` as a **configurable default allocation percentage**, not a direct measured memory usage. It is sufficient to flag contention risk, but actual RAM consumption and runtime remain unknown because no simulation ran.

## Precise gating outcome

| Question | Status |
|---|---|
| Does public core CAPSIS software exist? | YES — official registered demo/web installer |
| Does the normal public demo bundle Samsara2? | NO — not on enumerated list |
| Is a Samsara2-specific installer advertised by an official martelosim guide? | YES — named compatible version and link text |
| Can the link currently download without access or special registration? | **UNKNOWN** — not attempted/confirmed |
| Does the actual Samsara2 binary include a usage or redistribution licence? | **UNKNOWN** — no file inspection |
| Are academic/independent offline experiments permitted by that exact licence? | **UNKNOWN** until inspected or author clarifies |
| Does the module licence permit CCF inclusion/port or redistribution? | **UNKNOWN; do not assume** |
| Can #456 run without contention with Objective 1? | **NOT VERIFIED; scheduled local work deferred** |

**Verdict:** Browser access/licence preflight completed; local executable spike remains **WAITING / NOT RUNNING**. Not `BLOCKED — NOT EXECUTED` as a concluded installer failure, because the version-specific link has not been tested, no request sent and no licence grant inspected. If the future local worker cannot lawfully obtain the model, Gate 1 mandates `BLOCKED — NOT EXECUTED` rather than a fabricated benchmark.

## Management next step

1. **Keep #456 WAITING** while Objective 1 #443/#447 and the single-machine resource boundary are active; do **not** create a worktree, launch Capsis, or install.
2. When priority permits, Manager checks current machine CPU/RAM usage and agrees a **separate low-impact lane** with coordination writer #424 before explicit #456 release. Optionally restrict to obtaining and reading exact module licence before doing any model calculations.
3. The future worker must inspect the official version-specific link, hash/version, included `LICENSE` in Samsara2, and terms for **research/offline use** versus **code embedding/redistribution**, recording uncertainty. Author contact is appropriate if unresolved. No user credentials or personal details should be fabricated or submitted without user consent.
4. Only after the exact licence and ownership gate are clear can the executable Prelenfrey reproducibility test run. It must not overlap the current user/Unity qualification.

## GitHub ownership / evidence

- No edits to `main`, `Docs/Project/coordination/active-tasks.md`, Unity/C#, approved specs, Objective 1 or Objective 2 files.
- Browser-only research note on existing research-docs branch, not authority to change production roadmap.
- Dashboard/Drive sync not verified; no context authority files changed.
