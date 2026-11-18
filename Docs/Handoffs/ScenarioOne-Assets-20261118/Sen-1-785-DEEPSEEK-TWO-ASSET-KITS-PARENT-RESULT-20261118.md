# Sen-1 #785 — Compact30 two asset kits, parent result

**Verdict: PARTIAL.**

| Lane | Outcome |
|---|---|
| **#786 H1 / footpath / ground** | **BUILT** — real editable source, textures, exports and 16 renders on Files; parent-verified. **Not** #788-reviewed. |
| **#787 Oak / Beech / shelters** | **BLOCKED pre-write** by the binding owner no-root storage directive. No art exists. |
| **#788 independent review** | **NOT RUN** — the same directive forbids launching the distinct review process. |
| **Production / Unity / rights / owner look** | **STILL HELD.** Nothing here authorises import, publication or acceptance. |

Parent: **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`, variant high), native T3 Code / OpenCode TG Fork, host `jer-laptop`.
Parent claim: [#785 comment](https://github.com/jermur788/CCF/issues/785#issuecomment-6992584088). Parent branch `docs/sen-1-785-two-asset-kits-parent-20261118-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-785-AssetParent-20261118`, base `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`.

---

## 1. What changed the outcome: a later binding owner instruction

The fork was launched under the packet as read at 21:44Z, whose §2 admitted isolated original scratch art on Files. While the fork was already running, the owner imposed a higher-authority storage gate:

- **[Owner no-root development directive](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-OWNER-NO-ROOT-FILESYSTEM-DEVELOPMENT-STORAGE-DIRECTIVE-20261118.md)**, recorded in the packet header. Owner words: *"I dont want anymore dev on the root hard drive it is almost full"* (22:02Z).
- Commit `6d0e9a30` by `jermur788`, 2026-11-18T22:05:34Z: *"docs(sen-1): impose owner's no-root development and Files capacity gate on active asset TG Fork."*

The gate requires every worktree, source, export, temp, cache, render, autosave, program log and verification artefact to be on a genuinely separate non-root drive, keeps a **10 GiB free floor on Files**, and states:

> "If T3/OpenCode/Claude/Codex intrinsic logs still write to root and cannot safely be redirected, **STOP new development processes and report an honest BLOCKED state**."

**Compliance timeline, stated plainly.** The parent read the 21:38 packet revision, not the 22:05 amendment, and only saw the new header when #787 returned its block at ~22:50Z. #786's continuation therefore ran 22:35–22:45Z in ignorance of the gate, and its own handoff correctly refuses a zero-root-write claim. **After reading the directive the parent launched no further development process** — no #788, no #787 restart, no new Blender run. No live process was killed, no worktree cleaned or reverted, nothing deleted.

This is a **storage placement gate**. It grants no art, rights, Unity, spec or signature authority, and it does not accept or reject the existing H kit.

## 2. Physical storage evidence (required by the directive)

Measured `2026-11-18T22:48:30Z`; full command output in `_Parent/STORAGE-SNAPSHOT-HANDOFF.txt`.

| Item | Actual |
|---|---|
| Root | `/dev/sda2` ext4, UUID `d393b49a-24aa-4f88-b8c4-5121b96c240d`, `rw`, mounted `/` |
| Files | `/dev/sdb4` ext4, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, `rw`, mounted `/media/jer/Files` — **different physical device from root** |
| ZX20 | `/dev/sdd2` exFAT — never used, never selected as fallback |
| Root free | `10,688,335,872` B, 96% used, inode use 39% |
| Files free | `22,267,011,072` B, 96% used, inode use 9% — **12.3 GiB above the 10 GiB floor** |
| `/tmp` backing | `/dev/sda2` — **root-backed, therefore disallowed** |
| `/home/jer` backing | `/dev/sda2` — **root-backed, therefore disallowed** |

**Root-backed harness internals, measured, not relocatable safely:** `/home/jer/.t3/userdata` **5.5 G**, `/home/jer/.claude` **1.3 G**, `/tmp/claude-1000` **2.6 G**. These hold provider state and credentials. The directive forbids moving global credentials/HOME to pass a check, so the stop clause applies rather than a workaround.

All art and evidence produced by this fork is on Files:

| Subtree | Bytes |
|---|---:|
| `.../Sen-1-785-AssetKit-20261118/H1Path` | 39,668,768 |
| `.../Sen-1-785-AssetKit-20261118/JuvenilesProtection` | 81,332 |
| `.../Sen-1-785-AssetKit-20261118/_Parent` | 5,840 |
| `.../Sen-1-785-AssetKit-20261118/_Toolenv` (numpy for Blender exporters) | 64,818,731 |

`CCF-Primary` was never written: it still reports exactly **7** pre-existing dirty/untracked entries. No `CCF-Ops`, `lanes.json`, board, `unity-lock`, device-pin or Unity-lane action was taken anywhere in this fork. The stale ZX20 mount-identity defect was recorded and left unrepaired.

## 3. #786 — H1 entrance, footpath and ground kit: BUILT and parent-verified

Actor **gpt-6.1-sol / codex / T3 Code**, checkout `/media/jer/Files/CCF/Operations/CCF-786-EntrancePath-20261118`, branch `art/sen-1-786-h1-path-offline-kit-20261118-v1` @ **`58296564c96c20709c663522d68b45afb129cf2f`**, scratch `.../H1Path`, footprint 39.7 MB.
Receipts: [RUNNING](https://github.com/jermur788/CCF/issues/786#issuecomment-6992767008) · [resumption](https://github.com/jermur788/CCF/issues/786#issuecomment-6993656845) · [published result](https://github.com/jermur788/CCF/issues/786#issuecomment-6993806775).

**One tracked file only**, no binaries in Git: [`Sen-1-786-H1-ENTRANCE-FOOTPATH-ORIGINAL-ASSET-BUILD-RESULT-20261118.md`](https://github.com/jermur788/CCF/blob/art/sen-1-786-h1-path-offline-kit-20261118-v1/Docs/Handoffs/ScenarioOne-Assets-20261118/Sen-1-786-H1-ENTRANCE-FOOTPATH-ORIGINAL-ASSET-BUILD-RESULT-20261118.md).

| Artefact (under `H1Path/`) | Bytes | SHA-256 |
|---|---:|---|
| `blend/H1Path_Ground_v1.blend` | 938,040 | `0ec555544abd9cbd20b984e00e9a4aba02452762dafc37f95434018ec4e4b7c5` |
| `export/H1Path_Ground_v1.glb` | 277,112 | `5af65119d08bb5a1db4e9da37743850f39fee8ce11912afe33945e4ff664a56c` |
| `export/H1Path_Ground_v1.fbx` | 294,508 | `46fba9593fe85aa882e4408c6281de50813d70ad8ea44abd5fb92f86b26b29d7` |
| `textures/CompactGround_H1_Path_BaseDetailMask_2048.png` | 275,589 | `427d1f570db92660b884125c0f32a9c59bba5eef5ad20d8e9cd4331377f98356` |
| `textures/CompactGround_Detail_512.png` | 8,292 | `23954485e44ebbb3f52d3bbf144fb37b71d045b15839f1cee5719a8f2f585dd5` |
| `verify/SHA256SUMS` | 10,349 | `92f2d64bcb71de486d055ba2d258ac9ef3a18ed755ee709c6322bad4dda052d8` |
| `verify/RESULT.json` | 12,557 | `169700ea094b6dbbfc7ca2f7935513b98ff8cfd4a57797aefea2221d95593c60` |

**Measured content.** One mesh, 4 vertices, 2 triangles, world bounds exactly ±15 m, all y = 0, one UV layer, one opaque material, no collider/modifier/rigid body. C0 keeps all nine anchors and arithmetic-mean tangents: max exact same-Z departure **0.094074074 m** (cap 0.095), minimum sampled radius **0.625361736 m** (analytic bound 0.625284220), length **19.509002164 m**, endpoints `(2.99,−4.75)` and `(−2.10,+13.50)`. Macro core texels with P = 1: 114,372; zero path support outside 0.80 m; H1 inward support zero near/outside the rectangle edge; bilinear support bound 0.799990185 m. Both exports re-import in a fresh Blender process with matching bounds, UV and triangles. `checks.txt` is **38 PASS / 0 FAIL**. Texture PNGs, `recipe.json` and the GLB are byte-deterministic across two clean rebuilds; `.blend` and FBX are **not** byte-identical (geometry/UV/normal numerics identical) and that is disclosed rather than smoothed over. T1 was **not** adopted; A7's ~0.625 m short radius is reported as an open risk, not hidden.

**Parent verification (independent of the child's summary).** Origin ref equals local HEAD; the commit contains exactly one Markdown file; `sha256sum -c verify/SHA256SUMS` exits 0; 16 real PNGs exist including the previously missing spawn, plan and grayscale views; `CCF-Primary` unchanged. The parent visually inspected `Plan_colour_1920x1080.png` (H1 rectangle at x −9…+3 / z −15…−5, mouth at x≈2.99 meeting A0, meander through A7 to A8 — geometrically consistent) and `Spawn_A0_colour_1920x1080.png` (genuine Cycles render, continuous tread, no kerb, road or extra surface).

**Compliance deviation, disclosed.** This kit was completed at 22:45Z, after the 22:02Z gate and before the parent read it. Its Codex/T3 harness logs are root-backed; the lane does not claim zero root writes. The **art itself** is on Files, no binary entered Git, and no further work was done on it after the gate was read. Treat it as a preserved, reviewable scratch candidate whose build predates compliance awareness.

## 4. #787 — Oak, Beech and shelter kit: BLOCKED before any art

Actor attempt 1 (Claude A) died on a quota limit; attempt 2 (Claude B) died on a TLS `UNABLE_TO_VERIFY_LEAF_SIGNATURE` error after writing only a claim; attempt 3 read the 22:02Z directive and **stopped at G0**.

Branch `art/sen-1-787-broadleaf-protection-offline-kit-20261118-v1` @ **`cbbd5cac689e8a96b41dfbac3765eb3234e6c79d`** (one docs-only commit). Claim: [#787 BLOCKED](https://github.com/jermur788/CCF/issues/787#issuecomment-6993837129).
Scratch contains **8 files / 81,332 B**, none an asset, `_g0/SHA256SUMS` verifies clean, and the superseded attempt's claim preserved byte-identical under `_g0/superseded/`. No `.blend`, no export, no texture, no render, no generator was produced; no height, LOD count, panel count or radius was measured, so no J-kit number is claimed here.

**Parent accepts BLOCKED as the correct outcome**, not a failure of the actor. Its reason is exactly the directive's stop clause: Claude Code transcripts and state grow under root-backed `/home/jer/.claude` and `/tmp/claude-1000` with every tool call and cannot be relocated without moving credentials, which the directive forbids. The alternative — proceeding on a root-backed harness and hoping the writes stay small — is the behaviour the owner explicitly ruled out.

## 5. #788 — independent nonauthor review: NOT RUN

No review verdict exists, and the parent readback in §3 is **not** a substitute for it: it was performed by the orchestrating author-side session, not by a distinct nonauthor.

#788 was not launched because a distinct review session is itself a new development process with root-backed harness logs, and the packet's review requires re-running generators, re-deriving the mask and re-reading every export — exactly the heavy work the gate says to stop before, not after. Had it run, it would have covered: frozen `.blend` and export inspection, independent re-derivation of H1/C0 mask limits and texture texels, the 0.10 m shelter envelope on every vertex of every state, species/LOD/2–6 m readability, provenance, and an explicit **scratch-quality** verdict kept separate from a **PRODUCTION STILL HELD** verdict.

## 6. Exact minimal action to unblock the remainder

One owner decision, then the lane can resume into the same checkout and scratch root with no rework of the H kit:

1. **Permit this harness's intrinsic logs on root** while every art/scratch artefact stays on Files, with a stated cap (the measured T3/Claude state is already 9.4 GB on root and grows by megabytes per session, not gigabytes). This unblocks #787 and #788 immediately. **Recommended if the goal is to finish both kits now.**
2. **Relocate the harness** — a session whose config, transcript and state live on a non-root drive — then re-dispatch #787 (and #788) unchanged. Cleanest for the policy, but needs a different launcher, not a workaround inside this one.
3. **Free root space substantively** rather than by moving credentials, then re-affirm or relax the gate.

No option is taken unilaterally here: options 1 and 3 are owner decisions, and option 2 needs a harness change the parent cannot make from inside a running session.

## 7. What this result does and does not establish

- **Does:** one genuine original, editable, measured, reproducible H1/footpath/ground scratch kit exists on Files, with hashes, rebuild commands and 16 real captures; one J lane is honestly blocked with a preserved, hashed G0 dossier; the physical storage state is measured and recorded as the directive requires.
- **Does not:** establish that both kits were built; that any asset was independently reviewed; that any asset is production-ready, rights-cleared, owner-approved or Unity-verified. No D-032 decision, no owner look selection, no T1 adoption, no world/F-2/F-4 signature, no public binary publication, no `Assets/**` or scene change, no Unity/URP/player/collision/save/performance evidence, no Windows build.
- **Year 0 truth preserved:** no juveniles or shelters were placed, simulated or implied. The H kit draws no trees, stumps, road, truck, gate or extra surface.
- Dashboard and Drive synchronization: **NOT VERIFIED** — no board, Ops or mirror write was made or checked.

## 8. Claims, branches and custody

| Actor | Model / harness | Branch @ SHA | Tracked file |
|---|---|---|---|
| #785 parent | deepseek-v4.1-flash / T3 OpenCode | `docs/sen-1-785-two-asset-kits-parent-20261118-v1` | this file |
| #786 author | gpt-6.1-sol / Codex | `art/sen-1-786-h1-path-offline-kit-20261118-v1` @ `58296564c96c20709c663522d68b45afb129cf2f` | H1 result |
| #787 author | Claude Opus 5.5 / Claude Code | `art/sen-1-787-broadleaf-protection-offline-kit-20261118-v1` @ `cbbd5cac689e8a96b41dfbac3765eb3234e6c79d` | J blocked result |
| #788 reviewer | none launched | — | — |

Custody of all art remains with the Files scratch root `.../Sen-1-785-AssetKit-20261118/`, not with Git. The Markdown links above are evidence pointers, **not** downloadable binaries. Rebuild and re-render entry points are in the #786 handoff and `H1Path/source/rebuild.sh` (`build`, `render`, `import-glb`, `import-fbx`, `verify`).
