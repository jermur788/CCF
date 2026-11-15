# #556 — independent delta review of the ten Sitka annual-growth design corrections

**Date:** 15 November 2026  
**Verdict: APPROVED AS DRAFT QUALITY** — 0 blocking, **0 major**, **6 minor residuals (D-1…D-6)**, all docs-only. All four major repairs (M-1…M-4) and all six minor repairs (m-A…m-F) are technically adequate. Nothing here approves the specification, releases P1 work, books Unity time or closes any decision. The four DRAFT files remain **DRAFT / NOT APPROVED**, and every register row (C-1A, C-1, C-2a, C-2b, C-3…C-7) remains **OPEN**.

## 1. Reviewer, independence and scope

| Item | Record |
|---|---|
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code in local T3 Code |
| Host / worktree | `jer-laptop`; new private worktree `/media/jer/Files/CCF/Operations/CCF-SitkaAnnualGrowth-TenFinding-DeltaReview-20261115` (Files, ext4). No Unity lane or lease |
| Branch / start HEAD | `review/sitka-annual-growth-ten-finding-independent-delta-20261115-v1` at `0120959f9077a8adcd88fed5d8658fc2b714292c` (= repaired #555 candidate, clean) |
| Claim | [#556 RUNNING registration](https://github.com/jermur788/CCF/issues/556#issuecomment-6902845085), posted **before** any source or DRAFT inspection |
| Live board | `origin/main` `Docs/Project/coordination/active-tasks.md`, fetched this session: no row or text naming #556 or this branch. `git worktree list` showed no other worktree on this branch or path. No competing process |
| Material reviewed | The four DRAFT files and the #555 handoff at `0120959f`; the #554 findings at `a6bf2d01`; the diff `61e53074..0120959f` (4 modified, 1 added) |
| Source inspected (read-only `git show`) | CCF `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; P0 lab `2659db99e317d4aa93b51cf97131a61eab5fdeec` |

**Independence disclosure.** I am neither the Codex #552 author nor Claude Opus 5.5 (the #554 findings and #555 repair author). I authored none of the four DRAFTs, the #554 report or the #555 handoff. Two disclosures. (1) My session context included the user's auto-memory index and the outcome note for #555, which summarise the Opus repair; I did not treat that note as evidence, and every grade below rests on the actual files and source text. (2) An earlier run of this same task failed at provider start-up (a billing error) before any tool call, so it did no work.

**Not done, by design:** no C# compile, test, simulation, Unity Editor or Mono run, P0 lab run, browser qualification, GROWFOR rerun or field-data access. Every source statement below is a **direct source check (DSC)** of historical text at the two pins. None is an observed runtime behaviour. The one measured-style number I computed myself is marked **ARITH** (plain arithmetic on the checked-in P0 fixture values). The packet notes the source may be newer today; I did not check any newer revision.

## 2. Ten-finding matrix

Grades: **PASS / PARTIAL / FAIL** against the original #554 finding and its minimum correction. "Residual" points to section 3.

| # | Finding | Grade | Evidence (file / line) | Residual |
|---|---|---|---|---|
| **M-1** | Only 40 m / 80 m stands; P1-owned capture fixtures | **PASS** | Spec §5 C-5 (L155), §4.1 derived-catalog block (L109–114), §5.1 probe and dense/edge paragraphs (L176–180), §6 item 2 (L263); plan §9 (L173, L177), N05 (L185); REQ-D05/D11; C-5 row. DSC: `StandGeometryModel.cs` has `IsKnown` = Legacy40 or Enlarged80 and `StandSizeMeters` = 80 or 40; `ApplyStandGeometry` throws on unknown models and calls `StandApproachPath.Apply`; the Editor override `VerificationOverride` also throws on unknown models. ARITH on P0 `oracle-quarter-ha` at `2659db99`: trees T001/T002/T003 at corner (10,10), (25,25), (40,40) m, DBH 20/30/40 cm; recentred on (25 000, 25 000) mm they sit at (−15,−15), (0,0), (+15,+15) m, inside ±20 m. Neighbour spacing 21.2 m > 8 m, so CI = 0. Dq ≈ 31 cm, 18.75 stems/ha on 0.16 ha gives RD ≈ 0.016, so P = 0. The 50 m wording and the "first parity fixture" claim are gone. Derived fixtures never enter P0's directory | none |
| **M-2** | Independently pinned trust anchor | **PASS** | Spec §4.1 step 1 (L96) and trust-anchor block (L101–107); plan N01 (L181); REQ-B08 (L114). DSC: `FixtureCatalog.LoadFrom` (`FixtureCatalog.cs:20–44`) hashes the bytes it reads and contains no read of `MANIFEST.sha256`. The repair binds fixture ID, full file SHA-256, canonical snapshot hash and P0 revision in a **P1-owned embedded or immutable-packaged table**, never read from the P0 directory; the revision is provenance with no runtime Git. N01 now has the consistent fixture-plus-manifest forgery negative. Derived fixtures get their own table. No hash value is invented. The "integrity, not authorship" sentence is present (L107) | none |
| **M-3** | Pre-growth CI observer trap | **PASS** | Spec §5.1 observation points (L188–198), year block (L200); plan §9 (L173, L175), N03 (L183); REQ-C13 (L116); tasks C13. DSC: `GetCompetitionIndex` lazily calls `UpdateCompetition` when `competitionCurrent` is false (`ForestEcologyController.cs:1811–1819`). `OnTreeMortality` removes the victim's cache entry and invalidates (`:429–437`). `AdvanceOneYear` calls `UpdateCompetition` **unconditionally** at step 1 (`:475`), so a pre-advance forced read cannot hand the step a stale cache. `ApplyAdultDensityMortality` reads S from the private `competitionIndex` directly (`:1086–1096`), not through the getter, and `KillDensityVictims` runs after S, p and the roll are evaluated. A grep of `Assets/ForestPrototype` at the pin finds `GetCompetitionIndex` called only from diagnostics, the tree-marking manager and UI views, none reachable from `AdvanceOneYear`, so the survivor cross-check is source-plausible. That is a static reachability reading, not a run. The no-op, fail-closed and instrumented-vs-uninstrumented control are all specified as **future gates**, with no capture claimed | D-3 |
| **M-4** | P0-faithful conditional stale-on-load | **PASS** | Spec §4 Option 1 (L75), Mode A (L88), Mode B (L89), §5.2 stale-on-load (L236) and replay paragraph (L238); plan Mode A/B (L125–126), N09/N10 (L189–190); REQ-E03 (L71), REQ-E12 (L122); all four C-1A rows. DSC: `LabSession.FromFile` creates a fresh epoch and calls `StaleOpenApprovals(tx, "SessionLoaded")` (`LabSession.cs:76–85`), which appends one `PlanStale` per Approved/PendingWork plan and nothing otherwise (`:144–149`). `Plans` is kept sorted by `string.CompareOrdinal(PlanId)` (`Engine.cs:145`), so "ordinal plan-ID order" matches P0. Only `approvalSessionEpoch` on plans enters the canonical state (`CanonicalJson.cs:137`), which supports "current epoch not hashed". The unconditional `SessionLoaded` event is now only a separate unapproved alternative. The four C-1A rows are byte-identical and OPEN | D-1, D-2 |
| **m-A** | Capacity budgets, transactional refusal | **PASS** | Spec §4.1 capacity block (L120–127); plan N02 (L182); REQ-B09 (L115). DSC: `MaxTrees = 20_000`, `MaxFileBytes = 64 MiB` (`Models.cs:24, 28`). I re-derived every cell of the 1/10/40-year table: 800 B × 2,000 trees = 1.6/16/64 MB; × 20,000 = 16/160/640 MB; "about 4 years" to 64 MiB at 16 MB/yr holds (4.2); compact 20–40 B gives 0.04–0.08 MB/yr (2,000) and 0.4–0.8 MB/yr (20,000), inside every stated bound. Rows are labelled rough estimates, not measurements. Over-capacity fails closed before commit with no truncation | D-4 |
| **m-B** | Crown non-causality; asset pin | **PASS** | Spec §5.1 crown row (L167) and header (L182); REQ-D10 (L118). DSC: no `Crown`, `Light` or `Canopy` reference inside `UpdateCompetition`, the `GrowAdults` growth lines or `ApplyAdultDensityMortality`; crown enters through `RecomputeCanopy` only (`:841`). `PotentialCrownRadiusM` linear branch when `usePowerLawCrown` is false (`TreeSpeciesDefinition.cs:246–253`); asset `usePowerLawCrown: 0`. The text scopes the claim to this adult-only route, as source-derived inference | none |
| **m-C** | Silent failed year | **PASS** | Spec §5.1 year block (L200), §6 item 2 (L263); plan §9 (L173), N03; REQ-C13. DSC: `AdvanceOneYear` warns and returns before `ecologicalYear++` when species or grid is missing (`:460–465`). The contract now binds species explicitly, reads back species and grid, asserts exactly +1 year, and has an N03 negative | none |
| **m-D** | Hash field membership | **PASS** | Spec §5.2 table and consequences (L210–228); plan N08 (L188); REQ-E11 (L121). State hash excludes the history tip; the history digest covers envelopes that hold state hashes, so there is no cycle. The current session epoch is outside state, matching P0 (above). Raw Mode A equality is still demanded only for identical complete histories, and no projection is adopted | D-6 |
| **m-E** | Top-height definition vs field estimation | **PASS** | Spec §5.3 (L244); plan N12 (L192); REQ-F10 (L124); C-5 row. The text separates the 100-largest-stems-per-hectare definition from one-tree-per-0.01-ha subplot estimation, labels it reviewer domain explanation (not Broad–Lynch primary-verified), keeps equivalence NOT VERIFIED, and keeps k, ties, rounding and empty cases explicit. ARITH check: `ceil(100·areaHa)` gives k = 16 and 64 for 0.16 and 0.64 ha in f32, f64 and mixed evaluation, so the rule has no float edge at the two legal sizes | none |
| **m-F** | Literal RNG key | **PASS** | Spec §5.1 per-year record (L202) and RNG identity (L204); plan N07 (L187); REQ-C14 (L117); tasks C06. DSC: the key is `"ADULT-MORT-" + tree.TreeId` with `probability > 0.0 &&` short-circuit (`:1102–1103`); `CompareTreeIds` is `string.CompareOrdinal`; `treeId` is assigned from a string (`ForestTree.cs:654`); P0 IDs are `^[A-Za-z0-9_-]{1,64}$`, case-sensitive. Verbatim assignment and a rename/re-case negative are specified | none |

**Count:** 10 of 10 PASS. 0 PARTIAL, 0 FAIL. The residuals below are wording and reconciliation slips around otherwise sound repairs; none leaves a repaired contract technically wrong.

## 3. Residual findings (all MINOR, docs-only)

| ID | Severity | Where | Problem | Smallest correction |
|---|---|---|---|---|
| **D-1** | MINOR | Plan L125 (Mode A bullet); spec AC-P1-05 L322; tasks §4 L82; REQ-E06 Mode A sub-bullet (L75) | The Mode A "load event" counterexample survives in four places. Under the proposed Option 1 a load with **no** open plan appends zero events, so that counterexample cannot arise in Mode A. Plan L125 also lists "a load that stales an open plan", which is a **Mode B** event by definition (an open plan exists). That parenthetical is new in #555; `61e53074` said only "recorded load audit event". The spec §4 Mode A text (L88) and REQ-E12 already state it correctly | Delete "a load that stales an open plan" from plan L125. In the other three places say the load-event counterexample applies "only under Option 2 or an unapproved always-on audit variant" |
| **D-2** | MINOR | Plan N09 positive control (L189) | It says identical raw histories "are possible". "Possible" is not falsifiable. A test needs the restricted case stated as a requirement | Reword: no open plan at the split, no later approval, identical command IDs and payloads, then raw event bytes and history tip **must** be identical; any difference fails |
| **D-3** | MINOR | Spec §5.1 L194 and L202, plan N03 (L183), REQ-C13 (L116), C13 | The text asks for S, p and the roll "for every ID" at evaluation. At the pin, `ApplyAdultDensityMortality` returns at `:1084` (`DensityPressure <= 0.0`) before S is computed at all, and `probability` is a local (`:1102`). In a P ≤ 0 year nothing is evaluated to capture. Separately, S, p and `meanVulnerability` are locals of a private method, so the N03 positive control likely needs verification-copy instrumentation; the roll is a pure integer function and is reproducible | State that S/p/roll are recorded only in years the source evaluates them; otherwise `notEvaluated`, with S optionally recorded as a clearly **derived** diagnostic. Add to C-2a that the capture owner decides up front whether instrumenting a verification-only source copy is acceptable, because without it the N03 S/p positive control cannot pass. The fail-closed rule is correct and stays |
| **D-4** | MINOR | REQ-E01 (L69); spec §5 L133 and L135 (`P1AnnualAdvanced` "with per-ID accounting") vs §4.1 compact layout (L125–127) | REQ-E01 and L133/L135 read as requiring per-ID before/after values, CI, S, p and roll in every annual record, which is the full layout. §4.1 presents the compact layout as equally valid ("loses no replay integrity"), so the capacity choice is not actually open as written | Add to REQ-E01 and L135: per-ID detail is held in the archive (full layout) or recomputed on replay and exported as an optional trace (compact layout), chosen at C-1 |
| **D-5** | MINOR | Tasks P1-S0 row (L22) | The recommended-provider cell still reads "independent non-author Claude Opus 5.5 High" while the same row (added by #555) requires a reviewer who is neither Codex nor Claude Opus | Replace with "Claude Sonnet 5.5 High, or an equivalent reviewer who is neither Codex nor Claude Opus" |
| **D-6** | MINOR | Spec §5.2 L222 and L234 | The table says an audit-only `ExecutionRejected` changes the state hash "only if it records a command ID". Whether it consumes the ID decides whether a retried rejected command is idempotent, and what N08's golden vector expects | List "does a rejected command consume its command ID" as a named C-1 item; no repair needed before then |

**Not findings, for the record.** (a) Plan L69, tasks §7 readiness line and spec L339 still say the #555 delta review is pending; that is correct today and becomes stale only on the Manager's disposition. (b) P1's import must recompute P0's canonical snapshot hash, which means reimplementing or sharing P0's canonical JSON; this falls under the existing C-1 lineage and P1-base decision and needs no new wording. (c) The `[-S/2, S/2)` half-open bound in §5.1 (L174) is stated as a rule but I did not confirm it against the source's in-bounds test; the probe sits well inside, so it does not matter for the sparse control, and dense/edge fixtures will need to confirm it.

## 4. N01…N13 trace

"Achievable / falsifiable" asks whether the positive and negative cases can exist and fail under the pinned-source constraints. Nothing was run. The link column checks plan ↔ requirement ↔ task agreement.

| Row | Changed? | Achievable and falsifiable? | Link check (plan / REQ / tasks §9) |
|---|---|---|---|
| **N01** original fork | Yes | **Yes.** The consistent fixture-plus-manifest forgery fails against an embedded table. Trust-table hash values are pending, so the test cannot be authored until they are prepared (stated) | REQ-B08; P1-02/03 ✓ |
| **N02** namespace/root | Yes | **Yes.** Over-cap refusal needs a P1-approved cap number, which is deferred to C-1 (stated) | REQ-B09; P1-03/05 ✓ |
| **N03** numeric capture | Yes | **Mostly.** Source supports pre-advance CI and the survivor cache check. The S/p positive control depends on instrumentation (D-3) | REQ-C13; P1-01 ✓ |
| **N04** membership | No | **Yes.** Coherent with all-living source behaviour | REQ-D10; P1-01/02 ✓ |
| **N05** geometry | Yes | **Yes.** The sparse-probe positive control is fully specified and I reproduced its CI = 0 and P = 0 by arithmetic. Dense and edge fixtures are proposals only; the Enlarged80 negative applies only to the optional 80 m run | REQ-D11; P1-01 ✓ |
| **N06** annual causality | No | **Yes.** Order matches source: year++, storm, `UpdateCompetition`, `RecomputeCanopy`, `GrowAdults`, `RelaxCrowns`, density mortality (`:467–481`) | REQ-E10; P1-01/02/03 ✓ |
| **N07** mortality branches | Yes | **Yes.** Key, strict `<`, short-circuit and ordinal tie order match source | REQ-C14; P1-01/02 ✓ |
| **N08** event integrity | Yes | **Yes, once C-1 fixes the archive layout.** Layout is still open, so the test is parameterised (D-4, D-6) | REQ-E11; P1-03 ✓ |
| **N09** split Mode A | Yes | **Partly.** Zero-event load is falsifiable. The positive equality case is stated too loosely (D-2) | REQ-E12; P1-03 ✓ |
| **N10** split Mode B | Yes | **Yes.** One stale per open plan in ordinal plan-ID order matches P0 | REQ-E12; P1-03 ✓ |
| **N11** reconciliation | No | **Yes.** I re-checked both equations algebraically; the N and BA equations are consistent | REQ-E13; P1-03 ✓ |
| **N12** reference science | Yes | **Yes**, as a labelling and report check; no numeric field validation is implied | REQ-F10; P1-04 ✓ |
| **N13** wording / P0 | No | **Yes.** Label text identical in spec §1, REQ-F11 and the C-7 row | REQ-F11; P1-05 ✓ |

The changed set is exactly N01, N02, N03, N05, N07, N08, N09, N10 and N12, as the packet expected (`git diff 61e53074..0120959f` on plan.md shows only those N rows touched). N04, N06, N11 and N13 are untouched and remain coherent. Plan, requirements and task cross-references agree row by row.

## 5. Preservation and mechanical checks

| Check | Result |
|---|---|
| Files changed by #555 vs `61e53074` | 4 modified Markdown + 1 added handoff; nothing else (`git diff --stat`) |
| Decision register | Nine rows in each of the four files; SHA-256 prefix `3be4cd5a3f71a489` is identical in all four; all nine rows end **OPEN** |
| Requirement boxes | **72 unchecked, 0 checked** in `checklists/requirements.md` |
| Stale terms | No `P1-G0`, no "P0-style", no `±25`, no escaped backticks in any of the four files |
| Relative links and anchors | All relative links and heading anchors across the four files resolve by a local GitHub-slug check. **Not** a rendered GitHub check |
| Earlier fixes | I read every line #555 deleted from the four files (69 deletions): all are replaced decision-register rows, the old 50 m / "P0-style" / unconditional-`SessionLoaded` wording, or the superseded N-row, gate and task cells. No R-1…R-10 or m-1…m-8 requirement text was removed or contradicted; the Mode A/B (R-2) structure still holds |
| P0 and source | Nothing under `Tools/`, `Assets/` or the P0 fixture directory is touched by this review |

## 6. Gates this review does not touch

C-1A (owner: fork source and approval-after-load policy), C-6 (owner: exclusive Unity time) and C-7 (owner: non-forecast wording) stay with the product owner. C-2b stays post-capture. C-1…C-5 technical items keep the timing in spec §7.1. This review adopts no tolerance, no fork or reload policy, books no Unity time, approves no specification and releases no P1 task. Scenario One remains the first external demo.

## 7. Recommended next step

The Manager may fold D-1…D-6 into the next planned revision of the four files, or accept them as tracked minors. None blocks sending C-1A, C-6 and C-7 to the owner, or moving to the technical decisions in spec §7.1. D-3 is the one that could change a future plan: ask the C-2a owner early whether a verification-only instrumented source copy is acceptable. Do not dispatch P1-01 or any source or Unity work. Update the plan/tasks G1 status lines (#555 delta review pending) only when the Manager records its own disposition.

**Dashboard / Drive sync: NOT VERIFIED.** No coordination, canon, dashboard or Drive file was changed. No DRAFT, source, P0 or fixture file was edited.
