# Sen-1 — #590 author correction of #588 independent Compact graphics/asset DRAFT review

**Date:** Monday 16 November 2026.
**Owner/actual author:** Scenario One Manager [#424](https://github.com/jermur788/CCF/issues/424), OpenAI ChatGPT GPT-6 using connected GitHub/browser only, worktree **NONE**, Unity/OS lease **NONE**. Actual model/effort selector beyond GPT-6 not independently attested.
**Task:** [#590](https://github.com/jermur788/CCF/issues/590), registered [RUNNING] with dedicated docs-only branch `docs/sen-1-590-588-graphics-draft-review-repair-20261116-v1` and starting reviewed #587 tip `bdfd4fdeafce7d5431ec2c9330658330cdb090f0` **before author edits**. Only changed paths are allowed Markdown. Original #587 branch, Opus #588 review branch and live main source remain untouched.
**Independent source:** [#588 Opus 5.5 review](https://github.com/jermur788/CCF/blob/review/sen-1-588-compact-graphics-spec-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-588-INDEPENDENT-COMPACT-GRAPHICS-SPEC-REVIEW-RESULT-20261116.md), **NEEDS CHANGES** for DRAFT quality, 0 blockers / **2 major** / **6 minor** / **2 nit**. That verdict is accepted as source-graded documentation-review evidence; not a Unity defect report or approval of art.
**Correction outcome:** **AUTHOR FIX PUBLISHED — AWAITING INDEPENDENT CHANGED-LINES RECHECK.** The four-file spec remains **DRAFT / NOT APPROVED**; two important production design choices stay **BLOCKED** until real owner/Manager spec reconciliation, not marked magically fixed by prose.

## 1. Major corrections

| #588 finding | Document change and honest remaining gate |
| --- | --- |
| **M-1 — old 2.6m planks overlay the source Route A and H1** | `spec.md` FR-09, GQ-07 and AC-02/05 explicitly acknowledge that all **8 proposed Route A points match old `ForestTest.unity` plank centres**, the 32 along the route and 20 across H1; old 52 are 2.6 m wide and conflict with the proposed 1.2 m walking core/planting. **BLOCKING GQ-07 architecture choice:** **A** separate Compact-only scene/hierarchy without historical planks/approach/headland source consumers (preferred **draft**), or **B** shared scene with profile-gated **`Renderer.enabled`-only** exclusion, never SetActive/Destroy, with exact clone creation/order/rebuild on `StandApproachPath.Version`, Rack toggle, Legacy40 load, and parent compact **no-tree corridor/Stand Map/prescription/coverage** sources reconciled. Neither solution chosen, implemented or Unity proven. `plan.md` Phase0/Table and `tasks.md` T-02/T-04 block for scene and source reconciliation. No historical source object is altered here. |
| **M-2 — failed shelter already overlaps 1.2m path** | FR-05/15/GQ-02, AC-02/06, plan/T-02/T-04/T-06 and checklist NEG-02 now distinguish **upright-only** #583 B2 +0.121m from **failed visual** (direct `ScenarioProtectionVisuals` source: 28° tilt, +0.25 m world-x shift). #588's source-plane computation found failed O1 **0.141m inside the 1.2m physical core** (and 0.341m with illustrative 0.20m verge); failed O2 **0.155m inside the verge**. Renderer removes current cylinder collider, so these are **visual overlaps, not measured player-physics blocks**. Final guard/failure envelope or signed route/site redesign is a **known source FAIL / product-technical gate**, **not** an approved fix, and may not change Browse protection effects, year records, tree positions or the owner's 2 Oak+2 Beech count. |

**Implementation interpretation:** Do not order a worker to construct the new path over old 2.6m planks, and do not use “1.2m + upright shelters fit” as Unity/asset GO. The first production author needs a **signed Compact scene/architecture** and an **accepted failed-guard/source-footprint design**; until then the world manifest and graphics spec remain UNSIGNED/DRAFT. A separate Compact scene is a Manager-preferred option **only**, not the user's accepted scene architecture.

## 2. Six minor findings and two nits

| Finding | Document repair |
| --- | --- |
| **m-1** wrong path/zero-width char | `plan.md` now names `Assets/ForestPrototype/ScenarioOne/ScenarioHabitatVisuals.cs` and correct folder for clearance/preview classes; `VegetationSites` retyped without U+200B. |
| **m-2** missing vegetation verifier/determinism/rush | FR-11, plan Phase4, T-05 and checklist require **rendered + suppressed = source visual count per cell/kind**, unchanged ecology and clearance, **deterministic source `Jitter(cell,kind,plant,axis)`** (or verified stable alternative), defined display footprint including **rushes**, and a **Compact-only** assertion addressing the existing `ScenarioHabitatPresentationVerification.VerifyYear0GrassSupport` visible-count condition. Existing E80/Legacy/Reference verification MUST remain enforced, not weakened for green. |
| **m-3** W soft edge 0.35m versus tentative <=0.20m at G1 | FR-16/plan Phase3/T-04/checklist require **re-proportion/taper W or N2 near G1** before native comparison; W's Blender appearance and grayscale are NOT accepted player look. |
| **m-4** 2–6m plant leaf-art overreach | FR-14/AC-06/T-06/checklist separate real juvenile art **species distinction around 2–3m** and **4–6m findability using an owner-approved marker/inspection affordance**, not leaf inflation or growth edits. |
| **m-5** mixed ISO/save T-03B names | `spec.md` now clearly refers to signed `specs/scenario-independent-save-isolation-v1/` **ISO-58** teaching gate and **ISO-63** native verification, while `T-03B/T-04/T-05` are **separate Scenario One save orchestration #544** stages, NOT signed-spec or graphics-spec tasks; Compact activation BLOCKED. |
| **m-6** no trace table / width reasoning | `spec.md` now contains **FR→AC→task→checklist** grouping covering every FR-01..FR-20; FR-04 documents B2 **1.8m upright +0.021m without a verge**, **2.4m O1+B2 centres inside** with all 4 upright guards intersecting, source 1.2m candidate but failed guard/source-plank blockers. |
| **n-1** missing FR-08 / APP acronym | Checklist G573-M4 lists **FR-08 and FR-17** for camera/URP, and task summary says **APPROVED** not APP. |
| **n-2** reviewer packet 1.8m directional mistake | The **#587 branch copy of the original #588 review packet** was corrected on this **new author repair branch only**: a 1.8m core is **too wide** to retain a meaningful G1 shoulder, not “too narrow.” Original published #588 report and original #587 branch remain immutable and history-citable. |

No new functional requirement/acceptance/verification ID was inserted or deleted. The scene/plank choice is recorded as a **blocking architectural alternative**, not a hidden approved route.

## 3. Exact paths, what was checked and untouched boundary

The author changed exactly **five pre-existing allowed Markdown** relative to the original #587 head before this result:
- `specs/scenario-one-compact-graphics-v1/spec.md`
- `specs/scenario-one-compact-graphics-v1/plan.md`
- `specs/scenario-one-compact-graphics-v1/tasks.md`
- `specs/scenario-one-compact-graphics-v1/checklists/requirements.md`
- `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-588-INDEPENDENT-COMPACT-GRAPHICS-SPEC-REVIEW-PACKET-20261116.md` (**one wording nit**).

After this result is written, diff against pinned #587 author head must show those five **MODIFIED Markdown** plus **this one ADDED Markdown** and **no other path**.

**Personally checked from live GitHub:** full #588 report/readback; current `main` `ScenarioProtectionVisuals.cs` 28°/+0.25m failed-state transform, removed cylinder collider; `ScenarioHabitatPresentationVerification.cs` grass-count assertion; source paths of old presentation and #588 scene-plank research witness (latter **reviewer-reported source inspection**, not independently reparsed Unity YAML by this Manager); all four revised files individually fetched and source compare. A pre-result compare confirmed exactly the five allowed changes, verified **20 FR IDs**, **12 AC IDs**, **66 unchecked** original acceptance entries, **0 checked**, with original `DRAFT` status, correct path and no zero-width character. Updated handoff readback and final compare follow as the last author checks.

**Not done:** no original #508 CSV re-execution, Blender render viewing, Unity/Editor/collider/playtest, changes to Asset binaries, `ForestTest.unity`, material, scripts, saved data/Prefs, project-wide Decision Log/Milestone, accepted three-profile save schema, old Legacy40/Enlarged80/Reference assets. No native source tree count/planting survival or path 3D/G1 clash has been verified by this Manager task.

## 4. Next task: short independent nonauthor changed-lines-only check

**Review claim:** [#591](https://github.com/jermur788/CCF/issues/591), **READY / NOT RUNNING** until author result and exact final repaired head are checked/pinned. **Difficulty LOW–MEDIUM**, best independent Claude Sonnet/Opus 5.5, or another competent nonauthor reviewer using source read-only tools. Prefer a different worker than original #590 GPT-6; #588 Opus is allowed to recheck its own findings if independent of author.
**Reviewer branch:** `review/sen-1-591-590-graphics-delta-20261116-v1`, check vacant and fork from **final #590 corrected head**, docs-only worktree NONE or isolated review checkout, no Unity/physical lease.
**Exact deliverable:** `Docs/Handoffs/ScenarioOne-Graphics/Sen-1-591-INDEPENDENT-590-COMPACT-GRAPHICS-DELTA-RECHECK-20261116.md`, **one added Markdown only**.

**Review only:** original #587 reviewed source head → final #590 author repair head, the eight findings and two nit corrections, 20/12 IDs + 66 original unchecked, old parent/child/legacy + signed saves unchanged, source-footprint GQ-07 and GQ-02 still **BLOCKED/UNDECIDED**. Check that the separate Compact scene is described as a *preferred draft*, not owner approval, that existing failed guard is a *known visual conflict*, not runtime physics pass, and that the vegetation verifier is **profile-scoped instead of deleted/weakened**. No full #588 re-review, no 66 native tests or #508, no Unity, no asset licence/owner look review. Review verdict **APPROVED / NEEDS CHANGES / REJECTED FOR DRAFT DOCUMENT DELTA QUALITY ONLY**; no production/Unity/spec approval regardless of PASS.

**Paste-ready dispatch (send complete direct GitHub URL):**

> Take CCF Scenario One **#591** as the independent nonauthor changed-lines-only reviewer of the #590 Compact30 graphics DRAFT correction. Follow the COMPLETE #590 author result: https://github.com/jermur788/CCF/blob/docs/sen-1-590-588-graphics-draft-review-repair-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-590-COMPACT-GRAPHICS-DRAFT-EIGHT-FINDINGS-CORRECTION-RESULT-20261116.md and original #588 Opus review: https://github.com/jermur788/CCF/blob/review/sen-1-588-compact-graphics-spec-20261116-v1/Docs/Handoffs/ScenarioOne-Graphics/Sen-1-588-INDEPENDENT-COMPACT-GRAPHICS-SPEC-REVIEW-RESULT-20261116.md . Register your actual reviewer/model, exclusive docs branch/worktree and exact final repaired starting HEAD on #591 before writing. Compare original #587 reviewed head to #590 repaired head and examine ONLY two majors, six minors and two nits. Verify 20 FR, 12 AC, 66 unchecked and unchanged production/source/save boundaries. Report exact single-Markdown verdict, close [HANDED-OFF]. No Unity/C#/Assets/scene/save/Prefs/Windows build.

**Manager next after PASS:** accept document delta quality only. **Do NOT yet approve Compact graphics spec**: first separately resolve **GQ-07 scene architecture**, **GQ-02 protection failed-envelope and corridor**, final world manifest, direct owner look decision, D-032 policy, parent/child teaching/finance/ISO-58, and independent source/Unity verification with a fresh machine lease. Model what needs native tests, don't run full project Tier3 until whole milestone is ready.

**User action currently required:** NONE to complete docs-only correction and independent reviewer dispatch. Direct owner look/storage/scene-content decisions will be needed before production; no new 30m/no-rack/2+2 question.

**Dashboard and Drive:** NOT VERIFIED.
