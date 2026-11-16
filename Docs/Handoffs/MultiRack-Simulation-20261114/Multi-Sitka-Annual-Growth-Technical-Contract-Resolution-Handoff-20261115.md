# #562 handoff — Sitka annual growth technical contract proposals (C-1, C-2a, C-3, C-4, C-5)

**Result: HANDED-OFF — PROPOSED TECHNICAL CONTRACTS, NOT ACCEPTED.** Docs only. The four DRAFT files now carry source-grounded **proposals** for technical C-1, C-2a and C-3/C-4/C-5 in a new [spec §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#9-562-technical-contract-proposals-c-1-c-2a-c-3-c-4-c-5--proposed-not-accepted). Nothing is approved. C-2b stays OPEN until a real Unity capture exists. C-6 stays OPEN and unbooked. The specification stays **DRAFT / NOT APPROVED**. Next step: an **independent technical-spec delta review** by a reviewer who is not Claude Opus.

## 1. Identity, claim and pins

| Item | Record |
|---|---|
| Task | [#562](https://github.com/jermur788/CCF/issues/562), [binding packet](https://github.com/jermur788/CCF/blob/docs/manager-sitka-growth-technical-design-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/Multi-Sitka-Annual-Growth-Technical-Design-Resolution-Packet-20261115.md) |
| Author | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code inside T3 Code. The packet recommended Codex Sol High; this is a different model, disclosed in the claim |
| Independence | Opus also wrote the #555 repair. The reviewer of this work must **not** be Claude Opus. Claude Sonnet 5.5 High is recommended. A non-Codex reviewer is preferred, because some #552 (Codex) proposals are judged here |
| Claim | [RUNNING registration](https://github.com/jermur788/CCF/issues/562#issuecomment-6903400131), posted before any source inspection or writing |
| Host / worktree | Local Linux laptop; new docs-only worktree `/media/jer/Files/CCF/Operations/CCF-562-SitkaTechContract-20261115` (Files, ext4). No Unity Library, lane or lease |
| Branch / start | `docs/sitka-annual-growth-technical-contract-resolution-20261115-v1` at `9081e4591fbae90192149756bcb16e8570e26e47` (equal to the remote head; clean, 0 diff) |
| Live board | `origin/main` `f0031848` `Docs/Project/coordination/active-tasks.md`: no other owner of the four spec files or this handoff path |
| CCF source inspected | `main` `f0031848b839888674a8033a60b1044150aa7c6f`. Its `Assets` tree `824a1394c69404c847a9841d5f531e398589730b` is **identical** to the `Assets` tree at `fc18190b`, so the spec's older `fc18190b` line citations still hold |
| P0 lab inspected | `2659db99e317d4aa93b51cf97131a61eab5fdeec` (`feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1`); no newer P0 head on `origin` |
| Method | `git show` / `git archive` into a scratch folder; reading only. **No** build, compile, test, Unity, Mono, .NET, lab, browser or fixture run |

## 2. Technical decision and trade-off matrix (summary)

Full matrix with alternatives and source lines: [spec §9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix). Evidence classes: **DSC** direct source check, **SDI** source-derived inference, **SA** synthetic assumption, **FUE** future observed Unity evidence, **UND** unsupported, needs decision.

| C-item | Proposal | Evidence | Who must review |
|---|---|---|---|
| C-1 lineage (T-01) | P1 sibling directory on a branch cut from the exact P0 head (`2659db99` today, not `main`); P0 Core referenced read-only through public `LabFile.Parse` and `CanonicalJson.SnapshotHash`; `git diff --quiet <P0 pin> HEAD -- Tools/MultiRackSitkaLab` in every P1 handoff | DSC | Manager (lineage) + file-security reviewer |
| C-1 trust (T-02…T-04) | Compiled-in original-fixture table (ID, file name, exact length, file SHA-256, P0 snapshot hash, P0 format/schema, P0 revision as provenance, review reference); eight-step all-or-nothing check with length checked **before** reading; separate derived-fixture table, format and `p1d-` namespace; P1's own handle-relative read, not `FixtureCatalog` | DSC: `FixtureCatalog.LoadFrom` hashes whatever it reads and never reads `MANIFEST.sha256` | File-security reviewer; values prepared later by the import owner |
| C-1 files and host (T-05…T-07) | `.p1-data/`; descriptor classification as in P0; new: root owner/mode check, file and directory `fsync`; refusal before any temp file; port 5188, never 5187; `X-Sitka-P1-Lab` header; strict typed canonical JSON with float **bits only** in state; no wall-clock time anywhere | DSC of P0 mechanisms; the new checks are new requirements | File-security / browser reviewer |
| C-1 archive (T-08) | **Compact** authoritative archive, full replay on load, separate non-authoritative trace export. Caps 5,000 trees, 200 years, 200,000 events, 64 MiB | SDI arithmetic; replay time must be measured in P1-03 | Replay + file reviewer |
| C-1 rejected IDs (T-09/T-10, #556 D-6) | A **recorded** rejection consumes its ID; a **refused** request consumes nothing; dedup lookup **before** revision checks; only the primary event of a transaction carries the ID; dedup index rebuilt from history, **outside** `P1-STATE-V1`, so an audit-only rejection leaves the state hash unchanged (as in P0). Golden vectors G-01…G-08 | DSC: P0 has **no** command IDs, and its `PlanExecutionRejected` changes no state | Independent replay reviewer |
| C-2a capture (T-11…T-18) | Tier 1 (no source change): pre-step CI via `InvalidateCompetition()` + getter; victim order from `ForestTree.MortalityApplied`; RD/P/N/Dq from `LastAdultMortality`; exact rolls from `SimulationRandom.Roll`; optional read-only post-step cache reads. Seven datum statuses; scene inventory and OS/libm/CPU in the header; R0 (bare) vs R1 (observed) bit-exact | DSC; bit identity of pre-step and in-step CI is SDI → FUE | Numerical reviewer; explicit ruling on read-only reflection |
| C-2a S/p (T-14/T-15) | S, Vulnerability, mean vulnerability, p, boundary-loop RD and crown intermediates are **BLOCKED** as observed values without source instrumentation (they are locals). Substitute: falsifiable roll/p bracket check against the future adapter. Instrumented copy **not necessary** for the minimum oracle; **admissible only** under five conditions (spec §9.6) | DSC | Source owner #424 + numerical reviewer, inside a C-6 allocation |
| C-3/C-4/C-5 (T-19) | Per-item evidence class and named gate (spec §9.7) | Mixed, labelled | Source/spec reviewer; science reviewer for SI/H before P1-04 |

## 3. New source findings (not in earlier rounds)

1. **Second CI-getter trap.** `competitionCurrent` is cleared only by a living-tree removal, `RestoreEcologyState`, `ResetForDeterministicRun` or the public `InvalidateCompetition()` (`ForestEcologyController.cs:441, 514, 550, 563–566`). Growth does not clear it. After a year with no death or felling, a plain `GetCompetitionIndex` read before the next step returns **last year's** CI. A pre-step read must call `InvalidateCompetition()` first. (M-3 covered only the post-death recomputation.)
2. **The getter returns `0f` for a dead or absent tree** (`:1811–1821`). Reading a victim's CI through it writes exactly the fabricated zero the spec forbids.
3. **Public look-alikes are not the mortality S.** `GetCurrentSuppression` (`:1825–1831`) and `RecordSuppressionYear` (`:1033`) use clamped f32 expressions. Mortality S is an f64 local (`:1092`).
4. **Public observation points exist for most outputs.** `LastAdultMortality` (`:154`, set at `:1065`) exposes N/ha, Dq, RD, P, route and death counts. The static `ForestTree.MortalityApplied` event (`ForestTree.cs:31, 562`) gives exact kill order. Hazard victims come first (`:1108`).
5. **N/ha is an `int / float` expression stored as `double`** (`:1222`), and **height is stored as `h + ((float)(h·r) − h)`** (`:1029`, `ForestTree.cs:627`). Both are codegen-sensitive; captures must record stored values and adapters must not shortcut.
6. **`Math.Exp` / `Math.Pow`** feed Class III height, RD and Vulnerability (`SitkaGrowthModel.cs:41, 48, 74, 105`). Results can depend on the platform maths library, so the capture header must record OS, libc/libm and CPU. (That Mono uses the platform libm is SDI.)
7. **P0 has no command IDs.** Its audit-only `PlanExecutionRejected` leaves the snapshot hash unchanged (`LabSession.cs:229–239`, `Engine.cs:161–167`). P1 command IDs are a new design, not parity.
8. **P0 already classifies the opened descriptor** (`statx(fd, "", AT_EMPTY_PATH)`, `NoFollowFiles.cs:96–129`) but has **no `fsync`**. P0's `FixtureCatalog` reads by path (`FixtureCatalog.cs:25`).
9. **Enlarged80 approach path:** `StandApproachPath` has no `ForestTree` reference, so with no spawner nothing moves hand-placed trees (DSC + SDI). The readback stays as a cheap check.
10. **Grid bound:** `StandGeometryModel.CellIndex` is half-open `[-S/2, S/2)` (`:46–56`), closing #556 note (c) for the grid. Adult growth falls back to site 1 off-grid, so the bound is a fixture validity rule.
11. **`LogSummary` does not call the CI getter.** `StandDiagnostics()` does (`:2006, 2015`), so the harness must not call it before post-step reads.

## 4. What changed, file by file

| File | Change |
|---|---|
| `spec.md` | #562 header note; new **§9** (§9.1 matrix, §9.2 trust/lineage/files/host, §9.3 archive and caps, §9.4 command IDs and golden vectors, §9.5 capture observability, §9.6 instrumented-copy admissibility, §9.7 C-3/C-4/C-5 evidence classes, §9.8 still blocked). Targeted edits: §4.1 trust-anchor and capacity pointers; §5.1 observation points 1–3 (trap 2, `InvalidateCompetition()`, reflection proposal); §5.2 hash-membership row for the dedup index (**changed from Yes to proposed No**), the rejected-ID consequence bullet and the load bullet; §8 gate row |
| `plan.md` | #562 header note; G1 status; N01, N02, N03 and N08 positive and negative controls sharpened; new **§10** trace from T-items to N-rows |
| `tasks.md` | #562 header note; P1-S0 status and reviewer rule; stale "#555 awaiting delta review" readiness line corrected; P1-01 and P1-03 deliverables point to spec §9 |
| `checklists/requirements.md` | #562 header note; REQ-B08, REQ-B09, REQ-C13 and REQ-E11 text name the proposals; new **§J** trace (no checkboxes) |
| this handoff | added |

## 5. Preservation and mechanical checks

| Check | Result |
|---|---|
| Decision register | Nine `C-` rows in each of the four files; SHA-256 prefix of the row lines `d6365b4fe851f298` in all four, **the same as at the start HEAD**; `git diff` touches no register line. C-1A and C-7 owner CONFIRMED (D-082); C-2b and C-6 OPEN |
| Requirement boxes | **72 unchecked, 0 checked**; no box added |
| N01–N13 | All still **proposed, not run**; no new N-row |
| Relative links and anchors | All relative links and heading anchors in the four files resolve by a local GitHub-slug check. Not a rendered GitHub check |
| Scope | Only the four DRAFT files and this handoff. No `Assets/**`, `Tools/**`, P0, fixture, main, Decision Log, milestone or board edit |
| Branch diff vs `9081e459` | Expected 4 modified + 1 added Markdown; confirmed after push (see the #562 closing comment) |

## 6. Still blocked or open

- **C-2b** OPEN until real capture; **C-6** OPEN, unbooked. No Unity, lock or machine requested.
- S/p/mean-vulnerability/boundary-RD/crown intermediates **BLOCKED** as observed values without an admitted instrumented copy.
- Read-only reflection needs an explicit reviewer ruling. Without it, the survivor CI cross-check is limited to no-death years.
- Mono's use of the platform libm is SDI only.
- Trust-table values not prepared; no derived fixture exists.
- P1 base lineage is a Manager decision.
- `fsync` and root owner/mode checks are new P1 requirements to accept or drop.
- Caps and replay-time budget are proposals; P1-03 must measure load time.
- Broad–Lynch top-height field method, GROWFOR executable identity and Irish predictive validity remain unverified.

## 7. Questions for the independent reviewer

1. Is G-02 the right behaviour (a rejected ID stays rejected even after the plan becomes executable)?
2. Is keeping the dedup index out of `P1-STATE-V1` sound, given that history is always fully replayed on load?
3. Is read-only reflection after `AdvanceOneYear` an admissible observation, as distinct from the reflection M-1 forbids?
4. Are Tier-1 outputs plus the roll/p bracket check a sufficient basis for deciding C-2b later, or must the oracle wait for an instrumented copy?
5. Are the proposed caps (5,000 trees, 200 years) right for the intended experiments?
6. Should P1 add `fsync` and root ownership/mode checks beyond P0's qualified posture?

**Not done, by design:** no code, Unity, compiler, test, fixture, capture, source copy, build or machine lease; no edit to main, the Decision Log, the milestone, the live board or any mirror. **Dashboard / Drive sync: NOT VERIFIED.**
