# Manager independent review — Samsara2 offline spike #456

**Date:** 13 November 2026  
**Worker result:** [Published research handoff](SAMSARA2-OFFLINE-SPIKE-RESULT-20261113.md)  
**Manager verdict: APPROVED FOR RESEARCH-ONLY OFFLINE COMPARISON, WITH A MATERIAL COORDINATION PROCESS EXCEPTION.** The verdict approves the documented feasibility result, not independent re-execution or scientific calibration for Irish forestry. No production code or product spec is approved.

## Evidence directly inspected

1. The full 253-line GitHub handoff, the issue #456 timeline (BLOCKED, RUNNING, HANDED-OFF/CLOSED), and the release packet.
2. GitHub comparison from the worker's starting main baseline to its returned research revision: **one commit ahead, zero behind, exactly one added Markdown handoff**, and no committed CCF source, Unity, saves, specifications or live coordination changes.
3. Latest main coordination file, plus manager #424 issue comments, confirm that **no #456/Samsara2 entry was added to active-tasks.md**, even though the worker first correctly stopped to request one.
4. **Not independently inspected:** HP WSL run logs, actual machine state, outputs, source code of tools, local clean/dirty status or the 550-entry off-repo file manifest. Numerical and host-resource results below are **worker-reported, traceable by off-repo file paths and hashes**, not second-party reproduced.

## Requirement assessment

| Scope | Review finding |
|---|---|
| Installer and toolchain | **Worker-documented PASS:** uploaded archive hash matched; headless CAPSIS 22961, Java 8, R 4.5.3 and martelosim 0.1.3 ran in isolated HP WSL research directories. Installer provenance not verified. |
| Rights | **Research use adequate, distribution unresolved:** actual Samsara2 module notice GPL-3.0-or-later. Other bundled libraries and Prelenfrey demo data lack completed rights audit. |
| **Correction to prior static audit** | **No Samsara2 Java source is bundled.** Installer provides ~430 compiled model classes, resource/property files, veto and licence texts, not model .java files. The earlier claim that source was present was incorrect and is superseded here. |
| Native Prelenfrey demo | **Worker-documented PASS only after repairs:** shipped 22961 demo failed on uppercase FALSE, older inventory schema and absent corePolygon. Used official newer same-plot inventory and official polygon to run; three repeats ~41–45s and ~1.2–1.3GB peak RSS. As-shipped demo is NOT runnable unmodified. |
| Repeatability | **PASS identifying non-determinism:** no seed setter exposed; periphery generation, virtual saplings and later outcomes differ across repetitions. Three replicates are exploratory, not tight confidence bounds. |
| martelosim | **Worker-documented PASS for model run:** version 0.1.3 requires CAPSIS 22961 and completed packaged two-scenario 50-year run. Earlier stable 0.1.2 compatibility warning is superseded. PDF generation failed because the environment lacked Quarto Deno, not because the forest model failed. |
| Controlled study | **PASS for exploratory qualitative comparison:** three runs each of untreated / focal-tree-guided large-tree removal / broad small-tree thinning, 2023 and 2033 cuts under similar BA caps and 2053 outputs. Opposing structure, species and light outcomes were reported. |
| Treatment attribution | **NOT YET VERIFIED:** T1 removed large trees and no focal crop-tree IDs/adjacency were exported. Call it **focal-tree-guided removal of large competitors**, NOT confirmed crop-tree neighbourhood release. |
| Stand denominator and controls | **OPEN:** default exportCoreOnly=false includes generated edge trees; each treatment separately generated stochastic edge/sapling starting conditions. For stronger treatment inference rerun with shared prepared state, core-only metrics and documented selection/replicate variance. |
| Production boundaries | **GitHub scope PASS:** no source changes in the reviewed commit. No direct Manager verification of host/Unity cleanliness. All explicit CCF implementation and Irish-species inferences remain NO-GO. |
| **Gate 0 live ownership authority** | **PROCESS FAILURE:** release required #424's board entry BEFORE local changes. Worker first registered BLOCKED then continued after user authorised HP device repin, but the live coordination board was never updated. User approval of device/work does not make this an advance recorded board lock. Issue comments do not substitute for board authority. **Do not claim this gate passed.** |
| Durability/status | **PASS:** GitHub handoff published, issue #456 closed/HANDED-OFF. Dashboard/Drive sync NOT VERIFIED. |

## Decision boundaries and scientific interpretation

**GO:** An isolated, version-pinned Samsara2 and martelosim workflow is viable as a qualitative **independent offline forestry-model comparator**. Tree selection at similar removal BA can produce substantially different outputs. Adopt only the *research method*: controlled treatments, separate preparation from interventions, repetition, and transparent metrics.

**NO-GO:** Inference that this scientifically validates CCF game mechanics, numerical transfer to Ireland's Sitka/Oak/Beech, changes to any adopted Decision Log/spec, embedded GPL Java code in Unity, or a player-facing Prelenfrey expansion without dataset/terrain/rights assessment.

The reported 30-year trial is not a perfectly paired experiment: it generated randomized periphery/young-tree inputs separately by scenario, and exported scene rather than exclusively core values. Therefore qualitative differences are **hypothesis-generating**, not definitive measured causal treatment effects. The 2023/2033 removals are approximate matched BA, but select very different diameter distributions.

## Coordination exception — required correction, without fictional retrospect

- Keep #456 **HANDED-OFF/CLOSED**. Do not reopen or mark the experiment NOT EXECUTED.
- **Manager #424** remains the exclusive live-board writer. Ask #424 to record a **retrospective completed #456 lane and explicit Gate 0 exception**: actual Claude Opus 5.5/T3 Code worker, HP WSL host, research branch/worktree, user-repinned device, now closed, resource isolation reported. It must NOT be backdated or written as if approved in advance.
- No new execution may claim this oversight as permission to bypass the live board. The next worker must obtain prospective coordination entry before edits/runs; stop or mark BLOCKED otherwise.
- Off-repo source logs and data may remain on the HP research directory per packet; they are not independently reviewed here. Preserve manifest and research environment for optional follow-up.

## Next scientifically useful step (not released)

A small **separate follow-up** should: (a) prepare one shared treatment input with martelosim, (b) compare core-only output for T0/T1/T2, (c) identify focal IDs and quantify actual cut-tree-to-focal neighbour distances or honestly report inability, (d) use more replicates for ambiguous effects, and (e) keep third-party/data licence limitations separate. This is **research only** and does not authorise CCF integration. User's idea of a playable Prelenfrey forest needs a different rights/terrain/spec decision, not a silent expansion of this task.

## User-facing disposition

**Changed:** Spike results reviewed and bounded research GO accepted, with explicit corrected source/version and governance findings.  
**Passed:** GitHub docs-only scope, documented native and R model execution, reported treatment experiments and repeatability checks.  
**Failed/open:** required ownership-board entry, as-shipped demo, PDF reporting, shared core-only controls, local focal-tree attribution and independent access to raw logs.  
**Next:** #424 reconciles historical board exception; optional follow-on is parked until proper scheduling. Objective 1 still has priority.  
**User action:** none for accepting this research handoff.  
**Dashboard/Drive:** sync NOT VERIFIED; canonical product decisions and Unity unchanged.
