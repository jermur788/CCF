# #383 mount and storage summary — PARTIAL

Observed 2026-11-11 20:34 UTC. Bytes from `df -B1`, not reclaimable space; numbers are point-in-time.

| Mount | Device / filesystem / UUID | Total bytes | Used bytes | Available bytes | Mount options |
|---|---|---:|---:|---:|---|
| / | /dev/sda2 ext4 d393b49a-24aa-4f88-b8c4-5121b96c240d | 250375106560 | 236315041792 | 1267175424 | rw,relatime,errors=remount-ro |
| /media/jer/Files | /dev/sdb4 ext4 2283b135-bf37-42d6-921f-2fb861d0a8d4 | 498654666752 | 267268935680 | 205980205056 | rw,relatime |
| /media/jer/ZX20 | /dev/sdd2 exfat 18D6-896E | 512075235328 | 482487566336 | 29587668992 | rw,relatime, uid/gid 1000, fmask/dmask 0022, errors=remount-ro |
| /media/jer/Videos | /dev/sdb1 ext4 60e276b9-4abb-4325-a913-f4c43c14294b | 467298177024 | 381404110848 | 62081433600 | rw,relatime |

Other discovered partitions: sda1 EFI vfat 512 MiB; sdb2 vfat 488 MiB unmounted; sdb3 swap 14.9 GiB; sdd1 16 MiB, no filesystem shown; sdc reports 0 B. No mount attempted. Videos ownership for these projects was not established; no broad scan. Unavailable external/cloud volumes and Windows destination are UNKNOWN.

Selected allocated bytes from `du -x -B1`: CCF shared Git 4268265472; CCF Builds 3053748224; Main Integration Library 7216824320 (apparent 3385693233); #382 qualifier Library 7012089856; U1 Library 7250771968; U1 Build 1944190976; U3 Library 7270694912; UIQualification Library 7210270720; T3 node_modules 3831660544; T3 live userdata 5842542592; dashboard checkout 4534272. Child measurements overlap parent roots; no summation is valid. ExFAT allocation granularity explains material disparity between logical and allocated sizes. Measurements traversed live files and can change.

Conservative/likely/upper recoverable on each volume: UNKNOWN (no candidate has satisfied ownership, dependency and recovery proofs). Confirmed available-to-delete amount: zero. Unknown is not a claim that no space could eventually be recovered. Windows transfer volume UNKNOWN: inclusion set, source consistency and destination layout are unresolved. System `/` is critically constrained; do not stage there. No assumption about freeing storage on Files or root improving ZX20 free space.

The checked-in StorageArchitecture document contains historical device names; identify drives by current UUID/mount, not those old names. Linux paths, case, modes, links, long paths, exFAT's lack of POSIX permissions/symlinks, and SQLite locking require a Windows/WSL-specific plan. No Windows import or restoration test was run.
