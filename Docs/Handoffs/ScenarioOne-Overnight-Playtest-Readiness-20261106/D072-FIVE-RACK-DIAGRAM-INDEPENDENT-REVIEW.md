# D-072 Stage B / T3 (#296): five-rack teaching figure, INDEPENDENT SOURCE REVIEW

**Verdict: NEEDS CHANGES: 1 blocking, 3 major, 8 minor.** This is a source-only review opinion. **It is not Manager approval** and it is not a Unity, render or playtest result.

**The fixture is authentic.** I found no invented number. I ran the real production `ScenarioOneSuggestedRackLayout.Get` on the archived world's 1,344 trees, and it returned the shipped Z values exactly. The problems are elsewhere:

- the figure shows in worlds it does not describe (B-1);
- its added text probably pushes the Objective 1 card off the screen (M-1, an estimate);
- the offline suite cannot see the painter (M-2);
- the key does not name the confirmed state (M-3).

## Reviewer, lane and SHAs

| Item | Value |
|---|---|
| Provider / model | Anthropic, Claude Opus 5.5 (`claude-opus-5-5`), instance Claude B — Pro, Claude Code harness in T3 Code |
| Effort | Session reasoning-effort indicator **30**. I was told no requested effort for this review, and I do not claim a higher one. |
| **Independence limitation** | **Same provider family as the author.** Different account and instance (author Claude A, reviewer Claude B). Both enabled OpenAI instances are quota-exhausted until about 2026-11-08, so the packet's preferred cross-provider reviewer was not available. Shared blind spots between two Claude models are possible. Treat this as separate-instance review, not cross-provider review. |
| Author's reported effort | The dispatch says HIGH was requested. The handoff reports an exposed setting of 15 (low band). I record this mismatch as reported and did not verify it from code. |
| Reviewed | base `1b68ddcd` → source `579ffaac27ecbaceca6132cb2c64ecafaf118e75` → handoff `e7bda1f2eea4340a0d8eaae351abb43ffaff8202` |
| Lane | `/media/jer/Files/CCF/Operations/CCF-D072-FiveRackReview-20261106`, branch `docs/d072-five-rack-diagram-review-20261106-v1`, ext4, clean at start at `e7bda1f2` |
| Date | 2026-11-06 |
| Evidence (outside the repo) | `/media/jer/Files/CCF/Verification/D072-T3-FiveRackDiagram-Review-20261106/` |

## Findings

### Blocking

**B-1. The figure draws in every world, and its statements are false in non-V2 worlds.**
`Assets/ForestPrototype/UI/TeachingDiagram.cs:35` chooses the figure from the lesson number alone:

```csharp
stage = lesson == 0 ? Plan.ObjectiveOneFigure(SiteFromAuthority()) : Plan.For(lesson);
```

- **Who sees it:** the runtime test for a V2 world is `RackPlanningV2Active` (`ScenarioOneManager.RackPlan.cs:33-40`). Its comment says Legacy40, Reference Future v1, pre-v21 saves and pre-V2 Enlarged80 continuations are not V2 worlds. Any older save still reaches lesson 0. `LearningObjectivesView.cs:152-156` already switches the card's words on this test, but `:161` calls the diagram without it.
- **What goes wrong:** the card keeps its legacy words ("Press B, click the tree…") beside a picture that says "A: existing western headland. B: existing southern feeder" (`TeachingDiagramPlan.cs:596`). Those surfaces exist only when `useV2Headland` is true (`ForestStartingStand.cs:247`). The picture also shows an 80 m stand in a 40 m world and teaches field-marking that those worlds do not have. The title's "not your saved Stand Map" does not fix a sentence saying a road exists when it does not.
- **Why it blocks:**
  - The D-072 spec says "no save/Reference/Legacy40 changes" (`spec.md:62`).
  - The pinned rubric says "Preserve… Legacy40" (`e5fe1761` `spec.md:17`).
  - The same spec's map half requires a truthful V2 gate (FR-M1, FR-M5).
  - The Manager release carries "no phantom paths" for these worlds.
- **The result is certain:** this follows from the source alone, without Unity.
- **Author's disclosure:** caveat 1 reports the issue honestly. It leaves out that the legacy card would carry false statements and that the flag is already computed at the call site.

See question 9b for the options. **If the Manager explicitly accepts this behaviour, B-1 becomes a recorded caveat.** That decision is the Manager's.

### Major

**M-1. The added text probably pushes the Objective 1 card past the screen at both sizes. This is an estimate, not a measurement.**
`TeachingDiagram.cs:41` and `:56` add a 105-character title and 582 characters of notes at 20 UI units to the right-hand column, which already holds the frame and caption. Existing gates require the whole card to fit, and they were made to pass by the #224 overflow repair:

- `GuidedOnboardingVerification.cs:88` ("current objective fits", the root must contain the card);
- `UxPedagogyShots.cs:425-426` (`OBJECTIVE_CARD_FITS_THE_SCREEN`, `OBJECTIVE_CARD_NEEDS_NO_SCROLLING`).

I scaled from the real Unity captures `02-current-rack-objective-{1280x720,1920x1080}.png` in `ScenarioOneObjectivesOverflow-20261102/unity/guided-v2-after/evidence/`. Character width and line height were measured from those images:

| | 1280×720 | 1920×1080 |
|---|---|---|
| Frame width in the capture | 648 px | 946 px |
| Characters per line at 20 UI units | about 90 | about 88 |
| Added lines (title 2, notes 3 + 2 + 3) | 10 | 10 |
| Added height including margins | about 197 px | about 295 px |
| Estimated card bottom | **about 745 px** | **about 1,118 px** |
| Screen height | 720 px | 1,080 px |
| Scroll viewport bottom (approx.) | 701 px | 1,035 px |
| Estimated overrun past the viewport | about 44 px (3 lines) | about 83 px (3–4 lines) |

**Limits of this estimate:** those captures come from an older candidate (#224), and I assumed no other card changes since. Real text metrics may differ, so this must be measured in Unity. The margin is still large enough that I do not expect it to fit.

FR-5 would accept scrolling, but the harness does not, and the spec forbids weakening old checkers ("no old checker weakened"). Caveat 6 anticipates "roughly 5–8 lines", and the future-gate step 2 says only "objectives card still reachable", which understates the hard no-scroll assertions.

**Recommendation:** cut at least 4 lines of note text now, inside the two allowed files. For example, shorten the A/B/C note to one line. Do this before spending a Unity lease, and do not change the assertion.

**M-2. The offline suite cannot see the painter, so the shape, title and caption claims have no test behind them.**
The checker compiles only `TeachingDiagramPlan.cs`. "Colour-independent" is tested on an enum name (`FiveRackDiagramCheck.cs:215`). New painter code at `TeachingDiagram.cs:217-312` has no behavioural coverage. All of these passed with `FIVE_RACK_SUITE_PASS`:

| Mutation (mine) | What it breaks | Result |
|---|---|---|
| R9 planned corridor drawn dashed | draft and planned become identical | **survived** |
| R10 planned corridor fill made opaque | hides standing trees, so it looks cleared | **survived** |
| R11 notes never added | loses the 16–20 m, north and scale wording | **survived** |
| R12 title never added | loses the mandatory caption | **survived** |

R9 and R10 strike at the D-071/FR-T3 semantics, and R11 and R12 remove required text. Reading the painter myself, the shipped code does the right thing (see points 5 and 6). What is missing is protection against regressions. Unity captures will eventually see these, but a few regexes in `source_guards()` could catch R9, R11 and R12 today. **Recommendation:** add those guards, and say in the handoff that row 1f covers plan data only.

**M-3. The state key never names "confirmed" or "pending".**
FR-T3 requires "Planned vs confirmed/pending vs executed/harvested" to be told apart by shape plus words. FR-5 maps "confirmed planned solid outlined… 'pending work'" and its rubric requires the key to be "correctly named". The key rows (`TeachingDiagramPlan.cs:476-477`) are "Draft rack: dashed, a proposal" and "Planned rack 1–5: solid, trees stand". Nothing tells a learner that solid means confirmed and pending; the only link is a note below the frame. On the real Stand Map, solid means "rack in the Work Plan" (`StandMapSymbols.cs:82`). This is a one-row wording fix (the checker's `StartsWith("Planned")` allows it).

### Minor

| ID | Location | Finding |
|---|---|---|
| m-1 | `FiveRackDiagramCheck.cs:230-231`, `TeachingDiagramPlan.cs:491,493` | The check "every symbol on the map is explained in the key" only proves that four key shapes appear on the map, not that every map shape is in the key. The dashed property boundary and the clearing disc are explained nowhere. The boundary is also dashed while the key says "dashed, a proposal". |
| m-2 | `FiveRackDiagramCheck.cs:205` | "No tree disc on the existing surfaces" calls the same `OnExistingSurface` that production uses to place trees, so it tests itself. R7 (function returns false) **survived**. |
| m-3 | `FiveRackDiagramCheck.cs:175` | Approach fidelity runs one way: fixture points on the real track. R8 (track thinned to 3 points, bend lost) **survived**. The shipped data is fine: my reverse check gives a worst real-to-fixture distance of 0.0126 m across 121 real points. |
| m-4 | `run_five_rack_check.py:197,204` | The default `--out` is the author's evidence directory, and the runner deletes it first. A plain re-run destroys the author's offline logs. I used `--out` elsewhere and they were untouched. |
| m-5 | `run_five_rack_check.py:112-113`, `FiveRackDiagramCheck.cs:79` | With the archive absent, the suite prints `NOT RUN` but still ends in `FIVE_RACK_SUITE_PASS`, so a cloud run reports PASS without the crucial check. |
| m-6 | `TeachingDiagramPlan.cs:136`, `StandMapSymbols.cs:82` | Racks are lesson purple where the Stand Map uses amber. FR-5 asks that such differences be labelled as teaching annotations. The title covers this only loosely. |
| m-7 | handoff, "What changed" | "Existing painter methods are unchanged" holds for the 8 compared methods only. The constructor, `AddText`, `OnFrameGeometry` and `DrawItem` did change. I read those changes and they do nothing for old stages, but the sentence is broader than its proof. |
| m-8 | `TeachingDiagramPlan.cs:498` onward, `TeachingDiagram.cs:378` | 384 map tree discs, each a 40-step fill plus a 40-step stroke, is much heavier than the old pictures. Mesh cost is unmeasured. Carry it to the Unity performance check with #297 m-1. |

## Disposition per audit point

| # | Point | Disposition | Evidence |
|---|---|---|---|
| 1 | Scope | **PASS** | `git diff --name-status` shows exactly 5 files. USS, #297 map files, saves, scenes, prefabs, racks, MFM and 3D are unchanged. My own reflection dump of `For(-1)`..`For(7)` (every field, the palette `Blend` for 15 base roles, Kind ordinals) is **byte-identical**: both `c76b0e36…`, 79 lines. This also covers the palette, which the existing dump omits (R13 survived there). |
| 2 | **Authenticity (crux)** | **PASS, no invented number** | See below. |
| 3 | Topology | **PASS (source)** | 5 corridors and 5 entrances. Feeder Z −27.4 lies between −35.27 and −16.42, and 12.6 m from the south edge. Headland and feeder come from `ScenarioOneHeadlandGeometry` via `SiteFromAuthority`. Approach matches scene planks plus `StandApproachRoute`: 0 m forward, 0.0126 m reverse. R6 (fake spine) was caught. |
| 4 | Retained stand | **PASS (source)** | My own tally from the dump: 6 bands 16/65/61/60/85/17; along the corridors 16/15/16/16/17. Live trees exist in every band in the archive (58/249/230/223/301/61). The note says the discs are not a tree count. |
| 5 | Semantics | **PASS for code, gaps in M-2 and M-3** | Read in the painter: draft dashed, planned solid over visible trees, harvested opaque pale with stumps (key only), entrance square, field mark as stripes. No `Role.Fell` or slash anywhere. Note says "only Work Plan approval commits the work". |
| 6 | Text and orientation | **PASS (source)** | Title bytes match the packet exactly. North arrow and N are present. Wording is "guidance for this plantation, not a hard rule", with the spread computed from the fixture (15.6–19.4 m). Approximate-scale caveat present. No scale bar and no whole-game claim. Display depends on M-1 and M-2. |
| 7 | Static safety | **PASS** | No file, save, scene or live-map access in either file. The snapshot path is an unused provenance string. Dump sha `af5c982a…` matches the author's, on a separate run. |
| 8 | Test quality | **Real for plan data, blind for the painter** | Figure data is the real compiled `TeachingDiagramPlan.cs`, not shadowed by test values. The `expected` gaps and title are literals, but the Z values are also compared to the archive. 16 mutations in my driver: 9 caught, 7 survived (M-2, m-2, m-3, R13). |
| 9 | Red gates | **See below** | 9a artefact, 9b breach (B-1), 9c acceptable for now. |
| 10 | Honesty | **Good, with noted gaps** | All tallies reproduced. See below. |

### Point 2: how I tested authenticity

| Claim | My check | Result |
|---|---|---|
| Snapshot hash | `sha256sum` on the archive | `e8bb96f4…0ac5` **matches** |
| World identity | Parsed JSON | version 21, `standGeometryModel` 1, `rackPlanningModel` 1, seed 20260914, 1,344 trees; adjacent `result.json` PASS at `96d9abe2` |
| Five entrance IDs | Against `ScenarioOneSuggestedRackLayout.cs:23`, then the archive | Identical, in order; each found exactly once |
| Z values and rule | **Ran the real production `Get(1, 1, trees)` on all 1,344 archived trees**, compiled from source | 5 routes, every Z **float-exact** to `FixtureEntranceZ`; `REAL_GET_MATCHES_FIXTURE` |
| Run-from-headland rule | Same run | Each route runs (−35.4, Z) to (40, Z). Drawn corridors differ from it by 0.00000 m, against the 0.8 m FR-5 limit |
| Width 2.4 m | `RackGeometry.cs:53` and the run | 2.4; width deviation 0.000% |
| Gaps | Computed from the archive | 18.849411 / 16.660000 / 15.555294 / 19.438824 m |
| Second source | Real Unity log `RackV2-04A-20261027/unity/SuggestedLayoutVerification.log` | All 5 routes agree at 2 decimals |

This is stronger than the two earlier handoffs, which reconstructed the algebra rather than running it. Tree discs, row spacing and layout scale are design constants, and they are disclosed as schematic.

**One claim I did not settle:** the code comment says positions are "seeded per world", and I did not prove how far new worlds differ. The default seed field (`ForestSaveData.cs:13`) equals the fixture's seed, which may mean default new games match it closely.

### Point 10: handoff claims against what I ran

| Claim | Result |
|---|---|
| 100 + 75 checks, 0 warnings, 7 mutations caught | **Reproduced** at `e7bda1f2` |
| Dump sha, 422 items, band counts | **Reproduced** |
| 1310 → 1330 lesson checks | **Reproduced** |
| Design-match FAIL `lines=762` is pre-existing | **Reproduced**: same result with the base plan in a scratch clone |
| Typecheck clean, typo caught | **Reproduced**. A body typo gives `TYPECHECK_NEW_ERRORS`. A declaration typo gives `TYPECHECK_BLIND`, also not clean. |
| Stand Map 176 PASS | **Reproduced** |
| #297 suite FAIL at first scope guard | **Reproduced** |
| T2 38 PASS / 1 FAIL pre-existing | **Reproduced**; the diff `e1be15c1..1b68ddcd` already holds the Map files |
| Not a self-approval; Unity NOT RUN | Consistent with what I saw |
| Omissions | M-1's #224 hard gates, M-2's painter blindness, B-1's false statements, m-1's overstated check name |

## Questions 9a, 9b, 9c (opinion, not product policy)

### 9a. #297 file-inventory check: a stage artefact, not scope leakage

The check, `run_headland_check.py:90-92`:

```python
production = set(git("diff", BASE, "--name-only", "--", "Assets", "Packages", "ProjectSettings").splitlines())
allowed = {str(MAP / name) for name in ("StandMapSource.cs", "StandMapDrawPlan.cs", "StandMapModel.cs", "StandMapLegend.cs")}
require(production == allowed, "only four authorised display files changed; world/save/racks/Reference/Mini consumers untouched")
```

- Exact equality against `7f5278bf` means any later authorised Assets change fails it.
- The diff now shows exactly six files: #297's four plus the two files the Manager released for #296. `1b68ddcd..e7bda1f2` touches only those two under Assets.
- In an out-of-repo copy I replaced the guard with a **stricter** one, `production - t3 == allowed and t3 <= production`, instead of downgrading it to a print. Result: **30 source guards and `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`**, `HEADLAND_SOURCE_AND_OFFLINE_PASS`.

**Recommendation:** treat this as an artefact. Do not edit the accepted check silently. The Manager should rule on one separately owned repair that names the released stage files, or pin the check to `1b68ddcd`. Until then the suite stops at this line, so the 92 assertions cannot run in place. The fix for B-1 would also trip the same suite's protected list (`LearningObjectivesView.cs`, `:96`), so one ruling should cover both.

### 9b. Legacy40: yes, I read this as a breach of the gate in substance. It needs a Manager ruling.

See B-1. Gating options:

| Option | Allowed files? | Opinion |
|---|---|---|
| **Pass `rackV2Objective` at the call site:** `new TeachingDiagram(index, caption, rackV2Objective)`, with non-V2 falling back to `Plan.For(0)`, through a pure predicate like FR-M1 that is testable offline | **No.** `LearningObjectivesView.cs:161` is read-only for T2, and it needs a scope change | **Recommended.** One line, the flag already exists, the old picture stays for old worlds |
| Look up the manager inside `TeachingDiagram` with `FindFirstObjectByType<ScenarioOneManager>()` (the repo has precedent) | Yes | Possible but not recommended. It hides a scene dependency in a passive picture and trips the author's own source guard. |
| Keep the figure in all worlds and reword it | Yes | Weak. The notes would have to stop calling these surfaces existing, which undermines V2 teaching. |
| Manager accepts it as is | n/a | The Manager's call. It would need an explicit waiver against "no Legacy40 changes". |

### 9c. Five racks, then two: acceptable now, but a real inconsistency

The packet and FR-T3 explicitly keep later pictures unchanged, so this is compliant and not a defect in #296. `RackColumns = { 4, 9 }` is still the picture for lessons 2–8. The learner does move from a labelled real layout to an unlabelled two-rack concept picture. Lesson 2's words already mention a "middle rack", which the two-rack picture does not have; that older mismatch was there before this change.

**Recommendation:** do not block. Put a specific question in the user clarity review. If it confuses people, a separately scoped task could add a short "concept picture" caption.

## My re-run output

All generated output stayed outside the repo. The lane stayed clean (`git status --short` count 0).

| Command | Result |
|---|---|
| `run_five_rack_check.py --out <review dir>/suite` | `FIVE_RACK_PASS checks=100`, `FIVE_RACK_SUITE_PASS source_checks=75 failed=0`, `BUILD WARNINGS []`, 175 PASS lines, exit 0 |
| `run_diagram_check.py` | `DIAGRAM_PLAN_PASS checks=1330` (base via suite: 1310) |
| `run_diagram_check.py --design …/astra/stand.json` | `DIAGRAM_DESIGN_MATCH_FAIL lines=762`, same with base plan: pre-existing |
| `run_typecheck.py --mode prod` | `TYPECHECK_CLEAN`, 6 errors = baseline 6 |
| `run_standmap_check.py` | `176 passed, 0 failed`, `STANDMAP_OFFLINE_PASS` |
| `run_headland_check.py` (unmodified) | **FAIL** `HEADLAND_SOURCE_AND_OFFLINE_FAIL: only four authorised display files changed…` |
| Same, out-of-repo copy with the stricter guard | `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`, `HEADLAND_SOURCE_AND_OFFLINE_PASS` (diagnostic, not the real gate) |
| `check_sources.py` (T2) | **`T2_SOURCE_TALLY PASS=38 FAIL=1`**, pre-existing inventory failure |
| `git diff --check 1b68ddcd e7bda1f2` | clean |
| **Real `ScenarioOneSuggestedRackLayout.Get` on the archive** | `trees=1344 routes=5`, `maxDisplacementVsRealGet_m=0.00000`, `REAL_GET_MATCHES_FIXTURE` |
| **Figure C# compiled against installed Unity 6000.6.0f1 assemblies** (`UIElementsModule`, `CoreModule`, others; stubs for `UiKit` and `ForestSaveData` only) | `Build succeeded`, 0 warnings. A planted `p.lineCapp` was rejected, so the binding is real. **This is a compiler pass, not a Unity import or compile.** |

### Mutations reproduced (scratch clone outside the repo, my own driver `mutate.py`)

| Mutation | Result |
|---|---|
| A1 regularised spacing (author's) | Caught, 9 fails |
| A2 feeder on south edge (author's) | Caught |
| A3 cleared corridors (author's) | Caught, 6 fails |
| R1 rack 3 shifted +0.5 m (inside the 1% tolerance) | Caught by the snapshot and gap checks |
| R2 two entrance IDs swapped | Caught |
| R3 figure flipped north-down | Caught, 23 fails |
| R4 rack width doubled | Caught |
| R5 title wording changed | Caught |
| R6 fake northern spine | Caught |
| R7 `OnExistingSurface` disabled | **Survived** (m-2) |
| R8 approach thinned to 3 points | **Survived** (m-3) |
| R9–R12 painter changes | **Survived** (M-2) |
| R13 one palette byte changed | **Survived** the author's suite. Its lesson dump omits colours; my reflection dump includes them (I did not run R13 against it) |

Tally: 16 mutations, 9 caught, 7 survived. The author's full 7-mutation list also passed inside my suite run. Log: `reviewer-mutations.log`.

## What I could NOT verify

Not run in this review, and not claimed:

- Unity import or compile in the 6000.6.0f1 Editor, and any actual rendering
- 1280×720 and 1920×1080 captures; M-1 is arithmetic from older captures
- grayscale and colour-blind legibility, stripe contrast
- text metrics and wrapping; the 16 physical px floor as measured
- the two-reviewer 4/4 recognition test and user clarity
- native input, Objectives navigation, scroll behaviour
- save/load, old saves
- Legacy40 or Reference Future v1 gameplay; B-1 is from source reading
- `UxPedagogyShots`, `GuidedOnboardingVerification`, `StandApproachVerification`, `StandMapParity`
- Stand Map versus 3D agreement as rendered
- frame pacing and mesh cost
- Player or Windows build
- the #297 notes m-1..m-5, which stay carried to the Unity gate

The author's PIL preview (`five-rack-preview-PIL-NOT-UNITY.png`) was looked at only for orientation. **I used it as no evidence.**

## Final verdict

**NEEDS CHANGES** (source-only). **Not Manager approval.** The Manager #200 decides APPROVED / NEEDS CHANGES / REJECTED.

The figure's data is authentic and its source geometry is correct, so it should be repaired, not rejected. In order:

1. **Manager rules on B-1** (recommended: a one-line scope extension for the call site), and on the #297 guard in the same decision (9a).
2. **Trim the notes by at least 4 lines** (M-1) and name the confirmed state (M-3), inside the allowed files.
3. **Add painter guards** (M-2), and address the minor items where cheap.
4. **Targeted re-review** of that delta, then the Manager decision, then a fresh Unity lease. No Unity before that.

No production or test file was changed by this review.
