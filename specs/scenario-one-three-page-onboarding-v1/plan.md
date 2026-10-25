# Implementation plan

Feature: SCENARIO-ONE-THREE-PAGE-ONBOARDING-V1
Specification: APPROVED FOR DESIGN LOCK + IMPLEMENTATION
Execution state: VERIFYING — ONBOARDING REFERENCES LOCKED; COPY LOCKED
Base: 0805ffb9b6b1153442b3530d7404be5847ce9c41
Work inspected: /media/jer/ZX20/CCF-U1, feature/scenario-one-ui-visual-system-v2
Coordination anchor: /media/jer/ZX20/CCF-Primary, ops/workflow-foundation-v1

This plan creates no product authority. Follow spec.md and resolved reference lock. Artifacts now reside on the new feature branch in U1; original preflight staging remains historical evidence.

## Smallest implementation

Reuse the existing Scenario One UI root, shared USS tokens and existing startup entry points. Add one focused three-page onboarding view and one copy source; replace only the old introduction path. Keep unrelated first-use help intact. Do not add managers, dependencies or save fields.

Expected owned files (proposed, inspect again before writes):
- specs/scenario-one-three-page-onboarding-v1/**: specification, copy, plan, tasks, requirements, references and evidence links.
- Assets/ForestPrototype/UI/ScenarioOneOnboardingView.cs (+ matching .meta): bounded page state and layout.
- Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs (+ matching .meta): production copy from frozen text.
- Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs: mount/navigation/modal input routing.
- Assets/ForestPrototype/UI/MenuHelpView.cs and MenuHelpView.Text.cs: remove superseded introduction/More detail route, retain other menu help.
- Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss: onboarding geometry and layout using existing variables; no duplicated palette.
- Assets/ForestPrototype/UI/Resources/Onboarding/**: approved forestry imagery and metadata.
- Tools/Verification/ScenarioOneThreePageOnboarding/**: scoped disposable verification and comparison tooling.
- Existing startup/session harnesses only where expectations genuinely require the new three-page sequence.

StandaloneSessionMenu.Startup.cs already calls ShowScenarioIntroduction() after Start New and takes a separate Continue load path. Prefer retaining this behavior. Change that file only if verified necessary.

## Phase 1 — lock references/copy

Source composition mapping is explicitly user-approved and locked in the reference manifest. The user approved the V2 treatment and required species corrections; revised files are inspected and hashed. Preserve supplied originals, prepare exact final mockups, inspect species progression, record final SHA-256 values, then state ONBOARDING REFERENCES LOCKED. Freeze production copy from the written spec, never OCR. Exact production copy is in production-copy.md; COPY LOCKED.

## Phase 2 — establish task workspace/ownership

Use a new task branch from exact base; preserve qualified V2 source branch. Reconcile live active-tasks ownership with the authorised maintainer before production writes/Unity. Preserve all unrelated changes byte-exact. U1 currently has four original modifications: shared VS Code settings and three Woodland_Forbs materials. The coordination anchor has unrelated AGENTS/active-tasks modifications and untracked StorageArchitecture/ScenarioTwo audit. None is owned by this task.

## Phase 3 — flow and views

Start New → page index 0 → 1 → 2 → enter forest.
Back at page index 1 → 0, 2 → 1.
Continue existing save must never invoke new-game onboarding.
Block gameplay input and pointer propagation while onboarding is open; restore existing cursor/player/modal behavior on final Begin. Route keys deliberately so Esc/F1 cannot accidentally bypass all pages; resolve any optional skip policy against accepted startup behavior. No gameplay remapping.
Use the locked five-card process composition for generic Page 2; remove all scenario mechanics, controls, key prompts and rack instructions.
Keep the before/after/process/three-column Scenario One composition for Page 3.
Preserve relative panel/image proportions at both resolutions; use actual text labels and accessible buttons, not one screenshot as a nonfunctional interface.
Resolve blurred backdrop fidelity without introducing a new theme or touching unrelated scenes.
Remove only the superseded introduction and its own More detail path.

## Phase 4 — qualification

Tier 1: actual Unity import/compile, focused flow smoke.
Tier 2: authorised scoped onboarding functional, startup/session and six rendered captures; normal machine-wide Unity lock and clean exact candidate required. No full simulation chain unless non-UI behavior changes.

Functional: use real Start New, navigation buttons, Back, Begin and actual save Continue path. Verify page/modal/player/cursor state, no exceptions and unchanged world state across UI navigation. Test an isolated save/config only after inspecting backup/restore behavior; do not overwrite the user's save.

Visual: capture Page 1/2/3 at 1280×720 and 1920×1080. Produce six APPROVED REFERENCE | ACTUAL UNITY RENDER sheets with hashes and unchanged actual captures. Review composition, density, all text bounds and image semantics at both sizes. A mock browser rendering is not Unity evidence.

Forestry: explicitly check every §13 Page 1 and Page 3 criterion, including recognisable Oak and Beech and retained Sitka. Mark unknown identity NOT VERIFIED, never PASS.

## Phase 5 — handoff

Record source/final SHAs, copy, changed files, reference hashes, all six comparisons, forestry checks, functional/startup/Continue results, deviations, no non-UI impact, remaining gates and actual dashboard publication/readback. No self-declared reference-fidelity pass without evidence. Manager/integration gates remain separate.


Implementation note: MenuHelpView owns the focused onboarding view and keeps the existing modal contract. Only ScenarioOneUiRoot navigation keys and the explicit Continue dismiss path change. The full approved PNGs serve as artwork atlases; all visible interface text/buttons are live elements. Shader/camera blur is avoided by reusing the reference’s static blurred backdrop, which is illustrative rather than the live player forest. No scene/prefab or package change.

Actual implementation adds OnboardingIcon.cs for small native line icons. Final runtime source89130b7 passed run-04 (235 checks/0 failures, six real captures, Unity0); all four original user files restored byte-exact. Manager acceptance and integration remain separate gates.
