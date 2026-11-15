# MULTI-012: independent four-file DRAFT quality review of the Sitka P1 adult-rules comparator

**Date:** 15 November 2026
**Claim:** [#531](https://github.com/jermur788/CCF/issues/531)
**Binding packet:** `docs/multi-012-sitka-p1-independent-draft-spec-review-dispatch-20261115-v1` @ `d9e02adc`, `Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-SITKA-P1-FOUR-FILE-INDEPENDENT-DRAFT-REVIEW-PACKET-20261115.md`
**DRAFT under review:** MULTI-011 / [#528](https://github.com/jermur788/CCF/issues/528) by OpenAI GPT-6 (Overall Manager), `docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1` @ `50178f9df0641a75547fbeb29033d18a69a5cbb7`

## Verdict

**NEEDS CHANGES.** The changes are narrow and documentation-only.

| Severity | Count | IDs |
|---|---|---|
| BLOCKING | 0 | — |
| MAJOR | 2 | R-1 (numeric/oracle framing), R-2 (save/load equivalence vs P0 approval semantics) |
| MINOR | 8 | R-3 … R-10 |

Overall the DRAFT is careful and honest. Its claims match the CCF source at `fc18190b`, it is clear that it carries no authority to implement, and it keeps P0 isolated. It carries all eight m-1…m-8 findings and keeps all seven C-1…C-7 decisions explicitly OPEN, each with a testable proposal. Its scientific boundary with GROWFOR/Broad–Lynch is correct.

Two issues stop me approving it as decision-ready:

1. **R-1.** The DRAFT recommends "source-faithful binary32" for exact parity. It also treats a "verified pure .NET harness" as an equal alternative to a Unity capture for the P1-01 oracle. The repository itself shows that Unity Mono's float results depend on code generation. A scratch probe on the real P0 fixtures confirms the effect: 52–61 % of first-year DBH increments differ in their binary32 bits between two plausible float semantics. So a .NET route cannot stand in for the Unity runtime, and C-2 cannot be finally decided before the oracle exists. This changes a decision the owner has to make: whether P1-01 needs a scarce Unity lane.
2. **R-2.** Acceptance criteria AC-P1-05 and REQ-E06 require save → load → continue to reproduce the same event hashes "including approvals". The P0 semantics the DRAFT inherits stale every open approval on load, by a recorded event. As written, the criterion fails whenever an approval is open at the save point.

**What this verdict is not:**
- It is **not** spec approval.
- It does not resolve C-1…C-7.
- It does not release P1-01…P1-05, LAB-07, any Unity work or any Windows build.

Per the packet, I do not repair the DRAFT. The Manager rules on these findings and creates any docs-only follow-on.

## 1. Reviewer identity, host, worktree and preflight

| Item | Value |
|---|---|
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness inside local T3 Code. I am not the author; the DRAFT was written by OpenAI GPT-6. |
| Independence caveat | The MULTI-010 reviewer whose m-1…m-8 and C-1…C-7 the DRAFT carries was also a Claude Opus 5.5 session, so here I am judging how my own model family's findings were handled. To offset that, I re-derived source facts rather than trusting MULTI-010. R-1 **partly corrects MULTI-010's own m-4/C-2 recommendation** ("use binary32 with an exact-round-trip contract"). |
| Host | `jer-laptop` (Linux), user `jer` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-012-SitkaP1-DraftSpecReview-20261115`. It was absent beforehand, and I created it for this review. |
| Branch / starting HEAD | `review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1` @ `50178f9df0641a75547fbeb29033d18a69a5cbb7`. This equals the remote review branch and the pinned draft branch (`git ls-remote`). Status was clean at the start. |
| Draft custody | `git diff --stat fc18190b 50178f9d`: exactly five added Markdown files (4 DRAFT + 1 handoff, 487 lines). This matches the Manager's custody record. |
| Registration | #531 was fresh-read as `[READY / NOT RUNNING]`. Live `origin/main` `active-tasks.md` had no MULTI-012/#531 row and no Sitka P1 lock, and no other worktree used the path or branch. I posted [RUNNING](https://github.com/jermur788/CCF/issues/531#issuecomment-6891971573) and retitled the issue **before** inspecting any source. This was a docs task, so no Unity lease was taken. |
| Off-repo scratch | `/media/jer/Files/CCF/Verification/MULTI-012-SitkaP1-DraftSpecReview-20261115/` |
| Primary PDFs | **NOT AVAILABLE on this host.** The only hits for Broad/GROWFOR/Irish Forestry among the T3 attachment PDFs are two Scenario Two species reports, not the 2006 volume. Every GROWFOR claim below is therefore **EXTRACTION ONLY** (extraction V1B plus the Manager's recorded spot check). |

### Sources actually inspected

| Source | Ref |
|---|---|
| Four DRAFT files + MULTI-011 handoff | `50178f9d` (read in full) |
| MULTI-010 independent review | `review/multi-010-…` @ `3d9699dc` (read in full) |
| Manager MULTI-010 verdict | `docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1` (read in full) |
| Approved P0 spec | `docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1` (§2, §3 A1–A8, §5) |
| P0 lab source | `feature/multi-008-…` @ `2659db99`: `Engine.cs`, `LabSession.cs`, `LabFile.cs`, `LabDataStore.cs`, `Models.cs`, `CanonicalJson.cs`, `.gitignore`, `global.json`, `Directory.Build.props` |
| CCF source @ `fc18190b` | `ForestEcologyController.cs` (`AdvanceOneYear`, `UpdateCompetition`, `GrowAdults`, `ApplyAdultDensityMortality`, `StandRelativeDensity`, `RelaxCrowns`, `PromoteCohorts`); `SitkaGrowthModel.cs`; `SimulationRandom.cs`; `ForestTree.cs` (`ApplyGrowth`, `CurrentHeight`); `ForestEcologyCell.cs`; `Species/SitkaSpruce.asset`; `Tools/Verification/BrowsingProtectionVerification.cs`; `Docs/ScenarioTwoSpeciesFoundation.md`; `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Unity/HANDOFF.md` |
| P0 fixtures | from `2659db99` via the MULTI-010 scratch copies (SHA-256 re-checked: demo `d0eed689…`, perf `d12db111…`) |

## 2. Findings

Line numbers refer to the files at `50178f9d`. Every repair listed is documentation-only, in the DRAFT.

### R-1 — MAJOR — "binary32" is not a well-defined parity target, and a pure .NET oracle cannot substitute for the Unity runtime

**DRAFT text affected:**
- `spec.md` l.72 (C-2: "source-faithful binary32 … is recommended for exact parity") and l.114 (C-2 row)
- `spec.md` l.86 (item 2: "**or** an independently verified source-equivalent pure .NET harness")
- `plan.md` l.37, l.66, l.83 ("bit parity wherever obtainable … under a pinned environment")
- `tasks.md` l.11 (P1-01: "or reviewed source-equivalent .NET harness"; dependency "C-2…C-6 resolved")
- `requirements.md` REQ-C02 l.29, and H rows C-2/C-6

**Evidence (DIRECT SOURCE CHECK, `fc18190b`):**
- `ForestEcologyController.cs:990–993`. Routing the Hegyi term through an identical shared function "changed the 80-year anchor hash (7E39B70A14959FAD became DDBBCBDEFAA4E644) even with AggressiveInlining". The same expression gives different bits depending only on where it is compiled.
- `ForestEcologyController.cs:1021–1023`. "the legacy Mono lifecycle anchors depend on the exact floating-point evaluation".
- `Docs/ScenarioTwoSpeciesFoundation.md:143`. An extra local and reordered growth calculations "perturbed the Mono floating-point path" and failed eight reference gates.
- `Tools/Verification/BrowsingProtectionVerification.cs:289–291`. "Mono can retain extra precision in a float product expression while the returned assessment has already rounded to float".
- The P0 lab, the natural home for a P1 adapter, targets **CoreCLR .NET 8** (`global.json` SDK 8.0.133; `Directory.Build.props` `net8.0`). CoreCLR on x64 rounds each float operation to binary32.

**Independent probe (REPRODUCED ARITHMETIC, not Unity parity):**
- **Script:** `scripts/float_semantics_probe.py`, SHA-256 `a5b60a6e…7351683`. Results file SHA-256 `bde9a18a…38b7172`.
- **What it compares:** the source's CI and DBH-increment expressions on the real P0 live trees (centred coordinates) under two semantics:
  - S1: every float operation rounded to binary32;
  - S2: float sub-expressions held in binary64 and rounded only where the C# stores to a float local or field.
- **Results:**

  | Fixture | CI values with different bits | First-year DBH increments with different bits |
  |---|---|---|
  | demo | 38 / 153 | **94 / 153** |
  | perf | 500 / 1893 | **981 / 1893** |

  The maximum relative difference is ≈2.6e-7, about 1–2 ulp.
- Neither S1 nor S2 is claimed to be Unity's actual behaviour. The result shows only that "binary32" alone does not fix the bits. The evaluation semantics must match the runtime.

**Why this matters:**
- (a) "Exact parity" is only meaningful against values captured from the **actual Unity Mono runtime that produces the CCF anchors**.
- (b) A pure .NET harness, even one binding the real compiled CCF assemblies, runs under CoreCLR code generation. It can be "source-equivalent" only after it is cross-checked against a Unity capture on a shared vector set. So it does not remove the need for a Unity lane; it only reduces how long that lane is needed.
- (c) C-2 cannot be finally decided *before* P1-01, which `tasks.md` l.11 requires. Its correct answer depends on what the oracle shows.
- (d) The likely outcome is a reviewed tolerance plus hard branch-margin guards, not bit parity. The DRAFT already sketches that alternative at `plan.md` l.83.
- **Positive note:** the mortality roll is portable. `SimulationRandom.Roll` (`SimulationRandom.cs:41–55`) is pure integer hashing returning an exact 24-bit float, so RNG parity is not at risk. The precision risk lives in CI, DBH, height and crown.

**Smallest repair:**
1. In spec §3/§5/§6, C-2/C-6, plan §4/§5, tasks P1-01 and REQ-C02/F01, name the **parity reference of record** as per-ID values captured from the Unity Editor (Mono) runtime at the pinned source.
2. State that a pure .NET harness counts as an oracle only if it reproduces a Unity capture on an agreed shared vector set.
3. Split C-2 into:
   - **C-2a** (before P1-01): serialisation, units, display rounding, and the capture format, which must record float bits;
   - **C-2b** (after P1-01 evidence): whether the adapter targets bit parity or a tolerance with branch-margin guards.
4. Tell the owner plainly that P1-01 probably needs a bounded Unity lane, which competes with Scenario One for Unity time.
5. Record that the integer RNG roll is runtime-portable.

### R-2 — MAJOR — Save/load equivalence "including approvals" conflicts with P0's session-epoch approval semantics

**DRAFT text affected:**
- `spec.md` AC-P1-05 l.129 ("same … death set and event hashes as uninterrupted runs")
- `plan.md` l.85 ("must agree in ordered per-tree state/event hashes … plan staleness")
- `requirements.md` REQ-E06 l.59 ("equals two uninterrupted years, including death IDs, approvals and rolls")
- `tasks.md` l.63

**Evidence (DIRECT SOURCE CHECK, P0 @ `2659db99`):**
- `LabSession.cs:75–85`: "Every approved/pending plan becomes STALE by a recorded event" on load. It appends `PlanStale` with reason `SessionLoaded` (`:85`, `:144–149`).
- Approvals bind to a fresh random session epoch (`:69–70`, `:221–225`), and queueing checks that epoch (`Engine.cs:198`, `:211`).
- Step and execute stale open approvals with reasons `ExperimentStepAdvanced` and `ForestRevisionChanged` (`LabSession.cs:243`, `:252`; reasons at `CanonicalJson.cs:46`).

**What goes wrong:** with an APPROVED or PENDING_WORK plan open at the split point:
- the uninterrupted run stales it at the next annual advance (reason: step);
- the reloaded run stales it at load (`SessionLoaded`).

The biological state matches, but the event chain and hash do not, so the criterion as written cannot be met. The DRAFT asks for P1 work-plan semantics "separate from P0" (REQ-E03) yet copies P0's stage names and never says whether P1 keeps session-epoch binding. That question belongs to the open C-1A.

**Smallest repair:**
1. Define equivalence over per-ID biological state, the mortality/felling ledger, RNG inputs and model versions.
2. Either require "no open APPROVED/PENDING_WORK plan at the split point", or state that the reloaded chain contains exactly one extra recorded `SessionLoaded` stale event per open plan and is compared on that basis.
3. Add to C-1A the explicit sub-decision: does P1 keep P0's session-epoch approval binding?

### R-3 — MINOR — Every backtick is escaped, so code spans and the plan diagram do not render on GitHub

All of the backticks are written as `` \` ``: spec 82, plan 28, tasks 32, requirements 34, handoff 22.

I checked with the GitHub GFM renderer (`POST /markdown`):
- formulas such as `ΔD=1.2·site·…` show with literal backticks;
- the `plan.md` §2 architecture diagram (l.15–32) collapses into one `<br>`-joined paragraph with a visible "```text" and loses its alignment.

**Repair:** remove the backslash escapes. Content is unchanged.

### R-4 — MINOR — Gate labels collide and the order is ambiguous

- `plan.md` l.49 defines **G0** = evidence plan, marked COMPLETE.
- `tasks.md` uses **P1-G0** for the owner/Manager spec approval, which is plan **G2**.
- `tasks.md` l.17 then says "G0 and P1-01…05 are BLOCKED / NOT RELEASED", which contradicts plan G0 = COMPLETE.

**Repair:** add a one-line crosswalk, or rename tasks P1-G0 to P1-G2:

| tasks.md | plan.md |
|---|---|
| P1-S0 | G1 |
| P1-G0 | G2 |
| P1-01 … P1-05 | G3 … G7 |

### R-5 — MINOR — When each decision is needed, and who owns it, is inconsistent and unstated

- `plan.md` §4 (l.61) puts all of C-1…C-7 before P1-01.
- `tasks.md` l.11 requires only C-2…C-6 for P1-01.
- No file says which decisions are owner-level and which are technical.

**Repair:** add the timing/owner table from §4 of this report.

### R-6 — MINOR — The stale schema-23 harness repair is over-coupled to the oracle

- The `tasks.md` l.11 P1-01 work cell includes "repair stale schema23 verifier assertion separately".
- `tasks.md` l.48 says runtime capture "remains blocked" if that repair is not authorised.
- But spec l.85, plan l.37 and REQ-F02 make the repair a separate owned task.

A new P1-01 capture harness does not need the stale `SitkaGrowthModelVerification` `POLICY_NEW_GAME_GROWTH2` assertion. It blocks capture only if the capture reuses that harness.

**Repair:** remove the repair from the P1-01 work cell. Say the oracle is blocked by the stale assertion only if it reuses that harness. Keep the repair as its own claim.

### R-7 — MINOR — The oracle's runtime configuration is unspecified, and one source detail is ambiguous

To capture a closed adult population from the real `AdvanceOneYear`, the oracle has to configure non-adult systems without changing production code. The DRAFT does not say how.

Facts at `fc18190b`:
- Regeneration model 2, the new-game default, **throws** without a `ScenarioOneManager` (`ForestEcologyController.cs:483–486`).
- `PromoteCohorts` silently does nothing when there is no `ForestTreeSpawner` (`:1701–1703`). Otherwise recruits at DBH 2–20 cm join the RD and CI in later years.
- `ResolveStormForYear` runs before competition (`:472`).
- `ecologicalYear` increments **before** the roll (`:467`, roll key `:1103`). It starts at the controller's year, not the P0 `inventoryYear` (2026).
- Mortality suppression reuses the **pre-growth** CI from step 1; CI is not recomputed after growth (`:475`, `:1088–1092`). `requirements.md` REQ-C09 l.36 ("post-growth density, suppression") can be misread as post-growth suppression.

**Repair:**
1. Add an oracle configuration contract to spec §6 and tasks §3: regeneration model and spawner/scene composition, storm 0, and an initial-ecological-year/seed mapping.
2. Require that these settings are shown not to change adult outputs, for example with a one-year A/B.
3. Reword REQ-C09: "post-growth RD; suppression from the pre-growth CI".

### R-8 — MINOR — Author and approver overlap at G2 is not addressed

- The DRAFT author is the "OpenAI GPT-6 Overall Manager" (`spec.md` l.5).
- G2/P1-G0 names the "product owner and Overall Manager" as approvers (`plan.md` l.51, `tasks.md` l.10).
- REQ-A01 forbids self-approval, and the packet says "do not let the author self-approve".

**Repair:** state that, because the Manager authored the DRAFT, G2 needs the product owner's decision plus this independent review (or a re-review after changes). The Manager's approval alone does not count.

### R-9 — MINOR — The P0 code lineage and the shared ignore file are not stated

- The P0 lab source is **not on main**: `git ls-tree fc18190b -- Tools/MultiRackSitkaLab` gives 0 files, against 52 at `2659db99`. Yet `plan.md` l.34 places P1 under `Tools/MultiRackSitkaLab/` beside "existing `src/Core/Engine.cs`".
- The proposed "dedicated ignored `.lab-p1/`" (`spec.md` l.58) would need an edit to the P0-owned `Tools/MultiRackSitkaLab/.gitignore`, which ignores only `.lab-data/`. That is a shared-file touch.

**Repair:**
1. List "P0 integration lineage / P1 base branch" as an explicit precondition for P1-02.
2. Name `.gitignore` as a shared P0 file that needs its own ownership.

### R-10 — MINOR — Editorial

- `spec.md` l.126: "current-CFF" should read "current-CCF".
- The crosswalk "Status" column (`spec.md` l.100–107) mixes **DRAFT** and **OPEN** without defining either term.
- C-5 calls for square parity fixtures. The existing P0 `oracle-quarter-ha` is already a 50 m square (corner origin, 3 trees) and could be the first centred parity fixture. Worth noting.

## 3. Product, authority and P0 isolation (packet §3 A)

| Check | Result | Evidence |
|---|---|---|
| DRAFT-only authority is clear in all four files and the handoff | **PASS** | spec l.4, l.121, l.136; plan l.3, l.108; tasks l.3, l.17; requirements l.3, l.5, l.98; handoff l.5–6 |
| P0 calendar-only step, strict v1, integer mm, immutable fixtures | **PASS** | spec l.17; REQ-A03; matches `Engine.cs:168–174`, `LabFile.cs:100–102` |
| A distinct P1 format/extension/root is plausible | **PASS** | The P0 reader requires `format="ccf-sitka-lab"` first and `labSchemaVersion==1` (`LabFile.cs:100–102`). The listing regex `^[A-Za-z0-9_-]{1,64}\.sitkalab\.json$` (`LabDataStore.cs:62`) would never list a `.sitkap1.json`. The DRAFT rightly says a separate root does **not** automatically inherit the handle-relative write guarantees (spec l.60). |
| Fork from the original fixture vs a post-executed P0 snapshot | **OPEN, correctly flagged** as C-1A (spec l.59; REQ-B04). Separate save/replay stays possible either way; see R-2 for the approval sub-decision. |
| No native Windows lab claim, no Unity build, Scenario One unchanged | **PASS** | spec l.17; plan l.43; REQ-A04, REQ-E09; tasks l.80 |

## 4. Source and numerical scope (packet §3 B) — DIRECT SOURCE CHECK at `fc18190b`

| DRAFT claim | Source | Result |
|---|---|---|
| ΔD = 1.2·site·clamp01(1−D/120)/(1+CI/5); site always 1 | `GrowAdults:1015–1019`; `SitkaSpruce.asset` (1.2/120/Ci50 5); `ForestEcologyCell.cs:48`, the only assignment repo-wide | ✓ |
| Hegyi: living non-stump neighbours, inclusive 8 m, (Dn/max(1,Dt))/max(0.5,d), slack pre-prune, ordinal order | `:912`, `:930–999` (slack `1.00002f` `:985`, exact `distance > cutoff` `:988`) | ✓ |
| Class III E(age), E(30)=20.4, h·E(a+1)/E(a); 60 m clamp is game policy | `SitkaGrowthModel.cs:45–60`; `ForestTree.ApplyGrowth:627` | ✓. The DBH clamp `[1,200]` cm (`:626`) is not mentioned. It is harmless for P0 (minimum live DBH 6 cm) but belongs in the oracle cases. |
| Stage-dependent height getter (m-1) | `ForestTree.cs:161`, `:264–275` | ✓ |
| Crown from post-growth DBH and pre-growth CI, relax 0.15 | `RelaxCrowns:1229–1245`; asset | ✓ |
| All-living RD with Dq and N/ha over the square area | `StandRelativeDensity`, `StandAreaHectares:73`, `SitkaGrowthModel.RelativeDensity` | ✓ |
| P, S, p = min(0.5, 0.08·P²·S⁴/mean S⁴); boundary to RD ≤ 1 or count ≤ 1 | `SitkaGrowthModel.cs:96–114`; `:1111–1131` | ✓ exact, including the suppression-desc/ID tie order |
| Year++ → CI → growth → crown → mortality; felling before the advance | `AdvanceOneYear:467–481` | ✓. Storm and regeneration excluded as stated (see R-7). |
| Model 2 equals model 1 on a pure Reference-demand Sitka stand; growth 0 is diagnostic only | `:1072–1078` (comment: "pure Sitka is exactly growth model 1") | ✓ |
| Schema 24 vs stale harness 23 | MULTI-010 §2.3 (git ancestry) | ✓ classified honestly as stale test, not a source defect (spec l.50, REQ-F02) |
| Mortality roll keyed by RNG model, year, seed and full ID | `:1103`; `SimulationRandom.cs:41–55` | ✓. Integer hash, so portable (R-1). |

Other packet §3 B checks:
- **Source numbers labelled as source/calibration, not Irish field truth.** PASS: spec l.47–48; handoff l.32.
- **Below-threshold live stems.** The DRAFT treats them as a **deliberate synthetic Mature-equivalent restriction**, not an age model (spec l.74, REQ-D02). PASS. All P0 fixtures are even-aged 28 with live height ≥ 9.5 m, so they would be Mature in source anyway.
- **Closed population, natural death separate from felling, no growth on propose/approve.** PASS: spec l.68, AC-P1-04; REQ-E02/E03; consistent with P0 AC-05.
- **No "nine Python vectors = parity" claim.** PASS: spec l.29, l.52; AC-P1-02.
- **C-6 oracle-before-adapter order.** PASS in sequence, but see R-1 (route) and R-6/R-7 (blocking and configuration).

## 5. Scientific authority (packet §3 C) — EXTRACTION ONLY for every primary-paper claim

| Check | Result |
|---|---|
| B (m²/ha), N, H; pure even-aged; thinned/unthinned domains; SI = top height at age 30 | PASS (spec l.32–35; plan l.93) |
| No field forecast accuracy; 2017 NFI RMSE not a pass threshold | PASS (spec l.35; plan l.94; tasks l.71) |
| Top-height rule NOT VERIFIED; top-100-largest-DBH/ha only a proposed convention | PASS (spec l.78; REQ-D06) |
| SI and polygon area labelled as adapter modifications; no implicit SI tuning | PASS (spec l.78; plan l.94; tasks l.72) |
| 2006 Manager spot check not promoted to a full primary audit | PASS (spec l.33; plan l.95) |
| #479 marteloscope NOT VERIFIED | PASS (spec l.35; plan l.96) |
| `Growfor` 24+18 tests = internal consistency only; requalify, don't duplicate; no automatic calibration | PASS (spec l.90; tasks l.68, l.72; REQ-F04) |

## 6. Dispositions of m-1 … m-8

| ID | Disposition in DRAFT | Reviewer judgement |
|---|---|---|
| m-1 stage height | Mature-equivalent only; reject non-Mature (spec l.45; REQ-C06; tasks l.86) | **ADDRESSED** |
| m-2 site = 1 | Current-source parity, not a site class (spec l.43; REQ-C04) | **ADDRESSED** |
| m-3 P0 Dead without cause/year | Proposed `pre-inventory` sentinel; no simulated death (spec l.68, l.76; REQ-D03) | **ADDRESSED as an open proposal** (C-4) |
| m-4 float32/double | C-2 policy and oracle (spec l.72; REQ-C02) | **PARTIALLY ADDRESSED.** Superseded by R-1: the binary32 recommendation, MULTI-010's included, needs reframing. |
| m-5 square vs polygon | Square centred parity fixtures; polygon is a labelled extension (spec l.47, l.78; REQ-D05) | **ADDRESSED.** The probe confirms coordinate centring changes float bits, so the translation policy must be pinned. |
| m-6 RNG vs growth version | Separate axes; RNG 0/1 cases (spec l.48; REQ-C09; AC-P1-06) | **ADDRESSED** |
| m-7 113 = volume-equation calibration; page bases | spec l.35; plan l.95; REQ-F06 | **ADDRESSED** |
| m-8 below-threshold / near-onset / SI | spec l.80 figures (153/9/15, 1893/107/182, RD 0.1698/0.6085 vs 0.1689/0.6057) match MULTI-010 §5.2; dedicated fixtures required (REQ-D08) | **ADDRESSED** |

## 7. C-1 … C-7: status, timing and owner (my recommendation for the repair in R-5)

All seven are genuinely **OPEN** in all four files (spec §7, plan §4, tasks l.19, requirements §H), and each has a testable proposal. None is presented as an accepted default.

| ID | Needed before | Owner level | Reviewer comment |
|---|---|---|---|
| C-1 format/fork | P1-03 (format); **C-1A before P1-01** if post-intervention snapshots are in scope | C-1A and the approval-binding sub-decision are owner-level (they decide what a P1 experiment means); format strings are technical | Proposal sound (§3). Add the R-2 sub-decision. |
| C-2 numeric | **C-2a before P1-01; C-2b after P1-01 evidence** | Technical (spec approval) | Reframe per R-1 |
| C-3 population | Before P1-01 fixtures | Technical for parity mode (all-living is the source rule). Owner-level only if a non-parity "actionable-only" mode is wanted. | Proposal correct, and material at the 0.6 onset (m-8) |
| C-4 initial state | Before P1-01 fixtures | Technical | Add the ecological-year/seed mapping (R-7) |
| C-5 geometry/SI/H | Before P1-01 fixtures (geometry); before P1-04 (SI/H convention) | Technical | Sound; could reuse `oracle-quarter-ha` (R-10) |
| C-6 oracle route | Before P1-01 | **Owner-level** in practice, because it needs Unity-lane time against Scenario One priority (R-1) | Add the configuration contract (R-7); decouple the harness repair (R-6) |
| C-7 labels | Before any user-visible output (P1-04/P1-05) | Owner approves the wording | Consistent with P0 A8 ("SYNTHETIC TEST FOREST", model name/version) |

**Can a reviewed P1-01 runtime oracle be executed after a future approval?** Yes, conditionally. It needs a bounded Unity Editor lane with the configuration from R-7, or a .NET harness cross-checked against such a lane (R-1). Nothing in the DRAFT requires modifying production source, fixtures or saves. Unity-lane contention and the host's historical licence failures are scheduling risks, not design defects.

## 8. Cross-file consistency (DR-04)

- **Gate order** is consistent apart from the R-4 label collision.
- **P1-04 parallelism** is stated the same way in all files: it runs only after P1-02 is stable, in a separate read-only lane, with no shared source writes (plan l.55; tasks l.14; spec l.88).
- **No route** depends on a native Win32 lab port or an unsafe machine check.
- **Requirements** are observable, and **no checkbox is pre-checked** (`grep -c '\[x\]'` = 0).
- **All links** use branch-qualified URLs that resolved for the refs I read.

## 9. What ran and what did not

**Ran:**
- static reading of the sources in §1;
- the GitHub GFM render check (R-3);
- one bounded scratch probe (R-1);
- SHA-256 re-checks of the fixture copies.

**Did not run:**
- Unity Editor or any harness;
- C# compilation, the P0 lab or tests;
- any P1 code;
- empirical model validation;
- primary-PDF verification (not available);
- Growfor tests (not needed; MULTI-010 already ran them).

**Touched:** no `Assets/**`, `Tools/**`, `specs/**`, `Docs/Project/**` (board included), saves, Reference Future, Drive or dashboard. Worktree status was clean before this one file was added.

### Scratch evidence (off-repo, not an archive)

`/media/jer/Files/CCF/Verification/MULTI-012-SitkaP1-DraftSpecReview-20261115/`

| File | SHA-256 |
|---|---|
| `scripts/float_semantics_probe.py` | `a5b60a6e6c5c856309abb529e0ab3fd959bf3789ad207dc0b1408ab9a7351683` |
| `results-float-probe.json` | `bde9a18af3368abe738ff52543a8b881261050523cb3294e7c272f63038b7172` |

Probe inputs are the MULTI-010 scratch fixture copies (demo `d0eed689…f2ed`, perf `d12db111…90df`, originally exported from `2659db99`).

## 10. Next step

The Manager rules on R-1…R-10. If the ruling is NEEDS CHANGES, the Manager issues a bounded docs-only follow-on to amend the four files; this reviewer does not repair them. Re-review can be scoped to the delta. After that, the owner decides C-1A, C-6 (Unity lane) and C-7 wording. **No implementation is implied.**

GitHub publication proof (readback and the one-added-file diff against `50178f9d`) is in the #531 HANDED-OFF comment; this file cannot prove its own publication. **Dashboard/Drive sync: NOT VERIFIED.**
