# Sen-1 #645 — Independent focused review of #642 shared-save schema v24 and raw-origin fixture repair

**Manager:** #640, OpenAI ChatGPT GPT-6 (browser/GitHub). **Issue:** [#645](https://github.com/jermur788/CCF/issues/645) READY / NOT RUNNING, no reviewer yet.  
**Scope classification:** independent source-only review, MEDIUM difficulty because source change is bounded but admission identity and frozen Reference are high-stakes.  
**Best suited:** Anthropic Claude Sonnet 5.5 High or Opus 5.5 High, independent from OpenAI gpt-6.1-sol High author, with live GitHub read/write.  
**Lane:** GitHub/browser-only separate branch `review/sen-1-645-642-fixture-delta-20261116-v1` based on exact fixture result `7a88e0bec8ab26801f5bf5ce22c1af7d6f50a8e8`; no local worktree unless reviewer actually registers one. Claim #645 [RUNNING] with actual identity/branch/starting HEAD **before review Markdown write**.  
**Allowed review output ONLY:** `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-645-INDEPENDENT-642-SAVE-FIXTURE-DELTA-REVIEW-20261116.md`.

## Authority and precise inputs

- Original [#642 repair handoff](https://github.com/jermur788/CCF/blob/7a88e0bec8ab26801f5bf5ce22c1af7d6f50a8e8/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-642-SHARED-SAVE-V24-RAW-ORIGIN-FIXTURE-REPAIR-RESULT-20261116.md).
- Baseline unmerged save branch `410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`. Source-only repair `7ca95a0b2822c9c6cf9ac99cbbbfbe84afe4f07e`; result-publishing head `7a88e0bec8ab26801f5bf5ce22c1af7d6f50a8e8`. Compare exact source commit to its parent, not an arbitrary moving main.
- Only two edited files: `Tools/Verification/StandGeometry/StandGeometryVerification.cs` (+78/-16) and `Tools/Verification/ScenarioReferenceVerification.cs` (+16/-2), total +94/-18. Final branch additionally adds exactly one result Markdown. Confirm with independent GitHub compare.
- Manager independently inspected GitHub source inventory and patches; verdict **APPROVED FOR INDEPENDENT REVIEW ONLY**. Worker reports 82 bounded offline PASS and player/editor real-assembly Roslyn 0 errors/warnings. Those are author evidence, not independent tests or native Unity.
- Actual #642 RUNNING registration predates source edits; source worker used its own isolated Linux worktree. Main live board grants only these two paths to #642; no production/source shared-save/main merge.

## Required focused technical review

1. **Authorization/scope:** ensure source diff is exactly two fixtures, no production `Assets/**`, `Packages/**`, `ProjectSettings/**`, no protected archive/hash changes, and all prior required checks remain present. Preserve version 24 and D-066 teaching latch exclusion in independent historical hash oracle without changing historical baseline.
2. **Actual admission contract:** read relevant current production `ForestSaveController.LoadData`, `AdmittedSave.TryCreate/TryPrepare/Apply`, `TryPrepareReference`, `ForestSaveValidation`, `ForestSaveData.CurrentVersion`, and `ScenarioReferenceArchive` at the exact candidate. Verify fixture's `LoadRaw`, capture/clone refusal, error-string assertions and world-no-mutation controls use genuine admission rather than fake trust or false negatives. Challenge whether every `Check(...)` failure safely stops before dereferencing admission outputs; distinguish logging from fail-fast behavior if necessary.
3. **Positive and negative witnesses:** inspect v24 capture roundtrip, unchanged same-session captured object, two-way Legacy40/E80 geometry/cells, invalid 2/7/-1/1000/max geometry, missing/string/foreign/pre-v20 claimed geometry, mutated capture and typed clone refusals. Ensure test checks **correct failure reasons** and captures world state before and after rather than passing when blocked on unrelated provenance.
4. **Old-save/Reference:** distinguish fabricated `V19Layout(start)` (synthetic layout regression) from genuine frozen v12 source admitted via authenticated `TryPrepareReference(0/50)`. Confirm the original archive authority, original tree/species/stage/positions and Year50->100 continuation checks are retained and not weakened or bypassed. Year-50 current *authoring candidate* passage must be carefully separated from archived immutable v1.
5. **Calibration limitations:** author probe uses Newtonsoft and in-memory adapters, not Unity JsonUtility or real native world restoration; ensure user-visible PASS claims remain limited. A fractional token parser exception in adapter does not prove native refusal. Compare the author's offline test evidence to actual source logic; there is no obligation to rerun 113 checks/4 mutants or all previous suites. Source-inspection-only review is acceptable if clearly labelled; optional independent bounded static/compile check if reviewer has tools.
6. **Integration readiness:** decide whether exact unchanged fixture candidate is safe for a dedicated integrator to apply to the unmerged save branch, while keeping native qualification separately gated. Identify any blocking flaw and smallest repair needed. No automatic integration into main and no Unity permit follows a source verdict.

## Result and disposition

Publish one Markdown report at the named output with concrete references to diff lines/actual APIs, numbered findings severity (blocker/major/minor/nit), review of each requirement, evidence provenance and **APPROVED / NEEDS CHANGES / REJECTED** verdict. Include exactly what was/was not compiled or executed; give precise integration recommendation, residual risk and next action.

**Stop:** any missing input/access, altered candidate, conflicting output writer or material production defect requires BLOCKED/NEEDS CHANGES; do not edit source/fixtures/scene/assets/save/Prefs or protected Reference. No Unity/lease, Windows build, live coordination, dashboards or Drive modifications. Branch/reviewer separate from #642 author.

**Dispatch line:** Take CCF Scenario One #645 as independent nonauthor fixture reviewer. Follow the complete GitHub packet at https://github.com/jermur788/CCF/blob/docs/sen-1-645-independent-fixture-review-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-645-INDEPENDENT-642-SAVE-FIXTURE-DELTA-REVIEW-PACKET-20261116.md. Read exact #642 source and real save-admission APIs, register actual [RUNNING] model/provider/branch/HEAD, review the two-fixture +94/-18 delta and evidence, publish the named single Markdown with verdict. No production edits, Unity or merge.

**User action:** paste the dispatch to a separate independent reviewer chat (not the source author). Dashboard/Drive sync not verified.
