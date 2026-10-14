#!/usr/bin/env python3
"""Run the INTERNAL Development Player with a measurement plan and sample the machine around it.

    python3 Tools/Verification/PlayerPerformance/run_dense_player.py --build D01 --series B1 --plan baseline --res 1280x720

Plans (see PLANS below) are written as JSON next to the results and read by DensePerfProbe inside the Player. Every launch gets a fresh isolated
XDG_CONFIG_HOME, so the real user's saves and PlayerPrefs are never touched. Results: Build/PlayerPerformance/<build>/<series>/<plan>-<res>/ with
report.json (the probe), per-window frame CSVs, Player.log, gpu.csv (nvidia-smi), threads.json and machine.json (sampled here), context-*.txt.

The machine is shared with whatever else the user is running; context-before/after record load, top processes, GPU clocks and temperatures so a
noisy run can be recognised and repeated rather than trusted.
"""
import argparse
import json
import os
import re
import shutil
import signal
import subprocess
import threading
import time
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
UNITY = "/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity"
CLK = os.sysconf("SC_CLK_TCK")

# The views are fixed camera placements so every condition is compared from the same viewpoint.
VIEWS = ["spawn", "spawn90", "spawn180", "spawn270", "centre0", "centre180", "edge0"]


def step(op, **kw):
    return dict(op=op, name=kw.get("name", ""), arg=kw.get("arg", ""), seconds=float(kw.get("seconds", 0)), f=float(kw.get("f", 0)), n=int(kw.get("n", 0)))


def plan_baseline(view, markers):
    # The GPU reaches its steady thermal state (about 87 C, clocks down about 14%) after roughly 90 s of load, so the views are measured after a
    # warm-up of the same scene and the first view is repeated at the end to show what is left of the drift.
    steps = [step("markers", arg=markers), step("start_new"), step("settle", seconds=4), step("view", arg="spawn"), step("window", name="warmup", seconds=90)]
    for v in VIEWS:
        steps += [step("view", arg=v), step("settle", seconds=2), step("window", name="view_" + v, seconds=15)]
    steps += [step("view", arg="spawn"), step("settle", seconds=2), step("window", name="view_spawn_repeat", seconds=15),
              step("view", arg="spawn:40"), step("settle", seconds=2), step("window", name="view_spawn_pitch40_ground", seconds=15),
              step("view", arg="spawn:-25"), step("settle", seconds=2), step("window", name="view_spawn_pitch-25_up", seconds=15),
              step("view", arg="spawn"), step("settle", seconds=2), step("walk", name="walk", seconds=20), step("memory", name="after_walk"), step("capture", name="screenshot")]
    return steps


def plan_discover(view, markers):
    return [step("start_new"), step("settle", seconds=6), step("discover"), step("window", name="discovery_frames", seconds=3)]


def plan_marks(view, markers):
    steps = [step("markers", arg=markers), step("start_new"), step("settle", seconds=4), step("view", arg=view), step("window", name="warmup", seconds=60)]
    # With any mark a frame takes about a second at 1344 trees, so those windows are long enough for a handful of frames and the direct method timings use few calls.
    for n in [0, 1, 2, 5, 10, 20, 40]:
        calls = 20 if n == 0 else 3
        steps += [step("marks", n=n, arg="fell"), step("settle", seconds=1), step("time_marking", name=f"fell{n}", n=calls), step("window", name=f"marks_fell_{n}", seconds=10 if n == 0 else 30)]
    for n in [5, 20, 40]:
        steps += [step("marks", n=n, arg="cropfell"), step("settle", seconds=1), step("time_marking", name=f"cropfell{n}", n=3), step("window", name=f"marks_cropfell_{n}", seconds=30)]
    steps += [step("marks", n=0, arg="fell"), step("settle", seconds=2), step("window", name="marks_zero_repeat", seconds=10)]
    return steps


def plan_ab(view, markers):
    """Each condition is bracketed by baseline windows (B C B C B ...): the drift is slow and close to linear, so the mean of the two baselines
    on either side of a condition is the fair comparison. A warm-up first brings the GPU to its steady thermal state."""
    conditions = [("ssao-off", 0), ("shadows-off", 0), ("shadow-distance", 25), ("shadow-cascades", 2), ("shadow-resolution", 1024), ("soft-shadows-off", 0),
                  ("lod-bias", 1), ("lod-bias", 3), ("render-scale", 0.75), ("msaa-off", 0)]
    steps = [step("markers", arg=markers), step("start_new"), step("settle", seconds=4), step("view", arg=view), step("window", name="warmup", seconds=90), step("window", name="ab_base_0", seconds=10)]
    for index, (what, value) in enumerate(conditions):
        label = what + (f"={value:g}" if value else "")
        steps += [step("toggle", arg=what, f=value), step("settle", seconds=2), step("window", name="ab_" + label, seconds=10), step("reset"), step("settle", seconds=2),
                  step("window", name=f"ab_base_{index + 1}", seconds=10)]
    steps += [step("toggle", arg="ssao-off"), step("toggle", arg="shadows-off"), step("settle", seconds=2), step("window", name="ab_ssao-off+shadows-off", seconds=10), step("reset"), step("settle", seconds=2),
              step("window", name=f"ab_base_{len(conditions) + 1}", seconds=10),
              step("hud", arg="off"), step("settle", seconds=2), step("window", name="ab_hud-off", seconds=10), step("hud", arg="on"), step("settle", seconds=2),
              step("window", name=f"ab_base_{len(conditions) + 2}", seconds=10)]
    return steps


def plan_ops(view, markers):
    return [step("markers", arg=markers), step("start_new"), step("settle", seconds=8), step("memory", name="start_settled"),
            step("advance_year", name="annual_restored", n=5, arg="restore"), step("settle", seconds=3),
            step("save_load", name="save_load", n=5), step("restart", name="restart_1"), step("restart", name="restart_2"),
            step("settle", seconds=3), step("memory", name="end")]


def plan_profile(view, markers):
    return [step("markers", arg=markers), step("start_new"), step("settle", seconds=4), step("view", arg=view), step("window", name="warmup", seconds=60),
            step("profiler", name="profile_marks0.raw", seconds=6), step("marks", n=10, arg="fell"), step("settle", seconds=1),
            step("profiler", name="profile_marks10.raw", seconds=6), step("marks", n=40, arg="fell"), step("settle", seconds=1),
            step("profiler", name="profile_marks40.raw", seconds=6)]


PLANS = dict(baseline=plan_baseline, discover=plan_discover, marks=plan_marks, ab=plan_ab, ops=plan_ops, profile=plan_profile)


def sh(cmd):
    return subprocess.run(cmd, shell=True, capture_output=True, text=True).stdout


def context(path):
    text = ["date: " + time.strftime("%Y-%m-%dT%H:%M:%S%z"), "loadavg: " + Path("/proc/loadavg").read_text().strip(), "uptime: " + sh("uptime").strip(),
            "--- top processes by CPU", sh("ps -eo pid,pcpu,pmem,rss,etime,comm --sort=-pcpu | head -9"),
            "--- gpu", sh("nvidia-smi --query-gpu=name,driver_version,pstate,clocks.sm,clocks.mem,clocks.max.sm,temperature.gpu,utilization.gpu,power.draw,memory.used,clocks_throttle_reasons.active --format=csv"),
            "--- gpu processes", sh("nvidia-smi --query-compute-apps=pid,name,used_memory --format=csv; nvidia-smi pmon -c 1 2>/dev/null | head -12"),
            "--- memory", sh("free -m"), "--- cpu freq (MHz)", sh("grep MHz /proc/cpuinfo"), "--- thermal",
            sh("for z in /sys/class/thermal/thermal_zone*; do echo $(cat $z/type) $(cat $z/temp); done"), "--- governor", sh("cat /sys/devices/system/cpu/cpu0/cpufreq/scaling_governor"),
            "--- display", sh("xrandr --current 2>/dev/null | head -3"), "--- active window", sh("xdotool getactivewindow getwindowname 2>/dev/null")]
    Path(path).write_text("\n".join(text))


def thread_sampler(pid, stop, samples):
    previous = {}
    while not stop.is_set():
        row = dict(t=time.time())
        threads = {}
        try:
            for task in Path(f"/proc/{pid}/task").iterdir():
                stat = (task / "stat").read_text()
                name = re.search(r"\((.*)\)", stat).group(1)
                fields = stat.rsplit(")", 1)[1].split()
                ticks = int(fields[11]) + int(fields[12])          # utime + stime
                key = task.name
                if key in previous:
                    threads[name + ":" + key] = (ticks - previous[key]) / CLK
                previous[key] = ticks
            status = Path(f"/proc/{pid}/status").read_text()
            row.update({k: int(v) * 1024 for k, v in re.findall(r"^(VmRSS|VmHWM):\s+(\d+) kB", status, re.M)})
        except (FileNotFoundError, ProcessLookupError):
            break
        row["threads_cpu_seconds_per_second"] = {k: round(v, 3) for k, v in threads.items() if v > 0.01}
        row["loadavg"] = Path("/proc/loadavg").read_text().split()[:3]
        samples.append(row)
        stop.wait(1.0)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--build", required=True)
    parser.add_argument("--series", required=True)
    parser.add_argument("--plan", required=True, choices=sorted(PLANS))
    parser.add_argument("--res", default="1280x720")
    parser.add_argument("--view", default="spawn", help="the view used by the marks, ab and profile plans")
    parser.add_argument("--markers", default=None, help="marker list file (category|name|scale per line), relative to the series folder or absolute")
    parser.add_argument("--timeout", type=int, default=1500)
    args = parser.parse_args()
    assert re.fullmatch(r"\d+x\d+", args.res) and re.fullmatch(r"[A-Za-z0-9_-]+", args.series)
    build = ROOT / "Build/PlayerPerformance" / args.build
    receipt = json.loads((build / "build-receipt.json").read_text())
    assert receipt["status"] == "PASS" and receipt["development"]
    binary = Path(receipt["binary"])
    run_dir = build / args.series / f"{args.plan}-{args.res}"
    run_dir.mkdir(parents=True, exist_ok=False)
    width, height = args.res.split("x")
    markers = args.markers
    if markers:
        markers = str(Path(markers).resolve())
        shutil.copy2(markers, run_dir / "markers.txt")
    steps = PLANS[args.plan](args.view, markers or "")
    if args.plan != "discover" and not markers:
        steps = [s for s in steps if s["op"] != "markers"]
    (run_dir / "plan.json").write_text(json.dumps(dict(steps=steps), indent=1))
    assert not subprocess.run(["pgrep", "-f", "^" + UNITY + " "], capture_output=True, text=True).stdout.strip(), "a Unity Editor is running"
    config = run_dir / "config"
    config.mkdir()
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), DISPLAY=os.environ.get("DISPLAY", ":0"))
    for key in ["CCF_STAND_GEOMETRY", "CCF_RNG_MODEL", "CCF_REGEN_MODEL", "CCF_GROWTH_MODEL", "CCF_STARTUP_SCREEN"]:
        env.pop(key, None)
    context(run_dir / "context-before.txt")
    gpu_log = (run_dir / "gpu.csv").open("w")
    gpu = subprocess.Popen(["nvidia-smi", "--query-gpu=timestamp,clocks.sm,clocks.mem,clocks.max.sm,temperature.gpu,utilization.gpu,utilization.memory,power.draw,memory.used,clocks_throttle_reasons.active",
                            "--format=csv", "-lms", "1000"], stdout=gpu_log, stderr=subprocess.STDOUT)
    cmd = [str(binary), "-screen-width", width, "-screen-height", height, "-screen-fullscreen", "0", "-logFile", str(run_dir / "Player.log"),
           "--ccf-perf-out", str(run_dir), "--ccf-perf-plan", str(run_dir / "plan.json")]
    start = time.monotonic()
    player = subprocess.Popen(cmd, cwd=binary.parent, env=env, stdout=(run_dir / "console.log").open("w"), stderr=subprocess.STDOUT)
    stop = threading.Event()
    samples = []
    sampler = threading.Thread(target=thread_sampler, args=(player.pid, stop, samples), daemon=True)
    sampler.start()
    # Keep the Player's window in front so another application's focus change cannot throttle or hide it.
    activated = False
    code = None
    while True:
        code = player.poll()
        if code is not None:
            break
        elapsed = time.monotonic() - start
        if not activated and elapsed > 6:
            win = sh(f"xdotool search --pid {player.pid} --onlyvisible 2>/dev/null | tail -1").strip()
            if win:
                subprocess.run(["xdotool", "windowactivate", win], capture_output=True)
                activated = True
        if elapsed > args.timeout:
            player.terminate()
            try:
                player.wait(timeout=30)
            except subprocess.TimeoutExpired:
                player.kill()
            code = 124
            break
        time.sleep(0.5)
    stop.set()
    sampler.join(timeout=5)
    gpu.send_signal(signal.SIGINT)
    gpu.wait(timeout=10)
    gpu_log.close()
    context(run_dir / "context-after.txt")
    (run_dir / "threads.json").write_text(json.dumps(samples))
    report_path = run_dir / "report.json"
    status = json.loads(report_path.read_text())["status"] if report_path.exists() else "NO_REPORT"
    result = dict(plan=args.plan, res=args.res, exit_code=code, status=status, seconds=round(time.monotonic() - start, 1),
                  peak_rss_gib=round(max((s.get("VmHWM", 0) for s in samples), default=0) / 2**30, 3), run_dir=str(run_dir))
    (run_dir / "run-result.json").write_text(json.dumps(result, indent=2))
    print(json.dumps(result), flush=True)
    raise SystemExit(0 if code == 0 and status == "PASS" else 1)


if __name__ == "__main__":
    main()
