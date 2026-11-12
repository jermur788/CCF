# #423 — D-073 existing #417 artifacts: read-only G3 witness

**Witness:** Claude Sonnet 5.5 (model `claude-sonnet-5-5`, Claude A), re-dispatched after the Codex worker hit a usage limit and left nothing behind. Child claim #423 of #412. Read-only: no Unity run, no build, nothing in the evidence directory was changed, copied into, touched or chmod-ed. Despite the `CODEX-` file prefix (fixed by the dispatch), the witness is Sonnet, not Codex.

**Scope statement.** This corroborates the **ORIGINAL candidate `77a56a4461f8ab2d313082af50442ce24dc54471`** only (tested runner `0d624c61…`, final). It does **not** qualify any changed or repaired source (including the bounded FR-10 AnnualReviewView correction). It is a witness of the worker's artifacts, not a Unity re-run and not an integration/acceptance gate.

Evidence root: `/media/jer/Files/CCF/Verification/D073-YearComparison-417-20261112/` (543 files). #417 report: `origin/qualify/scenario-one-d073-year-comparison-unity-20261112-v1:Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/CODEX-417-D073-YEAR-COMPARISON-UNITY-QUALIFICATION-RESULT.md` — byte-identical (21,209 B) to `final-report-draft.md` in the evidence root.

## Verdict

| # | Point | Verdict | Supporting files (under evidence root) |
|---|---|---|---|
| 1 | runtime1–5 receipts/logs, real counts, FAIL history | **WITNESSED** | `runtime{1..5}/D073.log`, `result.json`, `candidate.txt`, `run-audit.json` |
| 2 | 18 screenshots: dimensions, content | **WITNESSED** | `runtime5/evidence/*.png`, `screenshot-audit.json` |
| 3 | Native input driver receipts | **WITNESSED** (receipt consistency; see limit) | `runtime{2..5}/driver/native-actions.jsonl`, `request-*/done-*/desktop-{before,after}-*.png`, `runtime5-command.json` |
| 4 | Old v9 host save + frozen Reference v12 | **WITNESSED** | `genuine-v9-audit.json`, `custody-before.json`, `runtime1/D073.log`, `source-identity.json` |
| 5 | Five leases taken and released normally | **WITNESSED** | `lease{1..5}-{acquire,release}.json`, `lock-final.json`, `end-state.json` |
| 6 | 37 host hashes + 4,011 production hashes before/after | **PARTLY WITNESSED** | `custody-before.json`, `lease{1..5}-custody.json`, `production-before.json`, `lease{1..5}-production-restored.json` |
| 7 | Digests | **DONE** | below |

### 1. Runs (real result lines counted by me: `^ *D073_CHECK (PASS|FAIL)` and `D073_CASE … (PASS|FAIL)`; the `…_FAILED` substring trap does not occur — `FAILED` count is 0 in all five logs)

| Run | Runner SHA (`candidate.txt`, matches `result.json.head` and lease head) | CHECK PASS | CHECK FAIL | CASE PASS/FAIL | Exit | Outcome |
|---|---|---|---|---|---|---|
| 1 | `b0775321e1494c60449bbdc3200db9b9cce8ea10` | 137 | 1 | 8 / 1 | 1 | FAIL: `exact requested game resolution` (logical 1600×900 vs pixel size) |
| 2 | `7b8ca89276f901a47045f49125c1fdfdbbcd66ef` | 19 | 1 | 0 / 1 | 1 | FAIL: `card exclusively inside existing scroll` |
| 3 | `ada4a0dd84e26b02305528ce3799b1d17badaee6` | 19 | 1 | 0 / 1 | 1 | FAIL: `live card exclusively inside existing scroll` |
| 4 | `e6d80838acc2536b9b31af0469b8444ff8ebabd5` | 28 | 1 | 0 / 1 | 1 | FAIL: `native target present/enabled` |
| 5 | `0d624c61bb8852850a07b4b33d83e740a032c86c` | **99** | **0** | 4 / 0 | 0 | `D073_PASS checks=99 shots=18` |

Runtime5 cases: native-1280x720 35, native-1920x1080 35, rendered-negative-1280x720 13, rendered-negative-1920x1080 13. Zero `error CS` in all five logs; Unity `6000.6.0f1 (f7f8ed4d1e24)`. The four failed runs are retained truthfully in the #417 report and `measurement-corrections.md`; the report's three "harness assumption" corrections map onto the failure messages above. Runtime5 `evidence/checks.txt` has 99 PASS lines + 4 CASE lines.

### 2. Screenshots (PNG IHDR read directly; SHA-256 recomputed, equals `screenshot-audit.json` for all 18)

| File (`runtime5/evidence/`) | Pixels | SHA-256 |
|---|---|---|
| native-1280x720-acknowledged.png | 1280×720 | 4cef377d2d2edb1d44f1d6405f644d5d9c7b85fce538b57dc7aad25411ee37d7 |
| native-1280x720-earlier-no-sources.png | 1280×720 | 19ff7ceb77e79494b703d9aa1ec6eecacf1919057af8bf0beee922bb4eccdf3c |
| native-1280x720-earlier-sources.png | 1280×720 | 83545fdaa3ef6d912c86a031f4d0b862be8b4c08fdf4cb8524f8e0f6cebcef69 |
| native-1280x720-expanded-species-caveat.png | 1280×720 | b43ade99c3f905d8ecdaf308b14f84c8951317d8b9ad7e963661107473186607 |
| native-1280x720-latest-footer.png | 1280×720 | 0457cce1885c98dc187f141a295ae992ba8c5583cca80b669e1f403cf3993e62 |
| native-1920x1080-acknowledged.png | 1920×1080 | 6c4e7c927b4a85fb505344baf524fa55930084c9fd0f7c04d2af2dc18624bb75 |
| native-1920x1080-earlier-no-sources.png | 1920×1080 | 333bebf6f4f169ed48116f260e90e840c1862765f1c70dc507cb3246c3c67e45 |
| native-1920x1080-earlier-sources.png | 1920×1080 | 240341de268591236205cd8f10c386fd6700aae021f4219cd05ddea023d4df27 |
| native-1920x1080-expanded-species-caveat.png | 1920×1080 | 4617c1b70242d1f88cbb9a081819c8b22bf32898726a1d7f6a74a26df8ea7bff |
| native-1920x1080-latest-footer.png | 1920×1080 | 3cdd8318d337715b63b7c8d325e625259b8c8c6e735d2ab356e198a7e30046e4 |
| negative-1280x720-ambiguous.png | 1280×720 | 2e278803d646b4f5c494bc2159a80e96cab8295bb4134b6e58f952723540fc10 |
| negative-1280x720-missing-latest.png | 1280×720 | f072b5b3acb9fe1aa9e0163c5b6652089c610f678d32c44bef845d10daa41a1f |
| negative-1280x720-missing-selected.png | 1280×720 | 0eb69ed4cddc7bb12834a47b4cc1352515d5ca406a07ef25409ff5d12c4628dd |
| negative-1280x720-species-one-sided.png | 1280×720 | d98d01ff774904ef783baff356d04aa556a25e36f9cc489961b68f876b3e4dbe |
| negative-1920x1080-ambiguous.png | 1920×1080 | 2ebf04b89dc286881b2ad636f2e9ebc9f072bdac5f86c1ee7d8a79b7e1709b07 |
| negative-1920x1080-missing-latest.png | 1920×1080 | 7355e26b573adab96f13e0272b6d1a5c7adb479ffcdf8b898590d38bd902001d |
| negative-1920x1080-missing-selected.png | 1920×1080 | fd89e47a016ee72debec825d5d54775123ca57189e7d657be6f6f0402edde544 |
| negative-1920x1080-species-one-sided.png | 1920×1080 | bc5487af313b9e800d02f79c54b97b2256e11e3a7e6d079ee06cb0c43532c67f |

All 18 are exactly 1280×720 or 1920×1080 (9 each). **I viewed 6 (both sizes):** 720p expanded-species-caveat, 720p negative-missing-selected, 720p acknowledged, 1080p earlier-sources, 1080p latest-footer, 1080p negative-species-one-sided. None is black/blank; all show the Annual Review modal over the forest. Seen: header "Year 3 (earlier year; latest is Year 6)" with RECORDED/DERIVED chips and "Show data sources" toggle; "YEAR 3 COMPARED WITH LATEST YEAR 6" with recorded outcomes (2 planted), snapshot table and differences; per-species rows with the fixed caveat "They do not show that Year 3's work caused the difference"; NOT RECORDED states ("No stand snapshot was recorded for Year 1. Nothing is compared."; "European beech: not recorded for one of the two years … Missing for Year 1; recorded for Year 6"); latest-year hint "Year 6 is the latest completed year. Choose an earlier year…" with the "I've read the annual results" footer button. Limit: `native-1280x720-acknowledged.png` is a scrolled Management History/Trends position without the comparison card in view (acknowledged-state, Later disabled); a screenshot is one scroll position. I did not view the other 12.

### 3. Native input
`runtime5/driver/native-actions.jsonl`: 56 ops = 24 absolute moves, 24 button-1 clicks, 6 button-4 (wheel) clicks, 2 `Escape` key ops; 28 request/done pairs with 28 desktop-before/after PNGs (56). Spot-checked pairs 0014 and 0026: before/after desktop images differ (hashes differ). Runtime2/3: 10 ops each; runtime4: 23; runtime1: 0 (failed before native entry — consistent with its failure). Runtime5 log has two `actual pointer wheel scroll changes position` PASS lines (1711→1387, 1735→1411). `runtime5-command.json` shows `--native`. **Limit:** I did not independently re-prove the xdotool/Xephyr delivery chain (the report says xdotool; `driver.stdout` is empty and the word does not appear in the receipts); I checked internal consistency and that the receipts are OS-level move/click/wheel/key records rather than method calls.

### 4. Old-save and Reference evidence (hashes/counts only; no save bytes read into this report)
- `genuine-host-v9-year39.json` SHA-256 `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3` — recomputed; equals `genuine-v9-audit.json.old_sha256` and the `custody-before.json` value for the host `forest-save.json.bak`. File mtime 2 Oct (pre-dates the run). Audit: `old_version 9`, `old_year 39`, `loaded_year 39`, `tree_count 361`, `ids_equal true`, `positions_equal true`, `shared_tree_field_differences []`. Runtime1 log: `genuine unmodified archived host v9/year39` PASS, `loads Legacy40` PASS, `all tree identities retained` PASS, ordinary advance PASS (case `AC10/genuine-v9-Legacy40` 9 checks).
- Frozen Reference v12 (runtime1 log): `REFERENCE_ARCHIVE_INTEGRITY_PASS milestones=0,20,50,100 schedule=56C8B99FA1E8DDD1 archiveFileFnv=1A42C7BD0439E72F`; embedded==stored, pinned, version 12: Y0 `A564039D9B7CE31D`, Y20 `F7DF7DAB53B6FD32`, Y50 `D5E75D6D21D631AC`, Y100 `7AD177B3CC2F73C7`; `NEGATIVE_CONTROL_PASS tamperedYear100Value=detected`; four preview-open checks PASS; case `AC10/frozen-reference` 22 checks. Archive/schedule file SHA-256s are in `source-identity.json` (bytes `789e3edd…41fc`, schedule `91cf5059…98f0`).
- Parity: `D073_PARITY manualHash=6DF6B75E8B957F8C skipHash=8B0BDF75F9BC3816 cardsExact=True` (raw hashes deliberately differ because of the intermediate acknowledgement; `parity-semantic-audit.json` is the worker's normalised audit — I read it but did not re-derive it).

### 5. Leases
Five distinct lock_ids, all owner `Codex gpt-6.1-sol HIGH #417`, task `CCF-417-D073-YEAR-COMPARISON`, lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, branch `qualify/scenario-one-d073-year-comparison-unity-20261112-v1`, head = the matching runner SHA: `a3990973…`, `b274b612…`, `eb33dbf7…`, `791f0101…`, `4508b59d…`. Every `release.json` has `status released`, `forced false`, `unity_editor_pids []`. `lock-final.json` and `end-state.json` (2026-11-12T16:09:40Z): lock free, no Editor PIDs, `normal_releases 5`. Limits: `release.json` carries no release timestamp (only file mtimes), and `pid`/`process_identity` are null in the acquire records, so "after Editor exit" rests on the empty-PID list and runtime exit codes.

### 6. Custody
- **37 host hashes:** `custody-before.json` has 37 keys, all with non-null SHA-256; `lease{1..5}-custody.json` are each identical to it (same file SHA `663e4428…7d`; dict-equal). WITNESSED from recorded manifests. I did **not** re-hash the live host files (private, and some such as logs may change legitimately).
- **4,011 production hashes:** `production-before.json` has 4,011 entries; each `lease{1..5}-production-restored.json` states `count 4011, same true`. But there is **no independent "after" manifest** in the directory — the "same" verdict is the worker's own comparison. Editor drift (material/settings normalisation, `.vscode/settings.json`) was restored and is preserved in `lease*-Editor-drift.patch` and `lease*-drift-bytes/` (inspected only as listings). Hence PARTLY.

## Defects and gaps (report vs raw files)
1. **No contradiction of a headline claim.** Counts, SHAs, exits, dimensions, hashes, lease ids in the #417 report all match the raw files.
2. "99 checks" = 96 inside the four named cases + 3 outside them (setup checks); the report does not state this split. Not an error.
3. The report says "native 35 checks at each size, actual xdotool pointer, wheel and keyboard Escape"; receipts show wheel as X button 4 and only two Escape ops in the whole run. Consistent, but xdotool itself is not named in any receipt.
4. No independent after-manifest for the 4,011 hashes (point 6).
5. `visual-review.json` is the worker's self QA (explicitly "not T06"); my 6-image look is an independent partial check only.
6. Report notes the known `UnityEditor.Search.SearchDatabase` startup exception; I saw one occurrence per log (`known_search_exception: 1` in `run-audit.json`), not a C# compile error.
7. Evidence directory contains `final-report-draft.md`, `run-audit.json`, `screenshot-audit.json`, `visual-review.json`, `end-state.json` newer than `lock-final.json` (written during closure; ordinary).

## Not checked
Source/diff of `77a56a44` or the repair; correctness of the D-073 mapper logic or the 132/43/21-assertion fixtures beyond the logged PASS lines; the other 12 screenshots by eye; the `-labels.txt` text dumps; the 56 desktop PNGs (2 pairs hash-compared only); the Xephyr/xdotool chain; live re-hash of host or production files; whether the physical board/dashboard reflect the claim; any behaviour of changed source.

## Digests (SHA-256)
**Whole evidence directory** — run from inside it: `find . -type f | sort | xargs -d '\n' sha256sum | sha256sum` over 543 files:
`4ee0f2626c3feaad28b592e0cf898f10762a99778c7bac1c490d8665ce840fc1`
(relative `./` paths; computed after all my reads; I made no writes, so a re-run should match unless #417 files change later.)

| Key file | SHA-256 |
|---|---|
| runtime1/D073.log | 6cc6af24642bb8a8ea086df009bca43e20d1bc772d4e20249f3ff0accd8a3750 |
| runtime2/D073.log | 1b41455144800e639adf5dac7797e8016977b21f6203a678ef56010e9a7fa5cd |
| runtime3/D073.log | 5d6e3f094a2007888a76aaec8ada25c0949986229233ca94d945dcdbf55f8f67 |
| runtime4/D073.log | a1652b38c21278dc1043e5ac5b16b1125b26d504c18badb06062485cc442f431 |
| runtime5/D073.log | a1c3c0bcc30eaf6d19c2196fec7fd1237436559c1589f3dcb54c6c9211362b07 |
| runtime5/result.json | 0e8dfcfda5fc33fa7bab3d8290d4c1324a675ee4946a48072f2dab9f9ce0b29d |
| runtime5/evidence/checks.txt | 68e74cdc13e8fe1cc6dbc741cc5ceaa8b629c3af7d6cdb2d6b02b985f629b3fa |
| runtime5/driver/native-actions.jsonl | f270c69aa630590c76f79b87e726426e7be699ab669ec4ca471fcc66b6cde60f |
| run-audit.json | 0e1dc2b64ea0d7cee7cb06ede11dad3120202d57424511844e26e878a2c63b76 |
| screenshot-audit.json | e11d673a7446eaaca8c0c2152661ad74acb548ce5a34f2ad9fc7db1f69058b3f |
| genuine-v9-audit.json | d2aa8c534446cf4656aceee1edd29edb20b91373d55a4c622501d459a4cd5928 |
| source-identity.json | 83f505f39e3a262b05acc988ffc2c3696f3034637a918108fd16c562f7b4c488 |
| custody-before.json (= lease1..5-custody.json) | 663e4428f924083e08563b37e0a4f35546d2bad50acac97bcb7116a4d31b4e7d |
| production-before.json | 470d25baf5fa8303bb8a06d431248693345c269a1591c258ad58e71275f3679c |
| lease1-release.json | e8022badc234f407533c93e98a5fe9364222025f31f7e0022e3bd4e2af23ba9f |
| lease5-release.json | a2f56e93e79b17f82434832f754d032c3ca14e485644fa12b6f32ac617259385 |
| lock-final.json | 40768aa7decc04605f5b87c5285891f74fd2774baaa2db09fa5fc0f83046be47 |
| end-state.json | d571fdab9be6176340602d5a4ce89ef5803702261fe3fd5c33bcfcc2449074d1 |
| measurement-corrections.md | cc22cf7ac32ad414d4a757bf47e9ba89f1579faeb3905ac75b94df3d8d2b1fe4 |
