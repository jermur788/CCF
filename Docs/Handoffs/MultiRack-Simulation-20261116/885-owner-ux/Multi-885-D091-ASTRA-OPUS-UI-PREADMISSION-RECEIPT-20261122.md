# Multi #885 — D-091 pre-admission: Astra → one Opus 5.5 UI child at exact baseline

**ADMITTED (reservation only).** Exactly one future Claude Opus 5.5 child of the Astra thread may implement the finished navy/slate UI at exact baseline `a692e1bf`, inside one new lane, with a single-file source allowlist. **Custody only** — the custodian launched nothing, created no checkout, built nothing, edited nothing.

Issued **2026-11-22T09:23Z** (observed clock `2026-11-22T09:23:00+00:00`).

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-astra-d091-admission-20261122-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-D091-ASTRA-OPUS-UI-PREADMISSION-RECEIPT-20261122.md

## 1. Issuer, requester, and the exact-base problem

| Item | Value |
|---|---|
| Issuer | existing D-091 physical custodian — opencode-go / DeepSeek V4.1 Flash, T3 session `a8d35599-48af-4f1b-a652-26f5ff44da5e` |
| Requester | Astra thread **`1f1a1a27-d17e-445f-b302-9670616e7494`** — verified live as "Forestry Simulator Dashboard Redesign", `codex_3` / **gpt-6-astra**, `running`, `branch: main`, **`worktreePath: null`** |
| Design claim | #885 comment `7076897286` (2026-11-22T09:02:56Z), docs-only, branch `docs/multi-885-astra-final-forest-ui-20261122-v1` |
| Design document | **NOT YET PUBLISHED** — the branch exists (head `3a67d098`) but `…/885-owner-ux/Multi-885-ASTRA-FINAL-DESIGN-AND-OPUS55-IMPLEMENTATION-PACKET-20261122.md` returns **404** at issue |
| Baseline | **`a692e1bf5d21304d06e696db1f47d1988ff80ee1`** — verified commit, `2026-11-22 01:16:35 +0000`, "docs(multi-885): record build, HTTP check and Opus 5.5 review outcome…", tip of `feat/multi-885-owner-ux-20261122-v1` |
| Target branch | `feat/multi-885-astra-opus-ui-20261122-v1` — **absent** locally and on GitHub (free) |

**Measured hazard (why naive delegation is refused):** Astra's checkout is the project root `/home/jer/Development/CCF-HP`, which is on **`main` @ `cbf3a618`** — not the baseline. T3 `delegate_task` with `isolation:"worktree"` bases the child on the **parent's current commit**, so it would silently produce a `cbf3a618` implementation, i.e. implementation against old main. This receipt therefore **refuses** that route.

## 2. Binding recommendation — tool-supported, exact base (no shell checkout by the custodian)

**Preferred route (single lane, child shares Astra's own exact-base checkout):**
1. Astra calls `t3_worktree_handoff` with
   `branch: "feat/multi-885-astra-opus-ui-20261122-v1"`,
   `baseRef: "a692e1bf5d21304d06e696db1f47d1988ff80ee1"`,
   `startFromOrigin: false`,
   **`runSetupScript: false`** ← mandatory: a repo setup script could build, which this admission forbids.
2. Astra verifies the new checkout **before** delegating: `git -C <worktreePath> rev-parse HEAD` equals `a692e1bf…` and `git status --porcelain --untracked-files=all` is empty. If either check fails, stop.
3. Astra dispatches **one** `delegate_task` with `isolation:"shared"` (the child then runs inside that exact-base worktree), `target: {providerInstanceId:"claudeAgent", model:"claude-opus-5-5"}`. Record the returned `childThreadId`/`childRunId` in the implementation packet.

**Accepted fallback (top-level thread bound to an explicit base):**
`t3_thread_launch` with `workspaceStrategy: {type:"worktree", branch:"feat/multi-885-astra-opus-ui-20261122-v1", baseRef:"a692e1bf…", startFromOrigin:false}`, `modelSelection: {instanceId:"claudeAgent", model:"claude-opus-5-5"}`, and `delegatedFrom: {environmentId, threadId:"1f1a1a27-…", title}`. The returned `threadId` becomes the bound child identity.

**Refused:** `delegate_task` with `isolation:"worktree"` from Astra's current checkout; any shell-created checkout by the custodian; any reuse of the OwnerUX or demo lane.

**Reserved lane path** (must be absent at creation, and is absent now):
`/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-AstraOpusUI-20261122`

## 3. What is reserved

| Field | Grant |
|---|---|
| Child identity | **exactly one** future child of `1f1a1a27-…`, provider `claudeAgent`, model **`claude-opus-5-5`**; the concrete thread id is recorded at dispatch. A second child requires a new receipt |
| Lane | one worktree at the path above, branch `feat/multi-885-astra-opus-ui-20261122-v1`, HEAD **exactly** `a692e1bf5d21304d06e696db1f47d1988ff80ee1` |
| Source writes | **only** the *literal interiors* of `MultiAreaPage.Html`, `MultiAreaPage.Css` and `MultiAreaPage.Js` inside `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` |
| Baseline file pin | `PlayerMultiApi.cs` @ baseline = blob **`7a589996a6a56b1852b8fff2e8ad0d85e6256b3e`**, 126 455 bytes |
| Documentation write | **one** new file only: `Docs/Handoffs/MultiRack-Simulation-20261116/885-owner-ux/Multi-885-ASTRA-OPUS-UI-IMPLEMENTATION-RESULT-20261122.md` |
| Writable outputs | that worktree's own `bin/`/`obj/` **only if** a build is later separately authorised; otherwise none |
| Term | open-ended under D-091; ends on documented completion/release, owner cancellation/handoff, or genuine conflict. **No fabricated expiry** |

## 4. Allowlist verification the child must produce

1. Every `git diff a692e1bf -- Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` hunk lies **entirely** inside a string-literal interior; no C# token, signature, method, route, field or comment outside the literals may change.
2. Evidence: versioned literal-masking script (recorded in the result Markdown) plus `before.masked.sha256` and `after.masked.sha256`, which must be **equal**. Astra re-runs it independently.
3. The result Markdown records the design document URL it implemented, the exact baseline SHA, the child thread id, the diffs applied, and the masked-hash equality.
4. If any non-literal byte differs, the run is a **FAIL** and stops — no "small extra fix" is permitted under this receipt.

## 5. Protected (must not be touched, read or written)

`Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/**`, all `src/Core/**`, `.p0ref` and P0 references, fixtures and goldens, every other `src/Ui` file byte, `Assets/**`, Unity, `main`, saves, and all other lanes. `wwwroot` is explicitly **not** in scope even though the UI page is embedded in `PlayerMultiApi.cs`.

## 6. Live state verified at issue

| Check | Value |
|---|---|
| Listeners | `127.0.0.1:33767` (#877, PID 1750123), `:43649` (#882, PID 1806361), **`:46593` (OwnerUX, PID 839989)** — all alive, all preserved |
| OwnerUX lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-OwnerUX-20261122` @ **`a692e1bf`**, branch `feat/multi-885-owner-ux-20261122-v1`, clean — **in live owner use, never assignable** |
| Demo lane | `CCF-Multi-885-Demo-Playable-20261121` @ `4cecf873` — note: **1 untracked file** `src/ccf-context.md` and a stray interactive `-bash` (PID 3223141) sitting in `…/src`; preserved untouched, not assigned, not cleaned by me |
| #867 | PID 390578 absent; owner's incident; not released |
| Integration / author / reviewer | `4cecf873` / `ac4c7b51` / `58950a9a`, all clean |
| Primary checkout | `/home/jer/Development/CCF-HP` on `main` `cbf3a618`, clean — the mismatched base that motivated §2 |
| Worktrees registered | 30 |
| Disk | 875 G available |

## 7. Prohibitions

No build or restore (including through a repo setup script), no P0 rebuild or regeneration, no `dotnet` invocation, no Unity or Windows build, no process kill/restart of #877/#882/OwnerUX/demo, no cleanup or deletion in any lane, no QA programme, no test-suite expansion, no `main` merge, no PR, no board edit, no ownership transfer, no second child, no shell-created checkout by the custodian, and no arbitrary expiry or auto-reclaim.

## 8. Stop conditions

Design document still unpublished (the child must cite a real URL before writing) · checkout HEAD ≠ `a692e1bf` · lane dirty at start · any non-literal change · `wwwroot`/Core/P0 touched · need to build · a foreign process on the lane · identity mismatch at dispatch. On any of these: stop, report the exact fact, take no workaround.

## 9. Non-claims

No design approval, no implementation, no review verdict, no QA, no Stage I or G2 status, no Q1 authorisation. This receipt reserves a lane and fixes a binding route; it grants no code judgment. Astra retains sole Opus dispatch and its own review role; the design document itself remains Astra's responsibility and is currently unverifiable.
