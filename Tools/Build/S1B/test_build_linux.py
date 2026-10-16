#!/usr/bin/env python3
"""Bounded, Unity-free tests for the Linux Player build/smoke tooling.

Uses a disposable mock project + mock Unity/Player executables under ZX20/CCF-Temp.
Never launches Unity and never fabricates a real Linux build: it only exercises the
orchestrator's control flow, path/id generation, output validation and failure paths.

Run:  python3 Tools/Build/S1B/test_build_linux.py
Exit 0 = all checks PASS.
"""
import importlib.util
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
        base = ["--project", str(proj), "--unity", str(mock), "--output-root", str(outroot), "--date", "20261016", "--sequence", "L01"]

        # 4. success
        env = dict(os.environ, MOCK_MODE="ok")
        p = subprocess.run([sys.executable, str(S1B / "build_linux.py"), *base], capture_output=True, text=True, env=env)
        check(p.returncode == 0 and '"status": "PASS"' in p.stdout, "success path PASS")
        proj_sha = subprocess.check_output(["git", "-C", str(proj), "rev-parse", "HEAD"], text=True).strip()
        built = outroot / (bl.build_id_for("20261016", "L01", proj_sha, False) + "-Linux-x64")
        check((built / "CCF.x86_64").is_file() and os.access(built / "CCF.x86_64", os.X_OK)
              and (built / "CCF_Data").is_dir() and (built / "UnityPlayer.so").is_file(), "linux layout validated")
        check((built / "package-manifest.json").is_file() and (built / "build-identity.json").is_file(), "manifest + identity written")

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
