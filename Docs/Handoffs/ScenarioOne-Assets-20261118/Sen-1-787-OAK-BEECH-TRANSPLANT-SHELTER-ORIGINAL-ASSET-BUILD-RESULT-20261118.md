# Sen-1 #787 — 0.60 m sessile Oak / European Beech transplants and slotted shelter kit — result

**Verdict: BLOCKED (G0-S owner no-root storage gate). No asset was built.** No source generator, `.blend`, glTF/FBX export, render or verification output exists. Blender was never launched by this lane. The production import, Unity integration and #698 Stage 1 remain held as before.

## 1. Actor and custody

| Item | Actual |
| --- | --- |
| Claim | https://github.com/jermur788/CCF/issues/787#issuecomment-6993837129 (supersedes the dead attempt's `[RUNNING]` comment 6992832682) |
| Model / provider | `claude-opus-5-5` / Claude A (`claudeAgent`) |
| Harness | T3 Code → Claude Code 2.1.327, Agent SDK 0.3.321. Implementation role |
| Host / session | `jer-laptop`, Linux 7.0.0-42-generic, uid 1000. Session `bdd92ecf-dd90-4946-99cc-e5b500b11a56`; first command at 2026-11-18T22:34:24Z |
| Checkout | `/media/jer/Files/CCF/Operations/CCF-787-BroadleafProtection-20261118` (realpath). Branch `art/sen-1-787-broadleaf-protection-offline-kit-20261118-v1`, base HEAD `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. Git common dir `/media/jer/Files/CCF/Repository.git` |
| Scratch root | `/media/jer/Files/CCF/Verification/Sen-1-785-AssetKit-20261118/JuvenilesProtection` |
| Superseded attempt | Prior `CLAIM.md`: session `777951fd…`, Claude B. It died on a TLS error and wrote no art. Preserved byte-identical at `_g0/superseded/CLAIM.prior-attempt-session-777951fd.md` and not trusted |

## 2. G0 physical gate — PASS (actual readback in `_g0/G0-READBACK.txt`)

- **Files mount:** `findmnt -no SOURCE,FSTYPE,UUID,OPTIONS /media/jer/Files` → `/dev/sdb4 ext4 2283b135-bf37-42d6-921f-2fb861d0a8d4 rw,relatime`.
- **Root mount:** root `/` is `/dev/sda2` ext4, UUID `d393b49a-24aa-4f88-b8c4-5121b96c240d`.
- **Separate disks:** `lsblk` shows `sdb4` on physical disk `sdb` and root on disk `sda`. Files is therefore a separate device, not a bind mount or symlink into root.
- **Ownership:** kit dir and scratch root are `jer:jer 775`, dev 2068. `id` = uid 1000 `jer`.
- **Write test:** create/read/remove passed at 22:34:24Z and again at 22:41:45Z, residue absent.
- **Free space, `df -B1`** at 22:41:45Z, and again at 22:44:40Z after all writes:

  | Filesystem | 22:41:45Z | 22:44:40Z |
  | --- | ---: | ---: |
  | Files | 22,267,416,576 B | 22,267,207,680 B |
  | Root `/` (96%) | 10,682,380,288 B | 10,694,938,624 B |

  The root reading is live and supersedes the parent's earlier "<2 GB" observation.
- **Files 10 GiB floor:** 10,737,418,240 B leaves 11,529,998,336 B of headroom, enough for the 1.2 GiB cap. **Capacity: PASS.**
- **ZX20:** `/dev/sdd2` exFAT was not used. The stale ZX20 Ops state was untouched: no lanes.json, board, unity-lock or device pin.
- **Primary:** `/media/jer/Files/CCF-Primary` was only read and still shows its 7 user entries.

## 3. G0-S owner no-root storage gate — FAIL, lane stopped before G1

The binding owner directive (2026-11-18T22:02Z, [directive](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-OWNER-NO-ROOT-FILESYSTEM-DEVELOPMENT-STORAGE-DIRECTIVE-20261118.md), sha256 `73780c57…7a6a`) is carried in the #785 packet banner (Manager branch `6d0e9a30`). The banner says: *"If T3/OpenCode/Claude/Codex intrinsic logs still write to root and cannot safely be redirected, STOP new development processes and report an honest BLOCKED state."* The #787 comment of 22:04Z says the same: *"STOP at safe checkpoint and report blocker, do not assert zero-root-writes."*

**Measured on this host:**

| Path class | Backing | Controllable by this actor? |
| --- | --- | --- |
| Scratch root, `<root>/tmp` (`TMPDIR`/`TMP`/`TEMP`), checkout, git store, numpy tool env | `/dev/sdb4` Files | Yes. Set for every shell of this lane, plus `PYTHONDONTWRITEBYTECODE=1` |
| Claude Code transcript `/home/jer/.claude/projects/-media-jer-Files-CCF-Primary/bdd92ecf-dd90-4946-99cc-e5b500b11a56.jsonl` (`CLAUDE_CONFIG_DIR=/home/jer/.claude`) | `/dev/sda2` root | **No.** 864,708 B at 22:37 → 993,395 B at 22:41:45Z, and it grows with every tool call |
| Claude Code shell snapshot, session json, plugin/skill sync files under `/home/jer/.claude`; harness task output under `/tmp/claude-1000/` | `/dev/sda2` root | **No** |
| T3 Code provider event logs, trace logs and state DB under `/home/jer/.t3/userdata` | `/dev/sda2` root | **No.** Updated in the same second as this lane's tool calls |

Relocating the uncontrollable paths would mean changing `CLAUDE_CONFIG_DIR`/`HOME` or T3's userdata location and relaunching. Those locations hold provider credentials and config. The directive forbids changing them ("Do not blindly change HOME, global configs, cached provider authentication …"), and it cannot be done from inside a running session. The lane therefore stopped at its safe checkpoint, before any development process. **No zero-root-write claim is made.**

**My own avoidable root writes, since corrected:**
- Three downloaded binding-source copies (52,430 B) briefly sat in the harness scratchpad under `/tmp`. They were copied to `_g0/binding-sources/`, sha256 values matched, and the root copies were removed.
- One empty directory I had created was removed.
- Auto-memory was deliberately not written, because it lives on root.

The parent's dispatch prompt pre-dated its own reading of the directive: it cites the 21:52Z "100% / under 2 GB" observation and requires only `TMPDIR` relocation. The directive states it takes precedence over individual task packets.

Sibling #786 continued at 22:35:42Z under the same directive ("Harness-internal logging relocation is not exposed; no zero-root-write claim"). That is a different reading, and Manager #731 should reconcile the two lanes. This lane makes no judgement on #786.

## 4. Outputs that exist (all non-asset, Files only)

Scratch root, 8 files, 81,332 B. `sha256sum -c _g0/SHA256SUMS` passed every line at 22:44:40Z.

| Path under scratch root | Bytes | SHA-256 |
| --- | ---: | --- |
| `CLAIM.md` | 6,688 | `cfe26a5dafc8576252edb71d42d7b12142f4193cf896ee153788fdbee0208c9e` |
| `_g0/G0-READBACK.txt` | 10,736 | `92cd48399fe2f78dde570d87bff716fa926b7a2c3ff509530bcfab27e07c16ab` |
| `_g0/BLOCKED-COMMENT.md` (copy of the posted claim) | 4,564 | `2a3bf68bfc31071b02c9e85138d88ea16c735964763be1cbea571d10edcbc4d6` |
| `_g0/superseded/CLAIM.prior-attempt-session-777951fd.md` | 6,249 | `c073c6675a91772eb189d47b2a01396241c5c110070e87363d3272ed5250e39b` |
| `_g0/binding-sources/blueprint.md` (#698 blueprint @ `7a8e575e`) | 22,668 | `cde70e9fd2cc5d2eca68867aee7c5413c8752c2db769220af21ac26577bf92d0` |
| `_g0/binding-sources/packet.md` (#785 packet @ `6d0e9a30`) | 23,368 | `c21c2119cdec1316a404447740e83079859224379bccb7d4cfbe77150af0558a` |
| `_g0/binding-sources/directive.md` (owner directive @ `6d0e9a30`) | 6,394 | `73780c57c6fb364f79658c94b7df8808a06e5928d6aae97873e5572283cb7a6a` |
| `_g0/SHA256SUMS` | 665 | `fa9962d7ae9674550c91f0b0d0d5bff61e48aedad6cdcfb775bc41a5b7aa8651` |

`tmp/` exists and is empty.

## 5. Required deliverables — status

| Deliverable (packet §5 / dispatch) | Status |
| --- | --- |
| `source/build_juveniles.py`, `source/build_shelter.py`, `source/recipe.json` | **NOT BUILT** |
| `blend/JuvenilesProtection.blend` | **NOT BUILT** |
| `export/`: 2 species masters + 4 first-year variants, 3 LODs each, glTF + FBX, re-import | **NOT BUILT** |
| `renders/`: 64 matrix stills + inside-tube + silhouette sheet, Cycles CPU | **NOT RENDERED** |
| Height / LOD / pivot / species-distinction checks | **NOT RUN.** No measured numbers exist |
| Shelter 24-panel / rim / stake / ≤512 tri / 2 slot checks | **NOT RUN** |
| Per-state XZ radius proof against 0.10 m | **NOT RUN.** The blueprint's analytic 0.0930389703 m was not re-measured |
| Export hashes, fresh re-import, determinism rebuild | **NOT RUN** |
| `SOURCE_RIGHTS.md` | **NOT WRITTEN.** No art exists to attest |

The Blender 4.0.2 / numpy `PYTHONPATH` / Cycles-without-denoise toolchain facts come from the parent and were **not re-verified** by this lane.

## 6. Unblock and resume

One decision by Manager #731 or the owner. An agent cannot make it:

1. **Explicit ruling:** bounded harness-intrinsic Claude Code/T3 transcript and log writes on root are accepted while every development output stays on Files. For a full build these writes have not been measured; the estimate is the order of tens of MB. **Or:**
2. **Owner-approved relaunch:** the lane is relaunched in a harness whose config, transcript and state directories are on a non-root filesystem.

After either decision, #787 can be re-dispatched into this same scratch root and checkout. The `_g0` evidence stays valid for its timestamps, but G0 must be re-read. The rest of the scope is unchanged:
- **Juveniles:** 2 masters + 4 first-year variants. This is six models, against the packet's "4 export prefabs/concepts" wording, which a resumed build will state openly.
- **LODs:** 3 per model.
- **Shelter:** the 24-panel recipe exactly as specified, with all present states proven against the r ≤ 0.10 m envelope.
- **Renders:** the full Cycles CPU matrix, denoising off.
- **Seal:** SHA256SUMS.

## 7. Limitations and not verified

- No geometry, LOD, radius, render, export, re-import or determinism claim exists. Nothing was produced to measure.
- No Unity/URP, player, controller or production import happened. There is no D-032 rights decision and no owner look selection.
- Year 0 has zero G1 plantings and shelters, and this lane changed nothing about that.
- Whole-system root writes outside this session's harness (journald, other sessions, T3's shared state DB) were not traced. Only the paths listed in §3 were measured.
- Independent review (#788) has nothing to review from this lane.
- Dashboard and Drive status: NOT VERIFIED.
