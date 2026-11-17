# Multi — Manager #641 ruling on #666 integration admission

**17 November 2026. Verdict: APPROVED FOR SEPARATE P0 ANCESTRY AND PHYSICAL INTEGRATION PREPARATION ONLY.** [Full independent #666 review](https://github.com/jermur788/CCF/blob/review/multi-p1-666-integration-admission-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-666-EXACT-CANDIDATE-INTEGRATION-ADMISSION-RESULT-20261117.md) inspected and accepted. #666 remains closed. No merge, P0 import or P1 operational/annual authorisation.

## Accepted direct evidence

At review base, current `main` contains neither `Tools/MultiRackSitkaLab/**` P0 nor `Tools/MultiRackSitkaP1Lab/**` P1. Exact accepted feature branch P1 tree has 27 tracked blobs and no drift after #662 source Manager acceptance. Original P0 qualified commit and tree have a different ancestry from main. All four CCF target source pins match current main at reviewed base. No P0 or P1 source files were changed in #666 review.

**Blocking lineage property checked against actual `tests/p0-isolation.sh`:** L0 runs `git log <qualified-P0-commit>..HEAD -- Tools/MultiRackSitkaLab` and demands empty output; L1 demands exact `HEAD:Tools/MultiRackSitkaLab` equals qualified tree. A new main-based P0 file-copy commit may satisfy L1 but fails L0. P0 integration must preserve both source-tree and meaningful provenance/history; history-aware git integration may be feasible but is **not yet demonstrated** on the future native target. Do not weaken/skip L0.

P1 exact 27-file extraction and native qualification is a **later step after** P0 prerequisite. Source identity `Accepted` permits ordinary read-only inspection of three original synthetic P0 fixtures and identity of distinct 30m synthetic fixture, not annual/work permissions. Default .NET L4 **FAIL 38 unresolved**; two-switch verification-only PASS conditional; no annual engine. No claim of runtime qualification.

## Manager's next decision and ownership

**Decision:** stage the prerequisite instead of giving a premature physical source merge permit. Created [#670 ancestry feasibility / Ops preflight](https://github.com/jermur788/CCF/issues/670) with one GitHub-only result permission and a native read-only preflight requirement. The exact P0 history-admission strategy is **not yet approved as an implementation**. An actual native integrator must first return a reproducible lineage/target-canonical preservation proof, all conflict and physical custody constraints, and a recommendation. If a scratch Git proof requires any filesystem mutation, physical Ops reservation must be established by the actual physical custodian first. No source write, merge, cherry-pick, rebase, file-tree extraction, Unity, build or annual simulation is authorised here.

**Potential strategy to assess, not authorise:** preserve the qualified P0 historical commit as an actual ancestor of a **fresh isolated main-based** candidate via a *carefully isolated history reconciliation* rather than a new P0 file-copy commit. Validate real Git path-history behaviour, including merge simplification and L0/L1, on an approved scratch lane. If the original L0 cannot pass without rewriting the verification rule, STOP for explicit separately reviewed policy decision. Do not interpret this as permission to merge the P1 feature or touch target canonical docs.

## State

- #662 four-record trust source: **MANAGER SOURCE ACCEPTED on feature, not integrated**.
- #666 integration admission: **CONDITIONALLY APPROVED for separate preparation, not merge**.
- #670 physical P0 ancestry feasibility: **READY FOR PREFLIGHT, no physical writing reservation established**.
- P1 `main` integrated: **NO**.
- Default L4: **FAIL 38**.
- First ecological year: **HOLD**, annual engine absent.
- Dashboard/Drive/physical Ops sync: **NOT VERIFIED**.

**User action:** send #670 preflight packet to native integration-capable worker; no product decision requested.
