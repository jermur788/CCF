# Multi #885 ? one-shot two-area browser demo result

**DEMO INCOMPLETE ? gameplay sequence OBSERVED; complete legible visual action-sequence evidence NOT captured.**

Worker: fresh separate Codex GPT-6.1 Sol, implementation/demo actor, medium effort, T3 Code. User explicitly authorised this separate worker. Run completed 2026-11-21T23:59:19Z. No source implementation, integration or acceptance performed.

## Authority and exact runtime

- [Physical admission receipt](https://github.com/jermur788/CCF/blob/docs/multi-885-d091-demo-admission-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-DEMO-RUNTIME-ADMISSION-RECEIPT-20261121.md).
- [Manager task](https://github.com/jermur788/CCF/blob/docs/multi-885-owner-playable-demo-priority-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-OWNER-PRIORITY-PLAYABLE-2D-DEMONSTRATION-NEXT-TASK-20261121.md).
- Detached demo worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Demo-Playable-20261121`, exact source HEAD **4cecf87339bea4d32907955a8d5625d1f27f96bd**, created at the receipt's fresh path. No source commit in this lane.
- Actual server PID **1641491**, endpoint **127.0.0.1:37729**, actual in-memory multi-area session **b41e79d4aa08ac5b74ee13ad65437849**. Capability intentionally omitted; no private access URL is published.
- Host DESKTOP-DL1BQI7, WSL2 Ubuntu, jer uid1000, 879 GiB free before allocation. Protected #877 PID1750123/33767 and #882 PID1806361/43649 alive before and after; #867 PID390578 absent and untouched.
- Live GitHub coordination fetched at start and end; final blob `9124d7854d071d2b612de91608ed64d35c1773b7`. No competing demo registration/path existed. Custodian receipt provided the exact lane allocation; worker did not edit the single-writer coordination board. Dashboard ingestion/published synchronization NOT VERIFIED and remains Manager follow-up.
- Read AgentWorkflow, Decision Log, current milestone, game brief and implementation overview. Task packet/candidate is task-level context; no separate context SHA was invented.

## Build and integrity

Copied only the three admitted `.p0ref` files from the integration lane, set copies mode444. Four-way project pin/DLL/sidecar/provenance agreement: **9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3**. P0 was never rebuilt.

One build: `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers`, from the demo Lab, SDK **8.0.134**, external TMPDIR in the evidence directory. PASS, 0 warnings/0 errors, 18.72 seconds. No test suite or Q1 execution. Runtime used `dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived` with that same pinned dotnet executable.

Before/after source `git status --porcelain` empty; eight protected source/fixture SHA256 checks unchanged and exact candidate bytes verified. Full tracked status also clean. Integration `4cecf873`, author `ac4c7b51`, reviewer `58950a9a` remained clean at their original HEADs. Their build outputs were not used or changed. Primary WSL Git was clean; Windows Git initially reported apparent asset changes, so no Windows Git write was performed and user work was preserved.

## Real player actions and observed outcomes

Used the actual browser page and existing UI controls, not injected UI states or fabricated API responses. Area A starts with 171 trees in 0.0915 ha; Area B with 73 in 0.15 ha. These authored areas differ in age, spacing and initial stocking and are **not comparable treatment/control plots**. Area B was deliberately left entirely unmanaged.

1. Inspected Area A, selected **MA-A-003** by exact ID and explicitly marked it retained Crop. Its initial DBH29.00cm, height18.70m; current Hegyi CI **3.5121**, 21 living neighbours within8m.
2. Chose listed competitor **MA-A-004** (22.1cm DBH, 1.68m away, CI contribution0.4532). Recorded reason **Competition release for retained Crop Tree MA-A-003 (operator intent)**. Pending plan contained exactly that single Fell in A and none in B; no tree felled.
3. **Approve:** plan-state revision2?3, work revision0, year0. Raw API comparison established both complete tree arrays identical to baseline, zero Felled.
4. **Execute:** reviewed existing confirmation and clicked Confirm felling. Exactly MA-A-004 became `fell / manual-work-plan` at year0, work revision0?1, plan-state revision4. A living171?170; A BA74.021606?73.602375m?/ha. B complete area object unchanged, living73, BA39.503594. Every other tree record unchanged. No year advance.
5. Switched to Area B and inspected **MA-B-001**: DBH35.2cm, height27.34m, age47; CI0.7330 and five local neighbours. No Crop/Fell action in B.
6. Clicked the actual annual-advance control **three times**, each followed by raw API snapshot. Genuine annual histories and natural-death IDs were returned by the unmodified engine. Executed-work history retained the single year0 Fell, with none in B.

| State | A living | A BA m?/ha | B living | B BA m?/ha | Natural deaths A/B in that step |
|---|---:|---:|---:|---:|---|
| Year0 baseline / approved |171|74.021606|73|39.503594|not advanced|
| Year0 executed |170|73.602375|73|39.503594|not advanced|
| Year1 |165|74.126462|73|41.150927|5 / 0|
| Year2 |155|72.837222|73|42.821524|10 / 0|
| Year3 |151|73.866018|73|44.514667|4 / 0|

At year3 A relative density0.803222, Dq23.872586cm, 1650.273 stems/ha; B relative density0.495075, Dq34.126428cm, 486.667 stems/ha. Crop MA-A-003 remains living and designated, DBH30.703892cm, height20.726187m. Fenced read-only competition API at year3 returned CI2.7723303 and19 neighbours; no pending removal. This later change combines selected removal, subsequent growth and natural mortality; it does not isolate a treatment effect or demonstrate ecological benefit. Different A/B annual results do not prove comparability. No cross-area treatment transfer was observed during execution; annual independence is the model's stated scope, not an empirical validation here.

Source/session fences were server-issued: session identity stable, model/source token retained, ecological/annual revisions advanced0?3, work0?1, plan-state0?2?3?4. No stale-token negative test was added; this run does not claim exhaustive fence qualification.

## Evidence and observed limitations

External archive: **`/home/jer/Development/CCF-885-Demo-Evidence-20261121`**. No evidence binary added to Git.

- `01-baseline.json`, `02-pending.json`, `03-approved.json`, `04-executed.json`, `05-year1.json`, `06-year2.json`, `07-year3.json`: raw GET snapshots.
- `08-crop-competition-year3.json`: genuine fenced read-only POST response.
- `01-baseline.png` and `02-browser.png` through `12-browser.png`: actual sequential browser captures, no synthetic redraw. 02 Crop,03 pending,04 approved,05 executed,06 untreated-B inspection,07 responsive view,08 year1,09 year2,10 focused year2 history,11 focused year3 history,12 focused receipt.
- `11-browser.png` clearly shows complete three-year results, death IDs and execution history; `12-browser.png` shows the original exact per-area receipt. These are legible, inspected captures. Early wide screenshots were unexpectedly clipped to1016px despite the browser reporting1280 or1920 CSS px, so the right-side Crop/Work Plan controls and parts of the action sequence were not fully captured. The later1000px responsive layout and scrolling to actual receipt/history produced readable evidence.
- **This fails the receipt's full legible 4?6-frame action-sequence evidence requirement.** The original pending/approved states cannot be recaptured after execution without another run, which this one-shot worker did not start. Raw API evidence and actual UI text substantiate those observed states but do not repair that visual acceptance gap. Hence DEMO INCOMPLETE rather than full demonstration completion.
- No gameplay defect or contradictory Work Plan semantics observed in this bounded sequence. One attempt to click a desktop tab after switching to1000px timed out because responsive mode stacks the panes; scrolling to their headings worked. This is an automation/viewport limitation, not an established product defect.
- `build.log`, protected before/after hashes, source status files, `listeners-after.txt`, and `evidence.sha256` provide provenance. Private runtime/access files are mode600 under a mode700 evidence root and are excluded from publication and the public evidence inventory; they must never be pasted/uploaded. The handoff contains no bearer capability.
- No owner physical-input/browser playtest, full viewport/accessibility matrix, N1 naturally-empty, L4 or protected DLL reproducibility verification performed. No new tooling installed.

## Cleanup and owner next step

**READY FOR OWNER BROWSER TEST ? no live session reserved or running.** Worker sent SIGTERM only to its own verified PID1641491; listener37729 disappeared. #877/#882 remained alive on their original ports. Browser tab owned by this worker closed. Demo detached lane and evidence retained cleanly for custody/review; no deletion/cleanup of foreign lanes.

Owner access needs a **new specific custodian reservation** and a fresh private capability generated on that next launch; the expired run URL must not be reused or published. No still-running listener was left for the owner. No approval for a second run is inferred from this report.

One-minute owner script after separately reserved launch: open the private exact127.0.0.1 two-area URL; inspect A/B; select MA-A-003 and Retain as Crop; choose listed MA-A-004 and the recorded competition-release intention; add Fell; read Work Plan; Approve and check both counts unchanged; Execute/Confirm and check only A loses MA-A-004 while year stays0; advance one year and inspect per-area results/history. Retake the full action sequence with verified readable captures before treating the demonstration evidence complete. Owner must personally judge usability and G2 acceptance.

## Boundaries / handoff assessment

No source/fixture/test/Unity/serialized/package/save changes, P0 regeneration, tracing, Q1, year50, Windows build, source merge or unrelated investigation. This publication adds one documentation-only Markdown on its own GitHub branch; the demo lane has no commit or source edits.

**Stage I remains NEEDS CHANGES; Q1 BLOCKED; N1 naturally-empty NOT OBSERVED; L4 and P0 reproducibility remain OPEN/DEFERRED; owner G2 NOT CLAIMED.** No qualification, calibration, empirical comparability, Unity verification or acceptance claim follows from this demonstration.

Recommended Manager next action: assess the raw observed gameplay, reconcile the documentation receipt/coordination/dashboard, and arrange a separately reserved owner browser trial with readable visual capture. Do not mark the visual-evidence requirement passed from this handoff.
