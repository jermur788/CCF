# CCF #885 — Opus 5.5 implementation result: final navy forest UI on the real `/multi` page

**IMPLEMENTED: YES (source).  BROWSER-TESTED: YES, but only against a disposable mock API (not the served `/multi`).  COMPILED: NO.  SERVED `/multi` RUNTIME: NO.  REVIEWED: NO.  OWNER ACCEPTED: NO.  INTEGRATED: NO.**

## Identity, authority and baseline

| Item | Value |
|---|---|
| Actor | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code. Running in the Astra owner thread `1f1a1a27-d17e-445f-b302-9670616e7494` after the Astra (gpt-6-astra) session hit its usage limit; the owner then directly instructed "I want you to implement this design". No separate child thread was created. |
| Design implemented | https://github.com/jermur788/CCF/blob/docs/multi-885-astra-final-forest-ui-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-FINAL-DESIGN-AND-OPUS55-IMPLEMENTATION-PACKET-20261122.md (docs commit `305582e2c6edd08f2aece72ed85dd93e5bdb9640`) |
| Custody | D-091 pre-admission receipt, docs commit `ce3417fbe36e9ca4923594a8f4b2dc5a35d10166` on `docs/multi-885-astra-d091-admission-20261122-v1`. Source-only; build, restore, `dotnet` and runtime **not** admitted, and none was run. |
| Baseline | `a692e1bf5d21304d06e696db1f47d1988ff80ee1` (`feat/multi-885-owner-ux-20261122-v1`); `PlayerMultiApi.cs` blob `7a589996a6a56b1852b8fff2e8ad0d85e6256b3e`, 126 455 bytes — both verified before editing. |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-AstraOpusUI-20261122`, branch `feat/multi-885-astra-opus-ui-20261122-v1`, clean at exact baseline before the first write. |
| Candidate | the commit that adds this file (see `git log -1` on the branch; reported to the owner/Astra with its SHA). |

**Deviation — lane creation route.** The receipt's preferred route (`t3_worktree_handoff`) failed: the tool timed out running `git status` on the dirty primary checkout over the `\\wsl.localhost` path. The lane was then created with WSL `git worktree add -b feat/multi-885-astra-opus-ui-20261122-v1 <reserved path> a692e1bf…` at exactly the reserved path, branch and base, after checking that path and branch (local and remote) were absent. HEAD and a clean status were verified before any edit. No other lane, listener, process or the primary checkout was touched.

## Changed files

1. `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` — **only the interiors** of `MultiAreaPage.Html`, `MultiAreaPage.Css`, `MultiAreaPage.Js`. 1 456 insertions, 708 deletions; file now 175 003 bytes.
2. This result file.

No `wwwroot`, Core, tests, fixtures, goldens, Unity, P0, packages or other `src/Ui` bytes changed. No dependency, CDN, font or asset was added.

### Outside-literals byte identity (rerunnable)

`before` = baseline blob via `git show a692e1bf…:Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`; `after` = candidate working file.

```
before.masked.sha256 34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16
after.masked.sha256  34126c39b99ef97adf3d704e84ef8be2cd4cd65d51079ed13d4e8e97ed1d4c16
```

Delimiter lines are unchanged (`    internal const string Html|Css|Js = """` and closing `""";` at column 0). The new interiors contain no `"""`, no CR, no BOM. `git diff --check` passed.

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

Rerun: `git show a692e1bf5d21304d06e696db1f47d1988ff80ee1:Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | python3 mask_literals_v1.py -` and `python3 mask_literals_v1.py Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` on the candidate.

## What changed on the page

**Layout and palette.** Navy top bar with brand, Year, two-line *Advance one ecological year · Both areas · does not execute work* and Re-read. Three summary cards: Area A, Area B (living count, basal area, ha — all from `view.areas`) and the shared Work Plan (approved / pending / “No queued work”, plus a separate local draft count that never changes the pending count; **View plan →** opens the Work Plan tab). One compact amber notice: the disclosure, the server chip labels (full statements as tooltips), **Details** (opens the Details tab) and the session-only chip. The workspace is a grid: map | legend (232 px) | side panel (380 px) at 1920; 176 / 310 px, 8 px gaps and 16 px gutters from 1024–1599 px; stacked map → side panel → two-column legend below 1024 px (also 200 % zoom). Tabs Tree / Work Plan (count) / Trees / Results (count) / Details sit in the head row on every width.

**Map.** The SVG fills the pane and its viewBox always has the pane's aspect ratio. At Fit, the area box is fitted into the part of the pane **beside the navigation dock** (or below it when that gives a larger fit, e.g. narrow), so the stand is never under the dock. Zoom and pan move only that usable part and can bring any part of the stand out from under the dock. All eleven graded zoom levels are kept; +/− disable at the ends; pan buttons are disabled at Fit and at each clamp. Hit-testing still uses the browser's own `getScreenCTM()` inverse and the unchanged server `api/multi/hit`, so framing never changes which tree a click hits. Light blue-grey exterior with a 5 m grid (10 m when zoomed far out) in experiment X/Z metres; slate stand interior with a subtle procedural texture, clipped by the **real area polygon**; dashed boundary; passive north indicator and a **metric scale bar computed from the current zoom**; a status pill (reference and draft count, approved-standing count, or add progress).

**Navigation dock (F2, F3).** One grouped ~212 px dock: N/W/E/S compass (centre glyph decorative), separate + / − / Fit column, then two area buttons drawn from each area's real polygon, with `aria-pressed`, an “N in plan” note and a screen-reader summary. They call the existing `showArea`; the old header area switch and the overview inset are removed, so there is no duplicate switch. All dock targets measure 44 × 44 px or larger.

**Trees.** Six original procedural Sitka spruce variants (dark crown mass, four tiers of whorled boughs lit from the north-west, feathered needle sprays, pale leader), drawn once into shared symbols at unit radius. Each tree reuses one, scaled to its **server `radiusM`** and turned by its own ID seed. Below 11 px on-screen radius a “lite” symbol without needles is used. Strokes are merged per tier and colour (about 30 elements per symbol), and each tree's drawing is cached and rebuilt only when its server item, or its lite/full level, changes. Felled trees are stumps with an X; naturally dead trees are bare grey branches with D — distinct from executed stumps. The transparent true-radius hit disc is unchanged and decorations take no pointer events.

**One shared mark system (F4).** The same functions draw the map and the legend. Marks: Crop C (solid pale-blue ring), Reference R (white dashed ring and tag), Inspected (solid white halo, for a non-living selection), Competitor 1–5 (dotted ring and number), Fell choice (solid warm ring, tick, warm number), Pending P (amber dashed ring), Approved A (double blue ring, still standing), Felled X, Dead D, and the boundary. Ring widths and badge sizes are in screen pixels, so they read the same at every zoom. The legend is visible by default with all eleven states. **Hide legend** frees the column and widens the map; **Show legend** appears at the top right of the map. The choice is kept in memory across area switches (no web storage).

**Reference tree and competitors (F1).** Selecting any living tree (map, Trees table, exact-ID box, overlap list) makes it that area's **reference** and reads its competition immediately, without marking Crop (U04). The card shows the ID, the Crop chip if retained, *Sitka spruce · status*, DBH / Height / **Crown radius** / Age, *Reference stays pinned while choosing*, and the separate Crop options action. For a living non-Crop reference there is also a small “Fell this tree instead” action through the same fenced path. The five server-ranked competitors are native checkboxes showing rank, exact ID, DBH, CI share, strength, distance and CI term. Ticking changes only a local per-area draft; it never calls `chooseTree` or moves the reference.

A row is disabled, with a visible reason, when its tree is retained Crop, already pending, in the approved plan, felled, dead or the reference itself. All rows are disabled while the plan is approved, during a re-read lock or while an add sequence runs. Draft count, **Clear selection** and **Add selected N to Fell plan** stay visible in a sticky dock at the bottom of the side panel.

The reason line keeps `reasonToSend()` precedence: trimmed note, else the ticked release intent (offered only when the reference is a retained Crop), else exactly `Owner-selected felling`. Competition totals, the planned figure (queued or approved only, never drafts) and the rank/strength disclaimer are under *Current / planned competition details*.

**Sequential add semantics.**
- The ordered ID list is frozen at the moment Add is pressed. One operation-wide guard (`state.batch` plus `state.busy`) holds for the whole sequence; area or reference switches, map selection, plan edits, Crop, year advance, Re-read and other writes wait with an announcement. Pan, zoom and reading stay available.
- Each `api/multi/pending/add` carries the fence of the **latest confirmed view** — recomputed from every 200 response, never reused, never parallel.
- An intermediate view keeps the report the sequence started from on screen (rows show each tree's new status), so competition reads cannot reset the batch. Competition is re-read once afterwards.
- On a 409 the page shows the server view and stops, reporting the confirmed IDs, the refused ID with its code and message, and the IDs not attempted.
- On a guard refusal it stops and says the request did not reach the engine.
- On a transport failure or `OutcomeUnconfirmed` it labels that ID **unconfirmed**, stops and locks commands until a successful Re-read. Nothing is retried or resent automatically.
- Confirmed IDs leave the draft. Refused and not-attempted IDs stay if still eligible. Nothing is presented as all-or-nothing, and nothing is rolled back.

**Draft reconciliation.** After every view or competition read, draft IDs that became ineligible or left the top five are dropped, and the page says which and why. Choosing a different reference in an area clears that area's draft with an announcement. Each area keeps its own reference and draft across A→B→A. An empty map click now keeps the reference and draft (previously it cleared the selection) so a stray click cannot discard choices. Before a year advance, unsent drafts get their own warning and Stay / Discard choice, separate from the server's explicit consent to cancel unexecuted work. Drafts are cleared only after a successful advance.

**Work Plan.** Pending → Approved → Execute stepper. *Approve Fell plan · N orders* (fells zero, locks editing). Approved box *N approved · still standing — Execution has not happened. Year remains Y*. Warm-brown *Execute approved Fell · N orders* opens the unchanged frozen exact-ID confirmation with Back focused; the dialog now also says the year does not change. *Cancel approved plan* discards orders. Technical PlanId and execution key stay collapsed under *Technical reference* (U07). The receipt, Results and Details panes keep their server-only content.

**Unchanged behaviour preserved (by source; listed so a reviewer can check):** `call`, `currentFence`, `run`, the lock and re-read rules, the stale-response guards in `refreshCompetition`, the Execute confirmation freeze/withdraw logic, the consent flow and `consentToCancelWork`, the exact-ID inputs, the overlap list, focus restore by `data-focus-key`, `textContent`-only text, no `innerHTML`, no inline style or handlers, no web storage, no other origin.

## F1–F4 and U01–U09 mapping (implementation status, not acceptance)

| Item | Status | Note |
|---|---|---|
| F1 multi-select competitors, retained reference, safe sequential add | DONE (source; mock-tested) | Semantics above |
| F2 real area buttons in the dock | DONE | Real polygons, `aria-pressed`, existing `showArea` |
| F3 grouped compass + zoom on the map | DONE | 44 px targets; stand kept beside the dock at Fit |
| F4 default-visible illustrated legend with hide/show | DONE | Same renderer as the map; preference in memory only |
| U01 graphics | PARTIAL | Visibly new spruce, palette and marks per the owner's latest navy/light-surround direction. This **supersedes** U01's earlier “match the Unity charcoal/forest look”; no Unity-asset side-by-side was made |
| U02 on-map grouped controls | DONE | |
| U03 graded zoom | DONE (kept) | Eleven levels; usable-area centring |
| U04 click any living tree → competition | DONE (kept) | |
| U05 numbered competitors → Fell in one clear action | DONE in the F1 form | Tick 1–5 then one Add. The old per-competitor *inspect* button and “Return to reference” were removed (spec F1: row toggles do not call `chooseTree`) |
| U06 no forced reason typing | DONE (kept) | Default `Owner-selected felling`; optional note; truthful release intent |
| U07 machine IDs hidden | DONE (kept) | Collapsed *Technical reference* only |
| U08 compact desktop | PARTIAL | 1280×720: no page scroll, all legend states visible, Add visible; the reference card plus Add dock leave room for about two competitor rows before the pane scrolls |
| U09 state mapping | DONE (source) | Letter, number, tick or ring pattern per state; dead ≠ felled |
| U10 / U11 | NOT MODELLED / DEFERRED | Untouched |

## Checks actually run

| Check | Result |
|---|---|
| Baseline HEAD, clean lane, baseline blob and size | pass |
| `node --check` on the JS extracted from the spliced C# literal (Node v26.13.1) | pass |
| Each literal interior equals the reviewed page file byte-for-byte | pass |
| Masked SHA-256 before = after | pass (values above) |
| `git diff --check`; changed paths = `PlayerMultiApi.cs` + this file | pass |
| C# compile / `dotnet build` | **not run — not admitted** |
| Served `/multi` in a real session | **not run — not admitted** |
| P0 hash before/after | not checked (no P0 access admitted; P0 untouched by construction) |

**Browser harness (disposable, outside the repository, not part of the candidate).** The exact HTML, CSS and JS from the C# literals were served by a local Node server with a strict CSP (`default-src 'none'; script-src 'self'; style-src 'self'; …`) and a mock `fetch`. The mock returns synthetic data in the payload shape the page reads (`view`, `scenes`, `labels`, `competition`, `refusal`, `execution`, `annual`), checks fences and can inject a refusal or a network failure. Headless Microsoft Edge was driven over CDP. **52 / 52 checks passed** and there were no CSP violations or console errors. The checks covered:
- legend visible with 11 states, and a server hit-test map click;
- crop reference with five rows: tick 4 / untick / Clear / re-tick, with the reference pinned and the pending count unchanged by drafts;
- A→B→A keeping reference and draft;
- four adds sent strictly in sequence with plan-state revisions 1, 2, 3, 4;
- the refusal mid-sequence report, which stays visible in the viewport;
- network failure → unconfirmed + lock, then Re-read with **no resend**;
- Approve fells zero and locks editing; the Execute dialog focuses Back and fells only approved IDs, with the year unchanged;
- the year-advance draft warning with Stay keeping year and draft, then advance clearing drafts and moving to year+1;
- legend hide/show, focus, and the preference surviving an area switch;
- all eleven zoom levels, pan clamps and Fit;
- dock targets ≥ 44 px;
- no horizontal overflow at 1920, 1280, 640, 390 and 960 css px at 2× (200 % of 1920×1080);
- at 1280×720: stand beside the dock, Add visible and all legend states visible;
- narrow order map → side → legend.

A zoom-step redraw measured about 150–230 ms in headless software rendering (no GPU). Private evidence (not committed): `C:\Users\hp\.t3\outputs\multi885-opus-impl\harness\` (`shots/01…13*.png`, `run-final.json`, `driver.mjs`, `mock-api.js`).

**What the harness does not prove:** the real server's payloads, labels, refusal codes, CSP header and timing; C# compilation of the literal; behaviour with the real 171/73-tree geometry. The mock's data, labels and rules are invented.

## Remaining limitations and deviations

- **Not compiled and not run on the real server.** Needed next, under a build/runtime grant: compile, serve `/multi` in a fresh disposable session, and repeat the focused flow, including one real refusal.
- At 1280×720 only about two competitor rows are visible before the side pane scrolls (the Add dock stays visible).
- The old single-competitor inspect flow (“Fell candidate” tag, `frameInspection`) was removed in favour of F1. A non-Crop reference can still be felled itself via “Fell this tree instead”.
- An empty map click no longer clears the selection (deliberate; protects drafts).
- Header chips now render inline in the notice, and `MultiDisclosure` is also listed first in Details.

## Next step

Astra (or the owner-routed reviewer) reviews this exact candidate. Fixes, if any, go back to this same writer under valid custody. Approval is not owner acceptance or integration.
