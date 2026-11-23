# MULTI Stage 1–4 `/multi` UI catch-up — Codex continuation handoff

| Field | Value |
|---|---|
| Date | 2026-11-23 |
| Purpose | Bring a Codex worker/manager up to date on work completed by a different route, and say exactly where every artifact lives |
| Task ID | `multi-stage4-ui-catchup-20261123` |
| Delivery lane | 2D `/multi` simulator, `Tools/MultiRackSitkaP1Lab` (no Unity, no `Assets/`) |
| Candidate branch | `feat/multi-stage4-ui-catchup-20261123-v1` |
| Reviewed candidate HEAD | `e7c1a57da519ed1ee03ccfbad240196a66df0905` |
| Source/test HEAD | `541bb898` |
| Base | `500a50ae` (Stage 4, APPROVED focused G3) |
| D-091 admission | `d3a9f20f` |
| Review verdict | **APPROVED (G2)** — suitable for owner playtest; **main integration UNAUTHORISED** |
| This handoff lives on | `docs/multi-900-codex-continuation-20261123-v1`, branched from `e7c1a57d` (the candidate head is NOT moved by this document) |
| Routing ledger | `/home/jer/.local/share/ccf-orchestration/routing-ledger.jsonl` (portable fallback; decisions `d-900-ui-catchup-impl-1`, `d-900-ui-catchup-review-1`, `d-900-ui-catchup-review-2`, `d-900-ui-catchup-repair-1`; attempts `att-20261123-01…05`) |

**Read first, in this order:** `specs/multi-stage4-ui-catchup-v1/spec.md` → the implementation handoff §1–§11 → the final review addendum → this file.

---

## 1. What happened, in one paragraph

The product owner asked for the 2D `/multi` simulator to catch up to the already-built Stages 1–4 engine: default the session to model 1/1/1, expose the authoritative Stage 4 read-outs (life state, mortality deadwood, canopy summary, 2.5 m light grid), wire a fenced Prune work-plan flow, and extend the #885 UI with rack summary readouts, a prune inspector, an optional Light Guide + ground inspector, and fallen-deadwood markers. One Claude Opus 5.5 writer implemented it in a single D-091 lane; GPT-6.1 Sol reviewed it twice, first **NEEDS CHANGES (G2)** on the screenshot acceptance criterion only, then **APPROVED (G2)** after the owner-chosen repair. Nothing was merged to `main`.

## 2. State you can verify yourself

```bash
cd /home/jer/Development/CCF-HP
git fetch origin
git ls-remote origin \
  refs/heads/feat/multi-stage4-ui-catchup-20261123-v1 \
  refs/heads/docs/multi-stage4-ui-catchup-review-20261123-v1 \
  refs/heads/docs/multi-stage4-ui-catchup-review-2-20261123-v1 \
  refs/heads/docs/multi-900-codex-continuation-20261123-v1 \
  refs/heads/main
```

| Ref | Expected |
|---|---|
| `feat/multi-stage4-ui-catchup-20261123-v1` | `e7c1a57d` |
| `docs/multi-stage4-ui-catchup-review-20261123-v1` | `42885ebf` (first review, NEEDS CHANGES) |
| `docs/multi-stage4-ui-catchup-review-2-20261123-v1` | `d82f34e8` (final addendum, APPROVED) |
| `docs/multi-900-codex-continuation-20261123-v1` | this file's commit |
| `main` | `3fb3437a` (coordination entry only) |

**Warning:** the local `main` checkout at `/home/jer/Development/CCF-HP` is **1864 commits ahead / 50 behind** `origin/main`. It is not the live board and must not be treated as authoritative. Fetch `origin/main`; do not reset, merge or "fix" the local `main` without an explicit owner instruction.

## 3. Authority and what is already settled — do not re-litigate

1. Owner packet of 2026-11-23 (supplied in-session, **never published as a committed file** — recorded as clarification F-6). Its substance is restated in the spec quartet and in §4 below.
2. `specs/multi-stage4-ui-catchup-v1/{spec,plan,tasks}.md` + `checklists/requirements.md`, **Status: APPROVED**, committed at `18518b97` **before** any source edit.
3. Owner decisions confirmed 2026-11-23 and recorded in `spec.md` "Open product decisions":
   - Prune lift heights are the engine's Unity Sitka schedule **2.5 m / 5 m / 6.5 m**.
   - A natural death keeps **both** the standing-dead crown with its `D` badge **and** the fallen-log marker (Unity shows only the log).
   - AC-9 was corrected by the owner from the orchestrator's 400 KB/3 MB cap to **≤ 0.9 MB per image, ≤ 5 MB per set, both resolutions committed**. That correction is an owner reconciliation of an orchestration cap, not a weakened product criterion.
4. Frozen and protected: Model 0 `0/0/0`, `Legacy40`, `Enlarged80`, `Reference Future v1`, `#886 Compact30`, saves, Unity `Assets/**`, packages, `main`.

## 4. Commits on the candidate branch (`500a50ae..e7c1a57d`)

| Commit | What it does |
|---|---|
| `d3a9f20f` | D-091 lane admission (custodian: DeepSeek V4.1 Flash; not a source commit) |
| `18518b97` | APPROVED spec quartet |
| `68d566cc` | Core UI read-out, scheduled Prune, 1/1/1 `/multi` default, atomic-refusal test |
| `f9f54f1d` | `/multi` payload carries the one-root Stage 4 read-out; fenced Prune routes |
| `f23ffb59` | Page: Stage 4 read-out, Prune flow, Light Guide, fallen deadwood |
| `527a4c02` | Smoke harness `cloneNode` fix (J01 root cause) + disclosure assertion follows 1/1/1 |
| `541bb898` | Browser-review fixes (band numbers at 1280, ground inspector in view, prune-only status) |
| `4d7267d0` | Implementation handoff, 1280×720 evidence, committed browser driver |
| `7dabed92` | Both screenshot resolutions committed; AC-9 reconciled with the owner |
| `e7c1a57d` | File-mode cleanup on the new PNGs (content unchanged) |

## 5. Where the code is (all under `Tools/MultiRackSitkaP1Lab/`)

| File | Role |
|---|---|
| `src/Core/MultiArea/MultiAreaUiReadout.cs` | **New.** One-root read-out: per-tree life state/death year/death cause/pruning lifts/eligibility + engine reason; per-rack cover, open ground, largest opening, RD, deaths, deadwood records with coordinates; 2.5 m ground cells with clipped shape, light index, covered fraction, contributing trees |
| `src/Core/MultiArea/MultiAreaContinuation.cs` | Model selection, Stage 4 root state, AddPrune/RemovePrune/Approve/Execute/Advance, `ReadStage4View()` |
| `src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs` | Adds `AddPrune`, `RemovePrune`, `ReadSnapshot`, `ReadoutFor`; `OpenSynthetic()` now opens **1/1/1** |
| `src/Core/CanopyLightEvaluator.cs` | Three internal per-cell helpers; existing output unchanged |
| `src/Core/Stage4Records.cs` | Unity Sitka lift heights 2.5 / 5 / 6.5 m |
| `src/Core/MultiArea/MultiAreaContracts.cs` | Additive Stage 4 labels (manifest hash unchanged) |
| `src/Ui/PlayerMultiApi.cs` | **The whole UI lives here.** `AddMultiAreaRoutes()` ~line 56; additive `stage4` payload; `api/multi/prune/add` and `prune/remove`; `internal static class MultiAreaPage` ~line 216 holds the HTML/CSS/JS as raw string literals (crown sprites are inline `data:image/webp`). There is no `wwwroot` page for `/multi` |
| `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs` | **Untouched.** A change here is a stop condition |
| `tests/Core.Tests/Stage4UiCatchupTests.cs` | **New.** U-01…U-04, including the execute-time atomic-refusal test (white-box: it injects a guard change into private state because public commands cannot produce that state) |
| `tests/Core.Tests/MultiAreaTests.cs`, `Program.cs` | Adapter member list and test registration |
| `tests/ui-smoke-multiarea.sh` | `cloneNode` stand-in + 1/1/1 disclosure assertion |

## 6. Where the documents and evidence are

All paths below are repo-relative; the `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/` prefix is abbreviated **H/**.

| Artifact | Location | Commit |
|---|---|---|
| D-091 admission | `H/MULTI-STAGE4-UI-CATCHUP-D091-LANE-ADMISSION-20261123.md` | `d3a9f20f` |
| Spec quartet | `specs/multi-stage4-ui-catchup-v1/{spec,plan,tasks}.md`, `checklists/requirements.md` | `18518b97`, AC-9 amended at `7dabed92` |
| Implementation handoff | `H/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md` | `4d7267d0`, updated `7dabed92` |
| First review (NEEDS CHANGES) | `H/MULTI-STAGE4-UI-CATCHUP-INDEPENDENT-REVIEW-20261123.md` | `42885ebf` on the review-1 branch |
| Final review addendum (APPROVED) | `H/MULTI-STAGE4-UI-CATCHUP-REVIEW-ADDENDUM-AC9-20261123.md` | `d82f34e8` on the review-2 branch |
| Screenshots (10 PNGs, 4,595,231 B) | `H/evidence/` — five states × 1280×720 and 1920×1080 | `4d7267d0`, `7dabed92` |
| Browser driver + raw results | `H/browser-driver/{cdp.mjs,scenario.mjs,browser-results-20261123.json,browser-run-20261123.txt}` | `4d7267d0` |
| Coordination entry | `Docs/Project/coordination/active-tasks.md` (top entry) | `3fb3437a` on `main` |
| External copy of the 1920 set (superseded) | `/home/jer/.local/share/ccf-evidence/900-ui-catchup-1920-20261123/` | n/a |

Working checkouts on this machine (do not assume they are free):

- Implementer lane: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123` (branch above, clean, no active writer)
- Review lane 1: `…/CCF-Multi-Stage4UiCatchup-Review-20261123`
- Review lane 2: `…/CCF-Multi-Stage4UiCatchup-Review2-20261123`
- This handoff's lane: `…/CCF-Multi-900-CodexHandoff-20261123` (removed after push; branch remains)

## 7. Verification: what was actually observed, and by whom

| Check | Value | Who |
|---|---|---|
| Solution + Ui builds | 0 warnings, 0 errors | Coordinator re-ran |
| Core suite | **184/184**, 0 failed | Coordinator re-ran; reviewer re-ran 76 relevant cases independently |
| Frozen anchors | `7e3a2a86…`, `a692a6d5…`, `814e8165…`, `8a926776…`, P0 `9a99f781…` unchanged before/after | Coordinator re-ran |
| Scope | 26 changed files, all inside the grant; `PlayerApi.cs`, `Program.cs`, `Assets/`, `.sln`, fixtures untouched | Coordinator checked |
| Real headless Edge 73/73 | Worker evidence in `browser-results` + PNGs; reviewer inspected, coordinator did **not** re-run | Worker + reviewer |
| `ui-smoke-multiarea.sh` | 110 pass / 6 fail (J04, J05, J09, J10, J11, J99) — pre-existing #885 selector drift exposed by the J01 fix; triaged, not fixed | Worker + reviewer |
| `ui-smoke.sh` (single-area) | 250 pass / 4 fail; the files it reads are identical to base | Worker claim |
| **Not run / not claimed** | Unity, import, save-load, Windows build, `p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`, owner playtest acceptance, integration to `main` | — |

## 8. Open findings (all nonblocking, do not merge-block)

**F-2 MINOR — unavailable read-out is treated as legacy/empty.**
Evidence: `src/Ui/PlayerMultiApi.cs:117-126, 1230-1232, 1550-1555, 1625, 2993-3005, 3294-3309`.
When the engine root moved, the API returns `stage4:null`; the page then falls back to model-0 wording ("edge neighbours excluded", "Light … NOT MODELLED"), `approvedPrunes` returns `[]`, and a prune-only approved view can offer an empty "Confirm felling" dialog until re-read. The reviewer's source analysis says this **cannot** cause unseen ecological execution, because the same response carries a stale fence and Execute refuses. Suggested correction: represent "unavailable" separately from "legacy", keep model provenance honest, and withdraw Execute until a paired snapshot is re-read.

**F-3 MINOR — light-band text halo scales with map metres.**
Evidence: `src/Ui/PlayerMultiApi.cs:937, 2175-2184`. The digit stroke has no `non-scaling-stroke`, so at 1920 the halos become large disks over the colour ramp; at 1280 the digits are tiny. Numbers remain present and correct. Presentation only.

**F-7 MINOR — stale summary wording after the AC-9 repair.**
`specs/multi-stage4-ui-catchup-v1/checklists/requirements.md:63`; `specs/multi-stage4-ui-catchup-v1/tasks.md:92`; implementation handoff lines 214, 236, 237, 242. Documentation upkeep only.

**Carried observations:** F-4 the Stage 4 anchor is printed by the test rather than hard-pinned; F-5 intentional-409 console-log sequencing; F-6 the original owner packet was never published as a committed artifact.

## 9. Environment and commands

```bash
# Toolchain — the system dotnet is 10.x and must NOT be used
/home/jer/.dotnet/dotnet --version        # expect 8.0.134
mkdir -p /tmp/codex900/tmp && export TMPDIR=/tmp/codex900/tmp   # keep TMPDIR outside the repo

cd <lane>/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll         # expect 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers

# Playtest server: OS-assigned loopback port, prints a fresh capability URL
/home/jer/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
```

- `.p0ref/` is **git-ignored** and is not in a fresh worktree. Copy it from the implementer lane; do **not** run `build-p0-core-ref.sh` (8.0.134 rebuilds a different hash and breaks the pin).
- Protected listeners, keep alive and untouched: `127.0.0.1:33767`, `:43649`, `:46593`.
- Only real browser on the machine: Windows **Microsoft Edge** at `/mnt/c/Program Files (x86)/Microsoft/Edge/Application/msedge.exe`, driven headless over CDP. No Chromium/Playwright/Firefox in WSL. `node` v26 exists only in a **login** shell (vite-plus path). The committed driver is `H/browser-driver/scenario.mjs` + `cdp.mjs`.
- Never commit a `/s/<token>/` capability URL; it changes every launch.

## 10. Rules for whoever picks this up

- One writable lane, one writer. If you implement, create your own worktree/branch and record it; do not write in the implementer lane while another writer holds it.
- No merge to `main`, no force push, no history rewrite, no deletions. G4 integration needs a separate Manager authorisation and a designated integrator.
- Do not touch `Assets/**`, Unity, `#886 Compact30`, `Legacy40`, `Enlarged80`, `Reference Future v1`, saves or packages.
- Do not flip the Core default: `new MultiAreaContinuation(experiment)` must stay Model 0/0/0; `/multi` opts into 1/1/1 at session creation only.
- Do not edit `src/Ui/PlayerApi.cs` or `src/Ui/Program.cs` without stopping and reporting; the guards and the page policy are shared with the single-area page.
- Do not re-litigate the four owner decisions in §3, and do not "re-merge" the #885 UI: it is already an ancestor (`git merge-base` of the Stage 4 branch and `feat/multi-885-astra-opus-ui-20261122-v1` equals the #885 head).
- Keep the frozen anchors and the P0 pin byte-identical; any drift is a stop condition.
- Publish a GitHub-tracked Markdown handoff for any substantive result; a local-only handoff is administratively incomplete.

## 11. Suggested next tasks, in order

1. **Owner playtest** (`/multi`, both resolutions, the flows in §9). This is the gate that actually closes the task; the review only authorises the playtest.
2. **F-2 bounded fix** if the owner wants it: presentation-state correction in `PlayerMultiApi.cs` plus a paired-snapshot re-read, then a focused re-review. Acceptance: an unavailable read-out never shows model-0 claims and never offers an Execute dialog; no engine change; tests for the null-read-out path.
3. **F-7 documentation upkeep**: the four stale summary lines listed in §8.
4. **Payload trim** only if measurement justifies it: each `/multi` view response is ~342 KB; measure before changing anything.
5. **G4 integration** only after 1–4 and only with a separate authorisation.

Stop and report rather than improvise if: a change is needed outside `Tools/MultiRackSitkaP1Lab/{src/Core,src/Ui/PlayerMultiApi.cs,tests}`, any anchor or the P0 pin drifts, an owner decision in §3 is contradicted, or the real browser cannot be driven.

## 12. Provenance of this document

Written by the orchestration route (`opencode-go / DeepSeek V4.1 Flash 1017`) acting as coordinator and D-091 custodian, from observed repository state on 2026-11-23. It contains no source change. Claims are separated into coordinator-observed (§7 rows marked "Coordinator"), reviewer-observed, and worker claims, so a later reader can tell what was actually run.
