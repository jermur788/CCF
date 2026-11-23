# CCF #885 — Opus 5.5 implementation result: final navy forest UI on the real `/multi` page

**IMPLEMENTED ON TASK BRANCH.  COMPILED: YES — clean rebuild, 0 warnings, 0 errors.  REAL `/multi` RUNTIME: YES — final re-check on the owner-iterated page passed 49 of 51 scripted checks on real stand data; the same 2 known non-passes as before (§5), neither a code defect.  OWNER PLAYTEST: iterated live with the owner on 2026-11-22 ("Looks good"), not yet formally accepted.  REVIEWED: NO.  INTEGRATED: NO.**

**Revision 3 (this commit)** records the owner-directed visual iteration made after the real-runtime check in revision 2 (`a42720bd`). It covers the map colours, the "Competition" label, fading, 2× enlargement and spread, Crown Brackets marks, crown images with a page-policy change, and three display fixes. It adds image provenance (§4a) and a crown-overlap model limitation for routing (§6). Revision 2 replaced the first version (in `3e6a6add`), which said "COMPILED: NO / not run" (deviation **D3**).

# IMPLEMENTATION HANDOFF

## 1. Identity, Routing & Execution Provenance

- **Task / Issue ID & Direct URL:** CCF #885 — https://github.com/jermur788/CCF/issues/885
- **Worker / Session Name:** T3 Code thread `1f1a1a27-d17e-445f-b302-9670616e7494` ("Forestry Simulator Dashboard Redesign"). It was originally the Astra design thread (OpenAI gpt-6-astra). After that session hit its usage limit, the owner re-bound it to Claude Opus 5.5 with the direct instruction "I want you to implement this design" (**D2**). Because this thread is now the Primary author, it is **NOT eligible** to act as independent reviewer.
- **Role:** Primary
- **Task Class:** PACKET ONLY
- **Model Identity:**
  - Requested: `claude-opus-5-5` (owner instruction and design packet). Selected: `claude-opus-5-5`. Resolved: `claude-opus-5-5`, as stated by the runtime model identity given to this session. Executed: `claude-opus-5-5`, from the same self-report; no external attestation was observed.
  - Effort: requested UNKNOWN, executed UNKNOWN (not observed).
  - Policy note: DeepSeek V4.1 Flash (opencode-go) acted only as the D-091 physical custodian, issuing the custody receipts. It wrote no source in this lane. This checkout's local git `user.name` is "DeepSeek V4.1 Flash orchestrator", so both commits on this branch set the author explicitly to `Claude Opus 5.5 <noreply@anthropic.com>`.
- **Tools Actually Used in This Session:**
  - Claude Code (Read/Write/Edit/Grep/Bash/PowerShell/ToolSearch).
  - T3 `t3_worktree_handoff` (failed, D1); T3 `html_preview`/`html_render` (design mock-ups shown to the owner).
  - T3 `delegate_task`, design-only children that wrote only to `C:\Users\hp\.t3\outputs\…` and changed no repository file:
    - `gpt-6-astra` (Codex pool): three mark designs; the owner chose "Crown Brackets".
    - `opencode/gpt-6.2-sol`: failed before doing any work (OpenAI/OpenCode credit balance); the owner switched to `gpt-6.1-sol`.
    - `gpt-6.1-sol` (Codex pool): three procedural crown designs. One ("Atlantic Whorls") was tried and then replaced by the owner's concept-board images.
  - WSL `git`; `gh` (read-only API reads, then branch pushes).
  - `/home/jer/.dotnet/dotnet` 8.0.134: incremental rebuilds during the iteration, then one clean rebuild (§5).
  - `python3` (splice, masking and edit scripts); `node` v26 (syntax checks; CDP drivers; sprite cutting via the browser canvas).
  - Already-installed Microsoft Edge, headless over CDP.
  - No package or tool was installed (no PIL/numpy; image processing used the browser canvas).

## 2. Repository, Branch & Durable Handoff Links

- **Repository:** https://github.com/jermur788/CCF
- **Worktree / Lane Path:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-AstraOpusUI-20261122`
- **Branch:** `feat/multi-885-astra-opus-ui-20261122-v1`
- **Base SHA:** `a692e1bf5d21304d06e696db1f47d1988ff80ee1`
- **Implementation commit (source + first result revision):** `3e6a6add63e3838f770170e2ba2974b4bc1fe298` — https://github.com/jermur788/CCF/commit/3e6a6add63e3838f770170e2ba2974b4bc1fe298
- **Revision 2 commit (real-runtime result):** `a42720bd4f467445e27bf663a3a435c6b230debc`
- **HEAD SHA:** the commit that adds this revision 3 (child of `a42720bd`). A file cannot contain its own commit hash, so the exact 40-character SHA is reported in the return message and is the branch tip.
- **Compare URL:** https://github.com/jermur788/CCF/compare/a692e1bf5d21304d06e696db1f47d1988ff80ee1...feat/multi-885-astra-opus-ui-20261122-v1
- **Packet Context Commit SHA:** `3a67d0987be0cf40d007892739d2ad295c84f0e0`
- **Current `main` HEAD SHA Verified Remote:** `3a67d0987be0cf40d007892739d2ad295c84f0e0` (fetched 2026-11-22T17:07:38Z; re-verified after the push — see the return message)
- **GITHUB HANDOFF PATH:** `Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-OPUS-UI-IMPLEMENTATION-RESULT-20261122.md`
- **GITHUB HANDOFF COMMIT:** the HEAD commit above
- **DIRECT GITHUB HANDOFF URL:** https://github.com/jermur788/CCF/blob/feat/multi-885-astra-opus-ui-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-OPUS-UI-IMPLEMENTATION-RESULT-20261122.md
- **EXTERNAL EVIDENCE ARCHIVE:** `/home/jer/Development/CCF-885-Astra-UI-Evidence-20261122` — mode `700` (verified). The capability exists only under `private/` (mode-600 files). Every other file is redacted and was scanned: the capability occurs 0 times outside `private/`.

**Authority chain**
- Design implemented: https://github.com/jermur788/CCF/blob/docs/multi-885-astra-final-forest-ui-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-FINAL-DESIGN-AND-OPUS55-IMPLEMENTATION-PACKET-20261122.md (`305582e2c6edd08f2aece72ed85dd93e5bdb9640`)
- D-091 pre-admission: `ce3417fbe36e9ca4923594a8f4b2dc5a35d10166`
- Amendment (one compile, one bounded runtime, issued 2026-11-22T09:28Z): `28abb53de644d8228e000b3efc8cb73839ec131e`
- Custodian reconciliation and runtime clearance (2026-11-22T16:47Z), `4231039ce1c9bffd674f42e0fb144842fdd91931`: https://github.com/jermur788/CCF/blob/docs/multi-885-astra-reconcile-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-D091-ASTRA-OPUS-UI-LANE-RECONCILIATION-AND-RUNTIME-CLEARANCE-20261122.md

## 3. Live GitHub Coordination & Physical Lock Snapshot

- **Live `Docs/Project/coordination/active-tasks.md`:** last changing commit `3a67d0987be0cf40d007892739d2ad295c84f0e0` (2026-11-21T09:25:49Z), blob `9124d7854d071d2b612de91608ed64d35c1773b7`; re-fetched 2026-11-22T23:57:42Z, unchanged since the 17:07Z fetch.
- **This Task's Live Claim / Lock Status:** the board has no row for this lane or branch. Custody for revision 2 came from the D-091 receipts above. Revision 3's source edits rest on the owner's direct in-session instructions, not on a D-091 amended grant (see **D6**). The "MULTI #885 … 2D MANAGER" row is present and active. `active-tasks.md` was not edited.
- **Active Overlapping or Adjacent Claims / Lanes:** none for `PlayerMultiApi.cs`, `PlayerApi.cs` or this branch. Adjacent rows (#886 Compact30, #887, #891, #894, #897) concern Unity, asset or research work and were not touched.
- **New #885 comment:** `7087370178` (2026-11-22T23:10:13Z, Manager), which:
  - approves exactly the three display fixes;
  - links handoff `1bde98db2348ab988073f24bed00a87ce7c853f4`;
  - requires a narrow D-091 amended source/build/commit grant first;
  - routes crown closure to [#899](https://github.com/jermur788/CCF/issues/899).
- **Machine-Wide Unity Lock State:** untouched. No Unity, Unity Hub, `Assets/` or Unity-lock file was read for locking, acquired or written.
- **Protected Listeners Alive After Run:** `127.0.0.1:33767` (PID 1750123), `127.0.0.1:43649` (PID 1806361) and `127.0.0.1:46593` (PID 839989) were listening before, during and after both runtime checks (revision 2 and the final revision 3 re-check), and were never bound, proxied or signalled. Their process start times are unchanged (Fri Nov 20 18:08, Fri Nov 20 21:00, Sun Nov 22 08:25).

## 4. Exact Changes & Scope Compliance

- **WHAT CHANGED:**
  - Revision 1 (`3e6a6add`): the owner-chosen final design, inside the `MultiAreaPage` Html, Css and Js literal interiors.
  - Revision 2 (`a42720bd`): this result file only.
  - Revision 3 (this commit): the owner-directed iteration listed below, inside the same three literals, plus **one policy line in `PlayerApi.cs`** (outside the literals; see the next block), plus this file.
- **FILES CHANGED by revision 3 (`git diff --stat a42720bd…HEAD`):**
  - `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`: +217/−145, all inside the three literal interiors.
  - `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs`: +2/−1.
  - This result Markdown.
  - No file added; nothing under `wwwroot/`; no csproj, props, Core, simulation, fixture, `tests/` or `Assets/` change. `AnnualRules.cs` and `CcfSourcePins.cs` are unchanged.
- **SCOPE CHECK:** Yes for `PlayerMultiApi.cs`: the masked hash is identical (below). **No** for strict literal-only scope: `PlayerApi.cs` changed, by owner choice (**D7**).
- **PLAYER-FACING RESULT:** see §5 — every item there was observed on the real `/multi` page with real stand data (MA-A: 171 trees, 5-vertex outline; MA-B: 73 trees, 4-vertex outline).
- **UNRELATED WORK & CLEANLINESS:**
  - Before staging, `git status --short` listed exactly the two source files plus this file. No untracked files, CR bytes or mode changes.
  - Build output (`.p0ref/`, `src/{Core,Ui}/{bin,obj}`) is git-ignored.
  - All my test servers are stopped, and no build-server process is running.

### Changes outside the three literals (exact)

| File | Lines (HEAD → new) | Change | Why |
|---|---|---|---|
| `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs` | 31 → 31–32 | Added `img-src data:;` to `PagePolicy` and one comment line above it | The page policy was `default-src 'none'`, which blocks every image. The owner chose (2026-11-22) to use crowns cut from their concept board as images rather than redrawn vector art. The images are embedded as `data:` WebP URIs in the Js literal, so no new route, file or origin is needed. `img-src data:` allows only inline data images; scripts, styles, connections, frames and other origins are unchanged. `PagePolicy` is shared with the single-area page, which now also permits inline data images (it uses none). |

No other byte outside the three literal interiors changed in either file. For `PlayerMultiApi.cs` this is proven by the masked hash (`34126c39…4c16` at `a692e1bf`, `a42720bd` and the working tree); for `PlayerApi.cs` by `git diff -U0` (one hunk, shown above).

### Revision 3 — owner-directed visual iteration (all inside the literals unless stated)

1. **Map colours.** Navy surround (`--exterior-1/2` `#14263a`/`#0f1c2b`) and dark slate stand floor (`--interior` `#1b2430`) replace the pale blue-grey surround. Grid, labels, outline, north arrow and scale bar are lightened to stay readable; legend swatches match.
2. **"CI term" → "Competition"** column heading in the competitor list.
3. **Fading.** When a reference tree's competition is shown, every tree outside the focus group (reference + top 5) fades to 25% opacity (was 72%).
4. **2× enlargement and spread.** The focus group is drawn at a fixed 2× crown radius, on top, for every selection.
   - The reference stays at its true position. Competitors move outward along their true bearing until crowns barely touch (crowns may overlap by 20% of their radii; extra gap 0 px).
   - A dotted line and dot join each moved crown to its true trunk position.
   - Display only: data, positions, radii and hit-testing are unchanged.
5. **Crown Brackets marks** (design by Astra, owner choice), replacing the full rings:
   - the reference gets white corner brackets and an **R** tile;
   - an inspected non-reference tree gets dotted side brackets and an **I** tile;
   - competitors get a short underline and a square rank tile; a draft tick is a ✓ box beside the rank tile;
   - pending fell: dashed underline and **P**; approved fell: double underline and **A**;
   - crop **C**, dead **D**, felled **X** tiles.
   - Each tile kind has its own corner, which fixes an older overlap where a crop's C covered its P/A. The legend uses the same functions.
6. **Crown images.** Living trees are drawn from three crown sprites cut from the owner's concept board (provenance in §4a), plus mirrored copies (six variants), each turned and stretched by its ID seed. Dead and felled symbols are unchanged procedural art. This needed the `PlayerApi.cs` policy line above.
   - Tried and replaced along the way: the brighter-green procedural trees, then GPT-6.1 Sol's "Atlantic Whorls". Neither is in the final code.
7. **Three display fixes** (approved by the Manager's `7087370178`):
   - (a) moved crowns are held wholly inside the visible map, clear of the navigation dock, with re-separation after each clamp; the group is never shrunk;
   - (b) each rank tile (and ✓ box) takes the first free corner that avoids other tiles, the Reference tag and the middle of the highlighted crowns;
   - (c) underlines are drawn across the lower crown (0.8 × radius) instead of floating below sparse tips.

### 4a. Crown image source, provenance and rights

| Item | Value |
|---|---|
| Source image | Owner-supplied concept board titled "Spruce Crown Assets — Top-Down Concept Board", attached by the owner in this T3 thread on 2026-11-22 with the instruction "use this" |
| File as received | `ChatGPT Image Nov 22, 2026, 10_35_34 PM.png`, PNG, 1254 × 1254 px, 2,379,325 bytes, sha256 `9888a28bc74426f77353f917961e94627376b60430e1471732f0e2ccefbb7058` |
| How it was made | **AI-generated.** The filename indicates ChatGPT (OpenAI) image generation. The owner generated and supplied it; this worker did not see the prompt or generation settings, so they are UNKNOWN. |
| What was taken | The three large crowns in the top row, cut as 256 × 256 px squares. Crops (centre x, centre y, radius in board px): (215, 292, 186), (603, 300, 180), (946, 292, 142). Background removed by a luminance threshold (alpha = clamp((luma − 0.062) / 0.07)) times a soft round edge (last 10% of radius). Exported as WebP quality 0.86 in headless Edge (canvas; no library installed). |
| Embedded sprites (in the Js literal as `data:image/webp;base64,…`) | `spruce-0.webp` 43,480 B sha256 `fd181894917da9a623b82df3102111df275fc1bcae1c48141f27743c3b7e2495`; `spruce-1.webp` 43,274 B sha256 `a66514a35e23f9df69f6526beb875210fbf923462ba62fbdb23c1834b77921f3`; `spruce-2.webp` 43,052 B sha256 `14ec15bd4b57624208c58d700352394d380d82c649bbfea967679442026595bb` (about 172 KB of base64 in source) |
| Licence / rights | **Owner-asserted; not independently verified.** The owner supplied the image for this project and directed its use. Rights in AI-generated output depend on the generator's terms and on local law. This worker makes no legal determination and found no third-party stock, scanned or game art identifiable in it. The owner or Manager should confirm the D-032 rights position before any release beyond this lab. |
| Biological meaning | None. The crowns are artistic; variant, rotation and stretch encode no health, age or habitat value. |
| Archived copies (not committed) | Board and sprites under `<evidence>/crown-images/`, together with the cutting script `cut.mjs`. |

### Outside-literals byte identity (rerunnable)

```
before.masked.sha256 34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16   (a692e1bf:…/PlayerMultiApi.cs)
after.masked.sha256  34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16   (a42720bd, revision 2)
after.masked.sha256  34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16   (revision 3 working tree, re-run before commit)
```

The custodian ran this script independently, got the same hash, and confirmed that the guard rejects a deliberate mutation of a code token.

```python
#!/usr/bin/env python3
# mask_literals_v1.py — CCF #885 outside-literals guard for Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs.
# Replaces the interior of exactly the three MultiAreaPage raw string literals (Html, Css, Js) with a fixed placeholder,
# keeps every other byte (including the opener/closer delimiter lines and line endings) and prints the SHA-256 of the
# result. Equal output for baseline and candidate proves no byte outside the three literal interiors changed.
# Usage: python3 mask_literals_v1.py <file>   or   git show <rev>:<path> | python3 mask_literals_v1.py -
import hashlib
import sys

OPENERS = (b'    internal const string Html = """', b'    internal const string Css = """', b'    internal const string Js = """')
CLOSER = b'""";'


def mask(data: bytes) -> bytes:
    out, inside, seen = [], None, []
    for line in data.split(b'\n'):
        if inside is not None:
            if line == CLOSER:
                out.append(b'<<MASKED ' + inside + b' INTERIOR>>')
                out.append(line)
                inside = None
            continue
        out.append(line)
        if line in OPENERS:
            if line in seen:
                raise SystemExit('duplicate opener: ' + line.decode())
            seen.append(line)
            inside = line.split(b' ')[7]
    if inside is not None:
        raise SystemExit('unterminated literal')
    if tuple(seen) != OPENERS:
        raise SystemExit('expected exactly the Html, Css, Js openers in order; saw ' + repr(seen))
    return b'\n'.join(out)


if __name__ == '__main__':
    raw = sys.stdin.buffer.read() if sys.argv[1] == '-' else open(sys.argv[1], 'rb').read()
    print(hashlib.sha256(mask(raw)).hexdigest())
```

### What the page now does (implementation summary)

- **Layout:**
  - Navy top bar with Year, a two-line Advance button ("Both areas · does not execute work") and Re-read.
  - Area A, Area B and Work Plan summary cards. The local draft count is separate from the pending count.
  - One compact notice with Details.
  - Grid of map | legend (232 px) | side panel (380 px) at 1920 px; 176 / 310 px from 1024–1599 px; stacked map → side panel → legend below 1024 px or at 200 % zoom.
- **Map:**
  - The viewBox always matches the pane's aspect ratio. Fit places the whole area beside (or below) the navigation dock, never under it.
  - All eleven graded zoom levels are kept, and pan is clamped.
  - Hit-testing is unchanged: the browser's `getScreenCTM()` with the server's `api/multi/hit`.
  - Navy exterior with a metric grid (revision 3; it was light blue-grey in revision 1); dark slate interior clipped to the real polygon; dashed boundary; passive north indicator; a scale bar computed from the current zoom; a status pill.
- **Dock (F2/F3):** N/W/E/S compass, a separate + / − / Fit column, and Area A/B buttons drawn from the real outlines (`aria-pressed`, existing `showArea`). The old duplicate area switch and the overview inset are removed. All targets are at least 44 px.
- **Trees:**
  - Revision 3: six crown variants drawn from the owner's concept-board sprites (§4a), scaled to each tree's server `radiusM` and turned and stretched by its ID seed. The revision 1 procedural spruce symbols are removed.
  - Each tree's drawing is cached and rebuilt only when its server item changes.
  - Felled trees are stumps with X; naturally dead trees are bare branches with D (procedural).
- **Legend (F4):** the same mark functions as the map draw all eleven states. Visible by default; Hide/Show is kept in memory across area switches (no web storage).
- **Competitors (F1):**
  - Selecting a living tree makes it the area's reference and reads its competition immediately.
  - The five ranked competitors are native checkboxes in a local, per-area draft. Ticking never moves the reference.
  - Ineligible rows show why they are disabled.
  - Draft count, Clear and "Add selected N to Fell plan" sit in a sticky dock with the outcome report.
  - "Fell this tree instead" offers the reference itself (living, non-Crop) through the same path.
  - `reasonToSend()` precedence is unchanged: note, else release intent (Crop reference only), else `Owner-selected felling`.
- **Sequential add:**
  - The ID list is frozen when Add is pressed, and one operation-wide guard holds until the sequence ends.
  - Each `pending/add` carries the fence of the latest confirmed view. Requests are never parallel and never retried automatically.
  - It stops at the first 409, guard refusal or unconfirmed outcome, and reports confirmed, refused/unconfirmed and not-attempted IDs.
  - An unconfirmed outcome locks commands until a successful Re-read.
- **Unchanged by source:** `call`, `currentFence`, `run`, the lock and re-read rules, stale-response guards, Execute confirmation freeze/withdraw, consent flow, exact-ID inputs, focus restore, `textContent`-only text, no `innerHTML`, no inline style or handlers, no other origin.

## 5. Verification Evidence

### 5.0 Final verification of revision 3 (2026-11-22 to 23, before this commit)

- **Clean rebuild (D4 rule):** run from `Tools/MultiRackSitkaP1Lab` as
  `TMPDIR=/tmp/ccf885-build-tmp DOTNET_CLI_TELEMETRY_OPTOUT=1 dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore --no-incremental --disable-build-servers`.
  - **Build succeeded, 0 Warning(s), 0 Error(s)**, exit 0 (warnings are errors via `Directory.Build.props`).
  - `--no-restore`: no package restore and no network fetch.
  - External TMPDIR held 0 files afterwards. No MSBuild, VBCSCompiler or build-server process remained.
  - `.p0ref/`: no file newer than the build; still pins `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. **No P0 rebuild.**
  - `sitkap1-ui.dll` sha256 was identical before and after the clean rebuild (`c7685b17eb8422493a5a6e7624ecb8f092ee5990f37ed2fb7047cd45919464fc`), so the runtime re-check below ran exactly this build.
- **Scope:**
  - Masked hash `34126c39…4c16` is unchanged.
  - `git status --short`: ` M …/PlayerApi.cs`, ` M …/PlayerMultiApi.cs` (plus this file).
  - `git diff --stat`: PlayerApi.cs 3 (+2/−1), PlayerMultiApi.cs 362 (+217/−145).
  - Forbidden-path scan (`Assets/`, `tests/`, `AnnualRules`, `CcfSourcePins`, `wwwroot`, csproj, props): none.
  - CR bytes 0; mode changes none.
  - No 64-hex capability string in the source diff.
- **Real `/multi` re-check:** own server PID 594927 on OS-assigned 127.0.0.1:46181, run on a scratch copy of the stand data. The server itself reports "Session only — nothing is saved." Drivers: the same `real.mjs` and `real-followup.mjs` as revision 2, with the capability read from a mode-600 file under `<evidence>/private/` and never printed. Results: main run 44 of 46, follow-up 5 of 5, so **49 of 51**. The 2 non-passes are the same known, non-defect items as in revision 2 (reason reset by the scripted A→B→A switch; the browser network-log line for the deliberate 409). The follow-up confirmed the exact release reason with **0 console errors, 0 exceptions and 0 CSP violations**, including with the `img-src data:` crown sprites loaded.
  - **Work Plan commitment rules are unchanged:**
    - Approve felled nothing (living 171/73, 6 orders approved, editing locked).
    - Execute showed the exact-ID confirmation, felled exactly the 6 approved trees (166/72) and left the **year at 0**.
    - Year advance showed the unexecuted-work consent and the separate unsent-draft warning. Keep changed nothing; cancel-and-advance moved year 0 → 1 and cleared drafts.
  - **HTTP:** 28 × 200, 1 × 409 (intended), 1 not sent (intended drop), 0 × 5xx. Server stderr 0 bytes.
  - **Layout:** no sideways scroll at 1920×1080 or 1280×720; the stand is clear of the dock for both areas.
  - **Visual spot-checks of the iteration** (headless screenshots, not committed):
    - an edge reference (MA-A-153) and a centre reference (MA-A-134): both groups come out at the same 2× scale;
    - the edge group is wholly clear of the dock;
    - no tile overlaps; underlines sit on the foliage;
    - 0 page exceptions.
- **Clean stop:** PID 594927 got SIGTERM after its command line was verified, and stopped; `:46181` closed. No own server is left. The three protected listeners were alive afterwards with unchanged start times. Evidence is in `<evidence>/real-final-20261122/` (checks, HTTP log, empty stderr, redacted stdout). The capability occurs 0 times outside `private/`.

### 5.1 Revision 2 verification (unchanged record)

- **VERIFICATION TIER RUN:** Tier 1 (bounded real `/multi` runtime check). The existing 16:36 build was reused per custodian receipt `4231039c` and not recompiled.
- **VERIFICATION RUN:**
  - Build, 2026-11-22 16:36 (lane local time), after the 09:32 amendment: `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` from `Tools/MultiRackSitkaP1Lab` (SDK 8.0.134). **Build succeeded, 0 warnings, 0 errors**, exit 0, with `TreatWarningsAsErrors=true` from `Directory.Build.props`.
  - P0: the pinned binary was copied read-only from the Integration lane and all three pin checks passed (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`). Not rebuilt.
  - Runtime: `/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll <evidence>/fixture`. Own **PID 1791937**, OS-assigned **127.0.0.1:43373**.
  - Scratch stand data: a copy of `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, sha256 `bcaaa5c3…57c4`, identical to the committed file before and after the run.
  - Browser: already-installed Edge (headless) driven over CDP by `real.mjs`, plus `real-followup.mjs` in the same server session. Both scripts are archived in the evidence directory.
  - Stopped with SIGTERM after verifying the command line; the server logged "Session ended; nothing was saved."
- **VERIFICATION RESULTS — real data:** the main run passed 44 of 46 checks and the same-session follow-up passed 5 of 5, so 49 of 51 overall. The 2 non-passes are not code defects:
  1. **"Server records the release reason":** the test ticked the release box and then switched A→B→A before Add. The area switch reset the reason, which is inherited baseline behaviour (see limitations), so `Owner-selected felling` was sent. The follow-up repeated the check without an area switch, and it passed.
  2. **"Zero console errors":** the only entry was the browser's own network log, "Failed to load resource: 409", for the deliberately provoked refusal. There were no JavaScript exceptions, no `console.error`/`warn` calls and no CSP violations.

  | Area | Observed on real `/multi` |
  |---|---|
  | Data and areas | Two areas (MA-A 171 trees, MA-B 73); the table and map draw every tree; area buttons have 5 and 4 polygon points, matching the real outlines; `aria-pressed` follows the shown area |
  | Fit vs dock | Stand clear of the dock at 1920 for A and B, with the legend hidden, and at 1280×720 for A and B |
  | Scale and zoom | Scale bar 5 m at Fit and 2× → 2 m at 4×; + disabled at the 11th level; Fit restores |
  | Legend | Visible by default with 11 states; Hide survives B→A→B→A; Show restores it and focuses Hide |
  | Ticking | Reference MA-A-001 marked Crop through the real API; 4 competitors ticked; reference pinned; pending count unchanged; A→B→A keeps reference and 4-tree draft |
  | Sequential add | 4 `pending/add` requests sent one after another, all HTTP 200, plan-state revisions 1 → 2 → 3 → 4, in ranked order; server pending holds exactly those IDs; rows then show "Already pending Fell"; living counts unchanged |
  | Release reason (follow-up) | Crop reference, one competitor ticked, release ticked, Add → server records exactly `Competition release for retained Crop Tree MA-A-001 (operator intent)` |
  | Real refusal mid-sequence | 3 ticked. After the 1st add's 200, the test marked the 2nd tree Crop through the real API. The server refused the 2nd add with **409 PlayerPlanStateStale**; the 3rd was **never sent**. The report read "Stopped after 1 of 3 … Refused MA-A-018 … Not attempted MA-A-017 … nothing was rolled back". The refused tree, now Crop, was dropped from the draft with a visible reason; the 3rd stayed ticked |
  | Lost request | A browser-side interception dropped one add before sending. The page reported **unconfirmed**, locked commands (Advance and Approve disabled) and raised the alert. Re-read sent one GET, lifted the lock, and the dropped add was **never sent or resent** |
  | Fell this tree instead | MA-B-001 added once (HTTP 200) with `Owner-selected felling` |
  | Approve | 6 orders approved; living counts unchanged (171/73); no event; ticks disabled with the explanation "The plan is approved, so editing is locked…" |
  | Execute | Exact-ID confirmation ("Fells exactly 6 named trees across 2 areas in Year 0, or refuses all.") with focus on Back. After Confirm: year still 0, living 166/72 (−5/−1, exactly the approved IDs), one receipt |
  | Year advance | With one pending order and one unsent tick, the dialog showed both the unexecuted-work consent and the separate unsent-draft warning, focused on Keep. Keep left year, order and draft unchanged. Cancel-and-advance: year 0 → 1, `cancelledUnexecutedWork = true`, pending empty, drafts cleared |
  | Layout | Sideways overflow 0 px at 1920×1080 (twice) and 1280×720. At 1280×720 Add is visible and all 11 legend states are visible |
  | Server | stderr 0 bytes; 0 exception/fail lines; HTTP across the run 28× 200, 1× 409 (the intended refusal), 1 not sent (the intended drop); 0× 5xx |
  | Browser | 0 JavaScript exceptions, 0 `console.error`/`warn` calls, 0 CSP violations. One browser network-log entry, "Failed to load resource: 409", for the deliberately provoked refusal |

  Screenshots (not committed): `<evidence>/shots/real-01…12*.png`. Results: `real-checks.json`, `real-followup-checks.json`, `real-http-log.json`, `server.out.redacted.txt`, `RUN-NOTES.txt`.
- **MEASUREMENT INTEGRITY CHECK (Gate G3):** `tests/` untouched. Part J of `tests/ui-smoke-multiarea.sh` is pre-existing baseline drift (**D5**): it references `reason-choice` and `(not a Crop Tree)`, and baseline `a692e1bf` `PlayerMultiApi.cs` contains 0 occurrences of each. The smoke script was not run.
- **REMAINING VERIFICATION GATES STILL REQUIRED:** independent review by a separate non-author thread (this thread is the Primary author under D2), then an owner visual and playtest check.

## 6. Blockers, Gates & Decisions for the Manager

- **DEVIATIONS (as recorded by the custodian, `4231039c`):**
  - **D1** — `t3_worktree_handoff` timed out on the dirty primary checkout; the lane was created with WSL `git worktree add -b … <reserved path> a692e1bf…`. Accepted.
  - **D2** — no separate child thread; the Astra thread was re-bound to `claude-opus-5-5` by direct owner instruction. Accepted as an identity amendment; single writer preserved; not eligible as reviewer.
  - **D3** — the earlier revision of this file said "COMPILED: NO / not run". The build actually ran at 16:36, after the 09:32 amendment and within its authority. Corrected in this revision.
  - **D4** — the build ran without `--disable-build-servers` and without an external `TMPDIR`. Waived once by the custodian; no build-server process remains and no output escaped the lane. A repeat must use both.
    - Revision 3: the iteration rebuilds were again ordinary incremental builds. The final clean rebuild (§5.0) used both rules, `--no-restore` and `--no-incremental`, and produced a byte-identical binary.
  - **D5** — smoke Part J drift is pre-existing, as described in §5.
- **DEVIATIONS added by revision 3 (recorded by this worker; not custodian-reviewed):**
  - **D6 — source edits without a D-091 amended grant.**
    - Receipt `4231039c` allowed runtime plus the result file only, and forbade source edits.
    - The revision 3 source edits were made on the owner's direct, repeated in-session instructions (2026-11-22, this thread), item by item, and committed and pushed on the owner's explicit instruction ("go ahead, finish verification, commit, and push").
    - Manager comment `7087370178` (23:10Z) approved the three display fixes but said a custodian must issue one narrow amended source/build/commit grant first. No such grant was issued or requested by this worker before the edits.
    - Single writer was preserved (this thread, this lane only). The custodian should reconcile.
  - **D7 — one change outside the literal-only write path:** `PlayerApi.cs` line 31 (`img-src data:`), by owner choice to use the concept-board images. Details and the security reasoning are in §4. This is beyond the Manager's proposed writable path (literals plus result file). It needs Manager/custodian acceptance, or reversal together with a switch back to vector crowns.
  - **D8 — design-only delegates.** Three child agents (Astra, Sol 6.2 failed, Sol 6.1) were used for design proposals only. They wrote nothing to the repository. Their outputs are under `C:\Users\hp\.t3\outputs\multi885-ring-options\` and `…\multi885-spruce-options\`.
- **KNOWN LIMITATIONS / UNVERIFIED CLAIMS:**
  - **Crown closure is a model issue, routed to [#899](https://github.com/jermur788/CCF/issues/899) (owner decision: "the map stays as it is").**
    - The owner felled 10+ trees over 3 years and the ground never became visible.
    - Area A has 171 trees on about 915 m² (about 5.4 m² per tree), with sampled crown radii about 2.9 m (about 26 m² each). That is roughly 5× nominal summed crown footprint. This is an estimate from area figures and sampled radii, not measured fractional cover.
    - Crown radius follows the Unity potential-crown formula `0.9415 + 0.07635 × DBH(cm)`, scaled by competition (`AnnualRules.cs`, `CcfSourcePins.cs`, both unchanged).
    - There is no recruitment, so tree counts only fall, and released neighbours grow into gaps.
    - Felled trees show a 0.35 m stump, by design.
    - The ecology/model owner should decide on a growing-space limit or a lower synthetic density. Nothing in the map was changed to fake gaps.
  - **Tile placement (watch item):** when a rank tile's preferred corner is taken, it moves to the next free corner and can sit between two crowns (seen for rank 4 around MA-A-134). It is then less obvious which crown it labels. The dotted leader line and the matching row in the competitor list still identify the tree. Mapping between rank, tree ID and list is unchanged.
  - **Focus display is not to scale while a tree is selected:** the reference and its competitors are drawn at 2× radius and competitors are moved apart (with leader lines to their true positions). Data, competition figures and hit-testing are unchanged. Clicking a moved crown selects that tree.
  - **Crowded edges:** if there is no room, a held competitor can still overlap a neighbour at the map edge, because the group is never shrunk.
  - **Crown sprites' rights are owner-asserted only** (§4a).
  - **"Match Unity" (U01) is PARTIAL.** The owner's later navy, light-surround and better-trees direction supersedes it; no Unity side-by-side was made.
  - **1280×720:** with the real (long, upper-case) `MultiDisclosure`, the notice wraps to three lines, so only part of the first competitor row shows before the side pane scrolls. Add and all legend states remain visible.
  - **Reason resets on area switch:** switching areas resets the reason choice (note and release tick) while the competitor draft is kept. This is inherited from the baseline `showArea` → `resetReasonPicker()`. The reason preview shows the reason that will be sent before Add.
  - **Behaviour changes:**
    - The old single-competitor "inspect" button and "Return to reference" are removed in favour of the F1 checkboxes.
    - An empty map click keeps the selection and draft.
    - `MultiDisclosure` is also listed first in Details.
  - **Not exercised:**
    - a guard (4xx non-409) refusal during a real add sequence;
    - real narrow widths (640 / 390 px) — checked earlier only in the mock-API harness (overflow 0);
    - screen reader;
    - real touch devices;
    - performance on a low-end machine.
- **F1–F4 / U01–U09 status:**
  - Done: F1, F2, F3, F4.
  - U01: partial.
  - U02, U03, U04: done.
  - U05: done in the F1 form.
  - U06, U07: done.
  - U08: partial (as above).
  - U09: done.
  - U10, U11: deferred.
- **TRIGGERED REVIEW GATES:**
  - custodian reconciliation of D6/D7;
  - independent non-author review of the exact branch tip, including the `PlayerApi.cs` policy line and the embedded images;
  - owner visual and playtest acceptance.
- **GITHUB ADMIN UPDATES STILL NEEDED (by the Manager, not this worker):** an `active-tasks.md` row for this lane, branch and its state, and an #885 comment linking this handoff.
- **DECISIONS NEEDED:**
  - accept or reverse the `img-src data:` policy line (D7), and the D-032 rights position for the AI-generated crown images;
  - who performs the independent review;
  - whether to shorten the notice at compact heights;
  - whether the reason choice should survive area switches;
  - #899 crown model.
- **WORKER ASSESSMENT:** IMPLEMENTED ON TASK BRANCH | Ready for independent review: **Yes, once the custodian has reconciled D6/D7**
- **RECOMMENDED NEXT STEP:**
  1. The custodian reconciles D6 and D7 against this exact branch tip.
  2. Dispatch one independent reviewer (a different thread, not this author) using this handoff, the masking script and the evidence archive.
  3. The owner formally accepts on the real `/multi` page.
