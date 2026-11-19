# Multi #837 — G2 source lineage, candidate admission and minimal source contract: G0 result

**19 November 2026 · READ-ONLY / DOCS-ONLY G0**

| Status | Value |
|---|---|
| Candidate source lineage | **VERIFIED** against the accessible lane, its archived bundle and live GitHub main |
| Physical custody / Ops board | **NOT INSPECTABLE in this session** (external volumes absent) — see §4 |
| V-01 two-file compile input | **PROSPECTIVE SCOPE DEFINED / NOT GRANTED** — see §5 |
| V-02 mirrored-scratch qualification | **PLAN DEFINED / NOT RUN** — see §6 |
| Minimal Stage5 amendment | **PROPOSED / NOT APPROVED** — see §8 |
| Implementation authority | **NONE — SOURCE HELD** |
| IMPLEMENTED / INTEGRATED / ACCEPTED | **NO / NO / NO** |
| L4 FAIL38 | **UNWAIVED, unchanged, not re-run** |

This result does **not** repeat the accepted #827/#834 source audit. It uses that audit's findings, adds only the lineage, permission and contract facts the accepted audit did not establish, and stops before any code, build or lease change.

---

## 1. Claim, method and limits

**Actor:** OpenCode harness, provider `opencode-go`, model **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`), host owner Windows 11 + WSL2 Ubuntu. GitHub docs access through authenticated `gh` account `jermur788`. Claim registered before any write at [#837 comment 7021994232](https://github.com/jermur788/CCF/issues/837#issuecomment-7021994232).

**Disclosed deviation:** the packet recommends local Codex GPT-6 High/Extra High or Claude Opus 5.5 High. This G0 ran on DeepSeek V4.1 Flash at the owner's direction. It had the packet's hard requirement — real lawful read-only access to the existing local #789 lane — but it is not the recommended capability, and the Manager should weigh that.

**Starting main:** `7ae4cf61f8bf7024d7917d1849a484aea20af584` (live GitHub, 2026-11-19T19:57:39Z). Local `origin/main` was stale (`852304d8`) and was not used as authority.
**Docs branch:** `review/multi-837-g2-lineage-contract-readonly-20261119-v1`.
**Only write:** this one Markdown.

**What was directly inspected**
- Local lane `/home/jer/CCF-Dev/Multi-20261119/y2engine`: 14 commits, clean tree, HEAD `6f9d2e990201bc47e361c3562be9390ed221baf6`; all `src/` and test files, the fixture, `.p0ref`, project files.
- Archived bundle `C:\CCF-Archives\Multi-20261119\engine\ccf-y2-10-engine-20261119.gitbundle` (`sha256 4733de42…`): `git bundle list-heads` = `6f9d2e99`; the archive byte inventory of the lane/evidence tars (listed, **not** extracted).
- Live GitHub main blobs for the two V-01 Assets files; CCF-HP local `HEAD`/working-tree hashes; CCF commit `a7f41983` and its subtrees.
- Accepted records only: #827/#834 (as reported by the Manager verdict), #789 result, #804 audit, #791 lease result, Stage 5 approved four-file design, live `active-tasks.md`, #730/#789/#804/#837 threads.

**What was NOT done:** no source/fixture/spec/coordination edit; no working-tree write to `CCF-HP` (ordinary `git status` refreshed Git's index, as it normally does; its pre-existing status entries sampled afterwards are content-identical to `HEAD`); no fetch/rebase/checkout; no archive extraction; no `.NET`, build, test or Unity; no board/lease/ZX20/Files write; no main/merge; no V-01/V-02 waiver; no L4 change.

**Why physical facts are missing:** `/media/jer` is absent in WSL. `lsblk` shows only the WSL virtual disks plus `/mnt/c` and `/mnt/d`. The Ops board, the Files lane, the immutable #725 original and the #791 receipts all live on `/media/jer/ZX20` or `/media/jer/Files`. I did not invent their contents. §4 states exactly what the owner/custodian must read back.

---

## 2. Candidate lineage — verified chain

The candidate is a **narrow, unmerged, local-only** P1 lane derived from the retained #725 Year-1 source. Nothing in it is on GitHub product code.

| Step | Artefact | Identity | Status |
|---|---|---|---|
| Origin | #725 retained qualified Year-1 source `ccf-725-src` | `df467dcc302eca564343e6da40f2fb88529a5d16`, 5,944 tracked files, `LEASE.md` intact, #735 custody to **2026-11-27T18:00Z** | **Reported by #791; original not mounted now** |
| Export | 30-file read-only export into new lane | 224,793 B, every lane blob ID **and mode** identical to `df467dcc`; fixture 32,567 B `bcaaa5c3…` | Reported and receipted by #791 |
| Lease | `Multi789Y2Src` at `/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/ccf-y2-10-src` | Board row `CCF-789-MULTI-YEAR2-10-SOURCE`, state READY, Unity No, **RESERVED SOLE-WRITER LEASE — no active writer**, custodian GPT-6.1 Sol session `01a1f2dd…`; `flock` on `/media/jer/ZX20/CCF-Ops/board`, 44→45 rows, board hash `4fa750eb…`→`0baf348d…` | Reported by #791; **not inspectable now** |
| Commits | Lane branch `feature/multi-789-year2-10-engine-20261119-v1` | root `4b81e71` "narrow Year1 export from #725 df467dcc" → HEAD `6f9d2e99` (2026-11-19T09:18:36Z), 14 commits, author identity `CCF Multi #791 custodian` | **Directly inspected** (clean) |
| Accessible copy | `/home/jer/CCF-Dev/Multi-20261119/y2engine` | HEAD `6f9d2e99`, clean, subtree `Tools/MultiRackSitkaP1Lab` tree `a818fb50…` | **Directly inspected** |
| Archive | `ccf-y2-10-engine-20261119.gitbundle` | 83,783 B, `sha256 4733de42…`, refs = `6f9d2e99` | **Directly inspected**; content identity proved by SHA, not by unpacking |
| Lane snapshot | `ccf-y2-10-lane-snapshot-20261119.tar` | 2,232,320 B, `e6b89e57…`, 153 entries under `./Tools/…` — **no `Assets/`** | Listed, **not extracted** |
| Evidence | `ccf-y2-10-evidence-20261119.tar` | 296,960 B, `7a1fb72b…`, only `Multi-789-OneShot-20261119/writer-m1/*` + `board-before-activation.md` | Listed, **not extracted** |
| GitHub | Any `feature/multi-789*` branch | **None** (`git ls-remote` empty) | **Verified absent** |

**Provenance statement.** The accessible home lane and the archived bundle are the same commit `6f9d2e99`; Git object identity makes their content identical without unpacking. The **physical** Files lane that Q actually built in is a different copy on an unmounted volume, so its on-disk state at Q time is reported, not currently verifiable.

**Custody statement.** The #791 record books a sole-writer *lease*, explicitly "a booking, not an active writer claim". In the GitHub records consulted for this G0 I found **no** explicit release of `CCF-789-MULTI-YEAR2-10-SOURCE` after #789 closed, and **no** current active writer. The honest state is: **lease disposition UNKNOWN — requires board readback**, not "free" and not "held".

---

## 3. What the candidate actually contains (verified read-only)

| Concern | Source fact | Consequence for G2 |
|---|---|---|
| Annual transaction | `AnnualContinuation.Advance` validates `expectedYear`/`expectedRevision`, computes outside the snapshot, calls `beforeCommit` (test fault seam), then publishes by one reference assignment, all under one `lock (sync)` (`AnnualContinuation.cs:26–47`) | Reusable compute-then-publish pattern and rollback evidence; **the lock covers the continuation, not a work transaction** |
| Zero-live | Refuses `ContinuationNoLivingStems` before any mutation (`:35–36`) | Manager's zero-live-after-Fell policy is still undecided; do not infer a rule |
| Revision | `Revision` increments once per annual advance (`:38`); tests assert `revision == year` for all ten years (`ContinuationTests.cs:32`) | Same-year work must **not** reuse this field |
| Death typing | `AnnualDeath(TreeId, EcologicalYear, string Cause, string Mechanism)`; natural deaths are `"self-thinning"` with `"hazard"`/`"boundary"` (`AnnualEngine.cs:7`, `AnnualRules.cs:49,138`) | **No typed cause enum.** A Fell must not be another `"self-thinning"` string |
| Public surface | `AnnualContinuation` and `ContinuationSnapshot` are `internal`; `AnnualEngine.ContinueInMemory()` is `internal` (`AnnualContinuation.cs:3,6`, `AnnualEngine.cs:67–70`) | No public committed current-year read model exists |
| CLI | `annual` runs one year from genesis and exits (`Program.cs:56–81`); no work/Fell route | No hidden execution path; good |
| Work/Fell | No Work Plan, approval, order, event, execution key, idempotency map or Fell transition anywhere in `src`/`tests`; the only `Felled` reference refuses a non-pristine **P0 original import** (`FixtureAccess.cs:67`) | Confirms the accepted #827 finding; this G0 adds no code |
| 170 mm | Fixture field `actionEligibleDbhThresholdMm = 170` with `populationRule` saying **every** living stem, including below-threshold stems, enters CI/RD/growth/mortality; CLI only reports `belowActionThreshold` | **Inert metadata.** It is not a Fell legality rule and must not become one without an owner product decision |
| Fixture identity | `p1d-30m-sitka-adult-v1`: 32,567 B, `sha256 bcaaa5c3…`, `contentHash b7dee8a3…`, 100 trees `P30-001…P30-100`, 0.09 ha (`3db851ec`), year 0, seed 20261116, stage MatureEquivalent, pruning 0 | Protected no-work oracle input |
| No-work oracle | Ten recorded `{Snapshot, Report}` digests, year 1 = `df2d44fd…`, year 10 = `10d31913…` (`ContinuationTests.cs:11`); 10-year trajectory hash reported as `cd5c1eb8…` | **Recorded vectors, not re-run here** |
| P0 dependency | `.p0ref/SitkaLab.Core.dll` 120,832 B `9a99f781…`; provenance `p0QualifiedCommit a7f41983`, `p0QualifiedTree f212c8f6`, SDK 8.0.133 | P0 is referenced as a **binary**, not rebuilt |
| P0 ancestry | `a7f41983:Tools/MultiRackSitkaLab` tree = `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` — exact match to the provenance tree | P0 provenance is internally consistent and reproducible from GitHub history |

**Critical build fact.** `Directory.Build.props` defaults `CcfRepoRoot` to `$(MSBuildThisFileDirectory)../../`, which resolves to the **y2engine root**, and the lane has no `Assets/` or `Tools/MultiRackSitkaLab/`. The recorded build only worked because it passed `-p:CcfRepoRoot=/media/jer/Files/…/work/ccfroot/` explicitly. **The lane cannot compile or test standalone.** Any future build must pass that property to a defined, pin-verified mirror, and that is exactly the V-02 gap.

---

## 4. Physical custody and Ops permission — NOT INSPECTABLE

This is a declared evidence gap, not a finding of free capacity.

| Required object | Recorded location | This session |
|---|---|---|
| Ops board (authoritative lease rows) | `/media/jer/ZX20/CCF-Ops/board` | **Not mounted** |
| `Multi789Y2Src` Files lane | `/media/jer/Files/CCF/Operations/CCF-789-Y2-10-20261119/ccf-y2-10-src` | **Not mounted** |
| Immutable #725 original | `/media/jer/Files/CCF/Operations/CCF-725-F0Annual-20261118/ccf-725-src` | **Not mounted** |
| #791 lease receipts / preflight / lint logs | `/media/jer/Files/CCF/Verification/Multi-791-Files-Lease-20261119/` | **Not mounted** |
| Final 50/50 qualification logs and Q evidence | Produced in the Files Ops workspace | **Not mounted; not present in the archived evidence tar** |
| ZX20 transfer archive | `/media/jer/ZX20/CCF-Transfers/Multi-20261119/` | **Not mounted** (an identical-size/hash engine bundle exists on `C:\CCF-Archives`) |

**Archived-evidence defect (material for V-02).** The archived `ccf-y2-10-evidence-20261119.tar` (`7a1fb72b…`, 296,960 B) contains only `writer-m1` from the one-shot workspace. Its `tests.txt` records **exit 1, "37/45 passed, 8 failed"**, and its stderr shows the mirror lacked `Tools/MultiRackSitkaLab/fixtures/synthetic/oracle-quarter-ha.sitkalab.json` and `Assets/ForestPrototype/SimulationRandom.cs`. That is an **intermediate, environment-failing** run, not the final accepted 50/50 run. The final logs are reported in the #789 GitHub result but are not in the surviving engine evidence archive. This does not overturn the accepted local qualification; it means the surviving archive is **insufficient to reconstruct V-02**, which is precisely why V-02 must be re-run rather than inferred.

**Archived board snapshot** (`board-before-activation.md`) is a Scenario One-era mirror dated 2026-10-16/22 and does **not** contain `CCF-789-MULTI-YEAR2-10-SOURCE`. It is provenance of the board's format, not current custody.

---

## 5. V-01 — prospective two-file compile-input scope (defined, NOT granted)

These two files are Scenario One production source. Multi #730 has **no write authority** over `Assets/`. This section records a *future read/compile-input* allowance only; it does not retroactively authorise the #789 staging variance.

| Field | Value |
|---|---|
| Path 1 | `Assets/ForestPrototype/SitkaGrowthModel.cs` |
| Git blob (live main `7ae4cf61`) | `e4c73402d46bb3b75a9cba2e4622857a913fac92` |
| SHA-256 of blob bytes | `291601041876d83451259990f82219c6ccd3fb704ba55e6dcb8189f13b446463` |
| Path 2 | `Assets/ForestPrototype/SimulationRandom.cs` |
| Git blob (live main `7ae4cf61`) | `bb83d7f8468002ab13a6aec6caf548e0bf3ad34b` |
| SHA-256 of blob bytes | `d1b52a604d2593c833e80f4c20bce1ead56f2ad51f19ce12625e4a6bb2ec927b` |
| Pin record | `CcfSourcePins.cs` (`InspectedCommit 90583811…`); also embedded in the fixture `sourceProvenance` |
| Current identity | Blob IDs at live main and at CCF-HP `HEAD`, and working-tree SHA-256, are **all identical to the pin** |
| Compile role | `SitkaP1.Core.csproj:27–28` `<Compile Include="$(CcfRepoRoot)…" Link="CcfSource/…"/>`, unchanged/read-only |
| Owning authority | Scenario One code authority (Manager #824) + product owner `jermur788`; **not** Multi #730 |
| Write permission proposed | **NONE.** Read/compile-input only, pinned to these two hashes |
| Granted now | **NO** |

**Honest caveat.** Blob equality is *evidence of identity*, not *authority to use*. The #789 variance was staging these files from public source outside the literal grant; the Manager refused retroactive authorisation (#804). A future grant must be **prospective**, name the scenario/source owner's approval, and state that no `Assets/` byte may be written by the Multi writer.

**Reader test cost.** The existing test `Program.cs:333–339` runs `git rev-parse HEAD:<path>` **and** a working-tree SHA-256 for all four pins (the two Assets plus two cited files). It therefore requires a mirror that is a real Git repo at a commit whose working tree matches the pins. A narrow lane cannot satisfy it; this is a V-02 mirror requirement, not a reason to weaken the test.

---

## 6. V-02 — independent mirrored-scratch qualification (plan, NOT run)

**What V-02 must prove**, under a later lawful lease and independent actor:

1. **Reproducible base.** Fresh clone of `jermur788/CCF` at `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` (reachable from the P0 branches; it is *not* an ancestor of live main — GitHub compare says diverged 182/26 — so the exact commit must be fetched, not assumed). Prove `Tools/MultiRackSitkaLab` subtree = `f212c8f6…`.
2. **Exact candidate mirror.** Overlay lane `6f9d2e99`'s `Tools/MultiRackSitkaP1Lab` into the clone. Publish a per-file manifest (path, bytes, SHA-256, Git blob) and prove **zero** missing/extra/content/mode differences versus the bundle ref `6f9d2e99`.
3. **Explicit compile root.** Build with `-p:CcfRepoRoot=<mirror root>/` (the default points at the lane and cannot resolve `Assets/`). Record the resolved absolute paths of the two V-01 inputs and their hashes **as compiled**.
4. **P0 closure.** Either place the pinned `.p0ref/SitkaLab.Core.dll` (`9a99f781…`, 120,832 B, provenance `a7f41983`/`f212c8f6`) or rebuild it from subtree `f212c8f6` with `build/build-p0-core-ref.sh` and require the same DLL hash. No P0 source edits.
5. **Offline environment.** SDK 8.0.133 from `global.json`, pinned NuGet config, no network restore, loopback DOWN / DNS_FAIL recorded.
6. **Full gate.** `dotnet build` 0 warnings/0 errors; full test suite expected **50/50**; capture stdout verbatim and the per-test list.
7. **No-work bit identity.** Re-run the ten recorded `{Snapshot, Report}` digests and the 10-year trajectory hash; require exact equality to the recorded values. Any mismatch is a STOP, never a re-baseline.
8. **Negative controls.** Stale year, stale revision, zero-live, injected pre-commit fault, duplicate concurrency, wrong-case ID, dead ID — each must refuse without partial state.
9. **Independence.** A different actor, session and model from the implementation writer; own probes, own mutant rebuilds, own hash reconciliation.
10. **L4 and environment gaps stated separately.** Default .NET **L4 FAIL38** remains unwaived and must not be relabelled green; the seven environment-dependent tests and the "Year-1 compared to recorded digest rather than pre-change rebuild" limitation must each get an explicit disposition.

**Why this is not already done.** #804 accepted V-02 as unresolved: the Q PASS belongs to a *tested mirror*, not to the narrow lane, and the mirror is no longer available. This G0 adds the reproducible base (`a7f41983` + subtree `f212c8f6`) and the exact missing archive evidence, but **does not execute V-02**.

**Bit-identity warning for the verifier.** The recorded digests are computed over `System.Text.Json` bytes of `{Snapshot, Report}` (`ContinuationTests.cs:118–119`). Adding **any** serialised member to `ContinuationSnapshot`, `AnnualState`, `AnnualTree`, `AnnualReport` or `AnnualDeath` changes all ten digests even for a no-work run. V-02 must therefore be run on the **contract-shaped** candidate, and the contract must preserve the old shapes (see §7).

---

## 7. Targeted source contract — smallest compatible interface

Direction only (per Manager §3 and the approved Stage 5 design). Names are proposals, not an approved ABI.

**C-1. Keep the annual transaction exactly as it is.** Extend `AnnualContinuation`'s existing `lock (sync)` compute-then-publish pattern. A Fell batch revalidates the whole immutable approved Work Plan from the one current snapshot, builds roster/result/event/history off to the side, and publishes one reference assignment. Add a work fault seam beside the existing `beforeCommit` seam. Any refusal or exception leaves biology, events, work state and history unchanged.

**C-2. Two clocks, but do not touch the serialised snapshot.** Keep `Revision` = annual advances and keep `revision == year` for no-work runs (`ContinuationTests.cs:32`). Introduce a **separate session work revision** stored **outside** `ContinuationSnapshot` (because C-7's bit identity forbids adding a serialised member). The full current-source token is:

```
(domain, fixtureId, fixtureFileSha256, fixtureContentHash,
 ecologicalYear, ecologyRevision, workRevision, optional committed-content digest)
```

A successful Fell increments **workRevision only**. The year is unchanged.

**C-3. Typed Fell, separate from natural death.** Add a new `FellRecord(TreeId, EcologicalYear, FellCause, WorkRevision, ExecutionKey)` (and a typed cause enum or exact token). Do **not** reuse the `AnnualDeath` string `"self-thinning"` and do not mutate `AnnualDeath`'s shape. The committed roster still records the tree's death state so the annual engine excludes it.

**C-4. Exactly-once only in the live process.** A session-local map inside the continuation instance holds `executionKey → committed result`. Identical key + identical payload returns the same committed result (no-op). Same key + different payload refuses. New key targeting an already-dead/already-felled ID refuses. No save, journal, restart or crash-replay promise.

**C-5. Real following year.** The next `Advance` must consume the actual surviving roster (already `Where(IsLiving)` and archive-preserving in `AnnualRules.Calculate`), with genuine growth/competition/natural mortality, one year increment only. Approved-unexecuted work follows the owner-approved warning/consent stale-cancel with **zero Fell**; denial changes nothing.

**C-6. One-scope public committed read frame.** Add one immutable public DTO exposing the committed year, ecology revision, work revision, roster, aggregates, provenance and the single Stage 5 management-scope ID (the V1 singleton). Do not expose `AnnualContinuation`'s internal mutable snapshot. No rack geometry, no area partitioning, no G1 comparator.

**C-7. Preserve the protected no-work oracle.** No edits to the fixture, the ten digests, the 100 IDs, the P0 binary or the two Assets. If a future design cannot express work state without changing serialised shapes, it must be an explicit Manager/owner re-baseline decision, never a silent rewrite.

**C-8. Fail closed on unresolved product decisions.**
- `actionEligibleDbhThresholdMm = 170` is **not** a Fell legality rule. No eligibility policy may be coded until the owner decides.
- A Fell batch that would leave **zero live stems** needs an explicit policy (refuse the batch vs legal terminal state). The current engine refuses zero-live *advance*; that is not a Fell policy.

---

## 8. Smallest Stage 5 spec amendment (proposed)

No new spec family and no new review chain. Amend the four already-approved Stage 5 files on a new docs branch:

| File | Proposed minimal change |
|---|---|
| `specs/multirack-sitka-2d-intervention-workflow-v1/spec.md` | Add one normative § "G2 source binding" capturing C-1…C-8: two clocks (ecology revision vs work revision), exact token, typed Fell, atomic publish, session-only replay, one-scope read frame, old no-work bit identity, and the two open product decisions |
| `plan.md` | Add gates M-0 (V-01 prospective grant + V-02 independent mirror qualification) before T-03 implementation; name the narrow-lane limitation |
| `tasks.md` | Insert T-02a (source-contract annex) and T-03 exact writer/allowlist; keep all 26 R / 24 X unchecked until executed |
| `checklists/requirements.md` | Add three items: (R) work revision is distinct from ecological year and old no-work digests are unchanged; (R) typed Fell distinct from natural death; (X) replay/refusal leaves zero partial state |
| `Docs/Project/decision-log.md` | One D-entry **only after the owner decides**: 170 mm eligibility and zero-live-after-Fell |

Until those two product decisions exist, implementation remains **HELD**, not "pending coding".

---

## 9. Prospective implementation writer allowlist (NOT granted)

For a later single-writer task only, on a new worktree derived from `6f9d2e99`:

| Action | Path | Current SHA-256 |
|---|---|---|
| Modify | `Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs` | `5f6ecc09…` |
| Modify | `Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs` | `54c93746…` |
| Add | `Tools/MultiRackSitkaP1Lab/src/Core/WorkContract.cs` (proposed) | — |
| Modify | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs` (registration only) | `deb0f736…` |
| Modify | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/ContinuationTests.cs` (additive) | `fad86578…` |
| Add | `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs` (proposed) | — |

**Read-only inputs:** fixture, `CcfSourcePins.cs`, `AnnualRules.cs`, `DerivedManifest.cs`, `TrustTables.cs`, `.p0ref/**`, the two V-01 Assets.
**Forbidden:** any `Assets/**` write; fixture edit; digest re-baseline; P0 source edit; Unity/URP; CLI public route (unless separately approved); economy/save/Host/G1/rack; main merge.

A separate independent verifier (different actor/session/model) must own V-02 and the feature qualification. The writer may not verify itself.

---

## 10. Exact permission and custody request

To move from G0 to an admissible implementation packet, the Manager/owner must supply **all** of:

1. **Board readback** of `CCF-789-MULTI-YEAR2-10-SOURCE`: current state, owner, lane path, HEAD, lock/flock status, board hash, `lanes.json` hash, and whether the `Multi789Y2Src` lease was released after #789 or still reserved. The board is at `/media/jer/ZX20/CCF-Ops/board`; this session cannot see it.
2. **#725 custody confirmation** from the #735 holder: original still clean at `df467dcc`, `LEASE.md` intact, custody window to 2026-11-27T18:00Z, no conflicting reader.
3. **V-01 prospective grant** from the Scenario One authority (Manager #824) + owner: read/compile-input only for the two exact blobs in §5, explicitly non-retroactive, with **zero** `Assets/` write permission.
4. **V-02 execution grant** for an independent actor: access to a complete reproducible CCF clone at `a7f41983`, the pinned P0 binary or rebuild authority, offline SDK/package inputs, and a defined evidence root. This is a *native* grant; G0 does not create it.
5. **Manager approval** of the §8 amendment (and the owner's two product decisions).
6. **One implementation writer** with an exclusive worktree/lane and an exact allowlist, plus **one separate verifier** with its own lane and no write overlap.

Without 1–6, source work stays **HELD**. A prospective reservation is not a granted lease, and this G0 does not treat it as one.

---

## 11. STOP conditions

Stop, do not implement, and return to #730 if any of these is true:

- the Ops board cannot be read back, or shows an active/conflicting writer on the lane or the two Assets;
- the #725 original is not clean at `df467dcc`, or its custody is contested;
- the V-01 two-file read scope is not explicitly granted by the Scenario One authority (blob equality is not permission);
- V-02 has not been executed independently on the exact `6f9d2e99` candidate with the recorded ten digests unchanged;
- the 170 mm eligibility or zero-live-after-Fell decision is still open (no silent default);
- a proposed change needs a new serialised member in the protected snapshot/report (would break the ten digests);
- anything requires editing `Assets/`, the fixture, `.p0ref`, the no-work oracle, or weakening an existing refusal/test;
- any request to call L4 FAIL38 green, or to treat a reported 50/50 as re-verified.

---

## 12. What this result does and does not claim

**Claimed (directly observed):** the lane/commit/bundle identity; the absence of any work/Fell implementation; the exact V-01 blob and SHA identity at live main; the inert 170 mm metadata; the `revision == year` test coupling; the digest serialisation shape; the missing `Assets/`/P0 fixtures in the lane; the archived evidence being an intermediate failing run; the absence of a GitHub branch.

**Not claimed:** physical lease state; V-01/V-02 completion; any build/test result; 50/50 re-verification; the ten digests re-run; L4 disposition; Unity/rendered behaviour; main integration; product acceptance.

**Next action for Manager #730:** rule on this G0, then either (a) obtain the six items in §10 and approve the §8 amendment, or (b) return a precise BLOCKED with the owner/custodian action that is missing. The recommended model for the actual V-02 work remains local Codex GPT-6 High or Claude Opus 5.5, not this G0's model.

**Dashboard/Drive mirrors:** NOT VERIFIED.
