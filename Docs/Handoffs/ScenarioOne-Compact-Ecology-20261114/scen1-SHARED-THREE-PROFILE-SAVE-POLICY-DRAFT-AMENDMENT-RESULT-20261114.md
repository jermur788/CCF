# scen1 — Shared three-profile save-policy DRAFT amendment — result

**Date:** 14 November 2026. **Task:** [#524](https://github.com/jermur788/CCF/issues/524). **Disposition: HANDED-OFF DRAFT AMENDMENT — NOT APPROVED POLICY, NOT PRODUCTION, NOT UNITY-VERIFIED.**

## 1. Authority, registration and scope

- **Actual worker:** Anthropic **Claude Opus 5.5 (`claude-opus-5-5`)**, Claude Code harness, acting directly with no subagents, through the GitHub API only. **Worktree NONE; Unity/Editor/machine lease NONE.** No local clone, no tests, no save or PlayerPrefs access.
- **Delegation:** Overall Manager #425's [express task-specific authorisation](https://github.com/jermur788/CCF/issues/524#issuecomment-6885995928), documentation only. It explicitly withholds approval of new literals, envelope, prefixes, ambiguous-fixture classification, final save/recovery policy, D-082 and implementation. Those are all recorded below as PROPOSED or BLOCKED.
- **Binding packet:** [scen1-BROWSER-THREE-PROFILE-SAVE-DRAFT-AMENDMENT-PACKET-20261114.md](https://github.com/jermur788/CCF/blob/docs/scen1-521-shared-save-amendment-dispatch-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-BROWSER-THREE-PROFILE-SAVE-DRAFT-AMENDMENT-PACKET-20261114.md).
- **Registration:** [RUNNING comment](https://github.com/jermur788/CCF/issues/524#issuecomment-6886155843), posted before the branch was created and before any tracked write.
- **Branch:** `docs/shared-three-profile-save-policy-20261114-v1`, verified absent, then created from the #436 DRAFT branch `docs/o2-save-isolation-four-part-draft-spec-20261112-v1` at registered base **`504969298451327aa8a80cc7a4014e5e12953742`**. The base was rechecked immediately before creation. This branch was **not** forked from main.
- **Main divergence (disclosed, not merged):** main `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; its merge base with the #436 DRAFT is `62e39ec621e38a84030c08b35a171e297056c9ed`. The DRAFT carries 6 docs commits that are not on main. Main is 27 commits ahead, and all of them change only `Docs/Project/coordination/active-tasks.md`. None was cherry-picked.
- **Exact output (four MODIFIED + one ADDED):**
  - `specs/scenario-independent-save-isolation-v1/spec.md`
  - `specs/scenario-independent-save-isolation-v1/plan.md`
  - `specs/scenario-independent-save-isolation-v1/tasks.md`
  - `specs/scenario-independent-save-isolation-v1/checklists/requirements.md`
  - this file (ADDED)
- **Not touched:** main, Decision Log, Current Milestone, live active-tasks (read only), the compact parent/child DRAFTs, the original #436 handoff, Assets/C#, Unity, saves, Reference archives, Drive/dashboard and any Windows build.

## 2. Sources read

| Source | Version read | Use |
| --- | --- | --- |
| #521 Overall #425 decision report | immutable `467244f3f679ca210e4de1f3c195bd31dfc6f156`, sections 3/5/7/8 in full | Three-profile registry, validation/lifecycle policy, preview, prefs, teaching (OPEN-C7) |
| #436 four-file DRAFT | `504969298451327aa8a80cc7a4014e5e12953742` | Base text; 36 ISO IDs |
| Decision Log, Current Milestone, active-tasks | main `fc18190b` | Canon: no D-082; historical five-rack first demo still canonical; no #524 collision on the board |
| #471 | issue + comments | PAUSED; D-082 not accepted |
| #516 parent compact DRAFT / #515 child DRAFT | their DRAFT branches | S-01–S-04 save requirements, OPEN-C4/C7, G1 four-order flow |
| #519 independent review | its branch | APPROVED DRAFT quality only; save guards PASS as requirement, NOT native replay |
| #524 issue + comments | live | Packet, #425 authorisation and conditions |

## 3. Evidence classification

### 3.1 Verified source facts — read-only on main `fc18190b` (latest Assets commit `9a6ed000`)

**#436 facts have not drifted.** Since the merge base, main changed only `active-tasks.md`.

Corrections and additions recorded in spec section 2:

1. **Correction:** pre-v20 saves map to Legacy40. A v20+ disk save with omitted or non-integer geometry is **rejected** (`ForestSaveValidation.Geometry.cs`, `ValidateGeometryJson`). #436's "v20 geometry omission maps to Legacy40" was imprecise.
2. Raw-JSON validation (Newtonsoft, duplicate-property error) runs only on the disk path (`ForestSaveController.TryReadSave`). **Direct `LoadData(ForestSaveData)` has no raw checks.**
3. `StandGeometryModel.StandSizeMeters` returns 40 m for any non-1 value. `IsKnown` accepts only 0/1.
4. No validator reads `scenarioId`. `RestoreSaveData` warns on a mismatch only **after** clearing protection, normalising and mutating (`ScenarioOneManager.cs` 1454–1483).
5. `ScenarioOne.asset` holds `scenarioId: scenario-one`, `definitionVersion: scenario-one-v13`. Every main-history commit touching the asset or the definition class carries only "scenario-one".
6. `ScenarioReferenceArchive.IdentityMatches` would accept any profile sharing the scenario-one definition (v12/v13). This is a compact-preview hazard.
7. Device-global prefs: `CCF.Learning.v1.` (`LearningObjectivesView.Curriculum.cs`) and `CCF.MenuHelp.v1.` (`MenuHelpView.cs`), each with an enumerated-key reset.
8. F10 has a duplicate legacy path literal (`StandaloneSessionMenu.cs:77`). F5 saves without confirmation. Startup New writes nothing. F5/F9 are suppressed during preview.
9. Work Plan planting seam:
   - `TryDesignateExactPlanting` creates one Pending exact-position `PlantJuvenile` order with a stable ID.
   - `ApprovePendingWork` approves all valid pending orders as a batch.
   - Resolution produces `Completed` + a PlantedJuvenile record, or `Failed`.
   - There is no Cancelled status.
10. Newer-version load warns and continues best-effort (`ForestSaveController.cs` 346–347). This is the existing historical behaviour.
11. No manifest of genuine v1–v24 player save fixtures was found in a main tree listing.

### 3.2 Proposals (PROPOSED / NEEDS APPROVAL)

- The three-profile registry with all literals from #521 section 5.
- The independent envelope for new profiles only, with root keys disjoint from ForestSaveData.
- Historical-slot foreign-marker rejection before version rules.
- The profile-specific version table.
- The shared nine-step preflight.
- Learning/help prefixes.
- Compact teaching evidence (FR-11).
- Preview gating (FR-12).
- One-router rule (FR-13).
- Registry immutability.

### 3.3 Unresolved

B-01–B-15 and Q-01–Q-03, in spec section 8.2/8.3.

## 4. Old #436 clauses and their replacements

| #436 clause | Original position | #524 replacement |
| --- | --- | --- |
| Scope | O1 + O2 only | Three profiles: historical O1, compact O1 (PROPOSED), O2 |
| §1 first demo | "O1 remains the complete first external demo with … Rack V2 five parallel racks" | Canon unchanged (historical five-rack); compact first-demo intent is the proposed D-082 exception, not canonical (#471 PAUSED) |
| SI-01 | "Only two recognised chapter IDs in v1" | Three recognised profiles, two chapter IDs kept; the two O1 profiles are distinct worlds, not a new chapter |
| SI-04 | Top-level chapter identity for new multi-scenario files | Historical files get no new field; compact/O2 use an envelope (SI-08) |
| SI-05 | v10–v24 missing ID: reject if authenticity cannot be shown | Retained as text; final rule BLOCKED (B-04); current acceptance neither tightened nor ratified |
| SI-06 | No O2 → O1 fallback | No fallback in any of six directed pairs |
| — | — | New SI-07 compact slot, SI-08 envelope, SI-09 prefixes, SI-10 immutability |
| FR-01 | Shared check on F9/F10/Continue/O2 selector/LoadData | One shared preflight also covering Start/New, Save/F5, preview entry/return and backup recovery; raw text or token required for public load |
| **FR-02** | "reject unknown/newer versions … present best-effort … not acceptable" | **Profile-specific:** strict for compact/O2; genuine historical O1 warn/best-effort preserved; foreign marker rejected first |
| FR-04 | Sentinel list | Plus active profile, all prefs namespaces and all three slots' bytes |
| FR-05 | Save containment | Plus byte-identical historical serialization; F5 confirmation is an owner question (Q-03) |
| **FR-07** | Version safety, implicitly fail-closed | Per-profile version table; historical newer-version best-effort preserved (B-05 for any later tightening) |
| Open decision 7 | "Recommended fail closed for disk loads" | Resolved in DRAFT as profile-specific |
| — | — | New FR-10 namespaces, FR-11 compact teaching evidence (four genuine G1 orders), FR-12 preview with three profiles, FR-13 one router |
| plan / tasks | Two-route phases, T-01 "two-scenario" | Three-profile routing map, phases and tasks; #425 policy vs #424 delivery made explicit; no compact-only router |
| ISO-01/02/03/04/05/06/09/12 | Two-route wording | Extended in place to three profiles (tagged *amended #524*) |
| ISO-10 | v10–v24 missing ID fails closed | Evaluated against the B-04 ruling |
| **ISO-11 / ISO-24** | Newer versions refuse / no best-effort | Strict rule kept for compact/O2; historical best-effort preserved (ISO-47). Destructive autosave is forbidden for all profiles. This is the correction #425 directed, not a weakening of the new-profile tests. |

## 5. Blocked on owner/save ratification

- **B-01:** compact/O2 filenames and IDs.
- **B-02:** envelope vs additive fields.
- **B-03:** historical-slot foreign nested ID precedence.
- **B-04:** v10–v24 raw-absent nested ID.
- **B-05:** permanence of historical best-effort.
- **B-06:** genuine fixture provenance.
- **B-07:** compact/O2 preview.
- **B-08:** teaching-evidence representation.
- **B-09:** recovery UX/filename and platform guarantees.
- **B-10:** scene/bootstrap selection and rollback.
- **B-11:** compact geometry dispatch number.
- **B-12:** D-082.
- **B-13:** per-profile payload version lineage.
- **B-14:** O2 definition identity.
- **B-15:** direct-caller inventory.

No implementer may choose any of these.

## 6. Checklist structure

The published checklist has **ISO-01–ISO-36** in their original order with their original tags, plus **ISO-37–ISO-63 (27 new)**. That is 63 unique, contiguous items, **all unchecked and NOT RUN**. A script verified this before publication. The new items cover three-slot containment and presence, envelope/raw identity, all six cross-placements, profile-specific versions, shared-preflight coverage, preview/Reference, three-slot failure/backup, namespaces, compact teaching evidence, fixture provenance, literal approval, O2 Gate 1 limits and registry immutability.

## 7. Verification at handoff

| Check | Result |
| --- | --- |
| #425 task-specific delegation present | PASS (comment 2026-11-14T23:37:13Z) |
| Base HEAD 5049692 rechecked; new branch absent before creation | PASS |
| RUNNING registration before first tracked write | PASS |
| Source facts re-checked read-only on main | PASS (document/source inspection only) |
| ISO numbering script on authored checklist | PASS (1–63, unique, contiguous, none checked) |
| Readback of five files and compare vs registered base (exactly 4 modified + 1 added) | Performed after publication; result recorded in the #524 HANDED-OFF comment with the final head SHA |
| Unity, compile, tests, old-save/hash replay, prefs behaviour | **NOT RUN** |
| Owner/#425 approval of policy | **NOT GIVEN** |
| Dashboard / Drive / context mirrors | **NOT VERIFIED** |
| Windows Unity Player build | **PROHIBITED** before the personal Scenario One playtest and explicit authorisation |

## 8. Next

1. **One independent nonauthor persistence/doc reviewer** reviews this amendment under #425 against main source and history. This author must not review their own work. Outcome: APPROVED / NEEDS CHANGES / REJECTED, for DRAFT quality only.
2. #425 obtains owner/save rulings on B-01–B-15 and decides spec approval; D-082 proceeds separately via #471.
3. Only then does #424 schedule one registered source writer (T-00 onward).

No C# or Unity claim may rest on this DRAFT.
