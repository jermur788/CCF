# PLAYER-TEST-BROWSER-LANE-02 — Review of new PARITY-07 and MFM-04/05 returns

## Scope

Browser/source review under #110 after the local T3 #106 lane returned new fixes/reviews/transplants. This review does not execute Unity, local worktrees or unpublished commits. The question is whether the returned exact candidate is inspectable and whether the evidence supports a Manager verdict without overclaiming.

## PARITY-07 — player-facing species truth

### Returned evidence

OpenAI 6.1 Sol independently found two blocking wording defects in the published `1e7b941` candidate:

1. the shared-species note could be read as denying represented species-specific crown geometry/relaxation;
2. the broadleaf two-lift/Oak grade-benefit wording was unconditional despite `BroadleafMarketActive` gating and legacy-save behaviour.

T3 reports a bounded Claude Sonnet 5.5 fix at `478bd84`, six new checker assertions, `PARITY07_COPY PASS checks=83` and `THREE_SPECIES_TIMBER_PASS checks=73`.

### Browser inspection result

`478bd84` is **not resolvable from GitHub**, and `feature/parity-07-player-facing-species-truth-v1` still publishes an older tip. Therefore the reported fix cannot be source-reviewed from the browser lane. The reported change is directionally consistent with the independent review, but issue text is not a substitute for inspecting the exact bytes.

### Verdict

**NEEDS CHANGES — PUBLISH EXACT PARITY-07 FIX BEFORE MANAGER SOURCE ACCEPTANCE.**

Required next action for #106:

- push/publish the exact `478bd84` fix or a clean equivalent candidate with the same intended delta;
- publish/update the handoff with the exact GitHub-resolvable SHA;
- preserve the 83/83 copy and 73 timber results;
- then perform Unity compile and 1280×720 / 1920×1080 rendered qualification on that exact candidate or a proven byte-identical descendant.

No new product blocker is indicated by the returned description.

## MFM-04 — deterministic prescription core

### Returned evidence

OpenAI 6.1 Sol identified a real fail-open defect in the published static candidate: null/invalid coverage could produce an empty safeguard report whose `AllWithin` returned true. T3 reports a Sonnet fix `1b6ba3e` adding explicit invalid-input state/reason, making `AllWithin` false for invalid input, adding four negative checks, keeping all five MFM digests unchanged and raising the focused suite to 240 checks. OpenAI 6.1 Sol reportedly re-reviewed that delta PASS. T3 then reports a bounded transplant onto the current working lineage at `a8d3ccd` with 240/240 offline PASS.

### Browser inspection result

Neither `1b6ba3e` nor `a8d3ccd` is resolvable from GitHub. The published feature branch remains at the earlier handoff tip `5b39001542ec654f0b8159096e8c2b39302ea3a1`.

The reported fix is the correct class of repair for the independently identified fail-open problem, and the unchanged digests are useful evidence, but the browser lane cannot independently verify the implementation or transplant bytes.

### Verdict

**NEEDS CHANGES — PUBLISH EXACT MFM-04 FIX/TRANSPLANT BEFORE MANAGER SOURCE ACCEPTANCE.**

Required next action for #106:

- publish `1b6ba3e` or the exact reviewed equivalent;
- publish the bounded current-lineage transplant `a8d3ccd` (or successor) without stale branch history;
- expose the exact changed files/diff and updated handoff;
- retain the OpenAI 6.1 Sol independent-review evidence, 240/240 checks and unchanged five digests;
- then run the focused Unity compile/non-mutation gate on the published exact candidate.

## MFM-05 — spacing-only Crop Tree candidate system

### Returned evidence

The original `4b54af0` candidate is published and its semantics were already browser-reviewed. Claude Sonnet 5.5 Medium subsequently independently reviewed it PASS: spacing-only Sitka-adult inspection candidates, deterministic ordering, no hidden quality ranking, no Crop Tree creation/bulk confirmation, individual non-mutating navigation, preserved MFM-04 calibration/digests. T3 reports a bounded transplant onto the reviewed MFM-04 descendant at `01a2836`, with MFM-05 95/95 plus MFM-04 240/240 and byte-identical repeats.

### Browser inspection result

`01a2836` is not resolvable from GitHub and the published MFM-05 feature branch remains at the older handoff tip `eccc2fd998d654a990e213d39bd83eeed3496229`. The independent review of the original candidate is valid evidence, but it does not prove that the unpublished current-lineage transplant is byte-correct.

### Verdict

**NEEDS CHANGES — PUBLISH EXACT MFM-05 TRANSPLANT BEFORE MANAGER SOURCE ACCEPTANCE.**

Required next action for #106:

- publish the exact bounded MFM-05 transplant on the reviewed/published MFM-04 descendant;
- preserve the already-passed Claude independent review, 95/95 focused checks, 240/240 MFM-04 regression and byte-identical repeats;
- run the focused Unity compile/non-mutation gate on the published combined candidate;
- do not claim MFM-06/07/08 complete.

## Aggregate decision

The returned technical direction is credible and no new product/design blocker is identified. The blocking issue is **publication/evidence completeness**: the new fixes/transplants exist only as local/issue-reported SHAs and therefore cannot receive browser Manager source acceptance or published-integration status.

**Aggregate verdict: NEEDS CHANGES — PUBLISH EXACT CANDIDATES, THEN CONTINUE EXISTING UNITY/RENDERED GATES.**

No Windows build.
