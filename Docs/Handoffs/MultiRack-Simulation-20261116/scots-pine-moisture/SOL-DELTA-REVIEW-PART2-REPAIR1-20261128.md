# Scots pine and soil moisture Part 2 independent delta review

INTEGRATION DECISION: APPROVED

Reviewed HEAD `952905243dee17e9830fe179eada38e5c2501783` on `feat/multi-2d-scots-pine-moisture-part2-v1`. **D-030 state: REVIEWED on isolated branch; NOT merged to integration/2d-sim or main.** This is the G2 acceptance recommendation for the repair delta. G4 integration, integrated AC-7 verification and owner acceptance remain pending. [Source and test fix](https://github.com/jermur788/CCF/commit/954b2778ddb97a26af554dc976d760b38f8e2c6a) · [Repaired HEAD](https://github.com/jermur788/CCF/commit/952905243dee17e9830fe179eada38e5c2501783)

## Scope and provenance

Used context commit `bdeb6a324acaf4129c10fb0846faac8b64974360` from the approved Option A spec and plan. Fetched `origin` before review and read the live coordination file from `origin/main` and `origin/integration/2d-sim`. Used the two checkouts and write scope assigned in the review packet. Compared the repair to the prior rejected head `cbc369030827d715ac02d2e4c45238ed6a6783b7` and checked the Phase 1 fence against feature base `f4dee8023d3492a25e61451c7e2a49515e507ed3`. [Prior independent review](SOL-INDEPENDENT-REVIEW-PART2-20261128.md) · [Approved spec](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/specs/multi-2d-scots-pine-moisture-v1/spec.md) · [Updated implementation handoff](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/IMPLEMENTATION-HANDOFF-PART2.md)

| Phase | Model | Reasoning effort | T3 thread ID and evidence |
| --- | --- | --- | --- |
| Requested | `gpt-6.2-sol` primary, `gpt-6.1-sol` only if primary unavailable | Unspecified | Coordinator `2d55bde9-c31e-4fac-a1e4-eac6bd96b16a`, review packet |
| Selected | `gpt-6.2-sol` via Codex (`codex_3`) | `high` | Reviewer `mcp:219373df-5a69-4fc9-88ed-342182d7868f`, launch selection and returned options |
| Resolved | `gpt-6.2-sol` via Codex (`codex_3`) | `high` | Same reviewer thread, T3 thread and active run model plus runtime effort |
| Executed | `gpt-6.2-sol` via Codex | `high` | Same reviewer thread, active harness identity. T3's separate `executedModel` field was null when read during the active run. |

The coordinator ran a separate DeepSeek V4.1 Flash thread at `2d55bde9-c31e-4fac-a1e4-eac6bd96b16a`. The implementer was Claude Opus 5.5 (`high`) at `mcp:b41ca022-a550-46fb-84f2-b04386c9a4ef`. T3 confirmed the implementer's executed model. The prior review records reviewer `mcp:launch:d47807af5f31dadd19fa58a728161eadcdd691fcc136f273f2ed3b60cec9d27c` at `medium`. This delta review used the primary model throughout. [Scots Pine Work Plan Save v3 Fix](t3-thread://v2/030a041c-34a1-4375-b3ab-fe0ff5333ea9/2d55bde9-c31e-4fac-a1e4-eac6bd96b16a) · [SP2 repair — Save v3 approved Scots pine plan (Claude Opus 5.5 high)](t3-thread://v2/030a041c-34a1-4375-b3ab-fe0ff5333ea9/mcp:b41ca022-a550-46fb-84f2-b04386c9a4ef) · [SP2 delta re-review — repair cycle 1 (GPT-6.2 Sol, independent)](t3-thread://v2/030a041c-34a1-4375-b3ab-fe0ff5333ea9/mcp:219373df-5a69-4fc9-88ed-342182d7868f)

## Repair diff

| File | Added | Removed |
| --- | ---: | ---: |
| `Tools/MultiRackSitkaP1Lab/src/Core/Saves/ScotsPineFeatureBlockCodec.cs` | 17 | 1 |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` | 4 | 1 |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/ScotsPineSaveTests.cs` | 72 | 0 |
| `Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/IMPLEMENTATION-HANDOFF-PART2.md` | 59 | 0 |
| **Delta total** | **152** | **2** |

The fix commit accounts for three files and +93/−2. The repaired HEAD adds only the handoff update. `git diff --check` passed for the repair and the full feature diff.

## Findings

1. **Former blocking Save v3 refusal — fixed.** `ForV2` clears the projected approval marker only when all four projected approved order lists are empty. The predicate matches the frozen validator's Fell, Stage 4 Prune, Stage 5 Clear and Phase 2 planting lists. The separate `scotsPine.approvedOrders` still comes from the live root. The slot probe verified a null marker in the inner v2 payload and one approved plant in the feature block. Load restored the same order as Pending, cleared approval, minted a new session, and refused the original token with `WorkPlanNotApproved` using the new fence. Fresh approval minted a different `PlanId`. [Projection](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/Tools/MultiRackSitkaP1Lab/src/Core/Saves/ScotsPineFeatureBlockCodec.cs#L38-L106) · [Frozen validator](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveValidator.cs#L169-L178)
2. **Former non-blocking draft undercount — fixed.** The load result adds exactly the verified pine block's `approvedOrders.Count` to the v2 saved approval count. The block carries only Scots pine Plant orders. Shelter and SpotClear still use the inner v2 approval, so both counts are disjoint. A missing pine block contributes zero. The slot probe measured 1, 2 and 3 formerly approved drafts for pine-only, pine plus Shelter, and pine plus Shelter and SpotClear respectively. [Load result](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs#L708-L733)

The repair confines the behavior change to the v3 projection and load reporting. The count reuses `DecodeParts` on the verified block. `SaveV3Codec.Decode`, its placeholder-root validation and its stateless contract are unchanged. The format and approval policy are unchanged. Spec conformance passes for the repair's Save v3 behavior within AC-7.

## Independent RED reproduction

Used WSL and `~/.dotnet/dotnet` 8.0.134, resolved from the lab's `global.json`. `TMPDIR`, logs and probe files were outside both checkouts. In the detached verification checkout, `git checkout cbc36903 --` reverted only the two repaired source files. I left SP2-S5 and SP2-S6 at repaired HEAD, built `Phase1Save.Tests.csproj` with 0 warnings and 0 errors, and ran the assembly. Exit status was 1. The frozen validator throws `P1Refusal` with code `SaveInconsistent` for the first line's message. [Refusal implementation](https://github.com/jermur788/CCF/blob/952905243dee17e9830fe179eada38e5c2501783/Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveValidator.cs#L511-L514)

```text
FAIL SP2-S5 v3 approved pine-only plan: Approve -> Save before Execute -> Load gives one Pending draft that needs a fresh approval — inconsistent save: an approved plan holds at least one order
FAIL SP2-S6 v3 approved pine Plant + Shelter: Save before Execute -> Load gives both orders as Pending drafts and counts 2 — two formerly approved orders restored as drafts: expected 2, got 1
56/58 passed, 2 failed
```

I restored both files with `git checkout 95290524 --` and confirmed empty `git status --short` before rebuilding. The checkout remained detached at the full reviewed SHA with empty status after verification.

## Repaired-state gates

Ran from `Tools/MultiRackSitkaP1Lab` with `~/.dotnet/dotnet`. All four builds used `--disable-build-servers -v minimal`.

| Gate | Result |
| --- | --- |
| `--version` | `8.0.134` |
| `build MultiRackSitkaP1Lab.sln` | 0 warnings, 0 errors |
| `build tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj` | 0 warnings, 0 errors |
| `build tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | 0 warnings, 0 errors |
| `build src/Ui/SitkaP1.Ui.csproj` | 0 warnings, 0 errors |
| `tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | 326/326 passed, 0 failed |
| `tests/Phase1.Save.Tests/bin/Debug/net8.0/Phase1Save.Tests.dll` | 58/58 passed, 0 failed |
| `tests/Core/ScotsPine/bin/Debug/net8.0/SitkaP1.Core.ScotsPine.Tests.dll` | 27/27 passed, 0 failed |

The exact Part 1 digest printed by the Scots pine suite was `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748`. The nine full values below matched the unchanged frozen manifest and Core output with zero drift. The Q6 pair matched for both model vectors. The source pins and the `.p0ref` file were checked directly. [Frozen manifest](https://github.com/jermur788/CCF/blob/f4dee8023d3492a25e61451c7e2a49515e507ed3/Tools/MultiRackSitkaP1Lab/build/2d-gate/frozen-anchors.json)

| Anchor | Exact SHA-256 |
| --- | --- |
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| Multi-area 0/0/0/0 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| Multi-area 0/0/1/0 | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| Stage 4 1/1/1/0 | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| Stage 5 1/1/1/1 | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| Phase 2 Q6, both vectors | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| P0 reference DLL | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

## Independent save-slot probe

Built a throwaway console program outside both worktrees against the repaired Core DLL. In the Dry Ridge & Moist Slope preset, used lane `MA-E` at `(73, 1)`. For each plan the probe queued the orders, approved, durably committed a schema 3 slot before Execute and opened it through a fresh `SaveV3Slots` instance. It checked the exact restored order identities through `ReadPhase2View()`.

| Approved plan before saving | Inner v2 approval | Pending after load | `DraftOrderCount` |
| --- | --- | ---: | ---: |
| Scots pine Plant, Economy off | Absent | 1 | 1 |
| Scots pine Plant + Shelter, Economy off | Present, 1 order | 2 | 2 |
| Scots pine Plant + Shelter + SpotClear, Economy on | Present, 2 orders | 3 | 3 |

All three loads cleared approval, refused the pre-save plan token under the new fence with `WorkPlanNotApproved`, and minted a different `PlanId` on fresh approval. The second slot save and load preserved every Pending order and returned `DraftOrderCount = 0` because that save held pending drafts. Strict decode accepted both re-save envelopes, and the entire `scotsPine` block was byte-identical for each case. An initial full-file equality assertion failed when the second save crossed a UTC second. Comparing the envelopes showed only the expected `header.savedUtc` and `checksum.value` changes. All persisted state matched in every case. Local evidence is in `/tmp/ccf-sp2-delta-sol.1OILNbkX/`, including the original failed assertion, the follow-up envelope diff and independent probe source.

## Fence and remaining scope

- `git diff --stat f4dee8023d3492a25e61451c7e2a49515e507ed3..952905243dee17e9830fe179eada38e5c2501783 -- Tools/MultiRackSitkaP1Lab/src/Core/Phase1` was empty. The tree ID was `fe970e3f78362310821156de0d34bea990387aee` at the feature base, pre-repair head and repaired HEAD.
- The repair changed zero files in `src/Ui`, `Assets`, `Packages`, `ProjectSettings`, `Docs/Project`, `2D_Simulator_Strategic_Roadmap.md` or `Tools/MultiRackSitkaLab`. The earlier Part 2 UI work accounts for 17 files relative to the feature base. The protected `src/Ui/wwwroot` tree was `dabdbd7fa07cd01148af14464c45ceea02548378` and the P0 tree was `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` at base and repaired HEAD. The `.p0ref` DLL retained its pinned SHA-256 before and after the gates and was never rebuilt.
- The frozen v2 golden test and its source were unchanged and passed for the 24,703-byte encoding, file SHA-256 `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7` and embedded checksum `9629ac1648fabd85702fc4105075cebe9f69e884a816edcce4c441a3866c9086`. SP2-S4 tamper refusals and the separate Core C-V3 unknown-block refusal passed unchanged.
- Downy birch and Common alder remain unwired in live planting, growth, regeneration and harvest. Their model references remain in offline Wave 2 code, the import mapping classifies them as `PendingModelSupport`, and UI mentions are roadmap items.
- Live GitHub refs at prepublication check showed feature HEAD `952905243dee17e9830fe179eada38e5c2501783`, `main` at `bdeb6a324acaf4129c10fb0846faac8b64974360` and `integration/2d-sim` at `97cecc0cb10827d9ba9f84c81bb447feabcb64b6`. The repaired HEAD is outside both main and integration ancestry.
- Skipped per packet: Edge/browser suites, `tests/ui-smoke*.sh`, `build/2d-gate/module-graph.mjs`, `build/2d-gate.sh` in all modes, `build/2d-gate/selftest.sh`, `run-step2-edge.sh`, npm/Node UI builds and Unity. Rendered feedback, full AC-7 on the integrated candidate, owner acceptance and dashboard/Drive synchronization remain unverified. The Fell, Prune and Stage 5 Clear terms of the approval projection were checked against the frozen validator by source inspection, with no additional pine-mixed slot cases for those three order kinds.
