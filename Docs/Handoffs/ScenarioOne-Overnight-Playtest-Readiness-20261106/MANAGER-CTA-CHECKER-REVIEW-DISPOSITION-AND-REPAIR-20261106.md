# Overall Manager #200 — #302 independent CTA-checker review disposition and #304 bounded repair dispatch

**Date:** 2026-11-06. **Overall Manager verdict for #299/#301 candidate:** **NEEDS CHANGES**. **Task class:** PACKET ONLY — bounded verification logic repair within accepted Objective 1 guidance; no new gameplay, persistence or UX spec. **Authoritative state:** GitHub live source, review, and coordination; this is not Unity/runtime acceptance.

**Parent source:** [#299](https://github.com/jermur788/CCF/issues/299) verification-only candidate at `f294fe07` on `verify/scenario-one-feedback-cta-anchor-20261106`; local qualification [#301](https://github.com/jermur788/CCF/issues/301); independent review [#302](https://github.com/jermur788/CCF/issues/302) [durable report](https://github.com/jermur788/CCF/blob/88cbb759dc2e54e97fcba60e94696dea06f7067a/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/BASELINE-CTA-CHECKER-INDEPENDENT-REVIEW.md). **Repair claim:** [#304](https://github.com/jermur788/CCF/issues/304). Draft [PR #300](https://github.com/jermur788/CCF/pull/300) remains unmerged.

## 1. Evidence reviewed

Manager directly inspected the published independent report, exact `objective_cta_source_check.py`, `test_objective_cta_source_check.py`, `check_baseline.py`, `README.md`, #299/#301/#302 claims and live `Docs/Project/coordination/active-tasks.md`. No Python/.NET/Unity was executed locally by Manager.

The genuinely independent reviewer was Claude Opus 5.5 on Claude A with actual **xhigh** effort, not the ChatGPT GPT-6 #299 source author or DeepSeek V4.1 Flash #301 qualifier. The reviewer independently reported: three-script py_compile exit 0; original 20/20 seeded negative mutations rejected; repaired candidate checker `10 PASS / 0 FAIL / 2 NOT-TESTABLE` exit 0; old baseline `6 PASS / 4 FAIL / 2 NOT-TESTABLE` exit 1; actual six `RackEditing.SnapFar` cases passed without mocks; 19/19 raw #301 checksums matched. These are **reviewer-executed source/offline observations**, not new Manager-run tests.

The reviewer additionally ran **21 independent broken-source mutations**, finding **14 false-green** scanner verdicts (plus two benign accepted). Those mutations were source-only and not compiled/rendered; their prospective UI impacts were assessed by code reasoning. They establish that the original 20 checks did not cover some important defects, not that the live UI currently exhibits them.

### Blocking F1

The scanner checks an exact `Stand map [M]` → `ui.ShowMap()` button and a valid guided `btn-primary` assignment, but does **not** prove that other static primary buttons are demoted or that the map is never unconditionally primary.

The following source mutations all incorrectly PASS:
- R01: add a separate unconditional `map.AddToClassList("btn-primary")`.
- R03: remove `objective-back-to-forest` from `GuidanceButtonNames`.
- R04: rename that Back-to-forest button out of the allowlist.
- R05: add a new statically primary `ui.CloseAll()` header button not in the scoped demotion list.

R03/R04/R05 would restore the original two-primary defect; R01 breaks conditional guidance outside the eligible V2 Objective 1 state. The current scanner `:101–103` check for unconditionally primary map is effectively dead because its button-constructor regex already forbids that constructor shape.

**Manager ruling:** F1 is **Major/blocking**. Do not accept it as a caveat, waive test 08, or merge the verification branch. Reviewer scratch prototype caught all four and accepted live source; that prototype is evidence of feasibility, **not shipped repair**.

### Other findings / dispositions

| Review item | Manager disposition |
|---|---|
| **F2 — override/order false greens** | **Bounded hardening required** for the cheap, material variants R08 map identity overwrite, R11 target override, R09 string-literal spoof, R14 editor-only styling, R20 missing close-Help-frame setter, R21 missing LateUpdate application. Attempt R12/R13/R15 with local limited guards if feasible; do not turn task into a complete C# parser. Document remaining lexical limitations and actual negative controls. |
| **F3 — verbatim CTA-name duplicate** | **Repair** with unique identity over ordinary/verbatim literal spellings, and add R07 negative control. |
| **F4 — SDK pin evidence** | **Correct attribution.** #301's TMPDIR `global.json` did *not* pin active SDK; observed effective SDK was 10.0.114. #302 executed a genuinely cwd-pinned 8.0.133 run with byte-identical outcome. Do not change historic raw logs/report, and require a genuinely pinned SDK 8 run in #304 evidence. |
| **F5 — README stale test 08** | **Scope extension authorised** to `Tools/Verification/OvernightPlaytestFeedback/README.md` (verification docs only), accurate conditional CTA contract and test-08 FAIL/exit semantics. |
| **F6 — weak failure diagnostics / dead assertion** | **Fix** dead assertion with F1, include source evidence/predicate on failures when available; old baseline must still yield FAIL rather than parse-crash or false PASS. |
| **F7 — line matching/import nit** | Non-blocking, optional if truly narrow; copying the amended three checker files **together** is mandatory. |

The existing correct source semantics are preserved: named `ShowMap` map button; explicit Objectives routing under active V2/unconfirmed rack plan; scoped `btn-primary` restoration/demotion; no broad root-wide sweep; display/ancestor/disabled, Help/reference/menu and same-frame guards; passive guidance without executing world changes. Do not weaken negative scenarios by moving the defect into a special-case whitelist.

## 2. Bounded #304 source repair — release conditions

**Difficulty: Medium–High** — Python source-structure checks, meaningful negative mutation quality and real .NET SDK provenance; no Unity. **Recommended model/provider:** DeepSeek orchestrator chooses an actual available code-capable Python/.NET worker with strong testing/AST-style reasoning, ideally high effort; GPT-6.1 Sol High if genuinely available, otherwise independently demonstrated capable Claude/other worker. Model availability, remaining quota and actual reasoning effort are UNKNOWN until orchestrator preflight; no nominal effort claims. The #302 reviewer should not self-author the repair and then re-review it.

**Branch** `verify/scenario-one-feedback-cta-anchor-20261106`, exact expected starting remote candidate `f294fe0769bdb910eb58576dc53751f669982528`. **Conditional isolated worktree** `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`, historically reviewer-observed clean but **not currently verified or leased**. The separate docs-only #302 reviewer worktree is released at handoff. #296/#303 owns a **different** feature worktree and is not to be touched. #200 remains sole writer of live `active-tasks.md`. Before [RUNNING], inspect actual task ownership, branch HEAD/origin, clean tracked+untracked files, mount writable/space, and conflicting writers. If unexpected divergence/dirty data, STOP without reset/stash/force push.

**Owned edits exactly:**
1. `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py`
2. `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py`
3. `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py`
4. `Tools/Verification/OvernightPlaytestFeedback/README.md`
5. New immutable result handoff: `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/BASELINE-CTA-CHECKER-F1-F3-REPAIR-HANDOFF.md`

Original numbered tests 01–07/09 and 06a/b/c, `BASE` constant, `baseline-output.txt` and every production `Assets/`, `Packages/`, `ProjectSettings/` file remain unchanged. Any additional files or change to a protected check is a STOP. No Unity Editor/Player, UI source, prefabs, saves, builds or unapproved main merge.

**Exact F1 implementation contract:** mechanically enumerate all sources of `btn-primary` on the Objectives constructor/header (initial inline `UiKit.Button` primary and later `AddToClassList`/`EnableInClassList`), reject any unconditional map primary, reject unnamed or duplicate-named static primary buttons, and require every competing static primary button's actual name to belong to `GuidanceButtonNames` demotion set. Verify the existing actual scoped loop restores and removes the class correctly. For ambiguous/unsupported source shapes fail closed instead of accepting arbitrary future primary constructions. Four new negative mutation fixtures equivalent to R01/R03/R04/R05 must **fail**, alongside existing 20.

**F2/F3 scope:** R07/R08/R09/R11/R14/R20/R21 minimum added negative fixtures with checks tied to actual source methods; do not accidentally count literal text as executable anchors; ensure no subsequent override can silently cancel the CTA within a bounded recognised control-flow shape. Additional cheap R12/R13/R15 desirable if source scanner remains manageable. Preserve benign whitespace/comment cases; report any unresolved false-green cases explicitly and do not silently reclassify them as PASS.

**Verification before re-review:**
- `python3 -m py_compile` on the three Python scripts.
- Existing 20/20 plus new F1/F2/F3 mutation controls, all broken-source mutations rejected.
- Candidate check post-fix/post-fix produces source-static 10/0/2 (or evidence-backed new true defect, never a forced green).
- Old historical baseline pre-fix/pre-fix produces 6/4/2 and exit 1, with `baseline-output.txt` unchanged.
- All six true production `RackEditing.SnapFar` cases unchanged/pass; no mock.
- To prove .NET SDK 8, run checker **with cwd inside a directory beneath SDK-8 `global.json`**, while invoking the script by absolute path. Record `dotnet --version` and SDK metadata **from that same cwd**, not a tempdir unrelated to the checker's working directory. Effective target is SDK 8.0.133, but the worker must report observed version rather than imply it. Keep scratch/pycache outside protected repo and do not edit canonical source to make SDK selection green.
- Diff/whitespace and protected-file checks, exact source worktree start/end, raw outputs, hashes, actual model/provider/effort, errors or NOT RUN.

**Required durable handoff:** `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/BASELINE-CTA-CHECKER-F1-F3-REPAIR-HANDOFF.md` on verification branch. Mark #304 [HANDED-OFF] only after pushed code/report and observed clean lane, then relinquish ownership. DeepSeek creates a **separate targeted independent re-review claim/docs-only lane** only after that handoff; reviewer inspects exact delta and mutation outcomes, issues explicit source-only verdict. Manager #200 reviews evidence and issues APPROVED / NEEDS CHANGES / REJECTED. #299 stays open pending approval.

## 3. Integration and gate separation

Do **not** merge [PR #300](https://github.com/jermur788/CCF/pull/300) wholesale: branch ancestry includes historical Objective UI production commits not on `main`. Only the ultimately reviewed/amended **three Python checker-file delta plus README update** may transfer to the **exact** approved combined Scenario One candidate after #296/#303 Stage B gates. Rerun source, mutation, old-baseline and true SDK-8 SnapFar verification there, because that tree has additional authorised diagram + Objectives call-site changes. Future Unity Tier-2 integrated CTA/input/render/720p/1080p/Help-close/save/Legacy40/Reference and user playtest remain distinct, non-waived. No Windows/Player build until user personally playtests exact candidate and explicitly approves.

## 4. Published status and user action

**Changed:** source review moved #299 to NEEDS CHANGES, #302 reviewer result accepted as accurate review evidence, #304 new bounded verification repair authorised. **Passed:** reviewer repeated 20/20 original negative controls, full 10/0/2 candidate, 6/4/2 original failure and six real SnapFar under genuine SDK 8. **Failed:** F1 and 14 independently constructed false-green scanner cases, bounded repair required. **Next:** DeepSeek assigns one actual capable #304 source writer on isolated verification worktree; subsequently independent targeted re-review and Manager approval. **User action:** paste dispatch below to orchestrator; no product choice required. **Dashboard:** update GitHub source; ingestion/published UI must be separately confirmed.

## 5. Paste-ready dispatch

> **DeepSeek orchestrator — execute #304 CTA-checker verification-only source repair, orchestration ONLY.** Read the **binding Manager disposition/packet** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-CTA-CHECKER-REVIEW-DISPOSITION-AND-REPAIR-20261106.md and live `Docs/Project/coordination/active-tasks.md`. Independent #302 Opus xhigh review found F1 blocking: test 08 allows unconditional map primary, missing/renamed Back-to-forest in scoped demotion, and a new CloseAll primary; reviewer reproduced those false greens despite 20/20 old seeded mutations and 10/0/2 candidate tally. Difficulty Medium–High; select actual available Python/.NET source-analysis worker/provider/effort, not fixed model or self-reviewer; log actual capacity. Verify exact branch `verify/scenario-one-feedback-cta-anchor-20261106` and conditional isolated worktree `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` against expected source HEAD `f294fe07`, origin, tracked+untracked clean, mount/disk and sole ownership BEFORE marking claim [RUNNING]. Touch only the three `Tools/Verification/OvernightPlaytestFeedback` Python scripts, its README, and the exact `BASELINE-CTA-CHECKER-F1-F3-REPAIR-HANDOFF.md`. Enforce all static `btn-primary` header sources demoted, reject R01/R03/R04/R05, and bounded R07/R08/R09/R11/R14/R20/R21 false greens; keep all historic tests and baseline files unchanged, improve fail diagnostics. Verify original 20 plus new mutation fixtures, py_compile, current post/post 10/0/2, old pre/pre 6/4/2 exit 1, six genuine SnapFar under **cwd-pinned effective SDK 8.0.133** (note #301 TMPDIR pin was ineffective but #302 independently reproduced exact SDK 8). Publish source/verification handoff on same branch and mark #304 [HANDED-OFF]. Then create a separate independent targeted re-review claim/docs lane and return to Manager for distinct approval. Do not touch #296/#303 feature worktree, C#/USS/Assets, personal saves, Unity, main merge or Windows/Player build. PR #300 MUST NOT be merged wholesale.
