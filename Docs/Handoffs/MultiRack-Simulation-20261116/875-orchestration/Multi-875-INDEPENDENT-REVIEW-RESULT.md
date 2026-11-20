# Multi #875 — independent review result: the real #874 selected 2D Stand Map UI and the repaired L03 smoke test

**Date:** 20 November 2026
**Task:** [#875](https://github.com/jermur788/CCF/issues/875) · **Reviewed task:** [#874](https://github.com/jermur788/CCF/issues/874) · **Manager:** [#863](https://github.com/jermur788/CCF/issues/863) · **Spec:** [#868 approved four-file spec](https://github.com/jermur788/CCF/blob/docs/multi-868-selected-ui-fourfile-draft-spec-20261120-v1/specs/multi-standmap-2d-selected-ui-v1/spec.md)
**Status:** independent reviewer result only. **Not integrated, not owner-accepted, not G2, not G4.** Visual fidelity to the selected reference image is **NOT VERIFIED** (no frozen reference asset exists).

---

## 1. Verdict

> **APPROVED — for the scope independently verified here: source review of the three frontend files and the one test file, plus the Tier-1/Tier-2 checks I executed myself on the exact candidate.**

There is **no blocking defect** in the reviewed candidate. The three changed UI files keep the authoritative C# engine as the only source of truth, keep every command fence, confirmation, replay and lock behaviour, and the repaired L03 is a genuine replacement for the old blanket ban rather than a weakening: I proved independently that the old check blocked the approved redesign, that the new check still fails on the old page, that it catches every regression the author claims, and that it does **not** suffer the width-loop defect the Manager asked about.

The remaining items in §6 are **non-blocking observations and guard-precision notes** for a later task; none of them is a defect in the shipped behaviour or a hole in what the tests currently claim to protect.

Separate gates, unchanged by this verdict: **runtime qualification by a human (owner G2)**, **literal Ctrl+zoom and screen-reader runs**, **frozen reference-image comparison**, and **integration (G4/main)** — all still open.

---

## 2. Reviewer identity, independence and lane

| Item | Value |
|---|---|
| Reviewer model | **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`) |
| Harness | OpenCode agent in T3 Code, WSL2 Ubuntu on `DESKTOP-DL1BQI7` |
| Independence | **Not** Claude Opus 5.5, not the Claude Code author thread, no author child/helper, no author transcript. Different provider and model family from the author. Session-independent rather than host-independent: same physical machine, disclosed deliberately. |
| Tools actually used | WSL2 bash 5.3.9, git, `gh` (actor `jermur788`), .NET SDK **8.0.134** (`~/.dotnet`), node **v26.13.1**, python **3.14.4**, Microsoft **Edge 157.0.4341.60** headless over the DevTools protocol (trusted `Input.*` events) |
| Review registration | [#875 comment 7044525961](https://github.com/jermur788/CCF/issues/875#issuecomment-7044525961) |
| Review lane | New detached worktree `/home/jer/Development/CCF-Review-875/wt-candidate` at exactly `03547c6d`; clean at start and after every run (`git status --porcelain --untracked-files=no` = 0 files). Build output, scratch mutation copies, evidence and screenshots all outside the repository. |
| Author lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-868-SelectedUI` was **not** used as a writable lane. |
| #867 owner-playtest server | Process 390578 was left running throughout and was still running at the end. My UI servers used ephemeral ports and their own scratch fixture copies; all were stopped. |
| Board state | `origin/main` `active-tasks.md` at `f852f6ca` contains no #868/#874/#875 row, and #875 had zero comments when I started. I did not edit the Manager-owned board file; my registration is the #875 comment plus this report. |

**Review-only:** no production code, test, fixture, specification, P0 reference or Unity asset was modified. The presented verdict rests on read paths and scratch copies.

---

## 3. Exact objects reviewed

| Item | Value |
|---|---|
| **Candidate under review** | **`03547c6d56b6ba05742e71714d0cc2a4431ecf78`** |
| UI source commit (unchanged by the repair) | `ccdb0b8af3ded00f2910a58296c5353884e75faa` |
| Base (#849 G2-qualified source) | `4c42e2012a4c1bee3a422e41a58535343d63ac9f` |
| Branch | `feat/multi-868-selected-standmap-ui-v1` |
| Docs-only HEAD (excluded from source review) | `e83fc9571143c7658cd9a4b95e5ab85f8e16df49` |
| Diff base→candidate | `index.html` +214/−119, `app.css` +194/−53, `app.js` +237/−54, `tests/ui-smoke.sh` +68/−9 (mode `100755`), plus the author's implementation handoff Markdown. **No** Core, `PlayerApi.cs`, `Program.cs`, csproj, fixture, P0, README, spec, Unity or `Docs/Project` change. |
| Verified unchanged by me | Fixture `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`; P0 four-way pin `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (csproj, DLL, `.sha256`, `provenance.txt` all equal, before and after my runs); P0 was not rebuilt. |

Artifacts read: the approved #868 spec, both #874 author handoffs (implementation and L03 repair), the Manager L03 grant packet's diff claim, and the four changed files in full.

---

## 4. What I executed myself (evidence tier 2)

Everything in this section is **my own execution on the exact candidate**, not the author's report.

| Gate | Command (from `Tools/MultiRackSitkaP1Lab`) | My result |
|---|---|---|
| Build | `~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj` and `.../tests/Core.Tests/SitkaP1.Core.Tests.csproj` | **0 warnings, 0 errors** |
| Core tests | `~/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **92/92 passed** |
| UI smoke at the exact candidate | `TMPDIR=<outside repo> NODE=<node> bash tests/ui-smoke.sh <evidence outside repo>` | **OVERALL: PASS (140 passed, 0 failed)**, HEAD printed as `03547c6d…` |
| L03 internals | same run, printed line | `media queries … ['(min-width: 1024px)', '(min-width: 600px)']; … page-layout queries other than (min-width: 1024px): []; (min-width: 1024px) applies at {'360': False, '640': False, '960': False}` |
| Static-checker mutation matrix | checker extracted verbatim from the committed heredoc, run against **scratch** copies of the served CSS/page | 15 cases, table in §5.2 |
| Guard-suite independence | the smoke run itself re-exercised G01–G23, V, R, A, H, P, E, Y, C, D, J01–J31, N01, Z01–Z03, W01–W05 | all pass in **my** run |
| Browser | Windows Edge 157.0.4341.60 headless, CDP `Input.dispatchMouseEvent`/`dispatchKeyEvent` (trusted), `Emulation.setDeviceMetricsOverride` | §7 |

### 4.1 Author claims vs my reproduction

| Author claim | My finding |
|---|---|
| Core **92/92** | **Reproduced**: 92/92. |
| UI smoke **140/140** on `03547c6d` | **Reproduced**: 140/140, same HEAD. |
| Original check would be **136/137** with L03 the only failure at `ccdb0b8a` | **Consistent**: I ran the **base** `independent` L03 logic against the candidate stylesheet and it fails, so the one pre-existing failure was real and the test change was necessary (§5.1). I did not re-run the whole base suite end-to-end. |
| Headless Edge **59/59**, trusted input | **Not reproduced as such** — the author's `drive.mjs` is local and not committed. My own CDP suite (different script) confirms the same facts it measured: map pane `654×531`, side panel `384×589`, header `107 px` at 1280×720 **exactly** as the author's B01–B03, no page scroll, one column at 960/640/360, dialog fit, trusted-event counts. See §7. |
| P0 4-way `9a99f781…`, fixture unchanged, nothing written | **Reproduced** by my own pin checks and by W01–W05 in my run. |
| Reference-image fidelity / literal Ctrl+zoom / screen reader / human play | Author marks them unverified. **I agree; still NOT VERIFIED** (§9). |

### 4.2 Reviewer-harness errors, disclosed

My first browser pass reported 6 "failures"; **all six were my own assertion errors**, each diagnosed and re-run with a corrected assertion, and **no product failure remained**:

1. I tested `#execute.hidden` instead of the enclosing `#approved-box` — corrected; the approved block is genuinely removed after Execute and the button is not rendered.
2. Four cases asserted "every control inside the *viewport*" in the stacked layout, where vertical page scroll is explicitly required by the spec — corrected to "inside the viewport *width*"; all pass.
3. I clicked the geometric centre of `P30-100`'s disc, which `P30-090`'s crown paints over; the page selected `P30-090` and the server's topmost candidate agreed — that is correct spatial behaviour (and a positive hit-test result, not a failure).
4. I attempted the "Other" refusal and a second Add on a tree that was already pending or felled, where the Add form is correctly hidden — corrected by using a living tree.

No author claim was accepted on the author's word; every number above is from my own run.

---

## 5. The repaired test file

### 5.1 Is the change legitimate, and is it a weakening?

* The **only** textual changes in `tests/ui-smoke.sh` are: the header comment, removal of the old `independent` mode, replacement of L03 by a `breakpoints` mode, and three added checks L06 (640 one column), L07 (640 reach) and L08 (1280 desktop structure). I diffed the whole file: every other label and assertion, including L01/L02/L04/L05 and all G/S/V/R/A/H/P/E/Y/C/D/J/N/Z/W checks, is byte-identical to `4c42e201`.
* The old L03 (`no @media rule touches the page layout`) **fails on the approved redesign by construction**: I ran the base checker against the candidate `app.css` → **FAIL**. So this was a genuine conflict with approved spec R-01, not a convenient edit.
* The new checks are **not vacuous**: run against the **base** page they fail exactly where they should — `breakpoints` **FAIL**, `desktop` **FAIL**, while `columns 360/640/960` and `reach 640` pass.
* Range of protection **increased**: the suite now pins a desktop structure (L08) and a third narrow width (L06/L07) that did not previously exist.

### 5.2 Is L03 effective? (independent mutation matrix)

I extracted the checker verbatim from the committed heredoc and ran it against scratch copies of the served files. `columns`/`reach`/`desktop` are the other L checks at the same width.

| Mutation (scratch copy) | L03 (`breakpoints`) | Other L checks | Same as author's table? |
|---|---|---|---|
| M1 `.layout` two columns at `(min-width: 900px)` | **FAIL** | `columns 960` FAIL | yes |
| M2 wrapper `body` grid at `(min-width: 700px)` (no page-part name) | **FAIL** | `columns 960` **pass** (only L03 sees it) | yes |
| M3 `* { float: left }` at 700px | **FAIL** | `columns 960` FAIL | yes |
| M4 `(max-width: 1023px) { .pane { display:none } }` | **FAIL** | — | yes |
| M5 `#side { display:flex }` at 800px (id, not class) | **FAIL** | — | yes |
| M6 benign `.filters` rule at 700px | **pass** (correctly not flagged) | pass | yes |
| M7 approved block moved to 900px | **FAIL** | `columns 960` FAIL, `desktop` FAIL | yes |
| M8 desktop rules made unconditional | **FAIL** | `columns 360/640` FAIL | yes |
| **M9 (mine)** `@media (min-width: 900px) { .map-body { grid-template-columns: 1fr 1fr } }` | **pass** | all pass | **gap — see F1** |
| **M10 (mine)** nested `@media` inside `@media` | **FAIL** (fail-closed) | — | good |
| **M11 (mine)** `:is(.layout, .side)` at 700px | **FAIL** | — | good |
| **M12 (mine)** `:where(main)` at 700px | **FAIL** | — | good |
| **M13 (mine)** same query written `(min-width:1024px)` (no space) | **FAIL** | — | brittle, fails closed — F2 |
| **M14 (mine)** legitimate `prefers-reduced-motion` query | **ERROR** "media query not understood" | `columns` errors too | pre-existing (#849) — F3 |
| **M15 (mine)** `.map-head` grid at 900px | **pass** | all pass | **gap — see F1** |

### 5.3 The Manager's width-loop concern — resolved, not a defect

`media_applies(query)` reads the module-level `width` parsed from `sys.argv[4]`, and the `for width in STACKED_WIDTHS:` loop is at module level, so it rebinds that same global before each call. I proved the rebinding is real rather than assuming it: on a **scratch copy** of the checker I redefined `DESKTOP_QUERY` to `(min-width: 700px)` and ran it against a 700 px stylesheet; the checker then printed `applies at {'360': False, '640': False, '960': True}` and failed as it should. So the loop genuinely evaluates three different widths. **No width-loop defect exists.**

Note also that the "applies at these widths" part is close to tautological for an exact `(min-width: 1024px)` string (1024 > 960 by arithmetic); the real force of L03 is the **exact-query equality** plus the element-containment rule, both of which I found working.

### 5.4 What L03 does not do

* It does not classify **descendants** of panels as layout-bearing (F1).
* It compares the query **string exactly**, so a cosmetic formatting change fails it (F2, fails closed).
* It inherits the #849 parser that refuses any non-width media query (F3, pre-existing).
* It is a **static reading** of stylesheet + page; it is not a layout engine and says so in its own output. Real-browser measurement remains separate, and I did that separately (§7).

---

## 6. Findings

No finding is blocking. Severity: **minor** = no current behavioural defect; **note** = observation for the owner/next task.

### F1 (minor — guard precision) A second breakpoint on a *descendant of a panel* escapes L03

`tests/ui-smoke.sh:289` `page_layout_selector()` treats a selector as page layout if it names `main`/`.layout`/`.side`/a panel, **or** if its last compound matches an element that **is or contains** one of those. Elements **inside** a panel (`.map-body`, `.map-head`, `.map-aside`, `.legend`, `.table-wrap`) are not classified, and no `columns` check looks at them.
Evidence: M9 and M15 above — both pass `breakpoints`, `columns 960`, `reach 960` and `desktop 1280`.
Impact: a future edit could introduce a second layout breakpoint inside the map body without any L check failing. It cannot reproduce the failure the old check protected (two-column *page*, clipping, horizontal page scroll): `.map-pane` is a scrolling pane and L01/L02/L06/L08 plus my browser measurements still pin the page column. The test's own label ("…or any element containing them") scopes it honestly, so this is precision, not a false claim.
Suggestion for a later task: classify descendants of `.map-panel`/`.side` as layout-bearing too, or assert that no media query outside 1024 changes `grid-template-columns` to more than one track on any element. **Not required before owner playtesting.**

### F2 (minor — brittleness, fails closed) Exact-text comparison of the breakpoint

`tests/ui-smoke.sh:272/302`: any non-identical spelling of the query fails L03, including `(min-width:1024px)` (M13). It fails safe, but a formatting-only edit will look like a layout regression.

### F3 (note — pre-existing, not caused by #874) The parser refuses non-width media queries

`tests/ui-smoke.sh:221` raises `FAIL: media query not understood` for anything except `min-width`/`max-width`. Adding a legitimate `@media (prefers-reduced-motion: reduce)` would abort **L01–L08** (M14). I confirmed the **#849 base** checker behaves identically, so this predates this candidate. Worth a follow-up when motion or print rules are next added.

### F4 (minor — wording) The species row counts felled trees as "trees"

`app.js:565` renders `Sitka spruce (100 trees)` from `state.scene.length`, while the management section immediately below shows `Living … (99 on map)` and `Felled … (1 on map)`. The numbers are the server's own statuses and are internally consistent, but a player can read "100 trees" as 100 standing. Suggest "100 trees in the scene" for the owner pass. Engine data is **not** misstated.

### F5 (note — UX, for owner G2) Execute/Cancel need a scroll inside the Plan pane at 1280×720

Measured at 1280×720 (Edge):

* selected + reason chosen: `Add` bottom **709 px** (inside the 720 px viewport), Plan pane 500 px tall over 1068 px of content;
* one pending order: `Approve` 644–710 px;
* approved plan: `Cancel` 997–1041 px and `Execute` 1051–1095 px, i.e. **below the pane's visible area**, reachable by scrolling the pane (page itself never scrolls: `scrollHeight` = 720).

This is permitted by R-01 ("bounded internal pane scroll … not a main document requiring scroll") and there is no page-length scroll, but the claim "core draft→approve→execute controls within one useful screen" is only partly true after approval. The owner should check this feels right in G2.

### F6 (note — copy) "or drag the map"

`app.js:695` says "Pan with the buttons, or scroll or drag the map." Mouse drag is not implemented (touch pan works natively through the scrolling pane). Cosmetic.

### F7 (note — cosmetic) Status chip colours

`app.css:188–190` gives explicit colours for Pending/Approved/Felled chips; `Living` and `NaturalDead` use the default chip border. Readable in both cases; no action.

### F8 (note — accessibility pattern) View buttons are toggle buttons, not a tablist

The four view buttons use `aria-pressed` + `aria-controls` rather than `role="tablist"`/`tab`. This is a valid, simpler toggle-button pattern; hidden panes are `display: none`, so they leave the accessibility tree. No action; noted so a later audit does not "fix" it into a tab widget without keyboard work.

---

## 7. UI, engine and accessibility: what I verified

### 7.1 Composition and layout (Edge, CDP, trusted input)

| Check | Result |
|---|---|
| 1280×720 | No page scroll at all (`scrollWidth×scrollHeight` = 1280×720); page grid **two tracks** `860px | 384px`; map pane **25,164,654,531**; side panel **884,119,384,589**; header **107 px** — identical to the author's B01–B03 |
| Every shown control ≥ 44×44 and inside the viewport at 1280 | pass |
| 1920×1080 | No page scroll; map grows to **1148×853** |
| 200 %-equivalent (960×540 @DPR2, 640×360 @DPR2) and 640 px, 360 px | No horizontal page scroll; **one** page column; all four panes stacked; view buttons hidden; every control inside the viewport **width** and ≥ 44×44 |
| Zoomed map | Zoom changes the scale note and never widens the page (`scrollWidth` 1280/1280) |
| Execute dialog at 640×360 | Fits the viewport, does not scroll internally; Escape closes it unsent and returns focus to Execute, with the plan still unexecuted on the server |
| Tab order | `skip → advance → reread → pan-north → pan-west → pan-east → pan-south → zoom-in → zoom-out → zoom-fit → view buttons`; Enter/Space operable |
| Console | No error, no uncaught exception, no CSP violation across all passes |

The viewport emulation is functionally equivalent to browser zoom for **layout and media queries** (zoom changes the CSS viewport). **Literal Ctrl+zoom was not operated** and is NOT VERIFIED; fractional-zoom rendering quality is not proven.

### 7.2 Genuine engine operations (browser + API, my runs)

* A trusted click on the drawn disc of `P30-063` selected exactly `P30-063` (server hit-test), and `insp-id` showed it.
* Clicking where `P30-090`'s crown paints above `P30-100`'s disc selected `P30-090` — browser topmost and server topmost agreed; the page did **not** silently pick the underlying tree.
* Keyboard ArrowDown on the reason list chose "Release competition around a retained tree"; the page previewed exactly that text and the **engine recorded exactly that text**; the list returned to the placeholder afterwards.
* Add at zero Fell: living stayed **100**, events **0**.
* Approve: engine minted a PlanId, living still **100**, events **0**, map showed an **A** badge and no P badge.
* Execute confirmation named the PlanId, the exact tree ID and its recorded reason, and started focus on **Back**. Confirming felled **exactly** `P30-063` in one typed Fell event; living **100 → 99**; a stump appeared; the approved block disappeared so Execute cannot be offered again.
* Advance ran a genuine Year+1 (annual revision 1).
* Refusals: placeholder Add, empty `Other`, whitespace-only `Other` all **sent nothing**, showed an inline message, set `aria-invalid` on the right field and moved focus to it; a wrong-case exact ID (`p30-001`) was refused with the selection unchanged; the custom `Other` text was **trimmed** by the engine (`wind-damaged leader, lean over the track`); switching away from `Other` cleared the hidden text box (no hidden default).
* Selection by exact ID and by the table's Select button both work; the table returns to the Plan pane and focuses the Selected tree heading; the table lists all 100 rows.
* Security guards in the same suite run: capability 404, Host/DNS-rebinding 403, Origin 403, 405/415/413/400, CSP/no-store/nosniff/no-referrer/DENY, byte-identical served files, stale-fence 409s, execution-key conflict, replay `replayed=true`, consent refusal and consented cancellation, loopback-only socket, SIGINT exit 0 with nothing saved, and nothing written (W01–W05).

### 7.3 Truthful map and legend

* The only pointer target is the server's own disc, still the tree group's **first child**; shadows, spokes, badges, rings and the ground all compute to `pointer-events: none` (checked in the live page).
* Felled trees are drawn at the engine's stump radius with an open canopy gap.
* Legend after one Fell: `Sitka spruce (100 trees)`, `Living … (99 on map)`, `Pending Fell (0)`, `Approved Fell (0)`, `Felled … (1 on map)`, plus a "Fell is the only management operation in this model" scope note and a separate "Map marks" section. Counts are tallies of the server's statuses. `Natural Dead` appears only when present.
* The no-save / session-only / no-money / comparison-unavailable disclosure and the four flags remain permanently visible; full provenance (fixture SHA, content hash, model versions, revisions, session) is in the Details pane with expandable full values.

### 7.4 Accessibility and security surface (measured)

* Sampled body text contrast **8.7:1 – 15.2:1** (`hint`, `note`, `subtitle`, `disclosure`, `step-note`, legend, rail terms/values, flags, field labels, table cells), primary button 7.95:1, danger button 9.53:1 — comfortably above 4.5:1. This is a **spot sample of computed colours**, not a full audit of every state.
* Visible focus ring 3 px; `role="alert"` region, `role="status" aria-live="polite"` region, `role="img"` map with title/description, `aria-label` on every icon control, labelled panes, table caption, `lang="en"`, skip link that switches to the Trees pane (verified in the browser).
* Page surface: **no** inline styles, **one** external script (`app.js`), **one** origin in `performance` entries, **no** web storage, no `innerHTML`/`insertAdjacentHTML`/`eval`, all text via `textContent`. CSP unchanged.
* The confirmation, lock and replay code paths are untouched by this candidate apart from presentation (verified in the `app.js` diff), and they passed J01–J31 in my own smoke run.

---

## 8. Does the UI still use the authoritative engine?

Yes, and the diff supports it:

* Every displayed value comes from `api/view` (`frame`, `stand`, `scene`, `source`, `annualHistory`, `events`) or from Core's `PlayerLabels`; the page's only local arithmetic is display rounding and the legend's status tally, which it states.
* The reason is sent as text through the existing `AddPending(fence, treeId, reason)` route; **no** new endpoint, enum, save field, species, economic value or client-side plan state was added.
* Approve/Execute/Advance still send the fence from the view they displayed; Execute sends the frozen confirmation's PlanId, minted key and fence; the lock still refuses every command and every hit-test until a successful re-read that began after the lock.
* "Sitka spruce" is a page constant, matching a fixture that is Sitka-only and the spec requirement that the species row must not invent Beech/Oak; it is not derived from a per-tree species field. This is honest for the current single-species fixture and would need revisiting before any multi-species fixture (see §11).

---

## 9. Not verified (explicit)

* **Reference-image fidelity:** the selected comparison images #1/#3 are AI-generated concepts, not frozen in GitHub. **No pixel or composition comparison is claimed.** The built page follows the text direction (dominant forest map, composed dark game-style hierarchy, two-part legend).
* **Literal Ctrl+zoom:** not operated. Equivalent CSS-viewport emulation only.
* **Human playtest / owner G2:** not performed. All input was trusted automation, not a person.
* **Screen reader:** no NVDA/Narrator run; semantics inspected in code and partly asserted in the browser.
* **Natural Dead** legend row and hatching: not exercised (no natural death occurred in my sessions either).
* **Contrast:** spot sample only, not a complete audit of every state and hover/disabled combination.
* **Screenshots:** mine are local to this reviewer machine (`/home/jer/Development/CCF-Review-875/evidence/`, `/mnt/c/Users/hp/AppData/Local/Temp/ccf875-review/out/`) and deliberately not committed; the capability URL is never published.
* **Full 136/137 reproduction at `ccdb0b8a`:** I proved the base check fails against the candidate CSS and ran the full suite at `03547c6d`, but did not run the entire old suite against the old UI in this review.

---

## 10. Boundaries respected

* Review only. No edit to `src/**`, `tests/**`, fixtures, specs, `Docs/Project`, Unity assets, P0, `main`, or the author's worktree.
* No rebuild of the protected P0 reference (`build-p0-core-ref.sh` not run); the four-way pin is equal before and after.
* The #867 owner-playtest server was not touched and was still running at the end.
* No merge to `main`, no G2/G4 claim, no release, no Ops/physical change, no capability URL in GitHub.
* Everything I executed ran on scratch copies outside the repository; the candidate worktree stayed clean.

---

## 11. Recommended next steps (Manager #863 / owner)

1. **Owner G2 playtest** on this exact candidate (`03547c6d`) at 1280×720 and 1920×1080, including the observations in F5 (Execute/Cancel need a pane scroll after approval) and F4 (species count reads as total trees).
2. **Freeze the reference image**: store the owner-selected comparison image #1 (plus image #3's legend direction) at a permanent path with SHA-256 and `generated-concept` provenance, then run a separate visual-fidelity comparison. Until then, visual fidelity stays NOT VERIFIED.
3. Optional, low-cost follow-ups for a later narrow task (none blocks playtesting): F1 (extend the layout-selector rule to panel descendants), F2/F3 (make the breakpoint comparison formatting-tolerant and let the parser tolerate non-width queries by ignoring them rather than aborting), F4/F6 copy tweaks, and a real screen-reader plus literal Ctrl+zoom pass.
4. Multi-species fixtures would need a real per-tree species value before the legend's species section can be more than a constant.

---

## 12. Reproduce

```bash
# clean read-only lane at the exact candidate
git worktree add --detach /tmp/ccf875-review 03547c6d56b6ba05742e71714d0cc2a4431ecf78
cd /tmp/ccf875-review/Tools/MultiRackSitkaP1Lab
cp <authorised pinned .p0ref>/* .p0ref/ && chmod a-w .p0ref/*      # never run build-p0-core-ref.sh
# 4-way pin: csproj, DLL, .sha256, provenance.txt = 9a99f781… ; stop on any mismatch
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj
~/.dotnet/dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj
~/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll        # 92/92
TMPDIR=/tmp/out NODE=$(command -v node) bash tests/ui-smoke.sh /tmp/evidence      # 140/140
```

For the static checker alone, extract the heredoc between `<< 'PY'` and `PY` from `tests/ui-smoke.sh` into `layout_check.py` and run:

```bash
python3 layout_check.py <app.css copy> <index.html copy> breakpoints 0
python3 layout_check.py <app.css copy> <index.html copy> desktop 1280
python3 layout_check.py <app.css copy> <index.html copy> columns 360
```

Mutate the **copy** (never the tracked file) to reproduce §5.2.

Browser witness: Windows Edge `--headless=new --remote-debugging-port=9223` with an isolated profile; drive `Input.dispatchMouseEvent`/`dispatchKeyEvent` and `Emulation.setDeviceMetricsOverride`; the capability URL is taken from the server's printed line and never published.

---

## 13. Reviewer evidence on the host (not published)

| Path | Contents |
|---|---|
| `/home/jer/Development/CCF-Review-875/evidence/smoke.out` | full 140-check smoke log at `03547c6d` |
| `/home/jer/Development/CCF-Review-875/evidence/smoke/` | smoke evidence copy |
| `/home/jer/Development/CCF-Review-875/scratch/mut/` | 15 scratch CSS/page mutations |
| `/home/jer/Development/CCF-Review-875/scratch/layout_check.py` | checker extracted verbatim from the committed test |
| `/mnt/c/Users/hp/AppData/Local/Temp/ccf875-review/out/review-drive*.log` | browser pass logs (CDP) |
| `/mnt/c/Users/hp/AppData/Local/Temp/ccf875-review/out/*.png` | local screenshots |

---

**Verdict restated:** **APPROVED** for the independently verified source and Tier-1/Tier-2 scope of candidate `03547c6d56b6ba05742e71714d0cc2a4431ecf78`. **Runtime qualification by a human, owner G2 acceptance, reference-image visual acceptance and integration remain separate and open, and nothing here marks the work integrated or owner-accepted.**
