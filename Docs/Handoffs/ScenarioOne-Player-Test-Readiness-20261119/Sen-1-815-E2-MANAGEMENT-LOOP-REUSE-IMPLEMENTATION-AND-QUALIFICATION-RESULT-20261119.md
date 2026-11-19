# Sen-1 #815 — Compact30 E2 management-loop reuse: implementation and qualification result

**Date:** 19 November 2026
**Parent claim:** #815
**Overall Manager:** #731
**Orchestrator:** one native T3 Code / OpenCode parent, `opencode-go/deepseek-v4.1-flash`, session `ses_e0bd6290cffeRm2rtYeyr5oni1`, Linux host `jer-laptop`.
**Base:** `origin/main` `159f0ff8b075e275045ddd1ab0925cb447364b77`.
**Source branch:** `feature/sen-1-815-compact30-e2-loop-reuse-20261119-v1`.
**Source worktree:** `/media/jer/Videos/CCF-815-E2-Core-20261119`.
**Docs branch:** `docs/sen-1-815-e2-parent-20261119-v1`.
**Final code candidate HEAD:** `397fff9f1261550baec934cc52af38b256d4d049`. The third independent review approved `ac634757` (static only). `a16746d1` and `397fff9f` close its non-blocking notes 1, 2 and 6 with defensive changes; they are compile-checked but were not re-reviewed. Branch tip: `397fff9f`.
**Overall parent verdict: PARTIAL — A–C source implementation and advisory D exist and compile; there is no Unity Editor/native run, no Compact save, no signed world/G1 geometry and no signed Compact full completion. This is not a playable PASS.**

## 1. Registration and actual ownership

Registered on #815 in [the RUNNING comment](https://github.com/jermur788/CCF/issues/815#issuecomment-7013218767).

- One exclusive core source writer: this parent.
- One internal nonauthor reviewer: Claude Opus 5.5, different provider, read-only.
- No parallel Unity, no save writer, no scene writer.
- New source: `Assets/ForestPrototype/ScenarioOne/Compact30/**` + `.meta`.
- New verification: `Tools/Verification/Sen1CompactE2Reuse/**`.
- Narrow shared hooks only, listed in the G0 record and the core result.
- Not touched: #690 save set, #718/#813 scene/JSON/`.meta`, `ForestTest.unity`, frozen Reference Future v1, old saves, the anchor's preserved dirty files, other owners' worktrees.

### Exact source delta (`159f0ff8..ac634757`)

| Kind | Files |
|---|---|
| New Compact adapter | `Assets/ForestPrototype/ScenarioOne/Compact30/Compact30Bootstrap.cs`, `Compact30Session.cs`, `Compact30SiteRules.cs` + `.meta`/folder meta |
| New runtime fixtures | `Compact30/Resources/Compact30Roster.json`, `Compact30Sites.json`, `Compact30SitkaSpruce.asset`, `Compact30SessileOak.asset`, `Compact30Beech.asset`, `Compact30Definition.asset` + `.meta` |
| Shared narrow hooks | `StandGeometryModel.cs`, `ForestEcologyController.cs`, `ForestTree.cs`, `ForestTreeSpawner.cs`, `ScenarioOneDefinition.cs`, `ScenarioOneManager.cs`, `ForestPlayer.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs`, `UI/StandMapView.cs`, `UI/WalkingHudView.cs`, `UI/MenuHelpView.cs` |
| New verification | `Tools/Verification/Sen1CompactE2Reuse/Compact30RulesOfflineCheck.cs`, `Sen1CompactE2ReuseVerification.cs`, `run_offline.py`, `README.md` |
| Not touched | #690 save set, #718/#813 scene/JSON/`.meta`, `ForestTest.unity`, frozen Reference Future v1, old saves, other owners' worktrees |

## 2. What was implemented, by slice

### A. Enter, assess, select retained trees — IMPLEMENTED (source-level)

- The exact `ScenarioOneCompact30` scene binds at runtime; every other scene is refused.
- 175 authored `P####_…` Sitka prefab instances get stable `ForestTree` identities and colliders without reshaping the E1 visuals.
- 30×30 m / 6×6 = 36-cell Compact ecology geometry.
- H1 arrival requires the player to be inside H1 and press Y.
- The live current-year assessment must be opened with J before Y can acknowledge it.
- Three living Sitka Crop Trees are player-selected with the existing C key. They are displayed truthfully and are **not** a gate, not pre-marked, and not persisted.
- No rack/MFM prompt remains in the Compact HUD, Work Plan, map, review or help entry points.

### B. Stock, four designations, Work Plan authority — IMPLEMENTED (source-level)

- The existing nursery/economy is available in year 1; the €12,000 / €4.50 / €5.50 / €5 shelter calibration is unchanged.
- Both acknowledgements are required before any designation.
- Four fixed sites: O1/O2 sessile oak, B1/B2 European beech, at the corrected #813 E1 positions. The scene `G1_*` marker transforms independently match those positions.
- Validation: exact species/stock/position, 6×6 cell, path envelope 0.8 + shelter 0.6, H1 exclusion, 0.7 m stem clearance, 0.5 m separation, recorded-clearance drift.
- Four distinct Pending `PlantJuvenile` orders with real stock reservation, individual actor/shelter and genuine quotes. No juvenile before resolution.
- Approved work is frozen at service and UI routes; the old bulk setters refuse; approved quotes are not re-written. Pending can be corrected by audited removal + a fresh Pending order. Completed slots cannot be replaced; failed/cancelled can; max four active and two per species.
- Free G/F/U player routes and non-site exact planting are refused.

### C. Annual resolution and first reviewed teaching loop — IMPLEMENTED (source-level)

- Normal annual advance resolves each approved order once with real cash/stock/minutes/contractor/owner settlement.
- Four real `PlantedJuvenile` records are created and linked by site/species/resolution year. A neutral 0.6 m per-species representation is shown; death hides it, promotion replaces it with the normal tree.
- CT-1 is exactly four successful linked rows, two oak and two beech. Failed/cancelled/partial work earns nothing.
- Y* is the latest actual resolution/report year.
- The **existing single Annual Review acknowledgement** is the covering control. The existing event9 and the Compact `reviewAcknowledgedYear=Y*` are one logical action; no second button or event.
- `AdvanceYearBlocker` holds every annual-advance route while CT-1 is true and the covering review is unacknowledged. The Work Plan advance button and `CanAdvanceYearNow` both reflect it.
- The historical five-rack two-cycle completion is suppressed and never used as Compact credit.

### D. Repeat/adapt and completion evidence — ADVISORY IMPLEMENTED

- Real read-only `Interventions` history is displayed.
- After T1 the player can plan later work through the normal Work Plan or press N to record a deliberate no-work year after a real annual consequence.
- No forced interval, rack latch, synthetic year or fixed intervention count.
- **M-A/OD-16 full Compact completion is not implemented and not awarded.** The Annual Review Compact card says so.

## 3. Specification, source permission and physical custody

- #782 parent/child specs are **DRAFT** and remain unmodified; all 148 + 33 inherited checkboxes remain unchecked.
- The bounded A–C + advisory D subset is recorded as an **orchestrator-recorded IMPLEMENTING subset** under the owner's direct #815 instruction (`Sen-1-815-ORCHESTRATOR-SPEC-SUBSET-AND-SOURCE-GRANT-RECORD-20261119.md`). It is not a Manager #731 signature.
- no lawful machine-wide Unity lease route was available; the deployed lock tool is absent and the recorded lane configuration is stale. #815 forbids reopening the Ops investigation. **No Unity Editor was started and no other owner's process was touched.**
- Source custody: verified base, clean new worktree on ext4 `/media/jer/Videos`, no root/tmp development. The Unity-6000 assembly compile used the installed 6000.6.0f1 managed assemblies.

### Post-review repairs at `58cabe6e`

The first independent review (`83eb87a`) found 0 critical / 6 major / 8 minor. The author fixed the source-level findings: no `RackTool`/rack decision/summary in Compact; Compact-only quote freeze with a changed-quote refusal before charging; validate-before-create refusal that disables free player actions and refuses a missing authored tree; bulk setters refuse entirely; reconcile inside the advance/review gates; service-level free-placement guard; identifiable stump object; ecology-before-manager ordering; two-click approved cancellation; correct #813 branch/scene recall; runtime species readiness; static session reset; and a corrected multi-year harness assertion. The Manager #731 process gate (M-4) and the old-world/Editor/save evidence gaps (M-5) remain open. Round 2 found H-1/N-1 plus m-a..m-d; `ac634757` fixed them and a narrow third recheck returned **APPROVED (static only), 0/0/0/6**. `a16746d1`/`397fff9f` close the non-blocking notes; they are not re-reviewed.

## 4. Verification matrix

| Gate | Result | Evidence |
|---|---|---|
| Tier 1 — offline Compact rules | **PASS** | `python3 Tools/Verification/Sen1CompactE2Reuse/run_offline.py` → `checks=36 failures=0` |
| Tier 1 — negative mutation witness | **PASS (test sensitivity)** | disabling the path-clearance refusal in a scratch copy makes the same run fail exactly `site on the path refused`; unmutated source is 36/36 |
| Tier 1 — Unity 6000 assembly compile (runtime `Assets/**/*.cs`, excluding `Editor/`) | **PASS** | 0 errors, 0 warnings against Unity 6000.6.0f1 managed assemblies |
| Tier 1 — disposable Editor harness compile (Unity 6000 + `UnityEditor.dll`) | **PASS** | 0 errors, 0 warnings |
| Tier 1 — `CloudTypeCheck --mode prod` | **PASS (approximation)** | `TYPECHECK_CLEAN`; 6 baseline errors, 0 new |
| Tier 2 — Unity Editor import/scene bind/GUI/gameplay | **NOT RUN** | no lawful lease; harness provided |
| Tier 2 — save/load continuity | **NOT RUN / HELD** | #690 source not edited; Compact save refused |
| Independent nonauthor review | **APPROVED (static only) on `ac634757`** | 0 critical / 0 major / 0 minor / 6 notes; final HEAD has post-approval note closures |
| Tier 3 — full integrated qualification | **NOT RUN** | not an integrated candidate; M-A unsigned |
| Owner E1 walk acceptance | **PENDING** | #813 owner walk was still pending at packet time |
| Owner E2 personal test | **NOT RUN** | depends on the Tier 2 gate |

Tier 1 results are source-only. They do not prove a Unity import, scene binding, rendering, input, audio, frame time or player experience.

## 5. E1 correction integration and owner acceptance

- The #813 placement JSON is still **invalid** (missing comma at line 132). Its bytes were not edited, copied as a whole or used as authority.
- Only `correction_20261119.old_to_new` was read as explicitly provisional E1 test data.
- The four corrected G1 positions match the E1 scene's `G1_*` marker transforms exactly: O1 (0.65, −2.65), O2 (0.20, −1.10), B1 (−1.20, −2.45), B2 (−0.45, −4.30).
- The 175-row roster reproduces the #676 candidate digest exactly.
- None of this signs production geometry or replaces the owner's world attestation. **Owner acceptance remains pending.**

## 6. #690 save disposition

**SAVE INTEGRATION HELD.** No file in the #690 allowlist was edited. There is no collision or merge. The Compact teaching state is in memory only. `StandGeometryModel.IsKnown` deliberately remains 0/1, so a hand-made model-2 Compact save is rejected by the existing validator before mutation rather than silently admitted. A signed `ccfCompactTeaching` implementation, profile routing and refusal fixtures remain a separate #690 owner task.

## 7. Unsigned product policy

- **M-A / OD-16:** unsigned. No stem/area/BA/local-change threshold, no durable Compact completion root/field/version and no completion award were invented.
- The historical two-cycle completion is deliberately not Compact credit.
- The three Crop Tree choice is non-gating guidance; past-selection/reselection policy remains open.
- No new tariff, grant, subsidy or vehicle route was invented.

## 8. No Windows build

No Windows build was produced, attempted or requested. No Linux Player build was produced either.

## 9. Real annual management trace

**NOT RUN.** No real in-game annual trace exists. The disposable Editor gate would produce one (`COMPACT30_E2_VERIFY_PASS`) when a lawful lease is available.

## 10. Internal independent review

**Round 1 (Claude Opus 5.5, different provider, read-only, against `83eb87a`):** NEEDS CHANGES — 0 critical / 6 major / 8 minor / 6 notes. Full report: `Sen-1-815-INTERNAL-NONAUTHOR-E2-REUSE-SOURCE-AND-REGRESSION-REVIEW-20261119.md`.

**Round 2 delta (against `b452e1a9`):** NEEDS CHANGES — 0 critical / 4 major / 4 minor / 5 notes. It confirmed M-2, M-3, M-6, m-1, m-2, m-3, m-4, m-5, m-6 and m-8 as fixed or statically fixed; it found H-1 (harness assertion unreachable after completed orders remain), N-1 (static refusal lock), m-a (F1 rack/save help), m-b (non-atomic Pending replacement), m-c (ungated/armed cancellation) and m-d (checks that could pass without the fix).

**Round 3 narrow recheck (against `ac634757`): APPROVED (static review only) — 0 critical / 0 major / 0 minor / 6 non-blocking notes.** The approved verdict applies to `ac634757`. Post-approval commits `a16746d1` and `397fff9f` close notes 1, 2 and 6 (work-plan editability precheck; static refusal-notice reference; per-live-juvenile visual checks) and are compile-checked, not re-reviewed. **The final HEAD has not been re-reviewed after those note closures.** The Manager #731 process gate remains unresolved and cannot be satisfied by a reviewer verdict.

## 11. Blockers and the one next executable gate

Two blockers precede the immediate technical gate:

1. **Manager #731 process gate:** the parent could not obtain a live Manager signature. The A–C + advisory D subset and the 10 shared-file grants are self-recorded as an implementing scope under the owner's direct #815 instruction. Manager #731 must ratify or reject that scope; the independent review cannot substitute for it.
2. **Scene availability:** the #813 `ScenarioOneCompact30` scene is not on this feature branch and its placement JSON is still malformed. The native gate needs the owner-released corrected scene, or the explicit test overlay in section 11.

The remaining technical gate is:

**GATE E2-NATIVE — under a lawful exclusive machine-wide Unity lease, open `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`, copy `Tools/Verification/Sen1CompactE2Reuse/Sen1CompactE2ReuseVerification.cs` into `Assets/ForestPrototype/`, run it, capture `COMPACT30_E2_VERIFY_PASS`/`FAIL` and representative screenshots, remove the copied file and its generated `.meta`, then hand the exact candidate to the owner for the personal E2 walk.**

The scene is **not** on the feature branch (it is the #813-owned scene lineage). A lawful test overlay without editing the scene or committing its malformed JSON is:

```bash
git -C /media/jer/Files/CCF-Primary worktree add /media/jer/Videos/CCF-815-E2-Native be60c174
git -C /media/jer/Videos/CCF-815-E2-Core-20261119 diff 159f0ff8..b452e1a9 \
  | git -C /media/jer/Videos/CCF-815-E2-Native apply
# then open the Compact scene with the E2 overlay and run the disposable harness
```

This overlay is for testing only. It does not make the #813 malformed placement JSON authoritative and does not sign the world. If the owner's corrected scene is published first, use that instead.

The overlay was checked read-only: `git diff 159f0ff8..58cabe6e | git apply --check` against a detached `be60c174` worktree reports **PATCH APPLIES CLEANLY**. It was then removed; no scene or JSON byte was edited.

After that, these are separate predicates, not part of the same gate:

1. #690 signed Compact save/profile/CT-3 persistence.
2. Owner M-A/OD-16 decision and durable completion storage.
3. Owner world attestation and signed G1 coordinates.
4. Windows permission after personal test.

## 12. Dashboard / Drive

**NOT VERIFIED.** No dashboard ingestion or Drive mirror regeneration was performed or verified by this parent. GitHub is canonical; the mirrors remain outstanding.

## 13. Unrelated work preserved

The coordination anchor's two modified tracked files and three untracked operations notes, all other worktrees, and the owner primary checkout's seven historical dirty entries were not touched. No scene, save, `Library`, `Temp`, `obj` or `Logs` content was edited or cleaned.

## 14. Parent verdict

**PARTIAL.**

Actual source implementation exists for A, B, C and advisory D; it reuses the existing Scenario One management/economy/ecology/planting/review/history systems and adds only the Compact binding, strict G1 rules, teaching gates and CT-3 hold. It compiles against Unity 6000.6.0f1 and passes the offline rule/negative checks. It is **not** Unity-qualified, **not** save-integrated, **not** a signed production world, **not** completion-awarded and **not** owner-accepted.
