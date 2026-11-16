# #581 — Multi-Sitka P1: second and final bounded documentation correction (#578 N-1…N-10)

**RESULT: CORRECTED IN DRAFT, HANDED OFF FOR MANAGER DISPOSITION.** All ten #578 findings are addressed across the four existing P1 DRAFT files, with one added result Markdown. No third review loop is claimed or dispatched.

**What this is not.** Not P1 specification approval, not adoption of any cross-platform policy, cap, checkpoint, `fsync` contract or hash optimisation, not a project gate, and **not independent verification of itself** — see the independence limitation below. The four P1 files remain **DRAFT / NOT APPROVED**; the **72 requirement boxes stay unchecked**; **N01–N13 stay unrun**; **C-2b** and **C-6** stay OPEN; D-082 is untouched.

---

## 1. Identity, lane and independence limitation

| Item | Record |
|---|---|
| Task | [#581](https://github.com/jermur788/CCF/issues/581); parent programme [#565](https://github.com/jermur788/CCF/issues/565); binding [Manager packet](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-578-manager-final-repair-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-578-MANAGER-VERDICT-SECOND-FINAL-DRAFT-REPAIR-PACKET-20261116.md) |
| Worker | **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`), provider `opencode-go`, OpenCode harness in T3 Code. **Effort is not observable in this runtime and is not claimed.** `RUNNING` registered before inspection |
| **Independence limitation** | **I am the #578 reviewer.** This repair therefore implements my own findings, and the Manager must treat it as **author-side work, not independent confirmation**. Nothing here is a second opinion on N-1…N-10; it is the text that a later independent reader should check. The packet's preferred author was the #575 Sonnet 5.5 worker, which was not used |
| Host / worktree | `jer-laptop`, Linux 7.0.0-42 x86_64. New isolated docs-only worktree `/media/jer/Files/CCF/Operations/CCF-MultiP1-578FinalDocs-20261116` (Files, ext4) |
| Branch / base | `docs/multi-sitka-p1-578-final-docs-correction-20261116-v1` from the exact #575 candidate **`53f3cd254e6d9ebc8ae48cee32dfd38afdd308df`**. Branch was free; worktree absent before creation; clean at start |
| Live board | `origin/main` `314f1c4d8bcbac51a5471295152d2dd066464e28` read before work: no row, lock or writer for the four P1 spec files, no #581 owner, no overlap with the Scenario One #579/#580/#582 compact lane |
| Method | `git diff`/`git show`/`git rev-parse`, direct reads, and `python3` byte counting and arithmetic. **No** Unity, Editor, Mono, .NET, compile, project test, lab, browser, save/load, `strace`/`inotifywait`, MSBuild isolation run or Windows build |

---

## 2. Scope and output

| Check | Result |
|---|---|
| Tracked changes | exactly **4 modified** (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) + **1 added** (this file) |
| Production surface | zero `Assets/**`, `Tools/**`, P0, fixture, save, Unity, `Docs/Project`, decision-log, milestone or `main` change |
| Register rows | 9 per file; SHA-256 prefix of those lines **`d6365b4fe851f298`**, byte-identical to the base in all four |
| Requirement boxes | **72 unchecked, 0 checked**, none added or removed |
| N-rows | N01–N13 only; **no new N-row**; "designed checks, not tests run" retained |
| Approval language | all four files still DRAFT / NOT APPROVED; §9 headline unchanged |
| New section | `spec.md` **§9.11** (committed example strings) and **§9.9.1** (probe digest); both labelled illustrative/proposed |
| Prior #575/#578 text | retained; superseded figures are marked as superseded rather than silently replaced |

---

## 3. Resolution of N-1 … N-10

| ID | Severity in #578 | Correction made | Anchor |
|---|---|---|---|
| **N-1** | MATERIAL | Portability **B** is now restricted to the **diagnostic-mismatch case only** — a class-C (or class-B/C) difference where every class-A field still matches — and the text says plainly that a fingerprint chain records *where* the platform changed, **not the other platform's bits**, so a segment produced with different `exp`/`pow` bits cannot be re-derived. Cross-platform continuation across differing arithmetic is documented as a **deferred, separately scoped, costed checkpoint alternative** (canonical state checkpoint at each platform boundary, per-segment hash rules, new envelope/hash/cap arithmetic) that is **not** introduced into §9.3's no-checkpoint proposal | `spec.md` §9.9 options table; §9.3 unchanged on checkpoint semantics; §9.8 bullet; §9.10 decision 2 |
| **N-2** | MATERIAL | The fingerprint is split into **three classes**: **A** candidate bit-affecting identity (architecture, libm identity/version, runtime build, `cpu.featureClass`, probe digest if adopted), **B** provenance/compatibility (`model.version`, `source.pin`, `archive.version`, trust-table version/digest), **C** diagnostics (OS family/release, kernel, CPU model, routine runtime version). **No field's causal role is asserted**; class membership is called a hypothesis awaiting a cross-host comparison, and the trust-table digest is explicitly *data provenance, not a maths field*. Option **A**'s full-refusal cost is stated explicitly: routine runtime builds, OS releases, CPU flag changes and trust-table revisions can force refusal **even when `exp`/`pow` return identical bits**, and that is a **costly, unapproved draft choice, not a software-safe fact**. A narrower candidate gate **A′** is described but **not selected** | `spec.md` §9.9 field table, class paragraph, options A/A′/B, §9.8, §9.10 decision 2 |
| **N-3** | MATERIAL | Load replay now has an explicit **concurrency fence**: the job records session identity, a **monotonically increasing session generation** (which must advance on **history-only audit-only rejections**, not only biological changes), history tip, forest revision and save epoch at dispatch; it derives a detached candidate; **activation re-checks all five under one exclusive activation/commit gate** and installs only on a match. A command, audit-only rejection, save, unload or second load in between aborts with **`SessionChanged`** and leaves memory, accepted commands and save bytes untouched. **`LoadInProgress`** refuses a second concurrent load. Four negative controls are specified (mutating command, history-only rejection, simultaneous loads, no partial state/disk change) | `spec.md` §9.3 "Refuse or abort" items 1–6 and the negative-control paragraph; `plan.md` N08; `tasks.md` §4; `requirements.md` REQ-E11 |
| **N-4** | MINOR | A fixed **evaluation order** is written: strict parse → archive/format version `UnsupportedVersion` → fingerprint presence/structure `FingerprintMissingOrInvalid` → comparison `PlatformFingerprintMismatch` → replay `ReplayMismatch`/`ReplayPlatformMismatch` → host capability `PlatformUnverifiable`. A **legacy archive without a fingerprint is `UnsupportedVersion`, not a malformed current-schema record**, and a parse failure is never a fingerprint class. Test 4 is split into "absent/malformed in the current schema" versus "genuine legacy schema" | `spec.md` §9.9 evaluation order, refusal table, test 4; `requirements.md` REQ-E07 |
| **N-5** | MINOR | `mathProbeDigest` is declared **not adoptable yet** and gets a new **§9.9.1**: a fixed, versioned argument list (`mathProbeArgListVersion = 1`, itself **UNDECIDED** and not invented), a versioned digest domain tag, and **all-or-nothing adoption per archive version** — a required digest that is absent, malformed or differently versioned refuses with `FingerprintMissingOrInvalid`; a save whose archive version does not require it must omit it. Equality remains a bounded probe over the listed arguments only | `spec.md` §9.9.1, field table, tests 8–9; `requirements.md` REQ-C13 |
| **N-6** | MINOR | 383 B is relabelled **"one example, not a floor"** and every committed example is disclaimed as a non-bound. The **smallest legal event is UNDECIDED**. Both thresholds are given: **335.54432 B** generic (64 MiB ÷ 200,000) and **329.48408 B** working (65,896,816 ÷ 200,000 after the 1,210,000 B snapshot and the 2,048 B header). A committed **E6 at 321 B** demonstrates that `MaxEvents` binds first below the threshold; the text says outright that no claim of "all 200,000 unreachable" is supported. The 264-digest argument is retained with the corrected **50.35 MiB** | `spec.md` §9.3 envelope table, reachability table, thresholds, consequence 2; §9.11 E4/E6; `plan.md` N02; `requirements.md` REQ-B09 |
| **N-7** | MINOR | **One published tree-record field set** replaces the inconsistent triple: `id`, `x`, `y`, `status`, `dbh`, `height`, `crown`, `age`, `provenance` → **242 B** (f32/16), **282 B** (f64/16), **330 B** (f64/64). The deltas are self-checking (five floats × 8 B = +40; ID = +48). The annual (786 B) and death-child (102/150 B) terms come from the same committed set, and the compact totals are recomputed to **0.72 / 6.91 / 1.88 MB** with the arithmetic shown | `spec.md` §9.3 term table, totals, check line; §4.1 mirror row (0.69/0.70/0.72 and 6.88/6.89/6.91 MB) |
| **N-8** | MINOR | Units are defined once: **MB = 10⁶ B**, **GB = 10⁹ B**, **MiB = 2²⁰ B**, no KB. The mixed comparison is converted (**52,800,000 B = 50.35 MiB**), the totals are also given in MiB (0.69 / 6.59 / 1.79), and the §4.1 row says "MB is decimal 10⁶" | `spec.md` §9.3 "Units (N-8)", totals, §4.1 |
| **N-9** | MINOR | A new **L0 history** layer (`git log <P0 pin>..HEAD -- <tree>` empty) closes the touched-then-reverted case that L1's tree comparison cannot see. L2–L4 are re-scoped to the **named P0 build-input tree**, with an explicit rule that under build mechanism (a) (external `git archive` extraction) they must run against the extraction root, because running them on the in-repo subtree would pass vacuously. Handoffs must state which tree they used | `spec.md` §9.2 layers L0–L4, "Which tree the layers name", "Committed bytes versus observed writes"; `plan.md` N13 and §10; `tasks.md` P1-02; `requirements.md` REQ-B07 |
| **N-10** | NIT | New **§9.11** commits the exact example strings — E1, E2, E3, E4, E5, E6, A1, D1, D2, T1, T2, T3, W1 — with their byte counts, and states that they are **illustrative, not a schema, not a grammar, not a golden vector, not a requirement**, and that re-counting must be done by byte length rather than by re-serialising | `spec.md` §9.11 (intro, table, 13 fenced strings, re-check tie-in) |

`plan.md` N02/N08/N13, `tasks.md` P1-02/P1-03 and §4, and `requirements.md` REQ-B07/B09/C13/E07/E11 are aligned with the corrected `spec.md`. No N-row and no requirement box was added or checked.

---

## 4. Arithmetic recomputed and committed

Everything below is arithmetic on committed strings, not a measurement.

| Quantity | Value | How |
|---|---|---|
| 64 MiB | 67,108,864 B | constant |
| Generic event threshold | **335.54432 B/event** | 67,108,864 ÷ 200,000 |
| Working event threshold | **329.48408 B/event** | (67,108,864 − 1,210,000 − 2,048) ÷ 200,000 |
| Available event bytes | 65,896,816 B | 67,108,864 − 1,210,000 − 2,048 |
| Reachability at `e_min` 564 / 477 / 429 / 428 / 333 / 321 | ≈ 116,800 / 138,100 / 153,600 / 154,000 / 197,900 / **205,300** | 65,896,816 ÷ `e_min` |
| Tree records | 242 / 282 / 330 B | T1, T2, T3 |
| Compact totals (D = N₀, W = 0) | 719,440 / 6,911,440 / 1,877,200 B = **0.72 / 6.91 / 1.88 MB** | 242·N₀ + 786·Y + 102·N₀ |
| Same totals in MiB | 0.69 / 6.59 / 1.79 | ÷ 1,048,576 |
| H2 state size S (5,000 trees) | 1,210,000 B | 5,000 × 242 |
| H2 bytes at E = 200,000 | ≈ 242 GB | 200,000 × 1,210,000 |
| H1 pair checks | ≈ 5×10⁹ | 5,000² × 200, upper bound |
| ρ at ages 0/1, 2, 3, 5, 10, 28 | 2.860, 1.825, 1.518, 1.288, 1.126, 1.029 | unchanged from #575; not re-derived here |

Application rates (0.1 / 0.5 / 1 GB/s) remain **assumptions**, every timing remains an **estimate**, and nothing was measured.

**Self-check performed on this text:** the 13 fenced strings in §9.11 were re-extracted from the finished Markdown, byte-length-counted, and parsed as JSON; the counts match both the §9.11 table and every derived figure in §9.3 (13/13). That is a text-consistency check, not a schema or runtime test.

---

## 5. Preservation and authority readback

| Requirement | Result |
|---|---|
| Nine decision-register rows unchanged | **Yes** — `d6365b4fe851f298` in all four files, versus the base |
| 72 unchecked / 0 checked | **Yes**; none added or removed |
| N01–N13 unrun, no new N-row | **Yes**; "designed checks, not tests run" retained |
| Four DRAFT / NOT APPROVED headers | **Yes**; no approval sentence added |
| C-2b and C-6 OPEN | **Yes**; no capture, lease or machine request |
| D-082 | **Untouched**; original-only synthetic fork, conditional stale-on-load and the synthetic-not-forecast disclosure unchanged |
| No policy selected | **Yes** — portability A/A′/B/C undecided; checkpoint deferred; event cap, replay budget, caps, trust values, `fsync`, hash optimisation and P0 base all still open |
| Only 4 M + 1 A | **Yes** |
| Dashboard / Drive | **NOT VERIFIED** (not checked; canonical Git remains the authority) |

---

## 6. Honest limitations

- **I am not an independent verifier of this repair.** It is the #578 reviewer's own text.
- Every byte count, total, threshold and timing is **arithmetic on illustrative strings or assumed rates**, unmeasured. The strings are examples, not a schema; a real canonical grammar would change every figure.
- No cross-platform comparison exists, so which fingerprint field actually changes a numerical result remains **unverified**; class membership is a hypothesis.
- The session fence, `SessionChanged`, `LoadInProgress`, the checkpoint alternative and the probe digest are **proposed names and mechanisms with no implementation and no test run**.
- L0–L4, the MSBuild isolation mechanisms, `strace`/`inotifywait` availability and any real P0 tree check were **not** executed.
- No Unity, Mono, .NET, compile, project test, lab, browser, save/load or Windows evidence exists for anything in this document.
- The 7,800 stems/ha and 200-year figures still have no stated source or named experiment.

---

## 7. Standard handoff block

```text
IMPLEMENTATION HANDOFF (documentation only)
Task: CCF #581 Multi-Sitka P1 second/final #578 N-1..N-10 DRAFT correction
Worker: DeepSeek V4.1 Flash (deepseek-v4.1-flash), provider opencode-go, OpenCode in T3 Code; effort not observable, not claimed
Role: Documentation correction author — ALSO the #578 reviewer, so not independent of these findings
Branch: docs/multi-sitka-p1-578-final-docs-correction-20261116-v1
Base: 53f3cd254e6d9ebc8ae48cee32dfd38afdd308df (exact #575 candidate)
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-578-Final-Bounded-Documentation-Correction-Result-20261116.md
WHAT CHANGED: four DRAFT spec files edited; this one Markdown added
SERIALIZED UNITY FILES TOUCHED: none
SIMULATION / SAVE IMPACT: none
MUST-NOT-CHANGE HASHES / ANCHORS: register rows d6365b4fe851f298 unchanged in all four; 72 unchecked boxes unchanged; Assets tree 824a1394c69404c847a9841d5f531e398589730b untouched
VERIFICATION RUN: scope diff, register hash, box counts, N-row check, stale-phrase sweep, fence/heading structure check, byte re-count and JSON parse of the 13 committed example strings, arithmetic recomputation
VERIFICATION NOT RUN: Unity, compile, tests, lab, browser, Mono/.NET, save/load, real P0 L0-L4, MSBuild isolation, strace/inotifywait, Windows, rendered-GitHub check beyond readback
SPEC CONFORMANCE: NOT APPLICABLE (specification not approved)
KNOWN LIMITATIONS: author is the #578 reviewer; all figures are arithmetic on illustrative strings or assumed rates; no cross-platform or numerical evidence
DECISIONS NEEDED: portability A/A'/B/C and any checkpoint funding; caps, event cap, replay budget; trust values; fsync contract; hash design; P0 base after #567; C-2b and C-6
Ready for review: Yes, Manager disposition of this exact diff
RECOMMENDED NEXT STEP: Manager inspects the diff and accepts or stops; no implementation, Unity or Windows work is released
Dashboard / Drive sync: NOT VERIFIED
```
