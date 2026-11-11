# #383 CCF-only cleanup approval queue — PARTIAL (#383 stays OPEN)
Preflight 2026-11-11 23:03Z: `/` sda2 ext4 19,826,892,800 free (user's ~20 GB confirmed); Files sdb4 182,951,198,720 free; ZX20 sdc2 exFAT 26,788,233,216 free (was unmounted at 23:00Z, mounted by 23:03Z). No Unity process running.
Nothing deleted, moved, or written to any candidate.

## Older vs UI-updated Scenario One
- U3 (#361): HEAD 4db486db, qualify/scenario-one-annual-review-reset-370; ancestor of #382 HEAD (5 commits behind, 0 ahead). Original anchor fb599eec is also an ancestor of #382. Ancestry only - NOT proof the local folder is redundant.
- #382: HEAD 2aaacad6, clean tree, released 2026-11-11T01:14Z.
- Recommended future playtest copy: #382. Both roots and warm Libraries (7.27 GB / 7.01 GB) PROTECTED - RETAIN.
- Not verified: save/PlayerPrefs equivalence. Only found `~/.config/unity3d/DefaultCompany/CCF/prefs` and `save-backup-20261104/forest-save.json`, shared by every lane.
- CCF-UIQualification-20261104 is a different, older lane (a1e15b52).

## Batch proposal
No batch is approvable yet. Smallest missing items: (1) owner/Manager confirmation that lanes MainInt277 and UIQual275 are retired; (2) decision whether ~7.2 GB reimport cost on re-use is acceptable. Z2 then Z1 (Library only, 14.4 GB ceiling) are the first candidates. Dirty files in both are float-noise reserialisation plus a .vscode solution name, so no unique source work was found.
## NOT RUN (outstanding, per Manager #200)
- Worktree candidate check (<=10 of 107 registered): NOT RUN
- Builds independent backup/archive proof: NOT RUN
- Windows migration impact: NOT RUN
- Unity launch, save/PlayerPrefs equivalence check: NOT RUN

## Boundaries
No file deleted, moved or archived. #361 (CCF-U3) and #382 projects and warm Libraries remain PROTECTED. T3 Fork Integration HOLD remains binding. The overall pre-ZX20 audit is NOT complete.
