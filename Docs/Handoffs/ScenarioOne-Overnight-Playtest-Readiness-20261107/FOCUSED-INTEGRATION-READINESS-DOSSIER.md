# Focused Scenario One integration readiness dossier — #315 (Mimo overnight, read-only preparation)

**Author:** Mimo orchestrator (`xiaomi-token-plan-ams/mimo-v2.6-pro`, effort UNKNOWN), under [#313](https://github.com/jermur788/CCF/issues/313) / binding [master packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MANAGER-MIMO-OVERNIGHT-SCENARIO-ONE-PLAYTEST-ORCHESTRATION-20261107.md), [conditional integration packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MANAGER-MIMO-FOCUSED-INTEGRATION-QUALIFICATION-PACKET-20261107.md).
**Date:** 2026-11-07 (IE). **Status: DOCS-ONLY PREPARATION.** This dossier **does not** create an integration candidate, register a lane, grant ownership, release Unity or waive any gate. Integration source writes and any Unity remain **WAITING MANDATORY MANAGER #200 + STORAGE GATES**. Proposed branch `integration/scenario-one-focused-playtest-20261107-v1` stays **NOT REGISTERED / NO WRITER**.
**Method:** read-only Git forensics (local canonical store, fetched refs), live machine/lease/disk observation, and the overnight results at the SHAs pinned below. No worker source was modified; no trial-merge result was checked out anywhere.

---

## 1. Executive summary

1. **No integration is executable tonight** and none was attempted. Three independent hard gates fail: (a) #287 Workbench Tier-2 is **NOT QUALIFIED** (verifier FAILs at the pinned candidate; #317 independent review returned NEEDS CHANGES with 4 blocking findings); (b) #309/#314 close **NEEDS CHANGES (narrow)** with two residual false-green classes and the one-loop budget spent; (c) ZX20 storage cannot host the required second cold Unity Library (21.62 GiB free; ~6.8 GiB Library would leave ~14.8 GiB, below the 15 GiB hard floor).
2. **The Git ancestry is workable**: a trial merge of the two approved/qualified-source feature branches is conflict-free, and the third (CTA) contributes a 4-file verification delta that layers cleanly on top. **But a naive branch merge of the CTA branch is forbidden** — it carries historical PR #300 Objective UI ancestors (see §3.3). The safe assembly order is specified in §3.4.
3. The 15-gate A–O matrix is **entirely NOT RUN** on any integrated candidate and must be repinned to the eventual exact candidate HEAD (§4).
4. Seven classes of Manager #200 decisions are required before any integration claim (§6). The morning action list is in §7.

---

## 2. Exact lineages and acceptance manifest (verified SHAs)

All SHAs below were re-read from the canonical store on 2026-11-07 06:4x–07:0x UTC after `git fetch`. "Accepted" reflects Manager #200's published verdicts only; worker/reviewer self-claims are labelled.

| Lineage | Branch | Tip SHA (origin) | Base / merge-base | Acceptance state | Carried into integration? |
|---|---|---|---|---|---|
| D-072 Stream A source: #295 Objective cues + #297 Full Stand Map headland/feeder + #296 five-rack diagram + #303 headland checker + #308 verifier repair | `feature/scenario-one-next-action-guidance-v1` | `9789369d02963b4444f8530b071413d066bf1ddf` | `e1be15c1` (old main) | **Manager SOURCE-APPROVED** (D-072 ruling; #310 independent combined review accepted source-only). Frozen feature lane. | **YES — full branch** (31 files vs base) |
| #284/#287 Workbench retirement (source `8b1946dc` source-approved; verifier `5085e8af`; result doc `4d95e452`) | `feature/scenario-one-workbench-retirement-20261105` | `4d95e45245d660e24b91e26754d9c57949bf3384` (code at `84…`→ see note) | `50042bbe` (which lies **inside** the guidance lineage) | Source (#284) **Manager-reviewed accepted**; verifier commit **NOT QUALIFIED** (#287 Tier-2 FAIL at `5085e8af`; #317 NEEDS CHANGES, 4 blocking) | **HOLD** — see §6 D1/D2. If accepted later: 6 production files + scene + 4 verifier files + 2 docs |
| #299/#304/#309 CTA checker (verification-only delta) | `verify/scenario-one-feedback-cta-anchor-20261106` | `753143e62a5cf1d7436874e731766c4893a4c7c7` (code `e29e3e1b`, loop-1 on top of `84d0ba92`/`b4d47d3d`) | `e1be15c1` | #309 worker-complete + one authorised repair loop; **#314 exact-head re-review: NEEDS CHANGES (narrow)** — 2 residual classes; Manager verdict pending | **VERIFICATION DELTA ONLY** (4 files, §3.3) |

Note on workbench tip: `4d95e452` = result-doc commit on top of verifier `5085e8af` on top of source `8b1946dc` (= `50042bbe`…`debf4d6c` lineage). The unqualified verifier commit was published to origin for provenance (fast-forward only) — ratification pending (§6 D6).

**Excluded by contract:** old PR #265 docs branch (never wholesale-merged), draft PR #300 (historical Objective UI ancestors — never wholesale-merged), rejected D-071 spine layouts, main merge of any feature branch, and any unreviewed nearby HEAD. No candidate may be upgraded "by inference" to a different HEAD.

---

## 3. Ancestry / diff / conflict analysis (read-only)

### 3.1 Topology

```
e1be15c1 (old main) ──┬───────────────────────────────► guidance tip 9789369d   (31 files)
                      └─ cta verify branch ───────────► cta tip 753143e6        (16 files, mixed: see 3.3)
50042bbe (inside guidance history) ─► workbench tip 4d95e452                     (12 files)
```

- `merge-base(main@b5c89dfc, guidance) = merge-base(main, cta) = e1be15c1`; `merge-base(guidance, workbench) = 50042bbe`. Main itself has advanced past `e1be15c1` with coordination/docs work (e.g. this overnight packet's own commits), so **main is not a silent integration target**; the packet forbids main merges anyway.

### 3.2 Trial merges (`git merge-tree --write-tree`, no checkout performed)

| Pair | Result | Interpretation |
|---|---|---|
| guidance × workbench | clean (tree `5ef4da64`) | No textual conflicts. Workbench touches 6 production files + scene + its verifiers; guidance touches UI/map/diagram + its verifiers. Disjoint at hunk level. |
| guidance × cta | clean (tree `b55094b1`) **BUT SEMANTICALLY UNSAFE** | Textually auto-merges, yet would silently fuse historical PR #300 UI ancestors with the approved guidance UI. **This path is forbidden** (§3.3). |
| workbench × cta | clean (tree `bb9e22ee`) | Same caveat. |

Textual cleanliness ≠ approved integration. The conflicts that matter here are **authority conflicts**, not merge conflicts.

### 3.3 The CTA branch hazard (highest-value finding of this dossier)

The verify branch's delta vs `e1be15c1` is **16 files**, of which only **four + handoff docs** are the reviewed checker work:

- Reviewed verification delta (the only transferable content): `Tools/Verification/OvernightPlaytestFeedback/{objective_cta_source_check.py, test_objective_cta_source_check.py, check_baseline.py, README.md}` (+ the RR1–RR4 handoff docs).
- **Historical PR #300 Objective UI ancestors — MUST NOT transfer:** `Assets/ForestPrototype/UI/{LearningObjectivesView.cs, ScenarioOneUiRoot.cs, StandMapView.cs, WorkPlanView.cs}`, `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss`, `Tools/Verification/NextActionGuidance/check_sources.py`, and the older BASELINE-*/OBJECTIVE-* handoffs. Five of these files are **also** touched by the approved guidance branch — exactly the overlap a naive merge would silently reconcile in favour of unreviewed ancestors.

This matches the standing Manager rule: *"transfer only reviewed verification delta to exact integrated candidate and rerun later"*.

### 3.4 Recommended assembly order (for the future integrator, after Manager release)

1. Start from `9789369d` (approved Stream A source) on the newly Manager-registered integration branch/worktree.
2. Merge `feature/scenario-one-workbench-retirement-20261105` **only after #287/#317 Manager disposition** (clean merge proven; 6 production files + scene + verifiers + docs).
3. Apply the CTA checker delta as an explicit **file-level transfer** (`git checkout 753143e6 -- <4 checker files + new handoff>`), never `git merge` of the verify branch. Re-run the full checker at the integrated HEAD; the checker must accept the integrated source and keep the historical baseline red.
4. Record the exact integrated SHA; all A–O evidence must reference that SHA and no other.

---

## 4. A–O qualification readiness (binding 15-gate matrix)

Source: [FOCUSED-PLAYTEST-QUALIFICATION-BROWSER-MATRIX-20261106](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/FOCUSED-PLAYTEST-QUALIFICATION-BROWSER-MATRIX-20261106.md). **Every gate is NOT RUN on an integrated candidate — no candidate exists.** Antecedent evidence available tonight is noted so the integrator does not re-derive it.

| Gate | Antecedent evidence in hand (not a PASS) | Hard remaining requirement |
|---|---|---|
| A Import/compile | #287 cold import+compile **PASS at `5085e8af`** (with package-bump caveat) | Compile at the **exact integrated HEAD** |
| B V2 surface lifecycle | none | Full lifecycle on integrated candidate |
| C Map geometry | #297 source-approved; offline geometry | Paired Full-map + 3D captures 720p/1080p, m-2 dark-edge check |
| D Full-vs-Mini | #297 offline tests | Rendered side-by-side same saved world |
| E Legend/paths | #297 source; m-5 open | Two-res colour/grayscale + observer identification |
| F Performance | none | Profiler allocations at still/map interactions (m-1) |
| G Five-rack diagram | #308/#310 four suites PASS (source); pins both diagram files | Authentic lesson render colour/grayscale at both resolutions |
| H Old-world diagram guard | B-1 guard source-checked | Actual Legacy40 / frozen Reference / pre-V2 screens |
| I Card fit / cues | #295 source-approved | Measured card bounds at 1280×720 and 1920×1080, native captures |
| J Native controls | #287's RackV2Objective1EndToEnd **PASS 173 checks, 24 real captures** (different candidate!) | Re-run at integrated HEAD; focus/consent/safe-default evidence |
| K Work Plan / MFM | #287 MfmCoverageWorldGate **PASS 57** incl. 149-task Work Plan flow (same caveat) | Re-run integrated; MFM non-commitment boundary |
| L Workbench retirement | #287 **FAIL/NOT RUN** (lifecycle gate false-red suspected; C–G NOT RUN; tree substitution unresolved) | Requires Manager-approved #287 repin + re-qualification first |
| M CTA checker integrity | #309 at `753143e6`: 275/275 mutations, 10/0/2, 6/4/2, SnapFar SDK8.0.133; **#314 NEEDS CHANGES (narrow)** | Manager ruling on N-1/N-2 residuals; full numbered checker on integrated HEAD |
| N Save/load | #287 partial (Reference bytes unchanged; legacy40 NOT RUN) | Full old-save/Legacy40/frozen-reference at integrated HEAD |
| O Final integrated | — | Independent integrated review (#316) + Manager verdict |

---

## 5. Storage / lock / capacity feasibility (live measurements)

| Observation | Value | Consequence |
|---|---|---|
| ZX20 free (after #287 cold import) | **21.62 GiB** | Below the 27 GiB new-import admission |
| Cold Library size (measured, #287 lane) | 6.6 GiB | A second cold Library ⇒ ~14.8 GiB free |
| CCF-Ops hard floor | 15 GiB | **Second cold import is INFEASIBLE** — would breach the floor |
| #287 lane Library | warm, bound to `4d95e452`'s parent tree | Reusing it for another HEAD implies a HEAD switch — **forbidden without Manager repin** |
| Unity lock | free all night after 01:49Z (verified repeatedly) | No lease conflicts; but see lane-dirty note |
| #287 lane cleanliness | **DIRTY** (Editor rewrites: `Packages/manifest.json` + `packages-lock.json` SDK 1.1.0→1.1.1, `.vscode/settings.json`, 5 SectionFive `.mat`) | `unity-lock acquire` will refuse this lane until Manager-directed resolution |
| Files (ext4) | ~54–56 GiB free | Evidence storage adequate |

**Feasibility verdict: WAITING STORAGE.** Options for Manager #200 (none self-executed): (a) explicit authority to delete *specific* inactive caches/Libraries (no blanket preauthorisation exists); (b) repin + reuse a warm Library (requires exact-HEAD planning); (c) defer integration Unity to a prepared machine/day. Integration planning must also reserve space for evidence and the post-#287 residual.

---

## 6. Missing Manager #200 decisions (all outstanding at handback)

- **D1 — #287 disposition:** the verifier FAILs at `5085e8af` trace to verifier code (lifecycle check-36 assertion demands all prerequisites met; Enlarged80/RackV2Persistence stale v23 pins — one proven pre-existing on `8b1946dc`). Decide: verifier-only repin + short re-run (worker diagnostics suggest small corrections; **not approved code**), or accept partial evidence. Also: rule on the **package-bump provenance** (evidence ran on SDK 1.1.1), the **dirty-lane resolution**, and the **#284 stale hash labels** vs measured anchors (`94589C749852F359` / `92F9635566BF6AFE`; new Enlarged80 `71481A025AB269B2` / `044C4578E3E8DBA9`).
- **D2 — #317 blockers (4):** package provenance; incomplete lifecycle/persistence qualification; unresolved Enlarged80 tree-identity substitution (possible hash nondeterminism); runner false-green handling (reproduced without Unity). Reviewer: **"a version repin alone is insufficient."**
- **D3 — #309/#314 disposition:** two options from the reviewer: (a) accept the checker as an accidental-regression guard and reword the two overstated README claims ("Each field declaration is pinned"; "only for an unconfirmed Rack V2 Objective 1" — the latter already corrected in loop 1), or (b) hold until two small guards (N-1 interpolation-hole tuple writes; N-2 decoy pin-binding) are written and re-reviewed. The scratch guards closed all 57 new cases but were fitted post-hoc (not approved). Residual disclosures: `W-DEP-05` out-of-scope game-progress logic; L-01 view-member alias route; O-05…O-09 accepted over-rejections.
- **D4 — Integration lane + capacity plan:** register exact branch/worktree/owner in `active-tasks.md`; choose a §5 storage option; without this, integration Unity cannot lawfully start.
- **D5 — A–O matrix repin** to the eventual exact integrated candidate HEAD (§4).
- **D6 — Push-provenance ratifications:** the unqualified `5085e8af` + result doc were published to origin (fast-forward, for GitHub provenance); the CTA verify branch was published through `753143e6`. Both were disclosed; ratify or censure.
- **D7 — Provider/routing policy:** three separately-exhaustible OpenAI quotas died tonight (ChatGPT account, Codex instance, OpenCode-delivered OpenAI), forcing the disclosed same-provider-family reviewer fallback for #314. Decide whether such fallbacks remain acceptable for independent reviews.

---

## 7. Next executable tasks (morning)

1. **Manager #200** rules D1–D7 above using: ledger `MIMO-OVERNIGHT-EXECUTION-LEDGER.md`, result `MIMO-OVERNIGHT-EXECUTION-RESULT.md`, and the four chain reports (#287 result + #317 review; #309 handoff + #314 report incl. exact-head section).
2. If D1 = repin: new #287 verifier-only repair + Tier-2 re-run on a Manager-repinned HEAD with resolved lane cleanliness and a §5 storage plan.
3. If D3 = option (b): one scoped #309 guard repair + exact-head re-review (new budget; tonight's single loop is spent).
4. Register the integration lane (D4), then run the §3.4 assembly + A–O matrix under a real exclusive lease, then #316 independent review.
5. Only after Manager acceptance of all of the above: user personal playtest handback. **Windows/Player build requires separate explicit user consent afterwards.**

---

## 8. Scope and honesty statement

This dossier is read-only preparation by the orchestrator. It contains no acceptance, no integration candidate, no Unity evidence and no waivers. Every "PASS" mentioned is scoped to the exact candidate that produced it and is not transferable to any future integrated HEAD. Frozen Reference Future v1, Legacy40, old saves, `active-tasks.md`, the decision log and all Manager files were untouched; the unrelated dirty work in `/media/jer/Files/CCF-Primary` remains preserved.
