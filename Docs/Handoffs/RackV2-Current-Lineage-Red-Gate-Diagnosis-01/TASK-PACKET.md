# Rack V2 Current-Lineage Red-Gate Diagnosis 01 — Task Packet

## Purpose

Diagnose the current `guided-v2`, `combined-v2`, and `combined-legacy` Rack V2 failures independently of MFM-08, classify every failure family, and define the smallest evidence-backed next action without changing production or verification under this claim.

## Authority

Manager review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/MANAGER-REVIEW-20261031.md`.

Claim: **#197**.

Exact current integration: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`.

Historical comparison start from #195: `cfdc93bf3943125d21570bc5749a9338f52d1cce`.

MFM-08 candidate, for comparison only where useful: `e5fb424635f16182748efcb0539e8a49539495a9`.

## Facts established by #195

- `combined-v2` on current integration: 447 pass / 11 fail, same as MFM-08 candidate and historical feature start.
- `combined-legacy` on current integration: 440 pass / 8 fail, same as MFM-08 candidate and improved from 416 / 32 on historical feature start.
- `guided-v2` remains red; the compared first failure line matches candidate/feature-start behavior, and current integration also has fresh-rack-review sequence failures.
- Current integration does not contain MFM-08, so those failures cannot be attributed to MFM-08 merely because they also occur on the MFM candidate.

## Scope

Diagnosis/read-only by default.

Allowed:

- Git/GitHub history inspection;
- current source/test/packet/spec/decision inspection;
- existing #195 log/receipt analysis if accessible;
- bounded offline reproduction that changes no tracked files;
- precise historical comparisons needed to classify a failure.

Not allowed:

- production edits;
- test/fixture/expected-value edits;
- protected-anchor changes;
- silent baseline updates;
- Unity lock acquisition without a separately released serialized phase;
- Windows build.

## Required classification

For each failing assertion or tightly related failure family classify as exactly one of:

- **CURRENT PRODUCTION DEFECT / REGRESSION**
- **STALE VERIFICATION CONTRACT**
- **INHERITED KNOWN FAILURE**
- **ENVIRONMENT / RUNNER ISSUE**
- **UNRESOLVED — MORE EVIDENCE REQUIRED**

Every classification must cite concrete repo/runtime/log evidence. Do not infer green status from age or similarity.

## Questions to answer

1. What are the exact failing assertions for all three gates on `ce673714...`?
2. Which failures already existed on `cfdc93bf...` and which appeared later?
3. For later failures, what is the first responsible lineage/commit where practical to establish?
4. Does any current failure invalidate accepted Rack V2 / D-064 behavior, or is it a stale/orthogonal harness expectation?
5. Are the fresh-rack-review sequence failures production behavior, expected-state drift, or verification drift?
6. What is the smallest next repair packet, if any?
7. Can the repair be verification-only, or does evidence require production work?
8. Which gate(s) would need rerun to close the resulting repair without unnecessary broad regression?

## Execution

- **Difficulty:** Medium — multi-gate lineage diagnosis with potentially mixed failure causes.
- **Preferred provider/model:** DeepSeek for browser/static diagnosis; escalate to an independent stronger reviewer only if material ambiguity remains.
- **Lane:** browser/GitHub lane #162.
- **Writable production worktree:** none.
- **Claim:** #197.

At start, refresh `Docs/Project/coordination/active-tasks.md` and mark #197 `[RUNNING]` with provider/model and the exact source SHAs being inspected. Do not claim a Unity lane.

## Handoff

Publish:

`Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/HANDOFF.md`

Include:

- exact three-gate failure inventory;
- classification table with evidence for every family;
- lineage/first-divergence evidence where established;
- explicit impact assessment on accepted Rack V2 / D-064 behavior;
- explicit statement that MFM-08 production was not modified;
- proposed smallest successor repair/verification packet if needed;
- exact unresolved questions if evidence remains insufficient.

At handoff mark #197 `[HANDED-OFF]`, link the handoff and close the execution claim.

## Success token

`RACK V2 CURRENT-LINEAGE RED-GATE DIAGNOSIS COMPLETE — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line

`Run Rack V2 Current-Lineage Red-Gate Diagnosis 01 from Docs/Handoffs/RackV2-Current-Lineage-Red-Gate-Diagnosis-01/TASK-PACKET.md under claim #197 in browser/read-only lane #162. Diagnose guided-v2, combined-v2 and combined-legacy on exact current integration ce6737146a7aa9ea953d7db0ca93b103f6c2c996 using #195 receipts, current contracts and Git history. Classify every failure family before proposing repair. Do not edit production/tests/fixtures, change anchors, or acquire Unity unless a separate serialized phase is released. Publish the required HANDOFF.md and return the exact success token or blocker.`
