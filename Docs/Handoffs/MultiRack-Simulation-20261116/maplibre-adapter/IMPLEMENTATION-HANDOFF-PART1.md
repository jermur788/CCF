# Open-Source Satellite Map Plug — MapLibre Display Adapter + Geo-Anchor Bridge — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: CCF 2D Simulator — Playable Milestone 7 (Part 1), fallback builder
Worker: Claude Opus 5.5 (claude-opus-5-5), effort as configured by the dispatching thread; sole implementation writer.
        Took over an interrupted, uncommitted draft by Gemini 4 Neon (high), which stopped on a provider quota error.
        The draft was audited and largely rewritten (see "Audit of the inherited draft"). Neither author has reviewed the other's work.
Branch: feat/multi-2d-maplibre-adapter-v1   (review copy: review/multi-2d-maplibre-adapter-v1)
Base: b2006767cbe70fa2c822497470908251a0fd6ffd (origin/integration/2d-sim, Scots Pine Part 2 merged green)
Context: origin/main = bdeb6a324acaf4129c10fb0846faac8b64974360 (untouched)
Code under test (repair cycle 6): 223932ccb07a7f588c664cb827304989790bb479 (fix(2d-sim): repair cycle 6)
  Repair cycle 5 code: fd81415eac29dd82a85e19d2fbaa979d8ee38842 (control gap: withhold-A exited 1; small control fix requested)
  Repair cycle 4 code: a311ee7024ed9cf74a36d4b1ed7a881f1b4c2725 (fifth review: NEEDS CHANGES, 1 blocking G3 + 1 docs nit)
  Repair cycle 3 code: 8f1a62a3fd56dd854834c76967537278e64f3ca2 (fourth review: NEEDS CHANGES, 1 blocking + 1 low/G3)
  Repair cycle 2 code: ce7732a93d01ebd1fb37167591ca787d7000a225 (third review: NEEDS CHANGES, 2 blocking + 1 non-blocking)
  Repair cycle 1 code: e4b33fc1066a83d4eb2b03c7afccd268dbad4021 (delta re-review: REQUEST CHANGES, 2 blocking + 2 non-blocking + 1 nit)
  Original Part 1 code: b6cb1461fc28470cff064f55aad8934d71c8080a (reviewed by GPT-6.2 Sol: REQUEST CHANGES)
HEAD: the commit that updates this file (docs + evidence summary only; no code change after 223932cc)
GITHUB HANDOFF PATH: https://github.com/jermur788/CCF/blob/feat/multi-2d-maplibre-adapter-v1/Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/IMPLEMENTATION-HANDOFF-PART1.md
DURABLE EVIDENCE (repair cycle 6): /media/jer/Files/CCF/Verification/Multi2DMapLibreAdapterPart1-20261128/run-20261128T161844Z/
  (mutants: run-20261128T161844Z/mutants/; repair cycle 5: run-20261128T154601Z/ (+ mutants/); repair cycle 4: run-20261128T145800Z/ (+ mutants/);
   repair cycle 3: run-20261128T142326Z/ (+ mutants/);
   repair cycle 2: run-20261128T133605Z/; repair cycle 1: run-20261128T125152Z/;
   original Part 1 run: run-20261128T120314Z/)
```

**State (D-030):** isolated branch, implemented, repaired six times after independent review (repair cycle 6 is a small control fix to repair cycle 5's B24 self-test), and re-verified by its own harness. It still needs an independent non-author delta re-review. NOT merged to integration/2d-sim or main. Not wired into `/multi`. Part 2 remains **QUEUED**.

## Repair cycle 6 (small control fix to repair cycle 5)

Sole writer: Claude Opus 5.5. Fix commit `223932cc`. The file fence is unchanged. Code changes are limited to `tests/maplibre-adapter/`: `drive-maplibre-cdp.mjs` (B24 self-test 5 and the summary line) and `write-summary.mjs` (schema v7). The spec docs and this handoff were also updated. The adapter code did not change, and neither did `server.mjs` or `reconcileAttempts`. No case was deleted or weakened. The matrix is still B01–B31.

Gate run on clean `223932cc` (`run.sh`, exit 0, 2026-11-28T16:18Z, `testedTreeDirty: false`):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B31 31/31**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests, **0 self-tests skipped**;
- 208 browser and 212 server responses plus the node-side 404, all with the exact headers; 213 server receipts; 26 worker targets.

| # | Finding | Fix | Regression test | Evidence (run-20261128T161844Z) |
|---|---|---|---|---|
| C6-1 (control gap) | **The withhold-A control failed.** The prior review required that withholding A (the canceled request) must not create a false failure. In repair cycle 5, the `mixed-dup-withhold-a` mutant exited 1, although the per-attempt accounting was clean (0/0). The cause: the mixed-duplicate self-test's setup check required both A's and B's CDP request events. With A's events deliberately removed, the self-test could not see its own fixture and reported `set up=false`, which failed B24. | The self-test setup is split in two. **Fixture facts** come from the page and the server, so no CDP loss can change them: A rejects with `AbortError`; B returns 200 `ok`; there are exactly 2 receipts; receipt 1 never had a response written and the client closed it; receipt 2 has a delivered 200. **CDP observation** means which of A and B the collector saw. Outcomes: **(1) EXECUTED.** The fixture facts hold and both requests were observed, so both halves run as in cycle 5. **(2) SKIPPED (setup unavailable).** All of the following hold: the fixture facts; exactly 1 request observed, and it is B (200); A's own receipt never had a response written; and the **live** accounting for that URL is clean (A's receipt listed as abandoned, B paired, no unpaired server response or unmatched browser response at the URL). Then only the "withhold A" half is reported as `SKIPPED`. Its counted reason appears in `self_tests_skipped=N` (summary line), in `audit.selfTestsSkipped` (report), and in `selfTestsSkipped` / `selfTestSkipReasons` in `summary.json` (schema v7). The live accounting is that control, because A is already withheld. The "withhold B" half still runs. **(3) SETUP FAILED, so B24 fails.** This covers every other case: B unobserved, extra or missing requests, a response written for A (even to a closed socket), or fixture facts that do not hold. The main per-attempt accounting and every other audit check (setup failures, unattached sessions, unanswered requests, orphans, worker coverage) are unchanged and still fail B20/B24 on their own. | The clean gate must run both halves: `withhold only B → [B's receipt] [ok]`, `withhold only A → no gaps [ok]`, `self-tests skipped=0`. Twenty mutants, listed below. | PASS. Mixed duplicate: A aborted after receipt #211 → `AbortError`; B → 200 as receipt #212; `fixture set up=true, CDP-observed requests=2/2 (A seen, B seen)`. Withhold B → `[{"key":"/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate","seq":212,"status":200,"closedSocket":false}]` [ok]; withhold A → `{"server":[],"browser":[]}` [ok]; self-tests skipped=0. `summary.json`: `mixedDuplicate.withholdA = "no false failure"`, `selfTestsSkipped: 0`. Accounting: 27 exact targets, 213 receipts, 212 server responses, 212 browser requests; 0 unpaired / 0 unmatched / 0 orphans; 4 canceled requests, each paired with its own receipt |
| C6-2 (docs) | AC-3 and B24 did not describe what happens when a self-test's own fixture is withheld, and the mid-flight-abort limitation was only in the handoff | spec.md AC-3 and status, plan.md status, the tasks.md B24 row, a new T-06R6 row and the checklist row now describe the three outcomes, the counted skip, the requirement that the clean gate shows `self_tests_skipped=0`, and the residual limitation (below) | n/a (docs) | spec, plan, tasks, checklist and this handoff all say repair cycles 1–6, `B01`–`B31`, and Part 2 `QUEUED` |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs, `run-mutants.sh`, `mutate.py` and `mutants-summary.txt` in `run-20261128T161844Z/mutants/`; 20 mutants, 3 of them new):**

| Mutation (disposable copy) | Purpose | Expected | Matrix result |
|---|---|---|---|
| (a) `mixed-dup-withhold-b`: the collector drops B's `requestWillBeSent` and `responseReceived` | G3 (cycle 5) | B24 fails, naming B's URL, receipt and status | exit 1. **B24 FAIL** alone (30/31). Live accounting: `server-response-without-its-own-browser-request=1 [{"key":"/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate","seq":212,"status":200,"closedSocket":false}]`. Self-test: `CDP-observed requests=1/2 (A seen, B absent)`, withhold-A half `[SETUP FAILED]` (not skipped); `self_tests_skipped=0` |
| (b) `mixed-dup-withhold-a`: the collector drops A's (the canceled request's) `requestWillBeSent` and every later event with its id | the C6-1 control | **exit 0**, B24 PASS, self-test SKIPPED with a counted reason, 0 accounting mismatches | **exit 0, 31/31**, 0 console errors / CSP / failed / non-loopback / setup failures / unanswered, **`self_tests_skipped=1`**. Reason: `setup unavailable: request A's own CDP events are absent from this run (1/2 browser requests observed at /fixtures/abandoned-then-ok.txt?b24-mixed-duplicate; A's receipt #211 never had a response written); the live accounting already runs without A and is clean for this URL (A's receipt abandoned, B paired)`. Live accounting over 211 browser requests: **0 unpaired server responses, 0 unmatched browser responses, 0 orphans**; abandoned receipts `[{"key":"…?b24-mixed-duplicate","seq":211}]`. The withhold-B half still ran: `[{"…","seq":212,"status":200,…}] [ok]` |
| (c1) `audit-setup-failure` (new): `Network.enable` fails for the silent worker's CDP session (a real audit setup failure; nothing withheld from the mixed duplicate) | C6-1 must not weaken real setup failures | FAIL | exit 1. **B20 + B24 FAIL** (29/31). `setup_failures=1, unanswered=1`; worker coverage failures=2 for that target (`setup failed`, no Script-type startup response); `self_tests_skipped=0` |
| (c2) `mixed-dup-withhold-a-plus-setup-failure` (new): (b) and (c1) together | the skip must not mask a real failure elsewhere | FAIL | exit 1. **B20 + B24 FAIL** (29/31). `setup_failures=1, unanswered=1, self_tests_skipped=1`: the withhold-A half is skipped as in (b), but the setup failure still fails the run |
| (c3) `mixed-dup-withhold-a-receipt-answered` (new): A's events withheld **and** the server writes a late 200 for A after the client closed the socket | the skip must not apply when A's absence hides a written response | FAIL | exit 1. **B24 FAIL** alone (30/31). `fixture set up=false`; withhold-A half `[SETUP FAILED]`; `self_tests_skipped=0`; live accounting names A's response: `[{"key":"/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate","seq":211,"status":200,"closedSocket":true}]` |
| (d1) `cancel-prefers-delivered`: pass 2 lets a canceled request take a delivered receipt first (cycle-5 rule mutant) | G3 (cycle 5) | B24 fails | exit 1. **B24 FAIL** alone. Withhold B → `[]` `[FAIL]` (A absorbed B's 200); withhold A → no gaps [ok] |
| (d2) `global-cancel-allowance`: the repair-cycle-4 per-URL allowance restored inside the pairing (cycle-5 rule mutant) | G3 (cycle 5) | B24 fails | exit 1. **B24 FAIL** alone. Withhold B → `[]` `[FAIL]` |
| Repair-cycle-4 mutants, re-run on `223932cc`: `withhold-b27-exact-url`, `pathname-keys` | cycle-4 G3-2 | B24 fails | exit 1. **B24 FAIL** alone in each case |
| `rejection-no-elapsed-check`, `rejection-always-timeout` | cycle-4 B1 (B31) | B31 fails | exit 1. **B31 FAIL** alone in each case |
| `deadline-race-accepted`, `post-decode-check-only-settled` | cycle-3 B1 (B30, late decode) | B30 fails | exit 1. **B30 FAIL** alone in each case |
| `worker-fetch-counted`, `resource-type-ignored`, `startup-script-withheld` | cycle-3 B2 (worker) | B24 fails | exit 1. **B24 FAIL** alone in each case |
| `startup-response-withheld` (also a real audit setup gap: an unanswered worker request) | cycle-3 B2 | B20 + B24 fail | exit 1. **B20 + B24 FAIL** (1 unanswered request) |
| `b27-slow-start-cycle2-limit` / `b27-slow-start` / `b27-deadline-late` | cycle-3 N3 | fail / pass / fail | exit 1 **B27 FAIL** / exit 0 **31/31** / exit 1 **B27 FAIL** |

Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-6 choices a reviewer should check:**

- **The skip is narrow on purpose.** It needs all of these: fixture facts that CDP cannot affect, exactly B observed, A's receipt with **no** response ever written, and clean live accounting at that URL. Anything else is a setup failure. Mutant (c3) shows that a withheld A whose receipt *was* answered (even to a closed socket) is not skipped and is named by the live accounting.
- **A skip does not fail the run, but it is never silent.** It is counted in the `CDP_MATRIX_SUMMARY` line (`self_tests_skipped=N`), in the B24 detail, in `cdp-matrix-report.json` (`audit.selfTestsSkipped`) and in `summary.json` (`selfTestsSkipped`, `selfTestSkipReasons`). A qualifying clean gate must show `self_tests_skipped=0`. The committed `summary.json` does: both halves executed. I chose not to make `run.sh` fail on a skip: a run in which A's events were genuinely lost would otherwise fail, and that is exactly the false failure the control forbids. A reviewer who prefers a hard gate can add `self_tests_skipped == 0` to `run.sh`'s overall verdict.
- **Residual limitation (unchanged from cycle 5, now stated in AC-3/B24 too).** A fetch aborted mid-flight can have its 200 written by the server to a still-open socket, while Chromium reports only `loadingFailed (canceled)`, with no `responseReceived` or `responseReceivedExtraInfo`. That attempt can be matched to its receipt only by exact URL, send order and time window, never by a shared request id (the server cannot see CDP ids, and CDP does not report the client port). The real run has one such request, `/fixtures/synthetic-imagery.png`. The ambiguous residue is the same as in cycle 5: a canceled request that never reached the server, sent within 5 ms of a withheld 200 to the same URL and canceled no more than 20 ms before that 200's receipt. Not exercised.

## Repair cycle 5 (after the fifth independent review: NEEDS CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `fd81415e`. The file fence is unchanged: only `tests/maplibre-adapter/` code changed (`drive-maplibre-cdp.mjs`, `server.mjs`, `write-summary.mjs`), plus spec docs and this handoff. The adapter code did not change. No case was deleted or weakened. The matrix is still B01–B31, and B24 is stronger.

Gate run on clean `fd81415e` (`run.sh`, exit 0, 2026-11-28T15:46Z):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B31 31/31**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests;
- 208 browser and 212 server responses plus the node-side 404, all with the exact headers; 213 server receipts;
- 26 worker targets, each matched to its own startup response of resource type `Script`.

| # | Finding | Fix | Regression test | Evidence (run-20261128T154601Z) |
|---|---|---|---|---|
| G3 (blocking) | **A cancellation could cover another request's missing response.** In the repro, request A to an exact URL was canceled after the server received it, and the server never answered it. Request B to the same URL got 200. Withholding only B's two CDP events gave server = 1, browser = 0, canceled = 1, and B24 passed: the per-URL rule `browser ≤ expected ≤ browser + canceled` excused B's missing response | `server.mjs` now logs a **receipt** for every request, in arrival order, answered or not. Each receipt records its response status (null if none was ever written) and whether the client had already closed the socket. Responses carry their `receiptSeq`. `reconcileAttempts` replaces the per-URL tally (`reconcileExactUrls`). Each browser request is paired with **its own** receipt: same exact path + query, received no earlier than 5 ms before the browser sent it and, if canceled, no later than 20 ms after the cancellation. Pass 1: requests that got a response take the earliest such receipt with a **delivered** response (open socket) of the same status. Pass 2: canceled requests take the earliest such receipt that was **not** delivered, and a delivered one only if none is left. Afterwards, **every server-written response must be paired**: an unpaired one fails as "server response without its own browser request/response" and names the URL, receipt number and status. A browser response whose own receipt has no matching delivered response fails the other way. So a cancellation explains only its own attempt and never reduces the browser responses that server responses require. An unpaired receipt that never got any response is listed and allowed (it is not a response). The only exceptions left are the node-side 404 and cache hits. Orphan browser responses (no request event) also fail | **B24** gains a **real** mixed duplicate, run in the page before the whole-run checks: A = `fetch('/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate')`, aborted once the server has received it (the route never answers the first receipt of a target); B = the same exact URL, answered 200 with `ok`. The setup is asserted: A rejects with `AbortError`, B gets 200 `ok`, there are 2 browser requests and 2 receipts, receipt 1 has no response and the client closed it, receipt 2 has a delivered 200. In-run self-tests: **withhold only B's CDP events → must fail, with exactly one unpaired server response at that URL, B's receipt number and status 200**; **withhold only A's → no failure**. The earlier self-tests stay: withhold both events for the exact B27 URL; the same sweep over every server-answered target; drop a server response (reverse); withhold one duplicate `/harness.html` request; and the twin worker / resource-type tests | PASS. 27 exact targets, 213 receipts, 212 server responses, 212 browser requests: 0 unpaired server responses, 0 unmatched browser responses, 0 orphans, 0 abandoned receipts, 0 cache hits. Receipt-after-send gap −0.0 to 19.3 ms (tolerance 5 ms). Canceled requests = 4, each paired with its own receipt: the mixed-duplicate A (receipt with no response); 1 `/fixtures/synthetic-imagery.png` (aborted mid-flight; its own 200 was written to an open socket); 2 `/fixtures/held-imagery.png`. Mixed duplicate: A aborted after receipt #211 → `AbortError`; B → 200 as receipt #212; withhold B → `[{"key":"/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate","seq":212,"status":200,"closedSocket":false}]` [ok]; withhold A → no gaps [ok]. B27 exact URL → `{"server":["…?b27-stalled-decode"],"browser":[]}`; sweep 27/27; reverse → `{"server":[],"browser":["…?b27-stalled-decode"]}`; duplicate → `{"server":["/harness.html"],"browser":[]}` |
| Docs nit | The spec status line listed repair cycles only up to 3, and plan.md said `B01`–`B30` | spec.md status now records repair cycles 1–5 and the `B01`–`B31` matrix; plan.md status and harness step say `B01`–`B31`; AC-3, the tasks.md B24 row, the new T-06R5 row and the checklist B24 row describe per-attempt accounting | n/a (docs) | plan.md no longer mentions `B30` alone; spec, plan, tasks, checklist and this handoff all say `B01`–`B31` and Part 2 `QUEUED` |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs, `run-mutants.sh`, `mutate.py` and `mutants-summary.txt` in `run-20261128T154601Z/mutants/`; 17 mutants):**

| Mutation (disposable copy) | Finding | Expected | Matrix result |
|---|---|---|---|
| (a) `mixed-dup-withhold-b`: the collector drops B's `requestWillBeSent` and `responseReceived` (the reviewer's reproduction) | G3 | B24 fails, naming B's response | exit 1. **B24 FAIL** alone. Accounting: 1 unpaired server response, `{"key":"/fixtures/abandoned-then-ok.txt?b24-mixed-duplicate","seq":212,"status":200,"closedSocket":false}`; 0 unanswered, and B20 passed. The in-run self-tests also fail, because they expect a clean baseline |
| (b) `mixed-dup-withhold-a`: the collector drops A's (the canceled request's) `requestWillBeSent` (control) | G3 | no false failure from the accounting | Accounting: **0 unpaired server responses, 0 unmatched browser responses, 0 orphans**; A's receipt without a response is allowed as abandoned. The matrix still exits 1 with **B24 FAIL** alone, but only because the in-run mixed-duplicate self-test refuses to run without A's request event (`set up=false`, so it cannot prove its own control). That is intended: a self-test that cannot see its fixture must not report ok. The in-run control on the real data (withhold A → no gaps) passes in the gate run |
| (c) `cancel-prefers-delivered`: pass 2 lets a canceled request take a delivered receipt first, with no events lost | G3 | B24 fails (self-test) | exit 1. **B24 FAIL** alone. Withhold B → `[]` `[FAIL]` (A absorbed B's 200); withhold A → ok |
| (d) `global-cancel-allowance`: the repair-cycle-4 rule restored inside the pairing (unpaired responses excused while the URL has any cancellation), with no events lost | G3 | B24 fails (self-test) | exit 1. **B24 FAIL** alone. Withhold B → `[]` `[FAIL]` |
| Repair-cycle-4 mutants, re-run on `fd81415e`: `withhold-b27-exact-url`, `pathname-keys` | cycle-4 G3-2 | B24 fails | exit 1. **B24 FAIL** alone in each case |
| `rejection-no-elapsed-check`, `rejection-always-timeout` | cycle-4 B1 (B31) | B31 fails | exit 1. **B31 FAIL** alone in each case |
| `deadline-race-accepted`, `post-decode-check-only-settled` | cycle-3 B1 (B30, late decode) | B30 fails | exit 1. **B30 FAIL** alone in each case |
| `worker-fetch-counted`, `resource-type-ignored`, `startup-script-withheld` | cycle-3 B2 (worker) | B24 fails | exit 1. **B24 FAIL** alone in each case |
| `startup-response-withheld` | cycle-3 B2 | B20 + B24 fail | exit 1. **B20 + B24 FAIL** (1 unanswered request) |
| `b27-slow-start-cycle2-limit` / `b27-slow-start` / `b27-deadline-late` | cycle-3 N3 | fail / pass / fail | exit 1 **B27 FAIL** / exit 0 **31/31** / exit 1 **B27 FAIL** |

Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-5 choices a reviewer should check:**

- **A canceled request can still be paired with its own delivered 200.** The real run has one: a `/fixtures/synthetic-imagery.png` request that the browser aborted mid-flight. The server wrote its 200 to a socket that was still open, and Chromium reported only `loadingFailed (canceled)`: no `responseReceived`, no `responseReceivedExtraInfo`. That is why the fallback in pass 2 exists. It is safe because canceled requests are paired after every responded request has taken its own receipt, prefer undelivered receipts, and must have sent the request before the receipt and been canceled no more than 20 ms before it. The measured receipts of canceled requests arrived 1.7 to 1,500 ms **before** the cancellation.
- **Pairing uses time and order, not a shared request id.** The server cannot see CDP request ids, and CDP does not report the client port. Two same-URL requests sent within the 5 ms tolerance whose outcomes differ could still be ambiguous. Pass 2's preference for undelivered receipts settles that in the strict direction, for example the reviewer's case reversed. The residual case is a canceled request that never reached the server, sent within 5 ms of a withheld 200 to the same URL and canceled no more than 20 ms before that 200's receipt. Not exercised.
- **Abandoned receipts are allowed** (receipts with no response ever written and no browser request), because the reviewer's control requires that withholding A must not fail. They are listed in the detail and counted in `summary.json` (0 in the gate run).
- **`summary.json` schema v6** adds `serverReceipts` and `attemptAccounting` (unpaired counts and the mixed-duplicate verdicts). The evidence folder also gains `browser-requests.json` and `server-receipt-log.json`.

## Repair cycle 4 (after the fourth independent review: NEEDS CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `a311ee70`. The file fence is unchanged: no C#, shell, save, Unity, `.meta`, build, package or `Docs/Project` change. No case was deleted or weakened. B31 is new. B24 now has exact-URL accounting. B30 (the success-path race) is unchanged.

Gate run on clean `a311ee70` (`run.sh`, exit 0, 2026-11-28T14:58Z):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B31 31/31**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests;
- 207 browser and 211 server responses plus the node-side 404, all with the exact headers;
- 26 worker targets, each matched to its own startup response of resource type `Script`.

| # | Finding | Fix | Regression test | Evidence (run-20261128T145800Z) |
|---|---|---|---|---|
| B1 | **An overdue decode rejection skipped the timeout and the abort.** Chromium's decoder rejected a corrupt bitmap. The rejection was held until 1,820 ms into a 1,200 ms probe, then released while the deadline timer was still queued. The catch recorded `error`, cleared the timer and never fired the abort signal | The `catch` in `probeImagery` now runs the same `overdue()` elapsed-time check as the success path before it settles. If the deadline has passed, `overdue()` calls `timeOut()`. That settles the probe as `timeout` (and `settle` clears the timer), then aborts the controller with reason `timeout`. The settled probe stays in the per-URL cache, which lives as long as the adapter. A rejection that arrives in time still settles as `error`. The test-only debug handle now lists each probe's `reason`, `aborted` and `abortReason` | **B31** (new, deterministic). Deadline 1,500 ms. **Control:** corrupt PNG bytes (`/fixtures/corrupt-imagery.png`, served as `image/png`) are rejected in time by Chromium's own decoder. Expected: reason `error`, no abort, the exact notice, no raster, 1 fetch. **Overdue:** a second corrupt URL. The harness runs the real decode at once but holds back its native rejection (`holdDecodes('predecode')` now holds rejections as well as bitmaps). Starting 150 ms before the deadline, one task blocks the main thread until 600 ms past it, then rejects the held decode synchronously, so the catch runs before the overdue timer task. The preconditions are recorded. Expected: reason `timeout`, abort emitted with reason `timeout`, failures exactly `["timeout"]`, the exact notice, no raster, 0 pending, 1 fetch. After the timer: no change. Same URL again: no refetch, no decode. After a same-instance remount, both cached results (timeout/aborted and error/not aborted) show the notice with no refetch and no raster, and maps constructed = 2 | PASS. Control: `error`, aborted=false, failures `["error"]`, 1 fetch. Race: the blocking task began at 1,349.3 ms with 1 held native rejection, 0 bitmaps and pending = 1. `InvalidStateError` was released at 2,100.0 ms (deadline 1,500) with the timer still queued, and the probe settled before the timer turn. Result: `satellite/imagery-unavailable`, notice "Imagery unavailable. Showing woodland map.", layers `[ccf-woodland-canvas, ccf-woodland-extent]`, reason `timeout`, abort emitted (reason `timeout`), failures `["timeout"]`, 1 server fetch. Same URL: 0 refetches, 0 decodes. Remount: overdue URL cached `timeout`/aborted, timely URL cached `error`, 0 refetches, no raster, 2 maps |
| G3-2 | **B24 matched server responses by pathname.** Withholding both CDP events for the exact B27 imagery URL still passed, because other requests shared its pathname | Server log entries now carry the exact request target (`url`). `reconcileExactUrls` keys server and browser (page and worker) responses by exact path + query and counts them as a multiset, in both directions. For each target: browser ≤ server − node-side ≤ browser + canceled. A server response with no browser response and no canceled request fails. So does a browser response with no server response. Three exceptions are allowed, are documented and are counted in the detail: the node-side 404, canceled/aborted requests (`loadingFailed.canceled`, never answered), and cache hits (response `from*` flags or `requestServedFromCache`). The old pathname `Set` check was removed | **B24**, with four in-run self-tests on the run's own data: (1) withholding both CDP events for `?b27-stalled-decode` must flag exactly that URL (3 other exact URLs share its pathname); (2) the same for **every** server-answered target; (3) dropping its server response must flag it in the reverse direction; (4) dropping one of several browser responses for a duplicate target (`/harness.html`) must be caught | PASS. 27 exact targets: 0 server responses without a browser response, 0 browser responses without a server response. Exceptions used: node-side 404 ×1; 3 canceled requests, which cover `/fixtures/synthetic-imagery.png` (server 33, browser 32, canceled 1) and `/fixtures/held-imagery.png` (server 2, browser 0, canceled 2); 0 cache hits. Self-tests: (1) `{"server":["…?b27-stalled-decode"],"browser":[]}`; (2) 26/26 caught; (3) `{"server":[],"browser":["…?b27-stalled-decode"]}`; (4) `{"server":["/harness.html"],"browser":[]}` |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs, `run-mutants.sh`, `mutate.py` and `mutants-summary.txt` in `run-20261128T145800Z/mutants/`):**

| Mutation (disposable copy) | Finding | Expected | Matrix result |
|---|---|---|---|
| (a) `rejection-no-elapsed-check`: the catch settles `error` with no elapsed-time check (the reviewer's defect) | B1 | B31 fails | exit 1. **B31 FAIL** alone. The race was set up, but the result was reason `error`, abort emitted = false (reason null), failures `["error"]`. The notice and no-raster checks still held, as in the review |
| (c) `rejection-always-timeout`: every rejection becomes a timeout (control sensitivity) | B1 | B31 fails (control) | exit 1. **B31 FAIL** alone. The control reported reason `timeout`, aborted = true, failures `["timeout"]` `[FAIL]`; the overdue phase passed |
| (b) `withhold-b27-exact-url`: the collector drops both `requestWillBeSent` and `responseReceived` for the exact B27 URL (the reviewer's reproduction) | G3-2 | B24 fails | exit 1. **B24 FAIL** alone. Server-without-browser = 1: `{"key":"/fixtures/synthetic-imagery.png?b27-stalled-decode","server":1,"browser":0,"canceled":0}`. Unanswered = 0 and B20 passed, so only the exact-URL accounting catches this. The in-run self-tests also fail here, because they expect a clean baseline |
| (d) `pathname-keys`: the accounting is keyed by pathname only, with no events lost | G3-2 | B24 fails | exit 1. **B24 FAIL** alone. Self-test (1) cannot isolate the B27 URL (0 exact siblings) `[FAIL]`, and the reverse self-test `[FAIL]` |
| Repair-cycle-3 mutants, re-run on `a311ee70`: `deadline-race-accepted`, `post-decode-check-only-settled` | B30 | B30 fails | exit 1. **B30 FAIL** alone in each case |
| `worker-fetch-counted`, `resource-type-ignored`, `startup-script-withheld` | cycle-3 B2 | B24 fails | exit 1. **B24 FAIL** alone in each case |
| `startup-response-withheld` | cycle-3 B2 | B20 + B24 fail | exit 1. **B20 + B24 FAIL** (1 unanswered request) |
| `b27-slow-start-cycle2-limit` / `b27-slow-start` / `b27-deadline-late` | cycle-3 N3 | fail / pass / fail | exit 1 **B27 FAIL** / exit 0 **31/31** / exit 1 **B27 FAIL** |

Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-4 choices a reviewer should check:**

- **Canceled requests give a one-sided allowance for each exact target.** A canceled request may or may not have reached the server, so it can cover at most one server-only response for its own exact URL. It never covers a different URL. Withholding the events of a canceled request removes the allowance too, so the check still fails.
- **Cache hits are excluded from the browser side.** No cache hit is expected, because every harness response is `no-store`. The count is reported (0 in this run). A cache hit served from cache counts as neither a browser response nor a server response.
- **`holdDecodes('predecode')` now also holds native rejections.** Before this change it rejected at once. Only B30 and B31 use this mode, and B30 uses a valid PNG, so its behaviour is unchanged.
- **The debug handle gained `probes`** (url, settled, reason, aborted, abortReason). It is test-only, non-enumerable and keyed by a symbol. The public shape is unchanged.

## Repair cycle 3 (after the third independent review: NEEDS CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `8f1a62a3`. Same file fence; no C#, shell, save, Unity, `.meta`, build, package or `Docs/Project` change. No existing case was deleted. B24 was strengthened and B30 was added. B27 was relaxed only in its render-relative bound, as the review asked; the probe-relative deadline assertion is unchanged.

Gate run on clean `8f1a62a3` (`run.sh`, exit 0, 2026-11-28T14:23Z):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B30 30/30**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests;
- 187 browser and 191 server responses plus the node-side 404, all with the exact headers;
- 24 worker targets, each matched to its own startup response of resource type `Script`.

| # | Finding | Fix | Regression test | Evidence (run-20261128T142326Z) |
|---|---|---|---|---|
| B1 | Deadline race: with a 1,200 ms deadline, a decode delivered at 1,817 ms while the main thread was busy was accepted. Its promise continuation ran before the overdue `setTimeout`, so the raster loaded and no fallback appeared | `probeImagery` records `startedAt = performance.now()`. After the fetch, after the body read and after the decode, `overdue()` checks the time elapsed since the start. If the deadline has passed, it settles the probe as `timeout` and aborts the controller, instead of accepting the result. This works whatever the microtask/timer order is. A late callback finds the probe settled. The bitmap is always closed in `finally`. The settled timeout stays in the per-URL cache, which lives for the adapter's lifetime, so it survives a same-instance remount | **B30** (new, deterministic). Deadline 1,500 ms. The harness decodes the real bitmap at once but withholds it (`holdDecodes('predecode')`). 150 ms before the deadline, measured from the probe's own start, one page task blocks the main thread until 600 ms past the deadline. It then resolves the held decode synchronously, so the continuation runs as a microtask **before** the overdue timer task. The case records the preconditions (probe pending when the task began and at release) and fails if the race was not really set up. It asserts: probe failures exactly `["timeout"]`, exact imagery notice, no raster layer, one fetch, late bitmap closed. After the timer: no change. Same URL: no refetch or decode. After a healthy same-instance remount: same URL → notice, no refetch, no raster. A different healthy URL loads with attribution. Back to the timed-out URL → notice, no refetch | PASS. The blocking task began 1,349.5 ms after the probe start (pending = 1, 1 ready bitmap). The decode was released at 2,100.0 ms (deadline 1,500) with the timer still queued, and the probe settled before the timer turn. Result: `satellite/imagery-unavailable`, notice "Imagery unavailable. Showing woodland map.", layers `[ccf-woodland-canvas, ccf-woodland-extent]`, probe failures `["timeout"]`, server fetches 1, late bitmap `[[0,0]]`. Same URL: 0 refetches, 0 decodes. Remount: same URL notice, 0 refetches, no raster, maps constructed 2. Healthy URL: fallback `none`, attribution visible. Back: notice, 0 refetches |
| B2 | B24 accepted an ordinary `Fetch` as startup-script evidence. The collector matched session and URL but ignored the CDP resource type. A silent worker that fetched its own URL, with its startup events withheld, still passed | Each response now carries `resourceType` (CDP `Network.responseReceived.type`). All own-URL responses are kept in `ownUrlResponses`. `workerCoverageFailures` counts only entries with `resourceType === 'Script'` in the worker's own session (target and session id) with the target URL, 200 and exact headers. A same-URL `Fetch` never counts. The silent-worker fixture now fetches its own URL once, so every run contains a real same-session `Fetch` twin of a startup URL | **B24**: the type rule, plus an **in-run self-test**. The silent worker's `Script` responses are withheld and its real `Fetch` is kept; this must fail exactly the silent worker(s), and the `Fetch` twin must exist. The drop-one and same-URL-twin self-tests are unchanged. **B20**: an unanswered startup request still fails the run | PASS. Own-URL response types seen `["Fetch","Script"]`. Silent worker: 1 `Script` + 1 `Fetch` (both 200, same session). Self-test: withholding its `Script` responses but keeping the `Fetch` → 1 failure. Drop one → 1. Twin → 1 |
| N3 | B27's extra render-relative limit (≤ deadline + 1,000 ms from `render()`) failed falsely under concurrent load. WebGL start-up took 1,244 ms, then the probe settled at 2,199.6 ms against its 2,200 ms deadline | The render-relative bound is now a sanity limit (`deadline + 8000` ms). The notice wait was widened to match. Still asserted, unchanged: reason `timeout`, settlement within [deadline − 5, deadline + 150] ms of the probe's own start, notice ≤ 20 ms after settlement while the decode is still held, 0 pending probes | **B27** (relaxed bound only) | PASS. The probe started at 274.8 ms and settled 2,199.9 ms later with reason `timeout`. The notice came 2,474.8 ms after `render()`, with the decode still held |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs, `run-mutants.sh`, `mutate.py` and `mutants-summary.txt` in `run-20261128T142326Z/mutants/`):**

| Mutation (disposable copy) | Finding | Expected | Matrix result |
|---|---|---|---|
| (a1) `deadline-race-accepted`: the three `overdue()` checks reverted to the repair-cycle-2 code (the reviewer's race) | B1 | B30 fails | exit 1. **B30 FAIL** alone. The race was set up (pending = 1 at release, 2,100 ms vs 1,500 ms). Result: `satellite/none`, empty notice, raster layer `ccf-georeferenced-imagery` present, no probe failure, 2 fetches (probe + MapLibre) |
| (a2) `post-decode-check-only-settled`: only the post-decode check weakened to "already settled?" | B1 | B30 fails | exit 1. **B30 FAIL** alone, same symptoms as a1 |
| (b1) `worker-fetch-counted`: the reviewer's reproduction. The silent worker's startup `Script` request and response are withheld, its same-URL `Fetch` is kept, and the coverage rule ignores the resource type | B2 | B24 fails | exit 1. **B24 FAIL**. Coverage failures = 0 (the `Fetch` counted, as in the review), and the in-run self-test reports 0 failures `[FAIL]`. B20 passes, because nothing was left unanswered |
| (b2) `resource-type-ignored`: the type rule removed, no events lost | B2 | B24 fails | exit 1. **B24 FAIL** alone: the self-test reports 0 failures `[FAIL]` |
| (b3) `startup-script-withheld`: the startup `Script` request and response withheld, type rule intact | B2 | B24 fails | exit 1. **B24 FAIL** alone: 1 coverage failure "no Script-type startup response in its own session" |
| (b4) `startup-response-withheld`: only the startup `Script` response withheld | B2 | B24 + B20 fail | exit 1. **B20 FAIL** (1 unanswered request) and **B24 FAIL** (1 coverage failure, 1 unanswered) |
| (c1) `b27-slow-start-cycle2-limit`: probe start delayed 1.3 s after `render()` (simulated load), with the repair-cycle-2 render limit restored | N3 | B27 fails (reproduces the false failure) | exit 1. **B27 FAIL** alone. The probe settled 2,199.9 ms after its own start, but the notice came 3,694.0 ms after `render()`, over the old 3,200 ms bound. The detail string still prints the new limit text, because only the condition was patched |
| (c2) `b27-slow-start`: the same load with the repaired B27 | N3 | exit 0 | exit 0, **30/30**: no false failure |
| (c3) `b27-deadline-late`: a real deadline defect (timer fires 400 ms late) | N3 | B27 fails | exit 1. **B27 FAIL** alone: settled 2,599.7 ms after the probe start, outside [2195, 2350] |

Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-3 choices a reviewer should check:**

- **The elapsed-time check uses `>= timeoutMs` from `performance.now()` at probe creation.** A step that finishes exactly at the deadline is a timeout. The probe start is taken just before the `imagery-probe-started` diagnostic, so B30's busy-wait (measured from that diagnostic) overshoots the internal deadline by at least 600 ms.
- **The silent-worker fixture changed.** It now fetches its own URL. That is the only way to get a genuine same-session `Fetch` twin in every run. B17 is unaffected: the worker still never answers MapLibre.
- **B30 depends on `holdDecodes('predecode')`.** The race needs a bitmap that is ready before release. The harness decodes it at hold time and withholds it, so the release is synchronous inside the blocking task.

## Repair cycle 2 (after the delta re-review: REQUEST CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `ce7732a9`. Same file fence; no C#, shell, save, Unity, `.meta`, build, package or `Docs/Project` change. No existing case was deleted or loosened. B24 and B26 were strengthened, and B27–B29 were added.

Gate run on clean `ce7732a9` (`run.sh`, exit 0, 2026-11-28T13:36Z):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B29 29/29**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests;
- 165 browser and 169 server responses plus the node-side 404, all with the exact headers;
- 22 worker targets, each matched to its own script response.

| # | Finding | Fix | Regression test | Evidence (run-20261128T133605Z) |
|---|---|---|---|---|
| B1 | A stalled `createImageBitmap` (after the image bytes arrived) kept the probe pending past its deadline. The imagery notice appeared only when the decode was released | `probeImagery` is now one settle-once promise. The deadline timer settles it to *unavailable* (reason `timeout`) whatever step it is in, then aborts the fetch. A later decode is a no-op: its bitmap is closed and discarded. An abort (display failure, destroy) also settles it. The timeout result stays cached per URL. The diagnostics now carry `imagery-probe-started` and the reason the probe failed | **B27**: the harness holds image decodes in the page (`holdDecodes`). The case checks: the server sent 200 for the image, the decode was held, the probe settled at its own deadline, the exact notice appeared, the map stayed usable, and a late release changed nothing. The same URL again was not refetched | PASS. Decode held at 643 ms. The probe started at 280.1 ms (`render()` first runs the synchronous WebGL check) and settled **2199.9 ms later with reason `timeout`**. The notice followed at 2480.1 ms with the decode **still held** and 0 pending probes. The map was ready with the watchdog off and no raster; zoom +0.5 kept centres at 0.0024 px. After release: still `imagery-unavailable`, `imageryLoaded=false`, late bitmap closed (0 × 0). Same URL again: 0 refetches, 0 new decodes |
| B2 | B24 matched worker responses by script URL, so one worker's response satisfied every worker with that URL. The reviewer dropped one response and the matrix still exited 0 | The audit records each worker under its CDP session id and target id. In order, it enables Runtime, Log and Network, then calls `runIfWaitingForDebugger`, awaiting each step. Responses are filed against the session that reported them. `workerCoverageFailures` requires, for every target, a 200 script response in its own session with the target URL and the exact headers. The following all go into `setupFailures`, which fails B20 and B24 and the run exit code: a target without session or target id, an unexpected target type, a worker not paused for the debugger, a failed enable or resume, events from an unattached session, and an http(s) request with neither a response nor a loading failure. A worker's script request is announced in the page session and answered in the worker session under the same `requestId`; it counts as answered only there | **B24** (per-target coverage; in-run self-tests: removing one worker's response, or lending it a same-URL twin's, is caught for exactly that worker) and **B20** (setup failures, unanswered requests). **B26**'s Satellite half now runs on a fresh adapter, so it cannot inherit focus state | PASS. 22 targets, 22 distinct sessions, 2 script URLs, 0 coverage failures, 0 setup failures, 0 unanswered. Self-tests: drop one → 1 failure; borrow a twin's response → 1 failure. B26 Satellite half: focus before = `null`, crown moved 0.000 px, focus stays `null` |
| N3 | Focusing MA-A and then rendering a frame that shows only MA-E kept the old Satellite camera | `render()` clears a focus that the new frame hides. When a map exists it refits the Satellite camera on the visible woodland. Clean Woodland already reframed through `focusBox`; it now also clears the focus | **B28** (both styles: the hide-render result is compared with an explicit `focusArea(null)`) | PASS. Satellite: MA-E-T01 moved from x = 1147.6 (focused) to 610.5; the reference is 610.5 (0.000 px, zoom difference 1.2e-10), on stage, no new map. Clean Woodland: 611.4 against 611.4, same matrix |
| N4 | A rejected remote imagery URL with antimeridian-crossing bounds pushed a supported stand to Clean Woodland | `imageryEligibility()` runs **before** the stand preflight. It applies the imagery policy and then checks the imagery's own footprint. Imagery that fails is dropped (imagery-unavailable path, never requested). The stand preflight now uses the area bounds only | **B29** (anchor 179.99° E: a remote URL with crossing bounds; same-origin imagery whose own bounds cross; a control with no imagery) | PASS 3/3. All three cases stay in Satellite with the exact imagery copy, no raster, no `footprint-unsupported`, and crowns within 0.0017 px. The same-origin case reports `CROSSES_ANTIMERIDIAN`. The dropped same-origin URL was requested 0 times |
| NIT5 | §3 remount wording | §3 now says a **healthy** adapter rebuilds on its next Satellite frame, and that one with a sticky display failure (§8) stays on Clean Woodland | — | Wording only. §4 and §8 were also updated for N3/N4 and the whole-probe deadline |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs in `run-20261128T133605Z/mutants/`):**

| Mutation (disposable copy) | Finding | Matrix result |
|---|---|---|
| (a) `worker-script-omitted`: the CDP collector discards ONE vendored worker's own script response (the reviewer's omission) | B2 | exit 1. **B24 FAIL**: 1 coverage failure naming target `2D9A459A…` ("no script response in its own session"). **B20 FAIL**: 1 unanswered request. All other cases pass |
| (b) `decode-not-bounded`: `probeImagery` restored to the repair-cycle-1 version (the timer only aborts fetch; the decode is awaited) | B1 | exit 1. **B27 FAIL** alone: decode held at 645 ms, no notice by the deadline ("never"), no probe settlement |
| (c) `hidden-focus-no-refit`: the hide-render keeps the old Satellite camera (`syncSatelliteOverlay` only) | N3 | exit 1. **B28 FAIL** alone: Satellite crown stays at x = 1147.6 against the 610.5 reference (537 px); the Clean Woodland half passes |
| (d) `imagery-bounds-in-preflight`: imagery bounds put back into the stand footprint preflight | N4 | exit 1. **B29 FAIL** alone (1/3): both crossing-imagery cases fall to Clean Woodland with `footprint-unsupported`; the control passes |
| (e) `satellite-hidden-focus` (extra): a hidden lane is focused and framed only while Satellite is active | B2 (B26 independence) | exit 1. **B26 FAIL** alone: the Satellite half on a fresh adapter moved the crown 268.5 px (focus = MA-A); the Clean Woodland half passes |

Scripts: `run-mutants.sh` and `mutate.py` (copied into `mutants/`, with `mutants-summary.txt`). Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-2 choices a reviewer should check:**

- **Imagery whose own footprint does not fit is dropped, not the map.** The reviewer asked for invalid imagery to be dropped. Same-origin imagery that crosses the antimeridian is treated the same way, because it cannot be drawn on the one longitude branch. Only the stand footprint can now cause the display fallback.
- **A hidden focus is cleared, not remembered.** If the lane becomes visible again, the view stays on the whole woodland until `focusArea` is called again.
- **The B27 timing bound is relative to the probe's own start.** Under SwiftShader, the synchronous WebGL availability check in `render()` takes about 280 ms before the probe starts, so the absolute bound from `render()` is only a sanity limit (≤ deadline + 1000 ms).

## Repair cycle 1 (after GPT-6.2 Sol review: REQUEST CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `e4b33fc1`. Same file fence; no C#, shell, save, Unity, `.meta`, build or `Docs/Project` change.

Gate run on clean `e4b33fc1` (`run.sh`, exit 0, 2026-11-28T12:51Z):

- vendor integrity **16/16**;
- geo oracle **38/38** (28 before, plus 10 footprint cases);
- browser matrix **B01–B26 26/26**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests.

| # | Finding | Fix | Regression test | Result |
|---|---|---|---|---|
| B1 | An in-flight imagery probe could undo the WebGL fallback; a held probe at startup kept Clean Woodland with no notice past the watchdog | `failDisplay` bumps the generation and aborts every pending probe (`AbortController`). `startSatellite` treats `displayFailed` as stale. The watchdog is armed when the satellite start begins, so it also covers the module import. The imagery probe runs alongside the map start (it no longer gates `new Map`) and is limited to `imageryTimeoutMs` = `min(8 s, loadTimeoutMs / 2)` | **B22**: (a) the server holds a new imagery response, then a forced WebGL context loss, then release; (b) startup with the imagery held; (c) a module import that never finishes | PASS. (a) The pending probe was aborted (1 → 0); after release the notice persisted, with no map and maps constructed 1 → 1; a later satellite frame stayed on Clean Woodland. (b) The imagery notice appeared after 1885 ms (< watchdog), and release changed nothing. (c) The watchdog fired after 2053 ms and the late import built no map |
| B2 | Satellite projection broke across the antimeridian (Jacobian sampled wrapped longitudes) | New `checkWebMercatorFootprint`, run before any MapLibre work. The footprint is every area plus the imagery bounds, grown by 50 m, with longitudes unwrapped relative to the anchor. A footprint crossing ±180° (east or west) or reaching ±85.0511° falls back to Clean Woodland with the display copy. The fallback is per frame and not sticky | **B21** (east crossing 179.9996° E; west crossing 179.9996° W; Mercator limit 85.0508° N; near-antimeridian 179.99° E/W and 84.9° N in Satellite), plus 10 geo-oracle footprint cases | PASS. Crossings: Clean Woodland, the correct code, 0 maps, crowns finite and on stage, px/m spread 1.00000. Near-antimeridian: radius error 0.216 mm, centres 0.0017 px. 84.9° N: radius error 1.750 mm (≤ 2 mm), centres 0.014 px. A supported frame afterwards uses Satellite |
| B3a | `normalizeFrame` aliased and froze nested caller anchor data | Only the supported anchor fields are copied, and only as primitives (a non-primitive becomes `null` and is rejected). Only the adapter's own copy is deep-frozen | **B23** (nested `provenance` + extra nested data; non-primitive `lonDeg`) | PASS. Caller unchanged, 0 frozen caller objects, still mutable; the adapter copy is frozen and has no `provenance` |
| B3b | `areas: []` left the old crowns and transform; clicks emitted coordinates from the old frame | New `clearOverlay()` empties Layers 4–8 (reserved slots kept), the selection marks, the transform and the hit-test matrix whenever the frame has no drawable area or the transform is unusable | **B23** (`areas: []` in both styles, click at the old crown position) | PASS. 0 crowns, layers [0,0,0,0,0], matrix null, 0 clicks emitted. The refilled frame draws 20 crowns and its click carries the new `frameKey` |
| B4 | The CSP gate passed after a late response lost its header | **B08** now runs after every other case. It covers every browser response (page **and worker sessions**: workers auto-attach paused so their own script fetch is reported), every server-written response (headers recorded by the server, including held/aborted imagery) and the node-side 404. **B24** adds a coverage check (required response kinds, incl. the silent-worker fixture), worker-script accounting (each of 17 worker targets reported its own script), "no unaccounted server response", and a pure sensitivity self-test | **B08**, **B24**, plus the disposable-copy mutation `late-csp-header` | PASS. 141 browser + 145 server responses + the 404, all exact; 12/12 kinds; 0 unaccounted. Removing one header is detected (1 mismatch each) |
| N5 | Attribution stayed visible with the image wholly off-screen | `shouldShowAttribution` also requires `imageryInView`: the projected imagery quad intersects the stage (separating-axis test, re-evaluated on move/resize) | **B25** | PASS. Off-screen: hidden. Zoomed out until the image is on screen: shown. Zoomed back: hidden |
| N6a | Focusing a hidden lane moved the visible lane | `focusArea` ignores an area the current frame hides; `focusBox` only frames visible areas | **B26** | PASS. The visible crown moved 0.000 px (both styles) |
| N6b | Same-instance destroy + mount kept the Satellite status on an empty canvas | `destroy` resets the style, fallback, imagery state and watchdog, and aborts probes. Only the sticky display failure and the probe cache persist | **B26** | PASS. Remount shows Clean Woodland with no notice, no attribution, no crowns and no canvas; the next satellite frame rebuilds (maps constructed = 2) |
| N7 | B01/tasks wording | Spec, plan and tasks now say: production adapter = exactly five string-keyed own methods (`Reflect.ownKeys`); the opt-in debug handle adds two non-enumerable symbols. B01 asserts both | **B01** | PASS (no behaviour change) |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs in `mutants/`):**

| Mutation (disposable copy) | Reviewer finding | Matrix result |
|---|---|---|
| `late-csp-header`: server drops CSP from the later B16 non-image response | B4 | exit 1. **B08 FAIL** (browser 1 + server 1 mismatch); B24 also FAILs, because its sensitivity counts become 2 |
| `pending-imagery-vs-webgl-loss`: no generation bump, no probe abort, `displayFailed` not treated as stale (the pre-repair behaviour) | B1 | exit 1. **B22 FAIL**: after release the notice is cleared (`""`); in (c) the late import flips the style to `satellite` |
| `antimeridian-no-footprint-check`: footprint check disabled | B2 | exit 1. **B21 FAIL** 3/6: crossings go to Satellite with crowns off stage, or show no notice |
| `empty-frame-stale-overlay`: `drawOverlay` returns early instead of clearing | B3b | exit 1. **B23 FAIL**: 20 crowns left, layers [1125,5,20,5,7], 1 stale click emitted |
| `attribution-ignores-viewport` (extra) | N5 | exit 1. **B25 FAIL** (attribution shown with the image off-screen) |
| `focus-hidden-lane` (extra) | N6a | exit 1. **B26 FAIL** (the visible crown moved 261.8 px) |
| `caller-anchor-aliased` (extra): shallow anchor copy restored | B3a | exit 1. **B23 FAIL** (3 caller objects frozen, caller no longer mutable) |

Scripts: `run-mutants.sh` and `apply-mutant.py` (copied into the evidence run folder); summary in `mutants/mutants-summary.txt`. No Chromium process or working copy was left behind.

**Repair-cycle choices a reviewer should check:**

- **Antimeridian:** reject rather than wrap. Rendering across ±180° would need wrapped camera, overlay and image-source handling inside the vendored MapLibre, which Part 1 does not attempt. The fallback reuses the locked display copy (`Map display unavailable. Showing Clean Woodland.`) instead of inventing a fourth string. It is not sticky.
- **Sticky display failure survives same-instance remount** (the spec says "for its lifetime"). Everything else resets on `destroy`.
- **Imagery timeout:** a timed-out probe is cached as unavailable for the adapter's lifetime, the same as any other probe result.
- **High latitude:** at 84.9° N the single-affine overlay measures 1.75 mm against the 2 mm bar on the 80 m fixture. That is the expected second-order term (∝ tan φ · distance²). A larger stand at very high latitude would need a re-centred derivative, so this is noted as a limitation and not addressed.

---

*The sections below describe the original Part 1 delivery (code `b6cb1461`) and are kept for history; the repair table above supersedes their verification numbers.*

## What changed

- **Spec quartet** `specs/multi-2d-maplibre-adapter-v1/`, status `APPROVED / IMPLEMENTING`.
  - Part 2 is explicitly QUEUED: live `/multi` toggle, `MultiRoutes.cs`, `PlayerApi.cs` CSP, `SitkaP1.Ui.csproj` resources.
  - The four locked defaults are recorded: 8-layer stack; 65 % imagery-only opacity; exact fallback copy; session-only style (Clean Woodland default, no MapLibre load until switched).
  - Also included: B01–B20 definitions in `tasks.md`, and an offline EPSG:2157 provenance section (spec §6). That section lists the parameters and the datum-shift rules, invents no datum-shift accuracy, and states that Part 1 consumes WGS84 anchors only.
- **Vendored MapLibre GL JS 6.20.0** (BSD-3-Clause), unmodified:
  - `maplibre-gl.mjs`, `maplibre-gl-worker.mjs`, `maplibre-gl.css`, `LICENSE.txt` and `provenance.json`;
  - `provenance.json` records the tarball SHA-256, the registry `integrity`/`shasum`, per-file SHA-256 and size, how the files were extracted, and that the 0-byte `maplibre-gl-shared.mjs` was omitted.
- **Adapter** (`map/maplibre/`):
  - `createMapLibreAdapter()` has exactly the five contract methods. The style is chosen per frame (`frame.mapStyle`), so there is no sixth method.
  - Layers 3–8 are one SVG overlay in metres, placed by a single affine transform. In Clean Woodland that is a uniform scale; in Satellite it is the projected Jacobian. A crown is therefore `<circle r = radiusM>` in both styles.
  - MapLibre is imported lazily, only for satellite with a valid anchor. `setWorkerUrl(...)` runs before `new Map`.
  - The MapLibre style is inline only: a background, an invisible same-colour GeoJSON extent fill that proves the worker is alive, and the optional image/raster.
  - Three fallbacks with the exact copy; a readiness watchdog; a sticky display-failure state.
  - No shared-shell imports.
- **Geo-anchor bridge** `geo-anchor.js`:
  - WGS84 geodetic ↔ ECEF ↔ ENU with true-north clockwise bearing, forward and inverse.
  - Bound to the frame's `sourceId` and `geometryDigest`. `originKind` must be on an explicit allow-list (`survey-corner | survey-point | site-origin`). Centroid-derived anchors, missing identity, mismatches, out-of-range lon/lat and non-finite bearing are all rejected.
- **Harness** `tests/maplibre-adapter/`: `run.sh` runs vendor integrity, then the geo oracle, then the strict-CSP offline headless-Chromium matrix B01–B20. It writes evidence outside the repo plus `evidence/summary.json` here.

## Files changed (vs base b2006767)

`git diff --name-only b2006767...HEAD` gives 31 files, all inside the four authorised prefixes:

- `specs/multi-2d-maplibre-adapter-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/{adapter.js, adapter.css, clean-woodland.js, frame-projector.js, geo-anchor.js, imagery-policy.js, layer-stack.js, overlay-layers.js}`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/vendor/maplibre-gl-6.20.0/{maplibre-gl.mjs, maplibre-gl-worker.mjs, maplibre-gl.css, LICENSE.txt, provenance.json}`
- `Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter/{run.sh, server.mjs, drive-maplibre-cdp.mjs, harness.html, harness.js, harness.css, fixture-80x80.mjs, synthetic-png.mjs, test-geo-anchor.mjs, verify-vendor.mjs, write-summary.mjs, fixtures/silent-worker.mjs}`
- `Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/{IMPLEMENTATION-HANDOFF-PART1.md, evidence/summary.json}`

No existing file was modified. Nothing was touched among: the shared shell (`multi.html`, `main.js`, `store.js`, `map/contract.js`, `map/renderer-svg.js`, `map/minimap.js`, …), C#, `.csproj`, `wwwroot/`, save data, Unity, `.meta`, `build/` or `Docs/Project/`.

## Player-facing result

None in the live game yet: Part 1 is a standalone module that `/multi` does not load.

In the harness page the player-equivalent view works as follows (screenshots are in the evidence folder):

- The woodland opens in **Clean Woodland**: dark `#15251a` canvas, lane borders, moisture cells, logs, crowns at true size, saplings, crop/fell marks, ranks and selection.
- Switching a frame to **Satellite** (with a valid anchor) shows the same overlay over a MapLibre canvas, with the local imagery at 65 % and its attribution.
- If something is missing, the map degrades to a one-line notice instead of a popup, and the selection stays.

## Simulation / save impact

None. Presentation only (D-020): no forestry, competition, growth or money maths. No save fields. No C# changes. No RNG or engine changes.

## Verification (run on clean commit b6cb1461, 2026-11-28T12:03Z)

Command: `Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter/run.sh` (exit 0). Environment: Linux node v26.13.1 (`~/.local/share/vite-plus/bin`), Chromium 156.0.8078.79, WebGL2 through ANGLE/SwiftShader (Vulkan 1.3.0, SwiftShader Device (Subzero)). The full transcript is `run-sh-transcript.txt` in the evidence folder. Verbatim summary lines:

```text
VENDOR_INTEGRITY_SUMMARY: 16 passed, 0 failed, files_checked=4
GEO_ORACLE_SUMMARY: 28 passed, 0 failed, max_roundtrip_m=7.3065e-8, oracle_30m=3.9981e-7, oracle_80m=4.4374e-7, oracle_120m=3.3160e-7
WEBGL2_READY {"ok":true,"version":"WebGL 2.0 (OpenGL ES 3.0 Chromium)","renderer":"ANGLE (Google, Vulkan 1.3.0 (SwiftShader Device (Subzero) (0x0000C0DE)), SwiftShader driver)"}
PASS B01: assertMapRenderer passes; own properties=[destroy,focusArea,mount,render,setSelection]; production adapter (no debug) properties=[destroy,focusArea,mount,render,setSelection]; frozen=true
PASS B02: callerFrameUnmutated=true, normalisedFrameDeepFrozen=true, copiedNotAliased=true, frozenInputAccepted=true, radiusCopiedVerbatim=true
PASS B03: style=clean-woodland, notice hidden=true, canvas pixel=(21.0,37.0,26.0) (want #15251a), layer1=rgb(21, 37, 26), vendorRequests=0, workers=0, crowns=20
PASS B04: 6/6 invalid anchors -> "No map location set. Showing Clean Woodland." with selection kept, 0 modals [missing anchor:ok; centroid origin:ok; digest mismatch:ok; frame without source identity:ok; latitude out of range:ok; non-finite bearing:ok]; vendorRequests=0
PASS B05: callsBeforeMount safe=true; roots after double mount=1, after destroy=0; second destroy + calls after destroy safe=true (roots=0); remount roots=1
PASS B06: stale delayed module load ignored (style=clean-woodland, map=false, constructed=0); back-to-back satellite frames -> shown frameKey=key-B, maps constructed=1
PASS B07: module/worker/css 200 after switch; worker targets=1 all at /map/maplibre/vendor/maplibre-gl-6.20.0/maplibre-gl-worker.mjs (no blob/data); getWorkerUrl matches; setWorkerUrl before new Map=true; CSP violations=0; console errors=0
PASS B08: 18 browser responses all carry the exact CSP + Referrer-Policy (mismatches=0); server 404 also exact=true; http requests=19, non-loopback=0, failed=0
PASS B09: glyphs=null, sprite=null, sources=["geojson inline data","image /fixtures/synthetic-imagery.png"], layers=[ccf-woodland-canvas:background,ccf-woodland-extent:fill,ccf-georeferenced-imagery:raster], background=#15251a, extent fill=#15251a (Layer 1, worker-tiled)
PASS B10: DOM bottom->top [woodland-canvas > georeferenced-imagery > stage2-reserved > ground-overlays > schematic-deadwood > living-crowns > planted-juveniles > marks-ranks-selection] matches descriptors; reserved slots drone-lidar=false/0, hillshade-contours=false/0, streams-rivers=false/0, roads-bridges-paths=false/0; MapLibre canvas order background<extent fill (Layer 1)<raster (Layer 2); overlay above canvas=true; marks topmost=true
PASS B11: 20 crowns, r attribute == radiusM metres: true; rendered rim -> MapLibre unproject -> local metres: max radius error 0.2115 mm (<= 2 mm); centre vs map.project max 0.0017 px; MA-C-T03 (3.8 m) = 31.92 px
PASS B12: satellite 800x500: max error 0.2111 mm, MA-C-T03 31.92 -> 21.47 px (x0.673); clean woodland uniform scale, max error 0.0001 mm, 31.09 px @1280x720 -> 21.59 px @800x500 (x0.694)
PASS B13: after zoom +0.8 and pan (120,-60) px: centre error max 0.0030 px (< 0.5), radius error max 0.2095 mm
PASS B14: default raster-opacity=0.65; imagery pixel (135.0,122.0,69.0) vs 65% blend (134.8,122.2,68.9) (+-4); attribution visible "Synthetic local test imagery (not a real photograph)"
PASS B15: 0%: paint=0, pixel (21.0,37.0,26.0) (= canvas #15251a), attribution hidden=true; 100%: paint=1, pixel (196.0,168.0,92.0) (= imagery), attribution visible=true; layers 1,4-8 + overlay opacity stay 1=true
PASS B16: 2/2 -> "Imagery unavailable. Showing woodland map." on the dark MapLibre canvas (pixels (21.0,37.0,26.0), (21.0,37.0,26.0)), no raster layer, attribution hidden, selection kept, 0 modals [no imagery configured:ok; imagery URL returns a non-image:ok]
PASS B17: 5/5 -> "Map display unavailable. Showing Clean Woodland.", selection kept, 0 modals [WebGL unavailable (no vendor request):ok; module import failure:ok; worker never responds (load watchdog):ok; WebGL context lost on a live map:ok; later satellite frame stays on Clean Woodland (no retry loop):ok]
PASS B18: satellite [["MA-C",8.2,36,null],["MA-C",5.5,44,"MA-C-PJ01"],["MA-E",4.5,18,null]]; clean woodland [["MA-C",8.2,36,null],["MA-C",5.5,44,"MA-C-PJ01"],["MA-E",4.5,18,null]]; keys exactly {areaId,u,v,juvenileId,frameKey}; 64 px drag panned the map=true and emitted 0 callbacks
PASS B19: L4 lanes=5, moisture cells dry=576/moist=544, cell 2.50 x 2.50 m; L5 deadwood=5; L6 crowns=20 [ApprovedFell,Felled,Living,NaturalDead,PendingFell]; L7 juveniles=5 (shelter 1); L8 crop ring+C=true, pending dashed=true/approved solid=true, ranks=1,2, selection on top with frame+SEL+aria=true; focusArea centres x=8.00 (MA-A) -> 72.00 (MA-E), frame unchanged=true, selection kept=true
PASS B20: storage writes=0, local/session items=0/0, cookies="", no [C]/[E]/disclaimers=true; fresh load style=clean-woodland, vendor requests=0, workers=0; whole run: console errors=0, CSP violations=0, failed requests=0, non-loopback=0
CDP_MATRIX_SUMMARY: 20 passed, 0 failed, total=20, console_errors=0, csp_violations=0, failed_requests=0, non_loopback=0
OVERALL: PASS
```

| Gate | Result |
|---|---|
| Vendor integrity | **16/16 PASS**: 4 files hashed (SHA-256 + size), file list, no static imports, shared module unreferenced, CSS `data:` only |
| Upstream re-check (one-off, network) | tarball re-downloaded; SHA-256 equals `provenance.json`; all 4 files byte-identical (`upstream-reverification.txt`) |
| Geo oracle | **28/28 PASS**: max round trip 7.3e-8 m; Vincenty max 4.0e-7 m (30 m), 4.4e-7 m (80 m), 3.3e-7 m (120 m); both axis azimuths 3.7e-9°; 20 rejection cases + 1 accept |
| B01–B20 | **20/20 PASS**; 0 console errors (page + workers), 0 CSP violations, 0 failed requests, 0 non-loopback requests |
| Harness sensitivity (mutants, temp copy) | screen-isotropic crowns → B11/B12/B13 FAIL (72.5 mm); attribution always on → B15 FAIL; eager MapLibre import → B03/B04/B17/B20 FAIL (logs in `mutants/`) |

**Exact CSP header** (sent on every response, including 404):
`default-src 'none'; script-src 'self'; style-src 'self'; connect-src 'self'; worker-src 'self'; img-src data: blob: 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'` plus `Referrer-Policy: no-referrer`.

**Network fence:** `--host-resolver-rules="MAP * ~NOTFOUND, EXCLUDE 127.0.0.1"`, plus a dead proxy (`--proxy-server=http://127.0.0.1:9`) so that even IP-literal outbound requests would fail and be counted. Ports are ephemeral loopback only; the protected listeners 33767/43649/46593 were not used. Chromium runs in its own process group and is stopped by the driver, with a `trap EXIT` safety net. No Chromium process was left after any run.

## Audit of the inherited draft (what was wrong and fixed)

- `Object.freeze(renderer)` was followed by `Object.defineProperty(renderer, …)`, which throws a TypeError, so the adapter could not be created at all. The debug handle is now opt-in (`exposeDebug`) and added before freezing; production adapters have exactly five own properties.
- MapLibre `move`/`load` handlers were generation-guarded, so any later `render` stopped the overlay following pans and could leave the map never marked loaded. They are now guarded by map-instance identity.
- Crowns were drawn as screen circles from averaged east/north scales. Web Mercator on WGS84 is anisotropic (ν/ρ ≈ 1.0024 at 53°N, *computed*), which misses 2 mm on large crowns. The overlay now uses the projected Jacobian, measured at 0.21 mm.
- The anchor binding was skipped when the frame had no identity, and the centroid check was a blacklist. Both are now strict: the frame binding is required and `originKind` must be on an allow-list.
- The normaliser invented values (radius 1 m, log length 3.2 m, species `sitka-spruce`) and the overlay invented moisture classes not used elsewhere. Incomplete records are now skipped; moisture styling covers only the two classes the shell knows.
- Imagery-unavailable was tested with a deliberate 404, while the auditor ignored *any* network 404 log line. The 404 is replaced by a 200 non-image response, so the "zero failed requests" rule stays strict.
- MapLibre drew its own lane fill/outline over the imagery, tinting it. It was removed; the lanes are drawn only in the SVG overlay.
- The worker-failure case was untestable because the background-only style never needs the worker. The invisible extent source plus a readiness watchdog now make worker failure detectable (B17).
- `run.sh` used `node -e` with ESM imports and had a no-op cleanup trap. Replaced by `write-summary.mjs` and a real process-group cleanup.

## Deviations and interpretations (for the reviewer)

1. **Attribution at 0 %.** The brief says both "attribution only while imagery visible" and "imagery visible with attribution at 0/65/100 %". At 0 % the imagery is not visible, so the attribution is **hidden** (B15 asserts this). Flip `shouldShowAttribution` if the owner wants it shown at 0 %.
2. **Layer 1 in Satellite** also contains an inline GeoJSON woodland-extent fill in `#15251a` (invisible against the background) so worker liveness is observable. MapLibre's layer list is therefore background → extent fill → raster.
3. **Layer 2 DOM marker.** The imagery is drawn inside the MapLibre canvas. The `data-layer-index="2"` element is a zero-size state marker (`data-imagery-visible`, `data-imagery-opacity`); B09/B10 also assert the canvas layer order.
4. **Satellite camera bearing = anchor `bearingDeg`**, so the woodland stays upright in both styles (the map is not north-up when the bearing ≠ 0).
5. **Display failure is sticky** for the adapter's lifetime (no retry loop); a new adapter retries.
6. **Pointer outside every area** falls back to the active area (same as `renderer-svg.js resolveMapPoint`), and the callback name is `deps.onPointer`. Part 2 maps it to the shell's selection logic.
7. **2 mm is a geometric criterion.** At about 8 px/m, 2 mm is far below one pixel. B11–B13 measure it through the rendered SVG transform and MapLibre's own `unproject` + the inverse bridge, not by counting screenshot pixels. The opacity checks *are* pixel comparisons.
8. One extra file outside the brief's list: `write-summary.mjs`. Two extra adapter files: `overlay-layers.js` and `clean-woodland.js` (focused siblings, allowed by the brief).

## NOT TESTED / skipped

- **Skipped by instruction:** .NET builds/tests, `Phase1.Save.Tests`, `build/2d-gate.sh`, `build/2d-gate/selftest.sh`, Unity.
- **Hardware GPU WebGL:** only SwiftShader was exercised.
- **Other browsers** (Firefox/Safari): not tested.
- **Touch input:** not tested.
- **Live `/multi` integration, the C# route/CSP/resource path, and real anchors or real imagery:** Part 2, QUEUED.
- **Real-world accuracy:** imagery georeferencing accuracy and datum-shift accuracy are out of scope. The fixture is synthetic and only proves internal consistency.
- **Part 2 extents:** stands larger than about 120 m. The affine error grows roughly with the square of the distance from the woodland centre; not measured beyond the 80 m fixture in the browser.

- **Repair cycle 6 additions, not tested:** a self-test skip caused by a genuine (unmutated) CDP event loss for request A. The skip branch is reached only by the `mixed-dup-withhold-a` mutants; the clean gate ran both halves (`self_tests_skipped=0`).
- **Repair cycle 5 additions, not tested:** per-attempt pairing under HTTP caching (every harness response is `no-store`, so the cache-hit count was 0); concurrent same-URL requests with mixed outcomes inside the 5 ms tolerance (see the repair-cycle-5 choices); HTTP/2 or remote servers (the harness is HTTP/1.1 on loopback, and the pairing relies on one host clock).
- **Repair cycle 4 additions, not tested:** decoder rejections from anything other than corrupt PNG bytes in Chromium 156; a real main-thread stall (B31, like B30, simulates one with a busy-wait inside a harness task); exact-URL accounting with HTTP caching enabled (every harness response is `no-store`, so the cache-hit exception is counted but was 0).
- **Repair cycle 3 additions, not tested:** a real main-thread stall in a real page (B30 simulates it with a busy-wait inside a harness task); CDP resource types in other Chromium versions or other browsers.
- **Repair cycle 2 additions, not tested:** a real slow image decoder (the stall is simulated by wrapping `createImageBitmap` in the harness page); non-Chromium worker auditing.
- **Repair cycle 1 additions, not tested:** rendering across the antimeridian (rejected by design); hardware-GPU context loss (SwiftShader `WEBGL_lose_context` only); real slow networks (the stall is simulated by a held loopback response).

## Next authorised step

An independent, non-author **delta re-review** of `review/multi-2d-maplibre-adapter-v1` (repair cycle 6, code `223932cc`, on top of repair cycle 5, code `fd81415e`) against the fifth review's findings and its withhold-A control requirement, this handoff and the spec. Part 2 stays QUEUED until an integrator approves a separate packet.

NOT merged to integration/2d-sim or main.
