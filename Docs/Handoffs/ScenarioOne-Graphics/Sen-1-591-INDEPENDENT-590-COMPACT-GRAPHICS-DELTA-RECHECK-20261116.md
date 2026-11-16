# Sen-1 #591 — Independent changed-lines recheck of the #590 Compact30 graphics DRAFT correction

**VERDICT: APPROVED (DRAFT DOCUMENT DELTA QUALITY ONLY).** 0 BLOCKER · 0 MAJOR · 1 MINOR · 3 NIT. All eight #588 findings (2 major, 6 minor) and both nits are addressed in the changed lines. The minor and the nits are non-blocking and are cheap docs-only follow-ups for Manager #424.

This is an independent, nonauthor review of **document delta quality**. It is **not** approval of the Compact graphics spec, the GQ-07 scene architecture, the GQ-02 failed-shelter envelope, any art look, any asset or licence, or any implementation. The four-file spec stays **DRAFT / NOT APPROVED**. **Unity NOT RUN.** No production code, asset, scene, save, Prefs or Windows build was touched.

| Field | Value |
|---|---|
| Task | Sen-1 #591 changed-lines-only recheck of [#590](https://github.com/jermur788/CCF/issues/590); parent Manager [#424](https://github.com/jermur788/CCF/issues/424) |
| Actual worker | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`) in Claude Code, running in T3 Code. The session does not expose an effort setting, so none is claimed. |
| Independence | Not the #590 GPT-6 author and not the #587 author. Not the #588 Opus reviewer. This session shared no context with any of them. |
| Tools actually used | Local `git` (fetch, diff, show, ls-remote, worktree), shell text tools. Read-only source inspection. No Unity, Editor, Blender, Python geometry run, save or Prefs access. |
| Reviewed delta | `bdfd4fdeafce7d5431ec2c9330658330cdb090f` (#587 reviewed head, the base for #588) → `2a76f77b8a208675668824298e6709a0d79a791d` (final #590 repaired head; tree `457197ab6ad7aca803e887272b736639fc291bd6`) |
| Review branch / worktree | `review/sen-1-591-590-graphics-delta-20261116-v1`, checked vacant on origin, forked from `2a76f77b`, in an isolated sparse docs-only checkout (`Docs/Handoffs/ScenarioOne-Graphics` only). No Unity Library, no lease. |
| Starting HEAD | `2a76f77b8a208675668824298e6709a0d79a791d` (re-fetched immediately before the branch was created; branch tip equalled the handoff's repaired head). `origin/main` was `c5af133b` at review. `git diff 375c40f0 origin/main -- Assets` is empty, so the source facts checked below hold on current `main`. |
| Registration | **Limitation:** this session has no `gh` and no GitHub issue-comment tool, so the #591 `[RUNNING]` comment could **not** be posted by me. The durable registration is the review branch itself, pushed to origin at `2a76f77b` with no tracked edit, **before** this file was written. Manager #424 should record the #591 claim state from this file. |
| Write allowlist | This one Markdown file only |

## 1. Scope

I reviewed only the changed lines in the five modified Markdown files, against the eight #588 findings and two nits. I did not re-review the whole spec, run the 66 native tests, re-open #508, look at assets or licences, or inspect Unity. I read the #588 report and the complete #590 author result first.

## 2. Boundary and preservation checks

| Check | Result | Evidence |
|---|---|---|
| Exactly the claimed paths changed | **PASS** | `git diff --name-status bdfd4fde 2a76f77b`: 5 `M` (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, the #588 packet) and 1 `A` (the #590 result). No Assets, Packages, ProjectSettings, scene, save or script path. |
| Parent/child compact specs, legacy and signed save contract unchanged | **PASS** | Not in the diff. The save-isolation contract is still only linked at `200add4e`. |
| #587 and #588 branches immutable | **PASS** | `ls-remote`: #587 tip is still `bdfd4fde`; the #588 review tip is still `b220a062`. The #590 branch is 6 commits ahead of `bdfd4fde` with merge-base `bdfd4fde`. |
| **20 FR** | **PASS** | 20 distinct `FR-NN` ids before and after. |
| **12 AC** | **PASS** | 12 distinct `AC-NN` ids before and after. |
| **66 unchecked, 0 checked** | **PASS** | 66 `- [ ]` and 0 `- [x]` before and after. |
| No id inserted, deleted or renamed | **PASS** | I extracted all 114 ids (20 FR, 12 AC, 66 checklist, 7 GQ, 9 T): the lists are identical before and after. The new trace table adds no id. |
| Status still DRAFT | **PASS** | `spec.md:4` still reads "DRAFT — NOT APPROVED / NO PRODUCTION, ASSET OR UNITY AUTHORITY". `tasks.md` T-02..T-09 are still BLOCKED and only T-01 (docs/review) is releasable. |
| No zero-width characters | **PASS** | U+200B count is 0 in all four spec files. The `VegetationSites` retype in `plan.md` is clean. |

## 3. Eight findings and two nits

### Majors

**M-1 (historical 2.6 m planks under Route A / H1): ADDRESSED, STILL EXPLICITLY BLOCKED.**
- FR-09 now records the plank/Route A coincidence and the 32 + 20 plank split, and attributes the 0.005 m match to "#588 source inspection". It forbids rendering over, or leaving gameplay consuming, the 2.6 m track.
- The scene choice is the blocking **GQ-07** row. Option **A** (separate Compact scene or hierarchy) is labelled "**preferred DRAFT** option" and "not approved". Option **B** (profile-gated `Renderer.enabled`-only exclusion, never `SetActive` or `Destroy`) is labelled "alternative requiring a separate source + independent review". FR-09 says "Neither choice is approved here."
- The B lifecycle rules are present: order relative to `StandApproachPath` clones and headland `CreateSurface`, re-evaluation on `StandApproachPath.Version`, Rack V2 toggle and Legacy40 load.
- The gameplay consumers (`ForestStartingStand` corridor, `StandMapSource`, `PrescriptionApproval`, coverage) are routed to parent Compact source-spec reconciliation, not to an art-only hide.
- AC-02 and AC-05 require no visible or consumed historical track, and old states unchanged. Checklist FR-09, AC-02, AC-05, G573-M2 and NEG-04 mirror this. `plan.md` Phase 0 and Phase 3 and `tasks.md` T-02 and T-04 depend on GQ-07, and the plan's protected-surface list says the inclusion/exclusion is "UNDECIDED (GQ-07); do not hide/set inactive or clone until accepted".
- Nothing in the delta states that a scene architecture has been chosen. The scene question is **not** marked fixed by prose.

**M-2 (failed shelter already crosses the 1.2 m core): ADDRESSED, STILL EXPLICITLY BLOCKED.**
- FR-05 limits the +0.121 m B2 headroom (and the +0.021 m adverse-shift figure) to **upright shelters only**. It records the failed visual (28° tilt, +0.25 m world x) and #588's O1 −0.141 m and O2 −0.155 m figures, attributed to "#588's source-plane mesh calculation".
- The text says the failed visual has no active collider today, so this is a **visual** overlap and not a measured physics block. It calls this a "known draft candidate FAIL" and says "do not declare 1.2 m G1 clearance PASS".
- FR-15, GQ-02 ("BLOCKING to native clearance"), AC-02, AC-06, NEG-02 ("known source FAIL"), T-02, T-04 and T-06, plus the plan table, all carry it. The remedy is left open: a bounded path-aware failed envelope or a signed route/site change. Examples such as "lean away from path" are marked as examples.
- The text keeps `failedYear`, Browse efficacy and the owner's 2 Oak + 2 Beech out of scope for the fix.
- I read the cited source (`Assets/ForestPrototype/ScenarioProtectionVisuals.cs`). The failed state is `Quaternion.Euler(0,0,28)` plus `position += (0.25, −0.1, 0)`. The collider is destroyed at creation. The tube is 1.2 m × radius 0.10 m. The 28° tilt about the centre puts the tube's horizontal extent at about ±0.37 m around a centre shifted +0.25 m, which matches #588's −0.12 to +0.62 m range. I did **not** re-run the −0.141 and −0.155 route intersections. They are #588 results, correctly attributed.
- The "known conflict, not a runtime physics pass" and "visual overlap" framing is preserved.

### Minors

- **m-1 (path and zero-width character): ADDRESSED.** `plan.md` now names `Assets/ForestPrototype/ScenarioOne/ScenarioHabitatVisuals.cs`, `ScenarioOneClearance.cs`, `ClearancePreview.cs` and `VegetationClearance.cs` with the right folder. I confirmed all four exist under `Assets/ForestPrototype/ScenarioOne/` on `main`. `VegetationSites` is retyped.
- **m-2 (vegetation verifier, determinism, rush): ADDRESSED, scoped not weakened.**
  - FR-11, plan Phase 4, T-05, AC-04, NEG-03 and checklist FR-11 / G573-M5 require rendered + suppressed = source visual instances per cell and kind, rush anchors, a per-class footprint radius, and deterministic `Jitter(cell, kind, plant, axis)` or a verified stable mapping, never `UnityEngine.Random`.
  - The existing `VerifyYear0GrassSupport` check is correctly cited. The source has `Check(patches.Length == visuals.GrassPatchCount, …)` at lines 202–203 of `Assets/ForestPrototype/ScenarioHabitatPresentationVerification.cs`.
  - FR-11, plan and T-05 all say the Compact assertion is **additional and Compact-scoped** and that the Legacy40/E80/Reference assertion stays intact ("no removal or weaker global test"). The verifier is profile-scoped, not deleted or weakened.
- **m-3 (W 0.35 m edge versus 0.20 m verge): ADDRESSED.** FR-16, plan Phase 3, T-04 and checklist FR-16 require re-proportioning or tapering W (and N2) near G1 before any native comparison. The arithmetic checks out: 1.020976 − 0.6 − 0.1 − 0.35 = −0.029 m. Blender grayscale and look remain worker-reported, not acceptance.
- **m-4 (2–6 m leaf-art overreach): ADDRESSED.** FR-14 intent, AC-06, T-06 and the checklist now separate species identification around 2–3 m from 4–6 m findability through an owner-approved marker or inspection affordance, and state "not enlarged leaves".
- **m-5 (ISO / T-03B naming): ADDRESSED.** `spec.md:83` now names the signed `specs/scenario-independent-save-isolation-v1/` ISO-58 teaching gate and ISO-63 native verification. It separates `T-03B/T-04/T-05` as the Scenario One save workstream **#544**, "not tasks in this graphics spec or the signed save spec". I confirmed the #544 / T-03B attribution in `active-tasks.md` on `main`. Compact activation stays BLOCKED, and the no-silent-scene-swap migration sentence is preserved.
- **m-6 (trace table and width facts): ADDRESSED.** `spec.md` has a new FR→AC→task→checklist table.
  - I checked it: all 20 FRs appear exactly once and all 12 ACs are referenced. The task ids are plausible against the T-01..T-09 titles.
  - The table says it is "DRAFT mapping, not evidence" and adds no gameplay criterion.
  - FR-04 now states the 1.8 m (+0.021 m B2, no verge) and 2.4 m (O1/B2 centres inside; all four upright guards overlap) facts, which match #588's #583 numbers.

### Nits

- **n-1 (FR-08 and "APP"): ADDRESSED.** Checklist G573-M4 now lists FR-08/17 and the tasks table says "**APPROVED**". No `APP` token remains.
- **n-2 (packet 1.8 m direction): ADDRESSED on the repair branch only.** The #588 packet now reads "1.8 m core too wide to retain a meaningful shoulder around the current G1 positions", and no "too narrow" remains in it. The original #587 branch and the published #588 report are untouched.

## 4. New observations on the changed lines (all non-blocking)

**m-A (MINOR) — the repair deleted unrelated scope-protection clauses from T-06 and T-04.** In rewriting those `Scope:` lines, the diff also removed content that no #588 finding asked to change:
- T-06 lost "map models by actual species/juvenile IDs and protection state" and "**keep existing later-stage adult/sapling variants**". A search shows the second clause now appears nowhere in the four files. It was the only line telling a T-06 worker not to replace non-juvenile tree visuals.
- T-04 lost "no vehicle access on footpath" and "correct path/Stand Map information". The first is still covered by FR-03 (pedestrian-only). The second is plausibly parent scope.
- *Failure scenario:* a future T-06 worker reading only the task text has no stated instruction to leave existing adult and sapling visuals alone.
- *Smallest remedy:* restore the "keep existing later-stage adult/sapling variants" and "by actual species/juvenile IDs and protection state" phrases in T-06 as one added clause. T-06 is BLOCKED, so this is a spec-quality issue, not a live risk.

**n-A (NIT) — three smaller unrequested trims.**
- `plan.md` Phase 3 dropped "Share no mutable material assets with old worlds". FR-08 and the protected list still cover it.
- `plan.md` Phase 4 dropped "proportionate understorey/dressing set based on tracked first-party art" and "visual shadow and visibility without extra biome coefficients". FR-12 covers reuse. The shadow point is not stated elsewhere.
- FR-16 dropped "no material finalisation without seeing real Compact native views". Checklist FR-16 and AC-03 still require native views.

**n-B (NIT) — the task release table does not list the new gates.** The `tasks.md` release table row for T-02 still reads "APPROVED spec, world manifest/save". The T-02 task body now also requires GQ-07 and GQ-02. Adding "GQ-07/GQ-02" to the T-02 and T-04 rows would keep the summary table consistent with the task bodies. The table still marks every row BLOCKED / NO, so nothing is released by the omission.

**n-C (NIT) — Option B's protected-set wording is shorter than #573 M-2.** FR-09 option B says the exclusion "protects original entities and their template/active-state dependencies". It does not repeat that the protected set also includes the runtime clones and the cutting. The coverage is present in AC-05 and G573-M2 ("template/active-state/renderer/source semantics") and "Historical … unchanged". One short phrase in FR-09 B would make it self-contained.

## 5. What this review does not establish

- No spec approval. GQ-07, GQ-02, GQ-03 (D-032), owner look, the final world manifest, parent/child teaching and finance gates, ISO-58 and fresh Unity verification all remain open.
- The M-1 plank-centre coincidence (within 0.005 m) and the M-2 −0.141 / −0.155 / −0.341 figures are **inherited from #588**. I did not re-parse `ForestTest.unity` or re-run the route-geometry intersection. I only confirmed the cited source constants and that the numbers match #588's published tables.
- The #590 statements that its own registration preceded its edits, and the #590/#587 issue comments, were not checked, because this session has no GitHub issue access.
- No #508 data, Blender scratch scenes, native capture, asset licence or look review.
- Dashboard / Drive mirror: **NOT VERIFIED**.

## 6. Handoff

**Verdict: APPROVED for draft document delta quality.** The two major findings are documented as blocked decisions and are not magically fixed. The 20 FR / 12 AC / 66 unchecked structure is intact.

**Recommended next step for #424:** accept the delta quality, and optionally restore the T-06 "keep existing later-stage variants" clause (m-A) and align the T-02/T-04 release-table rows (n-B) in a single docs-only commit. Do **not** treat this verdict as spec approval. GQ-07 (scene architecture), GQ-02 (failed-shelter envelope and corridor), the world manifest, the owner look decision, D-032 and independent Unity verification are still required before any production task.

**Repository mutation by this review:** this one file on `review/sen-1-591-590-graphics-delta-20261116-v1` (plus the empty branch registration push at `2a76f77b`). Author, #587, #588 and `main` files are untouched. No #591 claim comment was posted (see Registration).
