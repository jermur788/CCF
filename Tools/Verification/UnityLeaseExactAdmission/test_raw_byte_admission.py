#!/usr/bin/env python3
"""Targeted offline tests for the #340 repair of F1-F3 in the proposed unity-lock exact-dirty admission.
SOURCE PROPOSAL ONLY: nothing here is deployed, and nothing touches the real registered lane, a real lease, Unity or ZX20.

Run:  CCF_LEASE_TEST_TMP=<existing scratch dir on the Files disk> python3 -I -B test_raw_byte_admission.py -v
Old-source red run:  CCF_LEASE_TEST_PROPOSED=<old unity-lock.proposed> ... test_raw_byte_admission.py -v FalseAcceptReproductions

It reuses the disposable Git store + lane + fake lock fixtures of test_exact_admission.py (loaded by path, because -I removes the
script directory from sys.path). Two groups:

  FalseAcceptReproductions  black-box, through main(): the three #339 false accepts (F1 replace refs, F2 clean filter, F3 stat
                            cache) plus close variants. Each admits an uncontaminated fixture first (positive control), changes
                            only UNRELATED raw working bytes (or the effective object graph), then requires true non-acquisition:
                            exit 2, no lock.json, no exact-admission event. These must be RED on the old source and GREEN now.
  RawProofBehaviour         the repaired internals: canonical object graph, raw tracked-byte proof, symlink/special/untracked/
                            ignored handling, Git errors, no filters, no index/stat trust, cost counters.
"""
import hashlib, importlib.util, json, os, shutil, signal, subprocess, sys, unittest
from pathlib import Path

sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
_spec = importlib.util.spec_from_file_location('test_exact_admission', HERE / 'test_exact_admission.py')
T = importlib.util.module_from_spec(_spec); sys.modules['test_exact_admission'] = T; _spec.loader.exec_module(T)
git, sha, run, PATHS = T.git, T.sha, T.run, T.PATHS
OTHER = 'Assets/Other.cs'


class RawBase(T.Base):
    def lane_file(self, rel): return self.fx.lane / rel

    def history_events(self): return [e['event'] for e in self.fx.history('prop')]

    def assertFailClosed(self, result, text=None):
        """True non-acquisition: exit 2 with an error, no lock record, and no exact-admission event or audit record."""
        code, out, err = result
        self.assertEqual(code, 2, (out, err)); self.assertIn('ERROR: ', err)
        if text: self.assertIn(text, err)
        self.assertFalse(self.fx.lock_json('prop').exists(), 'a refused request must not create a lock')
        self.assertNotIn('exact-admission', self.history_events(), 'a refused request must not record an admission event')

    def admit_then_release(self):
        """Positive control: the uncontaminated fixture admits, and the fake lease is released again."""
        self.assertAccepted(self.exact())
        lock = json.loads(self.fx.lock_json('prop').read_text())
        r = run(self.prop, ['release', '--owner', 'tester', '--task-id', 'T-FIXTURE', '--lock-id', lock['lock_id']])
        self.assertEqual(r[0], 0, r)
        self.assertFalse(self.fx.lock_json('prop').exists())
        self.fx_events_before = self.history_events()

    def assertFailClosedAfterControl(self, result, text=None):
        code, out, err = result
        self.assertEqual(code, 2, ('FALSE ACCEPT', out, err)); self.assertIn('ERROR: ', err)
        if text: self.assertIn(text, err)
        self.assertFalse(self.fx.lock_json('prop').exists(), 'a refused request must not create a lock')
        self.assertEqual(self.history_events(), self.fx_events_before, 'a refused request must not add history')


# ===================================================================================== F1 / F2 / F3 reproductions
class FalseAcceptReproductions(RawBase):
    # ---- F1: Git replacement refs -------------------------------------------------------------------------------
    def test_F1_replace_ref_substitutes_pinned_head_tree(self):
        self.admit_then_release()
        original = git(self.fx.lane, 'rev-parse', 'HEAD').stdout.decode().strip()
        p = self.lane_file(OTHER); p.write_text('class ReplacedHeadSource {}\n')
        (self.fx.seed / OTHER).write_bytes(p.read_bytes()); git(self.fx.seed, 'add', OTHER); git(self.fx.seed, 'commit', '-q', '-m', 'replacement tree')
        git(self.fx.store, 'fetch', '-q', str(self.fx.seed), 'main')
        replacement = git(self.fx.seed, 'rev-parse', 'HEAD').stdout.decode().strip()
        git(self.fx.lane, 'replace', original, replacement); git(self.fx.lane, 'add', OTHER)
        self.assertEqual(git(self.fx.lane, 'rev-parse', 'HEAD').stdout.decode().strip(), original, 'precondition: displayed HEAD is still the pin')
        self.assertFailClosedAfterControl(self.exact())

    # ---- F2: repository-local clean filters / normalisation --------------------------------------------------------
    def test_F2_local_clean_filter_normalises_unrelated_drift(self):
        self.admit_then_release()
        (self.fx.store / 'info' / 'attributes').write_text(OTHER + ' filter=reviewhide\n')
        git(self.fx.lane, 'config', 'filter.reviewhide.clean', "printf 'class Other {}\\n'")
        self.lane_file(OTHER).write_text('class UnapprovedSourceDrift {}\n')
        git(self.fx.lane, 'add', OTHER)
        self.assertEqual(git(self.fx.lane, 'diff-index', '--cached', '--name-only', 'HEAD', '--').stdout, b'', 'precondition: no staged content differs')
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'Other.cs'), 0, 'precondition: ordinary Git status is fooled')
        self.assertFailClosedAfterControl(self.exact())

    def test_F2_autocrlf_normalises_crlf_working_bytes(self):
        self.admit_then_release()
        git(self.fx.lane, 'config', 'core.autocrlf', 'true'); git(self.fx.lane, 'config', 'core.safecrlf', 'false')
        self.lane_file(OTHER).write_bytes(b'class Other {}\r\n'); git(self.fx.lane, 'add', OTHER)   # fixture-only: Git normalises CRLF to the same blob
        self.assertEqual(git(self.fx.lane, 'diff-index', '--cached', '--name-only', 'HEAD', '--').stdout, b'', 'precondition: no staged content differs')
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'Other.cs'), 0, 'precondition: ordinary Git status is fooled')
        self.assertFailClosedAfterControl(self.exact())

    # ---- F3: non-racy same-size stat-cache drift ---------------------------------------------------------------------
    def test_F3_same_size_edit_with_restored_mtime_under_permissive_stat_config(self):
        self.admit_then_release()
        git(self.fx.lane, 'config', 'core.trustctime', 'false'); git(self.fx.lane, 'config', 'core.checkStat', 'minimal')
        p = self.lane_file(OTHER); os.utime(p, ns=(10_000_000_000, 10_000_000_000))
        git(self.fx.lane, 'update-index', '--refresh', check=False)   # fixture-only: record this unchanged file's stat
        st = p.stat(); old = p.read_bytes(); new = old.replace(b'Other', b'Drift'); self.assertEqual(len(old), len(new)); self.assertNotEqual(old, new)
        p.write_bytes(new); os.utime(p, ns=(st.st_atime_ns, st.st_mtime_ns))
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'Other.cs'), 0, 'precondition: ordinary Git status is fooled')
        self.assertFailClosedAfterControl(self.exact())

    # ---- one ordinary unrelated raw change must still be refused (sentinel; green on old and new) -------------------
    def test_sentinel_plain_unrelated_raw_change_refused(self):
        self.admit_then_release()
        self.lane_file(OTHER).write_text('plain unrelated edit\n')
        self.assertFailClosedAfterControl(self.exact(), 'not exactly the approved set')


class GraftOnPostFixture(RawBase):
    state = 'post'
    def test_F1_info_grafts_forges_required_ancestry(self):
        self.admit_then_release()
        empty = git(self.fx.store, 'hash-object', '-t', 'tree', '-w', '/dev/null').stdout.decode().strip()
        unrelated = git(self.fx.store, 'commit-tree', empty, '-m', 'unrelated').stdout.decode().strip()
        self.fx.profile['states']['post-transition']['must_descend_from'] = [unrelated]
        self.assertFailClosedAfterControl(self.exact(), 'does not descend from')          # honest history: refused
        (self.fx.store / 'info').mkdir(exist_ok=True)
        (self.fx.store / 'info' / 'grafts').write_text(f'{self.fx.head} {unrelated}\n')  # forge: HEAD's parent := unrelated
        probe = git(self.fx.lane, 'merge-base', '--is-ancestor', unrelated, 'HEAD', check=False).returncode
        self.assertEqual(probe, 0, 'precondition: Git honours the graft and believes the forged ancestry')
        self.assertFailClosedAfterControl(self.exact())


# ===================================================================================== repaired internals
class RawProofBehaviour(RawBase):
    def lane(self): return self.fx.lane
    def prof(self): return self.fx.profile
    def pinned(self): return set(self.fx.profile['states']['pre-transition']['dirty'])
    def prove(self, **over):
        prof = self.prof()
        return self.prop.prove_raw_working_tree(self.lane(), over.get('dirty', self.pinned()), over.get('ignored', prof['allowed_ignored']))

    # ---- canonical identity (F1) -------------------------------------------------------------------------------------
    def make_replace_ref(self):
        orig = git(self.lane(), 'rev-parse', 'HEAD:' + OTHER).stdout.decode().strip()
        proc = subprocess.run(['git', '-C', str(self.lane()), 'hash-object', '-w', '--stdin'], input=b'replaced\n', capture_output=True, env={**os.environ, **T.GIT_ENV})
        git(self.lane(), 'replace', orig, proc.stdout.decode().strip()); return orig

    def test_admission_git_calls_read_the_unreplaced_graph(self):
        self.make_replace_ref()
        self.assertEqual(git(self.lane(), 'cat-file', 'blob', 'HEAD:' + OTHER).stdout, b'replaced\n', 'precondition: plain Git is fooled')
        self.assertEqual(self.prop.git_exact(self.lane(), 'cat-file', 'blob', 'HEAD:' + OTHER)[1], b'class Other {}\n')

    def test_replace_ref_in_store_is_refused_even_if_unrelated_to_working_bytes(self):
        self.make_replace_ref(); self.assertFailClosed(self.exact(), 'replacement')

    def test_graft_file_is_refused(self):
        (self.fx.store / 'info').mkdir(exist_ok=True); (self.fx.store / 'info' / 'grafts').write_text('')
        self.assertFailClosed(self.exact(), 'graft')

    def test_shallow_repository_is_refused(self):
        (self.fx.store / 'shallow').write_text(self.fx.head + '\n'); self.assertFailClosed(self.exact(), 'shallow')

    def test_unsupported_object_format_is_refused(self):
        real = self.prop.git_exact
        def fake(lane, *a, **kw):
            if a[:2] == ('rev-parse', '--show-object-format'): return 0, b'md5\n'
            return real(lane, *a, **kw)
        self.prop.git_exact = fake; self.assertFailClosed(self.exact(), 'object format')

    def test_canonical_head_must_equal_the_pin(self):
        real = self.prop.git_exact
        def fake(lane, *a, **kw):
            if a[:3] == ('rev-parse', '--verify', 'HEAD^{commit}'): return 0, b'1' * 40 + b'\n'
            return real(lane, *a, **kw)
        self.prop.git_exact = fake; self.assertFailClosed(self.exact(), 'canonical HEAD')

    # ---- raw tracked bytes (F2/F3) ---------------------------------------------------------------------------------
    def test_proof_counts_and_audit_record(self):
        stats = self.prove()
        self.assertEqual((stats['tracked_files'], stats['hashed_files'], stats['pinned_skipped']), (11, 3, 8))
        self.assertEqual(stats['object_format'], 'sha1')
        self.assertAccepted(self.exact())
        ea = json.loads(self.fx.lock_json('prop').read_text())['exact_admission']
        self.assertEqual(ea['raw_proof']['hashed_files'], 3)
        self.assertEqual(ea['head_tree'], git(self.lane(), 'rev-parse', 'HEAD^{tree}').stdout.decode().strip())

    def test_proof_ignores_index_and_stat_entirely(self):
        (self.fx.store / 'worktrees' / 'lane' / 'index').write_bytes(b'not an index at all' * 20)
        self.prove()                                  # passes without ever reading the (now corrupt) index
        os.utime(self.lane_file(OTHER), (3, 3)); self.prove()
        self.lane_file(OTHER).write_bytes(b'class Other {}\n'.replace(b'Other', b'Drift')); self.assertRaises(ValueError, self.prove)

    def test_proof_never_runs_filters(self):
        marker = self.fx.root / 'filter-ran'
        (self.fx.store / 'info' / 'attributes').write_text('* filter=probe\n')
        for k in ('clean', 'smudge'): git(self.lane(), 'config', 'filter.probe.' + k, f'touch {marker}; cat')
        self.prove(); self.assertFalse(marker.exists(), 'raw proof must not invoke smudge/clean filters')

    def test_one_flipped_byte_in_each_unrelated_file_is_found(self):
        for rel in (OTHER, 'Tools/old.py', '.gitignore'):
            p = self.lane_file(rel); orig = p.read_bytes(); b = bytearray(orig); b[0] ^= 1; p.write_bytes(bytes(b))
            with self.assertRaisesRegex(ValueError, 'differs from the canonical HEAD blob'): self.prove()
            p.write_bytes(orig); self.prove()

    def test_same_size_flip_found_when_sizes_equal_and_size_change_found_without_hash(self):
        p = self.lane_file(OTHER); orig = p.read_bytes(); p.write_bytes(orig + b'x')
        with self.assertRaisesRegex(ValueError, 'differs from the canonical HEAD blob'): self.prove()
        p.write_bytes(orig[:-1]); self.assertRaises(ValueError, self.prove)

    def test_pinned_paths_are_not_hashed_here_but_must_still_be_regular_files(self):
        p = self.lane_file(PATHS[1]); p.unlink(); p.symlink_to(self.lane_file(PATHS[2]))
        with self.assertRaisesRegex(ValueError, 'symlink'): self.prove()

    def test_symlink_replacing_unrelated_tracked_file_is_refused_even_with_identical_bytes(self):
        p = self.lane_file(OTHER); target = self.fx.root / 'other-target.cs'; target.write_bytes(p.read_bytes()); p.unlink(); p.symlink_to(target)
        with self.assertRaisesRegex(ValueError, 'symlink'): self.prove()
        git(self.lane(), 'config', 'core.symlinks', 'false')
        self.assertFailClosed(self.exact())

    def test_symlinked_directory_replacing_tracked_directory_is_refused(self):
        real = self.fx.root / 'toolscopy'; shutil.copytree(self.lane_file('Tools'), real); shutil.rmtree(self.lane_file('Tools')); self.lane_file('Tools').symlink_to(real)
        with self.assertRaises(ValueError): self.prove()
        self.assertFailClosed(self.exact())

    def test_untracked_symlink_is_refused(self):
        self.lane_file('dangling').symlink_to('nowhere'); self.assertRaisesRegex(ValueError, 'untracked', self.prove)

    def test_tracked_file_replaced_by_directory_or_missing_is_refused(self):
        p = self.lane_file('Tools/old.py'); p.unlink(); self.assertRaisesRegex(ValueError, 'missing', self.prove)
        p.mkdir(); (p / 'x').write_text('x'); self.assertRaises(ValueError, self.prove)

    def test_fifo_is_refused_without_blocking(self):
        signal.signal(signal.SIGALRM, lambda *a: (_ for _ in ()).throw(AssertionError('raw proof blocked on a FIFO'))); signal.alarm(20)
        try:
            os.mkfifo(self.lane_file('untracked.fifo')); self.assertRaisesRegex(ValueError, 'special', self.prove)
            os.unlink(self.lane_file('untracked.fifo')); p = self.lane_file('Tools/old.py'); p.unlink(); os.mkfifo(p)
            self.assertRaisesRegex(ValueError, 'special|regular', self.prove)
        finally: signal.alarm(0)

    def test_untracked_files_anywhere_are_refused_independent_of_git_status_and_ignore_rules(self):
        (self.fx.store / 'info' / 'exclude').write_text('Hidden.cs\n'); self.lane_file('Assets/Hidden.cs').write_text('x')
        self.assertRaisesRegex(ValueError, 'untracked', self.prove)
        self.lane_file('Assets/Hidden.cs').unlink(); self.lane_file('Assets/Deep/er').mkdir(parents=True); self.lane_file('Assets/Deep/er/f.txt').write_text('x')
        self.assertRaisesRegex(ValueError, 'untracked', self.prove)

    def test_untracked_cache_cannot_hide_a_new_file_from_the_proof(self):
        git(self.lane(), 'config', 'core.untrackedCache', 'true'); git(self.lane(), 'config', 'core.trustctime', 'false'); git(self.lane(), 'config', 'core.checkStat', 'minimal')
        git(self.lane(), 'update-index', '--untracked-cache', check=False); git(self.lane(), 'status', '--porcelain', '--untracked-files=normal')
        d = self.lane_file('Assets'); st = d.stat(); self.lane_file('Assets/late.cs').write_text('x'); os.utime(d, ns=(st.st_atime_ns, st.st_mtime_ns))
        self.assertRaisesRegex(ValueError, 'untracked', self.prove)

    def test_empty_untracked_directories_and_allowed_ignored_contents_are_tolerated(self):
        self.lane_file('EmptyDir/Nested').mkdir(parents=True); self.lane_file('Library/deep/er').mkdir(parents=True)
        self.lane_file('Library/deep/er/anything.bin').write_bytes(b'\0' * 10)
        self.assertEqual(self.prove()['hashed_files'], 3)

    def test_tracked_path_under_an_allowed_ignored_entry_is_refused(self):
        with self.assertRaisesRegex(ValueError, 'ignored'): self.prove(ignored=self.prof()['allowed_ignored'] + ['Tools/'])

    def test_allowed_ignored_name_must_have_the_right_type(self):
        shutil.rmtree(self.lane_file('Library')); self.lane_file('Library').write_text('now a file')       # 'Library/' allows a directory only
        self.assertRaisesRegex(ValueError, 'untracked', self.prove)
        self.lane_file('Library').unlink(); self.lane_file('Library').symlink_to(self.fx.root)
        self.assertRaises(ValueError, self.prove)

    def test_unreadable_unrelated_file_is_a_refusal_not_a_skip(self):
        p = self.lane_file(OTHER); os.chmod(p, 0)
        try: self.assertRaises(ValueError, self.prove); self.assertFailClosed(self.exact())
        finally: os.chmod(p, 0o644)

    def test_stream_length_disagreeing_with_declared_size_is_refused(self):
        import io
        for data, declared in ((b'abc', 5), (b'abcdef', 5), (b'', 1)):   # file shrank / grew / vanished while being read
            with self.assertRaisesRegex(ValueError, 'changed while'): self.prop.blob_object_id(io.BytesIO(data), declared, 'sha1')

    # ---- parser and hashing helpers ----------------------------------------------------------------------------------
    def test_ls_tree_parser_rejects_what_it_cannot_prove(self):
        ok = b'100644 blob ' + b'a' * 40 + b'      5\tdir/file.txt\0'
        self.assertEqual(self.prop.parse_ls_tree(ok), {b'dir/file.txt': (b'100644', 'a' * 40, 5)})
        for bad in (b'160000 commit ' + b'a' * 40 + b'      -\tsub\0',           # gitlink / submodule
                    (b'100644 blob ' + b'a' * 40 + b'      5\tx\0') * 2,        # duplicate path
                    b'100666 blob ' + b'a' * 40 + b'      5\tx\0',              # unknown mode
                    b'garbage\0', b'100644 blob zz      5\tx\0'):
            with self.assertRaises(ValueError, msg=bad): self.prop.parse_ls_tree(bad)
        with self.assertRaisesRegex(ValueError, 'UTF-8'): self.prop.parse_ls_tree(b'100644 blob ' + b'a' * 40 + b'      5\tbad\xff.txt\0')

    def test_blob_id_matches_git_hash_object_across_chunk_boundaries(self):
        d = self.fx.root / 'hashcheck'; d.mkdir(); git(d, 'init', '-q')
        for size in (0, 1, (1 << 20) - 1, 1 << 20, (1 << 20) + 1, 3 * (1 << 20) + 17):
            data = hashlib.sha256(str(size).encode()).digest() * (size // 32 + 1); data = data[:size]
            f = d / 'blob.bin'; f.write_bytes(data)
            want = git(d, 'hash-object', '--no-filters', 'blob.bin').stdout.decode().strip()
            with open(f, 'rb') as fh: self.assertEqual(self.prop.blob_object_id(fh, size, 'sha1'), want, size)

    def test_blob_id_matches_git_for_sha256_repositories(self):
        d = self.fx.root / 'hash256'; d.mkdir()
        if subprocess.run(['git', '-C', str(d), 'init', '-q', '--object-format=sha256'], capture_output=True, env={**os.environ, **T.GIT_ENV}).returncode: self.skipTest('git cannot create sha256 repos')
        f = d / 'b'; f.write_bytes(b'sha256 repo content\n' * 100_000)
        want = git(d, 'hash-object', '--no-filters', 'b').stdout.decode().strip()
        with open(f, 'rb') as fh: self.assertEqual(self.prop.blob_object_id(fh, f.stat().st_size, 'sha256'), want)

    def test_tracked_symlink_is_compared_by_target_bytes(self):
        d = self.fx.root / 'symrepo'; d.mkdir(); (d / 'target.txt').write_text('t\n'); (d / 'link').symlink_to('target.txt')
        git(d, 'init', '-q', '--separate-git-dir', str(self.fx.root / 'symrepo.git')); git(d, 'add', '-A'); git(d, 'commit', '-q', '-m', 'c')
        self.assertEqual(self.prop.prove_raw_working_tree(d, set(), [])['tracked_files'], 2)
        (d / 'link').unlink(); (d / 'link').symlink_to('elsewhere.txt')
        with self.assertRaisesRegex(ValueError, 'symlink target differs'): self.prop.prove_raw_working_tree(d, set(), [])
        (d / 'link').unlink(); (d / 'link').write_text('target.txt')           # core.symlinks=false style checkout: not supported, refused
        with self.assertRaisesRegex(ValueError, 'symlink was replaced by a regular file'): self.prop.prove_raw_working_tree(d, set(), [])

    # ---- Git errors never become accepts ------------------------------------------------------------------------------
    def test_every_new_git_call_failing_is_a_refusal_with_no_lock(self):
        real = self.prop.git_exact
        for first in (('for-each-ref',), ('rev-parse', '--is-shallow-repository'), ('rev-parse', '--show-object-format'), ('rev-parse', '--verify', 'HEAD^{commit}'), ('rev-parse', 'HEAD^{tree}'), ('ls-tree',)):
            def fail(lane, *a, _first=first, **kw):
                if a[:len(_first)] == _first: raise ValueError(f'git {a[0]} failed (128): simulated')
                return real(lane, *a, **kw)
            self.prop.git_exact = fail
            try: self.assertFailClosed(self.exact(), 'simulated')
            finally: self.prop.git_exact = real

    def test_unexpected_exception_class_still_means_no_lock(self):
        real = self.prop.prove_raw_working_tree
        def boom(*a, **k): raise OSError(5, 'simulated EIO')
        self.prop.prove_raw_working_tree = boom
        try: self.assertFailClosed(self.exact(), 'simulated EIO')
        finally: self.prop.prove_raw_working_tree = real

    # ---- sentinels: the repair keeps the surrounding semantics -----------------------------------------------------------
    def test_sentinel_exact_eight_positive_then_exclusive_lock_blocks_second_request(self):
        self.assertAccepted(self.exact()); r = self.exact(); self.assertEqual(r[0], 2); self.assertIn('lock exists', r[2])

    def test_sentinel_held_lock_is_checked_before_any_raw_work(self):
        d = self.fx.root / 'state' / 'prop'; d.mkdir(parents=True)
        (d / 'lock.json').write_text(json.dumps({'lock_id': 'a' * 32, 'owner': 'someone', 'task_id': 'OTHER', 'lane': '/x', 'branch': 'b', 'head': '1' * 40, 'pid': os.getpid(),
            'process_identity': self.prop.process_identity(os.getpid()), 'start_timestamp': '2026-11-08T00:00:00+00:00', 'eta': '2099-01-01T00:00:00+00:00', 'notes': 'n'}))
        called = []; real = self.prop.prove_raw_working_tree
        self.prop.prove_raw_working_tree = lambda *a, **k: called.append(1) or real(*a, **k)
        try: r = self.exact()
        finally: self.prop.prove_raw_working_tree = real
        self.assertEqual(r[0], 2); self.assertIn('lock exists', r[2]); self.assertEqual(called, [])

    def test_sentinel_default_path_does_not_use_the_new_proof(self):
        called = []; real = self.prop.prove_raw_working_tree
        self.prop.prove_raw_working_tree = lambda *a, **k: called.append(1) or real(*a, **k)
        try: r = run(self.prop, self.fx.acquire_args(exact=False))
        finally: self.prop.prove_raw_working_tree = real
        self.assertEqual(r[0], 2); self.assertIn('Unity lane is dirty; inspect before execution', r[2]); self.assertEqual(called, [])


class RawProofPostTransition(RawBase):
    state = 'post'
    def test_clean_pinned_package_json_and_candidate_files_are_proved_against_the_new_head(self):
        prof = self.fx.profile
        stats = self.prop.prove_raw_working_tree(self.fx.lane, set(prof['states']['post-transition']['dirty']), prof['allowed_ignored'])
        self.assertEqual((stats['tracked_files'], stats['pinned_skipped'], stats['hashed_files']), (12, 6, 6))   # packages + Other + old.py + new_verifier + .gitignore
        (self.fx.lane / 'Tools/new_verifier.py').write_text('y=3\n')
        with self.assertRaisesRegex(ValueError, 'differs from the canonical HEAD blob'):
            self.prop.prove_raw_working_tree(self.fx.lane, set(prof['states']['post-transition']['dirty']), prof['allowed_ignored'])


if __name__ == '__main__':
    try: unittest.main(verbosity=2)
    finally:
        for d in T._SCRATCH: shutil.rmtree(d, ignore_errors=True)
