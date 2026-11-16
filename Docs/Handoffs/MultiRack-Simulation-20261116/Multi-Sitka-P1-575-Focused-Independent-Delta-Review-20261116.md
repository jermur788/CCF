# #578 — Focused independent delta review of the #575 Sitka P1 DRAFT repair (F-1, F-2, F-3, m-1…m-12)

**VERDICT: APPROVED AS DRAFT QUALITY — for the #575 delta only — with 3 MATERIAL and 6 MINOR findings and 12/12 minor corrections correctly applied.**

**What this verdict is.** An independent, non-author documentation review of the changes #575 made to the four P1 DRAFT files. It reproduces or falsifies the **new** arithmetic, source citations and preservation claims in that delta. Failures and partials are named with exact anchors and minimal corrections.

**What this verdict is not.** It is **not** approval of the P1 specification, of any technical proposal as a runtime contract, of any numeric cap, of a cross-platform save policy, of `fsync`, of the original-fixture trust-table values, or of a permanent P0 implementation base. It is **not** Unity, compile, runtime, lab or Windows evidence. The four files remain **DRAFT / NOT APPROVED**; the **72 requirement boxes stay unchecked**; **N01–N13 stay unrun**; **C-2b** and **C-6** stay OPEN. I changed exactly one file: this report.

| Item | Record |
|---|---|
| Task | [#578](https://github.com/jermur788/CCF/issues/578); binding [Manager packet](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-575-manager-disposition-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-575-MANAGER-REVIEW-CANDIDATE-VERDICT-AND-FOCUSED-DELTA-PACKET-20261116.md); parent [#565](https://github.com/jermur788/CCF/issues/565) |
| Reviewer | **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`), provider `opencode-go`, OpenCode harness in T3 Code. The dispatch names *DeepSeek High*; **this runtime exposes no effort selector and reports the `flash` variant, so "High" is not claimed or verified** — see the identity disclosure in the #578 [RUNNING comment](https://github.com/jermur788/CCF/issues/578#issuecomment-6911780832) |
| Independence | I authored none of #562 (Opus 5.5), #566/#575 (Sonnet 5.5) or the #552 Codex lineage, and had no prior context on this specification, the P0 lab or #567. Different model family from all three |
| Host / worktree | `jer-laptop`, Linux 7.0.0-42 x86_64. New isolated docs-only worktree `/media/jer/Files/CCF/Operations/CCF-578-SitkaP1DeltaReview-20261116` (Files, ext4, 80 G free). No Unity, Editor, lane, lease or Windows |
| Branch / start HEAD | `review/multi-sitka-p1-575-residuals-delta-20261116-v1` at **`53f3cd254e6d9ebc8ae48cee32dfd38afdd308df`** — identical to the exact #575 candidate and to the precreated remote branch tip; clean at start; branch held only by this worktree |
| Live board | `origin/main` `a7b01d1b373cfb3d5ccd9afdff5bfe9a3050a594` `Docs/Project/coordination/active-tasks.md` read before inspection: no #578 row, no Sitka P1 lock, no competing writer on the four spec files |
| Method | `git show` / `git diff` / `git rev-list` / `git ls-tree` / `git rev-parse`, plain file reads, and `python3` byte-count and arithmetic reconstructions in `/tmp/opencode` (scratch, not in the repository). **No** build, compile, test, Unity, Mono, .NET, lab, browser or Windows run |

Line anchors below are `spec.md:N` at `53f3cd25`; `Assets/ForestPrototype/<file>:N` is read from this worktree (its `Assets` tree is `824a1394c69404c847a9841d5f531e398589730b`, identical at `81470c59`, `6eb0730f` and review-time `origin/main` — re-verified); `P0:<file>:N` is `Tools/MultiRackSitkaLab/src/...` at `861b81d3`.

---

## 1. Diff scope and GitHub readback

| Check | Result |
|---|---|
| `git diff --name-status 81470c59 53f3cd25` | exactly **4 M** (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) + **1 A** (the #575 result Markdown). No other path |
| Diffstat | 441 insertions / 91 deletions over 5 files; of which 274/– in `spec.md`, 186 lines in the added result |
| Author's claim (result §4: 255 insertions, 91 deletions across the four spec files) | **Confirmed** (441 − 186 = 255) |
| Production surface | 0 files outside `specs/multirack-sitka-p1-adult-rules-comparator-v1/` and the one handoff. No `Assets/**`, `Tools/**`, P0, fixtures, `Docs/Project`, decision log, milestone, board or main change in the delta |
| Registers | 9 `^\| \*\*C-` rows in each file; SHA-256 prefix of those lines **`d6365b4fe851f298`** in all four, **identical to `81470c59`** and to the #566/#575 value |
| Requirement boxes | **72 unchecked, 0 checked**, none added or removed |
| N-rows | N01–N13 present, **no N14**; `plan.md:199` still says "designed checks, not tests run" |
| Approval language | Every file retains "DRAFT / NOT APPROVED"; no new approval sentence; §9 headline still "PROPOSED, NOT ACCEPTED" (`:351`) |
| Section numbering | §9.1–§9.10 contiguous; **§9.9 and §9.10 are new and unique**; all cross-references to them resolve (`spec.md:468, 537, 635, 689, 751, 760`; `plan.md:120, 187, 192, 214`; `tasks.md:87, 142, 144`) |
| Stale-phrase sweep | No remaining "No newer P0 head", no "`2659db99` today", no live "≈20–40 B per tree-year" or 650 B annual claim outside explicit withdrawal sentences (`:132`, `:476`, `:481`) |
| Authority integrity | The five "Manager working rulings" at `:748–753` are **faithfully transcribed** from the #575 packet §1 (reflection, `861b81d3` documentation reference, portability, capacity, `fsync`). No ruling is invented and none is upgraded to approval. The new §9.10 decision queue matches the packet's outstanding list |

---

## 2. F-1 — event envelope, storage formula, replay and abort

**Grade: PASS as a DRAFT proposal, with two arithmetic corrections (N-6, N-7) and one unit correction (N-8).**

### 2.1 What holds (recomputed)

| Claim | Where | My independent result |
|---|---|---|
| Four 64-hex digests = 4 × 66 = **264 B** once quoted | `:505` | **Correct.** This also silently fixes #566's "256 bytes", which omitted the quotes |
| `64 MiB ÷ 200,000 = 335.5 B` | `:515` | **Correct**: 67,108,864 / 200,000 = 335.544 |
| `200,000 × 264 B = 52.8 MB` is under 64 MiB | `:515` | **Correct**: 52,800,000 B = 50.35 MiB |
| Reachable **105,000 / 122,000 / 173,000** | `:515` | **Reproduced**: (67,108,864 − 1,000,000 − 2,048) = 66,106,816; ÷629 = 105,099; ÷542 = 121,968; ÷383 = 172,603 |
| Compact totals **0.68 / 6.44 / 1.78 MB** | `:490` | **Reproduced exactly** from 200·N₀ + 900·Y + 120·N₀: 676,000 / 6,436,000 / 1,780,000 B |
| §4.1 mirror **0.64 / 0.65 / 0.68 MB** and **6.40 / 6.41 / 6.44 MB** | `:130` | **Reproduced exactly**; one model, m-9 correctly applied in both locations |
| Full archive **64 / 640 / 800 MB** | `:489` | **Reproduced**: 800·N·Y |
| H1: N² × Y ≈ 5×10⁹ at 5,000 × 200 | `:521` | **Correct** as an upper bound; source is an all-pairs loop (`ForestEcologyController.cs:964–996`, `j` ascending, sum inline) |
| H2 table: 1/10/105/173/200 GB and the 0.1/0.5/1 GB/s seconds | `:527–533` | **Every cell reproduced**, including the 0.4× scaling note for 2,000 trees (`:535`) |
| Hegyi line-citation drift and the "sum inline" hash comment | `:550` | **Correct** (`:990–993` records `7E39B70A14959FAD → DDBBCBDEFAA4E644`) |
| P0 hashes the whole snapshot per event on append and on replay | `:520` | **Correct** (`LabSession.cs:131`; `Replayer.Replay` hashes at `:18` and `:34`; `PrevStateHash` is a comparison at `:27`) |

The dominant-term correction #566 demanded is genuinely made: H2 now exists, is labelled an estimate, and its assumed rates are labelled assumptions. The cap table (`:495–501`) correctly separates P0's own constants from new proposals and marks the quantitative event cap **UNDECIDED**. The refuse/abort strategy and the mandatory benchmark (`:537–544`) are the right shape.

### 2.2 N-6 (MINOR) — 383 B is not a "practical floor", and the byte-cap conclusion has a 2.5-byte margin

`:512` calls the terse 383 B envelope the "**practical floor**"; `:515` then uses it as `e_min` and `:533` calls 200,000 events "unreachable by bytes above".

My independent reconstruction of a terse canonical envelope with the fields the spec itself requires — one-letter keys, 16-character `commandId`, four 64-hex digests, sequence, year, revision, empty payload — is **333 B**. Dropping year and revision gives **321 B**; adding a source-identity field gives ~339 B. So 383 B is one illustrative variant ~50 B above a legal-looking floor, not a floor.

Consequence, computed exactly: the break-even is **66,106,816 ÷ 200,000 = 330.5 B**.

| `e_min` | Reachable events | Byte cap binds first? |
|---|---|---|
| 383 (stated "floor") | 172,603 | yes |
| 333 (my terse count) | 198,518 | yes — by 0.7% |
| 330.5 | 200,000 | exactly at the boundary |
| 321 (terse, no year/revision) | 205,940 | **no** |

Two further points. First, "empty payload" with a non-null `commandId` may not be a legal client event at all: `§9.4` defines `commandDigest` over "kind, arguments, expected revision", so a client command carries a payload. If the smallest *legal* event is instead the 416 B system event (`:513`), the true maximum is **158,910** — between the stated 122,000 and 173,000, and the 173,000 row is then a material over-statement of capacity. Second, because the conclusion's margin at 333 B is 0.7% and inverts at 321 B, the sentence "the byte cap binds before 200,000 events" is true **only for the counted variants**, exactly as hedged, but the label "practical floor" invites the stronger reading.

**Minimal correction:** replace "practical floor" with "one counted terse variant"; state the break-even numerically (`e_min > 330.5 B`); say that below it `MaxEvents` binds first; and state explicitly whether an empty payload is legal on an event with a non-null `commandId` — if it is not, 416 B is the better candidate for the smallest legal event and the 173,000 row must be dropped. Do **not** invent a new bound — `UNDECIDED` remains correct.

### 2.3 N-7 (MINOR) — the 204 / 228 / 290 B tree-record triple is not one layout

`:480` states a single record of `id`, `x`, `y`, `status`, `dbh`, `height`, `crown`, `age`, `provenance` with float wrappers, "counted at 204 B (f32, 16-char ID), 228 B (f64, 16-char ID) and 290 B (f64, 64-char ID)".

My reconstruction of exactly that field set (five float fields: x, y, dbh, height, crown):

| Variant | My count |
|---|---|
| f32, 16-char ID | **233 B** |
| f64, 16-char ID | **273 B** |
| f64, 64-char ID | **321 B** |

(The wrapper `{"k":"f32","b":"8hex"}` is 26 B, `{"k":"f64","b":"16hex"}` is 34 B; a 6-float variant gives 242 / 282 / 330.)

The stated triple cannot come from one field set. `204 → 228` is **+24**, which is exactly three float fields widening by 8 B — not five. `228 → 290` is **+62**, whereas a single ID occurrence growing 16 → 64 chars adds exactly **48** (that delta is confirmed independently in the envelope table, where 542 → 590 is precisely 48). So either the 64-char case carries 14 B of extra content, or the three numbers come from three different records.

Consequence: the `200·N₀` term is optimistic against the draft's own field list. Substituting 233 B gives 0.74 / 7.10 / 1.95 MB instead of 0.68 / 6.44 / 1.78 MB (9–11% low), and S ≈ 1.17 MB instead of 1.0 MB, which moves the reachable counts by under 1% (105,099 → 104,807 at `e_min` 629). **No decision changes**: 7.1 MB is still far below 64 MiB and the replay conclusions are insensitive.

Worth recording as an asset, not an error: the actual P0 fixture tree IDs are **5 characters** (`sitka-flat-perf-2000-v1`), so 200–290 B is conservative on ID length and optimistic on field count — the two roughly cancel.

**Minimal correction:** re-derive the three counts from one stated field set, or relabel 204/228/290 as three separately counted examples and keep `200·N₀` explicitly as a lower bound.

### 2.4 N-8 (MINOR) — MB and MiB are used interchangeably

`:485–491` prints "MB" for what is decimal 10⁶ (0.676 MB from 676,000 B), while the cap at `:498` is "64 MiB" and `:515` compares "52.8 MB" to "64 MiB". Every comparison happens to hold (52.8 MB = 50.35 MiB; 6.44 MB = 6.14 MiB), but the units are never defined and one sentence compares across them. **Minimal correction:** one clause — "MB means 10⁶ bytes; the cap and per-event figures are MiB (2²⁰)" — and convert the one mixed comparison.

### 2.5 N-3 (MATERIAL) — the detached replay has no session-quiescence rule

This is new in the delta (`:539–542`) and is the one place where the async proposal is weaker than the synchronous design it replaces. `:541` says replay "runs detached from the live session as a cancellable job", returns `ReplayAborted`, and "nothing is activated and nothing is written". `:542` says the client polls a job status. Nothing says what the **live session** may do while the job runs.

P0's load is synchronous (`LabSession.FromFile`, `LabSession.cs:76–89`), so this window does not exist today. With a detached job it does: a command can commit against the live session while the load is replaying, and the successful load then replaces the session, discarding that committed command from memory — and if the user saved in between, from disk as well. That is a lost update, not a refused request.

**Minimal correction:** state that an in-flight replay job holds the session read-only (or pins a generation), and that activation is refused with `ReplayAborted`/`SessionChanged` if the live history tip or forest revision changed since the job started. Add one negative test to §9.9/§9.3: *mutating command during an in-flight load → job aborts, nothing applied, nothing written*. This is a technical, not an owner, decision.

---

## 3. F-2 — platform fingerprint, portability and refusal classes

**Grade: PARTIAL.** The repair does what #566 asked (saved fingerprint, distinct non-destructive refusal, Linux-Editor-only statement, portability left OPEN), and the refusal table is a real improvement. But two of the three portability options are mis-specified in ways that would mislead the owner who has to choose, and the owner/Manager decision boundary must not be crossed to fix them.

Verified source/claim checks: `Math.Exp`/`Math.Pow` are used at `SitkaGrowthModel.cs:41, 48, 74, 105` (Exp at 41 and 48; Pow at 41, 48, 74, 105) **exactly as cited**; `ReplayMismatch` maps to P0's own class (`LabSession.cs:81`) **exactly**; "P1 targets Linux x64 and Ubuntu on WSL2 (§4)" matches `spec.md:452`; the Windows x86-64/Mono Player claim is correctly pinned to `Docs/Project/unity-project-overview.md:328` at `81470c59`, which is the build-tooling line describing a Windows x86-64/Mono build. The framing that replay-on-load makes this a **save-validity** issue, not just a capture issue, is the substantive F-2 fix and it is correct.

### 3.1 N-2 (MATERIAL) — option A's compared-field set refuses loads for reasons that cannot change bits, and the stated cost understates it

`:701` compares `os.family` **and** `os.release`; `:703` compares the CPU **flag list**; `:704` compares `runtime.version` and `runtime.build`; `:707` compares the **trust-table version and digest**. Option A (`:730`) refuses on **any** compared-field difference.

Consequences the cost column does not state:
- A .NET servicing patch (`runtime.version`) or a distro point release (`os.release`) refuses every existing save even when `Math.Exp`/`Math.Pow` are bit-identical. `:730` says only that "a routine distribution upgrade that changes the C library" does this — much narrower than the actual rule.
- A microcode/BIOS/VM change that adds a CPU flag refuses, although the draft itself admits the relevant flag set is "**unverified**" (`:703`).
- A reviewed trust-table update (§9.2 requires re-verification against the current table) invalidates every save written under the previous table. That may be intended, but it is another compare that cannot be a maths change.

This is exactly the kind of decision-table flaw the packet asked for, and it matters because **A is the draft default**.

**Minimal correction (owner boundary respected):** split the table into (i) fields that can change the arithmetic and therefore gate replay — architecture, libm identity+version, runtime build, feature class — and (ii) diagnostic fields recorded but not compared — `os.release`, `runtime.version`, kernel, CPU model. Alternatively, if a field differs, gate on `mathProbeDigest` when adopted. The choice of how strict A should be remains the owner's; the correction is to stop presenting a cost lower than the rule.

### 3.2 N-1 (MATERIAL) — option B is not implementable as described

`:731` offers B: on a fingerprint difference, "still replay; success is accepted, failure is `ReplayPlatformMismatch`", needing "a **fingerprint chain** (the platform of each segment of history), because a file continued on platform B may no longer replay on A".

A fingerprint chain records **where** the platform changed; it does not supply A's bits. P1 replays from genesis, so on platform B the first event of A's segment cannot be re-derived if A's `exp`/`pow` differ — its `prevStateHash` will not reproduce, and every later hash fails. B's "success" branch is therefore reachable only when the compared fields differ but the arithmetic is identical, i.e. precisely the cosmetic cases in N-2, which A refuses unnecessarily. As written, B cannot deliver "saves survive harmless upgrades" **and** hash verification.

Making B real is a format change, not a fingerprint-record change: a canonical **state checkpoint at each platform boundary**, replay restarting from the checkpoint, hashes compared within segments, and an explicit rule for the cross-segment history digest — which also changes §9.3's size model, its replay cost, and the "no checkpoint semantics" statement at `:465`.

**Minimal correction:** either (i) restrict B's description to "cosmetic field differences only, with the arithmetic-defining fields still gating", which is the honest cheap version; or (ii) state that B requires a platform-boundary checkpoint in the archive and cost it in §9.3. Do not adopt B on the strength of the current cost column. The decision of whether to fund (ii) is owner/Manager, not a documentation fix — flag it OPEN with the two alternatives.

### 3.3 N-4 (MINOR) — refusal-class precedence is undefined

`:720` makes `FingerprintMissingOrInvalid` cover "the record is absent", and `:721` gives `UnsupportedVersion` for an unknown format. A save written by an older P1 archive that predates the fingerprint record **has no record** — it would be reported as `FingerprintMissingOrInvalid` rather than `UnsupportedVersion`, unless the archive version is checked first, which the draft never says. Separately, a file that fails the strict reader never reaches the fingerprint at all, so a wholly malformed file must not be reported in a fingerprint class; `:720`'s phrase "a **structural** refusal by the strict reader" blurs that.

**Minimal correction:** add one sentence fixing the order — strict parse (reader's own format refusal) → archive/format version (`UnsupportedVersion`) → fingerprint presence/structure (`FingerprintMissingOrInvalid`) → fingerprint comparison (`PlatformFingerprintMismatch`) → replay (`ReplayMismatch` / `ReplayPlatformMismatch`). Split test 4 (`:738`) into "absent in an otherwise valid current-version file" and "absent because the format predates it".

### 3.4 N-5 (MINOR) — `mathProbeDigest` optionality is under-specified

`:708` marks the digest OPTIONAL and "computed at start-up; cannot be unavailable", and `:711` says equality proves nothing about unprobed arguments (honest). Undefined: (a) the "fixed argument list the spec defines" is not defined, so the field is not yet adoptable at all; (b) a save written **without** the digest loaded where the platform **has** adopted it — compare, ignore, or refuse?; (c) a save written **with** it loaded where archive policy no longer requires it. **Minimal correction:** state that adoption is all-or-nothing per archive version, that a missing digest under an archive version that requires it is `FingerprintMissingOrInvalid`, and that the argument list must be fixed in the spec before the field is used.

*Credit where due:* no host, user or path identifier is recorded; the record is explicitly self-reported and never a trust anchor; the provider is injectable so the tests are runnable on one machine; every refusal is non-destructive with no partial apply, no repair, no rewrite, no rename and no temp file. That is a materially better answer than #566's minimum correction.

---

## 4. F-3 — P0 lineage and the non-mutating proof

**Grade: PASS as a DRAFT proposal, with one scope gap (N-9).** Every git and source observation in the new text reproduces exactly.

| Claim | Where | My check |
|---|---|---|
| `861b81d3` is two commits after `2659db99` | `:361` | `git rev-list --count 2659db99..861b81d3` = **2** |
| Only `README.md` changed under `Tools/MultiRackSitkaLab` | `:361` | `git diff --stat` = `README.md \| 35 ++++---`, one file, 28/7 |
| Tree ids `c212f668…` / `fa25ae6e…` | `:361`, `:404` | **Exact match** (`git rev-parse <pin>:Tools/MultiRackSitkaLab`) |
| Three remote branches carry `861b81d3` | `:361` | Exactly the three named, and no others |
| `Assets` tree `824a1394…` identical at `81470c59` and `origin/main` (`6eb0730f`) | `:361` | **Exact**; `6eb0730f` is a commit on `main` whose `Assets` tree is `824a1394…`; still identical at review-time `main` `a7b01d1b` |
| P0 has no `fsync` | `:188`, `:400` | `git grep -i fsync\|Flush(true)` over `861b81d3 -- Tools/MultiRackSitkaLab` = **none** |
| P0's own `global.json` pins SDK `8.0.133`, `rollForward: latestPatch`; `Directory.Build.props` sets net8.0, nullable, warnings-as-errors, deterministic, invariant globalization | `:413` | **Exact** |
| Root `.gitignore` ignores `*.csproj` and `*.sln` at lines 26 and 28 | `:413` | **Exact** |
| P0 `.gitignore` re-includes `!*.sln`, `!*.csproj` and ignores `.lab-data/`, `bin/`, `obj/`, `TestResults/`, `evidence/*` | `:413` | **Exact** |
| `InternalsVisibleTo` only to P0's own tests | §9.2 | **Exact**: one entry, `SitkaLab.Core.Tests` |
| L2's `git status` behaviour claim | `:405` | **Confirmed empirically** (git 2.43.0, scratch repo): plain `--ignored` collapses to `!! sub/`; adding `--untracked-files=all` lists each ignored file |

The layered structure itself is the right answer to #566 F-3: each row states what it proves and what it does **not**, committed bytes are separated from observed writes, the pre-existing-ignored-files rule is "record, never clean" with explicit STOP conditions, and the two build-isolation mechanisms are left unselected for the build reviewer. The stale pin is corrected, the base is explicitly a Manager re-pin after #567, and P1's own build files are stated. The one nuance I would add is that L1's "does not prove" column should mention that it also does not detect a commit that touched P0 and was later reverted — which is why the history layer below is cheap and worth having.

### 4.1 N-9 (MINOR) — two gaps in the proof's coverage

1. **Scope under mechanism (a).** `:412` offers building P0 Core "from a `git archive` extraction **outside the repository**". If that mechanism is chosen, L2/L3/L4 as written still point at `Tools/MultiRackSitkaLab` inside the P1 worktree, which the build no longer touches — so they would pass vacuously while proving nothing about the tree the build actually reads. The recorded binary SHA-256 covers input identity, but the layer text must say so. **Correction:** "L2–L4 apply to the P0 tree the build actually reads: the in-repo subtree, or the external extraction root under mechanism (a)."
2. **No history layer.** L1 compares two trees; it cannot show that no commit on the P1 branch ever modified P0. The cheapest possible layer closes it: `git log --oneline <P0 pin>..HEAD -- Tools/MultiRackSitkaLab` must be empty. **Correction:** add it as L0 or fold it into L1's command line.

Neither gap affects the state-safety conclusion; both affect whether a handoff can honestly claim "P0 was never touched".

---

## 5. m-1 … m-12 — the twelve minor corrections

All twelve are applied, in the right place, with the right meaning, and each cited line reproduces. **12/12 PASS.**

| ID | Corrected text | Independent check | Grade |
|---|---|---|---|
| **m-1** | `:561`, `:564`: envelope `expectedForestRevision` mismatch = **refusal**; **plan-base** `RevisionMismatch`/`EpochMismatch` = recorded audit-only rejection; check order step 3 splits them | `Engine.cs:68–71` compares `plan.BaseForestRevision` and `plan.ApprovalSessionEpoch` — plan-base, not envelope. G-01…G-08 expectations unchanged | **PASS** |
| **m-2** | `:105`, `:376`: P0's **runtime** never reads `fixtures/synthetic/MANIFEST.sha256`; its unit test does | `git grep MANIFEST` at `861b81d3`: only `FileFormatTests.cs:24, 32` (read) and `FixtureTool/Program.cs:89–90` (write). No runtime read | **PASS** |
| **m-3** | `:605`, `:632`: `timeLapseEnabled == false` read back (`:27`, advances in `Update` `:256–262`); `CropTreeCompetition` is **not** a caller | `:27` is `timeLapseEnabled`; `:256–262` is the `if (!timeLapseEnabled) return … AdvanceOneYear()` block; full grep gives only `ForestPlayer.cs:1224`, `ForestTreeMarkingManager.cs:451`, `TreeInspectionView.cs:198` plus in-controller `:1830, :1836, :2006, :2015`; `CropTreeCompetition.cs:246` is a comment naming `GetCurrentSuppression` | **PASS** |
| **m-4** | `:380`: hazard loop `:1098–1108`, `HazardDeaths` `:1108`, call `:1134`, `KillDensityVictims` `:1137–1158` | All exact; the death application loop is `:1144–1152`, comment `:1137`, closing brace `:1158` | **PASS** |
| **m-5** | `:81`, `:347`: roll year is the **post-increment** `ecologicalYear` (`:467`); seed is the controller's `simulationSeed` | `:467` `ecologicalYear++` precedes `:1103` `Roll(rngModelVersion, "ADULT-MORT-" + TreeId, ecologicalYear, simulationSeed)` | **PASS** |
| **m-6** | `:63`, `:347`: read back `rngModelVersion`, do not assume model 1; serialized default is legacy 0; both models tested | `:9` = `rngModelVersion = SimulationRandom.LegacyModel` (= 0, `SimulationRandom.cs:15`); models 0/1 hash differently (`:46–54`) | **PASS** |
| **m-7** | `:367`: Sterbenz range `½ ≤ x/h ≤ 2`; ρ = 2.860 at ages 0/1, 1.825/2, 1.518/3, 1.288/5, 1.126/10, 1.029/28; hazard confined to ages 0–1; all fixture trees are 28 | **All ρ values reproduce** from `E(a)=b0(1−e^(−0.042a))^1.563` with `max(1,a)`: 2.8603, 1.8248, 1.5184, 1.2884, 1.1259, 1.0288. **All three fixtures have `ageAtInventoryYears` = 28** (checked in the fixture JSON) | **PASS** |
| **m-8** | `:184`, `:188`: checks on `fstat`/`fstatfs` of the **opened root handle**; local POSIX only; refuse DrvFs/9p/network/unknown; ext4-only candidate allow-list | Coherent; `/media/jer/Files` **is ext4** as the observation states; the "WSL2 roots are ext4 is background knowledge, not verified" hedge is correctly kept | **PASS** |
| **m-9** | `:132`, `:476`: one model; §4.1's 20–40 B/tree-year and <35 MB figures **withdrawn**; 650 B annual **superseded** | Single formula in both locations; the only remaining occurrences are the explicit withdrawal sentences; the two tables agree numerically | **PASS** |
| **m-10** | `:201`, `:374`, `:638`: R2 shares the Editor **binary and host**, not the running session or project | Consistent in spec, plan (`:179` A/B stays "same Editor", which is correct for A/B) and tasks (`:64`) | **PASS** |
| **m-11** | `:353`: Note 1 — for the `MortalityApplied` row the observation is **application** order, read from list order; no eighth status | Matches `:1098–1131` (hazard appended first) and `KillDensityVictims` applying in list order | **PASS** |
| **m-12** | `:312`, `:327`: the **client** mints `commandId`; a new intent gets a new ID; only a transport retry repeats one; G-02 states a reused recorded ID never executes; N08 gets the client-level negative case | Coherent with G-01…G-08 and with §9.4's dedup-before-revision ordering | **PASS** |

---

## 6. New findings summary

| ID | Severity | Location | One-line issue |
|---|---|---|---|
| **N-1** | **MATERIAL** | `:731`, `:465` | Option B cannot work without a cross-platform state checkpoint; a fingerprint chain does not supply the other platform's bits |
| **N-2** | **MATERIAL** | `:701, 703, 704, 707, 730` | Option A compares fields that cannot change the arithmetic (`os.release`, `runtime.version`, CPU flag list, trust-table digest) and its stated cost understates the refusal rate |
| **N-3** | **MATERIAL** | `:539–542` | Detached replay has no session-quiescence rule → a command committed during a load can be lost |
| **N-4** | MINOR | `:715–722`, `:738` | Refusal-class precedence undefined; `FingerprintMissingOrInvalid` vs `UnsupportedVersion` for a legacy save; a parse failure is not a fingerprint class |
| **N-5** | MINOR | `:708`, `:711` | `mathProbeDigest` adoption/missing semantics undefined; its argument list is not yet specified |
| **N-6** | MINOR | `:512, 515, 533` | 383 B is not a floor; break-even `e_min` is 330.5 B; 173,000 is not a maximum; a legal event needs a payload |
| **N-7** | MINOR | `:480, 485, 490` | 204/228/290 B cannot come from one field set; `200·N₀` is below the draft's own record; totals 9–11% low |
| **N-8** | MINOR | `:485–491, 515` | MB vs MiB undefined; one comparison mixes the units |
| **N-9** | MINOR | `:404–412` | L2–L4 scope under external-extraction build mechanism; no history layer |
| **N-10** | NIT | `:507–513`, `:480–482` | The counted example strings are not committed, so 383–629/204–290/907/93–141 cannot be re-derived by a reviewer; label them illustrative estimates or publish the strings |

**0 BLOCKING, 0 MAJOR, 3 MATERIAL, 6 MINOR, 1 NIT.**

Owner-vs-technical boundary, stated so it is not crossed: N-1's choice between the cheap B and a checkpointed B, N-2's strictness of option A, and every cap, budget, trust-table value, `fsync` acceptance and cross-platform policy remain **owner/Manager** decisions. N-3, N-4, N-5, N-6, N-7, N-8, N-9 and N-10 are documentation/technical corrections that a bounded second pass can make without any new product choice.

---

## 7. Preservation and authority readback

| Requirement | Result |
|---|---|
| Four files still DRAFT / NOT APPROVED | **Yes** — 32/13/11/14 "DRAFT" mentions; §9 still "PROPOSED, NOT ACCEPTED"; no approval sentence added |
| Nine decision-register rows unchanged | **Yes** — byte-identical, `d6365b4fe851f298`, all four files, versus `81470c59` |
| C-1A and C-7 owner CONFIRMED (D-082) | **Untouched**; no new decision ID; no D-082 rewrite |
| C-2b and C-6 OPEN | **Yes**; no capture, no lease, no machine request |
| 72 unchecked / 0 checked | **Yes**; none added or removed |
| N01–N13 proposed/unrun | **Yes**; no N-row added; "designed checks, not tests run" retained |
| No implementation/spec authority assumed | **Yes** — no cap accepted, no platform policy adopted, no P0 base selected, no reflection permission to execute |
| Only 4 M + 1 A in the delta | **Yes** — zero production, Unity, save, fixture, board, milestone or main changes |
| Dashboard / Drive | **NOT VERIFIED** (not checked in this review; canonical Git remains the authority) |

---

## 8. What I ran, and what I did not

**Ran (all non-mutating):** `git diff --name-status`/`--stat` between the reviewed pins; register-row hashing and row counts; checkbox counts; N-row sweep; stale-phrase greps; `git rev-parse` of tree objects and `Assets` trees; `git rev-list --count`; `git diff --stat` of the P0 subtree; `git branch -r --contains`; `git grep` for `fsync`, `MANIFEST`, `InternalsVisibleTo`; direct reads of the cited `ForestEcologyController.cs`, `SitkaGrowthModel.cs`, `SimulationRandom.cs`, `TreeSpeciesDefinition.cs`, `ForestTree.cs`, `ForestPlayer.cs`, `ForestTreeMarkingManager.cs`, `TreeInspectionView.cs`, `CropTreeCompetition.cs` and root `.gitignore` lines; P0 `Models.cs`, `FixtureCatalog.cs`, `LabSession.cs`, `Engine.cs`, `LabHost.cs`, `NoFollowFiles.cs`, `FileFormatTests.cs`, `global.json`, `Directory.Build.props`, `.gitignore`; fixture tree counts and ages. Scratch `python3` for: byte counts of reconstructed JSON envelopes and records, ρ from the Class III form, reachability and break-even, H2 table cells, and the compact totals. One scratch git repository to confirm the `git status --ignored` / `-uall` behaviour.

**Did not run, and do not claim:** Unity, Editor, Mono, .NET, C# compile, project tests, P0 lab, browser, Windows build, P0 L1–L4 against a real tree, save/load, `strace`/`inotifywait`, MSBuild isolation mechanisms, rendered-GitHub check beyond the closing readback, dashboard or Drive sync.

**Consequence:** the timing figures remain **estimates on assumed rates**; the libm behaviour remains **background knowledge, unverified**; the byte counts remain **my reconstructions of an unfixed illustrative layout**, not the author's strings (N-10). No numerical, platform or performance result is verified by this review.

---

## 9. Disposition of T-01, T-08 and save-side T-16

| Item | #566 | After #575 (this review) |
|---|---|---|
| **T-01** lineage | PARTIAL | **Adequate DRAFT proposal.** The stale pin, the commit-only proof and the build traps are all answered accurately, and every git/P0 observation reproduces. Still not adoptable as a *contract*: L1–L4 were never executed, the isolation mechanisms are untested, and the implementation base is a Manager re-pin after #567. Close N-9 before a handoff claims "P0 was never touched" |
| **T-08** archive, caps, replay | PARTIAL | **Adequate DRAFT proposal.** Caps are correctly demoted to placeholders, the dominant H2 term exists, the benchmark gate and refuse/abort path exist, and one size model survives. **Not** a supported limit: every number is an estimate, N-6/N-7 correct the envelope and record arithmetic, N-3 must add the quiescence rule, and the event cap stays UNDECIDED |
| **T-16** save-side libm/platform | PARTIAL | **Still PARTIAL.** The framing, the fingerprint record and the refusal partition are right, but N-1 (B), N-2 (A's compared set and cost), N-4 and N-5 must be corrected before an owner can choose a portability policy fairly. This is a decision-support defect, not a safety defect — the file-preserving fail-closed default is sound |

Carry T-02…T-07, T-09…T-15 and T-17…T-19 as before: working design proposals, subject to the minor edits above and to later complete-spec approval and verification. Nothing in this review makes any of them a runtime contract.

---

## 10. Handoff block

```text
REVIEW HANDOFF
Task: CCF #578 focused independent delta review of #575 (F-1, F-2, F-3, m-1..m-12)
Reviewer: DeepSeek V4.1 Flash (deepseek-v4.1-flash), provider opencode-go, OpenCode in T3 Code;
          dispatch preferred "DeepSeek High"; effort NOT observable and NOT claimed; model variant reported as flash
Role: Independent non-author reviewer, documentation only
Branch: review/multi-sitka-p1-575-residuals-delta-20261116-v1
Candidate reviewed: 53f3cd254e6d9ebc8ae48cee32dfd38afdd308df (exact #575 candidate, equal to remote tip)
Worktree: /media/jer/Files/CCF/Operations/CCF-578-SitkaP1DeltaReview-20261116 (Files, ext4)
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-575-Focused-Independent-Delta-Review-20261116.md
WHAT CHANGED: one Markdown file added
SERIALIZED UNITY FILES TOUCHED: none
SIMULATION / SAVE IMPACT: none
VERDICT: APPROVED AS DRAFT QUALITY (delta only); F-1 PASS with corrections, F-2 PARTIAL, F-3 PASS with corrections;
         12/12 minor corrections applied; 0 blocking, 0 major, 3 material (N-1..N-3), 6 minor (N-4..N-9), 1 nit (N-10)
MUST-NOT-CHANGE HASHES / ANCHORS: register rows d6365b4fe851f298 unchanged in all four; 72 unchecked boxes unchanged;
         Assets tree 824a1394c69404c847a9841d5f531e398589730b unchanged; P0 tree fa25ae6e39e715cc1d44a36d42607c309c7b5f7d at 861b81d3
VERIFICATION RUN: scope diff, register hash, box counts, N-row sweep, stale-phrase greps, git tree/object and count checks,
         direct source reads at the stated pins, scratch python arithmetic and byte counts, scratch-repo git status observation
VERIFICATION NOT RUN: Unity, Editor, Mono, .NET, compile, project tests, lab, browser, Windows, P0 L1-L4 on a real tree,
         MSBuild isolation, strace/inotifywait, dashboard, Drive
SPEC CONFORMANCE: NOT APPLICABLE (specification not approved)
KNOWN LIMITATIONS: byte counts are reconstructions of an unfixed illustrative layout, not the author's strings;
         timings are estimates on assumed rates; libm behaviour unverified; no platform or numerical result is verified here
DECISIONS NEEDED: portability option B's scope/checkpoint cost and option A's compared-field strictness (owner/Manager);
         caps, budget, trust values, fsync acceptance and P0 base remain open as recorded in spec §9.10
Ready for review: Yes
RECOMMENDED NEXT STEP: one bounded documentation pass for N-3..N-10 plus an owner/Manager decision on N-1/N-2;
         no implementation, fixture, Unity or Windows work is released by this review
P1 status unchanged: DRAFT / NOT APPROVED; 72 unchecked; N01-N13 unrun; C-2b and C-6 OPEN
Dashboard / Drive sync: NOT VERIFIED
```
