# scen1 — Independent #499 fix-only review of the #498 deadwood-provenance and header correction

**14 November 2026 | Verdict: APPROVED — DRAFT QUALITY ONLY.**

The #498 author correction resolves both required #496 findings (N-1 source/deadwood provenance, N-2 stale header/gate text) and folds in advisories N-3–N-5 without changing accepted Option A, the 115 unchecked requirements, OPEN-C1–C10 or any canonical/protected surface. The four DRAFT files now separate what #490 actually measured from what current source expects and from what remains native-unverified. **No author repair is required.**

This verdict is a documentation-quality judgement only. It is **not** product approval, Unity/native verification, a D-082 canonical decision, a save/router decision, permission to implement, or authorisation for a Windows build.

## 1. Registration and independence

| | |
|---|---|
| Claim | [#499](https://github.com/jermur788/CCF/issues/499), registered `[RUNNING]` before any write ([registration comment](https://github.com/jermur788/CCF/issues/499#issuecomment-6877493621)) |
| Actual reviewer | **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`) via an OpenCode agent in **T3 Code**, provider `opencode-go`. This is **not** the suggested Claude Opus / GPT High class and is registered as-is, as #496 registered its actual model. |
| Independence | Different session from the #495/#496/#498/#490/#484 authors. I authored none of those artefacts and had no earlier part in this scen1 chain. No model-family claim is made; capability/independence are session-level. |
| Starting main HEAD | `fc18190b976f1363f1b5d1b0881810db2f8c6e39` (`origin/main`, 2026-11-13T20:05:32Z) |
| Reviewed candidate | `docs/scen1-497-deadwood-provenance-draft-repair-20261114-v1` @ `b0d182fa1cf41d0c709ecaa177024c5e745dde40` |
| Comparison baseline | `docs/scen1-planting-led-compact-draft-20261114-v1` @ `a16a987c46bd93689ac60f2a5f621c4c9862d3a4` (#495) |
| Review branch | `docs/scen1-499-deadwood-fix-only-review-20261114-v1` (checked absent locally and on `origin`, then created from the start HEAD above) |
| Worktree / Unity | Docs-only worktree. **No Unity, Editor, machine lease, Player, save or production file was used or written.** Local tools were used read-only for source and published evidence. |
| Live coordination | `Docs/Project/coordination/active-tasks.md` checked: no #499 row, writer, lane or lock. Not edited. |

## 2. What I did and did not do

**Done (static/document and read-only evidence inspection):**
- Diffed the #498 branch against #495: exactly four modified DRAFT Markdown files plus one added handoff.
- Read all four corrected DRAFT files, including every changed hunk in full context.
- Machine-counted the checklist and compared requirement text row by row against #495.
- Read the original #496 review, the #495 packet, the #490 published report and the #490 Manager verdict.
- Read current-main source at `fc18190b`: `ForestTree.cs:547–563`, `ScenarioOneManager.cs:498–555`, `ForestEcologyController.cs:429–437, 1059–1150`, Storms windthrow caller, and `SitkaGrowthModel.cs` growth-model constants.
- Read-only inspection of the published #490 evidence: executed harness copy (`harness/Scen1LongHorizon490Native.cs`, SHA-256 `c7291f7a2a09afafa324769838b696178b9b00dd62b8e08ee0fbcb81b65126cb`, equal to the report pin), plus `armA/armB-scenario-state-y40.json` and `armA-snapshot-y40.json`.
- Read the live GitHub comments recording the #490/#424 erratum and #498 closure.

**Not done:** Unity or any native replay, #490 142-check re-execution, PNG pixel inspection, the full 115-requirement test suite, old-save/Reference hash replay, canonical D-082/#471 review, save/router review, Windows build. Dashboard and Drive/context mirrors: **NOT VERIFIED**; #424 retains operational sync.

## 3. Mandatory check results

| # | Focused question | Result |
|---|---|---|
| 1 | **N-1:** Do all four DRAFT references distinguish measured #490 mortality from the disabled-manager harness and the expected normal event→deadwood route, keep record/replay native-unverified, and stop claiming the game has no deadwood? Does the cited handler exist and is it correctly gated? | **PASS.** See §4. |
| 2 | **N-2:** Are header date/author/evidence and S-21 current; do C0-02/C0-06 use OPEN-C1–C10 and the 115-item count; is the stale blanket research gate gone? | **PASS.** See §5. |
| 3 | **N-3:** Are the #490 Oak/Beech protection status, Oak-specific crown values and the Manager's PNG-inspection limit accurate? | **PASS.** See §6. |
| 4 | **N-4:** Is the hedgerow seed-donor position labelled an unverified assumption needing research rather than a fact or approved mechanism? | **PASS.** See §6. |
| 5 | **N-5:** Are the planting plurality and "two alternatives" labelled Manager DRAFT inference with OPEN-C9 still open to the owner? | **PASS.** See §6. |
| 6 | **Scope integrity:** Exactly four modified DRAFTs plus one added handoff; 115 unique unchecked requirements; R-001–R-115 otherwise untouched; Option A/OPEN-C1–C10/old-world protection unchanged; no canonical, source, save or coordination change. | **PASS.** See §7. |

## 4. N-1 — source death event, harness distinction and deadwood provenance

### 4.1 Current-main source path (verified at `fc18190b`)

1. `ForestTree.cs:551–563` — `ApplyMortality(cause, year)` rejects a non-living tree and invalid/incomplete input, then sets `biologicallyDead`, `mortalityCause` (trimmed), `mortalityYear`, clears marks and chop progress, calls `gameObject.SetActive(false)`, and **then raises static `ForestTree.MortalityApplied`**. `RestoreMortality` (566–582) restores the persisted state **without** raising the event.
2. `ScenarioOneManager.cs:510–518` — `OnEnable` subscribes `OnTreeBiologicalDeath` to `ForestTree.MortalityApplied`; `OnDisable` unsubscribes it.
3. `ScenarioOneManager.cs:520–555` — `OnTreeBiologicalDeath` returns unless the ecology exists and either `GrowthModelVersion >= GrowthModel.SiteClassDensity` or windthrow v1 with cause `windthrow`. It computes biological volume, returns early at volume ≤ 0, then creates one `ScenarioDeadwoodRecord` with deadwood ID, tree ID, species ID, position, cell, original/remaining volume, original height/diameter, fallen year and decay year; adds it, and spawns the fallen-log visual (deferred for storm resolution).
4. Growth-model constants: `SitkaGrowthModel.cs:17–18` defines `SiteClassDensity = 1` and `SpeciesAware = 2`; `ScenarioOneManager.cs:503` sets `NewGameGrowthModel = GrowthModel.SpeciesAware`. A new game therefore meets the handler gate.
5. The annual density path reaches this event: `ForestEcologyController.ApplyAdultDensityMortality` (`:1062`) → `KillDensityVictims` (`:1138`) → `tree.ApplyMortality("self-thinning", ecologicalYear)` (`:1147`). Storms likewise call `ApplyMortality("windthrow", …)` (`ForestEcologyController.Storms.cs:242`).
6. `ForestEcologyController.OnTreeMortality` (`:429–437`) only removes the tree from living competition/growth/seed maps and queues a living-canopy rebuild. It does **not** replace or suppress the Scenario One manager's deadwood handler.

### 4.2 What the #490 fixture actually did

- The executed harness copy in the published evidence directory hashes to the report pin (`c7291f7a…65126cb`).
- `Scen1LongHorizon490Native.cs:77` executes `manager.enabled = false;`. In Unity this invokes `ScenarioOneManager.OnDisable`, which unsubscribes `OnTreeBiologicalDeath`, so no deadwood records or logs can be produced by that fixture **regardless of how many biological deaths occur**.
- The harness still drives years through `manager.AdvanceYear()` (lines 111, 112, 115, 118, 232); the manager instance remains callable while its component subscription is gone.
- Measured evidence is consistent with that mechanism: `armA-scenario-state-y40.json` and `armB-scenario-state-y40.json` both contain `deadwoodRecords: []` and `nextDeadwoodId: 1`; `armA-snapshot-y40.json` records no dead tree flags. The report's ~106/100 original Sitka deaths are aggregate measured losses.

### 4.3 Accuracy of the corrected DRAFT

- `spec.md` §1A now says individual causes and normal-play deadwood were **not measured in that fixture**, states that `ScenarioOneManager.enabled=false` invokes `OnDisable` and unsubscribes the event, describes the enabled growth-model ≥1 source expectation as intended behaviour, calls the reconciliation source/reviewer-derived, keeps native enabled-manager behaviour **UNVERIFIED**, and forbids inventing a log or asserting deadwood is absent in production.
- `plan.md:22` requires a small enabled-manager witness inside planned future qualification, not a #490 rerun, and keeps source/native tiers separate.
- `tasks.md` C0-07/C0-08 and the risk stop carry the same distinction and fold a single focused witness only if the approved work needs it.
- `checklists/requirements.md` R-111 asks for the native enabled-manager proof (once-only record/log, ID/species/cause/year/position, persistence, no duplicate/phantom) and explicitly says the #490 zeros do **not** establish normal-play absence.
- No corrected line claims that an enabled manager is guaranteed to produce a record for every conceivable death; the zero-volume early return and the storm branch are not misrepresented as universal.
- The earlier unsupported "hidden death / no player-visible deadwood" framing is gone from all four files; the guard against fabricated logs is retained.

**Conclusion:** the N-1 repair is accurate, correctly tiered and does not overclaim. The measured data and the source expectation are both preserved without converting either into a native PASS.

## 5. N-2 — header, evidence basis, gate sequence and counts

- `spec.md:3` records 14 November 2026 as the update date, attributes product authority to the owner/Overall Manager #425 and the Scenario One documentation authoring to Manager #424 via #495 plus the bounded #498 correction, and retains DRAFT status.
- `spec.md:5` lists the full evidence chain (#453/#459, #460, #462/#465/#467, #484, #490, #495, #496, #498) and correctly marks the normal-manager-enabled death/log witness and final acceptance as not verified.
- `spec.md:6` includes #496 NEEDS CHANGES, #498 author repair and the still-required separate narrow non-author correction check before canonical/owner/save decisions and formal APPROVED status.
- `spec.md` S-21 step 3 reflects #495 → #496 → #498 and the short non-author delta review, and forbids a renewed generic review or any claim that #490 validated normal deadwood.
- `tasks.md` C0-02 now names OPEN-C1–C10 with Option A establishment accepted and cohort size/placement/paired replay not approved; C0-06 says **115-item**, not 104.
- `tasks.md` adds a #496/#498 correction-status block; `plan.md` adds a correction chronology; the checklist header names #498 and retains the DRAFT/no-native-PASS statement.
- Searches found no remaining "104-item" gate, no "#462/465 only research" blanket gate, and no stale "13 November 2026" date outside the explicit "originally drafted 13 November" history.
- The four files remain internally consistent about the correction and about the fact that the DRAFT is still unapproved.

## 6. N-3, N-4 and N-5 advisories

- **N-3 PASS.** Against the #490 report: the Oak was owner-planted and sheltered (Year 2), the Beech contractor-planted and unsheltered; the Oak-specific crown radius was 0.69 m (Y10) → 0.33 m (Y20) → 0.45 m (Y40) at 18.1 m, while the Beech Year-40 radius was 1.04 m. `spec.md` §1A and S-12A now state those values, scope the narrow crown to Oak, and no longer generalise it to all broadleaves. `plan.md` records that the Manager read published descriptions but **did not inspect local PNG pixels**, that the worker's individual image inspection was limited, and that one "1080p twin statistics" label is not a full visual witness. This matches the #490 report §7 and the #490 Manager verdict §1.
- **N-4 PASS.** The "Owner-approved site framing" heading is now "Owner-permitted site framing, provisional implementation interpretation"; hedgerow viability is **UNVERIFIED / RESEARCH REQUIRED**, not an assumed site fact; no external-donor mechanism is approved. This matches the Manager's statement that a generic hedgerow need not contain fertile Oak/Beech and may carry other species.
- **N-5 PASS.** R-107 now opens with **MANAGER DRAFT INFERENCE, not an owner-accepted planting count or layout**. S-12A labels the "at least two options" comparison as **MANAGER DRAFT INFERENCE, not an owner-accepted count or design**. OPEN-C9 remains the owner decision; no count, ratio, layout, compulsory treatment or paired-run design is ratified.

## 7. Scope, owner decision and open-decision integrity

- `git diff --name-status #495..#498` = **A** `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-MANAGER-498-DEADWOOD-PROVENANCE-AND-HEADER-DRAFT-REPAIR-20261114.md` and **M** the four compact DRAFT files only. No `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/`, `Docs/Project/`, other specs, saves, live coordination or source branch content appears.
- Checklist rows: **115** `- [ ] R-…` rows, IDs contiguous and unique R-001–R-115, **zero** checked boxes anywhere in the four files. No requirement was added, removed or renumbered by #498.
- Requirement text compared row by row against #495: only **R-107** (N-5 label) and **R-111** (N-1 gate) changed. R-001–R-106 and R-108–R-115 are textually identical; R-049's earlier #495 rewrite is retained without further change.
- The `CONFIRMED OWNER INTENT — BOUNDED` Option A line is byte-identical to #495. R-105 and R-106 are unchanged. The accepted planting-led Oak and Beech establishment decision, the no-required-natural-recruit-by-Year40 boundary, D-059/D-060 and the three-species/full-loop framing are preserved.
- No hidden ratification: 30 m geometry, cohort count/placement, compulsory felling, fixed Year-40 completion, funding, hedgerow donors, twin-run saves (OPEN-C10) and the D-082 canonical patch remain open or explicitly unratified.
- OPEN-C1–C10 remain present and unresolved apart from the accepted establishment method; OPEN-1–OPEN-6 remain undecided. No premature closure of owner/gate decisions was found.
- Canonical integrity: `Docs/Project/decision-log.md` on `origin/main` still contains **no D-082 entry**; no canonical, milestone, brief or overview file changed in the branch. Protected old-world text (S-03, S-22, Legacy40/Enlarged80, model-0, frozen Reference Future v1) is retained. These are **text-preservation checks only**; no old-save or Reference hash was run.
- The #498 handoff's claimed erratum trail exists: the #496 source/harness erratum is published on [#490](https://github.com/jermur788/CCF/issues/490) and relayed on [#424](https://github.com/jermur788/CCF/issues/424) without rewriting the executed #490 report or the historical Manager verdict. The #498 issue is `[HANDED-OFF]` closed; #499 was open and unclaimed at my registration.
- A Manager source assumption was not treated as native evidence anywhere in the corrected DRAFT: R-111 and the plan keep the enabled-manager witness as **NOT RUN** until a future authorised fixture executes it.

## 8. Non-blocking observations (no repair required for this verdict)

1. **R-111 field ownership precision.** R-111's witness list reads "correct ID/species/cause/year/position". In current source, position/species/ID/fallen year live in the `ScenarioDeadwoodRecord`, while cause and mortality year persist on `ForestTree` (`MortalityCause`/`MortalityYear`), not in the record. The preceding sentence states the tree-level persistence, so the combined intent is readable; a future witness task should still name record fields and tree fields separately.
2. **R-111 subscription wording.** "when `ScenarioOneManager` is enabled, `OnTreeBiologicalDeath` subscribes" is slightly imprecise: `OnEnable` performs the subscription; `OnTreeBiologicalDeath` is the handler. The spec and plan wording is correct.
3. **Out-of-scope source-comment staleness.** `ForestTree.cs:547–550` still says "Explicit foundation API only: no annual mortality trigger calls this", but current annual density mortality (`ForestEcologyController.cs:1147`) and windthrow (`ForestEcologyController.Storms.cs:242`) do call it. This stale comment plausibly contributed to the original misreading. It is outside the four-DRAFT allowlist and **no code change is authorised or made here**; report to #424 with any future source fix.
4. **Conservative attribution in `plan.md`/`tasks.md`.** These say the harness "reportedly" disabled the manager. I verified it directly in the published executed harness; the under-claim is safe and may be strengthened in later copy if desired.
5. **Growth-model gate nuance.** The DRAFT phrases the handler gate as growth model ≥1; the source also admits the configured windthrow exception. This does not affect the self-thinning death gate being described and is not a defect.

## 9. Verdict and authority boundary

**APPROVED — for DRAFT documentation quality only.**

- Required #496 N-1 and N-2 repairs are present, source-accurate and consistently applied across all four files.
- Advisory N-3–N-5 are addressed accurately.
- The 115 requirements remain unique and unchecked; the owner-approved planting-led Option A decision is preserved; OPEN-C1–C10, OPEN-1–OPEN-6, D-059/D-060 and old-world protections are unchanged; canonical D-082 remains unpublished and with #425/#471.
- #424 may proceed to owner-facing cohort/site/management decision preparation from #490 evidence, keeping the source-vs-harness erratum visible and promising no planting success or natural recruitment.
- Any implementation, Unity/native witness, save-router work or Windows build still requires its own approved scope, ownership and gates. The R-111 enabled-manager deadwood witness remains **NOT RUN** and must not be described as verified.

**Not authorised by this review:** implementation, a Unity worktree or native run, closing D-082, any canonical Decision Log/Current Milestone/brief/overview or live coordination edit, save or router changes, historical #490 evidence mutation, or a Windows build. This is documentation quality only, not product approval or acceptance.

## 10. Publishing evidence

This review adds exactly one new Markdown path on `docs/scen1-499-deadwood-fix-only-review-20261114-v1`, created from `fc18190b976f1363f1b5d1b0881810db2f8c6e39`. GitHub readback and the one-file comparison against the starting revision are recorded on [#499](https://github.com/jermur788/CCF/issues/499). No other file was written. Dashboard and Drive/context mirror: **NOT VERIFIED**; #424 retains operational sync.
