# Deployment provenance

`deployment.json` schema 2 audits the existing live deployment, with a separate
`candidate` receipt for this branch. It records observations, not deployment or
source-review approval. Only the provenance file is being published to live;
the candidate tools and this verifier are not installed there.

`source_hashes` contains whole-file SHA-256 hashes verified against the named
historical source commit. Immutable files that differ from that source are
listed in `deployed_overrides` with their observed SHA-256 and a reason; they
are still checked, and their capture does not certify or silently import them
into candidate source. Previous hashes are retained where known.

`live_state` gives a reason for each excluded path: the operational board,
ledger, state, lane registrations, and derived Python bytecode cache. Only the
`lanes` member of `lanes.json` is mutable: `configuration_hashes` hashes the
remaining object as UTF-8 JSON with sorted keys, ASCII escaping and compact
separators. Consequently repository, device identity, Git stores, board path,
capacity floor and any new configuration keys remain checked. The branch's
four source lanes and live's sixteen registrations are expected operational
drift, not a request to copy live lane state into source.

The verifier accepts only these explicit live-state exclusions. It rejects
unclassified files and missing/changed immutable files. `deployment.json`
itself and timestamped `deployment.json.bak-YYYYMMDDTHHMMSSZ` backups are audit
metadata, excluded to avoid self-hashing; review their changes in Git and use
the recorded before/after hashes. A SHA-256 receipt detects accidental drift,
not hostile alteration of both the receipt and files.

Run from the source `CCF-Ops/` directory (Python standard library only):

```bash
python3 -B scripts/verify-deployment --root /media/jer/ZX20/CCF-Ops
python3 -B scripts/verify-deployment --root /media/jer/ZX20/CCF-Ops --candidate-root .
```

The first command can also use `--manifest /media/jer/ZX20/CCF-Ops/deployment.json`.
The verifier reads files only, imports no operational helper, never launches
Unity and never acquires a lock. PASS is exit 0; drift is exit 1; an invalid
receipt, invalid JSON or a filesystem error is exit 2. The optional candidate
check validates the narrow source-file set at its separately named source
commit, rather than pretending this branch is already deployed.

Round 2 ports repository/board admission for the documented Files CCF tree,
the two migrated Git-store paths, and durable non-Unity inventory. The candidate
retains its persistent default device pin. Explicit off-storage inventory
permission is restricted to non-Unity lanes inside the Files CCF tree; it is
not permission for arbitrary paths, and default callers remain ZX20-only.
These intentional differences from live are inventoried in the task result.
No live lane registrations or board content are imported. Candidate hashes
also cover the unchanged lock, linter and self-test so the tested scripts and
the inherited test-device assumptions remain identifiable.

When updating provenance, inspect every immutable difference first. Do not
make an audit green by excluding source code, rehashing unexplained drift or
copying board, ledger, state or live registrations into Git. Source acceptance,
runtime verification and dashboard publication remain separate gates.
