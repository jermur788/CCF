# Sen-1 #606 — Independent changed-lines recheck of the #604 Compact parent/planting source-bridge corrections

**Date:** 16 November 2026.
**Role:** independent nonauthor changed-lines reviewer, reporting to Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424).
- #604 and #600 were written by OpenAI GPT-6. I am neither of those authors nor a child of them.
- #601 was also performed by a Claude Opus 5.5 reviewer session. This recheck tests those findings against the #604 author's fixes; it does not review any content I authored.

**Actual reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`).
- Claude Code in a T3 Code host, on the owner's local Windows machine.
- Tools: git and the GitHub REST API (no `gh`). I cannot observe my own effort setting.

**Claim:** [#606](https://github.com/jermur788/CCF/issues/606). I posted the `[RUNNING]` registration ([comment](https://github.com/jermur788/CCF/issues/606#issuecomment-6918979566)) before my first tracked edit. It names the worker, branch, starting HEAD, worktree and the one-file allowlist.

**Review branch:** `review/sen-1-606-604-source-bridge-delta-20261116-v1`. Before I created it, it did not exist on origin. I branched it from the final #604 head.

**Starting HEAD:** `2ffcff9e895dd8d8e6d169c17dddb286d952c81a`, the final #604 tip pinned in the #606 issue body.

**Compare base:** the independently reviewed #600 final, `1b50879725c440221c806c28464ca658fc36fd6f`. The #580 base, `974ae8b9`, was used only to attribute #600-added lines.

**Worktree:** a separate, clean, docs-only local git worktree in a session scratch directory. I read production source with `git show origin/main:<path>`.

**Write scope:** this one Markdown file. I made no edits to specs, checklists, author handoffs, C#, scenes, Assets, ProjectSettings, saves/Prefs, live coordination, the dashboard or Drive. I held no Unity or machine lease.

## Verdict

**APPROVED FOR DRAFT DOC DELTA QUALITY ONLY. Two nonblocking minors and five nits are recorded below.**

**Severity count: 0 blocker / 0 major / 2 minor / 5 nit.**

- **Both #601 majors are resolved.** M1 is fully relabelled. M2 is now a coherent, explicitly unapproved DRAFT boot/title proposal that is consistent with current source.
- **Six of the seven #601 minors are fully resolved.**
- **m4 is partly resolved.** The new parent text is correct, but two #600-added table cells still say the opposite (this review's **m-A**).
- I found one new, small gap in the boot proposal (**m-B**).
- Fold both minors into the spec-approval revision, or into any earlier author pass. Neither one needs another review cycle by itself.

**This verdict does not approve either specification.**
- All eight files stay **DRAFT / NOT APPROVED**.
- The neutral boot scene, the boot-only picker and profile-bound Restart are **proposed architecture**. They are not approved, not implemented and not owner-accepted.
- This verdict grants no implementation, Unity, scene, save, ProjectSettings or Windows-build authority.

## 1. Preservation and scope checks (all PASS)

| Check | Method | Result |
| --- | --- | --- |
| Changed paths | `git diff --name-status 1b508797 2ffcff9e` | 8 modified spec Markdown files (the four parent and four planting files), plus 2 added handoffs: the #604 result and the #606 packet. No source, scene, Assets, ProjectSettings, save or binary paths. |
| Parent R lines | Extracted `- [.] R-NNN` lines at both commits, then `cmp` | **115/115 byte-identical.** 115 unique IDs, 115 `[ ]`, 0 checked. |
| Child CIP lines | Same method for `CIP-NNN` | **33/33 byte-identical.** 33 unique IDs, 33 `[ ]`, 0 checked. |
| Checkbox churn | Searched the diff for added or removed `[-*] [` lines | **0.** No checkbox or acceptance ID was added, removed or checked. |
| Deleted lines | Searched the diff for `^-` lines | 9. All are table/prose lines replaced in place by their corrected versions. No original #580 content was removed. |
| Status labels | Read every changed hunk | Every #604 section says DRAFT / proposed / not approved / no implementation authority. |

## 2. Finding-by-finding closure of #601

Line references are to tip `2ffcff9e`. "P" is `specs/scenario-one-compact-complete-loop-v1/` and "C" is `specs/scenario-one-compact-initial-planting-v1/`.

### M1 — ISO-58 mislabelled as identity preflight: **RESOLVED**

- #600 added 10 lines in the eight files that used `ISO-58` for identity. I checked each one against the tip: **none survives verbatim**. Each was rewritten to identity criteria **ISO-07/08/42/44/48/54/55 + ISO-15**, with ISO-58 marked separately as BLOCKED CG-TRIG.
  - P `spec.md:16, 51, 52`
  - P `plan.md:16, 20, 24`
  - P `tasks.md:14, 23, 26`
  - P `checklists/requirements.md:14, 23`
  - C `spec.md:15`, C `plan.md:15, 22`, C `tasks.md:15, 23`
  - C `checklists/requirements.md:15, 24`
- Every other `ISO-58` at the tip is one of two kinds. Both are correct:
  - the #580 header at line 3, which says "BLOCKED";
  - the OPEN-C1/C7 (CG-TRIG) decision rows.
- **I checked the substitution against the signed checklist** (`specs/scenario-independent-save-isolation-v1/checklists/requirements.md` @ `200add4e`):
  - ISO-07, -08, -42, -48: foreign/absent envelope and explicit-ID refusal before mutation.
  - ISO-44: Compact geometry/world/content key refusal and objective dispatch.
  - ISO-54: no mutation on refusal.
  - ISO-55: per-profile backup validation.
  - ISO-15: unchanged Legacy40/Enlarged80 numeric meaning.
  - ISO-58: "BLOCKED on CG-TRIG".
  - FR-07/W-03/H-05 are the signed explicit-foreign pre-mutation rejection rules.
  - The mapping is accurate.
- The #600 result handoff keeps the obsolete label. P `spec.md:16` and the #604 result §1 both supersede it as implementation guidance without rewriting it, which is the right choice for a historical record.

### M2 — boot/title scene source mismatch: **RESOLVED as a coherent DRAFT candidate**

Primary text:
- P `spec.md:14` and `spec.md:29, 32`
- P `plan.md:14`, P `tasks.md:14`, P `checklists/requirements.md:14`
- C `spec.md:15`, C `plan.md:15`, C `tasks.md:15`, C `checklists/requirements.md:15`

**Each source premise checked against `origin/main`:**

| Draft premise | Source evidence | Result |
| --- | --- | --- |
| `Bootstrap` runs once per launch and creates one `DontDestroyOnLoad` host | `StandaloneSessionMenu.cs:28-38`: a static `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]` method creates the host and calls `ShowStartupScreen()` | Accurate |
| `Open()` needs `ForestPlayer`, `ForestSaveController` and `ScenarioOneUiRoot` | `StandaloneSessionMenu.cs:81-82` finds all three. `Startup.cs:57-72` polls `Open()` for 600 frames, then hides and logs `CCF_STARTUP_SCREEN_UNAVAILABLE` | Accurate. A neutral boot scene would hit exactly this fallback unless the gameplay `Open()` is suppressed, which the draft requires. |
| Restart today reloads the active build index and shows no title | `StandaloneSessionMenu.cs:146-148`: `Close()` sets `startupScreen = false`, then `LoadScene(GetActiveScene().buildIndex)`. Bootstrap does not re-run. | Accurate. The proposal to "Restart without title" preserves current behaviour and drops only the active-index routing. |
| The first scene's `ForestStartingStand.Awake()` generates the world before the title | `ForestStartingStand.cs:52-67` | Accurate. With the proposed no-world index 0, no E80 world exists before profile choice. This satisfies signed W-01/H-06 ("validate before destination generation"). |
| The title's control rows depend on `forestUi` | `Startup.cs`, `ControlRows => ControlsReference.RowsFor(forestUi … RackPlanningV2Active)` | Consistent with the draft's requirement for a separate boot-only picker. |

**Coherence judgement:**
- One persistent host is suppressed in boot and then rebinds on gameplay entry. This is feasible, because `Open()` already re-finds its services each time it opens.
- The boot-only picker is separate from the host. Restart goes directly to the selected profile, with no active-index fallback. Return to Title is distinct and deferred.
- Together these are an internally consistent DRAFT candidate. They address every M2 repair item #601 asked for:
  - which scene is index 0;
  - whether that scene generates a world;
  - how the title opens without a gameplay world;
  - that Bootstrap fires once per launch;
  - what Restart shows (no title, for every profile).
- The ForestTest-first alternative is named, with its E80 pregeneration risk, and correctly marked **not selected** (P `spec.md:14`).
- The text never pretends that the neutral scene, the picker or any C# API exists.
- The previous #600 wording, "without silently changing the legacy first scene/index", is correctly replaced at P `spec.md:29`: ForestTest keeps its content, but its index may change.

One residual gap is recorded as **m-B** below.

### m1 — second `forest-save.json` call site: **RESOLVED**

- `StandaloneSessionMenu.cs:77` has `SavePath => …"forest-save.json"`. It is used at `:123` (Load offer) and `:125` (replace confirmation), and also at `Startup.cs:156`.
- The drafts now name both `ForestSaveController` and `StandaloneSessionMenu.SavePath` under the single #544 registry:
  - P `spec.md:16`, P `plan.md:16`, P `tasks.md:14`
  - C `spec.md:15`, C `tasks.md:15`, both checklists
- This matches signed FR-13 ("Both current filename call sites must use the same registry query").

### m2 — headland/approach injection: **RESOLVED**

- P `spec.md:18` names all of these:
  - `ScenarioOneHeadlandPresentation` (added at `ForestStartingStand.cs:54-55`);
  - `StandApproachExpected = true` (`:67`);
  - `CollectClearancePoints()`;
  - the `StandApproachPath.Version` cache coupling in `StandMapSource`.
- P `tasks.md:16` carries the task boundary.

### m3 — objective scaling: **RESOLVED**

- `ScenarioOneDefinition.cs:115-122` throws `ArgumentOutOfRangeException` when `!IsKnown(geometry)`, and returns `Mathf.RoundToInt(size²/40²)`. For 30 m that is 0.5625, which rounds to 1.
- All seven `Effective*` targets (`:124-130`) multiply by that factor.
- P `spec.md:20` requires a reviewed Compact-specific objective/area policy before any completion evidence.
- This matches signed ISO-44's "objective dispatch".

### m4 — reuse the geometry-keyed rack seams: **PARTLY RESOLVED (see m-A)**

- P `spec.md:18` is correct. It says to reuse the existing seams and lists the consumers to witness:
  - `RackPlanningV2Active` (`ScenarioOneManager.RackPlan.cs:37-40` requires `Enlarged80`);
  - `RackFirstStageActive` (`ScenarioOneManager.Racks.cs:55-62` requires `ScenarioOneRackLayout.AppliesTo`);
  - consumers: `ReadRacks`, `RackLayoutTracker`, `ControlsReference.RowsFor`, start guidance (`ScenarioOneManager.cs:89`) and `PrescriptionApproval`.
- P `tasks.md:16` agrees.
- But #600's "Compact-only bypass" wording survives in two table cells (**m-A**).

### m5 — batch planting controls and labour: **RESOLVED**

- `ScenarioOneManager.cs:875-890`: `SetPlantingExecution` rewrites every open `PlantJuvenile` order, and `SetPlantingShelters` rewrites every open exact-position one. Each order still stores its own `installShelter`.
- The drafts state that per-tree mixed protection and actor choice is **unimplemented desired scope**, or that the first lesson is batch-only:
  - P `spec.md:20`, C `spec.md:17`, C `plan.md:17`, C `tasks.md:15`, C checklist line 15.
- **Cost figures checked against `ScenarioOne.asset`:**
  - Stock: 2 × 550 c + 2 × 450 c = **€20**.
  - Shelters: 4 × 500 c = **up to €20**.
  - Labour: `contractorHourlyRateCents: 4500` × 4 × 10 min = **€30**.
  - Owner alternative: **40 owner minutes**.
- The figures are correctly labelled as quotes, not settled charges.
- There is no claim of free planting.

### m6 — scene address versus signed registry: **RESOLVED**

- The scene path, scene name and build index are each stated to be a **derived destination**, never a save identity or routing key. Only the #544 owner may extend the one registry.
  - P `spec.md:14, 16, 29`, P `plan.md:16`, P `tasks.md:14`
  - C `spec.md:15`, C `plan.md:15`, C checklist line 15
- This is consistent with signed spec §4.1 at line 143 ("Display labels/ordinals/scene names … are never routing keys").

### m7 — startup acceptance gap: **RESOLVED within the no-new-ID scope**

- Explicit future prose witnesses are mapped to existing unchecked IDs:
  - P `spec.md:22`, P checklist lines 12-16 (R-012…024, R-080…092);
  - C checklist lines 13-15 (CIP-023/027/032/033, plus CIP-014…022 for the planting controls).
- Each place says it is not a test PASS. Each also says that formal new acceptance IDs need a later APPROVED spec revision.
- No ID was invented.

### Nits n1–n3

- **n1: partly resolved.** The R-074 performance item is no longer filed as owner visual approval: P checklist line 26 now reads `R-001–004, R-075–080`. The remainder is carried as **n-1** below.
- **n2: not changed in the eight files.** `0.141` appears 6 times at both #600 and #604. #604 added no new copies, so drift risk did not grow. The author-result claim about this is inaccurate; see **n-2**.
- **n3: not applicable to the docs.** The #604 branch again uses repeated commit messages (8×, 7× and 8× identical subjects). This is harmless, and history should not be rewritten.

## 3. New findings from this recheck

### m-A (minor) — Two #600-added cells still contradict the corrected m4 seam reuse

- **P `tasks.md:24`, SC-04 proof cell:** "`PrescriptionApproval` rack dependency **explicitly bypassed for Compact**, not globally removed."
- **P `plan.md:23`, P3 row:** "suppresses only Compact rack/MFM UI and `PrescriptionApproval` entry."
- Both lines came from #600; neither exists at #580 `974ae8b9`. #601 m4 targeted exactly this framing.
- The new P `spec.md:18` and P `tasks.md:16` say the opposite: reuse the existing geometry-keyed negative seams, and invent no Compact-only bypass.
- **Risk:** an implementer reading the SC-04 table could add a Compact branch that bypasses `PrescriptionApproval`'s Rack V2 check, which would let prescriptions run without a rack. Current source already refuses them in any non-Enlarged80 world.
- **Repair (text only):** reword both cells to "Compact geometry falls through existing non-Enlarged80 seams. Witness that `PrescriptionApproval` refuses or omits, with no Compact-specific bypass."

### m-B (minor) — The boot picker's Continue preflight must be scene-independent, but the drafts do not say so

- **What the drafts already require:**
  - The boot-only picker is independent of `ForestSaveController` (P `spec.md:14`).
  - Continue must preflight before scene load (P `spec.md:31`, unchanged #600).
- **Current source:** Continue visibility uses `ForestSaveController.CanLoad`, an instance method on a scene object that validates against the **current** scene (`ForestSaveController.cs:240`; confirmed by #601).
- **The gap:** no changed line states the consequence. The #544 registry admission/preflight used for Continue and New must be callable from the no-world boot scene, with no scene-bound `ForestSaveController`.
- **Risk:** without that sentence, an implementer could load the destination scene first and then call `CanLoad`. That would reintroduce generation before validation, against signed W-01/H-06.
- **Repair:** add one clause to P `spec.md:14` (or `:16`) and C `spec.md:15`: "boot Continue/New availability and preflight use the #544 registry without any scene `ForestSaveController`; destination loads only after a pass."

### Nits

- **n-1:** The crosswalk ranges are still loose.
  - R-073/R-074 (asset provenance, performance) are now in no row.
  - The startup/save row and the new startup-witness note (P checklist lines 14, 23) still span R-080…092. That includes R-081 (no acceptance from static audit only), R-082 (dashboard publication) and R-087…089 (learning/help keys).
  - The owner row R-075–080 includes R-076/R-077 (lane/test tiers), which are not owner acceptance.
  - Suggested fix: give R-073/074 a performance/assets row and narrow the startup/save witness to R-012…024, R-083…086, R-090…092.
- **n-2:** The author result's n1–3 row says it "reduced copy-pasted source-number prose to a parent spec source of truth". The diff shows no such reduction: O1/O2/B2 figures appear the same number of times, and there are no deletions beyond the in-place row rewrites. The eight files are unaffected; the inaccuracy is only in the handoff narrative.
- **n-3:** C `plan.md:15` has broken nested bold, `**neutral no-world title picker **independent of gameplay-bound menu.Open()****`. It renders incorrectly.
- **n-4:** P `spec.md:20` says both setters act on "all four open **exact** planting orders". Only `SetPlantingShelters` filters on `exactPosition`; `SetPlantingExecution` rewrites every open `PlantJuvenile` order. The child wording at C `spec.md:17`, "all open planting orders", is the precise one. This makes no difference for a four-order Compact lesson.
- **n-5:** P `tasks.md:22`, the SC-02 row (unchanged #600), still says "preserve `ForestTest` startup compatibility". Under the index-0 boot proposal this is ambiguous. It is better read as "preserve ForestTest content and historical-profile start/restart behaviour", consistent with P `spec.md:29`.

## 4. Authority and protection checks (packet item 8): PASS

- **Owner decisions unchanged** (P `spec.md:24`, unchanged):
  - separate 30×30 m no-rack Compact scene;
  - W worn-earth path, 1.2 m core subject to clearance;
  - real visible guards;
  - no fresh H1 stumps;
  - neutral daylight;
  - **2 Oak + 2 Beech count**.
- **Still described as not native / unsigned:**
  - the world manifest;
  - the 14 H1 preworld omissions and 8 buffer stems;
  - the four sites;
  - the O1/O2 guard failures;
  - save and path.
- **Gates still open:** D-032, ISO-58/CG-TRIG BLOCKED, D-083 (not on main), OPEN-C5/completion policy and the third profile. None was decided by #604.
- **Still protected:** Legacy40, E80, Rack V2/MFM, frozen Reference Future v1, real v1–v24 saves and the owner Windows-build gate.

## 5. Not performed / not verified

- I did not run Unity, compile, run tests, render, check collisions, load or save, create a scene, or build for Windows.
- I made no edits to specs, checklists, author handoffs, source, Assets, ProjectSettings, live coordination, dashboard or Drive.
- Source statements come from reading `origin/main` (and the signed save spec at `200add4e`). They are not runtime verification.
- As the packet scoped, I did **not** repeat the full #601 review. I did not re-run the #508/#583 geometry (189/14/8/175, O1/O2/B2 margins), and I did not perform any of the 148 native checks.
- I did not re-inspect unchanged #580 base content, except to attribute the lines cited in m-A and n-5.
- I did not verify the boot proposal's runtime feasibility beyond source plausibility.

## 6. Recommended routing for #424

1. Accept **DRAFT doc delta quality only**. Fold m-A, m-B and, optionally, n-1…n-5 into the spec-approval revision, or into any earlier bounded author pass. No separate re-review is needed for those text edits alone.
2. These remain separate gates:
   - owner and technical acceptance of the neutral boot scene and boot-only picker architecture;
   - spec APPROVED status;
   - the signed world manifest;
   - OPEN-C1/C5/C7;
   - D-083 and D-032;
   - the #544 implementation;
   - ISO-58/CG-TRIG;
   - native Unity qualification;
   - the owner's personal playtest.
3. No Windows build.

**Canonical mapping:** APPROVED (DRAFT DOC DELTA QUALITY ONLY), 0/0/2/5. No spec status change. Dashboard/Drive: NOT VERIFIED.
