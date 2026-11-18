# Multi #730 / #784 — Year0 2D Viewer confined-CLI orchestration: **S0 admission result — STOPPED**

**Date:** 18 November 2026 UTC · **Claim:** [#784](https://github.com/jermur788/CCF/issues/784) · **Manager:** #730
**Product:** standalone, local-only, non-public **2D** Sitka Year0 read-only stand inspector. **Not** Scenario One; **#770 untouched**.
**Status:** **S0 COMPLETE (ADMISSION ONLY) · S1 NOT STARTED · SOURCE / PROTECTED / FIXTURE / LEASE GATES CLOSED · TWO BLOCKING DECISIONS OUTSTANDING.**

> This is an admission record. It contains no source code, no fixture bytes and no protected bytes, and it grants nothing. Every unverified item is labelled.

---

## 0. Verdict at a glance

| Stage | Result | Basis |
|---|---|---|
| **S0** parent registration + physical readiness refresh | **COMPLETE** | real parent identity; fresh `origin/main`; board/lanes read-only readback; lane Git state; narrow synthetic re-check |
| **Owner Option B R1–R4 acceptance** | **ABSENT** | no affirmative owner statement in chat or GitHub; the dispatch instruction restates the requirement, it does not accept |
| **Intended confined-CLI models available** | **NO** | all three intended model routes fail under the frozen launcher; only `opencode-go` works, and it does not offer them |
| **S1** Manager-signed B/F/C | **NOT STARTED** | blocked by both items above; parent may not self-sign G2 |
| **S2 / S3** Core / Viewer source | **NOT STARTED** | gates closed; no lane touched |
| **S4** independent Q + B posthash | **NOT STARTED** | no candidate exists |
| **L4 .NET** | **FAIL38 UNWAIVED** | disclosed, unchanged |

Two **independent** blockers exist. Either one alone holds the source stage:
1. the owner has not signed the precise Option B limited-risk acceptance; and
2. the intended Core/Viewer models cannot actually be run as confined CLI processes here.

---

## 1. S0 — actual parent registration

| Field | Value |
|---|---|
| Provider / model | `opencode-go` / **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`, variant `high`) |
| Harness | OpenCode v2.3.2 inside T3 Code |
| Parent thread | `84918da5-4168-461f-8852-89d68cdb1efd` |
| Host | `jer-laptop`, Linux Mint 22.3, kernel `7.0.0-42-generic`, x86_64, 4 CPU, 15 GiB RAM, unprivileged uid 1000 |
| Parent capability mode | `full-access`, default interaction — **disclosed**; the parent is docs-only by instruction, not by confinement |
| `origin/main` at start | `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` — *docs(coordination): add larger gated Sen-1 Compact30 vertical slice umbrella and test-only lane* |
| Result branch | `docs/multi-784-year0-viewer-admission-20261118-v1` |
| Result path | `Docs/Handoffs/MultiRack-Simulation-20261116/784-orchestration/Multi-784-DEEPSEEK-YEAR0-2D-VIEWER-IMPLEMENTATION-PARENT-RESULT-20261118.md` |
| `[RUNNING — ADMISSION ONLY]` | posted before the work: [comment 6992533366](https://github.com/jermur788/CCF/issues/784#issuecomment-6992533366) |

**Not registered, because nothing is authorised for them to do:** C Core writer, A Viewer writer, B one-file custodian, F fixture custodian, Q reviewer. The packet's proposed identities are recorded in §6 as *candidates*, not live sessions.

**#770 / Scenario One:** inspected only as `OPEN`; no read, write, comment or change was made.

---

## 2. S0 — physical readiness refresh (read-only; no Ops mutation)

Read by this parent acting as physical custodian for admission only. No board, lane, lease, hook or repository config was modified.

| Check | Observed |
|---|---|
| Board | `/media/jer/ZX20/CCF-Ops/board/active.md`, sha256 `4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994` — **byte-identical to the #755 Phase1 post-hash**; unchanged since 2026-11-18T17:28 |
| `lanes.json` | sha256 `c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e` — matches #755 post-hash |
| H0 rows | `CCF-755-H0A-CORE-LANE` and `CCF-755-H0A-VIEWER-LANE` present at lines 120–121, state `READY`, `RESERVED BY Manager #730 / NOT RUNNING / NO ACTIVE WRITER (writer UNASSIGNED)` |
| Other H0A rows | none (`grep -c H0A` = 2) — **no second writer, no collision** |
| Core lane | `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/ccf-h0a-core`, branch `feature/multi-h0a-core-year0-admission-20261118-v1`, HEAD `34185b8b71ba280c89091290345e537ef107bdf4`, **clean**, **zero remotes** |
| Viewer lane | `…/ccf-h0a-viewer`, branch `feature/multi-h0a-a-year0-viewer-20261118-v1`, same HEAD, **clean**, **zero remotes** |
| Isolated store | both lanes share `…/git/CCF-H0A-Year0Viewer.git` only; `push.default=nothing`; hookspath pinned to the local `hooks/` |
| Lane contents | sparse `Tools/MultiRackSitkaYear0Viewer` cone; on disk only `AGENTS.md`, `CLAUDE.md`, `.gitignore` (`.git` pointer) — **no source, as intended** |
| Protected #687 single file | **metadata only, contents never read**: `Engine.cs`, inode `11023779`, size `14167`, mode `664`, owner `jer`, mtime `2026-11-17T19:59:44Z` |
| #710 / #725 | **not accessed** |
| #639 fixture blob | `8bf81a43…26c` present in `/media/jer/Files/CCF/Repository.git`; `cat-file -t` = `blob`, `cat-file -s` = **32,567 bytes**. Not extracted, not copied, not opened, not hashed — those are F-grant actions |
| Unity machine lock | **not held**; latest history event is `release` 2026-11-15T22:44:25Z; `guard` inode unheld |
| Live H0A writer / Unity / dotnet process | **none** |
| Disk | Files ext4 **27 G free** (95 % used); ZX20 exFAT **45 G free** (91 % used) |

**Verdict: physical readiness PASS for admission.** The two lanes remain empty, clean, remote-less and uncontested. This is not a lease: no writer exists and none may start from this record.

---

## 3. S0 — confinement artefact comparison and narrow synthetic re-check

### 3.1 Artefact hashes match #779 exactly

| Artefact | sha256 | Match #779 |
|---|---|---|
| `launcher-v1.9.sh` | `64056e1fe167420070ba52953eb87ea8b2c31be0c4d8526b7dccef48906665c6` | **yes** |
| `opencode.policy.json` | `a8a2a5df9f5d568fb1436e62379750eb744333a48d68a22a92be7e4dc7de7f3d` | **yes** |

The packet's condition *"record whether policy/launcher v1.9 exact hashes match #779's documented policy and launcher, or STOP"* is satisfied: they match. No launcher or policy edit was made.

### 3.2 Narrow synthetic witness (not a new security audit)

Fresh synthetic lane `/tmp/opencode/s0-784/lane`, synthetic outside sentinel `/tmp/opencode/s0-784/outside/sentinel.txt`, run through the unmodified frozen launcher. **No protected, fixture or project path was used.**

| # | Operation | Result |
|---|---|---|
| 1 | read outside sentinel | **BLOCKED** — `Permission denied: external_directory` |
| 2 | write outside sentinel path | **BLOCKED** — `Permission denied: external_directory` |
| 3 | read lane file (control) | **ALLOWED** |
| 4 | write lane file (control) | **ALLOWED** |
| 5 | catalogue reported by the confined model | `read, write, edit, glob, grep, skill, opencode_worktree_remove` |

Interpretation, stated narrowly:
- The whole-process boundary still behaves as recorded in #779 for file reads and writes.
- Residual **R2 reproduces**: `opencode_worktree_remove` is still present in the catalogue despite deny. **Not exercised**; destructive behaviour remains unqualified, as documented.
- R1, R3 and R4 are unchanged and unmitigated: this run neither fixed nor re-audited them. Nothing here reduces the owner's decision.

---

## 4. The model/host blocker (precise, reproducible)

Every attempt below was made **inside** the frozen launcher and policy, in a synthetic lane, and is archived in §7.

| Model route | Intended role | Actual result |
|---|---|---|
| `openai/gpt-6.1-sol` | Core C | **HTTP 401** — provider credential rejected |
| `anthropic/claude-sonnet-5-5` | Viewer A | **`invalid x-api-key`** |
| `opencode/gpt-6.1-sol` | Core C (console route) | **`Reconnect OpenCode Console to continue.`** |
| `opencode/claude-sonnet-5-5` | Viewer A (console route) | **`Reconnect OpenCode Console to continue.`** |
| `opencode-go/deepseek-v4.1-flash` | **positive control only** | **works**; confinement enforced as in §3.2 |

`opencode models` lists 179 entries. The **only** provider with a working credential is `opencode-go`, whose catalogue contains no `gpt-6.1-sol`, no `claude-sonnet-5-5`, no `claude-opus-5-5`. So the exact intended source models are **not available as confined CLI sessions** in this environment today.

Host-side alternatives exist but are **not** the frozen entrypoint and were **not used**:
- `codex` CLI v0.170.0 is logged in (`Logged in using ChatGPT`) and could carry GPT-6.1 Sol.
- Claude Code v2.1.327 has stored credentials.

Both would need a **new whole-process launcher and policy**, plus independent witness requalification, and would add a new credential-mount residual. That is a Manager/owner decision, not a parent decision.

**No substitution was made. GPT-6.2 was never selected, offered as fallback, or run.** The packet's rule — *"If intended model cannot be run in a confined CLI, stop for exact model/host decision; never use unrestricted T3, secretly substitute model or GPT-6.2"* — is applied literally.

---

## 5. What was deliberately **not** done

- No owner or residual-risk acceptance inferred, restated as given, or self-signed.
- No G2 B/F/C/A/Q grant created or implied; the parent cannot grant its own privileges or sign for #730.
- **No protected source byte read.** #687 `Engine.cs` was touched as metadata only; #710/#725 untouched.
- **No fixture byte read, copied, extracted or hashed**; no 0444 destination created.
- **No lane write**, no branch, no commit, no stash, no clean, no reset in either H0-A worktree.
- No physical lease mutation, no Ops board/lane edit, no Unity lock.
- No Core, no Viewer, no HTML/JS, no browser qualification, no Q, no B posthash.
- No public publication of app source; this branch is documentation only.
- No interaction with #770 or Scenario One.
- L4 .NET **FAIL38 remains unwaived**; no release, Host, Unity, Windows or main integration claimed.

---

## 6. Decisions required before this can continue

### A. Owner — explicit Option B limited-risk acceptance (the risk-policy choice only)

The owner must affirmatively state the acceptance in their own message. The packet's required substance:

> For Multi #730's single local, non-public Year0 2D viewer prototype only, I explicitly accept the four currently documented residuals: R1 model-provider token inside the process; R2 denied-but-callable worktree-remove tool with destructive behaviour unqualified; R3 symlink directory-policy escape (limited by mounted filesystem); R4 OS-level outbound network and abstract sockets without endpoint allowlist. I authorise the Manager to consider the pinned, independently rechecked **whole-process confined CLI** environment, not unrestricted T3-delegated writers. This is a limited risk decision, **not** an automatic physical lease, protected read, fixture grant, source grant, release, public push or L4 waiver. Stop and seek re-approval on materially different conditions.

This acceptance does **not** authorise arbitrary protected reads, the fixture, a lease or any source edit by itself.

### B. Manager #730 — separate actor-specific grants, after a signed current lease

- **B** — independent nonwriter, no-follow regular-file read of the single #687 `Engine.cs`, two SHA-256 receipts (pre before C, post after Q), partial one-file coverage only, holder keeps sole write rights.
- **F** — one read-only, no-network, byte-exact copy of the approved derived #639 fixture to a 0444 destination, verified at 32,567 B and SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`.
- **C** — Core writer, exact frozen per-file allowlist, single-writer physical lease in `R/ccf-h0a-core`.
- Later **A** — Viewer writer only after independent Core review + freeze + custodian-controlled sharing, in `R/ccf-h0a-viewer`.
- Then **Q** — distinct nonauthor offline qualification lane.

### C. Model/host decision (choose one, explicitly)

1. **Restore the intended providers inside the frozen launcher** — make `openai/gpt-6.1-sol` and `anthropic/claude-sonnet-5-5` work under launcher v1.9 / policy `a8a2a5df…` (owner/provider action: re-authenticate OpenAI, Anthropic and OpenCode Console). No launcher change, no requalification. **Preferred — it keeps the already-witnessed boundary.**
2. **Authorise and requalify a different whole-process launcher** for `codex` and/or `claude` CLI, with a new policy, independent witness rounds, and explicit handling of their credential mounts. Slower; new residual surface must be named.
3. **Manager-approved, non-secret model substitution** with the substitute named in advance and its confined availability demonstrated. Not permitted silently; not GPT-6.2 under any wording.

Until **A + B + C** all exist in writing, the correct state is: lanes reserved, no writer, no source.

---

## 7. Durable evidence

Root: `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/s0-784-20261118/`

| File | Purpose |
|---|---|
| `physical-readiness.txt` | board/lane hashes, lane Git state, protected **metadata**, fixture blob type/size, lock, disk, main HEAD |
| `dsv4-run.txt` | positive control through the frozen launcher (outside read/write blocked, lane controls allowed) |
| `sol-run.txt` | `openai/gpt-6.1-sol` — 401 |
| `sonnet-run.txt` | `anthropic/claude-sonnet-5-5` — invalid key |
| `zen-sol-run.txt` | `opencode/gpt-6.1-sol` — console reconnect |
| `zen-sonnet-run.txt` | `opencode/claude-sonnet-5-5` — console reconnect |
| `SHA256SUMS` | digests of the above |

Frozen confinement artefacts remain at `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/g1b-779-20261118/`.

---

## 8. Next safe action

1. Owner sends the Option B affirmation (§6A).
2. Owner/Manager resolves the model route (§6C) — option 1 is the least risky and requires no new witness round.
3. Manager #730 signs B, F and C separately after a current signed physical lease; B pre-hash precedes any Core write.
4. Parent then launches the Core writer **only** as a confined whole-process CLI in `R/ccf-h0a-core`, with the exact allowlist, and proceeds sequentially to independent Core freeze, Viewer A, Q and B posthash.

**No claim of implementation, qualification, integration, release or acceptance is made here.**
