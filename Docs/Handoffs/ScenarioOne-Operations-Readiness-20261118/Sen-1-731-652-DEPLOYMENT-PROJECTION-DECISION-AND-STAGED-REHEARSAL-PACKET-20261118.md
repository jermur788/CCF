# Sen-1 #731 — #652 manager decisions after D-091 read-only live deployment preflight

**MANAGER VERDICT: APPROVED READ-ONLY PREFLIGHT EVIDENCE; DEPLOYMENT READINESS NEEDS CHANGES.**
**APPROVED TO PREPARE STAGED FILE PROJECTION AND EXACT ISOLATED REHEARSAL ONLY, SUBJECT TO REAL LANE/CUSTODY/SCRATCH AUTHORITY.**
**LIVE ZX20 CCF-Ops DEPLOYMENT HELD. UNITY HELD. NO WINDOWS.**

2026-11-18, Scenario One Overall Manager #731, OpenAI GPT-6/GitHub browser. This is a continuation of **the existing #652 workstream**, not a new Ops implementation lane, a new source author or a live installation permit.

## 1. Evidence and finding dispositions

Inspected [actual distinct-custodian preflight result](https://github.com/jermur788/CCF/blob/8506db60b4ecf4f42d212c4c0a2bd749cf622571/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-652-D091-LIVE-DEPLOYMENT-PREFLIGHT-AND-CANDIDATE-PROJECTION-RESULT-20261118.md), registered at [#652](https://github.com/jermur788/CCF/issues/652#issuecomment-6979268813) and summarised in [receipt](https://github.com/jermur788/CCF/issues/652#issuecomment-6979764020). Actual preflight custodian Anthropic Claude Opus 5.5 in separate T3 Code session; no source authoring, source review or live Ops change was performed by that session.

**APPROVED as documented read-only inspection, not an installer permit.** ZX20 persistent UUID/fstype/mount/space PASS, current live `verify-deployment` **28 source/7 overrides/1 config/5 state PASS**, original board lint **42 rows PASS**, all 50 installed Ops files byte-identical during inspection, no Unity PID/lock, no flock at observed time. These observations expire as live-lock proof and MUST be repeated at actual installer admission; this browser Manager has not executed laptop checks.

**Board/lane drift attribution accepted:** #734 at 12:41 released Multi #732, #735 at 12:52 changed Multi #725 retention; both distinct recorded owner-authorised custody actions, not an unexplained Scenario One source mutation. #652 row remained byte-identical. Preserve Multi Manager #730 authority: no reverting their board/lanes updates. Physical #652 row's stale SOURCE BLOCKED / prospective wording must be reconciled by the authorised board custodian, under the shared flock, before any installer start. Preflight worker held no lease.

**Source remains accepted without code edits:** exact D-091 `unity-lock`, tests, README, operator guide were independently APPROVED, source full suite **233/233 PASS** (author twice across Files/ZX20, reviewer independently twice). D-091's intended `--no-expiry`, explicit null-mode marker and old-record fail-closed behavior remain intact. No additional runtime source or tests are authorised by this packet.

## 2. Manager file-selection decisions — exact staging plan

The installer shall stage and rehearse the following projection, **NOT copy it live** until a later reviewed actor-specific deployment permit. Preserve source head and per-file SHA-256 mapping in the preflight report; a future installer must rehash exact bytes.

**COPY/REPLACE under a future separately granted live permit, five files ONLY:**
1. `CCF-Ops/scripts/unity-lock` — reviewed D-091 (candidate SHA-256 `8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8`).
2. `CCF-Ops/tests/self-test.py` — reviewed D-091 full suite (`1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50`).
3. `CCF-Ops/roles/unity-operator.md` — reviewed D-091 instructions (`e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db`).
4. `CCF-Ops/scripts/_ccf.py` — previously independently accepted **Option A** bounded default-reader semantics required by the D-091 suite (`24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435`), not live original. Verify actual stage imports and the original security negatives; do not simplify it.
5. `CCF-Ops/README.md` — **NEW MERGED DEPLOYMENT-ONLY DOCUMENT, NOT the currently reviewed D-091 source README bytes**. Take the live README as the preservation baseline; retain accurate persistent disk/by-id, duplicate visible UUID rejection, live durable verification folder and all other true installed-only statements. Add only the source-reviewed D-091 `--no-expiry`/explicit release/legacy migration/Editor PID/rollback instructions and correction of the stale legacy Git-store path. Do **not** resurrect retired `CCF-Verification/WorkflowFoundation` or retired ZX20 Builds/Verification paths. Explicitly clarify that source-only `DeploymentProvenance.md` is not installed in this projection and point to the real `deployment.json` as installed provenance. This document's new exact bytes **require focused independent delta review**, though no runtime code change is needed.

**RETAIN LIVE AS-IS / do not copy:**
- `CCF-Ops/environment.sh` (P-2 = NO COPY). The preflight finds the candidate source has a bounded durable-root check, but neither D-091 null-mode nor its required runtime imports depend on deploying this unrelated Option A enhancement. Avoid another semantic projection and test surface. Preserve the currently verified live environment and document this deliberate installed-only override. If exact staging tests reveal a genuine incompatibility, STOP and seek a revised projection with evidence rather than silently copying it.
- `CCF-Ops/AGENTS.md`, including live storage policy override; no older source replacement.
- `CCF-Ops/scripts/verify-deployment` (optional source-only verifier, P-2 = DO NOT INSTALL). Use the existing verified source command for staging and receipt audits, without putting a new executable on live Ops.
- `CCF-Ops/DeploymentProvenance.md` (optional source-only document, P-2 = DO NOT INSTALL). The merged live README must not falsely promise this file exists in installed Ops.
- All unchanged deployed scripts, roles and docs not explicitly in the five-path list.

**NEVER COPY/REPLACE live authority or state:** `lanes.json`, `board/active.md`, any ledger, `state/unity/{lock.json,guard,history.jsonl}`, any original gate script outside the selected set, caches, other worker files or historical worlds. Preserve current #734/#735 changes and the #690 source owner. No new mode changes on exFAT; validate bytes/owner readback.

**REGENERATE SEPARATELY, ONLY under a future installer permit:** `CCF-Ops/deployment.json` is a **sixth mutable path as an installation receipt**, NOT a sixth source code copy. The existing source manifest is STALE and was correctly rejected by the original verifier against D-091 source; NEVER copy it directly. Prepare a reviewed **prospective receipt template** in isolated scratch that maps each of the five chosen projected bytes (including the NEW merged README) and installed-only overrides, current source version, provenance and actual config. Preserve genuine old historical `deployed_at`/source provenance; put **actual** install timestamp only at successful later installation, not now. Validate with the original source `verify-deployment` against the exact staged candidate. Require **focused independent nonauthor review of the prospective manifest schema/content**, and repeat the exact projection checks after real physical board/lane state refresh.

**Unchanged source hashes** must remain pinned exactly to the approved runtime source. Any change to `unity-lock`, `_ccf.py` or its interfaces beyond this decision invalidates its approval and needs full appropriate requalification/different-worker review.

## 3. Exact permitted next activity: isolated artifact staging + rollback rehearsal, no live Ops write

**Task:** prepare merged README, future receipt template, exact selected projection, real-shaped staged tree and bounded tests/rehearsals **entirely in a newly verified disjoint authorised scratch/evidence folder**, never inside `/media/jer/ZX20/CCF-Ops` or the already-protected source worktree without an assigned source writer. Reuse existing #652 task and canonical source ownership; no competing source implementation. Proposed documents branch `docs/sen-1-652-d091-stage-rehearsal-20261118-v1`, created from freshly verified main, with one NEW result:
`Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-652-D091-EXACT-FIVE-FILE-STAGING-RECEIPT-AND-ROLLBACK-REHEARSAL-RESULT-20261118.md`.
Staged proposed merged README and prospective receipt may be published as **new docs-only audit attachments** under the same handoff folder if clearly marked NOT DEPLOYED and are independently reviewed; no publication of private user files or live content secrets.

**Best suited:** native Linux Ops engineer Sol/Codex, **HIGH** difficulty (staged provenance/atomic deployment rollback), either same prior #652 source actor if still legitimately owns its released/retained source scope or distinct pre-authorised integration operator without overlapping source modifications. The #652 source author must not also act as independent reviewer of their staging output. Before stage/rehearsal work, actual worker must register model/provider/session/host, exact starting source and docs branch, a fresh read of main coordination and physical owners, scratch custody and nonoverlap on #652; source code remains read-only.

**Required staged procedures:**
- Rehash all staged source/live inputs, verify disk identity and actual no other source writer. Recreate the exact five-file projection from pinned qualified source with merged README, preserving live overrides. Prove per-path old/live hashes and new/staged hashes, no unlisted change.
- Under **isolated** state/board/lanes replicas only (not live), test original validator, `verify-deployment` with newly prepared receipt (should PASS rather than source stale receipt FAIL), and appropriate full original self-test **233 PASS** if the staging method can run safely within separately authorised scratch. No backdating actual install time or claiming live deployment.
- Fresh backup of original 50-file sample bytes/modes, controlled atomic copy of exactly five stage files and staged receipt into isolated replica; injected partial failure after at least three file replacements, compare-before-write rollback, exact restoration of all pre-state bytes/modes and protected live-state replicas. Use original guard-contention negative in scratch; no lock on live Ops unless separately approved for a true pre-deploy read-only occupancy check.
- Test legacy lock read and marked-null lock reader compatibility **only on synthetic replicated state**, demonstrate actual rollback refusal when a no-expiry lock is active, and safe rollback after explicit synthetic release. Do not delete a live lock or use force. Preserve verified existing operator PID and CLI flags compatibility evidence.
- Staged manifest must not present a future timestamp as already deployed. If verifier requires an actual deployment timestamp even in a prospective fixture, use explicit synthetic `TEST_FIXTURE_ONLY` and do not publish the fixture as installed provenance.
- Independent different-worker/appropriate model-family reviewer assesses exact merged README, staged receipt schema/selected files, invariants and rollback evidence. Report APPROVED/NEEDS CHANGES/REJECTED. Staging author must correct any blocking issue before later deploy authorisation. No need to rerun independent source code review unless code changes.

**STOP on:** source/output file requiring a sixth runtime script, import incompatibility from retaining live `environment.sh`, failed backup/restore or manifest verifier, changed source hashes, invalid owner/physical scratch grant, unrelated dirty files, existing Unity/installer conflict, failure to preserve all other live owner state. This staging packet itself does not authorise a physical board row change or an Ops deployment.

## 4. Separate live-deployment gate remains

The earlier direct owner **NO LIVE INSTALL** order remains operative. The user's general "get the Unity part ready" is not an exact release of that order, nor is their no-time-limit policy consent to deploy software. Manager must inspect stage+independent review result and actual owner-controlled physical installation permission, current scoped exclusive installer reservation under the real board flock, process and Unity lock freedom, fresh 50-file and config hashes, byte-identical reviewed projection and compatible rollback. Only then publish **[APPROVED TO DEPLOY OPS — EXACT FIVE FILES PLUS RECEIPT]** bound to actual installer/session, followed by physical installed provenance, original validator/preflight and safe null-mode lifecycle readback. No assumption source PASS implies installed PASS.

Only after lawful Ops deployment can #718 Compact30's **separate ZX20** scene-worktree/physical Unity lease and final owner-signed seven-file W §12 grant and independent focused review be considered for source start. D-086–D-091 remain intact, including NO arbitrary worker time cutoff. No Windows until owner personal Unity Editor playtest and separate authorisation.

## 5. Status and return

- **#652:** source APPROVED; read-only installer preflight APPPROVED as evidence but **NEEDS CHANGES for deploy**. Staged five-file projection + merged README + separately generated receipt + exact rehearsal + independent focused review are NEXT; no live installation.
- **#718:** Unity/scene authoring HELD.
- **#690:** separate Claude source; physical state not presumed RUNNING.
- **Multi #730:** distinct manager and custody; #734/#735 lawful drift not reverted.
- Dashboard / Drive sync: NOT VERIFIED.

**Paste-ready handoff to staged installer engineer:**
> Take existing Sen-1 #652 for STAGING AND REHEARSAL ONLY (NO LIVE INSTALL). Read the COMPLETE Manager packet: https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-652-DEPLOYMENT-PROJECTION-DECISION-AND-STAGED-REHEARSAL-PACKET-20261118.md. Register actual model/provider/session/host, source/Docs starting state and independent scratch custody with no overlap. Preserve frozen reviewed D-091 code. Assemble the exact five-file staged projection, merging current LIVE README's valid storage facts with D-091 rules; retain live environment.sh and AGENTS, omit optional verify-deployment and DeploymentProvenance files, and prepare a separately reviewed prospective deployment.json receipt. Rehearse full 50-file backup, atomic selected-copy, failure injection/rollback, manifest verification and safe legacy/null-lock rollback in isolated scratch only, without any live Ops/board/lanes/Unity/source edits. Publish the exact GitHub result and request independent focused nonauthor review; STOP on scope or custody conflict. No Windows.
