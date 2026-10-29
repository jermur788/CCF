# PLAYER-TEST-BROWSER-LANE-01 — MFM-04 / MFM-05 static return review

## Identity

- Parent: #91 `PLAYER-TEST-OVERNIGHT-01`
- Browser lane: #105
- MFM-04 claim: #93
- MFM-05 claim: #100
- Execution/review lane: #106
- Browser inspection model/provider: OpenAI GPT-5.6 Sol, GitHub/browser-only manager lane.
- Current integration inspected: `integration/scenario-one-player-test-ready-v1` = `aca74acacc565f72ab802177cf69ca6ebb54d637`.

### MFM-04
- Branch: `feature/mfm-04-deterministic-prescription-core-v1`
- Exact production/check candidate: `71674e42c9fd186146aef1efee529019a68e7766`
- Worker: Claude Opus 5.5 / Anthropic
- Static result: `PRESCRIPTION CORE QUALIFIED` only

### MFM-05
- Branch: `feature/mfm-05-crop-tree-candidates-v1`
- Exact production/check candidate: `4b54af047a8cdbfc369cc477e9b22fe8c60a9a93`
- Worker: OpenAI GPT-6.1 Sol / Codex harness
- Static result: `CROP TREE CANDIDATE SYSTEM QUALIFIED` only

## Sources inspected

- Live #93 and #100 bodies/comments.
- `Docs/Handoffs/MFM-Implementation-04/HANDOFF.md`.
- `Docs/Handoffs/MFM-Implementation-05/HANDOFF.md`.
- Approved `specs/multiscale-forestry-management-v1/spec.md`, especially U1/U2/U5, P2, §§5–9.
- Candidate `ScenarioOneCropTreeCandidates.cs` at MFM-05 exact SHA.
- Git lineage against the current integration branch.

## MFM-04 findings

1. The static core stays within the approved pure-domain boundary: deterministic coverage, fit/proposal-domain logic and versioned calibration; no UI, save/schema, Work Plan mutation, approval state or `TreeMarkType.Fell` proposal is introduced by the returned slice.
2. The area-parity repair changes the measurement method to the MFM-03-aligned strip/exact-interval method while keeping the accepted 0.1 m step and calibration values unchanged. The handoff reports 236/236 checks, byte-identical repeats and area parity ≤0.001 m².
3. The handoff keeps the 30% teaching release, 40% safeguard, 8 m teaching spacing/Hegyi separation, MFM-02 geometry and frozen Reference boundary explicit.
4. It remains **not Unity-compiled and not independently reviewed**.
5. The branch is stale/diverged relative to the live integration line: `aca74ac..71674e4` reports merge base `72dbee8`, with the MFM branch 55 commits behind live integration. Therefore `71674e4` is evidence for the static slice, not a candidate that may be integrated without controlled reconciliation onto current integration.

## MFM-05 findings

1. The live task is the spacing-only Crop Tree candidate system, not a combined proposal UI/execution-policy slice.
2. The implementation adheres to P2/§8 at source level: candidates are Sitka adults only; spacing identifies where to inspect; diameter/height/competition/form/vigour/dominance are not selection inputs; no candidate becomes a Crop Tree in this class; every actionable target remains `Open`; no bulk-confirm API exists.
3. Existing/manual Crop Trees and sample Crop Trees act as blockers; broadleaves, Fell-marked, dead/missing and source/sample stems are excluded. Confirm/reject decisions are caller-supplied history, not generated authority.
4. Next/Previous navigation is individual, deterministic, skips decided entries and does not wrap or mutate state. This is consistent with the spec's efficient individual-navigation requirement while preserving 3D judgement for later MFM-07/08.
5. The returned static gates are substantial: 95/95 focused + unchanged MFM-04 236/236 regression, two-process byte-identical outputs, and the five MFM-04 digests unchanged.
6. It remains **not Unity-verified, not integrated, and not a complete candidate flow**. 3D-grounded confirm/reject enforcement, persistence/provenance, proposal approval, Work Plan commitment, rendered UX and workload qualification remain later owned work.
7. MFM-05 was implemented by OpenAI 6.1 Sol, so an independent same-provider review would not satisfy cross-provider independence. Use Claude Sonnet 5.5 Medium by default for the independent review if available; escalate to Opus 5.5 Low only with a recorded reason.
8. The MFM-05 lineage also diverges from current integration: its base `68ad3f3` has merge base `238bcaa` against `aca74ac` and is 20 commits behind the live integration line. A controlled transplant/rebase onto current integration is mandatory before Unity/integration qualification.

## Browser verdict

**MFM-04: APPROVED FOR OPENAI 6.1 SOL INDEPENDENT REVIEW + CURRENT-INTEGRATION RECONCILIATION + UNITY COMPILE.**

**MFM-05: APPROVED FOR CLAUDE CROSS-PROVIDER INDEPENDENT REVIEW + CURRENT-INTEGRATION RECONCILIATION + UNITY COMPILE.**

Neither verdict is `INTEGRATED`, `UNITY VERIFIED`, full MFM V1 acceptance or permission to bypass MFM-06/07/08.

## Exact bounded packet for T3 Code claim #106

### A. MFM-04 first
1. Refresh integration, #91/#93/#100/#105/#106 and live `active-tasks.md`.
2. Use OpenAI 6.1 Sol for an independent source review of exact MFM-04 static candidate `71674e4` against D-065/spec and MFM-03 accepted calibration. Verify the exact-interval area change preserves classification precedence/calibration and does not create hidden quality, broadleaf release, Fell proposals, approval or Work Plan mutation.
3. Reconcile/transplant the MFM-04 production + verification slice onto a clean descendant of the **current** integration branch. Do not merge 55 commits of stale branch history or overwrite newer integration work. Resolve overlaps by current authority, not by taking the old branch wholesale.
4. Rerun the MFM-04 offline gate on the reconciled descendant and require the accepted calibration/digests or an explained, authority-approved reason for movement. No silent rebaseline.
5. Unity import/compile the reconciled MFM-04 slice. Run focused live geometry/coverage checks sufficient to establish that the pure code compiles and consumes current Rack V2 geometry without mutation. No rendered UI claim is required for MFM-04 itself because it adds none.
6. If green, publish the exact reconciled MFM-04 SHA/evidence and make it the base for MFM-05 reconciliation. Integration remains a separate explicit step.

### B. MFM-05 on top of reviewed MFM-04
7. Independently review exact MFM-05 candidate `4b54af0` with Claude Sonnet 5.5 Medium by default. Escalate to Claude Opus 5.5 Low only if Sonnet is unavailable/insufficient and record the reason. Review P2/§8: spacing-only, observable inputs, no hidden quality ranking, Sitka-only, individual 3D-grounded confirmation requirement, no bulk confirmation and manual designation precedence.
8. Transplant only the bounded MFM-05 candidate-system delta onto the current-integration MFM-04 descendant. Do not absorb stale branch history.
9. Rerun MFM-05 95 checks plus MFM-04 regression/digests on that reconciled descendant; Unity import/compile the chain.
10. Confirm at runtime/static boundary that the MFM-05 class has no mutation/mark/Work Plan/save side effect and that its navigation result is only an inspection target. Do not claim the later 3D confirmation flow is complete.
11. Publish exact SHAs/evidence. Only integrate these static-core slices if their independent review, current-integration reconciliation, offline gates and Unity compile are green and live ownership permits. Do not mark MFM V1 complete; MFM-06/07/08 and later rendered/workload/full-flow gates remain.
12. No Windows build.

## Next executable action

#106 reviews and reconciles MFM-04 onto current integration first; MFM-05 follows on top, preserving cross-provider review independence and the approved D-065 dependency order.