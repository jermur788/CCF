# Sen-1 #890 — Reuse the existing three-page onboarding in Compact30

**Issued:** 2026-11-20 after owner personal Editor feedback at 21:44 UTC.  
**Status:** PACKET READY / NOT ACTOR-RUNNING / NO PHYSICAL LEASE / NO CODE EDIT.  
**Issue:** https://github.com/jermur788/CCF/issues/890  
**Difficulty:** LOW/MEDIUM bounded Unity C# startup UI hookup, with input/modal regression risk.  
**Provider:** Native Unity-capable GPT-6.1 Sol/Codex or an available qualified equivalent; browser GitHub-only planning cannot certify or edit local Unity. This packet does not start a worker.

## 1. Owner instruction and observed result

The owner personally walked the exact #886 candidate and reported that movement/collision is now fine. **This is limited owner feedback, not completion of all #886 tests or full Scenario One acceptance.** The missing player-facing feature is the initial three information pages. The owner explicitly says **"I want to just reuse them"**: restore existing approved production panels, not new design/copy/art, not a new menu architecture.

**Minimal genuine demonstration:** entering a **fresh Compact30 Editor scenario** opens the same three-page UI in order, Back/Next/Begin works, forest controls remain blocked until Begin, and a returning/loading player does not get an unwanted repeated startup sequence. On Begin, the no-rack Compact30 H1/assessment/Crop/plant/Work Plan loop is unchanged.

## 2. Exact source evidence (GitHub read-only, not native verification)

- **Source parent:** `81e65590fe15832cfc95c74ef0f02fcfb91d10d1`, branch `fix/sen1-886-compact30-tree-collision-20261120-v1`. This includes the #873 composition, #876 marker correction and #886 physical collision fix. Do not base on main or import #857.
- Source files on that SHA inspected directly: `Assets/ForestPrototype/UI/ScenarioOneOnboardingView.cs` implements the three live pages, buttons and page state; `ScenarioOneOnboardingCopy.cs` has approved text; `MenuHelpView.cs` has `ShowScenarioIntroduction`, `Onboarding`, modal/input handling and close/continue behavior; `StandaloneSessionMenu.Startup.cs` explicitly opens the introduction after Start New, not Continue.
- Three image assets and metadata already exist at `Assets/ForestPrototype/UI/Resources/Onboarding/page-1.png`, `page-2.png`, `page-3.png`; `ScenarioOneOnboardingView` calls `Resources.Load<Texture2D>("Onboarding/page-" + (PageIndex+1))`.
- **Likely immediate cause:** `ScenarioOneUiRoot.cs` lines ~148–149 show auto-introduction only if there is no `StandaloneSessionMenu` **and `!Compact30Session.IsActive`** and the manager is in a clean year-zero new-game state. Thus the Editor Compact route is deliberately excluded. Verify actual component lifecycle, state and loader in native Unity before editing; do not assume this is the only gate.
- Existing authoritative feature package: `specs/scenario-one-three-page-onboarding-v1/`, including production-copy.md, artwork reference manifest and verification. Spec notes status VERIFYING pending acceptance; owner asks only to **reuse existing implementation exactly**, not redesign or infer a new approved UX.
- Titles already present: “What is Continuous-Cover Forestry?”, “How CCF works over time”, “Scenario One — Transforming a Sitka Plantation”. Do not replace wording with a new "what is simulator" page.

## 3. Scope and custody

**Class:** PACKET ONLY; re-enable an existing approved feature in a new entry path, not design a new onboarding workflow. If inspection shows new persistence, startup-screen architecture, broad UI changes, or material product ambiguity is actually required, **STOP** and return the finding to #425 / authorised Scenario One manager.

**Proposed native source lane:** create a unique legal branch/worktree from exact `81e65590...`, name and location chosen by native actor only after live GitHub/physical CCF-Ops conflict checks; propose `fix/sen1-890-compact30-onboarding-reuse-20261120-v1` and new isolated worktree, not the owner's #886 test checkout. GitHub docs branch for this packet is not a source lane.

**Production write allowlist, one primary writer:** `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` only, to remove the erroneous Compact entry exclusion while keeping fresh-game and Standalone-session guards. Read-only linked scope may include `MenuHelpView.cs`, `ScenarioOneOnboardingView.cs`, `ScenarioOneOnboardingCopy.cs`, `StandaloneSessionMenu.Startup.cs`, `Compact30Bootstrap.cs`, `Compact30Session.cs` and `ScenarioOneManager.cs`; **reading these grants no write rights**. New targeted verification scripts under `Tools/Verification/Sen1CompactOnboardingReuse/` (outside Assets) and one durable native result Markdown are allowed. A second production path requires an explicit scoped amendment after evidence. Do not edit approved text, artwork, spec, UI theme, scene/prefab, inspector, saves, packages, runtime ecology, tree physics or any other production source.

Preserve #886 175-tree trunk/stump behavior, #876 110-check markers, Compact no-rack policy, H1 and exact four planting positions, current Work Plan, old Legacy40/Enlarged80, protected saves, frozen Reference Future/determinism and D-091 physical locks. No forced unlock; one writable Unity lane, exclusive Editor lease under actual Ops admission. Use private XDG_CONFIG_HOME to preserve user preferences and saves. Context policy: at G0 inspect live `Docs/Project/coordination/active-tasks.md`, one current full context stamp and exact actor/branch/HEAD/source overlap; stop only conflicting action if ownership is held.

## 4. Narrow qualification, no full T3

1. Unity 6000.6.0f1 actual import/compile without introduced C# errors, clean task-owned worktree, normal lock release.
2. Fresh **Compact30** menu `CCF → Player Test → Play Compact30` must display existing Page 1 before interactive walking. Verify exact Page 1, Page 2, Page 3 titles, approved artwork presence, buttons Next → Next → Begin, Back, keyboard arrows and Escape semantics; page one cannot be accidentally skipped.
3. While the modal is open, mouse/keyboard/Work Plan/mark actions cannot leak into forest gameplay. After Begin, player and cursor regain ordinary working controls. No duplicate or automatic reopening on ordinary HUD redraw.
4. Verify normal historical standalone Start New still presents the sequence exactly once; a supported Continue/load does **not** replay it. If Compact has no implemented persistence, **do not add it** merely to test Continue; report that Compact load is unavailable and test applicable historical Continue safely in isolated storage.
5. Verify a narrow initial Compact smoke: 175 living Sitka, normal H1 arrival, J/Y stand assessment, C/X marks; no unexpected rack UI. Use existing `Tools/Verification/ScenarioOneThreePageOnboarding/` gate where practical, but do not weaken or edit it or claim its old historical run is verification on the new candidate.
6. Prefer a 720p + 1080p render inspection if the reused UI shows clipping, missing textures, scaling or modal issue. Avoid unrelated art/forestry qualification. Preserve all native logs/screenshots in evidence and summarize in GitHub Markdown.

## 5. Handoff and gates

Publish one durable source/native **#890 result** on a worker source branch with exact commit, author/provider, file-list diff, context/branch/physical lease and release, verification including actual Editor UI screenshots or capture, negatives, any drift restoration, test project path and exact owner menu steps. Status labels must distinguish IMPLEMENTED, AUTHOR VERIFIED, OWNER TEST PENDING, INDEPENDENT REVIEW (if any), MAIN INTEGRATED and OWNER ACCEPTED. Do not auto-merge to main or build Windows. Owner should personally see the three pages and then return to testing the existing Compact gameplay. #425 or an explicitly appointed delivery manager handles any G4/G5 integration/review decision; #887 art manager does not acquire general gameplay source authority.

## 6. Complete native dispatch message

> **SOL/CODEX LOCAL UNITY — SEN-1 #890, reuse original three onboarding pages in Compact30.** Difficulty LOW/MEDIUM; PACKET ONLY; one writer, official physical CCF-Ops admission and exclusive Unity lease required. Binding DIRECT GitHub handoff: **https://github.com/jermur788/CCF/blob/docs/sen1-890-reuse-onboarding-packet-20261120-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-890-REUSE-THREE-PAGE-ONBOARDING-COMPACT30-REPAIR-PACKET-20261120.md**. Owner first-hand at 21:44 UTC reports walking on #886 fixed candidate is fine, but Compact30 Editor startup omits the three original approved information pages; owner explicitly requests **REUSE**, no redesign. Exact source parent `81e65590fe15832cfc95c74ef0f02fcfb91d10d1` (#886), NOT main. GitHub inspection finds existing `ScenarioOneOnboardingView`, production copy, 3 image resources, and `ScenarioOneUiRoot.cs` fresh-game intro condition that excludes `Compact30Session.IsActive`. First verify actual lifecycle; proposed one **production WRITE path** only `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`; read-only linked files are not write grants. New targeted scripts allowed outside Assets and one durable result Markdown. Fresh Compact30 must show Page1 → Page2 → Page3 → forest, Back/Next/Begin, correct modal/input blocking, no repeats; historical startup/Continue unchanged. Native Unity 6000.6.0f1 targeted real tests, no full T3. Preserve #886 collision/#876 markers/175 roster/H1/no racks, old saves/Reference/determinism, prefs, unrelated dirty lanes and D-091 leases. If extra production file/spec change or source ownership conflict appears, STOP and request scoped authority. Return exact new owner-test checkout and GitHub evidence. No main merge/Windows build, no auto acceptance.
