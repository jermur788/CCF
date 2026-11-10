# #376 — Focused Unity UI qualification: NEEDS CHANGES

Returned to [Manager #200](https://github.com/jermur788/CCF/issues/200). Author/operator qualification only; no independent review or integration approval.

The user requested a retry after the [first blocked preflight](https://github.com/jermur788/CCF/blob/docs/376-ui-qualification-blocked-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/UNITY-UI-PRESENTATION-V1-FOCUSED-TIER2-RESULT.md). ZX20 was now mounted and admission succeeded. **One Editor run was performed and stopped at the first material inspection layout failure**, as the binding Manager packet requires. Annual Review and the remaining behavioral matrix were not attempted. The source is not qualified.

## Exact source and safe admission

- Operator: local T3 Code / Codex gpt-6.1-sol, actual medium reasoning runtime. No delegated workers or Fable commission.
- Exact game HEAD: `c683e9eecdbf87960b591c8acabec3139a8cca57`.
- Branch: `qualify/scenario-one-ui-presentation-v1-20261110-v1`.
- Separate registered lane: `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual`. Fresh Library; no Library copied. U3/personal fork and dirty Workbench were not reused or changed.
- Unity: 6000.6.0f1, Linux Editor, PID 236452; private Xephyr display :86, native OS input through xdotool. No Player or Windows build.
- Storage: correct configured device/UUID validated by environment.sh; mounted `/dev/sdd2`, exFAT, rw; 36 GiB initially, 35 GiB after staging, 28 GiB at completion, above configured 15 GiB reserve. No data/cache deletion.
- Live coordination record and #361/#373/#374/#375/#376 consulted. Earlier personal-machine lease release is recorded in local guard history; no Editor or competing lease at admission. #373/#374 closed/handed off, #375 cancelled. User explicitly requested this retry.
- Exclusive lease `1400e94f815045fa9287381106da7bfb`, owner Codex-Sol-CCF376, acquired 2026-11-10T21:56:45.736108Z, normally released after run and cleanup. Exact release metadata: [release.json](qualification-376-evidence/release.json). No forced release or kill.
- 36 valued save/preferences files copied outside Unity's write area and byte-verified before launch. All 36 original hashes still match afterward. Actual runtime persistentDataPath, SavePath and PlayerPrefs routing were additionally verified before Play Mode: [paths.txt](qualification-376-evidence/paths.txt). F9 operated only on the disposable directory. F5 was not tested.
- 27 licensed ignored asset paths verified against retained pack hashes and copied into the new lane; no change to source asset licensing/provenance.

## First material failure: initial Crop inspection card clips draft warning

Reproduction in this run:

1. Load the genuine archived #369 Year 2 save into the proven disposable slot with native F9; [fixture provenance/hash](qualification-376-evidence/fixture-provenance.json). This is an earlier actual playthrough, not forced teaching-index/cached-fact injection.
2. Native Esc dismisses contextual help. Diagnostic positioning aims at P0709; native X applies one Fell mark. Diagnostic positioning then aims at Crop Tree P0710; native E opens inspection, and native Esc dismisses its first-use help.
3. Observe initial Page 1 of 15 at 1920×1080 and 1280×720. No V paging or scrolling is needed to reproduce.

**Actual:** the bottom of “Your draft plan” is cut off. The complete no-valid-open-job explanation and following material draft warning extend below the clipping inspection container. This violates the packet's explicit “No clipping on initial inspection page” check. The page indicator, stable tree IDs, Crop/Fell distinction, local-index denominator explanation and contextual keybar are visible, but do not cure the missing text.

At 720p, the UI uses logical 1600×900 coordinates: `inspect-column` y=80, height=712.5, bottom=792.5. The explanation starts at y=747.5 with height=66.25, ending at 813.75; the warning starts at 821.25 and ends at 865. The initial page height is 773.75, exceeding its container. [Full measured tree/bounds](qualification-376-evidence/06-inspection-default-1280x720-ui.tsv). The actual PNGs independently show clipping at both sizes. No code fix or in-place verifier rebaseline was attempted.

| Actual unedited PNG | 720p | 1080p |
|---|---|---|
| Ordinary fresh Rack-first HUD | [PNG](qualification-376-evidence/01-hud-1280x720.png) | [PNG](qualification-376-evidence/01-hud-1920x1080.png) |
| Work Plan, Objective 1 locked sections | [PNG](qualification-376-evidence/03-workplan-locked-1280x720.png) | [PNG](qualification-376-evidence/03-workplan-locked-1920x1080.png) |
| **FAIL: initial Crop/one-Fell inspection** | [PNG](qualification-376-evidence/06-inspection-default-1280x720.png) | [PNG](qualification-376-evidence/06-inspection-default-1920x1080.png) |

Six original captures, about 13 MiB; no illustration or edited preview. Additional intermediate screenshots remain in the durable evidence root. All published PNGs decoded nonblack; raw capture counts/logs and hashes are retained.

## Matrix and boundaries

| Required observation | 720p | 1080p |
|---|---|---|
| Ordinary HUD readable; Year/Cash only; valid bottom keys; Rack-first X/C hidden; minimap visible | PASS, scoped fresh state | PASS, scoped fresh state |
| Work Plan disabled Planting/Pruning/Nursery notes and fixed footer | PARTIAL | PARTIAL |
| MFM candidate native C confirm / N reject/reopen | NOT RUN | NOT RUN |
| Objective 3 Crop + one Fell initial card fit | **FAIL** | **FAIL** |
| All V/Shift+V pages, rank 1–5 native targeting and full material information | NOT RUN | NOT RUN |
| Rank marker objective/keybar occlusion | Observed clear in captured pose only | Observed clear in captured pose only |
| Work Plan edit/remove/quote/minimum/eligibility; original approval/advance guards | NOT RUN | NOT RUN |
| Annual Review work/money/forest, sources preserving expansion, unread Back/ACK, historical year | NOT RUN | NOT RUN |
| Legacy40 / frozen Reference compatibility | NOT RUN | NOT RUN |
| Grayscale review / long-state breadth | NOT RUN | NOT RUN |

Work Plan partial: the state dump confirms all three locked containers and notes disabled (`enabledInHierarchy=False`); Planting is visible in the initial scroll viewport. Pruning/Nursery need scroll for visual review; actual pointer attempts against them were not completed. Do not promote this to the complete locked-control acceptance check.

Native W movement changed player position; native Tab opened Work Plan, Esc dismissed help and returned to forest, F9 loaded the genuine disposable save, X created exactly one Fell mark, E opened the selected Crop Tree. Captures/state receipts support those bounded observations. Pointer/focus qualification, native V/Shift+V and ranks remain incomplete. Positioning/aim setup and size/capture commands are diagnostics, not claims of native traversal. No destructive year advance, harvest or approved-work execution was performed.

## Execution and measurement integrity

Unity exited normally with code 0, no reaper, one run about 861 seconds including roughly 610 seconds cold project initialization. Zero C# compiler errors observed. A UnityEditor.Search startup ArgumentOutOfRangeException and rendering stack diagnostics are retained in the full Editor log; no product root cause is inferred from them. [Selected log](qualification-376-evidence/selected-log.txt).

The disposable verifier is an adapted copy of the earlier #369 command/capture/path-check utility: renamed for #376, task-bound to this exact lane/lease. [Exact verifier bytes](qualification-376-evidence/Ccf376.cs.txt). It adds no gameplay or UI implementation. Existing historical assertions were neither changed nor run. Their old-container drift is **NOT EVALUATED**, not PASS.

The utility's [runner-result.json](qualification-376-evidence/runner-result.json) says PASS because it measures normal termination, capture/path checks and cleanup. **That is an execution receipt, not a UI qualification result. The operator's actual visual gate is FAIL / NEEDS CHANGES.** No failed screenshot was overruled by numeric checks.

## Cleanup, preservation and return

- No production commit, scene edit, simulation/save-format change, frozen-reference modification, merge, main change, Fable commission, broad suite, demo or Windows build.
- Editor changed six tracked lane files (VS Code settings and five materials) during import. Their full diff was preserved externally, then only these generated changes in the newly created lane were restored to candidate bytes after Editor exit. No original valued dirty files were restored over.
- Temporary Ccf376.cs and generated .meta removed. All tracked source in the qualification lane is now clean and exactly at c683e9ee; no verifier remains in Assets.
- Original U3 and Workbench status and binary diffs match their before receipts; 36 original valued-file hashes match. [Protection receipt](qualification-376-evidence/protection-after.json). Existing Primary edits to AGENTS.md and Docs/Operations/StorageArchitecture.md preserved.
- Dedicated lane remains registered and available as a fresh/warmed lane for a separately authorised continuation; it owns no lease. No lane or source is integrated or accepted.
- Full durable evidence: `/media/jer/Files/CCF/Verification/CCF-376-UIQualification-Retry-20261110/`, including full Editor log, original captures, commands, pack manifest, backups, side-effect patch, lease/release and preservation receipts. Public evidence [SHA-256 manifest](qualification-376-evidence/sha256.json).

**Recommended Manager action:** NEEDS CHANGES for this focused visual slice. Route one bounded source repair for the reproduced initial-card overflow, then determine the continuation scope. Remaining matrix is explicitly unqualified. No source repair is included here.

**DASHBOARD SYNC: NOT VERIFIED. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** GitHub handoff/claim updates do not prove separate dashboard ingestion/readback; Manager operational follow-up remains outstanding.
