# S1-D T-06 Independent Execution Confirmation Result

**S1-D T-06 INDEPENDENT EXECUTION CONFIRMED / UNITY QUALIFICATION WAITING — RACK UNITY PRIORITY**

Executor: T3 Code / Codex harness, OpenAI **gpt-6.1-sol**, low reasoning effort, 2026-10-29. This is a different provider/model from the Claude Sonnet 5.5 / Anthropic implementer and previous executor. All required commands were independently rerun; previous logs were used only for comparison.

Claim: [#87](https://github.com/jermur788/CCF/issues/87). Refreshed origin/main (`c5a269004a6b1efc3b6ffb134ca20070785f8e5e`), live coordination, claim, confirmation packet and Manager disposition before execution. Posted RUNNING with actual execution identity before running checks.

- Starting HEAD: `29bb6cbd6bf5dd81499a1e45bb68666f34969309`, verified exact and clean.
- Branch: `review/s1-d-management-event-id-integrity-independent-confirm-v1`.
- Worktree: `/media/jer/ZX20/CCF-S1D-T06IndependentConfirm`.
- Production validator blob: `28b786d49fc728e50a1009bc0c42fd0f7c4f33bb`, verified.
- Final handoff commit: the documentation-only commit containing this result; its full SHA is recorded in claim #87 after commit/push (avoids a self-referential SHA).
- Durable evidence: `/media/jer/Files/CCF/Verification/S1D-T06-IndependentConfirm-20261029/` (`EV` below).

## Independent U4 inspection: PASS

Compared `5702fc67e69a97ed0f19ac83e4dda6447327fa9e..29bb6cbd6bf5dd81499a1e45bb68666f34969309`. Only code delta is `Tools/Verification/U4/U4Qualification.cs`; the other delta is the prior HANDOFF.md addendum.

Only S3 (`SYN duplicate event IDs`) routes through `SyntheticRejectedFromS24`. The helper restores the original save, captures full save JSON and year, edits captured data, asserts LoadData refusal, waits two frames, then asserts identical year and full captured world/Scenario One JSON. Snapshot uses `JsonUtility.ToJson(saves.CaptureData())`. S1/S2/S4 retain their original accepted-save helper and semantics. There is no validator bypass or change to N3/completion rules. The deliberate duplicate chronology case remains covered by N3. No production defect or verification weakening found.

The helper does not assert the exact rejection reason; Unity must still establish its runtime refusal/non-mutation behavior. This is an existing disclosed limitation, not a new execution mismatch.

## Commands and independently observed results

Run from the dedicated worktree, with all output/build artifacts under EV. `-B` prevents imported Python modules leaving bytecode in the repository.

```sh
python3 -I -B Tools/Verification/ScenarioOneRepeatedInterventions/EventIdIntegrity/run_event_id_integrity_offline.py --out "$EV/t06-work" --mutations
python3 -I -B Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py --out "$EV/n3-work"
python3 -I -B Tools/Verification/ScenarioOneRepeatedInterventions/run_offline_check.py --out "$EV/t02-work" --mutations
python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools --out "$EV/typecheck-tools-work"
```

| Check | Exit | Observed result | Evidence log |
|---|---:|---|---|
| T-06 | 0 | EVENT_ID_OFFLINE_PASS checks=75 failures=0; EVENT_ID_MUTATIONS_PASS caught=7 survived=0 | 01-t06.log |
| N3 | 0 | N3_OFFLINE_PASS checks=88 NOT_UNITY_VERIFIED | 02-n3.log |
| T-02 canonical runner | 0 | CYCLE_STATE_OFFLINE_PASS checks=93 NOT_UNITY_VERIFIED; CYCLE_STATE_MUTATIONS_PASS caught=7 survived=0 | 03-t02.log |
| Stock tools type-check | 1 | TYPECHECK_NEW_ERRORS; 22 distinct NEW entries, baseline 43, all 43 GONE | 04-stock.log |

T-06 mutants caught: zero allowed (8 failing checks), negative allowed (2), duplicate allowed (9), unwired rule (19), earlier-than-existing validation (4), null list rejected (1), sorting repair (13).

T-02 mutants caught: acknowledgement ignores event order (1), ignores year (1), three-cycle cap (2), fixed-interval verdict (2), merges pending/approved (3), absent explanation (2), mutates input (2). No mutants survived or merely failed to build.

Stock NEW/GONE entries match the previous execution exactly. Four Painter2D declaration gaps and 18 duplicate declarations/member/attribute entries from unrelated offline harnesses produce the failure. None names U4Qualification.cs or SaveEventIdIntegrityVerification.cs. No baseline was changed. Stock results alone do not prove method-body binding.

## Focused method-body check: PASS within stated approximation

Independently generated projects outside the repo from the documented approach: all non-Editor Assets sources, checked-in InputSystemStub/UnityEditorStub, throwaway Painter2D/SessionState stubs, U4Qualification.cs and SaveEventIdIntegrityVerification.cs. Inspected the previous project/stubs, reused their stub definitions and project shape with every source path redirected to this exact candidate. No old compiled outputs were reused.

Exact projects and stubs are retained under EV. Commands:

```sh
cd "$EV/focused-edited"
dotnet build -nologo -v q -o "$EV/focused-edited/out"
cd "$EV/focused-control"
dotnet build -nologo -v q -o "$EV/focused-control/out"
```

- Unmodified candidate: zero errors in U4Qualification.cs and zero in SaveEventIdIntegrityVerification.cs. Build exits 1 for 15 unrelated production/API-gap diagnostics (05-focused-edited.log).
- Negative control: copied U4 outside the repository and inserted `undefinedSymbolForControl();` immediately before LoadData refusal in the new helper. Roslyn reports CS0103 at U4Qualification.control.cs(649,9), plus the same 15 unrelated diagnostics (05-focused-control.log). Build exits 1. This proves bodies are bound and the focused check can detect a defect in the new helper.
- The 15 unrelated diagnostics match the prior execution after normalizing source path/line numbers/whitespace: FindObjectsByType overloads, FindObjectsInactive, Mouse.WarpCursorPosition, Keyboard.shiftKey/numpadEnterKey, MeshGenerationContext.painter2D and GetEntityId. This uses UnityEngine.Modules 2021.3.33 and is not Unity 6000.6 compilation.

## Scope, remaining gates and handoff

No production, verification code, baseline, serialized Unity asset, schema, package or frozen reference changed. All tracked Assets/Tools SHA-256 values were recorded and rechecked unchanged; dedicated worktree remained clean until adding this result. Primary's existing AGENTS.md/active-tasks.md modifications and three untracked documents were preserved. No other lane was modified.

No Unity launched or lock acquired by this task. No Windows build, integration, integration PR or acceptance of the complete T-06 slice. RACK-V2-08 #49 retains Unity priority. Manager review follows this result. Required Unity gates remain SaveEventIdIntegrityVerification (Legacy40/Enlarged80 and all loader entry points), existing SAVE_HARDENING_VERIFY_PASS, save-cell-index hardening, U4 refusal/non-mutation, and applicable T-02 real import/compile observation. Expected runtime behavior: invalid IDs refuse before world/disk mutation; valid unique positive histories retain load compatibility.

DASHBOARD SYNC: NOT AVAILABLE. No CCF dashboard tool/CLI or publisher was available in this session; no generated dashboard state was edited. Claim #87 records the execution/result, but automatic GitHub dashboard audit advancement and published dashboard readback remain an operational follow-up for the Manager. Live coordination is Manager-owned and was not edited under this result-only packet.
