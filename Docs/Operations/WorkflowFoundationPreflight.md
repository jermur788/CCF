# Workflow foundation preflight

Inventory before setup: 2026-10-15T20:55:03.228622+00:00. Context/base: `c6dc136b7f1f8668d9f67f95f48968d290ad5067`.

## Verified storage and repository

`ZX20_ROOT=/media/jer/ZX20`; device `/dev/sda2`, label ZX20, UUID `18D6-896E`, exFAT. Initial available space: 60,003,057,664 bytes (55.88 GiB). `/media/jer/Files` is `/dev/sdc4`, **not ZX20**. Root SSD `/dev/sdb2` has about 4.7 GiB free.

The supplied working directory is a separate asset repository with no remote. CCF is positively identified by `https://github.com/jermur788/CCF.git`, canonical context files, and exact supplied context commit. Existing CCF common Git directory: `/home/jer/CCF/.git` (~3.8 GiB, SSD). Its primary checkout is `feature/forestry-ecology`, HEAD `a31ec62fc4e7b74d9a1d17b352f80df2cc819817`; it has unrelated dirty assets. The clean context worktree is `/media/jer/Files/CCF-verification-cost-policy-v1`, branch `docs/verification-cost-policy-v1`, supplied HEAD. Neither is safe for this implementation's writes under the new storage policy.

An independent full Git object/ref copy is therefore established at `$ZX20_ROOT/CCF-Repository.git` using local `clone --bare --no-hardlinks` (no alternates or dependency on the SSD object store); its origin is reset to the CCF remote. This is a new repository copy, not a migration. Old branches/worktrees remain untouched. Do not merge or repoint accepted feature branches. The task is based on the supplied context, not on a guessed final candidate.

Before creation, all nine new absolute targets below were absent, resolved below `/media/jer/ZX20`, and their nearest existing parent had the ZX20 device number. `findmnt -T` independently resolved each parent to `/dev/sda2 exfat /media/jer/ZX20`. No target had a worktree registration, branch, HEAD or Library:

| Path | Before | Reusable / safe to create | Planned purpose |
|---|---|---|---|
| /media/jer/ZX20/CCF-U1 | absent, 0 bytes | yes | primary Unity, cold initially |
| /media/jer/ZX20/CCF-U2 | absent, 0 bytes | yes | secondary Unity, cold initially |
| /media/jer/ZX20/CCF-Integration | absent, 0 bytes | yes | clean integration |
| /media/jer/ZX20/CCF-NonUnity | absent, 0 bytes | yes | offline tools/tests/docs |
| /media/jer/ZX20/CCF-Ops | absent, 0 bytes | yes | deployed operational files |
| /media/jer/ZX20/CCF-Builds | absent, 0 bytes | yes | future Player builds/archives |
| /media/jer/ZX20/CCF-Verification | absent, 0 bytes | yes | receipts and large evidence |
| /media/jer/ZX20/CCF-Temp | absent, 0 bytes | yes | temporary data and caches |
| /media/jer/ZX20/CCF-Repository.git | absent, 0 bytes | yes | independent common Git store |

No warm existing worktree can be reused at the requested paths without moving existing material, which is outside this task. Existing ZX20 warm candidates have historical branches and/or dirty materials. No Library is copied/shared or created; warm-up requires a later authorised Unity gate. Estimated new footprint: ~3.8 GiB object store + three ~1.1 GiB full checkouts + sparse NonUnity/docs + exFAT allocation overhead; budget 10 GiB, excluding later Unity Libraries. Stop lane setup if free space falls below 15 GiB. exFAT has no Unix symlink/mode semantics; this context has no tracked symlinks. Git filemode/symlink handling is local to the new store. No filesystem/package/project change is made.

`TMPDIR`, `TMP`, `TEMP` are explicitly redirected to ZX20/CCF-Temp for all write/test/setup commands; cache output uses ZX20/CCF-Temp/cache. No Unity Editor is running (licensing service only). No cleanup is authorised/performed. The old active-tasks contract is inspected and preserved.

## Registered worktrees before setup

Dirty state is raw Git status in the external JSON receipt. Existing paths are read-only for this task. A clean Git status does not prove a task is closed, untracked evidence disposable, or work safe to move. Every registered worktree depends on `/home/jer/CCF/.git`; migration requires explicit worktree-aware planning. No existing worktree is approved for modification or relocation here. Candidate reuse requires Manager assignment and inspection of untracked/ignored evidence.

| Path | Branch | HEAD | Disk approx | Library | Live / reusable / safe to modify |
|---|---|---|---|---|---|
| /home/jer/CCF | refs/heads/feature/forestry-ecology | a31ec62fc4e7b74d9a1d17b352f80df2cc819817 | 5.1G | False | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-claude | refs/heads/task/crop-tree-competitor-reasoning-poststorm | 869ee92a983a1af5fc470392eccc7557fbe45def | 1.3G | False | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-completion-readiness | refs/heads/task/scenario-one-completion-readiness | 6a0e01f73965e2ddd3ea815aa23693284a1d29a0 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-context-sync | refs/heads/task/context-sync-stage-roadmap-manager-continuation | cd239d7889c8224dcd46591deefa3089249e94c9 | 997M | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-crop-tree-competitors | refs/heads/task/crop-tree-competitor-reasoning | 33e8d55df005ea970d542ffee8e8e2bacc7ab5d6 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-e80-calibration | refs/heads/task/enlarged80-area-calibration | 23f84f9ac0d4a02d385869d3985e11a24cc5a23f | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-final-presentation | refs/heads/task/scenario-one-rng-model1-default | 4eab59031503de55e2e4f6932920db90967ca323 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-forestry-expansion | refs/heads/task/forestry-simulator-expansion | 22d6b21b8da4bdff5ad46a1886a2849905f5709c | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-integration-80m | refs/heads/integration/scenario-one-80m | f4535caf606bca46f35f02408920408d6b9f20d3 | 1.1G | True | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-integration-e80-calibration | refs/heads/integration/enlarged80-area-calibration | e8c73f25e6d93db818abdc6c794887f8f7c5ba43 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-main | refs/heads/main | 402a2b41108dc9aa91e225db9047956b0a91a97e | 4.5G | True | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-openai | refs/heads/task/juvenile-mortality-foundation | 1319c2ce8d10ee4e19d2fff8c05e92b9c81c75a1 | 997M | False | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-opencode-economy | refs/heads/task/scenario-one-economy-loop | 0ccb013fbffa54fd8c26a568da89c79f7e98e8f9 | 1006M | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-p2-bimodal | refs/heads/task/p2-interactive-bimodal-investigation | 11c3596072a0422d5426291e370129d209c56342 | 1.1G | False | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-pedagogy | refs/heads/task/scenario-one-pedagogy-overnight | 60674f161e9ea9c8123993d0932aea4c7edf38d6 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-pedagogy-p1 | refs/heads/task/scenario-one-pedagogy-p1 | 1a36ea97c724b3a026e76a0b5432349b46e6a298 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-project-context-migration | refs/heads/task/project-context-migration | 42db4e906c7e595762c1e504aeea009206e71e29 | 997M | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-review-ecology | refs/heads/review/ecology-calibration-2632de6 | 2632de60b65c5e3f98c6792feccb1fcee5cb7095 | 999M | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-s1-80m | refs/heads/task/scenario-one-80m-stand | a309f99e0f2c6a770044604bbf88ee2e2471a5b5 | 1.1G | False | dirty; preserve live work; no writes/moves here |
| /home/jer/CCF-s1a | refs/heads/integration/s1-b-standalone-readiness | 11c3596072a0422d5426291e370129d209c56342 | 1.7G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-save-hardening | refs/heads/task/save-cell-index-hardening | 12cc8ef0659f54055088047c2c6727d874bdffd8 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-site-infrastructure | refs/heads/task/oak-beech-site-infrastructure-v1 | 47539847d7e2531824c30f3dfe635d55404699a8 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-startup-verify | refs/heads/task/scenario-one-startup-screen | b99ba6993b6b7cfb9fc3b8de6c4eda88abd8a348 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-windthrow | refs/heads/task/windthrow-readiness | 9a9f4fb749b1f4026319a6ff1ac43a7fded98fe9 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/CCF-work-plan-residual | refs/heads/task/work-plan-residual-stand | 634e4d806c7296935c23c085b9a3ff146b1565a3 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-clearance-preview | refs/heads/task/scenario-one-clearance-preview | 8c5380dd95b397281bedc9bd153a69086b9aad9f | 1.5G | False | dirty; preserve live work; no writes/moves here |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-context-save-hardening-publication | DETACHED | c1564b857972ee18917e8a461c457533a54cc3f1 | 592K | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-independent-review | refs/heads/task/post-scenario1-independent-review | 6f252e2fbb7153f137abe7d1fa829a9dcb3011b9 | 1.1G | False | clean; activity unconfirmed; no writes/moves here |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-menu-tutorial | refs/heads/task/scenario-one-menu-tutorial | b3fa29dd242842efb1f54ee7fed7dd3d044778a4 | 1.2G | False | dirty; preserve live work; no writes/moves here |
| /media/jer/Files/CCF-a1-density-e5cfce3 | DETACHED | c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2 | 4.3G | True | dirty; preserve live work; no writes/moves here |
| /media/jer/Files/CCF-context-save-hardening | refs/heads/docs/save-cell-index-hardening-context | c1564b857972ee18917e8a461c457533a54cc3f1 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-density-rework-v2 | refs/heads/task/mixed-density-architecture-rework-v2 | a7a759ded087a824c1e8149e480ddf58ac0585eb | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-density-v3 | refs/heads/task/mixed-density-architecture-v3 | 9c736dc52717302b39faf61c75da958c81d8c1af | 4.8G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-f10-triage-v1 | refs/heads/fix/integrated-f10-menu-v1 | 5d69b7bf58eb961bad3127432e5372b1d2bdb9f9 | 496K | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-final-scenario-one-candidate-v1 | refs/heads/integration/scenario-one-final-candidate-v2 | c20c6dc242e4463b3de72e6c2d06b4ea9fa1fbc2 | 111M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-integration-save-hardening | refs/heads/integration/save-cell-index-hardening | 12cc8ef0659f54055088047c2c6727d874bdffd8 | 4.2G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-integration-startup | refs/heads/integration/scenario-one-startup-screen | 5d57208a411404b65b5d9a0d4f2a6e88c4086400 | 4.2G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-next-wave-20261014/lane-b | refs/heads/task/oak-beech-ordinal-production-calibration-v1 | 796da99029190ef71e68d5fa722f25240e696318 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-next-wave-20261014/lane-c | refs/heads/task/scenario-one-two-cycle-completion-implementation-v1 | eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-next-wave-20261014/lane-d | refs/heads/audit/scenario-one-ux-followup-549eae3-review-v1 | bf1b7e06b030218dbae530df0aed3aa87cb0a280 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-oak-beech-reconciliation-v2 | refs/heads/research/oak-beech-adult-site-reconciliation-v2 | c3fa549e6ce68a8659a781904b50122ed2d968e6 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-ordinal-growth-v1 | refs/heads/research/oak-beech-ordinal-growth-calibration-v1 | c35e0016fe28b6302bb4e120642f3b917b14cb67 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a1 | refs/heads/task/scenario-one-two-cycle-guidance-v1 | a93a7220fd8b4d8246b3613d913360dad06988e8 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a2 | refs/heads/tools/scenario-one-verification-fixture-fixes-v1 | 4c5a8a4b80e2e1df4d343a2aebc21475d6f1401a | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a3 | refs/heads/audit/scenario-one-integration-inputs-v1 | a245c0617800c2f3b4564c234c462fe638ced1ec | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a4 | refs/heads/rehearsal/scenario-one-final-integration-v1 | 872deb3c74156aa22ce4ae9be2a2f1b3583bbfef | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a5 | refs/heads/docs/scenario-one-integrator-packet-v1 | 60394d766c7722fded762571baa4058ed4d34828 | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-overnight-sol-20261015/a6 | refs/heads/audit/scenario-one-canonical-staleness-v1 | a3dc3976e295b36495c50a27605051ef074d4b57 | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-perf-dense-stand-v1 | refs/heads/perf/scenario1-dense-stand-player-v1 | e788b09f97958fd0d416cf4f67d63e7ef599df77 | 15G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-player-performance-profiling | refs/heads/task/player-performance-profiling | c1564b857972ee18917e8a461c457533a54cc3f1 | 9.2G | True | dirty; preserve live work; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job1 | refs/heads/audit/three-species-integration-readiness-v1 | 26d2bf8dc75ca4f612ec8e10ab9303afa044919c | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job2 | refs/heads/audit/save-v20-compatibility-contract-v1 | f4d0a1cac5f2a41e357042df417bf10d3bc6c078 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job3 | refs/heads/research/oak-beech-growth-preregistration-v1 | 60cfd822916325e618dd8c51267db10d5cac2412 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job4 | refs/heads/tools/oak-beech-growth-calibration-lab-v1 | c79f710c1ed96192d43ddbc07e816899de6a2b29 | 115M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job5 | refs/heads/audit/scenario-one-demo-acceptance-matrix-v1 | 52dd59f0d5f7958ed5fc36f90a3f45f856ac38dc | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job6 | refs/heads/audit/scenario-one-integration-rehearsal-v1 | 6739b8b2284ff6900ca88b2d453d36c06672c286 | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job7 | refs/heads/audit/verification-runbook-v1 | 5f88da28a6bfbd5239c30c8fac945e5deb449d6b | 110M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job8 | refs/heads/docs/scenario-one-next-canonical-draft-v1 | cd22014c336311e21bf2b856b590031c32eaa54e | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-serial-sol-20261014/job9 | refs/heads/audit/non-unity-readiness-summary-v1 | 25d0601c98f5de5324cbd5da3a2471954093302d | 109M | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-stage-a-windows-pilot-candidate | refs/heads/task/stage-a-windows-pilot-candidate | c1564b857972ee18917e8a461c457533a54cc3f1 | 7.9G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-startup-natural-baseline | DETACHED | e8c73f25e6d93db818abdc6c794887f8f7c5ba43 | 4.2G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-tier2-smoke-bc81627 | DETACHED | bc81627e8ca47fb619f6fbabcbf618dba0508e11 | 4.5G | True | dirty; preserve live work; no writes/moves here |
| /media/jer/Files/CCF-tools-fixes | refs/heads/tools/verification-fixture-fixes-v1 | 3a788a52def754edd22e322f79f2faf3c63b6fa1 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-two-cycle-completion | refs/heads/task/scenario-one-two-cycle-completion | 157ea7d12f361b824f56405284d46312e7c0b9fc | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-u4-guidance | refs/heads/task/scenario-one-completion-guidance-v1 | ba48591d874d666e72d421d1d0e5ff4dcfe3cd71 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verification-cost-policy-v1 | refs/heads/docs/verification-cost-policy-v1 | c6dc136b7f1f8668d9f67f95f48968d290ad5067 | 244K | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-final-density-a395e3f | refs/heads/verify/final-density-a395e3f | 4c00dd89b5e9fdd4ab34cd406d698d8a082d84e7 | 4.5G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-final-density-v2-a7a759d | refs/heads/verify/final-density-v2-a7a759d | 81126ee093b0db345112af17bcb2842a95fe1009 | 4.7G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-oak-beech-796da99 | refs/heads/verify/oak-beech-production-calibration-796da99 | a0c21e0743212ab270543bcd38549334ff7a0c97 | 4.7G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-overnight-b4894cc | refs/heads/audit/scenario-one-minimal-completion-v1 | 643e75b7d76ae35689bc5035fa607f8b5a3de12c | 4.5G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-scenario-one-ux-84e881a | refs/heads/ux/scenario-one-small-fixes-84e881a | 549eae3318f1bb4d01a2853384b58f282f7f4e73 | 5.2G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/Files/CCF-verify-two-cycle-eb7fd3d | refs/heads/verify/u4-guidance-a1-smoke-a93a722 | 899709f5f968d64cebed3c961b8596e3a2f245a3 | 4.7G | True | clean; activity unconfirmed; no writes/moves here |
| /media/jer/ZX20/CCF-integration-model2 | refs/heads/integration/p3-poststorm | 0ab73994153d55d08adbd65f928926e6e9165f7c | 1.9G | False | dirty; preserve live work; no writes/moves here |
| /media/jer/ZX20/CCF-post-p3-context-refresh | refs/heads/integration/post-p3-context-publication | 65b03e453ea57957a7f1b06b1cf0d0a5e9df0b65 | 1.6G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/ZX20/CCF-storms-windthrow | refs/heads/task/storms-windthrow-v1 | 857150b2f20afa52d7b36be88a835f07a4ee5a51 | 2.1G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/ZX20/CCF-survival | refs/heads/feature/survival-gameplay | 0fc92ca989c0df7109e2507b2aab5712f0cf6077 | 1.6G | False | clean; activity unconfirmed; no writes/moves here |
| /media/jer/ZX20/CCF-understorey-recruitment | refs/heads/task/understorey-recruitment-causality | 902903ff6d9790c02bdd01e8cc6f0aab8c2f0774 | 2.0G | False | dirty; preserve live work; no writes/moves here |
| /tmp/opencode/CCF-docs-s1-parity | refs/heads/docs/scenario-one-full-demo-parity | 321791b8203bc709c57615fd4d6a19614d78db24 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /tmp/opencode/CCF-oak-beech-timber | refs/heads/research/oak-beech-timber-economy-v1 | b9d4cd939837263a1a11a9768252a394e62b6fa0 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /tmp/opencode/CCF-overnight-review | refs/heads/research/overnight-branch-review-matrix | f5ead23f6ab4f19d71fbd355bef54816d5f738e0 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /tmp/opencode/CCF-s1-fullloop | refs/heads/research/scenario1-full-loop-audit | 9f3fb748b0e576b97aa94e1ccbed0d2eee471ac8 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /tmp/opencode/CCF-s1-teaching | refs/heads/research/scenario1-full-loop-teaching | 321791b8203bc709c57615fd4d6a19614d78db24 | 1.2G | False | clean; activity unconfirmed; no writes/moves here |
| /tmp/opencode/parity-audit | DETACHED | c1564b857972ee18917e8a461c457533a54cc3f1 | 1.2G | False | dirty; preserve live work; no writes/moves here |

## Other existing material (includes main-SSD development data)

Approximate allocation from `du -sh`; no files created in these paths. Live status is unknown unless noted in registered inventory above. None is certified safely movable; Git markers or registry membership require worktree-aware migration. Paths without a marker may still contain receipts/assets referenced by canonical Git. Full dependencies are not audited by this foundation task; preserve all. `/tmp/opencode` is on the main SSD (~6.7 GiB); its six registered worktrees include dirty `parity-audit`. Treat them as potentially live, not confirmed stale. No pruning/deletion/moving occurs. Full branch inventory and raw statuses: `/media/jer/ZX20/CCF-Verification/WorkflowFoundation/{preflight.json,branches-before.txt,worktrees-before.txt}`.

| Path | Approx disk | Device/mount | Git marker | Live / movable / canonical dependency |
|---|---|---|---|---|
| /home/jer/Documents/ChatGPT/Local Game Dev/Asset_Creation_Inventory.json | 44K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Astra_Crop_Tree_Pruning_Brief.md | 8.0K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Beech_Mature_01 | 109M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Beech_Mature_01.zip | 46M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Bilberry_Type_Cover_01 | 11M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Blender_session_backup.blend | 908K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Broadleaf_Logs_01 | 45M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-80m-Publication-f4535ca | 248K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Combined-NonUnity-20261014 | 260K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Enlarged80-AreaCalibration-23f84f9 | 16K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Enlarged80-Integration-e8c73f2 | 9.3M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Overnight-Qualification-b4894cc | 4.0M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Player-Performance-c1564b8 | 45M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Post-P3-Context-2c0f7cd | 640K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-PostStorm-Context-341ccbf | 360K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-S1A-Context-4a68d52 | 552K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-S1A-Integration-c01de2d | 96K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-S1B-Context-8bed399 | 344K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-S1B-Setup | 168K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Cell-Hardening-Final | 24M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Cell-Hardening-Stop | 644K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Hardening-Context-c1564b8 | 1.3M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Hardening-Integration-Final | 5.8M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Save-Hardening-Integration-Stop | 4.2M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Stage-A-B07-c1564b8 | 5.0M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Startup-Integration-5d57208 | 824K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Startup-Verification | 48K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-Storm-Integration-58d6b0b | 500K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-birch | 3.1G | /dev/sdb2 ext4   / | True | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/CCF-storage-cleanup-20261010.json | 4.0K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Crop_Tree_Pruning | 2.0G | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Deadwood_Fungi_01 | 35M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Delivery_Coverage_2026_09_29.json | 12K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Forest_Asset_Tools | 75M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Forest_Floor_Detail | 29M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Forest_Floor_Detail_Handoff_2026_09_29 | 6.5M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Forestry_Ground_Additions | 109M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Ground_Moss_Flat_01 | 58M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Ground_Moss_Natural_02 | 66M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Ground_Moss_Shoots_03 | 82M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/HANDOFF_6_1_SOL_ASSET_CREATION.md | 20K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Handoffs | 480K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/HelpEscapeP3Review-2026-10-09 | 2.5M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Legacy_Portability_2026_09_29.json | 20K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/OakBeech-Reconciliation-Evidence | 1.1M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/References | 5.6M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_Bent_Refined_Set | 124M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_BrokenTop_Set | 74M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_BrokenTop_Set.zip | 61M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_Cavity_Refined_Set | 98M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_CoarseBranch_Set | 81M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_Forked_Set | 76M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/SS_Forked_Set.zip | 63M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sessile_Oak_Three_Stages | 157M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sessile_Oak_Three_Stages.zip | 130M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Bent_Variants | 60M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Bent_Variants.zip | 55M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Hollow_Variants | 71M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Hollow_Variants.zip | 57M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Mature_01 | 31M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Mature_01.zip | 20M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Mature_Benchmark_01 | 138M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Plantation_02 | 261M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Pole_01 | 28M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Pole_01.zip | 21M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_RingBarked_01 | 77M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Seedling_01 | 21M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Sitka_Seedling_01.zip | 20M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/StormWindthrowRecovery-2026-10-08 | 30M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Tree_02_Bluepool_Forked.png | 1.5M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Tree_Prototype_01_Improved.png | 1.2M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Tree_Variation_Additions | 253M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Tree_Variation_Additions.zip | 180M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Unity-cache-cleanup-2026-10-11 | 188K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/UnityTreeAssets | 30M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/UnityTreeAssets.zip | 30M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Vegetation_Treatment_States_01 | 64M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Verification | 689M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Woodland_Assets | 89M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Woodland_Assets.zip | 74M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Woodland_Forbs_01 | 12M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/Documents/ChatGPT/Local Game Dev/Woodland_Grass_Rush | 13M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/CCF-s1b | 1.4G | /dev/sdb2 ext4   / | True | unknown / not certified / inspect before migration |
| /home/jer/CCF-survival | 0 | /dev/sda2 exfat  /media/jer/ZX20 | True | unknown / not certified / inspect before migration |
| /home/jer/CCF_Tree_Broadleaf_Ancient_07 | 636K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/CCF_Tree_Broadleaf_Ancient_09 | 636K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /home/jer/CCF_Tree_Broadleaf_Ancient_10 | 708K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/apc.txt | 12K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-fixtures | 312K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-growth.json | 4.0K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-n3 | 1.3M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-offline | 2.7M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-site | 412K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-types | 284K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate-v2-ux | 452K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/candidate_v2_driver.py | 8.0K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/ccf-fairness-repair | 1.4M | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/irish-econ.txt | 104K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/msd.txt | 20K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/sep.txt | 96K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /tmp/opencode/ssd.txt | 36K | /dev/sdb2 ext4   / | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-candidate-v2-qualification | 43M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-college-strand-a-20261015-out | 13M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-e80-calibration-local-output | 3.4G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-f10-triage-20261015-out | 180K | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-integration-80m-local-output | 3.6G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-integration-e80-calibration-local-output | 3.4G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-next-wave-20261014 | 335M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-overnight-sol-20261015 | 655M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-root-space-preservation-20261010 | 2.4G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-save-hardening-local-output | 3.2G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-serial-sol-20261014 | 1001M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-startup-verify-local-output | 3.3G | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-tier2-smoke-20261015-out | 355M | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/Files/CCF-verification-cost-policy-publication | 948K | /dev/sdc4 ext4   /media/jer/Files | False | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-Media | 2.5G | /dev/sda2 exfat  /media/jer/ZX20 | False | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-Repository.git | 4.2G | /dev/sda2 exfat  /media/jer/ZX20 | False | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-integration-s1a | 2.2G | /dev/sda2 exfat  /media/jer/ZX20 | True | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-p2-bimodal-local-output | 8.4M | /dev/sda2 exfat  /media/jer/ZX20 | False | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-s1-80m-local-output | 1.4G | /dev/sda2 exfat  /media/jer/ZX20 | False | unknown / not certified / inspect before migration |
| /media/jer/ZX20/CCF-s1b-local-output | 6.0G | /dev/sda2 exfat  /media/jer/ZX20 | False | unknown / not certified / inspect before migration |

## Follow-up storage assessment supplied by user

The user identified `/` (`/dev/sdb2`, about 98% used) as the pressure point, not `/media/jer/Files`. Proposed separate cleanup candidates include thirteen older `/home/jer/CCF-*` checkouts and `~/.config/unityhub/downloads/Unity-6000.6.0f1.tar.xz`, then a branch/status-reviewed group including `/tmp/opencode`, older integration/tool checkouts, three Local Game Dev worktrees and the separate CCF-birch clone. These are **candidates, not cleanup performed or independently certified safe here**. Fresh status, ignored/untracked evidence, live processes, remote containment and canonical Git dependencies must be checked in the separately authorised cleanup task. This inventory found untracked Build/Library in `/home/jer/CCF-s1-80m` and `/home/jer/CCF-integration-80m`, dirty/untracked content in CCF-p2-bimodal and other preserved dirty material. Plain `git worktree remove` may correctly refuse such paths. Do not force removal or prune ambiguous registrations. The user explicitly reports no deletion performed. Foundation setup neither requires nor authorises that cleanup.
