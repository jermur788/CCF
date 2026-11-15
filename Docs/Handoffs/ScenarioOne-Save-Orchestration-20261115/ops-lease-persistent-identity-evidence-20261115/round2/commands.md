# Round 2 repeatable commands

Run from the task repository root unless `CCF-Ops/` is specified.

```bash
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/round2/verify-storage-boundaries.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/round2/regression-probes.json
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/verify-identity.py CCF-Ops/scripts CCF-Ops/lanes.json
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/continuation/prove-provenance.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/round2/provenance-probes.json
python3 -B CCF-Ops/scripts/verify-deployment --root /media/jer/ZX20/CCF-Ops --manifest /media/jer/ZX20/CCF-Ops/deployment.json --candidate-root CCF-Ops
git diff --check
```

Required gates, working directory `CCF-Ops/`:

```bash
python3 -B tests/self-test.py
python3 -B scripts/packet-lint --board board/active.md
python3 -B scripts/packet-lint tests/fixtures/valid.md
```

Inspected self-test unchanged: passes 21 packet/board checks then stops at the
stale `/dev/sda2` fixture. It never reaches its isolated force tests. Do not
re-run past a repaired fixture under this no-force packet without resolving
that authority first. Its own stopped fixture directory was inventoried and
removed only after confirming no guard/history/lock existed. Boundary probes
use no acquisitions and create no negative state directory. Provenance probes
copy checked scripts/config only, never board/ledger/state; scratch tampering
is confined to Files and removed. Live changes are provenance plus backup only.

Full stdout/stderr/exits are preserved separately in the JSON receipts, without
filtering. Required gates are repeated at the final publication head and
published verbatim on #548; final receipts stay under the external round2
folder to avoid a self-referential evidence commit SHA.
