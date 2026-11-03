# Scenario One Tier-3 — Test-Volume Reduction Amendment — 2026-11-03

**Parent claim:** #231  
**Amendment record:** #238  
**Authority:** latest direct user instruction  
**Status:** ACTIVE AMENDMENT FOR THE RUNNING TIER-3 QUALIFICATION

## User intent

Reduce the planned Tier-3 test/gate execution volume by approximately **50%**, while concentrating effort on the tests that carry the highest product-risk and player-test-readiness value.

This amendment changes **how much duplicate/redundant qualification work is executed**. It does **not** authorize weakening assertions, lowering thresholds, rebaselining protected references, skipping a known failure, or calling an untested requirement passed.

Target: **about half as many test/gate executions as originally planned, while preserving the critical milestone protections.**

## Priority rule

Prefer, in order:

1. one current-candidate test that covers several important contracts at once;
2. a mandatory exact-candidate test required by the final packet;
3. a current-candidate test for high-consequence persistence/reference/commitment boundaries;
4. accepted immutable-ancestor evidence where the final production bytes for the relevant surface are unchanged;
5. omit low-value duplicate reruns that merely repeat already accepted evidence without materially increasing confidence.

Do not run two or three substantially overlapping gates merely because all exist. Choose the strongest/most integrated one unless a separate gate protects a distinct high-risk contract.

## MUST KEEP — highest-value current-candidate evidence

The running #231 qualification must still retain these milestone-critical protections:

### 1. Exact candidate compile/import sanity

One authoritative compile/import gate on the exact candidate.

### 2. Mandatory S1-C T-05 final-candidate rerun

This remains mandatory exactly as required by the original Tier-3 packet:

- current save schema v24 save/load;
- old-save behaviour;
- objective/progression/history chain;
- rendered `recorded / derived / not recorded` states.

Do not satisfy this only by inheritance.

### 3. Save / corruption / compatibility protection

Retain the smallest set of gates that together prove:

- current v24 round-trip;
- malformed/ambiguous data rejected before mutation;
- old/pre-feature save compatibility where required;
- management/provenance identity integrity;
- Legacy40 compatibility;
- frozen Reference Future v1 archive/integrity/continuation contract;
- no protected hash/reference silent rebaseline.

Where one integrated save/hardening gate proves several of these, prefer it over multiple overlapping reruns.

### 4. MFM and commitment-boundary path

Retain current-candidate evidence for the most important live player path:

`sample → fit/coverage → candidate judgement → 3D confirmation/spot-check → proposal → prescription approval → ordinary Work Plan → Work Plan approval → execution`

This evidence must still protect:

- player judgement;
- proposal != Fell mark;
- prescription approval != commitment;
- Work Plan approval remains commitment gate;
- invalid/unavailable fit cannot bypass approval;
- provenance survives into ordinary work/execution.

Prefer one integrated treatment-review/approval gate plus the complete-loop witness over rerunning every component gate separately.

### 5. Complete Scenario One loop / completion / solvency

Retain one explicit exact-candidate complete-loop witness through later intervention and understandable completion, including the important economy/solvency and objective-completion observations required for the player-test candidate.

### 6. Rendered player-facing evidence

Retain rendered evidence at **1280×720 and 1920×1080**, but reduce screenshot/test duplication.

Prioritize checkpoints that jointly demonstrate:

- Stand Map / Rack legibility;
- species truth/inspection where material;
- Annual Review/history;
- Work Plan previous/pending state;
- MFM candidate/proposal/approved-work distinctions;
- completion/review state;
- no critical clipping/overlap.

A small representative set at both resolutions is preferred over many near-duplicate captures.

### 7. Independent final review

Keep the independent final review. It should explicitly review the reduced-test rationale and flag any omitted gate whose absence creates a material blind spot.

## SHOULD INHERIT OR DROP WHEN REDUNDANT

Use accepted immutable-ancestor evidence instead of rerunning, where production bytes relevant to the requirement are unchanged and the handoff states the inheritance explicitly.

Good candidates for inheritance/reduction include:

- component-level Rack tests already subsumed by an integrated current-candidate Rack/Stand Map check;
- repeated pure species component gates where current candidate has no relevant production change and accepted parity evidence remains exact;
- repeated MFM component gates already covered by the integrated treatment-review/approval gate plus the complete-loop witness;
- duplicate Guided/Combined/PARITY rendered suites when one current integrated rendered suite demonstrates the same player-facing contract;
- repeated history cycles that add no new final-candidate evidence beyond the mandatory S1-C final-candidate rerun plus one integrated repeated-intervention witness;
- multiple equivalent save tests covering the same rejection/round-trip contract;
- repeated 720p/1080p screenshots of materially identical states.

## Approximate reduction target

The agent should actively redesign its remaining plan to achieve **roughly 50% fewer remaining test/gate executions** than it was planning before this amendment.

This is a directional target, not a mathematical requirement. If preserving all MUST KEEP protections results in a smaller reduction, preserve the protections rather than forcing the number.

If the run is already partly complete, count completed work as done and reduce the **remaining** planned executions by about half where feasible; do not redo completed tests merely to conform to this amendment.

## Failure / stop discipline remains unchanged

A real failure in any retained critical gate remains a blocker. Do not skip or replace a failing gate with inherited evidence merely to obtain green.

Do not:

- weaken individual assertions;
- lower thresholds;
- silently rebaseline anchors;
- edit production under #231;
- change save schema;
- omit the mandatory S1-C T-05 final-candidate rerun;
- omit malformed-save-before-mutation protection;
- omit frozen Reference Future / Legacy40 protection;
- omit the complete-loop witness;
- omit both-resolution rendered evidence;
- omit independent final review;
- build Windows;
- touch #194.

## Handoff reporting

The final Tier-3 handoff must include a short `Reduced test plan` section stating:

- which originally planned gates were omitted or inherited;
- why each omission was safe;
- which retained gate provides the relevant protection;
- approximate number of executions avoided;
- any residual confidence limitation caused by the reduction.

## Dispatch instruction

The running #231 agent should consume this amendment immediately and revise only the **remaining** qualification plan. Preserve already-completed valid evidence and do not restart the run.

Success criteria and Browser Overall acceptance authority remain unchanged.
