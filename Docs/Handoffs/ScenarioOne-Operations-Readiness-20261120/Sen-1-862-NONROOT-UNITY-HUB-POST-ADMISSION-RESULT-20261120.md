# Scenario One #862 — post-amendment admission and launch result

**Verdict: HELD AT PHYSICAL ADMISSION. Admission NOT PERFORMED; mode A NOT RUN; mode B NOT RUN.** The filename follows the manager's requested output; it does not imply that admission occurred. Observations through **2026-11-20 12:04:54 UTC**, jer-laptop.

## Assignment and continuing actor

Read the complete [#865 binding amendment](https://github.com/jermur788/CCF/blob/26cb53a10c1d835f9b13a1de450c75b1cec06208/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Sen-1-865-862-MANAGER-FIXTURE-LANE-ASSIGNMENT-AND-NARROW-ADMISSION-AMENDMENT-20261120.md), blob `18ee519d6199d15c23de1a62b2d5547c08b1453f`, alongside the original packet and [earlier native preflight](https://github.com/jermur788/CCF/blob/21c1c641a1699a9c6e0c668b0935e81138d4ca4e/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Scenario-One-NonRoot-Hub-Native-Preflight-Result-20261120.md).

- Actual continuing executor: **T3 Code / Codex gpt-6.1-sol, high reasoning**, configured Codex provider; ordinary real/effective UID **1000** on **jer-laptop**.
- Same T3 thread: `48af0d29-298b-45e5-b2a6-2d5a551e24f7`; local T3 environment `ac150346-a782-45eb-8cae-4e08fe9bab79`. Continuity confirmed by current-thread identity returned from T3. No successor or new child was launched.
- Current main fetched/read: `b4ce4d27309d70c1a366197cf2c69b8e849e346a`; its leading coordination entry confirms #865 succession and the conditional #862 assignment.
- Assigned test source branch: `test/sen1-862-nonroot-hub-launch-20261120-v1`, directly verified remote HEAD **3d8fbd698cd5b9290d24e4cbb3cf14849a27c2b6**.
- Chosen lane: **Sen1-862-NonRootHubSmoke**; chosen worktree basename **CCF-Sen1-862-NonRootHubSmoke-20261120** on the designated ZX20 Unity volume; selected ignored project **Build/NonRootHubSmoke** within that worktree.
- Intended Editor: **6000.6.0f1**; new empty built-in/Core 3D equivalent with disjoint scratch company/product. No existing CCF project or save is a substitute.
- Task class PACKET ONLY; Tier 1. Sole new tracked output is this Markdown on `docs/scenario-one-hub-smoke-post-amendment-result-20261120-v1`, based on refreshed main above.

**The previous missing fixture/lane selection is resolved by #865. The remaining issue is actual physical registration and validated admission.**

## Fresh direct checks

| Check | Actual result |
|---|---|
| Host / real UID / effective UID | jer-laptop / 1000 / 1000 |
| Deployed Unity lock | FREE; metadata null; Editor PIDs [] |
| ZX20 storage identity/free-space gate | PASS, writable; approximately 289.015 GiB free, 15 GiB minimum |
| Chosen lane entry in deployed lanes.json | Absent; zero matches by lane ID or exact assigned path |
| Chosen worktree / scratch project | Both absent; neither created |
| Deployed schema support | Inspected schema 1 parser supports a named `unity:true` lane with absolute path and purpose, rejects duplicate name/path, verifies designated storage; no script change needed merely to represent the selected lane |
| Physical board validator | **FAIL**, exit 1: `FAIL: invalid board state or duplicate ID` |
| Bounded read-only diagnosis of validator failure | Existing **CCF-857-BLANK-STARTER** row has state `O0 RUNNING / SOURCE+UNITY HELD`, outside the deployed accepted state set; no duplicate task IDs found |
| Changes to deployed configuration/board/scripts | **None** |

The board failure is pre-existing: its SHA-256 is identical to the earlier native preflight. The #862 amendment allows only the new lane and corresponding #862 board row; it does **not** authorise the test worker to rewrite #857's row or waive validation.

## Actual registrar request and transport failure

Inspected the earlier physical-custody session's original owner assignment and final receipt, confirming it had performed controlled board/lane transactions under the board-directory flock. Its historical task was bounded, so no standing general custody was assumed.

Sent **one** request to that existing session, asking it to independently confirm actual custody rights before any write, read the complete #865 amendment, register its identity, and perform only the permitted #862 admission with backups/hashes, candidate validation and readback. It was instructed to return HELD if authority/procedure was missing. The executor did not grant itself registrar authority.

T3 rejected the request **before starting a run**:

> Provider instance "claudeAgent_claude_b_pro" is disabled for project b42e83a0-a78c-4420-ba40-18e8eee6709b.

No new run/task ID was returned; no registrar acted and no cancellation was needed. No replacement provider, model or newly invented custodian was substituted. This transport failure does not establish that every possible physical custodian is unavailable; it establishes that this identified route could not obtain the required consent/registration. The owner was asked to identify an available authorised custodian.

Amendment §2 explicitly requires: **“the physical custodian must genuinely consent/confirm and independently verify real rights. If custodian is not available, do not fake its approval.”** The worker therefore stopped before lane mutation, worktree creation, fixture creation or Editor launch.

## Launch, licence and lease evidence

| Acceptance item | Result |
|---|---|
| Physical registrar identity/consent for new transaction | **NOT OBTAINED** |
| Single lane addition / corresponding board update | **NOT PERFORMED** |
| Registration backup / after-hash / validator pass | N/A; no transaction attempted |
| Empty Hub project created with disjoint identity | NOT RUN |
| Mode A: two non-root authenticated Hub Editor starts | **NOT RUN — admission held** |
| Mode B: genuine authorised same-desktop Hub GUI fallback | **NOT RUN — admission held; not reached** |
| Actual Editor EUID, valid licence, project open and no warning | NOT VERIFIED in this resumed turn |
| New scratch/project/cache root-owned-file comparison | NOT TESTED; no fixture exists |
| Lease acquired / normally released | None / N/A; live lock remained free |
| Two-launch PASS or cause of earlier root warnings | **NOT ESTABLISHED** |

Earlier UID1000 Hub/sandbox observations remain historical preflight evidence, not a substitute for two actual Editor starts. No security warning was acknowledged; no licence credentials or XML were accessed for this resumed check.

## Preservation and next bounded action

Deployed pre-state/readback SHA-256:

- `lanes.json`: **0047ef120742ef15d64221cc8ca0f2e166e982f4bee6ac8af2f3572042fa9c22**
- `board/active.md`: **8eeb3a87919be41b351bca6ec612630700b53cba7dfcfd91288918ed71651d12**

Both match the earlier native preflight. No live Ops file, policy, other lane, lock ownership, CCF tracked source, scene, package, save, Hub installation or system permission changed. The dirty Primary coordination documents and untracked operations notes/archive remain preserved. No Editor, worktree, game build, source merge or Windows build was created.

**Next:** Manager #865 / physical owner identifies an available authorised registrar. That registrar must confirm rights and handle the existing unrelated validator failure under its own appropriate authority, or provide a validated admission procedure that does not waive it; no #857 row change is authorised by this test. Then register only #862's exact lane/matching row with pre-state backups, hashes, official validation and independent readback. Once that evidence exists, the same named test worker can create its assigned clean worktree/isolated Hub fixture, refresh live lock/PIDs, acquire a normal exclusive lease, perform the two launches and release normally. A fresh permission to change gameplay or a new product choice is unnecessary.

**Dashboard sync: NOT VERIFIED; published dashboard reflects this task: UNKNOWN.** Notify #862/#861/#865/#425 and historical #844 with the immutable report. #865/#425 must ingest the precise admission hold and verify the published audit; no generated dashboard state, Drive mirror or canonical coordination text was edited here.
