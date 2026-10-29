# Scenario One — Remaining T3 / Code Work Handoff Matrix

**Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`  
**Prepared:** 2026-10-29 by Overall Manager / browser lane  
**Status:** PREPARED ROUTING MATRIX — NOT A RUNNING CLAIM

This document provides one routing/dispatch handoff for every remaining local T3/Code task currently defined by the accepted Scenario One programme. It does **not** grant production ownership and does **not** mean any worker is running. Each task must refresh `Docs/Project/coordination/active-tasks.md`, satisfy its canonical task packet gates, and create/use its own substantive claim before production/Unity work.

The canonical task packets remain authoritative for scope. If a later Manager/browser packet amendment or direct user instruction conflicts with this matrix, the newer authority wins.

## Already prepared live claims — do not duplicate

| Claim | Task | Difficulty | Recommended agent | Ultra Code? | Status |
|---|---|---:|---|---|---|
| #125 | MFM-07 integration | 3/5 | Claude Sonnet 5.5 Medium or OpenAI 6.1 Sol | No | Prepared for dispatch |
| #126 | PARITY-08 save/deterministic compatibility | 5/5 | Claude Opus 5.5 Low + independent OpenAI 6.1 Sol review | No | Prepared for dispatch |
| #123 | S1-C T-04 provenance labelling | 3/5 | Claude Sonnet 5.5 Medium | No | Prepared for dispatch |
| #124 | S1-D T-04 guide monotonicity | 4/5 | Claude Sonnet 5.5 Ultra Code | **Yes** | Prepared for dispatch |

These four already have exact GitHub claims and should be dispatched from their existing packets rather than recreated.

---

# Remaining implementation / qualification handoffs

## T3-05 — S1-C T-06 prior-treatment guidance

**Canonical packet:** `Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md`  
**Purpose:** add bounded prior-treatment guidance from persisted history plus current observation, reusing recorded-location navigation without inventing causal facts or prescriptions.  
**Difficulty:** **3/5** — bounded history/UI/copy integration with provenance and waypoint correctness.  
**Best agent:** **Claude Sonnet 5.5 Medium**.  
**Claude Sonnet 5.5 Ultra Code:** **No** — scope is bounded and a single writer is preferable; Ultra orchestration adds little.  
**Suggested lane:** `feature/s1-c-t06-prior-treatment-guidance-v1` / `/media/jer/Videos/CCF-S1C-T06`.  
**Start gate:** S1-C T-04 accepted/released; exact current integration SHA published; no overlapping S1-C history writer.  
**Success:** `S1-C T-06 PRIOR-TREATMENT GUIDANCE QUALIFIED`.

**Dispatch:**
`Run S1-C T-06 from Docs/Handoffs/S1-C-Implementation-06/TASK-PACKET.md only after S1-C T-04 is accepted on the exact current integration lineage. Use Claude Sonnet 5.5 Medium. Build bounded prior-treatment guidance only from persisted history plus current observation, reuse only recorded-location waypoints, and do not invent causal facts or prescriptions. Publish the exact candidate/handoff. No Windows build.`

---

## T3-06 — S1-C T-07 identity/history data-contract stability

**Canonical packet:** `Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md`  
**Purpose:** verify/document the one stable S1-C/S1-D identity/history contract against current and genuine old-save fixtures.  
**Difficulty:** **3/5** — bounded verification work, but identity drift has high downstream cost.  
**Best agent:** **Claude Sonnet 5.5 Medium**.  
**Ultra Code:** **No** — this is contract verification rather than a broad implementation task.  
**Suggested lane:** `verify/s1-c-t07-history-contract-v1` / `/media/jer/Videos/CCF-S1C-T07`.  
**Start gate:** no active save/identity writer; current MFM/parity save lineage pinned.  
**Success:** `S1-C T-07 HISTORY DATA CONTRACT QUALIFIED`.

**Dispatch:**
`Run S1-C T-07 from Docs/Handoffs/S1-C-Implementation-07/TASK-PACKET.md on the exact current published integration candidate once save/identity ownership is free. Use Claude Sonnet 5.5 Medium. Verify the stable tree/order/event/history identity contract with current and genuine old-save fixtures; do not add a competing identity scheme or repair historical IDs. No Windows build.`

---

## T3-07 — S1-C T-08 complete review/history feature qualification

**Canonical packet:** `Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md`  
**Purpose:** Tier-2 Unity/rendered qualification of the complete S1-C review/history feature.  
**Difficulty:** **4/5** — multi-year save/history/UI/rendered/old-save qualification.  
**Best agent:** **OpenAI 6.1 Sol** or Claude Sonnet 5.5 Medium if the OpenAI lane is occupied; escalate only for diagnosis.  
**Ultra Code:** **No** — this is primarily exact-candidate qualification; controlled serial evidence matters more than orchestration.  
**Suggested lane:** `qual/s1-c-review-history-v1` / `/media/jer/Videos/CCF-S1C-T08`.  
**Start gate:** S1-C T-01..T-07 accepted on one exact integrated candidate.  
**Success:** `S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`.

**Dispatch:**
`Run S1-C T-08 from Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md only after T-01..T-07 are accepted on one exact integration candidate. Prefer OpenAI 6.1 Sol for the local qualification lane. Execute the complete multi-year/history/old-save/grouped-harvest/no-work/provenance/rendered matrix at 1280×720 and 1920×1080. Qualification only: stop on production defects. No Windows build.`

---

## T3-08 — S1-D T-05 previous-versus-proposed Work Plan presentation

**Canonical packet:** `Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md`  
**Purpose:** group Approved / Pending new / Pending carried-over ordinary work using S1-C history and the integrated MFM-07 provenance API.  
**Difficulty:** **4/5** — shared Work Plan/history/MFM presentation seam with rendered verification.  
**Best agent:** **Claude Sonnet 5.5 Ultra Code**.  
**Ultra Code:** **YES — one of the strongest fits.** It benefits from coordinated inspection of Work Plan, history and MFM provenance APIs, while retaining one primary production writer.  
**Suggested lane:** `feature/s1-d-t05-previous-proposed-v1` / `/media/jer/Videos/CCF-S1D-T05`.  
**Start gate:** MFM-07 integrated and exact pending-work/provenance API pinned; no overlapping S1-C/MFM writer; #127/browser successor has reconciled ownership.  
**Success:** `S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`.

**Dispatch:**
`Run S1-D T-05 from Docs/Handoffs/S1-D-Implementation-05/TASK-PACKET.md only after MFM-07 is integrated and its exact pending-work/provenance API is pinned. Use Claude Sonnet 5.5 Ultra Code with one primary production writer. Own only the bounded WorkPlan presentation seam, reuse S1-C history plus ordinary MFM-generated pending work, and add no new persistence or duplicate history UI. No Windows build.`

---

## T3-09 — S1-D T-08 three-cycle witness and compatibility qualification

**Canonical packet:** `Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md`  
**Purpose:** execute the three-cycle + idle-year + stale-order + save/load + completion + post-completion + solvency witness.  
**Difficulty:** **5/5** — integrated multi-cycle Unity/save/compatibility qualification across S1-D/S1-C/Work Plan.  
**Best agent:** **OpenAI 6.1 Sol**.  
**Ultra Code:** **No** — the task is verification-heavy and machine-serialized; independence from the later Claude review is more valuable.  
**Suggested lane:** `verify/s1-d-multicycle-witness-v1` / `/media/jer/Videos/CCF-S1D-T08`.  
**Start gate:** S1-D T-02..T-06 published-integrated and accepted, including T-04/T-05.  
**Success:** `S1-D T-08 MULTI-CYCLE WITNESS PASS — READY FOR INDEPENDENT SPEC REVIEW`.

**Dispatch:**
`Run S1-D T-08 from Docs/Handoffs/S1-D-Implementation-08/TASK-PACKET.md only after T-02..T-06 are published-integrated on one exact candidate. Use OpenAI 6.1 Sol. Run the three-cycle, idle-year, stale-order, save/load, completion, post-completion and D-057 solvent witness plus protected regressions. Verification only; no production fixes or rebaseline. No Windows build.`

**Note:** T-09 is an independent review/browser task, not a T3 implementation task; it is therefore not included as a Code execution handoff here.

---

## T3-10 — MFM-08 map / 3D review UX

**Canonical packet:** `Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md`  
**Purpose:** make source sample, coverage, spacing-only candidates and proposals legible on Stand Map and in 3D while preserving non-commitment semantics.  
**Difficulty:** **5/5** — map + 3D interaction + candidate/proposal state + rendered qualification, plus the accepted MFM-07 partial coverage-adapter residual.  
**Best agent:** **Claude Sonnet 5.5 Ultra Code**.  
**Ultra Code:** **YES — strongest remaining Ultra fit.** It spans multiple UI/runtime surfaces and benefits from coordinated source exploration and verification planning. Keep one primary writer.  
**Suggested lane:** `feature/mfm-08-map-3d-review-ux-v1` / `/media/jer/Videos/CCF-MFM08`.  
**Additional start gate:** browser lane #127 (or successor) must first dispose the MFM-07 residual that the production prescription-world adapter omits infrastructure masks/management features. If a bounded prerequisite Code slice is required, that slice runs before this task.  
**Other gates:** MFM-07 integrated; overlapping Stand Map/teaching UI writers released.  
**Success:** `MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`.

**Dispatch:**
`Run MFM-08 from Docs/Handoffs/MFM-Implementation-08/TASK-PACKET.md only after MFM-07 is integrated, Browser Lane 06 has resolved the partial coverage-adapter prerequisite, and overlapping UI ownership is released. Use Claude Sonnet 5.5 Ultra Code with one primary writer. Candidates remain spacing-only inspection suggestions; proposals remain distinct from committed Fell/Crop/Rack marks. Perform real 3D interaction and render at both target resolutions. No Windows build.`

---

## T3-11 — MFM-09 teaching sequence / Work Plan integration

**Canonical packet:** `Docs/Handoffs/MFM-Implementation-09/TASK-PACKET.md`  
**Purpose:** add stable `Extend your treatment` teaching step(s) after Lesson 5 and connect the accepted MFM flow to the existing Work Plan presentation without changing commitment semantics.  
**Difficulty:** **4/5** — shared curriculum + Work Plan + provenance seam with stable-ID compatibility.  
**Best agent:** **Claude Sonnet 5.5 Ultra Code**.  
**Ultra Code:** **YES.** The task must reconcile MFM-08, S1-D T-04/T-05, stable teaching IDs and Work Plan semantics; coordinated exploration is useful.  
**Suggested lane:** `feature/mfm-v1-teaching-workplan-v1` / `/media/jer/Videos/CCF-MFM-09`.  
**Start gate:** MFM-07/08 plus S1-D T-04/T-05 accepted/integrated; no overlapping curriculum/Work Plan writer.  
**Success:** `SCENARIO ONE EXTENSION FLOW QUALIFIED`.

**Dispatch:**
`Run MFM-09 from Docs/Handoffs/MFM-Implementation-09/TASK-PACKET.md only after MFM-07/08 and S1-D T-04/T-05 are accepted on one exact integration candidate. Use Claude Sonnet 5.5 Ultra Code with one primary writer. Add new stable Extend-your-treatment IDs after Lesson 5, preserve all existing IDs, reuse the existing Work Plan grouping, and keep Work Plan approval as the commitment gate. No Windows build.`

---

## T3-12 — MFM-10 Sitka-only species-parity guard

**Canonical packet:** `Docs/Handoffs/MFM-Implementation-10/TASK-PACKET.md`  
**Purpose:** prove Oak/Beech never silently enter Sitka-only quantitative MFM rules and unknown species never fall back to Sitka.  
**Difficulty:** **3/5** — focused eligibility/routing verification with strong product-honesty constraints.  
**Best agent:** **Claude Sonnet 5.5 Medium**.  
**Ultra Code:** **No** — bounded verification/repair scope.  
**Suggested lane:** `verify/mfm-v1-species-guard-v1` / `/media/jer/Videos/CCF-MFM-10`.  
**Start gate:** MFM-09 + PARITY-08 accepted/integrated.  
**Success:** `SPECIES GUARD QUALIFIED`.

**Dispatch:**
`Run MFM-10 from Docs/Handoffs/MFM-Implementation-10/TASK-PACKET.md after MFM-09 and PARITY-08 are accepted on one exact integration candidate. Use Claude Sonnet 5.5 Medium. Prove MFM V1 remains Sitka-adult-only, Oak/Beech are explicitly excluded/manual, and unknown species never fall back to Sitka. Stop for a spec amendment if broadleaf prescription support is required. No Windows build.`

---

## T3-13 — PARITY-09 integrated three-species parity qualification

**Canonical packet:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md`  
**Purpose:** Tier-2 integrated qualification of the complete Sitka/Oak/Beech parity P0 slice.  
**Difficulty:** **5/5** — density, adult/juvenile, browse, pruning, timber, economy, save, UI and completion on one candidate.  
**Best executor:** **Claude Opus 5.5 Low** for the T3 qualification; mandatory independent **OpenAI 6.1 Sol** review.  
**Ultra Code:** **No** — milestone-quality evidence and false-green risk dominate; Opus + independent cross-provider review is preferable.  
**Suggested lane:** `qual/parity-09-integrated-v1` / `/media/jer/Videos/CCF-Parity09`.  
**Start gate:** PARITY-08 PASS + review APPROVED; MFM-10 qualified.  
**Success:** `THREE-SPECIES PARITY P0 VERIFIED` after Manager acceptance of the qualification/review.

**Dispatch:**
`Run PARITY-09 from Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md only after PARITY-08 and MFM-10 are green on one exact integration candidate. Use Claude Opus 5.5 Low for T3 qualification and OpenAI 6.1 Sol for independent cross-provider review. Execute the full parity checklist, protected anchors, MFM Sitka-only guard, completion/solvency and both rendered resolutions. No production fixes inside qualification. No Windows build.`

---

## T3-14 — MFM-11 calibration impact qualification

**Canonical packet:** `Docs/Handoffs/MFM-Implementation-11/TASK-PACKET.md`  
**Purpose:** compare sample-only, representative-prescription and plausible manual-override paths and determine whether wider treatment materially breaks accepted Scenario One outcomes.  
**Difficulty:** **5/5** — controlled multi-path Unity measurement across ecology, economy, canopy, regeneration and completion.  
**Best executor:** **Claude Sonnet 5.5 Medium** for measurement, escalating to **Opus 5.5 Low** only for difficult diagnosis; mandatory **OpenAI 6.1 Sol** independent review.  
**Ultra Code:** **No** — exact comparative measurement and independent review matter more than agent orchestration.  
**Suggested lane:** `qual/mfm-v1-calibration-impact-v1` / `/media/jer/Videos/CCF-MFM-11`.  
**Start gate:** MFM-10, PARITY-09 and S1-D repeated-intervention flow accepted on one exact candidate.  
**Success:** `MULTI-SCALE CALIBRATION QUALIFIED`; failure/stop token `BLOCKED — CALIBRATION IMPACT`.

**Dispatch:**
`Run MFM-11 from Docs/Handoffs/MFM-Implementation-11/TASK-PACKET.md only after MFM-10, PARITY-09 and the repeated-intervention flow are accepted. Use Claude Sonnet 5.5 Medium for the measurement run and OpenAI 6.1 Sol for independent review. Compare sample-only, representative-prescription and plausible manual-override paths across removals, BA/canopy, economy, deadwood/timber, regeneration/light, completion, solvency and protected anchors. Do not retune to pass. No Windows build.`

---

## T3-15 — MFM-12 integrated MFM V1 qualification

**Canonical packet:** `Docs/Handoffs/MFM-Implementation-12/TASK-PACKET.md`  
**Purpose:** end-to-end qualification of Multi-Scale Forestry Management V1 on one exact integrated Scenario One candidate.  
**Difficulty:** **5/5** — save + map + 3D + Work Plan + completion + rendered + checklist conformance.  
**Best executor:** **Claude Opus 5.5 Low** for T3 qualification; independent **OpenAI 6.1 Sol** architecture/spec review.  
**Ultra Code:** **No** — milestone verification breadth and false-green risk justify Opus plus independent review instead.  
**Suggested lane:** `qual/mfm-v1-integrated-v1` / `/media/jer/Videos/CCF-MFM-12`.  
**Start gate:** MFM-06..11 accepted, PARITY-09 approved, required S1-C/S1-D feature qualifications green.  
**Success:** `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST` after Manager acceptance.

**Dispatch:**
`Run MFM-12 from Docs/Handoffs/MFM-Implementation-12/TASK-PACKET.md only after MFM-06..11, PARITY-09 and required S1-C/S1-D gates are accepted on one exact integration candidate. Use Claude Opus 5.5 Low for the T3 qualification and OpenAI 6.1 Sol for independent review. Execute the full sample→prescription→3D candidate→proposal→Work Plan→execution→review chain, save/old-save/Reference gates, completion and rendered matrix. Qualification only. No Windows build.`

---

## T3-16 — Final Scenario One Tier-3 complete-loop qualification

**Canonical packet:** `Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md`  
**Purpose:** milestone-level qualification of one exact final Scenario One candidate before the user's personal playtest.  
**Difficulty:** **5/5** — highest-risk cross-feature qualification across Rack V2, parity, S1-C, S1-D, MFM, save/reference, completion/solvency and rendered interaction.  
**Best executor:** **Claude Opus 5.5 Low**; independent final reviewer **OpenAI 6.1 Sol**.  
**Ultra Code:** **No** — use the strongest reasoning + independent review rather than broad implementation orchestration.  
**Suggested lane:** `qual/scenario-one-final-tier3-v1` / `/media/jer/Videos/CCF-ScenarioOne-FinalT3`.  
**Start gate:** Rack V2 accepted; parity verified; S1-C qualified; S1-D T-08 PASS + T-09 approved; MFM-12 accepted; one exact final published candidate; no active writers.  
**Mandatory:** fresh S1-C T-05 objective/progression save/history rerun on the final combined candidate.  
**Success:** `SCENARIO ONE TIER-3 COMPLETE-LOOP QUALIFIED — READY FOR INDEPENDENT FINAL REVIEW`.

**Dispatch:**
`Run the final Scenario One Tier-3 qualification from Docs/Handoffs/PlayerTestReadiness-Overnight-01/FINAL-TIER3-TASK-PACKET.md only after every feature-level gate is accepted on one exact published integration candidate. Use Claude Opus 5.5 Low. Qualify Rack V2, three-species parity, S1-C, S1-D, MFM, current/old saves, Legacy40, frozen Reference Future, completion/solvency and both rendered resolutions. Mandatory: rerun S1-C T-05 save/history on the final candidate. No production fixes and no Windows build.`

---

# Ultra Code recommendation summary

Best suited to **Claude Sonnet 5.5 Ultra Code**:

1. **#124 / S1-D T-04** — already prepared; subtle cross-reload teaching-state implementation.
2. **S1-D T-05** — Work Plan + S1-C history + MFM provenance integration.
3. **MFM-08** — Stand Map + 3D interaction + candidate/proposal UX + rendered qualification.
4. **MFM-09** — curriculum + stable IDs + Work Plan + MFM/S1-D API integration.

Not preferred for the large 5/5 qualification gates (S1-D T-08, PARITY-09, MFM-11/12, final Tier-3): those benefit more from a strong single reasoning/verification model and cross-provider independent review than from multi-agent implementation orchestration.

# Claim discipline

Do not create `[RUNNING]` claims from this matrix ahead of dependency clearance. When a task becomes executable, the Manager/browser successor should:

1. refresh the exact integrated base and live ownership;
2. create or release the task's child claim;
3. name actual model/provider, branch, worktree and start SHA;
4. mark `[RUNNING]` before production edits/Unity;
5. publish the required handoff and close/release ownership at return.

No Windows build is authorised anywhere in this matrix.