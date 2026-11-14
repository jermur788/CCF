# Independent review — Irish Sitka 2D Reference Laboratory V1 four-file DRAFT

- **Review task:** [#485](https://github.com/jermur788/CCF/issues/485) (independent non-author review)
- **Date:** 14 November 2026
- **Reviewer (actual worker):** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code session in T3 Code. GitHub/git read access plus one docs-only write. **Worktree NONE**: no checkout switch, no Unity/Editor, no host or Ops lease, no code run, no model run, no dataset download. Registration: [#485 comment](https://github.com/jermur788/CCF/issues/485#issuecomment-6869319930).
- **Independence:** the draft author was OpenAI GPT-6 under [#483](https://github.com/jermur788/CCF/issues/483). This reviewer is a different provider, model and session, and did not contribute to the draft.
- **Reviewer branch:** `docs/multirack-sitka-lab-independent-review-result-20261114-v1`. Starting HEAD `fc18190b976f1363f1b5d1b0881810db2f8c6e39` (= `origin/main` at registration, 0 ahead / 0 changed).
- **Binding packet:** [MANAGER-SITKA-2D-LAB-V1-DRAFT-INDEPENDENT-REVIEW-PACKET-20261114.md](https://github.com/jermur788/CCF/blob/docs/multirack-sitka-lab-independent-review-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-SITKA-2D-LAB-V1-DRAFT-INDEPENDENT-REVIEW-PACKET-20261114.md) (packet branch HEAD `fd6038d9`).

## Verdict

**NEEDS CHANGES.**

The scope, the science boundary and the stand arithmetic are sound. **No REJECT-level or scientific blocker was found.** The basal-area, density, Dq and shoelace formulas are correct. The P0 "no biological growth" boundary is honest. The P1 evidence gate matches the canonical GROWFOR/Broad–Lynch limits in the research index. Scenario One and its three-species requirement are protected.

However, a P0 code worker would still have to guess several engineering contracts that the acceptance tests depend on:

- the state-machine transitions;
- the definition of a stale plan;
- whether execution is atomic or filters out ineligible trees (the files contradict each other);
- canonical serialisation, hash and PRNG for the byte-identity test;
- the edge and below-threshold tree rules;
- the save-isolation mechanism;
- who approves the spec.

The project's spec workflow exists to prevent that kind of guessing. These are **bounded text deltas with no new research**. A single author revision plus a short delta check should be enough.

**Owner P0 approval recommendation:** **not yet**. Once the MEDIUM items below (D-01 to D-08) are fixed and confirmed by a delta check, I recommend the designated approver consider formal P0 APPROVED status. This review does **not** ratify the spec, approve P0 or change its status.

**NO AUTOMATIC P1 OR UNITY CODE RELEASE.** Nothing in this review authorises P0 implementation, P1 model work, Unity changes, Scenario One changes or a Windows build.

## 1. Sources and versions inspected

| Item | Ref / version | Check |
|---|---|---|
| Author branch `docs/multirack-sitka-2d-reference-lab-draft-20261114-v1` | **pinned HEAD `754a467c220bde0beef7998256baae785b06fb07`** (commits `9269c561` spec → `593f8f6a` plan → `83d33f36` tasks → `754a467c` checklist) | merge-base with main `fc18190b`. `git diff --stat` shows **exactly 4 added files, 279 insertions, 0 modified/deleted**. Re-checked at publication; HEAD unchanged, so no concurrent author edits were detected. |
| `specs/multirack-sitka-2d-reference-lab-v1/spec.md` | @754a467c, 113 lines | read in full |
| `.../plan.md` | @754a467c, 61 lines | read in full |
| `.../tasks.md` | @754a467c, 43 lines | read in full |
| `.../checklists/requirements.md` | @754a467c, 62 lines | read in full |
| `specs/README.md`, `specs/_template/spec.md` | main @fc18190b | structure and status rules |
| `Docs/Project/AgentWorkflow.md` | main @fc18190b | approval authority (user = product owner; Overall Manager approves specs) |
| `Docs/Project/decision-log.md` | main @fc18190b | D-048, D-059, D-065, D-076, D-078, D-079, D-080, D-081 rows read |
| `Docs/Project/research-index.md` | main @fc18190b | Broad & Lynch 2006a/b records, CCF applicability boundary, mortality wording, D-048 lineage |
| `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md` | main @fc18190b (canonical repo copy) | state `B,N,H`; site index = top height at age 30; no formal numeric validity box; G1 manual top height < 49.3 m and post-first-thinning window; V1B validation flags (`individual_tree_validation: none`, `continuous_cover_validation: none demonstrated`) |
| `Docs/Project/coordination/active-tasks.md` | main @fc18190b, **read-only** | no #483/#485/Multi-Rack lane rows; #466 user-override note that "#424 remains sole routine writer" |
| `Tools/` tree on main and all remote branches | fetched 14 Nov | `Tools/MultiRackSitkaLab/` exists on **no** branch. Standalone Text Editor lives in `Tools/ContentReview/` (feature branches only). No path overlap. |
| CCF Sitka source presence | main @fc18190b | `Assets/ForestPrototype/AdultMortalityAccount.cs` and others exist. Not audited here; that is P1/LAB-06 work. |

Not inspected: the Game Dev attachment copy of the GROWFOR extraction (the canonical repo copy was used instead); the #472/#475/#479 handoff bodies beyond what the packet and draft summarise; the Unity runtime. The packet's mention of a "2024 Sitka efficacy caveat" was **not found** in the canonical extraction. The draft does not rely on it, so this is not a draft defect.

## 2. Independent arithmetic check (R4)

Independently derived, without Irish field assumptions:

- `BA_i = π (dbhCm/200)²` m². DBH in cm ÷ 100 gives metres, and radius is half of that, so `r = dbhCm/200` m. **Correct.** Example: 30 cm gives π·0.15² = **0.0706858 m²**.
- `N/ha = N / areaHa` and `BA/ha = ΣBA / areaHa`, with `areaHa = areaM²/10 000`. **Correct.** The m² to ha conversion should be stated explicitly in A4 (see D-11).
- `Dq = sqrt(Σ dbh² / N)`. **Correct.** It equals the diameter of the tree of mean basal area, and is not the arithmetic mean (REQ-C05 is right).
- Shoelace area `|Σ(x_i·y_{i+1} − x_{i+1}·y_i)| / 2`. **Correct for simple polygons only.** For self-intersecting polygons it gives wrong area silently (see D-07).
- Reproducible oracle: trees 20, 30 and 40 cm, all live, in a 0.25 ha square. ΣBA = π(0.01+0.0225+0.04) = **0.227765 m²**. BA/ha = **0.911062 m²/ha**. N/ha = **12**. Dq = sqrt(2900/3) = **31.0913 cm**. Felling the 40 cm tree removes 0.125664 m², which is 55.17 % of live BA.

**No formula defect, so no blocker under R4.** The R4 integrity items (stale approvals, immutable events, snapshot hash) are covered by D-02, D-03 and D-04.

## 3. Defects (severity-tagged)

Severity: **BLOCKER** means the spec is unsafe or wrong in principle. **MEDIUM** means a coding worker would have to guess a contract that an acceptance test depends on, so it must be fixed before APPROVED. **LOW** means clarity, consistency or template issues that should be fixed but need no re-review on their own.

### MEDIUM

**D-01 — The decision/work state machine has no transition table.** References: `spec.md` §3 A5; §4 bullets 2–3; `requirements.md` REQ-D01/D05; `tasks.md` LAB-03.

A5 lists `UNMARKED → PLAYER_MARKED → PROPOSED → APPROVED → PENDING_WORK → EXECUTED` plus `REJECTED/CANCELLED` and `MANUAL_REVIEW`. It does not say:

- whether status belongs to a **tree** or to a **plan** (the UI text mixes "mark" per tree with "Approve Experiment Plan" per plan);
- which user action moves `APPROVED → PENDING_WORK`, or whether the two are one state;
- whether a tree can sit in more than one open plan;
- what `MANUAL_REVIEW` means in a single-user lab, and how a tree enters or leaves it;
- whether "undo of a pending mark" means `PLAYER_MARKED` or `PENDING_WORK`.

**Required change:** add a small transition table: state × action → next state, guard, and whether a state change is permitted. Separate tree-mark state from plan state. Either define `MANUAL_REVIEW` or remove it from P0.

**D-02 — Execution is described both as filtering and as atomic rejection.** References: `spec.md` A5 ("changes live status to felled, once per unique living eligible tree") vs §4 ("Must reject dead IDs, duplicate IDs, … stale snapshots"); `requirements.md` REQ-D03 ("fell only eligible … IDs") vs REQ-D04 ("fail closed"); `tasks.md` P0 checklist line 6.

One reading filters out ineligible IDs and fells the rest. The other rejects the whole plan. These produce different event logs and different AC-05/AC-06 oracles. **Required change:** pick one. This reviewer recommends **atomic all-or-nothing**: any ineligible, duplicate or stale ID rejects the whole execution, logs a rejection event and leaves inventory unchanged. That is simpler to verify and matches "fail closed".

**D-03 — "Stale snapshot / stale plan" is undefined.** References: `spec.md` §4 bullet 3; AC-06 ("unexpected earlier snapshot"); REQ-D04; plan G3.

The spec does not define the mechanism. **Required change:** a plan records the `baseStateRevision` or `baseSnapshotHash` it was approved against. Execution is rejected if the live state differs. List the events that make an approved plan stale: any other execution, a calendar advance, a fixture reset or a load. State whether a stale plan can be re-approved against current state.

**D-04 — Canonical serialisation, hash algorithm and PRNG are unspecified, but AC-03 requires byte identity.** References: `spec.md` A3 ("seed, input checksum, output checksum"), A5 ("after-state/checksum"), AC-03; REQ-C07; tasks P0 checklist line 2.

Byte-identical JSON across runs, and across reload and replay, needs fixed rules:

- key/property order;
- float formatting (culture-invariant, round-trip-exact, fixed DBH/coordinate precision);
- encoding and line endings;
- tree order (sorted by `treeId`);
- a named hash (for example SHA-256 over canonical bytes).

Also, .NET's seeded `System.Random` is not documented as stable across runtime versions. **Required change:** specify these rules. Require either a small, explicitly specified PRNG implemented in Core, or **checked-in synthetic fixtures as the test oracle**, with the generator as a convenience only. Add an event sequence number and post-event state hash so that replay can be verified event by event.

**D-05 — Edge, out-of-polygon and below-threshold tree rules are left open.** References: `spec.md` A3 ("out-of-bounds points (unless explicit edge ledger)"), A4 (explicit DBH threshold), §4 ("out-of-scope stems"); REQ-C02 vs REQ-C06 ("never counted … without authorisation").

"Edge ledger" and "authorisation" are not defined anywhere. REQ-C02 says every tree is in bounds, while REQ-C06 allows exceptions. **Required change:** in P0, **reject any tree outside the polygon at load** (state whether a point on the boundary is inside, recommended inclusive) and drop the edge ledger from P0. State whether trees below the DBH threshold are displayed, selectable and executable, and whether their removal BA counts. Recommended: display them, exclude them from stand metrics, and make them non-executable in P0.

**D-06 — The save/load isolation mechanism and local-host trust boundary are not specified.** References: `spec.md` A6, §4 ("load P0 local file"), AC-07 ("not searched, read or written"); plan "loopback-only local API"; template sections *Save / backward compatibility* and *Security / write-back* are missing.

AC-07 is untestable as written. A generic "load file" control can read any path, including `forest-save.json`. **Required change:**

1. Store lab data only under a fixed lab data directory inside `Tools/MultiRackSitkaLab/` (or a user-chosen lab folder).
2. Reject any path outside it, including traversal.
3. Require a distinct format marker (for example `"format": "ccf-sitka-lab"`) and `labSchemaVersion`, and reject anything else before parsing the body.
4. Bind the host to `127.0.0.1` only, with no permissive CORS.
5. Add a negative test that feeds a CCF-shaped JSON and an outside path, and expects rejection without a read or write.

Add the two missing template sections: "No CCF save impact — separate format/directory", and the trust boundary above.

**D-07 — Polygon validity rules are incomplete.** References: `spec.md` A4 ("legitimate non-overlapping plot area"); REQ-C01; AC-04.

Shoelace area is silently wrong for self-intersecting rings. **Required change:** P0 polygon is a single simple ring with no holes, at least 3 distinct vertices, finite coordinates, and non-zero area. Either winding is accepted, using absolute area. Self-intersection is rejected. Add a self-intersecting (bow-tie) negative test to REQ-C01 and AC-04.

**D-08 — The approver is unnamed, open decisions lack owners, and #424 authority is stale.** References: `spec.md` §7 ("a delegated overall product owner must explicitly approve"); plan G0 ("Manager records formal APPROVED status"); §7 ("#424 manager retains sole live coordination writer"); OPEN-L1..L5.

`AgentWorkflow.md` names the user as product owner and the Overall Manager (currently ChatGPT Game Dev, reassignable by the user) as spec approver. "Delegated overall product owner" is neither. The #466 user override records "There is no 424" for that task, while still keeping #424 as routine board writer. The spec should not depend on #424 for approving a non-Scenario-One lab. `specs/_template/spec.md` requires each open decision to have "an owner and a decision route".

**Required change:**

- Name the approver as **the user, or the designated Overall Manager acting for the user**. State that the #483 author / Multi-Rack Manager cannot self-approve.
- Name who records any coordination entry: the current board writer under AgentWorkflow, without borrowing #424 Unity authority.
- Give OPEN-L1..L5 each an owner (user / Overall Manager) and a route.
- Mark OPEN-L1, L2 and L3 as **must-resolve before APPROVED**, and OPEN-L4 and L5 as **explicitly deferred, not P0**.

### LOW

**D-09 — The P0 "Advance Year" conflicts with per-tree age.** References: `spec.md` A1 ("age/year"), A6, AC-09; REQ-U07.

If the calendar advances and tree age increments while DBH and height stay fixed, the lab displays an implicitly false biology (older trees, no growth). If age does not increment, age and year drift apart. **Recommended:** in P0, store `ageAtInventory` per tree and never increment it. Label the control **"Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)"**. Record that a calendar advance makes open approvals stale (D-03). **Year-by-year consequences should appear only in P1** with source-qualified biology. This agrees with the draft's intent; the change only makes it mechanically unambiguous.

**D-10 — Risk of a second arithmetic source in the UI.** References: `spec.md` A2, §4 bullet 2, AC-02; REQ-U01.

The draft forbids a second biological store, but preview metrics (N, BA, BA fraction) could still be recomputed in browser JavaScript. **Recommended:** add "the UI performs no forestry arithmetic; all actual and preview metrics come from Core through the local API", with a test comparing UI-displayed values to Core output.

**D-11 — Fixture labelling has a loophole and "game" wording.** References: `spec.md` A3 ("TEST FIXTURE/GAMEPLAY CALIBRATION unless anchored to a particular Irish source"); REQ-P02 ("Irish-context").

The "unless anchored" clause lets a P0 worker insert sourced Irish numbers without review, which contradicts OPEN-L5. "Gameplay calibration" is the wrong label for a non-game lab. **Recommended:** in P0 all values are labelled `ENGINEERING TEST FIXTURE`, and any anchoring goes through the separate OPEN-L5 calibration step. Make the on-screen title "Synthetic Sitka test stand (flat)" so that "Irish" is not read as an evidence claim. State `areaHa = areaM²/10 000` in A4.

**D-12 — Top height in P0.** Reference: `spec.md` A4. **Recommended:** exclude top height from P0 outputs entirely. A defined sampling rule is a P1 item that must come from the GROWFOR/Broad–Lynch source definition, not from the worker.

**D-13 — Economic terms in a no-economy scope.** References: AC-05 ("costs"), §4 ("cash"). Remove them, or state "no cost/cash fields exist in P0".

**D-14 — Template conformance: stop conditions are only in plan.md.** References: `specs/README.md` ("must not silently drop … stop conditions"); spec §1–8. **Recommended:** add a Stop conditions section to `spec.md`, which can reference plan §Stop conditions, and an Evidence classification section (the A8, §5 and L0–L4 content already exists).

**D-15 — Writer-count inconsistency.** `tasks.md` LAB-04 allows a "strictly non-overlap UI subowner", while `plan.md` §Effort says "Do not send to multiple code writers at once". Pick one; a single writer is recommended for P0.

**D-16 — Performance target is unquantified.** References: §4 bullet 5, AC-11. **Recommended:** state a modest, owner-adjustable engineering target (for example "≥ 2 000 trees remain interactive") so that "performance documented" is checkable.

**D-17 — Markdown rendering.** All four files use escaped backticks (`` \` ``), so identifiers display with literal backtick characters instead of code formatting. In `plan.md` the escaped ```` \`\`\`text ```` fence means the package-shape tree **does not render as a block** and collapses into running text. Replace these with plain backticks.

**D-18 — Use the canonical evidence path.** Reference: `spec.md` header "Research". Cite the canonical repository copy `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md` (per `research-index.md`), not only the Game Dev attachment.

## 4. Disposition R1–R9

| # | Criterion | Disposition | Notes |
|---|---|---|---|
| **R1** | Product scope and ownership | **PASS** (LOW D-11) | Lab vs final 3D experience is clearly separated (§1, §2 P0 last sentence, §8 end-state). Scenario One is protected (§2, REQ-P04, AC-01). No Unity or old saves. `Tools/MultiRackSitkaLab/` is new on every branch and does not overlap `Tools/ContentReview/`. No lane, host or `active-tasks` ownership is claimed (header, §7, plan G1). |
| **R2** | Engineering viability | **NEEDS CHANGES** (D-01–D-07, D-10) | P0 is a coherent, testable unit and the layering (A2) is sensible. However, the state machine, atomic vs filter execution, stale-plan definition, canonical bytes/PRNG, edge rules, save isolation and polygon validity would force guesswork. No game-polish demands are made. |
| **R3** | No-growth P0 semantics | **PASS** (LOW D-09) | The trade-off is explicit and well argued (§2 P0, A6, A8, AC-09, REQ-U07, OPEN-L2). The calendar is labelled replay-only. No harvest happens from preview or approval (A5, AC-05). Recommend making age/year mechanics unambiguous and reserving year-by-year consequences for P1. |
| **R4** | Maths and conservation | **PASS on formulas / NEEDS CHANGES on integrity contracts** | All formulas are independently verified (§2 above). Dead/felled trees keep their identity (A4). Immutable executed events (A5). Snapshot hash and replay need D-02, D-03 and D-04 so that the oracles are reproducible by an independent implementer. |
| **R5** | Forestry science limits | **PASS** (LOW D-12, D-18) | §5 matches the canonical research index and GROWFOR extraction: stand-level `B,N,H`; pure even-aged domain; site index = top height at age 30 (verified); no individual-tree, CCF or regeneration validation; British max-density transfer noted (matches D-048/Comeau); "never call a copied pure Sitka height curve GROWFOR-based"; P1 requires current-source inspection, not the historical overview. No P1 coefficients are invented. |
| **R6** | Data, source and rights separation | **PASS** (LOW D-11) | No real plot import (§2 out of scope, AC-10, REQ-P05). `SYNTHETIC TEST FOREST` is mandatory everywhere. GBIF/Teagasc/Coillte data are gated (OPEN-L4). No "typical Irish" N/BA/DBH values are asserted. 100 m × 100 m is explicitly a test choice. Close the "unless anchored" loophole. |
| **R7** | Upgrade and integration boundary | **PASS** | P1 has its own source/parity review and go/no-go (§5, plan G7, LAB-06/07). No "same engine" claim is made. Connected 2D+3D and five-rack transfer remain later specs (§1, A7, AC-10). Consistent with D-065/D-076 and D-059 (three-species Scenario One preserved). |
| **R8** | Proportional tests | **PASS** (with D-04, D-06 tests added) | Tier 1 headless plus one real browser Tier 2, with no Unity/Tier 3/Windows (plan G5), is proportionate. Negative, stale and status cases are listed (AC-06, REQ-D04, REQ-U05). Add the bow-tie polygon, CCF-shaped/outside-path load rejection and UI-vs-Core value tests. P0 is correctly not judged on L4. |
| **R9** | Formal spec structure | **NEEDS CHANGES** (D-08, D-14, D-15, D-17) | All four files exist in the README structure. ACs and the dependency chain (LAB-00→08) are meaningful. OPEN-L1..L5 are not silently ratified. However, the approver is not named correctly, open decisions lack owners and routes, the stop-conditions section is missing from `spec.md`, there is a writer-count inconsistency, and rendering is broken. **Immediately implementable after approval:** LAB-01 to LAB-05 (P0) only. LAB-06/07 stay gated. |

## 5. Owner choices that acceptance requires

These are product choices, not reviewer decisions. Labelled per the packet:

1. **OPEN-L1** (research tool only vs future 2D game mode). Must be resolved before APPROVED. The draft's research-only default is reasonable.
2. **OPEN-L2** (P0 no-growth, P1 gated). Must be resolved before APPROVED. Recommended as drafted.
3. **OPEN-L3** (.NET core + local browser UI). Must be resolved before APPROVED, subject to the LAB-01 tooling audit. A C# core is a reasonable fit because a P1 parity port would come from C# CCF source.
4. **OPEN-L4 / OPEN-L5**. Explicitly deferred; not P0.
5. **D-02** (atomic vs filter execution). The reviewer recommends atomic; the owner may override.

## 6. Recommended next step

The author, or the Multi-Rack Manager on the author branch, applies D-01 to D-08, preferably also the LOW items, as a single revision. A short independent **delta check** (a new task, not this one) confirms them. Then the user or the designated Overall Manager decides on P0 APPROVED status. This review does not ratify anything, and **no P1 or Unity code is released**.

## 7. Write scope

This review adds exactly one file:
`Docs/Handoffs/MultiRack-Simulation-20261114/INDEPENDENT-SITKA-2D-LAB-V1-DRAFT-SPEC-REVIEW-20261114.md`
on `docs/multirack-sitka-lab-independent-review-result-20261114-v1`.

No author files, specs, canonical docs, `active-tasks.md`, source, Unity assets, saves or builds were changed. Dashboard/Drive mirror sync: **NOT VERIFIED**.
