# Manager #200 — C-01 source acceptance and integration gate

**Date:** 2026-11-12. **Verdict:** **APPROVED — SOURCE / BOUNDED PRESENTATION**, **NOT YET APPROVED FOR INTEGRATION**. **Authoritative exact C-01 code candidate:** `a314d308315b5894263d30461a1e8b52ca0f3605` on `feature/scenario-one-research-copy-v1-20261112`. **Base:** post-Fable UI-improved `b62446caf40fc2ac939d55a6bea6e201f41de700`.

## Directly inspected

- The nine-file source difference contains six UI text/presentation files, two small verification expectation changes and one handoff. No `Assets/ForestPrototype/ScenarioOne/`, save, serializer, `HashLayout`, economic, MFM, Rack V2 or ecology changes. GitHub compare confirms a strict descendant of the updated personal-playtest candidate, ahead by six and behind by zero.
- The source scope includes exactly two *display-only* nonliteral deviations. `AnnualReviewView.cs` renders the already-existing friendly objective-name mapping in the “Objective first reached” line; no stored internal `displayName` is altered. `WorkPlanOverview.cs` adds `valueBelow: true` to the young-tree clearance row using the existing UI helper to repair documented one-character-wide text wrapping. **Both explicitly approved by Manager as minimal presentation fixes within the accepted copy goal**.
- [Claude B final independent review](https://github.com/jermur788/CCF/blob/100a43a252c4d370c4b407b1d6aa5663c710063f/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-B-C01-RESEARCH-COPY-V1-FINAL-DELTA-REVIEW.md): APPROVED at actual low effort on this exact source; own offline 157 PASS, five mutations killed, source check. Reviewer audited Sol-authored Unity images, but **did not operate Unity**.
- [Sol Unity report](https://github.com/jermur788/CCF/blob/f4bd3a02f8f826dd9f0c9787d36b63e5bb428a72/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md): two readable resolutions for changed Work Plan layout, 0 compile errors and source-specific P3 Work Plan gate PASS on final exact source, other changed views captured on prior source and carried over by one-argument diff inference. Reviewer explicitly says this is a reasonable but **unrun** full-view carry-over, not a new test result.
- [Claude A orchestration handoff](https://github.com/jermur788/CCF/blob/437b3096aac14cca3b81de26773c7a3bd0ae9ecd/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CLAUDE-A-ORCHESTRATION-IMPLEMENTATION-RESULT.md): #390/#391/#392 HANDED-OFF and closed. Six actual Sol pool-A calls with no quota events, one independent Sonnet B review, no Opus or second Sol pool. Source implemented; NOT merged or user accepted.

## AC accounting

| Area | Manager assessment | Follow-up |
|---|---|---|
| FR-C1 Share and 30% wording | **APPROVED** source and authored UI evidence | No model changes |
| FR-C2 young growth/Oak/Beech | **APPROVED** wording and display-only first-reached mapping | Oak/Beech first-reached variants NOT RUN (low risk; targeted if fixture available) |
| FR-C3 canopy/edge/open Help | **APPROVED** qualitative, nongeographic copy | No site classifier |
| FR-C4 default wind-model limitation | **APPROVED** in-game Help | External-test note published with this Manager disposition |
| AC-6 protected identity | **SOURCE PATHS PROTECTED; NOT VERIFIED** same-save `CurrentWorldHash` baseline-vs-candidate | Focused authentic saved-world comparison REQUIRED before integration |
| AC-7 all affected displays | **BOUNDED PASS** authored main panels, final-head clearance; other final-head screen checks inferred | Do not relabel carry-over as Unity re-run |
| Nursery warning old harness | **UNDETERMINED** | Focused baseline/candidate assertion; if common failure, record existing debt; if candidate-only, repair before merge |
| RackV2HelpTeaching checker | **FAIL baseline and candidate** (reported existing plus candidate whitelist) | Outside copy change; retain raw failure, do not weaken |
| AC-8 proportionate handoff | **PASS** | One final focused task, no duplicated full Tier-2/Tier-3 |

## Merge decision / safety

**DO NOT merge into `main` now.** GitHub compare shows `main` and C-01 feature have materially diverged (hundreds of unrelated commits). No blind merge, cherry-pick or rebase into `main`. **DO NOT mutate** the remote `integration/scenario-one-personal-playtest-candidate-20261111-v1` or the user's #382 pinned worktree. Candidate is source-approved but saved-world identity and nursery regression classification remain open. After focused checks close, create or select a **fresh isolated exact integration-candidate branch** based on the accepted UI lineage and replay the reviewed feature change safely; confirm candidate integrity and ask Manager for final APPROVED/NEEDS CHANGES/REJECTED. No Windows build or user acceptance implied.

**C-01G focused follow-up:** [issue #393](https://github.com/jermur788/CCF/issues/393), [exact local packet](MANAGER-C01-FOCUSED-INTEGRATION-GATES-PACKET.md), Low source scope / Medium qualification difficulty; Sol 6.1 Medium with existing registered test lane only after explicit ownership/free Editor preflight. If new production source edits are required, STOP and request bounded Manager authorisation; unchanged source and docs-only report do **not** need a new independent Claude review.

## Local lane and disk ruling

- The report says one entry `ResearchCopyQual391` was added to `/media/jer/ZX20/CCF-Ops/lanes.json`, with local backup/diff. **Manager keeps this as PROVISIONAL / DO NOT REVERT based on remote evidence alone** pending a local registered-owner check. It was not independently read from the current host; don't claim it validated. No other agent may edit this registry concurrently.
- Keep `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` temporarily for the last focused qualification **if real-space reserve and sole owner remain safe**; do not create a new cold Library. Approx 1.45 GiB buffer above the 15 GiB floor at reported handoff time is small; no cleanup, deletion, unloading or relocation of user files without separate authorisation. Recheck actual free space and process/Editor lease before every run. After qualifications, return a safe **keep vs retire proposal** with bytes/recovery and owner confirmation; no automatic delete.
- Protect `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` (#382), user saves and Prefs, licensed ignored assets, frozen Reference Future v1/Legacy40 and all unrelated dirty checkout data. Report says user lane remained untouched and local hashes unchanged; this Manager has **not personally observed the host**.

## Status and next action

- **Changed:** revised teaching copy and two scoped presentation-only details on isolated source branch; Manager source acceptance above.
- **Passed:** source scope, independent reviewer, author Unity compile/work-plan/render at final candidate, 157 focused offline checks.
- **Failed/unverified:** same-save identity, nursery warning; Rack baseline failure explicitly preserved; Oak/Beech variants and full final-view recapture unrun.
- **Means:** candidate ready for **one bounded integration gate**, not ready to call integrated, released or user accepted.
- **Next:** execute #393 and return evidence for Manager verdict; preserve feature branch and current personal playtest candidate.
- **User action:** none for this focused qualification; no Windows build.
- **Dashboard/Drive:** downstream sync **NOT VERIFIED**; Manager live coordination update separately recorded.
