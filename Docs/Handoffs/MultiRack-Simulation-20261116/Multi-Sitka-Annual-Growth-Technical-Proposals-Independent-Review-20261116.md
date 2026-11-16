# #566 — Independent technical review of the #562 Sitka annual-growth technical proposals

**VERDICT: APPROVED AS DRAFT QUALITY — with 3 material residuals and 12 minor corrections.** 0 blocking, 0 major, 0 FAIL. Of the 19 items T-01…T-19, **16 PASS and 3 are PARTIAL** (T-01, T-08, T-16).

**What this verdict is not.** It is a documentation review of a DRAFT. It is **not** approval of the P1 specification, not approval of any technical proposal as an accepted contract, and not Unity, compile or runtime evidence. **C-2b** (numerical parity) stays **OPEN** until a real Unity capture exists. **C-6** (Unity machine time) stays **OPEN and unbooked**. The **72 requirement boxes stay unchecked** and **N01–N13 stay unrun**. The specification stays **DRAFT / NOT APPROVED**. I changed nothing except adding this one file.

---

## 1. Identity, independence and claim

| Item | Record |
|---|---|
| Task | [#566](https://github.com/jermur788/CCF/issues/566); Manager disposition [comment 6910648868](https://github.com/jermur788/CCF/issues/566#issuecomment-6910648868) |
| Reviewer | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code (Agent SDK harness) inside T3 Code. The user instructed "Sonnet 5.5 High". I cannot read my own effort setting from inside the session, so treat **High as user-stated, not verified** |
| Independence | I did not author #555, #562 or any file under review. #562 and #555 were Claude Opus 5.5. This is not an Opus account and not the orchestration parent of the author |
| Registration | [RUNNING comment 6910693461](https://github.com/jermur788/CCF/issues/566#issuecomment-6910693461), posted before any source inspection |
| Host / worktree | Linux laptop `jer-laptop`; docs-only worktree `/media/jer/Files/CCF/Operations/CCF-566-SitkaTechReview-20261116` (Files, ext4). No Unity Library, lane or lease |
| Branch / start HEAD | `review/multi-sitka-annual-growth-technical-independent-20261116-v1` at `81470c5955c781752148dae4d6240483c15e3ee7` (equal to the remote tip, clean at start) |
| Live board | `origin/main` `584d383f` `active-tasks.md` refreshed before work: no other #566 or #562 claim, no overlapping writer on the four spec files |
| Method | `git show` / `git archive` plus plain file reads. **No** build, compile, test, Unity, Mono, lab or browser run. Two small arithmetic checks were done with `python3 -I -B` (§6). Both are labelled and are not project execution |

## 2. Pins inspected

| Pin | Value | Check |
|---|---|---|
| Candidate | `81470c59` | 4 modified + 1 added Markdown vs parent `9081e459` (§3) |
| Game source | `Assets` tree `824a1394c69404c847a9841d5f531e398589730b` | **Identical** at `81470c59`, `f0031848`, `fc18190b`, P0 `2659db99` and today's `origin/main` `584d383f`. Every `fc18190b` line cite in the spec therefore applies to current source. I read the files in this worktree |
| P0 lab | `2659db99e317d4aa93b51cf97131a61eab5fdeec`, extracted with `git archive` | A newer P0 head exists (finding F-3) |

Line citations below are `spec.md:N` at the candidate, or `Assets/ForestPrototype/<file>:N`, or `P0:<file>:N` for `Tools/MultiRackSitkaLab/src/...` at `2659db99`.

## 3. Scope and preservation (mechanical, independently re-run)

| Check | Result |
|---|---|
| `git diff --name-status 9081e459 81470c59` | Exactly **4 M** (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) and **1 A** (the #562 handoff). No other path |
| Register rows | 9 `^\| \*\*C-` rows in each file; SHA-256 prefix of those lines `d6365b4fe851f298` in all four, identical to the parent |
| Requirement boxes | `requirements.md`: **72 unchecked, 0 checked**. The other three files have none |
| Register status | C-1A and C-7 owner CONFIRMED (D-082); C-2b OPEN (`spec.md:302`); C-6 OPEN, "no lease or source task" (`:306`) |
| N01–N13 | Still "designed checks, not tests run"; the plan adds no new row (`plan.md` §10 maps T-items to existing rows) |
| Links | 38 relative links and heading anchors across the five files resolve (local GitHub-slug check; not a rendered GitHub check) |
| Approval language | The new §9 states "PROPOSED, NOT ACCEPTED" and selects no owner policy (`spec.md:347–349`) |

## 4. T-01…T-19 grades

Legend: **PASS** means the claim and the cited source agree and the proposal is internally consistent. **PARTIAL** means source facts hold but the proposal is incomplete or inconsistent.

| Item | Grade | Where | Evidence and risk |
|---|---|---|---|
| **T-01** lineage | **PARTIAL** | `spec.md:369, 391–395` | P0 Core public API as claimed: `LabFile.Parse` (`P0:Core/LabFile.cs:125`), `CanonicalJson.SnapshotHash` (`CanonicalJson.cs:148`), `ForestSnapshot`/`LabFileContent` public (`Models.cs:104, 168`); `InternalsVisibleTo` only `SitkaLab.Core.Tests` (`SitkaLab.Core.csproj:7`). **Gaps:** F-3 (stale pin, proof does not detect working-tree writes, build-config traps) |
| **T-02** trust anchor | **PASS** | `:370, 397–423` | `FixtureCatalog.LoadFrom` hashes whatever bytes it reads and never reads `MANIFEST.sha256` (`P0:Core/FixtureCatalog.cs:20–44`). The eight-step order is sound: length is checked on the descriptor before reading, SHA-256 is checked **before** `LabFile.Parse`, and `Parse` already re-checks the embedded snapshot hash (`LabFile.cs:125–138`). Wording fix m-2 |
| **T-03** derived catalog | **PASS** | `:371, 425` | Separate table, format, suffix, `p1d-` namespace and import function; refuses crossover both ways. No derived fixture exists yet (correct) |
| **T-04** read path | **PASS** | `:372, 415–417` | The bytes that are hashed are the bytes that are parsed, so a path swap after the open cannot forge content. No-follow handle reads are therefore defence in depth for import. Intermediate path components are not covered by `O_NOFOLLOW`; the worst result is refusal, not forgery |
| **T-05** root, files, `fsync` | **PASS** | `:373, 427–432` | P0 has no `fsync`, no owner or mode check (grep of `P0:Core/NoFollowFiles.cs`: none). It does classify the opened descriptor with `statx(fd,"",AT_EMPTY_PATH)` (`:117`) and creates `0600` temp files with `O_CREAT\|O_EXCL\|O_NOFOLLOW` (`:153`). The spec's description is accurate. Minor m-8 |
| **T-06** host | **PASS** | `:374, 434` | P0 guards confirmed: loopback listen (`P0:Host/LabHost.cs:71`), exact Host (`:110, 115`), Origin and Sec-Fetch-Site, POST needs Origin and `X-Sitka-Lab: 1` (`:121`), JSON only. P0 default port 5187 (`:23`), so 5188 is the right separation |
| **T-07** serialization | **PASS** | `:375, 436–441` | P0 strict reader refuses BOM, comments, trailing commas, duplicate keys (`P0:LabFile.cs:13–22`). Float-bits-only state and no wall-clock fields remove two real replay hazards. Safe-integer limit suits a browser client |
| **T-08** archive and caps | **PARTIAL** | `:376, 443–473` | Compact-with-replay is the right shape. **But** F-1 (caps inconsistent, per-event hash cost missing) and F-2 (replay ties save validity to the host maths library) |
| **T-09** rejected IDs consume | **PASS** | `:377, 475–497` | Source confirmed: P0 has no command IDs, and its rejection is audit-only (`P0:Core/LabSession.cs:229–239`; `Engine.cs:161–166`; reasons list `CanonicalJson.cs:47–51`). Dedup-before-revision ordering is correct. Golden vectors separate the alternatives. Wording m-1 and UI contract m-12 |
| **T-10** dedup outside state hash | **PASS** | `:378, 496–497, 212–224` | Consistent with P0: `PlanExecutionRejected` changes no state. §5.2 row now reads "No" with the change disclosed. Duplicate-ID history refusal and replayer check (`:497`) close the gap |
| **T-11** growth-driving CI | **PASS** | `:379, 514–528, 535–536` | Verified, see §5. `InvalidateCompetition()` then getter is the right pre-step read. Minor m-3 |
| **T-12** victim order | **PASS** | `:380, 540` | Hazard victims first in ordinal order, then boundary in suppression-then-ID order (`ForestEcologyController.cs:1098–1131`); kills run in list order (`:1137–1157`); `MortalityApplied` fires inside `ApplyMortality` (`ForestTree.cs:562`). `HazardDeaths` set before the boundary loop (`:1108`). Cite drift m-4 |
| **T-13** `LastAdultMortality` | **PASS** | `:381, 539` | `AdultMortalityAccount` has exactly the claimed fields (`AdultMortalityAccount.cs`); assigned at `:1065`; `StemsPerHectare` is `int / float` then widened (`:1222`). Only for growth model ≥1 |
| **T-14** S/p blocked | **PASS** | `:382, 543, 547` | S is an f64 local (`:1092`), `meanVulnerability` (`:1096`) and `probability` (`:1102`) are locals; `GetCurrentSuppression` (`:1825–1831`) and `RecordSuppressionYear` (`:1033`) are f32 look-alikes. The bracket check is honest about its limit; its power is weaker than it sounds (Q4) |
| **T-15** instrumented copy | **PASS** | `:383, 564–575` | Not necessary for the minimum oracle; five admissibility conditions are sound, including the honest "R2 = R0 on outputs is necessary, not sufficient" (`:573`). Minor m-10 |
| **T-16** maths library | **PARTIAL** | `:384, 554, 603` | `Math.Pow`/`Math.Exp` sites are real (`SitkaGrowthModel.cs:41, 48, 74, 105`). Recording OS/libm/CPU in the capture header is right. **But** F-2: the same issue also hits P1's own replay on load, and the Windows player is a different platform from the Linux Editor capture |
| **T-17** store arithmetic | **PASS** | `:385, 555` | Source confirmed: height stored as `clamp(h + (float)(h·r) − h)` (`ForestEcologyController.cs:1029`; `ForestTree.cs:627`). The hazard is real but narrower than stated (m-7) |
| **T-18** datum statuses | **PASS** | `:386, 545` | Seven statuses, only four count as source observations, `notEvaluated`/`unavailable` never carry a number. Matches the source's short-circuits (`:1084, 1103`). Naming m-11 |
| **T-19** C-3/C-4/C-5 classes | **PASS** | `:387, 577–596` | Each row checked against source or arithmetic: see §6. Evidence classes are honest (`FUE`, `UND` where unknown) |

## 5. The source claims the packet asked me to examine

**5.1 `InvalidateCompetition()` and stale CI — CONFIRMED.** `competitionCurrent` becomes false only in `QueueLivingCanopyRebuild` (`ForestEcologyController.cs:441`), `RestoreEcologyState` (`:514`), `ResetForDeterministicRun` (`:550`) and `InvalidateCompetition()` (`:563–566`). `AdvanceOneYear` calls `UpdateCompetition` unconditionally (`:475`) and sets it true (`:476`; also `:998`). Growth does not clear it. So after a year with no death or felling, a plain `GetCompetitionIndex` returns last year's pre-growth CI. The proposed pre-step read (`InvalidateCompetition()` first) is correct, and the next `AdvanceOneYear` recomputes the same function on the same ordinal-sorted set (`FindTrees`, `:875–886`, `CompareOrdinal` `:863–866`), so bit identity is stronger than "SDI": it is one method on identical inputs. The spec is conservative.

**5.2 Dead and absent trees return `0f` — CONFIRMED.** `GetCompetitionIndex` returns `0f` when `!tree.IsLiving` (`:1813`) and when the cache lacks the tree (`:1820`). `OnTreeMortality` removes the victim's CI and growth entries (`:433–434`) and invalidates the cache (`:441`). A post-death getter read both writes the forbidden fabricated zero and triggers a recompute on survivors only.

**5.3 Real S/p versus look-alikes — CONFIRMED.** Mortality S is `double s = 1.0 − 1.0/(1.0 + ci/Ci50)` at `:1092`; `Vulnerability` uses `Math.Pow` in double and `AnnualDeathProbability` is a double expression (`SitkaGrowthModel.cs:103–113`). The public `GetCurrentSuppression` clamps in float (`:1825–1831`); `RecordSuppressionYear` is a float clamp (`:1033`). Neither is the mortality S. The pass returns before S exists when `DensityPressure <= 0.0` (`:1084`); the roll runs only when `probability > 0.0` (`:1103`).

**5.4 Observation points — CONFIRMED.** `LastAdultMortality` is public (`:154`) and replaced each pass (`:1064–1065`). `ForestTree.MortalityApplied` is a public static event (`ForestTree.cs:31`). `SimulationRandom.Roll` is public, integer-only, and returns `(x & 0xFFFFFF)/16777216f` for model 0 and `(state >> 40)/16777216f` for model 1 (`SimulationRandom.cs:41–55`): exact 24-bit fractions on both models. Private cache reads (`competitionIndex`, `annualDbhGrowth`) stay populated for survivors after the step because `OnTreeMortality` only removes victims.

**5.5 Reflection acceptability — my recommendation: ADMIT, with conditions.** The packet asks for an explicit reviewer ruling. Read-only reads of the two private dictionaries after `AdvanceOneYear` returns do not change the controller's IL. I recommend admitting them if all of these are written into §9.5: (a) reads happen only after the step returns; (b) `FieldInfo` is resolved once at harness start by exact name and **fails closed** if missing, because private names are not API, and the harness pins the source blob hash; (c) values are copied out and keyed by `TreeId` sorted ordinal (do not rely on `Dictionary` enumeration order); (d) the R1 = R0 bit-for-bit control stays mandatory. This is a recommendation to the Manager, not a ruling that adopts the proposal.

**5.6 Float precision and libm — see F-2.** The risky transcendental sites are `SitkaGrowthModel.cs:41` (static `ClassIIIb0`, computed once at type init), `:48`, `:74`, `:105`. That Mono calls the platform libm for them is still **SDI** (not proven here).

**5.7 Full replay and rejection semantics — see T-09, T-10, G-01…G-08.** The recorded/refused split is coherent. One cross-check I ran by hand: G-02 (a rejected ID stays rejected even after the plan becomes executable) is correct behaviour for an idempotency key, provided the UI mints a new ID per user intent (m-12).

**5.8 Budget — see F-1.** **5.9 C-3/C-4/C-5 evidence classes — see T-19 and §6.**

## 6. Independent arithmetic and source facts behind T-19 and T-17

| Claim | Check | Result |
|---|---|---|
| Area `standSize*standSize/10000f` (`ForestEcologyController.cs:73`) | My binary32 arithmetic | 40 m → `0.1599999964237213` (bits `3e23d70a`); 80 m → `0.6399999856948853` (bits `3f23d70a`). These are **predictions to compare with the future Unity readback (FUE)**, not observations |
| Top-height count `k = ceil(100·areaHa)` = 16 and 64 | Same area values, computed in double and in float | 16 and 64 both ways (0.64f is below 0.64, so no spurious 65) |
| Initial crown `max(0.1, 0.9415 + 0.07635·D)`, `usePowerLawCrown: 0` | `Species/SitkaSpruce.asset:23–25` | Matches |
| Demand defaults to Reference | `TreeSpeciesDefinition.cs:30`; asset has no `growingSpaceDemand` | Matches |
| Site = 1, no writer | `SiteProductivity` is a field initialiser only (`ForestEcologyCell.cs:48`); reads at `ForestEcologyController.cs:1960` fall back to 1f | Matches; the spec's "no source writer" holds for source text (scene data not inspected) |
| Cell bound half-open | `StandGeometryModel.CellIndex` refuses `x >= axis` (`:46–56`) | Matches |
| Approach path has no tree reference | `StandApproachPath.cs` has no `ForestTree` token | Matches |
| "All living stems" in CI and mortality | `FindTrees` excludes biologically dead (`:877–882`); CI counts `!IsStump` over that set (`:955`); mortality uses `IsLiving` (`:1068`) | Same set |
| Original fixtures | `oracle-quarter-ha` 3 trees, `sitka-flat-demo-v1` 162, `sitka-flat-perf-2000-v1` 2,000; every tree `ageAtInventoryYears` = 28 | The "≤ 2,000 trees" claim holds. Age 28 matters for T-17 (m-7) |
| Height ratio `H(a+1)/H(a)` from the Class III form (`SitkaGrowthModel.cs:48`) | My arithmetic | age 1: 2.86; age 2: 1.82; age 5: 1.29; age 28: ≈1.03 |

## 7. Findings

### Material (fix or decide before the proposals are adopted)

**F-1 (T-08): the proposed caps are inconsistent and the replay budget omits its dominant term.**
- `spec.md:464–469` offers four numbers as if derived. Two of them are P0's own constants: `MaxEvents` 200,000 and `MaxFileBytes` 64 MiB (`P0:Core/Models.cs:27–28`). §4.1 and REQ-B09 say P0's caps are "not inherited automatically" yet list only `MaxTrees`/`MaxFileBytes` (`spec.md` §4.1 file-capacity paragraph); `MaxEvents` is never mentioned as a P0 value.
- 64 MiB ÷ 200,000 = 335 bytes per event. The envelope in `spec.md:210` and `:489–494` carries a `commandDigest`, a previous-event digest, and pre- and post-state hashes: four 64-hex strings are 256 bytes before keys, ID or payload. A realistic canonical event is therefore several hundred bytes, so the **byte cap binds first** and 200,000 events is unreachable (my estimate puts it near ~130,000–150,000; unmeasured). Safe, but the stated budget is not a derived budget.
- §9.3 costs replay as Hegyi pair checks only (5×10⁹ for 5,000 trees × 200 years; `:473`). P0 hashes the full snapshot after **every** event (`P0:Core/LabSession.cs:131`) and P1's envelope carries pre/post state hashes (`:210`). With about 200 B per tree in the spec's own size model (`:456`), a 5,000-tree state is about 1 MB per hash. At ~130,000 events that is on the order of 100+ GB serialised and hashed per full load. At an assumed 0.1–1 GB/s, **minutes to tens of minutes**, from a synchronous load request. This is my order-of-magnitude estimate, not a measurement, but it outweighs the Hegyi term the spec does cost.
- **Minimum correction:** (1) state worst-case canonical bytes per event and derive the event cap from the byte cap, or drop `MaxEvents` as redundant; (2) add the per-event state-hash cost to the replay budget, or specify an incremental or per-annual-event-only state hash; (3) say which numbers equal P0's and why; (4) tie the 200-year cap to an experiment need (none is stated; see Q5); (5) reconcile the two compact-size models (§4.1 table "≈20–40 B per tree-year" vs §9.3 `200·N₀ + 650·Y + 120·D + W`).

**F-2 (T-16 and T-08): replay-on-every-load makes save validity depend on the host maths library, and the capture platform differs from the player.**
- T-16 limits the libm concern to the **capture header** and "any secondary harness" (`spec.md:384, 554`). But §9.3 and §5.2 load by **recomputing every year** and checking every hash. If P1's adapter uses `Math.Pow`/`Math.Exp` (it must, to follow the source), a save written under one glibc or .NET build may fail replay under another. That failure would surface as a hash mismatch, which the design treats as corruption or tamper. P1 is meant to run on Linux and on Windows via WSL2 (`spec.md:332`), and distribution upgrades change glibc.
- My background knowledge is that glibc's `pow`/`exp` are not guaranteed bit-identical across versions or CPU feature paths. **I have not verified that here**; it is exactly what the proposed header would have to measure.
- Separately, the production player is Windows x86-64 Mono (`Docs/Project/unity-project-overview.md:328`), while the proposed oracle is a Linux Editor capture. Linux-Editor parity does not establish Windows-player parity, and the spec does not say so.
- **Minimum correction / owner-Manager decision:** record a platform fingerprint (OS, libc/libm identity and version, runtime build, CPU class) in every P1 save; on mismatch, refuse with a **distinct** code (for example `ReplayPlatformMismatch`) that is not reported as tamper and does not discard the file; decide whether cross-platform loads are supported at all; and add one sentence to §9.5/C-2b that Linux-Editor capture is evidence for that platform only.

**F-3 (T-01): the lineage pin and its proof are weaker than stated.**
- `spec.md:355` says no newer P0 lab head existed. I found `origin/fix/multi-sitka-p0-save-response-r3-20261116-v1` and `origin/docs/multi-sitka-p0-operator-readiness-20261116-v1` at `861b81d3` (two commits after `2659db99`, committed 2026-11-14 23:12–23:13 UTC). Their diff to `Tools/MultiRackSitkaLab` is **README only** (`git diff --stat`), so source and fixtures are byte-identical today. A bounded P0 save-response repair (#567, READY / NOT RUNNING per the orchestration ledger) will move the head again, possibly in Core or Host.
- The proof `git diff --quiet <P0 pin> HEAD -- Tools/MultiRackSitkaLab` (`spec.md:394`) compares two **commits**. It cannot see uncommitted edits, untracked files, or ignored build output. A `ProjectReference` to P0 Core makes a P1 build write `bin/` and `obj/` inside the P0 tree; those are ignored by P0's `.gitignore`, so the proof passes while the P0 tree is dirty.
- P0 hand-written project files need `!*.sln` / `!*.csproj` because the repository root ignores them (`P0:.gitignore:1–4`), and P0 carries its own `global.json` and `Directory.Build.props`. A sibling P1 directory gets none of these automatically.
- **Minimum correction:** add `git status --porcelain --ignored -- Tools/MultiRackSitkaLab` (empty) to the handoff proof; build P1 so P0 outputs are redirected (or copy-reference); require the P1 tree to carry its own `global.json` and `.gitignore` re-includes; name which head the Manager picks (`2659db99`, `861b81d3` or post-#567) as an explicit decision, not "today's head".

### Minor (documentation only)

- **m-1 (T-09):** the §9.4 class table lists "revision or epoch mismatch" as an audit-only rejection (`:484`) and "envelope `expectedForestRevision` ≠ current" as a refusal (`:483`). They are different things: P0's `RevisionMismatch`/`EpochMismatch` compare the **plan's** recorded base revision and approval epoch with current state (`P0:Core/Engine.cs:68–71`); the envelope field is the client's view. Name both so G-01…G-08 cannot be implemented with the two swapped.
- **m-2 (T-02):** "P0 `MANIFEST.sha256` (… not read by P0)" (`:370`) is true of the **runtime**. P0's unit test reads and compares it (`P0:tests/Core.Tests/FileFormatTests.cs:24–32`). The conclusion stands; fix the sentence.
- **m-3 (T-11, §9.5 scene inventory):** the precondition list omits the controller's own `timeLapseEnabled` serialized field (`ForestEcologyController.cs:27`), which advances years from `Update` once enabled (`:256–262`). The "+1 year" readback would catch it, but the inventory should require `timeLapseEnabled == false`. The caller list also names `CropTreeCompetition` (`spec.md:526`), which only mentions the getter in a comment (`UI/CropTreeCompetition.cs:246`); harmless.
- **m-4 (T-12):** `KillDensityVictims` is at `:1137–1157` (cited `:1134–1153`); hazard loop `:1098–1108`. Drift of 3–4 lines.
- **m-5 (T-18):** state that the `Roll` year argument is the **post-increment** `ecologicalYear` (`:467`) the step lands on, and that the seed is `simulationSeed`, not the capture's own seed field.
- **m-6:** §9.5 should say the harness must read the roll-model integer (`rngModelVersion`) from the controller, not assume model 1; model 0 and model 1 give different rolls (`SimulationRandom.cs:41–55`).
- **m-7 (T-17):** `h + (x − h) = x` exactly in strict binary32 whenever `x/2 ≤ h ≤ 2x` (Sterbenz). From §6 the ratio exceeds 2 only at age ≤ 1; every P0 fixture tree is age 28 (ratio ≈ 1.03). So the store-arithmetic hazard is confined to age-0/1 vectors. Say so, and keep the small-age vector (`spec.md:586`).
- **m-8 (T-05):** state the filesystem requirement (local POSIX filesystem; refuse DrvFs, 9p and network mounts, where `st_uid`, mode and link counts are not reliable), and perform the owner/mode/`fsync` checks on `fstat` of the **opened root handle**, not on the path.
- **m-9:** §4.1's compact table (≈20–40 B per tree-year) and §9.3's formula describe the same layout with different models. Keep one.
- **m-10 (T-15):** R2 is built from a different source tree in a separate Unity project, so "three runs in the same Editor" (`:557`) means the same Editor binary and host, not the same running session. Say so.
- **m-11 (T-18):** `observedAtEvaluation` is used for the `MortalityApplied` order (`:540`), which is an application order, not an evaluation of S/p. Rename or footnote.
- **m-12 (T-09):** state who mints `commandId` (the client) and that a deliberate re-click uses a new ID while only transport retries reuse one. That is the contract G-02 depends on.

## 8. Answers to the six questions in the #562 handoff §7

1. **Is G-02 right?** Yes, as an idempotency rule. Pair it with m-12 so the UI cannot reuse an ID for a new intent.
2. **Is a dedup index outside `P1-STATE-V1` sound?** Yes, given full replay on load: the index is a pure function of the history, the history digest covers every primary event's `commandId` and `commandDigest` (`:496`), and the replayer refuses a duplicate ID and any `ExecutionRejected` whose pre/post state hashes differ (`:497`). It keeps Mode A state-hash comparisons independent of how many rejections occurred.
3. **Is read-only reflection after `AdvanceOneYear` admissible?** I recommend yes, with the four conditions in §5.5. The Manager rules.
4. **Are Tier-1 outputs plus the roll/p bracket enough to decide C-2b later?** Tier 1 observes CI, stored DBH/height/crown, RD/P/death counts, victim order and exact rolls. That is enough to decide **output-level** parity (bits or tolerance) and death-set identity, so the oracle need not wait for an instrumented copy. The bracket check is falsifiable but **low power**: a roll flips only if it lies between true `p` and `p_adapter`, which has probability about `|Δp|` per tree-year. It will catch model-level errors of roughly 10⁻⁴ or more over a few thousand tree-years and cannot see last-bit differences (which in turn almost never change an outcome). C-2b's text must say the bracket is **not** an observation of Unity's `p`. If C-2b wants bit-exact S or p, only an instrumented copy provides it.
5. **Are 5,000 trees and 200 years right?** 5,000 covers 0.64 ha at about 7,800 stems/ha (4,992) and the largest original fixture (2,000), so it sits at the dense edge by design. 200 years has no stated experiment behind it; I cannot judge it from the source. This is an **owner/Manager decision**, to be made together with F-1.
6. **Should P1 add `fsync` and root ownership/mode checks?** Yes to both: `fsync` of the temp file and then the directory after `renameat` is standard durability and cheap here. The ownership/mode check is good defence for a local root. State the filesystem requirement (m-8). Both are new P1 properties and need their own qualification; they are not inherited from P0.

## 9. What the Manager can adopt without further proof

| Proposal | Can be adopted as the working design basis? |
|---|---|
| T-02, T-03, T-04 trust tables and read path | **Yes**, subject to m-2. Values are still pending and need the import owner's preparation and file-security review |
| T-06, T-07 host and serialization | **Yes** |
| T-05 root, `fsync`, mode checks | **Yes**, subject to m-8; qualification needed in P1-03 |
| T-09, T-10 rejected IDs and dedup index | **Yes** as the design to implement and test, subject to m-1 and m-12. Still needs the golden vectors to pass |
| T-11, T-12, T-13, T-18 capture observability | **Yes** as the Tier-1 contract, subject to the reflection ruling (§5.5), m-3, m-5, m-6, m-11. Nothing is observed until a real capture exists |
| T-14, T-15 S/p blocked; instrumented copy | **Yes** as written (the BLOCKED statement is correct); the copy stays unauthorised and needs #424 plus a C-6 allocation |
| T-17 store arithmetic | **Yes**, with m-7 |
| T-19 evidence classes | **Yes** as labels; the SA/UND rows still need their named reviewers |
| **T-01** lineage | **No** until F-3 is applied and the Manager names the P0 head |
| **T-08** archive and caps | **No** until F-1 and F-2 are applied. Compact-with-replay as a *shape* is fine |
| **T-16** libm | **Only** the capture header part; F-2 must be settled before any save-compatibility claim |

## 10. Residual decisions and gates

**Manager / owner decisions**
- Which P0 head P1 branches from (`2659db99`, `861b81d3`, or after #567), and how P1 is told if P0 moves.
- Whether saves are portable across hosts, and the refusal code and wording when the platform fingerprint differs (F-2).
- The 200-year cap and the 5,000-tree cap against actual experiments, and whether to cap events by bytes only (F-1).
- Whether `fsync` and the root ownership/mode check are accepted as new P1 requirements (Q6).
- The reflection ruling (§5.5).

**Still open by design**
- **C-2b** parity: needs a real Unity capture. **C-6** machine time: unbooked; this review requests no slot.
- The S/p/mean-vulnerability/boundary-RD/crown intermediates stay BLOCKED as observed values unless #424 authorises an instrumented copy.
- That Mono uses the platform libm remains SDI; the Windows-player behaviour is unproven.
- Trust-table values, derived fixtures, the Broad–Lynch field method, the GROWFOR executable identity and Irish predictive validity remain unverified, as the DRAFT already says.

## 11. Standard handoff block

```text
REVIEW HANDOFF
Task: CCF #566 independent technical review of #562 Sitka annual-growth proposals
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5), Claude Code in T3 Code; effort High is user-stated, not verified
Role: Reviewer (documentation only)
Branch: review/multi-sitka-annual-growth-technical-independent-20261116-v1
Base: 81470c5955c781752148dae4d6240483c15e3ee7 (exact #562 candidate)
Context commit: not stated in the packet; the candidate commit above is the reviewed input
HEAD: recorded in the GitHub comment after this file is committed
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-Annual-Growth-Technical-Proposals-Independent-Review-20261116.md
WHAT CHANGED: one Markdown file added
SERIALIZED UNITY FILES TOUCHED: none
SIMULATION / SAVE IMPACT: none
MUST-NOT-CHANGE HASHES / ANCHORS: register rows d6365b4fe851f298 and 72 unchecked boxes unchanged in the four DRAFT files (not touched); Assets tree 824a1394c69404c847a9841d5f531e398589730b unchanged
VERIFICATION RUN: git diff scope, register-hash and checkbox counts, relative-link check, source reading at the pins above, two arithmetic checks with python3 -I -B
VERIFICATION NOT RUN: Unity, compile, test, runtime, lab, browser, Mono, GitHub rendered-page check of this file
SPEC CONFORMANCE: NOT APPLICABLE (the specification is not approved)
KNOWN LIMITATIONS: static source reading only; performance figures in F-1/F-2 are order-of-magnitude estimates, not measurements; the glibc behaviour in F-2 is background knowledge, unverified here
DECISIONS NEEDED: see §10
Ready for review: Yes
RECOMMENDED NEXT STEP: a docs-only repair round for F-1, F-2, F-3 and the minors, then a focused delta check; no implementation, fixture or Unity time is released by this review
Dashboard / Drive sync: NOT VERIFIED
```
