# Multi #841 — Internal-desktop G2 real Fell, approved Work Plan and genuine Year+1 ecology: implementation and independent qualification

**19 November 2026 · ONE PARENT ORCHESTRATION · DESKTOP INTERNAL DRIVE ONLY · NO ZX20 · NO EXTERNAL VOLUME**

Orchestrator: **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`, variant `high`), T3 Code / OpenCode, Windows host driving WSL2 Ubuntu.
Writer: same orchestrator session, exclusive internal lane (declared as writer, not as its own verifier).
Independent qualifier: **Claude Opus 5.5** (`claudeAgent`, effort `high`), a different provider and a nonauthor, in a separate internal lane.

---

## 1. Plain-English result

The Multi 2D engine can now do the real thing the Stage 5 design asked for, at engine level:

* a player can pick **exact living tree IDs** with a recorded reason and assemble a Work Plan;
* the whole batch must be **explicitly approved**, and approving it fells nothing;
* one separate **Execute** then fells the whole batch **atomically** — either every valid selected tree is felled, with one typed work event, or nothing at all changes;
* the **next genuine annual step** grows and kills trees from the **actual surviving stand**, not from the pre-work stand;
* the original **100 tree IDs**, the **ten-year no-work digest oracle** and every existing serialised shape are unchanged.

This is an **engine/core capability**. It is **not** a browser UI, not a save system, not the multi-area G1 work, and not a Unity integration.

---

## 2. Status vocabulary — what this result does and does not claim

| State | Meaning here |
|---|---|
| **IMPLEMENTED** | Yes. Source written, built and covered by native tests in an exclusive internal lane. |
| **REVIEWED / QUALIFIED (Tier 2)** | Independent nonauthor Claude Opus 5.5 qualification of the exact candidate — see §6. |
| **INTEGRATED** | **NO.** The implementation branch is deliberately **local-only**; it is not merged to `main`, not published as public source, and not wired to the Viewer/CLI. |
| **VERIFIED in Unity / rendered / Windows build** | **NO.** Out of scope for this task by instruction. |

Historical gates remain untouched and unwaived: #804 PARK / main-BLOCKED, historical V-01/V-02 variance, **L4 FAIL38**, no G1 multi-area comparability, no economics (G4), no save/Host (G3).

---

## 3. Authoritative inputs actually used

| Input | Observed value |
|---|---|
| Live `main` at start of orchestration | `4a06f1981041b7307ae8225fc0b5aca969f3f20b` |
| Live `main` at result publication | `ab3c5ec8b78498cb159565b0916c7b5c2d8a49cb` (main advanced during the run; unrelated Scenario One 3D-manager succession) |
| Candidate engine source | `/home/jer/CCF-Dev/Multi-20261119/y2engine`, branch `feature/multi-789-year2-10-engine-20261119-v1`, HEAD `6f9d2e99…` — **clean, read-only, never written** |
| Public source anchor | `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` |
| P0 subtree | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (matches the qualified tree) |
| P0 binary reference | `.p0ref/SitkaLab.Core.dll` sha256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| Fixture | `p1d-30m-sitka-adult-v1.sitkap1fx.json`, sha256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, 100 unique IDs `P30-001…P30-100` |
| Toolchain | .NET SDK **8.0.134** at `/home/jer/.dotnet` (`global.json` 8.0.133, `rollForward: latestPatch`), Linux `6.18.58.1-microsoft-standard-WSL2` |

No source audit was repeated. #827 and #837 findings were consumed as given.

---

## 4. Source custody, consent and gates actually established

**Internal-only custody.** All work is on the desktop's internal disk (`/dev/sdd`, 915 GB free). No ZX20, no `/media/jer/Files`, no mount, no external board write and no lease claim was made or inherited. Historical #791 external lease facts remain **UNKNOWN**, not "released".

**Lanes.** Created fresh after verifying the paths did not exist; nothing was stashed, cleaned, reset or overwritten:

* Writer: `/home/jer/CCF-Dev/Multi-20261119/g2-fell-writer`, branch `feature/multi-841-g2-atomic-fell-year-20261119-v1`.
* Qualifier: `/home/jer/CCF-Dev/Multi-20261119/g2-fell-qualification`, branch `qualification/multi-841-g2`, a fresh clone at the exact candidate commit.
* The user's dirty `CCF-HP` Unity checkout was **read only**, never modified.

**V-01 (read/compile-only), honest split:**

* *Product owner half:* **GRANTED** in this run — explicit authorisation to use exactly the two pinned blobs as read/compile-only input, with zero `Assets/` writes.
* *#824 Scenario One source-owner half:* **REQUESTED, NOT YET ANSWERED** ([#824 comment](https://github.com/jermur788/CCF/issues/824#issuecomment-7023840780)). This result does **not** claim Scenario One source-owner approval.

Both blobs were independently re-hashed from clean `main` and match the pins exactly; neither is locally modified. They are consumed unchanged from the public anchor commit's own tree, because `SitkaP1.Core.csproj` links them as compile inputs and the engine cannot build without them. **No `Assets/` write occurred.**

**§8 technical contract.** The product owner directly directed that D-093 be applied and that §8 be implemented in this run. §8 therefore carries **owner direction**, and remains **formally PROPOSED pending #730 acceptance** — this result does not claim Manager approval of the contract.

**V-02.** Not waived. The baseline was nevertheless established genuinely from scratch: a complete source closure anchored on the public commit `a7f41983`, the exact #789 engine tree overlaid byte-exactly, the pinned P0 binary, and a clean rebuild. The archived `writer-m1` 37/45 environmental run is not reused as proof.

---

## 5. What was implemented (IMPLEMENTED — source)

Changed paths, exactly the approved allowlist and nothing else:

```
A  Tools/MultiRackSitkaP1Lab/src/Core/WorkContract.cs
M  Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
M  Tools/MultiRackSitkaP1Lab/src/Core/AnnualEngine.cs
M  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs        (registers the new suite only)
A  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
```

Provably **not** touched: `AnnualRules.cs`, `src/Cli/**`, `fixtures/**`, `TrustTables.cs`, `DerivedManifest.cs`, `.p0ref/**`, `Assets/**`, `Tools/MultiRackSitkaLab/**`. The ten golden digest literals are unchanged.

**Behaviour delivered**

1. **Committed-current frame.** `CommittedFrame` exposes one immutable view: single management scope `MS-P30-UNDIVIDED-30M-V1`, domain, fixture file/content identity, ecological year, annual revision, work revision, the exact roster and real observed aggregates. Nothing preview-derived is elevated to authority.
2. **Manual Work Plan.** `AddPendingFell(treeId, reason)` requires a live, exact, case-sensitive ID and a reason. Duplicates, unknown IDs, case shifts, blank reasons and edits to an approved batch are refused with machine-readable codes. Selecting and approving fells nothing.
3. **Approval is the only commitment gate.** `ApprovePlan(planId)` produces one immutable `WorkPlan` bound to a `WorkSourceToken` covering fixture file SHA-256, fixture content hash, ecological year, annual revision, work revision, management scope, RNG model, growth model and algorithm lineage.
4. **Atomic typed Fell.** `Execute(plan, executionKey, beforePublish)` validates the key map, approval, source token, emptiness, duplicates and every ID's liveness **before** any publish. It then builds the new roster, the typed `FellRecord`/`WorkEvent`, the new work revision and the idempotency entry, reaches a single test-only fault seam, and publishes **one** immutable `WorkRoot` reference under the engine lock. A fault before publish leaves roster, event, work revision and key map all untouched.
5. **Typed cause.** Human Fell uses cause `fell` / mechanism `manual-work-plan`, deliberately distinct from the natural `self-thinning` + `hazard`/`boundary` deaths; the historical `AnnualDeath` shape and cause strings are not reused.
6. **Once-only, in process.** An identical key + identical payload returns the recorded result as a no-op; a reused key with a different payload is refused; a second execution of an already-executed plan is refused; no persistence or restart guarantee is implied.
7. **D-093 applied.** (P1) the 170 mm `actionEligibleDbhThresholdMm` is carried as metadata only and gates nothing — stems below and above 170 mm are equally selectable. (P2) a batch that would leave zero living stems is refused **as a whole**, with no tree, event, work-revision or idempotency mutation.
8. **Two clocks.** Fell moves only the work revision. The annual step moves only ecological year + annual revision. `Revision == year` still holds for no-work runs.
9. **Genuine Year+1.** After a year-*y* Fell, the next `Advance` runs the real competition/growth/mortality rules over the actual 99 post-Fell survivors; felled stems stay frozen and excluded; the year-*y* event is preserved.
10. **Stale-work policy.** With pending or approved-but-unexecuted work, `Advance` refuses unless explicit consent is given; consent stale-cancels at **zero Fell** and then advances through real biology; denial leaves the state byte-identical.

---

## 6. Independent nonauthor qualification (Claude Opus 5.5)

**Verdict: PASS — the feature qualifies at Tier 2.** Four non-blocking advisories (A-1…A-4), no acceptance failure.

The qualifier is a **different provider and a nonauthor**: Claude Opus 5.5 (`claude-opus-5-5`, effort `high`) in the Claude Code harness, on the same Windows 11 → WSL2 Ubuntu host, internal disk only. It worked in a **separate fresh lane** (`g2-fell-qualification`, branch `qualification/multi-841-g2`) cloned at the exact candidate commit, and wrote its own report and probes.

**Independently re-verified identity** — all matched: lane/HEAD `8741a328…`, engine tree `bf4b7439…`, P0 tree `f212c8f6…`, both pinned `Assets` blobs, `.p0ref` DLL sha256 `9a99f781…` (also enforced by the build's own `VerifyP0CoreReference` target), and the fixture sha256. `git diff --name-status be9b40c5 HEAD` returned exactly the five allowlisted paths. The candidate working tree stayed clean after every run, including after all probes.

**Reproduction:** clean rebuild (bin/obj deleted) → 0 warnings, 0 errors; **62/62** tests pass; all ten YEAR digests match the untouched literals at `ContinuationTests.cs:11` character for character.

**Source audit — 11/11 PASS**, including: unchanged `ContinuationSnapshot`/`AnnualDeath`/`AnnualTree`/`AnnualState`/`AnnualReport` shapes with new state confined to the new `WorkRoot`/`ExecutedWork` types; single-assignment publish inside one lock with the fault seam after all computation; typed `fell`/`manual-work-plan` cause; 170 mm carried as metadata only; and a plan token binding all twelve source/model fields.

**Its own adversarial probes — 17 probes, 2,113 checks, 0 failures**, built outside the repo against the candidate's built binary:

* **Concurrency:** 16 threads with different keys, 200 repetitions — exactly one winner each time, every loser refused with no leftover state.
* **Faults:** a pre-publish fault on a 5-stem batch left the full session image byte-identical; a retry then committed once.
* **Stale plans:** reusing a plan after the year advanced was refused; tampering with each token field was refused every time.
* **D-093:** felling 8 stems below 170 mm succeeded without refusal.
* **Genuine Year+1:** after a Fell at years 0, 3, 6, 7 or 8, the next year matched running the survivors alone — including years with real natural deaths — and 18–20 neighbours showed genuinely lower competition.

**Advisories (explicitly not blocking, recorded as bounded follow-ups):**

| ID | Advisory | Disposition |
|---|---|---|
| A-1 | The legacy `Advance(year, revision)` overload (`AnnualContinuation.cs:240`) does not fence the work revision, so a caller holding a pre-Fell year/revision can still advance. The new four-argument overload does fence it. | **Must be closed before any multi-reader/Host slice.** Not a G2 failure; no persistence exists today. |
| A-2 | On key retry the payload match ignores `PlanId` (line 163). Nothing changes and the recorded result is returned. | Harmless; tighten when the event ledger gains provenance use. |
| A-3 | The test-only `beforePublish` hook is re-entrant, which can produce two events. Production never passes the hook. | Test-only; guard if the seam is ever exposed. |
| A-4 | At selection, already-felled / unknown / wrong-case / empty IDs all return `UnknownTreeId`, so a UI cannot distinguish "already felled" from a typo. | UX refinement for the player-facing slice; the refusal is still correct and machine-readable. |

**Not verified by the qualifier (correctly out of scope):** Unity, 3D/rendered review, save/load and persistence, a Windows-native build, and integration into `main`. It pushed nothing and edited no tracked file.

Artifacts: `/home/jer/CCF-Dev/Multi-20261119/qualification/INDEPENDENT-QUALIFICATION-RESULT.md` and `qualification-run-20261119T222113Z.log` (internal storage). Disposable probe project: `/home/jer/CCF-Dev/Multi-20261119/g2-q-probes/`, outside the candidate repo.

**Decision on the advisories:** the qualified candidate `8741a328` is published **unchanged**. Correcting an advisory would change the artifact and require a fresh independent pass; the reviewer's verdict is PASS and none of A-1…A-4 is a G2 acceptance failure. They are carried as the first items of the next slice, with A-1 mandatory before any multi-reader or Host work.

---

## 7. Native evidence

**Untouched baseline (author lane, before any change):** build clean, **50/50** tests pass, all ten no-work digests reproduced exactly.

**Final candidate:** build clean, **62/62** tests pass — the 50 inherited tests plus 12 new G2 tests.

Recorded no-work digests, unchanged byte-for-byte and still asserted by the untouched golden literals:

```
YEAR 1  df2d44fdf9a879bd8191b89597f712ca68bacb62ff17b6033b918d9e9f5586f1
YEAR 2  23a1d889f551a840ae2ab7e689ab6b977dc1b7dabbf187fe7207aeae561c6abd
YEAR 3  953fc7ef660e66cc5a76e89a01d12aa2524d4e5d6d4d5bb04867d4700bb56afc
YEAR 4  6fc220c377555edc15892613c6468a3ca00884b310f85ad80d8102f7eef3f5ba
YEAR 5  cbd51e38889da9cf65376d38b03a0befe49f398a125802c7cb85d1079992a487
YEAR 6  01cfb6560386799d01dd3ef7c8a1b8d98b2cff6e4fc054e24509d6ca308b796e
YEAR 7  c6baab550107d0978ade9f3107ac715e9dcc0a28a3a01d27b70f53accb59c646
YEAR 8  61879c1e79aaf9c5b503d874a38837807da80d01bb6f645560a842beb03e7ab0
YEAR 9  faaea476fcdaec744e18fec2953bbe8125b56a8e3d52f826e30fdc4aa956169a
YEAR 10 10d31913531c627089a01e5c66aeea0c6fee4efad74e31d31f579e5264bcd7d5
```

---

## 8. Exact candidate identity and how to reproduce

* Candidate commit: `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6`
* Engine tree at that commit: `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4`
* Local branch (not pushed): `feature/multi-841-g2-atomic-fell-year-20261119-v1`
* Base: public anchor `a7f41983` + byte-exact #789 engine overlay (admission commit `be9b40c5`)

Reproduce:

```bash
export DOTNET_ROOT=/home/jer/.dotnet; export PATH=$DOTNET_ROOT:$PATH
cd /home/jer/CCF-Dev/Multi-20261119/g2-fell-qualification
dotnet build Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln -v q
dotnet run --project Tools/MultiRackSitkaP1Lab/tests/Core.Tests
```

**Durable, reviewable identity (internal storage, not pushed to GitHub):**

| Artifact | Value |
|---|---|
| Candidate commit | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6` |
| Engine tree | `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4` |
| Admission commit (engine overlay) | `be9b40c5f5de45059f3356c5972434da9bef8ae2` |
| Patch series from anchor | `/home/jer/CCF-Dev/Multi-20261119/artifacts/multi-841-g2-candidate-from-anchor-a7f41983.patch` |
| Patch sha256 | `7791b9ffefaa8f0284450d694bae6f8f7f8c13deae4f3f27a9563860091d07fe` (302,674 bytes, 2 commits) |
| Patch check | `git apply --check` **PASSES** on a clean `a7f41983` checkout |

Because the admission commit carries the whole engine directory, the patch is self-contained on top of the public anchor; no private history is required to reproduce the candidate.

---

## 9. Open gates this result does not close

1. **#730 Manager acceptance** — not given here; this document is the input to that verdict.
2. **#824 Scenario One source-owner V-01 half** — requested, unanswered. Owner half granted.
3. **§8 formal technical signoff** — owner-directed, still formally PROPOSED.
4. **Historical V-01/V-02 variance and L4 FAIL38** — unwaived; a new G2 pass cannot retroactively erase them.
5. **Source publication/merge** — deliberately not done. The branch is local-only with the identities above.
6. **No player-facing UI, CLI route, save, Unity, economics or G1 multi-area work.**

---

## 10. Next minimal step

1. **Close A-1** (fence the legacy `Advance(year, revision)` overload against a non-zero work revision) — mandatory before any multi-reader or Host slice, cheap, and testable in the same lane.
2. Then wire the capability to a **read-only player view plus a manual Work Plan screen** (Node/2D viewer), still with no save and no Unity: show the committed frame, allow per-ID select with reason (and a distinct "already felled" refusal, A-4), show the approved batch, require a separate Execute, and show the following-year report. That is UI work on top of a qualified core and needs its own allowlist and acceptance.

---

## 11. User action remaining

None required for correctness of this result. The single decision still outstanding is the source-owner V-01 reply on #824; and #730 must issue the acceptance verdict.
