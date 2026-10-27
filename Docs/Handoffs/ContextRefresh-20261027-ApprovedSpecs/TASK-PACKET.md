# PROJECT CONTEXT REFRESH — APPROVED S1-C / S1-D / THREE-SPECIES PARITY SPECS

**Task ID:** CONTEXT-REFRESH-20261027-S1CD-PARITY  
**Class:** Context publication / infrastructure only  
**Overall Manager:** ChatGPT Game Dev

## Why this task exists

The previously published project-context snapshot is `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`.

Canonical Git has since gained approved specification authority for:

- `specs/scenario-one-review-history-v1/` — Scenario One Review / Management History;
- `specs/scenario-one-repeated-interventions-v1/` — Scenario One Second and Subsequent Interventions; and
- `specs/scenario-one-three-species-parity-p0/` — Scenario One Three-Species Parity P0.

It has also gained Manager-reviewed task/handoff authority for those workstreams, including the accepted three-species evidence/parameter policy and the first parity requalification packet.

Therefore the older generated mirrors/composites and platform attachments are no longer a complete snapshot of current canonical authority.

## Goal

Regenerate and republish the complete project-context mirror set from one clean current Git SHA, following the established `Tools/ProjectContext` workflow and the prior Workflow Capability Sync handoff.

## Preconditions

1. Refresh `origin/main`.
2. Read live `Docs/Project/coordination/active-tasks.md`; it is excluded from the context snapshot and must not be baked as stale authority.
3. Choose one clean `main` SHA that contains the approved S1-C, S1-D and three-species parity specs plus their Manager review authority.
4. Do not include uncommitted work from any dirty primary checkout.

If canonical context changes again before generation starts, use the newer clean `main` SHA and report it explicitly.

## Required work

- run the established project-context generator from the chosen clean SHA;
- regenerate the full mirror set and both composites;
- verify deterministic rerun identity;
- verify manifest/hash consistency;
- publish the permanent Drive mirrors using the established one-way Git → Drive process;
- update the Project Context Guide factual SHA references if required by the established workflow;
- verify Drive readback byte identity;
- prepare the current ChatGPT/Claude platform attachment set;
- publish a GitHub-tracked handoff with exact context SHA, hashes, Drive readback and any platform user action still required.

## Restrictions

No gameplay code.  
No Unity run.  
No save/schema change.  
No Rack, Review/History, repeated-intervention, species-parity or Multi-Scale Forestry Management mechanics.  
Do not hand-edit generated composites/mirrors.  
Do not use a dirty preview as a context commit.  
Do not manually edit generated dashboard state.

## Required output

Publish:

`Docs/Verification/ContextRefresh-20261027-ApprovedSpecs/HANDOFF.md`

Report:

- exact source Git SHA;
- generated mirror/composite count;
- composite hashes;
- deterministic rerun result;
- manifest/hash verification;
- Drive publication/readback result;
- ChatGPT platform refresh status;
- Claude platform refresh status;
- dashboard availability.

## Exit

`PROJECT CONTEXT REFRESH COMPLETE — PLATFORM STATUS RECORDED`

or an explicit blocker.