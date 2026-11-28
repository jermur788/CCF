# Woodland Inspection Part 2 — Integration Receipt

**Date:** 2026-11-28 (UTC)
**Authority:** [Manager integration packet](https://github.com/jermur788/CCF/blob/8c17ecc41e4d1855df1bc28451272c8771e80862/Docs/Handoffs/MultiRack-Simulation-20261116/woodland-inspection/MANAGER-INTEGRATION-PACKET-PART2-20261128.md)
**State:** **MERGED TO `integration/2d-sim` ONLY. NOT merged to `main`.**

## Exact refs

| Ref | SHA |
|---|---|
| Integration baseline before merge | `0668b066ce6f6bb4f7c22b47667341888cf59782` |
| Approved feature / review candidate | `9413595bd257c11be141edcabfd014ee87350726` |
| Merge parent 1 (integration) | `0668b066ce6f6bb4f7c22b47667341888cf59782` |
| Merge parent 2 (feature) | `9413595bd257c11be141edcabfd014ee87350726` |
| **Final integration merge** | **`135b8561b595219b1b54ac547c94ea8d1f371be2`** |
| Protected `main` (unchanged) | `bdeb6a324acaf4129c10fb0846faac8b64974360` |
| Feature and review refs (unchanged) | `9413595bd257c11be141edcabfd014ee87350726` |

- Compare: https://github.com/jermur788/CCF/compare/0668b066ce6f6bb4f7c22b47667341888cf59782...135b8561b595219b1b54ac547c94ea8d1f371be2
- Merge: https://github.com/jermur788/CCF/commit/135b8561b595219b1b54ac547c94ea8d1f371be2

The prior R3 independent approval is [recorded separately](https://github.com/jermur788/CCF/blob/b524da53be2f1d93cb1538baf314a85830344365/Docs/Handoffs/MultiRack-Simulation-20261116/woodland-inspection/INDEPENDENT-R3-SAVE-MARKER-REVIEW-20261128.md). This receipt does not reopen R3 or commission another review.

## Admission and exclusive writer

- Dedicated integration worktree: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Integration-20261125`.
- Branch `integration/2d-sim`, clean, at the exact live remote baseline `0668b066` before merge.
- No other worktree checked out `integration/2d-sim`; no running `sitkap1-ui` or dotnet UI process; no Git merge/rebase in progress.
- Local single-writer claim recorded before the merge at `/tmp/ccf-woodland-part2-integration/claim.txt` (outside Git).
- Live board `Docs/Project/coordination/active-tasks.md` contained no matching current 2D Woodland Inspection integration entry; no competing contemporary writer entry was found.
- Writer: DeepSeek V4.1 Flash 1017 / OpenCode / T3 Code, acting as the integration writer under the approved packet.

## Merge and overlap handling

`git merge --no-ff 9413595b...` produced merge `135b8561...` with parents `0668b066` and `9413595b`.

- Five shared paths auto-merged with no conflict markers:
  - `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Commands.cs`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.History.cs`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs`
  - `Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Woodland.cs`
  - `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`
- No manual conflict resolution and no post-merge semantic edit were required.
- Both sides were inspected after the merge:
  - `git diff 9413595b..135b8561` contains only the newer trunk changes: Year-30 bounded-envelope work, senescence Part 1, the updated roadmap and their tests.
  - `git diff 0668b066..135b8561` contains only the reviewed feature's 26 paths.
  - The survey guard is still present at the actual commit point (after body parse, inside `multiHost.SyncLock`), with host-before-slot lock order and Save marker publication inside both locks.
  - Year-30 payload envelope/budget logic, the 4 MiB ceiling, M16/M17 checks and senescence files remain present.
- `git diff --check` passed. No conflict markers remain.
- Forbidden-path diff against the pre-merge integration baseline is empty for `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/**`, `2D_Simulator_Strategic_Roadmap.md`, `src/Core/Phase1/**`, `src/Ui/wwwroot/**`, `src/Ui/multi/map/maplibre/**`, closed `src/Core/WoodlandImport/*.cs`, `tests/Core/WoodlandImport/**` and `.p0ref/**`.
- The P0 binary pin is intact: `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.

## Minimum necessary testing on the exact combined candidate

Run from `Tools/MultiRackSitkaP1Lab` on `135b8561`, .NET SDK **8.0.134** (global.json 8.0.133, latestPatch) and Linux Node **26.13.1**.

| Gate | Result |
|---|---|
| `dotnet build MultiRackSitkaP1Lab.sln` | **0 warnings / 0 errors** |
| `dotnet build src/Ui/SitkaP1.Ui.csproj` | **0 warnings / 0 errors** |
| `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj --no-build --no-restore` | **326/326 passed, 0 failed** |
| Nine frozen simulation anchors | **9/9 exact, 0 drift** — values below |
| `tests/woodland-inspection-part2-api.sh` | **126 passed, 0 failed** |
| `node build/2d-gate/module-graph.mjs` | **PASS: 17 modules, acyclic, all reachable, 1 module script tag** |
| Narrow Year-30 M16/M17 extraction | **PASS** |
| Microsoft Edge CDP, 1920×1080 | **18/18 passed, 0 failed** |

### Frozen anchors (verbatim Core.Tests values)

| # | Anchor | SHA-256 |
|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| 2 | Multi-area 0/0/0/0 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| 3 | Crown C2 (0/0/1/0) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| 4 | Stage 4 1/1/1/0 | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| 5 | Stage 5 1/1/1/1 | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| 7 | Phase 2 Q6 | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| 8 | P0 Core binary | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| 9 | MA-1-MIXED 30-year | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

### API and race evidence

The 126-check harness passed on the merged candidate, including:
- offline catalogue order Donadea then Mersch; both CC BY 4.0; both `InventoryOnly`;
- both Part 1 canonical view byte hashes exact;
- honest Missing crown/age, no inferred boundary, all four species categories, TreMs, attribution;
- 13 refused request cases over 10 routes with HTTP 409 `WoodlandInspectionReadOnly`;
- paused-body K01–K11 commit-time refusals, including multi Advance, Skip, Save, Recover and single-area Advance, plus the no-survey control;
- 50 overlapping Donadea/Mersch load pairs with 0 contradictory replies and 0 view-byte hash mismatches;
- 30 simultaneous Save+Recover pairs on one slot, no timeouts, host→slot order;
- M01–M13 Save progress-marker publication, including a newer same-slot Save;
- clean SIGINT (rc 0), empty stderr, unchanged lab tree and git status.

### Narrow Year-30 evidence

Because `ui-smoke-multiarea.sh` has no section selector, the existing M16/M17 blocks were run from a disposable extraction outside Git; the full smoke sweep was not run.

- **M16:** 30 annual advances and view reads returned 200; Year-30 advance 520,443 B and Year-30 view 525,093 B, both above the 524,288 B soft limit and below the 4,194,304 B hard limit; compaction flags present; stale Year-0 lane fence returned `PlayerSourceStale`; 57 expected `PruneIntervalTooShort` refusals from 180 prune requests.
- **M17:** forced 3,000 B cap returned HTTP 503 `MultiViewPayloadBudgetExceeded`, budget 3,000, actual bytes above the cap; no history-compaction regression.
- **Result: PASS.**

### Touched-area browser evidence

Installed Microsoft Edge, CDP, 1920×1080, against a fresh combined-process server:

- normal play starts with Advance enabled;
- **Inspect real woodland** lists Donadea first and Mersch second;
- Donadea and Mersch each render 20 flat local-metre Year-0 markers;
- the persistent **Survey inspection mode — Year 0** banner names the plot;
- CC BY 4.0, DOI, citation and 2019 observation date are visible;
- no surveyed boundary is drawn and that absence is disclosed;
- crown radius and age show **Not recorded**;
- all four species categories and TreMs records are present;
- Advance, Skip, Approve and Execute are disabled;
- the survey uses two columns, not four;
- Return restores the same playable woodland with Advance enabled, and Advance succeeds;
- zero page exceptions, console errors and 5xx responses; server stopped cleanly on SIGINT (rc 0), empty stderr.

**Edge result: 18/18 passed, 0 failed.**

## Push and readback

Ordinary push (no force):

```
0668b066..135b8561  HEAD -> integration/2d-sim
```

Live `git ls-remote origin` after push:

```
135b8561b595219b1b54ac547c94ea8d1f371be2  refs/heads/integration/2d-sim
bdeb6a324acaf4129c10fb0846faac8b64974360  refs/heads/main
9413595bd257c11be141edcabfd014ee87350726  refs/heads/feat/multi-2d-woodland-inspection-part2-v1
9413595bd257c11be141edcabfd014ee87350726  refs/heads/review/multi-2d-woodland-inspection-part2-v1
```

The integration worktree is clean at `135b8561`; feature and review refs are unchanged; `main` is untouched.

## Limitations and non-claims

- This is an integration receipt, not a new feature review. The approved candidate and prior R3 approval are accepted as supplied by the packet.
- The full `ui-smoke-multiarea.sh`, 2D gate selftest, Phase1Save.Tests, `run-step2-edge.sh`, full browser matrix, Unity, Windows build and external-demo acceptance were **not** run, per the packet.
- Browser coverage is one viewport (1920×1080), not a matrix. The prior feature review covered 1280×720; that was not repeated on the merge.
- Both built-in plots remain InventoryOnly. This does **not** admit either plot to simulation, validate the publisher archive/coordinate assumptions, or claim owner acceptance.
- No roadmap, dashboard, Drive mirror or canonical context update was made in this integration step.
- The local single-writer claim is released by this receipt's publication. No server remains running.

**D-030:** **MERGED TO `integration/2d-sim` at `135b8561b595219b1b54ac547c94ea8d1f371be2`. NOT merged to `main` (`bdeb6a324acaf4129c10fb0846faac8b64974360`).**
