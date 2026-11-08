#!/usr/bin/env python3
"""Focused tests for N-1 (#343): the proposed unity-lock must not believe a Git tree listing whose objects fail their content hash.
SOURCE PROPOSAL ONLY: disposable fixture repositories under CCF_LEASE_TEST_TMP; the real Git store, deployed script, lanes, leases and Unity are never touched.

Run:  CCF_LEASE_TEST_TMP=<existing scratch dir on the Files disk> python3 -I -B test_tree_object_integrity.py -v
Old-source red run:  CCF_LEASE_TEST_PROPOSED=<unity-lock.proposed from 6b111b12> ... test_tree_object_integrity.py -v ForgedObjects

ForgedObjects   black-box through main(), same shape as test_raw_byte_admission: admit the honest fixture, release, then forge object bytes
                IN PLACE (the object keeps its id, so rev-parse HEAD and every ref are unchanged) so `git ls-tree` lists attacker-chosen
                blobs, make unrelated working bytes and the index agree with that forged listing, and require exit 2 with no lock and no
                exact-admission event. RED on the old source, GREEN now.
GraphBehaviour  verified_head_inventory directly: honest/packed positives, missing object, unsafe names, bounds.
"""
import hashlib, importlib.util, os, subprocess, sys, unittest, zlib
from pathlib import Path

sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
_spec = importlib.util.spec_from_file_location('test_raw_byte_admission', HERE / 'test_raw_byte_admission.py')
R = importlib.util.module_from_spec(_spec); sys.modules['test_raw_byte_admission'] = R; _spec.loader.exec_module(R)
T, git, OTHER = R.T, R.git, R.OTHER
FORGED_TEXT = b'class ForgedSource {}\n'


def loose_path(store, oid): return Path(store) / 'objects' / oid[:2] / oid[2:]

def forge_loose(store, oid, kind, payload):
    """Overwrite the loose object stored under `oid` with different (valid-looking) content. The old file is replaced, not edited in place,
    because a fixture clone hard-links objects with its seed repository."""
    f = loose_path(store, oid); assert f.exists(), 'fixture object is not loose: ' + oid
    f.unlink(); f.write_bytes(zlib.compress(kind + b' %d\0' % len(payload) + payload)); f.chmod(0o444)

def tree_entries(raw, rawlen=20):
    out, i = [], 0
    while i < len(raw):
        sp = raw.index(b' ', i); nul = raw.index(b'\0', sp)
        out.append((raw[i:sp], raw[sp+1:nul], raw[nul+1:nul+1+rawlen])); i = nul+1+rawlen
    return out

def tree_bytes(entries): return b''.join(m + b' ' + n + b'\0' + o for m, n, o in entries)

def cat(repo, kind, rev): return git(repo, 'cat-file', kind, rev).stdout

def hash_in(store, kind, payload):
    p = subprocess.run(['git', '-C', str(store), 'hash-object', '-w', '--literally', '-t', kind, '--stdin'], input=payload, capture_output=True, check=True, env={**os.environ, **T.GIT_ENV})
    return p.stdout.decode().strip()


class ForgedObjects(R.RawBase):
    def drift(self):
        """Unrelated source drift that Git itself is told is the committed content (index updated, status clean)."""
        self.lane_file(OTHER).write_bytes(FORGED_TEXT); git(self.fx.lane, 'add', OTHER)
        return git(self.fx.store, 'hash-object', '-w', str(self.lane_file(OTHER))).stdout.decode().strip()

    def assertGitBelievesTheForgery(self, oid):
        self.assertEqual(git(self.fx.lane, 'rev-parse', 'HEAD').stdout.decode().strip(), self.fx.head, 'precondition: displayed HEAD is still the pin')
        self.assertIn(oid, git(self.fx.lane, 'ls-tree', '-r', 'HEAD', OTHER).stdout.decode(), 'precondition: ls-tree lists the forged blob')
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'Other.cs'), 0, 'precondition: Git status is clean for the drifted file')
        self.assertEqual(git(self.fx.lane, 'diff-index', '--cached', '--name-only', 'HEAD', '--').stdout, b'', 'precondition: index agrees with the forged HEAD')

    def forged_assets_tree(self, blob):
        tid = git(self.fx.lane, 'rev-parse', 'HEAD:Assets').stdout.decode().strip()
        entries = [(m, n, bytes.fromhex(blob) if n == b'Other.cs' else o) for m, n, o in tree_entries(cat(self.fx.lane, 'tree', tid))]
        return tid, tree_bytes(entries)

    # ---- the reviewer's N-1 shape: forge the loose subtree that holds the drifted file ----------------------------
    def test_N1_forged_loose_subtree_object(self):
        self.admit_then_release()
        blob = self.drift(); tid, forged = self.forged_assets_tree(blob)
        forge_loose(self.fx.store, tid, b'tree', forged)
        self.assertGitBelievesTheForgery(blob)
        self.assertFailClosedAfterControl(self.exact(), 'does not match its object id')

    # ---- independently varied: a different subtree (Tools/), a different file and a different forged blob ---------------------
    def test_N1_forged_second_subtree_object(self):
        self.admit_then_release()
        p = self.lane_file('Tools/old.py'); p.write_bytes(b'x=9\n'); git(self.fx.lane, 'add', 'Tools/old.py')
        blob = git(self.fx.store, 'hash-object', '-w', str(p)).stdout.decode().strip()
        tid = git(self.fx.lane, 'rev-parse', 'HEAD:Tools').stdout.decode().strip()
        entries = [(m, n, bytes.fromhex(blob) if n == b'old.py' else o) for m, n, o in tree_entries(cat(self.fx.lane, 'tree', tid))]
        forge_loose(self.fx.store, tid, b'tree', tree_bytes(entries))
        self.assertIn(blob, git(self.fx.lane, 'ls-tree', '-r', 'HEAD', 'Tools/old.py').stdout.decode(), 'precondition: ls-tree lists the forged blob')
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'old.py'), 0, 'precondition: Git status is clean for the drifted file')
        self.assertFailClosedAfterControl(self.exact(), 'does not match its object id')

    # ---- ROOT tree and HEAD commit forgeries. Measured finding: Git's own parse_object() re-hashes these two, so they already ---------
    # ---- failed closed on the old source (green before and after); the N-1 gap is every tree BELOW the root. Kept as defence in depth.
    def test_N1_forged_root_tree_points_at_alternate_subtree(self):
        self.admit_then_release()
        blob = self.drift(); tid, alt = self.forged_assets_tree(blob)
        alt_id = hash_in(self.fx.store, 'tree', alt)        # a genuine, self-consistent tree object: only the ROOT forgery is dishonest
        root = git(self.fx.lane, 'rev-parse', 'HEAD^{tree}').stdout.decode().strip()
        entries = [(m, n, bytes.fromhex(alt_id) if n == b'Assets' else o) for m, n, o in tree_entries(cat(self.fx.lane, 'tree', root))]
        forge_loose(self.fx.store, root, b'tree', tree_bytes(entries))
        self.assertFailClosedAfterControl(self.exact())

    # ---- forge the pinned HEAD COMMIT so it names a different root tree ---------------------------
    def test_N1_forged_head_commit_names_alternate_root_tree(self):
        self.admit_then_release()
        blob = self.drift(); tid, alt = self.forged_assets_tree(blob)
        alt_assets = hash_in(self.fx.store, 'tree', alt)
        root = git(self.fx.lane, 'rev-parse', 'HEAD^{tree}').stdout.decode().strip()
        entries = [(m, n, bytes.fromhex(alt_assets) if n == b'Assets' else o) for m, n, o in tree_entries(cat(self.fx.lane, 'tree', root))]
        alt_root = hash_in(self.fx.store, 'tree', tree_bytes(entries))
        commit = cat(self.fx.lane, 'commit', 'HEAD'); forged = commit.replace(b'tree ' + root.encode(), b'tree ' + alt_root.encode(), 1)
        self.assertNotEqual(commit, forged)
        forge_loose(self.fx.store, self.fx.head, b'commit', forged)
        self.assertFailClosedAfterControl(self.exact())

    # ---- an object that was simply deleted is also a refusal, not a skip ----------------------------------------------
    def test_missing_tree_object_refused(self):
        self.admit_then_release()
        loose_path(self.fx.store, git(self.fx.lane, 'rev-parse', 'HEAD:Tools').stdout.decode().strip()).unlink()
        self.assertFailClosedAfterControl(self.exact())

    # ---- positive controls: honest objects, loose or packed, still admit (and record the verification cost) -------------------
    def test_honest_loose_objects_admit_and_report_graph_stats(self):
        self.assertAccepted(self.exact())
        import json
        ev = [e for e in self.fx.history('prop') if e['event'] == 'exact-admission'][-1]
        proof = ev['inspection']['raw_proof']
        self.assertGreaterEqual(proof['verified_trees'], 3); self.assertEqual(proof['head_tree'], git(self.fx.lane, 'rev-parse', 'HEAD^{tree}').stdout.decode().strip())

    def test_honest_packed_objects_admit(self):
        git(self.fx.store, 'repack', '-a', '-d', '-q'); git(self.fx.store, 'prune-packed')
        self.assertFalse(loose_path(self.fx.store, self.fx.head).exists(), 'precondition: objects are packed')
        self.assertAccepted(self.exact())


class GraphBehaviour(R.RawBase):
    def inv(self, head=None):
        head = head or self.fx.head
        return self.prop.verified_head_inventory(self.fx.lane, head, 'sha1')

    def test_inventory_matches_ls_tree_for_honest_graph(self):
        root, inventory, stats = self.inv()
        listed = self.prop.parse_ls_tree(git(self.fx.lane, 'ls-tree', '-r', '-l', '-z', '--full-tree', 'HEAD').stdout)
        self.assertEqual(inventory, {p: (m, o) for p, (m, o, n) in listed.items()})
        self.assertEqual(root, git(self.fx.lane, 'rev-parse', 'HEAD^{tree}').stdout.decode().strip())

    def commit_with_root(self, entries):
        root = hash_in(self.fx.store, 'tree', tree_bytes(entries))
        commit = hash_in(self.fx.store, 'commit', b'tree %s\nauthor t <t@e> 1 +0000\ncommitter t <t@e> 1 +0000\n\nx\n' % root.encode())
        return commit

    def test_unsafe_names_and_modes_refused_even_when_hash_valid(self):
        blob = bytes.fromhex(git(self.fx.lane, 'rev-parse', 'HEAD:Assets/Other.cs').stdout.decode().strip())
        for entry, text in (((b'100644', b'..', blob), 'unsafe'), ((b'100644', b'.GIT', blob), 'unsafe'), ((b'100644', b'a/b', blob), 'unsafe'),
                            ((b'160000', b'sub', blob), 'unsupported')):
            with self.subTest(entry=entry), self.assertRaisesRegex(ValueError, text):
                self.inv(self.commit_with_root([entry]))

    def test_repeated_name_refused(self):
        blob = bytes.fromhex(git(self.fx.lane, 'rev-parse', 'HEAD:Assets/Other.cs').stdout.decode().strip())
        with self.assertRaisesRegex(ValueError, 'repeated'): self.inv(self.commit_with_root([(b'100644', b'a', blob), (b'100644', b'a', blob)]))

    def test_wrong_object_type_refused(self):
        blob_id = git(self.fx.lane, 'rev-parse', 'HEAD:Assets/Other.cs').stdout.decode().strip()
        with self.assertRaisesRegex(ValueError, 'expected a commit'): self.inv(blob_id)

    def test_entry_bound_stops_hash_valid_expansion(self):
        self.prop.MAX_GRAPH_ENTRIES = 3
        with self.assertRaisesRegex(ValueError, 'too many'): self.inv()

    def test_object_bound_refuses(self):
        self.prop.MAX_GRAPH_OBJECTS = 1
        with self.assertRaisesRegex(ValueError, 'too many'): self.inv()


if __name__ == '__main__': unittest.main()
