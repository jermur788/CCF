# Implementation and qualification record

Feature: SCENARIO-ONE-THREE-PAGE-ONBOARDING-V1. Status: VERIFYING.
Source checkpoint: 0805ffb9b6b1153442b3530d7404be5847ce9c41.
Tested production checkpoint: 89130b7919367e22865c1b03a20d1aab56007fbf.
Branch: feature/scenario-one-three-page-onboarding-v1, U1.
Later documentation commits do not change the tested runtime bytes.
Authority: user specification plus explicit mapping, V2 treatment/forestry and temporary-clean-file approvals in T3 thread 0c55a504-715b-46b5-a3a2-e8ef3639bbff. Qualified V2 context reference: d5f11f865dc9123ac3ff0bab571e0402ad03d276; current direct user instructions apply.

## Result

Three live UI Toolkit pages replace the old introductory popup and its More detail route. MenuHelpView retains the existing modal contract and delegates only onboarding rendering/page state to ScenarioOneOnboardingView. Other contextual help retains its existing short/detail behavior.

Start New opens definition → generic five-card process → Scenario One → forest. Back reverses one page. The primary action is Next/Next/Begin. Arrow keys move forward/back; Escape moves back without skipping Page 1, and F1 cannot replace onboarding. Buttons are keyboard focusable. The existing save Continue path explicitly dismisses transient onboarding, closes help and loads through its existing validation/API. Generic walking first-use help no longer auto-opens onboarding after a load, including with reset preferences.

Panel geometry is authored against the exact 1672×941 reference coordinates and scales uniformly at 720p/1080p. Text and controls are live elements; the UI is not one nonfunctional screenshot. The exact locked PNG bytes serve as forestry artwork atlases. The blurred backdrop is the reference's static illustrative forest, not a real-time blur of the player's camera. Scenario dioramas use soft-edged texture frames to avoid background seams; the original forest/species pixels are retained.

All UI colours resolve through existing V2 USS variables. No onboarding palette tokens, fonts, packages, managers, save fields, scenes or prefabs were added. The existing licensed Liberation Serif major-title font and the existing body font remain. Illustrations retain natural forest colours.

## Reference/copy lock

Mapping and treatment were explicitly approved before production. Final Page 1/2/3 PNGs are immutable and SHA-256 listed in references/reference-manifest.md and final-hashes.json. production-copy.md is the exact text authority; ScenarioOneOnboardingCopy.cs supplies the production strings. Generated raster lettering is not used as runtime copy.

Working-set footprint for three originals, three revised references and the three identical runtime copies: 19,981,504 bytes. Runtime PNG copies equal the revised references byte-for-byte, so Git can reuse their blobs. No LFS/storage-policy or external-dependency change.

## Executed verification

Durable evidence: /media/jer/Files/CCF/Verification/ScenarioOneThreePageOnboardingV1-20261025/.

- Runtime C# type check: 161 sources, 116 references, zero errors (compile-03). Actual Unity import/compile and execution also passed on final candidate.
- Final run-04: ONBOARDING_PASS checks=235 failures=0 shots=6; Unity exit0; driver normal completion in 95.64 seconds; no cleanup failures.
- Normal startup, Start New, all forward/back transitions and Begin pass at both sizes.
- F1/Escape/arrow behavior passes via queued New Input System events. Session menu pause/restore during onboarding and in the forest passes.
- All page modal/player/cursor checks pass; Begin restores walking input and locked/hidden cursor.
- Actual save and real startup Continue pass in isolated XDG storage after first-use preferences are reset; no replay and world preserved.
- Pure UI world hash before/after: 30B1C4735B673ED2 → 30B1C4735B673ED2. No ecological year was advanced.
- Six actual Unity captures: every Page 1/2/3 at 1280×720 and 1920×1080; all label-height and button-center hit-target checks pass.
- No monitored runtime errors/exceptions/assertions. The Editor log retains a licensing access-token refresh diagnostic before execution; valid cached licence, imports and normal exit are recorded. No game exception is attributed to that diagnostic.
- Six APPROVED REFERENCE | ACTUAL UNITY RENDER sheets in comparisons/ preserve actual pixels at native size and enforce final-reference hashes. Primary inspected all six.

Runs 01/02 found the Page 2 regeneration statement's text box too short. Run 03 found the tracked MANAGEMENT label wrapping its last letter. Production geometry was corrected without removing words or weakening the checks. Run 04 passes both resolutions. Earlier failing runs are retained as iteration history.

## Forestry and rendered review

Page 1 first plot remains uniform, closely spaced and conifer dominated with plantation character. Middle plot visibly retains mature Sitka around selective openings with beginning regeneration. Final plot adds broadleaf crowns, smaller/younger trees and greater structural variation. The three stages are distinct; broadleaf additions are not generic conifer planting substitutes.

Page 2 planting stock shows a lobed-leaf Oak and oval-leaf Beech. The revised page contains no key prompts, crop/fell markers, dashed rack instructions or Scenario One assignment.

Page 3 goal retains conspicuous mature conical Sitka with recognisable foreground lobed Oak and unlobed oval Beech, intermediate broadleaves and regeneration. All three species are present illustratively. Spacing/sizes differ from the starting plantation and cover remains; no clear-fell/native-woodland replacement is implied. Pastoral/Atlantic imagery is compatible with the intended Irish illustrative setting. This is an illustration review, not botanical or empirical outcome validation.

Primary assessment: composition, illustration placement/proportions, hierarchy and information structure substantially follow the locked references at both sizes. The following rendering differences are explicitly submitted for Manager acceptance rather than silently treated as exact identity:

- The raster references' typefaces/weights and line breaks differ from live V2 text. Runtime uses the qualified regular serif title and existing sans body; it does not reproduce the raster's occasional serif body. This follows the approved V2 authority.
- Small tree/leaf/sprout/book/process icons are native Painter2D line drawings in the V2 accent. Their precise contours differ from the raster icons. Forestry illustrations themselves are the exact locked assets.
- Runtime panels/rules/buttons use actual qualified tokens, so their solid charcoal and restrained grey/teal treatment differ from the generated raster's residual blue-black gradients. No source gradient was sampled into a new token.
- The blurred forest backdrop is static illustrative artwork. No new real-time camera blur or render-pipeline change was made.

No material composition, page-purpose, behavior or forestry-image substitution was adopted. A later transparency-generation trial was rejected because it changed the artwork; it is not consumed. Manager may still judge the icon/text rendering differences unacceptable under the strict fidelity criterion; that gate remains open.

## Scope, restoration and limits

Production diff is confined to Assets/ForestPrototype/UI; remaining files are this spec/reference set and the focused disposable verification tools. No gameplay, simulation, ecological coefficients, save format/history, economy, progression, RNG, input bindings outside startup navigation, geometry or frozen Reference Future changes. No package, ProjectSettings, scene, prefab or pre-existing .meta changed. The full simulation suite was not rerun, as required by the UI-only scope.

The user explicitly approved temporary committed versions of the four pre-existing settings/material edits for clean-lane gates. Original bytes were preserved before any temporary change, restored afterward and all four SHA-256 values match. restoration-result.json records this. No other user's work was changed. Unity lock is free, no Editor remains, and no copied harness/.meta/licence remains. The qualified V2 source branch stays at0805ffb.

Test limits: button callbacks plus actual UI hit targets and queued input, not OS-native pointer/key delivery; Editor, not standalone Player/performance qualification; only the specified 720p/1080p sizes. Older single-popup introduction fixtures are superseded by this feature-specific gate for onboarding; no claim is made that the complete historical suite was rerun.

## Acceptance and handoff

Worker readiness: ready for visual acceptance review. Automated functional/render generation and primary forestry inspection pass. Formal G2/G3 acceptance where applicable, G4 integration, independent review and user player test are not claimed or waived. Spec remains VERIFYING, not VERIFIED. Final verdict: NEEDS PRODUCT DECISION — Manager acceptance of the concrete side-by-side result. The requested THREE-PAGE ONBOARDING QUALIFIED — READY FOR USER PLAYER TEST label is withheld until that acceptance.

Final checkpoint, complete changed-file list, exact copy/reference hashes, comparisons, dashboard publication/readback and next actions are in the durable HANDOFF.md at the evidence root. Dashboard uses the supported state updater/publisher, never manual generated-state edits.
