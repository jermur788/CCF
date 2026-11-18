# Multi #730 / #790 — confined CLI model-auth and external-storage recovery: **RESULT**

**Date:** 18 November 2026 UTC · **Claim:** [#790](https://github.com/jermur788/CCF/issues/790) · **Manager:** #730 · **Resumes:** #784 (admission only)
**Status:** **STORAGE BOUNDARY PASS · MODEL AUTH BLOCKED FOR BOTH INTENDED MODELS · OWNER LOGIN ACTION REQUIRED · NO SOURCE.**

> No credential value, token, cookie or auth-file content appears in this document. Diagnostics are status-only and redacted. Nothing here accepts R1–R4, grants source access, or changes any Ops/physical record.

---

## 0. Verdict

| Item | Result |
|---|---|
| Parent registered | **PASS** — DeepSeek V4.1 Flash, OpenCode v2.3.2 in T3 Code, thread `84918da5-4168-461f-8852-89d68cdb1efd`, `jer-laptop` |
| Frozen launcher/policy unchanged | **PASS** — `64056e1f…65c6` / `a8a2a5df…7f3d` before and after all tests |
| **GPT-6.1 Sol confined response** | **BLOCKED** — `openai/gpt-6.1-sol` → **HTTP 401** |
| **Claude Sonnet 5.5 confined response** | **BLOCKED** — `anthropic/claude-sonnet-5-5` → **`invalid x-api-key`** |
| Console alternative route | **BLOCKED** — `opencode/claude-sonnet-5-5` → **`Reconnect OpenCode Console to continue.`** |
| Synthetic confinement control | **PASS** — outside read **and** write denied, lane read/write allowed (DeepSeek control only) |
| External-drive storage boundary | **PASS with one bounded note** — no new root worktree/clone/large cache; all #790 artefacts on Files ext4 |
| Source / fixture / protected / Ops / #770 | **untouched** |
| L4 .NET | **FAIL38 UNWAIVED** |

Neither intended model can be declared PASS by substituting the DeepSeek control. Per the packet, that control is **not** permission to replace either writer.

---

## 1. Stage 0 — registration and read-only inspection

| Field | Value |
|---|---|
| Parent | `opencode-go` / **DeepSeek V4.1 Flash** (`deepseek-v4.1-flash`, variant `high`) |
| Harness / host | OpenCode v2.3.2 in T3 Code; `jer-laptop`, Linux `7.0.0-42-generic`, 4 CPU, 15 GiB |
| Thread | `84918da5-4168-461f-8852-89d68cdb1efd` |
| `origin/main` at start | `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` |
| `[RUNNING — AUTH/STORAGE RECOVERY ONLY]` | [comment 6992888980](https://github.com/jermur788/CCF/issues/790#issuecomment-6992888980) |
| Result branch | `docs/multi-730-790-auth-storage-recovery-20261118-v1` (docs-only, created via GitHub API — no local worktree) |

**Ops read-only readback (unchanged, no mutation):**

| Artefact | sha256 |
|---|---|
| `board/active.md` | `4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994` |
| `lanes.json` | `c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e` |

Both match the #755 Phase1 post-state exactly. H0-A Core and Viewer worktrees are still clean at `34185b8b71ba280c89091290345e537ef107bdf4`, leased to nobody. `#770` inspected as `OPEN` only.

---

## 2. Stage 1 — provider diagnostics (status only, no secrets)

`opencode auth list` reports five stored integrations. Structure/expiry metadata was read without printing credential material:

| Provider route | Credential type | Expiry state | Observed confined result | Classification |
|---|---|---|---|---|
| `openai` (GPT-6.1 Sol) | OAuth | **EXPIRED 2026-11-13T11:31Z** (5 days before this run) | HTTP **401** | Rejected/expired credential (`401`), not a missing model |
| `anthropic` (Claude Sonnet 5.5) | API key | no expiry field | **`invalid x-api-key`** | Rejected static key |
| `opencode` Console (both models) | API key | no expiry field | **`Reconnect OpenCode Console to continue.`** | Console key rejected/revoked |
| `opencode-go` | API key | none | **works** | Positive control only — not an authorised substitute |

Model identifiers themselves resolve: `opencode models` lists `openai/gpt-6.1-sol` and `anthropic/claude-sonnet-5-5` among 179 entries. The failures occur at the authentication stage, so this is **not** a "model not available" or entitlement diagnosis. Entitlement cannot be distinguished from credential rejection while no valid credential exists.

`codex login status` reports `Logged in using ChatGPT`, and a Claude Code credential file exists. These are **host** credentials for **different CLIs**; they were not used to prompt a model, because doing so inside this unrestricted parent would breach the packet's rule against testing with the owner's full host credentials.

### 2.1 Critical secondary finding — the frozen launcher reads its own credential copy

The launcher binds a **private data directory** (`/tmp/opencode/g1b-779/ocdata-frozen`) over `~/.local/share/opencode`. Its `auth.json` is currently **byte-identical** to the host file (same sha256 `266d83c3…e406`, same 2,573 B), so R1's scope is unchanged.

Consequence: **an owner re-login on the host will not reach the confined session.** The private copy stays stale until it is refreshed or the launcher is re-pointed. Both are credential/config mutations and need approval:

- **least change** — after the owner re-authenticates, copy the new host `auth.json` into the existing private data directory (same R1 surface, no new credential class);
- **structural** — launcher v1.10 binding a read-only credential file or a data directory under `/media/jer/Files`, which requires independent requalification.

Neither was done here.

---

## 3. Stage 2 — external-drive storage boundary

| Path | Filesystem | Block | Free at test time | Role |
|---|---|---|---|---|
| `/` and `/tmp` | ext4 (root device) | 4 KiB | **3.6 GiB (99 % used)** | OS + pre-existing scratch; **no new substantial data created** |
| `/media/jer/Files` | ext4 | 4 KiB | **21 GiB (96 % used)** | POSIX scratch/evidence; #790 lane and all outputs placed here |
| `/media/jer/ZX20` | exFAT | 128 KiB | **45 GiB (91 % used)** | large non-sensitive staging only; not used (no POSIX perms/symlinks) |

Compliance actions taken:
- the disposable lane, outside sentinel, logs and all evidence were created under `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/790-20261118/` — verified by `stat` on the files the confined agent wrote;
- **no clone, no new worktree, no copy of any repository**; the registered worktree count is unchanged at 214, and the six root `/tmp/opencode` worktrees are pre-existing, not created by #790;
- no cleanup, move, stash, reset or reclamation of anyone else's data;
- the pre-existing `/tmp/opencode` tree remains 9.5 GiB and was not added to; the frozen launcher data directory remained **27 MiB** before and after all tests, and root free space did not fall.

**Bounded note (not a failure, a proposal):** the frozen launcher's private data/cache directory is hardcoded under `/tmp` (root device). It is small and pre-existing, but it is the one runtime path that still writes to root. The launcher does not mount `/media/jer/Files` or `/media/jer/ZX20` generally — only the lane, which is why the lane was placed on Files. Relocating the data directory requires a launcher change and therefore **bounded requalification**, not an edit made here. The agent's own `/tmp` inside the sandbox is a `tmpfs`, so its transient temp does not consume root disk.

---

## 4. Stage 3 — acceptance test results

All runs used the **unchanged** launcher v1.9 and policy, with a synthetic lane on Files. No protected, fixture or project path was touched.

| Test | Actor | Expected | Observed | Verdict |
|---|---|---|---|---|
| T1 confined response | `openai/gpt-6.1-sol` | `PROBE-OK` | HTTP 401, no model turn | **BLOCKED** |
| T2 confined response | `anthropic/claude-sonnet-5-5` | `PROBE-OK` | `invalid x-api-key`, no model turn | **BLOCKED** |
| T3 console route | `opencode/claude-sonnet-5-5` | `PROBE-OK` | `Reconnect OpenCode Console to continue.` | **BLOCKED** |
| T4 positive control | `opencode-go/deepseek-v4.1-flash` | outside denied, lane allowed | outside read **BLOCKED** `external_directory`; outside write **BLOCKED**; lane read/write **ALLOWED** | **PASS (control)** |
| T5 catalogue | control | no shell/fetch/subagent | `edit, glob, grep, opencode_worktree_remove, read, skill, write` | **known R2 persists** |
| T6 artefact integrity | — | hashes unchanged | launcher `64056e1f…`, policy `a8a2a5df…` after all tests | **PASS** |

Because T1–T3 produced no model turn, **no out-of-lane denial test could be attributed to those models**. The denial evidence belongs to the DeepSeek control only and is labelled as such. `opencode_worktree_remove` remains listed; it was **not called**.

---

## 5. Required owner action (precise, no secrets in chat)

In a normal terminal **outside this agent**, run:

```
opencode auth login
```

and complete one or both of these supported connections:

1. **OpenAI** — choose the OpenAI/ChatGPT route and finish the browser sign-in (**primary route for GPT-6.1 Sol**; the stored OAuth expired 2026-11-13).
2. **Anthropic** — enter a valid Anthropic API key, or the supported Claude subscription method if offered (**primary route for Claude Sonnet 5.5**; the stored key is rejected).
3. **OpenCode Console** — optional single connection that covers both models through the `opencode/*` route (currently `Reconnect OpenCode Console`).

Do **not** paste any key into chat, GitHub, logs or a prompt. The CLI writes the credential to the owner's OpenCode profile.

Then approve **one** of:
- **(a) least change:** refresh the frozen private copy (`/tmp/opencode/g1b-779/ocdata-frozen/auth.json`) from the newly authenticated host profile, keeping the same R1 surface; or
- **(b) structural:** authorise launcher v1.10 with the data directory on `/media/jer/Files` plus independent requalification.

After that, the two-model test can be re-run unchanged in a few minutes.

### Decision table

| Option | Route | Requirement | Assessment |
|---|---|---|---|
| **1 — owner reconnect + private-copy refresh** | frozen launcher, intended providers | owner sign-in + approval (a) | **Recommended**; no new boundary, no witness round |
| 2 — new launcher for Codex/Claude CLI | codex v0.170.0 / Claude Code v2.1.327 | new policy + credential mounts + independent requalification + Manager approval | Slower; adds a new credential class |
| 3 — named substitution | e.g. a confined `opencode-go` model | explicit Manager/owner decision | DeepSeek control is **not** permission; never GPT-6.2 |

---

## 6. What was not done

- No source, fixture byte, protected `#687` content, `#710/#725`, Ops mutation, main change, `#770`/Scenario One interaction.
- No R1–R4 acceptance, no B/F/C/A/Q grant, no writer lease.
- No credential mutation, no `auth login/logout/export`, no shared-config overwrite, no credential mount change, no launcher/policy edit.
- No host-credential prompt through `codex`/`claude` CLI.
- No GPT-6.2 selection or fallback; L4 FAIL38 remains unwaived.

---

## 7. Durable evidence

`/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/790-20261118/`

| File | Purpose |
|---|---|
| `gpt61sol.txt` | T1 redacted output (401) |
| `sonnet55.txt` | T2 redacted output (`invalid x-api-key`) |
| `console-route.txt` | T3 redacted output (console reconnect) |
| `control-deepseek.txt` | T4/T5 positive and negative synthetic controls |
| `storage-receipt.txt` | mounts, free space, worktree count, artefact placement, hashes |
| `SHA256SUMS` | digests of the above |

Frozen artefacts remain in `…/g1b-779-20261118/`.

---

## 8. Next safe action

1. Owner performs the CLI sign-in(s) above and chooses refresh option (a) or (b).
2. Parent re-runs the same two-model synthetic test under the unchanged launcher.
3. If both PASS, #790 is **READY FOR MANAGER RE-ENTRY REVIEW of #784** — still **not** a writer start. Option B signature, physical lease, B pre-hash, F fixture copy, C Core, A Viewer and Q remain separate grants.
4. If either still fails, the return is a single blocking fact plus the next owner-visible step.

**No implementation, qualification, integration, release or acceptance is claimed.**
