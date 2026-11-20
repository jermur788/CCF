# CCF #849 — Independent Tier-2 final residual re-check

**Residual D4-R: APPROVED — resolved. Repaired source candidate `4c42e2012a4c1bee3a422e41a58535343d63ac9f`: APPROVED for the isolated #849 manual playable 2D slice, within the independent G2 review scope below.** The earlier native-200% two-column failure no longer occurs. No residual CRITICAL, MAJOR, MINOR or NIT defect was established in this focused re-check. D1/D2/D3/D5 remain resolved.

This supplies **Manager #730 G2 acceptance evidence only**. It does not grant Manager acceptance, G4 integration, integrated qualification, release, user acceptance or retrospective custody approval. All untouched prior dispositions and deferred gates remain in force.

| Field | Record |
|---|---|
| Reviewer | GPT-6.1 Sol, independent nonauthor; no implementing child, helper reviewer or source authoring |
| Date / host | 20 November 2026; DESKTOP-DL1BQI7, Windows / WSL2 Ubuntu, user jer |
| Candidate source / parent | `4c42e2012a4c1bee3a422e41a58535343d63ac9f` / `b7045ecd228e2b4241b1906b4427e0cb9cfe341a` |
| Documentary addendum read separately | `71b1c0f51d9a2e03de862bd071a879e5a125c14f`; not substituted for candidate source or independent runs |
| Review branch | `review/multi-849-sol-tier2-repair2-20261120-v1`, created directly from the candidate |
| Locked context | `62e39ec621e38a84030c08b35a171e297056c9ed` |
| Authority | Latest direct re-check packet; approved MULTI-STANDMAP-PLAYABLE-2D-V1 at the candidate, especially §8 A-4 |
| Exact scope | The four-path residual repair, unchanged dispositions D1/D2/D3/D5, map/selection/accessibility-path regression, required builds/tests/digests/P0 preservation |
| Spec conformance in scope | **PASS for A-4 in the measured required conditions and for the repaired behavior.** Broader accessibility and Stage 5 coverage retain the limits below |
| Review change | This one new Markdown only; no candidate fix, checklist edit, force-push, main merge or other-lane mutation |

This report supersedes only the affected dispositions of the [prior repair review at fee1324f](https://github.com/jermur788/CCF/blob/fee1324ffc06dec9671c25f861150d98a0cb1168/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REPAIR-REVIEW-20261120.md). Its resolved D1/D2/D3/D5 evidence remains applicable. The [original review at 7fd3b72c](https://github.com/jermur788/CCF/blob/7fd3b72cd3e50b0f33574210a6417e651c87312e/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-INDEPENDENT-TIER2-REVIEW-20261120.md) remains the source for untouched checklist dispositions.

## Independent lane and source preservation

I created a **fresh remote sparse clone**, separate from the author, original review and first re-check lanes:

```text
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Recheck2
```

The requested branch was created at the exact source SHA and was clean before copying the pinned reference or building. Tools, Docs and specs were materialised, with only the four additional tracked Assets inputs required by the existing build/provenance tests: SitkaGrowthModel.cs, SimulationRandom.cs, Species/SitkaSpruce.asset and TreeSpeciesDefinition.cs. They were read unchanged. No Git index lock or competing dotnet/MSBuild process was found before build. The primary checkout and all previous review/author lanes were left untouched.

The live GitHub coordination board was fetched at task start and refreshed before publication. Final blob: **`10e6077245f32730b7de7e5e7eb618d40085da42`**. It registers the independent Sol review role; it does not retrospectively prove author first-write timing, #846 physical/source release or the registered/actual author-lane discrepancy. Those limits are unchanged.

The exact `b7045ecd..4c42e201` diff is **four modified files, +159/−12**, with no mode change: README.md, app.css, app.js and ui-smoke.sh, all under Tools/MultiRackSitkaP1Lab. The app.js executable text is unchanged after removing comments. The Core and Core.Tests trees are byte-identical to `d2c04cbb`:

| Tree | Identical Git object |
|---|---|
| src/Core | `df681392af379aaa6796c431af4b29ad4bdd6837` |
| tests/Core.Tests | `6b1cb544fa342e2ded118c563db60d860b1d7913` |

The UI Program.cs, PlayerApi.cs and index.html are also byte-identical to `d2c04cbb`. Protected object checks against A-1 `4bb80973` passed for Assets, ProjectSettings, P0, fixtures, AnnualEngine, AnnualRules, WorkContract, CLI, solution, build/, Directory.Build.props and global.json.

P0 was copied **only** from the authorised A-1 lane:

```text
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-WorkRevision-Fence/Tools/MultiRackSitkaP1Lab/.p0ref
```

DLL hash, csproj pin, .sha256 and provenance all matched:

```text
9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
```

**P0 was never rebuilt.** All **52 P0 tracked-file checks** passed after the runs; its before/after file metadata listing was identical. The copied reference retained that hash. Candidate source stayed clean after all tests; only this report was then added.

## D4-R — resolved in the original native-zoom condition

A-4 requires: “At 360 CSS px width and at 200 % zoom: single column, no horizontal scrolling of core actions, long hashes wrap or collapse, targets are at least 44 × 44 CSS px.”

I served this clone's built UI, made **two real engine Fell results** (P30-007 and P30-050, each represented by the unchanged 0.35 m-radius stump), and retained a two-ID approved plan to inspect its identities and dialog. The page contained 98 living trees plus those two stumps, still **100 exact-ID rows/discs**.

I independently repeated the earlier **Edge 157.0.4341.60, 1920×1200 outer window, actual native 200%** condition. The browser setting was set and read back as **2**, CDP device-metrics overrides were cleared, and the CSS viewport became **948×554**, DPR **2**. This is native browser zoom, not CSS zoom or a DPR imitation.

| Condition | CSS viewport / DPR | Computed main column | Page scrollWidth / clientWidth | Smallest drawn disc | Ordinary controls |
|---|---|---|---|---|---|
| Edge native **100%**, outer 1920×1200 | 1896×1108 / 1 | **One: 1849px** | **1881 / 1881**, scrollX 0 | **17.5334 px**, whole-area view | **116/116 ≥44×44** |
| Edge native **200%**, outer 1920×1200 | 948×554 / 2 | **One: 908.5px** | **940 / 940**, scrollX 0 | **44.7999×44.7999 px**, 64 px/m | **116/116 ≥44×44** |
| Edge **360 CSS px**, 100% with explicit viewport emulation | 360×800 / 1 | **One: 313px** | **345 / 345**, scrollX 0 | **44.7998×44.7998 px**, 64 px/m | **116/116 ≥44×44** |

All 100 drawn discs were measured in each row. At 360 CSS px the remaining crowns are at least about **248.53 px** across; the stumps are the smallest targets. Every displayed ordinary core control was horizontally reachable, including the table's first-column Select buttons. The native-200% Execute dialog fit within the viewport and its two controls were ≥44 px tall. Expanded status identities remained contained; the PlanId/execution key used the existing wrap rules. The real plural dialog still named **P30-008 and P30-009** with their reasons; Escape closed it and restored Execute focus.

At normal wide-window 100%, the map still opens on the whole area. Small stumps are below 44 px there, as the visible whole-area note states; zoom controls and the exact-ID table remain available. This preserves the earlier disposition: normal wide 100% is outside A-4's two named target-size conditions. It is not reported as an all-map-targets ≥44 px result.

T3 Preview independently measured the 360 CSS px single-column/no-overflow condition. Its Control-plus and Control-equals attempts left innerWidth 1920, DPR 1 and visual scale 1; those two ineffective steps are **not counted as native zoom**. I then used a fresh private Edge profile. The driver set the browser's own zoom through settingsPrivate, as in the earlier reviewer reproduction. The browser toolbar/menu was not manually operated. Native-200% screenshots were inspected, including stable separate Map, Inspector/Work Plan and table views. No physical touch or human-player evidence is inferred from automation.

## No remaining route to a two-column page

I inspected the complete served page/CSS and the JavaScript's layout-related uses, in addition to the diff.

- `app.css:119` defines the sole main-page column rule: `.layout { display: grid; grid-template-columns: minmax(0, 1fr); ... }`. The entire former ≥900 px page-layout block is gone, including spanning placements.
- The **one remaining @media rule**, at ≥600 px, changes only **.filters** to two fields across. It cannot place Map beside Inspector/Work Plan. The status facts grid also has multiple fact cells; neither is a second main-page column.
- No container query, import, main-grid placement, alternate page-column rule or injected stylesheet remains. The .side container has its ordinary implicit single column.
- The one matchMedia/DPR branch in `app.js:542` chooses **map scale only**. Map zoom classes/width/height attributes affect the SVG and map pane; no branch changes the main layout.

Thus the unchanged one-column main rule applies at every width/zoom under these delivered styles. This conclusion is a source/cascade finding, supported by the three browser conditions above; it is not a claim to have enumerated infinitely many browser configurations.

The committed L01–L05 witnesses all passed. I also extracted their static evaluator to review-owned external storage: current CSS passes the 960 px column check, while the **actual old d2c04cbb CSS fails both the 960 px column and layout-independence checks**. No candidate mutation was used. These checks read CSS/HTML; they are not substituted for browser layout evidence.

## Exact selection, map pane and accessible table regression

The changed CSS does not touch .map-pane, SVG geometry, target radii, hit-testing, table markup or command semantics. Full Core MS-01…MS-06 passed again.

In this candidate's actual 360 CSS px served page:

- Trusted **Enter** on the table's P30-007 Select button selected that exact stump and marked the corresponding row current. Choosing it centred the map pane on its real position.
- Trusted **Space** on Pan east changed pane scrollLeft **1369→1501** while page scrollX stayed **0**. Enter on Zoom in changed map scale **64→96 px/m**; Zoom out restored 64.
- A trusted pointer click at P30-007's drawn centre sent a real fenced **api/hit** request. A second click **25 CSS px to the right**, outside its 22.4 px-radius disc, selected **P30-017**, not P30-007. No invisible enlarged stump target or nearest-centre replacement was introduced.
- Tab from the last map-pan button skipped the SVG/pane and reached the reason field. The pane retains tabindex −1. The table retains its named region, caption, **100 exact-ID row headers**, status and measurement columns, case-sensitive search and first-column Select controls.

The table therefore remains the intended keyboard and screen-reader path. Its semantic structure and keyboard use were checked; **a real screen reader was NOT TESTED**.

## Disclosed side effect — table now extends down the page

**Acceptable for this bounded candidate; no defect filed.** Removing the wide-layout block also removes its 70vh maximum height and explicit vertical scrolling for the tree table. The measured table now uses **5687 CSS px** vertically in both native wide-window conditions (5733 px at 360), with computed max-height **none**. It remains horizontally scrollable for its measurement columns and does not widen the page.

This makes the page longer and puts History further down. For the fixed 100-row slice, exact-ID search, status filtering, the skip-to-table link and first-column Select remain available; the keyboard path and map centring still work. A capped, separately scrolling table is not required by the accepted spec. Restoring an independent cap might be a later usability preference, but is not required to resolve D4-R and is not a condition of this verdict.

## D1/D2/D3/D5 dispositions retained

| Finding | Current disposition / basis |
|---|---|
| D1 — confirmation could rebind to another plan | **RESOLVED, retained.** Executable app.js and the relevant facade code are unchanged from d2c04cbb. Its immutable confirmation/withdrawal behavior retains the earlier independent real-browser race evidence. J03…J15 passed again in the full smoke |
| D2 — commands allowed after unconfirmed outcome | **RESOLVED, retained.** Control/handler/final POST locks and successful-Re-read recovery are unchanged. J16…J29 passed again, including committed-but-lost Add/Execute responses and blocked retries |
| D3 — unminted/future execution key accepted | **RESOLVED, retained.** Core is byte-identical; PS-20 and the smoke's unminted/future-key checks passed again. The previous independent 18-assertion API probe remains applicable; it was not rerun here |
| D5 — singular/plural confirmation wording | **RESOLVED, retained.** Labels and JavaScript are unchanged; PS-18/J04 passed again and the real native-200% two-tree dialog was inspected |

The full prior real-browser race/failure matrix was deliberately not repeated for a comment-only JS change. Its prior evidence is inherited explicitly; the current smoke's DOM stand-in is not described as a browser.

## Regression gates actually run

Working directory: Tools/MultiRackSitkaP1Lab in the fresh clone. SDK **8.0.134**, .NET/ASP.NET runtime **8.0.34**, Linux Node **v26.13.1** at the packet's specified path, Python **3.14.4**. Windows Node v26.13.1 drove private Edge using its built-in WebSocket support; no package was installed.

```sh
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln -p:TreatWarningsAsErrors=true
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
/home/jer/.dotnet/dotnet restore src/Ui/SitkaP1.Ui.csproj --source "$E/empty-nuget"
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore -p:TreatWarningsAsErrors=true
NODE="$HOME/.local/share/vite-plus/bin/node" TMPDIR="$E/tmp" tests/ui-smoke.sh "$E/smoke"
```

E is the review evidence directory below. The smoke was inspected before use; it runs its own scratch fixture and does not write a real save. The solution does not include UI, so both builds were run explicitly.

| Gate | Observed result |
|---|---|
| Solution build | **0 warnings, 0 errors** |
| All Core tests | **92/92 passed, 0 failed**: 66 baseline + 20 PS + 6 MS |
| PS-01 concurrency | 16 rounds, **788 atomic reads**, Execute won 0 / Advance won 16. Test passed; this run alone did not observe both winners. Prior exact-source review observed both |
| PS-14 post-Fell biology | PASS; **13 natural deaths** over ten years |
| UI restore / build | Restore succeeded from the empty local source; **0 warnings, 0 errors** |
| Entire ui-smoke.sh | **137 passed, 0 failed**, including **L01–L05**, J01–J31 and the existing envelope/workflow/replay/preservation checks |
| Ten no-work digests | **10/10 unchanged**. Extracted digest-record file SHA-256 **4e0e47a13e6b366b9eadf0d4c47c479da21fa31e8f64f6e07125826dae91ccee**, identical to the earlier independent record |
| Reviewer Edge driver | **39/39 checks PASS**, no page JavaScript exceptions; genuine stumps, native zoom, controls, exact selection, keyboard and map regression |
| Static witness negative control | Old residual rejected by column/independence checks; current CSS passes |
| P0 / source | Four-way reference pin, **52 file checks**, identical metadata, protected objects and Core byte identity all PASS; no rebuild or candidate edits |
| Whitespace / status | git diff --check PASS; source-clean after runs |

The smoke's server ended under **SIGINT, exit 0**, without stderr or writes to lab/P0/fixture/~/.aspnet. The independent browser server was stopped under **SIGTERM, exit 0** after exact process/cwd ownership checks; it printed “Session ended; nothing was saved.” and wrote no stderr. Private Edge instances and the Preview tab were closed.

## Checklist re-dispositions affected by A-4

These are independent review dispositions, **not edits to canonical checkboxes**. PASS is scoped to the delivered manual channel and named evidence. Unlisted dispositions stand.

| Item | Current status |
|---|---|
| A-4 | **PASS in the required measured conditions.** 360 CSS px and actual native 200% at the previously failing 1920 px window: one page column, no horizontal scrolling of core actions, contained identities and ≥44×44 targets |
| PM-R14 | **A-4 FAIL removed; accessibility coverage remains PARTIAL.** Browser geometry/keyboard evidence passes; real reader and physical touch NOT TESTED. A-7's truthful NOT RUN requirement is observed |
| PM-X15 | **PASS for the delivered browser/keyboard run and D1/D2 recovery negatives**, combining prior unchanged-source 1280/360/race evidence with this final 360/native-200% check. No remaining A-4 failure; human-player acceptance NOT TESTED |
| R-22; X-21 | **PARTIAL overall, A-4 PASS.** The delivered manual path's layout/targets/keyboard evidence is supported; real reader/physical touch and deferred sample/proposal coverage are not newly qualified |
| PM-R10; PM-X12; PM-X13 | **PASS retained for exact geometry/selection**; byte identity, all MS tests and real stump/outside-pointer regression |
| PM-X14 | **PASS reaffirmed: 137/137**, including L01–L05 |
| PM-X16 | **PASS — exact-candidate independent review performed and published; scoped verdict APPROVED** |
| AC-PM-13 | **Independent-review condition met for this exact candidate**, with an APPROVED verdict and no CRITICAL/MAJOR finding. Manager adjudication remains separate |
| PM-R15; R-25; X-24 | **Custody / broader acceptance limits unchanged; NOT VERIFIED or NOT SATISFIED for full acceptance** as previously recorded. This review does not satisfy missing physical/source provenance or integrated/Manager gates |

PM-R08, PM-R09, PM-R11, PM-X07, PM-X10, R-12/X-12, X-11, R-15/X-14 and R-26 retain the repaired dispositions in the preceding review. R-06/X-05 local-sample deferrals, G1-disabled dispositions, X-06 limits and the broader Stage 5 boundary remain unchanged. **No blanket Stage 5 PASS is issued.**

## Evidence and explicit limits

Review-owned external evidence:

```text
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-Recheck2-Evidence
C:\Users\hp\AppData\Local\Temp\ccf849-sol-repair2-20261120
```

The evidence manifest contains **45 files**. SHA-256 of SHA256SUMS: **3873b8f09b0aae2d91b27ed7cbbf0b80e8b842d64b5816c34c198de1b9bfeaca**. It includes build/test logs, source/P0 audits, the independently executed browser driver and request/geometry results, native-zoom screenshots, the static negative controls and truthful reviewer setup notes.

| Evidence | SHA-256 |
|---|---|
| core-tests.txt | fec3487ca46cfd9abbf8bd3de0b85d7c50509b6de6c45619f1e35ba0ddb566d2 |
| smoke/ui-smoke.txt | 22a4f09b840cd648b8bbdee19299d293eb72a2c1670b21342b91b7d7ec4555d5 |
| edge-observations.json | c3ecef19ce3d4a36c2881bcdcad67ea3bd5b84d2f42ec7d43ed990226f51df7d |
| edge-repair2.cjs | d5f343218bdebfc44cb2047b945f41381793fe64c8abbc191c207c6bbf7fdd78 |
| stable-screenshot-observations.json | 5729f113fc0737c9b23d974ad0a534b1596b0aff4d2b87a75cc95f04e5810eb4 |

**NOT TESTED:** real screen reader (including NVDA), physical touch, human play or human acceptance, integration or post-integration qualification, and retrospective author physical/source custody. Also not run: Unity, native game/Windows Player build, persistence/restart, exhaustive untouched Stage 5 permutations, full contrast audit, local-sample/G1/#805 deferred workflows, or a new complete D1/D2 browser race/fault campaign. The prior exact-source D1/D2 browser evidence and D3 independent API probe are retained, not relabelled as fresh runs.

Preview could not produce native zoom through its two shortcuts; this required check **was completed in private Edge**. Initial reviewer browser-target setup and an unsettled screenshot were corrected and disclosed in browser-notes.txt; neither is a candidate failure or counted passing evidence.

Dashboard ingestion/audit/published-state synchronization and Drive mirror refresh remain **NOT VERIFIED**. The sole-review-file remit does not authorise coordination or generated-dashboard changes; operational synchronization remains outstanding under AgentWorkflow.

**Handback to Manager #730:** use this as the final focused G2 independent review for the exact repaired #849 source candidate. D4-R is closed; no candidate repair is requested by this review. Remaining custody, broad accessibility, human, integration and acceptance gates retain their separate owners.
