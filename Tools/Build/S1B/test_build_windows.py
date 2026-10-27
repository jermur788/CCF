#!/usr/bin/env python3
"""Bounded, Unity-free tests for the Windows candidate build tooling (build_windows.py).

Same style as test_build_linux.py: a disposable mock project + a mock Unity executable. Never launches Unity and never
fabricates a real Windows build; it exercises the orchestrator's control flow (the licence, TMPDIR-socket and art-pack
guards it shares with build_linux.py, output/identity handling, failure paths), the tester README the builder ships, and
the Editor-still-running check of the legacy HDR wrapper.

Run:  python3 Tools/Build/S1B/test_build_windows.py
Exit 0 = all checks PASS. Scratch folders go under $CCF_TEST_TMP, else ZX20/CCF-Temp when present, else the system temp folder.
"""
import importlib.util
import json
import os
import shutil
import socket
import subprocess
import sys
import tempfile
import zipfile
from pathlib import Path

sys.dont_write_bytecode = True   # the repo does not ignore __pycache__; importing the tools must not litter it
ROOT = Path(__file__).resolve().parents[3]
S1B = Path(__file__).resolve().parent
checks = 0


def check(cond, label):
    global checks
    assert cond, label
    checks += 1
    print("PASS:", label, flush=True)


sys.path.insert(0, str(S1B))   # build_windows imports its sibling art_pack


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def mock_script(path, body):
    path.write_text("#!/usr/bin/env python3\n" + body)
    path.chmod(0o755)


def scratch_parent():
    explicit = os.environ.get("CCF_TEST_TMP")
    if explicit:
        return Path(explicit)
    temp = Path("/media/jer/ZX20/CCF-Temp")
    return temp if temp.is_dir() else None


def git_init(path):
    subprocess.run(["git", "-C", str(path), "init", "-q"], check=True)
    for k, v in [("user.email", "t@invalid"), ("user.name", "t"), ("commit.gpgsign", "false"), ("core.hooksPath", "/dev/null")]:
        subprocess.run(["git", "-C", str(path), "config", k, v], check=True)
    subprocess.run(["git", "-C", str(path), "add", "-A"], check=True)
    subprocess.run(["git", "-C", str(path), "commit", "-q", "-m", "seed"], check=True)


def make_project(path, manifest=None):
    """A minimal committed project with everything build_windows.py reads from the project."""
    (path / "Assets/ForestPrototype/Editor").mkdir(parents=True)     # the real project has this Editor assembly folder
    (path / "Assets/ForestPrototype/Editor/.gitkeep").write_text("")
    (path / "Assets/Scenes").mkdir(parents=True)
    (path / "Assets/Scenes/x.unity").write_text("m_Prefab: {guid: %s, type: 3}\n" % ("a" * 32))
    (path / "ProjectSettings").mkdir()
    (path / "ProjectSettings/ProjectSettings.asset").write_text("original\n")
    (path / "Tools/Build/S1B/Notices").mkdir(parents=True)
    (path / "Tools/Verification/S1B").mkdir(parents=True)
    shutil.copy2(S1B / "WindowsCandidateBuilder.cs", path / "Tools/Build/S1B/WindowsCandidateBuilder.cs")
    (path / "Tools/Build/S1B/Notices/UnityPlayer-Windows-Mono-6000.6.0f1.pdf").write_bytes(b"%PDF mock\n")
    (path / "Docs/Verification/S1B").mkdir(parents=True)
    shutil.copy2(ROOT / "Docs/Verification/S1B/TesterREADME.md", path / "Docs/Verification/S1B/TesterREADME.md")
    (path / "Docs/Verification/S1B/RightsAudit.md").write_text("rights audit\n")
    (path / "Library/PackageCache/com.test.pkg").mkdir(parents=True)
    (path / "Library/PackageCache/com.test.pkg/LICENSE.md").write_text("package licence\n")
    if manifest is not None:
        (path / "Tools/Build/S1B/art-pack-manifest.json").write_text(manifest)
    (path / ".gitignore").write_text("/Library/\n/Build/\n/Assets/InnerverseInteractive/\n/Assets/InnerverseInteractive.meta\n")
    git_init(path)
    return path


MOCK_UNITY = (
    "import json,os,sys\n"
    "a=sys.argv\n"
    "log=a[a.index('-logFile')+1]\n"
    "mode=os.environ.get('MOCK_MODE','ok')\n"
    "proj=a[a.index('-projectPath')+1]\n"
    "if not os.path.isfile(os.path.join(proj,'Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs')) or 'WindowsCandidateBuilder.Build' not in a:\n"
    "    open(log,'w').write('builder not staged in the Editor folder\\n'); sys.exit(9)\n"
    "lic=os.path.join(os.environ['XDG_CONFIG_HOME'],'unity3d/Unity/licenses/LICENCE.txt')\n"
    "seen=open(lic).read().strip() if os.path.exists(lic) else 'NONE'\n"
    "if mode=='fail':\n"
    "    open(log,'w').write('error CS mock\\n'); sys.exit(7)\n"
    "out=os.environ['CCF_BUILD_OUTPUT']\n"
    "open(log,'w').write(('S1B_WINDOWS_BUILD_PASS mock\\n' if mode!='nomarker' else 'nothing\\n')+'LICENCE_SEEN '+seen+'\\n'+'IDENTITY_SEEN '+os.environ['CCF_BUILD_IDENTITY']+'\\n')\n"
    "stamp=os.path.join(proj,'Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json')\n"
    "os.makedirs(os.path.dirname(stamp),exist_ok=True); open(stamp,'w').write(os.environ['CCF_BUILD_IDENTITY'])\n"
    "open(os.path.join(proj,'ProjectSettings/ProjectSettings.asset'),'w').write('rewritten by a build callback\\n')\n"
    "os.makedirs(os.path.join(out,'CCF_Data'),exist_ok=True)\n"
    "open(os.path.join(out,'CCF.exe'),'w').write('exe'); open(os.path.join(out,'UnityPlayer.dll'),'w').write('dll')\n"
    "os.makedirs(os.path.join(out,'CCF_BurstDebugInformation_DoNotShip'),exist_ok=True)\n"
    "open(os.path.join(out,'CCF_BurstDebugInformation_DoNotShip','x.txt'),'w').write('debug')\n"
    "if mode=='forbidden': open(os.path.join(out,'leak.cs'),'w').write('class X{}')\n"
    "sys.exit(0)\n")


def main():
    parent = scratch_parent()
    scratch = Path(tempfile.mkdtemp(prefix="windows-tooling-test-", dir=str(parent) if parent else None))
    # Unity (and the tool's preflight) need a socket-capable TMPDIR; the exFAT scratch drive is not one.
    sockdir = tempfile.mkdtemp(prefix="ccf-sock-", dir=os.environ.get("XDG_RUNTIME_DIR") or "/tmp")
    os.environ["TMPDIR"] = sockdir
    fake_editor = None
    try:
        # 1. syntax / import / interface
        for script in [S1B / "build_windows.py", S1B / "with_demo_hdr_quarantined.py", S1B / "test_build_windows.py"]:
            subprocess.run([sys.executable, "-B", "-c",
                            "import py_compile,sys; py_compile.compile(sys.argv[1], cfile=sys.argv[2], doraise=True)",
                            str(script), str(scratch / (script.name + ".pyc"))], check=True)
        bw = load("build_windows", S1B / "build_windows.py")
        check(True, "python compiles and imports")
        helptext = subprocess.run([sys.executable, "-B", str(S1B / "build_windows.py"), "--help"], capture_output=True, text=True).stdout
        check(all(flag in helptext for flag in ("--license-dir", "--art-pack", "--no-art-pack-check", "--project", "--output-root", "--sequence")),
              "--help lists the licence, art-pack, project and output options")
        source = (S1B / "build_windows.py").read_text()
        check("/home/" not in source and "/media/jer/ZX20/CCF" not in source, "no checkout-specific path in the orchestrator")
        check("CCF.x86_64" not in source and "LinuxPlayerBuilder" not in source, "no Linux target leaks into the Windows orchestrator")

        # 2. mock project + mock Unity
        proj = make_project(scratch / "proj")
        mock = scratch / "mock-unity"
        mock_script(mock, MOCK_UNITY)
        outroot = scratch / "out"
        base = ["--project", str(proj), "--unity", str(mock), "--output-root", str(outroot), "--date", "20261016", "--sequence", "B01",
                "--no-art-pack-check"]
        script = str(S1B / "build_windows.py")
        home = scratch / "home"            # an isolated $HOME: the default licence folder is looked up under it
        (home / ".config/unity3d/Unity/licenses").mkdir(parents=True)
        (home / ".config/unity3d/Unity/licenses/LICENCE.txt").write_text("DEFAULT\n")
        env = dict(os.environ, MOCK_MODE="ok", HOME=str(home))

        def build(*extra, mode="ok", use_base=True, environ=None):
            return subprocess.run([sys.executable, "-B", script, *(base if use_base else []), *extra], capture_output=True, text=True,
                                  env=dict(environ or env, MOCK_MODE=mode))

        # 3. success path
        p = build()
        check(p.returncode == 0 and '"status": "PASS"' in p.stdout, "success path PASS: " + p.stdout[-300:] + p.stderr[-300:])
        sha = subprocess.check_output(["git", "-C", str(proj), "rev-parse", "HEAD"], text=True).strip()
        build_id = "CCF-S1-20261016-B01-" + sha[:7]
        built = outroot / (build_id + "-Windows-x64")
        check((built / "CCF.exe").is_file() and (built / "UnityPlayer.dll").is_file() and (built / "CCF_Data").is_dir()
              and (built / "package-manifest.json").is_file() and (built / "build-identity.json").is_file(), "windows layout, manifest and identity written")
        archive = outroot / (build_id + "-Windows-x64.zip")
        check(archive.is_file() and "README.txt" in " ".join(zipfile.ZipFile(archive).namelist()), "archive contains the README")
        check(f'"archive_sha256"' in p.stdout and json.loads(p.stdout)["gitSha"] == sha, "result records the full SHA and the archive hash")
        readme = (built / "README.txt").read_text()
        check(build_id in readme and sha in readme and "@BUILD_ID@" not in readme and "@GIT_SHA@" not in readme, "README.txt carries the build ID and full SHA")
        check(not (proj / "Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs").exists()
              and not (proj / "Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json").exists(),
              "builder staged in the Editor folder (mock Unity enforces it) and its staged files removed afterwards")
        check((proj / "ProjectSettings/ProjectSettings.asset").read_text() == "original\n"
              and (outroot / (build_id + "-source-normalisation.diff")).read_text().count("rewritten by a build callback") == 1
              and subprocess.check_output(["git", "-C", str(proj), "status", "--porcelain"], text=True) == "",
              "build-rewritten settings restored byte-exactly and the diff kept")
        check((outroot / (build_id + "-developer-backups/CCF_BurstDebugInformation_DoNotShip")).is_dir()
              and not (built / "CCF_BurstDebugInformation_DoNotShip").exists(), "DoNotShip folders moved beside the candidate")
        check((built / "Notices/UnityPlayer-Windows-Mono-6000.6.0f1.pdf").is_file() and (built / "Notices/com.test.pkg/LICENSE.md").is_file()
              and (built / "Notices/RightsAudit.md").is_file(), "notices copied")
        ident = json.loads((built / "build-identity.json").read_text())
        check(ident["gitSha"] == sha and ident["platform"] == "Windows-x64" and ident["saveContract"] == "same-build only"
              and ident["nativeWindowsVerified"] is False, "identity: SHA, platform, same-build save contract, native Windows unverified")
        check(ident["artPack"] == {"verified": False, "reason": "--no-art-pack-check"}, "skipped art-pack check is recorded as unverified")
        log_text = next(outroot.glob("*-build.log")).read_text()
        check('"artPack"' in log_text and "LICENCE_SEEN DEFAULT" in log_text, "Unity is given the identity with the art-pack record and the default licence")

        # 4. refusals and failure paths
        p = build()
        check(p.returncode != 0 and "refusing to overwrite" in p.stderr, "existing output refused cleanly")
        p = build("--sequence", "B02", mode="fail")
        check(p.returncode != 0 and "Build failed" in p.stderr, "nonzero Unity exit reported")
        check(not (proj / "Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs").exists(), "staged builder removed after a failed build")
        p = build("--sequence", "B03", mode="nomarker")
        check(p.returncode != 0 and "Missing build success marker" in p.stderr, "missing success marker reported")
        p = build("--sequence", "B04", mode="forbidden")
        check(p.returncode != 0 and "forbidden tester payload" in p.stderr and "leak.cs" in p.stderr, "forbidden payload stops packaging")
        p = build("--sequence", "L01")
        check(p.returncode != 0 and "Invalid date/sequence" in p.stderr, "a non-B sequence is refused")
        (proj / "Docs/Verification/S1B/RightsAudit.md").write_text("dirty\n")
        p = build("--sequence", "B05")
        check(p.returncode != 0 and "committed and clean" in p.stderr, "dirty source refused")
        subprocess.run(["git", "-C", str(proj), "checkout", "--", "Docs/Verification/S1B/RightsAudit.md"], check=True)

        # 5. explicit Unity licence folder
        lic = scratch / "valid-licence"
        lic.mkdir()
        (lic / "LICENCE.txt").write_text("VALID\n")
        out_lic = scratch / "out-lic"
        p = subprocess.run([sys.executable, "-B", script, *base, "--output-root", str(out_lic), "--license-dir", str(lic)],
                           capture_output=True, text=True, env=env)
        log_lic = next(out_lic.glob("*-build.log")).read_text() if out_lic.exists() else ""
        check(p.returncode == 0 and "LICENCE_SEEN VALID" in log_lic, "--license-dir licence is the one Unity sees (not the stale default)")
        out_nolic = scratch / "out-nolic"
        p = subprocess.run([sys.executable, "-B", script, *base, "--output-root", str(out_nolic), "--license-dir", str(scratch / "does-not-exist")],
                           capture_output=True, text=True, env=env)
        check(p.returncode != 0 and "--license-dir folder not found" in p.stderr and not out_nolic.exists(),
              "missing --license-dir folder stops the build before any output")
        empty_home = scratch / "empty-home"
        empty_home.mkdir()
        p = subprocess.run([sys.executable, "-B", script, *base, "--output-root", str(scratch / "out-nodefault")],
                           capture_output=True, text=True, env=dict(env, HOME=str(empty_home)))
        check(p.returncode == 0, "an absent default licence folder no longer crashes the orchestrator (Unity reports it)")

        # 6. socket-capable TMPDIR preflight
        class NoSockets:
            AF_UNIX = socket.AF_UNIX
            SOCK_STREAM = socket.SOCK_STREAM

            def __init__(self, *args, **kwargs):
                pass

            def bind(self, path):
                raise OSError(95, "Operation not supported")

            def close(self):
                pass
        real_socket = bw.socket
        bw.socket = type("S", (), {"socket": NoSockets, "AF_UNIX": socket.AF_UNIX, "SOCK_STREAM": socket.SOCK_STREAM})
        try:
            bw.require_socket_capable_tmpdir()
            guard = None
        except SystemExit as error:
            guard = str(error)
        finally:
            bw.socket = real_socket
        check(guard is not None and "cannot create Unix sockets" in guard and "socket-capable" in guard, "a TMPDIR that cannot hold Unix sockets stops with the real reason")
        bw.require_socket_capable_tmpdir()
        check(True, "a socket-capable TMPDIR passes the guard")
        # The guard probes <TMPDIR>/.ccf-sock-<pid> and skips paths that do not fit sun_path (<100 bytes); only an honest
        # bind failure on a short path (exFAT) can exercise it end to end.
        short_enough = len((str(scratch) + "/.ccf-sock-9999999").encode()) < 100
        probe = socket.socket(socket.AF_UNIX, socket.SOCK_STREAM)
        try:
            probe.bind(str(scratch / ".s")) if short_enough else None
            scratch_blocks_sockets = False
        except OSError:
            scratch_blocks_sockets = True
        finally:
            probe.close()
        if scratch_blocks_sockets:
            p = subprocess.run([sys.executable, "-B", script, *base, "--output-root", str(scratch / "out-sock")],
                               capture_output=True, text=True, env=dict(env, TMPDIR=str(scratch)))
            check(p.returncode != 0 and "cannot create Unix sockets" in p.stderr and not (scratch / "out-sock").exists(),
                  "end to end: an exFAT-style TMPDIR stops the build before any output")
        else:
            check(True, "end-to-end TMPDIR guard (scratch filesystem supports sockets here; the guard itself is exercised above)")

        # 7. art-pack preflight, end to end
        ap = load("art_pack", S1B / "art_pack.py")
        packs = scratch / "packs"

        def make_pack(root, tamper=None):
            base_dir = root / "Ultimate Nature \u2013 Starter/Env"
            base_dir.mkdir(parents=True)

            def asset(name, data, guid):
                (base_dir / name).write_bytes(data)
                (base_dir / (name + ".meta")).write_text(
                    "fileFormatVersion: 2\nguid: %s\nAssetOrigin:\n  serializedVersion: 1\n  productId: 176906\n"
                    "  packageName: \"Ultimate Nature \\u2013 Starter\"\n  packageVersion: 1.3.0\n  uploadId: 964150\n" % guid)
            asset("a.prefab", b"%YAML prefab refs " + b"c" * 32, "a" * 32)
            asset("b.fbx", b"fbx-bytes", "b" * 32)
            asset("c.png", b"png-bytes", "c" * 32)
            asset("UNS_HDRI.hdr", b"huge demo hdr", "d" * 32)    # the pack's demonstration HDR: not referenced, must never be installed
            if tamper == "reqbytes":
                (base_dir / "c.png").write_bytes(b"other")
            return root

        pkproj = scratch / "pkproj"
        (pkproj / "Assets/Scenes").mkdir(parents=True)
        (pkproj / "Assets/Scenes/x.unity").write_text("m_Prefab: {guid: %s, type: 3}\n" % ("a" * 32))
        good = make_pack(packs / "good" / "InnerverseInteractive")
        mani = ap.generate_manifest(pkproj, good)
        mpath = scratch / "mani.json"
        mpath.write_text(json.dumps(mani, ensure_ascii=False))
        proj2 = make_project(scratch / "proj2", manifest=mpath.read_text())
        out2 = scratch / "out2"
        base2 = ["--project", str(proj2), "--unity", str(mock), "--output-root", str(out2), "--date", "20261016", "--sequence", "B01"]
        envk = dict(env)
        envk.pop("CCF_ART_PACK_DIR", None)
        p = subprocess.run([sys.executable, "-B", script, *base2], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "required but not available" in p.stderr and "--art-pack" in p.stderr
              and not out2.exists() and not (proj2 / "Assets/InnerverseInteractive").exists()
              and not (proj2 / "Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs").exists(),
              "missing pack stops the build before output/staging with a clear message")
        wrong = make_pack(packs / "wrongpack" / "InnerverseInteractive", tamper="reqbytes")
        p = subprocess.run([sys.executable, "-B", script, *base2, "--art-pack", str(wrong)], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "does not match" in p.stderr and not out2.exists(), "wrong pack stops the build")
        p = subprocess.run([sys.executable, "-B", script, *base2, "--art-pack", str(good)], capture_output=True, text=True, env=envk)
        check(p.returncode == 0 and '"status": "PASS"' in p.stdout, "explicit --art-pack builds: " + p.stderr[-300:])
        pack_dir = proj2 / "Assets/InnerverseInteractive"
        installed = sorted(x.relative_to(pack_dir).as_posix() for x in pack_dir.rglob("*") if x.is_file())
        check(installed == ["Ultimate Nature \u2013 Starter/Env/a.prefab", "Ultimate Nature \u2013 Starter/Env/a.prefab.meta",
                            "Ultimate Nature \u2013 Starter/Env/c.png", "Ultimate Nature \u2013 Starter/Env/c.png.meta"],
              "only the required files are installed: the demo HDR is never imported, so the HDR wrapper is not needed")
        check(subprocess.check_output(["git", "-C", str(proj2), "status", "--porcelain"], text=True) == "",
              "installed pack is gitignored; the committed tree stays clean")
        built2 = next(out2.glob("*-Windows-x64"))
        ident2 = json.loads((built2 / "build-identity.json").read_text())
        pm2 = json.loads((built2 / "package-manifest.json").read_text())
        ap2 = ident2["artPack"]
        check(ap2["verified"] is True and ap2["requiredSetSha256"] == mani["requiredSetSha256"] and ap2["packageVersion"] == "1.3.0"
              and ap2["assetStoreProductId"] == "176906" and ap2["committedToRepository"] is False and pm2["identity"]["artPack"] == ap2,
              "build identity + package manifest record the verified pack (version, product, checksum)")
        p = subprocess.run([sys.executable, "-B", script, *base2, "--sequence", "B02"], capture_output=True, text=True, env=envk)
        check(p.returncode == 0, "already-installed pack is verified in place (no --art-pack needed)")
        (pack_dir / "Ultimate Nature \u2013 Starter/Env/a.prefab").write_bytes(b"corrupt")
        p = subprocess.run([sys.executable, "-B", script, *base2, "--sequence", "B03"], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "installed in the project" in p.stderr and "different bytes" in p.stderr, "corrupted installed pack stops the build")

        # 8. the tester README that the builder ships (RB2)
        text = (ROOT / "Docs/Verification/S1B/TesterREADME.md").read_text()
        for phrase in ["@BUILD_ID@", "@GIT_SHA@", "Start New Scenario", "Save version 20", "RNG 1", "regeneration model 2", "growth model 2",
                       "Plan rack", "Approve Work Plan", "B in the forest", "Stand Map", "two management cycles", "racks first",
                       'CCF.exe -logFile "%USERPROFILE%\\Desktop\\CCF-Player.log"', "F5", "F9", "F10", "SAME build", "Windows protected your PC",
                       "WINE COMPATIBILITY SMOKE", "unverified"]:
            check(phrase in text, "tester README states: " + phrase)
        for stale in ["Stop after post-intervention", "outside this pilot", "Save 19", "growth 1", "Start a fresh Scenario One test and confirm",
                      "first-cycle candidate", "one management cycle"]:
            check(stale not in text, "tester README no longer says: " + stale)

        # 9. the legacy HDR wrapper no longer depends on one checkout
        wrapper = load("hdr_wrapper", S1B / "with_demo_hdr_quarantined.py")
        pattern = wrapper.editor_pattern(Path("/work/CCF.u1"))
        check("/home/jer" not in pattern and "CCF-s1b" not in pattern, "wrapper's Editor check names no fixed checkout")
        fake_editor = subprocess.Popen([sys.executable, "-c", "import time; time.sleep(60)", "/opt/Unity/Editor/Unity", "-projectPath", str(proj)])
        # the fake's command line reads "... /opt/Unity/Editor/Unity -projectPath <proj>"; give it time to exec
        for _ in range(50):
            hit = subprocess.run(["pgrep", "-af", wrapper.editor_pattern(proj)], capture_output=True, text=True).stdout
            if hit.strip():
                break
            subprocess.run(["sleep", "0.1"])
        other = subprocess.run(["pgrep", "-af", wrapper.editor_pattern(scratch / "some-other-checkout")], capture_output=True, text=True).stdout
        prefix = subprocess.run(["pgrep", "-af", wrapper.editor_pattern(Path(str(proj)[:-1]))], capture_output=True, text=True).stdout
        check(str(proj) in hit and not other.strip() and not prefix.strip(),
              "wrapper's Editor check finds an Editor on this checkout and ignores other checkouts (also a path that is a prefix of this one)")
        fake_editor.kill()
        fake_editor.wait()
        fake_editor = None

        print(f"\nALL {checks} CHECKS PASS\nscratch: {scratch}")
        return 0
    finally:
        if fake_editor is not None:
            fake_editor.kill()
        shutil.rmtree(scratch, ignore_errors=True)
        shutil.rmtree(sockdir, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
