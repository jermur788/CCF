# Multi #730 / #784 — standard-agent Year0 2D viewer: parent implementation result

**19 November 2026 UTC · Claim [#784](https://github.com/jermur788/CCF/issues/784) · Manager [#730](https://github.com/jermur788/CCF/issues/730)**
**Status: ACTUAL LOCAL CANDIDATE IMPLEMENTED AND OFFLINE-QUALIFIED · INDEPENDENT-MODEL Q NOT RUN (OWNER-ACCEPTED GAP) · NOT RELEASED, NOT MERGED, NOT PUBLICLY PUSHED.**

> Local-only, non-public, offline Year-0 Sitka 2D stand inspector. Not Scenario One, not #770, not 3D, not Unity. No source, fixture or package was published to GitHub. L4 .NET **FAIL38 remains unwaived**.

---

## 0. Verdict

| Stage | Result |
|---|---|
| B protected one-file pre/post hash | **PASS — identical** (`c0e84a1d…536e`), partial one-file coverage |
| F single fixture copy | **PASS** — 32,567 B, `bcaaa5c3…57c4`, mode `0444` |
| C Core (GPT-6.1 Sol) | **PASS** — frozen after two independent review rounds and two repair cycles; 113/113 tests |
| A Viewer (Claude Sonnet 5.5) | **PASS** — committed, 78/78 tests with the real package, 22/22 mutants killed |
| Package assembly | **PASS** — 127,831 B, `0b69cc1d…67b8`, manifest + assembler audit |
| P-02 semantic audit | **PASS (parent-run)** |
| Q offline qualification | **PASS (parent-run; independent-model Q NOT RUN)** |
| B posthash | **PASS — matches prehash** |
| Release / main / public source | **NOT DONE — out of scope** |

The candidate is ready for the Manager's **APPROVED / NEEDS CHANGES / REJECTED** decision on qualification. Implementation, qualification, integration and release remain distinct.

---

## 1. Grants actually used

All grants were issued in-session by the owner/Manager after exact scopes were presented; no agent self-issued one.

| Grant | Scope | Evidence of use |
|---|---|---|
| **B** | no-follow stat + SHA-256 of the single #687 `Engine.cs`, pre and post; no content retained | hash only, receipts below |
| **F** | one no-network copy of blob `8bf81a43…` at 32,567 B, mode `0444` | only the pinned fixture |
| **C** | GPT-6.1 Sol, Core lane, exact allowlist | Core commits |
| **Review** | Claude Opus 5.5, read-only, Core review + deltas | three review rounds |
| **A** | Claude Sonnet 5.5, Viewer lane, exact allowlist, after Core review passes | Viewer commit |
| **Q + B posthash** | distinct nonauthor offline qualification and posthash | see §7/§8 |

**Independence disclosure:** the Claude Opus 5.5 Q session did not run — the provider returned its 5-hour limit, and the T3 delegation tooling became unavailable during the stage. After the owner explicitly accepted that gap, the parent (a nonauthor of both Core and Viewer, but also the orchestrator) executed Q. This is **not** represented as an independent-model Q.

---

## 2. Actual actors

| Role | Actual model / provider | Session |
|---|---|---|
| Parent / orchestrator | DeepSeek V4.1 Flash, `opencode-go/deepseek-v4.1-flash` (high), OpenCode v2.3.2 in T3 Code | `84918da5-4168-461f-8852-89d68cdb1efd` |
| Core C | GPT-6.1 Sol, `codex_c75d8114…`, reasoning high | T3 Codex child `…delegate-task%3A6b6940ab…` (+ repair children) |
| Core review | Claude Opus 5.5, `claudeAgent` | T3 child `…delegate-task%3A96bdab5b…` and two delta reviews |
| Viewer A | Claude Sonnet 5.5, `claudeAgent` then `claudeAgent_claude_b_pro` after the first account hit its limit | T3 children `…ad0f63e4…`, `…6af7fd1b…` |
| Q | parent-executed, owner-accepted | this result |
| GPT-6.2 | **never selected, offered or used** | — |

---

## 3. Physical and storage compliance

- Core lane `…/ccf-h0a-core`: branch `feature/multi-h0a-core-year0-admission-20261118-v1`, HEAD **`c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1`**, remote-less, only the untracked read-only fixture.
- Viewer lane `…/ccf-h0a-viewer`: branch `feature/multi-h0a-a-year0-viewer-20261118-v1`, HEAD **`f3537fe37305199a55a06882e32a81487c53e540`**, clean, remote-less.
- **No new worktree, clone, branch or package cache was created**; the existing Files ext4 lanes were used in place. Packaging ran from the Core lane with the Viewer files copied in temporarily and removed afterwards; the assembler wrote only to `R/artifacts/20261119T003202Z/`.
- All build/test/temp output under `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/artifacts/tmp` or the lane.
- Ops board and `lanes.json` were not modified; #770/Scenario One untouched; #710/#725 never accessed.

---

## 4. Core

**Frozen candidate `c4124b63`** (`c29b0035` + review repairs + prototype hardening).

Committed allowlist under `Tools/MultiRackSitkaYear0Viewer/`: `core/year0-core.js`, `core/CONTRACT.md`, `build/assemble-year0-viewer.py`, `tests/core/**` (9 files). Fixture read-only and untracked to preserve `0444`.

- `core/CONTRACT.md` byte-identical to the pinned §2 artifact `1d2b51b1…f447d5`; normative annexes (§1.3 key sets, §5 packaging, §6 tests) supplied separately as `1d7f2139…7e06`.
- Admission S0–S12, digest-before-parse, f32 bit codec, Genesis-only typed observation, full F3 key, one recursively frozen `CcfYear0Core` global with five members, zero-argument `openGenesis`, fixed private pin.
- **113/113 Node tests**, 11 embedded packager checks; assembler `--validate-only` exit 0.

Review history:
1. **PASS WITH FINDINGS** — 0 blocker, 1 major (call-time built-in lookup let a later script alter the observation), 3 minor, 6 nit.
2. Delta review **FAIL** — the repair still allowed species/`Object.prototype`/iterator tampering.
3. Final delta review **PASS** — 0 blockers/majors; 15 new attack families all failed; observation and token identical.

Custodian resolutions recorded: contract governs admission; `CoordinateOutOfDomain` is **S9**, `StageUnsupported` is **S11**; age is any positive safe integer (fixture verified all 30); stage tokens `"S0"`…`"S12"` confirmed. All in `artifacts/contract/RESOLUTION-MEMO-CORE.md` and `DISPOSITION-REVIEW-CORE.md`.

---

## 5. Viewer

**Commit `f3537fe3`**, 14 allowlisted paths:

- `viewer/shell.template.html` (1,070 B), `viewer/viewer.css` (4,769 B), `viewer/viewer-map.js` (20,141 B), `viewer/viewer-ui.js` (24,812 B), `viewer/viewer-boot.js` (6,806 B)
- `tests/viewer/**` (9 files, ~71 KB)

Implementation: IIFE modules with one A-owned namespace; boot checks contract/version and branches only on the result's **own** `kind`; 30 m north-up Canvas with crown radii, stems, fit/pan/zoom, DPR; accessible 100-ID listbox, chooser, inspector with exact key and explicit unavailable states; refusal renders only the banner and a bounded notice; no network/storage/cookie/CDN/fonts/save.

- `node --test tests/viewer` → **74 pass, 4 skipped, 0 fail** without a package.
- With `CCF_VIEWER_PACKAGE_HTML` at the real package → **78/78 pass, 0 skipped**.
- **22/22 intended mutants killed** (writer evidence `viewer-mutants.txt`).

---

## 6. Package

Built through the Core lane's own assembler, no guard bypass:

| Item | Value |
|---|---|
| Path | `…/artifacts/20261119T003202Z/year0-viewer.html` |
| Size / SHA-256 | **127,831 B** / **`0b69cc1dcf07687c1e56a204797bcd8f32425d1c6a249e4efa09fadce72a67b8`** |
| Manifest | Core `c4124b63`, Viewer `f3537fe3`, ordered input hashes, fixture pin, assembler `e6f2db48…1e1a`, Python 3.12.3, assembly UTC |
| P-02 | **REQUIRED**, and performed (below) |

P-02 static audit of the actual emitted bytes: 5 scripts (`fixture`, `core`, `map`, `ui`, `boot`) in manifest order with exact hashes; CSP meta first in `<head>` and matching every inline script and the style body; one charset; no placeholders, external references, inline handlers or `style=`; no `CcfYear0CoreVerification` / `CCF_H0A_VERIFICATION_HARNESS` / `admitBytes`; exactly one Core global definition; disclosure lines present.

---

## 7. Offline qualification (parent-run, owner-accepted)

Environment: Chromium **156.0.8078.79**, Firefox **159.0**, true offline via `unshare -rn` — `ip addr/route` inside shows only `lo` DOWN and no route.

| Check | Result |
|---|---|
| Chromium 1280×720 | rendered; screenshot 268,935 B |
| Chromium 1920×1080 | rendered; screenshot 573,583 B |
| Chromium 360×800 | single-column stack; screenshot 118,436 B |
| Firefox 159 1280×720 | rendered; screenshot 294,682 B |
| CSP | **0** Content-Security-Policy/refused messages in Chromium stderr; scripts executed |
| Content | disclosure banner, north-up plan with crowns/stems/scale bar, all 100 IDs, inspector, controls; P30-001 and P30-053 present |
| Tamper | same-length base64 corruption → **“Admission refused, so no stand view is shown. Code: DigestMismatch · Stage: S3 · Detail: package.fixture”**, no map/list |
| Network | no route existed; zero requests possible |

Evidence: `…/Verification/CCF-H0A-Year0Viewer-20261118/784-standard-20261118/q-parent/` (report, audit JSON, hashes, screenshots, logs).

**Not verified:** real screen reader; 200 % text zoom; automated keyboard traversal in a real browser (keyboard logic is covered by the Node suite); DevTools request log.

---

## 8. B pre/post hash (partial one-file coverage)

| Stage | sha256 | inode | size |
|---|---|---|---|
| Pre | `c0e84a1de084df8fa12734eba554395e2f2bb71937ef5d94c480a894f24b536e` | 11023779 | 14,167 |
| Post | `c0e84a1de084df8fa12734eba554395e2f2bb71937ef5d94c480a894f24b536e` | 11023779 | 14,167 |

Method both times: `open(O_RDONLY|O_NOFOLLOW)` → `fstat` → SHA-256. No content retained, no copy. Coverage is this one file only; #710/#725 were not accessed and no general protected access is claimed.

---

## 9. What is not done / limitations

- **Independent-model Q** was not run; the parent-run qualification is accepted by the owner as the qualification evidence, with that gap recorded.
- No package release, distribution, external demo, main merge or public source push. Viewer/Core remain local-only in the remote-less H0 store.
- No Unity, Windows, Host, save, Year1–10, marking, Work Plan or Scenario One interaction.
- L4 .NET **FAIL38 unwaived**.
- Writer deviation recorded: the Viewer writer used `git -c core.hooksPath=/dev/null` once; the lane only has a `pre-push` hook, so nothing was bypassed, and it was unnecessary.

---

## 10. Manager decision requested

**APPROVED / NEEDS CHANGES / REJECTED** for the qualified candidate:

- Core `c4124b63`, Viewer `f3537fe3`, package `0b69cc1d…67b8`;
- nine evidence roots under `…/Verification/CCF-H0A-Year0Viewer-20261118/784-standard-20261118/`;
- risk acceptance: none needed beyond the owner's accepted Q-independence gap; no R1–R4 sandbox question applies to this route;
- nothing here is a release, merge, L4 waiver or public publication.
