#!/usr/bin/env python3
"""Bounded, Unity-free tests for the Linux Player build/smoke tooling.

Uses a disposable mock project + mock Unity/Player executables under ZX20/CCF-Temp.
Never launches Unity and never fabricates a real Linux build: it only exercises the
orchestrator's control flow, path/id generation, output validation and failure paths.

Run:  python3 Tools/Build/S1B/test_build_linux.py
Exit 0 = all checks PASS.
"""
import importlib.util
import json
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
S1B = Path(__file__).resolve().parent
ROOT_STORE = Path("/media/jer/ZX20")
checks = 0


def check(cond, label):
    global checks
    assert cond, label
    checks += 1
    print("PASS:", label, flush=True)


sys.path.insert(0, str(S1B))   # build_linux imports its sibling art_pack


def load(name, path):
    spec = importlib.util.spec_from_file_location(name, path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def run(script, *args, expect=None, cwd=None):
    p = subprocess.run([sys.executable, str(script), *args], capture_output=True, text=True, cwd=cwd)
    if expect is not None:
        assert p.returncode == expect, f"expected {expect} got {p.returncode}: {script} {args}\n{p.stdout}\n{p.stderr}"
    return p


def mock_script(path, body):
    path.write_text("#!/usr/bin/env python3\n" + body)
    path.chmod(0o755)


def main():
    scratch = Path(tempfile.mkdtemp(prefix="linux-tooling-test-", dir=ROOT_STORE / "CCF-Temp"))
    try:
        # 1. syntax / import
        for script in [S1B / "build_linux.py", S1B / "run_linux_player_smoke.py"]:
            run("-c", f"import py_compile,sys; py_compile.compile('{script}', doraise=True)", expect=0)
        bl = load("build_linux", S1B / "build_linux.py")
        rs = load("run_smoke", S1B / "run_linux_player_smoke.py")
        check(True, "python compiles and imports")

        # 2. pure helpers
        bid = bl.build_id_for("20261016", "L01", "a" * 40, False)
        check(bid == "CCF-S1-20261016-L01-aaaaaaa" and bl.build_id_for("20261016", "L01", "a" * 40, True).endswith("-SMOKE"),
              "build id + smoke suffix")
        out = bl.output_dir_for("/x/S1B", bid)
        check(str(out) == "/x/S1B/CCF-S1-20261016-L01-aaaaaaa-Linux-x64", "output dir shape")
        cmd = bl.unity_command("/u/Unity", "/p", "/l/build.log")
        check(cmd[:1] == ["/u/Unity"] and "LinuxPlayerBuilder.Build" in cmd and "-batchmode" in cmd, "unity command")
        check(bl.LINUX_BINARY == "CCF.x86_64" and "Windows" not in " ".join(cmd), "linux target naming, no windows leak")
        check(b"S1B_LINUX_BUILD_PASS" in subprocess.check_output(["grep", "-a", "S1B_LINUX_BUILD_PASS", str(S1B / "LinuxPlayerBuilder.cs")]),
              "linux builder emits linux pass marker")
        check(Path(S1B / "build_linux.py").read_text().count("CCF.exe") == 0, "no .exe assumption in linux orchestrator")

        # 3. mock project
        proj = scratch / "proj"
        (proj / "Assets/ForestPrototype").mkdir(parents=True)
        (proj / "Tools/Build/S1B").mkdir(parents=True)
        (proj / "Tools/Verification/S1B").mkdir(parents=True)
        shutil.copy2(S1B / "LinuxPlayerBuilder.cs", proj / "Tools/Build/S1B/LinuxPlayerBuilder.cs")
        shutil.copy2(ROOT / "Tools/Verification/S1B/StandalonePlayerSmoke.cs", proj / "Tools/Verification/S1B/StandalonePlayerSmoke.cs")
        (proj / "keep.txt").write_text("seed\n")
        subprocess.run(["git", "-C", str(proj), "init", "-q"], check=True)
        for k, v in [("user.email", "t@invalid"), ("user.name", "t"), ("commit.gpgsign", "false"), ("core.hooksPath", "/dev/null")]:
            subprocess.run(["git", "-C", str(proj), "config", k, v], check=True)
        subprocess.run(["git", "-C", str(proj), "add", "-A"], check=True)
        subprocess.run(["git", "-C", str(proj), "commit", "-q", "-m", "seed"], check=True)

        mock = scratch / "mock-unity"
        mock_script(mock, (
            "import os,sys\n"
            "a=sys.argv\n"
            "log=a[a.index('-logFile')+1] if '-logFile' in a else None\n"
            "mode=os.environ.get('MOCK_MODE','ok')\n"
            "proj=a[a.index('-projectPath')+1]\n"
            "if not os.path.isfile(os.path.join(proj,'Assets/ForestPrototype/Editor/LinuxPlayerBuilder.cs')) or os.path.exists(os.path.join(proj,'Assets/ForestPrototype/LinuxPlayerBuilder.cs')):\n"
            "    open(log,'w').write('builder not staged in the Editor folder only\\n'); sys.exit(9)\n"
            "if mode=='sleep':\n"
            "    import time; time.sleep(30)\n"
            "if mode=='fail':\n"
            "    open(log,'w').write('error CS mock\\n'); sys.exit(7)\n"
            "out=os.environ.get('CCF_BUILD_OUTPUT','')\n"
            "if log: open(log,'w').write(('S1B_LINUX_BUILD_PASS mock\\n' if mode!='nooutput' else 'nothing\\n'))\n"
            "if mode=='ok':\n"
            "    os.makedirs(os.path.join(out,'CCF_Data'),exist_ok=True)\n"
            "    b=os.path.join(out,'CCF.x86_64'); open(b,'w').write('bin')\n"
            "    os.chmod(b,0o755)\n"
            "    open(os.path.join(out,'UnityPlayer.so'),'w').write('so')\n"
            "os.makedirs(os.path.join(os.environ['XDG_CONFIG_HOME'],'unity3d'),exist_ok=True)\n"
            "sys.exit(0)\n"))

        outroot = scratch / "out"
        base = ["--project", str(proj), "--unity", str(mock), "--output-root", str(outroot), "--date", "20261016", "--sequence", "L01",
                "--no-art-pack-check"]

        # 4. success
        env = dict(os.environ, MOCK_MODE="ok")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base], capture_output=True, text=True, env=env)
        check(p.returncode == 0 and '"status": "PASS"' in p.stdout, "success path PASS")
        proj_sha = subprocess.check_output(["git", "-C", str(proj), "rev-parse", "HEAD"], text=True).strip()
        built = outroot / (bl.build_id_for("20261016", "L01", proj_sha, False) + "-Linux-x64")
        check((built / "CCF.x86_64").is_file() and os.access(built / "CCF.x86_64", os.X_OK)
              and (built / "CCF_Data").is_dir() and (built / "UnityPlayer.so").is_file(), "linux layout validated")
        check((built / "package-manifest.json").is_file() and (built / "build-identity.json").is_file(), "manifest + identity written")
        check(not (proj / "Assets/ForestPrototype/Editor/LinuxPlayerBuilder.cs").exists()
              and not (proj / "Assets/ForestPrototype/Editor/LinuxPlayerBuilder.cs.meta").exists(),
              "builder staged in the Editor folder (mock Unity enforces it) and removed afterwards")
        ident = json.loads((built / "build-identity.json").read_text())
        check(ident["gitSha"] == proj_sha and ident["toolingSource"]["gitSha"] == proj_sha
              and ident["toolingSource"]["builderStagedAt"] == "Assets/ForestPrototype/Editor/LinuxPlayerBuilder.cs"
              and "Tools/Build/S1B/LinuxPlayerBuilder.cs" in ident["toolingSource"]["files"], "tooling source recorded in the build identity")
        check(ident["artPack"] == {"verified": False, "reason": "--no-art-pack-check"}, "skipped art-pack check is recorded as unverified")

        # 5. existing output refused
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base], capture_output=True, text=True, env=env)
        check(p.returncode != 0 and "refusing to overwrite" in p.stderr, "existing output refused cleanly")

        # 6. nonzero process
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base, "--sequence", "L02"],
                           capture_output=True, text=True, env=dict(os.environ, MOCK_MODE="fail"))
        check(p.returncode == 1 and '"reason": "unity exit"' in p.stdout, "nonzero exit reported")

        # 7. missing marker/no output
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base, "--sequence", "L03"],
                           capture_output=True, text=True, env=dict(os.environ, MOCK_MODE="nooutput"))
        check(p.returncode == 1 and "S1B_LINUX_BUILD_PASS" in p.stdout, "missing marker reported")

        # 8. timeout
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base, "--sequence", "L04", "--timeout", "2"],
                           capture_output=True, text=True, env=dict(os.environ, MOCK_MODE="sleep"))
        check(p.returncode == 2 and '"reason": "timeout"' in p.stdout, "timeout handled")

        # 9. dirty source refused
        (proj / "keep.txt").write_text("dirty\n")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base, "--sequence", "L05"],
                           capture_output=True, text=True, env=env)
        check(p.returncode != 0 and "committed and clean" in p.stderr, "dirty source refused")
        subprocess.run(["git", "-C", str(proj), "checkout", "--", "keep.txt"], check=True)

        # 9b. art-pack dependency: manifest generation, verification, explicit install, and the end-to-end preflight
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
            asset("unused.png", b"not needed", "d" * 32)
            if tamper == "unrequired":
                (base_dir / "b.fbx").write_bytes(b"other")
            if tamper == "reqbytes":
                (base_dir / "c.png").write_bytes(b"other")
            if tamper == "guid":
                (base_dir / "c.png.meta").write_text((base_dir / "c.png.meta").read_text().replace("c" * 32, "e" * 32))
            if tamper == "missing":
                (base_dir / "c.png").unlink()
            return root

        pkproj = scratch / "pkproj"
        (pkproj / "Assets/Scenes").mkdir(parents=True)
        (pkproj / "Assets/Scenes/x.unity").write_text("m_Prefab: {guid: %s, type: 3}\n" % ("a" * 32))
        good = make_pack(packs / "good" / "InnerverseInteractive")
        mani = ap.generate_manifest(pkproj, good)
        check([e["path"].split("/")[-1] for e in mani["required"]] == ["a.prefab", "c.png"]
              and mani["packageVersion"] == "1.3.0" and mani["assetStoreProductId"] == "176906",
              "manifest = GUID-reference closure only (the unreferenced file and bytes are not pinned)")
        mpath = scratch / "mani.json"
        mpath.write_text(json.dumps(mani, ensure_ascii=False))
        mdoc = ap.load_manifest(mpath)
        check(ap.verify_pack(good, mdoc)["requiredFiles"] == 2, "matching pack verifies; extra pack files are allowed")
        for kind, needle in (("reqbytes", "different bytes"), ("missing", "missing"), ("guid", "different GUID")):
            bad = make_pack(packs / kind / "InnerverseInteractive", tamper=kind)
            try:
                ap.verify_pack(bad, mdoc)
                failed = None
            except ap.ArtPackError as error:
                failed = str(error)
            check(failed is not None and "does not match" in failed and needle in failed, "wrong pack refused: " + kind)
        unreq = make_pack(packs / "unrequired" / "InnerverseInteractive", tamper="unrequired")
        check(ap.verify_pack(unreq, mdoc)["requiredFiles"] == 2, "a change to a file the project does not need does not fail the preflight")
        check(ap.digest_lines(["x"]) != ap.digest_lines(["y"]), "digest helper")
        tampered = dict(mani, requiredSetSha256="0" * 64)
        (scratch / "tampered.json").write_text(json.dumps(tampered, ensure_ascii=False))
        try:
            ap.load_manifest(scratch / "tampered.json"); inconsistent = False
        except ap.ArtPackError:
            inconsistent = True
        check(inconsistent, "internally inconsistent manifest refused")

        def mock_project(path):
            (path / "Assets/ForestPrototype").mkdir(parents=True)
            (path / "Assets/Scenes").mkdir(parents=True)
            (path / "Assets/Scenes/x.unity").write_text("m_Prefab: {guid: %s, type: 3}\n" % ("a" * 32))
            (path / "Tools/Build/S1B").mkdir(parents=True)
            (path / "Tools/Verification/S1B").mkdir(parents=True)
            shutil.copy2(S1B / "LinuxPlayerBuilder.cs", path / "Tools/Build/S1B/LinuxPlayerBuilder.cs")
            shutil.copy2(mpath, path / "Tools/Build/S1B/art-pack-manifest.json")
            (path / ".gitignore").write_text("/Assets/InnerverseInteractive/\n/Assets/InnerverseInteractive.meta\n")
            subprocess.run(["git", "-C", str(path), "init", "-q"], check=True)
            for k, v in [("user.email", "t@invalid"), ("user.name", "t"), ("commit.gpgsign", "false"), ("core.hooksPath", "/dev/null")]:
                subprocess.run(["git", "-C", str(path), "config", k, v], check=True)
            subprocess.run(["git", "-C", str(path), "add", "-A"], check=True)
            subprocess.run(["git", "-C", str(path), "commit", "-q", "-m", "seed"], check=True)
            return path

        proj2 = mock_project(scratch / "proj2")
        out2 = scratch / "out2"
        base2 = ["--project", str(proj2), "--unity", str(mock), "--output-root", str(out2), "--date", "20261016", "--sequence", "L01"]
        envk = dict(os.environ, MOCK_MODE="ok")
        envk.pop("CCF_ART_PACK_DIR", None)
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base2], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "required but not available" in p.stderr and "--art-pack" in p.stderr
              and not out2.exists() and not (proj2 / "Assets/InnerverseInteractive").exists(),
              "missing pack stops the build before output/staging with a clear message")
        wrong = make_pack(packs / "wrongpack" / "InnerverseInteractive", tamper="reqbytes")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base2, "--art-pack", str(wrong)], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "does not match" in p.stderr and not out2.exists(), "wrong pack stops the build")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base2, "--art-pack", str(good)], capture_output=True, text=True, env=envk)
        check(p.returncode == 0 and '"status": "PASS"' in p.stdout, "explicit --art-pack builds")
        installed = sorted(x.relative_to(proj2 / "Assets/InnerverseInteractive").as_posix()
                           for x in (proj2 / "Assets/InnerverseInteractive").rglob("*") if x.is_file())
        check(installed == ["Ultimate Nature \u2013 Starter/Env/a.prefab", "Ultimate Nature \u2013 Starter/Env/a.prefab.meta",
                            "Ultimate Nature \u2013 Starter/Env/c.png", "Ultimate Nature \u2013 Starter/Env/c.png.meta"],
              "only the required files and their .meta are installed")
        check(subprocess.check_output(["git", "-C", str(proj2), "status", "--porcelain"], text=True) == "",
              "installed pack is gitignored; the committed tree stays clean")
        built2 = next(out2.glob("*-Linux-x64"))
        ident2 = json.loads((built2 / "build-identity.json").read_text())
        pm2 = json.loads((built2 / "package-manifest.json").read_text())
        ap2 = ident2["artPack"]
        check(ap2["verified"] is True and ap2["requiredSetSha256"] == mani["requiredSetSha256"] and ap2["packageVersion"] == "1.3.0"
              and ap2["assetStoreProductId"] == "176906" and ap2["committedToRepository"] is False
              and pm2["identity"]["artPack"] == ap2 and ap2["fullContent"]["matchesManifest"] is True,
              "build identity + package manifest record the verified pack (version, product, checksum)")
        check(ident2["toolingSource"]["files"]["Tools/Build/S1B/art-pack-manifest.json"] == ap.sha256_file(proj2 / "Tools/Build/S1B/art-pack-manifest.json"),
              "tooling source records the committed manifest hash")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base2, "--sequence", "L02"], capture_output=True, text=True, env=envk)
        check(p.returncode == 0, "already-installed pack is verified in place (no --art-pack needed)")
        (proj2 / "Assets/InnerverseInteractive/Ultimate Nature \u2013 Starter/Env/a.prefab").write_bytes(b"corrupt")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base2, "--sequence", "L03"], capture_output=True, text=True, env=envk)
        check(p.returncode != 0 and "installed in the project" in p.stderr and "different bytes" in p.stderr, "corrupted installed pack stops the build")

        # the committed manifest itself
        real = ap.load_manifest()
        text = (S1B / "art-pack-manifest.json").read_text(encoding="utf-8")
        guids = {e["path"].split("/")[-1]: e["guid"] for e in real["required"]}
        check(real["name"].startswith("Ultimate Nature") and real["packageVersion"] and real["assetStoreProductId"].isdigit()
              and guids.get("UNS_Mushroom_Patch.prefab") == "d0fa3c49b2c9cd84bb5a644c53ef025e"
              and guids.get("UNS_Stump.prefab") == "3aa54be08e092174ab6b689e524fdf44" and len(real["required"]) >= 2,
              "committed manifest: pack identity and the documented prefab GUIDs")
        check("/home/" not in text and "/media/" not in text and "/tmp/" not in text, "committed manifest holds no machine-specific path")

        # 10. smoke launcher — shared-output protocol, stale-evidence and PASS/FAIL
        player = scratch / "mock-player"
        # Faithful harness model: SAVE writes saved-world-hash.txt into --ccf-smoke-output;
        # LOAD reads that SAME location and fails if it is absent (as StandalonePlayerSmoke does).
        mock_script(player, (
            "import os,sys\n"
            "a=sys.argv\n"
            "d=a[a.index('--ccf-smoke-output')+1]; os.makedirs(d,exist_ok=True)\n"
            "log=a[a.index('-logFile')+1]\n"
            "if os.environ.get('MOCK_FATAL')=='1':\n"
            "    open(log,'w').write('S1B_SMOKE_FAIL boom\\n'); sys.exit(1)\n"
            "if '--ccf-smoke-save' in a:\n"
            "    open(os.path.join(d,'saved-world-hash.txt'),'w').write('HASH\\n')\n"
            "    open(log,'w').write('S1B_PLAYER_SAVE_PHASE_PASS ok\\n'); sys.exit(0)\n"
            "if os.path.exists(os.path.join(d,'saved-world-hash.txt')):\n"
            "    open(log,'w').write('S1B_PLAYER_LOAD_RESET_PHASE_PASS ok\\n'); sys.exit(0)\n"
            "open(log,'w').write('S1B_SMOKE_FAIL missing saved-world-hash.txt\\n'); sys.exit(1)\n"))

        def smoke_out_of(cmd):
            return cmd[cmd.index("--ccf-smoke-output") + 1]
        pd = scratch / "ev0"
        c_save, _ = rs.phase_command(player, rs.smoke_output_dir(pd), pd / "s.log", "save", {})
        c_load, _ = rs.phase_command(player, rs.smoke_output_dir(pd), pd / "l.log", "load", {})
        check("--ccf-smoke-save" in c_save and c_save[0] == str(player), "smoke save command")
        check("--ccf-smoke-load" in c_load, "smoke load command")
        check(smoke_out_of(c_save) == smoke_out_of(c_load)
              and smoke_out_of(c_save).endswith("/smoke"),
              "SAVE and LOAD share one --ccf-smoke-output (regression vs 335d4fb)")

        ev = scratch / "ev"
        p = subprocess.run([sys.executable, str(S1B / "run_linux_player_smoke.py"), "--player", str(player),
                            "--evidence-root", str(ev), "--label", "smoke-ok", "--display", ":0"],
                           capture_output=True, text=True)
        check(p.returncode == 0 and (ev / "smoke-ok/smoke-summary.json").is_file()
              and '"overall": "PASS"' in (ev / "smoke-ok/smoke-summary.json").read_text(), "smoke both phases PASS (shared dir)")

        # Stale evidence cannot carry over: the same label is a fresh, refused directory.
        before = (ev / "smoke-ok/smoke-summary.json").read_text()
        p = subprocess.run([sys.executable, str(S1B / "run_linux_player_smoke.py"), "--player", str(player),
                            "--evidence-root", str(ev), "--label", "smoke-ok", "--display", ":0"],
                           capture_output=True, text=True)
        check(p.returncode != 0 and (ev / "smoke-ok/smoke-summary.json").read_text() == before,
              "stale label refused (fresh run dir, prior evidence unchanged)")

        p = subprocess.run([sys.executable, str(S1B / "run_linux_player_smoke.py"), "--player", str(player),
                            "--evidence-root", str(ev), "--label", "smoke-bad", "--display", ":0", "--phase", "save"],
                           capture_output=True, text=True, env=dict(os.environ, MOCK_FATAL="1"))
        check(p.returncode == 1 and "S1B_SMOKE_FAIL" in (ev / "smoke-bad/smoke-summary.json").read_text(), "smoke fatal detected FAIL")
        p = subprocess.run([sys.executable, str(S1B / "run_linux_player_smoke.py"), "--player", str(player),
                            "--evidence-root", str(ev), "--label", "smoke-nodisp", "--display", ""],
                           capture_output=True, text=True)
        check(p.returncode != 0 and "DISPLAY is required" in p.stderr, "no DISPLAY refused")

        print(f"\nALL {checks} CHECKS PASS\nscratch: {scratch}")
        return 0
    finally:
        shutil.rmtree(scratch, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
