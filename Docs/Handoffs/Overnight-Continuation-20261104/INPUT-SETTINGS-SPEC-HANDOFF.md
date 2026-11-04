# INPUT-SETTINGS-SPEC-HANDOFF — Issue #257

- **Status:** **DRAFT** (docs only). Not reviewed, not integrated, not accepted. Independent GPT-family review still required (#257 comment).
- **Task:** CCF #257 — mouse sensitivity, rebindable controls, consent-based tester preferences.
- **Branch / worktree:** `docs/overnight-input-settings-spec-20261104` in `/media/jer/Files/CCF/Operations/CCF-Overnight-InputSpec-20261104`, from `origin/main` `8f0f2301`.
- **Deliverable:** `specs/player-input-settings-and-feedback-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}` plus this file.
- **Not touched:** `active-tasks.md`, `ROUTING-LEDGER.md`, any code/asset/scene/`.meta`, `main`, `integration/scenario-one-player-test-ready-v1`.
- **Not done:** no Unity, no Player build, no runtime, no collection of any data, no save-schema change.

## 1. What was read (VERIFIED by inspection, read-only via `git show`)

`ForestPlayer.cs`, `UI/ControlsReference.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/StandaloneSessionMenu.cs`, `UI/MenuHelpView.cs`, `Racks/RackTool.cs`, `UI/Map/StandMapRackEditor.cs`, `ScenarioOne/ScenarioOneManager.cs`, `ForestSaveController.cs`, `ForestSaveData.cs` (version only), `Assets/InputSystem_Actions.inputactions` (names/paths), `ProjectSettings/ProjectSettings.asset` (`activeInputHandler`), the verification tools referencing sensitivity/`ControlsReference`, and #257 with its two comments. Candidate `72b4efbd` was diffed for the same files; it is not an ancestor of `origin/main` (diverged 451/217 commits).

## 2. Key findings (details with file:line in `spec.md` §2)

1. Sensitivity is one serialized field (`ForestPlayer.cs:16`, used `:307`); nothing saves it. Verification tooling reads it by reflection (`CameraLookDiagnosis.cs:122`).
2. There is **no rebinding layer**: **71 control reads in 16 files on candidate `72b4efbd` (68 in 14 files on main)**, by the reproducible command in `spec.md` O-7a; no `InputAction`/`PlayerInput` use in `Assets/ForestPrototype/*.cs`; the template `.inputactions` is not used by code (scene/prefab references not inspected).
3. `ControlsReference` is help text only, with offline checks asserting its labels.
4. Esc is handled independently in **four consumer files** (`ForestPlayer`, `RackTool`, `ScenarioOneManager`, `ScenarioOneUiRoot`) with `RackTool.EscapeFrame` as the only coordination.
5. No settings/controls page exists in the F10 session menu; no keyboard/gamepad UI-focus handling was found; Tab is both Work Plan and the usual UI focus key.
6. Stand Map rack editing is pointer-only (left button, Shift, Delete/Backspace).
7. Only `PlayerPrefs` "seen/done" flags exist as device-level preferences; the game save (`forest-save.json`; `CurrentVersion` 21 on main, 24 on candidate) holds no settings.
8. Candidate `72b4efbd` adds a `P` key (rack spray marker) and a contextual `N` key (reject/reopen crop-tree candidate, `StandMapTreatmentReview.cs:518-519`); main has neither — the binding inventory depends on the base.

## 3. Verified vs assumed

| Verified (code read) | Unverified / assumed |
|---|---|
| All O-* facts in spec §2 at the cited lines on main (line shifts of +3 on candidate for ForestPlayer) | UI Toolkit keyboard/gamepad focusability; cursor state when settings open |
| `activeInputHandler: 1` | Whether any scene/prefab uses `InputSystem_Actions.inputactions` |
| #257 text and comments | Input System package version/APIs available in Unity 6000.6.0f1 |
| Save has no settings fields | Non-QWERTY behaviour; gamepad behaviour; Windows `persistentDataPath` |
| | Number/hardware of testers; sensitivity range; any "typical" preference (nothing measured) |
| | All runtime, rendered, input-device and accessibility claims: **UNVERIFIED / UNMEASURED** — Unity was not run |

## 4. Decision list for Manager / user

1. **Approve scope direction:** sensitivity first (M1), then rebinding (M2–M4)? (Q5)
2. **Per-device vs per-profile** settings (recommended per-device); confirm **no save-schema change** — any save change needs separate approval. (Q1, REQ-S5)
3. **Storage:** separate settings file vs `PlayerPrefs` vs input override JSON. (Q2)
4. **Fixed keys:** keep Esc, F1, F5/F9, F10 non-rebindable? (Q4)
5. **Gamepad:** out of V1 except UI focus? (Q3)
6. **Rebinding blast radius:** accept staged migration of 14 (main) / 16 (candidate) consumer files, subset first? (Q7)
7. **Opt-in local preference summary:** wanted at all, when, and who approves the consent wording? Nothing is implemented or claimed. (Q9)
8. **Timing:** before or after the player-test-ready release? (Q11)
9. **Stand Map keyboard editing:** separate follow-up spec? (Q8)
10. **Layout policy** for non-QWERTY users. (Q6)

## 5. Statistics caveat (carried into the spec)

Any summaries would come from a small, self-selected convenience sample, with uncontrolled hardware (DPI, acceleration). They can describe what a few testers chose; they cannot justify "optimal" or "recommended default" values. (Spec §3.7)

## 6. Next steps

Independent review (different provider); Manager answers §4; decision-log entry; only then does a Manager create M1 task packet(s). No implementation should start from this DRAFT.

## 7. Review response (Codex NEEDS CHANGES on PR #263; original author's fixes)

Status remains **DRAFT**. Docs only. Re-verified from source with `git grep`/`git show` at `72b4efbd` [C] and `8f0f2301` [M].

| # | Finding | Outcome | What changed |
|---|---|---|---|
| 1 | Inventory omits candidate N shortcut and consumer | **ACCEPTED, FIXED** (verified: `UI/StandMapTreatmentReview.cs:518-519` [C], called from `UI/TreeInspectionView.cs:103`; hints `ScenarioOneTreatmentReview.cs:34-36`; offline check `TreatmentReviewOfflineCheck.cs:281`; absent on main) | `spec.md` O-7, O-12 (new N paragraph), REQ-B2, REQ-B7, AT-22a; `tasks.md` IPS-020/025/040; this handoff finding 8. |
| 2 | Save version is 24 not 21; baselines mixed | **PARTLY ACCEPTED, FIXED.** 24 is correct for the candidate; **21 is correct for main** (`ForestSaveData.cs:7` on each), so the old text was a baseline mix rather than a plain error. Real defect: baseline unlabeled. | Header gains [M]/[C] citation convention with known line shifts; O-2/O-3/O-5/O-7/O-13/O-14/O-18 labelled; handoff v21 line and S-1 name both versions. |
| 3 | Invalid-binding fallback can collide with valid overrides | **ACCEPTED, FIXED** | New REQ-P3a (deterministic rule: validity pass, user override beats restored default, table-order tie-break, required-action floor, single write), REQ-P3 amended, AT-17a, Q13, IPS-024. |
| 4 | No authority hierarchy / ambiguity stop conditions | **ACCEPTED, FIXED** | New `spec.md` §8 (rank table: fixed keys > user setting > validated import > default > scene value treated as default > game save never) and §9 (S-1…S-8, S-1 = any save-schema need); `plan.md` and `tasks.md` header point to them. |
| 5 | "Esc in five places" wrong | **ACCEPTED, FIXED** | O-13 now "four consumer files, eight `escapeKey` reads" with [M]/[C] lines; no fifth invented; handoff finding 4 corrected. |

**Inventory re-derived (not adopted from the reviewer).** Command: `git grep -o -h -E 'wasPressedThisFrame|wasReleasedThisFrame|isPressed|\.ReadValue\(' <commit> -- 'Assets/ForestPrototype/*.cs' | wc -l` (`-n` for lines, `-l` for files). **[C] 72b4efbd: 71 occurrences / 63 lines / 16 files. [M] 8f0f2301: 68 / 60 / 14.** Reviewer: 73 / 61 / 16. File count agrees; occurrences and lines do not, and the reviewer's method is not stated, so the difference is unexplained rather than resolved; the spec keeps the stated, reproducible figures. My earlier "~75 across 15" was an estimate that missed two candidate files and mixed bases; ≈75 survives only as a count of *distinct controls* (the digit-key loop in `CompetitorAssessment` is one read site for five keys).

**Rejected:** none outright. Finding 2's premise "21 is wrong" was rejected in part (21 is right for main).

**Still unverified:** the reviewer's 73/61; whether scenes/prefabs reference `InputSystem_Actions.inputactions`; all Unity, UI-focus, Input System API, device and layout claims; line numbers for files not re-checked on [C] (cited as [M]).

---

# Appendix — V2 Orchestration Decision Appendix

**Decision unit:** #257 input-settings DRAFT spec. **Decision class: SPECIFICATION DRAFT ONLY.** This appendix records what was done. It approves no model, code, save change or spec status.

| Item | Record |
|---|---|
| Journal entry | **EVENT 006** (ledger text itself not edited; ROUTING-LEDGER.md is the DPC orchestrator's sole-writer file) |
| Pool / provider | `claudeAgent_claude_b_pro` (Claude B, Pro pool) |
| Model / effort | `claude-sonnet-5-5`, `effort: medium`, role `design` (DRAFT-spec writer) |
| Reviewer | not yet run; independent GPT-family review required before any acceptance; this worker cannot approve its own work |
| Base | `origin/main` `8f0f2301`; candidate read `72b4efbd`, via `git show` only |
| Unity | not started, no lease requested, deferred tonight on disk grounds |
| Quota / tokens / context | UNKNOWN; no telemetry exposed and none estimated |
| Retries / failures | none; one tool-status interruption, no effect on content |
| Self-assessed risk | Medium: a draft is only as good as the unrun matrix; unverified UI-focus and Input System API assumptions are flagged, not resolved |
| Routing evidence returned for the DPC orchestrator | Task completed in the same worker session; no escalation needed; no scope breaches |
