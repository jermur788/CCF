# Cadence V1 source implementation — handed off for independent review and Unity qualification

Supersedes the incomplete diagnostic report at 5a2cfc87. Source implemented; **NOT Unity-verified, integrated, game-accepted or a completed D-074 objective redesign**. GPT-6.1 Sol Medium, OpenAI Codex through T3 Code, sole worker; no subagents. 2026-11-12.

## Authority, lane and routing

Binding Manager packet and corrected APPROVED spec/plan/tasks/checklist read from refreshed `origin/docs/manager-cadence-review-disposition-20261112-v1`. User scope amendment https://github.com/jermur788/CCF/issues/397#issuecomment-6816216070 permits exactly the read-only ReferenceAuthoringActive property beside ReferencePreviewActive; that is the entire ScenarioOneManager diff.

All work in `/media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397`, branch `feature/scenario-one-cadence-dual-advance-v1-20261112`, starting HEAD `5a2cfc872bb0f6d941d152a8b55b8a7387bb0a57`, prior accepted source base `a314d308315b5894263d30461a1e8b52ca0f3605`. Starting worktree clean; live main coordination refreshed/read, no competing current writer identified. Files volume 176 GiB free, ZX20 20 GiB, above 15 GiB floor. No Unity lease requested/used; no Library created/copied. Primary launch directory and #382 personal lane untouched.

Routing: **Opus 5.5 orchestrator → Sol 6.1 Medium source worker**; earlier root-volume disk-floor stop followed by user-authorised Files disk reroute; continuation follows user's exact manager-accessor scope amendment. No delegate spawned in this run. Cost/quota telemetry UNKNOWN. `active-tasks.md` is Manager-exclusive and unchanged: **Manager #200 please ingest these routing/storage/amendment events and qualification status into the authoritative ledger**. Dashboard/Drive/publisher synchronization NOT VERIFIED. Recommend independent review by a different model family before Unity release/integration.

## Actual changes

- New `ScenarioManagementCadence.cs` and meta: read-only four-year target from unique matching successful Completed FellTree records; invalid/ambiguous identity excluded; current+4 fallback, five-tick ceiling, Year25/century caps. Access felling is calendar navigation only.
- `WorkPlanView.cs`: existing one-year approval/warning route preserved; suggested target, reason, explicit confirmation and return choice.
- `ScenarioOneUiRoot.cs`: conservative genuine teaching-stage-7 eligibility, open Work Plan, no Pending/Approved including invalid/MFM, genuine latest-review acknowledgment, reference preview/authoring, help/modal/session/rack-edit/consent and management guards. Confirmation revalidates target/guards. One existing AdvanceYear call per subsequent frame, pre/post checks for outcome/objective/storm/lesson/boundary/target/guard, between-tick cancel, failure feedback and partial progress retained. No synthetic acknowledgments, work imports/approvals, prescription reuse or new saved state.
- UI-owned busy lock preserves component enabled states for save hotkeys, StandaloneSessionMenu, player, marking, rack tool, construction/storage/sawpit and asset-review controls; manager ModalOpen blocks Tab; root controls disabled while independent progress/cancel overlay remains active. Restore on normal/stop/cancel/error/disable/controller-loss exits. Existing ecology remains authoritative; Scenario One already owns deliberate annual control.
- `AnnualReviewView.cs`: one presentation-only session summary; destination opens existing latest review/browser, one normal genuine user acknowledgment. Summary clears on next one-year route.
- `Tools/Verification/CadenceDualAdvance/`: actual calculator offline runner, runtime parity A/B and parameterized negative/stop/cancel/input-restoration checks, qualification procedure.

No completion-rule, schema, HashLayout, scene/prefab, package, .vscode, frozen Reference Future, ecology formula, MFM implementation or old-save changes. No saves/Prefs loaded or written, Unity/Player/Windows build, merge or #382 lane activity.

## Executed evidence and exact limits

1. `python3 Tools/Verification/CadenceDualAdvance/run_offline.py`: **PASS, 15 assertions**. Compiles actual calculator with minimal record stubs; fallback, felling anchor, overdue target, caps, Pending/Approved, invalid work, duplicate event/order, wrong target, failed event and non-felling cases. This does not run simulation.
2. Documented runtime-source approximation `python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod --out /media/jer/Files/CCF/Verification/Cadence397/typecheck-prod`: **TYPECHECK_BLIND**, existing SceneHandle unavailable in its Unity 2021 references.
3. Same runner with documented `--replace Assets/ForestPrototype/UI/Map/StandMapSource.cs /media/jer/Files/CCF/Verification/Cadence397/StandMapSource.compat.cs`: **TYPECHECK_CLEAN**, six baseline errors, zero additional errors. External compatibility copy replaces only SceneHandle with int; repository source unchanged. Final lifecycle run output in `typecheck-final-lifecycle`. This is not Unity compilation.
4. General `--mode tools` with same compatibility replacement: **TYPECHECK_BLIND** due existing unrelated offline stub collisions and missing Roslyn references. No blanket tools PASS.
5. Focused harness body binding: same prod approximation with additional `--replace Assets/ForestPrototype/ScenarioOne/ScenarioManagementCadence.cs /media/jer/Files/CCF/Verification/Cadence397/CadenceWithHarness.cs` containing calculator plus runtime harness with Editor guard stripped: **TYPECHECK_CLEAN**, six baseline errors, zero additional. Output `typecheck-harness`.
6. `git diff --check`: PASS. No generated Assets verification script or meta left behind. Initial `python` invocation unavailable under pyenv; corrected to python3 before edits/tests.

Durable local logs and external scratch compatibility copies: `/media/jer/Files/CCF/Verification/Cadence397/`. No new evidence path on ZX20.

**Written-only / NOT RUN:** runtime parity A (full JSON and CurrentWorldHash each actual tick, N=1..5, quiet and post-felling authentic worlds); runtime parity B (actual one-year UI with genuine intermediate acks, removing only ack events and renumbering event IDs/next ID; flags remain compared before destination ack); duplicate-click and Pending/Approved negative checks; parameterized refusal/nonmutation and injected-stop/cancel/input-restore checks. Actual hash equality/semantic equality/outcome-year equivalence is **NOT OBSERVED**. No authentic runtime fixture produced or loaded in this source-only run. The README specifies required fixture matrix, including lesson 0–6, Legacy40, storm/achievement/completion, failure tick k, historical MFM, every dialog/controller guard and controller-loss lifecycle. Those fixture-specific scenarios remain to be supplied/run in qualification; they are **not all self-contained executable fixtures in this handoff**. Injected failed-tick instrumentation remains a qualification task. Do not label this full packet test coverage PASS.

## Exact next qualification

Follow `Tools/Verification/CadenceDualAdvance/README.md`: separately authorised exclusive Unity lane only, copy/run/remove CadenceParity.cs and generated meta; caller restores original in-memory world. Invoke Run twice on authentic genuine stage-7 quiet/post-felling states, N=1..5; inspect k and require N in quiet fixtures. Use Reject, WorkOrderNegatives, RunStop and Cancel with the full fixture matrix. Retain original existing tests unchanged. Do not use old personal saves or alter frozen reference fixtures.

At 1280×720 and 1920×1080, open Work Plan after genuine stage 7 and latest review acknowledgment, remove all open orders, observe target/reason and confirm. Expected: one annual update per frame, progress/cancel, F5/F9/session menu/other commands inactive, early real stops, latest normal review with exact elapsed range and reason, prior-year browsing, one genuine acknowledgment. Check destination save/reload, authentic Legacy40/frozen compatibility and five-step wall time. No rollback promised; failures preserve completed annual steps.

Manager verdict and independent other-family review remain outstanding. Branch is a source candidate, not integrated or accepted. Final publication commit identifies the exact reviewed source and handoff together; Git status must be clean after publication.
