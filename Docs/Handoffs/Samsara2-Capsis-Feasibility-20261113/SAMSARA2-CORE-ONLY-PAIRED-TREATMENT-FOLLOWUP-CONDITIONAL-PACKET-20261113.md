# CONDITIONAL follow-up packet — #461 Samsara2 paired treatment/core-only verification

**Status: WAITING / NOT RUNNING. NO NEW LOCAL EXECUTION AUTHORISED.** The original #456 GO is already approved for **offline research comparison only**. This packet identifies the *next research question*, not a production feature.

**Claim:** https://github.com/jermur788/CCF/issues/461  
**Review that created this need:** [Independent Manager review of #456](MANAGER-456-OFFLINE-SPIKE-INDEPENDENT-REVIEW-20261113.md)  
**Research basis:** [#456 full execution result](SAMSARA2-OFFLINE-SPIKE-RESULT-20261113.md).

## Purpose

Check whether the #456 reported structural/light/regeneration differences persist when all three treatments start from **the same prepared initial forest** and outputs measure **core-only** trees, not generated edges. For the focal-guided treatment, identify candidate crop/focal tree IDs and verify spatial proximity between selected cut trees and focal trees. This is a bounded quality follow-up, **not** a repeat of installer/setup feasibility or a CCF implementation request.

**Difficulty:** MEDIUM, due to the missing seed interface, stand preparation/copying, per-core denominators, per-tree geolocation and probabilistic output variance.  
**Best-suited model/provider:** Claude Opus 5.5 / T3 Code Linux or OpenAI Sol / T3 Code on an isolated local HP WSL research lane. One actual writer only.  
**Execution lane proposal:** branch **research/samsara2-core-only-paired-20261113-v1** from a fresh inspected safe main; worktree **/home/jer/Development/CCF-HP-Worktrees/CCF-Samsara2-CorePaired-20261113**. Neither is claimed, created or verified here.  
**Research-only data root:** existing **/home/jer/Research/Samsara2-20261113/** is historical output; new runs to an isolated sibling. Do not overwrite #456 evidence.  
**Final result path:** Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-CORE-ONLY-PAIRED-FOLLOWUP-RESULT-20261113.md.

## Gate 0: required release, safe ownership and resources

1. **Manager priority release is pending.** #461 stays WAITING until an explicit decision releases this follow-up and schedules it behind Objective 1.
2. **Manager #424 is the sole live ownership-board writer.** A **prospective** #461 lane entry must be published on live active-tasks.md before worker creates/edits worktree or runs the software. Do not repeat the #456 board exception; issue comments alone do not grant lock authority.
3. Actual worker must inspect the HP machine, prior toolchain presence, CPU/RAM, concurrency, space, branch/worktree collisions and unrelated dirty work, then register in #461 as RUNNING with real model/provider, branch, worktree, starting full HEAD and ownership evidence. If not safe, mark BLOCKED, no install or runs.
4. Do not launch any Unity/CCF game process, acquire Unity Editor lease, touch protected saves, source/assets/approved specs, run Windows builds or import GPL code. Don't change global Java/R or the old evidence root.
5. Confirm licences and provenance caveats: Samsara2's GPL applies to the **module**; actual distributed binary contains no Java source. Prelenfrey data lacks explicit redistribution licence. Keep binary/data off GitHub and results limited to research.

## Gate 1: freeze the initial population and denominator

- Pin previously verified CAPSIS revision 22961, Java 8, martelosim 0.1.3, R 4.5.3 and inputs by hash. Do not install mismatched versions silently.
- Use the **martelosim prepare stage ONCE**, exporting an identical prepared model inventory (including the same generated periphery/virtual saplings) for every T0/T1/T2 branch. Confirm equality with output hashes and per-tree IDs/positions before interventions; if exact pairing is unsupported, report why and use an alternative controlled design with explicit uncertainty.
- Set **exportCoreOnly=true** for comparable metrics; confirm core polygon and denominator empirically from the program; distinguish net core 1.1 ha from inventory coordinate rectangle and generated periphery. Do not mislabel whole-scene counts as per-core.
- Preserve baseline no-cut trajectory and no-cut mortalities as control.

## Gate 2: replay and check treatment identity

- Recreate no-cut, focal-guided large-competitor removal and broad thinning using exact #456 rules and cap, dates 2023 and 2033, ending 2053. Verify that replay uses the same initial tree inventory and initial periphery as control; use no CCF code or invented forest metrics.
- Identify focal crop tree IDs and coordinates via documented model output or inspect public command interface/signatures only. **Never decompile without explicit permission or claim a private model field is exported**. If focal IDs are inaccessible, report UNKNOWN honestly and STOP claims of crop-tree local release.
- For each intervention, calculate actual per-cut-tree nearest-focal-tree distance distribution (or give reason unobservable), cut tree species/size, and spatial distances among cuts. Document which ecological result is output versus speculation.
- If the first exercise still preferentially removes large trees, preserve the honest treatment name. Optional small weight sweep for genuine crop-tree release **only if bounded and documented**; do not retune to produce a preconceived result.

## Gate 3: repeatability and interpretation

- Minimum 3 runs per treatment; increase only when measured uncertainty needs it and resources permit. Capture initial state hashes, exact commands, timing/memory, means, SD and replicate variation.
- Compare core-only N, G, V, diameter classes, ground irradiance, recruitment, saplings, mortality, and species where data exists. Distinguish initialization stochasticity from downstream model stochasticity.
- Quote observed differences relative to #456 but never transfer numerical coefficients to Irish Sitka/Oak/Beech or call this external science validation. If core-only/paired setup changes the direction or makes it uncertain, publish that finding without forcing GO.

## Gate 4: durable result

- Publish final research Markdown at specified result path on the isolated research branch with exact input/output manifest, machine environment, model versions, commands, reproducibility notes, failures, and GO/NO-GO for controlled offline comparison.
- Keep large CSV/log/data/binaries off CCF GitHub; cite local paths and hashes accurately. No unsupported remote reproduction or main merge.
- Mark claim HANDED-OFF, close, and return to independent Manager for APPROVED/NEEDS CHANGES/REJECTED. Manager can decide whether any later research/feature spec is warranted.
- Dashboard/Drive sync report VERIFIED or NOT VERIFIED, not assumed.

## Copy/paste dispatch after explicit release **only**

> Take ownership of CCF #461 after its Manager priority and live coordination-board release. Follow the full conditional packet at https://github.com/jermur788/CCF/blob/docs/manager-456-research-spike-verdict-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-CORE-ONLY-PAIRED-TREATMENT-FOLLOWUP-CONDITIONAL-PACKET-20261113.md . Register your actual research lane/worktree/HEAD and worker before edits. Use one shared Samsara2 prepared inventory and core-only exports to rerun the #456 treatments; check actual focal-tree identity and cut-distance attribution. Objective 1 resource priority, no Unity, production CCF source, saves or Windows build. Publish result or a truthful BLOCKED handoff.

**User action currently required:** None. This follow-up is parked, not dispatched.
