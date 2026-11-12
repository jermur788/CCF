# Manager #424 — first-rack field marking identity and guidance: conditional native safety witness

**12 November 2026. Task [#438](https://github.com/jermur788/CCF/issues/438) within ONE unified UX repair stream [#440](https://github.com/jermur788/CCF/issues/440). Status: BLOCKED — protected user's Editor session still owns Unity; this is a complete future dispatch packet, NOT an active worker authorisation.**

**Purpose:** independently determine whether the user-observed first-rack marking confusion is (A) missing/late minimap/HUD target, (B) player-facing order vs persistent rack-number mismatch, (C) wrong tree↔rack↔work association, or (D) inconclusive. Provide raw native receipts before any production fix; do **not** edit sources.

**Difficulty: HIGH** — may cross world-space rack geometry, entrance tree identity, HUD/minimap presentation, spray raycast and Work Plan pending orders, and must protect personal save/Unity/lease.

**Best-suited model/provider:** Sol/Codex local T3 Code using GPT-6.1 Sol High, or a similarly capable local repo+Unity engineer; only one owner of the machine-wide Unity lease. Manager #424 reviews. No parallel Unity with Objective 2 #441; that stream is docs-only research currently.

**Exact source candidate:** `integration/scenario-one-personal-playtest-d073-20261112-v1`. Exact qualified head from previous handoff: `55863f1b7719011bba16b3f3ab377e379c0b988a` (internal verification only). **Do not advance/cherry-pick/rebase this candidate as part of a diagnostic.** Prior personal #382 and frozen Reference Future v1 are never writable.

**Conditional isolated lane:** only after real safe-entry checks, propose branch `qualify/scenario-one-rack-entrance-witness-20261112-v1` and worktree `/media/jer/Files/CCF/Operations/CCF-S1-RackEntranceWitness-20261112` from the qualified candidate in a **new, disposable** sandbox if exact path/branch are free and disk capacity/Unity license permit; otherwise STOP and report BLOCKED rather than choosing an unregistered lane or touching `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` (the user's guarded personal test). The proposal is **not a claim that this local path exists, is safe or has capacity**.

**Claim:** [#438](https://github.com/jermur788/CCF/issues/438) is the safety-diagnostic execution claim, currently BLOCKED/NOT RUNNING. Before any local work, Manager must confirm safe user exit and actual Unity lease/lock, update live `active-tasks.md` as sole writer for exact owner/lane/branch/worktree/file set and authorise #438; worker then records [RUNNING] actual model/provider and starting HEAD. **No launch merely because this packet exists.**

## Mandatory before starting

1. Obtain **user's guarded launcher completion/terminal receipt** confirming Editor fully exited, machine lease released and original save/PlayerPrefs hashes unchanged. If unavailable, STOP (do not run source or Editor operations that could intrude).
2. Refresh `Docs/Project/coordination/active-tasks.md` and actual Ops board, #441, any other source/Unity lane; confirm exclusive owner. If another Editor/research worker holds Unity or dirty work conflicts, STOP. Never kill a process, force-release a lease, stash/clean/reset/discard another lane, or use Unity Hub.
3. Verify exact qualified candidate HEAD; confirm new worktree location clean, branch collision-free and adequate free disk, known Unity 6000.6.0f1 and isolated disposable project/profile/saves. Protect the known 37 valued save files and previous candidate, and do not use the user's isolated game profile for diagnostics. Any uncertainty = BLOCKED.
4. Set Tier-1 diagnostic scope only, no gameplay source edits, persistence/schema/asset changes, no Windows/Player build, no story advance into dangerous approval. Use source/hash identity checks as an independent negative control.

## Native witness — required reproducible sequence

- Use an authentic **new disposable Enlarged80 Scenario One** state with five suggested rack plans (do not fake/synthesize rack IDs/entrances). Inspect their actual persisted IDs, world-space line/endpoints, entrance tree IDs, path side and south/north ordering. Capture native 1280×720 before first painting (upper-left directional box, minimap, on-screen rack order).
- Dismiss Introducing Tree Marking **before spraying**. Record `TryGetRackFieldTarget` exact returned tree/rack, any HUD destination, minimap marker and range. Do not assume screen has a target merely because the source returns one.
- Walk the player's reported minor track route to the lowermost/southern entrance and spray the **actual eligible target entrance tree** with real native input; capture frame immediately before and after. Log the tree ID, resolved rack ID, spray success/failure, `EntranceMarked`, HUD/minimap target and any “Rack 1” label. Compare ordered south-to-north physical visit index versus **stable persisted rack ID**; they are separate fields and cannot be silently conflated.
- If no unexpected mapping, continue controlled spray to remaining four valid entrances, inspect 5/5 counter and list each confirmed rack's mark tree. Inspect **pending ordinary work orders** and exact rack/tree associations in a read-only view. **Do not approve or advance work** merely to complete the diagnostic; if wrong work association appears, stop immediately and save no affected game.
- Record bounds and visual legibility of existing 196-unit minimap at 720p/1080p, and whether a plausible increase conflicts with HUD. This is diagnostic evidence only; no resize implementation.
- Preserve screenshots (non-black native captures), raw logs, small source/identity table and a human-readable path/route diagram. Record whether wrong association was observed, suspected but unproven, or disproven for the tested fixture; avoid extrapolating one case to all saves/worlds.

## Classification and stop gates

| Class | Proof | Consequence |
|---|---|---|
| A presentation timing/waypoint only | Correct rack/tree/work association across tested sequence; first cue missing/late | UI source fix may later target initial HUD/minimap cue |
| B ordered label only | Correct association; stable ID order differs from accessible bottom-up visit order | Later design decision on visit-label index, **no stored rack ID renumbering** |
| C actual wrong association | Mark recorded against wrong rack/tree or pending work mismatched | **STOP, NEEDS CHANGES**, Manager and possible approved spec/data repair before proceeding |
| D incomplete or unsafe | Missing reliable native/profile/lock/screen/ID evidence | **BLOCKED / NOT VERIFIED**, not PASS |

**No game/source file modifications or broad test suite:** Tier-1 FAST diagnostic first; escalate to targeted Tier-2 only if a fix is separately authorised. Preserve D-064, D-065, D-076 and Legacy40/frozen Reference compatibility.

## Output and handoff

Publish exact GitHub Markdown handoff `Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/SOL-438-RACK-ENTRANCE-NATIVE-ID-WITNESS-RESULT.md` on a clean docs-only result branch distinct from the protected playtest candidate, with candidate/owner/actual lease, stop/start custody, screenshots and raw log links, five-row rack/tree/order table, classification A/B/C/D and explicit PASS/FAIL/NOT VERIFIED statements. Do not leave evidence only on local disk. At completion mark claim [HANDED-OFF], link result and close it; Manager examines original evidence and issues APPROVED/NEEDS CHANGES/REJECTED before launching coherent #440 UX implementation. The next implementation owner must coordinate #441's source/interface boundaries.

**No Windows build before user personally playtests the eventual Scenario One candidate and explicitly authorises it; the current human playtest has not been reported complete or accepted.**

**User action required:** before this dispatch can run, safely finish the guarded Unity Editor session and supply the final terminal save-integrity / lease-release receipt. For now, **no local worker action**. Dashboard/Drive mirror: UNVERIFIED.
