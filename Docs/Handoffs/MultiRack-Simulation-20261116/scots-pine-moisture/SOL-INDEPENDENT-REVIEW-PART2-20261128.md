# Scots pine and soil moisture, Part 2 independent review

**INTEGRATION DECISION: NEEDS CHANGES** for `cbc369030827d715ac02d2e4c45238ed6a6783b7`.

**D-030 state: REVIEWED on isolated branch; NOT merged to integration/2d-sim or main.** This is the reviewer’s integration recommendation for the exact feature commit. Integrated verification and owner acceptance are pending.

## Scope and reviewer provenance

| Phase | Model | Reasoning effort | Evidence |
| --- | --- | --- | --- |
| Requested | OpenAI GPT-6.2 Sol primary, GPT-6.1 Sol bounded fallback | Unspecified | Owner review brief |
| Selected | `gpt-6.2-sol` via Codex | `medium` | Top-level T3 launch selection, provider instance `codex_3` |
| Resolved | `gpt-6.2-sol` via Codex | `medium` | T3 thread and run model, with the effort confirmed by the active runtime |
| Executed | `gpt-6.2-sol` via Codex | `medium` | Active runtime identity. T3’s separate `executedModel` field was null during the run. |

Reviewer T3 thread ID `mcp:launch:d47807af5f31dadd19fa58a728161eadcdd691fcc136f273f2ed3b60cec9d27c`. T3 reported a fresh top-level thread with no parent relationship and its own worktree on local branch `t3/07e70952`. The implementation handoff attributes the feature to Claude Opus 5.5 at high effort. This review used the primary model throughout.

Reviewed the spec quartet, implementation handoff and feature diff against `f4dee8023d3492a25e61451c7e2a49515e507ed3`, which is also the merge base with the observed integration tip `97cecc0cb10827d9ba9f84c81bb447feabcb64b6`. At preflight, the remote feature and review branches both resolved to the requested candidate and `main` resolved to `bdeb6a324acaf4129c10fb0846faac8b64974360`. Read the live coordination file from both integration and main. The scoped review branch and write authority came from the owner’s review packet. The review branch carries this single report as a child of the feature commit. Coordination board and dashboard publication are outside this handoff’s single-file scope and their synchronization remains unverified. [Feature commit](https://github.com/jermur788/CCF/commit/cbc369030827d715ac02d2e4c45238ed6a6783b7) · [Implementation handoff](IMPLEMENTATION-HANDOFF-PART2.md)

## Findings

### Blocking — Save v3 refuses an approved pine-only planting plan

In a fresh 0.64 ha Dry Ridge & Moist Slope woodland, queue one Scots pine Plant order at a legal point, approve the Work Plan, then save before Execute. The independent probe used lane `MA-E` at `(73, 1)` with Economy off. Saving the pending plan returned schema 3. After approval, the same capture returned `SaveInconsistent` with “an approved plan holds at least one order.” No harvest or annual advance is required.

The v3 projection removes pine orders from `Phase2.ApprovedOrders` and leaves the root’s approval marker in place. When those are the only approved orders, the inner v2 payload has an empty approval. The frozen v2 encoder rejects it before the v3 feature block can be written. The registered v3 codec’s intended restoration of approved pine orders as pending drafts is therefore unreachable on this valid path. Fix the v3 projection and restore path while retaining the frozen v2 validator. Cover approval → Save → Load for a pine-only plan and check that the restored order is pending and requires a fresh approval. [Projection](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Core/Saves/ScotsPineFeatureBlockCodec.cs#L38-L65) · [V3 encode](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Core/Saves/SaveV3Codec.cs#L74-L99) · [Frozen validator](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Core/Phase1/Phase1SaveValidator.cs#L169-L179)

### Non-blocking — Load undercounts restored approved pine drafts

Adding a Shelter at the same point before approval keeps the inner v2 approval nonempty. The independent probe saved and loaded that plan. The loaded approval was cleared and both Plant and Shelter were pending. `DraftOrderCount` was **1**, while **2** approved orders had returned as drafts. The count derives only from the inner v2 draft, and the Saves dialog displays it. Include the restored pine draft in the result and cover the mixed case alongside the blocking case. [Pine draft restore](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Core/Saves/ScotsPineFeatureBlockCodec.cs#L174-L185) · [Load result](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs#L711-L729) · [Saves dialog](https://github.com/jermur788/CCF/blob/cbc369030827d715ac02d2e4c45238ed6a6783b7/Tools/MultiRackSitkaP1Lab/src/Ui/multi/dialogs/saves.js#L196-L204)

## Independent verification

WSL2 Linux, .NET SDK **8.0.134** resolved from lab `global.json` via `~/.dotnet/dotnet`, with Linux Node **v26.13.1** from `~/.local/share/vite-plus/bin`. No SDK 10 or Windows Node build/test commands ran. A fresh sparse Linux checkout of the reviewed commit held all build output. The ignored `.p0ref` was copied from the existing Year30Compaction worktree, recorded there as built with 8.0.133, and independently rehashed before use.

| Gate | Independent result |
| --- | --- |
| Clean, restore and build `MultiRackSitkaP1Lab.sln` | 0 warnings, 0 errors |
| Clean, restore and build `tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj` | 0 warnings, 0 errors |
| Clean, restore and build `tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | 0 warnings, 0 errors |
| Clean, restore and build `src/Ui/SitkaP1.Ui.csproj` | 0 warnings, 0 errors |
| Scots pine console suite | 27/27 passed, 0 failed in each of two processes. Byte-identical stdout. GROWTH-03 confirms WetMovingAlluvial’s 0.35 growth and 0.70 establishment modifiers. |
| Core.Tests | 326/326 passed, 0 failed. All nine frozen anchors matched. |
| Phase1Save.Tests | 56/56 passed, 0 failed, including SP2-S1–S4 and the frozen v2 golden encoding. The approved pine-only scenario is missing from these cases. |
| `node build/2d-gate/module-graph.mjs` | PASS. 16 modules, acyclic, all reachable from `main.js`, one module script tag. |
| `bash tests/ui-smoke-multiarea.sh` | 363 passed, 0 failed when run in the foreground. All three owned UI processes recorded clean SIGINT shutdown. |
| Supplemental approved-work Save v3 probe | One blocking save refusal and one non-blocking draft-count mismatch as reported above. |

The first sparse build lacked two linked frozen Unity source files. The first Core.Tests run had five missing P0 fixture failures (321/326). I expanded the sparse checkout to include those tracked inputs and reran the affected gates. The first smoke invocation ran as a background job and produced six SIGINT shutdown failures (360 passed). Its foreground rerun used the same code and built UI, and passed. No source changes were needed for these recoveries.

The Part 1 digest remains `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748`. Checked the complete frozen anchor values against the unchanged manifest, Core output and the copied binary.

| Anchor | Exact SHA-256 |
| --- | --- |
| 1. Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| 2. Multi-area 0/0/0/0 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| 3. Multi-area 0/0/1/0 | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| 4. Stage 4 1/1/1/0 | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| 5. Stage 5 1/1/1/1 | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| 6. Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| 7. Phase 2 Q6, both vectors | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| 8. P0 reference DLL | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| 9. MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

Local WSL evidence is under `/home/jer/Development/ccf-sp2-sol-review.YDmAR9Kb/evidence/`. The separate probe source is in the adjacent `probe/` directory. The full self-test, full browser gate, separate Step 2 Edge gate and Unity were skipped per scope. Ports 33767, 43649 and 46593 were left alone.

## Browser evidence inspection

Inspected the targeted Edge CDP driver and the implementer’s `/tmp/scotspine-part2-gate/final/07-browser/run.log`. The final log records **29 passed, 0 failed** at **1920×1080** and **29 passed, 0 failed** at **1280×720**, plus 13 server and worktree checks, for **71 passed, 0 failed**. The earlier 65/2 run at 1280×720 timed out opening Saves with History open. The committed driver closes History before opening Saves, and the final logs cover that path, the five lenses, inspector, work execution, annual history and cross-process load. Both viewports saved after Execute and Advance, so they did not exercise the approved pine-only failure. Both restored zero active recruits. Core SP2-10 separately covers positive recruitment and promotion in controlled gaps. Edge was available on Windows, and the existing evidence met the requested inspection. The targeted suite was not rerun independently.

## File fence and conformance

- `git diff --name-status <base> <feature> -- <protected paths>` returned zero entries for `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/MultiRackSitkaLab/` (P0), `Tools/MultiRackSitkaP1Lab/src/Core/Phase1/`, `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/`, `Docs/Project/` and `2D_Simulator_Strategic_Roadmap.md`. The whole feature diff has 54 files and passes `git diff --check`. The Phase1 tree ID is `fe970e3f78362310821156de0d34bea990387aee` at both commits and the frozen stylesheet and scripts tree ID is `dabdbd7fa07cd01148af14464c45ceea02548378` at both. The P0 tree ID is `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` at both.
- The frozen v2 golden byte length is 24,703 and its file SHA-256 remains `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7`. The legacy v2 route and neutral identity tests passed. Absent pine block follows the owner-approved NeutralLegacy or two-zone genesis rule. Unknown block keys and unknown fields are refused.
- Downy birch and Common alder remain outside live planting, regeneration, growth and harvest routing. Their references belong to offline species evidence and import classification as pending model support. The `src/Ui/multi/` changes render, label and format Core DTOs and perform display geometry. Forestry calculations and monetary settlement stay in Core.
- Reviewed the distinct growth and establishment paths, exclusion from the Sitka density subset and seed sources, deterministic local seed gating, pine-specific pruning and the separate timber price and green-density inputs. The approved €0.45 sapling, provisional light threshold and seed radius, and GROWFOR age-60 boundary stand as ratified. AC-7’s persistence requirement remains open on an approved pine-only plan.
