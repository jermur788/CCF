# Sen-1 #667 / #669 — Independent nonauthor review of the Compact30 world and CGTRIG/MAE save dossiers

**INDEPENDENT EVIDENCE-QUALITY REVIEW ONLY. NOT A CONTENT SIGNATURE, SAVE-CONTRACT SIGNATURE, SPEC APPROVAL, MANAGER ACCEPTANCE, INTEGRATION AUTHORISATION OR UNITY QUALIFICATION. UNITY NOT RUN.**

| Identity | Value |
| --- | --- |
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code session on T3 Code |
| Independence | Different model family from the author (OpenAI GPT-6.1 Sol, `gpt-6.1-sol`). Separate session. I wrote no part of either document and did not edit them. |
| Date | 2026-11-17 (UTC) |
| Review claim / parent / author claim | [#669](https://github.com/jermur788/CCF/issues/669) / [#667](https://github.com/jermur788/CCF/issues/667) / [#668](https://github.com/jermur788/CCF/issues/668) |
| Binding packet | `Sen-1-667-OPUS55-INDEPENDENT-REVIEW-DISPATCH-20261117.md` (local orchestration copy, orchestration HEAD `e5520091`) |
| Review branch / base | `review/sen-1-667-independent-gate-evidence-20261117-v1` / `85f2b35deb751d78e9486d4db0f68d8d14b5ea96` (checked out; matched) |
| Reviewed author branch / tip / base | `docs/sen-1-667-compact-world-save-evidence-20261117-v1` / **`d05207782bbb8274dbf73eeb383119aa0854adbe`** / `1e0fba5ffd5a07704eed3e44c34b3a960f61ac60` |
| Author diff (verified) | `git diff --stat 1e0fba5f d0520778`: exactly two added files, 699 insertions, nothing else; base is an ancestor of the tip; one commit |
| WORLD doc (`git show d0520778:…`) | `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md` — 342 lines — SHA-256 **`824c503c156741338670730ee3623270e9d658531e2e5ec27cf1145488208f3b`** — **MATCH** |
| SAVE doc (`git show d0520778:…`) | `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-CGTRIG-MAE-SAVE-EVIDENCE-DECISION-CANDIDATE-20261117.md` — 357 lines — SHA-256 **`791f243b3ff5d826e2df1b6da771c90368c9a8a33be1b4ab515a1892cf959f4f`** — **MATCH** |

Below, `W:n` means WORLD-doc line n and `S:n` means SAVE-doc line n, both at `d0520778`. All C# was read at the author base `1e0fba5f`. `git diff 500ef7ac 1e0fba5f -- Assets` is empty, which confirms the dossiers' static pin. The review-branch base `85f2b35d` also has no `Assets` diff from `1e0fba5f`.

## Verdict

# **APPROVED EVIDENCE QUALITY**

**Severity counts: CRITICAL 0 · MAJOR 0 · MINOR 2 · NOTE 3.**

The two documents are an accurate, source-faithful, internally consistent **SIGNATURE-NOT-READY** evidence package. They are ready for owner and save-owner decisions.

- Every quantitative world claim I recomputed reproduces exactly. That covers the hashes, the 189→175 membership, the H1 and buffer IDs, the 19 retained rack-labelled stems, 36-cell occupancy, basal-area and volume totals, route length, the nearest-stem margin, the G1 margins and the failed-shelter intrusion.
- Every C# claim I spot-checked matches source.
- The signed §4.2 quotes match commit `200add4e` verbatim.
- The deferred decisions stay deferred: M-A-SCOPE, A7, the admission policy and S1.

Neither MINOR finding changes a conclusion. Both are bounded corrections the author can make on its own branch. They are **recommended, not blocking**.

## Findings

| ID | Severity | Document line(s) | Source evidence | What is wrong | Required / recommended correction |
| --- | --- | --- | --- | --- | --- |
| F-01 | MINOR | S:96, S:186 | `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:48` (`NeedsAnnualReview => … !manager.AnnualReviewSeen`), `:462` (`if (NeedsAnnualReview) { ShowReview(); return false; }`); `Assets/ForestPrototype/UI/WorkPlanView.cs:167–169` (Advance disabled while `NeedsAnnualReview`; "Read first annual results to continue"); `ScenarioOneManager.cs:394` sets `annualReviewSeen=true`, and `:660` (scenario reset) is the only place that sets it false | The "current source" cell says the cited manager lines have "no unread-review gate". That is true of the manager API. But the cell leaves out that the **UI route already holds annual advance until the first annual review is acknowledged**. The flag is one-shot: it is never re-armed after the first acknowledgement, so it cannot serve as CT-3. The proposal and the S:144 / S:318 witnesses are still correct. The current-source description is incomplete, though, and the omitted UI hold is the nearest existing precedent for CT-3's "visible reason + open-review route". | Add one sentence or a row noting the existing UI-only, first-review-only, one-shot hold, with these citations. State that it is not CT-3, because `AdvanceYear()` and `CanAdvanceYearNow` have no gate and the hold never re-arms after the first acknowledgement. No count changes. |
| F-02 | MINOR | W:143–156 (with legend W:27; count W:342; mirrored S:349) | `ForestSaveData.cs:191–210`; `ForestTree.cs:32–34, 72–75` | Rows with the **same kind of evidence** get different classes. Each is a field absent from CB/ANN where the candidate is the fresh source/DTO default. Rows 150 (`stageTimer`, candidate 0), 151 (`chopProgress`, 0) and 154 (`pruningLifts`, unpruned) are labelled UNSUPPORTED. Rows 147–149 (mortality defaults), 153 (`equivalentSuppressedYears`, 0) and 156 (`lastPruningYear`, −1) are labelled GAME-CALIBRATION-PROPOSAL. Row 145 (suppression class, UNSUPPORTED) and row 153 (its persisted field, GAME-CALIBRATION-PROPOSAL) also disagree. The legend at W:27 says a suggested default does not upgrade an UNSUPPORTED row. The reported "36 unsupported" therefore rests on an unstated split rule. No value is invented, and every row still names W03. | Either state the rule that separates the two groups, or apply the legend uniformly and update W:342 / S:349 if the count changes. Re-check only the changed rows and the count lines. |
| F-03 | NOTE | S:194 | `ScenarioOneCompletionRules.cs:62–68` (`ShouldRecordAcknowledgement`), called at `ScenarioOneManager.cs:395`; `ScenarioOneManager.cs:2394–2400` is `EvaluateProgress` → `HasReviewedTwoCycleSequence` | The duplicate-event9 suppression helper is CR:62–68 (inside the cited CR:36–68), not M:2394–2400. The cited M range covers only the historical completion call. | Optional: cite M:395 / CR:62–68 for the helper. |
| F-04 | NOTE | S:195 | `ForestSaveValidation.cs:365–375` (planted-juvenile and open-order species checks), `:382–393` (objective-history header) | The VAL:365–393 citation for "raw/typed validation flow" points to two narrow validators rather than the flow. The flow is SC:261–274 / 324–340, which the row already cites. | Optional precision: name what VAL:365–393 shows, or drop it. |
| F-05 | NOTE | S:113, S:144, S:318 | `git grep -i skip` over `Assets/ForestPrototype/ScenarioOne` and `UI` at `1e0fba5f` finds only LINQ `.Skip()`; there is no multi-year skip route | "Annual advance/skip" and "N-year skip attempted" read as though a skip route exists today. It does not exist at base. | Optional: say "any future skip route", so a witness is not written against a non-existent API. |

**Checked for and not found:**

- No fabricated or contradicted facts and no invented values.
- No deferred policy decided by the author.
- No signature drift: "APPROVED/SIGNED/satisfies" appear only as negations, quoted status, or truth-table predicates, at S:3, S:44, S:135, S:137, S:160 and S:260.
- No new save key, sidecar, root or schema change smuggled in.
- No claim that current code captures the order↔juvenile pairing.

## What I verified and how

**Read-only commands:** `git fetch`, `git show <commit>:<path> | cat -n`, `git grep`, `git diff --stat`, `git rev-parse`, `sha256sum`, `gh api` (one contents lookup), and one review-written Python script run with `python3 -I -B`. The script did only JSON reads, hashing and arithmetic, and lives outside the repo in session scratch. No archived harness or #508 script was executed.

### 1. Source fidelity — all confirmed at `1e0fba5f`

- **`ScenarioOneWorkOrder.cs:29–72`:** no `juvenileId`. `ScenarioWorkStatus` is Pending/Approved/Completed/Failed (`:13–19`). `IsOpen` includes Approved (`:72`). Manual orders keep the prescription fields default (`:60–70`).
- **`ForestSaveData.cs:7`:** v24. `PlantedJuvenileSaveData` (`:215–228`) has no `workOrderId`. The TreeSaveData fields at `:185–212` match every SD line cited in the W matrix.
- **`ScenarioOneManager.cs`:**
  - `ResolvePlanting` (`:1861–1924`) is `private void`. It checks offer/species/cell (`:1866–1871`), then stock/quote (`:1873–1877`); the legacy branch calls `TryPlantJuvenile` (`:1879–1889`).
  - It then deducts stock (`:1890–1891`), applies clearance (`:1895`), and constructs the juvenile inline as `"PJ"+nextJuvenileId++` (`:1896–1907`), with the shelter taken from the list tail (`:1908–1914`).
  - It then does `checked` cash and minutes (`:1917–1918`) and sets Completed/resolvedYear (`:1920–1921`). Nothing returns the juvenile identity.
  - `WorkResolved` is recorded after resolve (`:1313–1321`). `ScenarioManagementEvent.cs:40–70` has `workOrderId` and no `juvenileId`.
  - `MarkAnnualReviewSeen` records event9 with `year = CurrentYear` (`:391–401`).
  - `CanAdvanceYearNow` (`:324–332`) and `AdvanceYear` (`:1253–1290`) have no unread-review gate. `report.year = ecology.EcologicalYear + 1` (`:1280`), and Approved orders are taken in ascending order (`:1267–1270`).
  - Other cited ranges match: the 0.7 m and 0.5 m guards (`:135`, `:171–174`), `SetPlantingExecution`/`SetPlantingShelters` demoting open/Approved orders to Pending (`:875–890`), cancellation records `OrderCancelled` (`:1185–1218`), `ApprovePendingWork` (`:1221–1250`), the juvenile counter and event counter re-derivation (`:1529–1534`), and the juvenile restore with `promotedTreeId` (`:1514–1521`).
- **`ScenarioOneCompletionRules.cs:36–68`:** historical two-cycle rule and the acknowledgement idempotence helper.
- **`UI/AnnualReviewView.cs:65–66`:** the callback guard `if (!showingEarlierYear)`, set at `:108`.
- **`UI/ScenarioOneUiRoot.cs:438–444`:** acknowledgement requires `screen == UiScreen.Review` and reports > 0. See F-01 for the omitted `:462` hold.
- **`ForestEcologyController.cs:912`:** `HegyiCutoffMeters = 8f`. `HegyiTerm` at `:920–926`. `competitionIndex` at `:52` and `:932`.
- **`ScenarioProtectionVisuals.cs`:** `:11–12` gives 1.2 m / r 0.10. `:81–83` destroys the collider. `:91–95` applies the failed pose: Euler Z 28°, +0.25 X, −0.1 Y. Expired is colour-only (`:99–100`).
- **`ForestSaveValidation.Geometry.cs:10–41`:** an unknown geometry model, pre-v20 nonzero geometry, and a missing or non-integer v20 geometry all reject.
- **`ForestSaveController.cs`:** single historical filename (`:9–19`). Disk raw validators at `:261–274`. Typed `LoadData` at `:324–340`. Capture writes `hasSimulation = true` (`:146`).
- **Other cited literals match:**
  - `ForestTreeSpawner.cs:145–148`: identity rotation.
  - `ForestStartingStand.cs:113–116`: y = 0.
  - `ForestPlayer.cs:4`: CharacterController. `:17`: 3.5 m reach.
  - `ForestTreeMarkingManager.cs:10`: 8 m marking reach.
  - `ScenarioOneDefinition.cs:58`: browse 0.2.
  - `ForestTree.cs` enums and defaults.
- **Decision log and specs:** decision-log D-079/081/082/083/084 at `:139/143/146/149/151`. Specs P:24–31 / 65–68 / 96 / 137–150, C:15/17/19/100/108, G:20/68–69/147/149/151 (GQ-07 says "preferred DRAFT architecture, not approved", which W:308 reports faithfully), and MAN:19/35/80.

### 2. Signed save boundary

- `200add4e:specs/scenario-independent-save-isolation-v1/spec.md` resolves to blob `8ac551022adb578abed0d9fbd413c8e9d002c3f9`. That equals SIG:22 on `origin/docs/scen1-manager-save-v2-immutable-approval-20261115-v1`, and SIG:13 binds the same commit.
- Every quoted fragment in S:48–82 matches SAVE:155/159/160/161/163/165/167 word for word, including the root keys, the seven envelope keys, schema1/payload24, the neutral rack fields, "Compact geometry explicitly2", the four row keys, "cancellation is an event, not a new ScenarioWorkStatus value", "first actual annual review covering completion of all four", and "A false acknowledgement … simply remains incomplete".
- The SAVE:148 registry literals, SI-10 (SAVE:182) and FR-01…FR-07 (SAVE:188–210) are cited accurately.
- S:100, S:208 and S:239 keep S1 and any pairing field outside the closed three-root/v24 graph. Each requires a separately signed revision.

### 3. Roster and provenance — independently recomputed

- **Annex text.** The annex at `55f79067` is blob `1ecf429cc7d759146d8481bd7c2567b3a4bbc75d`, 32346 bytes, file SHA-256 `10bbc446…98bd`. `gh api …/contents/…?ref=55f79067` returns the **same blob SHA and size from GitHub**, so the local object is the canonical GitHub text.
- **175-row roster.** §5's first `text` fence is lines 89–264: 9481 bytes, 175 rows, no BOM/CR, eight columns, IDs strictly ascending and unique. **SHA-256 `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` — MATCH.** I independently generated it from raw C-B-30 minus H1, using the stated formatting. The result is **byte-equal** to the fence.
- **189-row reference.** Generated from C-B-30 with the annex's source189 header and label column: 11359 bytes, **`e649b2aac5df85230b71372e0977cbf723990f1aabb5881a3914855575cccfbe` — MATCH.**
- **Raw byte hashes on this host:**
  - `…/native-20261113T152528Z/C-B-30.json`: 128518 B, **`3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` — MATCH.** Ledger `/inputs/C-B-30.json` equals it.
  - `…/scen1-508-EntrancePathPlanting-20261114/ledger.json`: 24746 B, **`aa10f54f2fac9ccfa23d11a450f29985c39bf966944c665cd822b0ef49b0bcf3` — MATCH.**
  - `sha256sum -c SHA256SUMS` in the #508 folder: **9/9 OK** (ledger, omissions-2.0, seven scripts). No script was executed.
  - `evidence-manifest.json` `cd35e159…`, `harness-provenance.json` `99a6484e…` and `executed-harness.cs` `3c0ebae8…` all match W:52–54. `SHA256SUMS` itself hashes to `f86940c9…`, which matches W:56.
  - The harness-provenance "not recompiled" caveat is reported accurately at W:68.
- **Absent path.** `…/native-20261113T152528Z/ledger.json` does not exist (`ls`: No such file). The documents report it as absent and claim no hash for it (W:66, S:351). That is correct.
- **C-B-30 metadata.**
  - size 30, lattice 16, target 189, seed 20260914, cells 36, rackZ −0.25, rackWidth `2.4000000953674316`.
  - 189 unique Sitka trees, all age 20, labelled north 99 / south 71 / rack 19.
  - Totals BA 3.6501191814 m² and volume 22.2125049420 m³.
  - 113 anchors: 104 at r 1.5, 9 at r 2.2.
  - Track: 52 planks, 27.5202551894 m.
  - The harness labels `InRack` at line 151 and `side` at line 162, after generation.
- **H1 and buffer.** Inside H1 there are **14** trees: P0004 P0005 P0006 P0207 P0306 P0307 P0403 P0404 P0405 P0406 P0407 P0503 P0505 P0507. All are south-labelled, and the set equals ledger `/H1/inside`. The **8** buffer IDs are P0003 P0009 P0103 P0109 P0209 P0504 P0506 P0508, equal to ledger `/H1/buffer1m`.
- **Retained stand.** 175 trees remain. **19** rack-labelled stems are retained, and their IDs and roster-precision x/z match W:100–118 (spot-checked: P0700, P0707, P0710, P0715, P0800, P0806, P0815). Totals are BA 3.3540142137 m² and volume 20.3413430341 m³.
- **Cell occupancy.** The floor-grid occupancy vector matches W:227–262 cell by cell. **Sum = 175.** No tree falls outside cells 0–35. Rounded roster coordinates give the same vector.
- **Route and margins.**
  - Eight-anchor route length: **16.6719614516 m**. Ledger `lengthM`: 16.6857879905.
  - Nearest retained stem is **P1106 at 1.6004984013 m**, giving a trunk-edge margin of **+0.9194443718 m**.
  - G1 sites all fall in cell 15:

    | Site | Route distance (m) | Margin to core (m) | Margin to core + verge (m) |
    | --- | ---: | ---: | ---: |
    | O1 | 1.057755 | +0.358 | +0.158 |
    | O2 | 1.262838 | +0.563 | +0.363 |
    | B1 | 1.222835 | +0.523 | +0.323 |
    | B2 | 1.020976 | +0.321 | +0.1209757 |

  - My own failed-pose model of the O1 envelope uses end-cap circles at 10,000 angles. Because the plan hull's extreme points lie on the caps, this sampling is sufficient. It gives a minimum of **0.4586424 m**, an **intrusion of 0.1413576 m**. W:288 labels this MODEL-DERIVED, not a collision witness, and correctly cites the destroyed collider.
- **Roster-digest non-claims.** W:78 and W:213 correctly state what the roster digest does not prove, and that no whole-world hash exists.

### 4. Field matrix coverage

- **Counts:** 88 rows (W:131–218). By class: SOURCE-VERIFIED 21, MODEL-DERIVED 5, OWNER CHOICE 17, GAME-CALIBRATION-PROPOSAL 9, **UNSUPPORTED 36**. The cell table has 36 rows, all UNSUPPORTED for ecology. 36 + 36 = **72**. W01–W14 and S01–S08 give **22** register entries. All match W:342 and S:349.
- **Required field groups — all present:**
  - **Tree:** identity, species, x/z, y, age, DBH, height, crown, prefab/LOD, yaw, stage, status, suppression, mark, mortality, plus the remaining TreeSaveData fields.
  - **Terrain:** height, normal, surface, bounds, collider.
  - **Cells:** 36 cells with flora, bramble/bracken, seed, browse, deadwood, canopy, coordinate rule and site drivers.
  - **H1:** surfaces, spawn, visitor/parking, collection, geometry, buffer, exclusions.
  - **Path:** eight anchors, spline/interpolation, projection, width/collider, verge, off-path refusal.
  - **North end:** A7.
  - **G1:** four sites, cell association, composition, tolerance.
  - **Shelter:** absent, effective, expired and failed states, including the r ≤ 0.10 m failed footprint.
  - **Player:** collider, 1.2 m core, 0.20 m verge, spawn, reach.
  - **Serialization:** contentKey, version/label, whole-world recipe, registry, historical refusal, model selectors, initial objects.
- No group is silently dropped. Apart from the F-02 split, I found no proposal labelled as verified and no unsupported value without a named gap.

### 5–8. Deferred decisions, labels and save-owner readiness

- **M-A-SCOPE** is DEFERRED/UNDECIDED (S:4, S:222, S:275–282, S:299, S:345; W:331). It is carried only as the unbound symbol `ScopeEligible`. No quota, no presumed one-stem eligibility, no threshold, and no CI/light improvement number is invented. P:148's historical "yes" is marked superseded.
- **A7** offers two GAME-CALIBRATION-PROPOSAL alternatives with "neither chosen" (W:194, W:264–269, W:305).
- **Admission policy.** Both alternatives, A (refuse) and B (admit incomplete), are covered across threat model, back-compatibility, migration, player experience, security limit and consistency with the signed text (S:154–163). The recommendation of B is explicitly "to the save owner, not a decision" (S:165). Both ruling texts are UNSIGNED (S:175).
- **Pairing.** "No current code path captures/persists the order↔juvenile pairing" (S:190, S:208). The atomicity caveat (S:204) matches the stock → juvenile → checked-cash order in source.
- **W-A.** The demotion of Approved orders by the planting setters and the per-tree requirements are reported accurately (S:214–218; source `:875–890`, `WorkPlanView.cs:450–461`).
- **S1 and S2.** Every S1 element is UNSIGNED and constrained by the closed root (S:239–267). S2's unsupportability is reasoned from the actual absences in WO/EV/SD/SNAP (S:271).
- **Hegyi cutoff.** The 8 m value is treated as a model calibration constant (S:235).
- **Status markers.** Spec families DRAFT, roster UNSIGNED, ISO-58/CG-TRIG BLOCKED, #652 HELD (W:336, S:357).

## What I could not verify, and why

- **GitHub issue-thread statements.** The #668 pre-edit receipt, live coordination blob semantics, the #543/#640/#425/#544 authority routing (S:286), the #647 fixture status `209a08e9` on an unmerged branch (S:288), and the #660/#663/#665 contents (W:279, W:332). I did not read those threads. The commits and blob IDs `a5541b8a` and `c0edc881` exist in the store.
- **The author's UTC read times** (W:51–64): unverifiable by construction.
- **ANN:35's 37/30 lattice-slot split.** The document itself labels it AUTHOR-REPORTED.
- **Runtime behaviour of any kind.** No Unity, Editor, compiler, test, harness, Player, save/load, Ops or lease was used. Static reading cannot establish exception atomicity, UI provenance, rendering or native geometry.

## What this verdict means and does not mean

**APPROVED EVIDENCE QUALITY means** that, within the scope above, the two dossiers at `d0520778` are a source-faithful, arithmetically correct and honestly labelled SIGNATURE-NOT-READY evidence package. The authorities can rely on it to make the listed decisions.

**It does not:**

- sign the Compact30 world, the roster `dac06a3b…` or any content key;
- sign, amend or reinterpret the save contract at `200add4e`;
- approve any DRAFT spec, CG-TRIG, admission policy A/B, S1, A7, W12 or M-A-SCOPE;
- release ISO-58 or the #652 deployment hold;
- authorise Unity, Ops, a lease, a source writer or a main integration;
- qualify anything natively.

F-01 and F-02 remain open recommended corrections.

## REVIEW INDEPENDENCE LIMITATION

The author's session and this review session run on the **same host** (`jer-laptop`) and use the **same project store** (`/media/jer/Files/CCF/Repository.git` and the Files CCF tree). Independence here means only:

- a different model family (Anthropic vs OpenAI);
- a separate session;
- no authorship overlap;
- reading only the pushed immutable artifact `d0520778` plus primary sources.

It does **not** mean separate hardware or separate organisational custody of evidence. The raw-byte checks re-read the same files the author read. They show the files are unchanged against the recorded hashes, not that the files were sourced independently.

## Bounded correction list (same author, own branch; changed lines only)

These are optional for this verdict and recommended before the save owner reads S:96.

1. **F-01:** S:96 (and S:186 if useful). Add the UI-only, one-shot first-review advance hold (`ScenarioOneUiRoot.cs:48/462`, `WorkPlanView.cs:167–169`, `ScenarioOneManager.cs:394/660`), and state that it is not CT-3.
2. **F-02:** W:143–156. State the class rule, or apply the W:27 legend uniformly. Update W:342 / S:349 only if the count changes.
3. **F-03 to F-05 (NOTEs):** citation and wording precision at S:194, S:195, S:113/144/318.

A re-check should cover only those lines and any changed counts. No whole rerun is needed.
