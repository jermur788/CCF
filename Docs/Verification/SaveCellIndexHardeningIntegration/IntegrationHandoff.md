# Save-cell-index hardening integration handoff

**INTEGRATED, VERIFIED AND PUBLISHED. No unresolved blocker.**

| Item | Result |
|---|---|
| Integration worktree | /media/jer/Files/CCF-integration-save-hardening |
| Integration branch | integration/save-cell-index-hardening |
| Pre-integration origin/main | 5d57208a411404b65b5d9a0d4f2a6e88c4086400 |
| Source branch | task/save-cell-index-hardening |
| Exact source SHA | 12cc8ef0659f54055088047c2c6727d874bdffd8 |
| Initial relationship | 4 ahead / 0 behind; merge base exactly expected main |
| Resulting main / remote read-back | 12cc8ef0659f54055088047c2c6727d874bdffd8 |
| Integration method | Pure fast-forward; no rewrite, amend, squash, rebase or cherry-pick |
| Push | Success, ordinary HEAD → origin/main; remote SHA read back exactly |
| Final Git status | Clean |

The integration checkout was created fresh at the required base and fast-forwarded to the exact reviewed source. The protected `/home/jer/CCF-main` checkout and its local main branch were left untouched. Remote main was checked again immediately before publication and still matched the required base.

## Settings restoration and generated drift

The initially approved `.vscode/settings.json` restoration was byte-for-byte to HEAD and index. Status immediately after restoration was clean, with no other tracked or unexpected untracked changes.

Each of the five behavioral gates regenerated the exact same settings bytes: trailing-comma removal and the worktree-specific solution name. Each occurrence matched the preserved approved SHA-256 `b490b756d2fdb3e775d0471975adf3c55613bfbce0ff4f874dc6898c7d3dfbcc` and was restored exactly to HEAD under the continuation authorization. The observed bytes remain preserved under the integration checkout's ignored `Build/SaveHardeningIntegration/Normalizations/` and their receipts are in [generated-drift.json](generated-drift.json).

No material normalization occurred during these integration gates. No new file drift, implementation drift or fixture drift occurred. All tracked bytes match the pre-import exact-source manifest; original user save, backup and temp files remain unchanged.

## Verification actually run

| Gate | Result | Detail |
|---|---|---|
| Unity 6000.6.0f1 fresh compile/import | PASS | Exit 0, no compile/import errors; 704.55 s |
| SaveCellIndexHardeningVerification | PASS | 131 cases: 78 rejected / 53 accepted; 257.77 s |
| SaveHardeningVerification | PASS | 60.94 s |
| StandGeometryVerification | PASS | 10 checks; 65.50 s |
| StartupScreenVerification, batch | PASS | Continue / CanLoad / load parity / refusal / Start New; 86.16 s |
| AreaCalibrationVerification | PASS | Legacy40 and Enlarged80 values unchanged; 65.01 s |

The dedicated hardening gate ran once for integration using the existing unchanged gate function and harness. Its acceptance/no-mutation markers match the qualified source exactly: saved geometry authoritative, required bounds and optional sentinels preserved, malformed data refused before mutation, CanLoad/LoadData/Load parity exact, disk bytes unchanged. The final harness SHA-256 matches source qualification. Per-fixture before/after fingerprints establish no mutation within each run; fingerprints containing generated record IDs are not treated as frozen cross-run anchors.

Startup batch mode explicitly skipped physical/virtual key-press and rendered-layout checks; no new physical-input, rendered-layout or Player verification is claimed. The targeted hardening startup checks also passed: valid Continue enabled, malformed nested save disabled, concise 68-character refusal, Start New unchanged.

## Emitted hashes and retained qualification evidence

StandGeometry emitted `EBE7A228F630AE41`, exactly matching qualified source. All emitted hash sets in the bounded comparison gates match source evidence. Calibration emitted the expected unchanged values: Legacy40 retained/regen/deadwood 60/3/0.02 and century 120/10/12/0.5; Enlarged80 240/12/0.08 and century 480/40/48/2.

The following seven frozen anchors remain the accepted exact-source qualification evidence. They were **not all re-emitted by this bounded integration suite**; no additional gate was run solely to regenerate them. Exact source identity, unchanged tracked bytes and the passing bounded comparison checks support carrying this evidence forward:

| Qualified source anchor | Exact value |
|---|---|
| Enlarged80 START | 04A78188A6F8E6F9 |
| Enlarged80 YEAR3 | 081DB58CE3034648 |
| P2 | F58FB0B1A421D28B |
| P3 | F7C2FC966816BBAD |
| Model2 | 702766DECE591E21 |
| Reference Y100 | 7AD177B3CC2F73C7 |
| Reference continuation | 9CDF21A541C5968D |

**Full regression not required and not repeated:** integration is an identity fast-forward of the qualified 24/24 PASS source; all bounded gates passed, emitted invariant hashes match, and no unexplained repository or behavioral divergence appeared. The approved recurring settings generation was exact and restored.

## Final scope audit

Source commits remained unchanged. Save v20, scenario-one-v13, Reference Future, area calibration, save fixtures, materials and other serialized assets remain unchanged relative to the exact reviewed source. No canonical/context documentation, bundles or attachments were updated. No Player build or profiling was performed. No disposable harness or generated harness .meta remains. Final status is clean. Original saves and unrelated work were preserved.

An external orchestration assertion was corrected before the first behavioral gate launched; no repository runner, fixture or source was edited. All five gates subsequently completed successfully.

[Publication receipt](publication.json), [bounded gate results](bounded-results.json), [final verification audit](verification-audit.json), [compile result](compile-result.json), provide the committed integration evidence. Full logs and byte manifests are preserved in `/home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Hardening-Integration-Final/` and the integration checkout’s ignored `Build/SaveHardeningIntegration/` and gate output folders. Work stops here at the Manager's requested boundary.
