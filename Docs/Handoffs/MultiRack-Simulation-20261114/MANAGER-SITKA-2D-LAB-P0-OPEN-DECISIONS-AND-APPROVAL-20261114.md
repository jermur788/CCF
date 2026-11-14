# Formal Overall Manager P0 decision and implementation gate — standalone synthetic Sitka 2D laboratory

**Date:** 14 November 2026.  
**Management task:** [#489](https://github.com/jermur788/CCF/issues/489).  
**Next single implementation claim:** [#491 READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/491).  
**Product authority:** designated Overall Development Manager under `Docs/Project/AgentWorkflow.md`, subordinate to user, `Docs/Project/decision-log.md`, `current-milestone.md` and `game-brief.md`. This document is a **manager decision** within the user's Sitka-first/offline incremental research direction, not a claim that the user directly spoke all engineering choices. The original #483 DRAFT author did not self-approve; a separate independent #485/#488 reviewer assessed the actual four-file DRAFT.

## 1. Inspected independent review and approval chain

1. Original standalone laboratory four-file DRAFT [#483](https://github.com/jermur788/CCF/issues/483), then independent #485 NEEDS CHANGES (eight MEDIUM and ten LOW document findings).
2. #486 corrected the draft on the separate docs branch at `e085ede824872a6a57abf5856be09ffc41e6ce60`, **exactly four amended spec files** against the original author candidate and no other changed files.
3. The [#488 independent Opus 5.5 delta review](https://github.com/jermur788/CCF/blob/fc7384226843aee3abb9314de0a0eb30cd64576e/Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-485-FINDINGS-DELTA-RECHECK-20261114.md) returned **APPROVED AS REVISED DRAFT QUALITY**. All 18 original findings PASS; engineering viability, mathematics and formal structure PASS. Seven N-01..N-07 recommendations LOW / non-blocking, explicitly not requiring further independent review. #488 itself does **not** approve P0 implementation; branch inspection verified the review was exactly one additional Markdown file vs main. No local engine/test/Unity evidence is implied by this documentary review.
4. Manager separately read the four actual revised files, AgentWorkflow, current decision log, milestone, Game Brief, live coordination board and review before deciding P0; used accepted project-level authority to resolve the three mandatory product/architecture gates, not the reviewer's verdict alone.

## 2. Formal product decisions — L1 through L5

| Decision | Final manager disposition | Scope effect |
|---|---|---|
| **L1 RESOLVED** | **Standalone, research-only local 2D engineering tool**, NOT a distributable 2D game mode | No new player scenario, game product, public host, cross-game save or milestone |
| **L2 RESOLVED** | **P0 NO BIOLOGICAL GROWTH**; calendar-only steps with ages/DBH/heights/mortality constant, actual thinning only on exact authorised work | Ecological annual dynamics are separate P1 pending model/source/evidence spec approval |
| **L3 RESOLVED** | **Independent .NET C# headless Core + 127.0.0.1-only local browser UI**; one writer and common source state | .NET runtime version pinned by local worker before fixture generation; no Unity/shared game runtime, internet or telemetry |
| **L4 DEFERRED, NOT P0** | Real mapped/verified Irish marteloscope data, IDs, field rights/licence | #479 raw access remains blocked; no substitute fictional “real” source |
| **L5 DEFERRED, NOT P0** | Irish physiological calibration, individual-tree growth predictive truth and validation | GROWFOR and Broad/Lynch are later stand-level bounded references, not per-tree spatial truth |

These conservative decisions follow the documented *user* Sitka-first, flat test-first direction but are labelled **Overall Manager decisions**, not new direct owner statements. User can supersede them by a newer direct product instruction.

## 3. Formal spec decision — P0 ONLY APPROVED

**Verdict: APPROVED — P0 standalone synthetic lab only.** The four-file canonical feature spec under `specs/multirack-sitka-2d-reference-lab-v1/` on this branch has been updated from DRAFT to **APPROVED for P0**:

- `spec.md`: bounded formal authority, L1/L2/L3 resolution, L4/L5 deferred, source-preserving reviewed scope;
- `plan.md`: G0 recorded as manager approval, sequential G1–G6 P0 gates;
- `tasks.md`: LAB-00 documentary gate complete; LAB-01..LAB-05 authorised to be **claimed** only after actual physical/source ownership preflight; LAB-06/07 P1 and LAB-08 remain unapproved;
- `checklists/requirements.md`: management-only P01/P06 checkboxes recorded as decisions, every actual **implementation acceptance check remains UNVERIFIED**.

The seven reviewer N-01…N-07 cleanup suggestions are folded in without new feature scope: (1) exactly one event for every snapshot-changing action, event hash chaining including audit-only no-change rejection and per-plan load staleness; (2) opaque newly minted runtime `sessionEpoch` distinct from persisted `approvalSessionEpoch`, replay deriving it from approval event, unconditionally stale loaded approvals; (3) path/symlink rejects **before opening** versus wrong-format/CCF-shaped content rejects **after reading header and before forest/event parsing**; (4) exclusively checked-in synthetic fixtures and **calendar-only experiment steps** vocabulary, no Irish prediction implied; (5) integer-mm polygon, explicit `LIVING/DEAD/FELLED` and approvalSessionEpoch spelling; (6) nonempty ASCII `^[A-Za-z0-9_-]+$` identifier contract; (7) optional negative **no P0 top-height** acceptance check and clarified load/reset-of-any-fixture case. Original #488 review explicitly states none of these wording/naming corrections needs another full review.

**Scientific/engineering boundary:** P0 verifies tree identity, polygon geometry, stand arithmetic, deterministic event/snapshot/replay, action authority and 2D representation **against synthetic engineering fixtures only**. No P0 growth, natural recruitment, real marteloscope import, ecological predictive validation, 3D, five-rack extrapolation, Oak/Beech, game production state, user save, Windows Player build or Scenario One scope reduction. The complete three-species Scenario One reference demo remains first external demo.

**Status distinctions:** SPEC APPROVED (P0 task-level authority only); **IMPLEMENTED=NO, REVIEWED CODE=NO, INTEGRATED=NO, VERIFIED=NO, USER ACCEPTED=NO**. Approval doesn't grant a physical Unity or host lease.

## 4. Next executable P0 implementation route

**Execution claim:** [#491](https://github.com/jermur788/CCF/issues/491) **READY / NOT RUNNING**, deliberately **not dispatched as RUNNING by this manager session**.  
**Difficulty:** MEDIUM–HIGH: precise .NET C# arithmetic and polygon, canonical JSON SHA-256/events, fail-closed local save/HTTP boundary, real browser UI.  
**Best-suited worker:** one Sol/Codex High T3 Code local C#/.NET + browser-capable agent, or equally equipped local implementer; *actual* provider must register. No simultaneous Core/Host/UI writers; independent non-author reviewer only after a handoff.  
**Approved source/spec branch:** `spec/multirack-sitka-2d-lab-p0-approved-20261114-v1`, containing the exact four approved spec artifacts. Worker must verify its current HEAD against the approved handoff and treat later drift as a STOP; no silent switch to DRAFT branch/main.  
**Prospective code branch:** `feature/multirack-sitka-2d-lab-p0-20261114-v1`, **only after** confirming branch name is unused and code branch created from the exact approved-spec branch.  
**Prospective physical worktree (NOT checked or created by this GitHub-only session):** `/media/jer/Files/CCF/Worktrees/CCF-Sitka2DLabP0-20261114`. Test mount, disk capacity, ownership, dirty status, CCF-Ops and concurrent local tasks first. If unavailable/occupied, **STOP** and return to Manager for a new explicit path; do not reuse a user's dirty checkout. **No Unity machine lease is requested or authorised**.  
**Exclusive file set:** new isolated `Tools/MultiRackSitkaLab/**` only, and exactly one new GitHub handoff `Docs/Handoffs/MultiRack-Simulation-20261114/SITKA-2D-LAB-P0-IMPLEMENTATION-AND-QUALIFICATION-HANDOFF-20261114.md` on that source branch. No new source writing elsewhere, no Unity `Assets/`, `Packages/`, `ProjectSettings/`, existing code, save/PlayerPrefs, other specs/canonical context or live coordination by the worker. No external packages without approved need.
**Live ownership:** `Docs/Project/coordination/active-tasks.md` is read live; #424 is sole board manager writer. Worker must obtain a prospective non-overlapping ownership/board entry from authorised manager **before** local source changes, separately from #491 issue claim. If #424 is unavailable and a real block exists, return for a specific explicit user/manager delegation, not silent single-writer override. Worker registers on #491 [RUNNING] with actual model/provider, full starting HEAD, branch, worktree and observed physical safety before editing. No Windows build.

### Implementation gates — cheapest proportionate testing

- **G1 preflight:** branch, worktree, disk, current .NET/Node/browser versions; no production/Unity process or dirty-work collision; register real claim and ownership.
- **G2 Core:** checked-in immutable synthetic Sitka fixture(s) without P0 PRNG; canonical integer-mm records, IDs/polygon and adult DBH threshold; hand-calculated BA/N/Dq; boundary-inclusive, bow-tie/outside/overflow negatives; no top height.
- **G3 event/authority:** explicit marking/plan states, one open-plan membership, no cutting before execution, exact atomic felling only after approve→queue→execute, reject-all if one target invalid; staleness on revision/epoch/load; every snapshot mutation logged and SHA-256 event replay matches canonical snapshot; lab-only `.lab-data/` save and fail-closed CCF-format/header/path.
- **G4 2D host/UI:** bind only `127.0.0.1`, restricted Host/Origin/CORS, no network/CDN/telemetry; actual source-authored tree-map view, immutable IDs, selected-tree inspector, proposal vs actual Core-only metrics, visible distinct approval/queue/execution and labelled no-growth status.
- **G5 focused qualification:** real headless tests, actual 2D browser click/edit/approve/queue/execute/save/reload/replay, status/metric parity, negative guards, one **2,000-tree** performance fixture selection/preview **≤2 seconds on recorded host/browser**, report any failures and independent reviewer re-check. **No Unity Tier3/Windows build**.
- **G6 handoff:** one GitHub Markdown result with exact source branch/HEAD/changed paths, commands/exits/screenshots/results, limits and independent review request. Actual HANDed-off claim closure; then Manager verdict. No automatic game integration.

### Copy/paste worker dispatch

> Take ownership of [CCF P0 Sitka 2D research lab task #491](https://github.com/jermur788/CCF/issues/491). Follow the exact **formally APPROVED P0-only four-file specification** and complete binding manager handoff: https://github.com/jermur788/CCF/blob/spec/multirack-sitka-2d-lab-p0-approved-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-2D-LAB-P0-OPEN-DECISIONS-AND-APPROVAL-20261114.md. Verify approved spec branch/HEAD and live coordination/physical worktree ownership **before edits**, register your actual model/provider/branch/worktree/starting HEAD on #491 as RUNNING, and use one writer for new `Tools/MultiRackSitkaLab/**` only. Implement LAB-01..LAB-05 **research-only**: immutable synthetic Sitka fixtures, deterministic .NET Core polygon/BA/Dq, separate marks/approved plan/atomic work, SHA-256 event replay, isolated lab-only save, local 127.0.0.1 browser UI and real Node/.NET/browser qualification. **P0 has NO BIOLOGICAL GROWTH**, no real Irish data, no Unity game/3D/rack extrapolation. Publish the exact result handoff and close #491 HANDED-OFF; do not self-approve implementation. Stop if local lane/disk/board/host is unsafe or unavailable, rather than touching user worktrees or weakening tests. No Windows Player build.

**Owner action:** None for formal manager approval and publication. To actually start P0 coding, paste the worker dispatch in a new local T3 Code/Sol High chat with a real C#/.NET/browser environment; issue creation alone is not an executing worker.  
**Dashboards/Drive:** NOT VERIFIED. No main/canonical Decision Log modification or mirror refresh is claimed; this is an accepted feature spec on its explicit GitHub source branch.
