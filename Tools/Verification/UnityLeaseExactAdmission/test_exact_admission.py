#!/usr/bin/env python3
"""Offline tests for the proposed unity-lock exact-dirty admission (SOURCE PROPOSAL; nothing here is deployed).

Run:  CCF_LEASE_TEST_TMP=<existing scratch dir on the Files disk> python3 -I -B test_exact_admission.py -v

Safety: every fixture is a disposable Git repository + fake lock directory under CCF_LEASE_TEST_TMP. The tests
never run the deployed script, never touch /media/jer/ZX20 (read-only hash of the deployed files only), never use
Unity, a real lease, the registered lane or any Library. Only these production facts are stubbed in the loaded
script module: the ZX20 mount/storage identity checks (config_read / require_storage / on_storage / ROOT) and Unity
process discovery (editors). Everything else - argument parsing, Git store/lane/registration/branch checks,
lock-file handling, the dirty-lane decision, the admission code - is the real script text.
"""
import contextlib, hashlib, importlib.machinery, importlib.util, io, json, multiprocessing, os, shutil
import subprocess, sys, tempfile, unittest
from datetime import datetime, timedelta, timezone
from pathlib import Path

sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
BASELINE = HERE / 'unity-lock.deployed-54cf0a95.txt'
PROPOSED = Path(os.environ.get('CCF_LEASE_TEST_PROPOSED', HERE / 'unity-lock.proposed'))  # override only for mutation checks
CCF_HELPER = HERE / '_ccf.deployed-ab4343ef.txt'
PATCH = HERE / 'unity-lock-exact-admission.patch'
DEPLOYED_DIR = Path('/media/jer/ZX20/CCF-Ops/scripts')
BASELINE_SHA = '54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5'
HELPER_SHA = 'ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001'
TMP_ROOT = Path(os.environ.get('CCF_LEASE_TEST_TMP', '/media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardSource-20261108/tmp'))
EVIDENCE_337 = Path('/media/jer/Files/CCF/Verification/CCF-Workbench111GenvB3-20261108/protected-before.json')
PROD_DIGEST_PRE = '90da4135e52e895d0e728229786d6c9f1fe52ce83591a9ae9bba40a94c42ce90'  # recorded in the proposal; operators restate it

GIT_ENV = {'GIT_CONFIG_GLOBAL': '/dev/null', 'GIT_CONFIG_NOSYSTEM': '1', 'GIT_AUTHOR_NAME': 't', 'GIT_AUTHOR_EMAIL': 't@example.invalid',
           'GIT_COMMITTER_NAME': 't', 'GIT_COMMITTER_EMAIL': 't@example.invalid'}
PATHS = ['.vscode/settings.json',
         'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat',
         'Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat',
         'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat',
         'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat',
         'Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat',
         'Packages/manifest.json', 'Packages/packages-lock.json']
PACKAGES = ('Packages/manifest.json', 'Packages/packages-lock.json')
FIX_IGNORED = ['Library/', 'Logs/', 'UserSettings/', 'Assembly-CSharp.csproj']


_SCRATCH = []
def scratch_dir():
    """A disposable directory under TMP_ROOT, removed when the test run ends."""
    d = Path(tempfile.mkdtemp(prefix='lease-scratch-', dir=TMP_ROOT)); _SCRATCH.append(d); return d

def sha(data): return hashlib.sha256(data).hexdigest()


def git(cwd, *args, check=True, env=None):
    e = dict(os.environ); e.update(GIT_ENV); e.update(env or {})
    p = subprocess.run(['git', '-C', str(cwd), *args], capture_output=True, env=e)
    if check and p.returncode: raise RuntimeError(f'git {args} failed: {p.stderr.decode()}')
    return p


def load_script(src, dest_dir, name):
    """Copy a script as <dest_dir>/CCF-Ops/scripts/unity-lock (+ the pinned helper) and import it without running main()."""
    scripts = dest_dir / 'CCF-Ops' / 'scripts'; scripts.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(src, scripts / 'unity-lock'); shutil.copyfile(CCF_HELPER, scripts / '_ccf.py')
    (scripts.parent / 'lanes.json').write_text('{}')
    if str(scripts) not in sys.path: sys.path.insert(0, str(scripts))
    loader = importlib.machinery.SourceFileLoader(name, str(scripts / 'unity-lock'))
    mod = importlib.util.module_from_spec(importlib.util.spec_from_loader(name, loader))
    loader.exec_module(mod)
    return mod


class Fixture:
    """A disposable Git store + registered worktree lane + fake lock directory, plus a fixture-pinned profile."""
    def __init__(self, state='pre', tmp=None):
        self.root = Path(tempfile.mkdtemp(prefix='lease-fx-', dir=tmp or TMP_ROOT)).resolve()
        self.seed = self.root / 'seed'; self.store = self.root / 'Repository.git'; self.lane = self.root / 'lane'
        self.evidence = self.root / 'evidence'; self.branch = 'feature/fixture-lane'
        self.old_head_files = {p: f'HEAD version of {p}\n'.encode() for p in PATHS}
        self.dirty_files = {p: f'DIRTY working bytes of {p}\n'.encode() for p in PATHS}
        self.state_name = 'pre-transition'
        self.make_repo(state)
        self.make_evidence()
        self.cfg = {'schema': 1, 'repository': str(self.store), 'git_stores': [{'name': 'fx', 'git_dir': str(self.store)}],
                    'lanes': [{'lane': 'FixtureLane', 'path': str(self.lane), 'purpose': 'fixture', 'unity': True}]}
        self.editors = []
        self.profile = self.make_profile()

    # -- repository -------------------------------------------------------------------------------------------
    def make_repo(self, state):
        self.seed.mkdir(); git(self.seed, 'init', '-q', '-b', 'main')
        for p, data in self.old_head_files.items():
            f = self.seed / p; f.parent.mkdir(parents=True, exist_ok=True); f.write_bytes(data)
        (self.seed / 'Assets/Other.cs').write_text('class Other {}\n'); (self.seed / 'Tools').mkdir(); (self.seed / 'Tools/old.py').write_text('x=1\n')
        (self.seed / '.gitignore').write_text('Library/\nLogs/\nUserSettings/\n*.csproj\n')
        git(self.seed, 'add', '-A'); git(self.seed, 'commit', '-q', '-m', 'old head')
        self.old_head = git(self.seed, 'rev-parse', 'HEAD').stdout.decode().strip()
        self.pkg_tracked = {}
        if state == 'post':
            for p in PACKAGES:
                self.pkg_tracked[p] = f'CANDIDATE tracked {p}\n'.encode(); (self.seed / p).write_bytes(self.pkg_tracked[p])
            (self.seed / 'Tools/new_verifier.py').write_text('y=2\n')
            git(self.seed, 'add', '-A'); git(self.seed, 'commit', '-q', '-m', 'candidate')
        self.head = git(self.seed, 'rev-parse', 'HEAD').stdout.decode().strip()
        git(self.root, 'clone', '-q', '--bare', str(self.seed), str(self.store))
        git(self.store, 'worktree', 'add', '-q', '-b', self.branch, str(self.lane), self.head)
        git(self.lane, 'config', 'core.fileMode', 'false')  # exFAT reports one fixed mode
        self.state_name = 'post-transition' if state == 'post' else 'pre-transition'
        for p in PATHS:
            if state == 'post' and p in PACKAGES: continue
            (self.lane / p).write_bytes(self.dirty_files[p])
        for p in PATHS: os.chmod(self.lane / p, 0o755)
        for d in ('Library/x', 'Logs/x', 'UserSettings/x'):
            (self.lane / d).parent.mkdir(exist_ok=True); (self.lane / d).write_text('ignored\n')
        (self.lane / 'Assembly-CSharp.csproj').write_text('<x/>\n')

    def make_evidence(self):
        entries = {}
        for sub, table in (('dirty-backup', self.dirty_files), ('head-versions', self.old_head_files)):
            for p, data in table.items():
                f = self.evidence / sub / p; f.parent.mkdir(parents=True, exist_ok=True); f.write_bytes(data); entries[f'{sub}/{p}'] = sha(data)
        (self.evidence / 'SHA256SUMS.txt').write_text(''.join(f'{h}  {rel}\n' for rel, h in entries.items()))
        self.manifest_sha = sha((self.evidence / 'SHA256SUMS.txt').read_bytes())

    def make_profile(self):
        mode = (self.lane / PATHS[0]).stat().st_mode
        table = {p: (sha(self.dirty_files[p]), sha(self.old_head_files[p]), len(self.dirty_files[p])) for p in PATHS}
        post_dirty = {p: v for p, v in table.items() if p not in PACKAGES}
        prof = {'lane_name': 'FixtureLane', 'lane_path': str(self.lane), 'repository': str(self.store), 'branch': self.branch,
                'file_mode': mode, 'valid_until': (datetime.now(timezone.utc) + timedelta(days=1)).isoformat(),
                'evidence_root': str(self.evidence), 'evidence_manifest_sha256': self.manifest_sha, 'allowed_ignored': list(FIX_IGNORED),
                'states': {'pre-transition': {'head': self.old_head, 'dirty': table, 'clean_pinned': {}, 'must_descend_from': [], 'allowed_changed_paths': None},
                           'post-transition': {'head': None, 'dirty': post_dirty, 'clean_pinned': {}, 'must_descend_from': [self.old_head], 'allowed_changed_paths': None}}}
        if self.pkg_tracked:
            post = prof['states']['post-transition']
            post['head'] = self.head
            post['clean_pinned'] = {p: (sha(d), len(d)) for p, d in self.pkg_tracked.items()}
            post['allowed_changed_paths'] = [*PACKAGES, 'Tools/new_verifier.py']
        return prof

    # -- script wiring ----------------------------------------------------------------------------------------
    def wire(self, mod, name):
        mod.ROOT = self.root; mod.DEFAULT = self.root / 'state' / name
        mod.on_storage = lambda path, root=None: Path(path).resolve()
        mod.require_storage = lambda cfg: None
        mod.config_read = lambda path, **kw: (self.cfg, None)
        mod.editors = lambda: list(self.editors)
        if hasattr(mod, 'EXACT_ADMISSION_PROFILES'): mod.EXACT_ADMISSION_PROFILES = {'FixtureProfile': self.profile}
        return mod

    def digest(self, mod, state=None):
        return mod.admission_digest('FixtureProfile', state or self.state_name)

    def acquire_args(self, mod=None, exact=True, **over):
        a = ['acquire', '--json', '--owner', 'tester', '--task-id', 'T-FIXTURE', '--lane', str(self.lane), '--branch', self.branch,
             '--head', self.cur_head(), '--eta', (datetime.now(timezone.utc) + timedelta(hours=1)).isoformat(), '--notes', 'fixture']
        if exact:
            vals = {'--exact-admission': 'FixtureProfile', '--exact-admission-state': self.state_name,
                    '--exact-admission-digest': None, '--exact-admission-reason': 'fixture proof',
                    '--exact-admission-receipt': 'fixture-receipt'}
            vals.update(over)
            if vals['--exact-admission-digest'] is None and mod: vals['--exact-admission-digest'] = self.digest(mod)
            for k, v in vals.items():
                if k in over and over[k] is None: continue  # explicit None removes the option
                if v is not None: a += [k, v]
        return a

    def cur_head(self): return git(self.lane, 'rev-parse', 'HEAD').stdout.decode().strip()
    def lock_json(self, name): return self.root / 'state' / name / 'lock.json'
    def history(self, name):
        f = self.root / 'state' / name / 'history.jsonl'
        return [json.loads(l) for l in f.read_text().splitlines()] if f.exists() else []
    def cleanup(self): shutil.rmtree(self.root, ignore_errors=True)


def run(mod, argv):
    """Mirror of the script's __main__ wrapper (the test below checks that wrapper is unchanged)."""
    old = sys.argv; sys.argv = ['unity-lock'] + list(argv)
    out, err = io.StringIO(), io.StringIO()
    with contextlib.redirect_stdout(out), contextlib.redirect_stderr(err):
        try: code = mod.main()
        except SystemExit as e: code = e.code
        except (ValueError, OSError, KeyError, TypeError) as e: print('ERROR: ' + str(e), file=sys.stderr); code = 2
    sys.argv = old
    return code, out.getvalue(), err.getvalue()


class Base(unittest.TestCase):
    state = 'pre'
    @classmethod
    def setUpClass(cls):
        TMP_ROOT.mkdir(parents=True, exist_ok=True)
        assert not str(TMP_ROOT.resolve()).startswith('/media/jer/ZX20'), 'tests must not write to ZX20'
        cls.loader_dir = Path(tempfile.mkdtemp(prefix='lease-scripts-', dir=TMP_ROOT))
    @classmethod
    def tearDownClass(cls): shutil.rmtree(cls.loader_dir, ignore_errors=True)
    def setUp(self):
        self.fx = Fixture(self.state); self.addCleanup(self.fx.cleanup)
        self.prop = self.fx.wire(load_script(PROPOSED, self.loader_dir / 'p', 'ulp_%s' % id(self)), 'prop')
        self.base = self.fx.wire(load_script(BASELINE, self.loader_dir / 'b', 'ulb_%s' % id(self)), 'base')
    def exact(self, **over):
        return run(self.prop, self.fx.acquire_args(self.prop, **over))
    def assertRejected(self, result, text=None):
        code, out, err = result
        self.assertEqual(code, 2, (out, err)); self.assertIn('ERROR: ', err)
        if text: self.assertIn(text, err)
        self.assertFalse(self.fx.lock_json('prop').exists(), 'a rejected request must not create a lock')
    def assertAccepted(self, result):
        code, out, err = result
        self.assertEqual(code, 0, (out, err)); self.assertTrue(self.fx.lock_json('prop').exists())


# ============================================================================ source integrity / structure
class SourceStructure(unittest.TestCase):
    def test_baseline_and_helper_are_the_pinned_deployed_bytes(self):
        self.assertEqual(sha(BASELINE.read_bytes()), BASELINE_SHA); self.assertEqual(sha(CCF_HELPER.read_bytes()), HELPER_SHA)
        if DEPLOYED_DIR.exists():  # read-only comparison with the live deployment, when present
            self.assertEqual(sha((DEPLOYED_DIR / 'unity-lock').read_bytes()), BASELINE_SHA, 'deployed unity-lock changed: proposal is stale')
            self.assertEqual(sha((DEPLOYED_DIR / '_ccf.py').read_bytes()), HELPER_SHA, 'deployed _ccf.py changed: proposal is stale')

    def test_patch_reproduces_proposed_exactly_and_removes_one_line(self):
        with tempfile.TemporaryDirectory(dir=TMP_ROOT) as d:
            shutil.copyfile(BASELINE, Path(d) / 'unity-lock')
            if shutil.which('patch'): cmd = ['patch', '-p1', '-s', '-i', str(PATCH)]
            else: cmd = ['git', 'apply', str(PATCH)]
            subprocess.run(cmd, cwd=d, check=True)
            self.assertEqual((Path(d) / 'unity-lock').read_bytes(), PROPOSED.read_bytes())
        removed = [l for l in PATCH.read_text().splitlines() if l.startswith('-') and not l.startswith('---')]
        self.assertEqual(removed, ["-            if command(['git','-C',str(lane),'status','--porcelain']): raise ValueError('Unity lane is dirty; inspect before execution')"])

    def test_release_status_and_main_wrapper_unchanged(self):
        b, p = BASELINE.read_text(), PROPOSED.read_text()
        for marker in ("        if not data and not malformed: raise ValueError('no lock to release')", "if __name__=='__main__':"):
            self.assertEqual(b[b.index(marker):], p[p.index(marker):], 'text after %r differs' % marker)
        for marker in ("def process_identity(pid):", "def guarded(state):"):  # flock/guard/stale-detection helpers
            end = lambda t: t.index('def ', t.index(marker) + 10)
            self.assertEqual(b[b.index(marker):end(b)], p[p.index(marker):end(p)])

    def test_default_dirty_message_and_single_acquire_site_preserved(self):
        p = PROPOSED.read_text()
        self.assertEqual(p.count("raise ValueError('Unity lane is dirty; inspect before execution')"), 1)
        self.assertEqual(p.count("if a.exact_admission is None:"), 1)

    def test_production_profile_is_one_lane_and_post_state_frozen_to_candidate_c(self):
        mod = load_script(PROPOSED, scratch_dir(), 'ulp_prod')
        profs = mod.EXACT_ADMISSION_PROFILES
        self.assertEqual(list(profs), ['WorkbenchTier2Reg287-exact-dirty-v1'])
        pr = profs['WorkbenchTier2Reg287-exact-dirty-v1']
        self.assertEqual((pr['lane_name'], pr['lane_path'], pr['repository']), ('WorkbenchTier2Reg287', '/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106', '/media/jer/Files/CCF/Repository.git'))
        self.assertEqual(pr['branch'], 'feature/scenario-one-workbench-retirement-20261105')
        self.assertEqual(pr['states']['pre-transition']['head'], '4d95e45245d660e24b91e26754d9c57949bf3384')
        # #347 Phase B: the post-transition pins are now filled from candidate C (PROVISIONAL, not accepted/deployed; see test_post_transition_profile.py)
        self.assertEqual(pr['states']['post-transition']['head'], '34e6db134b50f0a393733145962ca7c9c69a9e36')
        self.assertEqual(set(pr['states']['post-transition']['clean_pinned']), set(PACKAGES)); self.assertEqual(len(pr['states']['post-transition']['allowed_changed_paths']), 13)
        self.assertEqual(len(pr['states']['pre-transition']['dirty']), 8); self.assertEqual(len(pr['states']['post-transition']['dirty']), 6)
        self.assertEqual(pr['states']['post-transition']['must_descend_from'], ['46bc1fd618ae5b214d7de296a03ac7efec27cb7c'])

    def test_production_constants_match_337_protected_before_and_318_manifest(self):
        mod = load_script(PROPOSED, scratch_dir(), 'ulp_prod2')
        pr = mod.EXACT_ADMISSION_PROFILES['WorkbenchTier2Reg287-exact-dirty-v1']
        evidence = json.loads(EVIDENCE_337.read_text())
        self.assertEqual(len(evidence), 8)
        for e in evidence:
            w, h, n = pr['states']['pre-transition']['dirty'][e['path']]
            self.assertEqual((w, h, n, e['mode']), (e['working_sha256'], e['head_sha256'], e['size'], '0o100755'), e['path'])
        mod.verify_exact_evidence(pr)  # real #318 files on the Files disk: 16/16, read-only
        self.assertEqual(sha(Path(pr['evidence_root'], 'SHA256SUMS.txt').read_bytes()), '9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74')

    def test_production_digest_pin(self):
        mod = load_script(PROPOSED, scratch_dir(), 'ulp_prod3')
        d1 = mod.admission_digest('WorkbenchTier2Reg287-exact-dirty-v1', 'pre-transition')
        self.assertRegex(d1, r'^[0-9a-f]{64}$')
        if PROD_DIGEST_PRE: self.assertEqual(d1, PROD_DIGEST_PRE)
        print('\nPRODUCTION pre-transition digest:', d1)

    def test_production_profile_refused_on_any_other_lane_in_both_states(self):
        fx = Fixture('pre'); self.addCleanup(fx.cleanup)
        mod = fx.wire(load_script(PROPOSED, scratch_dir(), 'ulp_prod4'), 'prop')
        mod.EXACT_ADMISSION_PROFILES = load_script(PROPOSED, scratch_dir(), 'ulp_prod5').EXACT_ADMISSION_PROFILES
        pid = 'WorkbenchTier2Reg287-exact-dirty-v1'
        digest = mod.admission_digest(pid, 'pre-transition')
        r = run(mod, fx.acquire_args(mod, **{'--exact-admission': pid, '--exact-admission-digest': digest, '--exact-admission-state': 'pre-transition'}))
        self.assertEqual(r[0], 2); self.assertIn('limited to the profile lane', r[2])
        digest = mod.admission_digest(pid, 'post-transition')   # frozen now, so the lane check (not 'unsupported') is what refuses it
        r = run(mod, fx.acquire_args(mod, **{'--exact-admission': pid, '--exact-admission-digest': digest, '--exact-admission-state': 'post-transition'}))
        self.assertEqual(r[0], 2); self.assertIn('limited to the profile lane', r[2])
        self.assertFalse(fx.lock_json('prop').exists())


# ============================================================================ (a) default behaviour unchanged
class DefaultStrictClean(Base):
    VOLATILE = {'lock_id', 'start_timestamp', 'eta'}
    def both(self, argv):
        rb, rp = run(self.base, argv), run(self.prop, argv)
        def norm(r):
            code, out, err = r
            if out.strip():
                d = json.loads(out); d.get('metadata', {}).pop('lock_id', None); [d.get('metadata', {}).pop(k, None) for k in self.VOLATILE]
                out = json.dumps(d, sort_keys=True)
            return code, out, err
        return norm(rb), norm(rp), rb, rp

    def release(self, mod, name, code):
        data = json.loads(code[1])['metadata']
        return run(mod, ['release', '--owner', data['owner'], '--task-id', data['task_id'], '--lock-id', data['lock_id']])

    def test_default_dirty_lane_denied_identically_to_deployed(self):  # fixture lane is dirty (8 files + ignored)
        argv = self.fx.acquire_args(exact=False)
        nb, np_, rb, rp = self.both(argv)
        self.assertEqual(nb, np_); self.assertEqual(rp[0], 2); self.assertIn('Unity lane is dirty; inspect before execution', rp[2])
        self.assertFalse(self.fx.lock_json('prop').exists())

    def test_default_clean_lane_passes_identically_and_lock_has_no_new_fields(self):
        for p in PATHS: (self.fx.lane / p).write_bytes(self.fx.old_head_files[p])
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout, b'')
        nb, np_, rb, rp = self.both(self.fx.acquire_args(exact=False))
        self.assertEqual(rp[0], 0); self.assertEqual(nb, np_)
        self.assertNotIn('exact_admission', json.loads(self.fx.lock_json('prop').read_text()))
        self.assertEqual(self.release(self.prop, 'prop', rp)[0], 0)

    def test_default_denies_staged_untracked_and_modified_exactly_like_deployed(self):
        for p in PATHS: (self.fx.lane / p).write_bytes(self.fx.old_head_files[p])
        cases = {'untracked': lambda: (self.fx.lane / 'new.txt').write_text('n'),
                 'modified': lambda: (self.fx.lane / 'Assets/Other.cs').write_text('changed\n'),
                 'staged': lambda: (git(self.fx.lane, 'add', 'new.txt'))}
        for name, mutate in cases.items():
            mutate(); nb, np_, rb, rp = self.both(self.fx.acquire_args(exact=False))
            self.assertEqual(nb, np_, name); self.assertEqual(rp[0], 2, name); self.assertIn('Unity lane is dirty', rp[2], name)

    def test_exact_options_reject_clean_lane_instead_of_silently_passing(self):
        for p in PATHS: (self.fx.lane / p).write_bytes(self.fx.old_head_files[p])
        self.assertRejected(self.exact(), 'modified path set is not exactly the approved set')


# ============================================================================ (b) exact states accepted
class ExactPreTransition(Base):
    def test_exact_eight_file_state_accepted_and_audited(self):
        self.assertAccepted(self.exact())
        lock = json.loads(self.fx.lock_json('prop').read_text()); ea = lock['exact_admission']
        self.assertEqual((ea['profile'], ea['state'], ea['head'], ea['branch'], ea['lane']), ('FixtureProfile', 'pre-transition', self.fx.head, self.fx.branch, str(self.fx.lane)))
        self.assertEqual(set(ea['before']), set(PATHS))
        self.assertEqual(ea['before'][PATHS[1]]['sha256'], sha(self.fx.dirty_files[PATHS[1]]))
        self.assertEqual(ea['reason'], 'fixture proof'); self.assertEqual(ea['receipt'], 'fixture-receipt')
        events = [e['event'] for e in self.fx.history('prop')]; self.assertEqual(events, ['exact-admission'])
        self.assertEqual(self.fx.history('prop')[0]['manager_approval'], 'fixture-receipt')

    def test_acceptance_does_not_modify_lane_bytes_index_or_head(self):
        before_status = git(self.fx.lane, 'status', '--porcelain').stdout
        index_before = (self.fx.store / 'worktrees' / 'lane' / 'index').read_bytes()
        self.assertAccepted(self.exact())
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout, before_status)
        self.assertEqual((self.fx.store / 'worktrees' / 'lane' / 'index').read_bytes(), index_before, 'admission must not refresh the index')
        for p in PATHS: self.assertEqual((self.fx.lane / p).read_bytes(), self.fx.dirty_files[p])

    def test_lock_is_normal_for_deployed_status_and_release(self):
        self.assertAccepted(self.exact())
        data = json.loads(self.fx.lock_json('prop').read_text())
        shutil.copytree(self.fx.root / 'state' / 'prop', self.fx.root / 'state' / 'base')  # deployed script reads and releases the same record
        r = run(self.base, ['release', '--owner', data['owner'], '--task-id', data['task_id'], '--lock-id', data['lock_id']])
        self.assertEqual(r[0], 0, r)
        self.assertFalse(self.fx.lock_json('base').exists())
        r = run(self.prop, ['release', '--owner', data['owner'], '--task-id', data['task_id'], '--lock-id', data['lock_id']])
        self.assertEqual(r[0], 0, r); self.assertEqual([e['event'] for e in self.fx.history('prop')], ['exact-admission', 'release'])

    def test_stat_only_noise_is_not_drift_when_bytes_equal(self):
        for p in PATHS: os.utime(self.fx.lane / p, (1, 1))                    # mtime changes only
        os.utime(self.fx.lane / 'Assets/Other.cs', (5, 5))                    # unrelated tracked file stat change
        p = PATHS[3]; data = (self.fx.lane / p).read_bytes(); (self.fx.lane / p).unlink(); (self.fx.lane / p).write_bytes(data); os.chmod(self.fx.lane / p, 0o755)  # new inode, same bytes
        self.assertAccepted(self.exact())

    def test_reaccepting_second_time_blocked_by_exclusive_lock(self):
        self.assertAccepted(self.exact()); self.assertRejected_keep(self.exact(), 'lock exists')

    def assertRejected_keep(self, r, text):
        self.assertEqual(r[0], 2); self.assertIn(text, r[2])


class ExactPostTransition(Base):
    state = 'post'
    def test_frozen_fixture_post_state_with_six_dirty_accepted(self):
        self.assertEqual(self.fx.state_name, 'post-transition')
        self.assertAccepted(self.exact())
        ea = json.loads(self.fx.lock_json('prop').read_text())['exact_admission']
        self.assertEqual(set(ea['before']), set(PATHS))  # six dirty + the two clean-pinned package JSONs
        self.assertEqual(ea['before'][PACKAGES[0]]['sha256'], sha(self.fx.pkg_tracked[PACKAGES[0]]))

    def test_pre_state_name_on_post_head_is_refused(self):
        self.assertRejected(self.exact(**{'--exact-admission-state': 'pre-transition', '--exact-admission-digest': self.fx.digest(self.prop, 'pre-transition')}), 'pinned exact-admission state')

    def test_package_json_working_bytes_not_equal_to_tracked_refused(self):
        (self.fx.lane / PACKAGES[0]).write_bytes(b'edited package json\n')
        self.assertRejected(self.exact(), 'modified path set is not exactly the approved set')

    def test_package_json_dirty_but_hash_pinned_equal_still_refused_as_extra_dirty(self):
        (self.fx.lane / PACKAGES[1]).write_bytes(self.fx.dirty_files[PACKAGES[1]])  # the old pre-transition dirty bytes
        self.assertRejected(self.exact())

    def test_head_not_descending_from_pinned_ancestor_refused(self):
        self.fx.profile['states']['post-transition']['must_descend_from'] = ['0' * 40]
        self.assertRejected(self.exact(), 'merge-base failed')  # unknown object: Git error is a refusal
        empty = git(self.fx.store, 'hash-object', '-t', 'tree', '-w', '/dev/null').stdout.decode().strip()
        unrelated = git(self.fx.store, 'commit-tree', empty, '-m', 'unrelated').stdout.decode().strip()
        self.fx.profile['states']['post-transition']['must_descend_from'] = [unrelated]
        self.assertRejected(self.exact(), 'does not descend from')

    def test_candidate_changing_unlisted_path_refused(self):
        self.fx.profile['states']['post-transition']['allowed_changed_paths'] = [PACKAGES[0]]
        self.assertRejected(self.exact(), 'outside the frozen candidate list')

    def test_unfrozen_pieces_each_make_state_unsupported(self):
        for field, value in (('head', None), ('clean_pinned', {}), ('allowed_changed_paths', None)):
            old = self.fx.profile['states']['post-transition'][field]
            self.fx.profile['states']['post-transition'][field] = value
            self.assertRejected(self.exact(), 'not frozen by the Manager; unsupported')
            self.fx.profile['states']['post-transition'][field] = old


# ============================================================================ (c) one extra / changed thing
class ExtraOrChangedRejected(Base):
    def test_extra_modified_tracked_file(self):
        (self.fx.lane / 'Assets/Other.cs').write_text('changed\n'); self.assertRejected(self.exact(), 'not exactly the approved set')

    def test_extra_untracked_file(self):
        (self.fx.lane / 'Assets/new.cs').write_text('x'); self.assertRejected(self.exact(), "unexpected Git status entry: '?? ")

    def test_extra_untracked_directory(self):
        (self.fx.lane / 'Fresh').mkdir(); (self.fx.lane / 'Fresh/a').write_text('x'); self.assertRejected(self.exact(), '??')

    def test_extra_staged_change(self):
        (self.fx.lane / 'Assets/Other.cs').write_text('staged\n'); git(self.fx.lane, 'add', 'Assets/Other.cs'); self.assertRejected(self.exact())

    def test_protected_file_staged(self):
        git(self.fx.lane, 'add', PATHS[2]); self.assertRejected(self.exact(), 'unexpected Git status entry')

    def test_protected_file_one_byte_changed_same_size(self):
        p = self.fx.lane / PATHS[4]; b = bytearray(p.read_bytes()); b[0] ^= 1; p.write_bytes(bytes(b)); os.chmod(p, 0o755)
        self.assertRejected(self.exact(), 'protected bytes/size/mode/HEAD-blob mismatch')

    def test_protected_file_size_changed(self):
        p = self.fx.lane / PATHS[5]; p.write_bytes(p.read_bytes() + b'x'); self.assertRejected(self.exact(), 'protected bytes')

    def test_protected_file_reverted_to_head_so_set_is_short(self):
        (self.fx.lane / PATHS[6]).write_bytes(self.fx.old_head_files[PATHS[6]]); self.assertRejected(self.exact(), 'not exactly the approved set')

    def test_protected_file_deleted(self):
        (self.fx.lane / PATHS[7]).unlink(); self.assertRejected(self.exact(), 'unexpected Git status entry')

    def test_unlisted_ignored_path(self):
        (self.fx.lane / 'Assets/Hidden.csproj').write_text('x'); self.assertRejected(self.exact(), "'!! Assets/Hidden.csproj'")

    def test_info_exclude_cannot_hide_untracked_file(self):
        (self.fx.lane / 'Sneaky.cs').write_text('x')
        with (self.fx.store / 'info').joinpath('exclude').open('a') as f: f.write('Sneaky.cs\n')
        self.assertRejected(self.exact(), 'Sneaky.cs')

    def test_protected_file_mode_change_not_waived(self):
        os.chmod(self.fx.lane / PATHS[1], 0o644); self.assertRejected(self.exact(), 'mode')

    def test_protected_file_replaced_by_symlink_to_identical_bytes(self):
        p = self.fx.lane / PATHS[1]; target = self.fx.root / 'target.mat'; target.write_bytes(p.read_bytes()); p.unlink(); p.symlink_to(target)
        self.assertRejected(self.exact())

    def test_protected_directory_symlinked_elsewhere(self):
        real = self.fx.root / 'elsewhere'; shutil.move(str(self.fx.lane / 'Packages'), str(real)); (self.fx.lane / 'Packages').symlink_to(real)
        self.assertRejected(self.exact())


# ============================================================================ (d) wrong identity / evidence
class WrongIdentityOrEvidence(Base):
    def test_wrong_branch_argument_rejected_by_existing_check(self):
        self.assertRejected(run(self.prop, self.fx.acquire_args(self.prop)[:self.fx.acquire_args(self.prop).index('--branch') + 1] + ['feature/other'] + self.fx.acquire_args(self.prop)[self.fx.acquire_args(self.prop).index('--branch') + 2:]), 'branch/HEAD mismatch')

    def test_profile_branch_mismatch(self):
        self.fx.profile['branch'] = 'feature/not-this-one'; self.assertRejected(self.exact(), 'pinned exact-admission state')

    def test_wrong_head_even_when_caller_supplies_matching_head(self):
        git(self.fx.lane, 'commit', '-q', '--allow-empty', '-m', 'moved'); self.assertRejected(self.exact(), 'pinned exact-admission state')

    def test_wrong_head_when_profile_pins_other_commit(self):
        self.fx.profile['states']['pre-transition']['head'] = '1' * 40; self.assertRejected(self.exact(), 'pinned exact-admission state')

    def test_wrong_worktree_path(self):
        self.fx.profile['lane_path'] = str(self.fx.root / 'other-lane'); self.assertRejected(self.exact(), 'limited to the profile lane')

    def test_wrong_lane_name(self):
        self.fx.profile['lane_name'] = 'SomeOtherLane'; self.assertRejected(self.exact(), 'limited to the profile lane')

    def test_wrong_common_store_in_profile(self):
        self.fx.profile['repository'] = str(self.fx.root / 'Other.git'); self.assertRejected(self.exact(), 'limited to the profile lane')

    def test_wrong_common_store_in_config_existing_check(self):
        self.fx.cfg['repository'] = str(self.fx.root / 'Other.git'); self.assertRejected(self.exact(), 'unexpected lane Git store')

    def test_missing_manifest(self):
        (self.fx.evidence / 'SHA256SUMS.txt').unlink(); self.assertRejected(self.exact(), 'cannot safely open')

    def test_edited_manifest_even_by_a_trailing_newline(self):
        with (self.fx.evidence / 'SHA256SUMS.txt').open('a') as f: f.write('\n')
        self.assertRejected(self.exact(), 'missing or edited')

    def test_manifest_and_backup_edited_consistently_still_fails_pinned_manifest_hash(self):
        rel = 'dirty-backup/' + PATHS[1]; (self.fx.evidence / rel).write_bytes(b'tampered\n')
        lines = [l.replace(sha(self.fx.dirty_files[PATHS[1]]), sha(b'tampered\n')) for l in (self.fx.evidence / 'SHA256SUMS.txt').read_text().splitlines()]
        (self.fx.evidence / 'SHA256SUMS.txt').write_text('\n'.join(lines) + '\n'); self.assertRejected(self.exact(), 'missing or edited')

    def test_backup_file_edited_manifest_untouched(self):
        (self.fx.evidence / 'dirty-backup' / PATHS[2]).write_bytes(b'tampered\n'); self.assertRejected(self.exact(), 'preserved evidence file edited')

    def test_head_version_file_edited(self):
        (self.fx.evidence / 'head-versions' / PATHS[0]).write_bytes(b'tampered\n'); self.assertRejected(self.exact(), 'preserved evidence file edited')

    def test_backup_file_missing(self):
        (self.fx.evidence / 'dirty-backup' / PATHS[3]).unlink(); self.assertRejected(self.exact(), 'cannot safely open')

    def test_manifest_listing_extra_entry_but_pinned_hash_matches_profile_edit(self):
        with (self.fx.evidence / 'SHA256SUMS.txt').open('a') as f: f.write(sha(b'x') + '  dirty-backup/extra.txt\n')
        self.fx.profile['evidence_manifest_sha256'] = sha((self.fx.evidence / 'SHA256SUMS.txt').read_bytes())
        self.assertRejected(self.exact(), 'exactly the approved 16')

    def test_manifest_is_symlink(self):
        m = self.fx.evidence / 'SHA256SUMS.txt'; real = self.fx.root / 'real.sums'; shutil.move(str(m), str(real)); m.symlink_to(real); self.assertRejected(self.exact())

    def test_evidence_root_is_symlink(self):
        link = self.fx.root / 'evlink'; link.symlink_to(self.fx.evidence); self.fx.profile['evidence_root'] = str(link)
        self.assertRejected(self.exact(), 'canonical real directory')

    def test_expired_profile(self):
        self.fx.profile['valid_until'] = (datetime.now(timezone.utc) - timedelta(seconds=1)).isoformat(); self.assertRejected(self.exact(), 'expired')

    def test_wrong_digest(self):
        self.assertRejected(self.exact(**{'--exact-admission-digest': '0' * 64}), 'does not name the approved')

    def test_digest_changes_when_a_pinned_byte_hash_changes(self):
        d = self.fx.digest(self.prop); w, h, n = self.fx.profile['states']['pre-transition']['dirty'][PATHS[0]]
        self.fx.profile['states']['pre-transition']['dirty'][PATHS[0]] = ('0' * 64, h, n); self.assertNotEqual(d, self.fx.digest(self.prop))

    def test_unknown_profile_and_state(self):
        self.assertRejected(self.exact(**{'--exact-admission': 'Nope'}), 'unknown exact-admission profile')
        self.assertRejected(self.exact(**{'--exact-admission-state': 'sideways'}), 'unknown exact-admission state')

    def test_incomplete_or_misplaced_options_are_usage_errors(self):
        for missing in ('--exact-admission-state', '--exact-admission-digest', '--exact-admission-reason', '--exact-admission-receipt'):
            code, out, err = self.exact(**{missing: None}); self.assertEqual(code, 2, missing); self.assertIn('required together', err)
        code, out, err = self.exact(**{'--exact-admission-receipt': 'none'}); self.assertEqual(code, 2); self.assertIn('Manager approval', err)
        code, out, err = self.exact(**{'--exact-admission-reason': '   '}); self.assertEqual(code, 2)
        for op in ('status', 'release'):
            code, out, err = run(self.prop, [op, '--exact-admission', 'FixtureProfile']); self.assertEqual(code, 2, op); self.assertIn('only applies to acquire', err)
        self.assertFalse(self.fx.lock_json('prop').exists())


# ============================================================================ (e) lease semantics intact
class LeaseSemantics(Base):
    def seed_lock(self, **over):
        d = self.fx.root / 'state' / 'prop'; d.mkdir(parents=True, exist_ok=True)
        data = {'lock_id': 'a' * 32, 'owner': 'someone', 'task_id': 'OTHER', 'lane': '/x', 'branch': 'b', 'head': '1' * 40, 'pid': os.getpid(),
                'process_identity': self.prop.process_identity(os.getpid()), 'start_timestamp': datetime.now(timezone.utc).isoformat(),
                'eta': (datetime.now(timezone.utc) + timedelta(hours=1)).isoformat(), 'notes': 'n'}
        data.update(over); (d / 'lock.json').write_text(json.dumps(data)); return data

    def assertLockUntouched(self, data):
        self.assertEqual(json.loads(self.fx.lock_json('prop').read_text()), data)

    def test_held_lock_blocks_exact_acquire(self):
        d = self.seed_lock(); r = self.exact(); self.assertEqual(r[0], 2); self.assertIn('lock exists', r[2]); self.assertLockUntouched(d)

    def test_stale_lock_still_requires_explicit_release(self):
        d = self.seed_lock(pid=4194303, process_identity={'boot_id': 'x', 'start_ticks': '1'})
        r = self.exact(); self.assertEqual(r[0], 2); self.assertIn('stale/malformed records also require explicit release', r[2]); self.assertLockUntouched(d)

    def test_malformed_lock_blocks(self):
        d = self.fx.root / 'state' / 'prop'; d.mkdir(parents=True); (d / 'lock.json').write_text('{not json')
        r = self.exact(); self.assertEqual(r[0], 2); self.assertEqual((d / 'lock.json').read_text(), '{not json')

    def test_unclaimed_unity_editor_blocks(self):
        self.fx.editors = [4242]; self.assertRejected(self.exact(), 'Unity Editor already running')

    def test_other_branch_checkout_blocks(self):
        other = self.fx.root / 'other-checkout'
        git(self.fx.store, 'worktree', 'add', '-q', '--detach', str(other), self.fx.head)
        # a second writable checkout of the SAME branch cannot exist; simulate by registering conflict via config of second store
        git(other, 'checkout', '-q', '--ignore-other-worktrees', self.fx.branch)
        self.assertRejected(self.exact(), 'dual-store/lane conflict')

    def test_unregistered_lane_blocks(self):
        other = self.fx.root / 'Other.git'; git(self.fx.root, 'init', '-q', '--bare', str(other))
        self.fx.cfg['git_stores'] = [{'name': 'other', 'git_dir': str(other)}]  # lane is not registered in the inspected store
        self.assertRejected(self.exact(), 'not registered')

    def test_two_concurrent_exact_acquires_produce_exactly_one_lock(self):
        ctx = multiprocessing.get_context('fork')
        for attempt in range(4):
            shutil.rmtree(self.fx.root / 'state' / 'prop', ignore_errors=True)
            barrier, queue = ctx.Barrier(2), ctx.Queue()
            def worker(n):
                argv = self.fx.acquire_args(self.prop)
                argv[argv.index('--owner') + 1] = 'racer%d' % n
                barrier.wait(); queue.put(run(self.prop, argv)[0])
            procs = [ctx.Process(target=worker, args=(i,)) for i in range(2)]
            [p.start() for p in procs]; [p.join(60) for p in procs]
            codes = sorted(queue.get(timeout=5) for _ in procs)
            self.assertEqual(codes, [0, 2], 'attempt %d' % attempt)
            self.assertEqual(len([e for e in self.fx.history('prop') if e['event'] == 'exact-admission']), 1)

    def test_after_release_a_new_request_needs_full_revalidation(self):
        self.assertAccepted(self.exact()); data = json.loads(self.fx.lock_json('prop').read_text())
        self.assertEqual(run(self.prop, ['release', '--owner', 'tester', '--task-id', 'T-FIXTURE', '--lock-id', data['lock_id']])[0], 0)
        (self.fx.lane / 'Assets/Other.cs').write_text('drift after release\n'); self.assertRejected(self.exact())


# ============================================================================ (f) Git / stat errors never become accepts
class ErrorsNeverAccept(Base):
    def test_corrupt_index(self):
        idx = self.fx.store / 'worktrees' / 'lane' / 'index'; idx.write_bytes(b'garbage' * 50); self.assertRejected(self.exact())

    def test_assume_unchanged_flag_cannot_hide_unrelated_modification(self):
        (self.fx.lane / 'Assets/Other.cs').write_text('hidden change\n'); git(self.fx.lane, 'update-index', '--assume-unchanged', 'Assets/Other.cs')
        self.assertEqual(git(self.fx.lane, 'status', '--porcelain').stdout.count(b'Other.cs'), 0, 'precondition: Git status itself is fooled')
        self.assertRejected(self.exact(), 'hidden-change flag')

    def test_skip_worktree_flag_cannot_hide_unrelated_modification(self):
        (self.fx.lane / 'Tools/old.py').write_text('hidden change\n'); git(self.fx.lane, 'update-index', '--skip-worktree', 'Tools/old.py')
        self.assertRejected(self.exact(), 'hidden-change flag')

    def test_assume_unchanged_on_protected_path_also_refused(self):
        git(self.fx.lane, 'update-index', '--assume-unchanged', PATHS[0]); self.assertRejected(self.exact())

    def test_git_missing_from_path(self):
        argv = self.fx.acquire_args(self.prop)
        empty = self.fx.root / 'emptybin'; empty.mkdir(); old = os.environ['PATH']; os.environ['PATH'] = str(empty)
        try: r = run(self.prop, argv)
        finally: os.environ['PATH'] = old
        self.assertEqual(r[0], 2); self.assertFalse(self.fx.lock_json('prop').exists())

    def test_git_failure_inside_admission_only_is_refusal(self):
        real = self.prop.git_exact
        def failing(lane, *args, **kw):
            if args[0] == 'ls-files': raise ValueError('git ls-files failed (128): simulated')
            return real(lane, *args, **kw)
        self.prop.git_exact = failing; self.assertRejected(self.exact(), 'simulated')

    def test_nonzero_git_status_exit_is_refusal(self):
        real = self.prop.subprocess.run
        class P:  # a Git that "succeeds" with garbage on stderr but a failing exit status
            returncode = 1; stdout = b''; stderr = b'fatal: simulated'
        def fake(cmd, **kw): return P() if 'status' in cmd and '--ignored=traditional' in cmd else real(cmd, **kw)
        self.prop.subprocess.run = fake
        try: r = self.exact()
        finally: self.prop.subprocess.run = real
        self.assertRejected(r, 'simulated')

    def test_git_environment_redirection_is_ignored_by_admission(self):
        other = self.fx.root / 'decoy'; other.mkdir(); git(other, 'init', '-q')
        os.environ['GIT_DIR'] = str(other / '.git'); os.environ['GIT_WORK_TREE'] = str(other); os.environ['GIT_INDEX_FILE'] = str(other / 'idx')
        try: _, top = self.prop.git_exact(self.fx.lane, 'rev-parse', '--show-toplevel')
        finally:
            for k in ('GIT_DIR', 'GIT_WORK_TREE', 'GIT_INDEX_FILE'): os.environ.pop(k)
        self.assertEqual(Path(top.decode().strip()).resolve(), self.fx.lane)

    def test_non_utf8_path_refused(self):
        name = os.fsencode(self.fx.lane) + b'/bad\xff.txt'
        try:
            with open(name, 'wb') as f: f.write(b'x')
        except OSError: self.skipTest('filesystem rejects non-UTF-8 names')
        self.assertRejected(self.exact())


if __name__ == '__main__':
    try: unittest.main(verbosity=2)
    finally:
        for d in _SCRATCH: shutil.rmtree(d, ignore_errors=True)
