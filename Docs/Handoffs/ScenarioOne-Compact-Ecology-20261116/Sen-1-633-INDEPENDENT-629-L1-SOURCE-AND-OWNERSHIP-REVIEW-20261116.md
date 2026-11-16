# Sen-1 #633 — independent #629 source and ownership review

**HANDED-OFF — technical source-only verdict: APPROVED. Ownership/integration admission: NEEDS CHANGES / HOLD. NOT UNITY-VERIFIED; NOT INTEGRATED.**

The three production changes correctly harden the currently supported 0/1 world geometry without enabling Compact. No production fix is required for this bounded source review. This technical approval neither repairs the missing pre-edit ownership registration nor authorises incorporation into the shared-save line, Unity execution or a build.

## 1. Actual reviewer, registration and scope

| Field | Actual value |
|---|---|
| Task | [#633](https://github.com/jermur788/CCF/issues/633), reviewing [#629](https://github.com/jermur788/CCF/issues/629) |
| Reviewer | OpenAI **gpt-6.1-sol, high**, T3 Code / Codex harness; Linux host `jer-laptop`. Independently assigned reviewer, not the author or its helper. No delegation. Account/quota pool is not exposed. |
| Registration | [RUNNING claim](https://github.com/jermur788/CCF/issues/633#issuecomment-6924860358), 2026-11-16 18:09 UTC, before review output edits. Worktree creation finished clean at 18:09:16 UTC; the claim was posted while checkout was finishing. |
| Review branch | `review/sen-1-633-l1-geometry-cross-family-20261116-v1` |
| Review worktree | `/media/jer/Files/CCF/Operations/CCF-633-L1-Review` — isolated, non-Unity review checkout |
| Starting HEAD / candidate tip | `46066e032b9d37282ff1ce629304492b5c0e6fa2` |
| Production change | `ea3e6eeaca8cef0b75a44de28beb6c3da6b71113` |
| Exact baseline | `feature/scen1-shared-three-profile-save-v2` at `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1` |
| Candidate context commit | `62e39ec621e38a84030c08b35a171e297056c9ed` |
| Repository write allowlist | **This Markdown only.** No canonical-context, coordination, production, serialized-asset, package or verification-source edits. |
| Reviewer receipts | `/media/jer/Files/CCF/Operations/CCF-633-Review-Evidence-20261116/` |

Refreshed live GitHub main coordination and #633/#629/#424/#544/#553, both registered Git-store worktree lists, and the physical CCF-Ops board/lanes before registration. No competing #633 reviewer claim or branch was found. The #629 child and #544 source worktrees were clean at their stated heads; this is a present-state observation, **not proof of historical absence of overlap**. Main coordination remains #424's single-writer record; the stale physical board contains neither source task. No board, lane or lease state was changed.

Primary's unrelated dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were left untouched. New receipts are on the Files ext4 mount, observed as `/dev/sdb4`; historical storage documentation's device letter is not treated as current device identity.

## 2. Exact diff and technical decision

Baseline → source commit is exactly **three C# files, +29/−1**. Source commit → candidate tip is exactly the #629 Markdown handoff. Baseline → candidate therefore has four paths, no other production or asset changes.

| Changed source at exact candidate | Finding |
|---|---|
| [StandGeometryModel.cs:35–73](https://github.com/jermur788/CCF/blob/46066e032b9d37282ff1ce629304492b5c0e6fa2/Assets/ForestPrototype/StandGeometryModel.cs#L35) | `StandSizeMeters` explicitly returns 40/80 for 0/1 and throws `ArgumentOutOfRangeException` otherwise. Axis/count/starting-target helpers inherit this refusal. `CellIndex` instead returns its established invalid-grid sentinel `−1` before sizing unknown geometry. This matches its caller's refusal contract. |
| [StandWorldBoundary.cs:69–81](https://github.com/jermur788/CCF/blob/46066e032b9d37282ff1ce629304492b5c0e6fa2/Assets/ForestPrototype/StandWorldBoundary.cs#L69) | The new guard precedes cache assignment, size lookup and every scene write. Rejected values do not poison `appliedModel`; repeated unknowns after either valid model keep being refused. Valid-model transform, tiling, ridge and camera code is unchanged. Exception to diagnostic coverage: a fresh instance starts with cache `−1`, so `Apply(-1)` returns at the old equality fast path without logging. It still makes no mutation and never accepts a world. |
| [ForestStartingStand.cs:74–96](https://github.com/jermur788/CCF/blob/46066e032b9d37282ff1ce629304492b5c0e6fa2/Assets/ForestPrototype/ForestStartingStand.cs#L74) | After dependency lookup, E80 takes its unchanged generator; only 0 reaches the unchanged Legacy lattice. Unknown returns before clearance collection, allocation of planting slots, spawning and canopy/seed recomputation. **Generate was not executed** in this review or the original evidence. |

For 0/1 the new predicates do not change the selected paths or arithmetic. `IsKnown`, `Latest`, `NewGameModel`, `ForSave`, labels, Editor override, save schema/profile availability, tree roster algorithms and frozen Reference data are unchanged. No new geometry exists. Old-world equivalence is supported by source comparison and the stub observations below; native save/load, generation and fingerprints remain unverified.

The guard in `Generate` covers that method, **not its entire `Awake`**: `Awake` can already add headland presentation, set `StandApproachExpected` and apply the known new-game policy before calling it. This patch does not repair or certify #544's separate T-03B before-construction/bootstrap work.

## 3. Caller graph and validation-order dependency

The source audit covered all C# geometry-helper references under `Assets`, not just the grep gate's narrow ternary patterns. There are no alternate writes to the nonserialized private ecology geometry field beyond its initial 0 and `ApplyStandGeometry` assignment.

| Consumer group | Why the new throw is safe today |
|---|---|
| `ForestEcologyController.cs:92–107` | `ApplyStandGeometry` rejects `!IsKnown` **before** storing the value, sizing, applying the approach or raising the geometry event. Its getter and event therefore expose only 0/1 through normal production calls. |
| `StandWorldBoundary`, starting stand, rack planner/editing/plan/prescription approval, `StandMapTreatmentReview` | They consume that accepted getter/event, or now guard locally. Three `: 40f` expressions in rack code are null-ecology defaults (half extent), not unknown-model fallbacks. |
| `StandApproachPath`, `LearningObjectivesView.Sequence`, E80 generator | Size/target calls use the literal accepted E80 constant. |
| `ScenarioOneSuggestedRackLayout.Get` | Returns an empty list unless geometry is E80 before sizing. |
| `ScenarioOneDefinition.TargetAreaFactor` / objective evaluation and review | Already rejects unknown geometry before sizing. This remains a separate future-Compact calibration gate; the patch supplies no Compact target policy. |
| `ForestSaveController.cs:569–590` | Guards its preliminary count with `IsKnown`; calls the full validator; only on success uses `CellIndex` for treatment-patch centres. `−1` is already refused. |
| `ForestSaveValidation.cs:209–225` | Null/tree-list checks precede `ValidateGeometry`; a non-null geometry error returns before the new throwing `CellCount(ForSave(data))` at **line 221**. |

`ValidateGeometry` in `ForestSaveValidation.Geometry.cs:10–42` rejects pre-v20 nonzero geometry; for v20+ the historical binding accepts exactly 0/1, rejects reserved 2 explicitly, and rejects every other value. `ForSave` remains Legacy40 for older saves/null; current disk saves still require the explicit integer geometry via `ValidateGeometryJson`. No missing-field or old-version policy was loosened.

On the shared-save line, `AdmittedSave.TryPrepare` reparses its admitted JSON and calls `FindLoadProblem`; `AdmittedSave.Apply` checks preparation **before** invoking the admitted callback or `ApplyAdmittedData`. Thus the reviewed path rejects unknowns before applying them. The `try/catch` around actual world application does **not** surround the preceding `TryPrepare` call. A future reorder of line 221 above geometry validation could therefore turn a player-readable refusal into an escaping exception on CanLoad/prepare paths. **Safe now; non-blocking maintenance dependency, not an existing escape.** Preserve this order, ideally with a nearby precondition comment and a focused regression through the public validation/admission path in the next authorised verification task. Do not add a broad catch that hides broken validation order.

## 4. Positive-only feature checks: keep for this task, revisit before Compact

No remaining positive-only gate requires an 18-file rewrite to accept this patch. These gates select features for admitted worlds; they do not provide dimensions for unknown geometry.

| Gate | Current meaning and future obligation |
|---|---|
| `ScenarioOneRackLayout.AppliesTo`, suggested racks, `RackPlanningV2Active` | E80-only rack functionality; false/empty for other values is deliberate. Do not infer that a future Compact world is correctly supported merely because rack features are off. |
| `InitializeRackPlanForNewWorld` | Clears legacy/default rack state, enables V2 only for E80. This is a state-changing initializer, not a general unknown-input validator; safe under its accepted-ecology precondition. |
| `ForestSaveValidation.RackV2.cs:34–36` | Rejects V2 state on non-E80 geometry. It is a feature constraint, not a substitute for the earlier general geometry validator. |
| `ScenarioReferenceArchive` | Frozen Reference applicability/legacy-compatible identity is intentionally Legacy40-only. Non-Legacy current hashing is a distinct representation, not an unknown-world admission gate. Preserve this boundary. |
| `StandApproachPath.Apply/SaveHasApproach/Reconcile` | **Qualification to #629's explanation:** feature-OFF is not necessarily no-op. `Apply` removes an existing approach for non-E80; `Reconcile` calls it; `SaveHasApproach` returns true for non-E80. Direct unknown calls are not independently fail-closed here. Current callers are the guarded ecology application and validated save application, so no reachable regression is established. Before any new geometry/caller is admitted, explicitly decide approach removal/retention and guard that new route. |

Accordingly the three files are sufficient for **this current-world source hardening**. They do not make every public helper an unknown-input validator, and changing only `IsKnown` or the save-profile availability flag later would be unsafe. Compact needs coordinated dimensions, generation, validation, target calibration, path/rack/teaching decisions and its separately approved world manifest.

## 5. Offline evidence audit and independent checks

Original evidence was inspected at `/media/jer/Files/CCF/Operations/CCF-629-Evidence-20261116/`, never run through its original writing runners. All **42 files** remained hash-identical, with no additions/removals. The three `base_src` files match the exact `09ade4c4` Git blobs; `source.diff` matches the exact Git production diff. Every published short hash checked below matches:

| Artifact | SHA-256 prefix |
|---|---|
| `source.diff` | `e748c81296c9` |
| `harness/Program.cs` / `Stubs/UnityStubs.cs` | `78751c0b729d` / `434bad11c6bc` |
| `compare.py` / `grep_gate.py` | `f6a8307526de` / `b06fe22caebd` |
| `typecheck/Typecheck.csproj` | `a027ed30956c` |
| `logs/new.txt` / `logs/base.txt` | `86d8aea0d290` / `288c4f7deb65` |

Results, distinguishing archived evidence from fresh execution:

- **Replayed archived logs**, using a copied comparator with only its evidence path changed: candidate **100 checks / 0 failures**; baseline **50 failures**; mutants M1/M2/M3/M4 **24/10/6/4 failures**. This validates the log comparisons, not a fresh rerun of the author's whole differential harness. Its 35 compared known observables cover sizes/counts/targets/labels/policy/save mappings, two 58,081-position cell-grid hashes and the known boundary sequence. Stub snapshots cover transforms, tiling, camera and ridge-shaping call counts; the approach's native geometry is stubbed out.
- Inspected all four actual mutant diffs: fallback restored; boundary guard disabled; cell-index guard removed; boundary cache assignment moved before refusal. They target the stated protections, including repeated rejected values. All have archived successful builds.
- **Fresh independent strict probe: 113 assertions PASS**, compiling the actual candidate geometry model, boundary and geometry validator against the supplied Unity stubs and a minimal profile stub. It checks 0/1 dimensions/counts/targets/upper boundary, unknown 2/−1/max/min/3/100, exact exception type, invalid-index sentinel, old/current validation, unchanged accepted cache and repeated diagnostics after either accepted model. It also reproduces the fresh `Apply(-1)` diagnostic exception described above. The full tree generator and full save validator were not executed.
- **Fresh 4/4 mutant kills** using that independent small probe; each build succeeded and each run failed for the expected protection. These receipts do not rely solely on the original comparator.
- **Fresh Roslyn builds of candidate Assets: player and UNITY_EDITOR both exit 0, 0 warnings / 0 errors.** Used a copy of the original csproj, installed Unity **6000.6.0f1** managed assemblies and the same read-only U3 package assemblies. Preserved raw build output and exit status. No Unity executable was launched, and no U3 files were written. These are not Editor imports, asmdef-equivalent Unity compilation, ILPP or runtime tests. Original base typecheck and injected-CS0029 sensitivity claims were inspected as reported evidence, not independently rerun.
- The original grep/structure gate freshly reports PASS on the candidate. It is supporting evidence only: regex/source-order checks cannot prove caller safety or execute `Generate`.

Two **non-blocking evidence-tool limitations** must not be repeated as stronger claims: the original comparator accepts any `refused...` exception prefix rather than requiring `ArgumentOutOfRangeException`; and `run_typecheck.sh` discards the compiler exit status and filters only `error CS...`, so empty diagnostic logs alone cannot prove a successful build. The strict probe and raw-exit builds above address those uncertainties for this candidate without modifying original evidence. Future verification-tool maintenance should make both contracts explicit.

Receipts: `original-evidence-before.json` / `original-evidence-after.json`, `replay-*.txt`, `replay-and-mutation-summary.json`, `spot/Program.cs`, `spot-result.txt`, `spot-M*-build.txt` / `spot-M*-result.txt`, `typecheck-{player,editor}-raw.txt` / `typecheck-{player,editor}-exit.txt`, `source-callers.txt`, and `grep-candidate.txt` in the reviewer receipt directory.

## 6. Ownership and integration admission — separate verdict

**NEEDS CHANGES / HOLD for admission, independently of technical APPROVED.** The author's reported in-session “I approve both” records owner intent. It is not retrospective evidence of a published pre-edit #424/#544 single-writer designation, live registration or physical-board reconciliation. Neither the clean child today nor this review proves that no overlap existed when edits began. Do not backdate the claim or describe the missing step as completed.

The [Overall Manager disposition](https://github.com/jermur788/CCF/blob/docs/sen-1-425-629-source-verdict-633-review-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-425-MANAGER-629-PARTIAL-VERIFICATION-OWNERSHIP-AND-633-REVIEW-20261116.md) admits the child for independent review only. Before any source incorporation, **#424 and #544 must issue a forward-looking ruling naming the exclusive source/integration owner, exact target/head, permitted file set and method of incorporating the isolated child**, after checking actual conflicting claims. #424 or its explicitly assigned writer must record the truthful history and new assignment in authoritative live coordination. #553's assigned physical Ops maintainer must reconcile its working view separately. This reviewer has not acquired or claimed those roles.

G2 source-quality and G3 bounded offline-evidence assessment: **APPROVED within the limits above**. G4 source integration: **not approved by this review / no integration performed**. Protected native old-save/Reference behaviour remains **NOT VERIFIED**; save-line merge/qualification and T-03B/T-04 gates are distinct. No source-acceptance decision should silently absorb the child into a playable candidate.

## 7. Affordable next Tier-2 sequence — proposal only, NOT RUN

First obtain a separate Manager Unity permit; a real #548 deployed/self-test-green lease path; #553 physical/live ownership reconciliation; and an exclusive registered clean Unity-capable Linux lane at the **exact approved candidate HEAD**, with its own Library, isolated saves/Prefs/`XDG_CONFIG_HOME`, durable Files output and inspected normal lease acquisition. Re-check original fixture backup/restore and staged harness/meta cleanup. None of those conditions is granted here.

**Fixture prerequisite discovered during review:** do not run old wrappers blindly. `StandGeometryVerification.cs:122–163` still expects schema **23**, while this branch uses **24**, and passes parsed/cloned DTOs to `LoadData`. The shared-save admission contract refuses those unauthorised typed origins (`TryPrepareCapture`). Reference continuation also clones Year-50 DTOs (`ScenarioReferenceVerification.cs:505`). These are pre-existing qualification-fixture incompatibilities, not caused by #629. Have the designated verification owner reconcile the narrow fixtures to real admitted raw/captured sources, preserving every assertion and negative control, under a separate packet/review before relying on them. Do not weaken production admission or count an origin refusal as proof that unknown geometry itself was tested.

Then run once, sequentially, targeting 20–60 minutes after fixture readiness:

1. Same-head Editor import/compile; fresh E80 new-game gate (`Enlarged80Verification.Begin`, no geometry override): expect 80 m bounds, 256 cells, 1,344 original trees, correct approach/boundary and stable roster/positions. Brief walk/interaction smoke verifies usable boundary and movement.
2. Focused geometry/old-save gate (`StandGeometryVerification.Begin`, explicit Editor-only `CCF_STAND_GEOMETRY=0`, reconciled fixture): expect Legacy40 40 m/64 cells/336 start, genuine pre-v20 raw save load and round trip, known 0/1 boundary switching, and malformed geometry 2/−1/max refused through real admission before world/disk mutation. Include a v20+ missing/non-integer field refusal; do not fabricate an archived fixture from current data.
3. One protected Legacy40 completion (`ScenarioOneCompletionVerification.Begin`) and Reference integrity/preview/Year-50 continuation (`ScenarioReferenceVerification.Begin`, **never Freeze or BeginAuthoringCandidate**), with the reviewed model/configuration and pinned same-line anchors supplied by the operator's approved packet. Reference frozen Y100 remains `7AD177B3CC2F73C7`; documented v15-layout current continuation is `BFFF66015A6619DA`, not the superseded `9CDF...`. No anchor rebaseline. Do not import another branch's hashes without confirming configuration/provenance.

Require normal Editor exit, observable results, unchanged protected files/disk saves, and removal of every staged source/meta. Stop at the first failure; distinguish fixture failure from product failure. This is a bounded qualification proposal, not permission to repair production, run full suites or build Windows.

**Closure:** one new Markdown output; no C# edits, merge, Unity launch, lease acquisition, save/prefs access, asset imports, Ops deployment or Windows build. **Dashboard/Drive: NOT VERIFIED.** The Manager-owned live coordination update, automatic dashboard audit advancement and published-state readback remain an explicit operational follow-up for #424; this one-file reviewer did not mutate those records or claim synchronization.
