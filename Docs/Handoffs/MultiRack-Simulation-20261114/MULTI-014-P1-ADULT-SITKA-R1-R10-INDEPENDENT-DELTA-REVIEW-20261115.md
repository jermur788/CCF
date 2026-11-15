# MULTI-014 — independent focused delta review of the repaired Sitka P1 four-file DRAFT

**Date:** 15 November 2026

**Claim:** [CCF #542](https://github.com/jermur788/CCF/issues/542)

**Verdict: NEEDS CHANGES — DRAFT QUALITY ONLY.** Eight repairs PASS; R-2 and R-4 are PARTIAL. Residual severity: **0 BLOCKING, 1 MAJOR, 1 MINOR**. The remaining changes are narrow documentation repairs. This review approves neither the P1 specification nor implementation, Unity execution, scientific accuracy or game integration.

The numerical-reference repair is adequate: the actual pinned Unity Editor/Mono capture is the reference, C-2a precedes it, and C-2b follows it. The remaining MAJOR issue is the Mode A promise of identical canonical hashes while permitting a different load history. The obsolete `P1-G0` name also remains in REQ-F09. Both Manager watchpoints are confirmed against the exact candidate, rather than inferred from the repair handoff.

## 1. Reviewer, independence and custody

| Item | Directly observed record |
|---|---|
| Actual reviewer | **OpenAI `gpt-6.1-sol`, High reasoning**, Codex harness in local T3 Code; no delegates |
| Independence disclosure | Codex previously authored **LAB-06 / MULTI-009** source/evidence research. This is a separate review session, independent of the **GPT-6 DRAFT author** and **Claude Opus 5.5 MULTI-012 reviewer/MULTI-013 repair author**. It assesses documentation repairs; it is **not independent scientific replication of the earlier Codex research**. |
| Physical host | `jer-laptop`, Linux, user `jer` |
| Owned worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-014-SitkaP1-DeltaReview-20261115`; backing mount verified as `/dev/sdb4`, ext4, `/media/jer/Files` |
| Review branch | `review/multi-014-p1-draft-r1-r10-delta-20261115-v1` |
| Full starting HEAD | `8756c452cf76a8dd3169f2ab40c93349cb1edc60`; remote review branch and repaired candidate matched; new worktree was clean |
| Preflight and registration | Fresh-read #542 as READY / NOT RUNNING, the complete packet, original findings, Manager ruling and repair handoff. Read live GitHub coordination on both `main` and the workflow-designated `ops/workflow-foundation-v1`; neither supplied a conflicting MULTI-014/#542 reservation. Checked registered worktrees and remote branch heads; proposed path was absent and no worktree used the review branch. Created the isolated review worktree, then posted [actual RUNNING registration](https://github.com/jermur788/CCF/issues/542#issuecomment-6896331423) **before repaired-spec/source inspection, rendering or report edits**. The direct owner instruction/packet authorises this bounded review; no live coordination edit or general board ownership is inferred. |
| Preserved unrelated work | Primary remained on `ops/workflow-foundation-v1`; its pre-existing changes to `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were left untouched. No other worktree was changed. |
| Context | Candidate lineage's latest canonical context/instruction change is `62e39ec621e38a84030c08b35a171e297056c9ed`; applied canonical authority and Scenario One boundaries, with live coordination read separately. No context/mirror regeneration. |
| Unity ownership | No Unity session, lease, source harness or build was requested, acquired or run. A documentation worktree is not a Unity machine reservation. |

### Exact inputs

All line references below are to these immutable snapshots. `spec`, `plan`, `tasks` and `requirements` mean the four files under `specs/multirack-sitka-p1-adult-rules-comparator-v1/` at the repaired candidate.

| Input | Full ref |
|---|---|
| [Binding MULTI-014 packet](https://github.com/jermur788/CCF/blob/84331a8bb4605ad5bfd3e119680321d38079cc74/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-014-SITKA-P1-DRAFT-R1-R10-INDEPENDENT-DELTA-REVIEW-PACKET-20261115.md) | `84331a8bb4605ad5bfd3e119680321d38079cc74` |
| Repaired four-file DRAFT and MULTI-013 handoff | `8756c452cf76a8dd3169f2ab40c93349cb1edc60`, `docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1` |
| Pre-repair DRAFT | `50178f9df0641a75547fbeb29033d18a69a5cbb7` |
| [Original MULTI-012 review](https://github.com/jermur788/CCF/blob/effc9f6cb9c592fb65a96aed640013c0157dbab6/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md) | `effc9f6cb9c592fb65a96aed640013c0157dbab6` |
| [Manager R-1…R-10 ruling](https://github.com/jermur788/CCF/blob/d8b397b119dde884be17d02d117b993db58721d1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-012-P1-DRAFT-NEEDS-CHANGES-AND-REPAIR-DECISION-20261115.md) | `d8b397b119dde884be17d02d117b993db58721d1` |
| [Manager MULTI-010 evidence-plan disposition](https://github.com/jermur788/CCF/blob/6739eb65b8498e2a63dbb5238acee450bb8e1b66/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md) | `6739eb65b8498e2a63dbb5238acee450bb8e1b66` |
| CCF implementation snapshot | `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; live `main` also pointed here at review preflight |
| Qualified standalone P0 source | `2659db99e317d4aa93b51cf97131a61eab5fdeec`, `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1` |
| Approved P0 specification | `c92af201b6366e7e6314d587b6d4071dee352fa3`, `docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1` |

Read all four repaired documents and compared the repair with `50178f9d`. The repair diff is exactly **four MODIFIED DRAFT Markdown files plus one ADDED MULTI-013 handoff**. No `Assets`, `Tools`, `Packages` or `ProjectSettings` changes from the CCF snapshot are present in that candidate. Each repaired document was independently fetched by exact SHA from GitHub and matched the local Git blob byte-for-byte before rendering.

## 2. Independent R-1…R-10 matrix

| Finding | Grade | Evidence at repaired candidate and independent check | Residual severity |
|---|---|---|---|
| **R-1 — Unity numerical reference, C-2a/C-2b** | **PASS** | spec 55–61, 92–104, 118–131, 177–178, 195–196; plan 65–66, 87–91, 102–116; tasks 20–21, 57, 68; REQ-C01–C03/F01/F03. Unity Editor/Mono per-ID capture is mandatory; .NET is usable only after reproducing its vectors. Raw float/double bits and identity/serialization policy precede capture. Independently reviewed parity/tolerance policy follows capture; C-2b explicitly does not block P1-01. Discrete branch/victim/plan outcomes remain exact. Source evidence below supports the runtime caution. | None |
| **R-2 — approval binding and split-run hashes** | **PARTIAL** | spec 68–82 correctly adds the owner C-1A choice, biological projection and Mode B; REQ-B04/E03 and tasks 73/76–77 preserve load staleness and negative cases. However spec 81/199, plan 119, tasks 75 and REQ-E06 at 71 do not agree on a defined Mode A comparison when load history differs. See delta D-1. | **MAJOR** |
| **R-3 — GitHub/GFM rendering** | **PASS** | All four exact candidate blobs rendered through GitHub `POST /markdown`, `mode=gfm`, `context=jermur788/CCF`. Working inline code, tables and the plan's single fenced text diagram; zero escaped backticks or literal backticks outside code. Counts in §5. Historical MULTI-011 handoff was not edited. | None |
| **R-4 — gate crosswalk** | **PARTIAL** | spec 211–218, plan 51–69, tasks 5–26 and requirements 119 consistently identify completed evidence G0, review P1-S0/G1, product/spec approval P1-G2/G2, then P1-01…05/G3…G7. **REQ-F09 at requirements 94 still says P1-G0.** See D-2. | **MINOR** |
| **R-5 — decision owner/timing** | **PASS** | spec 169–184 is the timing/owner table, referenced by plan 76–95, tasks 19–28 and requirements 119. C-1A/C-6 are owner-level; C-7 wording needs owner approval. C-2a is before capture; C-2b is after capture/before adapter acceptance; population/initial state/geometry before fixtures; SI/H before P1-04; format before persistence. Technical choices still need independent review and later approval. No C-item is accepted by this review. | None |
| **R-6 — historical schema23 verifier** | **PASS** | spec 53/117, plan 41/95, tasks 20/59 and REQ-F02 make the repair separate and conditional on reuse. Checked actual `Tools/Verification/SitkaGrowthMortality/SitkaGrowthModelVerification.cs:96–98`: policy assertion still uses version 23; `ForestSaveData.cs:7` says 24. A dedicated capture need not reuse that harness; no weakened check or new source repair is authorised. | None |
| **R-7 — capture configuration/causality** | **PASS** | spec 48/51 and 122–129; plan 95/112; tasks C06/C11/C12; REQ-C09/C12/F01. Valid regeneration configuration, no spawner recruitment, storm0, explicit ecological-year/seed/model mapping, Mature/Reference/no-pruning restriction and future configuration A/B are stated. Source inspection confirms year increment before roll, pre-growth CI for suppression and post-growth DBH/RD for hazard, clamps and deterministic victim ordering. **A/B remains a future test, not a current PASS.** | None |
| **R-8 — author cannot approve own specification** | **PASS** | spec 7 and 186–190; plan G2 at 64; tasks P1-G2 at 19; REQ-A07. Independent review/delta evidence and explicit product-owner approval are required. The GPT-6 author/Manager may coordinate but cannot make its own DRAFT executable. | None |
| **R-9 — actual P0 lineage/shared ignore** | **PASS** | spec 11/67, plan 35, tasks 21–22 and REQ-B07. Independently counted 0 `Tools/MultiRackSitkaLab` paths at CCF snapshot versus 52 at qualified P0 SHA. P0 `.gitignore:5–6` ignores `.lab-data/`, not proposed `.lab-p1/`. Exact P0/P1 base lineage must be chosen before P1-02; shared ignore ownership or an isolated alternative is explicit. No automatic merge into main. | None |
| **R-10 — typo, status meanings, square fixture** | **PASS** | Original spec blob `6e23feaf83503a3a6fd3fec57475f9a10580a788` at `50178f9d:126` really says `current-CFF`; revised spec 195 says `current-CCF`. Repair author's claim is supported, notwithstanding the older negative Manager assertion. spec 139–154 defines DRAFT versus OPEN. spec 110 correctly describes P0 `oracle-quarter-ha`: 50 m square, corner origin, three trees, possible re-centred input rather than qualified Unity evidence. Checked its actual P0 JSON. | None |

### Source grounding for the numerical/configuration PASS grades

These are **direct source observations/source-derived conclusions**, not new Unity results, at CCF `fc18190b`:

- `ForestEcologyController.cs:990–994` records anchor changes from moving the same Hegyi expression to a helper; 1022–1023 warns that Mono anchors depend on exact evaluation. `BrowsingProtectionVerification.cs:289–294` documents retained extra precision; `Docs/ScenarioTwoSpeciesFoundation.md:141–145` records the changed Mono path and restoration. Thus a C# `float` declaration alone cannot establish the captured runtime bits.
- `SimulationRandom.cs:41–54` hashes C# character values with integer operations and returns a 24-bit numerator divided by 2^24. That mortality roll is portable with the same input/model; this does **not** make CI, DBH, crown, RD, hazard or threshold comparisons portable. Actual roll key includes `"ADULT-MORT-" + tree.TreeId` at ecology 1103.
- Ecology 863–884 sorts ordinal IDs; 975–994 performs the slack pre-query and inclusive 8 m final check. Ecology 467–493 orders year increment, storm, competition, growth, crown, adult mortality, then regeneration. Ecology 1082–1104 uses post-growth RD and the existing pre-growth CI for suppression/roll comparison; 1114–1127 orders suppression descending, ID ties, and stops at RD≤1 or count≤1. `SitkaGrowthModel.cs:98–113` supplies the density/hazard guards.
- `ForestTree.cs:264–274` confirms stage-sensitive height; 623–634 confirms DBH `[1,200]`, height `[0.1,60]`, crown floor 0.1. Ecology 483–486 throws for regeneration2 without a manager; 1699–1703 skips promotion without a spawner. These support a future isolated capture configuration; they do not prove its neutrality without the required A/B.

The prior **94/153 and 981/1,893** differences remain two-semantics **scratch arithmetic**, neither semantics established as Unity output. I did not rerun that probe or promote the older nine formula vectors into runtime evidence.

## 3. Required narrow corrections

### D-1 / residual R-2 — MAJOR — Mode A still conflates raw history with an undefined projected hash

**Exact locations:** [spec §4, line 81](https://github.com/jermur788/CCF/blob/8756c452cf76a8dd3169f2ab40c93349cb1edc60/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md#L81), spec AC-P1-05 line 199; [plan §5, line 119](https://github.com/jermur788/CCF/blob/8756c452cf76a8dd3169f2ab40c93349cb1edc60/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md#L119); [tasks §4, line 75](https://github.com/jermur788/CCF/blob/8756c452cf76a8dd3169f2ab40c93349cb1edc60/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md#L75); [REQ-E06, line 71](https://github.com/jermur788/CCF/blob/8756c452cf76a8dd3169f2ab40c93349cb1edc60/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md#L71).

Spec 78 correctly limits raw hash equality to identical ordered histories. But Mode A at 81 says to compare the hash “after removing that one named load event”; the acceptance criterion and companions instead require identical canonical hashes/chains “allowing only for” that event. **These are not yet one defined, executable invariant.**

For a P1 history hash, `[annual-1, annual-2]` and `[annual-1, load, annual-2]` have different canonical history inputs. Deleting a record does not itself specify how to recompute a comparison: event sequence numbers, embedded previous/post hashes and any load-dependent snapshot fields also need an explicit projection/replay rule. An identity-preserving audit event can leave a **snapshot** hash unchanged while changing a **history** hash. The DRAFT must name which it compares.

There is a second source-derived counterexample within the same R-2 defect. “No open APPROVED/PENDING_WORK plan at the split” also permits an unapproved plan at the split, or new plans afterwards. If C-1A retains P0 semantics, a subsequent legal approval records different runtime session epochs in the continuous and restarted runs, even with **no load event**:

- P0 `LabSession.cs:69–70,83` creates a fresh random session epoch on load; 221–225 writes it into the approval payload.
- `CanonicalJson.cs:133–138,171–175` includes that epoch in the canonical plan snapshot and approval payload. `Engine.cs:194–211` requires the live epoch for approval/queue.
- Consequently, no open approval **at the split** is insufficient to ensure identical future canonical event bytes or whole-plan snapshot hashes after equivalent legal approval/work actions. This is a static trace of inspected P0 code, **not a newly run P0/P1 experiment**.

**Important P0 distinction:** the existing P0 SHA-256 is over canonical **snapshot bytes**, with events linking previous/post snapshot hashes (`CanonicalJson.cs:144–148`; `LabSession.cs:14–35,118–131`), rather than a digest over the entire event log. P0 can have audit-only events with identical pre/post snapshot hashes. That does not establish equal P1 event-history hashes and must not be used to justify the current Mode A wording.

**Smallest recommended docs-only repair:** replace all five Mode A statements together with the same rule:

> Mode A requires the stated per-ID biological, ecological-year, RNG-input/roll, mortality/felling and executed-work equivalence. Raw canonical history hashes are required to match only when the complete canonical histories, including payloads, sequence and session-dependent hashed data, are identical. If load records an event or session binding changes hashed approval data, retain and validate each raw history separately and make no raw-history-hash equality claim. Any additional equality over a normalized comparison is a separately named, versioned projection, defined and independently reviewed under C-1/C-1A before P1-03 acceptance.

This removes the unsupported equality promise without choosing the owner's load-event or approval policy. If the owner later wants projected hash equality, the approved definition must enumerate retained/excluded fields, session-epoch treatment, ordering/renumbering, reserialization/rehashing and allowed differences; retain the original audit chain. Do not accept “remove the event” as the whole definition. A restricted no-load-event/no-later-approval identity case can remain a positive test, explicitly narrower than general Mode A.

**Correction acceptance:** same wording across spec §4/AC-P1-05, plan §5, tasks §4 and REQ-E06; distinguish snapshot, raw-history and any projected hash. Include the named-load-event case and a subsequent approval after a Mode A split as counterexamples to unconditional raw identity. Keep Mode B's one `PlanStale(SessionLoaded)` per affected plan, legal-action comparison, biological invariance and missing/duplicate-stale negative cases. Do not persist approvals across load as a workaround or change P0. C-1A remains the owner's OPEN choice.

### D-2 / residual R-4 — MINOR — REQ-F09 retains obsolete P1-G0

**Exact location:** [requirements.md:94, REQ-F09](https://github.com/jermur788/CCF/blob/8756c452cf76a8dd3169f2ab40c93349cb1edc60/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md#L94): “no code before **P1-G0** SPEC/product approval.” The same file's line 119 correctly identifies **P1-G2 = G2**. The repair handoff's assertion that no `P1-G0` remains is therefore inaccurate; it is not verification evidence.

**Smallest repair:** change just this obsolete gate token from `P1-G0` to **`P1-G2`** in REQ-F09. Check all four DRAFT files for remaining `P1-G0` references. Preserve plan **G0 = completed evidence-plan review**, P1-S0/G1, P1-G2/G2 and P1-01…05/G3…G7. No new approval or historical-handoff rewrite is needed.

## 4. Earlier findings and owner gates remain intact

| Earlier finding | Preservation in repaired DRAFT |
|---|---|
| m-1 stage-sensitive height | spec 48/106 and REQ-C06/D02 retain Mature-equivalent restriction and refusal; no young-stage validation claim |
| m-2 fixed site productivity | spec 46/108 and REQ-C04/D04 retain site=1 as current-source behaviour, not empirical site-index choice |
| m-3 initial dead without cause/year | spec 88/108, REQ-D03 and tasks 67 retain synthetic pre-inventory mapping as OPEN C-4, with no new simulated death |
| m-4 mixed precision and serialization | spec 55–61/92–104, REQ-C01–C03 and tasks 103 preserve the concern, correctly superseding the binary32 guarantee with Unity capture and timed C-2a/C-2b |
| m-5 geometry/origin | spec 50/110, REQ-D05 and tasks C02 retain centred square parity versus separately labelled polygon extension |
| m-6 RNG distinct from growth | spec 51/87, REQ-C09 and tasks C06 keep separate model/seed/year/ID axes |
| m-7 113 volume calibration/page bases | spec 36/38/153, plan 132 and REQ-F06 retain volume-equation attribution and primary-page limitations; no new primary-paper validation |
| m-8 subset/density/SI fixture risks | spec 110–112, REQ-D01/D07/D08 and tasks C03/C05 retain all-living/actionable distinction, synthetic counts and near-onset fixture caveat; figures remain earlier reviewer arithmetic |

All **C-1…C-7 remain OPEN**, including C-1A and both C-2 parts. C-1A chooses fork/approval meaning; C-6 requires a separate owner decision on scarce exclusive Unity time; C-7 wording needs owner approval. Technical decisions require the later reviewed specification. C-2a precedes P1-01; C-2b cannot close until its Unity evidence exists and must precede P1-02 acceptance. Initial population/state/geometry precede fixtures; SI/H precedes P1-04; identity/persistence precedes P1-03; labels precede visible output. No existing choice is adopted here.

No lane is booked for a future capture. The owner can defer P1 and retain P0. The **complete Sitka/Oak/Beech Scenario One** remains the first external-demo priority; this isolated adult Sitka comparator cannot reduce it. Even an eventual **APPROVED AS DRAFT QUALITY** verdict would support further Manager/owner decisions only. An approved specification, explicit owner choices, owned work packets and physical Unity scheduling remain separate future gates.

## 5. Actual GitHub GFM verification and evidence limits

I submitted the exact candidate bytes to GitHub's Markdown API and parsed its returned HTML. This is actual GitHub/GFM rendering evidence, not a browser screenshot or an inference from repaired backticks.

| File | Rendered code elements | Literal backticks outside code | Escaped backticks in source | Preformatted blocks | Rendered table rows by table |
|---|---|---|---|---|---|
| spec.md | 98 | 0 | 0 | 0 | 7 / 9 / 9 / 11 / 5 |
| plan.md | 25 | 0 | 0 | **1**, expected multiline diagram preserved | 5 / 10 |
| tasks.md | 27 | 0 | 0 | 0 | 5 / 8 / 9 |
| requirements.md | 34 | 0 | 0 | 0 | 9 |

Every rendered table row count matches its source after excluding the Markdown delimiter. Requirements has **60 unchecked boxes, zero checked**. The renderer check did not exercise browser interaction or qualify any Unity visuals.

Durable off-repo rendering/readback receipts: `/media/jer/Files/CCF/Verification/MULTI-014-SitkaP1-DeltaReview-20261115/` contains exact input copies, returned `.gfm.html`, `gfm-results.json` with source/HTML SHA-256 values, and `candidate-readback.json`. No additional evidence file is published into Git.

| Evidence class | What this review establishes |
|---|---|
| **Directly observed** | Git/remote heads and repair diff, ownership preflight and claim registration, four full repaired files and earlier rulings, exact GitHub blob readback, GFM HTML and source text |
| **Source-derived** | P0 approval/session/hash counterexamples; Unity order/clamps/branch/RNG/configuration facts and restricted growth-model route |
| **Earlier scratch arithmetic only** | 94/153 and 981/1,893 float-semantics differences; synthetic fixture counts/RD and older formula probes. Not rerun here, not Unity output or Irish measurement |
| **Untested / not requalified** | Unity capture, configuration A/B, C-2b parity, adapter/save-load execution, P0 runtime/browser/security regressions, GROWFOR executable/forecast accuracy, Irish empirical or longitudinal validation |

Original Broad–Lynch/GROWFOR PDFs were **not inspected or requalified in this review**. The Manager's limited primary-page spot check remains attributed to that Manager, and older primary-source availability limitations remain. No broad research rerun, Growfor suite, P0 program/tests, C# compilation, Unity process, gameplay/build test or Windows game build was run. No ecological coefficients, scientific thresholds or test expectations were changed.

## 6. Handoff and remaining decision

**Only tracked output:** this newly added Markdown report. The four repaired DRAFTs, MULTI-013 handoff, original review, P0 source/spec/fixtures/saves, `Assets/**`, `Tools/**`, `Docs/Project/**`, live coordination, main and frozen Reference Future remain unchanged. Publication commit/readback, the exact **ONE ADDED Markdown / ZERO other changes** GitHub comparison against `8756c452cf76a8dd3169f2ab40c93349cb1edc60`, and HANDED-OFF closure receipts are recorded on [#542](https://github.com/jermur788/CCF/issues/542) after publication.

**Next bounded action:** Manager disposition and a docs-only repair of D-1/R-2 plus D-2/R-4, followed by focused independent recheck. No need to reopen the already-passing numerical repair or launch Unity to resolve these wording defects. The strongest outstanding product choice is **C-1A's approval/session/load-event policy**, with any normalized comparison explicitly defined before persistence acceptance; **C-6 scheduling and C-7 wording also remain owner decisions**. This reviewer chooses none of them.

**Dashboard/Drive: NOT VERIFIED.** No canonical coordination/mirror write is authorised in this task; the Manager must arrange any later operational synchronization. Claim handoff/closure is not product approval, integration or a claim that the published dashboard reflects this result.
