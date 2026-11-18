# Multi #770 — DeepSeek TG Fork Multi 3D simulator Phase A result

**18 November 2026 · [Multi #770](https://github.com/jermur788/CCF/issues/770) · Manager [#730](https://github.com/jermur788/CCF/issues/730) · overall [#425](https://github.com/jermur788/CCF/issues/425)**

**Status: PHASE A COMPLETE (docs-only orchestration). 3D SOURCE/UNITY/SPEC STILL HELD.** This is the single parent result for the corrected Multi #770 trial. It reports what was actually produced, by which real models, what the independent reviewer found, what was repaired, and the one decision-ready path forward. It is **not** an implementation, a runtime qualification, a security PASS, an accepted 3D specification or a self-approval. No 3D application code, Unity scene, browser build, test run, fixture read, Ops row or `main` change exists as a result of this phase.

**Routing note.** The earlier `Sen-1-770-…COMPACT30-UNITY-TIER-S…` packet and its #731 G0 grant are **SUPERSEDED** and were never dispatched. #770 is Multi, managed by #730. Scenario One #718/#731 material was not touched.

---

## 1. Actual registration

| Field | Verified value |
| --- | --- |
| Actor / role | DeepSeek native TG Fork orchestrator (parent), Phase A docs-only; no source or file ownership over child lanes |
| Model | `opencode-go/deepseek-v4.1-flash` (DeepSeek V4.1 Flash), variant **high** |
| Harness / host | OpenCode inside the T3 Code desktop AppImage, native Linux, `jer-laptop`; `orchestrator_capabilities` + `delegate_task` surfaces |
| Parent thread / session | `b05d4ce1-93fc-42b9-aa4e-694180e97607`; `ses_e0ff86f69ffeD7ntUteHLcxjbg` |
| Fresh GitHub `main` | `ff03290a5ef9c53472618aba32591e5822c663e1` |
| Parent docs branch | `docs/multi-770-deepseek-tg-fork-3d-sim-phase-a-20261118-v1` (worktree `/tmp/opencode/ccf-770-docs`) |
| `[RUNNING]` registration | [comment 6988943063](https://github.com/jermur788/CCF/issues/770#issuecomment-6988943063), 19:17:16Z, before any edit |
| TG Fork skills | `orchestrate-work` `b27b317a…d74284057`, `delegate-work` `933e47b4…7f26748c`, `recover-worker` `ae5d678f…99fbef87`, `reconcile-handoff` `7d25e7e3…67e5abe9`, installed user scope from pinned package `6fd7e7c8` |
| Skill preflight | `native_preflight.py` still **FAIL** on its no-symlink policy; **not** reported as passing |
| Forbidden model | GPT-6.2 Sol/Astra never listed, selected, escalated or used as fallback |

## 2. Superseded Sen-1 work: safe-stop record

The obsolete Sen-1 Compact30 Unity packet was read and **evaluated before any mutation**. Because #731 had not adopted it at the time, no Sen-1 child, branch, worktree, deliverable or file was ever created. The only artefacts were two GitHub comments on #770 (a blocked registration and a skill-install receipt), preserved unedited, plus the local user-scope skill install. Nothing was deleted, stashed, reset or cleaned. On the owner's correction the claim was re-registered against the corrected Multi packet; no Sen-1 descendant was started, so there was nothing to stop.

## 3. Genuine TG Fork delegation

One review cycle with one bounded correction pass, all through the native `delegate_task` surface. Each lane held its own claim, branch and sole file. No child held source, fixture, protected-read or approval authority.

| Lane | Actual model / provider | Attempt | Commit | Branch | Wall time |
| --- | --- | --- | --- | --- | --- |
| **A1** architecture + state truth | **GPT-6.1 Sol** (`gpt-6.1-sol`, effort high) / `codex` | `att-770-a1-01` | `c8f43035` (743 lines) | `docs/multi-770-a1-3d-architecture-dossier-20261118-v1` | 12m20s |
| **A2** interaction + visual acceptance | **Claude Sonnet 5.5** (`claude-sonnet-5-5`) / `claudeAgent` | `att-770-a2-01` | `b34d1995` (377 lines) | `docs/multi-770-a2-3d-interaction-acceptance-dossier-20261118-v1` | 5m10s |
| **Q** independent nonauthor review | **Claude Opus 5.5** (`claude-opus-5-5`) / `claudeAgent_claude_b_pro` | `att-770-q-01` | `52b010d7` (133 lines) | `docs/multi-770-q-independent-3d-review-20261118-v1` | 5m46s |
| **A1 r1** correction | GPT-6.1 Sol | `att-770-a1-02` | `25c0eb42` (761 lines cumulative) | same A1 branch | 5m |
| **A2 r1** correction | Claude Sonnet 5.5 | `att-770-a2-02` | `2c7533e1` (490 lines cumulative) | same A2 branch | 8m |
| **Q delta** re-check | Claude Opus 5.5 | `att-770-q-02` | `88a17991` (216 lines cumulative) | same Q branch | 5m |

Every branch is based on `ff03290a` and adds **exactly one file** against that base; nothing else in the repository changed. Child claims precede their commits in every case (A1 19:21:24Z→19:32:05Z, A2 19:20:33Z→19:24:55Z, Q 19:34:41Z→19:40:01Z). No child reviewed or approved its own work; Q is a third model on a third provider instance and authored neither dossier.

## 4. What the lanes actually produced

### A1 — architecture and state/truth boundary (GPT-6.1 Sol)
[Pinned dossier](https://github.com/jermur788/CCF/blob/25c0eb427f9535a0d70749c27a6b6a65800fa0a8/Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-A1-3D-SIMULATION-ARCHITECTURE-AND-STATE-TRUTH-DOSSIER-20261118.md)

- Three truth layers with a no-reverse-flow invariant: authoritative simulated biology/numerics → approved 2D H0-A reference contract → proposed 3D display. A renderer may not become a source of record.
- A four-responsibility, one-direction architecture (admission/read boundary → projection adapter → scene+accessible inspector → session interaction) that explicitly prevents the adapter becoming a second simulator: a renderer can compute stem distances but has no authority to produce a competition index.
- Per-tree provenance requirements for the exact Year0 world: stable ID plus full observation identity, integer mm coordinates and the documented transform ID, availability semantics that must not collapse `NotEvaluated` into `Unavailable`, and `unityX/unityZ` identified as forward-transform f32 values with **no** Unity frame, handedness or engine meaning.
- Year/stage semantics: Year0 authoritative, Year1 historical and locally qualified only, Year2 DRAFT never rendered as authority; unsupported year/stage requests are refused rather than substituted.
- Parallel runtime comparisons (browser WebGL, separate Unity prototype, other native such as Godot), containment/reproducibility/performance evidence boundaries, a recommended smallest Year0 inspect-only slice, and 7 open owner decisions.
- States plainly that Phase A contains **no verified per-tree geometry** and every W-xx check is NOT RUN.

### A2 — interaction, accessibility, visual acceptance and scoped tests (Claude Sonnet 5.5)
[Pinned dossier](https://github.com/jermur788/CCF/blob/2c7533e192a171bfb123e40d80ef9235e6e1bb91/Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-A2-3D-INTERACTION-VISUAL-ACCEPTANCE-AND-TEST-DOSSIER-20261118.md)

- Deterministic scene construction from the trusted Year0 projection only; three never-conflated identities (data ID, internal visual identity, display ordinal); refuse rather than guess when a required value is absent.
- Inspect-only interaction with per-field SOURCE / DERIVED / PRESENTATION labelling, explicit non-features (no marking, no year progression, no save, no mutation), keyboard-complete path, ARIA table equivalent, and WebGL-unavailable degradation.
- Provenance and determinism: persistent "static Genesis Year0 fixture" banner plus source pin, dataset-level byte identity distinguished from rendered pixels, year/status disclosure rules that avoid implying unmodelled ecology.
- 26 proposed browser-candidate acceptance vectors (T-01…T-26, **none run**) plus 8 runtime-neutral required outcomes (N-01…N-08) so a Unity or native choice is not disadvantaged, 15 owner decisions, and an explicit NOT VERIFIED section.

## 5. Independent review outcome

Q reviewed both frozen commits in full and returned **BLOCKING FINDINGS - NEEDS AUTHOR REVISION**: 1 blocker, 5 major, 6 minor, 4 observations. The blocker was substantive, not stylistic: A2 had treated the adopted #758 contract as off-limits, and therefore told the owner that height and crown data do not exist when the adopted contract defines `heightM` and `crownRadiusM` (a *potential*-crown rule), plus documented IDs, integer-mm coordinates, transform ID, both seeds and the fixture pin. Q also found A2 showing per-row `status="live"` as a dataset-level "source status: live" banner, and having the viewer compute the strict `dbhMm < 170` comparison that the contract requires the read boundary to supply.

The parent independently verified Q's contract claims against the public adopted text before acting, confirming: the 12-key row `treeId, xMm, yMm, status, ageYears, dbhMm, heightMm, dbhCm, heightM, crownRadiusM, unityX, unityZ`; `status` as per-row life status; `belowActionNote` as annotation only with "A never compares DBH itself"; and `unityX/unityZ` as the `TransformBitMismatch` oracle with no engine semantics.

Both authors then received a bounded correction pass within their existing branch and sole file: A1 addressed Q-04 (unityX/unityZ), Q-07 (commit pins), Q-11 (precedent framing), Q-13 (clarity that no geometry is verified); A2 addressed the blocker and its assigned majors/minors after finally reading the public adopted contract. No finding was disputed. The corrected content is substantive, not a banner: A2 now documents `F-12…F-19`, the per-row life-status wording, `belowActionNote` as `PresentComputed` never recomputed by the viewer, both seeds, the three-identity model, and runtime-neutral outcome vectors.

### 5.1 Delta re-check (bounded, one review cycle)

The same reviewer re-checked its own 16 findings against A1 r1 `25c0eb42` and A2 r1 `2c7533e1`, verifying the revised text rather than the authors' own "ADDRESSED" banners: **DELTA RE-CHECK - ALL PRIOR BLOCKING AND MAJOR FINDINGS ADDRESSED**. Dispositions: Q-01 through Q-13 and Q-15 ADDRESSED, Q-14 NOT IN LANE (owner/Manager), Q-16 MOOT. The re-check also confirmed branch integrity (A1 and A2 each gained exactly one commit on top of the reviewed commit, no force-push) and no material regression: no new false statement, invented value, execution claim, silently closed decision, #769 duplication, Scenario One material, code block or scope widening.

Four optional wording items were left for spec stage rather than another author pass: one attribution of the "not health" wording, one imprecise attribution of the Unsupported/Blocked/Refused labels, one dropped "informational, not acceptance evidence" qualifier on the 200-envelope agreement, and two branch-URL citations that could be commit-pinned. None affects a decision, a fact or the blocker.

This was deliberately **one review cycle with one bounded delta**, not a review waterfall, and it materially changed the technical content: without Q, the owner would have decided fidelity on a false premise. The disposition of all four documents remains with Manager #730.

## 6. Cross-check with Multi #769 (no duplication)

#769 is the separate 2D H0-A Core/Viewer trial, closed and handed off at `5e175ebe`. Its four branches and its `769-orchestration/` documents were not read as inputs, not edited, not duplicated and not merged here. The only relationship is boundary-level: #769 establishes an approved 2D read contract and a document style; #770 explicitly treats that contract as a **2D reference only**, not an approved live 3D API. Q flagged one proximity risk (an A2-introduced Canvas-2D pseudo-3D option) that the owner/Manager must check against #769 before any pursuit.

## 7. Source-input inventory (verified, not assumed)

| Source | Pin observed | Use |
| --- | --- | --- |
| Corrected #770 packet | branch tip `89412961`, 11,101 B | authority and scope |
| #730 adoption comment | `6988856214`, 19:13:52Z | Phase A docs-only grant |
| #750 owner H0-A four-file approval | branch tip `a9579188`, 8,052 B | 2D reference scope |
| #758A contract adoption + local-only policy | `a7e9228b`, 7,210 B | adopted interface v1.0.0 |
| #758C complete corrected contract | `7af3add1` (pre-adoption), 512 lines | field/seed/identity facts |
| #766 G1/G2 admission | `06cad165`, 23,995 B | no 3D physical lane exists |
| #725 / #729 / #732 | closed: source, independent review, native qualification | Year1 is locally qualified, historical |
| #760 / #761 | closed: quality-approved **DRAFT** | Year2 and marking are not renderable authority |

No fixture bytes, protected P0 tree, #687/#710/#725 source, Ops row or Scenario One file was read or copied by any lane. Years and statuses above are document-stated; **no numeric value in this phase was re-derived from fixture bytes.**

## 8. Decision-ready 3D proposal (recommendation only)

The strongest common position across A1, A2 and Q is a **smallest credible Year0 inspect-only slice**: one non-public, local-only, immutable scene of the exact 100-tree synthetic experiment, schematic geometry derived only from admitted source dimensions, complete accessible ID inspection, and fail-closed refusal instead of a partial forest. It would prove that a spatial representation preserves source identity, coordinates, dimensions and availability semantics; it would prove nothing about growth, hazard, regeneration, economics or gameplay.

Runtime is deliberately **not** chosen. A1's parallel framing governs: browser WebGL, a separate Unity prototype, or another native runtime, each requiring its own containment, determinism, asset/licence, accessibility, performance and test-observability evidence, and none inheriting H0-A's permissions. A2's Canvas-2D pseudo-3D variant exists only as an owner option with an explicit #769 non-overlap check.

## 9. Owner decisions still required

Consolidated from A1 §12, A2 §9 and Q §7 without adding new ones: (1) purpose of Multi 3D — defer, non-public prototype, or step toward a simulator; (2) runtime; (3) time scope (Year0 only, or Year0 plus separately authorised static historical Year1); (4) source/read arrangement — reuse a frozen authorised artifact, a separately qualified projection, a separate decoder, or a live API as new scope; (5) baseline geometry — DBH markers only, schematic height+crown envelopes from sourced values, or richer art; (6) scale policy, including whether any exaggeration is a disclosed versioned mapping field; (7) interaction/camera complexity; (8) whether to show the <170 mm note at all (who computes it is already settled: the read boundary); (9) accessibility bar; (10) graphics-unavailable policy; (11) performance envelope and named reference machines; (12) determinism claim level; (13) status/availability vocabulary adoption; (14) test tooling; (15) coordinate mapping choice, including axis, handedness and pivot; (16) custody, storage, non-public handling and any later publication decision.

## 10. Measured orchestration performance

| Measure | Observed |
| --- | --- |
| Wall clock, registration to parent result | ~41 min |
| Dispatches | 5 substantive author/reviewer dispatches + 1 bounded delta re-check |
| Failed launches / retries | 0 launch failures; 1 bounded correction pass per author lane, then a single delta re-check |
| Child wall time | A1 12m20s · A2 5m10s · Q 5m46s · A1 r1 5m · A2 r1 8m |
| Parent verification time | ~7 min across 6 independent branch/diff/claim checks |
| Interruptions / handbacks | 0 interrupts; 0 abandoned children |
| Token, energy or quota figures | **UNKNOWN** — not exposed, not invented |

**Routing lessons, including failures.** (a) The pre-correction Sen-1 packet cost ~13 minutes of verification and two comments before it was superseded; gate-first checking was still the cheaper path because it prevented unauthorised scene work. (b) The TG Fork orchestration skills were genuinely absent from project scope; installing them at user scope was a real prerequisite, and `native_preflight.py` remains a false-negative for symlinked installs. (c) A requested `effort: high` was echoed in the A2 receipt but the child observed "low"; requested-versus-observed is now recorded rather than assumed. (d) A2's r1 correction had to push from a detached HEAD because its branch was already checked out in another worktree, leaving that other worktree one commit behind — a real single-writer/worktree-lock hazard worth avoiding in Phase B. (e) A substantive independent review changed the technical content materially: without Q, the owner would have decided fidelity on a false premise.

## 11. One next packet, conditional on product scope

No G1/G2 request is issued here. The single next action is a formal **Multi 3D SPEC** (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) authored only after the owner answers decisions 1–5 and 16, referencing A1/A2/Q at their corrected pins, with an independent nonauthor review and owner approval. Any later implementation then requires a fresh physical lane, actor-specific G1/G2 security grants, an approved source/asset allowlist, and — if Unity is chosen — a separate Unity lease and Editor qualification. L4 FAIL38 remains UNWAIVED.

## 12. What this does not prove

No 3D application, scene, renderer or runtime exists. Nothing was built, rendered, executed or tested; every vector in A2 is NOT RUN and every W-xx check in A1 is NOT RUN. No fixture byte was read, so **no per-tree coordinate, dimension or seed is verified data** — all such values are document-stated. No performance, accessibility, containment or security property was measured. No technology, scope, fidelity or policy decision was made by any lane or by the parent. This document approves nothing, qualifies nothing and waives nothing.

## 13. Evidence and continuity

- Routing ledger (append-only portable fallback, `store: portable-fallback`, `runtime: unavailable`, `canonical_persistence: false`): `/tmp/opencode/770-orchestration/routing-ledger.jsonl`, 27 records covering pre-dispatch readiness, write-ahead attempts, receipts, reconciliation, correction continuations and the delta re-check.
- Independently verified dossier hashes at reviewed pins: A1 r1 `922aa105…`, A2 r1 `aecb2ce7…`, Q r1 `bf5372f7…`.
- Child claims: [A1](https://github.com/jermur788/CCF/issues/770#issuecomment-6989053274) · [A2](https://github.com/jermur788/CCF/issues/770#issuecomment-6989030661) · [Q](https://github.com/jermur788/CCF/issues/770#issuecomment-6989377832).

**Handoff:** Phase A documentation is complete and review-corrected. Disposition of A1, A2 and Q belongs to Manager #730. No source, Unity, browser, test, build, Ops, `main` or Windows action is authorised by this result. Dashboard/Drive unverified.
