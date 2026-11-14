# scen1 — Manager #495: owner Option A planting-led transformation DRAFT revision and #496 independent delta review

**Date:** 14 November 2026 UTC. **Manager/author:** OpenAI GPT-6, Scenario One Manager #424, GitHub/browser-only, no local worktree/Unity.  
**Author docs claim:** [#495](https://github.com/jermur788/CCF/issues/495) — [RUNNING] during edits, [HANDED-OFF] after readback/diff; register actual before first write.  
**Reviewer claim:** [#496](https://github.com/jermur788/CCF/issues/496) — READY / NOT RUNNING; non-author separate reviewer.  
**Scope:** **DRAFT REVISION ONLY; NOT AN APPROVED FEATURE SPEC; ZERO PRODUCTION RELEASE.**  
**Difficulty of reviewer:** MEDIUM: constrain new accepted product intent, reconcile source-native findings, four-file consistency and 11 new unchecked requirements. **Recommended provider:** non-author Claude Opus High or independent GPT-6 High; capability/independence and registration matter more than family.  
**Naming:** starts `scen1`, as owner requested for new Scenario One handoffs.  
**Dashboard/Drive/context mirrors:** NOT VERIFIED; no context publication implied.

## 1. Exactly what the owner accepted

At 10:06 UTC on 14 November 2026 the owner **explicitly approved Option A** for first complete Scenario One demo: **Oak and Beech are established by player-led planting/protection and later management; natural Oak/Beech recruitment is NOT required to appear within elapsed 40 years.**

Accepted scope is narrow:
- All three Sitka/Oak/Beech remain subject to D-059 comparable modelling depth, D-060 complete actual management loop, real Work Plan, annual growth/browsing/protection, honest regenerated Sitka cohorts and observed consequences. No owner acceptance of guaranteed planted survival, artificial mature broadleaves, forced seed cohorts or compulsory felling.
- The owner's separate woodland clarification permits a small bounded Irish woodland with fields, roads and hedges beyond; it does **not** assume an external conifer neighbour forest or an off-map mature Oak/Beech seed donor. No *hedgerow seed* mechanism is approved; such a mechanic would need species-specific evidence and a separate approved spec if ever required.
- **NOT ratified:** 20/25/30m size, one-rack P/A lesson details, exactly how many Oak/Beech saplings/years/locations, economy funding/provider, three substantive-cycle count, precise time skips/Year40 end, same-start guided vs independent second run, twin save router, perfect guided marks/scoring, historical 3D rewind, old-world completion changes. OPEN-C1–C8 and future OPEN-1–OPEN-6 remain, plus newly labelled OPEN-C9 cohort details and OPEN-C10 dual-run design. Other user-proposed teaching mechanics are drafted only.

**Canonical conflict managed:** D-082 first-demo revision is still PROPOSED on Overall Manager #425's separate paused [#471](https://github.com/jermur788/CCF/issues/471) review branch. This #495 branch **does not** edit or merge `Docs/Project/decision-log.md`, `current-milestone.md`, `game-brief.md`, paused D-082 files or main. Option A is traceable as a direct user decision here and in the updated DRAFT. #425 must later reconcile D-082/Option A in canonical documents after its own authority/review gate, not overwrite concurrent work or invent a new Decision Log ID. #424 preserves historic five-rack Rack V2/Legacy40/Enlarged80/MFM, model0 and frozen Reference Future v1.

## 2. Actual source/evidence and what we did *not* infer

- [#484 direct offline finite 8m CI study](https://github.com/jermur788/CCF/blob/research/scenario-one-focal-ci-offline-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/FOCAL-BOUNDARY-CI-TIER1-RESULT-20261114.md): actual source trees re-evaluated under finite vs hypothetical outside Sitka. If the property is bounded by open land, finite CI is correct; large outside-tree differences are CONDITIONAL hypotheses, not proof of a defective small-world engine.
- [#490 40-year Unity research report](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Research/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-RESULT-20261114.md) and [handoff](https://github.com/jermur788/CCF/blob/research/scen1-three-species-long-horizon-native-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-THREE-SPECIES-LONG-HORIZON-NATIVE-HANDOFF-20261114.md): two 30m finite Year0-to-40 Unity scratch arms, worker-reported Editor exit0 / **142/142** research fixture checks, identical original 189 trees and early #460 baseline replay. ONE planted Oak/ONE Beech survived and promoted; Y40 height 18.10/19.40m, DBH12.28/15.00cm, **zero natural Oak/Beech seedling cohort bands**; broadleaf own small seed potential only near Year40. Sitka original deaths ~100 per arm; a Y5 Sitka crop release caused +1.34/+2.69cm crop diameters but barely altered planted broadleaf growth. The Oak's **0.45m modelled crown radius** at ~18m and camera occlusion pose visual/biology risks. One optional Y0 image was defective. Bramble/bracken exported but **not analysed**. No source growth tuning, no persisted new compact geometry or player study, no guarantee of mature reproductive hardwood.
- Direct current-main source: `ForestEcologyController.ApplyAdultDensityMortality` invokes `ForestTree.ApplyMortality("self-thinning",year)`; the latter hides the GameObject and does not, by itself, create a standing/fallen deadwood visual. The native study reported aggregate losses, **not per-victim observed mortality causes**. This difference must be explicit in DRAFT and any future narrative.

No #495 Unity, local build, photo inspection, actual source simulation or old-save/reference hash replay was run. Existing #460/#484/#490 results are cited as *external task evidence* with their limits, not newly certified by this author.

## 3. Exactly what changed — five-file allowlist

**Base:** independently reviewed #465 candidate `docs/compact-one-rack-463-three-findings-repair-20261113-v1`, starting source `06dcb22d5f5011ac4f911eb3b8a2fc544e1055f4`. [#467](https://github.com/jermur788/CCF/issues/467) had returned **APPROVED FOR OWNER/SAVE-DECISION PREPARATION** on F1–F3 text, not product authorisation. Our new author branch is `docs/scen1-planting-led-compact-draft-20261114-v1`, initially 0 ahead/0 behind base.

**Allowed modifications only:**
1. [`spec.md`](https://github.com/jermur788/CCF/blob/docs/scen1-planting-led-compact-draft-20261114-v1/specs/scenario-one-compact-complete-loop-v1/spec.md): new §1A records exactly accepted Option A, bounded-site framing and source-labelled 40-year findings; S-12A turns broadleaf establishment into an honest player workflow (not an approved count), S-12/Ecology future blockers, OPEN-C9 cohort design and OPEN-C10 same-start twin-run proposal explicitly pending; keeps OPEN-C1–C8.
2. [`plan.md`](https://github.com/jermur788/CCF/blob/docs/scen1-planting-led-compact-draft-20261114-v1/specs/scenario-one-compact-complete-loop-v1/plan.md): a limited next proof workflow, source-admitted mortality/deadwood and visual crown issues; #490 evidence not a new 30m source acceptance.
3. [`tasks.md`](https://github.com/jermur788/CCF/blob/docs/scen1-planting-led-compact-draft-20261114-v1/specs/scenario-one-compact-complete-loop-v1/tasks.md): D-05 proposed Option A biological/teaching workstream, C0-05..08 docs/review/decision/conditional future witness and risk stops; no real source claim/lease started.
4. [`checklists/requirements.md`](https://github.com/jermur788/CCF/blob/docs/scen1-planting-led-compact-draft-20261114-v1/specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md): R-049 finite-edge logic corrected; added **R-105 through R-115**, all unchecked, covering Option A, NO guaranteed natural broadleaf recruit, actual source-eligible planting/protection, cohort choice, mortality/deadwood/crown visibility, authentic annual provenance, no-work options, bounded same-start proposal, paused canonical #471 and old-save constraints. **All R-001–R-115 unique and unchecked; previous R-001–R-104 retained.**
5. This **NEW `scen1` Markdown handoff** (path above) with independent reviewer packet.

**Not touched:** actual source C#/Unity `Assets/`, `Packages/`, `ProjectSettings/`, save/Prefs, five-rack tests/Reference, any other spec, main, original #465 worker branch, paused #471 branch, Decision Log, Current Milestone, live coordination or remote Drive.

**Required author exit:** read back five files, diff against base exactly four modified DRAFTs + this one new Markdown, no status promotion, no checked acceptance box; comment [HANDED-OFF] and close #495; notify #424 and #425.

## 4. New independent #496 delta review (READY / NOT RUNNING)

**Purpose:** review ONLY #495 Option A/new research evidence DRAFT delta and its effect on old F1–F3 and future authority; not repeat a 115-check testing suite, redo #490 Unity, or assume product approval.

**Actual reviewer worker:** must be a **different session from #495 GPT-6 author**, register actual model/provider, independent docs-only branch and fresh Git starting HEAD under [#496](https://github.com/jermur788/CCF/issues/496) BEFORE writing. Suggested model Claude Opus High or independent GPT-6 High. No local worktree/Unity/physical machine lease; no live ownership or source overlap.

**Exact reviewer branch** (only if absent and clean): `docs/scen1-496-option-a-planting-delta-review-20261114-v1` from freshly verified main. **Only writable file:**
`Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-495-PLANTING-LED-OPTION-A-DRAFT-DELTA-REVIEW-20261114.md`.

**Read-only inputs:** direct owner Option A text as accurately quoted above, this #495 handoff; base #465 four docs and [#467 independent fix review](https://github.com/jermur788/CCF/blob/docs/compact-one-rack-465-independent-fixcheck-20261113-v1/Docs/Handoffs/ScenarioOne-Compact-Pivot-20261113/INDEPENDENT-465-THREE-FINDINGS-FIXCHECK-20261113.md); #484 and #490 original GitHub research/handoffs, [#490 Manager review](https://github.com/jermur788/CCF/blob/docs/scen1-manager-490-long-horizon-verdict-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-490-LONG-HORIZON-RESEARCH-VERDICT-AND-BROADLEAF-ESTABLISHMENT-DECISION-20261114.md); current main `ForestTree.ApplyMortality`, model density route, existing D-059/60/65/71/73–77/81; paused D-082 issue #471 and save-router #436.

**Reviewer checklist, issue explicit APPROVED / NEEDS CHANGES / REJECTED:**
1. Is Option A recorded as **accepted planting/protection**, while no natural Oak/Beech cohort by elapsed Year40 is a permitted outcome rather than a source guarantee? Does the spec retain growth/regeneration mechanics and D-059 parity?
2. Does any line inadvertently **approve 30m, 40-year completion, guaranteed planted survival, arbitrary new broadleaf planting count, external hedge donors, financed work, twin-run saves/ideal guided marks or forced Fell events**?
3. Are #484/#490 claims correctly tiered: exact year0 native/early replay, one Oak/Beech, 40 real yearly ticks, 142 worker-reported checks, narrow crowns, heavy Sitka self-thinning, *no measured individual death causes or deadwood render*, no Bramble analysis, no realistic natural-broadleaf seedlings, one defective image, old hashes not run?
4. Are S-12A, §1A, plan, D-05/C0-05..08, R-049 and **R-105..115** internally consistent and non-conflicting with old source semantics, D-077 Year25/year100 pause, currently unimplemented skip, accepted staging of three species and source learning history?
5. Verify exact four modified DRAFT files + one added handoff, all **115 unchecked**, no changes to main/canonical/coordination/production or old original.
6. Provide a sharply bounded next step: approve DRAFT quality and send to #424/owner-cohort planning and #425 canonical reconciliation, OR a finite actionable NEEDS CHANGES set. Do not approve implementation, create a new Unity worktree, or close D-082 without #425.

**Result path:** the `scen1-INDEPENDENT-495-PLANTING-LED-OPTION-A-DRAFT-DELTA-REVIEW-20261114.md` new Markdown above. At handoff read back and compare **only that one new path** with registered branch baseline, link direct published evidence and notify Manager #424. Mark #496 [HANDED-OFF]/CLOSED. Dashboard/Drive not verified.

## 5. What happens after #496

If **APPROVED as DRAFT quality**: #424 can prepare an *owner-facing* small-cohort/site/decision-clarity design using #490 evidence without promising planting success; #425 (not a worker on this branch) reconciles proposed D-082 first-demo shift and new accepted planting-led boundary in canonical Decision Log/Milestone with its independent #471 gate and project context synchronization. Then the *owner/save decision gates* OPEN-C1–C10 and #436 one-router spec remain; source+Unity launch needs a formally APPROVED four-file spec and distinct claims. A new native cohort experiment is only justified by exact remaining uncertainty and licensed machine slot; do not automatically rerun Year40.

If **NEEDS CHANGES**: one author-only bounded docs repair, then short nonauthor delta check, not a re-run of #460/#484/#490 nor 115 generic tests.

**User action for this author work:** none. To start #496 separate reviewer, user can send the paste-ready message below. **No Windows build.**

## 6. Paste-ready separate reviewer dispatch

> Take ownership of [CCF #496](https://github.com/jermur788/CCF/issues/496) as a **non-author independent four-file DRAFT delta reviewer**. Follow the full binding **scen1** handoff: https://github.com/jermur788/CCF/blob/docs/scen1-planting-led-compact-draft-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-495-OPTION-A-DRAFT-REVISION-AND-INDEPENDENT-DELTA-REVIEW-PACKET-20261114.md. Register actual model/provider, clean separate reviewer branch and starting HEAD on #496 before writing. Compare the four revised `specs/scenario-one-compact-complete-loop-v1/` DRAFT files to independently reviewed #465 baseline and user-approved **Option A** (plant/protect/manage Oak + Beech; no required natural broadleaf recruits by Year40), and verify faithful #484 finite-edge + #490 real Year40 research provenance, 11 new unchecked R-105..R-115, preserved R-001..104, old five-rack/protected saves, and all other OPEN-C decisions unresolved. This is a **focused documentation-quality review**, not implementation, Unity, ecological requalification or canonical D-082 approval. Publish only `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-495-PLANTING-LED-OPTION-A-DRAFT-DELTA-REVIEW-20261114.md` on disjoint docs branch. Issue APPROVED / NEEDS CHANGES / REJECTED with exact evidence, GitHub readback/one-file diff, close #496 HANDED-OFF, notify Manager #424. No game-source/save/Unity/Decision Log/current milestone/active coordination edits or Windows build.
