# Manager #200 — bounded post-#327 source closure: one author / one independent review — 2026-11-07

**Execution status:** PREPARED / NOT RUNNING. **Task class:** PACKET ONLY (two narrow verification repairs; no gameplay spec/persistence policy changes).
**Binding ruling:** https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-DISPOSITION-AND-VERIFIER-ARCHITECTURE-20261107.md
**Parent:** [#200](https://github.com/jermur788/CCF/issues/200). **Author claim:** [#328](https://github.com/jermur788/CCF/issues/328) READY, not running. **Independent review claim:** [#329](https://github.com/jermur788/CCF/issues/329) WAITING, not running.
**Purpose:** close two source-measurement holes with **one primary authoring handoff and one independent reviewer handoff** instead of repeated individual Manager loops. Neither task certifies game functionality or admits Unity.

## Recommended assignment

- **Primary author for #328:** T3 Code / OpenAI GPT-6.1 Sol **High**, difficulty **HIGH** (real C# symbol semantics, pinned Roslyn references, two-stage separate-worktree discipline). Use a genuinely available same-capability local coding worker if that model is unavailable. The *same actual owner* performs stages A then B serially, not competing parallel writers. Confirm provider/model/reasoning and live capacity at start. Do not ask Manager for ordinary bounded in-scope test repairs.
- **Independent reviewer for #329:** T3 Code / Claude Opus 5.5 High, difficulty **HIGH** (adversarial semantic/receipt integrity), if source author is OpenAI; otherwise select a truly independent capable model/provider from the actual author. One reviewer covers both exact branches, with two separate explicit verdicts. No prior reviewer/author self-review. Model availability is not inferred.
- **Browser Manager #200:** final G2/G3/G4 verdict only. Maintains live coordination. No second Manager gate between serial source stages absent a real blocker/scope/ownership breach.

## Hard boundaries and machine preflight

Before either source stage: refresh **live** `Docs/Project/coordination/active-tasks.md` and claim #328; verify actual bare Git common store `/media/jer/Files/CCF/Repository.git`, origin, full candidate HEAD/branch, allowed files, new unique lane registration where required, clean tracked/untracked/ignored status and existing owners, local free space, no conflicting writer or Git lock. Record real `[RUNNING]` receipt with actual model, branch/worktree and starting HEAD **before edits**. Do not assume source heads from this packet are still tips: if moved or dirty/owned, STOP and report rather than silently rebase.

**Global prohibited:** Unity lock/Editor/import/Library, operations storage cleanup, changes to eight protected dirty original Workbench lane files, saved player state, Reference Future v1, Legacy40 baselines, Assets production, Packages/ProjectSettings/.vscode, PR #300 whole merge, integration/main, Windows/Player build, arbitrary test weakening. No edit to source branches owned by prior authors; new isolated lanes only. All local evidence/logs in a fresh Files Verification directory with durable GitHub Markdown summaries.

## Stage A — source-only CTA Roslyn pinned binding

**Starting source:** published #325 `c83fae1af4ac1449cba0304e44b0df49b91f8919` on `verify/scenario-one-feedback-cta-anchor-20261106`. Source tested there `b040a9929b693f9ad28bc8f9f935f002b91a38b8`. Do not wholesale merge PR #300.
**NEW branch:** `fix/cta-roslyn-symbol-binding-20261107-v1`.
**NEW sole author worktree:** `/media/jer/Files/CCF/Operations/CCF-CTARoslyn-20261107`, canonical Files store, based on exact published #325 head only.
**Source scope:** `Tools/Verification/OvernightPlaytestFeedback/` current checker/test/README and a tightly bounded Roslyn entrypoint/source project if strictly required, enumerated in the handoff; no production files. No network-installed dependencies; use proven SDK 8.0.133 and 320 hash-manifest-pinned references or stop if untrustworthy.

**Required result:** a semantic guard on the **actual pinned C# source set**, not lexical guesswork, proves:
1. ScenarioOneUiRoot derives from the intended `UnityEngine.MonoBehaviour`.
2. Every reviewed `Time.frameCount` and `Time.unscaledTime` receiver in its Objective close/focus/frame logic resolves to the `UnityEngine.Time` type, not a user field, property, tuple, alias or companion type.
3. Every reviewed `DisplayStyle.Flex`/`DisplayStyle.None` receiver in the Objectives presentation resolves to the `UnityEngine.UIElements.DisplayStyle` enum/type, not a tuple/member or alternate symbol.
4. Declared/expected reviewed sites and counts are checked; silent missing sites, compile errors, wrong/missing SDK/reference/hash, ambiguous/unresolved bindings and absent semantic tooling **FAIL CLOSED**. No claim of Unity compilation; capture compiler/reference approximation boundaries.
5. Prove negative **#326 A326-N06/N07/N08** (tuple field/property and DisplayStyle), **L326-U01** (otherwise-uncollected `ZClock.cs`) and preserved S05/S06/S07 cause checks actually fail the full checker for the intended semantic reason. Keep all former **884 negative / 112 benign**, 47 N-1 +10 N-2, historical 6/4/2 failure, candidate 10/0/2 success and genuine .NET 8 SnapFar. Add benign semantic fixtures to guard against over-rejection.
6. Keep lexical checks for static coverage; **do not grow another generalized lexical C# parser**, redesign production UI, or build speculative test infrastructure. Stop after one bounded implementation if Roslyn reference identity is unreliable or hard requirements contradict; return a minimal documented decision tradeoff instead.

**Author handoff path:** `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/CTA-ROSLYN-PINNED-BINDING-RESULT.md` on the NEW source branch, with exact tested commit/source inventory, full test exits, negative validity, baseline limits, root/ref hashes and source-only qualification status. Then release this worktree/claim scope before Stage B.

## Stage B — Workbench source-only receipt R324-01

**Starting source:** published #323 `2c4aea1c94a0768c684376bc027ba15c5af5146f` on `fix/workbench-runner-integrity-20261107-v1`. Tested source `e94267695b4cfef05ba012010ddbf5db625be0ec`.
**NEW branch:** `fix/workbench-receipt-finalization-20261107-v1`.
**NEW sole author worktree:** `/media/jer/Files/CCF/Operations/CCF-WBReceipt-20261107`, canonical Files store, based on exact published #323 head only.
**Source scope:** `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py`, `test_runner.py` and new result handoff only unless Manager specifically approves another narrow verifier path. Zero change to actual Unity/historical source, packages or dirty registered lane.

**Required result:**
- A surviving early marker/receipt must explicitly say **INCOMPLETE / PROVISIONAL** until a final authoritative runner success receipt is genuinely committed. Raw Editor `..._PASS` and clean git status alone must never be interpretable as verified runner PASS.
- Under JSON write failure **and** fallback append failure (both simulated ENOSPC and real permission failure), process exit nonzero, any old surviving marker unequivocally indicates **incomplete**, no fabricated JSON or partial-success artifact. State the authoritative success conditions for process exit, JSON and marker, without claiming impossible cross-file atomicity.
- Keep R1 postrun dirty fail/diagnostics; R3 exact terminal PASS; R4 synthetic-save labels; R5 teaching negatives; the 13 original mocks N1–N3, 28 C# projections, #319 P7 old false green → current fail, historical equality and all prior source-only controls. Add only the two precise R324-01 negatives and necessary clean positives. Never weaken the gate to make package drift pass.

**Author handoff path:** `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md` on NEW branch, exact tested/source/report commits, mock/test receipts, negative controls, failure/provisional-state semantics and open real Unity barriers.

After both stages post #328 `[HANDED-OFF]` with two exact result GitHub links, close execution presence when Manager reviews. Do not declare source approval or self-integrate.

## Independent consolidated #329 review — WAIT until both author handoffs exist

**NEW reviewer docs branch:** `docs/verifier-roslyn-receipt-independent-20261107-v1` based on exact then-current origin/main.
**NEW reviewer worktree:** `/media/jer/Files/CCF/Operations/CCF-VerifierClosureReview-20261107`. Both source branches **READ ONLY**, replay in isolated reviewer scratch; no writer overlap.
**Reviewer handoff:** `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md`.

Reviewer must independently reconstruct the valid C#9 false greens and ensure actual semantic receiver symbols and class ancestry were checked (not merely output text); validate source inventory/hashes, benign cases, old accepted regressions and fail-closed degradation. For Workbench recreate R324-01's simultaneous write failures with actual permissions and verify that only provisional markers survive, plus reproduce old/new R1 P7. Produce **two separate** APPROVED / NEEDS CHANGES / REJECTED recommendations, explicit PASS/FAIL/NOT_RUN and exact Git identities, source-only limitation and no Unity claim. Any renewed issue outside bounded scope stops and returns to Manager rather than a fourth lexical repair loop. At end mark claim [HANDED-OFF], link report.

## After review / separate future gates

Manager #200 accepts/rejects source separately, then commissions one read-only package/dirty-file provenance + storage/registered-lane admission plan. No current fresh Unity admission: ZX20 was worker-reported ~22 GiB free against 27 GiB cold entry, 20 GiB stop and 15 GiB hard floor. Tree B3 still needs future approved, pose-captured Tier-1 Unity diagnostic; 15-area integrated A–O all NOT RUN. No Windows/Player build before personal Scenario One playtest and explicit user authority.

## Single copy/paste dispatch line — author #328

**User action to begin local source work (one T3 Code session, no need to split into separate chats):**

```text
CCF #328 — execute the binding Manager packet https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-NEXT-SOURCE-CLOSURE-PACKET-20261107.md as ONE primary T3 Code worker, two serial separate clean Files-store worktrees/branches: Stage A narrow Roslyn pinned Objective UI symbol-binding checker from the exact #325 published source, then Stage B Workbench R324-01 provisional/final receipt honesty from the exact #323 published source. Refresh live coordination/ownership, verify exact refs/clean/disk, mark #328 RUNNING with actual provider/model/branch/worktree/start HEAD before first edit. Preserve all old negative/benign controls, frozen Reference/Legacy40, and eight dirty original Workbench files; no Unity, packages, PR #300 merge, integration, cleanup or build. Publish both exact GitHub Markdown handoffs named in packet, mark #328 HANDED-OFF and stop for independent #329 review; never self-approve.
```

**Action after #328 handoff:** Manager releases independent #329 with its own one-paste message; no user product decision is required at this stage. Packet publication is not worker execution.
