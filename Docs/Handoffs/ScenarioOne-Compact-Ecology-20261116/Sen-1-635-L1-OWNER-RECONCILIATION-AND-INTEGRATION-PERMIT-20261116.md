# Sen-1 #635 — L-1 owner reconciliation and source-integration admission decision

**2026-11-16 — Manager #424 decision: NEEDS CHANGES / INTEGRATION HOLD.**  
**The exact three-file #629 source remains technically APPROVED under #633.** This is a forward-looking exclusive writer reservation and ready-to-execute-after-gates incorporation plan, **not source integration, Unity qualification, retroactive pre-edit permission, Compact activation, or approval of the unmerged #544 save line for a playable candidate.**

## 1. Decision authority and actual Manager registration

- Decision owner: Scenario One delivery Manager [#424](https://github.com/jermur788/CCF/issues/424), actual worker OpenAI ChatGPT **GPT-6**, browser/GitHub management only. The reasoning-effort selector is not independently attested. No Linux/Windows local worktree, physical CCF-Ops file/lease, Unity Editor or saves were accessed.
- Task claim: [#635](https://github.com/jermur788/CCF/issues/635), [RUNNING registration](https://github.com/jermur788/CCF/issues/635#issuecomment-6925389796) before this document was created.
- Manager documentation lane: `docs/sen-1-635-l1-ownership-integration-admission-20261116-v1` from `main` at **`37cd047ed39fd649563f70602d458e2970f6780f`**, with **only this one new Markdown**. Separately, #424 is sole currently designated GitHub writer of `main:Docs/Project/coordination/active-tasks.md`, with a fresh content-SHA check before modification. #425 owns portfolio/product policy, not a second live board writer.
- The #544 saved-worktree physical checks below are **reported by independent #633**, not presently re-inspected from this browser. GitHub branch-ref and diff checks were performed live by #424.

## 2. Exact GitHub source state observed, and review evidence

| Record | Observed truth |
|---|---|
| True shared-save target | `feature/scen1-shared-three-profile-save-v2`, **unmerged**, currently `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1` |
| Reviewed child | `sen-1/629-l1-geometry-dispatch-20261116-v1` at `46066e032b9d37282ff1ce629304492b5c0e6fa2` |
| Source-only change | `ea3e6eeaca8cef0b75a44de28beb6c3da6b71113`; direct child of the target HEAD, changes exactly **3 C# files, +29/−1** |
| Other child change | one additional Markdown #629 report commit; **not part of future source cherry-pick** |
| Verified compare | target → child: **ahead 2 / behind 0**, exactly three C# files and one added handoff; no scene, asset, save, package, pref, Unity or .meta delta |
| Independent review | [#633](https://github.com/jermur788/CCF/issues/633) [GitHub report](https://github.com/jermur788/CCF/blob/f709c68f482a966d5ac68e2131bcfb7508a494e5/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-633-INDEPENDENT-629-L1-SOURCE-AND-OWNERSHIP-REVIEW-20261116.md), reviewer OpenAI gpt-6.1-sol High independently of Claude #629 author; source **APPROVED**, ownership **HOLD** |
| Offline verification | independent **113 assertions PASS**, **4/4 mutants killed**, fresh player and Editor Roslyn typecheck **0 errors, exit 0**; original author differential **100/100**. None is Unity import/compile/runtime proof. |
| Physical worktree evidence (as of independent review) | #544 save checkout `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` clean at target; #629 isolated child `/media/jer/Files/CCF/Operations/CCF-629-L1-GeometryDispatch` clean at child. **Must re-check at real start; no browser guarantee of current dirtiness or process state.** |
| Main | `main` tip observed `37cd047ed39fd649563f70602d458e2970f6780f`. Neither #629 nor #544 is in a playable candidate or authorised for main merge here. |

**Technical judgement (source only): APPROVED.** Known geometry `Legacy40=0` and `Enlarged80=1` retain the reviewed paths. Unknown geometry now refuses in `StandSizeMeters`, `CellIndex`, world boundary and starting-stand generator without admitting Compact30=2. `IsKnown`, `NewGameModel` and save policy are unchanged. No broad 18-file rework is required for this bounded change.

**Preserve two specific caveats:** `StandApproachPath.Apply` can remove an existing approach when directly called with non-E80; it is safe only through today's guarded callers, **not universally no-op**. `ForestSaveValidation.cs:221` may throw for unknown geometry if a future edit moves its cell-count lookup ahead of `ValidateGeometry`; current validation order protects its public admission path. No permission to weaken/refactor those routes in this packet. Compact profile enabling remains prohibited pending separately approved world and save contracts.

## 3. Historical registration gap: preserve the truth

The #629 author reports the repository owner directly said “I approve both” in that session. This records **owner intent**, **not** a pre-edit [RUNNING] claim, #424/#544 prospective lock, live active-tasks entry or physical Ops board update. The #629 C# edit was completed on a separate clean child branch without those published prerequisites. The reviewer subsequently found both worktrees clean; that is **not proof there was no past overlap**. This Manager does **not** backdate ratification, label #629 as preauthorised, change its history or absorb its child into #544 automatically.

Recorded statuses: [#629](https://github.com/jermur788/CCF/issues/629) **IMPLEMENTED / PARTIALLY VERIFIED / NOT INTEGRATED / NOT UNITY-VERIFIED**; [#633](https://github.com/jermur788/CCF/issues/633) **HANDED-OFF/CLOSED, source review APPROVED**; [#544](https://github.com/jermur788/CCF/issues/544) **HANDED-OFF source-stage orchestration, its unmerged branch remains reserved for save custody**; #424 **RUNNING** delivery manager; #635 **MANAGEMENT COMPLETE when published, integration admission HOLD**.

## 4. Exclusive forward-looking integration writer and target

**One reserved writer, no competing source writer:** a **newly registered GPT-6.1 Sol High / Codex worker on Linux working within #544's shared-save source lane**, using [#638](https://github.com/jermur788/CCF/issues/638) as the single new source task claim. This is a specific **assignment of a future writer role**, not a claim that the original #544 owner has explicitly acknowledged the transfer or that a worker is already running. **#544 custodian acknowledgement is mandatory before execution**; if unavailable, keep #638 BLOCKED and source unchanged. The previous #629 author, #633 reviewer and #424 browser Manager do not simultaneously write this set.

- **Final incorporation target branch:** `feature/scen1-shared-three-profile-save-v2` (not `main`, not playable Candidate A).
- **Required starting HEAD:** `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1`; re-read at start, stop if moved.
- **Existing save-owner checkout to inspect:** `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115`. If it is not safely available, agree an explicitly exclusive replacement *before* writing; no duplicate checkout of the same branch in a competing worktree. Do not modify #629 or #633 worktrees.
- **Exclusive source allowlist:** `Assets/ForestPrototype/StandGeometryModel.cs`; `Assets/ForestPrototype/StandWorldBoundary.cs`; `Assets/ForestPrototype/ForestStartingStand.cs`. A separate one-file GitHub Markdown result is allowed at `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-L1-SHARED-SAVE-SOURCE-INCORPORATION-RESULT-20261116.md` only after work is actually performed. No others.
- **Incorporation method:** after custody/physical checks, cherry-pick **only** `ea3e6eeaca8cef0b75a44de28beb6c3da6b71113` into the exact shared-save branch starting at the pinned HEAD, in a single-writer checkout. Do **not** cherry-pick the #629 handoff commit, merge child tip wholesale, replay unrelated code, rebase or force-push. Diff must be exactly the same three source files / +29−1 and equal reviewed blobs. Stop if cherry-pick is not clean, any extra file/semantic change appears or current branch diverges. Keep the source-only result separate from later Unity and main integration decisions.
- **Before mutation:** #544 prospective assent to exclusive custody; refreshed authoritative main coordination and truthfully recorded #629/#633 history; separate actual physical [#553](https://github.com/jermur788/CCF/issues/553) board/lanes reconciliation, no source conflict; clean current target worktree, own actual branch/HEAD; [#638](https://github.com/jermur788/CCF/issues/638) **[RUNNING]** posted with real model/provider, Linux host, branch, worktree, starting HEAD, owner and three-file allowlist. No source edits until **all** checked, no backdated registration.
- **Later native gates, not source-authorisation substitutes:** [#548](https://github.com/jermur788/CCF/issues/548) durable lease must be deployed/self-test green; [#553](https://github.com/jermur788/CCF/issues/553) actual physical ownership/lock view; clean exclusive Linux Unity lane with own Library/Prefs/XDG_CONFIG_HOME and isolated saves; exact candidate, native same-head smoke/old-world verification and a separate #424 Unity permit.

## 5. Fixture authority before Tier-2

The independent #633 review found pre-existing fixture incompatibilities **on the schema-24 shared-save branch**: `StandGeometryVerification.cs:122–163` still expects schema 23 and supplies cloned/typed DTOs to `LoadData` that new raw-origin admission refuses. `ScenarioReferenceVerification.cs:505` also clones the Year-50 DTO for continuation. These are not #629 regressions and do not justify weakening the save gate or marking legacy/Reference passing.

**Separate bounded verification repair**, owned by a distinct verification worker after the source-incorporation admission decision, must preserve all positive assertions and real negative controls while loading genuine allowed raw/captured save origins. No forged “old save” from current data and no origin-refusal counted as successful geometry validation. Reconcile old pre-v20, valid v20+, malformed geometry 2/−1/max and missing/noninteger geometry, and genuine Reference continuation. Do not modify production admission just to satisfy a fixture.

Only after fixture repair and an independent focused check may a local Unity worker ask #424 for one Tier-2 run: E80 new-game 80m/256-cell/1344-tree geometry and short walk; Legacy40 40m/64-cell/336-tree load and old-save roundtrip; refused malformed raw saves before world/disk mutation; protected Legacy40 completion and frozen Reference preview/Year-50 continuation anchors. No archive freeze/rebaseline. Stop on first failure. This is not authorised now, and no Windows Player build may occur without the user's personal playtest and explicit authorisation.

## 6. Admission gate and exact next action

**Source technical quality: APPROVED. Integration admission: NEEDS CHANGES / HOLD.** The missing parts are forward custody acknowledgement and physical/logical single-writer reconciliation, not another speculative broad source rewrite. The reservation is now concrete and exclusive but **does not make #638 RUNNING**. Once #544 actually assents and the #553/actual local preflight clears, #424 may issue a narrow forward-looking release under #638 without repeating #633 technical review if exact code is unchanged. If conflict or branch drift arises, stop and re-evaluate; no silent approval.

**Next executable handoff and claim:** [#638 — sole GPT-6.1 Sol Linux save-branch source incorporation](https://github.com/jermur788/CCF/issues/638). **Difficulty MEDIUM** due save/ownership sensitivity. Intended worker's exact purpose is one three-file source-only cherry-pick with diff proof and durable result. **Paste-ready line (only when unblocked):** Take CCF Sen-1 #638 https://github.com/jermur788/CCF/issues/638 . As the single #544-approved Linux GPT-6.1 Sol integration writer, read #635 decision, verify exact source branch/HEAD and current physical ownership, register your real worktree/model/allowlist [RUNNING], cherry-pick only the reviewed #629 source commit, prove exactly three unchanged reviewed C# files, and publish the specified GitHub handoff. Do not merge main, launch Unity or modify saves. **User action required now:** none for Manager documentation; execution awaits actual #544 source-custodian/physical-operator coordination, not a product-design choice.

## 7. Publication, physical reconciliation and synchronization

#424 is responsible for the live `main:Docs/Project/coordination/active-tasks.md` update with **actual current time only**, no rewrite of old state. The physical [#553](https://github.com/jermur788/CCF/issues/553) maintainer must later add the genuine #544/#629/#633/#635/#638 source/claim states to its CCF-Ops board/lanes according to actual schema, without creating a fake Unity lane, altering unrelated registrations or claiming physical sync before readback. #548 remains open/undeployed and separate.

No dashboard ingestion, GitHub audit snapshot advancement, Drive mirror update or published dashboard readback has been observed by this browser Manager at decision writing; report **DASHBOARD/DRIVE NOT VERIFIED** until real evidence is obtained. No canonical context/decision/spec source changes are authorised by #635.
