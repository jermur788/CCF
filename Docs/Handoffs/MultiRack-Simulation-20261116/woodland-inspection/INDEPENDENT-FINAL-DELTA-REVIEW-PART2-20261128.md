# Playable Real-World Woodland Inspection — Part 2 final independent review

**INTEGRATION DECISION: NEEDS CHANGES**
**G2 acceptance:** FAIL on Save commit atomicity. **G3 evidence integrity:** focused harness passes, with the limits below. **G4 integration:** pending Manager authorization and combined-candidate verification.

| Item | Exact reference |
| --- | --- |
| Reviewer | Codex GPT-6.2 Sol, Medium, separate read-only lane |
| Candidate and GitHub feature/review heads | `70d61e4a80bd0d6107ba9c6655e8c5ef36d14490` |
| Prior review | `eaae40a4df1027f3f0c2fbdaf53b16c8716d7fe1` |
| R1 source and handoff | `8e986e63cadb8496f82d1cf98423d3dc90ccd816`, `89612ab1` |
| R2 source | `f8dbac6c75c088265fb1b5895b59913220147f75` |
| Manager packet | `7bffa7e504320f6ee3ede318bdcc717b455e8ca1` |
| Original Part 2 base | `b2006767cbe70fa2c822497470908251a0fd6ffd` |
| Integration target at review | `0668b066ce6f6bb4f7c22b47667341888cf59782` |
| Protected main and spec-stated context | `bdeb6a324acaf4129c10fb0846faac8b64974360` |
| Spec | `SPEC-multi-2d-woodland-inspection-V1`, APPROVED / Part 2 IMPLEMENTING. Delta conformance FAIL on §8.1's shared commit boundary. |

## Blocker — Save publishes its progress marker after releasing the host lock

R2 places the survey recheck, capture and slot write under host → slot. The host lock is released at line 144. `MarkPersisted` then reacquires the host lock at line 152. It replaces the host state record if that record is still reference-equal to the state captured by Save. Opening a survey changes the inspection field and leaves that record reference intact. A Save can therefore change the playable woodland's `hasUnsavedProgress` after the survey opens. This also affects the unsaved-progress confirmation for New Woodland and v3 Load. [Save critical section and marker update](https://github.com/jermur788/CCF/blob/70d61e4a80bd0d6107ba9c6655e8c5ef36d14490/Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Saves.cs#L123-L152) · [Host state and marker](https://github.com/jermur788/CCF/blob/70d61e4a80bd0d6107ba9c6655e8c5ef36d14490/Tools/MultiRackSitkaP1Lab/src/Ui/MultiSessionHost.cs#L53-L64) · [Survey load](https://github.com/jermur788/CCF/blob/70d61e4a80bd0d6107ba9c6655e8c5ef36d14490/Tools/MultiRackSitkaP1Lab/src/Ui/MultiWoodlandInspection.cs#L184-L188)

Exact source-derived reproduction schedule for a thread-selective debugger. The focused harness did not force this schedule.

1. Advance once so `GET api/multi/view` reports `hasUnsavedProgress: true`. Send a fenced Save for a disposable slot. Suspend only its request thread just after it leaves the host lock at line 144 and before `MarkPersisted`.
2. Open Donadea on another request and wait for its HTTP 200. Read `api/multi/view` while the survey is active. It still reports unsaved progress.
3. Resume Save and wait for HTTP 200. With Donadea still active, the same read now reports `hasUnsavedProgress: false`, although the session, year and revisions are identical to step 2.

R2 also removes same-slot serialization of marker publication. At the same pause point, Advance again and let a second Save to the same slot write the newer year. If the first Save publishes its marker before the second Save publishes, both captured the original host state record, and the first publication changes its reference. The second publication then silently skips. Both saves can return 200 with the latest year on disk while the live view still reports unsaved progress. The minimal correction is to publish the marker inside the existing host critical section immediately after a successful slot write. Retain host → slot order and serialize the response after releasing the host lock. Verify the observable progress marker in the forced ordering.

## Original findings and R2 evidence

- **R1 mutation race closed for engine and slot effects.** The admission check is a fast refusal. After request parsing, the authoritative guard shares the host lock with multi commands and Work Plan edits, Advance, Skip, Money switch, single-area commands, and the Save/Recover file mutations. K01–K11 independently showed typed commit refusals, unchanged session/year/revisions and a committing control. Save's late progress-marker publication is the outstanding exception above. Inspection load/exit and successful New Woodland/Load Save use that same lock to set or clear inspection. A refused New or Load leaves inspection intact by source inspection. The harness passed the successful replacements and stale New refusal.
- **R1 contradictory reply finding closed.** Load uses the entry committed by that request for both halves of the reply. Current, catalogue and exit each use one snapshot appropriate to their operation. O01–O05 yielded 100 HTTP 200 replies across 50 opposite-plot pairs, zero contradictions and zero digest mismatches. Donadea and Mersch served the actual canonical bytes with the Part 1 goldens. Both remain InventoryOnly Year 0, with no inferred boundary, simulated future, or fabricated missing measurements.
- **R1 coverage nit closed.** The checklist now identifies 13 refused requests across 10 distinct routes and names the separate paused-body checks. The exact served-byte assertions replace the declared-digest-only check. Clear is still outside the committed request matrix and its route uses the inspected guarded shared handler. The previously accepted J01 boot-order assertion is unchanged across R1/R2.
- **R2 Save/Recover lock inversion closed.** Save and Recover both take host before the slot gate, and the slot helpers never take the host. The list and load read files without holding both locks. All 30 Save/Recover pairs returned within their timeouts. Save answered 200 in all 30. Recover answered 200 or 409 in all 30. S05 verified an existing-slot Save refused at the survey commit check before SlotExists. S02 checks the Recover status only and its label overstates typed-body coverage. The race uses a valid primary and does not establish successful backup replacement under contention. S04 compares fences and does not check `hasUnsavedProgress`. [Harness assertions](https://github.com/jermur788/CCF/blob/70d61e4a80bd0d6107ba9c6655e8c5ef36d14490/Tools/MultiRackSitkaP1Lab/tests/woodland-inspection-part2-api.sh#L440-L458)
- **File fence passed.** R1/R2 change six UI C# files, the focused API harness, three spec artifacts and the implementation handoff. No importer, fixtures, engine, browser assets, J01, Unity, roadmap, canonical project docs or frozen reference changes in the two-repair delta. `git diff --check eaae40a4 70d61e4a` passed.

## Independently run

The live GitHub board was fetched from main and checked for a matching reservation. The packet assigned a separate review lane. The run was registered in the local evidence directory before execution, on a clean detached WSL checkout at the exact candidate, with an isolated scratch fixture and save directory and an OS-assigned loopback port. The primary checkout's unrelated untracked paths were left untouched. The initial sparse checkout omitted two tracked shared compiler inputs. Including `Assets/ForestPrototype` from the same candidate resolved those setup errors without source changes.

From `Tools/MultiRackSitkaP1Lab`:

- `~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --nologo` — SDK 8.0.134, 0 warnings and 0 errors. Verified the copied, pinned P0 DLL using SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. The build accepted the pin.
- `timeout 100 bash tests/woodland-inspection-part2-api.sh /tmp/ccf-woodland-part2-final-evidence.k5TTdQ61` — **113 passed, 0 failed**, including K/O/S. The server exited with code 0 and the candidate's tracked worktree was clean after the run.

Local evidence is in that WSL directory. Build log SHA-256 `a7f62eb8832ae0e59a377c678f8e01882f6149b9c2cfbc129bdb47ea766175a0`. API log SHA-256 `b61b383e7b696f60314d182a444df0cf92521b25c028dbd989d3f43c5d5a7366`.

The builder reports R2 smoke 363/363, clean UI and solution builds and a passing 17-module graph. The R1 evidence reports WoodlandInspection 20/20, WoodlandImport 29/29, Core 326/326, 9/9 anchors exact and Edge 69/69 at each viewport. Those evidence claims were read in the handoff. This lane ran the focused UI build and API harness only. The full regression suites, browser, gate selftest, Unity and Windows build were omitted per packet. [Implementation handoff and evidence](https://github.com/jermur788/CCF/blob/70d61e4a80bd0d6107ba9c6655e8c5ef36d14490/Docs/Handoffs/MultiRack-Simulation-20261116/woodland-inspection/IMPLEMENTATION-HANDOFF-PART2.md)

The Manager owns the follow-up fix, integration authorization and coordination/dashboard update. The dashboard state was not verified in this lane. Current integration overlap includes Commands, History, Saves, Stand, View, Woodland and UI smoke. Combined-candidate gates remain pending.

**D-030: NOT merged to integration/2d-sim or main.** This publication adds only this Markdown to a separate review docs ref.
