STILL-BLOCKING-WITH-NAMED-GAP

## A. Exceptions after publication

The original receipt EIO cases are fixed. `replace_one()` catches readback and directory open errors after `os.replace()`, records a failed directory sync, and returns the expected SHA-256 computed from the source bytes. The batch appends the record before checking success. A directory-close `OSError` also reaches the outer catch and triggers recovery. The literal guarantee that nothing can escape after rename is too broad. A default SIGINT (`KeyboardInterrupt`) during receipt readback escapes the `OSError` guards. In the in-memory probe, six candidate files were visible, five were booked, and zero were restored. [Publish and append](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:85)

## B. Receipt fault scenarios

All three logged injections targeted the already-renamed `deployment.json` at index 6 and reported `post-rename-io` with errno 5. The log records six bookings, six `RESTORED` outcomes, exact pre-state inventory, and a passing no-temp check for each.

| Receipt fault | Booked | Restored | Exact pre-state | Temp residue |
| --- | ---: | ---: | --- | --- |
| Directory sync EIO | 6 | 6 | Yes | None |
| Directory open EIO | 6 | 6 | Yes | None |
| Readback EIO | 6 | 6 | Yes | None |

I independently checked all three in-memory paths and observed the restore order `deployment.json`, `README.md`, `scripts/_ccf.py`, `roles/unity-operator.md`, `tests/self-test.py`, `scripts/unity-lock`. On the surviving ZX20 scratch tree, both the replica and its backup matched all 50 baseline paths and SHA-256 digests and contained no `*.tmp`. The surviving scratch JSON matched the durable copy byte for byte. The JSON records 20 PASS and zero FAIL, with scratch and live Ops on exFAT device 2098. [Scenario assertions](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:249), [logged outcomes](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat-qualification-v5.json:37), [baseline inventory](/media/jer/Files/CCF/Verification/Sen1-741-B-StageRehearsal-20261118/p1/live-inventory-before.sha256)

## C. Failed restore and named blocking gap

The injected restore-write failure on `scripts/_ccf.py` is handled honestly. The reverse loop reports `RESTORE-ERROR` and restores the other five paths, including the three remaining after the error. The scratch log records the retained candidate hash `24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435`. A separate repair returns the tree to its full baseline. The extracted functions returned the same five-to-one result. [Restore loop](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:163), [partial-state evidence](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat-qualification-v5.json:130)

**Blocking #7 stays open because an I/O error in a state check can still bypass or stop recovery.** Two read-only probes of the extracted functions reproduced this:

- A one-shot EIO hashing the existing receipt at line 138, after five successful replacements, escapes before the forward `try`. Five files stay published and zero are restored. The receipt retains its old bytes.
- If the renamed receipt alone stays unreadable, the forward readback error books all six. The first rollback comparison raises EIO at line 117. The error handler retries the same hash at line 172 without a guard. That EIO escapes before any failure record is appended, leaving all six candidate bytes in place and skipping the other five restores. [Forward check](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:138), [rollback comparison](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:116), [unguarded diagnostic read](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/exfat_qual_v5.py:165)

Either read failure can occur during a single-user install. A lawful pre-deploy correction on a disjoint ZX20 replica would route failed pre-state reads through the shared abort path and record an unreadable recovery path with its errno and hash unavailable, then continue with the other booked paths. A targeted fixture can verify five safe restores and a separately reported unresolved receipt for the persistent-read case, and exact baseline plus no temp residue for the one-shot precheck case. Neither requires a live Ops write or flock. Power loss, kernel crash, and volume-wide failure remain deploy-window residual risks managed by the verified off-volume backup and reconciliation under renewed custody.

## D. Dossier consistency

§8 step 7a prescribes precomputed expected hashes, booking before the success decision, compare-before-write in reverse order, and continued recovery with explicit partial-state reporting. That matches the tested receipt cases and isolated restore-write injection. The actual booking happens after the post-rename checks return, so it depends on those checks returning a record. The unguarded pre-state read and diagnostic re-read violate the procedure's broad recovery rule. The line claiming every listed case was rehearsed in v5 also exceeds the recorded tests. V5 has the three post-rename receipt faults, synthetic abort, restore-write failure, and a separate rollback drift refusal. It has no forward-write fault or forward pre-state drift injection. Step 5 describes directory sync as part of the shared primitive, while step 4's explicit five-file sequence omits it. §8.1 still describes the older 14/14 qualification. [Pinned dossier, §8](https://github.com/jermur788/CCF/blob/841d55117505ec5da43831148360d64195896222/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-741-D-CUSTODIAN-ONLY-INSTALLER-RESERVATION-AND-READINESS-DOSSIER-20261118.md#L124-L146)

The independent reviewer was OpenAI `gpt-6.2-sol` (high) through the T3 Code Codex harness, session `01a1ef2d-23f0-7790-88de-c2851a303226`. The review branch started at `c10eac9b579c877d31595336076adb2c4d09b647`. The source branch `docs/sen-1-741-deepseek-ops-and-compact-readiness-20261118-v1` resolved remotely to `841d55117505ec5da43831148360d64195896222`. I recomputed all four supplied pins. [Issue #746 registration](https://github.com/jermur788/CCF/issues/746#issuecomment-6982352242), [C1 at reviewed commit](https://github.com/jermur788/CCF/blob/841d55117505ec5da43831148360d64195896222/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-741-C1-EXFAT-REPLACEMENT-SYNC-AND-RECOVERY-QUALIFICATION-20261118.md), [durable evidence checksums](/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v5/SHA256SUMS)

| Artifact | Verified SHA-256 |
| --- | --- |
| Durable v5 JSON | `ea9ce18713043eaf4e8cd3a2bc538463c30c750e1df705b81e72ffb296a5ad1a` |
| Durable v5 Python | `8707300948e172479f9a88560967131091a0835dd842cab4d5f98d63d1d31f8a` |
| C1 Git blob bytes | `30ea0df60d80c1606730563b49bc62c15dbea34111f8db2d6865d826fc2de5cc` |
| Dossier Git blob bytes | `92ec454ef5c82e235e2bce4b80edcb300143cdafd32b1cdc94a84280523fbde2` |

The probes used in-memory filesystem doubles to verify control flow. The original exFAT run supplied the filesystem evidence. The full harness was not rerun. No live Ops write or flock, Unity, board/lane/state change, or deployment authority was exercised. The owner's **NO LIVE INSTALL** order remains in effect. Dashboard and Drive synchronization remain unverified for this review.
