# #554 — independent review of the #552 Sitka annual growth and natural-mortality design revision

**Date:** 15 November 2026  
**Verdict: NEEDS CHANGES** — 0 blocking, **4 major**, 6 minor. All corrections are **docs-only technical-spec repairs**. None requires a new owner decision, but M-4 changes how the owner's C-1A choice is described. The design remains **DRAFT / NOT APPROVED**. Every C-item, including C-1A and C-2a/C-2b, stays **OPEN**. This review approves no spec, code, Unity session or build.

## Reviewer, scope and independence

| Item | Record |
|---|---|
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in local T3 Code |
| Host / worktree | `jer-laptop`; new private worktree `/media/jer/Files/CCF/Operations/CCF-554-SitkaAnnualGrowth-IndependentReview-20261115` (Files, ext4) |
| Branch / start HEAD | `review/sitka-annual-growth-design-independent-20261115-v1` at `61e53074bd5593603d3c7ab324eca1c1487ac9dc` (= author candidate) |
| Claim | [#554 RUNNING registration](https://github.com/jermur788/CCF/issues/554#issuecomment-6902232677), recorded before source inspection |
| Live board | `origin/main` `a9b89ab8` `Docs/Project/coordination/active-tasks.md`: no #554/#552 row, no conflicting writer |
| Material reviewed | The four DRAFT files and the #552 handoff at `61e53074`; diff against baseline `3af7b7ff` (4 modified + 1 added, +335/−61 lines) |
| Source inspected (read-only `git show`) | CCF `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; P0 lab `2659db99e317d4aa93b51cf97131a61eab5fdeec` |

**Prior-involvement disclosure.** Claude Opus 5.5 sessions wrote MULTI-010 (#525), MULTI-012 (#531) and the MULTI-013 (#536) DRAFT repair. Some baseline wording at `3af7b7ff` (R-1…R-10 text, Mode A/B framing) is therefore Opus-authored. This review grades the **new #552 Codex additions**: spec §4.1, §5.1–§5.3, plan §8–§9, tasks §8–§9 and requirements §H–§I. It does not re-grade earlier Opus text as independent evidence, and it is not an independent forestry replication of the papers the earlier reviews used.

**Not done:** no C# compilation, simulation, Unity Editor run, P0 test, browser check, GROWFOR rerun or field-data access. Every source statement below is a **DIRECT SOURCE CHECK** of historical text at the pins named above. None is a runtime observation.

## Findings — major

### M-1. The 50 m quarter-hectare parity fixture cannot be built on the pinned source, and parity fixtures have nowhere to live

**Where:** spec §5 C-5 (line 129), §5.1 geometry adapter and source-area paragraphs (lines 148–150), §4.1 original-only import (lines 91–96); plan §9 configuration (line 171) and **N05** (line 183); requirements **REQ-D11** (line 117).

**Evidence (direct source check, `fc18190b`):**
- `StandGeometryModel.cs:32` — `IsKnown(model) => model == Legacy40 || model == Enlarged80`.
- `StandGeometryModel.cs:34` — `StandSizeMeters(model) => model == Enlarged80 ? 80f : 40f`.
- `ForestEcologyController.cs:79–80, 92–107` — `ApplyStandGeometry` is "the one path" that changes `standSizeMeters`, and it throws for unknown models.
- The Editor-only verification override also accepts only known models (`StandGeometryModel.cs:80–85`).

So the only source squares are **40 m (0.16 ha, Legacy40)** and **80 m (0.64 ha, Enlarged80)**. A 50 m / 0.25 ha stand cannot be configured without a production change or a non-production reflection write.

The spec treats 50 m feasibility as something "a future geometry-capture owner must demonstrate", with a stop rule. That evidence is already available from static source, and it settles the question for this pin. As written, the N05 positive control ("Quarter-ha maps to ±25 m, 0.25 ha") cannot be met.

**Second gap: where the parity fixtures live.** §4.1 allows import of **original P0 catalog fixtures only**. None of the three P0 fixtures is a 40 m or 80 m square: the quarter-ha is 50 m, and demo/perf are polygons. Every dense, threshold or edge source-parity fixture must therefore be new. The design does not say where those fixtures live or how they are trusted. Adding them to P0's `fixtures/synthetic/` directory would be a P0 change, because `FixtureCatalog.LoadFrom` enumerates that whole directory. Putting them anywhere else means bypassing the "original P0 import" path the spec describes.

**Minimum correction (technical, docs-only):**
1. Record as a source fact that the pin admits only Legacy40 (40 m) and Enlarged80 (80 m). Drop "candidate first source-parity fixture" for the 50 m quarter-ha.
2. Recentred `oracle-quarter-ha` still fits inside ±20 m: its trees sit at ±15 m and the origin. It can stay a **sparse wiring and height control** under Legacy40, but only with its source area declared as **0.16 ha**, not 0.25 ha. Its three trees are more than 8 m apart, and RD is about 0, so CI = 0 and P = 0 under either area. Rewrite the N05 positive control and REQ-D11 to match.
3. Add a **P1-owned capture-fixture catalog**: separate format, version and trust anchor (see M-2), for new 40 m and 80 m square fixtures. Keep it distinct from the original-P0 import path, and state that it is not a P0 change. Enlarged80 also invokes `StandApproachPath.Apply` (`ForestEcologyController.cs:102`). The capture packet must either show that this cannot move or exclude hand-placed trees, or use Legacy40.

### M-2. The trusted-catalog anchor is unspecified, and P0's own manifest cannot serve as it

**Where:** spec §4.1 step 1 (line 94); plan **N01** (line 179); requirements **REQ-B08** (line 112).

**Evidence (direct source check, `2659db99`):** `FixtureCatalog.LoadFrom` (`FixtureCatalog.cs:20–44`) computes `FileSha256` from whatever bytes it reads (`:44`). It never reads `fixtures/synthetic/MANIFEST.sha256`. That manifest sits in the same directory as the fixtures, so anyone who can rewrite a fixture can rewrite the manifest consistently.

The spec rightly rejects "a hash supplied by that same untrusted file". But it does not say where the "trusted revision-pinned manifest" lives, and a "full P0 source revision" cannot be verified at runtime without Git. As specified, an implementation could read P0's on-disk manifest and pass N01's "forged self-hash" check while still accepting a consistently forged fixture-plus-manifest pair.

**Minimum correction:**
1. State that the trust anchor is a reviewed table of `(fixtureId, fileSha256, snapshotHash, P0 revision)` stored in **P1-owned source or configuration**. Copy it from `MANIFEST.sha256` and the snapshot hashes at the pinned P0 revision, and never read it from the P0 data directory at runtime.
2. Treat the P0 revision as provenance metadata for that table, not something checked at runtime.
3. Add an N01 negative case: fixture **and** `MANIFEST.sha256` both rewritten consistently must still be refused.
4. Keep the spec's honest threat-model sentence that hashes are not authorship signatures.

### M-3. The capture contract misses a source observer trap that would record the wrong pre-growth CI

**Where:** spec §5.1 conversion and per-year record paragraphs (lines 156–158); plan §9 instrumentation sentence (line 173) and **N03** (line 181); requirements **REQ-C13** (line 114).

**Evidence (direct source check, `fc18190b`):**
- `GetCompetitionIndex` (`ForestEcologyController.cs:1811–1819`) **lazily re-runs `UpdateCompetition`** whenever `competitionCurrent` is false.
- `OnTreeMortality` (`:429–437`) removes the victim's entry (`competitionIndex.Remove(tree)`, `:433`) and sets `competitionCurrent = false` through `QueueLivingCanopyRebuild`.

So in any year with a death, a harness that reads CI through the public getter after `AdvanceOneYear` gets a **recomputed post-growth, post-mortality CI**, not the pre-growth CI that drove DBH, crowns and suppression S. The victims' own pre-growth CI and S, which are exactly the inputs to the mortality decision, are gone from controller state after the step. A capture taken through the obvious API would therefore be wrong without any error. This is not the general "instrumentation may change Mono code generation" caveat the spec already covers; it is a specific, source-visible trap the capture contract should name.

**Minimum correction (reviewer proposal, needs capture-owner confirmation):**
1. In C-2a, specify the **observation points**. Read per-ID CI immediately **before** `AdvanceOneYear`, through the same `UpdateCompetition` path (`InvalidateCompetition()` then the getter), with storm 0 and no state change between that read and step 1.
2. After the step, read the private survivors' entries without calling the getter, and require them to be **bit-identical** to the pre-advance values. That is a non-perturbing cross-check.
3. Add N03 negative cases: CI captured through the getter after a death year, and victim S reconstructed rather than observed, must both block the oracle.

### M-4. The §5.2 "stale-on-load" proposal is not P0 semantics, but the register calls it "P0-style"

**Where:** spec §5.2 stale-on-load paragraph (line 170), the C-1A register row "P0-style session-bound approvals stale on load" (line 228 and its three copies), and §4 Option 1 (line 73); requirements **REQ-E12** (line 120); plan **N09/N10** (lines 187–188).

**Evidence (direct source check, `2659db99`):** P0 `LabSession.FromFile` (`LabSession.cs:76–85`) appends **only** `PlanStale` events, through `StaleOpenApprovals(tx, "SessionLoaded")` (`:85`, `:144–149`). When no plan is open, P0 appends **nothing** on load.

The #552 proposal appends "exactly one `PlanStale(SessionLoaded)` per open … plan, … **plus a named `SessionLoaded` audit event**", so it records an event on every load. That is a third behaviour, different from both Option 1 (P0 semantics) and an undefined Option 2. It matters in two ways:
1. Under true P0 semantics, a Mode A split with no open plan and no later approval leaves **identical raw histories**, which is the "restricted test" the spec keeps. Under the #552 proposal, that case can never happen across a load.
2. An owner choosing the "P0-style" register text would be choosing a behaviour P0 does not have.

**Minimum correction:** pick one of these, docs-only.
- (a) Drop the always-appended audit event, so the proposal really is Option 1.
- (b) Name it as a distinct variant (for example "Option 1b: P0 staling plus load audit event"). Change the register row from "P0-style" to that name, and state in Mode A / REQ-E12 / N09 that raw-history equality across a load is then **never** reachable. The restricted equal-hash test then applies only to replay without a load.

## Findings — minor

- **m-A Ledger capacity arithmetic** (spec §4.1 line 101, §5.2). P0's ceilings are confirmed: `MaxTrees = 20_000` and `MaxFileBytes = 64 MiB` (`Models.cs:24, 28`). A per-ID annual record that carries typed datums would hold before/after DBH, H and crown, plus CI, S, p, roll and age: about 10 datums at roughly 80 bytes each. At the tree ceiling that is about **16 MB per year**, so the file fills in **about 4 years**. The 1,893-tree perf fixture would fill in about 40 years. These are reviewer estimates, not measurements. Because Load recomputes every year from the pinned model anyway, state the expected bytes per tree-year. Then choose deliberately between full per-ID payloads and per-year state hashes plus discrete outcomes (deaths, rolls).
- **m-B Crowns do not affect outcomes in this adult-only scope; pin the crown branch.** Crown radius is not read by `UpdateCompetition`, `GrowAdults` or `ApplyAdultDensityMortality`. It is read only by canopy/light (`ForestEcologyController.cs:841`) and other excluded systems (`:1353`). So the synthetic zero-CI crown initialisation (C-4) can change crown trajectories only, not DBH, height or deaths. This is a source-derived inference. Say so, because it lowers the C-4 risk. Also, `PotentialCrownRadiusM` has a power-law branch (`TreeSpeciesDefinition.cs:246–253`). Record the asset fact `usePowerLawCrown: 0` (`SitkaSpruce.asset:25`) as part of the linear-crown claim.
- **m-C Silent no-op year.** `AdvanceOneYear` logs a warning and **returns without advancing** when the species cannot be resolved (`ForestEcologyController.cs:460–465`). With no active `ForestTreeSpawner`, `ResolveSpecies` has no fallback unless the controller's `species` field is set. Add to the plan §9 configuration readback: assert that a species is resolved and that `EcologicalYear` actually incremented.
- **m-D Which fields the state hash covers.** §5.2 puts "plan approval/session fields where present" and "committed IDs" in the replayed state, but does not say whether the **current** session epoch and the committed-command set are inside `P1-STATE-V1`. Which way that goes decides whether the audit-only `ExecutionRejected` and `SessionLoaded` events change the state hash, and whether golden vectors can be deterministic. Name the hashed state fields explicitly.
- **m-E Top-height convention (science note, not a defect).** The proposed "mean height of the 100 largest-DBH stems per hectare" matches the conventional UK/Irish yield-model *definition* of top height. In practice, field estimation usually measures the largest-DBH tree in each 0.01 ha plot, which can differ from a strict top-k in small or clumped samples. This is reviewer domain knowledge, not checked against the Broad–Lynch primary text. Keep **NOT VERIFIED**, and record the definition-versus-estimation distinction in §5.3 / REQ-F10.
- **m-F RNG key string.** The roll key is the literal string `"ADULT-MORT-" + tree.TreeId` (`ForestEcologyController.cs:1103`). Write this exact key format into C-2a, and require that P0 `treeId` strings are assigned verbatim as CCF `TreeId`. Otherwise the "integer portable roll" claim does not carry over to imported IDs.

## Review matrix (packet items 1–11)

Evidence classes: **DSC** = direct source check at a historical pin; **INF** = source-derived inference; **SYN** = synthetic or design choice; **PROP** = reviewer proposal.

| # | Item | Grade | Evidence and notes | Severity / resolution type |
|---|---|---|---|---|
| 1 | Original verified input and trusted catalog | **PARTIAL** | The rejection rules for marks, plans, calendar changes and Felled match P0's own `FixtureCatalog` invariants (DSC `FixtureCatalog.cs:32–36`). Lineage holds: `Tools/MultiRackSitkaLab` has 0 paths at `fc18190b`, at current `origin/main` `a9b89ab8` and at `61e53074`, against 52 at `2659db99` (DSC). The trust anchor is unspecified and P0's manifest is co-located (DSC) | **M-2**, technical |
| 2 | Isolated data/security architecture | **PASS** | A sibling P1 service/root with its own ignore is proportionate. No P0 `.gitignore` edit is implied (P0 ignores `.lab-data/`, DSC). Handle, special-file, race, Origin, Host and CSRF protections are listed as requiring requalification, not inherited | m-A (capacity), technical |
| 3 | Pure-Sitka all-living population and synthetic initialisation | **PASS** | All-living matches source `FindTrees`, which excludes only biologically dead trees and skips stumps in CI and growth (DSC `:875–886`, `:955`). Zero-CI crown is labelled synthetic (SYN). Ages copied, dead trees null-cause | m-B, technical |
| 4 | Geometry and source-area parity | **FAIL** | Only 40 m and 80 m squares are admissible (DSC). The 50 m positive control is unachievable, and parity fixtures have no home. Recentring order and keeping elevation out of horizontal z are otherwise sound | **M-1**, technical |
| 5 | Unity numeric capture without manufactured precision | **PARTIAL** | Typed f32/f64 bit records, signed zero, `notEvaluated` for skipped rolls (matches the `probability > 0.0 &&` short-circuit, DSC `:1103`), and the uninstrumented control are good. Observation points are missing (DSC `:429–437`, `:1811–1819`); species no-op; key string | **M-3**, m-C, m-F, technical |
| 6 | Annual work and biological event order | **PASS** | Source order: year++ → storm → `UpdateCompetition` → `GrowAdults` (age +1 for every non-stump living tree, height from the pre-increment age) → `RelaxCrowns` (post-growth DBH, pre-growth CI) → density mortality (post-growth RD, stored pre-growth S, strict `roll < p`, boundary loop by descending S then ordinal ID, with `count <= 1` escape) (DSC `:467–481`, `:1003–1038`, `:1062–1158`, `:1229–1245`). No second release | none |
| 7 | Atomic annual ledger, snapshot, replay and capacity | **PARTIAL** | Compound event with child deaths, separate history/state tags with no cycle, idempotent command ID, and detached replay-before-activate are sound. Both BA reconciliation equations check out algebraically (INF). State-field membership is unclear, the capacity budget is missing, and the load-event interaction is in M-4 | M-4, m-A, m-D |
| 8 | Approvals on load and legal action comparisons | **PARTIAL** | The Mode B legal-equivalence rule (no comparing executed and unexecuted branches) and no adopted projection are correct. The "P0-style" label is inaccurate (DSC `LabSession.cs:76–85`) | **M-4**, technical (affects how the owner choice is worded) |
| 9 | Historical stand diagnostics and science claims | **PASS** | k / rounding / tie / empty rules are reproducible and labelled synthetic. SI is inferred or blocked. No Irish individual mortality, field validation or 113-plot holdout claim | m-E |
| 10 | Usable decision gates and science labels | **PASS** | Owner and technical timing are explicit. C-2b is post-capture and not circular. The P0 no-growth banner is unchanged (DSC `Models.cs:15–19`). The label is explicitly not a forecast. No author self-approval | none |
| 11 | Test matrix N01–N13 and feasibility | **PARTIAL** | Most rows are falsifiable and proportionate. N01, N03, N05 and N09/N10 need the corrections below | M-1…M-4 |

## N01–N13 crosswalk

| Row | Assessment | Change needed |
|---|---|---|
| N01 original fork | Falsifiable; matches P0 invariants | Add the consistent fixture-plus-manifest forgery case; name the P1-owned anchor (M-2) |
| N02 namespace/root | Proportionate; mirrors the qualified P0 security set | Add a capacity-refusal case sized to the stated byte budget (m-A) |
| N03 numeric capture | Good typed-bit negatives | Add the post-step-getter CI and reconstructed-victim-S negatives; add the pre/post survivor bit cross-check; species/year readback (M-3, m-C) |
| N04 membership | Sound (DSC all-living) | none |
| N05 geometry | **Positive control unachievable** | Use a 40 m / 0.16 ha (or 80 m / 0.64 ha) declared area; define the P1-owned square fixtures (M-1) |
| N06 annual causality | Matches source order | none |
| N07 mortality branches | Matches the source short-circuit, strict `<` and boundary order | Record the key string (m-F) |
| N08 event integrity | Sound | State which hashed state fields audit-only events touch (m-D) |
| N09 split Mode A | Correct conclusion | State whether raw equality across a load is reachable under the chosen load-event variant (M-4) |
| N10 split Mode B | Correct | Same variant naming (M-4) |
| N11 reconciliation | Equations correct | none |
| N12 reference science | Sound | Definition-versus-estimation note (m-E) |
| N13 wording/P0 | Sound | none |

## Owner versus technical gates

- **No finding needs a new owner decision.** M-1, M-2, M-3, m-A…m-F are technical-spec repairs. M-4 is technical too, but it fixes the **wording of the owner's C-1A choice**, so it must be repaired before C-1A goes to the owner.
- C-1A, C-6 and C-7 remain owner decisions. C-2b remains post-capture. The rest of C-1…C-5 remain technical, timed as in spec §7.1. The #552 additions slip no owner default in as accepted. The four decision registers are consistent with each other; the C-1A row's text needs the M-4 fix in all four copies.
- No new contradiction with the earlier reviewed hash/version contract (MULTI-016 Mode A/B) was found beyond M-4, which makes one of its restricted cases unreachable rather than contradicting it.

## Recommended next step

A focused docs-only repair of M-1…M-4 and m-A…m-F across the four files, then a **delta review by a non-Codex reviewer** (the author was Codex). Because this reviewer authored earlier baseline text, the Manager may prefer a different provider for that delta review. Do not dispatch P1-01 or any source/Unity work. Scenario One stays first.

**Dashboard / Drive sync: NOT VERIFIED.** No coordination, canon, dashboard or Drive file was changed.
