# Manager #200 → #355 — Unity 6000.6 Stand Map scene handle and integrated identity pin

**Date:** 9 November 2026.  
**Decision on #354:** **NEEDS CHANGES / valid BLOCKED safe stop**. Source staging/handoff and evidence accepted at their stated offline scope, but the combined candidate is **NOT READY FOR UNITY**.  
**Execution issue:** [#355](https://github.com/jermur788/CCF/issues/355) **READY / NOT RUNNING**.  
**Parent evidence:** [#354 staging result](https://github.com/jermur788/CCF/blob/b63309260ade1ec6f4062b932cc90e285d5656a5/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/FOCUSED-SOURCE-STAGING-RESULT-20261109.md) and [#354 claim](https://github.com/jermur788/CCF/issues/354).  
**Classification:** PACKET ONLY — bounded API compatibility/verification repair, no new feature, save policy, ecology model or player workflow.

## What Manager independently inspected

The #354 branch includes exactly two source-transfer commits over the #353 handoff successor, comprising 11 D-072 UI files, 12 D-072 verification files and 8 CTA-verifier files; GitHub comparison confirmed the full 31-path inventory. The next commit is one result Markdown file. Source tip: `04d65a56ae0ce6ae8b61e8a0cb0687e177978e64`; report tip: `b63309260ade1ec6f4062b932cc90e285d5656a5`. All 31 transferred blobs are worker-reported byte-identical to their approved origins, with a per-file manifest available locally; Manager's GitHub file list independently confirms the path set but did **not** independently re-hash all 31 file contents.

I inspected `Assets/ForestPrototype/UI/Map/StandMapSource.cs` at the source tip: `private int headlandScene = -1`, the no-property branch resetting it to `-1`, and `int scene = eco.gameObject.scene.handle` in `ReadExistingHeadland`. The #354 offline Roslyn compile against actual Unity 6000.6.0f1 reference DLLs reports **CS0619** on the obsolete implicit SceneHandle→int conversion. The same source is also extracted into the headland offline harness, whose `HeadlandSourceStubs.cs` incorrectly models `Scene.handle` as plain `int`; that is why prior source-only checks did not surface the real engine API mismatch.

Unity 6000.6 documentation specifies `SceneHandle` as the handle type and says to **prefer storing `SceneHandle` directly** rather than converting it to raw `ulong` (see [Unity SceneHandle API](https://docs.unity.com/en-us/engine/6000.7/script-reference/unityengine/scenemanagement/scenehandle) and [Unity 6000.6 GetRawData](https://docs.unity.com/en-us/engine/6000.6/script-reference/unityengine/scenemanagement/scenehandle/getrawdata)). The shipped 6000.6.0f1 release also explicitly obsoletes implicit int conversion.

## Binding Manager type/identity decision

**APPROVE the native typed identity cache; no game-design decision is needed.** Use:

- `using UnityEngine.SceneManagement;`
- `private SceneHandle? headlandScene;` — `null` means no previously cached identity.
- In the existing no-property reset, use `headlandScene = null;`.
- In `ReadExistingHeadland`, use `SceneHandle scene = eco.gameObject.scene.handle;`, preserving the existing condition `headlandScene != scene` and assignment `headlandScene = scene`.
- Keep all existing bound checks, presentation/lost-reference logic, failure-closed Legacy40/Reference-preview gating, cache refresh cadence, read counters, manager/ecology and scene-identity invalidation semantics.

**Not approved:** `(int)scene.handle`, `unchecked` or truncation, `GetRawData()` to `int`, `ulong.MaxValue` sentinel speculation, global Unity API shim, disabling the compile error, changing expected scenario state, or a different cache policy. If nullable typed comparison exposes an actual C#9 API constraint on the exact Unity 6000.6 DLL, STOP and return the precise error rather than improvising another type design.

This field is a presentation-only transient cache; there is no save/schema migration or product decision.

## Scope, owner, lane

**Plain-English purpose:** make the newly assembled Stand Map compile with the project's exact Unity version, and add honest new-candidate byte-identity coverage without weakening historical stage guards.

**Difficulty: MEDIUM** — one small production C# API correction plus source-extracted verification semantics/identity pins.

**Best-suited:** T3 Code, a capable C# / Python/.NET author; preferred GPT-6.1 Sol Medium/High or Claude Sonnet 5.5 Medium based on availability. Record actual provider/model/harness/effort, not suggested labels.

**Exact serial existing lane (not a fresh checkout):**
- `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`
- branch `integration/scenario-one-focused-playtest-20261109-v1`
- registered common store `/media/jer/Files/CCF/Repository.git`
- expected starting report HEAD `b63309260ade1ec6f4062b932cc90e285d5656a5`
- all edit rights are conditional on *fresh* live `active-tasks.md` and #354/#355 claim/branch, actual tracked/untracked/ignored clean, no competing owner, verified source HEAD and writable Files disk. No second lane or parallel writer. Mark #355 [RUNNING] with actual owner, model, branch, worktree and full start HEAD before edits. Manager #200 alone writes coordination.

**Allowed source/test files only:**
1. `Assets/ForestPrototype/UI/Map/StandMapSource.cs` — exactly the typed cache correction described above.
2. `Tools/Verification/StandMap/HeadlandOffline/HeadlandSourceStubs.cs` — model `Scene.handle` using a typed SceneHandle stub with real equality/null semantics rather than the old `int`.
3. `Tools/Verification/StandMap/HeadlandOffline/HeadlandOfflineCheck.cs` — minimally adapt scene-change fixture to typed handle and retain the current immediate invalidation expectation.
4. Only the strictly necessary `Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` and `test_stage_inventory_guard.py` changes to add **separately selected new integrated identity checks**, including one negative mutation.
5. Exactly one new Markdown report at `Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/SCENE-HANDLE-COMPATIBILITY-AND-INTEGRATED-PIN-RESULT-20261109.md`.

Any need to edit additional files, other Assets, `Packages/`, `.unity`, `.meta`, `ProjectSettings/`, save/Reference data, five-rack verifier, CTA checker, original scene geometry, Workbench or C2 correction is an explicit STOP / Manager decision. Do not touch the original eight-dirty warm Workbench, CCF-U3, licensed asset pack, user saves or installed unity-lock.

## Exact-pinning/verification policy

The historical #297 anchor and `run_headland_check.py` stage-specific #297/#296 delta assertions **remain historically true** and may continue to FAIL by design when applied to a combined tree. Do **not** edit the historical anchor, silently replace an old hash, alter stage-1 mutation expected results, skip strict assertions and call that stage green, or temporarily monkeypatch inventory to claim production PASS.

**Add a clearly distinct integrated-candidate identity mode or tiny explicitly named integrated audit**, preserving the original stage mode and historic negative controls. It must:
- confirm original `StandMapSource.cs` content at #297 anchor remains recoverable and independently pinned;
- accept *only* the approved typed scene cache delta relative to the #354 source-tip blob; pin the exact new production file bytes (or source blob identity) on the repaired candidate, and reject an arbitrary altered `StandMapSource.cs` with at least one direct negative case;
- explicitly enumerate allowed production paths as already recorded by #354: nine Workbench/C2 and eleven D-072 Assets paths, with **one new allowed change *within* `StandMapSource.cs`**, not a new path. No unexplained path, old/new save, scene or package drift;
- keep headland behaviour tests truthful, including immediate scene-change invalidation, no property, V2 eligibility, Reference Preview, Legacy40 and bounded lookup; preserve prior 92/92 behavioural cases where applicable;
- report historical stage-only inventory/ancestry failures as **EXPECTED STAGE-SCOPE MISMATCH / NOT A PASS**, not as waived/integrated PASS. Never amend the five-rack checker for this task.

If an integrated pass mode requires a broad checker redesign, STOP and provide a minimal proposed witness instead. The author should prefer small, separately named checks and stable explicit pins over generalised source-history inference.

## Narrow tests — stop once the question is answered

1. **Real DLL compile** once after the source correction: use the same C#9 Roslyn + the verified 320 Unity 6000.6.0f1 reference DLLs from #354; require 0 C# compile errors, no hidden missing assembly or reduced source set. This is offline compilation, **not Unity Editor compilation**.
2. Re-run **only CTA `check_baseline.py` on actual new candidate source** (check 08 is the key; expect 10 PASS / 0 FAIL / 2 NOT-TESTABLE) with pinned SDK8. Do not rerun the 884 negative / 112 benign campaign already valid on unchanged CTA checker bytes.
3. Run focused headland extracted C# harness and the new integrated identity negative control. Preserve original stage-specific 39 SOURCE/92 C# evidence as historical, and produce fresh reported `PASS/FAIL/NOT RUN` for directly executed new cases.
4. Byte-compare all protected C2/Workbench/pkg/scene/reference state plus original 31 transfer manifest; only the authorised source and verification delta may differ. The original headland stage-specific pin must remain unchanged when run in its original approved stage fixture. No need for a second full 14-failure five-rack run or broad A–O verification.
5. One source-read post-test status, evidence manifests, actual commands/exits. No Unity, global lease, Editor, native input/render, Library, Windows/Linux Player build, B3 replay, main/PR merge or production source rebase.

Do not keep testing after the exact failure is found, unless the failure explains a new material risk that needs Manager judgement.

## Handoff and acceptance

Publish **exactly**:

`Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/SCENE-HANDLE-COMPATIBILITY-AND-INTEGRATED-PIN-RESULT-20261109.md`

on the *same* candidate branch. Include the smallest source/test diff, old/new exact source and report identities, real 6000.6 DLL compile receipts, CTA check 08, headland scene-identity cases, pin negative, exact protected bytes and allowed-delta audit, expected stage-legacy failures, local clean receipt and an explicit **READY FOR MANAGER SOURCE/INTEGRITY REVIEW** or **BLOCKED** verdict.

Mark #355 [HANDED-OFF] and close the execution claim if source/report are published, or [BLOCKED] with exact reason. **STOP for Manager #200.** The Manager will independently inspect G3 pin integrity/G4 source admission in browser; no separate full verifier security programme without a fresh defect. **No automatic #315 Unity or #316 independent integrated review.**

## Paste-ready local dispatch

> T3 Code: Execute [CCF #355](https://github.com/jermur788/CCF/issues/355) exactly under the [Manager scene-handle compatibility/pin packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-354-TO-355-SCENE-HANDLE-COMPATIBILITY-PACKET-20261109.md). On the existing clean #354 Files source-staging branch and worktree only, replace the Stand Map int scene identity cache with nullable native SceneHandle, adapt the extracted headland stub and create a separate exact new integrated-candidate byte pin while retaining old stage checks. Run one real-Unity-DLL offline compile, CTA check 08, focused headland and negative identity tests only. Publish the specified result, close/hand off #355 and STOP for Manager. No Unity, lease, broader campaigns or main merge.

**User action required:** send this one dispatch to a local T3 Code session. No product/design decision required.
