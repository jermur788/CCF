# Sen-1 #648 — Physical Ops reservation for blocked #647

2026-11-16. **Overall Manager #640** authorises only a prospective physical-board custody registration. **Claim:** https://github.com/jermur788/CCF/issues/648 READY/NOT RUNNING. **Difficulty:** Low–Medium: one row, but safety-critical shared board and exclusive source ownership. **Best-suited:** existing GPT-6.1 Sol High / Codex Linux physical Ops maintainer, previously demonstrated in #553; worker must register actual identity and a separate claim, not assume #553 still active.

## Blocking evidence

Actual source worker #647 reported BLOCKED: https://github.com/jermur788/CCF/issues/647#issuecomment-6929492069 . Its clean unmerged source checkout has no competing current writer, but actual /media/jer/ZX20/CCF-Ops/board/active.md contains no #647 reservation. Original deployed packet-lint PASS (30 rows) does not constitute custody. Last observed board SHA-256 80bb872a00ac25b2ef2e9a96b4c7d721eb1e462fc5235a45149b9e3d8ebdc416; lanes SHA-256 1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21. Recheck actual bytes, no evergreen assumption. Manager #640 already registered prospective exact GitHub ownership for #647 in main Docs/Project/coordination/active-tasks.md.

**Exact required two-file reservation only:**
1. Tools/Verification/StandGeometry/StandGeometryVerification.cs
2. Tools/Verification/ScenarioReferenceVerification.cs

Prospective source worker: existing OpenAI gpt-6.1-sol High / Codex Linux #647 integrator. Target source branch: feature/scen1-shared-three-profile-save-v2. Existing source checkout: /media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115 . Last observed clean source HEAD: 410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35. Re-fetch current status and source/ref. #647 remains BLOCKED/NOT RUNNING; physical board registration must describe a **prospective reserved writer**, not active source execution.

## Binding physical-only steps

1. Read fresh main coordination and GitHub #544/#553/#638/#642/#645/#647/#648 issues, local active-worker registry/checkout inventory, physical board and lanes.json. Inspect full 17-column header and actual deployed packet-lint source. Confirm physical board's real single writer and absence of another source/fixture writer. Stop for conflicts or uncertain ownership.
2. Register actual **#648 [RUNNING] before physical mutation**, specifying model/provider/host, copied deployed Ops directory (not a Git checkout), physical board/lanes prehashes, actual physical maintenance lock, primary anchor branch/HEAD and protected dirty anchor files. Do not invent a new Ops lane or machine/Unity lease.
3. Make the minimum safe **one-row prospective #647 reservation** preserving existing #638 historical READY row, broader #544 save ownership, other tasks and all other board records. Row must contain exact two files, actual branch/worktree checked, source worker identity and BLOCKED/NOT RUNNING state pending subsequent fresh source preflight; no historical backdating or false RUNNING. Do not edit lanes.json unless a proven needed config change is separately authorised.
4. Preserve and independently hash existing primary dirty AGENTS.md and Docs/Operations/StorageArchitecture.md. Use timestamped recovery file under /media/jer/Files/CCF/Verification and compare-before-write atomic same-directory board replacement. Show exact before/after board diff (one added row only) and unchanged other records.
5. Run unchanged deployed command: python3 -B /media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md . Require exit 0. Read back physical row and hashes; inspect current GitHub main coordination again. Publish actual evidence to GitHub as one report on separately registered docs-only branch.
6. **Exact result:** Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-648-PHYSICAL-OPS-647-RESERVATION-RESULT-20261116.md . Post #648 handoff with physical readback and notify #647 with direct link. Only then may the separate #647 worker refresh ownership and source cleanliness and register actual RUNNING before cherry-picking the reviewed fixture commit. #648 never performs that cherry-pick.

**Stop conditions:** concurrent board writer; missing physical authority/lock; changed board/target with unclear custody; malformed 17-column row; failed original validator; unexpected diff; need to mutate another task or dirty file. Report BLOCKED and leave physical board untouched. Do not weaken linter or take shortcuts.

**Exclusions:** no source C# edits, Unity, lease, real saves/Prefs, production code, main merge, Windows build, Reference rebaseline, forced reset, Ops lane config change without permission. Dashboard/Drive sync not verified.

**User action:** dispatch a separate real Linux physical-board maintainer session. The same provider used for source may work sequentially under distinct #648 claim, but must not self-grant overlapping roles or invent physical ownership.

## Paste-ready dispatch

Take CCF Scenario One #648 as the physical Ops maintainer, not the source integrator. Follow COMPLETE GitHub packet: https://github.com/jermur788/CCF/blob/docs/sen-1-648-physical-ops-647-reservation-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-648-PHYSICAL-OPS-647-TWO-FIXTURE-OWNERSHIP-RESERVATION-PACKET-20261116.md . Recheck actual physical board/lanes/source conflicts and register actual [RUNNING] model/provider/host/deployed path/physical prehash on #648 before editing. Add precisely one prospective schema-valid #647 reservation for StandGeometryVerification.cs and ScenarioReferenceVerification.cs on the existing unmerged shared-save checkout. Preserve #544/#638/history and all dirty files. Run original packet-lint and read back actual physical bytes; publish the named GitHub handoff. No source work, Unity or Windows build. BLOCKED if real physical custody is unsafe.