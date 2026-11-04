# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed history belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`.

**Last reconciled:** 2026-11-04 by Overall Manager after merged docs-only PR #252 and closed #251 research; D-069 conditional Option A sample/coverage direction accepted for DRAFT analysis only; #253 offline qualification READY. No production authority or Unity lane.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- **#200 is the sole primary writer for this file during the current two-lane split.**
- Workers refresh this file and their child claim before asserting ownership/lock availability.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Heavy Unity is serialized machine-wide; a historical `free` receipt proves nothing about current availability.
- Claims track execution presence only; they do not grant ownership.
- No Windows build before the user's personal Scenario One playtest and explicit post-playtest authorisation.
- After meaningful state changes, #200 refreshes `jermur788/Overall-Dev/updates/ccf/latest.md`; CCF remains canonical.

## Current integration truth

The published Scenario One integration on `integration/scenario-one-player-test-ready-v1` is now the **final player-test candidate**. It includes the exact Manager-approved #232 Stand Map species-canopy differentiation by a lease-checked non-forced fast-forward from the exact Tier-3-approved base to the already-qualified direct-child production commit.

Accepted current-lineage state includes Rack V2 / D-064 VERIFIED; Sitka/Oak/Beech parity P0 VERIFIED; S1-C review/history accepted; S1-D repeated-intervention qualification accepted; MFM-08/09 integrated; MFM-10 `SPECIES GUARD QUALIFIED`; MFM-11 `MULTI-SCALE CALIBRATION QUALIFIED`; MFM-12 `MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`; #199 Combined HUD/Focus approved; #223 verification debt approved/integrated; #224 Objectives overflow approved/integrated; #230 fit-replay repair approved/integrated; final Tier-3 #231/#240 approved; and #232 Stand Map species-canopy differentiation **APPROVED / INTEGRATED**.

MFM-11 boundary remains explicit: extension-state calibration is within accepted GAMEPLAY CALIBRATION tolerance; post-treatment source-vs-current-Covered CI parity remains **NOT DEMONSTRATED**. The three pure-Beech reduced-simulator R2 misses remain accepted C-level limitations against the unchanged 0.40 criterion.

## Final Tier-3 and #232 delta

#231 Scenario One final Tier-3 qualification and #240 evidence closure are **APPROVED / CLOSED**. #232 then changed only procedural Stand Map canopy presentation plus durable Stand Map verification assertions.

#232 affected verification accepted by Manager:

- offline Stand Map checks 176/0;
- mixed-species before/after full-map rendering at 1280×720 and 1920×1080;
- unchanged 1,416-tree map-model fingerprint and simulation WorldHash;
- Combined HUD/minimap 465/0;
- Rack map-planning PASS;
- Rack confirm/minimap PASS;
- PARITY-07 rendered PASS;
- compile PASS;
- Scenario One completion PASS with the accepted completion fingerprint;
- no material redraw/performance regression;
- fresh cross-provider OpenAI read-only review: **APPROVED**.

Manager review:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/MANAGER-REVIEW-20261103.md`

Integration handoff:

`Docs/Handoffs/StandMap-Species-Canopy-Differentiation/INTEGRATION-HANDOFF.md`

Because the integration is the exact already-qualified direct-child commit on its original parent, no Unity or rendered/Tier-3 rerun is required solely for integration.

## Live / released tasks

| Task | Lane | Ownership | Status |
|---|---|---|---|
| **Browser / GitHub Manager Lane 03** (#200) | browser/GitHub | Manager review, releases, coordination | **RUNNING — UMBRELLA** |
| **Local Deep Orchestration Resume 02** (#203) | local orchestration | local routing + housekeeping | **RUNNING** |
| **Scenario One final Tier-3 qualification** (#231) | #203 local | parent milestone qualification | **APPROVED / CLOSED** |
| **Tier-3 Task 2 evidence closure** (#240) | #203 local | three evidence actions + delta review | **APPROVED / CLOSED** |
| **Stand Map species canopy differentiation** (#232) | #203 local / Unity | bounded player-facing map presentation | **APPROVED / INTEGRATED** |
| **#232 final integration** (#242) | browser/GitHub | exact fast-forward publication | **INTEGRATED / CLOSE** |
| **PARITY-03 Q10** (#74) | #203 when explicitly scheduled | non-blocking verification | **WAITING — NON-BLOCKING** |
| **Astra onboarding visual refresh** (#243) | Original isolated feature and verification lanes complete | Manager-approved and independently reviewed intro presentation, staged separately; formal 288/289 FAIL retained with specific waiver | **APPROVED / STAGED / LEASE RELEASED / CLOSED.** Original owner posted a normal release and FREE status receipt at 21:09 UTC; no current machine state is inferred from it. |
| **Independent intro visuals / exception review** (#244) | Claude Sonnet 5.5 Medium, read-only | Inspected both logs, all six raw screenshots, sources and visual fidelity; narrow waiver recommended | **APPROVED / CLOSED**; formal gate remains FAIL, review not a Unity rerun. |
| **Isolated intro refresh staging** (#245) | browser/GitHub Manager, no local Unity | Exact Git ref of independently reviewed #243 candidate, separate from active playtest lineage | **STAGED / CLOSED** — `integration/scenario-one-intro-visual-refresh-v1` published at reviewed feature tip; existing `integration/scenario-one-player-test-ready-v1` unchanged. |
| **Access spine / diagonal rack reconnaissance** (#246) | DPC, Claude Sonnet 5.5 Medium source audit + independent GPT-6.1 Terra Medium verification | Read-only inspected published player-test candidate; V2 ledger 4 events | **APPROVED / CLOSED — RESEARCH ONLY.** 13 PASS, 8 corrected, 1 FAIL original MFM kind taxonomy recorded/corrected before publication. No Unity/layout fit proof and no spec approval. |
| **Access spine geometry/MFM measurement** (#248) | DeepSeek DPC / isolated Unity measurement lane, historical lease released | Corrected curved-spine geometry and MFM mask sensitivity, cross-provider review preserved, docs-only PR #249 merged | **APPROVED / CLOSED AS RESEARCH ONLY.** No implementation, no feature/runtime acceptance. |
| **Full-spine 45° five-rack reference proposal** (#250) | DeepSeek DPC / original isolated measurement lane, historical lease released | 3W+2E five-rack reference, 45° spatial analysis and independent review | **APPROVED/CLOSED AS EVIDENCE, STOP IMPLEMENTATION.** Derive rejects this candidate's common-normal rack order; workbench conflict, headland-only entrances, corridor overlap, impacts and renders remain unresolved. |

| **MFM nonparallel rack sample/coverage options** (#251) | DeepSeek DPC, Claude Sonnet 5.5 Medium source study + GPT-6.1 Medium independent review | Compared side-local, network/graph and parallel fallback; lifecycle validity gap and save/hash semantics documented | **APPROVED / CLOSED AS CORRECTED RESEARCH.** Docs-only PR #252 merged; original review 28 PASS / 8 corrected / 4 FAIL, fully disclosed; no MFM change or spec approval. |
| **Offline Option A sample/coverage and rack validity qualification** (#253) | DeepSeek DPC; source-only candidate and conditional separate docs/evidence lane `docs/mfm-option-a-offline-20261104-v1` (must verify unused); Sonnet 5.5 Medium or GPT-6.1 Medium based on actual availability | **READY / NOT RUNNING — OFFLINE DESIGN/PROPERTY TEST ONLY.** Deterministic unique ownership, conservative reachable coverage, sample transfer, Confirm+confirmed-mutation+invalid-load guards, version/hash compatibility, cross-provider review | **NO UNITY, NO PRODUCTION WRITER.** Binding `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/OPTION-A-OFFLINE-QUALIFICATION-PACKET.md`. D-064/D-065 controlling; four-file new spec DRAFT/STOP. |

Housekeeping: browser tooling created temporary remote refs `tmp-noop-integration-check` and `noop`, both pointing at the integrated candidate. They do not affect product or candidate state. Local orchestration should delete them when convenient; browser connector has no branch-delete action.

## Astra #243 initial ownership and Unity lease boundary — earlier 2026-11-03 preflight (historical; superseded by latest return above)

- Manager #200 has **assigned exclusive onboarding production-file ownership to Astra #243** for the file set listed in the live table above. Other lanes must not edit those files while the claim remains active; this is ownership assignment, not proof that a worktree exists.
- The local branch/worktree names are authorised for isolated use **only if available**. Production must branch from the published player-test base, never write to `main` or the published integration checkout; preserve unrelated/untracked `Scenario1V1/` content reported in an existing checkout.
- **Unity slot:** the Manager authorises #243 as the next bounded Unity user for this task, but cannot confirm the machine is idle remotely. A real Editor/process/lock/user-playtest preflight and machine-wide lease acquisition by the local executor are mandatory before Unity starts. If the user's personal Unity playtest or another owner is active, remain waiting rather than claiming/replacing the lease. No competing Unity use permitted.
- Astra's latest claim comment reports the execution chat is attached to a **read-only Game Dev reference mirror**, not the real CCF repository/workspace. This is an environment blocker independent of file ownership and user approval. Reopen/attach the real CCF repository as a writable code-workspace root in the Astra environment before attempting managed `create_worktree`; do not bypass the workspace system or modify the existing checkout.
- Latest user-approved three images **in Astra's own execution chat** are reported to differ from earlier handoff ZIP hashes. Per latest direct user direction they are the visual source; obtain hashes from actual local originals and preserve both provenance sets. Added Page 1 sentence stays byte-exact and teal/blue.
- No Windows build, Player build, accepted-test rebaseline, or merge is permitted from this assignment; existing approved Tier-3, #232 and personal playtest remain intact.

## #243 returned onboarding implementation — earlier Manager checkpoint 2026-11-03 (HISTORICAL, superseded below)

The user-approved 3-page intro artwork and copy have been implemented on the isolated feature branch; **not integrated** into the published player-test candidate. Handoff: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md` (feature branch). Returned evidence: two focused Unity Editor runs, **288 checks passed and 1 failed** each, 6 screenshots each at 1280×720 and 1920×1080. The single `NO_EXCEPTIONS_OR_ERRORS` failure reports a UnityEditor.Search indexer `ArgumentOutOfRangeException` at Editor startup. The formal gate result remains **FAIL** (exit 1); no current product defect is demonstrated but environmental causation is not independently established. Screenshot visual quality is worker-reviewed only; Browser Manager has not directly inspected the externally stored raw captures.

**Manager verdict: NEEDS MORE EVIDENCE**, not an approval. Select an **exact-exception documented waiver pathway** only if independent cross-provider reviewer #244 confirms both raw traces are Unity Editor startup-only, no CCF runtime effect or other errors exist, and all six real Unity renders satisfy accepted visual requirements. Do **not** filter the test, label the original gate green, or rerun a full accepted test matrix. Packet: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/REVIEW-PACKET-20261103.md`.

**Operational block:** #243 worker reports lock remains held under its own owner ID and the lane is clean; Manager has not independently verified machine state. Only that **same owner**, after checking the deployed lock service/Editor processes and matching exact lease, may release the lock. No remote forced unlock, process kill, arbitrary cleanup, or fresh Unity while ZX20 is under its 15 GiB free-space floor (reported ~14 GB). Record local release proof on #243. Review #244 can run without Unity.

The previously issued player-test-ready token applies solely to the already-published integration candidate, unchanged by this unintegrated feature. User personal playtest is **in progress**, not accepted. No Windows build until personal playtest completion and explicit user authorisation.

## #243 final Manager disposition, #244 review and #245 staging — 2026-11-03

**Manager APPROVED** the bounded three-page onboarding visuals and copy, with a narrowly documented acceptance **waiver** for the exact Unity Editor Search indexer startup exception. The original **two Unity gates remain formally FAIL** (288/289, `ONBOARDING_FAIL`, exit 1). No test assertion/filter changed; no additional Unity run. The independent reviewer opened all six real rendered images at 1280×720 / 1920×1080 and verified Page 1's teal sentence and gradual retained-tree visual, Page 2 icons/halo/text fit, and Page 3 several mature Sitka. Minor accepted visual notes: Page 1 art ~10% vertical compression, Page 2 card 4 ~10 px right margin. Exact provenance uses the newer user-approved originals recorded in the 3 Nov art manifest; older ZIP hashes remain history. The error occurred **after CCF Awake/Start diagnostics but before the first onboarding check**, correcting an inaccurate implementation handoff phrase.

Mainline review: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/MANAGER-FINAL-REVIEW-20261103.md`. Independent review on `review/scenario-one-intro-astra-independent-20261103`: `INDEPENDENT-REVIEW-20261103.md`.

#245 published a **separate, exact reviewed Git ref** `integration/scenario-one-intro-visual-refresh-v1`, without file reconstruction, local Unity, test rerun or any change to the user's current `integration/scenario-one-player-test-ready-v1` baseline. Mainline staging handoff: `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/STAGING-INTEGRATION-HANDOFF.md`. **The user has not completed or accepted their personal playtest.** Do not silently switch their test branch; coordinate a transition before promotion.

**Local safety outstanding:** #243's original owner is the only party authorised to check the machine-wide held Unity lease and release its own exact matching lock after confirming no Editor processes; request fresh FREE receipt on #243. No forced unlock. The independent reviewer observed about 28 GB free on ZX20, superseding the earlier ~14 GB report, but it is a point-in-time receipt, not a substitute for a new admission check. No Unity run is requested now.

No Windows/Player build before personal playtest is complete and explicit post-playtest authorisation; no Reference Future v1/Legacy40, MFM or simulation changes from #243. #194 NO ACTION.

## Access-spine direction and DPC orchestration — 2026-11-03

The user's continuing Scenario One playtest has accepted a **conceptual** access spine: extend the existing central route where viable, connect diagonal racks, and keep 3D, Stand Map, minimap and marking consistent. Decision Log **D-067** records preferred direction but does **not** change D-064's approved Rack V2 plan. The user explicitly permits **Claude Sonnet 5.5 Medium** and **OpenAI GPT-6.1 Medium** as workers under **DPC** orchestration. No claim of current provider availability is made.

The **four-file DRAFT** spec is `specs/scenario-one-access-spine-route-consistency-v1/`. #246 is a read-only T3 code/scene reconnaissance only. Durable packet `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/RECONNAISSANCE-PACKET.md` and binding V2 ledger `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md` are published. DPC must record actual provider capacity/window/reset and context usage, alternatives, routing reasons, switches, tokens/runtime when observable, findings and outcomes **as they occur**, including a V2 Orchestration Decision Appendix in the return. Unknown remains UNKNOWN. The historical 30 Oct ledger and separate #241 backfill are outside this ledger's ownership.

To keep development moving, DPC can hand off and switch among **available, permitted Medium workers** or route a separately authorised, non-overlapping READY task while one task is blocked. It cannot implement DRAFT specs, bypass exclusive writer/Unity lock, override user testing or publish integration without Manager review. #200 remains sole live coordination writer. No Windows build.

## #246 Manager review and next blocked #248 — 2026-11-03

#246 source reconnaissance returned on main through merged docs-only PR #247 and is **Manager APPROVED/CLOSED as research only**. The independent 22-claim verification found 13 PASS, 8 corrected claims and one original **FAIL (MFM Track/Headland taxonomy)**; its published correction is retained, not relabelled. The DPC V2 ledger has four observed events; provider quotas/context remain UNKNOWN and no switch was fabricated.

**Critical derived risk:** creating more `Dirt Path*` planks automatically adds MFM `MaskKind.Track` treatment-exclusion membership; feeder/headland use `MaskKind.Headland`, also excluded. A1 path extension is **not cosmetic-only** and cannot be dispatched without quantified MFM impact. The central access-spine with diagonal racks remains preferred but **geometry unmeasured**; existing Rack V2/D-064 controls. The four-file new spec remains DRAFT.

Manager review: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-REVIEW-20261103.md`. New #248 real Unity geometry/mask measurement packet is prepared but **BLOCKED**, since exact running playtest candidate, safe Unity slot, original #243 lock release and Manager-assigned exclusive lane are not yet established. Do not grant #248 file ownership or claim runtime PASS. DPC may progress only independent authorised tasks without overlapping locks or writer scopes. The user's personal playtest remains ongoing; no Windows build authorisation.

## User frees Unity occupancy — #248 conditional local release, 2026-11-03 21:00 UTC

**Direct user confirmation:** nobody is using Unity now; work may resume. This resolves the **live-user occupancy** restriction on #248. It **does not** establish that the earlier personal Scenario One test is completed or accepted, reveal the exact local candidate, or prove the machine-wide serialized Unity lease is FREE.

Overall Manager now releases **#248 LOCAL PREFLIGHT** and conditionally reserves `verify/scenario-one-access-spine-measure-20261103` at `/media/jer/ZX20/CCF-AccessSpine-Measure-20261103` from the published player-test candidate, with strong preconditions: name/worktree must be unused, actual base/candidate confirmed, ZX20 capacity sufficient for added Unity Library with post-creation >15 GiB floor, no overlapping owner, no active Editor/display processes. DPC routes the **original #243 lock owner** to safely release only its exact matching lease using deployed CCF-Ops and records a current FREE receipt; DPC then acquires a new exclusive #248 lease. No force-release or workaround. If safe conditions fail, #248 returns BLOCKED; no Unity may launch. If all pass, the worker marks [RUNNING] with exact provider/model Medium effort, branch/worktree/base/lease and does bounded read-only geometry and hypothetical MFM mask measurement. DPC appends contemporaneous V2 ledger events and obtains independent review.

**No draft-spec coding or A1/A2 production, Windows build, change to user-save or player-test branch.** The user's playtest is still unaccepted; completed personal testing plus a separate explicit authorisation are mandatory before a Windows build.

## #243 release receipt accepted — #248 fresh admission gate

The original holder of #243 posted a matching non-forced Unity lease release with a FREE post-check at 21:09 UTC. Manager verified the GitHub receipt and closed #243. This resolves the old lock-release obligation; it does not confirm future machine availability. #248 can begin local preflight immediately. DPC must verify current process, lock, disk, branch and ownership state, then acquire its own new exclusive lease before read-only scene/MFM measurement. The personal playtest is not yet complete. The separately staged intro and published player-test baseline stay unchanged. No production or Windows build.

## #250 STOP for implementation and #251 read-only MFM contract — 2026-11-03

Docs-only **PR #249 MERGED**, publishing corrected #248 and #250 handoffs plus V2 ledger events. #248 and #250 are **APPROVED/CLOSED as research only**. #250's full-spine 45°/3W+2E candidate meets the site-specific spacing guideline in the derived reference layout but **cannot be implemented** under current MFM rack-sample geometry: production `ScenarioOneRackSampleGeometry.Derive` returns `Valid=false` for projected ordering. Additional independent blockers: spine/Forestry Workbench conflict, headland-only entrance authoring, R1/R2 corridor overlap, one trunk-collider endpoint, unqualified turning/3D/map/minimap render. Union corridor tree impact **120 living trees** is a scenario consequence, never an approved removal. The Track vs Headland mask row mix-up was detected by independent review and corrected: full proposed network as Track = **156 newly masked, 111 lost treatable stems, 459.82 m² lost**; as Headland = **150, 106 and 448.03 m²**. These sensitivity figures are **not qualified MFM prescriptions** because the candidate's own sample derivation fails. #250 review tally is 7 PASS / 6 corrected / 2 FAIL.

Manager verdict: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-FIVE-RACK-REVIEW-20261103.md`; four-file access-spine spec remains **DRAFT / STOP FOR IMPLEMENTATION**. New [#251](https://github.com/jermur788/CCF/issues/251) is **READY read-only** to compare branch/side-local MFM coverage, generalized network/graph coverage and alternative parallel geometry retaining the existing model. DPC must publish a source-backed, independently reviewed `MFM-TOPOLOGY-OPTIONS-HANDOFF.md`, maintain the V2 ledger with publish-time raw-number re-extraction, and return for Manager/product decision before any production spec/implementation dispatch. Its own docs-only lane remains conditional on clean unused ownership; no Unity is required. Historical #250 lock release does not grant new Unity ownership.

The user's personal player test is not completed/accepted and its published branch and save are unchanged. **No Windows build** before completion and explicit subsequent user permission.

## #251 corrected research and conditional A design / #253 offline qualifier — 2026-11-04

The #251 read-only study and contemporaneous DPC V2 ledger are on main through **merged docs-only PR #252**. Source audit/independent review established a specific two-family common-normal failure for the 45°/3W+2E reference, and a separate **current Rack V2 validity lifecycle gap**: Confirm, direct `UpdateRack` on confirmed plans and save validation do not check MFM `Derive` validity. The review initially returned **MATERIAL ERROR** (40 claims, 28 PASS, 8 corrected, 4 FAIL): missing confirmed-mutation gate, hashes inferred incorrectly from unchanged schema, stable unit IDs mistaken for meaning, curved-boundary overclaim. All are disclosed/corrected; #251 is **APPROVED/CLOSED as research only**. No end-to-end irreversible player failure was proven.

Manager [D-069] records **Option A conditional design direction** only: additive branch/side-local derivation, original parallel derivation unchanged, deterministic one-owner tree/area allocation, conservatively verified reach, one demonstrated sample for both groups only as a *hypothesis*, and **Confirm + every authoritative confirmed-plan mutation** validity policy with old invalid saved plans loadable but unsafe new actions suspended while preserving completed history. Option C parallel fallback remains; generalized persisted graph B1 not justified at five-rack scope. The 9 m teaching half-width is **not** an approved reach cap, and the user's preferred 45° geometry is not an accepted executable layout. D-064/D-065 remain binding, as do separate legal spine entrance, yard bypass, corridor overlap, turning and missing render blockers.

Manager review: `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/MANAGER-MFM-TOPOLOGY-REVIEW-20261104.md`. Four-file spine spec stays **DRAFT / STOP IMPLEMENTATION**. Next #253 is **READY for offline nonproduction analysis only**; no Unity or production owner. DeepSeek DPC must verify source/docs-lane ownership, monitor model limits, append V2 ledger and cross-provider review, source-reconcile all numbers and publish `OPTION-A-OFFLINE-QUALIFICATION-HANDOFF.md` before Manager spec approval. The established user-playtest branch and separately staged intro remain unchanged. User personal playtest not completed or accepted; **no Windows build** without explicit post-playtest authorisation.

## User / build gate

Browser Overall now issues:

`SCENARIO ONE PLAYER-TEST CANDIDATE READY — USER PLAYTEST REQUIRED`

The user must personally playtest the exact published `integration/scenario-one-player-test-ready-v1` candidate before any Windows build is authorised.

After the personal playtest, Browser Overall records the user-test result. A Windows build remains blocked until the user explicitly authorises it **after** that playtest.

## Hard boundaries

- Research Source Consolidation Audit #194 remains **NO ACTION**.
- Frozen Reference Future v1 and Legacy40 compatibility remain protected.
- Never weaken tests or silently rebaseline protected anchors.
- User-directed #238 test-volume reduction remains binding; do not rerun valid Tier-3 evidence without a new reason.
- The accepted stale economy v14 fixture remains non-blocking verification debt; it is not part of the player-test gate.
