# Sen-1 #616 — Independent review of the #614 core-first / reused-asset DRAFT delta

**Date:** 16 November 2026.
**Claim:** [#616](https://github.com/jermur788/CCF/issues/616). Reports to Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424).
**Role:** independent nonauthor reviewer of changed lines only. #614 was written by OpenAI GPT-6. I did not write #614, #604 or #600. I am not a child session of their author or of #424.

**Actual reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`).
- Runs as Claude Code in a T3 Code host, on the owner's machine. Git and `gh` ran inside the owner's WSL Ubuntu checkout.
- I cannot observe my own effort setting.

**Registration:** I posted a `[RUNNING]` comment ([link](https://github.com/jermur788/CCF/issues/616#issuecomment-6920962456)) before my first tracked edit. It names the reviewer, branch, starting HEAD, worktree and the single output path.

**Branch:** `review/sen-1-616-core-first-reuse-draft-delta-20261116-v1`.
- `git ls-remote` confirmed the branch was vacant on origin before I created it.
- **Starting HEAD:** `458fa47baef5e176b21f2bb4ac024e8d40ba0239`, the exact #614 candidate tip.
- **Compare base:** `2ffcff9e895dd8d8e6d169c17dddb286d952c81a`, the #604 tip that #606 reviewed.
- Live `origin/main` at registration was `ea08b206`. I read production source with `git show origin/main:<path>`.

**Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Sen1-616-Review`. It is a new, docs-only worktree used only for this review.

**Write scope:** only this one Markdown file.
- I did not edit specs, checklists, author handoffs, live coordination, the decision log, C#, scenes, `Assets/`, `ProjectSettings/`, saves or Prefs.
- I did not run Unity, did not hold a machine lease and did not make a Windows build.
- I did not touch the #576, #585 or #425 lanes.

## Verdict

**APPROVED — FOR #614 DRAFT DOC DELTA QUALITY ONLY.** The review found 2 nonblocking minors and 7 nits.

**Severity count: 0 blocker / 0 major / 2 minor / 7 nit.**

- The delta faithfully encodes the owner's direction: Linux-only development, reuse of existing first-party assets, core work first and art in parallel. It does not let the core lane bypass any approval, geometry, save or licence gate.
- Both #606 minors (m-A and m-B) are **correctly resolved**. The prescription change uses the existing seam and adds no invented Compact bypass. The boot preflight is scene-independent.
- Fold the minors and any nits into the spec-approval revision. They need no separate review cycle.

**This verdict does not approve either specification.** All eight files remain **DRAFT / NOT APPROVED / NOT IMPLEMENTED**. It grants no implementation, Unity, scene, save, asset-import, licence or Windows-build authority. No runtime, completion or production sign-off is implied.

## 1. Changed-file inventory and preservation (all PASS)

`git diff --name-status 2ffcff9e 458fa47b` shows **8 modified spec files and 1 added author result**. There are no source, scene, Assets, ProjectSettings, save or binary paths.

| File (P = `specs/scenario-one-compact-complete-loop-v1/`, C = `specs/scenario-one-compact-initial-planting-v1/`) | #614 change |
| --- | --- |
| P `spec.md` | New §#614 (lines 10–30): CORE/PARALLEL table, truthful placeholders, protected materials, geometry as core, #606 corrections, Linux-only, acceptance boundary. Plus one boot-preflight line at :36. |
| P `plan.md` | New C0–C5 schedule (:10–24). P3 row rewritten (m-A). |
| P `tasks.md` | New CF-00…07, ART-01+ and #606 note (:10–26). SC-02 rewritten (#606 n-5). SC-04 rewritten (m-A). |
| P `checklists/requirements.md` | Evidence overlay (:10–24). No new IDs. |
| C `spec.md` | New §#614 (:11–24). |
| C `plan.md` | New CP0–CP4 (:11–21). Nested-bold fix (#606 n-3). |
| C `tasks.md` | New CP-C0…C5 and CP-A1+ (:11–23). |
| C `checklists/requirements.md` | Overlay (:11–22). No new IDs. |

| Check | Method | Result |
| --- | --- | --- |
| Parent R lines | Extracted `^- \[.\] R-NNN` at both commits; compared SHA-256 | **115 unchecked, 0 checked, byte-identical** |
| Child CIP lines | Same method for `CIP-NNN` | **33 unchecked, 0 checked, byte-identical** |
| Checkbox churn | Counted added or removed `- [` lines in the diff | **0** |
| Deleted lines | Read every `^-` line | 4. Each is replaced in place (P3, SC-02, SC-04, C plan nested bold). No original content was lost. |
| Status labels | Read every hunk | Every #614 section is labelled DRAFT, not authority, not dispatched, or no new IDs. |

## 2. Review focus items

### 2.1 Owner-accepted direction versus unapproved designs: PASS

- **What is stated as accepted owner direction:** Linux-only Scenario One development, reuse of existing first-party assets first, and core work proceeding without waiting for final art. It matches the #424 owner entry (live `active-tasks.md`, 15:03 UTC). It is consistently separated from the exact scene, content, finance and teaching designs, which remain unapproved.
- **The core lane cannot run ahead of approvals:**
  - P `plan.md:12` starts the chain "after required approvals", with C0 covering the signed manifest, D-083 and financing/teaching/completion/save authority.
  - P `tasks.md:14` CF-00: "No source implementation while parent/child DRAFT".
  - C `plan.md:15` CP0 and C `tasks.md:15` CP-C0 say the same.
  - P `spec.md:30` lists D-083/#425, D-032, the signed manifest, OPEN-C1/C5/C7/C8, CG-TRIG/ISO-58 and DRAFT-to-APPROVED as still open.
- No changed line authorises building a DRAFT feature without D-083, the signed H1/Route A/G1/guard/Stand Map geometry, or the three-profile #544 registry.
- **Nit:** P checklist `:18` says the art items are "not blockers for beginning core code". Add "(after CF-00)" so the phrase cannot be read in isolation (n-6).

### 2.2 The two #606 corrections: PASS

**m-A — rack-dependent prescription: RESOLVED, with no invented bypass.**
- Both stale cells are gone:
  - P `plan.md:39` P3 now reads "uses existing non-Enlarged80 geometry seams … `PrescriptionApproval` refuses or omits rack-dependent flow; no Compact-specific bypass".
  - P `tasks.md:42` SC-04 now reads "refused/omitted via the existing non-Enlarged80 seam for Compact, not bypassed or globally removed".
- The new text agrees: P `spec.md:26`, P `tasks.md:26` and P checklist `:16`.
- **Checked against `origin/main` source:**
  - `RackPlanningV2Active` requires `StandGeometryModel.Enlarged80` (`ScenarioOneManager.RackPlan.cs:37-40`).
  - `RackFirstStageActive` requires `ScenarioOneRackLayout.AppliesTo`, which is `== Enlarged80` (`ScenarioOneRackLayout.cs:18`).
  - `TryReadPrescriptionWorld` returns "Prescriptions need a confirmed Rack Planning V2 plan." when `!RackPlanningV2Active` (`ScenarioOneManager.PrescriptionApproval.cs:255-258`).
  - So any new non-Enlarged80 Compact geometry value already falls through to refusal. The corrected wording describes real behaviour and needs no new branch.

**m-B — scene-independent boot preflight: RESOLVED.**
- The required clause now appears at:
  - P `spec.md:26` and `:36`;
  - P `tasks.md:16` (CF-02: "No boot `ForestSaveController.CanLoad`; preflight before gameplay scene/Prefs/live mutation");
  - C `spec.md:21`, C `plan.md:16`, C `tasks.md:17`, both checklists.
- Each place says New/Continue availability and admission query the single #544 registry without the scene-bound `ForestSaveController.CanLoad` (`ForestSaveController.cs:240`, an instance method). Each also says this happens before any gameplay scene or forest is loaded or generated.
- Each place labels this as DRAFT architecture, not a present API. This is exactly the repair #606 asked for, and it is consistent with signed W-01/H-06.

**Other #606 nits:**
- **Fixed:** n-3 (C `plan.md:27` bold) and n-5 (P `tasks.md:40` SC-02).
- **Fixed in substance:** n-1. P checklist `:20` now maps R-073 to provenance/licence/LOD/import and R-074 to measured performance.
- **Remains:** n-4. See n-1 below.

### 2.3 Asset reuse is conditional: PASS

- **Every reuse claim is conditional on inspection.** It requires real source path, licence, Unity version, stage and performance checks at the time an asset is touched:
  - P `spec.md:16`, P checklist `:20`;
  - C `spec.md:17`, C checklist `:19`, C `plan.md:15`;
  - author result §3 says plainly "NOT freshly inspected binaries".
- **I checked the leads against `origin/main` and #577:**
  - `Assets/ForestPrototype/Art/SitkaPlantation02/` is tracked.
  - Tracked young Sessile Oak and 2 m Oak/Beech sapling sets exist (#577 rows 2a/2b). #577 records their sources as local-only.
  - `Forest Ground.mat` and `Forest Path.mat` are tracked under `Assets/ForestPrototype/Materials/`.
  - Ultimate Nature (`/Assets/InnerverseInteractive/`) is gitignored (`.gitignore:80-85`). #577 row 1e records local copies as partial.
  - `ForestFloorV1` is absent from Git and local-only (#577 row 3f).
  - #577 F-6 says there is **no in-repo licence file**. The specs correctly do not treat being tracked as being licensed.
- **Protected items stay frozen:** `Forest Ground.mat`, `Forest Path.mat`, the 52 E80 planks/tracks, the existing 80 m E80 world, Rack V2/MFM, Legacy40, frozen Reference v1 and genuine v1–v24 saves (P `spec.md:22`, C `spec.md:23`, P `tasks.md:24`, C `plan.md:21`).
- **The Oak/Beech 0.6 m juvenile is handled correctly.**
  - The source is #577 F-2: `ScenarioHabitatVisuals.PlaceJuveniles` uniformly scales the 2 m sapling to `plantedJuvenileHeightM = 0.6`, about ×0.3.
  - Every changed file forbids presenting that as a qualified 0.6 m juvenile (P `spec.md:20`, P `tasks.md:18`, C `spec.md:17`, C checklist `:15`, C `plan.md:18`). Each says 2 m saplings may be used only at the 2 m stage.
  - A labelled neutral 0.6 m placeholder that keeps the real stage is allowed. Its physics must remain honest (P `spec.md:24`, C `spec.md:19`).

### 2.4 Aesthetics are off the critical path; functional geometry is not: PASS

- **Every file keeps these functional gates on the core path:**
  - the 1.2 m candidate core;
  - the O1 failed-guard **0.141 m** intrusion as a **FAIL** until a signed FR-06 amendment;
  - the O2 **0.155 m** illustrative verge and the B2 margin, still to be assessed;
  - the original **2 Oak + 2 Beech** G1;
  - the content-owner manifest STOP.
- The O1/O2/B2 wording appears in P `spec.md:24`, P `plan.md:18`, P `tasks.md:17`, P checklist `:12`, C `spec.md:19`, C `plan.md:17`, C `tasks.md:16` and C checklist `:13`.
- Primitive stand-ins must "carry the actual … guard physical footprint and collider" and must "not magically become passable" (P `spec.md:24`). P checklist `:12` adds "not suppress a collider to pass".
- **No changed line claims** that a route, G1 site, guard clearance, ecology or performance result has been physically qualified, or that one will follow automatically. "Source-screened" is used accurately: it was screened in #508 but is unsigned.
- One phrase could still be misread (n-4).

### 2.5 Gameplay depth and honesty are preserved: PASS (one minor)

- **Every file preserves gameplay depth and honesty:**
  - full Sitka/Oak/Beech parity;
  - real repeated interventions and no-work years;
  - S1-C/S1-D Annual Review and history;
  - genuine Pending orders, Work Plan approval and annual settlement;
  - quotes kept distinct from actual costs;
  - OPEN-C5 cash/financing/completion still owner-decided;
  - no fictional grant or natural donors, and no compulsory three cycles;
  - no credit for H1 pre-world omissions, initial planting, guards or a token Fell.
- **Cost figures match source:** €20 stock, up to €20 shelters, ≈€30 contractor or 40 owner minutes (C `spec.md:15`). #606 checked them against `ScenarioOne.asset`. They are labelled as quotes.
- **Batch versus per-tree controls match source:** `SetPlantingExecution` rewrites every open `PlantJuvenile` order. `SetPlantingShelters` rewrites open orders that have `exactPosition` (`ScenarioOneManager.cs:875-890`). See **m-1** for the gap with CIP-016.

### 2.6 Unchanged IDs, tiers and platform: PASS

- **ID spot-checks.** The overlay's ID ranges map to matching original text:
  - R-026…030 and R-065/066/070–072 cover the physical requirements.
  - R-073 is provenance/licence; R-074 is measured performance.
  - R-076/077 are lane and tiers; R-079 is the owner test before a Windows build.
  - R-081 is no static-only acceptance; R-082 is the dashboard.
  - CIP-014…022 cover planting/finance; CIP-027…033 cover arrival/route.
  - Two ranges are loose (n-2, n-3).
- **Tiers are proportionate:**
  - Tier-1 for focused source;
  - Tier-2 for the bounded field/planting/scene workflow;
  - **one** Tier-3 at the milestone candidate;
  - explicitly no full suite per art item (P `plan.md:24`, P `tasks.md:20`, C `tasks.md:20`).
- **Platform:**
  - Linux is the sole production host for C#, scenes, Unity import, Editor qualification and integration. Windows is limited to isolated art scratch (P `spec.md:28`, C `spec.md:23`).
  - Moving #605 Windows scratch to Linux needs explicit owner permission plus SHA-256 equality (P `spec.md:22`).
  - #611 is described as a narrow local-only allowance, not an import permit.
- **Owner's personal test versus external demo:** the owner's personal Linux Editor test happens first. A Windows build needs separate explicit approval. An external beta has its own later gate for art quality, licence and performance (P `spec.md:30`, P `plan.md:20`, C `tasks.md:23`).

## 3. Findings

### Minor (nonblocking)

**m-1 — The batch-only option conflicts with unchanged CIP-016 unless that requirement is amended.**
- **Where:** C `spec.md:15`, C checklist `:15`, C `tasks.md:18`, P `spec.md:26`.
- **Text:** "otherwise the owner must accept a transparent batch-only first-lesson restriction" / "explicitly approved batch-only constraint".
- **The conflict:** unchanged CIP-016 reads "Per-tree protection and owner/contractor method are genuine editable choices". If an owner-accepted batch-only lesson were implemented, CIP-016 could never be checked. Unless the specs say a formal amendment is needed, an implementer could treat batch-only as satisfying the checklist.
- **Proposed correction:** append to C checklist `:15`, and mirror it in C `spec.md:15`: "A batch-only first lesson requires an APPROVED amendment of CIP-016 in a later spec revision; until then CIP-016 stays unchecked and per-tree choice remains the requirement."

**m-2 — Contradictory rules for Compact-only placeholder materials.**
- **Where:** P `plan.md:17` (C1 art column): "First-party reuse and neutral Compact-only placeholders; **no new material signoff required**". C `plan.md:16` CP1 is similar.
- **The conflict:** P `spec.md:22` says "A separate Compact-only neutral material/copy may be considered **only with source/provenance and ownership approval**".
- **Risk:** a Linux scene owner could copy `Forest Ground.mat` or `Forest Path.mat` as an "unsigned placeholder". The two lines can be reconciled, since built-in primitives need no art sign-off, but the text does not say so.
- **Proposed correction:** P `plan.md:17` → "First-party reuse (after source/licence inspection) and neutral Compact-only primitives using built-in URP Lit with no copied protected material; no *art-quality* sign-off required, but any copy or derivative of an existing material needs the provenance/ownership approval in spec.md §#614". Mirror in C `plan.md:16`.

### Nits

- **n-1 (#606 n-4 residual).**
  - P `spec.md:44` (#604 text) still says both setters change "all four open exact planting orders".
  - The new P `spec.md:26` says it correctly: batch-wide `SetPlantingExecution` and exact-order `SetPlantingShelters`.
  - Align `:44` with `:26`.
- **n-2.** P checklist `:16`, "R-031…039 retain existing non-Enlarged80 rack eligibility refusal", is broader than the requirements it cites. Only R-035 and R-036 concern racks; R-031…034 and R-037…039 are about marks, Pending orders and BA. Change to "R-035/036".
- **n-3.** P checklist `:20`, "R-079 requires owner personally testing exact **Linux** Editor candidate": the word Linux comes from the owner's 16 Nov direction, not from R-079's text. Write "R-079 (read with the 16 Nov owner Linux-only direction)".
- **n-4.** In the CORE table, P `spec.md:16` says "**source-verified** H1, exact 14 pre-world omitted Sitka IDs". This could be read as already done. Prefer "signed, source-verified (pending) H1 …".
- **n-5.** Guard placeholders and juvenile visibility. #577 F-3 records that the current opaque 1.2 m shelter cylinder hides a 0.6 m juvenile. The new text requires an honest guard collider but does not require the planted juvenile to stay readable inside it. Suggest adding to C `spec.md:19`: "guard representation must keep the protected juvenile's identity/state inspectable (e.g., translucent or open-mesh primitive), without changing the collider footprint".
- **n-6.** P checklist `:18`, "not blockers for beginning core code": add "(after CF-00 authority)". See §2.1.
- **n-7 (routing, not a #614 defect).** The Compact graphics four-file DRAFT (#587/#590/#591) is outside the #614 allowlist and is not on this branch. C `plan.md:21` says the #614 schedule "governs any contradictory final-art prerequisites". That claim is effective only once the graphics owner reconciles the graphics DRAFT. The #614 result §5 already flags this. #424 should route it to the graphics owner.

## 4. Independence and limitations

- **Scope:** I reviewed the changed lines, spot-checked source and assets, and checked both #606 corrections.
- **I did not repeat:**
  - the full #601 or #606 reviews;
  - the #508/#583 geometry (189/14/8/175, O1/O2/B2);
  - the 148 native checks;
  - an inspection of any 3D binary, texture or licence.
- **Source evidence is from reading `origin/main`.** It is not runtime verification. I did not compile, run Unity, render, test collisions or round-trip a save.
- **Asset checks were limited** to repository paths and the #577 record. Whether those assets are biologically suitable, legally reusable or fast enough was **not** verified.
- **Not checked:** the dashboard and the Drive mirror.

## 5. Recommended routing for #424

1. Record **APPROVED — #614 DRAFT DOC DELTA QUALITY ONLY, 0/0/2/7**. Fold m-1, m-2 and any nits into the spec-approval revision. No separate re-review is needed for these text edits.
2. Route n-7 to the graphics DRAFT owner.
3. These remain separate open gates:
   - D-083/#425 and D-032;
   - the signed H1/Route A/G1/guard manifest and FR-06 resolution of O1;
   - OPEN-C1/C5/C7/C8 and ISO-58/CG-TRIG;
   - approval and implementation of the #544 three-profile registry;
   - parent/child DRAFT→APPROVED;
   - a registered Linux lane with a Unity lease for any CF/CP work;
   - the owner's personal Linux Editor playtest.
4. No Windows build.

**Canonical mapping:** APPROVED (#614 DRAFT DOC DELTA QUALITY ONLY), 0 blocker / 0 major / 2 minor / 7 nit. No spec status change. Dashboard/Drive: NOT VERIFIED.
