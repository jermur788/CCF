# CCF #857 — blank Unity/3D scenario starter: end-to-end orchestration ledger

**Parent:** [#857](https://github.com/jermur788/CCF/issues/857) · **Spec under review:** [#856](https://github.com/jermur788/CCF/issues/856) · **Review child:** [#858](https://github.com/jermur788/CCF/issues/858)
**Binding packet:** [BLANK-STARTER-END-TO-END-ORCHESTRATOR-AFTER-SPEC-DISPATCH-20261120.md](https://github.com/jermur788/CCF/blob/Dashboard-CCF-Blank-Starter-End-To-End-Orchestrator-Dispatch-20261120-v1/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-BLANK-STARTER-END-TO-END-ORCHESTRATOR-AFTER-SPEC-DISPATCH-20261120.md)

**Status categories used here are separate and are never merged:** `SPEC APPROVED` ≠ `SOURCE IMPLEMENTED` ≠ `INDEPENDENT REVIEWED` ≠ `INTEGRATED` ≠ `UNITY VERIFIED` ≠ `OWNER ACCEPTED`.

---

## 0. Orchestrator registration

| Field | Observed value |
|---|---|
| Role | Parent orchestrator for #857 (O0–O5). Coordination, routing, monitoring, read-only evidence inspection. |
| Provider / model | `opencode-go` / `deepseek-v4.1-flash`, T3 Code (OpenCode harness) |
| Host | `jer-laptop` (Linux), T3 environment `ac150346-a782-45eb-8cae-4e08fe9bab79` |
| Capabilities actually used | GitHub via authenticated `gh` (account `jermur788`), local shell, T3 `delegate_task` child workers, read-only file inspection |
| Routing constraint read first | `Docs/Project/AgentWorkflow.md` § Cost-aware task routing (main `cbcb3850c3952a8d74e54606279165b2f4be9add`, merged from #859): DeepSeek V4.1 Flash is **coordination-only** — it never writes code, tests, assets, implementation documentation or fixes and never gives substantive independent review. |
| Start commit | `cbcb3850c3952a8d74e54606279165b2f4be9add` (live `origin/main`, freshly fetched 2026-11-20 ~03:34 UTC) |
| Docs branch | `orchestration/ccf-857-blank-starter-end-to-end-20261120-v1` (created from that commit; verified non-existent beforehand) |
| Docs files owned (only) | `Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-END-TO-END-ORCHESTRATION-LEDGER-20261120.md`, `Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-END-TO-END-ORCHESTRATION-FINAL-RESULT-20261120.md`, plus `orchestration/routing-ledger.jsonl` (coordination record) |
| Docs worktree | `/media/jer/Files/CCF/Orchestration/CCF-857-BlankStarter-EndToEnd-20261120` (Files/ext4; not a Unity lane) |
| Physical claims | **None yet.** No Unity lane, no `unity-lock`, no serialized-asset custody, no source grant. Read-only observation only: `unity-lock status` → `free`, `unity_editor_pids: []`. |
| Interference with #856 author | None: the five G0 files are read only, never edited. |

Owner-reported status at packet publication ("nothing touching Unity at the moment") is treated as owner-reported, **not** as physical proof; the lease is re-verified before any Editor launch.

---

## 1. Phase O0 — specification intake, independent review, approval

| Step | State | Evidence |
|---|---|---|
| G0 author handoff received | **DONE** | #856 handoff comment 2026-11-20T03:20:48Z; final HEAD `4ea5a10db061b9fe69601118649bb77b698877cf` |
| Orchestrator fetched the five files at that exact commit | **DONE** | `gh api ...?ref=4ea5a10d…`; blob SHAs match the Overall G0 intake verification (spec `73c7c777…`, plan `59901649…`, tasks `a2960905…`, checklist `e98eec25…`, handoff `337240cd…`) |
| Independent nonauthor review dispatched | **RUNNING** | [#858](https://github.com/jermur788/CCF/issues/858); route `codex` / `gpt-6.1-sol` (AgentWorkflow review preference), independent of the GPT-6 browser author and of the future Opus 5.5 implementer; routing decision `d857-01`, attempt `d857-01-a1` |
| Review verdict | **RECEIVED — NEEDS CHANGES (document quality only)** | [Review file](https://github.com/jermur788/CCF/blob/da2c023f6b21221173ffd71e15b09c5cc2e7a437/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-858-BLANK-STARTER-SPEC-INDEPENDENT-REVIEW-20261120.md) on `review/ccf-858-blank-starter-spec-review-20261120-v1` @ `da2c023f6b21221173ffd71e15b09c5cc2e7a437`; executed model `gpt-6.1-sol`; one added Markdown, readback exact. Blocking: **F-01** (undefined mandatory production-content validation gate), **F-02** (conditional independent-review wording). Non-blocking: F-03, F-04, F-05. Source claims verified against real code; scope and save-fence boundaries assessed sound; no replacement architecture requested. |
| Spec corrections | **DONE — CORRECTED, NOT YET RE-REVIEWED** | `docs/ccf-856-blank-starter-review-corrections-20261120-v1` @ `c4d75f8e301fa1d755105531e3411588f6779b9d` (parent `4ea5a10`), authored by `claude-sonnet-5-5`, exactly four files +39/−25. Orchestrator independently verified the GitHub compare and read the `spec.md` diff in full. Status still DRAFT / NOT APPROVED. |
| Delta re-review | **DONE — NEEDS CHANGES** | [Delta review](https://github.com/jermur788/CCF/blob/c95319d741789a3c727a870acd02ec279a0feb46/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-858-BLANK-STARTER-SPEC-DELTA-REVIEW-20261120.md) @ `c95319d7`, GPT-6.1 Terra (recorded substitution). F-03/F-04/F-05 **RESOLVED**; F-01/F-02 partially resolved with three new findings: **D-01** stale pre-correction package links (blocker), **D-02** empty-slot witness does not prove data binding, **D-03** margin/reject-not-clamp/tolerance not labelled as proposals. |
| Second bounded correction | **DONE — VERIFIED BY DIRECT INSPECTION** | `23da9850a7245a8623244ff2e2e76fbf46c33f0c` on the same branch, authored by GPT-6.2 Sol (+25/−19, four files). Orchestrator verified the full compare: stale branch slug gone and the package bound to an approval-receipt commit SHA; AC-08/N-11/T2 require per-slot empty → distinctive value → cleared with recorded inputs and outputs; all three choices labelled proposed in all four files; status unchanged; 13 FR and 10 AC still mapped. No third review cycle opened — clerical, objectively checkable findings, and the implementation candidate still receives a full independent review. |
| **Accepted spec revision** | **`23da9850a7245a8623244ff2e2e76fbf46c33f0c`** | Owner conditional G5 + delegated G4 recorded 2026-11-20. Documents themselves remain DRAFT / NOT APPROVED; acceptance is an owner decision, not a document edit. |
| Owner G5 / Overall #425 G4 acceptance of the exact spec blob | **CONDITIONAL, RECORDED** | Owner answered 2026-11-20: approve the spec at `4ea5a10` as the implementation baseline, delegate G4 for exactly this scope, and proceed through the bounded correction and re-review. Acceptance of the final corrected blob is recorded below once the delta review returns. This is an **owner** decision recorded here, not a claim that the Overall Manager #425 session reviewed it. |

**Spec at intake:** `DRAFT / NOT APPROVED / NO IMPLEMENTATION AUTHORITY`. This ledger does not promote it.

---

## 2. Phase O1 — source preflight (read-only, orchestrator observation)

Observed directly at `cbcb3850` (read-only inspection; not a Unity run, not a source grant):

| File | Lines | Observed constraint for a blank starter |
|---|---|---|
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | 512 | Builds HUD, inspection, Work Plan, review, map, objectives. `Update()` returns immediately when `manager == null` (line 177), so a manager-less scene renders **nothing**. Rack tool/markers are only added when a manager exists (106–112). `SetScreen`, `ShowWorkPlan`, `ShowReview`, `ShowMap`, `ShowObjectives`, `CloseHelp`, `HandleKeys` all dereference `manager`. |
| `Assets/ForestPrototype/UI/WorkPlanView.cs` | 618 | Reads `ScenarioOneManager.GetWorkPlanSummary`, treasury, orders, approve/advance. |
| `Assets/ForestPrototype/UI/AnnualReviewView.cs` | 780 | Already has an empty-history message, but still renders objectives/history from manager state. |
| `Assets/ForestPrototype/UI/LearningObjectivesView.cs` | 282 | Scenario One rack-first teaching sequence and persisted progress. |
| `Assets/ForestPrototype/UI/StandMapView.cs` | 989 | Map screen; square model assumptions. |
| `Assets/ForestPrototype/UI/Map/StandMapSource.cs` | 378 | `HasProperty` needs ecology cells; populated mode scans `ForestTree`, `Dirt Path`, `Forest Clearing`. |
| `Assets/ForestPrototype/UI/Map/StandMapModel.cs` / `StandMapTransform.cs` | 275 / 76 | Square `SizeM` / `SidePx` assumptions. Rectangular 45 × 60 m is **not** an inspector toggle. |
| `Assets/ForestPrototype/ForestSaveController.cs` | 562 | `SavePath = Application.persistentDataPath/forest-save.json` (+ `.tmp`, `.bak`); F5/F9 in `Update`; only guard is `ReferencePreviewActive`. |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` | 157 | F10 menu; `File.Exists(SavePath)` probes at lines 123 and 125 before any user action; `Restart()` reloads `GetActiveScene().buildIndex` (an unlisted scene has index −1). |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs` | 223 | Startup Continue path calls `CanLoad`/`Load`. |
| `Assets/Scenes/ForestTest.unity` | 792 objects / 530 KB | The original five-rack world. Not a template to strip; a new scene must be authored, not carved out of this one. |
| `ProjectSettings/EditorBuildSettings.asset` | — | Only `Assets/Scenes/ForestTest.unity` is listed. Direct Editor scene-open is the preferred entry; no build-list edit. |

**Conclusion carried into the packet:** the shared seam is genuinely necessary — the UI root cannot present the common interface without a manager, and the save fence must sit in front of `File.Exists`/`CanLoad`, not only in front of writes.

---

## 3. Phase ledger (append-only)

| UTC | Phase | Actor (model) | Action | Evidence / commit | Result |
|---|---|---|---|---|---|
| 2026-11-20 ~03:34 | O0 | deepseek-v4.1-flash (coordination) | Fetched live main, five G0 files at `4ea5a10`, verified no #857 docs branch existed, created docs branch/worktree | branch `orchestration/ccf-857-…` from `cbcb3850` | READY |
| 2026-11-20 ~03:36 | O0 | deepseek-v4.1-flash | Read-only source preflight (§2) | local inspection | Informative, no claim |
| 2026-11-20 ~03:38 | O0 | deepseek-v4.1-flash → codex/gpt-6.1-sol | Dispatched independent spec review `d857-01-a1` | #858 | RUNNING |
| 2026-11-20 ~09:20 | O0 | codex/gpt-6.1-sol (reviewer) | Independent review returned **NEEDS CHANGES**, F-01/F-02 blocking | `da2c023f6b21221173ffd71e15b09c5cc2e7a437` | REVIEWED |
| 2026-11-20 ~09:35 | O0 | jermur788 (owner) | Conditional G5 acceptance + delegated G4 for this exact scope; authorised the bounded correction and re-review | session answer | GRANTED (conditional) |
| 2026-11-20 ~09:36 | O0 | deepseek-v4.1-flash → claudeAgent/claude-sonnet-5-5 | Dispatched bounded spec correction `d857-02-a1` (F-01…F-05) | `docs/ccf-856-blank-starter-review-corrections-20261120-v1` | DONE @ `c4d75f8e` |
| 2026-11-20 ~09:45 | O0 | openai/gpt-6.1-terra | Delta re-review `d857-03-a1` | `c95319d7` | NEEDS CHANGES (D-01…D-03) |
| 2026-11-20 ~10:05 | O0 | openai/gpt-6.2-sol | Second bounded correction `d857-04-a3` (two failed launches on `anthropic` and `opencode` recorded first) | `23da9850` | DONE, verified |
| 2026-11-20 ~10:20 | O1/O2 | deepseek-v4.1-flash → openai/gpt-6.2-sol | Dispatched implementation `d857-05-a1` on the registered ZX20 lane | `feat/ccf-857-blank-starter-20261120-v1` from `cbcb3850` | RUNNING |
| — | O0 | — | Delta re-review of the corrected blob | — | **PENDING** |
| — | O0 | — | Owner G5 / #425 G4 acceptance | — | **NOT GRANTED** |
| — | O1 | — | Source/asset grant, lane, board, lease | — | **NOT CLAIMED** |
| — | O2 | — | Implementation | — | **NOT STARTED** |
| — | O3 | — | Unity Tier 1/Tier 2 | — | **NOT RUN** |
| — | O4 | — | Independent source review | — | **NOT RUN** |
| — | O5 | — | Integration + final handoff | — | **NOT RUN** |

---

## 4. Stop rules in force

- No source, scene, ProjectSettings, package or save-policy change before the exact spec is independently reviewed **and** accepted by the required authorities.
- No Unity Editor launch without a registered, clean, branch-matching lane and a real machine-wide `unity-lock`; never force-release another actor's record.
- `forest-save.json`, `.tmp`, `.bak` are hashed before and after every native run and are never used as a scratch profile.
- Original five-rack scene/UI, Legacy40, Enlarged80, Rack V2/MFM, v1–v24 and frozen Reference Future v1 are protected.
- No merge, no main push, no Windows Player build.
