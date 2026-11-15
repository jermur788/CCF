# scen1 — DeepSeek parent orchestration: FINAL RESULT — three-profile save isolation and compact Scenario One

**Run:** 2026-11-15, parent claim [#535](https://github.com/jermur788/CCF/issues/535). **Orchestrator:** DeepSeek `opencode-go/deepseek-v4.1-flash`, variant **high**, ORCHESTRATOR ONLY.
**Orchestration branch:** `orchestration/scen1-shared-save-compact-complete-20261115-v1`, created at the published Manager release head `b5ae52d4c057bad819259c135d48a7b2ec32be6e`.
**Isolated docs-only worktree:** `/media/jer/Files/CCF/Orchestration/CCF-Scen1-SharedSave-20261115`. **Not** `/media/jer/Files/CCF-Primary`.
**Run ledger (continuously appended, single writer):** [scen1-ORCHESTRATION-EVIDENCE-LEDGER-20261115.md](https://github.com/jermur788/CCF/blob/orchestration/scen1-shared-save-compact-complete-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-ORCHESTRATION-EVIDENCE-LEDGER-20261115.md) — EVENTS 001–014 plus the V2 appendix below.

---

## 1. Verdict in one paragraph

**Stage A is complete and independently reviewed. Stages B–G did not start, because their mandatory product-approval gates are genuinely unmet — not because of capacity, and not because of any technical failure.** The save specification has been taken from an unratified DRAFT (63 unchecked requirements, 15 unaccepted blockers, several statements that only guessed at code behaviour) to a **repaired, independently reviewed APPROVAL CANDIDATE V2 that is fit to sign** — while every gate that belongs to the owner stayed shut. Two things were found that would have caused real damage if this run had been less careful: a **specification that would have been signed into conflict with accepted canon D-079/D-081** (wrong-world loads are supposed to be rejected *before* mutation; the draft kept a late warning and even forbade the fix), and the fact that **the ops Unity-lease tooling cannot acquire a lease on this host at all**. No production source, no Unity, no save file and no Windows build was touched.

**Nothing here is APPROVED, VERIFIED, INTEGRATED or ACCEPTED.** One owner signature stands between this run and authorised implementation.

---

## 2. What was actually produced (exact, verifiable)

| Claim | Work unit | Provider / model / effort | Branch | Commit | Status |
| --- | --- | --- | --- | --- | --- |
| [#537](https://github.com/jermur788/CCF/issues/537) | G0 read-only source + fixture provenance audit (1110 lines) | OpenAI **B** `gpt-6.1-sol` high | `docs/scen1-g0-save-source-audit-20261115-v1` | `3c8d7a50` | **HANDED-OFF**, closed |
| [#538](https://github.com/jermur788/CCF/issues/538) | Four-file spec **APPROVAL CANDIDATE V2** | OpenAI **B** `gpt-6.1-sol` high | `docs/scen1-shared-save-final-spec-candidate-20261115-v1` | `117f7617` | **HANDED-OFF**, closed |
| [#539](https://github.com/jermur788/CCF/issues/539) | Independent nonauthor review — **NEEDS CHANGES, 0 blocker / 5 material** | Claude **A** `claude-opus-5-5` **medium** | `docs/scen1-save-spec-v2-independent-review-20261115-v1` | `ad15e1d6` | **REVIEWED**, closed |
| [#540](https://github.com/jermur788/CCF/issues/540) | Bounded repair R1 — 13/13 findings addressed | OpenAI **B** `gpt-6.1-sol` high | `docs/scen1-save-spec-v2-repair-r1-20261115-v1` | `200add4e` | **HANDED-OFF**, closed |
| [#541](https://github.com/jermur788/CCF/issues/541) | Focused delta re-check of the repair | Claude **A** `claude-opus-5-5` medium | `docs/scen1-save-spec-v2-delta-recheck-20261115-v1` | see §8 | **REVIEWED** |

**Deliverables a reader can open directly:**

1. **G0 audit** — [scen1-SHARED-SAVE-G0-SOURCE-PROVENANCE-AUDIT-20261115.md](https://github.com/jermur788/CCF/blob/3c8d7a50a8f9f1baae417c46d7d0a06b68e495f8/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-G0-SOURCE-PROVENANCE-AUDIT-20261115.md). Entry-point/caller inventory, serialization and `.tmp`/`.bak` semantics, validation order, historical admission, geometry dispatch, Reference identity, prefs, fixture provenance, minimum implementable route, risk register.
2. **The signable package** — `specs/scenario-independent-save-isolation-v1/{spec,plan,tasks,checklists/requirements}.md` at `200add4e`, plus [the candidate report](https://github.com/jermur788/CCF/blob/200add4e6e5116268b79ef63131ff9b836f14c5d/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-SPEC-V2-APPROVAL-CANDIDATE-20261115.md) and [the R1 repair report](https://github.com/jermur788/CCF/blob/200add4e6e5116268b79ef63131ff9b836f14c5d/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-SPEC-V2-REPAIR-R1-20261115.md). **63 ISO IDs, all unchecked.** Status line: `APPROVAL CANDIDATE V2 — NOT APPROVED — NO IMPLEMENTATION AUTHORITY`.
3. **`§0.4 What changes for a historical player`** — the disclosure table added because of review finding M-1, including an explicit column **"Can a previously loadable save fail?"**.
4. **[Independent review](https://github.com/jermur788/CCF/blob/ad15e1d68a3545bb05d1214873c5a659a8ab7aa6/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-SPEC-V2-INDEPENDENT-REVIEW-20261115.md)** — 22 groups of `file:line` citations re-checked against source, all reproduce except two line-offset nits.

---

## 3. Gate status — why no code was written

The handoff makes production conditional on gates that are **not** mine to open. Their state, independently re-verified in this run rather than quoted:

| Gate | Required | State at end of run | Evidence |
| --- | --- | --- | --- |
| **A — evidence + final-spec candidate** | audit + signable package + review | **COMPLETE** | §2 above |
| **G1 — formal four-file APPROVED spec** | owner / authorised manager signature | **NOT MET — candidate is ready to sign** | #533 records the package as not approved; [#425](https://github.com/jermur788/CCF/issues/425) is `[AWAITING OWNER / NOT RUNNING]`; spec status line is still NOT APPROVED; 63/63 unchecked |
| **Compact parent + G1 child** | **APPROVED** specs | **NOT MET** | [#516](https://github.com/jermur788/CCF) parent = **115 unchecked, 0 checked**; [#515](https://github.com/jermur788/CCF) G1 child = **33 unchecked, 0 checked** — counted, not assumed |
| **Canonical D-082 / #471** | accepted exception so compact becomes first demo | **NOT MET** | [#471](https://github.com/jermur788/CCF/issues/471) `[PAUSED / NOT RUNNING]`; **D-082 is absent from the Decision Log**; five-rack Enlarged80 canon (D-056/D-064/D-065/D-071) still governs |
| **B — core profile safety (code)** | APPROVED spec + live file ownership | **BLOCKED BY G1** — deliberately not started | — |
| **C — independent source review of code** | a pushed source candidate | **BLOCKED** (no code exists) | — |
| **D — save feature Unity Tier 2** | source candidate + exclusive Unity lease | **BLOCKED**, and **additionally** blocked by an ops defect (§6.1) | — |
| **E — compact world/loop** | D-082 + compact specs + new source ownership | **BLOCKED** | — |
| **F — integrated Tier 3** | an integrated candidate | **NOT READY** | — |
| **G — owner playtest → Windows** | owner tested the exact Editor candidate | **NOT REACHED. No Windows build attempted or authorised.** | — |

**No DRAFT was treated as APPROVED. No ISO item was checked. No source file was edited. No Unity process was started. No save file was read for content beyond non-destructive metadata/hashing, and none was written.**

---

## 4. The one decision this run needs

This is deliberately **one grouped request**, not a stream of questions. Each item states a recommended narrow default; the owner may accept, modify or reject the bundle as a whole.

### D-A — Sign the four-file save specification (recommended: **sign**)

**What signing does:** accepts the package at commit `200add4e` — the owner-fixed rules, the recommended defaults, the explicit preserve/defer/HOLD dispositions, and the 63 ISO criteria as the contract for separately authorised implementation.
**What signing does NOT do** (stated in `§0.3`): it does not authorise code or serialized-asset edits, a work claim or lock, Unity/Player/build, save writes/migration/deletion, integration or release, archive/hash changes, canonical project edits, a complete-corpus claim, compact first-demo promotion, or approval of the H1 omission roster / G1 viability / compact completion threshold.
**Why it is safe to sign now:** the three audit-contradicted gaps (B-09 recovery, B-10 pre-mutation admission, B-11 third geometry) are written as **requirements to close**, not as claims of existing safety; historical *genuine* save admission is unchanged; every preservation anchor is intact; and the review verified the citations rather than the prose.
**If you decline:** nothing further can be done on the save substrate. Stage A would be archived as a candidate and the programme stays where it is.

### D-B — D-082 / #471 (recommended: **separate, later, and only after D-A**)

Compact cannot become the canonical first demo until D-082 exists as an accepted exception, and #471 is paused. I did **not** un-pause it: that is a portfolio decision, not an orchestration one. Recommended: authorise one bounded independent canonical review for #471 after D-A, so the five-rack Enlarged80 historical policy stays intact for historical profiles. **Nothing in this run depends on it.**

### D-C — Compact parent #516 + G1 child #515 (recommended: **do not approve yet**)

These cannot be approved on the present evidence, and approving them now would repeat the exact mistake this run caught. Still unresolved: the **30 × 30 m world dimension** (which the spec deliberately keeps out of the save signature — review finding M-2), the **exact H1 fourteen-tree omission roster**, **post-H1 G1 viability**, and a **testable completion predicate**. All four need a world/spec decision, not a save-policy decision.

### D-D — Implementation authorisation and the Unity blocker (recommended: **fix ops first**)

After D-A, the B stage needs: a live file-set claim through [#424](https://github.com/jermur788/CCF/issues/424) **and** a working Unity lease. The lease CLI is currently broken on this host (§6.1). Both are prerequisites, not formalities.

---

## 5. What this run genuinely established (and what it did not)

**Established, with evidence:**

- The save subsystem's real behaviour is now documented with `file:line` — **B-15 (caller inventory) proven within static scope**.
- **Three assumed safety properties are contradicted by the code.** Backup is notification-only (`ForestSaveController.cs:286`) with a non-atomic `File.Copy` fallback (`:84-85`); a foreign `scenarioId` only warns **after** geometry/tree/ecology restoration (`ScenarioOneManager.cs:1481`); a third geometry value would silently inherit 40 m legacy sizing (`StandGeometryModel.cs:34`, `ForestStartingStand.cs:83`).
- **Two further hazards the DRAFT had missed:** `CaptureData` is not universally pure — it can initialise live model-2 understorey (`ScenarioOneManager.cs:1398`) — and preview return is not an all-or-nothing rollback (`:725-740`).
- **Reference identity is chapter-level only** (`ScenarioReferenceArchive.cs:121-128`), so a compact profile reusing `scenario-one` would match the frozen Legacy40 archive unless preview is gated by active profile.
- **Genuine historical fixtures exist** — v8, v9, v12 (frozen), v19, v20, v21, v22 — with byte-verified provenance. I independently re-hashed the v20/v21 set: **all match, and their original paths still exist.**
- **Genuine coverage is absent** for v1–v7, v10, v11, v13–v18, v23, v24, and **no genuine file with a raw-absent nested ID exists anywhere.** B-04 therefore cannot be closed by evidence in this run.
- A trap for future verifiers: `recorded-v22-year3-save.json` exists in several evidence roots with **different contents** — fixtures must be identified by **path + sha256**, never by filename.

**Not established — and I will not imply otherwise:** no C# compilation, no Unity Editor, no Play Mode, no Player, no Windows build, no actual save/load, no `File.Replace` platform behaviour, no failure injection, no PlayerPrefs I/O, no geometry rendering, no compact world, no G1 planting, no completion run. **Every one of those remains NOT RUN.**

---

## 6. Operational findings the owner should know

**6.1 The Unity lease tooling cannot acquire a lease on this host.** `CCF-Ops/scripts/unity-lock status` fails with `ZX20 mount identity mismatch` because `lanes.json` pins `storage.device = /dev/sda2` while the live mount is `/dev/sdc2` (UUID `18D6-896E` and `exfat` both match). Recorded in the ledger as a real obstacle for **any** future Unity stage, and precisely why holding a lease during this run would have been pointless. No lock existed (`lock.json` absent), no Editor process, no stale lock created.

**6.2 The coordination anchor is 16 decisions stale.** `/media/jer/Files/CCF-Primary/Docs/Project/decision-log.md` at `46247486` holds **65** decision rows; `origin/main` at `fc18190b` holds **81**. The anchor is not an ancestor of `main` (divergent `ops/workflow-foundation-v1`). A worker reading only the anchor misses **D-066–D-081** — including the pre-mutation rejection rule this spec initially violated. **Recommendation:** canonical context reads must come from `main`, not from the anchor.

**6.3 The release branch is not a descendant of `main`.** `docs/scen1-deepseek-shared-save-implementation-release-20261115-v1` has parent `8217f5f8`, whose tree equals `main@fc18190b`; `git diff --name-status origin/main <release>` is **exactly 2 added files, 0 modifications**. Content-safe, but integration must be a deliberate rebase/cherry-pick, never a fast-forward.

---

## 7. Timing and throughput rows (for the project-wide ledger's owner)

Provided as **structured rows for a management-owned append** to `Docs/Project/coordination/development-throughput-ledger.md`. **I did not write that file or the live board.** Active-vs-wall time for delegated children is **not separable** from this harness; token counts and costs were **not observed** and are not estimated.

| Claim / stage | Start UTC (host `date -u`) | End UTC | Elapsed class | Blocked/queue reason | Provider / model / pool | Review cycles | Unity lease/import | Confidence |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| #535 parent registration | 09:52:41 | — | active | none | DeepSeek V4.1 Flash `high` | 0 | **NONE** | HIGH |
| #537 G0 audit attempt 1 | 10:03 | 10:04 | ~1 min, **zero output** | **PROVIDER_QUOTA** (OpenAI A) | OpenAI A `gpt-6.1-sol` high | 0 | NONE | HIGH, verbatim provider text |
| #537 G0 audit attempt 2 | 10:07 | 10:52 | ~45 min wall | none after switch | OpenAI **B** `gpt-6.1-sol` high | 0 | NONE | HIGH |
| #538 spec candidate V2 | 10:58 | 11:38 | ~40 min wall | none | OpenAI **B** `gpt-6.1-sol` high | 0 | NONE | HIGH |
| #539 independent review | 11:42 | 12:20 | ~38 min wall | none | Claude A `claude-opus-5-5` medium | 1 | NONE | HIGH |
| #540 repair R1 | 12:28 | 12:55 | ~27 min wall | none | OpenAI **B** `gpt-6.1-sol` high | — | NONE | HIGH |
| #541 delta re-check | 12:58 | see §8 | — | none | Claude A `claude-opus-5-5` medium | 1 (delta) | NONE | HIGH |
| **OpenAI A exhausted interval** | 10:04 | provider-stated **2026-11-18T13:43** (tz not stated) | **~3 days blocked** | quota | — | — | — | HIGH; timezone UNKNOWN |

**Measurable throughput facts:** 1 provider-quota loss with **zero work loss** (refusal preceded any tool call); 4 Sol tasks on OpenAI **B** with 4 successes; 2 Opus reviews on Claude **A**; **0 uses of the Sonnet fallback**, because the both-OpenAI-pools-unavailable condition never occurred; **0 Unity leases, 0 imports, 0 builds**.

---

## 8. V2 Orchestration Decision Appendix

**8.1 Policy, objective, hard boundaries, evaluation priorities.** Objective fixed by the owner: bring three-profile save isolation through evidence → approved spec → implementation → independent review → real verification, then continue compact work only where its own gates permit. Hard boundaries actually obeyed: orchestrator-only (no product code, no Unity, no self-review, no self-approval, no source merge); no production edit before an APPROVED spec; no save/Reference/anchor mutation; no Windows build; no live-board write; preserve the owner's unrelated dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`. Priority ordering used when they conflicted: **historical-save safety and canonical honesty over speed** — which is why the run stopped at the approval gate rather than shipping a plausible-looking implementation.

**8.2 Capacity and context.** Pools observed directly at start: OpenAI A `gpt-6.1-sol` (available), OpenAI B `gpt-6.1-sol` (available), Claude A + Claude B (`opus-5-5`, `sonnet-5-5`), DeepSeek parent. **Quota percentages are UNKNOWN for every pool — no such surface exists in this harness**, and none is invented. One real capacity event occurred: **OpenAI A refused** with a verbatim provider message and a stated reset of **2026-11-18T13:43** (timezone unstated). Recovery was immediate and lossless. Sonnet was **never** used, correctly, because its activation condition never fired.

**8.3 Routing and decomposition.** Chain: audit (evidence) → spec candidate (author) → independent review (nonauthor, cross-provider) → bounded repair (original author) → delta re-check (same reviewer). Each substantive task got its **own GitHub claim** with a write allowlist, an exact base commit, forbidden paths, and a durable handoff. Two deliberate anti-failure patterns: the **reviewer was forbidden to edit** the artefact, and the **repair was forbidden to touch what the review passed**. The author was reused for the repair on purpose — independence lives with the reviewer, not with a rotating pen — and the **same reviewer** performed the delta check because it already held the finding list.
*Switch costs observed:* one lost provider call (quota), no rework; restarts were unnecessary because every child was given a self-contained brief rather than "go find context".

**8.4 Outcomes and resources.** 5 child claims, 5 closed, 0 abandoned mid-work, 0 leaked claims. 4 Sol tasks, 2 Opus reviews, 1 quota switch, **0 Sonnet fallbacks**, **0 Unity leases**, **0 builds**. Wall-clock from registration to final report: see §7. **Tokens, provider costs and energy are NOT measured and are NOT estimated.** Nothing was accepted that a manager had not accepted; nothing was approved by me.

**8.5 Learning assessment.**
- **Highest-value step, unambiguously, was the independent review.** It caught a **canon conflict (D-079/D-081)** that the author and orchestrator both missed, plus four disclosure/scope defects. Confidence: **high**, because the conflict was verified directly against `origin/main` and the spec's own base commit already contained D-081.
- **Second-highest-value step was the orchestrator's own source cross-check** before trusting any artefact: 5/5 DRAFT claims reproduced, and the fixture anchors were verified by hashing rather than by reading the audit's assertion.
- **The G0 audit was worth its cost** — it converted three assumed protections into known gaps and found two hazards (impure capture, non-transactional preview return) that no document had recorded.
- **Cheap and decisive:** the discovery that the spec's base already contained D-081 reframed an apparent "missing context" problem as a **reconciliation failure**, which is a different and more serious defect class.
- **Avoidable cost:** the DRAFT carried 15 unratified blockers and 63 conditional requirements into an implementation gate; a stricter spec-review bar earlier would have avoided a full repair cycle. **Hypothesis, medium confidence:** requiring every specification requirement to name a test type *at authorship time* would have caught M-5 (untestable ISO-58) without a review round.
- **No false greens:** no test was weakened, no anchor rebaselined, no acceptance criterion checked to make progress look better.

**8.6 Evidence classification of every consequential conclusion.**

| Conclusion | Class | Basis |
| --- | --- | --- |
| Current code lacks three assumed save protections (B-09/10/11) | **OBSERVED FACT** | audit `file:line`; two of them independently re-read by the orchestrator |
| Spec candidate requires closure rather than assuming safety | **OBSERVED FACT** | document text + review PASS on that point |
| Spec conflicted with D-079/D-081 and was repaired | **OBSERVED FACT** | canonical `origin/main` decision log read directly |
| Historical fixtures v20/v21 are genuine and byte-exact | **OBSERVED FACT** | orchestrator hashed them; manifest matched; source paths exist |
| No genuine raw-absent-ID fixture exists | **OBSERVED FACT (bounded)** | bounded scans only; other machines/containers NOT VERIFIED |
| Unity lease CLI is broken on this host | **OBSERVED FACT** | command output |
| Coordination anchor is 16 decisions stale | **OBSERVED FACT** | row counts 65 vs 81, commit hashes |
| No implementation started because gates are unmet | **OBSERVED FACT** | no commits to `Assets/`; no Unity process; board unwritten |
| The candidate is "good enough to sign" | **ORCHESTRATOR CLAIM + REVIEW** | reviewer verdict, not an owner decision |
| Anything about runtime behaviour | **NOT OBSERVED** | never executed |

**8.7 Final summary.** **Attempted:** 5 tasks. **Completed:** 5 (audit, candidate, review, repair, delta check). **Failed:** 1 dispatch (quota, zero-loss). **Blocked:** stages B–G on unmet product-approval gates, and Unity additionally on an ops defect. **Quotas/provider handovers:** OpenAI A exhausted → B carried all Sol work; Sonnet never used; Opus used twice within preauthorisation. **Final exact candidate:** save spec `200add4e` on `docs/scen1-save-spec-v2-repair-r1-20261115-v1`; audit `3c8d7a50`. **Protected anchors:** untouched and, where checkable, re-verified (frozen Reference `789e3edd…b041fc`; `CCF.Learning.v1.*`; `CCF.MenuHelp.v1.*`; Legacy40=0/Enlarged80=1; five-rack canon). **Independent Opus findings:** 0 blocker / 5 material / 5 minor / 3 nit, all addressed in R1, delta-checked in #541. **Integration status:** NOT INTEGRATED, NOT VERIFIED at runtime, NOT ACCEPTED. **Unresolved product gates:** D-A signature, D-082/#471, compact parent+child, world dimension, H1 roster, G1 viability, completion predicate. **Exact next executable action:** owner signs or rejects D-A. **Dashboard/Drive:** **NOT VERIFIED.**

---

## 9. Next executable packets (ready, not started)

1. **On signature:** one source-ownership claim via #424 for `Assets/ForestPrototype/ForestSaveController.cs`, `ForestSaveData.cs`, `StandGeometryModel.cs`, `ForestStartingStand.cs`, `ScenarioOne/ScenarioOneManager.cs`, `ScenarioOne/ScenarioReferenceArchive.cs` + the menu files — **plus** a working Unity lease (§6.1), then the B stage against the ISO-01…ISO-63 contract.
2. **On D-B:** one bounded independent canonical review for #471 → D-082 acceptance → compact specs can then be adjudicated.
3. **On D-C evidence:** a world-dimension + H1 roster + G1 viability + completion-predicate decision packet, prepared for a single owner ruling.
4. **Always:** fix `CCF-Ops/lanes.json` `storage.device` to the live `/dev/sdc2` (or better, key lane identity on UUID only, since the device passthrough changes across boots).

---

**Notifications:** [#424](https://github.com/jermur788/CCF/issues/424) (Scenario One delivery/live coordination) and [#425](https://github.com/jermur788/CCF/issues/425) (Overall Manager, currently `AWAITING OWNER / NOT RUNNING`) are notified with direct ledger and result URLs. **No Drive or dashboard synchronisation is claimed; it is NOT VERIFIED.**
