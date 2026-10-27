# <Feature> Requirements / Acceptance Checklist

- **Spec ID:** SPEC-<feature>-V1
- **Spec status:** APPROVED / IMPLEMENTING / VERIFYING / VERIFIED
- **Checklist owner:** <named owner>
- **Implementation under test:** <branch / HEAD / evidence root>
- **Date:** <YYYY-MM-DD>

Use this checklist against the finished implementation. A checked item needs evidence, not assertion. Unchecked items with a stated reason are honest; unexplained gaps are not.

## Functional requirements

- [ ] FR-1: <restate the requirement> — evidence:
- [ ] FR-2: <restate the requirement> — evidence:

## Player / user workflow

- [ ] The intended workflow is reachable and understandable.
- [ ] Required feedback, affordances and failure/empty/edge states behave as specified.
- [ ] The implementation does not add unapproved player-facing behaviour.

## Authority and evidence

- [ ] Implementation follows the approved spec and plan.
- [ ] Evidence classifications in the spec remain accurate (empirical / transfer / inference / abstraction / calibration).
- [ ] No unlabelled pseudo-empirical coefficients or invented ecological claims were introduced.

## Save / backward compatibility

- [ ] Save impact matches the spec.
- [ ] Old saves, missing fields and Reference Future behaviour are correct.
- [ ] No unapproved migration or schema change.

If not applicable, state why:

## Security / write-back

- [ ] Trust boundaries, validation, destructive/write-back behaviour and failure handling match the spec.

If not applicable, state why:

## Acceptance criteria

- [ ] AC-1: <restate the criterion> — evidence:
- [ ] AC-2: <restate the criterion> — evidence:

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the handoff.
- [ ] Unresolved requirements are listed, not hidden.

## Verdict

- **Spec conformance:** PASS / FAIL / NOT VERIFIED
- **Implementation correctness:** PASS / FAIL / NOT VERIFIED
- **Notes:**
