# Sen-1 #676 — Independent W2 review of the Compact30 whole-world manifest candidate

**W2 INDEPENDENT REVIEW. DOCUMENTATION AND STATIC EVIDENCE ONLY. NOT AN OWNER SIGNATURE. NOT UNITY-VERIFIED.**

**Verdict: APPROVED** for document/evidence quality — CRITICAL 0 / MAJOR 0 / MINOR 3 / NOTE 6.
**Content status: SIGNATURE-BLOCKED.** Eight field rows stay BLOCKED (10, 12, 30, 44, 73, 74, 75, 83), `world_digest` is null, and the owner-signature section is unfilled.

This approval is not the owner's content signature. It is not spec approval or save-owner approval. It is not Unity or native qualification, and it is not integration, a build or a release.

## 1. Identity, independence and scope

| Item | Actual value |
| --- | --- |
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), provider instance `claudeAgent`, effort high; T3 Code / Claude Code harness |
| Host | `jer-laptop` (Linux). This is the same host and the same shared bare Git store (`/media/jer/Files/CCF/Repository.git`) as the author lane. Isolation is by worktree and session, not by machine. |
| Checkout | `/home/jer/.t3/worktrees/CCF-Primary/t3-11df020c-c0d05cb9adf62b91` (clean at start) |
| Local branch → remote branch | `t3/world-manifest-review` → `review/sen-1-676-world-manifest-independent-20261117-v1`, pushed by refspec. The same-named *local* branch is checked out, with no commits, in the failed first-attempt worktree `t3-65f12c98…`. I did not touch that worktree or the other failed-attempt worktrees (`t3-f9f6d97f…`, `t3-9b06e717…`). |
| Base HEAD | `12711c99957f6b70b8e0da7890be17251b030f96` = `origin/main` at fetch. It moved from the author base `ab091116` through docs/coordination-only commits `d435292b`, `5398ca21`, `e38e1cf3`, `a9fb2ca7` and `12711c99`. `git diff ab091116..12711c99 -- Assets` is empty. |
| Prospective claim | [#676 comment 6955878013](https://github.com/jermur788/CCF/issues/676#issuecomment-6955878013), posted before review |
| Sole writable file | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-676-INDEPENDENT-WHOLE-WORLD-CONTENT-REVIEW-20261117.md` (this new file) |
| Binding dispatch | `d3a28dab…` `Sen-1-675-DEEPSEEK-COMPACT30-WHOLE-WORLD-MANIFEST-CLOSURE-ORCHESTRATOR-PACKET-20261117.md`, read in full; §5 is the review checklist |

**Independence statement.** I am not the author of the #676 target. Its author is OpenAI `gpt-6.1-sol` / `codex`, a different model family. I did not author the #675 dispatch, the #676 orchestration ledger or the #672 Stage-W2 review. I will not correct the target.

**Disclosure — a correction to my prospective claim.** My claim comment said I did not author "the #672 dossier/correction". That is true of this session, but it is incomplete for the model family:

- The #672 baseline dossier (`3082b7d7`, corrected at `aab606ca`) carries `Co-Authored-By: Claude Opus 5.5`.
- So does the #657 roster annex (ANN, `941b2f14`, read at `1e0fba5f`).

Both are evidence inputs, not the target. My independence for the #676 target is unaffected. But my checks of the target's "before" counts and its ANN serialisation compare it against Claude-family baselines. I mitigated this as follows:

- I regenerated the roster from raw C-B-30 with my own code.
- I recounted "before" directly from the #672 matrix table. That table was independently reviewed by GPT-6.1 Sol (#672 W2, APPROVED delta at `ac332a8d`).
- I cross-checked the ecology formula against an archived native Unity witness (§5.5).

The #676 handoff comment repeats this correction.

**Frozen target (not modified).** Branch `docs/sen-1-676-world-manifest-author-20261117-v1`, commit `8617587ebb7e118183bdd3eec7de5b099c371fdd`, single parent `ab09111681bf1acce3823d6fe3343642a851f3d9`.

**Exclusions.** No production source, C#, scene, prefab, asset, material or `.meta` was changed. No save/schema, historical DTO, spec, canonical doc, live board, CCF-Ops, Drive or registry was touched. Nothing was written to main or an integration branch. I took no Unity, Editor, compiler, test, build, Ops, lease or Windows action, and installed no package or LFS. No archived script or harness was executed: I only read `executed-harness.cs` as text. ISO-58 stays BLOCKED, #652 stays HELD, the Compact specs stay DRAFT and the signed save contract stays immutable. OD-16 stays deferred.

**Preserved unrelated work.** My checkout was clean. I edited no other worktree or branch.

## 2. Methodology and exact commands

Everything below ran read-only against Git objects and raw evidence, except the one new report file. My scripts lived in my session scratchpad (outside the repository) and ran with `python3 -I -B`. One C program was compiled with `gcc -O0 -ffp-contract=off -std=c11` for true binary32 arithmetic.

```bash
pwd; git status; git fetch origin; git rev-parse origin/main             # 12711c99…
git switch -C t3/world-manifest-review origin/main
gh issue comment 676 --body-file <scratch>/claim.md                       # 6955878013
git diff --name-status ab0911168… 8617587e…                               # exactly 3 × A
git show 8617587e:<path> > <scratch>/t/<file>; wc -c; sha256sum; git rev-parse 8617587e:<path>
git diff afd5484f..ab091116 -- Assets ; git diff ab091116..origin/main -- Assets   # both empty
(cd $E && sha256sum -c SHA256SUMS)                                        # 9/9 OK
python3 -I -B: N/evidence-manifest.json member sweep                      # 61/62; closure-receipt only mismatch (F-03)
gh api repos/jermur788/CCF/issues/comments/<id>  (13 brief-supplied ids)  # 1 OK, 12 × HTTP 404
gh api --paginate repos/jermur788/CCF/issues/672/comments                 # 22 comments; receipts located
git show ab091116:<23 source files + ForestTest.unity + SitkaSpruce.asset + ScenarioOne.asset>
git show 1e0fba5f:<ANN>, aab606ca:<#672>, 200add4e:<save spec>, d3a28dab:<dispatch>
python3 -I -B roster.py | strict.py | deps.py | cmpcells.py | geom.py | recount.py
gcc … cells.c && ./cells roster.csv ; ./cells double roster.csv ; ./cells raw189.csv
```

## 3. Findings

Severity scale: CRITICAL / MAJOR / MINOR / NOTE. No CRITICAL or MAJOR defect survived verification. All three MINORs are precision or traceability gaps in content the owner would later attest at exact bytes. None is a false claim, and none changes a status count.

### F-1 — MINOR — The binary32 per-operation arithmetic policy is contradicted by archived native evidence

**Where:**

- C `/numeric_policy/arithmetic`: "IEEE754 binary32 rounding after every scalar source add/sub/mul/div and sqrt".
- Dossier §5, paragraph "Values below are computed expected readback…".
- All 36 `C /cells/*/expected_readback` canopy, light, flora and competitor values.

**Evidence.** The archived native run `N/C-B-30.json` `/states/0` (year 0, 189 living trees, 30 m / 36 cells) records `meanCanopy` 0.9798264503479004 and `meanLight` 0.0201735682785511. The harness reduces cells with LINQ `Average` (`executed-harness.cs:202`), which accumulates floats in double and casts the result to float. I recomputed the 36 cells from the raw 189 trees three ways:

| Arithmetic model | meanCanopy | meanLight | Matches native? |
| --- | ---: | ---: | --- |
| Strict binary32 after every operation (author's model; also my C program) | 0.9798264503479004 | 0.020173566415905952 | canopy yes, light **no (−1 ulp)** |
| Double within each statement, rounded at float stores (Mono-style) | 0.9798264503479004 | 0.0201735682785511 | **both exact** |
| Fully double, rounded at the end | 0.9798264503479004 | 0.0201735682785511 | **both exact** |

So the installed Editor runtime evidently evaluates these expressions with wider intermediates.

**Effect on the 175-tree candidate:**

- Under the native-matching model, **cells 7, 8 and 15** (15 is the G1 cell) differ from the published canopy/light by exactly 1 ulp (−5.96e-8).
- Flora proxies differ by up to 2.4e-7, and competitor covers by up to 1.5e-8.
- All of these are well inside the author's proposed native tolerance of 2e-6.

The author is honest about this: "native bitwise parity NOT VERIFIED" and "source math may still differ across Editor/compiler configurations". But the author did not reconcile the stated model against the available native witness.

**Correction requested.** State that the archived native year-0 means match wider-intermediate evaluation and not strict per-operation binary32. Mark the published values as tolerance-bound expected readback, not the exact bytes a native readback will reproduce. Optionally record the 1-ulp alternates for cells 7, 8 and 15. Do not change the 2e-6 tolerance policy.

**Positive corroboration from the same evidence.** The canopy formula itself, with its constants (reach = crown·1 + 2.5, vertical = height/8, product of gaps) and ordinal tree order, reproduces the native year-0 mean canopy exactly.

### F-2 — MINOR — The render-dependency blocker is attached to the wrong rows

**Where:**

- M `/signature_blockers/2` (B-W13-RENDER-DEPS): `field_rows` [30, 44, 73, 74, 75].
- M `/field_closure/9` and `/field_closure/11` (rows 10 and 12): `blocker_ids` list only B-W02-RIGHTS and B-W02-SCALE.
- Dossier §11.2.

**Evidence.**

- All 22 unresolved references in M `/visual_assignment/unresolved_asset_guids` come from the **11 tree-visual materials**. Examples: `Bark.mat`, `Sitka_Needles.mat`, `Sitka_Pole_Bark.mat`, `SS_Benchmark_*`, `Leaves.mat`.
- In `Bark.mat` at the base, `933532a4fcc9baf4fa0491de14d08ed7` is the `m_Shader` reference (line 24) and `d0353a89b1f911e48b9e16bdc9f2e058` is an `m_Script` reference (line 12).
- The new terrain, H1 and guard materials name the shader `Universal Render Pipeline/Lit` by name.

So the unresolved shader bytes block the tree visuals (rows 10 and 12) at least as directly as rows 30/44/73–75. The blocker text also calls these "material GUIDs", when they are a shader GUID and a script GUID referenced *by* materials.

**Impact.** No status change: rows 10 and 12 are already BLOCKED. But the remaining-blocker map that the owner page shows is incomplete.

**Correction requested.** Add B-W13-RENDER-DEPS to rows 10 and 12 (or state explicitly why it does not apply), and correct the GUID wording.

### F-3 — MINOR — Path curvature is not characterised; the tightest bend radius is 0.625 m at A7

**Where:** M `/path/interpolation`, M `/path/construction`, dossier §6.

**Evidence.** From the pinned coefficients for segment A6→A7 (h = 0.5 m, A = 0.084454…, B = −0.053454…):

- d²x/dz² runs from −0.43 m⁻¹ at A6 to **+1.599 m⁻¹ at A7**. This is an S-inflection within 0.5 m.
- Segment A7→A8 starts at +0.336 m⁻¹, so curvature is discontinuous at A7 (the curve is C1, not C2).
- The minimum radius of curvature of the centreline is **0.625 m**. That is barely above the 0.60 m core half-width and below the 0.80 m outer verge half-width.

The cause is the mean-of-secants tangent rule applied across very unequal segment lengths (2.5 m, then 0.5 m, then 2.75 m).

**Impact.** No safety claim fails. The core and verge are defined by a distance field, so nothing self-intersects. Every tree, G1 and guard margin holds (§5.6). But OD-08 B asked for a "smooth, natural-looking" path, and the dossier reports only "C1 smooth" and the 0.094 m deviation bound. On the inner side of A7 the visual verge will narrow or merge.

**Correction requested.** Report the curvature profile and the minimum radius with its location. Either present it to the owner as an explicit author proposal, or revise the tangent rule (for example, weight the tangents by segment length) and recompute the margins. No change to the anchors or sites is needed.

### N-1 — NOTE — The class rule has drifted from #672 §0.3 without a step column

- The dossier redefines OC as "accepted owner direction (including these actual later receipts)". In #672's C2 step, OC required D-083/D-084.
- Row 72 is reclassified OC→GCP although D-084 still states the absent-state rule, which would make it OC by first match.
- Row 88 is reclassified GCP→OC.
- Now that D-085 is on main (`d435292b`), several SV rows backed by OD-03 (13–26) would classify OC under the #672 rule.

None of this changes a status. Adding a per-row "step fired" column, as #672 did, would make future recounts mechanical.

### N-2 — NOTE — The owner-receipt IDs in my brief were wrong; the author's IDs are correct

- All 12 Round-2 IDs supplied in my brief (6949518464 … 6949605301) return HTTP 404. Only 6949218167 (Round 1) resolves.
- The author found the real receipts. I fetched all 13 (6949218167, 6949441218, 6949464424, 6949490928, 6949553913, 6949590935, 6949647130, 6949669482, 6949697546, 6949754608, 6949817726, 6950307529, 6950337240).
- Every body is **byte-equal** to the body embedded in M `/owner_decision_receipts`, and each recorded `body_utf8_sha256` matches the live body.
- None of the comments has been edited (`updated_at` equals `created_at`).

This is an erratum for the Manager and brief, not a defect in the target.

### N-3 — NOTE — Future hazard: `ScenarioSiteProfiles` is keyed on the shared chapter ID

`ScenarioSiteProfiles.TryGet` returns `ScenarioOneReference` for `"scenario-one"` (`ScenarioSiteProfiles.cs:30–40`). Compact's `chapterId` is also `scenario-one` (M `/identity/chapterId`).

The class has no callers at the base, and all Reference responses are 1.0, so there is no numeric effect today. But if a future activation keys on chapter ID, it would silently attach Reference to Compact, which OD-04 A forbids. The manifest says "Reference NOT applied" but does not name this collision. A future source binding should key on `profileId`.

### N-4 — NOTE — The OD-02 yaw trade-off is not shown to the owner as an explicit choice

Row 11 pins yaw = 0° for all 175 trees. Together with two variants per height band (`/visual_assignment`), this is a defensible reading of OD-02.

But the OD-02 receipt named "yaw/scale or supported visual variation rules" as permissible variation channels, and yaw is not biological state. The rationale given ("no invented yaw RNG") treats yaw like biology. The owner page should list this as an explicit author proposal (uniform orientation, repetition risk) rather than leaving it implied.

### N-5 — NOTE — Reproducibility depends on the exemplar key order and on files outside Git

- `/canonical_bytes/json` defines key order as "exactly this candidate file recursively". That is fine for attesting these exact bytes, but a corrected re-issue needs an explicit key schema so a third party can regenerate it.
- Fourteen dependencies are absolute paths on the Files volume (`kind: raw_local_file`), not Git objects.

The attestation page should name both conditions.

### N-6 — NOTE — Browse pressure 0.2 comes from a field initializer

The existing `ScenarioOne.asset` does not serialise `backgroundBrowsePressure`. Its value of 0.2 comes from the field initializer at `ScenarioOneDefinition.cs:58`. The author's citation is correct. A future native readback should confirm the effective value for the Compact definition asset.

### Carried bounded issues (unchanged)

- **D-N01:** the #672 correction summary at line 17 says "§8" for wording that is in §7.1. It is carried and was not rewritten.
- **F-03:** my own sweep of `N/evidence-manifest.json` gives 61/62 matches. `closure-receipt.json` is expected to hash to `db414df7…` but currently hashes to `e715ff7e…`. It is not a calculation input. The parent `evidence-manifest.json` matches 17/17. Nothing was repaired.

Neither issue is silently fixed, and neither is used to justify any other change.

## 4. Scope, identity and format (check 1)

| Check | Result |
| --- | --- |
| Frozen commit and parent | `8617587e…`, parent `ab09111681bf…` — PASS |
| Diff | exactly 3 added files in `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/`; no source, asset, `.meta`, save, spec, canonical or board path — PASS |
| Whole-world manifest | 308,748 B, `342e1afc4d39cf44c22e434dac3b7ad56d9902084bf3f61026e89086a561a677`, blob `6c562f34…` — PASS |
| 36-cell companion | 82,840 B, `2441926a0a619b9731166149ca2121f2bb89f4a56a8d8ff827c39e9d77e86123`, blob `afd28d1b…`; equals M `/cells/sha256` — PASS |
| Construction dossier | 88,048 B, `f553fff031246311d0000050b2a9ece8a81efd0c54f1006a34e74289fe8a0f26`, blob `fb02244b…`, 699 lines, no BOM or CR, final LF — PASS |
| Strict JSON (both) | Strict parse with duplicate-key rejection and a `parse_constant` that rejects NaN/Infinity; UTF-8, no BOM, no CR, single final LF, no tabs. 1,190 + 576 floats, all finite. Byte-exact round trip under `json.dumps(…, ensure_ascii=False, indent=2, allow_nan=False) + "\n"` — PASS |
| Key order | Deterministic insertion order reproduced by the round trip. "Fixed" is defined by exemplar (N-5) |

## 5. Independent evidence

### 5.1 Owner-decision non-drift (check 2)

| Receipt (verified live, body byte-equal) | Direction | What the candidate does | Drift? |
| --- | --- | --- | --- |
| 6949218167 R1 OD-00 A | owner personally signs | `/attestation/authorised_signer`; all 4 signature entries null | none |
| R1 OD-01 A | flat y = 0, no hydrology | `/terrain` y = 0, up +Y, `hydrology: null` | none |
| R1 OD-04 A | pinned recipe, selectors, seed; half-open grid; no Reference | selectors 1/2/2/0, seed 20260914, +15 → −1, Reference not applied | none (N-3 hazard) |
| R1 OD-13 A | separate scene; D-032 first-party; no new dependency or LFS | `/scene` proposed, not created; rights BLOCKED, not invented | none |
| R1 OD-15 A | roster `dac06a3b…` verbatim | reproduced byte-for-byte (§5.2) | none |
| 6949441218 OD-10 | natural end almost at the north edge; no numeric setback approved | A8 (−2.10, 13.50), 1.50 m setback labelled author proposal | none |
| 6949464424 OD-02 A | deterministic ID-driven first-party variation | FNV-1a `PickBase`; 4 unpruned families; yaw 0 | none (N-4) |
| 6949490928 OD-03 A | pinned fresh constructor, every field | 19/19 TreeSaveData members (§5.3) | none |
| 6949553913 OD-05 A | ~12×10 m flat H1, soft edge, no stumps | 12×10 m H1, 0.5 m smoothstep edge, no stumps | none |
| 6949590935 OD-06 A | minimal H1, no parked vehicles | 2 modes, both with 0 vehicles, props and footprints | none |
| 6949647130 OD-07 A | start in H1 facing the path; 3.5 m / 8 m reach | spawn (2, 0.1, −8), yaw 16.94° → A0; 3.5 / 8 | none |
| 6949669482 OD-08 B | smooth path; ~1.2 m core; ≤0.20 m verge; recheck all margins | Hermite, 0.6 + 0.2 distance field; margins recomputed | none (F-3) |
| 6949697546 OD-09 A | four designated sites; free walking | 4 sites, empty initial list; "off-path walking not restricted" | none |
| 6949754608 OD-11 A | form-distinct states; r ≤ 0.10; remove the 0.141 m intrusion | 4 states, r = 0.09304; O1 core margin +0.385 | none |
| 6949817726 OD-12 A | zero planar tolerance | `planar_tolerance_m: 0`; negative cases listed | none |
| 6950307529 OD-14 A | one versioned manifest; roster digest never a world digest | domain-separated recipe; `world_digest: null`; roster labelled `ROSTER_ONLY_NEVER_WORLD` | none |
| 6950337240 OD-17 A | no initial marks or suggestions | `markType` 0 ×175; `cropTreeIds` []; `crop_suggestions` []; P0000/P1504 not inherited | none |
| OD-16 | DEFERRED | text-searched all 3 files: no threshold, quota or minimum Fell count | none |

D-085 (now on main) matches all 17 directions. No reopened choice and no invented owner approval was found. The three retained crop trees have no pre-marks and no fabricated Fell orders.

### 5.2 Roster (check 3)

Starting from raw `N/C-B-30.json` (128,518 B, `3e7c37ad…`): the inclusive H1 rectangle [−9, 3] × [−15, −5] taken from `E/ledger.json` selects exactly 14 IDs. They are P0004, P0005, P0006, P0207, P0306, P0307, P0403, P0404, P0405, P0406, P0407, P0503, P0505 and P0507, and they equal the ledger's `H1/inside`. Then:

| Check | Result |
| --- | --- |
| Rows after omission; ASCII ID order; ANN header and format (`%.3f`, `%.3f`, `%d`, `%.2f`, `%.2f`, `%.3f`), LF, final LF | 175 rows, 9,481 B |
| SHA-256 | `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` — equals OD-15 |
| Equals ANN first text fence (`1e0fba5f`) | byte-equal |
| Manifest `/roster/rows` re-serialised | byte-equal; 0 numeric mismatches; no extra keys (`ci`, `edge`, `side`, `crop` and `fellable` absent) |
| 8 buffers P0003, P0009, P0103, P0109, P0209, P0504, P0506, P0508 | all retained; equal to `/H1/retained_buffer_ids` |
| 19 rack-labelled stems (`side == "rack"`) P0700–P0707, P0710–P0715, P0800, P0802, P0804, P0806, P0815 | all retained as ordinary Sitka |
| Species and age | 175 × `sitka-spruce`, 175 × age 20 |
| Harvest, fell, credit assumption | none (`/initial_management` empty; H1 `must_not` list) |

### 5.3 Tree state (check 4)

`TreeSaveData` at the base (`ForestSaveData.cs:185–212`) has **19** members. Capture (`ForestSaveController.cs:139–160`) writes each one. M `/tree_state/fields` maps all 19, in order, and each value checks out against the source:

- `stage` 0 = `Mature` (`ForestTree.cs:5–10, 72`).
- `stageTimer` and `chopProgress` are zero-initialised (lines 73–74).
- `hasSimulation` is true on capture, not the DTO default.
- `equivalentSuppressedYears` is 0.
- `heightMeters`, `diameterCm` and `crownRadiusMeters` come from `SetSimulationState` clamps [0.1, 60], [1, 200] and max(0.1). No roster value is altered (heights run 8.07–15.90 m).
- `pruningLifts` 0, `crownBaseHeightM` 0 and `lastPruningYear` −1 (lines 174–176). `InitializeForSpawn` does not set a crown base.
- `markType` 0 = `None`.
- `biologicallyDead` false, `mortalityCause` "", `mortalityYear` −1 (lines 32–34).
- `position` is (x, 0, z), with root rotation identity (`ForestTreeSpawner.cs:145–148`).

`Height` resolves to `CurrentHeight`, which equals `heightMeters` for stage `Mature` (`ForestTree.cs:264–275`), so the canopy calculation uses roster heights. The "Mature" enum is honestly described as a mechanics stage, not biological maturity. No marks, deaths, stumps or defect state are invented.

### 5.4 Occupancy (check 5a)

Using `floor((z+15)/5)·6 + floor((x+15)/5)` on binary32 roster coordinates, with half-open ranges:

`[4, 2, 0, 3, 7, 5, 6, 0, 0, 1, 7, 7, 8, 5, 3, 2, 3, 8, 7, 6, 6, 4, 6, 7, 8, 5, 2, 3, 7, 8, 9, 5, 2, 4, 6, 9]`

The sum is **175**. The zero-stem cells are **2, 7 and 8**. Every ID appears exactly once. Every cell's `tree_ids`, `address`, `center_xz_m` and `bounds_xz_m` match.

### 5.5 Cells — independent binary32 recomputation (check 5b)

I wrote a separate C program with no Python struct emulation. It reads the roster decimals with `strtof`; a `(float)strtod` variant gives identical output. It applies `SetSimulationState` clamps and evaluates, per cell:

- `EC:787–807` centre = −15 + (x + 0.5)·5.
- `EC:822–850`: `Vector2.Distance` as float sqrt; reach = crown·1 + 2.5 (the constant `CanopyShadeReachPerCrownRadius = 1` at `EC:918`); clamp01 lateral × clamp01(height/8); product of gaps.
- `US:27–52` flora with site = stability = 1 and opening = 0.
- `UC:24–33` target = 0.8·site·(0.1 + 0.9·light), initial = target × 0.25.

Result: **all 360 values (36 cells × canopy, light, 6 flora proxies, competitor target, initial cover) are bit-equal** to C `/cells/*/expected_readback`. bramble equals bracken in every cell. F-1 covers the 1-ulp native-model alternates. Other checks:

| Check | Result |
| --- | --- |
| Seed rain | `SitkaSpruce.asset` has `maturityOnsetYears` 20 and `Maturity(age ≤ onset) = 0` (`TreeSpeciesDefinition.cs:257–264`), so every potential is 0. `EC:1371` writes only seed rain > 0, so `seedRainBySpecies` is empty and `seedRainFor` is 0 for all three species. Model 2 uses age bands, so no legacy empty cohort is created. Matches all 36 cells. |
| Seed value | 20260914 is the source initializer (`EC:7`) and the serialized `ForestTest.unity:5780` value. C-B-30 `/seed` is the same number, as provenance only. Not an invented ecology seed. |
| Selectors | `ScenarioOneManager.cs:472–475, 493, 497, 503`: MixedModel = 1, Competition = 2, SpeciesAware = 2, `StormModel.None` = 0 (`SimulationRandom.cs:16`, `RegenerationModel.cs:15`, `SitkaGrowthModel.cs:18`, `StormModel.cs:8`) |
| Browse | `ScenarioOneDefinition.cs:58` gives 0.2f → 0.20000000298023224 in every cell; shelters and protected areas empty (N-6) |
| Site drivers | `ForestEcologyCell.cs:47–50` defaults 1/1/1; `RecentOpening` 0; cohorts and deadwood empty; `lastUpdatedYear` 0 — all 36 |
| Reference profile | Not applied. Dormant at the base, with no callers (N-3). |
| Are the CANDIDATE cells complete? | Yes. Every persisted cell field (`ForestCellSaveData`, `ScenarioUnderstoreyCell`) is covered by an expected value or an explicit empty. The only non-installed ingredient is the Compact geometry dispatch and the composition order, which `recipe_order[0]` discloses honestly. |


### 5.6 Geometry (check 6)

I recomputed the route from the eight `E/ledger.json` samples plus A8. I used exact point-to-curve distances (dense parameter search with golden-section refinement on the analytic Hermite, not chords) and a dense same-z deviation scan.

| Quantity | Author | Reviewer | Status |
| --- | ---: | ---: | --- |
| Pinned tangents m0–m8 | as published | identical (max difference 0) | PASS |
| Published A, B, C, D coefficients vs Hermite basis | — | max error 1.3e-15 | PASS |
| Max same-z deviation from extended polyline (cap 0.095 m) | 0.094074074 | 0.094074074 | PASS |
| Original 8-anchor polyline length | 16.671961452 | 16.671961452 | PASS |
| Extended polyline / smoothed curve length (m) | 19.488804 / 19.509002 | 19.488804 / 19.509002 | PASS |
| Chord error bound M/(8n²) | 7.57e-8 | formula correct for ‖r″‖ = \|X″(t)\| | PASS |
| Minimum radius of curvature | not reported | **0.625 m** at the A7 end of segment A6–A7 | F-3 |
| A8 north setback / outer-verge north margin | 1.50 / 0.70 | 1.50 / 0.70 | PASS (author proposal, not owner numeric) |
| Curve plus 0.8 m footprint inside ±15 m | yes | x ∈ [−2.710, 2.990], z ∈ [−4.75, 13.50] | PASS |
| A0 to H1 north edge | 0.25 | 0.25 (core overlaps H1) | PASS |

| G1 site (species, stored x, z) | Smooth distance (m) | Core − 0.10 guard margin | Core + verge − 0.10 margin | Nearest stems (centre distance, m) | Cell / in H1 |
| --- | ---: | ---: | ---: | --- | --- |
| O1 sessile-oak (1.17, −0.47) | 1.085015301 | +0.385015 | +0.185015 | P0707 2.431369; P0807 3.007715; P0710 3.670373 | 15 / no |
| O2 sessile-oak (1.57, −2.73) | 1.290959360 | +0.590959 | +0.390959 | P0508 2.065642; P0707 3.246653; P0710 3.619961 | 15 / no |
| B1 beech (3.43, −0.07) | 1.179673441 | +0.479673 | +0.279673 | P0710 1.691637; P0810 2.243619; P0909 3.051431 | 15 / no |
| B2 beech (3.83, −2.33) | 1.002937869 | +0.302938 | **+0.102938** | P0710 1.579277; P0510 3.062746; P0508 3.635981 | 15 / no |

My distances agree with the published `route_distance_m` to ≤ 5e-9, within the author's chord bound, and the published `*_lower_m` values are conservative. G1 pair distances are 2.2951253 m (×4) and 3.2457973 m (×2); both clear the source 0.5 m duplicate radius. Nearest living stem centres are ≥ 1.579 m from any site, clearing 0.7 m.

| Nearest trunk edges to route (all 175 checked; trunk radius = DBH/200) | Edge distance (m) | Core margin | Core + verge margin |
| --- | ---: | ---: | ---: |
| P1507 | 1.072740662 | +0.472741 | +0.272741 |
| P1506 | 1.189328779 | +0.589329 | +0.389329 |
| P1106 | 1.503522733 | +0.903523 | +0.703523 |
| P0907 | 1.675270394 | +1.075270 | +0.875270 |
| P0510 | 1.714439637 | +1.114440 | +0.914440 |
| P0909 / P0508 / P0710 | 1.797509 / 1.811281 / 2.119743 | > +1.19 | > +0.99 |

The A8 terminal's nearest trunk edges are P1507 1.072741 m, P1506 1.189329 m and P1505 3.176838 m. A 0.60 m terminal disc clears them by at least +0.4727 m.

**Is the walkable core really 1.2 m?** Statically yes. The core is the set of points with d ≤ 0.6 m from the continuous curve. No trunk proxy collider comes within 0.47 m of its edge:

- The primitive cylinder's capsule radius is 0.5·(DBH/100) = DBH/200 (`ForestTreeSpawner.cs:150–153`, `ForestTree.cs:671–678`).
- Polished-visual colliders are stripped (`ForestTree.cs` `StripInteractionColliders`).
- The procedural guards have no colliders.

**Is off-path walking unrestricted?** Yes. There is one continuous 30 × 0.2 × 30 m BoxCollider and no path-only collider. Site rules restrict planting only.

**Guard-state envelopes (corrected O1 intrusion):**

| State | Tube / stake height | Plan radius | O1 core margin (radius 0.10) | Distinguishable without colour |
| --- | --- | ---: | ---: | --- |
| absent | 0 / 0 | 0 | — | no object |
| effective | 1.20 / 1.25 m | 0.0930390 | +0.385 | tall, 4-slot intact tube |
| expired | 0.85–1.20 / 1.25 m | 0.0930390 | +0.385 | scalloped top |
| failed | 0.28–0.35 / 0.45 m | 0.0930390 | +0.385 | low jagged shell, snapped stake |

- The radius is the far stake corner, √(0.0925² + 0.01²) = 0.09303897 < 0.10, with the tube at 0.065. The stake spans x 0.0725–0.0925, outside the tube.
- **Historical O1 intrusion reproduced:** `ScenarioProtectionVisuals.cs:89–94` gives a 1.2 m × r 0.10 cylinder, Euler Z 28°, offset (+0.25, −0.10, 0). I swept the full projected hull, including the tangent segments between the cap ellipses, not only the caps. Minimum distance to the original polyline is **0.4586424279** m, so the intrusion is **0.1413575721** m — equal to the author's figure.
- All 4 × 4 site/state pairs have positive core and core+verge margins in the proposed model.

These are static, proposed-model results only. No mesh exists or was rendered.

### 5.7 Player, spawn and controls (check 7)

| Property | Manifest | Source (base) | Class |
| --- | --- | --- | --- |
| Controller | CharacterController r 0.3, h 1.8, centre (0, 0.9, 0), step 0.25, slope 45, skin 0.03, min move 0, layers 0/0/0, priority 0, scale 1 | `ForestTest.unity:2713–2801` (object 412577895 / component 412577897; values at lines 2778–2784) | source literal |
| Camera | local (0, 1.65, 0), identity rotation | `ForestTest.unity:4294–4303` (571414935) | source literal |
| Ordinary reach | 3.5 m | `ForestPlayer.cs:17` default and scene line 2749 | source literal + OD-07 |
| Tree-marking reach | 8 m | `ForestTreeMarkingManager.cs:10` default and scene line 5910 | source literal + OD-07 |
| Move / run / jump / second jump / gravity | 4 / 7 m/s; 1.2 / 2 m; 20 m/s² | `ForestPlayer.cs:10–15` and scene | source literal |
| Mass | null, with reason | no Rigidbody on Player | correct |
| Spawn root | (2.00, 0.10, −8.00), pitch 0, roll 0 | — | **author proposal** |
| Yaw | 16.94154239283657° = atan2(0.99, 3.25), facing A0 | recomputed exactly | author proposal |
| Safe zone | capsule + skin 0.33 m is ≥ 0.67 m inside H1. Nearest trunk edge P0410 3.121190 m (+2.791190). Spawn→A0 connector nearest trunk edge P0510 1.714440 m | recomputed exactly | static only |

Other scene components (`ForestBuildable`, `ForestWoodStorage`) serialise `interactionDistance: 4`. These are not part of OD-07's two reaches and need no manifest value now.

### 5.8 Assets and rights (check 8)

- The four pinned unpruned family prefabs exist at the base with the stated GUIDs:
  - `Sitka_Pole_01_Unpruned` `6fc4c8b0…`
  - `Sitka_Young_02_Unpruned` `c54cc46c…`
  - `Sitka_Mature_01_Unpruned` `b3a74f27…`
  - `Sitka_Mature_Benchmark_01_Unpruned` `69a172cf…`
- There are 91 prefabs under `Prefabs/Forestry/Pruning`.
- The family READMEs (for example `Art/CropTreePruning/Docs/README.md`) describe procedural or prototype provenance and contain **no licence**. The candidate's `licence: BLOCKED` and `document_licence: null` are honest. The scoped S1B audit is not promoted to blanket approval.
- The D-032 / OD-13 A disposition is preserved as direction only.
- The Compact scene `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` is absent at the base and on main, as the manifest says ("PROPOSED_NOT_CREATED").
- No guard, ground, H1 or path material or mesh exists, and none is claimed to exist.
- No binary is claimed present when absent.

### 5.9 Identity and binding (check 9)

- Every registry literal matches the signed spec at `200add4e` (blob `8ac551022adb…`) §4.1 line 148: `scenario-one`, `o1-compact-v1`, `scenario-one-compact-v1`, `compact30-v1`, `scen1-compact-h1-g1-v1`, slot `forest-save-scenario-one-compact-v1.json`, numeric 2 as a reservation only.
- The neutral rack fields match §4.2 line 160.
- The SI-10 materiality and refusal wording matches line 182.
- `Compact30-H1-RouteA-G1-content-v1` is described only as a label.
- `world_digest` is null, with status `UNAVAILABLE_INCOMPLETE_NOT_UNSIGNED_CANDIDATE_DIGEST`. That is justified while rows 10, 12, 30, 44, 73–75 and 83 remain open.
- The roster digest is never presented as a world digest.
- The attestation is `UNFILLED_NOT_SIGNABLE`, with the signer scoped to the owner personally.
- The candidate makes no save-field addition and implies no serializer or CG-TRIG sign-off.

### 5.10 Dependencies and determinism (check 11)

I checked **all 347** entries in M `/dependencies`, not just a sample:

- 333 `git_object` entries: each verified through `git cat-file --batch` for blob ID, byte length and SHA-256.
- 14 `raw_local_file` entries: each re-read and re-hashed.
- Result: **347/347 match**, with no duplicates.
- The 27 entries in C `/source_dependencies` also match 27/27.

The entries span `ab091116` (314), `1e0fba5f` (14), `aab606ca`, `ac332a8d`, `d3a28dab`, `200add4e` and `1d60f3a1` (1 each). Git blobs, raw files and runtime package constants are kept distinct: the URP shader and script GUIDs are explicitly unresolved, not hashed (F-2). `StandGeometryModel.cs` is absent, consistently with "no installed Compact dispatcher". A sample of 28 entries:

| # | Kind | Path | Commit | Blob | Bytes | SHA-256 (first 16) | Result |
| ---: | --- | --- | --- | --- | ---: | --- | --- |
| 0 | git_object | `Assets/ForestPrototype/ForestTree.cs` | ab091116 | 2e4ada0aa135 | 28561 | `0582c138d7012428` | match |
| 1 | git_object | `Assets/ForestPrototype/ForestTreeSpawner.cs` | ab091116 | 7ed9a4b3498e | 9668 | `7205311459081649` | match |
| 2 | git_object | `Assets/ForestPrototype/ForestTreeVisualBase.cs` | ab091116 | f89945e67ed6 | 821 | `ce6b18f900b19ffc` | match |
| 3 | git_object | `Assets/ForestPrototype/ForestEcologyController.cs` | ab091116 | 0290a7beab63 | 105786 | `5db7231f2164eb18` | match |
| 5 | git_object | `Assets/ForestPrototype/ScenarioOne/ScenarioOneUnderstorey.cs` | ab091116 | 128a03ef0fdd | 3324 | `fda9fea6b96a4bca` | match |
| 8 | git_object | `Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs` | ab091116 | b29a5e447cb9 | 10713 | `05417363975cec5f` | match |
| 10 | git_object | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` | ab091116 | f1a34b44feb7 | 3975 | `f352a7e75acf284b` | match |
| 14 | git_object | `Assets/ForestPrototype/ForestSaveData.cs` | ab091116 | 56065ea164da | 16120 | `093cbea99aae9844` | match |
| 20 | git_object | `Assets/ForestPrototype/SitkaGrowthModel.cs` | ab091116 | e4c73402d46b | 5940 | `291601041876d834` | match |
| 26 | git_object | `Docs/Project/AgentWorkflow.md` | ab091116 | d58e16ab0659 | 28316 | `3e3db30aa18049b2` | match |
| 30 | git_object | `Docs/Project/unity-project-overview.md` | ab091116 | 4f77c86bfcd0 | 70123 | `3cb70a77dc117f9c` | match |
| 40 | git_object | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-657-AUTHORED-COMPACT30-ROSTER-PROVENANCE-AND-OWNER-SIGNATURE-CANDIDATE-20261116.md` | 1e0fba5f | 1ecf429cc7d7 | 32346 | `10bbc4464c5a1831` | match |
| 60 | raw_local_file | `<Files>/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/evidence-manifest.json` | — | — | 6135 | `cd35e159c0d306f5` | match |
| 90 | git_object | `Assets/ForestPrototype/Prefabs/Forestry/Pruning/Sitka_Young_02/Sitka_Young_02_LowPruned_Recent.prefab.meta` | ab091116 | 0841e0707f2c | 154 | `061fbfe2a1c6c79c` | match |
| 120 | git_object | `Assets/ForestPrototype/Prefabs/Forestry/Pruning/Sitka_Mature_Benchmark_01/Sitka_Mature_Benchmark_01_LowPruned_Healed.prefab.meta` | ab091116 | be9c8f1cd81e | 154 | `e6f441200c250022` | match |
| 150 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Sitka_Pole_01/Sitka_Pole_01_LowPruned_Recent_LOD1.fbx.meta` | ab091116 | f6e4b62e4e0f | 3055 | `7e4b89ed3162e92a` | match |
| 180 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Sitka_Pole_01/Sitka_Pole_01_HighPruned_Healed_LOD1.fbx.meta` | ab091116 | 2e1027ed7531 | 3055 | `63389fb89fbb4300` | match |
| 210 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Sitka_Young_02/Sitka_Young_02_MediumPruned_Healed_LOD0.fbx.meta` | ab091116 | 4e47514a6ef1 | 3055 | `2dab0f292d3c6ac4` | match |
| 240 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Sitka_Mature_01/Sitka_Mature_01_LowPruned_Healed_LOD0.fbx.meta` | ab091116 | 51c4d3d76790 | 3055 | `8e32443085508ed2` | match |
| 270 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/FBX/Sitka_Mature_Benchmark_01_Unpruned_LOD1.fbx.meta` | ab091116 | 54fcb8fb05e3 | 3055 | `10614dca80d8ddd9` | match |
| 300 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/FBX/Sitka_Mature_Benchmark_01_MediumPruned_Recent_LOD0.fbx.meta` | ab091116 | f4c947df878f | 3055 | `4b58553ad67a64d2` | match |
| 330 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Textures/Pruning_HealedScar.png.meta` | ab091116 | fade332a7f4a | 2707 | `4c17df3280c45baf` | match |
| 333 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/Textures/Sitka_NeedleSprigs_BaseColorAlpha.png` | ab091116 | 81b2ed9f7948 | 4639024 | `596f2393bcda13d4` | match |
| 335 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/Textures/Sitka_Bark_BaseColor_v2.png` | ab091116 | 222287c571db | 3813680 | `e93ffd30393a8dfe` | match |
| 338 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/Textures/Sitka_Bark_Normal_v2.png.meta` | ab091116 | 01653b30791e | 2707 | `5416472fe6ec798f` | match |
| 340 | git_object | `Assets/ForestPrototype/Art/SitkaMatureBenchmark/Textures/Sitka_CutWood_BaseColor.png.meta` | ab091116 | 6ec5ecfc9dd6 | 2707 | `b4f9818687ebb3d6` | match |
| 343 | git_object | `Assets/ForestPrototype/Art/CropTreePruning/Docs/README.md` | ab091116 | 4775c14680ab | 5584 | `57e29ab838d13c71` | match |
| 346 | git_object | `Docs/Verification/S1B/RightsAudit.md` | ab091116 | 2e9011a2a3c6 | 5236 | `07c8a7a8dfaa97db` | match |

## 6. 88-row and 36-cell recount (check 10)

I counted from the JSON `/field_closure` (88 rows). I compared the "before" side with the parsed #672 matrix table at `aab606ca` (lines 239–356), and both sides with the dossier §3 table.

- Field names, before class/status and after class/status agree in all three sources for all 88 rows.
- Every `manifest_pointer` resolves.
- Every BLOCKED row names a blocker ID, and no CANDIDATE row carries one.

| Recount | SV | MD | OC | GCP | UNS | SIGNED | CANDIDATE | BLOCKED |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Before (#672 `aab606ca`, reviewer parse) | 21 | 4 | 10 | 1 | 52 | 0 | 36 | 52 |
| After (#676 JSON, reviewer count) | 36 | 13 | 20 | 11 | 8 | 0 | **80** | **8** |
| Author claim | 36 | 13 | 20 | 11 | 8 | 0 | 80 | 8 |

- **Changed status (44 rows, BLOCKED → CANDIDATE):** 9, 11, 13–26, 28, 29, 32, 34–43, 45–47, 59–64, 71, 76, 79, 80, 86, 87.
- **Class change without status change:** 72 (OC→GCP) and 88 (GCP→OC) — see N-1.
- **Still BLOCKED, with real blockers:**
  - Row 10: B-W02-RIGHTS (README has no licence).
  - Row 12: B-W02-SCALE (uniform-height scaling does not enforce DBH or crown).
  - Rows 30, 44, 73, 74, 75: B-W13-RENDER-DEPS (unresolved URP shader bytes; new materials and meshes not created).
  - Row 83: B-W14-WHOLE (depends on the others; no scene).
  - F-2: rows 10 and 12 should also carry B-W13.
- **Content behind each CANDIDATE:** I checked each changed row against #672's stated missing datum. Each one now has a concrete value or recipe at its pointer: terrain y/normal; yaw 0; the 19 tree fields; the BoxCollider; the 36 cells; the boundary −1 rule; the spawn pose; zero-prop staging modes; the Hermite curve and projection; the 0.6/0.2 distance field; the site-admission predicate; A8; zero tolerance; the scene controller literals; bound reaches; selectors and seed; initial management.
- **Misclassified rows:** none by status. No silent promotion.

| 36 cells | SIGNED | CANDIDATE | BLOCKED |
| --- | ---: | ---: | ---: |
| Before (#672) | 0 | 0 | 36 |
| After (C `/cells/*/signature_status`, reviewer count) | 0 | **36** | 0 |

All 36 cells carry full recomputed content (§5.5). C `/blockers` is `[]`. `native_readback_status` is `NOT_RUN` in C and M. M `/not_run` lists 16 items. Historical-world protections are preserved:

- No historical scene, DTO or `forest-save.json` change.
- Legacy40, Enlarged80, Rack V2, MFM and frozen Reference stay untouched.
- Geometry 2 is a reservation only.

### 6.1 Reviewer cell table (from my C program; all bit-equal to the companion)

| Cell (x,z) | Stems | Canopy (reviewer C binary32) | Light | Mosses | Ferns | Grasses | Forbs | Shrubs | Fungi | Bramble = bracken initial | Bit-equal to C JSON |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | --- |
| 0 (0,0) | 4 | 0.977539718 | 0.022460282 | 0.807575 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.692139 | 0.024042850 | yes (10/10) |
| 1 (1,0) | 2 | 0.887504637 | 0.112495363 | 0.637509 | 0.164048 | 0.000000 | 0.000000 | 0.000000 | 0.660627 | 0.040249165 | yes (10/10) |
| 2 (2,0) | 0 | 0.000000000 | 1.000000000 | 0.000000 | 0.000000 | 0.600000 | 0.550000 | 0.700000 | 0.350000 | 0.200000003 | yes (10/10) |
| 3 (3,0) | 3 | 0.996412933 | 0.003587067 | 0.843224 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.698745 | 0.020645673 | yes (10/10) |
| 4 (4,0) | 7 | 0.997817338 | 0.002182662 | 0.845877 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699236 | 0.020392880 | yes (10/10) |
| 5 (5,0) | 5 | 0.995279908 | 0.004720092 | 0.841084 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.698348 | 0.020849617 | yes (10/10) |
| 6 (0,1) | 6 | 0.999055684 | 0.000944316 | 0.848216 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699669 | 0.020169979 | yes (10/10) |
| 7 (1,1) | 0 | 0.531738400 | 0.468261600 | 0.000000 | 0.469564 | 0.102392 | 0.214589 | 0.000000 | 0.536108 | 0.104287088 | yes (10/10) |
| 8 (2,1) | 0 | 0.398184597 | 0.601815403 | 0.000000 | 0.246974 | 0.302723 | 0.407891 | 0.178177 | 0.489365 | 0.128326774 | yes (10/10) |
| 9 (3,1) | 1 | 0.878055513 | 0.121944487 | 0.619660 | 0.193577 | 0.000000 | 0.000000 | 0.000000 | 0.657319 | 0.041950006 | yes (10/10) |
| 10 (4,1) | 7 | 0.999606073 | 0.000393927 | 0.849256 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699862 | 0.020070909 | yes (10/10) |
| 11 (5,1) | 7 | 0.999832213 | 0.000167787 | 0.849683 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699941 | 0.020030202 | yes (10/10) |
| 12 (0,2) | 8 | 0.999850631 | 0.000149369 | 0.849718 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699948 | 0.020026887 | yes (10/10) |
| 13 (1,2) | 5 | 0.988871992 | 0.011128008 | 0.828980 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.696105 | 0.022003042 | yes (10/10) |
| 14 (2,2) | 3 | 0.969853640 | 0.030146360 | 0.793057 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.689449 | 0.025426347 | yes (10/10) |
| 15 (3,2) | 2 | 0.778926194 | 0.221073806 | 0.432416 | 0.503356 | 0.000000 | 0.000000 | 0.000000 | 0.622624 | 0.059793282 | yes (10/10) |
| 16 (4,2) | 3 | 0.988216102 | 0.011783898 | 0.827742 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.695876 | 0.022121102 | yes (10/10) |
| 17 (5,2) | 8 | 0.999619842 | 0.000380158 | 0.849282 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699867 | 0.020068429 | yes (10/10) |
| 18 (0,3) | 7 | 0.999926269 | 0.000073731 | 0.849861 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699974 | 0.020013271 | yes (10/10) |
| 19 (1,3) | 6 | 0.999517918 | 0.000482082 | 0.849089 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699831 | 0.020086777 | yes (10/10) |
| 20 (2,3) | 6 | 0.999413192 | 0.000586808 | 0.848892 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699795 | 0.020105626 | yes (10/10) |
| 21 (3,3) | 4 | 0.991965890 | 0.008034110 | 0.834825 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.697188 | 0.021446140 | yes (10/10) |
| 22 (4,3) | 6 | 0.999756932 | 0.000243068 | 0.849541 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699915 | 0.020043753 | yes (10/10) |
| 23 (5,3) | 7 | 0.999836445 | 0.000163555 | 0.849691 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699943 | 0.020029440 | yes (10/10) |
| 24 (0,4) | 8 | 0.999923289 | 0.000076711 | 0.849855 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699973 | 0.020013807 | yes (10/10) |
| 25 (1,4) | 5 | 0.997979164 | 0.002020836 | 0.846183 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699293 | 0.020363752 | yes (10/10) |
| 26 (2,4) | 2 | 0.966078281 | 0.033921719 | 0.785926 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.688127 | 0.026105910 | yes (10/10) |
| 27 (3,4) | 3 | 0.998565972 | 0.001434028 | 0.847291 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699498 | 0.020258127 | yes (10/10) |
| 28 (4,4) | 7 | 0.999756873 | 0.000243127 | 0.849541 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699915 | 0.020043764 | yes (10/10) |
| 29 (5,4) | 8 | 0.999856472 | 0.000143528 | 0.849729 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699950 | 0.020025836 | yes (10/10) |
| 30 (0,5) | 9 | 0.999846160 | 0.000153840 | 0.849709 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699946 | 0.020027691 | yes (10/10) |
| 31 (1,5) | 5 | 0.991473377 | 0.008526623 | 0.833894 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.697016 | 0.021534793 | yes (10/10) |
| 32 (2,5) | 2 | 0.872223735 | 0.127776265 | 0.608645 | 0.211801 | 0.000000 | 0.000000 | 0.000000 | 0.655278 | 0.042999726 | yes (10/10) |
| 33 (3,5) | 4 | 0.940183997 | 0.059816003 | 0.737014 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.679064 | 0.030766880 | yes (10/10) |
| 34 (4,5) | 6 | 0.999947846 | 0.000052154 | 0.849902 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699982 | 0.020009389 | yes (10/10) |
| 35 (5,5) | 9 | 0.999896586 | 0.000103414 | 0.849805 | 0.000000 | 0.000000 | 0.000000 | 0.000000 | 0.699964 | 0.020018615 | yes (10/10) |

## 7. What could not be verified, and why

- **Native behaviour of any kind.** No Unity, Editor or compiler session was authorised. That covers construction and readback, walking and collision, branch contacts, turning on the 0.625 m bend, interaction reach, rendering and LOD, guard legibility, 720p/1080p review, performance, save/load, profile refusal and Reference regression.
- **Bitwise native parity of the cell values.** F-1 shows that the archived native evidence follows a different arithmetic model; only a fresh native readback can settle it.
- **URP shader and script bytes** for `933532a4…` and `d0353a89…` are in the package cache, outside tracked Assets. Inspecting them was not authorised.
- **Asset rights.** No licence record exists to verify, so the provenance READMEs are all that could be read.
- **Realism** of the proposed guard and surface constructions (procedural specifications only).
- **Ecological truth.** All flora, site, browse and competitor responses are inherited game calibration, as the candidate says. None is an Irish field measurement.
- **Whole-world digest.** It is correctly not computed, so it was not verified.

## 8. NOT RUN

Unity, Editor, compiler/typecheck, tests, import, native construction/readback, walk/collision/interaction, annual ecology, save/load/refusal, Reference regression, rendering, performance, Windows, build, deploy, Ops, lease, Prefs, Drive/dashboard sync. No archived script or harness was executed. No author script was executed: I wrote and ran my own.

## 9. Recommended corrections (non-blocking; for the next author revision)

The verdict is APPROVED, so no correction round is required now. Any later revision that closes the eight blockers must produce new bytes and get a new review anyway. At that point the author should also fix:

1. **F-1:** reconcile `/numeric_policy` and dossier §5 with the native C-B-30 year-0 evidence. State that the values are tolerance-bound expected readback, and optionally list the 1-ulp alternates for cells 7, 8 and 15.
2. **F-2:** add B-W13-RENDER-DEPS to rows 10 and 12, or justify leaving it off. Describe `933532a4…` as the URP shader GUID and `d0353a89…` as a script GUID referenced by the materials.
3. **F-3:** report the curvature profile and the 0.625 m minimum radius at A7. Either accept it explicitly as an author proposal or revise the tangent rule and re-run every margin.
4. **Optional notes:** a step column (N-1); the chapter-ID key hazard (N-3); OD-02 yaw shown as an explicit owner trade-off (N-4); an explicit key schema plus the off-Git input list on the attestation page (N-5).

If the Manager instead routes a correction round now, the re-check should cover changed lines only, plus a re-hash of the three files.

## 10. Verdict

**APPROVED** — document/evidence quality. CRITICAL 0 / MAJOR 0 / MINOR 3 (F-1, F-2, F-3) / NOTE 6 (N-1 … N-6).

**SIGNATURE-BLOCKED** — the content is not signable. It has 8 BLOCKED rows (10, 12, 30, 44, 73, 74, 75, 83), `world_digest` null, an unfilled owner signature, and native qualification NOT RUN.

The following were independently reproduced:

- The three file hashes.
- Strict JSON format.
- The 175-row roster `dac06a3b…`.
- All 19 tree-state fields against the source.
- All 36 cells, bit-exact (360/360 values).
- Occupancy (175; zero cells 2, 7, 8).
- The seed and selector provenance.
- Every route, A8, G1, trunk, spawn and guard margin.
- The historical O1 intrusion of 0.1414 m.
- The registry literals against the signed spec.
- All 347 dependencies.
- The 88-row before→after recount: 0/36/52 → 0/80/8; cells 0/0/36 → 0/36/0.

All 13 owner receipts match live bodies byte for byte. No owner choice was reopened, OD-16 stays deferred, and no crop marks or Fell orders were created.

**This approval is not the owner's content signature. It is not spec approval or save-owner approval. It is not Unity or native qualification, integration, a build or a release.** Only the product owner personally can attest exact world bytes (OD-00 A), and only after the eight blockers close and the completed candidate is independently reviewed.
