# S1-D T-05 — Previous versus Proposed Presentation Readiness
**Label: BROWSER-CAPABLE**
**Claim #149**

## Purpose
Prepare the exact local implementation handoff for S1-D T-05 without touching production code. This browser task must reconcile the approved T-05 requirements against the now-integrated MFM-07 pending-work/provenance API, the existing S1-C history/year-link surface, and current live ownership so the eventual Code lane has one bounded writer surface and no duplicated history UI.

## Difficulty and routing
- Difficulty: **3/5** — browser-only, but cross-feature API and ownership boundaries matter.
- Preferred executor: OpenAI GPT-5.6 Sol browser/GitHub lane or equivalent strong browser reviewer.
- No local worktree, C#/Unity, save execution, integration or Windows build.

## Authority and known dependencies
- Approved S1-D spec: `specs/scenario-one-repeated-interventions-v1/`, task T-05 / FR-C1..C5 / AC-5.
- S1-D T-01 and T-02 are complete; T-02 implementation is already part of the repeated-intervention lineage.
- MFM-07 is integrated/qualified at exact production umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5` and provides the accepted pending-work/provenance boundary needed by T-05.
- S1-C T-04 provenance labelling is final Manager-approved at production `633775fd8650db3cf3482e550d106a0febd7b9cb` with docs-only successor `3ea4af8b`.
- S1-C T-06 claim #147 is currently the bounded history/review presentation writer. This browser task grants no production ownership and must not release overlapping writes while #147 owns the relevant surface.

## Required browser work
1. Refresh `Docs/Project/coordination/active-tasks.md`, #91, #106, #125, #147, #149 and current published refs before substantive conclusions.
2. Inspect exact MFM-07 production APIs/records that distinguish proposal/pending/committed/executed work. Treat repository code as implementation truth; do not infer semantics from stale packets.
3. Inspect S1-C history/year navigation APIs and the final provenance rules from T-04/T-06 lineage. Reuse that surface rather than creating a second history view.
4. Map the required T-05 presentation classes to authoritative records:
   - Approved;
   - Pending new;
   - Pending carried over;
   - links to the relevant Annual Review/history year;
   - spatial review only where a recorded location already exists.
5. Decide the smallest faithful production file/API surface. Prefer `WorkPlanView.cs` plus read-only links into existing S1-C history if that satisfies the spec. If `AnnualReviewView.cs` or another file currently owned by #147 is genuinely required, record a dependency on #147 completion instead of granting overlap.
6. Reconcile T-05 with MFM-09 so this task does not absorb Multi-Scale teaching, proposal-map UX, or candidate/prescription semantics that belong later.
7. Prepare the exact local Code packet and child implementation claim only when dependencies and ownership are clear. Name exact start lineage, branch/worktree, owned files, exclusions, verification gates, success token and stop conditions.
8. Publish `Docs/Handoffs/S1-D-T05-Readiness-20261029/BROWSER-RESULT.md`; update #149 and live coordination. Close #149 only after the browser result and next executable packet/claim are published.

## Binding product rules
- No new persisted field, including no Crop Tree designation year.
- No second S1-D history UI or dated past-work map layer.
- No automatic cancellation or approval.
- No causal claim unsupported by the existing records.
- Proposal/pending state is not a Fell mark and Work Plan approval remains the commitment gate.
- Reuse S1-C history/waypoint only where recorded locations exist.
- No MFM-09 teaching, save/schema, ecology/economy, completion-rule, Reference Future or Windows-build work.

## Required result
The result must say one of:
- **READY FOR CODE** — exact packet/claim published and ownership can be taken safely;
- **READY AFTER #147** — packet prepared but production waits for S1-C history/review ownership release;
- **NEEDS MANAGER DECISION** — only if a genuine product/API ambiguity remains.

## Copy/paste dispatch line
`Run browser claim #149 from Docs/Handoffs/S1-D-T05-Readiness-20261029/BROWSER-HANDOFF.md. Refresh live coordination and #91/#106/#125/#147/#149 first. Inspect the exact MFM-07 pending-work/provenance APIs and existing S1-C history/year-link surface, map Approved / Pending new / Pending carried over to authoritative records, and determine the smallest non-overlapping T-05 production surface. Prefer WorkPlanView plus read-only links into S1-C history; if #147-owned AnnualReview/history files are genuinely required, prepare the Code packet but keep it blocked until #147 releases them. Reconcile MFM-09 ownership, publish BROWSER-RESULT.md and the next executable claim/packet if safe. Browser/GitHub only; no C#/Unity/save/schema or Windows build.`
