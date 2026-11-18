# Sen-1 #741-E — Compact30 Tier S ONLY seven-path W §12 exception: owner-signable instrument

**STATUS: REVISION 2 — REVIEW DRAFT, NOT YET PRESENTABLE FOR SIGNATURE — UNSIGNED — INOPERATIVE.**
**PRESENTATION GATE (binding on every agent): this instrument may NOT be presented to the owner for signature while STOP field F-2 (operative physical Ops route) or F-4 (named human custody owner and event-driven release protocol) is blank. A signature given over an instrument in which F-2 or F-4 is blank is VOID. The final signable revision must record the operative route and the custody/release controls in its own bytes, and be independently reviewed again, before it is presented (§9).**
**This document confers no authority of any kind until the product owner personally signs these exact bytes (§9). Until then it is a proposal only: no source, Unity, Ops, publication or Windows action may rely on it.**

Prepared 2026-11-18 by the Sen-1 #741 subtask-E documentation worker (Anthropic Claude Sonnet 5.5, Claude Code harness; claim [#743](https://github.com/jermur788/CCF/issues/743)). The author did not review this document and may not: a separate reviewer session is required (§2, row R-2). **Revision 2** repairs the findings of the independent nonauthor review (NEEDS CHANGES, commit `b79b6919` on branch `review/sen-1-741-e-compact30-w12-nonauthor-20261118-v1`) against the first revision `5d67e845`; revision 2 itself has **not** been re-reviewed.

This is **one instrument, not a menu.** It contains no alternatives. If the owner does not accept any term, the correct response is **do not sign** and request a new version; no field may be edited by hand and then signed as if reviewed.

---

## 0. What this is, in plain words

The owner has already made the product choices needed to author a small, inert Compact30 test scene (Tier S). What is still missing is the owner's **personal, exact-byte consent** to a *narrow* exception to the frozen [#676 §12](https://github.com/jermur788/CCF/blob/docs/sen-1-676-world-manifest-author-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-676-COMPACT30-CONSTRUCTION-REPRODUCIBILITY-AND-OWNER-SIGNATURE-CANDIDATE-20261117.md) ("W §12") rule, which otherwise requires the whole world to be reviewed and attested **before** any scene bytes exist. This instrument asks for that exception for **exactly seven project paths and nothing else**.

Signing it would **not** start any work. Starting work needs further, separate, actor-specific gates (§5, §8) that other people must complete.

Sources this instrument is built from (all read-only): the Manager's [#731 filled-selections draft](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/Sen-1-731-COMPACT30-TIER-S-W12-OWNER-GRANT-FILLED-SELECTIONS-PENDING-SIGNATURE-20261118.md) (successor of the [#700 sheet](https://github.com/jermur788/CCF/blob/docs/sen-1-700-manager-intake-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-700-719-CORRECTED-COMPACT30-CONTENT-BYTES-AND-D032-OWNER-DECISION-SHEET-20261118.md)), decisions D-086 to D-091 in `Docs/Project/decision-log.md`, and the [#741 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-741-DEEPSEEK-ONE-CYCLE-OPS-DEPLOYMENT-AND-COMPACT-UNITY-READINESS-ORCHESTRATION-20261118.md) §3.E. D-086 to D-091 are **cited, not re-litigated**.

---

## 1. Reconciliation of the A8 endpoint (a defect in the #731 draft, corrected here)

The #731 draft contradicts itself: its signature table (§2) still says A8 is **"NOT SELECTED"**, while its §7 and D-090 record that the owner, at **2026-11-18 11:42 UTC**, agreed to the proposed endpoint. This instrument resolves that as follows, and the A8 row in the §2 table and the exception text in §7 are written to match:

- **A8 is a PROVISIONAL owner selection, with named residual risk.** It is neither "not selected" nor a final attestation.
- **Value:** A8 = (x **−2.10 m**, z **+13.50 m**); candidate terminal **dx/dz = +0.221818181818182**; it follows A7 (−2.71, +10.75) and sits 1.50 m south of the z = +15 m northern boundary. (Arithmetic cross-check by this author only: (−2.10 − −2.71) / (13.50 − 10.75) = 0.61 / 2.75 = 0.2218181818…, and 15 − 13.5 = 1.5. This confirms internal consistency of the quoted numbers; it does not verify the #676 curve itself.)
- **Residual risk (named):** the original A7 minimum bend radius of about **0.625 m** is retained as a known **visual risk**. If native viewing in Unity finds the bend objectionable, the worker must **STOP** and request a **separately reviewed replacement route**; stems and G1 sites are not moved to compensate.
- **Not signed by being selected:** choosing C0 and provisionally accepting A8 does not sign the world manifest, the route bytes, or any final world digest. The `#676` author candidate remains **0 signed / 80 candidate / 8 blocked**, `world_digest = null`.
- **Evidence basis:** the 11:42 UTC owner message is relayed through the Manager draft §7 and decision D-090. This author has not seen the original message itself (see §10).

---

## 2. Settled versus still required (one table)

"SETTLED" means the owner has already made the product choice, per the cited decision. "REQUIRED" means a separate gate that is **not** satisfied by this document, whether or not the owner signs it.

| # | Item | Status | Source / what is still needed |
| --- | --- | --- | --- |
| S-01 | Choice **A**, **Tier S only** as next production direction | **SETTLED** | D-086 (owner "Approved", 06:27:14 UTC). D-086 itself says this selects the approach to a *future* signed exception, not the exception. |
| S-02 | Owner-created first-party Sitka art; internal Unity use | **SETTLED** | D-087 (06:44:56 UTC). Not a public-release grant, not a warranty for third-party packages. |
| S-03 | Public GitHub branch publication of **exactly five** new permanent Tier S files | **SETTLED** | D-088 (06:50:06 UTC). No H/J art, no guard, no tool source, no raw masters. |
| S-04 | **Exactly two** temporary LOCAL-ONLY Editor script/meta files; audited removal | **SETTLED** (principle) | D-089 (07:07:38 UTC). The exact pair was **proposed by the #731 Manager draft** (D-089 left the exact paths to a later allowlist, and the earlier `Tools/Verification/…` location is not used); this instrument carries that pair into §3 for the owner's approval. |
| S-05 | One continuous walkable flat plane; logical H1/path/exclusion/planting masks | **SETTLED** | GQ-08 / D-090 (11:32 UTC). Architecture only; no Tier H art implied. |
| S-06 | Original **C0** route, 0.095 m same-Z maximum deviation ceiling | **SETTLED** | GQ-09 / D-090. Reported deviation 0.094074074074 m. No T1, no 0.190 m cap, no unstated curvature fixes. |
| S-07 | Worn-earth / needle-litter, neutral-daylight look | **SETTLED AS PROVISIONAL** | GQ-01 / D-090. Not final owner-approved bytes (see §4.4). |
| S-08 | A8 (−2.10, +13.50), dx/dz +0.221818181818182 | **SETTLED AS PROVISIONAL, residual risk named** | GQ-10 / D-090 (11:42 UTC); see §1. |
| S-09 | Compact30 scene worktree on **ZX20** | **SETTLED** (location) | D-090. Proposed exact target in §6; it does not exist yet. |
| S-10 | No arbitrary time limit on any worker/worktree | **SETTLED** (policy) | D-091 (11:49 UTC). Installed-Ops support is **UNVERIFIED** (§6.3). |
| S-11 | Tier H, Tier J, guards/juveniles, guard preview, Year0 Oak/Beech | **NOT GRANTED** | D-086/D-088; GQ-11 deferred. |
| R-1 | **Exact personal W §12 supersession consent over frozen #676 §12** | **REQUIRED** | The owner's own signature on these exact bytes (§9). Nothing else substitutes. |
| R-2 | Focused independent **nonauthor** review of this *filled* instrument | **REQUIRED — NOT RUN** | A separate session from this author, and from custodians (§8). |
| R-3 | Registered **non-expiring custody/release** protocol, recorded in the bytes presented to the owner | **REQUIRED — NOT REGISTERED** | §6.3; STOP field F-4. Blank F-4 blocks presentation for signature (§9). |
| R-4 | **Operative physical Ops route** | **REQUIRED — HELD** | #652 live install is held by the owner's earlier no-install order; no alternative identified. STOP field F-2. Blank F-2 blocks presentation for signature (§9); the owner's W §12 signature (R-1) is separate from any owner/Manager permission for #652 live deployment. |
| R-5 | **Named sole source producer / session / worktree** | **REQUIRED — NOT ASSIGNED** | STOP field F-1. |
| R-6 | Reviewed successor manifest M and ecology cells C, pinned by Git ref, path, bytes, SHA-256 | **REQUIRED — NOT PINNED** | STOP field F-5. Existing #676 M/C are candidates only. |
| R-7 | Unmodified installed preflight **READY / exit 0, no relevant warning**, plus physical Unity lease **ELIGIBILITY** verified | **REQUIRED — NOT RUN** | A physical custodian check on the real machine, later. Must precede F-7 (§5 ordered sequence, step 2); the lock itself is not yet held at that point. |
| R-8 | **Single** machine-wide Unity lock, verified process identity, acquired by the **admitted producer** with matching task/owner/ID | **REQUIRED — NOT ACQUIRED** | Comes **after** F-7 (§5 ordered sequence, step 4), never before. Not available from a GitHub-only record. |
| R-9 | Overall Manager's **separate** actor-specific `[APPROVED TO START — CONTENT BYTES ONLY]` | **REQUIRED — NOT ISSUED** | STOP field F-7. Depends on F-1, F-2, F-3a, F-4, F-5, F-6 and on R-7 (preflight READY + lease eligibility), **not** on the lock or on F-3b. |
| R-10a | **Pre-start** producer identity, known input rights/notice obligations and receipt plan | **REQUIRED — NOT YET AVAILABLE** | STOP field F-3a. This is what F-7 may depend on. |
| R-10b | **Post-production** receipts: actual new-byte authorship, third-party notice and custody | **REQUIRED — CANNOT EXIST YET** | STOP field F-3b. Required before any public push or content acceptance; **not** a prerequisite of F-7. |

---

## 3. Exhaustive list of the seven permitted project paths

All seven are **absent** at repository commit `e8e84027faa45d42316cd01bda34ad36393fd069` (checked by this author with `test -e` in a read-only inspection of the pre-created checkout). Each must again be absent in the real writer's starting worktree.

**Five NEW permanent Tier S files — public GitHub *branch* publication permitted by D-088 only after they are actually created and verified:**

| # | Exact path | Permission |
| --- | --- | --- |
| 1 | `Assets/ForestPrototype/Scenes.meta` | Create (new); publishable |
| 2 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` | Create (new); publishable |
| 3 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity.meta` | Create (new); publishable |
| 4 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json` | Create (new); publishable |
| 5 | `Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json.meta` | Create (new); publishable |

**Two TEMPORARY, LOCAL-ONLY files — D-089; never `git add`, commit, push or retain:**

| # | Exact path | Permission |
| --- | --- | --- |
| 6 | `Assets/ForestPrototype/Editor/Compact30SceneAuthoring.cs` | Create temporarily; remove after the audited removal procedure (§4.7) |
| 7 | `Assets/ForestPrototype/Editor/Compact30SceneAuthoring.cs.meta` | Create (by Unity import) temporarily; remove with #6 |

**Nothing else is granted.** In particular:

- The existing `Assets/ForestPrototype/Editor.meta` and **every other existing asset, scene, prefab, material, ScriptableObject, ProjectSettings and package file are READ-ONLY.**
- **No Tier H** (#688 ground/H1/path art), **no Tier J** (#698 guards/juveniles), **no guard or witness asset**, no new material, texture, mesh, shader, runtime script, `.asmdef`, package or build setting.
- **No save file, no gameplay, no runtime profile, no Game State bootstrap, no scene added to build settings, no Windows Player or build.**
- No broad path glob, no folder-level grant, no "generated side effect" allowance. The old proposed `Tools/Verification/Sen1Compact30SceneAuthoring/` location is **not** allowed.
- If Unity writes any tracked or untracked project file outside these seven paths (for example a modified existing `.meta`; the worktree's own untracked `Library/` cache is excluded), that is an **unexpected mutation: STOP** (§4.6).

---

## 4. Content and method terms

### 4.1 Ground and masks (S-05)
One continuous walkable flat ground plane. H1, path, exclusion and ecology/planting/admission masks are **logical** definitions, not physical dividers; there are **no separate path colliders** and the player may leave the path. The path core is **1.20 m**, distance-defined, with a **visual-only verge of no more than 0.20 m**.

### 4.2 Route (S-06, S-08)
Original **C0**: established-source anchors A0–A7, with cubic Hermite x(z) exactly as the #676 candidate, a **0.095 m** same-Z maximum deviation ceiling, and **A8 (−2.10, +13.50) provisional** per §1. The ~0.625 m A7 bend is a **named visual risk** (§1) that triggers STOP and a separately reviewed replacement if native viewing rejects it.

### 4.3 Scene contents (as proposed in #731 / #676; not independently re-verified by this author)
175 Sitka trees (14 omitted from the pre-world roster, not felled); four unpruned prefab families (57 / 50 / 31 / 37); 36 candidate 5 m ecology cells (6 × 6 over 30 × 30 m); four **empty** G1 sites reserved for future player planting of two sessile Oaks and two European Beeches. **No** initial guards, juveniles, felling marks, work, cash, natural-regeneration-success claim or historical rack construction. The scene is visually passive. It does **not** register a runtime profile, save identity, Game State bootstrap, build-settings scene, annual ecology, Work Plan, history or gameplay. Legacy40, Enlarged80, Rack V2/MFM, frozen Reference Future v1 and v1–v24 saves stay unchanged.

### 4.4 Look — PROVISIONAL (S-07)
Natural matte worn-earth / needle-litter path and an approximately 12 × 10 m pre-existing southern clearing; no stumps, engineered road, gate or vehicles; neutral daylight; visually soft edges. Exact colours, shader values, textures, lighting, fades and native appearance are **not final owner-approved bytes**.
- A provisional selection authorises **no new Tier H material, texture or asset** under Tier S.
- Any later **final** look change that changes content bytes **reauthorises those bytes**: it requires **re-authoring AND independent content review before the whole-world signature**.

### 4.5 Rights and publication (D-087, D-088, D-089 — cited only)
- **D-087:** owner-declared original first-party Sitka art; internal Unity use. It is not a blanket third-party licence.
- **D-088:** public GitHub branch publication applies **only** to the five permanent paths. A public branch push publishes the bytes even without a PR or merge.
- **D-089:** the two temporary script/meta files are local-only and never published.
- The actual producer of the new scene/placement bytes must be identified, with known input rights and notice obligations and a receipt plan, **before** source start (STOP field F-3a); the actual creation/dependency/material notices, rights and byte custody are recorded **after** production and **before** any public push or acceptance (STOP field F-3b). Scene data and existing prefab references may depend on third-party URP/material packages: exact notices are inspected, not assumed.

### 4.6 Required worker method and unexpected-mutation STOP
Pin reviewed M/C by Git ref, path, byte count, full SHA-256 and recipe **before the first source operation** (F-5). Capture exact before/after tracked, untracked and relevant ignored inventories; validate source M/C SHA-256, prefab/meta GUIDs and the absence of unexpected inputs. The only prefab families are Pole `6fc4c8b081234c78bb956c7cca208157` (57), Young `c54cc46c845eb788480bcfd416177fc8` (50), Mature `b3a74f27d72ef8c089cf2d51b3f67db6` (31), Benchmark `69a172cf4adc70f1fbcc66e989755893` (37), chosen by the preserved `ForestTreeSpawner.PickBase` FNV-1a stable-ID rule. No random yaw, no silent substitution, no prefab mutation, no source-level bootstrap (`ForestSaveController`, `ScenarioOneManager`, `ForestStartingStand`, `ForestEcologyController`), no construction/timber/work/cash. The placement JSON is derived from M/C and is never a second world authority.
**Any unexpected mutation, a need for a Tier H/J asset, or an unexpected import side effect is STOP.** Do not clean, stash, reset or revert unrelated work; evidence is held safely for disposition, not destroyed.

### 4.7 Temporary-tool removal
After the actual Unity processes have exited, remove **only task-owned** files #6 and #7, with provenance and file hashes; run an authorised **tool-free** Unity reimport/compile; verify no lingering class, source or `.meta`; and produce a five-permanent-file-only index/diff before any permitted public commit. Unity evidence operations are limited to input validation/import, author-and-reopen readback, removal audit, and tool-free import. Any native rendered/graphics session needs its own lawfully admitted phase; this instrument does not imply one.

---

## 5. Conditions this exception does NOT waive

- The W §12 supersession in §7 below covers conditions (a)–(e) for **these seven paths only**. They remain binding for later operative integration, full world acceptance and **everything outside the seven paths**: no worker may expand the exception by necessity or discretion.
- **Not waived (f):** none of the following may be skipped, merged or reordered. Before the temporary script is staged or Unity is opened, **all** of these separate, real facts must exist, **in this order**:
  1. **Signature and presentation:** a revision with F-2 and F-4 recorded in its own bytes was independently reviewed, presented, and personally signed by the owner (R-1; §9). The owner's authorisation of any live #652 installation, if that route is chosen, is separate.
  2. **Physical readiness, lock NOT yet held:** the physical custodian, under the exclusive common board flock, registers one nonconflicting lane and writer; the worktree is created on ZX20 only after real approval; the **unmodified installed preflight reads READY / exit 0 with no relevant warning**; and **Unity lease ELIGIBILITY** is verified.
  3. **Manager source-start (F-7):** the Overall Manager inspects those physical receipts and issues the separate actor/session/branch/HEAD/phase/receipt-specific `[APPROVED TO START — CONTENT BYTES ONLY]` naming the exact producer, session, starting candidate, phase, inputs, evidence and rollback limits. F-7 requires F-1, F-2, F-3a, F-4, F-5, F-6 and step 2 — **not** that the lock is already held, and **not** F-3b.
  4. **Admitted producer acquires the lock:** only then does the admitted producer register `[RUNNING]`, re-run status, and acquire the **single** machine-wide Unity lock with matching task/owner/ID. Any acquisition must meet the installed Ops contract (§6.3); no distant or placeholder date is used.
  5. **Only then** may the two temporary paths (#6, #7) be staged or Unity launched.
  6. **After production, before any public push or content acceptance:** the post-production receipts F-3b are captured, and independent nonauthor content review is done.
- The owner's general authorisation to *use Unity* (D-087) does not authorise #652 live Ops installation and does not bypass any physical safeguard.
- No world digest, signed complete-world attestation, spec approval, user beta, main integration, Windows Player or build consent follows from Tier S alone. The eight blocked fields across four blocker IDs in frozen #676 (the world-content blockers), Tier H/J rights and geometry, successor M/C and complete digest, independent full-world review, the formal 214 Compact-spec checks and the signed save/CG-TRIG bridge each keep their own gates.

---

## 6. Worktree, custody and "no arbitrary expiry"

### 6.1 The one permitted Unity home
The only permitted Unity home for this scene work is a dedicated **ZX20** Compact scene worktree:

`/media/jer/ZX20/CCF-Sen1-718-Compact30-Content-20261117`

with its **own `Library`** (never copied from or shared with another worktree), proposed branch `feature/sen-1-718-compact30-scene-content-v1` and proposed lane `Sen1Compact30Content718`. **This worktree does not exist yet, and it has no physical Ops lane and no Unity lock.** Naming it here neither creates nor reserves it. It must not be placed on the internal disk, nor reuse the #690 (Claude save-A0) or Multi/#730 ownership. It starts from the exact compatible D-073 lineage only after a fresh re-check against latest `main`, staged dependencies and a separate save donor.

### 6.2 No arbitrary expiry (D-091)
No worker or worktree under this exception carries an arbitrary time limit. There is **no expiry timestamp, no placeholder or far-future date, no timer, and no requirement to stop merely because work is slow**. Custody continues until an explicit documented handoff or release, owner cancellation, a genuine safety/conflict/authority stop, or task completion. A stale heartbeat triggers **investigation**, never automatic release, lease reclaim or process killing. Release is event-driven: the worker releases the exact lease only after the correct Editor processes exit, publishes a durable GitHub handoff and receives independent nonauthor content review; there is no automatic merge or gameplay activation.

### 6.3 What is UNVERIFIED and out of this grant's scope
**Dated negative evidence:** the accepted [#733 read-only audit](https://github.com/jermur788/CCF/blob/cfc9ba4f381fdd78639697bc9e7180dec70ff0be/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-733-SOL-READONLY-OPS-NO-EXPIRY-REAL-CONTRACT-RESULT-20261118.md) (2026-11-18, installed copy at `/media/jer/ZX20/CCF-Ops`) found that the installed `unity-lock` requires a nonempty, timezone-aware, **future ETA** for acquisition and treats a missing, null or indefinite value as malformed; its verdict was that operational no-expiry **needs a patch**. That is a record of what was found on that date, not a statement about the deployment at the time of use.

Whether the **installed** Ops board/lease schema can represent indefinite custody **at the time of use** is therefore still **UNVERIFIED** and must be **freshly verified** by the physical custodian at that time; the #733 finding is not carried forward as current fact in either direction. This instrument does not assert that it can, does not authorise any expiry-forging workaround, and does not authorise any change to the Ops installation or to existing leases (including #690's historical advisory expiry of 2026-11-19T00:00Z, which only its actual custodian may reconcile safely). Any amendment is a separate, independently reviewed, owner-authorised Ops procedure.

---

## 7. The exception text the owner is asked to sign

> I, the product owner of CCF, choose **Choice A, Compact30 Tier S ONLY**, solely to create an **inert, isolated Compact30 scene/placement content fixture** on the terms of this instrument. I accept that provisional scene appearance is not a final world signature.
>
> **Exactly seven project paths** are covered: the five permanent new files and two temporary local-only files listed in §3, and nothing else. **Tier H, Tier J, any guard or witness asset, any save, any gameplay or runtime code, and any Windows build are NOT granted.**
>
> For those seven paths and this limited, non-operative phase only, I **supersede frozen #676 W §12 narrowly**: (a) closure of the eight blocked fields across four blocker IDs in frozen #676 (the world-content blockers), (b) independent full-world review and (c) my personal exact-byte complete-world attestation are **deferred until after** the inert Tier S scene bytes and native witnesses can exist; and (d) formal approval of the Compact parent/planting/graphics specifications and (e) the separate signed save/serialization/CG-TRIG bridge do **not serve as prior prerequisites** for these seven paths only. All of (a)–(e) remain binding for everything outside these seven paths and for later operative integration and full world acceptance.
>
> I accept as **provisional**, with the named residual risk in §1: the original C0 route with its 0.095 m ceiling; the A8 endpoint (−2.10, +13.50) with terminal dx/dz +0.221818181818182; and the worn-earth/needle-litter neutral-daylight look. I accept that native viewing may require a STOP, a separately reviewed replacement, or reauthoring and re-review before any whole-world signature.
>
> I sign only a revision in which the operative physical Ops route (F-2) and the named human custody owner and event-driven release protocol (F-4) are **recorded in the bytes I sign**; a signature over an instrument with either blank is void.
>
> I do **not** waive the conditions in §5(f), including their order. I understand that my signature does **not** assign a producer, reserve a worktree or lane, install #652, set or imply any lease, or start any work, and that the physical Ops custodian/installer and the Overall Manager are **separate actors** whose own gates are still required (§8).
>
> I accept no arbitrary expiry on any worker or worktree (D-091), with event-driven release only. I understand that installed-Ops support for indefinite custody is unverified and outside this signature.

---

## 8. Separate actors, separately gated

One actor's grant **never implies** another's, and none of these gates is satisfied by this document.

| Actor | Gate that only this actor can provide | Status |
| --- | --- | --- |
| **Product owner** | Personal W §12 supersession consent and signature on these exact bytes (§9); separate owner authorisation for any live #652 installation | Signature **NOT GIVEN**. #652 live install **HELD** by the owner's earlier no-install order. |
| **Physical Ops custodian / installer** | Its own #652-specific permit; inspection of the real physical Ops contract under the exclusive board flock; lane registration; unmodified preflight READY; Unity lease **eligibility** (it does not itself take the lock for the producer); the smallest safe reviewed amendment for no-expiry custody if needed | **NOT PERMITTED / NOT RUN.** No custodian is assumed to hold the physical board. |
| **Overall Manager** | A separate actor/session/branch/HEAD/phase/receipt-specific `[APPROVED TO START — CONTENT BYTES ONLY]` after independently inspecting the physical receipts (preflight READY + lease eligibility); it precedes lock acquisition | **NOT ISSUED** |
| **Independent reviewer** | Focused nonauthor review of this filled instrument; later nonauthor content review of the produced bytes | **NOT RUN.** Must not be this author, the future producer, or a custodian. |
| **Source producer (future)** | After F-7: registers `[RUNNING]`, re-runs status, and acquires the single Unity lock with matching task/owner/ID; registers real provider/model/session/host and exclusive worktree ownership | **NOT ASSIGNED** (F-1) |

### STOP fields — values genuinely not yet assigned (leave blank; do not fill by assumption)

F-1, F-3a, F-5 and F-6 must be recorded before F-7. **F-2 and F-4 must additionally be recorded in this document's own bytes before it is presented to the owner for signature (§9).** F-3b is a post-production field and is deliberately **not** a prerequisite of F-7.

| Field | Required value | Prerequisite that must exist first | Value |
| --- | --- | --- | --- |
| **F-1** Sole production author / session / worktree | Real provider, model, session id, host, exact worktree path | Manager assignment; a proposal (strong native-Linux worker, e.g. Sol/Codex) is **not** an assignment; worktree not yet created | **STOP — NOT ASSIGNED:** `________________` |
| **F-2** Operative physical Ops route | Either #652 live deployment, or an independently validated alternative that meets the installed mandatory preflight | Separate owner authorisation of #652 live install and a named installer, **or** a validated permitted alternative; installed-Ops no-expiry support inspected (§6.3) | **STOP — NOT OPERATIVE:** `________________` (blocks presentation for signature) |
| **F-3a** PRE-START: producer identity, known input rights/notice obligations and receipt plan | Producer identity (cross-checked with F-1); a list of known inputs (existing first-party Sitka prefab families; any third-party URP/material packages they depend on) with known rights/notice obligations; a plan naming each F-3b receipt, its owner and its location | Manager assignment of the producer; inspection of the exact input dependencies from the pinned inputs (F-5). This is what F-7 may depend on | **STOP — NOT YET AVAILABLE:** `________________` |
| **F-3b** POST-PRODUCTION: actual new-byte authorship, third-party notice and custody receipts | Receipts of new-byte authorship, creation/dependency/material notices and byte custody for the actual produced bytes | The scene/placement bytes must first exist. Required **before any public push or content acceptance**; **not** required before F-7 | **STOP — CANNOT EXIST UNTIL PRODUCTION:** `________________` |
| **F-4** Named human custody owner and event-driven release protocol | Named person accountable for lease/release, and the registered release mechanism | Owner or custodian registration through the proper Ops process; no timer-based protocol | **STOP — NOT REGISTERED:** `________________` (blocks presentation for signature) |
| **F-5** Reviewed successor M/C pin | Git ref, path, byte count, full SHA-256, recipe for manifest M and cells C | Independent review of a successor; #676 M/C are candidates only | **STOP — NOT PINNED:** `________________` |
| **F-6** Identity of the focused independent reviewer | Actual reviewer provider/model/session of §2 R-2 | A different session from this author, producer and custodians | **STOP — NOT RUN:** `________________` |
| **F-7** Manager source-start | Link to the separate `[APPROVED TO START — CONTENT BYTES ONLY]` | F-1, F-2, F-3a, F-4, F-5 and F-6 recorded; physical preflight READY and Unity lease **eligibility** verified (§5 step 2). **Not** the lock being held, and **not** F-3b | **STOP — NOT ISSUED:** `________________` |

---

## 9. Owner signature block — BLANK

**PRESENTATION RULE (B-1).** This instrument **may not be presented to the owner for signature while F-2 (operative physical Ops route) or F-4 (named human custody owner and event-driven release protocol) is blank in its own bytes. A signature given over an instrument with F-2 or F-4 blank is void.** The final signable revision must record the operative route and the custody/release controls in its own bytes, and be independently reviewed again, before presentation. The owner's W §12 signature never stands in for the separate owner/Manager permission for any #652 live deployment, and the Manager's later source-start never stands in for either.

**THIS DOCUMENT IS INOPERATIVE UNTIL THE PRODUCT OWNER PERSONALLY SIGNS THESE EXACT BYTES** — and, per the presentation rule, only a revision that satisfies it can be so signed. No agent may sign, initial, date, complete or infer consent on the owner's behalf, and no edit to this file after signing is effective: the signature binds only the bytes whose SHA-256 is recorded below.

How the hash works: a file cannot contain its own SHA-256. The owner (or whoever the owner directs to run the hash, with the owner personally confirming it) therefore computes `sha256sum` of this file **as committed at a named commit** and records that value, the commit and a permalink in a **separate, external signing record** (for example a GitHub comment). The file itself stays unedited. The table below is a **template for that external record**: it is not filled in this file, and its blanks are not completed by editing these bytes.

| Field (template for the EXTERNAL signing record) | Value (blank here; recorded only in the external record by the owner) |
| --- | --- |
| Document path and commit signed | `________________` |
| **Exact document SHA-256** | `____________________________________________________________________` |
| Owner signature | `____________________________________` |
| Date / time (UTC) | `____-__-__ __:__:__ UTC` |
| Permalink to signing record | `____________________________________` |

Signing this document does not by itself satisfy any item marked REQUIRED in §2 other than R-1.

---

## 10. What the author of this instrument did and did not verify

**Revision 2:** the repairs were made against the independent review's findings B-1..B-3 and N-1..N-5; this author also read the cited #733 audit text and the #733-acceptance line in the coordination record to quote the dated ETA finding. Revision 2 is not re-reviewed.

**Verified (read or executed by this author):** the #743 brief, the #741 packet §3.E/§4, the #731 draft text, and D-086 to D-091 in `Docs/Project/decision-log.md` at the starting commit; the seven paths are absent and `Assets/ForestPrototype/Editor/` exists at `e8e84027`; the arithmetic cross-check in §1.

**NOT verified:** the original owner messages (06:27, 06:44, 06:50, 07:07, 11:32, 11:42, 11:49 UTC) beyond how the decision log and the Manager draft quote them; the #676 curve, deviation figure, tree/cell counts and prefab GUIDs; any live Ops, board, lane, lease or preflight state, or whether the installed Ops has since been patched; the state of #652 or #690 now; whether the installed Ops can hold indefinite custody; the availability of any named model or session; any dashboard or Drive mirror state. Nothing was staged, installed, imported, built or launched.
