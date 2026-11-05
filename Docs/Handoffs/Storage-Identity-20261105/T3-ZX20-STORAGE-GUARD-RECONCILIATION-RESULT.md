# ZX20 storage guard reconciliation — #290

Verdict: **BLOCKED — CONFIG CHANGE REQUIRES OWNER OR NEW DESIGN**. Phase A diagnosed; no local write. Phase B prerequisites are not met. Worker: T3 Code / OpenAI gpt-6.1-sol / medium, 2026-11-05 ~17:08–17:12 UTC. PACKET ONLY.

## Findings and physical identity

Current mounted ZX20 is `/dev/sdc2`, exact mount `/media/jer/ZX20`, exFAT UUID `18D6-896E`, label ZX20, PARTUUID `d625c63d-7800-4a54-bc89-005d98d2e6ac`. `blkid`, `lsblk`, `findmnt` and resolved UUID link agree. Both ata-NetacPortableSSD and usb-NetacPortableSSD stable by-id partition-2 links resolve to `/dev/sdc2`; these are aliases of one physical NetacPortableSSD, not two disks. Physical serial SHA-256: `fba844bb3bee3f984d5a1a0714298f0fe80bd4cd6685c8c2f5d99a577263f0e4` (raw serial omitted).

No other visible lsblk filesystem has UUID 18D6-896E; findmnt lists one target for `/dev/sdc2`. Unmounted visible partitions are sdb1/ext4, sdb2/vfat, sdb3/swap and unformatted sdc1; no duplicate target UUID observed. This is a current visible-device observation, not proof against disconnected cloned devices. `/dev/sda2` is now the Samsung internal ext4 root filesystem with a different UUID. Current observations match #289's physical model/serial, but no historical authorised serial baseline was found in the inspected deployment/config/docs; continuity with the originally registered physical drive is therefore not fully established. Enumerator renaming is plausible, not proved historical fact.

## Exact rejection and provenance

`CCF-Ops/scripts/_ccf.py:42–47` compares findmnt source **literally** to configured device, exact resolved target, filesystem and UUID. Config line 7 expects `/dev/sda2`; current source is `/dev/sdc2`. `config_read` line 87 invokes this check before further lane validation. `storage_health` lines 50–68 repeats source/target/filesystem/UUID checks and independently enforces rw plus >=15 GiB free. `environment.sh:7–8` calls config_read then require_storage; it also checks scratch and durable directories. `scripts/lane-status:31` separately compares literal device. Supplying a by-id path in lanes.json would not work with these literal findmnt-source comparisons.

README documents deliberate reviewed deployment, not a stable-device re-registration command. Searches of deployed README/scripts/tests found no documented config-only registration mechanism retaining persistent physical identity. A volatile `/dev/sdc2` substitution would not add such assurance. Guard/script redesign is outside this packet.

Live deployment is an explicit copy, not a Git checkout: README and deployment.json say source commit `dfe445e54d099a0e20c9c7d5a9e3e6f195968b4e`, deployed 2026-10-15. Current lanes.json, environment.sh, _ccf.py and README hashes differ from deployment.json source hashes; unity-lock matches. This is provenance drift requiring reconciliation, not a finding that legitimate historical changes should be reverted. No redeployment attempted.

Current SHA-256 receipts (unchanged by worker):
- `lanes.json`: `1b02d106fe74462c2aa4979eefc37ff2ee35645c3866fc53b95b7e44a1385c2f`
- `environment.sh`: `3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954`
- `scripts/_ccf.py`: `a68293db1806cf8b9788b1d68400b2c55607176c0062a021480b4f2298247f24`
- `scripts/unity-lock`: `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5`
- `README.md`: `02385d54a4ee6c82c2dc288a310686c1b3e44fb9c953435104a11caaefa4aed0`

## Ownership and processes

Refreshed main active-tasks names #200 sole coordination writer and #290 read-only-first conditional scope. Deployed board is a supporting view with historical rows; it does not grant ownership. Filesystem owner is jer:jer, which does not establish exclusive operational write authority. No actual operations owner's exclusive temporary write confirmation was obtained. Unknown pre-existing deployment differences were preserved.

At inspection the fixed state/unity directory contained guard and history.jsonl, no lock.json. ps inventory found no Unity Editor, Unity Hub/build or CCF-Ops process except this inspection command and its search. This is an instantaneous raw observation; a normal unity-lock status cannot establish a valid lease while config_read fails. No lease acquired/released/reset; no process killed. The live coordination record still says personal playtest has not been released; absence of a process does not release that ownership or satisfy #287.

## Checks actually run

Read-only: findmnt full mount inventory and per-source target, lsblk physical/filesystem inventory, blkid /dev/sdc2, by-id/by-uuid resolution, df, stat owner, ps, deployment hash comparisons, deployed guard/config/environment/README and local StorageArchitecture inspection, refreshed GitHub active-tasks and claims.

Direct actual `storage_health` returned WRONG DEVICE / ready=false / rw=true / free=36,194,877,440 bytes (33.709 GiB), threshold=16,106,127,360 bytes (15 GiB). `config_read` raised `ZX20 mount identity mismatch` with current /dev/sdc2, expected /dev/sda2. Prior #289 environment.sh execution failed at that same predicate; no new end-to-end environment pass is claimed. No guard bypass, fixture override, scratch-writing self-test or Unity/clone test used.

Static negative-check proof: source, mount target, filesystem or UUID mismatch independently makes storage_check raise; storage_health also rejects read-only or low space. This proves current source logic only, not a repaired persistent-identity design. No repair was performed; no after-repair PASS exists.

## Proposed bounded next task and rollback

Manager should first obtain explicit exclusive ops-owner confirmation and reconcile deployed source provenance. Then approve a separate small guard-design packet: resolve an approved persistent partition by-id to the actual block device, compare it against the mounted source while retaining exact target, unique UUID, filesystem, rw and space checks; reconcile every literal-source consumer and add isolated negative fixtures. Have the owner confirm the authorised historical hardware identity or explicitly register the verified current hardware. Do not silently substitute a volatile path. Exact implementation and tests require that new scope and review.

Mutations: only this GitHub report/claim publication. No local configuration, script, mount, lease, source repository or game file changes. Backup/atomic replacement/rollback were not needed because Phase B never began. Unknown local changes remain intact, including Primary AGENTS.md and StorageArchitecture.md.

#289 remains blocked until Manager accepts a repair and fresh guard/clone preflight and redispatches it. #287 separately needs explicit personal-playtest release and a new machine-wide lease. No context generation, Drive/attachment publication, Unity, saves, gameplay or Windows build performed.

Ready for Manager review: Yes, diagnostic only. Dashboard sync: NOT VERIFIED; Manager must ingest/read back this blocked diagnosis without manually editing generated state. Closing #290 after handoff records diagnostic execution completion, not repair acceptance or unblocking dependent tasks.
