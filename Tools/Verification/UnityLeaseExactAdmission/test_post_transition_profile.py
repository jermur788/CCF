#!/usr/bin/env python3
"""Targeted offline checks for the PROVISIONAL post-transition pins (CCF #347 Phase B). Nothing here is deployed or approved.

Run:  CCF_LEASE_TEST_TMP=<existing scratch dir on the Files disk> python3 -I -B test_post_transition_profile.py -v

What it does: derives every post-transition pin independently from Git objects (read-only `git cat-file` / `git diff` on the
shared store, reached through this checkout) and compares it with the profile in unity-lock.proposed; proves that only the
post-transition block and the expiry *comment* changed against the reviewed source 11cee0c2; recomputes the digest; and replays
the transition in one disposable miniature repository (selected old/C blobs plus the preserved #318 dirty bytes).
It never runs the deployed script, a lease, Unity, or admission on the real lane, and never touches the registered Workbench.
"""
import hashlib, importlib.util, os, re, shutil, subprocess, sys, tempfile, unittest
from pathlib import Path

sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
spec = importlib.util.spec_from_file_location('tea', HERE / 'test_exact_admission.py')
tea = importlib.util.module_from_spec(spec); spec.loader.exec_module(tea)

PID = 'WorkbenchTier2Reg287-exact-dirty-v1'
OLD = '4d95e45245d660e24b91e26754d9c57949bf3384'
OPTION_B = '46bc1fd618ae5b214d7de296a03ac7efec27cb7c'
CANDIDATE_C = '34e6db134b50f0a393733145962ca7c9c69a9e36'
REVIEWED_SOURCE = '11cee0c26c81b916c77a5310370d7ce42f2a203b'
PROPOSED_REL = 'Tools/Verification/UnityLeaseExactAdmission/unity-lock.proposed'
# PROVISIONAL, NOT ACCEPTED: recorded so a drift in any pinned value is visible; the Manager decides whether it is adopted.
POST_DIGEST_PROVISIONAL = '75b2998bca18c6cb8f932c8fca9b8baee9bd18c5d48c904da9d96a9e5dbb1113'
# Independent literals from the #337 protected-before table (the working package bytes; they equal the C blobs).
PACKAGE_PINS = {'Packages/manifest.json': ('f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c', 2297),
                'Packages/packages-lock.json': ('f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1', 14223)}
SIX = [p for p in tea.PATHS if p not in tea.PACKAGES]


def git(*args, check=True):
    p = subprocess.run(['git', '-C', str(HERE), '-c', 'core.commitGraph=false', *args], capture_output=True,
                       env=dict(os.environ, GIT_CONFIG_GLOBAL='/dev/null', GIT_CONFIG_NOSYSTEM='1', GIT_OPTIONAL_LOCKS='0'))
    if check and p.returncode: raise RuntimeError('git %s failed: %s' % (args, p.stderr.decode()))
    return p


def blob(rev, path): return git('cat-file', 'blob', rev + ':' + path).stdout
def sha256(data): return hashlib.sha256(data).hexdigest()


def post_state():
    mod = tea.load_script(tea.PROPOSED, tea.scratch_dir(), 'ptp_prod')
    return mod, mod.EXACT_ADMISSION_PROFILES[PID], mod.EXACT_ADMISSION_PROFILES[PID]['states']['post-transition']


class PinsDerivedFromGit(unittest.TestCase):
    def test_candidate_identity_and_ancestry(self):
        _, _, post = post_state()
        self.assertEqual(post['head'], CANDIDATE_C)
        self.assertEqual(git('cat-file', '-t', CANDIDATE_C).stdout.strip(), b'commit')
        self.assertEqual(post['must_descend_from'], [OPTION_B])
        for ancestor in (OPTION_B, OLD):
            self.assertEqual(git('merge-base', '--is-ancestor', ancestor, CANDIDATE_C, check=False).returncode, 0, ancestor)
        self.assertEqual(git('rev-list', '--count', OPTION_B + '..' + CANDIDATE_C).stdout.strip(), b'1')  # exactly one commit past Option B

    def test_two_clean_package_pins_equal_the_c_blobs_and_the_337_working_bytes(self):
        _, _, post = post_state()
        self.assertEqual(set(post['clean_pinned']), set(tea.PACKAGES))
        for path, (want_sha, want_size) in PACKAGE_PINS.items():
            data = blob(CANDIDATE_C, path)
            self.assertEqual((sha256(data), len(data)), (want_sha, want_size), path)
            self.assertEqual(post['clean_pinned'][path], (want_sha, want_size), path)
            self.assertEqual(blob(CANDIDATE_C, path), blob(OPTION_B, path), 'Phase A must not change the reviewed package bytes: ' + path)
            self.assertNotEqual(sha256(blob(OLD, path)), want_sha, 'the old HEAD blob must differ (that is why they were dirty)')

    def test_six_dirty_pins_keep_original_values_and_c_does_not_touch_them(self):
        _, prof, post = post_state()
        pre = prof['states']['pre-transition']['dirty']
        self.assertEqual(set(post['dirty']), set(SIX)); self.assertEqual(len(post['dirty']), 6)
        for path in SIX:
            self.assertEqual(post['dirty'][path], pre[path], path)               # identical (working sha, HEAD blob sha, size)
            w, h, n = post['dirty'][path]
            self.assertEqual(sha256(blob(CANDIDATE_C, path)), h, 'HEAD blob at C differs from the pinned HEAD blob: ' + path)
            self.assertEqual(blob(CANDIDATE_C, path), blob(OLD, path), 'C changes a protected path: ' + path)
            self.assertNotEqual(w, h)                                             # genuinely dirty bytes, not equal to HEAD

    def test_allowed_changed_paths_are_exactly_the_old_to_c_diff(self):
        _, _, post = post_state()
        real = sorted(git('diff', '--name-only', '--no-renames', OLD, CANDIDATE_C).stdout.decode().split('\n')[:-1])
        self.assertEqual(sorted(post['allowed_changed_paths']), real)
        self.assertEqual(len(real), 13)
        for p in real:
            self.assertTrue(p.startswith(('Tools/Verification/', 'Docs/')) or p in tea.PACKAGES, p)   # no Assets/, ProjectSettings/, .vscode/
        self.assertFalse(set(real) & set(SIX))
        status = dict(l.split('\t')[::-1] for l in git('diff-tree', '-r', '--no-renames', '--name-status', OLD, CANDIDATE_C).stdout.decode().splitlines())
        self.assertEqual(sorted(k for k, v in status.items() if v == 'A'), sorted(
            ['Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-WORKBENCH-287-VERIFIER-CLOSURE-RESULT.md',
             'Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md',
             'Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RUNNER-R1-R5-CORRECTION-RESULT.md',
             'Tools/Verification/RackV2Persistence/test_historical_projection.py', 'Tools/Verification/StandGeometry/test_b3_witness.py',
             'Tools/Verification/WorkbenchRetirement/test_runner.py']))
        self.assertEqual(sorted(k for k, v in status.items() if v == 'M'), sorted(
            [*tea.PACKAGES, 'Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs', 'Tools/Verification/StandGeometry/Enlarged80Verification.cs',
             'Tools/Verification/StandGeometry/run_enlarged80.py', 'Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs',
             'Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py']))

    def test_digest_is_reproducible_independently_and_provisional(self):
        mod, prof, post = post_state()
        got = mod.admission_digest(PID, 'post-transition')
        lines = [PID, 'post-transition', prof['lane_name'], prof['lane_path'], prof['branch'], CANDIDATE_C]
        lines += sorted('dirty\0%s\0%s\0%s\0%s' % (p, w, h, n) for p, (w, h, n) in post['dirty'].items())
        lines += sorted('clean\0%s\0%s\0%s' % (p, s, n) for p, (s, n) in post['clean_pinned'].items())
        self.assertEqual(got, sha256('\n'.join(lines).encode()))
        self.assertEqual(got, POST_DIGEST_PROVISIONAL, 'a pinned value changed: the provisional digest moved')
        self.assertNotEqual(got, mod.admission_digest(PID, 'pre-transition'))
        self.assertEqual(mod.admission_digest(PID, 'pre-transition'), tea.PROD_DIGEST_PRE)   # pre-transition digest untouched
        print('\nPOST-TRANSITION digest (PROVISIONAL, NOT ACCEPTED):', got)


class NothingElseChanged(unittest.TestCase):
    PROFILE_START, PROFILE_END = "EXACT_ADMISSION_PROFILES = {", "def admission_digest"

    @staticmethod
    def without_post_block_and_expiry_comment(text):
        a = text.index('            # After a separately rehearsed')
        b = text.index('        },\n    },\n}\n', a)
        text = text[:a] + '<POST-BLOCK>\n' + text[b:]
        return re.sub(r"('valid_until': '[^']*',)[^\n]*", r'\1', text)

    def test_only_post_block_and_expiry_comment_differ_from_reviewed_source(self):
        original = git('cat-file', 'blob', REVIEWED_SOURCE + ':' + PROPOSED_REL).stdout.decode()
        current = tea.PROPOSED.read_text(encoding='utf-8')
        self.assertEqual(self.without_post_block_and_expiry_comment(original), self.without_post_block_and_expiry_comment(current))
        # the expiry VALUE is the author placeholder from the reviewed source, unchanged and still not approved
        self.assertIn("'valid_until': '2026-11-11T00:00:00+00:00',", current)
        self.assertIn('NOT APPROVED, NOT ADOPTED', current)

    def test_loaded_profile_matches_reviewed_source_except_post_state(self):
        original = tea.scratch_dir() / 'orig-unity-lock'
        original.write_bytes(git('cat-file', 'blob', REVIEWED_SOURCE + ':' + PROPOSED_REL).stdout)
        old = tea.load_script(original, tea.scratch_dir(), 'ptp_old').EXACT_ADMISSION_PROFILES[PID]
        new = post_state()[1]
        self.assertEqual({k: v for k, v in old.items() if k != 'states'}, {k: v for k, v in new.items() if k != 'states'})
        self.assertEqual(old['states']['pre-transition'], new['states']['pre-transition'])
        self.assertEqual(old['states']['post-transition']['dirty'], new['states']['post-transition']['dirty'])
        self.assertEqual(old['states']['post-transition']['must_descend_from'], new['states']['post-transition']['must_descend_from'])
        self.assertIsNone(old['states']['post-transition']['head'])        # sanity: the reviewed source really was unfrozen

    def test_no_function_or_other_baseline_file_changed(self):
        changed = git('diff', '--name-only', REVIEWED_SOURCE, 'HEAD', '--', 'Tools/Verification/UnityLeaseExactAdmission').stdout.decode().split()
        self.assertTrue(set(changed) <= {'Tools/Verification/UnityLeaseExactAdmission/' + n for n in (
            'unity-lock.proposed', 'unity-lock-exact-admission.patch', 'test_exact_admission.py', 'test_post_transition_profile.py')}, changed)
        for untouched in ('unity-lock.deployed-54cf0a95.txt', '_ccf.deployed-ab4343ef.txt', 'test_raw_byte_admission.py', 'test_tree_object_integrity.py'):
            self.assertEqual(git('diff', '--quiet', REVIEWED_SOURCE, 'HEAD', '--', 'Tools/Verification/UnityLeaseExactAdmission/' + untouched, check=False).returncode, 0, untouched)


class MiniatureTransitionReplay(unittest.TestCase):
    """One disposable miniature repository: exact old and C blobs of the 21 relevant paths + the preserved #318 dirty bytes."""
    @classmethod
    def setUpClass(cls):
        _, cls.prof, cls.post = post_state()
        cls.evidence = Path(cls.prof['evidence_root'])
        cls.root = Path(tempfile.mkdtemp(prefix='ptp-replay-', dir=tea.TMP_ROOT))
        cls.base = cls.root / 'base'; cls.base.mkdir()
        env = dict(os.environ, **tea.GIT_ENV)
        def g(cwd, *a, check=True):
            p = subprocess.run(['git', '-C', str(cwd), *a], capture_output=True, env=env)
            if check and p.returncode: raise RuntimeError(p.stderr.decode())
            return p
        cls.g = staticmethod(g)
        g(cls.base, 'init', '-q', '-b', 'main'); g(cls.base, 'config', 'core.fileMode', 'false'); g(cls.base, 'config', 'core.autocrlf', 'false')
        changed = cls.post['allowed_changed_paths']
        for rev, tag in ((OLD, 'old'), (CANDIDATE_C, 'target')):
            for p in sorted(set(tea.PATHS) | set(changed)):
                exists = git('cat-file', '-e', rev + ':' + p, check=False).returncode == 0
                f = cls.base / p
                if exists:
                    f.parent.mkdir(parents=True, exist_ok=True); f.write_bytes(blob(rev, p)); g(cls.base, 'add', '--', p)
                elif f.exists():
                    g(cls.base, 'rm', '-q', '--', p)
            g(cls.base, 'commit', '-q', '-m', tag); g(cls.base, 'tag', tag)
        g(cls.base, 'checkout', '-q', 'old')
        for p in tea.PATHS: (cls.base / p).write_bytes((cls.evidence / 'dirty-backup' / p).read_bytes())  # preserved original eight dirty files

    @classmethod
    def tearDownClass(cls): shutil.rmtree(cls.root, ignore_errors=True)

    def copy(self, name):
        d = self.root / name; shutil.copytree(self.base, d); return d

    def status(self, d): return sorted(l[3:] for l in self.g(d, 'status', '--porcelain=v1').stdout.decode().splitlines())

    def test_start_state_is_the_eight_dirty_pre_transition_state(self):
        self.assertEqual(self.status(self.base), sorted(tea.PATHS))
        for p, (w, h, n) in self.prof['states']['pre-transition']['dirty'].items():
            data = (self.base / p).read_bytes(); self.assertEqual((sha256(data), len(data)), (w, n), p)
            self.assertEqual(sha256(self.g(self.base, 'cat-file', 'blob', 'HEAD:' + p).stdout), h, p)

    def test_plain_fast_forward_switch_and_checkout_are_refused_and_change_nothing(self):
        for name, cmd in (('ff', ['merge', '--ff-only', 'target']), ('switch', ['switch', '--detach', 'target']), ('checkout', ['checkout', 'target'])):
            d = self.copy('plain-' + name); p = self.g(d, *cmd, check=False)
            self.assertNotEqual(p.returncode, 0, name)
            msg = (p.stdout + p.stderr).decode()
            for pkg in tea.PACKAGES: self.assertIn(pkg, msg, name)
            self.assertEqual(self.g(d, 'rev-parse', 'HEAD').stdout, self.g(d, 'rev-parse', 'old').stdout, name)
            self.assertEqual(self.status(d), sorted(tea.PATHS), name)

    def test_staging_only_the_two_packages_then_ff_only_gives_exactly_the_post_state(self):
        d = self.copy('staged-ff')
        self.g(d, 'add', *tea.PACKAGES)
        self.assertEqual(sorted(self.g(d, 'diff', '--cached', '--name-only').stdout.decode().split()), sorted(tea.PACKAGES))
        p = self.g(d, 'merge', '--ff-only', 'target', check=False); self.assertEqual(p.returncode, 0, p.stderr)
        self.assertEqual(self.g(d, 'rev-parse', 'HEAD').stdout, self.g(d, 'rev-parse', 'target').stdout)
        self.assertEqual(self.status(d), sorted(SIX))                               # 8 -> 6 dirty
        self.assertEqual(self.g(d, 'diff', '--cached', '--name-only').stdout, b'')  # nothing left staged
        self.assertTrue(all(l.startswith('H ') for l in self.g(d, 'ls-files', '-v').stdout.decode().splitlines()))
        for p, (w, h, n) in self.post['dirty'].items():                              # the six keep their exact preserved bytes
            data = (d / p).read_bytes(); self.assertEqual((sha256(data), len(data)), (w, n), p)
            self.assertEqual(sha256(self.g(d, 'cat-file', 'blob', 'HEAD:' + p).stdout), h, p)
        for p, (s, n) in self.post['clean_pinned'].items():                          # the two packages now equal the tracked blobs and the pins
            data = (d / p).read_bytes(); self.assertEqual((sha256(data), len(data)), (s, n), p)
            self.assertEqual(sha256(self.g(d, 'cat-file', 'blob', 'HEAD:' + p).stdout), s, p)
        changed = sorted(self.g(d, 'diff', '--name-only', '--no-renames', 'old', 'HEAD').stdout.decode().split())
        self.assertEqual(changed, sorted(self.post['allowed_changed_paths']))        # the profile's list is exactly this transition

    def test_not_transitioned_fixture_fails_the_post_pins(self):
        # The wrong-HEAD / not-yet-transitioned state: its HEAD blobs for the packages are the OLD bytes, so the post pins cannot match.
        for p, (s, n) in self.post['clean_pinned'].items():
            self.assertNotEqual(sha256(self.g(self.base, 'cat-file', 'blob', 'HEAD:' + p).stdout), s, p)
        self.assertNotEqual(self.g(self.base, 'rev-parse', 'HEAD').stdout.decode().strip(), self.g(self.base, 'rev-parse', 'target').stdout.decode().strip())


if __name__ == '__main__':
    unittest.main(verbosity=2)
