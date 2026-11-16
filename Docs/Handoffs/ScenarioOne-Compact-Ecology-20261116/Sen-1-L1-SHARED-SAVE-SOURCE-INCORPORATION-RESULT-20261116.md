# Sen-1 #638 — L-1 shared-save source incorporation result

**16 November 2026 — SOURCE INCORPORATED INTO THE UNMERGED SAVE BRANCH ONLY.** Exact reviewed source incorporated and published; scoped offline checks PASS. **NOT Unity-verified, NOT merged into main/playable Candidate A, NOT gameplay accepted.**

## Actual worker, authority and registration

- Task: [existing #638](https://github.com/jermur788/CCF/issues/638); sole integration writer OpenAI **gpt-6.1-sol High**, Codex harness through T3 Code, configured Codex provider, Linux host `jer-laptop`. Same worker as the initial blocked preflight; no delegation, replacement writer or new reviewer.
- [Actual RUNNING registration](https://github.com/jermur788/CCF/issues/638#issuecomment-6927457373), GitHub creation **2026-11-16T19:54:38Z**, preceded source mutation. Fresh preflight timestamp `2026-11-16T19:54:28.105824+00:00`.
- Governing authority: [direct owner APPROVED TO START](https://github.com/jermur788/CCF/issues/638#issuecomment-6927355440), [owner conditional #544 custody transfer](https://github.com/jermur788/CCF/issues/544#issuecomment-6926144494), [actual #553 physical PASS](https://github.com/jermur788/CCF/issues/553#issuecomment-6927282972), [full conditional owner packet](https://github.com/jermur788/CCF/blob/docs/sen-1-425-owner-go-638-553-physical-release-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-425-OWNER-GO-553-PHYSICAL-CUSTODY-RECONCILIATION-638-SOURCE-RELEASE-20261116.md), and [original #635 technical/scope decision](https://github.com/jermur788/CCF/blob/docs/sen-1-635-l1-ownership-integration-admission-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-635-L1-OWNER-RECONCILIATION-AND-INTEGRATION-PERMIT-20261116.md).
- Latest direct owner instruction **“I authorise starting”** prospectively superseded waiting for a separate #425 start comment. This is owner authority, not an impersonated Manager comment. #425 retains sole main coordination ownership.
- No separate former-custodian assent or historical #629 pre-edit registration is invented. Physical #553 readback established the prospective three-file transfer and no competing live writer before this start.

## Fresh physical/source preflight

| Record | Observed result before mutation |
|---|---|
| Existing source checkout | `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` |
| Actual branch | `feature/scen1-shared-three-profile-save-v2` |
| Required local and fresh remote starting HEAD | `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1`, both exact |
| Worktree/index/untracked state | Clean; no merge/cherry-pick conflict |
| Source checkout inventory | Exactly one target-branch checkout across canonical and configured legacy Git stores |
| Physical board SHA-256 | `80bb872a00ac25b2ef2e9a96b4c7d721eb1e462fc5235a45149b9e3d8ebdc416` |
| Actual READY proof | `/media/jer/Files/CCF/Verification/ScenarioOne638OwnerStart-20261116T195041Z/physical-ready-proof.json`, matches fresh board bytes |
| Original deployed board validator | `python3 -B /media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md`: exit 0, 30 records |
| Physical source ownership | #544 HANDED-OFF with wider save restriction retained; #638 READY reserved this existing sole writer; #629/#633/#635 historical states preserved; #443 overlaps frozen |
| Current local worker registry | This resumed #638 source worker and disjoint standalone #639; no competing save/Ops source writer found |
| lanes.json SHA-256 | `1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21`, unchanged |
| Live main coordination | Main `5cefe7242d2f4e3f5ee9dd7b6a3e70a34f623494`, board blob `812031bbaf13bcd2a98b85ee9cbc932b1532aeff`; refreshed read-only, older HOLD superseded for this narrow action by direct owner START |
| Candidate context commit | `62e39ec621e38a84030c08b35a171e297056c9ed` |
| Durable evidence volume | Files ext4, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`; no new ZX20 evidence path |

No physical Ops writes or lease acquisition occurred under #638. Current worker observations are cooperative time-current checks, not proof of historical absence of overlap.

## Exact incorporation and production diff

Executed one clean `git cherry-pick ea3e6eeaca8cef0b75a44de28beb6c3da6b71113` in the existing source checkout. **Source result: [`78298069172e00e1086d200c649d5512334cfb56`](https://github.com/jermur788/CCF/commit/78298069172e00e1086d200c649d5512334cfb56)**, parent exactly `09ade4c4d86317ca1c80e3598ad05a7b71ee08c1`. Actual committer time **2026-11-16T19:54:57Z**; original reviewed author metadata retained. No child report was cherry-picked.

The **entire source-result tree** equals the reviewed `ea3e6eea` tree, Git tree `710fc62a89ffed2401ffc788c37abaf3f5ef2c9a`, not merely the three text patches. Baseline to source result: exactly **three C# files, +29/-1**. `git diff --check` PASS. Reviewed source blobs:

| Production file | Delta | Exact reviewed/result blob |
|---|---:|---|
| `Assets/ForestPrototype/StandGeometryModel.cs` | +16/-1 | `3f4ed63916aa14a44eec280c1918632dac9ed949` |
| `Assets/ForestPrototype/StandWorldBoundary.cs` | +7/-0 | `cff8b9a54a1f79a69a9e69333ba9e12039c5ccc9` |
| `Assets/ForestPrototype/ForestStartingStand.cs` | +6/-0 | `7f3f96ec649bfc7d4cff460bffef4f769e5d2b64` |

Resulting behavior: Legacy40=0 and Enlarged80=1 retain their reviewed paths and arithmetic. Unknown size requests throw `ArgumentOutOfRangeException`; unknown cell indexing returns -1; unknown boundary requests leave the boundary unchanged; an unknown starting-stand generator request returns before planting. `IsKnown`, the E80 new-game policy, save schema/profile admission and Compact unavailability are unchanged. No new architecture or save policy was introduced.

Ordinary, non-force source push succeeded to **`feature/scen1-shared-three-profile-save-v2`**. Fresh remote ref readback returned the full source result above. GitHub [baseline/source compare](https://github.com/jermur788/CCF/compare/09ade4c4d86317ca1c80e3598ad05a7b71ee08c1...78298069172e00e1086d200c649d5512334cfb56) independently returned **ahead 1 / behind 0, three files, +29/-1**. This Markdown is a separate documentation-only commit on the same unmerged branch; its final commit and publication readback are recorded on #638 after publication.

## Fresh scoped offline verification

Inspected original #629 harness, stubs, comparator and structure gate before execution. Used copies under Files evidence; never ran the original writing shell wrappers or modified original #629/#633 evidence. Built the two actual integrated geometry/boundary source files with .NET 10 and the inspected minimal Unity stubs; all build/runtime outputs, intermediate data and CLI home were isolated under the evidence directory. No new repository dependency/package was added.

| Fresh check | Actual result |
|---|---|
| Scope/parent/whole-tree/reviewed-blob equality | PASS; exact three-file +29/-1 source commit |
| Stub harness compiler and execution | Build exit 0; executable exit 0; raw outputs retained |
| Existing differential comparator on fresh integrated output | **100 checks, 0 failures**, exit 0; known-world comparison uses the retained reviewed baseline log, not a claimed fresh baseline execution |
| Exact unknown exception checks | **24/24 PASS**: six unknown model values across four sizing/target helpers require exact `refused:ArgumentOutOfRange`, strengthening the comparator's broad exception-prefix check |
| Whole harness output equality | All **101 observables byte-identical** to reviewed candidate output; SHA-256 `86d8aea0d290dde39a2b3981f3f0b4cb8ae4f368cb1c729f5f8421035418e755` |
| Structure/grep gate | **GREP_GATE PASS**, exit 0; generator guard and boundary check ordering covered statically |
| Whitespace and protected user/Ops bytes | PASS; no unrelated tracked changes |

The harness observes known dimensions/cell targets, 58,081-position indexing grids per known model, save-version-to-geometry mapping, unknown refusals and boundary/cache behavior with repeated rejections. Unity objects and ridge-shaping are stubbed; **the full generator, full save admission, native scene, old-save load, real movement and persistent-world behavior were not executed**. This is source-incorporation verification only.

Reproduce from the retained evidence:

```bash
python3 -B /media/jer/Files/CCF/Verification/ScenarioOne638SourceIncorporation-20261116T195400Z/verify.py /media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115
```

`verify.py`, `verification-result.json`, exact command arguments/exit codes, `harness-build.txt`, `harness-runtime.txt`, `compare-result.txt`, `structure-result.txt`, `preflight.json`, `running-readback.json`, `cherry-pick.txt`, `source-push.txt` and `source-github-compare.json` are under `/media/jer/Files/CCF/Verification/ScenarioOne638SourceIncorporation-20261116T195400Z/`. Original copied verification source remains identifiable; comparator's only copied change relocates its log directory. The independent [#633 review](https://github.com/jermur788/CCF/blob/f709c68f482a966d5ac68e2131bcfb7508a494e5/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-633-INDEPENDENT-629-L1-SOURCE-AND-OWNERSHIP-REVIEW-20261116.md) remains prior evidence: **113 independent assertions, 4/4 mutant kills, real-assembly player/Editor Roslyn builds**. Those checks were not repeated or relabelled as new #638 evidence; no redundant broad review was started.

## Preserved limitations and future gates

- `StandApproachPath.Apply` can remove an existing approach when called directly with non-E80; safety relies on today's guarded callers. It is not universally a no-op for unknown geometry.
- `ForestSaveValidation.cs:221` depends on geometry validation preceding its cell-count lookup. Current public admission ordering is safe; a future reorder could expose an exception. No validation route was weakened/refactored.
- Fresh boundary `Apply(-1)` can return through the existing cache equality path without a diagnostic, while making no mutation. The full `ForestStartingStand.Awake` is not certified before-construction-safe by this Generate guard; T-03B remains separate.
- Before Tier-2, a distinct verification task must reconcile schema-23 expectations and cloned/typed DTO origins in `StandGeometryVerification` and Reference continuation with the actual schema-24 raw-origin admission contract, preserving positive assertions and genuine negative controls. No production admission weakening, forged old saves or anchor rebaseline.
- Later Linux Unity requires #548 deployed/self-test-green lease, true physical owner/lane, its own Library/Prefs/isolated saves, exact approved candidate and a separate #425 permit. Proposed native sequence remains same-head import/compile, E80 80m/256-cell/1,344-tree new-game plus walk, genuine Legacy40 40m/64-cell/336-tree load/roundtrip, malformed raw-save refusal before mutation, protected Legacy40 completion and frozen Reference preview/Year-50 continuation. See #633's exact gate plan; none was run here.
- Frozen Reference Future v1, v1-v24 save contracts and all ISO qualification claims remain unaltered/unproved by this task. No personal playtest acceptance or Windows build permission follows.

## Unrelated work, publication and operational handoff

Primary remains on `ops/workflow-foundation-v1` at `46247486b37904f6b36825bb40fb6ed7bea35e88`. Its original dirty `AGENTS.md` SHA-256 `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd` and `Docs/Operations/StorageArchitecture.md` SHA-256 `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22` are preserved. Physical Ops board/lanes bytes remain as preflight; #638's physical READY row was not edited by this source writer. #629/#633 checkouts, main, scenes/prefabs/materials/.meta/ProjectSettings/packages, saves and PlayerPrefs were not modified. No Unity executable, lease, native tool or Windows build was invoked.

**Assessment: source incorporation complete; ready for Manager source-stage handoff only.** Source and this separate result are recoverable on the unmerged save branch. Final GitHub Markdown bytes/compare/branch readback and source checkout cleanliness are recorded in the #638 handoff after publication. #425 must reflect actual source completion in its authoritative main board; the physical maintainer retains Ops status synchronization. **DASHBOARD SYNC / PUBLISHED DASHBOARD / DRIVE: NOT VERIFIED**, outstanding operational follow-up; no generated dashboard state was edited. No main integration or gameplay acceptance is claimed.
