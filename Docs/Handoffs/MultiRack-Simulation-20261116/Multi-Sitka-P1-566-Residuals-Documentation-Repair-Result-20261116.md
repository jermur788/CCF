# #575 — Multi-Sitka P1: documentation repair of the #566 residuals (F-1, F-2, F-3, m-1…m-12)

**RESULT: REPAIRED IN DRAFT, HANDED OFF FOR ONE FOCUSED NON-AUTHOR DELTA REVIEW.** All three MATERIAL residuals and all twelve MINOR corrections of the independent [#566 review](https://github.com/jermur788/CCF/blob/7bca1903227832d7097f41ad9ce8487e5d42b346/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-Annual-Growth-Technical-Proposals-Independent-Review-20261116.md) are applied as documentation edits to the four P1 DRAFT files. This is the author's own account. It is **not** an independent grade, and it is **not** approval of the P1 specification, of any technical proposal, of any cap, of a platform policy or of a P0 base.

**What this is not.** No P1 code, Unity, compile, build, runtime test, fixture capture or Windows build was done or is requested. Every figure below is labelled **OBS** (read from source, git or a counted example string at a stated pin), **EST** (arithmetic on stated assumptions; unmeasured) or **ASSUMED** (a rate or layout I chose to size an estimate). The specification stays **DRAFT / NOT APPROVED**; the nine decision-register rows are byte-unchanged; the **72 requirements stay unchecked**; **N01–N13 stay unrun**; C-2b and C-6 stay OPEN. Dashboard and Drive: **NOT VERIFIED**.

---

## 1. Identity, independence and claim

| Item | Record |
|---|---|
| Task | [#575](https://github.com/jermur788/CCF/issues/575); binding [Manager packet](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-566-manager-repair-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-566-MANAGER-DRAFT-QUALITY-VERDICT-AND-REPAIR-PACKET-20261116.md) |
| Worker | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code in T3 Code. **Not Claude Opus 5.5**, which the packet preferred, and **not the #562 author**: I have no retained #562 session context and worked from the pinned sources and the #566 report. Effort is not observable from inside the session |
| Registration | [RUNNING receipt](https://github.com/jermur788/CCF/issues/575#issuecomment-6911204759), posted before any source inspection or edit |
| Host / worktree | `jer-laptop`; `/media/jer/Files/CCF/Operations/CCF-575-SitkaP1DocsRepair-20261116` (Files volume, ext4), created after ownership checks |
| Branch / start HEAD | `docs/multi-sitka-p1-566-residuals-repair-20261116-v1` from exactly `81470c5955c781752148dae4d6240483c15e3ee7` (the reviewed #562 candidate; equal to its remote tip) |
| Live board | `origin/main` `6eb0730f` `active-tasks.md` read before work: no Sitka P1, #562, #566 or #575 entry; no other open issue targets the four spec files; no existing branch or worktree for #575 |
| Independence note | The packet prefers the #566 reviewer (Sonnet 5.5 High) for the delta review. I am also Sonnet 5.5, so **author and preferred reviewer share a model family**. Independence then rests on a different session, not on a different model. The Manager decides whether that is acceptable (§10) |
| Method | `git show` / `git diff` / `git rev-list` / `git ls-tree` plus plain file reads; Python scripts run with `python3 -I -B` for byte counts, arithmetic and the consistency audit; one throw-away scratch git repository in the session scratchpad to observe `git status` behaviour. **No** build, compile, test, Unity, Mono, lab or browser run |

## 2. Pins and observations

| Pin | Value | Check |
|---|---|---|
| Baseline | `81470c59` | Four DRAFT files plus one handoff; `Tools/MultiRackSitkaLab` is not in this commit's tree (OBS) |
| Game source | `Assets` tree `824a1394c69404c847a9841d5f531e398589730b` | Identical at `81470c59` and at `origin/main` `6eb0730f` (OBS). All line citations read from `81470c59` |
| P0 working reference | `861b81d3540429d241bba0732a610212626f6828` | Two commits after `2659db99` (OBS `git rev-list --count`). The only changed path under `Tools/MultiRackSitkaLab` is `README.md`; tree objects `c212f668dc2a585ff51a039c50107acd2b58b3a8` (`2659db99`) and `fa25ae6e39e715cc1d44a36d42607c309c7b5f7d` (`861b81d3`). Branches at `861b81d3`: `origin/fix/multi-sitka-p0-save-response-r3-20261116-v1`, `origin/docs/multi-sitka-p0-operator-readiness-20261116-v1`, `origin/docs/manager-multi-008-wsl-acceptance-readme-20261114-v1` |

## 3. Scope and preservation (mechanical)

| Check | Result |
|---|---|
| Writable paths | Exactly the four existing `specs/multirack-sitka-p1-adult-rules-comparator-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` (modified) and this file (added). Nothing else was written in the worktree; no `__pycache__` |
| Register rows | 9 rows matching `^\| \*\*C-` in each file; SHA-256 prefix of those lines `d6365b4fe851f298` in all four, **identical to the baseline** and to the #566 value |
| Requirement boxes | `requirements.md`: **72 unchecked, 0 checked**; none added or removed |
| Test labels | N01–N13 rows present; the plan still says "designed checks, not tests run"; no N-row added (§9 of plan maps repairs onto existing rows) |
| Approval language | Every file keeps "DRAFT / NOT APPROVED"; no sentence says the specification is approved |
| D-082 | Accepted choices untouched (original verified synthetic input, conditional stale-on-load, synthetic-not-forecast disclosure) |
| Links | Relative links and heading anchors across the five files resolve (local GitHub-slug check, §12; not a rendered-GitHub check) |

## 4. Five-file diff

`git diff --stat 81470c59` for the four modified files (the added file is this one):

```text
specs/.../checklists/requirements.md |  26 +-
specs/.../plan.md                    |  28 ++-
specs/.../spec.md                    | 274 ++++---
specs/.../tasks.md                   |  18 +-
4 files changed, 255 insertions(+), 91 deletions(-)
```

The exact commit, push, compare URL and GitHub raw-readback hashes are in the closing GitHub comment on #575, because they do not exist until after this file is committed.

## 5. Disposition: F-1, F-2, F-3

| ID | Repair | Where (amended anchors) | Status |
|---|---|---|---|
| **F-1** caps, replay cost, testable budget | (1) `MaxEvents` 200,000 and `MaxFileBytes` 64 MiB labelled **P0's own constants** (`Models.cs:27–28`), not derived for P1; `MaxTrees` 5,000, `MaxEcologicalYears` 200 and the 32 MiB trace file labelled new, unvalidated proposals; 7,800 stems/ha has no stated source, 200 years has no named experiment and is **not** called empirically required. (2) A reproducible byte-envelope bound: four 64-hex digests are 264 B before anything else; counted example envelopes are 383–629 B (OBS), so the byte cap binds first at about 105,000–173,000 events (EST) in every counted variant, while 264 B alone would not prove it; the quantitative event cap is **UNDECIDED**. (3) Replay cost now includes one full-state hash per event (P0 `LabSession.cs:18, 34, 131`) as term H2 beside the Hegyi pair term H1, with an EST table at ASSUMED rates; the #566 order-of-magnitude is marked **unmeasured**. (4) A **mandatory P1-03 synthetic load benchmark** (N ∈ {162, 2,000, 5,000} × event counts up to the byte cap, including the smallest-event adversarial file) gates every cap. (5) A **refuse/abort** strategy replaces a blocking synchronous load: static `ReplayBudgetExceeded` pre-check, cancellable replay with `ReplayAborted`, polling. Budget value left to the Manager. (6) Incremental/checkpoint/Merkle ideas listed only as **open options (a)–(d), not adopted**. (7) Preservation of rejection-before-write and atomicity kept | [spec §4.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#41-proposed-original-fixture-import-and-file-boundary-c-1c-1a), [§9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix) T-08/T-10, [§9.3](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#93-c-1-proposal-archive-layout-and-capacity-rewritten-by-575-f-1-and-m-9), [§9.8](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#98-still-blocked-open-or-unverified-after-562); [plan §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#9-proposed-bounded-qualification-design--not-a-booking-or-executable-packet) N02/N08, [§10](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#10-562-proposal-to-check-trace); [tasks §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md#9-contract-to-task-deliverables-and-negative-test-ownership) P1-03; [REQ-B09/E11](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md) | Repaired in DRAFT; caps and budget **OPEN** |
| **F-2** maths library and platform | (1) The text now says exactly that the growth source uses `Math.Exp`/`Math.Pow` (`SitkaGrowthModel.cs:41, 48, 74, 105`), that different runtimes and library builds **may** differ in the last bit (background knowledge, **not verified here**), and that it hits P1's **replay-on-load and hash checks**, not only the Unity capture. (2) A **platform fingerprint schema proposal** (OS, architecture, CPU feature class, runtime or Mono build, libc/libm identity and version, model/source/archive versions, trust-table digest, an OPTIONAL measured `mathProbeDigest`); any unreadable required field **fails closed**, a libm identity is never invented, no host or user identifier is recorded, and the record is stated to be self-reported and never a trust anchor. (3) **Distinct refusal classes** (`PlatformFingerprintMismatch`, `ReplayPlatformMismatch`, `ReplayMismatch`, `FingerprintMissingOrInvalid`, `UnsupportedVersion`, `PlatformUnverifiable`) with a non-destructive, byte-preserving, detached-load rule and user-facing wording intent; missing and different fingerprints are treated differently and the rule is spelled out. (4) Cross-host portability is an **OPEN owner choice**: options A (same platform only, draft default), B (attempt and classify, needs a fingerprint chain) and C (platform-independent arithmetic, needs C-2b and an approved package), each with cost and tests. (5) "A Linux Unity Editor capture is evidence for that Linux Editor only; it cannot prove Windows Player parity" added in §3, §9.5, §9.8 and REQ-C01. C-2b stays OPEN | [spec §3](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#3-source-faithful-biological-and-timing-contract--proposal-not-parity-verdict), [§9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix) T-16, [§9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe), [§9.8](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#98-still-blocked-open-or-unverified-after-562), [§9.9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#99-platform-fingerprint-save-portability-and-refusal-classes-f-2-575--proposed-not-accepted); [plan §5/§9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#9-proposed-bounded-qualification-design--not-a-booking-or-executable-packet) N03/N08; [REQ-C01/C13/E07](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md) | Repaired in DRAFT; portability **OPEN** |
| **F-3** P0 lineage and non-mutating proof | (1) The stale "no newer P0 head" text is corrected: `861b81d3` (README-only) is the Manager's **documentation working reference**; #567 has separate ownership; the implementation base is **re-pinned explicitly by the Manager after independent #567 disposition**, never floated, never `main`. (2) The commit-only `git diff --quiet` is replaced by a **four-layer proof** (L1 tree ids, L2 `git status --porcelain --ignored --untracked-files=all`, L3 ignored-file snapshot, L4 write observation), each row saying what it proves and what it does not, with a rule for **pre-existing ignored files** (record, never clean) and STOP conditions; committed bytes and observed writes are separated. (3) **Build-output isolation** stated as an outcome (a P1 restore/build/test/run writes nothing under `Tools/MultiRackSitkaLab`) with two candidate mechanisms for the build reviewer, none selected. (4) P1's **own** `global.json`, `Directory.Build.props` and `.gitignore` re-includes (root `.gitignore` lines 26/28 ignore `*.csproj`/`*.sln`). (5) P1 remains a sibling tool and does not mutate qualified P0 | [spec §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#9-562-technical-contract-proposals-c-1-c-2a-c-3-c-4-c-5--proposed-not-accepted) pins, [§9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix) T-01, [§9.2](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#92-c-1-proposal-trust-tables-lineage-file-and-host-boundary), [§9.8](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#98-still-blocked-open-or-unverified-after-562); [plan §2/§9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#9-proposed-bounded-qualification-design--not-a-booking-or-executable-packet) N13, [§10](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#10-562-proposal-to-check-trace); [tasks §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md#9-contract-to-task-deliverables-and-negative-test-ownership) P1-02; [REQ-B07](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md) | Repaired in DRAFT; base pin **OPEN** |

## 6. Disposition: m-1…m-12

| ID | Correction made | Where |
|---|---|---|
| **m-1** | Two revision checks named and separated: envelope `expectedForestRevision` mismatch is a **refusal**; **plan-base** `RevisionMismatch`/`EpochMismatch` (P0 `Engine.cs:68–71`) are **recorded audit-only rejections**. Golden vectors G-01…G-08 and their expected results are unchanged; check order step 3 split accordingly | [spec §9.4](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#94-c-1-proposal-command-ids-rejected-commands-and-retries-556-d-6) |
| **m-2** | P0's **runtime** does not read `MANIFEST.sha256`; P0's unit test does (`FileFormatTests.cs:24–32`). The trust conclusion is unchanged | [spec §4.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#41-proposed-original-fixture-import-and-file-boundary-c-1c-1a), [§9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix) T-02 |
| **m-3** | `timeLapseEnabled == false` added as a capture precondition (`:27`, advances in `Update` `:256–262`). The caller list is corrected: `CropTreeCompetition` is **not** a caller (its `:246` is a comment about `GetCurrentSuppression`); the real direct callers found by grep are `ForestPlayer.cs:1224`, `ForestTreeMarkingManager.cs:451`, `TreeInspectionView.cs:198`, plus in-controller `GetCurrentSuppression` (`:1830`), `GetCompetitionLabel` (`:1836`) and `StandDiagnostics` (`:2006, 2015`) | [spec §6](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#6-c-6-source-parity-oracle-phased-qualification-and-stop-gates), [§9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) |
| **m-4** | Citations: hazard loop `:1098–1108`, call `:1134`, `KillDensityVictims` `:1137–1158` (comment 1137, signature 1138, closing brace 1158 at `81470c59`; #566 gave 1137–1157) | [spec §9.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#91-decision-and-trade-off-matrix) T-12 |
| **m-5** | The roll year is the **post-increment** `ecologicalYear` (`:467`), and the seed is the controller's `simulationSeed`, not the capture's own seed | [spec §5.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#51-proposed-initialization-geometry-and-capture-manifest-c-2ac-3c-4c-5), [§6](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#6-c-6-source-parity-oracle-phased-qualification-and-stop-gates), [§9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) |
| **m-6** | The harness reads the controller's actual `rngModelVersion` and does not assume model 1: its serialized default is the legacy model 0 (`ForestEcologyController.cs:9`), new games select model 1, the models differ (`SimulationRandom.cs:15–17, 41–55`); both are tested | [spec §5.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#51-proposed-initialization-geometry-and-capture-manifest-c-2ac-3c-4c-5), [§6](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#6-c-6-source-parity-oracle-phased-qualification-and-stop-gates), [§9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) |
| **m-7** | Sterbenz range stated: `x − h` is exact in binary32 for `½ ≤ x/h ≤ 2`, so `h + (x − h) = x`. Ratio table (OBS, scratch arithmetic): 2.860 at ages 0 and 1, 1.825 at 2, 1.518 at 3, 1.288 at 5, 1.126 at 10, 1.029 at 28. Hazard confined to ages 0 and 1; all original fixture trees are age 28; the small-age vector is kept | [spec §9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) T-17, [§9.7](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#97-c-3-c-4-and-c-5-provenance-and-evidence-classes-t-19) Age row |
| **m-8** | Owner/mode/`fsync` promise limited to a **local POSIX filesystem** on a reviewed allow-list (candidate ext4 only); DrvFs, 9p, network mounts and unknown types refuse; checks use `fstat`/`fstatfs` on the **opened root handle**. `fsync` and root checks restated as reviewer-supported **proposed** P1 additions, not P0 properties | [spec §9.2](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#92-c-1-proposal-trust-tables-lineage-file-and-host-boundary), [§9.8](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#98-still-blocked-open-or-unverified-after-562) |
| **m-9** | One compact-size model: the §9.3 formula, with terms and bases shown. The §4.1 "≈20–40 B per tree-year" row and its figures are **withdrawn** and replaced; both locations were edited and re-read | [spec §4.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#41-proposed-original-fixture-import-and-file-boundary-c-1c-1a), [§9.3](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#93-c-1-proposal-archive-layout-and-capacity-rewritten-by-575-f-1-and-m-9) |
| **m-10** | R2 shares the Editor **binary and host**, not the running session or project; headings and the no-perturbation control wording changed accordingly | [spec §5.1](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#51-proposed-initialization-geometry-and-capture-manifest-c-2ac-3c-4c-5), [§9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) |
| **m-11** | Note 1 states that for the `MortalityApplied` row the observation is **application** order, not an evaluation of S or p; the hazard/boundary split is read from list order; no eighth status | [spec §9.5](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#95-c-2a-proposal-what-the-uninstrumented-capture-can-observe) |
| **m-12** | The client mints `commandId`; a deliberate new click gets a **new** ID, only a transport retry repeats one; G-02 states a reused recorded ID never executes; a client-level negative test added to N08 | [spec §9.4](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#94-c-1-proposal-command-ids-rejected-commands-and-retries-556-d-6); [plan §9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#9-proposed-bounded-qualification-design--not-a-booking-or-executable-packet) N08 |

## 7. The 19 T-items after the repair (author's disposition, not an independent grade)

| Item | #566 grade | After #575 | Change |
|---|---|---|---|
| T-01 lineage | PARTIAL | Repaired in DRAFT | F-3 (§5): working reference, layered proof, build isolation, own build files. Open: base re-pin after #567; mechanisms untested; L1–L4 not run |
| T-02 trust anchor | PASS | PASS + m-2 | Sentence corrected |
| T-03 derived catalog | PASS | PASS | No change |
| T-04 read path | PASS | PASS | No change |
| T-05 root, files, `fsync` | PASS | PASS + m-8 | Filesystem scope and opened-handle checks |
| T-06 host | PASS | PASS | No change |
| T-07 serialization | PASS | PASS | No change; the fingerprint record uses the same strict reader |
| T-08 archive and caps | PARTIAL | Repaired in DRAFT | F-1 and F-2 (§5), m-9. Open: caps, event cap, replay budget, benchmark |
| T-09 rejected IDs | PASS | PASS + m-1, m-12 | Two revision checks; ID-minting contract |
| T-10 dedup index | PASS | PASS | "up to 200,000" replaced by "up to the (UNDECIDED) event cap" |
| T-11 growth-driving CI | PASS | PASS + m-3 | Time-lapse precondition; caller list; reflection ruling recorded |
| T-12 victim order | PASS | PASS + m-4 | Citations |
| T-13 `LastAdultMortality` | PASS | PASS | No change |
| T-14 S/p blocked | PASS | PASS | No change |
| T-15 instrumented copy | PASS | PASS + m-10 | Editor-binary versus session wording |
| T-16 maths library | PARTIAL | Repaired in DRAFT | F-2 (§5): fingerprint saved with every save; refusal classes; Linux Editor scope. Open: portability |
| T-17 store arithmetic | PASS | PASS + m-7 | Narrow hazard range |
| T-18 datum statuses | PASS | PASS + m-5, m-6, m-11 | Roll arguments; application-order note |
| T-19 C-3/C-4/C-5 classes | PASS | PASS | Age row cross-reference to m-7 only |

Carry T-02…T-07, T-09…T-15 and T-17…T-19 as **working design proposals**, contingent on this delta review and on later complete-spec approval and verification. T-01, T-08 and save-side T-16 are **not adoptable** until the delta review accepts the repairs **and** the Manager takes the open decisions in §9.

## 8. Evaluation of T-01, T-08 and T-16 (author self-assessment)

- **T-01.** The reviewer's three gaps are each answered: the stale pin (corrected with observed trees and branches), the weak proof (four layers with stated limits), and the build traps (output isolation, own `global.json`/`Directory.Build.props`/`.gitignore`). What I could not do: show that any isolation mechanism works. The two candidates are untried, and `strace`/`inotifywait` availability is unverified. The base pin is still a Manager decision.
- **T-08.** The caps are no longer presented as derived. The event cap is honestly UNDECIDED, the replay cost has its dominant term, and a benchmark and a refuse/abort strategy are required. What I could not do: measure anything. All timings rest on ASSUMED rates. The 7,800 stems/ha and 200-year figures are unresolved because I could not find a stated source or experiment.
- **T-16.** The platform dependency is stated, a fingerprint and refusal classes are proposed, and cross-host portability is left to the owner with three costed options. What I could not do: verify any library behaviour, confirm which CPU flags matter, or confirm how each fingerprint field can be read on each platform. Those are written as unverified.

## 9. Owner/Manager decision queue (all OPEN; nothing here is decided by this repair)

The queue is in [spec §9.10](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#910-decision-queue-and-recorded-manager-working-rulings-575--not-product-or-spec-approval). In short: (1) final implementation P0 base after independent #567 disposition; (2) cross-platform save portability, default A (refuse, preserve file); (3) empirical replay and byte caps, the need for 200 years, event cap or replay-cost guard, replay-time budget; (4) trust-table values; (5) later C-2b numerical proof and C-6 machine booking; (6) remaining unpublished field and GROWFOR evidence; (7) `fsync` and root owner/mode acceptance; (8) which hash design, if any, replaces one full-state hash per event. The five bounded Manager rulings from the packet are recorded there as rulings, not as approvals.

## 10. Focused delta-review recommendation

**One review, non-author, not this session, scoped to F-1/F-2/F-3, m-1…m-12 and their neighbours. Do not repeat the 19-item source audit.** The packet prefers Sonnet 5.5 High; because the author is also Sonnet 5.5, the Manager should decide whether that is acceptable or whether a non-Sonnet, non-Opus non-author is preferable. Suggested checks, in order of risk:

1. **Arithmetic I introduced** ([spec §9.3](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#93-c-1-proposal-archive-layout-and-capacity-rewritten-by-575-f-1-and-m-9)): the counted envelope sizes (383–629 B), the reachable-event bound (about 105,000–173,000), the H2 table, and the compact totals 0.68 / 6.44 / 1.78 MB. Recompute from the stated field lists; judge whether ASSUMED rates and the 2 KiB header are labelled honestly.
2. **The claim that the byte cap binds first** under every counted variant, and that 264 B alone does not prove it.
3. **The withdrawal of the §4.1 compact figures** (m-9): confirm no other place still quotes ≈20–40 B per tree-year or the old 650 B annual figure.
4. **F-3 layering** ([spec §9.2](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#92-c-1-proposal-trust-tables-lineage-file-and-host-boundary)): are L1–L4 accurately described, is the `--untracked-files=all` observation right, and is the pre-existing-ignored-files rule safe.
5. **Fingerprint and refusal classes** ([spec §9.9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#99-platform-fingerprint-save-portability-and-refusal-classes-f-2-575--proposed-not-accepted)): do the classes partition the cases without overlap, is "unavailable fails closed" consistent with the optional `mathProbeDigest`, and are options A/B/C fair.
6. **m-3 caller list** and **m-4 citations**: re-read `ForestEcologyController.cs` lines and the `CropTreeCompetition` comment.
7. **m-7 Sterbenz statement** and the ratio table.
8. **Preservation**: register hash `d6365b4fe851f298` in all four, 72/0 boxes, N01–N13 labels, no approval language, five-file scope.
9. **Cross-file consistency** of the new text in [plan](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md), [tasks](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md) and [requirements](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md), including [§J](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md#j-562-technical-proposal-trace-not-requirements-no-boxes).

## 11. Negative-test updates (documentation only; none run)

Added to existing rows, no new N-row: **N02** (DrvFs/9p/network or unknown filesystem refuses start-up; path-based owner/mode check fails review; smallest-event adversarial file refuses with `ReplayBudgetExceeded` and an untouched file; aborted replay activates nothing; no cap accepted before the benchmark). **N03** (`timeLapseEnabled` true or unread; `rngModelVersion` assumed; fabricated libm identity; reflection before the step returns or in enumeration order). **N08** (platform difference reported as corruption or the reverse; refusal that rewrites, renames, deletes or partly applies; a reused recorded ID that executes; envelope and plan-base revision checks swapped; blocking synchronous replay) and the eight positive/negative platform tests in [spec §9.9](../../../specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#99-platform-fingerprint-save-portability-and-refusal-classes-f-2-575--proposed-not-accepted). **N13** (any trace under `Tools/MultiRackSitkaLab` seen by L2–L4; an L1-only proof). N01, N04–N07 and N09–N12 are unchanged.

## 12. Source arithmetic, assumptions and the consistency audit

| Quantity | Value | Class |
|---|---|---|
| 64 MiB ÷ 200,000 | 335.5 B per event | OBS arithmetic |
| Counted example envelopes | 542 / 590 / 629 B (descriptive keys), 383 B (terse floor), 416 B (system event) | OBS of counted example strings; key names undecided |
| Example tree records | 204 B (f32, 16-char ID), 228 B (f64), 290 B (f64, 64-char ID) | OBS of counted example strings |
| Example annual envelope with five-datum summary | 907 B (replaces #562's 650 B) | OBS of a counted example string |
| Reachable events at 5,000 trees | about 105,000 / 122,000 / 173,000 | EST (assumes 200 B/tree and a 2 KiB header) |
| Bytes hashed at 5,000 trees | 1 GB per 1,000 events | EST |
| Replay time at 0.1 / 0.5 / 1 GB/s | e.g. 105 GB → 1,050 / 210 / 105 s | EST on ASSUMED rates; unmeasured |
| Hegyi ordered pairs, 5,000 × 200 | 5×10⁹ | EST (carried from #562; trees die, so an upper bound) |
| Height ratio ρ at ages 0/1, 2, 3, 5, 10, 28 | 2.860, 1.825, 1.518, 1.288, 1.126, 1.029 | OBS scratch arithmetic from `SitkaGrowthModel.cs:48`; matches #566 |
| `git status` modes | plain `--ignored` collapses to `!! bin/`; `--untracked-files=all` lists files | OBS in a scratch repository, not in the P0 tree |

The scripts that produced these numbers lived in the session scratchpad, not in the repository. They count bytes of example JSON strings and print arithmetic; they are not project tests. The #566 estimate "minutes to tens of minutes" and the "tens of seconds" Hegyi figure remain **unmeasured** wherever they appear.

**Consistency audit (local; run with `python3 -I -B`):** register hash, box counts, N-labels and approval language as in §3; relative-link and heading-anchor check over the five files; stale-phrase greps for "No newer", "`2659db99` today", "≈20–40 B", the old 650 B figure and the commit-only proof found no remaining stale claim outside deliberate "superseded/withdrawn" sentences.

## 13. Known limitations and what was NOT verified

- Static reading only. No Unity, Mono, .NET, lab, browser, build or test run; no Windows evidence.
- That runtimes or C-library builds return different last bits for `exp`/`pow`, which CPU flags change library paths, how libm identity and version can be read on each platform, whether `strace`/`inotifywait` or the MSBuild isolation mechanisms work: all **unverified** and written as such.
- The 7,800 stems/ha density and the need for 200 years: no source or experiment found; left to the owner/Manager.
- `git status` behaviour was observed in a scratch repository only; the L1–L4 layers were **not run** against a real P0 tree.
- Link checking is local; no rendered-GitHub check beyond the closing readback comment. Dashboard and Drive mirrors: **NOT VERIFIED**.
- No file outside the five listed paths was edited. Docs/Project, the Decision Log, active-tasks, #565 ledger, P0, fixtures and `main` were not touched.

## 14. Standard handoff block

```text
IMPLEMENTATION HANDOFF (documentation only)
Task: CCF #575 Multi-Sitka P1 #566 residuals repair (F-1, F-2, F-3, m-1..m-12)
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5), Claude Code in T3 Code; effort not observable; NOT Opus 5.5, NOT the #562 author
Role: Documentation repair author
Branch: docs/multi-sitka-p1-566-residuals-repair-20261116-v1
Base: 81470c5955c781752148dae4d6240483c15e3ee7 (exact reviewed #562 candidate)
HEAD: recorded in the GitHub comment after this file is committed
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-566-Residuals-Documentation-Repair-Result-20261116.md
WHAT CHANGED: four DRAFT spec files edited; this one Markdown file added
SERIALIZED UNITY FILES TOUCHED: none
SIMULATION / SAVE IMPACT: none
MUST-NOT-CHANGE HASHES / ANCHORS: register rows d6365b4fe851f298 unchanged in all four; 72 unchecked boxes unchanged; Assets tree 824a1394c69404c847a9841d5f531e398589730b unchanged
VERIFICATION RUN: git scope diff, register-hash and checkbox counts, N-label presence, relative link and anchor check, source reading at the pins above, byte-count and arithmetic scripts with python3 -I -B, scratch-repo git status observation
VERIFICATION NOT RUN: Unity, compile, test, runtime, lab, browser, Mono, .NET, Windows, P0-tree L1-L4 proof, rendered-GitHub check beyond the closing readback
SPEC CONFORMANCE: NOT APPLICABLE (the specification is not approved)
KNOWN LIMITATIONS: static reading only; all timings are estimates on assumed rates; library and platform behaviour unverified; author and preferred reviewer share a model family
DECISIONS NEEDED: spec section 9.10 (eight items)
Ready for review: Yes, one focused non-author delta review
RECOMMENDED NEXT STEP: Manager checks this delta, commissions the focused review, then decides the open policies; no implementation, fixture or Unity time is released
Dashboard / Drive sync: NOT VERIFIED
```
